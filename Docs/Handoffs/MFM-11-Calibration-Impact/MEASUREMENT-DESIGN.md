# MFM-11 Calibration Impact — Measurement Design

**Status:** DESIGN — PRE-IMPLEMENTATION  
**Claim:** #220  
**Candidate:** `3acdf9cb635cdca79606335801e97f9d7dcce8b0`  
**Date:** 2026-11-01  

## 1. The three paths

All three paths share the same deterministic initial world and fixture:

- **Fixture:** MFM-03 frozen Enlarged80 stand (1,344 Sitka stems, `id,x,z,dbh_cm,height_m`), frozen infrastructure JSON, pinned SHA-256.
- **Racks:** 5 teaching racks at qualified Z positions (`{-35.2711754, -16.4217644, 0.2382353, 15.7935295, 35.2323532}`), entrance X = −35.4, far X = 40, half-width = 40.
- **Sample crops:** `P0810`, `P1206`, `P1214`.
- **Sample release:** competitor removal around the 3 sample crops until `MinPlannedAndExecuted >= FirstReleaseShare (0.30)` with ceiling `MaxRemovedBasalShare (0.40)`.

### Path A — Historical sample-only

- Racks + sample + sample release only.
- **No** wider prescription application. No extension. No crop-tree candidates beyond the 3 sample crops.
- Removals = sample release competitor set only.
- This is the accepted pre-MFM teaching baseline.

### Path B — Representative-prescription/default extension

- Racks + sample + sample release (same as Path A).
- Then: coverage classification → crop-tree candidates (spacing-only, 16 m) → 17 crop trees confirmed → `ScenarioOnePrescription.ProposeRelease` produces proposals across 13 covered units (84 proposal links).
- Prescription approved → Work Plan approved → executed.
- This is the accepted MFM-09 extension path.

### Path C — Plausible manual override

- Racks + sample + sample release (same as Path A).
- **No** prescription system used. The player manually marks competitor trees around crop trees using ordinary `TreeMarkType.Fell` marks and the ordinary Work Plan.
- **Bounded rule (transparent, no hidden future knowledge):** For each of the 17 crop trees (same crop-tree set as Path B, chosen by the same spacing-only candidate ordering), the player marks the highest-Hegyi-competition competitors within the crop tree's 8 m influence zone until the crop tree's local release share reaches 30% (same rule target), subject to the 40% per-unit ceiling. The player does not know which trees the prescription would propose; they apply the same release rule locally.
- If the manual marks would exceed the 40% ceiling in any unit, the player stops early in that unit (transparent bound).
- Removals = manual Fell-mark set across all 17 crop trees' influence zones.

**Unavoidable difference:** Path A has only 3 sample crops; Paths B and C have 17 crop trees. The sample release set is identical across all three.

## 2. Metrics

For each path, measure by affected coverage unit and whole stand:

| # | Metric | Classification | Offline | Unity |
|---|--------|---------------|---------|-------|
| 1 | Living removals (stem count, species split) | DIRECT | YES | YES |
| 2 | Retained stems (count, species split) | DIRECT | YES | YES |
| 3 | Basal area before/after (per unit + whole stand) | DIRECT | YES | YES |
| 4 | Basal area across relevant cycles (Y0→Y1→Y2+) | DIRECT | NO | YES |
| 5 | Retained/continuous canopy evidence | DIRECT (model) | PARTIAL | YES |
| 6 | Cash / contractor costs / timber revenue | DIRECT (economy) | NO | YES |
| 7 | Timber outcomes (volume, assortment) | DIRECT (economy) | NO | YES |
| 8 | Deadwood outcomes (records, volume) | DIRECT (economy) | NO | YES |
| 9 | Regeneration/light effects | DIRECT (model) | NO | YES |
| 10 | Juvenile/protection/deadwood side-effects | DIRECT (model) | NO | YES |
| 11 | Repeated-intervention/completion reachability | DIRECT (rules) | NO | YES |
| 12 | Prescription/work-order provenance (Pending→Approved→Executed) | DIRECT (data) | PARTIAL | YES |
| 13 | Protected compatibility/fingerprints | DIRECT (hash) | YES | YES |
| 14 | Save/load continuation | DIRECT (save) | NO | YES |
| 15 | Per-unit basal-area safeguard (≤40%) | DIRECT | YES | YES |
| 16 | MFM-10 species guard (no Sitka→Oak/Beech transfer) | DIRECT | YES | YES |
| 17 | MFM-03 live residual: competition-index parity | DIRECT (model) | PARTIAL | YES |
| 18 | MFM-03 live residual: growth drift | DERIVED | PARTIAL | YES |
| 19 | MFM-03 live residual: whole-stand state | DIRECT | YES | YES |
| 20 | MFM-03 live residual: natural cohorts/juveniles/shelters/deadwood/marks | DIRECT (model) | NO | YES |

