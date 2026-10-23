# GROWFOR Technical Evidence Extraction v1

**Project:** CCF — Irish Sitka spruce plantation transformation / continuous-cover forestry game
**Prepared:** 15 October 2026
**Revised:** 23 October 2026 — V1B evidence-record sync (Broad & Lynch 2006a/2006b roles separated)
**Project context:** `c6dc136b7f1f8668d9f67f95f48968d290ad5067`
**Status:** **Supporting research / technical evidence only — not decision authority**
**Primary purpose:** Extract the equations, coefficients, operating domain, validation anchors and material caveats available in the supplied GROWFOR source packet, so they can be used for reproducible offline comparison against CCF without silently changing production simulation behaviour.

> **Authority boundary.** This document reports what the supplied sources support. It does not adopt GROWFOR as the CCF growth engine, change the accepted pure-Sitka compatibility contract, or promote transfer/model coefficients into gameplay constants.

---

## 1. Source packet

| ID | Source | Main use here |
|---|---|---|
| G1 | **GROWFOR User Manual** — *Irish Dynamic Yield Models for Forest Management* | Distributed model inventory, required inputs, runtime projection-limit messaging, thinning/assortment/economic interface. |
| G2a | **Broad, L.R. & Lynch, T. (2006a), “Growth models for Sitka spruce in Ireland”**, *Irish Forestry* 63(1&2): 53–79 | **Primary model-development / calibration source.** Sitka mathematical specification: state-space growth, height/site index, volume, thinning, assortment, fitted coefficients, development data and validity discussion. |
| G2b | **Broad, L.R. & Lynch, T. (2006b), “Panel data validation using cross-sectional methods”**, *Irish Forestry* 63(1&2): 80–95 | **Primary empirical source-data / panel-data validation study.** Estate-versus-research site-index comparison, research-panel representativeness, experimental dependence and cross-sectional research/nonresearch volume-data diagnosis. |
| G3 | **McCullagh, A., Hawkins, M., Broad, L. & Nieuwenhuis, M. (2013), “A comparison of two yield forecasting methods used in Ireland”** | Later GROWFOR species/model context, FC-vs-GROWFOR output anchors, thinning-intensity behaviour and important post-2006 Sitka-model caveat. |
| G4 | **McCullagh, A., Black, K. & Nieuwenhuis, M. (2017), “Evaluation of tree and stand-level growth models using national forest inventory data”** | Independent NFI comparison of GROWFOR and CARBWARE for pure even-aged Sitka spruce and lodgepole pine; validation statistics and limitations. |

**Now in the supplied packet:** the uploaded journal issue `Irish Forestry, Vol. 63 No. 1&2, 2006` contains both G2a (pp. 53–79) and G2b (pp. 80–95).

**Still not in the supplied packet:** GROWFOR source code; executable/version metadata sufficient to prove which published coefficient set is compiled into the supplied/manual version; coefficient sets for species other than Sitka.

---

## 2. Executive technical conclusions

1. **GROWFOR is an empirical stand-level dynamic state-space model.** For Sitka spruce, the core state is basal area `B`, stocking `N`, and top height `H`; volume, thinning and assortment are ancillary functions derived from that stand state. [G2a, pp. 4–11]
2. **The 2006 Sitka paper does publish the model mathematics and fitted coefficients.** This includes thinned and unthinned growth matrices, height/site-index parameters, a stand-volume equation, a thinning-transition equation and assortment coefficients. [G2a, pp. 12, 20–21, 27–29]
3. **Site is represented by top-height development.** The preferred Irish fit is a polymorphic Bertalanffy–Richards height model, with site index defined as top height at age 30. [G2a, pp. 5–6, 12–13]
4. **There is no formal published multivariate “safe numeric box” for `B`, `N`, `H` in G2a.** Broad & Lynch state that projections are adequate *within the range of data used in model construction* and use Figures 6–10 to show the relevant state-space region. They also state that thinned PSP data begin at first thinning and unthinned PSPs were first measured at about age 20, leaving an early-growth data gap. [G2a, pp. 18–24]
5. **The user manual provides an additional runtime restriction for the illustrated active Sitka-thinned model:** top height must be **< 49.3 m**, and projections are best confined to ages **greater than first thinning (if any) through clearfelling**. The same window says each growth model was developed from Irish data. [G1, p. 7]
6. **The distributed manual shows more than the five conifers analysed in G3.** Its model-selection screen lists Sitka spruce, Norway spruce, Douglas fir, lodgepole pine, Scots pine and larch as thinned/unthinned models, plus an Ash thinned model. That is **7 named species / 13 listed model options** in that manual screenshot. [G1, p. 7]
7. **The supplied evidence does not validate GROWFOR for Irish irregular/CCF stands.** G2a uses pure Sitka stand-level trajectories and G4's independent evaluation deliberately uses single-species even-aged Sitka and lodgepole plots. G3 says development data had mainly included low-thinned stands and identifies crown-thinning data as desirable for alternative silvicultural practices. [G2a, pp. 13–24; G3, pp. 10; G4, pp. 1–3]
8. **A version caveat is essential:** G3 reports that later testing found the Sitka volume model lacked an asymptote and required further work to fix upper growth limits. Therefore the exact 2006 equations/coefficients below are reproducible evidence for the **Broad–Lynch 2006 Sitka model**, but cannot be assumed without verification to be byte-for-byte identical to every later GROWFOR executable. [G3, p. 4]
9. **Best CCF use:** aggregate validation/calibration envelope for pure Sitka, not direct substitution for CCF's spatial individual-tree ecology. This is a modelling recommendation derived from the evidence, not an accepted project decision.
10. **Broad & Lynch 2006b is empirical source-data validation, not final-model predictive validation.** It supplies estate-versus-research site-index tests and a cross-sectional research/nonresearch volume diagnosis. It does **not** report longitudinal forecast RMSE/MAE, prediction-interval calibration, an untouched final-model test set, or CCF/irregular-stand validation. The accepted V1B classification is: **empirical source-data validation confirmed; final growth-model predictive validation not demonstrated.**

