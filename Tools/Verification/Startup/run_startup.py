#!/usr/bin/env python3
"""Run the disposable startup-screen gate, always removing the staged source and its meta.

Uses this worktree's own Library, an isolated Unity config/licence directory (so the Editor's persistent data
path, and therefore forest-save.json, is NOT the real player's), and refuses to start while another Unity
Editor is running. Only one Unity-heavy process may run at a time on this machine.

Modes:
  batch      -batchmode -nographics. Content, isolation, Continue/Start New/Controls/Quit routes, save-loader
             parity. Key presses and rendered checks are reported STARTUP_SKIPPED (they need an interactive Editor).
  bootstrap  batch with CCF_STARTUP_SCREEN=1: the real Bootstrap must open the title in the Editor by itself.
  rendered   interactive Editor (needs DISPLAY): virtual key presses, then layout checks and screenshots at
             1280x720 and 1920x1080 into Build/StartupScreen/<label>-captures.

--stand-geometry 0 runs the same gate under the explicit Editor-only Legacy40 override (historical path).
"""
import argparse, json, os, shutil, subprocess, time
from pathlib import Path

ROOT = Path(__file__).resolve().parents[3]
GATE = 'StartupScreenVerification'
DEFAULT_UNITY = '/media/jer/ZX20/Unity/6000.6.0f1/Editor/Unity'

p = argparse.ArgumentParser()
p.add_argument('--mode', choices=['batch', 'bootstrap', 'rendered'], required=True)
p.add_argument('--label', required=True)
p.add_argument('--unity', default=os.environ.get('UNITY', DEFAULT_UNITY))
p.add_argument('--stand-geometry', choices=['0', '1'], help='set the Editor-only CCF_STAND_GEOMETRY override')
args = p.parse_args()

busy = subprocess.run(['pgrep', '-f', '^' + args.unity + ' '], capture_output=True, text=True).stdout.split()
if busy:
    raise SystemExit('Another Unity Editor is running: ' + str(busy))

OUT = ROOT / 'Build/StartupScreen'
OUT.mkdir(parents=True, exist_ok=True)
config = OUT / 'config'
licenses = Path.home() / '.config/unity3d/Unity/licenses'
if licenses.exists():
    shutil.copytree(licenses, config / 'unity3d/Unity/licenses', dirs_exist_ok=True)

target = ROOT / 'Assets/ForestPrototype' / (GATE + '.cs')
assert not target.exists(), 'a staged copy already exists: ' + str(target)
log = OUT / (args.label + '.log')

cmd = [args.unity, '-projectPath', str(ROOT), '-job-worker-count', '2', '-executeMethod', GATE + '.Begin', '-logFile', str(log)]
env = dict(os.environ, XDG_CONFIG_HOME=str(config))
env.pop('CCF_STAND_GEOMETRY', None)
env.pop('CCF_STARTUP_SCREEN', None)
if args.mode in ('batch', 'bootstrap'):
    cmd += ['-batchmode', '-nographics']
if args.mode == 'bootstrap':
    env['CCF_STARTUP_SCREEN'] = '1'
if args.mode == 'rendered':
    env['DISPLAY'] = os.environ.get('DISPLAY', ':0')
    env['CCF_ACCEPTANCE_OUTPUT'] = str(OUT / (args.label + '-captures'))
if args.stand_geometry is not None:
    env['CCF_STAND_GEOMETRY'] = args.stand_geometry

start = time.monotonic()
shutil.copy2(Path(__file__).with_name(GATE + '.cs'), target)
try:
    try:
        code = subprocess.run(cmd, cwd=ROOT, env=env, timeout=1800).returncode
    except subprocess.TimeoutExpired:
        code = 124
finally:
    target.unlink(missing_ok=True)
    Path(str(target) + '.meta').unlink(missing_ok=True)

text = log.read_text(errors='replace') if log.exists() else ''
markers = [l for l in text.splitlines() if any(x in l for x in ('STARTUP_', 'error CS'))]
failed = any('STARTUP_SCREEN_VERIFY_FAIL' in l or 'error CS' in l for l in markers)
status = 'PASS' if code == 0 and 'STARTUP_SCREEN_VERIFY_PASS' in text and not failed else 'FAIL'
skipped = [l for l in markers if 'STARTUP_SKIPPED' in l]
result = dict(gate=GATE, mode=args.mode, stand_geometry=args.stand_geometry, status=status, exit_code=code,
              seconds=round(time.monotonic() - start, 2), skipped=skipped, markers=markers)
(OUT / (args.label + '.json')).write_text(json.dumps(result, indent=2) + '\n')
print(json.dumps(result), flush=True)
raise SystemExit(0 if status == 'PASS' else 1)
