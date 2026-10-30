# PLAYER-TEST-OVERNIGHT-01 — Overnight continuation, 2026-10-30

**Parent claim:** #91  
**Browser lane:** #162  
**Local C#/Unity umbrella:** #106  
**Label:** MIXED — browser-separated orchestration/review + local code + serialized Unity  
**Difficulty:** High — multi-claim dependency management, save/schema risk, integration, Unity serialization and provider failover  
**Orchestrator:** DeepSeek  
**Observed `main` before publication:** `8fe366cb317d3d07ef1bcddeaa0d6ae3ae48b9bf`  
**Current combined integration lineage before this handoff:** `cfdc93bf3943125d21570bc5749a9338f52d1cce`  
**Context source snapshot:** `d05b27ad376ce02748ee4703429ad3ff66a65542`

## Purpose

Continue the accepted Scenario One programme through as many safe, dependency-ready tasks as possible overnight without waiting for routine user input. The target remains one exact integrated, Tier-3-qualified candidate ready for the user's personal Scenario One playtest.

This continues #91; it does not create a competing orchestration programme. Reuse existing child claims and handoffs where scope still matches. Create one claim for every new substantive dispatched task that lacks one.

**NO WINDOWS BUILD.** The user must personally playtest the exact Scenario One candidate and explicitly authorise a Windows build afterward.

## Authority and supersession

Use the normal product authority order:

1. latest direct user instruction;
2. `Docs/Project/decision-log.md`;
3. `Docs/Project/current-milestone.md`;
4. `Docs/Project/game-brief.md`;
5. approved task-level specs.

Implementation truth is inspected repository / verified Unity state, then `Docs/Project/unity-project-overview.md`.

This continuation supersedes the **old provider-routing and stale queue sections** in `Docs/Handoffs/PlayerTestReadiness-Overnight-01/TASK-PACKET.md`, #91 and #106. It does not supersede accepted product requirements, verification gates, ownership rules or feature specs.

The old rule requiring Claude Opus as the first substantive delegate is cancelled. Claude authentication/quota problems must not stop the overnight programme when DeepSeek can safely continue.

At every task transition, refresh `Docs/Project/coordination/active-tasks.md`, relevant claims/comments, exact branch/worktree state, current `main`, and the current integration ref. Live coordination and durable handoffs outrank stale issue-title/body wording.

## Provider policy

### Default routing

**DeepSeek is the overnight orchestrator and default worker/reviewer/integrator. Use DeepSeek for every subtask it can reasonably perform.**

Use the two Claude subscriptions only for materially harder tasks, when a DeepSeek attempt exposes a real capability gap, or where cross-provider independent review is required or materially improves confidence.

Track the Claude subscriptions independently as **Claude A** and **Claude B**.

Default Claude assignment:

- **Claude Sonnet 5.5 — Medium** for difficult code, Unity debugging, architecture-sensitive UI, save/hash inspection and independent review.
- Escalate to **Claude Opus — Low/Medium effort** only when Sonnet is insufficient, a failed attempt justifies escalation, or the task is genuinely high-consequence/architecturally difficult.
- Do not use Opus merely because it is available.

### Provider availability / five-hour windows

Claude subscriptions may be logged out, quota-limited or otherwise unavailable. Treat provider availability as an execution resource, not a reason to stop the programme.

Maintain a live provider ledger in #91 comments with one row each for DeepSeek, Claude A and Claude B. For each slot record: `AVAILABLE`, `BUSY`, `COOLDOWN/QUOTA`, or `LOGIN-UNAVAILABLE`; current task if any; and any **service-reported** reset/recovery information.

Rules:

1. If a Claude subscription is logged out or repeatedly booted from login, mark it `LOGIN-UNAVAILABLE` and continue with DeepSeek or the other Claude slot. Do not burn the night on repeated authentication loops.
2. If a five-hour usage window is exhausted, checkpoint the owned child task safely, record the provider-reported reset/window if visible, mark that slot `COOLDOWN/QUOTA`, and route subsequent work elsewhere.
3. At logical task transitions, recheck unavailable Claude slots. When the provider reports the allowance has reset or the slot is usable again, mark it `AVAILABLE` and reuse it where helpful.
4. Do not bypass quotas, authentication, account limits or provider controls. “Reset” means resume/recycle a slot only when the provider makes it available again.
5. Never abandon dirty or uncommitted worker state because a provider becomes unavailable. Checkpoint safely on that worker's owned branch/worktree before switching.
6. A provider switch does not grant new ownership. The replacement worker must inspect the claim, handoff, exact branch/worktree status and candidate before continuing.
7. **Do not invent quota values or reset times.** If capacity is not directly reported, record `UNKNOWN`.

