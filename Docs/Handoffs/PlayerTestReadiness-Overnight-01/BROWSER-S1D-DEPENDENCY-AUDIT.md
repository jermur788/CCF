# S1-D repeated-interventions dependency audit — browser lane #110

## Purpose

Reconcile the approved `specs/scenario-one-repeated-interventions-v1/` task DAG against current published GitHub evidence and identify the next executable/browser-preparable work without prematurely dispatching T-08/T-09.

## Authoritative task dependencies

- T-04 depends on T-01 and must land before later MFM curriculum insertion touching the same guide/current-state files.
- T-05 depends on T-01 + T-02 and must coordinate with the S1-C UI/history owner.
- T-07 is cancelled for V1.
- T-08 is blocked on T-02..T-06.
- T-09 is blocked on T-08 and must be independently reviewed.

## Current evidence by task

| Task | Browser-reconciled state | Evidence / consequence |
|---|---|---|
| T-00 decisions | COMPLETE | Approved spec/Manager decisions already recorded. |
| T-01 baseline re-verification | COMPLETE / HANDED-OFF | Claim #13 closed; verdict `S1-D BASELINE RE-VERIFIED — T-02..T-06 MAY BE DISPATCHED`. |
| T-02 advisory cycle/readiness | INTEGRATED | Claim #72 handoff accepted and PR #76 merged at `9fcb5ee7065950584bf98c88ca5ce95a4a455ca8`. |
| T-03 teaching-footprint cap | LOCAL QUALIFIED / PUBLICATION RECONCILIATION REQUIRED | #103 reports production `f629be3`, RackLesson verification reconciliation and local Unity PASS, plus a local merge `22ba458...`; however the GitHub integration ref remains `aca74ac...` and `22ba458...` is not fetchable. Do not count T-03 as published integrated yet. |
| T-04 guide monotonicity | NOT STARTED | No matching substantive claim/handoff found. It must use the same faithful persisted-history completion concept as T-03 and land before MFM curriculum insertion. |
| T-05 previous-versus-proposed presentation | NOT STARTED | No matching substantive claim/handoff found. It reuses S1-C Annual Review/history surfaces and groups Work Plan open orders. Current #104 S1-C T-05 actively owns save/history/UI closure, so S1-D T-05 must not start until that ownership is released/reconciled. |
| T-06 management-event ID integrity | QUALIFIED / INTEGRATED IN ACCEPTED LINEAGE | Independent review and Unity qualification claims are closed; later Rack/S1-C claims treat T-06/Rack v21 as integrated. No live T-06 writer remains. |
| T-07 conditional persistence | CANCELLED | Must not be reopened without explicit Manager decision. |
| T-08 multi-cycle witness | BLOCKED | T-04 and T-05 are unmet; T-03 publication status is unresolved. The T-02..T-06 prerequisite set is therefore not complete. |
| T-09 independent spec-conformance review | BLOCKED | Cannot begin before T-08 passes. |

## File/ownership sequencing

### T-04

Approved scope centres on `ScenarioOneTeachingSequence.Current` / `LearningObjectivesView.Sequence.cs` and a history-only monotonic latch. It must preserve stable IDs, avoid new persisted state and prove no regression after save/load.

Start only after:

1. T-03's exact integrated descendant is published so T-04 can reuse one faithful definition of teaching-sample completion rather than creating a competing latch;
2. active PARITY-07 #102 releases any overlapping teaching/curriculum writer ownership; and
3. live `active-tasks.md` is refreshed immediately before claim RUNNING.

### T-05

Approved scope groups open Work Plan orders as Approved / Pending new / Pending carried over and links into the existing S1-C history surface. It must not create a second history UI or add designation-time persistence.

Start only after:

1. #104 S1-C T-05 v22 closure releases `WorkPlanView` / Annual Review/history ownership;
2. the exact S1-C integrated API/surface is refreshed; and
3. any T-04 overlap is resolved by single-writer ownership.

## Dispatch decision

- **Prepare T-04 packet now**, but do not mark a production claim RUNNING until the three start conditions above are true.
- **Prepare T-05 packet now**, but do not dispatch implementation while #104 owns the shared S1-C UI/history surface.
- **Do not dispatch T-08 or T-09 yet.** Their packets may be prepared as later verification/review templates, but execution remains blocked by the approved DAG.

## Verdict

**S1-D DAG RECONCILED — NEXT IMPLEMENTATION IS T-04, THEN T-05; T-08/T-09 REMAIN BLOCKED.**

No Windows build.
