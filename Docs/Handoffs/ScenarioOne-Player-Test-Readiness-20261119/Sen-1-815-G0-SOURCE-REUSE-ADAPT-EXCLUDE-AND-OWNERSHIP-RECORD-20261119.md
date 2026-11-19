# Sen-1 #815 — G0 source inspection, REUSE / ADAPT / EXCLUDE map, and ownership record

**Date:** 19 November 2026
**Parent claim:** #815
**Overall Manager:** #731
**Orchestrator:** one native T3 Code / OpenCode parent, `opencode-go/deepseek-v4.1-flash`
**Source base:** `origin/main` @ `159f0ff8b075e275045ddd1ab0925cb447364b77`
**Source branch:** `feature/sen-1-815-compact30-e2-loop-reuse-20261119-v1`
**Source worktree:** `/media/jer/Videos/CCF-815-E2-Core-20261119`
**Docs branch:** `docs/sen-1-815-e2-parent-20261119-v1`
**Docs worktree:** `/media/jer/Videos/CCF-815-E2-Parent-20261119`
**Status:** RUNNING — G0 complete; A–C implemented, advisory D implemented; save/Unity/owner gates recorded below.

This record is the G0 output required by the #815 packet. It is not an acceptance, a spec promotion, a world signature or a Unity qualification.

## 1. Verified inputs and authority actually read

| Input | Verified state |
|---|---|
| Decision Log / Current Milestone / Game Brief / Unity Overview / AgentWorkflow | Read from the source base. D-083/D-084/D-085 are the controlling Compact product intent. |
| #782 parent DRAFT `specs/scenario-one-compact-complete-loop-v1` | Read on `docs/sen-1-782-compact30-canonical-draft-stage-20261118-v1` @ `8dda056b`. **DRAFT / NOT APPROVED**; 148 inherited requirements unchecked. |
| #782 subordinate `specs/scenario-one-compact-initial-planting-v1` | Read on the same branch. **DRAFT**; CIP-001…CIP-033 unchecked. |
| Signed #677 supplement | Read on `docs/sen-1-677-manager-compact-save-policy-supplement-20261117-v1`. Signed technical policy; no new wire key; existing Annual Review acknowledgement is the one covering control. |
| #813 E1 scene lineage | Read on `sen-1/813-compact30-e1-visual-layout-fix-20261119-v1` @ `be60c174`. The scene is a **visual shell only**: 30×30 m ground/collider, H1, C0 path, `Trees` with exactly 175 `P####_…` Sitka prefab instances at the #676 roster coordinates, and a player/camera/CharacterController. It contains **no** `ScenarioOneManager`, `ForestEcologyController`, `ForestTreeSpawner`, `ForestSaveController` or `ForestTree` component. **The scene is not on `main` or on this feature branch; the E2 native gate must open the #813 scene lineage and apply this source candidate, or wait for the owner-released corrected scene.** |
| #813 placement JSON | `ScenarioOneCompact30Placement.json` **still fails strict JSON parsing** (`Expecting ',' delimiter` at line 132, the missing comma between `correction_20261119` and `player`). The malformed bytes were **not** edited, copied as a whole document or used as authority. Only the owner-observed `correction_20261119.old_to_new` values were read as explicitly provisional E1 test data, as the packet permits. The E1 scene's four `G1_*` marker transforms were independently read and are exactly the same corrected positions: O1 (0.65, −2.65), O2 (0.20, −1.10), B1 (−1.20, −2.45), B2 (−0.45, −4.30). |
| #690 shared save source | Read only. No file in the 17-file allowlist was edited. |

## 2. Reusable source actually inspected

