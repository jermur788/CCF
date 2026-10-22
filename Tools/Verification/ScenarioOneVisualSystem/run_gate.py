#!/usr/bin/env python3
"""Run one qualified Unity gate the way the previous accepted task did: staged harness files, the qualified env, and (for the guided
gate) a private Xephyr+openbox display with a real-input xdotool driver. Outside the repo on purpose (evidence tooling).
The Unity lock must be held. Staged files + .meta are always removed. Licence is copied (never edited) from the newest folder."""
import argparse, os, shutil, subprocess, sys, time
from pathlib import Path

ROOT = Path(__file__).resolve().parents[3]
UNITY = "/media/jer/ZX20/Unity/6000.6.0f1/Editor/Unity"
LIC = [Path.home() / ".config/unity3d/Unity/licenses", Path.home() / ".var/app/com.unity.UnityHub/config/unity3d/Unity/licenses"]
HERE = Path(__file__).resolve().parent

ap = argparse.ArgumentParser()
ap.add_argument("--name", required=True); ap.add_argument("--method", default="")
ap.add_argument("--files", nargs="*", default=[]); ap.add_argument("--out", required=True)
ap.add_argument("--env", nargs="*", default=[]); ap.add_argument("--real-input", default=None)
ap.add_argument("--private-display", action="store_true"); ap.add_argument("--timeout", type=int, default=1500)
ap.add_argument("--screen", default="1280x720")
ap.add_argument("--compile-only", action="store_true")
ap.add_argument("--normal-startup", action="store_true", help="Exercise normal Editor/Player startup instead of explicit Editor-only harness bypass")
a = ap.parse_args()
out = Path(a.out).resolve(); (out / "evidence").mkdir(parents=True, exist_ok=True)
config = out / "config"
src = max([c for c in LIC if (c / "UnityEntitlementLicense.xml").is_file()], key=lambda c: (c / "UnityEntitlementLicense.xml").stat().st_mtime)
shutil.copytree(src, config / "unity3d/Unity/licenses", dirs_exist_ok=True)
staged = []
for rel in a.files:
    d = ROOT / "Assets/ForestPrototype" / Path(rel).name
    if d.exists(): sys.exit("already present: " + str(d))
    staged.append((ROOT / "Tools/Verification" / rel, d))
env = dict(os.environ, XDG_CONFIG_HOME=str(config), CCF_DIAG_DIR=str(out / "evidence"), CCF_ACCEPTANCE_OUTPUT=str(out / "evidence"))
if not a.normal_startup:
    env["CCF_TEST_DIRECT_ENTRY"] = "1"
for item in a.env:
    k, v = item.split("=", 1); env[k] = v
procs = []
if a.private_display:
    env["DISPLAY"] = ":84"
    xe = subprocess.Popen(["Xephyr", ":84", "-screen", a.screen, "-ac", "-noreset"], stdout=open(out / "xephyr.log", "w"), stderr=subprocess.STDOUT)
    time.sleep(2)
    ob = subprocess.Popen(["openbox"], env=dict(os.environ, DISPLAY=":84"), stdout=open(out / "openbox.log", "w"), stderr=subprocess.STDOUT)
    procs += [ob, xe]; time.sleep(1)
else:
    env.setdefault("DISPLAY", ":0")
log = out / (a.name + ".log")
code = None; t0 = time.monotonic(); driver = None
try:
    for s, d in staged: shutil.copy2(s, d)
    command = [UNITY, "-projectPath", str(ROOT), "-logFile", str(log)]
    command += ["-batchmode", "-nographics", "-quit"] if a.compile_only else ["-executeMethod", a.method]
    (out / "command.json").write_text(__import__("json").dumps(command, indent=2) + "\n")
    unity = subprocess.Popen(command, cwd=ROOT, env=env)
    if a.real_input:
        driver = subprocess.Popen([sys.executable, str(HERE / a.real_input), str(out)], env=dict(os.environ, DISPLAY=":84"), stdout=open(out / "real-input-driver.log", "w"), stderr=subprocess.STDOUT)
    try:
        code = unity.wait(timeout=a.timeout)
    except subprocess.TimeoutExpired:
        unity.kill(); code = -1; print("TIMEOUT")
finally:
    if driver and driver.poll() is None: driver.terminate()
    for p in procs:
        if p.poll() is None: p.terminate()
    for s, d in staged:
        for p in (d, Path(str(d) + ".meta")):
            if p.exists(): p.unlink()
shutil.rmtree(config / "unity3d/Unity/licenses", ignore_errors=True)
text = log.read_text(errors="replace") if log.exists() else ""
keys = [l for l in text.splitlines() if any(k in l for k in ("_PASS", "_FAIL", "error CS", "No valid Unity", "UNVERIFIED", "ONBOARDING", "_VERIFY", "PLAYTHROUGH"))]
(out / "summary-lines.txt").write_text("\n".join(keys) + "\n")
fails = [l for l in keys if "FAIL" in l or "error CS" in l]
print("name=%s exit=%s seconds=%.0f lines=%d fail-ish=%d log=%s" % (a.name, code, time.monotonic() - t0, len(keys), len(fails), log))
for l in fails[:20]: print("  " + l[:240])
for l in keys[-3:]: print("  tail: " + l[:240])
left = [d for _, d in staged if d.exists() or Path(str(d) + ".meta").exists()]
if left: print("LEFTOVER", left); sys.exit(2)

if code != 0 or fails: sys.exit(1)
