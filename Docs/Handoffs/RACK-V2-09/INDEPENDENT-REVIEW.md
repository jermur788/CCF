# RACK-V2-09 — independent review (field marking, claim #90 / umbrella #91)

```text
INDEPENDENT REVIEW
Reviewer: Claude A — T3 Code / Claude Code harness, Anthropic claude-opus-5-5 (Opus 5.5); read-only review, no subagents.
Independence: did not implement or qualify RACK-V2-09. Implementation was OpenAI Codex gpt-6.1-sol (98bd0c4..1985a7b).
               Takeover and qualification were Claude B / Opus 5.5 (8f8316b plus the docs handoff 0399402).
Reviewed candidate: 8f8316b5a815ba8e7b68a9015923f940ae2ec8d2 (production base 72dbee8b8eab66f58046abfd49e77ead4c29d357 = origin/main at review time)
Branch tip at review: 0399402f6d4c9ddf3f8d14d8ed95b6420710671f (adds only Docs/Handoffs/RACK-V2-09/HANDOFF.md)
Worktree: /media/jer/ZX20/CCF-RackV2-09 (clean before this file was written)
Unity / lock / Windows build: none used. No production, verification, spec or coordination file edited.
```

## Commands run (all read-only) and results

| Command | Result |
|---|---|
| `git diff --stat 72dbee8..8f8316b` | 11 files: 7 production C# files plus one new script `.meta`; 3 files under `Tools/Verification/RackV2FieldMarking/`. |
| `git diff --stat 8f8316b..0399402` | only `Docs/Handoffs/RACK-V2-09/HANDOFF.md`. |
| `git show 8f8316b` | `run_gate.py` lock guard (owner ∈ {Codex, ClaudeB}, **plus** `task_id == RACK-V2-09`) and one README line. No assertion, check count or harness file changed. |
| `git diff --stat 72dbee8 8f8316b -- '*.unity' '*.prefab' '*.asset' '*.mat' ProjectSettings Packages Assets/ForestPrototype/ForestSave*` | empty: no serialized asset, package, settings, save-data or validation change. |
| `git diff --quiet 72dbee8 8f8316b -- Tools/Verification/RackV2Runtime Tools/Verification/RackV2Save` | identical to main (the runtime and save gates ran unmodified owning-task harnesses). |
| `git status --porcelain` and `ls Assets/ForestPrototype` (look for gate copies) | clean; no staged harness or `.meta` leftover. |
| Overlap check: diff of each open S1-C / S1-D / HUD-fix / MFM branch since its merge-base, restricted to the 6 touched production files; also `29bb6cb` | no overlap with any of them. |
| Evidence inspection under `/media/jer/Files/CCF/Verification/RackV2-09-20261029/` (details below) | consistent with the handoff. |

## Scope (review item 1): PASS

Production files: the new `ForestTreeMarkingManager.RackEntrances.cs` (+ `.meta` GUID `a195af90…`), plus `ForestTreeMarkingManager.cs`, `ScenarioOneManager.RackPlan.cs`, `ForestPlayer.cs`, `PlannedFellMarkers.cs`, `WalkingHudView.cs` and `ControlsReference.cs`. All of these are inside the packet's targeting / cue / spray / mark / conflict / HUD-status scope. There are no geometry-authoring, spacing, map-editing, MFM, ecology, economy, save-schema, Reference Future v1, package or Windows changes. `PlannedFellMarkers` suppresses discs only for V2 rack orders (`RackPlanningV2Active && order.rackId > 0`), so model 0 is unchanged. `8f8316b` is runner/README only and keeps the exact-HEAD, lane and clean-tree guards, so the gates are not weakened.

## Implementation audit (review item 2): PASS

