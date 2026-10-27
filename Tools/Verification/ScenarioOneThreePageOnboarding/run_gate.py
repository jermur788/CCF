#!/usr/bin/env python3
"""Run the disposable onboarding render/flow gate under an already-held CCF Unity lock.

Each owned process has a separate session. A child's shutdown signal cannot kill
this driver before its harness/config cleanup and exit receipt are written.
"""
import argparse
import json
import os
from pathlib import Path
import shutil
import signal
import subprocess
import time

ROOT = Path(__file__).resolve().parents[3]
UNITY = Path('/media/jer/ZX20/Unity/6000.6.0f1/Editor/Unity')


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--out', type=Path, required=True)
    parser.add_argument('--sizes', default='1280x720,1920x1080')
    parser.add_argument('--display', default=':84')
    parser.add_argument('--screen', default='2560x1440')
    parser.add_argument('--timeout', type=int, default=1200)
    args = parser.parse_args()
    out = args.out.resolve()
    out.mkdir(parents=True, exist_ok=True)
    evidence = out / 'evidence'
    evidence.mkdir(exist_ok=True)
    staged = ROOT / 'Assets/ForestPrototype/OnboardingVerification.cs'
    meta = Path(str(staged) + '.meta')
    if staged.exists() or meta.exists():
        raise SystemExit('Existing staged harness/meta: inspect ownership before running')
    display_number = args.display.lstrip(':').split('.')[0]
    if Path('/tmp/.X11-unix/X' + display_number).exists() or Path('/tmp/.X' + display_number + '-lock').exists():
        raise SystemExit('Requested private display is already occupied')
    licences = [Path.home()/'.config/unity3d/Unity/licenses',
                Path.home()/'.var/app/com.unity.UnityHub/config/unity3d/Unity/licenses']
    sources = [p for p in licences if (p/'UnityEntitlementLicense.xml').is_file()]
    if not sources:
        raise SystemExit('No existing local Unity licence; no launch')
    licence_copy = out / 'config/unity3d/Unity/licenses'
    shutil.copytree(max(sources, key=lambda p: (p/'UnityEntitlementLicense.xml').stat().st_mtime), licence_copy, dirs_exist_ok=True)
    env = dict(os.environ, XDG_CONFIG_HOME=str(out/'config'), DISPLAY=args.display,
               CCF_DIAG_DIR=str(evidence), CCF_ACCEPTANCE_OUTPUT=str(evidence), CCF_UI_SIZES=args.sizes)
    env.pop('CCF_TEST_DIRECT_ENTRY', None)
    command = [str(UNITY), '-projectPath', str(ROOT), '-logFile', str(out/'unity.log'),
               '-executeMethod', 'OnboardingVerification.Begin']
    (out/'command.json').write_text(json.dumps(command, indent=2)+'\n')
    processes = []
    streams = []
    copied = False
    code = None
    issue = None
    cleanup_failures = []
    started = time.monotonic()

    def interrupt(signum, frame):
        raise InterruptedError('driver received signal ' + str(signum))

    def launch(cmd, name, process_env):
        stream = open(out/name, 'w')
        streams.append(stream)
        process = subprocess.Popen(cmd, cwd=ROOT, env=process_env, stdin=subprocess.DEVNULL,
                                   stdout=stream, stderr=subprocess.STDOUT, start_new_session=True)
        processes.append(process)
        return process

    old_handler = signal.signal(signal.SIGTERM, interrupt)
    try:
        launch(['Xephyr', args.display, '-screen', args.screen, '-ac', '-noreset'], 'xephyr.log', os.environ)
        time.sleep(2)
        launch(['openbox'], 'openbox.log', env)
        time.sleep(1)
        shutil.copy2(Path(__file__).with_name('OnboardingVerification.cs'), staged)
        copied = True
        unity = launch(command, 'unity-stdout.log', env)
        (out/'process.json').write_text(json.dumps({'driver_pid':os.getpid(), 'unity_pid':unity.pid}, indent=2)+'\n')
        code = unity.wait(timeout=args.timeout)
    except (subprocess.TimeoutExpired, InterruptedError, KeyboardInterrupt, OSError) as error:
        issue = str(error)
    finally:
        # Only processes created by this invocation are touched; all hold their own sessions.
        for process in reversed(processes):
            if process.poll() is None:
                process.terminate()
                try:
                    process.wait(timeout=30)
                except subprocess.TimeoutExpired:
                    process.kill()
                    process.wait(timeout=10)
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
        log = (out/'unity.log').read_text(errors='replace') if (out/'unity.log').exists() else ''
        pass_marker = 'ONBOARDING_PASS' in log
        failures = [line for line in log.splitlines() if 'ONBOARDING_CHECK_FAIL' in line or 'error CS' in line]
        result = {'unity_exit':code, 'driver_issue':issue, 'seconds':round(time.monotonic()-started,2),
                  'pass_marker':pass_marker, 'failures':failures, 'harness_removed':not staged.exists() and not meta.exists(),
                  'licence_copy_removed':not licence_copy.exists(), 'cleanup_failures':cleanup_failures}
        result['pass'] = code == 0 and issue is None and pass_marker and not failures and result['harness_removed'] and result['licence_copy_removed'] and not cleanup_failures
        (out/'process-result.json').write_text(json.dumps(result, indent=2)+'\n')
        print(json.dumps(result, indent=2), flush=True)
    return 0 if result['pass'] else 1


if __name__ == '__main__':
    raise SystemExit(main())
