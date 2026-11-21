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

Quick lookup: collection page = paper page + offset — **P1 +2, P2 +14, P3 +34, P4 +54, P5 +84, P6 +98**.
(P4 Table 1 is p. 12 in both by coincidence.)

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
| Frame-tree density rules of thumb | ~100/ha overall; ~60–80/ha broadleaf; ~100–150/ha conifer; ~50/ha some transformation contexts; ~10–15 m indicative spacing | [P1:3–4] | TRANS | Assembled from review literature; conditional on objectives, crowns, site. **Not an Irish target** — an Irish measured trial value of ~250 future parent/frame trees ha⁻¹ (Brownhill) sits in the Practice composite §3–§4; the two are not reconciled and both must be labelled |
| Local release classes (Abetz & Klädtke; Welsh Sitka example) | 0–2 weak / 2–3 moderate / 3+ heavy neighbours | [P1:5] | TRANS/CAL | Boundaries overlap; not a species-wide rule |
| Frame-tree h/d realisation space (illustrated) | h/d ≈ 40–80, ideal line 60 | [P1:5] | TRANS | Illustration, not a stability threshold — see cross-reference |
| A-thinning index | uses tree height, target and competitor diameters, separation, intensity factor A; discrete A = 4–8 described as decreasing intensity | [P1:5] | METH | **NOT VERIFIED** by this producer and not checked by #896: the A = 4–8 range and its direction were taken from the extraction and must be confirmed on the rendered equation page before reuse |

### Human-selection finding — experimental evidence, but the agreement level was still low

- 26 British frame-tree selection **experiments**; typically 6–20 raters per group; 83–323 trees assessed per
  experiment. [P1:7; OBS]
- Participants **agreed more on which trees to retain than on which neighbours to fell as competitors**; low
  thinning elicited more agreement than the unfamiliar frame-tree crown thinning. Differences reported at
  p < 0.001. The authors conclude competitor recognition deserves more training. [P1:8–11; OBS/TRANS]
- **Absolute levels (P1:9).** Kappa was highest for low thinning (fair), then frame-tree selection (slight), lowest
  for competitors (slight). Frame-tree selection had no poor case and 13 in the fair class; the authors state that
  "in all cases agreement was low and did not exceed one case of moderate agreement". The relative ordering is the
  finding; the level was low throughout.

### Limits

British sample; objective-specific rules and h/d thresholds draw on broader literature; no calibrated
Sitka–Oak–Beech Irish competition or release coefficient; marteloscopes are discussed as training, not as proof
that a real harvesting operation follows any staged Work Plan. [P1:3, 7, 11; GAP]

## 4. Tree-marking agreement and training (P2)

- **Design.** 36 British marteloscope experiments at 12 sites; plots 0.1 ha each **except Ae at 0.133 ha** (P1:7);
  individual tree DBH (cm), height (m) and x/y recorded. 19 groups of raters, 9–20 people each, same binary
  mark/no-mark choice for every tree; ~95% employed by state forest organisations. [P2:1, 4–6; OBS]
- **Agreement statistic.** Fleiss' kappa (κ_F) among others. **An agreement statistic is not an ecological quality
  measure**; matching choices do not by themselves prove sustainable thinning. [P2:7–10; METH]
- **Results.** Low thinning κ_F "generally between 0.35 and 0.51 (fair to moderate)"; crown thinning "between 0.01
  and 0.28 (poor to slight)" with one exception, Loch Ard 2015 at κ_F = 0.44 (fair) [P2:14]. Overall mostly
  slight-to-fair [P2:1]. Fig. 7 counts: poor 4, slight 14, fair 17, moderate 1 — **no experiment reached substantial
  or almost perfect** [P2:16]; "4 of 36 poor" is also stated in prose [P2:17]. Low-vs-crown differences significant,
  p < 0.001 [P2:11]. [OBS]
- **Structure and conformity.** No forest characteristic was significantly related to κ_F. A **weak positive
  correlation** of the mean conformity number c̄ with the coefficient of variation of stem diameters (v_d) and with
  skewness (k_d) led the authors to suggest that conformity increases with increasing complexity of the stem-diameter
  structure [P2:15]. This is correlational, on low-diversity plantation sites (Table 2 v_d spans only 0.24–0.37;
  P2:6 notes stem-size diversity "is rather low, which is typical of plantations"). [OBS]

