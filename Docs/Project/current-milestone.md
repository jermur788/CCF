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

The repository migration/generator slice is integrated in `main` (verified 2026-10-04). Permanent Drive mirrors/composites and the ChatGPT Game Dev project instructions/attachments were refreshed and read back after Storm integration at context `58d6b0b483198abcf0208713d9a759f89d90e318`. This consistency cleanup supersedes that snapshot and republishes the corrected context. Independent Unity smoke runs in the worker worktrees remain open; these publication steps do not complete them.

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
- Permanent Drive mirrors/composites match the refreshed context commit and stamps; ChatGPT Game Dev instructions and stamped canonical attachments have been refreshed, with AgentWorkflow directly available.
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
- functional implementation, save schema v15 (v16 from D-047; v17 from D-048);
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

Outstanding simulation decision: the post-Scenario-1 readiness study (`task/post-scenario1-systems-readiness` @ `7f58618`, **not merged**) found that, under the default RNG model 0, planted-juvenile survival and browse rolls are autocorrelated. Shaded planted stock survives far longer than the shared probability intends. Resolved by D-046 (2026-10-05): new Scenario One games use RNG model 1; saves keep their recorded model; saves without the field and Reference Future v1 stay on model 0. Integrated on main at `4eab590`; post-merge gates PASS. Under model 1 the scenario completes in Year 25, stays solvent (minimum cash €5,897.70) and no required objective is newly blocked. **Active follow-on task:** Scenario 1 UI Toolkit redesign (`task/scenario-one-ui-redesign`, not to be merged without Manager/user review).

Deferred, outside the Scenario 1 minimum unless accepted elsewhere:
- deer fencing as a production feature;
- richer production understorey beyond the accepted Scenario One Model 2 bramble/bracken juvenile-survival competition;
- vegetation-control simulation expansion;
- enabling storms for new Scenario One games (dormant core accepted in D-050);
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

## Regeneration Model 1 (2026-10-06)

Integrated on main at `4670e73` (D-047), fast-forward from `402a2b4`. Save schema v16. New Scenario One games use regeneration model 1 (age bands with a sub-threshold accumulator; no seed-independent infill; corrected capacity, origin and promotion representation). Legacy saves and Reference Future v1 stay model 0, which is byte-identical to before.

Scenario 1 under model 1 remains achievable and viable: completed in Year 25 with all 8 objectives (regeneration 51 cells against 3), lowest cash €5,897.73; planting, clearance, shelters and economy gates pass. No objective or economy recalibration.

Open/deferred from this work:
- physical juvenile stems/ha interpretation (undefined);
- model1 retains non-causal habitat cover; new-game Model2 adds separate bramble/bracken survival competition (D-049, below);
- the 0.01 establishment-response cut-off remains an existing calibration rule;
- promotion remains an abstract band → individual handoff;
- harness red gates (Clearance, MenuTutorial, Removal) reproduced identically on `402a2b4`; reclassified in D-048 / pedagogy Workstream F as harness issues, not production failures (see Growth Model 1 below).

## Growth Model 1 (2026-10-07)

Integrated on main at `2cfbabc` (D-048), fast-forward from `3e4ee40`. Save schema v17.

**New Scenario One model stack:** RNG model 1, regeneration model 1, growth model 1, save v17. Legacy saves keep their stored/versioned behaviours; Reference Future v1 stays frozen legacy behaviour (growth 0).

**Growth model 1:**
- **Site:** Irish site Class III; Sitka height follows the Class III site-potential curve. 20.4 m at age 30 is the published Irish anchor [A]; other ages are modelled curve values.
- **Competition:** DBH and local Hegyi competition are unchanged. A separate British-Sitka SDI occupancy signal [B] drives self-thinning toward suppressed trees ([C] parameters).
- **Deadwood:** self-thinning deaths become existing fallen deadwood.

**Simulation validation outputs** (not empirical targets), unthinned: 1,706 / 900 / 538 / 344 / 256 stems/ha at ages 40 / 60 / 80 / 100 / 120; basal area 75.9 → 83.8 m²/ha; RD 0.83 → 0.97–0.99 (legacy growth: 416 m²/ha at 120).

**Scenario 1 under the new stack:** completed in Year 25 with all 8 objectives; lowest cash €6,139.75. No economy or objective changes.

