# Scenario One integration merge rehearsal v1

Docs output branch based exactly on canonical gameplay `c1564b857972ee18917e8a461c457533a54cc3f1`, identified in Job1 (`26d2bf8dc75ca4f612ec8e10ab9303afa044919c`). Manager docs context321791b is separate. **Disposable rehearsal only, not a real integration candidate or qualification approval. Unity not opened.**

## Exact available merge order and method

A separate detached sparse worktree `/media/jer/Files/CCF-serial-sol-20261014/rehearsal-temp` started at c1564b8. Source C# was fully materialised for approximate compilation, with no Unity Library/import. For each source: `git merge --no-ff --no-commit <exact-source>`, capture index/conflicts, run offline/type-check, create a temporary local merge commit solely to allow the next cumulative trial. After evidence capture reset that detached worktree to c1564b8 and remove it. **All cumulative integrated production states were discarded and never pushed.** Only documentation/evidence is published. Existing user/worker checkouts untouched.

1. Density v2 `a7a759ded087a824c1e8149e480ddf58ac0585eb` (includes overnight timber/history prerequisite).
2. Dormant site `47539847d7e2531824c30f3dfe635d55404699a8` (branches at9d9d991, common ancestor; does not replay old density).
3. UX `84e881aa6d0e96c5752f09492d5db37d2cc8925e` (inherits b4894cc; preserves cumulative v2/site).

No separate b4894cc merge needed: it is inherited. Timber b9d4cd9 and calibration c35e001 are research, not implementation predecessors. Completion157ea7d adds only stop evidence; no N3 implementation is available. Future production-calibration and N3 merges were **not rehearsed**. Existing local UX fix549eae3 is not a supplied accepted source and was not substituted. Qualificationcc54d6a is evidence/tools, not a production feature merge.

## Text conflict list

**None in the three available cumulative merges** (merge exit0, unmerged-file list empty at every step). No conflict resolution edits were necessary. This disproves the need to mechanically port all shared baseline-file intersections, but does not remove semantic dependencies. Adding future growth/completion/fixes can still introduce conflicts.

## Semantic resolution guidance / owners

| File/subsystem | Semantic hazard | Correct owner and exact guidance |
|---|---|---|
| ForestEcologyController.cs | Earlier site lineage carries older density interfaces | Density owner retains a7a759d SelectRoute/PlanComposite, SitkaSubsetDensitySignal and unchanged pure-Sitka branch. Site must remain unwired; common ancestry auto-merged this correctly. |
| TreeSpeciesDefinition.cs / MixedDensityModel* / AdultMortalityAccount | Legacy ordinal density wording/interfaces versus v2 occupancy | Density owner keeps v2 occupancy fields, composite math and tooltip. Do not apply baseline-to-site patch over v2. |
| ForestSiteProfile / SpeciesSiteResponse / ScenarioSiteProfiles | Dormant types mistaken for fitted/site-enabled growth | Site owner adds exclusive three-type API and isolated tool only; no save or production multiplier. |
| ScenarioOneManager.cs / History partial / AnnualReviewView / ScenarioOneUiRoot | UX source review boolean and future chronological acknowledgement coexist | Completion owner later records event9 only on actual acknowledgement; UX owner keeps inline review/guard/help. Empty/failed/same-year work cannot count. New candidate required before deciding resolution. |
| ForestSaveData / SaveController / Validation / ScenarioManagementEvent | Event append versus terminal-state compatibility | Save/completion owner keeps v20 and existing ordinals, appends9, never fabricates chronology; preserve old Completed/Failed. No save production delta was merged here. |
| TimberYield / WorkEconomy / ScenarioOneEconomyAdapter | Research versus implemented prices/quality; model2 gating | Economy owner preserves Sitka0–3/prices and verifies broadleaf4–8/quality/accounting; review evidence b9d4cd9, no implied price adoption. |
| WorkPlanOverview / run_s1a_gates / P3 anchor | UX text changed recorded P3 presentation hash | Manager must explicitly authorise the new text hash or restore wording. This rehearsal keeps anchor expectations untouched, does not run Unity/P3 and does not decide silently. |
| Verification runners | Approximate stubs/old species tests mistaken for full qualified ecology | Verification owner retains v2 suites and existing baselines; rerun integrated Unity U3/U4 and actual source-specific UX. Legacy broadleaf tests alone do not qualify model2. |

