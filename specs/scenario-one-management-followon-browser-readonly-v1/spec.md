# Scenario One — Browser-safe management follow-on read-only core V1

**STATUS: APPROVED FOR SOURCE-ONLY IMPLEMENTATION / NOT IMPLEMENTED / NOT VERIFIED.**  
**Decision authority:** D-073, D-074, D-076, D-077 and accepted S1-C/S1-D/MFM V1; prepared by Manager #200, 2026-11-12. **This is a deliberately narrow approved slice**, not blanket approval of the larger [D-073 outcome-feedback V2](https://github.com/jermur788/CCF/tree/docs/manager-cadence-review-disposition-20261112-v1/specs/scenario-one-multiyear-decision-outcome-feedback-v2), [D-074 spaced-major-intervention objective](https://github.com/jermur788/CCF/tree/docs/manager-cadence-review-disposition-20261112-v1/specs/scenario-one-spaced-major-intervention-learning-v1) or [D-076 repeated representative-rack management](https://github.com/jermur788/CCF/tree/docs/manager-cadence-review-disposition-20261112-v1/specs/scenario-one-multicycle-representative-rack-management-v1), all still **DRAFT** for their larger implementation.

## Purpose

Provide a **pure, deterministic, read-only source helper** that assembles the truthful facts needed by all three follow-on features **without writing gameplay state**: selected management year, recorded completed/failed work, relevant existing prescription/coverage unit provenance, genuinely recorded later stand/species observations, and cautiously identified **candidate** stand treatments. This reduces integration work when the time-advance UI lane finishes and avoids building a second history store, unsafe save changes or fabricating rack-by-rack historical ecology.

**Allowed production source changes, on the approved new browser branch ONLY:**
- Create **one** new file `Assets/ForestPrototype/ScenarioOne/ScenarioOneManagementFollowonReadout.cs` with non-mutating data-transfer/derived-view classes/functions using **existing** `YearHistoryView`, `ResolvedWorkRecord`, `ScenarioManagementEvent`, `ScenarioOneWorkOrder`, `ScenarioEcologicalSnapshot`, `ScenarioSpeciesOutcome`, `ScenarioManagementHistory` and `ScenarioInterventionHistory`. Choose the smallest API matching actual code; no Unity UI inheritance or MonoBehaviour.
- Create narrowly scoped **offline/source tests and verification runner files** under `Tools/Verification/ManagementFollowonReadout/` (or existing equivalent verified by source) without weakening any existing test. If the runner can actually be executed safely in the browser environment, record exact command/results; otherwise label tests **WRITTEN / NOT RUN**.
- Produce durable Markdown source, test, parity and gap handoff under `Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/`; update the **three larger DRAFT four-file specs** on the dedicated browser branch as *proposals*, without changing their status to APPROVED.

**Not writable under this slice:** `ScenarioOneManager*`, `ScenarioOneCompletionRules`, `ScenarioCycleState`, `ScenarioInterventionHistory`, `ScenarioManagementHistory`, `ScenarioEcologicalSnapshot`, save/migration/hash/reference files, `WorkPlanView`, `AnnualReviewView`, `ScenarioOneUiRoot`, other UI, existing MFM geometry/calibration/proposal files, existing tests/fixtures. Those are owned by established systems and/or active #397. The browser must not compile by changing protected files.

## Confirmed source and truth layers

Current C-01 isolated source contains:
- `ScenarioManagementHistory.BuildYearViews` and `YearHistoryView` for **L1 recorded facts**, yearly order resolution, storm, yearly report and **exact-year** ecological snapshot; `SnapshotDelta` is an existing derived comparison of **the immediately preceding available snapshot**, not proof of causal outcome.
- `ScenarioInterventionHistory.Build` already returns **before/after/latest stand-level** snapshot summaries, including cautious death counts. **Its `After` may fall back to the next available later snapshot**, so never label such a value as the actual intervention-year condition if the matching year's snapshot is missing.
- `ScenarioOneWorkOrder` records `prescriptionId`, `prescriptionCoverageUnitId`, `prescriptionCropTreeId`, `prescriptionReason` and `rackId`; `rackId=0` is not proof of a mature stand silvicultural treatment. **MFM proposals are not recorded work.**
- `ScenarioEcologicalSnapshot` has **stand/species aggregates only** (canopy, light, living trees, occupied natural-regeneration map cells, planted juvenile counts). No historical per-rack canopy or recruitment series. Natural modelled cohort occupancy is neither individual seedling count nor proven survival/recruitment.
- `ScenarioInterventionHistory` uses **any successful work** as an "intervention". A separate strict definition of an achieved **spaced major-stand objective** remains unapproved; read-only view may report candidate felling facts but may **not** claim a new objective is achieved.

