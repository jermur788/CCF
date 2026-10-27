# Manager GitHub routing update — handoff

**Date:** 2026-10-27
**Manager:** ChatGPT Game Dev — Overall Manager
**Repository:** `jermur788/CCF`
**Context commit:** `afb70d881ad2256573b5f25faccb07571d5a5f30`

## Verdict

`MANAGER GITHUB ROUTING SOURCES UPDATED — CONTEXT PUBLICATION REFRESH REQUIRED`

## Why this changed

The Overall Manager now has connected GitHub read/write capability in this project. Routine GitHub-native management no longer needs to be delegated to a local implementation worker merely because it is a repository change.

This changes routing, but it does not grant the Manager a local Unity worktree, Unity Editor, Blender, build environment or runtime verification capability.

## Canonical files changed

1. `Docs/Project/instructions/chatgpt-role.md`
   - Manager directly handles GitHub-native coordination, canonical documentation/spec/research/decision changes, handoff publication and small inspection-verifiable text/source edits.
   - Production Unity/C#, serialized assets, local integration/builds and runtime verification remain routed to capable workers.
   - GitHub handoffs are required for substantive results.

2. `Docs/Project/instructions/shared-project-instructions.md`
   - Workers must refresh the live GitHub `active-tasks.md` before claiming a missing registration/lock.
   - GitHub-only access is distinguished from local/Unity verification.
   - Local-only substantive handoffs are administratively incomplete.

3. `Docs/Project/instructions/claude-role.md`
   - Claude workers must not rely on stale dirty local coordination when live GitHub is available.
   - Overall Manager owns direct live-lock maintenance unless another coordinator is explicitly assigned.
   - Substantive handoffs require a GitHub-tracked copy.

4. `Docs/Project/AgentWorkflow.md`
   - Adds direct Manager GitHub capability and routing policy.
   - Revises `FREE HERE` and `SOL/CODEX` boundaries.
   - Defines GitHub-native pre-write checks.
   - Makes live GitHub task-lock refresh mandatory before a missing-lock STOP.
   - Adds GitHub handoff path/commit fields and durable handoff publication rules.
   - Updates Manager continuation/workflow to use direct GitHub management before spending a worker.

5. `AGENTS.md`
   - Root entry point now tells agents to refresh live GitHub locks, recognizes Manager GitHub-native edits, and requires GitHub-tracked substantive handoffs.

## Commits

- ChatGPT role: `587c2eb85051223be3f2056c4102af1b79a34926`
- Shared instructions: `5efcc49cb4898cdc0c52023f607dde89616d169c`
- Claude role: `fb2f1276ce23542a5d78665100b4a0c7a7af06a6`
- AgentWorkflow: `64da8c75a68d3f05c1347c71dea6f0d24827d872`
- AGENTS / resulting context commit: `afb70d881ad2256573b5f25faccb07571d5a5f30`

## Routing after this change

### Overall Manager / FREE HERE

Use direct GitHub tools for:

- live task/file-lock registration;
- repository/branch/commit/PR inspection;
- canonical documentation and specification maintenance;
- Decision Log / Research Index / milestone text updates when authorised;
- durable handoff publication;
- small text/source changes whose correctness is inspection-verifiable and which do not require local Unity/build verification.

### Local implementation workers

Use SOL/CODEX/Terra/other capable local workers for:

- Unity/C# production implementation;
- serialized assets/scenes/prefabs;
- local worktrees and integration;
- compiler/Unity/runtime verification;
- difficult debugging;
- builds and rendered review.

### Independent review/research

Continue using Claude/Deep Research when independent reasoning, review or evidence work is the cheapest capable route.

## No gameplay/product change

This update changes project workflow and routing only. It does not alter Scenario One gameplay, forestry evidence, simulation, save behaviour, milestones or accepted product decisions.

## Required follow-up

Because canonical context/instruction sources changed, the project-context publication pipeline must be rerun from context commit `afb70d881ad2256573b5f25faccb07571d5a5f30`:

1. run `Tools/ProjectContext/generate_project_instructions.py` from a clean checkout at that context commit;
2. regenerate the full mirror set and both composites at the same SHA;
3. refresh permanent Drive mirrors;
4. refresh/re-attach/replace project knowledge/attachments that still point at the previous context;
5. verify all generated stamps use the new full SHA;
6. do not manually edit generated mirror/composite files.

The current Manager session has GitHub write access but does not claim the local generator/Drive publication pipeline was run.

## Dashboard

This workflow change has not been verified in the published dashboard.

`DASHBOARD SYNC: PENDING`

`Published dashboard reflects this task: NO`
