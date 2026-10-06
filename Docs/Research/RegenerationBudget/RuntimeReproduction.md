# Regeneration accounting — runtime reproduction (current main, model 1)

Base 402a2b41108dc9aa91e225db9047956b0a91a97e (+ docs 70f5cc7); branch task/regeneration-population-budget. Unity 6000.6.0f1, batchmode, isolated config. Harness: `Tools/Verification/RegenerationBudget/RegenerationBudgetEvidence.cs` (disposable; copied into `Assets/ForestPrototype`, run, removed with its `.meta`). **No production code changed.**

All quantities are the production relative abundance ("density", occupancy = density / RegenDensityMax; RegenDensityMax = 1.5 for Sitka, beech and oak). Nothing below is stems/ha and no density × area conversion is used.

## Method and validity

- Fixtures call the production stage methods (`GrowExistingRegeneration`, `EstablishNewCohorts`, `PromoteCohorts`, `RestoreCellState`, `AdvancePlantedJuveniles`, `ApplyClearance`) with controlled cell light. RNG model 1, seed 20260914, browse pressure 0.2 (scenario default).
- Stand runs use the actual Scenario One starting-stand generator and three year-0 treatments (fell 0 / 20 / 60 % smallest stems), traced 100 years. The two regeneration stages run as instrumented copies of production code; everything else is production.
- **Faithfulness:** each treatment was replayed through unmodified `AdvanceOneYear`; full world-state hashes were identical at years 10, 25, 50 and 100 for all three treatments (`Evidence/state_hashes.csv`).
- **Ledger identity:** `after = before − lightLoss − browseLoss + infillAccepted − infillContraction − thresholdLoss + establishmentAccepted − establishmentContraction − exported` held in every one of 19,200 cell/species/year rows (max |residual| < 1e-5).
- **Long-run determinism:** the whole evidence run was executed twice in separate Unity processes. Ledger (SHA-256 `71d61b94fbd9b44f…`), funnels, state hashes, natural-vs-planted, clearance trace and fixture facts were byte-identical.

**Reproduce:** copy the harness to `Assets/ForestPrototype/`, then run Unity in batchmode with `-executeMethod RegenerationBudgetEvidence.Begin` and the environment variables `CCF_DIAG_DIR=<out>` and `CCF_REGEN_YEARS=100`. Remove the copy and its `.meta` afterwards. The full `ledger.csv` (3.2 MB, SHA-256 `71d61b94fbd9b44f207eef11030415539090c706987d3c25f72ba11f5e7c0580`) is not committed; it regenerates bit-identically. One complete run (fixtures, three 100-year treatments traced and replayed, clearance trace) took about 2 minutes on the GTX 980M / i5-6300HQ machine.

## Reproduced defects

