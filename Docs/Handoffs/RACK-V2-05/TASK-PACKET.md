# RACK-V2-05 — Overnight Rack Editing Implementation / Qualification Packet

**Task ID:** RACK-V2-05  
**Mode:** substantial implementation + focused qualification  
**Spec:** `specs/scenario-one-objective-1-rack-planning-field-marking-v2/` (APPROVED)  
**OPEN-3 authority:** `Docs/Handoffs/RACK-V2-05/OPEN-3-MANAGER-DECISION.md` — ACCEPTED 2026-10-28  
**Prerequisite:** RACK-V2-04 APPROVED / INTEGRATED through PR #41, merge `f91411a97c8cf8373f2164652de35f22669ab736`  
**Minimum authority base:** current `main` must descend from Manager OPEN-3 record `1a520555184a0088c27392c1325a8a77c40aab26`. Refresh `origin/main` immediately before starting and record the actual starting HEAD in the claim.

## Why this is the overnight task

This is the largest single bounded Rack V2 task currently authorised without entering RACK-V2-06+ scope. It is on the Objective-1 critical path and unlocks spacing teaching, confirmation/minimap, removal of competing authoring, field marking, persistence qualification and end-to-end functional qualification.

The goal is not to stop after a small patch. Use the overnight window to carry RACK-V2-05 through implementation, static/offline checks, real-pointer Unity interaction qualification, negative cases, publication and handoff unless a packet stop condition is genuinely reached.

## Required execution roles

### Primary writer

Use **T3 Code / strongest local SOL-CODEX Unity-capable implementation model**, with **GPT-6.1 Terra xhigh preferred for the interactive/UI implementation** when available.

Recommended branch: `feature/scenario-one-rack-v2-editing-v1`  
Recommended worktree: a dedicated clean worktree, e.g. `/media/jer/ZX20/CCF-RackV2-05`.

There is exactly **one writable lane/worktree and one primary writer** for the task at any instant.

### DeepSeek watcher — mandatory from task start

Start **OpenCode / DeepSeek v4.1 Flash (or strongest available DeepSeek)** as a read-mostly watchdog/continuation coordinator from the beginning of the task.

The watcher must:

1. refresh `Docs/Project/coordination/active-tasks.md`, the task claim and the primary branch before production work;
2. track progress against this packet and prevent scope drift into RACK-V2-06+;
3. watch for primary-model context exhaustion, provider quota/rate limits, tool/capability failure, repeated non-progress, or a model becoming materially less suitable for the current phase;
4. **change models/agents before the task stalls** when one of those limits is reached;
5. preserve the same branch/worktree/ownership contract across model transitions — never permit two concurrent writers;
6. record every executor/model/provider transition, current HEAD and reason in the task claim;
7. ensure the incoming agent reads this packet, live coordination, claim history and current worktree state before editing;
8. keep the task `[RUNNING]` across a clean model handoff rather than declaring a blocker merely because one model/provider hit a limit;
9. only mark `[BLOCKED]` when the task itself has a product/ownership/safety/technical stop that another capable model cannot continue through.

**Continuation ladder:** prefer another local Unity-capable SOL/CODEX/T3 model for implementation/Unity work; use Claude subscription 1 or 2 for independent reasoning/review or substantial code continuation when it has the required repo/worktree capability; use MiMo/DeepSeek for bounded static diagnosis, test repair or continuation where capable. Capability, not brand, decides the handoff.

If the environment can spawn/switch agents directly, do so. If it cannot, the watcher must immediately publish a continuation note in the claim naming the best next agent/model and exact branch/worktree/HEAD; do not discard or rewrite completed work.

## Claim / ownership preflight

Before production edits:

- refresh `origin/main` and live `active-tasks.md`;
- verify no conflicting writer owns the Stand Map / Rack editing files;
- verify the dedicated worktree is clean and based on current main;
- mark the task claim `[RUNNING]` with primary agent/model/provider, DeepSeek watcher, branch, worktree and starting HEAD;
- preserve all unrelated dirty work elsewhere; never stash, clean, absorb or overwrite it.