### Published agreement categories (P2 Table 3, PDF p. 9)

| F interval | Authors' category |
|---|---|
| < 0.10 | poor |
| 0.10–0.33 | slight |
| 0.33–0.50 | fair |
| 0.50–0.67 | moderate |
| 0.67–0.90 | substantial (not reached) |
| ≥ 0.90 | almost perfect (not reached) |

A research scale for study data. It is not a player score and must not be presented as one.

### Observed inventories (P2 Table 2; British descriptors, not Irish norms)

| Site / species | Stocking (trees/ha) | Basal area (m²/ha) | Mean quadratic DBH (cm) | Top height (m) |
|---|---|---|---|---|
| Ae / Sitka spruce | 1,336 | 41.9 | 20.1 | 21.1 |

Verified against the original ([P2:6]). **Cross-paper discrepancy:** P1 Table 1 ([P1:7]) lists the same site as
Ae, Sitka spruce, **1,321** trees/ha, G 41.9, d_g 20.1, **h₁₀₀ 21.2 m**, n = 176, and records the Ae marteloscope
as **0.133 ha** rather than 0.1 ha. Different measurement year and/or plot area; **neither is "the" Ae
inventory**, and this document does not reconcile them. Table 2 contains further sites; if more rows are needed,
transcribe them from the rendered table, not from plain-text extraction.

### Treated-stem descriptor (P2:10; METH)

B ratio = share of selected stem count ÷ share of selected basal area. B > 1 indicates smaller stems selected
(from below); B < 1 tends toward fewer, larger trees (crown thinning). A description of a marked treatment, not a
good/bad judgement.

### Use and limits

- **Producer inference, not a P2 finding.** A training design that tests *treatment intent* — future-crop
  retention, acceptable removal pressure, crown release, regeneration protection, work access — and gives reasoned
  feedback on controversial choices would fit P2:17, which asks whether exact tree marking is even necessary and
  argues that similar-but-different selections avoid "putting all eggs in one basket". P2 does not test any such
  design. [P2:16–17; DESIGN — producer inference]
- The paper records real operator variation. It contains **no statement** about automation, algorithms or delegated
  marking; P2:2 notes only that machine/harvester operators select trees in some countries. Any claim about a
  staged proposal-and-approval workflow is the project's, not this paper's. [P2:2; GAP]

## 5. Structural indices for read-only measurement (P3)

The lowest-risk set in the six papers: computed from existing species, DBH and coordinates, with no ecological
state change.

| Index | Definition | Page | Class |
|---|---|---|---|
| Species mingling M_i | For focal tree i, proportion of its n nearest neighbours of a different species; worked form uses n = 3 (values 0, 1/3, 2/3, 1); stand mean summarises local mixture | [P3:4–5] | METH |
| Diameter differentiation T_ij | 1 − min(DBH_i, DBH_j)/max(DBH_i, DBH_j); published descriptive classes are left-closed: 0 ≤ T < 0.3, 0.3 ≤ T < 0.5, 0.5 ≤ T < 0.7, 0.7 ≤ T ≤ 1.0 | [P3:5] | METH |
| Clark–Evans R | observed mean nearest-neighbour distance ÷ expected under a homogeneous random process (≈1/(2√λ), λ = stems/area); >1 regular, <1 clustered, ≈1 random-like | [P3:3] | METH |
| Contagion / angle index W_i | fraction of angles between neighbouring stems below the regular reference 360°/n (four neighbours ⇒ 90°) | [P3:3] | METH |
| Shannon H' | −Σ p_j ln p_j over species proportions; composition diversity only, ignores where species occur | [P3:4] | METH |
| Pielou segregation S | nearest-neighbour species-pair contingency measure, two species; positive = like-with-like | [P3:5] | METH |
| Pair correlation g(r) | relative frequency of stem pairs at distance r versus a chosen random reference; >1 extra pairs at r | [P3:6–7] | METH |
| Mark correlation | relation of a tree attribute (e.g. DBH) conditioned on pair distance r | [P3:7, 17] | METH |

