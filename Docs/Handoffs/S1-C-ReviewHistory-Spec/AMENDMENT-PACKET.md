# AMENDMENT PACKET — S1-C Review / Management History Specification

**Manager verdict being applied:** `NEEDS CHANGES — NARROW MANAGER AMENDMENT ONLY`  
**Branch:** `spec/scenario-one-review-history-v1`  
**Worker:** same S1-C specification lane  
**Task class:** specification amendment only; no production code  
**Context:** `eb218eab8b8e200a81cc99bfef7e8906bc21a88d`

## Goal

Apply the Manager decisions in `MANAGER-REVIEW.md`, remove the save-version collision, and convert the S1-C specification package from `DRAFT` to `APPROVED` implementation authority.

No new design work is requested.

## Required edits

### 1. Resolve the four product decisions

Apply these decisions consistently across `spec.md`, `plan.md`, `tasks.md`, and `checklists/requirements.md`:

- **Pre-feature objective achievement:** derive and label an achievement year only when exact first achievement can be recomputed solely from persisted year-tagged history and accepted objective logic; otherwise show `not recorded`. Never use a knowingly approximate year as exact fact.
- **Session-only browse / DBH growth:** do not persist in S1-C; label as session-only / not recorded after load.
- **Intervention detail:** aggregate-first, with drill-down to recorded targets.
- **Decision provenance:** existing commitment/result trail is sufficient for S1-C; mark/retain decision-history is a future extension point, not a requirement here.

Remove unresolved OPEN dependencies from T-03/T-05/T-06. A resolved-decision register may remain for traceability.

### 2. Correct save-version sequencing

Remove all fixed claims that S1-C will bump save v20 → v21.

Required rule:

- inspect the actual integration base immediately before the S1-C objective-achievement persistence change;
- use the **next available save-schema version**;
- never reuse a version already owned by an accepted/qualified concurrent feature;
- Rack Planning & Field Marking V2 currently reserves/implements **v21**, so if Rack V2 integrates first, S1-C is expected to use **v22**;
- preserve old-save defaulting and frozen Reference Future v1 exactly;
- no retrospective write into old saves.

Update T-05, plan integration boundaries, save section, and checklist accordingly.

### 3. Approve the spec package

Once the above edits are internally consistent:

- set `spec.md` status to `APPROVED`;
- set `plan.md`, `tasks.md`, and `checklists/requirements.md` to reference the approved spec state;
- retain T-01…T-08 as implementation tasks, still TODO;
- do not mark any implementation task complete.

### 4. Update the handoff

Append a Manager-amendment addendum to:

`Docs/Handoffs/S1-C-ReviewHistory-Spec/HANDOFF.md`

It must record:

- Manager review commit `65d354a6654b7ffd25b73cb04c9691f4aea127ea`;
- decisions applied;
- exact amended/approved HEAD;
- files changed;
- confirmation that no production code or Unity asset changed;
- exit statement `S1-C REVIEW/HISTORY SPEC APPROVED — READY FOR IMPLEMENTATION PACKETS`.

## Verification

Documentation-only checks:

- no remaining unresolved `OPEN-1`…`OPEN-4` dependency in the task graph;
- no remaining unconditional/fixed S1-C `v21` bump;
- no contradiction between spec, plan, tasks and checklist;
- no production/source/Unity/save file changed;
- branch remains a clean descendant of the reviewed S1-C branch.

## Stop conditions

Stop and return to Manager only if:

- another accepted feature has changed the save-version ownership facts again;
- applying the decisions exposes a genuinely new product ambiguity;
- a production-code change appears necessary to make the documents internally consistent.

Otherwise complete the amendments without requesting another product decision.

## Exit

`S1-C REVIEW/HISTORY SPEC APPROVED — READY FOR IMPLEMENTATION PACKETS`
