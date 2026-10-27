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

## Implementation support

With project access: inspect first, make only authorised changes, preserve unrelated work, verify behaviour appropriately. Distinguish GitHub-only access from local worktree/Unity access: a GitHub edit can establish repository state, documentation or coordination, but it does not prove local import, compilation, runtime behaviour or Unity rendering. Without project access: give precise version/configuration-matched instructions, identify unverified points, exact editor/test steps and expected behaviour.

## Development approach

Small playable improvements; simplest maintainable solution for the milestone; no speculative infrastructure. Preserve reasonable future multiplayer/growth/testing options without building unnecessary systems.

Separate ecology rules/data from graphics, controls or networking when beneficial. Explain dependencies/assets/packages before adding them and consider compatibility, maintenance, licensing, contributor access and simpler alternatives. Match actual Unity, pipeline and packages.

## Ecology and simulation

Ground ecological facts in credible sources, especially forestry, species suitability, recruitment, canopy and biome-specific behaviour. Distinguish empirical measurement, management guidance, inference, simulation abstraction and calibration.

Planting is not always an ecological improvement; consider site suitability and open habitats. Management should visibly trade immediate needs/timber against recruitment, habitat and resilience. Presentation must not invent causal ecology.

## Game quality and verification

Correct code alone is insufficient: check feedback, pacing and visible consequences. Compilation does not prove behaviour. Prioritise tests for important simulation rules, persistent state, deterministic/reference behaviour and interactions; avoid unnecessary tests for minor reversible changes.

## Multi-agent work

No permanent disciplinary ownership by model family. Assign by capability/tools/context/availability/quality. Follow AgentWorkflow for authority, context snapshots, roles/locks, worktrees, handoffs, integration states and Drive synchronization.

The Overall Manager may perform GitHub-native coordination and bounded canonical documentation/source maintenance directly when connected write tools are available. Workers should not consume another agent merely to update live locks, publish a handoff or make a Manager-authorised text-only repository change that the Manager can safely complete directly.

Do not infer integration/verification from conversation alone. Drive mirrors are read-only collaboration copies; change canonical Git sources then regenerate.

At substantial task start, check the context stamp and the live GitHub task-lock record when accessible. If live-linked context changes and the locked version cannot be retrieved, stop and notify Overall Manager. Static attachments may continue when stamps match the packet.

## Handover

Report what changed/was decided, actual verification, unresolved points, authoritative file needing update and next step. Implementation uses AgentWorkflow's detailed handoff format.

Every substantive implementation, qualification, review, integration or research handoff must also have a GitHub-tracked Markdown copy. External `/media/...` evidence can hold large logs, screenshots and raw artifacts, but a local-only handoff is not administratively complete.
