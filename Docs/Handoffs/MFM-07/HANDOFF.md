# MFM-07 handoff — proposal state and prescription approval boundary (claim #121)

PRESCRIPTION APPROVAL BOUNDARY QUALIFIED — READY FOR MANAGER REVIEW

Scope: the separate proposal state and the prescription-approval boundary only. No MFM-08 UX, no MFM-09 teaching insertion, no schema or save-validator change, no Windows build, no broadleaf quantitative extrapolation.

## Identity

- Executor: Claude Code, Anthropic Claude Sonnet 5.5 (`claude-sonnet-5-5`). (The packet's preferred route was OpenAI 6.1 Sol; the Manager's dispatch authorised Claude Code with Sonnet 5.5 preferred.)
- Starting umbrella SHA: `b971f4da0540e606bfee9702b0c99f81a0a89355` (MFM-06 v23, integrated and Unity-qualified).
- Branch: `feature/mfm-v1-proposal-approval-boundary-v1`; worktree `/media/jer/Videos/CCF-MFM-07`.
- **Implementation (qualified) SHA: `d4188b33e58b0fe67504a87167692d01b226c84e`.** The commit holding this handoff is a documentation-only successor; no verification is attributed to it.
- Unity 6000.6.0f1, clean detached U1 at the implementation SHA under normal lock `9e1ebf07576a430c91e15bb020d1bc85` (released; lock now FREE, no Editor).
- #121 was marked `[RUNNING]` in `Docs/Project/coordination/active-tasks.md` before production edits.

## File diff against b971f4d

Production (all new; **no existing production file was modified**, so `ScenarioOneManager.cs`, the save data/validator, the Work Plan and the marking manager are byte-identical to the base):

- `Assets/ForestPrototype/ScenarioOne/ScenarioOnePrescriptionApproval.cs` (+.meta) — pure, no UnityEngine: `Propose`, `ValidatePrescription`, `ValidateLink(s)`, `Build`.
- `Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.PrescriptionApproval.cs` (+.meta) — the only writer: `CreatePrescription`, `PreviewPrescription`, `CheckPrescriptionLink`, `ApprovePrescription`.

Verification tooling: `Tools/Verification/MfmApprovalBoundary/` (offline check + runner, Unity gate, a copy of the MFM-06 runner with one added gate). Coordination: `active-tasks.md`.

## Design

