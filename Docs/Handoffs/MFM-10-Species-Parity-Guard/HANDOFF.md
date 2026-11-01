# MFM-10 Local Species-Parity Residual Qualification — Handoff

**Claim:** #213  
**Parent:** #91 / browser Manager #200 / local orchestration #203  
**Browser prerequisite:** #212 — `Docs/Handoffs/MFM-10-Species-Parity-Guard/BROWSER-HANDOFF-20261101.md` at `a3372ce5f0c8c2e7fdbff61a74d395c7fff9f2fa`  
**Exact production candidate:** `6784ab17cac922a16815f4898a30737482101018`  
**Branch:** `verify/mfm-10-species-guard-v1`  
**Worktree:** `/media/jer/Files/CCF/Operations/CCF-MFM10Residual-20261101`  
**Status:** **LOCAL RESIDUAL EXECUTED — GATE NOT EXIT-0 — `SPECIES GUARD QUALIFIED` WITHHELD PENDING #200 DISPOSITION**  
**Unity:** not used and not required by the executed matrix.  
**No production file changed. No test, threshold, baseline or fixture was changed.**

## 1. Result in one paragraph

The MFM prescription-core residual suite **passed on the exact candidate** (`MFM04_PRESCRIPTION_CORE_PASS checks=240 failed=0`, deterministic repeat). The ThreeSpecies density residual suite **did not exit 0**: it reproduced two documented, pre-existing failure families that are not new MFM-10 species-guard defects — (a) one verification-scanner false positive on the MFM-04 identifier `MaxRemovedBasalShare`, and (b) the three Manager-adjudicated **accepted known C-level limitation** pure-Beech reduced-simulator R2 misses (`0.423 / 0.407 / 0.417` against the unchanged `0.40` threshold). The MFM-10 species-guard assertions and the Oak/Beech/mixed parity route assertions themselves passed. Because #213's published PASS condition requires both commands to exit 0, the final token is **not** issued unilaterally; the exact evidence is returned to #200 for disposition.

## 2. Preflight — OBSERVED FACT

- Exact HEAD verified before execution: `6784ab17cac922a16815f4898a30737482101018`.
- Branch: `verify/mfm-10-species-guard-v1`; worktree clean before and after the runs (`git status --porcelain` empty).
- `origin/integration/scenario-one-player-test-ready-v1` verified at the same exact candidate.
- No Unity Editor process was running; the machine-wide Unity lock was **not** acquired and remained `free`.
- Storage at execution: ZX20 `18,832,949,248` bytes free (`17.54 GiB`, above the 15.00 GiB floor); Files `61,352,005,632` bytes free (`57.1 GiB`).
- Toolchain: `dotnet 10.0.114`; `Python 3.12.3`. The runners target `net8.0` and completed normally.
- The offline worktree was placed on Files (NonUnity-class lane) because the matrix is deterministic offline-only and ZX20 was close to its storage floor. No Unity Library was created.

## 3. Command 1 — MFM prescription core — PASS

```bash
python3 -B Tools/Verification/MfmPrescriptionCore/run_prescription_core_offline.py \
  --out /media/jer/Files/CCF/Verification/MFM10-SpeciesGuard-20261101/01-prescription-core
```

**Exit code:** `0`  
**OBSERVED FACT:** `MFM04_PRESCRIPTION_CORE_PASS checks=240 failed=0`  
**OBSERVED FACT:** `MFM04_REPEAT_PASS two processes, byte-identical output: True`  
**OBSERVED FACT:** `MFM04_DIGEST coverage=b50810a729c77c05 candidates=23b4eadf7e0f7228 release=a1577f2a1b137970 fit=98ce8d428bacb4f1 calibration=8591fcfc8f163e8f`

Species-guard assertions that passed include:

- `PASS species: a non-Sitka adult in the sample invalidates coverage [The sample holds a non-Sitka adult (P0605); V1 quantitative rules are Sitka-only.]`
- `PASS release: a broadleaf Crop Tree gets no Sitka-calibrated release`
- `PASS derive: a non-Sitka sample tree -> SampleHasNonSitka`
- `PASS release: not-covered, missing, sample and felled Crop Trees are reported and get no proposals`

The runner also validated the frozen MFM-03 fixture SHA-256 values before compiling. No production source was edited; the runner compiles the production pure MFM sources unchanged.

**Receipts:** `01-prescription-core/run1.log`, `run2.log` (identical SHA-256 `7d4fbe83f4d9da2de77fc2b2a9b519bcccc39fe51d20bdad8e785f8f003ed7cf`).

