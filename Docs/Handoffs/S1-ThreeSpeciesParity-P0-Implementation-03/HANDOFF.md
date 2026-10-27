# PARITY-03 — Mixed-Density Candidate Requalification — Handoff

**Task:** `PARITY-03 — mixed-density candidate requalification + gap closure`
**Spec:** `specs/scenario-one-three-species-parity-p0/` (APPROVED; read from `origin/main` because the implementation base predates the approved-spec commit)
**Claim:** GitHub issue #8 (`jermur788/CCF`)
**Date:** 2026-10-27
**Worker / routing:** DeepSeek v4.1 Flash (`opencode-go/deepseek-v4.1-flash`) via OpenCode in T3 Code; implementation/qualification. Independent Claude review is a separate task.
**Worktree:** `/media/jer/ZX20/CCF-Parity03` (new, clean; the dirty Primary checkout was not touched)
**Branch:** `feature/scenario-one-three-species-parity-density-requal-v1`
**Base:** `c6342f79a3e89c040fadd232017a937132779385` (Manager/issue-selected Rack V2 03C lane head; production tree identical to this base)
**Tested HEAD:** `e1515e165eb40b40b5200992398984d69e0e8e8e` (packet-only commit above the base)
**Unity:** **NOT RUN.** The machine-wide Unity lane was occupied by the Rack V2 repair task throughout this pass (Unity PID 539342 on `/media/jer/ZX20/CCF-U1` at claim time; a later Rack repair run was active on the same lane at 18:52Z). This worktree has no Unity `Library` and was not launched.

## Verdict

**STATIC CONFORMANCE PASS — NOT UNITY-QUALIFIED.**

The inherited model-2 mixed-density implementation structurally conforms to FR-1/FR-2 of the approved parity spec. No production change is required. The packet exit token `MIXED DENSITY PARITY REQUALIFIED` is **not** claimed because the required Tier-2 Unity density qualification has not run.

Precise blocker: **machine-wide Unity lane unavailable (owned by Rack V2 repair); route this exact candidate to an available Unity lane for the focused mixed-density gate.**

## 1. Base selection and preflight

- The packet names `c6342f79a3e89c040fadd232017a937132779385` or a later Manager-chosen Rack-integrated descendant. Issue #8 confirms `c6342f7`; the implementation branch and this worktree were created from it.
- `c6342f7` is the Rack V2 03C lane head, **not** the combined Rack V2 integration head `9127c02`. Therefore this handoff makes **no integration-qualification claim**.
- Live ownership was read from `origin/main:Docs/Project/coordination/active-tasks.md` (PARITY-03 `READY / DISPATCHED`; no other density writer). The branch's own `active-tasks.md` predates the PARITY-03 row; it was not edited because the issue and live main file remain the coordination authority.
- No Rack V2 file, species YAML, scene/prefab, save schema, timber/economy/pruning/UI, Multi-Scale Forestry Management or Reference archive file was edited.
- The Primary checkout remained on `ops/workflow-foundation-v1` with its pre-existing dirty/untracked work untouched.

## 2. Production diff

**NONE.**

`git diff --stat c6342f7..HEAD -- Assets Packages ProjectSettings Tools` is empty. The only commit above the base is `e1515e1`, which adds `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-03/TASK-PACKET.md`. No production source change was made because the inherited implementation already conforms.

## 3. Files and symbols inspected