| File | Relevant facts |
|---|---|
| `Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.cs` (2,783 lines) | Existing cash/inventory, `TryPurchaseStock`, exact `TryDesignateExactPlanting`, Pending/Approved/Completed/Failed orders, `ApprovePendingWork`, `AdvanceYear`, `MarkAnnualReviewSeen`, crop-tree count, real annual report, per-order `WorkResolved` events, ecology annual step, planted juveniles, deadwood, history hooks. |
| `ScenarioOneManager.History.cs` | Read-only `Interventions`, `HistoryYears`, `CycleState`, `NextStepHint`; derived from real records. |
| `ScenarioOneManager.Prescriptions*.cs`, `ScenarioOnePrescriptionApproval.cs` | Rack/prescription flow; **not** used by Compact. |
| `ScenarioOneWorkOrder.cs` | `ScenarioWorkType`, `ScenarioWorkStatus`, exact-position fields, `executionMethod`, `installShelter`, `createdYear`, `resolvedYear`. |
| `PlantedJuvenile.cs` | Real `juvenileId`, species, exact position, `plantingYear`, survival and browse state. |
| `WorkEconomy/ForestryWorkCalculator.cs`, `ScenarioOneEconomyAdapter.cs` | Genuine quotes, contractor/owner minutes, stock and cash settlement. |
| `ForestTree.cs` | Stable `treeId`, living/standing/stump, mutually exclusive `Fell` / `CropTree` marks, growth, pruning, `Felled` event. |
| `ForestTreeMarkingManager.cs` | Real raycast inspection, X fell mark, C crop-tree mark, competition/light summary. |
| `ForestTreeSpawner.cs` | Public `Spawn(...)` factory and species resolution; used for any missing authored stem and later promoted juveniles. |
| `ForestEcologyController.cs` + partials | 5 m cell grid, competition/light/canopy, regeneration, browsing, storms, exactly one annual step. |
| `ForestEcologyCell.cs`, `SpeciesConfigurationPolicy.cs`, `TreeSpeciesDefinition.cs` | Cell state and species readiness. |
| `ScenarioOneDefinition.cs`, `ScenarioOne.asset` | €12,000 calibration, €4.50 Beech / €5.50 Oak stock, €5 shelter, 45 €/h contractor, owner minutes. |
| `UI/ScenarioOneUiRoot.cs`, `WorkPlanView.cs`, `AnnualReviewView.cs`, `WalkingHudView.cs`, `TreeInspectionView.cs` | Existing runtime UI Toolkit composition; Work Plan purchase/approve/advance and Annual Review acknowledgement routes. |
| `UI/LearningObjectivesView*.cs`, `ScenarioOneWorkPlanFocus.cs`, `ScenarioOneTeachingCap` | Five-rack teaching gates that would otherwise lock Compact planting/approval/advance. |
| `ForestPlayer.cs` | Movement/inspection; also free G-planting, F-harvest and U-uproot routes that bypass the Work Plan. |
| `ForestSaveController.cs`, `ForestSaveData.cs`, `ForestSaveValidation*.cs`, `ForestStartingStand.cs` | Inspected only. **Read-only; SAVE INTEGRATION HELD.** |
| `Tools/Verification/CloudTypeCheck/` | Existing source-only compile harness. |

## 3. REUSE / ADAPT / EXCLUDE map

### REUSE unchanged (behind the Compact adapter)

| Capability | Exact API / caller | Owner | Regression witness |
|---|---|---|---|
| Stable tree identity, living/standing/stump | `ForestTree.TreeId`, `IsLiving`, `CanChop`, `Stage` | shared source | Existing `Sen1Compact30Preflight` / old tree tests; new Compact checks in this task |
| Fell / CropTree exclusivity and marks | `ForestTree.SetMark`, `ForestTreeMarkingManager.ToggleMark/ToggleCropTree` | shared source | Existing tree-marking tests; Compact count/inspection tests |
| Exact-position planting order | `ScenarioOneManager.TryDesignateExactPlanting(itemId, position, method, shelter)` | shared source | Existing `CCFOakPlayerPlantingVerification` / `CCFBeechVerification`; new Compact site tests |
| Stock purchase and reservation | `TryPurchaseStock`, `GetStockQuantity`, `GetReservedStockQuantity` | shared source | Existing `ScenarioOneEconomyIntegrationVerification` |
| Work Plan quote/approval | `GetWorkPlanSummary`, `ApprovePendingWork`, `ValidateOpenOrders`, `AdvanceYear` | shared source | Existing economy/annual tests |
| Genuine annual settlement | `ResolveOrder`/`ResolvePlanting`, cash/stock/minutes, `ScenarioAnnualReport` | shared source | Existing annual/economy tests |
| Real juvenile creation and survival | `PlantedJuvenile`, `AdvancePlantedJuveniles`, `JuvenileEcologyRules` | shared source | Existing `JuvenileMortalityFoundationVerification` |
| Ecology annual step | `ForestEcologyController.AdvanceOneYear`, competition/light/canopy/regeneration/browse | shared source | Existing ecology verification suites |
| Annual review history and acknowledgement | `MarkAnnualReviewSeen`, `ScenarioOneCompletionRules.ShouldRecordAcknowledgement` | shared source | Existing annual-review tests |
| Read-only intervention history | `ScenarioOneManager.Interventions`, `HistoryYears` | shared source | Existing S1C/S1-D verification directories |
| UI shell (HUD, inspection, Work Plan, review, map) | `ScenarioOneUiRoot` runtime construction | shared source | Existing UI qualification evidence |
| Species definitions/economy calibration | `ScenarioOne.asset`, `Species/*.asset` | shared data | Existing Scenario One calibration anchors |

