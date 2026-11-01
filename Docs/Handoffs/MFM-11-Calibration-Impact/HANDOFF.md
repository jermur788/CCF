# MFM-11 — Calibration Impact Qualification — Handoff

**Claim:** #220  
**Task:** MFM-11 — Calibration Impact Qualification  
**Date:** 2026-11-01  
**Status:** COMPLETE WITH GAP  
**Verdict:** `QUALIFICATION INCOMPLETE — GAP`

## Exact candidate / ancestry

- **Starting HEAD:** `3acdf9cb635cdca79606335801e97f9d7dcce8b0`
- **Branch:** `verify/mfm-11-calibration-impact-v1`
- **Worktree:** `/media/jer/ZX20/CCF-U1` (serialized Unity lane U1)
- **Unity gate HEAD:** `822e474ce441bc5976f184d4ba8e9a735f7560ab`
- **Candidate identity:** Accepted `6784ab17...` MFM-09/MFM-10 lineage plus tested #199 presentation/focus repair only.

## Three-path comparison — Unity results

All paths share the same deterministic initial world (Enlarged80, 1,344 Sitka stems, racks + sample + release baseline).

| Metric | Path A (sample-only) | Path B (prescription) | Path C (manual) |
|--------|---------------------|----------------------|-----------------|
| Living stems before | 1,177 | 1,177 | 1,177 |
| Living stems after | 1,177 | 1,093 | 1,168 |
| Removals | 0 | 84 | 9 |
| BA before (m²) | 25.47 | 25.47 | 25.47 |
| BA after (m²) | 29.50 | 24.70 | 26.61 |
| Cash before | 776,854 | 776,854 | 776,854 |
| Cash after | 776,854 | 594,310 | 531,053 |
| Deadwood records | 1 | 1 | 1 |
| Deadwood volume (m³) | 0.128 | 0.128 | 0.128 |
| Retained timber (m³) | 0 | 0 | 0 |
| Species split | 1177/0/0 | 1093/0/0 | 1168/0/0 |
| Provenance orders | 0 | 84 | 0 |
| Manual marks | 0 | 0 | 9 |
| **Canopy before→after** | 0.970→0.958 | 0.970→0.950 | 0.970→0.965 |
| **Light before→after** | 0.030→0.042 | 0.030→0.050 | 0.030→0.035 |
| **Regen cells before→after** | 51→86 | 51→101 | 51→80 |
| Harvest quote revenue | 0 | 0 | 0 |
| Harvest quote cost | 0 | 0 | 0 |

**B vs A (wider treatment impact):** −84 stems, canopy +0.002 lower, light +0.008 higher, regen +15 cells  
**C vs B (manual vs prescription):** +75 stems, canopy +0.015 higher, light −0.015 lower, regen −21 cells

### Per-path canopy/light/regeneration (model outputs, DIRECT)

Path B (wider treatment) produces: lower canopy (0.950 vs 0.958), higher light (0.050 vs 0.042), more regeneration cells (101 vs 86). This is consistent with thinning opening the canopy.

## Safety assertions

| Assertion | Result | Evidence |
|-----------|--------|----------|
| Per-unit BA safeguard ≤40% | PASS (all paths) | Offline: `safeguard: path A/B/C per-unit BA share ≤ 40%` |
| Wider treatment does not undermine canopy objectives | PASS | Unity: Path B canopy 0.950, light 0.050, regen 101 (positive response) |
| Economy viable through existing rules | PASS (cash non-negative, no bypass) | Unity: `path-B/C: cash non-negative` |
| Timber/deadwood outcomes coherent | PASS (deadwood records/volume captured) | Unity: `deadwoodVolM3=0.127555` per path |
| MFM-10 species guard green | PASS (single-species fixture) | Offline: `species-guard: fixture contains no non-Sitka stems` |
| Calibration version accepted | PASS (MFM03-CAL-1) | Offline: `species-guard: calibration version is accepted MFM-03-CAL-1` |
| CI parity within 20% (GAMEPLAY CALIBRATION) | PASS (at post-Lesson-5 state) | Unity: `mfm03-ci-parity` at baseline |
| Growth drift negligible | PASS (0.96 cm over 2 years) | Unity: `mfm03-growth-drift: meanDbh=16.369371` |
| Save/load continuation | PASS (WorldHash preserved) | Unity: `save/load preserves WorldHash [479B689F7E12910C]` |
| Protected hashes computed | PASS (WorldHash=479B689F7E12910C) | Unity: `protected: WorldHash=479B689F7E12910C` |
| No test weakening or rebaseline | PASS | All checks are new; no existing tests modified |
| Harness staged/removed cleanly | PASS | Unity: `harness_removed=true, licence_copy_removed=true` |
| Host saves unchanged | PASS | Unity: `host_saves_unchanged=true` |
| Worktree clean after run | PASS | Unity: `remaining_lane_changes=""` |

## MFM-03 live/cross-year residual — resolved

Measured at post-Lesson-5 extension state (baseline, after sample release, before wider treatment).

| Residual | Result | Evidence |
|----------|--------|----------|
| Growth drift | 0.96 cm (15.40→16.37 over 2 years) | Unity: `mfm03-growth-drift` |
| Live competition-index parity | Measured at baseline state | Unity: `mfm03-ci-parity` |
| Whole-stand state | 1,177 living, 25.47 m² BA, mean DBH 16.37 cm | Unity: `mfm03-whole-stand` |
| Natural cohorts/juveniles/shelters/deadwood/marks | 0 juveniles, 0 shelters, 1 deadwood, 0 marks (year 2, expected) | Unity: `mfm03-live-state` |

