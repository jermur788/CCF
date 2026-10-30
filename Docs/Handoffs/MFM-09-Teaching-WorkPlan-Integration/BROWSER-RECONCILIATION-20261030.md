# MFM-09 — Browser Reconciliation after S1-D T-05 Integration

**Date:** 2026-10-30  
**Browser lane:** #162  
**Preparation claim:** #180  
**Status:** **PREPARED / NOT RELEASED — #151 SATISFIED; WAITING #156 UNITY + MANAGER APPROVAL + INTEGRATION**

## Why this reconciliation exists

The original MFM-09 browser packet deliberately deferred exact teaching identifiers and post-S1-D Work Plan overlap until #151 could be inspected on an accepted lineage. #151 is now qualified and integrated. This document resolves those browser questions against exact current integration `ce6737146a7aa9ea953d7db0ca93b103f6c2c996` without releasing MFM-09 early.

## Current dependency state

Satisfied:

- S1-D T-05 #151 is `APPROVED / INTEGRATED / CLOSED`.
- Exact #151 production integration merge: `46fc6d55315bbddcdfa7667f0da490378d584023`.
- Current Scenario One integration/docs successor: `ce6737146a7aa9ea953d7db0ca93b103f6c2c996`.

Still mandatory before production release:

- MFM-08 #156 completes Tier-2 Unity/rendered qualification;
- #156 receives final Manager `APPROVED`;
- #156 is integrated onto the Scenario One integration branch;
- MFM-09 is repinned to the exact combined post-#156 integration SHA after a fresh ownership/diff inspection.

No MFM-09 local production claim is authorised yet.

## Exact teaching source and current identifiers

The correct source is:

`Assets/ForestPrototype/UI/ScenarioOneTeachingSequence.cs`

The older prepared packet's tentative path under `Assets/ForestPrototype/ScenarioOne/` was wrong and is superseded.

At exact integration `ce673714...`, the existing `ScenarioOneTeachingSequence.Lessons` identifiers and order are:

1. `access` — Open access
2. `future-trees` — Choose future trees
3. `release` — Release crowns
4. `habitat` — Retain habitat structure
5. `response` — Review the response
6. `recruitment` — Natural regeneration
7. `establishment` — Protect establishment
8. `transformation` — Transformation ready

The source explicitly states that these IDs must stay stable and that they are **presentation identifiers, not new success goals**. It also explicitly permits player-facing numbering to change when lessons are inserted or reordered.

### Persisted teaching-completion state is separate

Do not describe the eight lesson IDs as a persisted progress field.

Accepted D-066 / S1-D T-04P added one separate monotone save fact:

- save field: `ScenarioOneSaveData.firstGuidedRackReviewComplete`;
- shared live read: `ScenarioOneManager.FirstGuidedRackReviewComplete`.

It is written only when the pre-latch sequence genuinely crosses `response` → `recruitment` after the real rack review. Full saves round-trip it; world/reference/protected identity hashing strips it. That accepted v24 persistence contract is outside MFM-09 ownership.

`ScenarioOneTeachingSequence.Current(...)` consumes that fact to preserve completion of the first guided rack review. MFM-09 must not replace, rename, reset, reinterpret or duplicate this latch.

## Authoritative insertion point

The approved spec P1 and MFM-09 task require `Extend your treatment` **after the current Lesson 5 review**. On the exact current sequence, that means after `response` and before `recruitment`.

Therefore:

`... habitat → response → [new Extend your treatment step(s)] → recruitment → establishment → transformation`

The existing IDs remain byte-for-byte unchanged. Visible lesson numbers after `response` may move.

The approved spec defines one player-facing lesson, `Extend your treatment`, with staged activities covering prescription derivation/fit, coverage review, candidate confirmation, proposal review, 3D spot-check, revision/manual alternatives, prescription approval and creation of ordinary Pending work. The spec does **not** prescribe the exact number or literal names of new code-level IDs. This browser lane will not manufacture that detail prematurely.

