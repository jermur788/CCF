#!/usr/bin/env python3
"""RACK-V2-12 Unity gate runner: stage disposable harness sources into a registered Unity lane, run ONE Editor, ALWAYS remove them.

Derived from the RACK-V2-COMBINED-REPAIR-01 driver run_gate.py (same CLI) and the RACK-V2-09 runner (built-in post-verdict reaper,
native OS-input driver). Differences: the lock must be held for task RACK-V2-12 naming the lane's exact HEAD; native input is optional
(--native); the Editor is reaped only after its own pass/fail verdict AND its "Cleanup mono" marker.

  run_gate.py --name RackV2Objective1EndToEnd --method RackV2Objective1EndToEnd.Begin \
      --files RackV2Objective1/RackV2Objective1EndToEnd.cs --native --normal-startup \
      --pass-token RACKV2_O1E2E_PASS --fail-token RACKV2_O1E2E_FAIL --out /media/jer/Files/CCF/Verification/<run>

--files are relative to the LANE's Tools/Verification (so the staged bytes are the candidate's); --ext-files are absolute paths of
external disposable sources (diagnostics on a baseline that does not contain them). Evidence goes outside the repository. The Unity
licence is COPIED into a throwaway XDG_CONFIG_HOME and deleted after the run (only a sha prefix is kept). TMPDIR is kept off ZX20.
"""
import argparse, hashlib, json, os, shutil, subprocess, sys, time
from pathlib import Path

LANE = Path(os.environ.get("CCF_GATE_LANE", "/media/jer/ZX20/CCF-U1")).resolve()
HERE = Path(__file__).resolve().parent
UNITY = "/media/jer/ZX20/Unity/6000.6.0f1/Editor/Unity"
LOCK = "/media/jer/ZX20/CCF-Ops/scripts/unity-lock"
LIC = [Path.home() / ".config/unity3d/Unity/licenses", Path.home() / ".var/app/com.unity.UnityHub/config/unity3d/Unity/licenses"]
TASK = "RACK-V2-12"

ap = argparse.ArgumentParser()
ap.add_argument("--name", required=True); ap.add_argument("--method", required=True)
ap.add_argument("--files", nargs="*", default=[]); ap.add_argument("--ext-files", nargs="*", default=[])
ap.add_argument("--out", required=True); ap.add_argument("--env", nargs="*", default=[])
ap.add_argument("--private-display", action="store_true", help="accepted for CLI compatibility; the display is always private")
ap.add_argument("--screen", default="1920x1080"); ap.add_argument("--display", default=":84")
ap.add_argument("--keep-focus", action="store_true", help="accepted for CLI compatibility; focus is always kept")
ap.add_argument("--timeout", type=int, default=1800); ap.add_argument("--grace", type=int, default=90)
ap.add_argument("--sizes", default=None); ap.add_argument("--pass-token", required=True, nargs="+")
ap.add_argument("--fail-token", default=None, help="verdict marker that also allows the post-verdict reaper")
ap.add_argument("--seed-pref", nargs="*", default=[]); ap.add_argument("--normal-startup", action="store_true")
ap.add_argument("--native", action="store_true", help="start native_input.py on the private display (CCF_DRIVER_DIR)")
a = ap.parse_args()
if a.display in (":0", ":1"): sys.exit("refuse the normal desktop display")

head = subprocess.check_output(["git", "rev-parse", "HEAD"], cwd=LANE, text=True).strip()
status = json.loads(subprocess.check_output(["python3", "-B", LOCK, "status", "--json"], text=True))
meta = status.get("metadata") or {}
if status.get("status") != "held" or meta.get("task_id") != TASK or meta.get("head") != head or Path(meta.get("lane", "/")).resolve() != LANE:
    sys.exit("OWNERSHIP CONFLICT: the machine-wide lock must be held for %s naming this lane and its exact HEAD" % TASK)
