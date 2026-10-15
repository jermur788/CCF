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

def storage_check(root=ROOT, uuid='18D6-896E', fstype='exfat', device='/dev/sda2'):
    root = Path(root).resolve(strict=True)
    info = json.loads(command(['findmnt','-J','-T',str(root),'-o','SOURCE,TARGET,FSTYPE,UUID']))['filesystems'][0]
    if info['source'] != device or info['target'] != str(root) or info['fstype'] != fstype or (info['uuid'] or '').lower() != uuid.lower():
        raise ValueError(f'ZX20 mount identity mismatch: {info}')
    return root


def storage_health(storage, free_bytes=None, mount_info=None):
    """Observe storage; test arguments may only lower the reported free space."""
    root = Path(storage['root']).resolve(strict=True)
    info = mount_info or json.loads(command(['findmnt','-J','-T',str(root),'-o','SOURCE,TARGET,FSTYPE,UUID,OPTIONS']))['filesystems'][0]
    identity = (info['source'] == storage['device'] and info['target'] == str(root)
                and info['fstype'] == storage['fstype']
                and (info['uuid'] or '').lower() == storage['uuid'].lower())
    stats = os.statvfs(root)
    available = stats.f_bavail * stats.f_frsize
    if free_bytes is not None:
        if type(free_bytes) is not int or not 0 <= free_bytes <= available:
            raise ValueError('test free bytes must be nonnegative and cannot increase actual capacity')
        available = free_bytes
    threshold = storage['minimum_free_bytes']
    writable = 'rw' in info.get('options','').split(',')
    status = 'PASS' if identity and writable and available >= threshold else 'WRONG DEVICE' if not identity else 'READ ONLY' if not writable else 'LOW SPACE'
    return dict(status=status,ready=status=='PASS',backing_device=info['source'],writable=writable,
                free_bytes=available,free_gib=available/1024**3,minimum_free_bytes=threshold,
                minimum_free_gib=threshold/1024**3)


def require_storage(cfg):
    health = storage_health(cfg['storage'])
    if not health['ready']:
        raise ValueError(f"BLOCK / {health['status']}: free={health['free_bytes']} threshold={health['minimum_free_bytes']}")
    return health

def config_read(path, permit_off_storage_lanes=False):
    path = Path(path)
    cfg = json.loads(path.read_text())
    if not isinstance(cfg,dict) or cfg.get('schema') != 1 or not isinstance(cfg.get('lanes'), list) or not cfg['lanes']:
        raise ValueError('config requires schema 1 and nonempty lanes list')
    s = cfg['storage']
    if not isinstance(s,dict):
        raise ValueError('storage must be an object')
    if type(s.get('minimum_free_bytes')) is not int or s['minimum_free_bytes'] <= 0 or not s.get('device'):
        raise ValueError('storage needs device and positive minimum_free_bytes')
    root = storage_check(s['root'], s['uuid'], s['fstype'], s['device'])
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
        if type(lane.get('unity')) is not bool:
            raise ValueError('each lane requires unity: true/false')
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
    stores = cfg.get('git_stores')
    if not isinstance(stores,list) or not stores:
        raise ValueError('config requires nonempty git_stores for branch preflight')
    store_paths = []
    for store in stores:
        if not isinstance(store,dict) or not store.get('name') or not Path(store['git_dir']).is_absolute():
            raise ValueError('store requires name and absolute git_dir')
        store_paths.append(str(Path(store['git_dir']).resolve()))
    if len(set(store_paths)) != len(store_paths) or str(repo.resolve()) not in store_paths:
        raise ValueError('duplicate Git store or configured repository missing from git_stores')
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

def resolve_lane(cfg, label):
    for lane in cfg['lanes']:
        if str(label) in {lane['lane'], 'CCF-'+lane['lane'], lane['path']}:
            return lane
    raise ValueError('lane is not configured: '+str(label))


def store_checkouts(cfg):
    """Both stores are read-only inputs. Registered branch checkouts are potential writers."""
    result = []
    for store in cfg['git_stores']:
        raw = command(['git','--git-dir='+store['git_dir'],'worktree','list','--porcelain'])
        for block in raw.split('\n\n'):
            entry = dict(line.split(' ',1) if ' ' in line else (line,True) for line in block.splitlines())
            if 'worktree' not in entry or 'bare' in entry:
                continue
            result.append(dict(store=store['name'],git_dir=store['git_dir'],path=entry['worktree'],
                               branch=entry.get('branch','').removeprefix('refs/heads/'),
                               head=entry.get('HEAD'),locked=entry.get('locked')))
    return result


def branch_conflicts(checkouts, branch, selected_path):
    return [c for c in checkouts if c['branch'] == branch
            and Path(c['path']).resolve() != Path(selected_path).resolve()]


def board_conflicts(rows, cfg=None):
    warnings = []
    for field in ['Lane','Branch']:
        seen = {}
        for row in rows:
            key = row[field]
            if key.lower() in {'none','n/a','tbd','-'}:
                continue
            if field == 'Lane' and cfg:
                try: key = resolve_lane(cfg,key)['path']
                except ValueError: pass
            if key in seen:
                warnings.append(f"duplicate live {field}: {key} ({seen[key]}, {row['Task ID']})")
            seen[key] = row['Task ID']
    return warnings


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
