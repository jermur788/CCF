# Requirements — MULTI Stage 5 numerical quality gates (DRAFT)

**Status: DRAFT.** Mark only from observed evidence. Words are kept separate: `IMPLEMENTED` ≠ `NUMERICALLY QUALIFIED` ≠ `REVIEWED`.

## Decision gates (before implementation)

- [x] G01 Owner locked the six product decisions and the `e7c1a57d` base.
- [ ] G02 Owner approved the §4 area-consistent scaling policy (or chose another).
- [ ] G03 Owner answered Q1–Q6 in `spec.md`.
- [ ] G04 Quartet marked `APPROVED`.
- [ ] G05 D-091 lane receipt and baseline fingerprints recorded.

## Qualification tests

- [ ] **S5-01 Frozen paths.** Byte-identical `0/0/0` (`7e3a2a86…`, `a692a6d5…`), `0/0/1` (`814e8165…`), experiment (`c66d566c…`), P0 pin (`9a99f781…`) and `1/1/1/0` output; all existing 184+ Core tests pass; build 0 warnings/0 errors.
- [ ] **S5-02 Dark interior vs managed gap.** Same seed sources: dense unthinned interior stays suppressed; thinned gap raises the modelled light index, establishes cohorts and grows them toward promotion (non-PAR caveat stated).
- [ ] **S5-03 Bramble/bracken.** Both initialise at `0.25·T`; track light with gain `0.30` / loss `0.45`; `p_veg` equals `0.35·max·heightVuln` exactly (full ≤ 0.3 m, zero ≥ 1.5 m).
- [ ] **S5-04 `ClearUndergrowth`.** Pending/Approve mutate nothing; Execute zeroes bramble and bracken on targets and keeps every Sitka cohort; never advances the year; fence/revision enforced; idempotent replay; stale or invalid targets refuse the whole batch.
- [ ] **S5-05 Browsing.** Pressure `0` bit-identical to light-only; `0.2` gives `p ≤ 0.03` height slowdown and `0.04·p` survival dip; cohorts ≥ 0.8 m take zero browse loss.
- [ ] **S5-06 Cross-rack seed rain.** Mature Sitka in A seed B and vice versa; zero seed from outer grassland; result identical under reversed area processing order; one shared mast roll per year.
- [ ] **S5-07 Grid consistency.** Same woodland at 2.5 m vs 5.0 m: recruitment and multi-year promotions per hectare within the approved tolerance (spec Q4), **with a shared-light control** that isolates the scaling policy from the different light fields; zero out-of-polygon trees.
- [ ] **S5-08 Promotion.** Oldest qualifying band (≥ 3.5 m) → one adult with unique deterministic `TreeId`, `Age = max(1, year − establishYear)`, `Height`, `DBH = clamp(1.5·H, 2, 20)`, `CrownModel 1` radius `r_0` (not `r_pot`), strictly in-polygon; enters adult competition, canopy union and light next refresh.
- [ ] **S5-09 Adult-empty rack.** A rack with 0 living adults advances, receives neighbour seed, updates undergrowth, grows and promotes cohorts.
- [ ] **S5-10 Determinism and atomicity.** Repeated multi-year runs give identical hashes; Stage 5 draws never change `"ADULT-MORT-"` rolls (adult mortality hashes equal with Stage 5 on/off where no promotion has occurred); a forced pre-commit failure leaves the previous `MultiAreaRoot` intact.
- [ ] **S5-11 Independent review** by a separate reviewer on the finished branch and handoff.

## Hygiene

- [ ] H1 No file outside `Tools/MultiRackSitkaP1Lab/`, `specs/`, `Docs/` handoff changed; no `Assets/`, `.meta`, `/multi` wwwroot or save-schema diff (`git diff --stat` shown).
- [ ] H2 Every constant labelled `GAMEPLAY CALIBRATION`/`TRANSFER`/`SIMULATION ABSTRACTION` in code comments or metadata; no claim of measured Irish values or calibrated PAR.
- [ ] H3 Handoff lists what was not verified.
