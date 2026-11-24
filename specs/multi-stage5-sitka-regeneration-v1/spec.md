# MULTI Stage 5 — Natural Sitka regeneration, bramble/bracken competition, selective undergrowth clearing and light browsing

**Status: APPROVED — owner approval 2026-11-23. AMENDED — owner Option A, 2026-11-23** (§4 promotion rule, Q4 and Q5; see the amendment record below). **AMENDED — owner final bounded-repair decisions, 2026-11-23** (`S5-07A` population and units, `S5-02` criterion; no rule change; see the second amendment record below). **AMENDED (documentation only) — owner record, 2026-11-24** (`S5-07A` keeps the whole-cell unit and its meaning is stated; the edge-block limitation is accepted and disclosed; the `S5-02` margins are adopted as the owner's from that date; no rule, constant, tolerance, population, control, test or code change; see the third amendment record below). The owner approved this quartet, the §4 area-consistent `2.5 m` scaling policy, and the binding Q1–Q6 resolutions recorded below. Implementation is authorised only inside an admitted D-091 lane branched from `e7c1a57d` (see `plan.md` Gate 0).

### Owner approval record (2026-11-23, binding — not re-litigated)

- Owner approved the §4 `2.5 m` area-consistent scaling policy.
- **Q1 Mast:** one annual `NextDouble()` roll on the isolated woodland-wide `STAGE5-MAST` stream, cumulative thresholds `<0.15 → 1.5`, `<0.50 → 0.4`, else `1.0`.
- **Q2 Promotion light gate:** `promotionMinimumLight = 0` — no extra light gate.
- **Q3 Token:** append the regeneration version to `AreaModelVersion` **only when `!= 0`**, so `0/0/0`, `0/0/1` and `1/1/1` tokens and hashes stay byte-identical.
- **Q4 `S5-07` tolerance:** `2.5 m / 5.0 m` ratio within `[0.75, 1.33]` across ≥20 seeds, for accepted abundance and multi-year promotion ratios; report the edge-block effect honestly. *(Amended 2026-11-23 by owner Option A: split into `S5-07A` / `S5-07B`, see the amendment record and §9. Amended again 2026-11-23, final bounded repair: the blocking `S5-07A` population is the blocks represented on both lattices, in per-hectare units of matched in-polygon area; see the second amendment record and §9. Owner record 2026-11-24: the whole-cell unit is kept, "per hectare" means per hectare of nominal matched grid area, and `S5-07A` certifies the matched population in aggregate only, with no edge-equivalence claim; see the third amendment record.)*
- **Q5 Promotion cap:** aligned `5 m` block cap (max 1 promotion per block per year). Tie-breaker: oldest `EstablishYear`, then tallest height, then deterministic `(Column, Row)`. *(Amended 2026-11-23 by owner Option A: cohort export and an area-proportional block budget are added, see the amendment record and §4.)*
- **Q6 Browse height:** `V(h)` uses pre-growth height.

Where the earlier draft text differs from these resolutions (notably the §4 block-winner order), the resolution above governs and the text below has been aligned to it.

**Correction made during the first approval, recorded explicitly (independent review finding F8).** The approval commit `85d85a8d` changed one rule line, not only status and records: the §4 "Block winner" row was corrected from the draft wording "greatest height, then oldest, then lowest `(col,row)`" (draft `6c74588e`) to "oldest `EstablishYear`, then tallest height, then lowest `(Column, Row)`", so that it matches the owner's Q5 resolution. The corrected order comes from the owner's Q5 answer. The original draft text did **not** say this; it stated the opposite priority of height and age.

### Owner amendment record — Option A (2026-11-23, binding)

**Why.** The first implementation pass (`feat/multi-stage5-sitka-regeneration-20261123-v1` @ `cb739cbd`) met every criterion except `S5-07`: accepted abundance was in tolerance, but the 2.5 m / 5.0 m ratio of multi-year promotions was 1.92–4.30 against `[0.75, 1.33]`. The independent review (`review/multi-stage5-sitka-regen-20261123-v1` @ `77b29925`, verdict NEEDS CHANGES) found no coding defect. The first-approval rule limited promotions per block **per year** but not per **cohort**, so the four 2.5 m cells of one block could turn one cohort into up to four trees. It also gave an edge block with fewer than four in-polygon cells the full budget of one.

**Owner decision, 2026-11-23: Option A.**

- **A1 Block-cohort export (Regeneration `1` only).** A promotion turns **one** band into **one** tree. The block's other represented bands of the **same cohort** are removed with it and exported into that same tree. Later cohorts may still promote in later years; that is correct behaviour, not a leak. Rule text: §4 "Cohort export".
- **A2 Area-proportional block budget, no invented constants.** `budget_block = (in-polygon cell centres in the block) × a`, with `a = (cell/5.0)²`. A full 2.5 m block has `4 × 0.25 = 1.0`, a block with 3 of 4 cells has `0.75`, and so on. At `5.0 m` a block is one cell with `a = 1.0`, so a full block has exactly `1.0` and the `5.0 m` behaviour equals the first-approval rule (owner purity criterion: `5.0 m` keeps `a = 1.0` with no edge-gate reduction). The fractional budget is realised deterministically with no damping constant, and the hard cap of one promotion per block per year stays. Rule text: §4 "Block credit".
- **A3 Split gate.** `S5-07` becomes `S5-07A` (**blocking**: identical shared light field, proved, not asserted) and `S5-07B` (**mandatory, non-blocking diagnostic**: each grid's own light). Rule text: §9 Q4 and `checklists/requirements.md`.
- **Stop rule.** If A1 and A2 as specified cannot reach `S5-07A`, the implementer stops and reports the numbers. The tolerance is not widened and no damping constant is added.

The A2 mechanism (block credit, §4) and the A1 cohort identity were chosen by the implementer within these owner constraints. They are written here before the amended code was implemented or measured.

### Owner amendment record — final bounded repair (2026-11-23, binding)

**Why.** The Option A repair (`feat/multi-stage5-sitka-regeneration-20261123-v1` @ `a3d86aa2`, handoff `MULTI-STAGE5-SITKA-REGENERATION-REPAIR-20261123.md`) left two red tests:
- **`S5-07A`.** Whole-stand promotion ratios were 1.575 (managed thinning) and 2.008 (no work). The two lattices do not cover the same boundary ground. Twelve MA-1 blocks hold in-polygon 2.5 m cells but no 5.0 m cell, because their 5 m centre lies outside every polygon. Under the shared field, those twelve blocks alone gave 1023 and 731 of the 2.5 m promotions, with nothing on the 5.0 m side to compare against.
- **`S5-02`.** The test read "suppressed" as an absolute proxy: every dense test cell in the poor-light band in year 12. That proxy was never an owner criterion. The interior's light legitimately rises as Area A self-thins.

**Owner decisions, 2026-11-23.** Neither decision changes a rule, a constant, the tolerance or the shared-light control.

- **D1 — `S5-07A` is matched-area normalised.** The blocking population is the set of aligned 5 m blocks represented on **both** lattices. These are the blocks whose 5 m centre lies inside a polygon, which is exactly the 5.0 m grid's cells. The ratios are per hectare of **matched in-polygon area**, computed from each lattice's own in-polygon cells in those blocks. Whole-stand figures and the unmatched blocks become `S5-07B` disclosures. Rule text: §9 Q4.
- **D2 — `S5-02` is comparative suppression.** The blocking claim is comparative: the dense unthinned interior never reaches promotion height, and its cohorts stay materially shorter and sparser than the managed gap across the horizon. The poor-light census becomes a printed diagnostic and is never asserted. Rule text: below.
- **Stop rule.** If `S5-07A` still misses `[0.75, 1.33]` after honest area normalisation, the implementer stops and reports the numbers. The rule, the tolerance, the population and the control are not adjusted.

**`S5-02` qualification criterion (D2).** *(The comparative criterion is the owner's D2 decision of 2026-11-23. The numeric margins in criterion 4 were chosen by the implementing worker and became the owner's binding margins only on 2026-11-24; see the provenance correction below and the third amendment record.)*

*Setup, unchanged from the first pass:*
- Two `1/1/1/1` MA-1 sessions with the same seed sources. The gap session fells every living Area A tree within 7.5 m of (20, 15) in year 0 (a managed gap); the dense session stays unthinned.
- The test cells are the Area A cells whose centre is within 5 m of (20, 15), read in both sessions.
- The horizon is 15 years. Light is the modelled index, not calibrated PAR.

*Blocking criteria:*
1. **The gap raises the light index.** In year 1, the mean light of the test cells is higher in the gap by more than 0.2. This is unchanged from the first pass, and its margin is not re-derived here.
2. **The dense interior never reaches promotion height.** In no year from 1 to 15 does any band of a dense test cell reach 3.5 m (§3.3), and no dense test cell promotes.
3. **The order never inverts.** In every year from 1 to 15:
   - the dense interior's tallest cohort is no taller than the tallest cohort the gap has reached (a gap cohort that has promoted counts at its promotion height);
   - the dense standing abundance is no larger than the gap's.
4. **Materially shorter and sparser.** In year 12 and in year 15:
   - the gap's tallest reached cohort is at least **0.315 m** taller than the dense interior's tallest;
   - the dense standing abundance is at most **0.8 ×** the gap's.
5. **The gap grows toward promotion.** In year 12, the gap's tallest cohort is at least 1.5 m, the bramble/bracken escape height of §3.1. This is unchanged from the first pass.

*Why these margins follow from the spec, not from observed data.* They were written here before the final-repair evidence run measured the `S5-02` criterion; see the provenance correction below for what the implementer had already read.

> **Provenance correction (2026-11-24).** The sentence above originally read: "They are fixed here before this pass measured anything." Placed inside the 2026-11-23 owner-decision record, it presented the margins as an owner record. They were not. The implementing worker chose 0.315 m, 0.8 × and years 12 and 15 after reading the previous handoff, which reported the gap's 3.24 m tallest band in year 12 and the 7/12 poor-light census. The owner had not seen the margins when that text was written. The owner adopted them on 2026-11-24 (third amendment record, R3); they are the owner's binding margins from that date, not before.
- **0.315 m = 0.35 × 0.90.** This is the largest annual height increment the §3.2 growth rule allows, because LightResponse peaks at 0.90. A smaller lead could come from cohorts established one year apart, so it would not show suppression. A lead of at least one maximal year of growth cannot come from that.
- **0.8** is the §3.2 poor-light survival factor, the spec's own one-year suppression penalty. "Materially sparser" means the interior holds no more than the gap would hold after one year of that penalty.
- **Year 12** is the first year in which a cohort established in year 1 can reach promotion height at the fastest growth the spec allows: `0.15 + 11 × 0.315 = 3.615 m ≥ 3.5 m`, while year 11 gives only 3.30 m. **Year 15** is the end of the horizon.
- *Standing abundance* is not credited with abundance a gap cohort exported by promotion. Criteria 3 and 4 are therefore conservative in the dense interior's favour.

*Diagnostic only (printed, never asserted):* the poor-light census, i.e. how many dense test cells have `LightResponse(light) < 0.15` in each year, including year 12.

> **Superseded proxy (first pass and Option A repair test, never an owner criterion), kept for history.** "In year 12, when the cohorts are compared, every dense test cell is in the poor-light band (LR < 0.15, survival × 0.8)." The repair run measured 7 of 12.

### Owner amendment record — owner record and limitation disclosure (2026-11-24, binding; documentation only)

**Why.** The final bounded repair (`feat/multi-stage5-sitka-regeneration-20261123-v1` @ `5b757d9b`) is green at 209/209, and its delta review is APPROVED. The owner then reviewed three things: the `S5-07A` result on three area bases, the matched-edge figures, and where the `S5-02` margins came from. This record writes down what the owner decided, so the spec states what the gate certifies and what it does not. It changes no rule, constant, tolerance, population, control, test or code. Every protected hash is untouched, including the `1/1/1/1` fingerprint `c4b670ed…`.

**R1 — `S5-07A` keeps the whole-cell unit (owner decision, 2026-11-24).**
- *The gate does not change.* It covers the matched population: the aligned 5 m blocks represented on both lattices, 101 on MA-1. It uses whole-cell units: in-polygon 2.5 m cell centres × 6.25 m², against 5.0 m cells × 25 m², per hectare. Both the pooled ratio and the mean ratio, for both metrics in both scenarios, must lie in `[0.75, 1.33]`.
- *Alternatives rejected.* The owner considered two other blocking units and rejected both:
  - each grid's own clipped polygon area;
  - the same true-ground area for both grids.
- *What "per hectare" means here.* The whole-cell denominator counts **nominal grid area**, not forest area. A 5.0 m cell counts 25 m² even where part of it lies outside the polygon. On MA-1 the 5.0 m denominator is 2525 m², but those cells clip to 2270.06 m² of polygon. In `S5-07A`, "per hectare" therefore means per hectare of **nominal matched grid area**. The 2026-11-23 wording "matched in-polygon area" (D1 and §9 Q4) overstated this, and §9 Q4 now says so.
- *Why the owner kept it.* Of the three bases, the whole-cell unit gives the highest promotion ratios. That makes it the conservative basis for promotions, the metric nearest the upper bound. It is not the conservative basis for accepted abundance: on the other two bases, abundance falls to 0.855–0.879. That is still inside the tolerance.

*Measured comparison (pooled 2.5 m / 5.0 m ratio; shared light field; matched population; 20 seeds; 30 years; MA-1; evidence run on `b5eadd19`, reproduced unchanged at `5b757d9b`):*

| Basis | Thinning promo | No work promo | Thinning abundance | No work abundance |
|---|---|---|---|---|
| whole-cell (gate) | 1.247 | 1.181 | 0.956 | 0.952 |
| own clipped area | 1.146 | 1.085 | 0.879 | 0.875 |
| same true ground | 1.120 | 1.061 | 0.859 | 0.855 |

Where each row comes from:
- *Whole-cell row.* This is the gate, printed and asserted by the test.
- *Same-true-ground row.* The test prints this row under the label "raw-count ratio … (not area-normalised, not gated)". Both lattices' matched blocks cover the same ground, so dividing each count by that one area cancels out. The raw-count ratio therefore **is** the same-true-ground per-hectare ratio.
- *Own-clipped-area row.* These are the printed totals divided by the printed clipped areas, 2219.88 m² and 2270.06 m². That is arithmetic on test output; the test does not print these ratios.

**R2 — Boundary limitation, accepted by the owner (2026-11-24).** `S5-07A` certifies the matched population **in aggregate only**. Matched edge blocks are **not** equivalent across the two grids under the Option A rule, and this spec makes **no edge-equivalence claim**. Nobody may quote the aggregate `S5-07A` pass as evidence that the grids agree on edge ground.
- *Edge promotions are out of tolerance in every run.* Matched edge promotions per nominal hectare are **1.902** (managed thinning) and **1.841** (no work). All 40 seed-runs are above 1.33: the per-seed minimums are 1.745 and 1.683.
- *Edge blocks carry most of the promotions.* The 24 matched edge blocks are 15–24 % of the gated area (343.75 of 2268.75 m² at 2.5 m; 600 of 2525 m² at 5.0 m). They carry 44–69 % of the promotions (managed thinning 1106 / 2520 and 1015 / 2249; no work 561 / 819 and 532 / 772). The aggregate passes because the interior dilutes the edge excess; it does not resolve it.
- *The mechanism is arithmetically identified.* A promotion exports only the in-polygon cells' abundance, but it always creates one whole tree. The per-block credit (§4) rarely binds, because blocks run out of cohorts before they run out of credit. On average a matched edge block has `n = 55 / 24 ≈ 2.29` in-polygon 2.5 m cells, so the whole-cell area factor is `4/n = 600 / 343.75 = 1.745`. The two grids promote about the same raw number of edge trees, 1106 / 1015 = 1.090. Hence `4/n × raw edge ratio = 1.745 × 1.090 = 1.902`. For no work: `1.745 × 561 / 532 = 1.841`.
- *No area basis makes both edge metrics clean.*
  - With the whole-cell unit, edge abundance looks exact (0.991 / 1.036, i.e. 0.99–1.04), and edge promotions are about 1.9.
  - With ground-based units (own clipped area or same true ground), edge promotions fall to 1.06–1.30, but edge abundance drops to 0.57–0.71.

  Changing the unit moves the discrepancy between the metrics; it does not remove it.

**R3 — `S5-02` margins adopted by the owner (2026-11-24).** The owner adopts these as binding `S5-02` margins, from 2026-11-24:
- a 0.315 m height lead;
- dense abundance at most 0.8 × the gap's;
- both compared in years 12 and 15.

The owner adopted them after review confirmed two things:
- *They are spec-derived:*
  - 0.315 = 0.35 × 0.90 × 1.0, one maximal year of §3.2 growth at site productivity 1.0;
  - 0.8 = 1 − 0.2, the §3.2 poor-light survival factor;
  - year 12 is the first year with 0.15 + 11 × 0.315 = 3.615 ≥ 3.5.
- *They sit 2–5× away from the observed data.* In year 12 the lead is 1.509 m against 0.315 (4.8×), and the abundance ratio is 0.349 against 0.8 (2.3×). For completeness, year 15 is 1.307 m (4.1×) and 0.446. That abundance margin is 1.8×, slightly under 2×.

*Provenance, stated honestly:*
- The implementing worker first chose these margins after reading the previous handoff, which gave the gap height of 3.24 m in year 12 and the 7/12 poor-light census.
- The earlier spec text presented them inside the 2026-11-23 owner-decision record, as if they were an owner record, before the owner had seen them.
- This amendment makes them the owner's from 2026-11-24. It does not imply that they were the owner's beforehand.

The superseded sentence is kept visible in the provenance correction under the second amendment record.

**Unchanged by this record:**
- the rules, constants, tolerance `[0.75, 1.33]`, seeds, horizon and scenarios;
- the matched population and the shared-light control and its proof;
- `S5-07B` (still non-blocking);
- the test code (including its "raw-count" label, an open cosmetic item) and all production code.

Task class: `SPEC REQUIRED` (new causal regeneration, understorey competition, browsing, promotion, one new Work Plan command).
Lane: standalone 2D Sitka Engine, `Tools/MultiRackSitkaP1Lab` only. **Zero** Unity `Assets/`, Compact30/Legacy40/Enlarged80/Reference Future v1, `/multi` UI, or save-schema change.

## 0. Source stamps (read-only audit, 2026-11-23)

| Item | Commit |
|---|---|
| Implementation base (owner-chosen) | `feat/multi-stage4-ui-catchup-20261123-v1` @ `e7c1a57da519ed1ee03ccfbad240196a66df0905` |
| Stage 4 Core (ancestor of base) | `feat/multi-stage4-adult-parity-20261123-v1` @ `500a50aea4bd049f231f05d73828fabdc0a7a85c` |
| Unity behaviour source (pinned formulas) | `main` @ `5e79e98621f1dff1ed388f6be55f94f78b3d8fe0` |
| `origin/main` at drafting | `bc8829d7de003e879c58c74662af02442516d555` |
| Owner UX R3 (deliberately NOT the base) | `0e777455…` — UI-only, in review |
| D-094 / D-095 approvals | `docs/multi-899-900-approved-spec-20261123-v1` @ `3238846d1dc1e68b16e7e49e24de34490af8ded2` (not yet on `main`) |

`e7c1a57d` and `500a50ae` are **not** ancestors of `origin/main`; Stage 5 therefore branches from `e7c1a57d`, not `main`.

## 1. Locked owner decisions (not re-litigated here)

1. Engine-first: pure-.NET engine + automated tests. `/multi` stays on Stage 4 (`1/1/1/0`); Core default stays `0/0/0/0`. No new UI, no save-schema change.
2. `ClearUndergrowth` is selective: it zeroes bramble and bracken on targeted cells and **keeps existing Sitka cohorts**. This is an intentional divergence from Unity's destructive whole-cell `ScenarioWorkType.RemoveRegeneration`.
3. One `2.5 m` ground-cell grid for light, undergrowth, cohorts and clearing, with an explicit area-consistent scaling policy (§4) proved by `S5-07`.
4. Promotion into `CrownModel 1`, strictly inside the clipped area polygon.
5. Connected-woodland rules: centre-in-polygon cells, adult-empty racks still advance, one shared annual mast roll, cross-rack seed rain, zero seed from outer grassland.
6. Version switch, neutral site, exact Unity browsing rules, atomic immutable state.

## 2. Version switch

```text
MultiAreaRegenerationModelVersion { Off = 0, SitkaCohortCompetitionBrowse = 1 }
```

Selection order is `Adult / Mortality / Crown / Regeneration`. Supported: `0/0/0/0`, `0/0/1/0`, `1/1/1/0`, `1/1/1/1`. Regeneration `1` requires `1/1/1` (it needs the Stage 4 adult step, the refreshed `2.5 m` decoupled light and `CrownModel 1`); every other combination is refused with a typed `P1Refusal`.
Regeneration `0` must produce **byte-identical** output to today's Stage 4 for every supported prior selection.

Honesty labels: all constants below are `GAMEPLAY CALIBRATION` or `TRANSFER` (Unity gameplay values). They are **not** measured Irish establishment probabilities and light is **not** calibrated PAR. Planting or regenerating trees is not automatically beneficial; the outputs are consequences of the stated simulation rules only.

## 3. Annual sequence at `1/1/1/1`

Steps 1–5 are the existing Stage 4 adult step, unchanged (woodland-wide 8 m Hegyi CI → adult growth → `CrownModel 1` relaxation → per-rack Comeau RD mortality and fallen deadwood → refreshed `2.5 m` decoupled light, read from the start-of-year root so area order cannot matter).

| # | Step | Rule |
|---|---|---|
| 5a | Undergrowth | per cell: `T = 0.8·clamp01(siteProductivity·soilStability)·(0.1+0.9·clamp01(light))`; year-0 `bramble = bracken = 0.25·T`; annual `cover' = clamp01(lerp(cover, T, T>cover ? 0.30 : 0.45))`. `siteProductivity = soilStability = 1.0`. |
| 6 | Grow/survive existing cohorts | see §3.2 |
| 7 | Mast | one woodland-wide roll `r = NextDouble()` on stream `STAGE5-MAST` for the year: `r < 0.15 → good, 1.5`; `0.15 ≤ r < 0.50 → poor, 0.4`; `r ≥ 0.50 → normal, 1.0` (verified against Unity `UpdateMast`: good probability 0.15, poor probability 0.35 added cumulatively, else normal). |
| 8 | Seed rain | per in-polygon cell centre `c`: `S_c = Σ_i P_i·exp(−d_ic/20)`, over living mature Sitka `i` in **all** connected racks with `d_ic ≤ 150 m`. `M(age)=0 (≤20), (age−20)/10 (20<age<30), 1 (≥30)`; `P_i = M·clamp01(r_i/4.0)·1.0·mast`, `r_i` = realised `CrownModel 1` radius. Outer grassland contributes zero. |
| 9 | Establishment | `seedFactor = 1−exp(−S_c/10)` (`S50 = 10`, as in `SitkaSpruce.asset`); `establishment = seedFactor·LightResponse(light_c)·suitability_c`; skip if `≤ 0.01`, else `requested = 0.5·establishment` scaled per §4. |
| 10 | Promotion | §3.3 and §4; then refresh canopy/light cache. |
| 11–12 | Neutral suitability / diagnostics | `suitability_c = 1.0` (no opening-decay or soil map is authored; stated, not invented). Emit read-only per-area annual accounts. |

`LightResponse(light)`: piecewise-linear through `(0,0) (0.10,0.10) (0.20,0.30) (0.25,0.50) (0.55,0.90) (1.00,0.80)`.

### 3.1 Bramble and bracken survival penalty

`p_veg = clamp01(0.35 · max(bramble, bracken) · clamp01((1.5 − preGrowthHeight)/(1.5 − 0.3)))` — full at `≤ 0.3 m`, zero at `≥ 1.5 m`. No graminoids, no browse concealment.

### 3.2 Juvenile growth, survival and browsing (exact Unity `JuvenileEcologyRules`)

```text
dH_light      = 0.35 · LightResponse(light) · siteProductivity
survival_light = 0.8 if LightResponse(light) < 0.15 else 1.0
V(h)  = 1 (h ≤ 0.5);  1 − (h − 0.5)/0.3 (0.5 < h < 0.8);  0 (h ≥ 0.8)
p     = min(0.95, pressure · 0.15 · V(h))        (≤ 0.03 at pressure 0.2)
dH    = dH_light · (1 − 0.90·p)
survival = survival_light · (1 − 0.04·p) · (1 − p_veg)     (double precision)
```

`pressure = 0.2` default; `pressure = 0` must take the exact non-browse arithmetic path and be bit-identical to light-only. No stem-form-damage variable, no shelters or fencing in Stage 5. Species constants: initial cohort height `0.15 m`, promotion height `3.5 m`, `regenDensityMax = 1.5`, `RepresentationThreshold = 0.01`, max 4 represented age bands plus one sub-threshold accumulator per cell (band merge/accumulator rules are **ported verbatim** from Unity `ForestEcologyController` @ `5e79e986`, not re-designed).

### 3.3 Promotion into `CrownModel 1`

The oldest qualifying represented band (`height ≥ 3.5 m`) becomes **one** adult Sitka and the band is removed (amended 2026-11-23: the block's other represented bands of the same cohort are exported into the same tree, and the block must hold promotion credit; §4):

```text
Age    = max(1, ecologicalYear − establishYear)
Height = cohort.Height
DBH    = clamp(1.5 · Height, 2, 20) cm
r_0    = max(0.1, r_pot(DBH) / sqrt(1 + CI_woodland/5))        CrownModel 1, NOT open-grown r_pot
```

`CI_woodland` is the connected-woodland Hegyi index (8 m) evaluated at the promoted stem. The light gate `PromotionMinimumLight` exists in Unity code but is **not serialised in `SitkaSpruce.asset`** (reads as 0, i.e. inactive); Stage 5 therefore applies no light gate — **Q2 resolved by owner: `promotionMinimumLight = 0`, no extra light gate**.
`TreeId = "R{year}-{areaId}-{col}-{row}"`, asserted unique against all existing IDs. Position: deterministic jitter inside the source cell from stream `STAGE5-PROMO-{areaId}-{col}-{row}-{year}`; if the point is outside the clipped area polygon, retry a fixed number of times, then fall back to the cell centre (centre-in-polygon is guaranteed by cell membership). A tree is never placed outside the polygon.

## 4. Area-consistent `2.5 m` scaling policy (APPROVED by owner 2026-11-23)

**Source facts (verified):** Unity cohort `Density` is an *abstract abundance per cell*, not a stem count ("Not a stem-count conversion", `ForestEcologyController`). Occupancy is `Density / RegenDensityMax`, shared capacity per cell is 1 (`ForestEcologyCell.AdmitDensity`). Promotion exports a whole band to **one** tree, at most one band per cell per year. All of this is defined for a `5 m` cell (`25 m²`). Copied unchanged onto `2.5 m` cells (`6.25 m²`), the same woodland would admit 4× the abundance per hectare and could promote 4× the trees per hectare per year.

Let `cell = 2.5` or `5.0` m and `a = (cell/5.0)²` (so `a = 0.25` at 2.5 m, `a = 1` at 5.0 m — the 5.0 m run therefore reproduces Unity's own scaling exactly).

| Quantity | Policy | Why |
|---|---|---|
| Requested density per event | `requested · a` | density is extensive (per cell) |
| Cell capacity | `RegenDensityMax · a` (1.5 → 0.375) | keeps **relative** occupancy (0–1) and abundance per hectare equal |
| Representation threshold | `0.01 · a` | a band of the same relative size stays represented |
| Promotion budget (**amended, Option A2**) | each aligned 5.0 m block (blocks = `floor(x/5), floor(z/5)` in the world frame) earns `budget_block = n_block · a` promotions per year, where `n_block` is the number of in-polygon cell centres of the block over all areas; **at most one promotion per block per year** (hard cap). Realised by the integer block credit below | trees/ha/yr ceiling proportional to in-polygon area; a full block, and every 5.0 m cell, keeps Unity's ceiling of exactly 1 |
| Block winner | oldest `EstablishYear`, then tallest height, then lowest `(Column, Row)` (owner Q5 resolution) | deterministic |
| Cohort export (**amended, Option A1**) | when a block promotes, every other cell of the block loses its represented band of the winner's cohort into the same tree; bands of other cohorts stay and can promote in later years | one tree per cohort per 5 m block, as one 5 m cell does |
| Growth, survival, seed rain, light, undergrowth | unchanged (all intensive) | per-point or per-plant rates |

Supported grids remain `{2.5, 5.0}` only (the existing `CanopyLightEvaluator` rule).

**Block credit (Option A2): integer and exact.** Let `C = (5.0 / cell)²` be the number of cells in a full block (`C = 4` at 2.5 m, `C = 1` at 5.0 m), so `budget_block = n_block / C`. Each block carries an integer credit `K`, counted in cell-years, with `K = 0` at year 0. In step 10 of every year, for every block:

1. `K ← K + n_block` (this year's budget accrues).
2. If the block has at least one qualifying candidate and `K ≥ C`, its winner promotes (one tree at most, the hard cap) and `K ← K − C`. Otherwise nothing in that block promotes or is exported this year.
3. `K ← min(K, C)`. Unspent credit never exceeds one promotion, which is the unit of the hard cap.

*Long-run rate.* A block that always has a candidate promotes exactly `n_block / C` times per year: every year for a full block, 3 years in every 4 for `n_block = 3`, every second year for `n_block = 2`, and 1 year in 4 for `n_block = 1`. In any `T` consecutive years a block promotes at most `1 + ⌊n_block · T / C⌋` times. A full block (`n_block = C`, which includes every 5.0 m cell) always holds `K ≥ C` after step 1, so its budget never binds. At 5.0 m a block is a single cell, so cohort export removes nothing either: the 5.0 m behaviour is exactly the first-approval rule, with `a = 1.0` and no edge-gate reduction (`S5-07A` checks this against a fingerprint measured on the first-pass engine). The arithmetic is integer: there is no fractional tree, no random draw and no constant other than `C`, which the grid geometry fixes. The credit is session state in `MultiAreaStage5State` and is never saved.

**Cohort identity and no double counting (Option A1).** A band's cohort is its `EstablishYear`. Within one cell the represented bands have unique establishment years: this is the ported Unity `NormalizeBands` invariant, and a merged band carries Unity's abundance-weighted, half-up-rounded year. A cohort therefore has at most one represented band per cell. When block `b` promotes the winner's band of cohort `Y`:

- the winner's band becomes the one tree and is removed; its height and establishment year set the tree as in §3.3 (unchanged);
- every other in-polygon cell of block `b`, in any area, removes its represented band with `EstablishYear = Y` if it has one, whether that band qualifies or not. Its abundance is exported into the same tree and booked as promotion export of that cell's own area;
- sub-threshold accumulators (which are not represented bands) and the bands of every other cohort are untouched. They may promote in later years, subject to the block credit.

There is no double counting. Each band lives in exactly one cell and is removed at most once. One promotion per block per year means one tree per exported cohort. The per-area account identity `starting − losses + accepted − exported = ending` holds in every year and is asserted. A later merge in a sibling cell can produce a band that again carries year `Y`; that band holds abundance that was never exported, so its later promotion is a new export, not a second count. A sibling band of the same cohort whose year was shifted to `Y ± 1` by an earlier merge is not matched. This is an accepted approximation of the cohort match, and `S5-07A` measures its consequence.

> **Superseded wording (first approval, 2026-11-23), kept for history.** Promotion budget: "**≤ 1 promotion per aligned 5.0 m block per year**, blocks = `floor(x/5), floor(z/5)` in the world frame — caps trees/ha/yr at Unity's ceiling without fractional trees". Block winner rationale: "deterministic; non-winners stay and can promote next year". "Known approximation: an edge `5 m` block with fewer than four in-polygon cell centres still gets the full budget of 1. `S5-07` measures this; it is not hidden." Option A replaced this because it limited trees per block per year but not per cohort, and it gave edge blocks a full budget.

## 5. `ClearUndergrowth` Work Plan command

- Order = `(AreaId, Column, Row)` of one existing in-polygon `2.5 m` ground cell. New order record alongside the Stage 4 `MultiAreaPruneOrder` pattern; the existing tree `WorkAction`/`MultiAreaWorkOrder` shapes do not change.
- Lifecycle is the existing fenced one: `Pending → Approve → Execute`, engine-minted execution key, idempotent replay, atomic all-or-nothing revalidation against the fence (`SessionId`, `MultiAreaSourceToken`, `PlanStateRevision`), zero calendar advance, zero mutation before Execute.
- Execute sets `bramble = bracken = 0` on the targeted cell(s) and leaves every Sitka cohort untouched. No cost: there is no economy in this stage.
- Stale, duplicate, out-of-polygon, unknown-area or non-existent-cell targets refuse the whole batch.
- **Caveat stated to the player model:** clearing is temporary. Next year step 5a regrows cover toward `T` at rate `0.30`; the window is one growing step of reduced `p_veg`.

## 6. RNG isolation

`SimulationRandom` (`RngModelVersion = 1`) with new keys only: `STAGE5-MAST` (woodland, per year), `STAGE5-PROMO-{areaId}-{col}-{row}-{year}`. Stage 5 must never draw from, or reorder, `"ADULT-MORT-" + TreeId`. Seed rain, establishment, growth and browsing are deterministic functions (no RNG). Exact `SimulationRandom.Roll` call shape is confirmed in plan Stage 1.

## 7. State and atomic commit

New immutable `MultiAreaStage5State` carried as one more optional member of `MultiAreaRoot` (`= null` on every model-0/Stage 1–4 root, the same pattern as `Stage4`). It holds per-area cells (undergrowth covers, cohorts, sub-threshold accumulator), the per-block promotion credit (§4, amended 2026-11-23), pending/approved/executed clear orders, annual accounts and the Stage 5 options (`BackgroundBrowsePressure`, grid). One reference assignment publishes it with the rest of the root; a simulated pre-commit failure must leave the previous root intact. Session-only: nothing here is saved. How the regeneration version enters `MultiAreaSourceToken` without changing earlier tokens is **Q3, resolved by owner**: it is appended to `AreaModelVersion` only when `!= 0`.

## 8. Adult-empty racks

An area with 0 living adults still: advances its ecological year, receives cross-rack seed, updates undergrowth, and grows/promotes its cohorts. Whether Stage 4 currently short-circuits such an area is verified in plan Stage 1; if it does, the fix is confined to `Regeneration = 1` paths.

## 9. Questions put to the owner (all resolved 2026-11-23 — see the approval record at the top)

- **Q1 Mast bands — resolved from source, please confirm.** The brief's `0.15 / 0.35 / 0.50` are probabilities/cumulative edges, not three sampled rolls: Unity gives good = 15 % (×1.5), poor = 35 % (×0.4, roll in `[0.15, 0.50)`), normal = 50 % (×1.0). Expected multiplier = 0.225 + 0.14 + 0.5 = 0.865. Unity rolls mast per species with the stand RNG; Stage 5 uses the isolated `STAGE5-MAST` stream per year instead (intentional, §6).
- **Q2 Promotion light gate.** No gate (Unity asset leaves it at 0). Confirm.
- **Q3 Token.** Proposal: add the regeneration version to the existing `AreaModelVersion` string only when `≠ 0`, leaving the record shape and all earlier tokens byte-identical.
- **Q4 `S5-07` tolerance — AMENDED by owner Option A, 2026-11-23 (split gate).** The draft proposal ("ratio of 2.5 m / 5.0 m for (a) mean relative occupancy, (b) cumulative promotions per hectare over 10 years lies in `[0.75, 1.33]`") is superseded. Its **10-year horizon was stale** (review finding F7): no band can reach 3.5 m within 10 years (`0.15 + 10 × 0.35 × 0.9 = 3.30 m`), so a 10-year promotion ratio is `0/0`. **The comparison horizon is 30 years.** Both parts use the same ≥ 20 deterministic woodland-mast seeds, 30 years, and two scenarios on MA-1: "managed thinning" (every second living stem felled in year 0) and "no work". The metrics are cumulative accepted establishment abundance (cohort abundance accepted into bands) and cumulative adult promotions. `S5-07A` measures them over the matched-area population below; `S5-07B` reports them over the whole stand (interior and edge blocks together) and over that population. *(Amended 2026-11-23, final bounded repair D1. The Option A text said "over the whole stand (interior and edge blocks together)" for both parts.)*
  - **`S5-07A` — BLOCKING (amended again 2026-11-23, final bounded repair D1).**
    - *Population.* The aligned 5 m blocks represented on **both** lattices: those whose 5 m centre `((X + 0.5) · 5, (Z + 0.5) · 5)` is strictly inside an area polygon, under the lattices' own centre-in-polygon rule. These are exactly the cells of the 5.0 m grid, 101 on MA-1. The test **asserts** this correspondence instead of assuming it:
      - every 5.0 m block holds exactly one 5.0 m cell;
      - the set of 2.5 m blocks with an in-polygon 5 m centre equals the set of 5.0 m cells;
      - every interior block is matched.
    - *Units.* Per hectare of **matched in-polygon area**. Each lattice's area comes from its own in-polygon cells in the matched blocks:
      - `A_2.5 = Σ n_block × 6.25 m²`, because a matched block holds 1–4 in-polygon 2.5 m cells;
      - `A_5.0 = (number of matched blocks) × 25 m²`.

      Both totals are reported and asserted to be > 0. *Accepted abundance per hectare* is the abundance accepted into bands in the matched blocks' cells over 30 years, divided by that lattice's matched area. *Promotions per hectare* is the number of promotions whose block is matched, divided by that lattice's matched area.

      *Owner record, 2026-11-24 (third amendment record, R1).*
      - These are **whole-cell units**, and the owner keeps them as the gate. "Per hectare" here means per hectare of **nominal matched grid area**, not forest area. A whole 5.0 m cell counts 25 m² even where part of it lies outside the polygon (2525 m² nominal against 2270.06 m² clipped on MA-1).
      - The owner considered and rejected two other blocking units: each grid's own clipped area, and the same true-ground area.
      - The whole-cell unit is the conservative basis for the promotion result, which is why the owner kept it.

      *Superseded label, kept for history:* "Per hectare of **matched in-polygon area**", as in the *Units* heading above. The formulas are unchanged; only the label overstated what the 5.0 m denominator measures.
    - *Scope — aggregate only (owner-accepted limitation, 2026-11-24; third amendment record, R2).* `S5-07A` certifies the matched population **in aggregate only**. Matched edge blocks are not equivalent under the Option A rule: their per-nominal-hectare promotion ratios are 1.902 / 1.841, and all 40 seed-runs are above 1.33. No edge-equivalence claim is made, and the aggregate pass must not be quoted as one.
    - *Gate.* Both of these lie in `[0.75, 1.33]`, for both metrics in both scenarios:
      - the pooled ratio `(Σ_s x_2.5,s / A_2.5) / (Σ_s x_5.0,s / A_5.0)`;
      - the mean over seeds of `(x_2.5,s / A_2.5) / (x_5.0,s / A_5.0)`.
    - *Unchanged:*
      - zero out-of-polygon promoted trees over every study run (whole stand, both lattices);
      - `5.0 m` keeps `a = 1.0` with no edge-gate reduction (its output equals the first-pass engine's);
      - no invented damping constant;
      - the shared light field below, and its proof.
    - *Printed, not gated:* whole-stand ratios and the matched **same-true-ground** ratios. *(Label corrected 2026-11-24. Previous wording: "raw-count matched ratios". The test output still labels them "raw-count ratio … (not area-normalised, not gated)". Both lattices' matched blocks cover the same true ground, so the raw-count ratio is the same-true-ground per-hectare ratio. The test string is unchanged, an open cosmetic item, because this amendment does not edit test code.)*
    - *Superseded wording (Option A, 2026-11-23), kept for history:* "Under an identical shared light field, both the **pooled** ratio (Σ 2.5 m / Σ 5.0 m over the seeds) and the **mean** of the per-seed ratios lie in `[0.75, 1.33]` for both metrics in both scenarios; pooled and mean must both pass. In addition: zero out-of-polygon promoted trees over every study run; `5.0 m` keeps `a = 1.0` with no edge-gate reduction (its output equals the first-pass engine's); and no invented damping constant." That wording compared raw whole-stand totals.

    *Shared light field.* For each scenario, the field is the light index of the **regeneration-free adult canopy**: the post-adult-step roster, in that year, of a `1/1/1/0` run of the same woodland and scenario. It is evaluated with the unchanged `CanopyLightEvaluator` kernel at the centre of each aligned 5 m block, and every cell of both grids reads its block-centre value (a 5.0 m cell's centre is its block centre). *Frozen or excluded, in both grids alike:* the shading of Stage 5 promoted trees on the regeneration layer. Each run keeps its own adult step, seed rain and promoted adults. *Proof, not assertion:* the test asserts bit-equal light at every block present in both lattices in every year of every seed and scenario. It also asserts that its year loop reproduces `MultiAreaContinuation.Advance` bit for bit when it is given the model light, so the light source is the only difference.
  - **`S5-07B` — MANDATORY, NON-BLOCKING DIAGNOSTIC.** The same comparison under each grid's own `CanopyLightEvaluator` field: cell-centre sampling of the run's own canopy, including its own promoted trees. There is no `[0.75, 1.33]` threshold and no assertion, so it can never fail the suite. Disclosed: the real-light accepted-abundance and promotion ratios (pooled, mean, and per-seed minimum/median/maximum); interior-only and edge-only sub-ratios for both light bases (a 5 m block is interior when all four of its 2.5 m cell centres are in polygon; a 5.0 m cell is interior when its block is); and the statement that light is a non-PAR modelled index (`unity-k10a10-decoupled-v1`), so a higher 2.5 m real-light recruitment reflects finer gap-resolution sampling of that index rather than a scaling defect.

    *Unmatched-block disclosure, added 2026-11-23 (final bounded repair D1).* `S5-07B` also reports the blocks represented on the 2.5 m lattice only, which number 12 on MA-1. For both light bases it gives:
    - their count;
    - their in-polygon 2.5 m area;
    - their promotions and accepted abundance.

    It states explicitly that the whole-stand 2.5 m figure exceeds the 5.0 m figure partly because the finer lattice represents boundary ground the coarser lattice cannot.
- **Q5 Block cap — AMENDED by owner Option A, 2026-11-23.** First approval: aligned `5 m` block cap, at most one promotion per block per year, tie-breaker oldest `EstablishYear`, then tallest height, then `(Column, Row)`. Amended: the same blocks, hard cap and tie-breaker, plus Option A1 cohort export and the Option A2 area-proportional budget realised as the integer block credit (§4). The draft's alternative "fractional-promotion accumulator" exists only in the form of that block credit; no fractional tree is ever created.
- **Q6 Browse height.** Browse `V(h)` and `p_veg` use pre-growth height (as stated for `p_veg`); confirm against `JuvenileEcologyRules` during implementation.

## 10. Exclusions

No Unity `Assets/`, Compact30, Legacy40, Enlarged80, Reference Future v1, historical saves, P0 pin (`9a99f781…`), or `/multi` HTML/JS/CSS. No Stage 5 UI controls/overlays, Oak/Beech, planting/shelters, economy, storms, graminoids, browse concealment, stem-form damage or save-schema change.

## 11. Evidence classification

| Part | Class |
|---|---|
| Seed-kernel, LightResponse, mast, browse, bramble/bracken constants | `GAMEPLAY CALIBRATION` / `TRANSFER` from Unity — not measured Irish values |
| Grid scaling policy §4 (incl. Option A cohort export and block credit) | `SIMULATION ABSTRACTION` (owner-approved 2026-11-23, amended by Option A the same day) |
| `CrownModel 1` initial radius on promotion | `SIMULATION ABSTRACTION` per D-094 |
| Neutral site (1.0) | deliberate neutral default, no soil data |

Qualification claims are limited to: numerical behaviour of the stated rules. No ecological forecast and no claim that regeneration is beneficial.
