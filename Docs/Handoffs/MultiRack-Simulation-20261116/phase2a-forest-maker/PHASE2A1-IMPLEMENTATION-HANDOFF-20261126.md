# CCF Multi 2D — Phase 2A-1 Pure-.NET Forest Maker & Canonical 0.64 ha Presets — Implementation Handoff

- **Date:** 2026-11-26
- **Branch:** `feat/multi-2d-phase2a1-forest-maker-core-20261126-v1`
- **Base Commit:** `6d72c30538b1fbe30d3cbcf083b5b368733662b8` (`origin/integration/2d-sim`, Phase 2B-1 + C1 merge)
- **Verified Implementation Commit:** `c335559c9a5e958027dc5290df0407687c013b54` (followed by this docs-only handoff commit on the same feature branch)
- **Worktree:** `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-2D-Phase2A1-20261126`
- **Spec:** `SPEC-multi-2d-phase2a-forest-maker-V1` (owner approved 2026-11-26, reconciled in `specs/multi-2d-phase2a-forest-maker-v1/`)
- **Authority Scope:** Sole D-091 implementer for Step 2A-1 Core only. Zero edits to `src/Ui/**`, `src/Core/Phase1/**`, `Assets/`, `Packages/`, `ProjectSettings/`, `Docs/Project/**`, `Tools/MultiRackSitkaLab/`, or `build/2d-gate/frozen-anchors.json`. Zero merges to `integration/2d-sim` or `main`; zero PRs.

---

## 1. Commit & File-by-File Change Summary

### Commit History (`6d72c305..HEAD`)
1. `594c3a45` — `docs(2d-forestmaker): publish approved Phase 2A spec quartet reconciled to 6d72c305`
2. `e04dd539` — `feat(2d-forestmaker): implement pure-.NET Forest Maker engine and 0.64 ha presets`
3. `07bdb34c` — `test(2d-forestmaker): add Phase 2A-1 verification suite and pin preset anchors 10-11`
4. `c335559c` — `build(2d-gate): tighten Core minPassed floor 291 -> 298 for Phase 2A-1 selftest C09`
5. *(this commit)* — `docs(2d-forestmaker): add Phase 2A-1 implementation handoff`

### Files Changed

