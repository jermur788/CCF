#!/usr/bin/env python3
"""Run the disposable Enlarged80Verification gate (80B, D-056) once, in batch mode, one Editor at a time.

Stages Enlarged80Verification.cs into Assets/ForestPrototype, always removes it and its .meta, uses an isolated
Unity config under Build/Enlarged80/config (licence copied from the user's installed licence) and writes
Build/Enlarged80/<label>.log and <label>.json. It never writes the player's save file or saves the scene.

Usage: run_enlarged80.py <label> [--b3-mode plain|authored-pin] [--candidate-sha <40-hex>]

B3 evidence witness (#347, #333 section 4d): the harness also logs ENLARGED80_B3W lines. --b3-mode authored-pin is the harness-only
control that puts the player on the authored start before every ResetWorld; plain (default) changes nothing. The JSON gains a "b3"
object: EVIDENCE_COMPLETE or EVIDENCE_INCOMPLETE with the missing record kinds. That is never a B3 verdict; the Manager decides B3.
Duplicate plain runs are made by running this script twice with different labels.
"""
import json, os, re, shutil, subprocess, sys, time
from pathlib import Path

ROOT = Path(__file__).resolve().parents[3]
UNITY = "/media/jer/ZX20/Unity/6000.6.0f1/Editor/Unity"
GATE = "Enlarged80Verification"
SOURCE = Path(__file__).with_name(GATE + ".cs")
TARGET = ROOT / "Assets" / "ForestPrototype" / (GATE + ".cs")
OUT = ROOT / "Build" / "Enlarged80"


# Record kinds a complete witness must contain, as (substring that must appear on some B3 line) pairs.
B3_REQUIRED = [
    "kind=START", "kind=IDENTITY phase=pre", "kind=IDENTITY phase=post", "kind=IDENTITY_SCENE phase=pre",
    "kind=IDENTITY_PACKAGE_FILE phase=pre path=Packages/manifest.json", "kind=IDENTITY_PACKAGE_FILE phase=pre path=Packages/packages-lock.json",
    "kind=IDENTITY_PACKAGE_CACHE phase=pre", "kind=POSE label=harness-start", "kind=POSE label=reset1-entry", "kind=POSE label=reset1-pre-Generate",
    "kind=POSE label=pre-TryBeginReferencePreview", "kind=POSE label=post-TryBeginReferencePreview", "kind=POSE label=post-EndReferencePreview+0",
    "kind=POSE label=post-EndReferencePreview+1", "kind=POSE label=post-EndReferencePreview+2", "kind=ANCHORS reset=1", "kind=ANCHOR reset=1",
    "kind=CLEARANCE_OMITTED_CORE reset=1", "kind=SEED reset=1", "kind=TREES reset=1", "kind=OMITTED_CORE reset=1",
    "kind=STEP reset=1 step=1", "kind=STEP reset=1 step=2", "kind=STEP reset=1 step=3", "kind=STEP reset=1 step=4", "kind=STEP reset=1 step=5",
    "kind=ARCHIVE phase=before", "kind=ARCHIVE phase=after", "kind=LEGACY40_ANCHORS phase=before", "kind=LEGACY40_ANCHORS phase=after",
    "kind=SUMMARY",
]


def b3_status(markers):
    """EVIDENCE_COMPLETE only if the harness says so AND every required record kind is present. Never a B3 verdict."""
    lines = [l for l in markers if "ENLARGED80_B3W" in l]
    missing = [r for r in B3_REQUIRED if not any(r in l for l in lines)]
    # POSE labels may carry a trailing detail (e.g. reset1-entry_model=1), so a substring match on the label prefix is intended.
    summary = [l for l in lines if "kind=SUMMARY" in l]
    verdict = re.search(r"witness=(EVIDENCE_COMPLETE|EVIDENCE_INCOMPLETE)", summary[-1]).group(1) if summary and re.search(r"witness=(EVIDENCE_COMPLETE|EVIDENCE_INCOMPLETE)", summary[-1]) else "NO_SUMMARY"
    unavailable = [l for l in lines if "kind=UNAVAILABLE" in l]
    status = "EVIDENCE_COMPLETE" if verdict == "EVIDENCE_COMPLETE" and not missing and not unavailable else "EVIDENCE_INCOMPLETE"
    return dict(status=status, harness_summary=verdict, missing_record_kinds=missing, unavailable_count=len(unavailable),
                lines=len(lines), b3_verdict="NOT_DECIDED_BY_HARNESS")


def main():
    args = sys.argv[1:]
    mode, candidate = "plain", None
    while "--b3-mode" in args:
        i = args.index("--b3-mode"); mode = args[i + 1]; del args[i:i + 2]
    while "--candidate-sha" in args:
        i = args.index("--candidate-sha"); candidate = args[i + 1]; del args[i:i + 2]
    if mode not in ("plain", "authored-pin"):
        raise SystemExit("--b3-mode must be plain or authored-pin")
    if candidate is not None and not re.fullmatch(r"[0-9a-f]{40}", candidate):
        raise SystemExit("--candidate-sha must be a full lowercase 40-hex Git SHA")
    label = args[0] if args else "run"
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
    head = subprocess.run(["git", "-C", str(ROOT), "rev-parse", "HEAD"], capture_output=True, text=True).stdout.strip()
    env.update(CCF_B3_MODE=mode, CCF_B3_LABEL=label, CCF_B3_HEAD=head if re.fullmatch(r"[0-9a-f]{40}", head) else "UNAVAILABLE")
    if candidate:
        env["CCF_B3_CANDIDATE_SHA"] = candidate
    else:
        env.pop("CCF_B3_CANDIDATE_SHA", None)
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
                  seconds=round(time.monotonic() - start, 1), b3=dict(b3_status(markers), mode=mode, candidate=candidate or "UNAVAILABLE"),
                  markers=markers)
    (OUT / (label + ".json")).write_text(json.dumps(result, indent=2) + "\n")
    print(json.dumps({k: result[k] for k in ["label", "status", "exit_code", "seconds"]}), flush=True)
    print("   B3 witness:", result["b3"]["status"], "(not a B3 verdict)", flush=True)
    for line in markers[-6:]:
        print("   ", line[:260])
    return 0 if passed else 1


if __name__ == "__main__":
    sys.exit(main())
