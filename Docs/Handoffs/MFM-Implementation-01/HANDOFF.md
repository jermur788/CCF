# MFM-01 — Prerequisite / Latest-Lineage Qualification

**Task:** `MFM-01`  
**Feature:** Multi-Scale Forestry Management V1  
**Packet:** `Docs/Handoffs/MFM-Implementation-01/TASK-PACKET.md`  
**Worker:** OpenCode / `deepseek-v4.1-flash`  
**Mode:** read-only base qualification (no production edits, no Unity lock)

**VERDICT: `MFM BASE QUALIFIED WITH LIVE UNITY CONFIRMATIONS DEFERRED TO MFM-02/03`**

The integrated Scenario One lineage satisfies the MFM-01 prerequisite contract. Rack V2 persistence, stable rack identity/geometry, save/world identity, Legacy40 exclusion and Reference Future compatibility are present and already have integration evidence. The MFM-02 geometry contract is **not yet implemented** in production code: current Lessons 2–5 still use the historical authored middle rack line and a nearest-neighbour half-width. Those are known MFM-02 work items, not an MFM-01 blocker. The exact live sample cannot be measured in this read-only lane, and the player-facing Rack V2 authoring path (RACK-V2-04/05) is not integrated yet.

## Inspected identity

| Item | Exact value |
|---|---|
| Inspected `main` | `fe8199441b5ab69c7644797b3e3792cc3c3674eb` (`origin/main` at inspection) |
| Required Rack V2 integration | `f23549b18b17b3542ec8f633d7309b11020990d4` |
| Qualified Rack V2 production HEAD | `7b557fd43ca9468a1be22c161c24064eec9f955a` |
| Rack V2 production bytes in integration | Identical: `git diff --name-only 7b557fd..f23549b` changes only `Docs/` |
| Commits after `f23549b` on inspected `main` | `2dd0b20`, `8aa462c`, `78b82e4`, `fe81994`; all `Docs/` only |
| Worktree | `/media/jer/ZX20/CCF-MFM01` |
| Branch | `handoff/mfm-01-qualification-v1` (created from `origin/main` at the inspected SHA) |
| Coordination claim | `461edcfb441ad7835147c8e8f6125475e711fb6a` — claim #14 marked `RUNNING` before substantive inspection |
| Unity lock | Not acquired. No Unity run. |

The primary checkout `/media/jer/ZX20/CCF-Primary` was **not** used or changed. It remains on `ops/workflow-foundation-v1` with its pre-existing modified `AGENTS.md` / `active-tasks.md` and untracked `Docs/Operations/StorageArchitecture.md` / `Docs/ScenarioTwoPreProductionAudit.md` preserved.

## What was inspected

- `Assets/ForestPrototype/ForestSaveData.cs`, `ForestSaveController.cs`, `ForestSaveValidation*.cs`;
- `Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.RackPlan.cs`, `ScenarioOneManager.Racks.cs`, `ScenarioOneManager.cs` (capture/restore/approval/completion paths);
- `Assets/ForestPrototype/Racks/RackGeometry.cs`, `RackPlanner.cs`;
- `Assets/ForestPrototype/ScenarioOne/ScenarioOneRackLayout.cs`, `ScenarioOneRackBay.cs`, `ScenarioOneRackBayReader.cs`, `ScenarioOneRackReview.cs`, `ScenarioOneHeadlandGeometry.cs`;
- `Assets/ForestPrototype/UI/LearningObjectivesView.Sequence.cs`, `ScenarioOneTeachingSequence.cs`, `ScenarioOneRackBayText.cs`, `RackLayoutTracker.cs`;
- `Assets/ForestPrototype/UI/Map/StandMapSource.cs`, `StandMapModel.cs`;
- `Assets/ForestPrototype/ForestTree.cs`, `ForestTreeMarkingManager.cs`, `ScenarioOneWorkOrder.cs`;
- canonical Rack V2 handoffs and Manager review at `f23549b` / `7b557fd`;
- `Docs/Handoffs/MFM-Implementation-01/TASK-PACKET.md`, D-064/D-065, the MFM spec/plan/tasks and the live coordination table.

