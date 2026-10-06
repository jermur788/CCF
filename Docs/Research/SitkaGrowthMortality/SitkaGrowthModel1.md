# Sitka growth model 1 — implementation record

Branch `task/sitka-site-height-adult-mortality`. Base `origin/main` @ 3e4ee40. Implements the Manager continuation "SITKA GROWTH / MORTALITY" decisions:
- Scenario One is Irish site Class III;
- Class III height for new games;
- save v17 `growthModel`;
- Hegyi kept;
- a separate SDI density signal;
- self-thinning mortality with deadwood.

Not merged.

## Model policy

| Item | Behaviour |
|---|---|
| Save | v17 adds `ForestSaveData.growthModel` (int) |
| Missing field / v ≤ 16 / explicit 0 | Growth model 0, legacy: `dH/dt = 0.45(1 − H/35)`, no adult mortality |
| New Scenario One games | Growth model 1 (`ScenarioOneManager.NewGameGrowthModel`) |
| Reference Future v1 | Growth model 0; the player's model is restored after preview |
| Migration | None |
| Storm fields | None added |

## Growth model 1

**Height (Sitka only)** — `SitkaGrowthModel.SiteTopHeightM`, `NextHeight`:
- **Envelope:** Lekwadi et al. (2012) Class III form `H = b0(1 − e^(−0.042 t))^1.563`, with b0 = **34.362** derived from the published age-30 anchor 20.4 m. The rounded paper b0 (34.228) is not used.
- **Envelope values:** H20 14.21, H30 20.40, H40 24.89, H60 30.14, H80 32.51, H100 33.56 m.
- **Individual trees** keep their relative height: `H ← H · E(age+1)/E(age)`. Suppressed trees stay shorter, there is no competition term, and thinning moves site top height only through which trees form the largest-DBH set.
- **Other species** keep the legacy height law.

**DBH:** unchanged. The Hegyi response `1/(1 + CI/Ci50)` remains the only DBH competition term, and density never enters DBH growth. Verified: identical per-tree DBH under growth 0 and 1 while no tree dies (fixture `DBH_UNCHANGED_BY_HEIGHT_MODEL`).

**Density signal:** `RD = N/ha × (Dq/25)^2.063 / 1868`. This is the British Sitka maximum size–density line from Comeau et al. (2010) [B]; see AdultMortalityCalibration.md for the equation, units, source and transfer limitation.

**Mortality** (`ForestEcologyController.ApplyAdultDensityMortality`, annual step 4b, after crowns, before canopy):
1. Compute the stand RD and density pressure `P = max(0, (RD − 0.6)/0.4)` [C].
2. Each adult tree dies with probability `min(0.5, 0.08·P²·S⁴/mean(S⁴))` [C], where S is current Hegyi suppression. The roll is `SimulationRandom.Roll(rngModel, "ADULT-MORT-"+id, year, seed)`.
3. Boundary: if survivors would exceed RD 1, the most suppressed die until RD ≤ 1 (ties by tree ID; running sums keep this linear).
4. Victims are collected first, then killed inside one change batch through `ForestTree.ApplyMortality("self-thinning", year)`. The canopy and seed rain rebuild once.

**Deadwood:** `ScenarioOneManager.OnTreeBiologicalDeath` (subscribed to `ForestTree.MortalityApplied`, growth model 1 only) creates one `ScenarioDeadwoodRecord` per death. It uses the existing fallen-log visual, decay, annual snapshot and save path, with fallen year = death year and volume = stem volume at death. Restored deaths do not raise the event, and `ApplyMortality` refuses a second death, so there is one record per tree. No new deadwood subsystem; no standing snags.

**Persisted state:** only `growthModel`. Mortality is stateless: it uses the recomputed competition index, saved DBH/height/age, and the existing mortality and deadwood fields. The cumulative `equivalentSuppressedYears` (persisted since v6) is a diagnostic that never decreases after release, so it is not authoritative for current vulnerability and is not used.

## Long-run gates (production growth model 1; RNG 1, regeneration 1, browse 0.2)

Data: `Evidence/growth_model1_stand_development.csv`. Two separate processes gave identical results (state hashes `Evidence/growth_model1_run_hashes.csv`). "Old" = legacy growth, no mortality (`Evidence/stand_development.csv`).