**Harness gate status:**
- **Removal:** stale harness contract (U now plans the accepted species-less area clearance). The corrected harness on `task/scenario-one-pedagogy-overnight` @ `60674f1` passes in batch on the integrated code.
- **Clearance:** stale first-use-help assumption plus an interactive-input requirement. The corrected harness passes in an interactive Editor.
- **MenuTutorial:** interactive-input requirement; passes in an interactive Editor.
- The harness fixes are to be ported in the pedagogy P0 task. None is a production failure.

**Open calibration items:**
- realised top height 6–9 % below the Class III curve at older ages;
- mortality onset/strength/exponent ([C]);
- 0.5 stem form factor unvalidated;
- Forest Yield cross-validation;
- Irish maximum-density data;
- recruit mortality pulses under a full canopy.

## Regeneration Bottlenecks v1

Browsing & Protection v1 (shared browsing response, accepted shelter protection, deterministic restore) shipped as part of Scenario 1. Scenario One Regeneration Model 2 now supplies the accepted bramble/bracken juvenile-survival competition (D-049); the other candidate elements remain queued direction, not active, and each needs its own accepted packet:
- richer understorey beyond the accepted Scenario One Model 2 bramble/bracken juvenile-survival competition;
- competition/concealment;
- spot control/regrowth.

## Regeneration Model 2 (2026-10-08)

The accepted Scenario One Model 2 v1 definition (D-049) is part of current main `89a5f34d8ab54d51daaebf84ed55a5397ac07406`. Current Save 19 adds the dormant D-050 storm fields; new games use RNG 1/regeneration 2/growth 1/storm 0. Independent bramble/bracken competitor state applies shared linear juvenile-survival pressure, with separately accounted vegetation/light/browse loss. The accepted values and evidence grades are D-049; they are gameplay abstractions, not measured Irish constants.

Clearance has useful, wasted and harmful outcomes, costs real cash and can remove existing juveniles. Do not make it compulsory or teach a repeat interval. The learning-panel copy follows the active saved model, preserving legacy truth. Biological heights are unchanged; the Beech renderer fix is retained.

Open calibration/playtest items: botanical type differences, bramble concealment, competitive graminoids, source/neighbor spread, stronger site/moisture calibration, target generalization beyond Scenario One, visual recognition and human clearance decisions. These do not block this accepted v1 integration. See `Docs/Verification/RegenerationModel2Integration/IntegrationHandoff.md` for its original verification. After Storm integration, D-050, Drive mirrors and Game Dev project instructions/attachments were republished at context `58d6b0b483198abcf0208713d9a759f89d90e318`; worker-worktree smoke gates remain open.

## Storms & Windthrow Model 1 — dormant integration (2026-10-08)

Manager approved source `857150b2f20afa52d7b36be88a835f07a4ee5a51` on current main `1fbefd8a40c69dd90abb83f2149e24b546a184cf`; the complete two-commit branch was fast-forwarded into the clean ZX20 integration checkout and verified. Save 19; new-game RNG 1/regeneration 2/growth 1/storm 0. Existing/missing-field saves and frozen Reference stay storm 0. D-050 fixes the accepted Model1 profile and schema; no activation/default change is implied.

Model1 supports one fallen/uprooted outcome, deterministic mortality/deadwood/opening, optional partial salvage through the existing work/economy path and shared Model2 causality. Existing causal inspection, salvage/work plan and disturbance-review UI is retained; no new tutorial objectives are introduced.

The profile (.02 occurrence; .02/.06/.18 uniform severity; ReducedProposal; BoundedRational; neutral site/external edge) is frozen gameplay calibration [C]. Future material changes need StormModel 2 or an approved persisted-profile/version architecture.

Open follow-ups: crown/root visual quality, combined developed/century deadwood and windthrow rendering, standalone Player profiling, final storm activation policy, full lesson/progression, directional outside-landscape exposure and snapped stems/snags. Current Editor measurements permit dormant integration only. Before default activation require standalone/developed/century/combined profiling and acceptable crown/root behaviour; no retrospective FPS target is introduced.

Claude P2/P3 work touching TreeInspectionView, WorkPlanView, AnnualReviewView, ScenarioOneUiRoot or StandMapView must rebase/port onto the newly published main before final verification; conflicts are outside this integration. Exact source, regression, compatibility, context/publication and follow-up identities are in `Docs/Verification/StormsWindthrowIntegration/IntegrationHandoff.md`. Dirty `/home/jer/CCF-main` remains preserved.
