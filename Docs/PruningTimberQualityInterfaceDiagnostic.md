# Pruning → Timber Quality Interface — diagnostic only

**Status: proposal for a later bounded task. No production pruning, quality,
residue, economy or save mechanics are changed.**

Prepared after Timber Assortment & Material Yield v1 was verified and committed
at `5af31df255bc69c1d58e9c0b670ec601d5911298`, as the packet's optional continuation.
Context: `db2b4f1c3c9eb5740709bf7eae9316b7a54f6eb8`; inspected implementation
remains the task branch based on `66bdd4d`.

## Evidence and accepted direction

- Project decision D-014: select Crop Trees and generate eligible work; individual
  cut marking is not the management interaction. D-009: worker/execution choice
  must share the same authoritative result.
- **Irish Forestry Economics, Labour and Contractor Operations for CCF Stage 1.pdf**,
  pp.2,9,11,17,21: pruning takes labour, aims at future quality, and does not have
  a guaranteed current €/t premium. Report productivity 12–20 trees/hour is [I/C],
  not a universal Irish work rate.
- **Irish_Sitka_CCF_Deep_Research_Report.pdf**, §18: larger branches/knots and
  wood properties matter, but detailed knot/ring-width grading was deferred.
- Teagasc **High pruning of conifer and broadleaf trees**, independently read:
  https://teagasc.ie/crops/forestry/advice/management/high-pruning-of-conifer-and-broadleaf-trees/
  [G]: healthy/productive/stable crop near or after first thinning; straight,
  vigorous selected conifers ideally no more than 18 cm DBH, with potential
  to increase diameter 2.5 times; 500–600 crop trees/ha; first lift to 3.5 m,
  second to 6 m after 2–4 years; preserve collar/bark, avoid flush cuts/stubs.
  It says timber **may** attract a premium, not that every pruned log does.
  Broadleaf shaping is a separate operation; do not copy conifer schedules to it.
- Teagasc **Timber Measurement Course handouts**, p.5, supports the two-lift
  3.5/6 m and 2–4-year guidance; not a fitted occlusion/clearwood model.

Labels: [E] empirical/published specification; [G] guidance; [I] inference;
[S] abstraction; [C] calibration. No occlusion time or premium is assigned here.

## What currently exists

At the starting branch:

| State/rule | Actual implementation | What it cannot establish |
|---|---|---|
| `PruningLifts`, `CrownBaseHeightM`, `LastPruningYear` | Number of lifts, highest designated clear-stem height, last successful year | Diameter/branch size at each lift, cut quality, closure time or knotty-core diameter |
| `ForestTree.TryPrune` | After CanPrune: records new height, multiplies crown radius by 0.92, increments lift count/year and refreshes visuals | Branch volume, mass removed or clearwood recovery |
| `CanPrune` | Living/choppable tree; at most three lifts; >=5 years between lifts; target higher than prior height and <60% of tree height | Teagasc <=18 cm DBH opportunity, natural live-crown ratio, stability/health assessment or work-risk class |
| `ScenarioOneDefinition` | Default targets 2.5/5/6.5 m [D] | These are not the Teagasc two-lift 3.5/6 m guidance |
| `ScenarioOneManager.ResolvePruning` | Calls authoritative TryPrune before charging; records task completion | Lifetime quality cohort/history per height band |
| Scar/prefab presentation | Age-dependent appearance, including the existing five-year visual healing threshold | Biological occlusion or knot-free wood proof |

These differences are recorded for a later policy/calibration task. They are not
corrected by the assortment task, and source guidance is not silently promoted
to an accepted replacement mechanic.

## Timing windows and information worth retaining

The guidance implies **size and future growth opportunity**, not a universal
pruning age. A small early defect core can eventually be enclosed by later wood;
late pruning cannot remove knots already inside the stem [I]. Tree age alone is
not enough. The last-year/current-height fields discard information about earlier
lifts, so historical saves cannot honestly reconstruct exact DBH/branch size at
those operations.

Recommended minimal future event record [S], only on a newly successful existing
pruning operation:

```text
PruningLiftQualityRecord
  treeId / worldOperationId
  operationYear
  previousPrunedHeightMm / newPrunedHeightMm
  dbhMmAtOperation
  observedOrModelledLocalStemDiameterByPrunedBand (if available)
  largestCutBranchDiameterMm (optional, with an explicit known/unknown flag)
  cutQualityOrStemInjuryAssessment (unknown unless genuinely supplied)
  sourceAndEvidenceStatus
```

