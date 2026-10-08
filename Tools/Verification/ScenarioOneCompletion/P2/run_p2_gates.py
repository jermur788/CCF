#!/usr/bin/env python3
"""P2 Unity gates: CropTreeCompetitorVerification batch x2 + interactive x1.

One Editor at a time: refuses to start if any Unity Editor process is running.
Stages the disposable harness into Assets/ForestPrototype and always removes it
(and its .meta). Isolated Unity config under Build/P2/config (licence copied
from ~/.config/unity3d/Unity/licenses). Results: Build/P2/results.json, logs and
captures under Build/P2/. Exit 0 only if every requested run passes and the two
batch determinism hashes match.

Usage: run_p2_gates.py [--batch-only | --interactive-only]
"""
import argparse
import fcntl
import json
import os
import re
import shutil
import subprocess
import sys
import time
from pathlib import Path

ROOT = Path(__file__).resolve().parents[4]
UNITY = "/media/jer/ZX20/Unity/6000.6.0f1/Editor/Unity"
GATE = "CropTreeCompetitorVerification"
SOURCE = Path(__file__).with_name(GATE + ".cs")
TARGET = ROOT / "Assets" / "ForestPrototype" / (GATE + ".cs")
OUT = ROOT / "Build" / "P2"


def other_unity_running():
    found = subprocess.run(["pgrep", "-f", "Editor/Unity "], capture_output=True, text=True).stdout.split()
    return [pid for pid in found if pid != str(os.getpid())]


def prepare_config():
    config = OUT / "config"
    licences = config / "unity3d" / "Unity" / "licenses"
    if not licences.exists():
        source = Path.home() / ".config" / "unity3d" / "Unity" / "licenses"
        shutil.copytree(source, licences)
    return config


def run_once(name, interactive, config):
    log = OUT / (name + ".log")
    evidence = OUT / "evidence"
    evidence.mkdir(parents=True, exist_ok=True)
    unit = "ccf-p2-" + str(time.time_ns())
    cmd = ["systemd-run", "--user", "--scope", "--quiet", "--unit=" + unit, "-p", "MemoryMax=8G", "-p", "MemorySwapMax=256M",
           UNITY, "-projectPath", str(ROOT), "-job-worker-count", "2", "-executeMethod", GATE + ".Begin", "-logFile", str(log)]
    if not interactive:
        cmd += ["-batchmode", "-nographics"]
    env = dict(os.environ, XDG_CONFIG_HOME=str(config), CCF_ACCEPTANCE_OUTPUT=str(evidence),
               DISPLAY=os.environ.get("DISPLAY", ":0"))
    start = time.monotonic()
    try:
        code = subprocess.run(cmd, cwd=ROOT, env=env, timeout=3600).returncode
    except subprocess.TimeoutExpired:
        subprocess.run(["systemctl", "--user", "stop", unit + ".scope"], check=False)
        code = 124
    text = log.read_text(errors="replace") if log.exists() else ""
    markers = [line for line in text.splitlines() if re.search(r"P2_[A-Z_]+|error CS|_FAIL", line)]
    hashes = re.findall(r"P2_DETERMINISM_HASH ([0-9A-F]{16})", text)
    passed = code == 0 and "P2_COMPETITOR_VERIFY_PASS" in text and not any("_FAIL" in m or "error CS" in m for m in markers)
    if interactive:
        passed = passed and text.count("P2_RENDERED_PASS") == 3
    return dict(run=name, mode="interactive" if interactive else "batch", status="PASS" if passed else "FAIL",
                exit_code=code, seconds=round(time.monotonic() - start, 1), hash=hashes[0] if hashes else None, markers=markers)


def main():
    parser = argparse.ArgumentParser()
    group = parser.add_mutually_exclusive_group()
    group.add_argument("--batch-only", action="store_true")
    group.add_argument("--interactive-only", action="store_true")
    args = parser.parse_args()

    OUT.mkdir(parents=True, exist_ok=True)
    lock = (OUT / "launch.lock").open("w")
    fcntl.flock(lock, fcntl.LOCK_EX | fcntl.LOCK_NB)
    busy = other_unity_running()
    if busy:
        sys.exit(f"STOP: another Unity Editor is running (pids {busy}); one heavy Unity session at a time.")
    if TARGET.exists():
        sys.exit(f"STOP: staged source already present: {TARGET}")
    config = prepare_config()
    head = subprocess.check_output(["git", "rev-parse", "HEAD"], cwd=ROOT).decode().strip()

    runs = []
    if not args.interactive_only:
        runs += [("batch-1", False), ("batch-2", False)]
    if not args.batch_only:
        runs += [("interactive", True)]

    results = []
    shutil.copy2(SOURCE, TARGET)
    try:
        for name, interactive in runs:
            result = run_once(name, interactive, config)
            results.append(result)
            print(json.dumps({k: result[k] for k in ("run", "status", "seconds", "exit_code", "hash")}), flush=True)
    finally:
        TARGET.unlink(missing_ok=True)
        Path(str(TARGET) + ".meta").unlink(missing_ok=True)

    batch_hashes = [r["hash"] for r in results if r["mode"] == "batch"]
    deterministic = len(batch_hashes) < 2 or (batch_hashes[0] is not None and len(set(batch_hashes)) == 1)
    summary = dict(head=head, results=results, batch_hashes=batch_hashes, deterministic=deterministic,
                   status="PASS" if deterministic and all(r["status"] == "PASS" for r in results) else "FAIL")
    (OUT / "results.json").write_text(json.dumps(summary, indent=2) + "\n")
    print("P2_GATES_" + summary["status"], "deterministic=" + str(deterministic))
    status = subprocess.check_output(["git", "status", "--short"], cwd=ROOT).decode()
    if status.strip():
        print("NOTE git status after run (revert Editor normalisation if unrelated):\n" + status)
    return 0 if summary["status"] == "PASS" else 1


if __name__ == "__main__":
    sys.exit(main())
