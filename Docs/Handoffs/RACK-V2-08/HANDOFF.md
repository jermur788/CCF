# RACK-V2-08 — implementation handoff (Combined HUD blocked by pre-existing main defects)

**Status: NOT READY — `NO COMPETING RACK AUTHOR — READY FOR MANAGER REVIEW` is NOT claimed.**
The production change and every gate that this task owns pass on the exact candidate. The protected **Combined HUD** gate still reports failures in **both** model branches. All of them reproduce on unmodified `main` (`8f8e6f0`), in Stand Map / minimap layout code this branch does not touch. Per the direct user decision (2026-10-28, "Hand off, flag as pre-existing"), nothing was weakened, skipped or fixed here; Manager disposition is required before this branch can be called ready.

```text
IMPLEMENTATION HANDOFF
Task: RACK-V2-08 — remove competing first-person rack authoring; claim #49
Executor: T3 Code / Claude Sonnet 5.5 (continuing primary, SINGLE WRITER) after the T3 Code / OpenAI Codex
          executor hit its provider usage limit (controlled model transition, recorded on #49). No watcher, no second writer.
Branch / worktree: feature/scenario-one-rack-v2-no-competing-author-v1 · /media/jer/ZX20/CCF-RackV2-08
Base: main 8f8e6f03c65894b0d933aad23a4ad69227fb33e8 (includes docs-only OPEN-3 PR #51); context ea9c4590d9f9d959bd5c56d7738e04c611bfcdcd
Production implementation commit: 6f3c77dc5675c5388c4bc482bd1d65be38a1f4f5
Exact tested candidate (Unity): 7dd54ee679729da07013c3e129d8485c6067495f   (Assets/ byte-identical to 6f3c77d)
Final tip: the documentation-only commit containing this file (full SHA recorded in claim #49)
Evidence: /media/jer/Files/CCF/Verification/RackV2-08-Resume-20261028T130353Z/
Ready for review: NO — Combined HUD disposition first
```

## What changed (production, `Assets/` only — 8 files, +65 / −8)

| File | Change |
|---|---|
| `Racks/RackTool.cs` | `Update`, `Request`, `Enter` and `Confirm` are inert while `RackPlanningV2Active`; an already-active tool/first-use screen is released on load. Model-0 behaviour untouched. |
| `ForestPlayer.cs`, `UI/WalkingHudView.cs` | V2 tree prompt and lower summary say `[M] Stand Map · Plan Racks`; legacy `[B] Create rack` text kept for model 0. |
| `ScenarioOne/ScenarioOneManager.cs` | V2 start guidance points to the Stand Map. |
| `UI/ControlsReference.cs`, `StandaloneSessionMenu.Startup.cs` | V2 control list drops the `B` row and describes `M` as Stand Map / Plan Racks (`RowsFor`, `KeysLineFor`). Legacy `Rows`/`KeysLine` kept. |
| `UI/MenuHelpView.cs`, `UI/LearningObjectivesView.cs` | V2 render-time substitution of Rack help, walking help and the three optional rack practice texts. Stable lesson ids/titles and legacy lesson data unchanged. |

Not touched: Stand Map code, `StandMapView`/`UI/Map`, `HudMinimapView`, stylesheets, scenes/prefabs/materials, saves/schema, ecology/economy, MFM, Reference Future, packages, Windows build. No temporary script or `.meta` remains under `Assets/ForestPrototype/` (runner removes them; verified).

## Verification on the exact candidate `7dd54ee` (Unity 6000.6.0f1, one Editor at a time, machine-wide lock, private Xephyr, native input)

| # | Gate (G3 item) | Result | Evidence |
|---|---|---|---|
| 6 | Genuine Legacy40 native first-person B/origin/pointer/reselect/cancel/confirm + ordinary work + approval/year advance | **PASS** `checks=15` | `unity-legacy40-run2/` |
| 6,7 | Guided + Sequential, **model-0** Enlarged80 fixture; original native sequence incl. original `!Harvested` regrowth assertion and legacy `Show trees` | **PASS** 600 pass / 0 fail, 56 captures | `unity-guided-legacy-run2/` |
| 1-4,7 | Guided + Sequential, **Rack V2**: real OS `B`/click/right-click/Enter/numpad-Enter, deferred first-use, direct API and legacy backend all inert (no `RackTool.AnyActive`, no plan/counter/allocator/work change); map-only help/HUD/objective copy; ordinary Fell/Crop/inspect via real keys; `M` → `Plan Racks` with no forest rack; real map reset + `Confirm Rack Plan`; full sequential progression; user-approved V2 regrowth split | **PASS** 553 pass / 0 fail | `unity-guided-v2-run2/` |
| 5 | **Unchanged** RACK-V2-07 confirm / reopen / minimap preservation gate | **PASS** `checks=213 shots=21` | `unity-confirm-preserve-run2/` |
| 7 | Combined HUD, model-0 fixture (run on `a475616`; Assets and the Combined harness are byte-identical to `7dd54ee`) | **FAIL** 430 pass / 20 fail | `unity-combined-legacy-run1/` |
| 7 | Combined HUD, Rack V2 | **FAIL** 433 pass / 22 fail | `unity-combined-v2-run1/` |
| — | **Baseline: unmodified `main` 8f8e6f0**, fresh V2 world, Combined HUD sections `estimate,competitors,map-window` | **same failures** 126 pass / 22 fail | `unity-main-baseline-v2sections/` |

