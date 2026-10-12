# Phase 6 — Oak and Beech timber assortments and economy

Authority: D-062 and the Manager-approved supplement "Phase 6 Oak/Beech timber & economy evidence" (research branch `research/oak-beech-timber-economy-v1` at `b9d4cd9` was not visible on the remote from this session, so the approved summary in the supplement was used, as it allows).
Status: **implemented; the pure layer is verified by executing the production code offline; the Unity-side wiring is type-checked only; the in-Editor gate is written and has never been run.**

## What exists now

In Scenario One an Oak or Beech tree is graded into products from its size and form, valued per product, and sold through the same contractor harvest job as Sitka, **when the game is on growth model 2**. On growth model 0 or 1 nothing changes: Oak and Beech are still unmarketed ("charged for felling but earn no sale revenue"), exactly as at the locked base.

### Why growth model 2 is the switch

The broadleaf market is part of the three-species package (D-059). The economy is derived at quote time from the saved growth model, so no new persisted field, no schema change and no migration are needed, and a game that started under the old economy keeps it. `ScenarioOneManager.BroadleafMarketActive` is `GrowthModel >= SpeciesAware`. `NewGameGrowthModel` is still 1, so **no new game uses the market until the Manager adopts model 2.** The coupling of an economy rule to an ecology model number is a pragmatic choice with a clear precedent (growth model 1 also bundled adult mortality with deadwood creation); the alternative is a separate saved market-model field, which would be a schema decision.

## Supplement constraints → how they were met

| Constraint | Implementation | Verified by |
|---|---|---|
| Oak ladder: veneer/planking > beam > fencing > second-quality planking > firewood | `TimberYieldDefaults.BroadleafSpecifications()` priorities 10–50 | offline `OAK_LADDER_ORDER` |
| Veneer: over 2.7 m, mid-log ≥ 45 cm, clean | length 2.8 m, `MinimumMidDiameterMm = 450`, required quality `Pruned` | offline veneer rule checks |
| Beam: over 2.5 m, mid-log ≥ 30 cm, some defects allowed | length 2.6 m, mid ≥ 300 mm, excludes fork/defect/damage/unusable | offline; mid-log boundary exact to 0.1 cm |
| Beech: second-quality planking and firewood only; does not inherit Oak | two Beech specifications, species-keyed | offline `BEECH_HAS_NO_VENEER_BEAM_OR_FENCING_CLASS`, `LARGE_PRUNED_BEECH_DOES_NOT_INHERIT_THE_OAK_LADDER` |
| No broadleaf pulp, stake/pole class or C24 class | none created | specification list asserts only the three species and the five products |
| Prices: Low/Reference/High/Selected, evidence ranges kept separate from scenario | `EconomicParameter` per product with sources and labels | offline `PRICE_PARAMETERS_USE_LOW_REFERENCE_HIGH_SELECTED_WITH_SOURCES` |
| Firewood is [C] | labelled `GameplayCalibration` with "NOT supported Irish stumpage evidence" | offline `FIREWOOD_PRICE_IS_LABELLED_CALIBRATION` |
| No flat broadleaf rate | one explicit price per species and product; five distinct Oak prices | offline `NO_FLAT_BROADLEAF_RATE`, `REVENUE_IS_PER_PRODUCT_NOT_A_FLAT_RATE`; `ScenarioOneDefinition.broadleafTimberValueCentsPerCubicMetre` is read by no production code and stays dormant |
| No species-specific harvest cost multiplier | Oak/Beech batches use the same work-equivalent proxy density as every species | offline `HARVEST_COST_IS_GENERIC_NOT_PER_SPECIES` (Sitka, Oak, Beech: identical cost) |
| Quality from actual state, no random grades | pruned stem section from persisted pruning history | offline pruning/eligibility checks; see "Quality model" |
| Pruning affects eligibility, not price | price book unchanged by grading | offline `PRICE_BOOK_UNCHANGED_BY_GRADING`, `PRUNING_RAISES_VALUE_ONLY_THROUGH_ELIGIBILITY` |
| Small Oak cannot be premium merely because it is Oak | dimension thresholds | offline, DBH 12/20/25/29 cm |
| Forked / malformed Oak capped | excluded-quality flags per rung | offline, whole-stem and sectional forks, stem defect |
| Large high-quality Oak reaches upper classes | pruned, straight, large | offline (synthetic 60 cm × 24 m) |
| Beech can reach second-quality planking | sound ≥ 25 cm small end | offline |
| Low-quality Oak/Beech have a firewood fallback | firewood rung accepts any quality | offline, `Unusable`, wind-damaged |
| Sitka unchanged | Sitka rules and price entries untouched; mid-log rule skipped when 0 | offline: 3,000 seeded Sitka stems and a 400-stem operation **identical to a fingerprint captured from the unmodified base commit**; Sitka quote unaffected by the extra prices |
| Oak maturity not accelerated | no growth change | none made |
| Shake out of P0 | not modelled | — |

