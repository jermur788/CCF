#!/usr/bin/env python3
"""Run the disposable S1-C T-03 rendered gate or unchanged protected T-02 gate under an already-held CCF Unity lock.

The project path is the Unity lane that holds the exact candidate (or baseline) commit: by default the checkout this script
lives in, or --project for a lane whose checkout does not contain the harness (the baseline commit). The harness bytes
are always copied from next to this script, so baseline and candidate runs use identical harness code.
It does NOT acquire the machine-wide lock: take it first with CCF-Ops/scripts/unity-lock and release it afterwards.

It preserves/restores shared VS Code settings and the three observed Forb material import side effects,
retaining their patches as evidence. Other lane changes cause a failed receipt.
It starts a private Xephyr display and openbox, copies the selected disposable harness into
Assets/ForestPrototype/, launches Unity with -executeMethod, and always removes the copied harness, its generated .meta
and the copied licence. Evidence (logs, review texts, screenshots, result summary) goes to --out, which must be outside
the repository (use /media/jer/Files/CCF/Verification/...).

--mode candidate (default) runs every check. --mode baseline runs the same fixture on the pre-change view and skips only
the checks that need the new behaviour, so the two evidence folders can be compared with compare_reviews.py.
"""
import argparse
import hashlib
import json
import os
from pathlib import Path
import shutil
import signal
import subprocess
import time

ROOT = Path(__file__).resolve().parents[3]   # default project: the checkout this script lives in
UNITY = Path('/media/jer/ZX20/Unity/6000.6.0f1/Editor/Unity')
HARNESS = 'AnnualReviewYearBrowsingVerification.cs'


