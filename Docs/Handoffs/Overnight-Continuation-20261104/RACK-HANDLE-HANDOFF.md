# RACK HANDLE — IMPLEMENTED

Issue #256; parent #254. Base `72b4efbd1c653fa2231f632a79030a83ff76f833`. Branch `feature/overnight-rack-handle-20261104`.

## Inspection transcript

Read-only inspections were repeated here to capture complete raw output after the interactive output limit truncated initial reads. All commands ran in the assigned worktree. Initial status was clean. Prior interactive commands were the same reads/searches, with `sed` slices of the editor (1–330, 320–540) and harness (140–330), and `rg -n 'rack|Rack|Shift' Assets/ForestPrototype/UI/StandMapView.cs`. Their full source output is captured below.

<details><summary>Commands and raw inspection output</summary>

```text

$ pwd && git status --short && git log --oneline -1
/media/jer/Files/CCF/Operations/CCF-Overnight-RackHandle-20261104
72b4efbd map: draw living conifer crowns as pointed whorl stars; smooth Beech outline (#232)

[exit 0]

$ git branch --show-current && git remote -v
feature/overnight-rack-handle-20261104
origin	https://github.com/jermur788/CCF.git (fetch)
origin	https://github.com/jermur788/CCF.git (push)

[exit 0]

$ cat AGENTS.md
# CCF Agent Entry Point

Before substantial work, read:

1. `Docs/Project/AgentWorkflow.md` — authority, context commit, worktrees, task roles, handoffs and integration.
2. `Docs/Project/decision-log.md` — accepted/current/open decisions.
3. `Docs/Project/current-milestone.md` — current scope.
4. `Docs/Project/game-brief.md` — long-term player/game vision.
5. `Docs/Project/unity-project-overview.md` — verified implementation facts and local verification guidance.
6. `Docs/Project/research-index.md` when ecology/research context is relevant.
7. `Docs/Project/coordination/active-tasks.md` at task start when live Git access is available.

AgentWorkflow owns source authority/conflict rules and multi-agent coordination.

## Repository and Unity rules

- Use only tools actually available in the current session.
- Inspect relevant existing files before editing.
- State the smallest reasonable change and the files you intend to touch.
- Explain material architecture, gameplay, rendering, save-data, package or project-structure trade-offs before changing them.
- Do not modify unrelated files. Check Git status and report unrelated changes before substantial work; preserve user work.
- Use Unity 6000.6.0f1 and the packages actually installed.
- Use the New Input System; do not introduce legacy `UnityEngine.Input` unless explicitly requested.
- Keep materials/shaders URP-compatible.
- Do not edit generated folders: `Library`, `Temp`, `Logs`, `obj`.
- Do not manually edit generated solution/project files without a specific reason.
- Preserve `.meta` files and GUID relationships.
- Do not delete assets unless explicitly authorised.
- Prefer built-in Unity features when they adequately solve the problem.
- Avoid unnecessary managers, singletons, service locators, frameworks and dependency injection.
- Write straightforward, descriptive C# suitable for a learner to maintain. Keep scripts focused on one responsibility where practical.
- Avoid premature optimisation. Comments should explain non-obvious reasons, not narrate every line.
- Do not silently rewrite a working system merely because another architecture looks cleaner.
- Do not add packages, Git LFS or external dependencies without approval.
- Do not alter frozen Scenario One Reference Future v1 unless explicitly authorised.
- `.vscode/settings.json` is tracked shared configuration; do not make routine local edits to it.
- If local VS Code settings differ, use user settings or an external `.code-workspace` outside the repo.
- Each Unity worktree has its own `Library`; do not copy/share one between worktrees.
- Build one small, playable improvement at a time. Do not implement a whole loop or speculative multiplayer infrastructure without task scope requiring it.

## Ecology and communication

Keep ecological simulation rules reasonably separate from graphics and controls where that clearly helps maintenance/testing. Do not invent ecological facts: flag assumptions needing research and distinguish established knowledge, simplified simulation rules and deliberate gameplay calibration. Planting trees is not automatically environmentally beneficial. Forestry should create visible consequences for regeneration, canopy, habitat and productivity.

Assume the user is learning Unity/C#. Explain unfamiliar concepts briefly, prefer one clear recommendation over unnecessary alternative architectures, and inspect available project information before asking for it. Distinguish verified facts, assumptions, proposals, actual changes and unresolved questions. Never claim completion without making and verifying the change.

## Verification

Compilation alone is not sufficient. After changing gameplay:

- run appropriate compiler/test/runtime gates;
- explain exactly what changed;
- give exact Unity test steps or commands;
- state expected observable behaviour;
- report anything not directly verified.

Important behavioural checks include movement/interaction, harvesting a tree only once, correct resource awards and construction consumption, regeneration, and persistent world changes after save/load.

For disposable scripts under `Tools/Verification/`, follow the documented copy/run/remove workflow and ensure no temporary script or generated `.meta` remains under `Assets/ForestPrototype/`.

### Serialized Unity assets

- Scenes, prefabs, ScriptableObjects, ProjectSettings and package/dependency files are high-conflict and single-writer by default.
- Avoid broad reserialization or opening/saving unrelated scenes/prefabs.
- Cloud/code-only workers should not hand-edit Unity YAML unless the task explicitly requires it and the diff can be reviewed.
- Never resolve a Unity YAML/serialized-file conflict mechanically without understanding both sides.
- Keep `.meta` files with their assets and preserve GUID relationships.

### Blender, binaries and asset production

- Do not assume Git LFS is configured; no LFS/storage-policy change is allowed without approval.
- Before adding large binary asset families, report expected footprint and preserve source/licensing provenance.
- Every substantial asset brief should specify biological/game state, visual purpose/reference, scale, variants, LOD, Unity requirements, biological boundary, performance target, source/licence and acceptance criteria.
- Asset generation is not completion. Completion is: **source asset → Unity import → material/prefab/LOD → representative scene → rendered player review → performance/behaviour check**.

### Verification harness warnings

- Inspect a harness before running it; do not assume identical side effects.
- The disposable integration harness is copied from `Tools/Verification/` into `Assets/ForestPrototype/`, run, then removed together with its generated `.meta`.
- It can touch `forest-save.json`; do not run it where the local save matters unless its backup/restore behaviour is acceptable.
- Auto-start verification scripts must never remain in a playable build.
- Deterministic/lifecycle hashes are comparable only under the same verified Editor/configuration conditions described by the harness documentation.

## Multi-agent

No model family permanently owns code, Unity, Blender, assets or testing. Follow the current task packet, context commit and temporary ownership from `Docs/Project/coordination/active-tasks.md` (or the lock state copied into the task packet).

When live GitHub access is available, fetch the current authoritative GitHub copy of `Docs/Project/coordination/active-tasks.md` at substantial task start and before reporting that a task/lock is missing. Do not let a stale or dirty local coordination copy override newer live GitHub state.

The Overall Manager may perform GitHub-native coordination, canonical documentation/specification updates and other bounded inspection-verifiable repository changes directly through connected GitHub tools. Such GitHub edits do not imply local worktree, Unity import, compilation, runtime or rendered verification.

Every substantive implementation, qualification, review, integration or research handoff must have a GitHub-tracked Markdown copy. Large raw evidence may remain in external verification storage, but a local-only handoff is administratively incomplete.

Do not call task-branch work integrated or verified until required integration gates have passed. Permanent Google Drive context mirrors are read-only collaboration copies. Make project-context changes in Git and regenerate mirrors; do not edit permanent mirrors as the source of truth.

## Legacy instruction migration audit

The useful rules from root `AI_Instructions` are represented before its retirement:

| Legacy section | Current owner |
|---|---|
| Technical setup (Unity/URP/Input/scenes/controller/Linux/VS Code; inspect facts) | Unity Project Overview; repository rules above |
| Small playable improvements, simplest maintainable solution, staged scope, multiplayer restraint | Game Brief; shared instructions; repository rules |
| Pre-change inspection, smallest change/files, trade-offs, unrelated-work protection | Repository rules; AgentWorkflow pre-write checks |
| Unity version, URP/Input, generated files, serialized assets, metadata, deletion, packages, built-ins | Repository/serialized-asset rules |
| Clear learner-maintainable C#, focused scripts, simple design, optimisation/comments, no silent rewrites | Repository rules |
| Ecology separation, evidence/assumptions, planting/site suitability, visible consequences | Ecology section; shared instructions; Game Brief |
| Git status, preserve user work, actual behavioural verification and observable test steps | Repository/verification rules; AgentWorkflow |
| Plain language, concepts, focused recommendations, facts/proposals/unverified distinctions | Communication section; shared instructions |
| Early first-playable loop | Superseded history H-001 in Decision Log; current staged loop in Game Brief |

The early prototype direction is preserved as history rather than promoted over the accepted current milestone. Migration source: accepted Revision 5 Drive drafts; the task packet authorises this repository migration.

[exit 0]

$ cat Docs/Project/AgentWorkflow.md
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

[exit 0]

$ cat Docs/Project/decision-log.md Docs/Project/current-milestone.md Docs/Project/game-brief.md
# CCF Project Decision Log

## Purpose

Record accepted decisions, current directions, proposals, deferrals and open questions. Suggestions from a chat, research report or agent must not silently become requirements.

Implementation facts belong in the Unity Project Overview. Current scope belongs in the Current Milestone; temporary task ownership belongs in live coordination.

## Status labels

- **Confirmed** — explicitly accepted by the user.
- **Current direction** — guides planning but may still change.
- **Proposed** — suggested but not accepted.
- **Deferred** — intentionally postponed.
- **Rejected** — considered and declined.
- **Open** — a decision is still required.
- **Superseded** — retained for history but replaced by a later decision.

## Acceptance audit — 2026-10-02

The Overall Manager reviewed the available conversation history before Revision 5. D-001–D-021 reflect accepted project decisions; D-022–D-024 are current directions; D-025–D-030 reflect accepted multi-agent direction; D-031–D-034 remain open. D-035–D-037 and D-040 were explicitly confirmed on 2026-10-02. D-038 preserves the accepted Valheim reference omitted from the first refreshed brief. D-039 was opened concerning pre-existing unmerged technical work and was resolved on 2026-10-02 (stack integrated and verified).

Historical dates remain broad where exact dates were not reconstructed.

The accepted canonical-context sync packet on 2026-10-02 updates D-007, D-009 and D-010 and confirms D-041: simulation-first progression, separate worker/execution choices, and a genuinely bounded Scenario One property. The earlier three-stage roadmap is retained below as superseded history.

The user explicitly accepted the Reference Future v1 contract on 2026-10-02; it is recorded as D-042 after the combined ecology package was integrated and verified on main.

On 2026-10-04 the user's interactive Scenario 1 smoke passed and the Scenario 1 integration was merged to main and verified. The final-integration packet records this as D-043 (functionally complete; final asset/presentation acceptance pending) and the Scenario 1 deferral list as D-044. The D-023 notes are updated because browsing is now integrated.

On 2026-10-12 the user accepted four decisions before simulation implementation continues: D-059 three-species modelling-depth parity, D-060 the first external demo contains the complete Scenario One, D-061 ordinal species-aware mixed-stand density representation v1, and D-062 an Oak/Beech timber/economic layer. D-060 supersedes the previous first-intervention external Stage-A boundary. D-062 reopens in part the broadleaf-market deferral in D-044. These decisions follow the Oak/Beech/Sitka species-parity audit, whose code findings are recorded in the Unity Project Overview; the audit did not change production code.

On 2026-10-22 the user accepted D-063: CCF uses spec-driven development selectively for substantial or ambiguous work. Small bounded work continues through ordinary AgentWorkflow packets. The Overall Manager may author a specification before routing implementation so implementation-agent tokens are spent primarily on implementation rather than product-design rediscovery.

On 2026-10-26 the user/Overall Manager accepted D-064: on the Enlarged80 stand, Scenario One Objective 1 plans extraction-rack geometry on the stand map and communicates the plan through physical entrance-tree marking in the forest. It is a specific exception and supersession for rack geometry only; it does not generally move spatial forestry decisions onto maps. The feature specification is `specs/scenario-one-objective-1-rack-planning-field-marking-v2/`.

## Decisions

| ID | Date | Topic | Status | Decision or position | Implication / notes |
|---|---|---|---|---|---|
| D-001 | 2026 | Engine | Confirmed | Use Unity. | Current project is implemented in Unity. |
| D-002 | 2026 | Core concept | Confirmed | Build a 3D sustainable forestry / land-management game with physical presence. | Forestry is the first reference domain; long-term scope may broaden. |
| D-003 | 2026 | Forestry approach | Confirmed | Continuous-cover forestry is central to Scenario One. | Maintain cover while creating regeneration, structural diversity and timber outcomes. |
| D-004 | 2026 | Player experience | Confirmed | **decide → advance time → walk landscape → observe/experience consequences → understand why → decide again**. | Consequences must be visible in 3D, not only reports. |
| D-005 | 2026 | Reference scenario | Confirmed | Irish Sitka plantation conversion toward continuous-cover forestry. | Ireland is reference content, not an engine assumption. |
| D-006 | 2026 | Scenario sequencing | Confirmed | Complete Scenario One before broad scenario/biome generalisation. | Avoid premature regional abstraction. |
| D-007 | 2026-10-02 | Gameplay stages | Confirmed | Simulation-first progression: Stage 1 Forestry Management Simulation; Stage 2 Expanded Land-Management Simulation; Stage 3 Optional Manual / Hybrid Solo; Stage 4 Physical Cooperative World. | Broaden simulation/menu-resolved land management before introducing optional manual execution; cooperation is later. Supersedes H-004. |
| D-008 | 2026 | Stage 1 time | Confirmed | Discrete annual ecological steps. | Seasons/finer time may follow for physical survival play. |
| D-009 | 2026-10-02 | Work architecture | Confirmed | decision → task → labour/material/tool requirements → execution method → authoritative world result. | Contractor versus landowner is separate from simulated versus manual execution. Contractor simulation, landowner simulation, optional manual solo and later cooperative manual share authoritative ecology/construction results. |
| D-010 | 2026-10-02 | Work Plan | Confirmed | Review, cost, choose contractor or landowner, approve and resolve work through simulation; do not spatially design the forest. | Spatial decisions are made while walking. Landowner work can be menu-simulated; manual physical execution is not required in Stage 1. |
| D-011 | 2026 | Progression | Confirmed | No XP ladder as main progression. | Capability, costs, logistics, tools, processing and choices drive progression. |
| D-012 | 2026 | Tools/crafting | Confirmed | Recognisable tools; no primitive stone-age crafting ladder. | Keep survival/construction plausible. |
| D-013 | 2026 | Tree marks | Confirmed | Red Fell and Blue Crop Tree/future timber are mutually exclusive. | Persistence details belong in Overview. |
| D-014 | 2026 | Pruning | Confirmed | Designate Crop Trees; generate work for eligible trees rather than marking individual cuts. | Eligibility remains authoritative. |
| D-015 | 2026 | Planting | Confirmed | Purchased stock placed at the actual ground position chosen by the player. | Implementation/persistence belongs in Overview. |
| D-016 | 2026 | Planting clearance | Confirmed | Approximately 1 m² local clearance/vegetation-treatment area. | Research recommends spot vegetation control; later causal interpretation needs acceptance/implementation. |
| D-017 | 2026 | Felling outcomes | Confirmed | Sell/extract, retain fallen deadwood, or keep useful construction timber. | Inventory/accounting belongs in Overview. |
| D-018 | 2026 | Reference Future | Confirmed | V1 is a frozen authored v12 reference, not an optimum/forecast. | Do not silently rewrite archive/hashes. |
| D-019 | 2026 | Reference compatibility | Confirmed | Newer code may load/preview frozen v12 while current Scenario One has later schemas. | Distinguish historical archive from current behaviour. |
| D-020 | 2026 | Presentation authority | Confirmed | Habitat/art/audio derives from authoritative state; do not invent causal states. | Ring-barking/windthrow/defect consequences require matching simulation state. |
| D-021 | 2026 | UI | Confirmed | Add choices inside interfaces rather than permanent shortcut accumulation. | Prefer hotbar/tool/context patterns. |
| D-022 | 2026 | Multiplayer | Current direction | Single-player first; cooperation later. | Preserve reasonable boundaries without speculative networking. |
| D-023 | 2026 | Browsing | Current direction | Recruitment bottleneck through species/stage damage and suppressed height; shared response for cohorts and exact juveniles. | Browsing & Protection v1 integrated on main 2026-10-04 (Scenario One background pressure 0.2, shelters). Browse history/form damage and fencing gameplay are not implemented (D-044). |
| D-024 | 2026 | Understorey | Current direction | Scenario One bramble/bracken survival competition is accepted in D-049. Graminoids, botanical type differences and browse concealment remain open. | Do not extend the accepted Scenario One target to other sites without review. |
| D-025 | 2026-10-02 | Multi-agent | Confirmed | Dynamic OpenAI/Anthropic workers/reviewers; no permanent disciplinary ownership. | Assign by capability, tools, context, availability and quality. |
| D-026 | 2026-10-02 | Overall Manager | Confirmed | Currently ChatGPT Game Dev project; user may reassign. | Maintains direction, boundaries and handoff review. |
| D-027 | 2026-10-02 | Repository truth | Confirmed | Git canonical; Drive one-way collaboration mirror. | Permanent mirrors carry source/commit/timestamp stamps. |
| D-028 | 2026-10-02 | Worktrees | Confirmed | Separate writable local worktrees; no concurrent writes to one Unity worktree. | Serialized assets default single-writer. |
| D-029 | 2026-10-02 | Capability | Confirmed | Environment/tool access, not model identity, defines capability. | Chat, Desktop/MCP, Code/cloud and OpenAI sessions may differ. |
| D-030 | 2026-10-02 | Verification language | Confirmed | Branch completion differs from integration/verification/acceptance. | Use shared integration states. |
| D-031 | 2026 | Open source | Open | Release/community regional variants under consideration. | Code/data/media licences need separate treatment. |
| D-032 | 2026 | Binary storage | Open | Decide large first-party FBX/PNG/Blender policy, including Git LFS. | No `.gitattributes` at baseline; no storage-policy change authorised. |
| D-033 | 2026 | Survival intensity | Open | Decide food/shelter/health/weather pressure. | Support the management loop. |
| D-034 | 2026 | Multiplayer timing | Open | Decide when cooperation becomes an active milestone. | Do not build networking prematurely. |
| D-035 | 2026-10-02 | Context location | Confirmed | Canonical context in `Docs/Project/`; root `AGENTS.md`/`CLAUDE.md`. | Instruction sources in `instructions/`; research index in project docs. |
| D-036 | 2026-10-02 | VS Code | Confirmed | Keep tracked shared `.vscode/settings.json`; local differences use user settings/external `.code-workspace`. | No routine dirty worktree-specific settings. |
| D-037 | 2026-10-02 | Legacy instructions | Confirmed | Retire root `AI_Instructions` after porting valid rules. | Do not let old prototype guidance compete with current direction. |
| D-038 | 2026 | Creative reference | Confirmed | Valheim informs exploration, gathering, building, progression and cooperation, not a specification. | Preserve useful experience without incompatible mechanics. |
| D-039 | 2026-10-02 | Technical stack | Confirmed | Resolved: integrate. The save/RNG/batch/edge stack is in `main` (merge `8ae7a25`) and was verified together with the combined ecology package at `b1e6c51`. | Was Open (review/integrate, revise or park; frozen during setup). Independent review still required for future save/determinism/reference-sensitive work. |
| D-040 | 2026-10-02 | Context generation | Confirmed | One full Git SHA snapshots the complete context/instruction-source set; deterministic committed-source composites/mirrors; live locks excluded. | No dirty content under an old SHA; lock changes do not require reattachment. |
| D-041 | 2026-10-02 | Scenario One property | Confirmed | Scenario One uses a genuinely bounded property. | Retains the Irish forestry reference scenario. |
| D-042 | 2026-10-02 | Reference Future v1 contract | Confirmed | Reference Future v1 is a frozen, immutable historical archive. Verification covers archive integrity against original embedded historical data, load/preview of frozen milestones, and deterministic continuation of historical saves using the current ecology. Exact reproduction of historical biology by later ecology versions is not required, and no versioned historical ecology engine is retained. | Integrity must not be tested by reserializing historical saves through current save classes. Continued biology may diverge diagnostically. A reference for a new model would be a new version, not an edit to v1. Detail: `Docs/ReferenceFutureContract.md`. |
| D-043 | 2026-10-04 | Scenario 1 status | Confirmed | Scenario 1 is **functionally complete**: integrated on main at `dc975e4` with save v15, browsing default 0.2, shelters, contractor/landowner execution, timber yield/economy, completion verification, and the Plantation02 Sitka, forest-floor and compact-brash presentation. Automated completion gate, 27/27 integration regression, post-merge gates and the user's interactive smoke all passed. | Historical baseline: final presentation acceptance subsequently passed and was integrated at `ec938b0` (D-045); Astra ForestFloorV1 was not adopted. This earlier functional-completion record does not certify current external-test readiness. Anchors: neutral `BFC55473C1506067`, Scenario One `3485B6630C9EA448`, completion `568922E1A6D73CDD`, Reference Y100 `7AD177B3CC2F73C7`. |
| D-044 | 2026-10-04 | Scenario 1 deferrals | Deferred | Outside the Scenario 1 minimum unless accepted elsewhere: deer fencing production feature, production understorey ecology, vegetation-control simulation expansion, storm activation/rendering acceptance (dormant core accepted in D-050), adult suppression mortality, browse-history/form-damage persistence, fence deterioration, pruning-quality premium, broadleaf market, manual forestry, co-op. | Each needs its own accepted packet. The v16 browse-history/form-damage document is a proposal only. |
| D-045 | 2026-10-04 | Scenario 1 presentation | Confirmed | Final presentation acceptance PASS; the presentation baseline is integrated on main at `ec938b0` (ten-state rendered review, track texture, HUD readability). ForestFloorV1 is not adopted. | RNG new-game policy was subsequently resolved by D-046 (model 1). Regeneration, growth and dormant storm follow-ups were accepted in D-047–D-050; other deferred work remains open. |
| D-046 | 2026-10-05 | RNG model policy | Confirmed | New Scenario One games start on RNG model 1 (`ScenarioOneManager.Awake`). Loads keep the model recorded in the save. A save without the field (absent → JsonUtility default) and Reference Future v1 stay on model 0. Integrated on main at `4eab590` (fast-forward); post-merge gates PASS. | No survival/calibration change and no schema bump. Labelled anchors: **model 0 (compatibility)** neutral `BFC55473C1506067`, normal `3485B6630C9EA448`, completion `568922E1A6D73CDD`, Reference Y100 `7AD177B3CC2F73C7`, continuation `9CDF21A541C5968D`; **model 1 (new-game default)** neutral `2A0B8C32AC0DE113`, normal `506E8AF6D8514C6C`, completion `00479F18970F9926` (completed Year 25, minimum cash €5,897.70). Detail: `Docs/Scenario1RngModelCorrection.md`. |
| D-047 | 2026-10-06 | Regeneration Model 1 | Confirmed | Save schema v16 adds `regenerationModel`. New Scenario One games use model 1 (age bands); v1–v15 saves, a missing field and Reference Future v1 are model 0, with no automatic migration. Model 1: juvenile abundance stays an abstract relative abundance (occupancy, not stems/ha); bands keyed by species + origin + establishment year, at most 4 represented bands per species + origin + cell plus one sub-threshold accumulator; 0.01 is a representation threshold for the species + origin population (sub-threshold recruitment accumulates, cannot promote, crosses into an ordinary band without creating a tree); no seed-independent infill; new recruits start at age 0 and initial height; natural/planted origins separate (origin defect fixed); capacity never shrinks existing stock and full rejection creates nothing; the oldest qualifying represented band promotes to one exact tree as an abstract handoff, younger bands remain. Integrated on main at `4670e73`. | Anchors: model 1 lifecycle neutral `962846D2F517B293`, normal `FBB8F470D85FF815`, completion `6F84AF319D301F87`; legacy model 0 unchanged (see Unity Project Overview). **Open/deferred:** physical juvenile stems/ha interpretation undefined; understorey cover has no causal regeneration effect; the 0.01 establishment-response cut-off remains an existing calibration rule; promotion remains an abstract band → individual handoff. Detail: `Docs/Research/RegenerationBudget/RegenerationModel1.md`. |
| D-048 | 2026-10-07 | Scenario One growth model 1 | Confirmed | Scenario One represents Irish Sitka site **Class III (average)**; the authored starting stand is kept. Save schema **v17** adds `growthModel`: missing field, v ≤ 16 and explicit 0 mean legacy growth; new Scenario One games use 1; Reference Future v1 stays 0; no automatic migration. Growth model 1: Sitka height follows the Class III site-potential curve `H = 34.362 × (1 − e^(−0.042 age))^1.563`, with each tree keeping its relative height and no competition term in height. DBH and local Hegyi competition are unchanged. A separate British-Sitka SDI stand-occupancy signal (`SDI = N/ha × (Dq/25)^2.063`, `RD = SDI/1868`) drives self-thinning biased toward suppressed individuals (`min(0.5, 0.08 × P² × S⁴/mean(S⁴))`, `P = max(0, (RD − 0.6)/0.4)`, with the RD 1 maximum line as a boundary). Biological self-thinning (cause `self-thinning`, death year) creates one existing fallen-deadwood record per tree. No storm model. Integrated on main at `2cfbabc`. | Evidence labels: **[A]** the Irish age-30 Class III top-height anchor 20.4 m (Lekwadi et al. 2012; b0 rescaled to it); other ages are modelled curve values. **[B]** the British Sitka density–size relationship (Comeau et al. 2010: slope −2.063, maximum SDI 1,868, Dq 25 cm), transfer evidence with regional differences reported; not Irish validation. **[C]** onset RD 0.6, strength 0.08 and suppression exponent 4 are calibration choices (onsets 0.5/0.6/0.7 tested; S² insufficiently selective). Anchors: growth model 1 lifecycle neutral `7E57B9DAEF5BF4D0`, normal `35E2BF1C2F55C2DE`, completion `D7C4DDD36B53FCCE`; legacy and regeneration-model-1 anchors retained under growth 0. **Open:** realised top height 6–9 % below the Class III curve at older ages; mortality calibration; 0.5 stem form factor unvalidated; Forest Yield cross-validation; Irish maximum-density data; recruit mortality pulses under a full canopy. Detail: `Docs/Research/SitkaGrowthMortality/SitkaGrowthModel1.md`. |
| D-049 | 2026-10-08 | Regeneration Model 2 — understorey competition | Confirmed | Manager accepted Model 2 v1 calibration at source `902903f` (implementation `49b01d8fc90ea0779ebfdc61a5a421af718da1d6`). Save18; new Scenario One games use regeneration2, v17 model1 remains1, Reference Future remains0. Exactly five persistent fields: independent cell bramble/bracken covers and independent local-patch covers/update year. Shared Sitka/Oak/Beech juvenile survival loss = `.35 × max(bramble,bracken) × clamp01((1.5 − pre-growth height)/(1.5 − .3))`; full at≤.3m and zero at≥1.5m [C]. Initial cover `.25 × target` [C]; shared target `.8 × clamp01(siteProductivity × soilStability) × (.1 + .9 × clamp01(light))` [C/I] (site coupling [I], .8 cap [C], .1 shade floor [C/I], .9 scaling [C]); bounded annual recovery gain .30/loss .45 [C/D]. Work → canopy → recovery → vegetation/light/browse survival → establishment/promotion; separate cause accounts retained. Clearance can help, waste money or remove useful recruitment; targeted treatment can outperform blanket clearance. Total habitat cover remains non-causal. | Gameplay calibration, not measured Irish mortality/recovery or a validated botanical productivity equation. Equal category trajectories are an acknowledged abstraction, not equal botanical abundance. No automatic target generalization beyond Scenario One; reassess drivers for site/soil/class/climate diversity. Open: independent type responses, bramble concealment, graminoids, neighbor spread, site/moisture calibration, visual recognition and human clearance playtest. No fixed treatment interval or presumed timber profit. Beech .6m display correction accepted; biology unchanged. Model2 anchors `FA855239CDDA32D8` / `02334804F65C0234`, completion `702766DECE591E21`; historical and frozen anchors retained. Clean integration verification and publication: `Docs/Verification/RegenerationModel2Integration/IntegrationHandoff.md`. |
| D-050 | 2026-10-08 | Storms & Windthrow Model 1 | Confirmed | Manager approved complete source `857150b2f20afa52d7b36be88a835f07a4ee5a51` for integration. Save 19 adds only root `stormModel` and resolved events `{year,severity,directionDegrees,cropTreesLost}`. StormModel 0 preserves no-storm history; existing/missing-field saves, Reference Future v1 and new Scenario One games remain 0 (new stack RNG 1/regeneration 2/growth 1/storm 0). StormModel 1 has one uprooted/fallen outcome, ID-keyed deterministic failure, causal mortality/deadwood/recent opening, one batched canopy and seed rebuild, and ordinary Model2 interaction through shared light/state. Default is leave deadwood; optional individual/partial SellAndExtract or KeepForUse reuses grouped harvest settlement, multiplier 1.00, unchanged prices and contractor minimum. | **Frozen StormModel 1 definition [C]:** annual occurrence .02, severities .02/.06/.18, weights 1:1:1, ReducedProposal, BoundedRational, neutral site 1 and neutral/absent external edge. These are gameplay calibration, not measured Irish hazard or engineering constants. Material recalibration requires StormModel 2 or separately approved persisted-profile/version architecture. Dormant integration does not activate storms or certify player readiness. Crown/root art, century combined rendering, standalone Player profiling, activation/default policy, full storm lessons, directional outside-landscape exposure and snapped stems/snags remain open. Post-integration results and exact publication identity: `Docs/Verification/StormsWindthrowIntegration/IntegrationHandoff.md`. |

| D-051 | 2026-10-09 | Forestry-first roadmap | Confirmed | Scenario One is the forestry reference foundation, not the endpoint. Develop Stage 1 into a strong structured multi-scenario CCF simulator before broad Stage 2 expansion. | This accepts direction, not Scenario Two implementation, infrastructure, or a research-proposed exact curriculum. |
| D-052 | 2026-10-09 | Independent forestry scenario progression / sandbox later | Confirmed | Forestry scenarios are independent forests; player knowledge and experience progress between them. Custom sandbox comes later. | No fixed six-scenario/two-branch curriculum or specific progression mechanics are accepted here. |
| D-053 | 2026-10-09 | Primary forestry audience | Confirmed | The primary reference audience is smaller private woodland/farm-forest owners and managers rather than industrial estates. | Frame management choices, explanations and logistics for this audience; retain credible trade-offs. |
| D-054 | 2026-10-09 | Fruit/nut first Stage-2 slice | Confirmed | After forestry maturity, fruit and nut trees are the first intended Stage 2 slice. | No Stage 2 implementation or detailed mechanics are authorised by this direction update. |
| D-055 | 2026-10-10 | Unavailable construction frames hidden | Confirmed | Unavailable building/construction frames are hidden. A construction frame is shown only when its corresponding structure can actually be built or placed. CCF has no player building options in the current game, so every such frame is hidden. Implemented and verified at `fac782072f23e28518e8840c621112fe4074db45`: `ForestBuildable.constructionAvailable` defaults off for all seven legacy sites (Workbench, Basic Shelter, Saw Pit, two Log Racks, Grinding Stone, Timber Sledge), which hides the frame, disables the site's solid colliders, removes the `[E] Build` prompt and makes `TryBuild` do nothing; structures already built in older saves still appear; the Plank Rack's static foundation marker and collider are switched off by two single-line scene edits. | Presentation/affordance policy, not authorisation to implement construction. Before the change all seven sites offered a live `[E] Build` action that spent carried, stored or retained timber, so the earlier premise that no building option existed was incorrect; the user chose to hide frames and disable the action. Source prefabs, built visuals and build code are retained, `ScenarioOneInteractionVerification` keeps the latent mechanic covered, and a site's flag is switched on only when its structure is genuinely buildable. Retained construction timber has no construction use for now. No save, ecology, economy, objective or Reference Future change. |
| D-056 | 2026-10-10 | Scenario One stand ≥ 80 × 80 m before pilot | Confirmed; implemented and verified | Scenario One's 40 × 40 m stand is retained as the historical development / prototype scale ("Legacy40"). New Scenario One games now start in an 80 × 80 m playable property ("Enlarged80"), integrated on main at `a309f99e0f2c6a770044604bbf88ee2e2471a5b5` (Phase-1 audit `9f49e2e`, 80A `b024633`, 80B `a309f99`) and verified there: property 0.64 ha; ecology grid 16 × 16 = 256 cells at 5 m; 1,344 starting trees at ≈ 2,100 stems/ha (age 20); the central 336 legacy tree IDs and exact positions are retained and 1,008 outer trees have deterministic IDs `PO{row}{col}`. Save v20 adds an explicit `standGeometryModel` (0 = Legacy40, 1 = Enlarged80); saves ≤ v19 or without the field load as Legacy40; new games use Enlarged80; Reference Future v1 remains the immutable Legacy40 archive. Historical Legacy40 hashes remain compatibility anchors; Enlarged80 has separately labelled current anchors (start `04A78188A6F8E6F9`, year 3 `081DB58CE3034648`). A bounded denser / higher-Hegyi band just inside the former ±20 m edge (61 preserved legacy clamp-line trees) is accepted as a consequence of exact core-position preservation. No Player or native-Windows performance claim is made. | Implements the accepted product direction. Economy and objective values were deliberately not recalibrated; several absolute count objectives are proportionally much easier in the enlarged stand (`Docs/Verification/StandExpansion/Enlarged80Calibration.md`), so area-sensitive calibration was required before the pilot; it is now implemented and verified under D-057. The geometry is applied at runtime (`StandWorldBoundary`); the scene was not re-authored. In Enlarged80 there is no geometry-compatible Reference Future, so the Century Review compares with the aspirational design targets. Editor measurements only: annual step about 9× and rendered frame about +70 % against Legacy40. Evidence: `Docs/Verification/StandExpansion/` and `Docs/Verification/Scenario80mIntegration/`. |
| D-057 | 2026-10-10 | Enlarged80 area-sensitive objective calibration | Confirmed; implemented and verified | Integrated on main at `23f84f9ac0d4a02d385869d3985e11a24cc5a23f` (production implementation `fa9f66dfed1b7f8a24295f379e2bb9e302e84a66`). Extensive effective targets are deliberately 4× for Enlarged80's 4× property area: Legacy40 retained originals 60, regeneration cells 3, deadwood 0.02 m³, century original trees 120, century broadleaf presence 10 per planted species, century regeneration cells 12, century deadwood 0.5 m³; Enlarged80 240, 12, 0.08 m³, 480, 40 per planted species, 48, 2.0 m³ respectively. Gameplay continuity calibration, not empirical forestry thresholds. | Serialized ScenarioOne.asset remains the Legacy40 baseline; geometry-aware effective-target accessors provide current values and reject unknown geometry. Unchanged: completion canopy 0.35, aspirational canopy 0.65, Year 25 completion horizon, Year 100 century review, €12,000 starting cash, €2,500 annual commissioned-harvest minimum, 2,400 owner minutes/year and unit labour/material/stock/timber values. Save remains v20, definition scenario-one-v13, Reference Future v1 immutable Legacy40. Restored pre-calibration Enlarged80 aspirational reviews refresh targets/achieved flags without changing measured history; Legacy40 and genuine frozen-reference cached reviews are unchanged. Century copy distinguishes aspirational targets (not a forecast, optimum or prescription) from a frozen Reference comparison. Viability witness completes in Year 25 and remains solvent (minimum €11,451.16). Future geometry requires a fresh calibration decision. Stage A remains NOT READY; minimal startup/title is now integrated under D-058; malformed cell-index save hardening is integrated and verified at `12cc8ef0659f54055088047c2c6727d874bdffd8` (Overview); next is standalone Player performance/build profiling. |
| D-058 | 2026-10-11 | Minimal Scenario One startup/title screen | Confirmed; implemented and Editor-verified | Integrated unchanged on main at `b99ba6993b6b7cfb9fc3b8de6c4eda88abd8a348` (eight approved commits from `e8c73f25e6d93db818abdc6c794887f8f7c5ba43`). Standalone/private-test Player startup presents a title layer before forest control: “CCF”, “Scenario One”, “Sitka Plantation to Continuous-Cover Forest”; actions Start New Scenario, Continue, Controls / Help and Quit; private-test build identity displayed. Existing StandaloneSessionMenu architecture reused; F10 remains the fuller in-game session menu. | Start New confirms when an existing save is present and does not delete it. Continue is offered only when existing save validation accepts the save; CanLoad is read-only and shares the loader’s pre-mutation validation. Editor Play Mode remains direct-entry by default; `CCF_STARTUP_SCREEN=1` is an Editor-only opt-in. Save remains v20; calibration and model identities unchanged. Actual Linux Editor mouse/keyboard input verified; synthetic graphical InputSystem delivery remains explicitly unverified. Native built-Player / Windows startup remains UNVERIFIED. Stage A NOT READY; malformed cell-index save hardening is integrated and verified at `12cc8ef0659f54055088047c2c6727d874bdffd8` (Overview); next is standalone Player performance/build profiling. |
| D-059 | 2026-10-12 | Three-species parity before Scenario One demo | Confirmed | Before the first external/playable Scenario One demo, Sitka spruce, Oak and Beech must have comparable modelling depth and species-specific fidelity. **Comparable depth does not mean equal empirical precision.** Use empirical species-specific evidence where available; labelled British/European transfer where appropriate; ordinal traits where quantitative evidence is insufficient; explicit calibration where necessary. **Do not manufacture empirical precision.** Oak and Beech must not silently inherit materially inappropriate Sitka-specific assumptions. | Establishes the accepted product requirement the species-parity audit was performed against. Evidence-boundary rule for every species correction: Irish evidence where available; British/European transfer where appropriate and labelled; ordinal traits where quantitative evidence is insufficient; gameplay calibration only where necessary and explicitly identified. No pseudo-empirical coefficients. Each species must read as a genuine biological representation, not Sitka relabelled with another name. Recorded code findings that motivate this requirement are in the Unity Project Overview. This decision authorises no production change by itself; implementation follows its own packets. |
| D-060 | 2026-10-12 | First external demo contains the complete Scenario One | Confirmed | The first external Scenario One demo contains the **complete** Scenario One, not a first-intervention slice. Before that demo the player must be able to: start from the initial forest; assess it; make an intervention; advance time; observe consequences; understand and review previous management; make second and later interventions; repeat the management cycle; and reach Scenario One completion. By scenario completion the player should have learned the basic CCF management loop through repeated intervention and adaptation. **S1-C (review/history) and S1-D (second and subsequent intervention support) are therefore PRE-DEMO requirements.** Incomplete internal technical builds remain useful for engineering tests. | **Supersedes** the previous first-intervention external Stage-A boundary recorded in Current Milestone ("Stage A — first-cycle pilot" and "S1-C and S1-D are not prerequisites for the Stage-A first-cycle pilot"). The replacement pre-demo sequence is in Current Milestone. B07 is an **internal technical baseline** and is not the eventual external-demo candidate. Internal engineering builds are not withheld from engineering use; they are simply not external-demo candidates. |
| D-061 | 2026-10-12 | Mixed-species density representation v1 | Confirmed | Mixed-stand density is represented by **ordinal species-aware growing-space-demand classes**. Requirements: (1) 100 % Sitka composition must route through the existing exact calibrated Sitka density implementation; (2) existing Sitka arithmetic must remain unchanged; (3) Oak/Beech growing-space classes must be explicitly represented as **ordinal/transferred modelling, not empirical Irish maximum-density equations**; (4) no DEC; (5) no DMC; (6) no mixture-productivity bonus; (7) do not silently re-baseline Sitka anchors. | **Rejected for P0:** simply excluding Oak/Beech from density effects; and invented Oak/Beech SDI / maximum-density equations unsupported by evidence. Evidence boundary supporting this choice: no published Irish oak or beech maximum-density, SDI or self-thinning relationship was found; density-equivalence coefficients are unsupported for these species pairs; mixture over-density modification is conceptually known but quantitatively unsupported. Pure-Sitka preservation is an **acceptance criterion to be proven**, not an expectation. Any Sitka anchor movement is an explicit decision, never a silent re-baseline. |
| D-062 | 2026-10-12 | Oak/Beech timber/economy before Scenario One demo | Confirmed | **Reopens the broadleaf-market deferral in D-044.** Oak and Beech must have an appropriate timber assortment and economic layer before the Scenario One external demo. Requirements: do **not** simply reuse Sitka assortment boundaries or prices; use best available evidence; distinguish empirical evidence, transfer and calibration explicitly; use size, quality, form and management history in assortment eligibility and value where practical; avoid false precision. | **Supersedes in part** D-044 ("broadleaf market" deferral). Verified state at `c1564b85`: timber assortment specifications and roadside prices are Sitka-only; Oak and Beech are explicitly unmarketed and the player-facing inspection says so honestly. That is an honest absence rather than silent Sitka inheritance, but it is not comparable modelling depth for the demo. Assortment shape may be transferred and must be labelled; Irish broadleaf price series are not established, so any selected values require explicit calibration labels rather than an empirical claim. Value should emerge from size + form/defects + management history → assortment eligibility → market price, not from a species price multiplier. |
| D-063 | 2026-10-22 | Spec-driven development workflow | Confirmed | CCF uses spec-driven development selectively for substantial or ambiguous work. Small bounded work continues through ordinary AgentWorkflow packets. The Overall Manager classifies work as `PACKET ONLY` or `SPEC REQUIRED` before production implementation and may author a specification before routing implementation so implementation-agent tokens are spent primarily on implementation rather than product-design rediscovery. | Approved feature specs are task-level implementation authority for their scope, subordinate to latest user instruction, Decision Log, Current Milestone and Game Brief; implementation truth remains inspected repository/verified Unity state. Artifacts live under `specs/<feature>/` (`spec.md`, `plan.md`, `tasks.md`, `checklists/requirements.md`) with templates in `specs/_template/`. Completed/superseded specs are preserved as project history. |
| D-064 | 2026-10-26 | Scenario One Objective 1 rack planning on the map; entrance-tree field marking | Confirmed; implementation pending | On the Enlarged80 stand only (`standGeometryModel == Enlarged80`), Scenario One Objective 1 plans extraction-rack geometry on the stand map (create, select, move, rotate, delete, undo, reset, confirm) and the independent first-person freehand rack-creation route is removed for that objective. After the plan is confirmed the player physically sprays an orange mark (vertical stripes on four sides, one interaction) on the first rack-removal tree nearest the path at each rack entrance. The objective counts racks planned (X / 5) and then rack entrances marked (X / 5), both derived from one authoritative rack-plan object set. **Sequence:** Plan Racks → Confirm Rack Plan → pending rack-removal orders created or refreshed → mark rack entrances in 3D → `rack.approve` → execution → `rack.result`. Confirm creates pending work only: it does not approve and does not execute felling. Field marking occurs before approval. If a confirmed plan is edited before approval, the pending rack-removal orders are reconciled with the new geometry and not duplicated. No parallel rack-execution mechanism is created. **Legacy40** saves and Reference Future v1 are not migrated to the V2 curriculum, need not have five racks at 16–20 m, are not subject to the new spacing validation, and keep their existing viable progression path. | **Specific exception/supersession for rack geometry only:** this does not move spatial forestry decisions generally onto maps, and the Work Plan does not become a spatial-design tool. **Evidence labels:** 16–20 m is conditional early-transformation guidance in the CCF book (p. 40), used as the Scenario One recommended band and presented as recommended for this stand, never as a universal CCF rule [gameplay calibration informed by source guidance]; five racks and the ~18 m default layout are Scenario One gameplay calibration; orange is a Scenario One choice within the book's orange/pink removal family (pp. 84–85); **map-only planning and first-tree-only entrance marking are accepted game-design derivations that the book does not establish**, and the game must not claim otherwise. Racks are session-only today, so V2 needs new persisted rack state and a save-schema change that remains subject to the spec's backward-compatibility rules, D-042 and G5; this decision changes no code or save. **OPEN-3 accepted 2026-10-28 by direct user authority:** a rack with zero executed removals may be reopened/edited through the authoritative rack reconciliation/cancellation path; reopening itself cancels nothing; where editing would cancel approved-but-unexecuted work, the player receives a warning and explicit consent is required before cancellation/reconciliation; any rack with one or more executed removals is geometry-locked and represented as harvested; executed removals are never reversed; only racks whose geometry changes lose geometry-bound entrance-mark state, and unrelated racks retain theirs. Not implemented or verified; no Windows build. Spec: `specs/scenario-one-objective-1-rack-planning-field-marking-v2/` (APPROVED); evidence: `Docs/Research/CCFBook/CCF_Book_Definitive_Reference.md` and `Docs/Research/RacksAndTreeMarking/`. |
| D-065 | 2026-10-27 | Multi-scale forestry management V1: demonstrate in a sample, apply the sample treatment to comparable areas | Confirmed; implementation pending | Scenario One Enlarged80 gains a required representative-management step after the current Lesson 5 review (U1–U5, P1–P4 in the spec): the player demonstrates forestry judgement in the qualified 3D teaching sample; the simulator derives a transparent, closed-vocabulary treatment prescription from the executed sample decisions; the player reviews coverage and proposals; approving the prescription creates **pending ordinary work orders with prescription provenance**; the existing Work Plan remains the commitment gate. **Architecture:** three conceptual scopes (property/strategic deferred; stand/operational; sample/tree-level) but only two player-facing environments (Management/map and 3D forest); the existing tree/ecology-cell simulation stays the only authoritative biological state and stand values are derived summaries. **Player-facing framing** is applying the demonstrated sample treatment to comparable areas; `delegated marking` is internal shorthand only. **Crop Trees outside the sample:** the system may propose spacing-only candidates (eligible Sitka adults), each individually confirmed or rejected by the player, grounded in 3D inspection, with no bulk confirmation and no claim of tree quality. **Proposals are never `TreeMarkType.Fell`** and cannot enter the Work Plan before prescription approval; player-authored state always takes precedence. Suggestions use only present-state information the player can observe or inspect. Full manual management remains available. The sample and coverage geometry derive from persisted, confirmed Rack V2 geometry (D-064), not a parallel temporary model. V1 quantitative treatment is Sitka-adult only; Oak and Beech are explicitly excluded until species parity (D-059) supplies accepted behaviour. The slice must be implemented and qualified before beginner/external Scenario One testing, and may contribute to S1-D but satisfies it only when S1-D's own acceptance requirements are explicitly verified. | **Amends D-010 in part:** the rule that spatial decisions are made while walking is no longer read as universal; demonstrated 3D judgement may be scaled by a reviewed prescription. It does not remove physical 3D judgement and does not make the Work Plan a spatial-design tool. **D-064 is unchanged:** it stays a specific exception for rack geometry; D-065 does not move tree marking or Crop Tree designation onto maps, which only navigate to, summarise and show coverage of candidates and proposals. **Evidence labels:** the delegated-treatment evidence audit is complete; primary-source spot checks support marked sample/control areas, sample racks in Irish conifer first thinning, UK feller-select/example-area practice and post-work inspection as **supporting analogues**. The simulator's staged sequence is an **evidence-informed game abstraction**, not a documented standard Irish or UK workflow; `operator selection` is not an established Irish technical term, contractors are not claimed to be routine marking crews, and unrestricted operator crown-release discretion is not implied (UK evidence shows cab visibility can make crown assessment unsuitable). The 9.0 m teaching half-width (offline-qualified by MFM-01, live Unity confirmation pending), coverage tolerances and the dedicated candidate-spacing value are **gameplay calibration**; the 8 m teaching Crop Tree spacing and the 8 m Hegyi neighbourhood remain separate constants. The 30% teaching release minimum and 40% basal-area safeguard are protected. Production coding is gated by the spec's prerequisites (Rack V2 persistence integrated, next available save schema, inspected latest Scenario One lineage, no conflicting writer); wider-treatment calibration impact on canopy, economy and completion must be qualified before it becomes the default flow. Legacy40 and Reference Future v1 (D-042) are unaffected and old saves must not fabricate prescriptions. Not implemented or verified; no Windows build. Spec: `specs/multiscale-forestry-management-v1/` (APPROVED FOR IMPLEMENTATION PLANNING). |

## Superseded early directions

| ID | Topic | Status | Earlier position | Superseded by |
|---|---|---|---|---|
| H-001 | First prototype | Superseded | Small woodland, one tree type, harvesting/construction/regeneration/save-load. | Implemented project progressed beyond it; current scope is Milestone. |
| H-002 | Time progression | Superseded | Continuous acceleration/seasons/discrete time undecided. | D-008 annual Stage 1; later stages remain open. |
| H-003 | Forestry economics | Superseded in part | Budgets/timber prices undecided. | Scenario One contains economy; calibration remains adjustable. |
| H-004 | Gameplay stages | Superseded | Stage 1 Management Simulation; Stage 2 Hybrid Solo; Stage 3 physical cooperative world. | D-007 simulation-first four-stage roadmap, with expanded land-management simulation before optional manual execution. |

## New decision template

### D-XXX — Decision title

- **Date:**
- **Status:**
- **Decision:**
- **Reason:**
- **Implications:**
- **Supersedes:**
# Current Development Milestone

## Purpose

Define the current Scenario One milestone, its live completion boundary and the work that remains. Verified implementation detail belongs in `Docs/Project/unity-project-overview.md`; accepted product decisions belong in `Docs/Project/decision-log.md`; temporary ownership and execution locks belong in `Docs/Project/coordination/active-tasks.md`.

**Last reconciled:** 2026-10-29.

## Milestone

**Scenario One — Complete-Loop Three-Species Demo Readiness**

The first external demo is the **complete Scenario One** (D-060), not a first-intervention pilot. Before that demo, Sitka spruce, Oak and Beech must be represented to comparable modelling depth (D-059), and the player must be able to assess the stand, plan and execute management, review consequences, make second and later interventions, repeat the management cycle and reach understandable scenario completion.

No external-demo candidate exists yet. No Windows build is authorised before the user personally playtests the Scenario One candidate and explicitly authorises a Windows build.

## Current programme state

### Rack V2 / D-064 — implementation in progress

Rack V2 is the authoritative Enlarged80 access-planning workflow. Rack geometry is planned on the Stand Map; confirmed entrances are to be physically marked in the 3D forest; Legacy40 and frozen Reference Future v1 compatibility remain mandatory.

The Rack V2 core, map planning/editing/spacing/confirmation/minimap work and the Combined HUD layout repair are integrated. The layout repair was Manager-approved and integrated through PR #80 / merge `b118d8e63f60d54c9f9a3bed6de40af83b35efc1` after exact-candidate Unity qualification.

**RACK-V2-08 — remove competing first-person rack planning and make Stand Map authoritative** is now Manager-approved, feature-qualified and integrated through PR #89 / merge `d925e9591f2fffb9419881a0e77031cefccf9aeb`. Its exact Unity-qualified candidate `0904f0f` passed the required model-0 and Rack V2 Guided/Sequential/Combined-HUD gates, including all seven previously-unwaived real-input assertions; Legacy40 and RACK-V2-07 preservation also passed. No Windows build was made.

The binding next Rack task is **RACK-V2-09 — physically mark confirmed rack entrance trees in orange** (claim #90). Its RACK-V2-07/RACK-V2-08 functional prerequisites are satisfied and the task packet is published, but execution is currently **storage-blocked**: the latest Unity handoff reported `/media/jer/ZX20` at approximately 11.5 GiB free, below the 15 GiB machine Unity-lock floor. No Unity task may launch until a fresh preflight proves that floor is restored.

Later Rack V2 help/teaching, persistence and full functional/rendered qualification tasks remain pending. Rack V2 is therefore **not yet complete**. MFM production remains gated by the Rack persistence/integration boundary in D-065.

### Sitka/Oak/Beech parity P0 — underway

Three-species parity work has started and several bounded slices are already integrated and/or Unity-qualified. The old statement that parity P0 had not started is superseded.

Current accepted state includes:
- **PARITY-03 — mixed-density parity:** Manager-approved and Unity-qualified; independent static review accepted. Q11 documentation/evidence-record repair is complete. Q10 remains a non-blocking evidence-strengthening follow-up and does not invalidate the accepted qualification.
- **PARITY-04 — adult Oak/Beech parity:** bounded documentation/verification alignment is integrated and focused adult Unity qualification is Manager-approved.
- **PARITY-05 — early-life growth and browsing parity:** implementation/docs/verification patch is Manager-reviewed and integrated through PR #69; focused Unity qualification remains pending under claim #70.

Other approved parity tasks remain before the whole three-species P0 can be marked complete. Comparable modelling depth does not imply equal empirical precision. Accepted evidence classes and limitations in `specs/scenario-one-three-species-parity-p0/` remain binding; unsupported precision must not be invented.

### S1-C — review/history for repeated intervention — underway

S1-C is pre-demo scope under D-060 and is no longer unimplemented.

- T-01 per-year query/history foundation: integrated.
- T-02 Annual Review year browsing/query wiring: integrated and qualified.
- **T-03 intervention drill-down and money honesty:** static implementation and offline qualification are Manager-accepted, but the change is **not integrated, not Unity-verified and not accepted as a complete slice**. Rendered Tier-2 qualification remains under claim #19 and waits behind the Rack critical path unless the Manager explicitly reroutes the Unity lane.
- Later S1-C tasks remain subject to the approved spec and dependencies.

Grouped harvest money remains authoritative at the annual settlement level; successful non-harvest work may show its own recorded contractor/material/owner-time values. Historical browsing must remain read-only with respect to current game state.

### S1-D — repeated interventions — underway

S1-D is pre-demo scope under D-060 and is no longer unimplemented.

- T-00 product decisions: complete.
- T-01 repeated-intervention baseline/assumption re-verification: Manager-accepted and integrated.
- T-02 advisory cycle/readiness state: Manager-accepted bounded slice and integrated; its new script/meta still needs real Unity import/compile observation during a later applicable S1-C/S1-D Unity gate.
- T-03 teaching-footprint-cap decoupling, T-04 guide monotonicity and T-05 previous-vs-proposed management remain.
- **T-06 management-event ID integrity:** static implementation is complete. Independent source review by GPT-5.6 Sol / OpenAI passed, and OpenAI gpt-6.1-sol independently reproduced the executable evidence on exact reviewed candidate `29bb6cb`: T-06 75 PASS + 7/7 mutants, N3 88 PASS, T-02 93 PASS + 7/7 mutants, with focused method-body negative-control evidence. The independent review gate is complete. T-06 is **not yet Unity-verified, integrated or accepted as a complete slice**; claim #88 owns its queued runtime save-load/no-mutation qualification.
- T-07 persisted cycle-state expansion is cancelled for V1.
- T-08 multi-cycle witness remains blocked until the required implementation tasks are complete; T-09 independent spec review follows T-08.

The machine storage-floor blocker currently prevents T-06's Unity qualification as well as other queued Unity work.

### Multi-Scale Forestry Management V1 / D-065 — planning approved, production gated

Multi-Scale Forestry Management V1 is required before beginner/external Scenario One testing. Its approved product rule remains: demonstrated sample treatment may be applied to comparable rack-served areas; proposals are not Fell marks; player judgement remains required; Work Plan approval is the commitment gate.

MFM-02 rack-served sample/coverage geometry is integrated. **MFM-03 — measure and calibrate representative-management coverage** remains authorised measurement work, but claim #39 is waiting behind the current Rack critical path for any Unity execution. Production coding for the management workflow remains gated by Rack V2 persistence/integration as specified by D-065.

## Pre-demo sequence

The sequence is dependency-driven rather than a licence to bypass live ownership. Current intent is:

1. Finish the Rack V2 critical path required by D-064 and the MFM dependency.
2. Complete and qualify the remaining Sitka/Oak/Beech parity P0 slices.
3. Complete S1-C review/history work.
4. Complete S1-D second-and-later-intervention support.
5. Complete and qualify Multi-Scale Forestry Management V1 before beginner/external testing.
6. Prove Scenario One completion is reachable and understandable across the complete loop.
7. Run Tier-3 full integrated Scenario One qualification.
8. User personally playtests the qualified Scenario One candidate.
9. Only after explicit user authorisation, create the fresh post-parity/full-loop Windows candidate.
10. Independent review plus native Windows technical smoke, then beginner external testing of the complete Scenario One.

The live Unity execution order is controlled by `Docs/Project/coordination/active-tasks.md` and Manager release, not by this numbered list alone.

## Completion checks

Before the first external Scenario One demo:

- the player can start from the initial forest, assess it, make an intervention, advance time, review consequences, make second and later interventions, adapt, repeat and reach scenario completion;
- Sitka spruce, Oak and Beech have comparable modelling depth with evidence quality labelled honestly;
- pure-Sitka protected arithmetic and anchors remain exact where required by D-061 and the parity spec;
- Oak and Beech do not silently inherit materially inappropriate Sitka-specific assumptions;
- mixed density uses the accepted ordinal species-aware growing-space architecture with no DEC, DMC or mixture-productivity bonus;
- Oak/Beech timber/economy follows the accepted parity evidence and does not fabricate form/site precision;
- Rack V2 uses Stand Map planning for Enlarged80, preserves compatibility and provides the approved physical entrance-marking behaviour;
- Multi-Scale Forestry Management V1 preserves player judgement and Work Plan commitment semantics;
- S1-C/S1-D history, advisory state and repeated-intervention behaviour are coherent across multiple cycles;
- malformed/ambiguous save data required by approved hardening tasks is rejected before mutation rather than silently repaired;
- frozen Reference Future v1 remains unchanged;
- full integrated qualification is green before a release candidate is considered;
- no Windows build occurs before the user's personal playtest and explicit authorisation.

## Compatibility and evidence constraints

- Preserve Legacy40 compatibility where the governing task/spec requires it.
- Preserve frozen Reference Future v1 bytes/semantics; do not resize, regenerate, migrate or reinterpret it.
- Preserve unrelated dirty work; one writer per owned subsystem/file set.
- Do not weaken verification merely to obtain a green result.
- Distinguish IMPLEMENTED, REVIEWED, INTEGRATED, VERIFIED and ACCEPTED.
- Use the authoritative evidence classes defined by the approved parity/evidence work. Game abstractions and gameplay calibrations must not be presented as measured Irish/UK forestry practice.
- Research guidance becomes a mechanic only when accepted by Decision Log/spec.

## Build candidate policy

Historical B06 evidence is not an external candidate. Earlier pre-parity or pre-full-loop builds are engineering evidence only. The eventual external-demo Windows candidate must be created fresh from the qualified complete-loop lineage after the user playtest/authorisation gate.

Player/native-Windows text, input and performance remain unqualified until their explicit gates run. Synthetic Editor input evidence does not substitute for native Windows smoke.

## Approved specs currently governing milestone work

- `specs/scenario-one-objective-1-rack-planning-field-marking-v2/` — Rack V2 / D-064.
- `specs/scenario-one-three-species-parity-p0/` — three-species parity / D-059, D-061, D-062.
- `specs/scenario-one-review-history-v1/` — S1-C review/history.
- `specs/scenario-one-repeated-interventions-v1/` — S1-D repeated interventions.
- `specs/multiscale-forestry-management-v1/` — MFM V1 / D-065; production coding remains gated as specified.

Only APPROVED / IMPLEMENTING / VERIFYING / VERIFIED specs authorise implementation. DRAFT/SUPERSEDED specs do not.

## Explicit exclusions

No Scenario Two implementation, sandbox, multi-scenario product expansion, unapproved yield equations, mixture-productivity bonus, stocking reduction, Reference Future modification, API/pay-as-you-go enablement, or unauthorised Windows build is part of this milestone. Deferred systems do not become scope merely because underlying infrastructure exists.

## Live coordination

`Docs/Project/coordination/active-tasks.md` is the live ownership/lock authority outside the context snapshot. Workers must refresh it and their task claim before substantial edits and before acquiring the machine-wide Unity lock. GitHub state alone does not prove local cleanliness or Unity/runtime state.
# CCF Game Brief

## Purpose

This file defines the long-term vision and intended player experience for CCF. It should change only when the direction of the game changes.

Implementation facts belong in the Unity Project Overview. Current work belongs in the Current Milestone. Accepted, proposed and unresolved decisions belong in the Decision Log.

## Core concept

CCF is a 3D sustainable forestry and land-management game in which the player makes management decisions, advances ecological time, and then physically walks through the landscape to experience the consequences.

The central loop is:

**decide → advance time → walk the landscape → observe and experience consequences → understand why → decide again**

Forestry is the first and most developed land-management system. Continuous-cover forestry is central to Scenario One: the player should be able to harvest timber while maintaining woodland cover, encourage or plant regeneration, manage competition and browsing, retain habitat structures, and gradually transform an even-aged plantation into a more structurally diverse forest.

The long-term game may expand beyond forestry into restoration, biodiversity, water, food production and other forms of land use. Scenario One is the forestry reference foundation, not the endpoint: Stage 1 should develop into a strong structured multi-scenario continuous-cover forestry simulator before broad Stage 2 expansion (D-051).

## Intended player experience

The player should feel physically present in the landscape rather than operating a spreadsheet with a 3D forest attached.

Management decisions should be visible and spatial. The player should walk the stand, inspect trees and regeneration, mark work, place planting locations or protection, and later return to see what those decisions produced.

Consequences should be:

- **visual** — canopy structure, regeneration, understorey, deadwood, tree form and built features change;
- **ecological** — recruitment, competition, browsing, habitat and resilience respond;
- **economic/logistical** — work, stock, contractor costs and material choices matter;
- **sensorial** — lighting, soundscape and atmosphere reinforce landscape change.

The player should be able to experience the same place 10, 20, 50 or 100 years later and recognise both intentional management and unintended consequences.

## Primary forestry audience

The primary reference audience is smaller private woodland and farm-forest owners/managers, rather than industrial estates (D-053). Management choices, explanations and economic/logistical framing should serve that audience without assuming a single correct prescription.

## Creative reference

Valheim remains a reference for exploration, gathering, building, progression and cooperative physical presence. It is an inspiration rather than a design specification. CCF should not inherit Valheim mechanics when they conflict with the land-management simulation, ecological credibility or the staged Scenario One direction.

## Scenario One — Irish reference implementation

Scenario One is the reference implementation: conversion of an even-aged Irish Sitka spruce plantation toward continuous-cover forestry.

Scenario One is a genuinely bounded property.

Ireland is the authored ecological and cultural context for this scenario, not a hard-coded assumption of the engine.

The project should keep a practical separation between core simulation/game systems, scenario/regional ecological data, and presentation/content packs. That separation should serve actual maintainability and future regional scenarios without creating speculative infrastructure.

Scenario One should establish a strong forestry foundation before broader scenario development. Forestry scenarios represent independent forests; the player carries knowledge and experience between them, rather than transferring the same forest or assuming shared inventories/economies. A custom sandbox comes later (D-052). Exact scenario count, curriculum, branching structure and progression mechanics require their own accepted scope.

## Staged gameplay direction

### Stage 1 — Forestry Management Simulation

The player inspects the forest, makes spatial management decisions, builds an annual Work Plan, chooses who does the work, resolves it through simulation, advances one ecological year, reviews the results, and walks the changed forest.

**inspect → mark/plan → choose who does work → resolve through simulation → advance time → ecology/economy update → review → walk**

Work may be assigned to a contractor or the landowner. Landowner execution can still be simulated through menus; manual physical execution is not required in Stage 1. Stage 1 is the current forestry-focused reference mode for Scenario One.

Stage 1 develops beyond Scenario One through structured independent forestry scenarios before broad Stage 2 expansion. This is accepted direction, not a claim that multi-scenario infrastructure is implemented.

### Stage 2 — Expanded Land-Management Simulation

After forestry maturity, fruit and nut trees are the first intended Stage 2 slice (D-054). Broaden the simulation before introducing manual execution. The wider direction includes:

- fruit and nut trees;
- ponds and water storage;
- biodiversity/habitat works;
- fencing, paths and infrastructure;
- earthworks and landscaping;
- timber construction;
- cob/earth construction where suitable;
- stone walls;
- broader regenerative/property-management systems.

These remain simulation/menu-resolved systems. Detailed Stage 2 mechanics belong in later accepted task scopes.

### Stage 3 — Optional Manual / Hybrid Solo

The player may optionally perform tasks physically that already exist in the authoritative simulation. Manual execution is an alternative execution method, not a second ecology/construction system.

### Stage 4 — Physical Cooperative World

A later cooperative mode may use a persistent shared world in which players physically perform the same authoritative forestry, construction and land-management tasks. Preserve reasonable future multiplayer options, but do not build multiplayer infrastructure before a milestone needs it.

## Management architecture principle

**Management decisions create tasks. How a task is executed is separate from what the task does to the world.**

**decision → task → labour/material/tool requirements → execution method → authoritative world result**

Possible execution methods are contractor simulation, landowner simulation, manual solo, and later cooperative manual. Choosing contractor versus landowner is separate from choosing simulated versus manual execution. All execution methods share the same authoritative world result rather than duplicating ecology or construction rules.

## Time

Stage 1 uses discrete annual ecological advances. Later stages may introduce seasons or finer time for survival, weather and physical work, but annual ecological processes should not be mechanically divided into quarters unless evidence or gameplay requires it.

Long time horizons are part of the experience. Advance through decades while retaining a comprehensible history of the stand.

## Survival and construction

Survival and construction support the land-management game rather than replacing it. Use recognisable tools and plausible materials rather than a primitive stone-age crafting ladder.

Future progression may include shelters/buildings, paths/signs/access infrastructure, suitable forestry residues or retained timber, fencing/tree protection, timber processing, food and other land uses.

Retained forest timber may feed construction, and smaller timber may have lower-grade uses. Cob/earth construction depends on suitable material, and stone may be used where available. Pond excavation and earthworks may generate potentially reusable earth or stone; do not assume all excavated soil is suitable clay.

Progression should come from capability, logistics, cost, maintenance and knowledge rather than a conventional XP ladder.

## Ecology and management philosophy

Use credible ecological knowledge, simplified where necessary for clear and enjoyable play. Distinguish measured/well-established knowledge, management guidance, evidence-derived inference, simulation abstraction and gameplay calibration.

Do not imply one universally correct forestry prescription. Create meaningful trade-offs between immediate timber/survival needs, recruitment, habitat/deadwood, understorey/browsing, long-term resilience, cost and labour.

Planting trees is not automatically ecologically beneficial. Site suitability and existing open habitats matter.

## Art, audio and authenticity

The long-term visual goal is a beautiful, immersive and readable forest. Art should make authoritative state visible rather than invent unsupported ecological state.

The Irish reference implementation may use first-party photography, field recordings and generated/authored assets. Environmental audio should communicate habitat/landscape change where credible source material is available.

## AI-assisted development

Use AI extensively for code, Unity, Blender, assets, research integration, testing and documentation. No model family permanently owns a discipline; assign tasks by capability, tools, context, availability and demonstrated quality. The user remains the product owner and final decision-maker.

## Open-source direction

Open-source release and community regional/property variants remain possibilities. If pursued, handle code/data and media-asset licensing separately so contributors know what may be reused.

## Design principles

- Make sustainability understandable through visible causes and consequences.
- Make forestry decisions spatial and physically legible.
- Experience delayed consequences rather than only reading scores.
- Preserve multiple credible management strategies.
- Separate ecological rules from presentation where that improves testing/maintenance.
- Prefer small playable improvements over speculative infrastructure.
- Build one excellent Irish reference scenario before generalising broadly.
- Verify actual player-facing behaviour, not only compilation.

[exit 0]

$ cat Docs/Project/unity-project-overview.md
# Unity Project Overview

## Purpose and baseline

Verified technical/implementation facts and local checking guidance, not design plans. Inspected repository/Unity state outranks planning for implementation questions.

Revision 5 baseline supplied by the Overall Manager:

- Snapshot date: 2026-10-02.
- Repository: `https://github.com/jermur788/CCF.git`.
- Branch: `main`.
- Exact implementation baseline: `0fc92ca989c0df7109e2507b2aab5712f0cf6077`.
- Inspect live worktree status before writes. Mutable branch positions may differ from this historical snapshot.

