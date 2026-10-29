# RACK-V2-11 — Rack V2 persistence / save compatibility handoff (STATIC READY / UNITY WAITING)

- **Claim / umbrella:** #96 under #91.
- **Worker:** Claude A. Model: Claude Opus 5.5 (`claude-opus-5-5`), provider Anthropic, running through Claude Code as the RACK-V2-11 test sub-agent.
- **Worktree / branch:** `/media/jer/Videos/CCF-RackV2-11`, `feature/scenario-one-rack-v2-persistence-v1`.
- **Base:** `6d3228c6ce7cbb1c3ec7b99339dd7a38c92a4c22`. This is the umbrella integration branch, and it contains RACK-V2-09.
- **Candidate SHA (tooling):** `cc9376ab6ad067508508511271d682746349e18b`. This handoff commit is a docs-only descendant of it.
- **Exit token:** `RACK V2 SAVE — STATIC READY / UNITY WAITING`. This is **not** `RACK V2 SAVE PASS`, because no Unity run was made.

## v21 representation findings (inspected, unchanged)

- `ForestSaveData.CurrentVersion = 21`.
- `ScenarioOneSaveData` carries:
  - `rackPlanningModel` (0 = Legacy, 1 = RackPlanningFieldMarkingV2);
  - `rackPlanConfirmed`;
  - `nextRackId` (default 1; ids are never reused);
  - `racks : List<RackSaveData>`.
- Each `RackSaveData` record carries:
  - `rackId`;
  - `entranceX/Z` and `farX/Z`;
  - `removalTreeIds`, the frozen removal set;
  - `entranceTreeId`, which must be in that set;
  - `entranceMarked`.
- `ScenarioOneWorkOrder.rackId` links an order to a rack. 0 means ordinary work.
- Counters are derived, never stored. `MarkedRackEntranceCount = racks.Count(RackHasCurrentMark)` while the plan is confirmed.
- **Load policy:**
  - `ScenarioOneSaveMigration` normalises every save older than v21 to model 0, an empty plan, `nextRackId 1` and every order at `rackId 0`. Nothing is synthesised.
  - `RestoreSaveData` reads rack data only from a v21 save (`nextRackId = max(1, saved)`).
  - V2 is active only when `standGeometryModel == Enlarged80` **and** model 1 (OPEN-1 / D-064).
  - `StandGeometryModel.ForSave` treats every save older than v20 as Legacy40.
- **Disk path** (`ForestSaveController`):
  - `Save()` writes pretty JSON atomically (`.tmp`, then replace, keeping `.bak`).
  - `CanLoad()` decides whether the title screen offers Continue.
  - `Load()` and Continue run `TryReadSave`: JsonUtility, then the Geometry/Storm/Competition/`ValidateRackV2Json` presence checks, all catching parse exceptions. They then run `FindLoadProblem`, the full semantic validation including `ValidateRackV2`, before the first world mutation.
- RACK-V2-09 added no field: the orange mark rides on the existing `entranceTreeId`/`entranceMarked`. G2 schema order confirms that v21 is final for Rack, and **this task adds no schema field**.

## Case matrix

"Before" is the coverage that existed at `6d3228c`: RACK-V2-03B `RackV2Save` (35 checks), RACK-V2-03C `RackV2Runtime` and RACK-V2-09 `RackV2FieldMarking`. Every one of those round trips runs **in memory** (CaptureData → JSON → LoadData). None writes or reads `forest-save.json`, and 03C explicitly deferred cold-start Continue to RACK-V2-11. "Added" is the new disk-path gate `Tools/Verification/RackV2Persistence/` (`RackV2PersistenceVerification.cs` + `run_gate.py`).

