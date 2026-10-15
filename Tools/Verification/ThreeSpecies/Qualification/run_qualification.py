#!/usr/bin/env python3
"""Stage disposable qualification file(s), run ONE Unity method, always clean up, record the result.

A small generic runner for the final-density qualification. The shipped run_unity_gates.py stages exactly one file
and has no way to add a second (the per-year probe) or to read a marker other than its own, and its default config
folder carries no licence. This runner:
  - refuses to start if an Editor on THIS project is already running (anchored pgrep, so it never matches itself);
  - copies the licence into an isolated XDG_CONFIG_HOME under --out, so no real EditorPrefs or save is touched;
  - stages each --file under Assets/ForestPrototype/ and ALWAYS removes it and its .meta afterwards;
  - runs   <unity> -projectPath <repo> -executeMethod <method> -logFile <log>   inside a memory-capped systemd scope;
  - compares `git status` before and after and fails on anything new except the known Editor rewrite of
    .vscode/settings.json (recorded, not failed);
  - writes results-<label>.json and <label>.markers.txt (the log lines that matter, without Unity's noise).

PASS needs: exit 0, the --marker in the log, no `error CS` line and no unexpected working-tree change.
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

ROOT = Path(__file__).resolve().parents[4]
UNITY = "/media/jer/ZX20/Unity/6000.6.0f1/Editor/Unity"
STAGE = ROOT / "Assets" / "ForestPrototype"
LICENCES = Path.home() / ".config/unity3d/Unity/licenses"
# Unity re-serialises these on every Editor run, independent of the code under test: the settings file gets its
# solution name, and seven to eleven SectionFive materials get float round-trip noise in _Color (0.045 -> 0.044999972).
# They are recorded in the result, never counted as an unexpected change.
def is_known_rewrite(line):
    if line == " M .vscode/settings.json":
        return True
    return line.startswith(" M Assets/ForestPrototype/Art/SectionFive/") and line.endswith(".mat")
KEEP = ("QUAL_", "DENSITY_QUAL_", "E80_MEASUREMENT", "SCENARIO_ONE_COMPLETION", "SCENARIO_HISTORY", "BROADLEAF", "MIXED_DENSITY_",
        "UX_QUAL", "error CS", "_PASS", "_FAIL", "_HASH")


def git_state():
    out = subprocess.check_output(["git", "status", "--porcelain", "--untracked-files=all"], cwd=ROOT).decode()
    return set(out.splitlines())


def main():
    p = argparse.ArgumentParser()
    p.add_argument("--label", required=True)
    p.add_argument("--out", required=True)
    p.add_argument("--method", required=True, help="Class.Begin")
    p.add_argument("--file", action="append", default=[], help="source to stage (repeatable)")
    p.add_argument("--marker", required=True, help="terminal pass marker that must appear in the log")
    p.add_argument("--interactive", action="store_true", help="graphical Editor (no -batchmode/-nographics)")
    p.add_argument("--timeout", type=int, default=3600)
    p.add_argument("--env", action="append", default=[], help="KEY=VALUE passed to the Editor")
    p.add_argument("--window", default=None, help="WIDTHxHEIGHT for -screen-width/-screen-height (interactive only)")
    a = p.parse_args()

    out = Path(a.out)
    (out / "evidence").mkdir(parents=True, exist_ok=True)
    config = out / "config"
    if not (config / "unity3d/Unity/licenses").exists():
        shutil.copytree(LICENCES, config / "unity3d/Unity/licenses")

    # One Editor per PROJECT (they would fight over this Library and Temp); an Editor on another worktree may run beside it.
    busy = subprocess.run(["pgrep", "-f", "^" + UNITY + " .*-projectPath " + str(ROOT) + "( |$)"], capture_output=True, text=True).stdout.split()
    if busy:
        raise SystemExit("Another Unity Editor is running: " + str(busy))

    staged = []
    for f in a.file:
        src = Path(f) if os.path.isabs(f) else ROOT / f
        target = STAGE / src.name
        if target.exists() or Path(str(target) + ".meta").exists():
            raise SystemExit("Refusing to start: staged copy or .meta already exists: " + str(target))
        staged.append((src, target))

    log = out / (a.label + ".log")
    unit = "ccf-fdq-%s-%d" % (a.label.replace("/", "-"), time.time_ns())
    cmd = ["systemd-run", "--user", "--scope", "--quiet", "--unit=" + unit, "-p", "MemoryMax=8G", "-p", "MemorySwapMax=256M",
           UNITY, "-projectPath", str(ROOT), "-job-worker-count", "2", "-executeMethod", a.method, "-logFile", str(log)]
    if a.interactive:
        if a.window:
            w, h = a.window.split("x")
            cmd += ["-screen-width", w, "-screen-height", h, "-screen-fullscreen", "0"]
    else:
        cmd += ["-batchmode", "-nographics"]
    env = dict(os.environ, XDG_CONFIG_HOME=str(config), CCF_DIAG_DIR=str(out / "evidence"), DISPLAY=os.environ.get("DISPLAY", ":0"))
    for item in a.env:
        k, v = item.split("=", 1)
        env[k] = v

    before = git_state()
    start = time.monotonic()
    code = None
    digests = {}
    try:
        for src, target in staged:
            data = src.read_bytes()
            digests[src.name] = hashlib.sha256(data).hexdigest()
            target.write_bytes(data)
        try:
            code = subprocess.run(cmd, cwd=ROOT, env=env, timeout=a.timeout).returncode
        except subprocess.TimeoutExpired:
            subprocess.run(["systemctl", "--user", "stop", unit + ".scope"], check=False)
            code = 124
    finally:
        for _, target in staged:
            target.unlink(missing_ok=True)
            Path(str(target) + ".meta").unlink(missing_ok=True)

    text = log.read_text(errors="replace") if log.exists() else ""
    lines = [l for l in text.splitlines() if any(k in l for k in KEEP)]
    (out / (a.label + ".markers.txt")).write_text("\n".join(lines) + "\n")
    after = git_state()
    new = sorted(after - before)
    rewrites = sorted(x for x in new if is_known_rewrite(x))
    unexpected = [x for x in new if not is_known_rewrite(x)]
    compile_errors = [l for l in text.splitlines() if "error CS" in l]
    passed = code == 0 and a.marker in text and not compile_errors and not unexpected
    result = {
        "label": a.label, "status": "PASS" if passed else "FAIL", "exit_code": code, "seconds": round(time.monotonic() - start, 1),
        "method": a.method, "marker": a.marker, "marker_found": a.marker in text, "env": a.env, "interactive": a.interactive,
        "run_head": subprocess.check_output(["git", "rev-parse", "HEAD"], cwd=ROOT).decode().strip(),
        "staged_sha256": digests, "compile_errors": compile_errors[:5], "unexpected_tree_changes": unexpected,
        "known_editor_rewrites": rewrites,
    }
    (out / ("results-" + a.label + ".json")).write_text(json.dumps(result, indent=2) + "\n")
    print(json.dumps({k: result[k] for k in ("label", "status", "exit_code", "seconds")}), flush=True)
    sys.exit(0 if passed else 1)


if __name__ == "__main__":
    main()
