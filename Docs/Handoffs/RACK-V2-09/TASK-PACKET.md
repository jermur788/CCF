# RACK-V2-09 — Field Marking Task Packet

## Task

**RACK-V2-09 — physically mark each confirmed Rack V2 entrance tree in orange.**

This task implements the approved field-marking step of Scenario One Objective 1 after Rack V2 map planning has become authoritative.

## Authority

- Approved spec: `specs/scenario-one-objective-1-rack-planning-field-marking-v2/`
- Task authority: `RACK-V2-09 — Field marking` in that spec's `tasks.md`
- Decision Log: D-064 / accepted OPEN-3 rules
- Integrated prerequisite: RACK-V2-08 through PR #89 / merge `d925e9591f2fffb9419881a0e77031cefccf9aeb`
- Earlier prerequisite: RACK-V2-07 confirmation/minimap already integrated and qualified.

Implementation base must be current `main` containing merge `d925e959...`, or a later descendant explicitly accepted by the Manager.

## Status / execution gate

**WAITING — DO NOT START UNITY OR PRODUCTION EDITS WHILE THE MACHINE STORAGE FLOOR FAILS.**

The last Rack qualification left `/media/jer/ZX20` at approximately 11.5 GiB free, below the 15 GiB machine Unity-lock floor. The lock is free but acquisition is expected to refuse. Before this task changes production or launches Unity, the worker must refresh live coordination and demonstrate that the required storage floor is restored.

Do not delete, clean, move or overwrite unrelated work to create space. Any storage remediation beyond already-authorised cleanup requires a separate safe ownership decision.

## Lane

- Target agent/model/provider: strongest available **OpenAI Codex / Sol-class Unity-capable local model**.
- Branch: `feature/scenario-one-rack-v2-field-marking-v1`
- Intended worktree: `/media/jer/ZX20/CCF-RackV2-09` **only after the storage preflight passes**. If that exact path cannot satisfy the machine storage rule, stop and return to the Manager rather than silently choosing a new writable lane.
- Task claim: assigned by Manager; claim tracks execution presence only.
- Unity: machine-wide lock required before any Editor/runtime work.
- Windows build: prohibited.

## Goal

After the player confirms a valid Rack V2 plan, they must walk to each confirmed rack entrance and physically mark the authoritative entrance tree in orange. The entrance-marked counter must derive from authoritative rack objects, not from a second independent counter or mark system.

## Required behaviour

### 1. Authoritative entrance-tree target

For each confirmed unmarked rack, resolve the entrance tree using the approved rule:

- the first rack-removal tree nearest the path/headland;
- near that rack's centreline;
- inside that rack's authoritative removal set.

If no defensible target exists, do **not** invent or substitute an unrelated tree. Return the approved failure state:

`RACK INVALID — ADJUST PLAN`

The player must not satisfy the objective by marking a nearby but unrelated tree.

### 2. World guidance

Provide a **subtle** waypoint/cue to the next relevant entrance target. It is guidance to a real target, not a glowing world rack line. The cue clears for that rack after its entrance is successfully marked.

Do not add persistent 3D rack geometry, world-space rack rails, or automatic traversal lines.

### 3. Spray interaction

Use the approved spray-marker interaction:

- player must be within the intended interaction range;
- aim/target validation is required;
- valid/invalid target feedback must be understandable;
- successful marking produces the short approved sound/visual feedback;
- one interaction applies the complete entrance mark rather than freehand painting.

Do not create a general-purpose painting mechanic.

### 4. Orange visual mark

A successful entrance mark is persistent on the target tree and visually reads as orange vertical stripes on four sides from one interaction, per the approved spec.

The mark must remain a forestry/gameplay mark, not a glowing waypoint substitute.

### 5. Mark conflicts

The entrance target may not simultaneously satisfy incompatible exclusive tree marks.

At minimum:

- Crop Tree cannot also be the orange entrance mark;
- Fell and orange entrance-mark conflicts must be prevented according to the existing exclusive-mark architecture;
- do not create a parallel mark system that bypasses the existing authoritative mark semantics merely to make the task work.

If the exclusive-mark architecture cannot support the approved orange state without weakening Fell/Crop behaviour, stop:

`RACK-V2-09 MARK-ARCHITECTURE CONFLICT — MANAGER DECISION REQUIRED`

### 6. Authoritative rack state

Entrance-mark status belongs to the authoritative rack object/state established by Rack V2. `Rack entrances marked X / 5` must derive from those objects.

Preserve OPEN-3:

- reopening itself cancels nothing;
- a rack with zero executed removals may be reopened/edited through the authoritative reconciliation path;
- if a rack's geometry changes, only that rack loses its geometry-bound entrance mark;
- a rack whose geometry is unchanged retains its mark;
- any rack with one or more executed removals is geometry-locked/harvested;
- executed removals are never reversed.

