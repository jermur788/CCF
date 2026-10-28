# PARITY-03-R — Independent Mixed-Density Conformance Review — Handoff

**Task:** `PARITY-03-R` — independent static conformance review
**Claim:** GitHub issue #9 (`jermur788/CCF`)
**Spec:** `specs/scenario-one-three-species-parity-p0/` (APPROVED), read from `origin/main` `49db765f1dd883310ae56c523d0f317a70d27353`
**Date:** 2026-10-28
**Reviewer:** Claude Sonnet 5.5 (`claude-sonnet-5-5`), Anthropic, via Claude Code in T3 Code. The claim recommended Opus; Sonnet 5.5 is what actually ran.
**Mode:** read-only. No production, spec, YAML, scene, Rack V2 or parameter edit. No Unity. No Windows build. Nothing written inside any CCF worktree except this file, on its own branch.

## Reviewed SHAs

| Role | SHA |
|---|---|
| Review target (static handoff) | `b22c75f439e34d5cf5bcd91a1b4303c40ceec2f8` |
| Tested production HEAD | `e1515e165eb40b40b5200992398984d69e0e8e8e` |
| Inherited implementation base | `c6342f79a3e89c040fadd232017a937132779385` |

Verified by `git diff`:

- `c6342f7..e1515e1` adds only `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-03/TASK-PACKET.md`.
- `e1515e1..b22c75f` adds only the PARITY-03 `HANDOFF.md`.
- So `Assets`, `Packages`, `ProjectSettings` and `Tools` at all three SHAs are byte-identical, and every source citation below holds for all three.
- The branch has since advanced to `03e0c67`, a docs-only Unity-qualification handoff. `git diff --stat e1515e1 03e0c67 -- Assets Packages ProjectSettings Tools` is empty.
- I did **not** review or rely on that Unity handoff. It is outside the stated review target.
- Packet text names `c6342f7`; issue #9 names `b22c75f`/`e1515e1`. Production is identical, so this does not change any finding.

## Verdict

**STATIC CONFORMANCE PASS — UNITY REQUALIFICATION STILL REQUIRED**

No production patch is needed. Production source conforms to FR-1 and FR-2 on every question I could test statically.

Two non-production items are **not** clean passes. Neither blocks running a Unity gate; both are for Manager disposition:

- **Q10:** the continuity tests find the right seam, but their fixtures and tolerances are weaker than "detects a composition cliff" implies.
- **Q11:** several old working records make claims that are stale or lack an authoritative evidence-class mapping.

This review cannot mark density requalified. That remains the Tier-2 Unity gate's job.

## 12-question verdict table

| # | Question | Verdict | Basis |
|---|---|---|---|
| 1 | Model 0/1 stay on legacy density | **PASS** (static) | See Q1 |
| 2 | Model-2 pure Sitka uses the exact protected route | **PASS** (static) | See Q2 |
| 3 | "Pure Sitka" is based only on living individual trees | **PASS** (static) | See Q3 |
| 4 | Any living non-Reference individual selects the component route | **PASS** (static) | See Q4 |
| 5 | Weights 1.0/1.5/2.0 are ordinal occupancy only, not SDI/max-density | **PASS** (static) | See Q5; one observation to keep out of player-facing text |
| 6 | Oak > Beech is a class choice only, no empirical wording | **PASS** (static) | See Q6 |
| 7 | Density victim selection is species-neutral | **PASS**, scoped to the generic component | See Q7; scope to confirm |
| 8 | Species `Ci50` absent from density mortality/victim ranking | **PASS**, scoped | See Q8; scope to confirm |
| 9 | No DEC, DMC, mixture bonus or hidden species density-capacity coefficient | **PASS** (static) | See Q9 |
| 10 | Continuity tests measure the right seam and are strong enough | **NOT PROVEN / QUALIFICATION REQUIRED** | Right seam, passes; adequacy not proven |
| 11 | Old verification records are valid under the approved evidence labels | **NEEDS CHANGE** (docs only) | See Q11 |
| 12 | Density files overlapped by Rack V2 or another live writer | **PASS** | See Q12 |

All "static" verdicts are source reading plus offline .NET arithmetic. None is a Unity or runtime measurement.

## Per-question evidence

All paths are under `Assets/ForestPrototype/` unless noted. Line numbers are at `e1515e1`.

