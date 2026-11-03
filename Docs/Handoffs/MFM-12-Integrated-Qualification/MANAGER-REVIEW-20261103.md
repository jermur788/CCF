# MFM-12 — Integrated Qualification — Manager Review — 2026-11-03

**Claim:** #225  
**Parent:** #91 / Browser Overall #200 / local orchestration #203  
**Qualification branch:** `qual/mfm-12-integrated-v2`  
**Exact production candidate qualified:** `e42bb9f437e4fcf20c5b753dacd1136470daf33e`  
**Published qualification tip integrated:** `12a95ec5cd1f0a892e37e9c1d492c55247fbc047`  
**Manager verdict:** **APPROVED**

## Decision

The fresh full MFM-12 rerun is accepted.

**Product token issued:**

`MULTI-SCALE FORESTRY MANAGEMENT V1 — INTEGRATED / READY FOR USER PLAYTEST`

This token closes the MFM V1 feature-level qualification gate and releases the final Scenario One Tier-3 qualification. It does **not** itself mean the complete Scenario One candidate is ready for the user's personal playtest. Scenario One still requires the final Tier-3 complete-loop gate on one exact integrated lineage.

## Why MFM-12 passes

The returned qualification is a fresh A–P rerun from the exact post-#230 candidate rather than a continuation of the stopped first attempt. The qualification branch has no production/package/project-settings drift from the exact candidate; it adds verification tooling and durable evidence/review documents only.

Accepted evidence includes:

- offline matrix complete: 14/14 steps pass, with the three previously accepted pure-Beech reduced-simulator R2 misses retained as an explicit limitation against the unchanged 0.40 criterion;
- MFM save mutation controls: 25/25 mutants killed;
- Unity matrix complete: 17/17 steps pass;
- MFM-07 approval boundary: 106 checks;
- MFM-08A coverage-world gate: 56 checks;
- MFM-06 current-save gate: 159 checks;
- frozen Reference Future v1 archive/preview/continuation gate: PASS;
- save hardening / malformed-save rejection-before-mutation: PASS;
- MFM-09 extension gate: 261 checks / 16 shots;
- MFM Treatment Review + fit replay: 412 checks / 29 shots;
- MFM-11 exact-candidate calibration/complete-loop witness: 47 checks / 2 shots;
- guided-v2: 535 PASS / 0 FAIL, with the earlier blank-capture set superseded by a receipted 47-image contentful rendered recheck at both target resolutions;
- Combined HUD: 465 PASS / 0 FAIL / 24 shots;
- PARITY-07 rendered: 1031 PASS / 0 FAIL / 36 shots;
- interaction, MixedDensity Legacy40, BroadleafTimber Legacy40, BroadleafTimber Enlarged80 and ScenarioHistory Legacy40 gates all pass.

The complete-loop MFM witness reaches year 25 completion with 8/8 objectives, includes a later intervention at year 17, ordinary Work Plan commitment/execution, and durable prescription provenance. The repaired source-sample fit replay is present in the live flow with agreement/disagreement information, inspectable disagreement context and refusal-without-mutation when the current fit becomes invalid.

Current save-schema v24 behavior, old/pre-feature compatibility, malformed-save rejection-before-mutation, Legacy40 and frozen Reference Future protection are accepted for this gate.

## Independent review

Round 1, Claude Sonnet 5.5, returned `NEEDS CHANGES` for three evidence/receipt issues only:

1. the first guided-v2 PNG set was blank despite the in-memory gate passing;
2. the cleanup/lock receipt was stale while the replacement capture run was still active;
3. the draft wording pre-asserted an independent approval that had not yet happened.

No production/spec/save/calibration/protected-anchor/player-judgement defect or stop condition was found.

The qualification worker corrected the evidence package without changing production: the guided-v2 rendered recheck produced 47 contentful captures with a binding receipt; the lock was released and current cleanup state re-established; and the verdict wording was corrected.

Claude's quota was then exhausted. OpenAI GPT-6.1-Sol performed the bounded delta confirmation and returned **APPROVED**, independently confirming all three blocking evidence findings were resolved and finding no new blocker.

The substitution is accepted because the substantive architecture/spec review was already completed by the cross-provider Claude round, while the second review was limited to confirming the corrected evidence/receipt delta.

## Accepted limitations / non-blockers retained

The following are retained and must not be silently promoted to stronger claims:

- the three pure-Beech reduced-simulator R2 misses remain an accepted C-level limitation against the unchanged 0.40 criterion;
- MFM-11 extension-state calibration remains a **GAMEPLAY CALIBRATION** result, not empirical forestry precision;
- post-treatment source-vs-current-Covered CI parity remains **NOT DEMONSTRATED**;
- F11 candidate workload remains a human/user-test obligation;
- the live fit fixture contains a player-only disagreement but no live rule-only case; rule-only presentation is covered by pure logic, not a separate rendered witness;
- the invalid-fit card / `FitUnavailable` branch is not separately rendered/live-executed as a distinct branch; the current stale-sample path still proves wider approval refusal with no mutation;
- no single MFM-prescription world demonstrates two-cycle Annual Review/history; repeated-cycle/history obligations are supported by the accepted S1-D history/repeated-intervention lineages plus the MFM complete-loop/provenance witness;
- fit results are recomputable from persisted authoritative inputs rather than separately persisted;
- personal player workload/usability remains for the user's playtest.

These limitations are accurately disclosed and do not invalidate the accepted MFM V1 product requirements or the final-gate release.

## Integration decision

The complete qualification branch was a clean descendant of the exact candidate and contained no production drift, so its verification tooling and durable evidence records were fast-forward integrated onto `integration/scenario-one-player-test-ready-v1` before this Manager review.

#225 may be closed as Manager-approved.

The next executable milestone is the **final Scenario One Tier-3 complete-loop qualification** on the exact post-review integration lineage. That final gate must integrate the already accepted Rack V2, three-species parity, S1-C review/history, S1-D repeated interventions and MFM V1 requirements on one candidate, including the mandatory final-candidate S1-C T-05 save/history rerun and rendered/interaction evidence.

## Build / user gate

No Windows build is authorised.

Only a successful Tier-3 result may issue:

`SCENARIO ONE PLAYER-TEST CANDIDATE READY — USER PLAYTEST REQUIRED`

The user must personally playtest that same exact candidate before any Windows build can be authorised.

Research Source Consolidation #194 remains **NO ACTION**.
