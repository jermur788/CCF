# MFM-08 — Tier-2 Unity Local Dispatch Handoff

**Date:** 2026-10-30  
**Claim:** #156  
**Parent:** #91 / local umbrella #106  
**Status:** RELEASED FOR LOCAL UNITY PREFLIGHT — NOT YET RUNNING  
**Mode:** verification-only by default; no production repair authority

## Purpose

Complete the mandatory Unity Tier-2 qualification for the already static-approved Multi-Scale Forestry Management V1 Stand Map / 3D review UX candidate. This job must prove the candidate in the real Unity environment, at both required render sizes, without changing accepted production semantics.

## Difficulty / model fit

**Difficulty: 4/5.**

Reason: implementation is already present and static-reviewed, so this is not a broad coding task; however, the qualification crosses Unity import/compile, live Stand Map geometry/layering, 3D input, candidate/proposal semantics, prescription approval, Work Plan commitment, rendered evidence at two resolutions, regressions, protected compatibility and machine-lock cleanup. A failure must be diagnosed without opportunistic production repair or test weakening.

**Ultra Code suitability: YES, provided the Ultra Code worker has full local repository + Unity Editor/runtime access and can inspect/capture rendered states.** This is a strong fit for a high-reasoning code/verification agent with Unity tooling. It is not suitable for a browser-only or source-only worker because runtime/rendered claims are mandatory.

Best-suited lane: strongest available local code/Unity verification worker (Ultra Code / SOL-Codex-class / equivalent high-reasoning Unity-capable agent). Independent final acceptance remains Overall Manager/browser review after the pushed handoff.

## Exact candidate / lane

- Feature branch: `feature/mfm-08-map-3d-review-ux-v1`
- Worktree: `/media/jer/Videos/CCF-MFM08`
- Original implementation start: `cfdc93bf3943125d21570bc5749a9338f52d1cce`
- Exact production candidate to qualify: `d55ec86a3f0bf13820f2080197b6b0f43d832e06`
- Existing docs/handoff tip: `d70cec1781c104794a6f225c8609847c55df1510`
- Current Scenario One integration context: `ce6737146a7aa9ea953d7db0ca93b103f6c2c996`
- Static Manager verdict: `APPROVED — STATIC/OFFLINE PHASE ONLY; UNITY TIER-2 STILL REQUIRED`
- Final Manager checklist: `Docs/Handoffs/MFM-08-Map-3D-Review-UX/FINAL-MANAGER-REVIEW-CHECKLIST-20261030.md`
- Integration-overlap preflight: `Docs/Handoffs/MFM-08-Map-3D-Review-UX/BROWSER-INTEGRATION-PREFLIGHT-20261030.md`

The browser overlap preflight found no production-file overlap between the MFM-08 candidate and current integration from their shared base. That does not replace fresh integration checking after qualification.

## Mandatory start preflight

Before touching Unity:

1. Refresh `Docs/Project/coordination/active-tasks.md`, #91, #106 and #156.
2. Confirm no other task owns the machine-wide Unity lock.
3. Confirm no Unity Editor/process from another owner is active.
4. Confirm free-space requirements remain satisfied.
5. Confirm the feature worktree/branch is clean and the exact candidate under qualification is `d55ec86a3f0bf13820f2080197b6b0f43d832e06` (or a verification-only descendant that clearly preserves that production tree).
6. Post `[RUNNING — UNITY]` on #156 with provider/model, branch/worktree, exact production head, verification head if different, and Unity-lock owner.

Do not infer lock availability from this handoff.

## Required Tier-2 evidence

Prove on the exact production candidate:

- Unity **6000.6.0f1** import/compile clean;
- rendered/readable treatment-review/Stand Map states at **1280×720** and **1920×1080**;
- authoritative source-sample footprint alignment and coverage geometry/layer ordering;
- ordinary Rack V2 / Fell / Crop marks remain usable and visually distinct;
- candidate queue uses real living-tree state and Previous/Next remains bounded/non-wrapping as designed;
- candidate navigation reaches the real tree;
- ordinary 3D `[C]` confirmation remains the authoritative Crop Tree action;
- `[N]` reject/reopen works as review-session behavior without input collision;
- no map path or bulk action silently confirms Crop Trees;
- candidate is presented as an inspection prompt, not a quality judgement;
- proposal presentation is visibly/textually noncommitted and distinct from Fell/Crop/Rack marks;
- Prepare creates only the prescription/snapshot and does not create Fell marks or committed work;
- at least one extrapolated unit receives the required **real 3D spot-check** using fresh authoritative coverage;
- prescription approval creates ordinary **Pending** work with provenance;
- Work Plan remains unapproved immediately afterward and remains the commitment gate;
- repeated prescription approval fails/blocks safely;
- manual/held/conflict state remains visible and is not overwritten;
- affected Rack V2 and MFM-04/05/06/07 regressions remain green;
- required Reference Future / protected compatibility remains unchanged;
- save/world/worktree/Unity lock return cleanly.

## Scope / stop conditions

Verification-only by default. Do **not** edit production to make the gate pass.

Stop and report exact evidence if:

- Unity compile/import fails because of a production defect;
- geometry/layer/input behavior materially conflicts with accepted semantics;
- candidate/proposal/commitment boundaries fail;
- the only path to green requires changing coverage/calibration, save/schema/hash/reference, Work Plan semantics, MFM-09 curriculum, proposal-to-Fell behavior or tests;
- another worker owns the required files or Unity lock.

No Windows build.

## Required handoff

Update/publish:

`Docs/Handoffs/MFM-08-Map-3D-Review-UX/HANDOFF.md`

Include exact production and verification SHAs, model/provider, branch/worktree, Unity lock receipt, commands, pass/fail counts, screenshot manifest at both resolutions, material runtime witnesses, regression/protected results, changed-file boundary and clean return state.

Expected local success token:

`MULTI-SCALE REVIEW UX QUALIFIED — READY FOR MANAGER REVIEW`

This token does not itself mean Manager APPROVED or integrated.

## Copy/paste dispatch line

`Run MFM-08 Tier-2 from Docs/Handoffs/MFM-08-Map-3D-Review-UX/LOCAL-DISPATCH-HANDOFF-20261030.md under claim #156. This is verification-only by default. Fresh-preflight active-tasks, worktree cleanliness, free space, Editor processes and the machine-wide Unity lock; then post [RUNNING — UNITY] with provider/model/head/worktree/lock owner. Qualify exact production candidate d55ec86a3f0bf13820f2080197b6b0f43d832e06 in Unity 6000.6.0f1, including 1280x720 + 1920x1080 rendered states, live map/3D geometry and input, [C]/[N], real 3D spot-check, proposal/noncommitment semantics, prescription→ordinary Pending→Work Plan commitment boundary, conflicts/regressions/protected compatibility and clean cleanup. Do not repair production or weaken tests under this claim. Publish HANDOFF.md and return MULTI-SCALE REVIEW UX QUALIFIED — READY FOR MANAGER REVIEW or the exact blocker. No Windows build.`

## User action required

Dispatch the copy/paste line to one Unity-capable local worker. This task can run in parallel with #187 because #187 is offline-only and owns no MFM-08 UI files.