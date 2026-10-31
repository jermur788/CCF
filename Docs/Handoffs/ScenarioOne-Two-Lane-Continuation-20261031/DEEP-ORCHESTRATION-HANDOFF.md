# Scenario One Long-Running Deep Orchestration 01 — Handoff

**Claim:** #201  
**Parent:** #91  
**Counterpart browser/Manager lane:** #200  
**Role:** long-running non-browser orchestration  
**Prepared:** 2026-10-31  
**Context snapshot:** `d05b27ad376ce02748ee4703429ad3ff66a65542`  
**Main at split:** browser handoff published after `8bd03bca142bbea6d379ad5f0a506f3c0922d2ec`  
**Published Scenario One integration at split:** `ce6737146a7aa9ea953d7db0ca93b103f6c2c996`

## Mission

Run the **long-running execution/orchestration lane for all remaining work that cannot safely be completed in the browser** until either:

1. the Scenario One programme reaches an exact integrated Tier-3-qualified candidate ready for the user's personal playtest; or
2. a genuine Manager/user decision, unsafe ownership conflict, unavailable required capability, or unresolved mandatory gate requires stopping.

This is an orchestration task, not one giant implementation task. Every substantive implementation/qualification/repair still uses its own child claim, lane/worktree and durable GitHub handoff.

At the same time, this run is an **adaptive-routing evidence run**. Capture enough data about decomposition, provider/model choices, escalation, capacity/context and outcomes that later analysis can learn which routing choices were effective and which were wasteful.

## Two-lane split

### #200 — browser/GitHub Manager lane

Owns:

- Manager review and canonical `APPROVED / NEEDS CHANGES / REJECTED` decisions;
- GitHub inspection and browser/static diagnosis;
- packet/repin/claim preparation;
- canonical status/docs changes;
- sole primary write ownership of `Docs/Project/coordination/active-tasks.md` during this split;
- review/closure of returned local handoffs;
- dependency release after accepted evidence.

### #201 — this long-running orchestration lane

Owns orchestration of:

- local worktrees/branches;
- C#/Unity implementation and bounded repair;
- verification tooling requiring local execution;
- compiler/import/Editor/runtime/render/native-input qualification;
- serialized Unity scheduling;
- local integration when authorised;
- execution-worker selection, switching and escalation;
- continuous orchestration evidence capture.

Read live `active-tasks.md` before every substantial dispatch/start, but do **not** become a competing writer to it. Post execution facts on #201 and child claims; return coordination changes to #200.

## Current routing policy — latest user instruction

### Orchestrator

**DeepSeek is the default long-running orchestrator.** It should retain the global dependency graph, watch child results/capacity, and continue safe dependency-independent work while another child is running or waiting for Manager review.

### Worker ladder

Claude is the principal high-difficulty worker family; DeepSeek remains the default lower-cost worker where capability is sufficient.

Use this starting ladder, then record deviations with evidence:

| Task profile | Default route | Escalation rule |
|---|---|---|
| Difficulty 1–2/5; bounded mechanical/offline verification; narrow tooling; straightforward branch/worktree execution | **DeepSeek** | Escalate only if it cannot complete correctly or a task-specific capability is missing. |
| Difficulty 3/5; moderate code/verification with clear packet and limited ambiguity | **DeepSeek first** unless the task is especially architecture/Unity-sensitive | Escalate to Sonnet 5.5 Medium for capability/reasoning need, not merely because a gate correctly finds a defect. |
| Difficulty 4/5; architecture-sensitive Unity/UI, substantial cross-file implementation, difficult debugging, important integration | **Claude Sonnet 5.5 Medium** | If Sonnet fails because of capability/reasoning after a bounded attempt, escalate to Opus. |
| Difficulty 5/5; high-consequence save/determinism/reference work; unusually difficult root cause; Sonnet capability failure | **Claude Opus 5.5 Low** by default | Use Opus Medium only when Low is demonstrably insufficient and record why. |

