# Phase 1 — species-aware mixed density (growth model 2)

Authority: Manager packet decisions D-059 (three-species parity), D-061 (mixed-species density v1: ordinal species-aware growing-space demand). These are not yet in the locked `decision-log.md` (MiMo is syncing it); see the final handoff, "Canonical updates required".

**Status: implemented, type-checked and checked offline. NOT RUN IN UNITY.** Nothing here is a Unity-verified fact.

## The defect this removes

`ForestEcologyController.ApplyAdultDensityMortality` (growth model 1) pools every living tree, whatever its species, into one stems/ha and one quadratic mean diameter and feeds them to the **British Sitka** maximum size-density relationship (`SitkaGrowthModel.RelativeDensity`, Comeau et al. 2010, slope −2.063, SDI 1,868 at Dq 25 cm; labelled [B] transfer evidence). Each Oak or Beech stem was therefore counted as exactly one Sitka stem. The same density pressure, hazard and maximum-line boundary then applied to the mixed stand. That is "Sitka-derived density pressure applied to a mixed stand", and it is not a statement about Oak or Beech.

## What D-061 allows, and what this does

D-061 accepts an **ordinal species-aware growing-space-demand representation** and rejects (a) ignoring Oak/Beech in density effects and (b) inventing Oak/Beech SDI/maximum-density equations. It forbids DEC, DMC, overyielding, mixture bonuses and silent Sitka-anchor rebaselines. Implemented accordingly:

- Three ordinal classes, `GrowingSpaceDemandClass`: `Reference`, `Intermediate`, `High`.
- A species carries its class as **data** (`TreeSpeciesDefinition.growingSpaceDemand`, default `Reference`). There is no per-species controller and no species switch in the ecology code.
- Stand relative density is the **sum over species** of that species' own Sitka-relationship density (its own stems/ha and its own Dq) multiplied by its class weight. A species of weight *w* reaches the maximum line at 1/*w* of the stem number Sitka does.
- The mortality shape is unchanged: the same onset, pressure, hazard form, suppression-biased victim choice, deterministic per-tree keyed roll and maximum-line boundary. Broadleaf and mixed stands can therefore self-thin; none is made immune.
- Nothing couples species beyond sharing the stand's weighted density: no DEC, no DMC, no overyielding, no mixture productivity bonus.

### The classes and their status

| Class | Weight | Species now in it | Status of the number |
|---|---:|---|---|
| Reference | 1.0 | Sitka spruce | **[B]** calibrated British Sitka line, used unchanged. Exactly 1.0 so a Reference-only tally is arithmetically the Sitka relationship. |
| Intermediate | 1.5 | Beech | **[ORD]** rank, **[C]** magnitude. |
| High | 2.0 | Sessile oak | **[ORD]** rank, **[C]** magnitude. |

These magnitudes are deliberately coarse and round. They are **not** measured Oak/Beech maximum-density data and are not Irish or British empirical values. They encode one qualitative statement: *Oak and Beech reach density-dependent mortality at lower stem numbers than Sitka*, and Oak earlier than Beech. The Sitka-versus-broadleaf ordering is the better-supported call (Sitka tolerates very high British stocking); the **Oak-above-Beech ordering is the weakest-supported call** (light-demanding, wider-crowned oak versus shade-tolerant beech is a qualitative contrast, not a fitted one). Merging both into one class is a one-field edit per asset if the Manager prefers fewer distinctions. No species-pack numbers were available in the repository (the evidence PDFs are local to the Manager and uncommitted), so no empirical value could be adopted.

Flagged for the Manager, not decided here: whether a literature-backed Oak/Beech SDImax-ratio exists that could replace the [ORD] magnitudes in a later version.

## Why a new growth-model value instead of editing model 1

`GrowthModel` is the repository's established mechanism for a behaviour change: model 0 legacy, model 1 Class III Sitka. Changing model 1 in place would alter the continuation of every existing model-1 save that contains Oak/Beech, and would move official mixed-stand anchors by construction. Instead:

- `GrowthModel.SpeciesAware = 2` is added; `Latest` becomes 2. Model 2 = model 1 + species-aware density (later phases attach to the same value).
- `ScenarioOneManager.NewGameGrowthModel` is **left at `SiteClassDensity` (1)**. No new game, no existing save and no official anchor changes behaviour. Adopting model 2 for new games is a one-line change that should be made only after the Unity gates below pass and the Manager accepts the re-baselined mixed-stand anchors. This is the safe reading of "do not rebaseline silently": nothing is rebaselined.
- Under model 2, a stand whose living trees are all Reference class takes the **existing code path, untouched**: pure Sitka under model 2 is model 1 by construction, not by coincidence.

## Architecture

```text
TreeSpeciesDefinition.GrowingSpaceDemand   (data; Oak=High, Beech=Intermediate, Sitka default Reference)
        |
ForestEcologyController.ApplyAdultDensityMortality
   living trees (FindTrees order: ordinal TreeId, dead excluded)
   if model>=2 and any living tree is non-Reference:
        ApplySpeciesAwareDensityMortality -> MixedDensityModel.Plan(stems, area, keyedRoll)   [pure, no UnityEngine]
   else:
        the existing Sitka route, line for line
   both routes end in KillDensityVictims (one change batch; ApplyMortality("self-thinning", year))
```

