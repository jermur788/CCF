# Scenario One — Objective 1 Rack Planning & Field Marking V2 Implementation Plan

- **Spec ID:** SPEC-scenario-one-objective-1-rack-planning-field-marking-v2-V1
- **Spec status:** APPROVED
- **Plan owner:** Overall Manager
- **Created:** 2026-10-26
- **Last updated:** 2026-10-26

> **Provenance.** The *Approved plan outline* under *Proposed technical approach* is the plan as supplied with the approved spec, unchanged in substance. Everything under *Architecture / current-state findings* and *Authoring recommendations* was added at authoring time. It is labelled as verified fact or recommendation and does not create product requirements. Where it differs from the supplied outline, the outline and the spec win.

## Architecture / current-state findings

### Where these findings come from, and their limit

The authoring checkout (`ops/workflow-foundation-v1` @ `b4de5d2`) **does not contain the Scenario One map or rack code**: none of `13988874`, `51d47dd` or `6bdcdd5` is an ancestor of it. The findings below come from a read-only `git show`/`git grep` of candidate `integration/scenario-one-hud-colour-pass-v1` @ `13988874cc36413d911243f4d3a6524436237bab`. Nothing was checked out, built or run in Unity.

They are reconnaissance, not the RACK-V2-01 inventory. RACK-V2-01 must repeat and correct them on the actual implementation base, and an implementation packet must name that base.

### Verified on candidate `13988874` (static code reading)

- **F-1 — A confirmed rack is not persisted.** `RackGroup` (`ScenarioOne/ScenarioOneManager.Racks.cs`) holds Id, AngleDegrees, AcrossM, WidthM, Start/End, LayoutIndex and OrderIds. It is session-only: the file comment says "Nothing here is saved", and `ClearRackGroups()` runs on new game and on load. After a load, rack trees are only ordinary individual felling orders. No rack field exists in the save data to reuse. **Consequence:** the persistence in spec §34 is new data, not reuse.
- **F-2 — Confirming a rack means creating felling orders.** `ConfirmRack(RackProposal, string source = "forest")` adds one pending `FellTree` order per tree to the draft Work Plan and clears any loose Fell mark on that tree. `RemoveRackGroup` removes pending orders and cancels approved ones. This is the existing rack-removal system that spec §36 says not to duplicate.
- **F-3 — Two creation surfaces already feed one function.** The first-person `Racks/RackTool.cs` (a MonoBehaviour with `Enter`/`SelectOrigin`/`Confirm`; the curriculum text says to press B) and the map's `UI/StandMapView.cs` (`PlaceRack(eco, index)`, a cell click while `rackMode`) both end in `ConfirmRack` (`RackTool.cs:224` with source `"forest"`, `StandMapView.cs:465` with source `"map"`). This is the "more than one interaction route" in spec §2.
- **F-4 — Several unsynchronised counters.** The candidate has at least `RackTool.ConfirmedCount`, `RackLayoutTracker.Done/Total` (derived from the authored layout lines and planned trees) and `ScenarioOneManager.RackGroups.Count` (which `LearningObjectivesView` uses to record `rack.create`). This matches the disagreement in spec §2.
- **F-5 — The layout is authored and derived, not stored.** `ScenarioOneRackLayout` has `RowsBetween = 7` (a rack every eighth planting row), applies to Enlarged80 only, and is recomputed from planted-tree positions. Its own comment says the spacing "is an authored Scenario One choice: it is not a forestry rule and carries no research claim". Live measurement (2026-10-22, candidate `f2ef2c5`, evidence `/media/jer/Files/CCF/Verification/ScenarioOneCompetitorSelectionRequalV2-20261022T201139Z/HANDOFF.md`) gives five racks at z = −31.409, −16.015, 0.015, 16.022 and 31.342, so the gaps are about 15.3–16.0 m, at or just under the bottom of the 16–20 m band. A code comment in `StandMapView.PlaceRack` says planting rows are "about 2 m apart", which agrees with eight row-steps over about 16 m. That is a comment and an inference, not a measurement, so re-measure it in RACK-V2-02.
- **F-6 — Tree marks are exclusive and two-valued.** Per-tree marks are None/Fell/CropTree (Unity Project Overview, V13) and the map mirrors that as `MapTreeMark { None, Fell, Crop }`. No orange or rack-entrance mark exists.
- **F-7 — Save and geometry.** `ForestSaveData.CurrentVersion = 20`, with `standGeometryModel`; saves at v19 or earlier, or without the field, load as Legacy40 (D-056). `ScenarioReferenceArchive` never strips or normalises the geometry field, and D-042 forbids testing Reference Future integrity by reserialising historical saves through current save classes.
- **F-8 — Downstream consumers of the authored layout.** `ScenarioOneRackBay`/`ScenarioOneRackBayText` (for example "the middle rack" when `RackCount == 5 && RackIndex == 2`), `ScenarioOneWorkPlanFocus`, `Map/StandMapSource` and `Racks/PlannedFellMarkers` read the authored layout or `RackGroups`. If racks become player-editable, these must be re-pointed at the authoritative rack objects or deliberately kept on the authored layout.
- **F-9 — Existing Objective 1 curriculum.** `LearningObjectivesView.Curriculum.cs` has `rack.read → rack.create → rack.layout → rack.approve → rack.result` (first-person) and a map step `rack.map`. Spec §30 replaces the front of this flow. Per the accepted OPEN-2 decision (D-064), `rack.approve` and `rack.result` are kept, in that order, after field marking.
- **F-10 — Existing gates encode the superseded workflow.** The recorded gates include `RackLessonVerification` (already failing on a stale 7.6 m literal against the live 8.004 m half-width, per the handoff above) and the rack 65 / UI 192 / teaching 20 / guided 74 check suites named in `Docs/Project/coordination/active-tasks.md`. They need an explicit disposition (update or retire, with reasons, under G3). They must not be silently loosened.

