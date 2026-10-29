# RACK-V2-09 — field-marking implementation handoff (field, runtime and save gates green)

**Exit: `RACK-V2-09 FIELD MARKING READY — READY FOR MANAGER REVIEW`**

```text
IMPLEMENTATION HANDOFF
Task: RACK-V2-09 — physically mark each confirmed Rack V2 entrance tree in orange; claim #90 (dispatched by #91)
Implementation: OpenAI Codex / gpt-6.1-sol (T3 Code Codex harness), commits 98bd0c4..1985a7b, field gate field-04;
                stopped at its provider usage limit before the runtime/save gates.
Takeover / qualification: T3 Code / Anthropic claude-opus-5-5 (Opus 5.5), Claude Code harness, single writer,
                no subagents (account not independently verifiable from the session); lock owner ClaudeB-RACK-V2-09.
Branch / worktree: feature/scenario-one-rack-v2-field-marking-v1 · /media/jer/ZX20/CCF-RackV2-09
Starting HEAD: 72dbee8b8eab66f58046abfd49e77ead4c29d357 (origin/main; contains RACK-V2-08 merge d925e959)
Takeover start HEAD: 1985a7ba1d54389013c38bf0dd63c9c92be75286 (clean)
Exact qualified candidate (all three gates): 8f8316b5a815ba8e7b68a9015923f940ae2ec8d2
Final tip: the documentation-only commit containing this file (full SHA recorded in claim #90)
Evidence root: /media/jer/Files/CCF/Verification/RackV2-09-20261029/
Ready for review: YES
```

## Production and serialized-asset inventory (`origin/main` 72dbee8 → 8f8316b)

| File | Change |
|---|---|
| `ForestTreeMarkingManager.RackEntrances.cs` (+ new `.meta`, GUID `a195af90…`) | New partial of the existing marking manager: spray equip/stow (`P`), use-time re-raycast aim/range (3.5 m physical reach), rack-entrance spray, orange four-side trunk patches, subtle entrance cue, synthesized 180 ms spray sound, target/prompt text. |
| `ForestTreeMarkingManager.cs` | `partial`; orange ↔ Fell/Crop reciprocal guards in the existing `Mark`/unmark paths; Crop cannot enter a rack-removal set; orange projected from rack records during marker refresh; mark rehydration on load bypasses live-plan guards (`restoringMarks`) because the incoming save is already validated. |
| `ScenarioOne/ScenarioOneManager.RackPlan.cs` | New `TryGetRackEntranceTarget`; `MarkRackEntrance` now validates through it. No rack-record field or schema change. |
| `ForestPlayer.cs` | Spray click does not also open tree inspection. |
| `Racks/PlannedFellMarkers.cs` | V2 rack-linked pending orders draw no automatic ground discs (they read as orange before spraying). Model 0 unchanged. |
| `UI/WalkingHudView.cs`, `UI/ControlsReference.cs` | Waypoint to the next real entrance, derived `Rack entrances marked X / 5`, spray prompts; V2-only `P` control row. |

**Serialized assets:** none. No scene, prefab, material, ScriptableObject, ProjectSettings or package change; the only new asset file is the C# script `.meta`.

Verification tooling: `Tools/Verification/RackV2FieldMarking/` (README, disposable harness, `run_gate.py`). The takeover commit `8f8316b` changes only the runner's lock-owner guard (accepts `Codex-RACK-V2-09` or `ClaudeB-RACK-V2-09` and now also requires task id `RACK-V2-09`; exact-HEAD, lane and clean-tree guards unchanged) plus the matching README line. No assertion was changed, skipped or rebaselined. The `runtime` and `save` gates run the unchanged owning-task harnesses (`Tools/Verification/RackV2Runtime/`, `Tools/Verification/RackV2Save/`, identical to `origin/main`).

## Target-resolution rule

`ScenarioOneManager.TryGetRackEntranceTarget(rackId)` resolves only the entrance identity already persisted on the authoritative v21 rack record (`entranceTreeId`) and accepts it only if **all** hold:

1. it is a member of that rack's own frozen `removalTreeIds`;
2. it is a living, choppable tree;
3. it lies inside that rack's existing qualified 2.4 m corridor (`RackGeometry.DefaultWidthM`) along its entrance→far centreline;
4. `FindEntranceTree(entrance, removalTreeIds)` (the existing "first removal tree nearest the headland near the centreline" rule) returns the same tree.

Otherwise it returns `RACK INVALID — ADJUST PLAN: …` and **no** target. It never searches for a substitute. At spray time the tree is found by a fresh raycast and must equal some confirmed rack's `EntranceTreeId`; a cached or caller-supplied tree cannot satisfy aim or range. An unrelated tree, a later removal tree in the same rack, or an identity outside the removal set therefore cannot be marked (each is asserted in the field gate).

## Mark architecture and conflict handling

- Orange state lives only on the rack record (`RackSaveData.entranceMarked`, from RACK-V2-03B/03C). It is never written to the loose Fell ID set or the per-tree `TreeMarkType` enum. There is no second entrance-mark ledger or counter: `MarkedRackEntranceCount` and the HUD count derive from rack objects.
- The paint is a projection that shares the marking manager's `MarkerObjects` registry and exclusive mutation API with Fell/Crop. Conflicts are reciprocal: Crop Tree or Fell cannot be sprayed orange; Fell, Crop and unmark cannot overwrite orange; clearing loose Fell treatment cannot erase the rack mark; Crop cannot be added inside a frozen removal set.
- Ordinary Fell/Crop marking outside the rack-entrance interaction is unchanged (asserted).
- No `MARK-ARCHITECTURE CONFLICT` was found.

## OPEN-3 behaviour (asserted in the field gate)

Reopening cancels nothing and keeps all marks and orders. An unchanged reconfirm keeps both. Editing one unexecuted rack's geometry clears only that rack's mark: its obsolete paint is removed and the other four keep theirs. The derived count follows the rack objects. A rack with an executed removal stays locked; its removal and mark are never reversed.

## Persistence-boundary disposition

**No new schema or persistence work.** `ForestSaveData.CurrentVersion` remains 21. The existing v21 rack fields `entranceTreeId` and `entranceMarked`, and their existing validation in `ForestSaveValidation.RackV2.cs`, carry the state. The diff touches no save, validation or migration file. The field gate proves partial-mark and all-marked in-memory v21 reloads restore the count and the four-side paint. It also proves that loading an ordinary Crop world over stale orange state preserves the incoming Crop. The unchanged RACK-V2-03B save gate passes. RACK-V2-11 still owns full persistence/schema qualification.

## Verification on exact candidate `8f8316b` (Unity 6000.6.0f1, machine-wide lock, private Xephyr :84 / openbox, one Editor at a time)

| Gate | Command | Result | Evidence |
|---|---|---|---|
| field (native OS `P` + left-click) | `python3 -B Tools/Verification/RackV2FieldMarking/run_gate.py --gate field --out …/field-05` | **PASS** `RACK_V2_FIELD_MARKING_PASS checks=99 shots=10`, exit 0, 149 s, 8 native driver actions | `field-05/` |
| runtime (unchanged RACK-V2-03C harness) | `… --gate runtime --out …/runtime-01` | **PASS** `RACKV2RUNTIME_VERIFY_PASS checks=14`, exit 0, 88 s | `runtime-01/` |
| save (unchanged RACK-V2-03B v21 harness) | `… --gate save --out …/save-01` | **PASS** `RACKV2SAVE_VERIFY_PASS checks=35`, 196 s; Editor reaped after verdict + `Cleanup mono` (see note) | `save-01/` |

Runtime 14 and save 35 match the counts recorded for those harnesses by earlier Rack V2 qualifications. In all three logs: 0 `error CS`, 0 FAIL markers, and exactly one exception. That exception is Unity's own `UnityEditor.Search.SearchDatabase` QuickSearch indexing at Editor startup (`ArgumentOutOfRangeException`, `SearchDatabase.cs:351`); it is not candidate code. (`summary-lines.txt` repeats some UI Toolkit stack frames only because its filter matches the text `System.Exception&`; those are not exceptions.)