#### Spec Quartet (`specs/multi-2d-phase2a-forest-maker-v1/`)
- `spec.md` — Reconciled status `DRAFT` → `APPROVED` (2026-11-26), implementation base → `6d72c30538b1fbe30d3cbcf083b5b368733662b8`, Q1/Q2 annotations → accepted defaults, §5.5 file scope aligned with `src/Core/MultiArea/ForestMaker/` and `src/Core/RngSalts.cs`.
- `plan.md` — Reconciled status, base SHA, Step 2A-1 file scope, and Q1/Q2 accepted defaults.
- `tasks.md` — Reconciled status, base SHA, and owner-directed `ForestMaker/` paths.
- `checklists/requirements.md` — Reconciled header metadata only; **all checkboxes remain unchecked `[ ]`** (reviewer's responsibility).

#### Core Engine (`Tools/MultiRackSitkaP1Lab/src/Core/`)
- `src/Core/MultiArea/ForestMaker/ForestMakerRecipe.cs` *(new, 375 lines)* — `ForestMakerSpeciesMix` enum (`PureSitka`, `SitkaPlusEdgeBroadleaves`, `CustomSplit`), `ForestMakerBounds` constants, immutable `ForestMakerRecipe` record with integer round-half-up `TotalStems()`, ordered typed `P1Refusal` checks (spec §5.1), canonical JSON encode/decode/SHA-256 hash using `CanonicalWriter`/`StrictJson`/`DerivedManifest.Sha256Hex`, and `ForestMakerPresets` (`SitkaEdgeBroadleaves064`, `PureSitka064`, `FindByPresetId`, `ByPresetId`, `ExperimentIdFor`).
- `src/Core/MultiArea/ForestMaker/ForestMakerGenerator.cs` *(new, 517 lines)* — `ForestMakerLaneSummary`, `ForestMakerResult`, and deterministic integer-only `ForestMakerGenerator.Generate(recipe)` implementing spec §5.2 rules 1–10 and §5.3 starting attributes (age-20 `Enlarged80` transfer, Q1 piecewise-linear custom ages `10→70, 20→160, 30→240, 40→300, 47→345, 60→400`, Q2 age-60 edge broadleaf MA-1 Area B transfer), plus `CreateSession(...)` convenience helper.
- `src/Core/RngSalts.cs` *(+5 lines)* — Registered `internal const string ForestMaker = "FOREST-MAKER-";` and appended it to `RngSalts.Families`. Zero existing salt values modified.
- `src/Core/MultiArea/MultiAreaExperiment.cs` *(+4/-1 lines)* — Added null-preserving `internal string? GeneratorNoteOverride { get; init; }` used in `Write()` as `GeneratorNoteOverride ?? <existing MA-1 literal>`. When `null`, MA-1 and MA-1-MIXED bytes remain byte-identical.

#### Tests & Gate Baseline (`Tools/MultiRackSitkaP1Lab/`)
- `tests/Core.Tests/ForestMakerTests.cs` *(new, 675 lines)* — 7-case Phase 2A-1 verification suite covering P2A-01 through P2A-06, Anchors 10–11, and 2A1-06 Economy interop on the real generated primary preset.
- `tests/Core.Tests/Program.cs` *(+1 line)* — Registered `ForestMakerTests.Register();`.
- `build/2d-gate/baseline-counts.json` *(+2/-2 lines)* — Tightened `suites.core.minPassed` upward from `291` to `298` (see Design Decisions §2).

---

## 2. Design Decisions & Naming Reconciliation

1. **Canonical preset IDs vs dispatch human aliases:**
   Canonical recipe identities and manifest bytes use the spec's exact IDs verbatim; the dispatch's descriptive aliases are human labels only:
   - `canonical-064ha-mixed-edges` → `fm-064-sitka-edge-broadleaves-v1` / `mad-fm-064-sitka-edge-broadleaves-v1`
   - `canonical-064ha-pure-sitka` → `fm-064-pure-sitka-v1` / `mad-fm-064-pure-sitka-v1`
2. **Integer-only determinism (spec §5.2):**
   All lane boundaries (`x_i = floor(i · W_mm / N)`), lane quotas (largest remainder, ties to lower lane index), integer square root (`ISqrt`), jittered lattice coordinates, broadleaf exclusion (`2,000 mm` exact squared distance), lane-edge margin (`500 mm` via `MultiAreaGeometry.InsideWithMargin`), starting DBH/height, and SplitMix64 mixing use integer arithmetic only.
3. **Hard stem cap (`MaxLivingStems = 3,000`) & refusal ordering:**
   `ForestMakerRecipe.RequireValid()` enforces the exact spec §5.1 refusal order before any tree allocation: `ForestMakerGeneratorUnsupported` → `ForestMakerPresetUnknown` → `ForestMakerPresetMismatch` → `ForestMakerWidthOutOfRange` → `ForestMakerLengthOutOfRange` → `ForestMakerLaneCountOutOfRange` → `ForestMakerAgeOutOfRange` → `ForestMakerDensityOutOfRange` → `ForestMakerSeedOutOfRange` → `ForestMakerSpeciesMixInvalid` → `ForestMakerStemCapExceeded` → `ForestMakerLaneGridAlignment` → `ForestMakerLaneEmpty` (and `ForestMakerInfeasibleStocking` during candidate selection; `ForestMakerRecipeNotCanonical` on strict canonical decode).
4. **Starting attributes (accepted Q1/Q2 defaults, spec §5.3):**
   - Age-20 Sitka: `DBH_mm = clamp(160 + (S - 2000) * 40 / 1000, 80, 240)`, `Height_mm = max(3500, 2000 + 62 * DBH_mm)` (`TRANSFER / GAMEPLAY CALIBRATION`).
   - Age-60 edge broadleaves (Q2): MA-1 Area B older cohort (`mean 345, spread 80, clamp 220..480, Height_mm = 27200 + 20 * (DBH_mm - 345)`) (`TRANSFER / RESEARCH REQUIRED`).
   - Custom ages 10–60 (Q1): piecewise-linear table `10→70, 20→160, 30→240, 40→300, 47→345, 60→400` mm, spread `mean/4`, clamp `[mean/2, 3*mean/2]`.
5. **RNG salt registration (`src/Core/RngSalts.cs`):**
   Added `RngSalts.ForestMaker = "FOREST-MAKER-"` to `RngSalts` and `RngSalts.Families` (referenced via `ForestMakerGenerator.StreamNamespaceKey`). Covered automatically by `RngSaltsTests` (reflection, distinctness, prefix-freedom) and gate G6 (`RNG_patterns=10, RNG_violations=0`). No existing salt value was changed.
6. **`baseline-counts.json` Core floor tightening (`291 → 298`):**
   `selftest-r1` proved that leaving `suites.core.minPassed` at `291` causes `selftest.sh` case `C09-g2-dropped-test` (which skips 1 test in `Model0FingerprintTests.cs`, producing `297/297`) to pass G2 (`297 >= 291`) instead of triggering the required `GATE G2 FAIL`. Per the dispatch rule ("Do NOT edit baseline-counts.json unless a check actually requires it") and the C1 precedent (`5913cf83`), `suites.core.minPassed` was tightened upward from `291` to `298` (commit `c335559c`), never weakened, restoring `C09` PASS and `29/29` selftest PASS.
7. **Zero changes to frozen anchors, biology, or economy:**
   `build/2d-gate/frozen-anchors.json` stays locked at anchors 1–9. Anchors 10–11 are pinned as test constants in `tests/Core.Tests/ForestMakerTests.cs`.

---

## 3. Canonical 0.64 ha Presets & Anchors 10–11

Both presets are `80.0 × 80.0 m` (`0.64 ha` total), 5 equal edge-to-edge `16 × 80 m` lanes (`MA-A`..`MA-E`, X boundaries `0 / 16 / 32 / 48 / 64 / 80 m`, `0.128 ha` / `1,280 m²` each), age 20 Sitka, `2,100 stems/ha`, **1,344 living stems**, and **1,024 2.5 m light cells** with per-lane counts **`192 / 224 / 192 / 224 / 192`**.

| Property | Primary Preset (Anchor 10) | Secondary Control Preset (Anchor 11) |
|---|---|---|
| **Canonical `PresetId`** | `fm-064-sitka-edge-broadleaves-v1` | `fm-064-pure-sitka-v1` |
| **Canonical `ExperimentId`** | `mad-fm-064-sitka-edge-broadleaves-v1` | `mad-fm-064-pure-sitka-v1` |
| **Dispatch human alias** | `canonical-064ha-mixed-edges` | `canonical-064ha-pure-sitka` |
| **SpeciesMix** | `SitkaPlusEdgeBroadleaves` | `PureSitka` |
| **Generator Seed** | `6,400,101` | `6,400,202` |
| **Lane Seeds (`MA-A..MA-E`)** | `64001011, 64001012, 64001013, 64001014, 64001015` | `64002021, 64002022, 64002023, 64002024, 64002025` |
| **Composition (total 1,344)** | 1,336 Sitka (age 20) + 4 Sessile Oak (age 60) + 4 European Beech (age 60) | 1,344 Sitka (age 20); empty `GenesisSpecies` |
| **Per-lane counts (`MA-A..MA-E`)** | `A: 265S+2O+2B (269)`, `B/C/D: 269S`, `E: 264S+2O+2B (268)` | `269 / 269 / 269 / 269 / 268` (all Sitka) |
| **Edge broadleaf positions** | West `MA-A` `x=1,250 mm`, `y=10/30/50/70 m` (`O,B,O,B`); East `MA-E` `x=78,750 mm`, `y=10/30/50/70 m` (`B,O,B,O`) | None |
| **2.5 m Cells (`MA-A..MA-E`)** | `192 / 224 / 192 / 224 / 192` (total `1,024`) | `192 / 224 / 192 / 224 / 192` (total `1,024`) |
| **Canonical `RecipeHash` (SHA-256)** | `f68d3e73b3742d0015145790cbeaa07555dac610ae78f684b422f2a480eb6e33` | `73d184de61477246b8799dd28a415f468fefc2bb7f7b3438d4432dae13df0e22` |
| **Manifest `ContentHash` (SHA-256)** | **`d79fabccf3a9447666c436a08c04895c58934fde19c5b5aaf072103612ae4fde`** *(Anchor 10)* | **`de0eb3f63d13423336348d3cd4f398b9c4f6ce23d77578e902c25960265a5945`** *(Anchor 11)* |
| **5-Year Phase 2 Repeat Hash** | `a000137663d72fcbf8e7d73cd039c1da95d28e6e50d22fdc8d792acf8650dd28` | `eeb0c37817699ad97bcdb08a13a091e76312d5b12b3d6d82a498a78cd3e37501` |

---

## 4. Test Inventory (`tests/Core.Tests/ForestMakerTests.cs`)

Core test count increased from `291` to **`298`** (+7 new cases, 0 failed):

1. `PASS forestmaker P2A-01a: recipe bounds table (min/max/min-1/max+1), exact 3000 cap, typed refusals and canonical decode`
   - Tests min/max/min-1/max+1 for WidthM, LengthM, LaneCount, StartAgeYears, StemsPerHa, Seed; exact 3,000 accepted (100×100 @ 3,000/ha and 101×100 @ 2,970/ha) and exact 3,001 refused (101×100 @ 2,971/ha) plus 4,320 refused (120×120 @ 3,000/ha); every typed refusal (`ForestMakerGeneratorUnsupported`, `ForestMakerPresetUnknown`, `ForestMakerPresetMismatch`, all 6 out-of-range codes, `ForestMakerSpeciesMixInvalid`, `ForestMakerStemCapExceeded`, `ForestMakerLaneGridAlignment` for both 2.5 m and 0.25 m centre lines, `ForestMakerLaneEmpty`, `ForestMakerInfeasibleStocking`, `ForestMakerRecipeNotCanonical`); canonical bytes and `RecipeHash` round-trip.
2. `PASS forestmaker P2A-01b: corner matrix (64 cases), CustomSplit and custom cohort ages`
   - Exercises all 64 `{min, max}` combinations across width, length, lanes, age, density and 2 species mixes (zero non-`P1Refusal` exceptions); verifies `CustomSplit` (25% Oak / 25% Beech / 50% Sitka and 100% Oak), custom experiment ID format `mad-fm-custom-<16 hex>`, and exact Q1 age anchors (`10→70`..`60→400`).
3. `PASS forestmaker P2A-02: determinism, seed sensitivity, geometry margins, 4-digit IDs, 5-year repeat and MA-1 invariance`
   - Equal recipe+seed → byte-identical manifests; different seed → different positions, same counts; unchanged `Validate()`; every stem strictly inside its lane and `>= 500 mm` from all edges; globally unique positions; 4-digit ordinal IDs (`MA-A-0001`..); unique lane seeds distinct from generator seed; identical 5-year Phase 2 repeat hash on both presets; `RngSalts.ForestMaker` registration; MA-1 and MA-1-MIXED unchanged (`GeneratorNoteOverride == null`).
4. `PASS forestmaker P2A-03 & P2A-04: primary and pure-Sitka 0.64 ha presets exact and anchors 10-11 pinned`
   - Asserts exact geometry, `0.128 ha` / `"1280"` per lane, species/age counts, per-lane counts, exact broadleaf positions and alternating species, `>= 2,000 mm` Sitka-to-broadleaf exclusion, pure Sitka lanes B–D, `1,024` 2.5 m cells (`192/224/192/224/192`), disclosure/note overrides, and pinned Anchors 10 and 11.
5. `PASS forestmaker P2A-05: species constants respected (maturity, C2 broadleaf genesis crowns, starting sizes)`
   - Asserts `Phase2SeedRain.Maturity(Oak/Beech, MaturityFullYears) == 1` and `Stage5SeedRain.Maturity(MaturityOnsetYears) == 0` from constants; verifies all 8 broadleaf genesis crowns equal `Phase2Crown.TargetRadiusM` bit-for-bit over the connected woodland; verifies §5.3 DBH/height formulas for all stems.
6. `PASS forestmaker P2A-06: connected 5-lane ecology (Hegyi CI, per-lane RD, cross-lane seed, no outside leakage, 15y advance)`
   - (a) `MA-B` Sitka within 8 m of `MA-A` has `woodlandCi > laneOnlyCi` and cross-border neighbours; (b) `Stage4View.Areas[i].RelativeDensity` matches per-lane `0.128 ha` RD (not pooled `0.64 ha`), and density routing is mixed for `MA-A`/`MA-E` and pure Sitka for `MA-B/C/D`; (c) primary preset delivers non-zero Oak and Beech seed rain into `MA-B` within 5 years while pure-Sitka control has `0.0` Oak/Beech seed rain everywhere; zero out-of-polygon stems/cells and open-grassland light `1.0f` outside perimeter; (d) both presets advance 15 full years under `1/1/1/1/1` without refusal.
7. `PASS forestmaker 2A1-06: Economy interop on real generated 0.64 ha primary preset`
   - Loads the real generated `fm-064-sitka-edge-broadleaves-v1` stand with `MultiAreaModelSelection.Phase2WithEconomy` (`EconomyModelVersion.Enabled = 1`); asserts `€12,000` (`1,200,000` cents) opening cash, contractor-only Fell (`EconomyFellContractorOnly`), pure quote, `€2,500` (`250,000` cents) minimum harvest fee, Approve → Execute settlement, clean 1-year advance (year 1, 1 year-end summary), and upfront cash refusal (`EconomyInsufficientCash`) on a `€2,000` opening balance with zero mutation.

---

## 5. Verification Gate & Selftest Results

Toolchain: WSL Ubuntu, `/home/jer/.dotnet/dotnet` SDK **`8.0.134`** (resolved via `global.json` `rollForward: latestPatch` over `8.0.100`), external verified P0 seed `/home/jer/Development/CCF-Multi-2D-Phase2A1-Evidence-20261126/p0ref-seed` (`9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3`), evidence root `/home/jer/Development/CCF-Multi-2D-Phase2A1-Evidence-20261126` (outside repo, mode `700`).

### Final Passing Runs on Verified Commit `c335559c9a5e958027dc5290df0407687c013b54`

| Check | Result | Evidence Path |
|---|---|---|
| **`build/2d-gate.sh --browser`** (`gate-r3`) | **`VERDICT: PASS (exit=0; NETWORK ISOLATION: PROVEN; P0 REFERENCE: PRE-SEEDED VERIFIED 9a99f781...34b3)`** | `/home/jer/Development/CCF-Multi-2D-Phase2A1-Evidence-20261126/gate-r3` & `gate-r3.console.log` |
| G0 Preflight | `GATE G0 PASS` | `gate-r3/preflight.json` |
| G1 Clean Builds (`0/0` warn/err) | `GATE G1 PASS` (`.sln`, `SitkaP1.Ui.csproj`, `Phase1Save.Tests.csproj`) | `gate-r3/g1-build-*.log` |
| G2 Core.Tests | `Core.Tests PASS (298/298 passed, 0 failed)` | `gate-r3/g2-core-tests.log` |
| G2 Phase1.Save.Tests | `Phase1.Save.Tests PASS (50/50 passed, 0 failed)` | `gate-r3/g2-save-tests.log` |
| G2 `ui-smoke-multiarea.sh` | `ui-smoke-multiarea.sh PASS (363 passed, 0 failed)` | `gate-r3/g2-ui-smoke-multiarea.log` |
| G2 `ui-smoke.sh` | `ui-smoke.sh PASS-BASELINE-DRIFT (250 passed, 4 known T0 #885 failures: L01,L02,L06,J31; G11b PASS)` | `gate-r3/g2-ui-smoke-single.log` |
| G2 `cli-safety.sh` | `cli-safety.sh PASS (23 passed, 0 failed)` | `gate-r3/g2-cli-safety.log` |
| G2 `build-output-safety.sh` | `PASS (C1-C6+P0 PASS; HOST-NA: C7-cold-build,C8-rebuild; .p0ref restored & re-verified)` | `gate-r3/g2-build-output-safety.log` |
| G2 `p0-isolation.sh` | `PASS (decoy=13/13, protected hits=0, unresolved=0)` | `gate-r3/g2-p0-isolation.log` |
| G2 S0c asset / graph / reconstruct | `S0c asset identity PASS`, `MODULE-GRAPH PASS` (14 modules), `JS-RECONSTRUCT PASS` | `gate-r3/g2-s0c-*.log` |
| G3 Frozen SHA-256 Anchors | `9/9 frozen SHA-256 anchors verified` (`GATE G3 PASS`) | `gate-r3/g3-anchors.json` |
| G4 Save v2 Golden & Phase1 Freeze | `v2 golden verified (bytes=24703, SchemaVersion=2, src/Core/Phase1 unchanged)` (`GATE G4 PASS`) | `gate-r3/gate.log` |
| G5 Payload Budgets | `5/5 PASS: M11, W137, W149, W258, M13` (`GATE G5 PASS`) | `gate-r3/gate.log` |
| G6 Core Determinism + RNG Salts | `76 files scanned; F1=0, F2_allowed=4, F2_unlisted=0, RNG_patterns=10, RNG_violations=0` (`GATE G6 PASS`) | `gate-r3/g6-determinism.json` |
| G7 Protected-Path Guard | `92 paths checked; wwwroot tree=dabdbd7fa07c...; P0 tree=f212c8f64fb2...` (`GATE G7 PASS`) | `gate-r3/g7-protected-paths.json` |
| Stage B Edge Browser Suites | `GATE B PASS`: Save Edge `63/63`, Step 2 Edge `67/67`, S0c Parity `36/36 outerHTML byte-identical` | `gate-r3/stage-b-*.log` |
| **`build/2d-gate/selftest.sh`** (`selftest-r4`) | **`OVERALL: PASS (29/29 passed, 0 failed; all exit codes 0,1,2,3,4,5,64 proven)` (exit 0)** | `/home/jer/Development/CCF-Multi-2D-Phase2A1-Evidence-20261126/selftest-r4` & `selftest-r4.console.log` |

### Preserved Prior Run Logs (Kept Intact per Protocol)
- `gate-r1` (`07bdb34c`): failed G2 solely on the pre-existing frozen `NB1c` OS-scheduling race in `Phase1.Save.Tests` (`49/50`, `expected 1, got 3`). Core passed `298/298`.
- `gate-r2` (`07bdb34c`): `VERDICT: PASS (exit=0)` (all G0–G7 + Stage B PASS before `baseline-counts.json` tightening).
- `selftest-r1` (`07bdb34c`, stopped early after C09 confirmed): revealed `C09` produced `297/297` which still exceeded old floor `291` (`GATE G2 PASS`), proving `baseline-counts.json` tightening (`291 → 298`) was required.
- `selftest-r2` (`c335559c`): `28/29 passed, 1 failed` — `C21-g2-asset-identity` exhausted all 3 retries on the pre-existing frozen `NB1c` OS-scheduling race (`49/50`, `expected 1, got 3`).
- `selftest-r3` (`c335559c`): interrupted mid-`C21` by a host server restart (`GATE UNEXPECTED_TERMINATION`).

---

## 6. Known Limitations, Open Questions & Honest "Not Verified" List

### Known Limitations & Deviations
1. **SDK patch version:** Local WSL toolchain resolves `.NET SDK 8.0.134` under `global.json` (`rollForward: latestPatch`), so cold-rebuilding the frozen `8.0.133` `.p0ref/SitkaLab.Core.dll` pin is `HOST-NA` (`C7-cold-build`, `C8-rebuild`); verified pre-seeded `.p0ref` mode was used throughout (`9a99f781...34b3` verified and restored).
2. **Pre-existing frozen `NB1c` OS-scheduling race:** `tests/Phase1.Save.Tests/Program.cs` (`NB1c`) intermittently reports `expected 1, got 3` under WSL2 scheduling load (`gate-r1`, `selftest-r2` C21, and several `selftest-r4` retry notices). Untouched because `Phase1.Save.Tests` and `src/Core/Phase1/**` are frozen in this lane.
3. **Research-flagged starting sizes:** Per spec §5.3, age-20 Sitka sizes are a simplified transfer of `Enlarged80`, edge broadleaves (Q2) reuse MA-1 Area B older-cohort sizing (`RESEARCH REQUIRED`), and custom-age anchors outside 20/47 (Q1) are `GAMEPLAY CALIBRATION — RESEARCH REQUIRED`. Every generated manifest carries `ForestMakerBounds.Disclosure` stating this explicitly.
4. **Handoff commit after selftest:** This Markdown-only handoff commit sits directly above `c335559c` (zero code, test, spec, or gate-data changes). Forbidden-path diff remains `0`.

### Not Verified in Step 2A-1
- **Phase 2A-2 scope (out of scope for 2A-1):** Save Schema v3 serialization/loading, `/multi` New Woodland modal and `api/multi/woodland/*` routes, `MultiSessionHost` atomic swap, Active Lane + Visible Neighbours SVG view, mini-map, 1-lane toggle, and M0 0.64 ha browser/payload speed qualification.
- **Unity Editor / 3D project:** `Assets/`, `Packages/`, `ProjectSettings/` were neither modified nor compiled in Unity.
- **Cold `.p0ref` rebuild (`C7`/`C8`):** `HOST-NA` on `.NET 8.0.134` (pre-seeded verified P0 reference used).
- **Independent review:** `specs/multi-2d-phase2a-forest-maker-v1/checklists/requirements.md` remains 100% unchecked for the independent reviewer.
