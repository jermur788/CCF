#!/usr/bin/env python3
"""RACK-V2-09 Tier-2 runner. Requires clean committed owned lane and held machine lock.
Stages only the selected disposable harness (+ existing native-input helper for field).
Private Xephyr/openbox and isolated config/license copy; no scene/disk-save writes.
Runs field/native OS input, unchanged runtime, or unchanged v21 save gate. Removes scripts,
metas and license copy even on failure. Reaps only its own Editor PID after verdict + Cleanup mono.
"""
import argparse
import hashlib
import os
import shutil
import subprocess
import sys
import time
from pathlib import Path

ROOT = Path(__file__).resolve().parents[3]
HERE = Path(__file__).resolve().parent
EDITING = ROOT / "Tools/Verification/RackV2Editing"
UNITY = "/media/jer/ZX20/Unity/6000.6.0f1/Editor/Unity"
LICENCES = [Path.home() / ".config/unity3d/Unity/licenses", Path.home() / ".var/app/com.unity.UnityHub/config/unity3d/Unity/licenses"]

GATES = {
    "field": dict(file=HERE / "RackV2FieldMarkingVerification.cs", method="RackV2FieldMarkingVerification.Begin", pass_marker="RACK_V2_FIELD_MARKING_PASS", fail_marker="RACK_V2_FIELD_MARKING_FAIL", driver=True),
    "runtime": dict(file=ROOT / "Tools/Verification/RackV2Runtime/RackV2RuntimeVerification.cs", method="RackV2RuntimeVerification.Begin", pass_marker="RACKV2RUNTIME_VERIFY_PASS", fail_marker="RACKV2RUNTIME_VERIFY_FAIL", driver=False),
    "save": dict(file=ROOT / "Tools/Verification/RackV2Save/RackV2SaveVerification.cs", method="RackV2SaveVerification.Begin", pass_marker="RACKV2SAVE_VERIFY_PASS", fail_marker="RACKV2SAVE_VERIFY_FAIL", driver=False),
}

ap = argparse.ArgumentParser()
ap.add_argument("--gate", required=True, choices=sorted(GATES))
ap.add_argument("--out", required=True)
ap.add_argument("--display", default=":84")
ap.add_argument("--screen", default="1920x1080")
ap.add_argument("--sizes", default="1280x720,1920x1080")
ap.add_argument("--timeout", type=int, default=2400)
ap.add_argument("--grace", type=int, default=90)
ap.add_argument("--env", nargs="*", default=[])
args = ap.parse_args()

# Require the normal machine-wide lock to name this clean committed checkout.
import json
head = subprocess.check_output(["git", "rev-parse", "HEAD"], cwd=ROOT, text=True).strip()
status = json.loads(subprocess.check_output(["python3", "-B", "/media/jer/ZX20/CCF-Ops/scripts/unity-lock", "status", "--json"], text=True))
metadata = status.get("metadata") or {}
if status.get("status") != "held" or metadata.get("owner") != "Codex-RACK-V2-09":
    sys.exit("RACK-V2-09 machine-wide Unity lock required")
if metadata.get("head") != head or Path(metadata.get("lane", "")).resolve() != ROOT:
    sys.exit("lock must name this exact candidate and lane")
# Retain the full lock record in each run.
if subprocess.check_output(["git", "status", "--porcelain"], cwd=ROOT, text=True).strip():
    sys.exit("commit/clean candidate before Unity qualification")
gate = GATES[args.gate]
out = Path(args.out).resolve()
if out.exists() and any(out.iterdir()): sys.exit("choose a fresh evidence directory")
(out / "evidence").mkdir(parents=True, exist_ok=True)
(out / "lock.json").write_text(json.dumps(status, indent=2))
(out / "candidate.txt").write_text(head + "\n")
driver_dir = out / "driver"
driver_dir.mkdir(exist_ok=True)

found = [c for c in LICENCES if (c / "UnityEntitlementLicense.xml").is_file()]
if not found:
    sys.exit("no Unity licence file found")
newest = max(found, key=lambda c: (c / "UnityEntitlementLicense.xml").stat().st_mtime)
config = out / "config"
shutil.copytree(newest, config / "unity3d/Unity/licenses", dirs_exist_ok=True)
licence_sha = hashlib.sha256((newest / "UnityEntitlementLicense.xml").read_bytes()).hexdigest()[:12]

dest = ROOT / "Assets/ForestPrototype" / gate["file"].name
if dest.exists():
    sys.exit("already present, investigate first: " + str(dest))
if not gate["file"].is_file():
    sys.exit("gate source missing: " + str(gate["file"]))

native_source = ROOT / "Tools/Verification/RackV2NoCompetingAuthor/RackAuthoringNativeInput.cs"
native_dest = ROOT / "Assets/ForestPrototype" / native_source.name
staged_paths = [dest, Path(str(dest) + ".meta")]
if gate["driver"]:
    staged_paths += [native_dest, Path(str(native_dest) + ".meta")]
if any(p.exists() for p in staged_paths): sys.exit("staged harness or meta already present")

