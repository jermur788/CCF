# P3 — Work Plan "What you are leaving" (Workstream D)

**Status:** design proposal. Updates `ResidualStandMetricAudit.md` (pedagogy branch, `60674f1`) to `a8596df` and to the Model 2 and storm candidates.

The Work Plan thinning card today describes only **what leaves the forest**: products, volume, value, cost, minimum charge, net. A CCF decision is judged by **what stays**. P3 adds one compact, read-only block. It is a *review* of decisions already made while walking. It must not become a remote marking tool (D-010).

## 1. Metric audit

Classes: **AVAILABLE NOW** (derivable at `a8596df` from authoritative state, no save change) · **AFTER REGEN MODEL 2** (needs the `902903f` state) · **AFTER STORMS** (needs Storm Model v1) · **UNSUPPORTED** (no authoritative state; showing it would invent ecology, D-020).

| Candidate | Class | How it is derived | Show in v1? | Notes |
|---|---|---|---|---|
| Live stems kept | AVAILABLE NOW | living trees − Fell-marked | **Yes** | "306 of 336 trees stay" |
| Basal area | AVAILABLE NOW | Σ π(DBH/200)² / 0.16 ha | **Yes** | Before → after, m²/ha. Glossary term |
| Standing volume | AVAILABLE NOW | Σ `BiologicalStemVolumeM3` (forecast already shows "keep x m³") | **Yes** | Label "standing stem volume (model)" |
| Notional value of trees left standing | AVAILABLE NOW | `TimberYieldCalculator` over retained stems, as the pedagogy harness did | Optional | Must say "if sold today at roadside; not money you have". Risks reading as profit; playtest first |
| Crop Trees retained | AVAILABLE NOW but **trivial** | Crop and Fell marks are exclusive (D-013), so every Crop Tree is always retained | Replace | Show "16 Crop Trees designated" instead |
| Crop Trees released | AVAILABLE NOW | `CropTreeReleaseMetric.md` | **Yes** | Central line |
| Seed-source retention | AVAILABLE NOW | Seed-bearing trees (`Species.Maturity(age) > 0.05`) kept / removed, by species | **Yes, from Year 1** | Year 0: "No trees bear seed yet." Broadleaf seed trees: none until planted trees reach age 40 |
| Species mixture | AVAILABLE NOW | Retained adults by species | Only if > 1 species | Year 0–25: 100 % Sitka. A line saying so is honest but noise |
| Spatial opening pattern | AVAILABLE NOW | `SpatialPatternFeedback.md` | **Yes** (descriptor + largest opening) | |
| Light change | AVAILABLE NOW | Forecast cell light without marked crowns (same as `RecomputeCanopy`, already in the forecast line) | **Yes** | Count of cells crossing a light band, plus the mean. The mean alone hides gaps. The maximum alone is misleading: the road-edge cell is already 0.84 at Year 0 |
| Canopy kept | AVAILABLE NOW | mean (1 − light) forecast | Yes | Ties to the canopy objective |
| Stand density relative to self-thinning | AVAILABLE NOW (Growth Model 1) | `SitkaGrowthModel.RelativeDensity` before/after; self-thinning onset RD 0.6 [C] | Later (diary) | Real, but technical. [B]/[C] parameters |
| Regeneration destroyed by **felling** | **UNSUPPORTED** | No harvest/extraction damage model | No | One honest sentence in help: "Felling here does not damage young trees; real harvesting can." |
| Regeneration destroyed by **clearance** | AVAILABLE NOW | `QueryClearance` targets (the preview already counts them) | **Yes**, in the clearance card | "Clearance in E4 also removes 2 young-tree groups and 1 planted sapling" |
| Regeneration that clearance would **benefit** | AFTER REGEN MODEL 2 | Model 2 bramble/bracken cover × juvenile height vulnerability | After M2 | Show competitor level LOW/MOD/HIGH beside young trees present |
| Deadwood retained | AVAILABLE NOW | "Leave as deadwood" volume in the plan + existing records | **Yes** | "+0.4 m³ fallen deadwood (2 stems). On site after: 1.1 m³" |
| Understorey competitor state | AFTER REGEN MODEL 2 | Cell `brambleCover` / `brackenCover` | After M2 | Current understorey groups are provisional and not causal. Do not show them as competitors |
| Wind exposure | AFTER STORMS | Vulnerability index V → Stable / Watch / Exposed [STORM-DESIGN] | After storms | "6 trees move to Exposed for a few years." The current "wind peak" is saturated; **remove it** from the forecast when storms land |
| H/D (slenderness) of Crop Trees | AVAILABLE NOW (descriptive) / interpretive AFTER STORMS | height/DBH | No (v1) | Without storms it explains nothing the game does |
| Habitat / biodiversity score | UNSUPPORTED as a score | `deadwoodHabitatValue` exists but uses [D] provisional weights | No | Never aggregate |
| Residual stem damage, soil/rack damage | UNSUPPORTED | none | No | Stage 2+ |
| Pruned clear-stem retained | AVAILABLE NOW but trivial | Pruned trees are Crop Trees, so always retained | No | — |
| Future timber value / yield | UNSUPPORTED as a prediction | No accepted forecast model | No | Reference Future is the only comparison and is not a forecast (D-018) |

## 2. The block (v1, AVAILABLE NOW only)

Real Year-0 figures for plan T2 (top-2 competitors of 16 Crop Trees, 30 trees) [HARNESS-Y0]:

```
WHAT YOU ARE LEAVING (estimate, before any growth)
  Trees            336 → 306 standing (−30)
  Basal area       41.1 → 36.2 m²/ha (−12 %)
  Stem volume      39.9 → 34.7 m³
  Crop Trees       16 designated · competition around them −20 %
                   (16 of 16 lose at least a tenth)
  Openings         Small openings in several places · largest about 2 cells
  Light            Cells with ground light ≥ 0.20: 4 → 5 · stand mean 0.05 → 0.06
  Seed trees       None bear seed yet (Sitka start at about age 20)
  Deadwood left    none in this plan · 0.0 m³ on site
```

Rules:

- Every line is **before → after**, never a verdict. No colour-coded good/bad.
- Exactly one sentence of interpretation is allowed, and only about **mechanism**: "Removing larger trees close to Crop Trees releases them most."
- Values update live as the player edits marks **in the forest**. The Work Plan block has no controls.
- Under 9 lines, so it fits at 1280 × 720 above the job list (readability audit).

## 3. Later lines (gated)

| Gate | Line |
|---|---|
| Regen Model 2 integrated | "Ground competitors: bramble/bracken HIGH in 3 opened cells. Young trees there may struggle unless vegetation is controlled." Mechanism only; never "clear here" |
| Storms v1 integrated | "Stability: 6 trees move to Exposed for a few years (mostly C3, D3)." No probability |
| Forest Diary | "Compared with your Year 1 thinning: …" (history) |

## 4. Implementation notes

- New read-only helper `UI/ResidualStandSummary.cs` (pure functions over trees, marks, ecology; no state). Shared with P2's release metric.
- Performance: O(N) for stems, BA, volume and seed; O(N·k) for Crop Tree CI (k ≈ 40 neighbours); O(N·cells) for affected-cell light. N = 336–1,000. Recompute **only when the mark set changes** (the existing marking cache already tracks this).
- Save impact: **none**.
- Determinism: pure functions of saved state. Text must be identical across processes (test).
- Files: `WorkPlanView.cs` (one section), new helper, test harness. Locks: `ScenarioOneManager`, ecology and save files are **read-only**.
