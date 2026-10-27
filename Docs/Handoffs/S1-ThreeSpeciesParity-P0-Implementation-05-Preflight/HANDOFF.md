# PARITY-05-PREFLIGHT — Early-Life / Browse Parameter Mapping Audit — Handoff

**Task:** `PARITY-05-PREFLIGHT` — early-life / browse parameter mapping audit
**Spec:** `specs/scenario-one-three-species-parity-p0/` — APPROVED
**Claim:** GitHub issue #31 (`jermur788/CCF`)
**Date:** 2026-10-27
**Worker / routing:** OpenCode harness, model `opencode-go/deepseek-v4.1-flash`, provider OpenCode Go, in T3 Code. This is **not** the packet's routed Claude subscription 2 / strongest-Claude model. The actual executor is recorded here; no claim of the preferred routing is made.
**Starting main HEAD:** `5296404030f16176f23c31fc2ca3759d4d036f1d`
**Inspected main tree:** `5296404030f16176f23c31fc2ca3759d4d036f1d`; the `main` ref later advanced to `d72beb877f2c517b240e415bb56e4b5ca2fe285c` only through the claim/coordination commits described in §1, with an empty net tree diff from the starting head.
**Worktree / branch:** `/media/jer/ZX20/CCF-Parity05`; publication branch `research/scenario-one-three-species-parity-p0-05-preflight-v1`, created from `d72beb8` (the coordination-only head whose tree is identical to the starting head). This is a read-only audit worktree, not production ownership.
**Unity:** **NOT RUN. Lock not acquired. No Editor, Player or build.**
**Production writes:** **none.** Only this handoff file is added. No production C#, species asset, scene, prefab, spec, decision-log, save-schema, verification-target or coordination edit is part of this task.

## Verdict

**`PARITY-05 MAPPING PREFLIGHT COMPLETE — READY FOR MANAGER DECISION`**

The mapping is complete, but it does **not** authorise a production patch. Three genuine authority/representation questions must be decided first (M1, M2, M3). This handoff does **not** claim `EARLY-LIFE SPECIES PARITY REQUALIFIED`; no Unity or runtime measurement was made.

### Headline findings

1. **The packet's numeric early-life table is not traceable in the committed PARITY-02 authority.** The values `0.014 / 0.62 / 0.93` (Sitka), `0.0035 / 0.28 / 0.88` (Oak) and `0.0025 / 0.24 / 0.86` (Beech) appear in the PARITY-05 dispatch packet, but not in the committed PARITY-02 handoff, the approved spec, the plan, the decision log or production code. The committed PARITY-02 Q-07 instead says: keep Oak's distinct survival/establishment curves, keep Beech on the combined legacy response as `FALLBACK-CONSERVATIVE`, and keep shared `0.35 m/yr`, `3.5 m` promotion, density parameters and mast probabilities. That is a **provenance/authority gap**, not a mapping that can be inferred from current code.
2. **The packet's browse susceptibility multipliers conflict with the committed PARITY-02 values.** The packet states `Sitka 1.0 / Beech 1.8 / Oak 2.2`; committed PARITY-02 Q-09 states palatability `Oak 1.0 > Beech 0.5 > Sitka 0.15`. These are different numbers and different relative orderings (`1 : 1.8 : 2.2` versus `1 : 3.33 : 6.67` when normalised to Sitka). Current production uses the committed `0.15 / 0.5 / 1.0` values. A new multiplier cannot be selected without the Manager resolving which authority is current.
3. **The accepted `0.2` protected-browse multiplier is absent and contradicted.** Current protection is a **binary access barrier**: an intact fence or effective shelter returns `protectionAccess = 0`; breached/outside returns `1`. The committed Browsing & Protection v1 policy explicitly says fencing is an access barrier, not a percentage reduction, and its gate asserts that fenced outcomes equal the browse-free baseline exactly. The only `0.2` in current authority is Scenario One's **background browse pressure**, not a protection multiplier. Treating them as the same thing would contradict accepted behaviour.
4. **Most qualitative species policy conforms.** Oak's distinct survival/establishment curves are active; Beech uses the combined fallback response; the regeneration substrate is shared/species-neutral; no broadleaf age-phase table is active; no browse-form state is persisted. The main label gap is that Beech's combined response is not explicitly labelled `FALLBACK-CONSERVATIVE` on the production documentation surface.
5. **One adjacent nonconformance exists but is outside this slice.** Unknown saved tree species currently fall back to the default species (Sitka) in `ForestSaveController`, and unknown regeneration species are skipped in `RestoreCellState`. FR-10 requires rejection before world mutation. That belongs to PARITY-08, not to a PARITY-05 early-life/browse patch.
6. **No live writer overlap was found** for the files a PARITY-05 production patch would touch. MFM-02 currently holds the machine-wide Unity lock and owns unrelated Rack-bay files; RACK-V2-04, S1-C-IMPL-03 and S1-D-T01 own UI/read-only surfaces, not the species/juvenile/browse files. Production ownership still must be granted explicitly.

---

## 1. Source identity, method and scope

### Identity

| Item | Value |
|---|---|
| Packet read | `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-05-Preflight/TASK-PACKET.md` at `5296404` |
| Starting / inspected main | `5296404030f16176f23c31fc2ca3759d4d036f1d` |
| Claim | GitHub issue #31, marked `[RUNNING]` before the substantive audit; actual model/provider/worktree/start head recorded there |
| Latest canonical-set commit | `17aa738c58b474476ce9c6d1f6163d42c63ae5d5` (PARITY-04 docs/verification patch); the packet named no context commit |
| Worktree | `/media/jer/ZX20/CCF-Parity05` (clean, read-only; no Unity `Library`) |
| Publication branch | `research/scenario-one-three-species-parity-p0-05-preflight-v1` |
| Unity | NOT RUN; lock not acquired |
| Production diff | NONE |

The `main` ref moved to `510bbc0` and then `d72beb8` during the task because of a transient coordination edit and its revert. `git diff --stat 5296404 d72beb8` is empty; the production and context bytes audited here are exactly the starting-head bytes. `Docs/Project/coordination/active-tasks.md` is byte-identical to the starting head in the final tree. Issue #31, not `active-tasks.md`, is the claim record for this dispatch. Claim timing note: the RUNNING state was recorded before the substantive audit; the first repository read began before the issue title update, and no production write or Unity action occurred at any point.

### Authority read, in order

- `Docs/Project/decision-log.md` (D-023, D-044, D-047, D-049, D-059, D-061–D-065), `Docs/Project/current-milestone.md`, `Docs/Project/game-brief.md`, `Docs/Project/unity-project-overview.md`, `Docs/Project/AgentWorkflow.md`.
- `specs/scenario-one-three-species-parity-p0/spec.md`, `plan.md`, `tasks.md`, `checklists/requirements.md`.
- `Docs/Handoffs/S1-ThreeSpeciesParity-P0-EvidenceClosure/HANDOFF.md` and `MANAGER-REVIEW.md`.
- PARITY-03 static and Unity Manager reviews; PARITY-04 preflight, patch and Unity Manager reviews.
- `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-03/MANAGER-REVIEW.md`, `...-03-Unity/MANAGER-REVIEW.md`, `...-04-Preflight/MANAGER-REVIEW.md`, `...-04/MANAGER-REVIEW.md`, `...-04-Unity/MANAGER-REVIEW.md`.
- PARITY-01 audit handoff (read from `origin/audit/scenario-one-three-species-parity-p0-current-main`; it is not present on current `main`) and the spec-authoring handoff (read from `origin/spec/scenario-one-three-species-parity-p0-v1`).
- Live `Docs/Project/coordination/active-tasks.md` at the starting head (read only).

