# Natural Old-Age Tree Death and Smooth Late-Life Growth V1 — Implementation Plan

- **Spec** [SPEC-multi-2d-senescence-late-growth-v1](spec.md)
- **Status** APPROVED / IMPLEMENTING (Part 1); Part 2 DEFERRED
- **Context commit** `bdeb6a324acaf4129c10fb0846faac8b64974360` (`origin/main`)
- **Base** `b2006767cbe70fa2c822497470908251a0fd6ffd` (`origin/integration/2d-sim`)
- **Branch** `feat/multi-2d-senescence-engine-v1`

## Part 1 architecture and file list

All Part 1 source files live in namespace `SitkaP1.Core.NaturalProcesses.Senescence`, using .NET 8 BCL plus read-only calls into existing `SitkaP1.Core.SpeciesWave2` types (zero edits to existing files).

### New files owned by this branch

1. **Spec quartet (`specs/multi-2d-senescence-late-growth-v1/`):**
   - `spec.md`
   - `plan.md`
   - `tasks.md`
   - `checklists/requirements.md`
2. **Engine (`Tools/MultiRackSitkaP1Lab/src/Core/NaturalProcesses/Senescence/`):**
   - `SenescenceContracts.cs` — `SenescenceModelVersion`, `SenescenceStreams`, `SenescenceText`, `SenescenceSpeciesProfile`, `SenescenceTreeInput`, `SenescenceRequest`, `SenescenceHazardEvaluation`, `SenescenceDeathRecord`, `SenescenceResult`, and defensive `SenescenceSnapshotList<T>` (plain enumeration copy, never `ToArray()` or `new List<T>(source)` on caller input).
   - `SenescenceHazard.cs` — six independent species profiles reusing `SpeciesMoistureTable` ID constants, pure `AnnualHazard(speciesId, ageYears)` smoothstep curve, and `SurvivalProbability(speciesId, startAgeInclusive, endAgeInclusive)`.
   - `SenescenceEngine.cs` — version-gated `Evaluate(SenescenceRequest)` and deterministic stateless `RollUniform(speciesId, treeId, year, seed)`: 64-bit FNV-1a over the full key, then Model 1's SplitMix64 mixing with year and seed, returning a 53-bit uniform double in `[0, 1)`. Not Model 1 bit-parity: the 32-bit key hash was replaced in repair cycle 1 because it let distinct trees share every roll (spec FR-3).
   - `LateLifeGrowth.cs` — `LateLifeGrowthRequest`, `LateLifeGrowthResult`, and `LateLifeGrowth` evaluator for `scots-pine`, `common-alder`, and `downy-birch` only.
3. **Standalone offline console test executable (`Tools/MultiRackSitkaP1Lab/tests/Core/Senescence/`):**
   - `SitkaP1.Senescence.Tests.csproj` — `OutputType=Exe`, compiling only `../../../src/Core/SpeciesWave2/**/*.cs`, `../../../src/Core/NaturalProcesses/Senescence/**/*.cs`, and its own test `.cs` files.
   - `Program.cs`
   - `TestRunner.cs`
   - `Fixtures.cs`
   - `VersionGateTests.cs`
   - `HazardTests.cs`
   - `RollsAndSemanticsTests.cs`
   - `LateLifeGrowthTests.cs`
   - `DeterminismTests.cs`
   - `HygieneTests.cs`
4. **Handoff (`Docs/Handoffs/MultiRack-Simulation-20261116/senescence-late-growth/`):**
   - `IMPLEMENTATION-HANDOFF-PART1.md`

## Key design choices and formulas

### 1. Six-species smoothstep annual hazard (`SenescenceHazard.cs`)

- Reuse `SpeciesMoistureTable.CommonAlder`, `.DownyBirch`, `.SitkaSpruce`, `.EuropeanBeech`, `.ScotsPine`, `.SessileOak`.
- For `ageYears <= profile.OnsetAgeYears`: return `0.0` (bit-exact zero).
- For `ageYears > profile.OnsetAgeYears`:
  ```text
  t = min(1.0, (ageYears - onset) / (double)ramp)
  s = t * t * (3.0 - 2.0 * t)
  h = maximumAnnualHazard * s
  ```
