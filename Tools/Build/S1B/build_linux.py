#!/usr/bin/env python3
"""Build one clean committed Linux Standalone (Mono) Player for the Tier-3 gate.

Separate from the accepted Windows orchestrator (build_windows.py): this file does
not import, modify or refactor it. Structure and safety checks mirror it where they
apply, but every Windows-specific assumption (.exe/.dll, notices, tester archive) is
deliberately absent. No distribution or tester-package claim is made.

Staging: copies LinuxPlayerBuilder.cs under Assets/ForestPrototype/Editor/ (the Editor
assembly, exactly like the accepted WindowsCandidateBuilder; in the player assembly it
cannot compile) and, with --smoke, the existing StandalonePlayerSmoke.cs under
Assets/ForestPrototype/, runs ONE Unity method, and always removes the staged copies and
their .meta afterwards. Refuses a dirty source tree, a pre-existing staged copy or a
pre-existing output directory.

Art pack: the shipped scene needs the locally acquired Asset Store pack "Ultimate Nature -
Starter" (gitignored, never committed). Before anything is staged the pack is verified
against Tools/Build/S1B/art-pack-manifest.json (see art_pack.py); a missing or different
pack stops the build with a precise message. Give its location with --art-pack DIR or
$CCF_ART_PACK_DIR; only the required files are installed into the gitignored
Assets/InnerverseInteractive. The pack identity and the tooling sources are recorded in
build-identity.json / package-manifest.json.

Output defaults to ZX20 substantial-data storage:
  /media/jer/ZX20/CCF-Builds/S1B/CCF-S1-<date>-<sequence>-<sha7>-Linux-x64/
"""
import argparse
import datetime
import hashlib
import json
import os
import shutil
import socket
import subprocess
import sys
import time
from pathlib import Path
from zoneinfo import ZoneInfo

sys.path.insert(0, str(Path(__file__).resolve().parent))
import art_pack  # noqa: E402  (sibling module; explicit local art-pack dependency check)

DEFAULT_UNITY = "/media/jer/ZX20/Unity/6000.6.0f1/Editor/Unity"
DEFAULT_OUTPUT_ROOT = "/media/jer/ZX20/CCF-Builds/S1B"
STAGE = "Assets/ForestPrototype"
BUILDER_STAGE = STAGE + "/Editor"   # Editor assembly: UnityEditor.Build types do not exist in the player assembly
TOOLING_FILES = ["Tools/Build/S1B/build_linux.py", "Tools/Build/S1B/art_pack.py", "Tools/Build/S1B/art-pack-manifest.json",
                 "Tools/Build/S1B/LinuxPlayerBuilder.cs", "Tools/Verification/S1B/StandalonePlayerSmoke.cs"]
LINUX_BINARY = "CCF.x86_64"
LINUX_DATA_DIR = "CCF_Data"
LINUX_RUNTIME_LIB = "UnityPlayer.so"
BUILD_MARKER = "S1B_LINUX_BUILD_PASS"

# Files Unity/URP rewrite during a build; original bytes are preserved and restored.
PRESERVED = [
    ".vscode/settings.json",
    "Assets/Settings/DefaultVolumeProfile.asset",
    "Assets/Settings/PC_RPAsset.asset",
    "Assets/Settings/UniversalRenderPipelineGlobalSettings.asset",
    "ProjectSettings/GraphicsSettings.asset",
    "ProjectSettings/ProjectSettings.asset",
    "ProjectSettings/UnityConnectSettings.asset",
]


def git(*args, cwd):
    return subprocess.check_output(["git", *args], cwd=cwd, text=True).strip()


def require_socket_capable_tmpdir():
    """Unity's licensing client creates a Unix socket under $TMPDIR. On exFAT (the ZX20 drive) it cannot, and the Editor
    then hangs for minutes with an unrelated-looking licensing error; stop early with the real reason instead."""
    import tempfile
    path = os.path.join(tempfile.gettempdir(), ".ccf-sock-%d" % os.getpid())
    if len(path.encode()) >= 100:
        return   # cannot probe within the AF_UNIX path limit
    probe = socket.socket(socket.AF_UNIX, socket.SOCK_STREAM)
    try:
        probe.bind(path)
    except OSError as error:
        raise SystemExit("STOP: TMPDIR %s cannot create Unix sockets (%s); Unity's licensing client would hang. "
                         "Set TMPDIR to a socket-capable folder such as /run/user/<uid>/ccf-build." % (tempfile.gettempdir(), error))
    finally:
        probe.close()
        try:
            os.unlink(path)
        except OSError:
            pass


def build_id_for(date, sequence, sha, smoke):
    value = f"CCF-S1-{date}-{sequence}-{sha[:7]}"
    return value + "-SMOKE" if smoke else value