### Q1 — Models 0/1 stay legacy: PASS
- `ForestEcologyController.cs:480-481`: density mortality runs only when `UsesSiteClassGrowth` (model ≥ 1). Model 0 has none.
- `ForestEcologyController.cs:1075`: `anyNonReference = UsesSpeciesAwareGrowth && HasNonReferenceDemand(...)`. Model 1 never evaluates composition.
- `MixedDensityModel.SelectRoute` (`MixedDensityModel.cs:176-178`) returns `LegacySitkaExact` unless `speciesAwareGrowthModel && anyNonReferenceStem`.
- `UsesSpeciesAwareGrowth => growthModelVersion >= GrowthModel.SpeciesAware` (`:151`; `SitkaGrowthModel.cs:18`).

### Q2 — Pure Sitka uses the exact protected route: PASS
- `ForestEcologyController.cs:1076-1080` routes to the species-aware method only when the route is `SpeciesAwareOccupancy`. Otherwise the original block at `:1081-1134` runs unchanged.
- `SpeciesConfigurationPolicy.cs:153-156`: `Reference` is reserved to `sitka-spruce`, and any other species set to `Reference` is rejected. "All living stems are Reference" therefore means "all living stems are Sitka".
- `Tools/Verification/ThreeSpecies/MixedDensityVerification.cs` (`PureSitkaExact`, ~258-296) compares model 1 against model 2 year by year (route, relative density, pressure, victim ids, tree-state hash). It is a good gate but **has not been run by me**. See remaining gates.
- Independent offline re-run (below): `I1_SITKA_COMPONENT_EQUALS_LITERAL_PORT_OF_THE_ORIGINAL_CONTROLLER_BLOCK` states=400, identical=400.

### Q3 — "Pure Sitka" means living individuals only: PASS
- The density input is `FindTrees()` (`:875-885`), which excludes `IsBiologicallyDead`, filtered by `tree.IsLiving` (`:1068`). `ForestTree.IsLiving => !IsStump && !biologicallyDead` (`ForestTree.cs:82`; `IsStump` at `:155`).
- Sub-promotion regeneration lives as cohorts in `ForestEcologyCell`, not as `ForestTree`. It never reaches `HasNonReferenceDemand`.
- Promoted juveniles become `ForestTree`s via the spawner (`PromoteSpeciesCohorts`, `:1714`), so they count from the next density step.
- Living trees are never deactivated (`SetActive(false)` on a tree object occurs only on biological death, `ForestTree.cs:285/552/569/656`). The `FindObjectsInactive.Exclude` enumeration cannot drop a living stem.
- **Low-severity observation:** if `tree.Species` is null the code falls back to the controller's default species, and with both null it treats the tree as Reference (`:1164`, `:1183-1186`). That is defensive and does not occur with the three-species roster. It is not a violation of FR-10, which concerns load-time species ids.

### Q4 — Any living non-Reference individual selects the component route: PASS
- `HasNonReferenceDemand` (`:1160-1169`) returns true on the first living stem whose class ≠ Reference.
- Oak is `growingSpaceDemand: 2` and Beech is `1` in the assets. The three Scenario Two placeholder species are `1`.
- Offline `ROUTE_ANY_OAK_OR_BEECH_IS_SPECIES_AWARE_OCCUPANCY` and `ROUTE_ONE_OAK_FLIPS_AND_REMOVAL_FLIPS_BACK` pass in my re-run of the density suite.

### Q5 — Weights are ordinal occupancy only: PASS (one observation)
- `ReferenceWeight/IntermediateWeight/HighWeight = 1.0/1.5/2.0` (`MixedDensityModel.cs:138-140`).
- They are used in exactly one place, `StemOccupiedSpace` (`:184-188`). The weighted cross-section feeds `Occupancy`, and `NonReferenceShare` in the composite planner (`MixedDensityModel.Composite.cs:150-161`).
- There is no species-specific exponent, no SDI, no Reineke slope, no species maximum.
- The generic response is a bounded, smooth, class-blind function of occupancy (`GenericLoss`, `Composite.cs:101-111`), zero at and below occupancy 1.0.
- Source headers (`MixedDensityModel.cs:18-56`) label weights `[ORD]`/`[C]`.
- **Observation, not a defect:** with `OccupancyNormalization = 55`, pure-stand pressure onset is arithmetically w × (stand basal area, m²/ha) = 55. That is about 55 / 36.7 / 27.5 m²/ha for Reference / Intermediate / High. These are derived consequences of gameplay-calibrated constants. They must never be shown to the player, or written in docs, as Oak/Beech capacities (FR-9, FR-12).
- Searching `UI/`, `ScenarioOne/`, `ForestPlayer.cs` and `Docs/` found no player-facing text that does so.