### Not yet inspected (RACK-V2-01 must cover)

The Help system (`UI/MenuHelpView.Text.cs`); whether a spray-marker tool or interaction exists, or `ForestTreeMarkingManager`'s aiming pattern is reusable; path/headland geometry; the minimap draw path (`HudMinimapView`) and map hit-testing (`StandMapCanvas`); existing 3D waypoint or cue infrastructure; how Objective 1 progress and the teaching sequence gate on rack state; whether tree-mark persistence has other consumers.

### Geometry feasibility (arithmetic only; RACK-V2-02 measures)

- **Enlarged80** (D-056: 80 × 80 m property). Five racks at spacing *s* span 4*s*: 64 m at 16 m, 72 m at 18 m, 80 m at 20 m. A full 20 m for all four gaps would place the outer racks on the property edge, so the top of the band is not attainable for every gap. An 18 m layout centred on the property leaves about 4 m at each side. The usable margin depends on the real tree extent, the property boundary and the forest-track approach, which are exactly what RACK-V2-02 must measure.
- **Legacy40** (40 × 40 m). Five racks at ≥16 m cannot fit; at most three racks at ≥16 m do. Per the accepted OPEN-1 decision (D-064), V2 does not apply to Legacy40, so this is not a stop condition; it is the reason for the boundary.
- **Source cross-check.** Book p. 40 ties 16–20 m to "1 in 8 or 9 rows or greater". With rows near 2.0 m that is about 16–18 m, and the authored layout is the 1-in-8 end. This is a plausibility check on the measurement, not a value to display: spec §15 forbids inventing a row count, so any row figure shown must be measured from the scene.

## Proposed technical approach

### Approved plan outline (as supplied)

#### 1. Preflight architecture inspection

Before edits inspect:

- current rack data model;
- current map rack-planning code;
- current first-person rack-placement code;
- minimap representation;
- Objective 1 state/counters;
- rack save/persistence;
- rack execution/removal behaviour;
- current tree mark architecture;
- Help system;
- stand/path geometry.

Produce a short implementation inventory before choosing code ownership.

#### 2. Geometry qualification

Measure:

- usable stand width perpendicular to intended rack direction;
- path/headland geometry;
- current suggested rack positions;
- existing plantation-row spacing.

Confirm whether:

`5 racks + approximately 16–20 m spacing`

is geometrically coherent.

If not:

STOP.

Do not proceed with arbitrary geometry.

#### 3. Authoritative rack-plan model

Refactor only as much as required to establish:

`one authoritative rack-plan object set`

Each rack requires stable identity and state.

Remove duplicated counter ownership.

#### 4. Map editor

Implement:

- Plan Racks mode;
- suggested rack layer;
- create;
- select;
- drag/move;
- rotate;
- delete;
- undo;
- reset;
- spacing measurement;
- validation;
- confirm.

