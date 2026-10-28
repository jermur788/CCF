# S1-D Implementation 01 — Baseline re-verification: handoff

**Verdict: S1-D BASELINE RE-VERIFIED — T-02..T-06 MAY BE DISPATCHED**

Read-only task. No production, save, scene/prefab, Unity-asset, completion-rule, Rack-lifecycle or S1-C UI change. No Unity run and no Windows build. Every finding below is a **static reading of the Git objects at the inspected SHA plus plain-.NET offline checks**. None of it is Unity-verified.

Three points in "Notes the Manager should weigh before dispatch" (N-1, N-2, N-3) sharpen existing findings or add a Rack V2 interaction. I judged none of them a new blocker or a baseline change, because each lands inside a task that is already scoped for it (T-03, T-04, T-02). The verdict is mine to propose; the Manager can overrule it.

## Identity

- Task: `S1-D-T01`, claim issue #13 (`[RUNNING]` before inspection, to be closed `[HANDED-OFF]` at publication).
- Worker: Claude Code (T3 Code harness) / Anthropic `claude-sonnet-5-5` (Sonnet 5.5), local session, no subagents. The packet recommends Opus or the strongest reasoning model; Sonnet 5.5 is what this session actually ran, so review depth is Sonnet-grade.
- **Candidate SHA inspected: `49db765f1dd883310ae56c523d0f317a70d27353`** (`origin/main`, re-read via the GitHub API at start and equal to the local fetch).
- Branch/worktree: `handoff/s1-d-t01-baseline-reverification-v1` at `/media/jer/ZX20/CCF-S1D-T01`, created clean at the candidate SHA (no Library). The dirty `/media/jer/ZX20/CCF-Primary` checkout was not touched.
- Evidence archive (outside the repo): `/media/jer/Files/CCF/Verification/S1D-T01-Baseline-20261028/` (`run.log`, per-gate `*.out`, `run_all.sh`, `typecheck-prod.out`).

## Preconditions (all true)

