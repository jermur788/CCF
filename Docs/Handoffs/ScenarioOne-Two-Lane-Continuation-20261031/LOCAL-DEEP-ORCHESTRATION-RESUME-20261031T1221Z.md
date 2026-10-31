# Scenario One Local Deep Orchestration Resume 02 — 2026-10-31 12:21 UTC

**Parent umbrella:** #201 / #91  
**Resume execution claim:** #203  
**Counterpart Manager/browser lane:** #200 / bounded browser claim #202  
**Orchestrator:** DeepSeek V4.1 Flash (or current equivalent available DeepSeek orchestrator)  
**Lane:** local repository/worktrees + serialized Unity as released through child claims  
**Difficulty:** High overall because this is a multi-task, long-running execution programme; individual work units are routed by difficulty  
**User action after publication:** give the dispatch line at the end to the local/long-running chat after the browser Manager has published the next release.

## Purpose

Resume the non-browser Scenario One programme as a long-running orchestrated task. DeepSeek is the orchestrator and the default worker for easy and bounded work. Harder work is escalated to Claude Sonnet 5.5. Opus is the **final** escalation tier, not a routine worker.

The goal is not simply to make tasks green. The orchestrator must also collect reliable routing evidence so later Manager/user decisions can learn which model/provider/task combinations are actually effective.

This handoff continues the existing #201 evidence programme and does not replace it. Existing `ORCHESTRATION-LEDGER.md` and `ORCHESTRATION-RESULT.md` are historical evidence for this continuation and must be preserved.

## Start gate

Do **not** begin new production/Unity work until #200/#202 has published the current Manager release and live `Docs/Project/coordination/active-tasks.md` confirms the relevant child ownership can start.

At start:

1. fetch/refresh origin and exact refs;
2. read live `Docs/Project/coordination/active-tasks.md`;
3. read #200/#202 and #201/#203;
4. read the child claim to be executed;
5. verify exact branch/worktree/start SHA;
6. verify worktree cleanliness without stashing/resetting/discarding unrelated work;
7. inspect free disk, Editor processes and machine-wide Unity lock before any Unity phase;
8. post `[RUNNING]` on the child claim with actual provider/model, branch, worktree and starting HEAD;
9. do not write `active-tasks.md` — #200/browser lane is the sole primary writer for that live coordination file.

Claims track execution presence only; they never grant ownership.

The user's explicit **NO ACTION** restriction on Research Source Consolidation Audit #194 remains in force unless explicitly lifted.

## Routing ladder — binding for this run

### Tier A — DeepSeek orchestrator + default worker

Use DeepSeek for the orchestration role and for all work it can reasonably complete without creating material quality risk.

Default DeepSeek work includes:

- difficulty 1–3 tasks;
- bounded verification repairs with clear accepted authority;
- mechanical or localized code changes;
- test/harness changes where the contract is explicit and test weakening is not required;
- scripts/tooling and evidence extraction;
- branch/worktree preparation and checkpointing;
- Unity gate execution and cleanup where the implementation is already clear;
- log/result classification;
- straightforward bug fixes with one subsystem and a small surface;
- independent review when the implementer is Claude and DeepSeek is sufficiently capable for the review scope;
- salvage/resume work after another provider is interrupted, provided ownership is clean and existing commits/work are preserved.

Current run evidence supports this default: the previous #201 run completed #196 and #198 with DeepSeek on bounded difficulty-3 verification work, including full Unity qualification, without model escalation.

### Tier B — Claude Sonnet 5.5 for genuinely harder work

Route to **Claude Sonnet 5.5 Medium** when the task is materially harder than the DeepSeek default and the extra reasoning/architecture capability is justified.

Typical Sonnet tasks:

- difficulty 4/5 cross-screen or cross-system production changes;
- architecture-sensitive Unity UI work;
- non-trivial persistence/save/determinism implementation;
- broad refactors with multiple interacting contracts;
- difficult debugging where several plausible root causes must be separated;
- high-risk production work where a bounded wrong fix would be costly;
- tasks for which DeepSeek has produced a classified capability/reasoning failure rather than an environment/test/authority failure.

Use Medium by default. Use higher Sonnet effort only if the available runtime supports it and the routing record explains why Medium is insufficient.

The previous #201 run routed #199 to Sonnet because it was a difficulty-4 cross-screen UI task. Sonnet correctly reached a protected-contract/authority boundary and stopped rather than weakening tests. That is not a capability failure.

