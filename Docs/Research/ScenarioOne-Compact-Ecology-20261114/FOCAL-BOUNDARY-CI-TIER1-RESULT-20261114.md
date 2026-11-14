# #484 Tier 1 — focal boundary competition-index sensitivity (original #460 20 m / 30 m forests)

**14 November 2026 — RESEARCH HANDED OFF (offline, no Unity).** Worker: Anthropic **Claude Sonnet 5.5** (`claude-sonnet-5-5`) via Claude Code in local T3 Code — not the GPT-6.1 Sol / Codex worker the packet recommended. Branch `research/scenario-one-focal-ci-offline-20261114-v1`, started from `main` `fc18190b976f1363f1b5d1b0881810db2f8c6e39`, tool commit `1e38659e97a96ed0107ff7854b1ed0dc3a6c4093`. Companion handoff: `Docs/Handoffs/ScenarioOne-Compact-Ecology-20261114/TIER1-FOCAL-BOUNDARY-CI-HANDOFF-20261114.md`. Tool: `Tools/Research/ScenarioOneFocalBoundaryCI/`.

**Scope, stated once.** This compares the *current 8 m Hegyi CI* of eight identified original trees in the finite #460 scratch forests with the same CI if an **explicitly hypothetical** surrounding forest existed. It is **not** a proof that a 20 m or 30 m production plot is wrongly modelled, not a calibration, not a survey of any real neighbouring forest, not a measured canopy/seed/wind correction, and not a two-versus-three-species test. If the land beyond the property is open, the **finite result is the correct ecological boundary assumption**, and this study does not argue otherwise.

## 1. Result in brief

* **DATA GATE: PASSED.** All raw inputs were present, hash-verified and reconciled to the executed harness (§2). **8 of 8** focal tree IDs were verified in the original Year-0 data, and the offline recomputation reproduced each recorded native CI to ≤ 7e-7 absolute.
* **Hypothetical surrounding forest changes the near-boundary picture a lot.** Within about 4 m of an edge, adding the assumed forest raises CI by **+1.2 to +4.3 (+20 % to +232 %)**; at 7.07 m from an edge the change is small (**+0.32, +6 %**); at 8.8 m and 13.7 m from an edge it is **exactly zero** (no external stem within 8 m).
* **Scale of the effect.** The added amount for the five stems within 3.7 m of an edge (1.2–4.3) is comparable to the *whole* finite CI of an interior stem (≈ 6.1). With the assumed forest, four of the five edge stems reach CI 6.1–8.5, in or just below the range of the finite stands' own 2–8 m-band mean CI (6.9–8.7); the fifth, P0708 (10.8), is a small-DBH stem (12.98 cm), so each neighbour term is larger. In other words the assumed context makes edge stems look broadly like inside-stand stems rather than producing absurd values. This is a plausibility check, not validation.
* **Answer to the packet's decision question.** For *these sampled trees*, a plausible assumed forest **can materially alter** the competitive picture, but only for stems roughly within 5–7 m of an edge. Whether that matters depends on a premise this study cannot settle: "plot is a subset of a larger forest" (finite model understates competition at the edge) versus "isolated property with open land" (finite model is right). **A separate native witness is worth its cost only if #424/the owner declares the first premise**; otherwise stop and keep the small property as a declared bounded training abstraction (§5).
* **Robustness.** The one sensitivity (expected-value stocking instead of one thinning realisation) moves the near-edge contributions by 0.2 %–12 % (P0604, 3 stems at the cutoff margin, by −24 %), so the result is not an artefact of one random thinning pattern. Negative controls all pass (§4.3).

## 2. Evidence, data gate and stage

**Read-only evidence root** (never written; metadata of all 89 files and SHA-256 of the four key files were identical before and after reading):
`/media/jer/Files/CCF/Verification/CompactOneRackNativeFeasibility460-20261113T145748Z/native-20261113T152528Z/`

