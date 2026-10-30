# PARITY-08 — Model-0 Baseline Discrimination

**Parent:** #91  
**Feature:** #126 PARITY-08  
**Follow-up:** #166 partial residual qualification  
**Claim:** #167  
**Label:** LOCAL/CODE REQUIRED — Unity diagnostic only  
**Difficulty:** 2/5 — one exact baseline replay and comparison

## Purpose

Determine whether the failed `completion-model0` diagnostic on exact PARITY-08 candidate `3eb9c954c363659a1b012e06ce58a35ab87f4303` is caused by PARITY-08 or already exists on the accepted pre-PARITY-08 baseline.

This task does **not** repair the historical completion harness, change reconstruction semantics, change an expected hash, change production, or rebaseline any anchor.

## Authority / why this is the next step

The approved PARITY-08 spec requires old model-0/1 save compatibility, model-2 compatibility, Legacy40, Reference archive and deterministic anchors. D-047 records completion anchor `6F84AF319D301F87` for regeneration model 1 / growth model 0, and D-048 says legacy/growth-0 anchors are retained.

Separately, positive old-model-0 save/load compatibility already has strong evidence: Reference Future v1 is a frozen **save-v12** archive, and current `ForestSaveController.LoadData` explicitly restores every save before v17 with `GrowthModel.Legacy` (0). The already-green Reference Future contract loads and continues its historical Year-50 save under current code. Therefore the failed completion replay is a deterministic/historical diagnostic that still needs attribution, not the only model-0 compatibility witness.

## Exact comparison

### Candidate result already observed

Candidate: `3eb9c954c363659a1b012e06ce58a35ab87f4303`

Using #166 plumbing `a3cb615ff4870c4fc8ab06cb6cf1fa18ce0a091f`:

- gate: `completion-model0`;
- existing `ScenarioOneCompletionVerification` staged unchanged from the #165 verification lineage;
- environment: `CCF_GROWTH_MODEL=0`, `CCF_REGEN_MODEL=1`; no reconstruction change;
- result: Unity exit 1, no PASS marker;
- every objective reported ok, but Scenario One did not reach `Completed` at Year 25;
- completion hash `046551EB29730CE1`;
- v16 layout hash `20B85B0152EA747A`;
- historical D-047/D-048 anchor `6F84AF319D301F87` was not reproduced.

### Baseline to run

Exact production baseline target:

`185200f57140c65ed4d540ff0c4d173451170d14`

This is the accepted integration base from which #126 started.

Use the **same verifier sources and same runner semantics** as the candidate run. The only intended experimental variable is the production target SHA in U1.

Do not edit verifier semantics before this comparison.

## Required procedure

1. Refresh `Docs/Project/coordination/active-tasks.md`, claims #91/#106/#126/#128/#166/#167, and current machine state.
2. Record #167 `[RUNNING]` with provider/model, branch/worktree, starting SHA and intended U1 target.
3. Fresh-check ZX20 free space, Unity lock and Editor processes. Respect >=15 GiB mandatory floor; prefer >=20 GiB.
4. Use branch/worktree `verify/parity-08-model0-baseline-v1` / `/media/jer/Videos/CCF-Parity08-Model0Baseline` or an equivalent clean verification-only lane.
5. Use the exact #166 runner/plumbing semantics from `a3cb615ff4870c4fc8ab06cb6cf1fa18ce0a091f` (or its handoff successor where production semantics are identical).
6. Detach U1 at exact baseline `185200f57140c65ed4d540ff0c4d173451170d14`.
7. Run compile if the runner requires a receipt, then run **only** `completion-model0`.
8. Capture exact markers: exit code, PASS/FAIL marker, completion year/outcome, objective lines, completion hash, v16 layout hash, v17/v18 compatibility hashes, host-save invariance, cleanup and lock release.
9. Restore U1 to its pre-task clean state and release the lock normally.

## Decision rule

### A. Baseline PASS / candidate FAIL

Return:

`PARITY-08 MODEL-0 BASELINE PASSES — CANDIDATE REGRESSION CONFIRMED`

Then #126 is **NEEDS CHANGES**. Do not modify production under #167.

### B. Baseline materially identical FAIL

Materially identical means the same substantive failure class: Year-25 completion does not become `Completed` despite the same objective-success pattern, with the same or clearly equivalent diagnostic/hash drift under identical verifier semantics.

Return:

`PARITY-08 MODEL-0 FAILURE REPRODUCED ON BASE — NOT CANDIDATE REGRESSION`

This establishes that #126 did not introduce the completion-replay drift. Do **not** rebaseline `6F84AF319D301F87` and do not silently rewrite the historical harness. #128 may then disposition this as pre-existing D-047/D-048 verification debt while relying on the already-green Reference Future v1 save-v12 load/continuation as positive old-model-0 save compatibility evidence.

### C. Materially different baseline outcome

STOP and return the exact comparison for Manager review. Do not infer equivalence.

## Boundaries

No production C#. No save/schema/validator changes. No fixture/anchor edits. No pre-calibration reconstruction changes. No expected-value update. No rebaseline. No Windows build.

## Deliverable

Publish:

`Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-08/MODEL0-BASELINE-HANDOFF.md`

Include exact baseline SHA, verifier SHA/source, commands, machine preflight, observed markers, candidate-vs-base table, cleanup state and one of the decision tokens above.
