# Natural Old-Age Tree Death and Smooth Late-Life Growth V1

- **Spec ID** `SPEC-multi-2d-senescence-late-growth-v1`
- **Status** APPROVED / IMPLEMENTING (Part 1); Part 2 DEFERRED
- **Owner** Overall Manager
- **Decision** Playable Milestone 8 Part 1 isolated engine packet (2026-11-28)
- **Task class** SPEC REQUIRED
- **Created** 2026-11-28
- **Last updated** 2026-11-28
- **Milestone** Playable Milestone 8 — Natural Old-Age Tree Death & Smooth Late-Life Growth, Part 1 (isolated engine)
- **Related evidence** `Docs/Project/decision-log.md` (D-030, D-050, D-059, D-062), `specs/multi-2d-birch-alder-v1/`, `Tools/MultiRackSitkaP1Lab/src/Core/SpeciesWave2/`
- **Supersedes / superseded by** none

## Product intent

Give the 2D continuous-cover forestry simulator two closely linked long-horizon ecological behaviours without distorting existing commercial-rotation baselines:

1. **Natural old-age tree death across all six species** — a modest, smooth, monotone non-decreasing residual annual mortality hazard layered on top of existing density/suppression and windthrow mortality. Pioneers (`common-alder`, `downy-birch`) begin senescing earlier than mid-longevity species (`sitka-spruce`, `european-beech`), which in turn senesce earlier than long-lived species (`scots-pine`, `sessile-oak`), but no species hits a birthday cliff and no pioneer cohort is wiped out automatically at commercial rotation age 60–70.
2. **Smooth late-life growth continuation for the three Wave 2 species** (`scots-pine`, `common-alder`, `downy-birch`) — when a stand passes the upper age of its existing Wave 2 calibration/teaching window (age 60 for Scots pine and Common alder; age 70 for Downy birch), potential diameter increment continues smoothly (`C0` and `C1`) with a gentle exponential taper toward zero rather than dropping abruptly to unavailable (`null`), while keeping the existing in-window values bit-exact and keeping crowding and site-moisture responses intact.

**Part 1 (this branch)** delivers the isolated, pure .NET 8 computation library and its standalone offline console test gate.

**Part 2 (explicitly DEFERRED)** will handle live integration under its own approved packet and gates, specifically:

- wiring natural old-age mortality into `AnnualPipeline.cs`;
- registering the senescence stream/salt constant(s) in `RngSalts.cs`;
- live Wave 2 late-life growth wiring;
- Version 3 six-species deadwood save validation (leaving Version 2 Phase1 untouched);
- History and Habitat UI presentation for old-age deaths and resulting deadwood.

## Problem

In long-running multi-species continuous-cover stands (100–200+ years), two structural gaps appear once thinning relieves self-thinning pressure and storms are absent or survived:

1. Without a residual old-age mortality process, thinned pioneers such as Common alder and Downy birch live indefinitely alongside Sessile oak and Scots pine, flattening a core ecological distinction between short-lived pioneers and long-lived climax/canopy trees.
2. The existing Wave 2 growth evaluators (`ScotsPineEngine.EvaluateGrowth` and `BirchAlderEngine.EvaluateGrowth`) return `null` as soon as stand age exceeds their upper calibration/teaching window boundary (60 or 70 years), which prevents continuous multi-century evaluation of older pine, birch and alder stems.

At the same time, D-059 forbids inventing pseudo-empirical Irish mortality or late-life yield curves, and frozen baselines / legacy saves must remain byte-identical until a woodland explicitly enables the new model version.

## Goals

