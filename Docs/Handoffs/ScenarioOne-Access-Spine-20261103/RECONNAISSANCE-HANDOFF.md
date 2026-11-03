# Scenario One access-spine & diagonal rack network — read-only reconnaissance handoff

**Issue:** [#246](https://github.com/jermur788/CCF/issues/246) — READ-ONLY. **Date:** 2026-11-03.
**Parent orchestrator:** DPC (local). **Overall Manager:** #200 (unchanged).
**Status:** **[HANDED-OFF]** — evidence inventory and recommendation only.
**Verdict:** **NO STOP CONDITION MET**, and **nothing is proven to fit either.** Layout A is the preferred direction but remains *unproven pending a Unity geometry measurement packet and an approved spec amendment*. The draft spine spec does collide with D-064 / Rack V2 in specific, listed places.

## 0. What this document is, and is not

It is a **source audit**: what the published candidate's code and authored data actually do, cited to file and line, plus the consequences for the draft access-spine spec.

It is **not** evidence of how the game looks or plays. Nothing here was observed in a running Unity Editor. No Unity launch, no build, no test run, no lock interaction, no save operation, no code change, and no modification of the user's personal playtest branch. Items that need runtime measurement are marked **UNMEASURED**.

Evidence labels used throughout:

- **OBSERVED** — cited repository fact at a named ref.
- **USER-CONFIRMED INTENT** — the user's stated design preference (D-067), not itself evidence of geometry.
- **INFERENCE** — reasoning from cited facts; may be wrong.
- **UNMEASURED** — cannot be established without Unity / runtime measurement.

## 0.1 Inspected refs and method

| Item | Value |
|---|---|
| Candidate (product code) | `integration/scenario-one-player-test-ready-v1` @ `72b4efbd1c653fa2231f632a79030a83ff76f833` |
| Docs and specs | `main` @ `d9b1268bc8e1ef9753c1e0216164f11bbc187205` |
| Method | read-only `git show` / `git grep` / `git ls-tree` against those refs from a clean worktree; sparse `Docs/` + `specs/` checkout for this document only |
| Unity | not launched, not inspected, no lease claimed or touched |

> **Caveat.** I could not verify that `72b4efbd` is the exact build the user is currently playtesting. It is the published tip of the candidate branch named in the packet. If the user's playtest build differs, the scene-specific claims may differ with it.

---

## 1. Access and route geometry

### 1.1 The central path is decoration with no collider

- **OBSERVED** — `Assets/Scenes/ForestTest.unity` is the only scene in `ProjectSettings/EditorBuildSettings.asset` (lines 8–9).
- **OBSERVED** — the scene contains 52 objects named `Dirt Path 00` … `Dirt Path 51` plus `Forest Clearing`. Each path object carries only `Transform`, `MeshFilter` and `MeshRenderer` — **no `Collider`**.
- **OBSERVED** — plank scale is `(2.6, 0.012, ~0.66)`, so the rendered path is **2.6 m wide** and sits 12 mm above ground, at ~0.5 m steps.
- **OBSERVED** — the authored line runs from about `(-0.93, -14.75)` at its south end to `(-2.71, 10.75)` at its north end, bowing to `x ≈ +3.0` near `z = -4.25`.
- **OBSERVED** — `Forest Clearing` sits at `(-2.77, 11)` with scale `9 × 9`.
- **OBSERVED** — `Assets/ForestPrototype/ScenarioOne/ScenarioOneHeadlandPresentation.cs` (lines 4–5) states the track is only a ground surface and that the existing ground supplies the collider.
- **INFERENCE** — walking the path is not possible *because* of the path; the player walks on the underlying ground. Any "the path is the route" claim is visual, not physical, today.

### 1.2 The approach extension is a second, Enlarged80-only part of the same route

- **OBSERVED** — `Assets/ForestPrototype/StandApproachPath.cs` (lines 36–72) clones the `Dirt Path 00` plank at runtime under names `Dirt Path X###`.
- **OBSERVED** — `Assets/ForestPrototype/StandApproachRoute.cs` builds the run from the authored south end, bends over `BendLengthM = 7` (line 18) to due south and terminates at `EndZ = -48.5` (line 20).
- **OBSERVED** — `StandApproachPath.cs` (lines 51, 140–182) uses the Enlarged80 edge at `z = -40` and adds a 6 m gate in the south ridge, a 10 m walled cutting, and a `BoxCollider` blocker.
- **INFERENCE** — the in-stand portion of the visible route is therefore roughly 51 m long (z ≈ -40 → +10.75) and stops at the clearing, about 29 m short of the north stand edge (z = +40).

### 1.3 Other surfaces that can read as "routes"

1. **Headland strip and feeder — 3D only.** `ScenarioOneHeadlandPresentation.cs` (lines 37–46) builds a procedural mesh with **no collider**, only while `RackPlanningV2Active`. The geometry comes from `ScenarioOne/ScenarioOneHeadlandGeometry.cs` (lines 7–14): strip at `x = -35.4`, `z = -37.7 … +37.7`, 2.6 m wide; feeder along `z = -27.4` from the headland to the path at `x = -2.436166`, with pads of radius 2.2 m.
2. **The approach cutting** — `Approach Ground` and its walls (`StandApproachPath.cs`, lines 140–175).
3. **Possible bare patches.** `ForestStartingStand.cs` (lines 247–251, 330–340) suppresses stems inside the headland strip, around path planks, the clearing, the player start and every buildable. **INFERENCE** — these clearances are the most likely source of "trails that look like routes but are not on the map". **UNMEASURED** — whether any reads that way on screen.

### 1.4 There is no single authoritative route geometry today

There are three overlapping sources:

- scene planks discovered by the name prefix `Dirt Path` (`UI/Map/StandMapSource.cs`, line 45);
- hard-coded constants in `ScenarioOneHeadlandGeometry`;
- racks, held in the save and in `RackPlan`.

**OBSERVED / INFERENCE** — `ScenarioOneHeadlandGeometry.PathX` (`-2.436166`) is a hand-copied duplicate of a value that `StandApproachRoute.Build` produces at runtime. If the route moves, the feeder does not follow. This matters directly for layout A: **extending the spine will not update `PathX` or `FeederZ`.**

---

## 2. Map consistency (Stand Map / minimap / 3D)

- **OBSERVED** — `UI/Map/StandMapSource.cs` (lines 5–19) builds "the ONE StandMapModel … for both the full Stand Map and the HUD minimap".
- **OBSERVED** — `UI/HudMinimapView.cs` (line 5) documents that it owns no rack or path data and hands the shared model to a surface.
- **OBSERVED** — paths reach the map only through `StandMapSource.ReadScenery` (lines ~342–377): it collects active Transforms whose name starts with `Dirt Path`, orders them by name, begins a new line on a jump of more than 2 m, and takes width from the first plank's scale. The clearing becomes a Landmark. The map re-reads when `StandApproachPath.Version` changes (line ~147).
- **OBSERVED** — the approach planks are included because they use the `Dirt Path X` prefix. **This is the existing mechanism by which a new path segment becomes visible on both the Stand Map and the minimap for free.**
- **OBSERVED** — the headland strip and feeder are **not** read by any map code. `grep` finds "headland" in map files only in legend text and prompts (`StandMapSymbols.cs`, line 94; `StandMapView.cs`, lines 600–601).
- **INFERENCE** — the headland and feeder therefore exist in 3D but not on the Stand Map or minimap. That is exactly the class of inconsistency the draft spec is written to remove. **UNMEASURED** — how visible the mismatch is.
- **OBSERVED** — racks on the map come from `ReadRacks` (lines ~260–319), reading `RackPlan` plus `GetSuggestedRackLayout()`. The 3D entrance cues read the same source (`ForestTreeMarkingManager.RackEntrances.cs`, lines 53–58). **Racks are consistent by construction; paths are not.**

**Consistency mechanisms that already work:** any segment authored as `Dirt Path…` planks reaches the map, the starting-stand clearance (`ForestStartingStand.cs`, line 330) and the MFM Track mask (`ScenarioOne/ScenarioOneCoverageWorld.cs`, line 9; `ScenarioOne/ScenarioOnePrescriptionApproval.cs`, line ~339) without extra code.

**Mechanisms that would break it:** a spine built as a second mesh type; the duplicated `PathX`/`FeederZ` constants; and the suggested layout itself, which is hard-coded to five entrance ids and east-west segments (`ScenarioOne/ScenarioOneSuggestedRackLayout.cs`, lines 22, 53–54). **A diagonal network needs new suggestion logic; nothing today synthesises diagonals.**

---

## 3. Rack V2 authoring constraints (the controlling contract, D-064)

Specification: `specs/scenario-one-objective-1-rack-planning-field-marking-v2/spec.md` (identical at `main` and in the candidate lineage).

### 3.1 The entrance rule is headland-only in code, "path/headland" in the spec

- **OBSERVED** — `Assets/ForestPrototype/Racks/RackEditing.cs` lines 162–163 return `EntranceOffHeadland` when `EntranceOnHeadland` (lines 123–125) fails. **The code accepts the western headland line only.**
- **OBSERVED** — spec §12 and §16 say "path/headland". So the code is *narrower* than the accepted spec.
- **OBSERVED** — `ResolveCreation` (lines 130–135) silently re-anchors: whichever end is nearer is treated as the entrance and snapped onto the headland (`SnapEntrance`, lines 117–121), with the far end kept.
- **OBSERVED** — `MoveAlongHeadland` (lines 139–145) slides only along the headland; `SnapFar` (lines 259–326) rotates about the entrance with a 4° angle window, a 2.5 m boundary window and a 30 m parallel search.

**Consequence for the draft's "draft a poor internal start and see why it is bad":** today an interior or disconnected start is **not explained — it is silently moved** to the western headland. That is the opposite of the teaching behaviour the draft proposes, and it is a *silent snap*, which spec §16 explicitly forbids ("Do not silently snap an invalid plan into validity").

### 3.2 The authoritative layer does not enforce the headland rule

- **OBSERVED** — `ScenarioOne/ScenarioOneManager.RackPlan.cs`, `TryValidateRackGeometry`, checks only: finite endpoints, non-zero length, and both endpoints inside the stand. No headland, no spacing, no overlap-of-geometry check at that layer.
- **OBSERVED** — the headland rule lives only in the editing facade: `ScenarioOne/ScenarioOneManager.RackEditing.cs` (lines 207–208, and `CreateRackFromMap` lines 239–249).
- **OBSERVED** — `ForestSaveValidation.RackV2.cs` enforces at most five racks (line 58) and exactly five when confirmed (line 60), plus entrance-tree bookkeeping (lines 107–121). It has **no headland check**.
- **INFERENCE** — a rack with an off-headland entrance would load from a save. The headland constraint is a *UI-authoring* rule, not a *data* rule. Any future spine-as-anchor design therefore does **not** need a save-version change just to move the entrance rule; it needs a deliberate decision about where the rule should live.

### 3.3 Five racks, and the counter

- **OBSERVED** — `RackPlanTargetCount = 5` (`ScenarioOne/ScenarioOneManager.RackPlan.cs`, line 19); `AddRack` caps at five (line ~117); `ConfirmRackPlan` requires exactly five (lines ~243–244); `CanConfirmRackPlan` requires five and every unlocked rack valid (`ScenarioOneManager.RackEditing.cs`, lines 107–128).
- **OBSERVED** — the single counter is derived: planned = `racks.Count`, marked derived (`RackPlan.cs`, lines 43–46), per spec §35.
- **OBSERVED** — spacing 16/20 m is **advisory only** (`RackEditing.cs`, lines 208–211). Nothing gates Confirm on it, although §16 says "strongly guided".

### 3.4 The four states are already distinct — keep them that way

| State | Where it lives | Citation |
|---|---|---|
| **Editable proposal** | draft rack set before confirm | `CanConfirmRackPlan`, `ScenarioOneManager.RackEditing.cs` 107–128 |
| **Confirmed, valid entrance** | `rackPlanConfirmed` + frozen `removalTreeIds` + `entranceTreeId` | `ScenarioOneManager.RackPlan.cs` ~243–276, 548–560 |
| **Marked** | `entranceMarked` set by `TrySprayRackEntrance` | `ForestTreeMarkingManager.RackEntrances.cs` 113–134 |
| **Approved work / executed** | Confirm creates **Pending** orders only; a rack with a Completed order is locked | `RackPlan.cs` 6–9, ~339; `RackGeometry.cs` 356 (`IsLocked => Completed > 0`) |

The draft spec's central distinction (plan ≠ marked ≠ approved ≠ cleared) is **already implemented**. That is good news: the spine work does not need to invent it, only to not break it.

---

## 4. Can five diagonal racks fit? What the code actually constrains

### 4.1 Real constants (all cited)

| Quantity | Value | Source |
|---|---|---|
| Stand | 80 × 80 m, half-extent 40 m, 5 m cells | `StandGeometryModel.cs` 34 |
| Tree lattice row spacing | **2.0 m** (scene-serialised; overrides the 1.9 in-code default) | `ForestTest.unity` ~5931; `RackGeometry.cs` 51 |
| Path corridor / clearing radii | 1.5 m / 2.2 m | `ForestTest.unity` ~5939–5940 |
| Rack width | default 2.4 m, min 1, max 8 | `RackGeometry.cs` 53–55 |
| Path plank width | 2.6 m | scene; `ScenarioOneHeadlandGeometry.cs` 12 |
| Headland strip | `x = -35.4`, 2.6 m wide, `z = -37.7 … +37.7` (75.4 m) | `ScenarioOneHeadlandGeometry.cs` 7–14 |
| Spacing band | 16–20 m, advisory | `RackEditing.cs` 209–210; spec §6 |
| MFM rack bay | `HalfLengthM = 12`, `TeachingHalfWidthM = 9` | `ScenarioOne/ScenarioOneRackBay.cs` 35–38 |

### 4.2 The current suggested layout is east-west, not diagonal

- **OBSERVED** — `ScenarioOne/ScenarioOneSuggestedRackLayout.cs` line 22 hard-codes five entrance tree ids: `PO0203, PO1204, PO2003, PO2803, PO3803`. Each segment is `new RackPoint(HeadlandX, z)` → `new RackPoint(farX, z)` (lines 53–54), i.e. a straight east-west line at constant `z` from the western headland to the eastern edge.
- **INFERENCE** — the row indices 2, 12, 20, 28, 38 imply gaps of about 20, 16, 16 and 20 m at 2.0 m rows, before jitter. **UNMEASURED** — the exact z values.

### 4.3 Arithmetic, with its assumptions stated

**INFERENCE.** Take a straight north–south spine of about 51 m inside the stand, with parallel branches at angle α to the spine. The perpendicular spacing between adjacent parallel branches is `d = Δs · sin α`, where `Δs` is their spacing measured along the spine. To keep `d ≥ 16 m`:

- three racks on one side needs two gaps, so about `32 / sin α` metres of spine;
- that fits inside 51 m only at **α ≳ 40°**;
- five racks need at least a 3 + 2 split across both sides;
- shallow diagonals (≤ ~30°) do not fit on one side.

This argument deliberately ignores: how branches from opposite sides meet, turning room, the clearing, and the fact that the spine does not actually run through the stand centre. **It is a plausibility check, not a layout.** It says the user's "herringbone" idea is geometrically possible in principle, and that a *shallow* one is not.

### 4.4 Unmeasurable from source

Exact spine endpoints after extension; the clearing's real extent; where entrances would land; machine turning room; whether trees stand on a proposed branch line; a defensible herringbone angle.

---

## 5. Movement, collision and "slower through the trees"

- **OBSERVED** — `ForestPlayer.cs` lines 10–11: `moveSpeed = 4`, `runSpeed = 7`; movement via `controller.Move(velocity * Time.deltaTime)` at lines 363–366.
- **OBSERVED** — `CharacterController`: height 1.8, radius 0.3, slope limit 45°, step offset 0.25.
- **OBSERVED** — **there is no path-, tree- or slope-based slowdown in `ForestPlayer`.** The ground is level, and there is no `Terrain` (see `StandMapSource.cs`, lines 17, 347).
- **OBSERVED** — the tree trunk collision proxy is a separate invisible `Trunk` object sized from stem diameter (`ForestTree.cs`, lines 278, 671–678); polished visuals have their colliders stripped (lines 500–511). **UNMEASURED** — the proxy's actual collider type and radius, which depend on the prefab.
- **CORRECTED IN REVIEW** — `PlantationBranchMovement.ContactSpeedFactor = 0.35` (`PlantationBranchMovement.cs`, line 9) exists and *is* referenced (line 126) and wired from `PlantationTreeVisual` (Register/Unregister, lines 164–197). It is **not** a player-speed mechanism: it is a branch-contact cost model for plantation tree visuals. The claim "present but unused" was wrong in detail and is withdrawn; the substantive point stands — **no player-facing route/tree slowdown exists in `ForestPlayer`.**

**INFERENCE** — with 2.0 m rows and a 0.3 m controller radius, whether the player can squeeze between trunks depends on jitter and stem radius. **UNMEASURED.**

**Design note.** No movement value proposed by the draft spec is supported by any constant found. Do not adopt a slowdown number from this document; there is no evidence for one.

---

## 6. Persistence, MFM and compatibility

- **OBSERVED** — `ForestSaveData.CurrentVersion = 24` (`ForestSaveData.cs`, line 7). v20 added `standGeometryModel`; v21 added `rackPlanningModel`, `rackPlanConfirmed`, `nextRackId`, `racks` (lines 85–96) with `RackSaveData` = id, entrance/far x/z, `removalTreeIds`, `entranceTreeId`, `entranceMarked` (lines 129–146); v22 objective history; v23 MFM prescriptions; v24 `firstGuidedRackReviewComplete`.
- **OBSERVED — documentation drift.** `Docs/Project/unity-project-overview.md` line 82 states "Current integrated schema: `ForestSaveData.CurrentVersion = 20`". The candidate is at 24. **The overview is stale and should be corrected by its owner, not by this handoff.**
- **OBSERVED** — **no path or route data is saved at all.** `StandApproachPath.cs` (lines 20–21) and `StandApproachRoute.cs` (lines 75–80) infer from the saved trees whether an Enlarged80 world was planted with the approach; `ForestSaveController.cs` (lines 355–357) applies that on load. This is a working precedent for "cosmetic/route extension that old saves simply do not get".
- **OBSERVED** — `StandApproachPath.Apply` does nothing unless the model is Enlarged80 (lines 39–42); `ScenarioOneSuggestedRackLayout` returns nothing for non-Enlarged80 (lines 31–33). Legacy40 and frozen Reference Future v1 are therefore already isolated from this feature family.
- **INFERENCE** — a spine that is *purely visual* (planks only) needs **no** schema change and cannot break old saves. A spine that becomes *authoritative* (a new anchor line, stored connectivity, per-rack spine linkage) would need a new version (v25) and a hash-compatible tail-field convention (`ScenarioReferenceArchive.WithoutRackV2Fields` / `WithoutPrescriptionState`, `ForestSaveData.cs` 85–109, `ScenarioReferenceArchive.cs` 259, 329).
- **OBSERVED — MFM coupling.** `ScenarioOne/ScenarioOneCoverageWorld.cs` (lines 8–10, 73–75) builds Track masks from the `Dirt Path*` planks **and** the headland constants, and a Clearing mask from the clearing position; `ScenarioOne/ScenarioOneManager.PrescriptionApproval.cs` (~330–340) consumes them. **Adding spine planks changes MFM coverage masks even with no new save field.**

**What would break old saves:** putting a path through trees that a saved Enlarged80 world already contains; changing `ScenarioOneHeadlandGeometry` constants; changing the five-rack validation; adding data outside the hash-compatible tail convention.

---

## 7. Smallest likely file ownership sets (for a future, separately authorised task)

| Area | Files | Conflict class |
|---|---|---|
| Path / spine geometry | `StandApproachRoute.cs`, `StandApproachPath.cs`, `ScenarioOne/ScenarioOneHeadlandGeometry.cs`, `ScenarioOneHeadlandPresentation.cs` | code; shared constants |
| Rack authoring rules | `Racks/RackEditing.cs`, `ScenarioOne/ScenarioOneManager.RackEditing.cs`, `ScenarioOne/ScenarioOneManager.RackPlan.cs`, `ScenarioOne/ScenarioOneSuggestedRackLayout.cs`, `UI/Map/StandMapRackEditor.cs` | code; **Rack V2 contract** |
| Map / minimap | `UI/Map/StandMapSource.cs` (+ `StandMapModel.cs`, `StandMapDrawPlan.cs`, `StandMapSymbols.cs`, `StandMapLegend.cs` as needed) | code; presentation |
| Persistence | `ForestSaveData.cs`, `ForestSaveValidation.RackV2.cs`, `ScenarioReferenceArchive.cs` | **high conflict, single writer** |
| MFM | `ScenarioOne/ScenarioOneCoverageWorld.cs`, `ScenarioOne/ScenarioOneManager.PrescriptionApproval.cs` | **high conflict** |
| Movement | `ForestPlayer.cs`, optionally `PlantationBranchMovement.cs` | code; tuned behaviour |
| Scene / serialized | `Assets/Scenes/ForestTest.unity` (planks, clearing, CharacterController, starting stand), tree prefabs (trunk proxy), any new ScriptableObject | **Unity YAML — single writer, never hand-edited by a cloud worker** |

This is an **ownership map, not an assignment.** #246 grants no ownership of any of these.

---

## 8. Layout trade-off

| | **A — extend central path as spine, diagonal branches** | **B — leave footpath, build separate mapped extraction spine** | **C — retain boundary/headland-only Rack V2** |
|---|---|---|---|
| **Fit to observed code** | Route data exists and is already shared by map + MFM through the `Dirt Path` prefix. Extension precedent exists (`StandApproachPath`). But the anchor rule is headland-only (`RackEditing.cs` 162) and the suggested layout is hard-coded east-west (`ScenarioOneSuggestedRackLayout.cs` 53–54). | Avoids touching the protected "path entering from outside the property" (spec §37), but needs a genuinely new geometry source, duplicating much of the headland/feeder system. | Fully consistent with the code as written. |
| **Persistence / MFM cost** | Low if visual-only (planks, no schema change). Moderate–high if the spine becomes authoritative (v25 + MFM mask change). | Higher: new source *and* new field, plus a second route system. | None. |
| **Player-visible consistency risk** | Medium — the headland/feeder 3D-only mismatch and the duplicated `PathX`/`FeederZ` must be fixed in the same change, or the inconsistency gets worse. | High — two route systems to explain; exactly the divergence the draft exists to remove. | Already shows the reported problem: headland 3D-only, entrances at the edge, racks crossing the central path. |
| **Unity measurement needed** | Spine endpoints, branch angles, entrance positions, passability, whether five fit. | Same, plus separation from the footpath. | Minimal. |
| **Verdict** | **Preferred — not proven.** Conditional on the measurement packet in §9 and an approved spec amendment. | Fallback only if A fails measurement. | Safe baseline; does not deliver D-067. |

**Manager recommendation:** pursue **A**, in two separable steps — (A1) *visual/mapped spine extension only*, which needs no schema change and directly fixes the "unmapped apparent trail" complaint; then (A2) *authoritative rack anchoring*, which is where D-064, the five-count, MFM masks and any v25 field must be re-decided. Keeping A1 and A2 as separate approvals is the cheapest way to give the user the route they asked for without spending the Rack V2 contract to get it.

---

## 9. What a Unity geometry measurement packet must establish

None of the following is answerable from source, and none of it is in this handoff:

1. True spine start/end after extension, and its distance to the north stand edge.
2. The clearing's real shape and extent, and whether a spine can pass beside it.
3. Whether five racks at ≥ 16 m perpendicular spacing fit with a chosen branch angle; the resulting entrance positions.
4. Machine turning room at spine ends, and at the headland junction.
5. Whether trunks actually block a diagonal branch line (collider type/radius; jitter).
6. Player passability between rows, and honest feedback when blocked.
7. How the path, headland, feeder, clearing and any apparent trails currently read on screen — the "why does this look like a route" question.
8. Stand Map and minimap rendering of the same set.

Recommended form: a read-only scene-measurement task producing coordinates and a plan-view annotated capture, **not** an implementation task, and **not** while the user's personal playtest holds the Unity slot.

---

## 10. Spec conflicts requiring amendment (not silent resolution)

Pinned against `specs/scenario-one-objective-1-rack-planning-field-marking-v2/spec.md` (D-064, approved) and the DRAFT `specs/scenario-one-access-spine-route-consistency-v1/spec.md`.

| # | Conflict | Controlling text | What an amendment must say |
|---|---|---|---|
| 1 | Extending the central path modifies a protected behaviour. | V2 §37 protects "path entering from outside the property"; §39.1 makes it a non-goal to change. | Whether "extend the path inward" is inside or outside the protected behaviour, explicitly. |
| 2 | The draft wants diagnosable disconnected starts; V2 forbids silent snapping *and* requires valid connection. | V2 §16: "all connect validly with the path/headland"; "Do not silently snap an invalid plan into validity". | Distinguish *drafting* (allowed, must explain) from *confirming* (must be valid). The current silent re-anchor (`RackEditing.cs` 130–135) already violates §16's second clause. |
| 3 | Code is narrower than spec: entrances must be on the headland, not "path/headland". | V2 §12, §16 vs `RackEditing.cs` 162–163. | Whether the anchor line becomes the spine, the headland, or both — and where that rule is enforced (facade only, or save validation too). |
| 4 | "The spine is not one of the five." | V2 §7, §11, §19, §35 define racks and derive the counter from the rack set. | If the spine were stored as a `RackSaveData` it would count as one of five. It needs a distinct type, or explicit non-membership wording, keeping counters derived. |
| 5 | A spine that removes trees would be a second removal system. | V2 §36; §39.6.4 stop condition. | Whether spine clearance is ever its own removal order, or is always expressed through rack orders. |
| 6 | New data must be additive. | V2 §34; §39.6.2 stop condition; D-042 Reference Future v1. | The exact v25 tail-field shape, or a decision that the spine stores nothing. |
| 7 | Work Plan gate and executed-rack locking must survive. | D-064 OPEN-2, OPEN-3; V2 §36, §39.7. | Confirmation that a spine does not add an approval path or unlock executed racks. |

D-067 states it does not override D-064. Items 1–7 are therefore **Manager decisions, not reconnaissance findings.**

---

## 11. Answering the packet's five tasks

| Packet task | Answer |
|---|---|
| 1. Exact candidate, Rack V2, scene, path assets, map/minimap; where the central path starts/ends; extra rendered trails; one authoritative route? | §1, §2. Path starts at the approach outside the property (`z = -48.5`), enters the stand at `z = -40`, ends at the clearing around `z ≈ +10.75`. It is decoration; the ground carries the player. **No single authoritative route** — planks, headland constants and racks are three sources. Extra surfaces: headland strip + feeder (3D only, unmapped), approach cutting, and tree-clearance patches. |
| 2. Authoring constraints: stand-edge/path anchoring, snapping, five racks, entrance marking, one counter, Work Plan gate; pin spec conflicts. | §3, §10. Anchoring is **headland-only**, silently enforced at creation only. Five racks, one derived counter, distinct plan/mark/approve/execute states all exist. Conflicts pinned in §10. |
| 3. Three layouts A/B/C. | §8. A preferred, unproven; B fallback; C safe baseline that does not deliver D-067. |
| 4. Reachability of the upper entrance, speed through stands, colliders, whether lines/textures match traversable geometry. | §1, §5. **UNMEASURED**, with the source-side facts recorded: no slowdown logic, no Terrain, trunk proxy exists with unknown collider details, 2.0 m rows. |
| 5. Smallest file ownership sets, save/MFM implications, Legacy40 / Reference Future v1, whether a Unity measurement packet is essential. | §6, §7, §9. Legacy40 and Reference Future v1 are already isolated by `Enlarged80`-only guards. **Yes — a measurement packet is essential** before any angled layout becomes authoritative. |

---

# Appendix A — V2 Orchestration Decision Appendix

Continues the V2 evidence effort. The 30 October historical ledger and the #241 backfill remain separate and untouched. Live append-only ledger: `Docs/Handoffs/ScenarioOne-Access-Spine-20261103/ORCHESTRATION-LEDGER.md`.

## A.1 Routing decision summary

| | Pool / model | Effort | Role | Why this one |
|---|---|---|---|---|
| Reconnaissance worker | `claudeAgent` → `claude-sonnet-5-5` (Claude Sonnet 5.5) | `medium` | `research`, read-only | Long-form, citation-heavy repository reading plus spec-to-code reconciliation. Chosen partly for **cross-provider independence**: #200 / Overall Manager operates in the GPT-6 family, so a Claude-side hand-off is independent of the authority that must accept it. |
| Independent verifier | `codex_c75d8114-…` ("ChatGPT - OpenAI B") → `gpt-6.1-terra` | `reasoningEffort: medium` | `review`, read-only | Different provider, different model, same refs. Also the designated continuity pool if the Claude pool degrades. |
| Orchestrator / author | DPC parent session (`opencode-go` → `deepseek-v4.1-flash`) | inherited | routing, verification, publication | Not a permitted worker for the recon task; used for orchestration and for this document. |

**Exploit vs explore:** the first recon route in this programme was **EXPLORE** (no prior measured cost/quality for either pool on this task class). The verification route was **EXPLOIT of independence** — a deliberate second provider rather than a second Claude pass.

## A.2 Capacity and context — what was actually observed

- **OBSERVED:** both authorised pools were **enabled** at dispatch. `orchestrator_capabilities` enumerated `claudeAgent` ("Claude A"), `claudeAgent_claude_b_pro` ("Claude B — Pro"), `codex`, `codex_c75d8114-…` ("ChatGPT - OpenAI B") and `opencode`, all with `canRunChildTask: true`. Disabled pools: `cursor`, `grok`, `antigravity`, `fx`, `muse`, `pi`.
- **UNKNOWN, recorded as UNKNOWN:** subscription quota, rate-limit window, reset time, remaining credit, and context-window headroom for either pool. **These tools expose no such telemetry.** Nothing here is estimated.
- **UNKNOWN:** token input/output, model-call count and elapsed wall time for either child. The harness did not surface them to this session. The ledger records the one timestamp that *was* observable (dispatch) and states the rest as unmeasured rather than reconstructing it.
- **Consequence:** capacity-to-completion was judged **"sufficient by task size / unknown by quota"**. The mitigation was not measurement but design: a fully self-contained brief that could be re-dispatched verbatim to the other provider with nothing to salvage.
- **Context checkpoints:** durable work was checkpointed *before* the long-running step — the issue comment, the ledger commit and the draft PR all existed before the recon result was consumed, so a lost session would not have lost the programme state.

## A.3 Failure and switch events

| Event | Classification | Action |
|---|---|---|
| Recon worker returned | normal completion | result accepted for review, not for publication |
| One citation materially wrong (`PlantationBranchMovement` described as unused) | **quality error, not a quota/cooldown failure** | corrected in §5 of this document before publication; recorded here rather than silently fixed |
| Quota cooldown / login failure | **none occurred** | no retry, no provider switch, no fallback triggered |

No provider switch was required. The verification route was *planned* independence, not a recovery switch, and the ledger records it as such rather than post-rationalising it as a fallback.

## A.4 Independence and acceptance

- **Reviewed:** the returned reconnaissance was independently citation-checked by a second provider (GPT-6.1 Medium) against the same refs. The check list, its verdicts and any correction are reproduced in Appendix B and in the ledger.
- **NOT claimed:** that this document is Unity-verified, that the layout fits, or that the draft spec is approved. **Implemented / Reviewed / Integrated / Verified / Accepted** are separate milestones and only the first two are touched here, and only for a *document*.
- **Not claimed either:** that `72b4efbd` is the exact build under playtest (see §0.1).

## A.5 Retrospective routing assessment

- **Effective:** two providers, two roles, no wasted reruns. The verifier's cost was low because it was given a fixed claim list rather than the open-ended task.
- **Cheaper sufficient path, in hindsight:** DPC could have spot-checked the twelve highest-risk citations itself and skipped the second provider. That would have been cheaper but would have lost the independent-provider property the ledger exists to evidence. Judged worth the cost for a hand-off the Manager must act on.
- **Missing telemetry:** per-child tokens, runtime and context use were unavailable. This is the main gap in the V2 record and is a harness limitation, not an omission.
- **Next action:** Overall Manager decides §10 items and commissions §9 measurement before any angled layout becomes authoritative. No implementation is authorised by this document.

---

# Appendix B — Independent verification result

*Filled from the GPT-6.1 Medium review of the same refs; see the ledger for the event record.*

---

*Read-only reconnaissance for #246. No Unity, no build, no test, no lock, no save, no production code, and no change to the user's playtest candidate.*
