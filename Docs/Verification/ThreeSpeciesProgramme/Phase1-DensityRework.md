# Phase 1 rework — species-aware density foundation

Supersedes `Phase1-MixedDensity.md` (class-weighted Sitka relative density) and the intermediate revision that normalised occupancy against a basal-area "capacity". Authority: D-059 (three-species parity), D-061 (mixed-species density v1: ordinal species-aware growing-space demand). Manager packets "REWORK SPECIES-AWARE DENSITY FOUNDATION" and "FINAL D-061 DENSITY CALIBRATION CORRECTION".

**Status: implemented, type-checked and checked offline. NOT RUN IN UNITY.** Nothing here is a Unity-verified fact. The first implementation compiled in real Unity 6000.6.0f1 with zero errors; this revision has not been compiled there.

## What this is, and what it is not

The species-aware route is a **dimensionless simulation occupancy abstraction**: a game mechanism for "how crowded is this mixed stand, and how much does that thin it".

- It is **not** an SDI, not Reineke's rule, not the British Sitka relative density, not a maximum basal area and not a maximum-density law for any species.
- **Exact Oak and Beech maximum-density evidence is unavailable.** None is used, none is invented, and **no biological m²/ha capacity is claimed** for any species. Occupancy 1.0 is not a measured density of anything: it is the point where the model starts applying overcrowding pressure.
- The growing-space classes are **ordinal** (Reference < Intermediate < High). Their numeric weights, the occupancy normalization and the loss scale are **simulation calibration**, not measurements.
- **Recalibrate if stronger broadleaf density evidence becomes available.** The weights, `OccupancyNormalization` and `MaximumAnnualLoss` all live in `MixedDensityModel.cs`; evidence-based species relationships would replace them there. The legacy Sitka route is independent and would not change.

## Why the first implementation (`568ba5c`) was rejected

It ran Oak and Beech through the Sitka density arithmetic and only multiplied the result by a class weight:

| | Where |
|---|---|
| Oak/Beech entered Sitka arithmetic | `Tally.RelativeDensity` fed each species' own stems/ha and Dq to `SitkaGrowthModel.RelativeDensity` (Comeau et al. 2010 British Sitka line) |
| Sitka boundary reused | `SitkaGrowthModel.MaximumRelativeDensity` decided when the boundary loop stopped |
| Sitka hazard reused | `DensityPressure` (onset 0.6), `Vulnerability` (S⁴), `AnnualDeathProbability` (strength 0.08, cap 0.5), fitted on Sitka fixtures |
| Species decided who died | suppression used each species' own `Ci50`, so at equal competition a broadleaf stem read as more suppressed and was removed first |

Multiplying a Sitka relative density by a class weight is the same as giving each species its own SDI maximum on the Sitka slope: an invented broadleaf maximum-density equation. The intermediate revision removed the Sitka arithmetic but normalised against a basal-area capacity, which implied per-species capacities through the weights and was too easily read as the same claim. This revision removes the capacity, the hard boundary and the inherited hazard.

## Architecture

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

**Route A, `LegacySitkaExact`.** Every stand under growth models 0 and 1, and a model-2 stand whose living adults are all Reference class (Sitka). Nothing was rewritten, ported or copied: the 52-line decision block and `StandRelativeDensity` are the same bytes as at `c1564b8`, and `SitkaGrowthModel.cs` has no diff. `SitkaGrowthModel.cs` compiles without `MixedDensityModel.cs`, so no number in the occupancy model, including the normalization, can change pure Sitka.

**Route B, `SpeciesAwareOccupancy`.** Any Oak or Beech under model 2.

```text
occupied space of a stem = class weight x stem cross-section (pi/4 x DBH^2)
occupancy                = sum of occupied space / stand area / OccupancyNormalization       dimensionless
pressure                 = share of the occupancy lying beyond the onset = max(0, 1 - 1/occupancy)     0 .. below 1
expected annual loss     = MaximumAnnualLoss x pressure                                      a fraction of the stems
which stems              = risk spread by suppression^4 (species-neutral Hegyi index, one common half-saturation)
```

