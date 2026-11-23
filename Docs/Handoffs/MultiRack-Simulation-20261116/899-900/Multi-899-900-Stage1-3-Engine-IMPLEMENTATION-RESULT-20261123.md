# Multi #899 / #900 — Stage 1–3 engine implementation result (2D Sitka engine)

**Statuses:** IMPLEMENTED — **yes** (on the task branch, not integrated). NUMERICALLY QUALIFIED — **author-run offline checks only** (listed below; an independent qualifier has not repeated them). REVIEWED — **no** (needs an independent reviewer). ACCEPTED — **no** (owner G2 decision).

| Item | Value |
|---|---|
| Branch | `feat/multi-899-900-crown-canopy-light-20261123-v1` (not pushed by the implementer) |
| Base / context commit | `1e4e02215d373e224fe6a74074ae3f62983db242` |
| Implementation HEAD | `095df3c0`, then the review-repair commit on top of `9799914d` (see "Repair pass" below) |
| Worktree | `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-899-900-Engine-20261123` |
| Authority | `origin/docs/multi-899-900-approved-spec-20261123-v1`: both APPROVED spec quartets and D-094 |
| SDK | `/home/jer/.dotnet/dotnet` 8.0.134; `.p0ref/SitkaLab.Core.dll` sha256 `9a99f781…1934b3`, unchanged and not rebuilt |
| Tools used | Bash, PowerShell→WSL, file read/edit. **No Unity or Editor, no `/multi` run, no browser.** |

## Commits (oldest first)

1. `02532b85` test: pin the CrownModel 0 output fingerprint, captured **before** any `src/Core` change.
2. `950ad348` Stage 1: `SpeciesProfile` and neutral `SiteConditions`, with no maths change.
3. `d958b062` Stage 2 CrownModel 1 (C2) and the Stage 3 `CanopyLightEvaluator` engine, plus the C2 tests.
4. `095df3c0` Stage 3 tests: oracles, Unity parity and the matched control.
5. `9799914d` handoff (first version; reviewed: NEEDS CHANGES).
6. Repair commit: Layer B clipped-area weighting, the 10-tree matched-control gate with crown evidence, independent oracles, and this updated handoff.

## Repair pass (independent review of `9799914d`: NEEDS CHANGES)

| Finding | Fix | Evidence |
|---|---|---|
| MAJOR: Layer B counted whole 0.0625 m² micro-cells (area outside the rack) and divided by sampled cells | `CanopyLightEvaluator.Cover` now weights every included micro-cell (strict-inside centre rule kept) by its exact polygon-clipped area, using the same Sutherland–Hodgman helper as the light layer (cells with all four corners inside are whole). `CoveredM2` = Σ clipped covered; `UncoveredM2` = polygon area − covered; `CoverFraction` = covered / polygon area; `OverlappedM2` and the largest opening area are Σ clipped; `MeanOverlapCount` is area-weighted; `SampledAreaM2` (Σ clipped) and `PolygonAreaM2` both stay visible. No record shape changed. | `light O-07` (new, sloped triangle), `light P-02` (now asserts covered + uncovered = polygon area within 1e-9 relative and fraction = covered / polygon) |
| MAJOR: the matched control used 12 trees; #899 C07 / #900 R11 require 10 | `light MC-01` is now the **10-tree gate**, with crown-radius distributions (Y0 / Y+1 / Y+3) and annual crown expansion of surviving trees, for control and treated. The 12-tree run is kept as `light MC-02`, labelled supplementary. | tables below |
| MINOR: implementation used as its own oracle | `crown C2-00`: five hand-computed C2 radii and one hand Hegyi CI → r_0 case, asserted against literal values to 1e-6, never derived from `CrownModel`. `light O-08`: duplicate crowns give the same union as one (exactly), and two r = 3 m crowns 3 m apart match the analytic union and lens. The existing non-overlap test `O-04` is kept. | oracle values below |

`crown C2-05` still uses its own 12-tree Fell (a radius-jump test, not the matched control). It was not part of the findings and is unchanged.

## Exact changed files (vs base)