### Q6 — Oak > Beech is a class choice only: PASS
- Only the integers `2` and `1` in `Species/SessileOak.asset:28` and `Species/BeechSpecies.asset:28` express the ordering.
- `TreeSpeciesDefinition.cs` tooltip says "Ordinal … not a measured density relationship for any species" and calls the Scenario Two classes an "ORDINAL PLACEHOLDER".
- `MixedDensityModel.cs:47` says "Not Oak or Beech data."
- I found no empirical justification text for the ordering in code, assets or the density records.

### Q7 — Density victim selection is species-neutral: PASS, scoped to the generic component
- `DensityStem` carries no species id (`MixedDensityModel.cs:84-99`). `Suppression` uses one `NeutralHalfSaturationCi = 5.0` (`:152, 227-231`), and `Vulnerability` uses one exponent 4.0.
- Generic victims are drawn from the stems not already taken by the Sitka component (`Composite.cs:176-216`). Weight is competitive vulnerability alone.
- Offline `I5_CLASS_LABELS_NEVER_ENTER_THE_GENERIC_ALLOCATION_AT_EQUAL_OCCUPANCY` and `GENERIC_ALLOCATION_DEPENDS_ON_COMPETITIVE_POSITION_ONLY_NOT_ON_WHICH_SPECIES_SITS_WHERE` pass.
- **Scope to confirm:** the overall mixed step is not class-neutral by architecture. Sitka stems also face the Sitka-line component (`SitkaSubsetDensitySignal.Resolve`), and Oak/Beech do not. FR-2 explicitly adopts this component architecture, so I read "species-neutral victim selection" as applying to the generic component, and I mark PASS on that reading. If the Manager meant whole-step neutrality, that is a spec-level question, not a code defect.
- The Hegyi index that feeds both routes uses neighbour diameters and distances only (`UpdateCompetition`, `:930-997`), so it is species-neutral.

### Q8 — Species `Ci50` absent from density mortality/victim ranking: PASS, scoped
- **Generic component and `MixedDensityModel*.cs`:** no `Ci50` reference. The only half-saturation is the neutral constant 5.0.
- **Sitka subset component:** `SitkaSubsetDensitySignal.SitkaCi50 = 5.0` (`:37`) is applied only to Reference stems (`:47-49`, `:65`). `MarkTaken` rejects any non-Reference Sitka victim (`Composite.cs:252`).
- **Legacy route:** uses `treeSpecies.Ci50` per tree (`ForestEcologyController.cs:1090-1092`), but it runs only when every living stem is Reference, i.e. Sitka.
- So Sitka's own `Ci50` appears in the protected Sitka arithmetic. It never ranks an Oak or Beech stem and cannot bias Oak against Beech or either against Sitka. `Ci50` stays a growth/crown trait for Oak/Beech (Oak 5, Beech 6.5).
- **Low-severity observation:** `SitkaCi50 = 5.0` is a duplicated literal, while the legacy route reads the asset (`SitkaSpruce.asset:22`, `ci50: 5`). They agree today. The only check I found that ties the two together is offline, `SITKA_SUBSET_SIGNAL_USES_THE_SITKA_ASSET_CI50`. A later Sitka `Ci50` edit would silently split the pure and mixed Sitka behaviour unless that check runs.

### Q9 — No DEC/DMC/mixture bonus/hidden capacity coefficient: PASS
- `git grep` for DEC, DMC, overyield and mixture bonus/productivity in `Assets/**/*.cs` returns one hit, the header comment `MixedDensityModel.cs:32` ("no DEC, no DMC…").
- Occupancy is plain addition over stems in canonical id order (`Occupancy`, `:201-207`). Offline `OCCUPANCY_IS_ADDITIVE_NO_MIXTURE_BONUS_NO_DEC_NO_DMC` passes.
- `OCCUPANCY_IS_THE_ONLY_INPUT_THE_RESPONSE_SEES` and `NO_HARD_SPECIES_DENSITY_CAPACITY` pass.
- The only per-class number is the weight, which is declared, not hidden. See the Q5 observation.
- `GenericAnnualBasalAreaBudgetShare = 0.125` (`Composite.cs:96`) is a class-blind per-year removal-rate limiter documented as `[C]`. It is not a density capacity.

### Q10 — Continuity tests: NOT PROVEN / QUALIFICATION REQUIRED
**What is right.** The sweep is `CompositeDensityOfflineCheck.ContinuityReplacement` (`Tools/Verification/ThreeSpecies/OfflineLogic/CompositeDensityOfflineCheck.cs`, assertions at `:399-402`). It replaces 1, 10, 50 and 100 of 1000 stems with a High-class stem. The 100% → 99.9% step is precisely the first non-Reference stem, so it does cross the route seam. By construction (generic response × non-Reference share) there is no cliff.

