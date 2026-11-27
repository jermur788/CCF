# CCF Multi 2D — Phase 2A: Forest Maker, Five-Lane 0.64 ha Presets, Save Schema v3 & Multi-Lane View — Specification

- **Spec ID:** `SPEC-multi-2d-phase2a-forest-maker-V1`
- **Feature ID:** `MULTI-2D-PHASE2A-FOREST-MAKER-V1`
- **Status:** `APPROVED` (owner approved 2026-11-26; Phase 2B-1 already merged on `integration/2d-sim` at `6d72c30538b1fbe30d3cbcf083b5b368733662b8`)
- **Owner:** Project owner (product decisions). Spec author: Claude Opus 5.5 (docs-only lane). Implementer(s): Phase 2A-1 admitted under D-091 (`feat/multi-2d-phase2a1-forest-maker-core-20261126-v1`); Phase 2A-2 to be admitted separately under D-091.
- **Created:** 2026-11-25
- **Last updated:** 2026-11-26
- **Docs base commit:** `5fe2fabe9073d7b72711f4ab48553c785351d7bc` (`feat/multi-2d-phase1-time-history-core-20261124-v1`, approved Phase 1 Step 1 Core)
- **Docs branch:** `docs/multi-2d-phase2a-forest-maker-spec-20261125-v1`
- **Implementation base (Step 2A-1):** `6d72c30538b1fbe30d3cbcf083b5b368733662b8` (`origin/integration/2d-sim`, incorporating accepted Phase 1 Step 2, S0a/S0b/S0c, and Phase 2B-1). Step 2A-2 will branch from accepted Step 2A-1 HEAD.
- **Decision log references:** D-020 (presentation authority: no frontend generator or validation math), D-030 (state honesty), D-062 (broadleaf economy, implemented separately in Phase 2B-1 and already merged at `6d72c305`), D-063 (selective SDD), D-091 (separate coding admission per lane).
- **Milestone:** MultiRack Sitka 2D standalone simulator (`Tools/MultiRackSitkaP1Lab`), Phase 2A.

---

## 1. Authority

Owner approved on 2026-11-26; this specification is task-level authority for Phase 2A only. It stays subordinate to the latest direct owner instruction, `Docs/Project/decision-log.md`, `Docs/Project/current-milestone.md` and `Docs/Project/game-brief.md`. Implementation truth is the inspected repository. A specification states intended behaviour and acceptance criteria. It never proves runtime verification or acceptance.

---

## 2. Product intent

Today the 2D simulator always opens one of two fixed two-area experiments: `MA-1` (`mad-sitka-ma1-two-area-v1`, about 0.24 ha) or `MA-1-MIXED` (`mad-ma1-mixed-demo-v1`). The choice is made by a command-line flag (`Program.cs` `--multi-experiment=ma-1|ma-1-mixed`). The session is fixed for the life of the process, and a save loads only into a session with the identical genesis (`Phase1SaveCodec.RequireExpected` → `SaveGenesisMismatch`).

Phase 2A lets the player start a **new woodland** from inside `/multi`:

- **Step 2A-1 (Core only):** a deterministic, parameterised **rectangular forest maker** in `SitkaP1.Core`. It turns a small validated recipe into an ordinary `MultiAreaExperiment` plus `Phase2Options`, which the unchanged `MultiAreaContinuation` then runs. It also defines two canonical **0.64 ha five-lane benchmark presets**. Step 2A-1 changes no UI and no save schema.
- **Step 2A-2 (UI, Save Schema v3, speed):** a lightweight **New Woodland** modal, guarded atomic replacement of the host session, self-contained **Save Schema v3** for generated stands with full v2 compatibility, an **Active Lane + Visible Neighbours** stand view with a whole-woodland mini-map and a 1-lane toggle, and measured payload and browser speed qualification at 1,344 trees / 1,024 cells of 2.5 m.

All ecology stays the existing Phase 2 (`1/1/1/1/1`) engine. The forest maker only decides **where the starting stems stand and what they start as**.

---

## 3. Out of scope and strict prohibitions (both steps)

