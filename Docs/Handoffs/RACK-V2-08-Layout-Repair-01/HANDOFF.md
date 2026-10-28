# RACK-V2-08-LAYOUT-REPAIR-01 — HANDOFF

```text
IMPLEMENTATION HANDOFF
Task: RACK-V2-08-LAYOUT-REPAIR-01 — Combined HUD pre-existing layout repair (claim #59)
Worker: T3 Code / OpenCode Go / deepseek-v4.1-flash (primary; implementation and the Unity qualification runs)
        -> continued and closed out by T3 Code / Claude `claude-sonnet-5-5` after the user interrupted the first run
           ("continue using sonnet"). One writer, same branch and worktree throughout.
Branch: fix/rack-v2-combined-hud-layout-v1
Worktree: /media/jer/ZX20/CCF-RackV2Layout (code/static); Unity qualification lane /media/jer/ZX20/CCF-U1 under the machine-wide lock
Starting main: 4923484cd819f707207782a12754b8f7809a1f16
EXACT UNITY-TESTED CANDIDATE: 12acabd8bef9d9c86ccb82aec6e7fab4e400238a
Final tip: the commit that adds this file (docs only; Assets/ bytes identical to the candidate)
EXTERNAL EVIDENCE ARCHIVE: /media/jer/Files/CCF/Verification/RackV2LayoutRepair-20261028T160000Z/
Unity: 6000.6.0f1
Exit: COMBINED HUD LAYOUT RESTORED — READY FOR MANAGER REVIEW (guided smoke open item disclosed below)
```

## What changed (production, 2 files, +16/−7)

**A. Stand Map overflow — `Assets/ForestPrototype/UI/StandMapView.cs`.** RACK-V2-04 (`76772f4`) put the `Plan rack(s)` tool buttons on their own `map-tools` row under the layer tabs. The map and its survey caption are fixed-size, so the extra row pushed the caption below the window. The tool row now shares the tab line: a `map-tabs-row` container holds the existing `map-tabs` strip and the existing `map-tools` row side by side (12-unit gap). Nothing was removed or hidden; the buttons, names (`map-tabs`, `map-tools`, `map-plan-rack`) and handlers are unchanged.

**B. Minimap teaching note squeezing HUD panels — `Assets/ForestPrototype/UI/ScenarioOneUiRoot.cs`.** The estimate/competitor panel's `maxHeight` subtracted `Minimap.ReservedHeightPx` (which already includes the minimap's bottom margin and the gap above it) and then a further 14-unit allowance, double-counting the margin. With the Rack V2 two-line note making the minimap taller, that cost up to 11 units of panel height and clipped the last rows. The allowance is now 4 units and the panel top gap 8 (was 10). `HudMinimapView.cs` is **not** touched: the note text, its timing and its meaning are unchanged.

No save/schema, geometry, edit/confirm/reopen, ecology/economy, scene/prefab/material or package change. The branch also carries the packet's coordination row (`active-tasks.md`).

## Protected Combined HUD assertions

`Tools/Verification/ScenarioOneCombined/CombinedHudComposition.cs` is **unchanged on this branch** (the branch diff against main touches only the two production files and `active-tasks.md`). No threshold, section or fixture was weakened and no G3 conflict was found.

**Harness actually executed (disclosure).** On main the Combined harness has no rack-model switch, so the Rack V2 branch cannot be selected. The runs used the RACK-V2-08 candidate version of the file (`7dd54ee`, which adds explicit `CCF_RACK_VERIFY_MODEL` = `legacy`/`v2` selection, the introduction traversal, a V2 replacement for the legacy-only `FOCUS_05/06` rack-first assertions, and the already Manager-accepted V2 `Show trees` replacement) plus three setup-only additions made in the lane: maximise the Game view, restart the time-limited minimap note before the estimate section (so the note-on layout is measured at every size rather than depending on elapsed time — stricter, not looser), and a minimap refresh before the V2 minimap-contract check. A whitespace-insensitive comparison of every line of main's file against the executed file found **only two lines absent**: the Game-view `Show/Focus/Repaint` line (split to add maximise) and the `FindPlanRack()` locator (text prefix `Plan rack` → button name `map-plan-rack`). Every clipping/window assertion — `MAPWIN_*_TEXT_ENDS_INSIDE_THE_WINDOW`, `ESTIMATE_PANEL_IS_NOT_CLIPPED`, `COMPETITOR_LIST_IS_NOT_CLIPPED` — is byte-identical. The executed file and its patch against main are in the evidence archive (`CombinedHudComposition.used.cs`, `CombinedHudComposition.used-vs-main.patch`); #49 carries the model-selection part on its own branch, so this repair does not commit a competing copy.

## Unity evidence (candidate `12acabd`, lock `19c682ace5c5493dac7ed4451b352044`, lane `/media/jer/ZX20/CCF-U1`)

