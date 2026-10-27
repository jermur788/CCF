# Scenario One Three-Species Parity P0 Bounded Tasks

- **Spec ID:** SPEC-scenario-one-three-species-parity-p0-V1
- **Spec status:** DRAFT
- **Task list owner:** Overall Manager (ChatGPT Game Dev)
- **Last updated:** 2026-10-27

Tasks are implementation units, not new product decisions. They may be drafted before approval, but implementation must not be dispatched until the spec is `APPROVED`. Material ambiguity returns to the Manager/spec.

## PARITY-01 — Current-main implementation audit

- **Status:** TODO
- **Goal:** Re-audit current `main` and enumerate every species-sensitive code/data/UI path relevant to D-059/D-061/D-062.
- **Scope:** adult growth/density; canopy/light/site; competition; regeneration; juveniles; browsing/protection; timber assortment/yield/economy; inspection/teaching; save/versioning; deterministic/reference gates.
- **Exclusions:** Read-only. No production edits, no Unity writes.
- **Owner / routing:** MiMo or Claude for read-only inspection; Manager review.
- **Dependencies:** —
- **Verification:** exact file/symbol inventory and current behaviour table for Sitka/Oak/Beech; clearly distinguish verified facts from assumptions.
- **Stop conditions:** current implementation contradicts a spec premise or shows a substantial additional cross-system dependency.
- **Completion state:** `CURRENT SPECIES IMPLEMENTATION MAPPED`.

## PARITY-02 — Evidence and parameter closure

- **Status:** TODO
- **Goal:** Convert accepted evidence into an explicit species parameter/ordinal/economic decision matrix without pseudo-empirical precision.
- **Scope:** Oak/Beech growing-space classes; adult traits; regeneration/juvenile/browse differences; timber assortment rules; economic values; evidence grades.
- **Exclusions:** No production code.
- **Owner / routing:** Claude or Deep Research evidence synthesis; Overall Manager acceptance.
- **Dependencies:** PARITY-01
- **Verification:** every value/class/rule linked to `DIRECT`, `TRANSFER`, `MODEL DERIVATION`, `ORDINAL REPRESENTATION`, `GAMEPLAY CALIBRATION`, `FALLBACK-CONSERVATIVE`, or `UNSUPPORTED — NEEDS DECISION`.
- **Stop conditions:** evidence does not support a proposed quantitative value and no acceptable ordinal/calibrated alternative is authorised.
- **Completion state:** `SPECIES PARAMETER MATRIX ACCEPTED`.

## PARITY-03 — Mixed density representation

- **Status:** TODO
- **Goal:** Implement D-061 while proving pure-Sitka preservation.
- **Scope:** ordinal species-aware growing-space-demand classes; mixed density routing; pure-Sitka exact path; focused diagnostics/tests.
- **Exclusions:** No DEC, DMC, mixture-productivity bonus, invented broadleaf SDI/self-thinning equations, or silent Sitka changes.
- **Owner / routing:** DeepSeek implementation; Claude independent review.
- **Dependencies:** PARITY-01, PARITY-02, spec APPROVED
- **Verification:** pure-Sitka arithmetic/anchors exact; mixed fixtures demonstrate class effects; G3 measurement integrity.
- **Stop conditions:** implementation cannot preserve exact pure-Sitka behaviour or requires an unapproved quantitative broadleaf equation.
- **Completion state:** `MIXED DENSITY PARITY QUALIFIED`.

## PARITY-04 — Adult / canopy / site parity

- **Status:** TODO
- **Goal:** Close accepted Oak/Beech adult species-trait gaps without creating parallel ecology.
- **Scope:** species-sensitive adult growth/site/canopy/light/competition traits identified by PARITY-01 and accepted in PARITY-02.
- **Exclusions:** no unrelated growth-model rewrite; no storm policy change.
- **Owner / routing:** DeepSeek implementation; Claude review for ecology/calibration.
- **Dependencies:** PARITY-02, PARITY-03 where density interaction applies
- **Verification:** focused species fixtures, pure-Sitka regression, evidence labels retained.
- **Stop conditions:** required coefficient remains unsupported/unaccepted.
- **Completion state:** `ADULT SPECIES PARITY QUALIFIED`.

## PARITY-05 — Regeneration / juvenile / browsing parity

- **Status:** TODO
- **Goal:** Close accepted species-sensitive early-life and browsing differences.
- **Scope:** establishment, juvenile growth/survival/promotion, browsing/protection behaviour where PARITY-02 identifies material species differences.
- **Exclusions:** no new understorey system, fencing feature, browse-history persistence or unrelated ecology expansion.
- **Owner / routing:** DeepSeek implementation; Claude ecology review.
- **Dependencies:** PARITY-01, PARITY-02
- **Verification:** species-specific fixtures across natural/planted paths; existing Sitka/model-0/reference compatibility as applicable.
- **Stop conditions:** work requires a new ecology mechanic outside P0 rather than a species-aware parameter/policy within the accepted architecture.
- **Completion state:** `EARLY-LIFE SPECIES PARITY QUALIFIED`.

