# Scenario One access-spine programme — live orchestration evidence ledger

**Created:** 2026-11-03. **Owner:** DPC parent orchestrator for local routing/evidence; Overall Manager #200 for acceptance, published coordination and integration decisions.  
**Scope:** #246 read-only reconnaissance and any later **separately approved** access-spine/Objective 1 implementation or verification tasks. **Status:** DRAFT spec, #246 READY; no production implementation authorised.

This is the live **append-only, run-scoped** ledger. The [30 October historical ledger](../PlayerTestReadiness-Overnight-01/ORCHESTRATION-LEDGER-20261030.md) remains distinct; [#241](https://github.com/jermur788/CCF/issues/241) owns its own separate backfill. Do not rewrite, duplicate or contaminate historical entries.

## Evidence contract — mandatory V2

Follow the full **ORCHESTRATION-EVIDENCE-APPENDIX-V2** supplied to the project and the established [local orchestration instructions](../ScenarioOne-Two-Lane-Continuation-20261031/LOCAL-DEEP-ORCHESTRATION-RESUME-20261031T1221Z.md). Capture **as work happens**, not retrospectively inventing usage or capacity.

For **every material route/dispatch/retry/switch/completion**, append timestamped entries with:
- **OBSERVED FACT / ORCHESTRATOR CLAIM / RETROSPECTIVE INFERENCE** distinctions.
- User goal, task risk/difficulty, phase and exact GitHub claim; model/provider/pool and **exact observed medium reasoning/effort configuration**.
- Alternatives considered, reason for selection, relative quality/cost/speed, task decomposition/parallelism, previous outcome and switching/recovery cost; mark **exploit/explore**.
- Per-pool capacity states `AVAILABLE`/`BUSY`/`COOLDOWN`/`LOGIN-UNAVAILABLE`/`UNKNOWN`; quota/window/reset/remaining context **only if directly evidenced**, source/confidence; capacity-to-completion `sufficient`/`marginal`/`insufficient`/`unknown`.
- When observable: token input/output, model calls, effort, elapsed wall time, compactions, context used/limit, interruptions, authentication failures and subscription rollovers. Never invent quota or energy-use metrics.
- Actual branch/worktree/starting HEAD and writer/Unity lease, test evidence, handoff link, result **IMPLEMENTED/REVIEWED/INTEGRATED/VERIFIED/ACCEPTED** as separate milestones, retries and salvage, accepted/rejected/unresolved; reason for stop/continuation.
- Retrospective assessment of routing effectiveness, wasted reruns/switches, cheaper sufficient paths, missing telemetry and next action.

**Parent DPC:** check real provider availability, quota/context and safe handoff checkpoint **before** dispatching and whenever an interruption is plausible. Prefer **Claude Sonnet 5.5 Medium** or **OpenAI GPT-6.1 Medium** according to observed capacity and task fit. These are permitted alternatives, **not proven available**; use exact runtime/provider names recorded in the event. A quota/cooldown is **not** a reasoning failure. Switch available providers when necessary and safe; preserve commits, claims and partial evidence; never bypass limits or accounts. Use cross-provider independence for review where required.

**Continuation:** while another approved, owned, non-overlapping task is ready, DPC may route it rather than wait on one provider/Unity slot. **Do not** interpret “keep development going” as permission to implement DRAFT specs, bypass #200 approvals, take another writer's files, assume the Unity lock is free, build Windows, or alter the active user playtest branch. When an authority/product/ownership gate blocks one task, issue [BLOCKED] with exact evidence and continue only with other authorised work.

## Provider / context snapshot (at ledger creation)

| Pool | State | Available model/effort | Remaining quota / reset | Context use / limit | Source / confidence | Capacity-to-completion |
|---|---|---|---|---|---|---|
| Claude pool(s) | UNKNOWN | Sonnet 5.5 Medium **user-authorised, availability unverified** | UNKNOWN | UNKNOWN | latest user instruction; availability UNKNOWN | UNKNOWN |
| OpenAI / T3 pool(s) | UNKNOWN | GPT-6.1 Medium **user-authorised, availability unverified** | UNKNOWN | UNKNOWN | latest user instruction; availability UNKNOWN | UNKNOWN |
| DPC orchestrator | UNKNOWN | user-designated parent agent; exact runtime model UNKNOWN | UNKNOWN | UNKNOWN | latest user instruction; session not yet observed | UNKNOWN |

## Events — append chronologically

### EVENT — 2026-11-03 19:50 UTC — Manager creates routing requirement / releases #246

- **Evidence class:** OBSERVED FACT (user instruction and GitHub documents); ORCHESTRATOR CLAIM (future routing preference).
- **Work unit:** [#246](https://github.com/jermur788/CCF/issues/246), read-only inspection of Scenario One access-spine/diagonal racks; parent local orchestration #201/#203, browser Manager #200.
- **User instruction:** DPC will orchestrate, monitor limits and avoid avoidable idle time; may use Sonnet 5.5 Medium and GPT-6.1 Medium; capture all material routing information and append to this ledger.
- **Preferred routing:** DPC selects between authorised medium-effort models after checking actual provider availability and capacity-to-completion; independent review should use a different provider when applicable.
- **Alternative(s):** Sonnet 5.5 Medium vs GPT-6.1 Medium; no selection or dispatch observed yet. **Exploit/explore:** UNKNOWN until dispatch.
- **Capacity, context, reset, worktree, runtime:** UNKNOWN; no local T3 state inspected by Browser Manager.
- **Result:** #246 READY, read-only; four-file spec remains DRAFT. Current published playtest candidate and protected Rack V2 contracts unchanged; no Unity/work started.
- **Next action:** DPC checks live coordination, both permitted pools, exact #246 candidate and read-only lane; marks [RUNNING] only at actual start, logs route, gathers code evidence and publishes `RECONNAISSANCE-HANDOFF.md`. On return, manager revises spec before any production.

### EVENT — <timestamp> — [append actual dispatch/switch/result]

- **Work unit / claim:**
- **Evidence class:** OBSERVED FACT / ORCHESTRATOR CLAIM / RETROSPECTIVE INFERENCE
- **Prior provider/model/effort:**
- **Selected provider/model/effort:**
- **Alternatives, reason and exploit/explore:**
- **Provider/pool state, quota/window and source:**
- **Context used/limit, compactions and source:**
- **Capacity-to-completion, switching cost:**
- **Branch/worktree, ownership, Unity status:**
- **Checks/evidence, runtime, tokens where observable:**
- **Outcome / reviewed / accepted / unresolved:**
- **Failure classification and recovery/next action:**