### Production files inspected

- `Assets/ForestPrototype/TreeSpeciesDefinition.cs`
- `Assets/ForestPrototype/JuvenileEcologyRules.cs`
- `Assets/ForestPrototype/ForestEcologyController.cs`
- `Assets/ForestPrototype/BrowsingConditions.cs`
- `Assets/ForestPrototype/ForestEcologyCell.cs`
- `Assets/ForestPrototype/RegenerationModel.cs`
- `Assets/ForestPrototype/SpeciesConfigurationPolicy.cs`
- `Assets/ForestPrototype/ForestTreeSpawner.cs`
- `Assets/ForestPrototype/ForestSaveData.cs`
- `Assets/ForestPrototype/ForestSaveController.cs`
- `Assets/ForestPrototype/ForestSaveValidation.cs`
- `Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.cs`
- `Assets/ForestPrototype/ScenarioOne/ScenarioOneDefinition.cs`
- `Assets/ForestPrototype/Species/SitkaSpruce.asset`
- `Assets/ForestPrototype/Species/SessileOak.asset`
- `Assets/ForestPrototype/Species/BeechSpecies.asset`
- `Assets/Scenes/ForestTest.unity` (species/spawner bindings by GUID only)

### Verification and documentation inspected

- `Tools/Verification/BrowsingProtectionVerification.cs`
- `Tools/Verification/JuvenileMortalityFoundationVerification.cs`
- `Tools/Verification/RegenerationBudget/RegenerationModelVerification.cs`
- `Tools/Verification/ThreeSpecies/MixedDensityVerification.cs` and the `ThreeSpecies` qualification layout
- `Docs/BrowsingProtectionV1.md`
- `Docs/BeechSpeciesParameters.md`
- `Assets/ForestPrototype/Docs/SessileOakV1.md`
- `Docs/Project/unity-project-overview.md` (current early-life/browse/save sections)

### What was and was not executed

**Executed:** `git show` / `git grep` / `git diff` / `git log`; direct file reads at `5296404`; stdlib-Python arithmetic on the read constants and response anchors (no repository writes). The arithmetic is diagnostic, not a Unity run.

**Not executed:** Unity import/compile, Play Mode, any `.cs` harness, any .NET offline suite, save load/save, Player build, Windows smoke, or any production edit. No Unity lock was requested or acquired.

---

## 2. M1 — Early-life parameter representation

### 2.1 Exact current production path

The annual sequence is `ForestEcologyController.AdvanceOneYear()` (`ForestEcologyController.cs:458–499`): competition, canopy, adult growth, crown relaxation, density mortality, `GrowExistingRegeneration()` (step 6), mast, `ComputeSeedRain()` (step 8), `EstablishNewCohorts()` (step 9), `PromoteCohorts()` (step 10), establishment-suitability update, recent-opening decay. Scenario One's exact planted juveniles are advanced separately by `ScenarioOneManager.AdvancePlantedJuveniles()` immediately after `ecology.AdvanceOneYear()` (`ScenarioOneManager.cs:1337`, method at `:1963–2035`). Both use `JuvenileEcologyRules` as the shared biological rule layer.

| Accepted Q07 quantity | Current production fields / values | Exact execution path | Representation status |
|---|---|---|---|
| **Seed arrival / seed saturation** | `supportsRegeneration` (all three `1`); `naturalSeedDispersal` (established species default to wind behaviour); `maturityOnsetYears` / `maturityFullYears` (Sitka 20/30; Oak/Beech 40/60); `seedPotentialPerMatureTree` (all 1); `seedDispersalScaleM` / `seedDispersalCutoffM` (Sitka 20/150; Oak 4/80; Beech 6/50); mast probabilities `0.15 / 0.35` and multipliers (Sitka 1.5/1/0.4; Oak 1/0.3/0.075; Beech 1/0.4/0.1); `seedSaturationS50` (Sitka 10; Oak/Beech 5). | `ComputeSpeciesSeedRain` (`ForestEcologyController.cs:1340–1379`) builds per-tree potential from maturity × `clamp(crownRadius/4)` × seed potential × mast, then exponential dispersal. `UpdateAllMastStates` (`:1267–1306`) applies species-isolated mast. `seedFactor = 1 − exp(−seedRain / S50)` (`:1472` model 0; `:1512` model 1). | **A** for the current seed/dispersal/mast/S50 policy. There is no `establishmentRatePerYear` field. |
| **Establishment response** | `JuvenileEstablishmentResponse(light)` (`TreeSpeciesDefinition.cs:286–292`): Oak uses distinct `establishmentResponseLight/Factor` (`SessileOak.asset:88–99`); Sitka and Beech use the growth `lightResponseLight/Factor` (`TreeSpeciesDefinition.cs:267–271`). `cell.EstablishmentSuitability` is a species-neutral disturbance scalar (`ForestEcologyController.cs:1779–1786`). | `EstablishNewCohorts` (`:1454–1496`) and `EstablishNewBands` (`:1502–1521`) compute `establishment = seedFactor × lightResponse × cell.EstablishmentSuitability`; values ≤ 0.01 are skipped. | **A** for the current light/disturbance response. There is no `establishmentSurvival` field and no separate species establishment-survival step. |
| **Establishment density admitted** | `regenDensityPerEstablishment = 0.5` and `regenDensityMax = 1.5` for all three; `RegenerationModel.RepresentationThreshold = 0.01` (`RegenerationModel.cs:22`); model 1 uses up to four represented bands plus one sub-threshold accumulator per species + origin + cell (`ForestEcologyCell.MaxBandsPerSpeciesOrigin = 4`). | Model 0: `cell.AddDensityWithSharedCapacity` (`ForestEcologyCell.cs:214–231`) clamps to the species maximum and shared other-species occupancy. Model 1: `AdmitRecruitment` (`ForestEcologyController.cs:1539–1574`) calls `cell.AdmitDensity` (`ForestEcologyCell.cs:171–181`), which admits only free shared occupancy and never shrinks existing stock. | **A** for the current relative-abundance admission. The accepted annual rate is not a current parameter; converting it to requested abundance is not defined. |
| **Annual juvenile survival** | `JuvenileEcologyRules.SurvivalResponse` (`JuvenileEcologyRules.cs:14–20`): Oak uses `DistinctJuvenileSurvivalResponse` (asset anchors 0.2/0.3/0.55/0.75/0.9/1/1/1 at light 0/0.01/0.05/0.1/0.15/0.2/0.3/1); Sitka and Beech use the combined fallback: `lightResponse < regenPoorLightThreshold (0.15) ? 1 − regenMortalityUnderPoorLight (0.2) : 1`. Browsing then multiplies survival by `(1 − browsedFraction × 0.04)`. | `GrowExistingRegeneration` (`ForestEcologyController.cs:1412–1419`) for cohorts; `AdvancePlantedJuveniles` (`ScenarioOneManager.cs:2013–2028`) for exact individuals. | **A** for the current light/browse survival rules. There is no base `juvenileAnnualSurvival`; above the poor-light threshold, Sitka/Beech survival is exactly 1.0, not 0.93/0.86. |
| **Juvenile height growth** | `regenHeightGrowthMPerYear = 0.35` all three; `regenInitialHeightM` Sitka/Beech 0.15, Oak 0.18; growth = `0.35 × JuvenileLightResponse(light) × cell.SiteProductivity`; browsing removes 90 % of the increment for the browsed fraction. | `JuvenileEcologyRules.GrowHeight` (`:22–26`, browse overload `:96–107`); `GrowExistingRegeneration` and `AdvancePlantedJuveniles`. | **A** for the shared calibration, with Oak's different light curve and initial height. |
| **Promotion thresholds** | `promotionHeightM = 3.5` all; `promotionMinimumLight` Oak 0.2, Sitka/Beech 0; `PromotionDbhCm = clamp(height × 1.5, 2, 20)`; model 1 promotes only the oldest qualifying represented band (sub-threshold accumulators never promote); exact planted juveniles promote individually. | `JuvenileEcologyRules.CanPromote` (`:33–34`); `PromoteSpeciesCohorts` (`ForestEcologyController.cs:1699–1761`); exact path `ScenarioOneManager.cs:2028–2035`. | **A** for the current promotion policy. |
| **Regeneration support / species registration** | `supportsRegeneration = 1` all three. `ForestTreeSpawner.KnownSpecies` includes only `IsSimulationReady` species (`ForestTreeSpawner.cs:44–66`). `ForestTest.unity` binds Sitka as `defaultSpecies` and Oak/Beech through two `speciesVisualSets` (scene `:5808`, `:5838` onward; asset GUIDs match `SitkaSpruce.asset`, `SessileOak.asset`, `BeechSpecies.asset`). `RegenerationSpecies` merges the spawner's known species with the default and sorts by species id (`ForestEcologyController.cs:1247–1260`). | `RegenerationSpecies`, `ComputeSeedRain`, `GrowExistingRegeneration`. | **A** for Oak/Beech registration and regeneration support. The established three bypass the new-species declaration policy (`SpeciesConfigurationPolicy.cs:197`); their serialized assets are the authority. |

