#!/usr/bin/env python3
"""Compare baseline and candidate T-02 evidence, and the acknowledgement-gate source line, for regression.

  compare_reviews.py --baseline DIR --candidate DIR [--repo REPO --base-rev REV --head-rev REV]

DIR is the folder given to run_t02_gate.py as --out (its evidence/ subfolder is read). Both runs use the same fixture, so
for states both runs visited the review text and gate table should be identical; any difference is printed and listed in
the summary (exit 1) so it has to be explained rather than assumed.

With --repo/--base-rev/--head-rev it also extracts the line that decides whether the acknowledgement footer shows from
AnnualReviewView.cs at both revisions and requires them to be identical.
"""
import argparse
import difflib
import json
from pathlib import Path
import re
import subprocess
import sys


def read_states(folder: Path):
    texts = {p.stem: p.read_text() for p in sorted((folder / 'evidence/review-texts').glob('*.txt'))}
    gates = {}
    table = folder / 'evidence/gate-states.tsv'
    if table.exists():
        lines = table.read_text().splitlines()
        for line in lines[1:]:
            cells = line.split('\t')
            gates[cells[0]] = cells[1:]
    return texts, gates


def gate_line(repo: str, rev: str):
    source = subprocess.run(['git', '-C', repo, 'show', rev + ':Assets/ForestPrototype/UI/AnnualReviewView.cs'],
                            capture_output=True, text=True, check=True).stdout
    match = re.search(r'acknowledgement\.style\.display\s*=\s*([^;]+);', source)
    return re.sub(r'\s+', ' ', match.group(1)).strip() if match else None


def main():
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    parser.add_argument('--baseline', type=Path, required=True)
    parser.add_argument('--candidate', type=Path, required=True)
    parser.add_argument('--repo')
    parser.add_argument('--base-rev')
    parser.add_argument('--head-rev')
    args = parser.parse_args()
    problems = []
    for name, folder in [('baseline', args.baseline), ('candidate', args.candidate)]:
        receipt = folder / 'process-result.json'
        if not receipt.exists() or not json.loads(receipt.read_text()).get('pass'):
            problems.append(name + ' has no passing process receipt')
    hashes = []
    for folder in (args.baseline, args.candidate):
        report = folder / 'evidence/t02-result.txt'
        match = re.search(r'world hash after fixture: (\w+)', report.read_text()) if report.exists() else None
        hashes.append(match.group(1) if match else None)
    print('fixture world hashes:', hashes)
    if not hashes[0] or hashes[0] != hashes[1]:
        problems.append('fixture world hashes missing or different')

    base_texts, base_gates = read_states(args.baseline)
    cand_texts, cand_gates = read_states(args.candidate)
    shared = sorted(set(base_texts) & set(cand_texts))
    required = {'latest-default', 'year1', 'year2', 'year3', 'latest-after-roundtrip',
                'completion-only-latest', 'completion-only-older', 'latest-after-ack', 'new-year-after-ack'}
    required.update('teaching-only-' + str(stage) + suffix for stage in (1, 4, 6) for suffix in ('-latest', '-older'))
    if not required.issubset(shared):
        problems.append('missing comparison states: ' + ', '.join(sorted(required - set(shared))))
    if not required.issubset(set(base_gates) & set(cand_gates)):
        problems.append('missing required gate states')
    print(f'states in both runs: {len(shared)}; baseline only: {sorted(set(base_texts) - set(cand_texts))}; '
          f'candidate only: {sorted(set(cand_texts) - set(base_texts))}')
    for label in shared:
        if base_texts[label] != cand_texts[label]:
            problems.append('text differs: ' + label)
            print('\n--- text differs:', label)
            for line in difflib.unified_diff(base_texts[label].splitlines(), cand_texts[label].splitlines(), 'baseline', 'candidate', lineterm='', n=1):
                print('   ', line)
    for label in sorted(set(base_gates) & set(cand_gates)):
        if base_gates[label] != cand_gates[label]:
            problems.append('gate state differs: ' + label)
            print('\n--- gate state differs:', label, '\n    baseline :', base_gates[label], '\n    candidate:', cand_gates[label])
    identical = [label for label in shared if base_texts[label] == cand_texts[label]]
    print(f'\nidentical review text: {len(identical)}/{len(shared)} states -> {", ".join(identical)}')

    if args.repo and args.base_rev and args.head_rev:
        before, after = gate_line(args.repo, args.base_rev), gate_line(args.repo, args.head_rev)
        print('\nacknowledgement footer rule @ base :', before)
        print('acknowledgement footer rule @ head :', after)
        if before != after:
            problems.append('acknowledgement footer rule changed')

    print('\nRESULT:', 'IDENTICAL (no regression in compared states)' if not problems else 'DIFFERENCES: ' + '; '.join(problems))
    return 0 if not problems else 1


if __name__ == '__main__':
    sys.exit(main())