def output_dir_for(output_root, build_id):
    return Path(output_root) / f"{build_id}-Linux-x64"


def unity_command(unity, project, log):
    return [str(unity), "-batchmode", "-nographics", "-projectPath", str(project),
            "-job-worker-count", "2", "-executeMethod", "LinuxPlayerBuilder.Build", "-logFile", str(log)]


def validate_output(out):
    """Return (ok, detail). Asserts the Linux Player layout, never a Windows one."""
    binary = out / LINUX_BINARY
    if not binary.is_file():
        return False, f"missing {LINUX_BINARY}"
    if not (out / LINUX_DATA_DIR).is_dir():
        return False, f"missing {LINUX_DATA_DIR}/"
    if not (out / LINUX_RUNTIME_LIB).is_file():
        return False, f"missing {LINUX_RUNTIME_LIB}"
    if not os.access(binary, os.X_OK):
        return False, f"{LINUX_BINARY} is not executable"
    return True, "linux layout present"


def sha256(path):
    return hashlib.sha256(Path(path).read_bytes()).hexdigest()


def manifest_for(out):
    files = {}
    for path in sorted(out.rglob("*")):
        if path.is_file():
            files[str(path.relative_to(out))] = {"bytes": path.stat().st_size, "sha256": sha256(path)}
    return {"files": files}