**Independent re-run.** Exported `e1515e1` with `git archive` to `/tmp`, then ran `run_offline_check.py --suite density-composite` and `--suite density` with `python3 -I -B`:

- Composite: 104/107 pass. The three failures are the same pure-Beech R2 misses the PARITY-03 handoff lists (`L40 B_BEECH_DENSE` 0.423, `L40 F_BEECH_YOUNG` 0.407, `E80 F_BEECH_YOUNG` 0.417 against 0.40).
- Density: the one scanner failure (Q11/Q12 below), plus the same three composite failures (4 `FAIL` lines in total).
- Both runs match the PARITY-03 handoff's figures:
  - `C1_TOTAL_LOSS_AT_99_9_PERCENT_SITKA_MATCHES_PURE`: worst 0.0008.
  - `C1_ADJACENT…`: worst step 0.0348, margin factor 2.80.
  - `C2_…`: worst 0.0005.
- My density-suite run ended with process exit 134, not the handoff's "exit 1". Every result line printed first. I did not investigate the exit code.

**Why this is not a proof of "strong enough to detect a cliff":**
1. **Dead rows.** The DBH 15 rows are 0.0000 at every composition, so they assert nothing. The **CI axis is degenerate**: CI 1, 3 and 8 give identical numbers because every stem in a fixture has the same CI. The sweep therefore cannot detect any CI-dependent or suppression-ranking discontinuity.
2. **Saturated rows.** DBH 40 and 60 lose 0.50–0.80 of stems per year, which no real Scenario One stand does. They mostly test the boundary rule, not the seam. The only low-loss, realistic row is DBH 25 (0.0280).
3. **Loose tolerance where it matters.** The 0.005 absolute tolerance is about 18% of the DBH 25 baseline loss. A seam artefact up to about +0.004 would pass. The observed worst (0.0008) has margin, but the assertion would not catch a small cliff.
4. **Geometry and class coverage.** `Area = 0.64` (line 33) is Enlarged80 only. Legacy40 (0.16 ha) is not swept, and Beech (Intermediate) is never the replacing class.
5. **Plan-level only.** The sweep runs the pure planner, not the controller wiring. The Unity gate's near-pure check (`CALL_PATH_ONE_OAK_SWITCHES_TO_OCCUPANCY_ROUTE`, `MixedDensityVerification.cs` ~305-313) asserts route and occupancy, not mortality continuity.

**Likely patch surface (verification only, no production change, for Manager disposition):** parameterise `ContinuityReplacement` by area (0.16 and 0.64) and replacing class; add heterogeneous DBH/CI stands in a realistic loss range; add a relative tolerance in the low-loss regime; add a controller-level 0 → 1 Oak mortality-continuity measurement to the Unity gate.

### Q11 — Old verification records: NEEDS CHANGE (docs only)
Stale or unmapped claims, all in working records under `Docs/Verification/ThreeSpeciesProgramme/` that describe themselves as non-canonical:

- `Phase1-DensityRework.md:171`: "Growth model 1 stays the default (`ScenarioOneManager.NewGameGrowthModel`); model 2 is not adopted." Now false: `ScenarioOneManager.cs:501` is `NewGameGrowthModel = GrowthModel.SpeciesAware`, which FR-10 permits.
- `CompositeDensityV3Handoff.md:3`: "Not merged. Growth Model 2 is still not the default." Same staleness.
- The Unity-measured figures in `CompositeDensityV3Handoff.md` (for example the "0.0011 Unity" continuity result) come from base `90e3496`, not `c6342f7`. They are history, not requalification evidence for this base.
- The records use runtime tags `[B]`, `[C]`, `[D]`, `[ORD]`. FR-12 requires an explicit map to the authoritative classes. `Phase1-DensityRework.md` evidence-status table (~108-112) maps Route A to "[B] … plus [C]" and Route B to "[ORD]/[C] only", but not to `TRANSFER` / `ORDINAL REPRESENTATION` / `GAMEPLAY CALIBRATION` / `FALLBACK-CONSERVATIVE`.
- `README.md` in the same folder: the phase table and check counts (density 35 / timber 71 / history 70) predate the current suites (the composite suite alone now has 107 checks).

The core honesty claims **are still valid**: no biological m²/ha capacity, no Oak/Beech SDI, Route B is `[ORD]/[C]` only.

