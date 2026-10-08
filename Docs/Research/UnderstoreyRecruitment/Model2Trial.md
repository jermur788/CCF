# Save 18 / regeneration 2 integrated trial

Manager approval: `018e2ec6-b25e-45ee-b560-308b529a8df3`, delivered 7 October 2026. Branch `task/understorey-recruitment-causality`, owned checkout `/media/jer/ZX20/CCF-understorey-recruitment`, context/base `a8596df9c52669a36709f09e85dfe6568640af49`. Earlier accepted audit `ed960b7` and continuation `7370ea0` remain historical evidence, not model-2 measurements. No main merge or canonical acceptance is claimed.

## What is implemented

Exactly five approved saved fields: cell `brambleCover`/`brackenCover`, patch `brambleCover`/`brackenCover`/`competitionUpdatedYear`. Independent horizontal fractions may overlap and do not sum to one. Cell `lastUpdatedYear` advances habitat and competitor state together. No generic grasses, shrubs, ferns, moss, fungi or rendered instance are converted into a botanical competitor population.

New Scenario One starts save18/model2. Historical v17 model1 retains model1; missing historical model fields retain existing legacy policy; Reference Future remains model0. A model2 save needs all explicit JSON fields, a complete unique grid in cell-index order, finite covers in [0,1], and nonnegative timestamps no later than authoritative ecological year. Invalid state is rejected before world changes; duplicate local geometry/year keys are rejected. Parsed in-memory state has range/timestamp checks; disk JSON also checks presence and numeric types, because JsonUtility defaults missing numbers to zero. Existing Newtonsoft package is reused, with no dependency change.

Capture of a freshly reset model2 scenario ensures its complete initial grid before serialization, so a save made before the first annual step or map access can reload. Existing valid grids are preserved; malformed loaded grids are rejected rather than rebuilt. This correction is model2-only.

The trial profile is intentionally visible in `UnderstoreyCompetitionCalibration` and is not additional saved ecology state. **Schema approval does not approve these coefficients or the initial cover rule. This branch executes a trial and is not approved for production integration.** Manager selection remains required if the outcomes or evidence cannot distinguish candidate profiles.

## Explicit candidate rule and evidence limits

`site = clamp01(siteProductivity × soilStability)`; `light = clamp01(cellLight)`; shared bramble/bracken target = `.8 × site × (.1 + .9 × light)`; initial cover = `.25 × target` for each category. Both categories are assumed possible at this Scenario One site [I]. The shared target avoids inventing a species ranking, while retaining independent mutable fields and local histories. Product/ramp/floor/max/initial fraction are [C], not measured vegetation cover. This is neither a fitted species establishment model nor a general habitat suitability map.