```
Tools/MultiRackSitkaP1Lab/src/Core/AnnualRules.cs                      (modified)
Tools/MultiRackSitkaP1Lab/src/Core/CanopyLightEvaluator.cs             (new)
Tools/MultiRackSitkaP1Lab/src/Core/CrownModel.cs                       (new)
Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/MultiAreaContinuation.cs  (modified)
Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/MultiAreaExperiment.cs    (modified: two new constants only)
Tools/MultiRackSitkaP1Lab/src/Core/SpeciesProfile.cs                   (new)
Tools/MultiRackSitkaP1Lab/tests/Core.Tests/CanopyLightTests.cs         (new)
Tools/MultiRackSitkaP1Lab/tests/Core.Tests/CrownC2Tests.cs             (new)
Tools/MultiRackSitkaP1Lab/tests/Core.Tests/Model0FingerprintTests.cs   (new)
Tools/MultiRackSitkaP1Lab/tests/Core.Tests/Program.cs                  (3 registration lines)
Docs/Handoffs/MultiRack-Simulation-20261116/899-900/<this file>        (new)
```

No changes to `src/Ui/**` (including `PlayerMultiApi.cs`), `src/Cli/**`, `Assets/**`, fixtures/goldens, `.sln`, `global.json`, build scripts, P0, saves or `main`. No new packages and no deleted files. `git diff --check` is clean, and all files use LF like the base.

## What was built

### Stage 1 — engine readiness (no maths change)
- **Dependency boundary:** `src/Core` has no UI, Unity or HTTP dependency. A grep for `UnityEngine|System.Net|Http*|SitkaP1.Ui|AspNetCore` finds nothing. Its only references are the pinned P0 binary and the two pure-`System` Scenario One files it already linked (`SitkaGrowthModel.cs`, `SimulationRandom.cs`).
- **Contracts:** the existing MultiArea immutable views and fenced commands were extended, not rewritten. I added one constructor overload, one read method and one counter. No positional record changed shape, so the UI JSON is unchanged.
- **`SpeciesProfile.Sitka`** holds the frozen values: crown intercept 0.9415, per-cm 0.07635, floor 0.1, Ci50 5, k 0.15, DBH growth 1.2 cm, DBH scale 120 cm, Hegyi radius 8 m, distance floor 0.5 m. `AnnualRules` now reads them from the profile with unchanged arithmetic. The linked `SitkaGrowthModel` density/mortality constants stay in their read-only source.
- **`SiteConditions`** has four neutral multiplier slots (moisture/drainage, fertility, exposure/climate, rooting stability), all fixed at 1. **No rule reads them.** `RequireNeutral()` refuses non-neutral values (`SiteEffectNotModelled`) rather than ignoring them silently.

### Stage 2 — CrownModel 1 (C2)
- `CrownModelVersion { LegacyFrozen = 0, AreaScaledEquilibriumC2 = 1 }`. **The default is 0 everywhere.** The UI's `MultiAreaWorkPlanAdapter.OpenSynthetic()` still builds a model 0 session.
- The model 1 rule id is `area-scaled-equilibrium-c2-v1`. The equations are implemented in float32 in `CrownModel`, exactly as spec #899 §"Approved model" gives them:
  - `r_pot = max(0.1, 0.9415 + 0.07635·DBH)`
  - `r_target = max(0.1, r_pot / sqrt(1 + CI/5))`
  - `r_0 = r_target(CI_0)`
  - `r_new = max(0.1, r + (r_target − r)·0.15)`
- **CI** is the existing `AnnualRules.Competition` Hegyi sum (8 m, 0.5 m floor, self-excluded). Model 1 evaluates it over **all living trees of both areas** in ordinal TreeId order (`CrownModel.Woodland`). Each year it is read from the start-of-year root, so area order cannot change any result and no area mutates another. A test checks A-then-B against B-then-A.
- **Year 0:** `new MultiAreaContinuation(experiment, C2)` derives `r_0 = r_target(CI_0)` over the genesis woodland when the session starts. The experiment manifest, its content hash `c66d566c…` and the frozen `p1d-30m-sitka-adult-v1` fixture are **unchanged**. Model 1 is a derived session path, not a new fixture. Model 1 sessions carry the token lineage `AreaModelVersion = "ma1-connected-woodland-crown-c2-v1"` and their own `EdgeModel` text. Model 0 keeps `ma1-isolated-per-area-v1`.
- **Work Plans, approvals, events, stand summaries, seeds and annual stepping stay per area.**
- **Old saves and scenarios:** this lab has no save. Every existing caller (UI, CLI, single-area `AnnualEngine` and `AnnualContinuation`) still gets model 0. Model 1 exists only for the MA-1 multi-area continuation, and only when selected explicitly.

