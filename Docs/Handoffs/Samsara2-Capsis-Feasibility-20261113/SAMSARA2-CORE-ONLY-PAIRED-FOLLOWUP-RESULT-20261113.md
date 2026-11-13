# #461 Samsara2 paired core-only treatments and focal attribution — result

**Date:** 13 November 2026
**Claim:** [#461](https://github.com/jermur788/CCF/issues/461), HANDED-OFF for independent Manager review (APPROVED / NEEDS CHANGES / REJECTED)
**Decision:** **GO for controlled offline comparison only.**
- The #456 directional findings hold up under a paired, core-only design.
- The treatment called "local competitor release" must keep its honest name. It is **large silver-fir removal under focal-guided weights**, and focal-tree adjacency is shown to be *absent*, not merely unverified.
- Nothing here is approved for CCF production, numerical transfer or Irish validation.

**Worker:** Anthropic Claude Opus 5.5 (`claude-opus-5-5`), Claude Code in T3 Code, on the HP Windows 11 host under WSL Ubuntu.

**Packet:** [#461 conditional packet](https://github.com/jermur788/CCF/blob/docs/manager-456-research-spike-verdict-20261113-v1/Docs/Handoffs/Samsara2-Capsis-Feasibility-20261113/SAMSARA2-CORE-ONLY-PAIRED-TREATMENT-FOLLOWUP-CONDITIONAL-PACKET-20261113.md). It builds on [#456 result](https://github.com/jermur788/CCF/blob/c7763776bc6a9d8d3506262605cabbc11d7771eb/Docs/Handoffs/Samsara2-Capsis-Feasibility-20261113/SAMSARA2-OFFLINE-SPIKE-RESULT-20261113.md) and the [#456 Manager review](https://github.com/jermur788/CCF/blob/docs/manager-456-research-spike-verdict-20261113-v1/Docs/Handoffs/Samsara2-Capsis-Feasibility-20261113/MANAGER-456-OFFLINE-SPIKE-INDEPENDENT-REVIEW-20261113.md).

**Labels used below:**
- **OBSERVED:** executed model output.
- **PROXY:** from the kernel thinner, not the Samsara2 thinner itself.
- **HYPOTHESIS:** an interpretation that has not been tested.
- **UNKNOWN:** not determined.

## 0. Gate 0 — ownership and boundaries

**A prospective board entry existed *before* any worktree edit, install or run.**
- Commit: `main` `d39c024416abb4614eef5c8654ddad0a686deaac`, `coordination: register prospective #461 Samsara2 research lane (user-transferred one-entry write)`.
- It is a 3-line addition: a top notice plus one Live-tasks row.
- **Why the worker wrote it:** Manager #424 is the sole board writer, but had not committed to the board since 00:34 UTC despite open requests. The packet forbids repeating the #456 workaround. When asked, the project owner explicitly chose to transfer this one board write to the worker.
- The entry says #424 remains sole writer for everything else, and that the #456 exception is not reused as authority.

**Lane:**
- Branch: `research/samsara2-core-only-paired-20261113-v1`.
- Worktree: `/home/jer/Development/CCF-HP-Worktrees/CCF-Samsara2-CorePaired-20261113`.
- Starting HEAD: `d39c0244`.
- RUNNING registration: [#461 comment](https://github.com/jermur788/CCF/issues/461#issuecomment-6856634738).

**Data root:** a new, isolated `/home/jer/Research/Samsara2-CorePaired-20261113/`. The #456 root was used **read-only**, only for its pinned toolchain and CAPSIS install. No #456 evidence was overwritten.

**Preflight:**
- Pinned versions present: CAPSIS revision 22961 (installer SHA-256 `cca418b2…`), Temurin 1.8.0_512, R 4.5.3, martelosim 0.1.3.
- No CAPSIS `memory` override. It was reset after each martelosim call and is absent at handoff.
- 6 vCPU, 7.2 of 7.7 GiB free, 928 GB disk free.
- No Unity or Java process on Windows or WSL. The run loop aborts if a Unity process appears.
- The main checkout was clean, and neither the branch nor the worktree collided with anything.

**Not touched:** Unity, CCF source, assets, saves, specs, Windows builds and global Java/R. No GPL module code was imported or modified, and nothing was decompiled.

**Rights (as in #456):**
- The Samsara2 module is GPL-3.0-or-later, and the installer contains no Samsara2 Java source.
- The CAPSIS kernel is LGPL-2.1 **and its source is shipped**: 2,717 `.java` files under `src/capsis`.
- The Prelenfrey data has no explicit redistribution licence.
- Binaries, data and outputs stay off GitHub.

## 1. Gate 1 — one frozen initial population and its denominator (OBSERVED)

**Shared stand:** martelosim 0.1.3 `prepare_samsara(create_periphery = TRUE, create_virtual_saplings = TRUE)` ran **once**, in 24.6 s. Its exported `Prelenfrey_finalInventory.txt` has SHA-256 `9b50cf8674fda700f0a187f855044963535cbdab5a25ae562b1f00030f887d51`.

It contains:
- the 645 measured trees;
- 647 saplings: 183 from the field data plus generated virtual saplings;
- cohorts.

The generated periphery trees from `PeripheryCreator` are **not** in the exported inventory. Treatment simulations therefore run with **no generated edge trees**, as martelosim's own workshop command files do.

**Pairing verified empirically:**
- All 15 treatment runs load byte-identical inventory copies.
- The stand-creation tree table (ID, species, x, y, DBH) has the identical SHA-256 `2fc2b6d5…5d23` in **all 15 runs**.
- The initial stand row is identical in all runs: N 551.282/ha, G 29.659 m²/ha, V 358.674 m³/ha, saplings 552.99/ha, ground irradiance 20.451 MJ/m²/yr.
- **Initial-state variance is exactly 0.** All replicate variance in this study is downstream: thinner selection plus growth, mortality and regeneration stochasticity.

**Core-only denominator, confirmed from program output:**
- `exportCoreOnly = true`.
- martelosim writes `corePolygon` as the full plot rectangle, −3…127 × −1…89 m.
- 645 trees ≥ 7.5 cm ÷ 551.282 N/ha = **1.17 ha** = 130 × 90 m. The program's scene area also reported 11,700 m².

**Differences from #456 to keep in mind:**
- The denominator is the **1.17 ha inventory rectangle**, not the official 1.10 ha marteloscope polygon (0–125 × 0–88 m).
- There are no generated periphery trees: #456 counted them, so N rose to 585/ha there.

**What the pairing holds fixed (OBSERVED, a design limitation):** one prepare draw. Two extra prepare draws, not used for treatments, gave 598 and 607 saplings against 647 (CV about 4%; every inventory hash differs). All treatment contrasts are conditional on draw 1, which is at the high end for saplings. That is the price of exact pairing.

**Calendar:** martelosim dates the stand **2026**. Interventions were therefore in 2026 and 2036, ending 2056: the same 0/+10/+30-year schedule as #456's 2023/2033/2053.

## 2. Gate 2 — treatment replay and identity

**Treatments:** T0 No-cut, T1 focal-guided and T2 broad thinning. Their thinning rows were generated by column name from the official Prelenfrey `Multicriteria_Random` template, with the **exact #456 rules**:
- 20% basal-area cap, target G 20 m²/ha, 100% thinning, no retention trees, no deadwood harvest.
- T1: 50 focal trees/ha, focal weights light-to-value 50 and distance to focal 50; thinning weights DBH +25 and distance to focal −75.
- T2: no focal trees; thinning weights DBH −50 and distance to cut +50.

**Same removal intensity (OBSERVED):** both first cuts removed 6.94 m² over the scene, which is 5.93 m²/ha, exactly 20% of 29.66. The cap was binding.

| First cut, 2026 (5 runs each, Samsara2 thinner) | T1 focal-guided | T2 broad |
|---|---|---|
| Trees cut | 19–21 | 480 in every run |
| Mean DBH of cut trees | 63.2–67.8 cm | 12.93 cm in every run |
| Species of cut trees | 95–100% SAP *Abies alba* (silver fir), plus at most 1 HET (beech) or ERS (sycamore maple) | mixed small stems |
| Overlap of cut sets between runs (Jaccard) | **0.36** (stochastic selection) | **1.00** (deterministic) |

### 2.1 Focal-tree identities

- **Not observable in Samsara2 outputs:** the script prints only `-> found 59 focal trees`. Focal IDs for the **actual Samsara2-thinner runs remain UNKNOWN.** The thinner's `ExportThinningTable` is reachable only from the GUI dialog.
- **What I did instead (PROXY):** CAPSIS ships the LGPL kernel thinner `capsis.lib.multicriteriathinner2.MultiCriteriaThinner2` with source, and `Samsa2Model` declares `MultiCriteriaThinner2Compatible`. Its selections are kept in **protected** fields (`focalTrees`, `thinningTrees`, `cutList`).
- **The driver:** a 2-class research-only driver outside the repo, `driver/src/ccfresearch/FocalProbe*.java`. It subclasses that kernel thinner through ordinary inheritance and sets the exact #456 parameters. It loads the **same** prepared inventory through the public `GScript2` and `Samsa2InitialParameters(String)` API, then calls `apply(false)`, the preview path, so the selection is computed but nothing is cut.
- **No Samsara2 code was decompiled, modified or copied.**

**How good a proxy it is (OBSERVED):**
- **T2:** probe and Samsara2 cut sets are identical (Jaccard probe-vs-Samsara 1.00; 480 trees, mean DBH 12.93 cm, G 6.92 m²). The proxy reproduces the Samsara2 thinner for this configuration.
- **T1:** they are **not identical**.
  - The probe is deterministic: 20 out of 20 previews gave the same 59 focal trees and 17 cut trees, all SAP, mean DBH 70.6 cm, G 6.68 m².
  - The Samsara2 thinner is stochastic on the identical stand: 19–21 trees, mean DBH 63–68 cm, G 6.94 m².
  - Probe-vs-Samsara overlap (0.32) is about the same as Samsara-vs-Samsara overlap (0.36). Two or three trees cut in each Samsara2 run are focal in the probe, so Samsara2's focal sets differ from the probe's.
  - **Conclusion:** the T1 attribution below is PROXY evidence about this *rule set*, not the identity of Samsara2's own focal trees.

### 2.2 Cut-to-focal distances, T1 (PROXY unless stated)

Distances use a torus over the 130 × 90 m plot, matching the thinner documentation's torus approach; Euclidean distances are kept off-repo.

| | Cut trees | All other living trees |
|---|---|---|
| Probe: focal trees | 59, mean DBH **20.2 cm**; HET beech 41%, SAP silver fir 39%, ERS sycamore 8.5%, others | — |
| Probe: median distance to nearest focal tree | **7.47 m** | 6.17 m |
| Probe: share within 5 m of a focal tree | **23.5%** | 36.4% |
| Samsara2 runs (OBSERVED cuts) vs the probe focal set: median distance | 7.24–7.51 m | 6.13–6.17 m |
| Samsara2 runs: share within 5 m | 14–26% | 36–37% |

**Interpretation (OBSERVED for the probe; consistent for the Samsara2 cuts):**
- Light-to-value efficiency picks **small, efficient trees** (mean DBH 20 cm) as focal trees.
- The DBH +25 thinning weight then dominates the normalised score, so the thinner removes the **largest silver firs**.
- Those are **not** neighbours of focal trees: they sit *farther* from focal trees than ordinary trees do.
- **Honest treatment name:** *large silver-fir removal under focal-guided weights.* It is **not a crop-tree neighbourhood release.** Crop-tree-release claims stop here, as the packet requires.

### 2.3 Bounded weight sweep (PROXY, preview only, 3 previews per setting, no growth runs)

| Thinning weights (DBH : distance to focal) | Cuts | Mean DBH | Within 5 m of focal, cut vs other | Median distance to focal, cut vs other |
|---|---|---|---|---|
| 25 : −75 (#456 rule) | 17 | 70.6 cm | 24% vs 36% | 7.47 vs 6.17 m |
| 0 : −100 | 135 | 20.2 cm | **43% vs 34%** | 5.75 vs 6.36 m |
| −25 : −75 | 451 | 13.3 cm | 37% vs 32% | 6.13 vs 6.58 m |
| −50 : −50 | 451 | 13.3 cm | 37% vs 32% | 6.13 vs 6.58 m |

- Even pure distance weighting produces only a **weak** proximity bias. Within this parameter family, the DBH term and the 20% basal-area cap control which trees are selected.
- I didn't retune further. Producing a genuine neighbourhood release in Samsara2 would likely need a different approach, such as a smaller cap or spatially explicit marking. **UNKNOWN.**

## 3. Gate 3 — repeatability and outcomes (OBSERVED, 5 runs per treatment, mean ± SD, core-only per ha)

| Metric (2056 unless noted) | T0 No-cut | T1 Focal-guided (large silver-fir removal) | T2 Broad thin |
|---|---|---|---|
| Live G removed, first / second cut (m²/ha) | 0 / 0 | 5.93±0.00 / 6.25±0.09 | 5.91±0.00 / 6.03±0.07 |
| Cumulative volume removed (m³/ha) | 0 | 178.1±1.5 | 99.2±0.5 |
| Ground irradiance just after the first cut (MJ/m²/yr) | 20.45 (uncut) | 24.13±0.29 | 31.03±0.01 |
| N/ha | 977±12 | 1191±25 | 718±13 |
| G (m²/ha) | 49.56±1.20 | 41.17±0.96 | 35.71±1.43 |
| V (m³/ha) | 623±18 | 468±10 | 485±22 |
| Quadratic mean DBH (cm) | 25.41±0.35 | 20.98±0.24 | 25.16±0.53 |
| G > 67.5 cm (m²/ha) | 19.20±1.25 | 8.70±1.55 | 21.41±1.98 |
| G, 17.5–47.5 cm (m²/ha) | 13.67 | 16.23 | 1.71 |
| Ground irradiance (MJ/m²/yr) | 9.27±0.33 | 9.39±0.63 | 20.68±0.94 |
| % area in the lowest light class (0–20) | 92.7±1.0 | 92.5±2.9 | 49.2±7.1 |
| Saplings/ha | 524±15 | 529±18 | 452±17 |
| Cumulative recruits / deaths over 30 years (N/ha) | 902±3 / 476±12 | 962±24 / 287±7 | 1107±10 / 124±13 |
| G by species (Alpine model species), from 2026 (SAP 22.9, HET 5.2, ERS 1.1) | SAP 36.1, HET 9.9, ERS 2.7 | SAP 24.2, HET 12.4, ERS 3.4 | SAP 31.6, HET 2.6, ERS 1.4 |

**Where the variance comes from (OBSERVED):**
- Initial state: 0, by construction.
- T2 first-cut selection: 0, deterministic.
- T1 first-cut selection: stochastic (Jaccard 0.36).
- Everything after that comes from model dynamics. 2056 coefficients of variation are about 1–4% for N, G and V, and about 3–4% for saplings.
- The single shared prepare draw is a separate source of variance that this design holds fixed (§1).

**Compared with #456 (2056 here vs 2053 there; same software versions, different start inventory and denominator):**

| #456 finding | #461 paired, core-only | Status |
|---|---|---|
| Equal basal area removed, opposite structure | 5.93 vs 5.91 m²/ha; T1 takes about 20 large silver firs, T2 about 480 small stems | **Holds** |
| T2 keeps very large trees and opens the ground; T1 shrinks the large-tree layer | VL G 21.4 / 19.2 / 8.7; irradiance 20.7 vs about 9.3 | **Holds** |
| T1 cuts deaths by about 43% vs no-cut; T2 lowest deaths | 476 → 287 (−40%) → 124 | **Holds** |
| T1 shifts silver fir toward beech; T2 removes small beech | SAP 24.2 / HET 12.4 vs T0 36.1 / 9.9; T2 HET 2.6 | **Holds** |
| Order of 2053 basal area (T0 > T1 > T2) | 49.6 > 41.2 > 35.7 | **Holds** |
| T1 more saplings than T0 (495 vs 468) | 529±18 vs 524±15 | **Not supported**: within noise |
| T2 fewest saplings despite most light | 452 vs about 525 | **Holds** (HYPOTHESIS: faster outgrowth into recruits; T2 has the most recruits, 1107) |
| T1 described as "local competitor release" | Adjacency absent (PROXY), large silver-fir removal (OBSERVED) | **Corrected name** |

**Runtime and memory per 30-year run on the 1.17 ha core:**

| Treatment | Wall time | Peak RSS |
|---|---|---|
| T0 | 28–31 s | about 0.95 GB |
| T1 | 51–56 s | about 1.66 GB |
| T2 | 25–28 s | about 1.26 GB |

These are faster than #456 because there are no edge trees. The probe used 26–29 s and 0.65–0.68 GB for 10–20 previews. **No extrapolation to CCF stands.**

## 4. GO / NO-GO

**GO for controlled offline comparison**, using this design:
1. One martelosim prepare draw shared by every treatment.
2. `exportCoreOnly = true`, with the denominator stated (1.17 ha here).
3. n ≥ 5 replicates.
4. Report the cut identities from per-tree output.
5. Ideally, repeat across several prepare draws if conclusions must not depend on one draw.

**Use the kernel-thinner probe only as a proxy.** It matched Samsara2 exactly for broad thinning but not for focal-guided thinning.

**NO-GO for:**
- calling the #456 T1 rule a crop-tree release;
- any CCF production use, code port, coefficient import or Irish calibration;
- treating this as external scientific validation.

The model species are *Abies alba*, *Fagus sylvatica* and *Acer pseudoplatanus* on an Alpine site, not Irish Sitka spruce, oak or beech.

## 5. Off-repo evidence (not committed)

- **Root:** `/home/jer/Research/Samsara2-CorePaired-20261113/`. Sizes: runs 129 MB, inputs 1.1 MB, logs 2.7 MB, scripts 120 KB, driver 44 KB.
- **Manifest:** `MANIFEST-SHA256.txt`, 518 entries, SHA-256 `e888d751b17e3673d9d1708cf878467ec2bd680c027b598e261c6128d2cddaa4`.

Key hashes:

| File | SHA-256 |
|---|---|
| Shared prepared inventory | `9b50cf86…7d51` |
| Paired command files T0 / T1 / T2 | `bbed811d…`, `c2e34cef…`, `bcaf65ee…` |
| `runs/_paired_checks.json` (pairing, denominator, cut IDs) | `db5e3c8b…` |
| `runs/_attribution.json` | `06e0b5d9…` |
| `_attribution_samsara_vs_probefocal.json` | `eeb012b4…` |
| `runs/_summary/summary.{csv,json}` | `f1cbd59b…`, `6854c0df…` |
| Probe selections T1 / T2 | `07b4991d…`, `85178870…` |
| `FocalProbe.java` / `FocalProbeMain.java` | `d1590e14…`, `83aedd56…` |

**Commands:**
- Preparation: `Rscript scripts/prepare_once.R`.
- Inputs: `scripts/build_inputs.sh` (runs `make_paired.py`).
- Treatment runs: `scripts/run_paired.sh 5`, which runs `capsis.sh -p script samsara2.pgms.flexiblescript.ScriptFlexibleSimuFormat2 <run>/Paired_CommandFile.txt`.
- Probe: `driver/build.sh` and `scripts/run_probe.sh {T1 20 | T2 10}`, which run `java -cp ./class:./ext/*:…:driver/classes capsis.app.Starter -p script ccfresearch.FocalProbeMain`.
- Sweep: `scripts/sweep.sh`.
- Analysis: `paired_checks.py`, `attrib.py`, `attrib2.py`, `outcome.sh`, `extras.sh`.

**Not done or not verified:**
- Focal IDs of the actual Samsara2-thinner runs (UNKNOWN).
- Whether the Samsara2 and kernel thinners are equivalent for T1 (they differ).
- Multiple prepare draws carried through the treatments.
- Rendered martelosim reports.
- Independent re-execution.
- Licences of third-party jars and the data.
- **Dashboard/Drive sync: NOT VERIFIED.**