| File | SHA-256 | Check |
|---|---|---|
| `native-result.json` | `58524ad4f634378a875313909bb5cf42cfb37468abe0e60a61b63ce00a51de40` | = #460 report pin; = manifest |
| `executed-harness.cs` | `3c0ebae88448d27b0aae4d0345b37a28026f8c9cc3b417d13920288853019863` | = #460 report pin; = manifest |
| `C-A-20.json` | `13f91368d044de2771e56a85a5daaa038a7fe7e5a9cab80995fbe8d45151a9ee` | = manifest; equals its copy inside `native-result.json` |
| `C-B-30.json` | `3e7c37ad010d66382722bbd932db6c93b9de1b78914a38c48b4f1a7e1f9d5a27` | = manifest; equals its copy inside `native-result.json` |
| `evidence-manifest.json` | `cd35e159c0d306f5833104e697d9657b45956559d2f52d0f42ca8a6ace6838d7` | not self-listed |

`evidence-manifest.json` lists 62 files; **61 match on disk**. The one mismatch, `closure-receipt.json`, was written at 15:51:15 after the manifest (15:50:26) and is not an input. Per-file chain of custody and the 12 reconciliation checks per candidate are in `results/results.json` (`reconciliation`, `custody`); the checks include: rows = recorded `initial` = Year-0 living (82 / 189); unique IDs; all `sitka-spruce` age 20; recorded `edge` = `size/2 − max(|x|,|z|)`; all stems inside the square; and, from the hash-verified harness text, that the tree list is **living-only** (`Trees()` filters `IsLiving`), is captured by `MeasureInitial` **immediately after `Generate` and before access felling**, and takes `ci` from `eco.GetCompetitionIndex(t)`.

**Stage used for every number:** *Year 0, immediately after the unmodified `ForestStartingStand.Generate`, before the Year-1 access felling; all living, Sitka, age 20.* Nothing from Year 5/9 or stump lists is mixed in. The 13 / 19 corridor ("rack") stems are living at this stage and count as competitors. Candidate IDs overlap (`P0000` exists in both worlds with different coordinates), so every record is scoped by candidate.

**The #460 anchors match this stage.** Recorded CI: P0101 = 5.4487 and P0604 = 5.4590 (the #460 report's "near 5.45"); P0000 = 1.8469 and P1504 = 3.4087 (report: 1.85 / 3.41).

**Source locked to** `Assets/ForestPrototype/ForestEcologyController.cs` blob `0290a7be…` and `ForestStartingStand.cs` blob `29af0628…`, identical at #460's baseline `6c13a9e1` and at `main` `fc18190b`. Formula (lines 912, 923–926, 930–997): living targets only, neighbours in ordinal-`TreeId` order, `distance = Vector2.Distance`, skip if `distance > 8`, `ci += (dbh_j / max(1, dbh_i)) / max(0.5, distance)`.

## 3. Method

### 3.1 Measured versus assumed

| Quantity | Status |
|---|---|
| Tree IDs, species, X/Z, DBH, recorded CI, edge distance of the 82 and 189 stems | **MEASURED** — read-only from #460 native Unity output |
| Finite CI of each focal tree | **RECOMPUTED offline** from those positions/DBHs with the source formula; cross-checked against the recorded native CI |
| Every external stem (existence, position, species, age, DBH) | **HYPOTHETICAL** — invented by this study; no real neighbouring forest exists in any input |
| External stocking (2,100 stems/ha) | Source starting-stand target (`StandGeometryModel.StartingStemsPerHectare`), applied by assumption outside the property |
| Anything about canopy, light, growth, seed, wind | **NOT MEASURED, NOT MODELLED** |

### 3.2 Focal selection (predeclared; geometry only; never delta)

Fixed in code before any external contribution was computed: (1) the two #460 native crop anchors per candidate; (2) the retained stem (north/south of the access corridor) with the greatest edge distance; (3) the retained stem whose edge distance is nearest the median retained edge distance (tie: lowest ID). Four per candidate, **eight in total** — the packet maximum. Both 30 m anchors are boundary stems (P0000 is a corner stem; the source clamps stems to 0.6 m inside the boundary), so they bound the high end rather than representing a typical tree; the "median" stem is the typical case. No focal tree was chosen or dropped after seeing a delta.

