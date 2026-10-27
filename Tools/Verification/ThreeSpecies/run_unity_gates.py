#!/usr/bin/env python3
"""Run the three-species programme's disposable Unity gates, one at a time.

STATUS: written in a code-only cloud session with NO Unity install. The staging,
cleanup and result-recording logic was exercised against a fake editor script
(see Docs/Verification/ThreeSpeciesProgramme/Phase10-Qualification.md); it has
never driven a real Unity Editor. Read it before relying on it.

What it does for each gate:
  1. refuses to start if the staged copy or its .meta already exists;
  2. copies the gate from Tools/Verification into Assets/ForestPrototype/;
  3. runs   <unity> -projectPath <repo> -executeMethod <Gate>.Begin -logFile <log>
     with an isolated XDG_CONFIG_HOME (so no real EditorPrefs/save is touched)
     and CCF_STAND_GEOMETRY=0 unless you set it;
  4. ALWAYS removes the staged copy and its .meta, even after a failure;
  5. compares `git status` before and after and fails the run if anything else
     changed in the working tree (Unity can re-serialise assets on import);
  6. writes one JSON result per gate under --out (default Build/ThreeSpecies,
     which is git-ignored).

PASS needs: exit code 0, the gate's terminal marker in the log, and no *_FAIL
marker or "error CS" line. Close any other Unity instance on this checkout first.

Usage:
  run_unity_gates.py --unity /path/to/Unity [--gate NAME ...] [--dry-run]
                     [--out DIR] [--timeout SECONDS]
  UNITY_EDITOR may be set instead of --unity. With no --gate, the four new
  gates run in order. ScenarioOneCompletionVerification is a measurement, not
  one of the four: it is only run when named, and CCF_GROWTH_MODEL passes through
  from your environment (use CCF_GROWTH_MODEL=2 to measure model 2).
"""
import argparse
import hashlib
import json
import os
import subprocess
import sys
import time
from pathlib import Path

ROOT = Path(__file__).resolve().parents[3]
STAGE_DIR = ROOT / "Assets" / "ForestPrototype"

# name -> (source relative to repo, run with a graphical Editor, terminal pass marker)
GATES = {
    "MixedDensityVerification": ("Tools/Verification/ThreeSpecies/MixedDensityVerification.cs", False, "MIXED_DENSITY_VERIFY_PASS"),
    "BroadleafAdultGrowthVerification": ("Tools/Verification/ThreeSpecies/BroadleafAdultGrowthVerification.cs", False, "BROADLEAF_GROWTH_VERIFY_PASS"),
    "BroadleafTimberEconomyVerification": ("Tools/Verification/ThreeSpecies/BroadleafTimberEconomyVerification.cs", False, "BROADLEAF_TIMBER_VERIFY_PASS"),
    # The history gate also checks the review gate through the UI root when one exists, so it runs with a graphical Editor
    # (the same choice the existing MenuTutorial gate makes).
    "ScenarioHistoryVerification": ("Tools/Verification/ThreeSpecies/ScenarioHistoryVerification.cs", True, "SCENARIO_HISTORY_VERIFY_PASS"),
    "ScenarioOneCompletionVerification": ("Tools/Verification/ScenarioOneCompletionVerification.cs", False, "SCENARIO_ONE_COMPLETION_VERIFY_PASS"),
}
DEFAULT_GATES = ["MixedDensityVerification", "BroadleafAdultGrowthVerification", "BroadleafTimberEconomyVerification", "ScenarioHistoryVerification"]


def git(*args):
    return subprocess.check_output(["git", *args], cwd=ROOT).decode().strip()


def tree_state():
    # Every tracked change and every untracked file, so a leftover staged script or a re-serialised asset shows up.
    return set(git("status", "--porcelain", "--untracked-files=all").splitlines())


