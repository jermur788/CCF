# Scenario One — implementation

Scenario One is turning the canonical 40 x 40 m, 336-stem Sitka plantation into
the first annual management scenario. It uses independent scenario
progression and the management-first loop:

`walk -> mark trees / planting sites -> review and buy stock -> approve -> advance one year -> observe`

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
4. effect: the existing authoritative Forestry APIs (`ForestTree.Fell`,
   `ForestTree.TryPrune`, species-selective regeneration removal), plus the
   Scenario One v13 individual planted-juvenile lifecycle. Legacy cell planting
   remains on Forestry's cohort pathway for v12 saves and Reference Future v1.

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
5. advance the individually positioned planted juveniles against the same
   species juvenile growth/survival response and their local cell light;
6. store the annual management report.

Outside the Work Plan, the upper-right management HUD stays visible while
walking: ecological year, current cash and the nursery species with saplings
ready to plant. "Ready" is inventory minus stock reserved for approved orders
and pending exact-position planting markers;
the reserved quantity is shown separately. The HUD hides in the full Work Plan
and while exploring a reference-future preview.

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
  Work Plan (one open order per species per cell) in the legacy v12 pathway;
- reserve stock and contractor cash when approving, then consume one sapling
  and pay the contractor only if `TryPlantJuvenile` succeeds on the legacy path;
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

The player can look at real ground with live regeneration, cycle the visible
species with `[R]` and press `[U]` to mark its cell for contractor removal.
Orders record the selected species/cell and estimated
density, time and cost. Resolution uses Forestry's authoritative
`TryUprootRegeneration` and charges only for a successful removal. The other
species in the cell is untouched by the treatment. Failed or cancelled orders
leave cash unspent. Annual reports and structured events store removed cohort
density and the `RegenerationRemoved` treatment; no harvest revenue or stock is
created. The Work Plan reviews and approves this spatial choice;
other modes retain the existing player-facing hold-to-uproot interaction.
Removal minutes are provisional gameplay calibration [D] on the definition.

## Fallen deadwood retention

Felling orders can choose `SellAndExtract`, `KeepForUse`, or
`RetainAsFallenDeadwood`. The Work Plan shows the outcome per order and lets the
player switch before approval. Extraction pays timber revenue and records
harvested volume. Retention pays no revenue, calls the same `ForestTree.Fell()`,
and creates a management-layer `ScenarioDeadwoodRecord` holding the tree/species
identity, position, cell, original stem volume and size. A simple fallen-stem
marker is spawned at the felling position and rebuilt from saved records on
load for presentation only; Forestry's
authoritative stump visual is untouched.
`KeepForUse` settles no timber revenue and adds the harvested biological volume
to the saved Scenario One timber stockpile. Each build can pay its wood cost
from carried wood, nearby stored wood and the remaining retained stockpile,
using 0.1 m³ per missing wood unit [D]; it never charges the same wood twice.

Deadwood decays deterministically once per ecological year: roughly 3% volume
loss per year with a floor at 12% of the original stem [D]. Each record carries a
diagnostic 0–5 decay class derived from volume loss. Habitat value [D] weights
remaining volume by decay class, peaking at intermediate decay. Annual reports
record deadwood created and volume decayed; ecological snapshots record log
count, remaining volume, mean decay class and habitat value. Deadwood never
feeds back into Forestry tree growth, competition or regeneration. Records
persist in save v11 alongside orders, events and understorey state.

## Evidence-backed pruning

Felling, planting and regeneration-removal orders are joined by `PruneTree`
orders. The Work Plan lists living trees that accept another clear-stem lift
and lets the player designate one. Target crown-base heights follow common Sitka
clear-stem practice [D]: 2.5 m, 5 m and 6.5 m for successive lifts. Forestry's
`ForestTree.TryPrune` enforces a three-lift cap, a minimum five-year recovery
interval, and a crown-base target below 60% of tree height. Each lift raises the
recorded crown base and reduces crown radius by a deterministic 8% [D] so the
biological light-interception change is represented without a second crown model.
Pruning state (lifts, crown base, last lift year) persists on the tree in save
v11; legacy saves load as unpruned. The management event stream records
`TreePruned` treatments with the tree identity and contractor cost.
Lift heights, recovery interval and the crown-radius proxy are provisional [D]
and require silvicultural calibration before being described as measured Irish
pruning responses. Pruned trees retain a reduced crown target during subsequent
Forestry crown relaxation; the unpruned Sitka annual path is unchanged.

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
annual execution. `[G]` enters planting mode in Scenario One when there is
unreserved owned stock. `[R]` cycles the owned species and left click on ground
sets each exact-position marker; `[Esc]` exits. Pending markers reserve one
sapling, can be cancelled in the Work Plan and remain visible at their chosen
positions through approval/save/load until contractor resolution. Completion
creates a separately persistent individual with ID, stock/species identity,
position, age, height and survival; promotion creates a tree at the same
position. The internal 5×5 m grid still supplies light/site/natural regeneration.
On planting, a ~0.56 m radius circular patch clears the proportional Sitka
regeneration density in each intersected cell. Same-year overlapping patches
are unioned rather than double-counted. The patch is not a permanent exclusion:
normal seed rain can recolonise in subsequent years. The existing
Forestry planting API remains available to other modes and verification tools.