- **Authoritative target.** `TryGetRackEntranceTarget` reads only the persisted `entranceTreeId`. It requires that tree to be in the rack's own `removalTreeIds`, standing and `CanChop`, inside the existing `RackGeometry.DefaultWidthM` corridor, **and** equal to the existing `FindEntranceTree` result. On any failure it returns `RACK INVALID — ADJUST PLAN: …` with `tree = null` and never substitutes another tree. `TryGetRackFieldTarget` stops at the first unmarked, unlocked confirmed rack and does not skip a broken rack to cue a neighbour.
- **Use-time aim and range.** `TrySprayRackEntrance` takes no tree argument. It re-raycasts with `RaycastAll` at 3.5 m from the view and uses the nearest non-player, non-habitat collider, so an intervening collider wins. It then matches the hit tree against `ConfirmedRacks[].EntranceTreeId`, and `MarkRackEntrance` re-validates through `TryGetRackEntranceTarget`. An unrelated tree, a later removal tree, or an identity outside the set cannot satisfy it. The gate asserts the unrelated, beyond-range, aimed-away, blocker, tampered-outside-set and later-tree cases.
- **One state, one ledger.** Orange is projected from `RackSaveData.entranceMarked && entranceTreeId == tree.TreeId` (`HasRackEntranceMark`). It is never written to `MarkedIds` or `TreeMarkType`; the gate asserts `MarkType == None && !IsMarked`. `MarkedRackEntranceCount` is the existing derived `racks.Count(RackHasCurrentMark)`. No new counter or ledger exists.
- **Reciprocal conflicts.** Spray refuses Crop or Fell, both in `MarkRackEntrance` and in the prompt. `Mark`, `Unmark` and `ToggleMark`/`ToggleCropTree` refuse an orange tree. `ClearAll` only touches `MarkedIds`. Crop is refused for any rack-removal tree, which mirrors the existing v21 validation rule "rack cannot remove Crop Tree".
- **OPEN-3.** `ReopenRackPlan` is unchanged and touches no marks or orders. Existing `ReshapeRack…`/`ConfirmRackPlan` code clears `entranceMarked` only for a changed rack. `SweepStaleMarks` removes paint whose rack mark is gone (asserted: obsolete paint removed, the other four retained). Locked racks are refused by `MarkRackEntrance` and skipped by the cue. The gate asserts that an executed rack stays geometry-locked and keeps its mark.
- **Cue.** It hides immediately on a successful spray, and the next frame cues the next real rack. The gate asserts `CueRackId` moves 1 → 2 → … → 0, and that no target exists after the fifth mark.
- **Input.** The New Input System is used throughout (`Keyboard.current.pKey`, `Mouse.current.leftButton`). There is no legacy `UnityEngine.Input`. Spray requires a locked cursor on the previous frame as well, so the click that captures the cursor does not also spray. While the spray is equipped, the spray suppresses inspect clicks.

## Persistence disposition (review item 3): PASS, no RACK-V2-11 expansion

`ForestSaveData.CurrentVersion` is still 21. No save, validation or migration file is in the diff. The state rides on the existing v21 `entranceTreeId`/`entranceMarked` fields and their existing validation, which already rejects orange together with Fell or Crop, and orange outside the removal set. The field gate performs real `CaptureData` → JSON → `LoadData` round trips in memory, with no disk write:

- a partial reload (count 1, paint restored);
- an all-marked reload (5, four stripes on each);
- an ordinary Crop world loaded over stale orange (incoming Crop preserved).

That last case works because the `restoringMarks` bypass applies only during rehydration of an already-validated incoming save, and only to the live-plan guards. The guard is reset in a `finally` block. Stale orange after a mark clear or geometry change is removed by the sweep (asserted). For a harvested rack, a felled entrance tree leaves `livingById`, so the sweep removes its paint (see N1). The unchanged RACK-V2-03B save gate passes 35/35.

## Legacy / model-0 disposition (review item 4): PASS

Every V2 entry point is gated on `RackPlanningV2Active`, which requires Enlarged80 **and** model 1. This covers spray equip, update, cue, prompt, `HasRackEntranceMark`, `IsRackRemovalTree`, the `ControlsReference` P row and the `PlannedFellMarkers` skip. The gate switches the real ecology to Legacy40 with model 0 and asserts: no spray, no cue, no target, legacy controls intact (B present, P absent). `ScenarioReferenceRunner`'s `Mark(Fell)` calls run in a model-0 world, where the orange guards are inert, so Reference Future v1 is unaffected. Ordinary Fell/Crop marking is asserted unchanged. Rack removal still goes only through the existing pending `FellTree` orders. There is no second removal mechanism, and `MarkRackEntrance` creates no orders.

## Evidence audit (review item 5): PASS

| Run | `candidate.txt` | `lock.json` (owner / task / head / lane) | Verdict line | `error CS` | Exceptions |
|---|---|---|---|---|---|
| field-05 | 8f8316b5… | ClaudeB-RACK-V2-09 / RACK-V2-09 / 8f8316b5… / CCF-RackV2-09 | `RACK_V2_FIELD_MARKING_PASS checks=99 shots=10` (99 `RACKFIELD_CHECK` lines) | 0 | 1 |
| runtime-01 | 8f8316b5… | same | `RACKV2RUNTIME_VERIFY_PASS checks=14` | 0 | 1 |
| save-01 | 8f8316b5… | same | `RACKV2SAVE_VERIFY_PASS checks=35` | 0 | 1 |

- **The single exception in each log** is `ArgumentOutOfRangeException` at `UnityEditor.Search.SearchDatabase+<EnumerateAll>.MoveNext` (`Modules/QuickSearch/Editor/Indexing/SearchDatabase.cs:351`), reached from `GetDefaultSearchDatabase`. This is Unity's own QuickSearch startup indexing. There is no candidate frame. Confirmed as not candidate code.
- **Native input.** `driver/native-actions.jsonl` records 8 OS actions (1× `p` key, 7× left click), which matches the handoff.
- **Captures.** `captures.txt` lists 10 PNGs: `01-valid-target` and `02-orange-first-mark` at 1280×720, plus `03-orange-side-{0..3}` at both 1280×720 and 1920×1080. All 10 are present.
- **Lock.** `takeover-lock-release.json` shows `status: released`, `forced: false`, lock `b71b6202…`, head 8f8316b, and no Editor PIDs.
- **Side effects.** Each run's `git-status-after.txt` lists only `.vscode/settings.json` plus Section Five `.mat` files (6 for field, 4 for runtime and save). Each `editor-side-effects-restored.patch` covers exactly those paths, and the worktree is clean now.
- **Reaper.** `save-01/reaper.txt` reaped only PID 305105, after the verdict and the `Cleanup mono` marker. The "U1 Editor" wording is inherited from the copied runner (cosmetic).
- **Licence.** No `.ulf` licence file is in evidence. Earlier failed runs `field-01`..`field-03` and the passing `field-04` (on `1985a7b`) are retained.

