# Research backlog (Part 13)

Ranked by what blocks implementation. The existing broad reports are **not** treated as stronger than they are:
- the Atlantic/Irish ecology reports give strategy roles and orderings, not calibrated coefficients;
- [R:EC] gives Irish Sitka prices and explicitly no robust minimum-job or species price schedule;
- [R:PS] is practitioner guidance (supporting, continental transfer caution).

Tags: **BLOCKS** = implementation cannot responsibly proceed; **CAL** = useful for calibration; **VAL** = later validation.

| Rank | ID | Question | Needed for | Tag | Existing project evidence | Likely sources to seek |
|---|---|---|---|---|---|---|
| 1 | **R1** | Height/site-index curves (or yield-class tables) for Douglas fir, Norway spruce, western hemlock, western red cedar, Scots pine; for birch/oak/beech in Ireland | G7, Wave B | **BLOCKS** Wave B | **None** for these conifers; oak/beech partial (BeechSpeciesParameters) | Irish/British yield models (Forest Research yield tables), Irish site-index work (as Lekwadi 2012 for Sitka) |
| 2 | **R2** | Shade tolerance orderings + juvenile light anchors (establishment/survival/growth) for all candidates | Wave A, B | **BLOCKS** (ordinal classes at minimum) | Sitka ~20 % RLI [R:SD]; birch/rowan/holly qualitative [R:WE] | Silvics literature; FC CCF guidance; light-response studies |
| 3 | **R3** | Assortment specifications (small-end diameter, lengths, defects) per productive species in Irish markets | T1, T3 | **BLOCKS** markets | Sitka only (Teagasc via [R:EC]) | Teagasc, Coillte/merchant specs, IFA |
| 4 | **R4** | Price basis per species/assortment (or a documented relation to Sitka) | T2–T4 | **BLOCKS** markets (no fabricated prices) | Sitka IFA 2024 roadside [R:EC] | IFA timber price surveys, Teagasc; hardwood market reports |
| 5 | **R5** | Maximum density (SDI slope/max) by species or species group; validity of additive relative density in mixtures | G3 | **BLOCKS** a species-aware self-thinning beyond Sitka | Sitka (Comeau 2010, [B]) | Density-management literature for the listed species |
| 6 | **R6** | Crown light transmission / shade-casting ordering by species | G2 | **BLOCKS** (ordinal classes) | None | Canopy transmittance studies |
| 7 | **R7** | Site tolerance envelopes (moisture, nutrients, exposure) per species | G4 | **BLOCKS** site matching (classes) | Native Forest Framework templates [R:SD] for native species | Irish species-site guides (Teagasc/DAFM), ESC-type systems [TR] |
| 8 | **R8** | Seed production onset, dispersal mode/scale, mast behaviour for new species | Wave A/B | CAL | Birch/rowan/holly qualitative [R:WE] | Silvics |
| 9 | **R9** | Browse palatability ordering for new species (deer in Ireland) | Wave A/B | CAL (ordering exists for some) | [Browsing report] for oak/beech/Sitka | Irish deer studies |
| 10 | **R10** | Longevity / senescence onset (birch) | G6 | CAL | None | Silvics |
| 11 | **R11** | Stem form distributions and quality grading rules (form classes, browse forks, pruning) | Quality wave | CAL | Pruning guidance [R:EC] | Hardwood quality grading guides |
| 12 | **R12** | Broadleaf harvesting work rates vs conifer | Contractor model | CAL | None | Work-study literature |
| 13 | **R13** | Storm rooting differences by species × wetness | Later storm depth | VAL | Generic [WindthrowV1] | Windthrow literature |
| 14 | **R14** | Plant-health constraints currently in force (larch *P. ramorum*, ash dieback) | Species availability | **BLOCKS** inclusion decisions only | None | DAFM plant-health notices |
| 15 | **R15** | Expert validation of mixed-stand trajectories | Forester review | VAL | — | Forester review protocol (Scenario One readiness) |

**Minimum to start the Foundation + Wave A:** R2 (ordinal), R6 (ordinal), R7 (classes for birch/rowan/holly/oak/beech/Sitka), R10 for birch. Wave A is achievable on ordinal evidence plus calibration. **Wave B needs R1, R3, R4 (and R5 for credible mixtures) first.**
