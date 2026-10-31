#!/usr/bin/env python3
"""MFM-08 Tier-2 (#156) runner. Verification only.

Requires the exact production candidate checked out clean in the Unity lane AND the normal machine-wide lock held by this task.
Stages only the disposable gate (+ the existing native-input helper), runs it under a private Xephyr/openbox with an isolated
licence/config copy, and removes every staged file and the licence copy even on failure. Reaps only its own Editor PID after the
verdict. Writes evidence only under --out (outside the repository).
"""
import argparse, hashlib, json, os, shutil, subprocess, sys, time
from pathlib import Path

HERE = Path(__file__).resolve().parent
LANE = Path("/media/jer/ZX20/CCF-U1")
CANDIDATE = "d55ec86a3f0bf13820f2080197b6b0f43d832e06"
LOCK_TASK = "MFM-08-156-TIER2-UNITY"
UNITY = "/media/jer/ZX20/Unity/6000.6.0f1/Editor/Unity"
LICENCES = [Path.home() / ".config/unity3d/Unity/licenses", Path.home() / ".var/app/com.unity.UnityHub/config/unity3d/Unity/licenses"]

ap = argparse.ArgumentParser()
ap.add_argument("--out", required=True)
ap.add_argument("--candidate", default=CANDIDATE)
ap.add_argument("--lock-task", default=LOCK_TASK)
ap.add_argument("--display", default=":84")
ap.add_argument("--screen", default="1920x1080")
ap.add_argument("--timeout", type=int, default=3000)
ap.add_argument("--grace", type=int, default=90)
args = ap.parse_args()
CANDIDATE, LOCK_TASK = args.candidate, args.lock_task

head = subprocess.check_output(["git", "rev-parse", "HEAD"], cwd=LANE, text=True).strip()
status = json.loads(subprocess.check_output(["python3", "-B", "/media/jer/ZX20/CCF-Ops/scripts/unity-lock", "--json", "status"], text=True))
meta = status.get("metadata") or {}
if status.get("status") != "held" or meta.get("task_id") != LOCK_TASK:
    sys.exit("the normal machine-wide Unity lock must be held by " + LOCK_TASK)
if head != CANDIDATE or meta.get("head") != CANDIDATE or Path(meta.get("lane", "")).resolve() != LANE:
    sys.exit("lock/lane must name the exact candidate")
if subprocess.check_output(["git", "status", "--porcelain"], cwd=LANE, text=True).strip():
    sys.exit("candidate lane is not clean")
out = Path(args.out).resolve()
if out.exists() and any(out.iterdir()):
    sys.exit("choose a fresh evidence directory")
(out / "evidence").mkdir(parents=True, exist_ok=True)
(out / "lock.json").write_text(json.dumps(status, indent=2))
(out / "candidate.txt").write_text(head + "\n")
driver_dir = out / "driver"; driver_dir.mkdir(exist_ok=True)

found = [c for c in LICENCES if (c / "UnityEntitlementLicense.xml").is_file()]
if not found:
    sys.exit("no Unity licence file found")
newest = max(found, key=lambda c: (c / "UnityEntitlementLicense.xml").stat().st_mtime)
config = out / "config"
shutil.copytree(newest, config / "unity3d/Unity/licenses", dirs_exist_ok=True)

gate_src = HERE / "MfmTreatmentReviewUnityGate.cs"
native_src = LANE / "Tools/Verification/RackV2NoCompetingAuthor/RackAuthoringNativeInput.cs"
driver_py = LANE / "Tools/Verification/RackV2NoCompetingAuthor/native_input.py"
staged = []
for src in (gate_src, native_src):
    dest = LANE / "Assets/ForestPrototype" / src.name
    if dest.exists() or Path(str(dest) + ".meta").exists():
        sys.exit("already present, investigate first: " + str(dest))
    staged += [dest, Path(str(dest) + ".meta")]

env = dict(os.environ, XDG_CONFIG_HOME=str(config), CCF_DIAG_DIR=str(out / "evidence"), CCF_ACCEPTANCE_OUTPUT=str(out / "evidence"),
           CCF_DRIVER_DIR=str(driver_dir), CCF_TEST_DIRECT_ENTRY="1", PYTHONDONTWRITEBYTECODE="1", DISPLAY=args.display)
for name in ("TMPDIR", "TMP", "TEMP", "XDG_CACHE_HOME"):
    env.pop(name, None)  # scratch on exFAT stalls the licensing client