| # | Defect | Fixture result (production methods) | In 100-year stand runs |
|---|---|---|---|
| 1 | **Seed-independent infill** | No adults, no seed: an existing Sitka cohort at light 0.3 or 0.9 grows 0.20 → 0.70 in 10 years; the survival-only counterfactual is 0.20 (unsourced gain +0.50). At light 0.05 no infill occurs. With no cohort, nothing appears (FX1 pass). | Infill is 10.1 / 11.1 / 13.8 % of all abundance additions (unthinned / 20 % / 60 %). Every occupied stand cell receives some arrival, so zero-arrival infill did not occur in-stand; the defect is that infill is **independent of arrival**, not that arrival is zero. 7–12 infill events per run occurred in years with no accepted establishment. |
| 2 | **Late recruits inherit age and height** | A cohort established in year 0 at 1.2 m receives new recruits in year 9: accepted 0.41, recorded age 9, height 1.2 m (new recruits should be age 0 at 0.15 m). | 63 % (unthinned) to 81 % (60 %) of establishment events merge into an older cohort. Inherited age median 33–36 years, p90 70–77, max 99; inherited height median 0.8–1.3 m, max 3.5 m. |
| 3 | **Promotion compression** | Abundance 0.011, 0.75 and 1.5 (0.7 %, 50 %, 100 % occupancy) each export to exactly one tree with zero residual. | 21 / 38 / 60 promotions; exported abundance per tree median 1.5 (full occupancy) in 76 / 84 / 92 % of promotions, min 0.11–0.15; residual after promotion always 0. |
| 4 | **Capacity rejection** | A full cohort accepts 0 of its request. An over-capacity cell (occupancy 1.4) **contracts** existing Sitka stock by 0.60 when 0.01 is requested. | Rejected / requested = 48.1 / 53.3 / 46.6 %; 315 / 519 / 805 fully rejected events. No contraction occurred in-stand (single species). |
| 5 | **Age set under full rejection** | Beech at full capacity; Sitka request fully rejected, yet the empty Sitka cohort is given establishment year and initial height. | Not observed in single-species stand runs (needs mixed species). |
| 6 | **Origin hazard (new)** | A planted-origin cohort promotes (density 0, origin stays Planted). Later natural arrival is accepted (0.40) but labelled Planted with height 0 and establishment year −1, because the age-initialisation guard skips Planted origin. | Not exercised (no planted cohorts in stand runs). |
| 7 | **Numerical reset loss** | — | 3.37 / 2.20 / 0.60 relative abundance zeroed by the 0.01 threshold over 100 years. At light 0.03 the threshold removes a natural oak/Sitka/beech cohort several years before the equivalent planted population dies. |

## Other findings

- **Browse vs light losses:** over 100 years cohort browse mortality is 0.10–0.23 against light mortality 47–120 relative units. Under the current shared rules, browsing acts on cohorts mainly through height delay, not mortality. This is consistent with the separate-mechanism design; it is not a calibration claim.
- **Natural vs planted** (`Evidence/natural_vs_planted.csv`; same cell, light, pressure 0.2; one natural cohort vs 1,000 exact planted juveniles through the production planted path, model 1):
  - survival-only cohort trajectories match planted survival closely, e.g. Sitka at light 0.15 after 20 years: 0.991 vs 0.995;
  - heights match (1.535 vs 1.536 m), and realised planted browse frequency matches the cohort's expected fraction (Sitka 0.023 vs 0.030, beech 0.102 vs 0.10, oak 0.18 vs 0.20);
  - **the whole divergence comes from infill:** the natural cohort reaches 4.3× its starting abundance at light ≥ 0.15 while planted survival stays ≤ 1.0;
  - at light 0.03 the reset threshold ends the natural cohort earlier.
- **Clearance and understorey causal trace** (`Evidence/clearance_trace.csv`; 60 % treatment, warm-up 15 years, cell 12, then 10 traced years):
  - *control:* the full cohort (1.5, 3.07 m) promotes to one tree in year 19, and the cell then re-establishes;
  - *clear cell:* production `ApplyClearance` removes 1.5 (occupancy 1 → 0), forgoing the year-19 tree. The cell re-fills to full capacity by year 22 (establishment plus infill), then rejects all further arrival;
  - *cover only:* zeroing understorey cover in all 64 cells without touching cohorts gives a regeneration state **bit-identical** to the control.
  
  So clearance affects regeneration only by removing abundance and releasing capacity. Vegetation cover has no causal path to juvenile survival or growth in current code (the browse `vegetationExposure` hook is fixed at 1).

## Not done

- **Tutorial management path:** the funnel plan's "real tutorial management path" run was not done. Treatments are the year-0 removal recipes only, with no planting or clearance during the run.
- **Mixed species:** stand runs contain natural Sitka regeneration only, because the starting stand has no beech or oak seed sources. Defects 5 and 6 and multi-species contraction are therefore fixture-only.
- **Model 0 and frozen Reference:** not re-run in this evidence task. No production change exists that could alter them.
