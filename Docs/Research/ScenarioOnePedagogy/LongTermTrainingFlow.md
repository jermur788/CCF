# Long-term consequence mode for practice plots (Workstream C5)

**Status:** NEW PROPOSAL [INF], with a technical feasibility check [HARNESS]. **Does not implement or propose Reference Future v2.**

## 1. Player flow

```
Practice copy (MarteloscopeConcept.md form C)
  mark Plan A → immediate review
  → "See it in 5 years" / "… 20 years" / "… 50 years"
  → simulate in the practice copy → walk the result (HUD banner: PRACTICE — YEAR 20 OF PLAN A)
  → "Back to the marking" (restores the Year-0 copy) → Plan B → same horizons
  → comparison table (A vs B at each visited horizon)
  → "Return to my forest"
```

The player always returns to the **same starting copy**. Long-term views are never mixed into the real forest.

## 2. Recommended horizons

| Horizon | Why | Cost (observed) |
|---|---|---|
| **Reset** | marking comparisons | instant (restore) |
| **5 years** | Crop-Tree response is visible; the first regeneration exists | ~5 annual steps |
| **20 years** | the opening has closed again; regeneration has established; second-intervention timing becomes visible | ~20 steps; the harness ran six treatments × 20 years plus checks in a few minutes in batch on modest hardware |
| **50 years** | optional; shows structural divergence | ~50 steps. **Re-evaluate after Sol's growth/mortality integration**: without adult mortality, stocking keeps rising (T0 basal area ≈ 93 m²/ha at Year 20 [HARNESS]) |

## 3. What the long-term view shows (all authoritative)

From saved annual snapshots and live state: living trees, basal area, standing volume, Crop-Tree DBH and last-year increment, canopy, mean light, regenerating cells by species, deadwood, cash. Harness evidence for 20 years is in `RepeatedInterventionDesign.md` §2.

**Important honesty rule:** the practice copy's long-term result assumes **no further management**. The banner must say so ("No further work was done in this preview"). Otherwise the player may read a single-intervention future as the predicted outcome of a CCF strategy.

## 4. Technical dependencies

| Dependency | Status | Notes |
|---|---|---|
| Deterministic advance | **Verified** in this work: the same plan from the same restored copy gives the same Year-20 world hash in-process (`D8802246D311A2F8` twice) and across two processes (identical output SHA-256) [HARNESS] | RNG 1 / regeneration 1 |
| Restore to the copy | **Verified**, with two requirements found here: restore from a **serialized copy** (`JsonUtility.FromJson(json)`), and **let at least one frame pass** before enumerating or capturing. `CaptureData()` includes inactive, not-yet-destroyed trees; the first harness version's in-frame repeat produced different hashes because of this | A production practice mode must follow the same rule |
| Save isolation | Precedent: Reference preview blocks F5/F9 while active (`ForestSaveController` checks `ReferencePreviewActive`) | Needs an equivalent "practice active" flag |
| UI isolation | Precedent: Reference preview banner; Help disabled during preview | The practice banner must persist during walking |
| Learning/objective isolation | Reference preview does not record learning progress (verified `MENU_PREFERENCES_PASS referenceClean=true`) | Same rule for practice; the player's objectives must not change |
| Time cost | 20 years for one plan takes seconds in batch | A short progress indicator in UI is enough |
| Reference Future v1 | untouched | The practice mode does not read or write the archive |

## 5. What is out of scope

- A new reference archive, or any change to Reference Future v1 (D-042).
- Simulating second interventions automatically inside the preview (that would be prescribing a strategy).
- Windthrow, harvest damage or timber-quality outcomes (not simulated).

## 6. Tests

1. Same plan → same Year-5/20 summary in two processes.
2. After "Return to my forest", the world hash equals the pre-practice hash (pattern: `RESIDUAL_STAND_WORLD_RESTORED`).
3. F5 during practice does not write the save slot.
4. Learning progress and forest objectives are unchanged by practice.
5. The banner text appears in every practice screen and while walking.
