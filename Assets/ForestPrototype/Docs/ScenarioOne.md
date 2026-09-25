# Scenario One — implementation

Scenario One is turning the canonical 40 x 40 m, 336-stem Sitka plantation into
the first annual management scenario. It uses independent scenario
progression and the management-first loop:

`inspect -> decide -> mark -> plan -> approve -> advance one year -> observe`

The implementation started from shared baseline `05673d4`, save v9. Older
summaries that describe save v8 or monthly progression are stale. Scenario One
uses annual progression only.

## Architecture

Management work is separated into:

1. intent: a persistent `ScenarioOneWorkOrder` keyed by authoritative tree ID,
   species ID, cell or world position;
2. requirement: estimated minutes, contractor cost, required stock and expected
   output;
3. execution: Scenario One's annual contractor resolver;
4. effect: the existing authoritative Forestry APIs (`ForestTree.Fell`, generic
   planting and species-selective regeneration removal).

Later manual or multiplayer executors can consume the same work order without a
second ecology implementation.

`ScenarioOneDefinition` holds provisional gameplay/economic calibration [D],
including starting cash, contractor rate, felling productivity, timber value and
minimum completion year. Currency is stored as integer cents.

`ScenarioOneManager` owns scenario state, Work Plan UI, approval and deterministic
annual orchestration. `[Tab]` opens the plan. While it is open the walking
controller is paused and the cursor is available. The current annual order is:

1. validate approved work;
2. resolve work in ascending persistent work-order ID;
3. settle contractor cost and immediate harvest revenue;
4. run `ForestEcologyController.AdvanceOneYear()` exactly once;
5. store the annual management report.

The old real-time ecology time-lapse is disabled while the management scenario is
active. Verification harnesses can still invoke the same ecology method directly,
so the canonical Sitka lifecycle remains isolated.

## Felling and planting vertical slices

- create an empty Work Plan and deliberately advance one year;
- convert marked, living trees into persistent felling orders;
- show time, cost, biological volume and expected timber revenue;
- approve work only when current cash covers contractor cost;
- resolve each tree once through `ForestTree.Fell()`;
- calculate actual revenue from biological stem volume at execution;
- preserve pending/approved/completed/failed orders and annual reports in save v10;
- migrate v1-v9 saves by starting Scenario One economy state while retaining the
  saved forest and ecology;
- purchase configurable Beech and Sessile Oak saplings in whole-number quantities;
- designate persistent, species-specific planting work by ecology cell in the
  Work Plan (one open order per species per cell);
- reserve stock and contractor cash when approving, then consume one sapling
  and pay the contractor only if `TryPlantJuvenile` succeeds;
- let players cancel pending or approved orders before the annual resolution,
  immediately releasing approved reservations;
- record unsuccessful planting without consuming stock or charging for work;
- keep stock, planting orders and biological origin/year through save v10.

Shop prices and planting minutes are provisional gameplay calibration [D] on
the scenario definition. Purchases are settled immediately in integer cents;
approved contractor work reserves its cost, and labour is settled on successful
annual execution. Planting via the
old direct `G` action is routed to the Work Plan in Scenario One. The existing
Forestry planting API remains available to other modes and verification tools.

The disposable `Tools/Verification/ScenarioOnePlantingVerification.cs` runner
checks both scene definitions, the fresh stand, purchases, both species' planted
origin, yearly settlement, failed planting, save/load continuation and v9 migration.

The first felling slice supports `SellAndExtract`. Fallen-deadwood retention is a
defined outcome but is not selectable until the deadwood system is implemented.

## Protected baseline

- fresh `ForestTest`: exactly 336 original Sitka;
- canonical lifecycle hash: `7E39B70A14959FAD`;
- Beech and Oak remain on the generic planting path;
- Scenario One planting stock is initially Beech and Sessile Oak;
- no monthly/seasonal time, networking, machinery or primitive crafting;
- `.vscode/settings.json` remains worktree-local and unmodified.

## Next slices

1. species-selective regeneration-removal orders;
2. ecological outcome history and dashboard;
3. deterministic functional-group understorey;
4. fallen deadwood, then evidence-backed pruning;
5. habitat-driven soundscape routing;
6. tutorial, objectives and completion/failure state.
