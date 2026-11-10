from pathlib import Path
import json
import sys
import time

evidence = Path(__file__).parent / 'run/evidence'
command = json.loads(sys.argv[1])
path = evidence / 'commands' / (str(time.time_ns()) + '.json')
temporary = path.with_suffix('.tmp')
temporary.write_text(json.dumps(command))
temporary.rename(path)
deadline = time.monotonic() + 45
while time.monotonic() < deadline:
    for result in ['.done', '.failed']:
        if Path(str(path) + result).exists():
            print(path.name, result, command)
            if result == '.failed':
                print(Path(str(path) + '.error').read_text())
                sys.exit(1)
            sys.exit(0)
    time.sleep(0.2)
raise RuntimeError('command not acknowledged within finite wait: ' + str(path))
