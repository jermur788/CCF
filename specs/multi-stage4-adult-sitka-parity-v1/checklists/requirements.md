# Requirements — Stage 4 quality gates

**Status: APPROVED (D-095, 2026-11-23).** Mark only from observed evidence.

## Decision gates (satisfied)

- [x] G01 Adult growth uses woodland CI; mortality RD is per rack.
- [x] G02 Biological death is distinct from felling; one fallen-deadwood record per death; no yield.
- [x] G03 Pruning mirrors Unity's 0.92 cut, 0.92^lifts target, 3-lift cap, 5-year interval and 60 % height guard.
- [x] G04 Sitka age phases are absent; site productivity defaults to 1.0.
- [x] G05 Model 0/P0 frozen; Stage 4 is versioned and session-only.

## Implementation evidence (unchecked until observed)

- [ ] R01 Model 0 fingerprints `7e3a2a86…` / `a692a6d5…` and P0 pin `9a99f781…` unchanged; all existing tests pass.
- [ ] R02 Stage 1–3 model 1 output unchanged when `AdultModelVersion=0`.
- [ ] R03 RD matches `N/ha * (Dq/25)^2.063 / 1868`; pressure/hazard match the Unity constants.
- [ ] R04 RD < 0.6 produces zero hazard deaths; 0.6–1.0 is suppression-biased; > 1.0 enforces the cap by killing the most suppressed survivor (TreeId tie-break).
- [ ] R05 A border tree in Area B gains DBH increment after a large Area A competitor within 8 m is felled; a tree > 8 m away does not change.
- [ ] R06 Overstocked Area A self-thins; Area B's RD is computed from Area B alone and is not diluted.
- [ ] R07 Each natural death is `BiologicallyDead` with cause/year, is not a stump, yields no timber, and emits exactly one deadwood record with the specified fields and volume formula.
- [ ] R08 Dead stems are excluded from CI, growth, union cover and light; canopy/light updates after mortality.
- [ ] R09 Prune Pending/Approve change nothing; Execute applies `r * 0.92`, increments lifts, records the year and does not advance the year.
- [ ] R10 Prune rejects dead/felled trees, a fourth lift, a lift within 5 years, a target at or below the current crown base, and a target at or above 60 % of height.
- [ ] R11 Future crown target includes `0.92^lifts` alongside CrownModel 1.
- [ ] R12 `SiteProductivity = 1.0` reproduces the baseline exactly; a synthetic non-1.0 value scales growth through the seam only.
- [ ] R13 Identical seed and commands produce bit-identical multi-year output.
- [ ] R14 No Unity, UI, save, fixture, P0 or `main` change; `/multi` builds 0/0 and serves 200 with no exceptions.
- [ ] R15 Status reported as IMPLEMENTED / NUMERICALLY QUALIFIED / REVIEWED, with independent review before acceptance.
