#!/usr/bin/env python3
"""Second-stage semantic guard for the Objective 1 CTA source check (#328).

The lexical checker (objective_cta_source_check.py) cannot know what a C# name *means*: a tuple-typed
member called `Time`, or a type declared in a file the collector never reads, changes what
`Time.frameCount` binds to while the text still looks right. This module asks the C# compiler instead.

It compiles the player-like source set (every Assets/**/*.cs outside Editor folders, C# 9, the Player
defines below) with Roslyn against 320 hash-pinned Unity reference DLLs, then judges the symbols that
ObjectiveCtaRoslynBinding.cs reports for ScenarioOneUiRoot:
  1. exactly one ScenarioOneUiRoot, declared only in ScenarioOneUiRoot.cs, derived from the metadata
     type UnityEngine.MonoBehaviour;
  2. every `Time.<member>` receiver in it is the metadata type UnityEngine.Time and every member is a
     property of that type, with the pinned counts (frameCount 3, unscaledTime 2);
  3. every `DisplayStyle.<member>` receiver is the metadata enum UnityEngine.UIElements.DisplayStyle
     and every member is an enum field of it, with the pinned counts (Flex 9, None 13).
Everything else FAILS CLOSED: wrong or missing SDK, Roslyn or reference file, compile errors, an unexpected
source topology, a missing, extra, unresolved or ambiguous site, or tool output that cannot be read.

This is an offline source approximation. It is NOT Unity compilation and proves nothing about the native
UI. Roslyn here is the .NET 8.0.133 SDK compiler at C# 9; Unity's own compiler, analyzers and generators
are not run. Run `python3 -B objective_cta_roslyn_binding.py` from a checkout to print the evidence.
"""
import hashlib
import json
import os
import re
import subprocess
import sys
import tempfile
from collections import Counter
from pathlib import Path
from xml.sax.saxutils import escape

from objective_cta_source_check import ROOT as ROOT_FILE, ObjectiveCtaSourceError

HERE = Path(__file__).resolve().parent
TOOL_SOURCE = HERE / "ObjectiveCtaRoslynBinding.cs"
MANIFEST = HERE / "roslyn-reference-manifest.txt"

# ---- pinned toolchain identity -------------------------------------------------------------------------
SDK_VERSION = "8.0.133"
ROSLYN_DLL_SHA256 = {  # the compiler the SDK above ships, SHA256 observed on this machine
    "Microsoft.CodeAnalysis.dll": "124939a14172f874777c37405e9fc9aefa8a5fda301a15f7feea3d96ca822346",
    "Microsoft.CodeAnalysis.CSharp.dll": "e91e2d1b62e5cf3b54369cab3a2d4055a7bcf9769c3a3182a65ae6a7e0941977",
}
MANIFEST_SHA256 = "978123241d3d6ff18973a0e9d486c0e20f4da30f5c2c7c5d3e2947c6fb3a18f1"
EXPECTED_REFERENCES = 320
# Read-only copies of the manifest's DLLs, named NNN-<file name> (NNN = manifest line number, zero based).
REFERENCE_DIR_ENV = "CCF_CTA_ROSLYN_REFERENCES"
DEFAULT_REFERENCE_DIR = Path("/media/jer/Files/CCF/Verification/CTA325-CompanionNameRepair-20261107/refs")
# Player-like symbols: the Unity 6000.6 standalone-Linux Player with the New Input System, no Editor.
DEFINES = ("UNITY_6000_6_OR_NEWER;UNITY_6000_0_OR_NEWER;UNITY_2023_1_OR_NEWER;UNITY_2022_1_OR_NEWER;"
           "UNITY_2021_1_OR_NEWER;UNITY_2020_1_OR_NEWER;ENABLE_INPUT_SYSTEM;UNITY_STANDALONE;UNITY_STANDALONE_LINUX")