**Classification legend:**
- **DIRECT** = authoritative model output or deterministic computation.
- **DERIVED** = computed from direct measurements (e.g. BA deltas, species ratios).
- **GAMEPLAY CALIBRATION** = accepted thresholds (20% density tolerance, etc.) — not empirical forestry claims.

## 3. Offline phase (this deliverable)

### What the offline harness proves

1. **Path A vs Path B removal arithmetic** per unit and whole stand:
   - Stem counts and BA removed by sample-only vs full prescription.
   - Per-unit safeguard: every unit's removed BA share ≤ 40%.
   - Species split of removals (fixture is all Sitka; code handles mixed).

2. **Path C manual override arithmetic:**
   - Manual Fell-mark removal set under the bounded rule.
   - Comparison against Path B's prescription proposals.
   - Per-unit safeguard for manual path.

3. **MFM-03 residual offline portion:**
   - Hegyi competition-index values in the fixture at the coverage-comparison state.
   - Whole-stand state: stems, BA, mean DBH, species composition.
   - Calibration tolerance boundary behavior (±20% density, ±15% DBH, ±20% CI).

4. **Species guard (MFM-10):**
   - No Sitka-only quantitative transfer to Oak/Beech.
   - Calibration parameters are species-aware.

5. **Provenance data model:**
   - Path B orders carry `prescriptionId`, `prescriptionCoverageUnitId`, `prescriptionCropTreeId`, `prescriptionReason`.
   - Path A/C orders have `prescriptionId = 0` (ordinary marks).

### What remains for the Unity phase

- Live execution of all three paths (felling, economy, growth).
- Cash/cost/revenue through the ordinary economy path.
- Timber/deadwood outcomes after execution.
- Regeneration/light effects and juvenile/protection/deadwood side-effects.
- Repeated-intervention and completion reachability over the Scenario One horizon.
- Save/load continuation and protected compatibility fingerprints.
- Live competition-index parity at post-Lesson-5 extension timing/state.
- Growth drift between sample state and post-Lesson-5 state.
- Natural cohorts/juveniles/shelters/deadwood/marks at extension timing.
- Canopy evidence from the authoritative model at runtime.

## 4. Smallest Unity matrix

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

## 5. Fixture and commands

- **Fixture:** `Tools/Verification/MfmCoverageCalibration/inputs/fixture-stand.csv` (SHA-256 `4b4b9703...`) + `fixture-infrastructure.json` (SHA-256 `c7dd5b81...`).
- **Offline command:** `python3 Tools/Verification/Mfm11CalibrationImpact/run_offline.py --out /media/jer/Files/CCF/Verification/MFM11-CalibrationImpact-20261101/offline`
- **Unity command (later):** `python3 Tools/Verification/Mfm11CalibrationImpact/run_gate.py --out ... --lock-id ... --owner ... --expected-head ...`

## 6. Evidence output

All evidence written outside the repo under `/media/jer/Files/CCF/Verification/MFM11-CalibrationImpact-20261101/`:

- `offline/` — offline check output, comparison tables, hashes
- `unity/` — Unity gate output (later phase)
- `commands.txt` — exact commands executed
- `result-hashes.txt` — SHA-256 of all evidence files