- When `ageYears >= onset + ramp`, `t == 1.0`, `s == 1.0`, and `h == maximumAnnualHazard` bit-exactly.
- Maximum annual increment occurs near `t = 0.5` where `ds/dt = 1.5`, so `max delta <= 1.5 * maxHazard / ramp <= 0.00075` across all six species (well below the `0.002` ceiling).
- For `common-alder`, ramp = **60 years** (onset 55, ceiling 0.030) was explicitly chosen to keep the pioneer hazard modest through the age 60–70 commercial rotation window without a cliff: `h(alder, 61..70) < 0.005` (`h(70) = 0.0046875`, survival 60→70 ≈ 0.974), while survival 70→150 remains ≈ 0.140 (inside `(0.05, 0.60)`). Downy birch 70→150 survival is ≈ 0.192. Survival from `a` to `b` is the product of `1 − h` over ages `a..b`, both ends included (`SurvivalProbability(species, a, b)`).
- Common alder heart rot (`BirchAlderTimber.cs`) is never referenced by `SenescenceHazard` or `SenescenceEngine`; it remains purely a timber-quality downgrade check on felled stems.

### 2. Version gating and determinism (`SenescenceEngine.cs`)

- `SenescenceModelVersion.Disabled` (0) is the default. When `Disabled`, `Evaluate` validates only the request/year/enum envelope and returns immediately with zero deaths, `RngDrawCount == 0`, and `TreesEvaluated == 0`, performing no species or age inspection. Legacy/frozen runs therefore incur zero RNG draws and stay byte-identical.
- When `OldAgeV1` (1):
  1. Validate tree list, reject blank/duplicate `TreeId`, negative `AgeYears`, and unknown `SpeciesId`.
  2. Sort snapshot trees by `TreeId` (`StringComparer.Ordinal`) so caller input order cannot affect output order or fate.
  3. For each tree, compute `hazard = SenescenceHazard.AnnualHazard(speciesId, ageYears)`, build key `"SENESCENCE-OLD-AGE-v1-" + speciesId + "/" + treeId`, perform one stateless keyed roll (64-bit FNV-1a over the key + SplitMix64 mixing with `year` and `seed`), increment `RngDrawCount`, and emit one `SenescenceDeathRecord` if `hazard > 0.0 && roll < hazard`.
- Because `SenescenceStreams.OldAgeTreePrefix` (`"SENESCENCE-OLD-AGE-v1-"`) is strictly prefix-free against all existing `RngSalts` and `NaturalProcessStreams` keys, Part 2 can register it directly in `RngSalts.cs` without shifting any existing stream or changing Part 1 hashes.

### 3. Why delegation preserves the existing Wave 2 window bit-exactly (`LateLifeGrowth.cs`)

- For `StandAgeYears <= cutoff` (60 for Scots pine and Common alder; 70 for Downy birch), `LateLifeGrowth.EvaluateGrowth` calls `ScotsPineEngine.EvaluateGrowth` or `BirchAlderEngine.EvaluateGrowth` directly and copies the returned `PotentialDbhIncrementCm`, `SiteAdjustedDbhIncrementCm`, and `SiteResponse` verbatim without re-evaluating arithmetic.
- This guarantees bit-exact agreement with existing Wave 2 results for every in-window age, stocking, basal area, top height, and moisture class, without editing any existing Wave 2 source file.
- For `StandAgeYears > cutoff`:
  - Domain bands are read directly from `FutureSpeciesCatalog.ScotsPine.Readiness` and `BirchAlderEngine.GrowthDomain(speciesId)`.
  - Cutoff anchor `A` is queried by delegating to the existing evaluator at `cutoff` with zero crowding and `SiteMoistureClass.NeutralLegacy` (never hardcoded). For Scots pine `A == 0.20` bit-exactly; for Downy birch `A == 0.25` bit-exactly; for Common alder `A == 0.19999999999999996` (2 ULPs below `0.20`, purely because of IEEE-754 representation of the unmodified linear window `0.95 - 0.75`, not a model discontinuity).
  - Nominal increment follows `A * Math.Exp(-k * (age - cutoff))`, where `k = (-windowSlope) / A`, matching the exact delegated cutoff bits and the exact window slope (`-0.008`, `-0.75/55`, `-0.60/65`) at the cutoff (`C0` bit-exact and `C1` smooth, with no manufactured 2-ULP jump for alder).
  - Crowding pressure uses the exact `max(normalised stocking, normalised basal area)` formula and each species' existing maximum crowding reduction (`0.50` pine, `0.65` alder, `0.70` birch). For Scots pine, the maximum crowding reduction is derived directly from `ScotsPineEngine.EvaluateGrowth` at max vs zero crowding so no private constant is duplicated.
  - Site growth multiplier is read once from `SpeciesMoistureTable.Cell(speciesId, request.Moisture).Response.GrowthMultiplier` and multiplied once (`potential * multiplier`).