These are definitions and their published worked forms. **This composite makes no recommendation about which, if
any, belongs in the game**; product decisions are recorded elsewhere and are not derived here.

### Two warnings that must travel with any index

1. **Edge and window bias.** Neighbours outside a bounded plot may be missing; small windows bias spatial indices;
   the paper states no single correction works for every index/plot shape. A physical estate boundary is not a
   sample-window edge, and invented off-map stems are not observed neighbours. [P3:8; METH/GAP]
2. **Randomness as a reference, not as a definition of nature.** The paper uses randomness "as a reference" and
   notes the criticism that a random reference "may not correspond to a particular biological meaning", that
   naturalness is "difficult, if not impossible, to define"; it concludes that randomness "as a reference is no
   substitute for naturalness", that deviation from randomness indicates "something special has been detected", and
   that further follow-up studies are needed to relate structure to habitat functions. [P3:8, 17; GAP]

Worked illustration (derived from the formula, not an observation): a pure Sitka plantation can hold many stems
with M ≈ 0 for most internal neighbourhoods; adding a few Oak/Beech increases species richness while local mingling
stays low if the broadleaves form a tight group, and dispersed broadleaf stems can raise local mingling. The formula
says nothing about whether either arrangement regenerates better. [P3:4–5; DESIGN — illustration only]

## 6. Crown geometry and plasticity — a research model, not a growth source (P4)

- **Scope.** Projects individual canopy positions and crown diameters with spatial interaction, crown growth,
  mortality, recruitment and possible crown-centre movement. Not a Sitka/Oak/Beech growth-yield model. [P4:2, 5–11;
  METH]
- **Data.** 5,591 spatial tree records (crown radii, total heights, coordinates). The source says they "were
  collected in a total of 17 plots **at each of** the five sites at Aarburg, Concise, Embrach, Zofingen and
  Zollikon" — which reads either as 17 plots in total or as 17 per site. **The interpretation is unresolved and is
  not resolved here**; do not quote "17 plots in total" as fact. 4–9 remeasurements, average ~7 years; largely
  European beech. [P4:12; OBS/TRANS] — *quoted value confirmed; interpretation open*
- **Fitting.** Potential crown-diameter growth multiplier fitted by upper-quantile regression at τ = 0.975; a
  local shot-noise interaction field adds neighbours' influence with crown-size-dependent kernels; competition
  modifies potential growth. [P4:6–8, 12–13; METH]

### Table 1 parameters (P4:12) — grouped exactly as the article groups them

| Group | Symbol = value | Meaning |
|---|---|---|
| Crown potential | k = 1.1287; q = −0.0387 | power function for the potential growth multiplier vs crown diameter (Eq. 2) |
| Interaction kernel | α = 0.2478; β = 1.6060; δ = 0.0747 | shot-noise kernel strength and ranges (Eq. 3) |
| Interaction scaling | ν = 96.3815 | standardising the interaction function (Eq. 5) |
| Height relationship | a = 0.5697; b = 9.1091 | total tree height estimation (Eq. 8) |
| Mortality | c₀ = 1.0200; c₁ = 0.0200 (Eq. 10); d = 0.0060 (no equation cited) | mortality threshold parameters |
| Recruitment | **λ_B** = 0.0390; f = 0.0900 | annual birth intensity; birth rate per m² of crown projection area |
| Crown movement | R = 0.4000 m | maximum annual displacement of a crown centre |
| Superorganism switch | S = 1.3000 | (crown radius + neighbour crown radius) ÷ distance must exceed S (Eq. 12) |

**Only k, q and the three kernel parameters were fitted** to the Swiss data. The article states the remaining
parameters "were freely determined" so that the model would simulate continuous forest development. This is not one
uniformly calibrated table. [P4:12–13; TRANS/CAL]

> Correction note: an earlier #883 draft wrongly reported these groups as shifted by one. That was a plain-text
> extraction artefact, not a source defect; the grouping above matches the rendered extraction and rendered article
> table, as independently confirmed by #896. See `03-PRIMARY-VERIFICATION-LOG.md` V-4.

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

