# P2 — Crop Tree competitor reasoning

```text
TASK PACKET
GOAL: When inspecting a Crop Tree, the player sees which neighbours compete with it,
      how much, and how their own Fell marks change that, and the marking forecast
      reports Crop Trees separately in a readable panel.
BRANCH: task/scenario-one-p2-crop-tree-competitors
```

**Start authority:** P1-INT integrated; decisions R2, R3 (`DecisionMatrix.md`). Design: `CropTreeCompetitorDesign.md`, `CropTreeReleaseMetric.md`.

**Scope**

1. New read-only helper `UI/CropTreeNeighbours.cs`: for a tree, list living neighbours ≤ `HegyiCutoffMeters`, each with distance, DBH, `HegyiTerm` and share of the total; total CI; CI recomputed without Fell-marked trees; growth-withheld before/after (`1/(1+CI/Ci50)`). Pure functions, deterministic ordering (term descending, then id).
2. `TreeInspectionView`: for a Crop Tree, the "Neighbours competing with this Crop Tree" section: top 5 + "n other trees together x %"; mark column; a "Marked to fell: n neighbours, x %" line; the release estimate sentence. For non-Crop Trees, a one-line "Competition comes from n trees within 8 m; largest share x %".
3. In-world numbered tags (1–5) on the listed neighbours **while inspection of a Crop Tree is open** (LineRenderer pattern from `ClearancePreview`; neutral colour; removed on close, modal, reference preview). **Behind a single bool, default ON, so the manual review can switch it off without a code change elsewhere.**
4. `ForestTreeMarkingManager.UpdateTreatmentOutcome`: add a Crop Tree mean CI before → after line; keep the stand-average line. `WalkingHudView`: put the forecast in a `panel` at body size (ReadabilityAudit V1).
5. Copy rules (design §3); deny-list test.

**Owned files:** `UI/TreeInspectionView.cs`, `UI/WalkingHudView.cs` (forecast panel only), `ForestTreeMarkingManager.cs` (forecast string/numbers only), new `UI/CropTreeNeighbours.cs` (+ `.meta`), new `Tools/Verification/CropTreeCompetitorVerification.cs`.

**Locked:** everything else; in particular `ScenarioOneManager`, ecology, save, objectives, `ForestPlayer`.

**Exclusions:** wind/stability labels; competition label thresholds; crown or height as competition; any "cut this" wording; P3 block.

**Save impact:** none.

**Test plan**

- Batch: for fixture Crop Tree **P0707** at Year 0, the helper's top five equal P0706, P0607, P0807, P0806, P0708 with shares 7.9 / 7.2 / 4.8 / 4.3 / 4.2 % (±0.1) and 48 neighbours (`Evidence/crop-neighbour-ranking.csv`); total CI equals `GetCompetitionIndex` within 1e-4.
- Batch: T2 marks → Crop Tree mean CI 6.0924 → 4.8916 (±0.001); T5 → 5.1171.
- Batch: deny-list over all new strings (`should`, `cut this`, `remove this`, `best`, `correct`, `worst`).
- Interactive rendered: inspection with list at 1280 × 720 / 1600 × 900 / 1920 × 1080; no overlap with the HUD; tags visible and gone after close; forecast panel readable.
- Anchors unchanged (completion, lifecycle, Reference).
- Performance: inspection refresh ≤ 2 ms at 336 trees (log).

**Manual review:** a human checks whether the tags read as instructions. If yes: switch them off and record the decision.

**Stop conditions:** CI mismatch with production; any anchor drift; any need to touch the manager/ecology.

**Handoff:** IMPLEMENTATION HANDOFF, with screenshots of P0707 inspection and the forecast panel.
