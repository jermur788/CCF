# CCF Multi 2D — Phase 2A: Forest Maker, Save Schema v3 & Multi-Lane View — Implementation Plan

- **Spec ID:** `SPEC-multi-2d-phase2a-forest-maker-V1`
- **Spec status:** `DRAFT` (awaiting owner approval)
- **Plan owner:** to be assigned at D-091 admission (one single-writer lane per step)
- **Docs base commit:** `5fe2fabe9073d7b72711f4ab48553c785351d7bc`
- **Implementation base:** accepted Phase 1 Step 2 HEAD (`feat/multi-2d-phase1-time-history-ui-20261125-v1`). Q7 in the handoff covers an earlier 2A-1 start.
- **Created:** 2026-11-25

---

## 1. Delivery shape

There are two lanes, **2A-1 before 2A-2**, and each gets its own D-091 admission, branch, independent review and owner acceptance.

| Step | Suggested branch | Touches | Must not touch |
|---|---|---|---|
| 2A-1 Core | `feat/multi-2d-phase2a1-forest-maker-core-<date>-v1` | 2 new Core files, 1 additive line set in `MultiAreaExperiment.cs`, 1 new test file + registration | `src/Ui/**`, `src/Core/Phase1/**`, biology/RNG, `MultiAreaGenerator.cs`, `MultiAreaEcology.cs`, `MultiAreaContinuation.cs` |
| 2A-2 UI/v3/speed | `feat/multi-2d-phase2a2-forest-maker-ui-v3-<date>-v1` (from accepted 2A-1) | `src/Core/Phase1/{Phase1SaveFormat,Phase1SaveEnvelope,Phase1SaveCodec,Phase1SaveValidator}.cs`, `MultiAreaContinuation.cs` (badge + v3 capture/expectation, additive), `MultiAreaExperiment.cs` (`BadgeOverride`), new `src/Ui/MultiSessionHost.cs`, `src/Ui/{PlayerApi,PlayerMultiApi,Program}.cs`, page assets, tests | biology/RNG, `Phase1SaveSlots.cs` (byte-agnostic, stays unchanged), `Phase1RootMapping.cs` unless strictly needed and justified |

---

## 2. Step 2A-1 design

### 2.1 `ForestMakerRecipe.cs`
- A `sealed record ForestMakerRecipe` with the §5.1 fields, plus a `ForestMakerSpeciesMix` enum (`PureSitka = 0`, `SitkaPlusEdgeBroadleaves = 1`, `CustomSplit = 2`).
- A `static class ForestMakerBounds` holding every bound as a named `const` (`MinSideM = 30`, `MaxSideM = 120`, `MinLanes = 2`, `MaxLanes = 8`, `MinAgeYears = 10`, `MaxAgeYears = 60`, `MinStemsPerHa = 500`, `MaxStemsPerHa = 3_000`, `MaxLivingStems = 3_000`, `MinSeed = 1`, `MaxSeed = 100_000_000`, `EdgeInsetMm = 1_250`, `ExclusionRadiusMm = 2_000`, `EdgeMarginMm = 500`, `EdgeBroadleafAgeYears = 60`).
- `RequireValid()` runs the refusal order from spec §5.1. `TotalStems()` is integer half-up. `CanonicalBytes()` uses the existing `CanonicalWriter`. `RecipeHash()` uses the existing `DerivedManifest.Sha256Hex`. `static ForestMakerRecipe Decode(JNode)` uses the existing `StrictJson` / `J` helpers and refuses on the canonical round-trip.
- `static class ForestMakerPresets` holds the two canonical recipes and `ByPresetId(string)`.

### 2.2 `ForestMakerGenerator.cs`
- `internal static ForestMakerResult Generate(ForestMakerRecipe recipe)` → `(MultiAreaExperiment Experiment, Phase2Options Options, string RecipeHash, string ContentHash, IReadOnlyList<ForestMakerLaneSummary> Lanes)`.
- Private steps match spec §5.2 rules 1–10, each its own small method (learner-readable): `LaneRectangles`, `LaneQuotas`, `EdgeBroadleaves`, `SitkaCandidates`, `SelectExact`, `StartingSize`, `AssignIds`, `RequireGridAlignment`, `LaneSeeds`.
- Integer-only SplitMix64 is a private copy (about 10 lines) seeded from `recipe.Seed`. It does not reuse `MultiAreaGenerator.Next`, whose seed is the fixed MA-1 constant.
- Sets `DisclosureOverride` (spec §5.3 text) and the new `GeneratorNoteOverride` (`"Designed in code by the CCF 2D forest maker v1 from a validated recipe. Not derived from the P1 30 m fixture, any P0 fixture or the Unity Enlarged80 lattice."`).

### 2.3 `MultiAreaExperiment.cs` (additive)
- `internal string? GeneratorNoteOverride { get; init; }`. The `Write` method's `generator.note` uses `GeneratorNoteOverride ?? <existing literal>`. MA-1 and MA-1-MIXED leave it null, so their bytes and hashes are unchanged (anchors 1–9 and the MA-1 manifest test prove this).

