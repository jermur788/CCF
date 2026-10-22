# CCF Context and Artifact Manifest

This is a human-readable map of canonical project context and artifact locations. It does not replace the canonical files or the generated context snapshot. The generated manifest for a full Git context SHA is `Build/ProjectContext/context-manifest.json`; do not hand-edit generated output.

## Core project context

| Path | Purpose | Normal loading |
|---|---|---|
| `Docs/Project/game-brief.md` | Long-term vision and intended player experience | Read for product/direction questions. |
| `Docs/Project/decision-log.md` | Accepted, current, proposed, deferred and open decisions | Read for authority and settled decisions. |
| `Docs/Project/current-milestone.md` | Current scope | Read at task start. |
| `Docs/Project/unity-project-overview.md` | Verified implementation facts and local verification guidance | Read for implementation truth. |
| `Docs/Project/AgentWorkflow.md` | Authority, task classes, packets, roles, gates and integration | Read before substantial work. |
| `Docs/Project/research-index.md` | Supporting ecology/research map | Read when research context is relevant. |
| `Docs/Project/instructions/*` | Shared and role instruction sources | Read the assigned role and the shared core. |
| `Docs/Project/coordination/active-tasks.md` | Live ownership, locks and task state | Read live at task start; excluded from the context commit. |

## Feature specifications

`specs/` is the canonical location for feature specifications, plans, bounded tasks and acceptance checklists. `specs/README.md` owns the artifact and status conventions.

- For a `SPEC REQUIRED` task, load the relevant `specs/<feature>/spec.md`, then `plan.md` and the assigned entry in `tasks.md`.
- Load a spec only when it is active for the task or its history is directly relevant. Do not load every spec into every context.
- Completed and `SUPERSEDED` specs are historical evidence, not standing instructions.
- Templates live in `specs/_template/`; they are not approved specifications.

## Generated and derived context

`Build/ProjectContext/` contains generated instruction composites, stamped mirrors and `context-manifest.json` for a full Git context SHA. Generated output is not hand-maintained canonical text. Regenerate it through `Tools/ProjectContext/generate_project_instructions.py`; do not treat a dirty preview as a committed context snapshot.

## Loading rule

Load the smallest context that answers the task:

1. the core project context required by the role;
2. the active approved spec, plan and assigned task for `SPEC REQUIRED` work;
3. only the research, evidence or historical verification that the task actually needs.

Do not indiscriminately load all specifications, research or historical evidence into every context.