env = dict(os.environ, XDG_CONFIG_HOME=str(config), CCF_DIAG_DIR=str(out / "evidence"), CCF_ACCEPTANCE_OUTPUT=str(out / "evidence"),
           CCF_UI_SIZES=args.sizes, CCF_TEST_DIRECT_ENTRY="1", PYTHONDONTWRITEBYTECODE="1", DISPLAY=args.display)
for name in ("TMPDIR", "TMP", "TEMP", "XDG_CACHE_HOME"):
    env.pop(name, None)  # never leave scratch on the exFAT SSD: the licensing client stalls there
if gate["driver"]:
    env["CCF_DRIVER_DIR"] = str(driver_dir)
for item in args.env:
    k, v = item.split("=", 1)
    env[k] = v

procs = []
xephyr = subprocess.Popen(["Xephyr", args.display, "-screen", args.screen, "-ac", "-noreset"], stdout=open(out / "xephyr.log", "w"), stderr=subprocess.STDOUT)
time.sleep(2)
procs.append(xephyr)
openbox = subprocess.Popen(["openbox"], env=dict(os.environ, DISPLAY=args.display), stdout=open(out / "openbox.log", "w"), stderr=subprocess.STDOUT)
procs.append(openbox)
time.sleep(1)

log = out / (args.gate + ".log")
code = None
reaped = False
started = time.monotonic()
unity = driver = focuser = None
try:
    shutil.copy2(gate["file"], dest)
    if gate["driver"]: shutil.copy2(native_source, native_dest)
    unity = subprocess.Popen([UNITY, "-projectPath", str(ROOT), "-logFile", str(log), "-executeMethod", gate["method"]], cwd=ROOT, env=env)
    (out / "unity.pid").write_text(str(unity.pid))
    if gate["driver"]:
        driver = subprocess.Popen([sys.executable, "-I", "-B", str(ROOT / "Tools/Verification/RackV2NoCompetingAuthor/native_input.py"), str(driver_dir), args.display],
                                  stdout=open(out / "driver.stdout", "w"), stderr=subprocess.STDOUT)
    w, h = args.screen.split("x")
    script = ("for i in $(seq 1 1200); do w=$(xdotool search --onlyvisible --name 'Unity -' | head -1); "
              "[ -z \"$w\" ] && w=$(xdotool search --onlyvisible --class Unity | tail -1); "
              "[ -n \"$w\" ] && { xdotool windowmove $w 0 0 2>/dev/null; xdotool windowsize $w %s %s 2>/dev/null; xdotool windowactivate --sync $w 2>/dev/null; }; sleep 2; done") % (w, h)
    focuser = subprocess.Popen(["bash", "-c", script], env=dict(os.environ, DISPLAY=args.display))

    finished_at = None
    while True:
        code = unity.poll()
        if code is not None:
            break
        elapsed = time.monotonic() - started
        if elapsed > args.timeout:
            unity.kill()
            code = -1
            print("TIMEOUT")
            break
        text = log.read_text(errors="replace") if log.exists() else ""
        verdict = gate["pass_marker"] in text or gate["fail_marker"] in text
        if verdict and finished_at is None:
            finished_at = time.monotonic()
        if finished_at is not None and time.monotonic() - finished_at > args.grace and "Cleanup mono" in text:
            (out / "reaper.txt").write_text("reaping hung U1 Editor %s after verdict and Cleanup mono; pid %d\n" % (log, unity.pid))
            unity.kill()
            unity.wait(timeout=30)
            code = -9
            reaped = True
            break
        if driver is not None and driver.poll() not in (None, 0):
            print("DRIVER FAILED", (driver_dir / "driver-error").read_text() if (driver_dir / "driver-error").exists() else "")
        time.sleep(2)
finally:
    (driver_dir / "finished").write_text("run ended")
    if driver is not None and driver.poll() is None:
        try:
            driver.wait(timeout=10)
        except subprocess.TimeoutExpired:
            driver.terminate()
    if focuser and focuser.poll() is None:
        focuser.terminate()
    if unity is not None and unity.poll() is None:
        unity.kill()
    for p in reversed(procs):
        if p.poll() is None:
            p.terminate()
    for path in staged_paths:
        if path.exists():
            path.unlink()
    shutil.rmtree(config / "unity3d/Unity/licenses", ignore_errors=True)

text = log.read_text(errors="replace") if log.exists() else ""
keys = [l for l in text.splitlines() if any(k in l for k in ("_PASS", "_FAIL", "RACKFIELD_CHECK", "error CS", "No valid Unity", "RACKV2EDIT_NOTE", "RACKV2EDIT_STATE", "RACKV2CM_NOTE", "RACKV2CM_STATE", "RACKV2MAP_STATE", "Exception")) and "VisualScripting" not in l]
(out / "summary-lines.txt").write_text("\n".join(keys) + "\n")
passed = gate["pass_marker"] in text and gate["fail_marker"] not in text and "error CS" not in text
exit_ok = code == 0 or (reaped and passed)
print("gate=%s exit=%s reaped=%s seconds=%.0f licence-sha=%s pass-marker=%s log=%s" % (args.gate, code, reaped, time.monotonic() - started, licence_sha, passed, log))
for l in keys[-12:]:
    print("  " + l[:300])
left = [p for p in staged_paths if p.exists()]
if left:
    print("LEFTOVER", left)
    sys.exit(2)
sys.exit(0 if exit_ok and passed else 1)
