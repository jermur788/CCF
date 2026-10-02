CURRENT IMPLEMENTATION INVENTORY
================================

Task: Timber Assortments & Material Yield Foundation v1. OpenAI/OpenCode, primary.
Starting HEAD `66bdd4d2da53e26b707a48809e9378c19175c209`; branch
`task/timber-assortment-yield-v1`, worktree `/home/jer/CCF-opencode-economy`.
Current context read directly from Git at
`db2b4f1c3c9eb5740709bf7eae9316b7a54f6eb8`; branch ancestry remains on the
older context. No merge/rebase or canonical-context edits.

| Existing class | Inspected authority at starting HEAD | Adapter boundary |
|---|---|---|
| `ForestTree` | `Diameter`/`SimulationDiameterCm`, `Height`/`SimulationHeightMeters`; species and ID. `Height` differs by legacy stage (1.1/≤2.7 m for sapling/young). `BiologicalStemVolumeM3 = DBH_cm² × 0.00007854 × Height × FormHeightRatio`, zero after felling or without species. No upper-stem diameters, bark thickness, taper equation or biological form/damage grading. | Snapshot ID/species/dimensions/volume **before** `Fell()`. Use the same height used by the supplied authoritative volume; do not silently switch to a different height. |
| `TreeSpeciesDefinition`, Sitka asset | FormHeightRatio 0.5, explicitly calibration [D] in existing terminology; not a measured taper coefficient. Public species volume interface remains unchanged. | Yield calculator consumes supplied volume, not a replacement allometry or species asset. |
| `ForestTree.Fell` | Idempotent CanChop guard, turns tree into stump, emits Felled and clears mark. No assortment or cash calculation. Visual stump height 0.35 m is presentation calibration. | Existing world effect remains authoritative; yield is an unwired pure calculation. |
| `ScenarioOneManager.CreateFellingOrder` / `ResolveFelling` | Pre-fell biological volume snapshot, provisional whole-stem €/m³ valuation, work cost; calls Fell; whole KeepForUse volume enters retainedTimberM3; separate deadwood/residue world records. | Future adapter replaces only product estimation/financial inputs. Do not add assortment income on top of old whole-stem revenue. |
| `ForestPlayer.ChopTree`, `TimberUnits` | Capacity-checked felling then 3–24 abstract wood units, approximately volume / 0.1 m³ with gameplay bounds. | Keep separate. No lossless conversion back from these bounded units. |
| `ForestBuildable`, `ForestWoodStorage`, `ForestSawPit` | Construction/survival wood/plank units; buildable retained-timber conversion 0.1 m³/unit. | Material foundation exposes volume/pieces, not buildings, milling yields or certified structural timber. |
| WorkEconomy at `66bdd4d` | Fixed-unit TimberBatch supports Pulp/Stake/Pallet/Sawlog, cubic centimetres or green grams, explicit density/price IDs, sold/kept/deadwood outcomes. Pure price/cost/eligibility/ledger calculation, unwired. | Separate adapter converts disjoint allocated logs to volume batches. No accounting rule changes required. |
| Pruning state | Lift count, current crown-base height, last pruning year; no DBH-at-cut, branch diameter, knotty-core or occlusion record. Cosmetic defect variants are not biological quality evidence. | Neutral quality by default. Future supplied quality flags/ranges; no inference from model appearance. |

## Evidence inspected

Labels: [E] published measurement/specification, [G] forestry guidance,
[I] inference, [S] simulation abstraction, [C] gameplay calibration.

1. **Irish Forestry Economics, Labour and Contractor Operations for CCF Stage 1.pdf**,
   complete source previously read; pp.4–7,19 explicitly describe multiple
   assortments, diameter/length criteria and species/moisture density.
   SHA-256 `fc51dc48e675951f345005aec4bf1c296954de61cc4f82dbf7b9566c34808741`.
2. **Irish_Sitka_CCF_Deep_Research_Report.pdf**, read all 15 pages, §17–18:
   basal area × form height; no fitted individual taper supplied; wood units
   separate, detailed timber quality deferred.
   SHA-256 `c2174dbdf85fdfbbe0e2873cdf9cb83f11bb012651abb3862f70054ee97f8e43`.
