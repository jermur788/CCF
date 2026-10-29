# S1-D T-06 — Unity Qualification Handoff

**Result: FAILED at Q1 (candidate verification harness defect). Not qualified.**

`S1-D T-06 UNITY QUALIFICATION NEEDS CHANGES — MANAGER REVIEW REQUIRED`

## Executor and scope

- Executor: Claude B / **Claude Opus 5.5** (`claude-opus-5-5`, Anthropic), Claude Code harness in T3 Code, acting as the test sub-agent for umbrella #91.
- Claim: #88 (left open).
- Packet: `Docs/Handoffs/S1-D-Implementation-06-Unity/TASK-PACKET.md` (read from `origin/main`).
- Exact candidate: **`29bb6cbd6bf5dd81499a1e45bb68666f34969309`**. Required production blob verified before and after the run: `Assets/ForestPrototype/ForestSaveValidation.cs` = `28b786d49fc728e50a1009bc0c42fd0f7c4f33bb`.
- Unity lane: `/media/jer/ZX20/CCF-U1`, detached at the candidate, clean at start and end, warm Library. Lane HEAD was not changed.
- Unity 6000.6.0f1, `-batchmode -nographics`. **No Windows build** (and no other build) was made.
- No production or verification file was edited. No assertion was weakened or rebaselined. No integration PR.
- Evidence: `/media/jer/Files/CCF/Verification/S1D-T06-Unity-20261029/`.

Verification source blobs at the candidate:

| File | Blob |
|---|---|
| `Tools/Verification/ScenarioOneRepeatedInterventions/EventIdIntegrity/SaveEventIdIntegrityVerification.cs` | `84d0cb1a658f73564ec88f553de223d4d70c9804` |
| `.../EventIdIntegrity/run_save_event_id_integrity.py` | `73db82196f4e012f8972d58e5c64ba3401c6e1ed` |
| `Tools/Verification/U4/U4Qualification.cs` | `e5755bb4fb6bbfdf450a93a5955464d502739e04` |
| `Tools/Verification/SaveHardeningVerification.cs` | `1bd30301f10e7376bba46be25c282f5c91b30f35` |
| `Tools/Verification/SaveCellIndexHardening/SaveCellIndexHardeningVerification.cs` | `8b14c5fcf668deefc2c63e67fa7007a30bbfd2cd` |

## Unity lock

| | |
|---|---|
| Owner | `ClaudeB-S1D-T06` |
| Task id | `S1-D-T-06` |
| Lock id | `8e6d93be04b4449790c49d6e3b1e2ec9` |
| Acquired | 2026-10-29T03:32:22Z, normal acquire (status was `free`, no Editor PIDs) |
| Released | about 2026-10-29T03:37Z, `status: released`, `forced: false`; status afterwards `free`, no Editor PIDs |

Receipts: `unity-lock-acquire.json`, `unity-lock-release.json`, `unity-lock-status-after.json`.

## Environment note: stale host licence (not candidate-caused)

The first Q5 attempt exited **198** in 2 s: `[Licensing::Module] Error: 'com.unity.editor.headless' was not found.` (`q5-failed-attempt-198/`). Every checked-in runner copies `~/.config/unity3d/Unity/licenses` into its isolated `XDG_CONFIG_HOME`. That host copy is stale (`UpdateDate 2026-10-15`). The Unity Hub (flatpak) licence is current (`IssueDate 2026-10-26`, `UpdateDate 2026-11-25`). The PARITY-03 Unity qualification handoff records the same failure and the same remedy.

Remedy used, with no harness, licence or global configuration edited:

- U4 runner: the isolated `Build/U4/run/config/.../licenses` was seeded from the Hub licence before the run. `run_qualification.py` and `run_u4.sh compile` copy only when that folder is absent.
- Q1 runner: it was run unchanged through `driver/hub_licence_shim.py`, kept in the evidence folder outside the repo. The shim only redirects the runner's `shutil.copytree` of the host licence folder to the Hub folder.
- Both licence files are byte-unchanged (`licences-unchanged.txt`). The isolated licence copies were deleted after the run.

**Ops follow-up:** refresh the host licence copy, or make the runners choose the newest licence the way `run_gate.py` does.

All Editor runs used `TMPDIR=/tmp/`.

## Gate results

### Q5 — Unity import / compile: PASS

```sh
TMPDIR=/tmp/ bash Tools/Verification/U4/run_u4.sh compile   # Hub licence seeded in isolated config
```

- Exit **0** in 64 s, ending `Exiting batchmode successfully now!`.
- Real Bee/Csc rebuild: `Csc Library/Bee/artifacts/1900b0aE.dag/Assembly-CSharp.dll (+2 others)`.
- **0** `error CS`. 527 `warning CS` lines, all pre-existing obsolete-API or hiding warnings. **None** names a T-06 file (`ForestSaveValidation.cs`) or a T-02 file (`ScenarioOne/ScenarioCycleState.cs`, `ScenarioOne/ScenarioOneManager.History.cs`).
- No import failure, and no `.meta` regenerated. This also gives the deferred T-02 import/compile observation: `ScenarioCycleState.cs` and its `.meta` imported and compiled cleanly.
- Only Editor side effect: `.vscode/settings.json` (known solution-name rewrite). The patch is in `editor-side-effect-vscode-settings.patch`, and the file was restored byte-exact afterwards.
- Logs: `q5-compile/`.

