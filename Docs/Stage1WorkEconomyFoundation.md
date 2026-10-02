CURRENT IMPLEMENTATION INVENTORY
================================

Task: Stage 1 Forestry Work & Economy Foundation v1. Worker: OpenAI/OpenCode,
primary implementation. Base and locked context:
`cd239d7889c8224dcd46591deefa3089249e94c9`.

## Inspected facts before design

| Existing class/system | What it owns at BASE | Authority and later connection |
|---|---|---|
| `ScenarioOneWorkOrder`, `ScenarioWorkType`, `ScenarioWorkStatus` | Individual FellTree/PlantJuvenile/RemoveRegeneration/PruneTree orders; target IDs/position, stock requirement, estimates, pending/approved/completed/failed, years | Keep order identity, approval and world-target data authoritative. Adapt one or several orders into an economic job; do not create a second pending-order store. |
| `FellingMaterialOutcome` in that file | SellAndExtract, RetainAsFallenDeadwood, KeepForUse | Preserve meanings. The pure domain uses those same named dispositions; map explicitly rather than casting unrelated enums. A single future stem can have multiple batches/dispositions. |
| `ScenarioOneManager` | Work Plan UI, marked-tree collection, validation/reservation, stock purchase, cash in integer euro cents, resolution, annual reports/events, exact planting, retained m³, deadwood | Remains the current live owner. `CreateFellingOrder` estimates hourly work and one species value/m³; `ResolveFelling` charges that estimate and revalues the entire stem, then calls `Fell`. No assortment or minimum-job model exists. New calculator can replace only economics later. |
| `ScenarioOneManager.AdvanceYear` | Sorted approved orders; checks available cash; resolves in one ecology change batch; settles immediately; advances ecology once; updates juveniles/understorey/deadwood/reports | Keep sequence and world effects. Aggregate eligible harvest orders into one explicitly commissioned job before applying a job minimum; never apply a €2,500 minimum independently to each tree. |
| `ScenarioOneDefinition`, `ScenarioShopEntry` | Provisional [D] €45/h, base/per-m³ felling minutes, Sitka €72/m³/broadleaf €65/m³, €4.50 Beech/€5.50 Oak stock, 10 min planting; pruning/regeneration removal rates | Historical/current definition remains unchanged. New independent price book is not silently substituted. Shop prices are retail/gameplay prices, not the report's wholesale nursery prices. |
| `ForestTreeMarkingManager` | Exclusive Fell/CropTree designations, marked/crop IDs, volume summary and conditional ecology forecast | Keep spatial planning and marking authoritative. Adapter reads stable IDs, not a duplicate mark store. |
| `ForestTree` | Biological volume, `Fell`, `TryPrune`, pruning/crown state and events | Remains world-effect authority. New domain neither calls nor reimplements these methods. |
| `ScenarioOneManager.ResolvePlanting` | Validates species/location/stock; consumes purchased stock; legacy cohort or exact individual/clearance path | Feed existing stock as already owned: no second nursery purchase charge. Preserve exact positions and authoritative planting outcomes. |
| `ScenarioOneManager.ResolvePruning` | Calls `TryPrune` before charging, records lift work, no immediate revenue | Use returned eligibility/accounting later; preserve pruning validation and no guaranteed timber premium. |
| `ScenarioOneManager.retainedTimberM3`, `TrySpendRetainedTimber` | Separate construction timber stockpile; adds whole KeepForUse stem volume; spending is bounded | Remains stockpile owner. New batches report original volume and disposition for later deposit; economic tonnes do not become construction wood units. |
| `ScenarioAnnualReport` / management events / scenario state DTOs | Annual costs, timber revenue, volumes, cash deltas, stock use, task provenance; saved by existing save system | Remain live history/save authority. Ledger is a calculated result, not a new persisted global wallet/history. No save-schema edit in this task. |
| `ForestPlayer`, `ForestWoodStorage`, `ForestSawPit`, `ForestBuildable` | Survival wood/plank units; player wood conversion is bounded 3–24; buildables can consume retained timber at 0.1 m³/unit | These are deliberately different gameplay units. No lossless cash/timber conversion is inferred from them. Construction/survival adapters are deferred. |

Search of production Assets found no separate reusable contractor/eligibility,
assortment price-book or ledger subsystem. Existing Work Plan is compatible with
an unwired calculation service; it is not replaced or duplicated.

## Primary evidence and provenance

