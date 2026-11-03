# Scenario One Tier-3 — Task 2 Evidence Closure Handoff — 2026-11-03

**Parent:** #231  
**Child claim:** #240  
**Manager checkpoint review:** `Docs/Handoffs/ScenarioOne-Tier3-PlayerTest-Candidate/MANAGER-CHECKPOINT-REVIEW-20261103.md`  
**Status:** READY / RELEASED AFTER PROVIDER RESTART

## Purpose

Close only the three evidence gaps left by Tier-3 Task 1. Reuse all valid Task 1 evidence. Do **not** rerun the completed matrix.

Difficulty: **Medium** — narrow Unity/native-input evidence closure with one lifecycle-timing discriminator and one rendered completion correction; no production implementation.

Best-suited executor: a capable local Unity/code worker with filesystem and native-input access. OpenCode/DeepSeek may resume if provider auth is healthy; otherwise use a Sol/Codex- or Claude-class local worker. Provider choice is operational, not a reason to rerun Task 1.

Independent delta reviewer: a separate capable provider/model that did not produce the new Task 2 evidence.

## Exact continuation state

Repository: `jermur788/CCF`  
Qualification branch: `qual/scenario-one-final-tier3-v1`  
Persisted Task 1 tip: `2a232885`  
Original production candidate: `6849caa0ef05baf1a52b95465ec6bba4ff94202b`  
Worktree: `/media/jer/ZX20/CCF-U1` if still clean/free; otherwise use a fresh dedicated #203-assigned worktree from the pushed qualification tip.

Before execution, refresh live `Docs/Project/coordination/active-tasks.md`, #231 and #240. Verify provider readiness, exact pushed Task 1 tip, clean lane including untracked files, host saves, evidence root, Editor/process state and current machine-wide Unity lock. Mark #240 `[RUNNING]` with actual provider/model, branch/worktree and starting HEAD.

## Binding evidence inherited from Task 1

Do not recompute the retained successful matrix, including:

- compile/import baseline;
- mandatory S1-C T-05 v24 save/history 544 checks / 34 cases and recorded/derived/not-recorded rendering;
- current/old/malformed save and provenance coverage;
- frozen Reference Future / Legacy40 protections;
- MFM approval/coverage/extension/treatment-review boundaries;
- data-level complete-loop Year-25 completion, 8/8 objectives and solvency;
- S1-C/S1-D history and repeated-cycle evidence;
- Rack field/save/persistence/Objective-1 E2E;
- guided-v2 / Combined / PARITY-07 / interaction evidence;
- species/economy/geometry evidence already accepted at the checkpoint.

The user-directed #238 test-volume reduction remains binding.

## Task 2 — exactly three fresh evidence actions

### A. Reference-preview normal-path discriminator

Using the protected-player Year-20 fixture, execute a frame-separated preview round trip that reflects ordinary UI lifecycle timing:

1. capture exact pre-preview canonical/player/protection/tree state;
2. enter Reference Preview;
3. yield at least one normal Unity frame after entry;
4. exit Reference Preview;
5. yield through the normal restoration/destruction lifecycle as required by the product path;
6. assert exact restoration of tree count/identity, protected-player state, geometry/version and other canonical state covered by the existing verification contract.

The purpose is to distinguish the known same-frame harness race from a player-facing defect. Do not manually restore state before the assertion.

**If this normal frame-separated path loses state, STOP: that is a candidate production defect requiring Browser Overall disposition.**

### B. Rack 1920×1080 clean native-input rotation receipt

Re-run only the contaminated 1920×1080 Rack rotation interaction with clean native input.

Requirements:

- fresh display/input environment;
- record requested pointer path and observed pointer/release path;
- demonstrate the intended rotation gesture actually lands/releases where requested;
- verify rotation changes as required while entrance position remains fixed by contract;
- no need to rerun the already-passing 720p case unless diagnosing a new failure.

**If a clean intended gesture still fails, STOP and return it as a product defect candidate.**

### C. Real rendered Completed-state evidence

Produce a usable `Completed` presentation at both 1280×720 and 1920×1080.

Preferred minimal method: correct the verification-only MFM-11 capture order so the screenshots are taken at the actual Year-25 Completed state **before** baseline restoration, then rerun only the MFM-11 witness/capture gate required to obtain that state.

Requirements:

- Year 25 / Completed state visible;
- 8/8 objective completion state represented consistently with the accepted data witness;
- screenshots are non-black, contentful and readable;
- no claim that a baseline-restored screenshot is a Completed-state screenshot.

Do not change production to manufacture the image.

## Stale Rack five-tab assertion

The current V2 UI intentionally contains six navigation tabs including `Treatment review`. The independent review classified the five-tab assertion as stale.

A **mechanical verification-only** correction to the accepted six-tab contract is permitted if needed for durable tooling hygiene. Do not change production UI and do not weaken navigation separation assertions. A separate full Rack product rerun is not required solely for this stale assertion.

## Verification edits allowed

Verification/docs only, narrowly necessary to:

- add frame separation to the reference-preview discriminator;
- correct completion screenshot timing;
- correct the stale six-tab verification expectation;
- improve receipt/input tracing for the single 1920 Rack rotation rerun.

No production edits under `Assets/`, `Packages/` or `ProjectSettings/`.

## Independent delta review

After A/B/C complete, obtain a fresh read-only independent delta review. It should inspect only:

- exact ancestry / no production drift;
- A/B/C raw receipts and screenshots;
- any narrow verification-only diffs;
- whether the three Task 1 blockers are resolved or whether a real product defect is now demonstrated.

Do not repeat the whole Task 1 architecture review.

## Stop conditions

Stop immediately for:

- normal frame-separated preview state loss;
- clean 1920 Rack rotation failure;
- inability to produce a genuine Completed-state rendering without production change;
- any production edit requirement;
- candidate/branch drift that invalidates Task 1 evidence;
- protected-anchor/save mutation issue;
- independent delta-review blocker.

No Windows build. #194 NO ACTION.

## Durable return

Update/publish:

- `Docs/Handoffs/PlayerTestReadiness-Overnight-01/FINAL-TIER3-HANDOFF.md`
- `Docs/Handoffs/PlayerTestReadiness-Overnight-01/FINAL-INDEPENDENT-REVIEW.md`
- `Docs/Handoffs/PlayerTestReadiness-Overnight-01/TASK-2-EVIDENCE-CLOSURE-RESULT-20261103.md`

Mark #240 `[HANDED-OFF]` and link the result.

If all three evidence items pass and the independent delta review approves, return:

`TIER-3 TASK 2 EVIDENCE CLOSURE PASS — READY FOR BROWSER OVERALL FINAL ACCEPTANCE`

Do not self-issue the final player-test token.