Before any Unity execution:

- refresh live coordination again;
- confirm the machine-wide Unity lock is free;
- acquire it normally and record the lock token in the task claim;
- do not bypass or kill another owner;
- no Windows build.

## Accepted OPEN-3 behaviour

Do not stop on stale spec text saying OPEN-3 is open. The later Manager decision is authoritative:

- editable/reopenable while the affected rack has **zero executed removals**;
- pending or approved-but-unexecuted rack work may be reconciled/cancelled through existing authoritative paths with warning as applicable;
- any rack with at least one executed removal has geometry locked and is represented as harvested/executed;
- executed removals are never reversed by editing;
- no duplicate work orders or second rack-removal system;
- only geometry-changed editable racks lose entrance-mark state, using existing Rack V2 state-management rules.

RACK-V2-05 does **not** build the RACK-V2-07 reopen/confirmation user flow. Implement only the editing mechanics/state boundaries required so later confirmation/reopen UI can call them safely.

## Goal

Deliver direct mouse manipulation of authoritative player rack objects on the Stand Map:

- create;
- select;
- move;
- rotate;
- delete;
- `Undo last rack edit`;
- `Reset to suggested layout` with confirmation when player edits would be discarded;
- soft snapping toward the qualified suggested/general orientation;
- soft snapping parallel to nearby player racks;
- valid entrance re-snapping to the approved path/headland geometry;
- enough freedom to deliberately deviate from suggestions.

The map remains the only new V2 rack-geometry authoring surface. Suggested dashed racks remain guidance and never become authoritative player racks merely by being displayed.

## Required architecture

1. Consume the existing authoritative Rack V2 plan/runtime APIs and the integrated `ScenarioOneSuggestedRackLayout.Get(...)` foundation. Do not duplicate IDs, headland geometry, suggested-route constants, work-order lifecycle or counters in UI code.
2. Player-rack editing must operate on one authoritative rack-plan object set with stable identity.
3. Creation establishes a valid entrance on the path/headland. Moving re-snaps the entrance appropriately. Rotation is around the entrance and must have an obvious pointer affordance (far endpoint or handle).
4. Delete removes only the selected editable rack through the authoritative path.
5. Undo is exactly one last rack edit; no general undo system.
6. Reset restores the current qualified suggested layout into player-editable rack objects only after confirmation if player edits would be lost. It must not mutate the immutable suggestion source.
7. Soft snapping aids alignment; it must not force a fixed invisible rail or silently make invalid geometry valid.
8. Do not independently persist or increment objective counters.
9. Preserve Legacy40 and Reference Future v1 behaviour.
10. Preserve RACK-V2-04 map tool/navigation behaviour and dashed-vs-solid semantics.

## Validation / negative cases

The task must reject or safely prevent:

- degenerate/near-zero rack length;
- duplicate rack geometry;
- materially overlapping rack geometry;
- rack entrance that cannot resolve to the approved path/headland;
- obviously outside-property/stand rack geometry;
- editing a rack with any executed removal;
- an edit path that duplicates pending/approved work orders;
- a suggestion read that mutates player rack state;
- a Legacy40/model-0/reference path entering V2 editing.

Do not silently snap an invalid plan into validity. RACK-V2-06 will own the final spacing-teaching copy/visualisation; RACK-V2-05 may expose only the minimal status/measurement plumbing needed for editing and tests.

## Files / ownership

Expected primary surface:

- `Assets/ForestPrototype/UI/StandMapView.cs`
- `Assets/ForestPrototype/UI/Map/*` narrowly required for pointer/selection/edit drawing and hit testing
- existing Rack V2 runtime/plan files only where a narrow authoritative editing API is required
- focused RACK-V2-05 verification under `Tools/Verification/`

