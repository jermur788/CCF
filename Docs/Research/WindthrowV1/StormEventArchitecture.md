# Storm event architecture (two layers)

**Status:** implementation-ready design proposal. All numbers are **[C] calibration placeholders** unless labelled otherwise.

```
Layer 1  STORM EVENT        did a storm happen this year? how severe? from which direction?
            │               (scenario hazard × deterministic roll)
            ▼
Layer 2  TREE VULNERABILITY which living trees fail, given this storm?
            │               (tree state × local context × storm intensity)
            ▼
         OUTCOMES           batched mortality → deadwood records → opening → visuals → review
```

The layers are separate on purpose:

- **Layer 1** carries everything the evidence cannot calibrate (frequency, severity). These are **scenario parameters [C]**.
- **Layer 2** carries what the evidence supports directionally (height, slenderness, recent opening, soil, dominance) [A/B shapes, C weights].
- **No annual per-tree probability exists outside a storm year.** In a calm year nothing is rolled per tree.

## 1. Storm model versioning

Follow the project's model-version pattern (`GrowthModel`, `RegenerationModel`, `SimulationRandom` models):

| `stormModel` | Meaning |
|---|---|
| 0 (Legacy / None) | No storms. All saves without the field, every save version below the storm version, **Reference Future v1**, and any explicit 0 |
| 1 (StormsV1) | This design |

New Scenario One games use 1 (**PRODUCT DECISION**: on by default, or opt-in "Storms" setting). Loads keep the saved model. No automatic migration. With 0, the annual step is **bit-identical to today**, so all existing anchors survive.

## 2. Layer 1 — storm event

### 2.1 Scenario hazard (definition data, not saved world state)

`ScenarioOneDefinition` additions (asset values; serialized definition, single-writer):

| Field | Meaning | v1 default [C] | Label |
|---|---|---|---|
| `stormAnnualProbability` | P(at least one damaging storm in a year) | 0.08 | [C] game prior; ATL suggests prototyping in 0–0.05 |
| `stormSeverityWeights` | relative weights of Moderate / Severe / Extreme | 0.70 / 0.25 / 0.05 | [C] |
| `stormSeverityIntensity` | intensity S per class (multiplies vulnerability) | 0.03 / 0.10 / 0.30 | [C], calibrated in Unity (`LongRunCalibrationPlan.md`) |
| `prevailingStormDirectionDeg` | mean direction storms come *from* | 225 (south-west) | [I] Irish prevailing south-westerlies; presentation and edge side only |
| `stormDirectionSpreadDeg` | ± spread | 45 | [C] |
| `siteWindHazard` | soil × regional exposure multiplier | 1.0 ("mineral, moderate") | **PRODUCT DECISION** (soil not modelled) |
| `sideSurroundings[4]` | N/E/S/W: sheltered / open | S open, others sheltered | **PRODUCT DECISION** (`BoundaryDecision.md`) |
| `stormGraceYears` | no storm before this year in a new game | 3 | [C] tutorial pacing: the player sees the stand before disturbance |

**Why 0.08 rather than ATL's 0–0.05?** One storm per ~12 years makes 1–3 storms likely before the Year-25 completion review. That is enough to be experienced, and few enough not to dominate. This is a **gameplay choice to be confirmed in calibration**, not evidence.

### 2.2 Event roll (deterministic)

At the start of `ForestEcologyController.AdvanceOneYear`, after `ecologicalYear++`, and only when `stormModel ≥ 1` and `year ≥ stormGraceYears`:

```
u      = SimulationRandom.Roll(rngModel, "STORM-OCCURS-v1", year, seed)
storm  = u < stormAnnualProbability
if storm:
    class     = pick(stormSeverityWeights, Roll(rngModel, "STORM-SEVERITY-v1", year, seed))
    direction = prevailing + spread × (2·Roll(rngModel, "STORM-DIRECTION-v1", year, seed) − 1)
```

- `Roll` hashes a string id. It **does not consume** the shared annual `System.Random`, so mast, survival and promotion draws are untouched (`Determinism` below).
- At most one damaging storm per year in v1. Multiple storms per year are undetectable at an annual step.

### 2.3 Debug / scripted storms

For tests and calibration, support `ForceStorm(year, class, direction)` as a harness/editor-only API that bypasses the occurrence roll but **uses the same Layer 2**. Never reachable from player UI.

## 3. Layer 2 — tree vulnerability (summary)

