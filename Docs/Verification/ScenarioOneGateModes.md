# Scenario One verification gate modes (P0 contract)

**Branch:** `task/scenario-one-pedagogy-p1` · **Base:** `origin/main` @ `3e4ee40`. Diagnosis: `Docs/Research/ScenarioOnePedagogy/BaselineRedGateInvestigation.md`.
**Status:** proposed verification contract, for Manager/integration review. The canonical Overview is not edited here.

## Modes

| Mode | Meaning |
|---|---|
| **BATCH-SAFE** | Valid in `-batchmode -nographics`. A failure is a real signal |
| **INTERACTIVE-REQUIRED** | Needs a visible Editor without `-batchmode`: cursor lock, keyboard input routed to play mode, rendered UI. In batch mode the harness **fails fast** with "requires an interactive (non -batchmode) Editor…". That message is an environment error, never a product regression |
| **HEADLESS-DIAGNOSTIC** | Produces evidence, not pass/fail acceptance. Run in batch |
| **OBSOLETE** | Tests a retired contract. **None at present.** Removal's retired species-specific U assertion was replaced by the current contract (see below), not deleted |

## Why some gates cannot run in batch

Measured on `3e4ee40`:

- In `-batchmode`, `Cursor.lockState` stays `None` after the harness sets `Locked`. `ForestPlayer.UpdateTreeInspection` deliberately ignores aim and closes inspection when unlocked.
- The project has no Input System settings asset. By default keyboard events reach play mode only through a focused Game View, and batch mode has none.

## Commands

The disposable-harness pattern is the same for every gate:

```bash
cp Tools/Verification/<Harness>.cs Assets/ForestPrototype/
XDG_CONFIG_HOME=<isolated dir containing unity3d/Unity -> your licence folder> \
CCF_ACCEPTANCE_OUTPUT=<evidence dir> \
<Unity 6000.6.0f1>/Editor/Unity [-batchmode -nographics] -projectPath <worktree> \
  -executeMethod <Harness>.Begin -logFile <log>
rm Assets/ForestPrototype/<Harness>.cs Assets/ForestPrototype/<Harness>.cs.meta
```

- **BATCH-SAFE / HEADLESS-DIAGNOSTIC:** include `-batchmode -nographics`.
- **INTERACTIVE-REQUIRED:** omit both flags and set `DISPLAY`. The Editor window opens, runs and exits by itself.

Always use an isolated profile, so the harness never touches the player's save or PlayerPrefs. After interactive runs, revert the Editor's material and settings normalisation (`git checkout -- .vscode/settings.json Assets/ForestPrototype/Art/SectionFive`), then check `git status`.

**Launcher note:** `Tools/Verification/run_clearance_gate.py` supports `--interactive`. However, its built-in anchor table predates D-047: it expects completion `00479F18970F9926` / `568922E1A6D73CDD`, while the current new-game anchor is `6F84AF319D301F87`. Until that table is updated (follow-up), judge completion by the token and the anchors in the Overview, not by the launcher's anchor check.

## Scenario One gates