### 2.2 Are the accepted Q07 simple values directly represented?

**No.** The accepted quantities are not fields or operations in the current architecture. The table below separates the qualitative policy (which is represented) from the numeric table (which is not).

| Accepted Q07 item | Sitka | Oak | Beech | Mapping class | Why |
|---|---|---|---|---|---|
| `establishmentRatePerYear` | 0.014 | 0.0035 | 0.0025 | **C** (new parameter/operation) | Current establishment is a deterministic relative-abundance increment: `(1 − e^(−seedRain/S50)) × light response × suitability × 0.5`, subject to shared capacity. There is no per-year rate or probability field. No unique derivation exists: the same annual value can be produced by infinitely many seed-rain/light combinations, and the current units are relative occupancy, not stems or a probability. |
| `establishmentSurvival` | 0.62 | 0.28 | 0.24 | **C** | There is no species establishment-survival factor. `cell.EstablishmentSuitability` is a shared disturbance scalar (default 1.0), and `JuvenileEstablishmentResponse` is a light response, not a survival step. Multiplying either by these constants would be a new operation and would not be a defensible derivation from the committed evidence. |
| `juvenileAnnualSurvival` | 0.93 | 0.88 | 0.86 | **C** (for Oak/Beech); **C/D** for Sitka | Current survival is light-dependent (Oak distinct curve; Sitka/Beech 1.0 above the 0.15 light-response threshold and 0.8 below), then browse-modified. There is no constant base annual survival. Adding one is a new operation. For Sitka, the value would also change the protected pure-Sitka lifecycle anchors, which the spec forbids without explicit authority; that makes it obsolete as a numeric target (**D**) unless the Manager explicitly reopens the protected contract. |
| Oak distinct juvenile survival/establishment curves | — | Present | — | **A** | `useDistinctJuvenileLightResponses: 1` and the two distinct arrays are active. |
| Beech combined fallback response | — | — | Present as behaviour | **A** behaviour, **label gap** | No distinct flag; `SurvivalResponse` uses the combined threshold rule and `JuvenileEstablishmentResponse` falls back to the growth light curve. The exact class `FALLBACK-CONSERVATIVE` is not attached on the Beech production documentation surface. |
| Shared juvenile growth/promotion/density calibration | 0.35 m/yr, 3.5 m, 0.5/1.5 | Same | Same | **A** | Directly represented; the values are shared and should be labelled `GAMEPLAY CALIBRATION`, not species evidence. |
| Species dispersal, maturity, mast, S50 | Present | Present | Present | **A** | Directly represented on the assets; no simple-rate conversion needed. |
| Broadleaf stock abstraction v1 | — | Shared | Shared | **A** behaviour, **label gap** | `plantedJuvenileAgeYears = 3`, `plantedJuvenileHeightM = 0.6`, `plantedJuvenileDensity = 1` are one shared set; the header still says "Planting (Beech v1)" (`ForestEcologyController.cs:29–35`). |
| Species-neutral regeneration substrate | Shared cell capacity/occupancy | Same | Same | **A** | `ForestEcologyCell` capacity and `RegenDensityPerEstablishment/Max` are shared; species enter through seed/light/demography only. |
| No persisted browse-form state | — | — | — | **A** | `LastBrowsedFraction` / `LastBrowseAssessmentYear` are diagnostic-only and not saved; protection records (shelters/areas) are persisted, but not browse-form damage. |
| No broadleaf dormant age-phase runtime | — | Empty | Empty | **A** | No Scenario One asset serializes `growthAgePhases`; `HasGrowthAgePhases` is false and `GrowthAgeFactors` returns 1/1. Scenario Two assets also serialize empty arrays. |

### 2.3 Why the numeric table cannot be treated as a unit conversion

At `seedRain = S50`, `light = 1.0`, `suitability = 1.0`, no browsing, the current model requests:

| Species | `seedFactor` | establishment response | requested relative abundance | Above 0.01 representation threshold? |
|---|---:|---:|---:|---|
| Sitka | 0.6321 | 0.8 | **0.2528** | Yes |
| Oak | 0.6321 | 1.0 | **0.3161** | Yes |
| Beech | 0.6321 | 1.0 | **0.3161** | Yes |

If `0.014 / 0.0035 / 0.0025` were treated directly as requested annual abundance, Sitka would be just above the `0.01` representation threshold, while Oak and Beech would be **below** it; Oak/Beech recruitment would enter the sub-threshold accumulator and would not create represented bands until later accumulation. That is a different biological/representation meaning, not a rescaling. If the values were treated as probabilities, they would need a potential recruitment pool that the current model does not have. Either conversion would be an invented operation, not a derivation.

Current response shapes also do not collapse to one annual constant. At representative light levels (no browse, site 1.0):

| Species | light 0.05 growth / survival / establishment | light 0.20 growth / survival / establishment | light 0.55 growth / survival / establishment |
|---|---|---|---|
| Sitka | 0.050 / 0.800 / 0.050 | 0.300 / 1.000 / 0.300 | 0.900 / 1.000 / 0.900 |
| Oak | 0.020 / 0.550 / 0.950 | 0.300 / 1.000 / 1.000 | 1.000 / 1.000 / 1.000 |
| Beech | 0.150 / 1.000 / 0.150 | 0.650 / 1.000 / 0.650 | 1.000 / 1.000 / 1.000 |

