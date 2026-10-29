#!/usr/bin/env python3
"""Run the disposable PARITY-07 rendered qualification under an already-held CCF Unity lock.

The caller holds the machine-wide lock (CCF-Ops/scripts/unity-lock) for the exact clean Unity lane; this runner never
acquires or releases it. It refuses a wrong or dirty lane, another task's lock, a running Unity Editor, a leftover staged
harness or an evidence folder outside /media/jer/Files/CCF/Verification/.

It starts a private Xephyr display and openbox, copies Parity07UnityRender.cs from next to this script into
Assets/ForestPrototype/ of --project, launches Unity 6000.6.0f1 with -executeMethod Parity07Unity.Begin and an isolated
XDG_CONFIG_HOME (so the player's forest-save.json is never touched), and always removes the harness, its .meta and the
copied licence. Known Editor import rewrites are restored by exact path; anything else is reported and left untouched.

NOT a Unity result until it has actually run: see README.md in this folder.
"""
import argparse
import json
import os
from pathlib import Path
import re
import shutil
import signal
import subprocess
import sys
import time

HERE = Path(__file__).resolve().parent
UNITY = Path('/media/jer/ZX20/Unity/6000.6.0f1/Editor/Unity')
UNITY_LOCK = Path('/media/jer/ZX20/CCF-Ops/scripts/unity-lock')
EVIDENCE_ROOT = Path('/media/jer/Files/CCF/Verification')
HARNESS = 'Parity07UnityRender.cs'
METHOD = 'Parity07Unity.Begin'
PASS_MARKER = 'PARITY07_UNITY_HARNESS_PASS'
FAIL_MARKER = 'PARITY07_UNITY_HARNESS_FAIL'
KNOWN_SIDE_EFFECTS = {
    '.vscode/settings.json',
    'Assets/ForestPrototype/Art/SectionFive/Bilberry_Type_Cover_01/Materials/Bilberry_Leaves.mat',
    'Assets/ForestPrototype/Art/SectionFive/Bilberry_Type_Cover_01/Materials/Bilberry_Stems.mat',
    'Assets/ForestPrototype/Art/SectionFive/Woodland_Forbs_01/Materials/Forb_Flower_Centre.mat',
    'Assets/ForestPrototype/Art/SectionFive/Woodland_Forbs_01/Materials/Forb_Leaves.mat',
    'Assets/ForestPrototype/Art/SectionFive/Woodland_Forbs_01/Materials/Forb_Stems.mat',
}


def git(root, *args):
    return subprocess.check_output(['git', '-C', str(root), *args], text=True)


def running_editors(own_pid):
    """Unity Editor processes visible in /proc (no pgrep: it matches itself)."""
    found = []
    for entry in Path('/proc').iterdir():
        if not entry.name.isdigit() or int(entry.name) == own_pid:
            continue
        try:
            cmd = (entry / 'cmdline').read_bytes().replace(b'\0', b' ').decode(errors='replace')
        except OSError:
            continue
        if '/Editor/Unity' in cmd and 'Unity.app' not in cmd and 'unity-lock' not in cmd:
            found.append(entry.name)
    return found


def preflight(args, root, out):
    problems = []
    if not (root / 'Assets/Scenes/ForestTest.unity').is_file():
        problems.append('project has no Assets/Scenes/ForestTest.unity: ' + str(root))
    if out != EVIDENCE_ROOT and EVIDENCE_ROOT not in out.parents:
        problems.append('evidence folder must be under ' + str(EVIDENCE_ROOT))
    if root in out.parents or out == root:
        problems.append('evidence folder must be outside the repository')
    if out.exists() and any(out.iterdir()):
        problems.append('evidence folder already exists and is not empty: ' + str(out))
    if not UNITY.is_file():
        problems.append('Unity 6000.6.0f1 not found at ' + str(UNITY))
    try:
        actual = git(root, 'rev-parse', 'HEAD').strip()
        expected = git(root, 'rev-parse', '--verify', args.expected_head + '^{commit}').strip()
        dirty = git(root, 'status', '--porcelain', '--untracked-files=all').strip()
    except subprocess.CalledProcessError as error:
        return problems + ['git inspection failed: ' + str(error)]
    if actual != expected:
        problems.append(f'lane HEAD {actual} is not the expected {expected}')
    if dirty:
        problems.append('lane is not clean:\n' + dirty)
    try:
        lock = json.loads(subprocess.check_output(['python3', '-B', str(UNITY_LOCK), 'status', '--json'], text=True))
    except (subprocess.CalledProcessError, OSError, ValueError) as error:
        return problems + ['cannot read the Unity lock: ' + str(error)]
    meta = lock.get('metadata') or {}
    if lock.get('status') != 'held' or meta.get('lock_id') != args.lock_id:
        problems.append('the held Unity lock is not this task\'s lock-id ' + args.lock_id)
    if meta.get('lane') not in (None, str(root)):
        problems.append('lock names a different lane: ' + str(meta.get('lane')))
    if meta.get('head') not in (None, actual):
        problems.append('lock names a different head: ' + str(meta.get('head')))
    if args.task_id and meta.get('task_id') != args.task_id:
        problems.append('lock belongs to task ' + str(meta.get('task_id')))
    if lock.get('unity_editor_pids') or running_editors(os.getpid()):
        problems.append('a Unity Editor is already running')
    staged = root / 'Assets/ForestPrototype' / HARNESS
    if staged.exists() or Path(str(staged) + '.meta').exists():
        problems.append('existing staged harness/meta: inspect ownership before running')
    display_number = args.display.lstrip(':').split('.')[0]
    if Path('/tmp/.X11-unix/X' + display_number).exists() or Path('/tmp/.X' + display_number + '-lock').exists():
        problems.append('requested private display ' + args.display + ' is occupied')
    if not any((p / 'UnityEntitlementLicense.xml').is_file() for p in licence_dirs()):
        problems.append('no existing local Unity licence; no launch')
    return problems


