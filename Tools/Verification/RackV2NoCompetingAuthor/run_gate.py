#!/usr/bin/env python3
"""Bounded RACK-V2-08 gate runner adapted from RACK-V2-07. Requires the caller's machine-wide lock.
Staged files/metas and copied licence are always removed. Each gate uses an isolated persistent-data path
and private Xephyr display. Model selection is explicit. Native input is delivered only in that display.
Same post-verdict/Cleanup-mono U1 shutdown policy as the inspected RACK-V2-07 runner.
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

SCOPE = HERE
GUIDED = [ROOT / "Tools/Verification/ScenarioOneRackUx/GuidedOnboardingVerification.cs",
          ROOT / "Tools/Verification/SequentialObjectives/SequentialObjectivesVerification.cs",
          ROOT / "Tools/Verification/ScenarioOneRackUx/RackUxShots.cs",
          HERE / "GuidedRackV2Verification.cs", HERE / "RackAuthoringNativeInput.cs"]
GATES = {
    "guided-v2": dict(files=GUIDED, method="GuidedOnboardingVerification.Begin", pass_marker="GUIDED_ONBOARDING_PASS", fail_marker="GUIDED_ONBOARDING_FAIL", driver="native", model="v2", geometry="1"),
    "guided-legacy": dict(files=GUIDED, method="GuidedOnboardingVerification.Begin", pass_marker="GUIDED_ONBOARDING_PASS", fail_marker="GUIDED_ONBOARDING_FAIL", driver="guided", model="legacy", geometry="1"),
    "combined-v2": dict(files=[ROOT / "Tools/Verification/ScenarioOneCombined/CombinedHudComposition.cs"], method="CombinedHudComposition.Begin", pass_marker="CHC_DONE", fail_marker="CHC_CHECK_FAIL", driver=False, model="v2", geometry="1"),
    "combined-legacy": dict(files=[ROOT / "Tools/Verification/ScenarioOneCombined/CombinedHudComposition.cs"], method="CombinedHudComposition.Begin", pass_marker="CHC_DONE", fail_marker="CHC_CHECK_FAIL", driver=False, model="legacy", geometry="1"),
    "legacy40": dict(files=[HERE / "Legacy40RackAuthoringVerification.cs", HERE / "RackAuthoringNativeInput.cs", ROOT / "Tools/Verification/ScenarioOneRackUx/RackUxShots.cs"], method="Legacy40RackAuthoringVerification.Begin", pass_marker="LEGACY40_RACK_AUTHORING_PASS", fail_marker="LEGACY40_RACK_AUTHORING_FAIL", driver="native", model="legacy", geometry="0"),
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
for name in ("evidence", "driver", "config"):
    previous = out / name
    if previous.exists() and any(previous.iterdir()):
        sys.exit("refuse reused fixture/input state; choose a new output directory: " + str(previous))
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

staged = [(source, ROOT / "Assets/ForestPrototype" / source.name) for source in gate["files"]]
for source, dest in staged:
    if dest.exists() or Path(str(dest) + ".meta").exists(): sys.exit("already staged: " + str(dest))
    if not source.is_file(): sys.exit("missing source: " + str(source))

env = dict(os.environ, XDG_CONFIG_HOME=str(config), CCF_DIAG_DIR=str(out / "evidence"), CCF_ACCEPTANCE_OUTPUT=str(out / "evidence"),
           CCF_UI_SIZES=args.sizes, CCF_TEST_DIRECT_ENTRY="1", PYTHONDONTWRITEBYTECODE="1", DISPLAY=args.display)
for name in ("TMPDIR", "TMP", "TEMP", "XDG_CACHE_HOME"):
    env.pop(name, None)  # never leave scratch on the exFAT SSD: the licensing client stalls there
if gate["driver"]:
    env["CCF_DRIVER_DIR"] = str(driver_dir)
env.update(CCF_RACK_VERIFY_MODEL=gate["model"], CCF_STAND_GEOMETRY=gate["geometry"], CCF_GROWTH_MODEL="2", CCF_SEQUENCE_VERIFY="1")
if gate["driver"] == "guided" and args.display != ":84": sys.exit("existing guided driver requires private display :84")
env.pop("CCF_COMBINED_SECTIONS", None)  # qualification always retains every HUD section
for item in args.env:
    k, v = item.split("=", 1)
    if k in {"CCF_RACK_VERIFY_MODEL", "CCF_STAND_GEOMETRY", "CCF_GROWTH_MODEL", "CCF_SEQUENCE_VERIFY", "CCF_COMBINED_SECTIONS", "CCF_TEST_DIRECT_ENTRY"}:
        sys.exit("qualification fixture/section overrides are forbidden: " + k)
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
    for source, dest in staged: shutil.copy2(source, dest)
    unity = subprocess.Popen([UNITY, "-projectPath", str(ROOT), "-logFile", str(log), "-executeMethod", gate["method"]], cwd=ROOT, env=env)
    (out / "unity.pid").write_text(str(unity.pid))
    if gate["driver"]:
        command = ([sys.executable, "-I", "-B", str(ROOT / "Tools/Verification/ScenarioOneVisualSystem/real_input_guided.py"), str(out)]
                   if gate["driver"] == "guided" else [sys.executable, "-I", "-B", str(HERE / "native_input.py"), str(driver_dir), args.display])
        driver = subprocess.Popen(command, stdout=open(out / "driver.stdout", "w"), stderr=subprocess.STDOUT)
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
    for source, dest in staged:
        for path in (dest, Path(str(dest) + ".meta")):
            if path.exists(): path.unlink()
    shutil.rmtree(config / "unity3d/Unity/licenses", ignore_errors=True)

text = log.read_text(errors="replace") if log.exists() else ""
keys = [l for l in text.splitlines() if any(k in l for k in ("_PASS", "_FAIL", "error CS", "No valid Unity", "RACKV2EDIT_NOTE", "RACKV2EDIT_STATE", "RACKV2CM_NOTE", "RACKV2CM_STATE", "RACKV2MAP_STATE", "CHC_DONE", "Exception")) and "VisualScripting" not in l]
(out / "summary-lines.txt").write_text("\n".join(keys) + "\n")
passed = gate["pass_marker"] in text and gate["fail_marker"] not in text and "error CS" not in text
if args.gate.startswith("combined"): passed = passed and "fail=0" in text
if args.gate == "guided-legacy": passed = passed and driver is not None and driver.poll() == 0
exit_ok = code == 0 or (reaped and passed)
print("gate=%s exit=%s reaped=%s seconds=%.0f licence-sha=%s pass-marker=%s log=%s" % (args.gate, code, reaped, time.monotonic() - started, licence_sha, passed, log))
for l in keys[-12:]:
    print("  " + l[:300])
left = [p for source, dest in staged for p in (dest, Path(str(dest) + ".meta")) if p.exists()]
if left:
    print("LEFTOVER", left)
    sys.exit(2)
sys.exit(0 if exit_ok and passed else 1)