# ---- pinned reviewed facts -----------------------------------------------------------------------------
ROOT_TYPE = "ScenarioOneUiRoot"
BASE_TYPE = ("UnityEngine.MonoBehaviour", "UnityEngine.CoreModule")
# receiver spelling -> (resolved metadata type, assembly, kind of the member symbol)
RECEIVERS = {
    "Time": ("UnityEngine.Time", "UnityEngine.CoreModule", "Property"),
    "DisplayStyle": ("UnityEngine.UIElements.DisplayStyle", "UnityEngine.UIElementsModule", "Field"),
}
# (receiver spelling, member) -> exact number of member accesses in ScenarioOneUiRoot.cs. A different
# number, or any other member on these receivers, is a changed or hidden site and must be reviewed.
EXPECTED_SITES = {("Time", "frameCount"): 3, ("Time", "unscaledTime"): 2,
                  ("DisplayStyle", "Flex"): 9, ("DisplayStyle", "None"): 13}
# Unity compiles these folders into other assemblies, and an assembly definition splits Assembly-CSharp:
# the single-assembly model below would then be wrong, so such a tree is refused rather than approximated.
_SPLIT_FILES = re.compile(r"\.(asmdef|asmref|rsp)$", re.I)
_FIRST_PASS_FOLDERS = {"Plugins", "Standard Assets", "Pro Standard Assets"}


class ObjectiveCtaBindingError(ObjectiveCtaSourceError):
    """The semantic stage could not demonstrate a binding (an unmet rule, or tooling that cannot run)."""


def _sha256(path):
    return hashlib.sha256(Path(path).read_bytes()).hexdigest()


# ---------------------------------------------------------------- toolchain and references (fail closed)

def reference_files(reference_dir=None):
    """The 320 pinned DLL paths, in manifest order, after proving manifest and every file by SHA256."""
    manifest_hash = _sha256(MANIFEST)
    if manifest_hash != MANIFEST_SHA256:
        raise ObjectiveCtaBindingError(f"reference manifest hash {manifest_hash} != pinned {MANIFEST_SHA256}")
    rows = [line.split("  ", 1) for line in MANIFEST.read_text(encoding="utf-8").splitlines() if line.strip()]
    if len(rows) != EXPECTED_REFERENCES or any(len(row) != 2 for row in rows):
        raise ObjectiveCtaBindingError(f"reference manifest has {len(rows)} usable rows, expected {EXPECTED_REFERENCES}")
    folder = Path(reference_dir or os.environ.get(REFERENCE_DIR_ENV) or DEFAULT_REFERENCE_DIR)
    if not folder.is_dir():
        raise ObjectiveCtaBindingError(f"reference folder missing: {folder} (set {REFERENCE_DIR_ENV})")
    found = []
    for index, (expected, original) in enumerate(rows):
        path = folder / f"{index:03d}-{Path(original).name}"
        if path.is_symlink() or not path.is_file():
            raise ObjectiveCtaBindingError(f"reference file missing or not a regular file: {path}")
        actual = _sha256(path)
        if actual != expected:
            raise ObjectiveCtaBindingError(f"reference hash mismatch for {path.name}: {actual} != manifest {expected}")
        found.append(str(path))
    extras = sorted(set(os.listdir(folder)) - {Path(p).name for p in found})
    if extras:
        raise ObjectiveCtaBindingError(f"reference folder holds {len(extras)} file(s) outside the manifest, e.g. {extras[0]}")
    return found


def _dotnet_env(home):
    return dict(os.environ, DOTNET_CLI_TELEMETRY_OPTOUT="1", DOTNET_NOLOGO="1", DOTNET_CLI_HOME=str(home),
                UseSharedCompilation="false", MSBUILDDISABLENODEREUSE="1")


def _run(command, cwd, env, what, timeout=300):
    try:
        done = subprocess.run(command, cwd=str(cwd), env=env, capture_output=True, text=True, timeout=timeout)
    except (OSError, subprocess.TimeoutExpired) as error:
        raise ObjectiveCtaBindingError(f"{what} could not run ({type(error).__name__}: {error})") from error
    return done


