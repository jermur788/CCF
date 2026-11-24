# Requirements — MULTI Phase 2 Oak and Beech numerical quality gates (DRAFT)

**Status: APPROVED — owner approval 2026-11-24** (gates G03/G04 closed; qualification items below are unmarked until observed). Mark only from observed evidence. Words are kept separate: `IMPLEMENTED` ≠ `NUMERICALLY QUALIFIED` ≠ `REVIEWED`.

- **Spec ID:** SPEC-multi-phase2-oak-beech-V1
- **Checklist owner:** Overall Manager
- **Implementation under test:** TBD (Step B branch / HEAD)
- **Date:** 2026-11-24

## Decision gates (before implementation)

- [x] G01 Owner locked the six product decisions (6/6 Option A) and the `5b757d9b` base.
- [x] G02 Stage 5 prerequisite on the record: `5b757d9b` APPROVED delta-only (`e8f47e73`); owner accepted R1 (`S5-07A` aggregate-only matched population) and R3 (`S5-02` margins) as written.
- [x] G03 Owner answered spec §14 Q1–Q6 (2026-11-24; binding text in the spec amendment record):
  - Q1 **Yes**: species version appended to `AreaModelVersion` only when `!= 0`, and all `0/0/0/0`…`1/1/1/1` tokens and hashes stay byte-identical.
  - Q2 **Yes**: spot patches affect exact-point planted-juvenile exposure only, and Unity's area-weighted cohort dilution is an intentional non-port.
  - Q3 **Yes**: three separate fenced orders `Plant` / `Shelter` / `SpotClear`; `Shelter` targets a living `PJ` or a same-batch `Plant`, and `SpotClear` may target any in-polygon point.
  - Q4 **Yes**: planted juveniles read same-year stepped bramble/bracken covers (audit correction A-1).
  - Q5 **Yes, with an owner-set share**: test-only `MA-1-MIXED-v1`; Area A pure Sitka; Area B one third Oak, one third Beech and one third Sitka, age 60.
  - Q6 **Yes (mandatory)**: `1/1/1/1/1` on unmodified all-Sitka MA-1 equals `1/1/1/1/0` identically in trees, bands and accounts, version token aside.
- [x] G04 Quartet marked `APPROVED — owner approval 2026-11-24` (spec amendment record; rebased onto Stage 5 docs tip `d1db81b5`).
- [x] G05 D-091 lane receipt and baseline anchors recorded (Step B handoff `PHASE2-STEPB-HANDOFF-20261124.md` §5, at `b565abf0`).

## Qualification tests

- [ ] **P2-01 Frozen paths.** With `Species = Off`, byte-identical outputs and tokens for:
  - single-area Model 0 `7e3a2a869ec1662458b74875cadee1e4b16cf2b87b0aaa05df2d9a0061311ece`;
  - multi `0/0/0/0` `a692a6d5bc374dd0225153b3bf0e0b0e9467d3c041b0a6b03414f42d3c02233d`;
  - multi `0/0/1/0` `814e81650b62428510fe60c3142b51d9055537d6014303074f08733bf8d46287`;
  - Stage 4 `1/1/1/0` `8a926776ffa3ca3dd1611c2e4d462f8c82b4d78f29ad5e83e5a8f8ce1d2b87fb`;
  - Stage 5 `1/1/1/1` `c4b670ede36ef133fb7ab9b43992319b340f27827ce85025ae68444c63d2dc35`, **now pinned as a constant** (declared test change, closes R8);
  - P0 Core binary pin `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3`.

  Also: `RequireSupported()` accepts exactly the five spec §2 tuples and refuses every other `Species = 1` combination with `SpeciesModelIncomplete`; all existing Stage 1–5 Core tests pass unchanged; build 0 warnings / 0 errors.
