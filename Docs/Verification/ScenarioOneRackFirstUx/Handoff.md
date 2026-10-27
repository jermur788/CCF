# Scenario One playtest UX revision and rack-first tutorial — implementation handoff

Task packets: SCENARIO ONE PLAYTEST UX REVISION and its amendment SCENARIO ONE RACK-FIRST TUTORIAL (the amendment supersedes the rack section of the first). Base `8942b5c3c2adb66164573f0c12a92b31adfcb0cf`, context `36cd791b329189e0ae38931d06ae51af1a1173ce`.

- Branch: `feature/scenario-one-rack-first-ux-v1`, Unity lane U1 (`/media/jer/ZX20/CCF-U1`), Unity 6000.6.0f1.
- Production head when this handoff was written: `cd0692a` ("a press on a HUD button no longer re-captures the mouse"). A narrow follow-up, `7a886a7` (first-use window owns the keyboard; see `ModalFixHandoff.md`), changes `ScenarioOneManager.cs`, `MenuHelpView.cs` and `ScenarioOneUiRoot.cs` after it; everything below describes `cd0692a` except where `ModalFixHandoff.md` says otherwise.
- Status: **implemented and verified in the Editor and in the Linux Player; native Windows not tested.** Not integrated; the Manager decides integration.

## Summary of the handoff fields

| Field | Result |
|---|---|
| Slice A playtest UI repairs | PASS in the rendered Editor harness at 1024×768, 1280×720, 1680×1050, 1920×1080 (overlap, pick and hit-test audits), and by a real-pointer Linux Player run at 1920×1080 (below). One defect that the harness could not see was found by the Player run and fixed (HUD buttons, below). The reported More Detail failure and the top-tab hitbox offset were **not reproduced**; see "Not reproduced". |
| Rack backend | PASS. `RackGeometry` (pure code, checked offline on plain .NET) plus `RackPlanner`; deterministic, bounded start/end, corridor width configurable (default 2.4 m), boundary-to-boundary clipping to the stand, preview before any change, no persisted rack object. |
| In-world tool | PASS. B starts it; aim at a planted row; translucent corridor; mouse wheel turns it in 15° steps; trees it would take are ringed in violet (not red, not blue); concise figures card; click or Enter adds to the DRAFT Work Plan; Esc or B cancels and changes nothing. |
| Map tool | PASS. "Plan rack" appears on the Stand Map once the first rack exists; it uses the same backend, previews on a clicked square, snaps to the planted row, can nudge rows, and adds through the same confirm path. |
| Scenario One layout facts | 41 plantation rows; five racks on rows 5, 13, 21, 29, 37 (index 4, 12, 20, 28, 36), seven rows left standing between racks; 157 trees (11.7% of 1,344); see the table below. |
| Rack full-block clipping | PASS. Every layout rack runs exactly from one stand boundary to the other (±40.00 m); a partial rack takes only the stretch between its start and end. |
| Work Plan before/after | Documented below. Racks are grouped ("Extraction racks · n racks, m trees"); the "What are you doing?" heading, "Add marked trees" and "Approve pending work" are gone; the one action is "Approve Work Plan". |
| First tutorial cycle rack-only | PASS. A fresh game offers racks first; five racks → approve → advance a year → Annual Review with no individual mark made (`marks=0 crop=0`). |
| Selective marking introduced afterwards | PASS as a **soft** gate (prompts, hints, lesson order, Help). After the first intervention the tree prompt and cue change to inspection, X and C. See "Open decisions". |
| Save schema | **UNCHANGED** (version 20). No rack object is persisted. |
| Ecology / calibration changed | **NO.** Growth, regeneration, browsing, density, Model2 calibration, objective thresholds, Reference Future and historical hashes are untouched. |
| Economy changed | **NO.** The €2,500 contractor visit minimum and timber prices are unchanged. A rack is ordinary FellTree work. |
| Verification | Tables below. |
| Screenshots / evidence | Index below. |

## What changed for the player