### Stage 3 — `CanopyLightEvaluator` (pure, read-only)
- **Inputs:** the immutable trees of each area plus its polygon (`CanopyArea`). Only living trees are used: stumps (`harvest`) and biologically dead trees are excluded through `IsLiving`.
- **Layer A:** for each living tree, the connected-woodland CI, living neighbours within 8 m, how many of those are cross-border, and `CrownRadiusM`. For each rack, stems/ha and basal area m²/ha come from the engine's own `WorkRules.Summarize`. No m²/tree is computed.
- **Layer B:** a 0.25 m micro-grid of cell centres strictly inside the polygon. Every living crown circle in the woodland is rasterised onto it, and each included micro-cell is weighted by its **exact polygon-clipped area**. The layer reports covered m² (the **union**, Σ clipped), uncovered m² (= polygon area − covered), fraction (= covered / polygon area), overlapped m² (≥2 crowns, Σ clipped), the area-weighted mean overlap count over the covered area, the number of 4-connected openings, and the largest opening's area (Σ clipped) and inscribed width. `SampledAreaM2` (Σ clipped areas of included cells) is reported beside `PolygonAreaM2`; the unsampled boundary strip is counted as uncovered, never renormalised away. Width comes from an exact Euclidean distance transform: width = (2d − 1) cells, where d is the distance to the nearest crown or polygon edge.
- **Layer C:** `unity-k10a10-decoupled-v1`. Cells are anchored at the world origin. A cell is included only if its centre is inside the polygon, and it is weighted by the exact area of the polygon clipped to the cell (Sutherland–Hodgman). The per-rack mean is weighted by clipped area. `SampledAreaM2` is reported next to `PolygonAreaM2`, so edge loss is visible and never renormalised away. The kernel follows Unity's float operation order: `reach = r·1 + 2.5`, lateral/vertical/influence clamp01, `gap *= 1 − influence` in TreeId order, then `light = 1 − clamp01(1 − gap)`. The grid is 2.5 m by default. 5.0 m is the parity mode, and no other grid size is accepted.
- **Metadata (`LightModelMetadata`):** version, `kind = "index"`, `calibratedPar = false`, the label `Modelled Light Index (Unity k10a10 proxy — not calibrated PAR)`, the grid, buffer 2.5, reach factor 1, vertical height 8, micro-cell 0.25, CI scope, crown rule id, caveat text, and the five uncertainties from spec #900 §3. The uncertainties are C2 bias (~+5–15 %), Area A stocking (~74 m²/ha), outer-edge brightening, the circular-crown simplification and non-causal status.
- **Recompute triggers:** `MultiAreaContinuation.ReadCanopyLight(grid)` caches one read-out per grid, keyed by `(AnnualRevision, WorkRevision)`. Tree state changes only on Execute or an annual step (which covers natural death and crown growth). Pending, Approve, Crop marks and reads therefore return the same cached object, and the recompute counter does not move. The read-out is session-only: it is never saved and never part of a token.

## Commands and results

All commands were run in WSL from `Tools/MultiRackSitkaP1Lab`, with `TMPDIR` set outside the repository.

```
/home/jer/.dotnet/dotnet build tests/Core.Tests/SitkaP1.Core.Tests.csproj    → 0 warnings, 0 errors
/home/jer/.dotnet/dotnet tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests.dll  → 160/160 passed, 0 failed (repair pass; was 156/156)
/home/jer/.dotnet/dotnet build src/Core/SitkaP1.Core.csproj   → 0 warnings, 0 errors
/home/jer/.dotnet/dotnet build src/Cli/SitkaP1.Cli.csproj     → 0 warnings, 0 errors  (unchanged source, compiled against new Core)
/home/jer/.dotnet/dotnet build src/Ui/SitkaP1.Ui.csproj       → 0 warnings, 0 errors  (unchanged source, compiled against new Core)
sha256sum .p0ref/SitkaLab.Core.dll → 9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3
```

On the base, the suite had 135 tests and all passed. With the two fingerprint tests added, and before any `src` change, the count was 137/137. All 135 pre-existing tests still pass. The 25 new tests are: fingerprint 3, crown C2 9 (incl. `C2-00` hand oracles), light 13 (incl. `O-07`, `O-08`, `MC-02`).

## Model 0 hash, before and after

| Fixed run | Captured on base before any change | After Stage 1 | After Stage 2/3 (implicit and explicit `LegacyFrozen`) |
|---|---|---|---|
| p1d-30m fixture, 10 genuine years | `7e3a2a869ec1662458b74875cadee1e4b16cf2b87b0aaa05df2d9a0061311ece` | identical | identical |
| MA-1 two areas, Fell 12 A + 3 B at Y0 and Y3, 10 years | `a692a6d5bc374dd0225153b3bf0e0b0e9467d3c041b0a6b03414f42d3c02233d` | identical | identical |