## 4. Command 2 — ThreeSpecies density residual — NOT EXIT-0

```bash
python3 -B Tools/Verification/ThreeSpecies/OfflineLogic/run_offline_check.py --suite density \
  --out /media/jer/Files/CCF/Verification/MFM10-SpeciesGuard-20261101/02-three-species-density
```

**Exit code:** `1`  
**OBSERVED FACT:** `THREE_SPECIES_OFFLINE_FAIL failures=1 checks=91`  
**OBSERVED FACT:** `COMPOSITE_DENSITY_FAIL checks=107 failures=3`  
**OBSERVED FACT:** `MIXED_DENSITY_ISOLATED_PASS compiledWithoutSitkaGrowthModel=true occupancy=2.1085 victims=26`  
**OBSERVED FACT:** `SITKA_MODEL_ISOLATED_PASS compiledWithoutMixedDensityModel=true relativeDensity=0.811598 pressure=0.528996`

A second identical run (`02-three-species-density-rerun`) reproduced exactly the same four failing checks and the same pass tokens. Runner log SHA-256 `fa792e2447dd0577b2ab1ee700a5432a113fb4cefca5d078d965a7a0621bc840` for both runs.

### 4.1 Failure family A — verification-scanner false positive (not a species maximum)

`FAIL NO_PRODUCTION_IDENTIFIER_OR_STRING_EXPRESSES_A_SPECIES_MAXIMUM_DENSITY_OR_MAX_BASAL_AREA`

Reported identifiers:

- `ScenarioOneTeachingCap.cs: identifier MaxRemovedBasalShare`
- `ScenarioOneRackBay.cs: identifier MaxRemovedBasalShare`
- `ScenarioOnePrescription.cs: identifier MaxRemovedBasalShare`

**OBSERVED FACT:** the scanner flags any identifier containing `max`/`maximum` plus `basal`/`SDI` outside `SitkaGrowthModel.cs` (`Tools/Verification/ThreeSpecies/OfflineLogic/MixedDensityOfflineCheck.cs`, the `words.Any(...)` predicate).  
**OBSERVED FACT:** `MaxRemovedBasalShare` is the MFM prescription **removal safeguard** (maximum share of basal area that a prescription may remove), not a species maximum density, maximum basal area or SDI.  
**OBSERVED FACT:** the same failure is already recorded as pre-existing on this lineage in `Docs/Handoffs/S1-C-Implementation-05/HANDOFF.md` and `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-05/HANDOFF.md` (the latter records it on `ScenarioOneRackBay.cs` and states it is pre-existing at base).  
**ORCHESTRATOR CLAIM:** this is a stale verification-asset heuristic, not a production species-cap violation. #213 has no authority to rewrite the verification asset; the browser handoff explicitly requires stopping and reporting before changing it.

### 4.2 Failure family B — accepted known C-level limitation (pure Beech R2)

`FAIL LONG_RUN_L40_B_BEECH_DENSE_MEETS_R1_R4 R1=True R2=False R3=True R4=True maxYearLoss26=0.423 ...`  
`FAIL LONG_RUN_L40_F_BEECH_YOUNG_MEETS_R1_R4 R1=True R2=False R3=True R4=True maxYearLoss26=0.407 ...`  
`FAIL LONG_RUN_E80_F_BEECH_YOUNG_MEETS_R1_R4 R1=True R2=False R3=True R4=True maxYearLoss26=0.417 ...`

**OBSERVED FACT:** these are exactly the three misses documented in `Docs/Verification/ThreeSpeciesProgramme/CompositeDensityKnownMisses.md` and `Docs/Verification/ScenarioOneVerificationDebtRepair.md`, and adjudicated in `Docs/Verification/ScenarioOneTier3Preflight/StaticPreflightFindings.md` as **ACCEPTED KNOWN C-LEVEL LIMITATION**. The criterion remains `0.40`; the values remain non-passes in evidence and were not waived or rebaselined.  
**OBSERVED FACT:** the same failure count (`checks=107 failures=3`) is recorded at the accepted preflight candidate and in the S1-C/S1-ThreeSpeciesParity handoffs.  
**ORCHESTRATOR CLAIM:** these are pre-existing reduced-simulator pure-Beech calibration misses, not an MFM-10 species-guard regression. They still prevent a strict exit-0 claim.

### 4.3 What did pass

