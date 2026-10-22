# CCF Multi-Agent Development Workflow

## Purpose

Single owner of source authority/conflicts, context-commit meaning, worktrees/branches, roles/locks, task packets/handoffs, review/integration states, cloud-to-local verification and Drive/project-context synchronization.

Root AGENTS.md owns repository/Unity/C# technical safety. Do not duplicate it except for specifically multi-agent coordination rules.

No model family permanently owns code, Unity, Blender, art, testing, tooling or research. Assign by capability, tools, context, availability and demonstrated quality.

## Authority

The user is product owner/final decision-maker. Overall Manager is currently ChatGPT Game Dev; the user may reassign it. Claude/OpenAI may be primary workers, reviewers or integrators in any supported discipline.

<!-- PROJECT_AUTHORITY_BLOCK_START -->
### Decision authority

For what the project intends or has accepted:

1. user's latest direct instruction;
2. `Docs/Project/decision-log.md`;
3. `Docs/Project/current-milestone.md`;
4. `Docs/Project/game-brief.md`.

### Implementation authority

For what currently exists:

1. inspected repository / verified Unity state;
2. `Docs/Project/unity-project-overview.md`.

Planning text does not override verified implementation facts. Research and agent output are supporting evidence/proposals, not accepted decisions unless the user accepts them.
<!-- PROJECT_AUTHORITY_BLOCK_END -->

Report conflicts and identify the authoritative file needing update.

CCF-Ops role files, board state, lane/capacity configuration and operational scripts are coordination mechanisms only. They confer no product decision authority, implementation truth or acceptance authority. The project authority hierarchy above remains unchanged.

## Canonical context

Shared sources: `Docs/Project/game-brief.md`, `decision-log.md`, `current-milestone.md`, `unity-project-overview.md`, `AgentWorkflow.md`; supporting `research-index.md`.

Instruction sources: `Docs/Project/instructions/shared-project-instructions.md`, `chatgpt-role.md`, `claude-role.md`. Repository entry points: root AGENTS.md and CLAUDE.md. Generator: `Tools/ProjectContext/generate_project_instructions.py`.

Generated composites are not hand-maintained canonical files. The generator combines the role header, authority between the markers above, shared core, and a generated context/source header. Write generated output outside canonical paths, normally ignored `Build/ProjectContext/`.

## Context commit

One **full Git SHA** snapshots the entire canonical context/instruction/entry-point/generator set. It is the most recent commit changing any member of that set. Read all sources together from that one commit, even if only one changed there.

`Docs/Project/coordination/active-tasks.md` is excluded and read live. Coordination-only commits do not advance context.

Clean generation refuses dirty/untracked context sources and reads committed text with `git show`. `--preview-dirty` explicitly labels working-tree content **DIRTY PREVIEW — NOT A CONTEXT COMMIT**; it must not present that content as a Git snapshot.

All permanent mirrors/composites in a refresh use the same full context SHA. A task locks to its packet's context commit, not a moving live source. Check stamps at substantial task start.

If live-linked context changes mid-task and the locked version cannot be retrieved, stop and notify Overall Manager. Static uploads may continue when their stamp matches the task packet.

## Git and Drive

Git is canonical. Permanent Drive context is a read-only one-way collaboration mirror, not an alternate editing surface. Final stamped mirrors begin:

```text
MIRROR — DO NOT EDIT

Source: <repo path>
Context: <full SHA>
Snapshot timestamp: <context commit timestamp with timezone>
```

The timestamp is the context commit's timestamp, not regeneration wall-clock time, so repeated generation is byte-identical. The manifest records source, composite and mirror SHA-256 hashes.

If Git/Drive differ, Git wins; regenerate. Attached/project copies do not automatically follow Drive edits. After context changes: regenerate the entire mirror set and both composites at one SHA, then refresh/re-attach/replace platform knowledge. Do not call older attachments current.

### Dashboard synchronization

After any substantial task, integration, review, user-test result or project-status change:

1. update the authoritative local project/coordination record;
2. allow/run the dashboard ingestion/update mechanism;
3. confirm the automatic GitHub dashboard audit snapshot advances;
4. confirm the published dashboard state actually reflects the new project status.

A substantial task is not fully closed until this synchronization is verified.

