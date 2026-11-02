# Scenario One Objectives Overflow Repair — Local Execution Handoff

Date: 2026-11-02
Claim: #224
Parent Manager lane: #200
Local orchestration: #203
Binding packet: `Docs/Handoffs/ScenarioOne-Objectives-Overflow-Repair/TASK-PACKET.md`

## Purpose

Execute the already-approved bounded local Unity/UI repair for the Scenario One Objectives-window overflow. Browser/GitHub work can inspect and review the repository, but it cannot establish the required rendered reproduction, Unity behavior, regression evidence, or machine-wide Unity lock state. This handoff therefore routes the task to the local implementation/Unity lane without changing its product scope.

## Difficulty

**Medium.** The defect is narrow, but success depends on exact-candidate Unity reproduction at two resolutions, a minimal player-facing layout repair, rendered regressions, Unity lock discipline, and independent review.

## Best-suited model/provider

- Primary implementation/verification: **SOL/CODEX high-reasoning local lane** under #203 orchestration.
- Independent review: **Claude Sonnet/Opus or DeepSeek** as a separate read-only review pass after implementation and rendered evidence are complete.

## Exact authority and starting state

Repository: `jermur788/CCF`

Exact current integration candidate:

`bbcefe06e0d6a0dce38f6eb01d622dca7677495c`

Production bytes are unchanged from the prior Scenario One production candidate; #223 verification-debt repairs and Manager review are integrated on top.

Current accepted guided-v2 state after #223:

- 533 PASS
- 2 FAIL
- both remaining failures are the `current objective fits` family at 1280×720 and 1920×1080.

Binding task packet:

https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ScenarioOne-Objectives-Overflow-Repair/TASK-PACKET.md

Execution claim:

https://github.com/jermur788/CCF/issues/224

Parent local orchestrator:

https://github.com/jermur788/CCF/issues/203

## Exact lane

Suggested branch:

`fix/scenario-one-objectives-overflow-v1`

Use one dedicated clean local worktree assigned by #203. Do not work in another owner's lane and do not reuse an unrelated dirty worktree.

Before production edits:

1. refresh `Docs/Project/coordination/active-tasks.md`;
2. confirm #224 is still the sole blocking repair before MFM-12 and is still available to claim;
3. confirm exact starting HEAD `bbcefe06e0d6a0dce38f6eb01d622dca7677495c`;
4. confirm clean worktree and ownership boundaries;
5. inspect current Editor state and acquire the machine-wide Unity lock before heavy Unity phases;
6. update issue #224 to `[RUNNING]` with provider/model, branch, worktree and starting HEAD.

## Required execution

### Phase A — reproduce before editing

On exact starting candidate, reproduce the `current objective fits` failures at both:

- 1280×720;
- 1920×1080.

Retain rendered evidence. If the failure does not reproduce, stop and return the evidence to Browser Overall rather than making a speculative UI change.

### Phase B — bounded production repair

If reproduced, inspect the rendered layout and authoritative objective content, then make the smallest robust fix.

Preferred solution classes:

- reflow;
- scroll behavior;
- bounded container sizing;
- other narrow layout correction that preserves all accepted objective content.

Do not hide, truncate, remove, or semantically shorten required objective content merely to make the assertion green.

### Phase C — regressions

Rerun guided-v2 at both target resolutions.

If overlapping UI surfaces are touched, rerun proportionate:

- Combined HUD rendered regressions;
- PARITY-07 rendered regressions.

If Rack/minimap surfaces are touched, rerun the relevant Rack confirmation/minimap gate.

Preserve and verify the already-integrated #223 verification-debt repairs.

### Phase D — independent review

Obtain an independent read-only review of:

- the production diff;
- before/after rendered evidence;
- regression results;
- whether the repair remained bounded and preserved objective semantics;
- whether any accepted #199, Rack V2, MFM-08/09, save/schema, ecology/economy, or protected-reference behavior was affected.

## Hard exclusions

Do not:

- weaken/delete/skip `current objective fits` or equivalent fit assertions;
- alter accepted objective semantics without Manager authority;
- change Rack or Multi-Scale Forestry Management workflow semantics;
- change save/schema behavior;
- change protected anchors or frozen Reference Future v1 compatibility;
- change ecology/economy/calibration;
- undo #223 verification repairs;
- touch #194;
- create a Windows build.

## Stop conditions

Stop and return to Browser Overall if:

- exact starting candidate cannot be established;
- ownership/Unity lock conflicts exist;
- the defect does not reproduce;
- the only plausible fix requires content/product changes, a new navigation model, or substantial UX redesign;
- a repair would cross Rack/MFM/save/reference/ecology/economy boundaries;
- regressions expose a new genuine production defect;
- independent review finds a blocker.

## Required durable return

Publish exactly:

`Docs/Handoffs/ScenarioOne-Objectives-Overflow-Repair/HANDOFF.md`

Include:

- exact starting and final HEAD;
- branch/worktree;
- reproduction evidence at both resolutions;
- production diff and files changed;
- before/after rendered screenshots or equivalent retained rendered evidence;
- guided-v2 counts at both target resolutions;
- Combined/PARITY-07/Rack regressions as applicable;
- Unity lock lifecycle and cleanup state;
- independent reviewer/provider/model/verdict/findings;
- confirmation objective semantics were preserved;
- confirmation #223 repairs remain intact;
- whether the final production commit is ready for Manager integration.

Required success token:

`SCENARIO ONE OBJECTIVES OVERFLOW REPAIRED — READY FOR MANAGER REVIEW`

At handoff, mark issue #224 `[HANDED-OFF]` and link the durable result.

Do **not** start MFM-12/#225. Browser Overall must inspect, approve, and integrate #224 first, then repin MFM-12 to the resulting exact production candidate.

## Copy/paste dispatch

`CCF local orchestrator — execute https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ScenarioOne-Objectives-Overflow-Repair/LOCAL-EXECUTION-HANDOFF-20261102.md exactly as written. Work under claim #224 from exact candidate bbcefe06e0d6a0dce38f6eb01d622dca7677495c in a dedicated clean local worktree. Reproduce the Objectives-window current-objective-fit failures at 1280x720 and 1920x1080 before editing, make only the smallest bounded layout/reflow repair without weakening the assertion or changing objective semantics, rerun guided-v2 plus applicable Combined/PARITY-07/Rack rendered regressions, obtain independent review, publish the required HANDOFF.md, and do not start MFM-12 or make a Windows build.`
