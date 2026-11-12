# Scenario One — Multi-Year Decision and Outcome Feedback V2

**STATUS: DRAFT / READY FOR MANAGER DECISION — NOT APPROVED FOR CROSS-SYSTEM IMPLEMENTATION.** 12 November 2026. Product need D-073 is accepted; only the narrower [browser read-only source contract](../scenario-one-management-followon-browser-readonly-v1/spec.md) is approved. This draft does not authorise UI, persistence, ecology or objective code.

## Purpose, existing truth and precise gap

The player should answer: **What did I actually have carried out in this selected year, what did the forest record then, and how is the recorded stand different in the latest completed year?** They can then walk the current forest and decide what to do next. This follows the accepted decide → annual advance → walk → observe → decide loop. The response must distinguish historical facts, model observations, arithmetic differences and non-prescriptive teaching considerations.

Inspected C-01 source on the [dedicated browser branch](https://github.com/jermur788/CCF/tree/feature/scenario-one-management-followon-browser-v1-20261112):
- [AnnualReviewView.cs](https://github.com/jermur788/CCF/blob/integration/scenario-one-c01-research-copy-candidate-20261112-v1/Assets/ForestPrototype/UI/AnnualReviewView.cs) already browses earlier/later completed years, work drill-down, annual report, short stand history and objective history. Do not build a second timeline.
- [WorkPlanView.cs](https://github.com/jermur788/CCF/blob/integration/scenario-one-c01-research-copy-candidate-20261112-v1/Assets/ForestPrototype/UI/WorkPlanView.cs) distinguishes pending, approved and carried-over work. The Work Plan, not a management-history view, remains the commitment gate.
- [ScenarioManagementHistory.cs](https://github.com/jermur788/CCF/blob/feature/scenario-one-management-followon-browser-v1-20261112/Assets/ForestPrototype/ScenarioOne/ScenarioManagementHistory.cs#L215) exposes YearHistoryView per completed year, WorkResolved events, order records when retained, cancellations, annual report and exact snapshot (when present). Harvest-job money belongs to the annual settlement and must not be split among trees.
- [ScenarioInterventionHistory.cs](https://github.com/jermur788/CCF/blob/feature/scenario-one-management-followon-browser-v1-20261112/Assets/ForestPrototype/ScenarioOne/ScenarioInterventionHistory.cs#L80) counts **any successful commissioned work** as an intervention, even planting; its After snapshot can be the first later available year. It is not an exact-year comparison API.
- [ScenarioEcologicalSnapshot.cs](https://github.com/jermur788/CCF/blob/feature/scenario-one-management-followon-browser-v1-20261112/Assets/ForestPrototype/ScenarioOne/ScenarioEcologicalSnapshot.cs) contains annual **stand/species aggregates only**, including modelled natural cohort cells/density versus planted cohort cells and planted individual juveniles. It has no historical rack-area ecology or exact natural seedling survival chronology.
- [ScenarioOneManagementFollowonReadout.cs](https://github.com/jermur788/CCF/blob/feature/scenario-one-management-followon-browser-v1-20261112/Assets/ForestPrototype/ScenarioOne/ScenarioOneManagementFollowonReadout.cs) is the authorised NEW source-only projection of already-built history. It remains uncompiled/unqualified in this browser packet; later UI work must use it only after source review and actual qualification.

**Source gap:** the navigation and recorded facts already exist, but the causal-neutral *link between selected year's recorded work, that year's exact observation, and the latest completed year's exact observation* is not presented in one concise place. Existing graph/intervention cards do not prove a selected-to-latest same-measure comparison. Recommendation: **Option B — one small section in the existing Annual Review; no second screen**. Option A (explain existing controls only) remains a viable Manager simplification. Option C (new longitudinal per-rack/cohort persistence) is out of scope.

## Proposed player presentation (not final layout)

Within the existing Annual Review for a selected completed Year A:
- **Recorded actions · Year A:** actual successful/failed/cancelled resolved orders (not proposed/unapproved marks); show an explicit missing-order-origin label where an old record lacks its order. Display grouped harvest money only from the authoritative year report.
- **Observed after Year A:** its exact saved year-A stand/species snapshot, or **Not recorded**. A work resolution is recorded when advancing into A; this is after work and the annual ecology step, not a before-operations photo.
- **Latest recorded completed Year B:** exact saved year-B snapshot, with Year B stated. If B equals A, say **No later completed year**. If the latest completed year lacks a snapshot, say **Not recorded** rather than silently using Year B−1, the current live forest or a first subsequent snapshot.
- **Change observed across B−A years:** simple same-unit arithmetic for trees, basal area, canopy, light, modelled occupied regeneration cells and deadwood where both snapshots are exact. Treat species absent on one side as **Not recorded for comparison**, not zero. Natural regenerationCells/regenerationDensity, plantedRegenerationCells and exact plantedJuveniles stay separate, with explicit model quantities/units.
- **Consider before the next decision:** an L4 prompt to revisit actual treated and untreated places in 3D and inspect current competition, existing planting and browsing; never prescribe a universal interval or assert this intervention caused the observed changes.

Example with *placeholders*, not invented game data: “Completed work in Year A: {succeeded} succeeded, {failed} failed. Stand observation: Year A → Year B, {years} elapsed. Trees {from} → {to}; modelled young-growth occupied cells {from} → {to}. These are recorded stand totals, not change caused by this work. Walk the affected places and reassess before planning.” Keep the current primary annual acknowledgment and navigation visible at 1280×720; mock up layout before approval.

## Functional requirements (proposals)

**M1 — Existing history surface:** Enter from Annual Review; preserve earlier/later selection, year grouping, return context, previously acknowledged review state and the separate D-077 skip destination review. No new timeline owner and no auto-acknowledgment.

**M2 — L1 history:** Use only YearHistoryView recorded events/orders and annual report for actual resolution outcomes. Created/approved/prescription-proposed work is not represented as completed. Historical decisions that were never saved, selected tree reasons and exact past player motives are **unknown**, not reverse-engineered. Distinguish work resolution year from order planning year.

**M3 — L2 model state and L3 differences:** Compare *exact* completed-year snapshots only through the source helper after it is qualified, with explicit A/B years; no latest-year fallback or falsely labelled InterventionSummary.After. Modelled occupancy/density is not counted as natural seedlings, species survival, hectare coverage or realised biodiversity.

**M4 — L4 teaching:** One concise, provenance-labelled, conditional consideration may mention light, competition, planting, protection and browsing only as present-state matters to inspect. No deterministic blame, scientific confidence precision, optimal prescription or completion score.

**M5 — Real coverage attribution:** Show executed work grouped by stored resolution year + prescription ID + coverage-unit ID when authenticated order linkage exists. Separate rack-owned access felling, possible stand-felling and unknowns; never infer a source rack from an area ID string or per-rack historical ecology. The [D-076 draft](../scenario-one-multicycle-representative-rack-management-v1/spec.md) owns future multiple-source identity/coverage policy. Existing current 3D world and Work Plan still own actual effects and approval.

**M6 — Compat and no edits to authority:** No new save fields/history stores, event or order IDs, world-hash policy, ecology equations, Work Plan approvals, old-save backfill, Legacy40/Reference Future v1 changes. Preserve Si​​tka/Oak/Beech comparable representation without importing Sitka MFM coefficients into broadleaves.

**M7 — Ownership and navigation:** #397 D-077 time-advance source owns WorkPlanView.cs, AnnualReviewView.cs, ScenarioOneUiRoot.cs until Manager releases that lane. This V2 cannot be implemented concurrently in those files. Source helper alone is not this player-facing feature.

## Reconciliation / approval questions

1. Approve **B: one concise selected-year → exact latest-year comparison in current Annual Review**, or A: existing UI plus teaching copy sufficient? Choose after a real 720p mockup. Do not accept C implicitly.
2. May older years with no exact selected/current snapshot show the bare recorded work and an explicit “Not recorded” observation? **Proposal: yes; no synthetic substitutes.**
3. Is Tier 1 per-unit executed-work drill-down necessary for first release, or can it follow a stand-level chronological card? **Proposal: attach group detail only where source truly supports it; no per-unit ecological trend.**
4. Which natural-juvenile explanation is sufficiently clear without implying individual survival? **Proposal: label both natural and planted metrics as modelled recorded quantities.**

## Demonstration and acceptance gates (future)

Use one real worked year, idle year, later management year, cancellation/failure, grouped harvest settlement, orphaned order, current and v20-era old save, Sitka/Oak/Beech captured species snapshots, exact-year missing records and both D-077 time actions. Verify zero new events/acknowledgments, no mutation, correct year labels, visual 1280×720 and 1920×1080, Legacy40/Reference Future preservation and no collateral changes. Source/type check and focused Tier-2 UI evidence on one exact integration head; no redundant full Tier-3 before a new milestone candidate. **No test or feature acceptance is claimed by this DRAFT.**
