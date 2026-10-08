# Windthrow outcome design

**Status:** proposal. The smallest gameplay-complete model.

## 1. Candidate outcomes

| Outcome | Biological meaning | Visual | Salvage | Deadwood | Assets |
|---|---|---|---|---|---|
| **UPROOTED / FALLEN** | whole tree overturned; root plate lifted | fallen stem along the storm direction, root plate at the base, pit | full stem, with a wind-damage downgrade | full stem as lying deadwood | root plate **exists**; log exists (stretched); full fallen tree with crown missing |
| SNAPPED STEM | stem broken at some height; stump/snag stands; top falls | standing snag (high stump) + fallen top | top only (shorter, more damaged) | lying top + **standing** snag | snag/high stump **missing** (ring-barked dead Sitka is a different look) |
| SURVIVES | — | nothing (v1) | — | — | — |

## 2. Does v1 need both?

**No. v1 uses one fallen state: `windthrow` = uprooted and fallen.**

| Criterion | One state | Both states |
|---|---|---|
| Gameplay-complete? | Yes: loss, gap, light, deadwood, salvage, visibility | Adds a standing-snag habitat and a different salvage fraction |
| Evidence need | Uprooting is a common Sitka failure on wet/shallow soils [I]; default site "mineral, moderate" | Needs a soil-dependent split rule [I] that we cannot calibrate yet |
| Save | cause string only | plus a break height (or a state enum) per tree |
| Assets | root plate exists | snag asset needed (BLOCKER for that state) |
| Deadwood model | lying deadwood only (existing record) | the standing-deadwood record type does not exist (ring-barking state is also deferred, D-020) |

**Recommendation:** v1 = `windthrow` only. **v1.1** adds `windsnap` once a standing-deadwood record and snag asset exist, sharing the ring-barked "standing dead" work.

## 3. Authoritative effects of a windthrown tree (all required)

| Requirement | Mechanism (reuse first) |
|---|---|
| Stops being live growing stock | `ForestTree.ApplyMortality("windthrow", year)`: tree inactive, excluded from competition, growth, seed and canopy |
| Affects canopy/light | Existing `OnTreeMortality` → batched `RecomputeCanopy` and `RecomputeSeedRain` once per storm |
| Enters deadwood | Existing `ScenarioOneManager.OnTreeBiologicalDeath` → one `ScenarioDeadwoodRecord` per tree. **Change needed:** the handler is gated on growth model ≥ 1. Storms on growth model 0 saves are not planned (new games only), so this is acceptable. Document it |
| Remains spatially visible | Fallen-log visual (exists), plus a **root plate at the base**, plus **fall direction from the storm**. Visual-only, derived from cause + storm record + tree hash |
| Creates new exposure | Storm resolver adds `+1` `RecentOpening` per victim to its cell (cap 2) |
| Potentially salvageable | Deadwood record is the salvage target (`SalvageDesign.md`) |
| Recorded for history | cause/year on the tree (saved), plus the storm event record |

## 4. Geometry of the fallen tree (presentation, deterministic)

- Fall bearing = storm direction (the direction the wind blows *to*) ± hash(treeId) × 20°.
- Root plate placed at the stem base, rotated to the fall bearing, scaled by DBH (e.g. plate diameter ≈ 1.5–2.5 m for 25–40 cm DBH [C, visual only]).
- Log from base to 0.8 × height along the bearing (current log length rule). **Note:** the current visual centres the log on the stem position. For windthrow the log must start at the root plate, so the base anchor needs an offset.
- Clip or skip any log that would leave the stand bounds or cross the forest road. Visual only; the record keeps the true base position.

## 5. What does **not** happen in v1

- No damage to neighbours from the falling tree (no dominoes, no crushing of regeneration).
- No soil pit/mound microsites affecting regeneration (an integration point for Sol's understorey/regeneration work, `StormEventArchitecture.md` §6).
- No change to timber quality of *surviving* neighbours.
