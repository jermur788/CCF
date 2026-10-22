# CCF Feature Specifications

## Purpose

`specs/` holds feature-level specifications for work that is substantial or ambiguous enough to need explicit product intent, technical planning and bounded implementation tasks before coding. It is a canonical project artifact location and a project-history record. It is not a second Decision Log and does not replace the Game Brief, Current Milestone, Unity Project Overview or live coordination.

`Docs/Project/AgentWorkflow.md` owns the workflow rules. This file owns the artifact and status conventions.

## When a spec is required

The Overall Manager classifies work before production implementation.

**PACKET ONLY** — ordinary AgentWorkflow packet, no spec required:

- small bugs;
- bounded fixes;
- verification repairs;
- simple wording/content changes;
- straightforward implementation that fits one clear packet.

**SPEC REQUIRED** — normally use SDD:

- new user/player workflows;
- cross-system features;
- persistence/data architecture;
- save-policy changes;
- security/write-back behaviour;
- substantial UX;
- major forestry/gameplay systems;
- multi-step or multi-agent features;
- work where ambiguity is likely to waste implementation tokens.

The Manager may upgrade a task after inspection. A worker that discovers a PACKET ONLY task has become substantially ambiguous stops and reports; it does not invent a spec or continue on an unapproved assumption.

## Normal artifact structure

```text
specs/<feature>/spec.md
specs/<feature>/plan.md
specs/<feature>/tasks.md
specs/<feature>/checklists/requirements.md
```

`<feature>` is a short lowercase slug. Use the reusable templates under `specs/_template/`. A feature may omit or combine a file only when it is genuinely not applicable, but it must not silently drop acceptance criteria, stop conditions or the authority statement.

Suggested flow:

```text
product intent
→ SPEC REQUIRED classification
→ spec.md
→ resolved product ambiguity
→ approval
→ plan.md
→ tasks.md
→ bounded implementation
→ verification against the spec
→ integration and history
```

## Authority

Approved feature specs are task-level implementation authority for their scope. They remain subordinate to:

1. latest user instruction;
2. `Docs/Project/decision-log.md`;
3. `Docs/Project/current-milestone.md`;
4. `Docs/Project/game-brief.md`.

Implementation truth remains the inspected repository and verified Unity state. A spec describes intended behaviour; it does not prove that behaviour exists. If a spec conflicts with newer accepted product intent, amend the spec rather than implementing the stale requirement. If implementation reality makes the spec impractical, return to the Manager/spec; do not silently reinterpret it.

The plan and task list are implementation artifacts derived from the approved spec. They do not create product requirements or override the spec.

Only specs in `APPROVED`, `IMPLEMENTING`, `VERIFYING` or `VERIFIED` status authorise implementation. `DRAFT` and `SUPERSEDED` specs do not.

## Status lifecycle

```text
DRAFT → APPROVED → IMPLEMENTING → VERIFYING → VERIFIED
any status → SUPERSEDED
```

- `DRAFT` — being written or clarified; not approved.
- `APPROVED` — accepted product intent and acceptance criteria; may be planned and divided into tasks.
- `IMPLEMENTING` — implementation in progress against approved tasks.
- `VERIFYING` — implementation exists and is being checked for correctness and spec conformance.
- `VERIFIED` — implementation and spec conformance accepted.
- `SUPERSEDED` — retained for history, replaced by a newer spec or decision.

Spec status is separate from task, review and integration states.

## Preservation

Completed and superseded specs remain in the repository as project history. Do not delete them or rewrite their accepted decisions to match a later implementation. If understanding changed:

- amend the active spec while it is still the current authority, or
- create a new spec and mark the old one `SUPERSEDED` with a pointer.

Do not load every historical spec into every future context. Load the active approved spec, plan and assigned task when relevant to the work. Completed specs are evidence/history, not standing instructions.

## Tooling

GitHub Spec Kit is not required. These are plain Markdown artifacts read directly by managers and agents. Spec Kit conventions may be used as a structure/process reference only.
