# CCF — DeepSeek orchestrator-only handoff template (APPROVED V1)

**Status:** APPROVED V1 — prospective orchestration policy by Overall Manager on 2026-11-05, following the user's direct direction. This is a reusable template, not a current-run dispatch or authorisation to start Unity/production.  
**Original draft prepared:** 2026-11-04; **approved:** 2026-11-05  
**Owner for approval and live coordination:** Overall Manager, browser/GitHub lane #200  
**Intended use:** Copy this template for each future local multi-agent orchestration run. Replace every {{PLACEHOLDER}} before dispatch. Do not send an unfilled template as execution authority.

## 1. Purpose and requested policy change

DeepSeek is the **dedicated orchestrator at High reasoning/effort**, not an implementation worker. Its value is global coordination, dependency sequencing, model/pool routing, capacity monitoring, checkpoint/salvage decisions, continual evidence-ledger updates and preparation of decision-ready returns for the Overall Manager.

**DeepSeek must not implement, patch, debug or review product code; operate Unity; run verification; edit tests; integrate; or make product/design acceptance decisions.** It may read repository/tool state, maintain its own orchestration records, create/update child task execution claims within authorised scope, and submit routing recommendations or packets for Manager action. No simultaneous DeepSeek worker/reviewer role, even for easy tasks.

Default execution workers:
- **OpenAI GPT-6.1 Sol, Medium** — default capable implementation, tooling, debugging and runtime verification where its environment has the required tools.
- **Claude Sonnet 5.5, Medium** — equally eligible default for implementation, investigation and independent review, selected by task fit, observed capacity and review independence rather than fixed discipline.
- **Claude Opus 5.5, Medium** — **exceptional escalation only**, after documented capability/reasoning need or explicit preauthorisation for unusual risk. Require the relevant Manager G1 cost/launch authorisation before an exceptional costly run. Never automatically escalate due to a real failing test, lock, provider quota, authentication, stale state, or missing requirements.

The user-requested defaults above are **approved prospective replacements** for the older DeepSeek-implements-easy-work and Opus-Low-first instructions in historical #201/#203 packets. Historical outcomes stay historical. **Apply this to new dispatches only after a current Manager handoff reconciles the relevant claims/ownership; never retroactively reclassify an already-running worker.** Do not turn this proposal into a gameplay decision.

## 2. Standard orchestration intake — fill before dispatch

### Identity

| Field | Required value |
|---|---|
| Programme / run | {{RUN_NAME}} |
| Parent task claim | {{PARENT_CLAIM_URL}} |
| Manager / escalation contact | {{OVERALL_MANAGER_CLAIM_OR_CONTACT}} |
| Run status | READY / RUNNING / BLOCKED / HANDED-OFF |
| Prepared at (UTC) | {{YYYY-MM-DD HH:MM UTC}} |
| Git repository | https://github.com/jermur788/CCF |
| Context snapshot | {{EXACT_CONTEXT_COMMIT_OR_STAMP}} |
| Canonical implementation base | {{FULL_CANDIDATE_COMMIT}} |
| Branch / evidence lane | {{ORCHESTRATION_BRANCH_AND_REGISTERED_WORKTREE}} |
| Live coordination read | {{URL_AND_TIMESTAMP_OF_LIVE_ACTIVE_TASKS_READ}} |
| Dated task queue | {{LINK_TO_CHILD_TASK_CLAIMS_AND_DEPENDENCIES}} |
| Ledger path | {{GIT_TRACKED_ORCHESTRATION_LEDGER_PATH}} |
| Result/handoff path | {{GIT_TRACKED_ORCHESTRATION_RESULT_OR_HANDOFF_PATH}} |
| Source authority | Latest user instruction → Decision Log → Current Milestone → Game Brief; approved specs only for their tasks |
| Implementation truth | Inspected repository and actually verified Unity state; not worker or chat assertion |

### Mission

**Player/system outcome:** {{ONE_PARAGRAPH_OUTCOME}}

**Why now / dependencies:** {{DEPENDENCY_ORDER_AND_CRITICAL_PATH}}

**Specific work released by Manager:** {{TASK_IDS_AND_AUTHORISATION_URLS}}

**Excluded or pending decisions:** {{UNAPPROVED_SPECS, DESIGN QUESTIONS, NO-ACTION ITEMS}}

**Objective and optimization priorities:** Quality/correctness > protected compatibility/one-writer safety > efficient cost, tokens and wall time. Adjust only to the actual user instruction. Capture user priority choices for quality, money, tokens, wall time, compute/resource use and exploration tolerance. Unknown preferences must remain UNKNOWN, not invented.