Read the complete 29-page **Irish Forestry Economics, Labour and Contractor
Operations for CCF Stage 1.pdf**, local source `/home/jer/Downloads/`.
SHA-256: `fc51dc48e675951f345005aec4bf1c296954de61cc4f82dbf7b9566c34808741`.
Do not commit the source PDF or imply that all its recommended parameters have
become accepted live Scenario One values.

- pp. 3–7: roadside versus standing/delivered accounting; multiple log assortments;
  IFA April–June 2024 Sitka, nominal euros, ex VAT.
- pp. 7–10, 16–18: combined harvest/forward rates, independent haulage,
  owner eligibility, time productivity and nursery evidence.
- pp. 19–22: species/moisture-specific volume conversion, configuration and
  worked job-scale example.
- pp. 2, 20, 23: €2,500 minimum is [C], sensitivity €1,500–4,000, **not** an Irish tariff.

Labels: [E] empirical, [G] management guidance, [I] inference,
[S] simulation abstraction, [C] gameplay calibration. Selected scenario values
have their own label alongside the evidence/reference range. Grants are a future
policy layer; storm/salvage uplifts are outside v1.

## Implemented production domain

Namespace: `CCF.Forestry.WorkEconomy`. Production files are confined to
`Assets/ForestPrototype/WorkEconomy/`, with normal Unity-generated `.meta` files.
All calculations are pure C#: no Unity dependency, scene reference, service
locator, static state, wallet, inventory manager or scheduling framework.

| File | Responsibility |
|---|---|
| `ForestryWorkDomain.cs` | Task, actor resources, requirements, timber batches, operation description, quote, resolution and signed ledger DTOs |
| `ForestryPriceBook.cs` | Replaceable typed parameter/range/provenance data; per-task eligibility/productivity; species/product prices and density; combined or split harvest schedules |
| `Stage1EconomyDefaults.cs` | Fresh independent baseline configuration instances; all selected economic/labour values live here rather than in calculation logic |
| `ForestryEconomyValidation.cs` | Input/configuration checks, units, provenance, identifiers, duplicate prevention and explicit stock/output specifications |
| `ForestryWorkCalculator.cs` | Requirements, costing, eligibility, valuation, quotation and deterministic accounting resolution |

Supported tasks: inspection, measurement, marking, harvest/thinning/felling,
planting, shelter installation, fencing, low-risk pruning and monitoring.
First/second/later thinning and clearfell are **cost contexts**, not additional
gameplay effects. Regeneration removal is an existing world operation with no
new economics profile here; keep its current resolution until a bounded adapter
profile is explicitly supplied.

`ForestryTask` references `WorldOperationId` and stable targets but contains no
execution method. `Quote(task, method, actor, book)` and
`Resolve(task, method, actor, book)` accept the method separately. Both contractor
and landowner-simulated execution return the same `WorldOperationDescription`.
Future manual/cooperative execution can add policies around that same task
without changing the authoritative forestry operation. V1 implements no manual
play, multiplayer, XP, calendar simulation or ecology.

The result is an **accounting resolution**, not proof that a world effect was
executed. It describes the operation to perform and accounting/material effects
to commit if that operation succeeds. The integrator still validates the live
target, executes the existing effect once, and applies the result once. Repeated
calculator calls have no side effects. Existing order completion status remains
the authoritative duplicate-execution guard.

### Requirements and eligibility

- Actor capability and tool flags describe the selected worker; contractor
  equipment is not implicitly taken from the player's inventory.
- Profiles contain allowed methods, required capability/tools, productivity and
  optional lead days. The default harvest profile prohibits ordinary owner
  production regardless of chainsaw ownership. Windblow processing is not an
  available v1 task. An explicit future qualified-owner policy needs both a
  changed eligibility rule and a positive owner labour estimate.
- Owner labour is integer person-minutes, rounded **up** for fractional work;
  it consumes the reported time budget, not a cash wage. Optional opportunity
  cost is shown separately from cash. Tool running costs can be configured.
- Harvest person-time is not inferred from €/t: `PersonMinutesKnown=false`
  explicitly distinguishes an unavailable machine time estimate from zero work.
- Stock is supplied as a snapshot of **unreserved** available material. Existing
  stock is reported as consumed without buying it a second time. `PurchaseForJob`
  adds one external material charge. One requirement per material ID prevents
  duplicate lines from spending the same stock twice.