### 3.3 The one hypothetical context

The source planting lattice (same origin `−span/2`, 1.9 m spacing, `x` from column and `z` from row) is extended outward to every node outside the property square. No jitter. A node is kept with probability `p = 2100 / (10000 / 1.9²) = 0.7581` by `murmur3_fmix32(FNV-1a(id))` (the same finaliser the source uses for its Enlarged80 outer ring; plain FNV was rejected because a unit test showed it kept 83 % of a ring instead of 76 %). All kept stems are Sitka, age 20, with DBH equal to the candidate's own Year-0 mean (**15.5696 cm** for 20 m, **15.4336 cm** for 30 m), IDs `X_r±RR_c±CC` (never a source `Pxxxx` ID). External stems per candidate: 576 / 768 nodes considered, 428 / 579 kept; 45 / 56 reach at least one focal tree. The 20 m lattice origin is −8.55 m and the 30 m origin is −14.25 m, so the first external row is 0.45 m / 1.15 m outside the boundary.

**Sensitivity S1 (the only one).** All nodes kept with fractional weight `p` — the realisation-free expected value of the same assumption.

**Linearity note (algebra, not a further run).** Each external term is proportional to the assumed external DBH, so ΔCI scales exactly with it: at half the assumed DBH the five near-edge ΔCI values would be 0.6–2.1 instead of 1.2–4.3.

## 4. Results

CI = Hegyi index at Year 0. *CI finite (recomputed)* uses only the actual on-property living stems. *CI context* = finite + hypothetical external terms. Ratio = context / finite. All values float64; the float32 emulation is in §4.2.

### 4.1 20 × 20 m (C-A-20: 82 living Sitka, 2,050 stems/ha, lattice 10 × 10)

| Focal | Side | X, Z (m) | DBH (cm) | Edge (m) | Finite nbrs | Recorded native CI | CI finite (recomputed) | Ext. stems in 8 m | External contribution | CI context | ΔCI | Ratio | S1 contribution |
|---|---|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| **P0101** | south | -7.252, -6.776 | 20.866 | 2.748 | 24 | 5.4487 | 5.4487 | 23 | 3.0490 | 8.4977 | 3.0490 | 1.560 | 3.0294 |
| **P0604** | north | -1.146, 2.928 | 19.250 | 7.072 | 34 | 5.4590 | 5.4590 | 3 | 0.3159 | 5.7749 | 0.3159 | 1.058 | 0.2395 |
| **P0504** | north | -0.621, 1.175 | 19.134 | 8.825 | 36 | 6.0594 | 6.0594 | 0 | 0.0000 | 6.0594 | 0.0000 | 1.000 | 0.0000 |
| **P0708** | north | 7.189, 5.439 | 12.984 | 2.811 | 22 | 6.9917 | 6.9917 | 19 | 3.8179 | 10.8096 | 3.8179 | 1.546 | 3.9630 |

Nearest hypothetical stem to each focal tree: P0101 3.72 m; P0604 7.52 m; P0504 none within 8 m; P0708 3.48 m.

### 4.2 30 × 30 m (C-B-30: 189 living Sitka, 2,100 stems/ha, lattice 16 × 16)

| Focal | Side | X, Z (m) | DBH (cm) | Edge (m) | Finite nbrs | Recorded native CI | CI finite (recomputed) | Ext. stems in 8 m | External contribution | CI context | ΔCI | Ratio | S1 contribution |
|---|---|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| **P0000** | south | -14.400, -14.400 | 22.212 | 0.600 | 10 | 1.8469 | 1.8469 | 29 | 4.2892 | 6.1360 | 4.2892 | 3.322 | 4.3481 |
| **P1504** | north | -6.864, 14.400 | 20.601 | 0.600 | 19 | 3.4087 | 3.4087 | 19 | 2.8817 | 6.2904 | 2.8817 | 1.845 | 2.8766 |
| **P0807** | north | -1.348, 1.175 | 18.237 | 13.652 | 35 | 6.0762 | 6.0762 | 0 | 0.0000 | 6.0762 | 0.0000 | 1.000 | 0.0000 |
| **P0209** | south | 3.231, -11.397 | 15.905 | 3.603 | 26 | 6.1785 | 6.1785 | 8 | 1.2469 | 7.4254 | 1.2469 | 1.202 | 1.3963 |