- `sitka-spruce`, `sessile-oak`, and `european-beech` are refused with `ArgumentException`; their existing growth equations are untouched.

## Verification commands

Run from `Tools/MultiRackSitkaP1Lab` using pinned SDK `8.0.133` (`global.json`):

```text
dotnet restore tests/Core/Senescence/SitkaP1.Senescence.Tests.csproj
dotnet build tests/Core/Senescence/SitkaP1.Senescence.Tests.csproj -c Release --no-restore
dotnet run --project tests/Core/Senescence/SitkaP1.Senescence.Tests.csproj -c Release --no-build --no-restore > run1.txt
dotnet run --project tests/Core/Senescence/SitkaP1.Senescence.Tests.csproj -c Release --no-build --no-restore > run2.txt
cmp run1.txt run2.txt
```

Verify the hard write fence against `b2006767cbe70fa2c822497470908251a0fd6ffd` (`git diff --name-only` and `git status --short`) and remove temporary `run1.txt`/`run2.txt` before committing.

## Risks

- **Calibration interpretation risk:** Players or future contributors could mistake the smoothstep hazard parameters or exponential late-life decay rates for fitted Irish curves. Mitigated by explicit `[C]` evidence tags on profiles/results and clear provenance disclosures in `spec.md` and code comments.
- **Scots pine private constants:** `ScotsPineGrowth.cs` keeps `0.60`, `0.20`, and `0.50` `private const`. Because Part 1 cannot edit `ScotsPineGrowth.cs`, `LateLifeGrowth.cs` derives the cutoff anchor, young anchor, and maximum crowding reduction directly by delegating to `ScotsPineEngine.EvaluateGrowth` at domain boundaries, avoiding duplicated numeric literals while keeping `ScotsPineGrowth.cs` untouched.
- **Alder IEEE-754 cutoff value:** Delegating to `BirchAlderEngine.EvaluateGrowth` at age 60 yields `0.19999999999999996` (2 ULPs below `0.20`). Using that delegated value as the continuation anchor `A` avoids any artificial 2-ULP jump while leaving `BirchAlderGrowth.cs` untouched.
- **NaturalProcesses test-project glob:** the existing standalone NaturalProcesses test project recursively compiles `src/Core/NaturalProcesses/**`, so it will pick up this module and fail to compile until the integrator applies one of the two one-line options in spec.md "Integration prerequisites and known risks". Part 1 does not edit it.

## Integration boundary and Part 2 sequencing (DEFERRED)

Part 2 is explicitly **DEFERRED** to a separate approved packet after Part 1 review. Before or at combination, the integrator resolves the NaturalProcesses test-project glob (spec.md, "Integration prerequisites and known risks"). In Part 2:

1. Register `SenescenceStreams.OldAgeTreePrefix` (`"SENESCENCE-OLD-AGE-v1-"`) in `RngSalts.cs` and pin equality in `Core.Tests`.
2. Wire `SenescenceEngine.Evaluate` into `AnnualPipeline.cs` after density/suppression and storm mortality, feeding `SenescenceDeathRecord` into the existing biological-death / fallen-deadwood / crop-tree-mark-clearing pathway (never `Fell()`).
3. Wire `LateLifeGrowth.EvaluateGrowth` into live Wave 2 growth execution for Scots pine, Common alder, and Downy birch while leaving Sitka, Oak and Beech growth untouched.
4. Add Version 3 six-species deadwood save validation and persist `SenescenceModelVersion` (defaulting legacy/missing/Version 2 saves to `Disabled` so all nine frozen baselines and Version 2 Phase1 remain untouched).
5. Surface `"Died of old age"` in History and Habitat UI without exposing calibration tags or probabilities.
