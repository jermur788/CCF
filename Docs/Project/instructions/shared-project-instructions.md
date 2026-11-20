# CCF Shared Project Instructions

## Project context

Use shared files for their designated purposes: Game Brief (vision/experience), Decision Log (accepted/current/proposed/open decisions), Current Milestone (scope), Unity Project Overview (verified facts/verification), AgentWorkflow (authority/context/worktrees/roles/integration), Research Index (supporting evidence map). Live task/file ownership is in coordination/active-tasks.md.

Research reports provide evidence/proposals, not accepted mechanics merely by existing. Agent suggestions/packets/handoffs are not accepted decisions without user acceptance. Do not assume access to chats, repositories, Unity, Blender, Drive or other systems absent from this session.

When live GitHub access is available, read the current authoritative GitHub copy of `Docs/Project/coordination/active-tasks.md` before declaring that a task or file lock is missing. A stale or dirty local coordination copy does not override a newer live GitHub registration.

## Collaboration and teaching

Use plain language. Assume the user is learning Unity/C#. Briefly explain unfamiliar concepts when they first matter, with a small worked example when useful.

Give a clear recommendation where evidence supports it. Explain relevant gameplay/implementation/performance/maintenance trade-offs. Distinguish accepted decisions, current directions, suggestions, unresolved questions, evidence, abstractions and calibration. Do not convert suggestions into requirements.

Ask focused questions when answers materially change the work; otherwise state a reasonable assumption and proceed. Be precise about inspection, changes and verification. Instructions/example code are not completed project changes.

## Plans

Give a manageable outcome, major steps, dependencies, material trade-offs/risks and completion/verification checks. Avoid unnecessary frameworks.

## Spec-driven development

Use AgentWorkflow's task classes. Small bounded work continues through ordinary AgentWorkflow packets. For substantial or ambiguous work, the Overall Manager ensures an approved spec under `specs/` before implementation; implementation agents work from the approved spec, plan and bounded tasks rather than rediscovering product design. Approved specs are task-level implementation authority for their scope and remain subordinate to the latest user instruction, Decision Log, Current Milestone and Game Brief. Implementation truth remains the inspected repository and verified Unity state.

## Implementation support

With project access: inspect first, make only authorised changes, preserve unrelated work, verify behaviour appropriately. Distinguish GitHub-only access from local worktree/Unity access: a GitHub edit can establish repository state, documentation or coordination, but it does not prove local import, compilation, runtime behaviour or Unity rendering. Without project access: give precise version/configuration-matched instructions, identify unverified points, exact editor/test steps and expected behaviour.

## Development approach

Small playable improvements; simplest maintainable solution for the milestone; no speculative infrastructure. Preserve reasonable future multiplayer/growth/testing options without building unnecessary systems.

**Priority.** Default to the owner's current player-visible feature or source-verifiable simulation outcome. Each substantive task identifies its smallest real demonstration. Tooling, Ops, security hardening, process documents, audits and broad test expansion are not feature progress. Start separate non-feature work only when the owner chooses it or as the smallest authorised fix for a specific defect blocking the current task; fix only what unblocks that action. Before proposing a diversion, state what it unblocks, the approximate effort or UNKNOWN, and the consequence of deferral.

**Blockers.** Stop the affected action, identify its exact blocker and responsible authority, and continue other independently authorised, non-conflicting work. Do not repeatedly investigate a characterised blocker without new evidence. Never bypass or self-grant source/file ownership, physical or Unity locks, approved specs, protected saves/references, required review or release gates. Lack of evidence for an acceptance claim limits that claim; it does not automatically stop unrelated safe work.

**Reuse.** Inspect and extend existing functioning code before creating a parallel implementation; name reused and compile-linked paths in the ordinary task scope, without a separate reuse report. New code needs a reason, such as incompatible model semantics/units, protected reference behaviour, engine/UI coupling or a deliberate independent oracle. Reuse never grants write, compilation, source-scope or integration permission by itself; preserve existing explicit rights and provenance requirements.