## Prerequisite table

| Prerequisite | Result | Evidence |
|---|---|---|
| Current `main` descends from `f23549b` | **PASS** | `git merge-base --is-ancestor` true; inspected `main` is `fe81994`. Only documentation/coordination changed after `f23549b`. |
| Persisted authoritative confirmed-rack objects exist | **PASS** | Save v21 `ScenarioOneSaveData.rackPlanningModel`, `rackPlanConfirmed`, `nextRackId`, `racks` (`ForestSaveData.cs:81-92`); `ScenarioOneManager.Racks` / `RackPlan` / `ConfirmedRacks` / `TryGetRack` snapshots (`ScenarioOneManager.RackPlan.cs:30,48-63`). Prior `RACKV2COMBINED_CONTINUE_PASS checks=46` and `RACKV2SAVE_VERIFY_PASS checks=35`. |
| Stable rack IDs and geometry survive save/load | **PASS** | `RackSaveData` stable positive `rackId`, entrance/far endpoints, frozen `removalTreeIds`, entrance identity/mark (`ForestSaveData.cs:99-117`); deep clone and restore (`ScenarioOneManager.cs:1420,1467-1470,2681-2685`); validation rejects duplicate IDs, degenerate geometry, unknown trees, missing confirmation fields (`ForestSaveValidation.RackV2.cs:68-147,163-200`). Prior cold-Continue gate proved IDs/geometry/order links exact. |
| World identity / old-save policy explicit | **PASS** | `RackPlanningModel` 0/1; `RackPlanningV2Active` requires model 1 **and** Enlarged80 (`ScenarioOneManager.RackPlan.cs:37-40`); pre-v21 saves normalize to model 0/no racks (`ScenarioOneSaveMigration.cs:29-39`); v21 JSON presence is mandatory (`ForestSaveValidation.RackV2.cs:163-200`). |
| Legacy40 does not enter Rack V2 | **PASS** | `RackPlanningV2Active` geometry gate; `ForestSaveValidation` rejects model 1 on non-Enlarged80 (`:34-36`); new-game V2 identity only for Enlarged80 (`ScenarioOneManager.cs:480-484`, `RackPlan.cs:89-94`); prior `RV2L_LEGACY40_PASS checks=15` and model-0 regression identical. |
| Reference Future v1 remains frozen/compatible | **PASS** | Reference requires `Legacy40` (`ScenarioReferenceArchive.cs:110-116`); empty V2 state strips the added bytes for historical hashes (`ScenarioReferenceArchive.cs:199-230`); prior `REFERENCE_FUTURE_V1_PASS` and exact anchors `7AD177B3CC2F73C7` / `BFFF66015A6619DA`. |
| Headland/access geometry + confirmed-rack APIs can support a representative rack-served sample | **PASS (static)** | `ScenarioOneHeadlandGeometry.cs:7-23`; `RackPlanRack` exposes entrance/far/angle/length/width/status/removal IDs (`RackGeometry.cs:360-392`); prior headland/access gates `HEADLAND_QUALIFICATION_PASS checks=124`, `HEADLAND_ACCESS_PASS checks=8`. |
| Exact data to order confirmed racks along the common normal and select the third/central rack | **PASS (API data), MFM-02 helper absent** | `ConfirmedRacks`/`Racks` + `RackPlanRack.Entrance/Far/AngleDegrees`; `RackGeometry.AngleDegrees`, `Direction`, `Across`, `Along`, `LengthM` (`RackGeometry.cs:86-112`). No common-normal/order/third-rack helper exists. |
| Current save schema identity | **PASS** | `ForestSaveData.CurrentVersion = 21` (`ForestSaveData.cs:7`). |
| Current teaching sequence / `ScenarioOneRackBay` / Lessons 2–5 | **PASS (static)** | Eight lessons, indices 0–7 (`ScenarioOneTeachingSequence.cs:28-67`); rack-bay analysis and frame (`ScenarioOneRackBay.cs`); live caller at `LearningObjectivesView.Sequence.cs:61-66`. |
| MFM-02 third-rack selection | **LIVE UNITY CONFIRMATION REQUIRED** | Data/API present; no selector exists; no player-authored V2 rack plan can yet be created because RACK-V2-04/05 are not integrated. |
| MFM-02 anchor = closest point to stand centre, clamped ±12 m | **LIVE UNITY CONFIRMATION REQUIRED** | `ScenarioOneRackBay.Frame` currently uses the start/end **midpoint** (`ScenarioOneRackBay.cs:116-122`), not the closest point to `(0,0)`. |
| MFM-02 fixed 9.0 m teaching half-width | **LIVE UNITY CONFIRMATION REQUIRED** | Current code uses `ScenarioOneRackBay.HalfWidth` = half the gap to the nearest authored line; the offline fixture measured **8.004 m** (`NONUI_RACKBAY_PASS`). No `TeachingHalfWidthM = 9.0` exists. |
| 8 m teaching Crop Tree spacing separate from 8 m Hegyi | **PASS (value), MFM-02 should separate constant** | The value is 8 m, but it is currently literally `FutureTreeSpacingM = ForestEcologyController.HegyiCutoffMeters` (`ScenarioOneRackBay.cs:20-22`), so the two parameters are coupled in code. |
| Rack corridor exclusion from actual confirmed Rack V2 geometry | **LIVE UNITY CONFIRMATION REQUIRED** | `ScenarioOneRackBay.InAccess` still compares against the authored layout lines (`ScenarioOneRackBay.cs:147-152`), not confirmed rack records. |
| Rack-served coverage boundaries separate from teaching footprint | **LIVE UNITY CONFIRMATION REQUIRED** | Not implemented. `HalfWidth` is the only strip derivation and it is used for the teaching footprint. |
| Outer remainders explicit | **LIVE UNITY CONFIRMATION REQUIRED** | Not implemented. No coverage-unit/remainder type exists. |
| No conflicting writer on inspected files | **PASS** | Live coordination has no active production writer on these files. See ownership section. |