- Planting needs one configured nursery-stock item per individual; shelter
  installation needs one shelter item. Fencing defaults to requiring an explicit
  fence-material specification. Supply a real/configured material quote, not a
  DAFM installed grant rate disguised as material cost. Minor repair policies can
  use a different configured profile; no invoice or grant is invented.
- Upfront cash must cover external costs. Timber receipts do not prefinance
  work. Contractor availability, owner time, materials, tools, capability and
  policy rejection are explicit `UnmetRequirement` entries.
- Valid unavailable work yields `Resolved=false`, zero realized cash/time,
  empty realized ledger and no material award/consumption. Malformed data throws
  `ArgumentException`; unrepresentable arithmetic throws `OverflowException`.

### Units, money and rounding

Money is signed `long` euro cents. Quantities are nonnegative fixed-unit integers:
whole items, square metres, millimetres; timber accepts green grams or cubic
centimetres. The report's €/tonne corresponds to 1,000,000 green grams. Volume
conversion is explicit through a matching species/moisture density ID:

`green grams = round(cubic centimetres × selected kg/m³ / 1000)`.

For example 1 m³ = 1,000,000 cm³, at 890 kg/m³ → 890,000 g = 0.89 t.
The original input quantity/unit remains in the material output. The Sitka
conversion is not automatically used for Beech/Oak/dry timber; missing/mismatched
species prices or density fail validation. These encodings do not change the
existing m³ stockpile or survival wood/plank units.

Decimal intermediates avoid binary floating-point money errors. Each monetary
component rounds to cents, midpoint away from zero; labour rounds up to minutes.
Checked sums/conversions reject overflow. There is no NaN/infinity input in this
API. A negative **net** is valid; negative quantities/rates/resources are not.

## Selected baseline configuration

All monetary values below are nominal ex VAT. The 2024 deck is replaceable, not
a timeless or current-2026 market claim. Each `EconomicParameter` has low,
reference, high, selected, unit, range-evidence label, selected-value label,
source/year. Selected stress-test values may lie outside the reference range;
the caller must retain honest metadata and use a new price-book ID when publishing
a changed scenario deck.

| Parameter | Low/reference/high | Label/provenance |
|---|---|---|
| Pulp roadside | €36 / €38 / €40 per t | Range [E], selected representative [C], IFA 2024 |
| Stake roadside | €42 / €47 / €52 per t | Range [E], selected representative [C], IFA 2024 |
| Pallet roadside | €48 / €68 / €75 per t | Length-dependent [E] range, generic selection [C] |
| Sawlog roadside | €88 / €95 / €105 per t | Length-dependent [E] range, generic selection [C] |
| First harvest + forward | €20 / €21 / €22 per t | [E], IFA 2024 |
| Second harvest + forward | €22 / €23 / €24 per t | [E], IFA 2024 |
| Later/third harvest + forward | €20/t | [E] approximate, IFA 2024; replaceable |
| Clearfell harvest + forward | €14 / €15 / €16 per t | [E], cost category only |
| Haulage | €12/t | [E] approximate; independent, opt-in owner-paid transport |
| Harvest-system minimum | €1,500 / €2,500 / €4,000 per job | [C], not an empirical contractor tariff |
| Fresh Sitka density | 750 / 890 / 950 kg/m³ | Selected fresh-log transfer [I] from Irish [E]; sensitivity range [C] |
| Contractor non-harvest labour | €45/h | [C] existing prototype placeholder; no national manual-work tariff claimed |
| Owner time opportunity value / tool running cost | €0/h by default | [S] unspecified/disabled; configurable independently |
| Nursery Sitka/standard Beech/Sessile Oak | €0.45 / €0.95 / €1.00 per plant | [E] None-So-Hardy 2025–26 **wholesale**, not current shop prices |
| Inspect/measure/monitor | 3 h/ha; 2–4 range | [C] |
| Marking | 10 h/ha; 6–15 range | [I] European study transfer |
| Planting | 100 plants/h selected | [C]; 60–180 sensitivity encoded as approximate reciprocal minutes |
| Low-risk pruning | 15 trees/h selected | [C] within report's [I/C] 12–20 guidance |
| Shelters | 45 individuals/h selected | [C] within 30–60 sensitivity |
| Fencing | 80 m per eight-hour person-day | [C], 40–150 sensitivity, no Irish empirical productivity claim |