## Verification actually run after every merge

All commands below ran in the detached cumulative worktree; generated .NET projects lived outside it. Logs/result metadata under RehearsalEvidence retain exact invocation, return code, timings and source identities. TYPECHECK_CLEAN means no new errors beyond7 production /43 tool baseline errors, **not Unity compilation**. Baselines were not updated.

| Step | Source | Gate | Result | Seconds | Evidence |
|---|---|---|---|---:|---|
| 1 | `a7a759ded087a824c1e8149e480ddf58ac0585eb` | diff-check | exit0 PASS (bounded non-Unity scope) | 0.03 | `RehearsalEvidence/1-diff-check.txt` |
| 1 | `a7a759ded087a824c1e8149e480ddf58ac0585eb` | offline-all | exit0 PASS (bounded non-Unity scope) | 33.93 | `RehearsalEvidence/1-offline-all.txt` |
| 1 | `a7a759ded087a824c1e8149e480ddf58ac0585eb` | typecheck-prod | exit0 PASS (bounded non-Unity scope) | 5.28 | `RehearsalEvidence/1-typecheck-prod.txt` |
| 1 | `a7a759ded087a824c1e8149e480ddf58ac0585eb` | typecheck-tools | exit0 PASS (bounded non-Unity scope) | 8.25 | `RehearsalEvidence/1-typecheck-tools.txt` |
| 2 | `47539847d7e2531824c30f3dfe635d55404699a8` | diff-check | exit0 PASS (bounded non-Unity scope) | 0.01 | `RehearsalEvidence/2-diff-check.txt` |
| 2 | `47539847d7e2531824c30f3dfe635d55404699a8` | offline-all | exit0 PASS (bounded non-Unity scope) | 26.9 | `RehearsalEvidence/2-offline-all.txt` |
| 2 | `47539847d7e2531824c30f3dfe635d55404699a8` | typecheck-prod | exit0 PASS (bounded non-Unity scope) | 5.38 | `RehearsalEvidence/2-typecheck-prod.txt` |
| 2 | `47539847d7e2531824c30f3dfe635d55404699a8` | typecheck-tools | exit0 PASS (bounded non-Unity scope) | 7.8 | `RehearsalEvidence/2-typecheck-tools.txt` |
| 2 | `47539847d7e2531824c30f3dfe635d55404699a8` | site | exit0 PASS (bounded non-Unity scope) | 4.29 | `RehearsalEvidence/2-site.txt` |
| 3 | `84e881aa6d0e96c5752f09492d5db37d2cc8925e` | diff-check | exit0 PASS (bounded non-Unity scope) | 0.02 | `RehearsalEvidence/3-diff-check.txt` |
| 3 | `84e881aa6d0e96c5752f09492d5db37d2cc8925e` | offline-all | exit0 PASS (bounded non-Unity scope) | 27.05 | `RehearsalEvidence/3-offline-all.txt` |
| 3 | `84e881aa6d0e96c5752f09492d5db37d2cc8925e` | typecheck-prod | exit0 PASS (bounded non-Unity scope) | 5.63 | `RehearsalEvidence/3-typecheck-prod.txt` |
| 3 | `84e881aa6d0e96c5752f09492d5db37d2cc8925e` | typecheck-tools | exit0 PASS (bounded non-Unity scope) | 7.16 | `RehearsalEvidence/3-typecheck-tools.txt` |
| 3 | `84e881aa6d0e96c5752f09492d5db37d2cc8925e` | site | exit0 PASS (bounded non-Unity scope) | 4.16 | `RehearsalEvidence/3-site.txt` |
| 3 | `84e881aa6d0e96c5752f09492d5db37d2cc8925e` | ux | exit0 PASS (bounded non-Unity scope) | 3.83 | `RehearsalEvidence/3-ux.txt` |

