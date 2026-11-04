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

## Execution events

### EVENT 003 — 2026-11-04 00:15–00:20 UTC — DPC live preflight: Git, providers, disk, Unified lease

- **Evidence class:** OBSERVED FACT (commands run in this session at these times).
- **Git:** `git fetch origin --prune` advanced `origin/main` to `8f0f230190830855112e3e7a1d7544db94a7f591` ("docs: provide one GitHub-linked dispatch line and ownership contract per overnight child"). Published player-test candidate `origin/integration/scenario-one-player-test-ready-v1` = `72b4efbd1c653fa2231f632a79030a83ff76f833`. `gh auth status` = account `jermur788`, scopes `gist, read:org, repo, workflow`, working. Repo worktree count before this session: 90.
- **Provider pools observed** (`orchestrator_capabilities`, this session): child-capable AND enabled — `claudeAgent` ("Claude A"), `claudeAgent_claude_b_pro` ("Claude B — Pro"), `codex` ("Codex"), `codex_c75d8114-…` ("ChatGPT - OpenAI B"), `opencode`. Disabled with no runnable models — `cursor`, `grok`, `antigravity`, `fx`, `muse`, `pi`. Exactly four independent frontier subscription pools are therefore available for cross-provider independence.
- **Model identifiers observed:** `claude-sonnet-5-5` (effort `low…max, ultrathink`), `claude-opus-5-5`, `claude-haiku-5-5`, `claude-fable-5-1`; `gpt-6.1-sol`, `gpt-6-astra`, `gpt-6.1-terra`, `gpt-6-sol`, `gpt-6-luna`, `gpt-5.6-sol/terra/luna` (`reasoningEffort` `low…ultra`). "GPT-6.1 Medium" is routed as `gpt-6.1-sol` + `reasoningEffort: medium`, not the model default (`low`).
- **Remaining quota/window/reset per pool: UNKNOWN.** The capability response exposes no quota, token, or reset fields; no account console was consulted, so no numeric claim is made and none will be inferred from later success/failure alone.
- **Disk (observed `df -h`):** `/media/jer/ZX20` 477G total, **19G available** (97% used) before lane creation; `/media/jer/Files` **47G available** then, **39G available** after four 1.2 GiB lane checkouts; `/` **1.4G available**. Unity admission is therefore the binding constraint, not docs capacity.
- **Unity:** no Unity **Editor** process observed. UnityHub and the licensing client are running (user's own Hub), which is not an Editor lease. Machine-wide lock status read: see EVENT 008. No Editor was started by DPC.
- **Coordination writer:** `Docs/Project/coordination/active-tasks.md` already carries #253 and #255–#260 as READY/NOT RUNNING with the conditional lanes (origin/main, reconciled 2026-11-04). Only #200 may write that file, so **DPC did not and will not edit it**; child claims are published here, on the issue, and in each PR instead. This is a stated boundary, not a missing gate.
- **Lanes created (clean, from exact bases):** `CCF-Overnight-DPC-20261104` (`orchestration/overnight-20261104-dpc`, origin/main); `CCF-MFM-OptionA-Docs-20261104` (`docs/mfm-option-a-offline-20261104-v1`, origin/main); `CCF-Overnight-ObjectiveCopy-20261104` (`feature/overnight-objective-copy-20261104`, `72b4efbd`); `CCF-Overnight-RackHandle-20261104` (`feature/overnight-rack-handle-20261104`, `72b4efbd`); `CCF-Overnight-InputSpec-20261104` (`docs/overnight-input-settings-spec-20261104`, origin/main); `CCF-Overnight-ObjectiveSpec-20261104` (`docs/overnight-objective-guidance-spec-20261104`, origin/main); `CCF-Overnight-MarkSpec-20261104` (`docs/overnight-mark-language-spec-20261104`, origin/main); `CCF-Overnight-FeedbackVerify-20261104` (`verify/overnight-playtest-feedback-20261104`, `72b4efbd`). All under `/media/jer/Files/CCF/Operations/`; none existed beforehand.
- **Outcome:** HANDED-OFF → dispatch wave 1.
- **Next:** EVENT 004–007 dispatch four independent lanes across four distinct pools.

### EVENT 004 — 2026-11-04 00:2x UTC — dispatch #255 Objective copy → Codex `gpt-6.1-sol` medium

- **Evidence class:** OBSERVED FACT (delegation call) / ORCHESTRATOR CLAIM (fit reasoning).
- **Task:** #255, PACKET-ONLY bounded production copy fix, P0, independent of every other lane.
- **Provider/model/effort:** pool `codex` ("Codex"), `gpt-6.1-sol`, `reasoningEffort: medium`, serviceTier default.
- **Why:** lowest-risk but production-touching; needs a strong code-capable Medium worker whose output can be reviewed by a *different* family. Codex is a distinct pool from both Claude subscriptions, preserving independence for the later review.
- **Pool status:** AVAILABLE (enabled, child-capable). **Quota/reset:** UNKNOWN. **Capacity-to-completion:** judged sufficient — two small C# text sources, no Unity required.
- **Ownership:** branch `feature/overnight-objective-copy-20261104`, worktree `/media/jer/Files/CCF/Operations/CCF-Overnight-ObjectiveCopy-20261104`, base `72b4efbd`, owns only `Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs` and `Assets/ForestPrototype/UI/MenuHelpView.Text.cs` + own handoff.
- **Outcome:** RUNNING.
- **Next:** return → INDEPENDENT REVIEW from a Claude pool.

### EVENT 005 — 2026-11-04 00:2x UTC — dispatch #253 Option A MFM offline qualification → Claude A `claude-sonnet-5-5` medium

- **Evidence class:** OBSERVED FACT (delegation call) / ORCHESTRATOR CLAIM (fit reasoning).
- **Task:** #253, offline research/qualification, P0, docs-only, no Unity, no production.
- **Provider/model/effort:** pool `claudeAgent` ("Claude A"), `claude-sonnet-5-5`, `effort: medium`. The model's own default is `high`; it is being run at medium deliberately to hold the user's stated starting tier, with escalation reserved for evidenced reasoning failure.
- **Why:** highest-consequence conceptual task in the wave (unique ownership, reach, sample transfer, save/hash compat). Sonnet 5.5 is the packet-named primary; the independent review will come from a GPT pool.
- **Alternatives:** GPT-6.1 medium (kept free as its reviewer), Opus 5.5 (held in reserve; not justified before any demonstrated failure).
- **Pool status:** AVAILABLE. **Quota/reset:** UNKNOWN. **Capacity-to-completion:** marginal-to-sufficient — long source reading; checkpointed handoff required.
- **Ownership:** `docs/mfm-option-a-offline-20261104-v1`, `/media/jer/Files/CCF/Operations/CCF-MFM-OptionA-Docs-20261104`, base origin/main, read-only candidate `72b4efbd`; may append only to its own handoff and the access-spine programme ledger pointer section.
- **Outcome:** RUNNING.
- **Next:** return → independent GPT review; also append pointer event to `Docs/Handoffs/ScenarioOne-Access-Spine-20261103/ORCHESTRATION-LEDGER.md`.

### EVENT 006 — 2026-11-04 00:2x UTC — dispatch #257 input settings DRAFT spec → Claude B `claude-sonnet-5-5` medium

- **Evidence class:** OBSERVED FACT (delegation call) / ORCHESTRATOR CLAIM (fit reasoning).
- **Task:** #257, four-file DRAFT spec only, docs-only, no implementation, no telemetry.
- **Provider/model/effort:** pool `claudeAgent_claude_b_pro` ("Claude B — Pro"), `claude-sonnet-5-5`, `effort: medium`.
- **Why:** deliberately uses the *second* Claude subscription so Claude A's #253 run is not competing for the same account capacity; input/persistence/consent design suits Claude's long-form spec style. Review will come from a GPT pool for genuine cross-family independence.
- **Pool status:** AVAILABLE. **Quota/reset:** UNKNOWN (separate account, unverified). **Capacity-to-completion:** sufficient.
- **Ownership:** `docs/overnight-input-settings-spec-20261104`, `/media/jer/Files/CCF/Operations/CCF-Overnight-InputSpec-20261104`, base origin/main; owns `specs/player-input-settings-and-feedback-v1/` + own handoff only.
- **Outcome:** RUNNING.
- **Next:** return → independent GPT review.

### EVENT 007 — 2026-11-04 00:2x UTC — dispatch #259 mark language / Pro Silva DRAFT → ChatGPT-OpenAI B `gpt-6.1-sol` medium

- **Evidence class:** OBSERVED FACT (delegation call) / ORCHESTRATOR CLAIM (fit reasoning).
- **Task:** #259, primary-source forestry research + four-file DRAFT spec, docs-only, no colour/marker change.
- **Provider/model/effort:** pool `codex_c75d8114-…` ("ChatGPT - OpenAI B"), `gpt-6.1-sol`, `reasoningEffort: medium`.
- **Why:** uses the *second* Codex pool so #255's implementation and #259's research do not share one account's capacity; the task needs external-source discipline ("do not call a convention a standard") which suits a search-capable worker. Review from a Claude pool.
- **Alternatives:** Claude B (busy with #257), Opus 5.5 (not justified yet).
- **Pool status:** AVAILABLE. **Quota/reset:** UNKNOWN. **Capacity-to-completion:** marginal — external evidence plus a four-file spec; checkpoint required.
- **Ownership:** `docs/overnight-mark-language-spec-20261104`, `/media/jer/Files/CCF/Operations/CCF-Overnight-MarkSpec-20261104`, base origin/main; owns `specs/scenario-one-tree-mark-language-v1/` + own handoff only.
- **Outcome:** RUNNING.
- **Next:** return → independent Claude review.

### EVENT 008 — 2026-11-04 00:2x UTC — Unity admission decision: NOT ADMITTED for wave 1 (disk), all runtime claims marked unverified

- **Evidence class:** OBSERVED FACT (lock tool + `df` output, this session).
- **Observed lock state:** `unity-lock status` → `Unity lock: free`, `metadata: None`, `unity_editor_pids: []`. No Editor process machine-wide. UnityHub (user's own, flatpak) is running and is not a lease.
- **Observed disk floor:** `lanes.json` `storage.minimum_free_bytes` = `16106127360` (15 GiB); live ZX20 free = 19 GiB. The floor is met *before* any cold import, not after: a cold Library for a new lane is documented at roughly 10–12 GiB on this volume, which also holds the shared bare Git store and U1's warm Library. The only warm Library belongs to `CCF-U1`, which the coordination record holds for the user's own playtest.
- **Decision (ORCHESTRATOR CLAIM, not a user instruction):** DPC does **not** acquire a Unity lease for #255/#256/#260 in this wave. Consequences are stated explicitly, not hidden: every gameplay/rendered/input-path claim produced overnight is **UNVERIFIED at runtime** and must be labelled UNMEASURED by the worker. Offline static compile/contract checks are still required and are the actual gate evidence.
- **Reason this is the conservative reading:** the packet permits Unity "only if a clean existing verification lane fits the disk budget after preflight" and orders offline work when disk is marginal. 19 GiB against a 15 GiB floor plus a 10–12 GiB import is marginal by any reading, and disturbing U1 would touch a lane held for the user.
- **Escalation path if a runtime defect is later suspected:** a coordinated lease on a budget-checked lane with the user's knowledge, not an unattended import.
- **Outcome:** BLOCKED (Unity), work continues offline.
- **Next:** if absolute verification is required, Manager/user decision on lane and disk, recorded here.

### EVENT 009 — 2026-11-04 00:21 UTC — wave 1 dispatched and claimed on GitHub

- **Evidence class:** OBSERVED FACT (delegation responses + `gh issue comment` exit status) / ORCHESTRATOR CLAIM (any capacity inference).
- **Dispatches confirmed RUNNING (returned by the orchestrator, not inferred):** #255 `codex`/`gpt-6.1-sol` (taskId `…a5c747d6`); #253 `claudeAgent`/`claude-sonnet-5-5` (taskId `…aa374267`); #257 `claudeAgent_claude_b_pro`/`claude-sonnet-5-5` (taskId `…efe39d5f`); #259 `codex_c75d8114`/`gpt-6.1-sol` (taskId `…243f848e`). Four distinct subscription pools, no pool double-booked in wave 1.
- **Claims published:** issue comments posted successfully on #255, #253, #257, #259 with provider, model, effort, branch, worktree, base HEAD, ownership and the Unity deferral.
- **Not dispatched yet:** #256, #258, #260 — held so that no pool is double-booked while wave 1 runs; they start as wave 1 returns. This is a real constraint (four pools, six remaining lanes), not a scheduling preference.
- **Quota/tokens/context:** UNKNOWN for all four pools; no usage endpoint is exposed. Absence of a failure so far is not evidence of headroom.
- **Outcome:** RUNNING (4 children).
- **Next:** on each return — cross-provider review by a *different* family, then wave 2 fill.

### EVENT 010 — 2026-11-04 00:27 UTC — dispatch #256 rack-handle fix → Codex `gpt-6.1-sol` medium

- **Evidence class:** OBSERVED FACT (delegation call, source reads) / ORCHESTRATOR CLAIM (fit, capacity).
- **Task:** #256, PACKET-ONLY bounded interaction fix, P0.
- **Provider/model/effort:** `codex` / `gpt-6.1-sol` / `reasoningEffort: medium` (second concurrent child on this pool — deliberate, both are small bounded edits, but this is the point where per-account concurrency becomes an assumption to watch).
- **Why:** the change is a gesture-wiring correction in one file plus instruction text; the reviewer will be a Claude pool for cross-family independence.
- **Source findings given to the worker as "verify, do not trust":** the rotation gesture is defined in `Assets/ForestPrototype/UI/Map/StandMapRackEditor.cs` (gesture enum `Create/Move/Reshape`, `Propose(px, !ShiftHeld(e.modifiers), …)` in `OnPointerMove`), and the player-facing instruction is `Assets/ForestPrototype/UI/StandMapView.cs:601`. That helper file is the "strictly necessary rack editing UI helper" the packet allows; `RackEditing`/`RackPlan` policy is excluded.
- **Pool status:** AVAILABLE (now 2 children). **Quota/reset:** UNKNOWN. **Capacity-to-completion:** sufficient for the edit, unknown for the account's concurrency headroom.
- **Ownership:** `feature/overnight-rack-handle-20261104`, `/media/jer/Files/CCF/Operations/CCF-Overnight-RackHandle-20261104`, base `72b4efbd`.
- **Outcome:** RUNNING.
- **Next:** return → independent Claude review; human manual test script in handoff.

### EVENT 011 — 2026-11-04 00:30 UTC — dispatch #260 baseline verification → ChatGPT-OpenAI B `gpt-6.1-sol` medium

- **Evidence class:** OBSERVED FACT (delegation call) / ORCHESTRATOR CLAIM (fit).
- **Task:** #260, offline verification-only, P1, owns a new `Tools/Verification/OvernightPlaytestFeedback/` subtree.
- **Provider/model/effort:** `codex_c75d8114` / `gpt-6.1-sol` / `reasoningEffort: medium`.
- **Why:** deliberately started BEFORE the #255/#256 fixes land, so it records the true pre-fix baseline rather than a post-fix state that would make the user's reported issues look fixed. Independent of both fixes by construction (different worktree, different files, and instructed to report baseline as baseline).
- **Pool status:** AVAILABLE (2 children on this account). **Quota/reset:** UNKNOWN.
- **Capacity-to-completion:** sufficient — static checks plus a handoff.
- **Concurrency note:** six children now run across four accounts (2/2/1/1). No pool has more than two. If an account starts failing, this event is the point where the oversubscription assumption began, and the ledger will say so plainly rather than reclassifying quota as reasoning failure.
- **Ownership:** `verify/overnight-playtest-feedback-20261104`, `/media/jer/Files/CCF/Operations/CCF-Overnight-FeedbackVerify-20261104`, base `72b4efbd`.
- **Outcome:** RUNNING.
- **Next:** independent Claude review; its PASS/FAIL table must be cross-checked against the #255/#256 diffs at publication.

## Morning / terminal checkpoint

DPC must publish `OVERNIGHT-ORCHESTRATION-RESULT.md` before ending a session or at a context/quota transition. Record each child claim status and handoff link, what passed/failed, unmerged branches, orphaned locks, pending reviews, actual wait/restart state and first next safe executable work. If provider truly cannot continue until reset, publish a **BLOCKED/COOLDOWN** checkpoint with observed reset evidence (or UNKNOWN), rather than promising an unconfigured timed delivery.