### 2.4 Tests (`tests/Core.Tests/ForestMakerTests.cs`)
- `P2A-01` bound and refusal matrix, plus a **corner matrix**: every combination of {min, max} for width, length, lanes, age and density, with two species mixes. Each case either generates and validates, or refuses with a typed `P1Refusal`. No other exception type escapes.
- `P2A-02` determinism (two calls → identical `ManifestFileBytes`), seed sensitivity, validity, margins, ID order.
- `P2A-03` / `P2A-04` exact preset assertions. Capture both preset content hashes and add them as anchor constants 10 and 11.
- `P2A-05` constants-driven assertions (read `Phase2Species.Oak.MaturityFullYears` etc.; no literals).
- `P2A-06` builds `new MultiAreaContinuation(experiment, MultiAreaModelSelection.Phase2, null, null, options)` and asserts (a) Hegyi CI via `CrownModel.WoodlandCompetition` over woodland vs lane-only, (b) per-lane RD denominator, (c) an MA-B oak/beech seed-input diagnostic > 0 within 5 years (primary) and = 0 everywhere (pure Sitka), (d) 15 annual advances without refusal.

---

## 3. Step 2A-2 design

### 3.1 Save Schema v3 (`src/Core/Phase1/`)
- `Phase1SaveFormat`: keep `SchemaVersion = 2` (v2) and add `SchemaVersionV3 = 3` and `RecipeGeneratorNote`.
- `Phase1SaveEnvelope`: `Phase1SaveHeader` gains `Phase1Recipe? Recipe` **as the last positional parameter with default null**, so all existing constructors and tests compile unchanged. `Phase1Recipe` is a public DTO mirroring `ForestMakerRecipe` plus `RecipeHash`.
- `Phase1SaveCodec`: `RequireFormatAndSchema` accepts 2 or 3. `EnvelopeJson` writes the header's version: 2 when `Recipe` is null, 3 otherwise. `HeaderJson` appends `recipe` only for v3. `ReadHeader` requires `recipe` iff schema 3 and refuses it on 2 (`SaveNotCanonical`). The v2 code path and messages stay byte-identical. `RequireExpected` is unchanged for v2. For v3 there is a new `RequireRecipeGenesis(header)` that runs spec §6.3 steps 2–4.
- `Phase1SaveValidator`: v3 consistency (recipe ↔ genesis IDs, seeds, area count = lane count).
- **v2 regression proof:** encode every existing `Samples.*` save before and after and compare SHA-256. The 50 existing tests run untouched.

### 3.2 Session host (`src/Ui/MultiSessionHost.cs`, new, ~150 lines)
- One `sealed class MultiSessionHost` holding an immutable `Current` record `(string ExperimentKey, ForestMakerRecipe? Recipe, MultiAreaContinuation Session, MultiAreaWorkPlanAdapter Adapter, string Badge, (int, int, int) PersistedMarker)`.
- `Replace(Func<Current> buildCandidate, MultiAreaFence fence, bool confirmDiscardUnsaved)`: under one host `lock`, check the fence against the current session, check unsaved progress against the marker, build the candidate, then swap the reference. Refusal → unchanged.
- `PlayerApi` / `PlayerMultiApi` read `host.Current` **once per request** and use that snapshot for the whole request. `Program.OpenMultiExperiment` builds the initial `Current`, so the `--multi-experiment` flag keeps working.

### 3.3 Routes and page
- Add the three `api/multi/woodland/*` routes (spec §7.2) to the existing route table and member allow-lists. Extend `api/multi/load` for v3.
- Page: a New woodland button + dialog inside the existing inline page / `wwwroot` assets, lane-strip summary when areas ≥ 3, an active-lane viewport (filter by `areaId` from the existing scene data, with no geometry derived client-side beyond scaling the Core coordinates for SVG), a mini-map, and a 1-lane toggle. Two-area layout code paths stay unchanged.

### 3.4 Payload and speed
- **M0 first:** add a measurement harness (Core test + browser script reusing the existing headless Edge / CDP lab recipe) **before** touching the budget code. Record the M0 table in the handoff.
- Replace the silent final fallback in the bounded view builder with: if the final candidate exceeds `budget` → throw a typed `MultiViewPayloadBudgetExceeded` → HTTP 503 JSON. The budget becomes `MultiViewBudget.For(areaCount, genesisStems)` per spec §8. Add a test seam to inject a tiny budget.
- The static/dynamic split happens only on the M0 evidence route (spec §8). Otherwise it is not built.

---

## 4. Verification commands (implementer runs them; this draft does not)

```
cd Tools/MultiRackSitkaP1Lab
dotnet build SitkaP1.sln -warnaserror
dotnet build src/Ui/SitkaP1.Ui.csproj -warnaserror
dotnet build tests/Phase1.Save.Tests/Phase1Save.Tests.csproj -warnaserror
dotnet tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests.dll
dotnet run --project tests/Phase1.Save.Tests/Phase1Save.Tests.csproj
sha256sum .p0ref/SitkaLab.Core.dll
bash tests/ui-smoke-multiarea.sh            # 2A-2 only; ephemeral port, never 33767/43649/46593
ss -tlnp | grep -E ':(33767|43649|46593)\b' # before and after; listeners unchanged
git diff --stat <base>..HEAD -- Assets Packages ProjectSettings Docs/Project/coordination   # must be empty
```

## 5. Risks

| Risk | Mitigation |
|---|---|
| Starting sizes read as a yield claim | Disclosure text plus TRANSFER / RESEARCH REQUIRED tags (spec §5.3); Q1/Q2 owner choice |
| 5-lane payload ≈ 4–5× MA-1 | M0 measurement, scaled budget, typed refusal instead of a silent overflow |
| v3 accidentally changes v2 bytes | v2 written only when `Recipe` is null, plus a before/after SHA-256 regression on all samples |
| Race during host swap | Single host lock, candidate built before the swap, per-request snapshot, fresh session ID |
| Lane boundary on the light-cell lattice drops cells | `ForestMakerLaneGridAlignment` refusal (spec §5.2 rule 7) |
