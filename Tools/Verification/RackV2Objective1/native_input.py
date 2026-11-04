#!/usr/bin/env python3
"""Deliver harness-requested OS events only on the named private Xephyr display; no game APIs.

Same request-file protocol and per-op pacing as RackV2NoCompetingAuthor/native_input.py (request-NNNN.json -> done-NNNN,
driver-error, stop on 'finished'), plus keydown / keyup / sleep so a harness can hold a key (walking with W).
Usage: native_input.py <driver-dir> <display>
"""
import json, subprocess, sys, time
from pathlib import Path

folder = Path(sys.argv[1]); display = sys.argv[2]
if display in (':0', ':1'):
    raise SystemExit('refuse the normal desktop')
env = {'DISPLAY': display, 'PATH': '/usr/bin:/bin'}
handled = set()
with (folder / 'native-actions.jsonl').open('a', buffering=1) as log:
    while not (folder / 'finished').exists():
        for path in sorted(folder.glob('request-*.json')):
            seq = int(path.stem.split('-')[1])
            if seq in handled:
                continue
            try:
                for op in json.loads(path.read_text())['ops']:
                    kind = op['op']
                    if kind == 'sleep':
                        time.sleep(float(op.get('seconds', 0)))
                    else:
                        args = {'key': ['key', op.get('key', '')], 'keydown': ['keydown', op.get('key', '')], 'keyup': ['keyup', op.get('key', '')],
                                'click': ['click', str(op.get('button', 1))],
                                'relative': ['mousemove_relative', '--sync', '--', str(op.get('x', 0)), str(op.get('y', 0))]}[kind]
                        subprocess.run(['xdotool', *args], env=env, check=True, timeout=20)
                    log.write(json.dumps({'seq': seq, 'op': op, 'time': time.time()}) + '\n')
                    if kind != 'sleep':
                        time.sleep(.4)
                (folder / f'done-{seq:04d}').write_text('done'); handled.add(seq)
            except Exception as error:
                (folder / 'driver-error').write_text(repr(error)); raise
        time.sleep(.04)