class Toolchain:
    """A built ObjectiveCtaRoslynBinding.dll plus the identity facts the evidence rows quote."""

    def __init__(self, work, dll, refs_file, references, facts, env):
        self.work, self.dll, self.refs_file, self.references, self.facts, self.env = work, dll, refs_file, references, facts, env

    def report(self, source_root, overlay_root=None):
        """Run the reporter and return its parsed JSON, or raise ObjectiveCtaBindingError."""
        out = Path(self.work) / "report.json"
        if out.exists():
            out.unlink()
        command = ["dotnet", str(self.dll), str(source_root), str(self.refs_file), DEFINES, str(out)]
        if overlay_root:
            command.append(str(overlay_root))
        done = _run(command, self.work, self.env, "Roslyn reporter")
        if done.returncode != 0:
            raise ObjectiveCtaBindingError(f"Roslyn reporter exited {done.returncode}: {(done.stderr or done.stdout).strip()[:300]}")
        try:
            document = json.loads(out.read_text(encoding="utf-8"))
            if document["language"] != "CSharp9" or document["referenceCount"] != EXPECTED_REFERENCES:
                raise ValueError("reporter ran with a different language level or reference count")
            document["results"][0]["label"]
            return document
        except (OSError, ValueError, KeyError, IndexError, TypeError) as error:
            raise ObjectiveCtaBindingError(f"Roslyn reporter output unreadable ({type(error).__name__}: {error})") from error


def build_toolchain(work, reference_dir=None, sdk_version=SDK_VERSION, roslyn_hashes=None):
    """Pin SDK 8.0.133 by a global.json in `work`, verify the compiler DLLs and references, build the reporter.

    The SDK `dotnet` runs is chosen by the working directory, so every dotnet call here uses `work`
    (and its global.json) whatever directory the caller is in. No NuGet package is restored."""
    work = Path(work)
    references = reference_files(reference_dir)
    env = _dotnet_env(work / "dotnet-home")
    (work / "global.json").write_text(json.dumps({"sdk": {"version": sdk_version, "rollForward": "disable"}}) + "\n")
    version = _run(["dotnet", "--version"], work, env, "dotnet --version")
    if version.returncode != 0 or version.stdout.strip() != sdk_version:
        raise ObjectiveCtaBindingError(
            f".NET SDK {sdk_version} is required but dotnet reported {(version.stdout or version.stderr).strip()[:120]!r}")
    listing = _run(["dotnet", "--list-sdks"], work, env, "dotnet --list-sdks")
    match = re.search(r"^" + re.escape(sdk_version) + r" \[(.+)\]$", listing.stdout, re.M)
    if not match:
        raise ObjectiveCtaBindingError(f".NET SDK {sdk_version} is not listed by dotnet --list-sdks")
    compiler = Path(match.group(1)) / sdk_version / "Roslyn" / "bincore"
    hashes = ROSLYN_DLL_SHA256 if roslyn_hashes is None else roslyn_hashes
    for name, expected in hashes.items():
        if not (compiler / name).is_file() or _sha256(compiler / name) != expected:
            raise ObjectiveCtaBindingError(f"Roslyn compiler file {compiler / name} is missing or differs from the pinned SHA256")
    refs_file = work / "references.txt"
    refs_file.write_text("".join(path + "\n" for path in references), encoding="utf-8")
    (work / "tool.csproj").write_text(
        '<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType>'
        '<TargetFramework>net8.0</TargetFramework><Nullable>disable</Nullable><ImplicitUsings>disable</ImplicitUsings>'
        '<EnableDefaultCompileItems>false</EnableDefaultCompileItems><UseAppHost>false</UseAppHost></PropertyGroup><ItemGroup>'
        f'<Compile Include="{escape(str(TOOL_SOURCE))}" />'
        f'<Reference Include="Microsoft.CodeAnalysis"><HintPath>{escape(str(compiler / "Microsoft.CodeAnalysis.dll"))}</HintPath></Reference>'
        f'<Reference Include="Microsoft.CodeAnalysis.CSharp"><HintPath>{escape(str(compiler / "Microsoft.CodeAnalysis.CSharp.dll"))}</HintPath></Reference>'
        '</ItemGroup></Project>')
    build = _run(["dotnet", "build", "tool.csproj", "-o", "out", "-v", "q", "--nologo"], work, env, "Roslyn reporter build")
    dll = work / "out" / "tool.dll"
    if build.returncode != 0 or not dll.is_file():
        raise ObjectiveCtaBindingError("Roslyn reporter build failed: " + (build.stdout + build.stderr).strip()[:300])
    facts = {"sdk": sdk_version, "references": len(references), "manifest": MANIFEST_SHA256}
    return Toolchain(work, dll, refs_file, references, facts, env)


# ------------------------------------------------------------------------------------- the rules