All MFM-10-relevant parity and refusal assertions passed, including:

- `PASS ROUTE_PURE_SITKA_IS_LEGACY_SITKA_EXACT`
- `PASS ROUTE_ANY_OAK_OR_BEECH_IS_SPECIES_AWARE_OCCUPANCY trials=400 speciesAware=400`
- `PASS LEGACY_SITKA_ARITHMETIC_UNCHANGED_FROM_BASE_C1564B8 1a81640dcd11cac8`
- `PASS MIXED_DENSITY_MODEL_SOURCE_HAS_NO_SITKA_REFERENCE`
- `PASS SITKA_ROUTE_AND_SITKA_MODEL_HAVE_NO_REFERENCE_TO_OCCUPANCY_MODEL`
- `PASS PURE_OAK_CAN_DIE_FROM_DENSITY_PRESSURE victims=28/500`
- `PASS PURE_BEECH_CAN_DIE_FROM_DENSITY_PRESSURE victims=37/600`
- `PASS MIXED_STAND_LOSES_STEMS_OF_ALL_THREE_CLASSES`
- `PASS MIXED_THREE_SPECIES_REPLAY_IDENTICAL`
- `PASS HAZARD_HAS_NO_SPECIES_PREFERENCE_AT_EQUAL_COMPETITION_AND_SIZE`
- `PASS DOCUMENTATION_STATES_NO_SPECIES_CAPACITY`
- `PASS MIXED_DENSITY_ISOLATED_PASS` and `PASS SITKA_MODEL_ISOLATED_PASS`

## 5. MFM-10 requirement disposition

| Requirement | Result on exact candidate |
|---|---|
| 10a — proposal disabled/safely limited for unsupported species | **SATISFIED by command 1** — non-Sitka coverage/sample refusal, `SampleHasNonSitka`, broadleaf Crop Tree gets no release |
| 10b — no Sitka-only coefficient silently applied to Oak/Beech | **SATISFIED by command 1 + command 2** — Sitka-only MFM guards pass; Oak/Beech density remains on the species-aware occupancy route; legacy Sitka arithmetic fingerprint unchanged |
| 10c — mixed-species tests prove accepted ordinal density while refusing unsupported calibration | **SATISFIED in substance** — all Oak/Beech/mixed route/refusal assertions pass; the suite's overall exit is red only because of the two documented pre-existing families above |
| Unity/runtime requirement | **None found** — no runtime-only species branch; no contradictory evidence invalidated the browser static boundary |

## 6. Disposition required from #200

#213's published PASS condition requires both commands to exit 0. They did not. The token is therefore withheld. Two bounded dispositions are available:

1. **Accept the combined evidence as MFM-10-sufficient.** Treat the three Beech R2 misses as the already-adjudicated accepted C-level limitation and the `MaxRemovedBasalShare` scanner hit as a known stale verification-asset false positive, then issue `SPECIES GUARD QUALIFIED` on the basis of the passing MFM-10 species-guard and parity assertions. This requires an explicit Manager statement that the two red families are out of MFM-10 scope.
2. **Release a separate bounded repair claim.** Narrow the ThreeSpecies source scanner so MFM removal safeguards such as `MaxRemovedBasalShare` are not treated as species maximum-density identifiers, and keep the three Beech R2 items as the accepted known limitation. #213 did not do this because it has no repair authority and the browser handoff requires stopping before changing a verification asset.

No third option should silently relax the `0.40` threshold, delete a failing fixture, or rebaseline the suite.

## 7. Evidence and cleanup

- Evidence root: `/media/jer/Files/CCF/Verification/MFM10-SpeciesGuard-20261101/`
  - `01-prescription-core/run1.log`, `run2.log`
  - `02-three-species-density/runner.log`
  - `02-three-species-density-rerun/runner.log`
- Worktree clean at handoff; branch contains this docs handoff only on top of `6784ab17`.
- No production, verification, fixture, threshold or baseline change.
- No Unity/Editor, no machine-wide Unity lock, no Windows build.
- Research Source Consolidation **#194 was not inspected or touched**.
- No integration and no self-release of PARITY-09, MFM-11, MFM-12 or Tier-3.

## 8. Exact next action

#200 reviews this handoff and either (a) explicitly accepts the two documented pre-existing red families as outside MFM-10 scope and issues the token, or (b) releases a bounded verification-asset repair for the scanner false positive. Until then, MFM-10 remains **NOT QUALIFIED** and downstream PARITY-09/MFM-11 remain blocked.
