#!/usr/bin/env python3
"""MFM-12 integrated qualification matrix (verification-only).

Runs the accepted offline gates and the disposable Unity gates on one exact lane HEAD.
The caller owns the normal machine-wide Unity lock for every Unity step; this script
never acquires or releases it. Evidence is written under Files/CCF/Verification.
"""
import argparse
import json
import os
import subprocess
import sys
import time
from pathlib import Path

UNITY = "/media/jer/ZX20/Unity/6000.6.0f1/Editor/Unity"
LOCK = "/media/jer/ZX20/CCF-Ops/scripts/unity-lock"
EVIDENCE = Path("/media/jer/Files/CCF/Verification")

OFFLINE = [
    ("01-site", ["python3", "-B", "Tools/Verification/SiteInfrastructure/run_site_check.py", "--out", "{out}"]),
    ("02-growth-integrated", ["python3", "-B", "Tools/Calibration/OakBeechGrowth/check_integrated_v1.py", "--out", "{out}/integrated.json"]),
    ("03-timber", ["python3", "-B", "Tools/Verification/ThreeSpecies/OfflineLogic/run_offline_check.py", "--suite", "timber", "--out", "{out}"]),
    ("04-density", ["python3", "-B", "Tools/Verification/ThreeSpecies/OfflineLogic/run_offline_check.py", "--suite", "density", "--out", "{out}"]),
    ("05-history", ["python3", "-B", "Tools/Verification/ThreeSpecies/OfflineLogic/run_offline_check.py", "--suite", "history", "--out", "{out}"]),
    ("06-pruning", ["python3", "-B", "Tools/Verification/ThreeSpecies/OfflineLogic/run_pruning_check.py", "--out", "{out}"]),
    ("07-prescription-core", ["python3", "-B", "Tools/Verification/MfmPrescriptionCore/run_prescription_core_offline.py", "--out", "{out}"]),
    ("08-candidates", ["python3", "-B", "Tools/Verification/MfmCropTreeCandidates/run_candidates_offline.py", "--out", "{out}"]),
    ("09-coverage-world", ["python3", "-B", "Tools/Verification/MfmCoverageWorld/run_coverage_world_offline.py", "--out", "{out}"]),
    ("10-approval-boundary", ["python3", "-B", "Tools/Verification/MfmApprovalBoundary/run_approval_boundary_offline.py", "--out", "{out}"]),
    ("11-treatment-review", ["python3", "-B", "Tools/Verification/MfmTreatmentReview/run_treatment_review_offline.py", "--out", "{out}"]),
    ("12-save-provenance", ["python3", "-B", "Tools/Verification/MfmSaveProvenance/run_offline_check.py", "--out", "{out}"]),
    ("13-save-mutation-controls", ["python3", "-B", "Tools/Verification/MfmSaveProvenance/run_mutation_controls.py", "--out", "{out}"]),
    ("14-parity08-save", ["python3", "-B", "Tools/Verification/Parity08Save/run_offline_check.py", "--out", "{out}"]),
]

UNITY_STEPS = [
    ("01-compile", "mfm-approval", "compile"),
    ("02-approval", "mfm-approval", "approval"),
    ("03-coverage-world", "mfm-approval", "coverage-world"),
    ("04-save", "mfm-approval", "save"),
    ("05-reference", "mfm-approval", "reference"),
    ("06-hardening", "mfm-approval", "hardening"),
    ("07-extension", "mfm09", "extension"),
    ("08-treatment-review", "mfm09", "treatment-review"),
    ("09-calibration-witness", "mfm11", "calibration"),
    ("10-guided-v2", "rackux", "guided-v2"),
    ("11-combined-v2", "rackux", "combined-v2"),
    ("12-parity07", "parity07", "parity07"),
    ("13-interaction", "mfm-approval", "interaction"),
    ("14-mixed-density-l40", "three-species", "MixedDensityVerification"),
    ("15-broadleaf-l40", "three-species", "BroadleafTimberEconomyVerification"),
    ("16-broadleaf-e80", "three-species", "BroadleafTimberEconomyVerification"),
    ("17-history-l40", "three-species", "ScenarioHistoryVerification"),
]

DENSITY_ACCEPTED = ("COMPOSITE_DENSITY_FAIL checks=107 failures=3", "LONG_RUN_L40_B_BEECH_DENSE", "LONG_RUN_L40_F_BEECH_YOUNG", "LONG_RUN_E80_F_BEECH_YOUNG")


