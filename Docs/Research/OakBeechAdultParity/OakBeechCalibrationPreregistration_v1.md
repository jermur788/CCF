# Oak/Beech calibration preregistration v1

Base `c35e0016fe28b6302bb4e120642f3b917b14cb67` (N2 research design). Locked before any candidate run. No production calibration, fitting, Assets edits or Unity. Existing Kernel A is adequate for **coarse ordinal calibration only**, not precise empirical growth prediction.

## Evidence and interpretation

N2 corrects the Beech diameter/height column error: Slovak crop-group Dq and mean height are not Irish individual DBH or top height. No doubled-height response is accepted. The provisional Oak height 20–25/30–35 m at age50/100 and Beech 25–30/35–40 m are [C] design plausibility diagnostics, not empirical gates. No measured DBH-age bands are authorised. Beech-before-Oak canopy timing and comparative late-growth longevity remain deferred paired-species claims. Only positive diameter response to release for each species is scored as supported direction. Site numerical multiplier is **1.0 only**; production cell productivity is not assumed uniform by this document.

## Locked exploratory ranges

Every row below is **[C] CALIBRATION SEARCH RANGE**, not an evidence coefficient, measured parameter, or Irish empirical value. These broad exploration bounds include existing .4/.4/120/40/3 calibration and vary rate, size brake and competition independently. Width is a tooling choice, not confidence interval.

| Species | Parameter | [C] CALIBRATION SEARCH RANGE | Units | Deterministic grid |
|---|---|---|---|---|
| Oak | Pd | 0.4–1.2 | cm/year | 0.4, 0.8, 1.2 |
| Oak | Ph | 0.4–0.9 | m/year | 0.4, 0.65, 0.9 |
| Oak | Dmax | 80–160 | cm | 80, 120, 160 |
| Oak | Hmax | 35–50 | m | 35, 42.5, 50 |
| Oak | Ci50 | 2–8 | Hegyi index | 2, 5, 8 |
| Beech | Pd | 0.4–1.4 | cm/year | 0.4, 0.9, 1.4 |
| Beech | Ph | 0.4–1.1 | m/year | 0.4, 0.75, 1.1 |
| Beech | Dmax | 80–160 | cm | 80, 120, 160 |
| Beech | Hmax | 35–50 | m | 35, 42.5, 50 |
| Beech | Ci50 | 3–10 | Hegyi index | 3, 6.5, 10 |

Cartesian grid: 243 candidates/species, 486 total, ordered species then Pd/Ph/Dmax/Hmax/Ci50. Do not narrow ranges after seeing output in this experiment. Baseline .4/.4/120/40/3 is a separate diagnostic control, not an optimum. Persist all failed candidates and reasons; any later changed scoring/ranges require a new preregistration.

## Faithful kernel and fixtures

Inspect source GrowAdults in ForestEcologyController, TreeSpeciesDefinition, ForestTree.ApplyGrowth and JuvenileEcologyRules. For an existing broadleaf tree each annual step is:

`ΔD = Pd * site * clamp01(1-D/Dmax) / (1+CI/Ci50)`

`ΔH = Ph * site * clamp01(1-H/Hmax)`

Apply the production storage bounds D=clamp(D+ΔD,1,200), H=clamp(H+ΔH,.5,60); age advances by one. Check actual source bounds before porting. Binary floating-point Python reproduction is mathematical, not a C#/Unity float/hash equivalence claim. Confirm recurrence against its closed form where brakes are unsaturated; never rebaseline a Sitka anchor.

Juvenile checkpoints: ages **5/10/20/30/40/50**. Controlled surviving stock starts age0, height.15 m; full-light no-browse/no-mortality height growth uses shipped .35 m/year, promotes at ≥3.5 m with D=clamp(height*1.5,2,20), then follows generic adult-tree storage kernel. Promotion is a representation boundary, not biological maturity. Keep an additional pure generic-tree control starting D2/H.5 age0 for isolating parameter effects; do not call it a planted sapling. Report both trajectories and distinguish from actual planted stock/light/browse history. Adult fixtures: start age50, D20 or40, H15 or25 (four controlled starting states); checkpoints **50/75/100/125/150**. Do not infer that these illustrative initial sizes were observed in Ireland.

Management fixture CI: dense8, moderately released3, strongly released1. Juveniles use the same full-light establishment control; after promotion they inherit fixed CI. Adult treatment starts at50; no invented effect on height. Include release-at50 continuation from the lifetime dense trajectory to separate early history from late treatment. No numerical site variation (site=1). No density mortality, recruitment, neighbour/canopy feedback, browsing or finances; this isolates growth, never compensates for density defects.

## Scoring priorities (lexicographic)

1. No pathological trajectory: finite, non-negative growth; bounded sizes; no overshoot/decline from states below maxima; no extreme early saturation. Retain ratios and ceiling hits.
2. Species ordering only where evidence supports it: no mandatory cross-species height/DBH ranking, no guessed Beech canopy speed or paired lifespan requirement.
3. Beech diameter responds positively to release (strong>moderate>dense by age100 in each adult fixture).
4. Oak diameter responds positively to release under the same test.
5. No invented doubled height response: treatments must give identical heights for matched initial state/age/site.
6. New plantings retain juvenile/young size at ordinary Scenario One timescales: predeclared [C] guard D<40 at age25 and age tracked separately; new stock is not relabelled as a 50-year adult on promotion. This does not require remaining in the aggregated juvenile representation for25 years; actual juvenile establishment/light/browse remains a separate Unity gate.
7. Premium Oak normally remains beyond ordinary completion for new stock: report first D≥60 cm under each lifetime/release fixture through age200, require no such proxy before age80 in this control. **60 cm is a labelled [C] standing-tree screening proxy**, not an assortment boundary or a conversion of mid-log diameter. Production premium eligibility additionally needs log taper/length, pruning and defects; no age embargo for existing old trees.
8. No compensation for density-model defects: fixed CI/site, no SDI/occupancy fitting or changing N1 parameters.

After these hard guards, report multi-age height-band distance for [C] design bands at50/100 over all three treatments (lifetime surviving-stock control). Report DBH trajectories without fabricated empirical bands. Distance is sum of distance outside each height interval divided by its interval width, averaged over ages; it is diagnostic ranking only. Never use one endpoint as the selection criterion. Report all bounded survivors, lowest-distance families and joint parameter sensitivity; do not pick a unique production value.

## Predeclared rejection conditions

Reject nonfinite values, negative increment, unexplained decline/overshoot, nonpositive parameters, storage clamp hits in tested bounds, D or H ≥95% of respective maximum before age50 in lifetime control, failure of positive release ordering at100 in any matched adult fixture, treatment-induced height divergence >1e-10 m, new-stock age25 D≥40 cm, or Oak proxy reaching60 cm before80. Reject any tool run using site≠1, altered density/Sitka code or changed ranges/scoring. A zero-survivor grid is C for this bounded experiment, not proof every real kernel variant is impossible. Provisional height-band miss is recorded, not falsely converted into an empirical rejection.

## Classification and next experiment

A: bounded families survive with a discriminating justified target set. B: kernel works directionally but many materially different families remain weakly identifiable, or only provisional height targets discriminate. C: preregistered bounded grid has no family satisfying hard targets simultaneously. A/B → Manager-approved bounded production-calibration comparison at multiple ages, initial states and release conditions after density U3; do not adopt coefficients or Model2 by this lab. Save/anchor/Unity/runtime qualification remains separate.