def topology_problems(source_root):
    """Source shapes that make a single Assembly-CSharp compile the wrong model of what Unity builds."""
    problems = []
    assets = Path(source_root) / "Assets"
    for path in sorted(assets.rglob("*")):
        parts = path.relative_to(source_root).parts
        if "Editor" in parts[:-1] or any(p.startswith(".") or p.endswith("~") for p in parts):
            continue  # Unity ignores these for the player assembly
        if path.is_file() and _SPLIT_FILES.search(path.name):
            problems.append(f"[topology] {'/'.join(parts)} splits or reconfigures the assembly; the single-assembly model is invalid")
        if path.is_file() and path.suffix == ".cs" and _FIRST_PASS_FOLDERS.intersection(parts[:-1]):
            problems.append(f"[topology] {'/'.join(parts)} is in a Unity first-pass folder compiled into another assembly")
    return problems


def _is_symbol(info, kind=None, display=None, assembly=None, containing=None):
    symbol = info.get("symbol")
    return bool(symbol and symbol["isMetadata"] and (kind is None or symbol["kind"] == kind)
                and (display is None or symbol["display"] == display)
                and (assembly is None or symbol["containingAssembly"] == assembly)
                and (containing is None or symbol["containingType"] == containing)
                and not symbol["isTupleElement"])


def _describe(info):
    symbol = info.get("symbol")
    if not symbol:
        names = ", ".join(c["display"] for c in info.get("candidates", [])) or "none"
        return f"unresolved or ambiguous ({info.get('candidateReason')}; candidates: {names})"
    place = f"metadata {symbol['containingAssembly']}" if symbol["isMetadata"] else "source in this project"
    tuple_note = ", tuple element" if symbol["isTupleElement"] else ""
    return f"{symbol['kind']} {symbol['display']} ({place}{tuple_note})"


def evaluate(result):
    """List every unmet rule for one reporter result (empty list = all rules met). Pure function of the JSON."""
    problems = []
    if result["errorCount"]:
        first = result["errors"][0] if result["errors"] else {}
        problems.append(f"[compile] the source set has {result['errorCount']} compile error(s) at C# 9 against the pinned "
                        f"references; first {first.get('id')} {first.get('file')}:{first.get('line')} {first.get('message')}")
    roots = result["rootTypes"]
    if len(roots) != 1:
        problems.append(f"[root] expected exactly one type named {ROOT_TYPE}, Roslyn found {len(roots)}: "
                        + (", ".join(f"{r['display']} in {','.join(r['declaringFiles'])}" for r in roots) or "none"))
    else:
        root = roots[0]
        if root["display"] != ROOT_TYPE or root["kind"] != "Class":
            problems.append(f"[root] {root['display']} ({root['kind']}) is not the global class {ROOT_TYPE}")
        if root["declaringFiles"] != [ROOT_FILE]:
            problems.append(f"[root] {ROOT_TYPE} is declared in {root['declaringFiles']}, expected only [{ROOT_FILE}]")
        base = root.get("baseType") or {}
        if not (base.get("isMetadata") and base.get("display") == BASE_TYPE[0] and base.get("containingAssembly") == BASE_TYPE[1]):
            problems.append(f"[base type] {ROOT_TYPE} derives from {base.get('display')} "
                            f"({'metadata ' + str(base.get('containingAssembly')) if base.get('isMetadata') else 'source in this project'}), "
                            f"not the metadata type {BASE_TYPE[0]} in {BASE_TYPE[1]}")
    counts = Counter()
    for site in result["sites"]:
        name, member = site["receiverName"], site["member"]
        counts[(name, member)] += 1
        type_name, assembly, member_kind = RECEIVERS[name]
        where = f"{site['file']}:{site['line']}"
        if not _is_symbol(site["receiver"], "NamedType", type_name, assembly):
            problems.append(f"[receiver] `{site['text']}` at {where}: receiver `{name}` binds to {_describe(site['receiver'])}, "
                            f"not the metadata type {type_name} in {assembly}")
        elif not _is_symbol(site["memberSymbol"], member_kind, containing=type_name):
            problems.append(f"[member] `{site['text']}` at {where}: member binds to {_describe(site['memberSymbol'])}, "
                            f"not a {member_kind.lower()} of the metadata type {type_name}")
    for key in sorted(set(counts) | set(EXPECTED_SITES)):
        if counts[key] != EXPECTED_SITES.get(key, 0):
            problems.append(f"[site count] {key[0]}.{key[1]}: Roslyn found {counts[key]} reviewed access(es) in {ROOT_FILE}, "
                            f"expected {EXPECTED_SITES.get(key, 0)}")
    for other in result["otherOccurrences"]:
        problems.append(f"[site] `{other['name']}` at {other['file']}:{other['line']} is used outside a pinned member access "
                        f"({other['parentKind']}: {other['text']})")
    return problems


