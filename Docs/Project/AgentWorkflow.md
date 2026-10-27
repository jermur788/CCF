# CCF Multi-Agent Development Workflow

## Purpose

Single owner of source authority/conflicts, spec-driven task classification/lifecycle, context-commit meaning, worktrees/branches, roles/locks, task packets/handoffs, review/integration states, cloud-to-local verification and Drive/project-context synchronization.

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

Approved feature specifications are task-level implementation authority for their scope only. They remain subordinate to the four sources above. If a spec conflicts with newer accepted product intent, amend the spec rather than implementing the stale requirement.

### Implementation authority

For what currently exists:

1. inspected repository / verified Unity state;
2. `Docs/Project/unity-project-overview.md`.

Planning text does not override verified implementation facts. Research and agent output are supporting evidence/proposals, not accepted decisions unless the user accepts them.
<!-- PROJECT_AUTHORITY_BLOCK_END -->

Report conflicts and identify the authoritative file needing update.

## Spec-driven development

CCF uses spec-driven development selectively. The Overall Manager classifies substantial work before production implementation. Classification is a workflow decision; it does not create product authority.

### PACKET ONLY

Use the ordinary AgentWorkflow packet and handoff for:

- small bugs;
- bounded fixes;
- verification repairs;
- simple wording/content changes;
- straightforward implementation that fits one clear packet.

Do not create a spec, plan or task list for this class. The packet remains the implementation instruction within accepted context.

### SPEC REQUIRED

Normally use SDD for:

- new user/player workflows;
- cross-system features;
- persistence/data architecture;
- save-policy changes;
- security/write-back behaviour;
- substantial UX;
- major forestry/gameplay systems;
- multi-step or multi-agent features;
- work where ambiguity is likely to waste implementation tokens.

The Overall Manager may upgrade a task to SPEC REQUIRED after inspection shows hidden ambiguity or cross-system impact. A worker that discovers a PACKET ONLY task has become substantially ambiguous stops and reports; it does not invent a spec or continue on an unapproved assumption.

### SDD lifecycle

1. classify `SPEC REQUIRED`;
2. create the specification;
3. resolve genuine product ambiguity with the user/authority;
4. approve the specification;
5. produce the implementation plan;
6. divide the work into bounded tasks;
7. implementation agents work from the approved artifacts;
8. implementation ambiguities that materially affect product behaviour return to the Manager/spec;
9. verify implementation correctness;
10. verify spec conformance;
11. integrate;
12. preserve the completed spec as project history.

Implementation agents must not rediscover settled product design. If a requirement is unclear, cite the spec section and ask; do not substitute a convenient assumption.

### Spec authority

Approved feature specs are task-level implementation authority for their scope. They remain subordinate to:

1. latest user instruction;
2. `decision-log.md`;
3. `current-milestone.md`;
4. `game-brief.md`.

Implementation truth remains the inspected repository / verified Unity state. A spec describes intended behaviour; it never proves that behaviour exists or overrides verified implementation facts. If a spec conflicts with newer accepted product intent, amend the spec rather than implementing the stale requirement. If implementation reality makes the spec impractical, return to the Manager/spec; do not silently reinterpret it.

Only specs in `APPROVED`, `IMPLEMENTING`, `VERIFYING` or `VERIFIED` status authorise implementation. `DRAFT` and `SUPERSEDED` specs do not.

### Spec artifacts

Canonical root: `specs/`. Standard structure:

```text
specs/<feature>/spec.md
specs/<feature>/plan.md
specs/<feature>/tasks.md
specs/<feature>/checklists/requirements.md
```

Reusable templates are in `specs/_template/`. `specs/README.md` owns the artifact and status conventions. The plan and task list implement the approved spec; they do not create product requirements or override it. GitHub Spec Kit tooling is not required; its conventions may be used as a structure/process reference only.

A feature may omit or combine sections where they are genuinely not applicable, but it must not silently drop acceptance criteria, stop conditions or the authority statement. Spec status is separate from implementation/integration states.

Status vocabulary:

- `DRAFT` — being written/clarified; not approved;
- `APPROVED` — accepted product intent and acceptance criteria; may be planned and divided into tasks;
- `IMPLEMENTING` — implementation in progress against approved tasks;
- `VERIFYING` — implementation exists and is being checked for correctness and spec conformance;
- `VERIFIED` — implementation and spec conformance accepted;
- `SUPERSEDED` — retained for history, replaced by a newer spec/decision.

## Canonical context

Shared sources: `Docs/Project/game-brief.md`, `decision-log.md`, `current-milestone.md`, `unity-project-overview.md`, `AgentWorkflow.md`; supporting `research-index.md`.

