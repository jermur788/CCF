import datetime
import os
import hashlib
import json
import shutil
import subprocess
import sys
from pathlib import Path

LANE = Path('/media/jer/ZX20/CCF-S1-ResearchCopy-V1-Qual')
ROOT = Path('/media/jer/Files/CCF/Verification/D073-FR10-427-20261112')
OUTPUT = ROOT / 'followup1'
LOCK = '/media/jer/ZX20/CCF-Ops/scripts/unity-lock'
# The lock owner must match between acquire and release; successor workers set CCF_427_OWNER to their own identity.
OWNER = os.environ.get('CCF_427_OWNER', 'OpenAI Codex gpt-6.1-sol HIGH #427')
TASK = 'CCF-427-D073-FR10'
BRANCH = 'qualify/scenario-one-d073-fr10-visible-provenance-unity-20261112-v1'
SOURCE = '339517ed2d49bb3d5716fc9cf029d9ef0eaeee72'


def git(*arguments):
    return subprocess.check_output(['git', *arguments], cwd=LANE, text=True)


def manifest(paths, base=Path('/')):
    return {path: hashlib.sha256((base / path).read_bytes()).hexdigest() if (base / path).is_file() else None for path in paths}


def write(name, value):
    destination = OUTPUT / name
    assert not destination.exists(), destination
    destination.write_text(json.dumps(value, indent=2) + '\n')


operation, tag = sys.argv[1:3]
state = json.loads(subprocess.check_output([LOCK, 'status', '--json'], text=True))
assert not state['unity_editor_pids'], state
host = json.loads((ROOT / 'host-before.json').read_text())
production = json.loads((ROOT / 'production-before.json').read_text())
if operation == 'start':
    assert state['status'] == 'free', state
    assert git('status', '--porcelain=v1', '-uall') == ''
    assert git('branch', '--show-current').strip() == BRANCH
    assert git('diff', SOURCE, '--', 'Assets', 'Packages', 'ProjectSettings') == ''
    assert manifest(host) == host and manifest(production, LANE) == production
    assert shutil.disk_usage(LANE).free > 15 * 2**30
    write(tag + '-host-before.json', manifest(host))
    write(tag + '-production-before.json', manifest(production, LANE))
    head = git('rev-parse', 'HEAD').strip()
    board = Path('/media/jer/ZX20/CCF-Ops/board/active.md')
    lines = board.read_text().splitlines(keepends=True)
    position = next(index for index, line in enumerate(lines) if line.startswith('| CCF-412-PREDATOR-FABLE-A-D073 |'))
    cells = lines[position].split('|')
    assert cells[7].strip() == 'ResearchCopyQual391'
    cells[8] = ' ' + BRANCH + ' '
    cells[9] = ' ' + head + ' '
    lines[position] = '|'.join(cells)
    board.write_text(''.join(lines))
    (OUTPUT / (tag + '-board.md')).write_text(''.join(lines))
    subprocess.run(['/media/jer/ZX20/CCF-Ops/scripts/packet-lint', '--board', str(board)], check=True)
    result = subprocess.run([LOCK, 'acquire', '--owner', OWNER, '--task-id', TASK, '--lane', str(LANE), '--branch', BRANCH, '--head', head,
                             '--eta', (datetime.datetime.now(datetime.timezone.utc) + datetime.timedelta(hours=2)).isoformat(),
                             '--notes', '#427 Follow-up1 bounded foldout diagnostics / distinct-ID fixture; ten-path restoration allowlist; isolated saves; no build; ' + os.environ.get('CCF_427_NOTE', '') + '', '--json'], capture_output=True, text=True)
    (OUTPUT / (tag + '-acquire.json')).write_text(result.stdout)
    assert result.returncode == 0, result.stderr
    print(result.stdout)
elif operation == 'finish':
    metadata = state['metadata']
    assert metadata['owner'] == OWNER and metadata['task_id'] == TASK
    host_after = manifest(host)
    write(tag + '-host-after.json', host_after)
    (OUTPUT / (tag + '-Editor-drift.patch')).write_text(git('diff', '--binary'))
    paths = git('diff', '--name-only').splitlines()
    allowed = set(json.loads((OUTPUT / 'restoration-allowlist.json').read_text()))
    for path in paths:
        destination = OUTPUT / (tag + '-drift-bytes') / path
        destination.parent.mkdir(parents=True, exist_ok=True)
        shutil.copy2(LANE / path, destination)
    unexpected = sorted(set(paths) - allowed)
    if not unexpected and paths:
        subprocess.run(['git', 'checkout', '--', *paths], cwd=LANE, check=True)
    after = manifest(production, LANE)
    write(tag + '-production-after.json', after)
    clean = git('status', '--porcelain=v1', '-uall') == ''
    write(tag + '-custody-result.json', {'host_count': len(host_after), 'host_identical': host == host_after, 'production_count': len(after),
                                       'production_identical': production == after, 'restored_paths': paths if not unexpected else [], 'unexpected_archived_not_restored': unexpected, 'clean': clean})
    result = subprocess.run([LOCK, 'release', '--owner', OWNER, '--task-id', TASK, '--lock-id', metadata['lock_id'],
                             '--reason', 'Editor exited; harness/meta removed; drift archived; ten-path allowlist applied only if no unexpected drift; custody recorded', '--json'], capture_output=True, text=True)
    (OUTPUT / (tag + '-release.json')).write_text(result.stdout)
    assert result.returncode == 0, result.stderr
    print(result.stdout)
    assert not unexpected and host == host_after and production == after and clean
else:
    raise SystemExit(operation)
