# Full Custom Woodland Builder V1

- **Spec ID** `SPEC-multi-2d-custom-woodland-builder-v1`
- **Status** APPROVED / IMPLEMENTING (Part 1)
- **Owner** Overall Manager
- **Decision** Owner-approved 2026-11-28, Option A on all four choices
- **Task class** SPEC REQUIRED
- **Created** 2026-11-28
- **Last updated** 2026-11-28
- **Milestone** Playable Milestone 6, Part 1 (isolated engine)
- **Related evidence** `specs/multi-2d-species-wave2-profiles-v1/`, `Tools/MultiRackSitkaP1Lab/src/Core/SpeciesWave2/SpeciesMoistureTable.cs`, `Tools/MultiRackSitkaP1Lab/src/Core/Saves/GenesisRecipeBlockCodec.cs` (Part 2 target only)
- **Supersedes / superseded by** none

## Product intent

Let the player create a woodland that is theirs rather than one of three fixed presets: choose a real Irish field shape, say where the wet and dry ground is, say how hard deer will browse it, and choose the species mix. Everything downstream — lanes, ground cells, trees and the habitat caution — follows from those choices.

Part 1 delivers a callable, offline-tested engine: a validated boundary polygon with four Irish field-shape templates, a lane clipper, a 2.5 m moisture grid with four natural patterns and per-cell paint, four plain-English deer browsing presets, and a deterministic six-species stand generator with a living-tree safety cap. Part 2 is **QUEUED** behind the Milestones 2–5 Part 2 wiring wave and connects the recipe to the New Woodland picker, `ForestMaker`, `AnnualPipeline`, Version 3 `genesisRecipe` and the `/multi` UI.

## Problem

A custom woodland is the one thing the 2D simulator cannot currently do. The soil-moisture vocabulary, the six species cards and the Phase 2A generator all exist, but there is no recipe that says "a 70 × 90 m L-shaped field with a wet stream edge, standard deer, 60 % spruce and 40 % oak, 1,800 stems per hectare". A player can only pick a fixed preset. Without a validated shape and a validated mix, the first attempt at a custom woodland would either place trees outside the field, silently drop awkward lanes, or generate a stand that is too large for the simulation to run.

## Goals

- Four flat-local-metre boundary templates that look like Irish fields, with validated simple corner adjustments.
- Work lanes that are genuinely clipped to the boundary and that refuse impossible slivers instead of silently shrinking.
- One 2.5 m ground grid aligned to the existing simulation grid, with four deterministic site-moisture patterns and per-cell paint overrides.
- Bog is always reported as a habitat-protection caution, never as a planting recommendation.
- Four plain-English deer browsing presets that map to the authoritative `backgroundBrowsePressure` values.
- A six-species mix engine with exact percentage allocation, mature edge seed trees and a fail-closed living-tree cap.
- A pure Part 1 API with no I/O, no clock and no ambient randomness.

## Non-goals

- No wiring into `ForestMaker`, `AnnualPipeline`, `MultiAreaContinuation`, `src/Ui`, Version 3 saves or `genesisRecipe`. Part 2 owns all of that.
- No save fields, no migrations, no schema version change.
- No individual deer entities, deer home ranges, fencing or browsing damage simulation. The presets set one aggregate pressure value only.
- No terrain height, slope or aspect model. The boundary and the grid are flat local metres.
- No species suitability filter at generation time: the player may plant any accepted species anywhere, and bog is flagged rather than blocked. Part 2 decides how the picker communicates that choice.
- No real-world map import, no drone or LiDAR input, no parcel detection.
- No change to `SpeciesWave2/*.cs`, `RngSalts.cs`, the parked engine lanes or any Unity asset.

## Player workflows

Not applicable in Part 1. Part 2 will expose the workflow: choose a field shape and adjust its corners, drag the boundary, choose a moisture pattern and paint individual 2.5 m cells, pick a deer preset in plain English, set the mix, age and stocking, place edge seed trees, read the living-tree count against the cap, and create the woodland. Part 1 claims no player-facing behaviour.

## Functional requirements