If the dashboard or publisher is unavailable, the task may hand off, but the synchronization failure must be explicitly recorded as an outstanding operational issue.

Never manually edit generated dashboard-state files.

## Capability

Use only currently available tools. Chats without repo/Unity tools may plan/research/review, not claim local implementation. Claude Desktop MCP can perform authorised local work; Code/cloud can implement but claim Unity verification only after actual runs. OpenAI/OpenCode can use available code/Unity/Blender/asset/integration tools.

## Cost-aware task routing

Use the least costly capable route for the task, considering actual tools, context, availability and verification needs. Routing labels are stable capability handles:

- **FREE HERE:** bounded planning, clarification and coordination.
- **DEEP RESEARCH:** substantial evidence collection and synthesis; distinguish research from accepted mechanics.
- **SOL/CODEX:** local/repository implementation, tooling, documentation and verification.
- **REGULAR CLAUDE VALUE:** independent reasoning, review and gate work.
- **CLAUDE CLOUD VALUE:** substantial cloud/Git-based implementation.

Reserve highest-capability models for work where additional capability materially justifies the cost. Verify actual tools and availability before dispatch. Machine/provider/model defaults belong in operational routing configuration; subscription/account details are not canonical project policy. Worker-cost policy does not belong in the gameplay Decision Log.

Where cost/capacity matters, record `harness / model / quota source` in the packet, operational board and ledger/handoff as relevant. Overall Manager explicitly assigns finite primary/reserve pools. Never silently switch quota source; never store credentials or account secrets in tracked files. Capacity exhaustion never waives, downgrades or satisfies a verification, review or acceptance gate: defer work or explicitly reroute it.

## Verification cost policy

Use the cheapest verification tier that can answer the current risk. Do not
repeatedly run full Unity qualification on intermediate implementation or
calibration branches.

### Tier 1 — FAST

Target: **5–15 minutes**.

Use during active implementation and calibration. Prefer compile/import where
needed, feature-specific smoke, relevant offline/static tests and one
representative deterministic runtime fixture. Stop when the immediate
implementation question is answered.

### Tier 2 — FEATURE QUALIFICATION

Target: **20–60 minutes**.

Use when a bounded feature is ready to hand off. Run the specific Unity
behaviour the feature changes, important negative cases, save/load only when
persistence is relevant, and a narrow regression set covering plausible
collateral damage.

### Tier 3 — FULL INTEGRATED QUALIFICATION

Several hours can be acceptable, but reserve this tier for integrated milestone
or release candidates. This is where exhaustive regression, all required
reference anchors, multiple long-run simulations, broad save/load coverage,
rendered UX matrices, Player profiling, multiple resolutions and platform
qualification belong.

Do not rerun Tier 3 merely because documentation, verification tooling,
calibration evidence or an unrelated subsystem changed.

Repeated long simulations, multiple seeds and multiple stand geometries require
a stated uncertainty they are intended to resolve. Stop once that question has
sufficient evidence; do not expand testing into open-ended certification.

During feature iteration, prefer offline/reduced-model testing for parameter
search and use Unity to confirm representative runtime equivalence. Do not use
Unity as the parameter-search loop.

After a feature passes its bounded qualification, defer broad regression to the
next integrated candidate unless the feature directly threatens a frozen
reference, save compatibility, deterministic behaviour or another high-risk
shared system.

Escalation to a higher tier requires at least one of:

- a failed gate;
- a concrete newly discovered risk;
- a direct threat to a protected reference/save/determinism contract; or
- explicit Manager approval.

## Multi-agent safety

1. One writable lane per active owner; one active owner per writable lane.
2. No concurrent conflicting writes, and no two writable lanes on the same branch, including across separate Git stores. Threads/sessions are disposable; lanes persist. Occupancy implies no acceptance, memory or authority.
3. Substantial tasks declare exact base, context commit and branch.
4. One primary writer owns a subsystem/file set.
5. Enforce high-conflict/asset safety from AGENTS through locks.
6. Do not stash/discard/overwrite/clean/silently absorb unrelated user work.
7. Major design, ecology, save architecture, dependencies, frozen-reference or destructive decisions return to Manager/user.

Use designated project storage and CCF-Ops lane configuration for persistent Unity, integration and non-Unity lanes. Preserve existing work rather than renaming or recreating lanes automatically.