Values are the interpolated current anchors; survival is the step/curve result, not an annual base rate. Sitka/Beech survival is a step (1.0 above the poor-light threshold, 0.8 below), so `0.93 / 0.86` are not current values at any light. Oak's `0.88` can be interpolated at one light level, but that is a point on a light-response curve, not a species-wide annual survival constant, and it does not license replacing the curve with a constant.

### 2.4 M1 conclusion

- **A** for the qualitative accepted policy: Oak distinct curves; Beech combined fallback; shared growth/promotion/density/dispersal/maturity/mast/S50; species-neutral substrate; no persisted form state; no dormant broadleaf age phases.
- **C** for every numeric Q07 quantity (`establishmentRatePerYear`, `establishmentSurvival`, `juvenileAnnualSurvival`) in all three species: the current architecture has no such fields and no defensible derivation.
- **D** for the numeric Sitka values if treated as implementation targets: the current Sitka calibration is stronger and the protected lifecycle anchors forbid a silent change. The accepted simple values can only remain ordinal/conservative policy evidence unless the Manager reopens the protected contract.
- **Do not implement** the numeric table in PARITY-05 without a Manager decision and a new explicit parameter design. The committed Q-07 policy is a label/curve-retention task, not a numeric replacement task.

---

## 3. M2 — Browse susceptibility mapping

### 3.1 Current formula and inputs

`JuvenileEcologyRules.AssessBrowse` (`JuvenileEcologyRules.cs:70–91`):

```text
effective = clamp01(backgroundPressure)
          × browsePalatability(species)          // serialized [0,1]
          × browseHeightVulnerability(species, height)
          × clamp01(vegetationExposure)          // 1 in v1
          × clamp01(protectionAccess)
p = min(BrowseProbabilityCap = 0.95, max(0, effective))
```

- `browseHeightVulnerability` (`:57–66`): 1 up to `browseFullVulnerabilityHeightM`, linear to 0 at `browseEscapeHeightM`. Sitka 0.5→0.8 m; Oak/Beech 1.2→1.8 m.
- `browsePalatability` is `[Range(0f, 1f)]` (`TreeSpeciesDefinition.cs:104`); asset values: Sitka `0.15`, Beech `0.5`, Oak `1.0`.
- `backgroundPressure` is the Scenario One `0.2` calibration (`ScenarioOneDefinition.cs:57`, applied at `ScenarioOneManager.cs:442`) or the controller default `0`.
- Browsing effects: 90 % of the year's height increment is removed when browsed (`BrowseHeightIncrementLoss = 0.9`), and survival is multiplied by `(1 − browsedFraction × 0.04)` (`BrowseExtraMortality = 0.04`) (`JuvenileEcologyRules.cs:47–53`, `:96–116`).
- Cohorts use the expected probability as `browsedFraction`; exact planted juveniles draw one deterministic annual event with `RealiseBrowse` (`:126–127`; `ScenarioOneManager.cs:1997–2011`).

### 3.2 Current versus accepted values

| Species | Current `browsePalatability` | Committed PARITY-02 Q-09 | Packet's accepted multiplier | Current relative ratio (normalised to Sitka) | Packet relative ratio |
|---|---:|---:|---:|---:|---:|
| Sitka | 0.15 | 0.15 | 1.0 | 1.0 | 1.0 |
| Beech | 0.50 | 0.50 | 1.8 | 3.33 | 1.8 |
| Oak | 1.00 | 1.00 | 2.2 | 6.67 | 2.2 |

The packet's `1.0 / 1.8 / 2.2` is a much flatter ordering than the committed `1 / 3.33 / 6.67`. It cannot be obtained by:
- multiplying the current palatability values by a common factor (ratios stay `1 : 3.33 : 6.67`);
- clamping a common scaling (ratios stay unless one value saturates);
- normalising the current values to `[0,1]` (they already are); or
- adding a separate multiplier on top of current palatability (the relative product becomes `1 : 6 : 14.67`, not `1 : 1.8 : 2.2`).

A defensible mapping requires a **single species susceptibility factor** whose relative values are `1.0 : 1.8 : 2.2`, plus a separately stated absolute scale. The current `browsePalatability` is already a 0–1 absolute scale and is not that factor unless it is replaced, or unless an explicit normalisation/anchor policy is adopted.

### 3.3 Arithmetic consequences at a height below full vulnerability

`p = background × species factor`, before height/exposure/access and the 0.95 cap. Current values are below.

| Background | Species | Current `p` | Direct multiplier as the factor | Max-normalised (`S/2.2`) | Anchored to current Sitka 0.15 (`0.15×S`) | Extra factor on current palatability (`pal×S`) |
|---|---:|---:|---:|---:|---:|---:|
| 0.2 | Sitka | 0.030 | 0.200 | 0.091 | 0.030 | 0.030 |
| 0.2 | Beech | 0.100 | 0.360 | 0.164 | 0.054 | 0.180 |
| 0.2 | Oak | 0.200 | 0.440 | 0.200 | 0.066 | 0.440 |
| 0.85 | Sitka | 0.1275 | 0.850 | 0.386 | 0.1275 | 0.1275 |
| 0.85 | Beech | 0.425 | 0.950 (cap) | 0.695 | 0.2295 | 0.765 |
| 0.85 | Oak | 0.850 | 0.950 (cap) | 0.850 | 0.2805 | 0.950 (cap) |

The **direct multiplier** keeps the packet's relative ratios but raises Sitka browsing 6.7× at `0.2` and saturates Oak/Beech at high pressure. **Max-normalisation** preserves the ratios and keeps the probability inside `[0,1]`, but changes all three absolute values and the pure-Sitka browse anchor. **Anchoring to current Sitka 0.15** preserves the pure-Sitka anchor exactly but **lowers** Oak/Beech browsing relative to the committed values (Oak `0.066` versus `0.200` at background `0.2`), which is likely contrary to the intended Oak > Beech > Sitka emphasis. **Extra factor on current palatability** double-counts species susceptibility and does not reproduce the accepted ratios.

### 3.4 M2 options (proposals only — no choice made)