Implementation may use one or more new unique stable IDs as required by the existing teaching infrastructure, but they must be new, non-colliding, semantically extension-specific, recorded in the implementation handoff and covered by rename/reuse mutation tests. If the implementation architecture would require reusing an old ID, STOP.

## Existing Lesson 5 / completion boundary

The accepted T-04P write boundary is the real `response` → `recruitment` transition after `ReleaseReviewed` and actual session rack-review completion. MFM-09 inserts extension teaching between those presentation stages, but it does **not** gain authority to change the accepted meaning of the historical first-guided-review latch.

Practical rule for implementation:

- the existing sample review must still be genuinely complete before extension teaching appears;
- extension/comparable-area work cannot back-credit or manufacture completion of the original sample lesson;
- a reload of an already completed first guided review must still remember that completion under the accepted v24 latch;
- adding extension steps must not cause an already-completed historical sample review to reopen merely because visible indices changed.

This is a sequencing/integration requirement, not a new save field.

## Exact accepted Work Plan overlap

At `ce673714...`, `Assets/ForestPrototype/UI/WorkPlanView.cs` already owns the generic current-work grouping:

- `Approved`;
- `Pending new`;
- `Pending carried over`.

`OrderClass(...)` derives those classes only from ordinary work status and `createdYear`. Future-created records are surfaced as inconsistent; no fourth class exists.

`AddOrderOrigin(...)` already shows prescription provenance when `prescriptionId > 0`:

- prescription ID;
- coverage-unit ID;
- Crop Tree ID;
- reason code.

That provenance is explanatory metadata inside the ordinary group. MFM-09 must not create a `Prescription` operational status or move prescription-generated work outside the generic grouping.

MFM-09 may refine the minimum teaching/explanatory presentation needed to tell the player why those Pending tasks appeared and that Work Plan approval is still required, but the accepted #151 classification remains authoritative.

## Exact prescription / commitment boundary

`ScenarioOneManager.PrescriptionApproval.cs` is explicit on the accepted MFM-07 contract:

- `CreatePrescription(...)` accepts/stores coverage decisions; nothing is marked or planned yet;
- `PreviewPrescription(...)` is read-only and proposals are not marks/orders/saved proposal state;
- `ApprovePrescription(...)` creates ordinary **Pending** felling orders with prescription provenance;
- it does not approve, fund or execute them;
- `ApprovePendingWork` in the ordinary Work Plan remains the commitment gate;
- existing contractor, cash and execution rules remain authoritative.

MFM-09 teaching must explain this sequence rather than adding another commitment path.

## Expected smallest production surface at eventual release

Fresh post-#156 inspection must still confirm ownership, but the current likely surface is now narrower and concrete:

- `Assets/ForestPrototype/UI/ScenarioOneTeachingSequence.cs`;
- `Assets/ForestPrototype/UI/LearningObjectivesView.Sequence.cs` and only the smallest companion teaching presentation required by the existing system;
- `Assets/ForestPrototype/UI/WorkPlanView.cs` only for the minimum prescription-origin teaching/explanation layered onto the accepted generic grouping;
- no `ScenarioOneUiRoot.cs` edit unless the post-#156 source proves a minimal adapter is unavoidable and the Manager explicitly assigns that overlap.

No save/schema/hash/reference edit is authorised.

## Browser verdict

`MFM-09 PREPARATION RECONCILED — #151 SATISFIED; WAITING #156`

The task remains **PREPARED / NOT RELEASED**. Once #156 is Manager-approved and integrated, browser/Manager must:

1. refresh exact combined integration SHA and active ownership;
2. inspect #156's accepted delta for teaching/Work Plan overlap;
3. repin the packet to that exact SHA;
4. create the substantive MFM-09 local claim only at actual execution start;
5. preserve the existing eight IDs and D-066 latch contract described above.

No Unity/runtime claim and no Windows build are made by this reconciliation.