New substantial CCF development data—repositories/worktrees, Unity Library/cache data, builds, verification receipts and scratch/temp data—must use the designated external project storage volume. Verify backing-device identity, not merely path text. Moving existing data requires separate authorisation. Device identity, mount paths, filesystem details and capacity thresholds remain machine-local operational configuration.

## Unity resource policy

Serialize heavy Unity execution machine-wide under one ownership lock across lanes and Git stores. A Unity packet states Unity required, exact candidate HEAD, Unity lane/owner, allowed phases and Player/native-input requirement. Verify the clean lane's actual branch/HEAD and ownership before acquiring the lock; never bypass another owner or kill its process through the lock.

The Integration lane constructs/integrates code and normally does not host the Editor. Post-integration Unity gates run in a designated Unity-capable lane (U1/U2 or a configured equivalent), placed cleanly at the exact integrated HEAD under the Unity lock. Do not duplicate the active integration branch into another writable lane; detached candidate validation or a distinct owned lane branch preserves branch ownership.

Follow the verification-cost policy above: offline/reduced search first, Unity confirms candidate behavior, and stop when the stated uncertainty is answered. Availability of an Editor does not expand scope.

## Pre-write checks

Identify exact worktree, branch/HEAD, status and unrelated changes; apply AGENTS technical checks; confirm unsaved serialized Unity edits if relevant, ownership/locks and context commit. Stop if those cannot be established safely.

## Roles

- **Primary:** implements bounded authorised scope; reports readiness for review, not its own integration approval.
- **Reviewer:** bounded assignment against accepted specification/context/evidence and the review gates below; independent of the implementing thread for the reviewed change, cross-family when the packet requires it; no competing implementation unless requested. A provider-native helper or child of the implementing worker does not count as an independent reviewer for that parent task; authorisation does not change this.
- **Integrator:** deliberately integrates approved work and runs post-integration gates; only one active integrator per integration worktree.
- **Self-integration:** only with explicit Manager permission for small reversible low-risk tasks. Not default for save migrations, determinism/reference, large serialized changes, major refactors or dependencies.

- **Execution Manager (T3):** within an assigned packet and accepted roadmap, inspect repository/live state; preflight packets; maintain assigned operational state/ledger; inspect currently available provider capabilities; route bounded work through T3-owned orchestration; launch/delegate bounded workers; monitor delegated tasks; collect handoffs/results; compare child findings; inspect task/thread identities; resolve simple factual disagreements through direct read-only inspection; and automatically continue the next already-authorised operational step when no gate, conflict or user decision blocks it. It cannot make product/design decisions, alter accepted scope, waive G1–G5, grant itself ownership, bypass active-task ownership or the Unity lock, weaken verification, integrate unapproved work, treat worker completion as acceptance, or silently change quota/provider source.
- **Unity Operator:** own serialized Unity execution, verify exact lane/branch/HEAD, hold the Unity lock, run only authorised gates and preserve evidence. It cannot expand scope.

## Orchestration and delegation

Capability to launch or delegate work is an orchestration interface, not an authority source. Authority still comes from accepted scope, the task packet, ownership/locks, the project authority hierarchy and G1–G5. Possession of a delegation command grants no permission to use it.

Current orchestration interfaces may include `delegate_task` for disposable child investigations/reviews and `t3_thread_launch` for persistent workers attached to known CCF lanes. These names describe current tooling, not permanent policy; use the currently supported mechanism. Concrete provider/model names and quota assignments belong in operational configuration and task records, not in this canonical workflow.

### Provider-native subagents

Workers may use provider-native subagents/background tasks for bounded side investigations when useful for repository reconnaissance, call-site tracing, save/data-flow inspection, test inspection, failure diagnosis, API/document inspection, or read-only comparison of implementation options. Do not require subagents for trivial work.

By default they are investigation helpers. They do not gain independent project authority, lane ownership, Unity ownership, integration authority or review authority. A provider-native helper/subagent must never concurrently write the same Unity worktree as its parent or another writer, and a helper working beneath an implementation parent must never satisfy that parent task's formal independent-review gate. Unless specifically authorised, it must not launch competing Unity processes, modify shared serialized assets, push/integrate, or change canonical documents. The parent worker owns the final handoff and must summarise any material subagent findings it relied on.