- Define an independent, smooth, non-decreasing residual annual old-age hazard curve for each of the six species using `SpeciesMoistureTable` species IDs.
- Guarantee zero hazard at and below onset, smooth `C1` onset with no birthday cliff, modest ceilings, ordinal longevity grouping across all late ages, and pioneer persistence past 100–150+ years.
- Keep Common alder heart rot (`BirchAlderTimber.cs`) strictly a timber-quality downgrade probability, never an annual mortality probability.
- Gate senescence behind `SenescenceModelVersion` defaulting to `Disabled` (0) so frozen baselines and legacy saves draw zero RNG and remain byte-identical.
- Provide stateless, order-independent, deterministic keyed rolls per `(species, treeId, year)` under a versioned stream prefix declared inside this module (without editing `RngSalts.cs` in Part 1).
- Define biological-death record semantics cleanly separate from `Fell()` (`Cause = "old-age"`, zero timber revenue, clears crop-tree marks, does not count as a fell, plain player text `"Died of old age"`).
- Provide smooth `C0`/`C1` late-life DBH growth continuation beyond age 60/70 for Scots pine, Common alder and Downy birch only, delegating bit-exactly inside the existing windows and leaving Sitka spruce, Sessile oak and European beech growth equations untouched.

## Non-goals

- No wiring into `AnnualPipeline.cs`, `RngSalts.cs`, `MultiAreaContinuation`, saves, or `/multi` UI in Part 1.
- No edits to any existing file in `src/Core/SpeciesWave2/**`, `src/Core/NaturalProcesses/*.cs`, `src/Ui/**`, `tests/Core.Tests/**`, `tests/Phase1.Save.Tests/**`, Unity assets, or `Docs/Project/**`.
- No replacement of, or double-counting against, density/suppression mortality or windthrow mortality.
- No conversion of Common alder heart-rot risk into tree mortality.
- No edits to Sitka spruce, Sessile oak or European beech growth equations.
- No claim of fitted Irish empirical old-age mortality or late-life yield curves.
- No Version 2 Phase1 save changes; Version 3 six-species deadwood validation is deferred to Part 2.

## Player workflows

Not applicable in Part 1 (pure offline library with no player-facing runtime call site).

In Part 2 (deferred), when a woodland has `SenescenceModelVersion.OldAgeV1` active, a tree that dies of old age is removed as a biological death (not a harvest), clears any crop-tree mark, enters the existing fallen-deadwood pathway with zero timber revenue, and appears in History / Habitat inspection with the plain natural forestry wording `"Died of old age"` — without evidence tags (`[C]`/`[E]`), calibration labels, or academic disclaimers in player-facing text.

## Functional requirements

### FR-1 — Six-species natural old-age mortality hazard

- Reuse the six species IDs directly from `SpeciesMoistureTable` (`"common-alder"`, `"downy-birch"`, `"sitka-spruce"`, `"european-beech"`, `"scots-pine"`, `"sessile-oak"`). Do not define a second species vocabulary.
- Model annual residual old-age hazard `h(age)` over whole-year ages (`age >= 0`) independently per species as a `C1` smoothstep ramp from `0` at onset to a modest ceiling:
  - `h(age) = 0` bit-exactly for `age <= onset`;
  - for `age > onset`: `t = min(1.0, (age - onset) / ramp)`, `s = t * t * (3 - 2 * t)`, `h(age) = maximumAnnualHazard * s`.
- Locked per-species calibration parameters (all `[C]`):

| Species ID | Onset (years) | Ramp (years) | Maximum annual hazard |
| --- | ---: | ---: | ---: |
| `common-alder` | 55 | 60 | 0.030 |
| `downy-birch` | 60 | 55 | 0.026 |
| `sitka-spruce` | 90 | 55 | 0.020 |
| `european-beech` | 115 | 60 | 0.016 |
| `scots-pine` | 150 | 70 | 0.010 |
| `sessile-oak` | 160 | 75 | 0.009 |