procs = []
xephyr = subprocess.Popen(["Xephyr", args.display, "-screen", args.screen, "-ac", "-noreset"], stdout=open(out / "xephyr.log", "w"), stderr=subprocess.STDOUT)
procs.append(xephyr); time.sleep(2)
procs.append(subprocess.Popen(["openbox"], env=dict(os.environ, DISPLAY=args.display), stdout=open(out / "openbox.log", "w"), stderr=subprocess.STDOUT)); time.sleep(1)

log = out / "unity.log"
code = None; reaped = False; started = time.monotonic(); unity = driver = focuser = None
PASS, FAIL = "MFM08_UNITY_PASS", "MFM08_UNITY_FAIL"
try:
    for src in (gate_src, native_src):
        shutil.copy2(src, LANE / "Assets/ForestPrototype" / src.name)
    unity = subprocess.Popen([UNITY, "-projectPath", str(LANE), "-logFile", str(log), "-executeMethod", "MfmTreatmentReviewUnityGate.Begin"], cwd=LANE, env=env)
    (out / "unity.pid").write_text(str(unity.pid))
    driver = subprocess.Popen([sys.executable, "-I", "-B", str(driver_py), str(driver_dir), args.display], stdout=open(out / "driver.stdout", "w"), stderr=subprocess.STDOUT)
    w, h = args.screen.split("x")
    script = ("for i in $(seq 1 1500); do w=$(xdotool search --onlyvisible --name 'Unity -' | head -1); "
              "[ -z \"$w\" ] && w=$(xdotool search --onlyvisible --class Unity | tail -1); "
              "[ -n \"$w\" ] && { xdotool windowmove $w 0 0 2>/dev/null; xdotool windowsize $w %s %s 2>/dev/null; xdotool windowactivate --sync $w 2>/dev/null; }; sleep 2; done") % (w, h)
    focuser = subprocess.Popen(["bash", "-c", script], env=dict(os.environ, DISPLAY=args.display))
    finished_at = None
    while True:
        code = unity.poll()
        if code is not None:
            break
        if time.monotonic() - started > args.timeout:
            unity.kill(); code = -1; print("TIMEOUT"); break
        text = log.read_text(errors="replace") if log.exists() else ""
        if "error CS" in text and "Compilation failed" in text or text.count("error CS") > 0 and "Scripts have compiler errors" in text:
            (out / "compile-error.txt").write_text("harness/project compile error; stopping own Editor pid %d\n" % unity.pid)
            unity.kill(); unity.wait(timeout=30); code = -2; break
        if (PASS in text or FAIL in text) and finished_at is None:
            finished_at = time.monotonic()
        if finished_at is not None and time.monotonic() - finished_at > args.grace and "Cleanup mono" in text:
            (out / "reaper.txt").write_text("reaped own Editor pid %d after verdict and Cleanup mono\n" % unity.pid)
            unity.kill(); unity.wait(timeout=30); code = -9; reaped = True; break
        if driver.poll() not in (None, 0):
            print("DRIVER FAILED"); break
        time.sleep(2)
finally:
    (driver_dir / "finished").write_text("run ended")
    if driver is not None and driver.poll() is None:
        try: driver.wait(timeout=10)
        except subprocess.TimeoutExpired: driver.terminate()
    if focuser and focuser.poll() is None: focuser.terminate()
    if unity is not None and unity.poll() is None: unity.kill()
    for p in reversed(procs):
        if p.poll() is None: p.terminate()
    for path in staged:
        if path.exists(): path.unlink()
    shutil.rmtree(config / "unity3d/Unity/licenses", ignore_errors=True)

text = log.read_text(errors="replace") if log.exists() else ""
keys = [l for l in text.splitlines() if any(k in l for k in ("MFM08_", "error CS", "No valid Unity", "Exception")) and "VisualScripting" not in l]
(out / "summary-lines.txt").write_text("\n".join(keys) + "\n")
passed = PASS in text and FAIL not in text and "error CS" not in text
print("exit=%s reaped=%s seconds=%.0f pass-marker=%s log=%s" % (code, reaped, time.monotonic() - started, passed, log))
for l in keys[-8:]:
    print("  " + l[:300])
left = [p for p in staged if p.exists()]
if left:
    print("LEFTOVER", left); sys.exit(2)
sys.exit(0 if (code == 0 or (reaped and passed)) and passed else 1)