**Help.** F1 is a modal window (framed card, scrolling body, Less/More detail and Continue buttons), with the unnecessary copy removed. First-use briefs and F1 share one More/Less detail toggle.

**Tree inspection.** The card no longer sits over the left/right panels. A neutral white cage marks the inspected tree in the world (not the red Fell colour, not the blue Crop colour). The card now leads with species, age, DBH, height, mark/status and one decision-relevant condition ("Growing space: Crowded"); stem volume moved into "If felled now"; the Overview and If-felled-now panels have equal width. The "you choose the trees…" and "first-use introductions…" lines are gone. Sitka foliage is tinted darker in code (`SitkaFoliage`, a shared copy of the Asset Store material; no third-party file bytes changed).

**Wording.** "Cell" became "square" / "map square"; "mean light" became "average forest-floor light" or "light on the ground"; "regenerating cells" became "areas with young regeneration"; the objective reads "Establish successful regeneration". The objective, thresholds and calculation are unchanged.

**Stand Map.** Opens as a modal with a dimmed backdrop that takes pointer events, so background controls receive no input. Layer tabs, header buttons and squares were verified by real clicks (below).

**Work Plan.** See next section.

### Work Plan semantics, before and after

Before (base `8942b5c3`):
- *Add marked trees* (`AddMarkedTreesToWorkPlan`) turned every living red Fell mark into a Pending FellTree work order and cleared the Fell marks (Crop designations persist). It skipped trees already in an open order and trees that cannot be felled. The same conversion also ran automatically when the Work Plan was opened and at the start of approval.
- *Approve pending work* (`ApprovePendingWork`) first did the same conversion, then validated, then moved **every valid Pending order** to Approved in one step if the whole-harvest quote was eligible and cost did not exceed cash. Nothing is felled until *Advance one year*.
- A "What are you doing?" heading sat above the actions.

After:
- Marking and racks populate the DRAFT Work Plan; opening the plan converts any remaining marks (unchanged behaviour). The redundant *Add marked trees* button is removed.
- The single footer action is **Approve Work Plan**, same call and same all-or-nothing rule as before; the label now says what it does. A rack group shows "DRAFT · not approved yet" and, after approval, "APPROVED" with Cancel approval.
- Racks are grouped at the top ("Extraction racks · 1 rack, 27 trees") with Remove rack and Show trees; removing a rack removes its whole group. Money, harvesting and residual-stand panels are unchanged and review the whole job (the €2,500 visit minimum is shared by everything felled in the visit).

## Scenario One authored rack layout

Measured by the rack harness in the Enlarged80 stand (seed and geometry as shipped); no calibration was changed.

| | |
|---|---|
| Plantation rows | 41 (rows run along X) |
| Racks | 5, on rows with index 4, 12, 20, 28, 36 |
| Rows left standing between racks | 7, 7, 7, 7 |
| Rack centre lines (Z, m) | −31.41, −16.02, 0.01, 16.01, 31.34 (spacing 15.4, 16.0, 16.0, 15.3 m) |
| Trees per rack | 32, 21, 33, 37, 34 = **157** of 1,344 living (11.7%) |
| Stem volume / sold volume | 17.89 m³ / 15.08 m³ |
| Timber income | €752.59 |
| Contractor cost (one visit, minimum) | €2,500.00 |
| Cash flow for the whole layout | **−€1,747.41** (cash €12,000.00 → €10,252.59), harvest cash flow equals the quote exactly |
| A single rack on its own | −€2,301 to −€2,402 (the visit minimum is shared when racks are felled together) |
| Neighbour rows | untouched by the harvest (328 → 328 trees); 157 stumps form the open corridors |
| Top-down preview | `30-topdown-planned-1920x1080.png`, `31-topdown-harvested-1920x1080.png` (in the planned view the two southern rows show as red planned trees, not violet: the harness saved and loaded after planting them, and rack grouping is session-only) |

