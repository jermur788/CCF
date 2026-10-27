# TASK PACKET — MFM-03 Coverage Calibration Measurement

**Feature:** Multi-Scale Forestry Management V1  
**Spec:** `specs/multiscale-forestry-management-v1/` — APPROVED / D-065  
**Task:** `MFM-03 — Coverage calibration measurement`  
**Class:** bounded measurement / gameplay-calibration qualification  
**Status:** READY / DISPATCHED  
**Manager:** ChatGPT Game Dev / Overall Manager

## Purpose

Measure variation across the MFM-02 rack-served coverage units and qualify the smallest defensible V1 coverage metrics/tolerances. This task must not implement prescription logic, proposal state, Work Plan mutation, candidate confirmation UX, save schema or map rendering.

Historical Stage C values are **candidate inputs only**. Do not assume them correct, do not silently preserve them, and do not present game calibration as forestry standard practice.

## Authority

Read, in order:

1. current `Docs/Project/decision-log.md`, `current-milestone.md`, `game-brief.md`;
2. `specs/multiscale-forestry-management-v1/spec.md`, `plan.md`, `tasks.md`, `checklists/requirements.md`;
3. `Docs/Handoffs/MFM-Implementation-02/HANDOFF.md` and `MANAGER-REVIEW.md`;
4. live `Docs/Project/coordination/active-tasks.md`.

MFM-02 canonical integration: PR #38 / merge `07eee3d035a95e02b294e233482e791ce06d4250`.

## Routing

Target: **SOL/CODEX / strongest local reasoning-capable code+measurement model**. T3 Code or equivalent is appropriate. Deep Research may be used only for an evidence-only subtask; it does not choose gameplay tolerances.

Recommended branch:
`research/mfm-coverage-calibration-v1`

Use a dedicated clean worktree. This task is measurement/calibration only; no UI ownership is granted.

## Unity sequencing

MFM-02 released the machine-wide Unity lock. `RACK-V2-04` claim #30 was already waiting and therefore has queue priority.

You may perform repository inspection, static/offline measurement tooling and analysis immediately. Before any Unity execution:

- refresh live coordination;
- verify #30 is not RUNNING/holding the lock;
- verify branch/HEAD/worktree ownership;
- acquire the machine-wide Unity lock normally;
- mark this task claim `[RUNNING]` with actual agent/model/provider, branch, worktree, starting HEAD and lock identity.

Do not bypass another Unity owner. No Windows build.

## Required measurement set

Using the integrated MFM-02 rack-served geometry and the accepted Scenario One V2 fixture, measure per coverage unit and the explicit outer remainder behaviour:

1. living stem count and density;
2. mean DBH;
3. mean accepted competition/Hegyi measure using the authoritative existing implementation;
4. the known former-edge dense-band effect, explicitly identified rather than averaged away;
5. outer-lattice / remainder behaviour, including zero-area explicit remainders;
6. regeneration / juvenile conflicts relevant to treatment transfer;
7. access / clearing exclusions relevant to whether the sample treatment is comparable;
8. dedicated Crop Tree candidate-spacing options and resulting confirmation workload.

For candidate-spacing measurement, keep MFM V1 semantics: spacing identifies where to inspect; it does **not** select quality or automatically designate Crop Trees. Do not process Oak/Beech with Sitka-only candidate rules.

## Required calibration analysis

Produce at least:

- the full per-unit metric table;
- candidate metric/tolerance definitions;
- sensitivity analysis showing how the covered/not-covered classification changes across a bounded tolerance range;
- examples of units that clearly fit, clearly fail, and sit near the boundary;
- effect of the former-edge band and outer remainder treatment;
- candidate-spacing workload comparison, including how many player confirmations/rejections would be required under each tested spacing option;
- explicit evidence labels for every proposed threshold.

Any chosen tolerance/value must be labelled **GAMEPLAY CALIBRATION** unless a stronger evidence class is actually supported by accepted research. Do not manufacture empirical precision.

## Decision boundary

You may recommend a V1 metric set and tolerances if the evidence is sufficient, but do not implement prescription logic. The Manager will accept/reject the calibration recommendation after handoff review.

If no stable, interpretable tolerance separates comparable from materially different units without excessive sensitivity, return:

`BLOCKED — COVERAGE CALIBRATION NOT ROBUST`

with the measurements and the smallest product decision needed.

## Preservation / exclusions

Do **not**:

- change MFM-02 geometry semantics;
- retune Rack V2 geometry/headlands;
- change Lessons 2–5 behaviour;
- implement prescriptions, candidate state, proposal state, Work Plan integration, Fell marks, approval or save state;
- edit map/3D UI;
- change species biology, economy, timber, pruning, S1-C/S1-D behaviour;
- add Oak/Beech quantitative treatment rules;
- rebaseline protected model-0/Legacy40/Reference Future fixtures;
- run a Windows build.

Any tooling added must be measurement-only and clearly separated from production mechanics.

## Verification expectations

Static/offline first. Use Unity only where the required metric cannot be established reliably from authoritative current state without it.

If Unity is used, capture exact candidate HEAD, lock receipt, command/gate identity, logs, measured fixture identity and clean release. Preserve the MFM-02 qualified fixture and protected historical paths.

The real player-authored Rack V2-path requalification remains a later integration obligation and is **not** silently satisfied by the five-rack fixture here.

## Handoff

Publish:
`Docs/Handoffs/MFM-Implementation-03/HANDOFF.md`

Include:

- exact inspected/tested HEAD;
- actual executor/model/provider;
- files/tooling changed, if any;
- complete per-unit measurement table;
- sensitivity analysis;
- candidate-spacing workload results;
- recommended metrics/tolerances with evidence labels;
- covered/not-covered examples;
- Unity evidence if used;
- preservation statement;
- remaining uncertainties/product decisions.

## Exit

`COVERAGE CALIBRATION QUALIFIED — READY FOR MANAGER REVIEW`

or the precise blocker token above.