### Q1 — T-06 runtime loader parity / no mutation: FAIL

```sh
TMPDIR=/tmp/ python3 -I -B <evidence>/driver/hub_licence_shim.py \
  Tools/Verification/ScenarioOneRepeatedInterventions/EventIdIntegrity/run_save_event_id_integrity.py \
  --i-hold-the-unity-lock --label run-1
```

- Result: `FAIL run-1`, Unity exit 1, 88.9 s, 0 compile errors, **0 `SAVE_EVENT_ID_CASE` lines**.
- Failure marker, verbatim:

  `SAVE_EVENT_ID_VERIFY_FAIL System.InvalidOperationException: valid distinct positive IDs 1,2,3 geometry=0 central validation mismatch: rack planning V2 requires the Enlarged80 stand`

- No `SAVE_EVENT_ID_VERIFY_PASS` was produced, so no case counts were observed. None of the required Q1 properties was demonstrated in Unity.

**Diagnosis (read-only, nothing changed):**

1. `SaveEventIdIntegrityVerification.Verify()` captures the live Enlarged80 world. It then builds the Legacy40 basis by cloning it and changing only `standGeometryModel = 0`, `cells.Take(64)` and `understoreyCells.Take(64)`.
2. Since Rack Planning & Field Marking V2 save v21 (`cf84d24`, D-064, an ancestor of the T-06 base), the live default world carries `scenarioOne.rackPlanningModel = RackPlanningFieldMarkingV2`. The Legacy40 clone keeps that value.
3. `ForestSaveValidation.ValidateRackV2` (`Assets/ForestPrototype/ForestSaveValidation.RackV2.cs:34-36`) correctly refuses V2 state on a non-Enlarged80 stand. The fixture's *valid* event-ID case is therefore rejected for an unrelated reason, and the harness's central-validation check fails on the first fixture.
4. The fixture builder is at fault, not the production event-ID validator. The run never reached any event-ID assertion. The later `historical v19` fixtures (built from the same `legacy` basis with `version = 19`) would also be refused, by the "older than version 21 cannot carry V2 state" rule.
5. Likely required repair, for the Manager to authorise: the harness's Legacy40 basis (and its v19 derivatives) must be a valid model-0 world. That means `rackPlanningModel = Legacy`, no racks, `rackPlanConfirmed = false`, `nextRackId = 1` and no rack-owned work orders. Equivalently, it could be derived from an actual Legacy40 world. This is a verification-file repair inside the T-06 candidate. It was not made here.

**Related risk, inferred but not run:** the pre-existing Q3 harness `SaveCellIndexHardeningVerification.cs:138` builds its Legacy40 basis the same way. It will probably hit the same Rack V2 rejection on the current base. If so, that is a base-level stale fixture from RACK-V2 save v21, not a T-06 defect. It should be checked before Q3 is used as a preservation gate.

### Q2, Q3, Q4: NOT RUN

The instruction was to stop at the first required failure, so `SaveHardeningVerification` (Q2), `run_save_cell_index_hardening.py` (Q3) and the U4 `core` phase containing `SYN duplicate event IDs` (Q4) were **not run**. Their status is unknown. The offline preservation suites (T-06 75 + 7/7 mutants, N3 88, T-02 93 + 7/7 mutants) were not rerun here. No candidate file changed. #87 confirmed them on this exact candidate.

## Side effects and cleanup

- Staged harness: the Q1 runner removed `Assets/ForestPrototype/SaveEventIdIntegrityVerification.cs` and its `.meta`. There are no staged `SaveEventId*`, `SaveHardening*`, `SaveCellIndex*` or `U4Qualification*` files under `Assets/ForestPrototype/`. The three tracked `*Verification.cs` files there are pre-existing.
- `.vscode/settings.json`: the generated patch matched the recorded patch and was restored. `git status --porcelain=v1 -uall` on U1 shows **0** lines, and HEAD is `29bb6cb`.
- Player saves (`~/.config/unity3d/DefaultCompany/CCF`): the `forest-save.json`, `.bak` and absent `.tmp` digests are identical before and after (`player-saves-before.txt`, `player-saves-after.txt`). The Q1 runner's own digest check passed (no `ORIGINAL SAVES CHANGED`). All Editor runs used isolated `XDG_CONFIG_HOME` folders under the ignored `Build/`.
- Unrelated work: none touched. This handoff is the only file committed. It is on docs branch `handoff/s1-d-t06-unity-v1` from `29bb6cb`, written in `/media/jer/ZX20/CCF-S1D-T06Review`.

## Storage

ZX20 had about 21 GiB free before and after, above the 15 GiB floor. Durable evidence is on Files, under `/media/jer/Files/CCF/Verification/S1D-T06-Unity-20261029/`.

## Disposition

- T-06 **cannot be integrated** on this evidence.
- The production validator compiled cleanly in Unity, but its runtime loader parity and no-mutation behaviour remain **unverified in Unity**.
- The candidate needs a Manager-authorised repair of the Q1 harness's Legacy40/v19 basis (verification only). After that, Q1 through Q5 should be rerun on the repaired candidate. The Manager should also decide whether Q3's identical fixture construction needs the same treatment.

`S1-D T-06 UNITY QUALIFICATION NEEDS CHANGES — MANAGER REVIEW REQUIRED`