---

## 2A. Evidence roles and validation status (V1B)

### 2A.1 Source roles

| Field | Value |
|---|---|
| `primary_model_source` | Broad & Lynch 2006a |
| `primary_validation_source` | Broad & Lynch 2006b |
| `model_name` | Broad–Lynch Irish Sitka spruce stand-growth model (`BroadLynch2006_Sitka` for precise reconstruction) |
| `model_level` | stand |
| `stand_structure_scope` | even-aged |
| `growth_state_variables` | `B_m2_ha`; `N_stems_ha`; `H_top_m` |
| `management_models` | separate thinned and unthinned state-space systems; thinning transition equation |
| `validation_evidence_class` | empirical source-data / panel-data validation |
| `validation_target` | panel representativeness, site-index distribution, experimental dependence, cross-sectional volume-data behaviour |
| `final_dynamic_model_external_validation` | false |
| `final_dynamic_model_longitudinal_validation` | false |
| `forecast_horizon_validation_reported` | false |
| `forecast_rmse_reported` | false |
| `forecast_mae_reported` | false |
| `prediction_interval_validation_reported` | false |
| `continuous_cover_validation` | none demonstrated |
| `individual_tree_validation` | none; model is stand-level |
| `validation_independent_of_original_research_panel` | true |
| `validation_independent_of_final_volume_model_calibration` | false |
| `independent_dynamic_growth_test_set` | none reported |

The two independence fields must stay separate. The 2003 nonresearch observations are independent of the original research experiments, but Broad & Lynch 2006a used them as a calibration dataset and adjusted fitting weights against them; they are therefore not an untouched test set for the final published volume model. [G2a, pp. 68–70; G2b, pp. 87–89]

### 2A.2 Primary empirical results from Broad & Lynch 2006b

**Site-index representativeness.** The estate reference population comprised **44,217** subcompartments: **27,887 thinned** and **16,330 unthinned**. The research site-index distribution differed strongly from the estate: thinned **χ²(7) = 625.5, p < 0.001**; unthinned **χ²(7) = 1648.2, p < 0.001**. The lowest site-index classes present in the estate were absent from the research plots. [G2b, pp. 82–84]

**Cross-sectional production-stand volume diagnosis.** The 2003 nonresearch sample comprised **113 stands**: **70 thinned** and **43 unthinned**. The common-volume relationship test gave **F(4,105) = 0.8835**, so the null of a common thinned/unthinned volume relationship was **not rejected**. Research-data volume residuals showed a systematic thinned/unthinned pattern; Broad & Lynch treated sample-tree selection as a plausible explanation, which is an evidence-consistent hypothesis rather than a demonstrated causal result. [G2b, pp. 87–89]

**Dependence limitation.** The research panel came from replicated, spatially blocked experiments with repeated measurements. Broad & Lynch explicitly acknowledge that this compromises plot-level independence; the reported χ² p-values should not be read as if every plot were an independent random estate sample. [G2b, pp. 82, 89]

### 2A.3 Mortality representation

Stand-level stocking loss/mortality is embedded in the `N` state dynamics, but Broad & Lynch does not provide an independently parameterised individual-tree or cause-specific mortality hazard. The `N` equation tracks observed stems per hectare and does not identify which tree dies or why, nor does it separate competition, windthrow, disease or other causes. Broad & Lynch is therefore **not** evidence for CCF's suppression-biased individual-tree mortality allocation. [G2a, pp. 4–11; G2b, pp. 90–92]

### 2A.4 CCF applicability boundary

Strongest support from G2a/G2b:

- pure Sitka spruce;
- stand level;
- principally even-aged;
- conventional thinned/unthinned conditions;
- aggregate `B`/`N`/`H` comparison.

Not validated by these sources:

- individual-tree growth response;
- local Hegyi release;
- natural regeneration/recruitment;
- multiple cohorts;
- mixed species;
- strongly irregular CCF structure;
- continuous-cover predictive behaviour.

Broad & Lynch remains a useful aggregate plausibility benchmark while Scenario One stays plantation-like. Its evidential authority decreases as the stand becomes structurally irregular, multicohort and mixed-species.

---

## 3. Distributed GROWFOR model inventory and interface

### 3.1 Model options visible in the supplied manual

The model-selection screenshot in G1 p. 7 shows:

| Species | Thinned | Unthinned |
|---|:---:|:---:|
| Sitka spruce | Yes | Yes |
| Norway spruce | Yes | Yes |
| Douglas fir | Yes | Yes |
| Lodgepole pine | Yes | Yes |
| Scots pine | Yes | Yes |
| Larch | Yes | Yes |
| Ash | Yes | **Not shown** |

This is a software/manual inventory only. The supplied packet does **not** contain published coefficient sets for all of these models.

### 3.2 Required stand inputs

G1 requires the following current-state inventory inputs:

- Age, years.
- Stocking, stems ha⁻¹.
- Top height, m.
- Either quadratic mean DBH, cm, **or** basal area, m² ha⁻¹.
- If both DBH and basal area are supplied, GROWFOR uses basal area in subsequent calculations.

For Sitka spruce, site index is calculated from age and top height and is defined using reference age 30. Yield class is reported retrospectively once final harvest is simulated. [G1, pp. 9–10]

### 3.3 Thinning controls in the user interface

A thinning can be specified by either:

- **material removed:** basal area, stocking and/or volume; or
- **desired post-thin state:** retained basal area, stocking and/or volume.

When multiple thinning fields are supplied, the manual says basal-area and stocking information are prioritised in subsequent calculations. Mean DBH and mean volume are model outputs rather than directly editable thinning fields. [G1, pp. 10–12]

