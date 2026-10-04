# Active Task Coordination

Live coordination, **outside the project context commit**. Workers read this file at task start; packets carry locks for workers without live Git access. Updating locks does not regenerate mirrors/attachments.

Only Overall Manager/assigned integrator maintains live locks on integration/main coordination branch. This initial task-branch file is created by the explicit migration packet; it is not permission for workers to rewrite other agents' live locks.

## Active tasks / temporary locks

| Task | Primary | Branch | Worktree | Owned files/subsystems | Shared restriction | Status |
|---|---|---|---|---|---|---|
| Project-context migration | OpenCode, primary implementation by task packet | task/project-context-migration | /home/jer/CCF-project-context-migration | Authorised Docs/Project files, AGENTS.md, CLAUDE.md, generator, AI_Instructions retirement | No gameplay/Unity/meta/settings changes; frozen stack untouched | Integrated: `42db4e9` is in `main` (verified 2026-10-04); generator runs from clean main context commits. Drive in-place publication and project attachments still pending |
| Local multi-agent setup | Unassigned | TBD | local machine | Worktree/tool setup and smoke verification | Preserve unrelated work; no gameplay changes | Waiting for assignment |
| Existing technical stack | — | main | — | Save/RNG/batch/edge stack | — | Cleared 2026-10-02: in main via `8ae7a25`; verified with ecology package (D-039 resolved) |
| Combined ecology package integration | Claude (integrator) | main | /home/jer/CCF-main | Ecology/juvenile/mortality/save v14/Reference Future verification | — | Completed and cleared 2026-10-02: gameplay head `b1e6c51`, verified on main; context `d5a3128` |
| Post-integration context cleanup + push | Claude (integrator) | main | /home/jer/CCF-main | Docs/context/coordination only | No production changes | Completed and cleared 2026-10-02; Drive in-place publication and project attachments still pending |
| Scenario 1 integration & completion (final integrator) | Claude (integrator) | integration/scenario-one-complete → main | /home/jer/CCF-main | Scenario 1 ecology/browsing/economy/save v15/presentation integration | — | Completed and cleared 2026-10-04: fast-forwarded to main at `dc975e4`; post-merge gates PASS; user interactive smoke PASS. Status: functionally complete |
| Scenario 1 Final Asset Acceptance Review | Unassigned | TBD | TBD | Presentation/asset acceptance only | No ecology/economy/save changes without a packet | Next task; not started |