Retain one record per lift, with each newly treated height band distinguishable.
Optional crown-condition information must distinguish **natural live-crown base**
from the currently designated pruning height: those are not interchangeable.
Do not derive a quantitative wood-quality observation from the chosen art model.

The first later task should record history and expose provenance, rather than
immediately turn it into a simulated grade premium. Durable storage would require
a separately authorised save task, including unknown legacy history handling.

## Proposed connection to yield quality

Existing v1 interface already accepts global/ranged `StemQualityFlags` and emits
that metadata with each allocated log. Default specs ignore Pruned. A possible
adapter can annotate a genuinely recorded pruned interval with **history only**:

```text
existing successful pruning event
→ recorded lift/history band
→ StemQualitySection(start, end, Flags = Pruned)
→ TimberYield calculation preserves history metadata
→ unchanged default assortment allocation / unchanged default prices
```

`RequiredQuality=Pruned` checks a whole-log history interval but is **not** a
clearwood/industrial-grade certification. A later quality model should distinguish:

- pruning history present;
- cut wounds not assessed / not known closed;
- assessed occlusion state;
- estimated knot/defect-core diameter at a height;
- estimated later clearwood envelope/potential;
- actual buyer grade/acceptance, supplied independently.

A simple future quality estimate might compare final local diameter with an
estimated defect core, but the core depends on diameter at treatment, branch
diameter and growth until occlusion. Neither a fixed elapsed-years threshold nor
the existing 0.92 crown-radius factor provides those quantities. The research
in this task does not justify a numeric defect-core allowance or occlusion curve.

Quality geometry may influence downgrade/acceptance or clearwood potential,
not inflate total stem material. A premium, if separately evidenced and enabled,
belongs to the buyer price book. The responsible party must not create different
biological quality for the same successfully prescribed pruning result.

## Pruning residues / possible material outputs

Pruning removes branches and associated foliage, not the merchantable bole.
Potential later outputs are small branchwood, twig/foliage residue and possibly
usable straight branch sections under an explicit use specification. None is
automatically a sawlog, fencing pole or commercial pulp delivery.

The current code has no authoritative branch diameters/lengths, wood volume or
fresh mass. The fixed crown-radius reduction is a response calibration, **not a
branch-biomass fraction**. FBX mesh/bounds also do not provide ecological mass.
Therefore no kg/m³ residue yield can be calculated defensibly from current
pruning state alone.

Proposed future output envelope [S]:

```text
PruningResidueDescription
  sourceTreeId / sourceOperationId / operationYear
  treatedHeightBand
  component = Branchwood | TwigAndFoliage
  quantityKnown = false unless measured or separately modelled
  quantityAndUnit + measurement/model provenance when known
  potentialUse / retained-on-site disposition when explicitly chosen
```

This is a proposed data interface, not implemented code. Unknown quantity is not
zero measured production. Quantified branch material must have its own supplied
branch budget/conservation check and density/moisture assumptions. Do not subtract
it from the existing stem-volume budget, or add it to stump/top/offcut volume
already conserved by TimberYield. Nutrient/deadwood effects remain ecology-owned.

## Recommended later bounded task and decisions

1. Manager confirms whether/when pruning mechanics should adopt researched
   two-lift guidance or keep the current calibration. Consider recovery/live-crown
   and eligibility explicitly; do not silently change the annual ecology stack.
2. Record new lift history without retroactively inventing missing DBH/branch
   measurements; specify a save migration only if authorised.
3. Add a diagnostic-only quality-band adapter first. Prove no change in baseline
   yield quantities/prices and no influence from visual scar state.
4. Research/validate occlusion and defect-core estimates before numeric clearwood
   recovery. Obtain buyer specifications before a priced quality grade.
5. Quantify residues only after authoritative branch measurements or a separately
   evidenced branch-yield abstraction exists.

Acceptance gates for that future task: history survives an authorised roundtrip;
unknown legacy values stay unknown; whole-log band coverage; no automatic pruning
premium; unchanged stem conservation; separate branch conservation; consistent
world result across execution parties; current pruning behaviour remains governed
by its accepted policy.

**Optional diagnostic complete. Implementation and product adoption deferred to
a future task; production pruning mechanics remain unchanged.**