The "about every eighth row, seven rows between" spacing is the authored Scenario One layout only; it is not a universal rule. Help says racks about every seventh to ninth row are common in Irish thinning. The layout is data (`ScenarioOneRackLayout.RowsBetween = 7`), not hard-coded row numbers.

The year-one net of −€1,747.41 (14.6% of starting cash) is a **teaching cost the Manager should accept or change**: the economy and completion gates still pass (below), so no calibration was touched, but the first intervention is now a cash loss by design. If that is unwanted, the packet's STOP rule applies and the option is a calibration decision (for example fewer or shorter racks), not a code fix.

## Save rule

Preview is transient. A confirmed, unharvested rack is a set of ordinary Pending/Approved FellTree orders, saved by the existing order serialisation. After harvest the rack is the physical corridor of stumps. The *grouping* of orders into a rack (for the Work Plan presentation and violet markers) is session state: after a load, rack trees appear as ordinary draft felling jobs and no tree is hidden (96-check harness: `LOAD_KEEPS_EVERY_PLANNED_RACK_TREE 53→53`, `saveVersion=20`). No save field was added.

## Verification

All gates ran on Unity 6000.6.0f1 in lane U1 against the committed tree, with disposable harnesses staged and removed by the runner. Tier-3 was **not** run.

| Check | Result | Evidence |
|---|---|---|
| Clean compile and import | PASS, zero `error CS` (also the Linux Player build of `cd0692a`, exit 0) | `compile-check/`, `build2/` |
| Offline pure logic on plain .NET, at `cd0692a` | PASS: `RACK_OFFLINE_PASS checks=23` (rack geometry), `SCENARIO_ONE_UX_PASS checks=188` (wording, first-use word budget, advance guard) | `offline/`, `ScenarioOneRackUx/OfflineLogic/run_rack_check.py`, `ScenarioOneUx/run_ux_check.py` |
| Rendered Slice A harness (shots, pick, overlap) at 4 sizes | PASS, 119 hard checks (earlier head, before the HUD-button fix; repeated on `cd0692a` below) | `final/shots/` incl. `audit.csv`, `pointer-targets.csv` |
| Rack harness (backend, tool, map, work plan, save/load, approval, harvest, rack-only cycle, stage 2) | PASS, 96 hard checks, 0 unverified (earlier head, before the HUD-button fix; repeated on `cd0692a` below) | `final/rack/` |
| Narrow regression: UX gate, Menu, U4 guidance, Startup, P2, P3, Interact | PASS on an earlier head (Menu and P3 first failed on intended wording and layout changes; their expectations were updated and they pass). **Not repeated on `cd0692a`**: that commit changes only the mouse-press handling in `ForestPlayer` and adds `ScenarioOneUiRoot.IsOverButton`; the rack and rendered harnesses were repeated | `regression/summary.txt` |
| Linux Player build and save/load smoke from `678f111` | PASS on the second attempt; the first attempt FAILED (see below) | `player-smoke-attempt1/`, `player-smoke/` |
| Real-pointer Linux Player run at 1920×1080 (borderless) | PASS except one defect found and fixed (below) | `player-pointer/` |
| Rebuilt Linux Player from `cd0692a` (build `CCF-S1-20261018-L02-cd0692a-SMOKE`) | PASS: HUD Map, Objectives and Help buttons open their screens with a human-speed click; Stand Map tabs and squares land where clicked at 1920×1080 and at 1280×720 render size (window stretched to 1920×1080) | `build2/`, `player-pointer3/` |
| Final Editor re-run on `cd0692a` (rack harness, rendered harness) | PASS: rendered harness 119 hard checks (overlap, pick and hit-test at four sizes) and rack harness 96 hard checks, 0 unverified, both on the committed `cd0692a` tree (the input-handling change was re-run, not assumed) | `final-cd0692a/` |

### Player smoke, first attempt