Nearest hypothetical stem to each focal tree: P0000 1.76 m; P1504 2.43 m; P0807 none within 8 m; P0209 4.99 m.

Every hypothetical stem that reaches a focal tree is listed in `results/external-stems.csv` with ID, coordinates, distance outside the property, assumed species/age/DBH, provenance label, distance to the focal tree and Hegyi term (101 candidate–stem–focal rows).

### 4.3 Precision, selection reasons and negative controls

20 × 20 m:

| Focal | Why selected | Float32-emulation CI finite | Float32-emulation CI context | abs(f64 − f32) context | abs(recomputed − recorded) f64 / f32 | Zero-external control (f64 / f32) | Source multiplier 1/(1+CI/5) finite → context |
|---|---|---:|---:|---:|---|---|---|
| P0101 | #460 native crop anchor | 5.4487 | 8.4977 | 7.0e-07 | 2.2e-07 / 0.0e+00 | PASS / PASS | 0.479 → 0.370 |
| P0604 | #460 native crop anchor | 5.4590 | 5.7749 | 2.4e-07 | 6.3e-08 / 0.0e+00 | PASS / PASS | 0.478 → 0.464 |
| P0504 | most interior retained stem (max edge distance) | 6.0594 | 6.0594 | 6.7e-07 | 6.7e-07 / 0.0e+00 | PASS / PASS | 0.452 → 0.452 |
| P0708 | retained stem nearest the median retained edge distance (2.811 m) | 6.9917 | 10.8096 | 1.9e-06 | 1.7e-07 / 4.8e-07 | PASS / PASS | 0.417 → 0.316 |

30 × 30 m:

| Focal | Why selected | Float32-emulation CI finite | Float32-emulation CI context | abs(f64 − f32) context | abs(recomputed − recorded) f64 / f32 | Zero-external control (f64 / f32) | Source multiplier 1/(1+CI/5) finite → context |
|---|---|---:|---:|---:|---|---|---|
| P0000 | #460 native crop anchor | 1.8469 | 6.1360 | 1.1e-07 | 6.7e-09 / 2.4e-07 | PASS / PASS | 0.730 → 0.449 |
| P1504 | #460 native crop anchor | 3.4087 | 6.2904 | 4.1e-07 | 7.9e-09 / 0.0e+00 | PASS / PASS | 0.595 → 0.443 |
| P0807 | most interior retained stem (max edge distance) | 6.0762 | 6.0762 | 1.5e-07 | 1.5e-07 / 0.0e+00 | PASS / PASS | 0.451 → 0.451 |
| P0209 | retained stem nearest the median retained edge distance (3.617 m) | 6.1785 | 7.4254 | 9.5e-07 | 6.7e-09 / 4.8e-07 | PASS / PASS | 0.447 → 0.402 |

Whole-stand recreation of the *recorded* finite CI (all stems, not only focal): C-A-20 — 82 stems; max abs(float64 − recorded) = 1.92e-06; max abs(float32 emulation − recorded) = 9.54e-07; float32 emulation bit-equal to recorded for 54/82. C-B-30 — 189 stems; max abs(float64 − recorded) = 2.39e-06; max abs(float32 emulation − recorded) = 1.91e-06; float32 emulation bit-equal to recorded for 112/189. So the offline arithmetic **approximates** the native values to about 2e-6 absolute and is bit-equal for 54/82 and 112/189 stems under the float32 emulation. **No claim of Unity bit-equality is made**; no native comparison was run. The float64 and float32 contextual CIs differ by < 2e-6, far below any decision-relevant scale.

