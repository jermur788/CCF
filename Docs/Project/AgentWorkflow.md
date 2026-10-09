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

## Capability

Use only currently available tools. Chats without repo/Unity tools may plan/research/review, not claim local implementation. Claude Desktop MCP can perform authorised local work; Code/cloud can implement but claim Unity verification only after actual runs. OpenAI/OpenCode can use available code/Unity/Blender/asset/integration tools.

## Cost-aware task routing

Use the least costly capable route for the task, considering actual tools, context, availability and verification needs. Use these routing labels in Manager packets and recommendations:

- **FREE HERE:** bounded planning, clarification, interpretation and coordination that the current chat can complete without a paid worker handoff.
- **DEEP RESEARCH:** substantial evidence collection and synthesis; keep research recommendations distinct from accepted mechanics.
- **SOL/CODEX:** repository/documentation work, implementation or local verification suited to available Codex tools.
- **REGULAR CLAUDE VALUE:** suitable independent review/reasoning; default direction is Sonnet 5.5 xhigh.
- **CLAUDE CLOUD VALUE:** substantial repository/implementation work; default preference is Ultra Code where it is capable.

Reserve Opus for unusually large, difficult or high-stakes tasks where additional capability materially justifies the cost. These are routing preferences, not claims that a model or tool is available. Verify actual capability before dispatch. Worker-cost policy belongs in workflow/role instructions, not the gameplay Decision Log.

## Multi-agent safety

1. One writable local worktree per active local agent.
2. No concurrent writes to the same Unity worktree.
3. Substantial tasks declare exact base, context commit and branch.
4. One primary writer owns a subsystem/file set.
5. Enforce high-conflict/asset safety from AGENTS through locks.
6. Do not stash/discard/overwrite/clean/silently absorb unrelated user work.
7. Major design, ecology, save architecture, dependencies, frozen-reference or destructive decisions return to Manager/user.

Target paths are `/home/jer/CCF-main`, `/home/jer/CCF-openai`, `/home/jer/CCF-claude`; preserve existing paths if renaming risks work and record verified results in Overview.

## Pre-write checks

Identify exact worktree, branch/HEAD, status and unrelated changes; apply AGENTS technical checks; confirm unsaved serialized Unity edits if relevant, ownership/locks and context commit. Stop if those cannot be established safely.

## Roles

- **Primary:** implements bounded authorised scope; reports readiness for review, not its own integration approval.
- **Reviewer:** checks against accepted specification/context/evidence/gates; no competing implementation unless requested. Prefer an independent reviewer for high-risk simulation/save/reference work.
- **Integrator:** deliberately integrates approved work and runs post-integration gates; only one active integrator per integration worktree.
- **Self-integration:** only with explicit Manager permission for small reversible low-risk tasks. Not default for save migrations, determinism/reference, large serialized changes, major refactors or dependencies.

## Live task locks

`Docs/Project/coordination/active-tasks.md` is single-writer live state maintained by Overall Manager/assigned integrator on integration/main coordination branch before parallel work. Workers read it live; packets carry locks where live access is absent. Lock changes do not refresh context. Sequence conflicting tasks or establish an interface. Do not rely on chat memory for ownership.

## Standard task packet

```text
TASK PACKET
PROJECT: CCF
MANAGER: <designated Overall Manager>
BASE: <exact implementation SHA>
CONTEXT COMMIT: <full snapshot SHA>
WORKTREE: <worker-specific path if local>
BRANCH: <task branch>
ROLE: Primary / Reviewer / Integrator
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
VERIFICATION:
-
STOP AND ASK IF:
Required work materially changes design, ecology, unauthorised save
architecture, frozen/reference behaviour, dependencies, destructive
repository state or ownership boundaries.
```

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
RECOMMENDED NEXT STEP:
-
```

## Reviewer/integrator decision

Only reviewer/integrator records:

```text
INTEGRATION DECISION
Status: APPROVED / NEEDS CHANGES / REJECTED
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

Before integration inspect exact branch/HEAD and expected/unexpected changes; preserve unrelated work; deliberately inspect serialized conflicts; integrate; import/compile in integration worktree; run required verification and AGENTS checks; report final integration/main HEAD.

Independent review matters for expensive/subtle failures: ecology, saves, determinism/reference, large refactors, multiplayer architecture and substantial performance changes. Routine reversible work does not automatically require two agents.

## Manager continuation after worker handoff

After receiving a worker handoff, the Overall Manager must immediately triage it and keep the work moving.

If the handoff is sufficiently clear, no user decision is required, no blocking conflict or mandatory review gate exists, and the next step is already implied by the accepted roadmap or task sequence, issue the next required bounded task packet or handoff in the same response. Do not wait for the user to ask for a handoff.

Return to the user for a decision only when the next step requires a genuine product/design choice, material scope change, unresolved conflict, unsafe integration choice, or materially ambiguous direction.

Independent review is not automatically required after every handoff. Follow the current review policy and batch reviews where appropriate; if review is deferred, record the implementation state accurately and continue with the next safe task.

## Manager workflow

1. Inspect accepted state and choose the next playable outcome.
2. Split parallel-safe tasks, update locks, assign workers, and issue bounded packets with one context commit.
3. Workers execute against locked context and return implementation handoffs.
4. Immediately triage each handoff against specification, evidence and the current review policy. Perform required reviews or explicitly record deferred/batched review and the accurate implementation state.
5. When no decision, blocking conflict or mandatory review gate prevents continuation, issue the next implied bounded task packet or handoff in the same response. Otherwise resolve the gate or return the genuine decision to the user.
6. Use one authorised integrator for approved work and run required integrated gates.
7. Update canonical context when needed, commit, regenerate mirrors/composites, refresh attachments, and continue to the next accepted outcome.