def main():
    p = argparse.ArgumentParser(description=__doc__)
    p.add_argument("--project", default=str(Path(__file__).resolve().parents[3]), help="project root (default: this repo)")
    p.add_argument("--unity", default=DEFAULT_UNITY)
    p.add_argument("--output-root", default=DEFAULT_OUTPUT_ROOT, help="build root (default: ZX20 CCF-Builds)")
    p.add_argument("--date", default=datetime.datetime.now(ZoneInfo("Europe/Dublin")).strftime("%Y%m%d"))
    p.add_argument("--sequence", default="L01")
    p.add_argument("--smoke", action="store_true", help="stage the diagnostic StandalonePlayerSmoke; not a tester package")
    p.add_argument("--timeout", type=int, default=5400)
    p.add_argument("--license-dir", default=None,
                   help="folder holding the CURRENTLY VALID Unity licence (Hub-managed). Default: ~/.config/unity3d/Unity/licenses, "
                        "which can be stale; Unity then stops with 'No valid Unity Editor license found'")
    p.add_argument("--art-pack", default=os.environ.get("CCF_ART_PACK_DIR"),
                   help="folder of the locally acquired Ultimate Nature - Starter pack (InnerverseInteractive, or its parent) "
                        "(default: $CCF_ART_PACK_DIR; else the pack must already be installed in the project)")
    p.add_argument("--no-art-pack-check", action="store_true",
                   help="TEST ONLY (mock projects): skip the art-pack check; recorded as unverified in the build identity")
    a = p.parse_args()

    root = Path(a.project).resolve()
    if not (root / STAGE).is_dir():
        raise SystemExit("STOP: " + str(root / STAGE) + " missing; run in a full project worktree")
    if git("status", "--porcelain", cwd=root):
        raise SystemExit("STOP: source must be committed and clean")
    if not a.date.isdigit() or len(a.date) != 8 or not (a.sequence.startswith(("L", "S")) and a.sequence[1:].isdigit()):
        raise SystemExit("Invalid date/sequence (sequence e.g. L01)")

    require_socket_capable_tmpdir()
    licences = Path(a.license_dir) if a.license_dir else Path.home() / ".config/unity3d/Unity/licenses"
    if a.license_dir and not licences.is_dir():
        raise SystemExit("STOP: --license-dir folder not found: " + str(licences))

    sha = git("rev-parse", "HEAD", cwd=root)
    build_id = build_id_for(a.date, a.sequence, sha, a.smoke)
    out = output_dir_for(a.output_root, build_id)
    identity = dict(buildId=build_id, gitSha=sha, unity="6000.6.0f1", platform="Linux-x64",
                    saveContract="same-build only", distributionClaim=False)
    # Tooling provenance: the committed sources this build ran from (the tree is clean, so these are the blobs of `sha`).
    identity["toolingSource"] = {"gitSha": sha, "builderStagedAt": BUILDER_STAGE + "/LinuxPlayerBuilder.cs",
                                 "files": {rel: sha256(root / rel) for rel in TOOLING_FILES if (root / rel).is_file()}}
    if a.no_art_pack_check:
        print("WARNING: --no-art-pack-check: the art pack is NOT verified (test use only)", file=sys.stderr)
        identity["artPack"] = {"verified": False, "reason": "--no-art-pack-check"}
    else:
        try:
            identity["artPack"] = art_pack.prepare(root, a.art_pack, root / "Tools/Build/S1B/art-pack-manifest.json")
        except art_pack.ArtPackError as error:
            raise SystemExit("STOP: " + str(error))
        full_content = identity["artPack"].get("fullContent")
        if isinstance(full_content, dict) and full_content.get("matchesManifest") is False:
            print("WARNING: the pack's whole-content checksum differs from the manifest; the required files do match", file=sys.stderr)

    builder_target = root / BUILDER_STAGE / "LinuxPlayerBuilder.cs"
    smoke_target = root / STAGE / "S1BStandalonePlayerSmoke.cs"
    stamp = root / STAGE / "UI/Resources/CCFBuildIdentity.json"
    for staged in (builder_target, Path(str(builder_target) + ".meta"), smoke_target,
                   Path(str(smoke_target) + ".meta"), stamp):
        if staged.exists():
            raise SystemExit("STOP: pre-existing staged/identity file: " + str(staged))
    if out.exists():
        raise SystemExit("STOP: output already exists; refusing to overwrite: " + str(out))
    builder_target.parent.mkdir(parents=True, exist_ok=True)
    out.mkdir(parents=True)

    config = Path(a.output_root) / "config"
    if licences.is_dir():
        shutil.copytree(licences, config / "unity3d/Unity/licenses", dirs_exist_ok=True)
    env = dict(os.environ, XDG_CONFIG_HOME=str(config), CCF_BUILD_OUTPUT=str(out), CCF_BUILD_IDENTITY=json.dumps(identity))

    preserved = [rel for rel in PRESERVED if (root / rel).exists()]
    preserved += [str(f.relative_to(root)) for f in (root / "Assets/ForestPrototype/Art/SectionFive").rglob("*.mat")] \
        if (root / "Assets/ForestPrototype/Art/SectionFive").is_dir() else []
    original = {rel: (root / rel).read_bytes() for rel in preserved}

    log = Path(a.output_root) / (build_id + "-build.log")
    collapsed = False
    start = time.monotonic()
    try:
        shutil.copy2(root / "Tools/Build/S1B/LinuxPlayerBuilder.cs", builder_target)
        if a.smoke:
            shutil.copy2(root / "Tools/Verification/S1B/StandalonePlayerSmoke.cs", smoke_target)
        try:
            code = subprocess.run(unity_command(a.unity, root, log), cwd=root, env=env, timeout=a.timeout).returncode
        except subprocess.TimeoutExpired:
            code = 124
    finally:
        for staged in (builder_target, Path(str(builder_target) + ".meta"), smoke_target,
                       Path(str(smoke_target) + ".meta"), stamp, Path(str(stamp) + ".meta")):
            staged.unlink(missing_ok=True)
        changed = [rel for rel, data in original.items() if (root / rel).read_bytes() != data]
        if changed:
            (Path(a.output_root) / (build_id + "-source-normalisation.diff")).write_bytes(
                subprocess.check_output(["git", "diff", "--", *changed], cwd=root))
            for rel in changed:
                (root / rel).write_bytes(original[rel])
        if git("status", "--porcelain", cwd=root):
            raise SystemExit("STOP: unexpected source drift after build; inspect Git diff")

    result = {"buildId": build_id, "gitSha": sha, "platform": "Linux-x64", "output": str(out),
              "seconds": round(time.monotonic() - start, 1), "smoke": a.smoke, "log": str(log),
              "artPack": identity["artPack"]}
    if code == 124:
        collapsed = True
        result.update(status="FAIL", reason="timeout", exitCode=124)
        print(json.dumps(result, indent=2))
        return 2
    if code != 0:
        result.update(status="FAIL", reason="unity exit", exitCode=code)
        print(json.dumps(result, indent=2))
        return 1
    text = log.read_text(errors="replace") if log.exists() else ""
    if BUILD_MARKER not in text:
        result.update(status="FAIL", reason="missing " + BUILD_MARKER)
        print(json.dumps(result, indent=2))
        return 1
    ok, detail = validate_output(out)
    if not ok:
        result.update(status="FAIL", reason=detail)
        print(json.dumps(result, indent=2))
        return 1
    (out / "build-identity.json").write_text(json.dumps(identity, indent=2) + "\n")
    manifest = manifest_for(out)
    manifest["identity"] = identity
    manifest_file = out / "package-manifest.json"
    manifest_file.write_text(json.dumps(manifest, indent=2) + "\n")
    result.update(status="PASS", output=detail, files=len(manifest["files"]),
                  manifest=str(manifest_file), manifest_sha256=sha256(manifest_file),
                  binary=LINUX_BINARY, executable=os.access(out / LINUX_BINARY, os.X_OK))
    print(json.dumps(result, indent=2))
    return 0


if __name__ == "__main__":
    sys.exit(main())