- **Sample.** 18 plots/stands **as stated** by the source (abstract; §2.1; P5:4 "the 18 plots altogether"), 40 m ×
  40 m, southern Finland, long-term selective-harvesting study, 1991–2016, typically every fifth growing season.
  **The source is internally inconsistent**: P5:3 also says "the 20 plots" and "selective thinning was repeated in
  16 stands … four stands were not thinned" (16 + 4 = 20). The discrepancy is **unresolved** and 18 must not be
  cited as a settled sample size. Norway spruce ~75% of basal area; minority species pooled because counts were
  small. [P5:3–4; OBS]
- **Growth observations.** 17,153 observations from 5,206 trees; mean annual diameter growth 2 mm/yr,
  interquartile range 0.7–2.9 mm/yr. [P5:4; OBS] — *confirmed*
- **Mortality records.** 18,423 live/dead observations from 5,635 trees; 1,206 deaths. Where a cause is attributed
  it is mostly wind and snow, then competition and harvesting damage; **more than half of the causes of death
  remained unknown**. [P5:4, 11; OBS — Valkonen et al. 2020a/2020b as cited by the source]
- **Ingrowth.** 3,360 trees reaching or surpassing 1.3 m; on average 40 ingrowth trees per measurement time and
  sample plot, **or 49 per hectare and year** — a figure the source states directly. Ingrowth is defined as entering
  the ≥1.3 m population — not germination, not seed rain. [P5:4; OBS/TRANS]
- **Competition field.** Relascope basal area (RBA) index with basal-area factor 2; the field can be evaluated at
  any x/y, so it can also drive ingrowth location. [P5:4, Eq. 1–2; METH]
- **Model structure.** Spatially explicit Poisson ingrowth on a log scale (competition field, site index,
  temperature sum, plot random effect); growth as potential increment × competition response + residual, fitted by
  component; probabilistic annual mortality whose observed causes are only partly attributed, so the mortality
  association is not a causal taxonomy. [P5:4–5, Eq. 3–8, 7, 10–11; METH/GAP]
- **Uncertainty.** Posterior draws represent parameter and random-effect uncertainty; 95% **global envelopes** for
  whole stand-summary series, which is stronger than independent yearly error bars. [P5:5–9; METH]

### Consequential numbers

| Quantity | Value | Page | Class | Limit |
|---|---|---|---|---|
| Hegyi variants and Gaussian kernels vs the chosen RBA index | "showed a performance similar to the selected RBA with very little difference"; RBA chosen partly for computational reasons; no numbers reported | [P5:11] | OBS/TRANS | A qualitative one-sentence comparison in one Finnish fit. It says nothing about CCF's Hegyi treatment and supports neither keeping nor replacing it |
| Research thinning scenario | cut to 12 m²/ha when BA exceeds 22 m²/ha, checked every five years | [P5:6–9] | CAL | Finnish recommendation; not an Irish prescription |
| Predicted BA after 15 yr, plot VES01, mean site index | 23.5 (19.8–25.9); 26.8 (22.7–29.5); 32.3 (27.7–36.0) m²/ha at plot effect −1.96σ / 0 / +1.96σ. At zero plot effect the site-index spread (min/mean/max) is 25.1 / 26.8 / 30.5 m²/ha | [P5:9] | CAL | Model output illustrating site-effect uncertainty. The plot-effect variation is equivalent to roughly an 11–21 m site-index range |
| Example 100-yr cumulative volume, plot LAP07 | ~500–1,800 m³/ha (effects unknown) vs ~1,300–1,900 m³/ha (plot effects known) | [P5:9] | CAL | Demonstrates widening uncertainty; not an Irish forecast |
| Plot effects vs site index | plot effects can be materially more influential than a conventional site-index covariate | [P5:11–12] | OBS | Supports honest provenance |

### Limits

Finnish Norway-spruce-dominated selection forest; pooled minor-species fit; uncertain causes of death; simplified
research thinning operator. This does **not** qualify an Irish Sitka–Oak–Beech replacement growth system and does
not justify changing frozen save or reference behaviour. [P5:3–4, 9–12; GAP]

