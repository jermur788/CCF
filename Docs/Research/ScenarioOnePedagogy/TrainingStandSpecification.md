# Training stand specification (Workstream C2)

**Status:** NEW PROPOSAL [INF]. Every tree id and value comes from the deterministic Scenario One stand, measured by `ResidualStandEvaluation` [HARNESS] (`Evidence/residual-stand-trees.csv`, `residual-stand-cells.csv`, `residual-stand-neighbours.csv`; RNG 1, regeneration 1, context `3e4ee40`). **No new ecological variable is required.**

## 1. Two deterministic training contexts (no new content)

| Context | How it is produced | Why |
|---|---|---|
| **TS-0 "First thinning"** | Scenario One new game, Year 0 | Even-aged, crowded, no regeneration, no seed trees. Positive-selection and release cases |
| **TS-10 "Ten years on, untouched"** | TS-0 advanced 10 years with no management. Deterministic: same world in repeated runs [HARNESS] | Every tree is seed-bearing (age 30); Sitka regeneration up to 2.7 m on the road edge; bigger stems with more timber value. Seed-source, regeneration-patch and second-intervention cases |

Both contexts can be generated on demand, so nothing new needs to be stored. Because they depend on the current growth model, **their ids and values must be re-baselined after Sol's Sitka growth integration.**

## 2. Stand facts (TS-0)

| Fact | Value |
|---|---|
| Area | 40 × 40 m = 0.16 ha; 8 × 8 cells of 5 m |
| Trees | 336 Sitka spruce, age 20, ids `P{row}{col}` |
| DBH | 9.7–21.1 cm (median 15.5) |
| Height : diameter | 69–85 (median 75) |
| Basal area | 41.1 m²/ha; standing volume 39.9 m³ |
| Competition label | 328 "crowded", 8 "moderate" |
| Wind label | **336 "high"** (see finding below) |
| Seed-bearing trees | 0 |
| Bright cells (light ≥ 0.20) | D6 (work clearing, 0.84), E1, F1, G1 (forest road, 0.31–0.63); all treeless |
| Densest cells | A8, E8, H8 (9 stems each in 25 m²) |
| Gross roadside value of the whole stand | ≈ €1,702 (below the €2,500 contractor minimum) |

**Two label-calibration findings [HARNESS][REPO]** (they matter for any training panel):

- The wind label is **"high" for all 336 trees** in TS-0 and TS-10. Scene thresholds are low 5 / high 12; the unthinned stand's diagnostic is 17–30. The field's own tooltip says it was calibrated so an unthinned stand reads "moderate", so it is stale. It cannot support an "excessive exposure" case until recalibrated.
- The competition label is "crowded" for 328 of 336 trees. It does not discriminate between trees; the numeric value and the "% growth withheld" sentence do.

## 3. Decision cases (C2 checklist)

| Required case | Status | Training case (TS-0 unless stated) | What the player should discover |
|---|---|---|---|
| Obvious future quality tree | SUPPORTED (vigour only) | **P1614**: 21.0 cm, 14.6 m, interior, competition 7.8. **P0707**: 20.7 cm, 15.2 m, competition 8.1 | Among the largest, growing well, but held back by close neighbours |
| Weak / non-competing suppressed tree | SUPPORTED (distance version) | **P0710**: 10.7 cm, 8.9 m, "crowded", 6.0 m from P0707; supplies 1.1 % of P0707's competition | Small and suppressed ≠ a useful removal for this Crop Tree |
| True competitor | SUPPORTED | **P0706**: 20.5 cm, 1.54 m from P0707; 7.9 % of its competition (rank 1 of 48) | Bigger and closer competes most |
| Poor-form large competitor ("wolf tree") | **UNSUPPORTED** | — | No stem-form state exists. Do not fake it. Optional future content decision |
| Edge tree that looks best | SUPPORTED | **P0018**: 21.1 cm, the largest tree, at the road edge, competition 1.8 ("moderate") | The biggest trees often owe their size to the open edge. Being largest is not the same as being the best Crop Tree for the interior |
| Useful seed source | PARTIAL | **TS-10:** all 336 trees seed-bearing; one species only | Removing big trees now also removes seed. Seed diversity is impossible without broadleaf trees |
| Removal that creates excessive exposure | **UNSUPPORTED now** | — | Blocked by the saturated wind label; revisit after recalibration |
| Small gap | SUPPORTED | **D6** (work clearing, light 0.84). TS-10 adds E3/E4 (0.24–0.54) | A gap already exists; adding more light next to it is a choice, not a necessity |
| Dense group | SUPPORTED | **A8, E8, H8** (9 stems per cell; local basal area 74–80 m²/ha) | A dense group can be thinned gradually or treated as a unit [PRAC §11.4 bio groups; §15 Finland]. Not a reason to clear it |
| Regeneration patch | SUPPORTED in TS-10 | Road-edge cells **E1, F1, G1**: Sitka density 1.5 (cap), up to 2.7 m. Also D1. 28 regenerating cells in TS-10 | Regeneration appears where seed and light meet. Here that is the road edge, not the thinned interior |
| Browsing issue | PARTIAL | None without planting (Sitka palatability 0.15). Teach through the player's own planting (`RegenerationTeaching.md` §4) | — |

## 4. Training scripts

### TS-0 Exercise A — "Choose and release" (positive selection)

1. Choose 4 Crop Trees in the marked area (rows 6–8, columns 6–8, around P0707).
2. Inspect each one's neighbours and mark the removals you think are justified.
3. Review (`MarkingAssessmentFramework.md`): Crop Trees released, competition change, removals not near a Crop Tree, money.
4. Reset and try the "clean-up" version: remove the smallest trees in the area. Compare.

Expected discovery: the clean-up plan removes more trees, releases the Crop Trees less, and costs the same visit.

### TS-0 Exercise B — "Same volume, different forest" (spatial pattern)

1. Plan a ~5 m³ removal spread around Crop Trees.
2. Reset and plan the same volume as one group.
3. Compare: cells touched, largest opening, Crop Trees released, light. Harness reference: T2 vs T5 — 23 vs 11 cells, largest opening 3 vs 9, 16 vs 7 Crop Trees released ≥10 %.
4. Optional: "see it in 20 years".

### TS-10 Exercise C — "Second look"

1. Find the regeneration on the road edge; read why it is there (seed + light).
2. Mark a second thinning around your Crop Trees; the review now reports seed-bearing trees removed.
3. Compare with a TS-0 plan's long-term result.

## 5. What would need new content (not proposed now)

| Case | Would need | Decision |
|---|---|---|
| Poor-form large tree | a stem-form or quality attribute (new tree state + save + visuals) | PRODUCT DECISION; not needed for v1 training |
| Small suppressed stem beside a Crop Tree | an authored stand layout or a seeded variant | content decision |
| Broadleaf seed tree | an authored mixed stand or a later Scenario One state with planted broadleaves at age 40+ | content decision |
| Exposure case | recalibrated wind label (presentation) | **recommended**, small, after Sol's height work |
