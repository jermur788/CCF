# RACK-V2-08 — implementation handoff (all required gates green)

**Exit: `NO COMPETING RACK AUTHOR — READY FOR MANAGER REVIEW`**

This revision supersedes the 2026-10-28 blocked handoff (`5b0ed56`). The Combined HUD blocker was repaired separately (PR #80, merge `b118d8e63f60d54c9f9a3bed6de40af83b35efc1`), brought into this branch, and every gate required by `RESUME-AFTER-LAYOUT-REPAIR.md` was re-run on the exact post-merge candidate.

```text
IMPLEMENTATION HANDOFF
Task: RACK-V2-08 — remove competing first-person rack authoring; claim #49
Executor: T3 Code / Anthropic claude-sonnet-5-5 (Sonnet 5.5), single writer, no watcher, no subagents
          (account not independently verifiable from the session). Earlier phases: Codex gpt-6.1-sol, then this same
          Sonnet 5.5 lane after a provider limit (controlled transitions recorded on #49).
Branch / worktree: feature/scenario-one-rack-v2-no-competing-author-v1 · /media/jer/ZX20/CCF-RackV2-08
Starting main (this resume): cf413c9a9059091ed26d691299a87076b7a48f5d (contains layout repair b118d8e, PR #80)
Production implementation commit: 6f3c77dc5675c5388c4bc482bd1d65be38a1f4f5 (unchanged)
Merge of repaired main: 3550326f4521a6a68a8f42502ee233cd027e451b (clean, no conflicts)
Exact tested candidate (Unity): 0904f0f6bb23b55758e3e61724f89aba5ee58ef9 (merge + coordination-row commit; Assets byte-identical to the merge)
Final tip: the documentation-only commit containing this file (full SHA recorded in claim #49)
Evidence: /media/jer/Files/CCF/Verification/RackV2-08-Resume2-20261028T233024Z/
Ready for review: YES
```

## What changed (production, `Assets/` only — 8 files, +65 / −8 against `origin/main` `cf413c9`)

| File | Change |
|---|---|
| `Racks/RackTool.cs` | `Update`, `Request`, `Enter` and `Confirm` are inert while `RackPlanningV2Active`; an already-active tool/first-use screen is released on load. Model-0 behaviour untouched. |
| `ForestPlayer.cs`, `UI/WalkingHudView.cs` | V2 tree prompt and lower summary say `[M] Stand Map · Plan Racks`; legacy `[B] Create rack` text kept for model 0. |
| `ScenarioOne/ScenarioOneManager.cs` | V2 start guidance points to the Stand Map. |
| `UI/ControlsReference.cs`, `StandaloneSessionMenu.Startup.cs` | V2 control list drops the `B` row and describes `M` as Stand Map / Plan Racks. Legacy `Rows`/`KeysLine` kept. |
| `UI/MenuHelpView.cs`, `UI/LearningObjectivesView.cs` | V2 render-time substitution of Rack help, walking help and the three optional rack practice texts. Stable lesson ids/titles and legacy lesson data unchanged. |

Verification harness/test changes (G3-dispositioned, see `G3-DISPOSITION.md`, `INTRO-G3-AMENDMENT.md`) are under `Tools/Verification/RackV2NoCompetingAuthor/` plus the split model-0 / Rack V2 coverage in the Guided, Sequential and Combined HUD gates. Not touched: Stand Map code, `HudMinimapView`, stylesheets, scenes/prefabs/materials, saves/schema, ecology/economy, MFM, Reference Future, packages, Windows build. No temporary script or `.meta` remains under `Assets/ForestPrototype/`.

## Former route inventory and result

`RackPlanningV2Active` (explicit rack model 1 **and** Enlarged80) is the identity guard; Enlarged80 geometry alone never selects V2, so historical model-0 Enlarged80 keeps its path.

| Former route | Rack V2 result | Model 0 / Legacy40 |
|---|---|---|
| `B` key, left/right click, Enter/numpad Enter, wheel (`RackTool.Update`) | inert: no tool, no `RackTool.AnyActive`, no plan/counter/allocator/work change (real OS input in the Guided V2 gate) | preserved (native B/origin/swivel/reselect/cancel/confirm) |
| First-use/deferred entry (`Request`, help close → `Enter`) | inert | preserved |
| Direct tool API (`SelectOrigin`/`ReselectOrigin`/`Turn`/`Confirm`) and legacy backend `ConfirmRack` | inert; no authoritative `AddRack`, plan unchanged | preserved |
| Preview panel / `hud-rack-panel` | never shown | preserved |
| Tree prompt, refined HUD prompt, lower HUD summary | `[M] Stand Map · Plan Racks`, no `B` | `[B] Create rack` kept |
| Startup controls, rack first-use/help, walking help, start guidance | map-only copy | legacy copy kept |
| Objective / optional rack curriculum text | V2 render-time substitution; ids/titles unchanged | legacy text kept |
| Stand Map `Plan Racks` (authoritative) | immediately available; real map reset + `Confirm Rack Plan` | n/a |
| Ordinary Fell/Red, Crop Tree/Blue, inspection, `E` | work via real keys | work |

## Verification on the exact candidate `0904f0f` (Unity 6000.6.0f1, one Editor at a time, machine-wide lock, private Xephyr, native input)

| Gate | Result | Evidence folder |
|---|---|---|
| Guided + Sequential, **model-0** Enlarged80 fixture (native B/origin/reselect/cancel/confirm incl. the seven real-OS-input assertions, original `!Harvested` regrowth, legacy `Show trees`) | **PASS** 600 / 0 (rerun; see disclosure 1) | `unity-guided-legacy-rerun1/` |
| Guided + Sequential, **Rack V2** (all former routes inert, map-only copy, ordinary Fell/Crop/inspect, `M`→`Plan Racks`, map reset+confirm, full progression, user-approved regrowth split, real Work Plan job lines) | **PASS** 553 / 0 | `unity-guided-v2/` |
| Combined HUD, **model-0**, 1280×720 + 1920×1080 | **PASS** `CHC_DONE pass=450 fail=0 shots=24` | `unity-combined-legacy/` |
| Combined HUD, **Rack V2**, 1280×720 + 1920×1080 | **PASS** `CHC_DONE pass=455 fail=0 shots=24` (rerun; see disclosure 2) | `unity-combined-v2-rerun1/` |
| Genuine Legacy40 native first-person B/origin/pointer/reselect/cancel/confirm, ordinary work, approval/year advance | **PASS** | `unity-legacy40/` |
| **Unchanged** RACK-V2-07 confirm / reopen / minimap gate | **PASS** `checks=213 shots=21` | `unity-confirm-preserve/` |

The layout repair works on this branch: no `MAPWIN_*` caption overflow and no estimate/competitor clipping in either model (both failure families that blocked the previous handoff).

Offline / static (merged tree, run from this worktree with `python3 -I -B`; `git status` empty before and after):

- runtime source compile **0 errors** (173 files, installed Unity references); Editor/harness type-compile with `UNITY_EDITOR` **0 errors**;
- RackV2 editing 61, spacing 11, suggested layout 147 (plus its static check), non-UI offline suite (keyboard / rack bay / rack sample / approach 49) all pass;
- `check_integrity.py` against base `8f8e6f0` and against `cf413c9`: **PROTECTED_INTEGRITY_PASS** (all original non-intro `Check` calls retained; Guided 54→62, Sequential 34→36, Combined 70→81);
- RACK-V2-07 static check: **PASS 10/10 with `--base 6f3c77d`**. With its default base (RACK-V2-07's `36d82f0`) check 3 reports `RackTool.cs` changed, which is the intended RACK-V2-08 change; the `--base 6f3c77d` run proves no authoritative rack/save source drifted after the production commit, including through the merge. Both logs are in the evidence folder.

Not run: StandMapParity and the RACK-V2-05 real-pointer editing gate (Stand Map editing code is not in this task's diff; the Combined HUD and RACK-V2-07 map/confirm gates exercise the repaired map on this candidate). Windows/Player build is not authorised and was not run.

## Disclosures for review

1. **Model-0 Guided first attempt failed; rerun passed.** `unity-guided-legacy-run1-FAIL-machine-load-avg20/` — 594 checks passed, then the real-OS-input chain failed (6 `GUIDED_FAIL` lines covering the seven unwaived assertions): after `B`, Escape and click the origin was set, but the preview angle never moved more than 10° (final 7.2°). The same Editor/driver code passed 600/0 before the merge, the driver script is unchanged from `8f8e6f0`, and the merge brought in no runtime change that touches mouse look (only a panel-height adjustment, the Stand Map tools row, and the new advisory `ScenarioCycleState`). The machine load average was about 20 on 4 cores during that attempt (browser processes not owned by this task); on the rerun it was about 5 and the identical gate passed 600/0 with the same assertions. The cause was **not proven** to be load; both runs are kept. Nothing was weakened, skipped or rebaselined. If this gate fails again on a busy machine, check load before suspecting product code.
2. **Rack V2 Combined HUD first run was invalid; rerun passed.** `unity-combined-v2-run1-FAIL-blank-game-view-captures/` — 430 pass / 25 fail: all 24 `SHOT_IS_NOT_BLANK` checks failed (the captures were uniform white, "no real surface") plus `FOCUS_06_V2_CONFIRMED_SOLID_MAP_MINIMAP_CONTRACT` at 1280×720 (passed at 1920×1080; not individually diagnosed, likely a pixel-based check on the same blank capture). The immediate rerun on the same candidate passed 455 / 0 with 24 real captures. Model-0 Combined HUD passed on its first run.
3. **V2 Work Plan assertion correction (carried forward, already Manager-accepted).** The V2 replacement for the legacy `Show trees` check asserts the real per-job rack-owned lines with enabled Sell/Remove; the protected legacy `Show trees` assertion is unchanged in model 0 and passes.

## Unity ownership and housekeeping

- Lock acquired normally for every gate under owner `T3-Claude-RACK-V2-08` (task RACK-V2-08, lane `/media/jer/ZX20/CCF-U1`, head `0904f0f`) and released with owner and lock id after each; receipts are `lock-acquire-*.json` / `*/lock-release.json`. Final state: lock free, no Editor, U1 clean and left detached at `0904f0f`.
- Editor side effects (`.vscode/settings.json` + five Section Five materials) were byte-compared with the previously inspected rewrite, captured as patches in each run folder and restored; U1 `git status` empty. No licence copies remain in evidence.
- Storage: ZX20 was at 8.5 GiB free against the 15 GiB lock floor at resume. With the user's instruction to use the Videos partition, three stale scratch directories from `CCF-Temp` (ReviewScratch2-codex-20261022-r2, ReviewScratch-codex-map-20261022, s1nonui) and four clean, pushed, merged worktrees (RackV2-04V2, -05, -06, -07) were copied to `/media/jer/Videos/CCF-ZX20-overflow/` (byte-verified; worktrees re-registered with `git worktree repair`, HEAD and clean status re-checked), then removed from ZX20. Receipt: `relocate-to-videos.log`. Nothing else was deleted. **ZX20 is back down to about 11.5 GiB free after the Editor runs, below the 15 GiB lock floor**, so the next Unity worker's `unity-lock acquire` will refuse until more space is freed; the Manager should plan that.
- No save/schema, ecology/economy, scene/prefab/material, package/settings, MFM, Windows/Player-build change. No PR yet; Manager review and integration decision pending. CCF-Primary's unrelated dirty work was never touched.

DASHBOARD SYNC: NOT AVAILABLE / NOT VERIFIED. Manager/operational publisher must ingest the claim/coordination update; no generated state was hand-edited.

## History (superseded, in Git)

Blocked handoffs for this task (`2f9d849` first-person-authoring G3, `5b0ed56` Combined HUD pre-existing layout defect) were cleared by `G3-DISPOSITION.md`, `INTRO-G3-AMENDMENT.md`, `COMBINED-HUD-MANAGER-DISPOSITION.md` and the layout repair (PR #80).