### Unthinned

| Age | Top H (Class III) | Mean H | Mean DBH / QMD cm | Stems/ha | SDI | RD | BA m²/ha (old) | Volume m³/ha (old) | H/D dominant / mean / suppressed (old mean) | Deaths (cum.) | Deadwood m³/ha (records) |
|---|---|---|---|---|---|---|---|---|---|---|---|
| 20 | 14.63 (14.21) | 11.65 | 15.59 / 15.79 | 2100 | 1004 | 0.435 | 41.10 (41.10) | 249.2 (249.2) | 72.2 / 75.1 / 78.9 (75.1) | 0 | 0.0 (0) |
| 30 | 20.92 (20.4) | 16.73 | 19.62 / 19.83 | 2088 | 1440 | 0.693 | 64.50 (64.79) | 559.8 (481.5) | 83.4 / 85.6 / 88.1 (74.5) | 2 | 2.2 (2) |
| 40 | 24.14 (24.89) | 19.98 | 23.34 / 23.79 | 1706 | 1576 | 0.825 | 75.87 (93.05) | 803.1 (801.8) | 83.7 / 85.5 / 84.8 (73.5) | 80 | 158.7 (80) |
| 60 | 28.25 (30.14) | 23.93 | 33.16 / 34.04 | 900 | 1477 | 0.911 | 81.92 (160.12) | 1052.9 (1696.2) | 70.3 / 71.8 / 68.6 (72.1) | 249 | 738.9 (249) |
| 80 | 30.11 (32.51) | 25.51 | 43.94 / 45.38 | 538 | 1400 | 0.984 | 86.94 (238.34) | 1204.0 (2892.0) | 57.4 / 58.4 / 57.8 (69.5) | 385 | 1317.6 (385) |
| 100 | 30.92 (33.56) | 26.07 | 54.27 / 56.56 | 344 | 1274 | 0.992 | 86.36 (324.55) | 1241.3 (4327.4) | 49.9 / 49.4 / 52.7 (66.3) | 496 | 1850.6 (496) |
| 120 | 31.05 (34.01) | 25.15 | 60.32 / 64.53 | 256 | 1174 | 0.970 | 83.81 (416.15) | 1218.8 (5937.2) | 43.4 / 45.0 / 57.7 (62.8) | 609 | 2286.8 (609) |

### Light (20 % from below at 25, 40)

| Age | Top H (Class III) | Mean H | Mean DBH / QMD cm | Stems/ha | SDI | RD | BA m²/ha (old) | Volume m³/ha (old) | H/D dominant / mean / suppressed (old mean) | Deaths (cum.) | Deadwood m³/ha (records) |
|---|---|---|---|---|---|---|---|---|---|---|---|
| 20 | 14.63 (14.21) | 11.65 | 15.59 / 15.79 | 2100 | 1004 | 0.435 | 41.10 (41.10) | 249.2 (249.2) | 72.2 / 75.1 / 78.9 (75.1) | 0 | 0.0 (0) |
| 30 | 20.92 (20.4) | 17.53 | 20.87 / 20.97 | 1681 | 1268 | 0.626 | 58.07 (58.07) | 518.3 (440.4) | 82.7 / 84.1 / 85.7 (72.0) | 0 | 0.0 (0) |
| 40 | 24.14 (24.89) | 20.08 | 23.90 / 24.57 | 1581 | 1538 | 0.817 | 74.97 (85.32) | 811.4 (745.5) | 82.7 / 83.5 / 81.1 (70.0) | 43 | 101.0 (43) |
| 60 | 28.04 (30.14) | 24.98 | 34.95 / 35.74 | 844 | 1498 | 0.944 | 84.66 (139.13) | 1117.9 (1493.4) | 67.8 / 71.5 / 72.4 (68.0) | 174 | 545.6 (174) |
| 80 | 29.93 (32.51) | 25.38 | 43.61 / 45.88 | 525 | 1391 | 0.984 | 86.81 (211.03) | 1235.7 (2579.6) | 56.2 / 59.1 / 63.0 (68.6) | 316 | 1167.0 (316) |
| 100 | 31.20 (33.56) | 28.10 | 57.53 / 59.08 | 313 | 1243 | 0.986 | 85.67 (289.74) | 1277.2 (3878.3) | 48.6 / 49.8 / 52.9 (66.0) | 448 | 1688.3 (448) |
| 120 | 31.25 (34.01) | 27.84 | 64.59 / 67.44 | 238 | 1168 | 0.985 | 84.83 (372.71) | 1298.5 (5326.5) | 43.6 / 44.8 / 52.5 (62.3) | 562 | 2082.0 (562) |