**Regression-witness caveat:** the right-hand column names suites/oracles that exist in the repository. **None of those old-world suites was run in this task** because no lawful Unity Editor lease was available. The only executed regression evidence is the source-level Unity-6000 compile plus the new Compact checks. Do not read this column as a PASS.

### ADAPT for Compact (all implemented in this task)

| Adaptation | Where | Why |
|---|---|---|
| Separate 30×30 m runtime/ecology geometry, 6×6 = 36 cells | `StandGeometryModel.Compact30 = 2`, `IsRuntimeGeometry`, `ForestEcologyController.ApplyStandGeometry` | The old 40/80 m models cannot express Compact30. `IsKnown` deliberately remains 0/1 so #690 save validation **rejects** a Compact save while save integration is held. |
| Compact runtime composition on the E1 visual shell | new `Compact30Bootstrap.cs` | The E1 scene has no scenario components. Runtime addition keeps the #813 scene file untouched. |
| Authored-visual tree identity without reshaping the E1 prefabs | `ForestTree.InitializeAuthoredVisual`, `visuallessAuthored` | The E1 prefabs are the approved look; a `ForestTree` proxy must not replace or rescale them. |
| Spawner runtime configuration | `ForestTreeSpawner.ConfigureForRuntime` | The runtime factory needs the three candidate species, bark and canopy template without Editor serialization. |
| H1 arrival + current stand assessment gates | new `Compact30Session.cs` (Y / J / Y) | No-rack teaching that the five-rack manager does not provide. |
| Three player-selected living Sitka Crop Trees as **non-gating** guidance | `Compact30Session` count + existing C key | D-085 OD-17 / M-4; no premarks, no latch, no save field. |
| Four fixed G1 sites: 2 sessile Oak + 2 European Beech | new `Compact30SiteRules.cs`, `Compact30Sites.json`, `Compact30Session.TryDesignate` | Exact species/stock/position validation; no free placement, no snapping. |
| Strict site clearance | `Compact30Rules.ValidateSite` (path envelope 0.8 + shelter 0.6, H1, stem 0.7, separation 0.5, cell, recorded-clearance drift) | Owner-observed #813 E1 correction; the four sites are provisional test data, not signed production coordinates. |
| Four-slot group and replacement rules | `Compact30GroupOrder` list; Completed blocked, Failed/cancelled replaceable, max 2 per species | D-084 C3/C4 and #677 T-07. |
| Approved-order immutability at service level | `ScenarioOneManager.ApprovedWorkOrdersImmutable` guards `SetPlantingExecution` / `SetPlantingShelters`; approved quote frozen in `ValidateOpenOrders`; per-site controls replace the bulk UI | The old bulk setters demoted Approved→Pending. |
| Covering review binding | `ScenarioOneManager.AnnualReviewAcknowledged` event + `Compact30Session.OnAnnualReviewAcknowledged`; `ScenarioOneUiRoot.NeedsAnnualReview` includes `CoveringReviewPending` | The existing single Annual Review control is the covering control; no second button or event. |
| CT-3 all-route annual hold | `ScenarioOneManager.AdvanceYearBlocker` checked in `AdvanceYear` and `CanAdvanceYearNow` | Blocks normal and UI advance once final 2+2 is true and the covering review is unacknowledged. |
| No historical two-cycle Compact completion | `ScenarioOneManager.SuppressHistoricalCompletion` | The old two-cycle checker must never award Compact credit. |
| No five-rack teaching leakage | Compact bypass in `TutorialHint`, `RackPlanProblem`, `RackApprovalProblem`, intro, O panel, HUD objective widget, Objectives buttons (HUD/map/review/work plan), Work Plan planting focus, Annual Review objectives card, Stand Map access summary, and no `RackTool`/J-K rack decision in Compact | Compact must remain free of rack-first prompts, objective quotas and MFM. **Static/source review only; runtime not verified.** |
| No free G/F/U bypass | `ForestPlayer` ignores G planting, F harvest, U uproot and 1/2 planting slots when `Compact30Session.IsActive` | Commitment must go through the Work Plan; no free felling/planting. |
| Felling visual on authored trees | `Compact30Session.OnAuthoredTreeFelled` hides the authored visual and adds a stump | The authored prefab has no `ForestTree` trunk/canopy proxies. |
| New candidate data fixtures | `Compact30Roster.json`, `Compact30Sites.json` + species/definition Resource copies | The runtime needs loadable data. The roster digest is verified at load; the sites are labelled provisional. |