At the supplied baseline, forestry/survival branches matched main. The separately recorded frozen chain ahead of main is:

| Branch | Tip | Ahead | Summary |
|---|---|---:|---|
| save-hardening-and-growth-cache | 3b71e4b | 1 | Save/load hardening and growth readout |
| rng-versioning | 8aede39 | 2 | Opt-in model 1; legacy model 0 preserved |
| batch-recompute | 457c897 | 3 | Batched rebuilds/shared Hegyi forecast term |
| edge-bias-diagnostic | a31ec62 | 4 | Edge diagnostic |

These changes are not in the pinned baseline. Their integration/acceptance must not be inferred from a branch label. The migration packet freezes these exact commits; it authorises no stack modification or merge. Verify other worktrees live rather than treating this baseline as their current state.

Later state (verified 2026-10-02): all four stack commits are ancestors of `main` (via merge `8ae7a25`), and the combined ecology package is integrated on top — see **Integrated ecology package** below.

## Engine and environment

| Detail | Baseline value |
|---|---|
| Editor | Unity 6000.6.0f1 |
| Pipeline | URP 17.6.0 |
| Input System | 1.20.0 |
| Test Framework | 1.8.0 |
| AI Navigation | 2.0.14 |
| Unity MCP | com.kitwright.unity.mcp from GitHub |
| Serialization | Force Text |
| Metadata | Visible Meta Files |
| Development environment | Linux; VS Code |
| Player controller | Assets/ForestPrototype/ForestPlayer.cs |