### Protected boundaries

- **Do not alter the published personal-player-test candidate, user saves, Legacy40 or frozen Reference Future v1** except by a later explicit, scoped Manager release satisfying all prerequisites.
- **No Windows or Player build** before the user personally playtests the Scenario One candidate and explicitly authorises the Windows build afterward.
- Preserve all unrelated dirty files. The coordination/context anchor is **/media/jer/Files/CCF-Primary** on the Files volume, not a Unity worktree. Do not stash, clean, reset, commit, overwrite or absorb its existing user-owned edits. Local access must be verified by the responsible local worker; a remote Manager cannot claim local verification.
- The machine-wide Unity lock is exclusive. A historical FREE receipt is not current lock availability. Before any Unity work a dedicated local worker must check registered lane, branch/HEAD, clean status, Editor processes, free disk and the deployed lock, then acquire its **own** lease.
- Browser Overall Manager #200 remains the **sole writer** of Docs/Project/coordination/active-tasks.md under the current split. DeepSeek reads it, sends ownership/lock change requests, and never writes it or generated dashboard-state files.
- Research is evidence, not product authority; DRAFT/SUPERSEDED specs never authorise implementation. Do not weaken failing tests or mislabel evidence as acceptance.
- Every substantive child needs its own GitHub execution claim and durable Markdown handoff; claims are execution presence, **not** ownership or approvals.
- No worker may self-merge or mark its own result ACCEPTED merely because tests passed.

## 3. Roles, routing and escalation

| Role | Provider / effort | Permitted activity | Prohibited activity |
|---|---|---|---|
| Parent orchestrator | **DeepSeek, High** (record actual model/provider and supported reasoning setting) | Read state; schedule; choose among authorised worker pools; track capacity/context; update append-only routing ledger; create/update execution claims and publish orchestration handoff; checkpoint/stop/escalate | Any implementation, code/test patch, Unity, verification run, substantive independent source review, merge/integration, approval, edit of live ownership |
| Default worker A | **GPT-6.1 Sol, Medium** | Implement or verify only its bounded authorised claim in an owned lane; produce handoff | Outside scope, bypass lock, overwrite others, approve own output |
| Default worker B | **Claude Sonnet 5.5, Medium** | Same, including independent read-only review of OpenAI implementation | Same; no same-thread self-review |
| Exceptional worker | **Claude Opus 5.5, Medium** | Manager-authorised difficult or high-consequence escalation; bounded scope and evidence | Routine jobs solely because available; unauthorised budget escalation |

**Routing rule:** Check available pools and task capability before selection. Choose Sol or Sonnet at Medium for best fit and sufficient capacity-to-completion, considering current context, observed previous outcomes, reviewer independence, task difficulty and switching costs. Use cross-provider independent review where the task's review gate requires it (Sol work → Sonnet review; Sonnet/Opus work → Sol review, subject to actual independence). Two workers must never write the same file set concurrently.

**Escalation to Opus Medium:** Document a specific failure of reasoning/capability by a default worker **after ruling out** environment/lock, quota/auth, scope/authority, real defect, incomplete input and verification-harness causes; OR document an unusually high-risk task for which the Manager explicitly preauthorises Opus. State why another default Medium worker or narrower decomposition is insufficient. Check pool availability and request G1 approval where required. If not approved, mark BLOCKED/return to Manager; do not quietly invoke Opus.

**Failure classes to record:** CAPACITY/QUOTA; AUTH/LOGIN; ENVIRONMENT/LOCK/DISK; SCOPE/AUTHORITY; SPEC/DESIGN AMBIGUITY; GENUINE PRODUCT DEFECT; TEST/HARNESS/EVIDENCE; MODEL CAPABILITY/REASONING; TOOL/TRANSPORT; UNKNOWN. A test revealing a real defect is not automatically a failed model. Do not bypass account limits or infer a quota reset merely from elapsed time.

## 4. Initial preflight and ownership boundary

Before dispatching even one child:

