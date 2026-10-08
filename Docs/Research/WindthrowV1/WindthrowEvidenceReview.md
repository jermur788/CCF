# Windthrow evidence review

**Status:** supporting evidence for design; not decision authority.
**Method:** existing project reports first (no new literature search in this task).

## Labels

| Label | Meaning |
|---|---|
| **[A]** | Empirical evidence (measured or fitted), as reported by a project source |
| **[B]** | Transferable evidence: guidance, modelling, or data from another region or species |
| **[C]** | Calibration / gameplay choice |
| **[I]** | Inference or general forestry knowledge **not verified against a source in this task**. Verify before using it as calibration |

## Sources used

| Key | Source (project copy) | Role |
|---|---|---|
| IRL | *Irish_Sitka_CCF_Deep_Research_Report* §11–14, §19–22; cites the Teagasc Windrisk project and the Teagasc windthrow/resilience note | Primary wind source |
| ATL | *Atlantic Temperate Rainforest Under CCF — Game-Simulation Mechanisms*, "Windthrow, stochastic disturbance and salvage" | Disturbance and salvage framing; cites Forestry Commission CCF guidance, UKFS, ForestGALES |
| MAC | Macdonald, Gardiner & Mason (2010), *Forestry* 83 (PDF in project archive) | Thinning, wind loading, timber quality |
| PRAC | European Pro Silva CCF Practice Synthesis (6 Oct 2026), Ireland §11.2–11.4, Finland §15 | Practitioner guidance |
| GM1 | `Docs/Research/SitkaGrowthMortality/SitkaGrowthModel1.md` + evidence CSV | Current simulated height and H/D |
| PLS | CCF Primary Literature Synthesis v1 | No windthrow content; crown envelope only |

## Findings by topic

### Sitka wind vulnerability and storm disturbance

- **[A] IRL:** "The Irish windthrow model identified top height, regional wind exposure, soil, ground preparation and thinning status as major predictors." Historical fitted coefficients show "particularly strong positive risk effects for gley and peat relative to brown earth."
- **[A] IRL — the key constraint:** the published Irish equation "predicts whether more than roughly 3 % of stems in comparable stands are windthrown by a specified top height. **It is not an annual individual-tree probability** and should not be pasted directly into `ForestTree.windthrowChance`." **This design does not convert it.** It can inform only the *direction* of stand-level effects (height, soil, exposure, thinning) and a qualitative sanity check (see `LongRunCalibrationPlan.md`).
- **[B] ATL:** Forestry Commission CCF guidance identifies windthrow as one of the main constraints on CCF adoption. ForestGALES exists to compare wind-damage risk under management alternatives.
- **[B] ATL:** "Exact stand-level wind responses are site- and species-specific and should therefore be parameterized rather than encoded as 'thinning always causes windthrow'."

### Storm frequency and severity

- **[B/C] IRL:** "Do not translate uncertain projections of future Irish storm frequency directly into deterministic gameplay. Storm frequency and severity are better treated as scenario parameters."
- **[C] ATL:** "No defensible universal annual storm probability follows from the cited evidence. For game prototyping, 0–5 % annual event probability and severity 0–1 can be exposed as scenario settings and must be identified as game priors, not ecological estimates."
- **Consequence:** storm probability and severity in this design are **[C] scenario parameters**, labelled as such everywhere.

### Height

- **[A] IRL:** top height is a major predictor; "the Irish empirical evidence supports a continuous increase in risk with stand height". Use "critical height as a management warning concept rather than a universal biological threshold".
- **[I]** Very short trees within a closed canopy carry little wind load. A ramp from near zero at low heights is a shape choice, not a measured threshold.
- **[A/C] GM1:** simulated top height 14.6 m (Year 0, age 20) → 20.9 m (age 30) → 24.1 m (age 40) → 31 m (age 120), following the Irish Class III anchor at age 30.

### Slenderness (H/D)

- **[B] IRL:** "slenderness_i = height_i / DBH_i" as the local tree modifier. "Dense crops can develop small crowns, long slender stems and weaker individual rooting architecture. Repeated earlier moderate thinning can therefore improve individual form even as any single thinning event temporarily increases exposure."
- **[B] MAC:** thinning focused on final-crop trees tends to favour tapered trees with low H/D ratios "to minimize the risk of wind or snow damage".
- **[PRAC] Ireland:** poor transformation candidates include "later-stage stands with high height:diameter ratios and small/receded crowns". Deep crowns are valuable for vigour and stability.
- **[I] (also noted by Sol in GM1):** H/D above roughly 80–100 is commonly associated with wind instability in Sitka. **No threshold is adopted**; H/D enters as a continuous factor.
- **[A/C] GM1:** dominant H/D 72 (Year 0) → about 83 (age 30–40) → 70 (age 60) → 43 (age 120). Suppressed H/D stays higher. **The most vulnerable period in simulation terms is Scenario Years 10–30.**