## The products

| Product | Species | Priority | Dimension rule | Quality rule | Reference price (evidence, euro/m³) |
|---|---|---:|---|---|---|
| First-grade veneer / planking | Oak | 10 | 2.8 m, mid-log ≥ 450 mm | pruned section required; no fork, defect, wind or browse form damage, unusable | 230 (range 180–280, midpoint selected) |
| Beam | Oak | 20 | 2.6 m, mid-log ≥ 300 mm | no fork, defect, wind damage, unusable | 145 |
| Fencing material | Oak | 30 | **[C]** small end 12–25 cm, 1.8 or 2.4 m | not wind damaged or unusable | 100 |
| Second-quality planking | Oak, Beech | 40 / 10 | **[C]** small end ≥ 25 cm, 2.4 m | no stem defect, wind damage, unusable | 60 |
| Firewood | Oak, Beech | 50 / 20 | **[C]** small end ≥ 7 cm, 2.0 m | any | **[C]** 30 (20–40) |

Evidence labels: veneer and beam dimensions and all non-firewood prices are `Inference` (the Manager-approved summary of Irish evidence, not independently re-checked here). Everything marked **[C]** is chosen for gameplay and is not an Irish specification. Selected prices are always `GameplayCalibration` as a selection. The size caps on Oak fencing exist because the approved ladder ranks fencing above second-quality planking: without an upper bound, fencing would absorb every log that planking could take.

### Units, and one thing the evidence could not settle

The approved evidence is euro per cubic metre; the price model is cents per tonne with a mass density. To keep revenue per cubic metre equal to the evidence without claiming an Oak or Beech density, each price is converted at the **890 kg/m³ fresh-volume work-equivalent proxy that harvest work quantity already uses for every species**, rounded to whole euro per tonne (veneer reference ≈ €258/t ≈ €229.6/m³). The engine requires a same-species density for every volume batch, so Oak and Beech have density entries holding that proxy, labelled "not a measured green density". Changing one to a real density would also require changing the harvest work accounting, otherwise cost would stop being generic.

**Not stated in the supplement:** whether the quoted prices are roadside, standing/stumpage or delivered. They are treated as roadside values [C]. This should be confirmed against the research file before the numbers are presented to players as Irish prices.

## Quality model, and what was deliberately not built

The only persisted per-tree form state is **pruning history** (lift count, crown base height, last lift year). There is no fork, shaping, sweep or browse-damage state on a living tree (browse form damage persistence is deferred, D-044). So:

- **"Clean" = a pruned stem section** from the ground to the crown base: `ScenarioOneEconomyAdapter.PrunedSections`, applied only to species whose ladder has a quality-dependent rung (Oak), so Sitka stems are untouched. A log qualifies for veneer/planking only if the whole log lies inside that section. More lifts (2.5, 5.0, 6.5 m) unlock more clear logs; the offline check shows 0, 1 and 2 veneer logs at the three lifts for a 60 cm × 24 m oak. This marks eligibility; it sets no price and it does not model when the clear wood formed.
- **Forks and defects** are honoured when a stem carries those flags (the engine and ladders cap them below the premium classes), but nothing in the live simulation sets them on living Oak yet. They are exercised through synthetic stems and, for windthrow salvage, the existing damage sections.
- **STOPPED — a persistent 1–4 stem-form grade.** The supplement requires proof that initial and later values can be derived deterministically from existing state. They cannot: nothing in the starting stand, natural recruitment or nursery stock defines a form, and nothing records one afterwards. Any initial assignment would be arbitrary, which the supplement forbids. A grade would need a defined origin (for example from planting stock, early-life competition history or browse history) and a persisted field; that is a design and schema decision for the Manager.
- **STOPPED — formative shaping as a separate concept.** Distinguishing shaping at 1–2 m from high pruning needs a new per-tree state, a work-order type and UI. The current schedule lifts every species to 2.5 / 5 / 6.5 m; changing it per species is a pruning-system change. Not built. The inspection text says plainly that unpruned Oak cannot reach the clear class.

## A biological finding the Manager should weigh (not a defect in this phase)

