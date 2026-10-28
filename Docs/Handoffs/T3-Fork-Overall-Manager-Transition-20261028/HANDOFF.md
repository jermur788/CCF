# T3 Fork — Overall Manager Transition Handoff

**Date:** 2026-10-28
**Status:** CURRENT MANAGER HANDOFF
**Owner:** Overall Manager

## Purpose

This is the durable continuation packet for a new Overall Manager chat. It is intended to avoid replaying the prior conversation. When this file conflicts with older roadmap/current-state snapshots, prefer this handoff plus inspected repository/runtime truth and the governing milestone specs.

## New-manager startup order

1. Read the Project Instructions for the T3 Fork project.
2. Read this handoff.
3. Read the active governing specs/tasks listed below.
4. Inspect the current CCF and T3 repository heads before acting.
5. Inspect active task/PR/handoff evidence rather than trusting worker claims.
6. Continue automatically through already-authorized review/repair/integration/next-task gates; stop only for a genuine product decision or other configured stop condition.

## Product / authority invariant

The fork is a small, upstream-friendly T3 fork providing a provider-neutral Project Runtime. Important project truth must persist independently of provider/model/conversation. Workspace mode is the simple single/few-provider project-workspace experience; Development mode adds multi-provider routing/execution/review without becoming a prerequisite for basic project use.

Authority remains:

- product intent: latest user instruction > accepted decisions > current milestone > governing spec;
- implementation truth: inspected repo/runtime > reproducible tests/evidence > implementation docs > worker claims;
- handoffs are continuation context, never proof;
- CCF remains authoritative for CCF ownership/review/integration/verification outcomes.

## Canonical repository checkpoints

### T3 fork

Repository: `jermur788/t3code`

Current integrated `main` on handoff:

`02a3d7a3a3129bdc80b283e991fd8bf9347e0f91`

Commit message: `docs: mark Documentation V1 repairs integrated`

Do not assume feature branches are merged merely because implementation exists. Inspect current branches/PRs before writes.

### CCF / manager-design repository

Repository: `jermur788/CCF`

Current `main` on handoff:

`a3ba18c96d78fc721d62d8df8dddc6a576d843d8`

Commit message: `manager: record ARM-010 dispatch`

CCF now contains the approved Adaptive Model Routing V1 governing package and cross-repo manager/task state.

## Verified T3 foundation

The following foundation is already accepted and should not be reopened without new contradictory evidence:

- Project Runtime V1 — VERIFIED.
- Development Profiles V1 — VERIFIED.
- Task/Handoff State V1 — VERIFIED.
- Fork Project Home — integrated.
- Runtime Task Routing & Execution V1 — VERIFIED and integrated.

RTE invariant remains:

> Either a worker launch is durably recoverable as one execution attempt, or no execution begins.

Routing precedence remains:

1. explicit one-run override;
2. task routing;
3. project routing;
4. Development Profile routing;
5. normal T3 behavior.

A routing preference is not authorization.

## Active lane A — Workspace Modes V1

### State

- WSM-010 — DESIGN PASS.
- WSM-020 — IMPLEMENTED.
- WSM-030 — STATIC PASS / OPERATIONAL BLOCKER.
- Workspace Modes V1 — NOT INTEGRATED / NOT VERIFIED.

Implementation PR:

`jermur788/t3code` PR #7 — `feat(web): add Workspace and Development interface modes`

Current PR state on handoff:

- open;
- draft;
- base `wsm/wsm-010-ux-spec`;
- head `wsm/wsm-020-interface-shell`;
- head `a79373dbd8ee4899e0dc0aee89738512a361a898`;
- mergeable;
- intentionally unmerged.

### Implemented behavior

- device-local `workspace | development` interface preference;
- first-use choice;
- simplified Workspace Project Home;
- Development mode preserves the existing Fork Home / RTE-V1 experience;
- Workspace Project Settings keep essential settings, Instructions and Sources visible and collapse Development Profile/routing/Runtime/task-execution controls under Advanced;
- deep links auto-reveal Advanced targets;
- Workspace settings navigation/search hides Development Profiles from normal discovery while direct/Advanced access still works;
- Settings → General exposes the T3 Fork interface mode;
- Workspace Home includes Classic T3 Home plus compact Advanced summaries for Development Profile, Runtime, Project State and Tasks/Handoffs.

### WSM-030 blocker

The previous manager could perform source/static review but could not execute a runnable T3 web/browser environment. GitHub Actions/status checks are not available for the fork branch. Therefore these remain unproven:

- browser/unit tests required by WSM;
- web typecheck/build/changed-file checks;
- rendered Workspace and Development inspection;
- desktop/narrow/mobile widths;
- mode-switch state preservation;
- zero write-RPC behavior from interface switching;
- Advanced/deep-link behavior;
- Development-mode regression;
- independent rendered review.

