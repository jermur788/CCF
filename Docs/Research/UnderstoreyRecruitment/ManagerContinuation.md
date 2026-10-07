# Manager continuation — survival-first competition

Status: **SAVE SCHEMA DECISION REQUIRED**. Audit ed960b7 is retained and pushed. Base main a8596df is an ancestor of this dedicated branch. No integration into main. Only production change is juvenile display normalization; survival/save/RNG/growth rules remain unchanged.

## Competitor-state gate

The six saved covers are normalized habitat proxies, not botanical populations. `ferns` does not identify bracken; `shrubs` does not identify bramble; `grasses` does not identify dense competing grass/rush. The visual palette derives bracken/bramble/dwarf-shrub appearances from these proxies and light. Reusing those appearances as authoritative competitor state would make biology follow the art and silently treat unrelated plants as competitors. Total habitat cover is rejected.

| Category | Current authoritative state | Classification for proposed v1 | Evidence |
|---|---|---|---|
| Bramble | No independent saved cover; generic shrubs only | MATERIAL COMPETITOR in dense patches | Transfer evidence B; no universal annual rate |
| Bracken | No independent saved cover; generic ferns only | MATERIAL COMPETITOR in dense patches | Transfer evidence B; ordinary ferns excluded |
| Dense grass/rush | Generic grass cover cannot resolve competing type/site/density | MINOR / UNCERTAIN; excluded from initial proposal | Mixed context evidence; no justified universal harmful category |
| Other woody shrubs | Generic shrub cover includes habitat/bilberry proxies | MINOR / UNCERTAIN; excluded from initial proposal | Requires explicit identity and mechanism |
| Moss, fungi, litter, low herbs, ordinary habitat cover | Existing cover proxies or presentation | NON-COMPETITOR FOR V1 | No automatic penalty; absence of v1 penalty is not proof of no ecological interaction |

No production competitor categories have been adopted. Proposed variables below have **no existing persistence**. Habitat fields retain their current meaning.

## Concrete schema decision for Manager

Approve or reject the following minimal two-category state design before implementation. Names are proposals, not added fields.

| Location / field | Type / unit / range | Why not derivable | Proposed default and migration | Save impact |
|---|---|---|---|---|
| ScenarioUnderstoreyCell.brambleCover | float; independent horizontal cover fraction 0..1 | Shrubs mix bramble and other vegetation; visual palette is not biology | New model 2 games initialize from an explicit reviewed competitor target/site rule; no numeric target approved yet. Existing v17 retains model 1 and ignores missing field | One numeric property per cell |
| ScenarioUnderstoreyCell.brackenCover | float; independent horizontal cover fraction 0..1 | Ferns mix bracken with ordinary shade ferns | Same version isolation; do not relabel old fern cover as bracken | One numeric property per cell |
| PlantingClearancePatch.brambleCover | float; local independent cover fraction 0..1 | Cell average and treatment-year display suppression cannot preserve a recovering cleared spot | Newly treated model 2 patch starts at 0; legacy model 1 patches remain unchanged/ignored | One numeric property per existing patch |
| PlantingClearancePatch.brackenCover | float; local independent cover fraction 0..1 | Same local recovery requirement | Same | One numeric property per existing patch |
| PlantingClearancePatch.competitionUpdatedYear | int; ecological year | createdYear identifies treatment, not last annual advancement | New model 2 treatment stamps treatment year; model 1 ignores absent value | One integer per existing patch |

Use the existing cell lastUpdatedYear for cell advancement; reuse patch center/radius/createdYear, existing save lists, clearance footprints and work orders. No new succession manager, taxon list, art fields or species matrix. Cell competitor covers represent background cover outside local treatment overrides; exact juveniles read the most recent covering patch (deterministic tie rule required). Natural relative abundance uses area-weighted cover with overlapping patch footprints counted once. This spatial aggregation must be verified before adoption; do not subtract both patch area and a second cell-area reduction. Area clearance zeros the whole targeted cell plus its applicable local overrides, and retains current juvenile-removal rules.

Cover fields can overlap vertically: they do not sum to one. Expected text JSON cost is linear: two scalar properties per cell and three per retained patch, ordinarily tens of bytes each; actual serialized bytes must be measured, not asserted here. No silent v17 conversion. Pending approval, save 18/new model 2 only; existing v17 remains model 1, Reference stays0. Missing/invalid model 2 state must be rejected or initialized by an explicit approved migration, never guessed from art. A model 1 save should not acquire causal competition merely by loading in the new build.

Proposed recurrence reuses existing per-year Lerp with .30 gain/.45 loss [D], sharing cell light/site drivers. These are not measured bramble/bracken recovery rates. Specific target functions and starting covers still need calibration [C/I], independently of the visual palette. Approved schema is necessary but not sufficient for production adoption.

Existing habitat recovery measurements remain valid: fixed-target fraction restored at years 1/3/5/10/20 is .30/.657/.83193/.97175/.99920 under .30 gain. Closed/moderate/open light cases are retained in `Evidence/recolonisation_runtime.csv`. These are habitat recurrence observations, **not competitor-specific recovery measurements**. No new biology fields or save/model bump were made.

