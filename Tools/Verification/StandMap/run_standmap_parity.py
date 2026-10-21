#!/usr/bin/env python3
"""Run the B5/B8 shared-map parity harness (StandMapParity.cs) in a graphical Unity Editor.

Stages Tools/Verification/StandMap/StandMapParity.cs into Assets/ForestPrototype/ for the run only, runs
`-executeMethod StandMapParity.Begin` against this checkout on an isolated Editor config (so PlayerPrefs and any save go to a
throwaway folder, not the player's), and always removes the staged script and its generated .meta afterwards.

Needs a graphical Editor (Game-view screenshots), so run it where DISPLAY works, and hold the Unity lock (CCF-Ops/scripts/unity-lock)
first: this script does not take it for you. Read the harness before running it. Evidence goes OUTSIDE the repository.

Usage: run_standmap_parity.py --output DIR [--sizes 1920x1080,1280x720] [--timeout SECONDS]
Exit code 0 only when the log has STANDMAP_PARITY_PASS and no compile error.
"""
import argparse, os, shutil, subprocess, sys, time
from pathlib import Path

ROOT = Path(__file__).resolve().parents[3]
HARNESS = ROOT / "Tools/Verification/StandMap/StandMapParity.cs"
STAGED = ROOT / "Assets/ForestPrototype/StandMapParity.cs"
UNITY = "/media/jer/ZX20/Unity/6000.6.0f1/Editor/Unity"


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--output", required=True)
    parser.add_argument("--sizes", default="1920x1080,1280x720")
    parser.add_argument("--timeout", type=int, default=2400)
    parser.add_argument("--unity", default=UNITY)
    args = parser.parse_args()
    out = Path(args.output).resolve()
    out.mkdir(parents=True, exist_ok=True)
    config = out / "config"
    licenses = Path.home() / ".config/unity3d/Unity/licenses"
    if licenses.is_dir():
        shutil.copytree(licenses, config / "unity3d/Unity/licenses", dirs_exist_ok=True)
    if STAGED.exists():
        sys.exit("An Assets copy already exists and must be investigated first: " + str(STAGED))
    log = out / "StandMapParity.log"
    shutil.copy2(HARNESS, STAGED)
    command = [args.unity, "-projectPath", str(ROOT), "-logFile", str(log), "-executeMethod", "StandMapParity.Begin"]
    env = dict(os.environ, XDG_CONFIG_HOME=str(config), CCF_DIAG_DIR=str(out / "evidence"), CCF_UI_SIZES=args.sizes,
               DISPLAY=os.environ.get("DISPLAY", ":0"))
    started = time.monotonic()
    code = None
    try:
        code = subprocess.run(command, cwd=ROOT, env=env, timeout=args.timeout).returncode
    except subprocess.TimeoutExpired:
        print("TIMEOUT after %d s" % args.timeout)
        code = -1
    finally:
        for path in (STAGED, Path(str(STAGED) + ".meta")):
            if path.exists():
                path.unlink()
    text = log.read_text(errors="replace") if log.exists() else ""
    lines = [l for l in text.splitlines() if "STANDMAP_" in l or "error CS" in l or "No valid Unity Editor license" in l]
    for line in lines:
        print(line)
    ok = code == 0 and "STANDMAP_PARITY_PASS" in text and "error CS" not in text and "STANDMAP_FAIL" not in text
    print("RESULT %s exit=%s seconds=%.0f log=%s" % ("PASS" if ok else "FAIL", code, time.monotonic() - started, log))
    leftovers = [p for p in (STAGED, Path(str(STAGED) + ".meta")) if p.exists()]
    if leftovers:
        print("LEFTOVER", leftovers)
        ok = False
    return 0 if ok else 1


if __name__ == "__main__":
    sys.exit(main())
