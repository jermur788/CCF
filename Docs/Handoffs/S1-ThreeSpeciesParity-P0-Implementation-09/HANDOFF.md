# PARITY-09 — Integrated Three-Species Parity Qualification — Handoff

**Claim:** #219  
**Parent:** #91 / browser Manager #200 / local orchestration #203  
**Authority:** `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-09/TASK-PACKET.md` + `RELEASE-REPIN-20261101.md`  
**Exact candidate:** `3acdf9cb635cdca79606335801e97f9d7dcce8b0`  
**Branch:** `qual/parity-09-integrated-v1`  
**Worker:** DeepSeek V4.1 Flash (`opencode-go/deepseek-v4.1-flash`), orchestrator + primary qualification worker  
**Independent reviewer:** Claude Sonnet 5.5 Medium (cross-provider; separate review file)  
**Status:** **QUALIFICATION EVIDENCE ASSEMBLED — READY FOR INDEPENDENT CROSS-PROVIDER REVIEW**  
**Final token:** not self-issued. `THREE-SPECIES PARITY P0 VERIFIED` remains a Manager verdict after independent review.

## 1. Identity, preflight and boundaries

- Exact candidate verified before every run: `3acdf9cb635cdca79606335801e97f9d7dcce8b0` (`origin/integration/scenario-one-player-test-ready-v1`).
- Branch `qual/parity-09-integrated-v1`; worktree `/media/jer/ZX20/CCF-U1`; clean before and after.
- Machine-wide Unity lock `513ebbfd5fd340b091f3b29d8780979d` acquired normally for the Unity phase and released normally; no Editor/Xephyr/openbox at release.
- Unity 6000.6.0f1; all Unity runs used the generic qualification runner with an isolated licence/config/save path, staged disposable harnesses, and cleanup in `finally`.
- No production, species asset, scene, prefab, package, ProjectSettings, save/schema, expected value, protected anchor or verification baseline was changed. No Windows build. #194 NO ACTION.
- Evidence root: `/media/jer/Files/CCF/Verification/PARITY-09-20261101/`.

## 2. Integrated candidate and ancestry

The candidate is the accepted `6784ab17...` MFM-09/MFM-10 lineage plus the tested #199 presentation/focus repair only (`3acdf9cb`). The #199 production delta is three UI presentation files plus the already-accepted Combined verification fixture. It does not touch parity density/species arithmetic, MFM prescription/coverage/calibration, species assets, save/schema, ecology/economy or protected anchors. The MFM-10 Manager review explicitly carries `SPECIES GUARD QUALIFIED` to this candidate.

Accepted PARITY-03…08 evidence is integrated on this ancestry. Where a historical gate is stale only because the accepted save schema advanced to v24 (S1-C T-05 v22, MFM-06 v23, D-066 v24), that is recorded as verification debt below, not as a parity regression.

## 3. Offline / static results

