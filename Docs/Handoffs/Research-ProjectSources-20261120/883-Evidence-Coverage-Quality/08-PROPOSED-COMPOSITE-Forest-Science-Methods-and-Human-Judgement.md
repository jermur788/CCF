# PROPOSED — CCF Forest Science Methods and Human Judgement Evidence Composite

> **STATUS: PROPOSED DRAFT — NOT CANONICAL, NOT REVIEWED, NOT ATTACHED, NOT A DECISION.**
> Created under #883 Stage C repair as a *candidate* project-facing supporting composite. It has not been
> approved by anyone, is not attached to the CCF Game Dev project, is not on `main`, and does not replace or
> amend any existing composite. The Decision Log and #425 remain authoritative.
> **Supporting research — not decision authority.**
>
> **Basis.** Condensed from the existing 24-page extraction
> (`CCF_Six_Papers_Relevance_Evidence_Extraction.pdf`, 11 Nov 2026), which indexes six complete papers held in
> `CCF_Six_Complete_Forestry_Research_Papers.pdf` (111 pp). This document adds no new literature research. Where
> a figure is unverified or the extraction is qualified, that is carried forward rather than smoothed over.

## 1. How to use this document

Every entry carries: the source paper ID, the **original paper page**, the evidence class, and the transfer limit.
`[P3:5]` means paper P3, page 5 of that paper's supplied PDF. Collection pages in the 111-page companion: P1
3–14, P2 15–34, P3 35–54, P4 55–84, P5 85–98, P6 99–111.

Evidence classes: **OBS** direct observation; **METH** published method/definition; **TRANS** non-Irish or
non-target-species result applied conceptually; **CAL** calibration/scenario assumption; **DESIGN** inference for
CCF; **GAP** missing or unresolved evidence.

**Authorities this document does not override:** accepted project decisions and specs; the four existing
composites (which hold the Irish ecology, browsing, growth/density and practice/economics evidence);
`research-index.md`; `GROWFOR_Technical_Evidence_Extraction_v1.md`.

## 2. Source manifest

| ID | Citation | Original pages | Collection pp. | Geography / species | Licence note |
|---|---|---|---|---|---|
| P1 | Pommerening, Maleki & Haufe (2021). Tamm Review: Individual-based forest management or Seeing the trees for the forest. *For. Ecol. Manage.* 501:119677. doi:10.1016/j.foreco.2021.119677 | 12 | 3–14 | Great Britain; planted even-aged stands, mostly Sitka spruce | CC BY |
| P2 | Pommerening, Pallarés Ramos, Kędziora, Haufe & Stoyan (2018). Rating experiments in forestry: How much agreement is there in tree marking? *PLOS ONE* 13(3):e0194747. doi:10.1371/journal.pone.0194747 | 20 | 15–34 | Britain; 12 sites, mostly Sitka plantations, one beech | CC BY |
| P3 | Pommerening (2002). Approaches to quantifying forest structures. *Forestry* 75(3):305–324. doi:10.1093/forestry/75.3.305 | 20 | 35–54 | European illustrative stands; methodological review | © Institute of Chartered Foresters |
| P4 | Pommerening, Gaulton, Magdon & Myllymäki (2021). CanopyShotNoise. *Int. J. Remote Sens.* 42(18):6837–6865. doi:10.1080/01431161.2021.1944695 | 30 | 55–84 | Switzerland; European beech | CC BY-NC-ND |
| P5 | Myllymäki, Kuronen, Bianchi, Pommerening & Mehtätalo (2024). A Bayesian approach to projecting forest dynamics and related uncertainty. *Ecol. Model.* 491:110669. doi:10.1016/j.ecolmodel.2024.110669 | 14 | 85–98 | southern Finland; Norway-spruce-dominated selection forest | CC BY |
| P6 | Pommerening, Szmyt & Duchiron (2024). Revisiting silvicultural systems. *Trees, Forests and People* 17:100597. doi:10.1016/j.tfp.2024.100597 | 13 | 99–111 | international review; Polish example | CC BY-NC-ND |

None of these six is an Irish study. None supplies an Irish coefficient. Rights are as published; this
condensation grants no new distribution or derivative rights.

## 3. Individual-tree management and frame trees (P1)