- [ ] **P2-02 Broadleaf adults.** Oak and Beech DBH and height increments equal spec §3.1 to float precision on hand-computed cases (open, competed, near `Dmax`/`Hmax`); Oak `r_pot = 0.1 + 0.1045·DBH`; Beech `r_pot = sqrt(exp(0.05 + 1.01·ln(max(0.1, DBH)))/π)`, and Beech's linear asset fields are never read; C2 target and relaxation with species `Ci50`/`k`; no transmissivity term in light.
- [ ] **P2-03 Mixed density.**
  1. A 100 % Sitka rack inside a mixed woodland gives victims and `ADULT-MORT-*` draws identical to the Stage 4 path, and draws no `ADULT-MORT-GENERIC-*` key.
  2. In a mixed rack, Sitka-component victims are only Sitka stems and equal the Comeau result on the Sitka subset.
  3. Generic occupancy, `G(O)`, `nonReferenceShare`, quota and remainder roll match spec §5.1 on hand-computed cases.
  4. **Neutrality:** two stems with equal `CI` and DBH, one Beech and one Sitka, carry equal victim weight (`Ci50 = 5`), while Beech growth uses `Ci50 = 6.5`.
  5. Generic removal never exceeds `0.125` of stand basal area in a year except the documented 1-stem fallback.
  6. Broadleaf pruning: 2 lifts, 3-year interval, exact 3.5 / 6.0 m targets; Sitka pruning unchanged.
  7. Fallen deadwood records carry `SpeciesId` and volume `DBH²·0.00007854·H·0.5`.
  8. *(Q6 accepted, mandatory)* `1/1/1/1/1` on unmodified all-Sitka MA-1 equals `1/1/1/1/0` in trees, bands and accounts.
