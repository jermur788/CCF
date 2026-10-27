# TASK PACKET — S1-C Review / Management History — Implementation 01

**Project:** CCF  
**Manager:** ChatGPT Game Dev  
**Task:** `S1-C-IMPL-01 — Per-year history query layer and contract foundation`  
**Task class:** APPROVED SPEC IMPLEMENTATION  
**Recommended worker:** MiMo (bounded C# / offline-test lane); DeepSeek acceptable if a second implementation slot is available  
**Branch:** `feature/s1-c-history-query-v1`  
**Base:** `c6342f79a3e89c040fadd232017a937132779385`  
**Context:** `eb218eab8b8e200a81cc99bfef7e8906bc21a88d`  
**Approved spec authority:** `origin/main` at `8120a6793e6842cb9771299ebe5bca9305735a39`, `specs/scenario-one-review-history-v1/` — `APPROVED`

## Why this packet exists

The Scenario One lineage already contains `ScenarioInterventionHistory` and `ScenarioOneManager.History.cs`, which derive intervention history from persisted records. The approved S1-C spec was drafted from an older `main` and therefore names a possible new `ScenarioManagementHistory` helper.

**Do not create a second authoritative history system.** Extend/refactor the existing derived history architecture so it satisfies approved S1-C T-01 while preserving existing public behaviour and the S1-D partial-baseline APIs.

## Goal

Provide a read-only per-year history query surface over existing saved records that cleanly distinguishes recorded facts from derived comparisons and can later drive year browsing in Annual Review.

The result must remain pure/derived: reading history must not mutate save/world state and must not add persisted fields.

## Existing architecture to preserve

At the base, inspect and preserve at least:

- `Assets/ForestPrototype/ScenarioOne/ScenarioInterventionHistory.cs`
- `Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.History.cs`
- any existing `ManagementHistoryText` / history verification code
- current `Interventions`, `StaleOrders`, `CompletionReview`, `CompletionGuidance`, and `NextStepHint` behaviour unless an approved S1-C requirement explicitly requires a compatible extension

Do not delete or rename existing public APIs merely to match the draft helper name.

## Required implementation

Implement approved S1-C T-01 by extending the existing history layer with a per-year view/query model that can represent, for each completed year:

- the matching `ScenarioAnnualReport`;
- all relevant management events for that year, including order creation/approval/cancellation/resolution, stock purchases and year advance;
- resolved work orders by `resolvedYear` where the order still exists;
- cancellations that survive only in the event log;
- ecological snapshot for the year and previous snapshot needed for derived delta context;
- storm record(s) for that year;
- relevant deadwood records;
- stable IDs passed through unchanged;
- layer/provenance classification sufficient to distinguish at minimum:
  - L1 recorded historical fact;
  - L3 derived comparison/reconstruction.

The API must support later Annual Review year browsing without requiring the UI to reconstruct raw record relationships itself.

### Grouped-harvest money rule

Preserve the approved honesty rule:

- annual-report totals are the authority for per-year money;
- grouped-harvest settlement must not be represented as authoritative per-order money;
- `harvestJobId` is not a unique job identity and must not be exposed as one;
- where job-level attribution is surfaced, label/structure it as grouped settlement metadata rather than fabricated per-order allocation.

### Existing intervention history

Existing intervention summaries may remain as a higher-level projection of the same records. Prefer shared internal helpers or a clean compatible extension over duplicate parsing logic.

## Owned files / allowed changes

Primary ownership for this packet:

- `Assets/ForestPrototype/ScenarioOne/ScenarioInterventionHistory.cs`
- `Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.History.cs`
- new history-query/model file(s) under `Assets/ForestPrototype/ScenarioOne/` only if genuinely useful
- new offline verification under `Tools/Verification/S1CManagementHistory/` (or equivalently bounded new path)
- this task's handoff

Small compatibility edits in directly related history-only files are allowed if documented.

## Must not edit

Do not edit:

- Rack Planning & Field Marking V2 runtime/lifecycle files, including `Racks/RackGeometry.cs`, `RackPlanner.cs`, `ScenarioOneManager.RackPlan.cs`;
- Rack headland/fresh-world files owned by 02B-R;
- `AnnualReviewView.cs`, `ScenarioOneUiRoot.cs`, Work Plan UI or other UI — those are later S1-C tasks;
- save schema/version or `ForestSaveData.cs`;
- objective recording/evaluation semantics;
- ecology, growth, regeneration, browsing, storm mechanics, economy calculators/prices;
- teaching/curriculum files;
- Multi-Scale Forestry Management code;
- Reference Future archive/content/hashes;
- scenes, prefabs, packages, ProjectSettings;
- Windows build.

If satisfying T-01 appears to require any of those, stop and report `CROSS-LANE / SPEC BOUNDARY`.

## Compatibility requirements

- Existing `ScenarioInterventionHistory.Build` semantics must remain available unless replaced by an API-compatible equivalent.
- Existing S1-D partial-baseline consumers must not break.
- No save field or schema bump.
- No mutation during query construction.
- No competing tree/order/event identity scheme.
- Legacy40, pre-feature Enlarged80 and Reference Future records must remain readable by the query layer without migration.

## Verification

Run bounded offline verification; no broad Unity qualification is required for this packet.

At minimum prove:

1. multi-year fixture returns one coherent view per completed year;
2. per-year event counts/types match raw records;
3. a cancelled order appears from `OrderCancelled` history even though the order itself no longer exists;
4. resolved retained orders are associated with the correct `resolvedYear`;
5. annual-report money remains the per-year authority;
6. grouped-harvest settlement is not fabricated per order;
7. ecological before/after/delta references are deterministic;
8. layer/provenance classification is deterministic;
9. two consecutive history queries produce equivalent results and do not change any persisted/world records;
10. existing intervention-history behaviour remains covered/regression-tested.

If an existing genuine old-save fixture is readily available to the lane, include one read-only query check. Do not create a fabricated migration just to satisfy this packet.

## Handoff

Publish:

`Docs/Handoffs/S1-C-Implementation-01/HANDOFF.md`

Include:

- exact base and final HEAD;
- files/symbols changed;
- how the existing `ScenarioInterventionHistory` architecture was extended rather than duplicated;
- public/query API shape;
- verification commands and results;
- compatibility/regression results;
- any mismatch between approved S1-C assumptions and the `c6342f7` lineage;
- unresolved blockers;
- recommendation for T-02 / T-07 continuation.

## Exit

`S1-C HISTORY QUERY FOUNDATION IMPLEMENTED — READY FOR MANAGER REVIEW`

Do not claim S1-C feature qualification, integration, or completion. This packet implements only T-01 and the minimum contract foundation needed for later tasks.
