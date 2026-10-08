# No single forestry score — multidimensional feedback (Workstream E)

**Status:** design proposal. Consistent with the Game Brief ("Do not imply one universally correct forestry prescription") and pedagogy decision paper 5. **Not yet a decision** (`DecisionMatrix.md` #E1).

## 1. Principle

The game explains **trade-offs between dimensions**. It never totals them. No "73 % sustainable", no "CCF score", no correct/incorrect treatment, no stars, no grade, no colour scale from bad to good across dimensions.

A plan can be called **inconsistent** only against the player's *own* stated intention, and that is a later feature. In v1 there is no stated intention, so v1 only describes.

## 2. Dimensions and their authoritative indicators

| Dimension | Question for the player | Indicators AVAILABLE NOW [REPO] | Later | Never |
|---|---|---|---|---|
| **ECONOMY** | What did it cost or earn, and what is left? | Cash, job net, small-job minimum, timber by assortment, notional standing value, owner hours | Salvage (storms), grants (policy layer, not accepted) | "profit score", NPV claims |
| **SILVICULTURE** | Are the trees I chose to keep growing better? | Crop Tree CI and growth withheld (before → after), Crop Tree DBH, pruning lifts | Crop Tree growth history (diary) | "quality grade" without a quality state |
| **REGENERATION** | Is a next generation establishing, of what, and what limits it? | Regenerating cells by species/origin, planted survival, light bands, seed presence, browse state, shelters | Vegetation competition limits (M2); per-cause loss lines (M2 ledger) | "regeneration score"; stems/ha (D-047: undefined) |
| **STRUCTURE** | Is the forest becoming more varied in size and layering? | DBH coefficient of variation (snapshot `dbhCoefficientOfVariation`), canopy, living trees, basal area, size classes, juvenile cohorts by age band (Model 1 bands) | Height layers (from Growth Model 1 heights) | "inverse-J achieved" |
| **STABILITY** | How exposed is the stand to wind? | *Nothing honest yet*: current wind labels are saturated | Stable / Watch / Exposed bands [STORM-DESIGN]; storm history | Storm probability; "safe" |
| **HABITAT** | What structures for wildlife are present? | Fallen deadwood volume, count and decay classes; ground-flora groups (descriptive) | Bramble/bracken as structure (M2), windthrow deadwood (storms) | Biodiversity score; aggregated habitat value (weights are [D]) |

## 3. Where it appears

- **Work Plan "What you are leaving"** (P3): Economy, Silviculture, Structure, Regeneration (where derivable) and Habitat lines.
- **Annual Review v2** (P4): the same dimensions as change-over-the-year.
- **Forest Diary** (P4): the same dimensions as trends.
- **Scenario completion** (P6): each dimension shown as the player's *trajectory*, not a pass mark (`ScenarioCompletionDesign.md`).

Layout rule: dimensions are **rows of facts in a fixed order**. They are not bars of equal length that invite visual summing, and not radar charts.

## 4. Eight credible plans that differ without one being wrong

Year-0 figures are real [HARNESS-Y0]/[PROTO]. "Later" effects are qualitative and follow the game's own mechanisms [INF]. They are **not** measured under Growth Model 1 (re-measure before quoting).

| # | Plan | Economy | Silviculture | Regeneration | Structure | Stability (after storms) | Habitat | Why it is credible |
|---|---|---|---|---|---|---|---|---|
| 1 | **Conservative release** (T4: top competitor of each of 16 Crop Trees, 15 trees) | −€2,366 (minimum dominates) | Crop CI −11 %; 10/16 released ≥ 10 % | Little new light (cells ≥ 0.20: 4 → 5) | Small change | Least exposure change | Unchanged | Keeps capital; a cautious first visit; plans to come back |
| 2 | **Crop-tree release** (T2: top 2 each, 30 trees) | −€2,250 | Crop CI −20 %; 16/16 released | Modest light | Small groups | Moderate | Unchanged | The textbook positive-selection first intervention |
| 3 | **Heavy release** (T3: top 6 each, 87 trees) | −€1,871 (more timber offsets the minimum) | Crop CI −45 % | Much more light; an 8-cell connected opening | Bigger structural change | Most exposure (later) | Unchanged | Fewer future visits; a deliberate trade of capital and stability for speed |
| 4 | **Concentrated group** (T5: one gap, 48 trees) | −€2,284 | Crop CI −16 %; only 7 released | Brightest single area (11 cells ≥ 0.20) | Starts a new cohort patch | New edge (later) | Unchanged | Classic group-selection start for regeneration |
| 5 | **Release + leave deadwood** (T2 with 5 stems left as deadwood) | Less revenue (deadwood is unsold) | As T2 | As T2 | As T2 | As T2 | +deadwood volume, a fresh-log habitat pulse | Habitat retention has a measurable timber opportunity cost ([ECON] "deadwood opportunity cost") |
| 6 | **Wait** (no intervention for 10 years, then thin) | €0 now; the later thinning is worth more but still pays the minimum | Crop Trees keep competing; stand closes | Natural Sitka regeneration under shade only | Uniform | Unchanged | Self-thinning deadwood appears (Growth Model 1) | Legitimate on a tiny property where every visit costs €2,500 |
| 7 | **Release + enrichment planting** (T2 + 12 Oak with shelters in the brightest cells) | T2 − about €216 (12 × €5.50 stock + €7.50 contractor planting + €5 shelter [D]) | As T2 | Broadleaves introduced where light allows | Species mixture begins | As T2 | As T2 | Targeted planting where seed sources are absent |
| 8 | **Release + clearance before planting** (plan 7 + cell clearance) | Plan 7 − ~€5 per cell | As T2 | Main: removes existing young growth with no survival benefit. **M2: can help where bramble/bracken is dense, waste money where it is not, or destroy good regeneration** | — | — | Removes ground vegetation for a while | Shows clearance is conditional |
| 9 | **Clean-up thinning** (T1: 84 smallest crowded stems) | −€2,347 | Crop CI −17 %, but 6 removals are > 8 m from every Crop Tree | Many small openings | Removes the suppressed layer (less size variation) | — | Removes future self-thinning deadwood | Common novice instinct. Not "wrong" in a stand-tidiness sense, but releases Crop Trees less per tree removed |

Reading the table: **no plan dominates every column.** Plan 3 releases most and earns most, but leaves the least capital and (later) the most exposure. Plan 6 costs nothing now but releases nothing. Plan 9 removes the most trees for one of the smaller releases.

## 5. How the game says it (copy pattern)

> "This plan releases your Crop Trees strongly and lets in more light. It also removes more growing stock, and opens one large area."

> "This plan keeps most of the stand. Your Crop Trees gain a little room. You will probably want to come back sooner."

Pattern: **mechanism → consequence → consequence**. No adjective of quality ("good", "poor", "ideal"). No imperative.

## 6. Tests (for the implementing packets)

- A static deny-list over all review strings: "score", "grade", "rating", "%" attached to "sustainable"/"CCF", "correct", "wrong", "should", "best", "ideal", "optimal".
- No numeric aggregate field across dimensions in any review model class.
- Determinism of text for identical state.