3. Teagasc, **Timber Products from Conifer Forests, Fact Sheet 3**, pp.1–2 [G/E]:
   sawlog 4.9 m / 20 cm small end; pallet 2.5–3.7 m / 14 cm; straight stakewood
   small-end 7–13 cm with buyer-dependent lengths (increasingly 3.4–3.7 m);
   pulp 3 m / selected minimum within 7–13 cm.
   https://teagasc.ie/wp-content/uploads/media/website/crops/forestry/advice/Fact-Sheet-3-Timber-Products-from-Conifer-Forests.pdf
   SHA-256 `f4a40aeee5849f08a363a9f900292006edb9e758ec824fd135e9b39d3433e0eb`.
4. Teagasc **Plan your Timber Sale** [G], independently read: pulp minimum
   small-end 7 cm, pallet 14 cm, sawlog buyer minimum 18 or 20 cm; poor form
   downgrades products; small straight timber can be stakewood.
   https://teagasc.ie/crops/forestry/advice/timber-harvesting/harvesting-and-selling-timber-from-conifer-forests/plan-your-timber-sale/
5. IFA **April–June 2024 survey** [E], independently read: observed lengths
   pallet 2.5/3.1/3.4/3.7 m; sawlog 4.9/5.5/6.1 m; stake 1.6 m with displayed
   >8/<15 cm. Its pulp <7 symbol conflicts with Teagasc's minimum-7 guidance;
   use explicit Teagasc minimum, not an invented universal IFA boundary.
   https://www.ifa.ie/markets-and-prices/timber-price-survey-from-april-june-2024/
6. Teagasc **Timber Measurement Course handouts**, pp.7–8 [G]: DBH at 1.3 m,
   form height accounts for taper. The table's Sitka form heights (2.83 at 8 m,
   4.56 at 12 m, 6.95 at 17.5 m) do **not** validate the game's 0.5 ratio.
   https://teagasc.ie/wp-content/uploads/2025/05/Timber-Measurement-Course-handouts.pdf
7. G.J. Hamilton, Forestry Commission **Forest Mensuration Handbook** (1975),
   printed p.190, geometric formulae for paraboloid and its frustum [G]:
   `V = πr²h/2`, frustum `V = πh(R²+r²)/2`. The geometric form is standard;
   choosing it for the entire game stem is **[S]**, not a fitted Sitka equation.
   https://cdn.forestresearch.gov.uk/1975/03/fcbk039.pdf
8. FAO, K. Jayaraman, **A Statistical Manual for Forestry Research**, §6.2.1 [G]:
   section measurement and Smalian/Huber/Newton volume methods, aggregation,
   explicit bark basis and variable moisture. This supports disjoint section
   accounting, not copying unrelated species coefficients.
   https://www.fao.org/4/x6831e/X6831E13.htm#5791

The general taper review by Burkhart & Tomé (2012), DOI
10.1007/978-90-481-3170-9_2, was inspected as a public abstract/reference list,
not full licensed text. It identifies simple/segmented/variable-exponent models
and the British Sitka work by Fonweban et al. (2011), DOI
10.1093/forestry/cpq043. No inaccessible fitted coefficients are claimed or used.

## Minimum model decision

Current data suffice for a **coarse, unwired, volume-budgeted approximation**.
They do not identify true individual taper. V1 uses a DBH-anchored paraboloid
for diameter screening and relative volume distribution [S], with supplied
biological volume as the conserved budget. This adds no growth/allometric state
and does not replace the existing biological volume model. Accuracy against Irish
measured stem profiles remains unvalidated. A measured squared-diameter profile
can replace this approximation through the same interface.

Industrial specifications remain source-labelled buyer/guidance examples,
not certification or a universal purchase promise. Default order is explicit
sawlog → pallet → stake → pulp, following the project economics report's
largest/highest-grade viable section first; no price optimiser or profitability
tuning. Selection of particular published nominal lengths is a scenario choice [S].

## Production model

Namespace `CCF.Forestry.TimberYield`, new scripts under
`Assets/ForestPrototype/TimberYield/`:

- `TimberYieldDomain`: measurements, requests, quality sections, allocated logs,
  residuals and per-stem/job results. Reuses the economy's existing assortment
  and disposition enums; no parallel enum with different product meanings.
- `TimberYieldConfiguration`: specifications, explicit allocation/length policy
  and a fresh replaceable Sitka configuration factory. No prices.
- `MerchantableStemModel`: local immutable shape snapshot, diameter screening,
  diameter ceiling and volume partition; explicit measured-profile alternative.
