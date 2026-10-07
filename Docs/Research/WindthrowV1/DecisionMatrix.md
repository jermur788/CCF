# Storms / windthrow / salvage v1 — decision matrix

**Status:** classification for Manager/user review. Nothing here is accepted.

| # | Item | Class | Recommendation |
|---|---|---|---|
| 1 | Storms/windthrow exist as a separate mortality cause (deferred in D-044) | **PRODUCT DECISION REQUIRED** (lifting the D-044 deferral) | Proceed as Storms v1 after Regeneration Model 2 integrates |
| 2 | Two-layer architecture (storm event × tree vulnerability); no annual per-tree probability | SUPPORTED DIRECTION ([A] IRL) | Accept |
| 3 | Storm probability and severity as **scenario parameters [C]**, labelled as game priors | SUPPORTED DIRECTION ([B] IRL, ATL) | Accept; defaults p = 0.08, M/S/X 0.70/0.25/0.05, grace 3 years, confirmed by calibration |
| 4 | Storms on for new games vs opt-in | **PRODUCT DECISION REQUIRED** | On for new games (the model is versioned; old saves unaffected). A difficulty toggle is a later UI decision |
| 5 | Scenario One soil / site wind hazard | **PRODUCT DECISION REQUIRED** | "Mineral, moderate" (×1.0). Peat/gley presets for future scenarios |
| 6 | Property surroundings per side (wind only in v1) | **PRODUCT DECISION REQUIRED** | South open (road/field), others sheltered; fallback all sheltered |
| 7 | Competition/light/seed boundary effects | FUTURE / DEFERRED | Separate packet; re-run edge-bias under C8 + Growth Model 1 first |
| 8 | Vulnerability index form (`TreeVulnerabilityDesign.md`) | NEW PROPOSAL | Accept the shape; weights are [C] |
| 9 | Recent thinning from `RecentOpening` (3 × 3 cells) + windthrow increments it | NEW PROPOSAL | Accept; no new per-tree field |
| 10 | One outcome state (`windthrow`, uprooted) in v1; snapping v1.1 | NEW PROPOSAL | Accept |
| 11 | Salvage as per-stem work orders reusing the harvest job, timber yield (`WindDamage` sections) and `SiteCostBasisPoints` | NEW PROPOSAL | Accept |
| 12 | Bulk "add all windthrow from Year N" in the Work Plan vs D-010 (no remote spatial design) | **PRODUCT DECISION REQUIRED** | Allow: the list is defined by the storm, not by the player's spatial choice |
| 13 | Salvage window (DecayClass ≤ 1) and value decay | NEW PROPOSAL [C] | Accept for calibration |
| 14 | Replace "Wind exposure: high" with Stable / Watch / Exposed + reason | NEW PROPOSAL | Accept; it fixes a known misleading label (pedagogy research decision #26) |
| 15 | No numeric storm or per-tree chance shown | SUPPORTED DIRECTION (game brief, pedagogy no-score principle) | Accept |
| 16 | Save: `stormModel` + `stormEvents[]` + enum additions | NEW PROPOSAL (save schema) | Accept; next free version after Sol's v18 |
| 17 | Fresh-crown visual via the reused living model (Option A) vs new debris asset (Option B) | **PRODUCT DECISION REQUIRED** (art/performance) | A for v1 with distance limits; B if the profile fails |
| 18 | Soil-pit asset | NEW PROPOSAL (art) | Create (small) |
| 19 | Storm damage to juveniles/regeneration (crushing) | FUTURE / DEFERRED | v2 with a footprint model |
| 20 | Pit/mound microsites for regeneration | FUTURE / DEFERRED | Integration point for Regeneration Model 2+ |
| 21 | Contractor-quality / residual-damage layer | FUTURE / DEFERRED | — |
| 22 | Completion objectives under storms (`retained-canopy ≥ 60` may fail after severe storms) | **PRODUCT DECISION REQUIRED** if calibration shows < 90 % viability | Prefer calibrating storms over changing objectives |
| 23 | Wind-damage timber quality on *surviving* trees | FUTURE / DEFERRED | — |
| 24 | Pest outbreaks after windthrow | FUTURE / DEFERRED (not Ireland v1) | — |