| Option | Representation change | Behavioural consequence | Anchor / compatibility consequence | Minimal files |
|---|---|---|---|---|
| **M2-0 — Keep committed values, treat packet multipliers as ordinal only** | None | Current `0.15 / 0.5 / 1.0`; ordering Oak > Beech > Sitka preserved; no change to browse probabilities | All anchors unchanged; pure-Sitka browse 0.2 lifecycle remains exact | Docs/verification labels only; no C# or assets |
| **M2-1 — Replace palatability semantics with a non-normalised susceptibility factor** | Rename/re-scope the species factor to `1.0 / 1.8 / 2.2`; widen/remove the `[0,1]` range; formula uses the factor directly | Sitka browsing rises 6.7× at background 0.2; Oak/Beech rise and can cap at 0.95; much stronger browsing delay | Pure-Sitka browse 0.2 lifecycle anchor would move; broadleaf browse anchors move; old saves on models 0/1 would change unless gated | `TreeSpeciesDefinition.cs`, `JuvenileEcologyRules.cs`, 3 species assets, `BrowsingProtectionVerification.cs`, docs |
| **M2-2 — Normalise the accepted multipliers by a stated reference** | Choose a reference, record it, map `S` into `[0,1]` (e.g. `S/2.2` → `0.455 / 0.818 / 1.0`, or `0.15×S` → `0.15 / 0.27 / 0.33`) | Ratios exactly `1 : 1.8 : 2.2`; absolute scale depends on the reference. Max-normalisation raises Sitka/Beech; Sitka-anchor lowers Oak/Beech. | Max-normalisation changes pure-Sitka anchor; Sitka-anchor preserves pure-Sitka but lowers Oak/Beech. Both need anchor measurement and Manager acceptance. | `JuvenileEcologyRules.cs` (if formula unchanged), 3 assets, verification/docs; no new field if only asset values change |
| **M2-3 — Add a separate multiplier on top of current palatability** | New non-normalised field; `p = background × palatability × multiplier × …` | Relative product `1 : 6 : 14.67`, not the accepted `1 : 1.8 : 2.2`; double-counts susceptibility; Oak/Beech strongly increased | Pure-Sitka unchanged if Sitka multiplier 1.0; broadleaf browse anchors move strongly | `TreeSpeciesDefinition.cs`, `JuvenileEcologyRules.cs`, `SpeciesConfigurationPolicy.cs`, 3 assets, verification/docs |
| **M2-4 — Apply the multiplier only to broadleaf species** | New field or asset-only values; Sitka keeps current `0.15` | Preserves pure-Sitka anchor; broadleaf probability changes; ordering preserved | Old saves using Oak/Beech assets still change unless gated by growth model/save version; a species gate is not present in `AssessBrowse` | As M2-1/M2-3 plus a model/species gate at call sites |

### 3.5 M2 recommendation

**Do not implement a numeric browse change in PARITY-05 until the Manager resolves the authority conflict.** The committed PARITY-02 Q-09 is explicit: keep `Oak 1.0 > Beech 0.5 > Sitka 0.15`. The packet's `1.0 / 1.8 / 2.2` is not in that committed record. If the packet values are the current Manager decision, they must be recorded in the evidence closure/spec and the old Q-09 values marked superseded; then the smallest defensible representation is a **single species susceptibility factor with a stated absolute-scale policy** (M2-2), not a second multiplier on top of the existing palatability. If the committed Q-09 remains authoritative, use M2-0 and correct the packet.

---

## 4. M3 — Protection factor

### 4.1 Current production mechanism

- `BrowsingConditions.ProtectionAt` (`BrowsingConditions.cs:135–165`) returns `protectionAccess = 0` for an effective shelter or an intact fence containing the point; `1` otherwise. It reports expired/failed/breached states but does not partially reduce access for an intact barrier.
- `BrowsingConditions.CohortAccess` (`:170–192`) samples a 5 × 5 grid over a cell and returns the **unfenced fraction**; an intact fence covering the whole cell returns `0`. This is a geometric access fraction, not a 20 % protected multiplier.
- `JuvenileEcologyRules.AssessBrowse` multiplies by `protectionAccess`; `p = 0` when access is 0. The `BrowseAssessment.Reason` is `Protected` when access is 0.
- `Docs/BrowsingProtectionV1.md` states: fencing is an **access barrier, binary** (`:55`); intact fence access `0`, breached `1`, effective shelter `0` (`:105–107`); fenced outcomes equal the browse-free baseline exactly (`:226`).
- `Tools/Verification/BrowsingProtectionVerification.cs` asserts intact fence access `0` (`:255`), fence-before-install `1` (`:254`), and fenced outcomes equal the baseline (`:409–413`).
- Scenario One's only `0.2` is `backgroundBrowsePressure` (`ScenarioOneDefinition.cs:57`), applied as stand-level pressure (`ScenarioOneManager.cs:442`). It is not a protection factor.

### 4.2 Status of the accepted `0.2` protected multiplier

**Absent and contradicted.** There is no species-specific or shared `0.2` protection multiplier in production. The current protection policy is binary exclusion, which is the opposite of a 20 % residual exposure. The accepted `0.2` may be a misreading of the Scenario One background pressure; the committed authority gives no protected multiplier.

### 4.3 M3 options

| Option | Change | Behaviour | Consequence |
|---|---|---|---|
| **M3-0 — Keep binary protection** | None; clarify wording | Intact fence/shelter gives `p = 0`; breached/expired gives normal pressure | Preserves Browsing & Protection v1, its gate and all browse anchors; the `0.2` multiplier remains unimplemented and must be marked as such |
| **M3-1 — Replace intact access with `0.2`** | `ProtectionAt`/`CohortAccess` return `0.2` for intact fence/effective shelter | Protected juveniles still receive 20 % of unprotected browse pressure; fenced outcomes no longer equal the browse-free baseline | Existing verification fails; accepted binary-barrier policy is contradicted; scenarios with no production protection are unaffected in normal play, but fixtures and any saved shelters/fences change; a product decision is required |
| **M3-2 — Add a new protection type with `0.2` access** | Keep fences/shelters binary; add a new record/config for partial protection | New mechanic; current fences/shelters unchanged | New scope, new UI/economy/save design; not a PARITY-05 label task |
| **M3-3 — Treat `0.2` as background pressure** | No code change; correct the packet/evidence wording | No behavioural change | Likely correct reading of current authority; preserves no-fixed-unit-count policy (the current lists have no cap) |

### 4.4 M3 recommendation

Use **M3-0/M3-3** unless the Manager explicitly intends a new protection design. The committed Browsing & Protection v1 policy and its gate are evidence that binary access is accepted behaviour. A `0.2` protected multiplier is a Browsing & Protection v2 product change, not a PARITY-05 early-life/browse requalification. Do not add a fixed fence/shelter count; the current protection model has no fixed unit count, and nothing in this audit justifies one.

---

## 5. M4 — Existing species-policy conformance