- `TimberYieldValidation`: dimensions, profile, provenance, units and config checks.
- `TimberYieldCalculator`: single-stem allocation and ordered job aggregation.
- `TimberYieldEconomyAdapter`: disjoint log volume → existing `TimberBatch[]` using
  caller-supplied opaque price/density IDs; verifies source/result volume totals.

Entry points:

```csharp
var config = TimberYieldDefaults.CreateSitka();
var stem = MerchantableStemModel.FromMetres(
    treeId, speciesId, dbhCm, heightM, authoritativeVolumeM3, measurementBasis);
var request = new StemYieldRequest { Stem = stem };
var single = TimberYieldCalculator.ResolveSingleStem(request, config);
var operation = TimberYieldCalculator.ResolveStandOperation(
    operationId, new[] { request }, config);
var batches = TimberYieldEconomyAdapter.ToTimberBatches(operation, marketBindings);
// A later existing-manager adapter supplies batches to ForestryTask.Timber.
```

The subsystem neither calls `Fell` nor spends money/creates inventory/world
objects. Results are estimates/planned log sections, not proof of physical
bucking. No scene, manager, species, ecology or save modifications.

## Taper and volume assumptions

For default profile, `H` is total height, `b` breast height, `D` DBH, `z` height:

```text
d²(z) = D² × (H − z) / (H − b)              [S], DBH anchored at b = 1.3 m
F(z)  = (2Hz − z²) / H²                    normalized paraboloid volume fraction
C(z)  = floor(supplied stem volume × F(z))  cumulative integer cm³
V(a,b) = C(b) − C(a)                       disjoint section volume
```

Squared-diameter comparisons use decimal arithmetic directly, avoiding a rounded
diameter admitting an undersize log. Displayed end diameters are conservative
integer-mm floors. `HeightToDiameter` returns the highest integer-mm point
meeting a diameter limit; the merchantable-top diagnostic does not guarantee
that a full nominal log fits there.

The absolute integral of the ideal DBH-anchored geometry generally **differs**
from the supplied biological volume. `GeometricProfileVolumeCm3` exposes that
difference. Section quantities use the supplied budget's normalized distribution,
not an increase to the ideal shape's volume. This is explicitly a volume-budgeted
yield proxy, **not a physically fitted taper-volume equation**. Do not interpret
normalization as a measured bark discount or quietly recalculate biological
stem volume from it. Default 0.5 form-height calibration remains untouched.

An explicitly selected measured profile supplies height/diameter nodes from base
to zero-diameter tip, with DBH agreement within a 1 mm quantization allowance.
V1 requires non-increasing diameter and strictly increasing heights. It linearly
interpolates **squared** diameter and integrates the trapezoidal area distribution
(paraboloid/Smalian interpretation [S/G]), again normalizing to the supplied
volume budget. It snapshots input points. This supports measured profiles later
without changing allocation or economy, but does not claim to represent all
irregular/multi-stem bole shapes.

Both input diameter and specification must use the same declared measurement/
bark basis. Current simulation volume has no explicit bark deduction. V1 retains
that input basis and reports it; industrial under-bark acceptance and a bark
model are not invented. A purchaser deck requiring another basis needs matching
measurements/explicit conversion before these calls.

## Configured products and allocation

| Product | Minimum small end | Maximum small end | Nominal lengths selected | Form policy |
|---|---:|---:|---|---|
| Sawlog | 200 mm | Unbounded | 4900 mm | Neutral normal form; supplied poor form/defect/damage can disqualify |
| Pallet | 140 mm | Unbounded | 3700,3400,3100,2500 mm | Same conservative supplied-form exclusions |
| Stake | 70 mm | 130 mm | 3700,3400 mm | Straight/normal-form assumption; form flags disqualify |
| Pulp | 70 mm | Unbounded | 3000 mm | Allows supplied downgraded form/defect; Unusable excludes |

These are Teagasc/IFA-informed **example specifications**, not all purchasers'
rules. Sawlog's 18-versus-20 cm choice and stake's IFA 1.6 m / >8,<15 cm
alternative remain configurable, not silently combined with conflicting sources.
No arbitrary pallet maximum is imposed: the higher-priority sawlog rule selects
eligible larger sections first. Separate min/max large/small-end limits, nominal
lengths plus bounds, species, minimum start/maximum end heights and required/
excluded quality flags are supported as data.

