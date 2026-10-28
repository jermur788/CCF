# RACK-V2-07 — Confirmation + Minimap Implementation / Qualification Packet

**Task ID:** RACK-V2-07  
**Mode:** bounded implementation + focused qualification  
**Spec:** `specs/scenario-one-objective-1-rack-planning-field-marking-v2/` (APPROVED)  
**Prerequisite:** RACK-V2-06 APPROVED / INTEGRATED through PR #46 / merge `3b5e157fa5ea7678d54606b4f0f5285a039f47e3`; Manager review `Docs/Handoffs/RACK-V2-06/MANAGER-REVIEW.md`  
**Minimum base:** current `main` must descend from `e57987ebb3631673d482c503e146b7e11b82c7c6`.

## Authority correction — OPEN-3 is resolved

Older D-064/spec/task text still says OPEN-3 is open. That text is stale and subordinate to the later direct user decision accepted 2026-10-28.

The accepted boundary is:

- a rack with **zero executed removals** may be reopened/edited;
- pending or approved-but-unexecuted rack work may be reconciled/cancelled through the existing authoritative path, with a clear warning/explicit consent where cancellation of approved work is required;
- a rack with **any executed removal** is geometry-locked and represented as harvested/executed;
- executed removals are never reversed;
- changing one editable rack invalidates only that rack's geometry-bound entrance-mark state;
- no duplicate work orders and no second rack-removal system.

Do **not** stop merely because older authority text says OPEN-3 is unresolved. Stop only if the current implementation exposes a genuinely new ambiguity beyond this accepted boundary.

## Goal

Implement the confirmed-plan transition and minimap propagation using the same authoritative Rack V2 plan already qualified in RACK-V2-03 through -06.

Deliver:

1. `Confirm Rack Plan` enabled only when the current V2 plan is valid and contains exactly five rack objects.
2. Confirm uses the existing authoritative confirm/order-reconciliation path; it creates/refreshes pending rack-removal work only and does not approve or execute it.
3. After confirmation, Objective 1 presentation switches from `Racks planned: X / 5` to `Rack entrances marked: X / 5`, both derived from authoritative rack objects.
4. A clear `Reopen Rack Plan` path.
5. Reopen follows the accepted OPEN-3 boundary and exposes any required warning/consent for approved-but-unexecuted work.
6. Geometry changes after reopen clear entrance-mark state only for the changed editable rack; unchanged racks retain their mark state.
7. Racks with executed removals remain visible but locked/harvested and cannot be geometry-edited.
8. The minimap uses the same dashed/solid conceptual language as the Stand Map and consumes the same authoritative rack plan.
9. First-time minimap teaching note equivalent to: `Dashed lines show suggested rack positions. Solid lines show your confirmed plan.` It must not imply dashed suggestions are completed/authoritative.

## Scope / expected surface

- `Assets/ForestPrototype/UI/StandMapView.cs` and narrow Stand Map confirmation/reopen plumbing.
- Existing Rack V2 runtime/facade only where needed to expose the accepted reopen/consent path without duplicating lifecycle state.
- Existing Objective/Learning sequence and counter presentation only where required for the planning → marking transition.
- Existing minimap UI/model/draw surface narrowly required to render suggested/confirmed racks consistently.
- Focused RACK-V2-07 verification under `Tools/Verification/`.

Reuse the existing authoritative rack objects, order lifecycle and mark state. Do not create a second confirmation flag, second rack list, second work-order path or independently incremented counters.

## Explicit exclusions

- No RACK-V2-08 removal of competing first-person authoring.
- No RACK-V2-09 3D waypoint/spray/orange-mark implementation.
- No RACK-V2-10 Help expansion beyond the minimap note required here.
- No RACK-V2-11 save/schema work.
- No MFM implementation.
- No species/ecology/growth/economy changes.
- No whole Objective-1 qualification.
- No Windows/Player build.

## Required behaviour / negative cases

### Confirm

- Invalid plan or fewer/more than five racks: confirmation disabled/refused with honest reason.
- Valid five-rack plan: confirmation succeeds exactly once through the authoritative path.
- Confirmation creates/refreshes pending rack-removal orders without duplicates.
- Confirm does not approve work, execute removals or mark entrances.
- Confirmed plan hides/disables geometry editing until reopened.

### Reopen before executed work

- Pending work: reopen is allowed and subsequent geometry edits reconcile pending orders through the existing path.
- Approved-but-unexecuted work: reopening/editing requires a visible warning and explicit consent before cancellation/recreation. Refusal/no consent changes nothing.
- Reopen with zero executed removals must not fabricate or duplicate orders.

