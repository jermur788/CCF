#!/usr/bin/env python3
"""MFM-11 Calibration Impact — Unity gate runner (verification-only).

Stages the disposable gate into Assets/ForestPrototype/, runs Unity with the machine-wide lock,
captures evidence outside the repo, restores known Editor side effects, and removes the harness.

Usage: run_gate.py --lock-id ID --owner OWNER --expected-head SHA [--out DIR] [--display :84]
"""
import argparse
import hashlib
import json
import os
import shutil
import subprocess
import sys
import time
from pathlib import Path

ROOT = Path(__file__).resolve().parents[3]
TASK_ID = 'MFM-12'
UNITY = Path('/media/jer/ZX20/Unity/6000.6.0f1/Editor/Unity')
LOCK_SCRIPT = Path('/media/jer/ZX20/CCF-Ops/scripts/unity-lock')

GATES = {
    'calibration': ('Mfm12IntegratedQualification/Mfm11CalibrationImpactGate.cs', 'MFM11_UNITY_PASS', 'MFM11_UNITY_FAIL'),
}

DRIVER_GATES = ('calibration',)
SAVES_ALLOWED = ('calibration',)


def sha256_file(path):
    h = hashlib.sha256()
    with open(path, 'rb') as f:
        for chunk in iter(lambda: f.read(65536), b''):
            h.update(chunk)
    return h.hexdigest()


