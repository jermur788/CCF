# MFM-11 — Calibration Impact Qualification — Handoff

**Claim:** #220  
**Task:** MFM-11 — Calibration Impact Qualification  
**Date:** 2026-11-01  
**Status:** OFFLINE PHASE COMPLETE — UNITY PHASE PENDING  
**Verdict:** NOT YET DETERMINED — awaiting Unity/runtime matrix

## Exact candidate / ancestry

- **Starting HEAD:** `3acdf9cb635cdca79606335801e97f9d7dcce8b0`
- **Branch:** `verify/mfm-11-calibration-impact-v1`
- **Worktree:** `/media/jer/Files/CCF/Operations/CCF-MFM11Calibration-20261101`
- **Candidate identity:** Accepted `6784ab17...` MFM-09/MFM-10 lineage plus tested #199 presentation/focus repair only.

## Claim / model / provider

- **Claim:** #220 (MFM-11 Calibration Impact Qualification)
- **Execution phase:** Offline only (plain .NET SDK, no Unity, no machine-wide lock)
- **PARITY-09 #219** currently holds the serialized Unity lane; Unity phase deferred

## Three paths compared

All paths share the same deterministic fixture (MFM-03 frozen Enlarged80 stand, 1,344 Sitka stems, pinned SHA-256).

| Path | Description | Removals | BA removed (m²) | BA retained |
|------|-------------|----------|-----------------|-------------|
| A — sample-only | Historical teaching/sample release (3 sample crops) | 34 stems | 0.521 | 97.8% |
| B — prescription | Representative MFM extension (17 crop trees, 84 proposals) | 118 stems | 2.366 | 89.8% |
| C — manual override | Bounded manual marks (top-3 competitors per crop tree) | 85 stems | 1.091 | 95.3% |

**B vs A (wider treatment impact):** +84 stems, +1.845 m² BA  
**C vs B (manual vs prescription):** −33 stems, −1.275 m² BA

### Per-unit comparison (coverage units)

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

## Safety assertions (offline portion)

| Assertion | Result | Evidence |
|-----------|--------|----------|
| Per-unit BA safeguard ≤40% | PASS (all paths) | `safeguard: path A/B/C per-unit BA share ≤ 40%` |
| Wider treatment retains ≥50% stand BA | PASS (B=89.8%, C=95.3%) | `canopy: prescription/manual retains ≥50%` |
| MFM-10 species guard green | PASS (single-species fixture, no transfer possible) | `species-guard: fixture contains no non-Sitka stems` |
| Calibration version accepted | PASS (MFM03-CAL-1) | `species-guard: calibration version is accepted MFM-03-CAL-1` |
| CI parity within 20% (GAMEPLAY CALIBRATION) | PASS (3.76% delta) | `mfm03-residual: live CI parity within 20%` |
| Growth drift negligible | PASS (0.076 cm) | `mfm03-residual: growth drift — fixture mean DBH near accepted 15.4 cm` |
| Determinism (byte-identical output) | PASS | `MFM11_REPEAT_PASS two processes, byte-identical output: True` |
| Fixture identity preserved | PASS | `fixture identity: live MFM-02 footprint 75 stems / 1.4417531 m2 / SHA c2f2fd5418794915` |

## MFM-03 live/cross-year residual (offline portion)

| Residual | Offline result | Unity phase needed |
|----------|---------------|-------------------|
| Growth drift | 0.076 cm (negligible) | Yes — cross-year drift at post-Lesson-5 timing |
| Live competition-index parity | 3.76% delta (within 20%) | Yes — live parity at extension timing |
| Whole-stand state | 1,195 living, 23.18 m² BA, mean DBH 15.48 cm | Yes — post-Lesson-5 state |
| Natural cohorts/juveniles/shelters/deadwood/marks | UNAVAILABLE offline (year-0 fixture) | Yes — at extension timing |

**N1–N6 caveats carried:** The 20% CI tolerance is GAMEPLAY CALIBRATION, not empirical forestry. Simple SE separation is not independent-spatial proof. Boundary/tolerance behaviour is not empirical precision. The offline fixture is same-year; cross-year robustness is not established here.