No automatic CCF multiplier, guaranteed pruning premium, inflation, forester fee,
grant, VAT, discounting, salvage uplift or timber-bucking algorithm is added.
Site cost basis points default to 10000 (1.0) and are explicitly caller-selected
cost data. Price entries can later distinguish log length/buyer specification by
ID without changing the batch model. The generic baseline is only Sitka;
broadleaf timber requires its own configured deck.

## Timber and roadside accounting contract

A stem/job can produce arbitrarily many batches: assortment, stable batch/source
tree IDs, species, fixed quantity/unit, density ID if needed, price ID,
disposition, extraction and owner-paid transport. V1 consumes quantified batches;
it does **not** infer a whole-tree product from DBH, distribute arbitrary product
percentages, or invent a taper/quality model. A caller must not pass the full stem
volume into every assortment; batches describe disjoint recovered quantities.

- `SellAndExtract`: must be extracted, sale value becomes positive cash.
- `KeepForUse`: sale revenue is zero; quantity and gross roadside reference value
  remain separately available. Extraction/transport is explicitly selected.
- `RetainAsFallenDeadwood`: not forwarded, not sold; reference value is noncash.
  Existing deadwood world-state/decay remains authoritative. The reference value
  is neither an actual payment nor a calculated net opportunity cost.

Harvest mass includes all produced material; forward/haul mass includes only
applicable extracted/owner-transported batches. Operational costs are calculated
on summed mass once, not separately charged per assortment; one economic task is
one commissioned job, so the minimum is applied once across its trees/batches.

The source provides **combined** harvesting + forwarding evidence. Defaults retain
this combined category and do not pretend harvesting and forwarding are each
€21/t. A separate-rate configuration instead reports those components separately
and ignores the combined rate. If some produced timber is not forwarded, a
combined-only schedule cannot honestly price it: quote becomes incomplete and
ineligible until supplied with split rates or a final explicit professional quote.
Incomplete quotes report no final net/settlement ledger; known components are
not an actionable total.

Per-tonne model:

```text
variable combined cost = rounded(total green tonnes × combined rate × site factor)
OR
variable split cost = rounded(harvest tonnes × harvest rate × site factor)
                    + rounded(forward tonnes × forward rate × site factor)
minimum adjustment = max(0, selected job minimum − variable cost)
harvest-system cost = variable cost + minimum adjustment
```

For `HasHarvestQuote=true`, `QuotedHarvestCosts` is the **final** job quote,
including mobilisation. It replaces model rates, site multiplier and model
minimum; its source survives in the returned quote. This permits properly quoted
fell-only work without imposing a combined machine-job calibration on it.
The explicit boolean is essential: Unity inline-class serialization can turn
null fields into default instances, so null is not used as an optional marker.

The baseline roadside route has `OwnerPaysHaulage=false`. Owner-paid transport is
a separate explicit charge at the configured haulage rate, including retained
material if transported. It does not invent a delivered-price premium. A future
mill-gate route must provide independently evidenced prices/sale-basis data;
standing-sale pricing is not implemented or mixed with this deck.

```text
cash revenue = sum(sold batch value)
external cost = contractor work + new material purchases + owner tool running + haulage
net cash = cash revenue − external cost
owner opportunity view = net cash − owner time shadow value
```

Ledger receipts are positive and payments negative. Component totals are
nonnegative. Retained/deadwood reference values and owner unpaid time are not
cash ledger entries. `SumLedger(result.Ledger)` exactly equals realized net cash.
Quote/result copy their arrays and nested quantity/batch data; later input or
configuration mutation cannot alter a previous result. DTOs are mutable for
serialization, not immutable persisted transaction records.

### Deterministic worked outputs

Synthetic mass fixtures test accounting, not biological tree yields:

| Fixture | Receipts | Variable work | Minimum adjustment | External cost | Net |
|---|---:|---:|---:|---:|---:|
| 10 t sawlog, first thinning | €950 | €210 | €2,290 | €2,500 | −€1,550 |
| 200 t sawlog, first thinning | €19,000 | €4,200 | €0 | €4,200 | €14,800 |
| Sold 100 t sawlog + 30 t pulp; retained 20 t stake; all forwarded | €10,640 | €3,150 | €0 | €3,150 | €7,490 |
| Same job, owner hauls the sold 130 t | €10,640 | €3,150 | €0 | €4,710 | €5,930 |

Mixed job retained stake reference: **€940**, reported with 20 t, excluded from
cash revenue. No flat CCF penalty is needed to produce job-scale differences.

## Verification

