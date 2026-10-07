# Storm performance plan

**Status:** architecture requirements. Estimates are static, not profiled.

## 1. Pipeline (one storm, one annual step)

```
1. evaluate event        Layer 1: 3 hash rolls                                   O(1)
2. candidate pass        one pass over living trees (sorted by id, as FindTrees):
                         bucket by cell; compute local top height per 3×3 block;
                         compute V_i from cached H, D, cell light, RO               O(N)
3. victim rolls          one id-keyed Roll per candidate with V_i > ε               O(N)
4. batch apply           BeginChangeBatch(); ApplyMortality("windthrow", y) for each victim;
                         RecentOpening += 1 per victim cell (cap); EndChangeBatch()
                         → ONE RecomputeCanopy + ONE RecomputeSeedRain               O(N × cells)
5. records               manager creates one deadwood record per victim (existing handler)
                         + one storm event record                                   O(victims)
6. visuals               spawn root plate + directed log (+ crown) per victim, once,
                         after the batch; habitat dressing rebuild once               O(victims)
```

**Avoid (packet requirement):**

- one canopy rebuild per tree: prevented by the existing batch (`QueueLivingCanopyRebuild` defers inside a batch);
- one save per tree: storms never save; saving stays a player action;
- one whole-world query per victim: no `FindObjectsByType` inside the victim loop; reuse the step-2 lists.

Growth Model 1 adult mortality already follows exactly this batch pattern (`ApplyAdultDensityMortality`). The storm resolver should mirror it.

## 2. Costs at Scenario One scale

- N ≤ ~360 living trees (336 start, recruits later); 64 cells.
- Steps 2–3: ~360 × small constant. Microseconds to sub-millisecond.
- Step 4: one canopy rebuild = 64 cells × N trees (already done several times per annual step).
- Victims per storm: target ≤ 25 % of stems in an extreme worst case → ≤ ~90.

## 3. Rendering

| Item | Per victim | ×90 worst case | Note |
|---|---|---|---|
| Root plate | 1,928 tris LOD0 / 548 LOD2 | 174k at LOD0 if all are near (they won't be) | LODGroup; within walking view maybe 10–20 |
| Log (segmented ×3) | 1,728 LOD0 | 156k LOD0 worst case | cheap |
| Fresh crown, Option A (reused living model) | ~40k LOD0 / ~6.8k LOD2 | **3.6M LOD0 if all near**; realistic view ~10–20 near → 0.4–0.8M | Highest cost. Mitigations: (1) crowns only for victims within N m of the player, or LOD1 minimum; (2) drop the crown after 2 years; (3) Option B debris asset (≤ 8k) |
| Draw calls | ~3–5 renderers per victim | ~300–450 | SRP Batcher + shared materials. Consider GPU instancing for root plates/logs (same mesh/material) |
| Habitat dressing (fungi/moss on deadwood) | existing per-record rules | scales with records | Already exists for self-thinning deadwood (Growth Model 1 creates hundreds of records by age 60: 249 deaths unthinned) → **storm v1 adds less than Growth Model 1 already does** |

**Important context:** Growth Model 1 self-thinning already produces hundreds of deadwood records and log visuals over a century (609 deaths unthinned to age 120). The storm packet should **measure deadwood-visual cost at Year 60–100 under Growth Model 1 first**, as a baseline. Storm logs add to an already-large population.

## 4. Requirements for the implementing packet

1. One canopy/seed rebuild per storm (assert in a test by counting rebuild calls, or via an exposed diagnostic counter).
2. Victims evaluated from cached lists: no per-victim scene queries.
3. Visual spawn once per annual step, after the ecology batch.
4. Worst-case fixture: forced Extreme storm at Year 30 after heavy thinning. Report frame time for the annual-step frame and steady-state walking frame time in the storm gap (rendered, interactive gate).
5. Long-run fixture: 100 years, stochastic storms + Growth Model 1. Report deadwood record count and visual instance count at Years 50/100.
6. Budget (reference GTX 980M class, editor): the annual-step frame may spike, but stay ≤ 1 s. Walking frame time in the storm area within +20 % of the same view without storm visuals [C].
