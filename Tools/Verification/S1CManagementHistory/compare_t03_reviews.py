#!/usr/bin/env python3
"""Compare unchanged T-02 fixtures across the pre-T03 integration and T03 candidate.

Raw compare_reviews.py evidence is retained and expected to differ. Only the four
approved T03 additions may be removed in a separate evidence copy; the unchanged
protected comparison then requires equal world hashes, gate tables and remaining
text. Neither source evidence nor a protected expected value is rewritten.
"""
import argparse
import json
from pathlib import Path
import re
import shutil
import subprocess
import sys

NOTE = 'The records hold one settlement for the whole job, so no money is shown per order or per tree. The year number labels the job; it is not a unique job ID.'
MONEY_NOTE = 'Timber sold and harvesting work above belong to one grouped harvest job for the year, not to single trees or orders.'
TOGGLE = re.compile(r'^\[button\] Show recorded details · [1-9]\d* tasks?(?: · [1-9]\d* map squares?)?$')
SETTLEMENT = re.compile(r'^Harvest job · year \d+: [1-9]\d* harvest order\(s\) settled together as one job\. Timber income €[\d,.]+ · harvesting cost €[\d,.]+\.$')
CANCELLED = re.compile(r'^[1-9]\d* order\(s\) cancelled before they were carried out\.$')


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--baseline', type=Path, required=True)
    parser.add_argument('--candidate', type=Path, required=True)
    parser.add_argument('--out', type=Path, required=True)
    parser.add_argument('--repo', required=True)
    parser.add_argument('--base-rev', required=True)
    parser.add_argument('--head-rev', required=True)
    args = parser.parse_args()
    out = args.out.resolve()
    if out.exists():
        raise SystemExit('Comparison output already exists; retain the prior evidence and use a new folder')
    if not out.is_relative_to(Path('/media/jer/Files/CCF/Verification')):
        raise SystemExit('Comparison output must use durable Files verification storage')
    out.mkdir(parents=True)
    compare = Path(__file__).with_name('compare_reviews.py')
    common = ['--repo', args.repo, '--base-rev', args.base_rev, '--head-rev', args.head_rev]
    raw = subprocess.run([sys.executable, '-B', str(compare), '--baseline', str(args.baseline), '--candidate', str(args.candidate), *common], capture_output=True, text=True)
    (out / 'raw-comparison.txt').write_text(raw.stdout + raw.stderr)
    receipts = {}
    removals = {}
    for name, source in [('baseline', args.baseline), ('candidate', args.candidate)]:
        dest = out / name
        (dest / 'evidence/review-texts').mkdir(parents=True)
        for file in ['process-result.json', 'evidence/t02-result.txt', 'evidence/gate-states.tsv']:
            shutil.copy2(source / file, dest / file)
        for path in sorted((source / 'evidence/review-texts').glob('*.txt')):
            lines = path.read_text().splitlines(keepends=True)
            kept, removed = [], []
            for line in lines:
                text = line.rstrip('\n')
                if name == 'candidate' and (text in (NOTE, MONEY_NOTE) or TOGGLE.fullmatch(text) or SETTLEMENT.fullmatch(text) or CANCELLED.fullmatch(text)):
                    removed.append(text)
                else:
                    kept.append(line)
            (dest / 'evidence/review-texts' / path.name).write_text(''.join(kept))
            if removed:
                removals[path.stem] = removed
        receipts[name] = json.loads((source / 'process-result.json').read_text()).get('pass')
    (out / 'approved-additions.json').write_text(json.dumps(removals, indent=2) + '\n')
    result = subprocess.run([sys.executable, '-B', str(compare), '--baseline', str(out / 'baseline'), '--candidate', str(out / 'candidate'), *common], capture_output=True, text=True)
    (out / 'protected-comparison.txt').write_text(result.stdout + result.stderr)
    ok = result.returncode == 0 and all(receipts.values()) and bool(removals)
    summary = {'pass': ok, 'raw_comparison_exit': raw.returncode, 'protected_comparison_exit': result.returncode,
               'states_with_approved_additions': len(removals), 'removed_addition_lines': sum(map(len, removals.values())),
               'baseline': str(args.baseline), 'candidate': str(args.candidate), 'base_rev': args.base_rev, 'head_rev': args.head_rev}
    (out / 'result.json').write_text(json.dumps(summary, indent=2) + '\n')
    print(result.stdout)
    print(json.dumps(summary, indent=2))
    return 0 if ok else 1


if __name__ == '__main__':
    raise SystemExit(main())
