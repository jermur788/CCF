#!/usr/bin/env python3
"""Type-check CCF C# sources without Unity, for code-only (cloud) sessions.

WHAT THIS IS: a compile of the runtime sources against community reference
assemblies for UnityEngine 2021.3 (NuGet `UnityEngine.Modules`) plus checked-in
stubs (InputSystemStub.cs, UnityEditorStub.cs, Painter2DStub.cs). It catches
syntax errors, missing members and wrong types in code that is edited without
Unity.

WHAT THIS IS NOT: it never runs the game, it is not Unity's compiler, and the
2021.3 API surface differs from Unity 6000.6. The known differences appear as a
constant set of errors recorded in baseline-*.txt. Only errors that are NOT in
the baseline are meaningful; a clean diff means "no new compile errors against
this approximation", never "Unity compiles it".

Painter2D: Unity 6's UI Toolkit vector API is absent from the 2021.3 reference
assemblies. Painter2DStub.cs supplies it, and the exact `context.painter2D`
expression is rewritten to a stub call in a temporary copy. Without that the
compiler stops at the declaration errors and skips every method body, so the
check would silently report nothing useful. The rewrite is deliberately narrow:
only that one known member access is changed.

Declaration-level errors still stop method-body binding. If any remain after the
run, the tool prints TYPECHECK_BLIND and exits 3 instead of claiming a clean
result; such an error must be fixed or excluded with a written reason, not
baselined.

Modes:
  prod   production sources under Assets/ (Editor folders excluded).
  tools  + UNITY_EDITOR defined and the Tools/Verification harnesses, so the
         disposable in-Editor gates can be checked too.

Needs the .NET SDK (8+) and, the first time, network access to nuget.org.
The generated project lives in a temporary directory outside the repository.

Usage: run_typecheck.py [--mode prod|tools] [--out DIR] [--update-baseline]
                        [--replace REPO_PATH FILE]
Exit status 0 when there are no errors beyond the baseline, 3 when a
declaration-level error makes method-body checking meaningless.
"""
import argparse
import glob
import os
import re
import shutil
import subprocess
import sys
import tempfile

HERE = os.path.dirname(os.path.abspath(__file__))
ROOT = os.path.abspath(os.path.join(HERE, "..", "..", ".."))

# Harnesses that inject low-level input need InputSystem.LowLevel, which is not stubbed.
TOOLS_EXCLUDE = [
    "Tools/Verification/**/OfflineLogic/**/*.cs",
    "Tools/Verification/MenuTutorialVerification.cs",
    "Tools/Verification/Startup/StartupScreenVerification.cs",
    "Tools/Verification/WindthrowV1/StormUiVerification.cs",
    # SCENARIO-ONE-UX-PEDAGOGY-V1 disposable Editor harnesses: they use Input System and Editor APIs this 2021.3 approximation does not stub
    # (InputSystem.QueueDeltaStateEvent / settings, Mouse.WarpCursorPosition, EditorWindow.position / maximized). Their real compile check is
    # Unity's own compiler against the project's references (Tools/Verification/ScenarioOneUxPedagogy/README.md).
    "Tools/Verification/ScenarioOneUxPedagogy/CameraLookDiagnosis.cs",
    "Tools/Verification/ScenarioOneUxPedagogy/UxPedagogyShots.cs",
    # SCENARIO-ONE-PLAYER-TEST-REMEDIATION-V1 combined-integration harness: Editor window reflection and Unity.Profiling recorders; checked by Unity's own compiler.
    "Tools/Verification/ScenarioOneCombined/CombinedHudComposition.cs",
    # Standalone offline-check programs. Each has its own runner that compiles a selected source
    # subset; several deliberately define global helper or production-stand-in types. Compiling
    # them together with production (or with each other) creates duplicate-type declaration errors,
    # which stop method-body binding for the whole run. Excluding them keeps the Editor harnesses
    # body-checked; the offline checks are still compiled and run by their own scripts.
    "Tools/Verification/RackV2Editing/OfflineCheck.cs",
    "Tools/Verification/RackV2Spacing/OfflineCheck.cs",
    # RackV2Editing, RackV2Spacing and RackV2ConfirmMinimap are copies of one harness and define
    # the same global PointerOp/PointerRequest helpers. Keep the first in the combined compile.
    "Tools/Verification/RackV2Spacing/RackV2SpacingVerification.cs",
    "Tools/Verification/RackV2ConfirmMinimap/RackV2ConfirmMinimapVerification.cs",
    "Tools/Verification/RackV2SuggestedLayout/OfflineCheck.cs",
    "Tools/Verification/S1CManagementHistory/HistoryOfflineStubs.cs",
    "Tools/Verification/S1CManagementHistory/S1CPerYearHistoryOfflineCheck.cs",
    "Tools/Verification/S1CManagementHistory/UnityShim.cs",
]