Full design: `TreeVulnerabilityDesign.md`.

```
V_i = Load(H_i) × Dominance(H_i / Htop) × Slender(H/D_i) × Openness(cell light) × Recent(opening near i)
      × Site(siteWindHazard) × Edge(open established side within reach, facing the storm)

p_i(storm) = 1 − exp(−S_class × V_i)                 (S from Layer 1)
victim_i   = Roll(rngModel, "WINDTHROW-v1-" + treeId, year, seed) < p_i
```

`1 − exp(−S·V)` keeps probabilities in [0, 1) and approximately linear for small S·V. `S` is the only quantity calibrated to stand-level damage bands (`LongRunCalibrationPlan.md`). `V` is a *relative* index; it is never shown to the player as a probability.

## 4. Annual step placement

```
ScenarioOneManager.AdvanceYear
  1  resolve approved management (batched)     ← thinning, salvage, planting…
  2  ecology.AdvanceOneYear():
       2.0  ecologicalYear++
       2.1  STORM (stormModel ≥ 1)              ← NEW: Layer 1 + Layer 2, batched outcomes
       2.2  competition … (unchanged sequence: canopy, growth, crowns, density mortality,
            regeneration, mast, seed, establishment, promotion, suitability, opening decay)
  3  juveniles, understorey, deadwood decay, snapshot, review   (manager, unchanged)
```

Consequences:

- A storm **after this year's thinning** sees the fresh opening (the "thinned right before a storm" case).
- Storm victims are removed **before** competition and growth, so survivors respond to the new space in the same annual step.
- The `RecentOpening` added by windthrow decays at step 12 like felling openings.
- Growth Model 1 density mortality runs after the storm in the same year, on the post-storm stand.

## 5. Event record (for history, review and visuals)

Store one compact record per storm (see `StormSaveAssessment.md`): `{year, class, directionDeg, victims, volumeM3}`. Victims themselves are already identifiable: `mortalityCause == "windthrow"`, `mortalityYear == year`.

## 6. What is deliberately absent in v1

- No wind-flow or topography simulation.
- No gusts or partial crown damage. No leaning trees.
- No storm damage to regeneration cohorts or planted juveniles. Young growth is below canopy and largely sheltered [I]. Falling trees could damage it, but that needs a footprint model (v2).
- No secondary damage cascade within a storm (no dominoes). Exposure created by this storm matters from the next year via `RecentOpening`.
- No pests after windthrow (bark beetle). Out of scope for Ireland v1.

## 7. Determinism (packet item 19)

**Contract:** the same save, ecological year, `stormModel`, `rngModelVersion` and `simulationSeed` must reproduce the same storm (occurrence, class, direction) and the same victims.

Rules consistent with the project's versioned RNG policy (`SimulationRandom`, D-046):

1. **Draws:** all storm draws use `SimulationRandom.Roll(rngModelVersion, id, year, simulationSeed)` with fixed, versioned string ids: `STORM-OCCURS-v1`, `STORM-SEVERITY-v1`, `STORM-DIRECTION-v1`, `WINDTHROW-v1-<treeId>`. `Roll` hashes the id, so draws are **order-independent** and do **not** consume the shared annual `System.Random`. Existing mast, survival and promotion sequences are unchanged.
2. **No new RNG model:** use whatever RNG model the save already records (model 1 for new games). No new RNG model is introduced.
3. **Stable inputs:** victim evaluation reads a deterministic list (`FindTrees()` sorted by tree id) and values computed **before** any victim is removed (two-pass: compute all V_i, then roll, then apply). The victim set never depends on removal order.
4. **Versioning:** any change to Layer 1/2 formulas or ids after release requires `stormModel = 2` (new ids, e.g. `…-v2`). Old saves keep their model, the same policy as the growth and regeneration models.
5. **Isolation from other new ecology:** Sol's Regeneration Model 2 / understorey work changes what grows *after* a storm, never which trees the storm selects in that year. Storm evaluation reads only tree state, cell light and `RecentOpening`, all of which exist at storm time.
6. **Reference Future v1:** archive and previews are model 0, so no storms (D-042). No change to the archive or its hashes.
7. **Tests:** two-process repeat (same victims, same world hash); save at Year N−1 and load → identical storm in Year N; `stormModel 0` reproduces all current anchors; changing iteration order (shuffled tree list in a fixture) gives the same victims.
