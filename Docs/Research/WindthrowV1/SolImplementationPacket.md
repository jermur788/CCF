# Sol implementation packet — Storms v1 core (W1 + W2)

```text
TASK PACKET
PROJECT: CCF
MANAGER: ChatGPT Game Dev (Overall Manager, D-026)
WORKER: Sol (ecology owner)
ROLE: Primary

STARTING AUTHORITY
- Start from current origin/main after Regeneration Model 2 integration.
  Do NOT use a8596df or any SHA in this document as the base.
- At task start: fetch origin/main; record the exact main SHA and the
  context SHA (latest commit touching Docs/Project, AGENTS.md, CLAUDE.md,
  Tools/ProjectContext); confirm Regeneration Model 2 (save v18 or later)
  is in main; read Docs/Project/coordination/active-tasks.md live; confirm
  the decisions listed under PRECONDITIONS are recorded.
- Supporting design: Docs/Research/WindthrowV1/ on task/windthrow-readiness
  (research, not authority). Accepted decisions override it.

WORKTREE: dedicated (e.g. /home/jer/CCF-storms)
BRANCH: task/storms-windthrow-v1

GOAL
Damaging storms occur as deterministic, versioned scenario events. Tall,
slender, dominant and recently exposed trees are most likely to be
windthrown. Victims become windthrown deadwood, open the canopy and raise
local exposure; the player can see them in the forest.

PRECONDITIONS (decisions; stop if absent)
- D-044 storms/windthrow deferral lifted for Scenario One (DecisionMatrix #1)
- Storms on for new games vs opt-in (#4)
- Scenario One site wind hazard / soil label (#5)
- Property surroundings per side, or the "all sheltered" fallback (#6)

IN SCOPE (W1 core + W2 calibration)
1. StormModel versioning (0 none / 1 v1), saved as ForestSaveData.stormModel
   in the next free save version; missing/0 = no storms; new games per #4;
   Reference Future v1 = 0.
2. Definition parameters on ScenarioOneDefinition (probability, severity
   weights and intensities, prevailing direction and spread, site hazard,
   side surroundings, grace years); all labelled [C]/scenario.
3. Storm step at the start of ForestEcologyController.AdvanceOneYear, after
   ecologicalYear++ and before competition (StormEventArchitecture.md §4).
4. Layer 1 rolls and Layer 2 vulnerability as pure, unit-testable functions
   (TreeVulnerabilityDesign.md §2): load, dominance (local top height),
   H/D, openness, recent opening (3×3 cells, RecentThinningAssessment.md),
   site, edge.
5. Two-pass victim selection with SimulationRandom.Roll ids
   STORM-OCCURS-v1, STORM-SEVERITY-v1, STORM-DIRECTION-v1,
   WINDTHROW-v1-<treeId>.
6. Batched outcome: ApplyMortality("windthrow", year) inside
   BeginChangeBatch/EndChangeBatch; +1 RecentOpening per victim cell (cap);
   existing death handler creates deadwood records (keep it cause-agnostic;
   confirm it fires under the growth model used by storm games).
7. scenarioOne.stormEvents[] {year, severityClass, directionDegrees,
   victims, volumeM3}; validation; restore.
8. Harness-only ForceStorm(year, class, direction) for tests.
9. A minimal diagnostic: annual log line STORM year class dir victims volume;
   GetWindRisk left as is OR redirected to V — record the choice (the UI
   relabel itself is W5, not this packet).
10. W2 calibration matrix (LongRunCalibrationPlan.md) and
    Docs/Research/WindthrowV1/CalibrationResults.md.

OUT OF SCOPE
- Visual root plates/crowns/pit (W3), salvage (W4), UI/labels (W5),
  tutorial (W6), snapping, juvenile damage, microsites, pests, contractor
  quality, competition/light/seed boundary changes, objective changes,
  economy values, Reference Future archive.

OWNED FILES / SUBSYSTEMS
- Assets/ForestPrototype/ForestEcologyController.cs (storm step only)
- new Assets/ForestPrototype/StormModel.cs, StormVulnerability.cs (+ .meta)
- ForestSaveData.cs, ForestSaveController.cs, ForestSaveValidation.cs
  (stormModel, stormEvents)
- ScenarioOne/ScenarioOneDefinition.cs and ScenarioOne/ScenarioOne.asset
  (SERIALIZED; single writer; minimal reviewed diff)
- ScenarioOne/ScenarioOneManager.cs (storm events list, save capture/restore,
  death-handler cause awareness only)
- Tools/Verification/WindthrowV1/* (new harnesses)

SHARED / LOCKED FILES
- UI/*, ForestTreeMarkingManager.cs: pedagogy/UI owner; do not edit.
- Art/prefabs/scenes: do not edit (no scene save).

SAVE GATE
- Next free schema version after Regeneration Model 2. Absent stormModel =
  0. Old saves load unchanged. stormEvents validated. No other field.
- JSON layout hash change handled as in D-047/D-048 (legacy-layout helper
  or explicitly re-recorded layout anchors).

ECOLOGY INTERACTIONS
- Storm before competition; victims leave competition, canopy and seed via
  existing mortality hooks; RecentOpening shared with establishment
  suitability and understorey; Regeneration Model 2 reacts through light
  and opening only (no storm-specific coupling).
- Growth Model 1 density mortality runs after the storm in the same year.

CALIBRATION MATRIX
- LongRunCalibrationPlan.md §1–3: 4 regimes × forced storms (M/S/X at
  Years 1/5/15/30/50) × 3 seeds; stochastic p 0.04/0.08/0.12 × 10 seeds ×
  100 years. Calibration targets [C] vs sanity checks clearly separated.

ASSET REQUIREMENTS (for later packets; none in W1/W2)
- Uses the existing deadwood log visual unchanged in W1/W2. Directed
  placement and root plates are W3.

PERFORMANCE REQUIREMENTS
- One canopy and one seed rebuild per storm; no per-victim scene queries;
  the storm step adds ≤ 5 ms on reference hardware in batch for 360 trees
  [C]; record growth_model + storm long-run timings.

TESTS (Tools/Verification/WindthrowV1, batch-safe unless stated)
- stormModel 0: every current anchor reproduces exactly (lifecycle,
  completion, regeneration, growth, Reference Future v1).
- Determinism: same save/year/model/seed → same storm and victims; two
  processes; save at N−1 → load → identical storm at N; shuffled iteration
  → identical victims.
- Ordering fixtures: heavy recent > light recent > unthinned; new gap edge >
  old gap edge; dominant > suppressed; Year-0 stand ≤ 1 % under a severe
  storm; released crop trees < slender dominants (same height).
- Outcome: victims dead with cause windthrow; one deadwood record each;
  canopy rebuilt once; RecentOpening incremented and capped; stormEvents
  saved and restored.
- Completion viability statistics under default storms (≥ 90 % target).
- Calibration result tables and run hashes committed.

LEGACY / REFERENCE REQUIREMENTS
- Reference Future v1 archive untouched, previews and continuation stay
  storm model 0 and reproduce their anchors.
- Saves v1–v18 load as storm model 0.

STOP CONDITIONS
- Any stormModel-0 anchor drifts.
- The Irish stand-level equation would have to be used as an annual
  per-tree probability to meet a target.
- Calibration cannot reach the targets without changing growth, mortality,
  regeneration or objectives.
- Save change beyond stormModel/stormEvents/enums appears necessary.
- File ownership conflict with an active writer (UI/pedagogy, Sol's
  understorey follow-ups).
- Reference Future v1 would change.

FINAL HANDOFF TEMPLATE
STORMS v1 CORE HANDOFF
BASE MAIN / CONTEXT / BRANCH / HEAD:
REGENERATION MODEL 2 PRESENT: yes/no (version)
SAVE VERSION: from → to; fields added:
STORM MODEL DEFAULTS (new games / legacy / Reference):
LAYER 1 PARAMETERS (all [C]):
LAYER 2 FORM + WEIGHTS:
CALIBRATION RESULTS: targets met? (table) / sanity checks:
DETERMINISM: in-process / two-process / save-load / shuffled:
ANCHORS: storm 0 unchanged (list) / new storm-1 anchors (list):
PERFORMANCE: storm step ms; worst case victims; rebuild count:
COMPLETION VIABILITY: % runs completed by Y25 under default storms:
OPEN QUESTIONS / DECISIONS:
FILES:
WORKTREE CLEAN:
STATUS: READY FOR MANAGER REVIEW / TARGETED FIX REQUIRED / BLOCKED
```
