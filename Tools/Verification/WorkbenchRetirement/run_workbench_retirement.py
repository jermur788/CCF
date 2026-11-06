#!/usr/bin/env python3
"""Run one disposable Unity gate once (batch mode, one Editor) for #287, then remove it and its .meta.

Usage: run_workbench_retirement.py <gate-name> <gate-source.cs> <out-dir> [--env K=V ...]
Uses an isolated XDG_CONFIG_HOME under <out-dir>/config (so persistentDataPath is isolated and the user's save and
prefs are never touched). The Unity licence is copied in, then deleted again after the run; only a sha prefix is recorded.
Never sources CCF-Ops/environment.sh (that moves TMPDIR onto exFAT). The caller must hold the Unity lease.
"""
import hashlib, json, os, shutil, subprocess, sys, time
from pathlib import Path

ROOT = Path(__file__).resolve().parents[3]
UNITY = "/media/jer/ZX20/Unity/6000.6.0f1/Editor/Unity"

def main():
    gate, source, out = sys.argv[1], Path(sys.argv[2]), Path(sys.argv[3])
    rest = sys.argv[4:]
    env_extra = dict(a.split("=", 1) for a in rest[1:]) if rest[:1] == ["--env"] else {}
    target = ROOT / "Assets" / "ForestPrototype" / source.name
    if subprocess.run(["pgrep", "-f", "6000.6.0f1/Editor/[U]nity"], capture_output=True).returncode == 0:
        raise SystemExit("A Unity Editor is already running")
    if target.exists():
        raise SystemExit("Existing staged harness: " + str(target))
    out.mkdir(parents=True, exist_ok=False)
    config = out / "config"
    lic = config / "unity3d" / "Unity" / "licenses"
    lic.parent.mkdir(parents=True)
    shutil.copytree(Path.home() / ".config/unity3d/Unity/licenses", lic)
    log = out / (gate + ".log")
    env = dict(os.environ, XDG_CONFIG_HOME=str(config), DISPLAY=os.environ.get("DISPLAY", ":0"), **env_extra)
    for k in ("CCF_STAND_GEOMETRY", "TMPDIR", "TMP", "TEMP", "XDG_CACHE_HOME"):
        if k not in env_extra:
            env.pop(k, None)
    cmd = [UNITY, "-projectPath", str(ROOT), "-job-worker-count", "2", "-executeMethod", gate + ".Begin",
           "-batchmode", "-nographics", "-logFile", str(log)]
    start = time.monotonic()
    shutil.copy2(source, target)
    try:
        try:
            code = subprocess.run(cmd, cwd=ROOT, env=env, timeout=2700).returncode
        except subprocess.TimeoutExpired:
            code = 124
    finally:
        target.unlink(missing_ok=True)
        Path(str(target) + ".meta").unlink(missing_ok=True)
        sha = ""
        for f in sorted(lic.glob("*")):
            sha += hashlib.sha256(f.read_bytes()).hexdigest()[:12] + " "
        shutil.rmtree(config / "unity3d" / "Unity" / "licenses", ignore_errors=True)
    text = log.read_text(errors="replace") if log.exists() else ""
    lines = [l for l in text.splitlines() if any(x in l for x in ("_PASS", "_FAIL", "WORKBENCH_", "error CS", "HASH", "ENLARGED80", "MFM08A"))]
    (out / (gate + ".markers.txt")).write_text("\n".join(lines) + "\n")
    (out / (gate + ".json")).write_text(json.dumps(dict(gate=gate, exit_code=code, seconds=round(time.monotonic() - start, 1), licence_sha_prefixes=sha.strip()), indent=2) + "\n")
    print(json.dumps(dict(gate=gate, exit_code=code, seconds=round(time.monotonic() - start, 1))), flush=True)
    for l in lines[-6:]:
        print("   ", l[:240])
    return code

if __name__ == "__main__":
    sys.exit(main())