Negative controls (all pass, also in the unit tests):

| Control | Result |
|---|---|
| Zero external stems → `CI_context == CI_finite` | PASS for all 8 focal trees in both float64 and float32 (bit-identical) |
| External stem 8.5 m from the only tree → no contribution | PASS (`negative_control_beyond_cutoff`: finite 0, context 0) |
| Cutoff semantics: 7.9999 m and exactly 8.0 m included, 8.0001 m excluded (source `distance > 8`) | PASS (unit test, both arithmetics) |
| Focal tree > 8 m from every external stem → exactly zero effect | PASS: P0504, P0807 (natural controls, ΔCI = 0 exactly) |
| No external ID equals any source ID; all external stems lie strictly outside the property | PASS (asserted per candidate at run time) |
| NaN / ±inf / non-positive DBH / duplicate or missing fields | rejected as DATA GATE errors (unit tests) |

### 4.4 Finite-forest context: CI by distance from an edge (recorded values, all stems)

20 × 20 m:

| Edge distance band (m) | Stems | Mean recorded finite CI |
|---|---:|---:|
| 0-2 | 29 | 4.919 |
| 2-4 | 25 | 6.906 |
| 4-8 | 25 | 7.842 |
| 8-inf | 3 | 6.644 |

30 × 30 m:

| Edge distance band (m) | Stems | Mean recorded finite CI |
|---|---:|---:|
| 0-2 | 70 | 6.165 |
| 2-4 | 39 | 7.747 |
| 4-8 | 48 | 8.707 |
| 8-inf | 32 | 7.349 |

In the finite forest, stems within 2 m of an edge have a clearly lower CI than inside-band stems (4.9 vs 6.9–7.8; 6.2 vs 7.7–8.7). This is the edge deficit the hypothetical context removes. Only 3 of 82 stems in the 20 m forest are 8 m or more from an edge (32 of 189 in the 30 m forest), so the 20 m "interior" is a very small group.

## 5. Interpretation and recommendation

1. **Sampled trees:** the effect is large for stems within about 4 m of an edge, small but non-zero near 7 m, and zero beyond about 8 m. It is a *geometric* consequence of an 8 m cutoff and a 20 m or 30 m property: #460 recorded 79/82 and 157/189 stems less than 8 m from an edge (a sampling aid, not a CI-error measure), so most stems in these properties are close enough to an edge for an external neighbour to matter *if one existed*.
2. **Two valid readings.** *Open land beyond the property:* finite CI is the correct boundary assumption and nothing here changes. *Property is a management subset of a larger forest:* the finite model omits real competitors for roughly the outer 5–7 m band, and the omission is not small relative to the CI scale. Two species cannot remove that edge/landscape-context issue by themselves.
3. **Mechanism gate — recommendation, not a decision.** The change is meaningful enough for these trees that it *could* alter a training explanation under the subset-of-a-forest reading. **If #424/the owner chooses that reading, preparing one separately authorised native Tier 2 witness is justified**, using real neighbouring trees in a new authorised world (not decorative meshes) and the same eight focal IDs, to test what the extra competition does to canopy/light/growth. **If the owner chooses the bounded-property reading, stop** and retain the small property as a declared training abstraction pending player-pedagogy evidence. This study authorises neither.
4. **Do not over-read the multiplier.** The source growth law multiplies potential DBH growth by `1/(1+CI/Ci50)` with Sitka `Ci50 = 5`; the table's last column shows how that factor would move (for example 0.730 → 0.449 for P0000). That is arithmetic of the existing multiplier for scale only; it is **not** a predicted DBH or height change, which also depends on DBH/Dmax, site, age, crown and canopy.

## 6. Unknowns and limitations

