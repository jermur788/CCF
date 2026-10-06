# Marking assessment framework (Workstreams C3, C4)

**Status:** NEW PROPOSAL [INF]. Example figures are real [HARNESS] for the Scenario One Year-0 stand (RNG 1, regeneration 1, 16 harness Crop Trees). **No universal score.**

## 1. Principle

A marking review answers three questions side by side:

1. **What did you take?** (harvest, money)
2. **What did you leave?** (capital, Crop Trees, structure, seed, cover)
3. **What changes next?** (release, light, stability; then long-term in `LongTermTrainingFlow.md`)

It never adds these into one number. Practitioner monitoring systems keep dimensions separate on purpose [PRAC §17.4 ANW/AFI; §13.2 Wallonia capital vs structure].

## 2. Dimensions and metrics

Every metric below is AUTHORITATIVE NOW or DERIVABLE NOW (`ResidualStandMetricAudit.md`). Labels are plain-language; the number is secondary.

### 2.1 Economic

| Metric | Source | Display |
|---|---|---|
| Gross timber income | Work Plan harvest quote (production) | € |
| Contractor cost, including small-job minimum | same | € with the minimum shown separately |
| Net result | same | € (often negative for early thinnings — say why) |

### 2.2 Silvicultural

| Metric | Source | Display |
|---|---|---|
| Crop Trees released | per-Crop-Tree competition before/after (Hegyi) | "15 of 16 Crop Trees have ≥10 % less competition" |
| Mean Crop-Tree competition change | same | "−20 %" |
| Predicted Crop-Tree growth change (next year) | production growth response | "+12 %" |
| Removals not touching any Crop Tree | Hegyi term = 0 for all Crop Trees | "6 removed trees are more than 8 m from any Crop Tree" |
| Low-effect removals | term < 5 % of each nearby Crop Tree's competition (diagnostic cut-off, labelled) | "77 removals each supply under 5 % of any Crop Tree's competition" |
| Productive capital retained | basal area, standing volume | "Basal area 41.1 → 36.2 m²/ha; standing volume 39.9 → 34.7 m³" |

### 2.3 Ecological / structural

| Metric | Source | Display |
|---|---|---|
| Canopy continuity | forecast cell light (production canopy formula) | "Mean ground light 0.05 → 0.06; 5 cells above 0.20" |
| Opening pattern | cells touched; largest connected opened patch; Clark–Evans index of removals | "Spread over 23 cells; largest opening 3 cells" |
| Seed sources retained | species maturity | "0 seed-bearing trees removed" (all are immature at Year 0; meaningful from Year 1 onward) |
| Species retained | species of removed/retained | trivial at Year 0 (all Sitka) |
| Deadwood created | felling outcome choice | "2 stems left as deadwood (0.3 m³)" |

### 2.4 Stability

| Metric | Source | Display | Caveat |
|---|---|---|---|
| Crop-Tree H/D (slenderness) | height / DBH | "Your Crop Trees: height ≈ 73 × diameter" | authoritative now |
| Wind diagnostic | production wind formula with forecast light and opening | **not recommended for display yet** | At Year 0 every tree already reads "high" under the scene thresholds (5/12): all 336 trees exceed 12, peak 29.7 untreated [HARNESS]. The label cannot discriminate. Recalibrate after the Sitka height work (Sol) — **PRODUCT DECISION REQUIRED** |

## 3. Example: three defensible treatments compared (C4)

Same stand, same Crop Trees, Year 0 [HARNESS `residual-stand-immediate.csv`]:

| | **A — conservative** (T4: 1 competitor per Crop Tree) | **B — Crop-Tree release** (T2: 2 per Crop Tree) | **C — stronger release** (T3: 6 per Crop Tree) |
|---|---|---|---|
| Trees removed | 15 | 30 | 87 |
| Volume removed | 2.74 m³ | 5.19 m³ | 13.52 m³ |
| Gross income | €134.46 | €250.24 | €628.73 |
| Contractor cost (incl. minimum) | €2,500 | €2,500 | €2,500 |
| **Net** | **−€2,365.54** | **−€2,249.76** | **−€1,871.27** |
| Basal area left (m²/ha) | 38.6 | 36.2 | 28.0 |
| Crop Trees released ≥10 % | 10 of 16 | 16 of 16 | 16 of 16 (all ≥25 %) |
| Crop-Tree growth change | +6.4 % | +11.7 % | +30.8 % |
| Cells touched / largest opening | 14 / 2 | 23 / 3 | 33 / 12 |
| Cells above 0.20 light | 5 | 5 | 8 |
| Crop DBH at Year 20 (no further work) | 29.27 cm | 29.73 cm | 31.33 cm |
| Regenerating cells at Year 20 | 39 | 40 | 46 |

How to read it (explanation text, no verdict):

- **A** keeps the most capital and cover and changes little. A good choice if you plan to return soon.
- **B** releases every Crop Tree moderately with modest disturbance.
- **C** gives the strongest release and loses the least money on this visit, because the fixed contractor minimum is spread over more timber. It opens the canopy most and leaves the least capital.

None is labelled correct. A plan is described as *plainly mistaken* only when it fails its own stated purpose, e.g. "release my Crop Trees" with 0 of 16 released (see `NoviceErrorLibrary.md`).

**Important economic observation [HARNESS]:** at Year 0 the gross roadside value of the *entire* stand is about €1,702. That is below the €2,500 contractor minimum (`minimumHarvestJobCents`, a manager-approved [C] value). **Any** first-year thinning therefore loses at least about €800, whatever is marked. At Year 10 the whole stand is worth about €4,063. This is a calibration fact of the bounded 0.16 ha property (D-041), not a teaching choice. The review should explain it; it should not hide it. Whether that is the intended experience is **PRODUCT DECISION REQUIRED** (economy owner) — see `DecisionMatrix.md`.

## 4. The "no correct answer" safeguards

1. Show at least two plans side by side, never a single plan with a grade.
2. Every number has a plain-language sentence; no colour code means "good" or "bad" (neutral colours only).
3. A **purpose** chosen by the player frames the review ("You aimed to release Crop Trees: 16 of 16 released").
4. Dimension order is fixed (economic → silvicultural → ecological → stability), so no dimension looks like the "score".
5. No totals across dimensions, no stars, no percentages of an ideal.

## 5. Tests

1. Same marks on the same stand produce byte-identical review numbers in two processes (pattern proven: identical output SHA-256 across two harness processes).
2. Equal removed volume with a different spatial pattern produces different spatial metrics (T2 vs T5: 23 vs 11 cells; largest opening 3 vs 9).
3. Harvested value and retained capital are both reported for every plan.
4. No field named "score", "grade" or "rating" exists in the review model (static test).
