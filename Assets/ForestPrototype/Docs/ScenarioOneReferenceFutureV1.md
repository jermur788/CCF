# Scenario One — Reference Future v1

One **authored, successful example** of Sitka-to-continuous-cover management,
not an optimal score or a universal thinning prescription. Its forest was
generated from the real 336-stem `ForestTest` start using Scenario One work
orders, contractor settlement and the unchanged authoritative Forestry annual
step. No final trees or regeneration cells were hand-constructed.

## Reproduction identity

| Field | Value |
| --- | --- |
| Reference / schedule | `reference-future-v1` |
| Scenario definition | `scenario-one-v12` |
| Forest save schema | v12 |
| Simulation seed | `20260914` |
| Starting stand | 336 living Sitka, 2,100 stems/ha |
| Canonical 80-year Sitka hash | `7E39B70A14959FAD` (separate regression fixture) |
| Initial full-world hash | `A564039D9B7CE31D` |
| Structured schedule hash | `56C8B99FA1E8DDD1` |
| Year-20 full-world hash | `F7DF7DAB53B6FD32` |
| Year-50 full-world hash | `D5E75D6D21D631AC` |
| Year-100 full-world hash | `7AD177B3CC2F73C7` |

This **v12 archive remains frozen** after the Scenario One v13 interaction
overhaul. Newer code verifies the unmodified original v12 milestone JSON hashes
before offering historical walkable previews; reserializing those worlds into
v13 adds fields and cannot reproduce v12 full-world hashes. V12 cell-based
planting orders continue to load on their historical cohort pathway. New
exact-position planting is an individual v13 pathway and has no claim to be
the same authored v12 reference run.

The exact schedule is `ScenarioOne/Resources/ScenarioOneReferenceScheduleV1.json`;
all decisions and their actual biological outcomes are in the saved management
events inside `ScenarioOneReferenceFutureV1.bytes`. The compressed archive also
contains **four full ordinary forest saves** (Year 0, 20, 50 and 100), not
only summary scores. `ScenarioReferenceArchive.WorldHash` canonicalizes save
list order before calculating its UTF-8 FNV-1a hash in this verified Editor
configuration. The reference-run driver can reproduce the run from the seed,
fresh stand and the authored schedule; it does not edit ecology parameters.

## Exact authored schedule

Years below are **treatment years**. The Work Plan is made in the preceding
ecological year, then approved work resolves before the one Forestry annual
step. BA percentages are of standing BA just before each treatment. Year 0
surveys and selects 84 future structural/seed Sitka (every fourth persistent
tree ID); 16 candidate Oak and 16 candidate Beech cells are recorded for
adaptive selection. Intensity and retention proportions are Scenario design
calibrations [D]. The readable JSON records every directive, count, targeting
rule, species, retention fraction and teaching note.

| Year | Management actually chosen |
| --- | --- |
| 1 | Selective release near retained Sitka: 49 stems, 15.3% BA; 5 retained as deadwood. |
| 5 | Second release: 40 stems, 13.3% BA; 4 deadwood. Clear competing Sitka cohorts at planting sites; designate 7 Oak and 9 Beech juveniles using nursery stock in brighter/partial-shade cells. |
| 10 | Around broadleaf establishment: 25 stems, 11.2% BA; 4 deadwood; selective Sitka cohort removal where competing. |
| 14 | Check *previously planted* pockets for failure; replace only if the cell remains suitable and has not promoted a tree. |
| 15 | Broadleaf release: 21 stems, 10.2% BA; 4 deadwood. |
| 20 | Low-intervention transition toward individual-tree/cohort management. |
| 25 | Local release: 12 stems, 5.2% BA; 2 deadwood. Prune selected unfelled, eligible timber stems. |
| 30 | Selective Sitka cohort control beside broadleaf patches; otherwise an empty Work Plan is valid. |
| 35 | Release after closure: 26 stems, 9.0% BA; 4 deadwood. |
| 40 | Deadwood-oriented release: 27 stems, 8.0% BA; 14 deadwood. |
| 45–50 | Selective pruning; check previous planting pockets rather than blanket replanting. |
| 55 | Remove 8 over-dominant Sitka with younger neighbours (8.2% BA); 2 deadwood; control selected Sitka regeneration near broadleaf trees. |
| 65 | Adaptive local release: 35 stems, 5.1% BA; 10 deadwood; selective Sitka cohort control. |
| 70 | Respond to local reclosure: 19 stems, 4.5% BA; 6 deadwood. |
| 75 | Remove only 6 older over-dominant Sitka where younger trees are ready (6.1% BA); 2 deadwood. |
| 85 | Limited release: 25 stems, 3.0% BA; 8 deadwood; selected pruning and regeneration control. |
| 90 | Light local release: 18 stems, 3.1% BA; 5 deadwood. |
| 100 | **No cosmetic final treatment.** Observe, record and freeze the resulting world. |

The authoring driver marks trees and imports two felling-order batches so
`SellAndExtract` and `RetainAsFallenDeadwood` use normal work resolution. It
buys shop stock by item ID, designates cell planting/removal and tree pruning
through the same public management API, approves within available cash, then
advances exactly one ecological year. Its spatial choices use stable IDs,
current light, persistent broadleaf establishment sites and local canopy
competition. Trees marked as future legacy structure are avoided by routine
release; limited over-dominant removals at Years 55/75 deliberately permit
generational turnover.

