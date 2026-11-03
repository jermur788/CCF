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

### EVENT — 2026-11-03 ~19:50Z–19:57:51Z — DPC takes over the programme, measures live capacity, dispatches #246

- **Work unit / claim:** [CCF #246](https://github.com/jermur788/CCF/issues/246), read-only access-spine reconnaissance. Parent orchestration #201/#203; Overall Manager #200 unchanged.
- **Evidence class:** OBSERVED FACT (tool outputs, refs, timestamps), ORCHESTRATOR CLAIM (suitability/capacity judgement).
- **Prior provider/model/effort:** none for this programme. The ledger's pre-existing snapshot rows for both pools read `UNKNOWN`; no prior dispatch existed.
- **Selected provider/model/effort:** T3 provider instance `claudeAgent` ("Claude A"), model `claude-sonnet-5-5`, option `effort: medium`. Child task id `node:delegated-task:command:mcp:7dba9c20…:delegate-task:041faad8…`, role `research`, isolation `shared` (parent checkout, read-only briefing).
- **Alternatives, reason and exploit/explore:** Alternative was `codex_c75d8114-…` ("ChatGPT - OpenAI B") `gpt-6.1-terra` / `gpt-6.1-sol` at `reasoningEffort: medium`. **Chosen:** Sonnet 5.5 Medium. Reasons: (a) the work is long-form repository reading plus spec-to-code reconciliation, which is compact-context and citation-heavy; (b) cross-provider independence — #200/Overall Manager operates in the GPT-6 family, so a Claude-side recon keeps the first hand-off independent of the authority that will accept it; (c) the OpenAI pool is deliberately held back as the independent verifier and as the continuity pool if the Claude pool degrades. **Exploit/explore:** EXPLORE — first route in this programme; no measured cost/quality data for either pool on this task class.
- **Provider/pool state, quota/window and source:** OBSERVED at ~19:53Z from `orchestrator_capabilities` and the model catalogue: `claudeAgent` (Claude Sonnet 5.5 / Opus 5.5 / Fable 5.1 / Haiku 5.5) and `claudeAgent_claude_b_pro` ("Claude B — Pro") are enabled and `canRunChildTask: true`; `codex` and `ChatGPT - OpenAI B` enabled with `gpt-6.1-sol` / `gpt-6.1-terra` (+`gpt-6-astra`, `gpt-6-luna`) and `reasoningEffort` low→ultra; `opencode` (current DPC driver) enabled. Disabled: `cursor`, `grok`, `antigravity`, `fx`, `muse`, `pi`. **No quota, rate-limit window, reset time or remaining-credit telemetry is exposed by these tools — remaining capacity is UNKNOWN and is recorded as UNKNOWN, not estimated.** Catalogue list price observed for `opencode/claude-sonnet-5-5` ($2 in / $10 out per Mtok); that listing does **not** meter the `claudeAgent` subscription pool, whose quota stays UNKNOWN.
- **Context used/limit, compactions and source:** DPC parent context use and limit are not exposed by this harness — UNKNOWN. Child context use/limit and compaction count are UNKNOWN at dispatch and will be recorded at completion if the child reports them.
- **Capacity-to-completion, switching cost:** `sufficient by task size / unknown by quota`. The work unit is a single read-only Medium inspection (no Unity, no build, no writes); it should finish well inside a normal Medium context. Because quota is unmeasured, the safe assumption is that it could be interrupted; the checkpoint is the issue comment plus this ledger. Switching cost if the Claude pool fails: low-to-moderate — the brief is fully self-contained and can be re-dispatched verbatim to `gpt-6.1-terra` at `reasoningEffort: medium` with no state to salvage.
- **Branch/worktree, ownership, Unity status:** Inspection lane is read-only against `origin/integration/scenario-one-player-test-ready-v1` @ `72b4efbd1c653fa2231f632a79030a83ff76f833` (verified published candidate tip) and `origin/main` @ `d9b1268b` for specs/docs. No new product branch. DPC docs lane: sparse-checkout worktree `/media/jer/Files/CCF/Operations/CCF-AccessSpineDocs`, branch `docs/access-spine-recon-20261103-v1` off `origin/main`, checking out only `Docs/` and `specs/`. No production file ownership claimed. **Unity: not inspected, not launched, not claimed; #243's reported lease state is unchanged and untouched.** `active-tasks.md` not edited (#200 remains sole writer).
- **Checks/evidence, runtime, tokens where observable:** Nothing testable runs in this task. Evidence produced is source-cited reading only. Child runtime and token counts are not yet observable and will be appended at completion.
- **Outcome / reviewed / accepted / unresolved:** DISPATCHED at `2026-11-03T19:57:51Z`, child status `running`. #246 comment updated to [RUNNING]. Result not yet reviewed; nothing accepted.
- **Failure classification and recovery/next action:** None yet. Next: on child return, DPC spot-checks citation load-bearing claims against the refs, routes a narrow independent cross-provider verification to `gpt-6.1-terra` (Medium), then publishes `RECONNAISSANCE-HANDOFF.md` with the V2 Orchestration Decision Appendix and marks #246 [HANDED-OFF].

### EVENT — 2026-11-03 ~20:00Z–20:05Z — recon returned; DPC spot-check; cross-provider verification dispatched

- **Work unit / claim:** #246. Docs lane `docs/access-spine-recon-20261103-v1`; PR [#247](https://github.com/jermur788/CCF/pull/247) opened as draft (docs-only, sparse `Docs/`+`specs/` worktree).
- **Evidence class:** OBSERVED FACT (child status, citation checks), ORCHESTRATOR CLAIM (quality judgement), RETROSPECTIVE INFERENCE (effectiveness notes in Appendix A).
- **Prior provider/model/effort:** `claudeAgent` / `claude-sonnet-5-5` / `effort: medium`, task `node:delegated-task:…:delegate-task:041faad8…`.
- **Selected provider/model/effort:** verification route `codex_c75d8114-7118-4224-8bf2-b9b62637e1fb` ("ChatGPT - OpenAI B") / `gpt-6.1-terra` / `reasoningEffort: medium`, role `review`, read-only, task `node:delegated-task:…:delegate-task:1c988a8c…`.
- **Alternatives, reason and exploit/explore:** alternative was to accept the Claude report on DPC spot-check alone (cheaper). Rejected because the ledger must distinguish *independently reviewed* from *merely claimed*, and because the Manager will act on this hand-off. Verification was constrained to a fixed 22-claim list rather than the open-ended task, to keep the second provider cheap. **Exploit/explore:** EXPLOIT of provider independence (deliberate second provider, not a recovery).
- **Provider/pool state, quota/window and source:** both pools `AVAILABLE` (enabled, `canRunChildTask: true`) at dispatch. Quota/window/reset **UNKNOWN** — no telemetry exposed. No cooldown or login failure observed.
- **Context used/limit, compactions and source:** UNKNOWN for both children; not surfaced to this session. DPC parent context use not exposed either. No compaction observed in this session. Not estimated.
- **Capacity-to-completion, switching cost:** `sufficient by task size / unknown by quota`. Switching cost if the Codex pool had been unavailable: zero salvage needed — the claim list was already written and could have been checked by DPC directly.
- **Branch/worktree, ownership, Unity status:** unchanged from the dispatch event. No production file touched. Unity not launched or inspected.
- **Checks/evidence, runtime, tokens where observable:** DPC spot-check of the highest-risk citations against `origin/integration/scenario-one-player-test-ready-v1`: `RackPlanTargetCount = 5` (`ScenarioOneManager.RackPlan.cs:19`) PASS; `EntranceOffHeadland` (`RackEditing.cs:162-163`) PASS; `ForestSaveData.CurrentVersion = 24` (`ForestSaveData.cs:7`) PASS; five entrance ids (`ScenarioOneSuggestedRackLayout.cs:22`) PASS; `DefaultWidthM = 2.4` (`RackGeometry.cs:53`) PASS; `PathX = -2.436166` / `FeederZ = -27.4` (`ScenarioOneHeadlandGeometry.cs:10-11`) PASS; `BendLengthM = 7` / `EndZ = -48.5` (`StandApproachRoute.cs:18,20`) PASS; `moveSpeed = 4` / `runSpeed = 7` (`ForestPlayer.cs:10-11`) PASS; `TryValidateRackGeometry` body enforces only finite/length/inside-stand (no headland) PASS; `ForestSaveValidation.RackV2.cs:58,60` five-rack rules with no headland check PASS; Rack V2 spec §16 / §37 / §39.6.1 / §39.7 OPEN-2, OPEN-3 text PASS; `unity-project-overview.md:82` still says `CurrentVersion = 20` against a candidate at 24 PASS (documentation drift confirmed). **One citation error found by DPC:** `PlantationBranchMovement.ContactSpeedFactor` was reported "unused"; it is referenced at `PlantationBranchMovement.cs:126` and wired from `PlantationTreeVisual.cs:164-197`. Classification: **worker quality error, not a quota/cooldown failure.** Corrected in the hand-off before publication.
- **Outcome / reviewed / accepted / unresolved:** recon **REVIEWED (DPC)** with one correction; independent cross-provider verification **IN FLIGHT**. Hand-off drafted, not yet published. Nothing accepted.
- **Failure classification and recovery/next action:** partial citation inaccuracy (1 of ~22 spot-checked claims) — corrected by the orchestrator, not by silent rewrite of the worker's output; the correction is disclosed in hand-off §5 and here. Next: apply verifier findings, publish `RECONNAISSANCE-HANDOFF.md` + Appendix B, comment on #246, mark [HANDED-OFF].
