```text
TASK PACKET
PROJECT: CCF
MANAGER: ChatGPT Game Dev (Overall Manager, D-026)
BASE: origin/main after P1 is integrated
CONTEXT COMMIT: latest full context SHA at issue time
WORKTREE: dedicated (e.g. /home/jer/CCF-positive-selection)
BRANCH: task/scenario-one-positive-selection-feedback
ROLE: Primary
GOAL: While marking, the player can see how strongly each neighbour competes
      with their Crop Trees, and the forecast reports the effect on Crop Trees
      separately from the stand average.
AUTHORITATIVE SOURCES:
- Decision Log D-013/D-014/D-020; Overview (competition, light formulas)
- Docs/Research/ScenarioOnePedagogy/PositiveSelectionTeaching.md
- Docs/Research/ScenarioOnePedagogy/ResidualStandMetricAudit.md §2
- Docs/Research/ScenarioOnePedagogy/ResidualStandEvaluationPrototype.md
IN SCOPE:
- New read-only helper UI/ResidualStandSummary.cs: per-Crop-Tree competition
  before/after for a set of marked trees using ForestEcologyController.HegyiTerm
  and HegyiCutoffMeters; predicted growth with the production expression; no
  new state; no caching that survives frames.
- Tree Inspection: when the inspected tree is within 8 m of >=1 Crop Tree, a
  line per Crop Tree (max 3): "Competes with your Crop Tree P0707: strong
  (rank 1 of 48, 8 % of its competition)". Bands strong/moderate/slight are
  presentation bands (labelled in code as [D]).
- ForestTreeMarkingManager.UpdateTreatmentOutcome: split into
  "Your Crop Trees: competition -x %, growth +y % (n of m released >=10 %)"
  and "Whole stand: growth +z %"; keep light/opening lines; REMOVE the wind
  band from the forecast until decision #26 (it is "high" for every tree).
OUT OF SCOPE:
- World highlighting of competitors (decision #14), Work Plan block (P3),
  any change to competition, growth, light or wind formulas, save, economy.
OWNED FILES / SUBSYSTEMS:
- Assets/ForestPrototype/UI/ResidualStandSummary.cs (+ .meta) NEW
- Assets/ForestPrototype/UI/TreeInspectionView.cs
- Assets/ForestPrototype/UI/WalkingHudView.cs (forecast display)
- Assets/ForestPrototype/ForestTreeMarkingManager.cs (UpdateTreatmentOutcome)
- Tools/Verification/ScenarioOnePedagogy/PositiveSelectionVerification.cs NEW
SHARED / LOCKED FILES:
- ForestEcologyController.cs read-only (Sol may change it; use only public
  HegyiTerm/HegyiCutoffMeters/GetCompetitionIndex).
COMPATIBILITY REQUIREMENTS:
- The annual step's inline Hegyi copy must not be modified (bit-exact anchor
  comment in UpdateCompetition).
MUST-NOT-CHANGE HASHES / ANCHORS:
- All lifecycle/completion/Reference anchors.
INTENTIONALLY CHANGED HASHES / ANCHORS:
None.
VERIFICATION:
- Batch harness: Year-0 fixture P0707 — relationship rank/share equals a
  recomputation; removing P0710 changes its competition <=2 %, removing P0706
  >=7 % (re-baseline numbers after Sol's integration if merged first).
- Batch harness: T1 vs T2 fixture marks show Crop-Tree vs stand inversion.
- Determinism: forecast text identical across two processes.
- ScenarioOneCompletionVerification + ScenarioOneInteractionVerification
  batch PASS, anchors unchanged.
- MenuTutorialVerification INTERACTIVE PASS; rendered capture of inspection
  with the relationship line at three resolutions.
STOP AND ASK IF:
Showing the relationship requires per-frame cost above ~1 ms at 336 trees
(then cache per inspection), or any wording reads as a recommendation.
```
