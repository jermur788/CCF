# Scenario One Review / Management History — Requirements / Acceptance Checklist

- **Spec ID:** SPEC-scenario-one-review-history-v1-V1
- **Spec status:** APPROVED (Manager review `65d354a`; decisions R-1…R-4 and save-version sequencing binding)
- **Checklist owner:** named at implementation dispatch
- **Implementation under test:** <branch / HEAD / evidence root>
- **Date:** <YYYY-MM-DD>

Use this checklist against the finished implementation. A checked item needs evidence, not assertion. Unchecked items with a stated reason are honest; unexplained gaps are not.

## Functional requirements

- [ ] FR-1: Year-browsable review history extending the Annual Review (every completed year reachable; no second history system) — evidence:
- [ ] FR-2: Intervention record shows work type, year, targets (per FR-8), immediate recorded result, execution method, status/failures — evidence:
- [ ] FR-3: Year record shows the annual report content and snapshot change vs previous year — evidence:
- [ ] FR-4: Provenance fidelity — derived/reconstructed values labelled; absent provenance shows "not recorded", never fabricated; derived objective years only where exact (R-1) — evidence:
- [ ] FR-5: L1/L2/L3/L4 layers distinguishable and labelled in every history surface — evidence:
- [ ] FR-6: Consequence framing only where the simulation records causality; otherwise co-occurrence wording — evidence:
- [ ] FR-7: Bounded prior-treatment guidance reachable from a reviewed intervention/year; grounded in L1+L2; optional waypoint leads to the recorded location — evidence:
- [ ] FR-8: Aggregate-first detail with drill-down to recorded targets (R-3) — evidence:
- [ ] FR-9: Grouped-harvest money shown as job settlement; annual-report totals authoritative per year; `harvestJobId` shown as a year label only — evidence:
- [ ] FR-10: Objective achieved years recorded at first achievement in new games and shown in history; pre-feature saves follow R-1 (labelled exact derivation only where evidence-complete, otherwise "not recorded"); `outcomeYear` consistent — evidence:
- [ ] FR-11: Storm history uses recorded `stormEvents`; reconstructed victim counts labelled; severity bands described as game bands — evidence:
- [ ] FR-12: History is append-only; no record rewriting by the review feature; new facts written at the time they occur — evidence:
- [ ] FR-13: Session-only facts labelled "not recorded after load" and not persisted (R-2) — evidence:
- [ ] FR-14: S1-D data contract (§9) documented and guarded; no competing identity scheme — evidence:
- [ ] FR-15: UI stays within the bounded (non-analytics) presentation — evidence:
- [ ] FR-16: No dependency on unimplemented Multi-Scale / Rack V2 state for the basic history requirement — evidence:

## Player / user workflow

- [ ] The intended workflow (review latest year; browse earlier years; drill into an intervention; reach next-decision guidance) is reachable and understandable.
- [ ] Required feedback, affordances and failure/empty/edge states behave as specified (no-work year; pre-feature save; post-load session-only facts).
- [ ] The implementation does not add unapproved player-facing behaviour.

## Authority and evidence

- [ ] Implementation follows the approved spec and plan.
- [ ] Evidence classifications in the spec remain accurate (guidance vs measured; game bands; design targets).
- [ ] No unlabelled pseudo-empirical coefficients or invented ecological claims were introduced (including L4 copy).

## Save / backward compatibility

- [ ] Save impact matches the spec: only the approved additive `objectiveAchievements` field (empty-default, no retro-writes), using the **next available save-schema version on the integration base** (recorded in the handoff; never a reused/owned version — Rack V2 reserves v21).
- [ ] Old saves, missing fields and Reference Future behaviour are correct (v20 and older load unchanged; archive hashes untouched; no retrospective history writes).
- [ ] No unapproved migration or schema change.

If not applicable, state why:

## Security / write-back

- [ ] History surfaces are read-only over saved records; no write-back; frozen Reference archive never re-serialized; absent history fails toward "not recorded".

If not applicable, state why:

## Acceptance criteria

- [ ] AC-1: Any completed year shows recorded work (type/when/where/result/status) from saved records — evidence:
- [ ] AC-2: Earlier years reachable in play after ≥ 3 advanced years; latest-year content preserved; two-resolution rendered check done — evidence:
- [ ] AC-3: One rendered example per content layer, with labels; session-only facts labelled — evidence:
- [ ] AC-4: Genuine pre-feature (v20) save shows no invented history — conditional rule verified: exactly derivable objective years appear labelled derived; non-derivable ones show "not recorded"; no approximate year presented as fact — evidence:
- [ ] AC-5: Prior-treatment guidance walkthrough on ≥ 2 interventions; no new simulation claims; waypoint correctness where offered — evidence:
- [ ] AC-6: Objective achieved years recorded at the time and survive save/load; `outcomeYear` consistent — evidence:
- [ ] AC-7: Grouped-harvest money never shown as authoritative per-order fact — evidence:
- [ ] AC-8: Opening history mutates nothing; save/load round-trip of history lists is stable — evidence:
- [ ] AC-9: Old-save compatibility and Reference Future v1 integrity gates pass unchanged — evidence:
- [ ] AC-10: S1-D contract documented; identity/retention stability checks pass — evidence:
- [ ] AC-11: No analytics-grid/export/scoreboard surface; multi-year view is at most simple trend context — evidence:

## Spec conformance

- [ ] Implementation matches the approved spec.
- [ ] Approved deviations are recorded in `spec.md` and the handoff.
- [ ] Resolved decisions R-1…R-4 (`spec.md` §15) are implemented as specified; any deviation is recorded, not hidden.

## Verdict

- **Spec conformance:** PASS / FAIL / NOT VERIFIED
- **Implementation correctness:** PASS / FAIL / NOT VERIFIED
- **Notes:**
