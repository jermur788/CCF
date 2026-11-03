#!/usr/bin/env python3
"""Final Scenario One Tier-3 complete-loop qualification matrix (verification-only).

Composes the accepted MFM-12, S1-C T-05, S1-D, Rack V2, three-species and completion
runners on one exact lane HEAD. The caller owns the normal machine-wide Unity lock for
every Unity step; this script never acquires or releases it. Evidence is written under
/media/jer/Files/CCF/Verification. No production file is changed.
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
TASK_ID = "FINAL-TIER-3"
OWNER = "T3-FinalTier3"

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
    ("15-s1d-cycle-state", ["python3", "-B", "Tools/Verification/ScenarioOneRepeatedInterventions/run_offline_check.py", "--out", "{out}", "--mutations"]),
    ("16-s1d-previous-proposed", ["python3", "-B", "Tools/Verification/ScenarioOneRepeatedInterventions/PreviousProposed/run_offline_check.py", "--out", "{out}", "--mutations"]),
    ("17-rack-editing", ["python3", "-B", "Tools/Verification/RackV2Editing/run_offline.py", "--out", "{out}"]),
    ("18-rack-spacing", ["python3", "-B", "Tools/Verification/RackV2Spacing/run_offline.py", "--out", "{out}"]),
    ("19-rack-suggested-layout", ["python3", "-B", "Tools/Verification/RackV2SuggestedLayout/run_offline.py", "--out", "{out}"]),
    ("20-rack-confirm-static", ["python3", "-B", "Tools/Verification/RackV2ConfirmMinimap/run_static.py", "--out", "{out}"]),
    ("21-standmap-offline", ["python3", "-B", "Tools/Verification/StandMap/OfflineLogic/run_standmap_check.py", "--out", "{out}"]),
    ("22-s1c-history-offline", ["python3", "-B", "Tools/Verification/S1CManagementHistory/run_t03_offline_check.py", "--out", "{out}"]),
]

# name, runner kind, runner-specific arguments
UNITY_STEPS = [
    ("01-compile", "s1c", ["--gate", "compile"]),
    ("02-objective", "s1c", ["--gate", "objective"]),
    ("03-render", "s1c", ["--gate", "render"]),
    ("04-mfm-approval", "mfm", ["--gate", "approval"]),
    ("05-mfm-coverage-world", "mfm", ["--gate", "coverage-world"]),
    ("06-mfm-save", "mfm", ["--gate", "save"]),
    ("07-mfm-reference", "mfm", ["--gate", "reference"]),
    ("08-mfm-hardening", "mfm", ["--gate", "hardening"]),
    ("09-mfm-extension", "ext", ["--gate", "extension"]),
    ("10-mfm-treatment-review", "ext", ["--gate", "treatment-review"]),
    ("11-mfm-calibration", "mfm11", ["--gate", "calibration"]),
    ("12-guided-v2", "rackux", ["--gate", "guided-v2", "--display", ":84"]),
    ("13-combined-v2", "rackux", ["--gate", "combined-v2", "--display", ":85"]),
    ("14-parity07", "parity07", []),
    ("15-interaction", "mfm", ["--gate", "interaction"]),
    ("16-mixed-density-l40", "three-species", ["MixedDensityVerification", "MIXED_DENSITY_VERIFY_PASS", "0", "0"]),
    ("17-broadleaf-l40", "three-species", ["BroadleafTimberEconomyVerification", "BROADLEAF_TIMBER_VERIFY_PASS", "0", "0"]),
    ("18-broadleaf-e80", "three-species", ["BroadleafTimberEconomyVerification", "BROADLEAF_TIMBER_VERIFY_PASS", "0", "1"]),
    ("19-history-l40", "three-species", ["ScenarioHistoryVerification", "SCENARIO_HISTORY_VERIFY_PASS", "1", "0"]),
    ("20-s1c-t02", "ext", ["--gate", "t02"]),
    ("21-s1c-t03", "ext", ["--gate", "t03"]),
    ("22-s1c-t04", "ext", ["--gate", "t04"]),
    ("23-s1c-t06", "ext", ["--gate", "t06"]),
    ("24-s1d-cap", "ext", ["--gate", "cap"]),
    ("25-s1d-lesson", "ext", ["--gate", "lesson"]),
    ("26-s1d-menu", "ext", ["--gate", "menu"]),
    ("27-s1d-teaching", "ext", ["--gate", "teaching"]),
    ("28-s1d-rack", "ext", ["--gate", "rack"]),
    ("29-s1d-event-id", "ext", ["--gate", "event-id"]),
    ("30-s1d-teaching-latch", "ext", ["--gate", "teaching-latch"]),
    ("31-s1d-guide", "ext", ["--gate", "guide"]),
    ("32-s1d-previous-proposed", "ext", ["--gate", "previous-proposed"]),
    ("33-completion", "ext", ["--gate", "completion"]),
    ("34-completion-model1", "ext", ["--gate", "completion-model1"]),
    ("35-n3", "ext", ["--gate", "n3"]),
    ("36-density-save", "ext", ["--gate", "density-save"]),
    ("37-progress", "ext", ["--gate", "progress"]),
    ("38-rack-map", "rackedit", ["--gate", "map"]),
    ("39-rack-editing", "rackedit", ["--gate", "editing"]),
    ("40-rack-confirm", "rackconfirm", ["--gate", "confirm"]),
    ("41-rack-field", "rackfield", ["--gate", "field"]),
    ("42-rack-save", "rackfield", ["--gate", "save"]),
    ("43-rack-persist-v2", "rackpersist", ["--gate", "v2"]),
    ("44-rack-persist-coldstart", "rackpersist", ["--gate", "coldstart"]),
    ("45-rack-legacy40", "rackpersist", ["--gate", "legacy40"]),
    ("46-rack-o1-e2e", "racko1", []),
    ("47-standgeometry", "qualification", ["StandGeometryVerification.Begin", "Tools/Verification/StandGeometry/StandGeometryVerification.cs", "STAND_GEOMETRY_VERIFY_PASS"]),
    ("48-enlarged80", "qualification", ["Enlarged80Verification.Begin", "Tools/Verification/StandGeometry/Enlarged80Verification.cs", "ENLARGED80_VERIFY_PASS"]),
    ("49-economy", "qualification", ["ScenarioOneEconomyIntegrationVerification.Begin", "Tools/Verification/ScenarioOneEconomyIntegrationVerification.cs", "SCENARIO_ONE_ECONOMY_INTEGRATION_VERIFY_PASS"]),
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
    ap.add_argument("--owner", default=OWNER)
    ap.add_argument("--root", default=str(EVIDENCE / "ScenarioOneFinalTier3-20261103"))
    ap.add_argument("--phase", choices=["offline", "unity", "all"], default="all")
    ap.add_argument("--only", default="")
    ap.add_argument("--keep-going", action="store_true")
    ap.add_argument("--timeout", type=int, default=3600)
    ap.add_argument("--fixtures", default="/media/jer/Files/CCF/Verification/S1D-T08-20261031/fixtures")
    ap.add_argument("--s1c-fixtures", default="/media/jer/Files/CCF/Verification/S1C-T05-Unity-20261029/fixtures")
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
        print("%-30s %-20s exit=%s %ss %s" % (name, status, code, seconds, note), flush=True)

    def want(name):
        return not only or name in only

    def base_env():
        env = dict(os.environ, PYTHONDONTWRITEBYTECODE="1", GIT_OPTIONAL_LOCKS="0",
                   CCF_LOCK_TASK_ID=TASK_ID, CCF_LOCK_OWNER=args.owner, CCF_LOCK_OWNERS=args.owner)
        for key in list(env):
            if key.startswith("CCF_") and key not in ("CCF_LOCK_TASK_ID", "CCF_LOCK_OWNER", "CCF_LOCK_OWNERS"):
                env.pop(key)
        return env

    if args.phase in ("offline", "all"):
        for name, template in OFFLINE:
            if not want(name):
                continue
            out = root / "offline" / name
            cmd = [x.replace("{out}", str(out)) for x in template]
            code, seconds, text = run(cmd, lane, base_env(), root / "logs" / "offline" / (name + ".log"), args.timeout)
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
                or meta.get("task_id") != TASK_ID or meta.get("owner") != args.owner
                or meta.get("head") != args.head or meta.get("lane") != str(lane)
                or status_json.get("unity_editor_pids")):
            print("OWNERSHIP CONFLICT: require the held %s lock for this lane and head" % TASK_ID, file=sys.stderr)
            return 2
        if subprocess.check_output(["git", "-C", str(lane), "rev-parse", "HEAD"], text=True).strip() != args.head:
            print("HEAD mismatch", file=sys.stderr)
            return 2
        if subprocess.check_output(["git", "-C", str(lane), "status", "--porcelain", "--untracked-files=all"], text=True).strip():
            print("lane is dirty", file=sys.stderr)
            return 2
        sizes = "1280x720,1920x1080"
        env = base_env()
        for name, kind, extra in UNITY_STEPS:
            if not want(name):
                continue
            out = root / "unity" / name
            timeout = args.timeout + 300
            if kind == "s1c":
                cmd = ["python3", "-B", "Tools/Verification/S1CObjectiveHistoryUnity/run_gate.py",
                       "--out", str(out), "--lock-id", args.lock_id, "--expected-head", args.head,
                       "--project", str(lane), "--sizes", sizes] + extra
                if name in ("02-objective", "03-render"):
                    cmd += ["--fixtures", args.s1c_fixtures]
            elif kind == "mfm":
                cmd = ["python3", "-B", "Tools/Verification/MfmApprovalBoundary/run_unity_gate.py",
                       "--out", str(out), "--lock-id", args.lock_id, "--task-id", TASK_ID,
                       "--owner", args.owner, "--expected-head", args.head,
                       "--project", str(lane), "--sizes", sizes, "--timeout", str(args.timeout)] + extra
            elif kind == "ext":
                cmd = ["python3", "-B", "Tools/Verification/Mfm12IntegratedQualification/run_extension_gate.py",
                       "--out", str(out), "--lock-id", args.lock_id, "--owner", args.owner,
                       "--expected-head", args.head, "--project", str(lane), "--sizes", sizes,
                       "--timeout", str(args.timeout)] + extra
                if name in ("20-s1c-t02", "21-s1c-t03", "22-s1c-t04", "23-s1c-t06"):
                    cmd += ["--fixtures", args.fixtures, "--prior-evidence", str(root / "unity" / "02-objective" / "evidence")]
            elif kind == "mfm11":
                cmd = ["python3", "-B", "Tools/Verification/Mfm12IntegratedQualification/run_gate.py",
                       "--out", str(out), "--lock-id", args.lock_id, "--owner", args.owner,
                       "--expected-head", args.head, "--project", str(lane), "--sizes", sizes,
                       "--timeout", str(args.timeout)] + extra
            elif kind == "rackux":
                cmd = ["python3", "-B", "Tools/Verification/RackV2NoCompetingAuthor/run_gate.py",
                       "--out", str(out), "--screen", "2560x1440", "--sizes", sizes,
                       "--timeout", str(args.timeout)] + extra
            elif kind == "parity07":
                cmd = ["python3", "-B", "Tools/Verification/Parity07Unity/run_gate.py",
                       "--project", str(lane), "--expected-head", args.head, "--lock-id", args.lock_id,
                       "--task-id", TASK_ID, "--out", str(out), "--sizes", sizes]
            elif kind == "three-species":
                method, marker, interactive, geometry = extra
                cmd = ["python3", "-B", "Tools/Verification/Qualification/run_qualification.py",
                       "--label", name, "--out", str(out), "--method", method,
                       "--file", "Tools/Verification/ThreeSpecies/" + method.split(".")[0] + ".cs",
                       "--marker", marker, "--env", "CCF_TEST_DIRECT_ENTRY=1",
                       "--env", "CCF_STAND_GEOMETRY=" + geometry, "--timeout", str(args.timeout)]
                if interactive == "1":
                    cmd += ["--interactive", "--window", "1280x720"]
            elif kind == "rackedit":
                cmd = ["python3", "-I", "-B", "Tools/Verification/RackV2Editing/run_gate.py",
                       "--out", str(out), "--sizes", sizes] + extra
            elif kind == "rackconfirm":
                cmd = ["python3", "-I", "-B", "Tools/Verification/RackV2ConfirmMinimap/run_gate.py",
                       "--out", str(out), "--sizes", sizes] + extra
            elif kind == "rackfield":
                cmd = ["python3", "-B", "Tools/Verification/RackV2FieldMarking/run_gate.py",
                       "--out", str(out), "--sizes", sizes] + extra
            elif kind == "rackpersist":
                cmd = ["python3", "-B", "Tools/Verification/RackV2Persistence/run_gate.py",
                       "--out", str(out)] + extra
                if name == "44-rack-persist-coldstart":
                    cmd += ["--seed-from", str(root / "unity" / "43-rack-persist-v2")]
            elif kind == "racko1":
                cmd = ["python3", "-I", "-B", "Tools/Verification/RackV2Objective1/run_gate.py",
                       "--name", "RackV2Objective1EndToEnd", "--method", "RackV2Objective1EndToEnd.Begin",
                       "--files", "RackV2Objective1/RackV2Objective1EndToEnd.cs",
                       "--native", "--normal-startup", "--timeout", "1500",
                       "--pass-token", "RACKV2_O1E2E_PASS", "--fail-token", "RACKV2_O1E2E_FAIL",
                       "--out", str(out)]
            elif kind == "qualification":
                method, source, marker = extra
                cmd = ["python3", "-B", "Tools/Verification/Qualification/run_qualification.py",
                       "--label", name, "--out", str(out), "--method", method,
                       "--file", source, "--marker", marker, "--timeout", str(args.timeout)]
            else:
                raise SystemExit("unknown runner " + kind)
            code, seconds, text = run(cmd, lane, env, root / "logs" / "unity" / (name + ".log"), timeout)
            status = "PASS" if code == 0 else "FAIL"
            if status == "FAIL" and not args.keep_going:
                record(name, "unity", status, code, seconds, root / "logs/unity" / (name + ".log"))
                return 1
            record(name, "unity", status, code, seconds, root / "logs/unity" / (name + ".log"))

    print("FINAL_TIER3_MATRIX_DONE steps=%d failures=%d" % (len(summary), sum(1 for s in summary if s["status"] == "FAIL")))
    return 0 if all(s["status"] != "FAIL" for s in summary) else 1


if __name__ == "__main__":
    sys.exit(main())