- Expose a read-only profile record per species (`SpeciesId`, `OnsetAgeYears`, `RampYears`, `MaximumAnnualHazard`, `Evidence`) and the pure hazard function `AnnualHazard(speciesId, ageYears)` with no crowding, growth, moisture, or storm inputs.
- Required qualitative and quantitative properties:
  - exact zero for `age <= onset`; `h(onset + 1) <= 0.002`; non-decreasing and finite across ages `0..300`; maximum year-on-year increase `<= 0.002`; ceiling equals declared maximum and `<= 0.05`;
  - ordinal grouping at late ages (120, 150, 200, 250): `min(alder, birch) > max(beech, sitka) > max(pine, oak)`; strict onset ordering `alder < birch < sitka < beech < pine < oak`; within each pair the later-onset species also has the lower ceiling (`oak < pine`, `beech < sitka`, `birch < alder`);
  - pioneers are never killed automatically at commercial rotation age 60–70 (`h(birch, 0..60) == 0`; `h(alder, 61..70) < 0.005`, with ramp 60 explicitly chosen to keep the pioneer hazard modest through age 70 without a cliff: `h(alder, 70) = 0.0046875`, survival 60→70 ≈ 0.974); survival product from age 70 to 150 lies strictly in `(0.05, 0.60)` for both alder (≈ 0.140) and birch (≈ 0.192), so a pioneer cohort can still retain veteran survivors past 100–150+ years.
  - "Survival from age `a` to age `b`" always means `SurvivalProbability(species, a, b)`: the product of `1 − h(age)` over whole-year ages `a..b`, both ends included.
- Common alder heart rot (`BirchAlderTimber.cs`) remains strictly a timber-quality downgrade probability on felled stems and is never converted into an annual mortality probability.

### FR-2 — Smooth late-life growth continuation (Wave 2 species only)

- Apply only to the three Wave 2 species, reusing their existing calibration/teaching cutoffs:
  - `scots-pine`: cutoff age 60 (nominal adult increment `0.20 cm/yr`, window `10..60`, young increment `0.60 cm/yr`, max crowding reduction `0.50`);
  - `common-alder`: cutoff age 60 (nominal adult increment `0.20 cm/yr`, window `5..60`, young increment `0.95 cm/yr`, max crowding reduction `0.65`);
  - `downy-birch`: cutoff age 70 (nominal adult increment `0.25 cm/yr`, window `5..70`, young increment `0.85 cm/yr`, max crowding reduction `0.70`).
- **In-window (`age <= cutoff`):** delegate directly to `ScotsPineEngine.EvaluateGrowth` for `scots-pine` and `BirchAlderEngine.EvaluateGrowth` for `common-alder` and `downy-birch` so existing values stay bit-exact. Reuse `BirchAlderEngine.GrowthDomain` and `FutureSpeciesCatalog.ScotsPine.Readiness` rather than duplicating window bands.
- **Beyond the cutoff (`age > cutoff`):**
  - Anchor `A` is obtained by delegating to the existing evaluator at `cutoff` with zero crowding (`StemsPerHectare = Low`, `BasalAreaM2PerHa = Low`, `TopHeightM = MinimumTopHeightM`) and `SiteMoistureClass.NeutralLegacy`, rather than hardcoding `0.20`/`0.25`. This yields `0.20` bit-exactly for Scots pine, `0.25` bit-exactly for Downy birch, and `0.19999999999999996` (2 ULPs below `0.20`) for Common alder purely because of IEEE-754 representation of the unmodified linear window (`0.95 - 0.75`), not because of a model discontinuity;
  - `windowSlope = (A - youngAnchor) / (cutoff - windowLowAge)` (where `youngAnchor` is the delegated zero-crowding `NeutralLegacy` potential at `windowLowAge`, matching `-0.008` for pine, `-0.75/55` for alder, `-0.60/65` for birch);
  - `k = (-windowSlope) / A`;
  - `nominal(age) = A * exp(-k * (age - cutoff))` (returning `A` bit-exactly at `age == cutoff`).
  - This continuation preserves bit-exact delegation, bit-exact `C0` continuity and `C1` slope continuity at the cutoff with no manufactured 2-ULP jump for alder, stays strictly positive and monotone decreasing, and asymptotically approaches zero.
  - No cliff: the first year past the cutoff declines by `A(1 − e^(−k))`, strictly less than the existing window's own final-year decline `|windowSlope|` (Scots pine ≈ 3.92 % of `A` vs 4.00 %, Common alder ≈ 6.59 % vs 6.82 %, Downy birch ≈ 3.63 % vs 3.69 %). A fixed bound below the window's own decline (for example "< 3 % of `A`") is not attainable without breaking `C1` continuity, so the no-cliff test compares against the window's final year instead.
