# Enlarged 80 × 80 m Stand — Area-Sensitive Calibration Evidence

Status: **evidence only. No value in this table was changed.** The Manager decision for this task ("do not recalibrate economy
or objectives; produce an explicit calibration table") is followed. Every value below is read from
`Assets/ForestPrototype/ScenarioOne/ScenarioOne.asset` / `ScenarioOneDefinition.cs`, or measured by the disposable harnesses
named in the handoff. `definitionVersion` stays `scenario-one-v13`.

Geometry facts used throughout: Legacy40 = 40 × 40 m, 0.16 ha, 64 cells, 336 living trees. Enlarged80 = 80 × 80 m, 0.64 ha,
256 cells, 1,344 living trees. Stocking is identical (2,100 stems/ha). Every count-based quantity therefore has a **×4** area
ratio; intensive quantities (means, shares, per-unit prices) do not.

## 1. Count and volume thresholds (these change meaning with area)

| Quantity | Value (unchanged) | Legacy40 meaning | Enlarged80 meaning | Equivalent if scaled ×4 (NOT applied) |
|---|---|---|---|---|
| `minimumRetainedOriginalTrees` (objective "Retain original canopy trees") | 60 trees | 17.9 % of 336 | 4.5 % of 1,344 | 240 |
| `minimumRegenerationCells` (objective "Maintain regenerating cells") | 3 cells | 4.7 % of 64 | 1.2 % of 256 | 12 |
| `minimumDeadwoodVolumeM3` (objective "Retain fallen deadwood") | 0.02 m³ | 0.125 m³/ha | 0.031 m³/ha | 0.08 m³ |
| `referenceOriginalTrees` (century aspirational target) | 120 trees | 35.7 % of 336 | 8.9 % of 1,344 | 480 |
| `referenceBroadleafPresence` (trees + regenerating cells per planted species) | 10 | 15.6 % of 64 cells | 3.9 % of 256 cells | 40 |
| `referenceRegenerationCells` | 12 cells | 18.8 % of 64 | 4.7 % of 256 | 48 |
| `referenceDeadwoodVolumeM3` | 0.5 m³ | 3.1 m³/ha | 0.78 m³/ha | 2.0 m³ |

Measured example (Enlarged80, real new game, 20 marked trees thinned in year 0 → annual review year 1, from
`Enlarged80Review`): "Retain original canopy trees: 1324 / 60" was already **done**; regenerating cells went 0 → 30 against a
minimum of 3. In Legacy40 the retained-canopy objective only bites if more than 82 % of the original trees are removed; in
Enlarged80 it only bites above 95.5 %. The count-based objectives are therefore much easier in the enlarged world. Whether that is
wanted is a Manager decision, not a code decision.

## 2. Economy values

| Quantity | Value (unchanged) | Effect of ×4 area |
|---|---|---|
| `startingCashCents` | €12,000 | €75,000/ha → €18,750/ha. Still buys the same 4.8 minimum harvest visits. |
| `minimumHarvestJobCents` | €2,500 per annual commissioned job | Per-job charge, independent of area. The same thinning *share* spreads it over 4× the trees: €52.08/tree (48 trees) → €13.02/tree (192 trees). |
| `ownerMinutesPerYear` | 2,400 (40 h) | 15,000 min/ha → 3,750 min/ha. At 10 min per sapling the owner can plant ≤240 saplings/year: 1,500/ha in Legacy40, 375/ha in Enlarged80. Planting-heavy strategies are 4× more constrained per hectare. |
| Sapling prices (€4.50 beech, €5.50 oak), `treeShelterMaterialCents` (€5.00) | per unit | Not area dependent. |
| `contractorHourlyRateCents`, felling/pruning minutes | per tree / per hour | Not area dependent; total work for a given *share* scales ×4. |

Measured (audit live mode, identical marking rule in both worlds: 14.3 % of the living trees marked at an even stride through
tree-ID order, i.e. about every 7th tree, after the year-1 advance; annual cycle with the €2,500 minimum charge; Editor, no
render). This is a benchmark rule, not a silvicultural prescription, and in ID order it is not spatially uniform:

| | Legacy40 (override) | Enlarged80 (default) |
|---|---|---|
| Trees marked / living after | 48 / 288 | 192 / 1,152 |
| Timber revenue, intervention year | €283.41 | €1,011.51 |
| Harvested volume | 6.39 m³ | 23.41 m³ |
| Cash after year 3 (from €12,000) | €9,783.41 | €10,511.51 |

At equal thinning share the minimum charge swallows far less of the revenue in the enlarged stand (net of the €2,500 job:
−€2,216.59 vs −€1,488.49). In Enlarged80 the 20-tree thinning used by the review was net −€2,412.13 (revenue €87.87), i.e. a
small job in a large stand is dominated by the minimum charge, as the Work Plan already explains to the player.
Note: the Phase 1 scratch 80 m candidate ended year 3 with €10,685.02 under the same rule, because its tree layout differed (a
density-preserving lattice, not the implemented core-plus-ring generator); the implemented figures above are the ones that
matter.

## 3. Intensive quantities (no area scaling, listed for completeness)

`minimumMeanCanopy` 0.35 and `referenceMeanCanopy` 0.65 are means over cells. A fixed number of felled trees moves the mean four
times less in the enlarged stand. `backgroundBrowsePressure`, understorey colonisation/loss rates, habitat weights and
`minimumCompletionYear` (25) / `centuryReviewYear` (100) are rates or times, not counts.

## 4. Century review in Enlarged80 (consequence of the Reference boundary, not a defect)

`ScenarioReferenceArchive.Matches` requires the live geometry to be Legacy40, so `ReferenceArchiveAvailable` is false in an
Enlarged80 game and `ScenarioOneObjectives.Review` falls back to the *aspirational design targets* in section 1
(`referenceOriginalTrees` 120 etc.). Those targets were set for 336 trees. The Reference Future v1 *preview* still works from an
Enlarged80 game (it temporarily applies Legacy40 and restores the exact player world; `StandGeometryVerification` and
`Enlarged80Verification` assert this), but century comparison against a real Reference run needs a Reference Future for
Enlarged80, which does not exist and is out of scope.

## 5. Calibration options for the Manager (none applied)

* **A. Leave as is.** Count-based objectives are about 4× easier; the century comparison uses weak targets.
* **B. Scale count-based thresholds ×4** (values in the right-hand column of section 1). Needs `ScenarioOne.asset` changes, which
  are single-writer serialized data and interact with `Matches`/`definitionVersion` identity.
* **C. Express count thresholds as shares of the live stand** (percentages of original trees / cells), so they are
  geometry-independent. A code change plus a definition migration decision.

Recommendation: C or B as its own bounded task *after* the pilot-relevant playtest of the enlarged stand, not inside this
foundation change. Until then the numbers above are the honest record of what the enlarged world does with the current
calibration.