- **FR-1 — Boundary templates and corner adjustments.** Four templates in flat local metres, each producing a counter-clockwise, non-self-intersecting simple polygon with a canonical start vertex:
  - `Rectangle(widthM, depthM)`;
  - `LShape(widthM, depthM, armWidthM, armDepthM)` — a rectangular field with one rectangular corner bay removed;
  - `TaperedWedge(southWidthM, northWidthM, depthM)` — a field that narrows from one headland to the other;
  - `AngledStreamEdge(widthM, depthM, streamSlantM)` — a field whose far boundary runs at an angle to a straight near boundary.
  Each template accepts a `CornerAdjustments` chamfer of 0–4 corners in metres (0 = sharp corner). A chamfer at a vertex must be non-negative, finite and strictly less than half of the shorter adjacent edge, so two adjacent chamfers can never meet. The bounding footprint must lie in **30–120 m** on both axes and the polygon area must be at least **450 m²**. Refuse: fewer than 3 or more than 16 vertices, non-finite coordinates, a repeated closing vertex, zero-length edges, spikes, self-intersection or touching between non-adjacent edges, and an out-of-range footprint or area. Clockwise input is normalised to counter-clockwise; the canonical start is the vertex with the smallest X, then the smallest Z.
- **FR-2 — Clipped parallel work lanes.** Clip **2–8** parallel lanes against the boundary. Lanes are horizontal bands: the polygon's Z extent is divided into `laneCount` equal bands and lane *k* is the exact polygon ∩ band. Index 0 is the southernmost band. Clipping uses Sutherland–Hodgman half-plane clipping, so every lane vertex lies inside or on the boundary polygon and no lane can leave the field.
  - A lane is usable only when it has at least 3 distinct vertices, a mean width (`area / band height`) of at least **8.0 m** and an area of at least **60 m²**.
  - If any lane fails, the whole plan is refused with a named reason. Lanes are never silently dropped, merged or widened, because that would change the work layout behind the player's back.
  - A refused plan reports which lane failed and why.
  - The engine creates nothing outside the boundary. The open grassland around the field is a Part 2 terrain and rendering concern; this module neither populates nor claims it.
- **FR-3 — 2.5 m moisture grid and four site patterns.** Build a ground grid of **2.5 m** square cells aligned to the global 2.5 m lattice (`origin = floor(min / 2.5) * 2.5`), matching the existing `Stage5Options.GridCellM` value. A cell is **active** only when its centre lies inside or on the boundary polygon; active cells are the only cells that carry a moisture class.
  - Four deterministic patterns over the active cells, using the existing `SiteMoistureClass` vocabulary unchanged:
    - `Uniform` — every active cell takes one supplied class;
    - `RidgeAndSlope` — a catena from the northern ridge to the southern toe: the upper 25 % of the polygon's Z extent is `DryFreeDraining`, the middle 50 % is `FreshMoistMineral`, the lower 25 % is `SeasonallyWetAerated`;
    - `StreamCorridor` — a straight watercourse along X at the polygon's mid-Z: cells within `corridorWidth / 2` of the centreline are `WetMovingAlluvial`, cells within a further **6 m** bank margin are `SeasonallyWetAerated`, the remainder is `FreshMoistMineral`;
    - `WetHollowBogPocket` — a circular hollow at the polygon's area centroid: cells within the radius are `StagnantPoorBog`, cells within a further **5 m** are `SeasonallyWetAerated`, the remainder is `FreshMoistMineral`.
  - `NeutralLegacy` is never generated and never painted. It remains the untouched default for every existing and legacy woodland.
  - Every active cell carries the table's own caution decision: `SpeciesMoistureTable.HabitatProtectionCaution`. A grid containing any active `StagnantPoorBog` cell reports `HasHabitatProtectionCaution` and the number of affected cells. Bog is a habitat caution, not a planting recommendation, and no ordering or sampling may imply otherwise.
- **FR-4 — Per-cell paint overrides.** Accept a deterministic list of `(cellX, cellZ, class)` paint overrides applied on top of the chosen base pattern before Year 0. Overrides must address an **active** cell and a real moisture class (`DryFreeDraining`, `FreshMoistMineral`, `SeasonallyWetAerated`, `WetMovingAlluvial`, `StagnantPoorBog`); an inactive cell, an out-of-range index or `NeutralLegacy` is refused. Later entries win over earlier entries for the same cell. The overrides are kept on the grid so a later Part 2 save can persist exactly what the player painted, and the caution flag is recomputed **after** painting.
- **FR-5 — Four plain-English deer browsing presets.** Expose exactly four presets that map to the authoritative aggregate `backgroundBrowsePressure`:

  | Preset | Value | Label |
  | --- | --- | --- |
  | `None` | `0.0` | No deer browsing pressure |
  | `Low` | `0.1` | Light deer browsing |
  | `Standard` | `0.2` | Standard deer browsing (existing baseline) |
  | `Heavy` | `0.4` | Heavy deer browsing |

  Each preset carries `[E]` provenance: `EvidenceTag.Of(EvidenceClass.Calibration)`, plus a plain note that these are gameplay calibration values and **not** Irish deer-per-square-kilometre density thresholds. No individual deer entity is created, moved or simulated. Any value outside the four enumerated presets is refused.