# Copies of one harness whose global helper types collide in a single all-tools compilation.
# The main pass keeps the first copy; each suffix is compiled in its own pass so its bodies are
# still checked instead of being silently dropped.
TOOLS_SUPPLEMENTS = [
    "Tools/Verification/RackV2Spacing/RackV2SpacingVerification.cs",
    "Tools/Verification/RackV2ConfirmMinimap/RackV2ConfirmMinimapVerification.cs",
]

# Unity 6 member access that the 2021.3 references lack. Only this exact call is rewritten.
PAINTER2D_ACCESS = re.compile(r"\bcontext\.painter2D\b")
# Error codes that make the compiler skip method-body binding. A clean baseline must contain none.
BLIND_CODES = ("CS0101", "CS0111", "CS0234", "CS0246", "CS0433", "CS0518", "CS0579")

PROJECT = """<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <OutputType>Library</OutputType>
    <LangVersion>9.0</LangVersion>
    <Nullable>disable</Nullable>
    <ImplicitUsings>disable</ImplicitUsings>
    <EnableDefaultCompileItems>false</EnableDefaultCompileItems>
    <GenerateAssemblyInfo>false</GenerateAssemblyInfo>
    <NoWarn>CS0169;CS0649;CS0414;CS0618;CS1701;CS0219;CS0168;CS0067;CS0108;CS0105;CS8981;NU1701</NoWarn>
    {define}
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="UnityEngine.Modules" Version="2021.3.33" />
    <PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
  </ItemGroup>
  <ItemGroup>
{items}
  </ItemGroup>
</Project>
"""


def expand_patterns(patterns):
    """Absolute paths matched by MSBuild-style globs (used only to find shim candidates)."""
    paths = set()
    for pattern in patterns:
        paths.update(glob.glob(os.path.join(ROOT, pattern), recursive=True))
    return {os.path.abspath(p) for p in paths}


def write_painter2d_shims(work, skip=()):
    """Copy files that use context.painter2D and replace that one Unity 6 access.

    Returns (shim_dir, absolute original paths) so the originals can be excluded and the
    shim copies compiled under their repository-relative paths. Files being replaced by a
    scratch copy are skipped: the replacement is compiled as-is.
    """
    shim_dir = os.path.join(work, "painter2d-shim")
    if os.path.isdir(shim_dir):
        shutil.rmtree(shim_dir)
    originals = set()
    assets = expand_patterns(["Assets/**/*.cs"]) - expand_patterns(["Assets/**/Editor/**/*.cs"])
    tools = expand_patterns(["Tools/Verification/**/*.cs"]) - expand_patterns(TOOLS_EXCLUDE)
    own_stubs = expand_patterns(["Tools/Verification/CloudTypeCheck/**/*.cs"])
    for source in sorted((assets | tools) - own_stubs):
        if os.path.abspath(source) in skip:
            continue
        with open(source, encoding="utf-8-sig") as handle:
            text = handle.read()
        if not PAINTER2D_ACCESS.search(text):
            continue
        dest = os.path.join(shim_dir, os.path.relpath(source, ROOT))
        os.makedirs(os.path.dirname(dest), exist_ok=True)
        with open(dest, "w") as handle:
            handle.write(PAINTER2D_ACCESS.sub("Painter2DCompat.Get(context)", text))
        originals.add(source)
    return shim_dir, originals


def build_items(mode, shim_dir, shim_originals, replacements, extra_sources=(), include_tools=True):
    items = ['    <Compile Include="%s" />' % os.path.join(HERE, "InputSystemStub.cs"),
             '    <Compile Include="%s" />' % os.path.join(HERE, "Painter2DStub.cs")]
    if mode == "tools":
        items.append('    <Compile Include="%s" />' % os.path.join(HERE, "UnityEditorStub.cs"))
    if os.path.isdir(shim_dir):
        items.append('    <Compile Include="%s" />' % os.path.join(shim_dir, "**/*.cs"))
    asset_exclude = [os.path.join(ROOT, "Assets/**/Editor/**/*.cs")]
    asset_exclude += [p for p in shim_originals if p.startswith(os.path.join(ROOT, "Assets") + os.sep)]
    asset_exclude += [p for p in replacements if p.startswith(os.path.join(ROOT, "Assets") + os.sep)]
    items.append('    <Compile Include="%s" Exclude="%s" />' % (os.path.join(ROOT, "Assets/**/*.cs"), ";".join(asset_exclude)))
    if mode == "tools" and include_tools:
        tool_exclude = [os.path.join(ROOT, p) for p in TOOLS_EXCLUDE]
        tool_exclude += [p for p in shim_originals if p.startswith(os.path.join(ROOT, "Tools") + os.sep)]
        tool_exclude += [p for p in replacements if p.startswith(os.path.join(ROOT, "Tools") + os.sep)]
        items.append('    <Compile Include="%s" Exclude="%s" />' % (os.path.join(ROOT, "Tools/Verification/**/*.cs"), ";".join(tool_exclude)))
    items += ['    <Compile Include="%s" />' % p for p in extra_sources]
    items += ['    <Compile Include="%s" />' % p for p in replacements.values()]
    return "\n".join(items)