The manual also provides a repeated thinning-cycle UI (fixed period, final thinning age and thinning volume). Its optional “threshold basal area” message is explicitly referenced to **Forestry Commission Booklet No. 34 / Hamilton & Christie (1971)** and is described as guidance. It is not a newly fitted GROWFOR biological threshold. [G1, p. 14]

---

## 4. Sitka state-space growth model — mathematical extraction

### 4.1 State variables

Broad & Lynch define the Sitka state vector as:

\[
\mathbf{x} = (B, N, H)^T
\]

where:

- `B` = stand basal area, m² ha⁻¹;
- `N` = stocking, stems ha⁻¹;
- `H` = top height, m.

The model is based on García's state-space formulation. [G2a, pp. 4–6]

### 4.2 Basic Bertalanffy–Richards form — G2a equation (1)

\[
\frac{d x^c}{dt}=a x^c+b
\]

The source then generalises this to a multivariate power transform. [G2a, p. 4]

### 4.3 Matrix power transform — G2a equation (2)

\[
\mathbf{x}^{\mathbf{C}}=\exp(\mathbf{C}\ln\mathbf{x})
\]

The logarithm and exponential are applied element-by-element to the vector. If `x=(B,N,H)^T`, the components of `x^C` become products of powers of `B`, `N` and `H`. [G2a, pp. 4–5]

### 4.4 Multivariate dynamic system — G2a equation (3)

\[
\frac{d\mathbf{x}^{\mathbf{C}}}{dt}
 = \mathbf{A}\mathbf{x}^{\mathbf{C}} + \mathbf{b}
\]

`A` and `C` are matrices and `b` is a vector. The fitted implementation is expressed through an eigen-decomposition rather than by publishing `A` directly in the appendix. [G2a, pp. 5–8]

### 4.5 Top-height / site equation — G2a equation (4)

With `H` in metres:

\[
\frac{dH^c}{dt}=b(a^c-H^c)
\]

Its solution is:

\[
H(t)=a\left[1-\left(1-\frac{H_0^c}{a^c}\right)e^{-b(t-t_0)}\right]^{1/c}
\]

where `H0` is top height at age `t0`. [G2a, pp. 5–6]

### 4.6 Site-index scaling — G2a equation (5)

For site index `S` defined as top height at age 30:

\[
b=-\frac{1}{30-t_0}
\ln\left(
\frac{a^c-S^c}{a^c-H_0^c}
\right)
\]

`b` is the site-indexing parameter in the preferred polymorphic model. The effect of site therefore acts as a time scaling; `C` is independent of site in this formulation. [G2a, p. 6]

### 4.7 Scaled-time empirical system — G2a equation (6)

With scaled time `τ = bt`, the empirical three-state system is written:

\[
\frac{d(B^{c_{11}}N^{c_{12}}H^{c_{13}})}{d\tau}
= a_{11}B^{c_{11}}N^{c_{12}}H^{c_{13}}
+ a_{12}B^{c_{21}}N^{c_{22}}H^{c_{23}}
+ a_{13}H^{c_{33}}+b_1
\]

\[
\frac{d(B^{c_{21}}N^{c_{22}}H^{c_{23}})}{d\tau}
= a_{21}B^{c_{11}}N^{c_{12}}H^{c_{13}}
+ a_{22}B^{c_{21}}N^{c_{22}}H^{c_{23}}
+ a_{23}H^{c_{33}}+b_2
\]

\[
\frac{dH^{c_{33}}}{d\tau}=-H^{c_{33}}+b_3
\]

The parsimonious form eventually retained imposes:

\[
a_{22}=a_{23}=b_2=c_{21}=c_{23}=0
\]

with the signs of `c22` and `a21` arranged so stocking decreases through time. [G2a, pp. 6–7, 14]

### 4.8 Analytical transition between thinnings

The transformed system is linear between thinning events. Broad & Lynch give the expected transition from state `x(t1)` to `x(t2)` as:

\[
\mathbf{x}(t_2)=
\left\{
\mathbf{a}+\mathbf{P}^{-1}
\exp\left[\mathbf{\Lambda}\,b(t_2-t_1)\right]
\mathbf{P}
\left[\mathbf{x}(t_1)^{\mathbf{C}}-\mathbf{a}\right]
\right\}^{\mathbf{C}^{-1}}
\]

where:

- `Λ` is the diagonal matrix of eigenvalues;
- `P` is the matrix of left eigenvectors;
- `A = P⁻¹ Λ P`;
- bold `a = A⁻¹ b` is the transformed-system asymptote vector;
- site enters through the time-scaling parameter `b`.

**Notation warning:** the bold asymptote vector **`a`** in this transition equation is not the same object as the scalar height-asymptote parameter `a = 47.0216` in section 5 below. [G2a, p. 8]

---

## 5. Height/site parameters and fitting evidence

Height fitting used **749 trajectories from 225 plots**. Both anamorphic and polymorphic height models were fitted; the polymorphic form had the larger log likelihood (`1318.48` vs `1261.87`). [G2a, p. 12]

The fitted global height parameters are:

| Parameter | Value | Meaning in height equation |
|---|---:|---|
| `a` | **47.0216** | asymptotic height parameter |
| `c` | **0.4571** | Richards shape parameter |
| `t0` | **−5.0 yr** | pinned time-origin parameter |
| `H0` | **0 m** | assumed height at `t0` |

Site index is represented through local parameter `b`, computed from equation (5). [G2a, p. 12]

---

## 6. Published Sitka growth-component coefficients

These are the coefficients printed in the G2a appendix. They should be tagged as **Broad–Lynch 2006 Sitka coefficients**, not automatically as the coefficients of every later GROWFOR software build; see the version caveat in section 12.

### 6.1 Thinned model — “systematic and selection thinning growth model”

