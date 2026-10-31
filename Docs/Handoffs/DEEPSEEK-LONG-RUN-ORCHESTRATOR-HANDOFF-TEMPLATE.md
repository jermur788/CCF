# DeepSeek Long-Run / Overnight Orchestrator Handoff Template

Use this template for substantial unattended work: overnight runs, workday-away runs, multi-task integration/qualification waves, or any job where one orchestrator must route several dependent tasks across multiple provider/subscription pools.

This is a reusable handoff template, not authority to start any particular task. A real run must be instantiated with exact objectives, candidate SHAs, task claims, ownership, branches/worktrees, current live coordination state and user/Manager constraints.

## 1. Dispatch summary

- **Handoff name/path:** `<exact run handoff path>`
- **Plain-English purpose:** `<what the unattended run is trying to achieve>`
- **Difficulty:** `<Low / Medium / High or 1-5>` — `<reason>`
- **Primary orchestrator:** **DeepSeek** — use the strongest suitable currently available DeepSeek orchestration/reasoning configuration.
- **Execution model:** DeepSeek remains the parent orchestrator; child work may be divided among currently available DeepSeek, Claude and OpenAI/Codex subscription pools according to task fit, independence requirements, live capacity and switching cost.
- **Exact orchestration lane:** `<branch / browser lane / worktree>`
- **Parent claim:** `<issue # / status>`
- **Child claims:** one claim for every substantive dispatched task.
- **User action required at dispatch:** `<none unless explicitly stated>`
- **Copy/paste dispatch line:** `Run <RUN-NAME> as the DeepSeek parent orchestrator. Refresh live coordination/provider/Unity state first; then decompose, route, checkpoint, review, integrate and continue automatically under this handoff. Maintain the live Orchestration Evidence Ledger and include the full V2 Orchestration Decision Appendix in the final handoff.`

## 2. Authority and protected constraints

At run start, refresh and obey the current project authority order and live coordination state. Do not infer accepted product intent, implementation status, Unity verification, integration or acceptance from this handoff alone.

Before dispatching or editing:

1. Refresh `Docs/Project/coordination/active-tasks.md`.
2. Refresh the parent and relevant child GitHub claims.
3. Confirm exact candidate/base SHA, branch and worktree for every writable lane.
4. Confirm one primary writer per subsystem/file set.
5. Preserve unrelated dirty work. Never stash, reset, discard, clean, overwrite or absorb another owner's work.
6. Separate browser-capable review/research/coordination from local C#/Unity work wherever practical.
7. Serialize heavy Unity machine-wide. A historical `free` receipt never proves current availability.
8. No Windows build before the user personally playtests the Scenario One candidate and explicitly authorises a Windows build.
9. Do not weaken tests, fixtures or acceptance criteria merely to obtain a green result.
10. Keep `IMPLEMENTED`, `REVIEWED`, `INTEGRATED`, `VERIFIED` and `ACCEPTED` distinct.

Any protected user/Manager decision for the specific run must be copied here verbatim or precisely paraphrased:

- `<protected decision 1>`
- `<protected decision 2>`

## 3. Parent orchestrator role

DeepSeek is the **parent orchestrator and default worker provider** for work it can reasonably handle. It owns decomposition, routing, dependency tracking, provider/pool capacity tracking, child dispatch, handoff inspection, review routing, safe integration decisions allowed by the packet, and the live evidence ledger.

The orchestrator must not behave as one giant implementation worker. It should split work when that improves parallelism, independence, context quality, provider utilisation or recovery from quota/context exhaustion.

The orchestrator must continue automatically while the next safe step is clear. Stop/escalate only for a product decision, material ambiguity, ownership conflict, mandatory review, unsafe integration, unresolved verification failure, user test gate or other explicit blocker.

## 4. Live provider/subscription policy

Provider/model availability is runtime state, not a promise in this template. Query the actual available pools/models at run start and whenever a material capacity event occurs.

### Required pool states

Track every usable provider/subscription pool as one of:

- `AVAILABLE`
- `BUSY`
- `COOLDOWN`
- `LOGIN-UNAVAILABLE`
- `UNKNOWN`

For each pool maintain:

| Provider / subscription pool | State | Exact model / effort available | Remaining-capacity estimate | Reset/window | Evidence source + confidence | Current work | Capacity-to-completion |
|---|---|---|---|---|---|---|---|
| DeepSeek | UNKNOWN | UNKNOWN | UNKNOWN | UNKNOWN | UNKNOWN | parent orchestration | UNKNOWN |
| Claude A | UNKNOWN | UNKNOWN | UNKNOWN | UNKNOWN | UNKNOWN | none | UNKNOWN |
| Claude B | UNKNOWN | UNKNOWN | UNKNOWN | UNKNOWN | UNKNOWN | none | UNKNOWN |
| OpenAI/Codex A | UNKNOWN | UNKNOWN | UNKNOWN | UNKNOWN | UNKNOWN | none | UNKNOWN |
| OpenAI/Codex B | UNKNOWN | UNKNOWN | UNKNOWN | UNKNOWN | UNKNOWN | none | UNKNOWN |

Only replace `UNKNOWN` with direct provider/runtime/T3/user evidence. Do not infer quota, reset time or successful login merely from elapsed time.

At every material dispatch record **capacity-to-completion** as `sufficient`, `marginal`, `insufficient` or `unknown`, plus expected interruption/switching cost.

### Routing defaults

- **DeepSeek:** parent orchestrator and default worker for browser/static work, research synthesis, bounded implementation, verification preparation, straightforward code work and tasks where it is clearly capable.
- **Claude subscriptions:** reserve for harder architecture/reasoning/review work where the additional capability is useful. Prefer Sonnet at a moderate reasoning level as the normal strong lane. Escalate to Opus only when task risk/difficulty materially justifies it; use the lowest reasoning level that remains adequate.
- **OpenAI/Codex subscriptions:** use when live availability and task fit favour a strong code/implementation/review lane, or to create cross-provider independence. Record the exact observed model and reasoning configuration rather than assuming a version from this template.
- **Independent review:** where independence is required, do not assign the reviewer to the same provider/model lineage that produced the implementation unless the governing packet explicitly allows it.
- Never silently switch provider, model, reasoning level or subscription pool. Record every material switch in the ledger.

Cost/capacity never waives a required review, qualification or acceptance gate. Defer, reroute or leave the gate unresolved.

## 5. Five-hour window / quota / authentication behaviour

Where a provider uses a rolling subscription window (commonly a five-hour window), treat the reset as **evidence-driven**:

1. While usable, checkpoint durable work early: commit/push when safe, publish partial handoff evidence, and record exact branch/worktree/HEAD.
2. If a pool reports quota exhaustion, subscription sharing limit, authentication loss or equivalent:
   - mark the pool `COOLDOWN` or `LOGIN-UNAVAILABLE`;
   - record the exact provider error text when available;
   - record reset time as `UNKNOWN` unless directly supplied;
   - preserve all pushed commits and unrelated/uncommitted work;
   - classify the event as a **provider capacity/auth failure**, not a code failure;
   - transfer the remaining task only after checking ownership and avoiding two writers on the same lane.
3. Start/resume a replacement worker in another available pool when it is safe and useful. Give it the exact durable checkpoint, branch/worktree, uncommitted-state warning and remaining acceptance criteria. Do not restart from scratch if work can be salvaged.
4. When the original pool appears to have reset, **recheck availability directly**. Do not assume it has reset because enough time passed.
5. If the original pool becomes available again, do not create a duplicate writer. Either assign it a different queued task or formally hand back/resume the task only after the current worker has checkpointed and relinquished ownership.
6. Record the switch, salvage/restart cost and result in the live ledger.

No quota/authentication bypass is authorised.

## 6. Context-pressure and restart policy

Track the parent orchestrator's context limit/usage when observable, including compactions and material context-pressure concerns. Prefer runtime/provider telemetry over model self-report.

If the parent or a child approaches context exhaustion:

- write a durable checkpoint before restarting;
- include exact objective, completed work, unresolved work, branch/worktree/HEAD, dirty-state warning, claims, locks, tests run, failures, protected decisions and next action;
- restart the worker only from the durable checkpoint;
- record the restart as a routing event, including switching/context cost;
- never reconstruct missing evidence after the fact.

## 7. Decomposition and concurrency rules

Before dispatch, create a task graph with dependencies and writer boundaries.

For each work unit record:

| Work unit | Parent | Type | Risk | Dependencies | Writable files/subsystem | Lane | Required review/Unity | Preferred provider | Status |
|---|---|---|---|---|---|---|---|---|---|
| `<id>` | `<parent>` | browser / research / code / Unity / review / integration | `<risk>` | `<deps>` | `<boundary>` | `<branch/worktree>` | `<gates>` | `<provider>` | READY/BLOCKED/RUNNING/HANDED-OFF |

Rules:

- One writable lane/worktree per owner and one primary writer per subsystem/file set.
- Parallelise independent browser/research/static work freely when useful.
- Parallelise code only across non-overlapping ownership boundaries.
- Serialize heavy Unity operations.
- Prefer browser-first review/preparation once a candidate is pushed.
- Every substantive child task gets a GitHub claim. Before production edits/Unity, mark it `[RUNNING]` with provider/model, branch, worktree and starting HEAD. If unable to start, mark `[BLOCKED]`. At durable handoff mark `[HANDED-OFF]`, link the handoff and close when appropriate.
- Claims track execution presence only; `active-tasks.md` remains the ownership authority.

## 8. Child worker packet minimum

Every child dispatch must contain:

- exact task and purpose;
- authority/spec/packet path;
- exact start SHA and expected candidate lineage;
- exact branch/worktree/lane;
- writable and forbidden file/subsystem boundaries;
- claim issue and required status update;
- dependencies and blockers;
- required verification tier/gates;
- protected anchors/reference/save/determinism expectations;
- required handoff path;
- exact success/blocker tokens where the task uses them;
- instruction to preserve unrelated work and stop on ownership overlap;
- instruction not to over-claim: offline/static completion is not Unity qualification, and completion is not acceptance.

## 9. Live Orchestration Evidence Ledger

Create a run-local ledger immediately, for example:

`Docs/Handoffs/<RUN-NAME>/ORCHESTRATION-LEDGER-<DATE>.md`

Append evidence **during** the run. Never reconstruct unknown capacity, routing rationale or resource values later.

### Evidence vocabulary

- **OBSERVED FACT:** directly supported by user/provider/repository/runtime/test evidence.
- **ORCHESTRATOR CLAIM:** what the orchestrator believed or why it chose a route.
- **RETROSPECTIVE INFERENCE:** a later conclusion drawn from outcomes.

Never relabel an orchestrator claim or retrospective inference as an observed fact.

### Starting policy record

Record:

- timestamp;
- user objective and hard constraints;
- optimisation preferences: quality, money, tokens, elapsed time, compute/resource use and exploration tolerance;
- provider/model order and escalation policy;
- concurrency/one-writer/review rules;
- protected Manager/user decisions;
- initial provider ledger;
- initial task/dependency graph;
- exact repo/integration/coordination state.

### Event template

Append one event for every material dispatch, switch, retry, capacity failure, restart, decomposition change, handoff, review verdict, Unity acquisition/release, integration or acceptance decision:

```md
### EVENT — <timestamp>

- **Work unit:**
- **Parent work unit:**
- **Evidence class:** OBSERVED FACT / ORCHESTRATOR CLAIM / RETROSPECTIVE INFERENCE
- **Task characteristics / risk:**
- **Decomposition topology:**
- **Previous provider/model/reasoning/pool:**
- **Selected provider/model/reasoning/pool/role:**
- **Alternatives considered:**
- **Reason / routing rule:**
- **Live capacity evidence:**
- **Capacity source + confidence:** provider / runtime / T3 / user / inferred / unknown
- **Capacity-to-completion:** sufficient / marginal / insufficient / unknown
- **Reported reset/window:**
- **Expected interruption / switching cost:**
- **Exploit / explore:**
- **Routing-policy / knowledge version:**
- **Previous failure/outcome influencing decision:**
- **Branch/worktree or browser lane:**
- **Actual outcome:**
- **Tests/runtime/review evidence:**
- **Handoff/commit/claim reference:**
- **Accepted / rejected / unresolved:**
- **Rework / retry / salvage:**
- **Failure classification if any:**
- **Resource observations:** tokens/model calls/reasoning/wall time/parallel workers/provider failures when observable
- **Next action:**
```

## 10. Mandatory Orchestration Decision Appendix — V2

The final run/result handoff must include the following appendix. Capture the underlying evidence during work; do not backfill invented values.

### 10.1 Policy

Record orchestrator provider/model/reasoning; user objective, hard constraints and optimisation preferences (quality, money, tokens, time, resource/compute use, exploration tolerance); provider/model order and escalation; concurrency/one-writer and review rules; protected Manager/user decisions.

### 10.2 Capacity + context

Maintain per provider/pool: state (`AVAILABLE/BUSY/COOLDOWN/LOGIN-UNAVAILABLE/UNKNOWN`), remaining-capacity estimate, reset/window, source/confidence and current work. At dispatch record capacity-to-completion (`sufficient/marginal/insufficient/unknown`) and expected interruption/switching cost.

For the orchestrator record context limit, used/estimated context, source/confidence, compactions and material context-pressure concerns when available. Prefer runtime/provider evidence over model self-report.

### 10.3 Routing + decomposition