### T3-owned delegated work

T3 delegation is appropriate when cross-provider/model work is useful, formal independent review is required, separately tracked child work is useful, isolated writable work is required, or filesystem/worktree isolation matters.

For independent writable work, prefer an isolated worktree. For persistent workers attached to known CCF lanes, use the currently supported persistent-thread launch mechanism. For disposable child investigations or reviews, use the currently supported delegated-task mechanism.

Every delegated child must receive explicit base, context commit, scope, exclusions, ownership/locks, acceptance criteria and stop conditions. A delegated child inherits no product, ownership, integration or acceptance authority beyond that packet. Child completion is a result for the parent/Manager to review, not acceptance, integration or a satisfied gate. Delegation must not bypass active-task ownership, the Unity lock or the independent-review requirement. Parallel Unity is never permitted unless separately authorised.

### Review boundary

A provider-native helper or child working beneath an implementation parent does not count as an independent reviewer for that parent task; authorisation or convenience does not change this. Formal independent review requires a separately assigned review role/task, independent of the implementing thread and consistent with the gates below. A separately assigned reviewer may be launched or delegated through T3 when genuinely independent of the implementation being reviewed and given an explicit reviewer assignment/packet. Delegation or completion alone never satisfies a gate. The review boundary is unchanged; G1–G5 are not waived, weakened or satisfied by child completion.

## Review gates

Use explicit gates rather than standing reviewer ownership. Assign bounded reviewers independently of the implementing thread; use cross-family review where the packet requires it. Gate completion requires evidence and an explicit decision, not worker completion or capacity availability.

| Gate | Trigger |
|---|---|
| G1 COST / LAUNCH | Tier-2/3 Unity; expensive multi-run queues; exceptional model/cost escalation. |
| G2 ACCEPTANCE | Before evidence/handoff is treated as meeting its acceptance criterion. |
| G3 MEASUREMENT INTEGRITY | Harness/fixture/threshold/expected-hash changes; calibration or measured evidence relied upon; any proposed weakening/waiver of a gate. |
| G4 INTEGRATION / CANON | Integration into shared/main candidate state; canonical context/workflow/instruction changes; conflicting shared/serialized systems. |
| G5 SCOPE / MILESTONE | Scope, milestone, product direction, dependency or save-format/history-policy change. |

Protected density, ecology/calibration, save, RNG/determinism and frozen/reference behavior require G2, G3 where measurement/calibration is involved, and G4 at integration. A proposed waiver triggers review; that review does not itself authorise bypassing a mandatory gate. Product/scope decisions still return to the existing authority hierarchy.

Canonical integration decisions remain APPROVED / NEEDS CHANGES / REJECTED. Per-packet A/B/C assessments are supplementary and must map explicitly to canonical decision status; they do not silently replace it. Deferred/batched review cannot mark an unmet mandatory gate satisfied.

## Role persistence

Loading sequence: entry instructions (`AGENTS.md` / `CLAUDE.md`) → role file → explicit task packet → first-message reminder. **This is not an authority order.** Scope and acceptance come from the packet, bounded by locked context and project authority. Role files constrain behavior and grant no extra authority. On conflict, STOP AND REPORT. The first-message reminder is reinforcement only, never the sole carrier of the role. Until entry-file wiring is in place, workers must receive ROLE FILE plus the explicit packet; do not assume automatic loading.

## Live task locks

`Docs/Project/coordination/active-tasks.md` is single-writer live state maintained by Overall Manager/assigned integrator on integration/main coordination branch before parallel work. Workers read it live; packets carry locks where live access is absent. Lock changes do not refresh context. Sequence conflicting tasks or establish an interface. Do not rely on chat memory for ownership.

CCF-Ops may maintain an operational board as a working view of live tasks, lanes and Unity queue. While active-tasks.md remains authoritative, the board grants no lock, ownership or permission. A task is READY only when the board and authoritative lock record agree; disagreement or a missing record means NOT READY. Overall Manager reconciles; never choose the more permissive record.

The current temporary physical record is named in active-tasks.md itself. For this T0/T1 publication, Manager has designated `ops/workflow-foundation-v1:Docs/Project/coordination/active-tasks.md` as that record, explicitly overriding the usual integration/main location for this transition. Read the live designation before future dispatch; this is not a permanent branch rule. Active-tasks is not retired or redefined, and no board-authority/T3 transition occurs here.

