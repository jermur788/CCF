#!/usr/bin/env python3
"""Run a RACK-V2 Unity gate against THIS checkout and ALWAYS remove what it staged (RACK-V2-07, adapted from RACK-V2-05/06).

  run_gate.py --gate confirm  --out DIR     the RACK-V2-07 confirm / reopen / minimap gate (real pointer, pointer_driver.py)
  run_gate.py --gate spacing  --out DIR     the RACK-V2-06 spacing gate, unchanged (preservation; includes the RACK-V2-05 editing checks)
  run_gate.py --gate editing  --out DIR     the RACK-V2-05 real-pointer editing gate, unchanged (preservation)
  run_gate.py --gate map      --out DIR     the RACK-V2-04 map planning gate, unchanged (preservation)
  run_gate.py --gate runtime  --out DIR     the RACK-V2-03 runtime gate, unchanged (preservation)

The Unity lock must already be held by the caller and this checkout must be a registered Unity lane (never the non-Unity
authoring worktree). The staged .cs and its generated .meta are removed afterwards, even on failure. The Unity licence is COPIED
(never edited) into a throwaway XDG_CONFIG_HOME inside DIR; that copy is deleted again and only a sha prefix is recorded. With
--private-display the Editor runs in a nested Xephyr + openbox so it cannot disturb the desktop.

Editor hang after its own shutdown (log ends at 'Cleanup mono', SIGTERM ignored): once the gate has printed its verdict, written
`finished` and the log has been quiet for --grace seconds, the Editor is killed BY PID and reaper.txt records exactly that, so a real
PASS is not turned into a FAIL. An Editor that has not reached 'Cleanup mono' is never reaped as a pass.
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
    "confirm": dict(file=HERE / "RackV2ConfirmMinimapVerification.cs", method="RackV2ConfirmMinimapVerification.Begin", pass_marker="RACK_V2_CONFIRM_MINIMAP_UNITY_PASS", fail_marker="RACK_V2_CONFIRM_MINIMAP_UNITY_FAIL", driver=True),
    "spacing": dict(file=ROOT / "Tools/Verification/RackV2Spacing/RackV2SpacingVerification.cs", method="RackV2SpacingVerification.Begin", pass_marker="RACK_V2_EDITING_UNITY_PASS", fail_marker="RACK_V2_EDITING_UNITY_FAIL", driver=True),
    "editing": dict(file=EDITING / "RackV2EditingVerification.cs", method="RackV2EditingVerification.Begin", pass_marker="RACK_V2_EDITING_UNITY_PASS", fail_marker="RACK_V2_EDITING_UNITY_FAIL", driver=True),
    "map": dict(file=ROOT / "Tools/Verification/RackV2MapPlanning/RackV2MapPlanningVerification.cs", method="RackV2MapPlanningVerification.Begin", pass_marker="RACK_V2_MAP_PLANNING_UNITY_PASS", fail_marker="RACK_V2_MAP_PLANNING_UNITY_FAIL", driver=False),
    "runtime": dict(file=ROOT / "Tools/Verification/RackV2Runtime/RackV2RuntimeVerification.cs", method="RackV2RuntimeVerification.Begin", pass_marker="RACKV2RUNTIME_VERIFY_PASS", fail_marker="RACKV2RUNTIME_VERIFY_FAIL", driver=False),
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

gate = GATES[args.gate]
out = Path(args.out).resolve()
(out / "evidence").mkdir(parents=True, exist_ok=True)
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
    unity = subprocess.Popen([UNITY, "-projectPath", str(ROOT), "-logFile", str(log), "-executeMethod", gate["method"]], cwd=ROOT, env=env)
    (out / "unity.pid").write_text(str(unity.pid))
    if gate["driver"]:
        driver = subprocess.Popen([sys.executable, "-I", "-B", str(EDITING / "pointer_driver.py"), str(driver_dir), args.display],
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
    for path in (dest, Path(str(dest) + ".meta")):
        if path.exists():
            path.unlink()
    shutil.rmtree(config / "unity3d/Unity/licenses", ignore_errors=True)

text = log.read_text(errors="replace") if log.exists() else ""
keys = [l for l in text.splitlines() if any(k in l for k in ("_PASS", "_FAIL", "error CS", "No valid Unity", "RACKV2EDIT_NOTE", "RACKV2EDIT_STATE", "RACKV2CM_NOTE", "RACKV2CM_STATE", "RACKV2MAP_STATE", "Exception")) and "VisualScripting" not in l]
(out / "summary-lines.txt").write_text("\n".join(keys) + "\n")
passed = gate["pass_marker"] in text and gate["fail_marker"] not in text and "error CS" not in text
exit_ok = code == 0 or (reaped and passed)
print("gate=%s exit=%s reaped=%s seconds=%.0f licence-sha=%s pass-marker=%s log=%s" % (args.gate, code, reaped, time.monotonic() - started, licence_sha, passed, log))
for l in keys[-12:]:
    print("  " + l[:300])
left = [p for p in (dest, Path(str(dest) + ".meta")) if p.exists()]
if left:
    print("LEFTOVER", left)
    sys.exit(2)
sys.exit(0 if exit_ok and passed else 1)
