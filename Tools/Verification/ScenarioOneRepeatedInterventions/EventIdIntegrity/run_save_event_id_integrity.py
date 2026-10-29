#!/usr/bin/env python3
"""Run the S1-D T-06 Editor gate (SaveEventIdIntegrityVerification) once, in the project this script lives in.

Copy/run/remove workflow: stages the disposable harness under Assets/ForestPrototype, runs Unity in batchmode, then removes
the source and its generated .meta. The player's forest-save.json/.bak/.tmp digests are checked unchanged afterwards.

THIS STARTS THE UNITY EDITOR. Run it only from the machine-wide Unity lane while you hold the Unity lock for this exact
candidate (see Docs/Project/coordination/active-tasks.md; RACK-V2-08 / #49 has priority). Hence the explicit flag below.

Usage: run_save_event_id_integrity.py --i-hold-the-unity-lock [--label NAME]
Expected log markers: SAVE_EVENT_ID_CASE lines per case and SAVE_EVENT_ID_VERIFY_PASS. Output goes to Build/ (ignored by Git
in this project) and should be copied to /media/jer/Files/CCF/Verification/ as evidence.
"""
import argparse, hashlib, json, os, re, shutil, subprocess, sys, time
from pathlib import Path

ROOT = Path(__file__).resolve().parents[4]
UNITY = "/media/jer/ZX20/Unity/6000.6.0f1/Editor/Unity"
SAVES = [Path.home() / ".config/unity3d/DefaultCompany/CCF" / n for n in ("forest-save.json", "forest-save.json.bak", "forest-save.json.tmp")]


def digest(path):
    return hashlib.sha256(path.read_bytes()).hexdigest() if path.exists() else None


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--i-hold-the-unity-lock", action="store_true", required=True)
    parser.add_argument("--label", default="run-1")
    args = parser.parse_args()
    out = ROOT / "Build" / "SaveEventIdIntegrity"
    out.mkdir(parents=True, exist_ok=True)
    config = out / "config"
    shutil.copytree(Path.home() / ".config/unity3d/Unity/licenses", config / "unity3d/Unity/licenses", dirs_exist_ok=True)
    env = dict(os.environ, XDG_CONFIG_HOME=str(config))
    for key in ("CCF_STAND_GEOMETRY", "CCF_STARTUP_SCREEN", "CCF_STARTUP_INPUT", "CCF_STARTUP_BOUNDARY"):
        env.pop(key, None)
    saved = {str(p): digest(p) for p in SAVES}
    (out / "original-saves.json").write_text(json.dumps(saved, indent=2))
    if subprocess.run(["pgrep", "-f", "^" + UNITY + " "], capture_output=True, text=True).stdout.strip():
        sys.exit("another Unity Editor is running; refusing to start")
    source = ROOT / "Tools/Verification/ScenarioOneRepeatedInterventions/EventIdIntegrity/SaveEventIdIntegrityVerification.cs"
    target = ROOT / "Assets/ForestPrototype/SaveEventIdIntegrityVerification.cs"
    if target.exists():
        sys.exit("refusing to overwrite " + str(target))
    shutil.copy2(source, target)
    log = out / (args.label + ".log")
    start = time.monotonic()
    try:
        cmd = [UNITY, "-projectPath", str(ROOT), "-job-worker-count", "2", "-executeMethod", "SaveEventIdIntegrityVerification.Begin",
               "-batchmode", "-nographics", "-logFile", str(log)]
        code = subprocess.run(cmd, cwd=ROOT, env=env, timeout=1800).returncode
    finally:
        target.unlink(missing_ok=True)
        Path(str(target) + ".meta").unlink(missing_ok=True)
    text = log.read_text(errors="replace")
    compile_errors = re.findall(r"error CS\d+.*", text)
    passed = code == 0 and "SAVE_EVENT_ID_VERIFY_PASS" in text and "VERIFY_FAIL" not in text and not compile_errors
    result = dict(label=args.label, exit_code=code, status="PASS" if passed else "FAIL", seconds=round(time.monotonic() - start, 2),
                  source_sha256=digest(source), compile_errors=compile_errors[:5],
                  markers=[l for l in text.splitlines() if l.startswith("SAVE_EVENT_ID_")])
    (out / (args.label + "-result.json")).write_text(json.dumps(result, indent=2))
    print(result["status"], args.label)
    if any(digest(Path(p)) != h for p, h in saved.items()):
        sys.exit("ORIGINAL SAVES CHANGED")
    return 0 if passed else 1


if __name__ == "__main__":
    sys.exit(main())
