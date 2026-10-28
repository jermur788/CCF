# S1-D T-02 — Advisory cycle/readiness state — Handoff

**Task:** `S1-D-T02 — derive advisory cycle/readiness state from existing persisted records` (claim #72)
**Packet:** `Docs/Handoffs/S1-D-Implementation-02/TASK-PACKET.md`
**Result:** `S1-D ADVISORY CYCLE STATE IMPLEMENTED — READY FOR MANAGER REVIEW`
**Mode:** code + offline verification only. **No Unity run, no Player build, no Windows build occurred or is claimed.**

## Who / where

| | |
|---|---|
| Actual model/provider | Claude Code (T3 Code harness) / Anthropic `claude-sonnet-5-5` (Sonnet 5.5), local session. Subscription account not independently verifiable from the session. |
| Branch | `feature/s1-d-cycle-state-v1` |
| Worktree | `/media/jer/Files/CCF/Operations/CCF-S1D-T02` (clean, non-Unity, created from `origin/main`) |
| Starting SHA | `85cf41d4813d2affcbf99c54668418221a286f84` |
| Coordination commit (pushed to `main`) | `cd60f189ad6f91c84f80d9b3b060afe6793bbd74` — only the S1-D-T01, S1-D-T02 and S1-C-IMPL-03 rows of `active-tasks.md`, under the bounded integrator authority in claim #72 |
| Code/verification commit | `d6c8cd54416bd23e228e214f32c8b3a29bfb2b5e` (all gates below ran at this SHA) |
| Branch tip | the commit that adds this file (docs-only on top of `d6c8cd5`) |
| Durable evidence | `/media/jer/Files/CCF/Verification/S1D-T02-CycleState-20261028/` (full logs per gate) |

Refreshed before work: `origin/main`, live `active-tasks.md` (no row owned `ScenarioOneManager.History.cs`/cycle-state files; S1-C-IMPL-01 completed), claim #72, claim #19 (`[WAITING]`, Manager hold in force). No ownership conflict.

## Changed files (vs `85cf41d`, excluding the coordination row edit)

| File | Change |
|---|---|
| `Assets/ForestPrototype/ScenarioOne/ScenarioCycleState.cs` | **new**, 196 lines, `using System*` only |
| `Assets/ForestPrototype/ScenarioOne/ScenarioCycleState.cs.meta` | **new**, hand-written minimal `.meta` (GUID `9963eda3e48649438d03b1b5eaa006fd`, checked unique under `Assets/` and `Packages/`); Unity was not run, so import has not been observed |
| `Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.History.cs` | +5 lines: one expression-bodied `CycleState` read accessor |
| `Tools/Verification/ScenarioOneRepeatedInterventions/CycleStateOfflineCheck.cs`, `run_offline_check.py` | **new** offline suite and runner |
| `Docs/Handoffs/S1-D-Implementation-02/HANDOFF.md` | this file |

Not touched: `UI/AnnualReviewView.cs`, `ScenarioManagementHistory.cs`, `ScenarioInterventionHistory.cs`, `ScenarioOneCompletionRules.cs`, `ForestSaveData.cs`, Rack/MFM files, scenes/prefabs/ProjectSettings/packages. No S1-C history-query API change was needed, so no stop for Manager API review was triggered.

## API

```csharp
public enum CycleReviewState { NoIntervention, Reviewed, NotYetReviewed, Unproven }

public sealed class CycleComponent {            // one named fact
    string Id; bool Present; int Count; string Reason /* never empty */; List<int> OrderIds /* ids only */ }

public sealed class ScenarioCycleState {
    const string AdvisoryLabel;                 // "…not a schedule, a recommendation or a requirement."
    int CurrentYear; ScenarioOneOutcome Outcome; int OutcomeYear; string OutcomeNote;
    int ResolvedInterventionCount, LatestInterventionOrdinal /*0 none*/, LatestInterventionYear /*-1 none*/,
        YearsSinceLatestIntervention /*-1 none; descriptive*/;
    CycleReviewState LatestInterventionReview;
    CycleComponent ApprovedAwaitingExecution, PendingWork, StaleOrInvalidOrders, LatestIntervention, YearsSince;
    IReadOnlyList<CycleComponent> Components;   // fixed order
    static ScenarioCycleState Derive(IReadOnlyList<InterventionSummary> interventions, events, orders,
                                     int currentYear, ScenarioOneOutcome outcome, int outcomeYear, int reviewYear);
    static CycleReviewState ReviewStateForYear(events, int interventionYear);   // the one acknowledgement derivation
}
// ScenarioOneManager.History.cs
public ScenarioCycleState CycleState => ScenarioCycleState.Derive(Interventions, managementEvents, workOrders,
                                                                  CurrentYear, outcome, outcomeYear, ReviewYear);
```

The only `bool` on the result is `CycleComponent.Present` (does this fact apply). There is no single yes/no verdict, interval, recommended treatment or gate.

## Chosen definitions (also in the code header comment and tested)

- **Resolved intervention** = one `InterventionSummary` from `ScenarioInterventionHistory.Build` / `ScenarioOneManager.Interventions` (a work-plan year with ≥1 *succeeded* task). `Derive` takes that list as input, so there is one history authority and no second parser. It is **not** `ScenarioOneCompletionRules.Plans`, which also needs a matching `Completed` order and unique event IDs. On a normal save they agree; on odd data (a succeeded event whose order record is gone) the cycle state follows the history surface. The suite pins this difference.
- **Reviewed** = an `AnnualReviewAcknowledged` event with `year >= interventionYear` and `eventId >` the last `WorkResolved` event of that year (the same year + event-ID ordering as the completion chronology and `NeedsReviewAcknowledgement`). An acknowledgement before a later plan does not cover it, so "outstanding after cycle two" is reported correctly. When event IDs are missing (≤0) or duplicated, order cannot be proven and the state is `Unproven` (no claim either way; mirrors the `Plans()` guard). This fourth state is my addition beyond the packet's "reviewed or not"; say if you would rather fold it into `NotYetReviewed`.
- **Carried over** = open order with `createdYear < CurrentYear`. Approved and pending are separate components; both annotate carried-over counts.
- **Stale/invalid** = delegates to `ScenarioInterventionHistory.StaleOrders` (the history layer's own rule; nothing is expired or removed).
- **Years since** = `max(0, CurrentYear − latest.Year)`; wording is identical at 5 and 92 years and states that observing without acting is always allowed.

## Offline verification (all at `d6c8cd5`)

| Gate | Command (from the worktree) | Result |
|---|---|---|
| **New** cycle state | `python3 -I Tools/Verification/ScenarioOneRepeatedInterventions/run_offline_check.py --out … --mutations` | `CYCLE_STATE_OFFLINE_PASS checks=93 NOT_UNITY_VERIFIED`; `CYCLE_STATE_MUTATIONS_PASS caught=7 survived=0` |
| N3 completion | `…/ScenarioOneCompletion/N3/run_n3_offline.py` | `N3_OFFLINE_PASS checks=88` (baseline 88) |
| **N3 normalised rules hash** | `source_normalization.normalized_sha256(ScenarioOneCompletionRules.cs)` and N3's own first assertion | `8166775d…80648`, equal to the pinned value; file has no diff vs `85cf41d` |
| S1-C query layer | `…/S1CManagementHistory/run_offline_check.py` | `S1C_HISTORY_QUERY_PASS checks=55` (baseline 55) |
| Scenario history | `…/ThreeSpecies/OfflineLogic/run_offline_check.py --suite history` | `THREE_SPECIES_HISTORY_PASS checks=74` (baseline 74) |
| Rack bay/sample/approach/keyboard | `…/ScenarioOneNonUi/run_nonui_offline.py` | 87 / 75 / 49 / 46 PASS (unchanged) |
| Runtime-source type-check | `…/CloudTypeCheck/run_typecheck.py --mode prod` | exit 1, same 4 `Painter2D` entries as T-01 reported; **none in `ScenarioOne/`**, so `ScenarioCycleState.cs` and the manager accessor type-check |
| Scenario One UX | `…/ScenarioOneUx/run_ux_check.py` | `SCENARIO_ONE_UX_FAIL failures=1 checks=209`: "the walking HUD has a short first-use text". **Reproduced on a pristine `85cf41d` checkout with the same result**; pre-existing, not touched or rebaselined |

Packet items 1–12 map to the suite: (1) no-intervention explanation; (2) cycles 1–4, reviewed and unreviewed, produce one identical masked signature; (3) idle years and a failed-only year add no intervention; (4) approved vs pending disjoint with different reasons; (5) carried-over visible; (6) stale/invalid equals the history rule, inputs byte-for-byte unchanged afterwards; (7) acknowledgement by year/event order (9 cases incl. before-resolution, earlier year, null entries, duplicate/zero IDs); (8) outstanding after cycle two stays advisory, with a reflection check that no public member is gate/interval/recommendation-shaped and a source scan that only the adapter and its accessor mention `CycleState` (no UI or gate consumes it); (9) years-since never changes other components; (10) deep-copy rebuild equal, inputs unmutated; (11) Completed and Failed outcomes keep cycle context; (12) no `UnityEngine`/persistence/clock/random API, no mutable static, not `[Serializable]`, `ForestSaveData.cs` has no cycle field. A final sweep also checks every reason across all 46 produced states is non-empty, ≥20 chars and free of prescriptive words.

**Negative controls (`--mutations`, copies outside the repo, each must turn the suite red):** ack ignores event order; ack ignores intervention year; hard-coded three-cycle cap (fails at cycle 4); a `ShouldIntervene` member; approved/pending merged; an empty reason; a mutated input order. All 7 caught.

## No schema / Unity / product-value change

- Diff vs `85cf41d` touches only the files listed above plus the coordination rows. `ForestSaveData`, schema version, completion rules, economy, ecology, Rack, MFM, scenes, prefabs, UXML/USS, ProjectSettings and packages are unchanged.
- No new persisted field, `PlayerPrefs`, session latch, cache or serialized component; rebuilding is a pure function of the records.
- Nothing in the game reads `CycleState`, so Approve Work Plan and Advance behaviour cannot have changed.

## Cleanliness

Worktree clean after commit (my `Tools/Verification/__pycache__` artefacts were removed; temporary base worktree for the UX reproduction was removed and pruned). The dirty `/media/jer/ZX20/CCF-Primary` checkout (`AGENTS.md`, `active-tasks.md`, two untracked docs) was not touched. Logs and build outputs live in `/tmp` and the Files evidence directory, never in the repository.

## Notes for Manager

1. **Not verified:** Unity import/compile of the new script and hand-written `.meta`; Unity's own compiler (type-check is an approximation); any rendered or runtime behaviour. No consumer exists yet; S1-D T-05 (presentation) will be the first.
2. The `Unproven` review state and the stated definition difference from `ScenarioOneCompletionRules` are the two judgement calls to confirm at review.
3. S1-C T-03 (#19) can now resume from refreshed `main` once this is integrated; this task did not need any change to `ScenarioManagementHistory.cs`/`ScenarioInterventionHistory.cs`/`AnnualReviewView.cs`, so the hold could be narrowed to `ScenarioOneManager.History.cs` only.
4. After integration, the `S1-D-T02` and `S1-C-IMPL-03` rows in `active-tasks.md` need updating (row edits for this task beyond the three bounded ones were not made).

## Remaining gate

Manager review/integration of branch `feature/s1-d-cycle-state-v1` (G4). No Unity gate is required for this task. A Unity import/compile smoke is naturally covered by the next Unity-qualified S1-D/S1-C task.

**DASHBOARD SYNC: NOT AVAILABLE**
**Published dashboard reflects this task: NO**