## Current save-schema identity

- `ForestSaveData.CurrentVersion = 21` (`ForestSaveData.cs:7`).
- Rack V2 fields are in `ScenarioOneSaveData`: `rackPlanningModel`, `rackPlanConfirmed`, `nextRackId`, `racks` (`ForestSaveData.cs:81-92`).
- One rack record: stable `rackId`, entrance/far centreline endpoints, frozen `removalTreeIds`, `entranceTreeId`, `entranceMarked` (`ForestSaveData.cs:99-117`).
- The only per-order rack provenance is `ScenarioOneWorkOrder.rackId` (`ScenarioOneWorkOrder.cs:56-59`).
- **There are no prescription, coverage, candidate, fit, approval or provenance fields.** MFM must use the next available schema after v21; do not assume a version number now.

## Authoritative rack API/data map

Read `ScenarioOneManager.Racks` / `RackPlan` / `ConfirmedRacks`; do **not** rebuild rack state from `RackGroup`, authored layout lines or order scans.

| Need | Authoritative source |
|---|---|
| Is this a V2 rack world? | `ScenarioOneManager.RackPlanningV2Active` (`RackPlan.cs:37-40`) |
| Raw persisted records | `ScenarioOneManager.Racks` (`RackSaveData`) (`RackPlan.cs:30`) |
| All racks / confirmed-only snapshots | `RackPlan`, `ConfirmedRacks`, `TryGetRack` (`RackPlan.cs:48-63`) |
| Stable identity | `RackPlanRack.Id`; `RackSaveData.rackId` |
| Centreline | `RackPlanRack.Entrance`, `.Far`; `RackSaveData.entranceX/Z`, `farX/Z` |
| Direction/angle | `RackPlanRack.AngleDegrees`; `RackGeometry.AngleDegrees` |
| Perpendicular offset / common normal | `RackGeometry.Across(point, angle)` / `Direction(angle + 90)` — **no convenience helper** |
| Along-rack position | `RackGeometry.Along`, `RackGeometry.LengthM` |
| Corridor width | `RackPlanRack.WidthM` (currently `RackGeometry.DefaultWidthM = 2.4 m`) |
| Frozen membership | `RackPlanRack.RemovalTreeIds` |
| Entrance identity / mark | `RackPlanRack.EntranceTreeId`, `.EntranceMarked` |
| Lifecycle / lock | `RackPlanRack.Orders`, `.IsLocked`; `RackOrderState` |