Do not use Opus for routine/mechanical work merely because it is available.

A provider/model failure must be classified before escalation:

- **CAPACITY/AUTH failure:** switch provider/pool; do not call it a capability failure.
- **ENVIRONMENT/LOCK/RUNNER failure:** repair/route the environment problem; do not escalate model capability reflexively.
- **SCOPE/AUTHORITY blocker:** return to #200; a stronger model cannot authorise missing scope.
- **CAPABILITY/REASONING failure:** justified model escalation.
- **VALID GATE FAILURE / REAL DEFECT:** the worker may have succeeded by finding the defect; route the required repair rather than treating the model as failed.

No OpenAI worker is part of the default ladder in this user-directed run unless #200/user explicitly reintroduces it.

## Mandatory source/reference material for orchestration learning

The user supplied the following Project sources specifically to seed routing-learning evidence. Read/use them as reference before the first material dispatch and preserve their distinctions.

### 1. `ORCHESTRATION-EVIDENCE-APPENDIX-V2.md`

Role: **current candidate evidence-capture contract**, not gameplay/product authority and not a fully implemented T3 runtime policy.

Mandatory concepts to carry into this run:

- record orchestrator provider/model/reasoning, objective, hard constraints and optimisation preferences;
- maintain per-provider/pool state: `AVAILABLE / BUSY / COOLDOWN / LOGIN-UNAVAILABLE / UNKNOWN`;
- capture remaining-capacity estimate, reset/window, source/confidence and current work where observable;
- at dispatch record **capacity-to-completion** (`sufficient / marginal / insufficient / unknown`) and expected switching/interruption cost;
- capture orchestrator context limit/use/pressure/compactions when observable;
- for each material dispatch/switch/retry/decomposition record task characteristics/risk, topology, previous/selected provider/model/reasoning/role, alternatives, reason/rule, capacity/context evidence, fit, switching cost, `exploit/explore`, policy/knowledge version and prior outcomes affecting the choice;
- record actual worker outcome, tests/runtime/review evidence, commit/handoff, acceptance state, retries/rework/salvage and failure class;
- record tokens, calls, wall time, parallel workers and provider failures **only when observable**; unknown stays UNKNOWN;
- do not call proxies measured energy;
- finish with learning assessment: what worked, waste/incorrect choices, cheaper likely-sufficient routes, justified escalation, capacity/context effects, overhead, missing telemetry and candidate experiments/policy changes.

### 2. `ORCHESTRATION-LEDGER-20261030.md`

Role: **historical observed evidence from the 2026-10-30 overnight run**, not current policy.

Known preserved ledger identity from the supplied bootstrap:

- title: `PLAYER-TEST-OVERNIGHT-01 — Orchestration Evidence Ledger — 2026-10-30`;
- 463 lines / 40,963 bytes in the preserved copy;
- SHA-256: `d568a2969bf38a707f74e28396c94602d34226d0307b2b9b118a14ed8d00f16e`.

Historical examples worth retaining as comparison evidence, not blindly repeating:

- DeepSeek was the overnight orchestrator/default provider under the user's earlier policy; Claude Sonnet was reserved for harder tasks and Opus for justified escalation.
- #143 independent review: DeepSeek browser review of an OpenAI implementation succeeded and was accepted.
- #156: Sonnet 5.5 Medium produced the offline MFM-08 candidate, but the worker over-claimed readiness before Unity; Manager corrected the state. This is a reporting/qualification lesson, not automatically a model-capability failure.
- #151: an OpenAI worker hit a subscription limit mid-task; DeepSeek resumed and salvaged pushed/uncommitted work. Classify this as capacity failure + successful salvage, not implementation failure.
- #124: DeepSeek handled bounded curriculum verification; Sonnet provided independent review/Unity work. Later failures were stale verification contracts/runner issues, illustrating why gate failure must be classified before model escalation.
- #126: Opus 5.5 Low was deliberately used for high-consequence save/determinism work and found a real unknown-species validation defect before Unity. This is evidence supporting escalation for genuinely high-risk work, not a rule that all save work requires Opus.