### Moderate (30 % from below at 25, 40, 55, 70)

| Age | Top H (Class III) | Mean H | Mean DBH / QMD cm | Stems/ha | SDI | RD | BA m²/ha (old) | Volume m³/ha (old) | H/D dominant / mean / suppressed (old mean) | Deaths (cum.) | Deadwood m³/ha (records) |
|---|---|---|---|---|---|---|---|---|---|---|---|
| 20 | 14.63 (14.21) | 11.65 | 15.59 / 15.79 | 2100 | 1004 | 0.435 | 41.10 (41.10) | 249.2 (249.2) | 72.2 / 75.1 / 78.9 (75.1) | 0 | 0.0 (0) |
| 30 | 20.92 (20.4) | 17.85 | 21.40 / 21.49 | 1469 | 1152 | 0.575 | 53.26 (53.26) | 482.2 (408.1) | 82.8 / 83.5 / 85.2 (71.1) | 0 | 0.0 (0) |
| 40 | 24.37 (24.89) | 19.89 | 23.96 / 24.83 | 1500 | 1484 | 0.792 | 72.64 (79.36) | 795.1 (698.3) | 81.7 / 82.3 / 79.0 (68.6) | 23 | 69.2 (23) |
| 60 | 28.28 (30.14) | 23.79 | 33.66 / 35.65 | 763 | 1347 | 0.849 | 76.10 (109.20) | 1036.1 (1189.6) | 66.8 / 70.4 / 68.4 (60.5) | 109 | 316.8 (109) |
| 80 | 30.12 (32.51) | 23.76 | 40.23 / 44.37 | 556 | 1397 | 0.973 | 86.02 (149.36) | 1274.7 (1847.8) | 56.1 / 60.7 / 66.9 (59.7) | 225 | 542.4 (225) |
| 100 | 31.40 (33.56) | 26.91 | 54.41 / 57.78 | 325 | 1247 | 0.980 | 85.23 (211.04) | 1303.6 (2838.0) | 49.5 / 51.6 / 61.4 (68.4) | 393 | 1072.8 (393) |
| 120 | 31.67 (34.01) | 27.01 | 61.81 / 66.25 | 244 | 1165 | 0.974 | 84.02 (275.32) | 1310.8 (3933.0) | 44.6 / 46.9 / 63.3 (67.8) | 520 | 1478.5 (520) |

### Heavy/selective (35 % most crowded at 25, 40, 55, 70)

| Age | Top H (Class III) | Mean H | Mean DBH / QMD cm | Stems/ha | SDI | RD | BA m²/ha (old) | Volume m³/ha (old) | H/D dominant / mean / suppressed (old mean) | Deaths (cum.) | Deadwood m³/ha (records) |
|---|---|---|---|---|---|---|---|---|---|---|---|
| 20 | 14.63 (14.21) | 11.65 | 15.59 / 15.79 | 2100 | 1004 | 0.435 | 41.10 (41.10) | 249.2 (249.2) | 72.2 / 75.1 / 78.9 (75.1) | 0 | 0.0 (0) |
| 30 | 20.98 (20.4) | 17.53 | 21.27 / 21.40 | 1363 | 1061 | 0.529 | 48.99 (48.99) | 439.4 (372.9) | 82.2 / 82.5 / 82.7 (70.7) | 0 | 0.0 (0) |
| 40 | 25.14 (24.89) | 19.04 | 23.43 / 24.56 | 1525 | 1482 | 0.787 | 72.24 (74.72) | 783.1 (653.1) | 81.7 / 80.3 / 74.7 (67.9) | 10 | 25.0 (10) |
| 60 | 29.82 (30.14) | 24.09 | 35.05 / 36.48 | 731 | 1341 | 0.854 | 76.44 (98.46) | 1018.0 (1061.1) | 70.4 / 68.5 / 65.2 (60.4) | 92 | 210.7 (92) |
| 80 | 31.99 (32.51) | 20.09 | 35.71 / 41.27 | 569 | 1272 | 0.857 | 76.10 (141.79) | 1078.1 (1742.3) | 59.2 / 58.3 / 66.1 (57.4) | 185 | 390.0 (185) |
| 100 | 32.77 (33.56) | 26.79 | 56.10 / 58.70 | 319 | 1254 | 0.993 | 86.27 (201.81) | 1284.6 (2698.4) | 50.4 / 49.2 / 53.2 (63.7) | 352 | 704.7 (352) |
| 120 | 33.05 (34.01) | 22.28 | 52.44 / 60.53 | 300 | 1240 | 0.995 | 86.33 (264.58) | 1306.6 (3759.9) | 44.5 / 48.8 / 66.8 (62.5) | 473 | 1081.0 (473) |

