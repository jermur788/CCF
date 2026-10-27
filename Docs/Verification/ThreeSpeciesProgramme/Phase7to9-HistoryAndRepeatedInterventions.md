# Phases 7–9 — management history, repeated interventions, completion presentation

Scope was set by the Manager's full-loop architecture audit: S1-C and S1-D are **presentation, teaching and re-entry work**, not new simulation mechanics, a new history database or a save-schema expansion. The master packet's Phases 7–9 were simplified accordingly and not broadened.

**Status:** derivation logic and wording are executed offline against the real persisted record classes (70 checks). The manager and UI wiring are type-checked only. The in-Editor gate is written and has **never been run**. Nothing here was seen on screen.

## What the existing system already does (audit confirmed by reading the source)

Multiple interventions work mechanically: repeated work orders; marking, pruning, planting, harvest; the FirstThinning / SecondThinning / LaterThinning cost tiers; persisted `annualReports`, `managementEvents`, `ecologicalSnapshots`, resolved `workOrders`, `deadwoodRecords` and `centuryReview`. The named one-cycle assumptions, and what was found:

| Assumption | Finding | Action |
|---|---|---|
| `TutorialHint` is linear / one-shot | After the first review the in-game line was the Learning summary forever ("Learning complete · revisit with O"), saying nothing about what to do in later cycles | Rewritten: after the first review the line is a **repeatable next step derived from the recorded history**, followed by the lesson summary. Earlier tutorial branches unchanged. No gate asserts the text |
| `annualReviewSeen` sticky one-time bool | It is **not** a recurring gate. `NeedsAnnualReview` is true only until the first acknowledgement, then stays false. The acknowledge button is on the review screen, so there is no soft-lock. It does bite on the first advance even if no work was done, which the existing MenuTutorial gate deliberately covers | **Unchanged.** No new gate of any kind. The first-intervention prompt is a card, not a lock |
| `AnnualReviewView` shows the latest year only | True | Year drill-down added (◀ Earlier / Later ▶, and "Read year N" buttons from the history) |
| `EvaluateProgress` only while Active, so the outcome is sticky | By design: Completed and Failed are terminal for the save; Completed does not stop management before the Century Review year; Failed stays terminal (Manager resolution 5) | **Unchanged.** Objective targets, `ScenarioOneObjectives.Evaluate` and `SetOutcome` untouched |
| Open orders from earlier years | `ValidateOpenOrders` ran on approve, advance and load, but a plan opened years later could still show an order whose tree had since died | Orders are surfaced and revalidated on re-entry (below). Never expired or deleted |

## Architecture: derived, not stored

`Assets/ForestPrototype/ScenarioOne/ScenarioInterventionHistory.cs` is a set of pure functions over records the save already carries. Nothing is persisted, so history cannot be duplicated, is identical after any save and load by construction, and needs no schema change. **`ForestSaveData.CurrentVersion` stays 20; there is no new saved field.**

