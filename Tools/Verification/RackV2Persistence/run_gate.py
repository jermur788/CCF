#!/usr/bin/env python3
"""RACK-V2-11 Tier-2 persistence runner. Inspect before running; run with `python3 -B`.

Refuses unless: the machine-wide Unity lock is held by owner ClaudeA-RACK-V2-11 / task RACK-V2-11 naming this
exact HEAD and this checkout as its lane; this checkout is a registered Unity lane in CCF-Ops lanes.json; the
tree is clean; the evidence directory is fresh and outside ZX20.

Gates:
  v2         RackV2PersistenceVerification, CCF_RACKV2_11_MODE=v2 (Enlarged80 production new game)
  legacy40   RackV2PersistenceVerification, CCF_RACKV2_11_MODE=legacy40, CCF_STAND_GEOMETRY=0
  coldstart  RackV2PersistenceVerification, CCF_RACKV2_11_MODE=coldstart; --seed-from <passed v2 run dir>
             seeds that run's exported partial-mark save into this run's fresh isolated save slot
  save03b    the unchanged RACK-V2-03B harness Tools/Verification/RackV2Save (in-memory v21 schema rules)

Save safety: XDG_CONFIG_HOME is a fresh per-run directory, so Application.persistentDataPath (and its
forest-save.json) is private to the run; the harness also backs up/restores that slot. The player's real
~/.config/unity3d/DefaultCompany/CCF/forest-save.json(.bak) and the frozen Reference Future v1 resource are
SHA-256 recorded before and after and must be unchanged.

The harness is copied into Assets/ForestPrototype and removed with its .meta in the finally path. After the
Editor exits, the known Editor rewrites (.vscode/settings.json and five Section Five materials) are captured
as a patch and restored from HEAD; any other changed path fails the run and is left for inspection. The Editor
may be reaped only after a verdict and the Editor's own Cleanup mono marker. No Windows or Player build.
"""
import argparse
import hashlib
import json
import os
import shutil
import subprocess
import sys
import time
from pathlib import Path

ROOT = Path(__file__).resolve().parents[3]
HERE = Path(__file__).resolve().parent
UNITY = "/media/jer/ZX20/Unity/6000.6.0f1/Editor/Unity"
LOCK = "/media/jer/ZX20/CCF-Ops/scripts/unity-lock"
LANES = Path("/media/jer/ZX20/CCF-Ops/lanes.json")
OWNER, TASK = "ClaudeA-RACK-V2-11", "RACK-V2-11"
LICENCES = [Path.home() / ".config/unity3d/Unity/licenses", Path.home() / ".var/app/com.unity.UnityHub/config/unity3d/Unity/licenses"]
PROTECTED = [
    Path.home() / ".config/unity3d/DefaultCompany/CCF/forest-save.json",
    Path.home() / ".config/unity3d/DefaultCompany/CCF/forest-save.json.bak",
    ROOT / "Assets/ForestPrototype/ScenarioOne/Resources/ScenarioOneReferenceFutureV1.bytes",
    ROOT / "Assets/ForestPrototype/ScenarioOne/Resources/ScenarioOneReferenceFutureV1.bytes.meta",
]
KNOWN_SIDE_EFFECTS = {
    ".vscode/settings.json",
    "Assets/ForestPrototype/Art/SectionFive/Bilberry_Type_Cover_01/Materials/Bilberry_Leaves.mat",
    "Assets/ForestPrototype/Art/SectionFive/Bilberry_Type_Cover_01/Materials/Bilberry_Stems.mat",
    "Assets/ForestPrototype/Art/SectionFive/Woodland_Forbs_01/Materials/Forb_Flower_Centre.mat",
    "Assets/ForestPrototype/Art/SectionFive/Woodland_Forbs_01/Materials/Forb_Leaves.mat",
    "Assets/ForestPrototype/Art/SectionFive/Woodland_Forbs_01/Materials/Forb_Stems.mat",
}
PERSIST = dict(file=HERE / "RackV2PersistenceVerification.cs", method="RackV2PersistenceVerification.Begin",
               pass_marker="RACKV2PERSIST_VERIFY_PASS", fail_marker="RACKV2PERSIST_VERIFY_FAIL")