| Requirement | Verdict | Verified facts and gaps |
|---|---|---|
| Oak distinct juvenile response | **CONFORMS** | `SessileOak.asset:69–99` sets `useDistinctJuvenileLightResponses: 1`, distinct survival and establishment arrays; `promotionMinimumLight: 0.2` (`:102`). `JuvenileEcologyRules.SurvivalResponse` selects the distinct path (`:16–17`). |
| Beech combined/fallback response | **CONFORMS behaviour; LABEL GAP** | Beech has no distinct flag; `SurvivalResponse` uses the combined `regenPoorLightThreshold`/`regenMortalityUnderPoorLight` rule (`:18–19`), and `JuvenileEstablishmentResponse` falls back to the growth light curve (`TreeSpeciesDefinition.cs:288–289`). `Assets/ForestPrototype/Docs/SessileOakV1.md:9` calls it the "legacy combined response path", but `Docs/BeechSpeciesParameters.md` does not attach the authoritative `FALLBACK-CONSERVATIVE` class to the Beech juvenile row, and no runtime comment uses that class. The checklist item is therefore not fully met at the production documentation surface. |
| Sitka baseline | **CONFORMS current bytes** | Sitka keeps the shared combined survival/establishment response, `0.35 m/yr`, `3.5 m`, `0.15` initial height, `S50 = 10`, and its protected browse values. The packet's simple Sitka values would change this baseline; they are not represented and would conflict with protected lifecycle anchors. |
| Species-neutral regeneration substrate | **CONFORMS** | `ForestEcologyCell` shared capacity/occupancy (`:171–231`) and `RegenDensityPerEstablishment/Max = 0.5/1.5` are shared by all three; species differences enter through seed, light, initial height and promotion-light values, not through a separate substrate. `RegenDensityMax` is technically a species field, but all three serialize `1.5`; a future per-species divergence would need a conscious decision. |
| No broadleaf dormant age-phase runtime | **CONFORMS** | No Scenario One asset serializes `growthAgePhases`; `HasGrowthAgePhases` is false; `GrowAdults` skips the age-phase hook when absent (`ForestEcologyController.cs:1024–1025`). Scenario Two assets also serialize `growthAgePhases: []`. |
| No persisted browse-form state | **CONFORMS** | `LastBrowsedFraction` and `LastBrowseAssessmentYear` are runtime diagnostics only (`ForestEcologyCell.cs:20–21`; `ForestEcologyController.cs:1397–1398`), not in `ForestSaveData`. `shelters`/`protectedAreas` are persisted protection configuration (`ForestSaveData.cs:77–78`), not browse-form damage. No browse-history/form field exists. |
| Unknown species / configuration handling | **NONCONFORMANCE for FR-10, out of this slice** | `ForestSaveController.LoadData` resolves an unknown tree species to `spawner.DefaultSpecies` with a warning (`ForestSaveController.cs:389–395`); `ForestEcologyController.RestoreCellState` logs and skips unknown regeneration species (`:596–599`); `ForestSaveValidation.Validate` validates tree IDs/stages/finite values and band structure but has no known-species check (`ForestSaveValidation.cs:44–75`, `:100–135`). FR-10 requires rejection before world mutation. This is PARITY-08's surface, not a browse/early-life edit. |
| Species configuration policy | **CONFORMS for the current three; future-field caution** | Established Sitka/Oak/Beech bypass the new-species declaration rules (`SpeciesConfigurationPolicy.cs:197`); non-established species must declare browse/juvenile/seed areas and are excluded from `KnownSpecies` when not ready (`ForestTreeSpawner.cs:44–66`; `ResolveSpecies` returns null). If a new early-life field is added, established species need no-op defaults that preserve current behaviour, and the policy should validate the new field for non-established species. |

---

## 6. M5 — Exact future patch surface and ownership

### 6.1 Live ownership check at `5296404`

| Task | Status | Owned surface | Overlap with a PARITY-05 production patch? |
|---|---|---|---|
| MFM-02 | **RUNNING**, claim #22; holds machine-wide Unity lock | `ScenarioOneRackBay*`, narrow `LearningObjectivesView.Sequence.cs`, optional plain geometry helper, focused non-UI/MFM verification | **None** in files; **Unity lock conflict** if PARITY-05 tried to run Unity now |
| RACK-V2-04 | WAITING, claim #30 | Stand Map Plan Racks mode / suggested dashed rack layer/legend | None in the species/juvenile/browse production files; map UI is separate |
| S1-C-IMPL-03 | READY, claim #19 | `UI/AnnualReviewView.cs`; narrow S1-C history-query files | None unless browse wording changes the annual review browse line (`UI/AnnualReviewView.cs:244–247`); coordinate before editing that shared UI |
| S1-D-T01 | READY, claim #13 | Read-only repeated-intervention baseline | None |
| PARITY-05-PREFLIGHT | RUNNING during the audit, claim #31; HANDED-OFF at publication | Read-only audit | This task owns no production file |
| WORKFLOW-CAPABILITY-SYNC-01 / CONTEXT-REFRESH | DONE / QUEUED (context only) | `Tools/ProjectContext` outputs; no gameplay | None |

**Result:** no active writer overlap on `TreeSpeciesDefinition.cs`, `JuvenileEcologyRules.cs`, `ForestEcologyController.cs`, `BrowsingConditions.cs`, `ForestEcologyCell.cs`, `ScenarioOneManager.cs` or the three species assets. Species assets and the scene are still high-conflict/single-writer surfaces and need explicit production ownership. The machine-wide Unity lock is held by MFM-02; do not acquire it or run Unity in this task or a follow-up without queue/ownership resolution.

### 6.2 Viable mapping options and exact files

#### Surface A — Label/verification-only (recommended default if M1/M2/M3 remain unresolved)

**Production C#/assets:** none.
**Serialized assets/scenes:** none.
**Documentation:** `Docs/BeechSpeciesParameters.md` (attach `FALLBACK-CONSERVATIVE` to the Beech juvenile response; state the shared values and the broadleaf stock abstraction), `Assets/ForestPrototype/Docs/SessileOakV1.md` (Oak distinct-curve status; current shared values), `Docs/Project/unity-project-overview.md` (current early-life/browse facts and evidence labels), `Docs/BrowsingProtectionV1.md` (binary protection; clarify that Scenario One `0.2` is background pressure, not a protected multiplier).
**Verification:** `Tools/Verification/JuvenileMortalityFoundationVerification.cs` (assert the species-specific response selection and protected hashes remain), `Tools/Verification/BrowsingProtectionVerification.cs` (assert current palatability values and binary protection; do not change anchors), `Tools/Verification/RegenerationBudget/RegenerationModelVerification.cs` (assert the species-specific establishment/survival/growth path and shared values). Optionally a small new `Tools/Verification/ThreeSpecies/EarlyLifeParityVerification.cs` that reads assets and asserts the accepted matrix without changing production.
**Behaviour:** unchanged. **Anchors:** unchanged. **Manager decision:** mechanical once the Manager confirms the committed Q-07/Q-09 policy is the current one.

#### Surface B — Add explicit early-life parameters (M1 mapping class C)

**Serialized assets:** `Assets/ForestPrototype/Species/SitkaSpruce.asset`, `SessileOak.asset`, `BeechSpecies.asset` (only if values are set; Sitka is protected and should normally remain unchanged). Scene/prefab files need no edit for a new serialized field on an existing asset; Unity serializes the default on the existing assets. `Assets/Scenes/ForestTest.unity` / `MixedSpeciesTest.unity` only if species references or spawner fields change.
**C#:** `TreeSpeciesDefinition.cs` (new serialized fields, accessors, `ToConfigurationFacts`), `SpeciesConfigurationPolicy.cs` (new field states/validation; established-species no-op defaults), `JuvenileEcologyRules.cs` (apply establishment/survival factors), `ForestEcologyController.cs` (pass a growth-model/species gate through `EstablishNewCohorts`/`EstablishNewBands`/`GrowExistingRegeneration`), `ScenarioOne/ScenarioOneManager.cs` (pass the gate through `AdvancePlantedJuveniles`), possibly `ForestEcologyCell.cs`/`RegenerationModel.cs` if the representation or threshold changes.
**Save:** no parity-specific field is required, but old saves on growth models 0/1 must remain unchanged; the gate must be explicit. If the design cannot be gated without a new persisted field, stop and return to the Manager (FR-10/stop condition).
**Behaviour:** Oak/Beech early-life changes; broadleaf-containing model-2 anchors move; pure Sitka must stay exact. If Sitka values are applied, the protected pure-Sitka lifecycle/density anchors break; that is a stop condition unless the Manager explicitly reopens them.
**Verification:** the three files above plus `Tools/Verification/ThreeSpecies/` focused parity fixture; Tier 2 Unity only after MFM-02 releases the lock.
**Manager decision:** yes — product/calibration and anchor scope.

#### Surface C — Browse-susceptibility representation change (M2 option M2-1/M2-2/M2-3)