def compile_project(work, mode, items, dotnet, shim_dir, replacement_paths):
    os.makedirs(work, exist_ok=True)
    define = "<DefineConstants>$(DefineConstants);UNITY_EDITOR</DefineConstants>" if mode == "tools" else ""
    with open(os.path.join(work, "typecheck.csproj"), "w") as handle:
        handle.write(PROJECT.format(items=items, define=define))
    env = dict(os.environ, DOTNET_CLI_TELEMETRY_OPTOUT="1", DOTNET_NOLOGO="1")
    result = subprocess.run([dotnet, "build", "-nologo", "-v", "q", "-o", os.path.join(work, "out")],
                            cwd=work, env=env, capture_output=True, text=True)
    output = result.stdout + result.stderr
    errors = sorted({n for n in (normalise(l, shim_dir, replacement_paths) for l in output.splitlines() if " error CS" in l) if n})
    if not errors and result.returncode != 0:
        sys.stdout.write(output)
        sys.exit("build failed without compiler errors (restore/network problem?)")
    return errors, output


def normalise(line, shim_dir, replacement_paths):
    """path(line,col): error CSnnnn: message [project]  ->  relpath: error CSnnnn: message"""
    match = re.match(r"^(.*?)\(\d+,\d+\): (error CS\d+: .*?)(?: \[[^\]]*\])?\s*$", line.strip())
    if not match:
        return None
    path = match.group(1)
    if os.path.abspath(path) in replacement_paths:
        path = replacement_paths[os.path.abspath(path)]
    elif path.startswith(ROOT + os.sep):
        path = path[len(ROOT) + 1:]
    elif path.startswith(shim_dir + os.sep):
        path = os.path.relpath(path, shim_dir)
    return "%s: %s" % (path, match.group(2))


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--mode", choices=["prod", "tools"], default="prod")
    parser.add_argument("--out", default=os.path.join(tempfile.gettempdir(), "ccf-typecheck"))
    parser.add_argument("--update-baseline", action="store_true")
    parser.add_argument("--replace", nargs=2, action="append", default=[], metavar=("REPO_PATH", "FILE"),
                        help="compile FILE instead of REPO_PATH (for a scratch/mutated harness copy)")
    args = parser.parse_args()
    dotnet = shutil.which("dotnet")
    if not dotnet:
        sys.exit("dotnet SDK not found")
    replacements = {}
    replacement_paths = {}
    for repo_path, replacement in args.replace:
        original = os.path.abspath(os.path.join(ROOT, repo_path))
        if not os.path.isfile(original):
            sys.exit("replacement target not found: " + repo_path)
        if not os.path.isfile(replacement):
            sys.exit("replacement file not found: " + replacement)
        replacements[original] = os.path.abspath(replacement)
        replacement_paths[os.path.abspath(replacement)] = repo_path
    work = os.path.join(args.out, args.mode)
    os.makedirs(work, exist_ok=True)
    shim_dir, shim_originals = write_painter2d_shims(work, set(replacements))
    errors, _ = compile_project(work, args.mode,
                                build_items(args.mode, shim_dir, shim_originals, replacements),
                                dotnet, shim_dir, replacement_paths)
    if args.mode == "tools":
        for index, supplement in enumerate(TOOLS_SUPPLEMENTS, 1):
            supplement_dir = os.path.join(work, "supplement-%d" % index)
            extra = [os.path.abspath(os.path.join(ROOT, supplement))]
            supplement_errors, _ = compile_project(
                supplement_dir, args.mode,
                build_items(args.mode, shim_dir, shim_originals, replacements, extra_sources=extra, include_tools=False),
                dotnet, shim_dir, replacement_paths)
            errors = sorted(set(errors) | set(supplement_errors))
    blockers = [e for e in errors if re.search(r"error (%s):" % "|".join(BLIND_CODES), e)]
    if blockers:
        print("[%s] declaration-level errors prevent method-body binding:" % args.mode)
        for line in blockers:
            print("BLIND " + line)
        print("TYPECHECK_BLIND mode=%s" % args.mode)
        if args.update_baseline:
            sys.exit("refusing to write a baseline containing declaration-level blockers")
        return 3
    baseline_path = os.path.join(HERE, "baseline-%s.txt" % args.mode)
    if args.update_baseline:
        with open(baseline_path, "w") as handle:
            handle.write("\n".join(errors) + ("\n" if errors else ""))
        print("baseline updated: %d errors -> %s" % (len(errors), baseline_path))
        return 0
    baseline = set()
    if os.path.isfile(baseline_path):
        baseline = {l.rstrip("\n") for l in open(baseline_path) if l.strip()}
    new = [e for e in errors if e not in baseline]
    resolved = sorted(baseline - set(errors))
    print("[%s] distinct errors: %d (baseline %d)" % (args.mode, len(errors), len(baseline)))
    for line in new:
        print("NEW  " + line)
    for line in resolved:
        print("GONE " + line)
    print("TYPECHECK_%s mode=%s" % ("CLEAN" if not new else "NEW_ERRORS", args.mode))
    return 0 if not new else 1


if __name__ == "__main__":
    sys.exit(main())