The first save/load smoke FAILED: the save phase failed its "Initial startup screen present" check, which runs 3 seconds after launch, before the title screen had opened on that cold start; no save was written, so the load phase then failed too ("Continue offered for the saved forest", plus an exception in `StandaloneSessionMenu.OnDestroy` during shutdown). A rerun with the same build passed both phases (markers `S1B_PLAYER_SAVE_PHASE_PASS`, `S1B_PLAYER_LOAD_RESET_PHASE_PASS`). I judge the first failure a cold-start timing flake of the smoke harness, not of the revision, but I have not proved that by reproducing it at the base commit. Both runs are kept.

### Real-pointer Player run: what was exercised

At 1920×1080, borderless, fresh config, driven with xdotool: Start New Scenario; More detail and Less detail (twice, returns to the short text); Continue; Stand Map opened by key, every layer tab, a square (H10, A1), Set waypoint (HUD shows "Waypoint A1: 57 m south-west"), Back to forest; B → rack tool → wheel → click (27 trees added, "Rack added to the Work Plan"); Tab; Work Plan grouped as a DRAFT rack; Approve Work Plan (APPROVED); Advance one year; Annual Review ("Year 1 complete", average forest-floor light, areas with young regeneration); acknowledge; Walk the forest. The clicks landed on the controls aimed at (the pressed tab highlighted exactly where clicked); first-use modals correctly swallowed clicks on the controls behind them.

### Defect found by the Player run and fixed

With the mouse freed (Esc), a click on a HUD button (Map, Objectives, Help) did nothing: `ForestPlayer` re-captured the mouse on any left press, so the button never received its release (and the press could also inspect the tree behind it). The behaviour is in the base commit. Fix `cd0692a`: a press over a UI `Button` stays with the button (`ScenarioOneUiRoot.IsOverButton`).

Controlled A/B, same correctly aimed human-speed click (press, 150 ms, release) on the Map [M] button at 1920×1080, mouse released with Esc first:

| Build | Result | Evidence |
|---|---|---|
| `678f111` (no fix; the same press handling as the base) | Stand Map does **not** open; the cursor re-locks (crosshair returns) | `player-pointer3/n02-old-after-map-button.png`, `q01-old-map-button-y269.png` |
| `cd0692a` | Stand Map opens | `player-pointer3/p01-new-map-button-y269.png` |

Objectives [O] and Help [F1] were then clicked on `cd0692a` and open (`r01`, `r04`). Two earlier "failures" in this session were my own mis-aimed clicks (about 20 px below the button, at the Editor audit's y rather than the Player's), and instantaneous xdotool clicks put press and release in one frame; they are not evidence. Note for players: the HUD buttons work only while the mouse is released (Esc), which is the documented behaviour; after closing a screen with Esc the forest takes the mouse back.

### Not reproduced

