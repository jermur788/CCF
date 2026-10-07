# Long-run calibration plan (future Unity matrix)

**Status:** plan for the implementing worker. Not run here (no Unity in this task).

## 1. Matrix

| Axis | Levels |
|---|---|
| Thinning regime | unthinned · light (top-2 competitors per crop tree, or ≈ 15–20 % BA from below) · moderate (≈ 30 %) · heavy (≈ 35–40 %, concentrated) — reuse the regimes in `SitkaGrowthModel1.md` and the pedagogy T-plans so results are comparable |
| Intervention years | 0 (Scenario Year), 15, 30 for thinned regimes |
| Storm design (A) — **forced** | one storm of class M / S / X forced at Year 1, 5, 15, 30, 50 (one per run). Measures conditional vulnerability |
| Storm design (B) — **stochastic** | annual probability 0.04 / 0.08 / 0.12 with default severity weights, 100 years |
| Seeds | ≥ 10 per stochastic cell (simulation seeds); forced runs 3 seeds |
| Models | RNG 1, regeneration model as on main at the time (model 2 if Sol's work has landed), growth model 1, storm model 1 |
| Controls | storm model 0 (must reproduce the current anchors exactly) |

Cost estimate: forced 4 × 5 × 3 × 3 = 180 runs of ≤ 60 years. Stochastic 4 × 3 × 10 = 120 runs of 100 years. Batch-safe, headless. Seconds to a minute per run on the reference hardware (Growth Model 1 runs: see `growth_model1_performance.csv`).

## 2. Metrics per run

Storm events (year, class, direction) · trees damaged · basal area lost (m²/ha) · volume lost (m³) · **H/D, height and dominance of victims vs survivors** · opening size (largest connected cluster of cells with light +0.05 after the storm; cells touched) · deadwood created · salvaged volume (scripted policies: none / all within 2 years) · salvage net € (break-even year) · cash (min, final) · regeneration cells and recruits after storms (by species) · understorey response (once Sol's state exists) · **completion viability** (completed by Year 25? `retained-canopy ≥ 60`, other objectives) · Crop Trees lost.

## 3. Calibration targets vs sanity checks

**Calibration targets [C]** (tune `S_class`, `siteWindHazard` until met). These are gameplay bands, not data:

| Target | Band |
|---|---|
| Moderate storm on any regime | ≤ 3 % of stems lost |
| Severe storm, unthinned stand at Years 15–30 | 3–10 % of stems lost |
| Severe storm within 2 years of heavy thinning (same years) | 10–25 % of stems, ≥ 1.5× the unthinned loss |
| Extreme storm, worst case | ≤ 40 % of stems; never total loss of the stand |
| Stochastic default (p = 0.08), 25 years | expected 1–3 damaging storms; ≥ 90 % of runs remain completion-viable under light/moderate management |

**Why these bands:** the IRL stand-level model concerns *whether more than about 3 % of stems* are windthrown by a given top height [A]. That makes "≈ 3 %" a meaningful scale marker for "noticeable damage". The other bands are design choices to make storms disruptive but recoverable [C].

**Sanity checks** (must hold; not tuned against):

1. Storm model 0 reproduces the main anchors exactly (growth model 1 lifecycle, completion, Reference Future v1).
2. Year-0 stand: severe storm → ≤ 1 % of stems.
3. Victims' mean H/D and height exceed survivors' (direction [B]).
4. Ordering of `GameplayCases.md` checks 1–5 holds on average across seeds.
5. Determinism: same save, year, model and seed → identical victims (two processes).
6. Storm damage increases light and regeneration in affected cells in the following years (no causal claim beyond the existing light → establishment path).
7. No storm damages juveniles or cohorts directly (v1 scope).
8. Performance: a worst-case storm year (≈ 100 victims) stays within the frame budget in `PerformancePlan.md`.

## 4. Reporting

A `Docs/Research/WindthrowV1/CalibrationResults.md` with distributions (median and 10–90 % across seeds) for each regime × storm, the selected `S_class`, and an explicit list of which targets are [C]. Commit the run-hash CSV for determinism evidence, following the Growth Model 1 practice.