| Check | Result |
|---|---|
| Live `active-tasks.md` refreshed from `origin/main` `49db765` | Done. S1-D-T01 row is `READY / DISPATCHED`, no lock conflict on the row. |
| Candidate descends from `f23549b18b17b3542ec8f633d7309b11020990d4` | Yes (`git merge-base --is-ancestor`). Packet unlock `2dd0b20` and `169970f2` are also ancestors. |
| Rack V2 core integrated on this candidate, not just a feature branch | Yes. `f23549b` is an ancestor. RACK-V2-05, 06, 07 and MFM-02 (`07eee3d`) are ancestors too. **RACK-V2-08** (`5b0ed56`, blocked at G3) is **not** in main. |
| Approved S1-C contract in canonical Git | Yes. `specs/scenario-one-review-history-v1/` is approved (`1b1bef7`). S1-C T-01 (`808d816`) and T-02 (`1a91f1c`) are in main via `169970f2`. S1-C T-03 (claim #19) is dispatched, not integrated. |
| Read-only | Yes. The worktree is clean except this file. A `Tools/Verification/__pycache__` created by my first offline runs was my own artefact and was removed. |

## Finding table (candidate `49db765`)

Statuses: `UNCHANGED`, `CHANGED BUT SPEC-COMPATIBLE`, `BLOCKER`. No finding is a `BLOCKER`.

| ID | Status | Evidence at `49db765` |
|---|---|---|
| F-1 | **UNCHANGED** (see N-1) | `LearningObjectivesView.RackPlanProblem()` (`UI/LearningObjectivesView.Sequence.cs`) is still `Bay != null && AccessOpen && !Bay.CanopyKept`, with no lesson-stage condition. `ScenarioOneUiRoot.ApproveWorkPlan` and `AdvanceFromWorkPlan` return false on it. `WorkPlanView` disables both buttons on it (lines 160-161). `RemovedBasalShare` is still cumulative over all in-footprint completed fellings, planned orders and marks (`ScenarioOneRackBay.Analyse`). `MaxRemovedBasalShare = 0.40f`. The only player-facing year-advance path is `AdvanceFromWorkPlan` (checked by grep; other callers are verification harnesses). The footprint **source** differs in V2 worlds: it is now the fixed-width 9 m teaching area on the third confirmed rack, not an authored line. The predicate and cumulative semantics are the same. |
| F-2 | **UNCHANGED** | `Analysis.HasEnoughFuture` requires `AreaCropCount == FutureTreesWanted (3)` and `Future.Count == 3`, with near, middle and far zones and both sides. `Current()` returns 1 when `Bay != null && !HasEnoughFuture` and 2 when `!CanopyKept`. `ReleaseExecuted` is still recomputed from current shares with `ExecutedTolerance 0.9`. |
| F-3 | **UNCHANGED** | `ReleaseYear` is still the latest in-footprint removal serving a Crop Tree. `ReleaseKey` is built from every in-footprint completed felling. `ScenarioOneRackReview.Synchronize` resets the session review on a key change. `Current()` returns 4 while `!ReleaseReviewed \|\| !RackReviewComplete`. (Numbering note: the code's `Current()` indexes are 0-based, so "lesson 5" in the spec is index 4.) |
| F-4 | **UNCHANGED** | `ScenarioOneRackReview` is session-only, with `Reset` on a `TeachingWorldVersion` change (incremented only at new game and load). Lesson-5 copy still says "After loading, repeat this inspection review; it is not stored in the forest save." |
| F-5 | **UNCHANGED** | `NeedsAnnualReview = AnnualReports.Count > 0 && !AnnualReviewSeen`. `annualReviewSeen` is set only in `MarkAnnualReviewSeen` and cleared only in the new-game reset, not in `AdvanceYear`. It gates the first advance only. `ShouldRecordAcknowledgement` is idempotent per year. The acknowledgement footer is display logic (`AnnualReviewView.cs:89`) and does not gate Approve or Advance. |
| F-6 | **UNCHANGED** | "Readiness" still exists only as text: `NextStepHint`, `CompletionGuidance`, `StaleOrders`, `AdvanceGuard`. No cycle-state class exists yet. |
| F-7 | **UNCHANGED** | `TreeMarkType { None, Fell, CropTree }` has no year or provenance field (no designation-year symbol anywhere in the code). `AddMarkedTreesToWorkPlan` still ends with `marking.ClearAll()`, which clears Fell marks only. `StandMapView` layers are still `Access, Light, Regeneration, Browse, Marks`. Rack V2 added a planning mode but no dated past-work layer. |
| F-8 | **UNCHANGED** | `LearningObjectivesView.Record/ReloadProgress` still use device-wide `PlayerPrefs` with prefix `CCF.Learning.v1.`. `TutorialHint` still embeds `Learning.Summary`. |
| F-9 | **UNCHANGED** | `ScenarioOneCompletionRules.cs` is byte-for-byte unchanged since `7840872`. `Plans()` still returns empty for any event ID `<= 0` or duplicated. `ForestSaveValidation` (lines 193-198, 303-306) still checks only cell index, owner minutes and execution method, with no event-ID check. Hash result below. |
| F-10 | **UNCHANGED** | `ScenarioOneManager.cs:834` counts distinct years with a succeeded `FellTree` event. `ScenarioOneEconomyAdapter.cs:164` maps 0, 1 and 2 or more to First, Second and Later thinning. |
| F-11 | **UNCHANGED** | `CanManage()` and `CanEditWorkPlan` are still `outcome != Failed && CurrentYear < ReviewYear`. `EvaluateProgress` transitions only from `Active`, so Completed does not stop management. Failed conditions are unchanged. |
| F-12 | **CHANGED BUT SPEC-COMPATIBLE** | Rack V2 persistence is present (`rackPlanningModel`, `rackPlanConfirmed`, `nextRackId`, `racks`, `ScenarioOneWorkOrder.rackId`), `ForestSaveData.CurrentVersion = 21`. Changes since `c6342f7`: (a) the open item the spec cites is now implemented. `ReopenRackPlan()` exists (RACK-V2-07, integrated) and returns the plan to unconfirmed without touching work or marks, and a rack with completed work stays locked. (b) The teaching area is now `ScenarioOneRackSampleGeometry.Derive`, the third of five confirmed racks, with a fixed 9 m half-width (MFM-02). Both are Rack V2 / MFM-owned and consistent with RD-9. See N-2. |
| R-1 | **UNCHANGED** | `ForestTreeMarkingManager` has no reference to lesson or teaching state. `ScenarioOneManager` refers to teaching only inside `TutorialHint`. The Fell mark → pending order → approve → advance path is unchanged. |
| R-2 | **UNCHANGED** | `TryPurchaseStock`, `TryDesignateExactPlanting`, `TryDesignatePruning`, `BatchPruneCropTrees`, `TryDesignateVegetationClearance`, `TryDesignateRegenerationRemoval` and salvage exist with no cycle-count condition. Only `CanManage()` and ordinary validity apply. A rack-owned order (`rackId > 0`) cannot be edited as an ordinary order (RACK-V2), which is Rack V2 behaviour, not a cycle assumption. Pruning lift limits remain unverified in code, as in the spec. |
| R-3 | **UNCHANGED** | `RevalidateOpenOrders()` is still called when the Work Plan opens (`ScenarioOneUiRoot.cs:190`). `StaleOrders` still surfaces orders with a validation message or `>= StaleAfterYears (2)`. Nothing is auto-expired. |
| R-4 | **UNCHANGED** | `ScenarioInterventionHistory.Build` is still a pure function with no two-cycle special case. All-failed years are skipped. |
| R-5 | **UNCHANGED** | `AdvanceGuard.Evaluate` warns only when pending work exists and none is approved. It is never a block and "Advance anyway" is always offered. |

## S1-C reconciliation (spec §9.4) — result: ONE derived-history authority, no duplication

- **(a) One implementation.** `ScenarioInterventionHistory` (resolved-work-plan projection) and `ScenarioManagementHistory` (S1-C per-year query layer, `YearHistoryView`) are two **layers of one derived-history architecture**. The S1-C layer states it extends the intervention history, and it calls the shared `internal` parsers `OtherDeaths` and `FelledTargetsByYear` rather than copying them. Both are static pure functions over the saved records. `ScenarioOneManager.History.cs` exposes `Interventions`, `HistoryYears`, `StaleOrders`, `CompletionReview`, `CompletionGuidance` and `NextStepHint` over them. A search of `Assets/` for History, Timeline, Ledger, Journal and Chronicle types found no other history type. There is no persisted history field in `ForestSaveData`, no second store and no second query layer. **No duplicated history UI**: `UI/AnnualReviewView.cs` is the only presenter, using both `Interventions` and `HistoryYears`. **STOP — DUPLICATED HISTORY AUTHORITY does not apply.**
- **(b) Grouping unit.** The intervention unit is the resolved work plan (a year with at least one succeeded task). Order-level records are the drill-down via `YearHistoryView.ResolvedWork`, `ResolvedOrders` and `Cancellations`. S1-D uses the unit the integrated surface exposes. See N-3(b) for a precision issue.
- **(c) Schema ordering.** The code is at `CurrentVersion = 21` (Rack V2). `ForestSaveData` has no `objectiveAchievements` field, so S1-C's additive objective-achievement record is not implemented. S1-C and MFM plans both say "next available version, no fixed number". S1-D V1 adds no field and claims no version, and this matches the spec.
- **(d) Stable identity.** Work-order ID, management-event ID, tree ID, deadwood ID and juvenile ID keys are all still exposed, so EI-1 is satisfiable. Management-event IDs are still unvalidated at load (F-9).
- **EI-5 (acknowledgement by year).** The S1-C layer has no named per-year acknowledgement accessor. `AnnualReviewAcknowledged` events only appear in `YearHistoryView.Events`, stamped with a year. `ScenarioOneCompletionRules.NeedsReviewAcknowledgement` returns false as soon as the two-cycle sequence holds, so it cannot serve later cycles. The T-02 adapter must therefore derive "latest resolved intervention reviewed" itself, which spec EI-5 allows.
- **Annual Review / completion guidance.** Annual-report access, year-browsable review, the completion summary and the next-step hint are all present on the one presenter.

## Rack V2 interaction result

- The Objective-1 rack lesson does not repeat by itself. `AccessOpen` for V2 is `RackLayout.Harvested && year > 0`. `Harvested` needs all five racks planned and every rack's work resolved, and a rack with completed work is geometry-locked, so it stays true. That stability depends on the rack lock, which Rack V2 owns. This is static only.
- **Teaching-area retarget.** The packet's follow-up (retarget Lessons 2-5 to a deterministic central confirmed rack; model 0 and Legacy40 keep the authored middle rack) is **present in code at this SHA**: `ScenarioOneRackSampleGeometry.Derive` selects the third of five confirmed racks, and `LearningObjectivesView.TeachingFacts` uses it when `RackPlanningV2Active`, otherwise the authored middle line. It was **not** Unity-verified here. `active-tasks.md` (MFM-02 row) says real player-authored Rack V2 requalification is deferred to a later integration gate, and the Rack Combined Repair row still reads "central-confirmed teaching-bay retarget required before full Objective-1 functional qualification". Treat it as present in source but not functionally qualified.
- **S1-D assumptions affected by the retarget:** F-1 (the 40 % footprint is now a fixed 9 m half-width around the third confirmed rack, not half the gap to neighbouring authored racks), F-2 to F-4 (the Crop Tree zones and `ReleaseKey` are computed on that footprint), and F-12. Nothing in the S1-D register depends on the old authored-line footprint for V2 worlds.
- **Unintegrated lane:** RACK-V2-08 (`feature/scenario-one-rack-v2-no-competing-author-v1`, blocked at G3) changes `ScenarioOneManager.cs` and `UI/LearningObjectivesView.cs` and is not in main. Its diff does not touch `LearningObjectivesView.Sequence.cs`, `ScenarioOneUiRoot.cs` or `WorkPlanView.cs`.

## Notes the Manager should weigh before dispatch

**N-1 — F-1 is sharper than the register states (static reasoning, not run).** `RemovedBasalShare = removed / (living-in-area + removed)`. A tree that dies naturally or in a storm drops out of the living term but does not enter `removed`, because `removed` needs a *completed felling order*. So a footprint that sits under 40 % after the player's felling can drift over 40 % through mortality alone. `CanopyKept` then goes false and `RackPlanProblem` stays non-empty. That disables Approve **and** Advance for every kind of work, and the Work Plan is the only player-facing way to advance a year. If the excess is all executed, there is nothing to cancel or unmark. Growth of neighbours can dilute it slowly, with no guarantee. This is the F-1 class-B problem, already scoped to T-03 / RD-5, but T-03's offline decision table should include this row. Spec §17 contains the matching T-03 stop condition ("retained hard constraint blocks all management indefinitely"). Please treat that as a reachable-by-reading concern, not a confirmed Unity repro.

**N-2 — Reopening a V2 rack plan moves the guide (static reasoning, not run).** `ReopenRackPlan()` sets `rackPlanConfirmed = false`, so `TeachingFacts` builds `bay = null`. The sequence then uses the no-bay branches: `CropTrees` becomes the global Crop Tree count, and index 4 requires `ReviewedTwoCycles && AreaReleased` instead of `RackReviewComplete`. The code comment says suspension is intended while the plan is unconfirmed or invalid. It is a new way for the guide to jump (for example from index 5 back to 4) that F-2 to F-4 do not list. The T-04 monotone latch must therefore cover the `Bay == null` state as well as the three regression paths in the register. Whether an ordinary player can reach it after work has progressed also depends on `ReopenRackPlan`'s locking rules, which are Rack V2's. T-04 must not edit those files (spec §17).

**N-3 — Precision items for T-02 / T-04.**
- (a) `LearningObjectivesView.Reviewed(year)` (private lambda in `TeachingFacts`) and `LearningObjectivesView.NeedsReviewAcknowledgement` (index 6, current-year only) are separate reads of the acknowledgement events. T-02 will add a third derivation unless it factors one.
- (b) "Resolved plan" has three slightly different definitions: `ScenarioOneCompletionRules.Plans` (a succeeded event whose completed order matches by id, year and type), `ScenarioOneCompletionGuidance.Plans` (a copy that mirrors the rules), and `ScenarioInterventionHistory.Build` (any succeeded `WorkResolved` event in the year, no order check). They agree on normal saves and differ only on odd data. The history ordinal is a presentation count, so this is not a competing authority, but the T-02 adapter must state which definition it uses for "latest resolved intervention".
- (c) `NextStepHint` is already conditional ("…if the forest needs it") and descriptive, so it is compatible with FR-D2 as written.

## Ownership / conflict notes (not blockers)

- **S1-C T-03 (claim #19, dispatched)** owns `UI/AnnualReviewView.cs` and "narrow S1-C history-query files only if required". S1-D T-02 adds a class beside `ScenarioInterventionHistory` and a read accessor in `ScenarioOneManager.History.cs`, the same files. Sequence T-02 or lock the shared files before dispatching both.
- **T-03 and T-04** edit `LearningObjectivesView.Sequence.cs`, `ScenarioOneUiRoot.cs`, `WorkPlanView.cs` and `ScenarioOneTeachingSequence.cs`. The active-tasks Rack Combined Repair row (integrated) still lists `UI/LearningObjectivesView*.cs` among files it owned, and RACK-V2-08 touches `LearningObjectivesView.cs`. Re-read the live lock table at dispatch.
- No shipped or accepted save fixture has duplicate or non-positive management-event IDs (RD-12 stop condition not triggered). Four tracked JSON evidence files contain management events (0, 1, 1 and 49 events), all with unique positive IDs, and both scene files hold `managementEvents: []`. The Reference Future archive is a Resources `TextAsset` and I did not parse it.

## Commands and checks run

All build output went outside the repo, under the evidence archive. Run from `/media/jer/ZX20/CCF-S1D-T01` at `49db765`.

| Check | Command (abridged) | Result |
|---|---|---|
| Scenario One completion N3 (offline) | `python3 -I Tools/Verification/ScenarioOneCompletion/N3/run_n3_offline.py --out …` | exit 0, `N3_OFFLINE_PASS checks=88 NOT_UNITY_VERIFIED` |
| Completion-rule hash | `source_normalization.normalized_sha256(ScenarioOneCompletionRules.cs)`, run directly and as the first assertion in N3 | `8166775d17827112282a668f8d0859d6e31c6868fa4724c77c13dbf6d6f80648`, **equal** to the pinned value |
| S1-C history query layer (offline) | `python3 -I Tools/Verification/S1CManagementHistory/run_offline_check.py --out …` | exit 0, `S1C_HISTORY_QUERY_PASS checks=55` |
| Scenario history (offline) | `python3 -I Tools/Verification/ThreeSpecies/OfflineLogic/run_offline_check.py --suite history --out …` | exit 0, `THREE_SPECIES_HISTORY_PASS checks=74` |
| Rack-bay / teaching (offline) | `python3 -I Tools/Verification/ScenarioOneNonUi/run_nonui_offline.py --out …` | exit 0, `NONUI_APPROACH_PASS 49`, `NONUI_KEYBOARD_PASS 46`, `NONUI_RACKBAY_PASS 87`, `NONUI_RACKSAMPLE_PASS 75` |
| Scenario One UX wording / rules (offline) | `python3 -I Tools/Verification/ScenarioOneUx/run_ux_check.py --out …` | **exit 1, `SCENARIO_ONE_UX_FAIL failures=1 checks=209`**, see below |
| Runtime-source type-check (non-Unity) | `PYTHONDONTWRITEBYTECODE=1 python3 -I Tools/Verification/CloudTypeCheck/run_typecheck.py --mode prod --out …` | **exit 1**, 4 "NEW" errors vs. baseline, see below |
| Static reading | Source reads of every file named in the finding table and the §9.4 greps for competing history types / stores / UIs | Recorded in the table above |

**Two non-passing gates, neither related to S1-D:**
- **UX check, 1 failure:** `FAIL the walking HUD has a short first-use text`. `MenuHelpView.Summary(Menu.WalkingHud)` now deliberately returns `null` (since the approved onboarding commit `37744f2`, an ancestor of `f23549b`). The older construction evidence log passed that assertion, so the check is stale against the onboarding change. The other 208 checks pass, including the `AdvanceGuard` and `ScenarioOneTeachingSequence` rules. I did not edit the check (read-only). A tooling fix is needed before this suite is used as a clean gate.
- **Type-check, 4 NEW errors, all `Painter2D` not found** in `UI/Map/StandMapCanvas.cs`, `UI/Map/StandMapLegendView.cs`, `UI/OnboardingIcon.cs` and `UI/TeachingDiagram.cs`. `Painter2D` is a UI Toolkit API from newer Unity than the 2021.3 reference assemblies the tool uses (the tool documents this class of difference). The baseline predates those files, which have since changed. No error is in any file S1-D reads or will edit (`ScenarioOne/*`, `LearningObjectives*`, `WorkPlanView`, `AnnualReviewView`, `ScenarioOneUiRoot`). The baseline file needs a tooling refresh; I did not update it.

## Completion-rule hash result

Normalised `ScenarioOneCompletionRules.cs` SHA-256 = `8166775d17827112282a668f8d0859d6e31c6868fa4724c77c13dbf6d6f80648` — **matches** the N3-pinned constant. The file has no diff since `7840872`. The N3 offline check (88 checks) covers the two-cycle chronology.

## New blockers or ownership conflicts

- **No new blocker. No duplicated history authority.** N-1 and N-2 are sharpened or new interactions inside already-scoped tasks (T-03, T-04). Ownership overlaps are listed above.
- Stale tooling (UX assertion, type-check baseline) is recorded for whoever next uses those suites as gates.

## Facts to correct in the S1-D spec (T-01 completion duty)

- Replace `[CODE c6342f7]` with the candidate SHA `49db765` for every register entry above that is marked UNCHANGED.
- §3 L-1 ("`main` does not contain the lesson curriculum… `CurrentVersion = 20`") is now historical. Main contains that code, `CurrentVersion = 21`, and the S1-C T-01 and T-02 history surfaces.
- F-12 / RD-9: the reopen item is implemented by Rack V2 (`ReopenRackPlan`, RACK-V2-07); S1-D still defers to Rack V2.
- F-1: add the N-1 mortality / storm drift to the register entry so T-03 starts from it.
- F-2 to F-4: add the N-2 `Bay == null` (reopened plan) guide path.

I made none of these edits (read-only task); this is a list for the Manager.

## Not verified

No Unity run, no Editor, no rendered UI, no save/load round trip, no Windows build. All behavioural claims about the live game (N-1, N-2, Objective-1 non-repetition, continued management after Completed) are static readings or offline pure-logic checks. The Reference Future archive contents and Unity-side anchor hashes were not examined.