**Reopen caveat:** `ConfirmedRacks` returns an empty list whenever `rackPlanConfirmed` is false (`RackPlan.cs:50-51`). `ReopenRackPlan()` sets that flag false without deleting the persisted racks. A teaching sample that must survive a reopened plan cannot rely on `ConfirmedRacks` alone; MFM-02 must either use the persisted `Racks`/`RackPlan` records or freeze the teaching rack identity when the sample first opens. OPEN-3 (reopen-after-execution) remains unresolved.

## Current lesson / rack-bay geometry map

- `ScenarioOneTeachingSequence.Lessons` has eight presentation lessons: access (0), future-trees (1), release (2), habitat (3), response (4), recruitment (5), establishment (6), transformation (7) (`ScenarioOneTeachingSequence.cs:28-67`).
- `Current(Facts)` returns 4 while Lesson 5 review is incomplete; it returns 5 when `ReleaseReviewed` and (on rack worlds) `RackReviewComplete` are true (`:69-80`). The new `Extend your treatment` step belongs after index 4 and before recruitment.
- `LearningObjectivesView.TeachingFacts()` still builds the Lessons 2–5 area from the **authored layout**:
  - `routes = ui.RackTool.Planner.LayoutLines` (`Sequence.cs:26`);
  - `layout = ui.RackTool.Planner.LayoutStatus()` (`:62`);
  - frame = `layout[layout.Count / 2]` plus `ScenarioOneRackBay.HalfWidth(routes, routes.Count / 2)` (`:63-65`).
- `ScenarioOneRackLayout` derives five authored lines from every eighth planted row on Enlarged80; `RackPlanner.Refresh` rebuilds them (`ScenarioOneRackLayout.cs:20-30`, `RackPlanner.cs:90-97`).
- `ScenarioOneRackBay.Frame`:
  - origin = **midpoint** of start/end (`:116-119`);
  - `Contains` = ±12 m along, half-width across, excluding the 1.2 m rack corridor (`:126-127`);
  - `InAccess` excludes authored `rackLines`, not confirmed rack corridors (`:147-152`);
  - `HalfWidth` = half the nearest authored-line gap (`:308-315`).
- Current offline fixture: middle authored line half-width **8.004 m**, **69 trees**, **1.29490 m²** post-access basal area; 13 removals, 24.7377% BA, minimum release 30.0599% (`NONUI_RACKBAY_PASS`). These remain reference-layout fixtures, not protected product invariants.
- `ScenarioOneRackReview` is session-only: inspections, J/K decisions and six stand portions reset on load and are not saved (`ScenarioOneRackReview.cs:1-7,20-24`). If Lesson 5 decisions are later used to derive a prescription, spec §15 requires them to become deliberate persisted inputs.
- In the qualified Rack V2 fixture the confirmed racks lay on the authored lines, so Lesson 2 was reachable. The combined-repair handoff explicitly records that a player who authors racks elsewhere would still be taught against the authored middle line (known limitation). MFM-02's third-confirmed-rack rule is the fix.

## Current map ownership / Stand Map source model