- **FR-6 — Six-species mix.** Accept a mix over exactly the six accepted species (`SitkaSpruce`, `SessileOak`, `EuropeanBeech`, `ScotsPine`, `DownyBirch`, `CommonAlder`, read from `SpeciesMoistureTable.SpeciesIds` — never re-typed). The mix lists each species exactly once with a finite percentage weight in `0–100`; the weights must sum to **100** within `1e-9`. Zero-weight species are allowed so a player can name all six and use four. Integer stem counts are allocated by largest remainder, with ties broken by the table's species order, so the counts always sum to the target and no species silently disappears.
- **FR-7 — Stand generation, edge seed trees and the safety cap.**
  - Start age is **10–60** years inclusive; stocking is **500–3,000** stems per hectare inclusive.
  - The planted target is `round(stocking × area in hectares)` and is placed on a deterministic jittered lattice inside the polygon, restricted to active 2.5 m cells so that every tree carries a defined moisture class instead of an invented legacy default. Every tree is inside the boundary, and every tree is assigned to the lane that contains it.
  - Optional mature outer-edge seed trees, `0–200` of them, are placed on the polygon's inward offset ring **4 m** from the boundary, strictly inside (at least **0.5 m** of clearance from every edge), spread evenly along the perimeter, flag `IsEdgeSeedTree` and start at `max(startAge, 60)` years. If the perimeter cannot seat the requested number at that inset they are refused rather than crowded together.
  - **Living-tree safety cap: 3,000.** Planted stems plus edge seed trees must not exceed it; exceeding it is refused with the target, the cap and the reason. A stand at or above **2,500** living trees reports `NearSafetyCap` so Part 2 can warn the player. Planting never silently trims the mix or the stocking to fit.
  - Each generated tree carries its lane index, its grid moisture class and that cell's habitat-protection caution, so Part 2 does not have to re-derive them.
  - Species assignment is a deterministic weighted shuffle: the same seed, mix and geometry always produce the same species at the same position. No `System.Random`, no clock, no ambient state.
- **FR-8 — Pure Part 1 boundary.** Immutable requests and results through a small .NET 8 API in `SitkaP1.Core.CustomWoodlandBuilder`. No file, network, clock or ambient randomness. No existing runtime entry point and no save payload is touched.

## Authority and source of truth

- **Accepted product intent.** The owner instruction of 2026-11-28 (Option A on all four locked choices), the Current Milestone, the Game Brief and the 2D strategic roadmap v1.22, in that order.
- **Implementation facts.** The inspected repository at base `f4dee8023d3492a25e61451c7e2a49515e507ed3` (`origin/integration/2d-sim`): `SpeciesMoistureTable` already carries the six species ids, the five real moisture classes, the NeutralLegacy identity row and `HabitatProtectionCaution`; `Phase1Stage5Options` already fixes `GridCellM` at 2.5 and validates `BackgroundBrowsePressure` in `0–1`; `SaveV3Format` already reserves the `genesisRecipe` block for Part 2.
- **Supporting evidence.** The Wave 2 profiles specification and the existing preset generators for the 0.64 ha stand convention.
- **Spec authority.** Approved and task-level implementation authority for Part 1 only, subordinate to the project authority hierarchy above.

## Evidence classification

| Input | Class | Note |
| --- | --- | --- |
| Six species ids and five real moisture classes | Empirical / authored table | Read from `SpeciesMoistureTable`; nothing re-typed. |
| `NeutralLegacy` = (1.0, 1.0) | Calibration | Existing table invariant; unchanged and untouched. |
| 2.5 m grid cell | Empirical | Matches the existing `Stage5Options.GridCellM` simulation grid. |
| 30–120 m footprint, 450 m² minimum area | Calibration | Sizing guardrails for a playable field, not a survey standard. |
| 8.0 m mean lane width, 60 m² lane area | Calibration | Teaching threshold for a usable work lane; not an Irish machine-access specification. |
| Four shape templates | Calibration | Stylised representations of common Irish field shapes, not surveyed parcels. |
| Moisture pattern geometry (25/50/25 catena, 6 m banks, 5 m hollow ring) | Calibration | Teaching patterns, not a hydrological model. |
| Deer presets 0.0 / 0.1 / 0.2 / 0.4 | Calibration `[E]` | Plain-English gameplay settings for the existing `backgroundBrowsePressure`; **not** Irish deer/km² thresholds. |
| Start age 10–60, stocking 500–3,000 stems/ha | Calibration | Playable ranges; not a silvicultural prescription. |
| 4 m edge inset, 60-year edge seed tree age, 0.5 m minimum clearance | Calibration | Stand-layout convention; not a seed-tree research finding. |
| 3,000 living-tree cap, 2,500 warning | Calibration | Simulation safety limit for the 2D playwright, not an ecological carrying capacity. |
| Largest-remainder allocation | Deterministic rule | Arithmetic, not an empirical claim. |