if status.get("unity_editor_pids"): sys.exit("another Unity Editor is running: %s" % status["unity_editor_pids"])
if subprocess.check_output(["git", "status", "--porcelain=v1", "-uall"], cwd=LANE, text=True).strip(): sys.exit("DIRTY LANE: refuse staging")
out = Path(a.out).resolve()
if not str(out).startswith("/media/jer/Files/CCF/Verification/"): sys.exit("evidence must live under /media/jer/Files/CCF/Verification/")
for sub in ("evidence", "config", "driver"):
    if (out / sub).exists() and any((out / sub).iterdir()): sys.exit("choose a fresh output directory: %s" % (out / sub))
(out / "evidence").mkdir(parents=True, exist_ok=True)
(out / "lock.json").write_text(json.dumps(status, indent=2) + "\n")
(out / "candidate.txt").write_text(head + "\n")
driver_dir = out / "driver"; driver_dir.mkdir(exist_ok=True)

config = out / "config"
found = [c for c in LIC if (c / "UnityEntitlementLicense.xml").is_file()]
if not found: sys.exit("no Unity licence file found")
src = max(found, key=lambda c: (c / "UnityEntitlementLicense.xml").stat().st_mtime)
shutil.copytree(src, config / "unity3d/Unity/licenses", dirs_exist_ok=True)
if a.seed_pref:
    pdir = config / "unity3d/DefaultCompany/CCF"; pdir.mkdir(parents=True, exist_ok=True)
    body = "".join('\t<pref name="%s" type="int">%s</pref>\n' % tuple(item.split("=", 1)) for item in a.seed_pref)
    (pdir / "prefs").write_text('<unity_prefs version_major="1" version_minor="1">\n' + body + "</unity_prefs>\n")

staged = []
for rel in a.files:
    staged.append((LANE / "Tools/Verification" / rel, LANE / "Assets/ForestPrototype" / Path(rel).name))
for ext in a.ext_files:
    staged.append((Path(ext).resolve(), LANE / "Assets/ForestPrototype" / Path(ext).name))
for s, d in staged:
    if not s.is_file(): sys.exit("missing source: %s" % s)
    if d.exists() or Path(str(d) + ".meta").exists(): sys.exit("already present, investigate first: %s" % d)
(out / "staged-sources.json").write_text(json.dumps([{"source": str(s), "sha256": hashlib.sha256(s.read_bytes()).hexdigest()} for s, _ in staged], indent=1) + "\n")

env = dict(os.environ, XDG_CONFIG_HOME=str(config), CCF_DIAG_DIR=str(out / "evidence"), CCF_ACCEPTANCE_OUTPUT=str(out / "evidence"),
           PYTHONDONTWRITEBYTECODE="1", DISPLAY=a.display, TMPDIR="/tmp/")
for k in ("TMP", "TEMP", "XDG_CACHE_HOME"): env.pop(k, None)  # never leave scratch on exFAT ZX20: the licensing client stalls there
if not a.normal_startup: env["CCF_TEST_DIRECT_ENTRY"] = "1"
if a.sizes: env["CCF_UI_SIZES"] = a.sizes
if a.native: env["CCF_DRIVER_DIR"] = str(driver_dir)
for item in a.env:
    k, v = item.split("=", 1); env[k] = v

procs = []
xe = subprocess.Popen(["Xephyr", a.display, "-screen", a.screen, "-ac", "-noreset"], stdout=open(out / "xephyr.log", "w"), stderr=subprocess.STDOUT)
time.sleep(2)
ob = subprocess.Popen(["openbox"], env=dict(os.environ, DISPLAY=a.display), stdout=open(out / "openbox.log", "w"), stderr=subprocess.STDOUT)
procs += [ob, xe]; time.sleep(1)