## Offline survival-form selection

Reproducible script: `Tools/Verification/UnderstoreyRecruitment/survival_forms.py`. Evidence: `Evidence/Continuation/survival_forms.csv` and JSON. 1,944 dimensionless outputs from reduced cover/type, browse/protection, height and strength blocks; three light settings, Sitka/Oak/Beech and natural/planted expected-survival adapters. These are synthetic arithmetic comparisons, not Unity model 2 runs or field estimates. Neither adapter samples exact individual mortality.

Let b,k be independent competitor cover fractions, c=max(b,k), v=clamp((1.5-height)/1.2,0,1). Annual vegetation-loss probability is strength × response × v. max combines overlapping types without summing independent covers into an unbounded penalty [I]. The height window reaches full vulnerability below .3m and zero at 1.5m [C], requiring sensitivity and ecological review. No height-growth effect.

| Form | Normalized response | Extra unsupported choices | Assessment |
|---|---|---|---|
| Linear | c | None beyond selected hazard/window | Leading form for next integrated trial: transparent bounded response |
| Saturating | (1-exp(-2c))/(1-exp(-2)) | Shape2 [C] | Stronger loss at intermediate cover without a discriminator |
| Threshold + saturation | Same saturation of max(0,c-.25)/.75 | Threshold .25 and shape2 [C] | Adds an unsupported safe-cover boundary |
| Type-weighted saturation | Saturation of (b+.5k)/1.5 | Unequal bracken weight .5 and shape2 [C] | Genuinely different equal-cover responses; no evidence warrants that ranking |

Select linear **for the next disposable trial**, not as an empirically superior curve or approved production model. Strengths .15/.35/.60 [C] bracket small/moderate/strong extra annual hazard; none is a transferred Harmer coefficient. The .35 middle case at .6m and .9/.9 cover gives .23625 vegetation loss; at zero cover or height>=1.5m gives0. Species share the response. No species-specific decimals. No extra light interaction: canopy light remains its own driver, and missing evidence does not justify a second shade penalty.

Illustrative accounting starts at1: vegetation loss, then browse loss on survivors, then separately labelled other/light loss, then remaining. Residual <1e-12; applying browse after vegetation changes absolute browse loss through remaining stock, not through a shared stress coefficient. Browse coefficient .25, pressure0/.8, other-loss .1×(1-light) are **C toy controls**, not production rules. Protection eliminates toy browsing only, leaving the vegetation probability unchanged. Requested establishment, seed supply, capacity and growth are absent from this isolated survival experiment and unchanged in production. Separate vegetation-loss runtime accounting is required in future model 2, with declared ordering.

## What remains before production

The schema gate prevents an honest integrated model 2 management matrix. The accepted 48 real 100-year model 1 runs and their10/25/50/100-year results remain the baseline; they must not be relabelled as candidate outcomes. New model 2 seed/height/promotion/adult/cash/clearance-cost/planting/shelter trajectories are **not available**. No conclusion that the new competition mechanic preserves Scenario One viability or makes paid clearance worthwhile can be drawn yet.

After schema approval: review target/default functions and hazard/window range; add separate natural/planted vegetation accounting; verify patch overlap and recovery; then run reduced long-run thinning/clearance/browse/protection worlds and economy gates. Demonstrate no-clear-needed, useful, wasted, harmful and repeated-management cases. Reject always-clear dominance. Existing clearance destroys stocked juveniles while freeing capacity; that accepted trade-off remains intentional.

No canonical decisions or Claude tutorial files changed. The approved state/parameter/integration gates still apply after this handoff.

## Reduced controls: separate causes

Toy losses per starting unit at light .4, height .6m, strength .35 [all C]:

| Cover/browse/protection | Vegetation loss | Browse loss | Remaining after separately recorded other loss |
|---|---:|---:|---:|
| Low/low/unprotected | 0.00000 | 0.00000 | 0.94000 |
| Low/high/unprotected | 0.00000 | 0.20000 | 0.75200 |
| High/low/unprotected | 0.23625 | 0.00000 | 0.71792 |
| High/high/unprotected | 0.23625 | 0.15275 | 0.57434 |
| High/high/protected | 0.23625 | 0.00000 | 0.71792 |

High cover here means both competitor fractions .9. These synthetic distinctions verify the proposed accounting semantics, not production outcomes; all three species and both expected-survival adapters share these numbers by design. Protection leaves vegetation loss intact.

| Form | Bramble .5 / bracken0: vegetation loss | Bramble0 / bracken .5: vegetation loss |
|---|---:|---:|
| linear | 0.13125 | 0.13125 |
| saturating | 0.19190 | 0.19190 |
| threshold_saturation | 0.14772 | 0.14772 |
| type_weighted_saturation | 0.14772 | 0.08606 |

The type-weighted form is no longer a renamed scalar-cover response: it produces unequal equal-cover losses. That distinction adds an unsupported ranking, so it is not selected. More complex forms have not demonstrated an empirical or management advantage.
