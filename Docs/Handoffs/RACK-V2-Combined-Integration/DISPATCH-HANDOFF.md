# RACK-V2 Combined Integration — Dispatch Handoff

**Date:** 2026-10-27  
**Issued by:** Overall Manager  
**Task:** `RACK-V2-COMBINED-INTEGRATION`  
**Status:** **READY / DISPATCHED**

## Dispatch instructions

**Send this handoff to:** **TTree Code — local Unity-capable Game Dev integration lane**.

Use a fresh dedicated integration worktree. Do not use the user's dirty primary checkout and do not reuse either source lane as the integration worktree.

**Integration branch:** `integration/scenario-one-rack-v2-v1`  
**Canonical execution packet:** `Docs/Handoffs/RACK-V2-Combined-Integration/TASK-PACKET.md` on `main`  
**03C Manager approval:** `Docs/Handoffs/RACK-V2-03C/MANAGER-REVIEW.md` on `main`

**Return token:**

`RACK V2 COMBINED CANDIDATE QUALIFIED — READY FOR MANAGER INTEGRATION REVIEW`

and publish `Docs/Handoffs/RACK-V2-Combined-Integration/HANDOFF.md` on the integration branch.

## Source identities — mandatory preflight

The integration worker must fetch and prove these exact inputs before writes:

- current `main` at task start — record the exact SHA in the handoff;
- 02B-R production candidate: `7f708e4e30e61f3ce952a4ac75bde5fa2d8d38cd` from `feature/scenario-one-rack-v2-headland-v1`;
- 03C production implementation: `2178d392dd0fd285a30383e0bcf4d0df1745a8af`;
- 03C final tested HEAD: `84f801850acc26906a7cd8dd1a452d0a8da3f90a`;
- 03C published branch tip at Manager approval: `098e18745cef9fdca5627bfb1b7f9bd3935e98e0` on `feature/scenario-one-rack-v2-runtime-v1`;
- common designated Rack V2 source base: `c6342f79a3e89c040fadd232017a937132779385`.

The tested 03C delta from `2178d392...` to `84f80185...` is harness-only. Do not treat `098e187...` as a new production candidate; it is a documentation/publication tip.

If any identity is missing or differs unexpectedly: `STOP — SOURCE IDENTITY MISMATCH` and return to the Manager before integrating.

## Integration objective

Create one reviewable Rack V2 candidate that deliberately reconciles:

1. qualified 02B-R fresh-world/headland/feeder implementation;
2. qualified 03C authoritative rack runtime/order lifecycle;
3. current canonical `main` documentation/spec/management authority.

Do not independently redesign either qualified source lane. Resolve production conflicts only after inspecting both source handoffs and the approved Rack V2 spec.

The full scope, exclusions, qualification requirements, protected fingerprints, F-10 / `RackUxRack` treatment, save checks, rendered checks and handoff requirements are defined in `TASK-PACKET.md`; follow that packet exactly.

## Required qualification emphasis

This is a combined candidate, so source-lane qualification alone is not sufficient. Run focused Tier-2 qualification on the **exact combined HEAD**, including:

- compile/import clean;
- 03C authoritative runtime gate;
- RackV2Save gate;
- 02B-R headland/access qualification;
- seven protected fingerprints exact;
- fresh model-1 and historical/model-0 compatibility;
- pending/approved/completed reconciliation semantics;
- stable rack IDs/geometry/mark state across save/load;
- real cold-start Continue from a persisted V2 save where the packet requires it;
- subsequent Scenario One progression through `rack.approve`, execution and `rack.result`;
- `RackUxRack` assertions explicitly mapped retained / replaced / retired under G3 rather than weakened;
- rendered review at 1280×720 and 1920×1080 for states the integration could regress.

Acquire and release the machine-wide Unity lock normally. No Windows build is authorised.

## Ownership / concurrency

Before editing, refresh live `Docs/Project/coordination/active-tasks.md` and verify no conflicting Rack V2 integration writer exists.

S1-C work may run concurrently only because its authorised production surface is separate. Do not absorb, merge or edit S1-C work as part of this task.

Do not implement Multi-Scale Forestry Management, S1-D, three-species parity, economy/ecology retuning, or unrelated UI work.

## Manager continuation after return

When the combined handoff returns, the Overall Manager will inspect the exact candidate and issue `APPROVED`, `NEEDS CHANGES`, or `REJECTED` for integration to canonical `main`.

If approved and integrated, that state unlocks the already-prepared S1-D baseline re-verification and the post-Rack Multi-Scale Forestry Management sequence; those are not part of this dispatch.

**DASHBOARD SYNC: NOT AVAILABLE**  
**Published dashboard reflects this task: NO**
