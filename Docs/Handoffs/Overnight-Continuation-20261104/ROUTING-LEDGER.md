# CCF Overnight DPC — live V2 routing, capacity and outcome ledger

**Opened:** 2026-11-04 00:07 UTC. **Canonical owner / single writer:** **DeepSeek DPC orchestrator** under [#254](https://github.com/jermur788/CCF/issues/254) / #203. Overall Manager #200 authors the initial contract and performs final acceptance.  
**Window:** user's overnight 2026-11-04 continuation, followed by durable handoff at the last available approved stage. **Run state at creation:** PREPARED / no DPC execution observed.  
**Related already-live programme ledger:** [Access-spine V2 ledger](../ScenarioOne-Access-Spine-20261103/ORCHESTRATION-LEDGER.md), which remains authoritative for #253 events and retains the full earlier #246–#251 research history. This overnight ledger records cross-programme routing and references the access-spine ledger for #253 rather than rewriting its history.
**Binding launch:** [MASTER-HANDOFF.md](MASTER-HANDOFF.md).

## Absolute ledger rules

1. **DPC alone writes this file** from one docs lane; worker agents return observation/event packets in their own handoffs or issue comments. Concurrent workers **do not** push this shared ledger. DPC uses an **append-only serial update**, fetches the latest main or owned branch version before each append, rebases/reconciles only its own additions if concurrent updates, and never overwrites a newer event. A real GitHub commit/PR/issue link makes an entry durable.
2. **Write an event at each material event:** worker/provider selection, task claim start, quota/context preflight, priority or model choice, independent review dispatch, escalation, retry, environment/Unity block, provider cooldown/reset wait, return from cooldown, re-routing, test result, handoff publication, integration decision or suspension. **Do not wait until morning to transcribe choices from memory.** Use raw logs where available.
3. Every event must distinguish **OBSERVED FACT**, **ORCHESTRATOR CLAIM**, and **RETROSPECTIVE INFERENCE**. If per-provider remaining tokens, ticket availability, quota window/reset, context use, price or cost is not exposed: **UNKNOWN**, never guess. `enabled`/login success is **not** proof of remaining quota. Capture observed labels, exact runtime model/provider, effort setting (Medium/High/Extra High/Opus as available), and any switches.
4. Record **why** a model was used rather than merely its name: phase, difficulty/risk, available alternative(s), reason for preferring it, exploit/explore, quality, estimated risk, capacity-to-completion and switching cost, prior attempts, capability-vs-environment/quota/permission classification. The user's escalation authority starts with Sonnet 5.5 or GPT-6.1 **Medium**. A higher-effort mode or **Opus 5.5** is allowed where task difficulty/review stakes justify it or after a documented reasoning failure. A quota limit is not reasoning failure; waiting for a real reset or routing a different enabled sub is preferable when safe.
5. Publish-time numeric claims require **source file/key/row re-extraction and sum reconciliation**, including review verdict totals, reference hashes and test counts. Do not trust memory. Record corrections and preserve superseded originals with provenance. Never claim a gate passed if failed with waiver.
6. Keep execution states distinct: **DRAFT / APPROVED SPEC / RUNNING / IMPLEMENTED / REVIEWED / INTEGRATED / VERIFIED / ACCEPTED**. A worker handoff is not a Manager acceptance; a docs PR is not a production merge. Publish branch/HEAD/file ownership, actual test/run/lock proof and returned handoff.
7. **Safety gates remain absolute:** #200 owns `active-tasks.md`; original player-test branch, user saves, Legacy40, frozen Reference Future v1 and protected forestry are unchanged without proper review; no unauthorized Rack V2/MFM spec overrides; one Unity machine-wide lease for all heavy work, fresh process/disk/preflight every admission; no Windows build before completed personal playtest and explicit subsequent authorisation. Never force-unlock, kill another process, overwrite dirty work, bypass quotas or simulate real provider resets.

## Per-event template — copy and fill at the time

### EVENT <sequential number> — <UTC timestamp> — <task / route / outcome>

- **Evidence class:** OBSERVED FACT / ORCHESTRATOR CLAIM / RETROSPECTIVE INFERENCE (separate per claim)
- **Task and claim / parent:** issue, status, spec/packet authority, priority, serial/parallel dependency
- **Provider pool; model; effort; runtime identity:** exact observed identifiers, not intended ones
- **Alternatives considered / why this route:** fit, cost/review stakes, exploit/explore, expected context-to-completion
- **Pool status:** AVAILABLE / BUSY / COOLDOWN / LOGIN-UNAVAILABLE / UNKNOWN; evidence source/time
- **Remaining quota/window/reset:** observed value + source, or UNKNOWN
- **Context used / limit / compactions / tokens / wall time:** observed figures + source, or UNKNOWN
- **Capacity-to-completion:** sufficient / marginal / insufficient / unknown, with evidence
- **Escalation, retry, wait and switch cost:** reason, classified failure, checkpointed artifacts
- **Ownership:** source ref/starting HEAD, branch, worktree, owned files, Unity lease/process/disk if relevant
- **Work/test/provenance:** commands/artifact URLs, original failures, review counts raw source, corrections
- **Outcome and authority:** IMPLEMENTED / REVIEWED / INTEGRATED / VERIFIED / ACCEPTED / FAILED / BLOCKED / HANDED-OFF / UNKNOWN
- **Next eligible step, dependency and Manager/user action:** exact claim/handoff link

## Initial events

### EVENT 001 — 2026-11-04 00:07 UTC — user authorises overnight progression

- **Evidence class:** OBSERVED FACT (latest direct user message).
- **Task:** long-running DeepSeek DPC orchestration; serial or independent parallel tasks with safe autonomy where existing task packets/specs authorise them.
- **Provider policy:** start with **Sonnet 5.5 Medium** and **GPT-6.1 Medium**; available other subs permissible; escalations to higher ticket/effort and Opus 5.5 authorised for justified complexity. Actual login, quotas, reset times, ticket cost, context, tokens, available runtime model list all **UNKNOWN** here.
- **Continuation:** real capacity cooldown may be waited out or a qualified worker substituted, after persisting checkpoints; no claim the browser Manager can run an autonomous background agent.
- **Safety:** separate code fixes from new high-ambiguity specs, preserve playtest candidate, no Windows build.
- **Next:** DeepSeek starts #254 only on real execution, records fresh provider/routing/ownership preflight and child claims. At initial ledger creation, no worker execution is observed.

### EVENT 002 — 2026-11-04 00:08 UTC — user reiterates live routing ledger

- **Evidence class:** OBSERVED FACT (latest user interruption).
- **Requirement:** every routing choice, model escalation, capacity change, fallback, wait/reset, result and review must be contemporaneously appended to this ledger (and programme ledger for #253), not summarised after the fact. No observed provider state or usage was supplied; **UNKNOWN**.

## Morning / terminal checkpoint

DPC must publish `OVERNIGHT-ORCHESTRATION-RESULT.md` before ending a session or at a context/quota transition. Record each child claim status and handoff link, what passed/failed, unmerged branches, orphaned locks, pending reviews, actual wait/restart state and first next safe executable work. If provider truly cannot continue until reset, publish a **BLOCKED/COOLDOWN** checkpoint with observed reset evidence (or UNKNOWN), rather than promising an unconfigured timed delivery.
