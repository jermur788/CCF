# Spatial pattern feedback (Workstream F)

**Status:** design proposal backed by an offline robustness test [PROTO]. **No "ideal gap distribution" is proposed.**

## 1. Reasoning re-checked against current systems

The pedagogy harness (`60674f1`) showed that **equal removed volume** can leave very different forests. T2 (crop-tree release, 5.19 m³) and T5 (one concentrated gap, 5.22 m³) remove the same volume. T5 raises mean ground light to 0.130 versus 0.060 and puts 11 cells at light ≥ 0.20 versus 5. T2 releases Crop Trees more (−19.7 % vs −16.0 % CI) [HARNESS-Y0].

Is that still true at `a8596df`?

- **Immediate (Year 0) pattern: yes.** Spatial descriptors depend only on positions, DBH, crown radius and the light formula. Growth Model 1 changes none of these.
- **Consequences over time: changed and not re-measured.** Growth Model 1 adds density mortality biased toward suppressed trees. Gaps therefore interact with self-thinning, and opened areas should see fewer suppression deaths. The pedagogy branch's 20-year rows predate D-048 and are **not** reused. Re-run them before quoting any long-term contrast.
- **Regen Model 2 [M2-CAND]:** brighter openings raise bramble/bracken target cover (`target ∝ 0.1 + 0.9 × light`). A concentrated gap therefore also gets *more ground competition*, which makes the pattern lesson richer and more honest.
- **Storms [STORM-DESIGN]:** the proposed recent-opening exposure is per 3 × 3 cells. A concentrated gap makes "Exposed" edge trees. This is a later line, not v1.

## 2. Candidate descriptors and their robustness

Definitions (per 5 m cell, Year-0 stand):

- *Opened cell:* the plan removes ≥ **s** of that cell's basal area.
- *Opening groups:* 4-connected groups of opened cells.
- **CONCENTRATED GAP:** the largest group has ≥ **c** cells *and* holds ≥ **m** of all removed basal area.
- **SMALL GROUP OPENINGS:** otherwise, if opened cells together hold ≥ **m** of removed basal area.
- **DISTRIBUTED REMOVAL:** otherwise.

Each treatment was classified at **27 threshold combinations** (s ∈ {0.25, 0.30, 0.40}, c ∈ {3, 4, 5}, m ∈ {0.4, 0.5, 0.6}) [PROTO, `Evidence/spatial-pattern.csv`]:

| Plan (Year 0) | Removed | Cells touched | Opened cells (s = 0.30) | Largest group | Share of removal in largest group | Central verdict | Agreement across 27 |
|---|---:|---:|---:|---:|---:|---|---|
| T1 novice clean-up | 84 | 39 | 18 | 3 cells | 11 % | Small group openings | 21/27 (others: Distributed) |
| T2 crop-tree release | 30 | 23 | 9 | 2 | 8 % | Small group openings | **15/27** (12 Distributed) |
| T3 heavy (top 6) | 87 | 33 | 29 | 8 | 22 % | Small group openings | 27/27 |
| T4 conservative (top 1) | 15 | 14 | 2 | 1 | 16 % | **Distributed removal** | 27/27 |
| T5 concentrated gap | 48 | 11 | 10 | 10 | **99 %** | **Concentrated gap** | 27/27 |

**Findings:**

1. The **two ends are robust.** "Concentrated gap" (T5) and "Distributed removal" (T4) do not change with thresholds.
2. **The middle is not.** T2, a classic crop-tree release, flips between "small groups" and "distributed" depending on thresholds. A three-way label would mislabel exactly the plan the tutorial most wants to teach.
3. T3 shows a real hazard: a *heavy* plan made only of crop-tree releases joins up into an 8-cell connected opening. The label "small group openings" hides that. **The size of the largest opening is more informative than the label.**

## 3. Recommendation

Show **facts first, a label only when robust**:

```
Openings     Spread through 23 cells · largest opening about 2 cells (50 m²)
Pattern      Mixed: some small openings
```

| Descriptor shown | Rule |
|---|---|
| **One concentrated gap** | Largest group holds ≥ 60 % of removed basal area *and* is ≥ 4 cells. Robust in the prototype |
| **Spread out** | Cells losing ≥ 30 % of their basal area hold < 40 % of all removed basal area. T4: 20 % (robust margin). T2: 55 %, so "Mixed". A rule on the *single most-thinned cell* was rejected: T4's worst cell loses 37.8 % and T2's 42.8 %, both too close to any round threshold |
| **Mixed: some small openings** | Everything else. An honest middle. No pretence of a crisp category |
| Always | Largest connected opening in cells and m², and the count of cells touched |

Wording never evaluates. "One concentrated gap" is neither good nor bad. It explains *what tends to follow* in the game's own mechanisms: more ground light in one place, therefore more regeneration opportunity there and (under M2) more ground competition. Under storms it adds a new edge.

**Not shown:** Clark–Evans R, light coefficient of variation, "gap distribution quality", any target size.

## 4. How it appears without becoming a remote editor

- A Work Plan line inside "What you are leaving" (`ResidualStandReview.md`). Read-only.
- On the Stand Map **Marks** layer, already present: cells with ≥ 40 % basal area marked get an outline. No new map controls. Clicking still only sets waypoints.
- In the forest: nothing new. The player sees the marks.

## 5. Validation required in Unity (packet P3 test plan)

- Re-run T1–T5 at Year 0 and Year 10 under the current model stack. Labels must match this table at Year 0.
- Determinism: same marks → same text, two processes.
- Edge cases: 1 tree marked (always "Spread out"); all trees in one cell (concentrated); empty plan (no block).
