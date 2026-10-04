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

**FUNCTIONALLY COMPLETE; final presentation acceptance PASS (2026-10-04).** The functional and presentation baseline is integrated. Simulation follow-ups remain open (see below); this is not a permanent closure of Scenario 1 simulation work.

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

Final presentation acceptance (`Docs/Scenario1FinalPresentationAcceptance.md`, merged at `ec938b0`):
- the ten-state rendered review passed: start, marking, pruning, thinning, dispositions, planting, protection, regeneration, later management, save/Reference;
- track and work-clearing now use the existing soil texture;
- inspection/HUD overlap and marking-summary/prompt clipping are fixed;
- Astra ForestFloorV1 was reviewed (manifest verified) and **not adopted**: no demonstrated ground gap;
- dry brash was accepted as subtle but understandable. Evidence limitation: the final review camera was occluded, so acceptance relies on an earlier close-range render;
- performance was acceptable on the observed hardware (editor, GTX 980M: about 20–30 ms median frame). This is not a formal performance certification.

Outstanding simulation decision: the post-Scenario-1 readiness study (`task/post-scenario1-systems-readiness` @ `7f58618`, **not merged**) found that, under the default RNG model 0, planted-juvenile survival and browse rolls are autocorrelated. Shaded planted stock survives far longer than the shared probability intends. The user approved the policy: new Scenario One games use RNG model 1; saves keep their recorded model; saves without the field and Reference Future v1 stay on model 0. **Active follow-on task:** Scenario 1 RNG model-1 default (`task/scenario-one-rng-model1-default`), for Manager integration review.

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

Remaining presentation polish (not blockers):
- moss cushions on the track edge;
- deadwood volume is shown in objectives and history rather than as its own annual-review line.

## Regeneration Bottlenecks v1

Browsing & Protection v1 (shared browsing response, accepted shelter protection, deterministic restore) shipped as part of Scenario 1. The other candidate elements remain queued direction, not active, and each needs its own accepted packet:
- causal understorey;
- competition/concealment;
- spot control/regrowth.