def main():
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    parser.add_argument('--out', type=Path, required=True)
    parser.add_argument('--lock-id', required=True, help='Already-held S1-C-T03 Unity lock ID')
    parser.add_argument('--expected-head', required=True, help='Exact clean Unity-lane commit to qualify')
    parser.add_argument('--preflight-only', action='store_true', help='Validate ownership without staging or launching')
    parser.add_argument('--project', type=Path, default=ROOT, help='Unity project (lane) to run in')
    parser.add_argument('--gate', choices=['t02', 't03'], default='t03')
    parser.add_argument('--mode', choices=['candidate', 'baseline'], default='candidate')
    parser.add_argument('--sizes', default='1280x720,1920x1080')
    parser.add_argument('--display', default=':84')
    parser.add_argument('--screen', default='2560x1440')
    parser.add_argument('--timeout', type=int, default=1500)
    args = parser.parse_args()
    harness = HARNESS if args.gate == 't02' else 'AnnualReviewDrillDownVerification.cs'
    entry = 'AnnualReviewYearBrowsingVerification.Begin' if args.gate == 't02' else 'AnnualReviewDrillDownVerification.Begin'
    marker = 'S1C_T02_' if args.gate == 't02' else 'S1C_T03_'
    root = args.project.resolve()
    out = args.out.resolve()
    # A released lock is not a lane reservation. Refuse another task's lock, checkout or staged harness before writes.
    lock = json.loads(subprocess.check_output(
        ['python3', '-B', '/media/jer/ZX20/CCF-Ops/scripts/unity-lock', 'status', '--json'], text=True))
    metadata = lock.get('metadata') or {}
    actual_head = subprocess.check_output(['git', '-C', str(root), 'rev-parse', 'HEAD'], text=True).strip()
    dirty = subprocess.check_output(['git', '-C', str(root), 'status', '--porcelain'], text=True).strip()
    if (lock.get('status') != 'held' or metadata.get('lock_id') != args.lock_id
            or metadata.get('task_id') != 'S1-C-T03' or metadata.get('owner') != 'Sol-S1C-T03' or metadata.get('lane') != str(root)
            or metadata.get('head') != args.expected_head or actual_head != args.expected_head
            or lock.get('unity_editor_pids') or dirty):
        raise SystemExit('OWNERSHIP CONFLICT: require this task\'s exact lock, a clean matching lane and no running Editor')
    if args.preflight_only:
        print('S1-C T-03 ownership preflight PASS: ' + actual_head)
        return 0
    if root in out.parents or out == root:
        raise SystemExit('Evidence folder must be outside the repository')
    out.mkdir(parents=True, exist_ok=True)
    evidence = out / 'evidence'
    evidence.mkdir(exist_ok=True)
    staged = root / 'Assets/ForestPrototype' / harness
    meta = Path(str(staged) + '.meta')
    if staged.exists() or meta.exists():
        raise SystemExit('Existing staged harness/meta: inspect ownership before running')
    display_number = args.display.lstrip(':').split('.')[0]
    if Path('/tmp/.X11-unix/X' + display_number).exists() or Path('/tmp/.X' + display_number + '-lock').exists():
        raise SystemExit('Requested private display is already occupied')
    licences = [Path.home() / '.config/unity3d/Unity/licenses',
                Path.home() / '.var/app/com.unity.UnityHub/config/unity3d/Unity/licenses']
    sources = [p for p in licences if (p / 'UnityEntitlementLicense.xml').is_file()]
    if not sources:
        raise SystemExit('No existing local Unity licence; no launch')
    licence_copy = out / 'config/unity3d/Unity/licenses'
    shutil.copytree(max(sources, key=lambda p: (p / 'UnityEntitlementLicense.xml').stat().st_mtime), licence_copy, dirs_exist_ok=True)
    env = dict(os.environ, XDG_CONFIG_HOME=str(out / 'config'), DISPLAY=args.display,
               CCF_DIAG_DIR=str(evidence), CCF_UI_SIZES=args.sizes, CCF_T02_MODE=args.mode)
    # The Unity licensing client needs a socket in TMPDIR; exFAT (ZX20) cannot hold one, so an inherited
    # TMPDIR=/media/jer/ZX20/... stalls the Editor at startup with "waiting for channel: LicenseClient".
    # Keep the trailing slash: the Editor's compiler prewarmer concatenates the directory and filename.
    env['TMPDIR'] = '/tmp/'
    env.pop('CCF_TEST_DIRECT_ENTRY', None)
    env.pop('CCF_STAND_GEOMETRY', None)   # production default geometry; never an inherited override
    command = [str(UNITY), '-projectPath', str(root), '-logFile', str(out / 'unity.log'),
               '-executeMethod', entry]
    (out / 'command.json').write_text(json.dumps({'command': command, 'mode': args.mode, 'root': str(root), 'expected_head': args.expected_head,
        'gate': args.gate, 'lock_id': args.lock_id, 'harness': harness,
        'harness_sha256': hashlib.sha256(Path(__file__).with_name(harness).read_bytes()).hexdigest()}, indent=2) + '\n')
    processes, streams = [], []
    # Unity's code-editor integration rewrites shared VS Code settings on import.
    # Preflight required a clean lane; retain the exact bytes and restore only this known side effect.
    settings = root / '.vscode/settings.json'
    settings_before = settings.read_bytes() if settings.exists() else None
    import_materials = [root / 'Assets/ForestPrototype/Art/SectionFive/Woodland_Forbs_01/Materials' / (name + '.mat')
                        for name in ('Forb_Flower_Centre', 'Forb_Leaves', 'Forb_Stems')]
    material_bytes = {p: p.read_bytes() for p in import_materials}
    copied = False
    code = issue = None
    cleanup_failures = []
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
        shutil.copy2(Path(__file__).with_name(harness), staged)
        copied = True
        unity = launch(command, 'unity-stdout.log', env)
        (out / 'process.json').write_text(json.dumps({'driver_pid': os.getpid(), 'unity_pid': unity.pid}, indent=2) + '\n')
        code = unity.wait(timeout=args.timeout)
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
        diff = subprocess.run(['git', '-C', str(root), 'diff', '--', '.vscode/settings.json'], capture_output=True, text=True, check=True).stdout
        (out / 'editor-side-effects.patch').write_text(diff)
        if settings_before is not None:
            settings.write_bytes(settings_before)
        elif settings.exists():
            settings.unlink()
        (out / 'material-import-side-effects.patch').write_text(subprocess.check_output(
            ['git', '-C', str(root), 'diff', '--', *[str(p.relative_to(root)) for p in import_materials]], text=True))
        for path, content in material_bytes.items():
            path.write_bytes(content)
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
        log = (out / 'unity.log').read_text(errors='replace') if (out / 'unity.log').exists() else ''
        pass_marker = marker + 'HARNESS_PASS' in log
        failures = [line for line in log.splitlines() if marker + 'CHECK_FAIL' in line or ' error CS' in line]
        result = {'gate': args.gate, 'mode': args.mode, 'unity_exit': code, 'driver_issue': issue,
                  'seconds': round(time.monotonic() - started, 2), 'pass_marker': pass_marker, 'failures': failures,
                  'harness_removed': not staged.exists() and not meta.exists(),
                  'licence_copy_removed': not licence_copy.exists(), 'cleanup_failures': cleanup_failures}
        remaining = subprocess.check_output(['git', '-C', str(root), 'status', '--porcelain'], text=True).strip()
        result['remaining_lane_changes'] = remaining
        result['disk_saves_created'] = [str(p.relative_to(out)) for p in (out / 'config').rglob('forest-save.json*')]
        result['pass'] = (not remaining and not result['disk_saves_created'] and code == 0 and issue is None and pass_marker and not failures
                          and result['harness_removed'] and result['licence_copy_removed'] and not cleanup_failures)
        (out / 'process-result.json').write_text(json.dumps(result, indent=2) + '\n')
        print(json.dumps(result, indent=2), flush=True)
    return 0 if result['pass'] else 1


if __name__ == '__main__':
    raise SystemExit(main())
