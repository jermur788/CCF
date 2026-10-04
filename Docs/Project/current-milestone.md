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

The repository migration/generator slice is integrated in `main` (verified 2026-10-04). Worktree Unity smoke runs, permanent Drive publication and project attachment refresh remain separately assigned milestone gates.

## Existing technical stack (frozen during setup; now integrated)

Historical snapshot as frozen at setup start:

```text
0fc92ca main
  └─ 3b71e4b save-hardening-and-growth-cache
      └─ 8aede39 rng-versioning
          └─ 457c897 batch-recompute
              └─ a31ec62 edge-bias-diagnostic
```

Commit SHAs, not mutable branch names, define the anchors. This is pre-existing work, not new setup scope.

Status (2026-10-02): resolved by D-039. The stack is in `main` via merge `8ae7a25`. The combined ecology package was integrated on top of it and verified at gameplay head `b1e6c51`: save v14, calibrated lifecycle `BFC55473C1506067`, Reference Future v1 Year 100 `7AD177B3CC2F73C7`. See the Unity Project Overview. The freeze no longer applies. Future save/load, RNG, ecology and frozen-reference changes still require independent review and regression gates before integration.

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
- Ownership and actual integration worktree are known; the formerly frozen stack is integrated and verified (D-039).
- Claude/assigned reviewer checks real clean-commit generator output with no unresolved MUST-fix workflow issue.

## Live coordination

`Docs/Project/coordination/active-tasks.md` is maintained by Overall Manager/assigned integrator on integration/main coordination branch. Workers read it at task start; packets carry locks for workers without live access. Lock-only changes do not regenerate context mirrors/attachments.

## Explicit exclusions

No simultaneous writes to one Unity worktree, broad gameplay refactor, frozen-stack changes, Reference Future changes, Git LFS/history rewrite, Drive-as-canonical editing, or bundled browsing/understorey implementation.

## Scenario 1 status (2026-10-04)

**FUNCTIONALLY COMPLETE — final asset/presentation acceptance pending.**

Integrated on `main` and verified (see the Unity Project Overview, **Integrated Scenario 1**):
- functional implementation, save schema v15;
- browsing default 0.2;
- shelters;
- contractor/landowner execution;
- timber yield and economy;
- completion verification;
- Plantation02 Sitka presentation, corrected forest-floor presentation and compact brash presentation.

Acceptance record:
- the automated completion gate passes;
- the integration regression passed 27/27;
- post-merge gates on `main` pass;
- the user's interactive Scenario 1 smoke passed, with the starting-stand and compact-brash corrections accepted in play;
- no performance problem was reported during that smoke.

Still pending: final asset/presentation acceptance. Astra ForestFloorV1 remains an external candidate delivery and is not automatically adopted.

Next task: **Scenario 1 Final Asset Acceptance Review**.

Deferred, outside the Scenario 1 minimum unless accepted elsewhere:
- deer fencing as a production feature;
- production understorey ecology;
- vegetation-control simulation expansion;
- storms/windthrow;
- adult suppression mortality;
- browse-history/form-damage persistence;
- fence deterioration;
- pruning-quality premium;
- broadleaf market;
- manual forestry;
- co-op.

Previously recorded presentation constraint: ring-barked, windthrow-root and defect assets stay unspawned until matching simulation state exists (D-020).

Untriaged notes from presentation handoffs, for the asset review:
- dry brash reads subtly at player distance;
- the track/clearing ground material is still a flat colour.

## Regeneration Bottlenecks v1

Browsing & Protection v1 (shared browsing response, accepted shelter protection, deterministic restore) shipped as part of Scenario 1. The other candidate elements remain queued direction, not active, and each needs its own accepted packet:
- causal understorey;
- competition/concealment;
- spot control/regrowth.
