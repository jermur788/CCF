# S1-D IMPLEMENTATION 06 — HANDOFF (T-06 management-event ID integrity)

**Status line:** `S1-D T-06 STATIC IMPLEMENTED / UNITY SAVE-HARDENING WAITING — RACK UNITY PRIORITY`

Static implementation and offline verification are complete. No Unity Editor run was made, so the Editor-only proofs
(CanLoad/LoadData/Load parity at runtime, disk bytes, live-world non-mutation, existing `SAVE_HARDENING_VERIFY_PASS`
and cell-index suite) are **not yet evidenced**. One existing Unity harness conflict needs a Manager disposition
before the Unity gates are run (see "Unresolved risks", item 1).

Independent review by a different provider/model is mandatory before integration. The implementer did not self-approve.

## Identity

| Item | Value |
|---|---|
| Claim | #83 (`jermur788/CCF`), marked `[RUNNING]` before any production edit |
| Model / provider | Claude Sonnet 5.5 (`claude-sonnet-5-5`) / Anthropic, Claude Code in T3 Code (the packet's accepted Sonnet-class fallback; not the preferred Codex/Sol lane) |
| Branch | `feature/s1-d-management-event-id-integrity-v1` |
| Worktree | `/media/jer/ZX20/CCF-S1D-T06` (dedicated, created clean from `origin/main`) |
| Starting HEAD | `e6ee19990fa7a16b7c69c2ecfa6393b45cc4ee67` (= `origin/main` at start) |
| Implementation SHA | `8e05b5a24d7c3c74d063feb28002a6d7f28db438` |
| Published tip | the commit that adds this file (the branch head on `origin`; its SHA is recorded in the claim #83 handoff comment, since a file cannot name its own commit) |
| Evidence folder | `/media/jer/Files/CCF/Verification/S1D-T06-EventIdIntegrity-20261029/` (`audit/`, `offline/`) |

## Production change (one file)

`Assets/ForestPrototype/ForestSaveValidation.cs` (+21 lines, no deletions):

- New private `ValidateManagementEventIds(ScenarioOneSaveData)`. For a non-null `scenarioOne.managementEvents` list it returns,
  for the first offender in list order:
  - `management event record {i} has a non-positive ID ({id})` when `eventId <= 0`;
  - `management event ID {id} appears twice (records {first} and {i})` for a repeated positive ID.
  Null record entries are skipped here because `ValidateScenarioCellIndices` already rejects them.
- Called as the **last** step of `ForestSaveValidation.Validate`, so every older rejection keeps its exact reason and precedence.
  The rule applies to every save version (it is not gated on `version >= 15`).
- A null `scenarioOne`, a null list or an empty list returns `null` (accepted), as before.
- No repair, renumber, dedupe or reorder. No new field, no `CurrentVersion` change (still 21), no `ForestSaveController` change.
  The player sees the existing `Save not loaded: <reason>.` wrapper (disk path: `CanLoad`, and `LoadData`'s message).

Why one place is enough (inspected, not assumed): `CanLoad` → `FindLoadProblem` and `LoadData` → `FindLoadProblem`, and
`FindLoadProblem` → `ForestSaveValidation.Validate`. `LoadData` returns `false` before the "First change to the world" comment.
`Load()` reads the file through `TryReadSave`, then calls `LoadData`. `ForestSaveController.cs` contains neither `eventId`
nor `managementEvents` (asserted by the offline check).

## Verification files added (all under `Tools/Verification/ScenarioOneRepeatedInterventions/EventIdIntegrity/`)

| File | Purpose | Run state |
|---|---|---|
| `EventIdIntegrityOfflineCheck.cs` | 75 checks against the PRODUCTION validator on plain .NET | **Run, PASS** |
| `run_event_id_integrity_offline.py` | builds/runs the check outside the repo; `--mutations` proves it can fail | **Run, PASS** |
| `SaveEventIdIntegrityVerification.cs` | disposable Editor gate: CanLoad / LoadData / Load parity, shared reason, no world/disk mutation | **Written, NOT compiled or run** |
| `run_save_event_id_integrity.py` | copy/run/remove driver for the Editor gate; refuses to start without `--i-hold-the-unity-lock` | **Not run** |

No other tracked file changed. No existing assertion was edited, weakened or rebaselined.

## A. Fixture-history compatibility scan (done before any production edit)

Method: `git grep` for `managementEvents` in every tracked file including binaries, inventory of tracked `.bytes/.gz/.zip/.json`
save-shaped files, then a scripted scan (`audit/fixture_audit.py`, outputs in `audit/`).

| Fixture | Save version | Event list | Result |
|---|---|---|---|
| `Assets/ForestPrototype/ScenarioOne/Resources/ScenarioOneReferenceFutureV1.bytes` (frozen Reference Future v1), milestone Y0 | v12 | present, 0 events | OK |
| same, Y20 | v12 | 511 events | all positive, all unique |
| same, Y50 | v12 | 800 events | all positive, all unique |
| same, Y100 | v12 | 1238 events | all positive, all unique |
| `Docs/Verification/StormsWindthrowIntegration/Evidence/StormCoreVerification/resolved_storm.json` | v19 | 0 events | OK |
| `.../StormRngWrite/world-0.json`, `world-1.json` | v19 | 1 event each | OK |
| `.../StormSalvageVerification/salvage_resolved.json` | v19 | 49 events (IDs 1..49) | OK |
| `Assets/Scenes/ForestTest.unity`, `MixedSpeciesTest.unity` (serialized manager) | n/a | `managementEvents: []` | OK (empty) |

No other tracked file carries an event list. **No ambiguous shipped or accepted save fixture was found, so the STOP rule was not
triggered for a save.** Event IDs are minted only in `ScenarioOneManager.RecordEvent` from a monotonic counter; the save
migration code (`ScenarioOneSaveMigration.cs`) does not create or alter event IDs.

Coverage honesty: the only tracked data for save versions 12 and 19 is above; there is no tracked event-bearing fixture for
v13–v18 or v20, and v21 data exists only as in-memory captures during Unity gates. The offline check additionally validates a
synthetic valid history at versions 1, 3, 12, 14, 15, 19, 20 and 21. The same offline run also loads the four Reference Future
milestones and the four storm saves through the real `Validate` and they all pass (see B).

## B. Focused positive / negative validation (offline, production code, PASS 75/75)

Log: `offline/event-id-offline-with-mutations.log`.

| Requirement | Result |
|---|---|
| Valid distinct positive IDs accepted (`1,2,3`; unordered `9,2,5`; gaps) | accepted |
| `eventId == 0` rejected | `management event record 0 has a non-positive ID (0)` |
| Negative rejected | `management event record 2 has a non-positive ID (-3)`; `int.MinValue` also rejected |
| Duplicate positive rejected (adjacent and non-adjacent) | `management event ID 2 appears twice (records 1 and 2)`; `... ID 5 ... (records 0 and 4)` |
| First offender in list order is reported, deterministically | zero-before-duplicate and duplicate-before-zero both pinned |
| Null list / empty list / property absent from JSON / no `scenarioOne` accepted | accepted |
| Rule applies at every version | duplicate and zero rejected at v1, 12, 14, 15, 19, 21; valid accepted at v1…21 |
| Unrelated validation unchanged | cell-index reason still wins over a bad ID; null-record, growth-model, duplicate-tree-ID, null-tree-list and null-save reasons byte-identical; events sharing a cell with distinct IDs stay accepted |
| Fixtures still accepted by the real validator | Reference Future v1 Y0/20/50/100 (v12) and the four v19 storm saves |
| No repair | list JSON and `nextManagementEventId` byte-identical after validating bad histories |
| Disk-parse path equals in-memory path | same reason for zeros, negatives and duplicates after a `JsonUtility` round trip; a record missing `eventId` parses as 0 and is rejected |
| No schema bump | `CurrentVersion` is 21 in source and in the compiled build |

Mutation proof (`--mutations`): 7 deliberately broken copies of `ForestSaveValidation.cs` (zero allowed, negative allowed,
duplicate allowed, rule not wired, rule moved before older checks, null list rejected, repair-by-sorting) were all caught;
none survived.

## C. CanLoad / LoadData parity

- **Static (done):** the offline check asserts `CanLoad` and `LoadData` both call `FindLoadProblem`, `FindLoadProblem` calls
  `ForestSaveValidation.Validate`, `LoadData` returns false before the first world change, and the controller holds no second
  event-ID rule. One rule, one authority.
- **Runtime (NOT YET RUN):** `SaveEventIdIntegrityVerification.cs` is written to prove, per case and for Legacy40 and Enlarged80:
  `CanLoad` verdict == `LoadData(parsed copy)` verdict == `Load()` (disk) verdict; the rejected `CanLoad` problem string
  equals `Save not loaded: <Validate reason>.`; the `LoadData` message (read from the controller's `message` field) and the
  disk-`Load` message both equal that same string. Cases: valid `1,2,3`; valid unordered `9,2,5`; empty list; list absent from
  the disk JSON; zero; negative; duplicate; duplicate-of-1; historical v19 valid and v19 duplicate; baseline restore.

## D. No mutation on rejection

- **Static (done):** the validator is a pure read and leaves the history, IDs and counter byte-identical (offline).
- **Runtime (NOT YET RUN):** the Editor gate snapshots a world string (geometry, ecological year, tree count, player pose, world
  hash, full Scenario One JSON) and a disk string (`forest-save.json`, `.bak`, `.tmp` as bytes) before each case. For every
  rejection it requires them unchanged after `CanLoad`, `LoadData`, `Load()` and one more frame, and checks ecological year
  and tree count explicitly. The runner also verifies the player's real save digests are unchanged after the run.

## E. Preservation

- Offline, run now, PASS: `run_n3_offline.py` (N3 completion rules plus the production validator): `N3_OFFLINE_PASS checks=88`.
  `run_offline_check.py --mutations` (T-02 cycle state): `CYCLE_STATE_OFFLINE_PASS checks=93`, 7/7 mutants caught.
  Logs in `offline/`.
- Unity, **not run**: existing `SAVE_HARDENING_VERIFY_PASS` gate (`Tools/Verification/SaveHardeningVerification.cs`, copy/run/remove
  per its header) and `Tools/Verification/SaveCellIndexHardening/run_save_cell_index_hardening.py`
  (`SAVE_CELL_INDEX_HARDENING_VERIFY_PASS`, `SAVE_CELL_STARTUP_PASS`). Static reading says the cell-index suite's accepted event
  fixtures use IDs 1 or 1,2 on top of the live `ForestTest` capture; that holds only while the captured baseline has no events of
  its own, which is an assumption to confirm in the run, not a verified fact.

## Unity

Not run. No Unity lock was requested or acquired. This honours the queue (`active-tasks.md`: #49 RACK-V2-08 has priority;
Rack was DISPATCHED, not proven RUNNING, when last read) and the packet's "do not acquire Unity ahead of #49". No Windows build.
Candidate for the later Unity run: the published tip of this branch (exact SHA in the claim comment), run in the machine Unity
lane after the Manager releases it. Exact commands for the lane holder, from the candidate worktree, once the lock is held:

```
python3 -I Tools/Verification/ScenarioOneRepeatedInterventions/EventIdIntegrity/run_save_event_id_integrity.py --i-hold-the-unity-lock --label run-1
python3 Tools/Verification/SaveCellIndexHardening/run_save_cell_index_hardening.py
# SaveHardeningVerification: stage per its header comment; expect SAVE_HARDENING_VERIFY_PASS; remove source + .meta afterwards
```

Expected (counted from the harness source, not observed): `SAVE_EVENT_ID_VERIFY_PASS cases=19 rejected=9 accepted=10 ...`
(8 cases per geometry basis x 2, plus historical v19 valid, historical v19 duplicate and the baseline restore), no `error CS`,
player save digests unchanged, and no temporary script or `.meta` left under `Assets/ForestPrototype/`. Because the Editor harness has never been compiled, a first-run compile error in
it is possible and would be a harness defect, not a production failure.

## Repo / worktree cleanliness and unrelated work

- Worktree `/media/jer/ZX20/CCF-S1D-T06` was clean at creation (0 changes) and holds only the five files listed above plus this
  handoff. `__pycache__` created by a Python import during an offline run was deleted; nothing left untracked.
- Changes to `origin/main` after `e6ee199`: none made by this task. `active-tasks.md` not edited (Manager-owned).
- The primary checkout `/media/jer/ZX20/CCF-Primary` (branch `ops/workflow-foundation-v1`) has pre-existing uncommitted edits
  (`AGENTS.md`, `active-tasks.md`, three untracked docs). They were observed and left untouched.
- Other worktrees were not touched.

## Unresolved risks / Manager decisions

1. **Existing Unity harness `Tools/Verification/U4/U4Qualification.cs` contradicts the new rule (predicted, not run).** Fixture
   "SYN duplicate event IDs" (around line 529) builds a history with duplicate `eventId`s and loads it through `RestoreJson`,
   which does `Expect(saves.LoadData(...), label + ": LoadData accepted")`. After T-06 that `LoadData` returns false, so that
   expectation will fail when U4 is next run. This is not a shipped or accepted *save* (it is an in-memory negative fixture for
   the completion chronology rule, and spec F-9/RD-12 already treats "ambiguous IDs load and never complete" as the behaviour
   being replaced), so I did not treat it as the STOP condition. But the packet forbids weakening or rebaselining existing
   assertions, so I left U4 untouched. **A Manager decision is needed** on how that case is dispositioned (for example: change
   it to assert load refusal, or inject the duplicate after load) before U4 is used as a gate again. The offline N3 and T-02
   suites already cover duplicate IDs at the rules level and are unaffected.
2. The Editor gate and its runner are uncompiled/unrun (above). The static offline proof does not exercise Unity's `JsonUtility`
   (the shim stands in for it) or the controller at runtime.
3. Shared load-path change: independent review is mandatory. Suggested review focus: placement of the call last in `Validate`
   (reason precedence), the "applies to all versions" choice, and the skipped-null-record branch.
4. The player-facing wording is functional, not copy-reviewed (`Save not loaded: management event ID 2 appears twice (records 1
   and 3).`). Record numbers are zero-based list positions.