The MCP dependency is an unpinned Git URL in `Packages/manifest.json`; the lock records `585ada203ff25b1fbea23821e159afa4a930d481`. There is no `.gitattributes` at baseline, so neither Git LFS nor a Unity YAML merge driver is configured.

## Project-context migration

At main baseline `0fc92ca…`, `Docs/Project/`, root AGENTS/CLAUDE and `Tools/ProjectContext/` did not exist; root `AI_Instructions` existed and was stale. Decision Log/Milestone define the accepted migration target, not an already-integrated baseline fact.

The migration was implemented on `task/project-context-migration` (tip `42db4e9`, based on the baseline SHA). It changes documentation/agent entry points and the stdlib Python generator only.

Later state (verified 2026-10-08): `42db4e9` is an ancestor of `main`, so the migrated context, entry points and generator are integrated. The generator has been run from clean context commits on `main` (outputs in ignored `Build/ProjectContext/`). After StormModel1 integration, all 15 permanent Drive mirror/composite files were refreshed and read back byte-for-byte at context `58d6b0b483198abcf0208713d9a759f89d90e318`; Game Dev instructions were saved/reopened against that composite and seven stamped canonical/manifest attachments were refreshed. The separate worker-worktree Unity smoke gates remain open; see Current Milestone.

## Shared VS Code settings

`.vscode/settings.json` is tracked shared configuration: exclusions/associations, nesting and default `CCF.slnx`, without worktree-specific paths. Keep it unchanged; use VS Code user settings or an external `.code-workspace` for local differences.

## Scenes and code

- `Assets/Scenes/ForestTest.unity`: reference forest/scenario scene.
- `Assets/Scenes/MixedSpeciesTest.unity`.
- `Assets/Scenes/ForestryAssetReview.unity`: walkable art/LOD review.
- `Assets/Scenes/SampleScene.unity`: exists, not the reference scene.

Most code is in `Assets/ForestPrototype/`:

| Area | Implementation |
|---|---|
| Individuals | ForestTree: ID/species/state, exclusive marks, pruning history |
| Ecology | ForestEcologyController: competition, canopy/light, growth, crowns, regeneration, seeds/establishment/promotion, diagnostics |
| Juvenile rules | JuvenileEcologyRules: shared juvenile survival/growth/promotion path for natural cohorts and exact planted juveniles |
| Mortality | ForestTree.ApplyMortality(cause, year): explicit biological death, separate from Fell() |
| Natural regeneration | Species-keyed, origin/history-aware cell cohorts |
| Exact planting | Scenario One individual juveniles until promotion |
| Management | ScenarioOneManager: work, economy/history, nursery/planting, treatment patches, retained timber, reference and Work Plan |
| Save/load | ForestSaveController / ForestSaveData |
| Understorey | Scenario One Regeneration Model 2 stores independent bramble/bracken cover and applies shared juvenile-survival loss; other habitat cover remains non-causal |
| Habitat/audio | Derived from ecology/understorey/deadwood/juveniles |
| Art | Editor material/LOD/prefab/review-scene tooling |

## Save and interactions

Baseline `ForestSaveData.CurrentVersion = 13`. The ecology package raised it to 14 (biological-mortality cause/year). Current integrated schema: `ForestSaveData.CurrentVersion = 20` (v20 adds the explicit `standGeometryModel`; see the Enlarged80 section); new Scenario One games use RNG model 1, regeneration model 2, growth model 1, storm model 0 and stand geometry Enlarged80. StormModel1 exists but is dormant by default. The schema includes the Model2 competitor fields and the D-050 storm fields; the historical Scenario 1 v15 execution/protection/economy fields remain. Definition: `scenario-one-v13`, display `Scenario One — Sitka Plantation to Continuous-Cover Forest`, execution `ManagementOnly`. Saves up to v19 and any save without `standGeometryModel` restore as Legacy40; atomic save hardening is in place.

V13 includes two-type marks, exact juveniles, clearance patches, retained construction timber, management/economy/history, understorey, deadwood, work orders and pruning history.

Marks are None/Fell/CropTree, exclusive. Trees store lifts/crown base/last year; Scenario One batches eligible Crop Tree pruning. Purchased Oak/Beech planting has ground-selected positions, markers, contractor orders, persistent individuals and promotion. Legacy cell/cohort planting remains for historical compatibility.

Felling outcomes: SellAndExtract, RetainAsFallenDeadwood, KeepForUse. Construction timber is tracked separately.

## Annual sequence

1. Competition.
2. Canopy/light.
3. Adult growth.
4. Crown relaxation.
5. Canopy/light refresh.
6. Regeneration growth.
7. Mast.
8. Seed rain.
9. Establishment.
10. Promotion.
11. Establishment suitability.
12. Opening decay.
13. Diagnostics.

At the Revision 5 baseline, browsing and researched three-group causal Understorey v1 were not authoritative systems. Browsing & Protection v1 is now integrated (see **Integrated Scenario 1**). Current Scenario One Regeneration Model 2 has independent bramble/bracken competitor state that causes juvenile survival loss. Other habitat cover remains non-causal; graminoids, concealment and richer botanical differences remain unimplemented.

## Reference Future v1

Historical reference, intentionally frozen after the v13 overhaul:

| Field | Value |
|---|---|
| Schedule | reference-future-v1 |
| Definition / save | scenario-one-v12 / v12 |
| Seed | 20260914 |
| Start | 336 Sitka; 2,100 stems/ha |
| Historical canonical lifecycle (pre-calibration) | 7E39B70A14959FAD — superseded for current ecology |
| Initial full world | A564039D9B7CE31D |
| Schedule hash | 56C8B99FA1E8DDD1 |
| Year 20 | F7DF7DAB53B6FD32 |
| Year 50 | D5E75D6D21D631AC |
| Year 100 | 7AD177B3CC2F73C7 |

Contract (D-042, `Docs/ReferenceFutureContract.md`): immutable archive; integrity checked on the original embedded JSON, never by reserialising through current save classes; frozen milestones load/preview under current code; Year-50 historical save continues deterministically under the current ecology and save version (v12 → v14 at the ecology integration, v12 → v15 since Scenario 1). Exact historical biology replay is not required; continuation divergence is diagnostic only. A new model gets a new reference version, not an edit to v1.

Schedule: `Assets/ForestPrototype/ScenarioOne/Resources/ScenarioOneReferenceScheduleV1.json`. Existing scenario/reference/art documentation is in `Assets/ForestPrototype/Docs/`, including `ScenarioOneReferenceFutureV1.md`; other verification/design documents are in root Docs.

## Integrated ecology package

Integrated on `main` 2026-10-02 by cherry-picking `1319c2c`, `2632de6` and `6dc6daf` onto context commit `cd239d7`, with no conflicts. The resulting gameplay integration head is `b1e6c51`. Its code tree is identical to `6dc6daf`; only main's newer canonical-context docs differ.

- **Shared juvenile ecology:** `JuvenileEcologyRules` is the single biological path for natural cohorts and exact planted juveniles. Legacy planted cohorts remain compatible, exact planted identity/position/provenance is kept, and promotion is deterministic.
- **Mortality foundation:** `ForestTree.ApplyMortality(cause, year)` sets `biologicallyDead` with cause and year. It is idempotent, excludes the tree from living systems, persists in save v14 and restores without mortality/harvest events. Ecological death is separate from `Fell()` and yields no timber or cash. At this historical v14 integration there was no automatic adult mortality or storm model, and no assumed deadwood disposition. Later D-048 adds adult density mortality and D-050 adds dormant StormModel 1; new games remain storm model 0.
- **C8 growth/competition (Sitka):** Hegyi cutoff 8 m (`ForestEcologyController.HegyiCutoffMeters`; global), Sitka Ci50 5, Sitka potential DBH growth 1.2 cm/yr. The competition equation is unchanged. At this 2026-10-02 integration Beech/Oak still carried their initial Ci50 3 and 0.4 cm/yr; that is history and is **not current**. The current accepted adult calibration is Oak Pd 0.8 cm/yr, Ph 0.65 m/yr, Dmax 120 cm, Hmax 42.5 m, Ci50 5 and Beech Pd 0.9 cm/yr, Ph 0.75 m/yr, Dmax 120 cm, Hmax 42.5 m, Ci50 6.5, all `GAMEPLAY CALIBRATION` (not empirical Irish broadleaf growth constants); see *Current Oak/Beech adult model* at the end of this document.
- **k10a10 light:** canopy shade reach = 1.0 × crown radius + half a 5 m cell (2.5 m) (`CanopyShadeReachPerCrownRadius`, shared by the marking forecast). Opacity is an implicit 1.0 and the light equation is unchanged.
- **Calibrated canonical lifecycle (RNG model 0, 80 years):** `BFC55473C1506067`. The calibration measurement hash is `D48A19525E69DD8A`.

**RNG model policy (D-046, main `4eab590`, verified 2026-10-05).** New games use model 1; loads keep the saved model; an absent field and Reference Future v1 mean model 0. `CCF_RNG_MODEL=0` makes the completion gate replay the legacy model. The anchors below were recorded under legacy regeneration (model 0); since D-047 they are reproduced with `regenerationModel = 0`, and JSON-based ones in the v15 layout.

| Anchor | RNG model 0 (compatibility) | RNG model 1 + legacy regeneration |
|---|---|---|
| Neutral lifecycle | `BFC55473C1506067` | `2A0B8C32AC0DE113` |
| Scenario One lifecycle (browse 0.2) | `3485B6630C9EA448` | `506E8AF6D8514C6C` |
| Completion gate | `568922E1A6D73CDD` | `00479F18970F9926` (completed Year 25, min cash 589770 cents) |
| Reference Future v1 Year 100 / continuation | `7AD177B3CC2F73C7` / `9CDF21A541C5968D` | n/a (v1 is model 0) |
| Policy save/load continuation (Year 20) | `CDC4DE8F8EF471E5` | `872094413305A082` |

Post-merge gates PASS on main: completion ×2 per model, interaction, browsing, planting, save hardening, RNG model, economy integration (120 assertions), Reference, RngModelPolicyVerification. Lag-1 survival-roll correlation is 0.6748 under model 0 and −0.0041 under model 1.

**Regeneration Model 1 (D-047, main `4670e73`, verified 2026-10-06).** Save v16 field `regenerationModel`: new games used1 before D-049 (current new games2); v1–v15, a missing field and Reference Future v1 0. Implementation: `RegenerationModel.cs` (constants, `RepresentationThreshold` = 0.01, annual account), `ForestEcologyCell` (bands, accumulator, `AdmitDensity`), `ForestEcologyController` (model-1 establishment, `AdmitRecruitment`, `NormalizeBands`, oldest-band promotion, `LastRegenerationAccount`), `ForestSaveValidation` (band keys: 4 represented + 1 accumulator per species + origin). Model 0 arithmetic is unchanged. `ScenarioReferenceArchive.LegacyV15WorldHash` re-hashes model-0 worlds in the v15 layout, because the v16 field changes JSON world hashes even for identical biology. `CCF_REGEN_MODEL=0` replays legacy regeneration in the completion gate.

| Historical v16 anchor | RNG 1 + regen 1 | Legacy (RNG 0 + regen 0) | RNG 1 + regen 0 |
|---|---|---|---|
| Neutral lifecycle | `962846D2F517B293` | `BFC55473C1506067` | `2A0B8C32AC0DE113` |
| Scenario One lifecycle (browse 0.2) | `FBB8F470D85FF815` | `3485B6630C9EA448` | `506E8AF6D8514C6C` |
| Completion gate | `6F84AF319D301F87` (Year 25, min cash 589773) | `568922E1A6D73CDD` (v15 layout) | `00479F18970F9926` (v15 layout) |
| Reference Future v1 Year 100 / continuation | n/a (v1 is model 0) | `7AD177B3CC2F73C7` / `9CDF21A541C5968D` (v15 layout; v16 layout `C5EF2C4CC3443208`) | n/a |

Verified at `4670e73` (isolated batch processes):
- `RegenerationModelVerification`: 35/35 twice, with identical 100-year funnels. Exact recruited trees for unthinned / 20 % / 60 % removal are 20/33/75 at year 25, 30/49/94 at year 50 and 32/51/96 at year 100.
- The legacy regeneration ledger (100 years, three treatments) is byte-identical to before (SHA-256 `71d61b94…`).
- Completion (×2 model 1, plus RNG 0 / regen 0 and RNG 1 / regen 0), interaction, canonical lifecycle, juvenile foundation, Reference, RNG and RNG policy, browsing, save hardening, planting, CCF Beech/Oak/Oak-player/planting, pruning, deadwood, progress, economy (120) and viability (64), Stage 1 work economy, timber yield, batch recompute, ecology calibration and the integration harness: PASS.
- ClearanceVerification, MenuTutorialVerification and ScenarioOneRemovalVerification fail identically on pre-merge `402a2b4` in batch mode. Reclassified in D-048 as harness issues (stale contracts / interactive-input requirements), not production failures.
- Under model 1, MixedSpeciesTest beech recruitment accumulates below threshold (accepted 0.0342, one crossing). It produces no trees, because of ledgered shade loss with zero structural extinction.

**Growth model 1 (D-048, main `2cfbabc`, verified 2026-10-07).**
- **Save:** v17 field `growthModel`: new Scenario One games 1; v1–16, a missing field and Reference Future v1 0.
- **Implementation:** `SitkaGrowthModel.cs` (Class III curve: b0 34.362 from the 20.4 m age-30 anchor, b2 0.042, b3 1.563; Comeau 2010 RD; [C] mortality parameters); `ForestEcologyController` (Sitka relative-height step and `ApplyAdultDensityMortality`, batched, step 4b); `ScenarioOneManager.OnTreeBiologicalDeath` (one fallen-deadwood record per death); `ScenarioReferenceArchive.LegacyV16WorldHash` (v16 layout).
- **Unchanged:** DBH, Hegyi, regeneration, browsing and economy.
- **Replay switch:** `CCF_GROWTH_MODEL=0` replays legacy growth in the completion gate.

| Historical v17 anchor | RNG 1, regen 1, growth 1 | RNG 1, regen 1, growth 0 | Legacy (0/0/0) | RNG 1, regen 0, growth 0 |
|---|---|---|---|---|
| Neutral lifecycle | `7E57B9DAEF5BF4D0` | `962846D2F517B293` | `BFC55473C1506067` | `2A0B8C32AC0DE113` |
| Scenario One lifecycle (browse 0.2) | `35E2BF1C2F55C2DE` | `FBB8F470D85FF815` | `3485B6630C9EA448` | `506E8AF6D8514C6C` |
| Completion | `D7C4DDD36B53FCCE` (Year 25; min cash 613,975) | `6F84AF319D301F87` (v16 layout) | `568922E1A6D73CDD` (v15 layout) | `00479F18970F9926` (v15 layout) |
| Reference Future v1 Y100 / continuation | n/a | n/a | `7AD177B3CC2F73C7` / `9CDF21A541C5968D` (v15 layout) | n/a |

Verified at `2cfbabc` (isolated batch processes unless stated):
- **New-model verification:** `SitkaGrowthModelVerification` 20/20; `RegenerationModelVerification` 35/35 with funnels byte-identical; growth-model-1 100-year runs identical to the source branch; the legacy regeneration ledger is byte-identical.
- **Completion and lifecycle:** completion under four model stacks; interaction; canonical lifecycle; juvenile foundation; Reference; RNG and RNG policy.
- **Gameplay and economy:** browsing; save hardening; planting; pruning; deadwood; progress; CCF Oak / Oak player / Beech / planting; batch recompute; Stage 1 work economy (420); timber yield (3,007); economy (120) and viability (64); ecology calibration; integration harness. All PASS.
- **Red gates:** Removal fails with its stale harness and passes with the corrected harness (`60674f1`). Clearance and MenuTutorial pass in an interactive Editor with the corrected harnesses.

Verified on integrated `main` at `b1e6c51` (Unity 6000.6.0f1, batchmode, each gate in its own process with an isolated `XDG_CONFIG_HOME`):

| Gate | Result |
|---|---|
| Import/compile | 0 compiler errors |
| EcologyCalibrationAdoptionVerification | PASS. Hash D48A19525E69DD8A. Interior/whole 0.3566/0.3877. Q/moderate/heavy 1.171/1.558/1.739 (interior 1.844). Edge 1.130. Light at Y5/Y10: control 0.029/0.043, Q 0.045/0.061, moderate 0.148/0.163, heavy 0.254/0.257 |
| JuvenileMortalityFoundationVerification | PASS. Shared juvenile rules (30 combinations), promotion persistence and tree mortality all pass |
| Canonical lifecycle, two separate processes | BFC55473C1506067 both times. The integration harness also reproduced it in A/B runs |
| Save hardening / RNG model / batch recompute | PASS |
| Scenario One interaction / habitat presentation | PASS. Year-0 grass: 4 patches in 3 supported cells; dark-cell negative control detected |
| Reference Future v1 | Archive integrity, archive negative control, preview Y0/20/50/100, current continuation (v12 → v14, deterministic) and contract all PASS. Year 100 = 7AD177B3CC2F73C7 |
| CCFIntegrationVerificationTemp, Beech, Oak, Oak player planting, planting | PASS |

The frozen archive and schedule blobs are byte-identical to the pre-integration main. A rendered ForestTest Year-0 smoke used an offscreen batch-mode render with Ultimate Nature present locally. The stand rendered normally, and grass appeared only at the bright clearing/road edge. This was not an interactive play session.

## Integrated Scenario 1

Fast-forwarded into `main` on 2026-10-04 from `integration/scenario-one-complete` at `dc975e4` (pre-merge main `bb314a4`). Detailed records: `Docs/BrowsingProtectionV1.md`, `Docs/Scenario1EcologyCompletion.md`, `Docs/Scenario1EconomyIntegration.md`, `Docs/Stage1WorkEconomyFoundation.md`, `Docs/TimberAssortmentYieldV1.md`, `Docs/SitkaPlantation02Integration.md`, `Docs/Scenario1StartingStandAssetFix.md`, `Docs/Scenario1CompactBrashFix.md`.

- **Browsing & Protection v1:** shared juvenile browsing response for natural cohorts and exact planted juveniles. Scenario One background browse pressure defaults to 0.2 (`ScenarioOneDefinition.backgroundBrowsePressure`; the asset does not override it). Neutral pressure 0 keeps the calibrated lifecycle `BFC55473C1506067`; the Scenario One lifecycle at 0.2 is `3485B6630C9EA448`.
- **Shelters:** tree shelters are coupled to exact planting and share its executor. A live shelter uses `installedYear` = resolution year and `effectiveYears` = 8. v15 restore clears protection, then restores saved shelters, then saved protected areas. `BrowseProtectionGeometry` builds fence-area records only; there is no fencing gameplay.
- **Work and economy:** `WorkEconomy` settlement, contractor or landowner-simulated execution per order (harvest is contractor-only), owner minutes, and `TimberYield` assortment allocation with sell / KeepForUse / retained deadwood outcomes. Economy continuation hash `023F4B627B758284`.
- **Save v15 bounded fields:** order executionMethod/installShelter/harvestJobId; scenario shelters/protectedAreas/ownerMinutesUsedThisYear; report minimum adjustment, owner minutes, retained volume and per-product sold volume/revenue; event executionMethod/ownerMinutes. No browse pressure, browse history or form damage is persisted.
- **Regeneration diagnosis and review:** aimed-ground diagnosis line, shelter visuals and annual review ecology lines.
- **Completion verification:** `Tools/Verification/ScenarioOneCompletionVerification.cs`, completion hash `568922E1A6D73CDD`.
- **Presentation:** Plantation Sitka 02 forms for trees under 20 m (pole/first-thinning), with legacy Sitka families retired from the active stand. Moss uses `Ground_Moss_Shoots_03` cushions, and `Forest Ground.mat` uses the textured soil/needle-litter material. Felling brash is a compact grounded 1.9–2.4 m patch beside the stump [D].

Integration regression (on `integration/scenario-one-complete`, Unity 6000.6.0f1, isolated-config batch processes): 27/27 gates PASS, after one fixture-version fix (`6ab31ce`).

Post-merge verification on `main` at `dc975e4` (2026-10-04):

| Gate | Result |
|---|---|
| Import/compile | 0 compiler errors |
| ScenarioOneCompletionVerification | PASS, `568922E1A6D73CDD` |
| Canonical lifecycle | `BFC55473C1506067`; Scenario One pressure 0.2 `3485B6630C9EA448` |
| BrowsingProtectionVerification | PASS (with shelter visuals, review lines, regeneration diagnosis and fence geometry) |
| Reference Future v1 | Contract PASS; Year 100 `7AD177B3CC2F73C7`; continuation v12 → v15 `9CDF21A541C5968D` |
| Scenario One interaction | PASS, including compact felling residue |
| Habitat presentation | PASS; legacy Sitka 0, one active display per tree at Years 0/20/50/100 |

The user's interactive Scenario 1 smoke (2026-10-04) passed, including the starting-stand and compact-brash presentation corrections. No performance problem was reported. Final presentation acceptance passed and was fast-forwarded into `main` at `ec938b0`. That merge brought in the track/clearing soil texture (`Forest Path.mat`), HUD readability fixes, the disposable review harness `Tools/Verification/ScenarioOnePresentationReview.cs` (run via `run_final_presentation.py`) and the record `Docs/Scenario1FinalPresentationAcceptance.md`. Post-merge gates are unchanged (14/14 PASS, all anchors). Astra ForestFloorV1 was reviewed and not adopted.

## Art and storage

Baseline committed presentation includes pruning families, refined Sitka benchmark, brash, logs/stumps/floor detail, bramble/bracken/fern/grass, other habitat, Oak/Beech juveniles, bent/cavity cosmetic variants, grass/rush and walkable LOD specimens. Ring-barked and some defect assets remain without matching causal spawning. D-050 now supplies causal windthrow state and fallen/root presentation under dormant StormModel 1; storms remain off by default. Crown/root visual quality and activation acceptance remain follow-ups.

Presentation must not invent ecology. Large binaries are ordinary Git objects; no new LFS/storage dependency or history rewrite is authorised.

## Verification

Disposable harnesses are under `Tools/Verification/`. Read `Docs/SuppressionHistory.md` and inspect each harness for side effects.

Documented integration procedure:

1. Use an idle/disposable worktree.
2. Copy `Tools/Verification/CCFIntegrationVerificationTemp.cs` into `Assets/ForestPrototype/`.
3. Run:

```text
<UNITY> -batchmode -nographics -projectPath <checkout> -executeMethod CCFIntegrationVerificationTemp.Begin -logFile <log>
```

4. Remove the copy and generated `.meta`.

This auto-start harness can touch `forest-save.json` and normally backs it up/restores it; assess that risk before running. Never leave it in a playable build. Compare hashes only under the verified Editor/configuration conditions.

Other documented entry points include `CCFBeechVerification.Begin`, `CCFOakVerification.Begin`, `CCFOakPlayerPlantingVerification.Begin`, `CCFPlantingVerification.Begin`, ScenarioOne Planting/Pruning/Removal/Deadwood/Progress verification `.Begin` methods and `ScenarioReferenceVerification.Begin`.

Minimum worker smoke: identify path/branch/HEAD/status; check shared settings; own Library; open Unity 6000.6.0f1 and ForestTest; check compile/console; run selected appropriate harness; remove script/meta; exit without unrelated serialized saves; account for final Git changes.

The migration generator uses Python 3.10+ standard library, not Unity. Run `python3 Tools/ProjectContext/generate_project_instructions.py` only after committing clean sources. Outputs are ignored in Build/ProjectContext.

## Local worktrees

The final integration/OpenAI/Claude worker layout and their independent Unity smoke gates are not established by this document. Target separate writable integration, OpenAI and Claude worktrees; preserve existing names if renaming would risk work. Check current paths/branches with `git worktree list` and record verified setup results through a separate assigned task.

Observed 2026-10-04 (`git worktree list`): `/home/jer/CCF-main` is the integration/main worktree (now on `main`). `/home/jer/CCF` (`feature/forestry-ecology`) holds uncommitted user ecology/art work that must be preserved. Task worktrees `/home/jer/CCF-claude`, `/home/jer/CCF-openai`, `/home/jer/CCF-opencode-economy`, `/home/jer/CCF-context-sync`, `/home/jer/CCF-project-context-migration` and `/home/jer/CCF-review-ecology` remain on their task branches.

## Revision 5 provenance

Source pack: Drive folder `1fUyti4nHlbKKwjWnvWsVvSbvFN-4mhm0` (Claude Review Pack — Refreshed Drafts); review request identifies Revision 5 and D-035–D-040 acceptance. Native Google Docs were read through authenticated read-only Drive access, not changed.

| Repository destination | Source Google document ID |
|---|---|
| Docs/Project/game-brief.md | 1Rkvo9nUrU99g8wz1XU2CSibo-WWZpoPK8u_DSKVOS9w |
| Docs/Project/decision-log.md | 1nODkvCnHD-89NziaRY4qreaba61yYn6JqvdwpyYLZ04 |
| Docs/Project/current-milestone.md | 10ksKpEkmMllHTZq9aCRwGTurv4S0TG9SjXBlCFcNgik |
| Docs/Project/unity-project-overview.md | 1WSjjXXGgA7f9Pyppo2PQdJ2pjXXW5xbqHzMh2cm7c1o |
| Docs/Project/AgentWorkflow.md | 1vh_C5dKZ6FAlucRV4K_KvMRp5vvjWwrRJ6LMBw7jMR0 |
| Docs/Project/research-index.md | 12pyVVmHpAcdyuxQsOEXQVkO2qFO6borwsRsrN61FpTE |
| Docs/Project/instructions/shared-project-instructions.md | 1t03jEuGVJxfYAXPAilLkgJWI_7a_RuzWBuASIfE-PIw |
| Docs/Project/instructions/chatgpt-role.md | 11_myY8N1taegsf-M8tQSXiZ2rjFzpCkop5w33TSf7AI |
| Docs/Project/instructions/claude-role.md | 1VH8V-t9xqDg02yv5c_Ui65BrfhbQCf_Ok3PwNoN64WU |
| Docs/Project/coordination/active-tasks.md | 1AUdhc-HnKTkWo5sbkwPOTtLGEIkhkgSShofv8x0Konk |
| AGENTS.md | 1QKEsZKCPblDEZUOWHTaSMYAV2f9jmWpGKboJd3EaKqo |
| CLAUDE.md | 1G-RaRphkDLoWSG_sXosgAuLy4K3OAbRzdxsV955oLlg |
| Tools/ProjectContext/generate_project_instructions.py | 1Q_Htq4yJ3xgfphDdCdYYz26VMccjoFOV850TMI-Cs_U |

Migration normalises text layout/encoding, ports useful legacy rules, records the task scope/baseline, and makes the supplied generator executable with clear error handling. It does not advance gameplay design or incorporate the frozen stack.

## Regeneration Model 2 — accepted Scenario One v1 (D-049, 2026-10-08)

Source `902903f`; implementation/evidence `49b01d8fc90ea0779ebfdc61a5a421af718da1d6`. Integrated and verified in `/media/jer/ZX20/CCF-integration-model2`, preserving dirty local main. Exact final revision and reproduction are in `Docs/Verification/RegenerationModel2Integration/IntegrationHandoff.md`.

- Save18 adds exactly cell brambleCover/brackenCover and local-patch brambleCover/brackenCover/competitionUpdatedYear. Independent finite0..1 fractions may overlap; no habitat/mesh inference. Complete indexed grids, explicit JSON fields, finite bounds, deterministic patch keys, stand-contained centers and nonfuture timestamps are validated before loading. Immediate-reset saves capture their complete initial grid. No sixth persistent ecology state.
- New Scenario One is RNG1/regeneration2/growth1. Existing v17 model1 is unchanged; legacy/missing-field policies and Reference0 remain unchanged.
- Shared Sitka/Oak/Beech hazard `.35 × exposure × height vulnerability`, exposure=max(two covers), full below .3m, linear to zero at1.5m. Site/target/initial/recovery profile is exactly D-049; these are [C]/[I]/[D] choices, not empirical Irish constants. Target is restricted to Scenario One; site-diverse content needs renewed driver calibration.
- Annual work/canopy changes precede competitor recovery and survival. Vegetation, light and browse losses remain separate runtime accounts. Height growth, seed/request/capacity/promotion rules and prices are unchanged. Natural abundance remains relative occupancy. Exact juveniles use an independent id-keyed vegetation domain.
- Latest patch/year then stable geometry key wins independently of list order; weighted natural exposure counts overlaps once. Completed saved whole-cell work masks older local overrides only inside that cell. Clearance retains juvenile-removal semantics, zeros applicable competitors, then allows recovery.
- Existing bramble/bracken art reads the new state; other habitat stays non-causal. Beech biological .6m renders at approximately .6m with matching planted semantics. Art recognition/human clearance playtests remain open.
- The learning panel uses the approved concise clearance explanation for model2, and preserves no-competition teaching for model0/1. Its display refreshes after saved-model changes without changing forest or lesson progress.
- Preserved model2 start/year1/completion: `FA855239CDDA32D8`, `02334804F65C0234`, `702766DECE591E21`. Historical model1 completion v17 layout `D7C4DDD36B53FCCE`; canonical and immutable Reference anchors retained.

The original39 paid century simulations and measured storage/timing are preserved as reviewed source evidence; the clean integration reproduces critical checks, complete regression, state and performance, plus actual rendered model-aware pedagogy. See the integration handoff for exact results and limits; no clearance profit or universal treatment schedule is claimed.

## Integrated dormant StormModel 1 (2026-10-08, D-050)

Manager-approved source `857150b2f20afa52d7b36be88a835f07a4ee5a51` is integrated as the complete fast-forward from main `1fbefd8a40c69dd90abb83f2149e24b546a184cf` in `/media/jer/ZX20/CCF-integration-model2`, branch `integration/storms-windthrow`. Gameplay production bytes remain identical to the approved source. Exact publication/canonical/context revisions and reproduced verification are in `Docs/Verification/StormsWindthrowIntegration/IntegrationHandoff.md`.

- Save 19; new Scenario One RNG 1/regeneration 2/growth 1/storm 0. Missing/historical saves and Reference Future v1 stay storms-off; no event history exists under StormModel 0.
- Frozen StormModel 1 profile: annual .02; severity .02/.06/.18; equal weights; ReducedProposal; BoundedRational; neutral site/external edge. All are [C] gameplay calibration. The historical `ComparisonDefault` method name does not authorize silent recalibration under the same version.
- Exactly root stormModel and event year/severity/directionDegrees/cropTreesLost are added. Death/deadwood/work records remain authoritative; no duplicate victims, fall bearings, thinning history, salvage history or calibration profile is persisted.
- One uprooted/fallen outcome removes live stock, records cause/year, creates one deadwood record, updates existing recentOpening and batches canopy/seed rebuilds. Changed light/understorey feeds ordinary Model2 survival/recruitment.
- Leave deadwood is default. Individual X-selection permits partial Sell/Keep through existing grouped harvest settlement and minimum; work multiplier 1.00, prices unchanged. Save/load and zero-volume extraction avoid duplicate salvage or material/crown resurrection.
- Existing qualitative Stable/Watch/Exposed causal feedback, storms-off explanation, fallen-stem prompt, Work Plan salvage, Annual Review and walking/map waypoint remain. No exact player-facing hazard percentages or new lesson objectives.
- Original root/log assets and a bounded rotated-crown prototype remain. 20 crowns within 35 m; existing LODs. This is not final art or performance acceptance.
- Compatibility: v18 start FA855239CDDA32D8; altered-site fixture year1 02334804F65C0234; completion 702766DECE591E21. The original altered-site fixture leaves SiteProductivity0; independent clean year1 is 8333BAA4126E8A09. Legacy Model1 v17 completion D7C4DDD36B53FCCE and Reference 7AD177B3CC2F73C7 / 9CDF21A541C5968D remain unchanged. Full modern hashes include schema19.

Current Editor-only performance is sufficient for dormant integration, not default activation or a Player-FPS claim. Before activation require standalone Player, developed/century and combined deadwood/windthrow profiling, plus acceptable crown/root behaviour. Bespoke art, optimisation, final activation decision, full lessons, surrounding directional exposure and snapped/snags remain separate scope. Future material Model1 recalibration needs a new storm version or approved persisted-profile architecture.

## Integrated P3 Work Plan overview (2026-10-09)

P3 is integrated on main at `0ab73994153d55d08adbd65f928926e6e9165f7c`, directly after P2 at `869ee92a983a1af5fc470392eccc7557fbe45def`. The Work Plan includes MONEY and WHAT YOU ARE LEAVING: prospective cash/headroom, standing-tree/basal-area/volume before and after, retained crop/seed trees, spatial removal descriptions, P2 competition reuse, Model2 clearance explanations and deadwood estimates. Existing salvage controls, authoritative quotes and economy values are retained. Save19 and the rng1/regen2/growth1/storm0 new-game stack are unchanged.

Source and focused verification tooling are in `Docs/Research/ScenarioOneCompletion/` and `Tools/Verification/ScenarioOneCompletion/P3/`. The historical Help/Escape failure is Manager-classified as a non-reproducible historical transient, not an established harness fault or production bug. S1-A Teaching Readiness was subsequently integrated and verified on main at `c01de2dbc89e760c49a9812a80dbed19632c185e`, preserving the two approved commits by fast-forward from `2c0f7cdbde7bb2ce982fda7a6963be48e01278aa`. Unity compile/import had zero errors; Teaching Copy, two independent MenuTutorial runs, Clearance, Removal, P2/P3 focused gates and the full 24/24 current regression passed, with all required compatibility anchors unchanged. Earlier varying MenuTutorial failures, including a baseline failure, remain in the verification evidence; the final independent passes used unchanged fixtures without the diagnostic frame cap. Stage A readiness after S1-B is recorded in the next section. Future multi-scenario infrastructure remains unimplemented. Uncommitted local material changes are not part of this committed project state.