| Gate (`Tools/Verification/…` unless stated) | Mode | Why | Expected pass token | What a failure means | What a failure does NOT mean |
|---|---|---|---|---|---|
| **ScenarioOneRemovalVerification** | BATCH-SAFE | API and reflection only; U-clearance step supplies the authoritative preview directly | `SCENARIO_ONE_REMOVAL_VERIFY_PASS` | Removal/clearance ordering, settlement, history or save/load regression; or U no longer plans a speciesless area clearance | — (it no longer tests the retired species-specific U behaviour) |
| **ClearanceVerification** | **INTERACTIVE-REQUIRED** | Walking ground ray (cursor lock); optional rendered captures with `CCF_CLEARANCE_CAPTURE=1` | `CLEARANCE_ACCEPTANCE_PASS` (plus `CLEARANCE_WALKING_PREVIEW_PASS`, `CLEARANCE_FIXTURES_PASS A … J`, `CLEARANCE_CIRCLE_PASS`, `CLEARANCE_REPEAT_MATRIX_PASS`, `CLEARANCE_ANNUAL_PASS`) | Clearance footprint, preview, targets, settlement, save/load or display regression | In batch mode: the fail-fast message is not a product regression. A first-use help screen is dismissed by the harness, as a player would |
| **MenuTutorialVerification** | **INTERACTIVE-REQUIRED** | Queued keyboard input, cursor lock, rendered layout at 1280/1600/1920 | `MENU_TUTORIAL_PLAYTHROUGH_PASS` | Help, menus, map/waypoint, annual-review gate, lessons or preference regression; or text clipping | In batch mode: fail-fast environment message only |
| **ScenarioOneTeachingCopyVerification** (new, P1) | BATCH-SAFE for content; INTERACTIVE for layout | Content and contract checks need no input; HUD forecast layout needs rendering | `TEACHING_COPY_VERIFY_PASS` (batch also logs `TEACHING_COPY_RENDERED_SKIPPED`; interactive logs `TEACHING_COPY_RENDERED_PASS` ×3) | Required teaching statement missing; clearance copy implying an unmodelled benefit; prescriptive phrasing; help not reopenable; objective, economy, ecology or save constants changed; forecast clipped | Not a comprehension test; human playtesting is still required |
| **ScenarioOnePresentationReview** | INTERACTIVE-REQUIRED for UI captures (rendered) | `ScreenCapture` of UI only outside batch mode | `SCENARIO_ONE_PRESENTATION_CAPTURE_PASS` | Capture/presentation regression | — |
| **ScenarioOneCompletionVerification** | BATCH-SAFE | Simulation API | `SCENARIO_ONE_COMPLETION_VERIFY_PASS` + `SCENARIO_ONE_COMPLETION_HASH 6F84AF319D301F87` (new game) | Completion path, economy or determinism regression | — |
| `Assets/ForestPrototype/ScenarioOneInteractionVerification` | BATCH-SAFE | Simulation and API (checked into Assets) | `SCENARIO_ONE_INTERACTION_VERIFY_PASS` | Interaction, lifecycle anchor or presentation-state regression | — |
| ScenarioReferenceVerification | BATCH-SAFE | Archive and replay | `REFERENCE_REPLAY_PASS`, `REFERENCE_PREVIEW_PASS` | Reference Future v1 contract regression | — |
| ScenarioOnePlantingVerification | BATCH-SAFE | API | `SCENARIO_ONE_PLANTING_VERIFY_PASS` | Planting regression | — |
| ScenarioOnePruningVerification | BATCH-SAFE | API | `SCENARIO_ONE_PRUNING_VERIFY_PASS` | Pruning regression | — |
| ScenarioOneDeadwoodVerification | BATCH-SAFE | API | `SCENARIO_ONE_DEADWOOD_VERIFY_PASS` | Deadwood regression | — |
| ScenarioOneProgressVerification | BATCH-SAFE | API | `SCENARIO_ONE_PROGRESS_VERIFY_PASS` | Objective/progress regression | — |
| ScenarioOneEconomyIntegrationVerification | BATCH-SAFE | API | `SCENARIO_ONE_ECONOMY_INTEGRATION_VERIFY_PASS` | Economy regression | — |
| BrowsingProtectionVerification | BATCH-SAFE | API | `BROWSING_PROTECTION_VERIFY_PASS` | Browsing/shelter regression | — |
| SaveHardeningVerification | BATCH-SAFE | API | `SAVE_HARDENING_VERIFY_PASS` | Save regression | — |
| RngModelVerification / RngModelPolicyVerification | BATCH-SAFE | API | `RNG_MODEL_VERIFY_PASS` / `RNG_MODEL_POLICY_VERIFY_PASS` | RNG regression | — |
| RegenerationBudget/RegenerationModelVerification | BATCH-SAFE | API | `REGENERATION_MODEL_VERIFY_PASS` | Regeneration model regression | — |
| Stage1WorkEconomyFoundationVerification, TimberAssortmentYieldVerification, BatchRecomputeVerification, EcologyCalibrationAdoptionVerification, JuvenileMortalityFoundationVerification, CCF* species/planting gates, CCFIntegrationVerificationTemp | BATCH-SAFE | API | their `*_VERIFY_PASS` / `*_PASS` tokens | Regression in that subsystem | — |
| `Assets/ForestPrototype/ScenarioHabitatPresentationVerification`, `RecentAssetReviewVerification` | BATCH-SAFE (state checks) | API and visual state | `SCENARIO_HABITAT_PRESENTATION_VERIFY_PASS`, `RECENT_ASSET_REVIEW_PLAY_PASS` | Presentation-state regression | — |
| EdgeBiasDiagnostic, ScenarioOneEcologyViabilityDiagnostic, RegenerationBudget/RegenerationBudgetEvidence | HEADLESS-DIAGNOSTIC | Evidence output | `EDGE_BIAS_DONE`, `SCENARIO_ONE_VIABILITY_DIAGNOSTIC_DONE`, `REGEN_BUDGET_EVIDENCE_PASS` | Diagnostic could not run | Not an acceptance failure by itself |
| `ScenarioOnePedagogy/ResidualStandEvaluation` (research branch, not ported) | HEADLESS-DIAGNOSTIC | Read-only marking-plan evidence | `RESIDUAL_STAND_EVALUATION_PASS` | Evidence harness broken | Not an acceptance gate; its numbers must be regenerated after the growth model lands |

## Verified in this packet

The actual runs and results are recorded in `Docs/Research/ScenarioOnePedagogy/P1ImplementationRecord.md`.