- **No biology changes.** The model vector stays exactly `1/1/1/1/1`. There are zero edits to `AnnualRules`, `CrownModel`, `Stage4*`, `Stage5*`, `Phase2*` rules or constants, `SitkaGrowthModel`, `SimulationRandom`, RNG stream keys, `.p0ref/SitkaLab.Core.dll` (`9a99f781…34b3`), or the existing `MultiAreaGenerator` / `MultiAreaDemoExperiments` outputs.
- **No change to MA-1 or MA-1-MIXED bytes.** Their manifests, content hashes, badges, disclosures and v2 save bytes stay identical. All **9 frozen SHA-256 anchors** stay exact (§10).
- **No Unity changes:** `Assets/` (including `ForestStartingStand.cs`, read only as a parameter source), `Compact30`, `Legacy40`, `Enlarged80`, Scenario One Reference Future v1, Unity `ForestSaveData`, `Packages/`, `ProjectSettings/`.
- **No protected-port use:** never bind, stop or probe-mutate `127.0.0.1:33767`, `:43649` or `:46593`.
- **Not copied from 3D:** Legacy40 tree IDs (`Pxxyy`, `POxxyy`), the `±19.4 m` clamp bands, road / headland / work-area / clearance cutouts, the centred `[-40, 40]` frame, and `FnvJitter` / `FnvHash`.
- **Phase 2B (excluded):** economy, money, prices, labour, D-062 timber assortments, `€` fields or placeholders.
- **Phase 3 (excluded):** jay acorn dispersal, irregular or non-rectangular stand shapes, deer or moisture sliders, storms or windthrow, Scots pine / downy birch / common alder.
- **Phase 4 (excluded):** MapLibre, drone or LiDAR imports. **Phase 5 (excluded):** Untouched Twin, Strategy A vs B comparator.
- **No new packages, projects, DI containers, service locators or frontend generator/validation math (D-020).** The browser only sends recipe fields and renders what Core returns.
- **No edits to** `Docs/Project/coordination/**`, `main`, tags or `.vscode/`, and no `git` history rewrites.

---

## 4. Coordinate frame and lane model (both steps)

The forest maker uses the existing multi-area frame (`MultiAreaIdentity.LocalFrame`): integer millimetres, **+X east, +Y north**, engine `XM = xMm/1000`, `ZM = yMm/1000`. The owner packet's "Z" is this **Y** axis. The frame is nonnegative: the woodland rectangle is `X ∈ [0, W]`, `Y ∈ [0, L]`.

- **Lanes** are `N` equal, parallel, edge-to-edge strips running the full length `L`, split along X. Lane `i` (0-based) is the strictly convex counter-clockwise rectangle `(x_i,0) → (x_{i+1},0) → (x_{i+1},L) → (x_i,L)` with `x_i = floor(i · W_mm / N)` in integer mm. There are no gaps, roads or headlands. Every boundary is shared, so `MultiAreaGeometry.Overlap` is false and touching is allowed.
- **Lane IDs** are `MA-A`, `MA-B`, … `MA-H` in ordinal order (matches `MultiAreaExperiment.AreaIdPattern`). Labels are `Lane A` … `Lane H`.
- **Outer perimeter** borders open grassland with no synthetic neighbours, which is the existing connected-woodland rule (`EdgeModelStage4`).
- **Ground cells** use the existing world-anchored lattice (`CanopyLightEvaluator`: cells anchored at the origin, a cell belongs to the area whose polygon strictly contains its centre). For the 80 × 80 m presets this gives **32 × 32 = 1,024 cells of 2.5 m**. Lane boundaries at 16/32/48/64 m are not on the 2.5 m lattice, so each lane holds 6, 7, 6, 7 and 6 cell columns: **192 / 224 / 192 / 224 / 192 cells** (lane areas are equal; cell-centre counts are not). This is a property of the existing evaluator, reported here so the UI does not imply equal cell counts.

---

## 5. Step 2A-1 — Core forest maker

### 5.1 Recipe (`ForestMakerRecipe`)

An immutable record. It is the **only** input to generation and it is what a v3 save stores.

| Field | Type | Bound (Core-enforced) |
|---|---|---|
| `GeneratorId` | string | exactly `ccf-2d-forest-maker` |
| `GeneratorVersion` | int | exactly `1` (only supported version) |
| `PresetId` | string or null | null for a custom stand, otherwise one of §5.4 |
| `WidthM` | int (whole metres, X) | `30 … 120` |
| `LengthM` | int (whole metres, Y) | `30 … 120` |
| `LaneCount` | int | `2 … 8` |
| `StartAgeYears` | int | `10 … 60` (Sitka and custom-split cohort; edge broadleaves are fixed at 60, §5.3) |
| `StemsPerHa` | int | `500 … 3,000` |
| `Seed` | int | `1 … 100,000,000` |
| `SpeciesMix` | enum | `PureSitka` \| `SitkaPlusEdgeBroadleaves` \| `CustomSplit` |
| `OakPercent`, `BeechPercent` | int | `0` unless `CustomSplit`; then each `0 … 100`, `Oak + Beech ≤ 100`, Sitka = remainder |

