# Scenario One — Multi-Year Decision and Outcome Feedback V2 Specification

- **Spec ID:** SPEC-scenario-one-multiyear-decision-outcome-feedback-V2
- **Status:** APPROVED — BOUNDED D-073 ANNUAL REVIEW V2. Implementation remains gated by exact ownership, approved task packet and qualified integration base; no worker or UI file is released by this status alone.
- **Owner:** Overall Manager
- **Created:** 2026-11-12
- **Last updated:** 2026-11-12 (Manager scope and source-honesty review after HP #409)
- **Decision log references:** D-073 (product priority); D-077 (time-advance contract this feature must not disturb). D-074 and D-076 are **not** adopted here.
- **Milestone:** as set in [current-milestone.md on `main`](https://github.com/jermur788/CCF/blob/main/Docs/Project/current-milestone.md); not restated here
- **Related evidence:** [#400 Manager disposition](https://github.com/jermur788/CCF/blob/docs/manager-part5-source-review-20261112-v1/Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/MANAGER-PART5-BROWSER-400-SOURCE-AND-DRAFT-SPEC-DISPOSITION-20261112.md); [#406 offline qualification result](../../Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/SOL-MANAGEMENT-FOLLOWON-READOUT-SOURCE-QUALIFICATION-RESULT.md); [#406 Manager verdict](https://github.com/jermur788/CCF/blob/docs/manager-part5-source-review-20261112-v1/Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/MANAGER-406-SOURCE-QUALIFICATION-INDEPENDENT-REVIEW-20261112.md); [#394 source and UX audit](https://github.com/jermur788/CCF/blob/docs/scenario-one-multiyear-feedback-source-audit-20261112-v1/Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/MULTIYEAR-FEEDBACK-SOURCE-AND-UX-DELTA-AUDIT.md)
- **Supersedes / superseded by:** replaces the earlier text of this same draft; none otherwise. The narrow [read-only source contract](../scenario-one-management-followon-browser-readonly-v1/spec.md) stays the authority for the helper itself.

## Product intent

D-073 asks that the player can follow **management decision → recorded work → later observed stand → next decision**. While looking at an earlier completed year in the Annual Review, the player should be able to answer one question:

> What was actually carried out in that year, what did the stand records say at the end of it, and what do they say at the end of the latest completed year?

The answer is a set of dated records and plain subtraction. It never says that the work caused the difference.

## Problem

Verified by reading source at `0ce4793f2a2a600ea01bbcf3e9800a2ac39293f4`:

- `AnnualReviewView` already lets the player step to earlier and later completed years, shows that year's work, money and forest headlines, and holds the acknowledgement button in the modal footer, which is hidden on earlier years.
- Its only stand comparison is **the previous recorded snapshot → the viewed year** (`Standing trees {before} → {now}`). Nothing compares the viewed year with the latest completed year.
- `ScenarioInterventionHistory` is not a substitute: it counts any successful work, planting included, as an intervention, and its "after" value may come from a later year than the one named.
- `ScenarioOneManagementFollowonReadout.Build(years, selectedYear)` already assembles exactly the missing comparison from existing records and writes nothing. It is offline-qualified ([#406](https://github.com/jermur788/CCF/issues/406)) and is not wired to any screen.

The gap is presentation only. No new data is needed.

## Goals

- One compact, read-only card inside the existing Annual Review (Manager's **Option B**).
- Exact-year records only, with every missing or ambiguous record named as such.
- No change to how the Annual Review is opened, navigated, acknowledged or closed.

## Non-goals

- No second timeline or history screen, and no change to the Work Plan, which stays the only approval gate.
- No new event, target, mark, order, save field, snapshot store, hash input or acknowledgement, and no auto-acknowledgement.
- No per-rack, per-unit or per-tree historical ecology. The save holds stand and species aggregates only.
- No causal wording, survival rates, hectares, scores, recommended intervals or "major intervention" classification.
- No old-save backfill and no substitute values from another year or from the live forest.
- No money in the new card. The existing money column and harvest settlement box remain the financial display.
- No D-074 spaced-intervention objective and no D-076 MFM extension. `PossibleTreatmentYears` and `ExecutedCoverage` from the helper are **not displayed** in this slice.
- No change to the helper, to `ScenarioManagementHistory`, to the cadence controller or to completion rules.

## User / player workflows

1. The player opens the Annual Review as today. It opens on the latest completed year, Year B.
2. On Year B the card is a single muted line saying this is the latest year and that an earlier year can be compared with it. The acknowledgement footer is unchanged.
3. The player presses **◀ Earlier year**. The view shows Year A. The card now shows Year A's resolved work, the stand recorded at the end of Year A, the stand recorded at the end of Year B, and the differences.
4. The player may open **By species** inside the card, step to other years, or leave with **Back to forest**. Nothing is saved or acknowledged by looking.

Year meaning, stated in the card: a year's work is resolved while advancing into that year, and its snapshot is taken after that year's ecological step. "Year A" therefore means **the stand at the end of Year A, after that year's work** — not a before-work picture.

### Illustrative layout — text only

Placeholders are in `{braces}`. This is a wording and ordering sketch. It is **not** a Unity screenshot, has not been rendered, and its fit at 1280×720 is unverified.

```text
YEAR {A} COMPARED WITH LATEST YEAR {B}
[DERIVED]       Recorded outcomes in Year {A}: {n} tree(s) felled · {n} planted · {n} crop tree(s) pruned
                · {n} windthrow stem(s) salvaged · {n} young-tree removal job(s)
[DERIVED]       {n} task(s) failed · {n} order(s) cancelled
[RECORDED]      Stand recorded at the end of Year {A} and at the end of Year {B}, {B−A} year(s) later.

                                      End of Year {A}   End of Year {B}   Difference [DERIVED]
Standing trees                        {a}               {b}               {±d}
Basal area                            {a} m²/ha         {b} m²/ha         {±d} m²/ha
Mean canopy cover                     {a} %             {b} %             {±d} points
Regeneration map squares (modelled)   {a}               {b}               {±d}
Fallen deadwood                       {a} m³            {b} m³            {±d} m³

▸ By species                                                             (closed by default)
    {Species}: standing trees {a} → {b} ({±d})
               natural regeneration map squares (modelled) {a} → {b} ({±d})
               planted regeneration map squares (modelled) {a} → {b} ({±d})
               planted young trees (counted) {a} → {b} ({±d})
    {Species}: [NOT RECORDED] not recorded for one of the two years; no difference shown.

These are recorded stand totals for two years. They do not show that Year {A}'s work caused the difference.
For what to inspect before deciding again, see the prior-treatment guidance in this review and walk the stand.
```

## Functional requirements

Requirement IDs replace the earlier M1–M7.

- **FR-1 Placement.** The card MUST live inside the existing Annual Review scroll area. It MUST NOT add a screen, tab, modal or timeline, and MUST NOT be placed in the header or the acknowledgement footer.
- **FR-2 Navigation and acknowledgement unchanged.** Earlier/later year buttons, `ShowLatest`, `TryShowYear`, the title text, the footer's visibility rule and **Back to forest** MUST behave exactly as before. The card MUST NOT call `AcknowledgeAnnualReview`, and viewing an earlier year MUST NOT acknowledge the latest year.
- **FR-3 Single data source.** All card content MUST come from one `ScenarioOneManagementFollowonReadout.Build(manager.HistoryYears, viewedYear)` result. The card MUST NOT read the live forest, `ScenarioInterventionHistory`, `YearHistoryView.Delta` or `PreviousSnapshot`.
- **FR-4 Selected-year outcomes.** `Result.RecordedWork` covers every completed year, so the card MUST use only rows whose `Year` equals the viewed year. It shows counts of recorded `Succeeded` event outcomes by task type, one count of `Failed` and one of `Cancelled`, subject to FR-5. These counts are **DERIVED from events**, not proof of verified individual tree or order identity, major intervention or causation. Pending, approved and proposed work MUST NOT appear.
- **FR-5 Fail closed on provenance.** Rows with `Link == AmbiguousIdentity` MUST be excluded from every outcome count and shown as `{n} work record(s) have ambiguous provenance and are not counted.` Rows with `Result == Unresolved` MUST also be excluded and shown as `{n} work record(s) have no recorded outcome.` If a row has both conditions, show it **only once in the ambiguous group**. Recorded succeeded/failed event outcomes whose `Link` is `MissingOrder` or `InconsistentOrder` may enter the event-outcome counts **only as recorded event outcomes**, accompanied by a `[NOT RECORDED]` warning that original order/target linkage cannot be verified; they cannot prove linked tree work, treated coverage or a major intervention.
- **FR-6 Exact years only.** The card shows the Year A observation only when `SelectedState == Recorded` and the Year B observation only when `LatestState == Recorded`. It MUST name both years. It MUST NOT fall back to Year B−1, the next later snapshot or the current forest.
- **FR-7 Differences and units.** Differences appear only when `ComparisonState == Recorded`, using the helper's `StandComparison` values with presentation-only unit conversion: stored canopy values are fractions, so display the A/B observations as percentages (value × 100) and `MeanCanopyChange × 100` as **percentage points**, matching existing Annual Review logic. Keep the helper's original units for standing trees, basal area (m²/ha), modelled occupied regeneration map squares and fallen deadwood volume (m³). Show the elapsed years. Do not mutate source values.
- **FR-8 Species rows.** Per species the card shows standing trees, natural regeneration map squares (modelled), planted regeneration map squares (modelled) and planted young trees (an exact count). These four MUST stay separate and MUST NOT be summed. A species with `State == NotRecorded` MUST read as **missing for one of the two years**, identifying which if possible from the copied selected/latest species lists. Never imply it was absent in both or substitute zero. Species are named through the existing `ScenarioOneUiFacts.SpeciesName`; whichever of Oak, Beech and Sitka spruce the snapshots hold are shown.
- **FR-9 State wording.** Each helper state maps to exactly one presentation:

  | Helper state | Card shows |
  |---|---|
  | No completed year (`LatestCompletedYear < 0`) | No card. The existing empty-state line is unchanged. |
  | Viewing the latest year (`ComparisonState == LatestNotLater`) | One muted line: `Year {B} is the latest completed year. Choose an earlier year to compare it with Year {B}.` No table. |
  | `SelectedState == NotRecorded` | Work lines, then `[NOT RECORDED] No stand snapshot was recorded for Year {A}. Nothing is compared.` |
  | `LatestState == NotRecorded` | Work lines, Year A observation if recorded, then `[NOT RECORDED] No stand snapshot was recorded for the latest year, Year {B}. Nothing is compared.` |
  | `ComparisonState == CannotCompare` | Work lines, then `[NOT RECORDED] The records for these years are ambiguous, so no comparison is shown.` |
  | No work rows for the viewed year | `No recorded work-resolution or cancellation events for Year {A}.` Do not infer that all historical work is known. |

- **FR-10 Provenance words.** Every displayed value carries correct provenance: `DERIVED` for event-outcome counts and arithmetic differences, `RECORDED` for copied snapshot values or individual recorded events, and `NOT RECORDED` for missing or unverified provenance. The card MUST respect the existing **Show data sources** toggle in the same way as the rest of the review.
- **FR-11 Caveat and guidance.** The card ends with the fixed caveat sentence shown in the layout. It adds no new advice text; it points to the existing prior-treatment guidance card. No sentence may state or imply cause.
- **FR-12 No money.** The card shows no currency. `RecordedWork.GroupedHarvestMoney` is not displayed.
- **FR-13 Same answer after either advance.** For the same saved history the card content MUST be identical whether the years were reached by repeated one-year advances or by a D-077 long advance. The session-only skip summary is separate and unaffected.
- **FR-14 Read-only and bounded cost.** Building the card MUST NOT mutate any manager, history, order, event or snapshot object. `Build` MUST run only on the existing change-key-guarded refresh path, not every frame.

**FR-9 precedence:** No completed year → no card. For latest-year selection the one-line latest-year state takes precedence. For earlier years, `CannotCompare` (duplicate year/species identity) takes precedence over missing-snapshot warnings; otherwise report selected missing, then latest missing, then the recorded comparison. Missing-work notices are supplementary, not competing snapshot states.

## Authority / source of truth

- **Accepted product intent:** D-073 in the [Decision Log on `main`](https://github.com/jermur788/CCF/blob/main/Docs/Project/decision-log.md) (canonical context `3f4e852e9579de7335e6d71ca660ecd2b0f8b561`); Manager's Option B choice in the #400 disposition.
- **Implementation facts:** source read at `0ce4793f2a2a600ea01bbcf3e9800a2ac39293f4` — `Assets/ForestPrototype/UI/AnnualReviewView.cs`, `Assets/ForestPrototype/ScenarioOne/ScenarioManagementHistory.cs`, `ScenarioEcologicalSnapshot.cs`, `ScenarioOneManagementFollowonReadout.cs` (blob `2c7bdb3c6b6f13790a325ebabc187c497b335b52`). The pinned base predates D-073 in its own Decision Log copy, so decisions are read from `main`.
- **Not inspected as a base:** the cadence candidate `e759193354b4613f88e3370a62ccaccf02c556cf` adds a session-only skip summary label and 15 lines to `AnnualReviewView.cs`. That diff was read; the candidate was not otherwise reviewed and its Unity qualification ([#408](https://github.com/jermur788/CCF/issues/408)) had not returned when this draft was written.
- **Spec authority:** none while `DRAFT`. If `APPROVED`, task-level authority for this scope only.

## Evidence classification

| Statement | Status | Source |
|---|---|---|
| Helper returns exact-year observations, `LatestNotLater`, `NotRecorded` and `CannotCompare` states; never borrows another year | **Offline-tested** | #406: 53 assertions PASS, worker-reported with committed logs, Manager-approved for offline scope |
| Duplicate event or order identifiers, mismatched or missing tree targets and conflicting species fail closed | **Offline-tested** | #406, including two guard-removal mutations that failed as intended |
| Natural cells and density stay separate from planted cells and exact planted juveniles; a species missing on one side is `NotRecorded` | **Offline-tested** | #406 |
| Helper binds to real production type signatures (191 sources, 0 errors) | **Offline-tested** against Player assemblies; **not** a Unity compile | #406 |
| Helper compiles and runs inside Unity 6000.6 | **Unity-unverified** | — |
| Any part of the card: placement, wording fit, scroll, footer priority, 1280×720 and 1920×1080 | **Unity-unverified** — nothing exists to test | — |
| Behaviour on a genuine old save, Legacy40 or frozen Reference Future v1 | **Unity-unverified**; #406 used synthetic legacy records only | — |
| Identical card content after one-year and long advances | **Unity-unverified**; depends on #408 parity results | — |

HP task #409 re-ran none of these. It read the source, the #406 result and the committed evidence.

Ecology: the card introduces no ecological rule, coefficient or claim. Regeneration figures are simulation abstractions (occupied model cells), labelled "modelled" and never presented as seedlings, survival or area.

## Save / backward compatibility

No save impact — the feature reads existing records and writes nothing. Save version, world hash, event numbering and Reference Future archives are untouched. A save that lacks a snapshot, report or order shows the matching FR-9 line. Old records are never repaired, completed or inferred.

## Security / write-back

Not applicable — no write-back, input parsing or trust boundary. The only obligation is FR-14.

## Acceptance criteria

- **AC-1** With no completed year the Annual Review is unchanged and shows no card.
- **AC-2** On the latest year the card is the single FR-9 line; the acknowledgement footer shows and works exactly as before.
- **AC-3** On an earlier year with both snapshots recorded, the card shows the FR-4 counts, both dated observations, the five FR-7 differences with correct sign and unit, and the elapsed years.
- **AC-4** Each remaining FR-9 row appears, with its exact wording, for a fixture that produces that state, and no number is shown in place of a missing record.
- **AC-5** Ambiguous and unresolved rows are excluded from counts and warned without double counting; event-only outcomes lacking consistent retained-order linkage carry the FR-5 uncertainty warning.
- **AC-6** Species rows follow FR-8 for Oak, Beech and Sitka spruce where recorded; a species absent in one year reads as not recorded.
- **AC-7** Stepping earlier and later, closing, reopening and acknowledging produce the same events, saved state and world hash as the same actions without the card.
- **AC-8** The card content for a given saved history is identical after one-year advances and after a long advance.
- **AC-9** At 1280×720 and 1920×1080 the card is readable, scrolls with the rest of the review, and never hides or displaces the acknowledgement footer or the year buttons.
- **AC-10** A current save, a genuine older save, Legacy40 and frozen Reference Future v1 load and behave as before; missing older records show FR-9 gap lines.
- **AC-11** The card shows no currency and no causal, survival, area, score or interval wording.
- **AC-12** On complete, unique, internally consistent histories, the card's event counts match the Work column **for task categories that both calculate from the same events**. The Work column also contains report-derived regeneration removals; this source must not be asserted equal to a missing event count. On ambiguous, missing-order or inconsistent histories, mismatches are expected and MUST be explained by FR-5 warnings rather than hidden or forced equal.

## Stop conditions

Return to the Manager rather than deciding locally if:

- any FR needs a change to the helper, to `ScenarioManagementHistory`, to a save or event type, or to any file other than those named in the plan;
- the card cannot fit at 1280×720 without displacing the footer or dropping an FR-9 state;
- AC-12 fails on a provenance-consistent real save, or a malformed record differs without a matching FR-5 warning;
- the cadence lane has not released `AnnualReviewView.cs`, or the integration base does not contain helper blob `2c7bdb3c…`;
- a requirement seems to need per-rack history, a causal statement or D-074/D-076 behaviour.

## Manager decisions — resolved 12 November 2026

1. **Scope:** Approve Option B, a single small read-only card inside the existing Annual Review. This is SPEC APPROVAL, not implementation or a Unity pass.
2. **Latest-year view:** show one muted discovery line, with no comparison table when the selected year is the latest completed year.
3. **Felling wording:** use neutral **“felled”**, not “thinned”, within this new card. Existing Work-column “thinned” groups rack-access felling too and is not an authoritative silvicultural classification. No edit to that existing column is authorised here.
4. **Work counts:** keep two concise **DERIVED recorded-event outcome** lines, with the FR-5 uncertainty warnings and the source-aligned AC-12 comparison. Do not describe these counts as verified work on a specific retained tree.
5. **Deferred measures:** leave out mean DBH, mean light, deadwood piece count, regeneration density and per-species basal area from the first slice.
6. **Deferred detail:** per-coverage-unit executed-work grouping and candidate treatment years remain outside this feature, awaiting the independent D-076 and D-074 approvals.

The Manager additionally corrected canopy fraction-to-percentage-point display, the meaning of a missing species on one side of a comparison, event-count provenance, FR-9 state precedence and the unsafe assumption that every Work-column category is event-counted. All changes are scoped to the spec: no source/Unity implementation or save alteration.
