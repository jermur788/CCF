# Browser Lane 04 — MFM-07 readiness and post-MFM-06 dependency audit

Parent: #91 PLAYER-TEST-OVERNIGHT-01
Claim: #120
Lane: Browser / Manager only
Status at publication: PREPARED FOR LIVE CHAT DISPATCH

## Purpose

Use a separate Game Dev browser chat to complete every non-local step needed to make MFM-07 executable as soon as MFM-06 integration (#113) returns. This lane may inspect GitHub, specs, handoffs and claims, publish management/review documents, reconcile dependencies and prepare exact execution packets. It must not perform or claim local C#/Unity/save/integration execution.

## Starting facts

- MFM-06 implementation claim #111 is handed off/closed.
- Exact reviewed production candidate: `13fd85452b8d3a55946fcb459af8b00e0326a579`.
- Independent Claude Opus 5.5 Low review round 2 is accepted by the Manager as satisfying the MFM-06 independent-review requirement.
- Extra cross-provider claim #112 is superseded/closed.
- #113 is the active MFM-06 integration/post-merge qualification task.
- Historical umbrella head `ac876a64969752a3864034521f42559671264b30` was local/unpushed; do not treat it as current truth without fresh evidence.

## Required browser work

1. Refresh #91, #106, #111/#112/#113, `Docs/Project/coordination/active-tasks.md`, and the approved Multi-Scale Forestry Management V1 spec/plan/tasks/checklist before substantive review.
2. Reconcile any stale issue/task-list wording with current evidence without altering product intent.
3. Audit MFM-07 against the approved architecture and the accepted MFM-06 review findings.
4. Ensure the MFM-07 execution packet explicitly requires **before any prescription-generated order is written, validate that order's coverage unit and Crop Tree against its prescription**.
5. Reconfirm the core boundaries:
   - proposals are not `TreeMarkType.Fell`;
   - prescription approval and Work Plan approval are distinct gates;
   - prescription approval creates pending ordinary work only;
   - no work is silently approved or executed;
   - manual player state takes precedence;
   - conflicts are surfaced rather than overwritten;
   - no broadleaf quantitative extrapolation is introduced;
   - MFM-06 v23 provenance is used, not duplicated.
6. Inspect likely shared-file ownership with remaining MFM, S1-C, S1-D and PARITY work. Publish a single-writer sequencing recommendation and stop on overlap that cannot be resolved from approved interfaces.
7. When #113 returns, inspect its published exact pre/post SHAs and post-merge gate evidence. Issue one Manager verdict: APPROVED / NEEDS CHANGES / REJECTED for proceeding to MFM-07.
8. If APPROVED, update or confirm the MFM-07 claim/packet with the exact new umbrella base. If NEEDS CHANGES/REJECTED, publish the bounded repair packet instead.
9. Continue with any additional browser-safe review/packet work that becomes clear, but do not create overlapping production ownership.

## Hard boundary

No production or verification C# edits. No local worktree claims. No Unity, compiler/runtime, save execution, rendered evidence or branch integration. No Windows build.

## Model routing

Use the browser chat's current Manager model directly. If a delegated browser reviewer is useful, default to Claude Sonnet 5.5 Medium; escalate to Opus 5.5 Low only with a recorded reason. OpenAI 6.1 Sol may be used for suitable independent review where it materially improves confidence; it is not a mandatory gate unless a spec explicitly says so.

## Exit

Publish a durable result under `Docs/Handoffs/PlayerTestReadiness-Overnight-01/` and update #120/#91.

Success token:

`MFM-07 READY FOR CODE EXECUTION — DEPENDENCIES AND OWNERSHIP RECONCILED`

## Copy/paste dispatch

Run claim #120 from `Docs/Handoffs/PlayerTestReadiness-Overnight-01/BROWSER-LANE-04-MFM07-READINESS.md`. Refresh #91/#106/#111/#112/#113, live coordination and the approved MFM spec first. Work only on browser-safe MFM-07 readiness, dependency/ownership reconciliation and returned-handoff review. When #113 returns, issue the Manager go/no-go verdict for MFM-07 and pin its exact integration base. No C#/Unity/save/integration execution and no Windows build.