def _summary(problems):
    shown = problems[:4]
    more = f" (+{len(problems) - len(shown)} more)" if len(problems) > len(shown) else ""
    return "Roslyn semantic binding unmet: " + " | ".join(shown) + more


def evidence_rows(result, facts):
    """Deterministic evidence for a passing result: toolchain identity, root base type and one row per pinned site group."""
    root = result["rootTypes"][0]
    rows = [(ROOT_FILE, 0, f"Roslyn {facts['sdk']} (C# 9, player-like defines), {facts['references']} manifest-pinned references "
                           f"verified by SHA256, manifest {facts['manifest'][:12]}; 0 compile errors; offline approximation, not Unity compilation"),
            (ROOT_FILE, 0, f"{ROOT_TYPE} : {root['baseType']['display']} [{root['baseType']['containingAssembly']}, metadata]")]
    groups = {}
    for site in result["sites"]:
        groups.setdefault((site["receiverName"], site["member"]), []).append(site)
    for (name, member), sites in sorted(groups.items()):
        symbol = sites[0]["receiver"]["symbol"]
        rows.append((ROOT_FILE, sites[0]["line"], f"{name}.{member} x{len(sites)} -> {symbol['display']} "
                                                  f"[{symbol['containingAssembly']}, metadata]"))
    return rows


def materialize_source_set(repo_root, destination):
    """Copy the checkout's player-relevant C# sources (Assets/**/*.cs outside Editor) into `destination`."""
    repo = Path(repo_root)
    for path in sorted((repo / "Assets").rglob("*.cs")):
        parts = path.relative_to(repo).parts
        if "Editor" in parts[:-1]:
            continue
        target = Path(destination).joinpath(*parts)
        target.parent.mkdir(parents=True, exist_ok=True)
        target.write_bytes(path.read_bytes())


def verify_objective_cta_binding(repo_root, toolchain=None, **toolchain_options):
    """Stage 2: return evidence rows when every semantic rule holds, else raise ObjectiveCtaBindingError.

    Any failure to run (missing SDK, reference or tool; unreadable output) is the same completed failure."""
    try:
        problems = topology_problems(repo_root)
        if problems:
            raise ObjectiveCtaBindingError(_summary(problems))
        with tempfile.TemporaryDirectory(prefix="ccf-cta-roslyn-") as directory:
            chain = toolchain or build_toolchain(directory, **toolchain_options)
            result = chain.report(repo_root)["results"][0]
            problems = evaluate(result)
            if problems:
                raise ObjectiveCtaBindingError(_summary(problems), rows=[(ROOT_FILE, 0, p) for p in problems[:8]])
            return evidence_rows(result, chain.facts)
    except ObjectiveCtaSourceError:
        raise
    except Exception as error:  # noqa: BLE001 - tooling that cannot run is a FAIL, never a pass or a crash
        raise ObjectiveCtaBindingError(f"Roslyn semantic binding could not run ({type(error).__name__}: {error})") from error


def main():
    repo_root = Path(sys.argv[1] if len(sys.argv) > 1 else HERE.parents[2]).resolve()
    try:
        with tempfile.TemporaryDirectory(prefix="ccf-cta-roslyn-") as directory:
            chain = build_toolchain(directory)
            result = chain.report(repo_root)["results"][0]
        print(f"files={result['fileCount']} sourceSetSha256={result['sourceSetSha256']} errors={result['errorCount']}")
        problems = evaluate(result) + topology_problems(repo_root)
        for line in problems:
            print("UNMET " + line)
        for path, line, text in (evidence_rows(result, chain.facts) if not problems else []):
            print(f"  {path}:{line}: {text}")
        return 1 if problems else 0
    except ObjectiveCtaSourceError as error:
        print("UNMET " + str(error))
        return 1


if __name__ == "__main__":
    raise SystemExit(main())
