# Recent thinning: how current state can know it

**Status:** assessment and recommendation. **No new persistent field is needed for v1.**

## 1. What the player and simulation need to know

For a tree at storm time: were neighbours removed recently, when, and how strong was the local opening?

## 2. Existing sources [REPO]

| Source | Saved? | Granularity | Timing | Strength | Limits |
|---|---|---|---|---|---|
| `ForestEcologyCell.RecentOpening` | **yes** (`ForestCellSaveData.recentOpening`) | 5 m cell | implicit: +1 per felled tree, × 0.5^(1/3) per year (3-yr half-life) | count of felled trees, capped at 2 | cap saturates group fellings; cell edges (a tree 0.5 m from a felled cell gets 0) |
| `ScenarioManagementEvent` (`WorkResolved`, `FellTree`, `Succeeded`) | **yes** | exact tree id, cell, world position | exact year | per tree (volume recorded) | needs a scan over events (bounded: Scenario One has at most hundreds of fellings) |
| Stumps (`ForestTree.stage == Stump`, saved) | yes | exact position | **no felling year on the tree** | — | year only via the event |
| `ScenarioDeadwoodRecord` | yes | exact position | `fallenYear` | volume | covers retained/fallen stems only, not extracted ones |
| Mortality (`mortalityCause/Year`) | yes | exact tree | exact year | — | self-thinning deaths of suppressed trees; tiny opening effect |
| Cell light | derived | cell | current | openness | does not distinguish new from old openings |

## 3. Recommendation for v1

**Use `RecentOpening`, read over the tree's own cell and its 8 neighbours, distance-weighted. Add windthrow openings to it through the same increment path as felling.**

```
RO_tree = max over the 3 × 3 cells c around the tree of
          cell[c].RecentOpening × w(distance from tree to the nearest point of c)
w(d) = 1 for d = 0 (own cell), linear to 0 at d = 5 m
```

Why:

- Saved, deterministic, already decaying with a 3-year half-life. It matches the [I] acclimation idea without a new rule.
- Already moves with felling (`OnTreeFelled`) and is already what the marking forecast reports, so the player's forecast and the storm read the same state.
- Reading 3 × 3 cells removes most of the cell-boundary artefact for trees near cell edges.

Required change (in the storm packet):

- Storm victims add `+1` to their cell's `RecentOpening` (capped), exactly like felling. Implemented in the storm resolver, **not** in `OnTreeMortality`, so self-thinning stays unchanged and existing anchors survive under `stormModel 0`.

## 4. When the event scan would be better (not v1)

Exact distances from felled stems (from events) would give a sharper "newly exposed edge" signal and avoid the cap. Cost: O(victim candidates × felled stems in the last ~10 years), which is small. **Consider in v1.1** if calibration shows the cell cap is too coarse, e.g. group fellings all reading as 2. It also needs no save field.

## 5. If persistent history were ever truly needed

Minimum field: **per tree, `exposureYear`** (last year a neighbour within 5 m was removed). It is not recommended: everything it would hold is derivable from events plus positions. Listed only so the alternative is explicit.