Primary evidence supports competition, site context and release after canopy change, but does not select a transferable quantitative initial cover rule. Bramble can survive deep shade: the shade floor is a candidate, and closed canopy is not an absence rule. See [Forest Research regeneration guidance](https://www.forestresearch.gov.uk/research/lowland-native-woodlands/natural-regeneration-of-broadleaved-trees-and-shrubs/), [FC information note 275](https://cdn.forestresearch.gov.uk/2022/02/rin275.pdf), [Harmer & Morgan 2007](https://doi.org/10.1093/forestry/cpm006), and [Bramble and regeneration under shade, Forestry 2013](https://doi.org/10.1093/forestry/cps066). Transfer of field oak results to shared Sitka/Oak/Beech annual coefficients would be unsupported. The audit evidence grades remain unchanged.

Annual cover recovery is the existing bounded recurrence shape: lerp toward annual target using gain `.30` when rising, loss `.45` otherwise [D/C]. These are competitor candidates; previous habitat measurements are not bramble/bracken observations. The actual model2 matrix varies gain/loss `.15/.25`, `.30/.45`, `.45/.65`, strength `.15/.35/.60`, escape height `1/1.5/2.5m`, and initial fraction `.1/.25/.5`. Separate runtime recurrence tests cover closed/moderate/open light and independent rising/falling cover.

Survival loss probability = `.35 × exposure × clamp01((1.5 − pre-growth height)/(1.5 − .3))`. Full vulnerability at ≤.3m and zero at ≥1.5m are calibration choices. `exposure` is max(local bramble, local bracken), bounded in [0,1]; no type ranking, light interaction, generic habitat or shelter immunity is added. Shared Sitka/Oak/Beech response applies to natural and exact juveniles after establishment. Growth calibration, seed rain and requested establishment equations are unchanged. Survival can indirectly free existing capacity; that is not an extra establishment multiplier.

## Spatial and annual semantics

Local patches override background. Most recent `createdYear` wins; equal years use ascending `(x,z,radius)` with greatest key winning. Equal keys are invalid save records. This does not depend on list order. A later completed whole-cell treatment masks an older patch only inside that cell; its cross-border portion remains valid outside. The mask is derived from existing saved completed work orders, with no sixth persistent field.

Natural exposure integrates **max at each point**, rather than max of mean covers. Existing 2cm vertical-strip accuracy is reused: circle interval events are clipped to each cell, and an ordered active set chooses one winning patch on overlapping area. The unpatched area uses background. No second area discount is applied. Cells hold local patch buckets; exact juvenile queries use a balanced bounding tree with precedence pruning, avoiding a juvenile × global-patch nested scan. Cache rebuild occurs once per annual step; clearing invalidates it. Worst-case geometric query traversal can still approach a local bucket scan and is disclosed in measured stress tests.

Annual order: paid work resolves; canopy changes; competitor/habitat recovery; vegetation survival loss at pre-growth height; existing juvenile growth/browse/light calculation; establishment; natural promotion; exact juvenile vegetation then existing growth/browse/light survival and promotion. For the natural population ledger, deaths are attributed **vegetation → light → browse**; exact deaths use vegetation roll followed by the unchanged light/browse survival roll, with light failures classified first. Browse-event growth remains the existing expected-versus-realised treatment. Diagnostic accounts separate starting, vegetation, light, browse, remaining, promotion and accepted establishment. Relative natural abundance is not an exact physical stem count and must not be priced as nursery stock.

Planting retains its paid stock/work/shelter rules and local preparation: local cover starts zero at treatment, then recovers. Area clearance removes the same qualifying cohorts and planted juveniles as before, and zeros background competitors. Consequently preparing an area before already prepared planting does not confer an extra planting immunity. Existing juvenile removal can outweigh future survival relief. Shelters affect browsing only and expire on their existing schedule. Prices are unchanged.

Existing bramble/bracken visual categories now read the independent fields at their actual positions, including patch cover. No art asset, material, existing GUID or Claude-owned UI changes. New helper scripts retain their own .meta files. Other habitat types retain their prior display state. Recognizability still needs player review; numeric biology is not inferred from art. The earlier Beech renderer-height normalization is retained.

## Reproduction and review

Close Unity and run from this owned checkout, one launcher at a time:

```bash
python3 Tools/Verification/UnderstoreyRecruitment/run_model2.py
python3 Tools/Verification/UnderstoreyRecruitment/run_model2.py --gate Model2Matrix
python3 Tools/Verification/UnderstoreyRecruitment/run_model2.py --gate Model2TargetedEconomy
python3 Tools/Verification/UnderstoreyRecruitment/run_model2.py --gate Model2Performance
python3 Tools/Verification/UnderstoreyRecruitment/run_regression.py
CCF_REGEN_MODEL=1 python3 Tools/Verification/UnderstoreyRecruitment/run_regression.py --gate ScenarioOneCompletionVerification
```

The launcher stages one diagnostic source/meta and removes it after Editor exit, uses the existing isolated configuration and shared exclusive launch lock, two workers and 8G/256M resource limits. Model2 results live under `Build/UnderstoreyRecruitment/Model2`; useful reviewed evidence is copied under `Evidence/Model2`. Full regression runs the actual new-game default, while the explicitly forced historical completion has separate outputs. Historical audit and continuation evidence are retained unchanged. Actual save-size comparison serializes the same world then removes just the five fields, isolating schema overhead; annual timing uses paired fresh simulation-only populations and reports variability, not graphical frame rate.

Final results, gameplay/economy assessment, anchors, measured overhead, unresolved parameter choices and branch delivery status belong to `Model2Handoff.md`. Exact conditional P1 wording belongs to `TutorialHandoff.md`; UI owner must integrate it before a public model2 release.