**Derived total:** `TotalStems = (StemsPerHa · W_mm · L_mm + 5·10⁹) / 10¹⁰` (integer, round half up). For 80 × 80 m at 2,100/ha this is exactly 1,344.

**Hard cap:** `MaxLivingStems = 3,000` (exact integer constant). `TotalStems > 3,000` → `P1Refusal("ForestMakerStemCapExceeded")` with the computed count in the message. Example: 120 × 120 m at 3,000/ha = 4,320 is refused.

**Typed refusals** (all `P1Refusal`, nothing coerced, checked in this order): `ForestMakerGeneratorUnsupported`, `ForestMakerPresetUnknown`, `ForestMakerPresetMismatch` (a preset ID whose fields differ from the canonical preset), `ForestMakerWidthOutOfRange`, `ForestMakerLengthOutOfRange`, `ForestMakerLaneCountOutOfRange`, `ForestMakerAgeOutOfRange`, `ForestMakerDensityOutOfRange`, `ForestMakerSeedOutOfRange`, `ForestMakerSpeciesMixInvalid`, `ForestMakerStemCapExceeded`, `ForestMakerLaneGridAlignment` (§5.2 rule 7), `ForestMakerLaneEmpty` (a lane quota of 0), `ForestMakerInfeasibleStocking` (§5.2 rule 5).

**Canonical recipe bytes:** written with the existing `CanonicalWriter`, in the field order of the table above, with `presetId` as JSON `null` when absent. `RecipeHash = SHA-256(canonical recipe bytes)` in lowercase hex. A recipe whose decoded value does not re-encode to the identical bytes is refused (`ForestMakerRecipeNotCanonical`).

### 5.2 Generation algorithm (`ForestMakerGenerator.Generate(recipe)`)

The algorithm is deterministic and uses integers only, so the output bytes never depend on floating-point evaluation. Randomness is SplitMix64 over `(Seed, laneIndex, cellIndex, salt)`, the same mixing structure as `MultiAreaGenerator.Next` but seeded from the recipe. It is not the engine's mortality RNG.

1. **Lanes:** compute rectangles per §4.
2. **Lane quotas:** split `TotalStems` across lanes by largest remainder on lane width (`quota_i = TotalStems · w_i / W_mm`). Floors come first. The remainder goes to the largest fractional parts, ties to the lower lane index. For the presets: `269, 269, 269, 269, 268`.
3. **Edge broadleaves (only `SitkaPlusEdgeBroadleaves`):** 4 per outer edge (8 total) are placed **first**. On the west edge, `x = 1,250 mm` (inside `MA-A`); on the east edge, `x = W_mm − 1,250 mm` (inside the last lane). Each edge has `y = L_mm · (2k+1) / 8` for `k = 0..3`. Species alternate south→north: west `Oak, Beech, Oak, Beech`; east `Beech, Oak, Beech, Oak` (4 Oak + 4 Beech). They count inside their lane's quota. Middle lanes start pure Sitka.
4. **Sitka lattice per lane:** `cols = max(1, isqrt(quota_i · w_i / L_mm))`, `rows = ceil(quota_i / cols)`. Pitches are `pX = w_i / cols` and `pY = L_mm / rows` (integer mm), centred with half-pitch inset. Jitter per axis is `±floor(pX/5)` and `±floor(pY/5)`. A slot is a candidate only if its jittered point is (a) at least `ExclusionRadiusMm = 2,000 mm` from every edge broadleaf, and (b) at least `EdgeMarginMm = 500 mm` inside its lane (`MultiAreaGeometry.InsideWithMargin`).
5. **Exact count:** keep the `quota_i − broadleaves_i` candidates with the smallest SplitMix key (salt `90`). This follows the "omit lowest hash" idea of the 3D generator but uses an independent hash. If there are too few candidates, add one row and retry, up to `rows · 2`. If it is still short → `ForestMakerInfeasibleStocking`.
6. **Starting attributes:** see §5.3. **Tree IDs:** within each lane, sort stems by `(yMm, xMm)` ascending and name them `<laneId>-<4-digit>` (`MA-A-0001`). Four digits keep ordinal order valid up to 9,999 per lane. Three digits (`D3`, as in MA-1) would break `TreeIdOrder` above 999 stems.
7. **Grid alignment guard:** refuse `ForestMakerLaneGridAlignment` if any interior lane boundary `x_i` lies on a 2.5 m light-cell centre line (`(x_i − 1,250) mod 2,500 = 0`) or a 0.25 m micro-cell centre line (`(x_i − 125) mod 250 = 0`). The existing evaluator's strict-inside rule would drop such a cell column from **both** lanes. Example: W = 30, N = 8 puts a boundary at 3,750 mm, so it is refused. With whole-metre W and N ≤ 8, the outer edges `0` and `W_mm` can never hit a centre line.
8. **Lane seeds:** `SimulationSeed_i = Seed · 10 + i + 1`. These are unique, never equal to `Seed`, and fit `int`. `GeneratorSeed = Seed`. The Stage 5 mast seed keeps its existing default (`(int)GeneratorSeed`).
9. **Experiment ID:** preset → its fixed ID (§5.4). Custom → `mad-fm-custom-` plus the first 16 hex characters of `RecipeHash` (matches `SafeId`, ≤ 64 characters).
10. **Output:** `MultiAreaExperiment` (validated by the unchanged `Validate()`), `Phase2Options` with `GenesisSpecies` holding the broadleaf TreeIds (default mast seeds), and a `ForestMakerResult` containing the recipe, `RecipeHash`, manifest `ContentHash`, per-lane stem and species counts, and the area in hectares. Two calls with equal recipes return identical manifest bytes.

