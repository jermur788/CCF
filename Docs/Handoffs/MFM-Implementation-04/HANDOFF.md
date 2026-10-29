# MFM-04 — Deterministic Prescription Core

**Task:** `MFM-04`
**Feature:** Multi-Scale Forestry Management V1 (D-065)
**Claim:** GitHub issue #93 (umbrella #91)
**Exit:** **PRESCRIPTION CORE QUALIFIED** (static/offline phase only — not Unity-verified, no Windows build)

## Worker / provider

| Phase | Worker, model and provider |
|---|---|
| WIP checkpoint `e635ef7` | Earlier MFM-04 worker; committed as "WIP checkpoint (unverified)", 234/236 |
| Area-parity fix, qualification, this handoff | Claude Code (T3 Code harness), Claude Opus 5.5 (`claude-opus-5-5`), Anthropic |

## Identity

| Item | Exact value |
|---|---|
| Base | `72dbee8` |
| WIP checkpoint | `e635ef7` |
| **Candidate SHA (production + check bytes)** | `71674e42c9fd186146aef1efee529019a68e7766` |
| Branch | `feature/mfm-04-deterministic-prescription-core-v1` |
| Worktree | `/media/jer/Videos/CCF-MFM04` |
| Evidence root | `/media/jer/Files/CCF/Verification/MFM04-PrescriptionCore-areafix-final` (plus `-areafix-run1`, `-areafix-run2`) |

The commit that adds this handoff is documentation-only on top of the candidate.

## Files (base `72dbee8` → candidate)

| File | Role |
|---|---|
| `Assets/ForestPrototype/ScenarioOne/ScenarioOneCoverage.cs` (+ `.meta`) | Coverage classification of the 15 anchor-aligned thirds and two remainders against the sample. **Fix in this pass:** `Frame.MeasureAreas`. |
| `Assets/ForestPrototype/ScenarioOne/ScenarioOneCropTreeCandidates.cs` (+ `.meta`) | Spacing-only Crop Tree candidates (domain for MFM-05). |
| `Assets/ForestPrototype/ScenarioOne/ScenarioOnePrescription.cs` (+ `.meta`) | Derive / CheckFit / ProposeRelease / EvaluateSafeguard. |
| `Assets/ForestPrototype/ScenarioOne/ScenarioOnePrescriptionCalibration.cs` (+ `.meta`) | Versioned calibration `MFM03-CAL-1`, rule `MFM-V1-RULE-1`. **This pass:** comment on `AreaStepM` only. |
| `Tools/Verification/MfmPrescriptionCore/run_prescription_core_offline.py` | Offline runner (plain .NET 8, two processes, byte-identical). |
| `Tools/Verification/MfmPrescriptionCore/OfflineLogic/PrescriptionCoreOfflineCheck.cs` | 236 checks. **This pass:** removed one WIP `Console.WriteLine("DBG …")` line; no assertion, threshold or expected value changed. |

No scene, prefab, ProjectSettings, package, save/schema, UI, Work Plan or approval file is touched.

## Root cause

`ScenarioOneCoverage.Frame.MeasureAreas` measured net area with a 2-D midpoint grid (`AreaStepM = 0.1`) over the whole 80 m stand:
each 0.1 × 0.1 cell was counted for whatever held its centre. MFM-03 (`Tools/Verification/MfmCoverageCalibration/analyse_coverage.py`,
`World.net_area`, commit `139b1f9`) integrated with 0.1 m midpoint strips in x but an **exact** union of corridor/mask/footprint intervals
in z. Circular and capsule mask edges and corridor edges that fall between grid centres in z therefore gave up to 1.676 m² area error
and 0.54 pp density error on some areas. A finer grid only shrinks this (0.05 m → 0.534 m², still failing) and would change the
calibration signature, so the measurement method — not the step — was the defect.

## Exact interval method (now in `Frame.MeasureAreas`)