| Required case | Before (in-memory) | Added (real `Save()`/`CanLoad()`/`Load()` disk path) | Status |
|---|---|---|---|
| New game | 03B §A: fresh V2 identity and a v21 round trip with an exact hash | `v2` §1: file states v21 / Enlarged80 / empty V2 plan explicitly; Continue offered; reload exact after the live world was changed | written, Unity pending |
| Old save before rack planning | 03B §E: synthetic v20 projection → model 0, hash-stable | `v2` §7: a v20 Enlarged80 **file** loads model 0, V2 inactive, and the V2 APIs refuse. It re-saves as v21 with an explicit model 0, which still loads as model 0 a second time. Hash equals the empty-plan identity. | written, Unity pending |
| Old save with existing rack progress (valid state: a model-0 world with legacy `ConfirmRack` rack felling; legacy rack groups are session-only) | 03B §E: one legacy order at rackId 0; 03C §I: legacy ConfirmRack after a pre-v21 load (in memory) | `v2` §8: legacy `ConfirmRack` on two authored lines. Both the v21-model-0 file and the v20 file reload with every order and mark exact, and never enter V2. `legacy40` §3 does the same on Legacy40. | written, Unity pending |
| Save/reload after confirmation | 03B §D (synthetic); 03C §H (real confirm, in memory) | `v2` §3: real `ConfirmRackPlanFromMap`, then disk save, clear, Continue. Signature (ids, geometry, frozen sets, links, marks) and world hash are exact. | written, Unity pending |
| After partial entrance marking | 09 field gate: partial reload in memory | `v2` §4: 2 of 5 marked, including the re-added id. Disk reload is exact, and the orange paint is projected from the rack record only (`MarkType None`). `coldstart`: a **new Editor process** continues that file exactly. | written, Unity pending |
| After all marks | 09 field gate: full reload in memory | `v2` §5: 5 of 5, disk reload exact, all orange projected. A second save cycle writes identical rack records and work orders. | written, Unity pending |
| Stable rack IDs across save/load | 03C signature (contiguous ids) | `v2` §2: draft delete + re-add gives non-contiguous ids with a new, never-reused id; ids and `nextRackId` are asserted after every disk reload; `coldstart` checks them across processes | written, Unity pending |
| Legacy40 save loads unchanged and never enters V2 | 03B §J: a model-1 claim on the live grid is refused; the Repair-01 `legacy40` progression round trip | `legacy40`: fresh Legacy40 v21 and v20 files, with and without legacy rack progress, load unchanged (geometry, signature, hash), keep model 0, and the V2 APIs refuse. Legacy40 files claiming model 1, or carrying a V2 rack record, are refused. | written, Unity pending |
| Malformed V2 data loads without a crash | 03B §G/§H: 19 semantic and 3 presence cases in memory; LoadData refuses before mutation | `v2` §6: **21 malformed files on disk**, each refused by `CanLoad` with a reason. `Load()` does not throw and the world hash and signature are unchanged. The cases are: truncated, not JSON, missing nextRackId, missing racks, wrong-type racks, missing entranceMarked, missing rackId, order missing rackId, non-string tree id, duplicate property, NaN endpoint, unknown model, duplicate ids, nextRackId reuse, confirmed with 4, marked with no identity, entrance outside its set, unknown tree, link to a missing rack, model 0 with racks, pre-v21 header claiming V2. Afterwards the valid file still loads exactly. | written, Unity pending |
| No Reference Future v1 reserialisation | 03B §I: archive verified, Y100 `7AD177B3CC2F73C7` | Every mode re-verifies the archive. The runner SHA-256s `ScenarioOneReferenceFutureV1.bytes(.meta)` before and after, and fails on any change. No archive world is ever loaded or saved by the gate. | static PASS (no diff); runtime pending |
| No unrelated schema change | — | `git diff 6d3228c cc9376a -- Assets ProjectSettings Packages` is empty. `CurrentVersion == 21` is asserted at runtime. | static PASS |

No case was found to be impossible. "Old save with existing rack progress" was interpreted as the only valid pre-V2 state: model-0 legacy rack felling orders. There is no persisted V2 progress older than v21.

## Commands and results (static / offline, run at `cc9376a`)

| Check | Command | Result |
|---|---|---|
| Runner syntax | `python3 -I -B -c "import ast; ast.parse(open('Tools/Verification/RackV2Persistence/run_gate.py').read())"` | ok |
| Repository tools typecheck | `python3 -I -B Tools/Verification/CloudTypeCheck/run_typecheck.py --mode tools` | Not usable at this base. It reports 22 NEW errors, all pre-existing and none in the new files: duplicate global types across `RackV2Editing`/`RackV2Spacing`/`RackV2SuggestedLayout`/`S1CManagementHistory` offline stubs, and `Painter2D` missing from the 2021.3 references. These declaration errors stop Roslyn before it binds method bodies. |
| Focused typecheck (scratch project outside repo: `/media/jer/Files/CCF/Operations/tmp-rackv2-11/focused`) | Production `Assets/**/*.cs` + Input/Editor stubs + a scratch `Painter2D` type stub + `RackV2PersistenceVerification.cs` + the unchanged `RackV2SaveVerification.cs` as control, `UNITY_EDITOR` defined, `dotnet build` | **0 errors in either harness.** The remaining errors are the known baseline API gaps (`FindObjectsByType()`, `WarpCursorPosition`, `GetEntityId`, `numpadEnterKey`, plus UI `Painter2D` members). |
| Negative control | The same project with one harness line mutated to `manager.NoSuchRackMember` | Caught: `CS1061 'ScenarioOneManager' does not contain a definition for 'NoSuchRackMember'`. Method bodies in the harness really are bound. |
| Scope | `git diff --stat 6d3228c cc9376a` | 3 files, all under `Tools/Verification/RackV2Persistence/`. Assets/ProjectSettings/Packages/`RackV2Save` are unchanged. |

