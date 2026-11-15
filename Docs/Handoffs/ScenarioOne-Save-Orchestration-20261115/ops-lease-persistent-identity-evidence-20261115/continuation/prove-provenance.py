#!/usr/bin/env python3
"""Repeat live read-only audit and isolated drift probes; write evidence on Files."""
import argparse
import json
from pathlib import Path
import shutil
import subprocess
import tempfile


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--ops', type=Path, required=True)
    parser.add_argument('--output', type=Path, required=True)
    args = parser.parse_args()
    ops = args.ops.resolve()
    verifier = ops / 'scripts/verify-deployment'
    manifest_path = ops / 'deployment.json'
    manifest = json.loads(manifest_path.read_text())
    live = Path('/media/jer/ZX20/CCF-Ops')
    results = []

    def run(label, root, expected, receipt=manifest_path, candidate=False):
        command = ['python3', '-B', str(verifier), '--root', str(root),
                   '--manifest', str(receipt)]
        if candidate:
            command += ['--candidate-root', str(ops)]
        result = subprocess.run(command, text=True, capture_output=True)
        record = dict(label=label, command=command, expected_exit=expected,
                      exit_code=result.returncode, stdout=result.stdout,
                      stderr=result.stderr)
        results.append(record)
        print(label, flush=True)
        print(result.stdout + result.stderr, end='', flush=True)
        print('exit=' + str(result.returncode), flush=True)
        assert result.returncode == expected, record

    run('LIVE READ-ONLY AUDIT', live, 0, candidate=True)
    # Only checked source/config is copied. Never copy live board/ledger/state.
    with tempfile.TemporaryDirectory(prefix='Ops548Provenance-',
            dir='/media/jer/Files/CCF/Operations') as tmp:
        scratch = Path(tmp)
        for name in sorted(set(manifest['source_hashes']) |
                           set(manifest['deployed_overrides']) | {'lanes.json'}):
            target = scratch / name
            target.parent.mkdir(parents=True, exist_ok=True)
            shutil.copyfile(live / name, target)
        run('ISOLATED CHECKED-FILE COPY', scratch, 0)
        helper = scratch / 'scripts/_ccf.py'
        original = helper.read_bytes()
        helper.write_bytes(original + b'\n# deliberate source drift probe\n')
        run('REAL SOURCE DRIFT (_ccf.py)', scratch, 1)
        helper.write_bytes(original)
        config = scratch / 'lanes.json'
        original_config = config.read_bytes()
        value = json.loads(original_config)
        value['repository'] = '/deliberately-wrong/repository.git'
        config.write_text(json.dumps(value))
        run('CONFIG DRIFT OUTSIDE /lanes', scratch, 1)
        value = json.loads(original_config)
        value['lanes'] = [{'lane': 'dummy-operational-registration'}]
        config.write_text(json.dumps(value))
        run('EXPECTED OPERATIONAL /lanes DRIFT', scratch, 0)
        config.write_bytes(original_config)
        extra = scratch / 'scripts/unexpected-source.py'
        extra.write_text('# unclassified source must fail\n')
        run('UNCLASSIFIED SOURCE ADDITION', scratch, 1)
        extra.unlink()
        receipt = scratch / 'deployment.json'
        value = json.loads(manifest_path.read_text())
        value['live_state']['scripts/_ccf.py'] = 'attempt to hide source drift'
        receipt.write_text(json.dumps(value))
        run('SOURCE EXCLUSION REJECTED', scratch, 2, receipt=receipt)
        run('ISOLATED COPY RESTORED', scratch, 0)
    args.output.write_text(json.dumps(results, indent=2) + '\n')
    print('PASS: all 8 provenance outcomes matched; live source/state untouched')


if __name__ == '__main__':
    main()