Do not introduce a second rack counter or an independent entrance-mark ledger.

## Persistence boundary

RACK-V2-09 may use the field-marking state already authorised on the Rack V2 authoritative object. **Do not silently expand into RACK-V2-11 persistence/schema work.**

The approved task list asks that the mark survive save/load, while RACK-V2-11 owns the complete persistence/schema qualification. Preflight the current implementation:

- if the existing authoritative rack serialization already carries the field-marking state without a new schema/persistence change, verify that bounded behaviour here;
- if making the mark survive save/load requires new save fields, migration policy, schema/version changes or broader persistence wiring, stop before editing that boundary and return `RACK-V2-09 PERSISTENCE DEPENDENCY — ROUTE TO RACK-V2-11`.

Reference Future v1 must remain frozen. Legacy40 must not enter Rack V2.

## Scope exclusions

Do not change:

- rack geometry authoring or spacing calibration;
- Rack V2 map editing except a narrowly required read-only target/status surface;
- ordinary forestry growth, competition, mortality, economy or species models;
- MFM proposal/prescription logic;
- unrelated Objective 1 progression;
- save schema/version unless separately authorised through the persistence dependency above;
- Reference Future v1;
- packages/project settings;
- Windows build configuration.

No freehand painting, extra future mark categories, glowing rack lines or automatic Fell marks.

## Preflight before production edits

1. Refresh current `main`, claim, and `Docs/Project/coordination/active-tasks.md`.
2. Confirm RACK-V2-08 is integrated and no later owner overlaps the files/subsystems needed here.
3. Confirm `/media/jer/ZX20` satisfies the machine Unity storage floor before creating/using the intended Unity worktree. If it does not, remain WAITING/BLOCKED.
4. Create/use only the assigned clean branch/worktree; preserve unrelated dirty work.
5. Mark the task claim `[RUNNING]` with actual model/provider, branch/worktree and starting HEAD before production edits or Unity execution.
6. Inspect the existing Rack V2 object, tree-mark architecture, entrance/removal APIs, save representation and current Objective-1 consumers before choosing edit files.
7. If scene/prefab/material edits are required, record a single writer for those assets before changing them.

## Verification

Use proportionate Tier-2 feature qualification on the exact candidate. At minimum prove:

- a confirmed Rack V2 plan exposes the correct entrance target for each rack;
- unrelated trees cannot satisfy an entrance mark;
- a Crop Tree cannot be marked orange;
- Fell/orange conflict behaviour is authoritative and non-duplicated;
- range and aim validation work;
- the successful spray interaction creates the complete orange mark and clears that rack's waypoint;
- marked count is derived from rack objects and progresses deterministically;
- reopening with unchanged geometry preserves marks;
- changing one unexecuted rack's geometry clears only that rack's geometry-bound mark;
- executed-removal lock semantics are preserved;
- Legacy40/model-0 behaviour remains compatible and does not enter the V2 marking flow;
- ordinary Fell/Crop marking outside the Rack V2 entrance interaction still works;
- clean Unity import/compile has no candidate-caused errors.

If existing serialization already carries the field-marking state without new persistence work, include a focused save/reload preservation check. Otherwise return the persistence dependency token rather than expanding RACK-V2-09.

Capture the important world/mark states at both 1280x720 and 1920x1080 when rendered evidence is useful, but RACK-V2-13 remains the dedicated rendered-appearance acceptance task.

Do not weaken, skip or rebaseline protected assertions merely to obtain green.

## Handoff

Publish:

`Docs/Handoffs/RACK-V2-09/HANDOFF.md`

Include:

- actual agent/model/provider;
- branch/worktree/starting HEAD and exact qualified candidate SHA;
- production and serialized-asset diff inventory;
- target-resolution rule and why it cannot mark an unrelated tree;
- mark architecture/conflict handling;
- OPEN-3 behaviour;
- persistence-boundary disposition;
- exact verification commands/results and captures;
- Unity lock ownership/release;
- storage preflight result;
- unrelated-work accounting;
- no-Windows-build statement.

### Success

`RACK-V2-09 FIELD MARKING READY — READY FOR MANAGER REVIEW`

### Stop — architecture

`RACK-V2-09 MARK-ARCHITECTURE CONFLICT — MANAGER DECISION REQUIRED`

### Stop — persistence boundary

`RACK-V2-09 PERSISTENCE DEPENDENCY — ROUTE TO RACK-V2-11`

### Blocked — storage

`RACK-V2-09 BLOCKED — UNITY STORAGE FLOOR NOT MET`