Offline (run on `6f3c77d`; `Assets/` unchanged since): runtime compile 172/0; Editor/harness type-compile 179/0 (re-compiled clean on `7dd54ee`); non-UI keyboard 46 / rack 87 / rack-sample 75 / approach 49, RackV2 editing 61, spacing 11, suggested layout 147, StandMap and confirmation static 10 — all pass. `check_integrity.py`: **PROTECTED_INTEGRITY_PASS** (all original non-intro `Check` calls retained in Guided 54→62, Sequential 34→36, Combined 70→81). An extra offline UX suite has 208 pass / 1 fail (obsolete Walking HUD first-use summary); compiling its inputs from `main` reproduces the same single failure; gate untouched.

Rendered review (captures in the evidence folders) confirms: V2 walking HUD and tree prompt read `[M] Stand Map · Plan Racks` with no `B` (main shows `[B] Create rack` in the same scene); V2 rack help/objective copy; ordinary Crop mark + inspection; confirmed map; Work Plan listing rack-owned jobs with real Sell / Keep / Leave / Remove controls.

## Why Combined HUD is not green — all reproduced on unmodified main

1. **`MAPWIN_*_TEXT_ENDS_INSIDE_THE_WINDOW` ×20 (10 per size, both models).** The Stand Map caption ("Local grid · metres east and north …") sits ~19 UI units below the window frame at 1280×720 and 1920×1080. Cause (observed in the capture, source-confirmed): main's `76772f4` (RACK-V2-04, 2026-10-27) puts `Plan rack(s)` on its own `map-tools` row (`StandMapView.cs:95-98, 241-251`), pushing the fixed 688 px map down. The last passing Combined HUD run of record is 2026-10-23 (`ScenarioOneUiNonUiIntegration-20261023T193031Z`), where the button sat inline with the tabs. No Combined HUD run exists in the verification archive between that and this task.
2. **`ESTIMATE_PANEL_IS_NOT_CLIPPED` / `COMPETITOR_LIST_IS_NOT_CLIPPED` (V2 world only; 2 here, 2 on the baseline).** In a V2 world the minimap shows RACK-V2-07's two-line teaching note ("Dashed lines show suggested rack positions…"), which makes the minimap taller and squeezes the panel above it (last label 2-11 units past the panel bottom). The note is time-limited (`HudMinimapView.UpdateTeachingNote`), so which panel/size is caught varies run to run (branch: estimate@1280, competitor@1920; main: estimate@1280 **identical 619/613** and estimate@1920). The model-0 branch has no note and no such failure.

None of this is in this branch's diff. The assertions were not changed, skipped or relaxed.

## Decisions needed from the Manager

- **Combined HUD disposition.** Recommended: a separate small Stand Map / minimap layout repair task (e.g. restore `Plan Racks` to the tab row or shrink the map, and make the minimap note not shrink panels), then re-run Combined HUD for **both** models on that base; this branch's Combined harness split already executes every other section (430 / 433 checks pass in the two model runs). Alternatively authorise the layout fix inside RACK-V2-08 as a bounded amendment (production scope beyond the G3 boundary; re-qualify every gate afterwards).
- **Disclosure — one new V2 assertion corrected.** The first V2 Unity run (`5bdfaae`, `unity-guided-v2-run1/`) passed 549 and failed only 3 outputs of one **new, unprotected** check inherited from the earlier executor: it required the legacy Work Plan `Show trees` control after V2 confirmation. That control exists only for session `RackGroups` made by legacy `ConfirmRack` (`ScenarioOneManager.Racks.cs:143`); V2 lists rack-owned fell orders one per line (main behaviour, unchanged). `7dd54ee` replaces it with equally strong assertions of the real presentation (no legacy group; harvest card counts all 149 rack-owned jobs; every job has its own line; real enabled Sell and Remove). The protected legacy `Show trees` assertion remains, unchanged, in the model-0 branch and passes. Reviewer should confirm this correction is acceptable.

## Unity ownership and housekeeping

- Lock: acquired normally each time and released with owner and lock id; the interrupted Codex Combined run was adopted under its own lock record (not bypassed) and released after verification. Final state: lock free, no Editor, U1 clean and left detached at `7dd54ee`.
- Editor side effects (`.vscode/settings.json` + five Section Five materials rewritten by the Editor) were byte-compared with the first inspected rewrite, captured as patches in each run folder, and restored every time; U1 `git status` clean.
- Storage floor: the user directly approved deleting only `/media/jer/ZX20/CCF-U1/Library/PlayerDataCache` (1,127,707,491 bytes, regenerable Player-build cache) to restore the 15 GiB floor; receipt `authorised-cache-cleanup-2.json`. An earlier user-approved `U3/Library` deletion is recorded in `authorised-cache-cleanup.json`. Nothing else was deleted.
- Two Editors were stopped by the runner's documented post-verdict hang policy (`reaper.txt`: both Combined runs, only after verdict + `Cleanup mono`). The next start showed a "Scene Backup Detected" dialog in the private display `:84`; **Delete** was clicked (it only removes Unity's own `Temp/__Backupscenes`); noted in `scene-backup-dialog-handling.txt`. That first confirm attempt (`unity-confirm-preserve-run1/`) therefore timed out at startup and is retained as **invalid, not a gate result**; run2 is the result.
- No save/schema, ecology/economy, scene/prefab/material, package/settings, MFM, Windows/Player-build change. No PR, no integration, no feature acceptance. CCF-Primary's unrelated dirty work was never touched.

DASHBOARD SYNC: NOT AVAILABLE / NOT VERIFIED. Manager/operational publisher must ingest the claim/coordination update; no generated state was hand-edited.

## History (superseded, in Git)

Earlier blocker handoffs for this task (first-person-authoring G3, three-page intro fixture) were cleared by `G3-DISPOSITION.md` and `INTRO-G3-AMENDMENT.md`; the bounded V2 regrowth split is recorded in the latter.

**Ready for Manager review of the Combined HUD disposition: Yes. Ready as a completed feature: No.**
