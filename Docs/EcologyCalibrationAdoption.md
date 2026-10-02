# Ecology calibration adoption (C8 growth/competition + k10a10 canopy/light)

| Item | Value |
|---|---|
| Branch | `task/ecology-calibration-adoption` |
| Base | `1319c2ce8d10ee4e19d2fff8c05e92b9c81c75a1` (shared juvenile ecology and tree mortality foundation) |
| Context | `cd239d7889c8224dcd46591deefa3089249e94c9` |
| Engine | Unity 6000.6.0f1 |

Calibration evidence, not merged here:
- `task/sitka-growth-competition-calibration` @ `4b721ff` (`Docs/SitkaGrowthCompetitionCalibration.md`);
- `task/canopy-light-calibration` @ `c648d0b` (`Docs/CanopyLightCalibration.md`).

## Production changes

| Parameter | Was | Now | Authoritative location |
|---|---:|---:|---|
| Hegyi competition cutoff | 20 m | **8 m** | `ForestEcologyController.HegyiCutoffMeters` (also used by the marking manager's growth forecast) |
| Sitka Ci50 | 3 | **5** | `Species/SitkaSpruce.asset` (`ci50`) |
| Sitka potential DBH growth | 0.9 cm/yr | **1.2 cm/yr** | `Species/SitkaSpruce.asset` (`potentialDbhGrowthCmPerYear`) |
| Canopy shade reach | 1.5 × crown radius + 2.5 m | **1.0 × crown radius + 2.5 m** | New `ForestEcologyController.CanopyShadeReachPerCrownRadius`, used by `RecomputeCanopy` and the marking manager's light forecast (which duplicated the literal 1.5) |

Unchanged:
- peak opacity, still an implicit 1;
- the competition-response equation `1/(1+CI/Ci50)`;
- the light equation;
- every juvenile rule, threshold and height-growth value;
- promotion, mortality and the save schema (v14);
- RNG models;
- the generator's starting crowns and crown relaxation.

`TreeSpeciesDefinition.cs` field defaults (0.9 and 3) only seed new species assets; the Sitka asset is authoritative, so the `.cs` file is untouched.

The Hegyi cutoff is global. Beech and oak keep their own Ci50 of 3 and potential of 0.4 cm/yr. Their competition indices now come from an 8 m neighbourhood, so their adult growth is less suppressed than before. The beech and oak gates still pass; broadleaf recalibration was not part of the approval (see Decisions needed).

## Production results against the calibration (80 m bounded stand, production `AdvanceOneYear`)

`Tools/Verification/EcologyCalibrationAdoptionVerification.cs` uses the same generator recipe and treatments as the calibration reports.

| Measure | Calibration | Production |
|---|---:|---:|
| Unthinned DBH growth, interior / whole (cm/yr) | 0.3566 / 0.3877 | 0.3566 / 0.3877 |
| Q-tree 10-year increment response | 1.171 | 1.171 |
| Moderate release response | 1.558 | 1.558 |
| Heavy release response, whole / interior | 1.739 / 1.844 | 1.739 / 1.844 |
| Control light, years 0 / 10 | 0.011 / 0.043 | 0.0107 / 0.0430 |
| Q-tree light, year 10 | 0.061 | 0.0609 |
| Moderate light, years 5 / 10 | 0.148 / 0.163 | 0.1480 / 0.1625 |
| Heavy light, years 0 / 5 / 10 | 0.210 / 0.254 / 0.257 | 0.2103 / 0.2544 / 0.2573 |
| 40 m whole ÷ 80 m interior, year-1 growth | about 1.10–1.13 (was 1.41) | 1.130 |
| Recruits / automatic deaths in 10 years | — | 0 / 0 |

The light ordering holds at years 0, 5 and 10: control < Q-tree < moderate < heavy. Production equals the shadow-model calibration because neither the juvenile/mortality foundation nor the annual step changes adult growth or canopy inputs.

## New calibrated lifecycle anchor

The canonical 80-year lifecycle (`ForestStandScenarios.LifecycleFixture`, RNG model 0) has a new expected hash:

- **New hash: `BFC55473C1506067`.** It was `7E39B70A14959FAD` before the calibration.
- **Determinism:** identical in four independent Unity processes:
  - `JuvenileMortalityFoundationVerification.BeginCanonical`, run twice;
  - `ScenarioOneInteractionVerification`;
  - `CCFIntegrationVerificationTemp`, which also runs two in-process lifecycles A and B.
- **Recruits:** 19 per run (was 30).

## Reference Future v1 (frozen, unchanged)

- `ScenarioOneReferenceFutureV1.bytes` and `ScenarioOneReferenceScheduleV1.json` are byte-identical to BASE, with the same git blob IDs.
- `ScenarioReferenceArchive.Load()` still verifies every milestone against its original embedded JSON (`verifiedFrozenWorld`).
- The stored Year-100 hash is `7AD177B3CC2F73C7`.
- The v12 archive loads, previews at Years 20, 50 and 100, and continues from Year 50 to Year 100 under current code.

**What changed by design.** Before this task, two gates also asserted that *current code* reproduces the frozen biology:
- the per-tree and per-cell Year-100 equality of the legacy v12 continuation in `ScenarioOneInteractionVerification`;
- milestone equality in `ScenarioReferenceVerification.Begin`.

Under the approved calibration that is impossible without preserving the old model. The interaction gate now:
- still requires loadability, a current-schema Year 100, Year-50 tree identity and position, the P0601 historical exception, and the stored archive hashes;
- reports the biology divergence (`SCENARIO_ONE_LEGACY_REFERENCE_DIVERGENCE`) instead of asserting equality.

Measured divergence at Year 100: 517 frozen trees against 494 in the calibrated continuation. 200 trees are identical, mostly trees felled before Year 50; 224 diverged, with a maximum DBH difference of 24.2 cm; and 93 frozen recruit IDs never recruit under calibrated light and growth.

`ScenarioReferenceVerification.Begin` (regenerate and compare) and `ReportFrozen` were not adapted:
- `Begin` would now fail its milestone comparison by design.
- `ReportFrozen` recomputes `WorldHash` through the current save classes and fails at Year 0 ("Frozen milestone year 0 is incompatible"). That check depends only on the save schema, not on ecology. The archive loader's own comment records that re-serialising v12 worlds with newer schemas yields a different hash, so `ReportFrozen` is stale since v13, independent of this task. This is inferred from code, not re-run at BASE.

Reference Future v2 is not created.

## Verification-tool adaptations (no production effect)

| File | Change | Why |
|---|---|---|
| `CCFIntegrationVerificationTemp.cs` | Expected lifecycle hash `BFC55473C1506067`; recruit counts must match between runs A and B instead of equalling 30 | Calibrated lifecycle |
| `JuvenileMortalityFoundationVerification.cs` | Expected canonical hash; mortality-fixture neighbour at 6 m instead of 12 m | 12 m is outside the 8 m cutoff, so the fixture lost the competition it tests. 6 m is inside the cutoff and outside the neighbour's 4.5 m shade reach, so the death still opens the focal cell. |
| `Assets/…/ScenarioOneInteractionVerification.cs` | Expected canonical hash; legacy-replay biology equality becomes a report; stored-hash archive check | See Reference Future section |
| `Assets/…/ScenarioHabitatPresentationVerification.cs` | Year-0 grass is allowed only when some cell exceeds the 0.40-light grass proxy | Under k10a10 the scene's road and work-clearing cells reach up to 0.84 light at Year 0 (3 of 64 cells at 0.40 or more), so the understorey proxy correctly yields grass there. Measured: 4 patches in those 3 cells. |
| `EcologyCalibrationAdoptionVerification.cs` (new) | Production C8/k10a10 gate | — |

## Not changed (deferred by the packet)

- Starting-crown recalibration;
- crown expansion and re-closure speed;
- crown-scaled neighbourhood;
- release lag;
- juvenile height growth;
- competition labels;
- adult suppression, storm or windthrow mortality;
- deadwood disposition.

The existing "crowded" label (CI ≥ 3) and the `StandDiagnostics` CI > 6 band are unchanged. Under the 8 m cutoff, unthinned Sitka CI is about 5–12, so most trees still read "crowded". No existing UI assumption broke.

## Reproduce

Copy one harness at a time into `Assets/ForestPrototype/`, run `-executeMethod <Class>.Begin` in Unity 6000.6.0f1 batchmode, then remove the copy and its `.meta`. The scene-resident Interaction and Habitat harnesses run in place.

Runs used an isolated `XDG_CONFIG_HOME` per run, linking only `unity3d/Unity` for the licence, so the player's real save was never touched.