Use the historical ledger to define comparison features and failure classes; do not treat its old task status as current.

### 3. `T3-Fork-Orchestration-Evidence-Bootstrap-20261030.zip`

Role: **storage/provenance bootstrap and historical references**.

Supplied archive SHA-256 in the Manager environment: `bb2590845f6add84bd5cb3382211df3a7e01b91c42078dc9abe3d3c5cdd3ecb5`.

It contains:

- `DEEPSEEK-HANDOFF-BOOTSTRAP-ORCHESTRATION-EVIDENCE-20261030.md`;
- `ORCHESTRATION-DECISION-APPENDIX-COMPACT-V1.md`;
- `ORCHESTRATION-EVIDENCE-APPENDIX-V2.md`;
- `ORCHESTRATION-LEDGER-20261030.md`;
- `README.md`;
- `SHA256SUMS.txt`;
- `SOURCE-REFERENCE-OVERNIGHT-CONTINUATION-20261030.md`.

Important preserved hashes from that bundle:

- ledger: `d568a2969bf38a707f74e28396c94602d34226d0307b2b9b118a14ed8d00f16e`;
- V2 appendix: `7f033e0894142f4ace7d8863b91a3022a3ff09e21e765e3e786a776852c4fd8a`;
- compact V1 appendix: `e0231a25fe5d586d1a1c787a254d2d24ca051adb4bec157a87eaf7279dd0105e`;
- bootstrap DeepSeek handoff: `3bb18ffbc84344f81cff286e80463ca1d1d2680a60c61d027d0dfebe6e7f0d7c`;
- source-reference pointer: `2e3c03d1b856c9b1d91fc595537114c8347f05c3870e3d12d17021e7a50014df`.

The bundle's separation rule remains mandatory: **historical source evidence is not current policy. Derived summaries, hypotheses and learned routing guidance remain distinguishable from observations.**

### 4. Pinned historical overnight continuation handoff

Historical reference only:

- repository: `jermur788/CCF`;
- commit: `4112718310cc842d9ba2d677bc55c4cb450e8bcb`;
- path: `Docs/Handoffs/PlayerTestReadiness-Overnight-01/OVERNIGHT-CONTINUATION-20261030.md`.

If used for comparison, read the pinned commit rather than current `main` so the evidence cannot drift. Do not allow that old handoff to override this user's newer routing instruction or current project authority.

## Mandatory new evidence ledger for this run

Create and maintain a durable append-oriented ledger under:

`Docs/Handoffs/ScenarioOne-Two-Lane-Continuation-20261031/ORCHESTRATION-LEDGER.md`

Capture events **when they happen**. Do not reconstruct missing provider capacity, context or rationale after the fact.

Use these evidence classes exactly:

- **OBSERVED FACT** — provider/runtime/repository/test/user evidence directly observed;
- **ORCHESTRATOR CLAIM** — what the orchestrator believed or why it chose a route at the time;
- **RETROSPECTIVE INFERENCE** — later conclusion drawn from accumulated outcomes.

Never promote a claim/inference into observed fact.

### Starting-policy section

Record:

- user objective: progress Scenario One to personal-playtest-ready exact candidate while learning from routing decisions;
- two-lane split: browser #200 versus non-browser #201;
- quality/cost preference: least costly capable worker, DeepSeek default, Sonnet 5.5 Medium for harder work, Opus only on justified escalation;
- hard constraints: one writer per subsystem, serialized Unity, no test weakening, no unauthorised product/save/reference changes, no Windows build before user playtest;
- review independence requirements;
- current integration and first task;
- source/reference versions used.

### Provider/pool ledger

At start and after material provider events, record for each usable pool:

| Field | Required value |
|---|---|
| provider/pool | exact runtime/provider label if known |
| model/reasoning | exact if known; otherwise UNKNOWN |
| state | AVAILABLE/BUSY/COOLDOWN/LOGIN-UNAVAILABLE/UNKNOWN |
| capacity estimate | exact/approximate/UNKNOWN |
| reset/window | observed value or UNKNOWN |
| source/confidence | provider/T3/user/runtime/inferred/unknown + confidence |
| current work | child task(s) |
| context state | available estimate/pressure/compaction or UNKNOWN |

Do not invent subscription state from absence of an error.

### Per-routing-event record

For **every material dispatch, decomposition, retry, provider switch, escalation or salvage**, capture:

- timestamp;
- work unit + parent/child claim;
- evidence class;
- task type and characteristics;
- difficulty 1–5 and why;
- risk class: ordinary / UI-runtime / integration / save-determinism-reference / other;
- decomposition topology and dependencies;
- previous provider/model/role;
- selected provider/model/reasoning/role;
- alternatives considered;
- exact selection reason/rule;
- provider capacity/context evidence and source;
- capacity-to-completion estimate;
- expected switching/interruption cost;
- `EXPLOIT` versus `EXPLORE` classification;
- prior outcome that influenced the choice;
- branch/worktree/lane and starting SHA;
- Unity requirement and lock state when relevant;
- result/evidence when returned;
- worker output/head/handoff;
- actual tests/runtime/render/review evidence;
- accepted/rejected/unresolved state from #200 when available;
- rework/retries/salvage;
- failure class;
- tokens/model calls/wall time/parallel-worker count if directly observable;
- exact next action.

### Learning checkpoints

After any of these triggers, append a short **RETROSPECTIVE INFERENCE** checkpoint:

- five material routing decisions;
- any provider/model escalation;
- any capacity/auth interruption;
- any task requiring a second model after a capability failure;
- any worker result rejected by #200;
- any expensive worker that turns out to have been unnecessary;
- any lower-cost worker that succeeds on work previously assumed to require escalation.

At each checkpoint calculate only what the observed data supports. Useful provisional measures include:

- first-pass technical success by task type/difficulty/model;
- Manager acceptance rate by model/task class;
- rework/retry count;
- salvage success after provider interruption;
- capability failures versus environment/gate/authority failures;
- escalation success rate;
- cases where a cheaper route likely would have sufficed;
- cases where escalation found a defect or prevented risky integration;
- observed wall time/tokens/calls when available;
- coordination overhead and Unity serialization delays where observable.

These are hypotheses/measurements, **not automatic future routing policy**. Proposed routing-policy changes go to #200/user for evaluation.

## Child-task routing and independence rules

- Every substantive child gets its own claim before production edits/Unity.
- Before production edits/Unity, mark the child `[RUNNING]` with actual provider/model, branch, worktree and starting HEAD.
- Claims do not grant ownership; live `active-tasks.md` remains authoritative.
- One writable worktree/lane per owner; one primary writer per subsystem/file set.
- Never stash/reset/discard/clean/overwrite unrelated dirty work.
- Heavy Unity remains serialized machine-wide under the authoritative lock.
- A historical free lock receipt does not prove current availability.
- Verification-only tasks do not gain production repair authority automatically.
- If a gate exposes a genuine production defect outside the packet, stop and return a bounded repair request to #200 unless the existing claim expressly grants that repair scope.
- Independent reviewer must be independent of the implementing thread. A helper child of the same implementer does not count as independent review.

## Current execution queue

Always refresh #200, #201, #91 and live `active-tasks.md` before acting. At handoff publication the intended sequence is:

### A. Immediate critical path — #196

**MFM-08 MapPlanning Contract Reconciliation 01**