- **Framework.** The unit of reasoning is a retained *frame tree* (also final crop tree, target tree, elite tree)
  and its immediate neighbourhood, not a uniform stand percentage. A frame tree may be chosen for commercial,
  ecological, conservation or recreational purpose. Not every surrounding tree is a competitor. [P1:1–4; METH/TRANS]
- **Sequence.** Select the frame tree first on vigour and objective-relevant qualities; relate it to nearby
  candidate frame trees; then judge whether neighbours threaten its crown or function **within the next 5–10
  years**. The horizon is review practice, not an Irish thinning-cycle rule. [P1:1–3; METH]
- **Minimal removal.** One frame tree may need none removed, another one, two or three. The remaining stand
  serves protection, habitat, timber, seed supply and local conditions. Blanket crown release is not implied.
  [P1:2, 4; METH]
- **Local ≠ global thinning.** Low thinning takes mainly lower-canopy stems; crown thinning removes
  dominant-canopy competitors to favour selected trees. A local treatment is not adequately described by stems/ha
  removed. [P1:4–5; METH]
- **Longitudinal value.** The same retained trees can be re-assessed after later interventions; diameter
  increment, crown space, h/d trajectory and stem integrity carry management-history information. [P1:5–6; METH]

### Quantitative extracts — examples only, never targets

| Quantity | Value | Page | Class | Limit |
|---|---|---|---|---|
| Frame-tree density rules of thumb | ~100/ha overall; ~60–80/ha broadleaf; ~100–150/ha conifer; ~50/ha some transformation contexts; ~10–15 m indicative spacing | [P1:3–4] | TRANS | Assembled from review literature; conditional on objectives, crowns, site |
| Local release classes (Abetz & Klädtke; Welsh Sitka example) | 0–2 weak / 2–3 moderate / 3+ heavy neighbours | [P1:5] | TRANS/CAL | Boundaries overlap; not a species-wide rule |
| Frame-tree h/d realisation space (illustrated) | h/d ≈ 40–80, ideal line 60 | [P1:5] | TRANS | Illustration, not a stability threshold — see cross-reference |
| A-thinning index | uses tree height, target and competitor diameters, separation, intensity factor A; discrete A = 4–8 decreases in intensity | [P1:5] | METH | Benchmark only until unit/boundary checks |

### Human-selection finding — the strongest teaching evidence in the set

- 26 British frame-tree selection **experiments**; typically 6–20 raters per group; 83–323 trees assessed per
  experiment. [P1:7; OBS]
- Participants **agreed more on which trees to retain than on which neighbours to fell as competitors**; low
  thinning elicited more agreement than the unfamiliar frame-tree crown thinning. Differences reported at
  p < 0.001. The authors conclude competitor recognition deserves more training. [P1:8–11; OBS/TRANS]

### Limits

British sample; objective-specific rules and h/d thresholds draw on broader literature; no calibrated
Sitka–Oak–Beech Irish competition or release coefficient; marteloscopes are discussed as training, not as proof
that a real harvesting operation follows any staged Work Plan. [P1:3, 7, 11; GAP]

## 4. Tree-marking agreement and training (P2)

- **Design.** 36 British marteloscope experiments at 12 sites; 0.1 ha each; individual tree DBH (cm), height (m)
  and x/y recorded. 19 groups of raters, 9–20 people each, same binary mark/no-mark choice for every tree; ~95%
  employed by state forest organisations. [P2:1, 4–6; OBS]
- **Agreement statistic.** Fleiss' kappa (F) among others. **An agreement statistic is not an ecological quality
  measure**; matching choices do not by themselves prove sustainable thinning. [P2:7–10; METH]
- **Results.** Poor-to-fair agreement in crown thinning; fair-to-moderate in low thinning; overall mostly
  slight-to-fair; 4 of 36 experiments in the poor category; **no experiment reached substantial or almost
  perfect**. Low-vs-crown differences significant, reported p < 0.001. [P2:11, 16–17; OBS]
- **No structural explanation.** No significant relationship of F with the surveyed structural characteristics; a
  different conformity metric showed weaker relationships with diameter diversity/skewness. This does not support
  "complex forests cause agreement" *or* "complex forests cause disagreement". [P2:15–17; OBS]