1. Read latest direct user instruction; live CCF active-tasks.md on GitHub; relevant #200/#203 or superseding claims; Decision Log, Current Milestone, exact approved spec/plan/task and existing handoff. Obtain exact candidate base/context and check for changes.
2. Read the actual local coordination anchor where locally available, **read-only**; preserve uncommitted user path fixes. Never represent GitHub coordination as proof of current local Unity lease or cleanliness.
3. Inspect actual provider/model/effort availability and quota/context telemetry **if exposed**. Record UNKNOWN for unavailable telemetry; a model catalogue is not proof of remaining subscription capacity.
4. Build a dependency DAG with task ownership, exact branches/worktrees and allowed files. Flag task class **PACKET ONLY** or **SPEC REQUIRED**, mandatory review gates G1–G5 and conflicts. Do not authorise work from DRAFT specs.
5. Ask Overall Manager #200 to register/change ownership where needed. The orchestrator may not bypass an unresolved lock or take the Manager's single-writer lane.
6. For each authorised child, publish the GitHub task packet and claim; post the actual [RUNNING] receipt **only after** its worker passes the specific start preflight. Include actual provider/model/effort, lane, branch, starting HEAD and Unity lease only if legitimately acquired.
7. Begin an append-only ledger **before** the first dispatch. Checkpoint after material events and before provider-context/quota limits. A handoff missing its GitHub copy is incomplete.

## 5. Standard child dispatch block — one per task

**Child task:** {{TASK_ID / ISSUE_URL}}  
**Parent:** {{PARENT_CLAIM}}  
**Purpose:** {{ONE RESULT TO DELIVER}}  
**Difficulty:** Low / Medium / High — {{WHY}}  
**Classification:** PACKET ONLY / SPEC REQUIRED  
**Approved spec/task:** {{SPEC_STATUS_AND_LINK_OR_NA}}  
**Model/effort:** GPT-6.1 Sol Medium / Sonnet 5.5 Medium / Opus 5.5 Medium by exceptional approval  
**Routing reason:** {{FIT, LIVE CAPACITY, ALTERNATIVES, INDEPENDENCE, COST, PRIOR OUTCOMES}}  
**Task packet (direct URL):** {{GITHUB_URL}}  
**Starting candidate:** {{EXACT_FULL_COMMIT_AND_BRANCH}}  
**Writable lane/worktree:** {{REGISTERED_PATH_AND_BRANCH}}  
**Allowed files:** {{LIST}}  
**Forbidden files/systems:** {{LIST}}  
**Ownership and lock:** {{LIVE_AUTHORITY_LINK / UNITY_REQUIRED, LEASE_OWNER / NONE}}  
**Review gates:** {{G1_G2_G3_G4_G5_AND_NEEDED_REVIEWER}}  
**Verification tier:** Tier 1 / 2 / 3; {{EXACT_GATES, EXPECTED_OUTPUT, LIMITS}}  
**Stop conditions:** {{LOCK, DIRTY_WORK, DIVERGED_HEAD, PRODUCT_CHANGE, UNAPPROVED_COST, FAILURES}}  
**Return artifact:** {{EXACT_GITHUB_MARKDOWN_HANDOFF_PATH}}  
**Success token:** {{TASK_SPECIFIC_RESULT}}  
**User action:** {{NONE_OR_EXACT_ACTION}}

**Copy/paste dispatch line:**

    Execute CCF {{TASK_ID}} as {{IMPLEMENTER_OR_REVIEWER}} using {{ACTUAL_SELECTED_MODEL_AND_EFFORT}}. Read {{DIRECT_TASK_PACKET_URL}}. Start only from {{EXACT_BASE}} on {{REGISTERED_BRANCH_AND_WORKTREE}} after refreshing live ownership and completing the required clean-lane/Unity preflight. Stay within {{FILE_SET_AND_SCOPE}}, perform {{GATES}}, stop on {{STOP_CONDITIONS}}, publish {{DIRECT_GITHUB_HANDOFF_EXPECTED_URL_OR_PATH}} and return the claim link. No self-merge, no published player-test candidate change, no Windows build.

## 6. Provider capacity and context register — update throughout run

| Provider/pool | Actual model/effort | State | Remaining estimate | Reset/window | Source/confidence | Context used/limit, compactions | Current task | Capacity-to-completion | Switching/interruption cost |
|---|---|---|---|---|---|---|---|---|---|
| DeepSeek orchestrator | {{MODEL}} / High | UNKNOWN | UNKNOWN | UNKNOWN | {{SOURCE}} | UNKNOWN | coordination/ledger only | UNKNOWN | {{VALUE}} |
| OpenAI worker pool(s) | GPT-6.1 Sol / Medium | UNKNOWN | UNKNOWN | UNKNOWN | {{SOURCE}} | UNKNOWN | {{TASK_OR_NONE}} | UNKNOWN | {{VALUE}} |
| Claude worker pool(s) | Sonnet 5.5 / Medium | UNKNOWN | UNKNOWN | UNKNOWN | {{SOURCE}} | UNKNOWN | {{TASK_OR_NONE}} | UNKNOWN | {{VALUE}} |
| Claude escalation pool(s) | Opus 5.5 / Medium | UNKNOWN | UNKNOWN | UNKNOWN | {{SOURCE}} | UNKNOWN | manager-approved only | UNKNOWN | {{VALUE}} |