\[
\mathbf{P}=
\begin{bmatrix}
1.0 & 366.9413 & -24.5087\\
6.7548\times10^{-4} & 1.0 & -1.1514\times10^{-2}\\
0 & 0 & 1
\end{bmatrix}
\]

\[
\mathbf{\Lambda}=\operatorname{diag}
(-0.367892,\,-9.118596\times10^{-2},\,-1.0)
\]

\[
\mathbf{P}^{-1}=
\begin{bmatrix}
1.3295 & -487.8635 & 26.9678\\
-8.9807\times10^{-4} & 1.3295 & -6.7016\times10^{-3}\\
0 & 0 & 1
\end{bmatrix}
\]

\[
\mathbf{C}=
\begin{bmatrix}
0.5342 & 6.9520\times10^{-2} & 0.4843\\
0 & -0.2642 & 0\\
0 & 0 & 0.4571
\end{bmatrix}
\]

\[
\mathbf{C}^{-1}=
\begin{bmatrix}
1.8718 & 0.4926 & -1.9834\\
0 & -3.7852 & 0\\
0 & 0 & 2.1878
\end{bmatrix}
\]

\[
\mathbf{a}=
\begin{bmatrix}
0\\
0.620599\\
5.812518
\end{bmatrix},
\qquad t_0=-5.0
\]

**Locator:** G2a, Appendix Box 1, PDF p. 27.

### 6.2 Unthinned model

\[
\mathbf{P}=
\begin{bmatrix}
1.0 & 15.5501 & -14.5477\\
-6.2924\times10^{-3} & 1.0 & 4.3626\times10^{-2}\\
0 & 0 & 1
\end{bmatrix}
\]

\[
\mathbf{\Lambda}=\operatorname{diag}
(-0.500102,\,4.893415\times10^{-2},\,-1.0)
\]

\[
\mathbf{P}^{-1}=
\begin{bmatrix}
0.9109 & -14.1642 & 13.8690\\
5.7316\times10^{-3} & 0.9109 & 4.3644\times10^{-2}\\
0 & 0 & 1
\end{bmatrix}
\]

\[
\mathbf{C}=
\begin{bmatrix}
0.6948 & -9.3764\times10^{-2} & 0.4711\\
0 & 8.1093\times10^{-2} & 0\\
0 & 0 & 0.4571
\end{bmatrix}
\]

\[
\mathbf{C}^{-1}=
\begin{bmatrix}
1.4393 & 1.6641 & -1.4835\\
0 & 12.3314 & 0\\
0 & 0 & 2.1878
\end{bmatrix}
\]

\[
\mathbf{a}=
\begin{bmatrix}
0\\
7.068916\\
5.812518
\end{bmatrix},
\qquad t_0=-5.0
\]

**Locator:** G2a, Appendix Box 2, PDF p. 28.

### 6.3 Growth-model fitting set sizes

G2a Table 1 reports:

- **Thinned dataset:** 1,330 growth trajectories.
- **Unthinned dataset:** 540 growth trajectories.

The retained parsimonious models were the reduced forms labelled Model 2 (thinned) and Model 6 (unthinned) in the paper's discussion. [G2a, pp. 14, 19]

---

## 7. Thinning-shock / relative-closure investigation

Broad & Lynch also tested an expanded state vector:

\[
\mathbf{x}=(B,N,H,R)^T
\]

where `R` is **relative closure / site occupancy after thinning**, bounded between 0 and 1. Immediately after thinning it is reduced to the ratio:

\[
R = \frac{B_{post}}{B_{pre}}
\]

and then is assumed to recover asymptotically toward 1. [G2a, p. 7]

The extended models were not retained as the best predictive formulation. For a very heavy thinning represented by `R=0.5` at site index 20, trial formulations took about **7.71 years** or **6.36 years** to reach `R=0.99`; nevertheless, their predictive performance remained inferior to the simpler reduced model, and the authors concluded the stand-level thinning effect was largely absent in the available data. [G2a, p. 15]

**CCF interpretation boundary:** this result is about whether an additional *stand-level post-thinning shock state* improved these aggregate growth forecasts. It is not evidence that individual retained trees do not respond to local release.

---

## 8. Stand-volume equation

### 8.1 General form

The paper starts from `V/B = f(B,N,H)` and identifies the following explicit Sitka equation by stepwise regression:

\[
\frac{V}{B}
=\beta_0+\beta_1H+\beta_2\frac{H}{\sqrt{N}}
+\beta_3\frac{NH}{B}+\beta_6\frac{B}{H}
\]

Therefore:

\[
V=B\left(
\beta_0+\beta_1H+\beta_2\frac{H}{\sqrt{N}}
+\beta_3\frac{NH}{B}+\beta_6\frac{B}{H}
\right)
\]

### 8.2 Fitted coefficients

| Coefficient | Estimate | Standard error |
|---|---:|---:|
| `β0` | **1.0013** | 0.1415 |
| `β1` | **0.5295** | 0.0106 |
| `β2` | **−1.6483** | 0.2815 |
| `β3` | **−0.0010** | 0.0001 |
| `β6` | **−0.2384** | 0.0244 |

Adjusted `R² = 0.9557`. [G2a, pp. 20–21]

### 8.3 Volume fitting data

The volume equation used **727 observations** from the Coillte database, including unthinned, systematic-thinned and selectively thinned stands. Each observation included `B`, `N`, `H` and volume to **7 cm small-end diameter (SED)**. [G2a, pp. 20–21]

---

## 9. Thinning-transition equation

### 9.1 Differential form — G2a equation (10)

\[
\frac{d\ln B}{d\ln N}=a B^b N^c H^d
\]

The left-hand side is the elasticity of basal area with respect to stocking: the percentage change in `B` produced by a percentage change in `N`. [G2a, p. 9]

### 9.2 Integrated form — G2a equation (11)

