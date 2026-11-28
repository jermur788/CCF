# Full Custom Woodland Builder V1 Implementation Plan

- **Spec** [SPEC-multi-2d-custom-woodland-builder-v1](spec.md)
- **Status** APPROVED / IMPLEMENTING (Part 1)
- **Context commit** `bdeb6a324acaf4129c10fb0846faac8b64974360` (`origin/main`)
- **Base** `f4dee8023d3492a25e61451c7e2a49515e507ed3` (`origin/integration/2d-sim`, live tip at cut time)
- **Branch** `feat/multi-2d-custom-woodland-builder-engine-v1`
- **Review branch** `review/multi-2d-custom-woodland-builder-engine-v1` (created by the coordinator after review, not by this worker)

## Part 1 approach

1. **Geometry first, as its own layer.** `PlanPoint` and `WoodlandPolygon` own flat-local-metre geometry, validation and the canonical counter-clockwise normalisation. `WoodlandShapeSpec` builds the four templates as base rings, then a chamfer pass applies the four corner adjustments and re-validates. The polygon is the single authority for containment, edge distance, centroid and area, so the lane clipper, the moisture grid and the generator all agree instead of each deriving its own bounds.
2. **Lanes by exact clipping.** `LaneClipper` splits the polygon's Z extent into equal bands and intersects the polygon with each band using Sutherland–Hodgman half-plane clipping. Clipping preserves containment by construction; the clipper then measures the result and refuses the whole plan when a lane falls below the declared mean width or area. Refusal is the default because a silently dropped lane would misreport the work the player can actually do.
3. **Grid aligned to the simulation.** `MoistureGrid` uses the global 2.5 m lattice origin `floor(min / 2.5) * 2.5`, so a custom woodland and an imported woodland agree on cell boundaries. Activity is a cell-centre containment test against the polygon. Patterns are pure functions of each active cell's centre, so there is no iteration-order dependency and no random source. `NeutralLegacy` is represented in the array as the inactive value and is refused as a pattern or a paint.
4. **Paint overrides as data.** Overrides are validated against the active-cell set and stored in declaration order. The grid resolves the base pattern first and then replays the overrides, which keeps "later wins" obvious and makes the Part 2 round-trip a direct serialisation of the same list.
5. **Deer presets as a closed vocabulary.** A four-value enum maps to the exact `float` values already accepted by `Phase1Stage5Options`. The mapping record carries `EvidenceTag.Of(EvidenceClass.Calibration)` and the not-a-density note, so the calibration label travels with the value instead of living only in prose.
6. **Mix allocation and placement.** Integer counts come from largest remainder over the six table species ids. Placement is a jittered lattice seeded from the recipe seed through a SplitMix64 stream, accepted by polygon containment, ordered deterministically, and then species-assigned by a Fisher–Yates shuffle over a list built from those counts. Edge seed trees use a separate inward-offset ring with an explicit clearance test, and the cap check happens before any tree is built.
7. **One entry point, one canonical text.** `CustomWoodlandBuilder.Build` validates, builds the polygon, lanes, grid, preset and stand, and returns an immutable result with a canonical text form. The standalone console gate hashes that text, so the digest covers geometry, moisture, deer and every tree.

## Files and ownership

The sole writer owns:

- `specs/multi-2d-custom-woodland-builder-v1/`
- `Tools/MultiRackSitkaP1Lab/src/Core/CustomWoodlandBuilder/`
- `Tools/MultiRackSitkaP1Lab/tests/Core/CustomWoodlandBuilder/`
- `Docs/Handoffs/MultiRack-Simulation-20261116/custom-woodland-builder/`

Nothing else may change. Separate commits carry the spec quartet, the module, the tests and the handoff so the integrator can inspect each fence boundary. No existing file is edited, so `NeutralLegacy` woodlands and every parked engine lane are untouched.

## Integration boundary and migration

Part 1 creates no save fields and no runtime calls. **Part 2 is QUEUED** behind the Milestones 2–5 Part 2 wiring wave, and at that point:

- the New Woodland picker collects a `CustomWoodlandRecipe` instead of a fixed preset;
- `ForestMaker` generates the Year 0 stand from `GeneratedStand`, using the lane index and moisture class already carried per tree;
- `AnnualPipeline` reads the `MoistureGrid` instead of re-deriving site moisture;
- Version 3 `genesisRecipe` persists the boundary spec, lane count, pattern, paint overrides, deer preset, mix, age, stocking, edge-tree count and seed;
- `/multi` shows the field outline, the painted cells, the habitat caution and the living-tree count against the cap;
- Version 2 and older saves keep their current behaviour with no fabricated recipe.

Part 2 must also decide how the picker presents a bog cell (caution text, not a block) and whether the player may exceed the 2,500 warning band.

## Verification and risks

Run from `Tools/MultiRackSitkaP1Lab` with SDK 8.0.133:

```text
dotnet restore tests/Core/CustomWoodlandBuilder/SitkaP1.CustomWoodlandBuilder.Tests.csproj
dotnet build   tests/Core/CustomWoodlandBuilder/SitkaP1.CustomWoodlandBuilder.Tests.csproj --no-restore
dotnet run --project tests/Core/CustomWoodlandBuilder/SitkaP1.CustomWoodlandBuilder.Tests.csproj --no-build --no-restore
```

Run the last command twice in separate processes and compare full output and the printed digest. Check the diff against the base for the four authorised prefixes only. `2d-gate.sh` and browser suites are deliberately not run on this isolated engine branch.

Known risks:

- **Shape templates are stylised.** A 30–120 m bounding footprint with a 450 m² minimum area will refuse genuinely small fields and will not reproduce a surveyed parcel. Part 2 must word the picker as "close enough", not "your field".
- **Mean lane width is an area average.** A dog-legged lane can pass the 8.0 m mean while having a locally narrow neck. The clipper refuses degenerate and narrow-average lanes, not every local pinch, and the spec says so.
- **Patterns are not hydrology.** The catena, corridor and hollow are teaching shapes with fixed proportions; a real wet patch is painted, not modelled.
- **Deer presets are aggregate.** `[E]` calibration on one pressure value. Part 2 must not describe them as deer numbers or as a damage model.
- **The cap is a simulation limit.** 3,000 living trees is a performance and clarity guardrail, not a carrying capacity, and a legal footprint at 3,000 stems/ha can exceed it. That refusal is deliberate and must stay visible in the picker.
- **Lattice placement is not a planting prescription.** Jittered rows do not represent a real planting design, machine reach or drainage.
- **Edge seed trees depend on shape.** A very concave polygon has a short usable inset ring; the generator refuses rather than crowding. Part 2 should surface that plainly.