### Reading

**Unthinned:**
- **Self-thinning:** begins once RD passes 0.6 (about age 26–30). The stand then self-thins along the British line (RD 0.98–0.99 from age 80) instead of growing without bound.
- **At age 120:** basal area 83.8 m²/ha and volume 1,219 m³/ha, against the old 416 m²/ha and 5,937 m³/ha.
- **Volume caveat:** the 0.5 form factor in `BiologicalStemVolumeM3` is untested against Irish volume data, so absolute volumes are indicative only.

**Thinned regimes:**
- **Mortality:** fewer suppression deaths (609 unthinned → 562 / 520 / 473) and much less deadwood (2,287 → 2,082 / 1,479 / 1,081 m³/ha cumulative).
- **DBH:** larger mean DBH at matched ages.
- **Top height:** little effect (age 120: 31.05 unthinned, 31.25–33.05 thinned). The largest effect is heavy/selective thinning, which keeps the tallest dominants.
- **Density:** RD returns to the line after the last thinning at age 70.

**Top height** tracks Class III within 0.75 m to age 40 (20.92 at 30, 24.14 at 40). It then sits 1.9–3.0 m below the envelope (6–9 %), because the largest-DBH set shifts toward trees whose authored height was lower relative to it. Individual trees are not required to equal the envelope; a dominance rule could close this if needed (not implemented).

**H/D:** dominant H/D rises to about 83 at age 30–40 (old mean 73–75), then falls to 43–45 at 120 as self-thinning frees growing space. Suppressed H/D stays higher late (53–67), since self-thinning removes most of those trees. This is input for the windthrow task; storms were not changed.

## Verification

All gates ran in isolated Unity 6000.6.0f1 batch processes on this branch.

**`SitkaGrowthModelVerification`: 20/20.**
- **Policy:** new game is growth 1 on save v17. A missing field, v16 and explicit 0 load as 0; explicit 1 loads as 1; an invalid value is rejected. Reference previews run as growth 0 and the player model is restored.
- **Height:** Class III envelope anchors; individual relative height; DBH unchanged by the height model; top height within 10 % of Class III; thinning effect on top height ≤ 5.6 %; deterministic.
- **Mortality:**
  - no deaths below the onset (start RD 0.435);
  - suppressed trees die far more than dominants (65 % vs 14 % over 10 years at RD ≈ 0.9);
  - cause `self-thinning` with the step year;
  - deadwood exactly once (73 records for 73 deaths);
  - no duplicate death;
  - boundary RD ≤ 1;
  - thinning lowers mortality (73 → 22);
  - save/load deterministic through a dying stand.
- **Anchors:** legacy and regeneration-model-1 lifecycle anchors reproduce under growth 0; growth-1 anchors are deterministic.

**Legacy compatibility:**