### Thinning, recent exposure and gaps

- **[A] IRL:** thinning status is a predictor. "In older dense stands, heavy canopy opening can trigger severe wind damage."
- **[B] IRL (Teagasc guidance):** gradual interventions, "ideally removing no more than about 20 % of basal area per intervention". **Management guidance, not an ecological threshold. Not hard-coded.**
- **[B] IRL:** "sudden concentrated thinning of a tall, previously sheltered crop can yield timber and growing space immediately while temporarily increasing exposure and storm risk."
- **[B] MAC:** heavy thinning increases wind loading on the remaining trees, which responds with compression wood and taper. Some stands are designated "no thin" owing to extreme vulnerability.
- **[I]** Post-thinning vulnerability is greatest soon after the intervention and declines over several years as trees acclimate. The existing 3-year half-life of `RecentOpening` is a **[C]** representation of that idea, not a measured rate.
- **[I]** Long-established edges are generally windfirmer than newly created edges.
- **[B] IRL / ATL:** very large openings increase wind exposure for edge trees, and a gap can be both a regeneration opportunity and a wind risk.

### Soil and site

- **[A] IRL:** gley and peat increase risk strongly relative to brown earth. Productivity and stability must be modelled separately: "Some wet or gley sites can support rapid Sitka growth while simultaneously producing shallow rooting and high wind vulnerability."
- **[REPO]** Scenario One has no soil type. `SoilStability` = 1 everywhere and is not saved. **PRODUCT DECISION** (`DecisionMatrix.md`): Scenario One soil class for storm hazard. Recommendation: *mineral, moderate stability*, consistent with Class III average productivity. It is a scenario label, not a measured site.

### Group stability

- **[PRAC] Ireland:** "closely grown bio groups can share rooting/crown behaviour and may be more stable if treated as a unit". **[PRAC] Finland:** dense groups are thinned gradually rather than made uniformly open.
- **Design use [C]:** mutual sheltering is represented implicitly through dominance (relative height) and low local openness. **No explicit group variable in v1.**

### Uprooting vs snapping

- **[I]** Trees on wet or shallow-rooting soils tend to overturn (uproot); well-anchored trees more often break. Both occur in Sitka storms.
- **[B] ATL:** "uprooted/snapped trees" listed as visible outcomes.
- **Design use:** v1 has one state, *uprooted*, because the root-plate assets exist and the default soil choice favours overturning. Snapping is deferred (`WindthrowOutcomeDesign.md`).

### Salvage and deadwood

- **[B] ATL:** UKFS recognises windblown trees as potential sources of deadwood and structural microsites. Storms make "the decision particularly sharp: natural disturbance can suddenly generate both an economic salvage opportunity and a pulse of habitat."
- **[B] ATL:** the player may "salvage different fractions after a storm"; retaining windblown trees is a legitimate choice. Safety and pest considerations can restrict retention (UKFS).
- **[B] MAC:** gap-edge trees are predicted to have poorer timber quality (taper, branches, compression wood).
- **[I]** Windblown timber value falls with delay (stain, decay) and breakage. The rate is a **[C]** calibration.

### Annual order

- **[B] IRL** (recommended simulation order): "1. Apply management actions and felling. **2. Resolve storms and other discrete disturbances.** 3. Rebuild local neighbour competition. 4. Recalculate canopy and light…"

## What must not be done

1. Convert the Irish stand-level equation (> 3 % of stems by a top height) into an annual per-tree probability.
2. Hard-code Teagasc's ≈ 20 % basal-area guidance as a damage threshold.
3. Present storm frequency or severity as measured Irish constants.
4. Show a numeric per-tree windthrow chance.
5. Use H/D 80 or 100 as a hard cut-off.

## Evidence gaps (for later research tasks)

- Irish Sitka storm-return statistics suitable for scenario presets.
- The Irish windthrow model's published coefficients (source paper not in the project archive). Retrieve them to build a *stand-level* calibration check.
- Acclimation time after thinning for Sitka: quantitative sources.
- Uprooting/breakage proportions by soil.
- Salvage value-loss rates in Irish markets.