def licence_dirs():
    return [Path.home() / '.config/unity3d/Unity/licenses',
            Path.home() / '.var/app/com.unity.UnityHub/config/unity3d/Unity/licenses']


def main():
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    parser.add_argument('--project', type=Path, required=True, help='Unity lane, e.g. /media/jer/ZX20/CCF-U1')
    parser.add_argument('--expected-head', required=True, help='Exact clean lane commit to qualify')
    parser.add_argument('--lock-id', required=True, help='Already-held Unity lock ID')
    parser.add_argument('--out', type=Path, required=True, help='Fresh folder under ' + str(EVIDENCE_ROOT))
    parser.add_argument('--task-id', default=None, help='Optional: also require this task id in the lock metadata')
    parser.add_argument('--preflight-only', action='store_true', help='Validate ownership without staging or launching')
    parser.add_argument('--sizes', default='1280x720,1920x1080')
    parser.add_argument('--display', default=':84')
    parser.add_argument('--screen', default='2560x1440')
    parser.add_argument('--timeout', type=int, default=1500)
    parser.add_argument('--grace', type=int, default=60, help='Seconds to wait for a clean exit after the verdict before reaping our own Editor')
    args = parser.parse_args()
    root = args.project.resolve()
    out = args.out.resolve()

    problems = preflight(args, root, out)
    if problems:
        print('OWNERSHIP/PREFLIGHT REFUSED:\n  ' + '\n  '.join(problems), file=sys.stderr)
        return 2
    if args.preflight_only:
        print('PARITY-07 rendered-gate preflight PASS: ' + git(root, 'rev-parse', 'HEAD').strip())
        return 0

    out.mkdir(parents=True, exist_ok=True)
    evidence = out / 'evidence'
    evidence.mkdir()
    staged = root / 'Assets/ForestPrototype' / HARNESS
    meta = Path(str(staged) + '.meta')
    sources = [p for p in licence_dirs() if (p / 'UnityEntitlementLicense.xml').is_file()]
    licence_copy = out / 'config/unity3d/Unity/licenses'
    shutil.copytree(max(sources, key=lambda p: (p / 'UnityEntitlementLicense.xml').stat().st_mtime), licence_copy, dirs_exist_ok=True)
    env = dict(os.environ, XDG_CONFIG_HOME=str(out / 'config'), DISPLAY=args.display,
               CCF_DIAG_DIR=str(evidence), CCF_UI_SIZES=args.sizes)
    # Exfat (ZX20) cannot hold the licensing client socket; keep the trailing slash for the compiler prewarmer.
    env['TMPDIR'] = '/tmp/'
    for name in ('CCF_TEST_DIRECT_ENTRY', 'CCF_STAND_GEOMETRY', 'CCF_ACCEPTANCE_OUTPUT'):
        env.pop(name, None)
    command = [str(UNITY), '-projectPath', str(root), '-logFile', str(out / 'unity.log'), '-executeMethod', METHOD]
    (out / 'command.json').write_text(json.dumps({'command': command, 'root': str(root), 'head': args.expected_head,
                                                  'lock_id': args.lock_id, 'sizes': args.sizes}, indent=2) + '\n')
    (out / 'head.txt').write_text(git(root, 'rev-parse', 'HEAD'))

    processes, streams = [], []
    copied = reaped = False
    code = issue = None
    started = time.monotonic()

    def interrupt(signum, frame):
        raise InterruptedError('driver received signal ' + str(signum))

    def launch(cmd, name, process_env):
        stream = open(out / name, 'w')
        streams.append(stream)
        process = subprocess.Popen(cmd, cwd=root, env=process_env, stdin=subprocess.DEVNULL,
                                   stdout=stream, stderr=subprocess.STDOUT, start_new_session=True)
        processes.append(process)
        return process

    old_handler = signal.signal(signal.SIGTERM, interrupt)
    try:
        launch(['Xephyr', args.display, '-screen', args.screen, '-ac', '-noreset'], 'xephyr.log', os.environ)
        time.sleep(2)
        launch(['openbox'], 'openbox.log', env)
        time.sleep(1)
        shutil.copy2(HERE / HARNESS, staged)
        copied = True
        unity = launch(command, 'unity-stdout.log', env)
        (out / 'process.json').write_text(json.dumps({'driver_pid': os.getpid(), 'unity_pid': unity.pid}, indent=2) + '\n')
        finished_at = None
        while True:
            code = unity.poll()
            if code is not None:
                break
            if time.monotonic() - started > args.timeout:
                raise subprocess.TimeoutExpired(command, args.timeout)
            text = (out / 'unity.log').read_text(errors='replace') if (out / 'unity.log').exists() else ''
            if finished_at is None and (PASS_MARKER in text or FAIL_MARKER in text):
                finished_at = time.monotonic()
            # A verdict has been written; the Editor sometimes hangs after "Cleanup mono". Reap only our own process.
            if finished_at is not None and time.monotonic() - finished_at > args.grace and 'Cleanup mono' in text:
                (out / 'reaper.txt').write_text('reaping this run\'s own hung Editor after verdict and Cleanup mono; pid %d\n' % unity.pid)
                os.killpg(unity.pid, signal.SIGKILL)
                unity.wait(timeout=30)
                code, reaped = -9, True
                break
            time.sleep(2)
    except (subprocess.TimeoutExpired, InterruptedError, KeyboardInterrupt, OSError) as error:
        issue = str(error)
    finally:
        # Only processes created by this invocation are touched; each holds its own session.
        for process in reversed(processes):
            if process.poll() is None:
                process.terminate()
                try:
                    process.wait(timeout=30)
                except subprocess.TimeoutExpired:
                    process.kill()
                    process.wait(timeout=10)
        cleanup_failures = []
        if copied:
            staged.unlink(missing_ok=True)
            meta.unlink(missing_ok=True)
        try:
            shutil.rmtree(licence_copy)
        except FileNotFoundError:
            pass
        except OSError as error:
            cleanup_failures.append('copied licence removal failed: ' + str(error))
        if licence_copy.exists() and not cleanup_failures:
            cleanup_failures.append('copied licence directory remains after cleanup')
        for stream in streams:
            stream.close()
        signal.signal(signal.SIGTERM, old_handler)

        # Editor side effects: capture, then restore only the known rewrites from HEAD by exact path.
        porcelain = git(root, 'status', '--porcelain', '--untracked-files=all')
        (out / 'git-status-after.txt').write_text(porcelain)
        changed = [line[3:] for line in porcelain.splitlines() if line.strip()]
        unexpected = [p for p in changed if p not in KNOWN_SIDE_EFFECTS]
        if changed and not unexpected:
            (out / 'editor-side-effects-restored.patch').write_text(git(root, 'diff', '--binary'))
            subprocess.check_call(['git', 'checkout', '--', *changed], cwd=root)
        clean_after = not git(root, 'status', '--porcelain', '--untracked-files=all').strip()

        log = (out / 'unity.log').read_text(errors='replace') if (out / 'unity.log').exists() else ''
        lines = log.splitlines()
        failures = [l for l in lines if 'PARITY07_CHECK_FAIL' in l or re.search(r'\berror CS\d+', l)]
        (out / 'summary-lines.txt').write_text('\n'.join(l for l in lines if 'PARITY07_' in l or re.search(r'\berror CS\d+', l)) + '\n')
        pass_marker = PASS_MARKER in log and FAIL_MARKER not in log
        shots = sorted(p.name for p in evidence.glob('*.png'))
        result = {'unity_exit': code, 'reaped_after_verdict': reaped, 'driver_issue': issue,
                  'seconds': round(time.monotonic() - started, 2), 'pass_marker': pass_marker, 'failures': failures,
                  'screenshots': len(shots), 'harness_removed': not staged.exists() and not meta.exists(),
                  'licence_copy_removed': not licence_copy.exists(), 'cleanup_failures': cleanup_failures,
                  'unexpected_lane_changes': unexpected, 'lane_clean_after': clean_after}
        exit_ok = code == 0 or (reaped and pass_marker)
        result['pass'] = (exit_ok and issue is None and pass_marker and not failures and result['harness_removed']
                          and result['licence_copy_removed'] and not cleanup_failures and not unexpected and clean_after)
        (out / 'process-result.json').write_text(json.dumps(result, indent=2) + '\n')
        print(json.dumps(result, indent=2), flush=True)
    return 0 if result['pass'] else 1


if __name__ == '__main__':
    raise SystemExit(main())