## Standard task packet

Fill every substantive field, including full base/context SHAs. The illustrative Tier 1 / Unity No defaults must be changed to match the assigned task. ROLE FILE is a reference or NONE. Existing aliases such as WORKTREE, BASE, STOP AND ASK IF and HANDOFF remain supported.

The DELEGATION, CHILD ROUTING, ORCHESTRATION RECORD and CHILD CONTEXT REQUIREMENT blocks are for substantive delegated or parallel work. Omit them, or mark NONE/N/A, when no delegation occurs; do not turn them into ceremony for tiny reversible tasks. Every delegated child still receives explicit base, context commit, scope, exclusions, ownership/locks, acceptance criteria and stop conditions.

```text
TASK PACKET
PROJECT: CCF
MANAGER: <designated Overall Manager>
ROUTING: <parent harness / model / quota source>
DELEGATION:
  Native subagents: <Allowed / Read-only only / Not allowed>
  T3 delegation: <Allowed / Not allowed>
  Writable child isolation: <Required / Not permitted / As assigned>
  Parallel Unity: Never unless separately authorised.
CHILD ROUTING:
  Preferred child provider/model:
  Review provider/model if required:
ORCHESTRATION RECORD:
  Parent thread:
  Delegated task IDs:
  Child thread IDs:
CHILD CONTEXT REQUIREMENT:
  Child base:
  Child context commit:
  Child scope:
  Child exclusions:
  Child ownership/locks:
  Child acceptance criteria:
  Child stop conditions:
BASE:
CONTEXT COMMIT:
LANE:
BRANCH:
ROLE: Primary / Reviewer / Integrator
ROLE FILE: NONE
UNITY REQUIRED: No
GOAL: <one concise player/system outcome>
AUTHORITATIVE SOURCES:
- Game Brief, Decision Log, Current Milestone, Overview, AgentWorkflow
- relevant research and task specification
IN SCOPE:
-
OUT OF SCOPE:
-
OWNED FILES / SUBSYSTEMS:
-
SHARED / LOCKED FILES:
-
COMPATIBILITY REQUIREMENTS:
- saves, determinism, frozen reference, Unity/pipeline/packages
MUST-NOT-CHANGE HASHES / ANCHORS:
-
INTENTIONALLY CHANGED HASHES / ANCHORS:
None unless explicitly authorised.
VERIFICATION TIER:
Tier 1

TIME BUDGET:
<target>

QUESTION THIS VERIFICATION MUST ANSWER:
<one explicit question>

VERIFICATION:
-
DO NOT ESCALATE TO A HIGHER TIER WITHOUT:
- a failed gate;
- a concrete newly discovered risk;
- a direct protected-reference/save/determinism risk; or
- Manager approval.
STOP AND ASK IF:
Required work materially changes design, ecology, unauthorised save
architecture, frozen/reference behaviour, dependencies, destructive
repository state or ownership boundaries.
HANDOFF FORMAT:
Standard worker handoff below, with exact HEAD, evidence, remaining gates and preserved unrelated work.
```

If UNITY REQUIRED is Yes, add exact HEAD, Unity lane, Unity ownership, allowed phases and Player/native input required (Yes/No). Mechanical lint must verify the configured lane is Unity-capable. These fields supplement the packet; a lint PASS does not authorise execution.

## Standard worker handoff

```text
IMPLEMENTATION HANDOFF
Task:
Worker:
Role:
Branch:
Base:
Context commit:
HEAD:
WHAT CHANGED:
-
FILES CHANGED:
-
SERIALIZED UNITY FILES TOUCHED:
-
PLAYER-FACING RESULT:
-
SIMULATION / SAVE IMPACT:
-
ASSETS ADDED OR MODIFIED:
-
MUST-NOT-CHANGE HASHES / ANCHORS:
-
INTENTIONALLY CHANGED HASHES / ANCHORS:
-
VERIFICATION RUN:
-
VERIFICATION RESULTS:
-
REMAINING VERIFICATION GATES:
-
KNOWN LIMITATIONS:
-
UNRELATED WORK PRESERVED:
-
DECISIONS NEEDED:
-
WORKER ASSESSMENT:
Ready for review: Yes / No / Conditional
DASHBOARD SYNC: PASS / FAIL / NOT AVAILABLE
PUBLISHED DASHBOARD REFLECTS THIS TASK: YES / NO
DASHBOARD SYNC FOLLOW-UP:
- (required if FAIL, NOT AVAILABLE or NO; give the reason and required follow-up)
RECOMMENDED NEXT STEP:
-
```

