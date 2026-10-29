# PARITY-03 Q10 — HANDOFF

**Result token:** `PARITY-03 Q10 STATIC EVIDENCE STRENGTHENED / CONTROLLER UNITY MEASUREMENT WAITING — RACK UNITY PRIORITY`

The full-success token (`PARITY-03 DENSITY CONTINUITY EVIDENCE STRENGTHENED — READY FOR MANAGER REVIEW`) is **not** claimed: the controller-level measurement is written and type-checked but has **not been run in Unity**.

## Identity and repository

| | |
|---|---|
| Claim | #74 (reused; no duplicate) |
| Model / provider | Claude Code, Claude Sonnet 5.5 (`claude-sonnet-5-5`), Anthropic |
| Branch / worktree | `verify/parity-03-density-continuity-q10-v1` / `/media/jer/ZX20/CCF-Q10` (clean, new) |
| Starting HEAD | `c5a269004a6b1efc3b6ffb134ca20070785f8e5e` (`origin/main`, descendant of the Q11 base `08361f0`) |
| `origin/main` since | advanced to `6c18f64` by docs-only commits; none touch this surface. Not merged into the branch. |
| Ending SHA | the commit that adds this file (see claim #74 comment / `git log -1`) |
| Durable evidence | `/media/jer/Files/CCF/Verification/PARITY03-Q10-20261029/` |

## Changed files (verification only)

1. `Tools/Verification/ThreeSpecies/OfflineLogic/CompositeDensityOfflineCheck.cs` — adds the Q10 section; the original continuity code is **untouched**.
2. `Tools/Verification/ThreeSpecies/MixedDensityVerification.cs` — adds the `SeamTransition` controller-level stage (not yet run).
3. `Tools/Verification/ThreeSpecies/OfflineLogic/run_composite_mutations.py` — 10 new mutants, `--check-file` option, and mutation of the check file itself for fixture-defect mutants.
4. `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-03-Q10/HANDOFF.md` — this file.

No file under `Assets/ForestPrototype/` changed. No species asset/YAML, scene, prefab, save/schema or calibration value changed. No Unity run, no Windows build, no Unity lock taken (lock was free when checked, but the queue still names #49 as priority and the Manager instruction was to wait).

## Offline results

Command: `python3 -I Tools/Verification/ThreeSpecies/OfflineLogic/run_offline_check.py --suite density`

| Suite | Before (c5a2690) | After |
|---|---|---|
| `density-composite` | 107 checks, 3 failures | **124 checks, 3 failures** (+17 new, all PASS) |
| `density` (MixedDensityOfflineCheck) | 91 checks, 1 failure | 91 checks, 1 failure (unchanged file) |
| `density-isolated`, `density-sitka-isolated` | PASS | PASS |

**Old continuity assertions:** all 107 original check names produce identical result *and* identical detail text before/after (C1 worst 0.0008, C2 worst 0.0005, adjacent-step margin 2.80, I1 draws=2400, NO_ROUTE_CLIFF PASS), and the original `CONTINUITY_TABLE` output is byte-identical. Nothing was weakened or rebaselined.

**Pre-existing failures, reported separately, not rebaselined** (identical on pristine `origin/main` `6c18f64`):

- `LONG_RUN_L40_B_BEECH_DENSE`, `LONG_RUN_L40_F_BEECH_YOUNG`, `LONG_RUN_E80_F_BEECH_YOUNG` — pure-Beech R2 (single-year loss 0.423 / 0.407 / 0.417 vs 0.40).
- `NO_PRODUCTION_IDENTIFIER_OR_STRING_EXPRESSES_A_SPECIES_MAXIMUM_DENSITY_OR_MAX_BASAL_AREA` — production identifier `MaxRemovedBasalShare` in `ScenarioOneRackBay.cs` (Rack production file). Unrelated to Q10; flagged for the Manager because the `density` suite is therefore not fully green on `main`.

## What the new evidence is

Geometry: Legacy40 **0.16 ha** and Enlarged80 **0.64 ha**, parsed from `StandGeometryModel.cs` / `ForestEcologyController.cs` by `Q10_GEOMETRY_AREAS_EQUAL_THE_PRODUCTION_SOURCES` (not assumed).
Classes: Intermediate (Beech) and High (Oak), DBH and competition index unchanged from the replaced Sitka stem.
Fixtures: deterministic closed-form sequences (no RNG), three heterogeneous Sitka structures at Sitka RD 0.72 / 0.80 / 0.88 (pure annual loss ≈ 0.7 % / 1.9 % / 3.9 %), stem count following area (Legacy40 N = 203/226/249; Enlarged80 N = 818/910/1001). DBH CV 0.22, CI CV ≈ 0.72, corr(DBH, CI) = −0.87, every CI distinct. Mean competition index of victims is 1.83–1.87× the stand mean (the old homogeneous fixtures give 1.00 by construction).
Seam: pure state on the **exact legacy block**; seam state on the species-aware route with exactly one stem replaced at four distinct positions (highest CI, median CI, lowest CI = largest DBH, smallest DBH), then 1 / 5 / 10 % stride replacement. Enlarged80 RD 0.88 (N = 1001) reproduces the original 1 / 10 / 50 / 100 ladder exactly. 84 states × 400 yearly keyed rolls.

### Numerical seam measurements (k = 1, worst over four positions)

| Geometry | Structure | Class | N | T_pure | worst abs Δloss | worst relative | worst ΔE[deaths]/yr |
|---|---|---|---|---|---|---|---|
| Legacy40 | LowLoss | Beech | 203 | 0.00733 | 0.00026 | 3.53 % | 0.053 |
| Legacy40 | LowLoss | Oak | 203 | 0.00733 | 0.00010 | 1.34 % | 0.020 |
| Legacy40 | Moderate | Beech / Oak | 226 | 0.01897 | 0.00030 / 0.00022 | 1.57 / 1.17 % | 0.067 / 0.050 |
| Legacy40 | Higher | Beech / Oak | 249 | 0.03849 | 0.00025 / 0.00037 | 0.65 / 0.97 % | 0.063 / 0.092 |
| Enlarged80 | LowLoss | Beech / Oak | 818 | 0.00710 | 0.00012 / 0.00009 | 1.72 / 1.21 % | 0.100 / 0.070 |
| Enlarged80 | Moderate | Beech / Oak | 910 | 0.01975 | 0.00005 / 0.00007 | 0.24 / 0.38 % | 0.043 / 0.067 |
| Enlarged80 | Higher | Beech / Oak | 1001 | 0.03879 | 0.00012 / 0.00007 | 0.30 / 0.17 % | 0.118 / 0.068 |

Overall: 48 seam states, worst 0.118 expected deaths/yr, 0.00037 absolute loss, 3.53 % relative.

Exact accounting at every state: Sitka-component victims equal the literal port of the original controller block on the Sitka stems alone for **33,600 / 33,600 draws**; the seam movement decomposes exactly into (the protected legacy block's own response to losing a Sitka stem) + (the generic term), and the generic term matches an independent `GenericLoss(occupancy) × non-Reference share × stems left` expectation within 2.0 standard errors worst case.

### Tolerances (VERIFICATION TOLERANCE / GAMEPLAY CALIBRATION — numerical continuity, **not forestry evidence**)

| Bound | Value | Worst measured | Margin |
|---|---|---|---|
| Seam absolute | ≤ 0.5 expected deaths/yr (half a stem) | 0.118 | 4.3× |
| Seam relative | ≤ 10 % of the pure annual deaths | 3.53 % | 2.8× |
| Ladder (k ≥ 2) | ≤ 0.25 stems per replaced stem + 0.5 | 0.71 of allowance | 1.41× |

The old absolute tolerance 0.005 is 68 % of the Legacy40 low-loss annual loss; the new half-stem bound is 2× (Legacy40) to 10× (Enlarged80) tighter in loss units and scale with N. A synthetic +0.004 loss jump (below 0.005) was accepted by the old tolerance in 48/48 seam states and is rejected by the new criteria in 48/48. The ladder bound is a regression guard set about 1.3× above the largest accepted-code per-stem movement (0.186); it is not a physical law, and a later legitimate recalibration of the generic response would need Manager re-approval of it. No bound was widened to absorb noise: the only random term is one quota draw per year (standard error ≤ 0.025 stems).

### Determinism

Closed-form fixtures and keyed rolls only. `Q10_REPLAY_IS_BIT_EXACT` regenerates six fixtures from scratch. The 84-row measurement digest is identical across four independent processes:
`Q10_MEASUREMENT_DIGEST sha256=e1a5a66a181967d7d3e882287d2c288d07449c56c75df5282a608335407cd76b`.

## Mutation / negative-control evidence

Final candidate (file hashes in the evidence directory), control carries only the 3 known R2 failures:

- **Original 18 mutants: 18 / 18 killed.**
- **New 10 Q10 mutants: 10 / 10 killed.**

Pre-Q10 suite (file from `origin/main`) against the four new production seam mutants:

| Mutant | Pre-Q10 suite | Q10 suite |
|---|---|---|
| `q10-lone-non-reference-stem-shielded` (first broadleaf never touched by generic) | **SURVIVED** | killed by generic-expectation, seam-absolute, seam-relative, synthetic-jump |
| `q10-lone-non-reference-stem-double-expectation` | **SURVIVED** | killed by generic-expectation, seam-absolute, synthetic-jump |
| `q10-seam-offset-0.4-percent` (flat +0.004 loss at the seam) | killed **incidentally** — only by the near-limit long-run `E80_B_BEECH_DENSE` R2 check; C1/C2/NO_ROUTE_CLIFF all still passed | killed by 7 assertions incl. seam-absolute, seam-relative, ladder, generic-expectation |
| `q10-sitka-subset-keeps-the-broadleaf-in-the-denominator` | killed (I1/I2/C3) | killed (same, plus Q10 exactness) |

(I had expected the 0.4 % offset to survive the old suite; it did not, but the old *continuity* assertions did not detect it.)

Fixture / criteria defects (mutating the check itself), all killed: competition index homogenised (`FIXTURES_ARE_HETEROGENEOUS`, `COMPETITION_INDEX_DECIDES_WHO_DIES`), DBH homogenised, Enlarged80 dropped, Beech-class replacement dropped (`COVERAGE`), stale Enlarged80 area (`GEOMETRY_AREAS_EQUAL_THE_PRODUCTION_SOURCES`), seam tolerance relaxed to nothing (`THE_CRITERIA_REJECT_A_SYNTHETIC_0_004_LOSS_JUMP`).

## Controller-level measurement — status

**Written, type-checked, NOT run, NOT verified.** Waiting behind RACK-V2-08 claim #49 (no Unity lock taken).

`MixedDensityVerificationRunner.SeamTransition` builds the existing grid stand pure Sitka and with exactly one Oak or one Beech (positions 0, N/2, N−1) on whichever geometry the run selects (`CCF_STAND_GEOMETRY`), stem count chosen so Sitka RD ≈ 0.78. It asserts: pure stand → `LegacySitkaExact`; one broadleaf → `SpeciesAwareOccupancy`; per-seed (6 seeds) realised deaths, Sitka-component deaths and generic deaths **equal** the production arithmetic applied to the controller's own stems and rolls; the Sitka component equals the original process on the Sitka stems alone; and the **expected** annual deaths (noise-free) are continuous across the 0 → 1 transition under the same 0.5-stem / 10 % bounds. Realised means over 6 paired draws are logged but not gated. No deaths are fabricated and no production state is manipulated; it uses the existing `Stand()` / `DensityStep()` machinery.

**Static prediction the Unity run must reproduce** (printed by the offline suite as `Q10_CONTROLLER_PREDICTION`, built with the production Hegyi on the identical grid): Legacy40 N = 170, pure expected deaths 2.707, seam 2.657–2.683 (Δ −0.023 to −0.049, −0.86 % to −1.83 %); Enlarged80 N = 682, pure 10.990, seam 10.916–10.963 (Δ −0.026 to −0.074, −0.24 % to −0.67 %). Worst 0.074 stems/yr against the 0.5 bound.

Type-check: `CloudTypeCheck/run_typecheck.py --mode tools` cannot prove these bodies (stale `baseline-tools.txt`, pre-existing declaration errors block body binding — known blind spot). Instead a focused project outside the repo (all `Assets/` sources, my harness only, a throwaway `Painter2D` stub) reports **0 errors in `MixedDensityVerification.cs`**, and a deliberately injected error inside `SeamTransition` **is** reported (negative control). Both in the evidence directory. This is a 2021.3 API approximation, not Unity's compiler.

Run when released: copy `MixedDensityVerification.cs` into `Assets/ForestPrototype/`, run with `-batchmode -nographics -executeMethod MixedDensityVerification.Begin` for `CCF_STAND_GEOMETRY=0` and `=1`, remove the copy and its `.meta`, and confirm the three `Q10_CONTROLLER_*` passes per species plus the previously accepted PARITY-03 checks. Use the exact candidate commit and `unity-lock` in the normal way.

## Observations for the Manager (measurement only; no production change authorised or made)

1. **No seam discontinuity found.** No stop condition triggered; pure-Sitka behaviour is bit-exact.
2. At larger replacement in a **low-loss** stand the mixed-route loss rises steeply in relative terms: 10 % High-class replacement at RD 0.72 gives +91 % (Legacy40) / +113 % (Enlarged80) relative, +14 % / +20 % for Intermediate. It is smooth in composition (within the ladder bound) and arises from the generic response being much stronger than the Sitka hazard at the same occupancy (≈ 1.2–1.3). It is a property of the accepted model, not a seam cliff; recorded for awareness only.
3. First guess for the "competition index decides who dies" bound (2.0) was arbitrary and failed against the measured 1.85; it was reset to 1.5 (anything clearly above the homogeneous 1.00) and is labelled as such in the code.
4. The `density` suite is red on `main` for `MaxRemovedBasalShare` (see above).

## Repository cleanliness

Only the four files above are changed on the branch; no `.meta`, no generated file, nothing written under `Assets/`; scratch work was in `/tmp`, evidence in the Files tree. The dirty `/media/jer/ZX20/CCF-Primary` checkout was not touched.