- [ ] **P2-04 Commands.** `Plant`/`Shelter`/`SpotClear`: Pending and Approve mutate nothing; Execute never advances the year; fence and revision enforced; idempotent replay with the engine-minted key; refusal of the **whole batch** on: out-of-polygon; `< 0.5 m` from another planting (lower work-order id wins); `< 0.7 m` from a living adult stem; duplicate order; shelter with no target or a second effective shelter within 0.25 m; `Species = Off`. No economy or stock field exists or changes.
- [ ] **P2-04b Boundary sliver** *(added at the Step B review follow-up; ratified refusal, spec amendment record F2).* A point strictly inside the polygon but in no in-polygon 2.5 m ground cell is refused with `Phase2PointWithoutGroundCell` for `Plant`, `Shelter` and `SpotClear`. The refused Add changes nothing. A sliver order injected into an approved batch refuses the **whole batch** atomically: no patch, juvenile, shelter, work record, year or revision. The same batch without it executes. *(Observed PASS at `8894ab86`, pending independent review.)*
- [ ] **P2-05 Browsing and shelters.** At `pressure = 0.2` an unprotected planted Oak loses height/survival per spec §6.2 and a sheltered one at the same point does not (`access = 0`). Shelter effective in years `installed … installed+7` and **not** in `installed+8` (strict `<`). `FailedYear` stays `-1`. Oak palatability 1.0 vs Beech 0.5 visible in `p`. `V(h)` full ≤ 1.2 m, zero ≥ 1.8 m. Cohorts ignore shelters. `pressure = 0` bit-identical to the light-only path.
- [ ] **P2-06 Spot clearance.** Three runs of the same planting: with a 1 m² patch, unweeded, and with whole-cell `ClearUndergrowth`. The patch is a circle of radius `0.5641896 m` (area 1.0 m²), zeroes cover only inside it, recovers each year at 0.30/0.45 toward the containing cell's `T`, and changes the planted stem's `p_veg` accordingly. In all three runs, every natural cohort's density in and around the patch is unchanged by the clearance itself (selective). The divergence from Unity's destructive clearance is stated in the test name or output.
- [ ] **P2-06b Paired survival** *(added at the Step B review follow-up, spec amendment record F1).* The same planted Oak in the same full-light cell, unweeded vs 1 m² spot-cleared, at default browse pressure over replicated area seeds. The unweeded arm records at least one death with `DeathCause = "vegetation"` (counted as `VegetationDeaths`, not light), and patched survival is **strictly greater** than unweeded survival. The criterion must fail if vegetation can never kill a planted stem (reviewer mutation m18). *(Observed PASS at `8894ab86`: 152/200 vs 177/200 over 6 years; pending independent m18 verification.)*
- [ ] **P2-07 Planted promotion.** A planted Oak/Beech reaching `H ≥ 3.5 m` promotes to `PL-{juvenileId}` at its exact X/Z in the same block and year as a natural promotion (block cap bypassed, natural block credit unchanged). A planted Oak at `H ≥ 3.5 m` under cell light `< 0.2` does **not** promote; at `≥ 0.2` it does; Beech has no gate. Initial radius is the C2 target, not `r_pot`. `Age = max(1, round(age))`, `DBH = clamp(1.5·H, 2, 20)`. No same-year canopy recompute.
- [ ] **P2-07b End-to-end plant → promote** *(added at the Step B review follow-up, spec amendment record F3).* On MA-1, through the public session path only, with no seeded or hand-built juvenile state: `SpotClear` + `Plant` + `Shelter` at a bright Area B point, then `Advance` until the juvenile promotes to `PL-{juvenileId}` at its exact X/Z with the C2 initial crown. Natural 5 m block credit and natural promotions equal an unplanted twin's in every year up to promotion. *(Observed PASS at `8894ab86`: `PJ0001` → `PL-PJ0001` in ecological year 9; pending independent review.)*
- [ ] **P2-08 Seed rain and mast.** Mature Oak/Beech in rack A seed rack B and vice versa within `λ`/cutoff (Oak 4/80 m, Beech 6/50 m); zero from outer grassland; identical under reversed rack order. Oak, Beech and Sitka mast sequences are independent (changing one species' stream seed changes no other species' multipliers or any `ADULT-MORT-*` roll). A rack with 0 living adults advances, receives seed, updates undergrowth and grows/promotes cohorts.
- [ ] **P2-09 Shared capacity and block cap.** A cell seeded by all three species never exceeds combined relative occupancy 1 (capacity `1.5·a`). At most one natural promotion per aligned 5 m block per year across all species, with Stage 5 partial edge credit. Constructed ties resolve oldest `EstablishYear` → tallest → `(Column, Row)` → ordinal `SpeciesId`. Cohort export removes only same-species, same-year sibling bands.
- [ ] **P2-10 Determinism.** On `MA-1-MIXED-v1`, two 30-year two-rack runs give identical hashes; reversed rack processing order gives identical hashes; replaying any Execute with the same key is a no-op; a forced pre-commit failure leaves the previous root intact; Phase 2 draws never change any `ADULT-MORT-{treeId}` roll.
- [ ] **P2-11 Independent review** by a separate reviewer on the finished branch and handoff.

## Hygiene

- [ ] H1 No file outside `Tools/MultiRackSitkaP1Lab/`, `specs/`, `Docs/Handoffs/` changed; no `Assets/`, `.meta`, `/multi` wwwroot, `ProjectSettings` or save-schema diff (`git diff --stat` shown); no mode-only churn.
- [ ] H2 Every constant labelled `GAMEPLAY CALIBRATION` / `ORDINAL` / `TRANSFER` / `SIMULATION ABSTRACTION` / `NEW OWNER RULE`; none labelled `IRISH DIRECT`; no claim of measured Irish values or calibrated PAR.
- [ ] H3 The 0.7 m adult-spacing rule is labelled a new owner rule, not a transfer.
- [ ] H4 Handoff states plainly that `Plant`/`Shelter`/`SpotClear` are not a port of Unity `ResolvePlanting` (no stock, cash, minutes or offer identity).
- [ ] H5 Handoff lists what was not verified (no Unity, UI or rendered verification).

## Verdict

- **Spec conformance:** NOT VERIFIED (Step B not started)
- **Implementation correctness:** NOT VERIFIED
- **Notes:** Draft gates only.