### 5.3 Starting attributes: existing values only, with research flags

- **Sitka at age 20 (both presets):** these values transfer the Unity `Enlarged80` start parameters at `main` (`ForestStartingStand.cs`: `canonicalAgeYears = 20`, DBH range 8–24 cm, height `2 + 0.62·DBH(cm)` m, minimum 3.5 m; `StandGeometryModel.StartingStemsPerHectare = 2100`). Integer form: `DBH_mm = clamp(160 + (S − 2000) · 40 / 1000, 80, 240)`, with `S` the sum of four uniforms in `[0, 1000]` (the MA-1 Irwin–Hall shape). `Height_mm = max(3,500, 2,000 + 62 · DBH_mm)`. Tagged **TRANSFER / GAMEPLAY CALIBRATION** (the shape is simplified, the vigour waves and neighbour classes are dropped, and it is not Unity parity).
- **Edge broadleaves at age 60:** the existing Core has **no species-specific genesis DBH/height relation**. The only precedent is `MultiAreaDemoExperiments.CreateMixedDemo`, which re-ages MA-1 Area B stems to 60 and keeps their Sitka-derived sizes. Accepted default (**Q2**): reuse the MA-1 Area B older cohort exactly (`mean 345 mm, spread 80, clamp 220–480 mm, height 27,200 + 20·(DBH − 345) mm`). It is tagged **TRANSFER from existing synthetic fixture / RESEARCH REQUIRED**.
- **Derived from existing species constants, unchanged:** the genesis crown radius comes from the existing Phase 2 path (`MultiAreaContinuation` → `Phase2GenesisCrowns` → `Phase2Crown.TargetRadiusM(species, DBH, CI₀)` over the connected woodland). Mast and seed eligibility: `Phase2Rules.Maturity(species, 60) = 1.0`, because `MaturityOnsetYears = 40` and `MaturityFullYears = 60` for both `Phase2Species.Oak` and `Phase2Species.Beech`. So the edge broadleaves are fully seed-bearing from Year 0. Sitka at age 20 has `Stage5SeedRain.Maturity(20) = 0` (onset 20, full 30), so it does not yet seed. Tests must assert both values from the constants, not from literals.
- **Custom ages other than 20:** no age→size model exists in Core. Accepted default (**Q1**): a single species-neutral piecewise-linear table of mean DBH by age, `10→70, 20→160, 30→240, 40→300, 47→345, 60→400 mm`, with spread `mean/4`, clamp `[mean/2, 3·mean/2]`, and the same height rule. The 20 and 47 anchors are TRANSFER (`Enlarged80`, MA-1 Area B). The other anchors are **GAMEPLAY CALIBRATION — RESEARCH REQUIRED** and are not a yield-table claim. Stocking density does not change starting size, and this is disclosed. `CustomSplit` broadleaves share the recipe cohort age and use the same table (MA-1-MIXED precedent).
- **Disclosure** (manifest `disclosure` via `DisclosureOverride`) for every forest-maker stand: `"SYNTHETIC FOREST-MAKER STAND. NOT A SURVEYED IRISH FOREST. SITE UNKNOWN. STARTING SIZES ARE TRANSFERRED/CALIBRATED GAMEPLAY VALUES, NOT A YIELD TABLE. CONNECTED WOODLAND: GROWTH, CROWNS AND SEED RAIN USE ALL LANES; DENSITY MORTALITY, WORK PLANS AND SUMMARIES STAY PER LANE. NOT UNITY NUMERICAL OR GEOMETRY PARITY."`