- **Proposal state.** A proposal is plain data (`OrderLink`: prescription id, tree, coverage unit, Crop Tree, closed reason code), recomputed on demand from the stored prescription and live confirmed state (spec 14, K6). It is never a `TreeMarkType.Fell` mark, never an order, never saved, and nothing in the Work Plan or marking manager references it.
- **Player precedence.** A manual Fell mark, an open order of any kind, or a Crop Tree designation always holds its tree; held trees are surfaced as `Conflicts`. A unit the player left manual, or that coverage no longer calls covered (including because of the player's own marks), receives no proposal and is surfaced as `UnitNoLongerCovered`. The player's own planned felling is counted against each unit's basal-area ceiling together with the proposal.
- **Acceptance is explicit.** `CreatePrescription` stores `AcceptedCovered`/`LeftManual` choices for named units only; nothing is implied. It writes no order, mark or event.
- **Prescription approval** (`ApprovePrescription`) creates ordinary **Pending** felling orders only, with v23 provenance (prescription id, coverage unit, Crop Tree, reason), one `OrderCreated` event each carrying `prescriptionId`, and flips the record to `Approved` with the current year. It never approves, funds, spends cash or executes. `ApprovePendingWork` (the Work Plan) is untouched and remains the commitment gate; existing cash/contractor/execution rules apply to the generated orders unchanged.
- **MFM-06 v23 is reused**: existing record, order and event fields; no new schema member, no version bump, no validator edit.

## Proof of the inherited MFM-06 condition (validate before write)

Before any order exists, `ApprovePrescription` (1) runs `Build`, which validates every proposed order through `ValidateLinks`; (2) resolves every target to a live fellable tree; (3) re-reads the world and runs `ValidateLinks` again on every order immediately before the first `CreateFellingOrder`; and only then (4) writes, inside a try/catch that rolls back orders, events, id allocators and the record. Static offline checks pin that textual order (F4/F6).

`ValidateLink` fails closed, each with a distinct bounded reason (≤ 220 chars, ids clipped), on: missing/different prescription; rule-version or calibration mismatch; prescription not waiting for approval; source rack / anchor / sample Crop Trees / sample removals changed; sample not derivable; coverage unreadable; unknown unit, the sample unit, a not-covered unit, a covered-but-unaccepted unit; target missing, not in the unit, not an eligible Sitka adult, already marked or ordered by the player; Crop Tree missing, not a confirmed living Sitka Crop Tree, a sample Crop Tree, or in an unaccepted unit; unknown reason code (including 0); and finally a link the prescription would not itself generate now. A single invalid link fails the whole list.

- Offline: 125 checks incl. D1–D39 for exactly these cases, E1–E5 (all-or-nothing `Build`: failures return zero orders). Reproducible, two-process byte-identical.
- Mutation: 11 deliberate mutations of the production check (skip target-in-unit, crop-confirmed, unit-accepted, in-proposal, manual-state, sample-changed, safeguard, open-order hold, implicit acceptance, reason code offset, reason 0) were each **caught** by the check. (Throwaway; not committed.)
- Live: `CheckPrescriptionLink` refusals for missing prescription, unknown unit, sample unit, no Crop Tree, sample Crop Tree, unconfirmed tree, player-ordered tree, reason 0, unknown tree, and a Crop Tree un-designated after the preview; six corrupted stored records (rule, calibration, source rack, sample Crop Trees, no accepted unit, already approved) each refused whole with the world identity (WorldHash, scenario JSON, marks, Crop Trees, order/event counts, allocator, cash) **unchanged** and no provenance order present.

## Work Plan leakage / bypass negative tests (all PASS)

Live (Unity, 81 proposals around 16 confirmed Crop Trees on the Enlarged80 Rack V2 world after executing the sample release through production APIs):

- Opening the Work Plan with an Accepted prescription adds no order; approving it reports "no valid pending work" and commits nothing; the proposal signature is unchanged by both.
- Proposed trees carry no Fell mark/Crop designation; no work order is open while only proposals exist; previewing changes nothing.
- A manual Fell mark on a proposed tree: held back, surfaced, becomes exactly one ordinary pending order (`prescriptionId` 0) when the Work Plan opens, and is untouched by prescription approval.
- Prescription approval: exactly the previewed trees → Pending orders, none Approved/Completed, no tree felled, no cash spent, year unchanged, no Fell mark created, approving twice refused.
- Only then ordinary `ApprovePendingWork` approves them (together with the player's own order); approved is not executed; a later ordinary `AdvanceYear` resolved the 77 tasks by the existing economy.
- Static: the Work Plan methods (`SetWorkPlanOpen`, `OpenWorkPlan`, `AddMarkedTreesToWorkPlan`, `ApprovePendingWork`) and the marking manager reference no prescription/proposal code; the writer never touches `ScenarioWorkStatus.Approved`, cash, `ApprovePendingWork`, `AdvanceYear` or marks.

## Unity / runtime and save-provenance results

Evidence root: `/media/jer/Files/CCF/Verification/MFM-07-Qualification-20261029-r5/` (earlier `…-r2/-r3/-r4` and the first run are failed or superseded attempts, kept).

| Gate | Result |
|---|---|
| compile/import | `MFM06_COMPILE_PASS` saveVersion=23 |
| **MFM-07 runtime gate** (`MfmApprovalBoundaryUnityGate`) | `MFM07_UNITY_PASS checks=106`, 106 s, normal exit 0 |
| MFM-06 real-controller save/load (regression) | `MFM06_UNITY_PASS checks=159` |
| hash dump (regression) | 18 lines, **byte-identical** to the MFM-06 integration candidate dump (`MFM-06-Integration-20261029/candidate/03-hash-dump/hash-dump.txt`) — no anchor rebaseline |
| interaction / Legacy compatibility (regression) | `SCENARIO_ONE_INTERACTION_VERIFY_PASS` |

Every runner result: `pass:true`, host saves unchanged, harness/licence copies removed, lane clean. Save/provenance inside the MFM-07 gate: after approval and Work Plan approval, a real `Save()` → file carries the provenance → `CanLoad` → `Load()`: whole-world identity and WorldHash equal; generated orders reload with status, coverage unit, Crop Tree and reason; the Approved record, approval year and allocator (`NextPrescriptionId` 2) and event linkage reload; reloading creates no new pending work and no proposal; the post-execution world validates and reloads. The isolated slot was deleted by the gate.

## Residuals and decisions for the Manager

1. **Partial world adapter.** `TryReadPrescriptionWorld` supplies live stems, orders, confirmed rack geometry and stored coverage choices but **no infrastructure masks or management features** (same labelled partial adapter as the MFM-04/05 Unity gate). The full production mask/feature adapter, and its calibration impact, remain open and should be owned before MFM-08/09 rely on coverage in a shipped flow.
2. **No dedicated approval event type.** History records one `OrderCreated` event per order with `prescriptionId`; the approval itself is on the record (`approvalState/approvalYear`). A new `ScenarioManagementEventType` would extend an append-only enum and touch history consumers/validator; not done (Manager call).
3. **Crop Tree confirmation** is read from the existing durable Crop Tree designation (`ForestTree.IsCropTree`); sample Crop Trees are excluded. No second designation system, no bulk or automatic confirmation.
4. **Coverage exclusion by player marks.** A player Fell mark inside a unit makes coverage exclude the whole unit for player management (observed live: R2C), so that unit's other proposals disappear too. This is surfaced (`UnitNoLongerCovered`) and safe, but the player-facing wording and whether a single mark should drop a unit is an MFM-08 UX/product question.
5. Gate reliance: the runtime gate drives marks/crop designations through `ForestTreeMarkingManager` and the real Work Plan manager APIs, not rendered UI or native input; MFM-08 will need rendered review.
6. Not run (outside scope): the full 24-gate sweep, N3, Frozen Reference and Windows. The MFM-07 diff touches no existing production file, and the hash/save/interaction regressions above pass.
7. Not pushed: this branch exists only locally in `/media/jer/Videos/CCF-MFM-07` until the Manager/owner publishes it.

## Exact next dependency

Manager review of this handoff. MFM-08 (map/3D review UX) follows acceptance and integration of MFM-07, plus resolution of residual 1; MFM-09 additionally needs S1-D T-04/T-05. #121 is released to HANDED-OFF; the Unity lock is free.
