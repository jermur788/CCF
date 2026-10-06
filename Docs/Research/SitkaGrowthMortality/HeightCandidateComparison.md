# Height candidate comparison (Phases 7–8, 14–15)

All candidates were evaluated **offline** on the production DBH trajectories (`Tools/Verification/SitkaGrowthMortality/height_candidates.py`). This is exact for adult trees: height does not enter the competition index, crowns use DBH only, and canopy light saturates at 8 m. Only planted trees take candidate heights; recruits keep production heights and never enter the top-height set. Data: `Evidence/height_candidates.csv`.

## Candidates

| ID | Form | Individual rule |
|---|---|---|
| Old | Production: `dH/dt = 0.45 (1 − H/35)` | Same law for every tree |
| **A-III** (packet A/B) | Irish Class III Chapman-Richards, published b2/b3, b0 rescaled so H30 = 20.4 m | Relative position kept: `H_i(a) = H_i(20) · E(a)/E(20)`, applied yearly as `H ← H · E(a+1)/E(a)` |
| A-II, A-IV | As A-III for Classes II and IV | As above |
| C-III (packet C) | Current architecture retuned: `dH/dt = 1.19 (1 − H/34.36)`, passing the authored start (14.63 m at 20) and the Class III anchor at 30 | Same law for every tree, from its own height |

## Results (unthinned stand top height, m; envelope values in brackets)

| | H20 | H30 | H40 | H60 | H80 | H100 | H120 |
|---|---|---|---|---|---|---|---|
| Old | 14.63 | 17.05 | 18.62 | 22.11 | 25.01 | 27.21 | 28.99 |
| **A-III** | 14.63 (14.21) | **20.92** (20.40) | 24.14 (24.89) | 28.36 (30.14) | 30.43 (32.51) | 30.88 (33.56) | 31.30 (34.01) |
| A-II | 14.63 (16.2) | 21.00 (**23.3**) | 24.88 | 31.22 | 35.49 | 37.59 | 39.25 |
| A-IV | 14.63 (11.3) | 22.47 (**17.4**) | 26.58 | 31.34 | 33.29 | 33.51 | 33.79 |
| C-III | 14.63 | 20.35 | 24.06 | 29.10 | 31.72 | 33.02 | 33.69 |

| Criterion | Old | A-III | A-II / A-IV | C-III |
|---|---|---|---|---|
| Starting-state fit | — (authored) | Start 0.4 m above envelope at 20: **consistent** | Start 1.6 m below (II) / 3.3 m above (IV): **inconsistent**; misses its own anchor by 2.3 / 5.1 m | Exact by construction |
| Irish anchor fit (H30) | 17.05 vs III 20.4: **fails** | 20.92 (+0.5) | Fails | 20.35 (−0.05) |
| Annual top-height increment, 20–30 / 30–40 / 40–60 / 60–80 / 80–100 | 0.24 / 0.16 / 0.17 / 0.15 / 0.11 | 0.63 / 0.32 / 0.21 / 0.10 / 0.02 | — | 0.57 / 0.37 / 0.25 / 0.13 / 0.07 |
| Top vs individual | No separation; small trees catch up with dominants (mean/top 0.97 at 120) | Site envelope plus individual relative position; suppressed trees stay shorter (mean/top 0.88 at 120) | — | No separation; strongest catch-up (mean/top 0.97) |
| DBH interaction | None | None | None | None |
| H/D, top trees, age 30 / 60 / 120 | 67 / 54 / 44 | 83 / 69 / 48 | 83–89 | 80 / 71 / 51 |
| H/D p90, age 120 | 72 | 68 | 73–84 | **85** (slender suppressed trees) |
| Timber effect, year-16 thinning (30 % from below) | €857 | €969 (+13 %) | €989–1,074 | €1,220 (+42 %) |
| Performance | O(n) | O(n), one closed-form evaluation per tree per year | O(n) | O(n) |
| Save/version | — | Stateless per tree: needs only the current height and the saved age. A model-version flag is needed to keep legacy replay (SaveVersioningAssessment) | Same | Same |

## Interpretation

- **Recommended: A-III.** Chapman-Richards Class III, b0 anchor-matched, with a relative-position individual rule. It is the only Irish-anchored curve consistent with the authored starting stand.
  - It separates the site envelope from realised individual height, as the synthesis recommends.
  - It makes no competition-driven height response, so ordinary thinning has a weak effect on top height: −0.7 to +1.7 m against unthinned (at most about 6 %, the largest under heavy/selective thinning at age 50). This comes from which trees form the largest-DBH set, not from a height response.
  - Caveats: this is the class III curve of a rounded-coefficient paper (correction at 30 only 0.13 m on b0), and the top-height definition needs checking against S4. Top height drifts about 2–3 m below the envelope after age 80 (StandDevelopmentBeforeAfter.md).
- **C-III** fits the anchor equally well and changes only two asset values. Its age-independent form keeps lifting suppressed trees toward the dominants. That inflates thinning revenue (+42–50 %) and produces very slender suppressed trees (H/D p90 85). Rejected, unless the simplest possible change is preferred and that behaviour is accepted.
- **Classes II and IV** cannot be represented without re-authoring the starting stand (heights and probably DBH). See Scenario1SiteProductivityDecision.md.

## Crown coherence (Phase 14)

- **Crowns:** they depend only on DBH (Tabbush open-grown envelope × the competition reduction), so the potential crown is unchanged by any height candidate. The pooled positive Hegyi coefficient of Davies & Pommerening is not used anywhere, and should not be.
- **Crown ratio:** what does change is the implied crown ratio. Live crown length is not represented, so taller stems with the same crown radius read as narrower-crowned trees. Davies & Pommerening's crown-length model (H/D and local basal area) is the evidence-backed future hook. Not implemented.

## H/D before windthrow (Phase 15)

A-III raises early slenderness. Top-tree H/D is about 82–83 at age 30–40 (old 61–67), and the stand mean is 83–85 (old 71–75), because the authored DBH stays put while height follows Class III.

The project has no accepted H/D stability threshold, and none is adopted here. As general knowledge, H/D above about 80–100 is commonly associated with wind instability in Sitka [I]. A future storm model calibrated against the old, stouter trees would see a materially more exposed stand under A-III. Any height decision should be made before storm calibration, not after.