**Serialized assets:** the three species assets (values/field).
**C#:** `TreeSpeciesDefinition.cs` (new field or range/semantics), `JuvenileEcologyRules.cs` (`AssessBrowse`), `SpeciesConfigurationPolicy.cs` (validation/defaults), possibly `BrowsingConditions.cs` only if protection also changes; call sites in `ForestEcologyController.cs` / `ScenarioOneManager.cs` if a model/species gate is required.
**Save:** no new field; old-save compatibility requires a gate because species assets are shared by old saves.
**Behaviour:** browse probabilities change; pure Sitka may change unless the mapping anchors it; broadleaf regeneration/completion anchors move.
**Verification:** `BrowsingProtectionVerification.cs` (palatability/value assertions and matrix), `JuvenileMortalityFoundationVerification.cs` (canonical protected hashes), regeneration/model-2 gates.
**Manager decision:** yes — evidence/calibration and anchor policy.

#### Surface D — Protected `0.2` multiplier (M3 option M3-1)

**Serialized assets/scenes:** none unless a new config field is serialized; existing protection records are saved in the scenario save (`ScenarioOneSaveData.shelters/protectedAreas`).
**C#:** `BrowsingConditions.cs` (`ProtectionAt`/`CohortAccess` return value), possibly `ScenarioOne/ScenarioOneDefinition.cs` / `ScenarioOne.asset` if made configurable.
**Verification:** `BrowsingProtectionVerification.cs` (fenced baseline equality and access assertions) and any browse anchor fixture.
**Behaviour:** protected juveniles receive 20 % exposure; the accepted binary-barrier policy and its gate are contradicted; normal Scenario One play has no production fences/shelters, so the visible impact is mainly fixtures/planted shelters unless protection gameplay is added.
**Manager decision:** yes — product/ecology policy; this is a Browsing v2 change, not a PARITY-05 label closure.

### 6.3 Recommended next production packet surface

**Recommended:** a **bounded PARITY-05 label/verification patch (Surface A)** first, unless the Manager explicitly decides that the packet's numeric M1/M2 values supersede the committed evidence. Surface A is the only surface that satisfies the current spec, the committed PARITY-02 evidence, protected anchors and the "no invented precision" rule without a new product decision.

If the Manager decides the numeric values are authoritative, split the work:
1. **PARITY-05A — early-life parameter design/evidence correction** (M1): update the evidence closure/spec with the exact values and units, then implement one explicit parameter model with a growth-model/species gate and protected-Sitka exclusion; no browse/protection changes in the same packet.
2. **PARITY-05B — browse susceptibility mapping** (M2): one representation choice, one normalisation policy, one anchor story; do not combine with M1.
3. **PARITY-05C — protection policy** (M3) only if the Manager accepts a new Browsing & Protection policy; otherwise record `0.2` as not adopted.

Do not put M1, M2 and M3 into one production packet: their anchors and verification stories are different, and a combined patch would make attribution impossible.

---

## 7. Manager-decision matrix

| # | Current production fact | Accepted evidence/spec intent | Mismatch or conformance | Evidence label | Viable minimal mapping option(s) | Behavioural consequence | Recommendation | Mechanical or genuine decision? |
|---|---|---|---|---|---|---|---|---|
| 1 | No `establishmentRatePerYear`, `establishmentSurvival` or `juvenileAnnualSurvival` field. Annual establishment is a deterministic relative-abundance increment; annual survival is light/browse dependent. | Packet: early-life gameplay calibration values Sitka `0.014/0.62/0.93`, Oak `0.0035/0.28/0.88`, Beech `0.0025/0.24/0.86`; broadleaf not more favourable than Sitka; Beech at least as constrained as Oak; no empirical Irish annual regeneration-rate basis. | **Not represented (mapping class C).** The values are also not in committed PARITY-02 Q-07; only the packet contains them. | Packet says `GAMEPLAY CALIBRATION`; committed evidence has no exact numeric class for them, so treat as `UNSUPPORTED — NEEDS DECISION` until the Manager records them. | Keep curves + label (Surface A) or new explicit parameters (Surface B). No defensible indirect (task class B) derivation. | Surface A: no change. Surface B: Oak/Beech early-life changes; broadleaf anchors move; Sitka must be excluded or protected anchors break. | Do not infer a conversion; decide whether the numeric table is authoritative and, if so, authorise a new parameter design. | **Genuine product/calibration decision.** |
| 2 | Pure-Sitka lifecycle anchors `BFC55473C1506067` (neutral), `3485B6630C9EA448` (browse 0.2), `7E57B9DAEF5BF4D0`, `35E2BF1C2F55C2DE` are protected; current Sitka has stronger quantitative calibration. | FR-1/FR-11: exact pure-Sitka route and protected anchors; D-059 says comparable depth, not equal empirical precision. | **Conflict if the packet's Sitka values are applied.** Current Sitka is not the simple `0.014/0.62/0.93` model. | `GAMEPLAY CALIBRATION` for current Sitka; packet values `UNSUPPORTED — NEEDS DECISION` for Sitka implementation. | Keep current Sitka (Surface A); do not apply simple Sitka values. If applied, explicit protected-anchor reopening is required. | Applying Sitka values would silently rebaseline protected lifecycle fingerprints; that is a stop condition. | Keep Sitka exact; treat the packet's Sitka row as ordinal/conservative policy only. | **Mechanical from FR-1/FR-11** (keep Sitka), unless the Manager explicitly reopens the protected contract. |
| 3 | Oak distinct curves present; Beech combined fallback present; shared `0.35/3.5/0.5/1.5/0.2/0.15` values present. | Q-07: keep Oak curves, Beech combined `FALLBACK-CONSERVATIVE`, shared growth/promotion/density/dispersal/maturity/mast/S50; no invented Beech curve. | **Conforms** for behaviour; Beech class label is not attached on the main Beech doc surface. | `TRANSFER`/`ORDINAL` (shade tolerance); `GAMEPLAY CALIBRATION` (shared values); `FALLBACK-CONSERVATIVE` (Beech combined response). | Surface A (label/verification only) — no numeric change. | No change; documentation becomes accurate. | Implement the label/verification closure (Surface A). | **Mechanical** from Q-07/checklist. |
| 4 | Current browse factor `browsePalatability = 0.15 / 0.5 / 1.0`; asset field is `[0,1]`; `AssessBrowse` multiplies it by background/height/exposure/access. | Packet: accepted susceptibility multipliers Sitka `1.0`, Beech `1.8`, Oak `2.2`. Committed Q-09: keep `Oak 1.0 > Beech 0.5 > Sitka 0.15`. | **Direct authority conflict.** The packet's ratios (`1 : 1.8 : 2.2`) differ from the committed values (`1 : 3.33 : 6.67`); current code implements the committed values. | Committed Q-09: `TRANSFER` (ordering) + `GAMEPLAY CALIBRATION` (numbers). Packet multipliers: `UNSUPPORTED — NEEDS DECISION` until recorded. | M2-0 (keep current), M2-1 (replace factor), M2-2 (normalise by a stated reference), M2-3 (extra multiplier — not ratio-faithful), M2-4 (broadleaf-only/gated). | M2-1 raises Sitka browsing 6.7× at 0.2 and saturates high pressure; M2-2 max-normalisation raises all, Sitka-anchor lowers Oak/Beech; M2-3 yields `1 : 6 : 14.67`; M2-0 no change. | Resolve the authority conflict first. If packet values win, use one species factor with a stated normalisation; do not multiply with current palatability. If Q-09 wins, keep current values and correct the packet. | **Genuine evidence/calibration decision.** |
| 5 | Protection is binary: intact fence/shelter access 0, breached/outside 1; the gate asserts fenced outcomes equal the browse-free baseline. Scenario One background pressure is 0.2. | Packet: accepted protected-browse multiplier 0.2 for all three; exact numeric effect remains gameplay calibration. Committed Browsing v1: fence is an access barrier, not a percentage reduction. | **Absent/contradicted.** The only `0.2` in current authority is background pressure, not a protection factor. | Committed protection: `DIRECT`/`TRANSFER` (barrier) + `GAMEPLAY CALIBRATION` (pressure). Packet `0.2` protected multiplier: `UNSUPPORTED — NEEDS DECISION`. | M3-0 (keep binary), M3-1 (intact access 0.2), M3-2 (new protection type), M3-3 (treat 0.2 as background pressure). | M3-1 gives 20 % residual exposure and breaks the existing fenced-baseline gate; M3-0/3 preserve accepted behaviour. | Keep binary protection; correct the wording. A 0.2 protected multiplier needs a separate Browsing v2 decision. | **Mechanical to keep committed binary policy**; genuine product decision only if the Manager wants partial protection. |
| 6 | Beech fallback behaviour is present, but the exact class is not attached on the Beech production doc surface. | Q-07/checklist: explicitly label Beech combined response `FALLBACK-CONSERVATIVE`. | **Label gap**, behaviour conforms. | `FALLBACK-CONSERVATIVE`. | Documentation/verification label update only. | No behaviour change; checklist item becomes evidence-backed. | Fix the label in Surface A. | **Mechanical** from Q-07/checklist. |
| 7 | Planting stock is shared `3 years / 0.6 m / density 1`, header says "Planting (Beech v1)". | Q-08: relabel as broadleaf stock abstraction v1; no per-species stock values without evidence. | **Label gap**, behaviour conforms. | `FALLBACK-CONSERVATIVE` / `GAMEPLAY CALIBRATION`. | Comment/UI label only. | No behaviour change. | Fix the label in Surface A. | **Mechanical** from Q-08. |
| 8 | Unknown tree species fall back to default (Sitka) on load; unknown regeneration species are skipped; validation has no species-ID check. | FR-10: reject unknown saved species IDs before world mutation; do not silently resolve to Sitka. | **Nonconformance**, but not a browse/early-life mapping issue. | Compatibility policy / `UNSUPPORTED — NEEDS DECISION` only if a historical fixture requires loading an unknown species. | PARITY-08: add a pre-mutation known-species validation and rejection path. | Unknown saves would fail safely instead of silently becoming Sitka; historical fixtures need review. | Route to PARITY-08; do not fix inside PARITY-05. | **Mechanical from FR-10**, but belongs to PARITY-08. |
| 9 | No active writer owns the species/juvenile/browse files; MFM-02 holds the Unity lock; RACK-V2-04/S1-C own UI surfaces. | Packet: no production worktree ownership; preserve unrelated work; no Unity needed. | **Conforms** for this read-only task; production ownership still needs granting. | Implementation fact / coordination. | Surface A needs no Unity and no species-YAML writer; Surfaces B/C/D need a production writer and a later Unity gate. | Surface A can proceed as docs/verification only; Surfaces B/C/D must queue behind MFM-02's Unity lock and one-writer rules. | Keep PARITY-05 read-only until the Manager chooses; then grant a bounded production lane. | **Mechanical coordination decision.** |

