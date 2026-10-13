# Phase 1 rework — species-aware density foundation

Supersedes the design in `Phase1-MixedDensity.md` ("What D-061 allows, and what this does" and "Architecture"). Authority: D-059 (three-species parity), D-061 (mixed-species density v1: ordinal species-aware growing-space demand). Manager packet "REWORK SPECIES-AWARE DENSITY FOUNDATION".

**Status: implemented, type-checked and checked offline. NOT RUN IN UNITY.** Nothing here is a Unity-verified fact. The previous branch compiled in real Unity 6000.6.0f1 with zero errors; this rework has not been compiled there.

## Why `568ba5c` was rejected

`MixedDensityModel` still ran Oak and Beech through the Sitka density arithmetic:

| | Where |
|---|---|
| Oak/Beech enter Sitka arithmetic | `Tally.RelativeDensity` fed each species' own stems/ha and Dq to `SitkaGrowthModel.RelativeDensity` (Comeau et al. 2010 British Sitka line, `N·(Dq/25)^2.063/1868`) and multiplied by the class weight; `Plan` called it again on every boundary step |
| Sitka maximum-density boundary reused | `SitkaGrowthModel.MaximumRelativeDensity` (RD 1, [B]) decided when the boundary loop stopped |
| Sitka mortality hazard reused | `DensityPressure` (onset 0.6), `Vulnerability` (S⁴) and `AnnualDeathProbability` (strength 0.08, cap 0.5), all fitted on Sitka fixtures (`AdultMortalityCalibration.md`) |
| Species decided who died | suppression used each species' own `Ci50` (Sitka 5, Oak/Beech 3), so at equal competition a broadleaf stem read as more suppressed and was hit harder and removed first |

Multiplying a Sitka relative density by *w* is algebraically dividing the Sitka SDImax by *w*: Oak became "SDImax 934" and Beech "1,245" on the Sitka slope. That is an invented broadleaf SDI and maximum-density equation, which D-061 forbids. The old test `WEIGHTED_DENSITY_SCALES_WITH_CLASS` asserted exactly this identity (Oak RD = 2 × Sitka RD to 1e-12).

## New architecture

```text
ForestEcologyController.ApplyAdultDensityMortality
   living adults (dead excluded)
   anyNonReference = growth model >= 2 && any living tree has a non-Reference class
   MixedDensityModel.SelectRoute(model2, anyNonReference)        [pure, one place]
     LegacySitkaExact        -> the original controller statements, byte-identical to base c1564b8
     SpeciesAwareOccupancy   -> ApplySpeciesAwareDensityMortality -> MixedDensityModel.Plan   [pure, no SitkaGrowthModel]
   both routes end in KillDensityVictims (one change batch; ApplyMortality("self-thinning", year))
   AdultMortalityAccount.Route records which route ran (diagnostic, never saved)
```

**Route A, `LegacySitkaExact`.** Every stand under growth models 0 and 1, and a model-2 stand whose living adults are all Reference class (Sitka). Nothing was rewritten, ported or copied: the 52-line decision block and `StandRelativeDensity` are the same bytes as at `c1564b8`, and `SitkaGrowthModel.cs` is not modified. Victim selection, mortality order, batching and the keyed random draws are therefore unchanged.

**Route B, `SpeciesAwareOccupancy`.** Any Oak or Beech under model 2.

```text
occupancy = sum over living stems of (class weight x stem basal area) / stand area / capacity
pressure  = max(0, (occupancy - onset) / (1 - onset))            0 at the onset, 1 at capacity, above 1 beyond it
loss      = strength x pressure^2                                expected fraction of the stand lost this year
```

- *When* the stand is overcrowded is the occupancy and pressure. *How much* is the expected loss. *Which* stems die is separate: each stem's loss probability is `min(cap, loss x S^4 / mean(S^4))`, where `S = CI / (CI + 5)` from the existing species-neutral Hegyi index, and the capacity boundary removes the most suppressed first (ties by ordinal tree id). Class never enters victim selection.
- Basal area (π/4·DBH²) is geometry, not a fitted exponent. The class weight is the only place species matter. Occupancy is a plain sum, so there is no DEC, DMC, overyielding or mixture bonus.
- It is **not** an SDI, not Reineke's rule, not the British Sitka relative density and not a biological maximum-density law. `MixedDensityModel.cs` does not reference `SitkaGrowthModel` and compiles without it.
- Oak and Beech are not immune: pure and mixed broadleaf stands overcrowd, self-thin and are held at capacity.