\[
\ln B
=-\frac{1}{b}
\ln\left[
B_0^{-b}-\frac{ab}{c}H^d\left(N^c-N_0^c\right)
\right]
\]

where `B0,N0` are pre-thinning basal area and stocking and `B,N` are post-thinning values. [G2a, p. 9]

### 9.3 Fitted coefficients

| Parameter | Estimate | Standard error |
|---|---:|---:|
| `a` | **10.0991** | 2.5326 |
| `b` | **0.2371** | 0.0460 |
| `c` | **0.2877** | 0.0288 |
| `d` | **0.5585** | 0.0528 |

### 9.4 Thinning fitting data and interpretation

- **459 observations** with basal area and stocking before/after thinning plus top height.
- First-thinning **systematic thinning observations were excluded** from the regression.
- The fitted transition represents the **selection component** of thinning.
- Rack-row thinning is handled in simulation by proportionate reductions in basal area and stocking. [G2a, p. 21]

The differential form has a useful composition property: two immediately consecutive stocking reductions give the same basal-area reduction as their combined reduction, all else equal. [G2a, p. 9]

---

## 10. Assortment model

### 10.1 Model family

Broad & Lynch use an **Unordered Multinomial Logistic (UNML)** model. For log category `j`:

\[
P(Y=j)=\frac{\exp(\mathbf{b}_j^T\mathbf{x}_j)}
{1+\sum_{k=1}^{J}\exp(\mathbf{b}_k^T\mathbf{x}_k)}
\]

with reference category:

\[
P(Y=0)=\frac{1}
{1+\sum_{k=1}^{J}\exp(\mathbf{b}_k^T\mathbf{x}_k)}
\]

Thinning-specific predictors modify the crop assortment when a thinning assortment is requested; they are zero otherwise. [G2a, pp. 10–11]

### 10.2 Published category definitions

- **Category 1:** proportion of volume in logs at least 3 m long with **SED ≥ 20 cm**.
- **Category 2:** proportion with **14 ≤ SED < 20 cm**.
- **Category 3:** proportion with **7 ≤ SED < 14 cm**, obtained by difference. [G2a, p. 22]

The assortment fit used **607 observations** pooled across unthinned and thinned stands. [G2a, pp. 21–22]

### 10.3 Category 1 coefficients — `SED ≥ 20 cm`

| Predictor | Coefficient |
|---|---:|
| Intercept | **27.1307** |
| `D0` | **−0.6992** |
| `1 / (D0 / 100)` | **−3.2105** |
| `D0² / 100` | **0.8237** |
| `D1 − D0` | **−3.8369** |
| `(D1 − D0)²` | **0.4853** |
| `(D1 − D0) / (H / 10)` | **3.3854** |
| `1 / ((D1 − D0) H)` | **−4.8838** |
| `H / (100 (D1 − D0)²)` | **0.1969** |
| `B0 − B1` | **0.3571** |
| `(N0 − N1) / 1000` | **−10.7710** |

### 10.4 Category 2 coefficients — `14 ≤ SED < 20 cm`

| Predictor | Coefficient |
|---|---:|
| Intercept | **−23.4200** |
| `D0` | **0.6734** |
| `1 / (D0 / 100)` | **2.1322** |
| `D0² / 100` | **−0.1435** |
| `D0 / H` | **11.9817** |
| `1 / ((D0 / 100)(H / 10))` | **−1.6478** |
| `D0² / H` | **−0.3680** |
| `D1 − D0` | **−0.5903** |
| `(D1 − D0)²` | **0.1036** |
| `B0 − B1` | **0.0961** |
| `(N0 − N1) / 1000` | **−1.8719** |

`D0,D1`, `B0,B1` and `N0,N1` refer respectively to before/after-thinning quadratic mean diameter, basal area and stocking in the thinning-assortment formulation. [G2a, pp. 11, 29]

---

## 11. Development domain and validity limits

### 11.1 What is directly supported

Broad & Lynch state that:

- the growth models give **adequate projections within the range of data used in model construction**;
- Figures 6–8 show the range/state-space region for the thinned model;
- thinned permanent-sample-plot trajectories begin **from first thinning forward**;
- unthinned permanent sample plots were first measured at **around age 20**;
- the Coillte PSP database contained **no early-growth information** suitable for constructing early-growth models. [G2a, pp. 18–24]

The supplied GROWFOR manual adds, for its illustrated **Sitka Spruce — (Thinned)** model:

- **top height < 49.3 m**;
- projections are best confined to ages **greater than first thinning (if any) to clearfelling**. [G1, p. 7]

### 11.2 What is *not* directly supplied

The sources do **not** provide a clean table of numerical lower/upper validity bounds for every combination of `B`, `N`, `H`, age and thinning history. Figures 6–10 visualize the development data, but converting their graphical envelopes into hard simulation cut-offs would be an additional modelling decision.

Likewise, the manual's visible `<49.3 m` top-height message applies to the currently active Sitka-thinned model in the screenshot. The packet does not provide equivalent projection-limit windows for every other model option.

### 11.3 Training-data character

The thinned Sitka model was fitted to trajectories from stands with:

- selection thinning;
- systematic thinning;
- line + selection thinning;
- a limited number of wider-spaced stands;
- unthinned trajectories included in the thinned dataset because all stands begin unthinned. [G2a, p. 13]

However, G3 later states that model-development data had **mainly included stands with low thinnings**, and notes that crown-thinning data would be desirable for alternative silvicultural practices. [G3, p. 10]

**Result:** the supplied data support a flexible even-aged stand model with multiple conventional thinning histories; they do not constitute direct Irish empirical validation of irregular CCF structure.

---

## 12. Version and reproducibility caveat

G3 is explicit that after model testing:

> the Sitka model's volumes were found to lack an asymptote and required further attention to fix the upper growth limits.

