# Stand development — old model vs candidate vs Irish validation

Old model: current production (no adult mortality). Candidate (diagnostic only, not production):
- **Height:** Irish Class III anchor-matched Chapman-Richards envelope, with each planted tree keeping its relative height (`H_i ∝ envelope(age)`).
- **Mortality:** prototype D, an individual suppression hazard plus an SDI 2,200 backstop.

Irish column: Class III anchor-matched top-height envelope. Values are per hectare (40 × 40 m stand = 0.16 ha).
Production settings: RNG model 1, regeneration model 1, browse pressure 0.2. Data: `Evidence/before_after.csv`, `Evidence/stand_development.csv`.

**Read with care:** the candidate mortality parameters are [I] placeholders (AdultMortalityDesign.md). These rows show behaviour, not calibrated outcomes.

## Unthinned

| Age | Top H old / cand / Irish III | Mean H old / cand | Dq cm old / cand | Stems/ha old / cand | BA m²/ha old / cand | Mean H/D old / cand | Top-tree H/D old / cand | Deaths (cand, cum.) | Deadwood m³/ha (cand) | Volume m³/ha old / cand |
|---|---|---|---|---|---|---|---|---|---|---|
| 20 | 14.63 / 14.63 / 14.21 | 11.65 / 11.65 | 15.79 / 15.79 | 2100 / 2100 | 41.10 / 41.10 | 75.1 / 75.1 | 71.9 / 71.9 | 0 | 0.0 | 249.2 / 249.2 |
| 30 | 17.05 / 20.92 / 20.4 | 14.48 / 16.83 | 19.82 / 20.06 | 2100 / 1956 | 64.79 / 61.80 | 74.5 / 85.0 | 67.3 / 82.5 | 23 | 18.0 | 481.5 / 538.2 |
| 40 | 18.62 / 24.17 / 24.89 | 16.4 / 19.87 | 23.17 / 23.74 | 2206 / 1944 | 93.05 / 86.07 | 73.5 / 85.4 | 60.8 / 78.9 | 42 | 45.6 | 801.8 / 911.3 |
| 60 | 22.11 / 28.36 / 30.14 | 20.3 / 25.47 | 29.98 / 33.60 | 2269 / 1369 | 160.12 / 121.34 | 72.1 / 76.4 | 53.8 / 68.6 | 161 | 327.1 | 1696.2 / 1582.1 |
| 80 | 25.01 / 30.6 / 32.51 | 23.48 / 27.26 | 36.37 / 43.76 | 2294 / 894 | 238.34 / 134.39 | 69.5 / 63.1 | 49.7 / 59.7 | 279 | 922.6 | 2892.0 / 1893.7 |
| 100 | 27.21 / 31.96 / 33.56 | 26.06 / 28.67 | 42.39 / 54.53 | 2300 / 625 | 324.55 / 145.95 | 66.3 / 53.0 | 46.5 / 52.7 | 349 | 1523.9 | 4327.4 / 2136.3 |
| 120 | 28.99 / 31.93 / 34.01 | 28.1 / 29.43 | 48.00 / 64.63 | 2300 / 475 | 416.15 / 155.83 | 62.8 / 45.5 | 44.1 / 46.0 | 395 | 2075.6 | 5937.2 / 2313.8 |

## Light thinning (20 % from below at 25, 40)