log = out / (a.name + ".log")
code = None; reaped = False; t0 = time.monotonic(); unity = driver = focuser = None
verdicts = list(a.pass_token[:1]) + ([a.fail_token] if a.fail_token else [])
try:
    for s, d in staged: shutil.copy2(s, d)
    unity = subprocess.Popen([UNITY, "-projectPath", str(LANE), "-job-worker-count", "2", "-logFile", str(log), "-executeMethod", a.method], cwd=LANE, env=env)
    (out / "unity.pid").write_text(str(unity.pid))
    if a.native:
        driver = subprocess.Popen([sys.executable, "-I", "-B", str(HERE / "native_input.py"), str(driver_dir), a.display],
                                  stdout=open(out / "driver.stdout", "w"), stderr=subprocess.STDOUT)
    w_, h_ = a.screen.split("x")
    script = ("for i in $(seq 1 1200); do w=$(xdotool search --onlyvisible --name 'Unity -' | head -1); "
              "[ -z \"$w\" ] && w=$(xdotool search --onlyvisible --class Unity | tail -1); "
              "[ -n \"$w\" ] && { xdotool windowmove $w 0 0 2>/dev/null; xdotool windowsize $w %s %s 2>/dev/null; xdotool windowactivate --sync $w 2>/dev/null; }; sleep 2; done") % (w_, h_)
    focuser = subprocess.Popen(["bash", "-c", script], env=dict(os.environ, DISPLAY=a.display))
    finished_at = None
    while True:
        code = unity.poll()
        if code is not None: break
        if time.monotonic() - t0 > a.timeout:
            unity.kill(); unity.wait(timeout=30); code = -1; print("TIMEOUT"); break
        text = log.read_text(errors="replace") if log.exists() else ""
        if finished_at is None and any(v in text for v in verdicts): finished_at = time.monotonic()
        if finished_at is not None and time.monotonic() - finished_at > a.grace and "\nCleanup mono" in text:
            (out / "reaper.txt").write_text("reaping hung U1 Editor %s after its verdict and Cleanup mono; pid %d\n" % (log, unity.pid))
            unity.kill(); unity.wait(timeout=30); code = -9; reaped = True; break
        time.sleep(2)
finally:
    (driver_dir / "finished").write_text("run ended")
    if driver is not None and driver.poll() is None:
        try: driver.wait(timeout=10)
        except subprocess.TimeoutExpired: driver.terminate()
    if focuser and focuser.poll() is None: focuser.terminate()
    if unity is not None and unity.poll() is None: unity.kill()
    for p in procs:
        if p.poll() is None: p.terminate()
    for _, d in staged:
        for p in (d, Path(str(d) + ".meta")):
            if p.exists(): p.unlink()

lic_dir = config / "unity3d/Unity/licenses"; removed = []
if lic_dir.exists():
    for p in sorted(lic_dir.rglob("*")):
        if p.is_file(): removed.append({"file": p.name, "sha256_prefix": hashlib.sha256(p.read_bytes()).hexdigest()[:8]})
    shutil.rmtree(lic_dir, ignore_errors=True)
(out / "licence-copy-removed.json").write_text(json.dumps({"selected_source": str(src), "removed_after_run": removed}, indent=1) + "\n")
text = log.read_text(errors="replace") if log.exists() else ""
keys = [l for l in text.splitlines() if any(k in l for k in ("_PASS", "_FAIL", "FAIL ", "error CS", "No valid Unity", "_DONE", "_DEFECT", "_MEASURE", "Exception"))
        and "VisualScripting" not in l]
(out / "summary-lines.txt").write_text("\n".join(keys) + "\n")
fails = [l for l in keys if ("FAIL" in l and "_MEASURE" not in l) or "error CS" in l]
left = [d for _, d in staged if d.exists() or Path(str(d) + ".meta").exists()]
tokens_ok = all(t in text for t in a.pass_token)
passed = (code == 0 or reaped) and tokens_ok and not fails and not left
print("name=%s exit=%s reaped=%s seconds=%.0f pass=%s log=%s" % (a.name, code, reaped, time.monotonic() - t0, passed, log))
for l in fails[:25]: print("  " + l[:300])
for l in keys[-8:]: print("  tail: " + l[:300])
(out / "result.json").write_text(json.dumps({"name": a.name, "status": "PASS" if passed else "FAIL", "exit": code, "reaped_after_verdict": reaped,
    "required_tokens": a.pass_token, "tokens_present": {t: (t in text) for t in a.pass_token}, "fail_lines": len(fails),
    "seconds": round(time.monotonic() - t0, 1), "head": head, "lane": str(LANE), "leftover": [str(p) for p in left]}, indent=2) + "\n")
if left: print("LEFTOVER", left); sys.exit(2)
sys.exit(0 if passed else 1)