## Findings

**Blocking: none.**

**Non-blocking**

- **N1 — brief stale paint and untested real harvest path.**
  - When an orange entrance tree is felled, `OnTreeNoLongerLiving → Unmark` returns early at the orange guard, because `HasRackEntranceMark` does not check `IsLiving`. The paint is then removed only by the next `SweepStaleMarks`, which runs at most every 0.5 s. Harmless, but brief stale orange can appear on the felled stem.
  - The gate's executed-rack fixture flips one order to `Completed` and does not fell the tree. "No stale orange on a harvested rack" is therefore proven by code inspection only.
  - Recommend that RACK-V2-12 assert it through real approve → execute.
- **N2 — Crop refusal for a non-entrance removal tree not directly asserted.** The public refusal of Crop for a *non-entrance* frozen removal tree is not directly asserted. The Crop-negative fixture uses `SetMark`. The later `Mark(target0, CropTree)` is stopped by the orange guard first. Code inspection confirms the `IsRackRemovalTree` guard. RACK-V2-12 should add the direct case.
- **N3 — per-frame cost.**
  - Every frame in a confirmed V2 world, `UpdateRackSpray` and the HUD each call `TryGetRackFieldTarget`. Each call does two `LivingTreesById()` scans (`FindObjectsByType`) and allocates `DescribeRacks()`.
  - While the spray is equipped, `RackSprayPrompt` also runs `RaycastAll`, and the HUD calls it more than once per frame.
  - Not a correctness issue, and it is in line with existing code patterns. Note for the RACK-V2-12/14 performance check.
- **N4 — out-of-order spraying accepted.** The player can spray any confirmed rack's real entrance out of cue order. That is correct against spec §21/§23: the cue is guidance, and the target is still authoritative. No change needed.
- **N5 — aim ray skips some colliders.** The aim ray skips `ForestEcologyController` colliders that are not trees, so ground-like colliders do not block. At 3.5 m reach this is negligible, but RACK-V2-12 may want to confirm the intended blocker set.

## Integration risk (review item 6)

- **File overlap.** None with the open S1-C T-03, S1-D T-06 (`29bb6cb`), `fix/rack-v2-combined-hud-layout-v1` or MFM branches.
- **`origin/main`.** Still at the base `72dbee8`, so `8f8316b` applies without conflict.
- **RACK-V2-10 (Help).** It must teach orange, red and blue using the actual P / left-click / 3.5 m controls added here.
- **RACK-V2-11 (persistence).** It owns full schema qualification on disk. This candidate adds no field, so it does not pre-empt that work.
- **RACK-V2-12.** Should pick up N1, N2 and N3.
- **RACK-V2-14 / MFM.** They read `entranceMarked` from the rack record, which is unchanged in shape.
- **HUD layout.** `WalkingHudView` now appends `RackFieldStatus` as a second line to the access text. Combined with the known pre-existing combined-HUD failures on main (Stand Map caption overflow, minimap-note clipping), the combined HUD should be re-run after integration.

## For RACK-V2-13 (rendered appearance; notes only, not judged here)

- **Stripe readability.** The stripe reads as a very thin orange sliver at 2.2 m in both `02-orange-first-mark-1280x720` and `03-orange-side-1-1920x1080`. Each patch is a 24° arc, with radius `Diameter/200 + 0.012 m`, so it may sit partly inside trunk meshes whose radius is larger than DBH/2.
- **"Four sides" visibility is not proven by the captures.** In `03-orange-side-1-1920x1080` the aim ray hits a nearer unrelated trunk, and the prompt reads "Not a rack entrance". The capture check only asserts non-black pixels. The four-stripe guarantee is structural (`childCount == 4`), not visual.
- **HUD issues already noted in the handoff.** The action prompt overlaps the lower stripe, and the marked count is duplicated in the objective card and the status line.
- **Additional HUD issues.**
  - The waypoint direction line wraps ("…within 3.5 m").
  - The transient message panel overlaps the waypoint panel at 720p.
  - A long `RACK INVALID — ADJUST PLAN: …` problem appended to the access line could overflow.

## Verdict

`8f8316b5a815ba8e7b68a9015923f940ae2ec8d2` can be integrated into the umbrella candidate as-is. No repair is required. Nothing was weakened, skipped or rebaselined.

**RACK-V2-09 REVIEW PASS — APPROVED FOR INTEGRATION**
