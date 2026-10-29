# PARITY-03 Q10 — Controller Continuity Measurement
**Label: LOCAL/CODE REQUIRED — T2 CODE SUITABLE**
**Claim #74**

## Purpose
Finish the already-static-approved PARITY-03 Q10 evidence-strengthening task by running the remaining controller-level Unity `SeamTransition` measurement. This is verification-only: do not change production behaviour.

## Difficulty / model
- Difficulty: **2/5**.
- Recommended executor: **T2 Code** / capable local Codex-class agent. Higher tiers are unnecessary unless the controller measurement exposes an unexpected defect.
- Existing static implementation was produced by Claude Sonnet 5.5 and Manager-approved at exact candidate `3383e763e8ccff945dd00250366b7f334fe57331`.

## Existing lane
- Claim: #74.
- Branch: `verify/parity-03-density-continuity-q10-v1`.
- Existing worktree: `/media/jer/ZX20/CCF-Q10`.
- Static candidate: `3383e763e8ccff945dd00250366b7f334fe57331`.
- Existing handoff: `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-03-Q10/HANDOFF.md`.
- Static Manager review: `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-03-Q10/MANAGER-REVIEW-STATIC.md`.

The historical Unity blocker was Rack field marking. Rack #90 is now completed, so that dependency no longer blocks this measurement. Unity remains machine-wide serialized.

## Start gate
Before touching the worktree or Unity:
1. refresh live `Docs/Project/coordination/active-tasks.md`, #74, #90, #91 and #106;
2. inspect the existing Q10 worktree without resetting/stashing/cleaning/discarding anything;
3. verify the static candidate and existing verification changes are intact;
4. compare the relevant density/controller production files against the current accepted lineage. If those production semantics changed since the static candidate in a way that invalidates the predicted measurement, STOP and return for Manager repin rather than silently rebasing the verification result;
5. verify the machine-wide Unity lock is free before acquiring it;
6. mark #74 `[RUNNING — UNITY CONTINUATION]` with model/provider, branch/worktree, exact starting HEAD and Unity lane.

## Required work
Run the already-written controller-level `SeamTransition` measurement on the faithful Q10 verification candidate. Confirm it reproduces the static continuity prediction and does not reveal a material 0→1 mixed-density discontinuity.

Do not:
- edit production C#;
- retune density/species/rack parameters;
- change save/schema/scenes/prefabs;
- rebaseline protected anchors;
- weaken tests to obtain green;
- build Windows.

If a harness-only compatibility repair is strictly required to run on the current Unity/editor environment, keep it minimal and document it separately from the measurement result.

## Verification / stop condition
Required evidence:
- Unity import/compile for the exact verification candidate;
- controller `SeamTransition` measurement result;
- comparison against the static predicted seam evidence in the existing handoff;
- confirm exact Sitka-component behaviour remains unchanged where the measurement covers it;
- document Unity version, candidate SHA, lock ownership, logs/evidence path and cleanliness.

If the measurement demonstrates a material production discontinuity or contradicts the accepted static result, STOP with:
`PARITY-03 Q10 CONTINUITY DEFECT FOUND — MANAGER G3/G5 DECISION REQUIRED`

Otherwise return:
`PARITY-03 DENSITY CONTINUITY EVIDENCE STRENGTHENED — READY FOR MANAGER REVIEW`

Update the existing `HANDOFF.md` with the Unity-continuation section and exact evidence. Do not integrate anything; this task is verification-only.

## Copy/paste dispatch line
`Continue claim #74 from Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-03-Q10/UNITY-CONTINUATION-20261029.md. This is LOCAL/CODE REQUIRED and suitable for T2 Code. Use the existing verify/parity-03-density-continuity-q10-v1 lane and /media/jer/ZX20/CCF-Q10; refresh live ownership and inspect the worktree before changing anything. The static candidate 3383e763e8ccff945dd00250366b7f334fe57331 is already Manager-approved for the static phase; Rack #90 is complete, so run only the remaining controller-level Unity SeamTransition measurement. Do not change production behaviour, retune parameters, rebaseline anchors or build Windows. If current production/controller semantics have materially changed since the static candidate, stop for Manager repin rather than silently rebasing.`