### Tier C — Opus as final escalation only

Use **Claude Opus** only as the final escalation tier.

Opus is justified only when at least one of the following is true:

- Sonnet has a documented capability/reasoning failure on a well-scoped task after environment, authority, test and ownership causes are ruled out;
- the task is unusually high-consequence/architecturally complex and the Manager explicitly releases it at that tier;
- an independent high-stakes review requires stronger reasoning than DeepSeek/Sonnet can credibly provide and the reason is documented.

If model effort choices exist, start at the lowest Opus effort capable of the job and escalate effort only with evidence. Do not send bounded/mechanical work to Opus.

### What is **not** a reason to escalate models

Do not escalate DeepSeek → Sonnet or Sonnet → Opus merely because of:

- Unity lock contention;
- provider quota/cooldown/login failure;
- stale Unity `Temp`/DB-lock state;
- disk pressure;
- a genuine failing test;
- an authority/product-decision conflict;
- a flaky input-driver timing failure;
- a transport timeout while a worker is still running;
- a dirty/occupied worktree;
- missing evidence;
- a task that was simply under-specified.

Classify and repair/salvage/retry the actual failure class first. A stronger model does not solve an authority gap or a machine-state problem.

## Provider/subscription capacity handling

Maintain a live provider/pool table for every usable DeepSeek/Claude subscription or worker pool.

Required states:

- `AVAILABLE`
- `BUSY`
- `COOLDOWN`
- `LOGIN-UNAVAILABLE`
- `UNKNOWN`

For each pool record:

- model and reasoning/effort;
- remaining-capacity estimate if observable;
- reset/window information if observable;
- source and confidence;
- current task;
- capacity-to-completion: `sufficient / marginal / insufficient / unknown`;
- expected interruption/switching cost.

Never invent quota or reset state. A five-hour/rolling window is reusable only after direct provider/T3/user evidence that the pool is available again; elapsed time alone does not prove reset.

If a subscription runs out mid-task:

1. checkpoint/push the valid work immediately if safe;
2. preserve uncommitted verification/evidence without stashing or discarding unrelated work;
3. mark the pool `COOLDOWN`, `LOGIN-UNAVAILABLE` or `UNKNOWN` based on actual evidence;
4. assess whether another DeepSeek pool can resume cleanly;
5. if not and the task is hard, route to Sonnet;
6. only use Opus if the escalation rule above is actually met;
7. record the switch, switching cost and salvage amount in the ledger.

Do not restart from zero when valid work can be safely salvaged.

## Concurrency and ownership

- One primary writer per subsystem/file set.
- One writable lane/worktree per owner.
- Heavy Unity is serialized machine-wide.
- Never run two Unity Editors against the same project/worktree.
- Before handoff between workers, checkpoint/commit/push as appropriate and relinquish ownership cleanly.
- Never stash, reset, discard, clean, overwrite or absorb unrelated dirty work.
- If file ownership overlaps, stop and return to #200 for ownership/API disposition.
- Browser-capable management/review work belongs to #200/#202 rather than consuming the local lane.

## Mandatory adaptive-routing evidence contract

Continue the existing #201 evidence programme. Append to the current ledger or create a clearly linked continuation ledger if branch structure requires it; do not rewrite historical entries.

At every material dispatch, provider/model switch, retry, salvage, task decomposition, concurrency decision or escalation record the following **at the time of the decision**:

### Policy / run state

- orchestrator provider/model/reasoning;
- user objective;
- hard constraints;
- optimization preference (quality/cost/tokens/time/compute/exploration tolerance);
- provider/model order and escalation ladder;
- one-writer/concurrency/review rules;
- protected Manager/user decisions.

### Capacity + context

For each provider/pool:

- state;
- remaining-capacity estimate;
- reset/window;
- source/confidence;
- current work;
- capacity-to-completion;
- switching/interruption cost.

For the orchestrator when observable:

- context limit;
- used/estimated context;
- compactions;
- material context-pressure concerns;
- source/confidence.

Unknown stays **UNKNOWN**.

### Routing + decomposition event

Record:

- work unit and parent;
- task type, difficulty and risk;
- decomposition topology/dependencies;
- previous provider/model/role;
- selected provider/model/reasoning/role;
- alternatives considered;
- reason/rule for selection;
- live capacity/context evidence;
- capacity fit;
- switching cost;
- `EXPLOIT` or `EXPLORE`;
- prior outcome influencing the choice;
- branch/worktree/start SHA;
- Unity requirement and lock state.

