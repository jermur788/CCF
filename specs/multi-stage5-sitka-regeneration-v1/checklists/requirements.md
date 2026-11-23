# Requirements — MULTI Stage 5 numerical quality gates (APPROVED)

**Status: APPROVED — owner approval 2026-11-23; AMENDED — owner Option A, 2026-11-23** (quartet, §4 `2.5 m` scaling policy and Q1–Q6 resolutions; Option A changes §4, Q4 and Q5 and splits `S5-07`; see the `spec.md` approval and amendment records). **AMENDED — owner final bounded-repair decisions, 2026-11-23:** the `S5-07A` population and units (D1) and the `S5-02` criterion (D2); no rule changes; see the second `spec.md` amendment record. Mark only from observed evidence. Words are kept separate: `IMPLEMENTED` ≠ `NUMERICALLY QUALIFIED` ≠ `REVIEWED`.

## Decision gates (before implementation)

- [x] G01 Owner locked the six product decisions and the `e7c1a57d` base.
- [x] G02 Owner approved the §4 area-consistent scaling policy (or chose another). (2026-11-23: approved as written)
- [x] G03 Owner answered Q1–Q6 in `spec.md`. (2026-11-23, recorded verbatim in the `spec.md` approval record)
- [x] G04 Quartet marked `APPROVED`. (2026-11-23)
- [ ] G05 D-091 lane receipt and baseline fingerprints recorded.
- [x] G06 Owner approved Option A (2026-11-23): block-cohort export, an area-proportional block budget with no invented constant and `5.0 m` unchanged at `a = 1.0`, and `S5-07` split into `S5-07A` (blocking) and `S5-07B` (non-blocking diagnostic).
- [x] G07 Owner final bounded-repair decisions (2026-11-23):
  - D1: `S5-07A` is gated on the blocks represented on both lattices, per hectare of matched in-polygon area.
  - D2: `S5-02` is comparative suppression; the poor-light census is a diagnostic only.

## Qualification tests

- [ ] **S5-01 Frozen paths.** Byte-identical `0/0/0` (`7e3a2a86…`, `a692a6d5…`), `0/0/1` (`814e8165…`), experiment (`c66d566c…`), P0 pin (`9a99f781…`) and `1/1/1/0` output; all existing 184+ Core tests pass; build 0 warnings/0 errors.
- [ ] **S5-02 Dark interior vs managed gap — comparative suppression (amended 2026-11-23, final bounded repair D2).** Same seed sources, a managed gap against the unthinned interior, 12 test cells, 15 years; non-PAR caveat stated. Blocking criteria (spec, second amendment record):
  1. The gap raises the year-1 light index by more than 0.2.
  2. No dense test cell's band reaches 3.5 m, and none promotes.
  3. In every year, the dense interior is neither taller than the tallest cohort the gap has reached nor more abundant.
  4. In years 12 and 15, the gap leads by at least **0.315 m** (one maximal year of §3.2 growth), and the dense abundance is at most **0.8 ×** the gap's (the §3.2 poor-light survival factor).
  5. The gap's tallest cohort is at least 1.5 m in year 12.

  The poor-light census (dense cells with LR < 0.15) is **printed, never asserted**.