## Integrated S1-B standalone-build readiness and hidden construction frames (2026-10-10, D-055/D-056)

Approved S1-B source `4359e500c6b312ebcf957397b4fe53205f3f1ca4` (seven commits) was integrated as a fast-forward of main `4a68d528bb9d01036de9d4df1739b7de2c8c6002`. A separate commit, `fac782072f23e28518e8840c621112fe4074db45`, then hid the unavailable construction frames (D-055). Verified in the clean worktree `/home/jer/CCF-s1a` with its own Library on Unity 6000.6.0f1. Evidence and exact results: `Docs/Verification/S1BIntegration/`.

- **Standalone session menu.** `StandaloneSessionMenu` bootstraps only in non-Editor players, so Editor gates are unaffected. F10 pauses time and controls and hides the forest UI; the menu shows the exact build ID and offers a fresh Scenario One, continue, save and load with explicit unsaved-progress/overwrite confirmations, and a normal quit without autosave. Reference preview cannot be saved through it. `ScenarioOneManager.Update` yields while `StandaloneSessionMenu.IsOpen`. Save 19 and the new-game stack rng1/regen2/growth1/storm0 are unchanged.
- **Build tooling.** `Tools/Build/S1B/` builds Windows x86-64/Mono from clean committed source, stamps the build ID and full SHA, opts private builds out of Unity Engine Diagnostics and checks the five service flags are false, excludes generated test metadata, package test assemblies and DontShip backups, and writes a SHA-256 manifest. The first candidate, B06 (`CCF-S1-20261009-B06-b3ba9b8`, archive SHA-256 `c7d4da9d5316c3bb419b39f16e84827bc1a41ab07c75ade01316d063a1d22188`), is **historical evidence only**: it predates the hidden-frame correction and the stand-expansion direction and must not be sent to external testers. No pilot candidate exists. Its Wine API smoke passed, but Wine rendered no readable UI text (LegacyRuntime font-face errors); the effect on native Windows is unknown.
- **Hidden construction frames (D-055).** Before the change ForestTest showed eight frame sets: seven `ForestBuildable` sites 4–8 m from the player start plus the Plank Rack's static foundation marker. Each of the seven offered a live `[E] Build X (N Wood)` action that spent carried, stored or retained timber, and a real `TryBuild` with enough carried wood constructed the Workbench. After the change: 0 frame renderers, 0 solid site colliders, 0 build prompts at any site, and `TryBuild` does nothing; exactly 35 active renderers were removed (all under "Unbuilt Site") and none added; the 336 trees are unchanged. `ForestBuildable.constructionAvailable` defaults off; structures already built in older saves still appear. The Plank Rack uses two single-line scene edits (`m_IsActive`, `m_Enabled`). `ScenarioOneInteractionVerification` asserts the default-off behaviour and then enables the flag by reflection so the latent mechanic (exact retained-timber debit) stays tested. `ForestSceneBuilder` (Editor) still authors the frames; re-running it would restore them. The hidden sites still reserve their footprint from starting-stand placement and the automatic planting search, which is unchanged behaviour.
- **Verified at `fac782072f23`:** compile/import 0 errors; SessionMenuVerification; Teaching Copy batch ×2 and rendered (read-only hash `EBE7A228F630AE41`); MenuTutorial ×2, Clearance, Removal; P2 batch ×2 `F58FB0B1A421D28B`; P3 batch ×2 and interactive `F7C2FC966816BBAD`; full 24-gate regression 24/24 with Model2 completion `702766DECE591E21`, Reference Y100 `7AD177B3CC2F73C7`, continuation `9CDF21A541C5968D` and every exercised 16-digit anchor unchanged against the committed modern baseline.
- **Open gate — P2 interactive rendered layout.** `CropTreeCompetitorVerification --interactive` fails with "a world label is drawn under a panel at 1280": competitor tag 1 lands on the top edge of the competitors panel at 1280×720. It fails identically at S1-A's integrated `c01de2d`, the approved S1-B tip `4359e50` and `fac7820`, with the screenshots pixel-identical across all three and with or without the Ultimate Nature pack, so it is not attributable to S1-B or D-055; the same code passed for the S1-B worker on 2026-10-09. The cause is not identified and the gate is not claimed passed.
- **Not verified / not implemented:** native Windows technical smoke (readable text and build ID, real input, walking, save/load, first-cycle usability, performance and stability); a pilot Windows candidate. (The ≥ 80 × 80 m stand, D-056, was integrated afterwards; see the next section.) Stage A is **not ready**.

## Integrated Enlarged80 stand (2026-10-10, D-056)

Approved source `a309f99e0f2c6a770044604bbf88ee2e2471a5b5` (three commits: Phase-1 audit `9f49e2e`, 80A geometry/save foundation `b024633`, 80B Enlarged80 `a309f99`) was integrated as a pure fast-forward of main `11c3596072a0422d5426291e370129d209c56342` and verified in the clean worktree `/home/jer/CCF-integration-80m` (branch `integration/scenario-one-80m`, its own Library) on Unity 6000.6.0f1. Evidence: `Docs/Verification/StandExpansion/` (implementation, calibration table, rendered review) and `Docs/Verification/Scenario80mIntegration/` (post-integration results).

- **Geometry model and save v20.** `StandGeometryModel`: `Legacy40 = 0` (40 × 40 m, 5 m cells, 8 × 8 = 64 cells) and `Enlarged80 = 1` (80 × 80 m, 16 × 16 = 256 cells). `ForestSaveData.CurrentVersion = 20` adds an explicit `standGeometryModel` to every v20 save; saves up to v19 or without the field are Legacy40. On load the geometry is decided and the save is validated against that geometry's own cell count before any world mutation, then `ForestEcologyController.ApplyStandGeometry` is the first mutation and cell-indexed state is restored afterwards; unknown or malformed geometry is rejected first. Not every Scenario One record that carries a cell index has a pre-mutation range check (recorded follow-up).
- **Authority.** The production geometry is the new-game policy (`StandGeometryPolicy`, Enlarged80) or the save's persisted `standGeometryModel`. `CCF_STAND_GEOMETRY` is an Editor-only verification override: compiled out of players, never serialized, not production authority.
- **Enlarged80 world.** 80 × 80 m, 0.64 ha, 256 cells, 1,344 living trees (336 retained legacy `Pxxxx` trees with unchanged IDs and exact positions, plus 1,008 outer `PO{row:D2}{col:D2}` trees) at ≈ 2,100 stems/ha, age 20. Tree-edge shares 0–5 / 5–10 / > 10 m = 27 / 17 / 56 % (64 % more than 8 m from the boundary; Legacy40: 46 / 32 / 21 %, 33 %). The retained core's neighbourhood-sensitive starting DBH is evaluated against the full enlarged occupancy: 181 deep-interior trees keep their DBH exactly and 20 former edge-ring trees lose the artificial edge bonus. 61 preserved legacy clamp-line trees (the Legacy40 lattice is 2.0 m, clamped to ±19.4 m) leave a bounded denser, higher-Hegyi band just inside the former ±20 m edge; this is accepted and the trees are not moved. Construction frames stay hidden (D-055).
- **Runtime boundary geometry.** `StandWorldBoundary`, added at runtime by `ForestEcologyController`, follows `StandGeometryApplied`: it scales the ground and moves the four boundary ridges to ±40 m, extends the camera far plane, and keeps ground texture density with a per-renderer `MaterialPropertyBlock`. It restores the authored transforms exactly for Legacy40. The scene file and the shared materials are unchanged; in the Editor's Scene view outside Play mode the ground still shows the 40 m authored state. `ForestStartingStand.Generate()` keeps the Legacy40 path untouched and adds `GenerateEnlarged80`.
- **Reference Future.** Reference Future v1 remains the immutable Legacy40 archive. `ScenarioReferenceArchive.Matches` requires live Legacy40 geometry, so in Enlarged80 `ReferenceArchiveAvailable` is false and the Century Review falls back to the aspirational design targets (the then-current copy still said "frozen Reference Future"; corrected by D-057 below). A Reference preview from an Enlarged80 game captures the player world, applies Legacy40 and the archive, and on exit restores the exact Enlarged80 world, the player's position, look and camera, and the Stand Map waypoint.
- **Anchors.** Hash identities are explicit: the current v20 hash (`CurrentWorldHash`) never strips the geometry field and is geometry-aware; the Legacy40 compatibility hash (`LegacyCompatibleWorldHash`) is the historical layout for Legacy40 only. Historical Legacy40 anchors are unchanged: Legacy40 starting world `EBE7A228F630AE41`, P2 `F58FB0B1A421D28B`, P3 `F7C2FC966816BBAD`, Model2 `702766DECE591E21`, Reference Y100 `7AD177B3CC2F73C7`, Reference continuation `9CDF21A541C5968D`. Enlarged80 has separately labelled current anchors: start `04A78188A6F8E6F9` and year 3 `081DB58CE3034648`; they are valid only for this generator and configuration.
- **Verified at `a309f99`.** Compile/import 0 errors; `StandGeometryVerification` (10 check groups) and `Enlarged80Verification` (12 check groups) PASS, with the anchors above exact; a Reference-preview round trip from Enlarged80 restoring the player pose and waypoint; SessionMenuVerification in Enlarged80 and under the explicit Legacy40 override; Teaching Copy ×3, MenuTutorial ×2, Clearance, Removal; P2 batch ×2 and interactive, P3 batch ×2 and interactive (all hashes exact); the full 24-gate regression under the explicit Editor-only Legacy40 path, 24/24; a rendered review confirming the 256-cell Stand Map at 1280 × 720 and 1920 × 1080 and the waypoint mapping. The P2 interactive gate passed on its first run in this fresh integration worktree; it is bimodal (it failed with the known signature at the beginning and end of the 80 m task worktree) and the open-gate note above is not closed.
- **Editor performance (not a Player or native-Windows measurement; GTX 980M, i5-6300HQ).** Legacy40 → Enlarged80: canopy recompute about 11 → 185–217 ms, annual ecology step about 0.12 → 0.9–1.1 s, full manager advance about 0.2 → 1.5–1.7 s, rendered frame median about 24 → 40 ms, new-game generation about 0.4 → 1.8 s. No optimisation was done.
- **Not changed by the stand expansion.** Economy and objective values (the pre-calibration area-sensitive thresholds are tabulated in `Docs/Verification/StandExpansion/Enlarged80Calibration.md`), the light model, tree stocking, construction policy, Reference Future v1, definition version `scenario-one-v13`.
- **Subsequent calibration/copy work:** completed under D-057 below. Malformed cell-index save hardening is now integrated and verified at `12cc8ef0659f54055088047c2c6727d874bdffd8` (below). **Still not verified / not implemented:** standalone Player performance profiling; native Windows technical smoke; a pilot Windows candidate. Stage A is **not ready**.

## Integrated Enlarged80 area-sensitive calibration (2026-10-10, D-057)

Approved source `23f84f9ac0d4a02d385869d3985e11a24cc5a23f` was integrated by fast-forward from `f4535caf606bca46f35f02408920408d6b9f20d3`, preserving all three commits, and post-integration verified in `/home/jer/CCF-integration-e80-calibration`, branch `integration/enlarged80-area-calibration`, with its own fresh Library on Unity 6000.6.0f1. The production implementation is `fa9f66dfed1b7f8a24295f379e2bb9e302e84a66`; the final source commit adds verification evidence. No calibration implementation was changed during integration. Source evidence: `Docs/Verification/Enlarged80Calibration/`; post-integration evidence is retained in `/media/jer/Files/CCF-integration-e80-calibration-local-output/Build/` and the integration handoff.

| Effective target | Legacy40 | Enlarged80 |
|---|---|---|
| Retained original trees | 60 | 240 |
| Regeneration cells | 3 | 12 |
| Fallen deadwood m³ | 0.02 | 0.08 |
| Century original trees | 120 | 480 |
| Century broadleaf presence per planted species | 10 | 40 |
| Century regeneration cells | 12 | 48 |
| Century fallen deadwood m³ | 0.5 | 2.0 |

- **Calibration boundary.** Extensive targets are deliberately 4× for 4× property area: gameplay continuity calibration, not empirical forestry thresholds. The serialized ScenarioOne.asset remains byte-identical as the Legacy40 baseline. Geometry-aware effective-target accessors reject unknown geometry; a future geometry requires a fresh calibration decision.
- **Unscaled.** Completion canopy 0.35, aspirational canopy 0.65, Year 25 completion horizon, Year 100 century review, starting cash €12,000, annual commissioned-harvest minimum €2,500, owner capacity 2,400 minutes/year and all unit labour/material/stock/timber values remain unchanged. Save v20, scenario-one-v13, geometry/stocking and immutable Legacy40 Reference Future v1 remain unchanged.
- **Restored Century Review.** Pre-calibration Enlarged80 aspirational reviews refresh target values and achieved flags on restore without rewriting measured values/history, outcome or management comparisons. Legacy40 and genuine frozen-reference cached reviews are unchanged. Objectives and Annual Review share effective targets. Enlarged80 Century Review says “Scenario One aspirational design targets”, “not a forecast, optimum or prescription” and labels rows “target”; genuine Legacy40 Reference review retains frozen Reference Future and “reference”.
- **Solvent feasibility witness.** Completed in Year 25; minimum cash €11,451.16; three intervention years; 462 trees felled; eight Oak and eight Beech planted. At Year 30: 793 original-species trees, 232 regeneration cells, about 58.73983 m³ deadwood, mean canopy about 0.8837839. Owner time reported 24 minutes total. One-tree harvest still loses money under the same €2,500 minimum; the larger first job spreads that fixed minimum over more stems. This is one feasibility witness, not an optimum, prescription or forestry threshold validation.
- **Post-integration gates.** Calibration, viability, StandGeometry, Enlarged80, Teaching Copy, MenuTutorial, Clearance, Removal and Interaction PASS; P2 and P3 batch ×2 and interactive PASS (no inherited P2 failure allowance needed in this checkout); full explicit Editor-only Legacy40 regression 24/24 PASS. Rendered calibration review PASS at 1280×720 and 1920×1080. Zero C# compilation errors and zero unexpected importer errors. Across the 55 audited logs, the known Editor Search exception occurred 39 times, each with the identical recorded base/candidate signature.
- **Unchanged anchors.** Enlarged80 start `04A78188A6F8E6F9`, year 3 `081DB58CE3034648`; P2 `F58FB0B1A421D28B`, P3 `F7C2FC966816BBAD`, Model2 `702766DECE591E21`, Reference Y100 `7AD177B3CC2F73C7`, continuation `9CDF21A541C5968D`; every exercised historical compatibility hash retained. No scene/prefab/material/settings/package/meta/archive change; user save/backup and unrelated work preserved.

Minimal startup/title screen is now integrated and Editor-verified under D-058 below. Stage A remains **NOT READY**. Malformed cell-index save hardening is now integrated and verified (below). Remaining pre-pilot gates begin with standalone Player performance/build profiling, a new Windows pilot candidate, Sonnet 5.5 xhigh combined pre-pilot review, native Windows technical smoke, genuine blocker fixes and the beginner pilot. No standalone/native-Windows execution or performance claim is made by these Linux Editor gates. S1-C/S1-D remain later full-loop work, not Stage-A prerequisites.

## Integrated minimal startup/title screen (2026-10-11, D-058)

Approved source `b99ba6993b6b7cfb9fc3b8de6c4eda88abd8a348` was integrated as a pure fast-forward from `e8c73f25e6d93db818abdc6c794887f8f7c5ba43`, preserving all eight commits. Post-integration checks ran on Unity 6000.6.0f1 in `/media/jer/Files/CCF-integration-startup`, branch `integration/scenario-one-startup-screen`, with its own fresh Library and isolated save/config paths. Implementation bytes were unchanged. Source evidence: `Docs/Verification/StartupScreen/`; bounded integration logs/audits: `Build/StartupIntegration/` in that checkout and the integration handoff.

- **Startup/session behaviour.** Player startup has a title layer before forest control: CCF / Scenario One / Sitka Plantation to Continuous-Cover Forest, private-test build identity and Start New Scenario / Continue / Controls / Help / Quit. The existing StandaloneSessionMenu owns it; F10 retains the fuller in-game session menu. Start New confirms an existing save and preserves disk bytes. Continue is available only for an accepted existing save and revalidates when clicked. CanLoad is read-only and shares pre-mutation loader validation. Editor Play Mode enters directly by default; `CCF_STARTUP_SCREEN=1` opts into the title in the Editor only.
- **Bounded post-integration verification.** Compile/import: zero C# errors and no new importer exception (only the exact accepted Editor Search signature). Startup batch, startup bootstrap, SaveHardeningVerification, StandGeometryVerification, established SessionMenuVerification and AreaCalibrationVerification PASS. Disposable Assets harnesses/meta removed; approved tracked bytes restored exactly after attributed Unity workspace-settings normalization; no scene/prefab/material/settings/package or unrelated drift; original user saves preserved.
- **Accepted source qualification.** Full explicit Legacy40 regression 24/24 PASS; rendered title checks at 1280×720 and 1920×1080; no-save/valid-save/invalid-save and Legacy40 Continue; read-only CanLoad; Start New preserving the disk save; one-host/one-overlay restart; actual Linux Editor mouse and keyboard checks PASS. The byte-identical complete source suite was not repeated during integration. Synthetic graphical InputSystem delivery remains explicitly UNVERIFIED; callbacks do not establish it. Source real-input launcher exited 143 after functional PASS and normal Unity shutdown; cleanup was performed after confirming exit.
- **Compatibility boundary.** Save v20, definition scenario-one-v13, geometry, stocking, calibration, model identities and frozen Reference Future unchanged. Accepted source anchors remain Enlarged80 start `04A78188A6F8E6F9` / year 3 `081DB58CE3034648`, P2 `F58FB0B1A421D28B`, P3 `F7C2FC966816BBAD`, Model2 `702766DECE591E21`, Reference Y100 `7AD177B3CC2F73C7` and compatibility continuation `9CDF21A541C5968D`. The natural-scene diagnostic hash is not a replacement anchor.
- **Release boundary.** Actual built-Player and native-Windows startup/input/save behaviour remain UNVERIFIED; no Player/native-Windows performance claim follows from Editor checks. No Windows candidate was built. Stage A **NOT READY**. Malformed cell-index save hardening is now integrated and verified (below). Next: standalone Player performance/build profiling → new Windows pilot candidate → Sonnet 5.5 xhigh combined pre-pilot review → native Windows technical smoke → fix genuine blockers → beginner Stage-A pilot. S1-C/S1-D remain outside Stage-A first-cycle prerequisites.


## Integrated malformed cell-index save hardening (2026-10-12)

Approved source `12cc8ef0659f54055088047c2c6727d874bdffd8` was integrated and published unchanged on main by pure fast-forward from `5d57208a411404b65b5d9a0d4f2a6e88c4086400`, preserving all four reviewed commits. Bounded integration ran on Unity 6000.6.0f1 in `/media/jer/Files/CCF-integration-save-hardening`, branch `integration/save-cell-index-hardening`, with its own fresh Library and isolated saves/config. Evidence: `Docs/Verification/SaveCellIndexHardeningIntegration/IntegrationHandoff.md`; audited field semantics: `Docs/Verification/SaveCellIndexHardening/FieldAudit.md`.

- **Saved-geometry authority.** Persisted ecology and nested Scenario One cell-index records are checked against `StandGeometryModel.ForSave(data)` before the first live-world mutation. Legacy40 accepts cells 0..63; Enlarged80 accepts 0..255, independently of the current live grid or caller-supplied count. CanLoad and LoadData use the same pre-mutation rejection path; malformed data is refused without world or disk mutation. Treatment-patch centre checks remain on the existing shared saved-geometry path.
- **Field semantics preserved.** Required indices reject negative/out-of-range values. Optional fields retain their field-specific −1 sentinel; `PlantedJuvenileSaveData.cellIndex = -1` remains valid. Planting/removal work orders require a real cell; tree-based orders may retain −1. Ecology records remain a unique sparse collection. Supplied understorey indices remain unique and in range, with the existing Model2 complete/ordered-grid contract retained. Legitimately shared cells in juveniles, deadwood, management history and distinct work orders remain valid; historical partial collections retain their actual semantics.
- **Bounded integration verification.** Fresh compile/import PASS with zero compile/import errors. SaveCellIndexHardeningVerification PASS once: **131 cases, 78 rejected, 53 accepted**, including valid/historical sentinels, both geometries, CanLoad/LoadData/Load parity, exact no-mutation/disk checks and startup Continue refusal. SaveHardeningVerification, StandGeometryVerification (10 checks), startup batch Continue/CanLoad and AreaCalibrationVerification PASS. Emitted comparison hashes agree with the qualified source; StandGeometry emitted `EBE7A228F630AE41`. The exact known shared-settings regeneration was restored to HEAD after every gate; no material, source, fixture or unrelated drift remained, and original saves were preserved.
- **Qualification boundary.** Source qualification was PASS ×2 at 131 cases/run plus full Legacy40 regression 24/24 and all seven frozen anchors exact. These are retained source results, not claims that integration reran the full suite or re-emitted all seven anchors. The pure identity fast-forward and green bounded checks required no full-regression repeat. Startup batch mode did not repeat rendered/physical-input checks; synthetic InputSystem graphical delivery remains unverified.
- **Unchanged scope and release boundary.** `ForestSaveData.CurrentVersion` remains 20; definition remains `scenario-one-v13`; Reference Future v1 remains immutable Legacy40. No schema, scenario-definition, calibration, fixture, gameplay, geometry or stocking change occurred. The accepted retained former-edge-band residual remains. The external demo is **NOT READY**. Per D-060 the first external demo is the complete Scenario One, so the remaining pre-demo gates are: species-parity P0 (D-059/D-061/D-062) → S1-C → S1-D → scenario completion → integrated full Scenario One qualification → fresh post-parity/full-loop Windows candidate → independent review and native Windows technical smoke → beginner external testing. B06 remains historical and must not be sent; B07 is an internal technical baseline and is not the eventual external-demo candidate. Built-Player performance and native Windows text/input/performance remain unqualified. See **Species-parity audit facts and pre-demo scope** below for the supersession of earlier Stage-A wording.

## Species-parity audit facts and pre-demo scope (D-059–D-062, 2026-10-12)

The Oak/Beech/Sitka species-parity audit inspected production code at `c1564b857972ee18917e8a461c457533a54cc3f1`. It changed no production code. The following are **inspected implementation facts at that commit**, each tied to the code that establishes it. They are dated history: the three-species lineage has since been integrated on `main` (Rack V2 core integration `f23549b`), and the current Oak/Beech adult-model state is recorded in *Current Oak/Beech adult model* at the end of this document (the density route itself is owned by PARITY-03 and is not restated here):

- **Adult Sitka height growth is species-gated.** `ForestEcologyController.cs:1014` applies `SitkaGrowthModel.NextHeight` only when `UsesSiteClassGrowth && SameSpeciesId(treeSpecies, SitkaGrowthModel.SpeciesId)`, where `SitkaGrowthModel.SpeciesId` is `"sitka-spruce"` (`SitkaGrowthModel.cs:24`). Other species keep the legacy height law at `ForestEcologyController.cs:1010–1011`.
- **Adult density / self-thinning is not species-gated.** `ForestEcologyController.ApplyAdultDensityMortality` (`:1040`) collects `living` as every `FindTrees()` entry with `tree.IsLiving` (`:1044–1046`) with no species filter, then calls `StandRelativeDensity(living, …)` (`:1050`).
- **The British Sitka relative-density calculation currently receives all living species.** `StandRelativeDensity` (`:1122–1133`) sums `Diameter²` over the whole living list, divides by `trees.Count` for `Dq` and by `StandAreaHectares` for `N/ha`, and returns `SitkaGrowthModel.RelativeDensity(stemsPerHectare, quadraticMeanDbhCm)` — the Comeau et al. (2010) British Sitka maximum size-density relationship implemented in `SitkaGrowthModel.cs`. Density pressure (`:1051`), the suppression-weighted hazard (`:1051–1070`) and the `RD ≤ 1.0` boundary enforced by killing the most suppressed survivors (`:1078–1104`) are all derived from that single stand-wide figure.
- **Oak and Beech therefore currently inherit the Sitka density / self-thinning treatment.** Consequence of the two facts above: a broadleaf of similar size is counted as occupying the same growing space as Sitka, is thinned by the Sitka `[C/D]` hazard calibration (onset RD 0.6, strength 0.08, suppression exponent 4), and is subject to the British Sitka RD 1.0 boundary. This is recorded here as an inspected limitation, not as accepted behaviour. D-059 requires that Oak and Beech must not silently inherit materially inappropriate Sitka-specific assumptions.
- **Oak and Beech dedicated verification is pinned to Legacy growth and does not currently qualify Growth Model 1.** `Tools/Verification/CCFOakVerification.cs:361` and `Tools/Verification/CCFBeechVerification.cs:25` both set `ecology.GrowthModelVersion = GrowthModel.Legacy`. No Oak or Beech test therefore exercises `ApplyAdultDensityMortality`, and no mixed-stand density test exists. This is why the limitation above is not currently caught by verification.
- **Pure-Sitka behaviour must remain exact during the replacement architecture.** When composition is 100 % Sitka, `StandRelativeDensity` already computes the intended calibrated Sitka figure and must continue to produce identical results. D-061 makes this an acceptance criterion: 100 % Sitka composition routes through the existing exact calibrated Sitka density implementation, existing Sitka arithmetic is unchanged, and Sitka anchors are not silently re-baselined.

**Scope these facts create (D-059, D-061, D-062).** Species-parity P0 is pre-demo work and is not started at this commit: species-aware density / self-thinning using ordinal species-aware growing-space-demand classes (D-061) with no DEC, no DMC and no mixture-productivity bonus; Oak and Beech adult parameter parity with labelled transfer/ordinal evidence; species-specific regeneration, juvenile and browse traits; species-specific canopy, light and site traits; an Oak/Beech timber assortment and economic layer that does not simply reuse Sitka assortment boundaries or prices (D-062); player-facing inspection and teaching parity; and integrated multi-species verification. Comparable depth does not mean equal empirical precision: evidence quality may differ between species where labelled honestly, and no pseudo-empirical coefficients are to be manufactured.

**Supersession note.** Earlier integration records in this document state that "S1-C/S1-D are not Stage-A prerequisites" and describe a beginner Stage-A first-cycle pilot as the next external step. Those statements are **superseded by D-060**. The first external demo is the complete Scenario One; S1-C and S1-D are pre-demo requirements. The historical wording above is retained unchanged as the record of what each integration reported at its own time.

## Current Oak/Beech adult model (PARITY-04 documentation alignment, 2026-10-27)

Source-level (static) facts about the production tree now on `main`, established read-only by the PARITY-04 preflight (`Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-04-Preflight/HANDOFF.md`; candidate `e1515e165eb40b40b5200992398984d69e0e8e8e`, production tree identical to `c6342f7` and byte-identical on `main`) and re-read on `0a313fa`. **This is documentation alignment, not Unity qualification:** final `ADULT SPECIES PARITY REQUALIFIED` waits on PARITY-03 Unity qualification and a later focused PARITY-04 Unity run of the strengthened `BroadleafAdultGrowthVerification`.

| Species | Pd cm/yr | Ph m/yr | Dmax cm | Hmax m | Ci50 | Growing-space class |
|---|---:|---:|---:|---:|---:|---|
| Oak (`SessileOak.asset`) | 0.8 | 0.65 | 120 | 42.5 | 5 | High (weight 2.0) |
| Beech (`BeechSpecies.asset`) | 0.9 | 0.75 | 120 | 42.5 | 6.5 | Intermediate (weight 1.5) |
| Sitka (`SitkaSpruce.asset`, protected comparator) | 1.2 | 0.45 | 120 | 35 | 5 | Reference (weight 1.0) |

- **Evidence class.** Every Oak/Beech value above is `GAMEPLAY CALIBRATION` (asset tag `[C]`): the search-grid centre of the pre-registered Oak/Beech calibration (candidates Oak-122 / Beech-122, `Docs/Verification/Calibration/OakBeechProductionCalibrationV1Handoff.md`), not a fitted or measured Irish broadleaf growth constant. The class weights are `GAMEPLAY CALIBRATION`; the class ordering is `ORDINAL REPRESENTATION`, and Oak > Beech is a weak, provisional ordering, not empirical precision (PARITY-02 U-01). The class is read only by the growth-model-2 density route.
- **Adult growth law** (`ForestEcologyController.GrowAdults`). DBH growth = `Pd × site × clamp01(1 − DBH/Dmax) × 1/(1 + CI/Ci50)`; height growth = `Ph × site × clamp01(1 − H/Hmax)` with no competition term. Oak and Beech use this shared legacy asymptotic law; Sitka's Class III age/site height envelope applies only to `sitka-spruce`. `site` is `cell.SiteProductivity`, which is 1.0 in production (never assigned outside verification harnesses).
- **Site response is neutral and dormant.** The species×site seam is exactly 1.0 for every species and site and is referenced by no production file. Scenario One's site is a versioned compatibility reference, not a soil survey or an "optimal" site. The qualitative drought/waterlogging directions recorded for Beech are ordinal data with no numerical effect: no species-specific drought or waterlogging penalty is modelled in P0.
- **Canopy/light is shared.** `RecomputeCanopy` has no species term, and there is no transmissivity, deciduous or leaf-off model. Species differ only through crown geometry (`PotentialCrownRadiusM`: Oak linear `0.10 + 0.1045·DBH`, Beech `ln CPA = 0.05 + 1.01·ln DBH`), crown relaxation (Oak 0.12, Beech 0.15, Sitka 0.15 per year) and `Ci50` in the crown target.
- **Competition neighbourhood.** One shared 8 m Hegyi cutoff (`HegyiCutoffMeters`), a Sitka-calibrated gameplay calibration with no broadleaf-specific radius evidence. Rack-owned `ScenarioOneRackBay.FutureTreeSpacingM` is defined as the same constant.
- **`Ci50` roles.** Species `Ci50` sets the DBH growth response, the crown target and the suppression diagnostics. Under growth model 2's mixed/species-aware density route, victim selection is species-neutral (one common half-saturation 5 and exponent 4) and does not use species `Ci50`; models 0/1 keep their protected legacy behaviour and a model-2 pure-Sitka stand keeps the exact Sitka route (Manager decision M-4).
- **Accepted Beech height-band miss.** From a promoted 3.5 m stem (age about 10) the shared law gives Oak 21.45 / 28.18 / 32.76 m and Beech 23.37 / 30.24 / 34.65 m at ages 50 / 75 / 100 (arithmetic derivation from the production recurrence, not simulation output). Against the provisional `[C]` bands used in calibration, Oak sits inside 20–25 m (age 50) and 30–35 m (age 100), while Beech is 1.63 m below 25–30 m and 0.35 m below 35–40 m. This follows arithmetically from the accepted constants and is an **accepted provisional calibration limitation** (PARITY-02 U-11), not an empirical failure or an implementation defect. It is not to be retuned or "fixed" with a new height model without a new evidence/calibration packet. The strengthened `BroadleafAdultGrowthVerification` prints these heights as diagnostics only, never as pass/fail targets.
- **Dated history stays dated.** `Docs/EcologyCalibrationAdoption.md` records the 2026-10-02 C8 adoption (Beech/Oak then kept Ci50 3 and 0.4 cm/yr) and is not current implementation truth.

## Current early-life and browse model (PARITY-05 documentation alignment, 2026-10-28)

Source-level (static) facts about the production tree on `main`, established read-only by the PARITY-05 preflight (`Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-05-Preflight/MANAGER-REVIEW.md`) and re-read on `86bd5c7`. **This is documentation alignment, not Unity qualification:** no value, formula, asset, save field or anchor changed, and final `EARLY-LIFE SPECIES PARITY REQUALIFIED` waits on a separate focused Unity qualification. Patch details: `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-05/HANDOFF.md`.

- **Oak** keeps distinct juvenile establishment and survival curves (`useDistinctJuvenileLightResponses: 1`), initial height 0.18 m and a 20% promotion-light floor.
- **Beech** has no distinct curve: it uses the combined legacy establishment/survival response (survival exactly 1.0 at or above light-response 0.15, 0.8 below), labelled `FALLBACK-CONSERVATIVE`. No Beech-specific curve was invented. **Sitka** uses the same combined response and remains the protected lifecycle comparator; its behaviour and anchors are unchanged.
- **Shared juvenile calibration (`GAMEPLAY CALIBRATION`)** is identical for all three species: 0.35 m/year full-light height growth, 3.5 m promotion height, 0.5 density per establishment, 1.5 density cap. Species differences enter only through seed dispersal/maturity/mast/saturation, juvenile light response, initial height and Oak's promotion-light floor. No annual establishment-rate or establishment-survival scalar exists.
- **Planting stock** (age 3 years, 0.6 m, density 1) is one shared **broadleaf stock abstraction v1**, not a Beech measurement. The Inspector header and tooltips on `ForestEcologyController` still say "Planting (Beech v1)"; that stale production comment is a recorded follow-up, not changed here.
- **Browsing.** `browsePalatability`: Sitka 0.15, Beech 0.5, Oak 1.0; ordering Oak > Beech > Sitka is `TRANSFER`, the numbers are gameplay-calibrated. Protection is binary access (intact fence or effective shelter 0, breached/outside 1) and protected outcomes equal the browse-free baseline. Scenario One's `0.2` is background browse pressure, **not** a protection multiplier. Packet-only early-life rates (`0.014/0.62/0.93` etc.), browse multipliers `1.0/1.8/2.2` and a `0.2` protected multiplier are not adopted and do not exist in the model.
- **Open follow-up (PARITY-08, not fixed here):** loading a save that names an unknown species falls back to the default species or skips the record instead of rejecting before world mutation.

[exit 0]

$ cat Docs/Project/coordination/active-tasks.md
# Active Task Coordination

This file is the **live single-writer ownership and execution-lock authority** outside the project context snapshot. Completed historical work belongs in Git history, handoffs, Manager reviews and `Docs/Project/unity-project-overview.md` rather than remaining as a live lock.

**Last reconciled:** 2026-10-29 by Overall Manager / GPT-5.6 Sol after MFM-06 #113 integration and browser #120 readiness approval.