| Age | Top H old / cand / Irish III | Mean H old / cand | Dq cm old / cand | Stems/ha old / cand | BA m²/ha old / cand | Mean H/D old / cand | Top-tree H/D old / cand | Deaths (cand, cum.) | Deadwood m³/ha (cand) | Volume m³/ha old / cand |
|---|---|---|---|---|---|---|---|---|---|---|
| 20 | 14.63 / 14.63 / 14.21 | 11.65 / 11.65 | 15.79 / 15.79 | 2100 / 2100 | 41.10 / 41.10 | 75.1 / 75.1 | 71.9 / 71.9 | 0 | 0.0 | 249.2 / 249.2 |
| 30 | 17.05 / 20.92 / 20.4 | 14.98 / 17.59 | 20.97 / 21.16 | 1681 / 1581 | 58.07 / 55.58 | 72.0 / 83.6 | 67.0 / 82.1 | 19 | 14.8 | 440.4 / 497.4 |
| 40 | 18.62 / 24.46 / 24.89 | 16.3 / 20.07 | 24.27 / 24.69 | 1844 / 1700 | 85.32 / 81.37 | 70.0 / 83.2 | 60.5 / 79.3 | 26 | 23.6 | 745.5 / 882.6 |
| 60 | 22.04 / 28.11 / 30.14 | 19.39 / 26.17 | 31.93 / 35.30 | 1738 / 1263 | 139.13 / 123.58 | 68.0 / 74.6 | 53.0 / 67.2 | 96 | 127.1 | 1493.4 / 1643.0 |
| 80 | 24.96 / 30.17 / 32.51 | 22.51 / 27.57 | 38.70 / 45.10 | 1794 / 850 | 211.03 / 135.80 | 68.6 / 62.2 | 48.8 / 58.2 | 222 | 717.7 | 2579.6 / 1938.7 |
| 100 | 27.2 / 31.33 / 33.56 | 25.24 / 28.78 | 45.19 / 55.58 | 1806 / 606 | 289.74 / 147.08 | 66.0 / 52.6 | 45.7 / 51.0 | 317 | 1302.0 | 3878.3 / 2171.2 |
| 120 | 28.96 / 31.27 / 34.01 | 27.47 / 29.63 | 51.26 / 65.63 | 1806 / 463 | 372.71 / 156.44 | 62.3 / 45.2 | 43.3 / 44.8 | 363 | 1844.5 | 5326.5 / 2329.9 |

## Moderate thinning (30 % from below at 25, 40, 55, 70)

| Age | Top H old / cand / Irish III | Mean H old / cand | Dq cm old / cand | Stems/ha old / cand | BA m²/ha old / cand | Mean H/D old / cand | Top-tree H/D old / cand | Deaths (cand, cum.) | Deadwood m³/ha (cand) | Volume m³/ha old / cand |
|---|---|---|---|---|---|---|---|---|---|---|
| 20 | 14.63 / 14.63 / 14.21 | 11.65 / 11.65 | 15.79 / 15.79 | 2100 / 2100 | 41.10 / 41.10 | 75.1 / 75.1 | 71.9 / 71.9 | 0 | 0.0 | 249.2 / 249.2 |
| 30 | 17.05 / 20.92 / 20.4 | 15.17 / 17.89 | 21.49 / 21.64 | 1469 / 1394 | 53.26 / 51.28 | 71.1 / 83.0 | 66.8 / 81.8 | 17 | 12.5 | 408.1 / 465.1 |
| 40 | 18.72 / 24.46 / 24.89 | 16.17 / 20.23 | 24.75 / 25.26 | 1650 / 1531 | 79.36 / 76.71 | 68.6 / 82.1 | 60.4 / 78.8 | 21 | 14.6 | 698.3 / 842.7 |
| 60 | 22.13 / 28.5 / 30.14 | 20.71 / 25.37 | 35.86 / 36.08 | 1081 / 1006 | 109.20 / 102.91 | 60.5 / 73.1 | 52.4 / 67.2 | 31 | 15.1 | 1189.6 / 1409.1 |
| 80 | 24.96 / 30.07 / 32.51 | 20.82 / 23.88 | 41.94 / 42.38 | 1081 / 913 | 149.36 / 128.72 | 59.7 / 65.9 | 47.7 / 57.0 | 68 | 17.4 | 1847.8 / 1904.5 |
| 100 | 27.19 / 30.55 / 33.56 | 21.81 / 29.28 | 46.02 / 57.11 | 1269 / 581 | 211.04 / 148.90 | 68.4 / 52.9 | 44.4 / 49.1 | 204 | 427.3 | 2838.0 / 2267.8 |
| 120 | 28.91 / 31.1 / 34.01 | 24.4 / 29.48 | 51.93 / 66.02 | 1300 / 463 | 275.32 / 158.33 | 67.8 / 46.7 | 41.8 / 43.9 | 273 | 925.8 | 3932.9 / 2442.3 |