At every step: density91/91, composite1917/1917, generic+composite isolated compile, Sitka isolated compile, timber71/71, history70/70. Site added atstep2 and remained63/63 over1250 profiles atstep3; UX176/176 afterstep3. Diff checks passed. No runtime, render, real input, real save loader, Player or performance test was executed. Density mutation coverage remains a separate Level1 run, not a merge-conflict check.

## Per-step exact changed files

### Step 1: density

Predecessor `c1564b857972ee18917e8a461c457533a54cc3f1`; source `a7a759ded087a824c1e8149e480ddf58ac0585eb`; ephemeral cumulative commit `8a665fa3a8413816ac9f79c3c37562b5e388cb50` (discarded, not published). Merge diagnostic:

```text
Automatic merge went well; stopped before committing as requested
```

- `Assets/ForestPrototype/AdultMortalityAccount.cs`
- `Assets/ForestPrototype/ForestEcologyController.cs`
- `Assets/ForestPrototype/MixedDensityModel.Composite.cs`
- `Assets/ForestPrototype/MixedDensityModel.Composite.cs.meta`
- `Assets/ForestPrototype/MixedDensityModel.cs`
- `Assets/ForestPrototype/MixedDensityModel.cs.meta`
- `Assets/ForestPrototype/ScenarioOne/ManagementHistoryText.cs`
- `Assets/ForestPrototype/ScenarioOne/ManagementHistoryText.cs.meta`
- `Assets/ForestPrototype/ScenarioOne/ScenarioInterventionHistory.cs`
- `Assets/ForestPrototype/ScenarioOne/ScenarioInterventionHistory.cs.meta`
- `Assets/ForestPrototype/ScenarioOne/ScenarioOneEconomyAdapter.cs`
- `Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.History.cs`
- `Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.History.cs.meta`
- `Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.cs`
- `Assets/ForestPrototype/SitkaGrowthModel.cs`
- `Assets/ForestPrototype/SitkaSubsetDensitySignal.cs`
- `Assets/ForestPrototype/SitkaSubsetDensitySignal.cs.meta`
- `Assets/ForestPrototype/Species/BeechSpecies.asset`
- `Assets/ForestPrototype/Species/SessileOak.asset`
- `Assets/ForestPrototype/TimberYield/TimberYieldCalculator.cs`
- `Assets/ForestPrototype/TimberYield/TimberYieldConfiguration.cs`
- `Assets/ForestPrototype/TimberYield/TimberYieldValidation.cs`
- `Assets/ForestPrototype/TreeSpeciesDefinition.cs`
- `Assets/ForestPrototype/UI/AnnualReviewView.cs`
- `Assets/ForestPrototype/UI/ScenarioOneUiRoot.cs`
- `Assets/ForestPrototype/UI/TreeInspectionView.cs`
- `Assets/ForestPrototype/UI/WalkingHudView.cs`
- `Assets/ForestPrototype/UI/WorkPlanView.cs`
- `Assets/ForestPrototype/WorkEconomy/ForestryWorkDomain.cs`
- `Assets/ForestPrototype/WorkEconomy/Stage1EconomyDefaults.cs`
- `Assets/ForestPrototype/WorkEconomy/TimberAssortmentLabels.cs`
- `Assets/ForestPrototype/WorkEconomy/TimberAssortmentLabels.cs.meta`
- `Docs/Verification/ThreeSpeciesProgramme/CompositeDensityV2Evidence/baseline-density-and-isolation.txt`
- `Docs/Verification/ThreeSpeciesProgramme/CompositeDensityV2Evidence/final-composite.txt`
- `Docs/Verification/ThreeSpeciesProgramme/CompositeDensityV2Evidence/initial-run.txt`
- `Docs/Verification/ThreeSpeciesProgramme/CompositeDensityV2Evidence/manifest.json`
- `Docs/Verification/ThreeSpeciesProgramme/CompositeDensityV2Evidence/mutations.json`
- `Docs/Verification/ThreeSpeciesProgramme/CompositeDensityV2Evidence/production-typecheck.txt`
- `Docs/Verification/ThreeSpeciesProgramme/CompositeDensityV2Evidence/timber-history.txt`
- `Docs/Verification/ThreeSpeciesProgramme/CompositeDensityV2Evidence/tools-typecheck.txt`
- `Docs/Verification/ThreeSpeciesProgramme/CompositeDensityV2Handoff.md`
- `Docs/Verification/ThreeSpeciesProgramme/CompositeDensityV2Predeclared.md`
- `Docs/Verification/ThreeSpeciesProgramme/Phase0-Baseline.md`
- `Docs/Verification/ThreeSpeciesProgramme/Phase1-DensityRework.md`
- `Docs/Verification/ThreeSpeciesProgramme/Phase1-MixedDensity.md`
- `Docs/Verification/ThreeSpeciesProgramme/Phase10-Qualification.md`
- `Docs/Verification/ThreeSpeciesProgramme/Phase2to5-BroadleafParity.md`
- `Docs/Verification/ThreeSpeciesProgramme/Phase6-BroadleafTimber.md`
- `Docs/Verification/ThreeSpeciesProgramme/Phase7to9-HistoryAndRepeatedInterventions.md`
- `Docs/Verification/ThreeSpeciesProgramme/README.md`
- `Tools/Verification/CloudTypeCheck/InputSystemStub.cs`
- `Tools/Verification/CloudTypeCheck/UnityEditorStub.cs`
- `Tools/Verification/CloudTypeCheck/baseline-prod.txt`
- `Tools/Verification/CloudTypeCheck/baseline-tools.txt`
- `Tools/Verification/CloudTypeCheck/run_typecheck.py`
- `Tools/Verification/ThreeSpecies/BroadleafAdultGrowthVerification.cs`
- `Tools/Verification/ThreeSpecies/BroadleafTimberEconomyVerification.cs`
- `Tools/Verification/ThreeSpecies/MixedDensityVerification.cs`
- `Tools/Verification/ThreeSpecies/OfflineLogic/BroadleafTimberOfflineCheck.cs`
- `Tools/Verification/ThreeSpecies/OfflineLogic/CompositeDensityOfflineCheck.cs`
- `Tools/Verification/ThreeSpecies/OfflineLogic/HistoryOfflineStubs.cs`
- `Tools/Verification/ThreeSpecies/OfflineLogic/MixedDensityIsolatedCheck.cs`
- `Tools/Verification/ThreeSpecies/OfflineLogic/MixedDensityOfflineCheck.cs`
- `Tools/Verification/ThreeSpecies/OfflineLogic/ScenarioHistoryOfflineCheck.cs`
- `Tools/Verification/ThreeSpecies/OfflineLogic/SitkaIsolatedCheck.cs`
- `Tools/Verification/ThreeSpecies/OfflineLogic/SitkaYieldFixture.cs`
- `Tools/Verification/ThreeSpecies/OfflineLogic/UnityShim.cs`
- `Tools/Verification/ThreeSpecies/OfflineLogic/run_composite_mutations.py`
- `Tools/Verification/ThreeSpecies/OfflineLogic/run_offline_check.py`
- `Tools/Verification/ThreeSpecies/ScenarioHistoryVerification.cs`
- `Tools/Verification/ThreeSpecies/run_unity_gates.py`
### Step 2: site