## Coordination rules

- Overall Manager or an explicitly assigned integrator maintains this file.
- Every worker with GitHub access refreshes this file and its task claim at substantial task start and before claiming a lock is missing.
- One writable lane/worktree per owner and one primary writer per subsystem/file set.
- Never stash, reset, discard, clean, overwrite or absorb unrelated dirty work.
- If file/subsystem ownership overlaps, stop before editing and return to the Manager for ownership/API disposition.
- A GitHub task claim records execution presence; it **does not grant ownership** and never overrides this file.
- Heavy Unity execution is serialized machine-wide. Verify candidate HEAD, branch/worktree and ownership before acquiring the Unity lock. Never bypass another Unity owner or kill its process.
- No Windows build before the user personally playtests the Scenario One candidate and explicitly authorises it.

## Current integration truth

MFM-06 #113 is **HANDED-OFF / CLOSED — INTEGRATED / VERIFIED for its bounded gate**.

- Pre-integration umbrella: `ac876a64969752a3864034521f42559671264b30`.
- Exact reviewed MFM-06 input: `13fd85452b8d3a55946fcb459af8b00e0326a579`.
- Exact merged and Unity-qualified production umbrella: **`b971f4da0540e606bfee9702b0c99f81a0a89355`**.
- Published `integration/scenario-one-player-test-ready-v1` head: **`7608f02cb8da1fabd8b78fd5aaf45b0c32086f67`**, a documentation-only successor of the tested production merge.
- Integration handoff: `Docs/Handoffs/MFM-06/INTEGRATION-HANDOFF.md`.
- Save schema is v23. Frozen Reference and protected hash compatibility passed without rebaseline.

Browser #120 reviewed #113 and returned **APPROVED — MFM-07 READY FOR CODE EXECUTION**. #120 is closed/released.

## Immediate execution order

1. **#121 MFM-07 — proposal state and approval boundary** is **DISPATCHED / RELEASED**, not yet assumed RUNNING. Production work must root from exact qualified candidate `b971f4da0540e606bfee9702b0c99f81a0a89355` unless a later Manager-approved docs-only descendant is explicitly selected.
2. Before edits, #121 refreshes this file, #91/#106/#113/#120/#121, worktrees, dirty state and Unity lock; then marks itself `[RUNNING]` with actual model/provider, branch, worktree and starting HEAD.
3. **Claude Code is authorised. Preferred route: Claude Sonnet 5.5 Ultra Code** for this 5/5 task, retaining one primary production writer. Escalate to Opus 5.5 Low only if architecture/conflict reasoning requires it.
4. No downstream task gains production ownership merely because a packet exists.

There is no standing assertion here that the Unity lock is free. The active worker must refresh lock/process/free-space state immediately before Unity.

## Live tasks / reserved lanes