#### 5. Minimap

Implement shared state representation.

No unexplained rack lines.

#### 6. 3D transition

After confirmation:

- expose entrance targets;
- supply subtle field guidance;
- do not expose rack freehand-creation interaction.

#### 7. Spray marking

Implement/reuse spray-marker interaction.

Apply orange vertical removal/machine-path mark.

Persist rack-entry mark state.

#### 8. Help and objective copy

Add contextual instruction.

Do not expose unnecessary future mark types.

#### 9. Persistence

Reconcile old saves and new V2 state.

Explicitly test:

- new game;
- old save before rack planning;
- old save with existing rack progress if such a state exists;
- save/reload after rack confirmation;
- save/reload after partial entrance marking;
- save/reload after all marks.

#### 10. Rendered review

Required at:

`1280 × 720`

`1920 × 1080`

Review:

- map before planning;
- Plan Racks active;
- selected rack and handles;
- spacing display;
- confirmed map;
- minimap;
- first entrance target;
- orange painted tree;
- Objective/Help state.

#### 11. Proportionate regression

Run focused rack/objective/save/UI qualification first.

Run broader protected Scenario One gates only where changed code intersects their behaviour.

Do not rerun hundreds of unrelated tests merely because the feature is visual.

However, because this feature crosses:

- Objective progression;
- persistence;
- rack world state;

it requires more than a purely visual smoke.

### Authoring recommendations (not part of the supplied outline)

These are proposals for RACK-V2-01 and RACK-V2-03 to confirm or reject against the real code. They are the smallest changes consistent with the spec.

- **R-1 — One persisted rack-plan object set replaces `RackGroup` as the counter source.** Each rack carries a stable ID, entrance anchor, direction, length, plan state (planned or confirmed), entrance-mark state and marked-tree ID (spec §11, §34). `RackGroup` is either retired or reduced to a view over these objects. All counters (`RackTool.ConfirmedCount`, `RackLayoutTracker` totals, `RackGroups.Count`) are then derived from it (spec §19, §35).
- **R-2 — Keep `ConfirmRack` → pending `FellTree` orders as the only rack-removal system.** Accepted as OPEN-2 (D-064): Confirm creates or refreshes pending orders only and never approves or executes; editing a confirmed plan before approval reconciles the pending orders with the revised geometry rather than duplicating them. The orange mark is a field-communication state of the rack object, not a second removal system (spec §36).
- **R-3 — Store the orange entrance mark on the rack object, with the marked tree's ID, rather than adding a third value to the exclusive per-tree mark enum.** The enum is read by the map, legend, save, Crop Tree pruning and competition-assessment code (F-6), so extending it has a wide blast radius. Spec §26 conflicts are then enforced at planning and marking time by checking the tree's existing Fell/Crop state. RACK-V2-01 must confirm this is workable; if the per-tree enum turns out to be required, that is stop condition 5 in spec §39.6 and returns to the Manager.
- **R-4 — Reuse the stand map, not a new editor.** Rack editing, handles, hit-testing and the dashed/solid symbology extend `StandMapView`, `Map/*` (`StandMapCanvas`, `StandMapDrawPlan`, `StandMapSymbols`, `StandMapLegend`) and `HudMinimapView`, so the main map and minimap share one symbol vocabulary (spec §20, §32, §33). `Plan Racks` is a toolbar control separate from the navigation tabs (spec §9).
- **R-5 — Remove first-person authoring by making `RackTool` unreachable for Objective 1**, including its B-key entry, controls-reference text and walking-HUD rack panel. Keep any world interaction that spec §4 still needs. `ConfirmRack`'s `source` argument stops distinguishing two authors (spec §8).
- **R-6 — Persistence is additive.** Add the smallest set of fields to carry R-1 and bump the save version following the D-046 to D-056 pattern: a missing field or an older version means "no V2 rack data". Never write to Reference Future v1. Validate defensively in `ForestSaveValidation` (spec §34, §39.5).
- **R-7 — Re-point or deliberately retain the F-8 consumers.** Decide per consumer in RACK-V2-01, and record the decision.

## Integration boundaries