### Outcome

Record:

- actual provider/model/config used;
- result;
- tests/runtime/rendered evidence;
- commit/handoff/claim;
- accepted/rejected/unresolved state;
- retries/rework/salvage;
- failure class;
- observable tokens/model calls/reasoning/wall time/parallel workers/provider failures where available.

**Worker completed is not Manager acceptance.**

### Evidence classes

Keep these exact classes distinct:

- **OBSERVED FACT** — provider/runtime/repository/test evidence;
- **ORCHESTRATOR CLAIM** — what the orchestrator believed and why it acted;
- **RETROSPECTIVE INFERENCE** — later analysis/learning conclusion.

Never promote a claim or inference to fact merely because it seems plausible.

### Learning checkpoints

Write a learning checkpoint after:

- every provider/model escalation;
- every capacity/auth failure;
- every rejected worker result;
- every significant salvage/retry;
- approximately every five material routing decisions;
- final run completion/pause.

Include:

- what worked;
- what wasted effort;
- whether a cheaper/lower-resource worker would likely have sufficed;
- whether escalation was actually justified;
- capacity/context effects;
- coordination overhead;
- missing telemetry;
- candidate routing-policy experiments;
- hypothesis + confidence + sample size.

Hypotheses are not active policy until Manager/user acceptance.

## Current known state from the prior #201 run

The prior run stopped correctly at a Manager decision boundary. It did **not** create a new integrated/Tier-3 candidate.

Returned tasks:

- #196 — MapPlanning reconciliation: branch `verify/mfm-08-mapplanning-contract-reconcile-v1` @ `eb4be600`; authoritative 168/168, 8 captures; Manager verdict pending at the time of the prior result.
- #198 — guided-v2 v24/D-066 reconciliation: branch `verify/guided-v2-v24-contract-reconcile-v1` @ `59b278f7`; guided-v2 535/0, guided-legacy 569/0; Manager verdict pending at the time of the prior result.
- #199 — partial Combined HUD/focus repair: branch `fix/scenario-one-combined-hud-focus-regression-v1` @ `42742a8a`; combined-v2 453/10, combined-legacy 440/8; blocked on Manager direction.

Prior routing observations:

- DeepSeek: first-pass technical success on two bounded difficulty-3 verification reconciliations;
- Sonnet 5.5 Medium: appropriate for #199 difficulty-4 UI diagnosis; it stopped on an authority conflict, not a capability failure;
- Opus: not used and not justified by the observed run;
- one real-input timing retry and one stale Unity Temp/startup issue were environment/runner failures and were correctly **not** model escalations;
- no provider capacity/auth failure occurred in that run;
- observed capacity-to-completion was generally UNKNOWN because live quota telemetry was unavailable.

Treat this as evidence for routing priors, not immutable policy.

## First released local task after #200/#202 disposition

Do not assume the task is released until the browser Manager records it.

The expected next bounded production continuation is #199 Option-1 layout repair.

### Expected #199 continuation scope

Preserve both sets of accepted/protected behavior:

- minimap remains visible for ordinary estimate inspection;
- accepted PARITY-07 estimate information/copy remains unless later authority changes it;
- central aiming prompt and detail/estimate columns remain clear;
- protected Combined minimap/focus assertions remain semantically unchanged;
- exposed 1920 V2 competitor-list clipping is repaired;
- no Rack geometry/marking/persistence changes;
- no MFM behavior changes;
- no save/schema/anchor/reference/ecology/economy changes.

### Suggested #199 routing

**Orchestrator:** DeepSeek.  
**Production implementation:** Sonnet 5.5 Medium — this remains a difficulty-4 cross-screen Unity UI/layout problem.  
**Gate execution/evidence:** DeepSeek unless implementation-specific debugging makes Sonnet continuation materially more efficient.  
**Independent review:** DeepSeek or another non-implementing provider/thread if sufficient; must be independent of the Sonnet implementation thread.  
**Opus:** not pre-authorised; use only if Sonnet has a classified capability failure after authority/environment/test causes are excluded.

### Required #199 verification

At minimum:

1. exact production diff and root cause;
2. full combined-v2 and combined-legacy at 1280×720 and 1920×1080;
3. PARITY-07 rendered/UI qualification for estimate/detail/prompt clearance;
4. affected minimap/inspection/focus regressions;
5. proportionate Rack/guided gates if the changed UI surface overlaps them;
6. independent implementation review;
7. clean worktree/Editor/lock return;
8. durable GitHub handoff.

