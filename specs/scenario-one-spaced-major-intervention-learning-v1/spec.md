# Scenario One — Spaced Major-Intervention Learning V1

**DRAFT — MANAGER REVIEW REQUESTED. NOT APPROVED FOR IMPLEMENTATION.** Revised 12 November 2026 under [#413](https://github.com/jermur788/CCF/issues/413), B0–B2. [Plan](plan.md), [tasks](tasks.md), [requirements](checklists/requirements.md).

## Authority and inspected baseline

Latest user authority and Decision Log D-074/D-077 govern intended behaviour. Locked context: `63044dfbbeb869996ceecec8a47619dcba048b0b`, verified full context-set last-change commit. Newer D-064/065/071/073/074/076/077 were read separately at [main 91511faa](https://github.com/jermur788/CCF/blob/91511faab737718f65fee896b32628a755cae90f/Docs/Project/decision-log.md). The [source audit](../../Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/FABLE-HP-D074-COMPLETION-HISTORY-SOURCE-AUDIT.md) contains labelled findings and limits.

Implementation pins are separate: cadence **E** = `e759193354b4613f88e3370a62ccaccf02c556cf`; #406 helper **R** = `0ce4793f2a2a600ea01bbcf3e9800a2ac39293f4`. #397 is CLOSED source handoff; #402/#408 subsequently qualified cadence at feature level on isolated E, not main. #406 is CLOSED / Manager offline-source approved; its helper is absent at E. This task establishes no Unity/import/runtime/rendered result.

At E, [CompletionRules.Plans / HasReviewedTwoCycleSequence](https://github.com/jermur788/CCF/blob/e759193354b4613f88e3370a62ccaccf02c556cf/Assets/ForestPrototype/ScenarioOne/ScenarioOneCompletionRules.cs#L15) accepts two distinct successful work-plan years with an acknowledgement between resolution-event boundaries. It checks unique Completed orders, matching type/year and valid global event IDs, but no minimum gap or substantive-treatment test. Consecutive planting can satisfy this sequence predicate, not all scenario objectives. History's intervention ordinal is derived presentation, not a silvicultural grade.

Orders retain rack/prescription/unit/crop provenance. Resolved events do not copy all those fields. Rack ID > 0 identifies V2 rack-owned work; zero does not prove stand treatment (legacy rack and manual corridor felling can also be zero). #406 lists candidate/rack/unclassified felling years, never grades them. Its order/event prescription-ID equality check conflicts with production resolution, which omits the ID (audit D074-06). Real MFM grouping needs separate qualification.

## Product recommendation

Teach a substantive treatment, years of stand development and reviewed observations, then a fresh justified treatment decision. Necessary intervening work remains allowed; a date never demands cutting.

D-077 preserves its navigation feature's completion predicate while requiring a **separate objective-level D-074 design**. Advisory copy alone does not fulfil that requirement. Recommend an **assessable non-gating learning objective**, with visible evidence and unknown states, preserving existing scenario completion. This recommendation requires Manager selection and is not accepted fulfilment of D-074. A mandatory new completion gate is a separate option needing explicit origin, persistence and compatibility approval.

## Proposed requirements

**FR-01 — Preserve completion.** Recommended non-gating learning must not change HasReviewedTwoCycleSequence, current objective predicates, outcome/outcomeYear, Year25 minimum, Year100 failure/Century Review, historical teaching or frozen Reference Future. Persisted Completed outcomes remain completed; Active historical worlds keep their previous completion route. Objective-achievement records are history, not live predicates. A gated alternative requires a separately approved contract.

**FR-02 — Substantive manual and MFM evidence.** A candidate treatment episode needs a bounded inspectable area, several actual retention/release decisions with a silvicultural purpose, and uniquely linked succeeded resolved work supporting that treatment. MD-02 must define exact scope and decision/quantity tests before code. One arbitrary Fell, two plan years, rack clearance or planting/pruning/protection/salvage/regeneration removal alone are insufficient. A mixed plan separates substantive work from access/maintenance rather than invalidating the entire plan. MFM must preserve actual demonstrated source/unit and execution evidence. Manual player-selected treatment needs an equivalent route without a prescription; “manual” here is tree selection, not a claim of physical manual labour execution. No universal hectares, stocking factor, BA percentage or Irish threshold is invented.

**FR-03 — Separate time, observation and review.** A tick produces a report, YearAdvanced and snapshot, not proof of player observation. Display elapsed years, available exact-year stand/species observations and actual review evidence separately. Skipped quiet years supply data but are not automatically player-reviewed. No nearest-year substitution, invented acknowledgement, inferred historical field inspection or causal claim. Approval must specify how a deliberate comparison and current reassessment are demonstrated; current acknowledgements do not prove field visits.

**FR-04 — Interval awaits choice.** Option A: at least **three elapsed years** between qualifying resolutions, two intervening annual observation records and deliberate comparison/reassessment. Option B: no numeric minimum, with an explicit observation/reassessment condition that two adjacent maintenance plans cannot satisfy. Manager must select the interval and review contract. Three is GAMEPLAY CALIBRATION, not universal Irish practice. D-077 four-year navigation/five-tick cap neither grants learning credit nor forces work. Legitimate earlier work remains possible without penalty or waiting lock.

**FR-05 — Genuine later judgement.** Later credit requires fresh live inspection, new retention/release choices and a separate succeeded episode. Another rack, an old review, date arrival, retained prescription or passage of time is insufficient. If no treatment is justified, show that honestly and allow observation; a gated alternative needs an explicit fair no-treatment progression ruling.

**FR-06 — Origin, progress and unknowns.** No durable D-074 cohort/episode marker is established. Serializer version 24, empty history, geometry, Rack V2 and first-guided-review latch cannot distinguish a new D-074 world from an old resave. MD-04 must approve origin, episode/progress persistence and monotone achieved status, including missing metadata, Active old worlds and immutable already-achieved completion. No backfilled motives, review or source identity. Until a safe contract exists, interim output is advisory/untracked and does not count as a completed objective or fulfil D-074.

**FR-07 — Honest spatial/species scope.** Stand/species differences are aggregate model observations, not historic per-rack ecology or causal effects. Occupied regeneration cells are not established future crop trees. Manual species-appropriate Oak/Beech treatment remains available; current Sitka-only automated MFM coefficients cannot transfer. Five racks are not five samples. D-076 may later provide executed-work provenance but is not required to permit the manual route.

**FR-08 — Explanation and ownership.** Existing objective/review surfaces show evidence, unknowns and next justified action, clearly separating learning, completion and navigation. No automatic marking, proposals, approval, work or acknowledgement. Preserve D-073 historical honesty, D-065 teaching, Work Plan commitment, Legacy40 and frozen reference. Future UI changes need fresh live D-073 ownership checks, not a stale “after #397” dependency.

## Manager decisions before APPROVED

| ID | Decision | Conservative recommendation |
|---|---|---|
| MD-01 | Non-gating assessed objective or new completion gate? | Choose assessed learning first; explicitly rule how it meets D-074. Advisory copy is interim only. |
| MD-02 | Substantive area, several decisions, resolution threshold and equivalent manual evidence? | Test authentic manual/MFM fixtures before accepting an inspectable gameplay contract; no candidate-count grading. |
| MD-03 | Three elapsed years or qualitative rule; exact review/reassessment proof? | Decide after examples; permit earlier work and no compulsory thinning. |
| MD-04 | Durable origin/episode/progress, old Active eligibility, achieved latch and hashes? | Independent save/history review; no inferred origin or retroactive credit. |
| MD-05 | Species/spatial fairness and no-treatment progression? | Manual alternative, no five-rack quota, no Sitka rules on broadleaves; resolve gated dead ends. |

## Future acceptance matrix

Proposed gates, **NOT RUN** here. Plan/tasks/checklist use these IDs.

| ID | Expected observable result | Requirements |
|---|---|---|
| AC-01 | Consecutive planting/maintenance and rack access retain applicable old sequence semantics but earn no substantive credit; one arbitrary Fell is insufficient. | FR-01/02 |
| AC-02 | Real manual and MFM episodes satisfy the approved evidence contract; failed/cancelled/unresolved/proposed work does not; mixed work is classified separately. | FR-02 |
| AC-03 | Annual/skip records contain no invented acknowledgements; exact observed/reviewed/missing years are honest; MD-03 boundary passes/fails correctly. | FR-03/04 |
| AC-04 | New later judgement/resolution earns evidence; stale reuse/date/review/rack change does not; no-treatment handling follows MD-05. | FR-05 |
| AC-05 | New, old Active, Completed, all-missing, duplicate-ID and reload cases preserve approved identity/outcomes, with no backfill/duplicate credit. | FR-01/06 |
| AC-06 | Year25/100, Legacy40/frozen archive and hashes remain correct; Sitka/Oak/Beech/manual routes remain fair without invented local ecology. | FR-01/07 |
| AC-07 | At 1280×720/1920×1080, learning/completion/navigation are distinct; equivalent annual/skip decisions give equivalent evidence with acknowledgement differences labelled. | FR-03/08 |

## Stops and exclusions

Unresolved MD-01…MD-05, missing evidence, protected hash/reference changes or conflicting source/UI ownership stop production release. Future schema, objectives and source identity need new bounded claims after approval and independent review. No production mechanics, Unity run, build, merge, package, scene or asset change accompanies this draft.