Predecessor `8a665fa3a8413816ac9f79c3c37562b5e388cb50`; source `47539847d7e2531824c30f3dfe635d55404699a8`; ephemeral cumulative commit `467d71313d29aae5d95493e2a1171fd88f5cfefd` (discarded, not published). Merge diagnostic:

```text
Automatic merge went well; stopped before committing as requested
```

- `Assets/ForestPrototype/ForestSiteProfile.cs`
- `Assets/ForestPrototype/ForestSiteProfile.cs.meta`
- `Assets/ForestPrototype/ScenarioSiteProfiles.cs`
- `Assets/ForestPrototype/ScenarioSiteProfiles.cs.meta`
- `Assets/ForestPrototype/SpeciesSiteResponse.cs`
- `Assets/ForestPrototype/SpeciesSiteResponse.cs.meta`
- `Tools/Verification/SiteInfrastructure/OfflineLogic/SiteInfrastructureOfflineCheck.cs`
- `Tools/Verification/SiteInfrastructure/run_site_check.py`
### Step 3: ux

Predecessor `467d71313d29aae5d95493e2a1171fd88f5cfefd`; source `84e881aa6d0e96c5752f09492d5db37d2cc8925e`; ephemeral cumulative commit `ffc98bdf8db85a7a70f53a33469a45535737f729` (discarded, not published). Merge diagnostic:

