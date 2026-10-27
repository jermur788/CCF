# HANDOFF — S1-C Implementation 01: Per-Year History Query Layer and Contract Foundation

**Exit statement: `S1-C HISTORY QUERY FOUNDATION IMPLEMENTED — READY FOR MANAGER REVIEW`**

## IMPLEMENTATION HANDOFF

- **Task:** `S1-C-IMPL-01 — Per-year history query layer and contract foundation` (packet `Docs/Handoffs/S1-C-Implementation-01/TASK-PACKET.md`, packet commit `9674c54`)
- **Worker:** T3 Code / opencode-go mimo-v2.6-pro (MiMo, bounded C# / offline-test lane)
- **Role:** Primary (APPROVED SPEC IMPLEMENTATION — approved spec `SPEC-scenario-one-review-history-v1-V1`, `specs/scenario-one-review-history-v1/`, approved at spec-branch HEAD `8120a6793e6842cb9771299ebe5bca9305735a39`)
- **Branch:** `feature/s1-c-history-query-v1`
- **Base:** `c6342f79a3e89c040fadd232017a937132779385`
- **Context commit:** `eb218eab8b8e200a81cc99bfef7e8906bc21a88d`
- **Final implementation HEAD:** `3f0f94fc3af48ea70863e62c1ae38f9b57a58ac7`
- **Published branch HEAD:** the commit containing this handoff (see publication record)

## WHAT CHANGED

Implemented approved S1-C **T-01 only** (plus its minimum contract foundation): a read-only per-year history query layer over the existing persisted records, extending — not replacing — the derived history architecture the lineage already has. Nothing is persisted; no save field, no schema bump; reading history cannot mutate world or save state.

### Files / symbols changed

| File | Change |
|---|---|
| `Assets/ForestPrototype/ScenarioOne/ScenarioManagementHistory.cs` | **NEW.** Per-year query model + builder: `HistoryLayer` (Recorded/Derived), `YearHistoryView`, `HistoryCancellation`, `ResolvedWorkRecord`, `SnapshotDelta`, `YearMoneySummary`, `GroupedHarvestSettlement`, and `ScenarioManagementHistory` (`BuildYearViews`, `BuildYearView`, `CompletedYears`). |
| `Assets/ForestPrototype/ScenarioOne/ScenarioManagementHistory.cs.meta` | NEW Unity meta (GUID `a53010f938004b5ca73eed79a14b1e83`), per repo practice for new production scripts. |
| `Assets/ForestPrototype/ScenarioOne/ScenarioInterventionHistory.cs` | Small documented compatibility edit only: `OtherDeaths` and `FelledTargetsByYear` changed `private` → `internal` so the per-year layer shares this parsing instead of duplicating it. **No behaviour change** to `Build`, `StaleOrders`, `Completion` or `NextStepHint`. |
| `Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.History.cs` | Added read accessor `HistoryYears` (passes the manager's own persisted lists, including `stormEvents`, into the query builder). Existing `Interventions`, `StaleOrders`, `CompletionReview`, `CompletionGuidance`, `NextStepHint`, `RevalidateOpenOrders` untouched. |
| `Tools/Verification/S1CManagementHistory/` | NEW bounded offline verification (check, shims, runner). Nothing under `Assets/` is temporary; no disposable harness was installed in the Unity project. |

### SERIALIZED UNITY FILES TOUCHED

None (only the new script's `.meta`).

### PLAYER-FACING RESULT

None yet — this is the query foundation. The later Annual Review year browser (T-02) will read `ScenarioOneManager.HistoryYears` and must not re-derive raw record relationships.

### SIMULATION / SAVE IMPACT

None. Pure query construction over existing records; no schema/version change (verified by the no-mutation check below).

## How the existing architecture was extended rather than duplicated

The packet correctly noted the approved spec was drafted from an older `main` and named a possible new `ScenarioManagementHistory` helper. At base `c6342f7` the lineage already derives history:

- `ScenarioInterventionHistory.cs` — derived `InterventionSummary` per resolved plan, `StaleOrders`, `CompletionSummary`, `NextStepHint`;
- `ScenarioOneManager.History.cs` — S1-C/S1-D read access (`Interventions`, `StaleOrders`, `CompletionReview`, `CompletionGuidance`, `NextStepHint`);
- `ManagementHistoryText.cs` — player-facing wording over those summaries;
- `Tools/Verification/ThreeSpecies/OfflineLogic/ScenarioHistoryOfflineCheck.cs` — offline coverage of that behaviour.

Extension approach taken:

1. The new `ScenarioManagementHistory` joins the **same** derived-history architecture: same persisted inputs, same "pure function of saved records" rule, no storage. It exposes the **per-year record view** underneath the existing higher-level intervention projection.
2. Shared parsing is factored, not copied: the deadwood/felled-target helpers on `ScenarioInterventionHistory` became `internal` and are called by the new layer.
3. No existing public API was deleted or renamed; `ScenarioInterventionHistory.Build` semantics are unchanged (regression-proven, see below). The S1-D partial-baseline consumers keep their APIs.

## Public / query API shape

```csharp
// ScenarioManagementHistory (static, read-only)
List<YearHistoryView> BuildYearViews(events, reports, snapshots, deadwood, orders, storms);
YearHistoryView      BuildYearView(..., int year);   // null when the year was never completed
List<int>            CompletedYears(events, reports);

// ScenarioOneManager (History partial)
IReadOnlyList<YearHistoryView> HistoryYears { get; }
```

`YearHistoryView` per completed year (annual report and/or `YearAdvanced` event):

- **L1 recorded (pass-through, stable IDs unchanged):** `Report`, `Events` (all types: order created/approved/cancelled/resolution, stock purchases, year advance), `ResolvedWork` (event + order where it still exists), `ResolvedOrders` (by `resolvedYear`), `Cancellations` (from `OrderCancelled` events where the order object is gone), `Storms`, `DeadwoodFallen`, `Snapshot`, `PreviousSnapshot`; convenience filters `OrderCreatedEvents` / `OrderApprovedEvents` / `WorkResolvedEvents` / `StockPurchaseEvents`.
- **L3 derived (labelled via `HistoryLayer.Derived`, never stored):** `Delta` (deterministic before/after subtraction), `Money` (annual-report authority + event stock sum), `HarvestSettlement` (grouped settlement metadata), `OtherDeathsInYear` (shared helper).

Grouped-harvest honesty (approved FR-9) is structural: `YearMoneySummary.Authority` points at the annual report; `GroupedHarvestSettlement` carries `ResolutionYearLabel` (explicitly the resolution year, **not** a unique job id), `SettlementEventId` (the last resolved harvest event of the year — exactly where `SettleHarvestJob` writes the settlement), whole-job `SettlementRevenueCents`/`SettlementContractorCostCents` and `GroupedOrderIds`; `ResolvedWorkRecord.MoneyIsGroupedSettlement` flags harvest events so UI cannot present them as per-order money; no per-order allocation is ever derived from the settlement. Legacy `harvestJobId == -1` orders (pre-v15 migration shape) still read as their year's group.

## Verification run / results

Commands (plain .NET SDK, temp project outside the repo; no Unity Editor):

1. `python3 Tools/Verification/S1CManagementHistory/run_offline_check.py`
   → **`S1C_HISTORY_QUERY_PASS checks=55`** — proves the packet's ten items: per-year coherence; per-year event counts/types match raw records; cancelled order visible from `OrderCancelled` although the order no longer exists; resolved retained orders associated with the correct `resolvedYear`; annual-report money is the per-year authority; grouped-harvest settlement not fabricated per order (earlier felling event keeps zero money; planting keeps per-order meaning; legacy group readable); ecological before/after/delta deterministic; layer/provenance classification deterministic; two consecutive queries equivalent with **zero record mutation**; existing `Build`/`StaleOrders`/`Completion`/`NextStepHint` expectations unchanged. The suite compiles the real `ScenarioInterventionHistory.cs`, `ScenarioManagementHistory.cs`, record classes, `StormModel.cs` and `SimulationRandom.cs` unchanged.
2. `python3 Tools/Verification/ThreeSpecies/OfflineLogic/run_offline_check.py --suite history`
   → **`THREE_SPECIES_HISTORY_PASS checks=74`** — full regression of the existing derived-history behaviour (including `ManagementHistoryText` wording) after the shared-helper visibility change.
3. `python3 Tools/Verification/CloudTypeCheck/run_typecheck.py`
   → **No error in any file this packet touched.** A/B recorded: with this change removed, the tree reports the identical 4 distinct `Painter2D` stub-coverage errors (UI map/onboarding/diagram files owned by other lanes) against a stale `baseline-prod.txt` (7 entries from an older tree). Zero typecheck delta from this change. **Not a production defect; tooling-baseline debt noted below, not fixed here (out of scope).**

**Old-save fixture:** none is readily available inside the repo (genuine saves live outside the Git tree), so no save file was fabricated and no migration was invented. Legacy readability is exercised instead through legacy-shaped records in the check fixture (pre-v15 `harvestJobId == -1` orders, `PO…` outer-tree ids, missing order records), per the packet's "do not create a fabricated migration" instruction.

**Compatibility/regression summary:** existing public history APIs unchanged and covered (74 checks); no save/schema change; no mutation during query construction; Legacy40 / pre-feature Enlarged80 / Reference Future records need no migration because the query reads only fields those records already carry.

## Mismatches between approved S1-C assumptions and the `c6342f7` lineage

1. **New helper name vs existing architecture.** The approved spec's plan section anticipated a possible new `ScenarioManagementHistory`; the lineage already had `ScenarioInterventionHistory` + `ScenarioOneManager.History.cs`. Per the packet, the existing architecture was extended; the new file is the per-year layer of that same architecture and shares its parsing helpers. No second authoritative history system exists. *(Plan §2 of the spec is effectively satisfied by extension rather than green-field addition; recorded here as the packet requests.)*
2. **Richer existing history surface than the spec's draft map.** The lineage already contains `ManagementHistoryText`, completion review/guidance (`CompletionReview`, `CompletionGuidance`, `NextStepHint`) and session-only `ScenarioOneRackReview`, plus the S1-D partial-baseline APIs. All preserved; none consumed by this packet beyond regression coverage.
3. **Rack V2 owns save v21 on this lineage.** Base already persists Rack Planning & Field Marking V2 state as **save v21** (`RackSaveData`, `rackPlanningModel`, `rackPlanConfirmed`, `nextRackId`, per-order `rackId`) — confirming the amendment packet's collision note. Any future S1-C objective-achievement persistence (T-05) **must use v22** on this lineage (next-available-version rule; no version reuse). This packet makes no schema change.
4. **Work orders carry `rackId` (v21) and `IsValidOpenFelling`.** Passed through unchanged by the query layer; identity scheme untouched.
5. **Storm input.** The manager already exposes `StormEvents`; the query takes storm records as a plain parameter like the other record lists, keeping the layer free of manager dependencies.

## MUST-NOT-CHANGE HASHES / ANCHORS

None changed. No save format, Reference Future archive content/hash, objective semantics, ecology/economy rule, or UI file was touched.

## INTENTIONALLY CHANGED HASHES / ANCHORS

None.

## SPEC-DRIVEN HANDOFF

- **SPEC ID / STATUS:** SPEC-scenario-one-review-history-v1-V1 / APPROVED
- **TASKS COMPLETED:** T-01 (history query layer) and the minimum contract foundation for T-07 (stable-ID pass-through, layer classification, grouped-settlement semantics embodied in the API).
- **SPEC DEVIATIONS:** None. The packet's authority note (extend existing `ScenarioInterventionHistory` instead of the spec plan's possible new helper) is a sanctioned implementation-shape choice recorded above; FR-1…FR-16 product requirements unchanged.
- **UNRESOLVED REQUIREMENTS:** None blocking. T-02…T-08 remain TODO.
- **ACCEPTANCE CRITERIA TESTED:** AC-8 (append-only / no mutation on read) is proven offline. AC-10 is foundation-only (contract embodied in the API shape; its documented contract table and stability assertions belong to T-07). AC-1…AC-7, AC-9, AC-11 need the UI tasks and Unity qualification (T-02…T-08) and are **not** claimed here.
- **SPEC CONFORMANCE:** PARTIAL — T-01 scope PASS (offline-proven); full spec conformance NOT VERIFIED (feature incomplete by design).

## REMAINING VERIFICATION GATES

T-02…T-08 per `specs/scenario-one-review-history-v1/tasks.md`: year-browsing UI, intervention drill-down, labelling pass, objective-achievement recording (v22 on this lineage, G2/G3/G4 save gates), next-decision guidance, contract documentation/stability checks, and Tier 2 feature qualification with the requirements checklist. **No S1-C feature qualification, integration or completion is claimed by this packet.**

## KNOWN LIMITATIONS

- Static/offline verification only; no Unity run was required or performed. Manager wiring (`HistoryYears`) is type-checked, not runtime-exercised.
- The settlement-event identification derives from record order (last resolved harvest event of the year), matching `SettleHarvestJob`'s write point; it is labelled L3 derived metadata, not a recorded job identity.
- `Tools/Verification/CloudTypeCheck/baseline-prod.txt` is stale relative to this lineage's UI additions (4 pre-existing `Painter2D` stub gaps, unchanged by this work). Tooling debt; not in this packet's ownership.
- No genuine old-save file was available in-repo; legacy-shape coverage is fixture-based (see above).

## UNRELATED WORK PRESERVED

All work in this lane's worktree only. Other worktrees/branches (spec lane, Primary coordination tree with its uncommitted files, Rack V2 lanes) untouched. No unrelated file in this worktree was modified.

## DECISIONS NEEDED

None for T-01. Manager decision deferred to T-05 planning: confirm **v22** for the additive `objectiveAchievements` field on this lineage (Rack V2 holds v21).

## WORKER ASSESSMENT

- **Ready for review: Yes** — T-01 scope is implemented, offline-proven against the packet's ten verification items, regression-clean against the existing history suite, and free of any save/UI/protected-file change.

## DASHBOARD SYNC

- **DASHBOARD SYNC: NOT AVAILABLE** (no publisher/CLI in this session; coordination record is the Manager's single-writer file)
- **PUBLISHED DASHBOARD REFLECTS THIS TASK: NO**
- **DASHBOARD SYNC FOLLOW-UP:** Manager to record this handoff in `Docs/Project/coordination/active-tasks.md` and run dashboard ingestion/publication per AgentWorkflow.

## RECOMMENDED NEXT STEP — T-02 / T-07 continuation

- **T-02 (year browsing):** build directly on `ScenarioOneManager.HistoryYears` / `ScenarioManagementHistory.BuildYearViews`; the UI must not re-join raw records. Preserve the latest-year-first flow and `annualReviewSeen` gate; display money from `YearMoneySummary.Authority`, harvest years via `GroupedHarvestSettlement` (never split per order), and label `HistoryLayer.Derived` content. Owned UI files: `AnnualReviewView.cs` (and `ScenarioOneUiRoot.cs` wiring only if needed).
- **T-07 (contract/stability):** turn `spec.md` §9 into the code-facing contract note beside `ScenarioManagementHistory` and extend `Tools/Verification/S1CManagementHistory/` with ID-format/retention assertions (counters append-only, resolved orders/events retained, no competing identity scheme); include the legacy-shape fixture as the old-save read-only check stand-in.
- **T-05 note for dispatch:** on this lineage the additive objective-achievement field must take **save v22** (Rack V2 holds v21), with the integration-base check still required at implementation time.

---

## Publication record

- Base: `c6342f79a3e89c040fadd232017a937132779385`
- Final implementation HEAD: `3f0f94fc3af48ea70863e62c1ae38f9b57a58ac7` (`history: add S1-C per-year history query layer (T-01)`)
- Published branch HEAD: the commit adding this handoff (pushed to `origin/feature/s1-c-history-query-v1`)