Do **not** edit MFM mechanics, S1-C/S1-D, parity, ecology, growth, economy, species data, Reference Future v1, unrelated UI, or generated context/dashboard state.

If a shared Rack runtime file is currently owned by another live writer, STOP before editing it and return `OWNERSHIP CONFLICT — MANAGER ROUTING REQUIRED` with the exact file/API needed.

## Explicit exclusions

No RACK-V2-06 spacing-teaching UX/copy beyond minimal editing plumbing.  
No RACK-V2-07 confirmation/minimap implementation or reopen UI.  
No RACK-V2-08 first-person author removal.  
No RACK-V2-09 orange field marking.  
No RACK-V2-10 Help work.  
No RACK-V2-11 save/schema expansion or migration.  
No whole Objective-1 qualification.  
No MFM implementation.  
No Windows build.

## Verification

### Static/offline

Create/extend focused checks proving:

- create/select/move/rotate/delete semantics;
- stable identity across edits;
- one-level undo for each authorised edit category;
- reset-to-suggested exact geometry source and confirmation guard;
- entrance snap validity;
- soft orientation/parallel snapping remains advisory;
- duplicate/overlap/degenerate/out-of-property negatives;
- executed rack lock;
- no suggestion mutation;
- no counter/work-order duplication;
- Legacy40/model-0/reference compatibility;
- existing RACK-V2-04 map-planning and suggested-layout checks remain green;
- runtime type compilation succeeds.

### Unity / real pointer

Use the exact implementation candidate and machine-wide Unity lock. At minimum qualify real-pointer interaction for:

- enter Plan Racks mode;
- create a rack;
- select it;
- drag/move and observe entrance re-snap;
- rotate via the intended mouse affordance;
- delete with visible UI;
- Delete/Backspace where appropriate;
- undo last edit;
- reset to suggested layout, including confirmation after player edits;
- deliberate deviation from suggested orientation remains possible;
- negative invalid geometry does not become authoritative;
- an executed rack cannot be moved/rotated/deleted;
- save/world state outside authoritative rack-plan changes remains stable.

Capture rendered evidence at **1280×720 and 1920×1080** sufficient to prove selection/handles and direct-manipulation affordances are readable. Full rendered-appearance acceptance remains RACK-V2-13.

### Preservation gates

Re-run protected Rack V2/map gates needed to prove no RACK-V2-04 regression. Preserve protected Scenario One/Legacy40/reference anchors relevant to this slice. Never rebaseline to make a failure green.

## Stop conditions

Return to Manager rather than inventing a product rule if:

- current authoritative rack APIs cannot support direct editing without a second rack state model;
- path/headland entrance snapping exposes a material product ambiguity not covered by the approved spec;
- executed-work locking would require undoing forestry results;
- save/schema change becomes necessary for this task rather than later RACK-V2-11;
- another live owner holds a required shared file;
- verification exposes protected-anchor movement or a subsequent-objective regression;
- no capable available agent/model can continue after the DeepSeek watcher exhausts the continuation ladder.

Do **not** stop merely because the first model hits quota/context/tool limits; transition to another suitable agent/model under the watcher protocol.

## Handoff

Publish a substantive GitHub Markdown handoff at:

`Docs/Handoffs/RACK-V2-05/HANDOFF.md`

It must include:

- starting main SHA;
- implementation commit(s) and exact Unity-tested candidate;
- final branch tip;
- complete changed-file list;
- model/provider transition log from the DeepSeek watcher;
- static/offline results;
- Unity version, lock receipt and real-pointer results;
- 1280×720 and 1920×1080 evidence locations;
- preservation/nonmutation results;
- explicit statement of what remains for RACK-V2-06+;
- any limitations without hiding them.

At handoff, rename the task claim `[HANDED-OFF]`, link the handoff and close it.

## Success exit

`RACK EDITING READY — READY FOR MANAGER REVIEW`
