# S1-C-IMPL-02 — Reroute Handoff

**Date:** 2026-10-27  
**Issued by:** Overall Manager  
**Task:** `S1-C-IMPL-02 — Annual Review year browsing requalification and query-layer wiring`  
**Status:** **REROUTE REQUIRED — PRIOR ATTEMPT STOPPED CLEANLY WITH NO PRODUCTION CHANGES**

## Dispatch instructions

**Send this handoff to:** **TTree Code — local Unity-capable Game Dev implementation lane**.  
Use a dedicated worktree for `feature/s1-c-history-ui-v1`. Do not send this back to a GitHub-only/browser session that cannot inspect the checked-out T-01 source and run Unity 6000.6.0f1.

**Branch:** `feature/s1-c-history-ui-v1`  
**Expected starting HEAD:** `cf7eb613594d742dab778008e7f41156c0152eda`  
**Task packet:** `Docs/Handoffs/S1-C-Implementation-02/TASK-PACKET.md`  
**Dependency base:** `808d8165d242da7632f37fea5c1d7355fa59279c`  
**Primary production file:** `Assets/ForestPrototype/UI/AnnualReviewView.cs`  
**Narrow secondary file only if genuinely required:** `Assets/ForestPrototype/UI/ScenarioOneUiRoot.cs`

**Return token:**

`S1-C YEAR BROWSING IMPLEMENTED / REQUALIFIED — READY FOR MANAGER REVIEW`

and publish `Docs/Handoffs/S1-C-Implementation-02/HANDOFF.md` on the implementation branch.

## What happened in the prior attempt

The previous implementation chat completed preflight and correctly stopped before editing. It reported:

- branch `feature/s1-c-history-ui-v1` at packet commit `cf7eb61`;
- no production changes;
- no unverified commit;
- no integration or verification claim;
- blocker: its GitHub-connector-only environment could not conclusively inspect the T-01 API/type contract and could not run the required Tier-2 Unity/rendered verification.

This is an **environment/capability stop**, not a product/spec ambiguity and not an implementation failure. No product decision is required.

## Manager-provided T-01 contract — inspect and use, do not reinvent

The T-01 source is canonical on the dependency lineage and must be inspected directly in the local checkout before editing.

### Manager accessor

`Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.History.cs` exposes:

```csharp
public IReadOnlyList<YearHistoryView> HistoryYears =>
    ScenarioManagementHistory.BuildYearViews(
        managementEvents,
        annualReports,
        ecologicalSnapshots,
        deadwoodRecords,
        workOrders,
        stormEvents);
```

`HistoryYears` is oldest-first and is the T-02 authority for completed-year navigation and selected-year history records.

### `YearHistoryView`

`Assets/ForestPrototype/ScenarioOne/ScenarioManagementHistory.cs` defines `YearHistoryView` with at least:

- `int Year`
- `ScenarioAnnualReport Report`
- `bool YearAdvancedRecorded`
- `List<ScenarioManagementEvent> Events`
- `List<ResolvedWorkRecord> ResolvedWork`
- `List<ScenarioOneWorkOrder> ResolvedOrders`
- `List<HistoryCancellation> Cancellations`
- `List<StormEventRecord> Storms`
- `List<ScenarioDeadwoodRecord> DeadwoodFallen`
- `ScenarioEcologicalSnapshot Snapshot`
- `ScenarioEcologicalSnapshot PreviousSnapshot`
- `SnapshotDelta Delta`
- `YearMoneySummary Money`
- `GroupedHarvestSettlement HarvestSettlement`
- `int OtherDeathsInYear`
- convenience filters over `Events` for created/approved/resolved/stock-purchase events.

`ScenarioManagementHistory.BuildYearViews(...)` returns one view per completed year, oldest first. `BuildYearView(..., int year)` returns one completed-year view or null for a year that was never completed.

Do not independently rejoin raw `AnnualReports`, `ManagementEvents`, `WorkOrders`, `EcologicalSnapshots`, storms or deadwood in the UI where `YearHistoryView` already supplies the selected-year relationship.

## Execution requirements

Follow the existing task packet unchanged except for this clarified routing and API contract. In particular:

1. refresh live `Docs/Project/coordination/active-tasks.md` before writing;
2. verify branch/worktree ownership and preserve unrelated dirty work;
3. inspect the actual checked-out T-01 source before editing;
4. adopt/requalify the inherited year browser rather than rebuilding it;
5. use `HistoryYears` as navigation authority and selected-year source;
6. preserve latest-year-first behavior and current-year acknowledgement semantics;
7. browsing older years must remain read-only and must not acknowledge the current review;
8. do not implement T-03 or later S1-C tasks;
9. acquire/release the machine-wide Unity lock for Unity execution;
10. run the task packet's Tier-2 bounded verification, including at least three completed years, acknowledgement gating, no-work/work years, compile/import, and rendered Annual Review at 1280×720 and 1920×1080;
11. publish the worker handoff on GitHub with exact final HEAD and evidence locations.

## Stop conditions

Stop before editing or before further writes if:

- live coordination shows another writer owns `AnnualReviewView.cs`;
- the checked-out T-01 source differs materially from the contract above;
- satisfying T-02 would require changing save/schema/objective semantics, ecology/economy, Rack V2, curriculum, or any T-03+ behavior;
- the Unity lock is owned by another task;
- the exact candidate cannot be qualified on the local Unity lane.

Return a precise blocker rather than inventing signatures or weakening tests.

## Manager state

The prior no-change attempt is accepted as a clean capability stop. It does not invalidate T-01, the T-02 packet, or the S1-C approved spec.

No user product decision is required.

**DASHBOARD SYNC: NOT AVAILABLE**  
**Published dashboard reflects this task: NO**