The same 2013 paper states that models were validated using data independent of their development data. [G3, p. 4]

This creates a critical reproducibility distinction:

- **Known exactly:** equations and coefficients printed in Broad & Lynch (2006).
- **Known operationally:** the later/manual GROWFOR application exposes Sitka thinned/unthinned models and other species models.
- **Not established by this packet:** whether the executable represented by the manual uses the exact 2006 Sitka volume specification unchanged, or a later corrected parameterisation/version.

**Implementation consequence:** any offline reimplementation should name/version the extracted model explicitly, e.g. `BroadLynch2006_Sitka`, until executable/source/version equivalence is demonstrated. Do not call such a reconstruction simply “current GROWFOR”.

---

## 13. Validation anchors

### 13.0 Broad & Lynch 2006b — empirical source-data validation anchors

| Item | Reported result | Source |
|---|---|---|
| Estate reference population | 44,217 subcompartments | G2b, p. 82 |
| Estate thinned / unthinned | 27,887 / 16,330 | G2b, p. 82 |
| Thinned site-index distribution | χ²(7) = 625.5, p < 0.001 | G2b, pp. 83–84 |
| Unthinned site-index distribution | χ²(7) = 1648.2, p < 0.001 | G2b, pp. 83–84 |
| 2003 nonresearch volume sample | 113 stands: 70 thinned, 43 unthinned | G2b, pp. 87–88 |
| Common-volume relationship test | F(4,105) = 0.8835; null not rejected | G2b, p. 89 |
| Research volume observations | 819 thinned + 425 unthinned = 1,244 | G2b, p. 83 |
| Independence of original research panel | yes | G2b, pp. 82, 87 |
| Independence of final volume-model calibration | **no** — 2006a used the 113 stands as calibration data and adjusted weights | G2a, pp. 68–70 |

These are empirical **data-quality/representativeness** results. They do not report forecast-horizon bias, RMSE, MAE, prediction-interval calibration or an untouched longitudinal evaluation of the final `B,N,H` system. Do not convert them into a model-prediction accuracy score.

### 13.1 2013 GROWFOR vs Forestry Commission yield tables

G3 compares five conifers — Sitka spruce, Norway spruce, Douglas fir, Scots pine and lodgepole pine — in **even-aged** scenarios. The same modelling approach as Sitka was adopted for the other listed species, but coefficients are not printed in the supplied paper. [G3, pp. 1, 4]

Test scenario characteristics included a 5-year thinning schedule and high yield classes; the Sitka comparison used yield class 24. [G3, p. 4]

Useful Sitka anchors:

| Scenario | FC cumulative volume | GROWFOR cumulative volume | Interpretation |
|---|---:|---:|---|
| No-thin | **1,026 m³ ha⁻¹** | **1,014 m³ ha⁻¹** | Sitka was very similar between the systems in this scenario. |
| MTI thinning | **1,135 m³ ha⁻¹** | **995 m³ ha⁻¹** | GROWFOR lower; authors judged MTI removals too severe in GF. |

[G3, pp. 5, 7]

Across species, progressively reducing thinning intensity by 7%, 14% and 21% of MMAI increased cumulative GROWFOR volume. The authors argue the FC marginal thinning intensity concept does not transfer cleanly into the site-index-based GROWFOR framework. [G3, pp. 8–10]

**Use in CCF:** these are model-to-model scenario anchors, not independent empirical validation observations.

### 13.2 2017 independent NFI evaluation — dataset envelope

G4 evaluates GROWFOR and CARBWARE using a subset of Irish NFI data restricted to **single-species even-aged plots**. For Sitka spruce:

- `N = 200` plots total;
- `107` classified as thinned, therefore `93` no-thin in the reported stratification;
- `183/200` contained occurrences of ingrowth, an important data-quality complication. [G4, pp. 3–4]

Sitka NFI1 characteristics:

| Variable | Mean | Min | Max | SD |
|---|---:|---:|---:|---:|
| Age, yr | 22.73 | 9.10 | 49.00 | 9.44 |
| Mean diameter, cm | 18.66 | 8.88 | 32.73 | 5.00 |
| Basal area, m² ha⁻¹ | 32.97 | 6.69 | 74.80 | 12.78 |
| Mean height, m | 11.93 | 4.91 | 30.80 | 4.21 |
| Stand volume, m³ ha⁻¹ | 201.30 | 11.93 | 1,010.60 | 145.66 |
| Stocking, stems ha⁻¹ | 1,662 | 240 | 4,967 | 911 |

[G4, Table 1, p. 5]

### 13.3 2017 Sitka performance statistics

| Stratum | Variable | GROWFOR Bias % | GROWFOR RMSE % | GROWFOR R² |
|---|---|---:|---:|---:|
| All Sitka (`N=200`) | Basal area | **−1.96** | **12.81** | **0.81** |
| All Sitka (`N=200`) | Volume | **−1.41** | **18.80** | **0.84** |
| R3 (`N=76`) | Basal area | −2.90 | 10.30 | 0.89 |
| R3 (`N=76`) | Volume | −3.72 | 14.32 | 0.92 |
| R2 (`N=114`) | Basal area | −1.94 | 14.00 | 0.71 |
| R2 (`N=114`) | Volume | −0.01 | 22.59 | 0.62 |
| R1 (`N=10`) | Basal area | 7.63 | 17.66 | 0.89 |
| R1 (`N=10`) | Volume | 16.81 | 23.91 | 0.88 |
| No-thin Sitka (`N=93`) | Basal area | −2.47 | 11.80 | 0.77 |
| No-thin Sitka (`N=93`) | Volume | −0.77 | 19.90 | 0.81 |
| Thinned Sitka (`N=107`) | Basal area | −1.44 | 13.86 | 0.80 |
| Thinned Sitka (`N=107`) | Volume | −1.97 | 17.80 | 0.86 |