| Check | Result |
|---|---|
| Canonical lifecycle | `BFC55473C1506067` / `3485B6630C9EA448` |
| Regeneration model 1 lifecycle (growth 0) | `962846D2F517B293` / `FBB8F470D85FF815` |
| Completion, RNG 1 + regen 1 + growth 0 | `6F84AF319D301F87` (v16 layout) |
| Completion, all legacy | `568922E1A6D73CDD` (v15 layout) |
| Completion, RNG 1 + legacy regen | `00479F18970F9926` (v15 layout) |
| Reference continuation | `9CDF21A541C5968D` (v15 layout) |
| Reference preview / archive integrity | Year 100 `7AD177B3CC2F73C7`; integrity PASS |
| Legacy regeneration ledger | Byte-identical (`71d61b94…`) |
| `RegenerationModelVerification` | 35/35, 100-year funnels byte-identical to the integrated main |

**Scenario 1 (growth model 1, new-game default):** completed in Year 25 with all 8 objectives, identical in two runs (`D7C4DDD36B53FCCE`):
- regeneration 53 cells (needs 3);
- retained canopy 218 (needs 60);
- fallen deadwood 4.23 m³ (natural deaths now contribute);
- lowest cash 613,975 cents (€6,139.75; growth 0: €5,897.73).

**Regression:** interaction, canonical, juvenile foundation, Reference, RNG and RNG policy, browsing, save hardening, planting, pruning, deadwood, progress, CCF Oak / Oak player / Beech / planting, batch recompute, Stage 1 work economy (420 assertions), timber yield (3,007), economy integration (120) and viability (64), ecology calibration and the integration harness all PASS.
- **Pre-existing failures:** Clearance, MenuTutorial and Removal fail with the same messages as on pre-merge main (reproduced on 402a2b4 and in the 4670e73 integration). Not changed here.

**Harness pins** (legacy contracts, not behaviour changes):
- Every legacy- and regeneration-anchor harness pins growth model 0, and save-version checks move to 17.
- The completion gate accepts `CCF_GROWTH_MODEL` and logs the v16-layout hash; `ScenarioReferenceArchive.LegacyV15WorldHash` / `LegacyV16WorldHash` strip the new field for historical comparison.
- `EcologyCalibrationAdoptionVerification` pins growth 0: its C8 DBH/light calibration and "no automatic death" assertion are legacy-growth contracts.

## Economy and tutorial

**Timber:** production quotes with the Class III heights (identical in law to candidate A-III) show +1.9 % revenue at scenario year 1, +13 % at year 16, +15 % at year 25 and +7 % at year 40 (`Evidence/timber_impact.csv`, reproduced on this branch). The €2,500 contractor minimum still dominates thinnings before year 40. No prices or costs changed.

**Tutorial:**
- First thinning (year 1) and pruning (lift heights already reachable at year 0) are unchanged.
- The second intervention and completion (Year 25) pass in the completion gate.
- Self-thinning begins in unthinned parts of the stand from about scenario year 6–10. It shows as natural deaths, fallen logs and deadwood in the review; the tutorial's year-1 thinning delays it.
- No tutorial objective was edited.

## Performance (`Evidence/growth_model1_performance.csv`)

Synthetic stands sized to sit in the hazard zone (RD ≈ 0.9); mean annual step of 3 years, growth 0 → growth 1:

| Trees | Step, growth 0 | Step, growth 1 | Deaths per year |
|---|---|---|---|
| 336 | 125 ms | 153 ms | 15 |
| 1,300 | 544 ms | 607 ms | 57 |
| 3,000 | 1,542 ms | 1,886 ms | 130 |
| 5,000 | 2,845 ms | 3,485 ms | 209 |

- **Evaluation:** linear, with no new O(n²) work (the competition index is reused).
- **Cost:** mostly per-death world work (deadwood record and log visual, marking handler), about 3 ms per death, against 13–25 ms in the unbatched prototype.
- **Batching:** canopy and seed-rain rebuilds happen once per step.

## Known limitations / open

- **Density relationship:** British, not Irish [B]. The intercept is via the published maximum SDI at Dq 25 cm; check against the full paper.
- **Calibration choices [C]:** the onset, strength, S⁴ vulnerability and cap. Forest Yield outputs were not available for validation.
- **Top height** drifts 6–9 % below the envelope after age 60 (dominance rule not implemented).
- **Recruit pulses** promoted under a closed canopy at the line die quickly. Plausible, but it also reflects the promotion rule.
- **Absolute volumes:** the 0.5 form factor is untested.
- **Pre-existing failures:** Clearance, MenuTutorial and Removal.
