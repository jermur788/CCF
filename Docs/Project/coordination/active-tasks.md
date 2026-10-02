# Active Task Coordination

Live coordination, **outside the project context commit**. Workers read this file at task start; packets carry locks for workers without live Git access. Updating locks does not regenerate mirrors/attachments.

Only Overall Manager/assigned integrator maintains live locks on integration/main coordination branch. This initial task-branch file is created by the explicit migration packet; it is not permission for workers to rewrite other agents' live locks.

## Active tasks / temporary locks

| Task | Primary | Branch | Worktree | Owned files/subsystems | Shared restriction | Status |
|---|---|---|---|---|---|---|
| Project-context migration | OpenCode, primary implementation by task packet | task/project-context-migration | /home/jer/CCF-project-context-migration | Authorised Docs/Project files, AGENTS.md, CLAUDE.md, generator, AI_Instructions retirement | No gameplay/Unity/meta/settings changes; frozen stack untouched | Implemented on task branch; generator gates passed; ready for independent review |
| Local multi-agent setup | Unassigned | TBD | local machine | Worktree/tool setup and smoke verification | Preserve unrelated work; no gameplay changes | Waiting for assignment |
| Existing technical stack | Frozen | Exact SHAs in Current Milestone | Existing worktrees; inspect live ownership | Save/RNG/ecology/diagnostic stack | No changes or merge in migration packet | Parked pending dedicated review decision |