Canonical in-repo handoff:

`docs/internals/wsm-030-qualification-handoff.md`

### Exact next action

If a runnable-agent handoff/evidence now exists, consume it first. Inspect actual branch evidence; repair routine defects; require fresh independent review; integrate WSM-010/020 only after WSM-030 runtime/rendered qualification passes. Do not claim VERIFIED from static evidence.

No user product decision is currently required.

## Active lane B — Documentation V1

### State

Documentation repairs are integrated on T3 `main`, but the documentation milestone is still:

**INTEGRATED / RUNTIME QUALIFICATION PENDING**

Canonical docs:

- `docs/manual.md`
- `docs/reference.md`

Durable repair handoff:

- `docs/internals/documentation-v1-review-repair-handoff.md`

### Remaining fresh-user gate

Run from a clean/fresh environment using only the manual/reference and prove:

1. source startup via `vp run dev` and printed `pairingUrl:`;
2. configured/enabled real provider;
3. explicit task Worker preference or project default model;
4. implementation task plus required Runtime handoff;
5. separate fresh review task/worker;
6. restart/persistence continuation;
7. project with no default model produces documented `no_default_model` behavior and recovery;
8. deleted-worker `reconcile_required` recovery: Recheck, then Cancel only after worker absence is confirmed;
9. approval-required worker path if practical.

This remains an OPERATIONAL verification gate, not a product decision. Do not mark `T3 FORK DOCUMENTATION V1 VERIFIED` until this end-to-end qualification exists.

## Active lane C — Adaptive Model Routing V1

### Governing design status

**APPROVED / MANAGER REVIEW PASS / INTEGRATED**

CCF design PR #62 was squash-merged. The approved design integration commit is:

`2e3138f380021d0973231af310dab731b3aa880d`

Claim #61 is closed `[DONE]` as the completed design task.

Governing artifacts on CCF `main`:

- `specs/t3-adaptive-model-routing-v1/spec.md`
- `specs/t3-adaptive-model-routing-v1/plan.md`
- `specs/t3-adaptive-model-routing-v1/tasks.md`
- `specs/t3-adaptive-model-routing-v1/checklists/requirements.md`
- `Docs/Handoffs/T3-Fork-Adaptive-Model-Routing-20261028/MANAGER-REVIEW.md`
- `Docs/Handoffs/T3-Fork-Adaptive-Model-Routing-20261028/MANAGER-INTEGRATION.md`

### Accepted V1 product decisions

1. Raw task/execution observations and learned outcome aggregates are project-local by default; subscription capacity is environment/provider-capacity-pool scoped. Broader cross-project outcome learning is deferred unless explicitly opted into or accepted later.
2. Recommendations are advisory-only in V1. Automatic application is deferred beyond V1 and requires a later product/spec decision.
3. Raw adaptive telemetry has no automatic expiry or lossy roll-up in V1; it remains local until explicit deletion or a later accepted retention policy. Deletion must not remove authoritative Runtime/CCF truth.
4. Opaque local capacity-pool IDs are canonical/default. Optional user-defined local display aliases are allowed; provider-derived personal/billing identity is not required or stored by default.

### Fixed constraints

- subscription-only / no API billing;
- no API keys for adaptive routing;
- no usage credits, overage, pay-as-you-go, auto-recharge, purchases/upgrades;
- Fable usage-credit/overage-like paths remain disabled;
- no provider region/privacy mutation;
- exact provider/model/options/runtime/interaction/workspace provenance where available;
- privacy-safe account/capacity-pool identity;
- provider-native quota/reset observations when available, otherwise explicit unknown;
- scheduled, observed bonus and inferred reset events remain distinct;
- OpenAI bonus-reset observations are stochastic evidence, not guaranteed entitlement;
- predicted difficulty is persisted before recommendation;
- realized difficulty comes later from execution/review/rework/verification evidence;
- worker terminal status/handoff is not accepted-work proof;
- existing T3 routing precedence, eligibility and execution path remain authoritative;
- controlled experiments must preserve one-writer/side-effect safety;
- CCF ownership/review/integration/verification authority is unchanged.

### Current implementation task — ARM-010

CCF issue #63:

`[READY] ARM-010 — T3 Adaptive contracts and persistence skeleton`

`t3code` GitHub Issues are disabled, so issue #63 in CCF is the durable cross-repo manager/task record. Implementation target is `jermur788/t3code`.

Dispatch base recorded for ARM-010:

`t3code main @ 02a3d7a3a3129bdc80b283e991fd8bf9347e0f91`

Before implementation, refresh the actual T3 main/upstream state and active writers. If main moved, use current implementation truth and reconcile only material design changes back to the approved spec.

ARM-010 scope is limited to:

- adaptive IDs/contracts/versioning;
- prediction/realized-difficulty record shapes;
- execution-observation references linked to Runtime task/attempt identity;
- privacy-safe capacity pool/snapshot/window/reset record shapes;
- outcome-reconciliation record shapes;
- recommendation-receipt and experiment-assignment record shapes;
- environment-DB repository/service persistence skeleton;
- append-only fork migration in the existing fail-closed migration sequence;
- adaptive telemetry deletion boundary that cannot delete Runtime/CCF truth.

ARM-010 explicitly excludes:

- recommendation scoring;
- provider quota adapters;
- UI/operator surfaces;
- automatic routing;
- routing precedence or eligibility changes;
- Runtime launch/orchestration semantic changes;
- API/credit/billing paths;
- CCF authority changes.

Required ARM-010 completion state:

**STATIC PASS / READY FOR MIGRATION REVIEW**, followed by fresh Claude/Codex-class migration/CAS/privacy review before ARM-020.

Do not advance to ARM-020 from a worker self-report alone.

## Immediate manager priorities

### Priority 1 — Consume/dispatch ARM-010

Inspect CCF issue #63 first. If a worker has already claimed/updated it, consume the handoff and inspect T3 repo evidence. If it is still READY and no writer owns the lane, dispatch ARM-010 to the least-cost capable implementation worker (DeepSeek/OpenCode is appropriate for the bounded contract/migration lane) with a mandatory durable handoff. Mark the task/claim RUNNING before substantive implementation if the issue/task workflow supports it.

After implementation:

- inspect branch/worktree/base/status;
- inspect migration ordering and current upstream overlap;
- run/reconcile contract/migration tests;
- route a fresh strong migration/CAS/privacy review;
- repair/retest routine findings automatically;
- integrate only after acceptance and then issue ARM-020.

### Priority 2 — Consume WSM-030 executable qualification if it arrives

WSM-030 can proceed in parallel with ARM-010 only with isolated ownership/worktrees; its web UI lane does not authorize edits to ARM migration/contract files. If runtime/rendered qualification passes, perform independent review and integrate. If evidence is missing, retain OPERATIONAL BLOCKER rather than weakening the gate.

### Priority 3 — Documentation fresh-user qualification

If a runnable-environment agent produces evidence, reconcile it against the manual/reference and the required troubleshooting cases. Repair docs and rerun affected paths as needed. Mark Documentation V1 VERIFIED only after the fresh-user gate passes.

## Separate dashboard boundary

The development-operations dashboard / Project Execution View is a separate OSS project. Do not conflate it with T3 Runtime state. T3 owns authoritative Runtime/orchestration state; the dashboard owns visualization/aggregation/monitoring/operator views and later documented API actions. New Runtime/execution state should live in reusable T3 domain services rather than dashboard-only handlers.

A previous PEV lane was IMPLEMENTED but not VERIFIED; its device/review/integration gates are separate from the current T3 fork manager priorities.

## Repo safety / continuation rules

Before substantial writable work:

- confirm repository, branch/worktree, base, HEAD and `git status`;
- identify unrelated changes/active writers;
- preserve other workers' uncommitted work;
- do not stash/reset/discard unrelated work;
- use isolated worktrees for parallel writable lanes where practical;
- one writer per overlapping code/migration lane.

After every substantial worker handoff:

1. inspect repo/runtime evidence;
2. reconcile with governing spec/task;
3. classify defects/blockers precisely;
4. repair/re-review/retest routine issues;
5. update canonical task/project state;
6. integrate when already authorized and acceptance is satisfied;
7. issue the next authorized bounded task;
8. stop only for a genuine user decision.

Use precise verification states: IMPLEMENTED, STATIC PASS, RUNTIME PASS, REVIEW PASS, INTEGRATED, VERIFIED, BLOCKED, DEFERRED.

## User action / stop state

No current product decision is required from the user.

Do not stop merely to report a worker completion. Continue through the next already-authorized gate. Stop only for a genuine product choice, material architecture/scope change, irreversible/destructive decision, unresolved security/integration conflict, or material cost decision.

## First message for the new manager

Recommended startup instruction to paste into a fresh manager chat:

> Act as Overall Manager for the T3 Fork project. Start from the Project Instructions and `Docs/Handoffs/T3-Fork-Overall-Manager-Transition-20261028/HANDOFF.md` on `jermur788/CCF` main. Inspect current CCF issue #63 and current `jermur788/t3code` main/PR #7 before acting. Continue automatically from canonical state: ARM-010 is the next authorized Adaptive Model Routing task; WSM-030 and Documentation V1 remain runtime-qualification gates. Preserve repo safety, mandatory handoffs, upstream compatibility, subscription-only/no-API constraints, and CCF verification authority. Do not rely on the previous chat as project state.