## 8. Regeneration-method taxonomy (P6)

- **Three situations.** *Shelter* — regeneration beneath retained mature canopy, where overhead protection competes
  with light. *Margin / forest edge* — regeneration in a zone beside mature forest, with mainly lateral protection
  and an environmental gradient; illustrative edge zones of 5–30 m, not an Irish universal range. **Bare land** —
  the paper's own term (the extraction renders it as "open"): regeneration away from mature-tree influence, in a
  gap/strip centre or a clearcut; high light coupled to exposure and ground-vegetation competition, so bare land
  does not automatically mean successful recruitment. [P6:2–3; METH/TRANS]
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

**Safeguards — producer inference, not the paper's finding.** P6's *regeneration strip* is a managed removal area
for creating cohorts at forest margins; the producer's caution is that it should not be conflated with an equipment
extraction rack. **This distinction is not made in P6**: the paper's own figures draw extraction racks *inside*
strip, wedge and group systems (Fig. 3B, Fig. 5). The game's rack and staging concepts are accepted design
(`research-index.md`, D-064/D-065) and are not derived from this paper. Do not implement automatic system selection
or a fixed regeneration recipe. [P6:3–4, 7, 9; DESIGN — producer inference]

**Sitka mentions (qualitative; no rate).** P6 does mention Sitka in contexts relevant to a Sitka transformation
project: clearcutting in Ireland followed by natural regeneration of *Picea sitchensis*, and a Welsh group-strip
shelterwood example (Cefn Du, Clocaenog). These are qualitative statements; P6 gives **no establishment rate,
probability or stocking figure** for Sitka. Recorded as candidate matrix row `P6-12`. [P6:6, 9; METH/TRANS]

**Limit.** A conceptual review. It contains no Irish establishment probabilities for Sitka, Oak or Beech.
[P6:1–5, 10–11; GAP]

## 9. Cross-references — where NOT to duplicate

| Topic | Existing home | Relationship to this document |
|---|---|---|
| Individual-tree judgement, CCF as management behaviour, marteloscopes as exercises | Practice composite §3 | Practitioner statements; P1/P2 add the *experimental* human-behaviour evidence |
| Irish measured frame-tree density (~250 future parent/frame trees ha⁻¹, Brownhill) | Practice §3–§4 | A measured Irish trial value. P1's ~100/ha is a literature rule of thumb; the two are **not reconciled** and neither is an Irish target |
| h/d used as a stability indicator | Practice §2; Growth §6 (windthrow and stability) | Different purpose from P1's frame-tree h/d realisation space; keep separate |
| Crown width/DBH envelopes, Welsh crown models | Growth §8 (Tabbush & White 1988; Davies & Pommerening 2008) | Real measured crown evidence; P4 is a simulation framework, not a substitute |
| Hegyi competition mechanism and its pooled-coefficient caution | Growth §5 (competition multiplier); Growth §8; Ecology §2 | The mechanism already exists. P5's relascope index is an alternative field, not a replacement |
| Density, site productivity, oak/beech growth envelopes | Growth §1–§7 | Irish/GROWFOR family; P5 is a Finnish method, not an input |
| Light, canopy gaps, seed supply, understorey | Ecology §2–§4 | Irish ecology; P6 supplies vocabulary only |
| Deadwood, veterans, biodiversity monitoring | Ecology §6, §10 | Irish and habitat-focused; P3 indices are not biodiversity measures |
| Marking conventions, rack planning, delegation boundaries | research-index (D-064, D-065); `RacksAndTreeMarking` extraction | Accepted design and field conventions; P1/P2 do not license a workflow |

> **Licence/provenance — publication held.** P4 and P6 are published under **CC BY-NC-ND**; P3 is © Institute of
> Chartered Foresters with no open-licence statement found on the pages checked. This document transcribes Table 1
> (15 numeric values) and paraphrases substantial parts of P3/P4/P6. Whether that may be distributed as a Project
> attachment is an **#425 / owner licensing decision, not decided here**; no redistribution right is inferred. If
> that decision disfavours the transcription, the fallback is reference-only treatment (signpost the paper and page
> rather than reproduce values).

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