Harness: `Tools/Verification/Stage1WorkEconomyFoundationVerification.cs`.
Runner: `Tools/Verification/run_stage1_work_economy_verification.py` (stdlib only).

```bash
python3 Tools/Verification/run_stage1_work_economy_verification.py \
  --dotnet /media/jer/ZX20/Unity/6000.6.0f1/Editor/Data/DotNetSdk/dotnet --repeat 2
```

Uses the already-installed .NET 8 SDK. No package references or new dependencies.
Generates its project and all build output in ignored
`Build/Stage1WorkEconomyVerification/`; C# language version 9, warnings as errors.
Each repeat is a separate process and must produce an identical evidence marker.

Unity 6000.6.0f1 verification: temporarily copy the harness to
`Assets/ForestPrototype/Editor/`, then run:

```text
<UNITY> -batchmode -nographics -projectPath <economy-worktree>
  -executeMethod Stage1WorkEconomyFoundationVerification.Begin -logFile <log>
```

The harness exits explicitly. Remove the temporary script and its generated
`.meta` afterward. It has no auto-start hook, does not open/save a scene and does
not touch `forest-save.json`. Production `.meta` files are kept.

Coverage includes the packet's fourteen cases plus upfront cash/stock/time
shortages, contractor availability/policy, zero batches, all cost contexts,
split rates, fell-only incomplete/final quotes, provenance, explicit protection
materials, minute/cent rounding, threshold quantities, culture/input-order
independence, 100-repeat resolution, snapshot isolation, malformed inputs,
species/density/unit mismatches, overflow and exact long serialization beyond
the binary-double integer range. Both serializers roundtrip task/config/resources
and result data. Disabled and enabled quote flags are checked explicitly.

The first Unity run caught the null-marker issue above; it was repaired and is
covered by the final repeatable fixture. Final standalone and Unity results are
recorded below after cleanup. Evidence SHA differs across serializers because
their JSON text encodings differ; comparison is within the same backend, not a
claim that Unity and System.Text.Json produce identical bytes.

### Final verification results (2026-10-02)

- Standalone Release compile: **zero errors, zero warnings**; two independent
  processes pass **66 fixtures / 420 assertions**, evidence SHA-256
  `2085CEEC5DE02838967B8F4B39A14993E28AE97BE5C3A6F070709E42156AD2F4`.
- Unity 6000.6.0f1: production and temporary Editor harness compile; two fresh
  batch processes pass **66 fixtures / 420 assertions**, evidence SHA-256
  `05E5E664EE3D8F74D20D76B9E2629C48C6D30FDB8F955A5E9FA43228C8AB62E1`.
- Final Unity logs: ignored
  `Build/Stage1WorkEconomyVerification/unity-final-1.log` and
  `unity-final-2.log`. Initial failed serializer run is retained as `unity-1.log`
  and is not counted as a pass.
- Mixed mass/volume inputs are converted before aggregation; input validation
  does not add incompatible raw units.
- Temporary Editor script and `.meta` removed; production metadata retained.
  Unity-generated local VS Code changes restored to the original shared bytes.
- Existing source files, scenes, packages, settings, save schema and historical
  reference resources have no changes against BASE. No ecology/gameplay gate or
  rendered scenario acceptance is claimed by these pure-foundation tests.

### Minimal API usage

```csharp
var prices = Stage1EconomyDefaults.Create();
var task = new ForestryTask
{
    TaskId = "inspection-2027",
    WorldOperationId = "existing-plan/inspection-2027",
    Type = ForestryTaskType.Inspection,
    Quantity = new WorkQuantity { Unit = WorkQuantityUnit.SquareMetres, Amount = 10000 }
};
var actor = new ExecutionResources
{
    Capabilities = WorkCapability.BasicManagement,
    Tools = WorkTool.InspectionEquipment,
    AvailableOwnerMinutes = 240
};
var result = ForestryWorkCalculator.Resolve(
    task, WorkExecutionMethod.LandownerSimulated, actor, prices);
// Resolved=true; OwnerMinutesConsumed=180; cash delta=0 at baseline.
// Apply the referenced inspection/information effect and time commitment
// through the existing manager only once, after authoritative validation.
```

## POST-CLAUDE INTEGRATION POINTS

These are proposed exact adapters; **none is wired in this branch**.

