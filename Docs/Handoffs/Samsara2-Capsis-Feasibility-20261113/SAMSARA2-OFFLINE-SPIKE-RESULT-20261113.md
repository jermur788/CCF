# Samsara2 offline feasibility spike — result (#456)

**Date:** 13 November 2026
**Claim:** [#456](https://github.com/jermur788/CCF/issues/456), now HANDED-OFF for independent Manager review (APPROVED / NEEDS CHANGES / REJECTED)
**Decision:** **GO for offline comparison/benchmark use only.** This is not approval for production integration, a Java-to-C# port, parameter import, gameplay calibration or Irish ecological validity.
**Worker:** Anthropic Claude Opus 5.5 (`claude-opus-5-5`), Claude Code harness in T3 Code. It ran on the **HP Windows 11 host under WSL Ubuntu**. The project owner directly told the worker to use this device instead of the ZX20 laptop; the lane repin is recorded on #456 and #424.
**Lane:** branch `research/samsara2-offline-trial-20261113-v1`, worktree `/home/jer/Development/CCF-HP-Worktrees/CCF-Samsara2-OfflineTrial-20261113`. Starting HEAD was live `origin/main` `6c13a9e182774945bf391bc8d4845ce74d3e5e88`.
**Basis (on branch `docs/samsara2-capsis-feasibility-20261113-v1`):** [Manager release](https://github.com/jermur788/CCF/blob/docs/samsara2-capsis-feasibility-20261113-v1/Docs/Handoffs/Samsara2-Capsis-Feasibility-20261113/MANAGER-456-USER-AUTHORISED-LOCAL-SPIKE-RELEASE-20261113.md), [conditional protocol](https://github.com/jermur788/CCF/blob/docs/samsara2-capsis-feasibility-20261113-v1/Docs/Handoffs/Samsara2-Capsis-Feasibility-20261113/SAMSARA2-OFFLINE-FEASIBILITY-SPIKE-CONDITIONAL-PACKET-20261113.md), [static licence audit](https://github.com/jermur788/CCF/blob/docs/samsara2-capsis-feasibility-20261113-v1/Docs/Handoffs/Samsara2-Capsis-Feasibility-20261113/SAMSARA2-UPLOADED-INSTALLER-STATIC-LICENCE-AUDIT-20261113.md).

## 0. Boundaries kept

**Nothing in CCF was touched:**
- No Unity Editor or Player and no Windows build.
- No change to CCF production code, `Assets/**`, packages or settings, saves, PlayerPrefs, specs or decisions, or the frozen Reference Future.
- No edit to `active-tasks.md`. Manager #424 remains its sole writer and has been asked to mirror this lane.

**This commit is the only repository change.** No binaries, CAPSIS files or model outputs were committed.

**All tools and data stayed outside the repository,** under `/home/jer/Research/Samsara2-20261113/`:
- Java 8, R, CAPSIS, inputs, runs and logs all live there.
- Nothing was installed with `apt` or system-wide.
- `HOME` was redirected to a throwaway folder, so the installer's desktop shortcut didn't land in the real home folder.

**Objective 1 right of way.** This host is separate from the ZX20/Files Objective 1 lanes. Before running, I confirmed that no Unity or Java process was running on Windows or WSL. The run script refuses to start if a Unity process exists in WSL. I did not check the other machine's state.

**Labels used below:**
- **OBSERVED:** printed by the executed model.
- **TRANSFER:** relevance to CCF.
- **HYPOTHESIS:** an interpretation that has not been tested.
- **UNKNOWN:** not determined.

## 1. Exact versions and provenance (OBSERVED)

| Item | Value |
|---|---|
| Installer | `capsis-Samsara2_4.2.7-22961_setup.jar`, 194,797,004 B, SHA-256 `cca418b217fc0e36f828990b32d79f27712e0193f2565754883d9aaabd871a13` (matches the audit). User-supplied file. I did not verify vendor signature or official download origin. |
| Installed CAPSIS | `revision` file = **22961**; the boot banner reads `Capsis 4.2.7-22961`. The IzPack metadata still says `appVersion 4.2.6`, which is a packaging inconsistency only. |
| Install method | Headless IzPack auto-install (`java -jar setup.jar auto-install.xml`) with packs Base and Data; took 3.5 s; footprint 282 MiB |
| Java | Eclipse Temurin **1.8.0_512-b09**, from the official Adoptium API; tarball SHA-256 `fb3bfa86…f7e4` verified against Adoptium's published checksum. The installer declares Java 1.8. |
| R | **R 4.5.3** from conda-forge, in an isolated micromamba 2.10.0 prefix, plus a conda C/C++ toolchain for source builds |
| martelosim | **0.1.3**, from `inrae.r-universe.dev`, RemoteSha `a848a56c795673d33fd1f0ffced22442c3892c02`, packaged 2026-10-26, MIT licence |
| Demo inventory used | `Prelenfrey_Inventory_C10_FieldRege_LowSap4.txt`, exported by CAPSIS 4.2.7-22930 on 30 Sep 2026. It is the inventory named in the official tutorial `Samsara2_TD_2026-10-05.docx`. |

## 2. Licence / rights finding

- **The Samsara2 module is GPL-3.0-or-later.** I confirmed this in the installed `class/samsara2/license_en.txt` (© 2026 Benoit Courbaud). The CAPSIS kernel is LGPL-2.1 (`class/capsis/license_en.html` and the installer's licence panel).
- **Correction to the static audit: no Samsara2 Java source is shipped.** The `src/samsara2/` folder contains only 93 `.properties` files, one `.veto` file and the licence texts; the 430 classes are compiled only. Source is not bundled, so GPL source access would have to come from the authors. I did no decompilation; inspection was limited to the constant strings `javap` lists and public signatures.
- **The CAPSIS licence panel sets a rule:** a module with no licence file is unpublished and belongs to its authors. Only `capsis` and `samsara2` have licence files here, which is consistent with this installer containing only Samsara2.
- **Other licences not assessed:**
  - The bundled `ext/*.jar` third-party libraries (GeoTools, JFreeChart, Groovy and others) carry their own licences, which I did not review individually.
  - The DemoFiles data and documentation (`.docx`, `.pptx`, PDF papers) have **no explicit data licence**. They were used locally for research only and were not redistributed. Any reuse beyond private research is **UNKNOWN** and needs author permission.
- **Implication for CCF:** GPL module code must not be imported into the Unity repository; that would raise distribution and derivative-work obligations. Running it offline as an external reference is fine for research.

## 3. P1 — Native Prelenfrey example

Command, run from the install folder:
`./capsis.sh -p script samsara2.pgms.flexiblescript.ScriptFlexibleSimuFormat2 <run>/Prelenfrey_CommandFile.txt`
`java` was on `PATH` via the isolated JDK, and the default heap was `-Xmx1600m` from `capsis.sh`.

### 3.1 The shipped demo does not run unmodified on 22961 (OBSERVED, material)

| Attempt | Failure printed by CAPSIS | Minimal recorded fix (in a copy; the install was left pristine) |
|---|---|---|
| 1 | `Error while interpreting: saveProject = FALSE, expected type: boolean` | `FALSE` → `false` |
| 2 | `Missing expected keywords in the inventory file: regeHeightClassLimits_m`. The demo's `Prelenfrey_Inventory.txt` is format version 21440 from 2024. | Use the same plot's newer official inventory, `…C10_FieldRege_LowSap4.txt` (same 645 tree IDs; all 645 marking IDs match) |
| 3 | `PeripheryCreator could not create trees, no core polygon in the scene`. The newer export has no `corePolygon`. | Copy the `corePolygon` record **verbatim** from the official older inventory of the same plot. The polygon spans 0–125 × 0–88 m, which lies inside the new plot's −3–127 × −1–89 m, and every tree is inside it. |

**Nothing was invented:** no parameters or values were created. The diffs and file hashes are in the off-repo manifest (§8).

**What this means:** the demo command file and its default inventory lag behind the 22961 parsers. The martelosim pathway (§4) avoids this because it builds a fresh new-format inventory.

### 3.2 Successful native run (OBSERVED)

This is scenario 2 "Random" exactly as published:
- 2023: stand creation, `PeripheryCreator`, `RegenerationInstallation` (virtual saplings) and the `MarteloThinner` field marking `Marked_Random`.
- 2033: one `MultiCriteriaThinner` run with the official weights.
- 2043: end.

The run exported outputs for stand (203 columns per species and for all species), trees, cells, CAT carbon and harvested-wood products, SylvaRock rockfall, the final inventory and Economics2 NPV.

| Run | Wall | Peak RSS | Model `computingTime` |
|---|---|---|---|
| native-r1 | 40.6 s | 1,269 MB | 38.1 s |
| native-r2 | 44.9 s | 1,195 MB | 43.7 s |
| native-r3 | 42.1 s | 1,226 MB | 38.2 s |

The model used about 260–330% CPU of 6 vCPU. Runs r2 and r3 overlapped with the R install, so their timing is slightly inflated.

### 3.3 Reproducibility (OBSERVED)

- **There is no seed control in the script interface.** The flexible-script reader's public fields contain no seed key. The model holds `java.util.Random` and Apache `RandomDataImpl` instances, but I found no way to set the seed. Determinism therefore **cannot** be claimed.
- **Identical input gives different output.** The SHA-256 of every output file differs across 3 identical runs.

What varies and by how much (3 runs, mean ± SD):

| Stage | Behaviour |
|---|---|
| Stand creation from inventory | **Deterministic** (SD 0): N 551.28/ha, G 29.66 m²/ha |
| Field marking `Marked_Random` | **Deterministic selection**: the same 203 trees in all runs |
| Periphery generation | Stochastic: G 31.77 ± 0.48 |
| Virtual sapling generation | Stochastic: 432 ± 24 saplings/ha (CV 5.5%) |
| 2033 multicriteria cut | 13–15 trees, cut G 5.84 ± 0.36 |
| 2043 state | N 887 ± 36 (CV 4.0%), G 30.55 ± 0.58 (CV 1.9%), ground irradiance 15.75 ± 0.38 MJ/m²/yr, saplings 447 ± 32 (CV 7.2%) |

**Practical rule:** treat Samsara2 as a stochastic model. Compare treatment means across replicates (3 replicates here; more for small effects). Don't diff single runs.

## 4. P2 — Direct flexible scripting vs martelosim

Both pathways call the same Java entry point: `capsis.sh -p script samsara2.pgms.flexiblescript.ScriptFlexibleSimuFormat2 <commandFile>`.

**martelosim 0.1.3 works with 22961 (OBSERVED).** Its `martelosim_options()` contains `ref_version <- 22961` and refuses older revisions. The audit's concern about 22711 versus 22961 applied to the older 0.1.2 guide and is **resolved**.

I ran the packaged workshop scripts `01_start.R` and `02_test.R` headless on the packaged Prelenfrey example, with no edits to package code. Results:

| Step | Result |
|---|---|
| `martelosim_options` | OK (1.7 s) |
| `new_inventory("Prelenfrey")` | OK |
| `build_samsa2_files` | OK (7.6 s). Writes a new-format inventory from the Excel data sheet. |
| `prepare_samsara` | OK (25.7 s). Periphery and virtual saplings are made once, then a final inventory is exported. |
| `random_marking` | OK |
| `write_samsara_cmd` (groups Random and Free_Growth, thinning every 10 years, 50 years, "auto") | OK |
| `run_samsara(memory_perc = 75)` | OK (202.1 s for 2 scenarios × 50 years), 0 exceptions. Peak RSS of the whole R session was 4.24 GB with a 6,178 MB heap. |
| `martelo_report` (quarto PDF) | **Failed because of my environment:** the conda `quarto` binary is missing `deno`. Model execution was unaffected. |

**Side effect:** `run_samsara` writes a `memory` file into the CAPSIS install through `setmem.sh`. I reset it afterwards with `sh setmem.sh`.

| Capability | Direct flexible script | martelosim 0.1.3 |
|---|---|---|
| Intervention vocabulary (from the reader's public fields) | Full: MultiCriteriaThinner, TargetDensity, RectangularGap, RectangularGapTargetG, DHAThinner, UnevenAgedManager, MarteloThinner, sapling plantation and cleaning in cells or circles, Disturbance (storm), Economics2, multiple scenarios per file | MarteloThinner, plus MultiCriteriaThinner parameters derived from field marking ("auto") or a silviculture sheet ("expert"). Fixed thinning interval. |
| Periphery and saplings | Per scenario, so each treatment gets its own random draw | Prepared once and shared by all groups, so treatments start from an identical prepared stand. This is a **better control design** for comparisons. |
| Input format robustness | The shipped demo files needed the 3 fixes in §3.1 | Builds a current-format inventory itself |
| Seed | None | None, same engine |
| Best use for CCF research | Arbitrary controlled treatment designs (§5) | Reproducible marteloscope-style comparisons and reports |

**Recommendation (HYPOTHESIS):** use martelosim's prepare stage to create one shared prepared inventory. Then drive custom treatments through direct command files that start from that final inventory. This combines martelosim's shared start with the direct script's full intervention vocabulary. It has not been executed in this spike.

## 5. P3 — Controlled treatment comparison (OBSERVED, research-only)

**Design:**
- All treatments use the same fix-3 input and the official 2023 creation events: stand, periphery and saplings.
- Interventions in 2023 and 2033, ending in 2053 (30 years).
- **3 replicates** per treatment, because there's no seed control.
- Command rows were generated by column name from the official `Multicriteria_Random` template row.

**Shared controls for T1 and T2:**
- No retention trees and no deadwood harvest.
- 100% of removals classed as thinning.
- Target G after cut 20 m²/ha, capped at **20% of G per intervention**. The cap was binding, so removal intensity is the same.

**Treatments:**
- **T0 No-cut.**
- **T1 Local competitor release:** 50 focal trees/ha, chosen with the documentation's example weights (light-to-value efficiency 50, distance to the nearest focal tree 50). Thinning weights were DBH +25 and distance to focal −75, the official Prelenfrey thinning weights.
- **T2 Broad thinning:** no focal trees. Thinning weights were DBH −50 (thinning from below) and distance to the nearest cut tree +50 (spreads the cuts across the stand).

Results (mean ± SD, n = 3):

| Metric | T0 No-cut | T1 Local release | T2 Broad thin |
|---|---|---|---|
| Live G removed, 2023 / 2033 (m²/ha) | 0 / 0 | 6.30±0.17 / 6.49±0.23 | 6.32±0.14 / 6.21±0.06 |
| Cumulative volume removed (m³/ha) | 0 | 169.1±6.1 | 101.1±2.2 |
| Trees removed per cut (scene) | — | 20–24, mean DBH ≈ 65 cm, mean nearest-neighbour distance between cut trees 10–15 m | 418–513, mean DBH ≈ 12–13 cm, nearest-neighbour distance 2.1–2.7 m |
| Ground irradiance just after the 2023 cut (MJ/m²/yr) | 17.86±0.23 (uncut) | 21.42±0.45 | 28.67±0.35 |
| 2053 N/ha | 880±30 | 1081±43 | 640±49 |
| 2053 G (m²/ha) | 50.52±1.15 | 40.80±0.23 | 35.75±1.81 |
| 2053 V (m³/ha) | 587±17 | 436±3 | 444±28 |
| 2053 quadratic mean DBH (cm) | 27.05±0.76 | 21.93±0.47 | 26.71±1.35 |
| 2053 G > 67.5 cm (m²/ha) | 21.8±1.9 | 8.2±1.9 | 23.9±2.0 |
| 2053 G, 17.5–47.5 cm (m²/ha) | 14.1 | 16.3 | 1.0 |
| 2053 ground irradiance (MJ/m²/yr) | 9.24±0.30 | 9.95±0.41 | 22.40±0.74 |
| 2053 % area in the lowest light class (0–20) | 94.3±1.8 | 91.7±0.5 | 43.9±5.6 |
| 2053 saplings/ha (< 4 m height class set) | 468±34 | 495±38 | 372±27 |
| Cumulative recruits / deaths over 30 years (N/ha) | 752±24 / 458±16 | 793±8 / 260±34 | 968±65 / 111±10 |
| 2053 G by species (model's Alpine species) | SAP *Abies alba* 37.0, HET *Fagus sylvatica* 9.7, ERS *Acer pseudoplatanus* 2.8 | SAP 23.6, HET 12.9, ERS 3.3 | SAP 31.6, HET 2.6, ERS 1.3 |

Runtime per 30-year run: T0 52–57 s, T1 73–77 s, T2 43–47 s. Peak RSS was 1.3–1.75 GB on one 1.1 ha plot with a 1600 MB heap. **I have not extrapolated these figures to CCF's full stand.**

**What the treatments actually did:**
- **OBSERVED: same basal area removed, opposite effects.** Both treatments removed nearly the same G, but did opposite things to structure and light. T2 removed many small trees spread across the stand, which opened the ground (lowest-light area fell from about 66% to about 27–44%) and kept the very large trees. T1 removed about 21 large trees per intervention. That shrank the large-tree layer, cut deaths by about 43% compared with no-cut, shifted silver fir toward beech, and left the ground nearly as shaded as no-cut by 2053.
- **OBSERVED: these weights did not give a classic crop-tree release.** With the documented weights, the multicriteria scoring picked large competitors, because DBH +25 dominated the normalised scores. The model doesn't export which trees were focal, so **cut-tree adjacency to focal trees was NOT verified.** T1 is better described as "focal-tree-guided removal of large competitors". Testing a true small-neighbourhood release would need a weight sweep, for example DBH 0 and distance to focal −100. **UNKNOWN.**
- **HYPOTHESIS: T2's lower sapling count despite much more light.** It is consistent with saplings growing out of the sapling class into recruits faster (T2 had the highest recruitment). I didn't test this.

**Mapping completeness and caveats:**
- **Stand outputs include generated periphery trees.** The official file sets `exportCoreOnly = false`, so N rises from 551 to 585/ha when the periphery is created. Cut-tree counts are whole-scene. Set `exportCoreOnly = true` for core-only statistics, as martelosim does.
- **Generated, not measured:** the periphery trees, the virtual saplings (`RegenerationInstallation`) and all later recruits. The original inventory trees, saplings and cohorts are measured field data.
- **Light edge handling:** the model's documentation says it uses a torus approach for light and distance calculations.
- `Ground_Irradiance_80_100_PercentArea` was 0 in every run, so it carries no information on this plot.
- Plot details: 1.1 ha core, 40° slope, aspect 75°, 1,280 m altitude, latitude 45.02 (French Prealps).

**Machine-readable outputs:** a per-state, per-metric CSV and JSON summary (mean, SD, CV, min, max) are kept off-repo (§8).

## 6. CCF relevance (TRANSFER / HYPOTHESIS, no production implication)

- **TRANSFER — useful as an independent qualitative benchmark.** Samsara2 makes the same total removal in different spatial patterns give clearly different structure, light, mortality and species trajectories. That supports CCF's design intent that marking choices, not just removal volume, should have visible consequences. CCF could check that its simplified rules move in the same *direction* for comparable contrasts:
  - thinning from below → more ground light;
  - removing big trees → large-tree basal area falls, density rises;
  - no-cut → closing canopy, more mortality, ground light falls.
- **The magnitudes do not transfer.** These are Alpine coefficients (Norway spruce, silver fir, beech, maples) on a steep Prealpine site. They are **not** Irish Sitka spruce, oak or beech, and no number should go into CCF. D-059, D-061, D-065, Rack V2, S1-C/S1-D, saves and the frozen Reference Future are untouched.
- **Architecture lessons (HYPOTHESIS):**
  - Treat stochastic replicates as a first-class part of any CCF-side validation.
  - Separate "prepare stand once" from "apply treatment" so comparisons are controlled; martelosim does this.
  - Express interventions as declarative event tables, as the command file does.

## 7. P4 decision: GO / NO-GO

**GO for offline comparison only, with conditions:**

1. Pin CAPSIS 22961 with installer SHA-256 `cca418b2…a13`, Temurin 8u512, martelosim 0.1.3 at `a848a56c`, and R 4.5.3.
2. Use the martelosim prepare stage, or the recorded fix-3 inputs. **Don't run the shipped demo command file unmodified.**
3. Always run replicates (n ≥ 3) and report mean ± SD. There's no seed control.
4. Use `exportCoreOnly = true` for core statistics in future runs.
5. Keep everything off the Unity repository; the module is GPL and its demo data has no explicit licence.
6. Before any T1-style claim, verify focal-tree adjacency, for example from per-tree outputs or a weight sweep.

**NO-GO for:** production integration, a code port, parameter or coefficient import, Irish calibration claims, or treating this as independent scientific validation of CCF.

## 8. Off-repo evidence (not committed)

- **Root:** `/home/jer/Research/Samsara2-20261113/` on the HP host WSL. Sizes: runs 200 MB, inputs 11 MB, logs 1.3 MB, scripts 148 KB.
- **Manifest:** `MANIFEST-SHA256.txt`, 550 entries, SHA-256 `aeab539356d9baead2216ebeb2fc3197cd9353cc12f92222dc8d0f2ec2e49203`.

Key hashes:

| File | SHA-256 |
|---|---|
| `inputs/prelenfrey-native-fix3/Prelenfrey_CommandFile.txt` | `6c8436a7…15b8` |
| `inputs/prelenfrey-native-fix3/Prelenfrey_Inventory_C10_FieldRege_LowSap4.txt` (with the `corePolygon` record) | `0ff8f563…8c94` |
| `inputs/prelenfrey-native-fix3-{commandfile,inventory}.diff` | `9d9cc851…7700`, `cb7ee30f…18a8` |
| `inputs/treatments/T0/T1/T2 …/Treatment_CommandFile.txt` | `df87ebbc…`, `ebba6dd2…`, `448970eb…` |
| `runs/_summary/summary.csv` | `6070cd0a…042e` |
| `runs/_summary/summary.json` | `6bfa3cb6…c8ec` |
| martelosim-generated `Prelenfrey_Martelo_CommandFile.txt` | `0cbdf9fa…57ff` |

**Folder layout:**
- `runs/<name>/` holds `_stdout.log`, `_capsis.log`, `_time.txt` (GNU time), `_run.txt` and `Output/`.
- Named runs: `native-r1..3`, `T0_NoCut-r1..3`, `T1_LocalRelease-r1..3`, `T2_BroadThin-r1..3` and `martelosim-prelenfrey/`.
- Logs: `logs/install.log`, `r_install*.log`, `martelosim_run.log`, `treatments.log`.

**Scripts in `scripts/`, in the order they were used:**
1. Install and setup: `get_jdk8.sh`, `auto-install.xml`, `get_r_martelosim*.sh`.
2. Building inputs: `make_native_fix*.sh`, `make_treatments.py`.
3. Running: `run_capsis.sh`, `run_treatments.sh`, `martelosim_prelenfrey.R`.
4. Analysis: `analyse.py`, `table.py`, `species.py`, `timings.sh`.

**Not done or not verified:**
- Rendered martelosim PDF reports (quarto/deno missing).
- An `exportCoreOnly = true` rerun.
- Focal-tree identities.
- Seed control.
- Vendor provenance of the installer.
- Licences of third-party jars and demo data.
- State of the other machine.
- **Dashboard/Drive sync: NOT VERIFIED.**