[G4, Table 2, p. 6]

GROWFOR generally had smaller bias/RMSE than the CARBWARE version evaluated, but the authors state that **both models required improvement** and that few predictions met even `R² = 0.90`, let alone the `R² > 0.95` standard they discuss. [G4, pp. 4–7]

### 13.4 2017 validation limitations

The study's own cautions are material:

- NFI did not directly collect the top-height variable required by GROWFOR; it had to be approximated.
- Ingrowth estimates produced unrealistic stocking changes.
- Plot-size/sampling effects influenced accuracy.
- The authors could not determine how much deviation came from model error versus data error/bias.
- They explicitly state that although Bertalanffy–Richards functions **may** extrapolate beyond development data, **accuracy is not guaranteed**. [G4, p. 7]

Therefore Table 2 is a useful independent performance anchor, not proof that GROWFOR is an exact Irish-growth ground truth.

---

## 14. CCF applicability assessment

### 14.1 Directly useful for CCF

**High-value Irish benchmark evidence:**

- Sitka site/top-height trajectory shape and age-30 site-index framing.
- Aggregate pure-Sitka trajectories for basal area and stocking.
- Stand volume from `B,N,H`.
- Conventional thinning-state transitions.
- Coarse Sitka assortment proportions.
- Independent NFI bias/RMSE anchors for aggregate `B` and `V`.

### 14.2 Not supported as direct GROWFOR transfer

This packet does **not** empirically validate the following for CCF:

- individual-tree diameter increments under spatially heterogeneous release;
- explicit local crown competition coefficients;
- gap-scale or multi-layer irregular-stand dynamics;
- mixed-species occupancy or mixture productivity effects;
- natural-regeneration recruitment;
- juvenile light/browse ecology;
- irregular CCF thinning response after repeated single-tree/small-group interventions;
- Oak or Beech growth coefficients;
- climate-response functions.

G4 notes that distance-independent single-tree frameworks have been applied to mixed-species and uneven-aged stands elsewhere, but **the actual G4 validation subset is single-species/even-aged Sitka and lodgepole pine**. That literature statement is not Irish CCF validation. [G4, pp. 2–3]

### 14.3 Evidence-consistent role in CCF

A conservative use is:

> **Use GROWFOR/Broad–Lynch as a stand-level Irish comparison/validation envelope for pure even-aged Sitka. Keep CCF's spatial individual-tree mechanisms responsible for local competition and intervention consequences.**

This envelope rests on Broad & Lynch 2006a for the fitted model and on Broad & Lynch 2006b for empirical source-data/panel-data diagnosis. It does **not** amount to independent predictive validation of the final dynamic model, and it does not validate CCF's individual-tree, regeneration, mixed-species or irregular-stand behaviour.

For comparison, aggregate the CCF state to GROWFOR-compatible variables (`B,N,H,V`) and compare matched trajectories. Local trees need not reproduce stand means individually; aggregate divergence should be explainable and bounded by the intended model differences.

This is a research-derived modelling recommendation, not a newly accepted mechanic.

---

## 15. Proposed offline validation protocol for CCF — non-authoritative

No production-code change is implied by this section.

### A. Reproduce the published 2006 model

1. Implement the height/site-index equation and both 2006 growth coefficient sets in an isolated verification tool.
2. Implement volume and thinning equations exactly as published.
3. Add the published assortment equations separately.
4. Call the implementation **BroadLynch2006_Sitka** until current-executable equivalence is established.

### B. Verify the extraction before comparing CCF

Minimum checks:

- `P × P⁻¹ ≈ I` for both growth models.
- `C × C⁻¹ ≈ I`.
- Site-index equation reproduces `H(30)=S` for representative `S` values.
- Transition composition is stable across split vs combined time steps in periods without thinning.
- Thinning equation gives the same result for immediately composed stocking reductions as the source describes.
- Assortment category probabilities remain in `[0,1]` and sum to 1.

### C. Matched CCF aggregate comparisons

Use pure-Sitka fixtures only at first. Compare, by year:

- top height `H`;
- stocking `N`;
- basal area `B`;
- quadratic mean DBH derived from `B,N`;
- stand volume `V`;
- mortality / stocking loss;
- assortment proportions where compatible.

Run at least:

1. no-thin baseline;
2. one conventional thinning;
3. repeated low-intensity thinning;
4. a sensitivity matrix over CCF's current site/productivity settings.

The purpose is to identify **systematic aggregate divergence**, not to force the spatial CCF model to match every GROWFOR value.

### D. Compare against external anchors

Check whether the CCF aggregate trajectories and the reconstructed 2006 model are sensible relative to:

- G3 Sitka no-thin `1,014 m³ ha⁻¹` cumulative GROWFOR anchor under the paper's scenario;
- G3 Sitka MTI `995 m³ ha⁻¹` GROWFOR anchor, with the source's warning that MTI was too severe;
- G4 NFI Sitka age/stocking/BA/volume envelope;
- G4 all-Sitka and thinned/no-thin bias/RMSE values as context, not pass/fail thresholds.

---

## 16. Evidence gaps / next retrieval priorities

1. **Independent longitudinal validation of the final dynamic model.** Broad & Lynch 2006b has now been examined and supplies empirical source-data/panel-data validation, but neither 2006a nor 2006b reports an untouched forecast test with horizon-specific RMSE/MAE, bias, prediction intervals or equivalent diagnostics for the final `B,N,H` system.
2. **Executable/source version identification.** Determine whether the available GROWFOR executable implements the exact G2a coefficients or later fixes mentioned by G3.
3. **Species-specific coefficient documentation** for Norway spruce, Douglas fir, lodgepole pine, Scots pine, larch and Ash.
4. **Per-model projection-limit messages** from the executable/manual, not only the visible Sitka-thinned `<49.3 m` screen.
5. **Model-development/validation datasets** for non-Sitka species, especially sample sizes and stand-history ranges.
6. **Irish irregular/CCF validation data.** The supplied authors explicitly do not provide such guidance; this remains a separate evidence gap rather than something to infer from even-aged fits.