No value in this specification is presented as an Irish empirical yield, deer-density, hydrological or parcel-geometry relationship.

## Save / backward compatibility

No save impact in Part 1: no field is added, no schema changes and no save is read. Existing woodlands keep `NeutralLegacy` by default. Scenario One, `Legacy40`, `Enlarged80` and frozen Reference Future v1 are untouched because no existing file changes.

Part 2 must specify how a custom recipe is persisted in Version 3 `genesisRecipe`, how a Version 2 or older save that has no recipe keeps its current behaviour without fabricating a boundary or a moisture grid, and how the painted overrides round-trip exactly.

## Security / write-back

Not applicable. Part 1 is a pure in-process computation: no input parsing, no file access, no network, no write-back. Validation is limited to argument guards that refuse non-finite geometry, out-of-range counts, inactive paint targets and mixes that do not sum to 100.

## Acceptance criteria

- **AC-1** All four templates produce a counter-clockwise simple polygon with the expected area and vertex count; an adjusted corner is chamfered and stays inside the footprint; footprint, area, chamfer, duplicate-vertex, spike and self-intersection violations are refused; clockwise input is normalised.
- **AC-2** Lanes are clipped exactly: every lane vertex and every lane's area lies inside the boundary polygon, lane indices are ordered from the south, the lane areas sum to the polygon area, and 2–8 is enforced. A configuration that produces a mean lane width below 8.0 m or an area below 60 m² is refused with a named reason instead of dropping the lane.
- **AC-3** The grid is 2.5 m and lattice-aligned; only cells whose centre is inside the polygon are active; all four patterns are deterministic and produce their declared classes; no active cell ever carries `NeutralLegacy`.
- **AC-4** Per-cell overrides apply on top of the base pattern, later entries win, an inactive or out-of-range cell and a `NeutralLegacy` paint are refused, and the habitat caution is recomputed after painting. `SpeciesMoistureTable.HabitatProtectionCaution` still returns true only for `StagnantPoorBog`, and the `NeutralLegacy` response stays bit-exact (1.0, 1.0) for all six species.
- **AC-5** The four deer presets map to exactly `0.0 / 0.1 / 0.2 / 0.4`, each carries `Calibration` (`[E]`) provenance and the not-a-density-threshold note, and no other preset value is accepted.
- **AC-6** A six-species mix summing to 100 allocates exact integer counts; a mix that does not sum to 100, repeats or omits a species, or carries a negative or non-finite weight is refused. Start age and stocking ranges are enforced at both ends.
- **AC-7** Generation places exactly `round(stocking × hectares)` planted stems, every tree strictly inside the polygon and inside its assigned lane, edge seed trees flagged, mature and at least 0.5 m from every edge, and a target above the 3,000 living-tree cap is refused. The same recipe twice in separate processes yields byte-identical output and the same digest; `NearSafetyCap` is reported from 2,500.
- **AC-8** Pinned .NET 8 restore, build and run succeed with zero warnings and zero errors, zero failed checks and identical canonical SHA-256 digests across separate processes. Every tracked change lies inside the four authorised prefixes.
- **AC-9 (Part 2, queued)** On an integrated candidate: the New Woodland picker creates the recipe, `ForestMaker` generates from it, `AnnualPipeline` consumes the moisture grid, Version 3 `genesisRecipe` round-trips the boundary, pattern, overrides, presets and mix, and `/multi` shows the habitat caution and the living-tree count.

## Stop conditions

Stop and return to the Manager if:

- product authority conflicts, or a locked behaviour in §Functional requirements is materially ambiguous;
- the work needs an edit outside the four authorised paths;
- Part 1 cannot be completed without wiring Part 2, a new dependency, a package change or a save change;
- a protected anchor (Scenario One, Reference Future v1, existing Wave 2 files, RNG salts, the parked engine lanes) would be threatened;
- the pinned offline gate fails in a way that cannot be repaired inside the authorised paths.

No further Part 1 product decision is required.