- There is **one** shared `StandMapModel`, built by `StandMapSource`; the full Stand Map and HUD minimap both read it (`StandMapModel.cs:4-6`, `StandMapSource.cs:5-19`).
- `StandMapSource.ReadRacks()` still reads `RackLayoutTracker.Lines` (authored layout) plus session-only `RackGroups`; it does **not** read persisted `Racks` / `ConfirmedRacks` (`StandMapSource.cs:219-242`).
- `RackLayoutTracker.Lines` deliberately remains the authored layout: “Lines stays the authored layout because the Stand Map still draws it” (`RackLayoutTracker.cs:9-12`). V2 progress itself reads persisted rack objects and `rackId` orders (`:30-56`).
- The headland/feeder is not drawn on the Stand Map or minimap; RACK-V2-02B-R records this as a map follow-up.
- Consequently, MFM coverage/sample/candidate map layers cannot be added by merely exposing the existing rack layer. They need new plumbing from confirmed rack/prescription state, and that is MFM-08 work (with RACK-V2-04/07 owning their own map changes). MFM-02 can remain a geometry/domain task.

## Current Work Plan ownership and approval boundary

- `ApprovePendingWork()` calls `AddMarkedTreesToWorkPlan(false)` and then approves **every** valid `Pending` order (`ScenarioOneManager.cs:1204-1233`).
- `AddMarkedTreesToWorkPlan` consumes every pending `Fell` mark, creates ordinary felling orders, then clears Fell marks; Crop Tree marks persist (`:1117-1145`).
- `AdvanceYear()` executes only `Approved` orders (`:1236+`).
- Rack-owned orders cannot be removed or cancelled individually; they are managed through the rack plan (`CancelApprovedOrder` / `RemovePendingOrder` reject `rackId > 0`; `ScenarioOneManager.Racks.cs`).
- `ScenarioOneWorkOrder.rackId` is the only provenance link. There is no prescription ID, coverage unit, candidate confirmation or approval state, and no separate proposal state.
- Therefore MFM proposals must not be `TreeMarkType.Fell` and must not be ordinary `Pending` `ScenarioOneWorkOrder`s before prescription approval; otherwise the existing Work Plan approval would commit them. MFM-06/07 must add the provenance/approval layer.

## Current Crop Tree / Fell / Rack-mark states and conflicts

- `TreeMarkType` is mutually exclusive: `None`, `Fell`, `CropTree` (`ForestTree.cs:16-23`); `SetMark` enforces one value (`:85-99`).
- `ForestTreeMarkingManager.MarkedIds` contains **only pending Fell marks**; Crop Tree status lives on the tree. X refuses a Crop Tree; C toggles Crop Tree; Fell marks are consumed when added to the Work Plan (`ForestTreeMarkingManager.cs:200-248,524-541`).
- Rack entrance mark is separate persisted state: `RackSaveData.entranceMarked` / `RackPlanRack.EntranceMarked`. `MarkRackEntrance` refuses a Crop Tree or Fell-marked entrance (`ScenarioOneManager.RackPlan.cs:371-377`); save validation also forbids a rack removal set containing a Crop Tree and a marked entrance carrying Fell/Crop (`ForestSaveValidation.RackV2.cs:102-119`).
- There is no `Proposal` mark type and no third mark slot. A proposal must remain a distinct domain state until approval; do not overload `Fell`.

## Current completion / review chronology relevant to “Extend your treatment”

- Lesson 5 (`response`, index 4) completes when the release year is reviewed and the rack-area review is complete. `RackReviewComplete` is session-only; Annual Review acknowledgement is persisted as `AnnualReviewAcknowledged` (`ScenarioOneManager.cs:388-400`).
- Inserting the new flow after index 4 shifts recruitment/establishment/transformation to indices 6/7/8. Code that hard-codes later indices must be updated: `LearningObjectivesView.Sequence.cs` `NeedsReviewAcknowledgement` (uses index 6), `TeachingInstruction` (6/7), `HudInstruction` (6/7), `TeachingProgress` (5/6/default); `ScenarioOneRackBayText` uses only 1–4 and is unaffected.
- Scenario completion is independent of the teaching index: `EvaluateProgress` requires all objectives plus `ScenarioOneCompletionRules.HasReviewedTwoCycleSequence(managementEvents, workOrders)` (`ScenarioOneManager.cs:2366-2373`). That rule counts **all** resolved Work Plans and requires an annual acknowledgement between the first and second. MFM-generated work can therefore become a qualifying intervention and must be qualified (MFM-09/11) so it neither accidentally short-circuits nor blocks completion.
- Spec §15: if the current Lesson 5 `[J]/[K]` review choices are used to derive a later prescription, they must first become persisted inputs; session-only review state must not silently affect durable prescriptions.

