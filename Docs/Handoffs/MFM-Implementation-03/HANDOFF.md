# MFM-03 — Coverage Calibration Measurement: Handoff

**Task:** `MFM-03 — Coverage calibration measurement` (claim #39, umbrella PLAYER-TEST-OVERNIGHT-01 #91)
**Packet:** `Docs/Handoffs/MFM-Implementation-03/TASK-PACKET.md`
**Feature / authority:** Multi-Scale Forestry Management V1, D-065; `specs/multiscale-forestry-management-v1/`
**Phase:** static/offline measurement. **Unity was not used; the Unity lock was not acquired.**
**Exit:** `COVERAGE CALIBRATION QUALIFIED — READY FOR MANAGER REVIEW` (recommendation only; the Manager accepts or rejects it)

## 1. Identity

| Item | Value |
|---|---|
| Inspected and tested HEAD (starting `origin/main`) | `72dbee8b8eab66f58046abfd49e77ead4c29d357` |
| Measurement-tooling commit | `139b1f9edebfcda4c0bbed2b195f0e6b0ed01f8c` |
| Branch / worktree | `research/mfm-coverage-calibration-v1` / `/media/jer/Videos/CCF-MFM03` |
| Executor | Claude Code (T3 Code harness), **Claude Opus 5.5** (`claude-opus-5-5`), provider Anthropic. Research sub-agent dispatched by the #91 umbrella integrator. No child agents. |
| MFM-02 baseline | PR #38 / merge `07eee3d`; Unity-tested HEAD `8f9be4f` |
| Evidence directory | `/media/jer/Files/CCF/Verification/MFM03-CoverageCalibration-20261029/` (`run.log`, `export.json`, `results.json`, `tables.md`, `negative-controls.txt`, `MANIFEST.sha256`) |
| Tools used | .NET SDK 10.0.112 (target `net8.0`), Python 3.12.3 standard library. No package or dependency was added. |

The production sources the measurement depends on (`ForestStartingStand.cs`, `ScenarioOneHeadlandGeometry.cs`, `ScenarioOneRackSampleGeometry.cs`, `ScenarioOneRackBay.cs`, `CompetitionShare.cs`, `RackGeometry.cs`, `ForestEcologyController.cs`, `OfflineStubs.cs`) have **no commits between `8f9be4f` and `72dbee8`**.

## 2. Files changed

All are new and measurement-only, under `Tools/Verification/MfmCoverageCalibration/`, plus this handoff:

| File | Purpose |
|---|---|
| `runner/CoverageExport.cs` | Exporter. It is compiled with the **unmodified** `ScenarioOneRackSampleGeometry`, `ScenarioOneRackBay`, `CompetitionShare` and `RackGeometry` sources and the existing offline `ForestEcologyController` stand-in. It re-checks the stand-in against the real Hegyi source text and exports per-tree facts. |
| `run_coverage_calibration.py` | Builds the exporter in a temporary directory outside the repo, then runs the analysis and the tables writer. |
| `analyse_coverage.py` | Masks, per-unit metrics, sensitivity sweeps, former-edge, stand-edge and remainder effects, regeneration diagnostic and candidate-spacing workload. Standard library only. |
| `write_tables.py` | Writes the Markdown tables reproduced below. |
| `inputs/fixture-stand.csv`, `inputs/fixture-infrastructure.json`, `inputs/historical-lesson5-regeneration.json`, `inputs/PROVENANCE.md` | Frozen inputs with their source hashes. |
| `README.md` | How to run the tooling and what it cannot show. |

No production C#, scene, prefab, ScriptableObject, ProjectSettings, package, save or coordination file was changed. Nothing was placed under `Assets/`.

Reproduce: `python3 -I -B Tools/Verification/MfmCoverageCalibration/run_coverage_calibration.py`. The run prints `MFM03_EXPORT_PASS`, `MFM03_ANALYSIS_PASS checks=16 failed=0` and `MFM03_TABLES_WRITTEN`.

## 3. Fixture and method

**Fixture.** The fresh Enlarged80 Rack V2 starting stand (1,344 Sitka stems, age 20, year 0) with the five MFM-02 qualified racks: east–west, width 2.4 m, x −35.4 → 40, z −35.2711754 / −16.4217644 / 0.2382353 / 15.7935295 / 35.2323532. The stand is the accepted RACK-V2-02A target-preserving reconstruction (see `inputs/PROVENANCE.md`). Every run checks it against the live MFM-02 Unity record and fails on any mismatch:

- teaching footprint: **75 stems, 1.4417531 m², SHA `c2f2fd5418794915`**;
- **10** corridor-excluded band stems, **11** stems between 8.00385 m and 9 m, and **8** stems in the 9–10 m band;
- `ScenarioOneRackSampleGeometry.Derive` reproduces the live unit offsets and areas (1132.28247 / 1420.37646 / 1288.61182 / 1399.76477 / 1158.96472 m²) and both explicit zero-area remainders, digit for digit.

Two negative controls (`negative-controls.txt`) show the identity check fails as it should. Moving one footprint stem 20 m changes the count, SHA and BA. Adding 0.5 cm to one stem's DBH changes the BA.

**State compared.** Post-access, pre-treatment, same year. The rack removal sets come from production `RackGeometry.Select` (149 stems), leaving 1,195 living stems. The source is the teaching footprint *before* the sample treatment. Comparing against a thinned sample would mix up treatment with representativeness. No growth, mortality, regeneration or player state is simulated (see §11).

**Competition.** Each stem's CI is `CompetitionShare.Measure(...).CurrentTotal` over all other living stems. That is the production Hegyi term and the 8 m cutoff, which is the same sum as the ecology's `UpdateCompetition`. The player can see this value: the inspection panel shows "competition index".

**Masks.** These areas are excluded from both stem sets and area denominators, identically for the source and for every target:

- rack corridors (production placement);
- headland and feeder (production `ScenarioOneHeadlandGeometry` constants with the 0.15 m stem guard);
- forest-track planks and the south approach (generator 1.5 m corridor);
- the work clearing at its visible 4.5 m art radius (three real stems, P1708/P1709/P1710, stand in the art);
- the player start and structures (2.2 m).

Area uses 0.1 m column integration with exact interval unions. The source area converges at 309.885 / 309.810 / 309.817 m² for steps of 0.2 / 0.1 / 0.05 m.

**Partitions tested.** All three are sub-partitions *inside* the MFM-02 units. MFM-02 geometry semantics are unchanged.

- **whole-strip:** the five MFM-02 units, x −40 → 40.
- **anchor-aligned thirds:** each unit split at the sample anchor ±12 m into W [−40, −12), C [−12, 12) and E [12, 40]. R3C is exactly the teaching footprint plus its corridor.
- **equal thirds:** each unit split at ±13.33 m.

## 4. Source (teaching footprint, post-access, pre-treatment)

| stems | net area m² | density /ha | mean DBH cm | QMD cm | BA m²/ha | mean CI | sd CI | P90 CI | above own P90 |
|---|---|---|---|---|---|---|---|---|---|
| 75 | 309.8 | 2421 | 15.40 | 15.64 | 46.5 | 8.048 | 1.692 | 10.124 | 9.3% |

The footprint spills over its own strip. Strip 3 spans −8.33 / +7.78 m across the rack, but the fixed 9 m teaching half-width is wider. So **70** sample stems lie in unit R3, **2** in R2 and **3** in R4. These sample stems are excluded from every target set and the footprint area is subtracted from neighbouring units.

## 5. Complete per-unit metric table

The primary partition is **anchor-aligned thirds**. Δ values are relative to the source. "SE ΔCI" is the two-sample standard error of the relative mean-CI difference. "Margin" is the largest normalised use of any recommended tolerance: ≤ 1 fits, and a value near 1 is a boundary unit. Columns:

- **> src P90**: share of the unit's stems above the source's 90th-percentile CI.
- **former edge**: stems on the preserved Legacy40 clamp line.
- **≤ 8 m bnd**: stems within the 8 m Hegyi reach of the stand boundary.
- **core**: retained Legacy40 core stems.
- **rack rem.**: stems removed by the rack access.
- **masked**: stems excluded by infrastructure masks.
- **sample**: footprint stems that fall in this unit.
- **corr/infra/sample m²**: area removed for corridors, infrastructure masks and the footprint.

| unit | stems | net m² | dens/ha | Δdens | DBH | ΔDBH | CI | ΔCI | SE ΔCI | > src P90 | former edge | ≤ 8 m bnd | core | rack rem. | masked | sample | corr/infra/sample m² | recommended | margin |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| R1W | 61 | 240 | 2541 | +5.0% | 15.36 | −0.3% | 5.38 | **−33.2%** | ±3.1 | 0.0% | 0 | 45 | 0 | 9 | 0 | 0 | 56/100/0 | **not covered** | 1.66 |
| R1C | 53 | 221 | 2394 | −1.1% | 15.08 | −2.1% | 6.71 | −16.6% | ±4.3 | 3.8% | 0 | 26 | 0 | 9 | 0 | 0 | 58/61/0 | covered | 0.83 |
| R1E | 72 | 329 | 2188 | −9.6% | 15.54 | +0.9% | 7.01 | −12.9% | ±4.2 | 6.9% | 0 | 47 | 0 | 12 | 0 | 0 | 67/0/0 | covered | 0.64 |
| R2W | 92 | 393 | 2341 | −3.3% | 15.32 | −0.5% | 7.34 | −8.7% | ±3.3 | 4.3% | 7 | 23 | 14 | 5 | 0 | 0 | 56/48/0 | covered | 0.44 |
| R2C | 65 | 306 | 2123 | −12.3% | 15.74 | +2.2% | 6.83 | −15.1% | ±3.2 | 1.5% | 5 | 0 | 36 | 1 | 0 | 2 | 58/48/14 | covered | 0.75 |
| R2E | 98 | 430 | 2279 | −5.8% | 15.34 | −0.4% | 8.42 | +4.7% | ±3.5 | 21.4% | 3 | 29 | 9 | 12 | 0 | 0 | 67/0/0 | covered | 0.29 |
| R3W | 86 | 352 | 2445 | +1.0% | 15.44 | +0.3% | 7.87 | −2.2% | ±3.7 | 18.6% | 5 | 22 | 27 | 12 | 0 | 0 | 56/43/0 | covered | 0.11 |
| R3C | 0 | 0 | — | — | — | — | — | — | — | — | 0 | 0 | 0 | 10 | 0 | 70 | 58/52/277 | **SOURCE (manual)** | — |
| R3E | 96 | 384 | 2501 | +3.3% | 15.30 | −0.7% | 8.58 | +6.7% | ±3.4 | 18.8% | 7 | 27 | 33 | 12 | 0 | 0 | 67/0/0 | covered | 0.33 |
| R4W | 100 | 387 | 2587 | +6.8% | 15.52 | +0.7% | 8.33 | +3.4% | ±3.7 | 19.0% | 9 | 25 | 26 | 9 | 0 | 0 | 56/47/0 | covered | 0.34 |
| R4C | 74 | 276 | 2685 | +10.9% | 15.76 | +2.3% | 9.15 | +13.7% | ±3.8 | 27.0% | 12 | 0 | 44 | 13 | 3 (clearing) | 3 | 58/68/18 | covered | 0.68 |
| R4E | 96 | 423 | 2271 | −6.2% | 15.38 | −0.2% | 8.46 | +5.2% | ±3.5 | 26.0% | 9 | 28 | 26 | 14 | 0 | 0 | 67/0/0 | covered | 0.31 |
| R5W | 77 | 314 | 2449 | +1.1% | 15.51 | +0.7% | 7.51 | −6.6% | ±4.7 | 22.1% | 0 | 43 | 0 | 8 | 0 | 0 | 56/35/0 | covered | 0.33 |
| R5C | 68 | 290 | 2344 | −3.2% | 15.91 | +3.3% | 7.38 | −8.3% | ±4.0 | 14.7% | 0 | 33 | 0 | 10 | 0 | 0 | 58/0/0 | covered | 0.41 |
| R5E | 79 | 338 | 2334 | −3.6% | 15.71 | +2.0% | 7.08 | −12.0% | ±4.1 | 8.9% | 0 | 47 | 0 | 13 | 0 | 0 | 67/0/0 | covered | 0.60 |

Native MFM-02 **whole strips**:

| unit | stems | net m² | dens/ha | Δdens | DBH | ΔDBH | CI | ΔCI | SE ΔCI | > src P90 | former edge | ≤ 8 m bnd | core | rack rem. | masked | sample | recommended | margin |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| R1 | 186 | 791 | 2353 | −2.8% | 15.35 | −0.4% | 6.39 | **−20.6%** | ±3.1 | 3.8% | 0 | 118 | 0 | 30 | 0 | 0 | not covered | **1.03** |
| R2 | 255 | 1129 | 2258 | −6.7% | 15.44 | +0.2% | 7.63 | −5.2% | ±2.8 | 10.2% | 15 | 52 | 59 | 18 | 0 | 2 | covered | 0.34 |
| R3 | 182 | 736 | 2474 | +2.2% | 15.37 | −0.2% | 8.25 | +2.5% | ±3.1 | 18.7% | 12 | 49 | 60 | 34 | 0 | 70 | covered | 0.12 |
| R4 | 270 | 1085 | 2489 | +2.8% | 15.54 | +0.9% | 8.60 | +6.9% | ±2.9 | 23.7% | 30 | 53 | 96 | 36 | 3 | 3 | covered | 0.34 |
| R5 | 224 | 943 | 2375 | −1.9% | 15.70 | +1.9% | 7.32 | −9.0% | ±3.2 | 15.2% | 0 | 123 | 0 | 31 | 0 | 0 | covered | 0.45 |

**Equal thirds** differ from anchor-aligned thirds in two ways. R2C becomes a boundary failure (density −20.2%, margin 1.01). R3C becomes a 7-stem sliver outside the sample (36 m²). The full table is in `tables.md`.

Other per-unit facts:

- **Species:** every stem is Sitka. The starting stand uses the spawner default species, and the historical Enlarged80 save has 1,344/1,344 `sitka-spruce` adults and Sitka-only cohorts. There are no Oak or Beech adults in the fixture. The Sitka-only eligibility rule therefore excludes nothing here but must stay in force.
- **Regeneration and juveniles:** the fixture carries no cell or juvenile state (§9).
- **Marks, Crop Trees and open work:** none outside the sample in the fixture.

## 6. What actually varies in this stand

The table shows a homogeneous, even-aged plantation.

- **DBH is not informative at year 0.** Units differ by −2.1 to +3.3%, and all 125 sample-sized windows fall within −2.1 to +4.2%.
- **Density varies little.** Units differ by −12.3 to +10.9%, and windows by −20.0 to +8.6%. Net-area masking matters (§7).
- **Mean CI is the metric that discriminates.** It is driven by three observable structures:
  1. **Stand-boundary truncation.** The model has no trees outside the property, so a stem's competition falls near the edge. Mean CI is −44% against the source at 0–2 m from the boundary, −29% at 2–4 m, about −3 to −5% at 4–8 m, and +4 to +7% beyond 8 m. R1W (45 of 61 stems within 8 m of the boundary, SW corner) and the R1 strip as a whole are the low-CI units. This is MODEL DERIVATION. Edge stems genuinely face less modelled competition, so a 30%-Share release transfers differently there.
  2. **Track and clearing openings.** The forest track crosses the teaching footprint: 36 plank centres lie inside it, and the work clearing touches its north edge. The 20 source stems within 4.5 m of the track or 7 m of the clearing average CI 6.46, against 8.63 for the other 55 (−25%). Together with the player-start and structure masks, they remove 64.6 m² (17.3%) of the corridor-free footprint. As a result the sample's mean CI sits about 7% below a track-free interior. Interior track-free units read +3 to +7% CI for this reason, not because they are materially different.
  3. **Rack corridors.** After access, stems 0–2 m from a corridor edge average CI 7.9–8.0, against 8.8 at 4–8 m. Every strip and the sample are corridor-centred in the same way, so this cancels out in comparisons. It does **not** cancel if one side is compared before access and the other after (§7, pre-access variant).

## 7. Sensitivity analysis

**Rule family tested** (all **GAMEPLAY CALIBRATION**). A region is covered iff all of these hold:

- stems ≥ n_min;
- |Δdensity| ≤ τd;
- |ΔDBH| ≤ τb;
- |ΔCI| ≤ τc;
- optionally, crowded-tail fraction ≤ source fraction + g.

Each run tested τd ∈ {10, 15, 20, 25, 30%}, τb ∈ {5, 10, 15, 20%}, τc ∈ {10, 15, 20, 25, 30%} and g ∈ {none, +5, +10, +15, +20 pp}, a full 500-point grid per partition.

**One-at-a-time sweeps, anchor-aligned thirds, around the recommended point (20/15/20, no tail guard, n ≥ 30):**

| varied | values → covered count (of 14 non-source regions) |
|---|---|
| density τd | 10% → 11 (R2C and R4C drop) · 15% → 13 · 20% → 13 · 25% → 13 · 30% → 13 |
| DBH τb | 5% → 13 · 10% → 13 · 15% → 13 · 20% → 13 |
| CI τc | 10% → 8 · 15% → 11 (R1C −16.6% and R2C −15.1% drop) · **20% → 13** · 25% → 13 · 30% → 13 |
| tail guard g | none → 13 · +5 pp → 5 · +10 pp → 9 · +15 pp → 11 · +20 pp → 13 |
| n_min | 10 / 20 / 30 / 40 → 13 · 60 → 12 (R1C) |

**Stable plateau.** Classification is identical for every τc in **(16.6%, 33.2%)**, every τd ≥ 12.3% and every τb ≥ 3.3%. The recommended point sits inside all three plateaus. The nearest covered unit is 3.4 pp from the CI threshold (R1C) and the nearest failed unit is 13.2 pp beyond it (R1W).

**Whole strips are fragile.** R1 fails at −20.6% against a 20% CI tolerance (margin 1.03). Any τc ≥ 21% covers the whole southern strip, and any τc ≤ 20% excludes all of it, including its interior parts. Equal thirds put R2C on the density boundary (margin 1.01).

**The crowded-tail guard is not robust and is not recommended for V1.** The prior unaccepted MFM-03 and MFM-03B work proposed "fraction above source P90 ≤ source + 10 pp". On this fixture:

- the covered count moves 5 → 9 → 11 → 13 as g goes +5 → +10 → +15 → +20 pp;
- at +10 pp, three units sit within 0.7 pp of the limit (R3W 18.6%, R3E 18.8%, R4W 19.0% against 19.33%), while a proportion of this size has a standard error of about 4 pp;
- 42% of all sample-sized windows exceed source + 10 pp. The source's own tail (9.3%) is unusually light because the track crosses it;
- it does **not** pick out the former-edge band: band stems exceed the source P90 *less* often (15.8%) than core-interior stems (19.5%).

**Noise floor (MODEL DERIVATION).** The two-sample standard error of ΔCI is 3.1–4.7 pp for these regions, so 20% is about 4–6 SE. Among the clean interior windows (no boundary or former-edge stems), 100% fall within ±10% CI. Among all 125 windows, 53% fall within ±10% CI, 90% within ±15% and 96% within ±20%. Tolerances of 10–15% would therefore classify position noise and edge proximity, not different conditions.

**Measurement-choice robustness, anchor-aligned thirds:**

| variant | covered (recommended rule) | covered (+10 pp tail guard) |
|---|---|---|
| baseline (visible clearing 4.5 m, post-access) | 13 | 9 |
| generator clearing radius 2.2 m | **13 (identical)** | 9 |
| both sides measured pre-access | 12 (R2C −23.6% CI drops) | 11 |
| no infrastructure masks (gross density) | 12 (R3E +24.9% density drops) | 8 |

The recommended tolerances do not depend on the clearing-radius choice. They **do** require two measurement definitions, which are part of the metric, not tolerances:

- compare the source and targets in the same post-access state;
- divide stem counts by masked net area.

**Growth drift.** Live MFM-02 recorded footprint BA ×1.05226 over one growth step, which implies a uniform DBH change of +2.6% per year, +5.2% after 2 years and +7.9% after 3. A ±10% DBH tolerance would leave only about 2 pp of headroom if targets are measured 2–3 years after the source snapshot (Lesson 2 snapshot → extension after the Lesson 5 review). ±15% keeps about 7 pp. CI is a ratio of DBHs, so it is approximately invariant to uniform growth.

## 8. Covered / not-covered examples

| class | unit | evidence |
|---|---|---|
| **Clear fit** | R3W (margin 0.11), R2E (0.29), R2W (0.44) | All three Δ values within ±9%. R2W includes 7 former-edge stems and is still within ±9%. |
| **Clear fail** | R1W (1.66) | Mean CI −33.2% (5.38 against 8.05), about 11 SE. 45 of 61 stems are within 8 m of the stand boundary. Player-facing reason: *"trees here face much less competition than in your sample — this corner is on the stand edge."* |
| **Boundary (covered)** | R1C (0.83) | CI −16.6% ± 4.3 pp; flips to not covered at τc = 15%. |
| **Boundary (covered)** | R2C (0.75) | CI −15.1%, density −12.3%. The approach track passes through it (48 m² masked) and it holds 5 former-edge stems; the feeder lies in R1C, not here. Flips at τc = 15% and at τd = 10%. |
| **Boundary (covered)** | R4C (0.68) | CI +13.7%, density +10.9%, 12 former-edge stems and the clearing edge. Flips at τd = 10%. |
| **Boundary (whole-strip)** | R1 (1.03) | Fails by 0.6 pp. This is why whole strips are not recommended. |
| **Source** | R3C | The footprint itself: manual, never a target. |

## 9. Former-edge band, outer lattice, remainders, regeneration and access

**Former-edge band (explicitly identified, not averaged away).** It consists of 61 retained Legacy40 clamp-line stems at |x| or |z| = 19.4 m; 4 are removed by racks and **57** remain.

- **Where it lies:** strips R2/R3/R4, specifically R2W 7, R2C 5, R2E 3, R3W 5, R3E 7, R4W 9, R4C 12 and R4E 9. **The sample contains none.**
- **Spacing:** the band is a closer double row. Mean nearest-neighbour distance is **1.45 m** against 1.66 m in the core interior (−12.6%).
- **Competition:** after access the band's mean CI is **8.47**, against 8.31 in the core interior (+1.9%) and 8.68 in the nearby outer plantation. The earlier Enlarged80 "+16% density / +15% Hegyi" figure is not reproduced at unit scale in the post-access V2 state.
- **Effect on unit means:** at most 2.2 pp of ΔCI (R4C +13.7% with the band against +11.5% without; R4 strip +6.9% against +5.9%). It decides no classification under any tolerance in the recommended plateau.
- **Recommendation:** **flag, do not exclude.** The coverage explanation may state "includes the slightly closer double row along the old stand edge". No band-specific exclusion is needed for V1 on this fixture. Do not identify band stems by ID or provenance in production logic; the classifier uses current positions and competition only.
- **Candidates on the band:** two of the seventeen recommended candidates lie on it (P0006, P2010). Spacing makes no judgement about them; the player confirms or rejects.

**Outer lattice and stand edge.** The outermost units (R1 and R5) and every W/E end segment contain the 0–8 m boundary zone where Hegyi competition is truncated (§6). This is the main reason a unit fails in this stand. The outer plantation (`PO*` stems) does not otherwise differ materially from the core: CI 8.68 against 8.31, DBH 15.46 against 15.42. Two recommended candidates (PO1700 at x −39.2 and PO3501 at x −37.5) lie west of the headland strip and are reached from the headland.

**Outer remainders.** In the fixture both remainders are explicit and empty: area 0 and 0 stems, with strip edges at −44.696 and +44.952 m beyond the ±40 m stand. Hypothetical non-qualified probes, run through production `Derive`, show that non-empty remainders occur with ordinary-looking plans:

| probe | remainder | stems |
|---|---|---|
| racks shifted 4 m north | 303 m² | 63 |
| racks shifted 4 m south | 252 m² | 64 |
| compressed 16 m centred | 160 m² | 30 |
| uniform 16 m | 0 m² | 0 |

**Recommendation:** a non-empty remainder defaults to **Manual** and is never compared or treated as a coverage unit. That is the spec's "not silently assigned". Apply the n ≥ 30 minimum only if a later decision lets remainders be classified.

**Regeneration and juvenile conflicts.** The V2 fixture is year 0 and carries no cell state, so the live post-Lesson-5 regeneration state **cannot be measured offline**; that is the Unity-only residual in §11. Supporting facts:

- **Code at `72dbee8`.** `ScenarioOneManager.ResolveFelling` → `ForestTree.Fell` → `ForestEcologyController.OnTreeFelled` (`:418`) only adds `RecentOpening` and queues a canopy rebuild. Juvenile and cohort removal happens only through `ScenarioOneClearance.QueryClearance` / `ApplyClearance` (`:8` / `:60`). Felling-only release work therefore does not destroy natural regeneration in the model.
- **Historical diagnostic** (pre-V2 Enlarged80 Lesson-5 save, year 2, old rack geometry; not the V2 state). There are 65 cells with Sitka natural cohorts, no planted juveniles, clearance patches or shelters, and one deadwood record. Cohorts touch every one of the 15 regions. Area-weighted relative occupancy runs from 0.003 (R3E) to 2.17 (R2C). This is summed cohort occupancy, not stems/ha.
- **Guard semantics recommended (ACCEPTED GAME DESIGN + MODEL DERIVATION; no numeric tolerance qualified).**
  - Hard-*exclude* treatment footprints and units containing planted juveniles, shelters, clearance patches, protected areas, retained-deadwood records, active planting/clearance/protection work, player Fell/Crop marks or open orders that conflict, and non-Sitka adults.
  - *Record and flag* natural regeneration (presence, height and relative occupancy) but do not exclude on it. A blanket "any natural regeneration → excluded" would remove every unit in the historical diagnostic.
  - No regeneration *similarity* tolerance is recommended for V1.

**Access and clearing exclusions that affect comparability:**

- **The track crosses the sample** (§6). The comparison stays valid only because the masks are applied identically and density uses net area. Without masks the classification changes (§7).
- The three clearing-art stems (P1708–P1710, in R4C and outside the sample) are excluded from candidates and removals.
- No stem sits in the headland, feeder, track or site masks; the generator already cleared them.
- Rack removals are excluded from release credit, consistent with the teaching account.

## 10. Candidate-spacing options and confirmation workload

These measurements keep MFM V1 semantics:

- **Eligible stems:** living Sitka adults in covered regions, outside masks, outside the sample, and not Crop Trees.
- **Spacing:** measured against the three sample Crop Trees (MFM-02 live chain: P0810, P1206, P1214) and earlier suggestions. Candidate spacing is its own parameter, separate from the 8 m teaching spacing and the 8 m Hegyi radius. 8 m is shown only as a comparison.
- **Order:** position only. *Unit-centre-first* takes stems in each region by distance to the region's mean stem position, regions in rack order then W→C→E. *West-east-scan* sorts by x, then z, then ID.
- **No quality, DBH, CI or dominance input is used.** Spacing identifies *where to inspect*; it designates nothing.
- **Rejections** are simulated with a deterministic hash of the tree ID (25% and 50%). This measures workload only and is not a model of quality.
  - **Batch:** the suggestion set is fixed, and a rejection leaves that position empty; manual designation is still available.
  - **Queue:** a rejected stem stops blocking spacing, so a nearby stem may be *shown* next. It still needs its own confirmation and is never auto-confirmed.
- **Time estimates (ASSUMPTION):** decisions × 15/30/60 s plus a straight nearest-neighbour tour at 4 m/s, starting from the sample anchor.

Scope: the 13 regions covered by the recommended rule, with 1,059 eligible stems on 0.444 ha net.

| spacing | order | candidates (decisions at 0% reject) | /net ha | regions without candidate | tour m | minutes @15/30/60 s | 25% reject: batch / queue (shown/conf/rej) | 50% reject: batch / queue | greedy indicator: reach 30% @ sample ceiling 17.9% (heaviest region BA) | @ 40% |
|---|---|---|---|---|---|---|---|---|---|---|
| 8 m (comparison only) | centre-first | 70 | 158 | 0 | 717 | 20.5 / 38.0 / 73.0 | 70/51/19 · 84/66/18 | 70/34/36 · 118/63/55 | 23/70 (17.9%) | 70/70 (36.7%) |
| 10 m | centre-first | 41 | 92 | 0 | 561 | 12.6 / 22.8 / 43.3 | 41/33/8 · 52/44/8 | 41/20/21 · 78/41/37 | 24/41 (17.9%) | 41/41 (30.4%) |
| 12 m | centre-first | 33 | 74 | 0 | 551 | 10.5 / 18.8 / 35.3 | 33/21/12 · 44/30/14 | 33/17/16 · 49/29/20 | 29/33 (17.8%) | 33/33 (23.3%) |
| 14 m | centre-first | 25 | 56 | 0 | 433 | 8.1 / 14.3 / 26.8 | 25/21/4 · 30/25/5 | 25/14/11 · 47/22/25 | 25/25 (17.4%) | 25/25 (18.2%) |
| **16 m** | **centre-first** | **17** | **38** | **0** | **295** | **5.5 / 9.7 / 18.2** | **17/15/2 · 19/17/2** | **17/10/7 · 30/14/16** | **17/17 (14.9%)** | 17/17 (14.9%) |
| 18 m | centre-first | 13 | 29 | 1 (R2C) | 316 | 4.6 / 7.8 / 14.3 | 13/9/4 · 18/13/5 | 13/6/7 · 25/13/12 | 13/13 (9.7%) | 13/13 (9.7%) |
| 20 m | centre-first | 11 | 25 | 2 (R2C, R5C) | 254 | 3.8 / 6.6 / 12.1 | 11/9/2 · 14/10/4 | 11/4/7 · 20/12/8 | 11/11 (9.7%) | 11/11 (9.7%) |
| 12 m | west-east | 39 | 88 | 0 | 692 | 12.6 / 22.4 / 41.9 | 39/28/11 · 56/36/20 | 39/17/22 · 80/34/46 | 28/39 (17.9%) | 39/39 (26.3%) |
| 14 m | west-east | 30 | 68 | 0 | 514 | 9.6 / 17.1 / 32.1 | 30/25/5 · 38/28/10 | 30/14/16 · 50/29/21 | 29/30 (17.7%) | 30/30 (20.6%) |
| 16 m | west-east | 23 | 52 | 0 | 446 | 7.6 / 13.4 / 24.9 | 23/19/4 · 27/21/6 | 23/14/9 · 38/23/15 | 23/23 (17.8%) | 23/23 (18.0%) |
| 18 m | west-east | 18 | 41 | 1 | 386 | 6.1 / 10.6 / 19.6 | 18/15/3 · 23/18/5 | 18/9/9 · 32/17/15 | 18/18 (12.9%) | 18/18 (12.9%) |

All 14 non-source regions as an upper bound (including R1W): 16 m centre-first gives 20 candidates, 12 m gives 35 and 8 m gives 75. The remaining rows are in `tables.md`.

**The greedy release indicator** is not a prescription and not a proof of feasibility. Around each confirmed candidate it removes the strongest-Share removable neighbours until 30% of the current CI is planned. Each unique removal's BA is charged to its own region, capped at either the sample-derived ceiling or 40%. The sample-derived ceiling is the **17.90%** BA share of the MFM-02 live harness's own initial release. A real player's sample will set a different ceiling.

**16 m, unit-centre-first** recommended candidates: R1C PO0321 · R1E PO0333 · R2W PO1107 · R2C P0006 · R2E PO1134 · R3W PO2107, PO1700 · R3E PO2133, PO1740 · R4W PO2907 · R4C P2010 · R4E PO2933 · R5W PO3709, PO3501 · R5C PO3919 · R5E PO3736, PO4028.

What the table shows:

- Below 14 m, the release indicator cannot serve every candidate inside the sample ceiling. That is 29/33 at 12 m and 24/41 at 10 m.
- At 14 m, all 25 candidates reach 30% but the heaviest region hits 17.4% against the 17.9% ceiling, only 0.5 pp of headroom.
- At **16 m**, all 17 candidates reach 30% with the heaviest region at 14.9% (3.0 pp of headroom), and every covered region still gets at least one candidate.
- At 18 m and 20 m, regions are left without candidates (R2C; R2C and R5C).
- Order matters as much as spacing: west-east-scan at 16 m gives 23 candidates and 17.8% heaviest.
- This independently corroborates, on the real MFM-02 fixture, the unaccepted MFM-03B direction ("16 m unit-centre-first"). MFM-03B used a different rack layout and a 22.17% ceiling.

## 11. Recommended V1 metric set and tolerances (for Manager decision)

| element | recommendation | evidence label |
|---|---|---|
| Comparison partition | MFM-02 rack-served units, each split at the sample anchor ±12 m into W/C/E thirds. The source third is Manual/source. MFM-02 geometry is unchanged. | GAMEPLAY CALIBRATION (robustness measured: §7) |
| Comparison state | Source = teaching footprint, post-access, **pre-treatment** snapshot. Targets = post-access current state, with no executed treatment inside them. Same masks on both sides. | MODEL DERIVATION (state sensitivity measured: §7) |
| Density | Living stems per **masked net hectare**; \|Δ\| ≤ **20%** | GAMEPLAY CALIBRATION (plateau ≥ 12.3%) |
| Mean DBH | \|Δ\| ≤ **15%** | GAMEPLAY CALIBRATION (non-discriminating at year 0; 15% keeps headroom for 2–3 growth years, which the live MFM-02 growth step puts at +2.6%/yr) |
| Mean competition | Mean of authoritative `CompetitionShare` / Hegyi CI (8 m); \|Δ\| ≤ **20%** | Metric: DIRECTLY EVIDENCE-BACKED / existing model authority. Tolerance: GAMEPLAY CALIBRATION (plateau 16.6–33.2%; about 4–6 SE) |
| Minimum stems | n ≥ **30** per region before a similarity decision | GAMEPLAY CALIBRATION (all fixture regions ≥ 53; 30 gives about 4 SE at 20%) |
| Crowded-tail (P90) guard | **Not adopted for V1** (tested; fails robustness) | — |
| Former-edge band | Flag in the explanation; do not exclude | MODEL DERIVATION (≤ 2.2 pp effect measured) |
| Outer remainders | Always explicit; non-empty → **Manual** | ACCEPTED GAME DESIGN (spec §4.2) |
| Hard exclusions | Corridors, headland, feeder, track, clearing and sites; planted juveniles, shelters, clearance, protected areas, deadwood records; conflicting player marks, Crop Trees and open work; non-Sitka adults | ACCEPTED GAME DESIGN (spec §10.3) |
| Natural regeneration | Record and flag; not an exclusion; no tolerance | MODEL DERIVATION (felling does not clear cohorts) + Unity residual |
| Candidate spacing | **16 m**, dedicated parameter, **unit-centre-first** position-only order, ties broken by x, then z, then ID. Spacing is measured to sample Crop Trees and earlier suggestions. Batch suggestion; every candidate individually confirmed or rejected; no bulk action; no silent replacement. | GAMEPLAY CALIBRATION (workload and indicator measured: §10). Separate from the 8 m teaching spacing and 8 m Hegyi radius. |
| Time per decision, walking speed | 15–60 s; 4 m/s | ASSUMPTION (not measured) |

Outcome on the fixture:

- **13 of 14** non-source thirds are covered (0.444 ha net).
- **R1W is not covered**, because of edge competition.
- R3C is the source.
- The 16 m queue holds **17 individual decisions**: about 10 minutes at 30 s each, including walking. Up to 30 decisions are shown if half are rejected under queue re-suggestion.

**Why this qualifies.** Every recommended tolerance sits on a measured plateau, with a clear gap to both the nearest covered and the nearest failed unit. It is insensitive to the clearing-mask choice. It gives interpretable reasons ("edge corner, much less competition"). The fragile alternatives (whole strips, equal thirds, the P90 guard, CI ±15%) were measured and set aside for stated reasons.

The coincidence with the historical Stage C mean values (20/15/20) is a measured outcome, not an inherited assumption. Stage C's proposed extras, the tail guard and the "no regeneration" condition, are **not** carried forward.

## 12. Unity-only residual need (queue behind the serialized Unity lane)

The similarity calibration above does not depend on Unity. Three items cannot be established offline, and the umbrella integrator should queue them:

1. **The live post-Lesson-5 state at extension time.** This covers growth drift between the source snapshot and the extension year, mortality, natural cohorts per 5 m cell, any planted juveniles, shelters or clearance, deadwood records, marks and open orders.
2. **A whole-stand live identity dump.** Offline identity is proven only for the 75-stem footprint, the band counts and the stand total.
3. **Live CI parity.** `ForestEcologyController.GetCompetitionIndex` should be compared with the offline `CompetitionShare` values on the live trees.

**Proposed measurement:**

- Add a disposable dump step to a copy of `Tools/Verification/MfmRackSample/MfmRackSampleGeometryVerification.cs` (mode `v2`). Write per-tree `{id, x, z, dbh, height, species, living, markType, GetCompetitionIndex}`, plus per-cell cohorts and the scenario's juveniles, shelters, clearances, deadwood and open orders.
- Write the dump to `CCF_ACCEPTANCE_OUTPUT` at two points: (a) right after the rack access executes and its review is acknowledged (the source snapshot year), and (b) after the Lessons 2–5 chain completes, including J/K review and reload (extension time).
- Then run `analyse_coverage.py` against JSON converted from each dump.
- Accept the calibration as unchanged if:
  - the dump (a) footprint SHA is still `c2f2fd5418794915`;
  - live CI equals the offline CI to within 1e-4;
  - at (b) the recommended classification of the 14 thirds is unchanged, or changes only for a stated live reason (regeneration flag, conflict exclusion or growth);
  - mean DBH drift between (a) and (b) is under 7.5% (half the DBH tolerance).

**Commands (inside a registered Unity lane, under the machine lock, after a storage preflight ≥ 15 GiB on ZX20 and with no other owner):** stage, run and remove the harness as MFM-02 did with `/media/jer/Files/CCF/Verification/MFM02-RackSampleGeometry-20261027T202415Z/driver/run_gate.py`:

```sh
--name MfmRackSampleGeometryVerification --method MfmRackSampleGeometryVerification.Begin \
--env CCF_MFM02_MODE=v2 --pass-token MFM02_V2_PASS
```

Update the driver's task-ID preflight from `MFM-02-RACK-SAMPLE-GEOMETRY` to the MFM-03 lock identity, and use a 3600 s timeout, warm. Restore the lane and release the lock afterwards. No Windows build.

4. **Real player-authored Rack V2 requalification** (spec and MFM-02 review) remains a later obligation. It is **not** satisfied by this five-rack fixture.
5. **Human candidate-confirmation UX** (queue, Next/Previous, real inspection time and rejection behaviour) remains a user-testing item (spec §8.2). The minutes above are not player measurements.

## 13. Preservation statement

- No production C#, scene, prefab, ScriptableObject, ProjectSettings, package, save, schema, map/UI, Work Plan, Rack V2, Lessons 2–5, species biology, economy, timber, pruning or S1-C/S1-D file was edited.
- No prescription, proposal or candidate state was implemented.
- The MFM-02 qualified fixture (75 stems / 1.4417531 m² / `c2f2fd5418794915`), model-0/Legacy40 fixtures and Reference Future v1 are untouched and not rebaselined.
- No Oak/Beech quantitative rule was added.
- **No Unity was launched and the Unity lock was not acquired.** No Windows build.
- Only `/media/jer/Videos/CCF-MFM03` was written in Git. Evidence was written only under `/media/jer/Files/CCF/Verification/MFM03-CoverageCalibration-20261029/`, and earlier MFM evidence directories were read, not modified.
- The build ran in a temporary directory that was removed afterwards. The worktree is clean after commit.

## 14. Remaining uncertainties and product decisions

1. **Accept the anchor-aligned thirds sub-partition** as the V1 comparison unit inside MFM-02's strips. The alternative is whole strips, which this measurement shows to be fragile.
2. **Accept "no tail guard"** for V1, which departs from the unaccepted MFM-03/03B proposals. Or ask for a different local-crowding signal later.
3. **Edge treatment.** On this fixture the classifier excludes only the stand-edge corner R1W, and the rest of the even-aged plantation is comparable. Is it acceptable that the extension covers about 93% of non-source thirds after one sample? The spec allows it (covered ≠ treated; candidates and approvals are still per tree and per prescription), but it is a product judgement.
4. **Batch or queue re-suggestion** after a rejection. Both are measured; batch is recommended as the simpler and more bounded option.
5. **Candidate spacing of 16 m** is fixture-specific GAMEPLAY CALIBRATION. A real player's sample sets a different BA ceiling, so MFM-04/11 must revalidate feasibility; the greedy indicator here is only an indicator.
6. **The live post-Lesson-5 state and natural-regeneration flag behaviour** need the Unity residual (§12).
7. These results come from **one** deterministic stand and one rack layout. They are not empirical forestry tolerances and must not be presented to players as forestry standards.

`COVERAGE CALIBRATION QUALIFIED — READY FOR MANAGER REVIEW`
