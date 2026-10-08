# Crop Tree release metric (Workstream C)

**Question the player asks:** *"How much did this plan release my selected Crop Trees?"*

**Status:** design proposal. Uses only the existing competition calculation. **No DBH-growth bonus is invented.**

## 1. Three layers, kept separate in wording and UI

| Layer | Definition | Where it comes from | Wording rule |
|---|---|---|---|
| **CURRENT AUTHORITATIVE METRIC** | Each Crop Tree's present Hegyi CI, and the share of its potential DBH growth withheld (`GetCurrentSuppression`) | `ForestEcologyController` [REPO] | Present tense: "is", "now" |
| **PREDICTED CONSEQUENCE** | Each Crop Tree's CI recomputed **without the Fell-marked trees**, the same formula and cutoff; growth withheld recomputed with `1/(1+CI/Ci50)` | The deterministic formula the marking forecast already uses (`ForestTreeMarkingManager :360-389`) | "If the marked trees are felled, and nothing else changes" |
| **FUTURE POSSIBILITY** | Realised diameter growth of the Crop Trees in later years, compared with the stand | Not saved per tree today; annual DBH growth is runtime only | Not shown in v1. A later Forest Diary option (§5) |

The predicted consequence is a **one-step, all-else-equal estimate**. Next year's actual growth also depends on site, size, crown changes, mortality, regeneration and (later) storms. The player must never read it as a promise.

## 2. Proposed summary (Work Plan, thinning card, and the residual block)

```
CROP TREES — RELEASE ESTIMATE (if nothing else changes)
  16 Crop Trees · 16 lose at least a tenth of their competition
  Competition around Crop Trees: 6.09 → 4.89 average (−20 %)
  Marked trees within 8 m of a Crop Tree: 30 of 30
```

| Line | Definition | Why this form |
|---|---|---|
| Crop Trees affected | Count of Crop Trees whose CI falls by ≥ 10 % | A per-tree threshold the player can check by inspection. The 10 % is a [C] presentation cut, stated in help |
| Competition change | Mean CI before → after over Crop Trees, and % change | Directly authoritative |
| Removals near Crop Trees | Marked trees within 8 m of at least one Crop Tree | Shows thinning that releases nothing ("clean-up" far from what you keep) |

**Rejected line: "meaningful competitors removed".** The prototype shows this count is **unstable and misleading** in a dense stand where competition is diffuse:

| Year-0 plan [PROTO, `Evidence/release-metric.csv`] | Removed | Crop CI change | Crop Trees ≥ 10 % released | "Meaningful" (≥ 10 % share) competitors removed | Removals > 8 m from every Crop Tree |
|---|---:|---:|---:|---:|---:|
| T0 none | 0 | 0 % | 0 | 0 | 0 |
| T1 novice clean-up (smallest crowded stems) | 84 | −16.6 % | 14 | 1 | 6 |
| T2 top-2 competitors per Crop Tree | 30 | −19.7 % | 16 | 10 | 0 |
| T3 top-6 per Crop Tree (heavy) | 87 | −44.5 % | 16 | 11 | 0 |
| T4 top-1 per Crop Tree (conservative) | 15 | −11.1 % | 10 | 6 | 0 |
| T5 concentrated gap, volume-matched to T2 | 48 | −16.0 % | 7 | **0** | 0 |

T5 releases seven Crop Trees by ≥ 10 % without removing a single "meaningful" neighbour. The count depends on an arbitrary share threshold; the CI change does not. The validated values match Unity's harness (mean crop CI before 6.0924, after 4.8916 for T2; offline 6.0925 / 4.892).

## 3. What the metric teaches (and its honest limits)

- T2 removes **30** trees and releases Crop Trees more than T1, which removes **84**. *Choosing which trees, not how many, drives release.* This is the central positive-selection lesson [HARNESS-Y0].
- T3 releases most (−44.5 %) but removes 13.1 m²/ha of basal area and opens a 12-cell area. Release has a cost in growing stock, light and exposure (`ResidualStandReview.md`).
- **Limits:** CI uses DBH and distance only. Crown size, height and light do not enter it. A tree's "release" in this metric is release from **diameter-growth competition as modelled**, not from shading. Help text must say so once.

## 4. Where it appears

1. **Tree Inspection** (P2), per Crop Tree: before/after CI and growth-withheld sentence.
2. **Marking forecast** (P2): Crop Tree figure separate from the stand average.
3. **Work Plan "What you are leaving"** (P3): the summary block above.
4. **Annual Review** (P4): *no new prediction*. It reports actual current CI for Crop Trees (authoritative now): "Competition around your Crop Trees: 4.9 (was 6.1 before the Year 1 thinning)". It is derivable only if the pre-thinning value is captured somewhere; see §5.

## 5. Save impact

- **v1: none.** Before/after is computed live from current state and current marks.
- "Was 6.1 before the thinning" in a later year needs history. **Option A (no save):** the snapshot already stores whole-stand mean DBH; it does not store Crop Tree CI. **Option B (save):** add `cropTreeMeanCi` and `cropTreeCount` to `ScenarioEcologicalSnapshot`. That is two fields per year, but a schema bump owned by the save single-writer, and it waits behind Model 2 (v18) and storms (v19). **Recommendation:** v1 without history; add B with the Forest Diary only if playtests show players want it.

## 6. Must not

- Show a "release score", a target percentage, or "well released / under-released" verdicts.
- Convert CI change into a promised cm/yr or €.
- Claim crown release, light release or stability change from this metric.