| Task | Primary / lane | Branch / worktree | Owned files or subsystem while active | Restrictions / dependencies | Live status |
|---|---|---|---|---|---|
| **PLAYER-TEST-OVERNIGHT-01** (#91) | DeepSeek orchestration/integration lead | `orchestration/scenario-one-player-test-ready-v1`; production stays on child lanes | Claims, dependency DAG, handoff triage, integration decisions, final Tier-3 assembly | Preserve child ownership; no product invention; no Windows build | **RUNNING** |
| **PLAYER-TEST-T3-CODE-LANE-01** (#106) | Local C#/Unity execution umbrella | Child worktrees only | Local implementation/qualification through child claims | Exact candidate/handoff required; Unity serialized | **RUNNING** |
| **MFM-07 proposal/approval boundary** (#121) | Claude Code; Sonnet 5.5 Ultra Code preferred | `feature/mfm-v1-proposal-approval-boundary-v1`; suggested `/media/jer/Videos/CCF-MFM-07` | Smallest proposal → validated pending-work surface required by MFM-07 | Root from `b971f4d`; no schema bump/save-validator edit; no MFM-08/09; no broadleaf quantitative extrapolation | **[RUNNING]** 2026-10-29 — Claude Code / Anthropic Claude Sonnet 5.5 (`claude-sonnet-5-5`); branch `feature/mfm-v1-proposal-approval-boundary-v1`; worktree `/media/jer/Videos/CCF-MFM-07`; starting HEAD `b971f4da0540e606bfee9702b0c99f81a0a89355` |
| **PARITY-03 Q10 evidence strengthening** (#74) | Waiting verifier when released | existing verification lane | Verification only | Non-blocking evidence strengthening | **WAITING** |

## MFM-07 single-writer boundary

Claim #121 and `Docs/Handoffs/MFM-07/TASK-PACKET.md` are the current execution authority.

When #121 starts, its writer owns only the smallest proposal→pending-work integration surface required by the published MFM-06 API:

- proposals remain separate from `TreeMarkType.Fell` and ordinary manual marks;
- prescription approval creates pending ordinary work only;
- Work Plan approval remains a separate commitment gate;
- manual player state wins conflicts and conflicts are surfaced rather than overwritten;
- **before each prescription-generated order is written, its referenced prescription, coverage unit and Crop Tree must validate; mismatch/staleness fails closed with no partial order/world mutation**;
- MFM-06 v23 IDs/provenance are consumed, not duplicated;
- no schema/version bump or save-validator edit is authorised; if required, STOP and return to Manager;
- no broadleaf quantitative extrapolation.

Likely hot/shared surfaces include `ScenarioOneManager`, ordinary work-order creation APIs and Work Plan integration hooks. Exact files must be recorded before edits. Any overlap with another active writer means stop before editing.

## Downstream sequencing — prepared only, no ownership yet

### Save / validation

- **PARITY-08** may start only under a separate live claim after MFM-06 integration. It owns save validation/determinism while active. Do not overlap it with any unexpected MFM-07 save-path edit.
- Final Tier-3 reruns the combined save/history chain on one exact final candidate.

### S1-C review/history

- **S1-C T-04**: provenance labelling on Annual Review/history.
- **S1-C T-06**: prior-treatment guidance/history navigation.
- **S1-C T-07**: stable identity/history contract verification.
- **S1-C T-08**: feature qualification after T-04/T-06/T-07.
- These may run beside MFM-07 only with explicitly non-overlapping live files. Shared `ScenarioOneManager`, save-validation, work-order or history-helper edits force serialization/Manager review.

### S1-D / teaching / Work Plan

- **S1-D T-04** owns teaching-sequence/current-state files and must land before MFM-09.
- **S1-D T-05** waits until the MFM-07 pending-work/provenance API is stable; it then owns the bounded previous-versus-proposed Work Plan presentation seam.
- **S1-D T-08** waits for T-02..T-06 acceptance.
- **S1-D T-09** waits for T-08 PASS and is independent review only.

### MFM continuation

- **MFM-08** follows accepted/integrated MFM-07 and owns bounded map/3D candidate/proposal review UX after overlapping writers are released.
- **MFM-09** follows MFM-07/08 plus S1-D T-04/T-05.
- **MFM-10** follows MFM-09 + PARITY-08 and retains the Sitka-only species guard.
- **MFM-11** follows stable parity/repeated-intervention/player-facing flow.
- **MFM-12** follows MFM-11 plus parity/S1-C/S1-D qualification and provides integrated MFM verification/review.

### Three-species parity

- **PARITY-08**: save/deterministic compatibility after MFM-06, subject to single-writer serialization above.
- **PARITY-09**: integrated parity qualification after PARITY-08 + MFM-10.

### Programme endgame

- Final Tier-3: `Docs/Handoffs/PlayerTestReadiness-Overnight-01/FINAL-TIER3-TASK-PACKET.md` — one exact final candidate; mandatory fresh S1-C T-05 save/history rerun.
- Final independent review: `Docs/Handoffs/PlayerTestReadiness-Overnight-01/FINAL-INDEPENDENT-REVIEW-TASK-PACKET.md`.
- User-playtest template: `Docs/Handoffs/PlayerTestReadiness-Overnight-01/FINAL-HANDOFF-TEMPLATE.md` — template only until final qualification passes.

## Released/completed claims relevant to current state

These own no files: #103 S1-D T-03; #104 S1-C T-05; #93 MFM-04; #100 MFM-05; #102 PARITY-07; #111 MFM-06 implementation; #112 superseded review; #113 MFM-06 integration; #118 browser lane 03; #120 browser lane 04.

## Non-dispatched backlog

Issue #28, **Verification harness hygiene — standalone body binding and site-seam allow-list**, remains backlog only and blocks no current feature.

[exit 0]

$ gh api repos/jermur788/CCF/contents/Docs/Project/coordination/active-tasks.md --jq .content | base64 -d
# Active Task Coordination

This file is the **live single-writer ownership and execution-lock authority** outside the project context snapshot. Completed history belongs in Git history, handoffs, Manager reviews and `Docs/Project/unity-project-overview.md`.

**Last reconciled:** 2026-11-04 by Overall Manager after user's overnight DeepSeek orchestration release #254 and six new child #255–#260. Disjoint production file sets, docs-only specs and verification ownership reserved conditionally; all currently READY (not running). #200 remains sole live coordination writer; no new Unity lease or build.

## Coordination rules

- Overall Manager or assigned integrator maintains this file.
- **#200 is the sole primary writer for this file during the current two-lane split.**
- Workers refresh this file and their child claim before asserting ownership/lock availability.
- One writable lane/worktree per owner and one primary writer per subsystem/file set.
- Never stash, reset, discard, clean, overwrite or absorb unrelated dirty work.
- Heavy Unity is serialized machine-wide; a historical `free` receipt proves nothing about current availability.
- Claims track execution presence only; they do not grant ownership.
- No Windows build before the user's personal Scenario One playtest and explicit post-playtest authorisation.
- After meaningful state changes, #200 refreshes `jermur788/Overall-Dev/updates/ccf/latest.md`; CCF remains canonical.

## Current integration truth

The published Scenario One integration on `integration/scenario-one-player-test-ready-v1` is now the **final player-test candidate**. It includes the exact Manager-approved #232 Stand Map species-canopy differentiation by a lease-checked non-forced fast-forward from the exact Tier-3-approved base to the already-qualified direct-child production commit.

Accepted current-lineage state includes Rack V2 / D-064 VERIFIED; Sitka/Oak/Beech parity P0 VERIFIED; S1-C review/history accepted; S1-D repeated-intervention qualification accepted; MFM-08/09 integrated; MFM-10 `SPECIES GUARD QUALIFIED`; MFM-11 `MULTI-SCALE CALIBRATION QUALIFIED`; MFM-12 `MULTI-SCALE FORESTRY MANAGEMENT V1 — INTEGRATED / READY FOR USER PLAYTEST`; #199 Combined HUD/Focus approved; #223 verification debt approved/integrated; #224 Objectives overflow approved/integrated; #230 fit-replay repair approved/integrated; final Tier-3 #231/#240 approved; and #232 Stand Map species-canopy differentiation **APPROVED / INTEGRATED**.

MFM-11 boundary remains explicit: extension-state calibration is within accepted GAMEPLAY CALIBRATION tolerance; post-treatment source-vs-current-Covered CI parity remains **NOT DEMONSTRATED**. The three pure-Beech reduced-simulator R2 misses remain accepted C-level limitations against the unchanged 0.40 criterion.

## Final Tier-3 and #232 delta

#231 Scenario One final Tier-3 qualification and #240 evidence closure are **APPROVED / CLOSED**. #232 then changed only procedural Stand Map canopy presentation plus durable Stand Map verification assertions.

#232 affected verification accepted by Manager:

- offline Stand Map checks 176/0;
- mixed-species before/after full-map rendering at 1280×720 and 1920×1080;
- unchanged 1,416-tree map-model fingerprint and simulation WorldHash;
- Combined HUD/minimap 465/0;
- Rack map-planning PASS;
- Rack confirm/minimap PASS;
- PARITY-07 rendered PASS;
- compile PASS;
- Scenario One completion PASS with the accepted completion fingerprint;
- no material redraw/performance regression;
- fresh cross-provider OpenAI read-only review: **APPROVED**.

Manager review:

`Docs/Handoffs/StandMap-Species-Canopy-Differentiation/MANAGER-REVIEW-20261103.md`

Integration handoff:

`Docs/Handoffs/StandMap-Species-Canopy-Differentiation/INTEGRATION-HANDOFF.md`

Because the integration is the exact already-qualified direct-child commit on its original parent, no Unity or rendered/Tier-3 rerun is required solely for integration.

## Live / released tasks

| Task | Lane | Ownership | Status |
|---|---|---|---|
| **Browser / GitHub Manager Lane 03** (#200) | browser/GitHub | Manager review, releases, coordination | **RUNNING — UMBRELLA** |
| **Local Deep Orchestration Resume 02** (#203) | local orchestration | local routing + housekeeping | **RUNNING** |
| **Scenario One final Tier-3 qualification** (#231) | #203 local | parent milestone qualification | **APPROVED / CLOSED** |
| **Tier-3 Task 2 evidence closure** (#240) | #203 local | three evidence actions + delta review | **APPROVED / CLOSED** |
| **Stand Map species canopy differentiation** (#232) | #203 local / Unity | bounded player-facing map presentation | **APPROVED / INTEGRATED** |
| **#232 final integration** (#242) | browser/GitHub | exact fast-forward publication | **INTEGRATED / CLOSE** |
| **PARITY-03 Q10** (#74) | #203 when explicitly scheduled | non-blocking verification | **WAITING — NON-BLOCKING** |
| **Astra onboarding visual refresh** (#243) | Original isolated feature and verification lanes complete | Manager-approved and independently reviewed intro presentation, staged separately; formal 288/289 FAIL retained with specific waiver | **APPROVED / STAGED / LEASE RELEASED / CLOSED.** Original owner posted a normal release and FREE status receipt at 21:09 UTC; no current machine state is inferred from it. |
| **Independent intro visuals / exception review** (#244) | Claude Sonnet 5.5 Medium, read-only | Inspected both logs, all six raw screenshots, sources and visual fidelity; narrow waiver recommended | **APPROVED / CLOSED**; formal gate remains FAIL, review not a Unity rerun. |
| **Isolated intro refresh staging** (#245) | browser/GitHub Manager, no local Unity | Exact Git ref of independently reviewed #243 candidate, separate from active playtest lineage | **STAGED / CLOSED** — `integration/scenario-one-intro-visual-refresh-v1` published at reviewed feature tip; existing `integration/scenario-one-player-test-ready-v1` unchanged. |
| **Access spine / diagonal rack reconnaissance** (#246) | DPC, Claude Sonnet 5.5 Medium source audit + independent GPT-6.1 Terra Medium verification | Read-only inspected published player-test candidate; V2 ledger 4 events | **APPROVED / CLOSED — RESEARCH ONLY.** 13 PASS, 8 corrected, 1 FAIL original MFM kind taxonomy recorded/corrected before publication. No Unity/layout fit proof and no spec approval. |
| **Access spine geometry/MFM measurement** (#248) | DeepSeek DPC / isolated Unity measurement lane, historical lease released | Corrected curved-spine geometry and MFM mask sensitivity, cross-provider review preserved, docs-only PR #249 merged | **APPROVED / CLOSED AS RESEARCH ONLY.** No implementation, no feature/runtime acceptance. |
| **Full-spine 45° five-rack reference proposal** (#250) | DeepSeek DPC / original isolated measurement lane, historical lease released | 3W+2E five-rack reference, 45° spatial analysis and independent review | **APPROVED/CLOSED AS EVIDENCE, STOP IMPLEMENTATION.** Derive rejects this candidate's common-normal rack order; workbench conflict, headland-only entrances, corridor overlap, impacts and renders remain unresolved. |
| **MFM nonparallel rack sample/coverage options** (#251) | DeepSeek DPC, Claude Sonnet 5.5 Medium source study + GPT-6.1 Medium independent review | Compared side-local, network/graph and parallel fallback; lifecycle validity gap and save/hash semantics documented | **APPROVED / CLOSED AS CORRECTED RESEARCH.** Docs-only PR #252 merged; original review 28 PASS / 8 corrected / 4 FAIL, fully disclosed; no MFM change or spec approval. |
| **Offline Option A sample/coverage and rack validity qualification** (#253) | DeepSeek DPC; source-only candidate and conditional separate docs/evidence lane `docs/mfm-option-a-offline-20261104-v1` (must verify unused); Sonnet 5.5 Medium or GPT-6.1 Medium based on actual availability | **READY / NOT RUNNING — OFFLINE DESIGN/PROPERTY TEST ONLY.** Deterministic unique ownership, conservative reachable coverage, sample transfer, Confirm+confirmed-mutation+invalid-load guards, version/hash compatibility, cross-provider review | **NO UNITY, NO PRODUCTION WRITER.** Binding `Docs/Handoffs/ScenarioOne-Access-Spine-20261103/OPTION-A-OFFLINE-QUALIFICATION-PACKET.md`. D-064/D-065 controlling; four-file new spec DRAFT/STOP. |

| **Overnight DPC orchestration** (#254) | DeepSeek DPC parent, #203 local runner; provider pool only as observed | Sole writer of `Docs/Handoffs/Overnight-Continuation-20261104/ROUTING-LEDGER.md`, final report; delegates child scopes, no shared production writer | **READY / NOT RUNNING**. Sonnet 5.5 / GPT-6.1 Medium first; available subs/higher ticket/Opus 5.5 may be used after logged justification. Distinct user/Manager approvals and Unity lock remain binding. |
| **Objective copy and tree-mark introduction** (#255) | Conditional `feature/overnight-objective-copy-20261104` / `CCF-Overnight-ObjectiveCopy-20261104` | **Exclusive** `Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs`, `MenuHelpView.Text.cs` and own tests | **READY / PACKET ONLY.** Correct 16–20m, header and intro language; do not falsely relabel actual painted marks or claim formal Pro Silva standard. PR/handoff and independent review required; no integration authorisation. |
| **Rack round-handle free-rotation fix** (#256) | Conditional `feature/overnight-rack-handle-20261104` / `CCF-Overnight-RackHandle-20261104` | **Exclusive** `Assets/ForestPrototype/UI/StandMapView.cs`, only required local rack UI helper(s); no ownership of authoritative Rack V2 policy | **READY / PACKET ONLY.** Rotation handle can bypass snap without Shift; ordinary snapping preserved; stop on product conflict. Affected input proof; serial Unity admission only. |
| **Mouse sensitivity/keybind/tester feedback spec** (#257) | Conditional docs-only `docs/overnight-input-settings-spec-20261104` / `CCF-Overnight-InputSpec-20261104` | Exclusive DRAFT `specs/player-input-settings-and-feedback-v1/` and own handoff | **READY / SPEC DRAFT ONLY.** No production or tester telemetry before approved permission/persistence/consent policy. |
| **Objective next-action/map-like teaching spec** (#258) | Conditional docs-only `docs/overnight-objective-guidance-spec-20261104` / `CCF-Overnight-ObjectiveSpec-20261104` | Exclusive DRAFT `specs/scenario-one-next-action-guidance-v1/` and own handoff | **READY / SPEC DRAFT ONLY.** Contextual Stand Map then Plan Racks CTA, faithful illustration; no UI production change. |
| **Tree mark visual language/Pro Silva evidence spec** (#259) | Conditional docs-only `docs/overnight-mark-language-spec-20261104` / `CCF-Overnight-MarkSpec-20261104` | Exclusive DRAFT `specs/scenario-one-tree-mark-language-v1/` and own handoff | **READY / SPEC/RESEARCH ONLY.** Reconcile existing orange rack stripes / red Fell / blue Crop with user-requested orange diagonal Fell + blue ring; no misleading product copy. |
| **Playtest feedback static baseline verification** (#260) | Conditional `verify/overnight-playtest-feedback-20261104` / `CCF-Overnight-FeedbackVerify-20261104` | Exclusive new `Tools/Verification/OvernightPlaytestFeedback/` and own handoff | **READY / OFFLINE VERIFICATION ONLY.** Do not edit source/approved tests or claim Unity-rendered verification from static checks. |

Housekeeping: browser tooling created temporary remote refs `tmp-noop-integration-check` and `noop`, both pointing at the integrated candidate. They do not affect product or candidate state. Local orchestration should delete them when convenient; browser connector has no branch-delete action.

## Astra #243 initial ownership and Unity lease boundary — earlier 2026-11-03 preflight (historical; superseded by latest return above)

- Manager #200 has **assigned exclusive onboarding production-file ownership to Astra #243** for the file set listed in the live table above. Other lanes must not edit those files while the claim remains active; this is ownership assignment, not proof that a worktree exists.
- The local branch/worktree names are authorised for isolated use **only if available**. Production must branch from the published player-test base, never write to `main` or the published integration checkout; preserve unrelated/untracked `Scenario1V1/` content reported in an existing checkout.
- **Unity slot:** the Manager authorises #243 as the next bounded Unity user for this task, but cannot confirm the machine is idle remotely. A real Editor/process/lock/user-playtest preflight and machine-wide lease acquisition by the local executor are mandatory before Unity starts. If the user's personal Unity playtest or another owner is active, remain waiting rather than claiming/replacing the lease. No competing Unity use permitted.
- Astra's latest claim comment reports the execution chat is attached to a **read-only Game Dev reference mirror**, not the real CCF repository/workspace. This is an environment blocker independent of file ownership and user approval. Reopen/attach the real CCF repository as a writable code-workspace root in the Astra environment before attempting managed `create_worktree`; do not bypass the workspace system or modify the existing checkout.
- Latest user-approved three images **in Astra's own execution chat** are reported to differ from earlier handoff ZIP hashes. Per latest direct user direction they are the visual source; obtain hashes from actual local originals and preserve both provenance sets. Added Page 1 sentence stays byte-exact and teal/blue.
- No Windows build, Player build, accepted-test rebaseline, or merge is permitted from this assignment; existing approved Tier-3, #232 and personal playtest remain intact.

## #243 returned onboarding implementation — earlier Manager checkpoint 2026-11-03 (HISTORICAL, superseded below)

The user-approved 3-page intro artwork and copy have been implemented on the isolated feature branch; **not integrated** into the published player-test candidate. Handoff: `Docs/Handoffs/ScenarioOne-Intro-Astra-Visual-Refresh-20261103/IMPLEMENTATION-HANDOFF.md` (feature branch). Returned evidence: two focused Unity Editor runs, **288 checks passed and 1 failed** each, 6 screenshots each at 1280×720 and 1920×1080. The single `NO_EXCEPTIONS_OR_ERRORS` failure reports a UnityEditor.Search indexer `ArgumentOutOfRangeException` at Editor startup. The formal gate result remains **FAIL** (exit 1); no current product defect is demonstrated but environmental causation is not independently established. Screenshot visual quality is worker-reviewed only; Browser Manager has not directly inspected the externally stored raw captures.

**Manager verdict: NEEDS MORE EVIDENCE**, not an approval. Select an **exact-exception documented waiver pathway** only if independent cross-provider reviewer #244 confirms both raw traces are Unity Editor startup-only, no CCF runtime effect or other errors exist, and all six real Unity renders satisfy accepted visual requirements. Do **not** filter the test, label the original gate green, or rerun a full accepted test matrix. Packet: `Docs/Handoffs/ScenarioOne-Intro-Astra-Visual-Refresh-20261103/REVIEW-PACKET-20261103.md`.

**Operational block:** #243 worker reports lock remains held under its own owner ID and the lane is clean; Manager has not independently verified machine state. Only that **same owner**, after checking the deployed lock service/Editor processes and matching exact lease, may release the lock. No remote forced unlock, process kill, arbitrary cleanup, or fresh Unity while ZX20 is under its 15 GiB free-space floor (reported ~14 GB). Record local release proof on #243. Review #244 can run without Unity.

The previously issued player-test-ready token applies solely to the already-published integration candidate, unchanged by this unintegrated feature. User personal playtest is **in progress**, not accepted. No Windows build until personal playtest completion and explicit user authorisation.

## #243 final Manager disposition, #244 review and #245 staging — 2026-11-03

**Manager APPROVED** the bounded three-page onboarding visuals and copy, with a narrowly documented acceptance **waiver** for the exact Unity Editor Search indexer startup exception. The original **two Unity gates remain formally FAIL** (288/289, `ONBOARDING_FAIL`, exit 1). No test assertion/filter changed; no additional Unity run. The independent reviewer opened all six real rendered images at 1280×720 / 1920×1080 and verified Page 1's teal sentence and gradual retained-tree visual, Page 2 icons/halo/text fit, and Page 3 several mature Sitka. Minor accepted visual notes: Page 1 art ~10% vertical compression, Page 2 card 4 ~10 px right margin. Exact provenance uses the newer user-approved originals recorded in the 3 Nov art manifest; older ZIP hashes remain history. The error occurred **after CCF Awake/Start diagnostics but before the first onboarding check**, correcting an inaccurate implementation handoff phrase.

Mainline review: `Docs/Handoffs/ScenarioOne-Intro-Astra-Visual-Refresh-20261103/MANAGER-FINAL-REVIEW-20261103.md`. Independent review on `review/scenario-one-intro-astra-independent-20261103`: `INDEPENDENT-REVIEW-20261103.md`.

#245 published a **separate, exact reviewed Git ref** `integration/scenario-one-intro-visual-refresh-v1`, without file reconstruction, local Unity, test rerun or any change to the user's current `integration/scenario-one-player-test-ready-v1` baseline. Mainline staging handoff: `Docs/Handoffs/ScenarioOne-Intro-Astra-Visual-Refresh-20261103/STAGING-INTEGRATION-HANDOFF.md`. **The user has not completed or accepted their personal playtest.** Do not silently switch their test branch; coordinate a transition before promotion.

**Local safety outstanding:** #243's original owner is the only party authorised to check the machine-wide held Unity lease and release its own exact matching lock after confirming no Editor processes; request fresh FREE receipt on #243. No forced unlock. The independent reviewer observed about 28 GB free on ZX20, superseding the earlier ~14 GB report, but it is a point-in-time receipt, not a substitute for a new admission check. No Unity run is requested now.

No Windows/Player build before personal playtest is complete and explicit post-playtest authorisation; no Reference Future v1/Legacy40, MFM or simulation changes from #243. #194 NO ACTION.

## Access-spine direction and DPC orchestration — 2026-11-03

The user's continuing Scenario One playtest has accepted a **conceptual** access spine: extend the existing central route where viable, connect diagonal racks, and keep 3D, Stand Map, minimap and marking consistent. Decision Log **D-067** records preferred direction but does **not** change D-064's approved Rack V2 plan. The user explicitly permits **Claude Sonnet 5.5 Medium** and **OpenAI GPT-6.1 Medium** as workers under **DPC** orchestration. No claim of current provider availability is made.

The **four-file DRAFT** spec is `specs/scenario-one-access-spine-route-consistency-v1/`. #246 is a read-only T3 code/scene reconnaissance only. Durable packet `Docs/Handoffs/ScenarioOne-Access-Spine-20261103/RECONNAISSANCE-PACKET.md` and binding V2 ledger `Docs/Handoffs/ScenarioOne-Access-Spine-20261103/ORCHESTRATION-LEDGER.md` are published. DPC must record actual provider capacity/window/reset and context usage, alternatives, routing reasons, switches, tokens/runtime when observable, findings and outcomes **as they occur**, including a V2 Orchestration Decision Appendix in the return. Unknown remains UNKNOWN. The historical 30 Oct ledger and separate #241 backfill are outside this ledger's ownership.

To keep development moving, DPC can hand off and switch among **available, permitted Medium workers** or route a separately authorised, non-overlapping READY task while one task is blocked. It cannot implement DRAFT specs, bypass exclusive writer/Unity lock, override user testing or publish integration without Manager review. #200 remains sole live coordination writer. No Windows build.

## #246 Manager review and next blocked #248 — 2026-11-03

#246 source reconnaissance returned on main through merged docs-only PR #247 and is **Manager APPROVED/CLOSED as research only**. The independent 22-claim verification found 13 PASS, 8 corrected claims and one original **FAIL (MFM Track/Headland taxonomy)**; its published correction is retained, not relabelled. The DPC V2 ledger has four observed events; provider quotas/context remain UNKNOWN and no switch was fabricated.

**Critical derived risk:** creating more `Dirt Path*` planks automatically adds MFM `MaskKind.Track` treatment-exclusion membership; feeder/headland use `MaskKind.Headland`, also excluded. A1 path extension is **not cosmetic-only** and cannot be dispatched without quantified MFM impact. The central access-spine with diagonal racks remains preferred but **geometry unmeasured**; existing Rack V2/D-064 controls. The four-file new spec remains DRAFT.

Manager review: `Docs/Handoffs/ScenarioOne-Access-Spine-20261103/MANAGER-REVIEW-20261103.md`. New #248 real Unity geometry/mask measurement packet is prepared but **BLOCKED**, since exact running playtest candidate, safe Unity slot, original #243 lock release and Manager-assigned exclusive lane are not yet established. Do not grant #248 file ownership or claim runtime PASS. DPC may progress only independent authorised tasks without overlapping locks or writer scopes. The user's personal playtest remains ongoing; no Windows build authorisation.

## User frees Unity occupancy — #248 conditional local release, 2026-11-03 21:00 UTC

**Direct user confirmation:** nobody is using Unity now; work may resume. This resolves the **live-user occupancy** restriction on #248. It **does not** establish that the earlier personal Scenario One test is completed or accepted, reveal the exact local candidate, or prove the machine-wide serialized Unity lease is FREE.

Overall Manager now releases **#248 LOCAL PREFLIGHT** and conditionally reserves `verify/scenario-one-access-spine-measure-20261103` at `/media/jer/ZX20/CCF-AccessSpine-Measure-20261103` from the published player-test candidate, with strong preconditions: name/worktree must be unused, actual base/candidate confirmed, ZX20 capacity sufficient for added Unity Library with post-creation >15 GiB floor, no overlapping owner, no active Editor/display processes. DPC routes the **original #243 lock owner** to safely release only its exact matching lease using deployed CCF-Ops and records a current FREE receipt; DPC then acquires a new exclusive #248 lease. No force-release or workaround. If safe conditions fail, #248 returns BLOCKED; no Unity may launch. If all pass, the worker marks [RUNNING] with exact provider/model Medium effort, branch/worktree/base/lease and does bounded read-only geometry and hypothetical MFM mask measurement. DPC appends contemporaneous V2 ledger events and obtains independent review.

**No draft-spec coding or A1/A2 production, Windows build, change to user-save or player-test branch.** The user's playtest is still unaccepted; completed personal testing plus a separate explicit authorisation are mandatory before a Windows build.

## #243 release receipt accepted — #248 fresh admission gate

The original holder of #243 posted a matching non-forced Unity lease release with a FREE post-check at 21:09 UTC. Manager verified the GitHub receipt and closed #243. This resolves the old lock-release obligation; it does not confirm future machine availability. #248 can begin local preflight immediately. DPC must verify current process, lock, disk, branch and ownership state, then acquire its own new exclusive lease before read-only scene/MFM measurement. The personal playtest is not yet complete. The separately staged intro and published player-test baseline stay unchanged. No production or Windows build.

## #250 STOP for implementation and #251 read-only MFM contract — 2026-11-03

Docs-only **PR #249 MERGED**, publishing corrected #248 and #250 handoffs plus V2 ledger events. #248 and #250 are **APPROVED/CLOSED as research only**. #250's full-spine 45°/3W+2E candidate meets the site-specific spacing guideline in the derived reference layout but **cannot be implemented** under current MFM rack-sample geometry: production `ScenarioOneRackSampleGeometry.Derive` returns `Valid=false` for projected ordering. Additional independent blockers: spine/Forestry Workbench conflict, headland-only entrance authoring, R1/R2 corridor overlap, one trunk-collider endpoint, unqualified turning/3D/map/minimap render. Union corridor tree impact **120 living trees** is a scenario consequence, never an approved removal. The Track vs Headland mask row mix-up was detected by independent review and corrected: full proposed network as Track = **156 newly masked, 111 lost treatable stems, 459.82 m² lost**; as Headland = **150, 106 and 448.03 m²**. These sensitivity figures are **not qualified MFM prescriptions** because the candidate's own sample derivation fails. #250 review tally is 7 PASS / 6 corrected / 2 FAIL.

Manager verdict: `Docs/Handoffs/ScenarioOne-Access-Spine-20261103/MANAGER-FIVE-RACK-REVIEW-20261103.md`; four-file access-spine spec remains **DRAFT / STOP FOR IMPLEMENTATION**. New [#251](https://github.com/jermur788/CCF/issues/251) is **READY read-only** to compare branch/side-local MFM coverage, generalized network/graph coverage and alternative parallel geometry retaining the existing model. DPC must publish a source-backed, independently reviewed `MFM-TOPOLOGY-OPTIONS-HANDOFF.md`, maintain the V2 ledger with publish-time raw-number re-extraction, and return for Manager/product decision before any production spec/implementation dispatch. Its own docs-only lane remains conditional on clean unused ownership; no Unity is required. Historical #250 lock release does not grant new Unity ownership.

The user's personal player test is not completed/accepted and its published branch and save are unchanged. **No Windows build** before completion and explicit subsequent user permission.

## #251 corrected research and conditional A design / #253 offline qualifier — 2026-11-04

The #251 read-only study and contemporaneous DPC V2 ledger are on main through **merged docs-only PR #252**. Source audit/independent review established a specific two-family common-normal failure for the 45°/3W+2E reference, and a separate **current Rack V2 validity lifecycle gap**: Confirm, direct `UpdateRack` on confirmed plans and save validation do not check MFM `Derive` validity. The review initially returned **MATERIAL ERROR** (40 claims, 28 PASS, 8 corrected, 4 FAIL): missing confirmed-mutation gate, hashes inferred incorrectly from unchanged schema, stable unit IDs mistaken for meaning, curved-boundary overclaim. All are disclosed/corrected; #251 is **APPROVED/CLOSED as research only**. No end-to-end irreversible player failure was proven.

Manager [D-069] records **Option A conditional design direction** only: additive branch/side-local derivation, original parallel derivation unchanged, deterministic one-owner tree/area allocation, conservatively verified reach, one demonstrated sample for both groups only as a *hypothesis*, and **Confirm + every authoritative confirmed-plan mutation** validity policy with old invalid saved plans loadable but unsafe new actions suspended while preserving completed history. Option C parallel fallback remains; generalized persisted graph B1 not justified at five-rack scope. The 9 m teaching half-width is **not** an approved reach cap, and the user's preferred 45° geometry is not an accepted executable layout. D-064/D-065 remain binding, as do separate legal spine entrance, yard bypass, corridor overlap, turning and missing render blockers.

Manager review: `Docs/Handoffs/ScenarioOne-Access-Spine-20261103/MANAGER-MFM-TOPOLOGY-REVIEW-20261104.md`. Four-file spine spec stays **DRAFT / STOP IMPLEMENTATION**. Next #253 is **READY for offline nonproduction analysis only**; no Unity or production owner. DeepSeek DPC must verify source/docs-lane ownership, monitor model limits, append V2 ledger and cross-provider review, source-reconcile all numbers and publish `OPTION-A-OFFLINE-QUALIFICATION-HANDOFF.md` before Manager spec approval. The established user-playtest branch and separately staged intro remain unchanged. User personal playtest not completed or accepted; **no Windows build** without explicit post-playtest authorisation.

## Overnight DeepSeek delegation release — 2026-11-04 00:07–00:08 UTC

**Direct user authority:** while user is asleep, DeepSeek DPC is authorised to orchestrate **serial and independent parallel** safe work with available provider subs and documented, justified escalation from Sonnet 5.5/GPT-6.1 Medium to higher thinking/ticket levels or Opus 5.5; it may checkpoint and wait on *observed* quota reset availability. This does **not** approve DRAFT features, override D-064/D-065/Reference Future, or finish the personal playtest. No Windows build.

**Binding master packet:** `Docs/Handoffs/Overnight-Continuation-20261104/MASTER-HANDOFF.md`; **append-only one-writer ledger:** `Docs/Handoffs/Overnight-Continuation-20261104/ROUTING-LEDGER.md` (DPC writer). DPC must record *each* material provider selection/delegation/context/capacity/effort escalation/wait or reset/retry/review/outcome as it happens, with exact evidence or UNKNOWN, before final morning handoff. Existing #253 access-spine programme ledger remains authoritative for #253 and is cross-linked. DPC issues worker claims/PRs per task; **#200 retains single-writer ownership here and integration/spec release verdicts**.

**Preassigned, disjoint scopes:** #255 UI objective/help copy (two C# files); #256 Stand Map rotation-handle UI (one C# file and strictly needed local UI helper); #260 new offline verification Tools subtree; #257 input/settings DRAFT spec; #258 objective/CTA DRAFT spec; #259 mark-colour/Pro Silva DRAFT spec; and already-READY #253 Option A MFM offline research. Every branch/worktree is **conditional on fresh safe owner/clean/path checks**; do not claim running, merge, Unity eligibility, model availability or free-space headroom from a static GitHub row. The user playtest candidate and staged intro remain unchanged.

**Implementation boundary:** only #255 and #256 are PACKET-authorised bounded production fixes. Future cross-system keybinding/consent/telemetry, colour-mark policy, guided objective workflow and branched-rack/MFM require approved specs. Code PRs need independent review and affected test evidence; no direct merge into protected playtest candidate while user is asleep. Unity remains globally serial with fresh lock/process/disk preflight and new exact owner/lease. Preserve existing saves, unrelated dirty work and frozen Reference Future v1/Legacy40.

## User / build gate

Browser Overall now issues:

`SCENARIO ONE PLAYER-TEST CANDIDATE READY — USER PLAYTEST REQUIRED`

The user must personally playtest the exact published `integration/scenario-one-player-test-ready-v1` candidate before any Windows build is authorised.

After the personal playtest, Browser Overall records the user-test result. A Windows build remains blocked until the user explicitly authorises it **after** that playtest.

## Hard boundaries

- Research Source Consolidation Audit #194 remains **NO ACTION**.
- Frozen Reference Future v1 and Legacy40 compatibility remain protected.
- Never weaken tests or silently rebaseline protected anchors.
- User-directed #238 test-volume reduction remains binding; do not rerun valid Tier-3 evidence without a new reason.
- The accepted stale economy v14 fixture remains non-blocking verification debt; it is not part of the player-test gate.

[exit 0]

$ gh issue view 256 --json title,body
{"body":"**Scope:** implement user's bounded interaction fix in Stand Map Rack V2: dragging a rack's *round rotation handle* must allow free rotation without also holding Shift to disable snapping. Preserve snapping for ordinary draw/move and explicit Shift override where unrelated; don't alter planned rack geometry validity, headland-only entrance enforcement, five-rack count, Work Plan gates or save. Update the confusing paired instruction and `16–20m` display in the Stand Map text if appropriate; no introduction of interior entrances/spine capability. Source candidate `integration/scenario-one-player-test-ready-v1`. Inspect UI event path before editing; if genuine conflict with D-064 or more than bounded handler change, STOP/spec escalation. \n\n**Difficulty Medium** — input event path plus focused interaction verification; worker GPT-6.1 Medium or Sonnet 5.5 Medium by live capacity. **Ownership** exclusive `Assets/ForestPrototype/UI/StandMapView.cs` and exact needed Rack editing UI helper(s) only; do not touch Objective Copy files or confirmed persistence/MFM. **Lane** `feature/overnight-rack-handle-20261104` / `/media/jer/Files/CCF/Operations/CCF-Overnight-RackHandle-20261104` if unoccupied/disk-safe. New exclusive Unity lease only after fresh local preflight; serialize heavy Unity with other workers. Return `Docs/Handoffs/Overnight-Continuation-20261104/RACK-HANDLE-HANDOFF.md`, focused before/after rendered/drag check, cross-provider review, no acceptance/merge inferred. No Windows build.","title":"[READY / PACKET ONLY] Rotate rack using drag handle without Shift"}

[exit 0]

$ cat Assets/ForestPrototype/UI/Map/StandMapRackEditor.cs
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

// Direct manipulation of the player's racks on the Stand Map (RACK-V2-05). It turns the pointer into rack edits and nothing more:
// the plan itself is ScenarioOneManager.RackPlan, every change goes through the facade in ScenarioOneManager.RackEditing.cs (which
// applies the editing rules and the existing AddRack / UpdateRack / DeleteRack), and the rules and snapping are RackEditing. This
// class keeps only presentation state: which rack is selected, the gesture under the pointer, the ghost drawn while dragging, the
// status line and a pending reset confirmation. None of it is saved.
//
// The gestures, all with the left button:
//   draw    press on empty map, drag, release   the end nearer the headland becomes the entrance, the other end the far end
//   select  click a rack                         the rack gets a bright outline, an entrance square and a round far-end handle
//   move    drag the body of a rack              it slides along the headland; its entrance re-snaps onto the headland
//   rotate  drag the round handle                the far end follows the pointer; the entrance stays put
// While dragging, the proposal is drawn as a dashed ghost (white if the rules accept it, red if they refuse it) and the plan is
// untouched; releasing applies it, or reports why it was refused. Snapping is soft and advisory; holding Shift turns it off.
public sealed class StandMapRackEditor
{
    private enum Gesture { None, Create, Move, Reshape }

    // A press that moves less than this is a click, not a drag.
    private const float DragStartPx = 4f;
    // How near the pointer must be to a rack's centre line to pick it, at least (its own half width can be more).
    private const float BodySlopPx = 9f;
    // Extra reach around the drawn handle, so the handle is easy to grab.
    private const float HandleSlackPx = 6f;

    private readonly ScenarioOneUiRoot ui;
    private readonly VisualElement layer;
    private readonly Action changed, repaint;
    private Gesture gesture = Gesture.None;
    private int gestureRackId;
    private int pointerId = -1;
    private Vector2 pressPx;
    private RackPoint pressWorld, originEntrance, originFar;
    private bool dragged;
    private List<float> suggestedAngles = new List<float>();
    private bool active;
    private RackEditGhost? ghost;

    // The rack whose outline and handle are shown, or 0.
    public int SelectedRackId { get; private set; }
    // The last thing an edit reported, and whether it was a refusal.
    public string Status { get; private set; } = "";
    public bool StatusIsProblem { get; private set; }
    // Bumped when the status changes for a reason other than a drag in progress, so the side card knows to rebuild.
    public int StatusVersion { get; private set; }
    // True while the reset confirmation is showing.
    public bool ResetPending { get; private set; }
    // A change that would cancel approved but unexecuted removal work waits here for an explicit yes (RACK-V2-07). The authoritative
    // plan has already refused it with a warning and changed nothing; "yes" repeats the same call with the consent flag, "no" drops it.
    // Presentation state only, never saved.
    private Func<RackPlanOperationResult> consentRetry;
    private string consentSuccess = "";
    public bool ConsentPending => consentRetry != null;
    public string ConsentMessage { get; private set; } = "";
    public bool Active => active;
    public bool Dragging => gesture != Gesture.None && dragged;
    public RackEditGhost? Ghost => ghost;
    // Raised for a status change that should only update the status label (a drag in progress), not rebuild the card.
    public event Action LiveStatusChanged;

    // `changed` rebuilds the map and its side card after a discrete action; `repaint` redraws just the map surface, for the many pointer
    // moves of a drag.
    public StandMapRackEditor(ScenarioOneUiRoot ui, VisualElement layer, Action changed, Action repaint)
    {
        this.ui = ui;
        this.layer = layer;
        this.changed = changed ?? (() => { });
        this.repaint = repaint ?? (() => { });
        layer.RegisterCallback<PointerDownEvent>(OnPointerDown);
        layer.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        layer.RegisterCallback<PointerUpEvent>(OnPointerUp);
        layer.RegisterCallback<PointerCancelEvent>(_ => Cancel());
        layer.RegisterCallback<PointerCaptureOutEvent>(_ => { if (gesture != Gesture.None) Cancel(); });
    }

    private ScenarioOneManager Manager => ui.Manager;

    // A string that changes whenever the side card has to be rebuilt.
    public string StateKey
        => SelectedRackId + "|" + (Manager != null ? Manager.PlannedRackCount + "|" + Manager.CanUndoRackEdit + "|" + Manager.RackEditingOpen : "")
            + "|" + ResetPending + "|" + ConsentPending + "|" + StatusVersion + "|" + active;

    // Shows or hides the pointer layer. Off whenever the map is not in V2 rack planning, or the plan cannot be edited.
    public void SetActive(bool on)
    {
        if (active == on) return;
        active = on;
        layer.style.display = on ? DisplayStyle.Flex : DisplayStyle.None;
        if (!on)
        {
            Cancel(false);
            ResetPending = false;
            consentRetry = null;
            ConsentMessage = "";
        }
    }

    public void ClearSelection()
    {
        SelectedRackId = 0;
        ui.MapSource.SelectedRackId = 0;
    }

    // Drops a selection whose rack no longer exists (deleted, or the world changed).
    public void Validate()
    {
        if (SelectedRackId > 0 && (Manager == null || !Manager.TryGetRack(SelectedRackId, out _)))
            ClearSelection();
    }

    public bool Select(int rackId)
    {
        if (Manager == null || !Manager.TryGetRack(rackId, out _)) return false;
        SelectedRackId = rackId;
        ui.MapSource.SelectedRackId = rackId;
        return true;
    }

    // ---- coordinates ----------------------------------------------------------------------------

    private StandMapTransform Transform => ui.MapSource.Model.TransformFor(StandMapView.MapSidePx);

    public Vector2 WorldToFramePx(RackPoint world)
    {
        MapPosition p = Transform.WorldToPixels(world);
        return new Vector2(p.X, p.Y);
    }

    public RackPoint FramePxToWorld(Vector2 framePx) => Transform.PixelsToWorld(new MapPosition(framePx.x, framePx.y));

    // ---- hit testing (the same geometry the draw plan paints) -----------------------------------

    public bool OnHandle(Vector2 framePx, RackPlanRack rack)
    {
        if (rack == null || rack.IsLocked) return false;
        float reach = StandMapDrawPlan.HandleRadiusPx(StandMapView.MapSidePx) + HandleSlackPx;
        return (WorldToFramePx(rack.Far) - framePx).magnitude <= reach;
    }

    // The player rack under the pointer, or 0. The nearest wins; a rack is picked within its own corridor or a little more.
    public int RackBodyAt(Vector2 framePx)
    {
        if (Manager == null) return 0;
        StandMapTransform t = Transform;
        int best = 0;
        float bestDistance = float.MaxValue;
        foreach (RackPlanRack rack in Manager.RackPlan)
        {
            Vector2 a = WorldToFramePx(rack.Entrance), b = WorldToFramePx(rack.Far);
            float distance = DistanceToSegment(framePx, a, b);
            float reach = Mathf.Max(BodySlopPx, t.MetresToPixels(rack.WidthM * 0.5f));
            if (distance <= reach && distance < bestDistance)
            {
                best = rack.Id;
                bestDistance = distance;
            }
        }
        return best;
    }

    private static float DistanceToSegment(Vector2 p, Vector2 a, Vector2 b)
    {
        Vector2 ab = b - a;
        float lengthSquared = ab.sqrMagnitude;
        float u = lengthSquared > 1e-6f ? Mathf.Clamp01(Vector2.Dot(p - a, ab) / lengthSquared) : 0f;
        return (p - (a + ab * u)).magnitude;
    }

    // ---- pointer --------------------------------------------------------------------------------

    private void OnPointerDown(PointerDownEvent e)
    {
        if (!active || e.button != 0 || gesture != Gesture.None || Manager == null) return;
        Vector2 px = e.localPosition;
        pressPx = px;
        pressWorld = FramePxToWorld(px);
        dragged = false;
        ghost = null;
        ResetPending = false;
        consentRetry = null;
        ConsentMessage = "";
        ScenarioOneManager manager = Manager;

        Gesture next = Gesture.None;
        if (SelectedRackId > 0 && manager.TryGetRack(SelectedRackId, out RackPlanRack selected) && OnHandle(px, selected))
        {
            next = Gesture.Reshape;
            BeginRackGesture(selected);
        }
        else
        {
            int hit = RackBodyAt(px);
            if (hit > 0 && manager.TryGetRack(hit, out RackPlanRack rack))
            {
                Select(hit);
                if (rack.IsLocked)
                {
                    SetStatus("Rack " + hit + " has been harvested. Its geometry is locked and cannot be changed.", true);
                }
                else
                {
                    next = Gesture.Move;
                    BeginRackGesture(rack);
                }
            }
            else
            {
                ClearSelection();
                if (manager.PlannedRackCount >= ScenarioOneManager.RackPlanTargetCount)
                    SetStatus("The plan already holds " + ScenarioOneManager.RackPlanTargetCount + " racks. Delete one to draw another.", false);
                else
                {
                    next = Gesture.Create;
                    gestureRackId = 0;
                }
            }
        }

        if (next != Gesture.None)
        {
            gesture = next;
            pointerId = e.pointerId;
            // The suggested orientation is read once per gesture: it is a scan of the plantation, too heavy for every pointer move.
            suggestedAngles = SuggestedAngles(manager);
            layer.CapturePointer(e.pointerId);
        }
        e.StopPropagation();
        changed();
    }

    private void BeginRackGesture(RackPlanRack rack)
    {
        gestureRackId = rack.Id;
        originEntrance = rack.Entrance;
        originFar = rack.Far;
    }

    private static List<float> SuggestedAngles(ScenarioOneManager manager)
    {
        var angles = new List<float>();
        foreach (SuggestedRackSegment segment in manager.GetSuggestedRackLayout())
            angles.Add(RackEditing.DirectedAngleDegrees(segment.Entrance, segment.Far));
        return angles;
    }

    private static bool ShiftHeld(EventModifiers modifiers)
        => (modifiers & EventModifiers.Shift) != 0 || (Keyboard.current != null && Keyboard.current.shiftKey.isPressed);

    private void OnPointerMove(PointerMoveEvent e)
    {
        if (gesture == Gesture.None || e.pointerId != pointerId) return;
        Vector2 px = e.localPosition;
        if (!dragged && (px - pressPx).sqrMagnitude < DragStartPx * DragStartPx) return;
        dragged = true;
        Propose(px, !ShiftHeld(e.modifiers), out RackPoint entrance, out RackPoint far, out RackSnapResult snap, out RackEditValidation check);
        ghost = new RackEditGhost(entrance, far, RackGeometry.DefaultWidthM, check.IsValid);
        ui.MapSource.EditGhost = ghost;
        ui.MapSource.MarkRackEditDirty();
        repaint();
        SetLiveStatus(Describe(entrance, far, snap, check), !check.IsValid);
        e.StopPropagation();
    }

    private void OnPointerUp(PointerUpEvent e)
    {
        if (e.button != 0 || gesture == Gesture.None || e.pointerId != pointerId) return;
        Gesture finished = gesture;
        int rackId = gestureRackId;
        bool wasDragged = dragged;
        // Releasing applies the proposal the player was last shown (the ghost), not a position read from the release event: when the
        // last pointer moves and the release land in one frame, the release can still carry the position from before them.
        // What you see when you let go is what you get.
        RackPoint entrance = default, far = default;
        bool haveProposal = ghost.HasValue;
        if (haveProposal)
        {
            entrance = ghost.Value.Start;
            far = ghost.Value.End;
        }
        EndGesture();
        if (wasDragged && haveProposal)
            Apply(finished, rackId, entrance, far);
        e.StopPropagation();
        changed();
    }

    private void Cancel(bool notify = true)
    {
        if (gesture == Gesture.None && !ghost.HasValue) return;
        EndGesture();
        if (notify) changed();
    }

    private void EndGesture()
    {
        // Clear the gesture BEFORE releasing capture: releasing raises PointerCaptureOut, whose handler cancels any live gesture.
        int captured = pointerId;
        gesture = Gesture.None;
        pointerId = -1;
        dragged = false;
        ghost = null;
        ui.MapSource.EditGhost = null;
        ui.MapSource.MarkRackEditDirty();
        if (captured >= 0 && layer.HasPointerCapture(captured))
            layer.ReleasePointer(captured);
    }

    // Where the proposal would be for a pointer at `px`, and whether the editing rules accept it.
    private void Propose(Vector2 px, bool snap, out RackPoint entrance, out RackPoint far, out RackSnapResult snapResult, out RackEditValidation check)
    {
        ScenarioOneManager manager = Manager;
        RackEditBounds bounds = manager.RackEditBounds;
        RackPoint pointer = FramePxToWorld(px);
        snapResult = default;
        switch (gesture)
        {
            case Gesture.Move:
                RackEditing.MoveAlongHeadland(bounds, originEntrance, originFar, originEntrance.Z + (pointer.Z - pressWorld.Z), out entrance, out far);
                break;
            case Gesture.Reshape:
                entrance = originEntrance;
                snapResult = RackEditing.SnapFar(bounds, entrance, pointer, suggestedAngles, manager.RackEditOthers(gestureRackId), snap);
                far = snapResult.Far;
                break;
            default:
            {
                RackEditing.ResolveCreation(bounds, pressWorld, pointer, out entrance, out RackPoint rawFar);
                snapResult = RackEditing.SnapFar(bounds, entrance, rawFar, suggestedAngles, manager.RackEditOthers(0), snap);
                far = snapResult.Far;
                break;
            }
        }
        check = manager.ValidateRackEdit(gesture == Gesture.Create ? 0 : gestureRackId, entrance, far);
    }

    private string Describe(RackPoint entrance, RackPoint far, RackSnapResult snap, RackEditValidation check)
    {
        if (!check.IsValid) return check.Message;
        float length = RackGeometry.LengthM(entrance, far);
        string text = UiKit.F(length, "0.0") + " m, pointing " + UiKit.F(Bearing(entrance, far), "0") + "° from north";
        if (snap.AngleSnapped)
            text += snap.ParallelToRackId > 0 ? " · parallel to rack " + snap.ParallelToRackId : " · along the suggested direction";
        if (snap.BoundarySnapped) text += " · to the stand edge";
        return text;
    }

    // Read the live ghost during a gesture, otherwise the selected authoritative rack. Suggestions are never included.
    public string SpacingText()
    {
        if (Manager == null) return "";
        RackPoint entrance, far;
        int ownId;
        if (Dragging && ghost.HasValue)
        {
            entrance = ghost.Value.Start;
            far = ghost.Value.End;
            ownId = gesture == Gesture.Create ? 0 : gestureRackId;
        }
        else if (SelectedRackId > 0 && Manager.TryGetRack(SelectedRackId, out RackPlanRack selected))
        {
            entrance = selected.Entrance;
            far = selected.Far;
            ownId = selected.Id;
        }
        else return "Select or draw a rack to see its nearest neighbour.";

        if (!RackEditing.NearestRack(entrance, far, Manager.RackEditOthers(ownId), out int neighbour, out float distance))
            return "No neighbouring player rack yet.";
        return "Nearest rack " + neighbour + ": " + UiKit.F(distance, "0.0") + " m — " + RackEditing.SpacingStatus(distance)
            + " (closest centre-line distance).";
    }

    // The compass bearing of a rack, in degrees from north (90 = east), as the map labels it.
    public static float Bearing(RackPoint entrance, RackPoint far)
    {
        float angle = RackEditing.DirectedAngleDegrees(entrance, far);
        return ((90f - angle) % 360f + 360f) % 360f;
    }

    // ---- applying an edit -----------------------------------------------------------------------

    private void Apply(Gesture finished, int rackId, RackPoint entrance, RackPoint far)
    {
        ScenarioOneManager manager = Manager;
        RackPlanOperationResult result;
        switch (finished)
        {
            case Gesture.Create:
                result = manager.CreateRackFromMap(entrance, far);
                if (result.Status == RackPlanOperationStatus.Applied)
                {
                    Select(result.RackId);
                    SetStatus("Added rack " + result.RackId + ".", false);
                }
                else Report(result, "");
                break;
            default:
                result = manager.ReshapeRackFromMap(rackId, entrance, far);
                Report(result, finished == Gesture.Move ? "Moved rack " + rackId + "." : "Rotated rack " + rackId + ".",
                    () => manager.ReshapeRackFromMap(rackId, entrance, far, true));
                break;
        }
        ui.MapSource.MarkRacksDirty();
    }

    private void Report(RackPlanOperationResult result, string success, Func<RackPlanOperationResult> retryWithConsent = null)
    {
        consentRetry = null;
        ConsentMessage = "";
        if (result.RequiresApprovalWarning && retryWithConsent != null)
        {
            // Nothing has changed yet. Ask, and keep the exact call to repeat if the player agrees.
            consentRetry = retryWithConsent;
            consentSuccess = success;
            ConsentMessage = result.Message;
            SetStatus("Waiting for your decision.", false);
            return;
        }
        string text = result.Status == RackPlanOperationStatus.Applied && !string.IsNullOrEmpty(success) ? success : result.Message;
        if (result.Status == RackPlanOperationStatus.NoChange && string.IsNullOrEmpty(text)) text = "Nothing changed.";
        SetStatus(text, !result.Succeeded);
    }

    private void SetStatus(string text, bool problem)
    {
        Status = text ?? "";
        StatusIsProblem = problem;
        StatusVersion++;
    }

    private void SetLiveStatus(string text, bool problem)
    {
        Status = text ?? "";
        StatusIsProblem = problem;
        LiveStatusChanged?.Invoke();
    }

    // ---- commands from the side card and the keyboard -------------------------------------------

    public void HandleKeys(Keyboard keyboard)
    {
        if (keyboard == null || !active || gesture != Gesture.None || SelectedRackId <= 0) return;
        if (keyboard.deleteKey.wasPressedThisFrame || keyboard.backspaceKey.wasPressedThisFrame)
            DeleteSelected();
    }

    public void DeleteSelected()
    {
        ResetPending = false;
        if (Manager == null) return;
        if (SelectedRackId <= 0)
        {
            SetStatus("Click a rack to select it first.", false);
            changed();
            return;
        }
        int id = SelectedRackId;
        RackPlanOperationResult result = Manager.DeleteRackFromMap(id);
        Report(result, "Deleted rack " + id + ".", () => Manager.DeleteRackFromMap(id, true));
        if (result.Status == RackPlanOperationStatus.Applied) ClearSelection();
        ui.MapSource.MarkRacksDirty();
        changed();
    }

    public void UndoLast()
    {
        ResetPending = false;
        if (Manager == null) return;
        RackPlanOperationResult result = Manager.UndoLastRackEdit();
        Report(result, result.Message, () => Manager.UndoLastRackEdit(true));
        if (result.Status == RackPlanOperationStatus.Applied)
        {
            // Show the rack the undo touched, if it still exists.
            if (result.RackId > 0 && Manager.TryGetRack(result.RackId, out _)) Select(result.RackId);
            else Validate();
        }
        ui.MapSource.MarkRacksDirty();
        changed();
    }

    // Reset to the suggested layout. If the plan holds racks that differ from it the player is asked first; with nothing to lose
    // (an empty plan) it applies at once, and with the plan already equal to the suggestion nothing is done.
    public void RequestReset()
    {
        if (Manager == null) return;
        IReadOnlyList<SuggestedRackSegment> suggestion = Manager.GetSuggestedRackLayout();
        if (suggestion.Count == 0)
        {
            SetStatus("There is no suggested layout to restore.", true);
        }
        else if (Manager.RackLayoutMatchesSuggestion(suggestion))
        {
            ResetPending = false;
            SetStatus("The plan already matches the suggested layout.", false);
        }
        else if (Manager.PlannedRackCount > 0)
        {
            ResetPending = true;
            StatusVersion++;
        }
        else
        {
            ApplyReset(suggestion);
        }
        changed();
    }

    public void ConfirmReset()
    {
        if (Manager == null) return;
        ApplyReset(Manager.GetSuggestedRackLayout());
        changed();
    }

    public void CancelReset()
    {
        ResetPending = false;
        StatusVersion++;
        changed();
    }

    // The warning the plan gave for a change that needs the player's consent, shown by a command that is not an edit gesture (Confirm
    // Rack Plan). Same rules as an edit: nothing changed yet; yes repeats the call with consent, no leaves everything as it was.
    public void AskConsent(RackPlanOperationResult warning, string successText, Func<RackPlanOperationResult> retryWithConsent)
    {
        ResetPending = false;
        Report(warning, successText, retryWithConsent);
        changed();
    }

    // Lets the Stand Map report the result of a plan-level command (confirm, reopen) in the same status line.
    public void NoteStatus(string text, bool problem)
    {
        ResetPending = false;
        SetStatus(text, problem);
    }

    public void AcceptConsent()
    {
        if (consentRetry == null) return;
        Func<RackPlanOperationResult> retry = consentRetry;
        string success = consentSuccess;
        consentRetry = null;
        ConsentMessage = "";
        RackPlanOperationResult result = retry();
        Report(result, success);
        if (result.Status == RackPlanOperationStatus.Applied) ClearSelection();
        ui.MapSource.MarkRacksDirty();
        ui.RackLayout?.Update(true);
        changed();
    }

    public void DeclineConsent()
    {
        if (consentRetry == null) return;
        consentRetry = null;
        ConsentMessage = "";
        SetStatus("Nothing was changed. The approved work stays as it is.", false);
        changed();
    }

    private void ApplyReset(IReadOnlyList<SuggestedRackSegment> suggestion)
    {
        ResetPending = false;
        RackPlanOperationResult result = Manager.ResetRacksToSuggestion(suggestion);
        Report(result, "Reset the plan to the suggested layout.", () => Manager.ResetRacksToSuggestion(suggestion, true));
        if (result.Status == RackPlanOperationStatus.Applied) ClearSelection();
        ui.MapSource.MarkRacksDirty();
    }
}

[exit 0]

$ cat Assets/ForestPrototype/UI/StandMapView.cs
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

// Screen 6 — stand map. A forestry plan of the property: the real trees, the racks, the path, the clearing, you and your destination, on
// a local survey grid. The analytical layers (Light, Regeneration, Browsing, Marks) are translucent overlays on that same plan, so the player
// never loses orientation, and the figure behind a colour is read by pointing at it or from the selected-square card.
//
// Diagnostic and navigation only: it shows recorded state and lets the player set a waypoint and plan a rack through the existing rack
// tool. It never moves the player and never recommends where to fell or plant.
//
// The 5 m ecology squares remain the unit of selection, waypoint and rack placement, and the data behind the Light and Regeneration
// overlays; they are invisible buttons laid over the plan by the shared map transform, not a drawn grid.
public sealed class StandMapView
{
    private enum Layer { Access, Light, Regeneration, Browse, Marks, Review }

    // Side of the map square in UI units. The square buttons, and everything the surface draws under them, are placed on this square by
    // the shared map transform (StandMapTransform); the minimap uses the same transform at its own, smaller side.
    public const float MapSidePx = 688f;
    private const float OverviewSidePx = 144f;
    private const string DefaultCaption = "Local grid · metres east and north of the south-west corner · north is up";

    private readonly ScenarioOneUiRoot ui;
    public VisualElement Root { get; }
    private readonly VisualElement grid, side, layerButtons, toolButtons, frame, eastLabels, northLabels, overviewFacts;
    private readonly StandMapSurface surface, overviewSurface;
    private readonly StandMapLegendView legendView;
    private readonly Label caption, northMark, scaleMark;
    // MFM-08 Treatment review: its own element over the surface (the shared map canvas and the HUD minimap are untouched) and its card.
    private readonly StandMapReviewLayer reviewLayer;
    private readonly StandMapTreatmentReview review;
    public StandMapTreatmentReview Review => review;
    public StandMapReviewLayer ReviewLayer => reviewLayer;
    public StandMapCanvas Canvas => surface.Canvas;
    public StandMapSurface Surface => surface;
    public StandMapCanopyLayer Canopy => surface.Canopy;
    public StandMapLegendView Legend => legendView;
    // The overview minimap in the right-hand stack: the same map, filtered as the HUD minimap is.
    public StandMapSurface Overview => overviewSurface;
    // The cell grid, exposed so verification can compare its real layout with the shared transform.
    public VisualElement Grid => grid;
    private Layer layer = Layer.Access;
    private Button[] cells;
    private int selectedCell = -1;
    private int waypointCell = -1;
    private GameObject waypointMarker;
    private string shownKey = "";
    private float labelledSize = -1f;
    private int factsVersion = -1;
    private bool journeyStartedAway;
    public bool HasWaypoint => waypointCell >= 0;
    public string CaptionText => caption.text;

    // Plan rack: the Stand Map's way into the same rack tool the forest uses (RackPlanner / ScenarioOneManager.ConfirmRack). Pick a
    // square on the row you want, check the preview, add it. Presentation state only: nothing changes until the rack is added.
    private bool rackMode, rackHasPoint;
    private float rackAngle;
    private RackPoint rackPoint;
    private RackProposal rackProposal;
    private readonly HashSet<int> rackCells = new HashSet<int>();
    public bool RackMode => rackMode;
    public RackProposal RackPreview => rackProposal;
    // Direct rack editing in a V2 world (RACK-V2-05): the pointer layer over the squares and the controller behind it.
    private readonly VisualElement editLayer;
    private readonly StandMapRackEditor editor;
    private Label editStatus, editSpacing;
    public StandMapRackEditor RackEditor => editor;
    // Reopen Rack Plan asks first when approved work is linked to the plan (RACK-V2-07). Presentation state only.
    private bool reopenPending;
    public VisualElement EditLayer => editLayer;
    // V2 planning is available immediately; the legacy tool retains its original teaching gate.
    public bool RackPlanningAvailable => ui.Manager != null && ui.RackTool != null && ui.Ecology != null && ui.Manager.CanEditWorkPlan
        && (ui.Manager.RackPlanningV2Active || !ui.Manager.RackFirstStageActive || ui.Learning.IsDone("rack.create"));

    public StandMapView(ScenarioOneUiRoot ui)
    {
        this.ui = ui;
        // The UI is built after the ecology already has its geometry (that event was raised before this existed).
        if (ui.Ecology != null) heldGeometry = ui.Ecology.StandGeometryModelVersion;
        Root = UiKit.Box("layer", "modal-backdrop");
        UiKit.MakeModal(Root);
        VisualElement modal = UiKit.Box("panel", "modal", "modal-map");
        Root.Add(modal);
        VisualElement header = UiKit.Box("modal-header");
        UiKit.Add(header, "Stand map", "title");
        VisualElement buttons = UiKit.Box("row");
        buttons.Add(UiKit.Button("Help [F1]", () => ui.ShowHelp(MenuHelpView.Menu.StandMap)));
        buttons.Add(UiKit.Button("Objectives [O]", () => ui.ShowObjectives()));
        buttons.Add(UiKit.Button("Work Plan", () => ui.ShowWorkPlan()));
        buttons.Add(UiKit.Button("Back to forest [M / Esc]", () => ui.CloseAll(), true, "btn-primary"));
        header.Add(buttons);
        modal.Add(header);

        layerButtons = UiKit.Box("map-tabs");
        layerButtons.name = "map-tabs";
        // The tool row shares the tab line instead of taking a second row: the fixed-size map and its survey caption
        // need that height at 720p and 1080p. It remains a separate row container, not one of the navigation tabs.
        VisualElement tabsRow = UiKit.Box("row");
        tabsRow.name = "map-tabs-row";
        tabsRow.Add(layerButtons);
        toolButtons = UiKit.Box("row");
        toolButtons.name = "map-tools";
        toolButtons.style.marginLeft = 12;
        tabsRow.Add(toolButtons);
        modal.Add(tabsRow);

        VisualElement body = UiKit.Box("map-body");
        VisualElement left = UiKit.Box("map-left");
        VisualElement wrap = UiKit.Box("map-wrap");
        northLabels = UiKit.Box("map-north-labels");
        northLabels.style.height = MapSidePx;
        wrap.Add(northLabels);
        VisualElement column = UiKit.Box("map-column");
        // One square frame holds the surface (the forest picture, the analytical field and the shapes, all drawn from the shared model and
        // the shared transform) and, over it, the invisible square buttons that make the map interactive.
        frame = UiKit.Box("map-frame");
        frame.name = "map-frame";
        frame.style.width = MapSidePx;
        frame.style.height = MapSidePx;
        frame.style.flexShrink = 0;
        surface = new StandMapSurface(MapDetail.Full) { name = "map-surface" };
        surface.style.position = Position.Absolute;
        surface.style.left = surface.style.top = surface.style.right = surface.style.bottom = 0;
        frame.Add(surface);
        reviewLayer = new StandMapReviewLayer();
        frame.Add(reviewLayer);
        review = new StandMapTreatmentReview(ui, SetWaypointAtWorld, () => Refresh(true));
        // The two words printed on the map itself, beside the north arrow and the scale bar that the plan draws.
        northMark = UiKit.Text("N", "map-north-label");
        northMark.pickingMode = PickingMode.Ignore;
        scaleMark = UiKit.Text("", "map-scale-label");
        scaleMark.pickingMode = PickingMode.Ignore;
        frame.Add(northMark);
        frame.Add(scaleMark);
        grid = UiKit.Box("map-grid");
        frame.Add(grid);
        // Over the squares, hidden unless the V2 plan can be edited: while shown it takes the pointer, so the squares underneath
        // (which only select and place legacy racks) are not clicked by a drag meant for a rack.
        editLayer = UiKit.Box("map-edit-layer");
        editLayer.name = "map-edit-layer";
        editLayer.style.position = Position.Absolute;
        editLayer.style.left = editLayer.style.top = editLayer.style.right = editLayer.style.bottom = 0;
        editLayer.pickingMode = PickingMode.Position;
        editLayer.style.display = DisplayStyle.None;
        frame.Add(editLayer);
        editor = new StandMapRackEditor(ui, editLayer, () => Refresh(true), () => surface.Sync(ui.MapSource.Model, Overlay));
        editor.LiveStatusChanged += ShowLiveEditStatus;
        frame.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        frame.RegisterCallback<PointerLeaveEvent>(_ => caption.text = DefaultCaption);
        column.Add(frame);
        eastLabels = UiKit.Box("map-east-labels");
        eastLabels.style.width = MapSidePx;
        column.Add(eastLabels);
        caption = UiKit.Add(column, DefaultCaption, "map-caption");
        caption.name = "map-caption";
        wrap.Add(column);
        left.Add(wrap);
        body.Add(left);

        // Two columns to the right of the map: the overview and the legend, then the card for the square or the rack. The card has the
        // window's full height, as the Stand Map's side card always has.
        VisualElement columnA = UiKit.Box("map-col-a");
        VisualElement overview = UiKit.Box("card", "map-card", "map-overview");
        overview.name = "map-overview";
        UiKit.Add(overview, "Overview", "map-card-title");
        VisualElement overviewRow = UiKit.Box("map-overview-row");
        VisualElement miniFrame = UiKit.Box("map-mini-frame");
        miniFrame.style.width = OverviewSidePx;
        miniFrame.style.height = OverviewSidePx;
        overviewSurface = new StandMapSurface(MapDetail.Mini) { name = "map-overview-surface" };
        overviewSurface.style.position = Position.Absolute;
        overviewSurface.style.left = overviewSurface.style.top = overviewSurface.style.right = overviewSurface.style.bottom = 0;
        miniFrame.Add(overviewSurface);
        overviewRow.Add(miniFrame);
        overviewFacts = UiKit.Box("map-facts");
        overviewRow.Add(overviewFacts);
        overview.Add(overviewRow);
        columnA.Add(overview);
        legendView = new StandMapLegendView();
        columnA.Add(legendView.Root);
        body.Add(columnA);
        side = UiKit.Box("card", "map-side");
        VisualElement columnB = UiKit.Box("map-col-b");
        columnB.Add(side);
        body.Add(columnB);
        modal.Add(body);
    }

    // Called every frame by the UI root while the map is open (Refresh runs only a few times a second, which would miss a key press).
    public void HandleKeys(Keyboard keyboard)
    {
        if (rackMode) editor.HandleKeys(keyboard);
    }

    private MapOverlay Overlay => layer == Layer.Access || layer == Layer.Review ? MapOverlay.None : layer == Layer.Light ? MapOverlay.Light : layer == Layer.Regeneration ? MapOverlay.Regeneration
        : layer == Layer.Browse ? MapOverlay.Browsing : MapOverlay.Marks;

    public void Refresh(bool force)
    {
        ForestEcologyController eco = ui.Ecology;
        if (eco == null || eco.Cells == null)
            return;
        // Direct rack editing is live only while planning racks in a V2 world whose plan is not confirmed.
        bool planningV2 = rackMode && ui.Manager.RackPlanningV2Active;
        editor.SetActive(planningV2 && ui.Manager.RackEditingOpen);
        if (!planningV2) editor.ClearSelection();
        editor.Validate();
        int playerCell = ui.Player != null ? eco.GetCellIndex(ui.Player.transform.position) : -1;
        if (selectedCell < 0) selectedCell = playerCell;
        // The rack being planned here is drawn on this map only (the minimap filters it out). Then the surface is given the shared
        // model, which is re-read now so it is as current as the squares below.
        ui.MapSource.Preview = rackMode && !ui.Manager.RackPlanningV2Active ? rackProposal : null;
        // Dashed guidance belongs to an open plan: once the plan is confirmed the solid racks are the whole story.
        ui.MapSource.ShowRackSuggestions = rackMode && ui.Manager.RackPlanningV2Active && !ui.Manager.RackPlanConfirmed;
        if (!planningV2 || !ui.Manager.RackPlanConfirmed) reopenPending = false;
        StandMapModel model = force ? ui.MapSource.Rebuild() : ui.MapSource.Model;
        // Opening the map must show the forest as it is now: finish any redraw that is part-way.
        if (force)
        {
            surface.Canopy.FinishNow(model);
            overviewSurface.Canopy.FinishNow(model);
        }
        surface.Sync(model, Overlay);
        overviewSurface.Sync(model, MapOverlay.None);
        string key = $"{eco.EcologicalYear}|{layer}|{selectedCell}|{playerCell}|{waypointCell}|{model.StaticVersion}|{model.CanopyVersion}"
            + $"|{rackMode}|{rackAngle}|{(rackProposal != null ? rackProposal.TreeCount : -1)}|{(ui.RackLayout != null ? ui.RackLayout.Version : -1)}"
            + (layer == Layer.Review ? "|r" + review.Version : "")
            + (planningV2 ? "|" + editor.StateKey + "|" + reopenPending + "|" + ui.Manager.RackPlanConfirmed + "|" + ui.Manager.MarkedRackEntranceCount : "");
        if (!force && key == shownKey)
            return;
        shownKey = key;

        BuildLayerButtons();
        EnsureCells(eco, model);
        UpdateCells(playerCell);
        UpdateField(eco);
        UpdateSurveyLabels(model);
        UpdateOverview(model, eco);
        // The dashed guidance is gone once the plan is confirmed, so its legend row goes too.
        legendView.Show(model, Overlay, key, rackMode && ui.Manager.RackPlanningV2Active && !ui.Manager.RackPlanConfirmed);
        BuildSide(eco, model);
    }

    private void BuildLayerButtons()
    {
        layerButtons.Clear();
        AddLayerButton("Access & position", Layer.Access);
        AddLayerButton("Light", Layer.Light);
        AddLayerButton("Regeneration", Layer.Regeneration);
        AddLayerButton("Browsing / protection", Layer.Browse);
        AddLayerButton("Fell & crop marks", Layer.Marks);
        if (ui.Manager != null && ui.Manager.RackPlanningV2Active) AddLayerButton("Treatment review", Layer.Review);
        toolButtons.Clear();
        if (RackPlanningAvailable)
        {
            string label = ui.Manager.RackPlanningV2Active
                ? (rackMode ? "Planning Racks — Active" : "Plan Racks")
                : (rackMode ? "Plan rack: ON" : "Plan rack");
            Button plan = UiKit.Button(label, ToggleRackMode, true,
                rackMode ? new[] { "btn-selected" } : new[] { "btn-primary" });
            plan.name = "map-plan-rack";
            toolButtons.Add(plan);
        }
    }

    private void AddLayerButton(string text, Layer value)
    {
        layerButtons.Add(UiKit.Button(text, () =>
        {
            layer = value;
            if (value != Layer.Access && value != Layer.Review) ui.Learning.Record(value == Layer.Light ? "map.light" : value == Layer.Regeneration ? "map.regeneration"
                : value == Layer.Browse ? "map.browse" : "map.marks");
            Refresh(true);
        }, true, layer == value ? new[] { "btn-selected" } : new string[0]));
    }

    // The 5 m squares: one invisible button each, made once per geometry and placed by the shared map transform, so a square sits exactly
    // where the world point at its centre is drawn. They are listed north row first (the order they appear on screen).
    private void EnsureCells(ForestEcologyController eco, StandMapModel model)
    {
        int n = eco.CellsPerAxis;
        if (cells != null && cells.Length == n * n) return;
        grid.Clear();
        cells = new Button[n * n];
        StandMapTransform transform = model.TransformFor(MapSidePx);
        float size = transform.CellSidePx(n);
        for (int z = n - 1; z >= 0; z--)
            for (int x = 0; x < n; x++)
            {
                int index = z * n + x;
                var cell = new Button(() => { selectedCell = index; ui.Learning.Record("map.select"); if (rackMode && !ui.Manager.RackPlanningV2Active) PlaceRack(ui.Ecology, index); Refresh(true); }) { text = "", focusable = false };
                cell.AddToClassList("map-cell");
                MapPosition corner = transform.CellTopLeft(index, n);
                cell.style.position = Position.Absolute;
                cell.style.left = corner.X;
                cell.style.top = corner.Y;
                cell.style.width = size;
                cell.style.height = size;
                cells[index] = cell;
                grid.Add(cell);
            }
    }

    // Which square is selected, which one you stand in, and (when planning a rack) which ones the rack crosses.
    private void UpdateCells(int playerCell)
    {
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i].EnableInClassList("map-cell-selected", i == selectedCell);
            cells[i].EnableInClassList("map-cell-player", i == playerCell);
            cells[i].EnableInClassList("map-cell-rack", rackMode && rackCells.Contains(i));
        }
    }

    // The Light and Regeneration overlays: one colour per square, laid over the plan as a smooth translucent field.
    private Color32[] fieldColours;
    private void UpdateField(ForestEcologyController eco)
    {
        if (layer != Layer.Light && layer != Layer.Regeneration) { surface.Field.Clear(); return; }
        int count = eco.CellCount;
        if (fieldColours == null || fieldColours.Length != count) fieldColours = new Color32[count];
        int[] planted = null;
        if (layer == Layer.Regeneration)
        {
            planted = new int[count];
            foreach (PlantedJuvenile j in ui.Manager.PlantedJuveniles)
            {
                if (!j.alive || !string.IsNullOrEmpty(j.promotedTreeId)) continue;
                int c = eco.GetCellIndex(j.position);
                if (c >= 0) planted[c]++;
            }
        }
        for (int i = 0; i < count; i++)
        {
            ForestEcologyCell c = eco.Cells[i];
            MapColour colour;
            if (layer == Layer.Light) colour = StandMapOverlayColours.Light(c.Light);
            else
            {
                float tallest = 0f;
                int cohorts = 0;
                foreach (ForestRegenerationCohort r in c.Regeneration)
                    if (r != null && r.Species != null && r.Density > 0f) { cohorts++; tallest = Mathf.Max(tallest, r.Height); }
                colour = StandMapOverlayColours.Regeneration(tallest, cohorts > 0, planted[i] > 0);
            }
            fieldColours[i] = new Color32(colour.R, colour.G, colour.B, colour.A);
        }
        surface.Field.Show(fieldColours, eco.CellsPerAxis);
    }

    // The survey furniture's words: easting and northing numbers along the edges, "N" by the arrow and the length beside the scale bar.
    // They depend only on the property, so they are rebuilt only when its size changes.
    private void UpdateSurveyLabels(StandMapModel model)
    {
        if (Mathf.Approximately(labelledSize, model.SizeM)) return;
        labelledSize = model.SizeM;
        eastLabels.Clear();
        northLabels.Clear();
        foreach (StandMapGridPlan.GridLabel label in StandMapGridPlan.Labels(model))
        {
            Label text = UiKit.Text(label.Text, "map-axis-label");
            text.pickingMode = PickingMode.Ignore;
            if (label.Easting)
            {
                text.AddToClassList("map-axis-east");
                text.style.left = label.Fraction * MapSidePx - 20f;
                eastLabels.Add(text);
            }
            else
            {
                text.AddToClassList("map-axis-north");
                text.style.top = (1f - label.Fraction) * MapSidePx - 9f;
                northLabels.Add(text);
            }
        }
        StandMapGridPlan.Furniture f = StandMapGridPlan.Layout(model.SizeM, MapSidePx, MapDetail.Full);
        northMark.style.left = f.ArrowX - 6f;
        northMark.style.top = f.ArrowY + f.ArrowSize * 0.7f + 1f;
        scaleMark.text = StandMapGridPlan.ScaleBarText(f.BarMetres);
        scaleMark.style.left = f.BarX1 + 8f;
        scaleMark.style.top = f.BarY - 13f;
    }

    // The overview card's figures, from the trees on the map. Rebuilt only when the trees change.
    private void UpdateOverview(StandMapModel model, ForestEcologyController eco)
    {
        if (factsVersion == model.CanopyVersion && overviewFacts.childCount > 0) return;
        factsVersion = model.CanopyVersion;
        overviewFacts.Clear();
        StandMapStats stats = model.ComputeStats();
        void Fact(string text)
        {
            overviewFacts.Add(UiKit.Text(text, "map-fact"));
        }
        Fact($"{model.SizeM.ToString("0", UiKit.Inv)} × {model.SizeM.ToString("0", UiKit.Inv)} m · {model.AreaHectares.ToString("0.00", UiKit.Inv)} ha");
        Fact(stats.LivingTrees.ToString("N0", UiKit.Inv) + " living trees");
        Fact(stats.StemsPerHectare.ToString("N0", UiKit.Inv) + " stems per ha");
        Fact("Mean DBH " + stats.MeanDbhCm.ToString("0.0", UiKit.Inv) + " cm");
        Fact("Basal area " + stats.BasalAreaM2PerHectare.ToString("0.0", UiKit.Inv) + " m²/ha");
        Fact(model.GroundIsLevel ? "Level ground · no relief data" : "Terrain present, not drawn");
    }

    // What the pointer is over: its local coordinates, the 5 m square and that square's own figures. This is where the numbers behind
    // the colours are read. framePx is a position inside the map frame in UI units, from its top-left corner.
    public string DescribeAt(Vector2 framePx)
    {
        ForestEcologyController eco = ui.Ecology;
        if (eco == null || eco.Cells == null) return DefaultCaption;
        if (framePx.x < 0f || framePx.y < 0f || framePx.x > MapSidePx || framePx.y > MapSidePx) return DefaultCaption;
        StandMapModel model = ui.MapSource.Model;
        RackPoint world = model.TransformFor(MapSidePx).PixelsToWorld(new MapPosition(framePx.x, framePx.y));
        int index = eco.GetCellIndex(new Vector3(world.X, 0f, world.Z));
        string text = $"E {UiKit.F(model.LocalEasting(world.X), "0.0")} m · N {UiKit.F(model.LocalNorthing(world.Z), "0.0")} m";
        if (index < 0) return text;
        ForestEcologyCell c = eco.Cells[index];
        text += $" · square {UiKit.CellLabel(index, eco.CellsPerAxis)} · ground light {UiKit.F(c.Light, "0.00")} · canopy {UiKit.F(c.Canopy, "0.00")}";
        if (layer == Layer.Regeneration)
        {
            float tallest = 0f;
            foreach (ForestRegenerationCohort r in c.Regeneration)
                if (r != null && r.Species != null && r.Density > 0f) tallest = Mathf.Max(tallest, r.Height);
            text += tallest > 0f ? $" · young trees up to {UiKit.F(tallest, "0.0")} m" : " · no young trees";
        }
        return text;
    }

    private void OnPointerMove(PointerMoveEvent e) => caption.text = DescribeAt(e.localPosition);

    // Marked trees per square, counted from the trees on the shared map (so the scene is not searched again).
    private static Dictionary<int, (int fell, int crop)> CountMarks(ForestEcologyController eco, StandMapModel model)
    {
        var counts = new Dictionary<int, (int fell, int crop)>();
        foreach (MapTree tree in model.Trees)
        {
            if (tree.State != MapTreeState.Living || tree.Mark == MapTreeMark.None) continue;
            int i = eco.GetCellIndex(new Vector3(tree.X, 0f, tree.Z));
            if (i < 0) continue;
            counts.TryGetValue(i, out var c);
            counts[i] = tree.Mark == MapTreeMark.Fell ? (c.fell + 1, c.crop) : (c.fell, c.crop + 1);
        }
        return counts;
    }

    private void ToggleRackMode()
    {
        rackMode = !rackMode;
        editor.ClearSelection();
        rackHasPoint = false;
        rackProposal = null;
        rackCells.Clear();
        if (rackMode && !ui.Manager.RackPlanningV2Active)
        {
            ui.RackTool.Planner.Refresh();
            rackAngle = ui.RackTool.Planner.DefaultAngle(0f, true);
        }
        ui.RackLayout?.Update(true);
        Refresh(true);
    }

    // A pointed square becomes the rack's position; the planner puts the rack on the planted row nearest the square's centre.
    private void PlaceRack(ForestEcologyController eco, int index)
    {
        Vector2 centre = eco.Cells[index].Center;
        // A square the planned layout crosses puts the rack on that layout row (rows are about 2 m apart and a square is 5 m, so the
        // centre of the square is usually nearer a different row); anywhere else the planner takes the row nearest the centre.
        float across = centre.y;
        float turn = RackGeometry.NormaliseAngle(rackAngle);
        if (ui.RackLayout != null && (turn < 0.5f || turn > 179.5f))
            foreach (RackLayoutLine line in ui.RackLayout.Lines)
                if (Mathf.Abs(centre.y - line.AcrossM) <= eco.CellSizeMeters * 0.5f) { across = line.AcrossM; break; }
        rackPoint = new RackPoint(centre.x, across);
        rackHasPoint = true;
        RebuildRack(eco);
    }

    private void RebuildRack(ForestEcologyController eco)
    {
        rackCells.Clear();
        rackProposal = null;
        if (!rackHasPoint) return;
        RackPlanner planner = ui.RackTool.Planner;
        planner.Refresh();
        rackProposal = planner.Propose(rackPoint, rackAngle);
        // Remember the snapped line so a nudge moves from the row actually shown.
        RackPoint a = rackProposal.Selection.Start, b = rackProposal.Selection.End;
        rackPoint = new RackPoint((a.X + b.X) * 0.5f, (a.Z + b.Z) * 0.5f);
        float length = rackProposal.Selection.LengthM;
        for (float t = 0f; t <= length; t += eco.CellSizeMeters * 0.5f)
        {
            float f = length > 0f ? t / length : 0f;
            int cell = eco.GetCellIndex(new Vector3(Mathf.Lerp(a.X, b.X, f), 0f, Mathf.Lerp(a.Z, b.Z, f)));
            if (cell >= 0) rackCells.Add(cell);
        }
    }

    private void NudgeRack(ForestEcologyController eco, int direction)
    {
        if (!rackHasPoint) return;
        IReadOnlyList<RackRow> rows = ui.RackTool.Planner.Rows(rackAngle);
        float across = RackGeometry.Across(rackPoint, rackAngle);
        int i = RackRows.Nearest(rows, across);
        if (i < 0) return;
        int target = Mathf.Clamp(i + direction, 0, rows.Count - 1);
        RackPoint d = RackGeometry.Direction(rackAngle);
        float along = RackGeometry.Along(rackPoint, rackAngle), c = rows[target].Centre;
        rackPoint = new RackPoint(d.X * along - d.Z * c, d.Z * along + d.X * c);
        RebuildRack(eco);
        Refresh(true);
    }

    private void TurnRack(ForestEcologyController eco)
    {
        rackAngle = RackGeometry.NormaliseAngle(rackAngle + 90f);
        RebuildRack(eco);
        Refresh(true);
    }

    private void ConfirmRackFromMap(ForestEcologyController eco)
    {
        if (rackProposal == null) return;
        ui.Manager.ConfirmRack(rackProposal, "map");
        ui.RackLayout?.Update(true);
        RebuildRack(eco);
        Refresh(true);
    }

    // The rack card at the top of the side panel: figures from the same proposal the forest tool shows, and the few controls.
    private void BuildRackCard(ForestEcologyController eco)
    {
        VisualElement card = UiKit.Box("card", "rack-panel");
        card.name = "map-rack-card";
        RackLayoutTracker layout = ui.RackLayout;
        UiKit.Add(card, "Plan an extraction rack", "rack-heading");
        if (layout != null && layout.Total > 0)
            UiKit.Add(card, $"Planned layout: {layout.Done} of {layout.Total} racks in the Work Plan. A dashed line on the map is a rack still to do.", "muted");
        RackProposal p = rackProposal;
        if (p == null)
            UiKit.Add(card, "Click the map on the planted row you want to open.", "body");
        else
        {
            if (p.LayoutIndex >= 0) UiKit.Add(card, $"Planned rack {p.LayoutIndex + 1} of {p.LayoutCount}", "body");
            else if (p.LayoutCount > 0) UiKit.Add(card, "Not on the planned layout", "body");
            if (!p.CanConfirm)
                UiKit.Add(card, string.IsNullOrEmpty(p.Problem) ? "No tree can be felled on this line." : p.Problem, "body");
            else
            {
                UiKit.Add(card, $"{p.TreeCount} tree{(p.TreeCount == 1 ? "" : "s")}", "rack-line");
                UiKit.Add(card, $"Estimated timber: {UiKit.F(p.SoldVolumeM3, "0.0")} m³ sold ({UiKit.F(p.StemVolumeM3, "0.0")} m³ stem volume)", "body");
                UiKit.Add(card, $"Estimated net return: {UiKit.SignedMoney(p.NetCents)} (timber {UiKit.Money(p.RevenueCents)}, harvesting {UiKit.Money(p.CostCents)}; this rack on its own)", "body");
            }
            if (p.Selection.SkippedCropTrees > 0) UiKit.Add(card, $"{p.Selection.SkippedCropTrees} Crop Tree(s) on the line are kept.", "muted");
        }
        VisualElement row = UiKit.Row(card, "row-wrap");
        row.Add(UiKit.Button("◀ Row", () => NudgeRack(eco, -1), rackProposal != null));
        row.Add(UiKit.Button("Row ▶", () => NudgeRack(eco, 1), rackProposal != null));
        row.Add(UiKit.Button("Turn 90°", () => TurnRack(eco), rackProposal != null));
        Button add = UiKit.Button("Add rack to Work Plan", () => ConfirmRackFromMap(eco), p != null && p.CanConfirm, "btn-primary");
        add.name = "map-rack-confirm";
        card.Add(add);
        Button cancel = UiKit.Button("Cancel", ToggleRackMode, true, "btn-ghost");
        cancel.name = "map-rack-cancel";
        card.Add(cancel);
        UiKit.Add(card, "Nothing is cut until you approve the Work Plan.", "faint");
        side.Add(card);
    }

    // The direct-editing controls for a V2 plan (RACK-V2-05): the derived rack count, the selected rack, how to edit, the last result,
    // and the commands. Every command goes to the editor, which goes to the authoritative plan; nothing is decided here.
    private void BuildEditCard()
    {
        ScenarioOneManager manager = ui.Manager;
        VisualElement card = UiKit.Box("card", "rack-panel");
        card.name = "map-edit-card";
        // The counter is the tracker's derived line: Racks planned while the plan is open, Rack entrances marked once it is confirmed.
        Label counter = UiKit.Add(card, ui.RackLayout != null ? ui.RackLayout.V2CounterText
            : $"Racks planned: {manager.PlannedRackCount} / {ScenarioOneManager.RackPlanTargetCount}", "body");
        counter.name = "map-edit-counter";
        editStatus = null;
        editSpacing = null;
        if (!manager.RackEditingOpen)
        {
            BuildConfirmedCard(card);
            side.Add(card);
            return;
        }

        RackPlanRack selected = null;
        bool hasSelected = editor.SelectedRackId > 0 && manager.TryGetRack(editor.SelectedRackId, out selected);
        if (hasSelected)
        {
            Label heading = UiKit.Add(card, "Rack " + selected.Id, "rack-heading");
            heading.name = "map-edit-selected";
            UiKit.Add(card, $"{UiKit.F(selected.LengthM, "0.0")} m long, pointing {UiKit.F(StandMapRackEditor.Bearing(selected.Entrance, selected.Far), "0")}° from north", "body");
            if (selected.IsLocked) UiKit.Add(card, "Harvested: its geometry is locked and cannot be changed.", "muted");
        }
        else
            UiKit.Add(card, "Drag from the headland into the stand to draw a rack. Click a rack to select it.", "body");
        UiKit.Add(card, "Drag a rack to slide it along the headland. Drag its round handle to rotate it. Hold Shift to turn snapping off.", "faint");

        UiKit.Add(card, "Recommended spacing for this stand: 16–20 m (advisory).", "body");
        editSpacing = UiKit.Add(card, editor.SpacingText(), "body");
        editSpacing.name = "map-edit-spacing";

        editStatus = UiKit.Add(card, editor.Status, "body");
        editStatus.name = "map-edit-status";
        ShowLiveEditStatus();

        BuildConsentPanel(card);
        VisualElement row = UiKit.Row(card, "row-wrap");
        Button delete = UiKit.Button("Delete rack", editor.DeleteSelected, hasSelected && !selected.IsLocked, "btn-ghost");
        delete.name = "map-edit-delete";
        row.Add(delete);
        Button undo = UiKit.Button("Undo last rack edit", editor.UndoLast, manager.CanUndoRackEdit, "btn-ghost");
        undo.name = "map-edit-undo";
        row.Add(undo);
        Button reset = UiKit.Button("Reset to suggested layout", editor.RequestReset, !editor.ResetPending, "btn-ghost");
        reset.name = "map-edit-reset";
        card.Add(reset);

        if (editor.ResetPending)
        {
            VisualElement confirm = UiKit.Box("card", "rack-panel");
            confirm.name = "map-edit-reset-panel";
            UiKit.Add(confirm, $"Discard your {manager.PlannedRackCount} planned rack{(manager.PlannedRackCount == 1 ? "" : "s")} and restore the suggested layout?", "body");
            VisualElement choices = UiKit.Row(confirm, "row-wrap");
            Button yes = UiKit.Button("Reset layout", editor.ConfirmReset, true, "btn-primary");
            yes.name = "map-edit-reset-confirm";
            choices.Add(yes);
            Button no = UiKit.Button("Keep my racks", editor.CancelReset, true, "btn-ghost");
            no.name = "map-edit-reset-cancel";
            choices.Add(no);
            card.Add(confirm);
        }
        BuildConfirmControl(card);
        side.Add(card);
    }

    // Confirm Rack Plan: enabled only for a valid plan of exactly five racks, otherwise the honest reason is printed beside it. The
    // button calls the authoritative confirmation, which creates or refreshes Pending removal work and approves or executes nothing.
    private void BuildConfirmControl(VisualElement card)
    {
        bool can = ui.Manager.CanConfirmRackPlan(out string reason);
        Button confirm = UiKit.Button("Confirm Rack Plan", ConfirmRackPlanFromCard, can && !editor.ConsentPending, "btn-primary");
        confirm.name = "map-rack-plan-confirm";
        card.Add(confirm);
        if (!can)
        {
            Label why = UiKit.Add(card, reason, "muted");
            why.name = "map-rack-plan-confirm-reason";
        }
        else UiKit.Add(card, "Confirming puts the removal work in the Work Plan as pending. Nothing is approved or cut.", "faint");
    }

    private void ConfirmRackPlanFromCard()
    {
        RackPlanOperationResult result = ui.Manager.ConfirmRackPlanFromMap();
        if (result.RequiresApprovalWarning)
        {
            editor.AskConsent(result, "Confirmed the rack plan.", () => ui.Manager.ConfirmRackPlanFromMap(true));
            return;
        }
        FinishPlanChange(result);
    }

    private void FinishPlanChange(RackPlanOperationResult result)
    {
        editor.ClearSelection();
        editor.NoteStatus(result.Message, !result.Succeeded);
        ui.MapSource.MarkRacksDirty();
        ui.RackLayout?.Update(true);
        Refresh(true);
    }

    // The approved-work warning for an edit or a confirmation: what would be cancelled, and the two answers.
    private void BuildConsentPanel(VisualElement card)
    {
        if (!editor.ConsentPending) return;
        VisualElement panel = UiKit.Box("card", "rack-panel");
        panel.name = "map-consent-panel";
        UiKit.Add(panel, editor.ConsentMessage, "body");
        VisualElement choices = UiKit.Row(panel, "row-wrap");
        Button yes = UiKit.Button("Cancel approved work", editor.AcceptConsent, true, "btn-primary");
        yes.name = "map-consent-accept";
        choices.Add(yes);
        Button no = UiKit.Button("Keep approved work", editor.DeclineConsent, true, "btn-ghost");
        no.name = "map-consent-decline";
        choices.Add(no);
        card.Add(panel);
    }

    // The card for a confirmed plan: its racks are fixed, so show what is fixed and offer Reopen Rack Plan. A harvested rack stays
    // locked whatever is reopened; a plan whose racks are all harvested has nothing left to reopen.
    private void BuildConfirmedCard(VisualElement card)
    {
        ScenarioOneManager manager = ui.Manager;
        ScenarioOneManager.RackReopenSummary summary = manager.DescribeRackReopen();
        UiKit.Add(card, "The rack plan is confirmed, so its racks are fixed until it is reopened.", "muted");
        if (summary.LockedRacks > 0)
            UiKit.Add(card, $"{summary.LockedRacks} rack{(summary.LockedRacks == 1 ? " has" : "s have")} been harvested and stay locked.", "muted");
        if (!string.IsNullOrEmpty(editor.Status))
        {
            Label status = UiKit.Add(card, editor.Status, "body");
            status.name = "map-edit-status";
        }

        if (reopenPending)
        {
            VisualElement panel = UiKit.Box("card", "rack-panel");
            panel.name = "map-reopen-panel";
            UiKit.Add(panel, $"{summary.RacksWithApprovedWork} rack{(summary.RacksWithApprovedWork == 1 ? " has" : "s have")} approved removal work that has not been carried out. "
                + "Reopening cancels nothing. If you then change one of those racks, you will be asked again before its approved work is cancelled and set up again as pending.", "body");
            VisualElement choices = UiKit.Row(panel, "row-wrap");
            Button yes = UiKit.Button("Reopen the plan", ReopenRackPlanFromCard, true, "btn-primary");
            yes.name = "map-reopen-accept";
            choices.Add(yes);
            Button no = UiKit.Button("Keep the plan confirmed", () => { reopenPending = false; Refresh(true); }, true, "btn-ghost");
            no.name = "map-reopen-decline";
            choices.Add(no);
            card.Add(panel);
            return;
        }

        Button reopen = UiKit.Button("Reopen Rack Plan", RequestReopenRackPlan, !summary.NothingEditable, "btn-primary");
        reopen.name = "map-rack-plan-reopen";
        card.Add(reopen);
        if (summary.NothingEditable)
            UiKit.Add(card, "Every rack has been harvested, so there is nothing left to reopen.", "faint");
    }

    private void RequestReopenRackPlan()
    {
        if (ui.Manager.DescribeRackReopen().RacksWithApprovedWork > 0)
        {
            reopenPending = true;
            Refresh(true);
            return;
        }
        ReopenRackPlanFromCard();
    }

    private void ReopenRackPlanFromCard()
    {
        reopenPending = false;
        FinishPlanChange(ui.Manager.ReopenRackPlan());
    }

    // The status line follows a drag live without rebuilding the card; a refusal is amber, anything else is plain.
    private void ShowLiveEditStatus()
    {
        if (editStatus == null) return;
        editStatus.text = editor.Status;
        if (editSpacing != null) editSpacing.text = editor.SpacingText();
        editStatus.style.color = editor.StatusIsProblem ? new StyleColor(UiKit.WarningColour) : new StyleColor(StyleKeyword.Null);
    }

    // What the selected layer shows, in one or two sentences, with the colour named in words.
    private string LayerNote(ForestEcologyController eco, StandMapModel model)
    {
        switch (layer)
        {
            case Layer.Light: return "Ground light reaching each 5 m square, 0 (dark) to 1 (open). Blue is shade, warm is open ground. Point at the map for a square's figure.";
            case Layer.Regeneration: return "Young trees by 5 m square, shaded by the tallest cohort: pale for a few seedlings, strong green from 3 m. Green dots are planted juveniles.";
            case Layer.Browse:
            {
                float pressure = eco.Browsing.BackgroundPressure;
                return $"Site browsing pressure: {BrowsingConditions.PressureBand(pressure)}. A ring is a shelter: blue while it protects next year, grey once it has expired.";
            }
            case Layer.Marks:
            {
                int fell = 0, crop = 0;
                foreach (MapTree t in model.Trees) { if (t.Mark == MapTreeMark.Fell) fell++; else if (t.Mark == MapTreeMark.Crop) crop++; }
                return $"Rings mark the trees you have marked, and the forest is dimmed so they stand out: red to fell ({fell}), blue Crop Tree ({crop}).";
            }
            default: return "Your position, the planned racks and the forest path. Select a place, then walk there.";
        }
    }

    private void BuildSide(ForestEcologyController eco, StandMapModel model)
    {
        side.Clear();
        reviewLayer.Hide();
        // Planning a rack: the rack card is the whole side panel (the square diagnosis is back when the mode is off).
        if (rackMode)
        {
            if (ui.Manager.RackPlanningV2Active)
            {
                UiKit.Add(side, "Planning Racks — Active", "heading");
                UiKit.Add(side, ui.Manager.RackPlanConfirmed ? "Solid lines are your confirmed plan."
                    : "Dashed lines are suggested routes. Solid lines are your planned racks.", "body");
                BuildEditCard();
            }
            else BuildRackCard(eco);
            return;
        }
        if (layer == Layer.Review)
        {
            ScenarioOneTreatmentReview.Presentation view = review.BuildCard(side);
            review.ShowLayer(reviewLayer, model, view);
            return;
        }
        if (selectedCell < 0 || selectedCell >= eco.Cells.Length)
        {
            UiKit.Add(side, "Select a square to see its diagnosis.", "body");
            return;
        }
        ForestEcologyCell c = eco.Cells[selectedCell];
        VisualElement diagnosis = side;
        if (layer == Layer.Access)
        {
            UiKit.Add(side, "Access and position", "heading");
            UiKit.Add(side, LayerNote(eco, model), "map-note");
            UiKit.Add(side, ui.Learning.Summary, "body");
            if (ui.RackLayout != null && ui.RackLayout.Total > 0)
                UiKit.Add(side, $"Access racks: {ui.RackLayout.Done} of {ui.RackLayout.Total} planned or open.", "muted");
            UiKit.Add(side, "Selected destination: " + UiKit.CellLabel(selectedCell, eco.CellsPerAxis), "body");
            var details = new Foldout { text = "Site details · light and young trees", value = false, name = "map-site-details" };
            side.Add(details);
            diagnosis = details;
        }
        else
            UiKit.Add(side, LayerNote(eco, model), "map-note");
        UiKit.Add(diagnosis, "Square " + UiKit.CellLabel(selectedCell, eco.CellsPerAxis), "heading");
        UiKit.Line(diagnosis, "Ground light", $"{UiKit.F(c.Light, "0.00")} ({RegenerationDiagnostics.LightBand(c.Light)})");
        UiKit.Line(diagnosis, "Canopy cover", UiKit.F(c.Canopy, "0.00"));
        UiKit.Add(diagnosis, "Regeneration: " + ScenarioOneUiFacts.RegenerationSummary(eco, selectedCell, ui.Manager.PlantedJuveniles), "body");

        float pressure = eco.Browsing.BackgroundPressure;
        var diagnoses = RegenerationDiagnostics.DiagnoseCell(eco, selectedCell);
        ForestTreeSpawner spawner = Object.FindFirstObjectByType<ForestTreeSpawner>();
        foreach (PlantedJuvenile j in ui.Manager.PlantedJuveniles.Where(j => j.alive && string.IsNullOrEmpty(j.promotedTreeId) && eco.GetCellIndex(j.position) == selectedCell).Take(4))
            diagnoses.Add(RegenerationDiagnostics.Diagnose(eco, j.position, ui.Manager.PlantedJuveniles, spawner));
        if (diagnoses.Count == 0)
        {
            RegenerationDiagnosis empty = default;
            empty.Light = c.Light;
            UiKit.Add(diagnosis, "Why: " + ScenarioOneUiFacts.Why(empty, eco, selectedCell), "muted");
        }
        foreach (RegenerationDiagnosis d in diagnoses.Take(6))
        {
            UiKit.Add(diagnosis, $"{d.SpeciesName}{(d.IsPlantedIndividual ? " " + d.JuvenileId : "")}, {UiKit.F(d.Height, "0.00")} m — {ScenarioOneUiFacts.BrowseState(d, pressure)}", "body");
            UiKit.Add(diagnosis, "Why: " + ScenarioOneUiFacts.Why(d, eco, selectedCell), "muted");
        }
        if (layer == Layer.Marks)
        {
            Dictionary<int, (int fell, int crop)> marks = CountMarks(eco, model);
            marks.TryGetValue(selectedCell, out var m);
            UiKit.Line(diagnosis, "Marked to fell here", m.fell.ToString(UiKit.Inv));
            UiKit.Line(diagnosis, "Crop Trees here", m.crop.ToString(UiKit.Inv));
        }

        int index = selectedCell;
        side.Add(UiKit.Button(waypointCell == index ? "Clear waypoint" : "Set waypoint", () =>
        {
            if (waypointCell == index) ClearWaypoint(); else SetWaypoint(eco, index);
            Refresh(true);
        }, true, "btn-primary"));
        UiKit.Add(side, "A waypoint marks the square in the forest and in the status panel. Walk there yourself; the map does not move you.", "faint");
    }

    // Review navigation: the square holding a world point becomes the destination, exactly as if it had been chosen on the map.
    private bool SetWaypointAtWorld(float x, float z)
        => ui.Ecology != null && SetWaypointCell(ui.Ecology, ui.Ecology.GetCellIndex(new Vector3(x, 0f, z)));

    public bool SetWaypointCell(ForestEcologyController eco, int index)
    {
        if (eco == null || eco.Cells == null || index < 0 || index >= eco.CellCount) return false;
        selectedCell = index;
        SetWaypoint(eco, index);
        Refresh(true);
        return true;
    }

    private void SetWaypoint(ForestEcologyController eco, int index)
    {
        waypointCell = index;
        journeyStartedAway = ui.Player != null && eco.GetCellIndex(ui.Player.transform.position) != index;
        ui.Learning.Record("map.waypoint");
        PlaceWaypointMarker(eco, index);
    }

    private void PlaceWaypointMarker(ForestEcologyController eco, int index)
    {
        Vector2 centre = eco.Cells[index].Center;
        if (waypointMarker == null)
        {
            // A tall thin post visible over regeneration; no collider so it never blocks play.
            waypointMarker = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            waypointMarker.name = "Scenario One Waypoint";
            Object.Destroy(waypointMarker.GetComponent<Collider>());
            waypointMarker.transform.localScale = new Vector3(0.15f, 3f, 0.15f);
            Renderer renderer = waypointMarker.GetComponent<Renderer>();
            renderer.material.color = new Color(1f, 0.85f, 0.2f);
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }
        float ground = 0f;
        if (Physics.Raycast(new Vector3(centre.x, 200f, centre.y), Vector3.down, out RaycastHit hit, 400f, ~0, QueryTriggerInteraction.Ignore))
            ground = hit.point.y;
        waypointMarker.transform.position = new Vector3(centre.x, ground + 3f, centre.y);
    }

    // The cell indices held here belong to one grid. When the stand geometry changes (a load, or Reference
    // preview entering and leaving Legacy40) the selection and waypoint of the old grid are set aside rather
    // than reinterpreted on the new grid, and the waypoint is put back if that geometry returns, so previewing
    // the Reference does not lose it. Presentation state only; it never enters a save.
    private int heldGeometry = StandGeometryModel.Legacy40;
    private int stashedGeometry = -1, stashedWaypointCell = -1;

    public void OnStandGeometryApplied(ForestEcologyController eco, int newModel)
    {
        if (newModel == heldGeometry)
            return;
        if (waypointCell >= 0)
        {
            stashedGeometry = heldGeometry;
            stashedWaypointCell = waypointCell;
        }
        selectedCell = -1;
        waypointCell = -1;
        editor.ClearSelection();
        DestroyWaypoint();
        shownKey = "";
        // The squares and the survey labels belong to the old geometry.
        cells = null;
        labelledSize = -1f;
        grid.Clear();
        heldGeometry = newModel;
        if (stashedWaypointCell >= 0 && stashedGeometry == newModel && eco?.Cells != null && stashedWaypointCell < eco.Cells.Length)
        {
            waypointCell = stashedWaypointCell;
            journeyStartedAway = ui.Player != null && eco.GetCellIndex(ui.Player.transform.position) != waypointCell;
            PlaceWaypointMarker(eco, waypointCell);
            stashedGeometry = -1;
            stashedWaypointCell = -1;
        }
    }

    private void ClearWaypoint()
    {
        waypointCell = -1;
        DestroyWaypoint();
    }

    public void DestroyWaypoint()
    {
        if (waypointMarker != null)
            Object.Destroy(waypointMarker);
        waypointMarker = null;
    }

    public bool TryGetWaypointTarget(out Vector2 target)
    {
        ForestEcologyController eco = ui.Ecology;
        target = default;
        if (waypointCell < 0 || eco == null || eco.Cells == null || waypointCell >= eco.Cells.Length)
            return false;
        target = eco.Cells[waypointCell].Center;
        return true;
    }

    public string WaypointDescription(Vector3 playerPosition)
    {
        ForestEcologyController eco = ui.Ecology;
        if (!TryGetWaypointTarget(out Vector2 target)) return "";
        Vector2 delta = target - new Vector2(playerPosition.x, playerPosition.z);
        bool walking = ui.CurrentScreen == ScenarioOneUiRoot.UiScreen.None && !ui.Manager.AnyPanelOpen;
        if (walking && journeyStartedAway && delta.magnitude < eco.CellSizeMeters * 0.5f)
        {
            ui.Learning.Record("map.arrive");
            if ((ui.Marking != null && ui.Marking.AimingAtGround && eco.GetCellIndex(ui.Marking.AimedGroundPoint) == waypointCell)
                || (ui.Player != null && ui.Player.InspectedTree != null && eco.GetCellIndex(ui.Player.InspectedTree.transform.position) == waypointCell))
                ui.Learning.Record("map.inspectsite");
        }
        string cell = UiKit.CellLabel(waypointCell, eco.CellsPerAxis);
        if (delta.magnitude < eco.CellSizeMeters * 0.5f)
            return $"Waypoint {cell}: you are here";
        return $"Waypoint {cell}: {Mathf.RoundToInt(delta.magnitude)} m {Compass(delta)}";
    }

    private static string Compass(Vector2 delta)
    {
        string[] names = { "east", "north-east", "north", "north-west", "west", "south-west", "south", "south-east" };
        float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
        int sector = Mathf.RoundToInt(angle / 45f);
        return names[((sector % 8) + 8) % 8];
    }
}

[exit 0]

$ cat Tools/Verification/RackV2ConfirmMinimap/run_static.py
#!/usr/bin/env python3
"""Read-only static verification for RACK-V2-07 (confirm/reopen/minimap)."""
import argparse
import fnmatch
import os
import re
import subprocess
import sys
from pathlib import Path
DEFAULT_ROOT = str(Path(__file__).resolve().parents[3])
DEFAULT_BASE = "36d82f027e1112529f8d167e20fb67985d28b79d"
A = "Assets/ForestPrototype"
results = []
def report(ok, message):
    results.append((ok, message))
    print(("PASS " if ok else "FAIL ") + message)
def git(root, *args):
    return subprocess.run(["git", "-C", str(root)] + list(args), capture_output=True, text=True)
def read(path):
    try:
        return path.read_text(encoding="utf-8", errors="replace")
    except OSError:
        return None
LITERAL = re.compile(
    r'//[^\n]*|/\*[\s\S]*?\*/|(?:\$@|@\$|\$|@)?"(?:[^"\\]|\\.|"")*"|\'(?:[^\'\\]|\\.)*\'')
def mask_code(text):
    """Blank comments and string/char literals, preserving offsets and newlines."""
    return LITERAL.sub(lambda m: re.sub(r"[^\n]", " ", m.group(0)), text)
def extract_body(text, signature_re, label):
    """Return the original text of a method/property body found by brace matching."""
    m = re.search(signature_re, text)
    if not m:
        return None, "signature not found: " + label
    masked = mask_code(text)
    start = masked.find("{", m.end())
    if start < 0:
        return None, "no opening brace for " + label
    depth = 0
    for i in range(start, len(masked)):
        if masked[i] == "{":
            depth += 1
        elif masked[i] == "}":
            depth -= 1
            if depth == 0:
                return text[start:i + 1], None
    return None, "unbalanced braces for " + label
def split_args(inner):
    args, depth, current = [], 0, []
    for ch in inner:
        if ch == "," and depth == 0:
            args.append("".join(current).strip())
            current = []
        else:
            depth += ch in "([{"
            depth -= ch in ")]}"
            current.append(ch)
    if current:
        args.append("".join(current).strip())
    return args
def add_call_args(text, symbol):
    pos = text.find("Add(MapSymbol." + symbol)
    if pos < 0:
        return None
    open_paren = text.find("(", pos)
    masked, depth = mask_code(text), 0
    for i in range(open_paren, len(masked)):
        depth += masked[i] == "("
        depth -= masked[i] == ")"
        if depth == 0:
            return split_args(text[open_paren + 1:i])
    return None
def check_1(root):
    text = read(root / A / "ScenarioOne/ScenarioOneManager.RackEditing.cs")
    body, err = (None, "file missing") if text is None else extract_body(
        text, r"bool\s+CanConfirmRackPlan\s*\([^)]*\)", "CanConfirmRackPlan")
    if body is None:
        return report(False, "1 " + err)
    ok = ("RackPlanTargetCount" in body and "racks.Count != RackPlanTargetCount" in body
          and "ValidateRackEdit(" in body)
    report(ok, "1 CanConfirmRackPlan reports target count and validates editable racks")
def check_2(root):
    text = read(root / A / "ScenarioOne/ScenarioOneManager.RackEditing.cs")
    body, err = (None, "file missing") if text is None else extract_body(
        text, r"RackPlanOperationResult\s+ConfirmRackPlanFromMap\s*\([^)]*\)", "ConfirmRackPlanFromMap")
    if body is None:
        return report(False, "2 " + err)
    calls = "CanConfirmRackPlan(" in body and "ConfirmRackPlan(" in body
    banned = [t for t in ["ApprovePendingWork", "MarkRackEntrance", "Completed",
                          "rackPlanConfirmed =", "workOrders.Add"] if t in body]
    report(calls and not banned,
           "2 ConfirmRackPlanFromMap delegates only" if calls and not banned
           else "2 calls=%s banned=%s" % (calls, banned))
def check_3(root, base):
    paths = {A + p for p in ["/ScenarioOne/ScenarioOneManager.RackPlan.cs", "/Racks/RackGeometry.cs",
                            "/Racks/RackPlanner.cs", "/Racks/RackEditing.cs", "/Racks/RackTool.cs",
                            "/Racks/PlannedFellMarkers.cs"]}
    for pattern in ["ScenarioOneSuggestedRackLayout*.cs", "ForestSave*.cs", "ScenarioOneSave*.cs"]:
        paths.update(str(p.relative_to(root)) for p in (root / A).rglob("*.cs")
                     if fnmatch.fnmatch(p.name, pattern))
    changed = [p for p in sorted(paths) if git(root, "diff", "--quiet", base, "HEAD", "--", p).returncode != 0]
    report(not changed,
           "3 authoritative rack/save sources unchanged since base (%d files)" % len(paths)
           if not changed else "3 changed since base: " + ", ".join(changed))
def check_4(root):
    mutation = re.compile(r"(PlannedRackCount|MarkedRackEntranceCount|RackPlanTargetCount)\s*(\+\+|--|\+=|-=|=[^=>])")
    hits, skipped = [], 0
    for p in (root / A).rglob("*.cs"):
        for lineno, line in enumerate((read(p) or "").splitlines(), 1):
            if mutation.search(line):
                if "const " in line:
                    skipped += 1
                else:
                    hits.append("%s:%d: %s" % (p, lineno, line.strip()))
    tracker = read(root / A / "UI/RackLayoutTracker.cs")
    body, err = (None, "file missing") if tracker is None else extract_body(
        tracker, r"public\s+string\s+V2CounterText\b", "V2CounterText")
    derived = body is not None and all(
        t in body for t in ["PlannedRackCount", "MarkedRackEntranceCount", "RackPlanConfirmed"])
    msg = "4 no rack-counter mutation; V2CounterText is derived"
    if skipped:
        msg += " (%d const declaration skipped)" % skipped
    if hits or not derived:
        msg = "4 mutations=%s derived=%s" % (hits[:3], derived)
        if body is None:
            msg += " (" + err + ")"
    report(not hits and derived, msg)
def check_5(root):
    source = read(root / A / "UI/Map/StandMapSource.cs")
    body, err = (None, "file missing") if source is None else extract_body(
        source, r"private\s+void\s+ReadRacks\s*\(\s*\)", "ReadRacks")
    reads_both = body is not None and "RackPlan" in body and "GetSuggestedRackLayout" in body
    banned = [".AddRack(", ".UpdateRack(", ".DeleteRack(", "ConfirmRackPlan", "ReopenRackPlan",
              "MarkRackEntrance", "ClearRackEntranceMark", "ReshapeRackFromMap",
              "CreateRackFromMap", "DeleteRackFromMap", "ResetRacksToSuggestion", "ApprovePendingWork"]
    files = ["UI/HudMinimapView.cs", "UI/Map/StandMapSource.cs", "UI/Map/StandMapDrawPlan.cs",
             "UI/Map/StandMapModel.cs", "UI/Map/StandMapSurface.cs", "UI/Map/StandMapCanvas.cs"]
    found = []
    for rel in files:
        text = read(root / A / rel)
        if text is None:
            found.append(rel + ":missing")
            continue
        found.extend(rel + ":" + token for token in banned if token in text)
    report(reads_both and not found,
           "5 ReadRacks reads the plan and suggestions; map/minimap sources are read-only"
           if reads_both and not found else "5 reads_both=%s found=%s" % (reads_both, found[:4]))
def check_6(root):
    source = read(root / A / "UI/Map/StandMapSource.cs")
    body, err = (None, "file missing") if source is None else extract_body(
        source, r"private\s+void\s+ReadRacks\s*\(\s*\)", "ReadRacks")
    if body is None:
        return report(False, "6 " + err)
    lines = body.splitlines()
    def guarded(needle):
        for i, line in enumerate(lines):
            if needle in line:
                return any("!v2" in prev for prev in lines[max(0, i - 3):i])
        return False
    guard_ok = (guarded("foreach (RackLayoutLine line in layout.Lines)")
                and guarded("foreach (RackGroup group in ui.Manager.RackGroups)"))
    leaks = []
    for rel in ["UI/LearningObjectivesView.cs", "UI/LearningObjectivesView.Sequence.cs"]:
        text = read(root / A / rel)
        if text is None:
            leaks.append(rel + ":missing")
            continue
        for lineno, line in enumerate(text.splitlines(), 1):
            if ("V2ObjectiveInstruction" in line or "V2CounterText" in line) and "RackPlanV2" not in line:
                leaks.append("%s:%d" % (rel, lineno))
    report(guard_ok and not leaks,
           "6 legacy layouts are behind !v2; V2 objective text is behind RackPlanV2"
           if guard_ok and not leaks else "6 v2_guards=%s leaks=%s" % (guard_ok, leaks[:4]))
def check_7(root):
    text = read(root / A / "UI/HudMinimapView.cs")
    note = "Dashed lines show suggested rack positions. Solid lines show your confirmed plan."
    ok = text is not None and note in text and "note.pickingMode = PickingMode.Ignore" in text \
        and "Button" not in text
    report(ok, "7 minimap teaching note is present, non-interactive and button-free")
def check_8(root):
    text = read(root / A / "UI/Map/StandMapSymbols.cs")
    suggested = add_call_args(text or "", "RackSuggested")
    player = add_call_args(text or "", "RackPlayer")
    sug_ok = (suggested is not None and len(suggested) >= 9 and suggested[4] == "true"
              and suggested[7].endswith("f") and suggested[8].endswith("f")
              and float(suggested[7][:-1]) > 0 and float(suggested[8][:-1]) > 0)
    player_ok = player is not None and len(player) == 7 and player[4] == "true"
    report(sug_ok and player_ok,
           "8 suggested racks are dashed in the mini map; player racks are solid"
           if sug_ok and player_ok else "8 suggested=%s player=%s" % (suggested, player))
def check_9(root):
    pattern = re.compile(r"(?:FromMap|ResetRacksToSuggestion|UndoLastRackEdit)\([^)]*,\s*true\s*\)")
    bad = []
    for rel in ["UI/Map/StandMapRackEditor.cs", "UI/StandMapView.cs"]:
        text = read(root / A / rel)
        if text is None:
            bad.append(rel + ":missing")
            continue
        for lineno, line in enumerate(text.splitlines(), 1):
            if pattern.search(line) and "=>" not in line:
                bad.append("%s:%d: %s" % (rel, lineno, line.strip()))
    report(not bad, "9 approved cancellation is never granted automatically"
           if not bad else "9 auto-granted consent: " + " | ".join(bad[:3]))
def check_10(root):
    problems = []
    st = git(root, "status", "--porcelain")
    if st.returncode != 0:
        problems.append("git status failed")
    for line in st.stdout.splitlines():
        rel = line[3:].strip().split(" -> ")[-1]
        if rel.startswith(A + "/") and os.path.basename(rel).startswith("RackV2"):
            problems.append("leftover " + rel)
    problems.extend("orphan meta " + str(m.relative_to(root))
                    for m in (root / A).rglob("*.meta") if not m.with_suffix("").exists())
    report(not problems, "10 no leftover RackV2 harness and no orphan .meta under ForestPrototype"
           if not problems else "10 " + "; ".join(problems[:4]))
def main():
    parser = argparse.ArgumentParser(description="Read-only RACK-V2-07 static verification")
    parser.add_argument("--root", default=DEFAULT_ROOT)
    parser.add_argument("--base", default=DEFAULT_BASE)
    parser.add_argument("--out", default=None, help="directory for report.txt")
    args = parser.parse_args()
    root = Path(os.path.abspath(args.root))
    if not (root.is_dir() and (root / A).is_dir()):
        print("FAIL 0 project root not found: " + str(root))
        print("RACK_V2_CONFIRM_MINIMAP_STATIC_FAIL failed=1")
        return 1
    for check in (lambda: check_1(root), lambda: check_2(root), lambda: check_3(root, args.base),
                  lambda: check_4(root), lambda: check_5(root), lambda: check_6(root),
                  lambda: check_7(root), lambda: check_8(root), lambda: check_9(root),
                  lambda: check_10(root)):
        check()
    failed = sum(1 for ok, _ in results if not ok)
    if args.out:
        os.makedirs(args.out, exist_ok=True)
        with open(os.path.join(args.out, "report.txt"), "w", encoding="utf-8") as f:
            for ok, message in results:
                f.write(("PASS " if ok else "FAIL ") + message + "\n")
    if failed:
        print("RACK_V2_CONFIRM_MINIMAP_STATIC_FAIL failed=%d" % failed)
        return 1
    print("RACK_V2_CONFIRM_MINIMAP_STATIC_PASS checks=%d" % len(results))
    return 0
if __name__ == "__main__":
    sys.exit(main())

[exit 0]

$ rg --files -g AGENTS.md -g "*test*" -g "*static*" Tools Assets/ForestPrototype/UI
Tools/Calibration/OakBeechGrowth/test_source_normalization.py
Tools/Build/S1B/test_build_linux.py
Tools/Build/S1B/test_build_windows.py
Tools/Verification/Growfor/test_broad_lynch_2006_sitka.py
Tools/Verification/Growfor/test_compare_ccf.py
Tools/Verification/RackV2ConfirmMinimap/run_static.py
Tools/Verification/Qualification/test_component_ba_evidence.py
Tools/Verification/Qualification/test_density_qualification.py
Tools/Verification/Qualification/test_ux_runner.py
Tools/Verification/Qualification/test_evidence_format.py
Tools/Verification/Qualification/test_run_qualification.py
Tools/Verification/ScenarioOneUx/Qualification/run_f10_self_test.py

[exit 0]

$ git log -1 --format=%H -- AGENTS.md CLAUDE.md Docs/Project/AgentWorkflow.md Docs/Project/decision-log.md Docs/Project/current-milestone.md Docs/Project/game-brief.md Docs/Project/unity-project-overview.md Docs/Project/research-index.md Docs/Project/instructions Tools/ProjectContext/generate_project_instructions.py
63e95ba2175f7e239a278c616681b286719821f8

[exit 0]
```
</details>

## Offline verification commands and raw output

Focused assertions extend the existing read-only harness helpers in-memory; no harness or Assets temporary file was installed. This is source inspection and Boolean wiring verification, not compiled C# or Unity input proof.

```text

$ git diff --check

[exit 0]
```

```text

$ git diff -- Assets/ForestPrototype/UI/Map/StandMapRackEditor.cs Assets/ForestPrototype/UI/StandMapView.cs
diff --git a/Assets/ForestPrototype/UI/Map/StandMapRackEditor.cs b/Assets/ForestPrototype/UI/Map/StandMapRackEditor.cs
index 11b4e214..c565672c 100644
--- a/Assets/ForestPrototype/UI/Map/StandMapRackEditor.cs
+++ b/Assets/ForestPrototype/UI/Map/StandMapRackEditor.cs
@@ -16,7 +16,8 @@ using UnityEngine.UIElements;
 //   move    drag the body of a rack              it slides along the headland; its entrance re-snaps onto the headland
 //   rotate  drag the round handle                the far end follows the pointer; the entrance stays put
 // While dragging, the proposal is drawn as a dashed ghost (white if the rules accept it, red if they refuse it) and the plan is
-// untouched; releasing applies it, or reports why it was refused. Snapping is soft and advisory; holding Shift turns it off.
+// untouched; releasing applies it, or reports why it was refused. The round handle rotates freely. For drawing, snapping is soft
+// and advisory; holding Shift turns it off. Moving keeps the entrance on the headland.
 public sealed class StandMapRackEditor
 {
     private enum Gesture { None, Create, Move, Reshape }
@@ -257,7 +258,7 @@ public sealed class StandMapRackEditor
         Vector2 px = e.localPosition;
         if (!dragged && (px - pressPx).sqrMagnitude < DragStartPx * DragStartPx) return;
         dragged = true;
-        Propose(px, !ShiftHeld(e.modifiers), out RackPoint entrance, out RackPoint far, out RackSnapResult snap, out RackEditValidation check);
+        Propose(px, gesture != Gesture.Reshape && !ShiftHeld(e.modifiers), out RackPoint entrance, out RackPoint far, out RackSnapResult snap, out RackEditValidation check);
         ghost = new RackEditGhost(entrance, far, RackGeometry.DefaultWidthM, check.IsValid);
         ui.MapSource.EditGhost = ghost;
         ui.MapSource.MarkRackEditDirty();
diff --git a/Assets/ForestPrototype/UI/StandMapView.cs b/Assets/ForestPrototype/UI/StandMapView.cs
index 04578a1e..92b3679a 100644
--- a/Assets/ForestPrototype/UI/StandMapView.cs
+++ b/Assets/ForestPrototype/UI/StandMapView.cs
@@ -598,7 +598,7 @@ public sealed class StandMapView
         }
         else
             UiKit.Add(card, "Drag from the headland into the stand to draw a rack. Click a rack to select it.", "body");
-        UiKit.Add(card, "Drag a rack to slide it along the headland. Drag its round handle to rotate it. Hold Shift to turn snapping off.", "faint");
+        UiKit.Add(card, "Drag a rack to slide it along the headland. Its round handle rotates freely. Hold Shift while drawing to turn snapping off.", "faint");
 
         UiKit.Add(card, "Recommended spacing for this stand: 16–20 m (advisory).", "body");
         editSpacing = UiKit.Add(card, editor.SpacingText(), "body");

[exit 0]
```

```text

$ python3 Tools/Verification/RackV2ConfirmMinimap/run_static.py --root . --base 72b4efbd1c653fa2231f632a79030a83ff76f833
PASS 1 CanConfirmRackPlan reports target count and validates editable racks
PASS 2 ConfirmRackPlanFromMap delegates only
PASS 3 authoritative rack/save sources unchanged since base (14 files)
PASS 4 no rack-counter mutation; V2CounterText is derived (1 const declaration skipped)
PASS 5 ReadRacks reads the plan and suggestions; map/minimap sources are read-only
PASS 6 legacy layouts are behind !v2; V2 objective text is behind RackPlanV2
PASS 7 minimap teaching note is present, non-interactive and button-free
PASS 8 suggested racks are dashed in the mini map; player racks are solid
PASS 9 approved cancellation is never granted automatically
PASS 10 no leftover RackV2 harness and no orphan .meta under ForestPrototype
RACK_V2_CONFIRM_MINIMAP_STATIC_PASS checks=10

[exit 0]
```

```text

$ python3 -
STDIN:
import runpy
from pathlib import Path
h = runpy.run_path('Tools/Verification/RackV2ConfirmMinimap/run_static.py', run_name='rack_static_helpers')
s = Path('Assets/ForestPrototype/UI/Map/StandMapRackEditor.cs').read_text()
body, error = h['extract_body'](s, r'private\s+void\s+OnPointerMove\s*\([^)]*\)', 'OnPointerMove')
assert error is None, error
assert 'Propose(px, gesture != Gesture.Reshape && !ShiftHeld(e.modifiers),' in body
assert 'next = Gesture.Reshape;' in s[s.index('&& OnHandle(px, selected))'):s.index('else', s.index('&& OnHandle(px, selected))'))]
for gesture in ['Create', 'Move', 'Reshape']:
    for shift in [False, True]:
        snap = gesture != 'Reshape' and not shift
        assert snap == (False if gesture == 'Reshape' else not shift)
        print(f'PASS snap argument: {gesture}, Shift={shift} -> {snap}')
base = __import__('subprocess').check_output(['git', 'show', 'HEAD:Assets/ForestPrototype/UI/Map/StandMapRackEditor.cs'], text=True)
for signature in ['Propose', 'OnPointerDown', 'OnPointerUp', 'Apply', 'ShiftHeld']:
    if signature == 'ShiftHeld':
        assert s[s.index('private static bool ShiftHeld'):s.index('private void OnPointerMove')] == base[base.index('private static bool ShiftHeld'):base.index('private void OnPointerMove')]
    else:
        pattern = r'private\s+void\s+' + signature + r'\s*\([^)]*\)'
        current, err = h['extract_body'](s, pattern, signature)
        original, err2 = h['extract_body'](base, pattern, signature)
        assert err is None and err2 is None and current == original, signature
    print('PASS unchanged method: ' + signature)
assert 'Its round handle rotates freely. Hold Shift while drawing to turn snapping off.' in Path('Assets/ForestPrototype/UI/StandMapView.cs').read_text()
print('PASS paired instruction text')

PASS snap argument: Create, Shift=False -> True
PASS snap argument: Create, Shift=True -> False
PASS snap argument: Move, Shift=False -> True
PASS snap argument: Move, Shift=True -> False
PASS snap argument: Reshape, Shift=False -> False
PASS snap argument: Reshape, Shift=True -> False
PASS unchanged method: Propose
PASS unchanged method: OnPointerDown
PASS unchanged method: OnPointerUp
PASS unchanged method: Apply
PASS unchanged method: ShiftHeld
PASS paired instruction text

[exit 0]
```

```text

$ git status --short
 M Assets/ForestPrototype/UI/Map/StandMapRackEditor.cs
 M Assets/ForestPrototype/UI/StandMapView.cs
?? Docs/Handoffs/Overnight-Continuation-20261104/

[exit 0]
```

## Result and affected paths

Status: **IMPLEMENTED** on task branch only; independent review and integration remain outstanding. No ownership or policy conflict found. Live GitHub coordination confirms #256's bounded UI scope; local coordination is older and was not edited.

Only `StandMapRackEditor.cs`, `StandMapView.cs`, and this handoff changed. No serialized assets, .meta files, packages, save fields, authoritative rack policy or frozen reference changed. No unrelated work existed or was modified.

The selected rack's round far-end handle is hit-tested before its body and selects `Gesture.Reshape`. Its pointer-move proposal now receives `snap=false` regardless of Shift. The entrance remains `originEntrance`; existing validation still runs. Release still applies the last displayed ghost via the unchanged manager facade. Free rotation does not mean invalid geometry becomes legal.

Create receives the same `!ShiftHeld(e.modifiers)` value as before. Move receives that same value too, but its existing `MoveAlongHeadland` branch does not consume advisory snapping; its headland constraint stays active with or without Shift. Help now says: “Drag a rack to slide it along the headland. Its round handle rotates freely. Hold Shift while drawing to turn snapping off.” The nearby comment makes the same distinction.

The exact production diff above includes all before/after expressions. Focused source assertions are reproducible from the recorded stdin. They use the existing harness extraction helpers without changing its owned Tools files. No temporary script or generated meta was added under Assets. No Unity harness or save-touching test was run.

## UNVERIFIED / remaining gates

- Unity 6000.6.0f1 import and C# compilation.
- Actual mouse/keyboard delivery, rotation continuity, ghost/release agreement, Shift delivery, rendered text fit and map rendering.
- Runtime geometry refusal, headland enforcement, five-rack and Work Plan gates, save/load and broader gameplay regression. Relevant source paths are unchanged; this is not runtime proof.
- Player build and Windows behaviour; neither was run.
- Independent review, integration, acceptance and dashboard synchronization. #200/DPC retain coordination and routing-ledger ownership; this handoff returns evidence for them to publish.

Reason: user explicitly prohibited Unity/Editor/Player execution tonight: approximately 19 GiB free versus a 15 GiB floor plus a 10–12 GiB cold import. No admission bypass or machine-state claim was made.

## Manual Unity test script for a later authorised session

1. After fresh disk/process/lease admission, open this exact branch's candidate in Unity 6000.6.0f1. Wait for import/compile and inspect Console for errors. Use an isolated disposable Scenario One Enlarged80 session; preserve the player's save.
2. Open Stand Map, choose Plan Racks, and ensure the V2 plan is open and has fewer than five racks (delete only a disposable planned rack if necessary). Read the instruction line: it should describe free handle rotation and Shift for drawing.
3. With Shift released, draw from an empty headland position into the stand, slowly passing near a suggested rack direction/stand edge. Expect advisory drawing snap and its status indication where applicable. Repeat with Shift held: expect no advisory angle/edge snapping. Entrance validity stays constrained in both cases. Undo between trials.
4. Select an editable rack, grab its round far-end handle, and slowly drag across a suggested angle and parallel-to-neighbour angle with Shift released. Expect continuous unsnapped preview, a fixed entrance and no advisory snap indication. Release at a valid non-snapped angle: expect the stored rack to match its last ghost. Repeat holding Shift: same unsnapped rotation. Invalid geometry should show refusal and leave the rack unchanged.
5. Drag the rack body along the headland, first without Shift and then with Shift. Expect the rack to slide, retain its orientation/length, and keep its entrance on the headland as before; Shift must not permit an interior entrance. Release and compare with the last ghost.
6. On disposable geometry, attempt an invalid rotated segment and a sixth rack. Expect the existing refusal/count limits. Confirming a rack plan must still require exactly five valid racks and must not approve or execute Work Plan work. Reopen/locked rack rules should remain unchanged.

All expected observations above require later human execution; none is reported as observed tonight.

## V2 Orchestration Decision Appendix

Journal citation: **EVENT 010 (pool codex / gpt-6.1-sol medium)**. Actual worker runtime: T3 Code Codex harness, `gpt-6.1-sol`, medium reasoning. Role: implementation sub-agent, performing the work directly; no child agents, provider switch or effort escalation. Provider quota/window/reset, token consumption and precise total runtime: UNKNOWN. User supplied the assigned lane and branch; no other lane was inspected or modified.

Routing rationale: bounded UI wiring and copy fix, suitable for the assigned Medium worker with local Git and offline Python. Policy changes were unnecessary. Cheapest relevant gate: read-only static harness plus focused source assertions. Unity admission deliberately deferred by binding user instruction. Outcome: IMPLEMENTED, ready to return for independent review with explicit missing Unity evidence. No self-approval. DPC must append this evidence to its ledger; this worker did not edit it.

```text
$ git diff --check
[exit 0]
```

```text
$ git diff --stat
 Assets/ForestPrototype/UI/Map/StandMapRackEditor.cs | 5 +++--
 Assets/ForestPrototype/UI/StandMapView.cs           | 2 +-
 2 files changed, 4 insertions(+), 3 deletions(-)
[exit 0]
```

```text
$ git diff --name-only
Assets/ForestPrototype/UI/Map/StandMapRackEditor.cs
Assets/ForestPrototype/UI/StandMapView.cs
[exit 0]
```

```text
$ git add Assets/ForestPrototype/UI/Map/StandMapRackEditor.cs Assets/ForestPrototype/UI/StandMapView.cs Docs/Handoffs/Overnight-Continuation-20261104/RACK-HANDLE-HANDOFF.md
[exit 0]
```