- **Intervention = a resolved work plan**: a year in which at least one commissioned task succeeded (felling, pruning, planting, protection, clearance, salvage). A year where every task failed is not an intervention (the annual review still reports it). The ordinal is a presentation count and is deliberately separate from the economy's FirstThinning / SecondThinning / LaterThinning tiers, which count felling years and are unchanged.
- Year convention (from `AdvanceYear`): the advance into year *Y* resolves the plan; its events and report carry year *Y*; "before" is the snapshot at *Y*−1, "after" the snapshot at *Y* (work, then one ecological step), "since" runs to the latest snapshot.
- Per intervention: work counts by type and species (and sheltered plantings, from the resolved orders), tasks that failed and why, cost (contractor + materials), timber income and products, net, the small-job minimum, owner minutes, closing cash, before/after/latest snapshots, and tree deaths not caused by felling (deadwood records stamped with the year, excluding that year's felling targets).
- **Why no new persisted structure is needed:** every item above is present in events, reports, snapshots, deadwood records or orders, as the offline check demonstrates by deriving all of it from those records alone and again from a field-for-field copy.
- Honest limit: persisted records do not say *why* a tree died (self-thinning, windthrow or other); the history says "tree deaths that were not felled".
- Wording is in `ManagementHistoryText` (strings only). It reports recorded change and says outright that it does not prove the work caused it.

## What the player now gets

- **Annual Review:** year drill-down; a **Management history** card (one block per intervention: work, money, "that year" before→after, "since" up to the latest year, plus a button to read that year's full review); on the review of the first intervention, a **"Your first intervention"** card that tells the player what to look for and prescribes nothing; at the year the objectives are met, a **completion summary** on top of that year's review. Past years use only snapshots recorded then (live-state browsing and shelter lines are omitted so an old date never describes today's forest).
- **Review prompting:** the review already opens after every advance. After the first intervention it carries the prompt card; later it is signposted by the history card and the repeatable hint. **No acknowledgement is required in any later cycle.**
- **Work Plan:** an "Orders needing attention" card lists open orders that carry a validation problem or were planned two or more years ago, with the reason and age. Opening the plan from the UI revalidates every open order against the forest as it is and rebuilds cached quotes. Nothing is removed, cancelled or approved automatically.
- **HUD:** "· N interventions" beside the year (recomputed only when the event list changes).
- **Completion presentation (Phase 9):** derived from existing state at the actual completion year (not year 100): objectives met; each intervention; stand start → completion (living trees, basal area, canopy, regenerating cells, deadwood, composition by species); money (starting → closing cash, timber income, work and materials, lowest cash); tree deaths not caused by felling. Explicitly "not a score"; the Century Review stays separate. **No completion rule was changed or invented**; the objective logic and targets are untouched, and verification-harness timing (`completedYear == minimumYear`, `year == 30`) was not turned into gameplay.

## Supplement acceptance checks

| Check | Result |
|---|---|
| `CurrentVersion` remains 20 | Yes; no schema or field change (offline: summary types are not serialisable; gate asserts 20) |
| No duplicate history persistence layer | Yes; nothing stored, snapshots not re-saved |
| No second scenario-state machine | Yes; reads `Outcome`/`OutcomeYear` only |
| No `annualReviewSeen` soft-lock | Yes; unchanged one-time gate, no new gate; offline hint check shows a hint for every state |
| Objective targets unchanged | Yes (not touched) |
| €2,500 minimum-job rule unchanged | Yes (not touched; displayed as a note when it applied) |
| Frozen anchors not rebaselined | Yes; nothing in the simulation changed in these phases |

## S1-D repeated interventions: what is and is not demonstrated

Demonstrated by execution (offline): the derivation treats the fourth cycle exactly like the first (no two-cycle special case), idle years add no intervention, a prune-only plan counts, a mixed plan with failures counts correctly, felled-and-left stems are not miscounted as deaths, and rebuilds are repeatable and non-mutating.

**Not demonstrated (needs Unity):** that three real cycles through the real manager produce those records and that the marks, pruning state, planting, shelters, work plan, costs, regeneration, UI and save/reload behave across cycles. `ScenarioHistoryVerification.cs` is written to check exactly that (three cycles with idle years between, an order deliberately left pending for two years and then resolved, no recurring gate, history identical after save/load, completion summary for a forced outcome) and has not been run. The existing simulation gates already cover the mechanics of each system once; this gate covers their repetition.

## Verification

```text
python3 Tools/Verification/ThreeSpecies/OfflineLogic/run_offline_check.py --suite history   # THREE_SPECIES_HISTORY_PASS checks=70
python3 Tools/Verification/CloudTypeCheck/run_typecheck.py --mode prod                      # TYPECHECK_CLEAN
python3 Tools/Verification/CloudTypeCheck/run_typecheck.py --mode tools                     # TYPECHECK_CLEAN
```

The offline suite compiles the production `ScenarioInterventionHistory.cs`, `ManagementHistoryText.cs` and the **real** `ScenarioManagementEvent`, `ScenarioAnnualReport`, `ScenarioEcologicalSnapshot`, `ScenarioDeadwoodRecord`, `ScenarioOneWorkOrder` and `ForestSaveData` against a tiny `UnityEngine` shim (`Vector3`, `Mathf`, `JsonUtility` via `System.Text.Json`). It uses the shim, not the Unity reference assemblies, because those assemblies type-check but cannot execute outside Unity. A handful of save-schema neighbours that `ForestSaveData` lists but the history never reads (browse and storm records, three scenario types) are empty stand-ins; the `ScenarioOneOutcome` enum mirrors production.

```text
CCF_STAND_GEOMETRY=0 <UNITY> -batchmode -nographics -projectPath <checkout> \
    -executeMethod ScenarioHistoryVerification.Begin -logFile <log>
# expect SCENARIO_HISTORY_VERIFY_PASS; remove the copied script and its .meta afterwards
```

Existing gates that touch changed surfaces and must still pass unchanged: `MenuTutorialVerification` (first-review gate, acknowledgement persistence), `ScenarioOneProgressVerification`, `ScenarioOneTeachingCopyVerification`, `CropTreeCompetitorVerification`/`WorkPlanResidualVerification` (UI layout; Work Plan gained one conditional card).

## Known limitations

1. Nothing was viewed on screen. Layout of the new cards, button behaviour and the 1280×720 fit are unchecked.
2. The new Work Plan card and HUD text use existing USS classes (`card`, `heading`, `muted`, `chip-fell`, `row-wrap`); no stylesheet was edited.
3. `StaleAfterYears = 2` is a [C] choice.
4. The first-review gate still triggers on the first advance even if no work was done (unchanged, deliberately).
5. "Strongly prompt" for the first intervention is a card on the review that opens automatically after the advance; there is no modal or key-press requirement.
6. Learning copy (`fell.cost`) still says "no broadleaf timber market is configured in this scenario"; that is untrue on growth model 2. It was not edited because the teaching copy is verified text; update it when model 2 is adopted for new games.
7. Mortality cause is not recoverable from persisted records.
8. `ScenarioInterventionHistory.Build` is recomputed on each read (no cache) because a cache could go stale across loads; cost is small (a few thousand records) and the UI reads it only on refresh or when the event count changes.

## Canonical updates required (not made)

- `current-milestone.md`: S1-C/S1-D are presentation and re-entry work implemented as derived history; completion presentation; remaining Unity gates.
- `unity-project-overview.md`: `ScenarioInterventionHistory`, `ManagementHistoryText`, `ScenarioOneManager.History.cs`, the new review/Work Plan/HUD elements, no schema change.
- `decision-log.md`: "intervention = resolved work plan" for presentation, kept distinct from thinning tiers.
