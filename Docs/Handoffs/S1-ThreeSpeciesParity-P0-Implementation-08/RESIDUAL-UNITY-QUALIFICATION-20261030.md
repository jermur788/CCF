# PARITY-08 residual Unity qualification — model-0 + Sitka timber anchors

**Parent:** #91  
**Feature claim:** #126  
**Label:** LOCAL/CODE REQUIRED — verification/Unity only  
**Difficulty:** 3/5 — bounded harness/runner work on high-consequence compatibility evidence; no production change  
**Exact PARITY-08 candidate under test:** `3eb9c954c363659a1b012e06ce58a35ab87f4303`  
**Current verification/integration lineage:** `b6b96cb3a51e80bd6b61df7dc73b1c1237950a32` (contains approved #165 reconciliation)

## Purpose

Close the two mandatory PARITY-08 evidence gaps that remain after #165:

1. a real Unity positive **growth-model-0** compatibility/save-load replay on the exact `3eb9c954...` candidate;
2. a real Unity execution of the protected **Sitka timber anchor** checks on the exact same candidate.

The PARITY-08 packet explicitly requires positive model-0/model-1/model-2 fixtures and protected pure-Sitka lifecycle/density and Sitka timber anchors. Model-1/model-2 and protected density evidence are now clean after #165; these two items remain.

## Scope

Verification-only. Add or minimally extend runner/harness plumbing only where required to drive existing accepted behavior on an installed Unity Editor.

Allowed:

- smallest runner mode / entry point needed for model-0 compatibility replay;
- smallest runner mode / entry point needed to drive existing ThreeSpecies Sitka timber-anchor verification in Unity;
- verification-only diagnostics needed to prove exact candidate/fixture/anchor identity.

Forbidden:

- production C# changes;
- save/schema/validator/hash/reference behavior changes;
- anchor/fixture/rebaseline edits;
- changing accepted model-0 historical semantics;
- changing timber economics/species calibration;
- weakening or deleting assertions;
- broad stale-v23 sweep outside what these two gates actually require;
- Windows build.

If either gate exposes a genuine production or protected-anchor defect, STOP and return the exact failure. Do not repair production under this claim.

## Required execution

Before Unity:

- refresh `Docs/Project/coordination/active-tasks.md`, #91/#106/#126/#128 and current integration ref;
- confirm exact candidate `3eb9c954c363659a1b012e06ce58a35ab87f4303` is unchanged;
- fresh-check Unity lock, Editor processes and storage floor;
- claim the machine lane normally;
- use the approved #165 verification runner behavior from current integration lineage; do not silently revert its v24 reconciliation.

Required clean receipts:

1. compile/import on exact candidate;
2. model-0 positive compatibility/save-load replay with explicit PASS marker and clean Unity exit;
3. protected Sitka timber-anchor gate with explicit expected anchor values/fingerprint comparison and clean Unity exit;
4. host saves unchanged, staged harness removed, worktree/lane clean, normal lock release.

A PASS marker followed by Editor hang is not a pass; rerun and preserve the hung receipt.

## Handoff

Publish:

`Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-08/RESIDUAL-UNITY-HANDOFF.md`

Include exact verification commit, exact candidate, commands, PASS markers, expected/observed anchors, all failed/hung attempts, lock/storage state and clean restoration.

Success token:

`PARITY-08 RESIDUAL UNITY EVIDENCE COMPLETE — READY FOR #128 FINAL REVIEW`

No Windows build.