## Heavy/selective (35 % most crowded at 25, 40, 55, 70)

| Age | Top H old / cand / Irish III | Mean H old / cand | Dq cm old / cand | Stems/ha old / cand | BA m²/ha old / cand | Mean H/D old / cand | Top-tree H/D old / cand | Deaths (cand, cum.) | Deadwood m³/ha (cand) | Volume m³/ha old / cand |
|---|---|---|---|---|---|---|---|---|---|---|
| 20 | 14.63 / 14.63 / 14.21 | 11.65 / 11.65 | 15.79 / 15.79 | 2100 / 2100 | 41.10 / 41.10 | 75.1 / 75.1 | 71.9 / 71.9 | 0 | 0.0 | 249.2 / 249.2 |
| 30 | 17.09 / 20.91 / 20.4 | 14.97 / 17.64 | 21.40 / 21.60 | 1363 / 1300 | 48.99 / 47.65 | 70.7 / 82.1 | 66.7 / 81.2 | 16 | 11.3 | 372.9 / 429.0 |
| 40 | 19.06 / 25.2 / 24.89 | 15.64 / 19.4 | 24.53 / 25.00 | 1581 / 1488 | 74.72 / 73.01 | 67.9 / 80.4 | 61.3 / 80.5 | 20 | 11.5 | 653.1 / 794.6 |
| 60 | 22.5 / 29.93 / 30.14 | 18.74 / 23.04 | 34.55 / 35.24 | 1050 / 988 | 98.46 / 96.29 | 60.4 / 70.1 | 53.0 / 70.3 | 33 | 12.0 | 1061.1 / 1283.8 |
| 80 | 25.21 / 32.29 / 32.51 | 20.85 / 24.71 | 43.03 / 44.71 | 975 / 825 | 141.79 / 129.51 | 57.4 / 62.1 | 47.5 / 60.6 | 43 | 12.9 | 1742.3 / 1863.7 |
| 100 | 27.44 / 32.84 / 33.56 | 21.83 / 28.38 | 47.53 / 57.75 | 1138 / 569 | 201.81 / 149.00 | 63.7 / 50.7 | 44.1 / 52.2 | 144 | 438.7 | 2698.4 / 2216.9 |
| 120 | 29.11 / 32.86 / 34.01 | 24.45 / 29.32 | 53.83 / 67.77 | 1163 / 444 | 264.58 / 160.05 | 62.5 / 43.8 | 41.5 / 46.0 | 202 | 912.0 | 3759.9 / 2390.3 |

## Reading

- **Old model, no mortality:** stocking never falls; regeneration even adds stems. Basal area rises without limit: 93 m²/ha at 40, 160 at 60 and 416 at 120 unthinned (265–373 thinned). Volume reaches 3,760–5,937 m³/ha.
- **When it becomes implausible:** closed Sitka stands are generally understood to carry well under about 80–90 m²/ha. That band is general forestry knowledge [I], not a value from the supplied evidence. On it, the no-mortality stand becomes implausible at about **age 35–40** unthinned and age 45–60 thinned.
- **Candidate height:** top height follows Class III within about 0.7 m to age 40 (+0.5 m at 30, −0.7 m at 40), then drifts 1.5–3 m under the envelope (−1.8 m at 60, −2.1 m at 120). The drift is the same without mortality (−2.7 m at 120). The largest-DBH set shifts over time toward trees whose authored height sat lower relative to the envelope, and the proportional rule preserves that. A production version would need an explicit dominance rule if top height must track the envelope exactly.
- **Candidate mortality:** density stays bounded (basal area 156–158 m²/ha at 120), but the SDI 2,200 backstop is far too permissive. The level must come from Irish/UK maximum-density evidence before any production use.
- **Candidate H/D:** much higher early (mean 83–85 at age 30–40, against 71–75 old). This matters for the storm milestone; see AdultMortalityDesign.md and Scenario1SiteProductivityDecision.md.
