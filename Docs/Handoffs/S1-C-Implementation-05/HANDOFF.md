# S1-C Implementation 05 — T-05 objective/progression history (save v22)

**Verdict: S1-C T-05 IMPLEMENTED — READY FOR CROSS-PROVIDER SAVE REVIEW**

Static/offline only. No Unity Editor, no Unity lock, no Windows build. Nothing here is Unity-verified, integrated or accepted. Claim #104 stays open.

## Identity

- Worker: T3 Code / Claude Code, **Claude Sonnet 5.5** (Anthropic, `claude-sonnet-5-5`).
- Branch/worktree: `feature/s1-c-t05-objective-history-v1`, `/media/jer/Videos/CCF-S1C-T05`; base `aca74acacc565f72ab802177cf69ca6ebb54d637` (umbrella integration with S1-C T-03, S1-D T-06, Rack v21, PARITY-06).
- First candidate `cd7b518` (reviewed: NEEDS CHANGES, `INDEPENDENT-REVIEW.md` at `d6c5e97`). **Round-2 fix commit: see the worker report / claim #104 for the exact SHA** (a commit cannot name itself); this handoff is committed with it. Only `ScenarioOneManager.ObjectiveHistory.cs`, the Annual Review label, and the offline check changed; schema, validation position and hash-strip rules are exactly as at `cd7b518`.
- Save-version check at the integration base (done before editing): `ForestSaveData.CurrentVersion == 21` (Rack v21 final) → next available is **v22**. No other schema change is made.

## v22 diff (production)

| File | Change |
|---|---|
| `ForestSaveData.cs` | `CurrentVersion` 21 → **22**. `ScenarioOneSaveData.objectiveAchievements` (`List<ScenarioObjectiveAchievement>`) declared **last**, after the v21 rack run. New `[Serializable] ScenarioObjectiveAchievement { string objectiveId; int achievedYear; float targetValueAtAchievement; }` exactly as spec §11. |
| `ScenarioOne/ScenarioOneManager.ObjectiveHistory.cs` (new, + `.meta`) | State, `ObjectiveAchievements`, `ObjectiveAchievedYear(id)` (-1 = not recorded), `RecordObjectiveAchievements` (first achievement only), clone, reset, and the load guard below. |
| `ScenarioOne/ScenarioOneManager.cs` | Four one-line wiring changes: `CaptureSaveData` clones the list; `RestoreSaveData` calls `NormalizeObjectiveHistory` then `RestoreObjectiveHistory`; the new-scenario reset clears it; `EvaluateProgress` calls `RecordObjectiveAchievements(objectives, snapshot.year)` inside the `Active` branch, before `SetOutcome`. `outcome`/`outcomeYear` logic is untouched. |
| `ScenarioOne/ScenarioOneSaveMigration.cs` | `NormalizeObjectiveHistory(data, version)`: a pre-v22 save gets the empty list. Nothing derived or back-filled. |
| `ForestSaveValidation.cs` | `ValidateObjectiveHistory` (semantic) wired after Rack v21 and **before** the final T-06 `ValidateManagementEventIds` (which keeps its position); `ValidateObjectiveHistoryJson` (presence/type/duplicate-key). Added to this existing file rather than a new partial so that the protected harnesses that enumerate validator files (N3, EventId) compile unchanged. |
| `ForestSaveController.cs` | `ValidateObjectiveHistoryJson` appended to the existing pre-mutation `TryReadSave` refusal chain. |
| `ScenarioOne/ScenarioReferenceArchive.cs` | Hash-strip extension (below) and `Canonicalize` sorts the list by (year, ordinal id). `Load` and Reference identity checks are untouched. |
| `ScenarioOne/ScenarioManagementHistory.cs`, `ScenarioOneManager.History.cs` | `YearHistoryView.ObjectivesAchieved`; `BuildYearViews`/`BuildYearView` take a **trailing optional** `achievements` parameter so every existing T-01/T-03 call site compiles and behaves identically. |
| `UI/AnnualReviewView.cs` | Existing surfaces only (no new history UI): the selected year's Forest column lists "Objective first reached: …"; the existing OBJECTIVES card adds "First reached in year N" or "Year reached: not recorded" under each achieved objective. |