## MFM-02 contract reconciliation

| Contract input | Current code | Static conclusion | Live need |
|---|---|---|---|
| Teaching rack = third confirmed rack ordered along common normal | `ConfirmedRacks` sorted by rack id; no ordering helper | API data is sufficient; MFM-02 must add deterministic ordering (common normal, `Across`, tie-break by `Id`) | Real five-rack plan; verify third rack is central and stable |
| Anchor = closest point to stand centre, clamped ±12 m | `Frame` midpoint | Not implemented; add anchor-based frame/window | Actual anchor coordinates and clamp |
| Fixed 9.0 m teaching half-width | `HalfWidth` = nearest authored gap/2; fixture 8.004 m | Not implemented; add `TeachingHalfWidthM = 9.0` | Live footprint counts/zones |
| 8 m teaching Crop Tree spacing separate from 8 m Hegyi | Same constant (`FutureTreeSpacingM = HegyiCutoffMeters`) | Value correct, parameter not separate | Regression that both remain 8 m and independent |
| Rack corridor exclusion from actual confirmed geometry | Authored `rackLines` comparison | Not implemented; use confirmed segments/width | Actual corridor/tree exclusion |
| Rack-served coverage boundaries separate from teaching footprint | Only `HalfWidth` | Not implemented; add half-distance coverage units | Actual unit boundaries/areas |
| Outer remainders explicit | None | Not implemented | Actual remainder areas and manual/excluded state |

## Exact MFM-02 patch surface prediction

1. **`Assets/ForestPrototype/UI/LearningObjectivesView.Sequence.cs`**
   - Replace the V2 area/frame construction at `:61-66` with a V2 branch that selects the teaching rack and passes the selected frame + actual rack corridors.
   - Keep `layout[layout.Count / 2]` / `HalfWidth(routes, ...)` for model-0/Legacy40 only.
   - Keep the existing V2 access-year branch at `:29-32`; do not reintroduce authored-line predicates for V2.
2. **`Assets/ForestPrototype/ScenarioOne/ScenarioOneRackBay.cs`**
   - Add `TeachingHalfWidthM = 9.0f` (name per approved contract), leaving `DefaultHalfWidthM` for model 0.
   - Add an anchor-based `Frame` construction (or `FromAnchor(anchor, direction, halfWidth)`), not midpoint-only.
   - Replace `InAccess`’s authored-line test with actual confirmed rack segment/corridor exclusion, or accept an explicit corridor set.
   - Ensure `RackCount`, `RackIndex`, `CentreZ`, `HalfWidthM` used by `ScenarioOneRackBayText` are truthful for V2 (five racks, index 2) or add explicit fields.
   - Declare a separate `TeachingCropTreeSpacingM = 8f` rather than aliasing `HegyiCutoffMeters`.
3. **New plain geometry helper** (name provisional, e.g. `Assets/ForestPrototype/ScenarioOne/ScenarioOneRackSampleGeometry.cs`):
   - common-normal derivation from confirmed rack angles;
   - deterministic ordering by perpendicular offset, tie-break `rackId`;
   - select index 2 of 5;
   - closest-point anchor with `Clamp(dot(-entrance, direction), HalfLengthM, LengthM - HalfLengthM)`;
   - rack-served unit boundaries at half-distance between adjacent racks;
   - explicit left/right outer remainders and corridor exclusion;
   - no UnityEngine dependency where practical.