- Claim: #196.
- Exact production candidate: `e5fb424635f16182748efcb0539e8a49539495a9`.
- Branch: `verify/mfm-08-mapplanning-contract-reconcile-v1`.
- Intended registered Unity lane: `/media/jer/ZX20/CCF-U1`, but fresh preflight is mandatory.
- Scope: verification-only, state-aware reconciliation of the stale exact-five-tab expectation; original five tabs remain required, authorised `Treatment review` is additive in MFM/V2, historical/non-MFM five-tab contract preserved.
- Required result: complete authoritative `RackV2MapPlanning` gate runs all checks and passes; lane/Editor/lock clean.
- Difficulty: **3/5** — bounded verification repair with test-weakening risk + Unity.
- Default routing under this new policy: **DeepSeek may take the bounded static/test edit if it has the required local/Unity capability; use Sonnet 5.5 Medium if the runtime/tooling or semantic reconciliation is not a good DeepSeek fit.** Do not use Opus unless a real capability blocker justifies it.
- Return to #200 for Manager verdict. Do not self-upgrade #156.

### B. Current-lineage debt — #198 and #199

**#198 Guided-v2 v24 / D-066 Contract Reconciliation 01**

- Verification-only first.
- Reconcile stale v23/pre-D-066 guided/SequentialObjectives expectations to accepted v24/D-066 persistence semantics without skipping real-input/review/save-load coverage.
- Difficulty: **3/5**. DeepSeek default if capable; Sonnet for Unity/semantic difficulty.

**#199 Scenario One Combined HUD / Focus Regression Repair 01**

- Real current-lineage UI/focus regression; protected assertions were previously green and must not be weakened.
- Root-cause estimate/minimap, competitor panel/tags and FOCUS_04/06/07 in both V2 and legacy modes.
- Difficulty: **4/5**. Default **Sonnet 5.5 Medium** because this is cross-screen Unity UI/focus production work. Escalate to Opus only after a classified Sonnet capability failure or if evidence reveals genuinely 5/5 cross-system risk.

These two are pre-Tier-3 debt. Follow #200's release/serialization decisions; do not let them silently displace the critical path.

### C. S1-C / S1-D

- S1-C T-08 final history qualification is dependency-cleared on accepted T-07 but must be freshly repinned/released by #200 and serialized through Unity.
- S1-D T-08 multi-cycle witness is qualification-only on its released lineage unless #200 republishes a later exact candidate.
- S1-D T-09 is browser/read-only independent review and therefore belongs to #200 after T-08 PASS, not this lane.

### D. MFM / parity chain

After #156 is Manager-approved and integrated on an exact combined lineage:

1. MFM-09 teaching / Work Plan integration — repin/release by #200 first.
2. MFM-10 species-parity guard — must return `SPECIES GUARD QUALIFIED`.
3. PARITY-09 fresh repin/integrated qualification.
4. MFM-11 calibration impact, including the carried MFM-03 live/cross-year residual.
5. MFM-12 integrated MFM qualification.

Route each bounded child by actual difficulty/capability, not task-code prestige.

### E. Final Tier-3

Only after required Rack V2, PARITY, S1-C, S1-D and MFM components are accepted/integrated on one exact candidate:

- execute the full integrated Tier-3 qualification defined by the prepared packet/current live authority;
- include required save/reference/Legacy compatibility, rendered/runtime matrices and complete-loop evidence;
- return exact success token only when earned:
  `SCENARIO ONE PLAYER-TEST CANDIDATE READY — USER PLAYTEST REQUIRED`.

That token means ready for the user's personal playtest, **not user acceptance**.

**No Windows build before the user personally playtests that exact candidate and explicitly authorises a build afterward.**

## Parallelism strategy

Use parallel workers only when it creates clear throughput without ownership/Unity conflict or excessive evidence ambiguity.

Preferred pattern:

- one serialized Unity child at a time;
- one or more dependency-independent offline/static children only when file ownership is disjoint and their work cannot invalidate the active Unity candidate;
- #200 continuously handles browser review/repin/status in parallel;
- avoid speculative parallel production on tasks whose base will be invalidated by an imminent integration.

Record why each parallel dispatch was worth the coordination cost.

## Stop / escalation conditions

Return immediately to #200/user when:

- a required product decision is not settled by current authority/spec;
- a child needs save/schema/reference/product behaviour outside its authorised scope;
- two writers overlap on a protected/shared subsystem;
- integration choice is materially unsafe or ambiguous;
- a required independent review cannot be obtained;
- a provider/model needed for a mandatory capability is unavailable and no safe alternative exists;
- local state cannot establish exact branch/HEAD/cleanliness/ownership safely;
- any worker proposes weakening a test merely to get green;
- Windows build becomes the next step — it remains prohibited pending user playtest + explicit authorization.

Do not stop merely because one worker/provider fails if safe rerouting is available and the task remains authorised.

## Explicit research-source restriction

The user previously instructed the Manager **not to act on** `Docs/Handoffs/Research-Source-Consolidation-Audit-01/HANDOFF.md` / #194. This long-running task must not turn the orchestration evidence work into research-source cleanup or execute #194. Treat that material as informational only unless the user explicitly lifts the restriction.

## Required final outputs from #201

Before declaring this long-running orchestration complete, publish:

1. `Docs/Handoffs/ScenarioOne-Two-Lane-Continuation-20261031/ORCHESTRATION-LEDGER.md` — append-oriented event evidence;
2. `Docs/Handoffs/ScenarioOne-Two-Lane-Continuation-20261031/ORCHESTRATION-RESULT.md` — final run handoff/result;
3. all normal child task handoffs required by AgentWorkflow.

`ORCHESTRATION-RESULT.md` must include:

- exact final candidate/integration status;
- all material delegated attempts and outcomes;
- completed/failed/cancelled tasks;
- provider capacity/auth failures;
- retries/salvages;
- routing/decomposition decisions and alternatives;
- decisions using direct versus inferred/unknown capacity evidence;
- model/provider escalation reasons and outcomes;
- acceptance/rejection outcomes from #200;
- actual tokens/calls/wall time where observable;
- Unity/editor lanes and serialized operations;
- worktrees/branches;
- rendered/runtime qualification;
- independently reviewed save/reference/determinism work;
- any changed protected anchors/baselines;
- unresolved work;
- exact next action;
- **Adaptive Routing Learning Assessment** with observed metrics, retrospective hypotheses + confidence, and proposed experiments/policy changes kept explicitly non-authoritative until accepted.

“Worker completed” is never a substitute for Manager acceptance or verified runtime evidence.

## Difficulty / best-suited orchestrator

**Difficulty: High** because this is a long-running multi-task orchestration problem spanning several local workers, one serialized Unity resource, dependency repinning and adaptive routing evidence.

**Best-suited primary orchestrator:** DeepSeek long-running/deep-research orchestration agent with local execution/delegation visibility.  
**Principal escalation worker:** Claude Sonnet 5.5 Medium.  
**Highest escalation:** Claude Opus 5.5 Low, then Medium only when justified by a classified capability/high-consequence need.

## Copy/paste dispatch line

`Take Scenario One Long-Running Deep Orchestration 01 under claim #201 using Docs/Handoffs/ScenarioOne-Two-Lane-Continuation-20261031/DEEP-ORCHESTRATION-HANDOFF.md. DeepSeek is the orchestrator/default worker for tasks it can reasonably handle; use Claude Sonnet 5.5 Medium for harder work and escalate to Opus 5.5 Low (then Medium only if justified) after a classified capability/high-consequence need, not for ordinary gate/environment/capacity failures. Coordinate all non-browser local/Unity work through child claims while browser/Manager lane #200 owns reviews, releases and active-tasks writes. Start from fresh live state with #196 as the first current critical task unless #200/live coordination has superseded it. Maintain the mandatory adaptive-routing ORCHESTRATION-LEDGER continuously using the supplied V2 appendix/20261030 ledger/bootstrap evidence contract, and publish ORCHESTRATION-RESULT at the end. Preserve one-writer/Unity locks, protected anchors, Reference Future/Legacy compatibility, and no Windows build before user playtest + explicit authorisation.`

**User action required:** launch the long-running/deep-research orchestration task with the dispatch line above or point it directly to claim #201 and this handoff.