### Semantics

- Record: appended once per objective when `EvaluateProgress` first sees it achieved, in objective order, `achievedYear` = evaluation snapshot year, `targetValueAtAchievement` = the objective target then. A later loss keeps the record. Nothing is recorded after the scenario outcome is final (evaluation already stops then).
- Missing field / empty list = **not recorded**, at every version.
- **No invented past (AC-4), round 2.** On every load the manager replays the evaluations that already happened: one per year with a `YearAdvanced` event (and `<= outcomeYear` once the outcome is final), using that year's snapshot with the events/resolved orders as they stood then. An objective achieved in such a year with no record predates recording: it goes in a session-only `achievedBeforeRecording` set and is **never recorded later** (so met-in-8 / lost / re-met-in-14 cannot become "14"). The year-0 baseline snapshot has no `YearAdvanced` event, is never evaluated, and so never guards anything: a new game keeps everything recordable across a year-0 save/reload.
- **R-1 derived year (display only, never saved).** For the same unrecorded objectives the first evaluated year in which `Evaluate` reports achieved is shown as **derived**, but only if no earlier evaluated year lacks its snapshot (evidence-complete); otherwise "not recorded". `ObjectiveYear(id, out source)` returns Recorded / Derived / NotRecorded. The Annual Review OBJECTIVES card shows "First reached in year N (recorded)", "… (derived from saved records)" or "Year reached: not recorded".
- Validation (v22+): record non-null; id is one of the six fixed ids or `introduced-<species>`; no duplicate ids; `0 <= achievedYear <= ecologicalYear`; years non-decreasing; target finite and `>= 0`. A pre-v22 save carrying records is refused. JSON: the field must be an explicit array of fully typed records, no duplicate keys, parse errors refused. All checks are pure and run before the first scene change.

### Hash-strip compatibility (Rack v21 pattern, deliberate and versioned in `ScenarioReferenceArchive`)

- Every world-identity hash (`WorldHash`, `CurrentWorldHash`, `LegacyCompatibleWorldHash`, `LegacyV15..V18`) removes `,"objectiveAchievements":[…]` **whatever it contains**. The record is a write-once observation derived from the same simulation, so it is not part of world identity. A "strip only when empty" rule would have moved every live anchor whose run achieves an objective; this is the decision to review (G3).
- `HashLayout` relabels the header: rack-default worlds → `"version":20` (as before); worlds carrying rack state → `"version":21` (they hashed with the v21 header before; without the relabel the bump would have moved them to 22).
- Reference Future v1 is not touched: `ScenarioReferenceArchive.Load`, `IdentityMatches` (`saveVersion == 12`) and the original-bytes re-hash are unchanged; `Resources/` is byte-identical to `aca74ac` (runner check).

## Offline evidence (plain .NET, nothing written in the repo)

`python3 -I -B Tools/Verification/S1CObjectiveHistory/run_offline_check.py` → **exit 0, `S1C_T05_OFFLINE_PASS checks=115`** (was 101), plus `BASELINE_DUMP_WRITTEN version=21` and `REFERENCE_FUTURE_V1_RESOURCES_UNCHANGED_VS_aca74ac`.

It builds two projects: the **baseline** from `git show aca74ac:` `ForestSaveData.cs` and the hash path of `ScenarioReferenceArchive.cs` (save v21), and the **candidate** from the working tree (real schema, validators, migration, hash path, history query and `ScenarioOneManager.ObjectiveHistory.cs` behind a minimal manager stub).