1. The along-axis range of the stand (`±h(|AxisX|+|AxisZ|)`) is split at the segment boundaries `anchorAlong ± SegmentHalfLengthM`, so
   no strip straddles two segments (for the fixture: x ∈ [−40,−12], [−12,12], [12,40], as MFM-03's anchor-aligned-thirds rects).
2. Each piece is divided into `ceil(length / AreaStepM)` midpoint strips (width `du`). A strip is the line through the midpoint running
   along the rack normal (for the east-west fixture: a column at fixed x, running in z — exactly MFM-03's column).
3. On that line every shape is convex, so it covers one interval, computed exactly: stand square (two slabs), every confirmed rack
   corridor (oriented rectangle), the teaching frame rectangle (and its corridors / model-0 inner band), every mask capsule (hull of the
   two end-disc chords and the middle rectangle), and every unit/remainder offset boundary (line crossing).
4. The line is cut at every interval end. On each piece no shape membership can change, so the piece is classified **once at its middle
   by the existing `Locate`** and its exact length × `du` is added to the sample, area or remainder total.

Because `Locate` is unchanged and still does the classifying, precedence is identical to the stems': outside > rack corridor > outer
remainder > mask > sample > area segment. `AreaStepM` keeps its accepted value 0.1 and its place in the calibration signature; it is now
the strip width (as MFM-03's `STEP`), not a 2-D grid cell. No calibration number changed (thirds/±12 m segment, ±20/15/20 %, ≥30 stems,
boundary margins 0.65/1.5, 16 m spacing, no tail guard, second sample not required).

The method is general for rotated/translated rack layouts; the fixture exercises the axis-aligned case only (residual below).

## Verification (actually run, this session)

Command: `python3 -I -B Tools/Verification/MfmPrescriptionCore/run_prescription_core_offline.py --out /media/jer/Files/CCF/Verification/MFM04-PrescriptionCore-areafix-final`

```
MFM04_BUILD_PASS sources=10 warnings-as-errors
MFM04_DIGEST coverage=b50810a729c77c05 candidates=23b4eadf7e0f7228 release=a1577f2a1b137970 fit=98ce8d428bacb4f1 calibration=8591fcfc8f163e8f
MFM04_PRESCRIPTION_CORE_PASS checks=236 failed=0
MFM04_REPEAT_PASS two processes, byte-identical output: True
```

- **236/236 PASS**, 0 failed. Run three separate times (six processes); every `run*.log` has SHA-256
  `fc4d2dd548f980302493e4c02d59d707b2838e06d8481bd16e1f05a5b94356b2`.
- Fixture SHA-256 as pinned by the runner: `fixture-stand.csv` `4b4b9703…2f7433`, `fixture-infrastructure.json` `c7dd5b81…644630`
  (from `73356e3`).
- Area parity: every one of the 14 compared areas is within **≤ 0.001 m²** of MFM-03 (the expected values are stored to 3 dp; was
  1.676 m²). Density parity worst ≤ 0.0005 pp (was 0.539818 pp). Source: 75 stems, 309.809539 m² (MFM-03 309.80954).
- Not run: Unity, Editor compile, Windows build (out of scope for this static phase; no Unity lock taken).

## Interfaces for MFM-05 / 06 / 07 (all pure, no UnityEngine, write nothing)

- `ScenarioOneCoverage.Classify(Input, ScenarioOnePrescriptionCalibration) → Result` — areas (`Id` `R{rack}{L|C|H}`, `State`,
  `Fit`, `Reasons`, `Flags`, `Metrics`, `Comparison`, `StemIds`, `Evidence`), `Remainders`, `Source`, `SampleRemovalIds`,
  `Competition`, `CoveredCount/NonSourceCount`, `SecondSampleRequired`, `Signature()`. Helpers: `Compare`, `CompetitionIndex`, `AreaId`.
  Inputs: confirmed MFM-02 geometry, stems, orders, `Mask`s, `Feature`s, `PlayerManualAreas`.
- `ScenarioOneCropTreeCandidates.Suggest(coverage, currentStems, sampleCropTreeIds, confirmedIds, rejectedIds, calibration)` — spacing-only,
  batch-stable candidate list (**MFM-05**). Candidates never become Crop Trees here; caller records confirmations/rejections.
- `ScenarioOnePrescription.Derive(geometry, stems, orders, calibration) → Derivation` (rule from the executed sample);
  `CheckFit(...)` (honesty replay listing differing decisions); `ProposeRelease(rule, coverage, currentStems, confirmedCropTreeIds,
  sampleCropTreeIds, keepStanding)` → `ReleasePlan` (plain proposal data); `EvaluateSafeguard(coverage, currentStems, proposedRemovalIds,
  ceilingShare)`.
- `ScenarioOnePrescriptionCalibration.V1` / `RuleVersion` / `Signature()` — the version + signature **MFM-06** should persist as
  provenance with any stored proposal.
- **MFM-07** owns proposal state and approval: nothing here is a `TreeMarkType.Fell` mark, a `ScenarioOneWorkOrder` or approved; the Work
  Plan stays the commitment gate.

## Preserved anchors

- MFM-03 calibration `MFM03-CAL-1` and rule `MFM-V1-RULE-1` unchanged; calibration digest `8591fcfc8f163e8f`.
- MFM-03 parity thresholds as authored (0.5 m², 0.2 pp, 0.002 mean), source 75 stems / 309.81 m², R1L clear fail on mean competition
  with margin 1.659, MFM-03 section-8 boundary/clear fits, edge-stem counts — all PASS unchanged.
- Protected values outside calibration: 30 % first release, 40 % basal safeguard, 8 m teaching spacing, 8 m Hegyi reach.
- MFM-02 geometry (`ScenarioOneRackSampleGeometry`, `ScenarioOneRackBay`) not modified. Frozen Scenario One Reference Future v1 not touched.
- Scope boundary: no UI, save/schema, Work Plan mutation, approval state, hidden ranking, broadleaf quantitative release or Fell proposals.

## Residuals

- The fixture is axis-aligned; rotated-layout area measurement is implemented generally but has no MFM-03 parity reference.
- Not Unity-compiled in this phase; the files are pure C# 9 and build warnings-as-errors on .NET 8 alongside the existing offline stubs.
  A Unity compile/integration gate is still required before integration.
- Live infrastructure → `Mask` adapter is the caller's job (later layer), as before.

**PRESCRIPTION CORE QUALIFIED**