`GreedyButtToTipConfiguredPriority` is the explicit v1 strategy:

1. Reserve configured stump (350 mm [S], taken from existing visual stump scale,
   **not** an empirical felling height).
2. At the current start, examine species-matching specs by priority; ties use
   ordinal specification ID. Examine configured nominal lengths in explicit
   LongestFirst or ShortestFirst order.
3. Accept the first section satisfying diameter/length/section/quality criteria.
   No prices, future-value calculation, stand-average yield percentages or
   lookahead optimiser.
4. Record its disjoint budget volume. Skip configured cross-cut loss (5 mm [S])
   as a separately accounted residual interval.
5. If none fits, scan upward in 100 mm steps [S], accounting for skipped volume;
   this permits recovery above a supplied unusable section or after a start/
   diameter restriction. Coalesce adjacent residual intervals. Grid precision is
   configurable and is not an exact industrial bucking optimisation.
6. When below all diameter limits, or no species spec applies, keep the remaining
   volume residual. Never shrink an offcut to an unpublished minimum length.

The stem diameter ceiling is monotone in both supported profile kinds. Very small
or zero-volume stems award no product. Positive sub-cm³ planned pieces award no
zero-volume logs; their intervals stay residual. V1 engineering guards default
to 100 m height / 5 m DBH [S], not biological growth limits.

## Quality and retained material

Neutral quality (`None`) is a transparent assumption of usable ordinary form;
it is not evidence that the tree was inspected or is certified. Current visual
defect variants are ignored. Callers may supply whole-stem or half-open height
ranges for PoorForm, StemDefect, WindDamage, BrowseFormDamage, Pruned or Unusable.

Excluded flags anywhere in a candidate log reject that grade. Required flags must
hold **throughout** the log, not just overlap one small section. Boundary tests
cover this. Pruned is ignored by default specs: no automatic clearwood, product
upgrade, volume increase or premium. Use a pruning height range rather than
claiming the whole stem was pruned from one lift. A browse event is not itself
evidence of permanent form damage. Likewise, windthrow alone must not set a
WindDamage downgrade: the report explicitly notes intact windblown logs can
retain value. Flags mean an externally established section-quality restriction;
v1 simulates no damage, browsing, storm or pruning effects.

Allocation and disposition are separate. Per-stem default disposition and
assortment overrides allow sold sawlog/pulp with retained stakes, or all eligible
logs retained. Potential material-use categories include PolesAndStakes,
SmallRoundwood, RoundwoodForProcessing and SawlogForProcessing. Outputs expose
actual log lengths, diameters, volume, pieces and quality. These are potential
uses, not a promise that defective pulp is sound construction timber or C24.

Straight small dimension stems are evaluated as stakes before pulp; they can be
kept as poles/stakes. Any eligible assortment can be retained without a sale.
Below-commercial-threshold material stays residual; there is no fabricated
sub-7 cm industrial product or monetary price. Future bespoke small-roundwood
use rules can consume that residual explicitly. No milling recovery, fences or
buildings are implemented.

## Units and material conservation

Dimensions: integer millimetres; source DBH in cm / height in m is explicitly
quantized to nearest mm, midpoint away from zero. Volume: integer **cm³**, with
1 m³ = 1,000,000 cm³. Incoming m³ budget is floored to cm³, losing <1 cm³ rather
than creating material. No conversion from abstract survival wood units.

Every physical interval belongs to exactly one allocated log or residual:

```text
sold log cm³ + retained-use log cm³ + deadwood-reference log cm³
  + residual cm³ (stump, cut loss, offcuts, undersize top) = supplied stem cm³
```

Differences of cumulative integer volumes telescope: reconciliation tolerance is
**zero cm³ after input quantization**. Tests check volume AND full nonoverlapping
height coverage. Stump residual remains standing; other stem residues are not
automatically given an ecological deadwood disposition. Branches, foliage,
roots and their brash mass are outside the biological **stem** budget. Existing
residue art must not be used to invent additional measured branch volume.

Job results sort trees ordinally, reject duplicate trees, retain per-tree logs/
diagnostics/residuals, and summarize volume/pieces by species, assortment and
disposition. Aggregates exactly equal single-stem sums. Prior results are
independent of later input/configuration mutation. Plain DTOs are serializable;
no new persistent save representation or schema change.