- *Superseded (first approval, wording):* "**S5-02 Dark interior vs managed gap.** Same seed sources: dense unthinned interior stays suppressed; thinned gap raises the modelled light index, establishes cohorts and grows them toward promotion (non-PAR caveat stated)." *Superseded (first-pass and Option A test proxy, never owner-specified):* "every dense test cell is in the poor-light band in year 12."
- [ ] **S5-03 Bramble/bracken.** Both initialise at `0.25·T`; track light with gain `0.30` / loss `0.45`; `p_veg` equals `0.35·max·heightVuln` exactly (full ≤ 0.3 m, zero ≥ 1.5 m).
- [ ] **S5-04 `ClearUndergrowth`.** Pending/Approve mutate nothing; Execute zeroes bramble and bracken on targets and keeps every Sitka cohort; never advances the year; fence/revision enforced; idempotent replay; stale or invalid targets refuse the whole batch.
- [ ] **S5-05 Browsing.** Pressure `0` bit-identical to light-only; `0.2` gives `p ≤ 0.03` height slowdown and `0.04·p` survival dip; cohorts ≥ 0.8 m take zero browse loss.
- [ ] **S5-06 Cross-rack seed rain.** Mature Sitka in A seed B and vice versa; zero seed from outer grassland; result identical under reversed area processing order; one shared mast roll per year.
- [ ] **S5-07A Grid consistency — BLOCKING (amended 2026-11-23, owner Option A; amended again 2026-11-23, final bounded repair D1).**
  - *Setup.* The same MA-1 woodland at 2.5 m and at 5.0 m: ≥ 20 seeds, 30 years, managed thinning and no work.
  - *Shared light.* Both grids read an **identical shared light field**: the regeneration-free `1/1/1/0` adult canopy, read at aligned 5 m block centres. Bit-equal light at every coincident point in every year is asserted, and the test's year loop is shown to reproduce `Advance` bit for bit under model light.
  - *Population.* The **matched-area population**: the aligned 5 m blocks represented on both lattices (5 m centre inside a polygon; 101 on MA-1). Its 1:1 correspondence with the 5.0 m cells is **asserted**.
  - *Units.* **Per hectare of matched in-polygon area**: 2.5 m cells × 6.25 m² (1–4 per matched block) against 5.0 m cells × 25 m². Both area totals are reported and asserted > 0.
  - *Gate.* The **pooled and mean** area-normalised ratios of cumulative accepted abundance and of cumulative promotions all lie in `[0.75, 1.33]`.
  - *Also required.* Zero out-of-polygon promoted trees. `5.0 m` keeps `a = 1.0` with no edge-gate reduction (output equals the first-pass engine). No damping constant. (spec §4, §9 Q4)
- *Superseded (Option A, 2026-11-23):* "The whole-stand **pooled and mean** ratios of cumulative accepted abundance and of cumulative promotions all lie in `[0.75, 1.33]`." (raw whole-stand totals)
- [ ] **S5-07B Grid diagnostic — MANDATORY, NON-BLOCKING (amended 2026-11-23, owner Option A; disclosure added 2026-11-23, final bounded repair D1).** The same comparison under each grid's own light field, with no threshold and no assertion. It reports:
  - real-light ratios;
  - interior-only and edge-only sub-ratios for both light bases;
  - per-seed minimum/median/maximum;
  - the non-PAR (`unity-k10a10-decoupled-v1`) statement;
  - the finer-sampling statement;
  - **the unmatched blocks**, which exist on the 2.5 m lattice only (12 on MA-1): their count, their in-polygon area, and their promotions and accepted abundance under both light bases, with the explicit statement that the whole-stand 2.5 m figure exceeds 5.0 m partly because the finer lattice represents boundary ground the coarser one cannot. (spec §9 Q4)
- *Superseded (first approval):* "**S5-07 Grid consistency.** Same woodland at 2.5 m vs 5.0 m: recruitment and multi-year promotions per hectare within the approved tolerance (spec Q4), **with a shared-light control** that isolates the scaling policy from the different light fields; zero out-of-polygon trees."
- [ ] **S5-08 Promotion.** Oldest qualifying band (≥ 3.5 m) → one adult with unique deterministic `TreeId`, `Age = max(1, year − establishYear)`, `Height`, `DBH = clamp(1.5·H, 2, 20)`, `CrownModel 1` radius `r_0` (not `r_pot`), strictly in-polygon; enters adult competition, canopy union and light next refresh.
- [ ] **S5-09 Adult-empty rack.** A rack with 0 living adults advances, receives neighbour seed, updates undergrowth, grows and promotes cohorts.
- [ ] **S5-10 Determinism and atomicity.** Repeated multi-year runs give identical hashes; Stage 5 draws never change `"ADULT-MORT-"` rolls (adult mortality hashes equal with Stage 5 on/off where no promotion has occurred); a forced pre-commit failure leaves the previous `MultiAreaRoot` intact.
- [ ] **S5-11 Independent review** by a separate reviewer on the finished branch and handoff.

## Hygiene

- [ ] H1 No file outside `Tools/MultiRackSitkaP1Lab/`, `specs/`, `Docs/` handoff changed; no `Assets/`, `.meta`, `/multi` wwwroot or save-schema diff (`git diff --stat` shown).
- [ ] H2 Every constant labelled `GAMEPLAY CALIBRATION`/`TRANSFER`/`SIMULATION ABSTRACTION` in code comments or metadata; no claim of measured Irish values or calibrated PAR.
- [ ] H3 Handoff lists what was not verified.
