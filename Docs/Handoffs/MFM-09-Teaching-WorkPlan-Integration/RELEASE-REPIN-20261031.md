# MFM-09 — Release / Exact-Candidate Repin — 2026-10-31

**Execution claim:** #207  
**Parent:** #91 / browser #200 / local orchestration #201/#203  
**Base packet:** `Docs/Handoffs/MFM-09-Teaching-WorkPlan-Integration/TASK-PACKET.md`  
**Status:** **RELEASED / READY — QUEUED BEHIND CURRENT S1-C UNITY PRIORITY**

This amendment supersedes the base packet's stale `PREPARED / NOT RELEASED — WAITING #156` status and all `TBD AFTER #156 INTEGRATION` release fields. All substantive MFM-09 authority, teaching/Work Plan semantics, exclusions, verification requirements and success token in the base packet remain binding.

## Exact released start

`integration/scenario-one-player-test-ready-v1` = **`34e360f85835afd15f5323f287372e0f2ee83dd9`**.

Accepted prerequisites on this exact published lineage:

- S1-D T-05 #151 — APPROVED / INTEGRATED / CLOSED;
- MFM-08 #156 and integration #205 — APPROVED / INTEGRATED / CLOSED;
- v24 / D-066 teaching latch authority remains accepted;
- MFM-08 treatment-review / candidate / proposal UX is now integrated and is read-only context for MFM-09.

Browser overlap review: the MFM-08 integrated 15-file delta modifies Stand Map / Treatment Review / Tree Inspection surfaces and MFM-08 offline verification; it does not modify MFM-09's expected primary teaching sequence, `LearningObjectivesView.Sequence.cs`, or `WorkPlanView.cs` surfaces. If fresh local inspection shows a need to edit `ScenarioOneUiRoot.cs` or MFM-08 review files, STOP for Manager overlap disposition as required by the base packet.

## Execution / scheduling

Claim #207 is READY, but #206 S1-C T-08 holds the preferred next serialized Unity slot. Non-Unity implementation may begin only when #203 confirms no ownership conflict and doing so will not silently advance the published integration lineage ahead of the established S1-C/S1-D qualification sequence. Any Unity phase is machine-serialized.

Before production edits: refresh `active-tasks.md`, #207, exact integration head, branch/worktree cleanliness and writer ownership. Mark #207 `[RUNNING]` with actual provider/model, branch/worktree and start SHA only at actual start.

Suggested branch/worktree remain:

- `feature/mfm-09-teaching-workplan-integration-v1`
- `/media/jer/Videos/CCF-MFM09`

## Binding boundaries

Preserve all existing stable teaching IDs and D-066 first-guided-review latch. Insert `Extend your treatment` after `response` and before `recruitment`. Prescription approval creates ordinary Pending work with provenance; Work Plan approval remains the commitment gate. Do not duplicate/redesign MFM-08 review UX, alter save/schema/hash/Reference, Rack V2, Annual Review/history, contractor/economy semantics, or accepted old teaching IDs.

Publish `Docs/Handoffs/MFM-09-Teaching-WorkPlan-Integration/HANDOFF.md`.

Success token: `SCENARIO ONE EXTENSION FLOW QUALIFIED`.

Dispatch:

`Run MFM-09 under claim #207 using Docs/Handoffs/MFM-09-Teaching-WorkPlan-Integration/TASK-PACKET.md plus RELEASE-REPIN-20261031.md from exact published integration 34e360f85835afd15f5323f287372e0f2ee83dd9 after fresh ownership preflight. Preserve all eight existing teaching IDs and the v24 D-066 latch; add Extend your treatment after response and before recruitment; keep prescription-generated work ordinary Pending until Work Plan approval; consume rather than redesign MFM-08 review UX. Run required offline/mutation checks and serialized Unity Tier-2 at both render sizes. Publish HANDOFF.md and return SCENARIO ONE EXTENSION FLOW QUALIFIED only if every gate passes. No Windows build.`

User action required: none.