## Representative outputs and calibration scope

No parameter was tuned to prices/profitability. These are qualitative grading and
conservation checks, not calibration against measured Irish taper/bucking data.
Synthetic fixtures use the current starting branch's biological volume formula
with form ratio 0.5:

| DBH / height | Sawlog cm³ | Pallet cm³ | Stake cm³ | Pulp cm³ | Residual cm³ | Input cm³ |
|---|---:|---:|---:|---:|---:|---:|
| 4 cm / 3.5 m | 0 | 0 | 0 | 0 | 2,199 | 2,199 |
| 9 cm / 14 m, supplied poor form | 0 | 0 | 0 | 16,563 | 27,969 | 44,532 |
| 14 cm / 14 m | 0 | 0 | 80,939 | 15,635 | 11,182 | 107,756 |
| 18 cm / 16 m | 0 | 123,732 | 66,483 | 0 | 13,360 | 203,575 |
| 35 cm / 25 m | 978,418 | 115,125 | 62,368 | 0 | 46,732 | 1,202,643 |

The mature fixture yields 3 sawlogs + 1 pallet + 1 stake, not one product for the
whole tree. The 14 cm fixture yields two stakes + one pulp log.

Unity additionally read the **actual** immutable v1 archive, without restoring
or advancing any world. Year-0 trees span the starting plantation; later samples
are median original P-prefixed crop trees. Dimensions are rounded mm below;
input volume uses the existing float volume-interface arithmetic before flooring.
Later archive trees are **historical examples**, not current-model forecasts.

| Archive / tree | DBH mm / height mm | Sawlog cm³ | Pallet cm³ | Stake cm³ | Pulp cm³ | Residual cm³ | Input cm³ |
|---|---|---:|---:|---:|---:|---:|---:|
| Y0 P1114 | 96 / 8,081 | 0 | 0 | 19,677 | 0 | 9,849 | 29,526 |
| Y0 P1416 | 155 / 12,165 | 0 | 41,030 | 42,878 | 19,131 | 11,907 | 114,946 |
| Y0 P0018 | 211 / 15,525 | 0 | 191,568 | 49,384 | 0 | 30,983 | 271,935 |
| Y50 P1712 | 235 / 22,857 | 185,755 | 236,659 | 40,820 | 0 | 30,492 | 493,726 |
| Y100 P1906 | 321 / 28,354 | 868,269 | 202,922 | 42,642 | 0 | 36,003 | 1,149,836 |

All selected first-thinning trees produce no sawlog; larger historical crop trees
produce sawlog with secondary assortments. This is the expected qualitative
relationship, not validation of an exact real product percentage.

## Exact WorkEconomy / existing-system adapter

`ResolveStandOperation` → `TimberYieldEconomyAdapter.ToTimberBatches`:

- One stable batch per disjoint log, source tree ID retained; input volume used
  exactly once, never copied as full stem volume into each product.
- Unit stays `TimberQuantityUnit.CubicCentimetres`. Explicit per-species/product
  `TimberMarketBinding` supplies existing RoadsidePriceId and DensityId.
- Adapter has no numeric prices/density or costs. WorkEconomy performs its own
  documented species/moisture conversion. Default fresh Sitka density may be
  selected explicitly there; it is not silently selected by yield.
- Sold logs are extracted. Kept logs use explicit ExtractRetainedToRoadside;
  deadwood-reference logs are not extracted. Owner-paid haulage is explicit.
- Residuals remain in the yield result, never a sale batch. The adapter checks
  per-tree and operation totals and rejects volume tampering/duplicate log IDs.
- WorkEconomy was exercised with actual generated batches and a final fixture
  job quote; retained reference value and ledger reconciliation passed. Existing
  per-tonne/minimum/eligibility/rounding rules were not changed.

Future ScenarioOne adapter:

1. Read stable target IDs and pre-fell dimensions/volume; preserve measured basis.
2. Resolve yield for the explicitly commissioned group and chosen material
   dispositions. Select market/density bindings in the financial integration
   layer, not the allocator.
3. Pass batches to existing `ForestryTask.Timber`; choose appropriate cost context
   and quote/split schedule for onsite retained/fell-only work. Apply job minimum
   once per commissioned job, not per tree/log.
4. Revalidate live trees, execute existing `Fell` once; settle successful work once
   via existing manager. Preserve annual ecology batching/order and failure paths.