## FR-B1 — Read-only selected-year / later-year comparison

Take **already-built** authoritative `IReadOnlyList<YearHistoryView>` or equivalent source records and a selected completed year. Return explicit statuses: `Recorded`, `NotRecorded`, `CannotCompare` (or comparable typed alternatives). Use only **exact** saved snapshot years. Expose the observation year and source/units for each metric; if a selected or later snapshot is missing, **do not synthesize it from future/current data**. No implied causal link from treatment to result. If no later recorded year exists, return a meaningful “latest not later” state rather than a zero change.

## FR-B2 — Historical work and source attribution

Use existing `YearHistoryView.ResolvedWork` and, where appropriate, `ScenarioManagementHistory.DescribeWork`. Include only actual succeeded/failed/cancelled work statuses, faithfully distinguishing completed from order approved and proposal. For **successful executed** work with a valid preserved order and non-empty `prescriptionCoverageUnitId`, group by **year, prescription ID and coverage unit ID**. Preserve order/task identities and report missing link or unknown historical origin. Never infer **rack ID** from the coverage-unit ID's string format unless the accepted `ScenarioOneCoverage.Area` identity contract explicitly validates it. A source rack may be known from the existing durable prescription record **only when that record is actually supplied**; otherwise mark missing rather than assign one.

## FR-B3 — Major-treatment *candidate* observations, not new success rule

Label **recorded successful FellTree work** separately from planting, protection, pruning, salvage, access clearance and other maintenance. Distinguish `rackId>0` rack-owned access felling from ordinary felling as far as surviving order evidence permits; if incomplete/mixed, give `NeedsClassification` rather than treating it as a qualifying major management treatment. Do not change `HasReviewedTwoCycleSequence`, add timed gates, rename existing Intervention ordinals or decide exact multi-year educational interval. A conservative read-only `possible stand-treatment year` can support the later D-074 **DRAFT**, but is not an accepted forest-practice classifier.

## FR-B4 — Honest young-growth and species labels

For annual stand/species snapshot comparisons, use same-unit fields only; separate per-species `regenerationCells`, `regenerationDensity`, `plantedJuveniles` and `plantedRegenerationCells` as **modelled quantities**, not survival of individually traced natural seedlings. Do not sum or treat cross-origin quantities as equivalent. Missing fields/version/year → no comparison. No claim of exact local shade, terrain, soil, wind protection, site heterogeneity or intervention-caused regeneration.

## FR-B5 — All future workflows remain player-governed

This helper **never** marks a Fell tree, selects Crop Trees, changes rack plan, generates/approves a prescription, adds Pending work, approves a Work Plan, advances the clock, acknowledges an Annual Review, writes a save, updates objectives or mutates the passed-in lists. No new save/event/goal field, schema-version bump, hash change, automatic treatment, or UI change. Keep Legacy40, frozen Reference Future v1 and user personal playtest untouched.

## FR-B6 — Determinism, tests, scope and evidence

Stable sort by explicit year, event ID and order ID; avoid dictionary ordering leaks and silently skipped duplicates. Test missing/duplicate work IDs, unknown orders, old-year gaps, orphaned prescription IDs, source-year/future observation mismatch, mixed executed and pending orders, multiple prescriptions/units, no-work year, natural/planted species fields, backward-compatible no-prescription saves, stable repeated reads and **no mutation**. Record that plain source review is **NOT compiled/NOT Unity verified** unless an actual tool run proves otherwise. Produce a small integration contract showing how the **later** Annual Review might consume the helper **after #397** without writing that UI now.

**Stop and publish [BLOCKED], not unapproved source edits**, if actual source requires a new saved field, changes to `ScenarioManagementHistory` or `ScenarioOneManager`, a historical per-rack metric, major objective gate or UI action. Those need a separately accepted four-file implementation spec/Manager disposition.

## Acceptance

Manager may approve **source design only** once all new-file changes are inspectable and have a GitHub handoff; actual compiler, offline fixture and Unity verification still follow before integration. A returned browser report must distinguish source IMPLEMENTED, source REVIEWED, actual TESTED, INTEGRATED and user ACCEPTED. Production default workflow of the larger three features is **not** complete from this source helper alone.