### Independent review

Preserve genuine reviewer independence for high-risk save/reference/determinism work.

- If DeepSeek implements, prefer Claude for required independent review.
- If Claude implements, DeepSeek can independently review.
- Existing #143 implementation was produced by OpenAI gpt-6.1-sol, so DeepSeek or Claude is independent from that implementation.
- If the preferred independent provider is temporarily unavailable, continue other safe tasks rather than self-approving the blocked high-risk change.

The independent reviewer is read-only. Any repair returns to the owning implementation claim or a new explicitly owned repair claim.

## Mandatory browser/local separation

Before dispatching **every** work unit, classify it as `BROWSER-CAPABLE`, `LOCAL/CODE REQUIRED`, or `MIXED`.

### BROWSER-CAPABLE — route through #162

Use browser/GitHub workers for work that does not require a local worktree or Unity, including:

- GitHub inspection, issue/claim updates and dependency reconciliation;
- reading pushed commits/diffs/handoffs/evidence;
- static/independent implementation review where pushed evidence is sufficient;
- Manager/integration-readiness review against accepted specs;
- canonical docs/spec/task-packet/release-handoff preparation;
- repinning downstream tasks to exact accepted/integrated SHAs;
- closing/relabeling stale claims from durable evidence;
- preparing the next local task before a local slot becomes free;
- integration planning and safe GitHub-side integration operations when no local verification is needed.

Browser work must be separated out and completed there rather than consuming a local Code/Unity worker.

### LOCAL/CODE REQUIRED — route through substantive child claim / #106

Use local Code only for:

- production C#/Unity edits in a worktree;
- local verification/tooling changes;
- compiler/build-of-assemblies checks;
- Unity Editor/runtime/rendered interaction qualification;
- local test harness execution;
- integration work that requires local conflict resolution or local verification.

### MIXED

Split mixed tasks into separate phases. Browser preparation/review/repin happens first or concurrently; local implementation/Unity happens only for the part that requires it. **Do not default an entire mixed task into the local lane.**

Browser reviews may run concurrently with unrelated local work. One-writer-per-subsystem remains mandatory. Heavy Unity remains one machine-wide lane.

## Continuous orchestration evidence collection

The orchestrator must gather routing/capacity/outcome information **while work is happening**, not reconstruct it from memory at the end.

Create and continuously update:

`Docs/Handoffs/PlayerTestReadiness-Overnight-01/ORCHESTRATION-LEDGER-20261030.md`

The ledger is an append-only evidence record for this overnight run. For every substantive dispatch, provider switch, quota/login event, retry, handoff, review and integration decision, record the evidence at the time it occurs.

Each entry must distinguish:

- **Observed fact** — direct provider/repository/runtime/test evidence;
- **Orchestrator claim** — what DeepSeek believed and why it acted;
- **Retrospective inference** — a later conclusion from outcomes.

Do not promote an orchestrator claim to observed fact.

The final overnight handoff must draw its Orchestration Decision Appendix from this ledger.

## Starting execution state

Refresh all of this before acting. At handoff publication, the durable state is:

### #143 — S1-D T-04P persistence

- Claim #143 is active.
- OpenAI gpt-6.1-sol implemented the v24 one-latch candidate.
- Pushed review/runtime candidate reported: `b9bf11aaf8f6fd7ea43e33e39258560408b88b12`; production reported identical to `129d0aced1f9935ce683cfcf68169285d1316c4f`, with a verification-only diagnostic repair.
- Offline evidence reported green: 36 latch checks, T-03=48, 16,384 oracle rows, 80 v23/v24 all-hash rows, MFM-06=172, MFM-07=125, N3=88; installed Unity assembly compile zero errors.
- Last durable worker record says Unity qualification is running under its normal lock. **Do not assume it is still running or complete; refresh #143 and machine state.**
- Mandatory independent implementation review is still pending. Browser/review claim **#161** has been prepared.

Immediate #143 sequence:

1. let the existing owning worker finish its Unity chain if it still owns the machine;
2. pin the exact final handed-off SHA;
3. run #161 independent review — **BROWSER-CAPABLE first** if pushed diff/handoff/runtime evidence is sufficient; use local independent execution only if the reviewer identifies a specific evidence gap;
4. if review is `NEEDS CHANGES`, return repairs to #143 and re-review exact repaired SHA;
5. only after qualification + independent APPROVED, issue the delegated overnight integration verdict and integrate safely;
6. release/repin #124 to the exact accepted/integrated v24 lineage.

### #151 — S1-D T-05 previous vs proposed

