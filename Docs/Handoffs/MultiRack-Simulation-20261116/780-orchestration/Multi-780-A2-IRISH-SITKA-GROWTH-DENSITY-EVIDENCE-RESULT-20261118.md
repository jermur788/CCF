# Multi #780 A2 — Irish Sitka growth, density, competition and mortality evidence envelope

**Status:** **DRAFT RESEARCH EVIDENCE — NOT A FORECAST — NO CALIBRATION ACCEPTANCE.**
This file accepts no coefficient, approves no calibration, makes no empirical prediction for any Irish stand, and turns no gameplay choice into evidence. Every "suggested envelope" in §10 is a conditional option for an owner or later calibration lane, never adopted.

| Field | Value |
| --- | --- |
| Task | [#780](https://github.com/jermur788/CCF/issues/780) (Multi independent quantitative-biology research), Manager [#730](https://github.com/jermur788/CCF/issues/730); worker role A2 forestry-evidence |
| Author | Claude Sonnet 5.5, exact model id `claude-sonnet-5-5`; provider "Claude A via T3 Code"; harness Claude Code CLI / Agent SDK; no subagents. Dispatched by a DeepSeek V4.1 Flash parent. Independent Claude Opus 5.5 review is requested by the parent; this author may not self-review. |
| Host / time | `jer-laptop`, Linux 7.0.0-42-generic x86_64; first observed 2026-11-18T20:15:08Z |
| Registration | [RUNNING comment, posted before any file write](https://github.com/jermur788/CCF/issues/780#issuecomment-6990359354) |
| Base | `origin/main` = `a69b3e2e90829505c792d95f5a3eba3e7d8d8132`, re-verified twice via `gh api` (20:15Z and 20:20Z); equals the brief's SHA, no change |
| Branch / single file | `docs/multi-780-a2-sonnet-irish-sitka-20261118-v1` / this one new Markdown only |
| Work class | Docs-only reading of local durable copies plus offline arithmetic (stdlib Python, Appendix B). No .NET, native, Unity, Host, save, product-source, Ops, main, fixture or protected #710/#725/P0/#687/#639 access. |

**Observed commit and checkout note.** The local primary checkout was at `46247486` with unrelated uncommitted files, so it is not `a69b3e2e`. For every source file in §0.1 I compared the git blob ID in the checkout against `origin/main:<path>` (= `a69b3e2e`); all eight are byte-identical. The sources were therefore read at the observed main revision. Nothing in the checkout was modified.

## 0. What was read

### 0.1 Files read (read-only)

| # | File | Sections read | Git blob (equal at `a69b3e2e`) |
| --- | --- | --- | --- |
| R1 | `Docs/Research/CCF_Growth_Density_and_Species_Quantitative_Evidence_Composite.md` (35,555 B; sha256 `2704b0aa…f73b`) | §§1–11 in full (manifest, §4 productivity, §5 individual tree, §§1–3 density/DEC/DMC, §8 crown, §9 GROWFOR boundary) | `44bb320a5a57ee10da5cc52973e82201041f44e7` |
| R2 | `Docs/Research/SitkaGrowthMortality/GROWFOR_Technical_Evidence_Extraction_v1.md` | §§1–19 in full (esp. §2A evidence roles, §§3–9, §11–§14) | `80a90530df15c5c17f3bbb544b36d6e991b45868` |
| R3 | `…/SitkaGrowthModel1.md` | whole file (growth model 1 record, long-run tables) | `44a6f1b95f6289c541244fa24cf1fa76e37f5b28` |
| R4 | `…/IrishHeightValidation.md` | whole file | `994c3b7257705821177b9bbaba9212f3c04e4227` |
| R5 | `…/StandDevelopmentBeforeAfter.md` | whole file (a *superseded candidate*, used only to identify what not to reuse) | `d33e2875592a9d079dafb033cb070758f189742b` |
| R6 | `…/AdultMortalityCalibration.md` | whole file | `f0bf586e3ee0d5464bb6801c6617568b61a41eaa` |
| R7 | `…/CurrentModelAudit.md` | whole file | `db3f4397ce4ac7d3e83c8507d2cbe044f0b594e8` |
| R8 | `…/EvidenceMatrix.csv` | all 26 rows | `f946618f50e6af7ec6c20bc208d4a78b67b3895e` |
| R9 | #737 DRAFT (raw, branch `docs/multi-737-b-spec-draft-20261118-v1`, tip `da37b23b…`; 44,375 B; sha256 `f9e64dd3…8ebc`) | whole file; rule values are in the paragraph after the §S4 table | n/a (unmerged branch) |
| R10 | #760 DRAFT (raw, branch `docs/multi-760-b-year2-decision-closure-result-20261118-v1`, tip `149b4c80…` at fetch; 39,363 B; sha256 `3952c9a5…6b8c`) | whole file | n/a (unmerged branch) |

### 0.2 Missing or not read

* `CCF_Ecology_and_Regeneration_Evidence_Composite.md` — **MISSING** (not in the checkout under `Docs/Research/` and not in the `origin/main` tree at `a69b3e2e`). No regeneration, recruitment, juvenile-light or browse evidence is stated or inferred here.
* **No primary paper or PDF was re-read.** The referenced PDFs (Lekwadi 2012, Broad & Lynch 2006a/b, McCullagh 2013/2017, Comeau et al. 2010, Tabbush & White 1988, Davies & Pommerening 2008, Jones et al. 2026, Conry & Clinch 1989, the Irish Sitka Deep Research Report, the C8 calibration report, the Frenchpark source) are not in the accessible material. Every primary-study number below is **cited via secondary synthesis; primary not re-read** (marked †).
* Not read (outside the brief): `Evidence/*.csv`, `AdultMortalityDesign.md`, `HeightCandidateComparison.md`, `Scenario1SiteProductivityDecision.md`, the unmerged P1 four-file spec, #725/#729/#732 reports (only as summarised inside R9/R10), `Docs/Research/RegenerationBudget`, `UnderstoreyRecruitment`, `WindthrowV1`.
* Protected material (#710/#725 source, P0 source, #687, #639 fixture bytes, fixture/CLI outputs) was neither read nor re-hashed. Multi-P1 stand numbers below come only from numbers *printed in R9/R10*.

## 1. Evidence taxonomy and legend

Every table row ends in a class code. A code has a **kind** and, for measurements, a **geography** tag.

| Code | Kind | Meaning in this file |
| --- | --- | --- |
| **P†** | PRIMARY EVIDENCE | A measured, fitted or tested result attributed to a named primary study. Because no primary paper was re-read, **every P is P†**: "cited via secondary synthesis; primary not re-read". |
| **S** | SECONDARY RESEARCH SYNTHESIS | A CCF-synthesis interpretation, classification, range, rule of thumb or inventory statement without a separately attributable primary number. |
| **M** | MATHEMATICAL DERIVATION | Computed from stated inputs by a stated formula (Appendix B reproduces all). Inherits the weakest input class; adds no new information. |
| **G** | GAMEPLAY-CALIBRATION PROPOSAL | A CCF model constant, engine rule, authored starting state or DRAFT proposal. Never evidence. |
| **U** | UNSUPPORTED-NEEDS-DECISION | No accessible support. Left as a gap; not filled. |

Geography: **IE** Irish; **GB** British; **EU** continental European; **CCF** project model/stand. "IRISH DIRECT" in R1 is a CCF label, not a verification by this file.

**Population boundary applied throughout.** The accessible Irish stand-level evidence (Lekwadi, Broad & Lynch, the NFI evaluation) describes **pure, even-aged Sitka under conventional (principally low) thinning or no thinning**. It does not describe irregular, mixed, multicohort or continuous-cover stands (R2 §2A.4, §14.2).

## 2. Cross-cutting QA flags found while reading

| ID | Flag | Consequence |
| --- | --- | --- |
| F-01 | R1 §2 reports "~55 m²/ha untreated BA (Jones et al. 2026)" as IRISH DIRECT, yet R1's manifest says that source has "mostly absent Irish data; transfer/modelled". Irish Frenchpark unthinned values (1,762 stems/ha, mean DBH 23.7 cm) imply BA ≥ 77.7 m²/ha (row D-06). | The 55 m²/ha figure's location, age, thinning and definition are unverified and cannot serve as an Irish maximum. Treated as S, geography **unverified**. |
| F-02 | Lekwadi Table 3 rounded coefficients miss the published age-30 anchors by −2.19 to −0.08 m and give 45–52 m asymptotes for classes I–II (R1 §4, R4; reproduced in S-03). | Use anchors as validation; do not freeze curves from rounded coefficients. |
| F-03 | The two Irish stand-level top-height descriptions (Lekwadi-form Class III vs the GROWFOR height equation at the same age-30 top height) agree at 30 yr by construction and diverge by +7.1 % at 40 yr, +18.4 % at 60 yr, +30.5 % at 100 yr (S-10). | Do not average. The top-height *definition* in each source is unverified (R4 flags "100 largest-DBH stems/ha" as unchecked against Lekwadi). |
| F-04 | R8 row "thinning response … moderate/heavy vs unthinned: observed 1.26/1.37" does not match R1's Frenchpark table (light 29.8, medium 31.1, heavy 32.4 vs unthinned 23.7 cm → 1.257 / 1.312 / 1.367). The "1.26" equals *light*, not moderate. | Label mismatch to resolve before any tree-level "Frenchpark ratio" assertion. |
| F-05 | Published MAI/CAI intersection for Class III is 0.75 m/yr; the anchor-matched Class III curve peaks at 0.71 m/yr (age 20) and the rounded one at 0.708 (S-05). | Consistent with F-02: curves are approximations. |
| F-06 | NFI Table 1 means cannot be combined: means of products ≠ products of means (mean diameter ≠ Dq). BA computed from mean stocking 1,662 and mean diameter 18.66 cm is 45.5 m²/ha against the reported mean BA 32.97. | Never combine NFI means into a "typical stand". |
| F-07 | R8 still carries superseded rows (maximum SDI "1500–2200 tested", grade I; "80–90 m²/ha plausible", grade I) beside the Comeau row. R6 states the unsourced 80–90 ceiling was dropped. R5 is a candidate with SDI 2,200 backstop "far too permissive". | Use only R6/R3 for the density line; do not cite R5 or the I-grade rows as evidence. |
| F-08 | The Multi-P1 draft engine uses "growth model 2" (R9, R10); R3 documents growth **model 1**. Whether model 2 shares model 1's height and DBH rules was not verified. | Rule-implication rows labelled G; "model 1 rule" explicitly named. |
| F-09 | Two different synthetic stands exist: Scenario One 40 × 40 m, 336 stems (R7) and Multi-P1 30 × 30 m, 100 stems (R9). | Not interchangeable; not mixed in any row. |
| F-10 | Lekwadi classes I–V cover age-30 top height 16.0–27.6 m; R1 §4 also quotes an age-30 site index range 4.5–28.9 m (Deep Research Report). | The class table does not span the whole quoted range; the mapping is not stated in accessible text. |

## 3. Irish Sitka site productivity and potential height

**Scope.** Stand-level top height of even-aged Sitka under conventional thinning; "yield class" in the FC sense (m³/ha/yr) is **not** supplied for Sitka in any accessible file, and Lekwadi's classes I–V are top-height classes, not FC yield classes. Rotation length is not stated in any accessible file (**U**).

| ID | Item (value, unit) | Population / site / age / condition | Class | Uncertainty and caveat |
| --- | --- | --- | --- | --- |
| S-01 | Lekwadi et al. 2012 dataset: 578 modelling plots; 4,190 repeatedly measured top-height trees; 11,421 paired top-height/age observations; independent validation set 1,120 paired observations (R1 §4) | Public plots across seven Irish forest regions plus private-forest data; Irish Sitka | P† IE | Top-height definition not restated in accessible text (R4). Planting vs total age basis not stated. |
| S-02 | Age-30 top heights: I 27.6 m (±2.4 %), II 23.3 m (±2.9 %), III 20.4 m (±2.8 %), IV 17.4 m (±2.9 %), V 16.0 m (±2.1 %) | Five Irish productivity classes (I very good to V very poor) | P† IE | The meaning of "±%" (relative SE, interval or error) is not stated in accessible text. |
| S-03 | Polymorphic Chapman–Richards `H(t)=b0(1−exp(−b2 t))^b3`, rounded Table 3 (b0 / b2 / b3, with the quoted parenthetical q): I 58.371 / 0.024 / 1.247 (q .99); II 49.523 / 0.025 / 1.247 (.95); III 34.228 / 0.042 / 1.563 (.50); IV 28.272 / 0.051 / 2.134 (.12); V 22.557 / 0.060 / 2.619 (.02) | Same | P† IE (rounded) | q is not explained in accessible text. **M** check: rounded H30 = 25.41 / 22.31 / 20.32 / 16.80 / 14.05 m, i.e. −2.19 / −0.99 / −0.08 / −0.60 / −1.95 m against S-02; rounded H100 = 51.8 / 44.5 / 33.4 / 27.9 / 22.4 m. Matches R4. Flag F-02. |
| S-04 | Anchor-matched Class III curve (b0 rescaled 34.228 → 34.362 so H30 = 20.4; b2, b3 kept): H20 14.21, H30 20.40, H40 24.89, H60 30.14, H80 32.51, H100 33.56, H120 34.01 m | Derived curve for site Class III | M (from P† IE) | **Modelled, not a measured Irish trajectory** (R3 says so); only 20.4 m at age 30 is the published anchor. Reproduces R4/R3. |
| S-05 | MAI/CAI intersections 0.89, 0.82, 0.75, 0.58, 0.45 m/yr (I–V); age of maximum mean annual top-height increment 17 yr (I) to 29 yr (V) | Irish classes | P† IE | **M** check on S-04: maximum MAI 0.710 m/yr at age 20 (integer search), not 0.75. Class III's own published age is not in accessible text. Flag F-05. |
| S-06 | First thinning about 15–17 yr (Class I) to about 25–29 yr (Class V) | Irish classes | P† IE | Management anchor, not a biological trigger (R1 §4). |
| S-07 | Age-30 site index range about 4.5 m (wet, nutrient-poor) to 28.9 m (very moist, nutrient-rich) | Irish Sitka, broader range (Deep Research Report) | S IE | Flag F-10. Source not re-read. |
| S-08 | Large site-related differences in Sitka yield class among brown earths, peaty gleys, peaty podzols and blanket peat; about 4,000 ha pre-1968 State forest inventory (Slieve Bloom); excludes very small sub-compartments and generally mixed stands unless one species exceeds 80% of the canopy | Conry & Clinch 1989; Irish State forest | P† IE (qualitative) | **No numeric Sitka yield classes quoted** in accessible text. Numeric Sitka YC ↔ site-index mapping: **U**. |
| S-09 | GROWFOR/Broad–Lynch height model: polymorphic Bertalanffy–Richards; site index = top height at age 30; a = 47.0216, c = 0.4571, t0 = −5.0 yr, H0 = 0 m; 749 trajectories from 225 plots; log-likelihood 1318.48 (polymorphic) vs 1261.87 (anamorphic) (R2 §5) | Irish PSP data, thinned and unthinned | P† IE | Version caveat (R2 §12): coefficients are *BroadLynch2006_Sitka*, not proven equal to the current executable. |
| S-10 | **Derived** GROWFOR height for S = 20.4 m at age 30: `b = −ln(1−(S/a)^c)/(30−t0)` = 0.03280; `H(t)=a(1−e^{−b(t−t0)})^{1/c}`. H20 13.20, H30 20.40, H40 26.65, H60 35.67, H80 40.92, H100 43.80 m. Versus S-04: −1.00 (−7.1 %), 0, +1.76 (+7.1 %), +5.53 (+18.4 %), +8.41 (+25.9 %), +10.24 (+30.5 %) m. Age 30→40 increment 4.49 m (S-04) vs 6.25 m | Same S as Class III anchor; extrapolation | M (from P† IE) | Both curves were fitted to even-aged plot data of limited age range; GROWFOR thinned PSPs start at first thinning, unthinned PSPs at about age 20, no early-growth data (R2 §11.1). Manual window: top height < 49.3 m, projections best from first thinning to clearfelling (R2 §11.1). Whether S in GROWFOR and Class III in Lekwadi describe the same measured quantity is unverified. Flag F-03. **No number between the two is offered.** |
| S-11 | Top height is a site surrogate largely independent of stocking and normal thinning intensity | Irish Sitka | P† IE (qualitative, via R1 §4) | Qualitative; no magnitude of the independence in accessible text. |
| S-12 | In the retained GROWFOR system site acts as a **time scaling** (`b`) of the whole B–N–H system; `C` independent of site (R2 §4.6) | Irish PSP | P† IE (model structure) | Equation structure only; no site-specific B or N trajectories transcribed in R2. |
| S-13 | Rotation length; FC yield class values for Sitka; thinning-cycle norms beyond S-06 | — | **U** | Not in accessible material. |

## 4. Density evidence

### 4.1 Three evidence families (kept separate)

1. **Measured Irish stand summaries** (D-01, D-02, D-03).
2. **Fitted Irish stand models** (D-04: GROWFOR B–N–H; no transcribed numeric B(N,Dq) table).
3. **Transferred values** (D-05 British maximum line; D-07 Reineke; D-08 continental).

| ID | Item (value, unit) | Population / condition | Class | Uncertainty and caveat |
| --- | --- | --- | --- | --- |
| D-01 | **NFI1 Sitka summary** (R2 §13.2, Table 1 of McCullagh 2017), n = 200 plots: age 22.73 yr (min 9.10, max 49.00, SD 9.44); mean diameter 18.66 cm (8.88–32.73, SD 5.00); BA 32.97 m²/ha (6.69–74.80, SD 12.78); mean height 11.93 m (4.91–30.80, SD 4.21); stand volume 201.30 m³/ha (11.93–1,010.60, SD 145.66); stocking 1,662 stems/ha (240–4,967, SD 911) | Irish NFI, single-species even-aged Sitka; 107 thinned, 93 no-thin; 183/200 contain ingrowth | P† IE | Top height was approximated (NFI does not record it); ingrowth distorted stocking; plot-size effects; SDs are **between-plot**, not within-stand. Flag F-06. |
| D-02 | **Frenchpark thinning trial after 29 growing seasons** (R1 §5): unthinned 1,762 stems/ha, mean DBH 23.7 cm, mean tree volume 0.44 m³; light 925, 29.8 cm, 0.70 m³; medium 761, 31.1 cm, 0.76 m³; heavy 616, 32.4 cm, 0.85 m³ | Irish thinning experiment (Frenchpark) | P† IE | Whether "29 growing seasons" counts from planting or from first treatment is not stated. Site productivity, planting density, thinning type/timing and removed volumes are not in accessible text (**U**). Treatment labels "light/medium/heavy" are as quoted. |
| D-03 | "Plot basal area climbed to ~55 m²/ha in untreated stands" (Jones et al. 2026, long-term Sitka thinning trial) | Location unverified | S (geography unverified) | Flag F-01. R1 labels it IRISH DIRECT; its manifest contradicts that. Not used as a maximum. |
| D-04 | GROWFOR/Broad–Lynch state-space B, N, H with separate thinned and unthinned systems; fitted on 1,330 thinned and 540 unthinned growth trajectories; volume equation on 727 observations; thinning transition on 459 observations (R2 §6.3, §8.3, §9.4) | Irish PSPs, even-aged, principally low thinning | P† IE | Coefficient matrices are printed in R2 §6 but **not reproduced or run here**. No transcribed B(t) or N(t) numeric trajectories. N is observed stocking; mortality is embedded, not parameterised (§7). |
| D-05 | British maximum size–density line (Comeau, White, Kerr & Hale 2010, *Forestry* 83:461–468): slope of log N on log Dq = −2.063 (steeper than Reineke −1.605); maximum SDI = 1,868; `SDI = N·(Dq/25)^2.063`; `RD = SDI/1868` | British Sitka, Forestry Commission data; paper compares Britain and Canada | P† GB (abstract level) | R6: slope, SDImax and the 25 cm reference are "externally confirmed sufficiently"; the intercept is derived from max SDI at 25 cm and "check against the full paper". The paper reports regional differences and recommends region-specific lines. Sample size, fitting method and site-shift detail not in accessible text. **No Irish validation.** The line is a limiting boundary, not a typical stand. |
| D-06 | **Derived BA on the British line** (formula in Appendix B). At RD = 1: Dq 15 cm → 5,359 stems/ha, BA 94.7; 20 → 2,960, 93.0; 25 → 1,868, 91.7; 30 → 1,282, 90.6; 40 → 708, 89.0; 50 → 447, 87.8; 60 → 307, 86.8 m²/ha. At RD = 0.6 (the CCF onset value): BA 56.8 (Dq 15), 55.8 (20), 55.0 (25), 54.4 (30), 53.4 (40), 52.7 (50), 52.1 (60) m²/ha | Any stand that sat exactly on the transferred line | M (from P† GB) | BA on the line varies only 86.8–94.7 m²/ha over Dq 15–60 cm. This is a property of the transferred slope, **not an Irish measurement**. |
| D-06b | **Derived RD of Frenchpark** on the British line, using mean DBH as a Dq proxy (understates Dq, so RD and BA are lower bounds): unthinned RD ≥ 0.845, BA ≥ 77.7 m²/ha; light ≥ 0.711, ≥ 64.5; medium ≥ 0.639, ≥ 57.8; heavy ≥ 0.563, ≥ 50.8 | Frenchpark at 29 growing seasons | M (from P† IE and P† GB) | Mixes an Irish summary with a British line; thinned stands also had earlier removals. Frenchpark unthinned BA ≥ 77.7 m²/ha exceeds the Jones "~55" (flag F-01). |
| D-07 | Reineke exponent −1.605 (`log10 N = −1.605 log10 D + k`); k for Irish Sitka unknown | Universal forestry transfer | P† (general forestry) | "Placeholder only"; ordinal guidance (R1 §2). Superseded for Sitka by D-05 in the CCF model. |
| D-08 | Pretzsch & del Río 2020: maximum-density `N = a·dbq^b` and DEC/DMC; analysed 64 experiments, 328 plots, 2,627 surveys; species Norway spruce, Scots pine, Douglas-fir, beech, sessile oak (R1 §3) | Central European long-term experiments | P† EU | **Sitka is not among the analysed species** and no Irish data; the paper says data were too limited to model DMC smoothly. Coefficients not transcribed and "NOT to be lifted into CCF". |
| D-09 | UK guidance to respace dense natural regeneration toward about 2,000–2,500 stems/ha on suitable stable sites (R1 §5) | British silvicultural guidance | S GB | A silvicultural target, "not a biological constant". |
| D-10 | **Multi-P1 synthetic stand** (printed in R9 only): 100 living age-30 Sitka, 30 × 30 m (declared area 0.09 ha), RD 0.609795828454288 at Year 0 and 0.6375836886879149 after Year-1 growth. **Derived:** N = 1,111.1 stems/ha; Dq ≈ 25.30 cm (Year 0) → 25.86 cm (post-growth); BA ≈ 55.9 → 58.3 m²/ha | Authored synthetic fixture, not a measured stand | M (from G) | Derived only from the printed RD, count and area; fixture bytes were not read. Inherits the D-05 transfer. Dq at which this N reaches RD 1: 32.16 cm (BA 90.3). |
| D-11 | CCF Scenario One start stand: 2,100 stems/ha, Dq 15.79 cm, BA 41.1 m²/ha, RD 0.435 (R6, R7) | Authored (40 × 40 m, 336 stems, age 20) | G CCF | Authored; flag F-09. |
| D-12 | FC-vs-GROWFOR Sitka cumulative volume, yield class 24, even-aged: no-thin 1,026 vs 1,014 m³/ha; MTI thinning 1,135 vs 995 m³/ha (R2 §13.1) | Model-to-model scenario | S IE (model comparison) | "Model-to-model scenario anchors, not independent empirical validation." |
| D-13 | GROWFOR vs NFI (n = 200): volume bias −1.41 %, RMSE 18.80 %, R² 0.84; basal area bias −1.96 %, RMSE 12.81 %, R² 0.81. Thinned (n = 107) BA bias −1.44 %, RMSE 13.86 %, R² 0.80; no-thin (n = 93) BA bias −2.47 %, RMSE 11.80 %, R² 0.77 (R2 §13.3) | Irish NFI cross-section | P† IE | Cross-sectional with NFI data-quality limits; authors say both models need improvement. **Not** longitudinal validation of the final dynamic model. |

## 5. Growth evidence

### 5.1 DBH and height envelopes

| ID | Item | Population / condition | Class | Uncertainty and caveat |
| --- | --- | --- | --- | --- |
| G-01 | Mean DBH ratio after thinning relative to unthinned at 29 growing seasons (Frenchpark): light 29.8/23.7 = 1.257; medium 31.1/23.7 = 1.312; heavy 32.4/23.7 = 1.367 | Irish thinning experiment | M (from P† IE) | Stand mean, not individual-tree release. Flag F-04. |
| G-02 | CCF potential DBH growth 1.2 cm/yr with "evidence range 0.9–1.5"; Frenchpark thinning ratios "reproduced" by the CCF response (1.28 / 1.38 vs 1.26 / 1.37) (R7, R8 rows `dbh`) | CCF calibration C8, from Teagasc inference and Frenchpark | G CCF (grade C calibration) | R7: "absolute value only loosely constrained". The C8 report was not read. |
| G-03 | Unthinned DBH growth after about 16 cm: 0.27–0.8 cm/yr; ages 20–40 inside the band, later ages "unsupported (no density feedback)" (R8) | Frenchpark inference via C8 | S IE (calibration-grade inference) | Not a direct measurement list. |
| G-04 | Height growth: top height depends on site, not stocking; the British individual-tree Sitka model did not retain competition for height (R1 §5) | Irish site model; British individual-tree model (not named in accessible text) | P† IE (site) / S GB (model) | The named source for the British model is not in accessible text. |
| G-05 | CCF growth model 1 height: Class III envelope (S-04) with each tree keeping its relative height, `H ← H·E(age+1)/E(age)`; thinning moves top height only through which trees form the largest-DBH set; CCF top height drifts 6–9 % under the envelope after age 60 (R3) | CCF model | G CCF | Engine behaviour, not a measurement. |
| G-06 | Individual-tree DBH–height relationship, age-30 to age-40 DBH or height increments, and within-stand size distribution for Irish Sitka | — | **U** | No accessible Irish measurement. NFI SDs (D-01) are between-plot. CCF's authored DBH p10/p50/p90 (12.02 / 15.49 / 19.02 cm at Dq 15.79, R7) are G, not evidence. |

### 5.2 Stand-level GROWFOR versus individual-tree dynamics (explicit boundary)

| Question | Stand-level GROWFOR / Broad–Lynch | Independent individual-tree experimental evidence accessible here |
| --- | --- | --- |
| State | B, N, H per hectare (R2 §4.1) | None accessible for Irish Sitka DBH release. Crown geometry exists (Tabbush & White, Davies & Pommerening) |
| Spatial / neighbour effects | None; no tree positions | Davies & Pommerening: stem-mapped Welsh plots, spatial variables improved crown models |
| Thinning response | Selection-thinning transition, thinned/unthinned systems; the extended "relative closure" state was **not retained** and the authors concluded the stand-level post-thinning effect was largely absent in the available data (R2 §7) | Frenchpark means (G-01), stand level only |
| Mortality | Embedded in N dynamics; **no** individual or cause-specific hazard (R2 §2A.3) | None accessible |
| Validation | Source-data validation only (2006b); NFI cross-sectional 2017; no longitudinal final-model forecast, no continuous-cover, no individual-tree validation (R2 §2A.1; R1 §9.1 scores 2 / 0 / 0) | n/a |

**Required statement.** GROWFOR/Broad–Lynch **does not validate** local tree-level Hegyi release, spatially heterogeneous diameter increment, regeneration or recruitment, multiple cohorts, mixed species, or strongly irregular or continuous-cover behaviour (R2 §2A.4, §14.2; R3). Its supportable use is an **aggregate plausibility comparison** for pure even-aged Sitka under conventional thinning. A thinning effect that is absent at stand level (R2 §7) is not evidence that retained trees do not respond locally; the extraction says so explicitly.

## 6. Competition and crown

### 6.1 What is supported at tree level, and what is not

| ID | Item | Population / condition | Class | Uncertainty and caveat |
| --- | --- | --- | --- | --- |
| C-01 | Open-grown Sitka potential crown: `CrownWidth_cm = 188.30 + 15.27·DBH_cm`, equivalently `PotentialCrownRadius_m = 0.9415 + 0.07635·DBH_cm` (halving and converting units checked); r = 0.93; 73 trees (ages 9–71), 72 used after outlier removal; DBH 7–112 cm sampled, 7–80 cm in the regression table (Tabbush & White 1988) | British open-grown Sitka | P† GB | **A potential (open-grown) envelope, not a plantation crown** (R1 §8). Derived radius: 2.09 m (DBH 15), 2.85 (25), 3.23 (30), 4.00 (40), 4.76 (50), 5.52 m (60). Outside DBH 7–80 cm is extrapolation. |
| C-02 | **Derived occupancy check:** at Dq 25.3 cm the potential radius is 2.87 m, crown projection 25.9 m² per tree; 100 trees on 900 m² (mean spacing 3.0 m) at potential would project 2,590 m², 288 % of ground area | Multi-P1 synthetic stand (printed numbers only) | M (from P† GB and G) | Shows realised crowns must be far below potential; **the realised/potential ratio is not evidenced** (C-05). |
| C-03 | Davies & Pommerening 2008: 154 Sitka and 125 birch crowns in six stem-mapped Welsh plots; Sitka crown-radius "efficiency" 0.75 → 0.77 and crown-length efficiency 0.67 → 0.74 when spatial variables are added; crown length `CL_m = H·exp(−max(0, 0.58557·(H/DBH_cm) + 0.30910·BA_5))`; pooled crown-radius model has a **positive Hegyi coefficient** (R8: `r = exp(−2.178 + 0.823 ln D + 0.109 Hegyi7)`) that the synthesis says must not be used causally | British Sitka and birch | P† GB | "Efficiency", `BA_5` (neighbourhood basal area and its unit/radius) are not defined in accessible text. The counterintuitive coefficient is a documented conflict, not a rule. |
| C-04 | Hegyi index `CI_i = Σ_j (DBH_j/DBH_i)/distance_ij` is an established method; its response `1/(1+CI/CI50)`, CI50, the 8 m cutoff and the CI→crown reduction are CCF abstractions (R1 §5, R7) | Method general; parameters CCF | S (method) / G CCF (parameters) | R7: Hegyi with fixed distances is scale-invariant, so uniform growth leaves CI unchanged and provides **no carrying capacity**. |
| C-05 | Realised crown radius in closed Irish Sitka stands as a function of DBH, spacing or CI | — | **U** | Not accessible. |
| C-06 | CCF crown rule: target = potential × response, relaxation 15 %/yr (R7); R1 gives a "starting range" 0.1–0.2/yr | CCF | G CCF | Calibration, not evidence. |
| C-07 | Tree-level DBH competition multiplier `1/(1+CI/5)` (Ci50 = 5) | CCF | G CCF | Calibrated to reproduce Frenchpark stand-mean ratios (G-02), not validated per tree. |

### 6.2 Unsupported gameplay rules (explicit list)

Local Hegyi release magnitude; Ci50 = 5 and the 8 m cutoff; the crown competition reduction; the relaxation rate; the use of suppression S = 1 − 1/(1+CI/5) as a mortality weight; any statement that dominants survive at a given rate. All are G.

## 7. Mortality

### 7.1 What is and is not evidenced

| ID | Item | Population / condition | Class | Uncertainty and caveat |
| --- | --- | --- | --- | --- |
| M-01 | Broad & Lynch tracks observed stocking `N`; mortality is implicit. It "does not provide an independently parameterised individual-tree or cause-specific mortality hazard" and is "not evidence for CCF's suppression-biased individual-tree mortality allocation" (R2 §2A.3) | Irish PSPs | P† IE (statement of scope) | Fixes the evidential boundary: **there is no accessible Irish mortality hazard.** |
| M-02 | Comeau line (D-05) is a **maximum-density boundary**, not a mortality rate or dead-stem distribution | British | P† GB | Boundary only. |
| M-03 | Forest Research *Forest Yield* unthinned Sitka outputs (stems, DBH, BA, mortality by age) were "not obtained" and remain the recommended validation source (R6, R8 last row) | British | **U** | The only accessible *named* mortality-by-age benchmark is missing. |
| M-04 | NFI stocking range 240–4,967 stems/ha and 183 of 200 plots with ingrowth (D-01); Frenchpark stems by treatment (D-02) | Irish | P† IE | Stocking is not a mortality estimate (no initial stocking, no remeasurement in accessible text). |
| M-05 | Irish windthrow model: top height, exposure, soil, ground preparation and thinning status are predictors; it predicts whether more than about 3 % of stems are windthrown by a given top height, not an annual per-tree probability (R1 §6) | Irish stand level | P† IE | A different cause of loss; the 3 % is a stand-level exceedance threshold. Fossy Hill (wetter) had windfall and Ballycullen practically none, treatment not separable from site. No rates. |
| M-06 | Measured dead stems per hectare, annual mortality percentage, size or suppression class distribution of deaths, or snag/deadwood rates for Irish Sitka at ages 30–40 | — | **U** | Not accessible. Not inferred. |
| M-07 | CCF rule: `RD = N·(Dq/25)^2.063/1868`; pressure `P = max(0,(RD−0.6)/0.4)`; per-tree probability `min(0.5, 0.08·P²·S⁴/mean(S⁴))` with S the Hegyi suppression; boundary: if survivors exceed RD 1 the most suppressed die until RD ≤ 1; onset 0.6, strength 0.08, S⁴ exponent and 0.5 cap are CCF calibration (R3, R6; R9 restates them as the Multi-P1 source rule) | CCF | G CCF | **Game calibration, not an Irish empirical law.** R6: the exponent moved from S² to S⁴ because S² let even-sized dominants die at about 2 %/yr in a fixture; normalisation keeps the expected stand rate `m·P²`. |
| M-08 | Derived consequences of M-07: expected stand hazard rate `m·P²` = 0.5 % (RD 0.7), 2.0 % (0.8), 4.5 % (0.9), 8.0 %/yr (1.0); for the printed Multi-P1 stand (post-growth RD 0.6376): P = 0.0940; expected Year-1 hazard deaths = 100 · 0.08 · P² = 0.0706 (about 0.07 stems, consistent with the reported zero deaths). The 0.5 cap cannot bind here because the largest possible per-tree p is bounded by N · 0.08 · P² = 0.0706 | CCF rule applied to printed numbers | M (from G) | Holds while no tree hits the 0.5 cap. Says nothing about real mortality. |
| M-09 | CCF long-run runs (growth model 1, Scenario One stand): unthinned RD 0.435 → 0.69 (age 30) → 0.91 (60) → 0.97–0.99 (80–120); 609 cumulative deaths by age 120, i.e. over 100 simulated years from age 20 (count as printed in R3/R6; per-stand vs per-ha and recruit inclusion not stated); most-suppressed quarter lost 65 % vs least-suppressed 14 % in a 10-yr RD ≈ 0.9 fixture; recruit pulses account for single-year spikes of 22–50 % (R3, R6) | CCF | G CCF | Output of the rule, not evidence. R3 itself lists the calibration as open. |

### 7.2 Statement on the current S⁴ normalisation

The S⁴ vulnerability exponent, the mean(S⁴) normalisation, onset RD 0.6, strength 0.08, the 0.5 cap and the RD ≤ 1 boundary are **CCF game calibration** (R6 "Selected [C]"). Comeau's line supplies the *position of a boundary* (transfer evidence); nothing accessible supplies the *shape, rate or size bias of mortality*. No hazard function is proposed here.

## 8. Required comparison-metrics table

Targets for a later calibration comparison. A cell marked **U** means "do not fill; needs a decision or new evidence". **No row is an acceptance threshold.**

| ID | Metric | Unit | Accessible value or range | Population / site / age / condition | Class | Uncertainty and caveat |
| --- | --- | --- | --- | --- | --- | --- |
| CM-01 | BA, measured | m²/ha | Mean 32.97, range 6.69–74.80, SD 12.78 | Irish NFI1 even-aged pure Sitka, n = 200, mean age 22.73 (9.10–49.00); thinned/no-thin mix | P† IE | Between-plot; plots not stratified by site class in accessible text; age ≤ 49. |
| CM-02 | BA, designed-trial lower bound | m²/ha | 77.7 (unthinned), 64.5 (light), 57.8 (medium), 50.8 (heavy) | Frenchpark, 29 growing seasons; BA from stems and mean DBH | M (from P† IE) | Lower bound (mean DBH < Dq); age basis unknown (D-02). |
| CM-03 | BA on British maximum line | m²/ha | 86.8–94.7 at Dq 15–60 cm; 52.1–56.8 at RD 0.6 | Transferred British line | M (from P† GB) | Boundary, not typical stand; no Irish validation. |
| CM-04 | BA, untreated trial value | m²/ha | about 55 | Jones et al. 2026 trial; location unverified | S | Flag F-01. Not an Irish maximum. |
| CM-05 | BA of Multi-P1 synthetic stand (no-intervention) | m²/ha | about 55.9 (Year 0), 58.3 (post Year-1 growth) | Printed RD/N/area only | M (from G) | Authored; synthetic; inherits D-05. |
| CM-06 | BA at age 40 and 50 for Irish Sitka | m²/ha | — | — | **U** | GROWFOR B(t) not transcribed/run here; no accessible measured trajectory. |
| CM-07 | Stems per hectare, measured | stems/ha | NFI mean 1,662 (240–4,967, SD 911); Frenchpark 1,762 / 925 / 761 / 616 | NFI (above); Frenchpark unthinned/light/medium/heavy at 29 seasons | P† IE | Ingrowth, plot size, thinning history distort; initial planting density **U**. |
| CM-08 | Stems per hectare of Multi-P1 stand and CCF scenario stand | stems/ha | 1,111.1 (Multi-P1, age 30, derived); 2,100 (Scenario One, age 20) | Authored | M (from G) / G CCF | Different stands (F-09). |
| CM-09 | Mean DBH | cm | NFI mean diameter 18.66 (8.88–32.73, SD 5.00) at mean age 22.73; Frenchpark 23.7 / 29.8 / 31.1 / 32.4 | As CM-01 / CM-07 | P† IE | Arithmetic mean diameter, not Dq; SD is between-plot. |
| CM-10 | Quadratic mean DBH (Dq) | cm | **U** for measured Irish values; Multi-P1 derived 25.30 → 25.86 (not evidence) | — | **U** / M (from G) | No accessible measured Dq. |
| CM-11 | Mean height | m | NFI mean 11.93 (4.91–30.80, SD 4.21) at mean age 22.73 | NFI | P† IE | NFI top-height approximation; definition of mean height not restated. |
| CM-12 | Top height at age 30 by class | m | I 27.6, II 23.3, III 20.4, IV 17.4, V 16.0 | Lekwadi Irish classes | P† IE | "±%" undefined (S-02). |
| CM-13 | Top height at age 40 (Class III context) | m | 24.89 (Lekwadi-form, S-04) and 26.65 (GROWFOR-equation, S-10); **two values, no average** | Site with 20.4 m at 30 | M (from P† IE) | 7.1 % apart (F-03); both are fitted curves, not measured trajectories. |
| CM-14 | Top-height increment 30→40 (Class III context) | m | 4.49 (S-04) to 6.25 (S-10) | Same | M (from P† IE) | Wide model disagreement. |
| CM-15 | Crown envelope, open-grown potential radius | m | 2.09 (DBH 15), 2.85 (25), 3.23 (30), 4.00 (40), 4.76 (50), 5.52 (60) | Tabbush & White, British open-grown, DBH 7–80 cm | P† GB | **Upper bound only** (C-01). |
| CM-16 | Realised crown radius or width in stands | m | — | — | **U** | C-05. |
| CM-17 | Within-stand size variation (DBH coefficient of variation or percentile spread) | % or cm | — | — | **U** | NFI SDs are between-plot. CCF's authored spread is G. |
| CM-18 | Dead stems (count or BA per hectare, snags) | stems/ha | — | — | **U** | M-06. |
| CM-19 | Mortality rate (annual) at age 30–40 | %/yr | — | — | **U** | M-06. |
| CM-20 | Mortality distribution by size or suppression class | — | — | — | **U** | M-01, M-06. Only G values exist (65 % vs 14 %, M-09). |
| CM-21 | Relative density on British line | dimensionless | RD ≥ 0.845 / 0.711 / 0.639 / 0.563 (Frenchpark, lower bounds); Multi-P1 0.610 → 0.638 | As above | M (from P† IE and GB; from G) | Mixed Irish data and British line. |
| CM-22 | Stand volume | m³/ha | NFI mean 201.30 (11.93–1,010.60); FC/GROWFOR YC24 cumulative 1,026 / 1,014 (no-thin) | NFI; model-to-model | P† IE / S IE | CCF volume uses a 0.5 form-factor "untested against Irish volume data" (R3), so CCF volumes are indicative only (G). |
| CM-23 | GROWFOR vs NFI performance | % / R² | See D-13 | NFI cross-section | P† IE | Context, not pass/fail thresholds (R2 §13.3). |
| CM-24 | Repeat-step monotonicity | — | See §9 Group A (required by contract) and Group B (expected, empirical) | — | M / G / U per row | Separates what is arithmetic from what is empirical. |

## 9. Ten sequential annual steps: three groups

**Frame.** A no-intervention run `0 → 1 → … → 10` of the closed adult pure-Sitka Multi-P1 population is, in the #737/#760 DRAFTs, a **proposed verification horizon of ten individual steps**, not a game lifespan, not a product limit and not an Irish forecast (R9 §S1; R10 B-O03). Nothing here predicts any Irish stand. The rule statements below are those printed in the DRAFTs (R9, R10, status DRAFT, not approved); the "growth model 2" definition was not read (F-08), so height and DBH statements are conditioned on "if the model-1 rules apply".

### Group A — mathematically required by the engine contract (from rule text, independent of biology)

| ID | Quantity | Required property per step and over ten steps | Class |
| --- | --- | --- | --- |
| T-01 | Ecological year | Increases by exactly 1 per step; after ten steps year 10. | G (spec text) |
| T-02 | Age of each living tree | +1 per step, so +10 after ten steps for survivors (age 30 → 40). Dead rows: freeze-at-death is **proposed and not adopted** (OPEN-B3). | G |
| T-03 | Living N | Non-increasing without recruitment or work; `N_after = N_before − deaths` each step; stable tree IDs; no resurrection or duplicate death. | G / M |
| T-04 | Living DBH under the printed rule `ΔD = 1.2·site·clamp01(1−D/120)/(1+CI/5)` (site = 1) | Non-negative and ≤ 1.2 cm/yr, so ≤ 12 cm in ten steps; from Dq 25.30 cm the maximum (CI = 0) is +9.06 cm (to 34.36 cm). Uniform-tree illustration, no deaths: CI 15 → +2.34 cm (RD10 0.732, BA 66.7 m²/ha); CI 8.2 → +3.53 (0.798, 72.5); CI 5 → +4.63 (0.862, 78.2); CI 2 → +6.55 (0.981, 88.5); CI 0 → +9.06 (1.146, 103.0). The RD10/BA10 values are a pre-mortality counterfactual; the engine's RD > 1 boundary would cap RD at 1. CI 8.2 is illustrative, taken from R7's Scenario One stand (a different stand, F-09), not a Multi-P1 property. | M (from G) |
| T-05 | Year-1 hazard | Expected hazard deaths about 0.07 stems (M-08), consistent with the reported zero. | M (from G) |
| T-06 | Height of a surviving tree under the model-1 relative-height rule | Multiplied each year by `E(age+1)/E(age) > 1`; ten-step factor `E(40)/E(30) = 1.2202` independent of competition. Crown relaxation `r ← r + 0.15(target − r)` closes `1 − 0.85^10 = 80.3 %` of a constant gap in ten steps; it can move down if the realised radius exceeds the target. | M (from G) |
| T-07 | RD and BA | With no deaths RD rises with `Dq^2.063` and BA rises (each step has ΔD ≥ 0). RD reaches 1 for N = 1,111 stems/ha only at Dq 32.16 cm (+6.9 cm over the Year-0 Dq of about 25.30 cm). With deaths BA and RD can move either way. After the mortality phase RD ≤ 1 except that a last survivor may exceed it. No hazard deaths while post-growth RD ≤ 0.6; hazard probability ≤ 0.5. | M (from G) |
| T-08 | Determinism | Identical seed, versions and runtime reproduce identical per-year outputs; the roll depends on seed, target year, RNG version and tree ID, drawn only when p > 0. | G |
| T-09 | Per-step accounting | Natural-death IDs are disjoint between hazard and boundary phases; `BA_after` follows from grown living stems. | G |
| T-10 | Dead-tree exclusion (the Year1→Year2 seam; #760 §3 S3 M-1 contract) | Dead trees are excluded as Hegyi target **and** as neighbour, and from N, BA, Dq, RD, mean(S⁴) and the hazard rolls; dead IDs are retained in the tree list. Source: #760 §3 S3 (printed contract; the DRAFT itself was not re-read here). | G (spec text) |

### Group B — empirical expectations (direction or ordinal range, with limited support)

| ID | Expectation | Support | Class |
| --- | --- | --- | --- |
| E-01 | Top height rises with age on a productive Irish site and is largely independent of stocking or normal thinning; at 30→40 a Class III site gains roughly 4.5–6.3 m in the two fitted descriptions (CM-13, CM-14). **Only relevant if the engine reports top height**, which was not verified. | Qualitative P† IE; curve values M | P† IE / M |
| E-02 | Quadratic mean DBH increases in the absence of intervention; a ten-step gain would be expected to fall inside the inferred stand-level unthinned band of 0.27–0.8 cm/yr, i.e. 2.7–8 cm (G-03). The engine's uniform-tree gains (T-04) sit inside that band for CI 2–8.2 (3.53–6.55 cm), below it at CI 15 (2.34 cm) and above it at CI 0 (9.06 cm); this compares a uniform-tree illustration with a stand-level inference, so it is ordinal only. | Grade C inference from Frenchpark via C8; C8 report not read | S IE |
| E-03 | A stand near RD 0.6 at age 30 is expected to remain at or below the transferred British boundary (RD ≤ 1) through age 40 | Transferred line, ordinal. Comeau supplies a boundary position, not an observed trajectory; the RD ≤ 1 cap is a CCF rule. | S (inference from P† GB boundary) |
| E-04 | Density-dependent self-thinning is expected to appear only as a stand approaches a maximum size–density line, and to intensify toward it. (Under the CCF rule specifically, no hazard deaths occur at RD ≤ 0.6; that threshold is G, not evidence.) | Direction rests on the existence of the Comeau boundary and general forestry; the onset and rates are unsupported | S / G |
| E-05 | In a thinned scenario (not part of a no-intervention test) mean DBH after thinning exceeds unthinned (Frenchpark ratios 1.257–1.367 at 29 seasons) | P† IE stand-level | M (from P† IE) |

### Group C — cannot be asserted from accessible evidence

* The **number** of deaths, annual mortality percentage or BA/N at year 10 for any real or synthetic stand (M-06, CM-18/19).
* **Which** trees die, or any size/suppression bias of deaths (CM-20).
* Within-stand DBH spread, height–DBH relation and realised crown size at ages 30–40 (G-06, C-05).
* Whether the ten-year trajectory of the synthetic stand resembles a real Irish stand, because the fixture is authored and its site class, top height, planting history and size distribution are not evidenced in accessible text.
* Any behaviour after recruitment, mixed species, irregular structure or repeated single-tree interventions (R2 §14.2).
* Any numerical oracle for years 2–10; R9/R10 state none exists.

## 10. Open decisions and conditional envelopes (not adopted)

Each option is a conditional for an owner or later calibration lane; none is recommended as a coefficient.

| ID | Decision | Conditional options (evidence they would rest on) | Not supported / needs decision |
| --- | --- | --- | --- |
| O-1 | Initial density and size distribution for a synthetic stand | (a) Keep the frozen Multi-P1 stand and label it synthetic (1,111 stems/ha, Dq ≈ 25.3 cm, BA ≈ 55.9 m²/ha at age 30; D-10). (b) Re-anchor inside the NFI envelope (age 9–49; 240–4,967 stems/ha; BA 6.7–74.8 m²/ha; D-01). (c) Re-anchor to Frenchpark-type summaries (616–1,762 stems/ha, 23.7–32.4 cm; D-02). | Within-stand size distribution is **U**; any chosen spread must be labelled gameplay. Site class for the fixture is **U** in accessible text. |
| O-2 | Acceptable BA/RD range for comparison | (a) Ordinal only: report RD against the British line and flag RD > 1. (b) Report BA against the observed NFI range and Frenchpark lower bounds as *context*. (c) Wait for the Jones 2026 provenance (F-01). | A numeric ceiling or typical value for Irish Sitka is **U**; the earlier 80–90 m²/ha ceiling was unsourced and dropped (R6). |
| O-3 | Mortality calibration target | (a) None: keep mortality as labelled gameplay and report diagnostics only. (b) Obtain the missing Forest Yield unthinned Sitka outputs (R6). (c) Use a reconstructed, explicitly named `BroadLynch2006_Sitka` N(t) as a stand-level comparison curve for conventional even-aged stands (needs separate verification, R2 §15). (d) Obtain Irish repeat-measurement data. | No hazard form, rate, onset or size bias is evidenced (M-01, M-06). The S⁴ exponent and all M-07 constants stay G. |
| O-4 | Crown envelope | (a) Use Tabbush potential radius as an upper bound only. (b) Add a realised-crown source (a Davies & Pommerening-type relation) after reading the primary. (c) Keep the CI-based reduction as labelled gameplay. | Realised crown size (C-05) is **U**; the relaxation rate (C-06) is G. |
| O-5 | May any GROWFOR stand equation be a **comparison-only** benchmark? | Conditional yes for aggregate B, N, H, V of pure even-aged Sitka under conventional thinning, if: the model is named `BroadLynch2006_Sitka`; the version caveat is carried (R2 §12); the top-height and site-index definitions are reconciled with the engine; thinning is limited to conventional regimes; and results are never used for tree-level, regeneration, mixed or irregular behaviour. Report the Lekwadi-form vs GROWFOR height disagreement (S-10) as a band, not a target. | No GROWFOR coefficient is approved. Not a validation of the engine; validation score remains 2/0/0 (R1 §9.1). |
| O-6 | Top-height definition | Reconcile "100 largest-DBH stems/ha" (R4, flagged unverified), Lekwadi and GROWFOR definitions before comparing top height. | **U** until a primary source is read. |
| O-7 | Growth-model identity | Confirm what "growth model 2" in the Multi-P1 engine changes relative to model 1 before using S-04 or the DBH rule in a comparison. | F-08 open. |
| O-8 | Dead-row and empty-stand policy | Owner decisions OPEN-B2/B3 in R9/R10. | Not evidence questions. |

## 11. Provenance appendix

### A. Source locators for each number family

| Number family | Where it appears in accessible material |
| --- | --- |
| Lekwadi dataset, anchors, Table 3, MAI/CAI, thinning ages | R1 §4 (citing the Primary Literature Synthesis pp. 3–4, 10); R4; R8 `site` rows |
| Site range 4.5–28.9 m | R1 §4 (Irish_Sitka_CCF_Deep_Research_Report p. 3) |
| Conry & Clinch 1989 | R1 §10 |
| GROWFOR height parameters, sample sizes, coefficients, validity limits | R2 §§5, 6.3, 8.3, 9.4, 11 |
| NFI Table 1 and performance | R2 §§13.2–13.4 (McCullagh et al. 2017, Tables 1–2) |
| FC vs GROWFOR volumes | R2 §13.1 (McCullagh et al. 2013) |
| Frenchpark table | R1 §5 (Irish_Sitka_CCF_Deep_Research_Report p. 4) |
| Jones et al. 2026 "~55 m²/ha" | R1 §2 |
| Comeau slope −2.063, SDI 1,868, Dq 25 cm | R6; R3; R8 row `mortality` |
| Reineke −1.605 | R1 §2; R8 |
| Pretzsch & del Río coverage | R1 §3 |
| Tabbush & White; Davies & Pommerening | R1 §5, §8; R7; R8 `crown` |
| CCF rule constants (RD 0.6, 0.08, S⁴, 0.5, crown 0.15) | R3, R6, R7; restated in R9 after §S4 |
| Multi-P1 stand numbers (100 stems, 30 × 30 m, area bits, RD values) | R9 "Protected Year-1 witness" paragraph; R10 §1 |
| Windthrow | R1 §6 |
| Validation scoring 2/0/0 | R1 §9.1; R2 §2A |

### B. Reproducing every derived number

All **M** values come from this standard-library Python script (no network, no repository files; inputs are the literals above). It was run once during authoring and its output matches every derived value in this file.

```python
# Reproduces every MATHEMATICAL DERIVATION (M) number in this document. Pure standard library.
import math
# --- Inputs transcribed from accessible CCF documents (see Appendix A) ---
lek = {'I': (58.371, 0.024, 1.247, 27.6), 'II': (49.523, 0.025, 1.247, 23.3), 'III': (34.228, 0.042, 1.563, 20.4),
       'IV': (28.272, 0.051, 2.134, 17.4), 'V': (22.557, 0.060, 2.619, 16.0)}   # b0,b2,b3, published age-30 anchor
GF_A, GF_C, GF_T0 = 47.0216, 0.4571, -5.0                                        # GROWFOR height parameters
SLOPE, SDI_MAX, DQ_REF = 2.063, 1868.0, 25.0                                     # Comeau British line
H = lambda b0, b2, b3, t: b0 * (1 - math.exp(-b2 * t)) ** b3
BA = lambda n, dq: n * math.pi / 4 * (dq / 100) ** 2                             # m2/ha, dq in cm
RD = lambda n, dq: n * (dq / DQ_REF) ** SLOPE / SDI_MAX
# S-03 rounded Lekwadi coefficients vs anchors
for k, (b0, b2, b3, a30) in lek.items():
    print('S-03', k, 'H30_rounded=%.2f diff=%.2f H100=%.1f' % (H(b0, b2, b3, 30), H(b0, b2, b3, 30) - a30, H(b0, b2, b3, 100)))
# S-04 anchor-matched class III (b0 rescaled so H(30)=20.4)
b2, b3 = 0.042, 1.563
b0 = 20.4 / (1 - math.exp(-b2 * 30)) ** b3
print('S-04 b0=%.3f' % b0, {t: round(H(b0, b2, b3, t), 2) for t in (20, 30, 40, 60, 80, 100, 120)})
print('S-05 max MAI (integer ages 5..79) = %.3f m/yr at age %d' % max((H(b0, b2, b3, t) / t, t) for t in range(5, 80)))
# S-10 GROWFOR height equation, S = 20.4
bb = -math.log(1 - (20.4 / GF_A) ** GF_C) / (30 - GF_T0)
G = lambda t: GF_A * (1 - math.exp(-bb * (t - GF_T0))) ** (1 / GF_C)
for t in (20, 30, 40, 60, 80, 100):
    l = H(b0, b2, b3, t); print('S-10 t=%d GROWFOR=%.2f Lekwadi-III=%.2f diff=%.2f (%.1f%%)' % (t, G(t), l, G(t) - l, 100 * (G(t) - l) / l))
# D-05 British maximum line: BA at RD = 1 and RD = 0.6
for dq in (15, 20, 25, 30, 40, 50, 60):
    nmax = SDI_MAX * (dq / DQ_REF) ** -SLOPE
    print('D-05 Dq=%d Nmax=%.0f BA(RD1)=%.1f N(RD0.6)=%.0f BA(RD0.6)=%.1f' % (dq, nmax, BA(nmax, dq), 0.6 * nmax, BA(0.6 * nmax, dq)))
# D-06 Frenchpark (mean DBH used as Dq proxy: BA and RD are lower bounds)
for name, n, d in (('unthinned', 1762, 23.7), ('light', 925, 29.8), ('medium', 761, 31.1), ('heavy', 616, 32.4)):
    print('D-06', name, 'BA>=%.1f RD>=%.3f DBH ratio=%.3f' % (BA(n, d), RD(n, d), d / 23.7))
# D-10 Multi-P1 synthetic stand, from text-reported N, area, RD only (no fixture re-read)
n = 100 / 0.09
for rd in (0.609795828454288, 0.6375836886879149):
    dq = DQ_REF * (rd * SDI_MAX / n) ** (1 / SLOPE); print('D-10 N/ha=%.1f RD=%.4f Dq=%.2f BA=%.1f' % (n, rd, dq, BA(n, dq)))
print('D-10 NFI mean-of-means BA from N and Dq: %.1f (reported mean BA 32.97)' % BA(1662, 18.66))
# C-01 Tabbush potential crown
for d in (15, 25, 30, 40, 50, 60):
    print('C-01 DBH=%d radius=%.3f m width=%.1f cm' % (d, 0.9415 + 0.07635 * d, 188.30 + 15.27 * d))
r = 0.9415 + 0.07635 * 25.30
print('C-02 r=%.3f area/tree=%.1f m2, 100 trees/900 m2 = %.0f%%' % (r, math.pi * r * r, 100 * 100 * math.pi * r * r / 900))
# T-xx ten-step quantities under the DRAFT rule values (uniform-tree illustration, no deaths)
def grow(d, ci, yrs):
    for _ in range(yrs): d += 1.2 * max(0, min(1, 1 - d / 120)) / (1 + ci / 5)
    return d
for ci in (0, 2, 5, 8.2, 15):
    d10 = grow(25.30, ci, 10); print('T-04 CI=%s Dq10=%.2f gain=%.2f RD10=%.3f BA10=%.1f' % (ci, d10, d10 - 25.30, 0.609795828454288 * (d10 / 25.30) ** SLOPE, BA(n, d10)))
P = (0.6375836886879149 - 0.6) / 0.4
print('T-05 P=%.4f expected Year-1 hazard deaths (uncapped) = %.4f' % (P, 100 * 0.08 * P * P))
print('T-06 relative height ratio E(40)/E(30)=%.4f, crown gap closed in 10 y at k=0.15: %.4f' % (H(b0, b2, b3, 40) / H(b0, b2, b3, 30), 1 - 0.85 ** 10))
print('T-07 Dq at which RD=1 for N=1111.1: %.2f cm' % (DQ_REF * (SDI_MAX / n) ** (1 / SLOPE)))
for rd in (0.7, 0.8, 0.9, 1.0):
    p = (rd - 0.6) / 0.4; print('T-08 RD=%.1f stand expected hazard rate m*P^2 = %.4f' % (rd, 0.08 * p * p))
```

### C. Statements required by the brief

* **No primary-source measurement was manufactured.** Every measured value is quoted from an accessible CCF document and flagged † (primary not re-read). Every derived value is flagged M and reproduced in Appendix B.
* **No new growth or mortality coefficient is accepted**, proposed for adoption, or fitted. G values are documented as gameplay calibration.
* **No forecast claim** for any Irish or synthetic stand; §9 states properties, not outcomes.
* **No gameplay calibration is presented as evidence**: the S⁴ normalisation, onset RD 0.6, strength 0.08, cap 0.5 and crown and Hegyi constants are G.
* Protected material (see §0.2) was not read or re-hashed. L4 FAIL38 remains unwaived; no execution of any product or .NET build occurred.

## 12. Acceptance criteria for a later calibration comparison, and NOT RUN list

### 12.1 Proposed acceptance criteria (for a future lane; not met or checked here)

1. Every compared quantity carries its class code (P†/S/M/G/U) and a geography tag; U cells stay U unless new evidence is added with provenance.
2. Primary sources for any P† value used as a target are re-read by a named reviewer and the † removed explicitly, row by row.
3. Top-height comparisons state the top-height definition used by engine, Lekwadi and GROWFOR (O-6) and report the S-10 disagreement as a band.
4. Density comparisons report RD against the British line and state it as transferred; BA/N are reported against CM-01/CM-02 as context, not as thresholds.
5. Any use of GROWFOR equations is labelled `BroadLynch2006_Sitka`, restricted to aggregate B, N, H, V of pure even-aged Sitka under conventional thinning, with the version caveat and validation score 2/0/0 carried.
6. No tree-level, regeneration, mixed-species or irregular-stand claim is made from stand-level evidence.
7. Mortality is compared only on aggregates that exist (N, RD, BA); mortality rate, size bias and dead-stem distribution remain U until evidence exists.
8. Group A properties (§9) are tested as arithmetic contract checks, separate from Group B empirical expectations, and no Group C item is asserted.
9. A comparison reports differences and explains them; it does not tune a coefficient to hit an evidence value without a separate decision.
10. Independent review by a non-author checks each number against §11.A and reproduces Appendix B.

### 12.2 NOT RUN

* No .NET, native, Unity, Mono or Windows execution; no Host, save, viewer (#776), #779, #770 or Scenario One work.
* No engine run, no Year-2 to Year-10 simulation, no GROWFOR reconstruction (no B, N, H trajectory, no matrices, no volume equation), no thinning or assortment calculation.
* No re-read of any primary paper or PDF; no regeneration/ecology composite (missing).
* No access to protected #710/#725/P0/#687/#639 source, fixture bytes or CLI output; no re-hash of them.
* No test of the Multi-P1 growth model 2.
* The only computation executed was the stdlib arithmetic in Appendix B.

## 13. Correction log

| Field | Value |
| --- | --- |
| Trigger | Independent Claude Opus 5.5 review of this file at base commit `7f58bb5c9ab2f85cf00a0d0882562a955727f1fd`, published in Q review commit `bf8fec885258cc4e35a29d754499a0556230dab5` (`Multi-780-Q-INDEPENDENT-QUANT-AND-FORESTRY-REVIEW-20261118.md`). Reviewer verdict on A2: NEEDS BOUNDED CORRECTION, 0 BLOCKER, 1 SHOULD, 6 NIT; no numeric mismatch found. |
| Finding IDs | A2-S1 (§7.1 M-09 wording), A2-N1 (§7.1 M-08 arithmetic sentence), A2-N2 (§9 E-03 class), A2-N3 (§9 T-04 labels), A2-N4 (§3 S-08 qualifier restored), A2-N5 (§9 new row T-10), A2-N6 (§2 F-06 headline). |
| Numbers | **No derived number, table value or class other than E-03's reclassification changed.** The edits are wording, labelling and one added coverage row. |
| Correction commit | The single new commit on this branch after `7f58bb5c`; its SHA is recorded in the commit itself and in the [CORRECTION HANDED-OFF] comment on #780 (a file cannot contain its own commit hash). |
| Executor | A fresh Claude Sonnet 5.5 session (`claude-sonnet-5-5`) on provider Claude B (Pro), because the original Claude A session hit its five-hour rate limit. Authorship continuity is on the same document and branch, not the same session. The Author row above still describes the original session. |