def run(cmd, cwd, env, log_path, timeout):
    log_path.parent.mkdir(parents=True, exist_ok=True)
    started = time.monotonic()
    with log_path.open("w") as log:
        log.write("$ " + " ".join(cmd) + "\n\n")
        log.flush()
        try:
            p = subprocess.run(cmd, cwd=str(cwd), env=env, stdout=log, stderr=subprocess.STDOUT, timeout=timeout)
            code = p.returncode
        except subprocess.TimeoutExpired:
            log.write("\nTIMEOUT after %s s\n" % timeout)
            code = 124
    return code, round(time.monotonic() - started, 1), log_path.read_text(errors="replace")


def main():
    ap = argparse.ArgumentParser()
    ap.add_argument("--lane", default="/media/jer/ZX20/CCF-U1")
    ap.add_argument("--head", required=True)
    ap.add_argument("--lock-id", default="")
    ap.add_argument("--owner", default="")
    ap.add_argument("--root", default=str(EVIDENCE / "MFM12-IntegratedQualification-Rerun-20261102"))
    ap.add_argument("--phase", choices=["offline", "unity", "all"], default="all")
    ap.add_argument("--only", default="")
    ap.add_argument("--keep-going", action="store_true")
    ap.add_argument("--timeout", type=int, default=3600)
    args = ap.parse_args()
    lane = Path(args.lane).resolve()
    root = Path(args.root).resolve()
    root.mkdir(parents=True, exist_ok=True)
    summary = []
    only = set(x for x in args.only.split(",") if x)

    def record(name, phase, status, code, seconds, log, note=""):
        summary.append(dict(step=name, phase=phase, status=status, exit=code, seconds=seconds,
                            log=str(log), note=note))
        (root / "summary.json").write_text(json.dumps(summary, indent=2) + "\n")
        print("%-28s %-20s exit=%s %ss %s" % (name, status, code, seconds, note), flush=True)

    def want(name):
        return not only or name in only

    if args.phase in ("offline", "all"):
        for name, template in OFFLINE:
            if not want(name):
                continue
            out = root / "offline" / name
            cmd = [x.replace("{out}", str(out)) for x in template]
            env = dict(os.environ, PYTHONDONTWRITEBYTECODE="1", GIT_OPTIONAL_LOCKS="0")
            for key in list(env):
                if key.startswith("CCF_"):
                    env.pop(key)
            code, seconds, text = run(cmd, lane, env, root / "logs" / (phase := "offline") / (name + ".log"), args.timeout)
            status = "PASS" if code == 0 else "FAIL"
            note = ""
            if name == "04-density" and code != 0:
                if all(marker in text for marker in DENSITY_ACCEPTED) and "MaxRemovedBasalShare" not in text:
                    status, note = "ACCEPTED_LIMITATION", "three accepted pure-Beech R2 misses; 0.40 unchanged"
            if status == "FAIL" and not args.keep_going:
                record(name, "offline", status, code, seconds, root / "logs/offline" / (name + ".log"), note)
                return 1
            record(name, "offline", status, code, seconds, root / "logs/offline" / (name + ".log"), note)

    if args.phase in ("unity", "all"):
        status_json = json.loads(subprocess.check_output(["python3", "-B", LOCK, "status", "--json"], text=True))
        meta = status_json.get("metadata") or {}
        if (status_json.get("status") != "held" or meta.get("lock_id") != args.lock_id
                or meta.get("task_id") != "MFM-12" or meta.get("owner") != args.owner
                or meta.get("head") != args.head or meta.get("lane") != str(lane)
                or status_json.get("unity_editor_pids")):
            print("OWNERSHIP CONFLICT: require the held MFM-12 lock for this lane and head", file=sys.stderr)
            return 2
        if subprocess.check_output(["git", "-C", str(lane), "rev-parse", "HEAD"], text=True).strip() != args.head:
            print("HEAD mismatch", file=sys.stderr)
            return 2
        if subprocess.check_output(["git", "-C", str(lane), "status", "--porcelain", "--untracked-files=all"], text=True).strip():
            print("lane is dirty", file=sys.stderr)
            return 2
        sizes = "1280x720,1920x1080"
        for name, runner, gate in UNITY_STEPS:
            if not want(name):
                continue
            out = root / "unity" / name
            if runner in ("mfm-approval", "mfm09", "mfm11"):
                if runner == "mfm-approval":
                    base = ["python3", "-B", "Tools/Verification/MfmApprovalBoundary/run_unity_gate.py",
                            "--gate", gate, "--out", str(out), "--lock-id", args.lock_id,
                            "--task-id", "MFM-12", "--owner", args.owner, "--expected-head", args.head,
                            "--project", str(lane), "--sizes", sizes, "--timeout", str(args.timeout)]
                elif runner == "mfm09":
                    base = ["python3", "-B", "Tools/Verification/Mfm12IntegratedQualification/run_extension_gate.py",
                            "--gate", gate, "--out", str(out), "--lock-id", args.lock_id,
                            "--owner", args.owner, "--expected-head", args.head,
                            "--project", str(lane), "--sizes", sizes, "--timeout", str(args.timeout)]
                else:
                    base = ["python3", "-B", "Tools/Verification/Mfm12IntegratedQualification/run_gate.py",
                            "--gate", gate, "--out", str(out), "--lock-id", args.lock_id,
                            "--owner", args.owner, "--expected-head", args.head]
            elif runner == "rackux":
                display = ":84" if gate == "guided-v2" else ":85"
                base = ["python3", "-B", "Tools/Verification/RackV2NoCompetingAuthor/run_gate.py",
                        "--gate", gate, "--out", str(out), "--display", display,
                        "--screen", "2560x1440", "--sizes", sizes, "--timeout", str(args.timeout)]
            elif runner == "three-species":
                if gate == "ScenarioHistoryVerification":
                    method, marker, interactive, geometry = ("ScenarioHistoryVerification.Begin", "SCENARIO_HISTORY_VERIFY_PASS", True, "0")
                elif gate == "MixedDensityVerification":
                    method, marker, interactive, geometry = ("MixedDensityVerification.Begin", "MIXED_DENSITY_VERIFY_PASS", False, "0")
                else:
                    method, marker, interactive = ("BroadleafTimberEconomyVerification.Begin", "BROADLEAF_TIMBER_VERIFY_PASS", False)
                    geometry = "1" if name.endswith("e80") else "0"
                base = ["python3", "-B", "Tools/Verification/Qualification/run_qualification.py",
                        "--label", name, "--out", str(out), "--method", method,
                        "--file", "Tools/Verification/ThreeSpecies/" + gate + ".cs",
                        "--marker", marker, "--env", "CCF_TEST_DIRECT_ENTRY=1",
                        "--env", "CCF_STAND_GEOMETRY=" + geometry, "--timeout", str(args.timeout)]
                if interactive:
                    base += ["--interactive", "--window", "1280x720"]
                env = dict(os.environ, PYTHONDONTWRITEBYTECODE="1", GIT_OPTIONAL_LOCKS="0")
                for key in list(env):
                    if key.startswith("CCF_"):
                        env.pop(key)
                code, seconds, text = run(base, lane, env, root / "logs/unity" / (name + ".log"), args.timeout + 300)
                status = "PASS" if code == 0 else "FAIL"
                if status == "FAIL" and not args.keep_going:
                    record(name, "unity", status, code, seconds, root / "logs/unity" / (name + ".log"))
                    return 1
                record(name, "unity", status, code, seconds, root / "logs/unity" / (name + ".log"))
                continue
            elif runner == "parity07":
                base = ["python3", "-B", "Tools/Verification/Parity07Unity/run_gate.py",
                        "--project", str(lane), "--expected-head", args.head, "--lock-id", args.lock_id,
                        "--task-id", "MFM-12", "--out", str(out)]
            else:
                raise SystemExit("unknown runner " + runner)
            env = dict(os.environ, PYTHONDONTWRITEBYTECODE="1", GIT_OPTIONAL_LOCKS="0")
            for key in list(env):
                if key.startswith("CCF_"):
                    env.pop(key)
            code, seconds, text = run(base, lane, env, root / "logs/unity" / (name + ".log"), args.timeout + 300)
            status = "PASS" if code == 0 else "FAIL"
            if status == "FAIL" and not args.keep_going:
                record(name, "unity", status, code, seconds, root / "logs/unity" / (name + ".log"))
                return 1
            record(name, "unity", status, code, seconds, root / "logs/unity" / (name + ".log"))

    print("MFM12_MATRIX_DONE steps=%d failures=%d" % (len(summary), sum(1 for s in summary if s["status"] == "FAIL")))
    return 0 if all(s["status"] != "FAIL" for s in summary) else 1


if __name__ == "__main__":
    sys.exit(main())