| Area (counted from the run output) | Checks |
|---|---|
| Schema, layout, v21 → v22 round trip, v21 load/re-save (`schema`, `round`, `layout`, `v21`) | 11 |
| Older saves v8–v20 and migration (`v8`…`v20`, `v22`, `migration`) | 9 |
| Refusals (21), matching no-mutation checks (10), accepted well-formed cases (2) | 33 |
| Recording rules, capture, load (`record`, `capture`, `load`) | 9 |
| **B-1** new game: year-0 save/reload, first review, later save/reload (`new`) | 5 |
| **B-2** R-1 derived vs not recorded: derivable ×3, never-achieved, evidence gap ×2 (`R-1`) | 5 |
| Pre-v22 load / re-save / reset / restore(null) (`pre-v22`, `reset`, `restore`) | 7 |
| **B-3** lost-and-regained: control, no record, derived 8 not 14, after re-save/reload (`lost/regained`) | 4 |
| Deterministic ordering | 2 |
| History year views | 3 |
| Hash compatibility vs the v21 baseline (8 fixture worlds × 7 functions + coverage + achievements-do-not-move) | 18 |
| Source audits | 8 |
| **Total** | **115** |

Mutation check (each break made the run fail, then was reverted): remove the strip, remove the v21 header relabel, remove the duplicate check, remove the old-version refusal, remove the migration, then for round 2: treat any event year as evaluated (year-0 baseline), drop the evidence-gap rule, ignore the gap, ignore the outcome-year limit, iterate years descending → all killed (the first round-2 mutant initially survived, so a year-0 non-`YearAdvanced` event was added to the new-game fixture).

### Protected-state regression (same toolchain, this branch)

| Harness | Result |
|---|---|
| S1-C T-01 query `S1C_HISTORY_QUERY_PASS` | **55** PASS |
| S1-C T-03 `S1C_T03_DRILLDOWN_PASS` | **72** PASS |
| S1-D T-02 `CYCLE_STATE_OFFLINE_PASS` | **93** PASS |
| Non-UI keyboard/RackBay/RackSample/Approach | **46 / 87 / 75 / 49** PASS |
| ThreeSpecies timber / history | **73 / 74** PASS |
| N3 `N3_OFFLINE_PASS` | **88** PASS only with the schema pin overridden (temporary, reverted); unmodified it stops at `enum9 schema21` |
| S1-D T-06 EventId | 75 checks; 2 failures unmodified, 1 with the first pin overridden — all pin assertions (the remaining one is the source-text `CurrentVersion = 21` check) |

Pre-existing, unrelated, **identical at the base**: `THREE_SPECIES_OFFLINE_FAIL failures=1` (`NO_PRODUCTION_IDENTIFIER…` on `ScenarioOneRackBay.cs`) and `COMPOSITE_DENSITY_FAIL checks=107 failures=3`; both were re-run on a clean checkout of `aca74ac` and fail the same way. `P2`/`P3` offline runners need `/home/jer/CCF-main/Assembly-CSharp.csproj` and cannot run in this environment.

## Required follow-up: stale `version == 21` pins (no pin edited in this task)

Per the independent review, none is a production break (production uses `>=`/`<` thresholds); every one is a `Tools/Verification/` label to be relabelled by one reviewed "v22 pin relabel" change owned by the Unity qualifier, with no assertion weakened. I did not edit any; N3 and EventId were measured with a temporary override that was reverted.

**Label relabels (21 → 22), first list:** `ScenarioOneEconomyIntegrationVerification.cs:104`, `ScenarioOneTeachingCopyVerification.cs:87`, `ScenarioOneCompletionVerification.cs:153`, `ScenarioOneRackUx/RackUxRack.cs:507`, `ScenarioOneCompletion/N3/OfflineLogic/N3OfflineCheck.cs:82`, `ScenarioOneCompletion/N3/N3CompletionVerification.cs:104`, `ScenarioOneRepeatedInterventions/EventIdIntegrity/EventIdIntegrityOfflineCheck.cs:215-216` (source-text pin), `ModelDefault/ModelDefaultVerification.cs:253`, `Enlarged80Calibration/AreaCalibrationVerification.cs:85`, `SequentialObjectives/SequentialObjectivesVerification.cs:229,339`, `RackV2FieldMarking/RackV2FieldMarkingVerification.cs:162`, `StandGeometry/StandGeometryVerification.cs:118`, `U4Guidance/U4GuidanceSmoke.cs:213`, `U4/U4Qualification.cs:335`, `ThreeSpecies/MixedDensityVerification.cs:192`, `ThreeSpecies/ScenarioHistoryVerification.cs:110`, `ThreeSpecies/BroadleafTimberEconomyVerification.cs:107,164`, `RackV2Persistence/RackV2PersistenceVerification.cs:174`.

