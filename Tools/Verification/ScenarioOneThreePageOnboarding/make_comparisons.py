#!/usr/bin/env python3
"""Compose evidence sheets; retain actual Unity pixels at native size, no retouching."""
import argparse
import importlib.util
import json
from pathlib import Path


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--renders', type=Path, required=True)
    parser.add_argument('--out', type=Path, required=True)
    args = parser.parse_args()
    tools = Path(__file__).resolve().parent
    repository = tools.parents[2]
    spec = importlib.util.spec_from_file_location('v2_comparison', tools.parent / 'ScenarioOneVisualSystemV2/make_comparisons.py')
    compositor = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(compositor)
    references = repository / 'specs/scenario-one-three-page-onboarding-v1/references'
    locks = json.loads((references / 'final-hashes.json').read_text())
    prepared = []
    for page in range(1, 4):
        reference = references / ('page-%d-locked.png' % page)
        reference_hash = compositor.sha256(reference)
        assert reference_hash == locks[page - 1]['sha256'], 'Final reference changed'
        for size in ['1280x720', '1920x1080']:
            actual = args.renders / ('page-%d-%s.png' % (page, size))
            assert actual.is_file(), 'Missing real Unity render: ' + str(actual)
            rendered = compositor.load_rgb(actual, 'actual Unity capture')
            assert '%dx%d' % rendered.size == size
            prepared.append((page, size, reference, reference_hash, actual, rendered))
    args.out.mkdir(parents=True, exist_ok=True)
    records = []
    for page, size, reference, reference_hash, actual, rendered in prepared:
        actual_hash = compositor.sha256(actual)
        sheet = compositor.build_sheet(compositor.load_rgb(reference, 'locked reference'), rendered,
                                       reference.name, reference_hash, actual.name, actual_hash)
        destination = args.out / ('page-%d-%s-comparison.png' % (page, size))
        sheet.save(destination)
        records.append(dict(page=page, size=size, reference=str(reference), reference_sha256=reference_hash,
                            actual=str(actual), actual_sha256=actual_hash, comparison=str(destination),
                            comparison_sha256=compositor.sha256(destination)))
    (args.out / 'comparison-manifest.json').write_text(json.dumps(records, indent=2) + '\n')
    print('Six native-pixel comparison sheets written; final-reference hashes enforced.')


if __name__ == '__main__':
    main()