```text
Automatic merge went well; stopped before committing as requested
```

- `Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.cs`
- `Assets/ForestPrototype/UI/AdvanceGuard.cs`
- `Assets/ForestPrototype/UI/AdvanceGuard.cs.meta`
- `Assets/ForestPrototype/UI/AnnualReviewView.cs`
- `Assets/ForestPrototype/UI/ControlsReference.cs`
- `Assets/ForestPrototype/UI/ControlsReference.cs.meta`
- `Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs`
- `Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs.meta`
- `Assets/ForestPrototype/UI/LearningObjectivesView.cs`
- `Assets/ForestPrototype/UI/MenuHelpView.Text.cs`
- `Assets/ForestPrototype/UI/MenuHelpView.Text.cs.meta`
- `Assets/ForestPrototype/UI/MenuHelpView.cs`
- `Assets/ForestPrototype/UI/ScenarioOneUiRoot.cs`
- `Assets/ForestPrototype/UI/StandaloneSessionMenu.Startup.cs`
- `Assets/ForestPrototype/UI/StandaloneSessionMenu.cs`
- `Assets/ForestPrototype/UI/TreeInspectionView.cs`
- `Assets/ForestPrototype/UI/WalkingHudView.cs`
- `Assets/ForestPrototype/UI/WorkPlanOverview.cs`
- `Assets/ForestPrototype/UI/WorkPlanView.cs`
- `Docs/Verification/ScenarioOneUxGap/Handoff.md`
- `Tools/Verification/MenuTutorialVerification.cs`
- `Tools/Verification/ScenarioOneTeachingCopyVerification.cs`
- `Tools/Verification/ScenarioOneUx/OfflineLogic/ScenarioOneUxOfflineCheck.cs`
- `Tools/Verification/ScenarioOneUx/run_ux_check.py`

## Missing dependencies and STOP conditions

Density candidate is still WAITING_ON_UNITY U3 despite successful source merges. Missing bounded production Oak/Beech calibration, chronological N3/U4 implementation and qualified integrated UX fix disposition. No integrated external-demo candidate exists. Full-loop/saves/runtime/Player/native Windows/performance qualification remains open.

Stop an eventual integration for unavailable exact source, competing writer, accepted-mechanic ambiguity, required schema bump, unauthorised Assets edits, canonical contradiction or anchor rebaseline. Additionally fail-fast on new approximate errors/offline assertion failures, reinstated pooled density, active site wiring, changed Sitka numerics or unapproved model default. Capture and abort any future ambiguous conflict; do not make a gameplay decision as a merge resolution. Final published audit worktree contains docs/evidence only and is clean after commit; disposable worktree was removed after reset.
