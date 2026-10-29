# S1-C Implementation 05 — T-05 objective/progression history (save v22)

**Verdict: S1-C T-05 IMPLEMENTED — READY FOR CROSS-PROVIDER SAVE REVIEW**

Static/offline only. No Unity Editor, no Unity lock, no Windows build. Nothing here is Unity-verified, integrated or accepted. Claim #104 stays open.

## Identity

- Worker: T3 Code / Claude Code, **Claude Sonnet 5.5** (Anthropic, `claude-sonnet-5-5`).
- Branch/worktree: `feature/s1-c-t05-objective-history-v1`, `/media/jer/Videos/CCF-S1C-T05`; base `aca74acacc565f72ab802177cf69ca6ebb54d637` (umbrella integration with S1-C T-03, S1-D T-06, Rack v21, PARITY-06).
- **Production + verification commit: `cd7b5183bec72919a953a2d50d878eeead545303`.** This handoff is committed on top of it and changes no code; the branch tip SHA is in the worker report.
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
- **No invented past (AC-4).** Evaluation only runs when the year advances, so a save always matches its last evaluation. On every load, any objective that is achieved *and* has no record is therefore from before v22; it goes in a session-only `achievedBeforeRecording` set and is never stamped with a later year. The set is rebuilt on every load (so a pre-v22 save re-saved as v22 stays "not recorded"). Not persisted.
- Validation (v22+): record non-null; id is one of the six fixed ids or `introduced-<species>`; no duplicate ids; `0 <= achievedYear <= ecologicalYear`; years non-decreasing; target finite and `>= 0`. A pre-v22 save carrying records is refused. JSON: the field must be an explicit array of fully typed records, no duplicate keys, parse errors refused. All checks are pure and run before the first scene change.

### Hash-strip compatibility (Rack v21 pattern, deliberate and versioned in `ScenarioReferenceArchive`)

- Every world-identity hash (`WorldHash`, `CurrentWorldHash`, `LegacyCompatibleWorldHash`, `LegacyV15..V18`) removes `,"objectiveAchievements":[…]` **whatever it contains**. The record is a write-once observation derived from the same simulation, so it is not part of world identity. A "strip only when empty" rule would have moved every live anchor whose run achieves an objective; this is the decision to review (G3).
- `HashLayout` relabels the header: rack-default worlds → `"version":20` (as before); worlds carrying rack state → `"version":21` (they hashed with the v21 header before; without the relabel the bump would have moved them to 22).
- Reference Future v1 is not touched: `ScenarioReferenceArchive.Load`, `IdentityMatches` (`saveVersion == 12`) and the original-bytes re-hash are unchanged; `Resources/` is byte-identical to `aca74ac` (runner check).

## Offline evidence (plain .NET, nothing written in the repo)

`python3 -I -B Tools/Verification/S1CObjectiveHistory/run_offline_check.py` → **exit 0, `S1C_T05_OFFLINE_PASS checks=101`**, plus `BASELINE_DUMP_WRITTEN version=21` and `REFERENCE_FUTURE_V1_RESOURCES_UNCHANGED_VS_aca74ac`.

It builds two projects: the **baseline** from `git show aca74ac:` `ForestSaveData.cs` and the hash path of `ScenarioReferenceArchive.cs` (save v21), and the **candidate** from the working tree (real schema, validators, migration, hash path, history query and `ScenarioOneManager.ObjectiveHistory.cs` behind a minimal manager stub).

| Area (counted from the run output) | Checks |
|---|---|
| Schema, layout, v21 → v22 round trip, v21 load/re-save (`schema`, `round`, `layout`, `v21`) | 11 |
| Older saves v8–v20 and migration (`v8`…`v20`, `v22`, `migration`) | 9 |
| Refusals (21) and the matching no-mutation checks (10), plus 2 accepted well-formed cases | 33 |
| Recording rules, capture/load, pre-v22 no-invented-past, reset/restore(null) (`record`, `capture`, `load`, `pre-v22`, `reset`, `restore`) | 17 |
| Deterministic ordering | 2 |
| History year views | 3 |
| Hash compatibility vs the v21 baseline: 8 fixture worlds × 7 hash functions reproduced exactly, fixture-count and coverage checks, achievements do not move any hash | 18 |
| Source audits (wiring order, outcomeYear untouched, pre-mutation chain, T-06 position, Reference identity, v15/v21 normalisation) | 8 |
| **Total** | **101** |

Mutation check (each break made the run fail, then was reverted): remove the strip, remove the v21 header relabel, remove the load guard, remove the duplicate check, remove the old-version refusal, remove the migration → 6/6 killed.

### Protected-state regression (same toolchain, this branch)