### 5.4 Canonical 0.64 ha five-lane presets

Both presets are 80 × 80 m, 5 lanes of 16 × 80 m (`0.128 ha` each, X boundaries `0/16/32/48/64/80 m`), age 20, 2,100 stems/ha, **1,344 living stems** and **1,024 cells of 2.5 m**.

| Preset ID (`PresetId` / experiment ID) | Mix | Seed | Composition |
|---|---|---|---|
| `fm-064-sitka-edge-broadleaves-v1` / `mad-fm-064-sitka-edge-broadleaves-v1` | `SitkaPlusEdgeBroadleaves` | `6,400,101` | 1,336 Sitka (age 20) + 4 sessile oak + 4 European beech (age 60). Per lane: A 265 S + 2 O + 2 B; B, C, D 269 S; E 264 S + 2 O + 2 B |
| `fm-064-pure-sitka-v1` / `mad-fm-064-pure-sitka-v1` | `PureSitka` | `6,400,202` | 1,344 Sitka (age 20). Per lane: 269, 269, 269, 269, 268 |

Badge text (UI, Step 2A-2): `"Forest maker — 0.64 ha, 5 lanes, Sitka + edge broadleaves (synthetic, not a surveyed Irish forecast)"` and `"Forest maker — 0.64 ha, 5 lanes, pure Sitka (synthetic, not a surveyed Irish forecast)"`. The SHA-256 content hash of each preset manifest is **captured once in 2A-1 and frozen as a new anchor** (anchors 10 and 11, additive to the existing 9).

### 5.5 Step 2A-1 file scope

- **New:** `src/Core/MultiArea/ForestMaker/ForestMakerRecipe.cs`, `src/Core/MultiArea/ForestMaker/ForestMakerGenerator.cs`, `tests/Core.Tests/ForestMakerTests.cs`.
- **Edited (additive, null-preserving):** `src/Core/MultiArea/MultiAreaExperiment.cs` gains `GeneratorNoteOverride { get; init; }`, because the current manifest note claims "Designed in code by the MA-1 author". When null, the MA-1 bytes are unchanged. `src/Core/RngSalts.cs` registers the new Forest Maker RNG prefix in `Families` without touching any existing salt value. `tests/Core.Tests/Program.cs` gets suite registration only.
- **Untouched:** everything under `src/Ui/**`, `src/Core/Phase1/**`, every biology file, `MultiAreaGenerator.cs`, `MultiAreaEcology.cs` (`MultiAreaDemoExperiments`), `MultiAreaContinuation.cs`, and Phase 2B-1 economy math.

---

## 6. Step 2A-2 — Save Schema v3

### 6.1 Versioning rule

- `Phase1SaveFormat` keeps `SchemaVersion = 2` **as the v2 constant** and adds `SchemaVersionV3 = 3`. The codec reads **2 or 3** and refuses anything else with the existing `SaveSchemaUnsupported`.
- **Sessions from MA-1 and MA-1-MIXED keep writing v2, byte for byte.** Only forest-maker sessions write v3. All 50 existing `Phase1.Save.Tests` cases pass unchanged.
- The v2 decode path, its canonical-bytes rule, checksum, validator, `RequireExpected` refusal semantics and messages, atomic slot writes (`Phase1SaveSlots`, unchanged) and fresh-session-ID minting on load are all preserved.

### 6.2 v3 header

The header is the v2 header plus one required member `recipe` placed after `lineage`. It holds the canonical recipe object (§5.1), `recipeHash`, and `generatorNote: "ccf-2d-forest-maker v1"`. `genesis.experimentId`, `genesis.manifestContentHash`, `genesis.generatorId` (`ccf-2d-forest-maker`), `generatorVersion` and `generatorSeed` must agree with the recipe. The checksum keeps covering `header || payload`, so a recipe cannot be swapped onto another payload. The v3 payload shape is identical to v2. The save stores the **recipe plus the expected manifest content hash**, not the full manifest; the trees are already in the payload.

### 6.3 v3 load (self-contained genesis)

Steps run in this order, and any failure is a typed refusal with **zero change** to the running session:

1. Decode with every v2 gate (size, UTF-8, strict JSON, canonical bytes, checksum, validator).
2. Validate the recipe (§5.1 bounds; an unknown generator or version → `SaveRecipeUnsupported`). Check that the recipe re-encodes canonically and that `SHA-256` matches `recipeHash` (`SaveRecipeHashMismatch`).
3. Regenerate the experiment from the recipe and require `ContentHash() == genesis.manifestContentHash` and equal areas, seeds and hectares (`SaveRecipeGenesisMismatch`).
4. Require the model vector `1/1/1/1/1` and the current engine lineage (`SaveModelMismatch` / `SaveLineageMismatch`, existing messages).
5. Build a **fresh** `MultiAreaContinuation` from the regenerated experiment and options, then run the existing `Restore` checks against it (options equality, legal restored orders, approval → Pending draft).
6. Only then swap the host session atomically (§7.3). A fresh session ID is minted, and old fences, Work Plan, history caches and canopy/ground caches belong to the discarded session.

A v3 file in a build without v3 support is refused by the existing `SaveSchemaUnsupported`. A v2 save while a forest-maker session is running keeps the existing `SaveGenesisMismatch` (Open Question **Q4**).

---

## 7. Step 2A-2 — `/multi` UI and host session replacement

### 7.1 New Woodland modal

- A **New woodland** button sits beside **Saved games** in the existing header `top-actions`. There is no fourth permanent layout column, and the `1280×720` and `1920×1080` gates stay as they are.
- A dialog (`role="dialog"`, focus trap, Esc closes, returns focus) lists: `MA-1` (all Sitka, 2 areas), `MA-1-MIXED` (2 areas), the two 0.64 ha presets, and **Custom rectangle**.
- The custom fields are width, length, lanes, start age, stems/ha, seed and species mix (with oak/beech % for `CustomSplit`). Any field change calls `POST api/multi/woodland/preview`. Core returns either the derived total stems, per-lane counts, area, cell count, the experiment ID and the research-flag text, or the typed refusal. The browser does **no** bound, count or geometry math (D-020). It shows the Core refusal message verbatim.
- **Confirm:** when the current session has progress since its genesis, last save or last load (the host tracks the `(AnnualRevision, WorkRevision, PlanStateRevision)` triple at those moments), the dialog shows an explicit **"Discard unsaved progress and start a new woodland?"** step. The request must carry `confirmDiscardUnsaved: true`. Otherwise it is refused (`WoodlandUnsavedProgress`, 409) and nothing changes.

### 7.2 Routes (capability-scoped, existing request rules)

- `GET api/multi/woodland/options`: the built-in list, preset recipes, bounds (sent for display only; Core still enforces them) and the current experiment ID.
- `POST api/multi/woodland/preview` `{recipe}`: read-only, zero mutation.
- `POST api/multi/woodland/new` `{fence, choice: "ma-1"|"ma-1-mixed"|"preset"|"custom", recipe?, confirmDiscardUnsaved}`: needs the **current** fence, so a stale tab cannot replace a newer session (`PlayerSessionMismatch` / `PlayerSourceStale` / `PlayerPlanStateStale`).
- `POST api/multi/load`: a v3 slot rebuilds and replaces per §6.3, behind the same unsaved-progress confirmation. A v2 slot keeps exact current behaviour.

### 7.3 Atomic host replacement

`PlayerApi` currently holds `multiArea` and `multiSession` as fixed constructor fields. Step 2A-2 introduces one small `MultiSessionHost` that holds `(experimentKey or recipe, MultiAreaContinuation, MultiAreaWorkPlanAdapter, badge, progress marker)` behind a single reference, swapped under one host lock. The candidate session is fully built **before** the swap. Any refusal leaves the old reference untouched. Requests already running against the old object finish against it, and their results carry the old session ID, so later fenced calls fail. There is no singleton, service locator or DI. `MultiAreaContinuation`'s badge selection (currently a hard-coded two-way choice) takes an optional badge from `MultiAreaExperiment` (`BadgeOverride { get; init; }`, null → existing behaviour).

### 7.4 Active Lane + Visible Neighbours view

- **Default view:** the active lane in full detail plus its immediate neighbours. That is **2 lanes side by side** when the active lane is an outer lane (`A`, or the last lane) and **3 lanes** for any middle lane. Lane borders are clearly drawn and labelled. Outside the outer perimeter is drawn as open grassland, never as trees.
- **Compact whole-woodland mini-map:** every lane outline, a low-detail stem/cover rendering from the same Core payload, the active lane highlighted, and click / Enter on a lane to make it active (keyboard reachable).
- **1-lane toggle:** shows only the active lane.
- **Two-area stands (`MA-1`, `MA-1-MIXED`)** keep their current layout and behaviour. With 2 areas, "active + neighbour" is the whole stand. The lane view and mini-map appear only when there are ≥ 3 areas.
- The summary area keeps one compact card per lane. With more than 2 areas the cards become a single-row lane strip, readable at 1280 px with up to 8 lanes. Header copy changes from "Both areas" to "All lanes" only when there are ≥ 3 areas.
- All hit-testing, Work Plan verbs, crop marks, cell detail and history keep using the existing Core routes and the area ID of the clicked object. The lane view is presentation only.

