import json
import os
import subprocess
import sys
import time
from pathlib import Path

folder = Path(sys.argv[1])
display = sys.argv[2]
if display in (':0', ':1'):
    raise SystemExit('refuse the normal desktop')
environment = dict(os.environ, DISPLAY=display)
handled = set()
with (folder / 'native-actions.jsonl').open('a', buffering=1) as log:
    while not (folder / 'finished').exists():
        for path in sorted(folder.glob('request-*.json')):
            sequence = int(path.stem.split('-')[1])
            if sequence in handled:
                continue
            try:
                subprocess.run(['import', '-display', display, '-window', 'root', str(folder / f'desktop-before-{sequence:04d}.png')], check=True, timeout=20)
                for operation in json.loads(path.read_text())['ops']:
                    kind = operation['op']
                    if kind == 'burst':
                        command = ['xdotool', 'click', '--repeat', '2', '--delay', '100', '1']
                    else:
                        command = ['xdotool', *{
                            'key': ['key', operation.get('key', '')],
                            'click': ['click', str(operation.get('button', 1))],
                            'absolute': ['mousemove', '--sync', str(operation.get('x', 0)), str(operation.get('y', 0))],
                            'relative': ['mousemove_relative', '--sync', '--', str(operation.get('x', 0)), str(operation.get('y', 0))]
                        }[kind]]
                    started = time.time()
                    subprocess.run(command, env=environment, check=True, timeout=20)
                    log.write(json.dumps({'seq': sequence, 'op': operation, 'started': started, 'finished': time.time()}) + '\n')
                    time.sleep(.4)
                subprocess.run(['import', '-display', display, '-window', 'root', str(folder / f'desktop-after-{sequence:04d}.png')], check=True, timeout=20)
                (folder / f'done-{sequence:04d}').write_text('done')
                handled.add(sequence)
            except Exception as error:
                (folder / 'driver-error').write_text(repr(error))
                raise
        time.sleep(.04)