| Harness | Result |
|---|---|
| S1-C T-01 query `S1C_HISTORY_QUERY_PASS` | **55** PASS |
| S1-C T-03 `S1C_T03_DRILLDOWN_PASS` | **72** PASS |
| S1-D T-02 `CYCLE_STATE_OFFLINE_PASS` | **93** PASS |
| Non-UI (RackBay/RackSample/Approach) | **87 / 75 / 49** PASS |
| ThreeSpecies timber / history | **73 / 74** PASS |
| N3 `N3_OFFLINE_PASS` | **88** PASS only with the schema pin overridden (see below); unmodified it stops at `enum9 schema21` |
| S1-D T-06 EventId | 75 checks; the 2 failures are both schema-pin assertions (below); the other 73 pass |

Pre-existing, unrelated: `THREE_SPECIES_OFFLINE_FAIL failures=1` (`NO_PRODUCTION_IDENTIFIER…` on `ScenarioOneRackBay.cs`, a file this branch does not touch, `git diff aca74ac` empty). `P2`/`P3` offline runners need `/home/jer/CCF-main/Assembly-CSharp.csproj` and cannot run in this environment.

## Required follow-up: stale `CurrentVersion == 21` pins (decision for Manager / integrator; not edited here)

The bump to v22 necessarily invalidates harness assertions that pin "the schema is still 21". I did **not** weaken, edit or rebaseline any of them. To measure N3 and EventId I overrode the pin temporarily in the working tree and reverted (`git checkout`); the committed harness bytes are unchanged. Files that currently assert `ForestSaveData.CurrentVersion == 21` (or the text `CurrentVersion = 21`):

`ScenarioOneEconomyIntegrationVerification.cs:104`, `ScenarioOneTeachingCopyVerification.cs:87`, `ScenarioOneCompletionVerification.cs:153`, `ScenarioOneRackUx/RackUxRack.cs:507`, `ScenarioOneCompletion/N3/OfflineLogic/N3OfflineCheck.cs:82`, `ScenarioOneCompletion/N3/N3CompletionVerification.cs:104`, `ScenarioOneRepeatedInterventions/EventIdIntegrity/EventIdIntegrityOfflineCheck.cs:215-216`, `ModelDefault/ModelDefaultVerification.cs:253`, `Enlarged80Calibration/AreaCalibrationVerification.cs:85`, `SequentialObjectives/SequentialObjectivesVerification.cs:229,339`, `RackV2FieldMarking/RackV2FieldMarkingVerification.cs:162`, `StandGeometry/StandGeometryVerification.cs:118` (also `version == 21`), `U4Guidance/U4GuidanceSmoke.cs:213`, `U4/U4Qualification.cs:335`, `ThreeSpecies/MixedDensityVerification.cs:192`, `ThreeSpecies/ScenarioHistoryVerification.cs:110`, `ThreeSpecies/BroadleafTimberEconomyVerification.cs:107,164`, `ThreeSpecies/Qualification/DensityQualification.cs:952` (also asserts the `"version":21` JSON), `RackV2Persistence/RackV2PersistenceVerification.cs:174`.

Each must be relabelled to v22 by a deliberate, reviewed pin update (these are version labels, not behavioural anchors) before the Unity gates can pass; `DensityQualification.cs:952` additionally asserts an exact JSON key set and `"version":21`, which now includes `objectiveAchievements` and `"version":22`. I recommend one narrowly scoped "v22 pin relabel" change owned by whoever runs the Unity qualification, reviewed against this list.

## Unity residual (not done, not claimed)

Unity import/compile of the new partial and the edited files; the real `ScenarioOneManager` capture/restore/`EvaluateProgress` path (only source-audited and run against a stub offline); the live disk load path through `ForestSaveController`; `JsonUtility` byte output (the offline hash proof uses `System.Text.Json`, so it proves the strip/relabel logic and field order, not Unity float formatting); rendered Annual Review text for the two new lines (T-03 surface); all deterministic/lifecycle anchors and hash-bearing gates under real Unity; genuine v20/v21 save load.

## Not done / open for the reviewer

- **R-1 derivation for pre-v22 saves is not implemented.** Old saves show "Year reached: not recorded" for achieved objectives; no exact derivation from year-tagged L1 records is attempted (no speculative data was in scope). The spec's AC-4 "one exactly derivable objective" evidence therefore still needs either a follow-up derivation task or an explicit decision to defer.
- G3 decision to confirm: achievements are excluded from every world hash always, not only when empty.
- The load guard (`achievedBeforeRecording`) is my reading of "no retrospective writing"; confirm it against the intended T-05 semantics.

## Protected-state statement

Reference Future v1 bytes and its loader/identity checks: unchanged. No anchor or protected value was changed, rebaselined or weakened; no harness file was edited; Rack v21 behaviour and normalisation (`version >= 21`), v15 normalisation, T-03 drill-down code and T-06 event-ID validation order are unchanged. The only behaviour added to existing code paths is the optional trailing parameter on the history view builders and the additive v22 field.

## Exit token

**S1-C T-05 IMPLEMENTED — READY FOR CROSS-PROVIDER SAVE REVIEW**
