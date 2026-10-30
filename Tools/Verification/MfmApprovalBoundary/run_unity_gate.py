#!/usr/bin/env python3
"""MFM-07 (prescription approval boundary) Unity gates runner. A copy of the reviewed MFM-06 runner (MfmSaveProvenanceUnity/run_gate.py) whose
only changes are the added "approval" gate, the default task id and the allowance for its isolated disk round trip.
Requires exact clean U1 HEAD and this task's lock.
Stages inspected harnesses, uses isolated config/private display, restores only known import
side effects and removes all staged source/metas and copied licence. No production edits.
Historical gates explicitly load valid rack-model-0 fixtures before their Start.
Output must be on Files.
"""
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

ROOT = Path(__file__).resolve().parents[3]   # default project: the checkout this script lives in
UNITY = Path('/media/jer/ZX20/Unity/6000.6.0f1/Editor/Unity')
GATES = {
    'teaching-latch': ('TeachingPersistence/PersistenceUnityVerification.cs', 'TEACHING_LATCH_UNITY_PASS', 'TEACHING_LATCH_UNITY_FAIL'),
    'reference': ('ScenarioReferenceVerification.cs', 'REFERENCE_FUTURE_V1_PASS', 'REFERENCE_FUTURE_V1_FAIL'),
    'hardening': ('SaveHardeningVerification.cs', 'SAVE_HARDENING_VERIFY_PASS', 'SAVE_HARDENING_VERIFY_FAIL'),
    'progress': ('ScenarioOneProgressVerification.cs', 'SCENARIO_ONE_PROGRESS_VERIFY_PASS', 'SCENARIO_ONE_PROGRESS_VERIFY_FAIL'),
    'completion': ('ScenarioOneCompletionVerification.cs', 'SCENARIO_ONE_COMPLETION_VERIFY_PASS', 'SCENARIO_ONE_COMPLETION_VERIFY_FAIL'),
    'completion-model1': ('ScenarioOneCompletionVerification.cs', 'SCENARIO_ONE_COMPLETION_VERIFY_PASS', 'SCENARIO_ONE_COMPLETION_VERIFY_FAIL'),
    'n3': ('ScenarioOneCompletion/N3/N3CompletionVerification.cs', 'N3_U4_VERIFY_PASS', 'N3_U4_VERIFY_FAIL'),
    'density-save': ('ThreeSpecies/Qualification/DensityQualification.cs', 'DENSITY_QUAL_PASS', 'DENSITY_QUAL_FAIL'),
    'interaction': ('S1CObjectiveHistoryUnity/S1CInteractionGate.cs', 'SCENARIO_ONE_INTERACTION_VERIFY_PASS', 'SCENARIO_ONE_INTERACTION_VERIFY_FAIL'),
    'compile': ('MfmSaveProvenanceUnity/MfmCompileGate.cs', 'MFM06_COMPILE_PASS', 'error CS'),
    'hash-dump': ('MfmSaveProvenanceUnity/MfmHashDumpGate.cs', 'MFM06_HASHDUMP_PASS', 'MFM06_HASHDUMP_FAIL'),
    'save': ('MfmSaveProvenanceUnity/MfmSaveProvenanceUnityCheck.cs', 'MFM06_UNITY_PASS', 'MFM06_UNITY_CHECK_FAIL'),
    'approval': ('MfmApprovalBoundary/MfmApprovalBoundaryUnityGate.cs', 'MFM07_UNITY_PASS', 'MFM07_UNITY_FAIL'),
    'coverage-world': ('MfmCoverageWorld/MfmCoverageWorldUnityGate.cs', 'MFM08A_UNITY_PASS', 'MFM08A_UNITY_FAIL'),
}

# Gates that deliberately load rack-model-0 fixtures and must not use production geometry defaults.
LEGACY_GATES = ('reference', 'progress', 'completion', 'completion-model1', 'interaction', 'density-save')
# Gates whose harness runs the shared TeachingCapGateSetup companion from the historical gate suite.
HISTORICAL_COMPANION_GATES = ('teaching-latch', 'reference', 'hardening', 'progress', 'completion', 'completion-model1', 'n3', 'density-save', 'interaction')