| Gate | Result | Classification |
|---|---|---|
| `check_integrated_v1.py` | **FAIL** — `ForestSaveData.cs` normalized hash expected `4734c9d7…`, actual `ed2b48b6…` | **Pre-existing verification debt** (PARITY-04 Preflight F-5; save schema advanced to accepted v24). Not a parity defect. |
| `test_source_normalization.py` | **PASS** — `SOURCE_NORMALIZATION_PASS` | Static guard healthy. |
| `SiteInfrastructure/run_site_check.py` | **FAIL** — `G_SAVE_VERSION_IS_STILL_23`; 63 checks, 1 failure | **Stale v23 pin**; current accepted save version is v24. Not a parity defect. |
| `ThreeSpecies/check_fixture_controls.py` | **PASS** — `FIXTURE_CONTROLS_PASS checks=9 static-contracts=7 NOT_UNITY_VERIFIED` | Fixture controls. |
| `check_early_life_conformance.py` | **PASS** — `EARLY_LIFE_CONFORMANCE_STATIC_PASS checks=30 NOT_UNITY_VERIFIED` | PARITY-05 static conformance. |
| `check_player_facing_truth.py` | **PASS** — `PARITY07_COPY PASS checks=89 passed=89 failed=0 NOT_UNITY_VERIFIED` | PARITY-07 copy truth. |
| `run_pruning_check.py` | **FAIL** — `protected bytes moved: ForestSaveData.cs` | **Stale v23 pin**; current accepted save version is v24. Not a pruning/parity defect. |
| `Parity08Save/run_offline_check.py` | **PASS** — `PARITY08_SPECIES_OFFLINE_PASS checks=23` | Save/unknown-species safety. |
| `MfmPrescriptionCore/run_prescription_core_offline.py` | **PASS** — `MFM04_PRESCRIPTION_CORE_PASS checks=240 failed=0`; `MFM04_REPEAT_PASS` byte-identical | MFM-10 Sitka-only guard. |
| `MfmSaveProvenance/run_offline_check.py` | **PASS** — `MFM06_SAVE_OFFLINE_PASS checks=172`; `REFERENCE_FUTURE_V1_RESOURCES_UNCHANGED_VS_ac876a6` | Save/provenance and Reference bytes. |
| `ThreeSpecies/run_offline_check.py --suite timber` | **PASS** — `THREE_SPECIES_TIMBER_PASS checks=73` | Timber/economy arithmetic. |
| `--suite density` | **FAIL** — `THREE_SPECIES_OFFLINE_FAIL failures=1 checks=91` (`MaxRemovedBasalShare` scanner false positive); `COMPOSITE_DENSITY_FAIL checks=107 failures=3` (three pure-Beech R2 misses `0.423/0.407/0.417` vs unchanged `0.40`) | **Accepted known C-level limitation + known scanner false positive**; explicitly preserved, not relabelled or relaxed. |
| `--suite density-isolated` | **PASS** — `MIXED_DENSITY_ISOLATED_PASS compiledWithoutSitkaGrowthModel=true` | Arithmetic isolation. |
| `--suite density-sitka-isolated` | **PASS** — `SITKA_MODEL_ISOLATED_PASS compiledWithoutMixedDensityModel=true` | Arithmetic isolation. |
| `--suite history` | **FAIL (build)** — `ManagementHistoryText.cs: WorkItemDetail` not found | **Pre-existing verification-asset build failure**: the harness csproj omits `ScenarioManagementHistory.cs`, where `WorkItemDetail` is defined. Not a parity defect. |

## 4. Unity results

All runs on exact `3acdf9cb`, Legacy40 unless stated, under the held lock.

| Gate | Result | Key evidence |
|---|---|---|
| `BroadleafAdultGrowthVerification` Legacy40 | **PASS** — `BROADLEAF_GROWTH_VERIFY_PASS checks=30` | Adult constants, site scalar 1.0, shared canopy/light, 8 m Hegyi, protected Sitka comparator. |
| `BroadleafAdultGrowthVerification` Enlarged80 | **PASS** — `BROADLEAF_GROWTH_VERIFY_PASS checks=30` | Same assertions on Enlarged80. |
| `DensityQualification` `ANCHORS,SITKA100,SITKACOMP` | **PASS** — `DENSITY_QUAL_PASS geometry=L40 hardChecks=5` | Pure-Sitka model-2 exactly equals model-1 (`7E57B9DAEF5BF4D0` / `35E2BF1C2F55C2DE`); 24/24 controller victim sets, 356 victims; exact legacy route. |
| `ScenarioOneCompletionVerification` model 2, Legacy40 | **PASS** — `SCENARIO_ONE_COMPLETION_VERIFY_PASS`, 12 phase PASS records | Completed year 25; `9C77EB4BEE427FB9`; min/year-30 cash 613,978 cents; y30 living 199, canopy 0.879, regen 60 cells, deadwood 19.436 m³. An earlier Enlarged80 attempt failed the harness precondition `start is not 336 living Sitka` because it did not use the Legacy40 336-tree start; the Legacy40 rerun above is the qualification run. This is a harness precondition, not a defect. |
| `ScenarioReferenceVerification` Legacy40 | **PASS** — `REFERENCE_FUTURE_V1_PASS` | Archive milestones 0/20/50/100 exact (`A564039D9B7CE31D` / `F7DF7DAB53B6FD32` / `D5E75D6D21D631AC` / `7AD177B3CC2F73C7`); schedule `56C8B99FA1E8DDD1`; continuation `EBD49ADEF18C3F36`, v15 layout `BFFF66015A6619DA`; negative control detects tamper. |
| `SaveHardeningVerification` Legacy40 | **PASS** — `SAVE_HARDENING_VERIFY_PASS` | Save/load hardening; growth kept, trees=336, year=2. |
| `MixedDensityVerification` Legacy40 | **FAIL only at `POLICY_SAVE_VERSION_UNCHANGED saveVersion=24`** | All live mixed-density behaviour checks before that assertion passed (thresholds, pressure, over-crowding, deterministic save/load, model-1/model-2 anchors). **Stale v23 pin**, not a density regression. |
| `BroadleafTimberEconomyVerification` Legacy40 / Enlarged80 | **FAIL at the first check** `save version unchanged` (expects v23) | The gate aborts before its timber assertions. **Stale v23 pin**; the offline timber suite passes 73 checks and accepted PARITY-06 Unity evidence is on the integrated ancestry. |
| `Parity07Unity` rendered gate | **PASS** — `PARITY07_UNITY_HARNESS_PASS shots=36 passes=1031 failures=0` | Reused from the #199 independent review at exact `3acdf9cb`; estimate/copy/prompt/detail clearance and 1280×720 + 1920×1080 rendered states. |
| `guided-v2` | **11 identical pre-existing failures** | Proven at exact base and final during #199 review; stale v23 pins, Objectives overflow and teaching-sequence indices. Not introduced by PARITY-09; remains a separate residual before any later gate requiring guided-v2 green. |

