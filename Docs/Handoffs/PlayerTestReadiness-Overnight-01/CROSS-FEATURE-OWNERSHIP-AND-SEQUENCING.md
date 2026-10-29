# Scenario One Player-Test Readiness — Cross-Feature Ownership and Sequencing

**Parent:** #91  
**Current browser lane:** #122  
**Date:** 2026-10-29  
**Status:** Manager sequencing disposition; no packet/claim grants production ownership by itself

## Purpose

Prevent remaining S1-C, S1-D, three-species parity and Multi-Scale Forestry Management work from creating parallel writers on the same save, teaching and Work Plan surfaces, while allowing genuinely file-separable work to proceed.

This document changes no product requirement. It orders approved work around the current qualified production base and the MFM-07 return.

## Current published production boundary

Exact post-MFM-06 production candidate:

`b971f4da0540e606bfee9702b0c99f81a0a89355`

This exact SHA was merged and Unity-qualified with save schema v23, frozen Reference Future v1 and protected hash compatibility green.

Published `integration/scenario-one-player-test-ready-v1` has documentation-only successor:

`7608f02cb8da1fabd8b78fd5aaf45b0c32086f67`

MFM-07 #121 is released from the exact production SHA above. At the last browser refresh its GitHub claim still read `[DISPATCHED]`, so no local execution state is inferred until the worker publishes it.

## Immediate parallel-safe work

The following two tasks are now released because their intended production file sets do not overlap the authorised MFM-07 scope:

1. **S1-C T-04 provenance labelling — #123**  
   Packet: `Docs/Handoffs/S1-C-Implementation-04/TASK-PACKET.md`.  
   Owns Annual Review/history presentation/query labels only while RUNNING. No save/schema/validator or MFM work-order/proposal edits.
2. **S1-D T-04 guide monotonicity — #124**  
   Packet: `Docs/Handoffs/S1-D-Implementation-04/TASK-PACKET.md`.  
   Owns `ScenarioOneTeachingSequence` / `LearningObjectivesView.Sequence` only while RUNNING. No schema/validator/MFM implementation. Must land before MFM-09.

Both may implement/offline-test in separate worktrees while MFM-07 works, subject to actual live ownership. Any Unity qualification remains machine-wide serialized.

## MFM-07 return gate

When #121 publishes `Docs/Handoffs/MFM-07/HANDOFF.md`, browser/Manager review must inspect the exact diff and evidence before any dependent writer is released.

Mandatory review questions:

- proposals remain distinct from `TreeMarkType.Fell`;
- Work Plan open/approval cannot import unapproved proposals;
- prescription approval creates pending ordinary work only;
- ordinary Work Plan approval remains required afterward;
- manual player state wins conflicts;
- each generated order validates prescription + coverage unit + player-confirmed Crop Tree **before write**, with fail-closed/no-partial-mutation evidence;
- v23 provenance is reused, not duplicated;
- no unauthorised schema/save-validator/hash/reference change;
- no MFM-09 teaching insertion;
- no broadleaf quantitative extrapolation.

Verdict is exactly `APPROVED`, `NEEDS CHANGES`, or `REJECTED`.

## Post-MFM-07 executable fan-out

If MFM-07 is **APPROVED** and integrated/published on one exact candidate, the next work may fan out only across non-overlapping surfaces:

### A. MFM-08 — Map / 3D review UX

- Packet: `Docs/Handoffs/MFM-Implementation-08/TASK-PACKET.md`.
- Dependency: accepted/integrated MFM-07 API.
- Primary surface: Stand Map / candidate-proposal review / bounded 3D navigation and rendered UX.
- Keep separate from teaching/Work Plan writers.
- Suggested route: Claude Sonnet 5.5 Medium or stronger UI-capable Code lane; rendered qualification under serialized Unity.

### B. S1-D T-05 — Previous versus proposed presentation

- Packet: `Docs/Handoffs/S1-D-Implementation-05/TASK-PACKET.md`.
- Dependency: MFM-07 pending-work/provenance API stable; S1-C history surface API published and not actively owned for the same files.
- Primary surface: `WorkPlanView` grouping (`Approved`, `Pending new`, `Pending carried over`) plus smallest navigation hook to existing S1-C history.
- It must consume, not duplicate, S1-C history and MFM generated-pending provenance.
- May run beside MFM-08 if exact file ownership is disjoint.