5. Deposit only retained-use log **volume** into retainedTimberM3 once; keep
   residual/stump/deadwood separate. Use the existing authoritative deadwood
   creation path. No financial duplication with old whole-stem €/m³ revenue.

An all-residual operation intentionally produces an empty batch list. The
existing WorkEconomy harvest API requires quantified timber for a nonzero job;
an all-residual felling charge is therefore a **later financial adapter/API scope**,
not a fake zero-tree job or invented pulp revenue. This does not block yield
calculation or ordinary merchantable thinning consumption. No fix to the already
verified accounting model is bundled here. Mixed jobs value only recovered,
priced material; residual handling/mobilisation policy needs integration approval.

## Verification and performance

Harness: `Tools/Verification/TimberAssortmentYieldVerification.cs`. No auto-start
hooks, scene writes, saves or live felling. Standalone runner (stdlib + already
installed .NET 8, no packages):

```bash
python3 Tools/Verification/run_timber_assortment_yield_verification.py \
  --dotnet /media/jer/ZX20/Unity/6000.6.0f1/Editor/Data/DotNetSdk/dotnet --repeat 2
```

Unity: temporarily copy harness to `Assets/ForestPrototype/Editor/`, run:

```text
<UNITY> -batchmode -nographics -projectPath <economy-worktree>
  -executeMethod TimberAssortmentYieldVerification.Begin -logFile <log>
```

Remove the temporary script and generated `.meta` afterward. The Editor-only
diagnostic reads/validates the original frozen archive but never previews,
restores, advances or saves it. Production metadata is retained.

Final results:

- .NET Release compile: zero warnings/errors. Two independent processes pass
  **120 fixtures / 3,007 assertions**.
- Unity 6000.6.0f1: compile and two independent processes pass the same cases;
  actual JsonUtility task/config/result roundtrip passes.
- All four runs produce evidence SHA-256
  `88FAF806EBE8FD5F2047740AE965823F0EE4286BE23EE5AD1B209CD2A228C4A8`.
- Both Unity runs also report `TIMBER_YIELD_ARCHIVE_DIAGNOSTIC_PASS readOnly=true`
  and identical five-tree product tables.
- Original WorkEconomy gate rerun twice: 66 fixtures / 420 assertions, unchanged
  SHA `2085CEEC5DE02838967B8F4B39A14993E28AE97BE5C3A6F070709E42156AD2F4`.
- All requested fifteen cases covered, plus a dimension/length-policy sweep,
  full height partition, duplicate rejection, explicit species/quality ranges,
  required-quality coverage, conservation-checked adapter and snapshot isolation.

Measured whole-batch wall time, one warmup, synthetic varied stems, including
validation/allocation/aggregation (not process startup or serialization):

| Stems | .NET two runs, ms | Unity two runs, ms |
|---:|---|---|
| 100 | 11.828 / 13.238 | 31.639 / 23.547 |
| 500 | 30.036 / 31.344 | 147.244 / 117.751 |
| 1,500 | 108.013 / 106.981 | 519.880 / 352.516 |

Times are diagnostic, machine/backend-dependent, not deterministic assertions.
No speculative optimisation or claim of a measured ecology-relative speedup.
Evidence/logs in ignored `Build/TimberAssortmentYieldVerification/`:
`standalone-1.log`, `standalone-2.log`, `unity-1.log`, `unity-2.log`.

## Limitations / decisions

Foundation complete; no product decision blocks it. Before live wiring, approve
the approximation/spec deck and obtain measured Irish/British Sitka taper/buyer
spec validation where exact product yields matter. Also settle bark basis,
retained-material logistics, all-residual charging and partial-job failure.

Default species is Sitka only; other species remain residual unless explicitly
configured. No bark stripping, knots, strength grading, bucking optimiser,
branch/foliage yield, salvage, root plates, damage simulation, milling, construction,
prices or wallet. Shorter than breast-height stems have no valid DBH input and
are rejected rather than assigned fabricated merchantable dimensions. Neutral
form, stump, kerf and scan step are documented abstractions. The measured-profile
path is a monotone piecewise approximation, not a detailed 3D engine.

Unwired: existing ScenarioOneManager, felling, stockpile, ecology/browsing,
species assets, save schemas, v1 resources, scenes/prefabs, packages/settings and
canonical context all remain unchanged. Task branch remains based on 66bdd4d.