- `Assets/ForestPrototype/MixedDensityModel.cs` — `DensityRoute`, `GrowingSpaceDemandClass`, `DensityStem`, `SelectRoute`, `AnyNonReference`, `Weight`, `StemOccupiedSpace`, `OvercrowdingPressure`, `ExpectedAnnualLoss`, `Suppression`, `Vulnerability`, `AnnualLossProbability`, `Plan`, `CanonicalOrder`.
- `Assets/ForestPrototype/MixedDensityModel.Composite.cs` — `GenericResponseCalibration`, `PlanComponents`, `StemBasalArea`, `MarkTaken`, `GenericQuotaRollKey`, basal-area budget.
- `Assets/ForestPrototype/SitkaSubsetDensitySignal.cs` — `SitkaCi50`, `Resolve`, `RequiredDeaths`.
- `Assets/ForestPrototype/ForestEcologyController.cs` — `UsesSpeciesAwareGrowth` (line 151), `ApplyAdultDensityMortality` (1062), `FindTrees` (875), `HasNonReferenceDemand` (1160), `ApplySpeciesAwareDensityMortality` (1177), `KillDensityVictims`, `StandRelativeDensity`, `PromoteSpeciesCohorts` (1714).
- `Assets/ForestPrototype/ForestTree.cs` — `IsLiving` (82), `IsStump` (155), `IsBiologicallyDead`.
- `Assets/ForestPrototype/TreeSpeciesDefinition.cs` — default `growingSpaceDemand = Reference` (30), `GrowingSpaceDemand` (142).
- `Assets/ForestPrototype/ForestEcologyCell.cs` — sub-promotion `ForestRegenerationCohort` lives in cell `Regeneration` (5, 52-53), not as `ForestTree`.
- `Assets/ForestPrototype/Species/{SitkaSpruce,SessileOak,BeechSpecies}.asset` — demand defaults/values.
- `Assets/ForestPrototype/AdultMortalityAccount.cs` — diagnostic route/account fields (never saved).
- `Assets/ForestPrototype/SitkaGrowthModel.cs` — protected arithmetic, inspected but not changed.
- `Tools/Verification/ThreeSpecies/OfflineLogic/*` — mixed/composite offline checks, isolation compiles, continuity sweep, long runs.
- `Tools/Verification/ThreeSpecies/MixedDensityVerification.cs` — the existing in-Editor gate (not run; Unity unavailable).
- Approved spec/plan/tasks from `origin/main` and the PARITY-02 evidence handoff.

## 4. Route-selection proof

| Requirement | Evidence | Result |
|---|---|---|
| Models 0/1 never enter the model-2 mixed route | `UsesSpeciesAwareGrowth => growthModelVersion >= GrowthModel.SpeciesAware` (`ForestEcologyController.cs:151`); `SelectRoute(false, ...)` always returns `LegacySitkaExact` (`MixedDensityModel.cs:176-178`); offline `ROUTE_MODELS_BELOW_2_ALWAYS_LEGACY` | PASS |
| Model-2 pure Sitka enters the exact protected route | `HasNonReferenceDemand` returns false when every living stem is `Reference`; `SelectRoute(true, false) == LegacySitkaExact`; the composite method returns before the legacy block; offline `ROUTE_PURE_SITKA_IS_LEGACY_SITKA_EXACT` (1-6 and 1000 stems) | PASS |
| Any living non-Reference stem switches to the component route | `HasNonReferenceDemand` tests each living tree's `GrowingSpaceDemand != Reference`; Oak asset = 2 (High), Beech asset = 1 (Intermediate); offline `ROUTE_ANY_OAK_OR_BEECH_IS_SPECIES_AWARE_OCCUPANCY` (400/400) and `ROUTE_ONE_OAK_FLIPS_AND_REMOVAL_FLIPS_BACK` | PASS |
| Dead stems do not switch the route | `FindTrees()` excludes `IsBiologicallyDead` (`ForestEcologyController.cs:882`); `IsLiving => !IsStump && !biologicallyDead` (`ForestTree.cs:82`); `ApplyAdultDensityMortality` adds only `tree.IsLiving` (1068) | PASS |
| Stumps do not switch the route | `IsLiving` excludes `IsStump`; `FindTrees()` may include stumps but the density input filters them via `IsLiving` | PASS |
| Sub-promotion regeneration cohorts do not switch the route | They are `ForestRegenerationCohort` objects in `ForestEcologyCell.Regeneration` (`ForestEcologyCell.cs:5,52-53`), never `ForestTree`; only promotion calls `spawner.Spawn(...)` (`ForestEcologyController.cs:1741`), creating a `ForestTree` that then counts as a living individual | PASS |
| Promoted juveniles count | Promotion creates a `ForestTree` via `ForestTreeSpawner.Spawn`; the density input is every living `ForestTree` | PASS |
| Class weights are 1.0 / 1.5 / 2.0 | `ReferenceWeight = 1.0`, `IntermediateWeight = 1.5`, `HighWeight = 2.0` (`MixedDensityModel.cs:138-140`); offline `WEIGHTS_ARE_THE_PROVISIONAL_CLASSES` | PASS |
| Species `Ci50` does not bias species-aware density victims | `DensityStem` carries no species id (`MixedDensityModel.cs:84-95`); generic suppression uses one common `NeutralHalfSaturationCi = 5.0` and `VulnerabilityExponent = 4.0` (`152`, `227-229`); `MixedDensityModel`/`.Composite` code contains no `Ci50`; offline `NO_SPECIES_CLASS_OR_WEIGHT_IN_ANY_MORTALITY_FORMULA_SOURCE`, `HAZARD_HAS_NO_SPECIES_PREFERENCE_AT_EQUAL_COMPETITION_AND_SIZE` (Ref 2298 / Intermediate 2313 / High 2167 deaths, max deviation 2.38 sigma), and `GENERIC_ALLOCATION_DEPENDS_ON_COMPETITIVE_POSITION_ONLY_NOT_ON_WHICH_SPECIES_SITS_WHERE` | PASS |
| The protected Sitka component only takes Reference victims | `SitkaSubsetDensitySignal.Resolve` selects only `Reference` stems; `MarkTaken` rejects any non-Reference Sitka victim (`MixedDensityModel.Composite.cs:252`); offline `I4_NO_OAK_OR_BEECH_STEM_IS_EVER_A_SITKA_COMPONENT_VICTIM` | PASS |
| No DEC / DMC / mixture-productivity bonus | Occupancy is plain weighted cross-section addition; offline `OCCUPANCY_IS_ADDITIVE_NO_MIXTURE_BONUS_NO_DEC_NO_DMC` (worst relative difference 1.8e-15) | PASS |
| No Oak/Beech SDI or broadleaf maximum-density equation | `MixedDensityModel` uses a dimensionless occupancy normalization and bounded response, not a species maximum-density law; offline `CALIBRATION_CONSTANTS_ARE_NAMED_AS_NORMALIZATION_AND_LOSS_NOT_DENSITY_LIMITS`, `NO_HARD_SPECIES_DENSITY_CAPACITY` | PASS |