### Published agreement categories (P2 Table 3, PDF p. 9)

| F interval | Authors' category |
|---|---|
| < 0.10 | poor |
| 0.10–0.33 | slight |
| 0.33–0.50 | fair |
| 0.50–0.67 | moderate |
| 0.67–0.90 | substantial (not reached) |
| > 0.90 | almost perfect (not reached) |

A research scale for study data. It is not a player score and must not be presented as one.

### Observed inventories (P2 Table 2; British descriptors, not Irish norms)

| Site / species | Stocking (trees/ha) | Basal area (m²/ha) | Mean quadratic DBH (cm) | Top height (m) |
|---|---|---|---|---|
| Ae / Sitka spruce | 1,336 | 41.9 | 20.1 | 21.1 |

Verified against the original. Table 2 contains further sites; transcribe from the rendered table if more rows
are needed, not from plain-text extraction.

### Treated-stem descriptor (P2:10; METH)

B ratio = share of selected stem count ÷ share of selected basal area. B > 1 indicates smaller stems selected
(from below); B < 1 tends toward fewer, larger trees (crown thinning). A description of a marked treatment, not a
good/bad judgement.

### Use and limits

- Training should test **treatment intent** — future-crop retention, acceptable removal pressure, crown release,
  regeneration protection, work access — and give reasoned feedback on controversial choices. It should not
  penalise a different but defensible marking because it differs from one expert's tree IDs. [P2:3, 16–17; DESIGN]
- The paper records real operator variation; it does **not** show that an automated procedure can safely replace
  independent judgement. A proposed treatment stays a proposal. [P2:2–4, 17; TRANS/DESIGN]

## 5. Structural indices for read-only measurement (P3)

The lowest-risk set in the six papers: computed from existing species, DBH and coordinates, with no ecological
state change.

| Index | Definition | Page | Class | Handling |
|---|---|---|---|---|
| Species mingling M_i | For focal tree i, proportion of its n nearest neighbours of a different species; worked form uses n = 3 (values 0, 1/3, 2/3, 1); stand mean summarises local mixture | [P3:4–5] | METH | Read-only candidate; show beside species proportions |
| Diameter differentiation T_ij | 1 − min(DBH_i, DBH_j)/max(DBH_i, DBH_j); published descriptive classes <0.3, 0.3–0.5, 0.5–0.7, >0.7 | [P3:5] | METH | Read-only candidate; distinguishes size mixture from diameter variance |
| Clark–Evans R | observed mean nearest-neighbour distance ÷ expected under a homogeneous random process (≈1/(2√λ), λ = stems/area); >1 regular, <1 clustered, ≈1 random-like | [P3:3] | METH | Optional; keep separated from mixing |
| Contagion / angle index W_i | fraction of angles between neighbouring stems below the regular reference 360°/n (four neighbours ⇒ 90°) | [P3:3] | METH | Offline research chart; edge correction required |
| Shannon H' | −Σ p_j ln p_j over species proportions; composition diversity only, ignores where species occur | [P3:4] | METH | Optional advanced |
| Pielou segregation S | nearest-neighbour species-pair contingency measure, two species; positive = like-with-like | [P3:5] | METH | Two-species diagnostic only |
| Pair correlation g(r) | relative frequency of stem pairs at distance r versus a chosen random reference; >1 extra pairs at r | [P3:6–7] | METH | Advanced research |
| Mark correlation | relation of a tree attribute (e.g. DBH) conditioned on pair distance r | [P3:7, 17] | METH | Advanced research |

### Two warnings that must travel with any index

1. **Edge and window bias.** Neighbours outside a bounded plot may be missing; small windows bias spatial indices;
   the paper states no single correction works for every index/plot shape. A physical estate boundary is not a
   sample-window edge, and invented off-map stems are not observed neighbours. [P3:8; METH/GAP]
2. **Randomness ≠ naturalness.** Randomness is a statistical reference, not a synonym for naturalness, health,
   resilience or biodiversity. The paper states the biological relationship between structural indices and habitat
   variables still needed validation. A structural score must not become a biodiversity objective. [P3:8, 17; GAP]

