# Requirements — MULTI #899 numerical quality gates

**Status: APPROVED (D-094, 2026-11-23).** Mark only from observed evidence.

## Decision gates (satisfied before implementation)

- [x] G01 Owner selected one realised-crown algorithm with exact equations and constants (D-094: C2).
- [x] G02 Model 0 frozen for backward compatibility and baseline regression.
- [x] G03 Connected-woodland boundary policy fixed: cross-border neighbours inside 8 m count; outer perimeter borders open grassland; no synthetic ghosts.
- [x] G04 Honest provenance recorded: simulation abstraction/gameplay calibration, Frenchpark +2–7 % widest-crown and ~5–15 % mean-radius upward-bias caveat.
- [x] G05 Quartet marked APPROVED; no frozen fixture, save schema, Unity or UI change is implied.

## Implementation evidence (unchecked until observed)

- [ ] C01 Model 0 reproduces exact current output/hashes on fixed runs.
- [ ] C02 Model 1 year 0 uses `r_0 = r_target(CI_0)`, not `r_pot`, on both areas.
- [ ] C03 Annual radius follows `r + (r_target - r) * 0.15`, stays finite/positive and never moves more than 15 % of the remaining gap in one year.
- [ ] C04 `CI` spans all living trees in the connected woodland in deterministic TreeId order; a stem within 8 m of the shared border sees cross-border neighbours; a stem at the outer perimeter sees none.
- [ ] C05 Work Plans, approvals and summaries remain per rack; commands mutate only the addressed area.
- [ ] C06 Clipped **union** cover and uncovered area are reported, never the sum of crown disks.
- [ ] C07 Years 0/1/3 matched no-work vs real 10-tree Fell results reported with exact removed tree IDs and per-area metrics.
- [ ] C08 No new scenario/model version rewrites frozen `p1d-30m-sitka-adult-v1`; compatibility and default selection recorded.
- [ ] C09 No Unity, SVG, map-artwork or save-schema change.
- [ ] C10 Deterministic replay and protected references (P0 hash, Legacy40, Reference Future v1) preserved.
- [ ] C11 Direct commit, evidence, independent review and owner playtest reported separately; no automatic acceptance.