* **No real external forest exists in any input.** Species, age, DBH, spacing, gaps, roads, clearances and ownership beyond the boundary are all assumed. A mixed or older Oak/Beech surround would give different DBHs and stocking.
* **The on-property trees were generated for a finite stand.** Source DBH classes depend on lattice neighbour counts that treat the boundary as open (edge stems receive a "released" +1.5 cm class), so the focal DBH values would likely differ slightly if the stand had been generated inside a continuous forest. DBH was held fixed; this affects the target-DBH denominator only to second order.
* **External stems are unjittered lattice points** with uniform DBH; real neighbours would scatter around the lattice. S1 bounds the stocking-realisation effect but not DBH or spatial-pattern variation.
* **P0604 (7.07 m from an edge) is cutoff-limited:** its nearest hypothetical stem is 7.52 m, so three stems carry the whole result; treat +0.32 as "small and placement-dependent".
* **Eight stems are a probe, not a sample.** The two 30 m anchors are corner stems and exaggerate the typical effect; the 20 m anchors sit at 2.7 m and 7.1 m.
* **Not examined:** crown/canopy/light overlap across the boundary (#460 recorded potential crown crossing for 29/82 and 78/189 stems), seed supply (source kernels exceed these widths), wind and edge mortality, browsing, Year-25/40 behaviour, Oak/Beech growth under the shared 8 m radius (which has no broadleaf-specific evidence), the two-versus-three species question, and anything visual.
* **Hegyi arithmetic is approximate versus Unity** (≈ 2e-6 absolute); no native bit-comparison was run.

## 7. Reproduction and test matrix

Python 3.12.3, standard library only, no network.

```bash
python3 -I -B Tools/Research/ScenarioOneFocalBoundaryCI/focal_boundary_ci.py --out-dir <new scratch dir>
python3 -I -B -m unittest discover -s Tools/Research/ScenarioOneFocalBoundaryCI -p 'test_*.py' -v
```

Outputs `results.json`, `focal-rows.csv`, `external-stems.csv` are **byte-identical** to the committed `results/` copies and between three consecutive runs. SHA-256: `results.json` `915f9540117b69b9344af76e9352b1dcc04353a8503afceb9f36d171c4693a20`; `focal-rows.csv` `6f279f12668e867bff1e12b372077511b330de997fdcc58db96213c31ccaf9a8`; `external-stems.csv` `dd7148616e0b88e9b435bc816e51dddc87191e3dcbd807d92ebe69fe67ce21af`; script `focal_boundary_ci.py` `a93cf0fb3f71e0a60e88f2f59c03ac3c10d9f92774bb11d70054b68dba0667b2`.

| Check | Result |
|---|---|
| Unit tests (30: hand-calculated Hegyi term and floors, 8 m cutoff, zero-external, beyond-cutoff, pair exclusion, target/neighbour DBH direction, boundary classification, 20 m vs 30 m identity, external ID/outside invariants, thinning fraction, hash vectors, NaN/dup/missing guards, determinism) | **PASS 30/30** (28 pure, 2 against the real evidence) |
| Real-evidence run, exit 0, repeated ×3 byte-identical | **PASS** |
| Evidence custody (89-file metadata + 4 key-file hashes before = after) | **PASS** |
| Data gate: hashes, manifest, harness-stage reconciliation, recreation of recorded CI ≤ 1e-4 | **PASS** (measured ≤ 2.4e-6) |
| Zero-external negative control, 8/8 focal × 2 arithmetics | **PASS** |
| Hegyi bit-equality against the Unity engine | **NOT RUN** (no Unity by design) |
| Unity Editor / game / native witness / Year-25/40 simulation | **NOT RUN** (out of scope) |
| Protected Legacy/Reference replay hashes | **NOT RUN** |
| Dashboard / Drive mirrors | **NOT VERIFIED** |

*Development note (honest record).* The first full test run failed one of my own tests: the hash-thinned external ring kept 83 % of nodes against the 76 % target. I replaced plain FNV-1a with FNV-1a plus the source's own murmur3 finaliser **before computing any result**, and the test then passed (kept 74 % and 75 %). No parameter was changed after seeing a focal result.
