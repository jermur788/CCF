#!/usr/bin/env python3
"""Run one disposable Unity harness against THIS checkout (stage -> run -> ALWAYS remove staged files and their .meta), the way the
repo's own gate runners do. Evidence tooling for SCENARIO-ONE-PROFESSIONAL-MAP-REDESIGN-V1; the evidence it writes goes outside the repository.

  run_unity_harness.py --name ProfessionalMapVerification --method ProfessionalMapVerification.Begin --files ScenarioOneProfessionalMap/ProfessionalMapVerification.cs --out DIR
    [--private-display --screen 1920x1080] [--env K=V ...] [--keep-focus] [--sizes 1280x720,1920x1080]

The Unity lock must already be held by the caller. The licence is COPIED (never edited) from the newest of the host and Hub folders
into a throwaway XDG_CONFIG_HOME. With --private-display the Editor runs in a nested Xephyr + openbox so it cannot disturb the user's
desktop and its window owns focus; --keep-focus re-activates the Editor window with xdotool during startup.
"""
import argparse, os, shutil, subprocess, sys, time
from pathlib import Path

ROOT = Path(__file__).resolve().parents[3]
UNITY = "/media/jer/ZX20/Unity/6000.6.0f1/Editor/Unity"
LIC = [Path.home() / ".config/unity3d/Unity/licenses", Path.home() / ".var/app/com.unity.UnityHub/config/unity3d/Unity/licenses"]

ap = argparse.ArgumentParser()
ap.add_argument("--name", required=True); ap.add_argument("--method", required=True)
ap.add_argument("--files", nargs="*", default=[]); ap.add_argument("--out", required=True)
ap.add_argument("--env", nargs="*", default=[]); ap.add_argument("--private-display", action="store_true")
ap.add_argument("--screen", default="1920x1080"); ap.add_argument("--timeout", type=int, default=1800)
ap.add_argument("--keep-focus", action="store_true"); ap.add_argument("--display", default=":84")
ap.add_argument("--sizes", default="1280x720,1920x1080")
a = ap.parse_args()

out = Path(a.out).resolve(); (out / "evidence").mkdir(parents=True, exist_ok=True)
config = out / "config"
found = [c for c in LIC if (c / "UnityEntitlementLicense.xml").is_file()]
if not found: sys.exit("no Unity licence file found")
src = max(found, key=lambda c: (c / "UnityEntitlementLicense.xml").stat().st_mtime)
shutil.copytree(src, config / "unity3d/Unity/licenses", dirs_exist_ok=True)

staged = []
for rel in a.files:
    dest = ROOT / "Assets/ForestPrototype" / Path(rel).name
    if dest.exists(): sys.exit("already present, investigate first: " + str(dest))
    staged.append((ROOT / "Tools/Verification" / rel, dest))

env = dict(os.environ, XDG_CONFIG_HOME=str(config), CCF_DIAG_DIR=str(out / "evidence"), CCF_ACCEPTANCE_OUTPUT=str(out / "evidence"),
           CCF_UI_SIZES=a.sizes, PYTHONDONTWRITEBYTECODE="1")
for item in a.env:
    k, v = item.split("=", 1); env[k] = v

procs = []
if a.private_display:
    env["DISPLAY"] = a.display
    xe = subprocess.Popen(["Xephyr", a.display, "-screen", a.screen, "-ac", "-noreset"], stdout=open(out / "xephyr.log", "w"), stderr=subprocess.STDOUT)
    time.sleep(2)
    ob = subprocess.Popen(["openbox"], env=dict(os.environ, DISPLAY=a.display), stdout=open(out / "openbox.log", "w"), stderr=subprocess.STDOUT)
    procs += [ob, xe]; time.sleep(1)
else:
    env.setdefault("DISPLAY", ":0")

log = out / (a.name + ".log")
code = None; t0 = time.monotonic(); focuser = None; unity = None
try:
    for s, d in staged: shutil.copy2(s, d)
    unity = subprocess.Popen([UNITY, "-projectPath", str(ROOT), "-logFile", str(log), "-executeMethod", a.method], cwd=ROOT, env=env)
    if a.keep_focus and a.private_display:
        # Keep the Editor window focused, and make it fill the private display: Unity starts with a small main window (about 1280 x 345), which
        # draws the Game view at 0.25x and skews pointer travel. Resizing is repeated while the window settles; it is a no-op once the size is right.
        w_, h_ = a.screen.split("x")
        script = ("for i in $(seq 1 400); do w=$(xdotool search --onlyvisible --name 'Unity -' | head -1); "
                  "[ -z \"$w\" ] && w=$(xdotool search --onlyvisible --class Unity | tail -1); "
                  "[ -n \"$w\" ] && { xdotool windowmove $w 0 0 2>/dev/null; xdotool windowsize $w %s %s 2>/dev/null; xdotool windowactivate --sync $w 2>/dev/null; }; sleep 2; done") % (w_, h_)
        focuser = subprocess.Popen(["bash", "-c", script], env=dict(os.environ, DISPLAY=a.display))
    try:
        code = unity.wait(timeout=a.timeout)
    except subprocess.TimeoutExpired:
        unity.kill(); code = -1; print("TIMEOUT")
finally:
    if focuser and focuser.poll() is None: focuser.terminate()
    for p in procs:
        if p.poll() is None: p.terminate()
    for s, d in staged:
        for p in (d, Path(str(d) + ".meta")):
            if p.exists(): p.unlink()

text = log.read_text(errors="replace") if log.exists() else ""
keys = [l for l in text.splitlines() if any(k in l for k in ("_PASS", "_FAIL", "error CS", "No valid Unity", "UNVERIFIED", "CAMLOOK_", "UXP_", "PMV_PASS_ALL", "PMV_FAIL", "MAPSHOTS_", "MENUSHOTS_", "_DONE", "Exception"))
        and "VisualScripting" not in l]
(out / "summary-lines.txt").write_text("\n".join(keys) + "\n")
fails = [l for l in keys if "FAIL" in l or "error CS" in l]
print("name=%s exit=%s seconds=%.0f lines=%d fail-ish=%d log=%s" % (a.name, code, time.monotonic() - t0, len(keys), len(fails), log))
for l in fails[:25]: print("  " + l[:260])
for l in keys[-8:]: print("  tail: " + l[:300])
left = [d for _, d in staged if d.exists() or Path(str(d) + ".meta").exists()]
if left: print("LEFTOVER", left); sys.exit(2)