Worked illustration (derived from the formula, not an observation): a pure Sitka plantation can hold many stems
with M ≈ 0 for most internal neighbourhoods; adding a few Oak/Beech increases species richness while local mingling
stays low if the broadleaves form a tight group, and dispersed broadleaf stems can raise mingling without
improving recruitment. [P3:4–5; DESIGN]

## 6. Crown geometry and plasticity — a research model, not a growth source (P4)

- **Scope.** Projects individual canopy positions and crown diameters with spatial interaction, crown growth,
  mortality, recruitment and possible crown-centre movement. Not a Sitka/Oak/Beech growth-yield model. [P4:2, 5–11;
  METH]
- **Data.** 5,591 spatial tree records (crown radii, total heights, coordinates) from 17 plots at five Swiss sites;
  4–9 remeasurements, average ~7 years; largely European beech. [P4:12; OBS/TRANS] — *verified at source*
- **Fitting.** Potential crown-diameter growth multiplier fitted by upper-quantile regression at τ = 0.975; a
  local shot-noise interaction field adds neighbours' influence with crown-size-dependent kernels; competition
  modifies potential growth. [P4:6–8, 12–13; METH]

### Table 1 parameters (P4:12) — grouped exactly as the article groups them

| Group | Symbol = value | Meaning |
|---|---|---|
| Crown potential | k = 1.1287; q = −0.0387 | power function for the potential growth multiplier vs crown diameter (Eq. 2) |
| Interaction kernel | 0.2478; 1.6060; 0.0747 | shot-noise kernel strength and ranges (Eq. 3) |
| Interaction scaling | 96.3815 | standardising the interaction function (Eq. 5) |
| Height relationship | a = 0.5697; b = 9.1091 | total tree height estimation (Eq. 8) |
| Mortality | c0 = 1.0200; c1 = 0.0200; d = 0.0060 | mortality threshold parameters (Eq. 10) |
| Recruitment | B = 0.0390; f = 0.0900 | annual birth intensity; birth rate per m² of crown projection area |
| Crown movement | R = 0.4000 m | maximum annual displacement of a crown centre |
| Superorganism switch | S = 1.3000 | (crown radius + neighbour crown radius) ÷ distance must exceed S (Eq. 12) |

**Only k, q and the three kernel parameters were fitted** to the Swiss data. The article states the remaining
parameters were freely determined so that the model would simulate continuous forest. This is not one uniformly
calibrated table. [P4:12; TRANS/CAL]

> Correction note: an earlier #883 draft wrongly reported these groups as shifted by one. That was a artefact of
> reading plain-text extraction rather than rendered pages; the grouping above matches both the rendered
> extraction and the rendered article table. See `03-PRIMARY-VERIFICATION-LOG.md` V-4.

- **Experimental settings.** Window 100 m × 100 m; initial Poisson intensity 0.025 points/m²; four combinations
  C0S0 / C1S0 / C0S1 / C1S1 (crown plasticity off/on; superorganism mechanism off/on); ensembles of 500
  simulations. [P4:14, 17–23; CAL]
- **Simulated finding.** The superorganism scenario produced clusters with more compact crown packing and larger
  gaps; crown plasticity alone produced a weaker increase in regularity and little change in gap dynamics. These
  are model-scenario outputs, **not** evidence of root sharing, facilitation, or Irish gap trajectories.
  [P4:23–25; OBS of simulation / GAP]

**Excluded by the source's own framing:** any automatic superorganism mechanic; the R = 0.4 m crown drift as a
game rule; Swiss beech constants as Irish Oak/Beech values; free-standing birth probabilities. [P4:12–14, 24–25;
GAP/DESIGN]

## 7. Spatial growth and forecast uncertainty (P5)

- **Sample.** 18 analysed plots (figure as extracted; see the V-9 qualification in
  `03-PRIMARY-VERIFICATION-LOG.md`), 40 m × 40 m, southern Finland, long-term selective-harvesting study,
  1991–2016, typically every fifth growing season. Norway spruce ~75% of basal area; minority species pooled
  because counts were small. [P5:3–4; OBS]
- **Growth observations.** 17,153 observations from 5,206 trees; mean annual diameter growth 2 mm/yr,
  interquartile range 0.7–2.9 mm/yr. [P5:4; OBS] — *verified at source*
- **Mortality records.** 18,423 live/dead observations from 5,635 trees; 1,206 deaths, mostly wind and snow,
  then competition and harvesting damage. [P5:4; OBS] — *verified at source*
