# Scenario One — Multi-Year Decision and Outcome Feedback V2 Requirements / Acceptance Checklist

- **Spec ID:** SPEC-scenario-one-multiyear-decision-outcome-feedback-V2
- **Spec status:** DRAFT — MANAGER REVIEW REQUESTED
- **Checklist owner:** Overall Manager
- **Implementation under test:** none — no implementation exists
- **Date:** 2026-11-12 (HP #409 refinement)

A checked item needs evidence, not assertion. **Nothing in the implementation sections is checked, because nothing has been built or run.** The only checked lines are three preconditions that other tasks completed, each with its evidence and who produced it. IDs refer to [spec.md](../spec.md); task IDs to [tasks.md](../tasks.md).

## Preconditions

- [x] Static source audit of the existing Annual Review, Work Plan boundary and absence of per-rack history — evidence: [#394 audit](https://github.com/jermur788/CCF/blob/docs/scenario-one-multiyear-feedback-source-audit-20261112-v1/Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/MULTIYEAR-FEEDBACK-SOURCE-AND-UX-DELTA-AUDIT.md) (browser worker; source reading only).
- [x] Manager chose Option B — evidence: [#400 disposition](https://github.com/jermur788/CCF/blob/docs/manager-part5-source-review-20261112-v1/Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/MANAGER-PART5-BROWSER-400-SOURCE-AND-DRAFT-SPEC-DISPOSITION-20261112.md). A design direction, not a UI release.
- [x] Read-only helper qualified **offline** — evidence: [#406 result](../../../Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/SOL-MANAGEMENT-FOLLOWON-READOUT-SOURCE-QUALIFICATION-RESULT.md) and [Manager verdict](https://github.com/jermur788/CCF/blob/docs/manager-part5-source-review-20261112-v1/Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/MANAGER-406-SOURCE-QUALIFICATION-INDEPENDENT-REVIEW-20261112.md): 53 assertions PASS, type binding 0 errors, worker-reported with committed logs. Not re-run by #409. Not a Unity result.
- [ ] Spec `APPROVED` by the Overall Manager with the six open decisions answered (T-00).
- [ ] Cadence lane release of `AnnualReviewView.cs` recorded; base HEAD and single UI owner recorded (T-01).
- [ ] Helper blob on the base is `2c7bdb3c6b6f13790a325ebabc187c497b335b52` (T-01).
- [ ] Helper's 53 offline assertions and type binding re-run unchanged on the base (T-02) — evidence:

## Functional requirements

Offline — checkable by the T-03 fixture:

- [ ] FR-4: only rows of the viewed year are counted; succeeded by task type, one failed count, one cancelled count; no pending, approved or proposed work — evidence:
- [ ] FR-5: ambiguous-identity and unresolved rows are excluded from counts and shown on their own lines — evidence:
- [ ] FR-6: observations appear only for exact recorded years; both years named; no fallback year — evidence:
- [ ] FR-7: the five stand differences use helper values unmodified, with units and elapsed years — evidence:
- [ ] FR-8: four species measures kept separate; a species missing in one year reads not recorded, never zero — evidence:
- [ ] FR-9: each of the six state rows produces its exact wording — evidence:
- [ ] FR-11: fixed caveat sentence present; no causal wording anywhere — evidence:
- [ ] FR-12: no currency in any row — evidence:
- [ ] FR-14 (part): inputs unchanged after the mapper call — evidence:

Source review — checkable from the T-04 diff:

- [ ] FR-1: card built inside the existing scroll area; no new screen, header or footer content — evidence:
- [ ] FR-3: one `Build` call is the only data source; no read of the live forest, `ScenarioInterventionHistory`, `Delta` or `PreviousSnapshot` — evidence:
- [ ] FR-14 (part): `Build` called only inside the change-key guard — evidence:
- [ ] Changed paths are exactly those the plan allows — evidence:

Unity — checkable only by T-05:

- [ ] FR-2: year buttons, title, footer rule and Back to forest behave as before; an earlier year does not acknowledge the latest — evidence:
- [ ] FR-10: provenance words visible and following the Show data sources toggle — evidence:
- [ ] FR-13: identical card content after one-year and long advances — evidence:

## Player / user workflow

- [ ] The card is reachable by pressing Earlier year and understandable without other screens — evidence:
- [ ] Empty, missing and ambiguous states read as specified — evidence:
- [ ] No unapproved player-facing behaviour was added — evidence:

## Authority and evidence

- [ ] Implementation follows the approved spec and plan.
- [ ] The spec's offline-tested / Unity-unverified table has been updated to match what was actually run.
- [ ] No ecological claim, coefficient, survival figure, area or causal statement was introduced.

## Save / backward compatibility

- [ ] No save, event, hash or archive change (diff evidence) — evidence:
- [ ] A current save, a genuine older save, Legacy40 and frozen Reference Future v1 behave as before — evidence:
- [ ] No migration or backfill.

## Security / write-back

Not applicable: the feature reads records and writes nothing. FR-14 covers the only obligation.

## Acceptance criteria

- [ ] AC-1: no completed year → review unchanged, no card — evidence:
- [ ] AC-2: latest year → single line; footer works as before — evidence:
- [ ] AC-3: earlier year, both snapshots → counts, two dated observations, five differences, elapsed years — evidence:
- [ ] AC-4: every other FR-9 state shows its wording and no substitute number — evidence:
- [ ] AC-5: ambiguous and unresolved rows only on their own lines — evidence:
- [ ] AC-6: species rows correct for Oak, Beech and Sitka spruce where recorded — evidence:
- [ ] AC-7: same events, saved state and world hash with and without the card — evidence:
- [ ] AC-8: same card content after one-year and long advances — evidence:
- [ ] AC-9: readable at 1280×720 and 1920×1080; footer and year buttons never displaced — evidence:
- [ ] AC-10: current save, genuine older save, Legacy40 and Reference Future v1 unchanged — evidence:
- [ ] AC-11: no currency, causal, survival, area, score or interval wording — evidence:
- [ ] AC-12: card counts equal Work column counts on every fixture year — evidence:

## Spec conformance

- [ ] Implementation matches the approved spec.
- [ ] Approved deviations are recorded in `spec.md` and the handoff.
- [ ] Unresolved requirements are listed, not hidden.
- [ ] Independent review and Manager verdict recorded (T-06); no Windows or Player build without direct user authorisation.

## Verdict

- **Spec conformance:** NOT VERIFIED — no implementation exists
- **Implementation correctness:** NOT VERIFIED — no implementation exists
- **Notes:** Draft refined by HP task #409 on 2026-11-12. Documentation only.