---

## 8. Step 2A-2 — Measured speed and payload

- **Existing gate kept:** `GET api/multi/view` raw UTF-8 ≤ **524,288 B (512 KiB)** on `MA-1` and `MA-1-MIXED` (current tests unchanged).
- **Silent-overflow gap fixed:** on the Step 2 branch, `PlayerMultiApi.cs` around L508–529 (the final `emptyBytes` / `trimmedBytes` fallback) can return a body **larger** than `MaxViewPayloadBytes` without saying so. After the fix, the server **never** returns a view body above the active budget. If every existing reduction still exceeds it, it returns a typed refusal: HTTP 503 `{"code":"MultiViewPayloadBudgetExceeded","bytes":<n>,"budget":<b>}`. The page shows an explicit banner and keeps the last good view. A test must force the path (tiny test budget) and assert the refusal.
- **Scaled budget for forest-maker stands:** `budget = 524,288 B` when there are ≤ 2 areas. Otherwise `budget = min(MaxScaledViewBytes, 524,288 × ceil(genesisStems / 300))`, where `MaxScaledViewBytes = 2,621,440 B (2.5 MiB)` (Open Question **Q5**). The final constant is **fixed only after measurement gate M0** (below). It may be lowered without owner approval but never raised.
- **Measurement gate M0 (must run before any optimisation):** on both 0.64 ha presets, at Year 0 and after 15 annual advances, record: Core `ReadCompositeUiSnapshot` ms (median of 5), raw view bytes, browser `JSON.parse` ms, SVG DOM build + first paint ms for the 3-lane and 1-lane views, and annual advance ms (median of 5). Record environment details (CPU, .NET version, browser build, headless or not). Use the recipes in the lab notes, never the protected ports.
- **Speed targets (to qualify, recommended defaults, Q5):** snapshot ≤ 250 ms, parse ≤ 150 ms, 3-lane render ≤ 500 ms, annual advance ≤ 2,000 ms, all measured at Year 15. Missing a target is **reported honestly**, not hidden.
- **Static/dynamic split:** only allowed if M0 shows the budget or targets fail with the existing compaction. It would mean sending immutable lane geometry and genesis-only fields once per session through a separate cached route. It needs a recorded M0 table in the handoff that justifies it.

---

## 9. Acceptance criteria

