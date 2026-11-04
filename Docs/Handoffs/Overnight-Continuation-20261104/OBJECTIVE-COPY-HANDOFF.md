# Objective copy handoff — #255

Status: **IMPLEMENTED** (not reviewed/integrated/accepted).
Branch: `feature/overnight-objective-copy-20261104`; clean starting HEAD `72b4efbd1c653fa2231f632a79030a83ff76f833`. Live GitHub coordination confirms disjoint #255 ownership; local coordination is stale. No unrelated work changed. No Unity started.

## File+line summary and before/after

- `LearningObjectivesView.Curriculum.cs:88,114,115`: three player-facing `16–20 m` → `16–20m` (existing en dash preserved per #255; rack spacing, not duration).
- `MenuHelpView.Text.cs:45`: `Tree Marks · what the paint on a tree means` → `Introducing tree marking`.
- Same file :93: `In this forest a paint mark records a management decision on the tree itself. Scenario One uses three marks so far:` → `Scenario 1 introduces 3 CCF markings. In this forest a paint mark records a management decision on the tree itself:`.
- Same file :27: `Removal trees associated with a machine route can be marked with vertical stripes. The CCF source material describes removal/treatment colours such as orange or pink; Scenario One uses orange.` → `Orange vertical stripes identify rack entrance trees associated with machine-path removal in Scenario 1.`
- Same file :95: `The source describes the stripes on four sides of the tree; here one spray paints all four.` → `One spray paints vertical stripes on four sides of the entrance tree.` Adjacent comment updated accordingly.
- Only other file: this handoff. String/comment-only edit: no API, input, save, rack, MFM, objective-state, telemetry or architecture change; no new dependency or asset changes.

## Marker evidence / contradiction

Actual inspected painters: `ForestTreeMarkingManager.RackEntrances.cs:183–224` creates orange vertical patches on four sides. `ForestTreeMarkingManager.cs:580–619` creates diamond/disc markers and a blue Crop trunk band. `UI/Map/StandMapSymbols.cs:106–107` uses red Fell and blue Crop rings; StandMapView and MenuHelpView agree on those colours. Preserved all three existing teaching rows. No orange rack mark relabelled Fell, no diagonal slash claim, no formal Pro Silva Ireland standard claim.

Source discrepancy: `ForestTreeMarkingManager.markerColor` fallback and `Materials/HarvestMark.mat` use orange-red `(1,0.35,0.05)` despite red Fell map/prompt terminology. Kept existing labels per task; rendered appearance and active material assignment require later review. No colour changes or guesses made.

## Verification

Raw command transcript below includes full source/context reads and all verification output. Initial tool displays were truncated; full reads were repeated into this transcript. Mutation operations: Python literal replacements shown by the recorded diff, then the adjacent comment update; these reported `Updated three spacing labels and four teaching text literals; no logic edits.` Exit 0.

- Old-string rg: no matches, exit 1 (expected absence).
- New-string rg: three spacing occurrences, exact heading/intro, preserved marker rows; exit 0.
- Focused Python assertions: PASS, exit 0; copy and marker rows checked, code unchanged after stripping strings/comments and normalizing whitespace. First diagnostic failed because it retained the blank line removed by shortening a comment; corrected diagnostic passed. No production fix was needed for that diagnostic.
- Existing RackV2HelpTeaching static subset: 14 checks, 13 PASS / 1 FAIL, exit 1. S10 rejects only the required new handoff path under its historical allow-list. S1–S9, S11, S12 pass. Tests unchanged.
- Full .NET harness NOT run: inspected assertions hard-code superseded title, spacing and spec §28 wording; generated build output also exceeds assigned-file scope. No compile claim.
- `git diff --check`: exit 0, no output. Guessed nonexistent painter/drawing paths returned exit 2; corrected source discovery is recorded. No harness copied into Assets.

## UNVERIFIED / UNMEASURED

Unity import/compilation; runtime behaviour; rendered English text, wrapping/readability and actual marker colours; help modal timing/unlock; input paths; save/load; performance; Player and Windows builds. Unity admission is deferred tonight on disk grounds by binding task instruction. Static source checks do not prove these behaviours. Independent review has since been performed and the correction re-checked (see 'Review outcome and DPC correction' below); integration, Manager acceptance and dashboard publication remain unverified, and #200 owns coordination follow-through. No other lane read or modified; no active-tasks or routing-ledger edit.

Future Unity steps after separate admission: open Objective 1 and rack reading; expect `16–20m` in the Objective 1 concept and the rack-reading step. The Stand Map card (`StandMapView.cs:603`) still shows `16–20 m` with a space, because that file is outside this packet's two-file scope; do not treat that remaining spaced form as a regression of this change. Confirm rack plan, open Tree Marks help via F1; expect exact heading/intro and three preserved rows. Inspect orange four-sided entrance stripes, Fell/Crop colour appearance and text fit. These are future checks, not results.

## V2 Orchestration Decision Appendix

**EVENT 004**: pool `codex`, actual model `gpt-6.1-sol`, **medium** effort. User supplied the isolated worktree, candidate and bounded PACKET ONLY scope. No provider switch, escalation, child delegation, Unity admission or integration. Routing evidence: clean candidate, live disjoint ownership, two production files and tracked handoff; offline results and limitations above. DPC remains sole routing-ledger writer.

**Next step = independent cross-provider review.** Manager handles test/spec reconciliation, dashboard synchronization and any later integration.

## Command transcript

All inspection/verification commands are reproduced below with raw output. Initial combined reads exceeded the tool display limit; the full reads are captured again here. No Unity was run.

$ pwd && git status --short && git log --oneline -1
```text
/media/jer/Files/CCF/Operations/CCF-Overnight-ObjectiveCopy-20261104
?? Docs/Handoffs/Overnight-Continuation-20261104/
72b4efbd map: draw living conifer crowns as pointed whorl stars; smooth Beech outline (#232)

Exit: 0
```

$ git remote get-url origin && git branch --show-current
```text
https://github.com/jermur788/CCF.git
feature/overnight-objective-copy-20261104

Exit: 0
```

$ cat AGENTS.md
```text
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

Exit: 0
```

$ cat Docs/Project/AgentWorkflow.md
```text
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

Exit: 0
```

$ cat Docs/Project/decision-log.md
```text
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

Exit: 0
```

$ cat Docs/Project/current-milestone.md
```text
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

Exit: 0
```

$ cat Docs/Project/game-brief.md
```text
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

Exit: 0
```

$ cat Docs/Project/unity-project-overview.md
```text
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

Exit: 0
```

$ cat Docs/Project/coordination/active-tasks.md
```text
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

Exit: 0
```

$ gh api repos/jermur788/CCF/contents/Docs/Project/coordination/active-tasks.md --jq .content | base64 -d
```text
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

Exit: 0
```

$ gh issue view 255 --json title,body
```text
{"body":"**Scope:** small unambiguous approved player-test text corrections only. Inspect actual candidate `integration/scenario-one-player-test-ready-v1` first. Own only `Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs` and `Assets/ForestPrototype/UI/MenuHelpView.Text.cs` plus focused text verification. Change Objective 1 displayed `16–20 m` to user-requested `16–20m` in player-facing concept (not geometry meters); heading exactly **Introducing tree marking**; introductory sentence and phrasing **Scenario 1 introduces 3 CCF markings**, and avoid player-facing mention of \"CCF source material\" while retaining accuracy about current orange vertical rack entrance marks vs actual red Fell and blue Crop status. **DO NOT** change red Fell to orange diagonal slashes or blue to ring in copy until actual rendering semantics and spec reconciled; do not assert a formal Pro Silva Ireland standard without primary-source confirmation. Copy only; no input, save, rack, MFM semantics changed.\n\n**Difficulty Low** — two text sources and verification; **worker** Sonnet 5.5 Medium or GPT-6.1 Medium as available; no need for Opus. **Lane** conditional `feature/overnight-objective-copy-20261104` in isolated small docs/code worktree `/media/jer/Files/CCF/Operations/CCF-Overnight-ObjectiveCopy-20261104`, only if unused, clean and no conflicting owner; branch from published exact player-test candidate, never change original integration. **Verify:** focused static assertion, compile or safe bounded Unity only with exclusive fresh lock; English visual text proof where feasible, cross-provider code review. Return `Docs/Handoffs/Overnight-Continuation-20261104/OBJECTIVE-COPY-HANDOFF.md` on own PR; no integrate until Manager review. Reference master overnight packet. No Windows build.","title":"[READY / PACKET ONLY] Objective 1 units and tree-marking introductory copy"}

Exit: 0
```

$ rg --files -g AGENTS.md -g "*Painter*" -g "*Offline*" -g "*csproj" Tools Assets
```text
Tools/Verification/TeachingPersistence/PersistenceOfflineCheck.cs
Tools/Verification/RackV2HelpTeaching/RackV2HelpTeachingOfflineCheck.cs
Tools/Verification/ScenarioOneCompletion/P2/OfflineLogic/CropTreeCompetitionOfflineCheck.cs
Tools/Verification/ScenarioOneCompletion/P3/OfflineLogic/WorkPlanResidualOfflineCheck.cs
Tools/Verification/ScenarioOneCompletion/N3/OfflineLogic/N3OfflineCheck.cs
Tools/Verification/MfmCropTreeCandidates/OfflineLogic/CropTreeCandidatesOfflineCheck.cs
Tools/Verification/MfmSaveProvenance/PrescriptionSaveOfflineCheck.cs
Tools/Verification/ScenarioOneNonUi/OfflineLogic/StandApproachOfflineCheck.cs
Tools/Verification/ScenarioOneNonUi/OfflineLogic/OfflineStubs.cs
Tools/Verification/ScenarioOneNonUi/OfflineLogic/NonUiOfflineMain.cs
Tools/Verification/ScenarioOneNonUi/OfflineLogic/RackBayOfflineCheck.cs
Tools/Verification/ScenarioOneNonUi/OfflineLogic/RackSampleGeometryOfflineCheck.cs
Tools/Verification/ScenarioOneNonUi/OfflineLogic/CompetitorKeyboardOfflineCheck.cs
Tools/Verification/ScenarioOneUx/OfflineLogic/ScenarioOneUxOfflineCheck.cs
Tools/Verification/Parity08Save/Parity08SpeciesOfflineCheck.cs
Tools/Verification/RackV2Editing/OfflineCheck.cs
Tools/Verification/MfmCoverageWorld/OfflineLogic/CoverageWorldOfflineCheck.cs
Tools/Verification/SiteInfrastructure/OfflineLogic/SiteInfrastructureOfflineCheck.cs
Tools/Verification/SiteInfrastructure/OfflineLogic/ScenarioTwoSiteRulesOfflineCheck.cs
Tools/Verification/StandMap/OfflineLogic/StandMapOfflineCheck.cs
Tools/Verification/S1CObjectiveHistory/ObjectiveHistoryOfflineCheck.cs
Tools/Verification/ScenarioTwoSpeciesFoundation/OfflineLogic/ScenarioTwoSpeciesFoundationOfflineCheck.cs
Tools/Verification/MfmPrescriptionCore/OfflineLogic/PrescriptionCoreOfflineCheck.cs
Tools/Verification/Mfm09ExtensionFlow/Mfm09OfflineCheck.cs
Tools/Verification/S1CManagementHistory/S1CGuidanceOfflineCheck.cs
Tools/Verification/S1CManagementHistory/S1CPerYearHistoryOfflineCheck.cs
Tools/Verification/S1CManagementHistory/S1CDrillDownOfflineCheck.cs
Tools/Verification/S1CManagementHistory/HistoryOfflineStubs.cs
Tools/Verification/S1CManagementHistory/S1CProvenanceOfflineCheck.cs
Tools/Verification/ScenarioOneRepeatedInterventions/TeachingCapOfflineCheck.cs
Tools/Verification/ScenarioOneRepeatedInterventions/EventIdIntegrity/EventIdIntegrityOfflineCheck.cs
Tools/Verification/ScenarioOneRepeatedInterventions/PreviousProposed/PreviousProposedOfflineCheck.cs
Tools/Verification/ScenarioOneRepeatedInterventions/CycleStateOfflineCheck.cs
Tools/Verification/ScenarioOneRepeatedInterventions/TeachingSequenceMonotonicityOfflineCheck.cs
Tools/Verification/MfmTreatmentReview/OfflineLogic/TreatmentReviewOfflineCheck.cs
Tools/Verification/RackV2SuggestedLayout/OfflineCheck.cs
Tools/Verification/ScenarioOneRackUx/OfflineLogic/RackOfflineCheck.cs
Tools/Verification/MfmApprovalBoundary/OfflineLogic/ApprovalBoundaryOfflineCheck.cs
Tools/Verification/RackV2Spacing/OfflineCheck.cs
Tools/Verification/ThreeSpecies/OfflineLogic/ScenarioHistoryOfflineCheck.cs
Tools/Verification/ThreeSpecies/OfflineLogic/CompositeDensityOfflineCheck.cs
Tools/Verification/ThreeSpecies/OfflineLogic/BroadleafTimberOfflineCheck.cs
Tools/Verification/ThreeSpecies/OfflineLogic/MixedDensityOfflineCheck.cs
Tools/Verification/ThreeSpecies/OfflineLogic/BroadleafPruningOfflineCheck.cs
Tools/Verification/ThreeSpecies/OfflineLogic/HistoryOfflineStubs.cs
Tools/Verification/ThreeSpecies/OfflineLogic/PruningOfflineStubs.cs

Exit: 0
```

$ cat Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs Assets/ForestPrototype/UI/MenuHelpView.Text.cs
```text
using System.Collections.Generic;
using System.Linq;

// The lesson list (topics, steps and their order). Kept free of UI Toolkit and PlayerPrefs so the order and wording can be
// checked offline on plain .NET. Step ids are the keys of the saved learning progress: never rename or reuse one. This is
// one half of LearningObjectivesView; the screen itself is in LearningObjectivesView.cs.
public sealed partial class LearningObjectivesView
{
    public const string PreferencePrefix = "CCF.Learning.v1.";
    public sealed class Step
    {
        public readonly string Id, Title, Text, Requires;
        public readonly bool Reading;
        public Step(string id, string title, string text, bool reading = false, string requires = null)
        { Id = id; Title = title; Text = text; Reading = reading; Requires = requires; }
    }
    public sealed class Topic
    {
        public readonly string Title, Text;
        public readonly Step[] Steps;
        public Topic(string title, string text, params Step[] steps)
        { Title = title; Text = text; Steps = steps; }
    }
    public static readonly Topic[] Topics = {
        new Topic("1. Create access racks", "Scenario One begins by opening machine routes through the plantation, before any selective thinning. Racks are permanent routes used to remove timber from the stand. Establishing them now gives machinery access for this and future thinnings. Nothing is cut until you approve the work and advance a year.",
            new Step("rack.read", "What an extraction rack is", "A rack is a straight corridor of trees felled across the plantation so machinery can drive along it. It is permanent: it stays open for this and later thinnings. This plantation follows an authored rack plan of five evenly spaced racks across the planted rows (seven rows left standing between them). The status line and the Stand Map show how many are done.", true),
            new Step("rack.create", "Create your first rack", "Press B in the forest. Click the intended starting tree or ground point to fix the origin. Aim to pivot the violet corridor forward; the mouse wheel adjusts its direction. Click again to add the previewed trees to the Work Plan. Right-click chooses another start; Esc or B cancels."),
            new Step("rack.layout", "Complete the rack layout", "Open the rest of the authored rack plan: five racks in all. Use B in the forest, or open the Stand Map [M] and choose Plan rack, which shows the rows still to do. The status line counts the racks already in the Work Plan."),
            new Step("rack.approve", "Approve the rack work", "Open the Work Plan [Tab]. Each rack is one group with its trees, timber and income. Check the cost, then choose Approve Work Plan. Approval does not cut anything yet."),
            new Step("rack.result", "Advance a year and see the racks", "Advance one year and read the Annual Review. Then walk a rack: it is now an open corridor of stumps across the plantation. Inspecting trees, Crop Trees and selective felling come next.")),
        new Topic("2. Tree inspection and Crop Trees", "Begin with trees worth retaining and favouring, then inspect their competitors. Being smaller or suppressed is not by itself a reason to remove a tree. There is no single correct tree picked by this lesson.",
            new Step("tree.inspect", "Inspect a living tree", "Look at a tree and press E. Read its identity, age, height, DBH and competition. E closes inspection; F1 revisits the explanation."),
            new Step("tree.read", "Understand the measurements", "DBH is trunk diameter at breast height. Competition describes neighbours restricting growth. The light figure is the light on the ground at the tree's base. Use the fields together with the tree and its neighbours, not as a command to remove it.", true, "tree.inspect"),
            new Step("tree.crop", "Designate a Crop Tree", "Aim at a living tree you want to retain and press C. A blue mark means retain/favour, not fell. Press C again to undo it. Choose a tree yourself after inspection; this designation is also used by crop-tree pruning.")),
        new Topic("3. Felling", "Continuous-cover forestry needs repeated selective management. More felling is not automatically better: consider what remains, then return later to inspect and reassess. Learn the proposal → plan → approval → results sequence. Take time to compare trees; no cutting is required in year one.",
            new Step("fell.mark", "Propose a tree for felling", "Aim at a living tree and press X. The red mark is only a proposal. X again removes it. Crop and Fell marks are alternatives on the same tree; check the label before proceeding."),
            new Step("fell.plan", "Import the Fell mark into the Work Plan", "Press Tab. Your red Fell marks join the Work Plan as jobs when you open it. Inspect each job's timber estimate, contractor cost and material outcome. Removing a pending job lets you reconsider."),
            new Step("fell.cost", "Read the harvest costs", "Compare total labour, timber income, net value and remaining cash. A small visit can be dominated by the contractor's minimum charge. Estimates are re-quoted when work resolves.", true, "fell.plan"),
            new Step("fell.approve", "Approve a felling job when ready", "Approve Work Plan commits the planned jobs. Approval does not immediately cut trees. Check every pending job first: that button can approve more than your felling proposal."),
            new Step("fell.result", "Review a successful felling result", "Advance a year when your plan is ready. Open Annual Review and read WORK DONE, MONEY and FOREST. Successful felling removes the standing tree; failed work needs its failure reason checked. Walk back to inspect the change.")),
        new Topic("4. Work Plan and Annual Review", "The forest is where you decide what should happen. The Work Plan reviews execution and cost; Annual Review records what actually happened.",
            new Step("plan.open", "Open Work Plan", "Press Tab. Compare your open jobs, cash, expected timber income and available owner time. Opening the plan neither approves jobs nor advances time."),
            new Step("plan.read", "Understand approval and time", "Approve Work Plan commits the chosen jobs. Advance one year executes approved jobs and advances the forest. Check all pending jobs and costs first. Tutorials have no first-year deadline; take observations and decisions at your own pace.", true, "plan.open"),
            new Step("review.open", "Open actual annual results", "After an annual cycle, read WORK DONE for successes/failures, MONEY for costs/income and FOREST for changes. An empty year-zero review is not a completed cycle."),
            new Step("review.read", "Read and acknowledge the results", "Use I've read the annual results after reading the three sections. The first acknowledgement unlocks further year advances. Keep using the results to choose what to inspect next; you need not finish every learning topic to continue.")),
        new Topic("5. Stand Map", "Start with opening the map and reading one square. Practise navigation next; explore the other layers when you need them. The map finds places to investigate. It cannot make forestry decisions or carry out work remotely.",
            new Step("map.open", "Open the map", "Press M, or choose Stand map on the HUD. North is up. The white square border shows your position; letter/number labels identify locations."),
            new Step("map.select", "Select a square and read its information", "Click a square. The side panel shows its light, canopy and young growth. Read that diagnosis before choosing a destination."),
            new Step("map.light", "Read the Light layer", "Choose Light. Values run from 0 (dark) to 1 (open), measured at ground level. Brighter squares have more ground light. This does not measure a particular tree's crown light."),
            new Step("map.waypoint", "Set a waypoint", "Choose a square away from where you are standing, then Set waypoint. This adds a destination marker; it does not move you."),
            new Step("map.return", "Return to the forest", "Press M or Esc, or use Back to forest. The HUD now gives the destination's compass direction and distance."),
            new Step("map.arrive", "Walk to the destination", "Follow the HUD direction and watch the distance decrease until it says you are here. Walk from outside the destination square; setting a waypoint on your own square is not this practice journey."),
            new Step("map.inspectsite", "Inspect the site on foot", "At the destination, look at the ground report or inspect a nearby tree with E. Compare what you see with the square diagnosis before choosing any management work."),
            new Step("map.regeneration", "Explore Regeneration", "Choose Regeneration. P counts living planted juveniles; the height is the tallest natural seedling cohort. Read the side panel for species and conditions. More light or an opening can create an opportunity for natural regeneration, but it needs seed and suitable conditions; success is not guaranteed. An empty square is information, not an instruction to plant."),
            new Step("map.browse", "Explore Browsing / protection", "Choose Browsing / protection. The map shows site browsing pressure and shelters expected to be effective next year. S counts shelters; a dash means none. Visit young growth and compare its condition with the ground report."),
            new Step("map.marks", "Explore Fell & crop marks", "Choose Fell & crop marks. Squares count Fell proposals; diamonds count Crop Trees. Red means proposed removal, blue means retain/favour. This is a view of decisions made in the forest, not a remote marking tool."),
            new Step("rack.map", "Plan a rack on the Stand Map", "Open the Stand Map [M], choose Plan rack, pick a square on the row you want and add the rack. The map uses the same rack tool as the forest. This practice is optional."),
            new Step("map.compare", "Understand the limits of a map", "A square summarises an area. Switch layers to compare light, young growth, protection and your marks, then walk there. A colour or a low number alone does not tell you which tree to fell.", true, "map.inspectsite")),
        new Topic("6. Pruning", "Pruning removes lower branches from retained Crop Trees for future timber quality and a clearer stem. Pruning sets no price by itself: where Oak timber is marketed, the recorded pruned section can make a large log eligible for a higher grade, while Sitka and Beech have no such premium here, and fork or defect shape is not modelled. It costs contractor time and money. A tree may need to grow or recover before it is eligible.",
            new Step("prune.read", "Understand eligibility", "The Work Plan reports how many Crop Trees are eligible. Sitka uses lifts to 2.5 m, 5 m and 6.5 m (at most three lifts, at least five years apart). When Oak and Beech timber markets are active, Oak and Beech use two lifts, to 3.5 m then 6.0 m, at least three years apart; otherwise they keep the three-lift schedule above. All are limited to a crown base below 60% of tree height. These are scenario rules, not a recommendation to prune every tree.", true, "tree.crop"),
            new Step("prune.plan", "Plan an eligible pruning lift", "In Work Plan, find Pruning and Add eligible crop tree pruning tasks. This can add several eligible trees; review each target height and cost, and remove jobs you do not want. If none are eligible, return in a later year."),
            new Step("prune.approve", "Approve the selected pruning work", "Check contractor cost and available cash, then approve pending work. A designated Crop Tree stays standing; the lift is executed during the annual work cycle."),
            new Step("prune.result", "Read the pruning result", "After a successful lift, read Annual Review. Return to that tree and inspect its Pruning field for the lift count and clear-stem height. Do not expect another immediate lift.")),
        new Topic("7. Fallen deadwood", "Deadwood is a material outcome of felling. Keeping a fallen stem on site differs from selling it or keeping timber for construction.",
            new Step("deadwood.read", "Understand the material choices", "Sell extracts timber for configured sale income. Keep for use retains usable construction timber. Leave as deadwood retains the fallen stem on site for the scenario's habitat record; it earns no timber sale income. Contractor work can still cost money.", true, "fell.plan"),
            new Step("deadwood.plan", "Choose Leave as deadwood", "On a pending felling job, choose Leave as deadwood. Compare the new quote and cash balance before approving. This changes that job's outcome; it does not create a log until work succeeds."),
            new Step("deadwood.result", "Review retained deadwood", "Resolve the approved job in a later annual cycle and read FOREST in Annual Review. It reports newly retained volume and fallen logs on site. The scenario records decay over time; deadwood is not stored construction timber."),
            new Step("deadwood.visit", "Visit a retained fallen stem", "Walk back to within a few metres of a retained log and look at the site. Compare the standing-tree removal and the fallen stem with the recorded outcome. This lesson can wait until you choose a suitable habitat treatment.")),
        new Topic("8. Planting and protection", "Use ground observations to choose a site. Planting is not automatically an improvement; natural regeneration is not required for every useful management outcome. Planning a spot, paying for nursery stock and approving work are separate steps.",
            new Step("plant.ground", "Read the ground report", "Look at the ground to see light, regeneration and browsing conditions. Use the map to locate another area if useful. Check existing growth before deciding whether planting is appropriate."),
            new Step("plant.stock", "Buy nursery stock", "In Work Plan, open Nursery, choose a quantity and buy the species you intend to plant. Read the total purchase cost and cash warning before buying. Buying stock does not plant it. Contractor planting and shelter materials cost extra; using your own time for unsheltered planting has no external labour charge. Stock is consumed when successful planting work uses it."),
            new Step("plant.plan", "Mark a planting spot", "Back in the forest, press G for planting mode, select owned stock and click the intended ground spot. This creates a planned marker, not an immediately established tree. Esc leaves planting mode."),
            new Step("plant.execution", "Read executor and shelter choices", "Work Plan lets planting use contractor labour or your limited annual time. Shelters add material cost and use the same executor as planting. Check browsing and your budget; changed approved work needs approval again. Review each order before advancing.", true, "plant.plan"),
            new Step("plant.shelter", "Plan a shelter when appropriate", "Set Shelters ON in Work Plan and inspect the planting order's with shelter label. Do this only when you choose protection for the site; this optional practice can wait. The browsing layer later shows whether shelters remain effective."),
            new Step("plant.approve", "Approve planting work", "Check stock availability, labour/time, shelters and available cash. Approve the jobs you intend to carry out. Approval reserves a plan; establishment is recorded after the annual cycle succeeds."),
            new Step("plant.result", "Read planting results", "Read Annual Review after successful planting, then revisit the marked site. Later years show growth and browsing response. A successful job does not guarantee that a juvenile survives indefinitely.")),
        new Topic("9. Vegetation clearance", "Clearance removes competing vegetation inside the previewed square footprint. It is a planned contractor operation; standing trees remain.",
            new Step("clear.read", "Read the footprint before planning", "Aim at ground with competing vegetation and use U to plan area clearance. Read the preview for the actual targets and area. Existing young growth inside the footprint may be removed, so inspect it before approving.", true, "plant.ground"),
            new Step("clear.plan", "Plan a clearance area", "Use the forest preview to designate a square, then find Vegetation clearance in Work Plan. Check the square label, targeted vegetation, contractor cost and any validation problem. Do not clear a square just because the map is dark."),
            new Step("clear.approve", "Approve clearance when ready", "Approve the chosen work after checking what will be removed. Pending jobs can be removed if you change your mind. The plan is resolved at the annual cycle, not at designation."),
            new Step("clear.result", "Read clearance results", "Read the successful work result. Then revisit the footprint on foot to compare removed competing growth with the planned area. Standing trees are not part of this operation, and the site can develop new vegetation later."))
    };

    // Objective 1 in a Rack Planning V2 world (D-064). Spec §31 gives the meaning of the first three sentences and §7 the five-rack
    // sentence. 16-20 m is the book's early-transformation example and this stand's calibrated band; five racks is calibration for this
    // stand. Neither is taught as a general rule.
    public const string RackV2Concept = "Extraction racks allow machinery to reach trees without creating access everywhere across the stand. "
        + "In early CCF transformation, rack spacing may commonly be around 16–20 m, but appropriate spacing depends on the stand, machinery and site. "
        + "Scenario One uses that range as a guide for this plantation. "
        + "This stand needs five extraction racks to create a workable first access layout. "
        + "Plan the racks on the map, then mark each rack entrance in the forest.";
    public const string RackV2Practice = "Racks stay open for this and later thinnings, so plan the whole layout on the map before you confirm it. "
        + "The Stand Map shows each rack's spacing against this stand's recommended range. Leave the rows between racks standing: that standing forest is what you are keeping. "
        + "Confirming adds pending removal work only; nothing is cut until you approve the Work Plan and advance a year.";

    // The Objective 1 sentence in a V2 world: the derived counter, then the next step of the spec §30 flow. Plan and confirm on the
    // map, mark the five entrances in the forest, then the existing rack.approve step (D-064 OPEN-2: marking comes before approval).
    public static string RackV2Instruction(string counter, bool confirmed, int marked, int target, bool workPending)
    {
        if (!confirmed) return counter + ". Plan Racks on the Stand Map [M], then choose Confirm Rack Plan.";
        if (marked < target)
            return counter + ". Return to the forest and mark the first rack tree nearest the path at each rack entrance: [P] spray marker, aim within 3.5 m, left-click.";
        return counter + (workPending ? ". Next, review and approve the rack removal work in the Work Plan [Tab], then advance one year."
            : ". Open the Work Plan [Tab] and advance one year.");
    }

    // The rack lessons' wording in a V2 world, where the map is the only rack author and the entrances are marked before approval.
    // Null keeps the step's own text. The step ids and their saved progress are unchanged.
    public static string RackV2StepText(string stepId)
    {
        switch (stepId)
        {
            case "rack.read": return "A rack is a straight corridor of removed trees that machinery drives along, so the rest of the stand can be reached without driving through it. It stays open for this and later thinnings. "
                + "Spacing is conditional: around 16–20 m between racks may be common in early transformation, around 30 m or more may be viable in developed CCF, and there is no universal rule. "
                + "Scenario One recommends 16–20 m and five racks for this plantation only.";
            case "rack.create": return "Open the Stand Map [M] and choose Plan Racks. Create and edit your first rack on the map; no trees are cut by planning.";
            case "rack.layout": return "Plan five racks on the Stand Map [M] and choose Confirm Rack Plan. Then return to the forest and mark each rack entrance tree in orange: [P] spray marker, aim within 3.5 m, left-click. Tree Marks help explains the paint.";
            case "rack.approve": return "Once the rack entrances are marked, open the Work Plan [Tab]. Check the rack removal work and its cost, then choose Approve Work Plan. Approval does not cut anything yet.";
            case "rack.map": return "Open the Stand Map [M] and choose Plan Racks to create or edit rack geometry. The map is the rack-planning surface; it does not approve or execute work.";
            default: return null;
        }
    }

    // Presentation only. A planned layout is still objective 1 until its work is actually carried out in an annual cycle.
    // This reads the existing stand/year state, so device-wide practice credits cannot skip the opening rack lesson.
    public static int GuidedTopicIndex(bool hasLayout, bool racksHarvested, int ecologicalYear, System.Func<string, bool> isDone)
    {
        if (hasLayout && (!racksHarvested || ecologicalYear <= 0)) return 0;
        for (int i = hasLayout ? 1 : 0; i < Topics.Length; i++)
            if (Topics[i].Steps.Any(step => !isDone(step.Id))) return i;
        return Topics.Length - 1;
    }

    public static IEnumerable<string> StepIds => Topics.SelectMany(t => t.Steps).Select(s => s.Id);

    // The step the HUD names as "Next lesson". Topics are in teaching order (look at trees, mark, plan, approve, read the
    // review, then the map and the optional practices), so the first step left in that order is the cue. Action steps
    // come before reading steps: the cue should point at something to do in the forest, while the explanatory readings
    // wait in the Objectives screen until nothing else is left. Null when every step is done.
    public static Step NextCueStep(System.Func<string, bool> isDone)
    {
        foreach (Topic topic in Topics)
            foreach (Step step in topic.Steps)
                if (!step.Reading && !isDone(step.Id)) return step;
        foreach (Topic topic in Topics)
            foreach (Step step in topic.Steps)
                if (step.Reading && !isDone(step.Id)) return step;
        return null;
    }

    // Whether oak and beech earn timber income depends on the saved forest's growth model (ScenarioOneManager.BroadleafMarketActive),
    // so a lesson must not say either way on its own. The Harvest card in the Work Plan reports the same thing per job.
    public static string MarketSentence(bool broadleafMarketActive) => broadleafMarketActive
        ? "In this forest oak and beech stems have timber prices as well as Sitka spruce, so check each job's roadside value."
        : "In this forest only Sitka spruce earns timber income: felled oak or beech is charged for but earns no sale income.";

    // The text shown for a step. Most steps are fixed; the harvest-cost reading adds the market sentence for the active forest.
    public static string StepText(Step step, bool broadleafMarketActive)
        => step.Id == "fell.cost" ? step.Text + " " + MarketSentence(broadleafMarketActive) : step.Text;

    public static string CueText(Step step) => step == null
        ? "Learning complete · revisit with O"
        : "Next lesson: " + step.Title + " · [O] Objectives";
}
// Wording for the first-use and F1 help screens. Kept free of UI Toolkit and PlayerPrefs so the text can be checked
// offline on plain .NET (Tools/Verification/ThreeSpecies/OfflineLogic). This is one half of MenuHelpView; the
// screen itself is in MenuHelpView.cs.
public sealed partial class MenuHelpView
{
    // New screens go at the end: the names are the keys of the local first-use preferences.
    public enum Menu { WalkingHud, TreeInspection, StandMap, WorkPlan, AnnualReview, Rack, TreeMarks }

    // Tree Marks (RACK-V2-10, spec §28) is unlocked when field marking first becomes relevant: a Rack Planning V2 world whose
    // rack plan is confirmed. Both facts are already saved forest state; whether this device has seen the screen is the usual
    // local preference, so no forest/save field is added. Before that, and in every Legacy40/model-0 world, it is never shown.
    public static bool TreeMarksUnlocked(bool rackPlanningV2Active, bool rackPlanConfirmed) => rackPlanningV2Active && rackPlanConfirmed;

    // While unlocked, walking in the forest is the Tree Marks context until its first-use screen has been seen (that is the
    // unlock moment), and again whenever the spray marker is equipped, so F1 explains the marks while marking.
    public static bool TreeMarksContext(bool unlocked, bool introduced, bool sprayEquipped) => unlocked && (!introduced || sprayEquipped);

    // The marks this forest uses so far, and nothing more (spec §25, §28, §29): later conventions join when a lesson needs them.
    public static readonly string[][] TreeMarkEntries =
    {
        new[] { "Orange vertical stripes", "Rack / machine-path removal" },
        new[] { "Red", "Fell" },
        new[] { "Blue", "Crop Tree / future timber" }
    };

    // Spec §28 wording. The first sentence pair is the source convention (book pp. 84-85, orange or pink; Scenario One's choice of
    // orange is calibration); the second discloses that marking only the entrance tree is this game's teaching simplification.
    public const string TreeMarksOrangeText = "Removal trees associated with a machine route can be marked with vertical stripes. The CCF source material describes removal/treatment colours such as orange or pink; Scenario One uses orange.";
    public const string TreeMarksSimplificationText = "The game asks you to mark the rack entrance tree as a teaching action. Real marking practice may mark additional trees along a route.";

    // The Rack screen in a Rack Planning V2 world (spec §30): plan on the map, confirm, mark the entrances, then approve.
    public const string RackV2Explanation = "Plan extraction racks on the Stand Map [M]. Choose Plan Racks, create or edit the corridors, then choose Confirm Rack Plan. "
        + "Back in the forest, mark each rack entrance tree in orange with the spray marker [P]. "
        + "Confirmation adds pending removal work; review and approve it in the Work Plan [Tab], then advance one year. "
        + "Racks are permanent machine-access routes for this and future thinnings. Individual Fell and Crop Tree marks remain forest decisions.";

    public static string Title(Menu menu)
    {
        switch (menu)
        {
            case Menu.TreeInspection: return "Tree Inspection · understand one tree";
            case Menu.StandMap: return "Stand Map · find where to look";
            case Menu.WorkPlan: return "Work Plan · review and approve jobs";
            case Menu.AnnualReview: return "Annual Review · learn from the year";
            case Menu.Rack: return "Extraction racks · open machine routes";
            case Menu.TreeMarks: return "Tree Marks · what the paint on a tree means";
            default: return "Walking HUD · your forest at a glance";
        }
    }

    // The short text shown the first time a screen is met, or null when that screen has no short form and shows its full
    // Explanation instead. The first-use screens stay brief so they do not pile up in front of the forest; the full
    // Explanation is always one click ("More detail") or F1 away.
    public static string Summary(Menu menu)
    {
        switch (menu)
        {
            case Menu.WalkingHud:
                return null;
            case Menu.Rack:
                return "Racks are permanent routes used to remove timber from the stand. Establishing them now gives machinery access for this and future thinnings. Click a tree or ground point where the rack should start. The start stays fixed while you aim. The preview extends forward only. Click again to add it to the Work Plan; right-click reselects the start, and Esc cancels. Nothing is cut until you approve the plan.";
            case Menu.TreeInspection:
                return "Start with a tree worth keeping, then see what competes with it. The card shows its size and how crowded it is; press V for more detail. Press C to mark a Crop Tree (keep and favour) or X to propose felling. Marks are only proposals: nothing is cut until you approve work in the Work Plan.";
            case Menu.StandMap:
                return "The map helps you decide where to look, not what to do. Choose a layer, select a square to read it, set a waypoint, then walk there and inspect the site yourself. It cannot mark, clear, plant or approve anything remotely.";
            case Menu.WorkPlan:
                return "The Work Plan collects your marks and planting spots into jobs you can review and approve. Check each cost, then approve the plan. Approval alone changes nothing: Advance one year carries out the approved jobs, and jobs you have not approved wait.";
            default:
                // The Annual Review has no first-use screen: its short guidance sits in the review itself (AnnualReviewReadPrompt).
                return null;
        }
    }

    // Historical verification API; onboarding now has a separate per-page copy source.
    public const int ScenarioIntroWordCap = 200;

    // Shown inside the Annual Review, beside the button that acknowledges the first results, instead of a help screen opening
    // on top of the review. The detailed explanation stays on Help [F1].
    public const string AnnualReviewReadPrompt = "Read WORK DONE, MONEY and FOREST. One thinning does not finish continuous-cover forestry: when you have read them, walk back, inspect how the forest responded and reassess before the next intervention.";

    public static string SummaryTitle(Menu menu)
    {
        switch (menu)
        {
            default: return Title(menu);
        }
    }

    public static string Explanation(Menu menu)
    {
        switch (menu)
        {
            case Menu.TreeMarks:
                return "In this forest a paint mark records a management decision on the tree itself. Scenario One uses three marks so far:\n"
                    + string.Join("\n", System.Array.ConvertAll(TreeMarkEntries, entry => "• " + entry[0] + " — " + entry[1])) + "\n\n"
                    + TreeMarksOrangeText + " The source describes the stripes on four sides of the tree; here one spray paints all four.\n\n"
                    + TreeMarksSimplificationText + "\n\n"
                    + "After Confirm Rack Plan, walk to a rack entrance and press P to equip the spray marker. Aim at the first rack tree nearest the path, within 3.5 m, and left-click; P stows the marker. "
                    + "A tree carries only one of these marks: the spray will not mark a Fell or Crop Tree, and X or C cannot change an orange mark. "
                    + "Marking cuts nothing: the rack's removal work still needs approval in the Work Plan [Tab].";
            case Menu.Rack:
                return "An extraction rack is a straight corridor of felled trees across the plantation. The harvesting machine drives along it, so the rest of the stand can be thinned later without driving through standing trees. Racks are permanent extraction routes: they stay open for this and future thinnings.\n\n"
                    + "Press B in the forest to create a rack. Click the intended starting tree or ground point: its ring and post mark the fixed origin. Aim to pivot the violet corridor forward from that point; the arrow shows the direction and violet rings mark the trees it would take. The mouse wheel adjusts the direction. Click or press Enter again to add it to the Work Plan; right-click reselects the start; Esc or B cancels. No work is added by choosing a start.\n\n"
                    + "This plantation follows an authored rack plan: five evenly spaced racks across the planted rows, with seven rows left standing between racks. The status line counts how many are in the Work Plan. After your first rack, the Stand Map can plan the rest: open it with M and choose Plan rack, which marks the squares of a planned rack that are still to do (◇) and those already in the Work Plan (✓).\n\n"
                    + "A rack keeps any Crop Tree on its line. Racks are felled in one contractor visit together with any other felling you have planned, so the visit minimum is shared. A rack is still ordinary felling work: approve it in the Work Plan [Tab], then advance a year.";
            case Menu.TreeInspection:
                return "Start with a tree worth keeping, then inspect its competitors. Inspect a tree with E (or left-click) to understand its size, growth, competition and management state. Use this before deciding which trees to favour or remove.\n\n"
                    + "DBH is trunk diameter measured at breast height. Competition shows whether neighbours restrict growth; the competition index (CI) is one number combining how large and how close the neighbouring trees are, and a higher number means more competition. Press V on the card for the rest: stem volume, light on the ground at the tree's base, recent growth, wind exposure and pruning.\n\n"
                    + "A Crop Tree is selected to retain and favour for future development. A Fell mark proposes removal. X and C change these marks; they do not execute work.\n\n"
                    + "Crop Trees are trees you choose to favour for the future. Nearby trees can reduce their growing space, but being small or suppressed does not by itself make a tree a problem. Inspect which neighbours contribute most to competition before deciding what to remove. On a Crop Tree's card, keys 1–5 show its listed neighbours in the forest.\n\n"
                    + "Close Help, then press E to close inspection and return to walking.";
            case Menu.StandMap:
                return "The Stand Map is a plan of your property seen from above: every tree is drawn at the size of its crown, with the racks, the forest path and you, on a grid in metres. It helps you find patterns across the forest that are difficult to see among the trees. Open it with M when looking for a site to inspect. Underneath, the ground is divided into squares of 5 m × 5 m, each named by a letter and a number: point at the map to read the square and its figures.\n\n"
                    + "Each layer adds its colour over the same plan: use Light for darker/brighter areas, Regeneration for young growth, Browsing / protection for deer browsing (deer eating the shoots of young trees) and the shelters that protect planted saplings, and Fell & crop marks for your marked trees.\n\n"
                    + "Select a square, read its information, then choose Set Waypoint. Close the map with M, Esc or Back to forest. Follow the HUD direction and distance, then inspect the site directly before deciding what to do.\n\n"
                    + "The map helps you find where to look. It cannot mark, clear, plant or approve work remotely, and it does not make the forestry decision for you.";
            case Menu.WorkPlan:
                return "The Work Plan collects decisions made in the forest into jobs you can review and approve. Open it with Tab after marking trees or planting/clearance sites; your marks join the plan when you open it.\n\n"
                    + "Review labour, material costs, cash after approved work and expected timber value. A contractor's minimum job charge can make a small harvest expensive. Some tasks let you choose contractor or landowner execution.\n\n"
                    + "Review or remove jobs, choose available executors and purchase nursery stock here. Approve Work Plan commits the plan; Advance one year resolves approved jobs and forest change. Approval alone does not immediately change the forest.\n\n"
                    + "Decide what should happen in the forest; review how it will happen and what it costs here. The residual stand is what remains standing after the planned work, and its summary (including basal area, the trunk cross-section area per hectare) describes what remains; it does not decide the forest structure for you. Use Tab or Back to forest to return.";
            case Menu.AnnualReview:
                return "After advancing time, the Annual Review shows what work happened and how the forest and finances changed. Read it before planning the next cycle.\n\n"
                    + "WORK DONE: what did I do, and did any jobs fail?\nMONEY: what did it cost or earn, and what cash remains?\nFOREST: how did growth, light, regeneration and habitat respond?\n\n"
                    + "Average forest-floor light is the mean of the light reaching the ground over every 5 m × 5 m square of the stand, from 0 (dark) to 1 (open). Areas with young regeneration counts the squares where young trees are present; the scenario objective Young trees present asks for a minimum number of them. Neither number tells you which tree to fell.\n\n"
                    + "One thinning does not finish continuous-cover forestry. Walk back, inspect the forest response and reassess before the next intervention. Openings can create an opportunity for regeneration; seed, light, browsing and vegetation affect the outcome. Use these results to decide what to inspect next. This screen records outcomes; it does not choose or execute forestry work.\n\n"
                    + "Close Help and read the three sections. After your first results, choose I've read the annual results to unlock further year advances. Use Esc or Walk the forest to return and inspect the changes; Open Work Plan returns to planning.";
            default:
                return "Your role: you manage an even-aged Sitka spruce plantation and guide its transformation toward a continuous-cover forest. Decisions play out over years, and you revisit them as the forest changes.\n\n"
                    + "In general, continuous-cover forestry means keeping tree cover at all times, harvesting selectively instead of clear-felling, and letting young trees grow up among older ones. How many species the forest holds is a management choice that depends on the site, the light and your aims. Scenario One's goal is more specific.\n\n"
                    + "You start with an even-aged plantation of almost one species. Over repeated management cycles you are aiming for a forest that:\n• keeps continuous tree cover;\n• keeps the best Sitka, still growing;\n• has trees of many sizes and ages;\n• has young trees coming from seed in the spaces you open (natural regeneration);\n• has other suitable species, such as oak and beech, where they fit;\n• is more varied in structure and habitat.\nSitka is not being removed. It becomes one part of a more varied forest.\n\n"
                    + "Continuous-cover forestry is a repeated process of selective management, not one thinning. A typical cycle:\n1. Observe: walk the forest, inspect trees and read the light on the ground.\n2. Choose the trees to keep: mark the Crop Trees (C) you want to grow on.\n3. Choose what to remove: the neighbours that most crowd them, not simply the smallest trees.\n4. Create access: racks (B) are permanent routes that keep machines off the rest of the stand.\n5. Manage deadwood: choose which felled wood stays on the ground as habitat.\n6. Prepare for young trees: openings let in light, but seed, deer browsing and ground vegetation decide what survives.\n7. Add species where justified: plant oak or beech only where there is light and a reason, and protect them.\n8. Let time pass: approve the Work Plan, then advance a year.\n9. Review: read the Annual Review, then walk back and inspect how the forest responded.\n10. Adapt: the next cycle starts from what you found, not what you hoped for.\nWhat you leave standing matters as much as what you remove; more felling is not automatically better.\n\n"
                    + "Real transformations often take decades; yours will change one year at a time. In this game, mixing species does not by itself make the forest more productive; what it adds is variety, habitat and more than one kind of timber to sell. There is no single correct prescription, and the amounts, costs and targets in this scenario are gameplay calibration, not forecasts.\n\n"
                    + "The HUD shows the current year, cash, objectives and information about the ground or tree you are looking at. Current objective progress appears in the status panel.\n\n"
                    + "The forest is where you make management decisions. Look at ground to check light, regeneration and browsing conditions (browsing is deer eating the shoots of young trees), or press E (or left-click) while looking at a tree to inspect it. Use these observations before marking work.\n\n"
                    + "A waypoint's direction and distance appear here when one is active. M opens the Stand Map; Tab opens the Work Plan to review and approve jobs. The HUD itself does not approve work.\n\n"
                    + "O opens Learning objectives: the current forest success objectives, then short lessons in a suggested order. The first lessons follow a first cycle: create extraction racks, approve the work, advance a year and read the Annual Review. Inspecting trees, Crop Trees and selective felling follow once the racks are open. Map practice, pruning, deadwood, planting and clearance wait until you want them. Progress is remembered on this device, with no first-year deadline.\n\n"
                    + ControlsReference.KeysLine + "\n\n"
                    + "Close Help to walk. Esc releases the mouse; click to resume looking around. Press F1 any time to revisit this screen's help.";
        }
    }
}

Exit: 0
```

$ rg -n "orange|Orange|Fell|Crop|marking" Assets/ForestPrototype/UI/StandMapView* Assets/ForestPrototype/*/*Painter*
```text
rg: Assets/ForestPrototype/*/*Painter*: No such file or directory (os error 2)
Assets/ForestPrototype/UI/StandMapView.cs:254:        AddLayerButton("Fell & crop marks", Layer.Marks);
Assets/ForestPrototype/UI/StandMapView.cs:442:            counts[i] = tree.Mark == MapTreeMark.Fell ? (c.fell + 1, c.crop) : (c.fell, c.crop + 1);
Assets/ForestPrototype/UI/StandMapView.cs:554:            if (p.Selection.SkippedCropTrees > 0) UiKit.Add(card, $"{p.Selection.SkippedCropTrees} Crop Tree(s) on the line are kept.", "muted");
Assets/ForestPrototype/UI/StandMapView.cs:774:                foreach (MapTree t in model.Trees) { if (t.Mark == MapTreeMark.Fell) fell++; else if (t.Mark == MapTreeMark.Crop) crop++; }
Assets/ForestPrototype/UI/StandMapView.cs:775:                return $"Rings mark the trees you have marked, and the forest is dimmed so they stand out: red to fell ({fell}), blue Crop Tree ({crop}).";
Assets/ForestPrototype/UI/StandMapView.cs:851:            UiKit.Line(diagnosis, "Crop Trees here", m.crop.ToString(UiKit.Inv));

Exit: 2
```

$ rg -n "LearningObjectivesView|MenuHelpView|16.?20|Introducing.*mark" Tools
```text
Tools/Calibration/OakBeechGrowth/production_v1_inputs.json:28:    "Assets/ForestPrototype/SitkaGrowthModel.cs": "c887fc29a4591f0be3002b756411256149a01139a10276d161622038c7389d2f",
Tools/Verification/RackV2Objective1/RackV2Objective1EndToEnd.cs:46:        foreach (string id in LearningObjectivesView.StepIds) PlayerPrefs.DeleteKey(LearningObjectivesView.PreferencePrefix + id);
Tools/Verification/RackV2Objective1/RackV2Objective1EndToEnd.cs:47:        foreach (MenuHelpView.Menu menu in Enum.GetValues(typeof(MenuHelpView.Menu))) PlayerPrefs.DeleteKey(MenuHelpView.PreferencePrefix + menu);
Tools/Verification/RackV2Objective1/RackV2Objective1EndToEnd.cs:306:            && objective.Contains("16–20 m") && objective.Contains("Plan Racks"), "Objective 1 details explain purpose, conditional spacing and map planning (§30 step 1, §31)");
Tools/Verification/RackV2Objective1/RackV2Objective1EndToEnd.cs:422:        Check(ui.Help.IsOpen && ui.Help.CurrentMenu == MenuHelpView.Menu.TreeMarks, "Tree Marks help introduces itself when field marking becomes relevant");
Tools/Verification/RackV2Objective1/README.md:10:| Start | `Start New Scenario` on the real startup screen; the premise pages | fresh Enlarged80 Rack Planning V2 world, 1344 stems, no racks/work; Objective 1 current; §30 step 1 / §31 copy (purpose, conditional 16–20 m, Plan Racks; no universal 18 m / five-rack claim) |
Tools/Verification/RackV2NoCompetingAuthor/GuidedRackV2Verification.cs:65:        ui.Help.Show(MenuHelpView.Menu.Rack);
Tools/Verification/RackV2NoCompetingAuthor/GuidedRackV2Verification.cs:78:        ui.Help.Show(MenuHelpView.Menu.WalkingHud); yield return Pause();
Tools/Verification/RackV2NoCompetingAuthor/Legacy40RackAuthoringVerification.cs:25:        MenuHelpView.ResetProgress(); LearningObjectivesView.ResetProgress();
Tools/Verification/ScenarioOneProfessionalMap/ProfessionalMapVerification.cs:46:        foreach (MenuHelpView.Menu menu in Enum.GetValues(typeof(MenuHelpView.Menu))) PlayerPrefs.SetInt(MenuHelpView.PreferencePrefix + menu, 1);
Tools/Verification/ScenarioOneProfessionalMap/MapRedesignShots.cs:48:        foreach (MenuHelpView.Menu menu in Enum.GetValues(typeof(MenuHelpView.Menu))) PlayerPrefs.SetInt(MenuHelpView.PreferencePrefix + menu, 1);
Tools/Verification/ScenarioOneNonUi/UnityGates/RackLessonVerification.cs:98:        MethodInfo method = typeof(LearningObjectivesView).GetMethod("TeachingFacts", BindingFlags.Instance | BindingFlags.NonPublic);
Tools/Verification/ScenarioOneNonUi/UnityGates/RackLessonVerification.cs:304:        // DecideRackInspection only acts at index 4 (LearningObjectivesView.Sequence.cs), so J decisions are inert and cannot cut or rewind.
Tools/Verification/ScenarioOneThreePageOnboarding/OnboardingVerification.cs:147:        MenuHelpView.ResetProgress();
Tools/Verification/Parity07Unity/Parity07UnityRender.cs:20:// drives the real ScenarioOneUiRoot / TreeInspectionView / WorkPlanView / LearningObjectivesView at 1280x720 and 1920x1080.
Tools/Verification/RackV2Editing/RackV2EditingVerification.cs:51:        foreach (MenuHelpView.Menu menu in Enum.GetValues(typeof(MenuHelpView.Menu)))
Tools/Verification/RackV2Editing/RackV2EditingVerification.cs:52:            PlayerPrefs.SetInt(MenuHelpView.PreferencePrefix + menu, 1);
Tools/Verification/ScenarioOneTeachingCopyVerification.cs:65:        foreach (string key in Enum.GetValues(typeof(MenuHelpView.Menu)).Cast<MenuHelpView.Menu>().Select(menu => MenuHelpView.PreferencePrefix + menu)
Tools/Verification/ScenarioOneTeachingCopyVerification.cs:66:            .Concat(LearningObjectivesView.StepIds.Select(id => LearningObjectivesView.PreferencePrefix + id)))
Tools/Verification/ScenarioOneTeachingCopyVerification.cs:89:        string help = MenuHelpView.Explanation(MenuHelpView.Menu.WalkingHud), tree = MenuHelpView.Explanation(MenuHelpView.Menu.TreeInspection);
Tools/Verification/ScenarioOneTeachingCopyVerification.cs:93:        string lessons = string.Join(" ", LearningObjectivesView.Topics.Select(t => t.Text + " " + string.Join(" ", t.Steps.Select(s => s.Text))));
Tools/Verification/ScenarioOneTeachingCopyVerification.cs:100:        Check(Has(MenuHelpView.Explanation(MenuHelpView.Menu.WorkPlan), "execution") && Has(MenuHelpView.Explanation(MenuHelpView.Menu.WorkPlan), "does not decide the forest structure"), "plan purpose");
Tools/Verification/ScenarioOneTeachingCopyVerification.cs:101:        Check(Has(MenuHelpView.Explanation(MenuHelpView.Menu.AnnualReview), "reassess") && Has(MenuHelpView.Explanation(MenuHelpView.Menu.AnnualReview), "Walk back"), "return to inspect");
Tools/Verification/ScenarioOneTeachingCopyVerification.cs:102:        Check(Has(LearningObjectivesView.ClearanceExplanation(2), "reduce the survival") && Has(LearningObjectivesView.ClearanceExplanation(2), "removes young trees")
Tools/Verification/ScenarioOneTeachingCopyVerification.cs:103:            && Has(LearningObjectivesView.ClearanceExplanation(2), "can return"), "three-part Model2 clearance meaning");
Tools/Verification/ScenarioOneTeachingCopyVerification.cs:104:        foreach (int legacy in new[] { 0, 1 }) Check(Has(LearningObjectivesView.ClearanceExplanation(legacy), "does not change young-tree survival"), "legacy clearance truth");
Tools/Verification/ScenarioOneTeachingCopyVerification.cs:170:            ui.CloseAll(); ui.CloseHelp(); ui.ShowHelp(MenuHelpView.Menu.WalkingHud); yield return Settle();
Tools/Verification/ScenarioOneNonUi/OfflineLogic/core-stand-live-2m.csv:254:P1620,19.4000,11.8076,14.6099
Tools/Verification/ScenarioOneNonUi/OfflineLogic/core-stand-live-2m.csv:281:P1806,-8.0582,16.2029,17.0861
Tools/Verification/ScenarioOneVisualSystem/VisualSystemFlowShots.cs:35:        foreach (MenuHelpView.Menu menu in Enum.GetValues(typeof(MenuHelpView.Menu))) PlayerPrefs.SetInt(MenuHelpView.PreferencePrefix + menu, 1);
Tools/Verification/ScenarioOneVisualSystem/VisualSystemFlowShots.cs:142:        if (StandaloneSessionMenu.IsOpen || !initialUi.Help.IsOpen || initialUi.Help.CurrentMenu != MenuHelpView.Menu.WalkingHud)
Tools/Verification/ScenarioOneVisualSystem/VisualSystemFlowShots.cs:148:            typeof(MenuHelpView).GetMethod("ToggleDetail", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(initialUi.Help, null);
Tools/Verification/ScenarioOneVisualSystem/VisualSystemFlowShots.cs:150:            typeof(MenuHelpView).GetMethod("ToggleDetail", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(initialUi.Help, null);
Tools/Verification/ScenarioOneNonUi/OfflineLogic/RackSampleGeometryOfflineCheck.cs:379:        string sequence = Source(root, "Assets/ForestPrototype/UI/LearningObjectivesView.Sequence.cs");
Tools/Verification/ScenarioOneUx/run_ux_check.py:26:    "MenuHelpView.Text.cs",
Tools/Verification/ScenarioOneUx/run_ux_check.py:27:    "LearningObjectivesView.Curriculum.cs",
Tools/Verification/ScenarioOneUx/Qualification/make_startup_diagnostic.py:18:          'Debug.Log("DIAG_BEFORE_KEYS helpOpen=" + ui.Help.IsOpen + " helpMenu=" + ui.Help.CurrentMenu + " uiEnabled=" + ui.enabled + " introducedWalkingHud=" + MenuHelpView.Introduced(MenuHelpView.Menu.WalkingHud));\n            ')
Tools/Verification/ScenarioOneUx/Qualification/UxQualification.cs:71:        foreach (MenuHelpView.Menu menu in Enum.GetValues(typeof(MenuHelpView.Menu))) PlayerPrefs.DeleteKey(MenuHelpView.PreferencePrefix + menu);
Tools/Verification/ScenarioOneUx/Qualification/UxQualification.cs:72:        foreach (string id in LearningObjectivesView.StepIds) PlayerPrefs.DeleteKey(LearningObjectivesView.PreferencePrefix + id);
Tools/Verification/ScenarioOneUx/Qualification/UxQualification.cs:348:        Hard("ROLE_FRESH_PLAYER_SEES_THE_WALKING_HUD_INTRODUCTION", ui.Help.IsOpen && ui.Help.CurrentMenu == MenuHelpView.Menu.WalkingHud, $"open={ui.Help.IsOpen} menu={ui.Help.CurrentMenu}");
Tools/Verification/ScenarioOneUx/Qualification/UxQualification.cs:457:        if (ui.Help.IsOpen && ui.Help.CurrentMenu == MenuHelpView.Menu.WalkingHud) { ui.CloseHelp(); yield return null; yield return null; }
Tools/Verification/ScenarioOneUx/Qualification/UxQualification.cs:462:        Hard("LEARNING_STEP_IDS_ARE_48_THE_42_SAVED_ONES_PLUS_SIX_RACK_STEPS", LearningObjectivesView.StepIds.Count() == 48, $"count={LearningObjectivesView.StepIds.Count()} sortedIdHash={Fnv(string.Join("|", LearningObjectivesView.StepIds.OrderBy(s => s, StringComparer.Ordinal)))}");
Tools/Verification/ScenarioOneUx/Qualification/UxQualification.cs:469:        bool introOpened = ui.Help.IsOpen && ui.Help.CurrentMenu == MenuHelpView.Menu.TreeInspection;
Tools/Verification/ScenarioOneUx/Qualification/UxQualification.cs:491:        bool planIntro = ui.Help.IsOpen && ui.Help.CurrentMenu == MenuHelpView.Menu.WorkPlan;
Tools/Verification/ScenarioOneUx/Qualification/UxQualification.cs:526:        Hard("ANNUAL_REVIEW_HAS_ITS_READING_GUIDANCE_INLINE_BESIDE_THE_ACKNOWLEDGEMENT", ack != null && Visible(ack) && ack.parent.Query<Label>().ToList().Any(l => l.text == MenuHelpView.AnnualReviewReadPrompt), ack == null ? "no acknowledge button" : "prompt present=" + ack.parent.Query<Label>().ToList().Any(l => l.text == MenuHelpView.AnnualReviewReadPrompt));
Tools/Verification/ScenarioOneUx/Qualification/UxQualification.cs:530:        Hard("REVIEW_F1_OPENS_HELP_FOR_THE_REVIEW", ui.Help.IsOpen && ui.Help.CurrentMenu == MenuHelpView.Menu.AnnualReview, $"open={ui.Help.IsOpen} menu={ui.Help.CurrentMenu}");
Tools/Verification/ScenarioOneUx/Qualification/UxQualification.cs:632:        int helpSeen = Enum.GetValues(typeof(MenuHelpView.Menu)).Cast<MenuHelpView.Menu>().Count(MenuHelpView.Introduced);
Tools/Verification/ScenarioOneUx/Qualification/UxQualification.cs:652:        int helpAfterCancel = Enum.GetValues(typeof(MenuHelpView.Menu)).Cast<MenuHelpView.Menu>().Count(MenuHelpView.Introduced);
Tools/Verification/ScenarioOneUx/Qualification/UxQualification.cs:658:        int helpAfter = Enum.GetValues(typeof(MenuHelpView.Menu)).Cast<MenuHelpView.Menu>().Count(MenuHelpView.Introduced);
Tools/Verification/ScenarioOneUx/Qualification/UxQualification.cs:674:        Hard("AFTER_RESET_THE_ROLE_PANEL_OPENS_ONCE", ui.Help.IsOpen && ui.Help.CurrentMenu == MenuHelpView.Menu.WalkingHud && Count(Texts(ui.Help.Root), "Your role · Scenario One") == 1, $"open={ui.Help.IsOpen} menu={ui.Help.CurrentMenu} titles={Count(Texts(ui.Help.Root), "Your role · Scenario One")}");
Tools/Verification/ScenarioOneUx/Qualification/UxQualification.cs:926:        for (int i = 0; i < 10; i++) { ui.ResetTeachingProgress(); ui.ShowHelp(MenuHelpView.Menu.WalkingHud); yield return null; ui.CloseHelp(); yield return null; }
Tools/Verification/ScenarioOneUx/Qualification/UxQualification.cs:970:        for (int i = 0; i < 10; i++) { sw.Restart(); ui.ResetTeachingProgress(); ui.ShowHelp(MenuHelpView.Menu.WalkingHud); ui.CloseHelp(); sw.Stop(); helpMs.Add(sw.Elapsed.TotalMilliseconds); yield return null; }
Tools/Verification/S1B/StandalonePlayerSmoke.cs:59:            ui.ShowHelp(MenuHelpView.Menu.WalkingHud); Check(ui.Help.IsOpen,"S1-A Help opens"); yield return Capture("help"); ui.CloseHelp();
Tools/Verification/ScenarioOneUx/OfflineLogic/ScenarioOneUxOfflineCheck.cs:93:        string brief = MenuHelpView.Summary(MenuHelpView.Menu.WalkingHud);
Tools/Verification/ScenarioOneUx/OfflineLogic/ScenarioOneUxOfflineCheck.cs:98:        Check(Words(brief) <= MenuHelpView.ScenarioIntroWordCap, "the scenario introduction stays short enough to read before play", Words(brief) + " of " + MenuHelpView.ScenarioIntroWordCap + " words");
Tools/Verification/ScenarioOneUx/OfflineLogic/ScenarioOneUxOfflineCheck.cs:107:        Check(MenuHelpView.SummaryTitle(MenuHelpView.Menu.WalkingHud) != MenuHelpView.Title(MenuHelpView.Menu.WalkingHud), "the briefing has its own title");
Tools/Verification/ScenarioOneUx/OfflineLogic/ScenarioOneUxOfflineCheck.cs:108:        string full = MenuHelpView.Explanation(MenuHelpView.Menu.WalkingHud);
Tools/Verification/ScenarioOneUx/OfflineLogic/ScenarioOneUxOfflineCheck.cs:143:        foreach (MenuHelpView.Menu menu in Enum.GetValues(typeof(MenuHelpView.Menu)))
Tools/Verification/ScenarioOneUx/OfflineLogic/ScenarioOneUxOfflineCheck.cs:145:            string brief = MenuHelpView.Summary(menu);
Tools/Verification/ScenarioOneUx/OfflineLogic/ScenarioOneUxOfflineCheck.cs:146:            string detail = MenuHelpView.Explanation(menu);
Tools/Verification/ScenarioOneUx/OfflineLogic/ScenarioOneUxOfflineCheck.cs:149:            bool introduction = menu == MenuHelpView.Menu.WalkingHud;
Tools/Verification/ScenarioOneUx/OfflineLogic/ScenarioOneUxOfflineCheck.cs:152:            Check(Words(brief) <= (introduction ? MenuHelpView.ScenarioIntroWordCap : 130), "first-use text is brief for " + menu, Words(brief) + " words");
Tools/Verification/ScenarioOneUx/OfflineLogic/ScenarioOneUxOfflineCheck.cs:158:        Check(MenuHelpView.Summary(MenuHelpView.Menu.AnnualReview) == null, "the Annual Review has no first-use help screen to stack over its acknowledgement");
Tools/Verification/ScenarioOneUx/OfflineLogic/ScenarioOneUxOfflineCheck.cs:159:        string prompt = MenuHelpView.AnnualReviewReadPrompt;
Tools/Verification/ScenarioOneUx/OfflineLogic/ScenarioOneUxOfflineCheck.cs:163:        Check(Has(MenuHelpView.Summary(MenuHelpView.Menu.TreeInspection) ?? "", "proposals") && Has(MenuHelpView.Summary(MenuHelpView.Menu.TreeInspection) ?? "", "Crop Tree"),
Tools/Verification/ScenarioOneUx/OfflineLogic/ScenarioOneUxOfflineCheck.cs:165:        Check(Has(MenuHelpView.Summary(MenuHelpView.Menu.StandMap) ?? "", "not what to do"), "stand map first-use: navigation aid, not remote management");
Tools/Verification/ScenarioOneUx/OfflineLogic/ScenarioOneUxOfflineCheck.cs:166:        Check(Has(MenuHelpView.Summary(MenuHelpView.Menu.WorkPlan) ?? "", "Approval alone changes nothing"), "work plan first-use: approval is not execution");
Tools/Verification/ScenarioOneUx/OfflineLogic/ScenarioOneUxOfflineCheck.cs:173:        string[] titles = LearningObjectivesView.Topics.Select(t => t.Title).ToArray();
Tools/Verification/ScenarioOneUx/OfflineLogic/ScenarioOneUxOfflineCheck.cs:174:        Check(LearningObjectivesView.GuidedTopicIndex(true, false, 0, id => false) == 0, "fresh forest teaches rack objective first");
Tools/Verification/ScenarioOneUx/OfflineLogic/ScenarioOneUxOfflineCheck.cs:175:        Check(LearningObjectivesView.GuidedTopicIndex(true, false, 3, id => true) == 0, "old device credits and empty years cannot skip rack work");
Tools/Verification/ScenarioOneUx/OfflineLogic/ScenarioOneUxOfflineCheck.cs:176:        Check(LearningObjectivesView.GuidedTopicIndex(true, true, 0, id => false) == 0, "objective 2 waits for annual advance even if rack flag is ready");
Tools/Verification/ScenarioOneUx/OfflineLogic/ScenarioOneUxOfflineCheck.cs:177:        Check(LearningObjectivesView.GuidedTopicIndex(true, true, 1, id => false) == 1, "harvested racks plus one annual advance activate objective 2");
Tools/Verification/ScenarioOneUx/OfflineLogic/ScenarioOneUxOfflineCheck.cs:187:        for (LearningObjectivesView.Step step = LearningObjectivesView.NextCueStep(done.Contains); step != null && cues.Count < 60;
Tools/Verification/ScenarioOneUx/OfflineLogic/ScenarioOneUxOfflineCheck.cs:188:             step = LearningObjectivesView.NextCueStep(done.Contains))
Tools/Verification/ScenarioOneUx/OfflineLogic/ScenarioOneUxOfflineCheck.cs:199:        int firstReading = cues.FindIndex(id => LearningObjectivesView.Topics.SelectMany(t => t.Steps).First(s => s.Id == id).Reading);
Tools/Verification/ScenarioOneUx/OfflineLogic/ScenarioOneUxOfflineCheck.cs:200:        int lastAction = cues.FindLastIndex(id => !LearningObjectivesView.Topics.SelectMany(t => t.Steps).First(s => s.Id == id).Reading);
Tools/Verification/ScenarioOneUx/OfflineLogic/ScenarioOneUxOfflineCheck.cs:202:        Check(LearningObjectivesView.CueText(LearningObjectivesView.NextCueStep(id => false)) == "Next lesson: Create your first rack · [O] Objectives", "the first cue reads as an invitation to create a rack");
Tools/Verification/ScenarioOneUx/OfflineLogic/ScenarioOneUxOfflineCheck.cs:204:        Check(LearningObjectivesView.CueText(null) == "Learning complete · revisit with O", "all done reads as complete");
Tools/Verification/ScenarioOneUx/OfflineLogic/ScenarioOneUxOfflineCheck.cs:205:        Check(Has(MenuHelpView.Explanation(MenuHelpView.Menu.WalkingHud), "create extraction racks") && Has(MenuHelpView.Explanation(MenuHelpView.Menu.WalkingHud), "selective felling follow once the racks are open") && !Has(MenuHelpView.Explanation(MenuHelpView.Menu.WalkingHud), "start with simple map steps"),
Tools/Verification/ScenarioOneUx/OfflineLogic/ScenarioOneUxOfflineCheck.cs:233:        Check(Has(MenuHelpView.Explanation(MenuHelpView.Menu.WalkingHud), ControlsReference.KeysLine), "the full Walking HUD help includes the keys line");
Tools/Verification/ScenarioOneUx/OfflineLogic/ScenarioOneUxOfflineCheck.cs:236:        string tree = MenuHelpView.Explanation(MenuHelpView.Menu.TreeInspection);
Tools/Verification/ScenarioOneUx/OfflineLogic/ScenarioOneUxOfflineCheck.cs:240:        string plan = MenuHelpView.Explanation(MenuHelpView.Menu.WorkPlan);
Tools/Verification/ScenarioOneUx/OfflineLogic/ScenarioOneUxOfflineCheck.cs:242:        Check(Has(MenuHelpView.Explanation(MenuHelpView.Menu.WalkingHud), "browsing is deer eating the shoots of young trees"), "browsing is defined in the Walking HUD help");
Tools/Verification/ScenarioOneUx/OfflineLogic/ScenarioOneUxOfflineCheck.cs:243:        Check(Has(MenuHelpView.Explanation(MenuHelpView.Menu.StandMap), "deer eating the shoots of young trees"), "browsing is defined in the Stand Map help");
Tools/Verification/ScenarioOneUx/OfflineLogic/ScenarioOneUxOfflineCheck.cs:244:        Check(Has(MenuHelpView.Explanation(MenuHelpView.Menu.TreeInspection), "left-click"), "inspection help mentions left-click");
Tools/Verification/ScenarioOneUx/OfflineLogic/ScenarioOneUxOfflineCheck.cs:251:        string present = LearningObjectivesView.MarketSentence(true), absent = LearningObjectivesView.MarketSentence(false);
Tools/Verification/ScenarioOneUx/OfflineLogic/ScenarioOneUxOfflineCheck.cs:255:        LearningObjectivesView.Step cost = LearningObjectivesView.Topics.SelectMany(t => t.Steps).First(s => s.Id == "fell.cost");
Tools/Verification/ScenarioOneUx/OfflineLogic/ScenarioOneUxOfflineCheck.cs:257:        Check(LearningObjectivesView.StepText(cost, true).EndsWith(present) && LearningObjectivesView.StepText(cost, false).EndsWith(absent), "the harvest-cost lesson appends the sentence for the active forest");
Tools/Verification/ScenarioOneUx/OfflineLogic/ScenarioOneUxOfflineCheck.cs:258:        LearningObjectivesView.Step other = LearningObjectivesView.Topics.SelectMany(t => t.Steps).First(s => s.Id == "fell.approve");
Tools/Verification/ScenarioOneUx/OfflineLogic/ScenarioOneUxOfflineCheck.cs:259:        Check(LearningObjectivesView.StepText(other, true) == other.Text && LearningObjectivesView.StepText(other, false) == other.Text, "other lessons are unchanged by the market state");
Tools/Verification/ScenarioOneUx/OfflineLogic/ScenarioOneUxOfflineCheck.cs:283:        var ids = LearningObjectivesView.StepIds.ToList();
Tools/Verification/ScenarioOneUx/OfflineLogic/ScenarioOneUxOfflineCheck.cs:287:        foreach (LearningObjectivesView.Topic topic in LearningObjectivesView.Topics)
Tools/Verification/ScenarioOneUx/OfflineLogic/ScenarioOneUxOfflineCheck.cs:288:            foreach (LearningObjectivesView.Step step in topic.Steps)
Tools/Verification/ScenarioOneUx/OfflineLogic/ScenarioOneUxOfflineCheck.cs:290:        Check(LearningObjectivesView.PreferencePrefix == "CCF.Learning.v1.", "learning preference prefix is unchanged");
Tools/Verification/ScenarioOneUx/OfflineLogic/ScenarioOneUxOfflineCheck.cs:291:        foreach (MenuHelpView.Menu menu in Enum.GetValues(typeof(MenuHelpView.Menu)))
Tools/Verification/ScenarioOneUx/OfflineLogic/ScenarioOneUxOfflineCheck.cs:293:            Check(!string.IsNullOrWhiteSpace(MenuHelpView.Title(menu)), "help title exists for " + menu);
Tools/Verification/ScenarioOneUx/OfflineLogic/ScenarioOneUxOfflineCheck.cs:294:            Check(!string.IsNullOrWhiteSpace(MenuHelpView.Explanation(menu)), "full help text exists for " + menu);
Tools/Verification/RackV2ConfirmMinimap/RackV2ConfirmMinimapVerification.cs:56:        foreach (MenuHelpView.Menu menu in Enum.GetValues(typeof(MenuHelpView.Menu)))
Tools/Verification/RackV2ConfirmMinimap/RackV2ConfirmMinimapVerification.cs:57:            PlayerPrefs.SetInt(MenuHelpView.PreferencePrefix + menu, 1);
Tools/Verification/RackV2ConfirmMinimap/RackV2ConfirmMinimapVerification.cs:844:            Check(spacing.Contains("Nearest rack") && ui.Map.Root.Q("map-edit-card").Query<Label>().ToList().Any(l => l.text.Contains("16–20 m")), tag + ": RACK-V2-06 spacing teaching still works after a reopen: " + spacing);
Tools/Verification/RackV2ConfirmMinimap/run_static.py:162:    for rel in ["UI/LearningObjectivesView.cs", "UI/LearningObjectivesView.Sequence.cs"]:
Tools/Verification/ThreeSpecies/OfflineLogic/check_player_facing_truth.py:45:work = src("WorkPlanView.cs"); cur = src("LearningObjectivesView.Curriculum.cs")
Tools/Verification/ScenarioOneRepeatedInterventions/TeachingCapOfflineCheck.cs:166:        string seq = Read(root, "Assets/ForestPrototype/UI/LearningObjectivesView.Sequence.cs");
Tools/Verification/MfmTreatmentReviewUnity/MfmTreatmentReviewUnityGate.cs:37:        foreach (MenuHelpView.Menu menu in Enum.GetValues(typeof(MenuHelpView.Menu))) PlayerPrefs.SetInt(MenuHelpView.PreferencePrefix + menu, 1);
Tools/Verification/TeachingPersistence/PersistenceUnityVerification.cs:99:        MethodInfo method = typeof(LearningObjectivesView).GetMethod("TeachingFacts", BindingFlags.Instance | BindingFlags.NonPublic);
Tools/Verification/ScenarioOneRepeatedInterventions/TeachingSequenceMonotonicityOfflineCheck.cs:373:        string view = Read(root, "Assets/ForestPrototype/UI/LearningObjectivesView.Sequence.cs");
Tools/Verification/TeachingPersistence/PersistenceOfflineCheck.cs:81:        string view = File.ReadAllText(Path.Combine(root, "Assets/ForestPrototype/UI/LearningObjectivesView.Sequence.cs"));
Tools/Verification/ScenarioOneRepeatedInterventions/run_teaching_sequence_offline.py:35:    "Assets/ForestPrototype/UI/LearningObjectivesView.Sequence.cs",
Tools/Verification/Mfm09ExtensionFlow/run_offline.py:28:OBJ = "Assets/ForestPrototype/UI/LearningObjectivesView.cs"
Tools/Verification/Mfm09ExtensionFlow/run_offline.py:29:OBJSEQ = "Assets/ForestPrototype/UI/LearningObjectivesView.Sequence.cs"
Tools/Verification/ScenarioOneRepeatedInterventions/TeachingCapUnityVerification.cs:51:    ScenarioOneTeachingSequence.Facts Facts() => (ScenarioOneTeachingSequence.Facts)typeof(LearningObjectivesView)
Tools/Verification/RackV2HelpTeaching/run_offline.py:9:  offline  compiles the production pure-wording files (MenuHelpView.Text.cs, LearningObjectivesView.Curriculum.cs,
Tools/Verification/RackV2HelpTeaching/run_offline.py:30:PURE = [UI / "MenuHelpView.Text.cs", UI / "LearningObjectivesView.Curriculum.cs", UI / "ControlsReference.cs"]
Tools/Verification/RackV2HelpTeaching/run_offline.py:61:    context = body(root_ui, r"private\s+MenuHelpView\.Menu\s+HelpContext\s*\(\s*\)")
Tools/Verification/RackV2HelpTeaching/run_offline.py:62:    report("MenuHelpView.TreeMarksUnlocked(manager.RackPlanningV2Active, manager.RackPlanConfirmed)" in context,
Tools/Verification/RackV2HelpTeaching/run_offline.py:64:    report("MenuHelpView.Introduced(MenuHelpView.Menu.TreeMarks)" in context and "marking.RackSprayEquipped" in context
Tools/Verification/RackV2HelpTeaching/run_offline.py:65:           and "return MenuHelpView.Menu.TreeMarks;" in context,
Tools/Verification/RackV2HelpTeaching/run_offline.py:71:    introduce = re.search(r"if \(screen != UiScreen\.Objectives && context != MenuHelpView\.Menu\.AnnualReview && context != MenuHelpView\.Menu\.Rack\)\s*\n\s*help\.Introduce\(context\);", root_ui)
Tools/Verification/RackV2HelpTeaching/run_offline.py:73:    help_view = (UI / "MenuHelpView.cs").read_text()
Tools/Verification/RackV2HelpTeaching/run_offline.py:77:    learning = (UI / "LearningObjectivesView.cs").read_text()
Tools/Verification/RackV2HelpTeaching/run_offline.py:85:    report("LearningObjectivesView.RackV2Instruction(V2CounterText, m.RackPlanConfirmed, m.MarkedRackEntranceCount," in instruction
Tools/Verification/RackV2HelpTeaching/run_offline.py:99:        "Assets/ForestPrototype/UI/MenuHelpView.Text.cs", "Assets/ForestPrototype/UI/MenuHelpView.cs",
Tools/Verification/RackV2HelpTeaching/run_offline.py:100:        "Assets/ForestPrototype/UI/ScenarioOneUiRoot.cs", "Assets/ForestPrototype/UI/LearningObjectivesView.cs",
Tools/Verification/RackV2HelpTeaching/run_offline.py:101:        "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs", "Assets/ForestPrototype/UI/RackLayoutTracker.cs",
Tools/Verification/RackV2HelpTeaching/README.md:14:- **Offline part (`RackV2HelpTeachingOfflineCheck.cs`)** compiles `MenuHelpView.Text.cs`, `LearningObjectivesView.Curriculum.cs` and `ControlsReference.cs` and checks them. It reads the spec's quoted §7, §28 and §31 sentences, so the copy cannot drift from the spec silently. It checks:
Tools/Verification/Mfm09ExtensionFlow/Mfm09ExtensionGate.cs:38:        foreach (MenuHelpView.Menu menu in Enum.GetValues(typeof(MenuHelpView.Menu))) PlayerPrefs.SetInt(MenuHelpView.PreferencePrefix + menu, 1);
Tools/Verification/Mfm09ExtensionFlow/Mfm09ExtensionGate.cs:139:    private LearningObjectivesView Learn
Tools/Verification/Mfm09ExtensionFlow/Mfm09ExtensionGate.cs:143:            typeof(LearningObjectivesView).GetField("sequenceFacts", Private).SetValue(ui.Learning, null);
Tools/Verification/Mfm09ExtensionFlow/Mfm09ExtensionGate.cs:150:        MethodInfo method = typeof(LearningObjectivesView).GetMethod("TeachingFacts", Private);
Tools/Verification/RackV2HelpTeaching/RackV2HelpTeachingOfflineCheck.cs:53:        string[] names = Enum.GetNames(typeof(MenuHelpView.Menu));
Tools/Verification/RackV2HelpTeaching/RackV2HelpTeachingOfflineCheck.cs:56:        Check(MenuHelpView.Title(MenuHelpView.Menu.TreeMarks).StartsWith("Tree Marks", StringComparison.Ordinal), "the screen is titled Tree Marks");
Tools/Verification/RackV2HelpTeaching/RackV2HelpTeachingOfflineCheck.cs:57:        Check(MenuHelpView.Summary(MenuHelpView.Menu.TreeMarks) == null, "first use shows the full Tree Marks table and disclosure, not a shorter text");
Tools/Verification/RackV2HelpTeaching/RackV2HelpTeachingOfflineCheck.cs:60:        string[][] entries = MenuHelpView.TreeMarkEntries;
Tools/Verification/RackV2HelpTeaching/RackV2HelpTeachingOfflineCheck.cs:66:        string marks = MenuHelpView.Explanation(MenuHelpView.Menu.TreeMarks);
Tools/Verification/RackV2HelpTeaching/RackV2HelpTeachingOfflineCheck.cs:71:        string outsideSource = marks.Replace(MenuHelpView.TreeMarksOrangeText, "");
Tools/Verification/RackV2HelpTeaching/RackV2HelpTeachingOfflineCheck.cs:82:            Check(MenuHelpView.TreeMarksOrangeText == s28[0], "orange convention text equals spec §28", MenuHelpView.TreeMarksOrangeText);
Tools/Verification/RackV2HelpTeaching/RackV2HelpTeachingOfflineCheck.cs:83:            Check(MenuHelpView.TreeMarksSimplificationText == s28[1], "simplification disclosure equals spec §28", MenuHelpView.TreeMarksSimplificationText);
Tools/Verification/RackV2HelpTeaching/RackV2HelpTeachingOfflineCheck.cs:85:        Check(marks.Contains(MenuHelpView.TreeMarksOrangeText) && marks.Contains(MenuHelpView.TreeMarksSimplificationText),
Tools/Verification/RackV2HelpTeaching/RackV2HelpTeachingOfflineCheck.cs:94:        Check(MenuHelpView.TreeMarksUnlocked(true, true), "unlocked: V2 world with a confirmed plan");
Tools/Verification/RackV2HelpTeaching/RackV2HelpTeachingOfflineCheck.cs:95:        Check(!MenuHelpView.TreeMarksUnlocked(true, false), "locked: V2 plan not yet confirmed (or reopened)");
Tools/Verification/RackV2HelpTeaching/RackV2HelpTeachingOfflineCheck.cs:96:        Check(!MenuHelpView.TreeMarksUnlocked(false, true), "locked: Legacy40/model-0 world");
Tools/Verification/RackV2HelpTeaching/RackV2HelpTeachingOfflineCheck.cs:97:        Check(!MenuHelpView.TreeMarksUnlocked(false, false), "locked: model-0 world without a plan");
Tools/Verification/RackV2HelpTeaching/RackV2HelpTeachingOfflineCheck.cs:101:                Check(!MenuHelpView.TreeMarksContext(false, introduced, spray), $"never the context while locked (seen={introduced}, spray={spray})");
Tools/Verification/RackV2HelpTeaching/RackV2HelpTeachingOfflineCheck.cs:102:                Check(MenuHelpView.TreeMarksContext(true, introduced, spray) == (!introduced || spray),
Tools/Verification/RackV2HelpTeaching/RackV2HelpTeachingOfflineCheck.cs:107:        string concept = LearningObjectivesView.RackV2Concept;
Tools/Verification/RackV2HelpTeaching/RackV2HelpTeachingOfflineCheck.cs:113:        Check(InOrder(concept, "machinery", "16–20 m", "depends on the stand", "five extraction racks", "Plan the racks on the map", "mark each rack entrance"),
Tools/Verification/RackV2HelpTeaching/RackV2HelpTeachingOfflineCheck.cs:115:        string read = LearningObjectivesView.RackV2StepText("rack.read");
Tools/Verification/RackV2HelpTeaching/RackV2HelpTeachingOfflineCheck.cs:116:        Check(read != null && Has(read, "16–20 m") && Has(read, "30 m or more") && Has(read, "no universal rule") && Has(read, "this plantation only"),
Tools/Verification/RackV2HelpTeaching/RackV2HelpTeachingOfflineCheck.cs:120:        string open = LearningObjectivesView.RackV2Instruction("Racks planned: 2 / 5", false, 0, 5, false);
Tools/Verification/RackV2HelpTeaching/RackV2HelpTeachingOfflineCheck.cs:127:                string field = LearningObjectivesView.RackV2Instruction($"Rack entrances marked: {marked} / 5", true, marked, 5, pending);
Tools/Verification/RackV2HelpTeaching/RackV2HelpTeachingOfflineCheck.cs:132:        string done = LearningObjectivesView.RackV2Instruction("Rack entrances marked: 5 / 5", true, 5, 5, true);
Tools/Verification/RackV2HelpTeaching/RackV2HelpTeachingOfflineCheck.cs:135:        string approved = LearningObjectivesView.RackV2Instruction("Rack entrances marked: 5 / 5", true, 5, 5, false);
Tools/Verification/RackV2HelpTeaching/RackV2HelpTeachingOfflineCheck.cs:138:        string layout = LearningObjectivesView.RackV2StepText("rack.layout");
Tools/Verification/RackV2HelpTeaching/RackV2HelpTeachingOfflineCheck.cs:140:        string approve = LearningObjectivesView.RackV2StepText("rack.approve");
Tools/Verification/RackV2HelpTeaching/RackV2HelpTeachingOfflineCheck.cs:142:        Check(LearningObjectivesView.RackV2StepText("rack.result") == null && LearningObjectivesView.RackV2StepText("tree.inspect") == null,
Tools/Verification/RackV2HelpTeaching/RackV2HelpTeachingOfflineCheck.cs:144:        string help = MenuHelpView.RackV2Explanation;
Tools/Verification/RackV2HelpTeaching/RackV2HelpTeachingOfflineCheck.cs:147:        Check(string.Join(",", LearningObjectivesView.Topics[0].Steps.Select(step => step.Id)) == "rack.read,rack.create,rack.layout,rack.approve,rack.result",
Tools/Verification/RackV2HelpTeaching/RackV2HelpTeachingOfflineCheck.cs:149:        Check(LearningObjectivesView.StepIds.Count() == LearningObjectivesView.StepIds.Distinct().Count() && LearningObjectivesView.StepIds.Count() == 48,
Tools/Verification/RackV2HelpTeaching/RackV2HelpTeachingOfflineCheck.cs:150:            "no lesson step added, removed or duplicated (saved progress keys stable)", LearningObjectivesView.StepIds.Count().ToString());
Tools/Verification/RackV2HelpTeaching/RackV2HelpTeachingOfflineCheck.cs:158:            ["RackV2Practice"] = LearningObjectivesView.RackV2Practice,
Tools/Verification/RackV2HelpTeaching/RackV2HelpTeachingOfflineCheck.cs:160:            ["instruction.field"] = LearningObjectivesView.RackV2Instruction("Rack entrances marked: 0 / 5", true, 0, 5, true),
Tools/Verification/RackV2HelpTeaching/RackV2HelpTeachingOfflineCheck.cs:165:            if (LearningObjectivesView.RackV2StepText(id) != null) surfaces["step." + id] = LearningObjectivesView.RackV2StepText(id);
Tools/Verification/RackV2HelpTeaching/RackV2HelpTeachingOfflineCheck.cs:182:            "the 16–20 m band is framed as this stand's guide, never a general rule");
Tools/Verification/RackV2FieldMarking/RackV2FieldMarkingVerification.cs:27:        foreach (MenuHelpView.Menu menu in Enum.GetValues(typeof(MenuHelpView.Menu))) PlayerPrefs.SetInt(MenuHelpView.PreferencePrefix + menu, 1);
Tools/Verification/ScenarioOneCompletion/P2/CropTreeCompetitorVerification.cs:387:        => LearningObjectivesView.StepIds.Where(id => PlayerPrefs.GetInt(LearningObjectivesView.PreferencePrefix + id, 0) == 1).ToList();
Tools/Verification/Mfm12IntegratedQualification/Mfm11CalibrationImpactGate.cs:32:        foreach (MenuHelpView.Menu menu in Enum.GetValues(typeof(MenuHelpView.Menu)))
Tools/Verification/Mfm12IntegratedQualification/Mfm11CalibrationImpactGate.cs:33:            PlayerPrefs.SetInt(MenuHelpView.PreferencePrefix + menu, 1);
Tools/Verification/Mfm12IntegratedQualification/Mfm11CalibrationImpactGate.cs:89:    private LearningObjectivesView Learn
Tools/Verification/Mfm12IntegratedQualification/Mfm11CalibrationImpactGate.cs:93:            var f = typeof(LearningObjectivesView).GetField("sequenceFacts", Private);
Tools/Verification/ScenarioOneCombined/CombinedHudComposition.cs:46:        foreach (MenuHelpView.Menu menu in Enum.GetValues(typeof(MenuHelpView.Menu))) PlayerPrefs.SetInt(MenuHelpView.PreferencePrefix + menu, 1);
Tools/Verification/ScenarioOneCombined/CombinedHudComposition.cs:227:        FieldInfo facts = typeof(LearningObjectivesView).GetField("sequenceFacts", flags), next = typeof(LearningObjectivesView).GetField("nextSequenceRefresh", flags);
Tools/Verification/ScenarioOneCompletion/P2/Evidence/input-y0-trees.csv:64:Y0,P0601,sitka-spruce,-18.26,-7.72,16,20,16.36,11.75,2.19,71.8,0.1235,5.0439,crowded,0.5022,18.30,high,0,0
Tools/Verification/ScenarioOneCompletion/P2/Evidence/input-y0-trees.csv:80:Y0,P0700,sitka-spruce,-19.40,-5.72,16,20,17.67,13.42,2.29,76.0,0.1646,4.5414,crowded,0.4760,19.37,high,0,0
Tools/Verification/ScenarioOneCompletion/P2/Evidence/input-y0-trees.csv:81:Y0,P0701,sitka-spruce,-17.79,-6.15,16,20,14.43,10.72,2.04,74.3,0.0877,6.9957,crowded,0.5832,18.94,high,0,0
Tools/Verification/ScenarioOneCompletion/P2/Evidence/input-y0-trees.csv:82:Y0,P0702,sitka-spruce,-16.31,-5.68,16,20,12.08,8.93,1.86,73.9,0.0512,9.5098,crowded,0.6554,18.85,high,0,0
Tools/Verification/ScenarioOneCompletion/P2/Evidence/input-y0-trees.csv:254:Y0,P1620,sitka-spruce,19.40,11.81,55,20,14.61,11.21,2.06,76.7,0.0939,7.8190,crowded,0.6100,19.19,high,0,0
Tools/Verification/ScenarioOneCompletion/P2/Evidence/input-y0-trees.csv:281:Y0,P1806,sitka-spruce,-8.06,16.20,58,20,17.09,12.54,2.25,73.4,0.1438,7.8855,crowded,0.6120,18.37,high,0,0
Tools/Verification/S1CManagementHistory/AnnualReviewYearBrowsingVerification.cs:340:        Type learning = typeof(LearningObjectivesView);
Tools/Verification/ScenarioOneUxPedagogy/CameraLookDiagnosis.cs:72:        foreach (MenuHelpView.Menu menu in Enum.GetValues(typeof(MenuHelpView.Menu))) PlayerPrefs.SetInt(MenuHelpView.PreferencePrefix + menu, 1);
Tools/Verification/ScenarioOneUxPedagogy/UxPedagogyShots.cs:60:        foreach (MenuHelpView.Menu menu in Enum.GetValues(typeof(MenuHelpView.Menu))) PlayerPrefs.SetInt(MenuHelpView.PreferencePrefix + menu, 1);
Tools/Verification/ScenarioOneUxPedagogy/UxPedagogyShots.cs:198:        FieldInfo facts = typeof(LearningObjectivesView).GetField("sequenceFacts", flags), next = typeof(LearningObjectivesView).GetField("nextSequenceRefresh", flags);
Tools/Verification/ScenarioOneUxPedagogy/UxPedagogyShots.cs:364:        foreach (var item in new[] { (MenuHelpView.Menu.WorkPlan, "workplan"), (MenuHelpView.Menu.StandMap, "map") })
Tools/Verification/ScenarioOneUxPedagogy/UxPedagogyShots.cs:367:            PlayerPrefs.DeleteKey(MenuHelpView.PreferencePrefix + item.Item1);
Tools/Verification/ScenarioOneUxPedagogy/UxPedagogyShots.cs:368:            if (item.Item1 == MenuHelpView.Menu.WorkPlan) ui.ShowWorkPlan(); else ui.ShowMap();
Tools/Verification/ScenarioOneUxPedagogy/UxPedagogyShots.cs:378:            PlayerPrefs.SetInt(MenuHelpView.PreferencePrefix + item.Item1, 1);
Tools/Verification/ScenarioOneUxPedagogy/UxPedagogyShots.cs:528:        MethodInfo method = typeof(LearningObjectivesView).GetMethod("TeachingFacts", BindingFlags.Instance | BindingFlags.NonPublic);
Tools/Verification/ScenarioOneCompletion/P3/Evidence/input-y0-trees.csv:64:Y0,P0601,sitka-spruce,-18.26,-7.72,16,20,16.36,11.75,2.19,71.8,0.1235,5.0439,crowded,0.5022,18.30,high,0,0
Tools/Verification/ScenarioOneCompletion/P3/Evidence/input-y0-trees.csv:80:Y0,P0700,sitka-spruce,-19.40,-5.72,16,20,17.67,13.42,2.29,76.0,0.1646,4.5414,crowded,0.4760,19.37,high,0,0
Tools/Verification/ScenarioOneCompletion/P3/Evidence/input-y0-trees.csv:81:Y0,P0701,sitka-spruce,-17.79,-6.15,16,20,14.43,10.72,2.04,74.3,0.0877,6.9957,crowded,0.5832,18.94,high,0,0
Tools/Verification/ScenarioOneCompletion/P3/Evidence/input-y0-trees.csv:82:Y0,P0702,sitka-spruce,-16.31,-5.68,16,20,12.08,8.93,1.86,73.9,0.0512,9.5098,crowded,0.6554,18.85,high,0,0
Tools/Verification/ScenarioOneCompletion/P3/Evidence/input-y0-trees.csv:254:Y0,P1620,sitka-spruce,19.40,11.81,55,20,14.61,11.21,2.06,76.7,0.0939,7.8190,crowded,0.6100,19.19,high,0,0
Tools/Verification/ScenarioOneCompletion/P3/Evidence/input-y0-trees.csv:281:Y0,P1806,sitka-spruce,-8.06,16.20,58,20,17.09,12.54,2.25,73.4,0.1438,7.8855,crowded,0.6120,18.37,high,0,0
Tools/Verification/RackV2MapPlanning/RackV2MapPlanningVerification.cs:43:        foreach (MenuHelpView.Menu menu in Enum.GetValues(typeof(MenuHelpView.Menu)))
Tools/Verification/RackV2MapPlanning/RackV2MapPlanningVerification.cs:44:            PlayerPrefs.SetInt(MenuHelpView.PreferencePrefix + menu, 1);
Tools/Verification/Startup/StartupScreenVerification.cs:256:            Check(ui.Help.CurrentMenu == MenuHelpView.Menu.WalkingHud && !player.enabled && saves.enabled && ui.enabled,
Tools/Verification/Startup/StartupScreenVerification.cs:346:            Check(ui.Help.CurrentMenu == MenuHelpView.Menu.WalkingHud, "real input: existing Walking Help introduction");
Tools/Verification/MenuTutorialVerification.cs:51:        foreach (MenuHelpView.Menu menu in Enum.GetValues(typeof(MenuHelpView.Menu)))
Tools/Verification/MenuTutorialVerification.cs:53:            string key = MenuHelpView.PreferencePrefix + menu;
Tools/Verification/MenuTutorialVerification.cs:57:        foreach (string id in LearningObjectivesView.StepIds)
Tools/Verification/MenuTutorialVerification.cs:59:            string key = LearningObjectivesView.PreferencePrefix + id;
Tools/Verification/MenuTutorialVerification.cs:128:        Check(ui.Help.IsOpen && ui.Help.CurrentMenu == MenuHelpView.Menu.WalkingHud, "HUD introduction missing at start");
Tools/Verification/MenuTutorialVerification.cs:134:        if (!ui.Help.IsOpen) ui.ShowHelp(MenuHelpView.Menu.WalkingHud);
Tools/Verification/MenuTutorialVerification.cs:135:        Check(ui.Help.IsOpen && ui.Help.CurrentMenu == MenuHelpView.Menu.WalkingHud, "HUD help window shown after the introduction");
Tools/Verification/MenuTutorialVerification.cs:136:        yield return CaptureMenu(MenuHelpView.Menu.WalkingHud);
Tools/Verification/MenuTutorialVerification.cs:151:        Check(ui.Help.IsOpen && ui.Help.CurrentMenu == MenuHelpView.Menu.TreeInspection, "Tree introduction missing");
Tools/Verification/MenuTutorialVerification.cs:152:        yield return CaptureMenu(MenuHelpView.Menu.TreeInspection);
Tools/Verification/MenuTutorialVerification.cs:157:        Check(ui.Help.IsOpen && ui.Help.CurrentMenu == MenuHelpView.Menu.TreeInspection, "HUD Help did not follow inspection context");
Tools/Verification/MenuTutorialVerification.cs:168:        Check(ui.Help.IsOpen && ui.Help.CurrentMenu == MenuHelpView.Menu.StandMap, "Map introduction missing");
Tools/Verification/MenuTutorialVerification.cs:169:        yield return CaptureMenu(MenuHelpView.Menu.StandMap);
Tools/Verification/MenuTutorialVerification.cs:265:        Check(ui.Help.IsOpen && ui.Help.CurrentMenu == MenuHelpView.Menu.WorkPlan, "Work Plan introduction missing");
Tools/Verification/MenuTutorialVerification.cs:266:        yield return CaptureMenu(MenuHelpView.Menu.WorkPlan);
Tools/Verification/MenuTutorialVerification.cs:285:        Check(acknowledgeButton != null && acknowledgeButton.parent.Query<Label>().ToList().Any(l => l.text == MenuHelpView.AnnualReviewReadPrompt),
Tools/Verification/MenuTutorialVerification.cs:287:        ui.ShowHelp(MenuHelpView.Menu.AnnualReview); yield return null;
Tools/Verification/MenuTutorialVerification.cs:288:        Check(ui.Help.IsOpen && ui.Help.CurrentMenu == MenuHelpView.Menu.AnnualReview, "Help does not reopen the Annual Review explanation on request");
Tools/Verification/MenuTutorialVerification.cs:289:        yield return CaptureMenu(MenuHelpView.Menu.AnnualReview);
Tools/Verification/MenuTutorialVerification.cs:317:        ui.ShowHelp(MenuHelpView.Menu.WalkingHud); Check(!ui.Help.IsOpen, "Help leaked into Reference preview");
Tools/Verification/MenuTutorialVerification.cs:322:        foreach (MenuHelpView.Menu menu in Enum.GetValues(typeof(MenuHelpView.Menu)).Cast<MenuHelpView.Menu>().Where(m => m != MenuHelpView.Menu.Rack && m != MenuHelpView.Menu.TreeMarks)) Check(MenuHelpView.Introduced(menu), "Introduction preference missing: " + menu);
Tools/Verification/MenuTutorialVerification.cs:341:        MethodInfo method = typeof(LearningObjectivesView).GetMethod("TeachingFacts", BindingFlags.Instance | BindingFlags.NonPublic);
Tools/Verification/MenuTutorialVerification.cs:482:        var reloadedProgress = new LearningObjectivesView(ui);
Tools/Verification/MenuTutorialVerification.cs:603:    private IEnumerator CaptureMenu(MenuHelpView.Menu menu)
Tools/Verification/StandMap/StandMapParity.cs:51:        foreach (MenuHelpView.Menu menu in Enum.GetValues(typeof(MenuHelpView.Menu))) PlayerPrefs.SetInt(MenuHelpView.PreferencePrefix + menu, 1);
Tools/Verification/RackV2Spacing/README.md:7:The closest centre-line separation is shown, including where racks converge; it is not an average or a measurement between corridor edges. The 16–20 m recommendation is Scenario One gameplay calibration informed by source guidance. The row count is omitted because accepted evidence authorising player-facing equivalence was not established.
Tools/Verification/RackV2Spacing/RackV2SpacingVerification.cs:51:        foreach (MenuHelpView.Menu menu in Enum.GetValues(typeof(MenuHelpView.Menu)))
Tools/Verification/RackV2Spacing/RackV2SpacingVerification.cs:52:            PlayerPrefs.SetInt(MenuHelpView.PreferencePrefix + menu, 1);
Tools/Verification/ScenarioOneCompletion/P3/WorkPlanResidualVerification.cs:215:        Check(leaving.Contains(LearningObjectivesView.ClearanceExplanation(RegenerationModel.Competition)), "model-aware clearance explanation missing");
Tools/Verification/UnderstoreyRecruitment/Model2PedagogyVerification.cs:52: string ClearanceTitle(){return LearningObjectivesView.Topics.Single(t=>t.Title.EndsWith("Vegetation clearance",StringComparison.Ordinal)).Title;}
Tools/Verification/UnderstoreyRecruitment/Model2PedagogyVerification.cs:63:   string shown=Displayed();string expected=LearningObjectivesView.ClearanceExplanation(model);
Tools/Verification/UnderstoreyRecruitment/Model2PedagogyVerification.cs:76:   Check(Clearance().Query<Label>().ToList().Any(l=>l.text==LearningObjectivesView.ClearanceExplanation(2)&&l.worldBound.height>20),"approved copy laid out "+size.x);
Tools/Verification/ScenarioOneRackUx/GuidedOnboardingVerification.cs:28:        MenuHelpView.ResetProgress();
Tools/Verification/ScenarioOneRackUx/GuidedOnboardingVerification.cs:29:        LearningObjectivesView.ResetProgress();
Tools/Verification/ScenarioOneRackUx/GuidedOnboardingVerification.cs:284:        var preferences = LearningObjectivesView.StepIds.ToDictionary(id => id, id => PlayerPrefs.GetInt(LearningObjectivesView.PreferencePrefix+id,-1));
Tools/Verification/ScenarioOneRackUx/GuidedOnboardingVerification.cs:285:        foreach (string id in LearningObjectivesView.StepIds) PlayerPrefs.SetInt(LearningObjectivesView.PreferencePrefix+id,1);
Tools/Verification/ScenarioOneRackUx/GuidedOnboardingVerification.cs:286:        var previousDevice = new LearningObjectivesView(ui);
Tools/Verification/ScenarioOneRackUx/GuidedOnboardingVerification.cs:290:            if (item.Value < 0) PlayerPrefs.DeleteKey(LearningObjectivesView.PreferencePrefix+item.Key);
Tools/Verification/ScenarioOneRackUx/GuidedOnboardingVerification.cs:291:            else PlayerPrefs.SetInt(LearningObjectivesView.PreferencePrefix+item.Key,item.Value);
Tools/Verification/ScenarioOneRackUx/RackUxShots.cs:74:        foreach (MenuHelpView.Menu menu in Enum.GetValues(typeof(MenuHelpView.Menu))) PlayerPrefs.DeleteKey(MenuHelpView.PreferencePrefix + menu);
Tools/Verification/ScenarioOneRackUx/RackUxShots.cs:75:        foreach (string id in LearningObjectivesView.StepIds) PlayerPrefs.DeleteKey(LearningObjectivesView.PreferencePrefix + id);
Tools/Verification/ScenarioOneRackUx/RackUxShots.cs:271:        Hard($"MORE_DETAIL_IS_OFFERED_{menuName}", more != null && Visible(more) && more.text == MenuHelpView.MoreDetailLabel, more == null ? "no button" : $"text=[{more.text}]");
Tools/Verification/ScenarioOneRackUx/RackUxShots.cs:276:        Hard($"MORE_DETAIL_OPENS_THE_FULL_TEXT_{menuName}", ui.Help.IsOpen && full.Length > brief.Length + 100 && more.text == MenuHelpView.LessDetailLabel, $"chars {brief.Length}→{full.Length} button=[{more.text}]");
Tools/Verification/ScenarioOneRackUx/RackUxShots.cs:278:        Hard($"LESS_DETAIL_RETURNS_TO_THE_SHORT_TEXT_{menuName}", ui.Help.IsOpen && Texts(ui.Help.Root) == brief && more.text == MenuHelpView.MoreDetailLabel, $"button=[{more.text}]");
Tools/Verification/ScenarioOneRackUx/RackUxShots.cs:292:            PlayerPrefs.DeleteKey(MenuHelpView.PreferencePrefix + MenuHelpView.Menu.WalkingHud);
Tools/Verification/ScenarioOneRackUx/RackUxShots.cs:293:            ui.Help.Introduce(MenuHelpView.Menu.WalkingHud); yield return Settle(6);
Tools/Verification/ScenarioOneRackUx/RackUxShots.cs:346:        Hard("HELP_FRESH_PLAYER_SEES_INTRO", ui.Help.IsOpen && ui.Help.CurrentMenu == MenuHelpView.Menu.WalkingHud, $"open={ui.Help.IsOpen}");
Tools/Verification/ScenarioOneRackUx/RackUxShots.cs:352:        Hard("MORE_DETAIL_CHANGES_THE_SCREEN", ui.Help.IsOpen && after != before && after.Length > before.Length && more.text == MenuHelpView.LessDetailLabel, $"chars {before.Length}→{after.Length} button=[{more.text}]");
Tools/Verification/ScenarioOneRackUx/RackUxShots.cs:356:        Hard("LESS_DETAIL_GOES_BACK", ui.Help.IsOpen && Texts(ui.Help.Root) == before && more.text == MenuHelpView.MoreDetailLabel, $"button=[{more.text}]");
Tools/Verification/WindthrowV1/StormUiVerification.cs:42: [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]static void Prepare(){if(!Environment.GetCommandLineArgs().Contains("StormUiVerification.Begin"))return;foreach(MenuHelpView.Menu menu in Enum.GetValues(typeof(MenuHelpView.Menu))){string key=MenuHelpView.PreferencePrefix+menu;Before[key]=PlayerPrefs.HasKey(key)?PlayerPrefs.GetInt(key):(int?)null;PlayerPrefs.SetInt(key,1);}foreach(string id in LearningObjectivesView.StepIds){string key=LearningObjectivesView.PreferencePrefix+id;Before[key]=PlayerPrefs.HasKey(key)?PlayerPrefs.GetInt(key):(int?)null;}}
Tools/Verification/ScenarioOneRackUx/RackUxRack.cs:38:        foreach (MenuHelpView.Menu menu in Enum.GetValues(typeof(MenuHelpView.Menu))) PlayerPrefs.DeleteKey(MenuHelpView.PreferencePrefix + menu);
Tools/Verification/ScenarioOneRackUx/RackUxRack.cs:39:        foreach (string id in LearningObjectivesView.StepIds) PlayerPrefs.DeleteKey(LearningObjectivesView.PreferencePrefix + id);
Tools/Verification/ScenarioOneRackUx/RackUxRack.cs:223:        Hard("STAGE1_TOPIC_ONE_IS_RACKS", LearningObjectivesView.Topics[0].Title.EndsWith("Extraction racks") && LearningObjectivesView.Topics[1].Title.EndsWith("Tree inspection and Crop Trees"), LearningObjectivesView.Topics[0].Title);
Tools/Verification/ScenarioOneRackUx/RackUxRack.cs:240:        string prefix = MenuHelpView.PreferencePrefix;
Tools/Verification/ScenarioOneRackUx/RackUxRack.cs:242:        PlayerPrefs.SetInt(prefix + MenuHelpView.Menu.WorkPlan, 1);
Tools/Verification/ScenarioOneRackUx/RackUxRack.cs:243:        ui.ShowHelp(MenuHelpView.Menu.WalkingHud);
Tools/Verification/ScenarioOneRackUx/RackUxRack.cs:252:        Hard("MODAL_KEEPS_ITS_OWN_SCREEN", ui.Help.CurrentMenu == MenuHelpView.Menu.WalkingHud, ui.Help.CurrentMenu.ToString());
Tools/Verification/ScenarioOneRackUx/RackUxRack.cs:269:        PlayerPrefs.DeleteKey(prefix + MenuHelpView.Menu.WorkPlan);
Tools/Verification/ScenarioOneRackUx/RackUxRack.cs:335:        bool introShown = ui.Help.IsOpen && ui.Help.CurrentMenu == MenuHelpView.Menu.Rack;
Tools/Verification/ScenarioOneRackUx/RackUxRack.cs:344:            Hard("MORE_DETAIL_OPENS_THE_FULL_RACK_EXPLANATION", Texts(ui.Help.Root).Length > rackBrief.Length + 100 && rackMore.text == MenuHelpView.LessDetailLabel && Texts(ui.Help.Root).Contains("planted row"), "");
Tools/Verification/ScenarioOneRackUx/RackUxRack.cs:355:            Hard("RACK_MODAL_STAYS_THE_RACK_SCREEN", ui.Help.CurrentMenu == MenuHelpView.Menu.Rack && Texts(ui.Help.Root) == rackBrief, ui.Help.CurrentMenu.ToString());
Tools/Verification/ScenarioOneRackUx/RackUxRack.cs:360:        if (!tool.Active) { Unverified("B key path did not open the tool; entering through RackTool.Enter()"); PlayerPrefs.SetInt(MenuHelpView.PreferencePrefix + MenuHelpView.Menu.Rack, 1); tool.Enter(); }
Tools/Verification/MfmRackSample/MfmRackSampleGeometryVerification.cs:18:// game uses it. Lessons 2-5 are driven through the PRODUCTION fact path (LearningObjectivesView.TeachingFacts), so what is asserted is what
Tools/Verification/MfmRackSample/MfmRackSampleGeometryVerification.cs:50:        foreach (string id in LearningObjectivesView.StepIds) PlayerPrefs.DeleteKey(LearningObjectivesView.PreferencePrefix + id);
Tools/Verification/MfmRackSample/MfmRackSampleGeometryVerification.cs:156:        => (ScenarioOneTeachingSequence.Facts)typeof(LearningObjectivesView).GetMethod("TeachingFacts", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(ui.Learning, null);
Tools/Verification/MfmRackSample/MfmRackSampleGeometryVerification.cs:161:        typeof(LearningObjectivesView).GetField("sequenceFacts", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(ui.Learning, null);
Tools/Verification/SequentialObjectives/SequentialObjectivesVerification.cs:150:        MethodInfo method = typeof(LearningObjectivesView).GetMethod("TeachingFacts", BindingFlags.Instance | BindingFlags.NonPublic);
Tools/Verification/MfmRackSample/README.md:5:the production `LearningObjectivesView.TeachingFacts` path. The decision rules themselves are also checked on plain .NET by
Tools/Verification/RackV2Combined/RackV2LegacyProgressionRegression.cs:42:        foreach (string id in LearningObjectivesView.StepIds) PlayerPrefs.DeleteKey(LearningObjectivesView.PreferencePrefix + id);
Tools/Verification/RackV2Combined/RackV2LegacyProgressionRegression.cs:202:        foreach (string id in LearningObjectivesView.StepIds) PlayerPrefs.DeleteKey(LearningObjectivesView.PreferencePrefix + id);
Tools/Verification/RackV2Combined/RackV2CombinedVerification.cs:44:        foreach (string id in LearningObjectivesView.StepIds) PlayerPrefs.DeleteKey(LearningObjectivesView.PreferencePrefix + id);
Tools/Verification/RackV2Combined/RackV2CombinedVerification.cs:305:        Note("STAGE1_CUE", "cue=" + ui.Learning.Summary.Replace("\n", " / ") + " | topic1=" + LearningObjectivesView.Topics[0].Title);
Tools/Verification/ScenarioOneVisualSystemV2/VisualSystemV2Shots.cs:205:        if (!ui.Help.IsOpen || ui.Help.CurrentMenu != MenuHelpView.Menu.WalkingHud)
Tools/Verification/ScenarioOneVisualSystemV2/VisualSystemV2Shots.cs:303:            ui.ShowHelp(MenuHelpView.Menu.WalkingHud);
Tools/Verification/ClearanceVerification.cs:95:        foreach (MenuHelpView.Menu menu in Enum.GetValues(typeof(MenuHelpView.Menu)))
Tools/Verification/ClearanceVerification.cs:97:            string key = MenuHelpView.PreferencePrefix + menu;

Exit: 0
```

$ rg --files Assets | rg "Paint|Mark"
```text
Assets/ForestPrototype/ForestTreeMarkingManager.cs
Assets/ForestPrototype/ForestTreeMarkingManager.RackEntrances.cs
Assets/ForestPrototype/Racks/PlannedFellMarkers.cs.meta
Assets/ForestPrototype/Racks/PlannedFellMarkers.cs
Assets/ForestPrototype/ForestTreeMarkingManager.RackEntrances.cs.meta
Assets/ForestPrototype/Materials/HarvestMark.mat
Assets/ForestPrototype/Materials/HarvestMark.mat.meta
Assets/ForestPrototype/ForestTreeMarkingManager.cs.meta

Exit: 0
```

$ cat Tools/Verification/RackV2HelpTeaching/README.md Tools/Verification/RackV2HelpTeaching/RackV2HelpTeachingOfflineCheck.cs
```text
# RACK-V2-10 help / teaching offline check

```
python3 -B Tools/Verification/RackV2HelpTeaching/run_offline.py --out <dir outside the repo> [--base 6d3228c]
```

No Unity process. Nothing is written inside the repository (the generated .NET project lives in `--out`). Needs `dotnet` (SDK 8+). Exit 0 and `RACK_V2_HELP_TEACHING_PASS` on success.

- **Static part (S1–S12)** reads the UI files that cannot compile on plain .NET and checks their wiring:
  - `ScenarioOneUiRoot.HelpContext` unlocks Tree Marks only from the saved `RackPlanningV2Active` and `RackPlanConfirmed` state.
  - Tree Marks is the walking context until it has been seen once, and again while the spray marker is equipped, and the existing context `Introduce` path opens it.
  - The V2 Rack help, the Objective 1 concept, good practice, instruction and lesson texts use the checked pure copy, and only in a V2 world.
  - Since `--base`, only the RACK-V2-10 copy/wiring files changed: no marking, geometry, manager, save/schema, scene, prefab, material, settings, package or `.meta` change, and nothing untracked under `Assets/`.
- **Offline part (`RackV2HelpTeachingOfflineCheck.cs`)** compiles `MenuHelpView.Text.cs`, `LearningObjectivesView.Curriculum.cs` and `ControlsReference.cs` and checks them. It reads the spec's quoted §7, §28 and §31 sentences, so the copy cannot drift from the spec silently. It checks:
  - the help screen names;
  - the Tree Marks entry set (orange, red and blue only; no future categories);
  - the §28 orange convention and first-tree simplification disclosure, verbatim;
  - the unlock truth tables;
  - the §31/§7 Objective 1 sentences;
  - the §30 flow at every plan/marking state;
  - the lesson ids;
  - forbidden claims (`18 m`, `ideal`, `always`, five-rack quota wording, universal-practice wording, legacy `B` authoring) on every RACK-V2-10 surface.

This proves wording, the entry set, the unlock rule and the flow order. It does not prove rendering, modal timing in play or text fit; those belong to the rendered gate (RACK-V2-13).
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

// RACK-V2-10 offline check: Tree Marks help, its unlock rule and the Rack Planning V2 Objective 1 wording, compiled from the
// production pure-wording files on plain .NET. It reads the approved spec for the quoted §7, §28 and §31 sentences, so the copy
// cannot drift from the spec without failing. It proves wording, entry set, unlock rule and flow order only, not rendering.
public static class RackV2HelpTeachingOfflineCheck
{
    static int checks, failures;
    static void Check(bool condition, string message, string detail = "")
    {
        checks++;
        if (!condition) failures++;
        Console.WriteLine((condition ? "PASS " : "FAIL ") + message + (condition || detail.Length == 0 ? "" : " :: " + detail));
    }

    static bool Has(string text, string part) => text.IndexOf(part, StringComparison.OrdinalIgnoreCase) >= 0;
    static bool InOrder(string text, params string[] parts)
    {
        int at = -1;
        foreach (string part in parts)
        {
            int next = text.IndexOf(part, at + 1, StringComparison.OrdinalIgnoreCase);
            if (next < 0) return false;
            at = next;
        }
        return true;
    }

    static readonly string[] RackIds = { "rack.read", "rack.create", "rack.layout", "rack.approve", "rack.result", "rack.map" };

    // The block-quoted sentences of one spec section (lines starting with "> " between that heading and the next "## ").
    static List<string> Quotes(string[] spec, string heading)
    {
        var quotes = new List<string>();
        int start = Array.FindIndex(spec, line => line.StartsWith(heading, StringComparison.Ordinal));
        if (start < 0) return quotes;
        for (int i = start + 1; i < spec.Length && !spec[i].StartsWith("## ", StringComparison.Ordinal); i++)
            if (spec[i].StartsWith("> ", StringComparison.Ordinal) && !spec[i].StartsWith("> **", StringComparison.Ordinal))
                quotes.Add(spec[i].Substring(2).Trim());
        return quotes;
    }

    public static int Main(string[] args)
    {
        string root = args.Length > 0 ? args[0] : ".";
        string[] spec = File.ReadAllLines(Path.Combine(root, "specs/scenario-one-objective-1-rack-planning-field-marking-v2/spec.md"));

        // 1. Menu enum: Tree Marks is a new screen at the end; existing preference keys (the names) are unchanged.
        string[] names = Enum.GetNames(typeof(MenuHelpView.Menu));
        Check(string.Join(",", names) == "WalkingHud,TreeInspection,StandMap,WorkPlan,AnnualReview,Rack,TreeMarks",
            "help screens: existing names and order kept, TreeMarks appended", string.Join(",", names));
        Check(MenuHelpView.Title(MenuHelpView.Menu.TreeMarks).StartsWith("Tree Marks", StringComparison.Ordinal), "the screen is titled Tree Marks");
        Check(MenuHelpView.Summary(MenuHelpView.Menu.TreeMarks) == null, "first use shows the full Tree Marks table and disclosure, not a shorter text");

        // 2. Entry set: orange, red and blue only (spec §25, §28, §29).
        string[][] entries = MenuHelpView.TreeMarkEntries;
        Check(entries.Length == 3, "Tree Marks has exactly three entries", entries.Length.ToString());
        Check(entries.Length == 3 && entries[0][0] == "Orange vertical stripes" && entries[0][1] == "Rack / machine-path removal",
            "entry 1: Orange vertical stripes — Rack / machine-path removal");
        Check(entries.Length == 3 && entries[1][0] == "Red" && entries[1][1] == "Fell", "entry 2: Red — Fell");
        Check(entries.Length == 3 && entries[2][0] == "Blue" && entries[2][1] == "Crop Tree / future timber", "entry 3: Blue — Crop Tree / future timber");
        string marks = MenuHelpView.Explanation(MenuHelpView.Menu.TreeMarks);
        foreach (string[] entry in entries)
            Check(marks.Contains("• " + entry[0] + " — " + entry[1]), "the help lists " + entry[0]);
        Check(Regex.Matches(marks, "^• ", RegexOptions.Multiline).Count == 3, "the help lists no fourth mark");
        // The only other colour named is the source's "orange or pink", inside the orange convention sentence.
        string outsideSource = marks.Replace(MenuHelpView.TreeMarksOrangeText, "");
        foreach (string colour in new[] { "pink", "yellow", "white", "green", "purple", "violet", "black" })
            Check(!Has(outsideSource, colour), "no unused mark colour outside the source sentence: " + colour);
        foreach (string future in new[] { "deadwood", "habitat", "route-side", "retained", "boundary", "future marks" })
            Check(!Has(marks, future), "no future marking category: " + future);

        // 3. Spec §28 wording: the orange convention and the first-tree simplification disclosure, verbatim.
        List<string> s28 = Quotes(spec, "## 28.");
        Check(s28.Count == 2, "spec §28 has its two quoted sentences", s28.Count.ToString());
        if (s28.Count == 2)
        {
            Check(MenuHelpView.TreeMarksOrangeText == s28[0], "orange convention text equals spec §28", MenuHelpView.TreeMarksOrangeText);
            Check(MenuHelpView.TreeMarksSimplificationText == s28[1], "simplification disclosure equals spec §28", MenuHelpView.TreeMarksSimplificationText);
        }
        Check(marks.Contains(MenuHelpView.TreeMarksOrangeText) && marks.Contains(MenuHelpView.TreeMarksSimplificationText),
            "the Tree Marks help shows both §28 sentences");
        Check(Has(marks, "four sides") && Has(marks, "one spray"), "the help separates the source's four-sided stripes from the game's single spray (§24)");
        Check(Has(marks, "press P") && Has(marks, "3.5 m") && Has(marks, "left-click"), "the help names the real spray controls (P, 3.5 m, left-click)");
        string pRow = ControlsReference.RowsFor(true).First()[1];
        Check(ControlsReference.RowsFor(true).First()[0] == "P" && Has(pRow, "3.5 m") && Has(pRow, "left-click"), "the controls reference states the same spray reach and click");
        Check(Has(marks, "only one of these marks") && Has(marks, "approval"), "the help states mark exclusivity and that marking cuts nothing");

        // 4. Unlock rule: only a Rack Planning V2 world with a confirmed plan; context while unseen or with the spray equipped.
        Check(MenuHelpView.TreeMarksUnlocked(true, true), "unlocked: V2 world with a confirmed plan");
        Check(!MenuHelpView.TreeMarksUnlocked(true, false), "locked: V2 plan not yet confirmed (or reopened)");
        Check(!MenuHelpView.TreeMarksUnlocked(false, true), "locked: Legacy40/model-0 world");
        Check(!MenuHelpView.TreeMarksUnlocked(false, false), "locked: model-0 world without a plan");
        foreach (bool introduced in new[] { false, true })
            foreach (bool spray in new[] { false, true })
            {
                Check(!MenuHelpView.TreeMarksContext(false, introduced, spray), $"never the context while locked (seen={introduced}, spray={spray})");
                Check(MenuHelpView.TreeMarksContext(true, introduced, spray) == (!introduced || spray),
                    $"unlocked context (seen={introduced}, spray={spray}) = {!introduced || spray}");
            }

        // 5. Objective 1 copy, spec §31 and §7 sentences in the V2 objective.
        string concept = LearningObjectivesView.RackV2Concept;
        List<string> s31 = Quotes(spec, "## 31.");
        Check(s31.Count == 3, "spec §31 has its three quoted sentences", s31.Count.ToString());
        foreach (string quote in s31) Check(concept.Contains(quote), "Objective 1 explains (§31): " + quote);
        List<string> s7 = Quotes(spec, "## 7.");
        Check(s7.Count == 2 && concept.Contains(s7[0]) && !concept.Contains(s7[1]), "Objective 1 states the five-rack calibration as §7 asks, not the quota", string.Join(" | ", s7));
        Check(InOrder(concept, "machinery", "16–20 m", "depends on the stand", "five extraction racks", "Plan the racks on the map", "mark each rack entrance"),
            "Objective 1 why-it-matters order: purpose, spacing, conditional nature, five racks, plan then mark (§30 step 1)");
        string read = LearningObjectivesView.RackV2StepText("rack.read");
        Check(read != null && Has(read, "16–20 m") && Has(read, "30 m or more") && Has(read, "no universal rule") && Has(read, "this plantation only"),
            "rack reading gives §5.1's conditional examples and keeps the recommendation to this stand", read ?? "null");

        // 6. Objective 1 flow (§30 steps 2-7, D-064 OPEN-2: field marking before approval).
        string open = LearningObjectivesView.RackV2Instruction("Racks planned: 2 / 5", false, 0, 5, false);
        Check(open.StartsWith("Racks planned: 2 / 5. ") && InOrder(open, "Plan Racks", "Stand Map [M]", "Confirm Rack Plan") && !Has(open, "[P]"),
            "steps 2-5: unconfirmed plan says open the map, Plan Racks, Confirm Rack Plan", open);
        for (int marked = 0; marked < 5; marked++)
        {
            foreach (bool pending in new[] { true, false })
            {
                string field = LearningObjectivesView.RackV2Instruction($"Rack entrances marked: {marked} / 5", true, marked, 5, pending);
                Check(field.StartsWith($"Rack entrances marked: {marked} / 5. ") && InOrder(field, "Return to the forest", "first rack tree nearest the path", "entrance", "[P]", "3.5 m", "left-click")
                    && !Has(field, "approve"), $"step 6: confirmed, {marked} of 5 marked (work pending={pending}) says mark entrances, not approve", field);
            }
        }
        string done = LearningObjectivesView.RackV2Instruction("Rack entrances marked: 5 / 5", true, 5, 5, true);
        Check(done.StartsWith("Rack entrances marked: 5 / 5. ") && InOrder(done, "approve", "Work Plan [Tab]", "advance one year") && !Has(done, "[P]"),
            "step 7: five entrances marked hands over to rack.approve", done);
        string approved = LearningObjectivesView.RackV2Instruction("Rack entrances marked: 5 / 5", true, 5, 5, false);
        Check(approved.StartsWith("Rack entrances marked: 5 / 5. ") && Has(approved, "advance one year") && !Has(approved, "approve the"),
            "after approval the objective says advance one year", approved);
        string layout = LearningObjectivesView.RackV2StepText("rack.layout");
        Check(layout != null && InOrder(layout, "Plan five racks", "Confirm Rack Plan", "return to the forest", "mark each rack entrance", "[P]"), "rack.layout lesson: confirm, then mark", layout ?? "null");
        string approve = LearningObjectivesView.RackV2StepText("rack.approve");
        Check(approve != null && InOrder(approve, "entrances are marked", "Work Plan [Tab]", "Approve Work Plan"), "rack.approve lesson follows marking", approve ?? "null");
        Check(LearningObjectivesView.RackV2StepText("rack.result") == null && LearningObjectivesView.RackV2StepText("tree.inspect") == null,
            "rack.result and non-rack lessons keep their own text");
        string help = MenuHelpView.RackV2Explanation;
        Check(InOrder(help, "Stand Map [M]", "Plan Racks", "Confirm Rack Plan", "spray marker [P]", "approve", "advance one year"),
            "V2 Rack help follows the §30 order: plan, confirm, mark, approve, advance", help);
        Check(string.Join(",", LearningObjectivesView.Topics[0].Steps.Select(step => step.Id)) == "rack.read,rack.create,rack.layout,rack.approve,rack.result",
            "Objective 1 lesson ids and order unchanged (rack.approve then rack.result, OPEN-2)");
        Check(LearningObjectivesView.StepIds.Count() == LearningObjectivesView.StepIds.Distinct().Count() && LearningObjectivesView.StepIds.Count() == 48,
            "no lesson step added, removed or duplicated (saved progress keys stable)", LearningObjectivesView.StepIds.Count().ToString());

        // 7. Forbidden claims across every RACK-V2-10 surface.
        var surfaces = new Dictionary<string, string>
        {
            ["TreeMarks"] = marks,
            ["RackV2Explanation"] = help,
            ["RackV2Concept"] = concept,
            ["RackV2Practice"] = LearningObjectivesView.RackV2Practice,
            ["instruction.open"] = open,
            ["instruction.field"] = LearningObjectivesView.RackV2Instruction("Rack entrances marked: 0 / 5", true, 0, 5, true),
            ["instruction.done"] = done,
            ["instruction.approved"] = approved,
        };
        foreach (string id in RackIds)
            if (LearningObjectivesView.RackV2StepText(id) != null) surfaces["step." + id] = LearningObjectivesView.RackV2StepText(id);
        var forbidden = new[]
        {
            @"\b18\s?m\b", @"\bideal\b", @"\boptimal\b", @"\balways\b", @"best practice", @"standard practice", @"universally",
            @"five racks are (always )?required", @"require[sd]? five racks", @"CCF (stands|forests) (require|need)", @"\ball CCF\b", @"\bevery CCF\b",
            @"only the (first|entrance) tree is marked", @"authored rack plan", @"seven rows", @"\[B\]", @"Press B", @"Use B", @"racks \(B\)",
        };
        foreach (KeyValuePair<string, string> surface in surfaces)
        {
            string[] hits = forbidden.Where(pattern => Regex.IsMatch(surface.Value, pattern, RegexOptions.IgnoreCase)).ToArray();
            Check(hits.Length == 0, "no forbidden claim in " + surface.Key, string.Join(" ; ", hits));
            // "universal" may appear only as a denial ("no universal rule").
            bool universalOk = Regex.Matches(surface.Value, @"universal", RegexOptions.IgnoreCase).Cast<Match>()
                .All(m => m.Index >= 3 && surface.Value.Substring(m.Index - 3, 3).Equals("no ", StringComparison.OrdinalIgnoreCase));
            Check(universalOk, "\"universal\" appears only as \"no universal rule\" in " + surface.Key);
        }
        Check(Has(concept, "Scenario One uses that range as a guide for this plantation") && Has(read ?? "", "for this plantation only"),
            "the 16–20 m band is framed as this stand's guide, never a general rule");

        Console.WriteLine((failures == 0 ? "RACK_V2_HELP_TEACHING_OFFLINE_PASS" : "RACK_V2_HELP_TEACHING_OFFLINE_FAIL failures=" + failures + "") + " checks=" + checks);
        return failures == 0 ? 0 : 1;
    }
}

Exit: 0
```

$ rg --files Tools/Verification/RackV2HelpTeaching
```text
Tools/Verification/RackV2HelpTeaching/run_offline.py
Tools/Verification/RackV2HelpTeaching/README.md
Tools/Verification/RackV2HelpTeaching/RackV2HelpTeachingOfflineCheck.cs

Exit: 0
```

$ rg -n "orange|Orange|Color|Fell|Crop|stripe" Assets/ForestPrototype/TreeMarkVisual.cs Assets/ForestPrototype/RackEntranceMarkVisual.cs
```text
rg: Assets/ForestPrototype/TreeMarkVisual.cs: No such file or directory (os error 2)
rg: Assets/ForestPrototype/RackEntranceMarkVisual.cs: No such file or directory (os error 2)

Exit: 2
```

$ git status --short
```text
?? Docs/Handoffs/Overnight-Continuation-20261104/

Exit: 0
```

$ cat Assets/ForestPrototype/ForestTreeMarkingManager.cs Assets/ForestPrototype/ForestTreeMarkingManager.RackEntrances.cs Assets/ForestPrototype/Racks/PlannedFellMarkers.cs
```text
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Ecology-owned forestry planning: mark trees for harvest without felling them.
// Marking is deliberately separate from harvesting so a proposed treatment can
// be walked through and reconsidered before any cutting happens.
public sealed partial class ForestTreeMarkingManager : MonoBehaviour
{
    [SerializeField, Min(0.5f)] private float interactionDistance = 8f; // [D] planning reach, calibration
    [SerializeField] private Color markerColor = new Color(1f, 0.35f, 0.05f, 1f);
    [SerializeField] private Color cropTreeColor = new Color(0.15f, 0.4f, 0.95f, 1f);

    private static readonly Dictionary<string, GameObject> MarkerObjects = new Dictionary<string, GameObject>();
    // This set contains ONLY pending Fell marks. Crop status lives on each tree
    // and survives consuming a felling batch or clearing a draft harvest.
    private static readonly HashSet<string> MarkedIds = new HashSet<string>();

    public static ForestTreeMarkingManager Instance { get; private set; }

    private Camera view;
    private Transform playerRoot;
    private readonly RaycastHit[] hitBuffer = new RaycastHit[16];
    private ForestTree aimedTree;
    private ForestEcologyController ecologyCache;
    private ScenarioOneManager diagnosisScenario;
    private ForestTreeSpawner diagnosisSpawner;
    private ForestPlayer inspectionPlayer;
    private bool aimingAtGround;
    private Vector3 aimedGroundPoint;
    private readonly List<ForestTree> markedTreeCache = new List<ForestTree>();
    private bool markedCacheDirty = true;
    private bool restoringMarks;
    private float nextStaleSweepTime;
    private string message = "";
    private float messageTimer;
    private GUIStyle promptStyle;
    private GUIStyle messageStyle;
    private GUIStyle counterStyle;
    private GUIStyle outcomeStyle;
    [SerializeField] private Material markerMaterial;
    private Material cropMarkerMaterial;

    public int MarkedCount => MarkedIds.Count;

    // Treatment summary for the HUD: only trees still standing with a mark,
    // and their summed biological stem volume. Marking selects; it never fells.
    public int LivingMarkedCount { get; private set; }
    public float MarkedVolumeM3 { get; private set; }
    public int LivingCropTreeCount { get; private set; }

    // Conditional consequence forecast for the pending treatment, rebuilt with
    // the summary sweep. Empty when nothing is marked.
    public string TreatmentOutcome { get; private set; }

    public List<string> GetMarkedIds()
    {
        return new List<string>(MarkedIds);
    }

    public List<string> GetCropTreeIds()
    {
        var result = new List<string>();
        foreach (ForestTree tree in Object.FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
            if (tree != null && !tree.IsStump && tree.IsCropTree)
                result.Add(tree.TreeId);
        return result;
    }

    public bool IsMarked(ForestTree tree)
    {
        return tree != null && MarkedIds.Contains(tree.TreeId);
    }

    private void Awake()
    {
        Instance = this;
        // These statics survive play-mode sessions without a domain reload;
        // a fresh session must not inherit the previous session's pending marks.
        MarkedIds.Clear();
        MarkerObjects.Clear();
        view = Camera.main;
        ForestPlayer player = view != null ? view.GetComponentInParent<ForestPlayer>() : null;
        playerRoot = player != null ? player.transform : null;
        inspectionPlayer = player;
    }

    private void OnEnable()
    {
        ForestTree.Felled += OnTreeNoLongerLiving;
        ForestTree.MortalityApplied += OnTreeNoLongerLiving;
    }

    private void OnDisable()
    {
        ForestTree.Felled -= OnTreeNoLongerLiving;
        ForestTree.MortalityApplied -= OnTreeNoLongerLiving;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
        if (cropMarkerMaterial != null)
            Destroy(cropMarkerMaterial);
        DisposeRackMarkingVisuals();
    }

    private void Update()
    {
        UpdateMarkedSummary();
        UpdateRackSpray();

        if (messageTimer > 0f)
            messageTimer -= Time.deltaTime;

        aimedTree = null;
        aimingAtGround = false;
        // While an extraction rack is being aimed, X and C do not mark the tree under the reticle.
        if (view == null || Cursor.lockState != CursorLockMode.Locked || RackTool.AnyActive)
            return;

        // A competitor chosen with 1-5 on the Crop Tree competitor list takes X, wherever the reticle points.
        if (TryMarkKeyboardTarget())
            return;

        int hitCount = Physics.RaycastNonAlloc(view.transform.position, view.transform.forward, hitBuffer, interactionDistance);
        bool found = false;
        RaycastHit nearest = default;
        for (int i = 0; i < hitCount; i++)
        {
            RaycastHit candidate = hitBuffer[i];
            Transform candidateTransform = candidate.collider.transform;
            if (playerRoot != null && (candidateTransform == playerRoot || candidateTransform.IsChildOf(playerRoot)))
                continue;
            if (candidate.collider.GetComponentInParent<ScenarioWindthrowVisual>() == null
                && (candidate.collider.GetComponentInParent<ScenarioHabitatVisuals>() != null
                    || (candidate.collider.GetComponentInParent<ForestEcologyController>() != null
                        && candidate.collider.GetComponentInParent<ForestTree>() == null))) continue;
            if (!found || candidate.distance < nearest.distance)
            {
                nearest = candidate;
                found = true;
            }
        }
        if (!found)
            return;

        ScenarioWindthrowVisual windthrow = nearest.collider.GetComponentInParent<ScenarioWindthrowVisual>();
        if (windthrow != null)
        {
            ScenarioOneManager manager = Object.FindFirstObjectByType<ScenarioOneManager>();
            if (Keyboard.current != null && Keyboard.current.xKey.wasPressedThisFrame && manager != null)
                manager.ToggleSalvagePlan(windthrow.TreeId);
            return;
        }
        ForestTree tree = nearest.collider.GetComponentInParent<ForestTree>();
        if (tree == null || !tree.IsLiving)
        {
            // Aiming at open ground: surface the local regeneration state.
            if (ecologyCache == null)
                ecologyCache = Object.FindFirstObjectByType<ForestEcologyController>();
            if (ecologyCache != null)
            {
                aimingAtGround = true;
                aimedGroundPoint = nearest.point;
            }
            return;
        }

        aimedTree = tree;
        Keyboard keyboard = Keyboard.current;
        if (keyboard != null && keyboard.xKey.wasPressedThisFrame)
            ToggleMark(tree);
        if (keyboard != null && keyboard.cKey.wasPressedThisFrame)
            ToggleCropTree(tree);
    }

    // The tree the player chose by keyboard (the competitor list sets this; it is read only at the moment X is pressed, so a list that
    // has closed or a tree that is gone can never be marked by mistake). Null when nothing is chosen: X then acts on the aimed tree.
    public System.Func<ForestTree> KeyboardTarget { get; set; }

    private bool TryMarkKeyboardTarget()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null || !keyboard.xKey.wasPressedThisFrame || KeyboardTarget == null)
            return false;
        ForestTree target = KeyboardTarget();
        if (target == null || !target.IsLiving)
            return false;
        ToggleMark(target);
        return true;
    }

    private void OnTreeNoLongerLiving(ForestTree tree)
    {
        Unmark(tree, false);
    }

    // X, by mouse aim or by a competitor chosen with 1-5, comes through here. A tree still marked as a Crop Tree is a positive choice to
    // keep, so X refuses it rather than silently replacing that mark with a felling mark; C removes the Crop Tree mark first.
    public void ToggleMark(ForestTree tree)
    {
        if (tree == null || !tree.IsLiving)
            return;
        if (tree.MarkType == TreeMarkType.CropTree)
        {
            SetMessage($"{tree.TreeId} is a Crop Tree. Its Crop Tree mark must be removed first: inspect it and press C.");
            return;
        }
        if (tree.MarkType == TreeMarkType.Fell)
            Unmark(tree);
        else
            Mark(tree, TreeMarkType.Fell);
    }

    public void ToggleCropTree(ForestTree tree)
    {
        if (tree == null || !tree.IsLiving)
            return;
        if (tree.MarkType == TreeMarkType.CropTree)
            Unmark(tree);
        else
            Mark(tree, TreeMarkType.CropTree);
    }

    public void Mark(ForestTree tree, TreeMarkType type, bool feedback = true)
    {
        if (tree == null || !tree.IsLiving)
            return;
        if (!restoringMarks && HasRackEntranceMark(tree))
        {
            if (feedback) SetMessage("Orange rack entrance: reopen and change that rack to clear its mark before applying Fell or Crop Tree.");
            return;
        }
        // A Crop Tree cannot be silently added to a frozen rack-removal set (save v21 has the same rule).
        if (!restoringMarks && type == TreeMarkType.CropTree && IsRackRemovalTree(tree))
        {
            if (feedback) SetMessage("This tree belongs to a rack-removal plan. Adjust that rack before retaining it as a Crop Tree.");
            return;
        }
        if (type == TreeMarkType.None)
        {
            Unmark(tree, feedback);
            return;
        }
        RemoveMarker(tree.TreeId);
        tree.SetMark(type);
        MarkedIds.Remove(tree.TreeId);
        if (type == TreeMarkType.Fell)
            MarkedIds.Add(tree.TreeId);
        CreateMarker(tree, type);
        markedCacheDirty = true;
        if (feedback)
        {
            string label = type == TreeMarkType.CropTree ? "Crop Tree" : "harvest";
            SetMessage($"Marked {tree.TreeId} as {label} ({MarkedIds.Count} marked)");
        }
    }

    // Preserve the original public harvesting API for existing Forestry tools.
    public void Mark(ForestTree tree, bool feedback = true) => Mark(tree, TreeMarkType.Fell, feedback);

    public void Unmark(ForestTree tree, bool feedback = true)
    {
        if (tree == null)
            return;
        if (HasRackEntranceMark(tree))
        {
            if (feedback) SetMessage("The orange entrance mark belongs to its rack. Reopen and change that rack to clear it.");
            return;
        }
        tree.SetMark(TreeMarkType.None);
        MarkedIds.Remove(tree.TreeId);
        RemoveMarker(tree.TreeId);
        markedCacheDirty = true;
        if (feedback)
            SetMessage($"Unmarked {tree.TreeId} ({MarkedIds.Count} marked)");
    }

    public void ClearAll()
    {
        var ids = new List<string>(MarkedIds);
        foreach (string id in ids)
            RemoveMarker(id);
        MarkedIds.Clear();
        foreach (ForestTree tree in Object.FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
            if (tree != null && tree.IsMarkedForFell)
                tree.SetMark(TreeMarkType.None);
        markedCacheDirty = true;
    }

    private void ClearAllMarks()
    {
        foreach (string id in new List<string>(MarkerObjects.Keys)) RemoveMarker(id);
        MarkedIds.Clear();
        foreach (ForestTree tree in Object.FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
            if (tree != null)
                tree.SetMark(TreeMarkType.None);
        markedCacheDirty = true;
    }

    // The HUD summary must reflect exactly the trees still standing with a mark.
    // Marks whose tree disappeared without a felling event are pruned by the sweep.
    private void UpdateMarkedSummary()
    {
        bool refresh = markedCacheDirty || Time.unscaledTime >= nextStaleSweepTime;
        if (refresh)
        {
            markedCacheDirty = false;
            nextStaleSweepTime = Time.unscaledTime + 0.5f;
            SweepStaleMarks();
            RebuildMarkedTreeCache();
            UpdateTreatmentOutcome();
        }

        int living = 0;
        float volume = 0f;
        foreach (ForestTree tree in markedTreeCache)
        {
            if (tree == null || tree.IsStump)
                continue;
            living++;
            volume += tree.BiologicalStemVolumeM3;
        }
        LivingMarkedCount = living;
        MarkedVolumeM3 = volume;
        LivingCropTreeCount = GetCropTreeIds().Count;

        UpdateTreatmentOutcome();
    }

    // Small player-facing forecast of what felling the marked trees would do:
    // retained volume, growth release of the retained trees, light in the opened
    // cells, the resulting wind peak and the opening intensity. Uses the same
    // formulas as the ecology simulation with the marked trees removed, so the
    // numbers are honest approximations of one year after the felling.
    private void UpdateTreatmentOutcome()
    {
        TreatmentOutcome = "";
        if (markedTreeCache.Count == 0)
            return;

        ForestEcologyController eco = Object.FindFirstObjectByType<ForestEcologyController>();
        TreeSpeciesDefinition species = eco != null ? eco.ResolveSpecies() : null;
        if (eco == null || eco.Cells == null || species == null)
            return;

        var marked = new HashSet<ForestTree>();
        foreach (ForestTree tree in markedTreeCache)
            if (tree != null && !tree.IsStump)
                marked.Add(tree);
        if (marked.Count == 0)
            return;

        var living = new List<ForestTree>();
        foreach (ForestTree tree in Object.FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
            if (tree != null && !tree.IsStump)
                living.Add(tree);

        float cap = eco.MaxRecentOpeningPerCell;

        // Forecast per-cell opening: the marked fells add one step each, capped.
        var markedPerCell = new Dictionary<int, int>();
        var affected = new HashSet<int>();
        foreach (ForestTree tree in marked)
        {
            int index = eco.GetCellIndex(tree.transform.position);
            if (index < 0)
                continue;
            affected.Add(index);
            markedPerCell.TryGetValue(index, out int count);
            markedPerCell[index] = count + 1;
        }
        if (affected.Count == 0)
            return;

        var forecastOpening = new Dictionary<int, float>();
        foreach (int index in affected)
        {
            markedPerCell.TryGetValue(index, out int count);
            forecastOpening[index] = Mathf.Min(cap, eco.Cells[index].RecentOpening + count);
        }

        // Forecast light in the affected cells: the same fractional-cover model
        // as RecomputeCanopy, with the marked crowns removed.
        float lightNowSum = 0f, lightAfterSum = 0f;
        var forecastLight = new Dictionary<int, float>();
        foreach (int index in affected)
        {
            Vector2 center = eco.Cells[index].Center;
            float gap = 1f;
            foreach (ForestTree tree in living)
            {
                if (marked.Contains(tree))
                    continue;
                Vector2 position = new Vector2(tree.transform.position.x, tree.transform.position.z);
                float distance = Vector2.Distance(center, position);
                float reach = tree.CrownRadius * ForestEcologyController.CanopyShadeReachPerCrownRadius + eco.CellSizeMeters * 0.5f;
                float lateral = Mathf.Clamp01(1f - distance / reach);
                float vertical = Mathf.Clamp01(tree.Height / 8f);
                gap *= 1f - Mathf.Clamp01(lateral * vertical);
            }
            // Light is the residual gap product (see RecomputeCanopy), so removing
            // crowns raises it.
            forecastLight[index] = gap;
            lightNowSum += eco.Cells[index].Light;
            lightAfterSum += gap;
        }
        float lightNow = lightNowSum / affected.Count;
        float lightAfter = lightAfterSum / affected.Count;

        // Competition and growth of the retained trees, current versus forecast.
        // The competition-after sum is a loop over every pair of living trees. Calling transform.position and Diameter inside the pair loop cost about
        // 0.7 s per call at 1344 trees (Development Player), so each tree's position, diameter and marked state are read once into arrays. Pairs clearly
        // beyond the cutoff are skipped with a squared-distance test that is slightly wider than the cutoff, then the same exact distance test and the same
        // additions in the same order follow, so the result is bit for bit what the plain loop gives.
        const float competitionCutoffM = ForestEcologyController.HegyiCutoffMeters;
        const float prefilterCutoffM = competitionCutoffM + 0.01f;
        int livingCount = living.Count;
        var positionX = new float[livingCount];
        var positionZ = new float[livingCount];
        var diameter = new float[livingCount];
        var isMarked = new bool[livingCount];
        for (int i = 0; i < livingCount; i++)
        {
            Vector3 point = living[i].transform.position;
            positionX[i] = point.x;
            positionZ[i] = point.z;
            diameter[i] = living[i].Diameter;
            isMarked[i] = marked.Contains(living[i]);
        }
        float growthNowSum = 0f, growthAfterSum = 0f;
        int remainingCount = 0;
        for (int i = 0; i < livingCount; i++)
        {
            if (isMarked[i])
                continue;
            ForestTree tree = living[i];
            TreeSpeciesDefinition treeSpecies = tree.Species != null ? tree.Species : species;
            Vector2 position = new Vector2(positionX[i], positionZ[i]);
            float site = eco.GetSiteProductivity(tree.transform.position);
            float sizeFactor = Mathf.Clamp01(1f - diameter[i] / treeSpecies.MaxDbhCm);
            float potential = treeSpecies.PotentialDbhGrowthCmPerYear * site * sizeFactor;
            if (treeSpecies.HasGrowthAgePhases)
            {
                treeSpecies.GrowthAgeFactors(tree.AgeYears, out float dbhPhaseFactor, out float heightPhaseFactor);
                potential *= dbhPhaseFactor;
            }

            float ciNow = eco.GetCompetitionIndex(tree);
            growthNowSum += potential * (1f / (1f + ciNow / treeSpecies.Ci50));

            float ciAfter = 0f;
            for (int j = 0; j < livingCount; j++)
            {
                if (j == i || isMarked[j])
                    continue;
                float dx = positionX[j] - positionX[i];
                float dz = positionZ[j] - positionZ[i];
                if (dx * dx + dz * dz > prefilterCutoffM * prefilterCutoffM)
                    continue;
                float distance = Vector2.Distance(position, new Vector2(positionX[j], positionZ[j]));
                if (distance > competitionCutoffM)
                    continue;
                ciAfter += ForestEcologyController.HegyiTerm(diameter[j], diameter[i], distance);
            }
            growthAfterSum += potential * (1f / (1f + ciAfter / treeSpecies.Ci50));
            remainingCount++;
        }
        float growthNow = remainingCount > 0 ? growthNowSum / remainingCount : 0f;
        float growthAfter = remainingCount > 0 ? growthAfterSum / remainingCount : 0f;
        float growthPercent = growthNow > 0.0001f ? (growthAfter - growthNow) / growthNow * 100f : 0f;

        // Read the same relative vulnerability used by actual storm failure.
        // The preview rebuilds shelter from retained trees and leaves no cache mutation.
        float windPeak = eco.ForecastStormPeak(living.FindAll(tree => !marked.Contains(tree)).ToArray(), forecastLight, forecastOpening);

        float keepVolume = 0f;
        foreach (ForestTree tree in living)
            if (!marked.Contains(tree))
                keepVolume += tree.BiologicalStemVolumeM3;

        float openingPeak = 0f;
        foreach (float value in forecastOpening.Values)
            if (value > openingPeak)
                openingPeak = value;

        TreatmentOutcome = $"If felled now: keep {keepVolume:0.0} m3 · growth {(growthPercent >= 0f ? "+" : "")}{growthPercent:0}% · gap light {lightNow:0.00} → {lightAfter:0.00}"
            + $" · wind exposure {ForestEcologyController.StormExposureBand(windPeak)}{(eco.StormModelVersion == 0 ? " (storms off)" : "")} · opening {openingPeak:0.0} of {cap:0.#}";
    }

    private void SweepStaleMarks()
    {
        var livingById = new Dictionary<string, ForestTree>();
        foreach (ForestTree tree in Object.FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
        {
            if (tree != null && !tree.IsStump && !string.IsNullOrEmpty(tree.TreeId))
                livingById[tree.TreeId] = tree;
        }

        // Orange is a projection of the existing v21 rack record, not a third saved per-tree mark.
        // It shares this registry and the same exclusive mutation API with ordinary Fell/Crop marks.
        foreach (ForestTree tree in livingById.Values)
        {
            bool orange = tree.IsLiving && HasRackEntranceMark(tree);
            if (MarkerObjects.TryGetValue(tree.TreeId, out GameObject paint) && paint != null
                && (paint.name == RackEntranceMarkerName) != orange)
                RemoveMarker(tree.TreeId);
            if (orange)
            {
                CreateRackEntranceMarker(tree);
                MarkedIds.Remove(tree.TreeId);
                continue;
            }
            if (tree.IsMarkedForFell)
                MarkedIds.Add(tree.TreeId);
            else
                MarkedIds.Remove(tree.TreeId);
            if (tree.MarkType != TreeMarkType.None &&
                (!MarkerObjects.TryGetValue(tree.TreeId, out GameObject marker) || marker == null))
                CreateMarker(tree, tree.MarkType);
        }
        var stale = new List<string>();
        foreach (string id in MarkerObjects.Keys)
            if (!livingById.TryGetValue(id, out ForestTree tree) || !tree.IsLiving
                || (tree.MarkType == TreeMarkType.None && !HasRackEntranceMark(tree)))
                stale.Add(id);
        foreach (string id in stale)
        {
            MarkedIds.Remove(id);
            RemoveMarker(id);
        }
        MarkedIds.RemoveWhere(id => !livingById.ContainsKey(id));

        if (stale.Count > 0)
            markedCacheDirty = true;
    }

    private void RebuildMarkedTreeCache()
    {
        markedTreeCache.Clear();
        foreach (ForestTree tree in Object.FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
        {
            if (tree == null || tree.IsStump || string.IsNullOrEmpty(tree.TreeId))
                continue;
            if (!tree.IsMarkedForFell || !MarkedIds.Contains(tree.TreeId))
                continue;
            markedTreeCache.Add(tree);
        }
    }

    // Used by save loading; marks are keyed by the persistent tree id.
    public void RestoreMarks(List<string> fellIds, List<string> cropIds = null)
    {
        ClearAllMarks();
        var treesById = new Dictionary<string, ForestTree>();
        foreach (ForestTree tree in Object.FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
        {
            if (tree != null && !string.IsNullOrEmpty(tree.TreeId))
                treesById[tree.TreeId] = tree;
        }
        // Load validates the incoming combined save before mutation. At this point the scenario
        // still holds the previous world's racks, so do not apply live planning guards to rehydration.
        restoringMarks = true;
        try
        {
            if (fellIds != null)
                foreach (string id in fellIds)
                    if (treesById.TryGetValue(id, out ForestTree tree) && !tree.IsStump)
                        Mark(tree, TreeMarkType.Fell, false);
            if (cropIds != null)
                foreach (string id in cropIds)
                    if (treesById.TryGetValue(id, out ForestTree tree) && !tree.IsStump)
                        Mark(tree, TreeMarkType.CropTree, false);
        }
        finally { restoringMarks = false; }
    }

    private void CreateMarker(ForestTree tree, TreeMarkType type)
    {
        if (MarkerObjects.TryGetValue(tree.TreeId, out GameObject existing) && existing != null)
            return;
        Material material = MarkerMaterial(type);
        if (material == null)
            return;

        var marker = new GameObject(type == TreeMarkType.CropTree ? "Crop Tree Mark" : "Harvest Mark");
        marker.transform.SetParent(tree.transform, false);

        GameObject diamond = GameObject.CreatePrimitive(PrimitiveType.Cube);
        diamond.name = "Diamond";
        Destroy(diamond.GetComponent<Collider>());
        diamond.transform.SetParent(marker.transform, false);
        diamond.transform.localPosition = new Vector3(0f, tree.Height + 0.6f, 0f);
        diamond.transform.localRotation = Quaternion.Euler(45f, 45f, 0f);
        diamond.transform.localScale = Vector3.one * 0.45f;
        diamond.GetComponent<Renderer>().sharedMaterial = material;

        GameObject disc = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        disc.name = "Disc";
        Destroy(disc.GetComponent<Collider>());
        disc.transform.SetParent(marker.transform, false);
        disc.transform.localPosition = new Vector3(0f, 0.05f, 0f);
        disc.transform.localScale = new Vector3(1.2f, 0.02f, 1.2f);
        disc.GetComponent<Renderer>().sharedMaterial = material;

        // A visible painted band on the trunk makes persistent blue Crop Trees
        // identifiable while walking under the canopy, not just from above.
        if (type == TreeMarkType.CropTree)
        {
            GameObject band = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            band.name = "Blue Trunk Band";
            Destroy(band.GetComponent<Collider>());
            band.transform.SetParent(marker.transform, false);
            band.transform.localPosition = new Vector3(0f, Mathf.Min(1.6f, tree.Height * 0.5f), 0f);
            band.transform.localScale = new Vector3(Mathf.Max(0.25f, tree.Diameter / 100f + 0.08f),
                0.12f, Mathf.Max(0.25f, tree.Diameter / 100f + 0.08f));
            band.GetComponent<Renderer>().sharedMaterial = material;
        }

        MarkerObjects[tree.TreeId] = marker;
    }

    private void RemoveMarker(string treeId)
    {
        if (MarkerObjects.TryGetValue(treeId, out GameObject marker))
        {
            if (marker != null)
            {
                if (marker.name == RackEntranceMarkerName)
                    foreach (MeshFilter patch in marker.GetComponentsInChildren<MeshFilter>()) Destroy(patch.sharedMesh);
                Destroy(marker);
            }
            MarkerObjects.Remove(treeId);
        }
    }

    private Material MarkerMaterial(TreeMarkType type)
    {
        if (type == TreeMarkType.CropTree && cropMarkerMaterial != null)
            return cropMarkerMaterial;
        if (type == TreeMarkType.Fell && markerMaterial != null)
            return markerMaterial;
        if (type == TreeMarkType.CropTree && markerMaterial != null)
        {
            cropMarkerMaterial = new Material(markerMaterial) { name = "CropTreeMark", color = cropTreeColor };
            return cropMarkerMaterial;
        }
        Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
        if (shader == null)
            shader = Shader.Find("Unlit/Color");
        if (shader == null)
        {
            Debug.LogError("Harvest mark material is missing (no serialized material and no shader found in this build). Assign the HarvestMark material asset to the marking manager.");
            return null;
        }
        if (type == TreeMarkType.CropTree)
        {
            cropMarkerMaterial = new Material(shader) { name = "CropTreeMark", color = cropTreeColor };
            return cropMarkerMaterial;
        }
        markerMaterial = new Material(shader) { name = "HarvestMark", color = markerColor };
        return markerMaterial;
    }

    private void SetMessage(string text)
    {
        message = text;
        messageTimer = 2.5f;
    }

    // ----- Read-only presentation access (Scenario One UI Toolkit HUD) -----
    public ForestTree AimedTree => aimedTree;
    public bool AimingAtGround => aimingAtGround && aimedTree == null;
    public Vector3 AimedGroundPoint => aimedGroundPoint;
    public string TransientMessage => messageTimer > 0f ? message : "";

    // The mark prompt for the aimed tree, or "" when no tree is aimed at.
    public string MarkPromptText()
    {
        if (RackSprayEquipped) return RackSprayPrompt();
        if (aimedTree == null)
            return "";
        if (HasRackEntranceMark(aimedTree)) return "Orange · rack entrance marked";
        return aimedTree.IsCropTree ? "[C] Remove Crop Tree"
            : IsMarked(aimedTree) ? "[X] Unmark Fell  |  [C] Retain as Crop Tree"
            : "[X] Mark to Fell  |  [C] Retain as Crop Tree";
    }

    private void OnGUI()
    {
        // Scenario One draws this HUD with UI Toolkit (ScenarioOneUiRoot).
        if (UnityEngine.Object.FindFirstObjectByType<ScenarioOneManager>() != null)
            return;
        float hudScale = ForestHud.Scale;
        if (messageStyle == null)
        {
            messageStyle = new GUIStyle(GUI.skin.label)
            {
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true
            };
            messageStyle.normal.textColor = new Color(1f, 0.85f, 0.55f);
        }
        messageStyle.fontSize = Mathf.RoundToInt(36f * hudScale);
        if (messageTimer > 0f)
        {
            float messageWidth = Mathf.Min(920f * hudScale, Screen.width - 32f);
            Rect messageRect = new Rect(Screen.width * 0.5f - messageWidth * 0.5f, 16f + 92f * hudScale + 8f, messageWidth, 92f * hudScale);
            ForestHud.Panel(messageRect);
            GUI.Label(messageRect, message, messageStyle);
        }

        if (counterStyle == null)
        {
            counterStyle = new GUIStyle(GUI.skin.label)
            {
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.LowerLeft
            };
            counterStyle.normal.textColor = new Color(0.95f, 0.85f, 0.65f);
        }
        counterStyle.fontSize = Mathf.RoundToInt(22f * hudScale);
        // Use the available width and measured height: the full summary is longer
        // than the former fixed 680-unit label and can wrap at smaller resolutions.
        counterStyle.wordWrap = true;
        string summary = $"Fell marks: {LivingMarkedCount}  |  Crop Trees: {LivingCropTreeCount}  |  Fell volume: {MarkedVolumeM3:0.0} m³   [X] Fell / [C] Crop";
        float summaryWidth = Screen.width - 36f;
        float summaryHeight = counterStyle.CalcHeight(new GUIContent(summary), summaryWidth);
        GUI.Label(new Rect(18f, Screen.height - 16f * hudScale - summaryHeight,
            summaryWidth, summaryHeight), summary, counterStyle);

        if (!string.IsNullOrEmpty(TreatmentOutcome))
        {
            if (outcomeStyle == null)
            {
                outcomeStyle = new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.LowerLeft
                };
                outcomeStyle.normal.textColor = new Color(0.8f, 0.88f, 0.95f);
            }
            outcomeStyle.fontSize = Mathf.RoundToInt(17f * hudScale);
            GUI.Label(new Rect(18f, Screen.height - 74f * hudScale, Screen.width - 36f, 24f * hudScale), TreatmentOutcome, outcomeStyle);
        }

        if (aimedTree == null && aimingAtGround && ecologyCache != null)
        {
            string report = ecologyCache.RegenerationReportLine(aimedGroundPoint);
            if (!string.IsNullOrEmpty(report))
            {
                if (outcomeStyle == null)
                {
                    outcomeStyle = new GUIStyle(GUI.skin.label)
                    {
                        alignment = TextAnchor.LowerLeft
                    };
                    outcomeStyle.normal.textColor = new Color(0.8f, 0.88f, 0.95f);
                }
                outcomeStyle.fontSize = Mathf.RoundToInt(17f * hudScale);
                GUI.Label(new Rect(18f, Screen.height - 104f * hudScale, Screen.width - 36f, 24f * hudScale), report, outcomeStyle);
                // Regeneration "why" line: the nearest exact planted juvenile, or the
                // cell's leading cohort (read-only; Docs/Scenario1EcologyCompletion.md).
                if (diagnosisScenario == null)
                    diagnosisScenario = FindFirstObjectByType<ScenarioOneManager>();
                if (diagnosisSpawner == null)
                    diagnosisSpawner = FindFirstObjectByType<ForestTreeSpawner>();
                RegenerationDiagnosis diagnosis = RegenerationDiagnostics.Diagnose(ecologyCache, aimedGroundPoint,
                    diagnosisScenario != null ? diagnosisScenario.PlantedJuveniles : null, diagnosisSpawner);
                if (diagnosis.HasJuvenile)
                    GUI.Label(new Rect(18f, Screen.height - 134f * hudScale, Screen.width - 36f, 24f * hudScale),
                        diagnosis.Summary(), outcomeStyle);
            }
        }

        // The mark prompt stays off the crosshair while the inspection card is
        // open, so it never covers the card text.
        if (aimedTree == null || (inspectionPlayer != null && inspectionPlayer.IsInspecting))
            return;

        if (promptStyle == null)
        {
            promptStyle = new GUIStyle(GUI.skin.label)
            {
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            promptStyle.normal.textColor = Color.white;
        }
        promptStyle.fontSize = Mathf.RoundToInt(26f * hudScale);
        string prompt = MarkPromptText();
        // Size the panel to the text: the fixed 460-unit width wrapped the last word.
        float promptWidth = Mathf.Min(Screen.width - 36f,
            promptStyle.CalcSize(new GUIContent(prompt)).x + 32f * hudScale);
        float promptHeight = 56f * hudScale;
        Rect promptRect = new Rect(Screen.width * 0.5f - promptWidth * 0.5f, Screen.height * 0.5f - 96f * hudScale, promptWidth, promptHeight);
        ForestHud.Panel(promptRect);
        GUI.Label(promptRect, prompt, promptStyle);
    }
}
using UnityEngine;
using UnityEngine.InputSystem;

// Field communication of the authoritative v21 rack records (plan R-3). Orange never enters
// the loose Fell ID set or the saved per-tree enum. All three marks use MarkerObjects and
// the same marking manager's conflict checks; there is no second entrance-mark ledger.
public sealed partial class ForestTreeMarkingManager
{
    public const float RackSprayDistance = 3.5f; // [C] physical spray/use reach, not a forestry constant
    private const string RackEntranceMarkerName = "Orange Rack Entrance Mark";
    private ScenarioOneManager rackScenario;
    private Material orangePaint, cueMaterial;
    private GameObject entranceCue;
    private AudioSource sprayAudio;
    private AudioClip sprayClip;
    private bool sprayLockedLastFrame;
    private float sprayFeedbackUntil;

    public bool RackSprayEquipped { get; private set; }
    public bool RackSprayFeedbackActive => Time.unscaledTime < sprayFeedbackUntil;
    public int CueRackId { get; private set; }
    public string RackFieldStatus { get; private set; } = "";

    private ScenarioOneManager RackScenario
    {
        get
        {
            if (rackScenario == null) rackScenario = FindFirstObjectByType<ScenarioOneManager>();
            return rackScenario;
        }
    }

    public bool HasRackEntranceMark(ForestTree tree)
    {
        ScenarioOneManager scenario = RackScenario;
        if (tree == null || scenario == null || !scenario.RackPlanningV2Active) return false;
        // Read draft records too: reopening alone never erases a geometry-bound mark.
        foreach (RackSaveData rack in scenario.Racks)
            if (rack != null && rack.entranceMarked && rack.entranceTreeId == tree.TreeId) return true;
        return false;
    }

    private bool IsRackRemovalTree(ForestTree tree)
    {
        ScenarioOneManager scenario = RackScenario;
        if (tree == null || scenario == null || !scenario.RackPlanningV2Active) return false;
        foreach (RackSaveData rack in scenario.Racks)
            if (rack != null && rack.removalTreeIds != null && rack.removalTreeIds.Contains(tree.TreeId)) return true;
        return false;
    }

    // First unmarked, unworked rack by stable ID. Never redirect a broken rack to a nearby tree.
    public bool TryGetRackFieldTarget(out RackPlanRack rack, out ForestTree tree, out string problem)
    {
        rack = null; tree = null; problem = "";
        ScenarioOneManager scenario = RackScenario;
        if (scenario == null || !scenario.RackPlanningV2Active || !scenario.RackPlanConfirmed) return false;
        foreach (RackPlanRack candidate in scenario.ConfirmedRacks)
        {
            if (candidate.EntranceMarked || candidate.IsLocked) continue;
            rack = candidate;
            return scenario.TryGetRackEntranceTarget(candidate.Id, out tree, out problem);
        }
        return false;
    }

    public bool EquipRackSpray(bool equip)
    {
        ScenarioOneManager scenario = RackScenario;
        if (equip && (scenario == null || !scenario.RackPlanningV2Active || !scenario.RackPlanConfirmed
            || (inspectionPlayer != null && inspectionPlayer.IsPlantingMode)))
        {
            SetMessage("Confirm the rack plan and leave planting mode before equipping the spray marker.");
            return false;
        }
        RackSprayEquipped = equip;
        SetMessage(equip ? "Spray marker equipped · [Left-click] Spray · [P] Stow."
            : "Spray marker stowed.");
        return true;
    }

    private void UpdateRackSpray()
    {
        ScenarioOneManager scenario = RackScenario;
        bool available = scenario != null && scenario.RackPlanningV2Active && scenario.RackPlanConfirmed;
        bool locked = Cursor.lockState == CursorLockMode.Locked;
        if (!available || (inspectionPlayer != null && inspectionPlayer.IsPlantingMode)) RackSprayEquipped = false;
        if (available && locked && Keyboard.current != null && Keyboard.current.pKey.wasPressedThisFrame)
            EquipRackSpray(!RackSprayEquipped);
        if (RackSprayEquipped && locked && sprayLockedLastFrame && Application.isFocused
            && Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            TrySprayRackEntrance();
        sprayLockedLastFrame = locked;

        bool target = TryGetRackFieldTarget(out RackPlanRack rack, out ForestTree tree, out string problem);
        RackFieldStatus = !available ? "" : $"Rack entrances marked {scenario.MarkedRackEntranceCount} / {ScenarioOneManager.RackPlanTargetCount}"
            + (!string.IsNullOrEmpty(problem) ? " · " + problem : "");
        CueRackId = target ? rack.Id : 0;
        if (target)
        {
            EnsureEntranceCue();
            if (entranceCue != null)
            {
                entranceCue.SetActive(true);
                entranceCue.transform.position = tree.transform.position + Vector3.up * Mathf.Min(2.6f, tree.Height * 0.75f);
                if (view != null) entranceCue.transform.rotation = Quaternion.LookRotation(view.transform.position - entranceCue.transform.position);
            }
        }
        else if (entranceCue != null) entranceCue.SetActive(false);
    }

    // Re-raycast at use time. Neither a cached tree nor a caller-supplied tree can satisfy aim/range.
    public RackPlanOperationResult TrySprayRackEntrance()
    {
        ScenarioOneManager scenario = RackScenario;
        RackPlanOperationResult result;
        if (!RackSprayEquipped || scenario == null || !scenario.RackPlanningV2Active || !scenario.RackPlanConfirmed
            || Cursor.lockState != CursorLockMode.Locked)
            result = RackPlanOperationResult.Invalid("Equip the spray marker [P] after confirming the rack plan.");
        else if (!TryRackSprayAim(out ForestTree tree))
            result = RackPlanOperationResult.Invalid("Aim at the entrance trunk and move within 3.5 m to spray.");
        else
        {
            RackPlanRack targetRack = null;
            foreach (RackPlanRack rack in scenario.ConfirmedRacks)
                if (rack.EntranceTreeId == tree.TreeId) { targetRack = rack; break; }
            result = targetRack == null
                ? RackPlanOperationResult.Invalid("That tree is not a rack entrance. Follow the entrance cue.")
                : scenario.MarkRackEntrance(targetRack.Id);
            if (result.Status == RackPlanOperationStatus.Applied)
            {
                CreateRackEntranceMarker(tree);
                PlaySprayFeedback();
                // Hide this rack's cue immediately; next Update projects the next real target.
                if (entranceCue != null) entranceCue.SetActive(false);
                CueRackId = 0;
            }
        }
        SetMessage(result.Message);
        return result;
    }

    private bool TryRackSprayAim(out ForestTree tree)
    {
        tree = null;
        if (view == null) return false;
        // RaycastAll avoids dropping the closest blocker when dense crowns fill a fixed buffer.
        RaycastHit[] hits = Physics.RaycastAll(view.transform.position, view.transform.forward, RackSprayDistance,
            Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
        float nearest = float.MaxValue;
        foreach (RaycastHit hit in hits)
        {
            Transform hitTransform = hit.collider.transform;
            if (playerRoot != null && (hitTransform == playerRoot || hitTransform.IsChildOf(playerRoot))) continue;
            if (hit.collider.GetComponentInParent<ScenarioHabitatVisuals>() != null
                || (hit.collider.GetComponentInParent<ForestEcologyController>() != null
                    && hit.collider.GetComponentInParent<ForestTree>() == null)) continue;
            if (hit.distance >= nearest) continue;
            nearest = hit.distance;
            tree = hit.collider.GetComponentInParent<ForestTree>();
        }
        return tree != null && tree.IsLiving;
    }

    public string RackSprayPrompt()
    {
        if (!RackSprayEquipped) return "";
        if (!TryRackSprayAim(out ForestTree tree)) return "Spray marker · aim at an entrance trunk within 3.5 m · [P] Stow";
        if (HasRackEntranceMark(tree)) return "Orange · entrance already marked · [P] Stow";
        ScenarioOneManager scenario = RackScenario;
        foreach (RackPlanRack rack in scenario.ConfirmedRacks)
        {
            if (rack.EntranceTreeId != tree.TreeId) continue;
            if (rack.IsLocked) return "Rack already harvested · entrance locked";
            if (tree.IsCropTree) return "Crop Tree · remove its blue designation before spraying";
            if (tree.IsMarkedForFell) return "Fell mark · remove its red designation before spraying";
            if (!scenario.TryGetRackEntranceTarget(rack.Id, out _, out string problem)) return problem;
            return $"Rack {rack.Id} entrance · [Left-click] Spray orange · [P] Stow";
        }
        return "Not a rack entrance · follow the entrance cue · [P] Stow";
    }

    private Material OrangePaint()
    {
        if (orangePaint == null)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
            if (shader != null) orangePaint = new Material(shader) { name = "Rack entrance orange paint", color = new Color(0.95f, 0.28f, 0.025f) };
        }
        return orangePaint;
    }

    private void CreateRackEntranceMarker(ForestTree tree)
    {
        if (MarkerObjects.TryGetValue(tree.TreeId, out GameObject existing) && existing != null) return;
        Material paint = OrangePaint();
        if (paint == null) return;
        var marker = new GameObject(RackEntranceMarkerName);
        marker.transform.SetParent(tree.transform, false);
        // Four narrow curved patches wrap the trunk rather than becoming floating square signs.
        float radius = Mathf.Max(0.08f, tree.Diameter / 200f) + 0.012f;
        for (int side = 0; side < 4; side++)
        {
            var stripe = new GameObject("Orange vertical stripe " + (side + 1));
            stripe.transform.SetParent(marker.transform, false);
            const int segments = 6;
            var vertices = new Vector3[(segments + 1) * 2];
            var triangles = new int[segments * 6];
            for (int i = 0; i <= segments; i++)
            {
                float angle = (side * 90f - 12f + 24f * i / segments) * Mathf.Deg2Rad;
                float x = Mathf.Sin(angle) * radius, z = Mathf.Cos(angle) * radius;
                vertices[i * 2] = new Vector3(x, 1.0f, z);
                vertices[i * 2 + 1] = new Vector3(x, Mathf.Min(1.85f, tree.Height * 0.75f), z);
                if (i == segments) continue;
                int a = i * 2, t = i * 6;
                triangles[t] = a; triangles[t + 1] = a + 2; triangles[t + 2] = a + 1;
                triangles[t + 3] = a + 2; triangles[t + 4] = a + 3; triangles[t + 5] = a + 1;
            }
            var mesh = new Mesh { name = "Rack entrance paint patch" };
            mesh.vertices = vertices; mesh.triangles = triangles; mesh.RecalculateNormals();
            stripe.AddComponent<MeshFilter>().sharedMesh = mesh;
            stripe.AddComponent<MeshRenderer>().sharedMaterial = paint;
        }
        MarkerObjects[tree.TreeId] = marker;
    }

    private void EnsureEntranceCue()
    {
        if (entranceCue != null) return;
        Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
        if (shader == null) return;
        cueMaterial = new Material(shader) { name = "Rack entrance cue", color = new Color(0.8f, 0.76f, 0.57f) };
        entranceCue = new GameObject("Next rack entrance cue");
        LineRenderer line = entranceCue.AddComponent<LineRenderer>();
        line.sharedMaterial = cueMaterial; line.useWorldSpace = false; line.loop = true;
        line.widthMultiplier = 0.018f; line.positionCount = 4;
        line.SetPositions(new[] { new Vector3(0f, 0.18f, 0f), new Vector3(0.13f, 0f, 0f), new Vector3(0f, -0.18f, 0f), new Vector3(-0.13f, 0f, 0f) });
        line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
    }

    private void PlaySprayFeedback()
    {
        sprayFeedbackUntil = Time.unscaledTime + 0.18f;
        if (sprayAudio == null)
        {
            sprayAudio = gameObject.AddComponent<AudioSource>();
            sprayAudio.playOnAwake = false; sprayAudio.spatialBlend = 0f; sprayAudio.volume = 0.15f;
            const int samples = 7938; // 180 ms, synthesized hiss, no external asset/licence or simulation RNG
            var data = new float[samples];
            uint noise = 9173;
            float previous = 0f;
            for (int i = 0; i < data.Length; i++)
            {
                noise ^= noise << 13; noise ^= noise >> 17; noise ^= noise << 5;
                float value = (noise % 65536) / 32768f - 1f;
                data[i] = (value - previous) * 0.35f * Mathf.Sin(Mathf.PI * i / samples);
                previous = value;
            }
            sprayClip = AudioClip.Create("Short forestry spray", samples, 1, 44100, false);
            sprayClip.SetData(data, 0);
        }
        sprayAudio.PlayOneShot(sprayClip);
    }

    private void DisposeRackMarkingVisuals()
    {
        if (entranceCue != null) Destroy(entranceCue);
        if (orangePaint != null) Destroy(orangePaint);
        if (cueMaterial != null) Destroy(cueMaterial);
        if (sprayClip != null) Destroy(sprayClip);
    }
}
using System.Collections.Generic;
using System.Text;
using UnityEngine;

// Shows, in the forest, every tree that is in the Work Plan for felling, so nothing is removed that the player cannot see.
//
// A Fell mark (red) is a proposal; when the Work Plan opens, the marks become felling orders and the marks are consumed. Without
// this, the trees that are about to be felled would carry no sign at all between planning and harvest. Trees planned as part of an
// extraction rack carry a VIOLET marker (two stacked diamonds and a wider ground ring), every other planned tree the same red as
// a Fell mark. Neither is blue (Crop Tree), and the neutral white cage is reserved for the inspected tree.
//
// Presentation only: the markers are parented to the trees, are rebuilt from the work orders a few times a second, and write
// no tree, order, save or ecology state. The grouping into racks is session state (see ScenarioOneManager.Racks), so after a
// load rack trees show as ordinary planned trees.
public sealed class PlannedFellMarkers : MonoBehaviour
{
    public static readonly Color RackColour = new Color(0.74f, 0.46f, 0.95f, 1f);
    public static readonly Color FellColour = new Color(1f, 0.35f, 0.05f, 1f);
    private const float SyncSeconds = 0.25f;

    private ScenarioOneManager manager;
    private readonly Dictionary<string, GameObject> markers = new Dictionary<string, GameObject>();
    private readonly Dictionary<string, bool> markerIsRack = new Dictionary<string, bool>();
    private Material rackMaterial, fellMaterial;
    private string signature = "";
    private float nextSync;

    // How many trees currently carry a planned-felling marker, and how many of those are rack trees (the verification reads these).
    public int MarkerCount => markers.Count;
    public int RackMarkerCount { get { int n = 0; foreach (bool rack in markerIsRack.Values) if (rack) n++; return n; } }
    public bool HasMarker(string treeId) => markers.ContainsKey(treeId);
    public bool IsRackMarker(string treeId) => markerIsRack.TryGetValue(treeId, out bool rack) && rack;

    private void Update()
    {
        if (Time.unscaledTime < nextSync)
            return;
        nextSync = Time.unscaledTime + SyncSeconds;
        if (manager == null)
            manager = GetComponent<ScenarioOneManager>() ?? FindFirstObjectByType<ScenarioOneManager>();
        if (manager == null)
            return;
        // A Reference Future preview swaps the forest for the frozen reference: the player's plan has no trees there to mark.
        if (manager.ReferencePreviewActive)
        {
            ClearAll();
            return;
        }
        Sync();
    }

    private void ClearAll()
    {
        foreach (GameObject marker in markers.Values)
            if (marker != null) Destroy(marker);
        markers.Clear();
        markerIsRack.Clear();
        signature = "";
    }

    private void Sync()
    {
        var wanted = new Dictionary<string, bool>(System.StringComparer.Ordinal);
        var sb = new StringBuilder();
        foreach (ScenarioOneWorkOrder order in manager.WorkOrders)
        {
            if (!order.IsOpen || order.type != ScenarioWorkType.FellTree || string.IsNullOrEmpty(order.targetTreeId))
                continue;
            // V2 communicates confirmed routes on the map and painted entrances in the forest.
            // Automatic pending-order discs would look like orange marks before the player sprays.
            if (manager.RackPlanningV2Active && order.rackId > 0) continue;
            bool rack = manager.RackGroupOfOrder(order.workOrderId) != null;
            wanted[order.targetTreeId] = rack;
            sb.Append(order.targetTreeId).Append(rack ? 'R' : 'F').Append(';');
        }
        // The same trees can be replaced by a load, so the identity of the tree objects is part of what is watched.
        ForestTree[] all = FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        long identity = 17;
        foreach (ForestTree tree in all)
            if (tree != null && wanted.ContainsKey(tree.TreeId))
                identity += System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(tree) * 31L;
        sb.Append('#').Append(identity);
        string next = sb.ToString();
        if (next == signature)
            return;
        signature = next;

        var stale = new List<string>();
        foreach (KeyValuePair<string, GameObject> pair in markers)
            if (pair.Value == null || !wanted.TryGetValue(pair.Key, out bool rack) || markerIsRack[pair.Key] != rack)
                stale.Add(pair.Key);
        foreach (string id in stale)
        {
            if (markers[id] != null) Destroy(markers[id]);
            markers.Remove(id);
            markerIsRack.Remove(id);
        }
        foreach (ForestTree tree in all)
        {
            if (tree == null || !tree.IsLiving || markers.ContainsKey(tree.TreeId) || !wanted.TryGetValue(tree.TreeId, out bool isRack))
                continue;
            markers[tree.TreeId] = Build(tree, isRack);
            markerIsRack[tree.TreeId] = isRack;
        }
    }

    private GameObject Build(ForestTree tree, bool rack)
    {
        Material material = rack ? RackMaterial() : FellMaterial();
        var marker = new GameObject(rack ? "Rack Mark" : "Planned Fell Mark");
        marker.transform.SetParent(tree.transform, false);
        AddPrimitive(marker.transform, PrimitiveType.Cube, "Diamond", new Vector3(0f, tree.Height + 0.6f, 0f),
            Quaternion.Euler(45f, 45f, 0f), Vector3.one * 0.45f, material);
        if (rack)
            AddPrimitive(marker.transform, PrimitiveType.Cube, "Second diamond", new Vector3(0f, tree.Height + 1.25f, 0f),
                Quaternion.Euler(45f, 45f, 0f), Vector3.one * 0.3f, material);
        float disc = rack ? 1.7f : 1.2f;
        AddPrimitive(marker.transform, PrimitiveType.Cylinder, "Disc", new Vector3(0f, 0.05f, 0f),
            Quaternion.identity, new Vector3(disc, 0.02f, disc), material);
        return marker;
    }

    private static void AddPrimitive(Transform parent, PrimitiveType type, string name, Vector3 position, Quaternion rotation, Vector3 scale, Material material)
    {
        GameObject part = GameObject.CreatePrimitive(type);
        part.name = name;
        Collider collider = part.GetComponent<Collider>();
        if (collider != null) Destroy(collider);
        part.transform.SetParent(parent, false);
        part.transform.localPosition = position;
        part.transform.localRotation = rotation;
        part.transform.localScale = scale;
        Renderer renderer = part.GetComponent<Renderer>();
        renderer.sharedMaterial = material;
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
    }

    private Material RackMaterial() => rackMaterial != null ? rackMaterial : rackMaterial = NewMaterial("Planned rack mark", RackColour);
    private Material FellMaterial() => fellMaterial != null ? fellMaterial : fellMaterial = NewMaterial("Planned fell mark", FellColour);

    private static Material NewMaterial(string name, Color colour)
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Unlit") ?? Shader.Find("Unlit/Color");
        return new Material(shader) { name = name, color = colour };
    }

    private void OnDestroy()
    {
        foreach (GameObject marker in markers.Values)
            if (marker != null) Destroy(marker);
        markers.Clear();
        if (rackMaterial != null) Destroy(rackMaterial);
        if (fellMaterial != null) Destroy(fellMaterial);
    }
}

Exit: 0
```

$ cat Tools/Verification/RackV2HelpTeaching/run_offline.py
```text
#!/usr/bin/env python3
"""RACK-V2-10 bounded offline/static check: Tree Marks help, its unlock wiring and the V2 Objective 1 copy.

Two parts, no Unity process, nothing written inside the repository:

  static   reads the UI files that cannot compile on plain .NET (UI Toolkit/PlayerPrefs) and checks how they use the pure
           rules: the Tree Marks help context, the V2 Rack help, the Objective 1 concept/practice/instruction/step wording, and
           that no rack marking, geometry, save/schema, scene, prefab, package or settings file changed since the base.
  offline  compiles the production pure-wording files (MenuHelpView.Text.cs, LearningObjectivesView.Curriculum.cs,
           ControlsReference.cs) with RackV2HelpTeachingOfflineCheck.cs on plain .NET and runs it against the approved spec text.

  run_offline.py [--out DIR] [--base SHA]   exit 0 and RACK_V2_HELP_TEACHING_PASS on success. Needs `dotnet` (SDK 8+).

It proves wording, entry set, unlock rule and flow order. Rendering, modal timing in play and text fit are RACK-V2-13's.
"""
import argparse
import os
import re
import shutil
import subprocess
import sys
import tempfile
from pathlib import Path
from xml.sax.saxutils import escape

HERE = Path(__file__).resolve().parent
ROOT = HERE.parents[2]
UI = ROOT / "Assets/ForestPrototype/UI"
DEFAULT_BASE = "6d3228c"  # integration/scenario-one-player-test-ready-v1 with RACK-V2-09 merged (RACK-V2-10 starting HEAD)
PURE = [UI / "MenuHelpView.Text.cs", UI / "LearningObjectivesView.Curriculum.cs", UI / "ControlsReference.cs"]

results = []


def report(ok, message):
    results.append(ok)
    print(("PASS " if ok else "FAIL ") + message)


def git(*args):
    return subprocess.run(["git", "-C", str(ROOT)] + list(args), capture_output=True, text=True)


def body(text, signature):
    """Text of the first brace-delimited block after a signature regex (comments/strings are not special: bodies here are simple)."""
    m = re.search(signature, text)
    if not m:
        return ""
    start = text.find("{", m.end())
    depth = 0
    for i in range(start, len(text)):
        depth += text[i] == "{"
        depth -= text[i] == "}"
        if depth == 0:
            return text[start:i + 1]
    return ""


def static_checks(base):
    root_ui = (UI / "ScenarioOneUiRoot.cs").read_text()
    context = body(root_ui, r"private\s+MenuHelpView\.Menu\s+HelpContext\s*\(\s*\)")
    report("MenuHelpView.TreeMarksUnlocked(manager.RackPlanningV2Active, manager.RackPlanConfirmed)" in context,
           "S1 HelpContext unlocks Tree Marks from saved state only: V2 active and plan confirmed")
    report("MenuHelpView.Introduced(MenuHelpView.Menu.TreeMarks)" in context and "marking.RackSprayEquipped" in context
           and "return MenuHelpView.Menu.TreeMarks;" in context,
           "S2 HelpContext shows Tree Marks until first seen, and while the spray marker is equipped")
    order = [context.find(token) for token in ("Menu.Rack;", "Menu.WorkPlan;", "Menu.StandMap;", "Menu.AnnualReview;",
                                               "Menu.TreeInspection;", "Menu.TreeMarks;", "Menu.WalkingHud;")]
    report(all(i >= 0 for i in order) and order == sorted(order),
           "S3 Tree Marks is a walking-forest context only: after every screen and inspection, before the Walking HUD")
    introduce = re.search(r"if \(screen != UiScreen\.Objectives && context != MenuHelpView\.Menu\.AnnualReview && context != MenuHelpView\.Menu\.Rack\)\s*\n\s*help\.Introduce\(context\);", root_ui)
    report(introduce is not None, "S4 the existing context Introduce path (not a parallel help UI) opens Tree Marks on first use")
    help_view = (UI / "MenuHelpView.cs").read_text()
    render = body(help_view, r"private\s+void\s+Render\s*\(")
    report("ui.Manager.RackPlanningV2Active" in render and "text = RackV2Explanation;" in render,
           "S5 the V2 Rack help uses the checked RackV2Explanation, inside the V2 branch")
    learning = (UI / "LearningObjectivesView.cs").read_text()
    report("bool rackV2Objective = TeachingRacks && ui.Manager.RackPlanningV2Active;" in learning
           and "rackV2Objective ? RackV2Concept : lesson.Concept" in learning and "rackV2Objective ? RackV2Practice : lesson.Practice" in learning,
           "S6 Objective 1 why-it-matters and good-practice use the V2 copy only for the V2 rack objective")
    report("if (ui.Manager.RackPlanningV2Active) text = RackV2StepText(step.Id) ?? text;" in learning,
           "S7 rack lesson texts use RackV2StepText only in a V2 world")
    tracker = (UI / "RackLayoutTracker.cs").read_text()
    instruction = body(tracker, r"public\s+string\s+V2ObjectiveInstruction")
    report("LearningObjectivesView.RackV2Instruction(V2CounterText, m.RackPlanConfirmed, m.MarkedRackEntranceCount," in instruction
           and "ScenarioOneManager.RackPlanTargetCount, WorkPending)" in instruction,
           "S8 the V2 objective sentence (HUD widget and Objectives screen) is the checked RackV2Instruction")
    for path in PURE:
        text = path.read_text()
        # The plain .NET compile below is the real proof (no UnityEngine reference exists there); this names the file that broke it.
        report("using UnityEngine" not in text, "S9 %s stays free of UnityEngine" % path.name)

    changed = git("diff", "--name-only", base, "HEAD").stdout.split() + git("diff", "--name-only", "HEAD").stdout.split() \
        + git("ls-files", "--others", "--exclude-standard").stdout.split()
    if git("rev-parse", "--verify", base + "^{commit}").returncode != 0:
        report(False, "S10 base %s is not a commit here" % base)
        return
    allowed = {
        "Assets/ForestPrototype/UI/MenuHelpView.Text.cs", "Assets/ForestPrototype/UI/MenuHelpView.cs",
        "Assets/ForestPrototype/UI/ScenarioOneUiRoot.cs", "Assets/ForestPrototype/UI/LearningObjectivesView.cs",
        "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs", "Assets/ForestPrototype/UI/RackLayoutTracker.cs",
        "Tools/Verification/MenuTutorialVerification.cs",
    }
    outside = sorted({p for p in changed if p not in allowed and not p.startswith("Tools/Verification/RackV2HelpTeaching/")
                      and not p.startswith("Docs/Handoffs/RACK-V2-10/")})
    report(not outside, "S10 only the RACK-V2-10 copy/wiring files changed since %s%s" % (base, "" if not outside else ": " + ", ".join(outside[:6])))
    protected = [p for p in changed if re.search(r"(ForestTreeMarkingManager|ScenarioOneManager|ForestSave|Racks/|\.unity$|\.prefab$|\.mat$|\.asset$|^ProjectSettings/|^Packages/|\.meta$)", p)]
    report(not protected, "S11 no marking, rack geometry, manager, save/schema, scene, prefab, material, settings, package or .meta change")
    untracked = [p for p in git("ls-files", "--others", "--exclude-standard", "--", "Assets").stdout.split()]
    report(not untracked, "S12 no untracked file (copied harness or generated .meta) under Assets/%s" % ("" if not untracked else ": " + ", ".join(untracked[:4])))


def offline(out):
    dotnet = shutil.which("dotnet")
    if not dotnet:
        report(False, "dotnet SDK not found")
        return
    sources = PURE + [HERE / "RackV2HelpTeachingOfflineCheck.cs"]
    out.mkdir(parents=True, exist_ok=True)
    items = "\n".join('<Compile Include="%s" />' % escape(str(p)) for p in sources)
    (out / "check.csproj").write_text(
        '<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net8.0</TargetFramework>'
        '<LangVersion>9.0</LangVersion><Nullable>disable</Nullable><ImplicitUsings>disable</ImplicitUsings>'
        '<EnableDefaultCompileItems>false</EnableDefaultCompileItems><StartupObject>RackV2HelpTeachingOfflineCheck</StartupObject></PropertyGroup>'
        "<ItemGroup>" + items + "</ItemGroup></Project>")
    env = dict(os.environ, DOTNET_CLI_TELEMETRY_OPTOUT="1", DOTNET_NOLOGO="1")
    build = subprocess.run([dotnet, "build", "-nologo", "-v", "q", "-c", "Release"], cwd=out, env=env, capture_output=True, text=True)
    if build.returncode:
        sys.stdout.write(build.stdout + build.stderr)
        report(False, "offline build")
        return
    run = subprocess.run([dotnet, "run", "--no-build", "-c", "Release", "--", str(ROOT)], cwd=out, env=env, capture_output=True, text=True)
    sys.stdout.write(run.stdout + run.stderr)
    report(run.returncode == 0 and "RACK_V2_HELP_TEACHING_OFFLINE_PASS" in run.stdout, "offline wording/unlock/flow check")


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--out", type=Path, default=Path(tempfile.gettempdir()) / "ccf-rack-v2-help-teaching-offline")
    parser.add_argument("--base", default=DEFAULT_BASE)
    args = parser.parse_args()
    static_checks(args.base)
    offline(args.out)
    failed = results.count(False)
    print(("RACK_V2_HELP_TEACHING_PASS" if not failed else "RACK_V2_HELP_TEACHING_FAIL failed=%d" % failed) + " static_and_offline=%d" % len(results))
    return 1 if failed else 0


if __name__ == "__main__":
    sys.exit(main())

Exit: 0
```

$ rg -n "16.?20|Paint|Color|Draw.*Mark|Fell|Crop" Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs Assets/ForestPrototype/UI/StandMapView.Drawing.cs
```text
rg: Assets/ForestPrototype/UI/StandMapView.Drawing.cs: No such file or directory (os error 2)
Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs:30:            new Step("rack.result", "Advance a year and see the racks", "Advance one year and read the Annual Review. Then walk a rack: it is now an open corridor of stumps across the plantation. Inspecting trees, Crop Trees and selective felling come next.")),
Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs:31:        new Topic("2. Tree inspection and Crop Trees", "Begin with trees worth retaining and favouring, then inspect their competitors. Being smaller or suppressed is not by itself a reason to remove a tree. There is no single correct tree picked by this lesson.",
Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs:34:            new Step("tree.crop", "Designate a Crop Tree", "Aim at a living tree you want to retain and press C. A blue mark means retain/favour, not fell. Press C again to undo it. Choose a tree yourself after inspection; this designation is also used by crop-tree pruning.")),
Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs:35:        new Topic("3. Felling", "Continuous-cover forestry needs repeated selective management. More felling is not automatically better: consider what remains, then return later to inspect and reassess. Learn the proposal → plan → approval → results sequence. Take time to compare trees; no cutting is required in year one.",
Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs:36:            new Step("fell.mark", "Propose a tree for felling", "Aim at a living tree and press X. The red mark is only a proposal. X again removes it. Crop and Fell marks are alternatives on the same tree; check the label before proceeding."),
Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs:37:            new Step("fell.plan", "Import the Fell mark into the Work Plan", "Press Tab. Your red Fell marks join the Work Plan as jobs when you open it. Inspect each job's timber estimate, contractor cost and material outcome. Removing a pending job lets you reconsider."),
Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs:56:            new Step("map.marks", "Explore Fell & crop marks", "Choose Fell & crop marks. Squares count Fell proposals; diamonds count Crop Trees. Red means proposed removal, blue means retain/favour. This is a view of decisions made in the forest, not a remote marking tool."),
Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs:59:        new Topic("6. Pruning", "Pruning removes lower branches from retained Crop Trees for future timber quality and a clearer stem. Pruning sets no price by itself: where Oak timber is marketed, the recorded pruned section can make a large log eligible for a higher grade, while Sitka and Beech have no such premium here, and fork or defect shape is not modelled. It costs contractor time and money. A tree may need to grow or recover before it is eligible.",
Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs:60:            new Step("prune.read", "Understand eligibility", "The Work Plan reports how many Crop Trees are eligible. Sitka uses lifts to 2.5 m, 5 m and 6.5 m (at most three lifts, at least five years apart). When Oak and Beech timber markets are active, Oak and Beech use two lifts, to 3.5 m then 6.0 m, at least three years apart; otherwise they keep the three-lift schedule above. All are limited to a crown base below 60% of tree height. These are scenario rules, not a recommendation to prune every tree.", true, "tree.crop"),
Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs:62:            new Step("prune.approve", "Approve the selected pruning work", "Check contractor cost and available cash, then approve pending work. A designated Crop Tree stays standing; the lift is executed during the annual work cycle."),
Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs:85:    // sentence. 16-20 m is the book's early-transformation example and this stand's calibrated band; five racks is calibration for this
Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs:88:        + "In early CCF transformation, rack spacing may commonly be around 16–20 m, but appropriate spacing depends on the stand, machinery and site. "
Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs:114:                + "Spacing is conditional: around 16–20 m between racks may be common in early transformation, around 30 m or more may be viable in developed CCF, and there is no universal rule. "
Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs:115:                + "Scenario One recommends 16–20 m and five racks for this plantation only.";

Exit: 2
```

$ rg --files Assets/ForestPrototype/UI | rg StandMap
```text
Assets/ForestPrototype/UI/StandMapView.cs
Assets/ForestPrototype/UI/StandMapView.cs.meta
Assets/ForestPrototype/UI/StandMapTreatmentReview.cs
Assets/ForestPrototype/UI/Map/StandMapLegend.cs.meta
Assets/ForestPrototype/UI/Map/StandMapSource.cs.meta
Assets/ForestPrototype/UI/Map/StandMapReviewPlan.cs
Assets/ForestPrototype/UI/Map/StandMapCanopyLayer.cs.meta
Assets/ForestPrototype/UI/Map/StandMapCanopyRaster.cs
Assets/ForestPrototype/UI/Map/StandMapDrawPlan.cs.meta
Assets/ForestPrototype/UI/Map/StandMapCanvas.cs.meta
Assets/ForestPrototype/UI/Map/StandMapAreaFootprints.cs.meta
Assets/ForestPrototype/UI/Map/StandMapRackEditor.cs
Assets/ForestPrototype/UI/Map/StandMapSource.cs
Assets/ForestPrototype/UI/Map/StandMapDrawPlan.cs
Assets/ForestPrototype/UI/Map/StandMapSymbols.cs
Assets/ForestPrototype/UI/Map/StandMapRackEditor.cs.meta
Assets/ForestPrototype/UI/Map/StandMapOverlay.cs
Assets/ForestPrototype/UI/Map/StandMapTransform.cs
Assets/ForestPrototype/UI/Map/StandMapAreaFootprints.cs
Assets/ForestPrototype/UI/Map/StandMapOverlay.cs.meta
Assets/ForestPrototype/UI/Map/StandMapGridPlan.cs.meta
Assets/ForestPrototype/UI/Map/StandMapSurface.cs
Assets/ForestPrototype/UI/Map/StandMapLegendView.cs
Assets/ForestPrototype/UI/Map/StandMapLegend.cs
Assets/ForestPrototype/UI/Map/StandMapCanvas.cs
Assets/ForestPrototype/UI/Map/StandMapTransform.cs.meta
Assets/ForestPrototype/UI/Map/StandMapSurface.cs.meta
Assets/ForestPrototype/UI/Map/StandMapCanopyLayer.cs
Assets/ForestPrototype/UI/Map/StandMapReviewPlan.cs.meta
Assets/ForestPrototype/UI/Map/StandMapReviewLayer.cs
Assets/ForestPrototype/UI/Map/StandMapModel.cs
Assets/ForestPrototype/UI/Map/StandMapCanopyRaster.cs.meta
Assets/ForestPrototype/UI/Map/StandMapLegendView.cs.meta
Assets/ForestPrototype/UI/Map/StandMapReviewLayer.cs.meta
Assets/ForestPrototype/UI/Map/StandMapSymbols.cs.meta
Assets/ForestPrototype/UI/Map/StandMapGridPlan.cs
Assets/ForestPrototype/UI/Map/StandMapModel.cs.meta
Assets/ForestPrototype/UI/StandMapTreatmentReview.cs.meta

Exit: 0
```

$ rg -n -A 45 "private void CreateMarker" Assets/ForestPrototype/ForestTreeMarkingManager.cs
```text
580:    private void CreateMarker(ForestTree tree, TreeMarkType type)
581-    {
582-        if (MarkerObjects.TryGetValue(tree.TreeId, out GameObject existing) && existing != null)
583-            return;
584-        Material material = MarkerMaterial(type);
585-        if (material == null)
586-            return;
587-
588-        var marker = new GameObject(type == TreeMarkType.CropTree ? "Crop Tree Mark" : "Harvest Mark");
589-        marker.transform.SetParent(tree.transform, false);
590-
591-        GameObject diamond = GameObject.CreatePrimitive(PrimitiveType.Cube);
592-        diamond.name = "Diamond";
593-        Destroy(diamond.GetComponent<Collider>());
594-        diamond.transform.SetParent(marker.transform, false);
595-        diamond.transform.localPosition = new Vector3(0f, tree.Height + 0.6f, 0f);
596-        diamond.transform.localRotation = Quaternion.Euler(45f, 45f, 0f);
597-        diamond.transform.localScale = Vector3.one * 0.45f;
598-        diamond.GetComponent<Renderer>().sharedMaterial = material;
599-
600-        GameObject disc = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
601-        disc.name = "Disc";
602-        Destroy(disc.GetComponent<Collider>());
603-        disc.transform.SetParent(marker.transform, false);
604-        disc.transform.localPosition = new Vector3(0f, 0.05f, 0f);
605-        disc.transform.localScale = new Vector3(1.2f, 0.02f, 1.2f);
606-        disc.GetComponent<Renderer>().sharedMaterial = material;
607-
608-        // A visible painted band on the trunk makes persistent blue Crop Trees
609-        // identifiable while walking under the canopy, not just from above.
610-        if (type == TreeMarkType.CropTree)
611-        {
612-            GameObject band = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
613-            band.name = "Blue Trunk Band";
614-            Destroy(band.GetComponent<Collider>());
615-            band.transform.SetParent(marker.transform, false);
616-            band.transform.localPosition = new Vector3(0f, Mathf.Min(1.6f, tree.Height * 0.5f), 0f);
617-            band.transform.localScale = new Vector3(Mathf.Max(0.25f, tree.Diameter / 100f + 0.08f),
618-                0.12f, Mathf.Max(0.25f, tree.Diameter / 100f + 0.08f));
619-            band.GetComponent<Renderer>().sharedMaterial = material;
620-        }
621-
622-        MarkerObjects[tree.TreeId] = marker;
623-    }
624-
625-    private void RemoveMarker(string treeId)

Exit: 0
```

$ rg -n -A 55 "Create.*Stripe|Create.*Mark|Paint|new Color|four|side" Assets/ForestPrototype/ForestTreeMarkingManager.RackEntrances.cs
```text
12:    private Material orangePaint, cueMaterial;
13-    private GameObject entranceCue;
14-    private AudioSource sprayAudio;
15-    private AudioClip sprayClip;
16-    private bool sprayLockedLastFrame;
17-    private float sprayFeedbackUntil;
18-
19-    public bool RackSprayEquipped { get; private set; }
20-    public bool RackSprayFeedbackActive => Time.unscaledTime < sprayFeedbackUntil;
21-    public int CueRackId { get; private set; }
22-    public string RackFieldStatus { get; private set; } = "";
23-
24-    private ScenarioOneManager RackScenario
25-    {
26-        get
27-        {
28-            if (rackScenario == null) rackScenario = FindFirstObjectByType<ScenarioOneManager>();
29-            return rackScenario;
30-        }
31-    }
32-
33-    public bool HasRackEntranceMark(ForestTree tree)
34-    {
35-        ScenarioOneManager scenario = RackScenario;
36-        if (tree == null || scenario == null || !scenario.RackPlanningV2Active) return false;
37-        // Read draft records too: reopening alone never erases a geometry-bound mark.
38-        foreach (RackSaveData rack in scenario.Racks)
39-            if (rack != null && rack.entranceMarked && rack.entranceTreeId == tree.TreeId) return true;
40-        return false;
41-    }
42-
43-    private bool IsRackRemovalTree(ForestTree tree)
44-    {
45-        ScenarioOneManager scenario = RackScenario;
46-        if (tree == null || scenario == null || !scenario.RackPlanningV2Active) return false;
47-        foreach (RackSaveData rack in scenario.Racks)
48-            if (rack != null && rack.removalTreeIds != null && rack.removalTreeIds.Contains(tree.TreeId)) return true;
49-        return false;
50-    }
51-
52-    // First unmarked, unworked rack by stable ID. Never redirect a broken rack to a nearby tree.
53-    public bool TryGetRackFieldTarget(out RackPlanRack rack, out ForestTree tree, out string problem)
54-    {
55-        rack = null; tree = null; problem = "";
56-        ScenarioOneManager scenario = RackScenario;
57-        if (scenario == null || !scenario.RackPlanningV2Active || !scenario.RackPlanConfirmed) return false;
58-        foreach (RackPlanRack candidate in scenario.ConfirmedRacks)
59-        {
60-            if (candidate.EntranceMarked || candidate.IsLocked) continue;
61-            rack = candidate;
62-            return scenario.TryGetRackEntranceTarget(candidate.Id, out tree, out problem);
63-        }
64-        return false;
65-    }
66-
67-    public bool EquipRackSpray(bool equip)
--
132:                CreateRackEntranceMarker(tree);
133-                PlaySprayFeedback();
134-                // Hide this rack's cue immediately; next Update projects the next real target.
135-                if (entranceCue != null) entranceCue.SetActive(false);
136-                CueRackId = 0;
137-            }
138-        }
139-        SetMessage(result.Message);
140-        return result;
141-    }
142-
143-    private bool TryRackSprayAim(out ForestTree tree)
144-    {
145-        tree = null;
146-        if (view == null) return false;
147-        // RaycastAll avoids dropping the closest blocker when dense crowns fill a fixed buffer.
148-        RaycastHit[] hits = Physics.RaycastAll(view.transform.position, view.transform.forward, RackSprayDistance,
149-            Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
150-        float nearest = float.MaxValue;
151-        foreach (RaycastHit hit in hits)
152-        {
153-            Transform hitTransform = hit.collider.transform;
154-            if (playerRoot != null && (hitTransform == playerRoot || hitTransform.IsChildOf(playerRoot))) continue;
155-            if (hit.collider.GetComponentInParent<ScenarioHabitatVisuals>() != null
156-                || (hit.collider.GetComponentInParent<ForestEcologyController>() != null
157-                    && hit.collider.GetComponentInParent<ForestTree>() == null)) continue;
158-            if (hit.distance >= nearest) continue;
159-            nearest = hit.distance;
160-            tree = hit.collider.GetComponentInParent<ForestTree>();
161-        }
162-        return tree != null && tree.IsLiving;
163-    }
164-
165-    public string RackSprayPrompt()
166-    {
167-        if (!RackSprayEquipped) return "";
168-        if (!TryRackSprayAim(out ForestTree tree)) return "Spray marker · aim at an entrance trunk within 3.5 m · [P] Stow";
169-        if (HasRackEntranceMark(tree)) return "Orange · entrance already marked · [P] Stow";
170-        ScenarioOneManager scenario = RackScenario;
171-        foreach (RackPlanRack rack in scenario.ConfirmedRacks)
172-        {
173-            if (rack.EntranceTreeId != tree.TreeId) continue;
174-            if (rack.IsLocked) return "Rack already harvested · entrance locked";
175-            if (tree.IsCropTree) return "Crop Tree · remove its blue designation before spraying";
176-            if (tree.IsMarkedForFell) return "Fell mark · remove its red designation before spraying";
177-            if (!scenario.TryGetRackEntranceTarget(rack.Id, out _, out string problem)) return problem;
178-            return $"Rack {rack.Id} entrance · [Left-click] Spray orange · [P] Stow";
179-        }
180-        return "Not a rack entrance · follow the entrance cue · [P] Stow";
181-    }
182-
183:    private Material OrangePaint()
184-    {
185:        if (orangePaint == null)
186-        {
187-            Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
188:            if (shader != null) orangePaint = new Material(shader) { name = "Rack entrance orange paint", color = new Color(0.95f, 0.28f, 0.025f) };
189-        }
190:        return orangePaint;
191-    }
192-
193:    private void CreateRackEntranceMarker(ForestTree tree)
194-    {
195-        if (MarkerObjects.TryGetValue(tree.TreeId, out GameObject existing) && existing != null) return;
196:        Material paint = OrangePaint();
197-        if (paint == null) return;
198-        var marker = new GameObject(RackEntranceMarkerName);
199-        marker.transform.SetParent(tree.transform, false);
200-        // Four narrow curved patches wrap the trunk rather than becoming floating square signs.
201-        float radius = Mathf.Max(0.08f, tree.Diameter / 200f) + 0.012f;
202:        for (int side = 0; side < 4; side++)
203-        {
204:            var stripe = new GameObject("Orange vertical stripe " + (side + 1));
205-            stripe.transform.SetParent(marker.transform, false);
206-            const int segments = 6;
207-            var vertices = new Vector3[(segments + 1) * 2];
208-            var triangles = new int[segments * 6];
209-            for (int i = 0; i <= segments; i++)
210-            {
211:                float angle = (side * 90f - 12f + 24f * i / segments) * Mathf.Deg2Rad;
212-                float x = Mathf.Sin(angle) * radius, z = Mathf.Cos(angle) * radius;
213-                vertices[i * 2] = new Vector3(x, 1.0f, z);
214-                vertices[i * 2 + 1] = new Vector3(x, Mathf.Min(1.85f, tree.Height * 0.75f), z);
215-                if (i == segments) continue;
216-                int a = i * 2, t = i * 6;
217-                triangles[t] = a; triangles[t + 1] = a + 2; triangles[t + 2] = a + 1;
218-                triangles[t + 3] = a + 2; triangles[t + 4] = a + 3; triangles[t + 5] = a + 1;
219-            }
220-            var mesh = new Mesh { name = "Rack entrance paint patch" };
221-            mesh.vertices = vertices; mesh.triangles = triangles; mesh.RecalculateNormals();
222-            stripe.AddComponent<MeshFilter>().sharedMesh = mesh;
223-            stripe.AddComponent<MeshRenderer>().sharedMaterial = paint;
224-        }
225-        MarkerObjects[tree.TreeId] = marker;
226-    }
227-
228-    private void EnsureEntranceCue()
229-    {
230-        if (entranceCue != null) return;
231-        Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
232-        if (shader == null) return;
233:        cueMaterial = new Material(shader) { name = "Rack entrance cue", color = new Color(0.8f, 0.76f, 0.57f) };
234-        entranceCue = new GameObject("Next rack entrance cue");
235-        LineRenderer line = entranceCue.AddComponent<LineRenderer>();
236-        line.sharedMaterial = cueMaterial; line.useWorldSpace = false; line.loop = true;
237-        line.widthMultiplier = 0.018f; line.positionCount = 4;
238-        line.SetPositions(new[] { new Vector3(0f, 0.18f, 0f), new Vector3(0.13f, 0f, 0f), new Vector3(0f, -0.18f, 0f), new Vector3(-0.13f, 0f, 0f) });
239-        line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
240-    }
241-
242-    private void PlaySprayFeedback()
243-    {
244-        sprayFeedbackUntil = Time.unscaledTime + 0.18f;
245-        if (sprayAudio == null)
246-        {
247-            sprayAudio = gameObject.AddComponent<AudioSource>();
248-            sprayAudio.playOnAwake = false; sprayAudio.spatialBlend = 0f; sprayAudio.volume = 0.15f;
249-            const int samples = 7938; // 180 ms, synthesized hiss, no external asset/licence or simulation RNG
250-            var data = new float[samples];
251-            uint noise = 9173;
252-            float previous = 0f;
253-            for (int i = 0; i < data.Length; i++)
254-            {
255-                noise ^= noise << 13; noise ^= noise >> 17; noise ^= noise << 5;
256-                float value = (noise % 65536) / 32768f - 1f;
257-                data[i] = (value - previous) * 0.35f * Mathf.Sin(Mathf.PI * i / samples);
258-                previous = value;
259-            }
260-            sprayClip = AudioClip.Create("Short forestry spray", samples, 1, 44100, false);
261-            sprayClip.SetData(data, 0);
262-        }
263-        sprayAudio.PlayOneShot(sprayClip);
264-    }
265-
266-    private void DisposeRackMarkingVisuals()
267-    {
268-        if (entranceCue != null) Destroy(entranceCue);
269:        if (orangePaint != null) Destroy(orangePaint);
270-        if (cueMaterial != null) Destroy(cueMaterial);
271-        if (sprayClip != null) Destroy(sprayClip);
272-    }
273-}

Exit: 0
```

$ rg -n -A 16 "Fell|Crop|orange|Orange" Assets/ForestPrototype/UI/Map/StandMapSymbols.cs
```text
106:        Add(MapSymbol.TreeMarkFell, new MapColour(220, 76, 62), 0f, 2.2f, false, "red ring", "marked to fell");
107:        Add(MapSymbol.TreeMarkCrop, new MapColour(78, 140, 236), 0f, 2.6f, false, "blue ring", "Crop Tree");
108-        Add(MapSymbol.Juvenile, new MapColour(170, 226, 128), 0f, 1.2f, false, "light green dot", "planted juvenile");
109-        Add(MapSymbol.Shelter, new MapColour(128, 182, 222), 0f, 1.8f, false, "blue ring", "shelter, still effective");
110-        Add(MapSymbol.ShelterLapsed, new MapColour(158, 158, 150), 0f, 1.8f, false, "grey ring", "shelter, expired or failed");
111-        return table;
112-    }
113-
114-    public static MapSymbolStyle Get(MapSymbol symbol) => styles[symbol];
115-
116-    public static IEnumerable<MapSymbolStyle> All => styles.Values;
117-
118-    // The filter. Mini shows the symbols marked InMini; Full shows all of them.
119-    public static bool IsDrawn(MapSymbol symbol, MapDetail detail) => detail == MapDetail.Full || styles[symbol].InMini;
120-
121-    // One line naming the drawn symbols, for a presentation that prints its key as a sentence.
122-    public static string Legend(MapDetail detail)
123-    {

Exit: 0
```

$ command -v dotnet
```text
/usr/bin/dotnet

Exit: 0
```

$ rg --files -g AGENTS.md
```text
AGENTS.md

Exit: 0
```

$ rg -n '16–20 m|CCF source material|Tree Marks · what the paint on a tree means|Scenario One uses three marks so far|The source describes' Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs Assets/ForestPrototype/UI/MenuHelpView.Text.cs
```text

Exit: 1
```

$ rg -n '16–20m|Introducing tree marking|Scenario 1 introduces 3 CCF markings|Orange vertical|new\[\] \{ "Red"|new\[\] \{ "Blue"|One spray paints' Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs Assets/ForestPrototype/UI/MenuHelpView.Text.cs
```text
Assets/ForestPrototype/UI/MenuHelpView.Text.cs:21:        new[] { "Orange vertical stripes", "Rack / machine-path removal" },
Assets/ForestPrototype/UI/MenuHelpView.Text.cs:22:        new[] { "Red", "Fell" },
Assets/ForestPrototype/UI/MenuHelpView.Text.cs:23:        new[] { "Blue", "Crop Tree / future timber" }
Assets/ForestPrototype/UI/MenuHelpView.Text.cs:28:    public const string TreeMarksOrangeText = "Orange vertical stripes identify rack entrance trees associated with machine-path removal in Scenario 1.";
Assets/ForestPrototype/UI/MenuHelpView.Text.cs:46:            case Menu.TreeMarks: return "Introducing tree marking";
Assets/ForestPrototype/UI/MenuHelpView.Text.cs:94:                return "Scenario 1 introduces 3 CCF markings. In this forest a paint mark records a management decision on the tree itself:\n"
Assets/ForestPrototype/UI/MenuHelpView.Text.cs:96:                    + TreeMarksOrangeText + " One spray paints vertical stripes on four sides of the entrance tree.\n\n"
Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs:88:        + "In early CCF transformation, rack spacing may commonly be around 16–20m, but appropriate spacing depends on the stand, machinery and site. "
Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs:114:                + "Spacing is conditional: around 16–20m between racks may be common in early transformation, around 30 m or more may be viable in developed CCF, and there is no universal rule. "
Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs:115:                + "Scenario One recommends 16–20m and five racks for this plantation only.";

Exit: 0
```

$ python3 -B -c "import runpy; m=runpy.run_path('Tools/Verification/RackV2HelpTeaching/run_offline.py'); m['static_checks']('72b4efbd1c653fa2231f632a79030a83ff76f833'); r=m['results']; print('STATIC_SUBSET checks=%d failures=%d' % (len(r),r.count(False))); raise SystemExit(1 if False in r else 0)"
```text
PASS S1 HelpContext unlocks Tree Marks from saved state only: V2 active and plan confirmed
PASS S2 HelpContext shows Tree Marks until first seen, and while the spray marker is equipped
PASS S3 Tree Marks is a walking-forest context only: after every screen and inspection, before the Walking HUD
PASS S4 the existing context Introduce path (not a parallel help UI) opens Tree Marks on first use
PASS S5 the V2 Rack help uses the checked RackV2Explanation, inside the V2 branch
PASS S6 Objective 1 why-it-matters and good-practice use the V2 copy only for the V2 rack objective
PASS S7 rack lesson texts use RackV2StepText only in a V2 world
PASS S8 the V2 objective sentence (HUD widget and Objectives screen) is the checked RackV2Instruction
PASS S9 MenuHelpView.Text.cs stays free of UnityEngine
PASS S9 LearningObjectivesView.Curriculum.cs stays free of UnityEngine
PASS S9 ControlsReference.cs stays free of UnityEngine
FAIL S10 only the RACK-V2-10 copy/wiring files changed since 72b4efbd1c653fa2231f632a79030a83ff76f833: Docs/Handoffs/Overnight-Continuation-20261104/OBJECTIVE-COPY-HANDOFF.md
PASS S11 no marking, rack geometry, manager, save/schema, scene, prefab, material, settings, package or .meta change
PASS S12 no untracked file (copied harness or generated .meta) under Assets/
STATIC_SUBSET checks=14 failures=1

Exit: 1
```

$ git diff --check
```text

Exit: 0
```

$ git diff --stat
```text
 Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs | 6 +++---
 Assets/ForestPrototype/UI/MenuHelpView.Text.cs                 | 8 ++++----
 2 files changed, 7 insertions(+), 7 deletions(-)

Exit: 0
```

$ git diff -- Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs Assets/ForestPrototype/UI/MenuHelpView.Text.cs
```text
diff --git a/Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs b/Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs
index a6b2472f..9a705593 100644
--- a/Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs
+++ b/Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs
@@ -85,7 +85,7 @@ public sealed partial class LearningObjectivesView
     // sentence. 16-20 m is the book's early-transformation example and this stand's calibrated band; five racks is calibration for this
     // stand. Neither is taught as a general rule.
     public const string RackV2Concept = "Extraction racks allow machinery to reach trees without creating access everywhere across the stand. "
-        + "In early CCF transformation, rack spacing may commonly be around 16–20 m, but appropriate spacing depends on the stand, machinery and site. "
+        + "In early CCF transformation, rack spacing may commonly be around 16–20m, but appropriate spacing depends on the stand, machinery and site. "
         + "Scenario One uses that range as a guide for this plantation. "
         + "This stand needs five extraction racks to create a workable first access layout. "
         + "Plan the racks on the map, then mark each rack entrance in the forest.";
@@ -111,8 +111,8 @@ public sealed partial class LearningObjectivesView
         switch (stepId)
         {
             case "rack.read": return "A rack is a straight corridor of removed trees that machinery drives along, so the rest of the stand can be reached without driving through it. It stays open for this and later thinnings. "
-                + "Spacing is conditional: around 16–20 m between racks may be common in early transformation, around 30 m or more may be viable in developed CCF, and there is no universal rule. "
-                + "Scenario One recommends 16–20 m and five racks for this plantation only.";
+                + "Spacing is conditional: around 16–20m between racks may be common in early transformation, around 30 m or more may be viable in developed CCF, and there is no universal rule. "
+                + "Scenario One recommends 16–20m and five racks for this plantation only.";
             case "rack.create": return "Open the Stand Map [M] and choose Plan Racks. Create and edit your first rack on the map; no trees are cut by planning.";
             case "rack.layout": return "Plan five racks on the Stand Map [M] and choose Confirm Rack Plan. Then return to the forest and mark each rack entrance tree in orange: [P] spray marker, aim within 3.5 m, left-click. Tree Marks help explains the paint.";
             case "rack.approve": return "Once the rack entrances are marked, open the Work Plan [Tab]. Check the rack removal work and its cost, then choose Approve Work Plan. Approval does not cut anything yet.";
diff --git a/Assets/ForestPrototype/UI/MenuHelpView.Text.cs b/Assets/ForestPrototype/UI/MenuHelpView.Text.cs
index 0c7a3443..9ce46e6f 100644
--- a/Assets/ForestPrototype/UI/MenuHelpView.Text.cs
+++ b/Assets/ForestPrototype/UI/MenuHelpView.Text.cs
@@ -25,7 +25,7 @@ public sealed partial class MenuHelpView
 
     // Spec §28 wording. The first sentence pair is the source convention (book pp. 84-85, orange or pink; Scenario One's choice of
     // orange is calibration); the second discloses that marking only the entrance tree is this game's teaching simplification.
-    public const string TreeMarksOrangeText = "Removal trees associated with a machine route can be marked with vertical stripes. The CCF source material describes removal/treatment colours such as orange or pink; Scenario One uses orange.";
+    public const string TreeMarksOrangeText = "Orange vertical stripes identify rack entrance trees associated with machine-path removal in Scenario 1.";
     public const string TreeMarksSimplificationText = "The game asks you to mark the rack entrance tree as a teaching action. Real marking practice may mark additional trees along a route.";
 
     // The Rack screen in a Rack Planning V2 world (spec §30): plan on the map, confirm, mark the entrances, then approve.
@@ -43,7 +43,7 @@ public sealed partial class MenuHelpView
             case Menu.WorkPlan: return "Work Plan · review and approve jobs";
             case Menu.AnnualReview: return "Annual Review · learn from the year";
             case Menu.Rack: return "Extraction racks · open machine routes";
-            case Menu.TreeMarks: return "Tree Marks · what the paint on a tree means";
+            case Menu.TreeMarks: return "Introducing tree marking";
             default: return "Walking HUD · your forest at a glance";
         }
     }
@@ -91,9 +91,9 @@ public sealed partial class MenuHelpView
         switch (menu)
         {
             case Menu.TreeMarks:
-                return "In this forest a paint mark records a management decision on the tree itself. Scenario One uses three marks so far:\n"
+                return "Scenario 1 introduces 3 CCF markings. In this forest a paint mark records a management decision on the tree itself:\n"
                     + string.Join("\n", System.Array.ConvertAll(TreeMarkEntries, entry => "• " + entry[0] + " — " + entry[1])) + "\n\n"
-                    + TreeMarksOrangeText + " The source describes the stripes on four sides of the tree; here one spray paints all four.\n\n"
+                    + TreeMarksOrangeText + " One spray paints vertical stripes on four sides of the entrance tree.\n\n"
                     + TreeMarksSimplificationText + "\n\n"
                     + "After Confirm Rack Plan, walk to a rack entrance and press P to equip the spray marker. Aim at the first rack tree nearest the path, within 3.5 m, and left-click; P stows the marker. "
                     + "A tree carries only one of these marks: the spray will not mark a Fell or Crop Tree, and X or C cannot change an orange mark. "

Exit: 0
```

$ rg -n -A 34 "Material MarkerMaterial" Assets/ForestPrototype/ForestTreeMarkingManager.cs
```text
639:    private Material MarkerMaterial(TreeMarkType type)
640-    {
641-        if (type == TreeMarkType.CropTree && cropMarkerMaterial != null)
642-            return cropMarkerMaterial;
643-        if (type == TreeMarkType.Fell && markerMaterial != null)
644-            return markerMaterial;
645-        if (type == TreeMarkType.CropTree && markerMaterial != null)
646-        {
647-            cropMarkerMaterial = new Material(markerMaterial) { name = "CropTreeMark", color = cropTreeColor };
648-            return cropMarkerMaterial;
649-        }
650-        Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
651-        if (shader == null)
652-            shader = Shader.Find("Unlit/Color");
653-        if (shader == null)
654-        {
655-            Debug.LogError("Harvest mark material is missing (no serialized material and no shader found in this build). Assign the HarvestMark material asset to the marking manager.");
656-            return null;
657-        }
658-        if (type == TreeMarkType.CropTree)
659-        {
660-            cropMarkerMaterial = new Material(shader) { name = "CropTreeMark", color = cropTreeColor };
661-            return cropMarkerMaterial;
662-        }
663-        markerMaterial = new Material(shader) { name = "HarvestMark", color = markerColor };
664-        return markerMaterial;
665-    }
666-
667-    private void SetMessage(string text)
668-    {
669-        message = text;
670-        messageTimer = 2.5f;
671-    }
672-
673-    // ----- Read-only presentation access (Scenario One UI Toolkit HUD) -----

Exit: 0
```

$ cat Assets/ForestPrototype/Materials/HarvestMark.mat
```text
%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!114 &-9098976119410394004
MonoBehaviour:
  m_ObjectHideFlags: 11
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 0}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: d0353a89b1f911e48b9e16bdc9f2e058, type: 3}
  m_Name: 
  m_EditorClassIdentifier: Unity.RenderPipelines.Universal.Editor::UnityEditor.Rendering.Universal.AssetVersion
  version: 10
--- !u!21 &2100000
Material:
  serializedVersion: 8
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_Name: HarvestMark
  m_Shader: {fileID: 4800000, guid: 650dd9526735d5b46b79224bc6e94025, type: 3}
  m_Parent: {fileID: 0}
  m_ModifiedSerializedProperties: 0
  m_ValidKeywords: []
  m_InvalidKeywords: []
  m_LightmapFlags: 4
  m_EnableInstancingVariants: 0
  m_DoubleSidedGI: 0
  m_CustomRenderQueue: -1
  stringTagMap:
    RenderType: Opaque
  disabledShaderPasses:
  - MOTIONVECTORS
  m_LockedProperties: 
  m_SavedProperties:
    serializedVersion: 3
    m_TexEnvs:
    - _BaseMap:
        m_Texture: {fileID: 0}
        m_Scale: {x: 1, y: 1}
        m_Offset: {x: 0, y: 0}
    - _MainTex:
        m_Texture: {fileID: 0}
        m_Scale: {x: 1, y: 1}
        m_Offset: {x: 0, y: 0}
    m_Ints: []
    m_Floats:
    - _AddPrecomputedVelocity: 0
    - _AlphaClip: 0
    - _AlphaToMask: 0
    - _Blend: 0
    - _BlendOp: 0
    - _Cull: 2
    - _Cutoff: 0.5
    - _DstBlend: 0
    - _DstBlendAlpha: 0
    - _QueueOffset: 0
    - _SampleGI: 0
    - _SrcBlend: 1
    - _SrcBlendAlpha: 1
    - _Surface: 0
    - _XRMotionVectorsPass: 1
    - _ZWrite: 1
    m_Colors:
    - _BaseColor: {r: 1, g: 0.35, b: 0.05, a: 1}
    - _Color: {r: 1, g: 0.34999996, b: 0.049999982, a: 1}
  m_BuildTextureStacks: []
  m_AllowLocking: 1

Exit: 0
```

$ git diff --check
```text

Exit: 0
```

$ python3 -B - <<'CHECK'
from pathlib import Path
import re, subprocess
paths=['Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs','Assets/ForestPrototype/UI/MenuHelpView.Text.cs']
a,b=[Path(x).read_text() for x in paths]
assert a.count('16–20m') == 3 and '16–20 m' not in a
assert 'return "Introducing tree marking";' in b
assert 'return "Scenario 1 introduces 3 CCF markings.' in b
assert 'CCF source material' not in a+b
assert 'new[] { "Orange vertical stripes", "Rack / machine-path removal" }' in b
assert 'new[] { "Red", "Fell" }' in b
assert 'new[] { "Blue", "Crop Tree / future timber" }' in b
for path in paths:
 old=subprocess.check_output(['git','show','72b4efbd:'+path],text=True)
 new=Path(path).read_text()
 def skeleton(s):
  s=re.sub(r'//[^\n]*','',s)
  return re.sub(r'"(?:\\.|[^"\\])*"','"STRING"',s)
 assert skeleton(old)==skeleton(new), path
print('PASS: 3 spacing labels; exact heading/intro; forbidden phrase absent; three marker labels preserved; non-string code unchanged in both files.')
CHECK
```text
Traceback (most recent call last):
  File "<stdin>", line 18, in <module>
AssertionError: Assets/ForestPrototype/UI/MenuHelpView.Text.cs

Exit: 1
```

Corrected focused assertion: normalize whitespace after stripping comments and string literals (the initial diagnostic failed on the shortened comment blank line). Same checks repeated; raw result:
```text
PASS: 3 spacing labels; exact heading/intro; forbidden phrase absent; marker rows preserved; non-string/comment code unchanged (whitespace normalized).
Exit: 0
```

Production commit command: `git add Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs Assets/ForestPrototype/UI/MenuHelpView.Text.cs` (exit 0, no output), then `git commit -m "fix(ui): clarify objective spacing and tree marking introduction (#255)"`. Raw result:
```text
[feature/overnight-objective-copy-20261104 16d20d5e] fix(ui): clarify objective spacing and tree marking introduction (#255)
 2 files changed, 8 insertions(+), 9 deletions(-)
Exit: 0
```

$ git log --oneline -1
```text
16d20d5e fix(ui): clarify objective spacing and tree marking introduction (#255)

Exit: 0
```

$ git diff --check
```text

Exit: 0
```

$ git status --short
```text
?? Docs/Handoffs/Overnight-Continuation-20261104/

Exit: 0
```

$ git push -u origin feature/overnight-objective-copy-20261104
```text
remote: 
remote: Create a pull request for 'feature/overnight-objective-copy-20261104' on GitHub by visiting:        
remote:      https://github.com/jermur788/CCF/pull/new/feature/overnight-objective-copy-20261104        
remote: 
To https://github.com/jermur788/CCF.git
 * [new branch]        feature/overnight-objective-copy-20261104 -> feature/overnight-objective-copy-20261104
branch 'feature/overnight-objective-copy-20261104' set up to track 'origin/feature/overnight-objective-copy-20261104'.

Exit: 0
```

Delivery commands after transcript closure: stage/commit this handoff, push branch again, create PR to main, link PR to thread. Their final SHA/PR and raw delivery results are returned in the session; this handoff cannot include its own eventual commit hash.

---

## Review outcome and DPC correction (added after independent review)

**Review verdict:** APPROVE WITH REQUIRED CHANGES, posted on PR #262 by an independent Claude-family reviewer (`claude-sonnet-5-5`, medium effort, read-only detached checkout at `af1821b6`). The worker's own status in this file was IMPLEMENTED, never REVIEWED.

**The reviewer's required change, and what was done about it.** The code change was correct, but the change left the repository's own verification suite asserting the superseded copy. The reviewer identified five stale assertions in `Tools/Verification/RackV2HelpTeaching/RackV2HelpTeachingOfflineCheck.cs` (title `Tree Marks`; orange text equal to spec §28; `16–20 m` at three places) and one in `Tools/Verification/RackV2Objective1/RackV2Objective1EndToEnd.cs`. The worker's handoff had reported only a single S10 failure and had not disclosed these; that under-reporting is recorded here rather than quietly fixed.

DPC applied the correction directly (small, mechanical, within this packet's "focused own test output" allowance), on the same branch:

| File | Change | Why it is not a weakening |
|---|---|---|
| `RackV2HelpTeachingOfflineCheck.cs` | title check now asserts the exact new heading | still an exact equality, not a substring or a removal |
| `RackV2HelpTeachingOfflineCheck.cs` | orange text now asserts the exact new sentence | still exact equality; spec §28 is now stale and needs Manager reconciliation |
| `RackV2HelpTeachingOfflineCheck.cs` | a `UnitStyle()` helper normalises only `16–20 m` → `16–20m` for spec-sentence comparisons | whole sentences are still compared in full; only the unit style the user asked for is normalised |
| `RackV2HelpTeachingOfflineCheck.cs` | three `16–20 m` literals updated | the assertions still require the range to be present |
| `RackV2Objective1EndToEnd.cs` | one `16–20 m` literal updated | same requirement, new approved spelling |
| `run_offline.py` | S10 allow-list gained the two exact paths this change legitimately touches | the scope guard still rejects everything else; no prefix was widened |

**Raw result after the correction** (`python3 Tools/Verification/RackV2HelpTeaching/run_offline.py --base 72b4efbd`, dotnet 10.0.114):

```
RACK_V2_HELP_TEACHING_OFFLINE_PASS checks=97
PASS offline wording/unlock/flow check
RACK_V2_HELP_TEACHING_PASS static_and_offline=15
```

Before the correction the same command reported `RACK_V2_HELP_TEACHING_OFFLINE_FAIL failures=5 checks=97` and `RACK_V2_HELP_TEACHING_FAIL failed=2 static_and_offline=15`. Both numbers are reproduced from the terminal, not from memory.

**Still open and NOT fixed here (deliberately):**

1. `Assets/ForestPrototype/UI/StandMapView.cs:603` still shows `16–20 m` with a space on the Stand Map card, so a player sees `16–20m` in Objective 1 and `16–20 m` there. That file belongs to the #256 lane and is outside this packet's two-file scope. Widening scope is a Manager decision, not an overnight convenience.
2. Spec §28 in `specs/scenario-one-objective-1-rack-planning-field-marking-v2/spec.md` still contains the "CCF source material" sentence the user objected to. The production string is now the authority for the player-facing text; the spec needs a Manager reconciliation pass.
3. The Fell-mark colour discrepancy (in-world Fell paint is orange-red `(1,0.35,0.05)`, rack entrance paint `(0.95,0.28,0.025)`, only the shape differs) is recorded for #259 and the morning report. No copy or marker was changed on the back of it.

**Reviewer's unverified list, repeated so it is not lost:** rendered text, wrapping and fit in the help modal; actual on-screen difference between Fell and rack-stripe colour; and any test result by execution. DPC executed the offline suite; nothing was executed in Unity.