def run_gate(name, unity, out, timeout, dry_run):
    source, interactive, marker = GATES[name]
    staged = STAGE_DIR / (name + ".cs")
    staged_meta = Path(str(staged) + ".meta")
    log = out / (name + ".log")
    cmd = [unity, "-projectPath", str(ROOT), "-executeMethod", name + ".Begin", "-logFile", str(log)]
    if not interactive:
        cmd += ["-batchmode", "-nographics"]
    env = dict(os.environ, XDG_CONFIG_HOME=str(out / "config"), CCF_DIAG_DIR=str(out / "evidence"), DISPLAY=os.environ.get("DISPLAY", ":0"))
    env.setdefault("CCF_STAND_GEOMETRY", "0")  # historical anchors run in the explicit Editor-only Legacy40 override (D-056)
    if dry_run:
        print(json.dumps({"gate": name, "would_stage": str(staged), "command": cmd, "CCF_STAND_GEOMETRY": env["CCF_STAND_GEOMETRY"], "CCF_GROWTH_MODEL": env.get("CCF_GROWTH_MODEL")}))
        return None
    if staged.exists() or staged_meta.exists():
        raise RuntimeError("Refusing to start: staged copy or .meta already exists: " + str(staged))
    (out / "config").mkdir(parents=True, exist_ok=True)
    (out / "evidence").mkdir(parents=True, exist_ok=True)
    data = (ROOT / source).read_bytes()
    before = tree_state()
    start = time.monotonic()
    code = None
    try:
        staged.write_bytes(data)
        try:
            code = subprocess.run(cmd, cwd=ROOT, env=env, timeout=timeout).returncode
        except subprocess.TimeoutExpired:
            code = 124
    finally:
        staged.unlink(missing_ok=True)
        staged_meta.unlink(missing_ok=True)
    content = log.read_text(errors="replace") if log.exists() else ""
    markers = [line for line in content.splitlines() if any(x in line for x in ("_PASS", "_FAIL", "_HASH", "error CS"))]
    failed_marker = any("_FAIL" in line or "error CS" in line for line in markers)
    unexpected = sorted(tree_state() - before)  # anything new in the working tree after the staged copy is gone
    passed = code == 0 and marker in content and not failed_marker and not unexpected
    result = {
        "gate": name,
        "status": "PASS" if passed else "FAIL",
        "exit_code": code,
        "seconds": round(time.monotonic() - start, 2),
        "mode": "interactive" if interactive else "batch",
        "run_head": git("rev-parse", "HEAD"),
        "run_branch": git("branch", "--show-current"),
        "source_sha256": hashlib.sha256(data).hexdigest(),
        "ccf_stand_geometry": env["CCF_STAND_GEOMETRY"],
        "ccf_growth_model": env.get("CCF_GROWTH_MODEL"),
        "unexpected_tree_changes": unexpected,
        "markers": markers,
    }
    (out / ("results-" + name + ".json")).write_text(json.dumps(result, indent=2) + "\n")
    print(json.dumps({k: result[k] for k in ("gate", "status", "exit_code", "seconds")}), flush=True)
    return result


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--unity", default=os.environ.get("UNITY_EDITOR"))
    parser.add_argument("--gate", action="append", choices=sorted(GATES))
    parser.add_argument("--out", default=str(ROOT / "Build" / "ThreeSpecies"))
    parser.add_argument("--timeout", type=int, default=1200)
    parser.add_argument("--dry-run", action="store_true")
    args = parser.parse_args()
    if not args.unity and not args.dry_run:
        parser.error("give --unity or set UNITY_EDITOR (no Unity path is assumed)")
    out = Path(args.out)
    out.mkdir(parents=True, exist_ok=True)
    failures = 0
    for name in args.gate or DEFAULT_GATES:
        result = run_gate(name, args.unity or "<unity>", out, args.timeout, args.dry_run)
        if result is not None and result["status"] != "PASS":
            failures += 1
    sys.exit(1 if failures else 0)


if __name__ == "__main__":
    main()
