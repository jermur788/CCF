#!/usr/bin/env python3
"""Launch the staged StandalonePlayerSmoke in a built Linux Player (SAVE then LOAD).

Reuses Tools/Verification/S1B/StandalonePlayerSmoke.cs; adds no gameplay harness and no
FPS benchmark. Isolates persistent data with a ZX20-backed XDG_CONFIG_HOME so the two
launches share one save but do not touch a real player's data. Records wall-clock,
timeout status, process exit and a bounded fatal/exception scan.

Graphical: a valid DISPLAY is required (a Linux Player is not assumed headless).

Evidence defaults to:
  /media/jer/ZX20/CCF-Verification/ScenarioOneLinuxPlayerSmoke-<timestamp>/
"""
import argparse
import datetime
import json
import os
import re
import subprocess
import sys
import time
from pathlib import Path

DEFAULT_EVIDENCE_ROOT = "/media/jer/ZX20/CCF-Verification"
LINUX_BINARY = "CCF.x86_64"
SAVE_MARKER = "S1B_PLAYER_SAVE_PHASE_PASS"
LOAD_MARKER = "S1B_PLAYER_LOAD_RESET_PHASE_PASS"
FATAL = re.compile(r"S1B_SMOKE_FAIL|Unhandled exception|NullReferenceException|error CS")


def smoke_output_dir(phase_dir):
    """One shared directory for BOTH phases.

    StandalonePlayerSmoke.cs writes `saved-world-hash.txt` into `--ccf-smoke-output`
    during SAVE and reads that same location during LOAD, so the two launches must
    receive the identical path. `phase_dir` itself stays freshly created per run so
    stale evidence cannot carry over.
    """
    return phase_dir / "smoke"


def phase_command(player, smoke_out, log, phase, extra_env):
    flag = "--ccf-smoke-save" if phase == "save" else "--ccf-smoke-load"
    cmd = [str(player), "--ccf-smoke-output", str(smoke_out), flag, "-logFile", str(log)]
    env = dict(os.environ, **extra_env)
    return cmd, env


def run_phase(player, phase_dir, phase, extra_env, timeout):
    log = phase_dir / f"player-{phase}.log"
    cmd, env = phase_command(player, smoke_output_dir(phase_dir), log, phase, extra_env)
    start = time.monotonic()
    try:
        code = subprocess.run(cmd, env=env, timeout=timeout).returncode
        timed_out = False
    except subprocess.TimeoutExpired:
        code, timed_out = 124, True
    text = log.read_text(errors="replace") if log.exists() else ""
    marker = SAVE_MARKER if phase == "save" else LOAD_MARKER
    fatals = sorted(set(FATAL.findall(text)))
    return {"phase": phase, "exitCode": code, "timedOut": timed_out,
            "seconds": round(time.monotonic() - start, 1), "marker": marker,
            "markerFound": marker in text, "fatalMatches": fatals, "log": str(log)}


def main():
    p = argparse.ArgumentParser(description=__doc__)
    p.add_argument("--player", required=True, help="path to the built Linux Player binary (CCF.x86_64)")
    p.add_argument("--evidence-root", default=DEFAULT_EVIDENCE_ROOT)
    p.add_argument("--label", default=None)
    p.add_argument("--display", default=os.environ.get("DISPLAY", ":0"))
    p.add_argument("--timeout", type=int, default=600)
    p.add_argument("--phase", choices=["both", "save", "load"], default="both")
    a = p.parse_args()

    player = Path(a.player).resolve()
    if not player.is_file():
        raise SystemExit("STOP: player binary not found: " + str(player))
    if not os.access(player, os.X_OK):
        raise SystemExit("STOP: player binary not executable: " + str(player))
    if not a.display:
        raise SystemExit("STOP: a DISPLAY is required (Linux Player is not assumed headless)")

    label = a.label or datetime.datetime.now(datetime.timezone.utc).strftime("ScenarioOneLinuxPlayerSmoke-%Y%m%dT%H%M%SZ")
    phase_dir = Path(a.evidence_root) / label
    phase_dir.mkdir(parents=True, exist_ok=False)
    xdg = phase_dir / "config"
    smoke_out = smoke_output_dir(phase_dir)
    smoke_out.mkdir(parents=True, exist_ok=True)
    extra_env = {"DISPLAY": a.display, "XDG_CONFIG_HOME": str(xdg)}
    # save and load share one persistentDataPath under the isolated config
    (phase_dir / "environment.json").write_text(json.dumps(
        {"player": str(player), "display": a.display, "xdg_config_home": str(xdg),
         "smoke_output": str(smoke_out), "timeout": a.timeout, "label": label}, indent=2) + "\n")

    phases = ["save", "load"] if a.phase == "both" else [a.phase]
    results = [run_phase(player, phase_dir, ph, extra_env, a.timeout) for ph in phases]
    overall = all(r["exitCode"] == 0 and r["markerFound"] and not r["fatalMatches"] for r in results)
    summary = {"label": label, "player": str(player), "phases": results, "overall": "PASS" if overall else "FAIL",
               "category": "Linux Player build-and-smoke (bounded); not a benchmark or native-Windows claim"}
    (phase_dir / "smoke-summary.json").write_text(json.dumps(summary, indent=2) + "\n")
    print(json.dumps({"label": label, "overall": summary["overall"],
                      "phases": [(r["phase"], r["exitCode"], r["markerFound"], r["timedOut"]) for r in results]}, indent=2))
    return 0 if overall else 1


if __name__ == "__main__":
    sys.exit(main())