## 5. Checklist evidence map

| Checklist area | Evidence | Status |
|---|---|---|
| Pure-Sitka operational definition; models 0/1 legacy; model-2 exact route; protected fingerprints | Accepted PARITY-03/04/06 evidence + new `DensityQualification` ANCHORS/SITKA100/SITKACOMP (`7E57B9DAEF5BF4D0` / `35E2BF1C2F55C2DE`, 24/24, 356 victims) + `BroadleafAdultGrowth` protected comparator | **PASS** |
| Mixed density Reference 1.0 / Beech 1.5 / Oak 2.0; Oak > Beech ordinal; species-neutral victims; near-pure continuity; no DEC/DMC/SDI/broadleaf cap | Accepted PARITY-03 + new `MixedDensityVerification` behaviour checks (all passed before the stale v23 pin) + offline density suite (accepted known misses preserved) + `MIXED_DENSITY_ISOLATED_PASS` | **PASS** (behaviour); stale pin recorded |
| Oak/Beech adult values and gameplay-calibration labels; Beech height-band accepted miss; asymptotic height; dormant site multiplier; shared canopy/light; 8 m Hegyi; no invented coefficients | New `BroadleafAdultGrowthVerification` 30/30 at Legacy40 and Enlarged80 + accepted PARITY-04 evidence + `EARLY_LIFE_CONFORMANCE_STATIC_PASS` | **PASS** |
| Oak distinct juvenile curves; Beech fallback-conservative; shared calibration labels; broadleaf stock abstraction; browse order Oak > Beech > Sitka; shared browse values; no persisted browse form | Accepted PARITY-05 evidence + `EARLY_LIFE_CONFORMANCE_STATIC_PASS checks=30` + accepted Unity evidence on ancestry | **PASS** |
| Sitka three-lift exact; Oak/Beech two lifts at 3.5/6.0 m; ≥3-year interval; 8% crown reduction/60% cap labels; no formative/fork state | Accepted PARITY-06 independent review + offline pruning evidence (stale save pin recorded) + offline timber suite | **PASS** (accepted evidence); Unity gate blocked by stale v23 pin |
| Oak five-rung and Beech two-rung ladders; no silent premium inheritance; size + pruning/crown quality; no fork/defect claim; Teagasc/selected-price/890-proxy labels; no dormant flat value; no cost multiplier; premium Oak not guaranteed | `THREE_SPECIES_TIMBER_PASS checks=73` + accepted PARITY-06 Unity evidence (66 assertions × 2 geometries) + `PARITY07_COPY PASS 89/89` | **PASS** |
| Player-facing truth and rendered states | `PARITY07_COPY PASS checks=89` + `PARITY07_UNITY_HARNESS_PASS shots=36 passes=1031 failures=0` at exact `3acdf9cb` | **PASS** |
| No parity-specific save field/schema bump; model-2 new-game gate; models 0/1 legacy; unknown species rejected before mutation; CanLoad/LoadData consistency; species-less sale records; Legacy40 exact; Reference bytes exact; no historical reinterpretation | `PARITY08_SPECIES_OFFLINE_PASS checks=23` + `MFM06_SAVE_OFFLINE_PASS checks=172` + `SAVE_HARDENING_VERIFY_PASS` + `REFERENCE_FUTURE_V1_PASS` + accepted PARITY-08 evidence | **PASS** |
| Protected anchors exact; broadleaf-containing movement measured/attributed; no silent rebase; G3 integrity | `DensityQualification` exact anchors; completion `9C77EB4BEE427FB9`; Reference archive/preview/continuation exact; accepted PARITY-06 G3 evidence; no anchor change in the #199 delta | **PASS** |
| Evidence vocabulary (DIRECT / TRANSFER / MODEL DERIVATION / ORDINAL / GAMEPLAY CALIBRATION / FALLBACK-CONSERVATIVE / UNSUPPORTED) | Accepted PARITY-02…06 evidence labels + `PARITY07_COPY` + this handoff's classifications; no unsupported value was promoted | **PASS** |
| MFM quantitative treatment Sitka-adult-only; Oak/Beech cannot enter Sitka-only rules | `MFM04_PRESCRIPTION_CORE_PASS checks=240 failed=0` + `MFM04_REPEAT_PASS` + MFM-10 Manager review carried to `3acdf9cb` | **PASS** |
| Completion and solvency | `ScenarioOneCompletionVerification` model 2: completed year 25, `9C77EB4BEE427FB9`, min/year-30 cash 613,978 cents, all objectives ok | **PASS** |
| Independent reviewer confirms spec conformance; G2/G3/G4 | **Pending Claude Sonnet 5.5 independent review**; this handoff does not self-accept | **PENDING REVIEW** |