## Fell marks and the Work Plan

`[M]` is the in-world decision; the Work Plan reviews, costs and approves it.
Opening the Work Plan (and approving work) automatically imports any red Fell
marks as felling orders, so a mark can never silently miss the annual plan. The
explicit "Add marked trees" button remains for clarity. Importing felling
orders consumes only red marks — blue Crop Tree designations persist.

## Forestry interaction and compatibility (save v13)

`[M]` marks a living tree red for felling; `[C]` designates it as a persistent
blue Crop Tree, replacing any Fell mark. The two marks cannot coexist. Blue
trunk bands persist through annual advances and save/load, and are removed only
when the player explicitly unmarks or the tree is felled. Importing red Fell
marks into the Work Plan consumes only red marks. Crop Tree pruning is an
eligible batch in the Work Plan; the existing biological lift and recovery
constraints remain authoritative, and scheduled-for-felling trees are excluded.
Legacy v12 cell-designated planting/pruning work orders remain loadable.

The frozen `scenario-one-v12` Reference Future v1 archive remains immutable:
336 starting Sitka, schedule `56C8B99FA1E8DDD1`, Year-100 v12 world hash
`7AD177B3CC2F73C7`. v13 loads its v12 milestone saves as a historical,
walkable example and verifies their *original v12 JSON* before preview. A v13
save necessarily has a different full-world hash and must not be presented as
reproducing the frozen v12 bytes. The independent canonical Sitka lifecycle
hash remains `7E39B70A14959FAD`.

The disposable `Tools/Verification/ScenarioOnePlantingVerification.cs` runner
checks both scene definitions, the fresh stand, purchases, both species' planted
origin, yearly settlement, failed planting, save/load continuation and v9 migration.

Headless Scenario One interaction verification exercises marking, batch
pruning, owned-stock markers, exact planted individual promotion, overlapping
clearance, retained-timber construction and the frozen v12 preview. The
separate reference-continuation and full older regression gates remain required
before integrating the v13 interaction overhaul.

## Habitat presentation and soundscape routing

`ScenarioHabitatPresentation.md` specifies the derived visual classes,
source/continuity gate, seven habitat-linked sound layers with silence until
recordings are assigned, and the Year-0/20/50/100 walkable-preview verification. All are
presentation-only; existing Forestry and saved understorey proxies remain
authoritative. The Century Review interprets the structural signals without
adding wildlife objectives or claiming unverified ancient-woodland flora.

## Tutorial, outcomes and Century Review (save v12)

The Work Plan shows a five-step onboarding path: mark a tree, purchase stock,
plan/approve work, advance a year and inspect the annual review. Existing
structured work/events provide the milestones; review-opened state survives
save/load. Felling input in Scenario One points to marking and the Work Plan;
the direct Forestry chopping entry point remains available for non-scenario
modes and regression tooling.

Starting in the provisional minimum year, completion requires a contractor
opening, live original-species canopy, ongoing regeneration, continuous canopy,
retained deadwood and persistent planted presence of every species offered in
the scenario nursery. All threshold values live on `ScenarioOneDefinition` [D]
and are evaluated from the current ecological snapshot plus authoritative
work-order history. An irreversible cash lock with a positive contractor rate
fails early; reaching the Century Review year without satisfying the objectives
also fails. Completion is retained as an achievement, and a completed scenario
may continue to Year 100. Year 100 records an immutable Century Review and
stops further annual management. The review compares actual structural metrics
against **aspirational design targets**, not a simulated no-management future;
the reference values need calibration before being presented as an ecological
forecast. Versions 1–11 retain their biological and management state and
initialize the new outcome/review fields safely. No completed or failed past
scenarios are inferred from pre-v12 summaries.

`ScenarioOneReferenceFutureV1.md` documents the separately authored and
verified 100-year Reference Future. Once its compatible frozen archive is
present, the Work Plan offers walkable Year-20/50/100 previews that restore the
player's previous full forest state on exit. At the actual Century Review, the
player's structural and management comparisons use that reference's measured
Year-100 save/history, not the provisional design targets. The reference is
one possible path, not a score or a forestry prescription.

## Protected baseline

- fresh `ForestTest`: exactly 336 original Sitka;
- canonical lifecycle hash: `7E39B70A14959FAD`;
- Beech and Oak remain on the generic planting path;
- Scenario One planting stock is initially Beech and Sessile Oak;
- no monthly/seasonal time, networking, machinery or primitive crafting;
- `.vscode/settings.json` remains worktree-local and unmodified.

## Remaining player-facing checks

1. Confirm the blue trunk band, planting pegs, eight ground-layer types,
   changing log appearance and any newly assigned recordings in an actual
   walkable Game view at Years 0, 20, 50 and 100.
2. The full Forestry integration suite, four-year presentation gate and
   interaction/canonical/reference-replay gate passed in `/home/jer/CCF`, where
   **Ultimate Nature – Starter** is installed locally. The isolated worktree
   lacks that gitignored pack; its pack-dependent visual assertion is not a
   gameplay regression.
3. Calibrate provisional Irish price/pruning values only with a separately
   verified source, reference year and unit.
