# Current Development Milestone

## Purpose

Define current scope. Verified implementation belongs in Overview; accepted decisions in Decision Log; temporary ownership in live coordination.

## Milestone

**Multi-Agent Development Setup & Shared-Context Refresh**

Create one reliable context and safe workflow for ChatGPT, OpenAI/OpenCode, Claude Project chat, Claude Desktop/MCP and Claude Code/cloud without divergent truth or concurrent Unity writes. Finish this enabling milestone before routine parallel implementation.

## In scope

1. Canonical context in `Docs/Project/`: Game Brief, Decision Log, Current Milestone, Unity Project Overview, AgentWorkflow and supporting Research Index.
2. Canonical `instructions/`: shared core, ChatGPT role and Claude role.
3. Live coordination in `Docs/Project/coordination/active-tasks.md`, outside context snapshots.
4. Root `AGENTS.md` and `CLAUDE.md` entry points.
5. Deterministic `Tools/ProjectContext/generate_project_instructions.py`: role header + marked workflow authority + shared core; full context SHA/source paths; generated composites outside canonical paths in ignored `Build/ProjectContext/`.
6. Port valid root `AI_Instructions` rules, then retire it.
7. Keep tracked `.vscode/settings.json` unchanged; local differences use user settings/external `.code-workspace`.
8. Drive as a read-only, one-way Git mirror: complete set at one SHA, source/context/timestamp stamps, refreshed project/chat copies.
9. Establish/verify separate integration/main, OpenAI/OpenCode and Claude Desktop writable worktrees.
10. One `Library` per Unity worktree.
11. Minimum smoke gate independently in worker worktrees, including harness cleanup.
12. Record verified local paths, branches, HEADs and commands in Overview.
13. Commit canonical sources on a clean setup task branch, run the real generator at that full context SHA, then review actual composites/manifest before integrating to main.

The current task packet authorises the repository migration/generator slice only; worktree Unity smoke runs, permanent Drive publication and project attachment refresh remain separately assigned milestone gates.

## Existing technical stack frozen during setup

Exact snapshot:

```text
0fc92ca main
  └─ 3b71e4b save-hardening-and-growth-cache
      └─ 8aede39 rng-versioning
          └─ 457c897 batch-recompute
              └─ a31ec62 edge-bias-diagnostic
```

Commit SHAs, not mutable branch names, define the anchors. This is pre-existing work, not new setup scope.

- Do not add commits to, merge to main or discard this stack during setup.
- Preserve it for a dedicated review/integrate/park decision.
- Before local operations involving its worktrees, Overall Manager/user must confirm no worker is still writing them.
- Save/load, RNG, ecology and frozen-reference changes require independent review and regression gates before eventual integration.

## Completion checks

- All agreed context, instruction-source, research-index and entry-point paths are committed.
- Live coordination exists and is excluded from context-commit semantics.
- Committed generator refuses dirty/untracked sources, uses full context SHAs, writes hashes/mirrors and reproduces both composites deterministically from committed sources.
- Valid legacy instructions are ported before retirement.
- Shared VS Code settings unchanged.
- Drive mirrors/composites match one context commit and stamps; platform project copies refreshed, with AgentWorkflow directly available.
- Stale review copies archived.
- Distinct OpenAI/Claude worktrees and separate Libraries; worker smoke gates pass.
- No temporary verification script or generated `.meta` remains in Assets.
- Ownership and actual integration worktree are known; frozen stack preserved.
- Claude/assigned reviewer checks real clean-commit generator output with no unresolved MUST-fix workflow issue.

## Live coordination

`Docs/Project/coordination/active-tasks.md` is maintained by Overall Manager/assigned integrator on integration/main coordination branch. Workers read it at task start; packets carry locks for workers without live access. Lock-only changes do not regenerate context mirrors/attachments.

## Explicit exclusions

No simultaneous writes to one Unity worktree, broad gameplay refactor, frozen-stack changes, Reference Future changes, Git LFS/history rewrite, Drive-as-canonical editing, or bundled browsing/understorey implementation.

## Next queued gameplay milestone

**Regeneration Bottlenecks v1 — current direction, not active.** Likely browsing/shared response, accepted protection, causal understorey, competition/concealment, spot control/regrowth and deterministic persistence. Exact scope requires its own accepted packet.