For each material dispatch, switch, retry or decomposition decision record: task/work unit + parent; task characteristics/risk; decomposition topology; previous and selected provider/model/reasoning/role; known alternatives; reason/rule; live capacity/context evidence; capacity fit; switching cost; `exploit/explore`; routing-policy/knowledge version if available; prior outcome affecting the choice. Unknown stays **UNKNOWN**.

### 10.4 Outcomes + resources

For each significant worker/subtask record actual provider/model/config; outcome; tests/runtime/review evidence; handoff/commit; accepted/rejected/unresolved; rework/retries/salvage; failure class. **Completed is not acceptance.**

Where observable record input/output tokens, model calls, reasoning effort, wall time, parallel workers and provider failures. Any compute/resource estimate must state source/confidence; never label a proxy as measured energy.

### 10.5 Learning assessment

Record what routing/decomposition worked; waste/incorrect choices; cheaper/lower-resource strategies likely sufficient; justified escalation; capacity/context effects; coordination overhead; missing telemetry; candidate experiments/policy changes. Mark hypotheses + confidence. They are not facts or active policy until evaluated/accepted.

### 10.6 Evidence rule

Keep distinct:

- **Observed fact:** provider/runtime/repo/test evidence.
- **Orchestrator claim:** what it believed/why it acted.
- **Retrospective inference:** later conclusion.

Preserve provenance; never convert claims/inferences into facts.

### 10.7 Final summary

Include attempts; completed/failed/cancelled work; capacity failures; retries/salvages; material routing/decomposition decisions; direct vs inferred/unknown capacity decisions; resource observations; review findings; unresolved work; exact next action.

For game work also include Unity/editor lanes, serialized Unity operations, worktrees/branches, rendered/runtime qualification, independently reviewed save/reference/determinism work, and changed protected anchors/baselines.

## 11. Review, integration and verification policy

Use proportionate verification:

- **Tier 1 FAST:** implementation/calibration questions.
- **Tier 2 FEATURE QUALIFICATION:** bounded feature handoff.
- **Tier 3 FULL INTEGRATED QUALIFICATION:** milestone/release candidate only.

For returned worker handoffs:

1. Inspect the actual evidence and exact candidate.
2. Issue `APPROVED`, `NEEDS CHANGES` or `REJECTED` for the relevant scope.
3. Update/close the worker claim as appropriate.
4. Create the next executable packet/handoff when the next step is clear.
5. Integrate only when ownership, review and verification requirements are satisfied.
6. Do not treat GitHub state alone as proof of local cleanliness, Unity import/compile, runtime behaviour or rendered state.

Save/reference/determinism work should receive genuinely independent review when required by the governing packet. Protected anchors/baselines must never be silently re-baselined.

## 12. Final run handoff requirements

The final handoff must state in plain language:

- what changed;
- what passed and failed;
- what is implemented vs reviewed vs integrated vs verified vs accepted;
- every unresolved blocker;
- provider/subscription capacity events and salvage/restarts;
- exact branches/worktrees/SHAs and claim statuses;
- Unity/Editor/lock state and whether lanes were returned clean;
- any rendered/runtime qualification actually performed;
- whether any protected save/reference/determinism anchor changed;
- the complete V2 Orchestration Decision Appendix;
- the exact next executable action;
- exact user action required, or `none`.

A long run is not complete until its substantive implementation, qualification, review, integration and research outputs have durable GitHub Markdown handoffs. Local-only output is incomplete.

## 13. Run instantiation checklist

Before giving this to DeepSeek, replace all placeholders and attach/link the run-specific sources:

- [ ] Current `active-tasks.md` refreshed.
- [ ] Exact parent/integration SHA recorded.
- [ ] Parent claim created/updated.
- [ ] Child task graph prepared with one claim per substantive dispatch.
- [ ] Browser vs local/Unity lanes separated.
- [ ] Writable boundaries and ownership checked.
- [ ] Provider/subscription pools listed but capacity left `UNKNOWN` until observed.
- [ ] User/Manager protected decisions copied into the handoff.
- [ ] Unity serialization and no-Windows-before-playtest rule included.
- [ ] Run-local orchestration ledger path created.
- [ ] Final handoff path named in advance.
- [ ] V2 appendix required in the final handoff.
- [ ] Copy/paste dispatch line supplied.

---

**Template provenance:** derived from the Project's `ORCHESTRATION-EVIDENCE-APPENDIX-V2.md`, the `PLAYER-TEST-OVERNIGHT-01 — Orchestration Evidence Ledger — 2026-10-30`, current CCF coordination/ownership rules, and the user's standing requirement that large unattended runs use a DeepSeek parent orchestrator that actively redistributes work across available subscription pools while preserving live capacity and routing evidence.