- **May touch:** rack model and manager partials, map and minimap UI, objective/teaching sequence and Help text, tree-marking interaction and its visual, and the save fields and validation needed for R-1. The files are candidates only, listed under *Affected systems / files*.
- **Must not touch (spec §37):** stand geometry and the path entering from outside the property; growth, competition and species models; the economy; Crop Tree mechanics; the Work Plan architecture; subsequent objectives; Reference Future v1 and its archive (D-042).
- **Single-writer / high-conflict areas (AGENTS.md):** scenes, prefabs, ScriptableObjects, ProjectSettings, UI Toolkit assets (UXML/USS), package files and `.meta` files. Avoid broad reserialisation and opening unrelated scenes or prefabs. Cloud or code-only workers must not hand-edit Unity YAML. If a prefab, material or scene change is needed for the spray visual or waypoint, route it through the single writer.
- **Do not edit:** `Library`, `Temp`, `Logs`, `obj`, `.vscode/settings.json`, or frozen Scenario One Reference Future v1.
- **Contracts used:** qualified UI Visual System V2 (`specs/scenario-one-ui-visual-system-v2`); the existing Work Plan order system; the existing save/load and validation path.
- **No Windows build** (spec §37).

## Affected systems / files

Candidates from a grep of candidate `13988874`, **not** a verified inventory (RACK-V2-01 produces that):

- Rack model and tool: `Racks/RackGeometry.cs`, `Racks/RackPlanner.cs`, `Racks/RackTool.cs`, `Racks/PlannedFellMarkers.cs`, `ScenarioOne/ScenarioOneManager.Racks.cs`, `ScenarioOne/ScenarioOneRackLayout.cs`, `ScenarioOne/ScenarioOneRackBay*.cs`, `UI/RackLayoutTracker.cs`
- Map and minimap: `UI/StandMapView.cs`, `UI/Map/StandMap*.cs`, `UI/HudMinimapView.cs`
- Objective, teaching and Help: `UI/LearningObjectivesView*.cs`, `UI/ScenarioOneTeachingSequence.cs`, `UI/ScenarioOneRackBayText.cs`, `UI/ScenarioOneWorkPlanFocus.cs`, `UI/MenuHelpView.Text.cs`, `UI/ControlsReference.cs`, `UI/TeachingDiagram*.cs`
- First-person and marking: `ForestPlayer.cs`, `ForestTreeMarkingManager.cs`, `UI/WalkingHudView.cs`, `UI/InspectedTreeIndicator.cs`, `UI/TreeInspectionView.cs`
- Persistence: `ForestSaveData.cs`, `ForestSaveController.cs`, `ForestSaveValidation*.cs`
- Verification: the existing rack/guided/teaching gates under `Tools/Verification/` (F-10), plus new disposable harnesses following the documented copy/run/remove workflow.

## Ownership / concurrency considerations

- One primary writer per subsystem and one active owner per writable lane (AgentWorkflow *Multi-agent safety*). Read `Docs/Project/coordination/active-tasks.md` live at task start. At authoring no task row claims this work, and the Unity lanes (U1/U2/U3), their owners and the Unity ownership lock are the Overall Manager's to assign.
- Heavy Unity execution is serialised machine-wide under one ownership lock. A Unity packet must state the exact candidate HEAD, lane/owner, allowed phases and the Player/native-input requirement.
- **Overlapping live work:** the same UI files are touched by `SCENARIO-ONE-UI-VISUAL-SYSTEM-V2`, the three-page onboarding rows and the Scenario One UI/non-UI integration rows. The implementation base must be the current Scenario One candidate that contains qualified UI V2, and RACK-V2-14 reconciles. The packet fixes the base and the context commit, which is not this checkout.
- The disposable harness workflow can touch `forest-save.json`. Use backup/restore and keep any generated `.meta` out of `Assets/ForestPrototype/` afterwards.
- Durable evidence goes under `/media/jer/Files/CCF/Verification/…` (Storage policy), not on ZX20.

## Migration / save implications

- The candidate is at save `CurrentVersion = 20` (F-7) and racks are not persisted today (F-1). The change is additive: new rack-plan fields, a version bump, and "missing field or older version = no V2 rack data".
- Old saves must load without a crash and preserve existing state where safely interpretable. An old save that already has rack felling orders keeps them as ordinary orders. V2 data is not invented for it, and its Objective 1 state must be defensible (spec §34).
- Legacy40 saves and Reference Future v1 are unchanged: V2 applies only where `standGeometryModel == Enlarged80` (OPEN-1, D-064). The archive is never reserialised through current save classes (D-042).
- Counters are derived, never stored (spec §35).
- Not a migration: no change to growth, competition or economy data, so existing determinism anchors must not move.

