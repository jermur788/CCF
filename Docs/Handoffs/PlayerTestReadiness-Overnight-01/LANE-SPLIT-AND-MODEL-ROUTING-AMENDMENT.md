# PLAYER-TEST-OVERNIGHT-01 — Lane split and model-routing amendment

## Authority

This amendment records the user's 2026-10-29 routing instruction for the remaining work under parent #91. It changes orchestration/routing only; it creates no gameplay requirement and does not supersede accepted product authority.

The original mandatory first Claude A / Opus-medium start audit, if already completed, remains valid historical execution. **Prospectively from this amendment**, use the model ladder below.

## Orchestrator

**DeepSeek remains the single orchestration/integration lead under parent claim #91.**

DeepSeek must keep the programme moving across two execution lanes:

- **Browser/Manager lane:** claim #105; packet `BROWSER-LANE-HANDOFF.md`.
- **T3 Code local lane:** claim #106; packet `T3-CODE-LANE-HANDOFF.md`.

The lane claims do not replace feature claims or `Docs/Project/coordination/active-tasks.md` ownership.

## Model ladder

Use the least expensive capable level while preserving required independence:

1. **Claude Sonnet 5.5 Medium** — default for new subtasks.
2. **Claude Opus 5.5 Low** — first escalation when Sonnet is uncertain, fails materially, encounters difficult architecture/save/reconciliation work, or when stronger review is justified.
3. Continue escalating Claude capability only when there is a recorded reason.
4. **OpenAI 6.1 Sol** — authorised for implementation, qualification, difficult reasoning and cross-provider independent review.

For high-risk save/reference/determinism changes, use cross-provider independence where practical. The implementing thread or its helper does not count as the independent reviewer.

## Subtask tracking requirement

DeepSeek must maintain a live subtask ledger in #91 comments. After every dispatch and every returned handoff, record at minimum:

| Field | Required |
|---|---|
| Subtask | plain-English task name + code |
| Child claim | issue number/status |
| Lane | Browser or T3 Code |
| Model/provider | exact worker configuration |
| Branch/worktree | exact writable lane or `read-only browser` |
| Starting/exact candidate SHA | as applicable |
| Dependency | what blocks/unblocks it |
| Result | IMPLEMENTED / REVIEWED / INTEGRATED / VERIFIED kept distinct |
| Handoff | exact GitHub Markdown path |
| Next action | immediately executable next step or explicit blocker |

`Docs/Project/coordination/active-tasks.md` remains the live single-writer ownership and Unity-lock authority. Do not create a competing ownership ledger.

## Keep-work-moving rule

After a subtask returns:

1. inspect the handoff/evidence;
2. issue the required verdict;
3. update/close the child claim as appropriate;
4. update #91 ledger;
5. dispatch the next dependency-ready subtask to browser #105 or T3 Code #106 immediately;
6. if one lane is blocked, continue independent safe work in the other lane;
7. stop only for a genuine product decision, ownership conflict, mandatory independent review, unsafe integration, user playtest, or other material ambiguity that accepted authority cannot resolve.

Do not ask the user to manually coordinate ordinary sequencing.

## Current routing split

### Browser lane

Best for:

- GitHub/repo inspection and evidence reconciliation;
- source/static independent review;
- handoff verdicts;
- version/save sequencing review without local execution;
- issue/claim/task-packet management;
- diagnosing whether a reported mismatch is likely harness/fixture/source-lineage versus production, then writing the bounded T3 packet;
- checking player-facing copy/spec conformance from source;
- final independent checklist review after executable evidence exists.

Browser must not claim Unity/compiler/runtime/rendered/save-execution results it did not observe.

### T3 Code lane

Required for:

- production/verification C# edits;
- save-schema/version-pin changes;
- local worktrees/dirty-state management;
- Unity import/compile/runtime/batchmode;
- rendered captures;
- save/load/hash execution;
- serialized assets;
- executable integration qualification;
- Tier-2/Tier-3 execution and exact candidate assembly.

## Current immediate split

Browser #105 should first:

1. review S1-C T-05 v22 lineage and stale v21 pin relabel boundary;
2. inspect S1-D T-03 RackLesson candidate/base mismatch and publish a bounded reconciliation packet;
3. source-review PARITY-07 static return;
4. source-review MFM-04/05 static returns;
5. keep #91 DAG/current claims reconciled.

T3 Code #106 should consume those review outputs and:

1. close S1-C T-05 v22 Unity/save/hash/rendered gates and integrate if approved;
2. reconcile and Unity-qualify S1-D T-03;
3. close PARITY-07 Unity/rendered residual;
4. close MFM-04/05 Unity residuals;
5. continue approved S1-C, S1-D, parity and MFM execution through final Tier-3.

## No-build boundary

**No Windows build.** The user must personally playtest the exact Scenario One candidate and explicitly authorise a Windows build first.

## Amendment token

`PLAYER-TEST PROGRAMME SPLIT INTO BROWSER + T3 CODE LANES — DEEPSEEK ORCHESTRATION CONTINUES`