# Multi #842 — G1 real source fields and multi-area fixture feasibility (read-only result)

**Date:** 19 November 2026
**Issue:** [#842](https://github.com/jermur788/CCF/issues/842). Decision owner: Manager [#730](https://github.com/jermur788/CCF/issues/730).
**Verdict:** **BLOCKED — NEEDS NEW EVIDENCE** for a source-defensible "sample + ≥2 comparable targets + control" fixture. The code and schema work is a **bounded extension** (§4). It does not fix the evidence gap, because no feature that could tell two areas apart is real today. **G1 stays DISABLED / Manual / NotCovered. #828 stays DRAFT.**

## 0. Actor, method and boundaries

| Item | Value |
|---|---|
| Actor | Claude Opus 5.5 (`claude-opus-5-5`, Anthropic), Claude Code harness in T3 Code |
| Host | Desktop `DESKTOP-DL1BQI7`, Windows 11 host reading WSL2 Ubuntu. The source sits on `/dev/sdd`, the WSL2 virtual disk on the internal drive. No laptop, ZX20, external Files volume or mount was used. |
| Live main | `4a06f1981041b7307ae8225fc0b5aca969f3f20b` at registration; moved to `8a5d96390f8eae07d7da0680d1623aefaffe5578` by write time, and the result branch is based on that commit. The source candidate is local and unaffected by main. |
| Read-only source | `/home/jer/CCF-Dev/Multi-20261119/y2engine`, branch `feature/multi-789-year2-10-engine-20261119-v1`, HEAD `6f9d2e990201bc47e361c3562be9390ed221baf6`. `git --no-optional-locks status --porcelain` was empty. Same candidate as #837 and the #841 registration. |
| Other read-only inputs | P0 lab at GitHub commit `a7f41983c2f2f233b8fe999d1a5b10e72bdd2866` (`git show` object reads in the CCF-HP clone, no checkout). #814 marking candidate `_814/marking` HEAD `e8e1d376…` (read only). #828 spec and checklist, and the #827 result (GitHub reads). |
| Fixture identity re-observed | `fixtures/derived/p1d-30m-sitka-adult-v1.sitkap1fx.json`: SHA-256 `bcaaa5c3d9b3d36ad8319ffcc361a6225a02b90281cef8435de7c52f8ff757c4`, 32,567 bytes. Matches the catalog record `TrustTables.cs:99-102`. |
| Method | File reads, `grep`, read-only `git rev-parse/ls-files/ls-tree/show/cat-file` with `--no-optional-locks`, `sha256sum`, and one isolated Python (`-I`) pass over the fixture JSON to get distributions. **No build, test, .NET, Unity, checkout, fetch into the source, fixture authoring or source edit.** |
| #841 protection | #841 writes only to `g2-fell-writer` and its qualification lane, which are separate physical paths. Neither was read. `y2engine` has no write reservation. |
| Writes | This one Markdown on `review/multi-842-g1-real-field-and-fixture-feasibility-20261119-v1`, created from main `8a5d9639`, plus #842 and #730 comments. |

Paths below are relative to `y2engine/Tools/MultiRackSitkaP1Lab/` unless stated otherwise. `P0:` means `Tools/MultiRackSitkaLab/src/Core/` at `a7f41983`.

## 1. Accepted current-source fields (Q1)

Key: **DIRECT** means stored in the fixture. **DERIVED** means computed by documented rule. **SYNTHETIC CONSTANT** means fixed for every fixture by the v1 schema. **ABSENT** means no representation exists. "Direct" means stored, **not measured**: every stored tree value is a synthetic generator choice (`ManifestGenerator.cs:6-15`, `DerivedManifest.cs:164`).

| Field | Class | Source / units / precision | Observed in fixture | Current state or explanatory only |
|---|---|---|---|---|
| Tree ID | DIRECT | `DerivedManifest.cs:9`; safe ID `^[A-Za-z0-9_-]{1,64}$` (`:65,331`); strictly ordinal-ascending, case-sensitive (`:332-333`) | `P30-001…P30-100`, row-major (`ManifestGenerator.cs:55`) | Current. Engine identity and RNG key (`AnnualRules.cs:43`) |
| X/Y | DIRECT | int64 mm, `[0,30000)` (`DerivedManifest.cs:10-11,334`); duplicate XY refused (`:318`) | x 1069–29028, y 928–29092; jittered 3 m grid, ±600 mm (`ManifestGenerator.cs:8,47-48`) | Current (genesis only; trees do not move) |
| unityX / unityZ | DERIVED | f32 bits, `(mm−15000)/1000` (`:20-21,43,79,341-342`); engine `XM/ZM` float (`AnnualEngine.cs:9,40`) | — | Current, engine truth for distance |
| Status | DIRECT at genesis, then DERIVED | Must be `"live"` (`DerivedManifest.cs:335`); later `AnnualDeath(TreeId, EcologicalYear, Cause, Mechanism)` (`AnnualEngine.cs:7-12`) | 100 live | Current. Causes are `self-thinning` with `hazard` or `boundary` only (`AnnualRules.cs:49,138`). **No Fell cause** (#827 §2a) |
| Age | DIRECT | int years 1..10000 (`DerivedManifest.cs:13,336`) | **All 30** (one distinct value) | Current. Advances +1/yr (`AnnualRules.cs:99`). **One cohort, so age cannot tell areas apart** |
| DBH | DIRECT mm, then DERIVED f32 cm | int mm `[10,2000]` (`:14,61,337`); `dbhCm=(float)(dbhMm/10.0)` (`:77,339`); grows in float (`AnnualRules.cs:90-91`) | 131–349 mm, 76 distinct; 12 below 170 mm | Current engine value is f32 cm. **Use bit words, not viewer-rounded strings** |
| Height | DIRECT mm, then DERIVED f32 m | int mm `[100,60000]` (`:15,62,338`); `(float)(heightMm/1000.0)` (`:78`); `SitkaGrowthModel.NextHeight` (`AnnualRules.cs:92`) | 17.13–23.67 m; linear in DBH by construction (`ManifestGenerator.cs:13,52`) | Current. **Not independent of DBH at genesis** |
| Crown radius | DERIVED (synthetic initialisation) | f32, `max(0.1, 0.9415+0.07635·dbhCm)` (`DerivedManifest.cs:54-55,80-85`; `CcfSourcePins.cs:28-30`); relaxes 0.15/yr (`AnnualRules.cs:94-98`) | — | Current, but "not a measured crown" (`:55`) |
| Competition (Hegyi CI, neighbour count) | DERIVED, report only | 8 m cutoff, 0.5 m floor, f32 (`AnnualRules.cs:6-7,67-82`); audit `Neighbours`, `PreGrowthCi`, `Suppression` (`AnnualEngine.cs:20-21`) | max-CI tree `P30-053` (`tests/…/AnnualTests.cs:108`) | **Explanatory only.** Not stored in state |
| Stand count, N/ha, BA m²/ha, Dq, RD | DERIVED, double | `AnnualRules.cs:102-118`; `AnnualStand` (`AnnualEngine.cs:17-18`) | ~1,111 N/ha; BA ≈ 55.9 m²/ha (informational recomputation from mm, not engine output) | **Whole-fixture only.** No per-area or local denominator |
| Density pressure / mean vulnerability | DERIVED, double | `AnnualRules.cs:20,27-33` | — | Whole-fixture only. This drives every tree's mortality |
| Area | SYNTHETIC CONSTANT | `areaHa` f32 `0x3db851ec` (0.09 ha), exact 900 m² (`DerivedManifest.cs:44-46,243-246`) | — | One denominator per engine (`AnnualEngine.cs:31,38`) |
| Geometry | SYNTHETIC CONSTANT | `P1DerivedSquare30m`, 4-vertex square, side 30000 (`:38-39,117-128,226-240`) | — | One polygon per fixture |
| Site productivity | SYNTHETIC CONSTANT | `1.0f` "has no writer" (`:50`); parse refuses anything else (`:262`); **read as a compile-time constant inside growth** (`AnnualRules.cs:89`) | `3f800000` | Cannot vary per area without changing the engine rule |
| Elevation / slope | SYNTHETIC CONSTANT / ABSENT | `elevationMm` must be 0 (`DerivedManifest.cs:125,229`); P1 has no slope field. P0's `SlopeDeg` must be 0 on import (`FixtureAccess.cs:68`) | 0 | — |
| Species / stage / demand class / pruning | SYNTHETIC CONSTANT | `Sitka`, `MatureEquivalent`, `Reference`, 0 (`DerivedManifest.cs:51-53,263-266`) | — | A constant cannot discriminate |
| Excluded systems | ABSENT by declaration | browsing, canopyLight, juvenileLifecycle, mixedSpecies, pruning, recruitment, regeneration, **storm** (`:58-59,271`) | — | Wind and storm are explicitly absent |
| Terrain, soil, wind exposure, drainage, yield class | **ABSENT** | No field anywhere in P1 or P0 | — | **UNKNOWN. Must not be fabricated** |
| Year | DIRECT constant, then in-memory | `initialEcologicalYear` must be 0 (`DerivedManifest.cs:253`); `sourceInventoryYear` must be null (`:255`); year+revision only in `internal ContinuationSnapshot` (`AnnualContinuation.cs:3,21-24`) | 0 | No public committed-frame API (#827 §1d) |
| Seeds and models | DIRECT / constants | `simulationSeed` int32 (`:257-258`), `generatorSeed` (`:222`); growth model 2, RNG model 1, algorithm id (`:47-49,259-261`) | seed 20261116 | Mortality roll key `"ADULT-MORT-"+TreeId`, year, seed (`AnnualRules.cs:43-44`). **No area term** |
| Action eligibility | DIRECT, informational | `actionEligibleDbhThresholdMm` (`:71,267-268`) | 170 | D-093: informational only, not a comparator feature |
| **Management-area identity** | **ABSENT** | Root keys are an exact allowlist (`DerivedManifest.cs:199-201`) and tree keys are exact (`:299-300`): no `areaId`/`scopeId` can be added to v1 | — | `derivedFixtureId` identifies a fixture. `P1Domain` is a claim domain (`Domain.cs:3-15`), **not an area** |

**P0 originals** (`ccf-sitka-lab` v1). These are also `sourceClass: Synthetic`. Each file has one `forestId`, one `standAreaPolygon`, `inventoryYear` 2026, `adultThresholdMm` 100, `siteProfile.slopeDeg` 0, and per-tree `zMm`, `ageAtInventoryYears`, live/dead `status` and `markState` (`P0:Models.cs:66-80,104-114`). Observed: `oracle-quarter-ha` 3 trees; `sitka-flat-demo-v1` 162 trees (153 live, 9 dead) on a 5-vertex polygon; `sitka-flat-perf-2000-v1` 2000 trees (1893 live, 107 dead) on a 200×100 m rectangle. The P0 import is read-only, and annual advance is refused for this domain (`FixtureAccess.cs:150-151`). P0 adds polygons and dead stems but **no area identity and no site variation**, and it cannot run ecology. So it is **not a G1 source**.

**#814 viewer.** `candidate-marks.js:27` hard-codes `ID_PATTERN = /^P30-[0-9]{3}$/`, and `:23-24` hard-code the 170 mm / `41880000` threshold. Any new fixture's ID family would be refused there. This is a downstream surface only, not engine truth.

## 2. Area identity (Q2)

- **One management scope only.** One fixture is one 30 m square, one `areaHa`, one engine state (`AnnualEngine.cs:30-43`) and one stand aggregate (`AnnualRules.cs:19,60-61`). The catalog has exactly one derived record (`TrustTables.cs:97-107`). `ReadDerived` admits only the 30 m domain (`FixtureAccess.cs:118-119`).
- **No independent area ID, geometry set or per-area denominator** exists in any accepted representation (§1 last row).
- **No multiple distinct source instances.** One `p1d-` fixture plus three P0 inspection-only files, each a separate world with an unrelated ID family.
- **Stable IDs across stands:** IDs are unique only within one manifest (`DerivedManifest.cs:332-333`). The mortality RNG key has no area or fixture component (`AnnualRules.cs:43`). Two areas sharing one seed and the ID `P30-001` would get **identical roll streams**, which is correlation that has nothing to do with biology. Any multi-area design needs globally unique IDs, or a distinct seed per area, recorded in provenance.
- **Binding to committed year and revision:** only the internal `ContinuationSnapshot(State, Revision)` with year/revision fences (`AnnualContinuation.cs:26-34`). There is no fixture-digest-plus-year public token. That binding belongs to #841's G2 contract; this audit does not prescribe it.
- **Why the original must not be split:** the four 15 m quadrants of `P30-*` have mean DBH 267.2 / 232.5 / 234.0 / 254.6 mm (n=25 each). Those differences are SplitMix generator noise (`ManifestGenerator.cs:17-19,49-51`), not area properties. Quadrants share one whole-stand density pressure (`AnnualRules.cs:20`), and the 8 m Hegyi neighbourhood crosses quadrant edges. A split would invent "differences" between areas that are the same population, and it would break the protected oracle.

## 3. Separate larger-fixture feasibility (Q3)

### 3a. What the schema would allow without touching the original

- `DerivedManifest` v1 is **closed**. These are fixed by constant: generator ID/version (`:220-221`), catalog version (`:213`), geometry (`:226-240`), area (`:243-246`), site 1.0 (`:262`), year 0 (`:253`) and the exact key sets (`:199-201,299-300`). A multi-area fixture **cannot** be expressed in v1.
- Additional **separate** 30 m `p1d-` manifests, one per "area", would pass `Parse` if they used the same generator ID string. `generatorSeed`, `simulationSeed`, tree values and IDs are free (`:222,257,303-315`). But this would (a) misstate provenance, because `GeneratorId` names the one coded generator (`ManifestGenerator.cs:21-31`); (b) require new `DerivedCatalog.Records` entries, which **change `DerivedCatalog.Digest()`** (`TrustTables.cs:112-120`), a value printed by `verify-derived` (`src/Cli/Program.cs:44`); and (c) still carry no grouping, area ID or shared-year binding. **Not recommended.**
- Therefore a new, wholly distinct format family, catalog and domain tag is **unavoidable** for a real multi-area fixture. A new `P1Domain` value touches `Domain.cs:7-34` and the exhaustive `AnnualGate` switch (`FixtureAccess.cs:148-159`). That must be weighed against #841's ownership of those files (§5).

### 3b. Engine reuse for per-area ecology

- `AnnualRules.Calculate(AnnualState, seed, areaHa)` is a pure static function (`AnnualRules.cs:13`). Running it **once per area, with that area's own state and denominator**, reuses the accepted rules unchanged and keeps every area's density pressure and boundary thinning local (`:20,120-142`).
- If areas instead share one engine state, competition crosses areas (`:67-82`), and density pressure and boundary thinning become whole-estate values (`:19-20,59`). That is a different model. **Per-area isolation is a simplification** (no edge effects between neighbouring compartments) and must be disclosed.
- **Per-area site productivity is not feasible** without changing `AnnualRules.Grow` (`:89` reads the constant). That path carries the protected Year 1–10 oracle and is #841's lane. Site therefore stays 1.0 in every area, so **site is UNKNOWN, not "equal", for every area**.

### 3c. The evidence gap (why the verdict is BLOCKED)

The only features that can vary between areas are the authored tree lists and what is computed from them: N/ha, BA/ha, Dq, RD, DBH/height distributions, age (if a different cohort is authored) and spatial pattern / CI distribution. **All of them would be authored by the fixture designer.** Site, terrain, wind and soil are absent. Species, stage and site are constant. So:

1. "Comparable" targets would be comparable **by construction**, which is a tautology. That does not meet #828 G1-C ("independently verify each distinct scope … source-validated comparable target areas").
2. Even targets drawn from the same generator distribution differ in realised statistics (see the quadrant spread in §2). Calling them COMPARABLE needs a tolerance, and **no tolerance is accepted anywhere**. This audit invents none. #843 (science-evidence brief) is the place for a sourced tolerance. Manager #730 must reconcile the two results.
3. A DIFFERENT control is easy to author (for example a much higher stocking or another cohort age). An UNKNOWN control is easy to declare (a required feature marked missing). Neither proves the positive case.

**Feasibility:**

| Part | Rating |
|---|---|
| New distinct fixture format, catalog, domain and per-area engine runs, preserving every current pin | **FEASIBLE WITH BOUNDED EXTENSION** (after #841 acceptance and spec approval; §5) |
| DIFFERENT and UNKNOWN controls | **FEASIBLE WITH BOUNDED EXTENSION** |
| ≥2 *source-validated* COMPARABLE targets | **BLOCKED — NEEDS NEW EVIDENCE**: a real or independently sourced between-area feature set (at least site/yield class), plus an accepted calibration/tolerance. Alternatively, an explicit owner decision to accept *designed* (synthetic, by-construction) comparability as a labelled demonstration (Q-A below) |
| Overall full G1 acceptance per #828 | **BLOCKED — NEEDS NEW EVIDENCE** |

## 4. Comparator feature inventory and negative coverage (Q4)

The tri-state inventory below lists available features only. **No threshold, weighting or affirmative match is proposed.** "Default" is the state the comparator must return until an accepted predicate exists.

| Feature | Available as | Can discriminate today? | Default state |
|---|---|---|---|
| Species / stage | constant | No | UNKNOWN (a match on a constant is not evidence) |
| Cohort age | int, per tree | Only if authored differently | UNKNOWN (proxy only) |
| N/ha, BA/ha, Dq, RD | derived double, per area if run per area | Yes, but values are authored | UNKNOWN until tolerance accepted |
| DBH / height distribution | f32 per tree | Yes, but authored; height is tied to DBH at genesis | UNKNOWN |
| CI / neighbour distribution | derived f32 per year | Yes, but authored spatial pattern | UNKNOWN |
| Site productivity | constant 1.0 | No | **UNKNOWN for all areas** |
| Terrain / slope / elevation / soil / wind / exposure | absent or zero | No | **UNKNOWN for all areas** |
| Year / revision / fixture digest | in-memory only | n/a (a binding, not a feature) | Mismatch ⇒ refuse |

**Positive and negative matrix for a future fixture.** Expected outcomes are requirements, not results. Nothing was run.

| # | Case | Expected |
|---|---|---|
| P-1 | Sample S: exact live player-chosen IDs in one declared scope | Accepted as sample; no work created |
| P-2 | Targets T1, T2 meet an **accepted** predicate | COMPARABLE, with itemised per-tree proposals that stay hypothetical. **Not achievable until §3c evidence exists** |
| P-3 | Control C-DIFF with authored mismatch (density/size/cohort) | DIFFERENT, excluded and explained |
| P-4 | Control C-UNK with a required feature missing | UNKNOWN, Manual/NotCovered |
| N-1 | Mismatched size, density or neighbourhood | DIFFERENT, no proposal |
| N-2 | Site or wind unavailable (the case for **every** area today) | UNKNOWN; must never be treated as "equal" |
| N-3 | Species and/or cohort match alone | Not COMPARABLE |
| N-4 | Any required feature missing, NaN or non-finite bit word | UNKNOWN, refuse proposal |
| N-5 | Stale year or revision, or different fixture digest | Refuse; no proposal or work |
| N-6 | Tree ID wrong case, unknown, dead, duplicate, or from another area or fixture | Refuse without mutation |
| N-7 | Same tree ID in two areas under one seed | Fixture refused (RNG stream collision, `AnnualRules.cs:43`) |
| N-8 | Automatic broadcast to all areas, or unreviewed whole-area treatment | Zero proposals accepted; zero PendingWork |
| N-9 | Fixture built by subdividing or rebasing the original `P30-*` | Fixture refused |
| N-10 | Original Year 1–10 digests (`tests/…/ContinuationTests.cs:11`) or catalog record altered | Fail. Protected oracle |
| N-11 | Comparator threshold present without a cited acceptance | Fail closed (DISABLED) |

## 5. Minimum path after this result (Q5)

**Order:** (1) #841 G2 accepted by #730 → (2) owner answers Q-A–Q-E → (3) the evidence for §3c is supplied, or Q-A explicitly accepts designed comparability → (4) #828 spec revised and approved → (5) one bounded fixture task → (6) independent non-authoring qualification.

**Pins that must stay byte-identical:** catalog record and digest (`TrustTables.cs:97-120`; file SHA `bcaaa5c3…`, content hash `b7dee8a3…`, 32,567 bytes); Year 1–10 digests (`ContinuationTests.cs:11,18-42`); strict-parse tests (`tests/…/Program.cs:80-165`); RNG key probes (`Program.cs:343-347`, `AnnualTests.cs:117`); `P30-*` expectations (`AnnualTests.cs:108,122`); source pins (`CcfSourcePins.cs:10-24`); #811/#814 mirrored build inputs and golden output.

**Proposed future allowlist.** This is not a grant, and new files are preferred:

- NEW `Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/` — separate manifest format and strict parser with an explicit `areas[]` (scope ID, polygon, `areaHa`, per-area seed, tree list with globally unique IDs, per-feature provenance and missingness); a **separate** catalog class, so `DerivedCatalog.Digest()` is untouched; a per-area runner that calls `AnnualRules.Calculate` once per area; and a feature-extraction-only comparability inventory that returns UNKNOWN by default.
- NEW `Tools/MultiRackSitkaP1Lab/fixtures/multiarea/<id>.json`, plus its generator in the new folder.
- NEW `tests/Core.Tests/MultiAreaTests.cs`, plus a one-line registration in `tests/Core.Tests/Program.cs`.
- MINIMAL, only if a domain tag is required: `Domain.cs` and the `AnnualGate` switch in `FixtureAccess.cs`. **Coordinate with #841. Do not edit in parallel.**
- **Excluded:** `AnnualRules.cs`, `AnnualEngine.cs`, `AnnualContinuation.cs`, `DerivedManifest.cs`, `ManifestGenerator.cs`, the original fixture, `TrustTables.cs` records, and any #841 Fell/Work Plan types. Cross-area atomic Fell waits for #841's accepted transaction design.
- **Reviewer:** a non-authoring agent on a different provider from the author, with native build/test rights, plus a separate forestry/source assessor for the predicate (#843 input). The author never sets catalog `Accepted` (`TrustTables.cs:5-9`).

**Owner/Manager questions** (returned rather than defaulted):

- **Q-A.** Is a **synthetic designed-comparability** fixture, where targets are comparable by construction and labelled as such, acceptable for G1 V1? Or must comparability rest on real or externally sourced inventory and site data?
- **Q-B.** Must site/yield class be a required matching feature? If yes, every area is UNKNOWN until the engine gains per-area site, which means changing `AnnualRules.cs:89` in a separate, oracle-safe task.
- **Q-C.** Should areas be spatially isolated (no cross-area competition; recommended simplification) or adjacent with shared edges?
- **Q-D.** Who supplies the comparator tolerance: #843 research, Manager calibration, or none (exact-match designs only)?
- **Q-E.** Must one Work Plan spanning several areas be all-or-none? That depends on #841's accepted transaction.

## 6. NEXT / STOP

- **STOP:** no G1 implementation and no fixture authoring. #828 stays DRAFT, with R-02, R-04 and R-05 and X-01, X-03 and X-04 still unmet.
- **NEXT:** Manager #730 compares this with #843, puts Q-A–Q-E to the owner, and waits for #841 acceptance. Only then dispatch one bounded fixture/qualification task under the §5 allowlist.

*Read-only result. No source, fixture, test, build, Unity, external device or main change was made. Every claim above comes from file and line reads at the stated commits; nothing was executed except a hash and an isolated JSON read.*