**Added by the review (captured `version == 21`, also label relabels):** `ScenarioOneCompletionVerification.cs:443`, `ModelDefault/ModelDefaultVerification.cs:327`, `WindthrowV1/StormCoreVerification.cs:37`, `StandGeometry/Enlarged80Verification.cs:273`, `StandGeometry/StandGeometryVerification.cs:157`, `RackV2Save/RackV2SaveVerification.cs:119,232`, `SitkaGrowthMortality/SitkaGrowthModelVerification.cs:97`, `ThreeSpecies/ScenarioHistoryVerification.cs:187`.

**Judgement, not a number:** `ThreeSpecies/Qualification/DensityQualification.cs:952` (relabel `CurrentVersion == 21` and `"version":21`; its growth-model-1 vs model-2 key-set equality may differ if only one run records achievements — decide under Unity, exclude the record's keys rather than weaken). `RackV2Save/RackV2SaveVerification.cs:221` (`v20Parsed.version == 20`) is a fixture-input assertion; leave it. Harnesses comparing raw `CaptureData()` JSON across a run that reaches an objective (e.g. `SequentialObjectivesVerification.cs:343/361`, `RackV2Editing` `SaveJson()` hashes) may now differ by a record; only measurable under Unity — explain, do not relabel.

## Unity residual (not done, not claimed)

Unity import/compile of the new partial and the edited files; the real `ScenarioOneManager` capture/restore/`EvaluateProgress` path and the replay's use of the real `ScenarioOneObjectives.Evaluate` (offline it runs against a stub table; the call shape is source-audited only); the live disk load path through `ForestSaveController`; `JsonUtility` byte output (the offline hash proof uses `System.Text.Json`, so it proves the strip/relabel logic and field order, not Unity float formatting); rendered Annual Review text for the two new lines (T-03 surface); all deterministic/lifecycle anchors and hash-bearing gates under real Unity; genuine v20/v21 save load.

## Round-2 review fixes (B-1, B-2, B-3) and what remains open

- **B-1 fixed:** year-0 baseline is never an evaluated year; new game → save at year 0 → reload → first review records canopy objectives; later save/reload keeps them (5 tests, plus a mutant that treats year 0 as evaluated).
- **B-2 fixed:** labelled derived year for evidence-complete old saves, else "not recorded"; one derivable (three objectives) and non-derivable (never achieved; evidence gaps) offline cases. Render-time only; nothing written to old saves.
- **B-3 fixed:** the guard covers every evaluated past year, not just the last snapshot; met-8 / lost-12 / re-met-14 stays unrecorded, shows derived 8, and survives a re-save as v22 and reload.
- Derivation is as exact as the persisted records: it assumes completed orders carry `resolvedYear` and that year-tagged events/snapshots are retained; a missing snapshot year degrades to "not recorded". Real-save exactness (genuine v20/v21 saves) is Unity/AC-4 evidence still to collect.
- Non-blocking review notes N-1 (hard-coded fixed objective ids in the validator), N-2 (wording; I added the recorded/derived labels but kept "First reached in year N"), N-3 (`Canonicalize` sort is harmless) are left for the Manager.
- G3 to confirm: achievements are excluded from every world hash always (accepted by the review).

## Protected-state statement

Reference Future v1 bytes and its loader/identity checks: unchanged. No anchor or protected value was changed, rebaselined or weakened; no harness file was edited; Rack v21 behaviour and normalisation (`version >= 21`), v15 normalisation, T-03 drill-down code and T-06 event-ID validation order are unchanged. The only behaviour added to existing code paths is the optional trailing parameter on the history view builders and the additive v22 field.

## Exit token

**S1-C T-05 IMPLEMENTED — READY FOR CROSS-PROVIDER SAVE REVIEW**
