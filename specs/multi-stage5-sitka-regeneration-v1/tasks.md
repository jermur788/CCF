# Tasks — MULTI Stage 5 (APPROVED)

**Status: APPROVED — owner approval 2026-11-23; AMENDED — owner Option A, 2026-11-23** (quartet, §4 `2.5 m` scaling policy and Q1–Q6 resolutions; Option A changes §4, Q4 and Q5; see the `spec.md` approval and amendment records). **AMENDED — owner final bounded-repair decisions, 2026-11-23:** the `S5-07A` population and units, and the `S5-02` criterion (T16). **AMENDED (documentation only) — owner record, 2026-11-24** (T0f; spec third amendment record). Boxes are ticked only from observed evidence.

- [x] T0a Owner locked decisions 1–6 and `e7c1a57d` base recorded (2026-11-23).
- [x] T0b Read-only source audit: Unity `RegenDensityMax = 1.5`, per-cell occupancy `density/max`, one band promoted per cell per year, `PromotionHeightM = 3.5`, mast 15 %/35 %/50 %, `promotionMinimumLight` absent from `SitkaSpruce.asset`.
- [x] T0c Draft quartet published for owner review.
- [x] T0d **Owner approves quartet and answers Q1–Q6; status changed to APPROVED.** (2026-11-23)
- [x] T0e **Owner approves Option A after the first pass failed `S5-07`** (block-cohort export, area-proportional block budget, split `S5-07A`/`S5-07B`); spec §4, Q4 and Q5 amended. (2026-11-23)
- [x] T0f **Owner record (2026-11-24, documentation only)** in the spec's third amendment record:
  - `S5-07A` keeps the whole-cell unit (nominal matched grid area);
  - the aggregate-only scope and the edge limitation are owner-accepted;
  - the `S5-02` margins are adopted as the owner's from 2026-11-24, with provenance disclosed.

  No code or test change. The test label "raw-count ratio … (not area-normalised)", which is actually the same-true-ground figure, is left as an open cosmetic item.
- [ ] T1 Gate 0: live coordination refresh, D-091 admission, branch from `e7c1a57d`, baseline fingerprints and test count recorded.
- [ ] T2 Stage 1 read-only confirmations (spec Q3, adult-empty short-circuit, `SimulationRandom` shape, band merge code).
- [ ] T3 Version enum, selection/refusal rules, null `MultiAreaStage5State`; Regeneration `0` byte-identical on `0/0/0/0`, `0/0/1/0`, `1/1/1/0`.
- [ ] T4 Undergrowth rules + tests (`S5-03`).
- [ ] T5 Juvenile growth/survival/browse + tests (`S5-05`).
- [ ] T6 Mast + seed rain + establishment + capacity scaling + tests (`S5-02`, `S5-06`).
- [ ] T7 Promotion (block cap, placement, `CrownModel 1` radius) + tests (`S5-08`).
- [ ] T8 Annual wiring, adult-empty continuation, atomic commit (`S5-09`, `S5-10`).
- [ ] T9 `ClearUndergrowth` command through the fenced lifecycle (`S5-04`).
- [ ] T10 Grid-consistency study at 2.5 m vs 5.0 m, including shared-light control (`S5-07`). *(Superseded by T10a/T10b, owner Option A, 2026-11-23.)*
- [ ] T10a Option A promotion rule: block-cohort export and integer block credit (spec §4, amended); `5.0 m` unchanged.
- [ ] T10b `S5-07A` (blocking, identical shared light field, proved) and `S5-07B` (mandatory non-blocking diagnostic).
- [ ] T11 Full frozen-path regression and existing 184+ tests, builds `0/0` (`S5-01`).
- [ ] T12 One handoff Markdown on GitHub with honest status words.
- [ ] T13 Independent review (different model/session from the implementer), then owner acceptance.
- [ ] T14 Option A repair pass on the task branch, with F3–F6 cleanups and a repair handoff.
- [ ] T15 Delta re-review of the changed promotion code and the new `S5-07A`/`S5-07B` tests, then owner acceptance.
- [ ] T16 Final bounded repair (owner decisions 2026-11-23, no rule change):
  - matched-area `S5-07A` (D1) and the `S5-07B` unmatched-block disclosure;
  - comparative `S5-02` (D2), with the poor-light census as a diagnostic;
  - a straddling-block test (F6 corner);
  - a final repair handoff, then delta re-review.
