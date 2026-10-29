# MFM-07 — Proposal State and Approval Boundary

Parent: #91 PLAYER-TEST-OVERNIGHT-01
Claim: #121
Lane: ChatGPT Code / local T3 execution
Status: PREPARED — BLOCKED ON #113 + browser #120 readiness verdict

## Purpose

Implement the separate prescription proposal state and prescription approval boundary required by Multi-Scale Forestry Management V1, without allowing proposals to become Fell marks or bypass the existing Work Plan commitment gate.

## Difficulty

High. This task crosses proposal generation, provenance, work-order creation and the approval boundary, and must preserve manual player authority.

## Preferred model/provider

ChatGPT Code / OpenAI 6.1 Sol.

## Dependencies

Do not start until:

1. #113 returns `MFM-06 V23 INTEGRATED — READY FOR MFM-07` on an exact umbrella SHA.
2. Browser #120 returns `MFM-07 READY FOR CODE EXECUTION — DEPENDENCIES AND OWNERSHIP RECONCILED` or an equivalent explicit Manager approval.
3. Live `active-tasks.md` confirms no conflicting writer on the files/interfaces MFM-07 needs.

Root the worktree from the exact post-#113 umbrella candidate, not from historical `ac876a6`.

Suggested branch: `feature/mfm-v1-proposal-approval-boundary-v1`
Suggested worktree: `/media/jer/Videos/CCF-MFM-07`

## Binding authority

Approved `specs/multiscale-forestry-management-v1/`, task MFM-07 and checklist H/I/J/K as applicable.

## Critical invariants

- Prescription proposals are never `TreeMarkType.Fell`.
- Opening the Work Plan cannot import unapproved proposals.
- Approving the Work Plan cannot silently import unapproved proposals.
- Prescription approval creates **pending ordinary work only**.
- Work Plan approval remains required after prescription approval.
- Existing contractor/landowner/economy execution remains authoritative.
- Manual player state takes precedence over proposal state.
- Conflicts are surfaced and reconciled rather than silently overwritten.
- MFM-06 v23 provenance is reused rather than duplicated.
- No broadleaf quantitative extrapolation is introduced.

## Mandatory inherited MFM-06 review condition

**Before any prescription-generated work order is written, validate that order's coverage unit and Crop Tree against the referenced prescription.**

Required behavior:
- referenced prescription exists and is the expected rule/version;
- referenced coverage unit is valid for that prescription and accepted/eligible for generated work;
- referenced Crop Tree is valid for that prescription/unit and remains player-confirmed where required;
- mismatch or stale identity fails closed;
- no invalid order is written and no world/work state is partially mutated;
- validation result is testable and produces a clear bounded reason.

Do not defer this check to MFM-08/09.

## Scope

Implement only the proposal-state and prescription-approval boundary plus the minimum API/state needed to create validated pending ordinary work with MFM-06 provenance.

## Exclusions

- no MFM-08 map/3D review UX except a minimal interface required by the MFM-07 contract;
- no MFM-09 teaching-sequence insertion;
- no hidden automatic approval or execution;
- no automatic Crop Tree confirmation;
- no schema bump unless v23 is demonstrably insufficient and the task returns to the Manager first;
- no Oak/Beech Sitka-rule transfer;
- no Windows build.

## Required verification

Focused offline/static tests:
- proposal state is distinct from Fell marks;
- deterministic proposal identity/order where applicable;
- manual state wins defined conflicts;
- invalid/stale prescription link fails closed;
- invalid coverage-unit link fails closed;
- invalid/unconfirmed Crop Tree link fails closed;
- failed validation creates no partial work-order mutation.

Work Plan boundary negative tests:
- opening Work Plan causes no unapproved proposal import;
- Work Plan approval alone causes no unapproved proposal import;
- prescription approval creates pending work but does not approve/execute it;
- subsequent ordinary Work Plan approval remains required.

Unity/local qualification on exact candidate:
- compile/import;
- focused runtime path for proposal → prescription approval → pending work;
- no auto-execution;
- save/load of v23 state/provenance as needed;
- narrow regressions for existing Work Plan/manual marks/orders.

## Handoff

Publish `Docs/Handoffs/MFM-07/HANDOFF.md` with:
- exact starting umbrella SHA;
- implementation SHA;
- file diff;
- explicit proof of the MFM-06 inherited validation condition;
- Work Plan leakage/bypass negative-test results;
- Unity/runtime result;
- save/provenance result;
- residuals and exact next dependency.

Success token:

`PRESCRIPTION APPROVAL BOUNDARY QUALIFIED — READY FOR MANAGER REVIEW`

## Copy/paste dispatch

Run claim #121 from `Docs/Handoffs/MFM-07/TASK-PACKET.md` only after #113 and browser #120 release it. Use ChatGPT Code / OpenAI 6.1 Sol. Root from the exact post-MFM-06 integration SHA. Implement only separate proposal state and prescription approval, preserving Work Plan as the commitment gate. Before writing any prescription-generated order, validate its coverage unit and Crop Tree against its prescription and fail closed on mismatch. Publish `Docs/Handoffs/MFM-07/HANDOFF.md`. No Windows build.