### C. PARITY-08 — Save / deterministic compatibility

- Packet: `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-08/TASK-PACKET.md`.
- Release only after the actual MFM-07 diff confirms MFM-07 did not take an unreconciled save/validator/hash/reference surface.
- Owns save validation/determinism while RUNNING; no other save-path writer overlaps it.
- No parity-specific schema field/version bump; protected Sitka/Legacy/Reference anchors may not be rebaselined.

### D. S1-C continuation

After T-04 accepts/integrates:

- T-06 prior-treatment guidance uses the same S1-C history/Annual Review surface and should serialize behind any active S1-C UI writer.
- T-07 identity/history contract verification may be read-only/offline where possible and must not create a competing identity/history store.
- T-08 waits for T-04/T-06/T-07 and qualifies the complete S1-C slice on the current combined lineage.

## Curriculum / Work Plan single-writer order

The required order on the shared teaching/current-state and Work Plan seam is:

1. **S1-D T-04 #124** — guide monotonicity.
2. **S1-D T-05** — previous/pending Work Plan grouping after MFM-07 API is stable.
3. **MFM-09** — `Extend your treatment` teaching insertion and generated-pending explanation.

MFM-09 must consume the accepted T-04 monotonicity and T-05 grouping. It must not recreate those mechanisms.

Existing lesson IDs remain stable; MFM-09 adds new IDs only.

## Species sequence

1. **PARITY-08** — save/deterministic compatibility.
2. **MFM-10** — Sitka-only MFM species guard on the actual MFM-09 flow.
3. **PARITY-09** — integrated parity qualification including the actual MFM guard.

This order avoids a dependency cycle and ensures Oak/Beech are not silently processed by Sitka-only proposal logic.

## Repeated-intervention / review closure

- **S1-D T-08** waits for T-02/T-03/T-04/T-05/T-06 and performs the multi-cycle witness.
- **S1-D T-09** is independent spec-conformance review after T-08 PASS.
- **S1-C T-08** waits for T-04/T-06/T-07.

## MFM endgame

1. MFM-08 review UX.
2. MFM-09 teaching / Work Plan integration after S1-D T-04/T-05.
3. MFM-10 species guard after PARITY-08.
4. MFM-11 calibration-impact qualification only when the intended repeated-intervention/player-facing flow is stable.
5. MFM-12 integrated MFM verification/review after MFM-11 plus relevant parity/S1-C/S1-D feature qualification.

## Programme endgame

Final Tier-3 runs only after all feature-level gates are accepted on one exact combined candidate. It reruns the S1-C T-05 save/history chain and protected save/reference/determinism gates on that exact candidate. Then the independent final review occurs. Only after the exact Tier-3-qualified candidate is handed to the user does the user personally playtest it.

No Windows build before the user's personal playtest and explicit authorisation.

## Shared-surface summary

- **Save/load/validation/hash/reference:** MFM-06 → MFM-07 return inspection → PARITY-08 → final Tier-3. One writer.
- **Teaching sequence/current state:** S1-D T-04 → MFM-09. One writer.
- **Annual Review/history:** S1-C T-04 → S1-C T-06/T-07 → S1-C T-08. One S1-C writer per overlapping file set.
- **Work Plan presentation:** MFM-07 API first → S1-D T-05 → MFM-09.
- **Map/3D MFM review UX:** MFM-08 after MFM-07, independent from Work Plan/curriculum where possible.
- **Species truth:** PARITY-08 → MFM-10 → PARITY-09.
- **Unity:** all heavy/Editor qualification serialized machine-wide regardless of code-file separation.

## Dispatch rule

Packet publication and a `[DISPATCHED]` claim grant no production ownership. At actual start, the worker refreshes live `active-tasks.md`, verifies branch/worktree/HEAD and conflicts, marks its claim `[RUNNING]`, and the live coordination record assigns the single-writer surface.

No Windows build.