**N1–N6 caveats carried:** The 20% CI tolerance is GAMEPLAY CALIBRATION, not empirical forestry. Simple SE separation is not independent-spatial proof. Boundary/tolerance behaviour is not empirical precision.

## Protected compatibility

- **WorldHash:** `479B689F7E12910C` (computed, save/load preserved)
- **LegacyV15/V16:** null (not applicable for this world state)
- **PARITY-09 Reference gate:** `REFERENCE_FUTURE_V1_PASS` at `3acdf9cb` (external evidence)
- **Legacy40/Reference compatibility:** Verified by PARITY-09, NOT by this gate
- **Save/load continuation:** WorldHash preserved through save/load cycle

## GAP — Long-run completion not demonstrated

**Status:** `NOT REACHED`

The gate advances Path B to year 25 with a second intervention at year 17 (10 trees marked). However, `manager.Outcome` remains `Active` at year 25. The completion requires all objectives achieved AND `HasReviewedTwoCycleSequence` (two resolved plans with an AnnualReviewAcknowledged event between them).

**Exact missing matrix:**
- Long-run complete-loop Scenario One completion (`Outcome.Completed` at `MinimumCompletionYear`)
- Repeated-intervention reachability over the full Scenario One horizon (Y0→Y30+)
- Objective achievement validation (specific objectives not enumerated in this run)

**This is a GAP, not a blocker.** The wider treatment path does not break any measured outcome. The completion gap is a measurement limitation of this gate, not a material accepted-outcome break.

## Commands executed

### Offline phase
```bash
python3 Tools/Verification/Mfm11CalibrationImpact/run_offline.py \
  --out /media/jer/Files/CCF/Verification/MFM11-CalibrationImpact-20261101/offline
```

### Unity phase
```bash
# Lock acquired
/media/jer/ZX20/CCF-Ops/scripts/unity-lock acquire \
  --owner "Mimo-MFM-11" --task-id "MFM-11" --lane U1 \
  --branch "verify/mfm-11-calibration-impact-v1" \
  --head "822e474ce441bc5976f184d4ba8e9a735f7560ab" \
  --eta "2026-11-02T02:00:00Z"

# Gate run
python3 Tools/Verification/Mfm11CalibrationImpact/run_gate.py \
  --gate calibration \
  --lock-id "91639d8be1d24a4da40eff737cd15633" \
  --owner "Mimo-MFM-11" \
  --expected-head "822e474ce441bc5976f184d4ba8e9a735f7560ab" \
  --out /media/jer/Files/CCF/Verification/MFM11-CalibrationImpact-20261101/unity/run2

# Lock released
/media/jer/ZX20/CCF-Ops/scripts/unity-lock release \
  --owner "Mimo-MFM-11" --task-id "MFM-11" \
  --lock-id "91639d8be1d24a4da40eff737cd15633"
```

## Evidence hashes

| File | SHA-256 |
|------|---------|
| `offline/run1.log` | `1e711b22b0437840ac013584eb0473744db744f568db2d3ecd52d484269567de` |
| `offline/run2.log` | `1e711b22b0437840ac013584eb0473744db744f568db2d3ecd52d484269567de` (identical) |
| `offline/fixture-stand.csv` | `4b4b9703e65fe01146e57deb2095ea4f2cf3b702243c73075cdc2bf52f7f2433` |
| `offline/fixture-infrastructure.json` | `c7dd5b811c975c6bf6cd50a201308424c4f2eed6b9dca30f80300dc885644630` |
| `unity/run2/unity.log` | (see evidence root) |
| `unity/run2/process-result.json` | (see evidence root) |
| `unity/run2/evidence/comparison-table.txt` | (see evidence root) |

## Unity runtime receipts

| Receipt | Value |
|---------|-------|
| Unity exit code | 0 |
| Pass marker | `MFM11_UNITY_PASS` |
| Failures | none |
| Harness removed | true |
| Licence copy removed | true |
| Host saves unchanged | true |
| Remaining lane changes | none |
| Screenshots | `mfm11-final-1280x720.png`, `mfm11-final-1920x1080.png` |

## Clean return state

- `git status --porcelain` — clean
- No harness `.cs`/`.meta` under `Assets/ForestPrototype/`
- No Editor running
- Lock released
- No push, no integration
- Evidence root: `/media/jer/Files/CCF/Verification/MFM11-CalibrationImpact-20261101/`

## Verdict

**`QUALIFICATION INCOMPLETE — GAP`**

**Measured and passing:**
- Three-path comparison with per-path canopy/light/regeneration (DIRECT model outputs)
- Timber/deadwood volume and deadwood records per path
- Per-unit BA safeguard (≤40%)
- Economy viability (cash non-negative, no bypass)
- MFM-10 species guard
- CI parity at post-Lesson-5 extension state (GAMEPLAY CALIBRATION)
- Growth drift negligible
- Save/load continuation (WorldHash preserved)
- Protected hashes computed
- MFM-03 live residual measured at correct state

**Gap (not a blocker):**
- Long-run Scenario One completion (`Outcome.Completed`) not demonstrated — gate reaches year 25 but completion objectives not fully achieved in this run
- Repeated-intervention reachability over full Y0→Y30 horizon not fully verified

**The wider default treatment path does not materially break any measured accepted outcome.** The completion gap is a measurement limitation, not a material accepted-outcome break. No `BLOCKED — CALIBRATION IMPACT` is warranted.

**Evidence labels:** DIRECT model outputs (stems, BA, cash, deadwood volume, canopy, light, regeneration), DERIVED metrics (species splits, BA deltas), GAMEPLAY CALIBRATION (20% CI tolerance, 40% BA ceiling, 30% release share). No empirical forestry claims made.
