# TASK PACKET — PARITY-04 Focused Adult Species Unity Qualification

**Feature:** Scenario One Three-Species Parity P0  
**Parent task:** `PARITY-04-PATCH`  
**Task:** `PARITY-04-UNITY-QUALIFICATION`  
**Status:** BLOCKED — WAIT FOR PARITY-03 UNITY PASS  
**Manager:** ChatGPT Game Dev / Overall Manager

## Prerequisites

Do not execute this packet until:

1. PARITY-03 Unity qualification claim #10 has published a passing handoff and the Manager has accepted it; and
2. the machine-wide Unity lock is free in normal queue order.

If PARITY-03 returns `NEEDS CHANGES` or `REJECTED`, stop this task as blocked and return to the Manager.

## Candidate authority

Start from current `main` containing:

- PARITY-04 bounded patch integration PR #26 / merge `71b5ae4c83f3ce6d489cdec81f3306abb54d9df1`;
- `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-04/MANAGER-REVIEW.md`;
- the approved parity spec under `specs/scenario-one-three-species-parity-p0/`.

Create a dedicated clean verification worktree/lane. Refresh `Docs/Project/coordination/active-tasks.md` immediately before taking the Unity lock.

Recommended routing: T3 Code / local Unity-capable verification lane, strongest reasoning-capable model available.

## Goal

Run the first real Unity execution of the strengthened PARITY-04 adult gate on the exact integrated patch and determine whether the accepted Oak/Beech adult model, neutral site scalar, shared canopy/light, and shared Hegyi neighbourhood are verified in the live Unity project.

This is verification-only. Do not edit production or verification targets merely to get green.

## Required checks

### A. Identity and cleanliness

- verify `main` contains merge `71b5ae4c83f3ce6d489cdec81f3306abb54d9df1`;
- exact worktree/HEAD recorded;
- worktree clean before execution;
- no competing writer on the PARITY-04 verification surface;
- acquire/release the machine-wide Unity lock normally; no force-release.

### B. Static gates on the integrated candidate

Re-run without modification:

1. integrated calibration/source-contract gate; expected successful marker:
   `INTEGRATED_CALIBRATION_V1_PASS checkpoints=360 protected_files=7`
2. site check; expected existing PASS with 63 checks;
3. fixture controls; expected existing PASS with 9 checks;
4. source normalizer self-tests if required by the existing PARITY-04 handoff.

If the integrated source contract has drifted again because later production commits changed one of the seven pinned files, stop `SOURCE CONTRACT DRIFT — MANAGER REVIEW REQUIRED`. Do not refresh hashes in this task.

### C. Unity import / compile

Run Unity 6000.6.0f1 (or the exact repository-qualified Editor version if current canonical project metadata differs) on the exact candidate HEAD.

Requirements:

- import/compile succeeds;
- no new compile errors;
- do not use the standalone repo type-check as a substitute for Unity compilation;
- existing unrelated warnings may be reported but not suppressed/reclassified without evidence.

### D. Strengthened adult gate

Run `BroadleafAdultGrowthVerification` exactly as integrated.

Expected successful marker/check count from the patch handoff:

`checks=30`

The gate must verify at least:

- accepted adult constants exactly from the loaded Sitka/Oak/Beech assets;
- Scenario One live site scalar exactly `1.0` for the exercised ecology state;
- species-neutral canopy/light for identical geometry, with the sensitivity control proving the comparison is non-vacuous;
- exact shared 8 m Hegyi cutoff behaviour at the tested inside/outside boundary;
- Oak/Beech adult law behaviour already retained by the existing gate;
- protected Sitka comparator behaviour remains unchanged.

The Oak/Beech age-50/75/100 height values are **diagnostics only**. The accepted Beech provisional band miss must not fail the gate and must not be retuned.

### E. Preservation

No changes to:

- species assets;
- scenes/prefabs/materials;
- growth arithmetic or constants;
- save schema;
- site model;
- Hegyi radius;
- crown equations;
- protected fingerprints/anchors;
- Reference Future v1;
- Rack/S1-C/S1-D/MFM work;
- Windows build.

## Site-seam interpretation

Do not modify `run_site_check.py` or broaden its allow-list merely so the adult Unity gate can call dormant seam types. The separate site check is the seam-level proof; the adult Unity gate is the live-scalar proof. If either fails, report the exact failure.

## G3 integrity

Do not:

- weaken an assertion;
- change an expected value/check count to match output;
- refresh source-contract hashes;
- convert a failure into diagnostic output;
- suppress compiler/runtime failures;
- rebaseline any protected value.

Any such need returns to the Manager.

## Handoff

Publish:

`Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-04-Unity/HANDOFF.md`

Include:

- worker/model/provider;
- branch/worktree/starting HEAD/final tested HEAD;
- Unity version;
- lock receipt/acquire/release evidence;
- exact static-gate outputs;
- compile/import result;
- exact adult-gate marker/check count;
- Oak/Beech height diagnostic values as diagnostics, clearly not pass/fail targets;
- confirmation that no production/verification target changed during qualification;
- recommendation `APPROVE`, `NEEDS CHANGES`, or `REJECT`.

At handoff, mark the task claim `[HANDED-OFF]`, link the handoff, and close it.

## Exit

On success:

`ADULT SPECIES PARITY REQUALIFIED — READY FOR MANAGER REVIEW`

Otherwise return a precise blocker/failure token.

No whole-parity completion, Scenario One demo readiness, or Windows-build readiness is implied.
