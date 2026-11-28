# Scots Pine and Site Moisture V1 Implementation Plan

- **Spec** [SPEC-multi-2d-scots-pine-moisture-V1](spec.md)
- **Status** APPROVED / Part 1 implemented / Part 2 IMPLEMENTING (branch `feat/multi-2d-scots-pine-moisture-part2-v1`, base `f4dee802`)
- **Context commit** `bdeb6a324acaf4129c10fb0846faac8b64974360`
- **Base** `c82e4e7` (`c82e4e759c2ceef1c13ddb6a033e034097aef77e`, `origin/integration/2d-sim`)
- **Branch** `feat/multi-2d-scots-pine-engine-v1`

## Part 1 approach

1. Keep all species data in `SpeciesMoistureTable` and the Scots pine card in `FutureSpeciesCatalog`. Expose area-weighted site selection, rectangular moisture sampling and the table response through `SiteMoistureEngine`. Accumulate decimal square-metre weights for input-order independence at ordinary map scales. Carry the bog caution as a property of the selected class.
2. Expose the card, adult growth, local regeneration and timber grading through `ScotsPineEngine`. Compute an annual potential DBH increment that declines linearly from 0.60 to 0.20 cm/year across the age range. Scale it by a crowding factor from 1.0 to 0.5 based on the larger of the two normalised stocking and basal-area pressures. Apply the table's growth modifier once. Treat top height as a domain boundary. These choices are provisional calibration.
3. Compute one good seed year per four-year block, at offset 0 or 1 selected by a salted integer mix of woodland seed and block index. This bounds gaps at 3–5 years and permits direct queries in any order. Use elapsed years starting at zero. Gap data carries the age of the oldest on-map Scots pine seed tree supplied by the caller. Local dispersal geometry and actual recruitment are outside this computation.
4. Price stems using species-specific teaching defaults, the existing card's bands and green density, and decimal arithmetic. The caller supplies maturity, observed pruning height, early stocking history and Small Roundwood suitability. This avoids a new maturity, spacing, taper or defects model. Values are gross and unrounded.
5. Add a standalone console project linking only the existing and new Wave 2 C# sources, alongside its own tests. Check branch state, run the pinned restore/build/run sequence, repeat the execution for the determinism digest, and publish the tracked handoff.

## Files and ownership

The sole writer owns `specs/multi-2d-scots-pine-moisture-v1/`, `Tools/MultiRackSitkaP1Lab/src/Core/SpeciesWave2/ScotsPine/`, `Tools/MultiRackSitkaP1Lab/tests/Core/ScotsPine/` and `Docs/Handoffs/MultiRack-Simulation-20261116/scots-pine-moisture/`. Compare the full diff to the base before pushing the feature branch. Use separate commits for spec, module, tests and handoff.

## Integration boundary and migration

Part 1 creates no save fields. **Part 2 is IMPLEMENTING** (task MULTI-2D-SCOTS-PINE-MOISTURE-P2-20261128; it was queued behind Playable Milestone 1). At that point AnnualPipeline will consume the distinct growth and regeneration results, MultiAreaContinuation will supply area state, ForestMaker will apply the map and generation choices, Version 3 saves will carry approved persistent state, and `/multi` will surface the choices and warning. Actual regeneration will require a separately reviewed way to derive local seed proximity, openings and competition from map state.

## Verification and risks

Use SDK 8.0.133 from the lab root and the standalone console gate. Verify bitwise neutral identity, table correspondence, map boundaries, weighted ties, bog warning, age and canopy thresholds, seed-year spacing, growth limits, timber pricing and repeatability. The table gives no fresh moist ordering within the tied leaders. The growth, regeneration and money outputs are teaching inputs whose player impact requires separate calibration and integrated review.