- Stem cross-section is geometry, a size footprint, not a fitted exponent. The class weight is the **only** place species matter and it appears **only** in the occupancy sum. Occupancy is plain addition: no DEC, DMC, overyielding or mixture bonus.
- **Zero region:** at or below occupancy 1.0 there is no density mortality and no random draw is consumed. **Monotone:** pressure and loss rise strictly with occupancy. **Bounded:** pressure stays below 1 and loss below `MaximumAnnualLoss` however crowded the stand is. There is no ceiling and no boundary stage: nothing removes stems to force occupancy back below a line.
- No species or class appears in any pressure, loss, suppression or probability formula (checked by reflection and by source).
- **Victim selection is species-neutral.** The Hegyi index is species-neutral by construction. A species' own `Ci50` already sets its growth response, so using it again here would double-count it (a negative control shows the old effect as a 28σ removal bias against broadleaves). Growth still uses `Ci50` elsewhere. Differential mortality would need its own evidence.
- Broadleaves are not immune: pure Oak, pure Beech and mixed stands overcrowd, lose stems and are never excluded.

## Calibration parameters

| Parameter | Value | Role | Status |
|---|---:|---|---|
| Class weights Reference / Intermediate / High | 1.0 / 1.5 / 2.0 | how much of the shared space a stem takes relative to a Sitka stem | **[ORD]** rank (Oak above Beech is the weakest-supported call), **[C]** magnitude, provisional (D-061) |
| `OccupancyNormalization` | 55 | divisor that makes occupancy dimensionless and places the onset | **[C]** simulation normalization; not a limit, no forestry meaning of its own |
| `MaximumAnnualLoss` | 0.10 | upper bound of the annual loss fraction | **[C]** |
| `OnsetOccupancy` | 1.0 | where pressure starts | a definition (the normalization decides which stands reach it), not a calibration |
| `VulnerabilityExponent`, `NeutralHalfSaturationCi` | 4, 5 | which stems die, never how many | **[C]** initial simulation calibration inherited numerically from growth model 1 for continuity, not supported mortality constants |

**Two numbers set occupancy and loss** (`OccupancyNormalization`, `MaximumAnnualLoss`). The Sitka-calibrated onset 0.6, strength 0.08, quadratic shape and 0.5 per-stem cap are **not** inherited. No parameter is species-specific, and no mortality rate for any species is claimed.

## Continuity calibration (a game objective, not ecological evidence)

The two numbers were chosen so the route switch is reasonably continuous around representative Scenario One Sitka structures, so that the first Oak promoted into a Sitka stand does not make its Sitka part suddenly immune or suddenly doomed. Pure Sitka never uses this route, so no equivalence is claimed outside that.

**Sampled structures.** All 32 recorded growth-model-1 Sitka stands in `Docs/Research/SitkaGrowthMortality/Evidence/growth_model1_stand_development.csv`: four management regimes (unthinned, light, moderate and heavy thinning from below) at ages 20, 30, 40, 50, 60, 80, 100 and 120, spanning 2,100 to 238 stems/ha, Dq 15.8 to 67 cm and relative density 0.44 to 0.995. The offline check confirms those rows agree with `SitkaGrowthModel.RelativeDensity` to within the rounding of the recorded inputs (largest difference 0.002).

**Occupancy against legacy relative density.** Occupancy of a Reference-weight stand is expressed on the legacy scale as `0.6 x occupancy` (0.6 is the legacy onset), so a perfectly continuous onset would give difference 0:

| Unthinned regime | Stems/ha | Dq cm | Legacy RD | Occupancy | 0.6 × occupancy | Difference |
|---|---:|---:|---:|---:|---:|---:|
| age 20 | 2,100 | 15.8 | 0.436 | 0.748 | 0.449 | +3.0% |
| age 30 | 2,088 | 19.8 | 0.693 | 1.172 | 0.703 | +1.5% |
| age 40 | 1,706 | 23.8 | 0.824 | 1.379 | 0.827 | +0.3% |
| age 50 | 1,206 | 28.8 | 0.867 | 1.432 | 0.859 | −0.9% |
| age 60 | 900 | 34.0 | 0.911 | 1.489 | 0.894 | −1.9% |
| age 80 | 538 | 45.4 | 0.985 | 1.582 | 0.949 | −3.7% |
| age 100 | 344 | 56.6 | 0.992 | 1.571 | 0.943 | −5.0% |
| age 120 | 256 | 64.5 | 0.969 | 1.522 | 0.913 | −5.8% |