- **"More Detail does nothing."** Not reproduced in the Editor harness (every first-use screen's More/Less detail verified open and closing) nor in the Player at 1920×1080 borderless, 1600×900 decorated, 1280×720 popup. The revision hardens the path (a single toggle button with explicit labels, a modal backdrop that takes pointer events, explicit button states), but I cannot claim the owner's cause is fixed. The HUD-button defect above is the only real "button does nothing" cause I found. The owner's environment (Windows, display scaling) was not available.
- **Top-tab pointer/hitbox offset.** Not reproduced. The Editor hit-test audit passes at four sizes (`audit.csv`), and the real-pointer Player runs show each click highlighting the tab clicked, at 1920×1080 and at a 1280×720 render stretched to a 1920×1080 window (a scaling case, `player-pointer3/t*`). Cause unknown; may be specific to Windows or display scaling.

## Observations and open decisions for the Manager

1. **Selective marking is a soft gate.** X and C still work from the start; the opening stage only changes the prompts, cue, status line and lesson order. A hard gate would break existing gates and is a design decision.
2. **Rack grouping is session-only** (no schema change, by choice). After a load the five-rack progress count and violet markers fall back to ordinary planned trees. Persisting grouping needs a save-schema change: STOP-and-report item if wanted.
3. **First-year cash −€1,747.41** from the authored layout (above).
4. **Tab while a first-use brief is open** opened the Work Plan underneath the modal (seen with the rack brief), and M and O replaced or closed it. Fixed by the follow-up described in `ModalFixHandoff.md`.
5. **Foliage tint** was judged by eye (`SitkaFoliage.Tint`) and is a one-line tune.
6. **Windows native** was not tested; Linux Player only. Stage A readiness is unaffected by this handoff.
7. The Editor rewrites `.vscode/settings.json` and five SectionFive `.mat` files each run; they were restored to the base bytes and are not part of the diff.

## Files

Production (new): `Racks/RackGeometry.cs`, `RackPlanner.cs`, `RackTool.cs`, `PlannedFellMarkers.cs`; `ScenarioOne/ScenarioOneManager.Racks.cs`, `ScenarioOneRackLayout.cs`; `UI/RackLayoutTracker.cs`, `UI/InspectedTreeIndicator.cs`; `SitkaFoliage.cs` (each with its `.meta`).
Production (changed): `ForestPlayer.cs`, `ForestTree.cs`, `ForestTreeMarkingManager.cs`, `PlantationTreeVisual.cs`, `ScenarioEcologyReviewLines.cs`, `ScenarioOne/ScenarioOneManager.cs`, `ScenarioOneWorkOrder.cs`, `ManagementHistoryText.cs`, and in `UI/`: `ScenarioOneUi.uss`, `MenuHelpView(.Text).cs`, `TreeInspectionView.cs`, `ScenarioOneUiRoot.cs`, `WalkingHudView.cs`, `WorkPlanView.cs`, `WorkPlanOverview.cs`, `StandMapView.cs`, `AnnualReviewView.cs`, `ScenarioOneUiFacts.cs`, `UiKit.cs`, `AdvanceGuard.cs`, `ControlsReference.cs`, `LearningObjectivesView(.Curriculum).cs`.
Tools: new `Tools/Verification/ScenarioOneRackUx/` (`RackUxShots.cs`, `RackUxRack.cs`, `OfflineLogic/`); expectations updated in `ScenarioOneUx/Qualification/UxQualification.cs`, `ScenarioOneUx/OfflineLogic/ScenarioOneUxOfflineCheck.cs`, `MenuTutorialVerification.cs`, `ScenarioOneCompletion/P3/WorkPlanResidualVerification.cs`, `BrowsingProtectionVerification.cs`.
52 files, +3,521/−220 against the base.

## Evidence index

All under `/media/jer/ZX20/CCF-Verification/ScenarioOneRackFirstUx-20261017/` (not committed; disk is tight).

- `compare/` BEFORE/AFTER side-by-side JPEGs (Help, full help, inspection, Crop Tree inspection, Work Plan, Annual Review, Stand Map) at 1920×1080 and 1280×720; `before/`, `before-editor/` raw BEFORE; `final/shots/evidence/final/` raw AFTER (13 screens × 4 sizes) and rack screens `final/rack/evidence/rack/` (22 screens, including the top-down planned and harvested layouts).
- `final/shots/evidence/final/pointer-targets.csv` control centres per size; `audit.csv` overlap and hit-test audit.
- `regression/` per-gate output and `summary.txt`.
- `player-smoke-attempt1/`, `player-smoke/` Player save/load smoke; `player-pointer/` real-pointer screenshots on `678f111` (`a*` start/More detail, `e*`–`g*` Stand Map, `h*`–`l*` rack, Work Plan, approval, Annual Review; `c*` is a mis-aimed click, kept for honesty); `player-pointer3/` the A/B and the `cd0692a` checks (`n`, `q` old build; `o`, `p`, `r` fixed build, 1920×1080; `s`, `t` fixed build at 1280×720 render); `player-pointer2/` an early `cd0692a` run with mis-aimed clicks; `offline/` the two offline checks (`RACK_OFFLINE_PASS checks=23`, `SCENARIO_ONE_UX_PASS checks=188`); `build/`, `build2/` Player builds; `final-cd0692a/` the final Editor re-run.
