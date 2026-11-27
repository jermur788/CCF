# Downy Birch and Common Alder V1 Requirements Checklist

- **Spec ID** `SPEC-multi-2d-birch-alder-v1`
- **Spec status** APPROVED / IMPLEMENTING
- **Checklist owner** Overall Manager
- **Implementation under test** `feat/multi-2d-birch-alder-engine-v1`, Part 1 only
- **Date** 2026-11-27

## Part 1 worker evidence

| Requirement | Acceptance check | Evidence |
| --- | --- | --- |
| FR-1 | Neutral identity, table agreement, separate modifiers, bog caution, no duplicated foundation type | AC-1, SITE-01 |
| FR-2 | Standard 0.64 ha layout, y-midpoint split, intact five lanes, inclusive bounds, bad input refused | AC-2, ZONE-01/02 |
| FR-3 | Birch card reuse, pioneer-then-moderate growth, strong crowding, bounded domain | AC-3, BIRCH-01, BIRCH-GROWTH-01/02 |
| FR-3 | Birch local seed, bright opening, low competition, 3–5 year schedule | AC-3, BIRCH-REGEN-01/02 |
| FR-4 | Alder card reuse, fast-then-slow growth, bounded domain | AC-4, ALDER-01, ALDER-GROWTH-01/02 |
| FR-4 | Alder local seed, moisture gate, exposed mineral ground, bog refused | AC-4, ALDER-REGEN-01/02 |
| FR-4 | Heart-rot onset, rising risk, determinism, no pre-50 downgrade | AC-4, ROT-01/02 |
| FR-5 | EUR 42/t bulk, card densities, EUR 0.95 sapling, quality-stem flag and buyer note, alder downgrade | AC-5, TIMBER-01/02/03, ROT-03 |
| FR-6 | Standalone console gate, fenced source additions, cross-process digest | AC-6, DET-01 and worker handoff |
| D-059 | Evidence classes and provisional parameters documented | Spec evidence table and public evidence tags |
| Save and write-back | Part 1 is a pure computation API | Source and file-fence inspection |

## Remaining acceptance

- [ ] Independent reviewer checks Part 1 conformance, calibration wording and determinism against the final feature commit.
- [ ] Part 2 AC-7 is **QUEUED** behind Playable Milestone 1 and Scots Pine Part 2, and is itself sequenced Downy birch first, then Common alder.
- [ ] The coordinator creates `review/multi-2d-birch-alder-engine-v1` after review; this worker does not.

## Verdict

- **Spec conformance:** NOT VERIFIED (worker self-check only)
- **Implementation correctness:** NOT VERIFIED (worker self-check only)