Feature specifications are canonical project artifacts under `specs/`. Load the active approved spec, plan and assigned task only when relevant to the task; do not load every spec into every context. Completed and superseded specs remain historical evidence.

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
2. allow/run the dashboard ingestion/update mechanism where that mechanism is in use;
3. confirm any automatic GitHub dashboard audit snapshot advances;
4. confirm the published dashboard state actually reflects the new project status.

A substantial task is not fully closed until this synchronization is verified.

If the dashboard or publisher is unavailable, the task may hand off, but the synchronization failure must be explicitly recorded as an outstanding operational issue.

Never manually edit generated dashboard-state files.

## Verification cost policy

Use the cheapest verification tier that can answer the current risk. Do not repeatedly run full Unity qualification on intermediate implementation or calibration branches.

### Tier 1 — FAST

Target: **5–15 minutes**.

Use during active implementation and calibration. Prefer compile/import where needed, feature-specific smoke, relevant offline/static tests and one representative deterministic runtime fixture. Stop when the immediate implementation question is answered.

### Tier 2 — FEATURE QUALIFICATION

Target: **20–60 minutes**.

Use when a bounded feature is ready to hand off. Run the specific Unity behaviour the feature changes, important negative cases, save/load only when persistence is relevant, and a narrow regression set covering plausible collateral damage.

### Tier 3 — FULL INTEGRATED QUALIFICATION

Several hours can be acceptable, but reserve this tier for integrated milestone or release candidates. This is where exhaustive regression, all required reference anchors, multiple long-run simulations, broad save/load coverage, rendered UX matrices, Player profiling, multiple resolutions and platform qualification belong.

Do not rerun Tier 3 merely because documentation, verification tooling, calibration evidence or an unrelated subsystem changed.

Repeated long simulations, multiple seeds and multiple stand geometries require a stated uncertainty they are intended to resolve. Stop once that question has sufficient evidence; do not expand testing into open-ended certification.

During feature iteration, prefer offline/reduced-model testing for parameter search and use Unity to confirm representative runtime equivalence. Do not use Unity as the parameter-search loop.

After a feature passes its bounded qualification, defer broad regression to the next integrated candidate unless the feature directly threatens a frozen reference, save compatibility, deterministic behaviour or another high-risk shared system.

Escalation to a higher tier requires at least one of:

- a failed gate;
- a concrete newly discovered risk;
- a direct threat to a protected reference/save/determinism contract; or
- explicit Manager approval.

## Capability

Use only currently available tools and distinguish their scope precisely.

When the Overall Manager has connected GitHub write access, GitHub-native management is **FREE HERE** and should normally be done directly rather than delegated. This includes repository inspection, commit/branch/PR review, live-lock maintenance, canonical documentation/specification/research/decision updates, durable handoff publication, and small reviewable text/source edits that do not require local Editor/build verification.

GitHub access is not local-worktree or Unity access. A GitHub change proves repository state only; it does not prove import, compilation, runtime behaviour, rendering, local filesystem state or worktree cleanliness. Production Unity/C# implementation, serialized assets, builds, local integration, difficult debugging and runtime verification normally remain with a worker that has those tools.

Chats without repository tools may plan/research/review only. Claude Desktop MCP can perform authorised local work; Code/cloud can implement but claim Unity verification only after actual runs. OpenAI/OpenCode can use available code/Unity/Blender/asset/integration tools.

## Cost-aware task routing

Use the least costly capable route for the task, considering actual tools, context, availability and verification needs. Use these routing labels in Manager packets and recommendations:

- **FREE HERE:** planning, clarification, interpretation, coordination, GitHub-native repository inspection, live-lock management, canonical documentation/spec maintenance, handoff publication, and small reviewable text/source edits the current chat can safely complete without local Unity/build verification.
- **DEEP RESEARCH:** substantial evidence collection and synthesis; keep research recommendations distinct from accepted mechanics.
- **SOL/CODEX:** repository/Unity implementation, local worktree changes, tooling, integration, debugging or verification requiring compiler/Editor/runtime access.
- **REGULAR CLAUDE VALUE:** suitable independent review/reasoning; default direction is Sonnet 5.5 xhigh.
- **CLAUDE CLOUD VALUE:** substantial repository/implementation work; default preference is Ultra Code where it is capable.

Reserve Opus for unusually large, difficult or high-stakes tasks where additional capability materially justifies the cost. These are routing preferences, not claims that a model or tool is available. Verify actual capability before dispatch. Worker-cost policy belongs in workflow/role instructions, not the gameplay Decision Log.

## Multi-agent safety

