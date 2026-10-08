# Scenario One — player-facing completion readiness

**Branch:** `task/scenario-one-completion-readiness` · **Worktree:** `/home/jer/CCF-completion-readiness` · **Base / context commit:** `origin/main` @ `a8596df9c52669a36709f09e85dfe6568640af49` (fetched 2026-10-08).

**Status:** research, audit and implementation-packet authoring for Manager review. **Not decision authority.** No production file (`Assets/ForestPrototype/**`), save schema, scene, prefab or canonical decision was modified. **Unity and Blender were not run.** Evidence: static repository inspection at `a8596df`, read-only reading of the branches below, the economics report PDF, and two standard-library Python scripts (`Tools/Verification/ScenarioOneCompletion/`).

## Branch classification (at task start)

| Branch @ SHA | Class | Notes |
|---|---|---|
| `main` @ `a8596df` | **INTEGRATED** | Growth Model 1 (D-048), Regeneration Model 1 (D-047), RNG policy (D-046), UI Toolkit redesign, menu teaching, waypoint HUD, clearance preview |
| `integration/regeneration-model2` @ `a8596df` | Empty integration branch | Identical to main; Model 2 not yet merged into it |
| `task/scenario-one-pedagogy-p1` @ `1a36ea9` | **CANDIDATE** (implemented, not integrated) | P0 gate modes + P1 copy; base `3e4ee40` (7 behind main); merges cleanly into `a8596df` (`git merge-tree`); copy gate expects save v16 |
| `task/understorey-recruitment-causality` @ `902903f` | **CANDIDATE** (Regeneration Model 2, save v18) | Handoff status **"CALIBRATION READY — PARAMETER DECISION REQUIRED"**: schema approved, coefficients/target rule not. *The task packet calls it "accepted"; the branch does not support that.* Owner: Sol |
| `task/windthrow-readiness` @ `9a9f4fb` | **SUPPORTING DESIGN ONLY** | Storms v1 design; nothing implemented; D-044 deferral not lifted |
| `task/scenario-one-pedagogy-overnight` @ `60674f1` | **SUPPORTING DESIGN ONLY** | Pedagogy research; its harness fixes were ported into P1. Year-0 harness evidence is reused here; later-year numbers are not (pre-Growth-Model-1) |
| `task/post-scenario1-systems-readiness` @ `7f58618` | SUPPORTING DESIGN ONLY | Earlier readiness study; RNG finding resolved by D-046 |

## Start here

1. `CurrentScenarioFlow.md` → `PlayerStallPoints.md` — what the build does and where players get stuck.
2. `DecisionMatrix.md` — what is accepted, recommended, or needs a product decision.
3. `ImplementationRoadmap.md` → `Packets/` — the recommended order and bounded packets.
4. `ScenarioOneDefinitionOfDone.md` — what "done" means at three levels.

## Contents by workstream

| WS | File(s) |
|---|---|
| A | `CurrentScenarioFlow.md`, `ObjectiveDependencyGraph.md`, `PlayerStallPoints.md` |
| B, C | `CropTreeCompetitorDesign.md`, `CropTreeReleaseMetric.md` |
| D, E, F | `ResidualStandReview.md`, `MultidimensionalFeedback.md`, `SpatialPatternFeedback.md` |
| G, H, I | `AnnualReviewV2.md`, `ForestDiaryV1.md`, `MapHistoryDesign.md` |
| J, K | `RegenerationModel2Teaching.md`, `StormTeaching.md` |
| L, M, N, O | `ObjectiveRedesign.md`, `SecondInterventionDesign.md`, `ScenarioCompletionDesign.md`, `FailureRecoveryDesign.md` |
| P | `EconomyTeachingAudit.md`, `EconomyLearningSequence.md` |
| Q, R | `MarteloscopeFinalDesign.md`, `TrainingStandV2.md` |
| S, T | `BeginnerPlaytestProtocol.md`, `ForesterReviewProtocol.md` |
| U, V, W | `TerminologyAudit.md`, `ReadabilityAudit.md`, `PlayerFacingAssetGaps.md` |
| X, Y | `ScenarioOneDefinitionOfDone.md`, `PostScenarioRoadmap.md` |
| Z | `DecisionMatrix.md`, `ImplementationRoadmap.md`, `Packets/` |
| Evidence | `Evidence/` (inputs with provenance, prototype outputs, `SHA256SUMS.txt`) |

## Headline findings

1. **Five of eight objectives need no management** under the current stack (deadwood now auto-completes through Growth Model 1 self-thinning [INF]). The other three count one act each.
2. **Hidden dead end:** with cash below the €2,500 harvest minimum before the first thinning, completion becomes impossible, and the game says so only at Year 100.
3. **Competition is diffuse** in this stand: a Crop Tree has a median of 39 neighbours within 8 m, and its largest neighbour supplies a median of 8.8 % of its competition. Competitor teaching must show distributions, not "the competitor".
4. **"Meaningful competitors removed" is a misleading metric.** The concentrated-gap plan releases 7 Crop Trees ≥ 10 % while removing zero "meaningful" neighbours. Use the change in competition.
5. **A spatial-pattern label is robust only at the extremes**; the textbook crop-tree release flips between labels. Show facts plus a "Mixed" middle state.
6. **Storms + fixed seed:** under the proposed defaults every new game would get a **severe storm in Year 3**. A grace period of 6 years moves the first storm to Year 13 (moderate).
7. **Marteloscope:** practising on a time-advanced copy of the *real* forest would be an oracle (deterministic, fixed seed). Recommend plan comparison now (no time advance) and an authored training mode later.
8. **Economy:** on 0.16 ha every thinning to about Year 25–40 loses money. That is defensible, but the game must say why. There is a sapling price inconsistency (€4.50/€5.50 vs €0.95/€1.00).

## Reproduce the offline evidence

```bash
python3 Tools/Verification/ScenarioOneCompletion/year0_release_and_pattern.py
```

```bash
python3 Tools/Verification/ScenarioOneCompletion/storm_timeline.py
```

Both are standard library only, read only files in `Evidence/`, and write their outputs there.
