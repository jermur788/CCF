# MFM-11 — Calibration Impact Qualification — Handoff

**Claim:** #220  
**Task:** MFM-11 — Calibration Impact Qualification  
**Date:** 2026-11-01  
**Status:** COMPLETE  
**Verdict:** `MULTI-SCALE CALIBRATION QUALIFIED`

## Exact candidate / ancestry

- **Starting HEAD:** `3acdf9cb635cdca79606335801e97f9d7dcce8b0`
- **Branch:** `verify/mfm-11-calibration-impact-v1`
- **Worktree:** `/media/jer/ZX20/CCF-U1` (serialized Unity lane U1)
- **Candidate identity:** Accepted `6784ab17...` MFM-09/MFM-10 lineage plus tested #199 presentation/focus repair only.

## Claim / model / provider

- **Claim:** #220 (MFM-11 Calibration Impact Qualification)
- **Execution phases:** Offline (.NET SDK) + Unity (serialized lock)

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
| Species split | 1177/0/0 | 1093/0/0 | 1168/0/0 |
| Provenance orders | 0 | 84 | 0 |
| Manual marks | 0 | 0 | 9 |

**B vs A (wider treatment impact):** −84 stems, −4.80 m² BA (after growth), −182,544 cash  
**C vs B (manual vs prescription):** +75 stems, +1.92 m² BA, −63,257 cash

### Per-unit comparison (offline, frozen MFM-03 fixture)

| Unit | A-stems | A-baM² | B-stems | B-baM² | C-stems | C-baM² |
|------|---------|--------|---------|--------|---------|--------|
| R1C | 0 | 0 | 4 | 0.094 | 2 | 0.027 |
| R1H | 0 | 0 | 6 | 0.107 | 3 | 0.044 |
| R2C | 2 | 0.036 | 7 | 0.133 | 5 | 0.067 |
| R2H | 0 | 0 | 7 | 0.154 | 4 | 0.050 |
| R2L | 0 | 0 | 7 | 0.140 | 5 | 0.063 |
| R3H | 3 | 0.055 | 13 | 0.271 | 8 | 0.110 |
| R3L | 0 | 0 | 8 | 0.192 | 5 | 0.057 |
| R4C | 1 | 0.014 | 6 | 0.149 | 4 | 0.046 |
| R4H | 2 | 0.036 | 7 | 0.120 | 5 | 0.066 |
| R4L | 1 | 0.016 | 8 | 0.169 | 4 | 0.048 |
| R5C | 0 | 0 | 3 | 0.069 | 3 | 0.030 |
| R5H | 0 | 0 | 9 | 0.179 | 6 | 0.063 |
| R5L | 0 | 0 | 8 | 0.225 | 6 | 0.057 |
| sample | 25 | 0.363 | 25 | 0.363 | 25 | 0.363 |
| **Total** | **34** | **0.521** | **118** | **2.366** | **85** | **1.091** |

## Safety assertions

| Assertion | Result | Evidence |
|-----------|--------|----------|
| Per-unit BA safeguard ≤40% | PASS (all paths) | Offline: `safeguard: path A/B/C per-unit BA share ≤ 40%` |
| Wider treatment retains ≥50% stand BA | PASS (B=89.8%, C=95.3%) | Offline: `canopy: prescription/manual retains ≥50%` |
| MFM-10 species guard green | PASS (single-species fixture) | Offline: `species-guard: fixture contains no non-Sitka stems` |
| Calibration version accepted | PASS (MFM03-CAL-1) | Offline: `species-guard: calibration version is accepted MFM-03-CAL-1` |
| CI parity within 20% (GAMEPLAY CALIBRATION) | PASS (4.17% delta) | Unity: `mfm03: CI parity within 20%` |
| Growth drift negligible | PASS (0.96 cm over 2 years) | Unity: `mfm03-growth-drift: meanDbh=16.369371` |
| Economy viable through existing rules | PASS (cash non-negative, no bypass) | Unity: `path-B/C: cash non-negative` |
| Save/load continuation | PASS (WorldHash preserved) | Unity: `save/load preserves world hash: FA86A3D3582FD1FB` |
| Protected hashes computed | PASS (WorldHash=EB80755047AB0606) | Unity: `protected: WorldHash=EB80755047AB0606` |
| Determinism (offline) | PASS (byte-identical) | Offline: `MFM11_REPEAT_PASS two processes, byte-identical output` |
| No test weakening or rebaseline | PASS | All checks are new; no existing tests modified |
| Harness staged/removed cleanly | PASS | Unity: `harness_removed=true, licence_copy_removed=true` |
| Host saves unchanged | PASS | Unity: `host_saves_unchanged=true` |
| Worktree clean after run | PASS | Unity: `remaining_lane_changes=""` |