## 6. Verification debt and residuals (not repaired inside qualification)

1. **Stale v23 save-version pins** in `check_integrated_v1.py` (`ForestSaveData.cs` normalized hash), `SiteInfrastructure` (`G_SAVE_VERSION_IS_STILL_23`), `run_pruning_check.py` (`protected bytes moved`), `MixedDensityVerification` (`POLICY_SAVE_VERSION_UNCHANGED`), and `BroadleafTimberEconomyVerification` (`CurrentVersion == 23`). The current accepted save version is v24 (D-066); the PARITY-08 evidence passes. These are verification-only repins for a later bounded task, not parity defects. They were not edited here.
2. **Three pure-Beech reduced-simulator R2 misses** (`0.423/0.407/0.417` vs unchanged `0.40`) remain explicit non-passes and the Manager-adjudicated accepted C-level limitation. The `MaxRemovedBasalShare` source-scanner hit remains a false positive; neither is relabelled.
3. **`--suite history` build failure** (`WorkItemDetail`) is a pre-existing verification-asset build issue, identical on the accepted base.
4. **guided-v2** remains 11 identical pre-existing failures; reconcile before any later gate that requires guided-v2 green.
5. **No Unity rerun of the PARITY-06 timber gate** was possible because it aborts on the stale v23 pin. The offline timber suite passes on the exact candidate, and the accepted PARITY-06 Unity evidence is on the integrated ancestry; the #199 delta is UI-only.

## 7. Cleanup and state

- U1 clean at `3acdf9cb`; only the known `.vscode/settings.json` Editor side effect occurred and was restored to HEAD.
- Machine-wide Unity lock **free**; no Editor/Xephyr/openbox.
- No staged harness `.cs`/`.meta` remains under `Assets/ForestPrototype/`.
- No push, no integration, no Windows build. #194 untouched.

## 8. Recommendation and exact next action

**Recommendation:** the evidence supports PARITY-09 conformance for independent review. The only red items are pre-existing verification-pin staleness and accepted known limitations, all explicitly preserved. The final `THREE-SPECIES PARITY P0 VERIFIED` token is **not self-issued**; Claude Sonnet 5.5 independent review and #200 Manager acceptance are required.

**Next action:** Claude Sonnet 5.5 reviews this handoff, the checklist evidence map, the stale-pin classifications and the raw evidence, then publishes `INDEPENDENT-REVIEW.md` with `APPROVED / NEEDS CHANGES / REJECTED`. Manager then decides whether the stale verification pins require a bounded repin before the final token or are accepted as verification debt.