1. One writable lane/worktree per active owner; one active owner per writable lane/worktree.
2. No concurrent conflicting writes, and no two writable lanes/worktrees on the same branch, including across separate Git stores.
3. Substantial tasks declare exact base, context commit and branch.
4. One primary writer owns a subsystem/file set.
5. Enforce high-conflict/asset safety from AGENTS through locks.
6. Do not stash/discard/overwrite/clean/silently absorb unrelated user work.
7. Major design, ecology, save architecture, dependencies, frozen-reference or destructive decisions return to Manager/user.

Use the registered worktree/lane paths recorded in `Docs/Project/coordination/active-tasks.md`; preserve existing paths rather than renaming or recreating them automatically, and record verified results in Overview.

## Unity resource policy

Serialize heavy Unity execution machine-wide under one ownership lock across lanes and Git stores. A Unity packet states Unity required, exact candidate HEAD, Unity lane/owner, allowed phases and Player/native-input requirement. Verify the clean lane's actual branch/HEAD and ownership before acquiring the lock; never bypass another owner or kill its process through the lock.

The integration lane constructs/integrates code and normally does not host the Editor. Post-integration Unity gates run in a designated Unity-capable lane, placed cleanly at the exact integrated HEAD under the Unity lock. Do not duplicate the active integration branch into another writable lane; detached candidate validation or a distinct owned lane branch preserves branch ownership.

Follow the verification-cost policy above: offline/reduced search first, Unity confirms candidate behaviour, and stop when the stated uncertainty is answered. Availability of an Editor does not expand scope.

## Pre-write checks

For local work, identify exact worktree, branch/HEAD, status and unrelated changes; apply AGENTS technical checks; confirm unsaved serialized Unity edits if relevant, ownership/locks and context commit. Stop if those cannot be established safely.

For GitHub-native Manager writes, inspect the authoritative branch/path and current blob/commit state, preserve concurrent work, keep the change bounded, and do not claim local-worktree or Unity verification. GitHub-native coordination writes do not require a clean local Primary checkout because they do not mutate it.

## Roles

- **Primary:** implements bounded authorised scope; for SPEC REQUIRED work, works from the approved spec, plan and assigned task and reports spec deviations; reports readiness for review, not its own integration approval.
- **Reviewer:** checks against accepted specification/context/evidence and the review gates below; for SPEC REQUIRED work, checks spec conformance as well as implementation correctness; independent of the implementing thread for the reviewed change; no competing implementation unless requested. Prefer an independent reviewer for high-risk simulation/save/reference work. A provider-native helper or child of the implementing worker does not count as an independent reviewer for that parent task.
- **Integrator:** deliberately integrates approved work and runs post-integration gates; for SPEC REQUIRED work, confirms the spec is approved and conformance was reported; only one active integrator per integration worktree.
- **Self-integration:** only with explicit Manager permission for small reversible low-risk tasks. Not default for save migrations, determinism/reference, large serialized changes, major refactors or dependencies.

## Review gates

Use explicit gates rather than standing reviewer ownership. Assign bounded reviewers independently of the implementing thread. Gate completion requires evidence and an explicit decision, not worker completion or capacity availability.

| Gate | Trigger |
|---|---|
| G1 COST / LAUNCH | Tier-2/3 Unity; expensive multi-run queues; exceptional model/cost escalation. |
| G2 ACCEPTANCE | Before evidence/handoff is treated as meeting its acceptance criterion. |
| G3 MEASUREMENT INTEGRITY | Harness/fixture/threshold/expected-hash changes; calibration or measured evidence relied upon; any proposed weakening/waiver of a gate. |
| G4 INTEGRATION / CANON | Integration into shared/main candidate state; canonical context/workflow/instruction changes; conflicting shared/serialized systems. |
| G5 SCOPE / MILESTONE | Scope, milestone, product direction, dependency or save-format/history-policy change. |

Protected density, ecology/calibration, save, RNG/determinism and frozen/reference behaviour require G2, G3 where measurement/calibration is involved, and G4 at integration. A proposed waiver triggers review; that review does not itself authorise bypassing a mandatory gate. Product/scope decisions still return to the existing authority hierarchy.

Canonical integration decisions remain APPROVED / NEEDS CHANGES / REJECTED. Per-packet assessments are supplementary and must map explicitly to canonical decision status; they do not silently replace it. Deferred/batched review cannot mark an unmet mandatory gate satisfied.

## Live task locks

`Docs/Project/coordination/active-tasks.md` is single-writer live state maintained by Overall Manager/assigned integrator before parallel work. When connected GitHub write access is available, the Overall Manager may maintain this file directly on the authoritative coordination/main branch without routing the edit through a local worker.

