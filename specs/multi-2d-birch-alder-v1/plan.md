# Downy Birch and Common Alder V1 Implementation Plan

- **Spec** [SPEC-multi-2d-birch-alder-v1](spec.md)
- **Status** APPROVED / IMPLEMENTING
- **Context commit** `bdeb6a324acaf4129c10fb0846faac8b64974360` (`origin/main`)
- **Base** `24871551ae906b6911b48bb17c252c023bdea502` (`origin/integration/2d-sim`, live tip at cut time)
- **Branch** `feat/multi-2d-birch-alder-engine-v1`
- **Review branch** `review/multi-2d-birch-alder-engine-v1` (created by the coordinator after review, not by this worker)

## Part 1 approach

1. **Reuse the foundations in place.** Namespace `SitkaP1.Core.SpeciesWave2.BirchAlder` reads `SpeciesMoistureTable.Cell/Response/HabitatProtectionCaution` and `FutureSpeciesCatalog.DownyBirch` / `.CommonAlder` directly. No new moisture vocabulary, no copied card, no copied table row. Result records expose the foundation's own `MoistureCell` and `SiteResponse` values so that a later shared `SiteMoistureEngine` (Scots Pine Part 1) can replace the thin accessor without changing the public shape of the results.
2. **Wet Valley & Slope helper.** `WetValleyAndSlopeMap` owns `StandBounds`-equivalent validation locally (its own `StandBounds` type would collide with the parked Scots Pine lane, so it takes four `double`s in a `WetValleyBounds` record) and exposes `At(x, y)`. Split on the y midpoint: below is `WetMovingAlluvial`, the midpoint and above is `FreshMoistMineral`. `CreateStandard()` returns the 80 × 80 m, five-lane 0.64 ha layout. `LaneCount` and `LaneWidthM` are exposed so Part 2 can align mapped trees to lanes without re-deriving the geometry.
3. **Growth.** One shared crowding model — normalised stocking and basal-area pressure, take the larger — with per-species constants: juvenile increment, adult increment, maximum crowding reduction, and a provisional teaching domain. Birch: 0.85 → 0.25 cm/year, up to 70 % crowding reduction. Alder: 0.95 → 0.20 cm/year, up to 65 %. Each applies the site growth multiplier exactly once. `null` outside the declared domain, which is explicitly a calibration window rather than a card readiness domain (both cards carry no numeric domain).
4. **Local regeneration.** One shared gate order (seed tree, seed year, light, then species-specific competition/moisture gates) with per-species salts so birch and alder good-seed-year schedules differ. Four-year blocks with a good year at offset 0 or 1 give 3–5 year gaps and allow direct out-of-order queries. Alder adds the moisture class gate and the `ExposedMineralGround` input.
5. **Heart rot.** `HeartRotRiskProbability(age)` is a clamp of a linear 0 → 0.35 ramp across 50–60 years. `InspectHeartRot(stemSeed, age)` hashes the stem identity and age under a dedicated salt and returns whether the stem downgrades. Nothing mutates state and no RNG stream is consumed.
6. **Timber.** One `GradeTimber(BirchAlderTimberRequest)` over a shared shape: bulk price, card density, sapling cost, quality-stem flag and heart-rot downgrade. Prices are decimal; the density conversion is decimal. Quality stems keep the bulk price and are marked unpriced rather than given a placeholder premium.
7. **Evidence.** A standalone console project compiles only `src/Core/SpeciesWave2/**/*.cs` plus its own tests, prints per-suite and total counts and a canonical SHA-256, and exits non-zero on failure. Restore, build and run use the lab's pinned SDK 8.0.133.

## Files and ownership

The sole writer owns:

- `specs/multi-2d-birch-alder-v1/`
- `Tools/MultiRackSitkaP1Lab/src/Core/SpeciesWave2/BirchAlder/`
- `Tools/MultiRackSitkaP1Lab/tests/Core/BirchAlder/`
- `Docs/Handoffs/MultiRack-Simulation-20261116/birch-alder/`

Nothing else may change. Separate commits carry the spec quartet, the module, the tests and the handoff so the integrator can inspect each fence boundary. `NeutralLegacy` is untouched for existing woodlands because no existing file is edited.

## Integration boundary and migration

Part 1 creates no save fields and no runtime calls. **Part 2 is QUEUED** behind Playable Milestone 1 and Scots Pine Part 2, and is itself sequenced: Downy birch first, then Common alder. At that point:

- `AnnualPipeline` consumes the growth and regeneration results in its existing stages;
- `MultiAreaContinuation` supplies area state and the moisture class per cell;
- `ForestMaker` applies `WetValleyAndSlopeMap` for the 0.64 ha wet-valley preset;
- Version 3 saves carry the approved species and moisture state, with old and Version 2 saves defaulting to `NeutralLegacy`;
- `/multi` surfaces the zones, the habitat caution, the bulk price and the quality-stem note.

Part 2 must also decide how a real map supplies seed-tree ages, opening size, undergrowth and exposed mineral ground, because Part 1 takes them from the caller.

## Verification and risks

Run from `Tools/MultiRackSitkaP1Lab` with SDK 8.0.133:

```text
dotnet restore tests/Core/BirchAlder/SitkaP1.BirchAlder.Tests.csproj
dotnet build   tests/Core/BirchAlder/SitkaP1.BirchAlder.Tests.csproj --no-restore
dotnet run --project tests/Core/BirchAlder/SitkaP1.BirchAlder.Tests.csproj --no-build --no-restore
```

Run the last command twice in separate processes and compare full output and the printed digest. Check the diff against the base for the four authorised prefixes only.

Known risks:

- the teaching domains and growth shapes are calibration, not prediction;
- the alder heart-rot maximum probability is a teaching value and could read as a disease model if Part 2 words it carelessly;
- a quality stem earns bulk money, which will feel disappointing in play; Part 2 must keep the buyer note visible rather than hide it;
- the y-midpoint split is a layout convention, not a survey;
- Part 1 cannot see the parked Scots Pine types, so the foundation table is read directly. If the two lanes are combined, the coordinator should decide whether the thin accessor is hoisted rather than merged.
