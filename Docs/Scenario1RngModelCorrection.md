# Scenario 1 — RNG model-1 default for new games

Branch `task/scenario-one-rng-model1-default`, from `main` @ `a123eec` (presentation baseline integrated). A compatibility correction under the user-approved policy. It changes no survival probability, calibration, save schema or Reference Future data.

| Kind of state | RNG model |
|---|---|
| New Scenario One game | **model 1**: the corrected, versioned random-domain behaviour |
| Save with `rngModelVersion` | the recorded model, restored exactly |
| Save without the field (pre-field saves) | **model 0**: legacy compatibility |
| Reference Future v1 (frozen v12 archive) | **model 0**: legacy compatibility, unchanged |

No existing save is migrated.

## Root cause

`SimulationRandom.Roll(model 0, id, year, seed)` hashes the id, then folds in the year and the seed with FNV steps. For a fixed individual, consecutive years therefore give strongly correlated values. Measured over 2,000 ids × 30 years, lag-1 r = **0.675**.

Exact planted juveniles realise survival (`Roll(id, year)`) and browsing (`Roll(id + "/browse", year)`) through this domain. An individual that survives one year is therefore very likely to survive the next. Natural cohorts use expected values (no per-individual roll) and are unaffected.

`SimulationRandom` already documented this bias. Model 1 (splitmix64 mixing, lag-1 r = −0.004) already existed and is saved per world. But the scene's `ForestEcologyController.rngModelVersion` default was 0, so every new game used model 0. The issue was found in the post-Scenario-1 readiness study (`task/post-scenario1-systems-readiness` @ `7f58618`, not merged).

## Contract traced

| Question | Answer (code) |
|---|---|
| Where does a new game get its model? | Before: the serialized `ForestEcologyController.rngModelVersion` default (0; the scene does not override it). Now: `ScenarioOneManager.Awake` sets `NewGameRngModel` (= 1) when a fresh session initialises a new scenario (`initialized: 0` in the scene). |
| Where does a loaded game get it? | `ForestSaveController.LoadData` → `RestoreEcologyState(year, seed, data.rngModelVersion)`. This overrides the new-game value. |
| What if the field is absent? | `JsonUtility` keeps the `ForestSaveData` field default **0**, which is legacy. Absent and explicit 0 both mean model 0, exactly as the policy requires, so no schema change is needed. |
| Reference Future v1? | The embedded archive has no `rngModelVersion` field, so previews load model 0. Leaving a preview restores the player's captured model. |
| Why not set the model in `InitializeNewScenario`? | `RestoreSaveData` calls it for v1–9 saves after the ecology state is restored, which would flip those legacy saves to model 1. It therefore stays model-neutral. |
| Tests that assume model 0 | Canonical lifecycle anchors. `JuvenileMortalityFoundationVerification` and `BrowsingProtectionVerification` already set model 0 explicitly. `ScenarioOneInteractionVerification` and `CCFIntegrationVerificationTemp` inherited the scene default and now pin model 0 for the canonical anchor. `RngModelVerification` tests both models. |

## Changes

| File | Change |
|---|---|
| `ScenarioOne/ScenarioOneManager.cs` | `NewGameRngModel = SimulationRandom.MixedModel`; set in `Awake` for a fresh new game only |
| `SimulationRandom.cs` | Header comment states the policy |
| `ScenarioOneInteractionVerification.cs` (scene-resident gate) | Canonical 80-year anchor pinned to model 0; session model restored |
| `Tools/Verification/CCFIntegrationVerificationTemp.cs` | Same pin for its lifecycle anchor |
| `Tools/Verification/ScenarioOneCompletionVerification.cs` | Logs the model. `CCF_RNG_MODEL=0` replays the legacy completion anchor. |
| `Tools/Verification/RngModelPolicyVerification.cs` (new) | Policy gate (below) |

## Verification — `RngModelPolicyVerification` (PASS)