---

## 17. Project-maintenance note

The current CCF Research Index describes GROWFOR as having “five total modelled species — Sitka spruce, Douglas fir, lodgepole pine, Norway spruce and Scots pine.” The supplied GROWFOR manual's model-selection screen additionally shows **Larch** and **Ash**. The canonical Research Index should therefore be corrected in a separately authorised documentation update; this research document itself does not modify project authority.

Likewise, any future statement that “the GROWFOR equations are unavailable” should be narrowed: **the supplied Broad & Lynch 2006a paper publishes the Sitka mathematical specification and coefficients**, but current-executable equivalence and other-species coefficient sets remain unverified. Broad & Lynch 2006b supplies empirical source-data validation, not final dynamic-model predictive validation.

---

## 18. Source locators

### G1 — GROWFOR User Manual

- p. 7 — model-selection screen; projection-limit popup (`Sitka Spruce - (Thinned)`, top height `<49.3 m`, Irish data, first-thinning-to-clearfelling guidance).
- pp. 9–10 — required inventory inputs; Sitka site-index reference age 30; yield-class definition.
- pp. 10–14 — thinning controls, repeated thinning cycle and FC threshold-BA guidance.
- pp. 15–17 — timber-price, assortment and NPV interface.

### G2a — Broad & Lynch 2006a

- pp. 4–8 — equations (1)–(9), state-space growth and analytical transition.
- pp. 9–11 — thinning and assortment equations.
- p. 12 — height-model fitting and `a,c,t0` parameters.
- pp. 13–20 — fitting sets, retained growth models, observed-vs-predicted figures/state-space range.
- pp. 20–21 — volume equation and coefficients; thinning coefficients.
- pp. 22–24 — assortment categories and validity/discussion/early-growth limitation.
- p. 27 — thinned growth matrices/coefficient set.
- p. 28 — unthinned growth matrices/coefficient set.
- p. 29 — assortment coefficients.
- pp. 68–70 — the 113 nonresearch observations used as a calibration dataset; ad hoc weight modification against the calibration data.

### G2b — Broad & Lynch 2006b

- pp. 80–82 — purpose, dataset roles and the panel/cross-section distinction.
- p. 82 — 44,217 estate subcompartments (27,887 thinned, 16,330 unthinned); blocked experimental design and correlated residuals.
- p. 83 — 819 thinned and 425 unthinned research volume observations.
- pp. 83–84 — estate-versus-research site-index histograms; χ²(7) = 625.5 (thinned) and 1648.2 (unthinned), both p < 0.001; non-independence qualification.
- pp. 87–88 — 2003 nonresearch sample: 70 thinned + 43 unthinned stands.
- p. 89 — common-volume relationship F(4,105) = 0.8835, null not rejected; error-correlation and sample-tree-bias discussion.
- pp. 90–92 — limitations and implications for panel-data yield modelling.

### G3 — McCullagh et al. 2013

- p. 4 — five conifer models; same modelling approach; Sitka upper-growth/asymptote caveat; independent validation statement.
- pp. 5–8 — FC/GROWFOR cumulative-volume comparisons and thinning-intensity results.
- pp. 9–10 — Bertalanffy–Richards extrapolation caution context; low-thinning development-data statement; site index and alternative-silviculture discussion.

### G4 — McCullagh et al. 2017

- pp. 1–3 — model definitions; even-aged/single-species evaluation scope; GROWFOR and CARBWARE inputs.
- pp. 4–6 — NFI evaluation design, dataset characteristics and performance statistics.
- p. 7 — data-quality, model-performance and extrapolation limitations.

---

## 19. Evidence status for implementation planning

| Item | Evidence status from this packet | CCF use now |
|---|---|---|
| Broad–Lynch 2006a Sitka equations | Direct published specification (primary model-development/calibration source) | Reproducible offline benchmark candidate |
| 2006a Sitka coefficients | Direct published specification | Reproduce only with explicit 2006 version label |
| Broad–Lynch 2006b source-data validation | Empirical site-index representativeness, experimental-dependence and cross-sectional volume diagnosis (primary empirical data-validation source) | Source-data/panel-data validation only |
| Final dynamic model external/longitudinal validation | **Not demonstrated** | Do not claim |
| Forecast RMSE/MAE/prediction intervals | **Not reported** | Do not claim |
| GROWFOR executable equivalence to 2006 coefficients | **Unverified** | Do not assume |
| Sitka age-30 site-index concept | Strong Irish model evidence | Validation/calibration reference |
| Sitka stand BA/N/H dynamics | Irish stand-level empirical model | Aggregate validation reference |
| Sitka thinning equation | Irish conventional-thinning empirical model | Offline comparator; not direct CCF local-response rule |
| Sitka assortment coefficients | Published Irish model | Historical/technical comparator; current market/value layer separate |
| Other GROWFOR species existence | Manual + G3 support; manual shows additional larch/Ash | Correct research inventory; no coefficients yet |
| Other-species coefficients | **Absent from packet** | Do not invent |
| Irish irregular/CCF GROWFOR validation | **Absent** | Do not claim |
| Mixed-species/uneven-aged Irish validation | **Absent** | Do not claim |
| 2017 NFI performance statistics | Independent cross-sectional evaluation with substantial data caveats | External context, not longitudinal final-model validation |

**Bottom line:** the packet is sufficient to reproduce a named 2006 Sitka stand model and construct a serious CCF-vs-GROWFOR aggregate validation harness. It is not sufficient to treat GROWFOR as a validated irregular-CCF engine or to assume the 2006 coefficient set is identical to the later distributed executable.