| Existing system → new call | Returned result → existing authority |
|---|---|
| `ForestTreeMarkingManager.GetMarkedIds`, `ScenarioOneManager.AddMarkedTreesToWorkPlan`, `CreateFellingOrder` → new `ScenarioOneEconomyAdapter` builds one `ForestryTask(Harvest)` per commissioned intervention, preserving source order/tree IDs and chosen cost context; converts disjoint actual/estimated assortments into batches → `Quote` | Display job costs/minimum/retained values/eligibility in existing Work Plan; marks, targets, approval and world effects stay existing authority. **Group across trees before the floor.** |
| `ApprovePendingWork`, `ReservedContractorCashCents`, `ValidateOpenOrders` → quote selected worker against unreserved cash/stock/time snapshots | Reserve one job's external costs/time/materials; do not reserve each tree's share as another full job. Existing approved orders remain duplicate-execution authority. |
| `AdvanceYear`/`ResolveFelling` → refresh live target/assortment quantities, re-quote or `Resolve` approved job | After eligibility and successful `ForestTree.Fell`, apply one returned signed cash delta/ledger, update existing report/events/order status. Keep `BeginChangeBatch`/`EndChangeBatch`, single annual advance and current deadwood/residue creation. Do not also charge old hourly estimates or old whole-stem values. |
| Existing `FellingMaterialOutcome` → explicit switch to `TimberDisposition` | SellAndExtract → sale; KeepForUse → no sale and retained output; RetainAsFallenDeadwood → no sale/no forwarding and existing deadwood path. No ordinal casts or changed biological meaning. |
| `retainedTimberM3`/`TrySpendRetainedTimber` → read resolved KeepForUse outputs' original volume, or invert an explicitly matching density if mass was supplied | Deposit retained **volume** once into existing stockpile. Keep construction/sawpit/player wood units and spending unchanged. Do not deposit deadwood or revalue reference money as material quantity. |
| `TryPurchaseStock`, `GetStockQuantity`, `GetReservedStockQuantity`, `ResolvePlanting` → `ForestryTask(Planting)` with matching `ExistingStock` requirement → `Resolve` | Consume already-purchased stock once, apply work cost/time only; keep legacy/exact authoritative planting and clearance paths. Selecting wholesale versus current retail shop prices is separate configuration approval. |
| Crop Tree pruning-order creation and `ResolvePruning` → `ForestryTask(Pruning)` for eligible targets → quote/resolve chosen worker | Execute existing `TryPrune` first; commit cost/time only on success. Existing lift/target-height/year state remains authority. No guaranteed immediate timber premium. |
| Annual report/events → returned cost breakdown and ledger | Map existing contractor-work/timber/cash fields without double payment. Rich retained-assortment and owner-time/job history may need a **later separately authorised** persisted representation; no save changes here. |

### Smallest post-integration patch

1. Add a narrow `ScenarioOneEconomyAdapter.cs` translating existing orders and
   resource snapshots to this API; supply a bounded explicit assortment estimator
   or actual recovered-batch source. Do not substitute DBH→one product.
2. Commission/group harvest orders once, select contractor/owner in the existing
   Work Plan and reserve one aggregate quote. Continue to use existing approval,
   status and world-target records.
3. Replace only the old financial settlement within existing resolution with
   one calculator result; preserve world-effect order/failure checks and apply
   result only after success. Refresh quote if targets/quantities change and
   define partial-job failure handling before charging the aggregate minimum.
4. Add scoped integration tests for one-time harvest/payment, failed-world-effect
   no-payment, stock reservations, retained-volume deposit, annual ordering and
   unchanged biology across execution choices. Historical v1 preview/resources
   stay frozen; current economy changes need deliberate contract/test updates.

This is a small **financial adapter** boundary, not a claim that bucking,
grouped-job approval, durable owner-time/execution selection and richer job
history already exist. Persistence of those new live choices requires its own
authorised integration/save decision; the current task is deliberately unwired.

## Decisions and limitations

No product decision blocks this foundation. Before gameplay wiring, Manager
should approve the selected live deck, nursery retail/wholesale handling,
assortment source, intervention grouping and partial-failure charging, and
retained-material extraction/transport assumptions. The existing KeepForUse
record has volume but no physical extraction/location-cost record; the adapter
must supply that assumption explicitly rather than inventing it here.

No automatic bucking/quality/species yield, grants, taxes, NPV, salvage,
windblow task, availability calendar, tool inventory, stock purchases applied to
the world, or global economic history. The plain DTOs are Unity-serialization
ready but no save schema/file is changed. Production runtime behaviour in
Scenario One remains the inspected base implementation until a reviewed adapter
is deliberately integrated.
