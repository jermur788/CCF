#!/usr/bin/env python3
"""Run the disposable Enlarged80Verification gate (80B, D-056) once, in batch mode, one Editor at a time.

Stages Enlarged80Verification.cs into Assets/ForestPrototype, always removes it and its .meta, uses an isolated
Unity config under Build/Enlarged80/config (licence copied from the user's installed licence) and writes
Build/Enlarged80/<label>.log and <label>.json. It never writes the player's save file or saves the scene.

Usage: run_stand_geometry.py <label>
"""
import json, os, shutil, subprocess, sys, time
from pathlib import Path

ROOT = Path(__file__).resolve().parents[3]
UNITY = "/media/jer/ZX20/Unity/6000.6.0f1/Editor/Unity"
GATE = "Enlarged80Verification"
SOURCE = Path(__file__).with_name(GATE + ".cs")
TARGET = ROOT / "Assets" / "ForestPrototype" / (GATE + ".cs")
OUT = ROOT / "Build" / "Enlarged80"


def main():
    label = sys.argv[1] if len(sys.argv) > 1 else "run"
    busy = subprocess.run(["pgrep", "-f", "^" + UNITY + " "], capture_output=True, text=True).stdout.split()
    if busy:
        raise SystemExit("Another Unity Editor is running: " + str(busy))
    if TARGET.exists():
        raise SystemExit("Existing staged harness: " + str(TARGET))
    OUT.mkdir(parents=True, exist_ok=True)
    config = OUT / "config"
    licences = config / "unity3d" / "Unity" / "licenses"
    if not licences.exists():
        shutil.copytree(Path.home() / ".config/unity3d/Unity/licenses", licences)
    log = OUT / (label + ".log")
    unit = "ccf-enl80-" + str(time.time_ns())
    cmd = ["systemd-run", "--user", "--scope", "--quiet", "--unit=" + unit, "-p", "MemoryMax=8G", "-p", "MemorySwapMax=256M",
           UNITY, "-projectPath", str(ROOT), "-job-worker-count", "2", "-executeMethod", GATE + ".Begin",
           "-batchmode", "-nographics", "-logFile", str(log)]
    env = dict(os.environ, XDG_CONFIG_HOME=str(config), DISPLAY=os.environ.get("DISPLAY", ":0"))
    env.pop("CCF_STAND_GEOMETRY", None)  # this gate must see the real new-game policy, never an override
    start = time.monotonic()
    shutil.copy2(SOURCE, TARGET)
    try:
        try:
            code = subprocess.run(cmd, cwd=ROOT, env=env, timeout=1800).returncode
        except subprocess.TimeoutExpired:
            subprocess.run(["systemctl", "--user", "stop", unit + ".scope"], check=False)
            code = 124
    finally:
        TARGET.unlink(missing_ok=True)
        Path(str(TARGET) + ".meta").unlink(missing_ok=True)
    content = log.read_text(errors="replace") if log.exists() else ""
    markers = [l for l in content.splitlines() if any(x in l for x in ["ENLARGED80", "error CS"])]
    passed = code == 0 and "ENLARGED80_VERIFY_PASS" in content and not any("_FAIL" in l or "error CS" in l for l in markers)
    result = dict(label=label, gate=GATE, status="PASS" if passed else "FAIL", exit_code=code,
                  seconds=round(time.monotonic() - start, 1), markers=markers)
    (OUT / (label + ".json")).write_text(json.dumps(result, indent=2) + "\n")
    print(json.dumps({k: result[k] for k in ["label", "status", "exit_code", "seconds"]}), flush=True)
    for line in markers[-6:]:
        print("   ", line[:260])
    return 0 if passed else 1


if __name__ == "__main__":
    sys.exit(main())