### EXCLUDE completely from Compact gameplay

Extraction racks and Rack V2 planning/entrance geometry; orange first-rack marks; rack-first tutorial and `RackFirstStageActive`/`RackPlanProblem`/`RackApprovalProblem` gates; MFM sample propagation; the old five-rack two-cycle completion; the Enlarged80 headland/feeder/rack clearance and global new-game switch; the old stand tree inventory as the Compact one; H1's 14 pre-world omissions as prior fells/revenue/credit; the 226 exterior decorative Sitka/ground/brash/fence as ecology cells, tree IDs, operations, payments or persisted identity; free player planting/felling/uprooting outside the Work Plan; a physical manual-felling minigame; vehicle use of the pedestrian path; new save roots/keys and CCF M-A completion storage; Windows builds.

## 4. Ownership and gates

### Actual source ownership

| Item | Value |
|---|---|
| Exclusive core source writer | this one parent (no parallel source writer) |
| Branch / HEAD at G0 | `feature/sen-1-815-compact30-e2-loop-reuse-20261119-v1` from `159f0ff8` |
| Disjoint worktree | `/media/jer/Videos/CCF-815-E2-Core-20261119` (ext4, non-root storage) |
| New source directory | `Assets/ForestPrototype/ScenarioOne/Compact30/**` (+ `.meta`) |
| New verification directory | `Tools/Verification/Sen1CompactE2Reuse/**` |
| Narrow shared-file grants used | `StandGeometryModel.cs`, `ForestEcologyController.cs`, `ForestTree.cs`, `ForestTreeSpawner.cs`, `ScenarioOneDefinition.cs`, `ScenarioOneManager.cs`, `UI/ScenarioOneUiRoot.cs`, `UI/WorkPlanView.cs`, `UI/AnnualReviewView.cs`, `ForestPlayer.cs` — each only the documented hooks above |
| Not touched | `ForestSaveController.cs`, `ForestSaveData.cs`, `ForestSaveValidation*.cs`, `ForestStartingStand.cs`, session/profile routing, the #690 allowlist, the #813 scene/JSON/`.meta`, `ForestTest.unity`, the frozen Reference Future v1, old saves |
| Other owners' work | Preserved: the anchor's two modified tracked files and three untracked operations notes; all existing worktrees listed by `git worktree list`; the owner primary checkout's seven historical dirty entries |

### Spec and source permission

The #782 parent/child specs are **DRAFT** and were not promoted. The direct owner instruction in #815 outranks the draft and explicitly authorises this implementation. A bounded **orchestrator-recorded IMPLEMENTING subset** for A–C plus advisory D is recorded in `Sen-1-815-ORCHESTRATOR-SPEC-SUBSET-AND-SOURCE-GRANT-RECORD-20261119.md`. It is not a Manager #731 signature, not a durable spec approval, not a world signature and not a save-policy amendment. **Manager #731 ratification or rejection of that subset and the file grants is still an open gate; the internal review correctly treats it as a major process blocker.** All inherited requirement checkboxes remain untouched.

### Unity custody

No lawful machine-wide Unity lease route was available: the deployed `unity-lock` tool is absent from the current Operations tree and the recorded lanes configuration is stale. The #815 packet explicitly forbids reopening that investigation and forbids using a known unsafe lock. No Unity Editor was started and no other owner's process was touched. **Editor/native verification: NOT RUN.** The source was instead compiled against the actual Unity 6000.6.0f1 managed assemblies (runtime `Assets/**/*.cs`, excluding `Editor/`), with 0 errors, and separately through `Tools/Verification/CloudTypeCheck` (2021.3 approximation, clean baseline). Neither is a Unity import, scene binding, GUI run or gameplay PASS.

### Save integration

**SAVE INTEGRATION HELD.** Compact teaching state is in memory only. `ForestSaveData`/`ForestSaveValidation`/`ForestSaveController` and the #690 allowlist were not edited, so no Compact save is captured or admitted. The runtime explicitly displays this. `StandGeometryModel.IsKnown` still refuses model 2, so a hand-made Compact save is rejected before mutation rather than silently admitted.
