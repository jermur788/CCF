# Local Execution Handoff — MFM-12 Fit-Replay Review Integration

**Target:** CCF local implementation lane under #203  
**Parent qualification:** #225 MFM-12  
**Repair claim:** create/use the dedicated fit-replay repair claim linked by Browser Overall  
**Binding packet:** `Docs/Handoffs/MFM-12-Fit-Replay-Repair/TASK-PACKET.md`  
**Manager stop review:** `Docs/Handoffs/MFM-12-Integrated-Qualification/MANAGER-REVIEW-STOP-20261102.md`  
**Difficulty:** Medium/High — bounded existing-core integration across manager/UI/approval plus Unity/rendered regressions.  
**Best-suited primary:** SOL/CODEX high-reasoning local implementation lane.  
**Independent review:** fresh cross-provider reviewer, preferably Claude Sonnet 5.5 or equivalent.  
**Suggested branch:** `fix/mfm-fit-replay-review-v1`  
**Worktree:** dedicated clean lane assigned by #203.  
**User action required:** paste the dispatch supplied by Browser Overall into the local orchestrator.

## Purpose

Fix only MFM-12 blocking defect S1: the existing deterministic `ScenarioOnePrescription.CheckFit` replay is not connected to the live player Treatment Review/approval flow, so the spec-required source-sample agreement/disagreement and Share/removal consequences are not shown before wider prescription approval.

The pure core already exists and passed its offline gate. Reuse it. Do not redesign the prescription algorithm.

## Preflight

Before production edits:

1. refresh `Docs/Project/coordination/active-tasks.md` and the repair claim;
2. verify the exact current `integration/scenario-one-player-test-ready-v1` head supplied by Browser Overall;
3. create/use only the dedicated repair branch/worktree;
4. verify worktree cleanliness and unrelated dirty work ownership;
5. inspect current Unity Editor processes and the machine-wide Unity lock;
6. mark the repair claim `[RUNNING]` with provider/model, branch, worktree and starting HEAD.

Do not start or reopen #225 as a qualification run during the repair. #225 remains blocked until Browser Overall accepts/integrates the repair and freshly repins it.

## Required implementation

Follow the binding packet. In particular:

- obtain the fit replay from the current authoritative sample/world through domain/manager code, not by duplicating rules in the UI;
- show an agreement/disagreement summary;
- expose player-only and rule-only sample decisions individually enough to inspect them;
- show relevant player-vs-rule Share/removal consequences;
- present fit as an honesty comparison, not a forestry-correctness score;
- block wider prescription approval when a valid current fit replay is unavailable/invalid;
- keep prescription approval distinct from Work Plan approval;
- preserve proposals as proposals, player judgement and all existing candidate/coverage semantics.

Use the smallest architecture-consistent presentation inside the existing Treatment Review. Do not invent a new navigation model.

## Verification-tool debt allowed in this lane

The stopped MFM-12 run exposed three stale verification-only items. Mechanically reconcile them only where accepted current authority is unambiguous:

- MFM Coverage World Unity save-version assertion v23 → accepted v24;
- MFM save-provenance mutation M14 target v23 → accepted v24 so M14–M25 can execute;
- MFM-08 treatment-review boundary pin/allowed lineage against later accepted integration.

Do not weaken assertions. If a change requires guessing product intent, leave it red and return it to Browser Overall.

## Required verification

Run the focused pure/static, Unity/runtime and rendered obligations in the packet, including fit mapping/gating cases, live Treatment Review, 1280×720 and 1920×1080 rendered evidence, and proportionate guided-v2 / Combined HUD / PARITY-07 / MFM treatment-review/approval regressions.

If Rack/minimap source or style is touched, rerun its relevant rendered gate. Otherwise document why separate Rack/minimap rerun is unnecessary.

Obtain fresh independent cross-provider review before handoff.

## Stop conditions

Stop without unrelated repair if the core itself is wrong, save/schema change appears necessary, substantial UX redesign is required, protected anchors move, a new production defect appears, verification debt cannot be mechanically reconciled, or independent review blocks.

No Windows build. #194 NO ACTION.

## Durable return

Publish exactly:

`Docs/Handoffs/MFM-12-Fit-Replay-Repair/HANDOFF.md`

Success token for the repair only:

`MFM FIT REPLAY REPAIR COMPLETE — READY FOR MANAGER REVIEW`

At completion mark the repair claim `[HANDED-OFF]`, link the handoff and stop. Do not restart MFM-12 yourself.