If both protected contracts cannot be satisfied without a true product decision, stop and return the smallest evidence-backed conflict. Do not shorten copy, weaken tests or supersede protected assertions without #200/user authority.

## Subsequent programme queue

Follow live #200 releases, not this text alone. Expected dependency shape after MFM-08 closure is:

1. browser Manager approves/integrates #156 if warranted and republishes exact combined lineage;
2. S1-C T-08 final history qualification;
3. S1-D T-08 multi-cycle witness;
4. S1-D T-09 browser/independent conformance review;
5. MFM-09 teaching / Work Plan integration;
6. MFM-10 species-parity guard — must return `SPECIES GUARD QUALIFIED`;
7. PARITY-09 and MFM-11 when dependencies clear;
8. MFM-12 integrated MFM qualification;
9. #199 and any other applicable protected-gate debt green before final Tier-3;
10. final Scenario One Tier-3 complete-loop qualification on one exact candidate;
11. user personally playtests that exact candidate;
12. Windows build only after explicit post-playtest authorisation.

At each dependency boundary, stop only if a Manager/product/ownership decision is required. Otherwise continue automatically using the released packets and the routing ladder above.

## Final run output

Publish/append:

- `Docs/Handoffs/ScenarioOne-Two-Lane-Continuation-20261031/ORCHESTRATION-LEDGER.md` with continuation events;
- `Docs/Handoffs/ScenarioOne-Two-Lane-Continuation-20261031/ORCHESTRATION-RESULT.md` with the newest run summary or a clearly linked successor result if preserving the previous result verbatim is cleaner;
- individual child handoffs for every substantive implementation/qualification/review/integration result.

Final summary must include:

- all attempts;
- completed/failed/cancelled work;
- provider capacity/auth failures;
- retries/salvage;
- material routing/decomposition decisions;
- direct vs inferred/unknown capacity decisions;
- resource observations;
- review findings;
- unresolved work;
- exact next action;
- Unity/editor lanes and serialized operations;
- worktrees/branches;
- rendered/runtime qualification;
- any independently reviewed save/reference/determinism work;
- any changed protected anchors/baselines — normally **none** unless explicitly authorised.

## Hard stops

Stop and return to #200/user for:

- product/authority conflict;
- scope expansion beyond an approved packet/spec;
- ownership overlap;
- unsafe integration conflict;
- required independent review that cannot be obtained;
- mandatory user test;
- proposed protected-anchor/reference/schema change without authority;
- any Windows build request before the user's personal Scenario One playtest and explicit authorisation.

Do **not** stop merely because one provider hits quota if another safe worker can resume under the routing ladder and ownership rules.

## Copy/paste dispatch line

`Take Local Deep Orchestration Resume 02 under claim #203, parent #201/#91. Wait for the current #200/#202 Manager release, then refresh active-tasks.md and all relevant child claims/refs before starting. DeepSeek is the orchestrator and default worker for all easy/bounded difficulty-1–3 work, verification, tooling, gate execution and salvage it can reasonably handle. Route genuinely harder difficulty-4 architecture-sensitive/cross-system work to Claude Sonnet 5.5 Medium. Escalate to Opus only as the final capability tier after a documented Sonnet reasoning/capability failure or an explicitly justified high-consequence need; never escalate for environment, lock, quota, genuine test, authority, stale-state or transport failures. Maintain the mandatory adaptive-routing ledger from ORCHESTRATION-EVIDENCE-APPENDIX-V2: provider/pool state and capacity, alternatives, reason, context, capacity-to-completion, switching cost, exploit/explore, outcomes, retries/salvage/resources, and OBSERVED FACT / ORCHESTRATOR CLAIM / RETROSPECTIVE INFERENCE separation. Do not write active-tasks.md; #200 owns it. Use substantive child claims and fresh ownership/Unity preflight. Expected first released hard task is the #199 bounded layout repair: use Sonnet for production UI implementation, DeepSeek for orchestration/gates/evidence where suitable, preserve the protected Combined minimap/focus contract and PARITY-07 estimate/prompt clearance, fix 1920 competitor clipping, obtain independent review, and return a durable handoff. Continue through released S1-C/S1-D/MFM/PARITY/Tier-3 dependencies automatically unless a true Manager/user/ownership stop is reached. #194 remains NO ACTION. No Windows build before user playtest + explicit authorisation.`