- **Ingrowth.** 3,360 trees reaching or surpassing 1.3 m; the extraction quotes ~49 trees/ha/year. Ingrowth is
  defined as entering the ≥1.3 m population — not germination, not seed rain. [P5:4; OBS/TRANS]
- **Competition field.** Relascope basal area (RBA) index with basal-area factor 2; the field can be evaluated at
  any x/y, so it can also drive ingrowth location. [P5:4, Eq. 1–2; METH]
- **Model structure.** Spatially explicit Poisson ingrowth on a log scale (competition field, site index,
  temperature sum, plot random effect); growth as potential increment × competition response + residual, fitted by
  component; probabilistic annual mortality. The cause of every observed death is unknown, so the mortality
  association is not a causal taxonomy. [P5:4–5, Eq. 3–8, 7, 10–11; METH/GAP]
- **Uncertainty.** Posterior draws represent parameter and random-effect uncertainty; 95% **global envelopes** for
  whole stand-summary series, which is stronger than independent yearly error bars. [P5:5–9; METH]

### Consequential numbers

| Quantity | Value | Page | Class | Limit |
|---|---|---|---|---|
| Hegyi variants and Gaussian kernels vs the chosen RBA index | performed similarly in exploratory analysis; computational convenience influenced the final choice | [P5:11] | OBS/TRANS | This is the strongest published argument for *benchmarking*, not replacing, an existing Hegyi treatment |
| Research thinning scenario | cut to 12 m²/ha when BA exceeds 22 m²/ha, checked every five years | [P5:6–9] | CAL | Finnish recommendation; not an Irish prescription |
| Predicted BA after 15 yr, one plot | 23.5 (19.8–25.9); 26.8 (22.7–29.5); 32.3 (27.7–36.0) m²/ha across negative/zero/positive plot-effect cases | [P5:9] | CAL | Model output illustrating site-effect uncertainty |
| Example 100-yr cumulative volume, plot LAP07 | ~500–1,800 m³/ha (effects unknown) vs ~1,300–1,900 m³/ha (plot effects known) | [P5:9] | CAL | Demonstrates widening uncertainty; not an Irish forecast |
| Plot effects vs site index | plot effects can be materially more influential than a conventional site-index covariate | [P5:11–12] | OBS | Supports honest provenance |

### Limits

Finnish Norway-spruce-dominated selection forest; pooled minor-species fit; uncertain causes of death; simplified
research thinning operator. This does **not** qualify an Irish Sitka–Oak–Beech replacement growth system and does
not justify changing frozen save or reference behaviour. [P5:3–4, 9–12; GAP]

## 8. Regeneration-method taxonomy (P6)

- **Three situations.** *Shelter* — regeneration beneath retained mature canopy, where overhead protection competes
  with light. *Margin / forest edge* — regeneration in a zone beside mature forest, with mainly lateral protection
  and an environmental gradient; illustrative edge zones of 5–30 m, not an Irish universal range. *Open* — away
  from mature-tree influence, in a gap/strip centre or a clearcut; high light coupled to exposure and ground-vegetation
  competition, so open does not automatically mean successful recruitment. [P6:2–3; METH/TRANS]
- **Four basic systems.** Clearcut-replanting (classification only, not a recommendation); uniform shelterwood
  (repeated distributed openings under retained overstorey, eventual removal); strip system (localised progressive
  strips emphasising the edge principle); group system (small and/or enlarging gaps, where gap margin and open
  centre are different environments). [P6:2–4; METH]
- **Two specialised systems.** Selection/plenter — local interventions with a persistent mixture of sizes and
  development stages rather than a finite wave of overstorey removal. Two-storeyed high forest — a sustained
  upper/lower storey pairing, distinct from a temporary shelterwood phase. [P6:5; METH]
- **Time and purpose.** Preparatory, seeding/establishment, secondary and final/removal fellings describe different
  purposes within staged regeneration; a single fell does not guarantee adequate new woodland. [P6:4; METH]
- **Variants vs combinations.** Variants are small modifications of one system; combinations mix components across
  space or time; Tables 1–3 enumerate the possibilities. [P6:6–11; METH]