### Reopen after partial/full execution

- A rack with any executed removal stays geometry-locked and visually harvested/locked.
- Other zero-executed racks may still be reopened/edited if the authoritative state machine supports that accepted per-rack boundary.
- Never undo or recreate an executed removal.
- If the current lifecycle cannot reopen the remaining editable racks without corrupting executed work, STOP and return `EXECUTED-WORK REOPEN CONFLICT — MANAGER REVIEW REQUIRED` with exact state/API evidence.

### Entrance-mark state

RACK-V2-09 player spraying is not implemented yet. For this task, exercise partial-mark states through the existing authoritative rack mark-state/runtime fixture APIs only. Prove:

- reopening alone does not clear marks;
- changing rack A clears only rack A's mark;
- unchanged racks retain marks;
- executed/locked rack state cannot be invalidated by a geometry edit.

### Minimap

- Before confirmation, any suggested routes are clearly dashed/guidance only.
- After confirmation, the authoritative confirmed plan is solid and distinguishable.
- Minimap and Stand Map represent the same rack identities/geometries.
- No minimap rendering path mutates authoritative rack state.

## Verification

### Static/offline

Prove at minimum:

- exact-five + validity confirmation gate;
- counter switch derives from rack objects;
- confirm is idempotent/no duplicate orders;
- pending and approved-but-unexecuted reopen paths obey accepted warning/consent semantics;
- executed rack lock is preserved;
- per-rack mark invalidation is exact;
- minimap uses authoritative rack/suggestion sources, not copied mutable state;
- Legacy40/model-0/reference path does not enter V2 confirmation/reopen/minimap curriculum;
- RACK-V2-04/05/06 protected offline checks remain green;
- runtime type compile succeeds.

### Unity / rendered interaction

Use the exact candidate under the machine-wide Unity lock. At minimum exercise:

- invalid <5 plan cannot confirm;
- valid five-rack plan confirms;
- objective text/counter transition is visible;
- confirmed map/minimap agree;
- reopen before marking/pending state;
- reopen with fixture-set partial entrance-mark state and edit one rack, preserving other marks;
- approved-but-unexecuted warning/decline/consent path if reachable through the current UI/runtime;
- executed rack remains locked while any permitted zero-executed rack remains editable;
- no duplicate work orders;
- RACK-V2-06 spacing teaching still functions after reopen;
- minimap dashed/solid teaching is readable at 1280×720 and 1920×1080.

If MFM-03 #39 or another task owns Unity, continue static work and wait for normal lock release. Never bypass or kill another owner.

## Preservation gates

Re-run the bounded Rack V2 gates needed to prove no regression to:

- RACK-V2-03 authoritative runtime/order lifecycle;
- RACK-V2-04 map planning/dashed-vs-solid semantics;
- RACK-V2-05 editing/OPEN-3 lock behaviour;
- RACK-V2-06 spacing teaching.

Never weaken or rebaseline protected assertions merely to get green.

## Stop conditions

Return to Manager if:

- the accepted per-rack OPEN-3 boundary cannot be represented without a second lifecycle/state model;
- approved-work cancellation cannot be surfaced with explicit user consent using the existing authoritative path;
- executing one rack forces unsafe mutation/reversal of completed forestry work when reopening another rack;
- minimap propagation would require duplicated persisted rack state;
- another live writer owns a required shared file;
- protected Rack V2 behaviour regresses.

## Coordination / ownership

Recommended branch: `feature/scenario-one-rack-v2-confirm-minimap-v1` in a dedicated clean worktree.

Before production edits:

- refresh current `main`, claim and live `active-tasks.md`;
- follow `Docs/Handoffs/RACK-V2-07/COORDINATION-AMENDMENT.md`;
- verify no conflicting writer owns required files;
- mark the task claim `[RUNNING]` with agent/model/provider, branch, worktree and starting HEAD.

Before Unity, refresh live coordination, acquire the machine-wide lock normally and record the lock token. No Windows build.

## Handoff

Publish:

`Docs/Handoffs/RACK-V2-07/HANDOFF.md`

Include starting main, implementation commits, exact tested candidate, final tip, changed files, OPEN-3 state coverage, confirm/reopen/order evidence, mark-state preservation, minimap evidence at both resolutions, preservation gates, Unity lock receipt and limitations.

At publication rename the claim `[HANDED-OFF]`, link the handoff and close it.

## Success exit

`CONFIRMED PLAN PROPAGATES — READY FOR MANAGER REVIEW`
