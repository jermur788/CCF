#!/usr/bin/env python3
"""Run the disposable StandExpansionAudit harness once (interactive, one Editor at a time).

Stages Tools/Verification/StandExpansion/StandExpansionAudit.cs into Assets/ForestPrototype, always removes it and
its .meta, uses an isolated Unity config under Build/StandExpansion/config (licence copied from the user's
installed licence) and writes the log, audit-summary.json, per-world tree CSVs and screenshots under
Build/StandExpansion/<label>/. It never writes the player's save file and never saves the scene.

Usage: run_stand_audit.py <label> [--trees80 N] [--no-render]
"""
import argparse, os, shutil, subprocess, sys, time
from pathlib import Path

ROOT = Path(__file__).resolve().parents[3]
UNITY = "/media/jer/ZX20/Unity/6000.6.0f1/Editor/Unity"
GATE = "StandExpansionAudit"
SOURCE = Path(__file__).with_name(GATE + ".cs")
TARGET = ROOT / "Assets" / "ForestPrototype" / (GATE + ".cs")


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("label")
    parser.add_argument("--trees80", default="1344")
    parser.add_argument("--no-render", action="store_true")
    args = parser.parse_args()
    busy = subprocess.run(["pgrep", "-f", "^" + UNITY + " "], capture_output=True, text=True).stdout.split()
    if busy:
        raise SystemExit("Another Unity Editor is running: " + str(busy))
    if TARGET.exists():
        raise SystemExit("Existing staged harness: " + str(TARGET))
    out = ROOT / "Build" / "StandExpansion" / args.label
    out.mkdir(parents=True, exist_ok=True)
    config = ROOT / "Build" / "StandExpansion" / "config"
    licences = config / "unity3d" / "Unity" / "licenses"
    if not licences.exists():
        shutil.copytree(Path.home() / ".config/unity3d/Unity/licenses", licences)
    log = out / "editor.log"
    unit = "ccf-standaudit-" + str(time.time_ns())
    cmd = ["systemd-run", "--user", "--scope", "--quiet", "--unit=" + unit, "-p", "MemoryMax=10G", "-p", "MemorySwapMax=256M",
           UNITY, "-projectPath", str(ROOT), "-job-worker-count", "2", "-executeMethod", GATE + ".Begin", "-logFile", str(log)]
    env = dict(os.environ, XDG_CONFIG_HOME=str(config), CCF_ACCEPTANCE_OUTPUT=str(out), CCF_AUDIT_TREES80=args.trees80,
               CCF_AUDIT_RENDER="0" if args.no_render else "1", DISPLAY=os.environ.get("DISPLAY", ":0"))
    start = time.monotonic()
    shutil.copy2(SOURCE, TARGET)
    try:
        try:
            code = subprocess.run(cmd, cwd=ROOT, env=env, timeout=3600).returncode
        except subprocess.TimeoutExpired:
            subprocess.run(["systemctl", "--user", "stop", unit + ".scope"], check=False)
            code = 124
    finally:
        TARGET.unlink(missing_ok=True)
        Path(str(TARGET) + ".meta").unlink(missing_ok=True)
    content = log.read_text(errors="replace") if log.exists() else ""
    passed = code == 0 and "STAND_AUDIT_PASS" in content and "error CS" not in content
    print({"label": args.label, "status": "PASS" if passed else "FAIL", "exit_code": code,
           "seconds": round(time.monotonic() - start, 1)}, flush=True)
    return 0 if passed else 1


if __name__ == "__main__":
    sys.exit(main())