| Test | Result |
|---|---|
| 1 New game default | model 1; the fresh capture records 1 |
| 2 Model-1 determinism | Two 80-year lifecycle runs identical (neutral and pressure 0.2) |
| 3 Year independence | lag-1 r: model 0 = 0.6748, model 1 = −0.0041 (2,000 ids × 30 years) |
| 4 Model-0 legacy | Neutral `BFC55473C1506067`, normal `3485B6630C9EA448`, both reproduced |
| 5 Model-1 save | load retains 1; recapture 1 |
| 6 Model-0 save | load retains 0; recapture 0 |
| 7 Save without field | loads as 0 (field stripped from a real capture; `{"version":13}` also 0) |
| 8 Reference Future v1 | Years 20/50/100 preview on model 0; the player's model 1 is restored on return |
| Continuation | Uninterrupted run = save/load continuation for model 0 (`CDC4DE8F8EF471E5`) and model 1 (`872094413305A082`): 12 planted oaks, Year 20 |

## Planted-juvenile diagnostic (production survival rule, 4,000 ids)

| Case | Annual p | Years | Expected | Model 0 | Model 1 |
|---|---:|---:|---:|---:|---:|
| Sitka, deep shade (light 0.005) | 0.80 | 18 | 0.018 | **0.378** (z = 171) | 0.017 (z = −0.5) |
| Oak, light 0.15 | 0.90 | 18 | 0.150 | **0.559** (z = 72) | 0.152 (z = 0.2) |
| Oak, light 0.05 | 0.55 | 6 | 0.028 | **0.343** (z = 122) | 0.027 (z = −0.2) |

Model 1 realises the survival probability the shared juvenile rule intends. No probability was changed.

## Anchors

| Anchor | Model 0 (legacy compatibility) | Model 1 (new-game default) |
|---|---|---|
| Neutral lifecycle (fixture, browse 0) | `BFC55473C1506067` (unchanged) | `2A0B8C32AC0DE113` |
| Normal lifecycle (fixture, browse 0.2) | `3485B6630C9EA448` (unchanged) | `506E8AF6D8514C6C` |
| Completion playthrough | `568922E1A6D73CDD` (via `CCF_RNG_MODEL=0`, reproduced twice) | `00479F18970F9926` (reproduced twice) |
| Reference Future v1 Year 100 | `7AD177B3CC2F73C7` (frozen, model 0) | n/a |
| Reference continuation (v12 → v15) | `9CDF21A541C5968D` (save has no field, so model 0) | n/a |

The model-1 anchors are new current-behaviour anchors **for review**. They differ from model 0 because the mast, promotion offsets, planted survival and browse draws come from the corrected domains.

## Regression

Unity 6000.6.0f1 batchmode, one isolated-config process per gate.

**Class B — new-game default (model 1), all PASS:**
- completion ×2 (`00479F18970F9926` both runs);
- interaction (canonical pinned to model 0: `BFC55473C1506067`; continuation `9CDF21A541C5968D`);
- browsing and protection;
- planting; pruning; removal; deadwood;
- save hardening;
- `RngModelVerification`;
- WorkEconomy (`05E5E664…62E1`);
- TimberYield (`88FAF806…C4A8`);
- economy integration;
- habitat presentation;
- Reference Future (integrity, negative control, previews, continuation, contract; Year 100 `7AD177B3CC2F73C7`);
- `CCFIntegrationVerificationTemp` (lifecycle A = B = `BFC55473C1506067`).

**Class A — legacy model-0 compatibility, all PASS:**
- canonical neutral `BFC55473C1506067` and normal `3485B6630C9EA448`;
- completion ×2 with `CCF_RNG_MODEL=0`: `568922E1A6D73CDD` (unchanged).

**Gameplay under model 1 (completion playthrough):**

| Measure | Model 0 | Model 1 |
|---|---|---|
| Outcome | Completed at Year 25 | Completed at Year 25 |
| Minimum cash | €5,897.70 | €5,897.70 (identical; economy is RNG-free) |
| Retained original canopy trees | 221 | 220 |
| Mean canopy | 0.893 | 0.888 |
| Regenerating cells | 47 | 48 |

The tested Scenario 1 plan remains viable and solvent. No survival, browsing or economic parameter was changed.