| Gate | Result |
|---|---|
| Combined HUD, model-0/Legacy40 branch, 1280×720 | PASS 225 / FAIL 0 |
| Combined HUD, model-0/Legacy40 branch, 1920×1080 | PASS 225 / FAIL 0 |
| Combined HUD, Rack V2 branch, both sizes in one run | PASS 455 / FAIL 0 |
| `MAPWIN_*_TEXT_ENDS_INSIDE_THE_WINDOW` | all PASS at both sizes in all three runs (e.g. 1280: lowest text 857.5 vs window bottom 882.5; 1920: 860.0 vs 881.7) |
| `ESTIMATE_PANEL_IS_NOT_CLIPPED` / `COMPETITOR_LIST_IS_NOT_CLIPPED` | PASS at both sizes in all three runs. Tightest: V2 1920×1080 competitor list, last label bottom 896 = panel bottom 896 (passes, zero slack) |
| RACK-V2-07 confirm/reopen/minimap gate (unchanged) | PASS 213 checks / 21 captures at 1280×720 + 1920×1080 |
| RACK-V2-04 map-planning gate | PASS 142 checks / 8 captures |
| RACK-V2-05/06 editing gate | PASS 326 checks / 17 captures |
| Offline: Stand Map 171, editing 61, spacing 11, suggested layout 147 (+ static) | all PASS |

Per-gate logs, `summary-lines.txt`, captures and runner receipts are under the evidence archive (`final-*` directories); the pre-fix baseline runs on unmodified main are in the non-`final-` directories.

## Guided/Sequential smoke (packet item 7)

**Not obtained green — disclosed, for Manager disposition.**

- *Rack V2 guided/sequential:* not runnable on this candidate. The current guided harness on main requires the first-person `B` rack confirmation (the reason #49 is BLOCKED), and the RACK-V2-08 V2 guided harness needs RACK-V2-08 production code that this branch deliberately does not contain. Rack V2 Stand Map/minimap coverage is instead given by the Combined V2 run (455), the RACK-V2-07 confirm gate (213), the RACK-V2-04 map gate (142) and the editing gate (326), all PASS.
- *Model-0 guided/sequential:* attempted with the RACK-V2-08 `guided-legacy` harness (`Tools/Verification/RackV2NoCompetingAuthor/run_gate.py --gate guided-legacy`, copied to the archive under `harness-variants/`). Three early attempts hung at Editor start-up (stale `Temp/UnityLockfile`, then a Package Manager IPC failure while the Files drive was unmounted by a server restart); the first attempt and the last complete attempt (`final-guided-legacy-smoke` and `final-guided-legacy-smoke-r5`) both ran to the real-OS-input phase. The last attempt recorded 396 `GUIDED_PASS` including 288 layout/fit/wrap/overlap checks at both sizes. It then hit the 20-minute runner timeout with exactly 7 `GUIDED_FAIL`, all in the real-input chain: `real pointer movement swivelled preview around fixed start` (the xdotool relative mouse move did not turn the camera, in this attempt and in the first), and its consequences (`rack confirmed through real OS input`, `real-input choose/reselect/cancel sequence exercised`, `real confirmation creates ordinary rack work`, the two `Show trees` checks and `enabled control: missing`, which all need a confirmed rack). No layout assertion failed. The guided real-input gate is documented as flaky in long chains; **I did not re-run it on unmodified main to prove it fails there too**, and the model-0 guided smoke is therefore not claimed as passed.
- *Why the path is considered unaffected (argument, not proof):* the repair changes only a UI container arrangement in `StandMapView` and a max-height subtraction in `ScenarioOneUiRoot`; it touches no input handling, rack-planner, order or save code, and the controls the pointer driver uses (`B` key, forest clicks) are not in the changed layout. A Manager decision is needed on whether this satisfies packet item 7 or a clean-run of the guided gate on a quiet machine is required before integration.

## Cleanup / lock

- Unity lock `19c682ace5c5493dac7ed4451b352044` (owner `T3-DeepSeek-RACK-V2-08-LAYOUT-REPAIR-01`) acquired 2026-10-28T20:31Z for exact HEAD `12acabd` on lane `/media/jer/ZX20/CCF-U1`; released normally at handoff (see claim #59 comment for the release receipt).
- Lane `/media/jer/ZX20/CCF-U1` left clean at `12acabd`: Editor-rewritten SectionFive materials and `.vscode/settings.json` were diff-inspected (material serialisation and the VS Code solution name only), saved as `u1-side-effects-final.patch` and restored byte-exactly; staged disposable harness sources and their `.meta` files removed; modified harness files restored to HEAD and archived; `Temp/` deleted; no Editor, Xephyr or openbox process remains.
- Two Editor processes hung at start-up during the guided attempts and were killed by PID (SIGKILL; they ignored SIGTERM). The first run was also interrupted by the user mid-gate; the server restart later cancelled one attempt. None of these runs is used as evidence except where named above.

## Not verified / limits

- No Windows/Player build (not authorised).
- The Rack V2 competitor-list fit at 1920×1080 has zero slack; it passes the protected assertion but any further growth of the minimap note would need another look.
- The guided/sequential smoke is not green (see above); this is the open item for the Manager.

## After integration

#49 resumes in its existing branch/worktree, merges the repaired `main`, and re-runs the gates listed in the packet (Combined HUD both models, RACK-V2-08 Guided/Sequential model-0 and V2, Legacy40, RACK-V2-07 preservation). Its Combined harness already carries the model-selection edits above.