A reproducible source-level proof (24/24 checks) is at `/media/jer/Files/CCF/Verification/Parity03-StaticRequalification-20261027T185345Z/static-proof.py` with output `logs/static-proof.log`.

## 5. Protected pure-Sitka arithmetic

No protected file changed. `git diff c6342f7..HEAD -- Assets` is empty. The mixed offline suite re-checks the accepted hashes and reports:

| Check | Result |
|---|---|
| `LEGACY_SITKA_ARITHMETIC_UNCHANGED_FROM_BASE_C1564B8` | PASS (`1a81640dcd11cac8...`) |
| `LEGACY_CONTROLLER_DECISION_BLOCK_UNCHANGED_FROM_BASE_C1564B8` | PASS (`ee7dc266441de752...`) |
| `LEGACY_STAND_RELATIVE_DENSITY_UNCHANGED_FROM_BASE_C1564B8` | PASS (`80ef453c02a4298d...`) |
| `PURE_SITKA_NUMBERS_UNCHANGED_BY_ANY_NORMALIZATION_CHANGE_AND_NO_SHARED_STATE` | PASS (bit-identical; no mutable shared state; invalid calibration rejected) |
| `SITKA_SUBSET_SIGNAL_USES_THE_SITKA_ASSET_CI50` | PASS (asset 5, signal 5) |
| `LONG_RUN_L40_PURE_SITKA_IS_IDENTICAL_UNDER_THE_V2_AND_V3_KERNELS` | PASS |
| `LONG_RUN_E80_PURE_SITKA_IS_IDENTICAL_UNDER_THE_V2_AND_V3_KERNELS` | PASS |
| `I1_AT_THE_REPLACEMENT_STATES_THE_SITKA_COMPONENT_EQUALS_THE_ORIGINAL_BLOCK_ON_THE_SITKA_STEMS` | PASS (2400/2400 draws) |

