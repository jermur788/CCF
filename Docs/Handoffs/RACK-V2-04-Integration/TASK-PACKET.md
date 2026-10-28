# RACK-V2-04-INTEGRATION — Clean qualified map-planning branch for Manager merge

## Purpose

Prepare the already-qualified RACK-V2-04 branch for canonical integration without rerunning Unity and without importing the worker-side `active-tasks.md` edit.

This is an **integration-preparation task only**. Do not change product behaviour.

## Authority

- Approved spec: `specs/scenario-one-objective-1-rack-planning-field-marking-v2/`.
- Manager review: `Docs/Handoffs/RACK-V2-04/MANAGER-REVIEW.md`.
- Qualified production checkpoint: `bcb4f345544e50cbb32b8fecd9f82a33a284b0e2`.
- Exact Unity/rendered-tested candidate: `68bcf6d0b1f0f6907d1dc0ba5a220583e4ade058`.
- Published v2 branch tip before integration cleanup: `61a5924a9b4fa31d84e3b739e0c3405b630d9f37`.
- Existing integration PR: #41.
- Qualified handoff blob prefix supplied by publication worker: `c09717f1a19e`.

## Target lane

Use the existing RACK-V2-04 v2 Git/worktree lane if available, or another clean Git-capable lane. No Unity ownership is required.

## Required preflight

1. Fetch current `origin/main` and the current remote head of `feature/scenario-one-rack-v2-map-planning-mode-v2`.
2. Refresh `Docs/Project/coordination/active-tasks.md` read-only. Do not edit it.
3. Mark the task claim `[RUNNING]` with actual agent/model/provider, branch/worktree and starting HEAD.
4. Verify PR #41 still targets `main` and its head is the expected published v2 branch. If the head has moved unexpectedly, stop and report `RACK-V2-04 INTEGRATION HEAD MOVED — MANAGER REVIEW REQUIRED`.

## Integration operation

Rebuild/rebase the qualified v2 branch onto current `main` so that PR #41 becomes mergeable, while preserving the already-qualified task content.

Required invariants:

- **Do not import any worker-side change to `Docs/Project/coordination/active-tasks.md`.** The resulting branch version of that file must be byte-identical to current `main`.
- Preserve the qualified RACK-V2-04 production bytes from `bcb4f34` for:
  - `Assets/ForestPrototype/UI/Map/StandMapDrawPlan.cs`
  - `Assets/ForestPrototype/UI/Map/StandMapLegend.cs`
  - `Assets/ForestPrototype/UI/Map/StandMapLegendView.cs`
  - `Assets/ForestPrototype/UI/Map/StandMapModel.cs`
  - `Assets/ForestPrototype/UI/Map/StandMapSource.cs`
  - `Assets/ForestPrototype/UI/Map/StandMapSymbols.cs`
  - `Assets/ForestPrototype/UI/StandMapView.cs`
- Preserve the focused verification additions/changes and the published qualified `Docs/Handoffs/RACK-V2-04/HANDOFF.md` content.
- Do not modify Rack V2 geometry, rack lifecycle, confirmation/reopen policy, field marking, minimap, MFM, save/schema, ecology/economy, or any other production subsystem.
- Do not change the tested product behaviour to resolve a merge problem.

The Manager has already compared the branch merge base `81a037e35126bbc256fcda076019af47e8e0954f` to current `main` and found **no intervening changes to the seven RACK-V2-04 production/map files**. Recheck this on the actual current main before publishing.

## Verification

No Unity rerun is required if all of the following hold:

1. the seven production files above are byte-identical to their blobs at `bcb4f34`;
2. `Assets/`, `Packages/` and `ProjectSettings/` differ from the qualified production checkpoint only by those already-qualified RACK-V2-04 production changes and by unrelated current-main changes outside the task-owned files;
3. `active-tasks.md` is byte-identical to current main;
4. the qualified handoff content remains unchanged;
5. PR #41 reports mergeable after publication.

Run the existing offline/static checks if needed to prove rebase integrity, but do not change expected values or qualification semantics.

If any qualified production file must change relative to `bcb4f34`, or current main now overlaps one of those files, stop with:

`RACK-V2-04 INTEGRATION CONFLICT — MANAGER REVIEW REQUIRED`

Do not rerun Unity merely to legitimise an unreviewed production merge.

## Publication

Prefer updating `feature/scenario-one-rack-v2-map-planning-mode-v2` with force-with-lease so existing PR #41 remains the integration vehicle. If that is unsafe, create a clean integration branch and PR and explicitly mark PR #41 superseded; do not leave two apparently canonical integration PRs open.

Publish a short handoff at:

`Docs/Handoffs/RACK-V2-04-Integration/HANDOFF.md`

Record:

- current-main base SHA;
- old and new branch heads;
- exact production-byte identity result;
- `active-tasks.md` identity result;
- PR number and mergeability;
- any offline/static checks run;
- confirmation that Unity was not rerun.

At publication, mark the claim `[HANDED-OFF]`, link the handoff and close it.

## Exit

Success:

`RACK-V2-04 CLEAN INTEGRATION READY — READY FOR MANAGER MERGE`

Blocker:

`RACK-V2-04 INTEGRATION CONFLICT — MANAGER REVIEW REQUIRED`

No Windows build.
