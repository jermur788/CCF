#!/usr/bin/env python3
"""Run acceptance/regrression tools in their own config, preserving player saves.

Evidence stays outside Git by default. A requested Tools/Verification harness is
copied into Assets only during the process, then removed with its generated meta.
No production scene is saved by this launcher. Read the requested harness first.
"""
import argparse
import json
import os
from pathlib import Path
import shutil
import subprocess
import sys
import time
os.environ.setdefault('CCF_STAND_GEOMETRY', '0')  # historical anchors run in the explicit Editor-only Legacy40 override (D-056); export CCF_STAND_GEOMETRY=1 to run them in Enlarged80 deliberately

ROOT = Path(__file__).resolve().parents[2]


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('gate', help='Class name, or Import for compile/import only')
    parser.add_argument('--method', default='Begin')
    parser.add_argument('--graphics', action='store_true')
    parser.add_argument('--interactive', action='store_true', help='Visible Editor Game view for genuine IMGUI capture')
    parser.add_argument('--detach', action='store_true', help='Keep launcher/cleanup alive across terminal tool timeouts')
    parser.add_argument('--tag', default='')
    parser.add_argument('--timeout', type=int, default=1800)
    parser.add_argument('--output', default='/tmp/opencode/ccf-final-acceptance')
    parser.add_argument('--unity', default='/media/jer/ZX20/Unity/6000.6.0f1/Editor/Unity')
    args = parser.parse_args()
    out = Path(args.output).resolve()
    out.mkdir(parents=True, exist_ok=True)
    if args.detach:
        wrapper = out / (args.gate + '-' + (args.tag or args.method) + '.launcher.log')
        with wrapper.open('w') as stream:
            proc = subprocess.Popen([sys.executable, str(Path(__file__).resolve())]
                + [arg for arg in sys.argv[1:] if arg != '--detach'], cwd=ROOT,
                stdout=stream, stderr=subprocess.STDOUT, start_new_session=True)
        print('DETACHED_LAUNCHER pid=' + str(proc.pid) + ' log=' + str(wrapper))
        return
    config = out / 'config'
    licenses = Path.home() / '.config/unity3d/Unity/licenses'
    if licenses.is_dir():
        shutil.copytree(licenses, config / 'unity3d/Unity/licenses', dirs_exist_ok=True)
    suffix = args.tag or args.method
    log = out / (args.gate + '-' + suffix + '.log')
    source = ROOT / 'Tools/Verification' / (args.gate + '.cs')
    target = ROOT / 'Assets/ForestPrototype' / source.name
    copied = False
    if source.exists():
        if target.exists():
            raise RuntimeError('Existing Assets harness must be investigated: ' + str(target))
        shutil.copy2(source, target)
        copied = True
    command = [args.unity, '-projectPath', str(ROOT), '-logFile', str(log)]
    if not args.interactive:
        command += ['-batchmode']
    if not args.graphics and not args.interactive:
        command += ['-nographics']
    if args.gate == 'Import':
        command += ['-quit']
    else:
        command += ['-executeMethod', args.gate + '.' + args.method]
    env = dict(os.environ, XDG_CONFIG_HOME=str(config), CCF_ACCEPTANCE_OUTPUT=str(out / 'evidence'), DISPLAY=os.environ.get('DISPLAY', ':0'))
    started = time.monotonic()
    try:
        proc = subprocess.run(command, cwd=ROOT, env=env, timeout=args.timeout)
        text = log.read_text(errors='replace') if log.exists() else ''
        lines = [line for line in text.splitlines() if any(key in line for key in
            ('_PASS', '_FAIL', '_HASH', 'error CS', 'No valid Unity Editor license', 'ASSET_REVIEW_', 'CANONICAL_LIFECYCLE'))]
        for line in lines:
            print(line)
        failed = proc.returncode != 0 or 'error CS' in text or 'No valid Unity Editor license' in text or '_VERIFY_FAIL' in text
        if args.gate != 'Import' and not any('_PASS' in line for line in lines):
            failed = True
        result = {'gate': args.gate, 'method': args.method, 'tag': suffix,
                  'exit_code': proc.returncode, 'status': 'FAIL' if failed else 'PASS',
                  'duration_seconds': round(time.monotonic()-started, 2),
                  'graphics': args.graphics, 'log': str(log), 'markers': lines}
        (out / (args.gate + '-' + suffix + '.result.json')).write_text(json.dumps(result, indent=2) + '\n')
        if failed:
            raise RuntimeError('Gate failed; inspect ' + str(log))
        print('ISOLATED_PROCESS_PASS ' + str(log))
    finally:
        if copied:
            target.unlink(missing_ok=True)
            Path(str(target) + '.meta').unlink(missing_ok=True)


if __name__ == '__main__':
    main()