Under the current Oak adult growth (Phase 2–5 record) DBH at tree age 112 is at most about 38 cm with zero competition and about 26–31 cm with moderate competition. A 45 cm mid-log diameter therefore cannot occur in Scenario One, and beam-size stems are marginal and late. The supplement says Oak's unrealised potential is a teaching point; this is the mechanism behind it, and it comes from the DBH calibration as much as from the ladder. The offline checks use synthetic 60 cm stems to prove the rules; the simulation will rarely present them. If that is not the intended experience, the DBH calibration is the lever, and it needs the evidence listed in `Phase2to5-BroadleafParity.md`.

## Player-facing change

- Work Plan harvest card: products show readable names (First-grade veneer / planking, Beam, Fencing material, Second-quality planking, Firewood; the four Sitka names are unchanged). The "no market" line now refers to the species concerned instead of "broadleaf".
- Tree inspection "If felled now": under model 2, Oak and Beech show their products and, for broadleaves, a short explanation that grade depends on size and form, whether pruning has been done, and why premium Oak needs a large, straight log. Under model 0/1, the previous "No timber market is configured" line remains.

## Save compatibility

- `TimberAssortment` appends five values; the four existing integers are unchanged (offline check). Annual-report sale records store the product as an integer, so reports with the new products load in this build; an older build would reject them as an undefined product (the validator uses `Enum.IsDefined`) — intended, and only possible after a model-2 game sells Oak or Beech.
- No new persisted field; `ForestSaveData.CurrentVersion` stays 20; no migration.
- The harvest quote cache key now also includes the market switch and each target's pruning state (a lift changes grading without changing height or diameter). Not persisted.

## Verification

Executed here (`run_offline_check.py --suite timber`): **`THREE_SPECIES_TIMBER_PASS checks=71`**, using the unchanged production `TimberYield` and `WorkEconomy` sources.

Type-checked only: `ScenarioOneEconomyAdapter`, `ScenarioOneManager`, `WorkPlanView`, `TreeInspectionView` (`run_typecheck.py` prod and tools: no error beyond baseline). One real mistake in my own UI edit (a missing `using` and a wrong property) was caught by it and fixed.

**Not run — Unity:** `Tools/Verification/ThreeSpecies/BroadleafTimberEconomyVerification.cs`. Covers: model 1 keeps Oak/Beech unmarketed and Sitka revenue; model 2 earns revenue with nothing unmarketed; Sitka revenue and total cost identical across models; unpruned vs pruned Oak (eligibility changes, prices do not, Beech and Sitka unmoved); the quote cache invalidates on a pruning lift; re-resolution at settlement reproduces the quote; cash equals the job ledger; the annual report records the new products; the save validates and reloads, and an undefined product is still rejected.

```text
CCF_STAND_GEOMETRY=0 <UNITY> -batchmode -nographics -projectPath <checkout> \
    -executeMethod BroadleafTimberEconomyVerification.Begin -logFile <log>
# expect BROADLEAF_TIMBER_VERIFY_PASS; remove the copied script and its .meta afterwards
# and the existing economy gates must still pass unchanged (defaults are unchanged):
#   ScenarioOneEconomyIntegrationVerification (its Beech 'unmarketed' assertions hold on model 1),
#   TimberAssortmentYieldVerification, Stage1WorkEconomyFoundationVerification, ScenarioOneDeadwoodVerification.
```

## Known limitations and open decisions

1. Not run in Unity (the whole Unity-side wiring).
2. Price basis (roadside / standing / delivered) of the approved figures is unstated; treated as roadside [C].
3. Oak/Beech density entries are work proxies; real densities need accompanying work-accounting change.
4. Fencing, second-quality planking and firewood dimensions are [C].
5. A persistent form grade and formative shaping are stopped (above).
6. Premium Oak is close to unreachable under current Oak DBH growth (above).
7. The legacy flat `broadleafTimberValueCentsPerCubicMetre` (6,500) remains in `ScenarioOne.asset`/`ScenarioOneDefinition` and is referenced only by an old verification; removing it is a separate clean-up (serialized asset).
8. Salvage of windthrown Oak/Beech would be graded by the same ladders with the existing damage sections; not specifically tested.

## Canonical updates required (not made)

- `decision-log.md`: D-062 acceptance, the supplement's ladder, the model-2 coupling, the stopped sub-features and the open price-basis question.
- `unity-project-overview.md`: the five new products, `MinimumMidDiameterMm`, `CreateScenarioOne`, `BroadleafMarketActive`, the work-proxy density convention.
- `current-milestone.md`: the Oak/Beech market as part of the pre-demo requirement; the premium-Oak reachability finding.