Allowed states: **AVAILABLE / BUSY / COOLDOWN / LOGIN-UNAVAILABLE / UNKNOWN**. Capacity-to-completion: **sufficient / marginal / insufficient / unknown**. Capture actual provider/pool identity rather than treating multiple subscriptions as one. Preserve reset/window as UNKNOWN unless evidenced. Record context limit, used/estimated, source/confidence, compactions and material pressure only where observable. Do not infer measured energy from token counts or pricing proxies.

## 7. Append-only V2 routing ledger event — fill at the time, not retrospectively

### EVENT {{SEQUENCE}} — {{UTC_TIMESTAMP}} — {{EVENT_TYPE}}

- **Evidence class:** OBSERVED FACT / ORCHESTRATOR CLAIM / RETROSPECTIVE INFERENCE (split bullets if different).
- **User objective / hard constraints / optimization priorities:** {{REFERENCE_OR_CHANGE}}.
- **Work unit / parent / task characteristics / risk:** {{CLAIM, DIFFICULTY, RISK}}.
- **Decomposition topology / predecessor dependency / ownership:** {{DAG_AND_LANES}}.
- **Previous provider/model/effort/role:** {{VALUE_OR_NONE}}.
- **Selected provider/model/effort/role:** {{EXACT_RUNTIME_ID_AND_POOL}}.
- **Alternatives considered and why rejected:** {{VALUES_OR_UNKNOWN}}.
- **Routing rule/reason, exploit vs explore, policy version, prior outcome:** {{VALUES}}.
- **Pool availability, quota/remaining/window/reset, source and confidence:** {{OBSERVED_VALUES_OR_UNKNOWN}}.
- **Context used/limit/compactions/pressure, source and confidence:** {{VALUES_OR_UNKNOWN}}.
- **Capacity-to-completion and expected interruption/switching cost:** {{VALUES_OR_UNKNOWN}}.
- **Actual branch, worktree, start HEAD, exclusive owner, Unity/process/disk preflight:** {{EVIDENCE_URLS_OR_NA}}.
- **Operation/outcome/verification/review:** {{RUNNING, PASS, FAIL, HANDED-OFF, REVIEWED, APPROVED, BLOCKED, ETC.; DO NOT CONFLATE}}.
- **Handoff/PR/claim links and exact candidate:** {{URLS}}.
- **Actual resources when observable:** input/output tokens, calls, wall time, retries, parallel workers, provider failures and compute/resource estimate **with provenance and confidence**, otherwise UNKNOWN.
- **Failure class / salvage / retry / cost / lost work:** {{VALUE_OR_NONE}}.
- **Manager gate disposition / unresolved items / next action:** {{VALUE}}.

**Minimum event triggers:** task decomposition; provider/model selection; actual dispatch; ownership/Unity acquisition or denial; context/quota degradation; switch/retry/salvage; child return; verification/review; Manager acceptance/rejection; stop/block; material resource use; final continuation/handoff. Do not retroactively manufacture missing measurements. Preserve historical corrections as new append entries, not silent rewrites.

## 8. Decision, continuation and escalation protocol

On every child return:
1. Inspect the durable GitHub handoff, actual repo diff/branch ancestry and reported verification receipts. Identify evidence directly checked by the orchestrator versus worker-reported evidence.
2. Classify IMPLEMENTED, HANDED-OFF, REVIEWED, INTEGRATED, VERIFIED and ACCEPTED separately. A worker's PASS is not Manager acceptance.
3. Check independent review requirements, exact candidate and preserved protected invariants. Route a read-only review to the *other provider family* if required; reviewers receive bounded claims and write no implementation files.
4. Send findings and explicit **APPROVED / NEEDS CHANGES / REJECTED recommendation** to Overall Manager; **only Manager** issues the binding verdict, integration release, product scope decisions and coordination edits.
5. Continue to another already-authorised, nonoverlapping task when safe. Never treat "keep working" as permission to implement a DRAFT spec or race an active Unity/user playtest.
6. If no safe authorised task remains, report BLOCKED with exactly what decision/lock/capacity is missing. Do not self-authorise a new feature or build.

### Manager-only review gates

- **G1 COST/LAUNCH:** expensive Unity Tier 2/3, exceptional Opus Medium or costly rerun.
- **G2 ACCEPTANCE:** before acceptance of evidence/handoff criteria.
- **G3 MEASUREMENT:** test changes, expected hashes, fixture/threshold changes, calibration or proposed waiver.
- **G4 INTEGRATION/CANON:** moving shared candidates or editing canonical workflow/instructions.
- **G5 SCOPE/MILESTONE:** changed player workflow, product decision, save/history policy or scope.