- **Terminology.** *Transformation* is gradual change of structure and/or species towards CCF; *conversion* is
  often more abrupt species change in restoration. A distinction used in this paper, not a universal Irish legal
  definition. [P6:2; METH]
- **Planting.** Partial and enrichment/gap planting are compatible with CCF when natural regeneration is
  insufficient or a new species is wanted. This supports offering options; it gives planting no automatic
  ecological credit. [P6:6–7; METH]
- **Illustrative figures.** Static gaps of 0.05–0.5 ha in one Polish classification example; such figures
  illustrate systems and are not thresholds for an Irish stand or rack diagram. Large gaps make edges ecologically
  important but raise wind and microclimate risk; the authors discuss trade-offs, not a universal optimum.
  [P6:3–4, 10–11; TRANS]

**Safeguards.** P6's *regeneration strip* is a managed removal area for creating cohorts at forest margins — it is
not an equipment extraction rack. The game's rack and staging concepts are accepted design (see `research-index.md`,
D-064/D-065) and are not derived from this paper. Do not implement automatic system selection or a fixed
regeneration recipe. [P6:3–4; DESIGN]

**Limit.** A conceptual review. It contains no Irish establishment probabilities for Sitka, Oak or Beech.
[P6:1–5, 10–11; GAP]

## 9. Cross-references — where NOT to duplicate

| Topic | Existing home | Relationship to this document |
|---|---|---|
| Individual-tree judgement, CCF as management behaviour, marteloscopes as exercises | Practice composite §3 | Practitioner statements; P1/P2 add the *experimental* human-behaviour evidence |
| h/d used as a stability indicator | Practice §2; Growth §5 | Different purpose from P1's frame-tree h/d realisation space; keep separate |
| Crown width/DBH envelopes, Welsh crown models | Growth §8 (Tabbush & White 1988; Davies & Pommerening 2008) | Real measured crown evidence; P4 is a simulation framework, not a substitute |
| Density, site productivity, oak/beech growth envelopes | Growth §1–§7 | Irish/GROWFOR family; P5 is a Finnish method, not an input |
| Light, canopy gaps, seed supply, understorey | Ecology §2–§4 | Irish ecology; P6 supplies vocabulary only |
| Deadwood, veterans, biodiversity monitoring | Ecology §6, §10 | Irish and habitat-focused; P3 indices are not biodiversity measures |
| Marking conventions, rack planning, delegation boundaries | research-index (D-064, D-065); `RacksAndTreeMarking` extraction | Accepted design and field conventions; P1/P2 do not license a workflow |

## 10. What this evidence does not authorise

Restating the extraction's own exclusions, because they are the part most likely to be dropped:

- No Irish species-specific crown-plasticity rates; no Irish self-thinning or maximum-density envelopes for
  Oak/Beech; no validated Sitka–Oak–Beech density-equivalence coefficients.
- No Irish probabilities for seed production, establishment, deer damage or mortality.
- No optimal group-gap size for the Scenario One site; no universally correct sequence of Fell marks.
- No in-game below-ground physiological network because simulated crowns clustered.
- No calibration of CCF's current code, and no claim that these procedures match the Work Plan, delegated-proposal
  or rack-map interactions.
- **No gameplay coefficient, threshold, score or UI string.** A number appearing in a table here is not a
  calibration.

## 11. Retrieval aids

- Highest-value original pages: P1 pp. 2–5 (frame trees, local thinning, intensity, h/d), 7–11 (experiment
  methods and results); P2 p. 6 (12-site inventory), p. 9 (agreement categories), pp. 10–17 (comparisons,
  conclusions); P3 p. 2 (structural-variable classification), pp. 4–5 (neighbourhood diagram, formulas), p. 8
  (edge/reference caveats), p. 17 (biodiversity limits); P4 pp. 5–9 (field and canopy formulas), p. 12 (parameter
  table), pp. 13–14 (cycle and experimental setup), pp. 24–25 (findings); P5 pp. 3–4 (data, competition formulas),
  pp. 5–6 (model and scenarios), pp. 7–12 (validation, uncertainty); P6 pp. 2–5 (principles and systems),
  pp. 6–11 (variants, combinations, Polish examples).
- View the original figures and tables in the 111-page companion rather than trusting text extraction.