def preflight(args, root):
    """Ownership preflight: validate lock, head, cleanliness."""
    if args.preflight_only or args.gate != 'compile':
        compile_result = args.out.parent / '01-compile' / 'process-result.json'
        if compile_result.exists():
            data = json.loads(compile_result.read_text())
            if data.get('head') != args.expected_head:
                raise SystemExit('COMPILE GATE HEAD MISMATCH: %s != %s' % (data.get('head'), args.expected_head))

    status = subprocess.run(
        [sys.executable, '-B', str(LOCK_SCRIPT), 'status', '--json'],
        capture_output=True, text=True, check=True,
    )
    lock = json.loads(status.stdout)
    meta = lock.get('metadata', {})
    problems = []
    if lock.get('status') != 'held':
        problems.append('lock not held: %s' % lock.get('status'))
    if meta.get('lock_id') != args.lock_id:
        problems.append('lock_id mismatch: %s != %s' % (meta.get('lock_id'), args.lock_id))
    if meta.get('task_id') != TASK_ID:
        problems.append('task_id mismatch: %s != %s' % (meta.get('task_id'), TASK_ID))
    if meta.get('owner') != args.owner:
        problems.append('owner mismatch: %s != %s' % (meta.get('owner'), args.owner))
    if meta.get('head') != args.expected_head:
        problems.append('lock head mismatch: %s != %s' % (meta.get('head'), args.expected_head))
    actual_head = subprocess.run(
        ['git', '-C', str(root), 'rev-parse', 'HEAD'],
        capture_output=True, text=True, check=True,
    ).stdout.strip()
    if actual_head != args.expected_head:
        problems.append('git head mismatch: %s != %s' % (actual_head, args.expected_head))
    if lock.get('unity_editor_pids'):
        problems.append('Editor PIDs running: %s' % lock['unity_editor_pids'])
    porcelain = subprocess.run(
        ['git', '-C', str(root), 'status', '--porcelain'],
        capture_output=True, text=True, check=True,
    ).stdout.strip()
    if porcelain:
        problems.append('worktree not clean:\n%s' % porcelain)
    if problems:
        raise SystemExit('OWNERSHIP CONFLICT:\n' + '\n'.join(problems))
    print('MFM12 calibration-witness ownership preflight PASS: %s' % args.expected_head)
    if args.preflight_only:
        return


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument('--gate', default='calibration', choices=list(GATES))
    parser.add_argument('--out', type=Path, default=None)
    parser.add_argument('--lock-id', required=True)
    parser.add_argument('--owner', required=True)
    parser.add_argument('--expected-head', required=True)
    parser.add_argument('--display', default=':84')
    parser.add_argument('--timeout', type=int, default=3600)
    parser.add_argument('--reap-grace', type=int, default=120,
                        help='Seconds after a verdict before reaping our own Editor')
    parser.add_argument('--preflight-only', action='store_true')
    args = parser.parse_args()

    stamp = time.strftime('%Y%m%dT%H%M%SZ', time.gmtime())
    if args.out is None:
        args.out = Path('/media/jer/Files/CCF/Verification/MFM12-IntegratedQualification-20261102/unity') / stamp
    args.out = args.out.resolve()
    args.out.mkdir(parents=True, exist_ok=True)

    preflight(args, ROOT)
    if args.preflight_only:
        return 0

    source_rel, pass_marker, fail_marker = GATES[args.gate]
    source = ROOT / 'Tools' / 'Verification' / source_rel
    harness = source.name
    staged = ROOT / 'Assets' / 'ForestPrototype' / harness
    pairs = [(source, staged)]

    if args.gate in DRIVER_GATES:
        native_src = ROOT / 'Tools' / 'Verification' / 'RackV2NoCompetingAuthor' / 'RackAuthoringNativeInput.cs'
        pairs.append((native_src, ROOT / 'Assets' / 'ForestPrototype' / native_src.name))

    # Stage
    for origin, target in pairs:
        if target.exists():
            raise SystemExit('staged target already exists: %s' % target)
        meta = target.with_suffix(target.suffix + '.meta')
        if meta.exists():
            raise SystemExit('staged .meta already exists: %s' % meta)
        shutil.copy2(origin, target)

    entry = source.stem + '.Begin'
    evidence = args.out / 'evidence'
    evidence.mkdir(exist_ok=True)
    driver_dir = args.out / 'driver'
    if args.gate in DRIVER_GATES:
        driver_dir.mkdir(exist_ok=True)

    # Environment
    config_dir = args.out / 'config'
    config_dir.mkdir(exist_ok=True)

    # Copy Unity licence into isolated config
    licences = [Path.home() / '.config' / 'unity3d' / 'Unity' / 'licenses',
                Path.home() / '.var' / 'app' / 'com.unity.UnityHub' / 'config' / 'unity3d' / 'Unity' / 'licenses']
    sources = [p for p in licences if (p / 'UnityEntitlementLicense.xml').is_file()]
    if not sources:
        raise SystemExit('No existing local Unity licence; no launch')
    licence_copy = config_dir / 'unity3d' / 'Unity' / 'licenses'
    shutil.copytree(max(sources, key=lambda p: (p / 'UnityEntitlementLicense.xml').stat().st_mtime), licence_copy, dirs_exist_ok=True)

    env = {
        'XDG_CONFIG_HOME': str(config_dir),
        'DISPLAY': args.display,
        'TMPDIR': '/tmp/',
        'CCF_DIAG_DIR': str(evidence),
        'CCF_DRIVER_DIR': str(driver_dir),
        'CCF_TEST_DIRECT_ENTRY': '1',
        'DOTNET_CLI_TELEMETRY_OPTOUT': '1',
    }
    # Preserve essential env
    for key in ('PATH', 'HOME', 'USER', 'LANG', 'LC_ALL', 'UNITY_EXT_LOGGING'):
        if key in os.environ:
            env[key] = os.environ[key]

    command = [str(UNITY), '-projectPath', str(ROOT), '-logFile', str(args.out / 'unity.log'), '-executeMethod', entry]
    (args.out / 'command.json').write_text(json.dumps({
        'command': command, 'gate': args.gate, 'head': args.expected_head,
        'lock_id': args.lock_id, 'harness_sha256': sha256_file(source),
        'runner_sha256': sha256_file(Path(__file__)),
    }, indent=2))

    # Snapshot side effects
    vscode = ROOT / '.vscode' / 'settings.json'
    vscode_bytes = vscode.read_bytes() if vscode.exists() else None
    mat_files = list((ROOT / 'Assets' / 'ForestPrototype' / 'Art' / 'SectionFive').glob('*/Materials/*.mat')) if (ROOT / 'Assets' / 'ForestPrototype' / 'Art' / 'SectionFive').exists() else []
    mat_bytes = {m: m.read_bytes() for m in mat_files}

    # Host saves
    host_save_dir = Path.home() / '.config' / 'unity3d' / 'DefaultCompany' / 'CCF'
    host_saves_before = {}
    if host_save_dir.exists():
        for f in host_save_dir.glob('forest-save.json*'):
            host_saves_before[str(f)] = sha256_file(f)

    # Launch Xephyr + openbox
    xephyr = subprocess.Popen(['Xephyr', args.display, '-screen', '2560x1440', '-ac', '-noreset'],
                              stdout=open(args.out / 'xephyr.log', 'w'), stderr=subprocess.STDOUT)
    time.sleep(1)
    openbox = subprocess.Popen(['openbox'], env=dict(env),
                               stdout=open(args.out / 'openbox.log', 'w'), stderr=subprocess.STDOUT)
    time.sleep(0.5)

    # Launch native input driver if needed
    driver_proc = None
    if args.gate in DRIVER_GATES:
        driver_proc = subprocess.Popen(
            [sys.executable, '-I', '-B', str(ROOT / 'Tools' / 'Verification' / 'RackV2NoCompetingAuthor' / 'native_input.py'),
             str(driver_dir), args.display],
            env=dict(env), stdout=open(args.out / 'driver.stdout', 'w'), stderr=subprocess.STDOUT,
        )

    # Launch Unity
    start = time.time()
    unity = subprocess.Popen(command, env=dict(env), stdout=open(args.out / 'unity-stdout.log', 'w'), stderr=subprocess.STDOUT)
    (args.out / 'process.json').write_text(json.dumps({'unity_pid': unity.pid, 'driver_pid': driver_proc.pid if driver_proc else None}))

    # Wait for Unity. Reap only our own Editor if it has printed its verdict but
    # does not exit (the calibration witness can spin in shutdown after its verdict).
    finish_timeout = time.time() + args.timeout
    finished_at = None
    reaped = False
    while True:
        code = unity.poll()
        if code is not None:
            break
        if time.time() > finish_timeout:
            unity.kill()
            code = unity.wait()
            break
        log_now = (args.out / 'unity.log').read_text(errors='replace') if (args.out / 'unity.log').exists() else ''
        if (pass_marker in log_now or fail_marker in log_now) and finished_at is None:
            finished_at = time.time()
        if finished_at is not None and time.time() - finished_at > args.reap_grace:
            unity.kill()
            code = unity.wait()
            reaped = True
            (args.out / 'reaper.txt').write_text('reaped own Editor pid %d after verdict\n' % unity.pid)
            break
        time.sleep(2)
    elapsed = time.time() - start

    # Stop driver
    if driver_proc:
        driver_proc.terminate()
        try:
            driver_proc.wait(timeout=5)
        except subprocess.TimeoutExpired:
            driver_proc.kill()

    # Stop Xephyr
    openbox.terminate()
    xephyr.terminate()

    # Read log
    log_text = (args.out / 'unity.log').read_text() if (args.out / 'unity.log').exists() else ''
    has_pass = pass_marker in log_text
    failures = [line for line in log_text.splitlines() if fail_marker in line or ' error CS' in line]

    # Cleanup staged files
    harness_removed = True
    cleanup_failures = []
    for _, target in pairs:
        try:
            if target.exists():
                target.unlink()
            meta = target.with_suffix(target.suffix + '.meta')
            if meta.exists():
                meta.unlink()
        except Exception as e:
            harness_removed = False
            cleanup_failures.append(str(e))

    # Remove copied licence
    licence_copy_removed = True
    try:
        if licence_copy.exists():
            shutil.rmtree(licence_copy)
    except Exception as e:
        licence_copy_removed = False
        cleanup_failures.append('copied licence removal failed: ' + str(e))
    if licence_copy.exists() and licence_copy_removed:
        licence_copy_removed = False
        cleanup_failures.append('copied licence directory remains after cleanup')

    # Restore side effects
    if vscode_bytes is not None:
        vscode.write_bytes(vscode_bytes)
    elif vscode.exists():
        vscode.unlink()
    for m, b in mat_bytes.items():
        m.write_bytes(b)

    # Check host saves unchanged
    host_saves_unchanged = True
    if host_save_dir.exists():
        for f in host_save_dir.glob('forest-save.json*'):
            if str(f) in host_saves_before:
                if sha256_file(f) != host_saves_before[str(f)]:
                    host_saves_unchanged = False

    # Isolated saves
    isolated_saves = list(config_dir.glob('**/forest-save.json*')) if config_dir.exists() else []
    disk_saves_created = []
    for s in isolated_saves:
        if args.gate not in SAVES_ALLOWED:
            disk_saves_created.append(str(s))
        s.unlink(missing_ok=True)

    # Remaining lane changes
    porcelain = subprocess.run(
        ['git', '-C', str(ROOT), 'status', '--porcelain'],
        capture_output=True, text=True, check=True,
    ).stdout.strip()

    result = {
        'command': command, 'head': args.expected_head, 'lock_id': args.lock_id,
        'gate': args.gate, 'unity_exit': code, 'seconds': round(elapsed, 1),
        'pass_marker': has_pass, 'failures': failures, 'reaped': reaped,
        'harness_removed': harness_removed, 'licence_copy_removed': licence_copy_removed,
        'cleanup_failures': cleanup_failures,
        'host_saves_unchanged': host_saves_unchanged,
        'disk_saves_created': disk_saves_created,
        'remaining_lane_changes': porcelain,
        'pass': host_saves_unchanged and not porcelain and not disk_saves_created
                and (code == 0 or (reaped and has_pass)) and has_pass and not failures
                and harness_removed and licence_copy_removed and not cleanup_failures,
    }
    (args.out / 'process-result.json').write_text(json.dumps(result, indent=2))

    print('MFM12_CALIBRATION_WITNESS_RUN exit=%d pass=%s checks_in_log=%s' % (code, result['pass'], has_pass))
    return 0 if result['pass'] else 1


if __name__ == '__main__':
    sys.exit(main())