## 9. Return handoff — same format every run

**Handoff path:** {{DURABLE_GITHUB_MARKDOWN_PATH}}  
**Result status:** HANDED-OFF / BLOCKED / NEEDS CHANGES — never self-label ACCEPTED  
**Purpose completed:** {{SCOPE}}  
**Task table:** {{TASK_ID, WORKER_POOL, MODEL/EFFORT, EXACT_BRANCH/HEAD, RESULT, TEST EVIDENCE, REVIEW, CLAIM}}  
**What changed:** {{ONLY_SUPPORTED_FACTS}}  
**What passed/failed:** {{EXACT_RESULTS_AND_GATE_SCOPE}}  
**Protected contracts:** {{UNCHANGED / CHANGED_WITH_MANAGER_AUTHORITY / UNVERIFIED}}  
**Open gaps:** {{BLOCKERS_AND_NONBLOCKERS}}  
**Provider events:** {{CAPACITY, SWITCHES, RETRIES, SALVAGE, DEVIATIONS}}  
**Resource observations:** {{KNOWN_MEASUREMENTS_AND_UNKNOWN_TELEMETRY}}  
**Learning assessment:** {{WHAT_WORKED, WASTE, CHEAPER_SUFFICIENT_ROUTES, ESCALATION, CONTEXT_PRESSURE, COORDINATION_OVERHEAD, HYPOTHESES_AND_CONFIDENCE}}  
**Final summary:** {{ATTEMPTS, COMPLETED, FAILED, CANCELLED, CAPACITY_FAILURES, ROUTING_DECISIONS, DIRECT_VS_INFERRED_CAPACITY, RESOURCE_OBSERVATIONS, REVIEWS, UNRESOLVED, NEXT_ACTION}}  
**Dashboard:** {{MIRROR_UPDATED_AND_READ_BACK / INGESTION_AUDIT_VERIFIED / UNKNOWN / DEFERRED_TO_MANAGER}}  
**Manager decision requested:** {{APPROVE / NEEDS CHANGES / REJECT_AND_EXACT_REASON}}  
**Next packet when authorised:** {{DURABLE_PACKET_URL_OR_NONE}}  
**User action required:** {{EXACT_ACTION_OR_NONE}}

Every substantive implementation, qualification, review, integration and research result needs a GitHub Markdown handoff. Large raw logs and captures may stay in verification storage but must be linked and described. Use the **Mandatory Orchestration Evidence Appendix V2** in the final result. Record observed facts separately from contemporaneous orchestrator claims and retrospective hypotheses. Include a ledger continuity checkpoint allowing a future orchestrator to resume without reconstructing unknown values.

## 10. Historical approval checklist (disposition in Manager review)

- [x] Accept **DeepSeek High orchestrator-only**, with **zero** implementation, test execution or substantive independent review.
- [x] Accept **GPT-6.1 Sol Medium** and **Sonnet 5.5 Medium** as coequal default worker pools, selected by evidence/fit and reviewer independence.
- [x] Accept **Opus 5.5 Medium only for justified, Manager-authorised escalation**, rather than the previous Low-first historical rule.
- [x] Check reconciliation with active #91/#201/#203 instructions and any current working child packets; new policy applies prospectively only after approval, not retroactively.
- [x] Confirm child claim lifecycle and single-writer active-tasks.md authority remain with browser Manager #200; DeepSeek writes orchestration ledger, not live coordination.
- [x] Verify that the template faithfully captures every V2 evidence dimension without inventing unavailable provider quota/context/energy information.
- [x] Confirm no new Unity, integration, personal-playtest or Windows-build authority is implied.
- [x] Choose approved canonical template location and whether/how to update AgentWorkflow/role instructions; if canonical context changes, regenerate stamped mirrors at one clean Git snapshot before describing them as current.
- [x] Publish explicit APPROVED / NEEDS CHANGES / REJECTED verdict, then issue a versioned approved template and future dispatch packet.

**Approval completed via the Manager review linked below. The checklist above is preserved as the historical draft review rubric, not an outstanding approval gate. The current run must use its own dated exact-state handoff.**


**Manager review:** https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/Orchestration-Template-20261104/MANAGER-REVIEW.md
**Current prospective run handoff:** https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/Orchestration-20261105/DEEPSEEK-ORCHESTRATOR-ONLY-RESUME-PACKET.md
