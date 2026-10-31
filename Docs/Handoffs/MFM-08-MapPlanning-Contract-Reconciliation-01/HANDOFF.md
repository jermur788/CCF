# MFM-08 MapPlanning Contract Reconciliation 01 — Handoff (#196)

**Success token:** `MFM-08 MAPPLANNING CONTRACT RECONCILED — READY FOR MANAGER REVIEW`

## Claim and execution

| Field | Value |
|---|---|
| Claim | #196 (parent #201/#91) |
| Executed by | #201 long-running lane; DeepSeek V4.1 Flash as orchestrator/default bounded worker |
| Branch | `verify/mfm-08-mapplanning-contract-reconcile-v1` |
| Starting production candidate | `e5fb424635f16182748efcb0539e8a49539495a9` |
| Verification patch commit | `eef1bdf9` |
| Unity lane | `/media/jer/ZX20/CCF-U1` |
| Lock | `e30d88891af84a539c4eab6e804a37c0`, acquired and released normally |
| Evidence root | `/media/jer/Files/CCF/Verification/ScenarioOneMapPlanningReconcile-20261031T092600Z/unity/` |

## What changed

One verification file only:

`Tools/Verification/RackV2MapPlanning/RackV2MapPlanningVerification.cs` — 24 insertions, 1 deletion.

The single stale assertion

```csharp
Check(tabsNow.Count == 5, "five navigation tabs remain navigation");
```

was replaced by a state-aware contract helper `CheckLayerTabsContract(labels, rackPlanningV2Active, stage)` that proves:

1. the original five tabs exist **in order** and retain their navigation labels —
   `Access & position`, `Light`, `Regeneration`, `Browsing / protection`, `Fell & crop marks`;
2. the expected count is five when Rack Planning V2 is inactive and six when it is active;
3. `Treatment review` exists only as the **additive** sixth V2 layer, after the five originals, so it cannot replace or rename one of them;
4. the historical/non-MFM contract is also asserted through the same predicate with five labels and `rackPlanningV2Active=false` (the MFM candidate's runtime state always has V2 active, so a separate non-V2 scene cannot be expressed here);
5. the Plan Racks tool remains a sibling control, not a navigation tab (existing checks unchanged).

This is not a blind `5 -> 6` count edit: name, order, additive position and the non-V2 branch are all asserted. No unrelated check was removed, skipped, rebaselined or relaxed.

`git diff e5fb4246..eef1bdf9`: 1 file, +24/-1, all under `Tools/Verification/RackV2MapPlanning/`.

## Authoritative gate result

Complete authoritative `RackV2MapPlanning` gate on exact production candidate `e5fb4246` plus only this verification patch:

- Unity exit code **0**; Editor log `RackV2MapPlanningVerification.log`
  SHA-256 `bae8222818aceeff5f277fdc34944461ff5c10ff8f457dc71fe057f8fc3301c3`
- **`RACK_V2_MAP_PLANNING_UNITY_PASS checks=168 shots=8`**
- 168/168 `RACKV2MAP_CHECK` lines, 0 failures; no `error CS`.
- 8 captures (4 per resolution) at 1280x720 and 1920x1080; `shots.json`
  SHA-256 `f58a6575c99a026c73a0cb0efba580a1256f94dab04d124e7ec1de98b43af9c7`.
- Inspected capture `1280x720-planning-active-1280x720.png`: six navigation tabs in the required order, `Treatment review` immediately after the five originals, `Planning Racks — Active` present as a tool control, not a tab.
- New contract checks observed at both resolutions, e.g. `1280x720 navigation tab count is 6 (Rack Planning V2 active: True)` and `historical/non-MFM contract navigation tab count is 5 (Rack Planning V2 active: False)`.

The check count rises from the historical 142 to 168 because the reconciled contract adds five ordered-name assertions plus the source/destination count per resolution; no pre-existing check was dropped.

## Compile / import

- Unity imported and compiled the staged harness; script compilation completed and the gate executed.
- One benign Editor pre-warm line `VBCSCompilerPreWarmer: ERROR: Couldn't pre-warm as failed to write stub code temporary file` appears before compilation; actual compilation reported no `error CS` and the harness ran to completion. Recorded as non-blocking environment noise, not a gate failure.
- The UI renderer stack lines in the log are `Debug.Log` stack traces emitted by the harness `Check` helper, not exceptions.

## Lane / lock cleanup

- Staged `Assets/ForestPrototype/RackV2MapPlanningVerification.cs` and its generated `.meta` removed by the runner; verified absent.
- Unity/URP Editor side effects on `.vscode/settings.json` and three `Woodland_Forbs_01` materials (`Forb_Flower_Centre`, `Forb_Leaves`, `Forb_Stems`) were restored byte-exactly with `git checkout -- <paths>`; the lane is clean at the verification commit.
- Lock released normally by matching owner/task/lock id; live status `free`, metadata `null`, no Unity Editor PIDs.
- No Unity Editor, Xephyr or openbox process remains.

## Scope / caveats

- **No production change:** no UI/gameplay/scene/prefab/asset/save/schema/calibration/economy/ecology/spec/anchor edit. The production candidate remains `e5fb4246`.
- The non-MFM historical branch is asserted through the shared predicate, not by running a second non-V2 build; that limitation is explicit above.
- No Windows build. No push of any integration/release branch (see coordination note).

## Manager decision requested

#200 to review the exact verification diff and full gate result and decide whether MFM-08 #156 can be upgraded. No self-upgrade is claimed here. #196 execution is complete: `[HANDED-OFF]`.