## Observed milestones

| Metric | Year 0 | Year 20 | Year 50 | Year 100 |
| --- | ---: | ---: | ---: | ---: |
| Living trees (Sitka / Oak / Beech) | 336 (336 / 0 / 0) | 217 (201 / 7 / 9) | 201 (185 / 7 / 9) | 206 (162 / 11 / 33) |
| Original Sitka retained | 336 | 201 | 162 | 127 (all age 120) |
| Natural Sitka / natural broadleaf individuals | 0 / 0 | 0 / 0 | 23 / 0 | 35 / 28 |
| Planted Oak / Beech promoted trees | 0 / 0 | 7 / 9 | 7 / 9 | 7 / 9 |
| Sitka / Oak / Beech regeneration cells | 0 / 0 / 0 | 59 / 0 / 0 | 57 / 0 / 1 | 58 / 3 / 15 |
| Sitka / Oak / Beech regeneration density (cell-summed model units) | 0 / 0 / 0 | 38.96 / 0 / 0 | 40.06 / 0 / 0.01 | 40.06 / 0.44 / 3.20 |
| Cells with any regeneration (of 64) | 0 | 59 | 57 | 58 |
| Mean canopy / mean light | 0.982 / 0.018 | 0.785 / 0.215 | 0.784 / 0.216 | 0.816 / 0.184 |
| Bright cells (light ≥0.30) / shaded cells (<0.10) | not captured | 19 / 33 | 15 / 24 | 13 / 26 |
| Largest connected very-open patch (light ≥0.80; cells) | not captured | 1 | 1 | 1 |
| Retained deadwood logs / remaining volume | 0 / 0 | 17 / 1.681 m³ | 37 / 1.423 m³ | 70 / 2.512 m³ |
| Mean grass / forb / shrub / fern cover [D] | 0 / 0 / 0 / 0.030 | 0.059 / 0.070 / 0.030 / 0.165 | 0.058 / 0.070 / 0.040 / 0.221 | 0.041 / 0.050 / 0.020 / 0.224 |
| Grassy cells (>0.10 cover) | 0 | 9 | 9 | 8 |
| Individual size classes | baseline | 2 | 3 | 3 |
| Individual age range | 20 | 19–40 | 11–70 | 12–120 (48 trees under 45) |
| Completed pruning treatments | 0 | 0 | 18 | 23 |
| Completed Sitka cohort removals | 0 | 12 | 15 | 28 |

Year-100 living trees include 4 naturally recruited Oak and 24 naturally
recruited Beech, with old Sitka in the upper canopy and 35 naturally recruited
Sitka contributing younger structure. The stand retains both shaded and
brighter cells. The baseline grassy/shrub/forb proxies are essentially zero;
local opening builds an understorey mosaic, without changing Forestry growth.

## Management and finances

- 241 trees sold/extracted; 70 retained as fallen deadwood;
- 16 juvenile plantings, 28 targeted Sitka-cohort removals, 23 prunings;
- 378 completed tasks, **zero failed tasks**;
- timber revenue **€4,253.49**, contractor cost **€4,043.25**;
- closing cash **€12,099.24**, from the configured €12,000 start after stock purchases;
- minimum annual mean canopy **0.73**; largest connected very-open patch
  **6 of 64 cells** across all 100 years. No intervention reset the canopy;
- Year 100 resolved **zero** work orders; there was no cosmetic final cut.

Prices and functional-group/deadwood/soundscape responses remain provisional
gameplay calibrations [D]. The exact Year-100 saved world — including actual
trees, cell cohorts, deadwood, inventory, reports, events and Century Review —
is loaded for the walkable in-game preview. `[Tab]` returns to the player's
forest without overwriting their save or changing its full-world hash.

## Revisions and verification

The first authored draft planted by *species ID* instead of the required
shop-item ID, and marked both felling outcomes in one import batch. Fixing
those management-driver mistakes produced real planting and deadwood. The
initial future-tree-centric releases maintained too much shade near maturing
broadleaf parents: it recruited 39 Sitka by Year 100 but no new broadleaf
individuals. The schedule was revised toward small, repeated **broadleaf-local
releases** and species-selective Sitka cohort control in Years 10–90; no species
ecology parameter was changed. The authoring driver attempted to avoid
same-year pruning/felling, but inspection of the **frozen** archive shows one
remaining case: `P0601` was both pruned and felled in Year 85. V13 Work Plan
validation correctly excludes that pruning; a replay under v13 is not entitled
to the exact v12 full-world hash. The historical archive is not rewritten to
remove its recorded treatment.

The Year-50 reload/replay found two **management-state defects**, not an
ecology-parameter defect: ecological summaries accumulated tree floats in
unspecified Unity object order after reload, and `JsonUtility` materialized an
absent Century Review as an empty Year-0 object. Sorting snapshot aggregation
by persistent tree ID and normalizing that empty review made the Year-50 to
Year-100 full-world replay exact. With the frozen schedule/resource installed,
the verified reproduction hashes all four milestones exactly. The separate
canonical Sitka test, full Scene Builder validation, save/load tests, clean
console gate and broadleaf lifecycle regressions remain mandatory before any
integration/push.