Earlier evidence on the previous candidate `1985a7b` is retained unchanged. `field-04/` passed 99/10. `field-01`..`field-03` are earlier failed or aborted attempts (Cursor compile ambiguity, range fixture), fixed by `ad8abf0`, `cffdb0f` and `1985a7b`. Because the runner commit changed HEAD, field was rerun as `field-05`, so all three gates are on one exact HEAD.

**Captures** (`field-05/evidence/`, listed in `captures.txt`): `01-valid-target-1280x720`, `02-orange-first-mark-1280x720`, `03-orange-side-{0..3}` at both 1280×720 and 1920×1080. A sampled 1080p capture shows the vertical orange trunk stripe, the next-rack cue/waypoint and the derived `Rack entrances marked 1 / 5`. RACK-V2-13 owns rendered-appearance acceptance. Observations for it: in that capture the action prompt overlaps the lower part of the stripe, and the marked count appears both in the objective card and in the lower status line.

**Fixtures / not claimed:** tree placement, map confirmation and camera pose are harness fixtures; the gate makes no native-walking claim. The Crop-negative fixture calls `SetMark` directly because the public manager correctly refuses Crop inside a frozen removal set. Legacy40/model-0 exclusion is asserted against the actual model-0 path. No independent G2/G3/G4 review was performed by this worker.

**Save-gate reaping note:** after `RACKV2SAVE_VERIFY_PASS` and the Editor's `Cleanup mono` marker, the Editor did not exit within the 90 s grace period. Per its documented rule, the runner killed only its own Editor PID 305105 (`save-01/reaper.txt`; that line says "U1 Editor", wording inherited from the copied runner, but it is this lane's Editor). The verdict was already logged.

## Unity lock, storage and housekeeping

- **Storage preflight:** ZX20 had 21 GiB free at acquire (floor 15 GiB; `unity-lock acquire` accepted). After the runs it had 20.55 GiB free. All durable output is under `/media/jer/Files/CCF/Verification/`.
- **Lock:** acquired normally at 2026-10-29T03:16:19Z: id `b71b620226dd4d27ab390e3f25ba5c80`, owner `ClaudeB-RACK-V2-09`, task `RACK-V2-09`, lane `/media/jer/ZX20/CCF-RackV2-09`, head `8f8316b`. A copy is in each run's `lock.json`. Released normally (not forced) at 2026-10-29T03:24:58Z; receipt in `takeover-lock-release.json`. Final state: free, no Editor, Xephyr or openbox process left. The original worker's orphaned lock `27b55c36…` had already been released by #91 before this takeover.
- **Editor side effects:** each run rewrote `.vscode/settings.json` and Section Five materials (six files after field, four after runtime and save). They were captured as `<run>/editor-side-effects-restored.patch` and restored from HEAD by `takeover-tools/restore_side_effects.sh`, which refuses if any other path changed. The field-05 patch is identical to the earlier `editor-side-effects-restored.patch` apart from blob index lines. After each restore, `git status` was empty.
- No staged harness script or `.meta` remains under `Assets/ForestPrototype/`. The runner's `finally` removed the copies, and none of the three gate filenames is present. The tracked pre-existing `*Verification.cs` files there are from main. No licence copy remains in evidence.

## Unrelated-work accounting

Only this lane's worktree and branch were written. `/media/jer/ZX20/CCF-Primary` (branch `ops/workflow-foundation-v1`, with pre-existing modified `AGENTS.md` / `active-tasks.md` and untracked operations docs) was not modified. No other worktree, main, Reference Future v1, package, ProjectSettings, scene, prefab, material or save-schema file was changed. Nothing was integrated to main and no PR was opened.

## Windows build

No Windows or standalone Player build was configured or run.

DASHBOARD SYNC: NOT AVAILABLE / NOT VERIFIED. The Manager/operational publisher must ingest the claim/coordination update; no generated state was hand-edited.

**`RACK-V2-09 FIELD MARKING READY — READY FOR MANAGER REVIEW`**
