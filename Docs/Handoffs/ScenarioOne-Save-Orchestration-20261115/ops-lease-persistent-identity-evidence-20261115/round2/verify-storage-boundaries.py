#!/usr/bin/env python3
"""Real mount/config regression probes; no Editor, live acquisition or force."""
import argparse
from copy import deepcopy
import hashlib
import json
import os
from pathlib import Path
import shutil
import subprocess
import sys
import tempfile
import uuid


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--ops', type=Path, required=True)
    parser.add_argument('--output', type=Path, required=True)
    args = parser.parse_args()
    ops = args.ops.resolve()
    live = Path('/media/jer/ZX20/CCF-Ops')
    sys.path.insert(0, str(ops / 'scripts'))
    import _ccf
    cfg = json.loads((ops / 'lanes.json').read_text())
    _ccf.storage_check()
    results = []

    def check(value, label, **details):
        results.append(dict(label=label, passed=bool(value), **details))
        print(('PASS: ' if value else 'FAIL: ') + label, flush=True)
        assert value, results[-1]

    def run(command):
        p = subprocess.run([str(v) for v in command], capture_output=True,
                           text=True, env=dict(os.environ, GIT_OPTIONAL_LOCKS='0',
                                              PYTHONDONTWRITEBYTECODE='1'))
        return dict(command=[str(v) for v in command], exit_code=p.returncode,
                    stdout=p.stdout, stderr=p.stderr)

    def rejected(config, value, permit, label, error):
        config.write_text(json.dumps(value))
        try:
            _ccf.config_read(config, permit_off_storage_lanes=permit)
        except ValueError as exc:
            check(error in str(exc), label, error=str(exc), permission=permit)
        else:
            check(False, label)

    def metadata(root):
        return {str(p.relative_to(root)): (p.stat().st_mtime_ns,
                hashlib.sha256(p.read_bytes()).hexdigest())
                for p in root.rglob('*') if p.is_file()}

    try:
        with tempfile.TemporaryDirectory(prefix='Ops548Boundary-',
                dir='/media/jer/Files/CCF/Operations') as tmp:
            scratch = Path(tmp)
            # Minimal scripts/config only: no board, ledger or state import.
            for label, source in [('candidate', ops), ('live-copy', live)]:
                target = scratch / label
                (target / 'scripts').mkdir(parents=True)
                for name in ['_ccf.py', 'unity-lock']:
                    shutil.copyfile(source / 'scripts' / name, target / 'scripts' / name)
                baseline = json.loads((source / 'lanes.json').read_text())
                config = target / 'lanes.json'
                negatives = [('wrong UUID', 'uuid', 'DEAD-BEEF'),
                    ('wrong fstype', 'fstype', 'ext4'),
                    ('nonexistent by-id', 'device', '/dev/disk/by-id/CCF-NONEXISTENT-548'),
                    ('wrong mount root', 'root', '/media/jer/Files')]
                # Failed admission must occur before any guard/state creation.
                state = _ccf.ROOT / 'CCF-Temp' / ('ops548-never-created-' + uuid.uuid4().hex)
                for description, key, value in negatives:
                    bad = deepcopy(baseline)
                    bad['storage'][key] = value
                    config.write_text(json.dumps(bad))
                    for operation in ['status', 'acquire', 'release']:
                        result = run([sys.executable, '-B', target / 'scripts/unity-lock',
                                      operation, '--test-state-dir', state, '--json'])
                        check(result['exit_code'] == 2 and
                              'ZX20 mount identity mismatch' in result['stderr'],
                              f'{label}: {description} rejects {operation}', **result)
                        check(not state.exists(), f'{label}: {description} {operation} creates no state')

            config = scratch / 'boundary.json'
            baseline = deepcopy(cfg)
            baseline['board'] = str(ops / 'board/active.md')
            config.write_text(json.dumps(baseline))
            loaded, board = _ccf.config_read(config)
            check(loaded['repository'] == cfg['repository'] and board == ops / 'board/active.md',
                  'Files repository and config-relative Files board admitted')
            check(_ccf.on_project_storage(_ccf.ROOT / 'CCF-Ops') == _ccf.ROOT / 'CCF-Ops',
                  'ZX20 project path remains admitted')
            for outside in ['/', '/tmp', '/media/jer/Files/CCF-sibling/repository.git']:
                bad = deepcopy(baseline)
                bad['repository'] = outside
                rejected(config, bad, False, 'repository outside both trees rejected: ' + outside,
                         'repository must be on ZX20 or the Files CCF tree')
            link = scratch / 'escape'
            link.symlink_to('/tmp', target_is_directory=True)
            bad = deepcopy(baseline)
            bad['repository'] = str(link)
            rejected(config, bad, False, 'repository symlink escape rejected',
                     'repository must be on ZX20 or the Files CCF tree')
            bad = deepcopy(baseline)
            bad['board'] = '/tmp/outside-board.md'
            rejected(config, bad, False, 'board outside both trees rejected',
                     'outside the Files CCF tree')

            def lane_config(path, unity=False):
                value = deepcopy(baseline)
                value['lanes'] = [dict(lane='fixture', path=str(path),
                                       purpose='isolated boundary test', unity=unity)]
                return value

            durable_lane = scratch / 'durable-lane'
            value = lane_config(durable_lane)
            rejected(config, value, False, 'Files non-Unity lane rejected without explicit permission',
                     'path escapes ZX20')
            config.write_text(json.dumps(value))
            _ccf.config_read(config, permit_off_storage_lanes=True)
            check(True, 'Files non-Unity lane admitted with explicit permission')
            for permit in [False, True]:
                rejected(config, lane_config(durable_lane, unity=True), permit,
                         f'Files Unity lane rejected (permission={permit})', 'path escapes ZX20')
            for outside in ['/', '/tmp', '/media/jer/Files/CCF-sibling/lane', str(link)]:
                for permit in [False, True]:
                    rejected(config, lane_config(outside), permit,
                             f'lane escape rejected: {outside} (permission={permit})',
                             'outside the Files CCF tree' if permit else 'path escapes ZX20')
            # Real local Git fixture on Files verifies the newly ported inventory.
            result = run(['git', 'init', '-b', 'test/ops548-durable', durable_lane])
            check(result['exit_code'] == 0, 'isolated durable Git fixture created', **result)
            result = run(['git', '-C', durable_lane, '-c', 'user.name=CCF Offline Test',
                          '-c', 'user.email=test@invalid.local', '-c', 'commit.gpgSign=false',
                          '-c', 'core.hooksPath=/dev/null', 'commit', '--allow-empty',
                          '-m', 'isolated durable inventory fixture'])
            check(result['exit_code'] == 0, 'isolated fixture HEAD created', **result)
            value = lane_config(durable_lane)
            value['repository'] = str(durable_lane / '.git')
            value['git_stores'] = [dict(name='fixture', git_dir=value['repository'])]
            config.write_text(json.dumps(value))
            before = metadata(durable_lane / '.git')
            result = run([sys.executable, '-B', ops / 'scripts/lane-status',
                          '--config', config, '--json'])
            row = json.loads(result['stdout'])[0] if result['exit_code'] == 0 else {}
            check(result['exit_code'] == 0 and row.get('zx20') is False and
                  row.get('registered_worktree') is True and
                  'approx_disk_usage' in row and row['approx_disk_usage'] and
                  row['conflict_warning'] == [], 'durable non-Unity inventory inspected as Git worktree', **result)
            check(before == metadata(durable_lane / '.git'),
                  'durable inventory preserves Git metadata bytes and mtimes')
            value['lanes'][0]['path'] = '/'
            config.write_text(json.dumps(value))
            result = run([sys.executable, '-B', ops / 'scripts/lane-status', '--config', config, '--json'])
            check(result['exit_code'] == 2 and 'outside the Files CCF tree' in result['stderr'],
                  'lane-status explicit permission cannot admit arbitrary root', **result)

        health = _ccf.storage_health(cfg['storage'])
        check(health['status'] == 'PASS', 'positive physical storage identity and capacity PASS', health=health)
        for label, source in [('candidate', ops), ('live', live)]:
            result = run([sys.executable, '-B', source / 'scripts/unity-lock', 'status', '--json'])
            status = json.loads(result['stdout']) if result['exit_code'] == 0 else {}
            check(result['exit_code'] == 0 and status.get('status') == 'free' and
                  status.get('metadata') is None and status.get('unity_editor_pids') == [],
                  label + ' lock reports free with zero Editor PIDs', **result)
        print(f'STORAGE_BOUNDARY_VERIFICATION_PASS {len(results)}', flush=True)
    finally:
        args.output.write_text(json.dumps(results, indent=2) + '\n')


if __name__ == '__main__':
    main()