GATES = {
    "v2": dict(PERSIST, env={"CCF_RACKV2_11_MODE": "v2"}),
    "legacy40": dict(PERSIST, env={"CCF_RACKV2_11_MODE": "legacy40", "CCF_STAND_GEOMETRY": "0"}),
    "coldstart": dict(PERSIST, env={"CCF_RACKV2_11_MODE": "coldstart"}),
    "save03b": dict(file=ROOT / "Tools/Verification/RackV2Save/RackV2SaveVerification.cs", method="RackV2SaveVerification.Begin",
                    pass_marker="RACKV2SAVE_VERIFY_PASS", fail_marker="RACKV2SAVE_VERIFY_FAIL", env={}),
}


def git(*a):
    return subprocess.check_output(["git", *a], cwd=ROOT, text=True)


def sha(path):
    return hashlib.sha256(path.read_bytes()).hexdigest() if path.is_file() else "absent"


ap = argparse.ArgumentParser()
ap.add_argument("--gate", required=True, choices=sorted(GATES))
ap.add_argument("--out", required=True)
ap.add_argument("--seed-from", help="coldstart only: a passed v2 run directory")
ap.add_argument("--display", default=":86")
ap.add_argument("--screen", default="1920x1080")
ap.add_argument("--timeout", type=int, default=2400)
ap.add_argument("--grace", type=int, default=90)
args = ap.parse_args()
gate = GATES[args.gate]

head = git("rev-parse", "HEAD").strip()
status = json.loads(subprocess.check_output(["python3", "-B", LOCK, "status", "--json"], text=True))
meta = status.get("metadata") or {}
if status.get("status") != "held" or meta.get("owner") != OWNER or meta.get("task_id") != TASK:
    sys.exit("RACK-V2-11 machine-wide Unity lock (owner %s, task %s) required" % (OWNER, TASK))
if meta.get("head") != head or Path(meta.get("lane", "")).resolve() != ROOT:
    sys.exit("lock must name this exact candidate and lane")
lanes = json.loads(LANES.read_text())["lanes"]
if not any(Path(l["path"]).resolve() == ROOT and l.get("unity") for l in lanes):
    sys.exit("this checkout is not a registered Unity lane in " + str(LANES))
if git("status", "--porcelain", "--untracked-files=all").strip():
    sys.exit("commit/clean candidate before Unity qualification")
out = Path(args.out).resolve()
if str(out).startswith("/media/jer/ZX20"):
    sys.exit("durable evidence belongs under /media/jer/Files/CCF/Verification, not ZX20")
if out.exists() and any(out.iterdir()):
    sys.exit("choose a fresh evidence directory")
evidence = out / "evidence"
evidence.mkdir(parents=True, exist_ok=True)
(out / "lock.json").write_text(json.dumps(status, indent=2))
(out / "candidate.txt").write_text(head + "\n")
protected_before = {str(p): sha(p) for p in PROTECTED}
(out / "protected-before.json").write_text(json.dumps(protected_before, indent=2))

found = [c for c in LICENCES if (c / "UnityEntitlementLicense.xml").is_file()]
if not found:
    sys.exit("no Unity licence file found")
newest = max(found, key=lambda c: (c / "UnityEntitlementLicense.xml").stat().st_mtime)
config = out / "config"
shutil.copytree(newest, config / "unity3d/Unity/licenses", dirs_exist_ok=True)
slot = config / "unity3d/DefaultCompany/CCF"

env = dict(os.environ, XDG_CONFIG_HOME=str(config), CCF_DIAG_DIR=str(evidence), CCF_ACCEPTANCE_OUTPUT=str(evidence),
           CCF_TEST_DIRECT_ENTRY="1", PYTHONDONTWRITEBYTECODE="1", DISPLAY=args.display)
for name in ("TMPDIR", "TMP", "TEMP", "XDG_CACHE_HOME", "CCF_STAND_GEOMETRY"):
    env.pop(name, None)  # never leave scratch on the exFAT SSD; never inherit a geometry override
env.update(gate["env"])
if args.gate == "coldstart":
    seed = Path(args.seed_from or "").resolve()
    if not (seed / "evidence/rackv2-11-partial-save.json").is_file() or "RACKV2PERSIST_VERIFY_PASS mode=v2" not in (seed / "v2.log").read_text(errors="replace"):
        sys.exit("--seed-from must be a passed v2 run directory")
    slot.mkdir(parents=True, exist_ok=True)
    shutil.copy2(seed / "evidence/rackv2-11-partial-save.json", slot / "forest-save.json")
    shutil.copy2(seed / "evidence/rackv2-11-partial-expected.json", evidence / "seeded-expected.json")
    env["CCF_RACKV2_11_EXPECTED"] = str(evidence / "seeded-expected.json")
    (out / "seed.txt").write_text("%s sha256=%s\n" % (seed, sha(slot / "forest-save.json")))