The fingerprint is SHA-256 over the IEEE-754 bits of every tree, audit row (CI, suppression, probability, roll), stand and death. For reference, the model 1 fingerprint on the same MA-1 run is `814e81650b62428510fe60c3142b51d9055537d6014303074f08733bf8d46287` and is deterministic.

## Spec conformance and test evidence

| Requirement | Evidence (test) | Result |
|---|---|---|
| Model 0 exact parity | `model0 F-01/F-02`, `crown C2-01` | PASS (hashes above) |
| Year-0 equilibrium on both areas (#899 C02) | `crown C2-02`: every tree has `r == r_target(CI_0)` bit-exact; only `CrownRadiusM` differs from model 0 genesis | PASS |
| 15 % cap, finite, within [0.1, r_pot] (C03) | `crown C2-04`: each surviving tree, 10 years, bit-exact C2 step; largest untreated one-year change **0.0672 m** | PASS |
| No multi-metre jump after one Fell | `crown C2-05`: 12-tree Fell; largest change in the following year **0.0447 m** | PASS |
| Cross-border neighbours at the shared border (C04, R08) | `crown C2-06`: `MA-A-060` has 20 own-area neighbours + **3 cross-border**; CI 6.064 → 6.745; r_0 uses the woodland CI | PASS |
| Outer perimeter: no synthetic neighbour | `crown C2-06`: westernmost A and easternmost B stems; neighbour count equals the brute-force count of real trees within 8 m; woodland CI is bit-equal to own-area CI | PASS |
| Determinism and area-order independence (R12) | `crown C2-03`, `light P-02` | PASS |
| Lifecycle mark → refusal → approve → execute → advance (C05, R10) | `crown C2-07` for model 0 and model 1: refusals (`MultiAreaWrongArea`, `PlayerPlanStateStale`) leave the view byte-identical; Approve changes no tree and no canopy value (same cached object); Execute fells exactly the approved IDs, leaves the year at 0 and recomputes once; Advance gives Year 1 and recomputes once | PASS |
| Light oracles (R01–R05) | `light O-01…O-06` (values below) | PASS |
| Exact 5.0 m Unity parity (R06) | `light P-01`: every one of 101 cells is bit-identical to a literal transcription of `RecomputeCanopy` (blob `0290a7be`), for models 0 and 1; at 2.5 m the reach stays r + 2.5 (383 cells differ from the coupled r + 1.25) | PASS |
| Union, not sum; uncovered and openings (C06, R07) | `light P-02/P-03`, `O-08` (duplicate/overlapping crowns), MC table | PASS |
| Cover clipped to the rack polygon | `light O-07` (sloped triangle, hand area 30.5 m²), `light P-02` | PASS |
| Independent C2 radius oracles | `crown C2-00` (literal hand values) | PASS |
| Per-rack clipped-area weighting, no blending (R09) | `light P-02`: Σw = SampledAreaM2 ≤ polygon area; boundary cells clipped; mean = Σlw/Σw | PASS |
| Metadata (R13) | `light P-02` | PASS |
| Matched control (C07, R11) | `light MC-01` **10-tree gate** (tables below); `MC-02` 12-tree supplementary | PASS |

### Oracle values (`light O-*`)

| Case | 2.5 m | 5.0 m | Target |
|---|---|---|---|
| Empty stand | 1.000 (every cell) | 1.000 | 1.000 |
| Single mature tree overhead (h 20) | 0.000 | 0.000 | 0.000 |
| Single 4 m young tree overhead | 0.500 | 0.500 | 0.500 |
| Drip line r = 1.73 m | 0.408983 | 0.408983 | 0.409 |
| Two-tree gap (r 1.73, ±2.5 m) | 0.349301; overlap 0 m² | 0.349301; overlap 0 m² | ≈0.349, 0 % overlap |
| Stump + dead tree overhead | 1.000; cover 0 | 1.000; cover 0 | 1.000 |
| Lattice (10×10, 4 m spacing, r 1.6, h 12), mean light | 0.2093 | 0.1998 (ratio **1.05**) | no grid artefact |

### Independent geometry oracles (`light O-07`, `light O-08`)

Sloped-boundary triangle (0,0) (9,1) (2,7); hand shoelace area = |9·7 − 1·2| / 2 = **30.5 m²**; every edge cuts micro-cells diagonally.

| Case | Sampled (Σ clipped) m² | Covered m² | Uncovered m² | Covered + uncovered | Fraction (÷ polygon) | Covered ÷ sampled (old basis, not reported) |
|---|---|---|---|---|---|---|
| Crown r 2 m at (5.5, 4) straddling the sloped edge | 29.658730 | 6.391369 | 24.108631 | 30.5 (≤1e-9 rel.) | 0.209553 | 0.215497 |
| Crown r 20 m covering everything | 29.658730 | 29.658730 | 0.841270 | 30.5 (≤1e-9 rel.) | 0.972417 | 1.000000 |
| Empty | 29.658730 | 0 | 30.5 | 30.5 | 0 | — |

The 0.841 m² edge loss (cells whose centre is outside) stays visible as uncovered and is below the perimeter × s/2 bound (asserted).

Crown union, r = 3 m, on the 40 m square (analytic: disc 9π = 28.274334 m²; d = 3 m lens = 6π − 1.5·√27 = 11.055327 m²; union = 18π − lens = 45.493341 m²):

| Case | Covered m² | Overlapped m² | Mean overlap | Analytic |
|---|---|---|---|---|
| One crown | 27.9375 | 0 | 1 | 28.274334 (−1.19 % raster) |
| Two identical crowns, same centre | 27.9375 (exactly = one) | 27.9375 | 2 | not doubled |
| Two crowns 3 m apart | 45.0625 | 10.8125 | — | union 45.493341 (−0.95 %), lens 11.055327 (−2.2 %) |

Tolerances are 3 % (disc/union) and 5 % (lens), the 0.25 m raster sampling error at this radius; the duplicate case is asserted exactly.

### Independent C2 radius oracles (`crown C2-00`)

Hand values from `r_pot = 0.9415 + 0.07635·DBH`, `r_target = r_pot / sqrt(1 + CI/5)`, `r_new = r + (r_target − r)·0.15`, asserted to 1e-6 against literals:

| Case | Hand value (m) | Engine float32 | \|diff\| |
|---|---|---|---|
| DBH 30, CI 0 | 3.232 | 3.232 | 1.1e-7 |
| DBH 30, CI 5 | 2.2853691167949215 | 2.2853692 | 4.1e-8 |
| DBH 30, CI 15 | 1.616 | 1.616 | 5.6e-8 |
| DBH 45, CI 2.5 | 3.574009658865889 | 3.5740097 | 2.0e-9 |
| One step from r 2.0, DBH 30, CI 5 | 2.042805367519238 | 2.0428054 | 6.6e-8 |
| Hegyi end-to-end: DBH 30 target, DBH 30 at 4 m + DBH 60 at 3 m → CI | 0.9166666666666666 | 0.9166667 | <1e-6 |
| … → r_0 | 2.971103650398022 | 2.971104 | <1e-6 |

For comparison, the superseded coupled reach (r + cell/2) on the same lattice gives 0.4931 / 0.1998, a ratio of 2.47. **The spec's "13×" figure was not reproduced on this lattice.** The test asserts only that the decoupled ratio is below 1.5 and below the legacy ratio. The exact configuration behind the original 13× is UNKNOWN to the implementer.

### Genesis read-out (C2, 2.5 m grid)

| Area | Living stems | Stems/ha | BA m²/ha | Cover (union) | Overlapped | Light cells | Sampled / polygon m² | Mean / min / max light | Cross-border cells |
|---|---|---|---|---|---|---|---|---|---|
| MA-A | 171 | 1868.9 | 74.02 | 96.42 % | 549.55 m² | 144 | 877.09 / 915 | 0.0148 / 0.0007 / 0.2200 | 2 |
| MA-B | 73 | 486.7 | 39.50 | 87.87 % | 577.74 m² | 239 | 1442.16 / 1500 | 0.1472 / 0.0101 / 0.8191 | 6 |

At 5.0 m: A has 39 cells with mean 0.0113, and B has 62 cells with mean 0.1520.

Layer B clipped-area cover at genesis (identical at both light grids; the cover grid is always 0.25 m):

| Area | Polygon m² | Cover sampled (Σ clipped) m² | Covered m² | Uncovered m² (polygon − covered) | Fraction (÷ polygon) | Overlapped m² | Largest opening m² | Before repair (unclipped cells) |
|---|---|---|---|---|---|---|---|---|
| MA-A | 915 | 912.4554 | 882.2861 | 32.7139 | 0.964247 | 549.5526 | 5.1641 | 96.60 %, uncovered 31.12 m² |
| MA-B | 1500 | 1494.3314 | 1318.0746 | 181.9254 | 0.878716 | 577.7401 | 61.1944 | 88.11 % |

Year-0 crown radius distribution (m):

| Model | Area | n | min | p25 | median | p75 | max | mean |
|---|---|---|---|---|---|---|---|---|
| Model 1 (C2) | A | 171 | 0.966 | 1.515 | 1.691 | 1.890 | 2.637 | 1.725 |
| Model 1 (C2) | B | 73 | 1.825 | 2.557 | 2.954 | 3.389 | 3.967 | 2.941 |
| Model 0 (open-grown) | A | 171 | 1.827 | 2.430 | 2.621 | 2.827 | 3.438 | 2.628 |
| Model 0 (open-grown) | B | 73 | 2.369 | 2.942 | 3.400 | 3.797 | 4.484 | 3.338 |

## Matched control — GATE: 10-tree Fell (C2, 2.5 m light grid, clipped cover; `light MC-01`)

- **Control:** an untreated clone of the same experiment.
- **Treated:** a real fenced Work Plan Mark → Approve → Execute on the **10** living Area A trees nearest (35, 9) m, beside the shared A/B border. The test asserts exactly 10.
- **Removed IDs (10):** MA-A-059, MA-A-044, MA-A-060, MA-A-058, MA-A-043, MA-A-075, MA-A-076, MA-A-029, MA-A-074, MA-A-028.
- **Fell neighbours** (living trees within 8 m of any felled stem): 37 (A 34, B 3).
- **After Approve only**, the treated read-out is identical to the control (asserted on every number).

| Stage | Run | A stems | A BA m²/ha | A cover | A covered / uncovered m² | A openings | A largest opening | A mean light | B cover | B mean light |
|---|---|---|---|---|---|---|---|---|---|---|
| Y0 before (approved) | control = treated | 171 | 74.02 | 96.42 % | 882.29 / 32.71 | 55 | 5.16 m² / 1.33 m | 0.0148 | 87.87 % | 0.1472 |
| Y0 after Fell | control | 171 | 74.02 | 96.42 % | 882.29 / 32.71 | 55 | 5.16 m² / 1.33 m | 0.0148 | 87.87 % | 0.1472 |
| Y0 after Fell | treated | 161 | 69.05 | 91.56 % | 837.79 / 77.21 | 52 | 50.53 m² / 5.75 m | 0.0489 | 87.76 % | 0.1508 |
| Y+1 | control | 166 | 74.56 | 95.72 % | 875.85 / 39.15 | 57 | 5.16 m² / 1.33 m | 0.0159 | 88.01 % | 0.1465 |
| Y+1 | treated | 160 | 71.21 | 91.44 % | 836.67 / 78.33 | 55 | 49.97 m² / 5.75 m | 0.0484 | 87.93 % | 0.1500 |
| Y+3 | control | 152 | 74.32 | 93.54 % | 855.87 / 59.13 | 61 | 8.06 m² / 1.99 m | 0.0216 | 88.48 % | 0.1435 |
| Y+3 | treated | 149 | 71.97 | 89.69 % | 820.71 / 94.29 | 59 | 48.03 m² / 5.75 m | 0.0508 | 88.43 % | 0.1469 |

| Stage | Δ A cover | Δ A uncovered | Δ A mean light | Δ B cover | 2.5 m cells changed, A | 2.5 m cells changed, B (cross-border) | Max \|Δ light\| |
|---|---|---|---|---|---|---|---|
| Y0 after Fell | −4.86 pp | +44.49 m² | +0.0341 | −0.113 pp | 23 | 3 | 0.9738 |
| Y+1 | −4.28 pp | +39.18 m² | +0.0325 | −0.083 pp | 66 | 23 | 0.9740 |
| Y+3 | −3.84 pp | +35.16 m² | +0.0292 | −0.054 pp | 136 | 23 | 0.9749 |

### Per-tree crown radius distribution (m; nearest-rank percentiles)

| Stage | Run | Group | n | min | p25 | median | p75 | max | mean |
|---|---|---|---|---|---|---|---|---|---|
| Y0 after Fell | control | A all living | 171 | 0.966 | 1.515 | 1.691 | 1.890 | 2.637 | 1.725 |
| Y0 after Fell | treated | A all living | 161 | 0.966 | 1.528 | 1.691 | 1.872 | 2.637 | 1.717 |
| Y0 after Fell | control | Fell neighbours | 37 | 0.966 | 1.528 | 1.823 | 2.003 | 2.728 | 1.800 |
| Y0 after Fell | treated | Fell neighbours | 37 | 0.966 | 1.528 | 1.823 | 2.003 | 2.728 | 1.800 |
| Y+1 | control | A all living | 166 | 0.968 | 1.517 | 1.694 | 1.904 | 2.642 | 1.731 |
| Y+1 | treated | A all living | 160 | 0.987 | 1.531 | 1.697 | 1.877 | 2.642 | 1.723 |
| Y+1 | control | Fell neighbours | 36 | 0.968 | 1.530 | 1.836 | 2.007 | 2.736 | 1.808 |
| Y+1 | treated | Fell neighbours | 37 | 0.987 | 1.541 | 1.853 | 2.013 | 2.740 | 1.817 |
| Y+3 | control | A all living | 152 | 0.983 | 1.566 | 1.743 | 1.962 | 2.678 | 1.770 |
| Y+3 | treated | A all living | 149 | 1.031 | 1.567 | 1.738 | 1.932 | 2.666 | 1.757 |
| Y+3 | control | Fell neighbours | 33 | 0.983 | 1.615 | 1.875 | 2.043 | 2.774 | 1.862 |
| Y+3 | treated | Fell neighbours | 34 | 1.031 | 1.640 | 1.898 | 2.098 | 2.784 | 1.889 |

At Y0 the Fell-neighbour rows are identical by construction: Execute changes no surviving tree. The treated "A all living" rows differ only because the 10 felled crowns left the set.

### Annual crown expansion of surviving trees (m/yr, r(year) − r(year − 1), trees living at both ends of the year)

| Year | Run | Group | n | min | p25 | median | p75 | max | mean |
|---|---|---|---|---|---|---|---|---|---|
| 1 | control | A survivors | 166 | +0.0018 | +0.0026 | +0.0030 | +0.0038 | +0.0059 | +0.0032 |
| 1 | treated | A survivors | 160 | +0.0019 | +0.0027 | +0.0033 | +0.0045 | +0.0447 | +0.0060 |
| 1 | control | Fell-neighbour survivors | 36 | +0.0018 | +0.0026 | +0.0032 | +0.0040 | +0.0086 | +0.0036 |
| 1 | treated | Fell-neighbour survivors | 37 | +0.0037 | +0.0083 | +0.0134 | +0.0203 | +0.0447 | +0.0163 |
| 2 | control | A survivors | 156 | +0.0038 | +0.0068 | +0.0086 | +0.0104 | +0.0159 | +0.0086 |
| 2 | treated | A survivors | 154 | +0.0037 | +0.0051 | +0.0067 | +0.0096 | +0.0462 | +0.0090 |
| 2 | control | Fell-neighbour survivors | 34 | +0.0065 | +0.0086 | +0.0097 | +0.0110 | +0.0160 | +0.0102 |
| 2 | treated | Fell-neighbour survivors | 35 | +0.0074 | +0.0109 | +0.0176 | +0.0241 | +0.0462 | +0.0193 |
| 3 | control | A survivors | 152 | +0.0065 | +0.0122 | +0.0148 | +0.0174 | +0.0306 | +0.0153 |
| 3 | treated | A survivors | 149 | +0.0052 | +0.0092 | +0.0120 | +0.0169 | +0.0473 | +0.0141 |
| 3 | control | Fell-neighbour survivors | 33 | +0.0091 | +0.0136 | +0.0160 | +0.0193 | +0.0268 | +0.0164 |
| 3 | treated | Fell-neighbour survivors | 34 | +0.0153 | +0.0182 | +0.0232 | +0.0285 | +0.0473 | +0.0246 |

How to read the matched control:
- Fell neighbours expand about 4.5× faster than in the control in year 1 (mean +0.0163 vs +0.0036 m) and about 1.5× in year 3. The largest single-tree step is 0.047 m/yr: gradual crown closure, no jump. Over three years the gap stays open: treated A cover is still −3.84 pp below control at Y+3.
- The treated "A survivors" median in years 2–3 is slightly below the control. That is because control mortality removes more suppressed stems (control A lost 19 stems by Y+3; treated lost 12 beyond the Fell), so the populations differ. It is a real density consequence, not noise. For the same reason, from Y+1 the "cells changed" counts include divergent natural mortality as well as the Fell's own effect.
- **Cross-border effect:** the Fell changed light in 3 Area B cells at once, and B's union cover fell by 0.113 pp, because felled A crowns had overlapped into B. From Y+1, B crowns also respond through the woodland CI. B's DBH and basal area are unchanged because growth stays per area.

### Supplementary (NOT the gate): 12-tree Fell (`light MC-02`)

Removed IDs: the 10 above plus MA-A-057 and MA-A-073. Fell neighbours: 39 (A 36, B 3).

| Stage | Treated A stems / BA | Δ A cover | Δ A uncovered | Δ A mean light | A largest opening (treated) | Fell-neighbour mean expansion, treated vs control |
|---|---|---|---|---|---|---|
| Y0 after Fell | 159 / 67.81 | −6.04 pp | +55.24 m² | +0.0427 | 61.66 m² / 6.25 m | — |
| Y+1 | 158 / 69.95 | −5.40 pp | +49.37 m² | +0.0410 | 60.59 m² / 6.25 m | +0.0180 vs +0.0035 (year 1) |
| Y+3 | 148 / 71.08 | −4.28 pp | +39.16 m² | +0.0365 | 58.53 m² / 6.07 m | +0.0262 vs +0.0168 (year 3) |

The full 12-tree radius and expansion rows are printed by the test (`MC12|…`).

**Runtime**, measured with Stopwatch: mean of 5 full fresh evaluations after a JIT warm-up, WSL2 on the owner's machine, Debug build. Clipping boundary micro-cells added a little cost.

| Tree counts | Grid | Time per evaluation |
|---|---|---|
| 149 + 73 living (10-tree treated, Y+3) | 2.5 m | 39.1 ms |
| 149 + 73 living (10-tree treated, Y+3) | 5.0 m | 29.3 ms |
| 171 + 73 (genesis) | 2.5 m | 33.8 ms (single run) |

Most of the time goes to the 0.25 m cover grid. The timings vary between runs on this machine (the previous pass measured 22–27 ms).

## Known limits, interpretations and UNKNOWNs

1. **Interpretation, needs reviewer confirmation:** the model 1 woodland CI drives **only the crown response**. DBH growth, height and mortality (suppression and the boundary rule) keep the existing per-area CI. My basis is spec #899 §"Approved model": *"Annual DBH growth, height growth, mortality, work-plan semantics and all other rules are unchanged. Only crown initialisation and crown response change."* The boundary-policy bullet ("Hegyi competition within 8 m … cross the shared rack border") could be read as also covering growth and mortality. If the owner meant that, it is a separate, non-crown change and would need its own decision. The model 0 label `MultiEdgeExcluded` ("each area's competition uses only its own trees") stays true for the default UI session.
2. Model 1's annual `r_pot` uses the **post-growth DBH**, mirroring model 0. Year 0 uses the genesis DBH.
3. The 5.0 m "exact Unity parity" is against a transcription of the Unity equation **evaluated in .NET float32**. It was **not** run inside Unity or Mono, so Unity runtime floating-point behaviour is unverified.
4. The light index is evaluated with Unity's `1 − clamp01(1 − gap)`. It can differ from the bare product Π(1 − influence) only by float rounding (about 1 ulp).
5. Cover is sampled at 0.25 m cell centres. A micro-cell is included when its centre is strictly inside the polygon, and it is weighted by its exact polygon-clipped area, as in the light layer. Covered + uncovered = polygon area, and the fraction is over the polygon area. A boundary micro-cell whose centre is outside is not sampled. Its in-rack sliver counts as uncovered: about 2.5 m² in A and 5.7 m² in B (polygon − sampled). This small, conservative bias is kept visible through `SampledAreaM2`. Crown discs are still point-sampled at micro-cell centres, so a single r = 3 m disc reads about 1.2 % low (`O-08`). A centre exactly on a polygon edge counts in no area.
6. "Opening width" means the inscribed diameter on the micro-grid, bounded by crowns **and the polygon edge**. "Mean overlap count" is area-weighted over the covered area.
7. The canopy read-out is computed lazily: at most once per committed tree state, on first read after load, Execute or an annual step. Pending and Approve never trigger it.
8. **Not done in this pass:**
   - The targeted `/multi` run (#899 plan step 6, S8): out of scope for this writer, since UI and served pages are forbidden.
   - `tests/p0-isolation.sh`, `build-output-safety.sh`, `cli-safety.sh`, `ui-smoke*.sh`: not run. p0-isolation does a cold rebuild that could rebuild `.p0ref` under 8.0.134 and break the pin. The UI smoke tests start servers.
   - Unity, Blender and the Editor: not used.
9. **UNKNOWN:** calibrated PAR, LAI and transmittance (deliberately not modelled); field accuracy beyond the spec's Frenchpark caveat; the origin of the 13× figure.
10. No commit was pushed. The orchestrator pushes.

## Reproduce

```bash
cd /home/jer/Development/CCF-HP-Worktrees/CCF-Multi-899-900-Engine-20261123/Tools/MultiRackSitkaP1Lab
export TMPDIR=/tmp/<outside-repo>
/home/jer/.dotnet/dotnet build tests/Core.Tests/SitkaP1.Core.Tests.csproj
/home/jer/.dotnet/dotnet tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests.dll | grep -E "model0|crown|light|MC\||oracle|Unity|passed"
```

The expected result is `160/160 passed`, with the fingerprints and tables above printed to stdout. Runtime figures will vary by machine.