Workers with live GitHub access must fetch the current authoritative GitHub copy at substantial task start and again before declaring a registration/lock missing. A stale or dirty local coordination copy does not override a newer live GitHub registration. Packets carry locks where live access is absent. Lock changes do not refresh context. Sequence conflicting tasks or establish an interface. Do not rely on chat memory alone for ownership.

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
TASK CLASS: PACKET ONLY / SPEC REQUIRED
SPEC (SPEC REQUIRED only):
- ID / status:
- Location:
- Assigned task(s):
- Acceptance criteria for this packet:
GOAL: <one concise player/system outcome>
AUTHORITATIVE SOURCES:
- Game Brief, Decision Log, Current Milestone, Overview, AgentWorkflow
- relevant research and task specification
- approved spec, plan and assigned task for SPEC REQUIRED work
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
VERIFICATION TIER: 1 / 2 / 3
VERIFICATION:
-
GITHUB HANDOFF:
- tracked Markdown path required for substantive result
STOP AND ASK IF:
Required work materially changes design, ecology, unauthorised save
architecture, frozen/reference behaviour, dependencies, destructive
repository state or ownership boundaries. For SPEC REQUIRED work, also
stop if the approved spec does not settle a material product ambiguity.
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
GITHUB HANDOFF PATH:
GITHUB HANDOFF COMMIT:
EXTERNAL EVIDENCE ARCHIVE:
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
SPEC CONFORMANCE (SPEC REQUIRED only): PASS / FAIL / NOT VERIFIED
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

## Durable handoff publication

Every substantive implementation, qualification, review, integration or research result must have a GitHub-tracked Markdown handoff. Large screenshots, logs, binaries and raw measurement artifacts may remain in `/media/...` or other external evidence storage, but the GitHub handoff must contain enough summary, verification, failures, unresolved decisions, evidence locations and verdict for remote Manager review.

A technically valid local result may be accepted for its narrow gate, but a local-only handoff is administratively incomplete and the task is not fully closed until the GitHub copy exists. The Overall Manager may publish or repair documentation-only handoffs directly through GitHub when the substantive worker result is already established.

## Reviewer/integrator decision

Only reviewer/integrator records:

```text
INTEGRATION DECISION
Status: APPROVED / NEEDS CHANGES / REJECTED
Gate: G1 / G2 / G3 / G4 / G5 (applicable gate and scope)
Reviewed branch:
Reviewed HEAD:
Context commit:
SPEC (SPEC REQUIRED only):
- ID / status:
- Conformance: PASS / FAIL / NOT VERIFIED
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
GitHub handoff path:
GitHub handoff commit:
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

Spec status (see Spec-driven development) is separate from these implementation states. `PROPOSED` here means an implementation proposal, not an approved feature spec.

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

Use direct GitHub access first for Manager-owned follow-up that is purely repository administration, live-lock maintenance, canonical text/spec changes or handoff publication. Do not spend an implementation worker merely to perform those tasks when the Manager can safely complete them.

Return to the user for a decision only when the next step requires a genuine product/design choice, material scope change, unresolved conflict, unsafe integration choice, or materially ambiguous direction.

Independent review is not automatically required after every handoff. Follow the current review policy and batch reviews where appropriate; if review is deferred, record the implementation state accurately and continue with the next safe task.

## Manager workflow

1. Inspect accepted state and choose the next playable outcome.
2. Classify the work as `PACKET ONLY` or `SPEC REQUIRED` before production implementation. For `SPEC REQUIRED`, ensure an approved spec exists, then maintain the plan and bounded tasks. For `PACKET ONLY`, continue with the ordinary packet.
3. Use direct GitHub capability for live locks, canonical documentation/spec updates, handoff publication and other safe GitHub-native management; split remaining parallel-safe local/Unity work, assign workers, and issue bounded packets with one context commit.
4. Workers execute against locked context and return GitHub-published implementation handoffs plus external evidence where needed.
5. Immediately triage each handoff against the approved spec where applicable, evidence and the current review policy. Perform required reviews or explicitly record deferred/batched review and the accurate implementation state.
6. When no decision, blocking conflict or mandatory review gate prevents continuation, perform Manager-owned GitHub follow-up directly and issue the next implied bounded worker task. Otherwise resolve the gate or return the genuine decision to the user.
7. Use one authorised integrator for approved work requiring local integration and run required integrated gates, including spec-conformance verification for `SPEC REQUIRED` work.
8. Update canonical Git context when needed. After the context-source commit, regenerate mirrors/composites and refresh attachments; if the Manager cannot run that local publication tooling, assign a bounded context-publication worker and do not present stale mirrors as current.
9. Confirm dashboard synchronization for every substantial task and project-status change: update the authoritative coordination record, run or allow ingestion, verify any automatic GitHub audit snapshot advances, and verify the published dashboard reflects the new state. If the dashboard or publisher is unavailable, record the synchronization failure as an outstanding operational issue.