One verification-scanner issue belongs here and in Q12. `MixedDensityOfflineCheck.NoBiologicalDensityClaim` (`MixedDensityOfflineCheck.cs:358`) scans all of `Assets/ForestPrototype`, so it flags the Rack V2 identifier `ScenarioOneRackBay.MaxRemovedBasalShare` (`ScenarioOne/ScenarioOneRackBay.cs:26`). It is not a density-capacity claim. Separately, the stricter per-file identifier rule (`densityFiles`, `:364`) lists only `MixedDensityModel.cs` and `AdultMortalityAccount.cs`. It omits `MixedDensityModel.Composite.cs` and `SitkaSubsetDensitySignal.cs`, which are the files that hold the basal-area budget and the Sitka component. Fixing either is verification-policy work for the Manager.

### Q12 — Overlap with Rack V2 or another live writer: PASS
- Diffed `e1515e1` against `origin/main`, `origin/integration/scenario-one-rack-v2-v1`, `origin/feature/scenario-one-rack-v2-no-competing-author-v1` and `origin/feature/mfm-rack-sample-geometry-v1`.
- Paths: `MixedDensityModel*.cs`, `SitkaSubsetDensitySignal.cs`, `SitkaGrowthModel.cs`, `ForestEcologyController.cs`, `AdultMortalityAccount.cs`, `TreeSpeciesDefinition.cs`, `Species/`, `Tools/Verification/ThreeSpecies/`.
- Result: no production density file differs on any of them.
- `main` has moved two verification-only files since `e1515e1` (`BroadleafAdultGrowthVerification.cs`, `check_early_life_conformance.py`, PARITY-04-PATCH/05). The MFM branch carries the first of those. Neither touches density.
- Live coordination: `Docs/Project/coordination/active-tasks.md` (working-tree copy on `ops/workflow-foundation-v1`) has **no PARITY-03 or PARITY-03-R row**, and no row lists a density source file as locked. Dispatch for this work is tracked in GitHub issues #8, #9 and #10. The open Rack V2 claim #59 is Combined HUD layout (`StandMapView.cs`, `HudMinimapView.cs`), not density.
- I did not edit `active-tasks.md`. It is single-writer, and the Primary checkout has uncommitted edits to it that are not mine.

## Likely patch surface

- **Production:** none.
- **Verification-only (Manager decides):** `CompositeDensityOfflineCheck.ContinuityReplacement` strengthening (Q10); `MixedDensityOfflineCheck.NoBiologicalDensityClaim` scoping and `densityFiles` list (Q11/Q12); optionally a test tying `SitkaSubsetDensitySignal.SitkaCi50` to the asset.
- **Docs-only:** the stale lines and label mapping in `Docs/Verification/ThreeSpeciesProgramme/` (Q11).

## Remaining Unity/runtime gates

Not performed by this review, and not provable by static reading:

1. Tier-2 `MixedDensityVerification` on the exact candidate: model 2 = model 1 for pure Sitka (hash, victims, route), one-Oak route flip, pure Oak and pure Beech, mixed stands, exact replay, below-onset no mortality, dying-stand save/load, and the Legacy40 and Enlarged80 anchors.
2. Protected Sitka fingerprints and lifecycle anchors, plus clean compile/import in Unity 6000.6.0f1.
3. A controller-level near-pure mortality-continuity measurement, which no current Unity check provides (Q10).
4. Requalification on the actual integration base if it is not `c6342f7`. `c6342f7` is a Rack V2 lane head, not combined integration or `main`.
5. Manager acceptance that a density change affecting broadleaf-containing model-2 anchors is an authorised-change candidate under FR-11, if any anchor moves.

The three pure-Beech R2 misses remain recorded baseline debt (`CompositeDensityKnownMisses.md`). I neither retuned nor rebaselined them. `E80 B_BEECH_DENSE = 0.400` is exactly at the limit and should not be reported as a robust pass.

## Evidence and reproduction

- Static source reading at `e1515e1` using `git show` and `git grep`. No checkout, so no Unity `Library` and no `__pycache__` was created.
- Offline re-run from a `git archive` export of `e1515e1` into `/tmp/parity03r/src` (outside every worktree), with `python3 -I -B`, .NET SDK 10.0.112:
  - `run_offline_check.py --suite density-composite`
  - `run_offline_check.py --suite density`
- Compared against the PARITY-03 static handoff's figures: they agree.
- Not re-run: the typecheck (its failures are Unity 6 API gaps in a 2021.3 approximation, unrelated to density) and the PARITY-03 `static-proof.py` (I derived Q1–Q9 from source instead).

## Product ambiguities returned to the Manager

None blocking. Two scope readings, either of which I would defend, are flagged for confirmation:

- Q7: whether "species-neutral victim selection" means the generic component only (my reading, per FR-2's adopted component architecture) or the whole mixed step.
- Q8: whether "species `Ci50` absent" admits Sitka's own `Ci50` inside the protected Sitka arithmetic (my reading).