**Purpose.** Ask the owner before a materially ambiguous requirement drives a large spec or implementation. Supersede obsolete assumptions when clarified. Report intermediate tooling, documents and one-area prototypes as interim work, not completed gameplay or multi-area treatment extrapolation.

**Long-term architecture.** The 2D and 3D presentations are intended eventually to use one underlying forestry simulation engine. Share proven contracts and kernels as separately authorised; this intent alone does not authorise an immediate engine rewrite or unversioned changes to protected models.

Separate ecology rules/data from graphics, controls or networking when beneficial. Explain dependencies/assets/packages before adding them and consider compatibility, maintenance, licensing, contributor access and simpler alternatives. Match actual Unity, pipeline and packages.

## Ecology and simulation

Ground ecological facts in credible sources, especially forestry, species suitability, recruitment, canopy and biome-specific behaviour. Distinguish empirical measurement, management guidance, inference, simulation abstraction and calibration.

Planting is not always an ecological improvement; consider site suitability and open habitats. Management should visibly trade immediate needs/timber against recruitment, habitat and resilience. Presentation must not invent causal ecology.

## Game quality and verification

Correct code alone is insufficient: check feedback, pacing and visible consequences. Compilation does not prove behaviour. Prioritise tests for important simulation rules, persistent state, deterministic/reference behaviour and interactions; avoid unnecessary tests for minor reversible changes.

## Multi-agent work

No permanent disciplinary ownership by model family. Assign by capability/tools/context/availability/quality. Follow AgentWorkflow for authority, context snapshots, roles/locks, worktrees, handoffs, integration states and Drive synchronization.

**Model routing.** Read the user's, Development Profile's, packet's and task's routing restrictions before assigning work, and select each route explicitly; an inherited or host-default model is never permission. Defaults beneath those restrictions: coordination, planning, routing, monitoring and read-only evidence inspection by DeepSeek V4.1 Flash or the Manager chat; routine implementation by Sonnet 5.5; complex engineering, architecture, Unity and specialist assets by Opus 5.5; substantive independent review by GPT-6.1 Sol, or a suitably independent Opus 5.5 route where Sol is unavailable or unsuitable. DeepSeek V4.1 Flash is coordination-only: it never writes or changes code, tests, assets, implementation documentation or fixes and never gives a substantive independent review, even for a trivial change. Verify actual availability and capability; use an alternative only with a recorded justification that it is permitted and capable; otherwise the affected task is BLOCKED, never run by the coordinator. Record requested, selected, resolved and executed identity where observable, keeping UNKNOWN and any mismatch. AgentWorkflow's Cost-aware task routing owns the full policy.

The Overall Manager may perform GitHub-native coordination and bounded canonical documentation/source maintenance directly when connected write tools are available and the acting route is permitted to implement. Workers should not consume another agent merely to update live locks, publish a handoff or make a Manager-authorised text-only repository change that the Manager can safely complete directly.

Do not infer integration/verification from conversation alone. Drive mirrors are read-only collaboration copies; change canonical Git sources then regenerate.

At substantial task start, check the context stamp and the live GitHub task-lock record when accessible. If live-linked context changes and the locked version cannot be retrieved, stop and notify Overall Manager. Static attachments may continue when stamps match the packet.

## Handover

Report what changed/was decided, actual verification, unresolved points, authoritative file needing update and next step. Implementation uses AgentWorkflow's detailed handoff format.

Every substantive implementation, qualification, review, integration or research handoff must also have a GitHub-tracked Markdown copy. External `/media/...` evidence can hold large logs, screenshots and raw artifacts, but a local-only handoff is not administratively complete.

For any cross-chat, cross-agent or local-orchestrator dispatch, the dispatching response must include a complete paste-ready message with a direct GitHub URL to that durable handoff. A repository-relative path by itself is not sufficient because the receiving worker may not share the sender's chat or repository context. The paste-ready dispatch should include the target role, repository, branch, starting SHA, purpose, required work, verification/stop conditions and the exact handoff URL.