Current SHA-256 of the inspected files is in `/media/jer/Files/CCF/Verification/Parity03-StaticRequalification-20261027T185345Z/protected-sha256.txt`.

## 6. Near-pure route-continuity measurement

The existing 1000-stem replacement sweep (400 seeds; Sitka share 100 / 99.9 / 99 / 95 / 90%; DBH 15/25/40/60 cm; CI 1/3/8) was rerun on this HEAD. Values are total annual loss / Sitka-component loss.

| Fixture | 100% Sitka | 99.9% | 99% | 95% | 90% |
|---|---:|---:|---:|---:|---:|
| DBH 15 (all CI) | 0.0000 / 0.0000 | 0.0000 / 0.0000 | 0.0000 / 0.0000 | 0.0000 / 0.0000 | 0.0000 / 0.0000 |
| DBH 25 (all CI) | 0.0280 / 0.0280 | 0.0280 / 0.0278 | 0.0279 / 0.0259 | 0.0318 / 0.0191 | 0.0436 / 0.0118 |
| DBH 40 (all CI) | 0.5470 / 0.5470 | 0.5465 / 0.5466 | 0.5416 / 0.5425 | 0.5214 / 0.5236 | 0.5034 / 0.5041 |
| DBH 60 (all CI) | 0.8040 / 0.8040 | 0.8032 / 0.8038 | 0.7961 / 0.8020 | 0.7657 / 0.7937 | 0.7309 / 0.7822 |

Measured checks:

- `C1_TOTAL_LOSS_AT_99_9_PERCENT_SITKA_MATCHES_PURE`: PASS, worst difference **0.0008** (tolerance 0.0050), 12 states.
- `C2_SITKA_COMPONENT_LOSS_PER_SITKA_STEM_AT_99_9_PERCENT_MATCHES_PURE`: PASS, worst difference **0.0005** (tolerance 0.0050).
- `C1_ADJACENT_TOTAL_LOSS_STEPS_ARE_WITHIN_2X_COMPOSITION_CHANGE_PLUS_0_005`: PASS, 48 steps, worst step **0.0348**, smallest margin factor **2.80**.
- `NO_ROUTE_CLIFF_NO_TOTAL_LOSS_STEP_OVER_0_02_FOR_A_COMPOSITION_STEP_OF_0_001_OR_LESS`: PASS.
- The 95%/90% rows diverge as expected because the generic component grows with the non-Reference share; there is no discontinuity at the route seam.

Raw table: `/media/jer/Files/CCF/Verification/Parity03-StaticRequalification-20261027T185345Z/logs/continuity.txt`.

## 7. Mixed fixtures and long runs

Offline reduced-simulator results on this HEAD:

| Fixture | Result |
|---|---|
| L40/E80 pure Oak dense and young | R1-R4 PASS |
| L40/E80 pure Beech dense and young | R1, R3, R4 PASS; **R2 fails in three cases** (`L40 B_BEECH_DENSE`, `L40 F_BEECH_YOUNG`, `E80 F_BEECH_YOUNG`) |
| L40/E80 mixed C_MIXED_DENSE and G_MIXED_YOUNG | R2-R4 PASS; Sitka subset held at its own line; exact replay; generic basal-area share within the 0.125 budget |
| L40/E80 pure Sitka v2/v3 kernels | Bit-identical |
| Negative control (v2 generic setting) | Fails the Beech flattening test as expected |

Typical mixed stand outcomes (living stems at year 300, Sitka/Beech/Oak): L40 C_MIXED_DENSE 10/7/9, L40 G_MIXED_YOUNG 22/12/3, E80 C_MIXED_DENSE 58/26/20, E80 G_MIXED_YOUNG 58/30/10. Full tables are in the log.

The three Beech R2 failures are the already-recorded pre-existing baseline debt (`SCENARIO-TWO-SPECIES-MODEL-FOUNDATION-COMPLETE-V1`, follow-up 5; `CCF-S1-SEQUENTIAL-OBJECTIVES-*` records). They are not caused by PARITY-03: the production tree is byte-identical to the untouched base. They are reported, not rebaselined or retuned.