The approximation compiles against UnityEngine 2021.3 reference assemblies, not Unity 6000.6. Unity's own compiler at the gate is the real compile check.

## Protected save / reference evidence

- Player save untouched: no Unity was run, and nothing wrote to `~/.config/unity3d/DefaultCompany/CCF/` (`forest-save.json` mtime 2026-10-17, `.bak` 2026-10-02).
- When it runs, the gate uses a fresh per-run `XDG_CONFIG_HOME`. The harness refuses unless `persistentDataPath` is inside it, and it also backs up and restores that slot byte-exact. The runner hashes the real save, its `.bak` and the Reference v1 resource before and after.
- No `Assets/ForestPrototype/` staging happened, and no `.meta` was generated. The scratch typecheck projects live under `/media/jer/Files/CCF/Operations/tmp-rackv2-11/`, outside the repo.

## Unity status: WAITING

When the gate was ready (2026-10-29), the Unity lock was **held** by `DeepSeek-S1D-T06` / `S1-D-T-06` (lane `/media/jer/ZX20/CCF-U1`, ETA 2026-10-29T09:00Z). T-06 (#88) is also ahead in the G2 Unity order. The lock was not bypassed.

Two further prerequisites must be cleared before the run:

1. **Lane registration.** `/media/jer/Videos/CCF-RackV2-11` is not in `/media/jer/ZX20/CCF-Ops/lanes.json`, and `run_gate.py` refuses an unregistered lane. The umbrella (#91) or the Manager must register it, as #91 did for RackV2-09, or reassign the run to a registered lane at this exact HEAD. Note that this checkout is on `/media/jer/Videos` (ext4), not ZX20.
2. **Library.** This checkout has no `Library/`, so the first run is a cold import.

Exact next commands, once the lock is free and the lane is registered (substitute the final HEAD):

```
cd /media/jer/Videos/CCF-RackV2-11 && git status --porcelain   # must be empty
python3 -B /media/jer/ZX20/CCF-Ops/scripts/unity-lock acquire --owner ClaudeA-RACK-V2-11 --task-id RACK-V2-11 \
  --lane /media/jer/Videos/CCF-RackV2-11 --branch feature/scenario-one-rack-v2-persistence-v1 --head $(git rev-parse HEAD) --eta <eta>
E=/media/jer/Files/CCF/Verification/RackV2-11-<date>
python3 -B Tools/Verification/RackV2Persistence/run_gate.py --gate v2        --out $E/v2-01
python3 -B Tools/Verification/RackV2Persistence/run_gate.py --gate coldstart --out $E/coldstart-01 --seed-from $E/v2-01
python3 -B Tools/Verification/RackV2Persistence/run_gate.py --gate legacy40  --out $E/legacy40-01
python3 -B Tools/Verification/RackV2Persistence/run_gate.py --gate save03b   --out $E/save03b-01
python3 -B /media/jer/ZX20/CCF-Ops/scripts/unity-lock release --lock-id <id>
```

Expected markers:

- `RACKV2PERSIST_VERIFY_PASS mode=v2` (7 PASS lines, 21 `RACKV2PERSIST_REFUSED` lines, plus the archive check);
- `mode=coldstart`;
- `mode=legacy40`;
- `RACKV2SAVE_VERIFY_PASS checks=35`.

Each run must also show `protected-unchanged=True clean-after=True leftover=[]`.

A first run may fail for real. If it does, fix only the harness's own fixture mechanics, and never by weakening an assertion. A production disagreement is a finding to report and goes to G5 if it touches schema.

## Boundaries

- Verification only. No production save class, schema/version, validation rule, migration policy, Reference Future v1, scene, prefab, material, package or ProjectSettings change. No assertion was weakened or rebaselined.
- Only this worktree and branch were written. `/media/jer/ZX20/CCF-Primary` and every other worktree were not touched.
- **No Windows build** and no standalone Player build were made or attempted.
- Independent review and integration remain with the parent (#91).