Files:

| File | Change |
|---|---|
| `Assets/ForestPrototype/MixedDensityModel.cs` (+`.meta`) | New. Pure model: class enum, weights, `Tally` (per-species running sums in ordinal species order), `Plan`. No `UnityEngine` types. |
| `Assets/ForestPrototype/SitkaGrowthModel.cs` | `GrowthModel.SpeciesAware = 2`, `Latest = 2`, comment. No Sitka arithmetic touched. |
| `Assets/ForestPrototype/TreeSpeciesDefinition.cs` | New serialized field `growingSpaceDemand` (default `Reference`) and accessor. |
| `Assets/ForestPrototype/Species/SessileOak.asset`, `BeechSpecies.asset` | One YAML line each (`growingSpaceDemand: 2` / `: 1`). The Sitka asset is **not edited**. |
| `Assets/ForestPrototype/ForestEcologyController.cs` | One gated branch before the Sitka route; the Sitka route's tail (the kill loop) moved verbatim into `KillDensityVictims`, shared by both routes; two new methods. |
| `Assets/ForestPrototype/AdultMortalityAccount.cs` | Two diagnostic fields (`SpeciesAwareRoute`, `UnweightedRelativeDensity`). Never saved. |

Reviewing the controller diff confirms the Sitka route is textually unchanged: `git diff` shows exactly (1) the new property, (2) a 6-line inserted branch ahead of the unchanged `account.RelativeDensity = StandRelativeDensity(...)` line, (3) the tail block moved into a helper with identical statements and order, (4) the new methods.

## Determinism and random state

- The per-tree death roll is `SimulationRandom.Roll(rngModelVersion, "ADULT-MORT-" + treeId, year, seed)`: a keyed hash, not a stream. The species-aware route uses the **same domain string**, so a tree's draw for a given year is the same number on either route, and no random state is consumed, reordered or advanced.
- Victim ordering: hazard victims in tree-id order; boundary victims most-suppressed-first with ordinal tree-id ties; unchanged rules.
- Species accumulation order is ordinal species id (binary-insertion sorted), independent of enumeration order.

## Persistence and save compatibility

- **No new persistent state.** The class lives on the species asset. `ForestSaveData` is untouched; `CurrentVersion` stays 20.
- The existing `growthModel` field now accepts 0, 1 and 2 (validator range is `GrowthModel.Latest`). A save with `growthModel` 3 is still rejected (`unknown growth model 3`).
- Saves ≤ v16 and saves without the field still load as model 0; model-1 saves still load as model 1 and run exactly as before (verified by construction; **not run**).
- A save written with `growthModel: 2` by this build will be **rejected by an older build** ("unknown growth model 2"). That is the intended forward behaviour and matches how models 1 (v17) and regen 2 (v18) behaved; no new-game default produces such a save until the Manager adopts model 2.
- Reference Future v1 is model 0 and is untouched.

## What was verified, and how

### Offline (executed here) — `Tools/Verification/ThreeSpecies/OfflineLogic/`

`python3 Tools/Verification/ThreeSpecies/OfflineLogic/run_offline_check.py` compiles the **production** `SitkaGrowthModel.cs` and `MixedDensityModel.cs` unchanged with the check and runs it on the plain .NET SDK. Result: **`THREE_SPECIES_OFFLINE_PASS checks=35`**.

Highlights (all executed, deterministic seeds):

- `SITKA_ONLY_RELATIVE_DENSITY_BIT_EXACT` — 316 random Sitka stands (four areas, 1–1,500 stems, 3–95 cm): the species-aware relative density is **bit-identical** to the original pooled computation.
- `SITKA_ONLY_PLAN_IDENTICAL_TO_ORIGINAL_ROUTE` — 112 stands straddling the onset and the maximum line (72 with deaths, 56 with boundary deaths, 16 hazard-only): victim lists, hazard/boundary counts, relative density and pressure are **identical** to a restatement of the original controller route. The generalisation reduces to the Sitka route exactly; production still routes pure Sitka to the original code, so this is a second, independent layer of protection.
- Weights exactly 1.0 / 1.5 / 2.0, ordered; unknown class fails loudly; a species cannot straddle two classes.
- Dense **pure Beech and pure Oak** have pressure where the same stems as Sitka have none (Oak greater than Beech); crowded pure Beech/Oak stands **self-thin and end at or below the maximum line**.
- Mixed stands (Sitka+Oak, Sitka+Beech, Oak+Beech, all three) end on or below the maximum line, boundary victims are most-suppressed-first, victims are more suppressed than survivors, all three species die in the aggregate run.
- Onset threshold: one stem below the crossing there is no pressure and **no roll is consumed**; one above, pressure is positive. At most one roll per stem.
- Replay identical; species interleaving does not change density bit-for-bit; different years give different draws.
- The Oak/Beech assets carry `2`/`1`, the Sitka asset carries no field (Reference by default).