## Evidence status

| Item | Status |
|---|---|
| Route A (British Sitka line, slope −2.063, SDImax 1,868, hazard, maximum line) | **[B]** transferred British evidence plus **[C]** calibration, exactly as accepted for growth model 1. Unchanged. |
| Basal area as stem footprint | Geometry. Not empirical, not fitted. |
| Class ranks Sitka < Beech < Oak | **[ORD]** ordinal. Sitka vs broadleaf is the better-supported call; **Oak above Beech is the weakest-supported call**. |
| Class weights 1.0 / 1.5 / 2.0 | **[ORD]** rank, **[C]** magnitude, provisional (D-061). Not measured Oak/Beech data. |
| Capacity 90 m²/ha of Reference-equivalent basal area | **[C]**. No evidence exists for any absolute capacity for Oak or Beech. Chosen **only for continuity** with the accepted Sitka line (below). |
| Onset 0.6, strength 0.08, loss cap 0.5, vulnerability exponent 4, neutral Ci 5 | **[C]**. Same response shape and values as growth model 1 so Reference-weight stems behave alike either side of the route switch. Not fitted to Oak/Beech. |
| Hegyi competition index | Existing model, species-neutral by construction (diameters and distances only). |

No empirical Oak/Beech density datum is used anywhere in route B.

## Decisions for the Manager

1. **The capacity is anchored to the Sitka line for continuity.** Any D-061 implementation needs one absolute capacity number, and the only calibrated density reference in the repository is Sitka's. With 90 m²/ha, a stand of Reference-weight stems has occupancy 5% below to 10% above Sitka relative density at equal stems/ha for Dq 8–80 cm (measured: 0.947 to 1.095). Without that, the first Oak promoted into a Sitka stand would make its Sitka part suddenly immune or suddenly doomed. The cost: Oak's capacity is 45 m²/ha and Beech's 60, which are consequences of the provisional weights, not findings. Model 1's calibration record "dropped the unsourced 80–90 m²/ha ceiling" for Sitka because an evidence-based line existed; for broadleaves none does. Change `OccupancyCapacityM2PerHa` if you prefer a different anchor.
2. **Hazard constants equal growth model 1's, as separate named constants.** "Conservative" is read as "no new invented numbers", not "gentler". A gentler broadleaf hazard is a one-line change but would break continuity at the route switch.
3. **Victim suppression uses one common half-saturation (5), not each species' `Ci50`.** This is a deliberate departure from the previous branch. It removes a systematic broadleaf removal bias (a negative control shows the old effect: Reference 1,567 vs Intermediate 3,895 vs High 3,738 deaths in the same fixture, a 33σ bias). The value 5 equals Sitka's `Ci50`, so Sitka stems keep exactly the suppression they had. Shade tolerance (Beech) belongs to a later, deliberate species-behaviour step.

## Verification

**Offline (run here): 68 checks + isolated compile.**

```text
python3 Tools/Verification/ThreeSpecies/OfflineLogic/run_offline_check.py --suite density
  THREE_SPECIES_OFFLINE_PASS checks=68
  MIXED_DENSITY_ISOLATED_PASS compiledWithoutSitkaGrowthModel=true
```