- Apply the same crowding rule as the existing evaluators (`crowding = max(normalised stocking pressure, normalised basal-area pressure)` over the species' existing domain bands; `potential = nominal * (1 - maximumCrowdingReduction * crowding)`) and the same single site-growth multiplier (`siteAdjusted = potential * SpeciesMoistureTable.Cell(species, moisture).Response.GrowthMultiplier`). Never apply the moisture multiplier twice; `NeutralLegacy` remains bit-exact `1.0`.
- Beyond the cutoff, enforce the same stocking, basal-area and minimum top-height domain checks as the species window; outside them, return `null` potential and site-adjusted increments (`IsAvailable == false`).
- Refuse `sitka-spruce`, `sessile-oak`, `european-beech`, and unknown species IDs with `ArgumentException`; their existing growth equations are untouched.
- Expose cutoff age, nominal continuation value at any `age >= cutoff`, and the unified growth evaluation result (`SpeciesId`, `Moisture`, `SiteResponse`, `PotentialDbhIncrementCm`, `SiteAdjustedDbhIncrementCm`, `IsWithinCalibrationWindow`).

### FR-3 — Version gate, determinism and validation

- Define `public enum SenescenceModelVersion { Disabled = 0, OldAgeV1 = 1 }`. Default is `Disabled` (0).
- When `Disabled`: return zero deaths, `RngDrawCount == 0`, `TreesEvaluated == 0`, and perform no species or age evaluation (unknown species must not throw while disabled).
- When `OldAgeV1`:
  - make exactly one stateless deterministic keyed roll per tree per year (even when a tree's age carries zero hazard);
  - compose the key injectively from `(speciesId, treeId)` under a versioned stream prefix constant declared inside this module (`"SENESCENCE-OLD-AGE-v1-"`), strictly prefix-free against existing `RngSalts` and `NaturalProcessStreams` families; the key builder accepts only the six supported species IDs, none of which contains `/`, so `prefix + speciesId + "/" + treeId` is injective even when a tree ID contains `/`;
  - mirror the Model 1 hashing approach of `NaturalProcessRandom.cs` (FNV-1a over the key string followed by SplitMix64-style mixing with `year` and `seed`, returning a uniform value in `[0, 1)`), without `System.Random` and without compiling or editing `NaturalProcessRandom.cs`;
  - changing key (species or tree ID), year, or seed changes the draw; input tree order does not affect results (evaluate/emit in deterministic `TreeId` ordinal order).
- Snapshot all caller-supplied collections defensively via plain enumeration so mutating caller collections after construction cannot alter requests or results.
- Validate inputs: reject `Year < 1` with `ArgumentOutOfRangeException`; reject unknown `SenescenceModelVersion` with `ArgumentOutOfRangeException`; when enabled, reject null tree list (`ArgumentNullException`), blank/duplicate `TreeId` (`ArgumentException`), negative age (`ArgumentOutOfRangeException`), and unknown species (`ArgumentException`).

### FR-4 — Biological-death semantics

- Keep old-age death cleanly separate from `Fell()`.
- Exact cause constant: `"old-age"`.
- Every `SenescenceDeathRecord` exposes:
  - `TreeId`, `SpeciesId`, `Year`, `Cause` (`"old-age"`), `PlayerCauseText` (`"Died of old age"`);
  - `BiologicalDeath == true`;
  - `TimberRevenueEur == 0m`;
  - `ClearsCropTreeMarks == true`;
  - `CountsAsFell == false`.
- The cause, the player wording and the four flags are fixed by the record type; its only public constructor takes `(TreeId, SpeciesId, Year)`, so no caller can build an old-age death record with other semantics.
- At most one death record per tree per evaluation. Output is structured so Part 2 can feed each death record directly into the existing fallen-deadwood pathway.

### FR-5 — Player-facing wording

- Exact player-facing cause string constant: `"Died of old age"`.
- Must use plain natural forestry wording and must contain no evidence tags (`[C]`, `[E]`, `[T]`, `[I]`), no `"calibration"`, and no academic disclaimers.

### FR-6 — Integration boundary

- Pure .NET 8 BCL-only library in namespace `SitkaP1.Core.NaturalProcesses.Senescence` under `Tools/MultiRackSitkaP1Lab/src/Core/NaturalProcesses/Senescence/`.
- Zero I/O, zero system clock, zero `System.Random`, zero Unity references, zero P0 references.
- Zero edits to any existing file in Part 1. Part 2 integration is deferred to a separate packet.

## Authority and source of truth

- **Accepted product intent.** The Playable Milestone 8 Part 1 task packet (2026-11-28), Decision Log (`D-030` verification/integration language, `D-050` versioned model/stream discipline, `D-059` species modelling-depth parity and evidence honesty, `D-062` species-specific broadleaf treatment), Current Milestone and Game Brief.
- **Implementation facts.** Inspected repository at base `b2006767cbe70fa2c822497470908251a0fd6ffd` (`origin/integration/2d-sim`) and context commit `bdeb6a324acaf4129c10fb0846faac8b64974360`: `SpeciesMoistureTable.cs`, `FutureSpeciesCatalog.cs`, `ScotsPineGrowth.cs`, `BirchAlderGrowth.cs`, `BirchAlderTimber.cs`, `NaturalProcessRandom.cs`, `RngSalts.cs`.
- **Spec authority.** Approved and task-level implementation authority for Part 1 only, subordinate to the project authority hierarchy.

## Provenance and evidence classification (Track A)

Track A covers species longevity ordering, late-life senescence and post-window growth decline. In accordance with D-059, no bibliographic citation is invented and no value is presented as an Irish empirical mortality or late-life yield fit.

| Element | Evidence class | Provenance and evidence limit |
| --- | --- | --- |
| Ordinal longevity grouping: pioneers (`common-alder`, `downy-birch`) shortest-lived < mid-longevity (`sitka-spruce`, `european-beech`) < long-lived (`scots-pine`, `sessile-oak`) | `[I]` Inference / `[T]` Transfer | General European and British forestry species-longevity knowledge, not a fitted Irish survival dataset. Individual site-grown trees of every species routinely live well past commercial rotation age. |
| Within-pair ordering (`alder` before `birch`, `sitka` before `beech`, `pine` before `oak`) | `[I]` Inference | General silvicultural longevity expectation in Atlantic/temperate Europe; not a measured Irish hazard ranking. |
| No fitted Irish annual old-age mortality curve | Evidence limit | No fitted Irish annual senescence probability function exists in project evidence for these six species. Every numeric parameter below is therefore gameplay calibration, not a fitted ecological rate. |
| Per-species onset ages (55, 60, 90, 115, 150, 160 yr), ramp widths (55–75 yr), and maximum annual hazards (0.009–0.030) | `[C]` Calibration | Deliberate teaching/simulation calibration chosen to produce smooth residual mortality without birthday cliffs or commercial-rotation wipeouts while preserving long-lived veterans past 100–150+ years. |
| Smoothstep functional form `s(t) = t^2 (3 - 2t)` | `[C]` Calibration | Mathematical convenience guaranteeing bit-exact zero through onset, `C1` continuity at onset and ceiling, and bounded annual slope (`<= 0.002/yr`). |
| Common alder heart rot (`BirchAlderTimber.cs`) kept strictly as timber downgrade | `[C]` Calibration | Preserves existing Wave 2 contract (`FutureSpeciesCatalog.CommonAlder`, `SPEC-multi-2d-birch-alder-v1`); heart rot downgrades timber grade only and is never mapped to annual mortality. |
| Wave 2 in-window growth shapes and domain bands (`scots-pine` 10–60, `common-alder` 5–60, `downy-birch` 5–70) | `[C]` Calibration (reused unchanged) | Delegated bit-exactly to existing `ScotsPineEngine` and `BirchAlderEngine`; Scots pine domain originates from Irish GROWFOR metadata on the profile card, while birch/alder teaching windows are calibration. |
| Post-cutoff exponential taper `adultIncrement * exp(-k * (age - cutoff))` with `k = -windowSlope / adultIncrement` | `[C]` Calibration | Smooth `C0`/`C1` mathematical continuation preserving the exact cutoff value and slope without inventing a late-life empirical yield curve. |
| Single application of `SpeciesMoistureTable` growth multiplier and crowding reduction beyond cutoff | `[C]` Calibration | Reuses the existing Wave 2 calibration rules consistently across the cutoff boundary; `NeutralLegacy` remains bit-exact `1.0`. |

### Calibration disclosure

All numeric hazard curves and late-life growth continuation rates in this module are gameplay calibration (`[C]`), informed qualitatively by general species-longevity ordering (`[I]`/`[T]`). They are not empirical forecasts of Irish tree lifespan or old-growth diameter increment.

## Save / backward compatibility

- **Part 1:** Zero save impact. No save file is read or written, no save schema is changed, and no existing file is modified.
- **Version gate guarantee:** `SenescenceModelVersion` defaults to `Disabled` (0). When `Disabled`, the engine performs zero RNG draws and zero tree evaluations, ensuring all nine frozen baselines and legacy saves remain byte-identical once wired.
- **Version 2 Phase1:** Completely untouched in both Part 1 and Part 2.
- **Part 2 (deferred):** Version 3 six-species deadwood save validation and persistence of `SenescenceModelVersion` will be specified and verified in Part 2.

## Security / write-back

Pure in-process computation with no file I/O, no network, no external parsing, and no destructive writes. All caller-supplied collections are defensively copied into private arrays via plain enumeration (preventing hostile `ICollection.CopyTo` aliasing), and all numeric/identifier arguments are validated against finite/non-negative/non-empty bounds.

## Integration prerequisites and known risks

- **Existing NaturalProcesses test project (integration prerequisite).** `Tools/MultiRackSitkaP1Lab/tests/Core/NaturalProcesses/SitkaP1.NaturalProcesses.Tests.csproj` compiles `../../../src/Core/NaturalProcesses/**/*.cs` recursively and no Species Wave 2 sources. By inspection, once this branch is combined it picks up the mandated `src/Core/NaturalProcesses/Senescence/` files and fails to compile, because they reference `SitkaP1.Core.SpeciesWave2` types. Part 1 deliberately does not edit that project (outside this lane's write fence). The single integrator must do one of:
  - (a) add the Species Wave 2 sources to it: `<Compile Include="../../../src/Core/SpeciesWave2/**/*.cs" />`; or
  - (b) exclude this module from it: `<Compile Remove="../../../src/Core/NaturalProcesses/Senescence/**/*.cs" />`.

  Either option also needs the matching update to that project's JW-T64 check, which (by inspection) asserts exactly one `<Compile ` item and the exact original glob line. Its JW-T63 hygiene scan reads the folder recursively; the module sources are kept free of JW-T63's banned tokens (checked by this lane's HYG-01). This lane's own gate does not build or run that project (verification diet).
- **Main Core project.** `SitkaP1.Core.csproj` globs all of `src/Core/**`, including Species Wave 2, so it compiles the new files while no Core code references them. By inspection it is unaffected: no type-name clashes, and no G6 determinism, file-API or RNG-literal hits. It is not built in this lane because it needs the out-of-scope `.p0ref`.
- **Transcendental determinism.** The late-life continuation uses `Math.Exp`. The pinned digest holds for the verified runtime and platform (SDK 8.0.133, linux-x64). Part 2 must decide whether continuation outputs may feed persisted or cross-platform-compared state.
- **Disabled envelope.** Unlike StormModel 0, `Disabled` still refuses `Year < 1` and undefined model values (FR-3). Part 2 must call it only with `Year >= 1`.

## Acceptance criteria

- **AC-1 (Version gate & baseline preservation):** `Disabled` (default) produces zero deaths and `RngDrawCount == 0` across 1,000 mixed-age trees (including ages 200+) and does not throw on unknown species; `OldAgeV1` makes exactly one keyed roll per tree even when age carries zero hazard.
- **AC-2 (Six-species hazard curves):** All six `SpeciesMoistureTable` species have bit-exact zero hazard at and below onset, `h(onset + 1) <= 0.002`, non-decreasing finite curves through age 300 with max annual step `<= 0.002`, declared ceilings `<= 0.05`, strict ordinal grouping at ages 120/150/200/250, pioneer non-wipeout at commercial rotation (`birch` 0 through 60; `alder` `< 0.005` across 61–70 with ramp 60) with 70–150 survival in `(0.05, 0.60)`, crowding independence, and clean separation from alder heart rot.
- **AC-3 (Keyed rolls & biological-death semantics):** Repeatable, input-order-independent rolls varying with species, tree ID, year and seed; at most one death per tree; exact biological-death properties (`Cause == "old-age"`, `BiologicalDeath`, `TimberRevenueEur == 0m`, `ClearsCropTreeMarks`, `CountsAsFell == false`); plain player text `"Died of old age"` free of tags/disclaimers; argument guards reject duplicates, negative ages, `Year < 1`, and enabled unknown species; 1,000 age-200 alder cohort yields modest residual deaths (`0 < deaths < 100`).
- **AC-4 (Wave 2 late-life growth continuation):** Exact bit-for-bit delegation inside windows (pine 42, alder 33, birch 44); bit-exact continuity between `NominalIncrementAtOrBeyondCutoff(species, cutoff)` and delegated zero-crowding `NeutralLegacy` potential at cutoff (`ApproxEq` within `1e-12` of 0.20, 0.20, 0.25, noting the 2-ULP IEEE-754 representation of alder's unmodified linear window) with `C1` slope continuity and no artificial jump (first post-cutoff year falls by less than the window's own final year); strictly positive monotone decreasing continuation through age 200+; preserved crowding and single moisture-multiplier application (`NeutralLegacy == 1.0`); `null` outside non-age domain bands; refuses `sitka-spruce`, `sessile-oak`, `european-beech`.
- **AC-5 (Pinned offline determinism & build gate):** Restored and built with pinned SDK 8.0.133 (`0 warnings / 0 errors` under `TreatWarningsAsErrors`), standalone console executable passes all checks with exit code 0, and two separate processes produce byte-identical output and matching pinned canonical SHA-256 digest (with doubles encoded as raw IEEE-754 bits).
- **AC-6 (Hard write fence):** Only new files under the four authorised prefixes exist vs base `b2006767cbe70fa2c822497470908251a0fd6ffd`; zero existing files are modified.
- **AC-7 (Part 2 — DEFERRED):** Wire senescence into `AnnualPipeline.cs`, register senescence stream salt(s) in `RngSalts.cs`, wire live Wave 2 late-life growth, validate Version 3 six-species deadwood saves (leaving Version 2 Phase1 untouched), and verify History/Habitat UI `"Died of old age"` presentation in an integrated candidate.

## Stop conditions

Stop and return to the Manager if:

- product authority conflicts or a locked rule in FR-1..FR-6 is materially ambiguous;
- implementation requires modifying any file outside the four authorised prefixes;
- Part 1 cannot compile or pass offline verification without wiring Part 2, adding packages, or touching `RngSalts.cs` / `AnnualPipeline.cs` / saves;
- any frozen baseline or protected Wave 2 file would be altered.
