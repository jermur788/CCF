# P3 — "What you are leaving", cash dead-end warning, terminology fixes

```text
TASK PACKET
GOAL: The Work Plan shows what stays in the forest (before → after) beside what is
      removed; the player is warned before spending into the cash dead end; raw
      IDs and jargon leave player text.
BRANCH: task/scenario-one-p3-residual-stand
```

**Start authority:** P2 integrated; decisions R4, R5, E1 (tone), U1, P1e. Design: `ResidualStandReview.md`, `SpatialPatternFeedback.md`, `EconomyLearningSequence.md`, `TerminologyAudit.md`.

**Scope**

1. New read-only helper `UI/ResidualStandSummary.cs`: stems, basal area, stem volume, Crop Tree release summary (via P2's helper), seed-bearing kept/removed by species, deadwood in the plan and on site, forecast light (cells ≥ 0.20, mean), spatial facts and robust label (rules in `SpatialPatternFeedback.md` §3). Recompute only when marks change.
2. `WorkPlanView`: "WHAT YOU ARE LEAVING" block above the thinning job (≤ 9 lines; no controls); minimum-charge explanation as a scenario rule with property scale.
3. Cash dead-end warning: when no thinning has completed and (approval or purchase) would leave cash < `MinimumHarvestJobCents`, show the warning text before the action (approval/purchase still allowed). When the state already exists, show the dead-end message in the Work Plan and HUD (`FailureRecoveryDesign.md` §3.2).
4. Terminology: display names for species in objectives (render time), review lines and the century review view; task display names; "cohort" → "young-tree group", "juveniles" → "young trees"/"planted saplings" in rendered text; "Marked … as harvest" → "Marked … to fell"; "Approve all pending jobs (n)".
5. Readability V2: no information-bearing text below 13 px base (`.uss` tokens `muted`, `faint`, `stat-label`).

**Owned files:** `UI/WorkPlanView.cs`, `UI/AnnualReviewView.cs` (names only), `UI/WalkingHudView.cs` (dead-end line only), `UI/Resources/ScenarioOneUi.uss` (font sizes only), `ScenarioEcologyReviewLines.cs` (display names only), `ForestTreeMarkingManager.cs` (message string), `ClearancePreview.cs`/`VegetationClearance.cs` (wording), new `UI/ResidualStandSummary.cs`, new harness.

**Locked:** `ScenarioOneManager.cs` (read only), `ScenarioOneObjectives.cs` (**no change to stored displayName**; map at render), ecology, save, economy values.

**Exclusions:** wind/storm lines; M2 lines; Annual Review restructure (P4); changing approval rules or economy values.

**Save impact:** none.

**Test plan**

- Batch fixtures (Year 0): T2 marks → 336 → 306 stems; basal area 41.1 → 36.2 m²/ha; label "Mixed"; T5 → "One concentrated gap", largest group 10 cells; T4 → "Spread out" (`Evidence/spatial-pattern.csv`).
- Batch: no field or string named score/grade/rating/sustainability %; deterministic text in two processes.
- Batch: S1 fixture: buy stock to €2,400 before any thinning → warning shown; dead-end message present; no failure triggered.
- String sweep: no raw species ids in rendered text.
- Interactive rendered at 1280 × 720 / 1366 × 768 / 1920 × 1080: the block fits without overlap.
- Anchors unchanged.

**Manual review:** read the block for T1, T2, T3, T5 marks. Does any line read as a verdict?

**Stop conditions:** any need to change approval/economy logic; anchor drift; the label rules failing the fixtures (report; do not tune silently).

**Handoff:** IMPLEMENTATION HANDOFF with fixture outputs and screenshots.
