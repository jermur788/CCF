"""Small shared parsing/storage checks; Python standard library only."""
import json
import os
from pathlib import Path
import re
import subprocess

ROOT = Path('/media/jer/ZX20')
BOARD_FIELDS = ['Task ID', 'State', 'Owner', 'Harness', 'Model', 'Quota source',
                'Lane', 'Branch', 'HEAD', 'Subsystem', 'Locks', 'Unity required?',
                'Unity queue position', 'Verification tier', 'Open gate', 'Dependency', 'Next action']
STATES = {'READY','ACTIVE','WAITING','BLOCKED','VERIFY','INTEGRATE','DONE-PENDING-LEDGER'}

def command(args, cwd=None):
    env = dict(os.environ, GIT_OPTIONAL_LOCKS='0', PYTHONDONTWRITEBYTECODE='1')
    p = subprocess.run(args, cwd=cwd, env=env, capture_output=True, text=True)
    if p.returncode:
        raise ValueError(f'{args[0]} failed ({p.returncode}): {p.stderr.strip()}')
    return p.stdout.strip()

def on_storage(path, root=ROOT):
    root = root.resolve(strict=True)
    path = Path(path).resolve()
    if not path.is_relative_to(root):
        raise ValueError(f'path escapes ZX20: {path}')
    parent = path
    while not parent.exists():
        parent = parent.parent
    if os.stat(parent).st_dev != os.stat(root).st_dev:
        raise ValueError(f'path is on another filesystem: {path}')
    return path

def storage_check(root=ROOT, uuid='18D6-896E', fstype='exfat'):
    root = Path(root).resolve(strict=True)
    info = json.loads(command(['findmnt','-J','-T',str(root),'-o','SOURCE,TARGET,FSTYPE,UUID']))['filesystems'][0]
    if info['source'] != '/dev/sda2' or info['target'] != str(root) or info['fstype'] != fstype or (info['uuid'] or '').lower() != uuid.lower():
        raise ValueError(f'ZX20 mount identity mismatch: {info}')
    return root

def config_read(path, permit_off_storage_lanes=False):
    path = Path(path)
    cfg = json.loads(path.read_text())
    if not isinstance(cfg,dict) or cfg.get('schema') != 1 or not isinstance(cfg.get('lanes'), list) or not cfg['lanes']:
        raise ValueError('config requires schema 1 and nonempty lanes list')
    s = cfg['storage']
    if not isinstance(s,dict):
        raise ValueError('storage must be an object')
    root = storage_check(s['root'], s['uuid'], s['fstype'])
    # Alternate mounts/lock universes are not silently configurable.
    if root != ROOT:
        raise ValueError('config root must be the verified ZX20 mount')
    repo = on_storage(cfg['repository'], root)
    if not repo.is_dir():
        raise ValueError(f'missing repository: {repo}')
    seen_names, seen_paths = set(), set()
    for lane in cfg['lanes']:
        if not isinstance(lane,dict):
            raise ValueError('each lane must be an object')
        if not isinstance(lane.get('lane'), str) or not lane['lane'].strip() or not lane.get('purpose'):
            raise ValueError('lane needs nonempty lane, purpose and absolute path')
        if not Path(lane['path']).is_absolute():
            raise ValueError('lane path must be absolute')
        # Only the read-only inventory may observe off-storage paths so it can
        # report ZX20: NO; mutation callers retain strict storage rejection.
        resolved = str(Path(lane['path']).resolve() if permit_off_storage_lanes else on_storage(lane['path'], root))
        if lane['lane'] in seen_names or resolved in seen_paths:
            raise ValueError('duplicate lane name/path')
        seen_names.add(lane['lane']); seen_paths.add(resolved)
    board = on_storage(path.resolve().parent / cfg['board'], root)
    return cfg, board

def board_read(path):
    lines = [line.strip() for line in Path(path).read_text().splitlines() if line.strip().startswith('|')]
    if len(lines) < 2:
        raise ValueError('board table/header missing')
    split = lambda line: [v.strip() for v in line.strip('|').split('|')]
    headers = split(lines[0])
    if headers != BOARD_FIELDS or len(split(lines[1])) != len(headers):
        raise ValueError('board must use the documented ordered required fields')
    rows, ids = [], set()
    for line in lines[2:]:
        cells = split(line)
        if len(cells) != len(headers) or any(not c for c in cells):
            raise ValueError('malformed board row; use None/N/A for empty values')
        row = dict(zip(headers,cells))
        if row['State'] not in STATES or row['Task ID'] in ids:
            raise ValueError('invalid board state or duplicate ID')
        if row['Unity required?'] not in {'Yes','No'}:
            raise ValueError('Unity required? must be Yes/No')
        if row['HEAD'] != 'TBD' and not re.fullmatch(r'[0-9a-fA-F]{40}',row['HEAD']):
            raise ValueError('board HEAD must be exact SHA or TBD')
        ids.add(row['Task ID']); rows.append(row)
    return rows

def fields_read(text):
    """Colon fields and Markdown headings; no natural-language interpretation."""
    fields, current = {}, None
    for line in text.splitlines():
        if line.strip().startswith('```'):
            continue
        match = re.match(r'^\s*(?:#{1,6}\s+)?([A-Za-z][A-Za-z0-9 /?_-]*):\s*(.*)$',line)
        heading = re.match(r'^#{1,6}\s+([A-Za-z][A-Za-z0-9 /?_-]*)\s*$',line)
        if match or heading:
            key = (match or heading).group(1).strip().lower()
            value = match.group(2).strip() if match else ''
            fields.setdefault(key,[]).append(value)
            current = fields[key]
        elif current is not None and line.strip():
            current[-1] += '\n' + line.strip()
    return fields