## Verification strategy

- **Implementation correctness:** focused rack, objective, save and UI qualification first (spec plan §11). That covers the rack-plan model and counters; map editing (create/select/move/rotate/delete/undo/reset) including soft snapping and validation; confirmation and reopen behaviour (spec §17, §18); entrance-tree targeting and negative cases (unrelated trees must not satisfy the objective); mark-conflict rules (spec §26); and the persistence matrix in the *Approved plan outline* section 9, plus Legacy40 load and a malformed-V2-data load. Prove "no simulation or growth regression" with a narrow check that the existing lifecycle anchors are unchanged. Run the broader protected Scenario One gates only where changed code intersects them. Existing rack gates that encode the superseded workflow (F-10) are updated or retired explicitly under G3.
- **Spec conformance:** `checklists/requirements.md`, with evidence for AC-1 to AC-20, and the rendered review in the *Approved plan outline* section 10 at 1280 × 720 and 1920 × 1080, including a representative live 3D marking state. Independent review is a separately assigned reviewer. A helper beneath the implementing worker does not count.
- **Verification tier:** Tier 1 during iteration; Tier 2 FEATURE QUALIFICATION at hand-off (the feature crosses objective progression, persistence and rack world state, so more than a visual smoke is required). Tier-2 Unity needs G1 launch approval. The two-resolution rendered review is spec-mandated and scoped to the listed views, not a general Tier 3 matrix. Escalate to Tier 3 only at the integrated candidate (RACK-V2-14) and only under the AgentWorkflow escalation rules.
- **Gates:** G2 acceptance; G3 for any harness, fixture, threshold or expected-value change; G4 at integration; G5 for the save-format change (AgentWorkflow *Multi-agent safety* item 7 sends save architecture to the Manager/user).

## Risks

- **RK-1 — Hidden coupling to the authored layout (F-8).** Player-edited racks can break the rack-bay lesson, Work Plan focus or map source that assume five authored lines and a "middle rack".
- **RK-2 — First persistence of rack data (F-1).** A new schema field means defaults for old saves, validation hardening and Reference Future immutability must all be right; a mistake silently invalidates saves (spec §34).
- **RK-3 — The geometry may not fit the full band for five racks (Geometry feasibility).** The stop condition may fire for real, in which case the spec must be amended, not worked around.
- **RK-4 — Mark-model blast radius (R-3).** Putting the orange mark in the per-tree enum touches the map, legend, save and pruning. Putting it on the rack object requires careful conflict checks.
- **RK-5 — Existing gates silently loosened (F-10).** Old rack gates assert the superseded workflow; "fixing" them to pass is a G3 matter.
- **RK-6 — Plan, mark and execute blur (spec §36).** Confirming creates real Work Plan orders (F-2). Reconciling pending orders on an edit before approval is decided (OPEN-2); reopening a plan after orders are approved or executed is still undefined (OPEN-3).
- **RK-7 — Visual-lane collision.** Several live UI rows touch the same files; merge conflicts in UI Toolkit assets are likely if the base is wrong.
- **RK-8 — Spray visual versus URP and performance.** A four-sided stripe mark across several marked trees must stay URP-compatible and cheap; verify in the rendered review.

## Implementation sequence

Each step maps to one task in `tasks.md`. Order is derived from the dependencies listed there.

1. Inspect the real architecture on the named base — **RACK-V2-01**
2. Qualify geometry, and either confirm five-rack calibration or stop — **RACK-V2-02**
3. Establish the single authoritative rack-plan model — **RACK-V2-03**
4. Map planning mode, separate from the navigation tabs — **RACK-V2-04**
5. Rack editing: create, select, move, rotate, delete, undo, reset — **RACK-V2-05**
6. Spacing teaching and plan validation — **RACK-V2-06**
7. Confirmation, reopen behaviour, counters and minimap — **RACK-V2-07**
8. Remove the competing first-person author — **RACK-V2-08**
9. Field marking: targeting, waypoint, spray, orange mark — **RACK-V2-09**
10. Help and objective copy — **RACK-V2-10**
11. Persistence verification — **RACK-V2-11**
12. Functional qualification of the whole Objective 1 flow — **RACK-V2-12**
13. Rendered review at both resolutions — **RACK-V2-13**
14. Integration and dashboard reconciliation — **RACK-V2-14**
