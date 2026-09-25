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
- keep stock, planting orders and biological origin/year through save v10+.

## Structured management history (save v11)

Management events are persisted alongside orders and annual reports. Each has a
monotonic event ID and a typed kind (purchase, create, approve, cancel, resolve,
annual advance), year, work-order/task identity, species, tree ID or cell,
quantity and stock item, estimated/actual contractor cost, materials consumed,
timber revenue, biological volume, cash delta and closing balance. Executed
tasks store success/failure, a diagnostic reason for failure and a separate
ecological treatment type. Purchases and planning are recorded in the current
ecological year; annual work resolution is recorded for the year it advances
into. Forestry's planted-origin year remains the pre-advance ecological year.
Versions 1–9 still start the configured economy; version 10 saves retain their
existing orders/reports and begin an empty event history. Historic events are
not fabricated from earlier summary strings.

Each complete ecological year also stores a read-only stand snapshot. It
includes living stem count, basal area per hectare, mean DBH, canopy/light,
regeneration occupancy/opening and per-species trees and natural/planted cohort
counts. These measurements are observed after Forestry's annual step; no
ecology rule uses them. The first observation is the fresh or loaded stand
baseline, and version-10 saves start observing from the load onward without
inventing earlier snapshots. The Work Plan's expandable annual review compares
current and baseline structure alongside recent reports and typed event history.

## Species-selective regeneration control

The Work Plan can designate an entire live species cohort in an ecology cell
for contractor removal. Orders record the selected species/cell and estimated
density, time and cost. Resolution uses Forestry's authoritative
`TryUprootRegeneration` and charges only for a successful removal. The other
species in the cell is untouched by the treatment. Failed or cancelled orders
leave cash unspent. Annual reports and structured events store removed cohort
density and the `RegenerationRemoved` treatment; no harvest revenue or stock is
created. A `U` attempt in Scenario One directs the player to the Work Plan;
other modes retain the existing player-facing hold-to-uproot interaction.
Removal minutes are provisional gameplay calibration [D] on the definition.

## Deterministic understorey functional groups

Each ecology cell has saved moss, fern, grass, forb, shrub and litter-fungus
cover proxies (0–1). Initial cover is derived from Forestry canopy/light and
site state. The management annual step moves each group deterministically
towards its shade/gap target with configurable colonisation and loss rates [D].
Dark Sitka litter supports moss/fungi, while openings favour ferns and then
herbaceous/shrub groups; returning shade reduces gap groups. There are no
random rolls, detailed botanical species or feedback into tree growth. The
annual review shows mean functional-group cover; the save retains individual
cell state for continuation and replay. Existing scene-scattered Nature Detail
props remain visual decoration rather than biological authority.

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

1. fallen deadwood, then evidence-backed pruning;
2. habitat-driven soundscape routing;
3. tutorial, objectives and completion/failure state.