| ID | Step | Criterion |
|---|---|---|
| **P2A-01** | 2A-1 | Recipe bounds and typed refusals: every bound at min, max, min−1 and max+1. Exact cap 3,000 (3,000 accepted, 3,001 → `ForestMakerStemCapExceeded`). Each refusal code fires for its own reason, with no coercion. Recipe canonical bytes and `RecipeHash` are stable, and non-canonical input is refused. |
| **P2A-02** | 2A-1 | Determinism and validity: equal recipes → identical manifest bytes and content hash. A different seed → different positions, same counts. Every generated experiment passes the unchanged `MultiAreaExperiment.Validate()`. Every stem is strictly inside its lane and ≥ 500 mm from lane edges. No duplicate positions. Four-digit IDs are ordinal within each lane. Lane seeds are unique. |
| **P2A-03** | 2A-1 | Primary preset exact: 80 × 80 m, 5 lanes at x = 0/16/32/48/64/80 m, `0.128 ha` per lane (`AreaHa` from the existing expression), 1,344 living stems = 1,336 Sitka + 4 Oak + 4 Beech. Per-lane counts as §5.4. Broadleaf positions and species as §5.2 rule 3. Every Sitka is ≥ 2,000 mm from every broadleaf. Lanes B–D are pure Sitka. 1,024 cells of 2.5 m with 192/224/192/224/192 per lane. |
| **P2A-04** | 2A-1 | Pure-Sitka preset exact: 1,344 Sitka, per-lane 269/269/269/269/268, empty `GenesisSpecies`, 1,024 cells. Preset manifest hashes are captured as anchors 10 and 11. |
| **P2A-05** | 2A-1 | Species constants respected: broadleaf genesis crowns equal `Phase2Crown.TargetRadiusM` over the connected woodland. `Phase2Rules.Maturity(Oak/Beech, 60) == 1` and `Stage5SeedRain.Maturity(20) == 0`, both read from the constants. Starting DBH/height match §5.3 formulas. No biology file changed (diff check). |
| **P2A-06** | 2A-1 | Cross-lane ecology on 0.64 ha: (a) for a Sitka in MA-B within 8 m of the MA-A boundary, woodland Hegyi CI ≠ CI computed from MA-B trees alone. (b) Density mortality / RD stays per lane (each lane uses its own `0.128 ha`). (c) In the primary preset, at least one MA-B cell receives non-zero oak or beech seed input from an MA-A edge broadleaf within the first 5 years. In the pure-Sitka preset, oak/beech seed input is zero everywhere (control). (d) Both presets advance 15 years under `1/1/1/1/1` without refusal. |
| **P2A-07** | 2A-2 | Save Schema v3: a forest-maker session saves v3 with the §6.2 header. Save → load into a fresh process reproduces identical canonical bytes and an identical 5-year continuation. Refusals: corrupt recipe, recipe/hash mismatch, recipe/genesis mismatch, unsupported generator version, schema 4, edited payload — each typed, with zero mutation. MA-1 / MA-1-MIXED still write **v2** and their saves are byte-identical before and after. All 50 existing Save tests pass unchanged. |
| **P2A-08** | 2A-2 | Session replacement: New Woodland and v3 load are atomic. A refused or unconfirmed request leaves session ID, root, Work Plan and caches unchanged. A successful replacement mints a new session ID, so every old fence is refused (Execute/Advance replay included), and the Work Plan, history and canopy/ground caches belong to the new stand. A stale-fence request cannot replace the session. |
| **P2A-09** | 2A-2 | UI: the New Woodland button sits beside Saved games, with the modal accessible (focus trap, Esc, labels). The preview shows Core numbers and refusals verbatim, with zero client validation math (source grep). Active Lane + Neighbours shows 2 lanes at A/E and 3 at B/C/D, with a working mini-map and 1-lane toggle. Two-area layout is unchanged. The `1280×720` and `1920×1080` gates pass with the dialog closed and open. |
| **P2A-10** | 2A-2 | Speed and payload: M0 table recorded for both presets at Year 0 and Year 15. MA-1 / MA-1-MIXED still ≤ 512 KiB. 5-lane views ≤ the frozen scaled budget. The forced-overflow test returns `MultiViewPayloadBudgetExceeded`, never an over-budget body. Speed targets reported against Q5 values. |
| **P2A-REG** | both | Clean `0/0` builds (`.sln`, `SitkaP1.Ui.csproj`, `Phase1Save.Tests.csproj`). Full Core and Save suites green (baseline counts + new). The existing 9 frozen anchors are exact, plus anchors 10–11 from 2A-1 onward. `.p0ref` hash exact. Zero diff under `Assets/`, `Packages/`, `ProjectSettings/`, `Docs/Project/coordination/`. Protected listeners `:33767`, `:43649`, `:46593` untouched. 2A-1 additionally: zero diff under `src/Ui/**` and `src/Core/Phase1/**`. |

---

## 10. Frozen anchors (must stay exact)

1 Single-area Model 0 `7e3a2a86…1ece` · 2 Multi-area `0/0/0/0` `a692a6d5…233d` · 3 Crown C2 `814e8165…6287` · 4 Stage 4 `8a926776…87fb` · 5 Stage 5 `c4b670ed…dc35` · 6 Stage 5 5.0 m purity `c67edc6b…53d8` · 7 Phase 2 Q6 `776a68df…e22d` · 8 `.p0ref` `9a99f781…34b3` · 9 MA-1-MIXED 30-year `93e9abda…251f`. Full values: `Docs/Handoffs/MultiRack-Simulation-20261116/phase1-2d/PHASE1-2D-STEP1-TIME-HISTORY-CORE-NEON-IMPLEMENTATION-20261124.md` §5.

---

## 11. Stop conditions (implementer must stop and report)

1. Any frozen anchor changes, or MA-1 / MA-1-MIXED manifest or v2 save bytes change.
2. A preset cannot reach exactly 1,344 stems under §5.2 without changing the algorithm.
3. `MultiAreaExperiment.Validate()` or `MultiAreaContinuation` would need a behavioural (not additive) change.
4. v3 cannot be added without changing v2 decode bytes or messages.
5. M0 shows the 5-lane view cannot meet even the 2.5 MiB ceiling with existing compaction. In that case stop and present measurements before designing a static/dynamic split.
