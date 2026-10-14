#!/usr/bin/env python3
"""Build the INTERNAL Linux Development Player for the dense-stand performance baseline (task: perf/scenario1-dense-stand-player-v1).

Run inside the S1-B HDR wrapper so the Manager-approved demo HDR stays out of the Player:

    python3 Tools/Build/S1B/with_demo_hdr_quarantined.py python3 Tools/Verification/PlayerPerformance/run_dense_build.py --label D01

Production source (Assets, ProjectSettings, Packages) must equal the base commit exactly; only this tooling folder differs. The builder and the
probe are copied into Assets, built, and always removed with their .meta files. Unity/URP rewrite a few settings files during a build; their original
bytes are restored and the generated diff is kept next to the build. Output (ignored by Git): Build/PlayerPerformance/<label>/.
"""
import argparse
import hashlib
import json
import os
import re
import shutil
import subprocess
import time
from pathlib import Path

ROOT = Path(__file__).resolve().parents[3]
BASE = "c1564b857972ee18917e8a461c457533a54cc3f1"
UNITY = "/media/jer/ZX20/Unity/6000.6.0f1/Editor/Unity"


def git(*args):
    return subprocess.check_output(["git", *args], cwd=ROOT, text=True).strip()


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--label", required=True)
    args = parser.parse_args()
    assert re.fullmatch(r"D[0-9]{2,}", args.label), "label must look like D01"
    assert subprocess.run(["git", "merge-base", "--is-ancestor", BASE, "HEAD"], cwd=ROOT).returncode == 0, "HEAD must descend from the base"
    assert not git("diff", BASE, "HEAD", "--name-only", "--", "Assets", "ProjectSettings", "Packages"), "production source differs from the base"
    assert not git("status", "--porcelain"), "worktree must be clean"
    assert not subprocess.run(["pgrep", "-f", "^" + UNITY + " "], capture_output=True, text=True).stdout.strip(), "a Unity Editor is already running"

    out = ROOT / "Build/PlayerPerformance" / args.label
    out.mkdir(parents=True, exist_ok=False)
    dest = out / "player"
    dest.mkdir()
    tools = ROOT / "Tools/Verification/PlayerPerformance"
    builder = ROOT / "Assets/ForestPrototype/Editor/DenseProfilingBuilder.cs"
    probe = ROOT / "Assets/ForestPrototype/DensePerfProbe.cs"
    assert not builder.exists() and not probe.exists(), "a staged copy already exists"
    config = out / "config"
    shutil.copytree(Path.home() / ".config/unity3d/Unity/licenses", config / "unity3d/Unity/licenses", dirs_exist_ok=True)
    env = dict(os.environ, XDG_CONFIG_HOME=str(config), CCF_BUILD_OUTPUT=str(dest))
    log = out / "build.log"

    preserved = [".vscode/settings.json", "Assets/Settings/DefaultVolumeProfile.asset", "Assets/Settings/PC_RPAsset.asset",
                 "Assets/Settings/UniversalRenderPipelineGlobalSettings.asset", "ProjectSettings/GraphicsSettings.asset",
                 "ProjectSettings/ProjectSettings.asset", "ProjectSettings/UnityConnectSettings.asset"]
    preserved += [str(p.relative_to(ROOT)) for p in (ROOT / "Assets/ForestPrototype/Art/SectionFive").rglob("*.mat")]
    original = {name: (ROOT / name).read_bytes() for name in preserved}
    start = time.monotonic()
    unit = "ccf-dense-build-" + str(time.time_ns())
    try:
        builder.parent.mkdir(exist_ok=True)
        shutil.copy2(tools / "DenseProfilingBuilder.cs", builder)
        shutil.copy2(tools / "DensePerfProbe.cs", probe)
        code = subprocess.run(["systemd-run", "--user", "--scope", "--quiet", "--unit=" + unit, "-p", "MemoryMax=8G", "-p", "MemorySwapMax=256M",
                               UNITY, "-projectPath", str(ROOT), "-job-worker-count", "2", "-batchmode", "-nographics",
                               "-executeMethod", "DenseProfilingBuilder.Build", "-logFile", str(log)], cwd=ROOT, env=env, timeout=5400).returncode
    finally:
        for p in (builder, probe):
            p.unlink(missing_ok=True)
            Path(str(p) + ".meta").unlink(missing_ok=True)
        editor_dir = builder.parent
        if editor_dir.exists() and not any(editor_dir.iterdir()):
            editor_dir.rmdir()
            Path(str(editor_dir) + ".meta").unlink(missing_ok=True)
        changed = [name for name, data in original.items() if (ROOT / name).read_bytes() != data]
        (out / "build-normalisation.diff").write_bytes(subprocess.check_output(["git", "diff", "--", *changed], cwd=ROOT))
        for name in changed:
            (ROOT / name).write_bytes(original[name])
    status = git("status", "--porcelain")
    text = log.read_text(errors="replace")
    ok = code == 0 and "DENSE_PERF_DEVELOPMENT_BUILD_PASS" in text and not re.search(r"error CS\d+", text)
    for directory in list(dest.iterdir()):
        if directory.is_dir() and any(w in directory.name.lower() for w in ["donotship", "dontship", "backupthisfolder"]):
            backups = out / "developer-backups"
            backups.mkdir(exist_ok=True)
            shutil.move(str(directory), str(backups / directory.name))
    binary = dest / "CCF.x86_64"
    receipt = dict(label=args.label, status="PASS" if ok and binary.is_file() else "FAIL", exit_code=code, head=git("rev-parse", "HEAD"), base=BASE,
                   seconds=round(time.monotonic() - start, 1), development=True, backend="Mono", binary=str(binary),
                   binary_sha256=hashlib.sha256(binary.read_bytes()).hexdigest() if binary.is_file() else None,
                   normalised_and_restored=changed, tree_status_after=status)
    (out / "build-receipt.json").write_text(json.dumps(receipt, indent=2))
    print(json.dumps(receipt, indent=2), flush=True)
    raise SystemExit(0 if receipt["status"] == "PASS" and not status else 1)


if __name__ == "__main__":
    main()
