# S1-D IMPLEMENTATION 01 — BASELINE RE-VERIFICATION

**Task ID:** S1-D-T01  
**Feature:** Scenario One Second and Subsequent Interventions  
**Spec:** `specs/scenario-one-repeated-interventions-v1/` — **APPROVED**  
**Task type:** READ-ONLY BASELINE / QUALIFICATION PRECHECK  
**Overall Manager:** ChatGPT Game Dev  
**Context used for specification:** `eb218eab8b8e200a81cc99bfef7e8906bc21a88d`

## STATUS

**BLOCKED-READY — DO NOT START UNTIL RACK V2 CORE IS INTEGRATED.**

The S1-C Review / Management History contract is already APPROVED and integrated into `main`. The remaining dependency is the actual combined Rack Planning & Field Marking V2 core integration base. This task MUST inspect that real integrated candidate, not `c6342f7` and not either Rack lane in isolation.

When the Manager publishes the combined Rack V2 integration candidate SHA, this packet becomes executable without further product decisions.

## Goal

Re-verify the S1-D assumption register against the exact integrated Scenario One implementation base before any repeated-intervention production edit is allowed.

The task answers one question:

> Does the integrated code still match the approved S1-D assumptions closely enough that T-02 through T-06 may be dispatched as written?

## Preconditions

Before starting:

1. Refresh the live GitHub `Docs/Project/coordination/active-tasks.md`.
2. Obtain the exact Manager-designated combined Rack V2 integrated candidate SHA.
3. Confirm Rack V2 core is integrated on that candidate, not merely implemented on a feature branch.
4. Confirm the approved S1-C contract exists in canonical Git.
5. Work read-only. No production edits, save edits, scene/prefab edits or Unity asset writes.

If any precondition is false: **STOP — DEPENDENCY NOT READY**.

## Required inspection

Re-check every approved S1-D baseline finding on the exact candidate:

- F-1…F-12 first-cycle assumptions;
- R-1…R-5 already-repeatable behaviours;
- Rack V2 Objective-1 teaching/lifecycle interaction;
- S1-C history implementation and data contract;
- repeated Work Plan / mark / approve / advance availability;
- teaching-footprint 40% cap behaviour and ownership;
- sequential guide regression conditions;
- Annual Review acknowledgement semantics;
- open/stale-order behaviour;
- protected two-reviewed-cycle completion chronology;
- management-event identity validation status;
- continued management after Completed;
- save/schema/model ordering relevant to later tasks.

## S1-C reconciliation — mandatory

Confirm there is exactly **one authoritative derived management-history implementation**.

Reconcile the approved S1-C contract with the inherited lineage, including at least:

- intervention grouping unit;
- stable work-order and management-event identity;
- annual report access;
- ecological snapshot / before-after semantics;
- Annual Review acknowledgement-by-year derivation;
- completion review / next-step guidance;
- stale/open-order query surface;
- whether any duplicate history store, query layer or UI has appeared.

If two competing history systems/stores exist: **STOP — DUPLICATED HISTORY AUTHORITY**.

## Verification

Run the existing offline checks applicable to the exact candidate, including:

- Scenario One completion N3 offline check;
- Scenario history offline check;
- Rack-bay / teaching offline check;
- runtime-source compile/typecheck where already available and non-Unity;
- exact normalised `ScenarioOneCompletionRules` hash comparison.

Do not launch a broad Unity qualification matrix for this read-only task. If a narrow Unity run becomes necessary to determine whether a static finding is true, return to the Manager first.

## Required output

Publish:

`Docs/Handoffs/S1-D-Implementation-01/HANDOFF.md`

The handoff MUST contain:

- exact candidate SHA inspected;
- exact branch/worktree used;
- finding table: each F-1…F-12 and R-1…R-5 → `UNCHANGED`, `CHANGED BUT SPEC-COMPATIBLE`, or `BLOCKER`;
- S1-C reconciliation result;
- Rack V2 interaction result;
- exact commands/checks run and results;
- completion-rule hash result;
- new blockers or ownership conflicts;
- verdict.

## Verdicts

Use exactly one:

- `S1-D BASELINE RE-VERIFIED — T-02..T-06 MAY BE DISPATCHED`
- `S1-D BASELINE CHANGED — MANAGER REVIEW REQUIRED`
- `S1-D BLOCKED — INTEGRATION / AUTHORITY CONFLICT`

## Exclusions

No production change.  
No save-schema change.  
No completion-rule change.  
No objective/economy/ecology change.  
No Rack lifecycle edit.  
No S1-C UI implementation.  
No Multi-Scale Forestry Management implementation.  
No Windows build.