---

## 8. What is established, what is assumed, what remains open

**Established by inspection at `5296404`:**

- Current early-life path, values, units and execution order are as described in §2.
- Current browse formula and values are as described in §3.
- Current protection is binary access; the only `0.2` is background pressure.
- Oak distinct curves, Beech combined fallback, shared substrate, no active broadleaf age phases, no persisted browse-form state.
- Unknown-species fallback is a real FR-10 gap outside this slice.
- No live writer overlap on the PARITY-05 production surface; MFM-02 holds the Unity lock.

**Assumed / not verified:**

- The packet's numeric Q07 table and browse multipliers are treated as Manager-supplied policy intent for mapping, not as committed evidence. Their provenance in Git is not established.
- No Unity runtime behaviour, save/load, or rendered result was measured. The arithmetic examples are static derivations from the read bytes.
- The historical PARITY-02 evidence closure was read at its published commit; the audit/spec handoffs are not on current `main` and were read from their branches.

**Open for the Manager:**

1. Which early-life representation is authoritative: current curves (Q-07) or the packet's simple numeric table?
2. If the numeric table is authoritative, does it apply to Sitka? If yes, the protected lifecycle contract must be explicitly reopened; if no, the Sitka row is not implemented and must be labelled as such.
3. Which browse values are authoritative: committed Q-09 `1.0/0.5/0.15` or packet `1.0/1.8/2.2`? If the latter, which absolute-scale/normalisation policy is accepted?
4. Is the protected `0.2` multiplier a real intended mechanic or a misreading of the Scenario One background pressure?
5. Is PARITY-05 the right packet for a Browsing & Protection policy change, or should that be a separate Browsing v2 task?

---

## 9. Recommended verification for the next production packet

No Unity was run here. The following are proposed gates for whichever production option the Manager authorises; they are not claimed as run.

- **Static/offline:** asset/field contract checks; exact response-anchor arithmetic; old-model gating proof; protected-Sitka byte/hash audit; no unknown-species mutation in the affected path.
- **Tier 2 Unity (only in an authorised Unity lane, after MFM-02 releases the lock):** pure-Sitka neutral and browse-0.2 lifecycle anchors; Legacy40 start; model-0/1 old-save compatibility; model-2 broadleaf regeneration/completion/solvency; browse matrix (species × light × pressure × protection) with the existing `BrowsingProtectionVerification` expectations or the Manager-approved replacement; save/load continuation for the affected path; no Windows build.
- **Anchor policy:** any broadleaf-containing model-2 movement must be measured under G3 and explicitly accepted; pure-Sitka/Legacy40/Sitka timber/Reference archive anchors remain exact. Do not rebaseline silently.
- **No Unity in PARITY-05-PREFLIGHT:** this task's read-only status and the MFM-02 lock both prohibit it.

---

## 10. Handoff metadata

| Item | Value |
|---|---|
| Exact inspected main tree | `5296404030f16176f23c31fc2ca3759d4d036f1d` |
| Publication branch | `research/scenario-one-three-species-parity-p0-05-preflight-v1` |
| Publication worktree | `/media/jer/ZX20/CCF-Parity05` |
| Actual executor | OpenCode harness / `opencode-go/deepseek-v4.1-flash` / OpenCode Go / T3 Code |
| Unity | NOT RUN; lock not acquired |
| Production edits | NONE |
| Claim | GitHub issue #31 (`[RUNNING]` during execution; renamed `[HANDED-OFF]` and closed at publication per the issue instruction) |
| `active-tasks.md` | Final tree byte-identical to `5296404`; a transient coordination edit was reverted before publication (net diff empty) |
| Next action | Manager decides M1/M2/M3; then dispatch a bounded PARITY-05 patch (recommended Surface A if the committed evidence remains authoritative) |

## 11. Exit

**`PARITY-05 MAPPING PREFLIGHT COMPLETE — READY FOR MANAGER DECISION`**