4. **`Assets/ForestPrototype/ScenarioOne/ScenarioOneRackBayReader.cs`**
   - Add an overload/parameter for confirmed rack corridors so the reader does not infer them from authored `rackLines`.
5. **`Assets/ForestPrototype/UI/ScenarioOneRackBayText.cs`**
   - Verify `AreaName` (“the middle rack” for 5/index 2) and wording against the selected confirmed rack; no functional geometry there.
6. **Tests**
   - Extend `Tools/Verification/ScenarioOneNonUi/OfflineLogic/RackBayOfflineCheck.cs` and `run_nonui_offline.py` (or add a V2 companion) for: shuffled rack IDs → same third rack; varied angles/offsets; anchor/clamp; 9.0 m; 8 m spacing independence; actual corridor exclusion; coverage half-distance; outer remainders; model-0 unchanged.
   - Retarget `Tools/Verification/ScenarioOneNonUi/UnityGates/RackLessonVerification.cs` for a live V2 confirmed plan; keep the model-0 gate separate.
   - Re-run save/load, `RackV2SaveVerification`, `ScenarioReferenceVerification`, `ScenarioOneCompletionVerification`, and the existing model-0/Legacy40 regression.
7. **Not MFM-02:** `UI/Map/*` coverage rendering (MFM-08), prescription/save/proposal layers (MFM-04/06/07), teaching insertion/copy (MFM-09). Do not make these changes in the geometry adapter.

## Live measurements still required by MFM-02/03

**MFM-02**
1. A real five-rack confirmed V2 plan (once RACK-V2-04/05 authoring exists, or an approved non-destructive fixture): rack IDs, endpoints, angles, widths, confirmation/reopen state.
2. Common normal and third-rack selection; verify the selected rack is the intended central rack and remains stable across save/load and a reopened plan.
3. Closest-point anchor, unclamped/clamped distance, ±12 m endpoints, and actual footprint tree IDs/counts; near/middle/far zones and both sides.
4. Actual 9.0 m footprint with confirmed-rack corridor exclusion; compare with the historical 69-tree / 1.29490 m² fixture; label any new fixture values rather than silently rebaselining.
5. Three Crop Trees at 8 m spacing, 30% release and 40% safeguard still feasible; model-0/Legacy40 behavior unchanged.
6. Save/load and reopen behavior; Reference/Legacy anchors unchanged.
7. Rendered/HUD inspection copy and teaching transition into Lesson 2; “middle rack” wording matches the selected rack.

**MFM-03**
1. Current unit metrics: living stems/density, mean DBH, mean local competition, species composition, regeneration/juvenile presence, marks/open work, access/clearing and former-edge dense band.
2. Coverage-unit boundaries and four-state classification (covered/not covered/excluded/manual), including outer remainders and rack corridors.
3. Dedicated candidate spacing and confirmation workload. The unmerged MFM-03B evidence recommends a **16 m, unit-centre-first, spacing-only** candidate set (5–10 candidates, ~14.90% maximum unit BA in its reference layout), but that is unaccepted supporting evidence, not a production rule.
4. Calibration impact on basal area, retained canopy, cash/economy, deadwood/timber and long-run completion before the extension becomes default (MFM-11).

## Ownership / conflict findings