`MixedDensityOfflineCheck` also proves pure Oak and pure Beech overcrowd, self-thin toward their own occupancy, and lose stems of all three classes in mixed stands; no class is immune or privileged.

## 8. Commands, gates and results

All commands were run in `/media/jer/ZX20/CCF-Parity03`; temporary compile output was outside the repository.

| Command | Result |
|---|---|
| `python3 Tools/Verification/ThreeSpecies/OfflineLogic/run_offline_check.py --suite density --out /tmp/opencode/parity03/offline` | Exit 1. Density 90/91 (one stale scanner false positive); composite 104/107 (three known Beech R2 failures); both isolation compiles PASS (`MIXED_DENSITY_ISOLATED_PASS`, `SITKA_MODEL_ISOLATED_PASS`). |
| `python3 Tools/Verification/CloudTypeCheck/run_typecheck.py --mode prod` | Exit 1. 4 new `Painter2D` errors in UI files (Unity 6 API absent from the 2021.3 reference approximation), 7 old baseline entries gone. No density-file error. Stale approximation baseline, not a density compile gate. |
| `python3 .../Parity03-StaticRequalification-.../static-proof.py /media/jer/ZX20/CCF-Parity03` | Exit 0; 24/24 checks PASS. |
| `git diff --stat c6342f7..HEAD -- Assets Packages ProjectSettings Tools` | Empty (NONE). |
| Unity `MixedDensityVerification` / Tier-2 density gate | **NOT RUN** (machine-wide lane held by Rack V2 repair). |

## 9. Failures, anomalies and unresolved items

1. **Unity Tier-2 qualification is outstanding.** `MixedDensityVerification` (pure Sitka, near-pure mixed, Oak/Beech mixed, protected Sitka fingerprints, compile/import) has not run on this exact HEAD. The route-selection and arithmetic proofs above are static/offline only.
2. **One stale offline source-scanner false positive.** `MixedDensityOfflineCheck.NoBiologicalDensityClaim` flags `ScenarioOneRackBay.MaxRemovedBasalShare` (Rack V2's basal-area-removal safeguard, line 26) as a "max basal area" name. It is not a species density-capacity claim and is not in the density surface. The check was not edited: changing a safety scanner to accommodate a Rack V2 identifier is verification-policy work outside this packet, and the Rack file is forbidden surface. The density model's own constants pass the naming check.
3. **Three pre-existing Beech R2 long-run failures** (listed above), already recorded as baseline debt. No retune.
4. **CloudTypeCheck baseline is stale** for the Rack V2/UI base (Painter2D is a Unity 6 API not present in the 2021.3 approximation). It does not indicate a density defect; the density sources compile unchanged in the offline .NET suites.
5. **Base is not current main.** `c6342f7` is the Manager-selected Rack V2 03C lane head, not combined integration `9127c02` and not `main`. No integration qualification is claimed.

## 10. Evidence locations

- Handoff and raw logs: `/media/jer/Files/CCF/Verification/Parity03-StaticRequalification-20261027T185345Z/`
  - `logs/offline-density.log` (full offline run)
  - `logs/continuity.txt` (continuity table and C1-C5 checks)
  - `logs/routing-and-guards.txt` (route, asset, protected-hash and scanner results)
  - `logs/gates-and-failures.txt` (FAIL lines, long-run tables, isolation tokens)
  - `logs/static-proof.log` (24/24 source-level proof)
  - `logs/typecheck-prod.log` (approximation typecheck)
  - `protected-sha256.txt`, `git-state.txt`, `static-proof.py`

## 11. Verdict and next action

**STATIC CONFORMANCE PASS — NOT UNITY-QUALIFIED.**

No production patch is required. The next required gate is the focused Tier-2 Unity mixed-density qualification on this exact candidate under the machine-wide Unity lock, on an available lane. If that gate produces the required pure-Sitka, near-pure, mixed and fingerprint receipts with no compiler/import errors, PARITY-03 can return `MIXED DENSITY PARITY REQUALIFIED`. Until then, issue #8 remains blocked and no full three-species parity, integration or milestone claim is made.