- Ready for offline implementation from exact start `cfdc93bf3943125d21570bc5749a9338f52d1cce`.
- Primary production surface is `WorkPlanView.cs`.
- Reuse integrated S1-C exact-year/history navigation.
- Prefer no `AnnualReviewView.cs` edit; stop for overlap disposition if a missing tiny navigation API is proven.
- Offline/static work may start immediately in an independent local lane if ownership is free.
- Final rendered/interaction qualification requires serialized Unity.

### #156 — MFM-08 Stand Map / 3D review UX

- Manager repin comment `6511551513` supersedes the stale #147-waiting wording.
- Exact released start is now `cfdc93bf3943125d21570bc5749a9338f52d1cce`.
- Offline implementation may start after fresh ownership checks.
- Unity is no longer blocked by #147; it is only machine-serialized.
- No Annual Review/history, S1-D Work Plan history, save/schema, MFM-09 curriculum, coverage-world/calibration, proposal→Fell-mark or bulk-confirmation ownership.

### #124 / #126 / #128

- #124 is blocked only on accepted/integrated #143 and then consumes the historical-completion read; it owns curriculum/read-only consumption, not save/schema.
- #126 PARITY-08 is blocked on the accepted S1-D persistence/curriculum lineage and must be **browser-repinned to the actual later v24 integration** before implementation. Do not use its old v23 production-base wording blindly.
- #128 is the independent PARITY-08 review and starts only after #126 publishes one exact candidate/handoff.

### Nonblocking follow-up

- #74 PARITY-03 Q10 remains evidence-strengthening only. Do not let it displace critical-path S1-D/MFM/parity Unity work.

Historical issues whose titles still say `[RUNNING]` are not proof of live ownership. `active-tasks.md` plus current task comments/handoffs govern.

## Overnight execution priority

The orchestrator should keep all usable lanes productive without violating single-writer rules.

### Priority chain A — persistence/curriculum/parity critical path

1. **#143** finish Unity/handoff → #161 independent review → delegated integration.
2. **Browser lane #162:** immediately prepare/repin #124 to exact integrated #143 lineage.
3. **#124** implement/qualify guide monotonicity; review/integrate.
4. **Browser lane #162:** repin/release #126 PARITY-08 to the actual accepted v24/S1-D lineage and current integration state.
5. **#126** implement/qualify PARITY-08.
6. **#128** independent review, browser-first where possible; local only for identified evidence gaps.
7. Integrate accepted PARITY-08 and continue the accepted parity dependency chain toward PARITY-09.

High-risk save/reference/determinism tasks require the specified independent review before acceptance. No hash/reference rebaseline.

### Priority chain B — repeated-intervention presentation

Run **#151 offline** as soon as a non-overlapping local worker is available. It may proceed while #143 uses Unity. When offline work is complete, queue its Unity qualification behind the current critical Unity owner. Browser review/integration preparation should happen through #162 as soon as the pushed candidate exists.

### Priority chain C — MFM-08 UX

Run **#156 offline** as soon as a non-overlapping local worker is available. It may proceed in parallel with #151/#143 because its ownership is separate. Queue Tier-2 Unity/rendered qualification through the single Unity lane. As soon as a pushed candidate/handoff exists, use #162 for static/Manager review prep.

### After #124 + #151 + #156

When MFM-08 and S1-D T-04/T-05 are accepted/integrated, use #162 to inspect the approved MFM-09 requirements and prepare/repin its exact implementation packet before consuming a local slot. Then dispatch the local MFM-09 implementation through its own substantive claim.

Continue through remaining accepted S1-D/parity/MFM tasks in dependency order, always browser-separating review/coordination/packet work first. Reconcile completed Rack/S1-C evidence rather than rerunning work merely because a historical issue title remains open/stale.

### Unity scheduling

Unity is strictly serialized. Before **every** Unity acquisition:

- refresh `active-tasks.md` and current claim;
- confirm no other owner;
- check lock and Editor process state;
- check free space on the partition actually hosting/importing the Unity lane;
- preserve the >=15 GiB mandatory floor and prefer >=20 GiB practical headroom;
- declare exact candidate SHA/lane/owner;
- never kill another worker's process or bypass its lock.

While Unity is busy, keep browser work and non-overlapping offline work moving.

## Integration policy

Keep `IMPLEMENTED`, `REVIEWED`, `INTEGRATED`, `VERIFIED` and `ACCEPTED` distinct.

For every returned implementation/review:

1. inspect exact candidate/handoff/evidence;
2. issue `APPROVED`, `NEEDS CHANGES` or `REJECTED` against accepted requirements;
3. if APPROVED, integrate only after checking current integration head and overlap;
4. preserve both histories with normal merge semantics; no silent transplant/rebase/amend/reset;
5. if conflict or unexpected production overlap occurs, stop that integration and assign explicit ownership/review rather than guessing;
6. publish an integration handoff with exact parents/merge SHA and accepted evidence;
7. repin/release the next dependency immediately when safe.

No local or GitHub state alone proves Unity/runtime/rendered success. Require the relevant qualification evidence.

## Hard stops

Do not stop the whole overnight programme for a local failure if unrelated safe work remains. Block only the affected chain and continue independent work.

Stop/hold the affected chain for:

- a genuine unresolved product decision or scope change;
- unsafe ownership overlap that cannot be resolved from accepted API/ownership boundaries;
- protected Reference Future/save/determinism incompatibility;
- inability to maintain Unity storage/lock safety;
- a mandatory review that cannot be made independent;
- an integration conflict requiring product/ownership judgement;
- a test failure that can be resolved only by weakening/rebaselining an accepted gate.

Do not invent gameplay behaviour merely to keep the overnight run moving.

## Required durable outputs

Continuously maintain:

`Docs/Handoffs/PlayerTestReadiness-Overnight-01/ORCHESTRATION-LEDGER-20261030.md`

At the end of the overnight session publish:

`Docs/Handoffs/PlayerTestReadiness-Overnight-01/OVERNIGHT-RESULT-20261030.md`

Update #91 with a concise final status and exact next action. Update `active-tasks.md` to the verified live state before handing control back.

## Mandatory Orchestration Decision Appendix

Append the following sections to `OVERNIGHT-RESULT-20261030.md`. The orchestrator must gather the underlying information continuously in the orchestration ledger rather than reconstructing it afterward.

### 1. Orchestration policy

Record:

- orchestrator provider/model;
- user-supplied routing priorities;
- preferred provider/model order;
- when to escalate to stronger models;
- capacity/subscription constraints known at start;
- concurrency / one-writer rules;
- review requirements;
- protected decisions requiring Manager/user approval.

### 2. Material routing decisions

For every significant worker/model change, record:

- task/work unit;
- previous provider/model;
- selected provider/model;
- alternatives considered, if known;
- why it was selected;
- routing rule applied;
- capacity evidence available at that moment;
- capacity source: provider / T3 / user / inferred / unknown;
- reset time if reported;
- previous failure/outcome that influenced the decision;
- resulting worker outcome.

Do not reconstruct missing quota values. If capacity is unknown, write **UNKNOWN**.

### 3. Capacity events

Record every material subscription/provider event:

- provider/pool;
- timestamp;
- exact/approximate/unknown capacity state;
- reported reset/window;
- source of information;
- whether it caused a routing change;
- what worker/model was chosen next.

### 4. Worker outcomes

For each significant worker:

- task;
- provider/model;
- outcome;
- tests/evidence;
- handoff or commit reference;
- rework required;
- accepted / rejected / unresolved;
- failure classification.

A worker saying “completed” is not proof of acceptance.

### 5. End-of-session assessment

Briefly record:

- routing decisions that worked well;
- routing decisions that appear wasteful or incorrect;
- repeated failure patterns;
- tasks where a cheaper model would probably have sufficed;
- tasks where escalation was justified;
- missing telemetry that would have improved decisions;
- recommended routing-policy changes for future runs.

### 6. Evidence rule

Keep three things distinct:

**Observed fact** — supported by runtime/provider/repo/test evidence.

**Orchestrator claim** — what the orchestrator says it believed or why it acted.

**Retrospective inference** — conclusion drawn later from outcomes.

Do not convert orchestrator claims into ground truth.

### 7. Mandatory final summary

Include:

- total delegated attempts;
- completed / failed / cancelled counts;
- capacity-related failures;
- retries/salvages;
- material routing decisions;
- decisions backed by direct capacity evidence;
- decisions made with inferred/unknown capacity;
- review findings;
- unresolved tasks;
- exact next action.

For game-development work also include:

- Unity/editor lanes used;
- serialized Unity operations;
- worktrees/branches touched;
- rendered/runtime qualification results;
- save/reference/determinism work that received independent review;
- any protected anchors or baselines changed.

## Success / return condition

Continue automatically until either:

1. the parent success condition is actually proven: one exact integrated Tier-3-qualified Scenario One candidate ready for user personal playtest, in which case return:

`SCENARIO ONE PLAYER-TEST CANDIDATE READY — USER PLAYTEST REQUIRED`

or

2. no further safe dependency-ready work can proceed without a hard-stop decision, in which case publish the exact blockers and the best next action in `OVERNIGHT-RESULT-20261030.md`.

No Windows build under either condition.