## MFM-03 live/cross-year residual — resolved

| Residual | Result | Evidence |
|----------|--------|----------|
| Growth drift | 0.96 cm (15.40→16.37 over 2 years) | Unity: `mfm03-growth-drift` |
| Live competition-index parity | 4.17% delta (within 20%) | Unity: `mfm03-ci-parity: source=8.059146 covered=7.723093` |
| Whole-stand state | 1,177 living, 25.47 m² BA, mean DBH 16.37 cm | Unity: `mfm03-whole-stand` |
| Natural cohorts/juveniles/shelters/deadwood/marks | 0 juveniles, 0 shelters, 1 deadwood, 0 marks (year 2, expected) | Unity: `mfm03-live-state` |

**N1–N6 caveats carried:** The 20% CI tolerance is GAMEPLAY CALIBRATION, not empirical forestry. Simple SE separation is not independent-spatial proof. Boundary/tolerance behaviour is not empirical precision. The offline fixture is same-year; Unity confirms cross-year drift is negligible (0.96 cm over 2 years).

## Canopy evidence

- Mean canopy: 0.970 (authoritative model)
- Mean light: 0.030
- Path B retains 89.8% of stand BA (well above 50% threshold)
- Path C retains 95.3% of stand BA

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
  --head "72972cc45a95ecd7fa9a14301f5551f447a0de50" \
  --eta "2026-11-02T02:00:00Z" \
  --notes "MFM-11 #220 Unity matrix; exact candidate 3acdf9cb; verification only"

# Gate run
python3 Tools/Verification/Mfm11CalibrationImpact/run_gate.py \
  --gate calibration \
  --lock-id "cc3d8b74e4854ab7b08988db2f5945a4" \
  --owner "Mimo-MFM-11" \
  --expected-head "72972cc45a95ecd7fa9a14301f5551f447a0de50" \
  --out /media/jer/Files/CCF/Verification/MFM11-CalibrationImpact-20261101/unity/run1

# Lock released
/media/jer/ZX20/CCF-Ops/scripts/unity-lock release \
  --owner "Mimo-MFM-11" --task-id "MFM-11" \
  --lock-id "cc3d8b74e4854ab7b08988db2f5945a4"
```

## Evidence hashes

| File | SHA-256 |
|------|---------|
| `offline/run1.log` | `1e711b22b0437840ac013584eb0473744db744f568db2d3ecd52d484269567de` |
| `offline/run2.log` | `1e711b22b0437840ac013584eb0473744db744f568db2d3ecd52d484269567de` (identical) |
| `offline/fixture-stand.csv` | `4b4b9703e65fe01146e57deb2095ea4f2cf3b702243c73075cdc2bf52f7f2433` |
| `offline/fixture-infrastructure.json` | `c7dd5b811c975c6bf6cd50a201308424c4f2eed6b9dca30f80300dc885644630` |
| `unity/run1/unity.log` | `ea0477302396dbd1b81b8f173f2dce9c955c9fcf92773950aa38574324a3adf3` |
| `unity/run1/process-result.json` | `20561231fe3e31190c6843f675dcd35e1321c6a9c97f5e358d5f442217ebffee` |
| `unity/run1/evidence/comparison-table.txt` | `9baed4337e463416e0718b913342dc02e83de45e59458f1f93dbb561ae697c88` |

## Unity runtime receipts

| Receipt | Value |
|---------|-------|
| Unity exit code | 0 |
| Pass marker | `MFM11_UNITY_PASS checks=54 shots=2` |
| Failures | none |
| Duration | 130.7 seconds |
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

**`MULTI-SCALE CALIBRATION QUALIFIED`**

The wider default treatment path (Path B) does not materially break accepted Scenario One outcomes:
- Per-unit basal-area safeguard respected (all units ≤40%)
- Canopy objectives not undermined (89.8% BA retained)
- Economy viable through existing rules (cash non-negative, no bypass)
- Timber/deadwood outcomes coherent (1 deadwood record per path)
- Long-run Scenario One completion reachable (Path B reduces stems as expected)
- MFM-10 species guard remains green (no Sitka→Oak/Beech transfer)
- Protected hashes/Legacy40/Reference intact (WorldHash computed, save/load preserved)
- No test weakening or rebaseline
- MFM-03 live residual resolved (CI parity 4.17%, growth drift 0.96 cm)

**Evidence labels:** Direct model outputs (stems, BA, cash, deadwood, canopy), derived metrics (species splits, BA deltas), GAMEPLAY CALIBRATION (20% CI tolerance, 40% BA ceiling, 30% release share). No empirical forestry claims made.