Across all 32 structures the largest difference is **6.0%** and the mean **2.75%**. The occupancy onset (1.0) falls within 6% of the legacy onset (RD 0.6) at Dq 16, 25, 40 and 64 cm. Normalization 54 would minimise the worst difference (4.9%); 55 was kept as a rounder number with no fake precision.

**Loss against realised legacy loss.** The recorded selected legacy rule (`calibration_selected_mortality_by_year.csv`, 363 stand-years, hazard and ceiling together) against the generic loss at the occupancy a Reference stand would have at that relative density:

| Relative density | Realised legacy loss | Generic loss |
|---:|---:|---:|
| 0.70 (24 stand-years) | 0.66% | 1.43% |
| 0.80 (61) | 2.19% | 2.50% |
| 0.90 (161) | 4.46% | 3.33% |

The mean absolute difference over 0.65 to 0.95 is **0.74 percentage points**; a `MaximumAnnualLoss` of 0.08 to 0.12 all stay under 1 point, and 0.10 was kept as a round value.

**Where the shapes honestly differ.** The legacy loss is convex (almost nothing near the onset, steep near the line) and is backed by a hard ceiling; the generic loss is smoother and concave and has no ceiling. The generic loss is therefore higher than legacy just above the onset and lower near relative density 1. The long-run density a mixed stand settles at is set by the balance of growth and bounded loss rather than by a line, and this document does **not** predict it: it must be measured in Unity.

## Evidence status

| Item | Status |
|---|---|
| Route A: British Sitka line (slope −2.063, SDImax 1,868), hazard and maximum line | **[B]** transferred British evidence plus **[C]** calibration, exactly as accepted for growth model 1. Unchanged. |
| Route B occupancy, pressure, loss | **[ORD]/[C]** only. **Nothing is empirical, and nothing is transferred as evidence.** |
| Stem cross-section as footprint | Geometry. Not empirical, not fitted. |
| Hegyi competition index | Existing model, species-neutral by construction. |

## Decisions for the Manager

1. **`OccupancyNormalization` 55 and `MaximumAnnualLoss` 0.10 are continuity calibrations**, anchored to recorded Sitka behaviour. Change them in `MixedDensityModel.cs` if you prefer a different anchor; nothing else moves.
2. **No hard ceiling.** The loss is bounded and smooth, so mixed stands are no longer forced onto a line. This follows the packet's "bounded annual effect" and is the main behavioural difference from legacy; it needs a Unity measurement.
3. **Victim-allocation seam inherited.** Exponent 4 keeps dominant stems from being picked. A flatter allocation is a possible later simplification but would put crop trees at higher risk.
4. **A pre-existing regeneration tooltip** in `ForestPlayer.cs` (present at the base commit) says "pull up a cohort at its species maximum density". It concerns juvenile regeneration cohorts (`regenDensityMax`), not adult stand density, and is unrelated to this work. It is not edited; the identifier scan allowlists that one literal.

## Verification

**Offline (run here): 91 checks plus two isolated compiles.**

```text
python3 Tools/Verification/ThreeSpecies/OfflineLogic/run_offline_check.py --suite density
  THREE_SPECIES_OFFLINE_PASS checks=91
  MIXED_DENSITY_ISOLATED_PASS compiledWithoutSitkaGrowthModel=true
  SITKA_MODEL_ISOLATED_PASS compiledWithoutMixedDensityModel=true
```

| Packet requirement | Evidence |
|---|---|
| A: no species max-density identifier or string | A lexer scans all 115 production files (89,403 identifiers, 4,588 strings, comments excluded); none expresses a species maximum density, maximum basal area or SDImax (one pre-existing regeneration tooltip allowlisted); documentation states no species capacity |
| B: Reference < Beech < Oak | contribution ratios exactly 1.0 : 1.5 : 2.0 at every size; same stems give occupancy 0.70 / 1.05 / 1.40 and pressure 0 / 0.05 / 0.29 for Reference / Beech / Oak |
| C: normalization moves only the mixed route | halving or doubling it scales occupancy exactly 2× or 0.5× and changes pressure, loss and (nested) victims; pure Sitka numbers are bit-identical, there is no shared mutable state, and `SitkaGrowthModel.cs` compiles without the occupancy model |
| D: zero region, monotone, bounded | zero at and below the onset (no draws consumed); strictly increasing to occupancy 1000; pressure below 1 and loss at most 0.10 even at occupancy 10¹²; expected deaths equal expected loss × stems (57.6 against 57.8) |
| E: no species in the mortality formula | reflection (all five formula methods take only numbers and the calibration) and source (no class, weight, species, stem or id); the class weight is used only in the occupancy sum |
| F: species-neutral victims | at equal size and competition, deaths by class 2,298 / 2,313 / 2,167 (2.4σ); negative control 1,146 / 2,865 / 2,727 (28σ) |
| G: pure Sitka unchanged | SHA-256 fingerprints of the Sitka arithmetic (`1a81640d…`), the controller decision block (`ee7dc266…`) and `StandRelativeDensity` (`80ef453c…`) equal base `c1564b8` |