| Packet test | Offline evidence |
|---|---|
| A pure Sitka exact | Routing is Legacy for every all-Reference stand and every stand under models 0/1. SHA-256 of the Sitka arithmetic (`1a81640d…`), the controller decision block (`ee7dc266…`) and `StandRelativeDensity` (`80ef453c…`) equal the values taken from base `c1564b8`. Model 1 vs model 2 **runtime** equality needs the Editor gate. |
| B pure Oak dense | occupancy 1.24 > capacity; 116 of 500 die by hazard; ends at 0.95 ≤ capacity; boundary holds capacity within one stem when the hazard spares everyone |
| C pure Beech dense | occupancy 1.12; 99 of 600 die; ends at 0.94; boundary likewise |
| D mixed | occupancy is additive to 1.8e-15 (no bonus); downgrading a high-demand stem lowers occupancy in order by exactly its contribution; replay identical; all three classes lose stems |
| E demand order | contribution ratios exactly 1.0 : 1.5 : 2.0 at every size; same stems give occupancy 0.428 / 0.641 / 0.855 and pressure 0 / 0.10 / 0.64 for Sitka-class / Beech / Oak |
| F below threshold | occupancy 0.42: pressure 0, zero random draws consumed, no victims; crossing is exact between 352 and 353 stems |
| G above threshold | in the hazard zone deaths occur (and none when the draw spares); above capacity the boundary acts and stops at 0.999 |
| Call path | `SelectRoute` (pure, exhaustive/random); source guards on the controller wiring order; the occupancy method and model contain no Sitka reference; **`MixedDensityModel.cs` compiles without `SitkaGrowthModel.cs`** |
| Neutral victims | at equal size and competition, deaths by class 3,074 / 3,107 / 2,996 (1.4σ); negative control 1,567 / 3,895 / 3,738 (33σ) |

**Mutation tests** (copies in scratch, not the repository): borrowing `SitkaGrowthModel.MaximumRelativeDensity`, editing one constant in the legacy block, nudging `MaximumSdi`, routing pure Sitka to occupancy, ignoring the class weight, and reintroducing species-dependent suppression were each detected.

**Type-check:** prod 7 errors and tools 43 errors, both identical to the committed baselines (`TYPECHECK_CLEAN`).

**Other offline suites:** timber 71 and history 70 checks pass unchanged.

## Not verified

- **Nothing was run in Unity.** The rewritten gate `Tools/Verification/ThreeSpecies/MixedDensityVerification.cs` is type-checked and desk-checked (fixture occupancies were computed independently on both stand geometries) but has never executed.
- Pure Sitka model 1 vs model 2 runtime equality, the lifecycle anchors `7E57B9DAEF5BF4D0` / `35E2BF1C2F55C2DE`, save/load through a dying mixed stand, and every Scenario One anchor are untested here.

## Unity gates still required

```text
# 1. compile/import with Unity 6000.6.0f1 (zero C# errors)
# 2. copy Tools/Verification/ThreeSpecies/MixedDensityVerification.cs to Assets/ForestPrototype/, then
CCF_STAND_GEOMETRY=0 <UNITY> -batchmode -nographics -projectPath <checkout> \
    -executeMethod MixedDensityVerification.Begin -logFile <log>
#    expect MIXED_DENSITY_VERIFY_PASS; remove the copy and its .meta afterwards
#    (repeat without CCF_STAND_GEOMETRY for Enlarged80)
# 3. the existing 24-gate regression: defaults are unchanged, so every anchor must equal Phase0-Baseline.md
# 4. run_unity_gates.py (staged runner) for the remaining programme gates
```

## Compatibility

Growth model 1 stays the default (`ScenarioOneManager.NewGameGrowthModel`); model 2 is not adopted. Save version stays 20 and no field is persisted; the species assets are unchanged (`growingSpaceDemand` 2 / 1 on Oak / Beech, absent on Sitka); `SitkaGrowthModel.cs`, Reference Future v1, timber, history, UI and Project docs are untouched.

## Known limitations

1. Not run in Unity.
2. All route-B magnitudes are [ORD]/[C]; the capacity is a continuity anchor, not evidence.
3. Size-density behaviour is for even-aged pure stands; a weighted sum over an irregular mixture is an abstraction, in keeping with D-061.
4. Neutral victim selection means Beech's shade tolerance is not represented in mortality; flagged for the later species-behaviour step.
5. The route switch is a regime change by design: a Sitka stand and the same stand with one Oak use different arithmetic, kept close by the continuity anchor but not identical.