## PARITY-06 — Oak/Beech timber assortment and economics

- **Status:** TODO
- **Goal:** Implement D-062 with a broadleaf-appropriate product/value path.
- **Scope:** Oak/Beech assortment eligibility; form/quality/history inputs that already exist authoritatively; species/assortment values; Work Plan/timber settlement integration where required.
- **Exclusions:** no simple Sitka price multiplier; no invented Irish market precision; no new quality state solely to make pricing convenient unless separately approved.
- **Owner / routing:** DeepSeek implementation; Claude evidence/economy review.
- **Dependencies:** PARITY-01, PARITY-02
- **Verification:** Oak/Beech do not use Sitka assortment boundaries/prices; deterministic yield/value fixtures; economy viability measurement.
- **Stop conditions:** required value/threshold lacks accepted evidence/calibration or requires new persisted quality/history state not approved.
- **Completion state:** `BROADLEAF TIMBER ECONOMY QUALIFIED`.

## PARITY-07 — Player-facing species parity

- **Status:** TODO
- **Goal:** Make material species differences understandable to the player.
- **Scope:** inspection/teaching/Work Plan/timber copy identified in PARITY-01; evidence-limit wording; species-specific explanations.
- **Exclusions:** no broad UI redesign; no unsupported management-prescription claims.
- **Owner / routing:** Claude or MiMo copy/layout proposal; implementation by DeepSeek/Claude where code/UI integration is needed.
- **Dependencies:** PARITY-03–PARITY-06 enough to establish actual behaviour
- **Verification:** rendered 720p/1080p where UI changes; copy review against research/Decision Log; no pseudo-empirical wording.
- **Stop conditions:** copy would assert unsupported Irish/UK practice or hide calibration/transfer status.
- **Completion state:** `PLAYER-FACING SPECIES PARITY QUALIFIED`.

## PARITY-08 — Save / deterministic compatibility

- **Status:** TODO
- **Goal:** Prove parity changes preserve history and deterministic/reference contracts.
- **Scope:** schema decision; old/missing-field saves; malformed values if new state exists; Legacy40; Reference Future v1; pure-Sitka anchors; mixed candidate anchors if needed.
- **Exclusions:** no reserialization of Reference Future v1 as integrity proof.
- **Owner / routing:** DeepSeek implementation/verification; Claude independent review for save/reference risk.
- **Dependencies:** PARITY-03–PARITY-06
- **Verification:** Tier 2 save/load/determinism set; G5 if schema changed.
- **Stop conditions:** compatibility requires silent migration/rebaseline.
- **Completion state:** `SPECIES PARITY COMPATIBILITY PASS`.

## PARITY-09 — Integrated parity qualification

- **Status:** TODO
- **Goal:** Qualify the combined three-species implementation against the milestone and spec.
- **Scope:** requirements checklist; pure/mixed fixtures; player-facing review; save/load; economy; long-run Scenario One completion; Multi-Scale Forestry Management Sitka-only guard; protected references.
- **Exclusions:** no Windows external-demo build.
- **Owner / routing:** DeepSeek/local integrator for Unity qualification; Claude independent review; Overall Manager G4 decision.
- **Dependencies:** PARITY-03–PARITY-08
- **Verification:** Tier 2 combined qualification, escalating to Tier 3 only for the later full Scenario One integrated candidate.
- **Stop conditions:** protected anchor movement, completion/economy break, or spec-conformance failure.
- **Completion state:** `THREE-SPECIES PARITY P0 VERIFIED`.

## Task summary

| Task | Status | Owner / routing | Depends on |
|---|---|---|---|
| PARITY-01 | TODO | MiMo/Claude read-only | — |
| PARITY-02 | TODO | Claude/Deep Research + Manager | PARITY-01 |
| PARITY-03 | TODO | DeepSeek + Claude review | 01, 02, APPROVED spec |
| PARITY-04 | TODO | DeepSeek + Claude review | 02, 03 as applicable |
| PARITY-05 | TODO | DeepSeek + Claude review | 01, 02 |
| PARITY-06 | TODO | DeepSeek + Claude review | 01, 02 |
| PARITY-07 | TODO | Claude/MiMo + implementation owner | 03–06 |
| PARITY-08 | TODO | DeepSeek + Claude review | 03–06 |
| PARITY-09 | TODO | local integrator + Claude + Manager | 03–08 |