def main():
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    parser.add_argument('--out', type=Path, required=True)
    parser.add_argument('--lock-id', required=True, help='Already-held Unity lock ID for this task')
    parser.add_argument('--task-id', default='MFM-07', help='Lock metadata task_id this run must own')
    parser.add_argument('--owner', required=True, help="Lock metadata owner this run must own, e.g. 'T3-MFM06'")
    parser.add_argument('--expected-head', required=True, help='Exact clean Unity-lane commit to qualify')
    parser.add_argument('--preflight-only', action='store_true', help='Validate ownership without staging or launching')
    parser.add_argument('--project', type=Path, default=ROOT, help='Unity project (lane) to run in')
    parser.add_argument('--gate', choices=sorted(GATES), default='save')
    parser.add_argument('--mode', choices=['candidate', 'baseline'], default='candidate')
    parser.add_argument('--sizes', default='1280x720,1920x1080')
    parser.add_argument('--display', default=':84')
    parser.add_argument('--screen', default='2560x1440')
    parser.add_argument('--timeout', type=int, default=1500)
    parser.add_argument('--skip-compile-receipt', action='store_true',
                        help='Bypass the sibling 01-compile receipt check; requires --compile-receipt-note')
    parser.add_argument('--compile-receipt-note', help='Reason recorded in command.json when the compile receipt is skipped')
    args = parser.parse_args()
    relative, pass_marker, fail_marker = GATES[args.gate]
    source = ROOT / 'Tools/Verification' / relative
    harness = source.name
    entry = source.stem + '.Begin'
    root = args.project.resolve()
    out = args.out.resolve()
    if root != Path('/media/jer/ZX20/CCF-U1'):
        raise SystemExit('This task authorises Unity only in U1')
    if args.skip_compile_receipt:
        if not args.compile_receipt_note:
            raise SystemExit('--skip-compile-receipt requires --compile-receipt-note')
    elif args.gate != 'compile':
        receipt = out.parent / '01-compile/process-result.json'
        if not receipt.is_file():
            raise SystemExit('Run the compile gate first at 01-compile')
        compiled = json.loads(receipt.read_text())
        if not compiled.get('pass') or compiled.get('head') != args.expected_head or compiled.get('lock_id') != args.lock_id:
            raise SystemExit('Compilation has not passed for this head and lock')
    # A released lock is not a lane reservation. Refuse another task's lock, checkout or staged harness before writes.
    lock = json.loads(subprocess.check_output(
        ['python3', '-B', '/media/jer/ZX20/CCF-Ops/scripts/unity-lock', 'status', '--json'], text=True))
    metadata = lock.get('metadata') or {}
    actual_head = subprocess.check_output(['git', '-C', str(root), 'rev-parse', 'HEAD'], text=True).strip()
    dirty = subprocess.check_output(['git', '-C', str(root), 'status', '--porcelain'], text=True).strip()
    if (lock.get('status') != 'held' or metadata.get('lock_id') != args.lock_id
            or metadata.get('task_id') != args.task_id or metadata.get('owner') != args.owner or metadata.get('lane') != str(root)
            or metadata.get('head') != args.expected_head or actual_head != args.expected_head
            or lock.get('unity_editor_pids') or dirty):
        raise SystemExit('OWNERSHIP CONFLICT: require this task\'s exact lock (task_id ' + args.task_id + ', owner ' + args.owner
                         + '), a clean matching lane and no running Editor')
    if args.preflight_only:
        print(args.task_id + ' (' + args.owner + ') ownership preflight PASS: ' + actual_head)
        return 0
    if root in out.parents or out == root:
        raise SystemExit('Evidence folder must be outside the repository')
    if out.exists() and any(out.iterdir()):
        raise SystemExit('Choose an unused evidence directory')
    if not out.is_relative_to(Path('/media/jer/Files/CCF/Verification')):
        raise SystemExit('Durable evidence must be under Files/CCF/Verification')
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
    companions = []
    if args.gate in HISTORICAL_COMPANION_GATES:
        companions.append(ROOT / 'Tools/Verification/ScenarioOneRepeatedInterventions/TeachingCapGateSetup.cs')
    if args.gate == 'teaching-latch':
        companions.append(ROOT / 'Tools/Verification/TeachingPersistence/PersistenceHashFixtures.cs')
    if args.gate == 'hash-dump':
        companions.append(ROOT / 'Tools/Verification/MfmSaveProvenance/HashFixtures.cs')
    pairs = [(source, staged)] + [(p, root / 'Assets/ForestPrototype' / p.name) for p in companions]
    for _, target in pairs:
        if target.exists() or Path(str(target) + '.meta').exists():
            raise SystemExit('Existing harness: ' + str(target))
    if any(not origin.is_file() for origin, _ in pairs):
        raise SystemExit('Missing verification source; nothing staged')
    licences = [Path.home() / '.config/unity3d/Unity/licenses',
                Path.home() / '.var/app/com.unity.UnityHub/config/unity3d/Unity/licenses']
    sources = [p for p in licences if (p / 'UnityEntitlementLicense.xml').is_file()]
    if not sources:
        raise SystemExit('No existing local Unity licence; no launch')
    licence_copy = out / 'config/unity3d/Unity/licenses'
    shutil.copytree(max(sources, key=lambda p: (p / 'UnityEntitlementLicense.xml').stat().st_mtime), licence_copy, dirs_exist_ok=True)
    env = dict(os.environ, XDG_CONFIG_HOME=str(out / 'config'), DISPLAY=args.display,
               CCF_DIAG_DIR=str(evidence), CCF_UI_SIZES=args.sizes, CCF_ACCEPTANCE_OUTPUT=str(evidence), CCF_T02_MODE=args.mode, CCF_T03_LEGACY='1' if args.gate == 'teaching-latch' else '0')
    # The Unity licensing client needs a socket in TMPDIR; exFAT (ZX20) cannot hold one, so an inherited
    # TMPDIR=/media/jer/ZX20/... stalls the Editor at startup with "waiting for channel: LicenseClient".
    # Keep the trailing slash: the Editor's compiler prewarmer concatenates the directory and filename.
    for key in list(env):
        if key.startswith('CCF_') and key not in ('CCF_DIAG_DIR', 'CCF_UI_SIZES', 'CCF_ACCEPTANCE_OUTPUT', 'CCF_T02_MODE', 'CCF_T03_LEGACY'):
            env.pop(key)
    env['TMPDIR'] = '/tmp/'
    env['CCF_TEST_DIRECT_ENTRY'] = '1'  # these gameplay harnesses require a live new world, not the startup title
    for key in ('CCF_STARTUP_SCREEN', 'CCF_STARTUP_INPUT', 'CCF_STARTUP_BOUNDARY', 'CCF_RACK_VERIFY_MODEL', 'CCF_GROWTH_MODEL'):
        env.pop(key, None)
    env.pop('CCF_STAND_GEOMETRY', None)   # production default geometry; never an inherited override
    if args.gate in LEGACY_GATES:
        env['CCF_STAND_GEOMETRY'] = '0'
        env['CCF_T03_LEGACY'] = '1'
    if args.gate == 'completion-model1':
        env['CCF_GROWTH_MODEL'] = '1'
    if args.gate == 'density-save':
        env['CCF_QUAL_PHASES'] = 'ANCHORS,SAVELOAD'
    command = [str(UNITY), '-projectPath', str(root), '-logFile', str(out / 'unity.log'),
               '-executeMethod', entry]
    (out / 'command.json').write_text(json.dumps({'command': command, 'mode': args.mode, 'root': str(root), 'expected_head': args.expected_head,
        'gate': args.gate, 'lock_id': args.lock_id, 'task_id': args.task_id, 'owner': args.owner, 'harness': harness,
        'compile_receipt_note': args.compile_receipt_note,
        'harness_sha256': hashlib.sha256(source.read_bytes()).hexdigest(),
        'runner_sha256': hashlib.sha256(Path(__file__).read_bytes()).hexdigest(),
        'verification_environment': {k: v for k, v in env.items() if k.startswith('CCF_') or k in ('XDG_CONFIG_HOME', 'DISPLAY', 'TMPDIR')}}, indent=2) + '\n')
    processes, streams = [], []
    # Unity's code-editor integration rewrites shared VS Code settings on import.
    # Preflight required a clean lane; retain the exact bytes and restore only this known side effect.
    settings = root / '.vscode/settings.json'
    settings_before = settings.read_bytes() if settings.exists() else None
    # Rendered scenes make Unity rewrite shared SectionFive material imports (texture refs, alpha-to-mask); a known import side effect.
    # Snapshot every SectionFive material so the diff is retained as evidence and the original bytes are restored.
    import_materials = sorted((root / 'Assets/ForestPrototype/Art/SectionFive').glob('*/Materials/*.mat'))
    material_bytes = {p: p.read_bytes() for p in import_materials}
    host_saves = [Path.home() / '.config/unity3d/DefaultCompany/CCF' / n for n in ('forest-save.json', 'forest-save.json.bak', 'forest-save.json.tmp')]
    def digest(p):
        return hashlib.sha256(p.read_bytes()).hexdigest() if p.exists() else None
    save_hashes = {str(p): digest(p) for p in host_saves}
    (out / 'host-saves-before.json').write_text(json.dumps(save_hashes, indent=2) + '\n')
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
        copied = True
        for origin, target in pairs:
            shutil.copy2(origin, target)
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
            for _, target in pairs:
                target.unlink(missing_ok=True)
                Path(str(target) + '.meta').unlink(missing_ok=True)
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
        has_pass = pass_marker in log
        failures = [line for line in log.splitlines() if fail_marker in line or ' error CS' in line]
        hash_dump_lines = []
        if args.gate == 'hash-dump':
            hash_dump_lines = [line for line in log.splitlines() if re.match(r'^\s*MFM06_HASH ', line)]
            (out / 'hash-dump.txt').write_text('\n'.join(hash_dump_lines) + ('\n' if hash_dump_lines else ''))
        result = {'command': command, 'head': actual_head, 'lock_id': args.lock_id, 'task_id': args.task_id, 'owner': args.owner,
                  'gate': args.gate, 'mode': args.mode, 'unity_exit': code, 'driver_issue': issue,
                  'seconds': round(time.monotonic() - started, 2), 'pass_marker': has_pass, 'failures': failures,
                  'harness_removed': all(not t.exists() and not Path(str(t) + '.meta').exists() for _, t in pairs),
                  'licence_copy_removed': not licence_copy.exists(), 'cleanup_failures': cleanup_failures}
        if args.gate == 'hash-dump':
            result['hash_dump_lines'] = len(hash_dump_lines)
        remaining = subprocess.check_output(['git', '-C', str(root), 'status', '--porcelain'], text=True).strip()
        result['remaining_lane_changes'] = remaining
        result['host_saves_unchanged'] = all(digest(Path(p)) == h for p, h in save_hashes.items())
        isolated_saves = sorted((out / 'config').rglob('forest-save.json*'))
        result['isolated_saves_after_gate'] = {str(p.relative_to(out)): digest(p) for p in isolated_saves}
        for p in isolated_saves:
            p.unlink()
        result['disk_saves_created'] = [str(p.relative_to(out)) for p in isolated_saves]
        # The save gate exercises the real controller disk path, so isolated saves are expected there and deleted above.
        # 'progress' also calls ForestSaveController.Save() and leaves forest-save.json.bak in the isolated config.
        # Every other gate must leave the isolated config untouched.
        saves_allowed = args.gate in ('save', 'approval', 'coverage-world', 'teaching-latch', 'progress')
        result['pass'] = (result['host_saves_unchanged'] and not remaining and (saves_allowed or not result['disk_saves_created'])
                          and code == 0 and issue is None and has_pass and not failures
                          and result['harness_removed'] and result['licence_copy_removed'] and not cleanup_failures)
        (out / 'process-result.json').write_text(json.dumps(result, indent=2) + '\n')
        print(json.dumps(result, indent=2), flush=True)
    return 0 if result['pass'] else 1


if __name__ == '__main__':
    raise SystemExit(main())