- **No conflicting active production writer** on the inspected files. Claim #14 is this task.
- Claim #6 (`S1-C-IMPL-02`) is `READY TO RESUME`, not running; it owns `UI/AnnualReviewView.cs` and narrow `ScenarioOneUiRoot.cs`, which this task did not touch.
- Claims #11 (PARITY-04) and #13 (S1-D-T01) are read-only/dispatched; no production overlap.
- **RACK-V2-04** (`Docs/Handoffs/RACK-V2-04/TASK-PACKET.md`, dispatched at `fe81994`) is not registered in the live coordination table. It will own `StandMapView` / `UI/Map/*` map-planning presentation; MFM-08 must serialize with it. RACK-V2-05+ authoring/editing are not implemented, so a player-authored confirmed plan cannot yet be produced.
- **MFM-03B documentation branch** `docs/mfm-03b-release-feasibility-20261027` exists at `4f250d7` (base `b07dd8a`), adding only `Docs/Verification/MFM03BReleaseFeasibility/HANDOFF.md`. It is not integrated and not registered in live coordination. Its handoff refers to “accepted MFM-01 and MFM-03” and recommends the 16 m calibration; current canonical coordination still lists MFM-01 as dispatched and does not accept MFM-03B. Treat it as unaccepted supporting evidence; do not treat it as canonical or merge it blindly. It has no production-file overlap with this task.
- **Canonical context is stale for Rack V2.** The latest context-changing Overview commit is `eb218ea` (2026-10-27 09:54), before Rack V2 integration (`f23549b`, 19:34) and before save v21. `Docs/Project/unity-project-overview.md` does not currently record Rack V2/save v21. MFM-02 should lock to an updated context commit or explicitly use the inspected implementation as authority; do not rely on the stale Overview for Rack V2 facts.
- The task packet has no explicit `CONTEXT COMMIT` field; this handoff records the inspected `main` SHA instead. That is a packet-format gap for the Manager, not an implementation conflict.
- The primary checkout’s dirty files and the player save were preserved. No Unity lock, no production file, no serialized asset and no package/settings change was made.

## Static verification run

| Gate | Result |
|---|---|
| `python3 -I Tools/Verification/ScenarioOneRackUx/OfflineLogic/run_rack_check.py` | `RACK_OFFLINE_PASS checks=65` |
| `python3 -I Tools/Verification/ScenarioOneNonUi/run_nonui_offline.py` | `NONUI_KEYBOARD_PASS checks=46`; `NONUI_RACKBAY_PASS checks=87`; `NONUI_APPROACH_PASS checks=49` |
| Current rack-area fixture | **8.004 m** half-width; **69 trees**; **1.29490 m²** post-access BA; current 8 m spacing coupled to Hegyi |
| `git diff --name-only 7b557fd..f23549b` | only `Docs/` |
| `git diff --name-only f23549b..fe81994` | only `Docs/` |
| V2 authoring call-site check | `AddRack`, `ConfirmRackPlan`, `UpdateRack`, `DeleteRack`, `ReopenRackPlan`, `MarkRackEntrance` have **no player-facing UI/scene callers** on inspected `main`; only disposable verification tools under `Tools/Verification/RackV2*` exercise them |

Offline evidence: `/media/jer/Files/CCF/Verification/MFM01-PrerequisiteQualification-20261027/offline/`.

No Unity run was made; therefore no live scene, save-slot, rendering or input claim is made here.

## Decisions needed

1. **Reopen policy for the teaching sample:** choose whether MFM-02 reads persisted `Racks` (survives reopening) or `ConfirmedRacks` (empty after reopen), and how the sample behaves if the plan is reopened mid-lesson. OPEN-3 remains unresolved.
2. **8 m constant separation:** approve separating the teaching Crop Tree spacing constant from `HegyiCutoffMeters` (same value today) so the protected parameters cannot drift together.
3. **Live qualification route:** confirm that MFM-02 live testing may wait for RACK-V2-04/05 authoring, or approve a non-destructive five-rack fixture for the geometry gate.
4. **MFM-03B evidence:** Manager/G2/G3 decide whether the unmerged 16 m centre-first calibration is accepted as an MFM-03 input. It is not accepted by this task.
5. **Context refresh:** update the canonical context/Overview for Rack V2/save v21 before MFM-02 production coding locks its context commit.

## Verdict

**`MFM BASE QUALIFIED WITH LIVE UNITY CONFIRMATIONS DEFERRED TO MFM-02/03`**

The Rack V2 prerequisite is integrated and structurally sound. MFM-02 can be specified against the authoritative `Racks`/`RackPlanRack` data now, but its third-rack, anchor, 9.0 m, corridor, coverage and remainder behavior is not yet implemented and must be qualified live before it becomes production teaching geometry.