## Commands executed

```bash
# Offline harness (this phase)
python3 Tools/Verification/Mfm11CalibrationImpact/run_offline.py \
  --out /media/jer/Files/CCF/Verification/MFM11-CalibrationImpact-20261101/offline
```

## Evidence hashes

| File | SHA-256 |
|------|---------|
| `offline/run1.log` | `1e711b22b0437840ac013584eb0473744db744f568db2d3ecd52d484269567de` |
| `offline/run2.log` | `1e711b22b0437840ac013584eb0473744db744f568db2d3ecd52d484269567de` (identical) |
| `offline/fixture-stand.csv` | `4b4b9703e65fe01146e57deb2095ea4f2cf3b702243c73075cdc2bf52f7f2433` |
| `offline/fixture-infrastructure.json` | `c7dd5b811c975c6bf6cd50a201308424c4f2eed6b9dca30f80300dc885644630` |

## What remains for the Unity phase

The following measurements require the Unity/runtime matrix (serialized machine-wide lock, currently held by PARITY-09 #219):

1. **Live execution of all three paths** — felling, economy, growth through the ordinary game loop
2. **Cash / contractor costs / timber revenue** — through the existing economy path
3. **Timber and deadwood outcomes** — after execution
4. **Regeneration / light effects** — and juvenile/protection/deadwood side-effects
5. **Repeated-intervention and completion reachability** — over the Scenario One horizon (Y0→Y30)
6. **Save/load continuation** — and protected compatibility fingerprints
7. **Live competition-index parity** — at post-Lesson-5 extension timing/state
8. **Growth drift** — between sample state and post-Lesson-5 state (cross-year)
9. **Natural cohorts / juveniles / shelters / deadwood / marks** — at extension timing
10. **Canopy evidence** — from the authoritative model at runtime

## Smallest Unity matrix

| Phase | Path | What to measure | Priority |
|-------|------|----------------|----------|
| U1 | A | Sample-only execution: removals, BA, cash, deadwood | HIGH |
| U2 | B | Full prescription execution: removals, BA, cash, deadwood, provenance | HIGH |
| U3 | C | Manual override execution: removals, BA, cash, deadwood | HIGH |
| U4 | A/B/C | Save/load after execution | HIGH |
| U5 | B | Post-Lesson-5 extension timing: live CI parity, growth drift, whole-stand state | HIGH |
| U6 | B | Natural cohorts/juveniles/shelters/deadwood/marks at extension timing | HIGH |
| U7 | A/B/C | Regeneration/light effects (Y1→Y2) | MEDIUM |
| U8 | A/B/C | Repeated intervention (Y6+) and completion (Y16→30) | MEDIUM |
| U9 | A/B/C | Protected hashes, Legacy40, Reference compatibility | HIGH |
| U10 | B | Canopy evidence from authoritative model | MEDIUM |

## Clean return state

- `git status --porcelain` shows only new verification tooling and handoff docs (no production changes)
- No harness `.cs`/`.meta` staged under `Assets/ForestPrototype/`
- No Unity run performed (offline phase only)
- No machine-wide lock acquired
- No push, no integration
- Evidence root: `/media/jer/Files/CCF/Verification/MFM11-CalibrationImpact-20261101/`

## Verdict

**NOT YET DETERMINED.** The offline phase proves the three-path removal arithmetic, per-unit safeguard, canopy retention, CI parity and species guard on the frozen MFM-03 fixture. The Unity/runtime matrix (live execution, economy, regeneration, completion, save/load, protected compatibility, live MFM-03 residual) is required before a pass/blocker verdict can be issued.

**Success token:** `MULTI-SCALE CALIBRATION QUALIFIED` — **NOT YET CLAIMED** (Unity phase pending)  
**Blocker token:** `BLOCKED — CALIBRATION IMPACT` — **NOT ISSUED** (no material accepted-outcome break found offline)
