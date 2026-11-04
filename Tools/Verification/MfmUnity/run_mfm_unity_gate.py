#!/usr/bin/env python3
"""Disposable MFM-04/05 Unity gate. Caller owns the machine-wide lock; never acquires it."""
import argparse
import hashlib
import json
import os
from pathlib import Path
import re
import shutil
import signal
import subprocess
import time

ROOT = Path(__file__).resolve().parents[3]
UNITY = Path('/media/jer/ZX20/Unity/6000.6.0f1/Editor/Unity')
U1 = Path('/media/jer/ZX20/CCF-U1')
EVIDENCE = Path('/media/jer/Files/CCF/Verification')
LOCK = '/media/jer/ZX20/CCF-Ops/scripts/unity-lock'


def git(root, *args):
    return subprocess.check_output(['git', '-C', str(root), *args], text=True).strip()


def digest(path):
    return hashlib.sha256(path.read_bytes()).hexdigest() if path.is_file() else None


def ownership(root, expected, lock_id):
    state = json.loads(subprocess.check_output(['python3', '-B', LOCK, 'status', '--json'], text=True))
    meta = state.get('metadata') or {}
    if (state.get('status') != 'held' or meta.get('lock_id') != lock_id
            or meta.get('lane') != str(root) or meta.get('head') != expected
            or meta.get('task_id') != 'MFM-04-05' or not meta.get('owner')
            or state.get('unity_editor_pids') or git(root, 'rev-parse', 'HEAD') != expected
            or git(root, 'status', '--porcelain', '--untracked-files=all')):
        raise SystemExit('OWNERSHIP CONFLICT: require exact held MFM-04-05 lock/lane/HEAD, clean lane and no Editor')


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--project', type=Path, required=True)
    parser.add_argument('--expected-head', required=True)
    parser.add_argument('--lock-id', required=True)
    parser.add_argument('--out', type=Path, required=True)
    parser.add_argument('--preflight-only', action='store_true')
    parser.add_argument('--display', default=':86')
    parser.add_argument('--timeout', type=int, default=1200)
    args = parser.parse_args()
    root, out = args.project.resolve(), args.out.resolve()
    if root != U1.resolve() or not re.fullmatch(r'[0-9a-f]{40}', args.expected_head):
        raise SystemExit('Require U1 and a full exact 40-character HEAD')
    if not out.is_relative_to(EVIDENCE.resolve()) or out == EVIDENCE.resolve() or root == out or root in out.parents:
        raise SystemExit('Use a dedicated evidence directory under Files/CCF/Verification')
    if not re.fullmatch(r':\d+', args.display) or args.timeout <= 0:
        raise SystemExit('Require a private :NUMBER display and positive timeout')
    ownership(root, args.expected_head, args.lock_id)
    if (root / 'ProjectSettings/ProjectVersion.txt').read_text().splitlines()[0] != 'm_EditorVersion: 6000.6.0f1':
        raise SystemExit('Project is not Unity 6000.6.0f1')
    if shutil.disk_usage(root).free < 15 * 1024**3:
        raise SystemExit('U1 requires at least 15 GiB free before Unity launch')
    source = ROOT / 'Tools/Verification/MfmUnity/MfmUnityCompileCheck.cs'
    # Runner and harness must be the committed versions on the candidate, even when invoked from another checkout.
    for path in (source, Path(__file__).resolve()):
        relative = str(path.relative_to(ROOT))
        committed = subprocess.check_output(['git', '-C', str(root), 'show', 'HEAD:' + relative])
        if committed != path.read_bytes():
            raise SystemExit('Runner/harness differs from clean candidate: ' + relative)
    if args.preflight_only:
        print('MFM ownership/source preflight PASS ' + args.expected_head)
        return 0
    if out.exists() and any(out.iterdir()):
        raise SystemExit('Evidence directory must be unused')
    staged = root / 'Assets/ForestPrototype/MfmUnityCompileCheck.cs'
    meta = Path(str(staged) + '.meta')
    if staged.exists() or meta.exists():
        raise SystemExit('Staged harness/meta already exists')
    number = args.display[1:]
    if Path('/tmp/.X11-unix/X' + number).exists() or Path('/tmp/.X' + number + '-lock').exists():
        raise SystemExit('Private display is occupied')
    for executable in ('Xephyr', 'openbox'):
        if shutil.which(executable) is None:
            raise SystemExit('Missing ' + executable)
    if not UNITY.is_file():
        raise SystemExit('Missing approved Unity Editor')
    licences = [Path.home() / '.config/unity3d/Unity/licenses',
                Path.home() / '.var/app/com.unity.UnityHub/config/unity3d/Unity/licenses']
    licences = [p for p in licences if (p / 'UnityEntitlementLicense.xml').is_file()]
    if not licences:
        raise SystemExit('No existing Unity licence; no launch')
    # No other checkout is changed. Preserve only documented import side effects, never arbitrary dirty files.
    known = [root / '.vscode/settings.json'] + sorted((root / 'Assets/ForestPrototype/Art/SectionFive').glob('*/Materials/*.mat'))
    originals = {p: p.read_bytes() if p.exists() else None for p in known}
    host_dir = Path.home() / '.config/unity3d/DefaultCompany/CCF'
    host_saves = {host_dir / n: digest(host_dir / n) for n in ('forest-save.json', 'forest-save.json.bak', 'forest-save.json.tmp')}
    out.mkdir(parents=True, exist_ok=True)
    evidence = out / 'evidence'
    evidence.mkdir()
    licence_copy = out / 'config/unity3d/Unity/licenses'
    env = dict(os.environ)
    # Remove all inherited scenario/harness overrides, then opt into the fresh production world.
    for key in list(env):
        if key.startswith('CCF_'):
            del env[key]
    env.update(XDG_CONFIG_HOME=str(out / 'config'), DISPLAY=args.display, TMPDIR='/tmp/',
               CCF_DIAG_DIR=str(evidence), CCF_TEST_DIRECT_ENTRY='1')
    command = [str(UNITY), '-projectPath', str(root), '-logFile', str(out / 'unity.log'),
               '-executeMethod', 'MfmUnityCompileCheck.Begin']
    def write(name, value):
        (out / name).write_text(json.dumps(value, indent=2) + '\n')
    write('command.json', dict(command=command, expected_head=args.expected_head, lock_id=args.lock_id,
          task_id='MFM-04-05', source_sha256=digest(source), project=str(root)))
    write('host-saves-before.json', {str(p): h for p, h in host_saves.items()})
    processes, streams, cleanup_errors = [], [], []
    staged_owned = False
    code = issue = None
    started = time.monotonic()

    def launch(cmd, name, process_env):
        stream = open(out / name, 'w')
        streams.append(stream)
        proc = subprocess.Popen(cmd, cwd=root, env=process_env, stdin=subprocess.DEVNULL,
                                stdout=stream, stderr=subprocess.STDOUT, start_new_session=True)
        processes.append(proc)
        return proc

    def interrupted(signum, frame):
        raise InterruptedError('signal ' + str(signum))

    previous = signal.signal(signal.SIGTERM, interrupted)
    try:
        # Recheck immediately before any staging or Editor launch. The caller must retain the lock until return.
        ownership(root, args.expected_head, args.lock_id)
        shutil.copytree(max(licences, key=lambda p: (p / 'UnityEntitlementLicense.xml').stat().st_mtime), licence_copy)
        display = launch(['Xephyr', args.display, '-screen', '1280x720', '-ac', '-noreset'], 'xephyr.log', os.environ)
        time.sleep(2)
        if display.poll() is not None:
            raise RuntimeError('Private display failed to start')
        launch(['openbox'], 'openbox.log', env)
        with staged.open('xb') as target:
            staged_owned = True
            target.write(source.read_bytes())
        editor = launch(command, 'unity-stdout.log', env)
        write('process.json', dict(driver_pid=os.getpid(), unity_pid=editor.pid))
        code = editor.wait(timeout=args.timeout)
    except (subprocess.TimeoutExpired, InterruptedError, KeyboardInterrupt, OSError, RuntimeError) as error:
        issue = str(error)
    finally:
        # Terminate only sessions launched here, including their compiler/WM descendants.
        for proc in reversed(processes):
            try:
                os.killpg(proc.pid, signal.SIGTERM)
                if proc.poll() is None:
                    proc.wait(timeout=10)
            except ProcessLookupError:
                pass
            except subprocess.TimeoutExpired:
                os.killpg(proc.pid, signal.SIGKILL)
                proc.wait(timeout=10)
        for path in ((staged, meta) if staged_owned else ()):
            try:
                path.unlink(missing_ok=True)  # also handles a partial copy
            except OSError as error:
                cleanup_errors.append(str(error))
        try:
            patch = subprocess.check_output(['git', '-C', str(root), 'diff', '--', *[str(p.relative_to(root)) for p in known]], text=True)
            (out / 'editor-import-side-effects.patch').write_text(patch)
        except (OSError, subprocess.CalledProcessError) as error:
            cleanup_errors.append(str(error))
        for path, original in originals.items():
            try:
                if original is None:
                    path.unlink(missing_ok=True)
                else:
                    path.write_bytes(original)
            except OSError as error:
                cleanup_errors.append(str(error))
        try:
            if licence_copy.exists():
                shutil.rmtree(licence_copy)
        except OSError as error:
            cleanup_errors.append(str(error))
        for stream in streams:
            stream.close()
        signal.signal(signal.SIGTERM, previous)
        log = (out / 'unity.log').read_text(errors='replace') if (out / 'unity.log').exists() else ''
        failures = [line for line in log.splitlines() if re.search(r'MFM_UNITY_FAIL|error CS\d+|Scripts have compiler errors|Compilation failed|Exception:', line)]
        remaining = git(root, 'status', '--porcelain', '--untracked-files=all')
        disk_saves = [str(p.relative_to(out)) for p in (out / 'config').rglob('forest-save.json*')]
        unchanged = all(digest(p) == h for p, h in host_saves.items())
        result = dict(unity_exit=code, driver_issue=issue, seconds=round(time.monotonic() - started, 2),
                      pass_marker='MFM_UNITY_PASS' in log, failures=failures,
                      harness_removed=not staged.exists() and not meta.exists(),
                      licence_copy_removed=not licence_copy.exists(), cleanup_errors=cleanup_errors,
                      remaining_lane_changes=remaining, host_saves_unchanged=unchanged, disk_saves_created=disk_saves)
        result['pass'] = (code == 0 and issue is None and result['pass_marker'] and not failures
                          and result['harness_removed'] and result['licence_copy_removed'] and not cleanup_errors
                          and not remaining and unchanged and not disk_saves)
        write('process-result.json', result)
        print(json.dumps(result, indent=2), flush=True)
    return 0 if result['pass'] else 1


if __name__ == '__main__':
    raise SystemExit(main())