## Reviewer/integrator decision

Only reviewer/integrator records:

```text
INTEGRATION DECISION
Status: APPROVED / NEEDS CHANGES / REJECTED
Gate: G1 / G2 / G3 / G4 / G5 (applicable gate and scope)
Reviewed branch:
Reviewed HEAD:
Context commit:
Reasons:
-
Required fixes / remaining gates:
-
Integration target:
Integrated HEAD: <only after integration>
Post-integration verification:
-
```

## Cloud-to-local handoff

Workers without required Unity runs must say so:

```text
CLOUD HANDOFF — NOT UNITY-VERIFIED
Base:
Context commit:
HEAD:
Branch:
Files changed:
Serialized Unity files touched:
Package/ProjectSettings changes:
MUST-NOT-CHANGE HASHES / ANCHORS:
-
INTENTIONALLY CHANGED HASHES / ANCHORS:
None unless explicitly authorised.
Tests actually run:
UNITY GATES STILL REQUIRED:
-
Suggested commands/menu actions:
-
Expected observable behaviour:
-
```

## Integration states and procedure

- **PROPOSED:** specification only.
- **IMPLEMENTED ON TASK BRANCH:** worker completed its branch.
- **REVIEWED:** another worker/manager inspected it.
- **NEEDS CHANGES:** fixes required.
- **REJECTED:** path will not be integrated.
- **INTEGRATED:** merged/cherry-picked into integration worktree.
- **VERIFIED:** required integrated runtime/regression/performance/player gates passed.
- **ACCEPTED:** user/Manager accepts result.

Before integration inspect exact branch/HEAD and expected/unexpected changes; preserve unrelated work; deliberately inspect serialized conflicts; integrate; perform required import/compile/runtime gates in the designated clean Unity lane at that exact integrated HEAD under the Unity lock; run required verification and AGENTS checks; report final integration/main HEAD.

Independent review matters for expensive/subtle failures: ecology, saves, determinism/reference, large refactors, multiplayer architecture and substantial performance changes. Routine reversible work does not automatically require two agents.

## Manager continuation after worker handoff

After receiving a worker handoff, the Overall Manager must immediately triage it and keep the work moving.

If the handoff is sufficiently clear, no user decision is required, no blocking conflict or mandatory review gate exists, and the next step is already implied by the accepted roadmap or task sequence, issue the next required bounded task packet or handoff in the same response. Do not wait for the user to ask for a handoff.

Return to the user for a decision only when the next step requires a genuine product/design choice, material scope change, unresolved conflict, unsafe integration choice, or materially ambiguous direction.

Independent review is not automatically required after every handoff. Follow the review gates and batch reviews where appropriate; if review is deferred, record the implementation state accurately and continue with the next safe task.

## Manager workflow

1. Inspect accepted state and choose the next playable outcome.
2. Split parallel-safe tasks, update locks, assign workers, and issue bounded packets with one context commit.
3. Workers execute against locked context and return implementation handoffs.
4. Immediately triage each handoff against specification, evidence and the review gates. Perform required reviews or explicitly record deferred/batched review and the accurate implementation state.
5. When no decision, blocking conflict or mandatory review gate prevents continuation, issue the next implied bounded task packet or handoff in the same response. Otherwise resolve the gate or return the genuine decision to the user.
6. Use one authorised integrator for approved work and run required integrated gates.
7. Update canonical context when needed, commit, regenerate mirrors/composites, refresh attachments, and continue to the next accepted outcome.
8. Confirm dashboard synchronization for every substantial task and project-status change: update the authoritative coordination record, run or allow ingestion, verify the automatic GitHub audit snapshot advances, and verify the published dashboard reflects the new state. Dashboard publication is required, not optional reporting. If the dashboard or publisher is unavailable, record the synchronization failure as an outstanding operational issue.