One check failed during development and the failure was in the test's hypothesis, not the model: pooling a handful of small Oak stems does **not** meaningfully lower pooled density (0.5484 vs 0.5483; `Dq^2.063` makes the effect negligible). The real defect is that the pooled route gave broadleaf stems no extra weight; the assertion was rewritten to compare a half-Sitka/half-Oak stand (pooled Sitka route 0.4755 vs weighted 0.7133) and now passes.

### Type-checked

`run_typecheck.py --mode prod` and `--mode tools`: no error beyond the committed baselines (7 and 43). This includes the new Unity gate below.

### Not run — requires Unity

`Tools/Verification/ThreeSpecies/MixedDensityVerification.cs` is the in-Editor gate. It is written and type-checked but **has never been executed**. It covers: species classes on the real assets; policy (model 2 loads, model 3 rejected, v16 save → 0, Reference preview → 0 and restored); pure-Sitka model 2 equals model 1 year by year and by tree-state hash over 10 years of a stand in the hazard zone; routing flips on a single Oak; dense pure Sitka/Beech/Oak ordering; pure Beech/Oak self-thin, are held on the maximum line, record cause and year and create deadwood exactly once; the four mixtures including replay; the onset threshold on the real controller; save/load determinism through a dying mixed stand; and the **GM1 pure-Sitka lifecycle anchors `7E57B9DAEF5BF4D0` / `35E2BF1C2F55C2DE` reproduced under model 2**.

## Unity gates still required for Phase 1

```text
# 1. compile/import with Unity 6000.6.0f1 (zero C# errors; no new importer exceptions)
# 2. copy Tools/Verification/ThreeSpecies/MixedDensityVerification.cs to Assets/ForestPrototype/, then
CCF_STAND_GEOMETRY=0 <UNITY> -batchmode -nographics -projectPath <checkout> \
    -executeMethod MixedDensityVerification.Begin -logFile <log>
#    expect MIXED_DENSITY_VERIFY_PASS; remove the copy and its .meta afterwards
# 3. the existing 24-gate regression (unchanged defaults => every anchor must be exactly as in Phase0-Baseline.md)
# 4. measurement, not a pass/fail gate: run the completion gate under model 2
CCF_GROWTH_MODEL=2 <UNITY> ... -executeMethod ScenarioOneCompletionVerification.Begin
#    record the new SCENARIO_ONE_COMPLETION_HASH, whether it completes in Year 25, minimum cash,
#    and the first divergence from 702766DECE591E21 if any. That decides whether to adopt model 2.
```

## Predicted effect on anchors (a prediction from reading code, not a measurement)

- Every anchor produced under models 0 and 1 and every pure-Sitka fixture: **unchanged**, because the default is unchanged and the Sitka route is untouched.
- Under model 2, fixtures that never hold a non-Sitka adult (lifecycle, Reference Future): **unchanged**.
- Under model 2, the Scenario One completion and Enlarged80 fixtures can change only if a planted or recruited Oak/Beech tree has been promoted to an adult **and** the weighted density crosses a threshold the pooled density did not. Sixteen planted stems among ~1,000 Sitka shift the weighted density by a few percent, so a change needs the stand to sit near the onset (RD 0.6) or maximum line (RD 1.0) at that moment. Until the gate is run this is unknown.

## Known limitations and open questions

1. Not run in Unity (the whole point of the gate list above).
2. Class magnitudes are [ORD]/[C]; the Oak-above-Beech ordering is the weakest call.
3. Onset 0.6, strength 0.08, suppression exponent 4 and the maximum line RD 1.0 are the Sitka calibration ([B]/[C]) applied to the weighted density. They were not recalibrated for broadleaves and there is no evidence to do so.
4. Suppression for victim choice uses each species' own Hegyi `Ci50` (Sitka 5, Oak and Beech 3). At equal competition a lower `Ci50` reads as more suppressed, so Oak/Beech are, if anything, more likely than Sitka to be chosen in a mixed hazard stage. Beech's shade tolerance is therefore not represented in mortality. This is flagged for Phase 3/4 (shade/site behaviour) rather than changed here.
5. Size-density relationships are for even-aged pure stands; applying a weighted sum to irregular mixtures is an abstraction, in keeping with D-061.
6. The unrelated `StandRelativeDensity` helper is still used by the Sitka route and unchanged.

## Canonical updates required (not made — MiMo owns these files)

- `decision-log.md`: record D-059, D-060, D-061 and D-062 as accepted by the Manager; add the growth-model-2 definition (version gate, Reference route untouched, class table and status labels).
- `unity-project-overview.md`: growth models 0/1/2; `MixedDensityModel`; `growingSpaceDemand` on species; verification entry points `MixedDensityVerification.Begin` and the offline check; the cloud type-check tool and its limits.
- `current-milestone.md`: three-species parity and full-loop requirement before the external demo (D-059/D-060).
