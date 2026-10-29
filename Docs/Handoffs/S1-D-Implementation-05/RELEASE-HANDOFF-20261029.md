# S1-D T-05 — Release Handoff

**Claim:** #151  
**Label:** LOCAL/CODE REQUIRED  
**Status:** RELEASED FOR NON-UNITY IMPLEMENTATION  
**Difficulty:** 4/5 — bounded presentation work across Work Plan and existing history navigation, with strict semantic separation from MFM-09 and no new persistence.  
**Best-suited executor:** OpenAI Sol/Codex high-reasoning UI lane or Claude Sonnet-class Code lane, one primary writer.

## Exact start

Use the current Scenario One integration lineage after Manager-approved S1-C T-06:

- S1-C T-06 reviewed production merge: `d959e0397c80bc7a9d7a707c8d4f437f946231f4`
- S1-C T-06 qualified/final handoff: `5a0fc5190e2d47a02997706c68e38958a3a3bb3f`
- exact integration production merge: `625e0adf5e7fd71184cb074e85e1901b417ec02d`
- integration docs successor / exact #151 start: **`cfdc93bf3943125d21570bc5749a9338f52d1cce`**

Branch/worktree:
- `feature/s1-d-t05-previous-proposed-v1`
- `/media/jer/Videos/CCF-S1D-T05`

Before production edits, refresh `Docs/Project/coordination/active-tasks.md`, #91/#106/#143/#151/#156 and the integration branch; verify clean exact start and no overlapping Work Plan/history/MFM-09 writer; then mark #151 `[RUNNING — OFFLINE]` with actual model/provider/branch/worktree/start SHA.

## Purpose

Implement the approved S1-D T-05 repeated-intervention presentation using ordinary open Work Plan records and the already-integrated S1-C review/history surface. Do not create a second history store or another interpretation of prior work.

Binding grouping:
- **Approved** = open ordinary `ScenarioOneWorkOrder` with `status == Approved`.
- **Pending new** = `status == Pending && createdYear == CurrentEcologicalYear`.
- **Pending carried over** = `status == Pending && createdYear < CurrentEcologicalYear`.
- `createdYear > CurrentEcologicalYear` is inconsistent data; surface/stop appropriately, do not invent a fourth class.

Prescription approval remains distinct: MFM prescription approval creates ordinary **Pending** work with provenance. It is not Work Plan approval. Work Plan approval remains the commitment gate.

## Ownership / surface

Primary production surface:
- `Assets/ForestPrototype/UI/WorkPlanView.cs`

Reuse the integrated S1-C exact-year/review/history seam. Prefer no edit to `AnnualReviewView.cs`. If a genuinely missing tiny read-only navigation hook is required, stop before editing and record the exact missing API/overlap for Manager disposition rather than broadening silently.

A smallest unowned UI routing hook may be used only if clearly necessary and ownership is free. Do not collide with MFM-08 #156.

## Hard exclusions

No new persistence/date field; no save/schema/validator/hash/reference edits; no duplicate history UI/store; no auto-approval/cancellation; no completion/economy/ecology changes; no MFM proposal semantics; no MFM-09 `Extend your treatment` curriculum or prescription-specific teaching/grouping; no Windows build.

## Verification / handoff

Non-Unity implementation and static/offline verification may begin now. Heavy Unity remains serialized: immediately before any Unity use, refresh current lock/process/free-space/priority and do not bypass another owner.

Required rendered qualification remains 1280×720 and 1920×1080 with a three-cycle fixture proving Approved / Pending new / Pending carried over are visibly distinct and prior/carried work links to the faithful S1-C Annual Review/history year without mutating history.

Publish `Docs/Handoffs/S1-D-Implementation-05/HANDOFF.md`.

Success token: `S1-D T-05 PREVIOUS/PROPOSED PRESENTATION QUALIFIED — READY FOR MANAGER REVIEW`.

No Windows build.