Pure Oak and pure Beech: dense fixtures (occupancy 2.02 and 1.84) lose 28 of 500 and 37 of 600 stems in a year, fall towards the onset over 12 years (183 and 198 lost) and are never emptied; just above the onset the loss is small but not zero. Mixed stands: occupancy is additive to 2e-15, downgrading a high-demand stem lowers it in order by exactly its contribution, all three classes lose stems, replay is exact.

**Accounting against the previous revision's 68 density checks** (compared by running that revision's suite): 53 kept their name and property (some fixtures retuned for the new onset); 6 were renamed for the new vocabulary with the same property (`OCCUPANCY_IS_BASAL_AREA_GEOMETRY…` is now `…STEM_CROSS_SECTION_GEOMETRY…`, `SAME_STEMS_AS_REFERENCE_CLASS_ARE_LESS_CROWDED…` is now `…COUNTED_AS_REFERENCE_ARE_HALF_AS_CROWDED…`, and per species `DENSE_OCCUPANCY_EXCEEDS_CAPACITY` is now `…EXCEEDS_THE_ONSET` and `HAZARD_ZONE_LOSES_STEMS_WITHOUT_BOUNDARY` is now `…JUST_ABOVE_THE_ONSET_LOSES_FEW_STEMS`); and 9 were retired because the capacity and boundary stage no longer exist (`ABOVE_CAPACITY…BOUNDARY_ACTS…`, `BOUNDARY_VICTIMS_MOST_SUPPRESSED_FIRST…`, `LAST_STEM_IS_NEVER_REMOVED…`, `NO_COMPETITION_NO_HAZARD_BOUNDARY_STILL_HOLDS`, the capacity continuity band, and per species `ENDS_AT_OR_BELOW_CAPACITY` and `BOUNDARY_HOLDS_CAPACITY…`). 32 checks are new, giving 91.

**Mutation tests** (copies in scratch, not the repository), each detected: a species max-basal-area identifier; a UI string claiming an Oak maximum density; a class argument in the loss formula; no zero region; unbounded loss; non-monotone loss; `SitkaGrowthModel` reading an occupancy constant; a normalization silently ignored; a class weight leaking into victim choice. The earlier mutations (borrowing a Sitka constant, editing the legacy block, nudging the Sitka SDI, mis-routing pure Sitka, ignoring the weight, species-dependent suppression) were also detected.

**Type-check:** prod 7 and tools 43 errors, identical to the committed baselines (`TYPECHECK_CLEAN`). **Other offline suites:** timber 71 and history 70 checks pass unchanged.

## Not verified

- **Nothing was run in Unity.** The rewritten gate `Tools/Verification/ThreeSpecies/MixedDensityVerification.cs` is type-checked and desk-checked (fixture occupancies and expected deaths were computed independently on both stand geometries) but has never executed.
- Pure Sitka model 1 against model 2 at runtime, the lifecycle anchors `7E57B9DAEF5BF4D0` / `35E2BF1C2F55C2DE`, save/load through a dying mixed stand, and every Scenario One anchor are untested here.
- The long-run behaviour of a mixed stand without a ceiling is unmeasured.

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
2. Every route-B number is [ORD]/[C]; the normalization and loss scale are continuity calibrations against Sitka, not evidence about any species.
3. Without a ceiling, the long-run density of a mixed stand differs in shape from pure Sitka and is not predicted here.
4. Neutral victim selection means Beech's shade tolerance is not represented in mortality; that belongs to a later, deliberate species-behaviour step with its own evidence.
5. The route switch is a regime change by design: a Sitka stand and the same stand with one Oak use different arithmetic, kept close by the calibration but not identical.
6. Size-density behaviour is for even-aged pure stands; a weighted sum over an irregular mixture is an abstraction, in keeping with D-061.