elif args.seed_from:
    sys.exit("--seed-from is coldstart only")

dest = ROOT / "Assets/ForestPrototype" / gate["file"].name
staged = [dest, Path(str(dest) + ".meta")]
if any(p.exists() for p in staged):
    sys.exit("staged harness or meta already present, investigate first")

procs = []
xephyr = subprocess.Popen(["Xephyr", args.display, "-screen", args.screen, "-ac", "-noreset"], stdout=open(out / "xephyr.log", "w"), stderr=subprocess.STDOUT)
procs.append(xephyr)
time.sleep(2)
procs.append(subprocess.Popen(["openbox"], env=dict(os.environ, DISPLAY=args.display), stdout=open(out / "openbox.log", "w"), stderr=subprocess.STDOUT))
time.sleep(1)

log = out / (args.gate + ".log")
code, reaped, unity, focuser = None, False, None, None
started = time.monotonic()
try:
    shutil.copy2(gate["file"], dest)
    unity = subprocess.Popen([UNITY, "-projectPath", str(ROOT), "-logFile", str(log), "-executeMethod", gate["method"]], cwd=ROOT, env=env)
    (out / "unity.pid").write_text(str(unity.pid))
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
        if time.monotonic() - started > args.timeout:
            unity.kill()
            code = -1
            print("TIMEOUT")
            break
        text = log.read_text(errors="replace") if log.exists() else ""
        if finished_at is None and (gate["pass_marker"] in text or gate["fail_marker"] in text):
            finished_at = time.monotonic()
        if finished_at is not None and time.monotonic() - finished_at > args.grace and "Cleanup mono" in text:
            (out / "reaper.txt").write_text("reaping this run's own hung Editor after verdict and Cleanup mono; pid %d\n" % unity.pid)
            unity.kill()
            unity.wait(timeout=30)
            code, reaped = -9, True
            break
        time.sleep(2)
finally:
    if focuser and focuser.poll() is None:
        focuser.terminate()
    if unity is not None and unity.poll() is None:
        unity.kill()
    for p in reversed(procs):
        if p.poll() is None:
            p.terminate()
    for path in staged:
        if path.exists():
            path.unlink()
    shutil.rmtree(config / "unity3d/Unity/licenses", ignore_errors=True)

# Editor side effects: capture, then restore only the known rewrites from HEAD.
porcelain = git("status", "--porcelain", "--untracked-files=all")
(out / "git-status-after.txt").write_text(porcelain)
changed = [line[3:] for line in porcelain.splitlines() if line.strip()]
unexpected = [p for p in changed if p not in KNOWN_SIDE_EFFECTS]
if not unexpected and changed:
    (out / "editor-side-effects-restored.patch").write_text(git("diff", "--binary"))
    subprocess.check_call(["git", "checkout", "--", *changed], cwd=ROOT)
clean_after = not git("status", "--porcelain", "--untracked-files=all").strip()

protected_after = {str(p): sha(p) for p in PROTECTED}
(out / "protected-after.json").write_text(json.dumps(protected_after, indent=2))
protected_ok = protected_after == protected_before
slot_files = sorted(p.name for p in slot.glob("forest-save*")) if slot.exists() else []
(out / "slot-after.txt").write_text("\n".join(slot_files) + "\n")

text = log.read_text(errors="replace") if log.exists() else ""
keys = [l for l in text.splitlines() if any(k in l for k in ("_PASS", "_FAIL", "RACKV2PERSIST_", "RACKV2SAVE_CHECK", "error CS", "No valid Unity", "Exception")) and "VisualScripting" not in l]
(out / "summary-lines.txt").write_text("\n".join(keys) + "\n")
passed = gate["pass_marker"] in text and gate["fail_marker"] not in text and "error CS" not in text
exit_ok = code == 0 or (reaped and passed)
leftover = [str(p) for p in staged if p.exists()]
print("gate=%s exit=%s reaped=%s seconds=%.0f pass-marker=%s protected-unchanged=%s clean-after=%s unexpected=%s leftover=%s slot=%s log=%s" % (
    args.gate, code, reaped, time.monotonic() - started, passed, protected_ok, clean_after, unexpected, leftover, slot_files, log))
for l in keys[-12:]:
    print("  " + l[:300])
sys.exit(0 if exit_ok and passed and protected_ok and clean_after and not leftover else 1)
