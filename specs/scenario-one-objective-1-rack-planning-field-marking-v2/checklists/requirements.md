# Scenario One — Objective 1 Rack Planning & Field Marking V2 Requirements / Acceptance Checklist

- **Spec ID:** SPEC-scenario-one-objective-1-rack-planning-field-marking-v2-V1
- **Spec status:** APPROVED
- **Checklist owner:** TBD — assigned verifier (independent of the implementing thread)
- **Implementation under test:** not yet implemented — branch / HEAD / evidence root to be recorded by the verifier
- **Date:** 2026-10-26 (created; every item below is unchecked)

Use this checklist against the finished implementation. A checked item needs evidence, not assertion. Unchecked items with a stated reason are honest; unexplained gaps are not.

**Provenance.** The checklist sections from *Product* to *User-test gate* are the checklist as supplied with the approved spec. The *Acceptance criteria*, *Open product decisions*, *Spec conformance* and *Verdict* sections follow the repository template and restate `spec.md` §38 and §39.7 so no acceptance criterion is dropped.

## Product

- [ ] Map is sole rack-geometry author.
- [ ] 3D forest remains part of learning through physical marking.
- [ ] No competing map/first-person rack creation.
- [ ] Five racks confirmed as coherent with actual geometry.
- [ ] Five is presented as Scenario One calibration only.

## Evidence

- [ ] 16–20 m described as conditional early-transformation guidance.
- [ ] No universal "ideal rack spacing" claim.
- [ ] 30 m+ developed-CCF example not incorrectly applied to this starting stand.
- [ ] Orange mark choice identified as Scenario One choice within source-supported removal colours.
- [ ] First-tree-only marking identified as game-design derivation.

## Map UX

- [ ] Plan Racks is not a navigation tab.
- [ ] Suggested racks visibly dashed.
- [ ] Player racks visibly solid.
- [ ] Select works.
- [ ] Move works.
- [ ] Rotate works with mouse.
- [ ] Delete works.
- [ ] Undo works.
- [ ] Reset works.
- [ ] Spacing is displayed.
- [ ] Invalid plan explains why.
- [ ] Confirm Plan exists.

## Counters

- [ ] Planned counter derives from rack objects.
- [ ] `Racks planned X/5` correct.
- [ ] `Rack entrances marked X/5` separate.
- [ ] No map/manual counter disagreement remains.

## Minimap

- [ ] Suggested/planned distinction understandable.
- [ ] First-time explanation exists.
- [ ] Confirmed plan propagates correctly.

## Field marking

- [ ] Player must physically approach entrance tree.
- [ ] Valid target is the first appropriate rack-removal tree nearest path.
- [ ] Unrelated trees cannot satisfy objective.
- [ ] Orange mark visible.
- [ ] Vertical stripe form represented.
- [ ] Four-sided practice represented without four tedious clicks.
- [ ] Mark persists through save/load.
- [ ] Entrance waypoint clears after marking.

## Mark conflicts

- [ ] Orange rack-removal cannot coexist with Blue Crop Tree.
- [ ] Blue Crop Tree cannot coexist with Red Fell.
- [ ] Existing mark safety rules preserved.
- [ ] Conflict visible during planning where possible.

## Teaching

- [ ] Why racks exist explained.
- [ ] Spacing reason explained.
- [ ] Scenario-specific recommendation explained.
- [ ] Conditional nature of real spacing explained.
- [ ] Paint-mark meaning explained.
- [ ] First-tree-only simplification disclosed appropriately.
- [ ] Help unlocks contextually.

## Persistence

- [ ] Existing save architecture inspected first.
- [ ] Old saves load.
- [ ] New game works.
- [ ] Partial rack-plan save/load works.
- [ ] Confirmed-plan save/load works.
- [ ] Partial marking save/load works.
- [ ] Complete marking save/load works.
- [ ] Stable rack identities preserved.

## Behaviour

- [ ] No forestry growth change.
- [ ] No competition change.
- [ ] No species-model change.
- [ ] No economy regression.
- [ ] Subsequent objectives remain reachable.
- [ ] No unrelated UI redesign.

## Visual

- [ ] 720p map review.
- [ ] 1080p map review.
- [ ] 720p 3D marking review.
- [ ] 1080p 3D marking review.
- [ ] Planning handles readable.
- [ ] Spacing text readable.
- [ ] Minimap lines understandable.
- [ ] Orange marks believable on trunks.
- [ ] No giant glowing world rack lines.

## User-test gate

- [ ] Objective 1 internally qualified.
- [ ] Candidate integrated with current Scenario One.
- [ ] Dashboard synchronized.
- [ ] User player test still required.
- [ ] No Windows build before user authorization.

## Acceptance criteria

- [ ] AC-1: Map is the sole rack-geometry authoring surface — evidence:
- [ ] AC-2: First-person freehand rack creation no longer creates a competing authoritative rack — evidence:
- [ ] AC-3: Player can create/select/move/rotate/delete racks — evidence:
- [ ] AC-4: Suggested racks are distinct from player racks — evidence:
- [ ] AC-5: Spacing is visible — evidence:
- [ ] AC-6: 16–20 m is presented as Scenario One guidance, not universal law — evidence:
- [ ] AC-7: Five-rack calibration fits verified stand geometry — evidence:
- [ ] AC-8: `Plan Racks` is visually a tool, not a tab — evidence:
- [ ] AC-9: Plan can be confirmed — evidence:
- [ ] AC-10: Minimap clearly represents suggested/planned rack states — evidence:
- [ ] AC-11: Rack counter has one authoritative source — evidence:
- [ ] AC-12: Player physically sprays the first appropriate entrance tree for each rack — evidence:
- [ ] AC-13: Orange four-sided vertical rack/removal mark is visible and persistent — evidence:
- [ ] AC-14: Marked-entry counter is separate from planned-rack counter — evidence:
- [ ] AC-15: Incompatible mark conflicts are prevented — evidence:
- [ ] AC-16: Help explains the marking convention and its gameplay simplification — evidence:
- [ ] AC-17: Old saves load — evidence:
- [ ] AC-18: No simulation/growth regression occurs — evidence:
- [ ] AC-19: Rendered map and 3D review passes — evidence:
- [ ] AC-20: User can understand the workflow without prior knowledge of the old implementation — evidence:

## Product decisions (spec §39.7, D-064)

Behaviour implied by the accepted decisions. Each needs evidence when the implementation exists:

- [ ] V2 applies only where `standGeometryModel == Enlarged80`; Legacy40 saves keep their existing viable progression, are not required to have five racks at 16–20 m and do not meet the new spacing-validation gate — evidence:
- [ ] Confirm creates or refreshes pending rack-removal orders only; it neither approves nor executes — evidence:
- [ ] Field marking happens before `rack.approve`; the sequence continues `rack.approve` → execution → `rack.result` — evidence:
- [ ] Editing a confirmed plan before approval reconciles pending orders with the new geometry and does not duplicate them — evidence:
- [ ] No parallel rack-execution mechanism exists — evidence:

Decision status:

- [x] OPEN-1 (Legacy40 saves): `ACCEPTED — V2 APPLIES TO ENLARGED80 ONLY` — evidence: Manager decision 2026-10-26, Decision Log D-064
- [x] OPEN-2 (Work Plan boundary): `ACCEPTED — KEEP rack.approve AND rack.result` — evidence: Manager decision 2026-10-26, Decision Log D-064
- [x] OPEN-3 (reopening after work has progressed): `ACCEPTED — ZERO EXECUTED REMOVALS MAY REOPEN; EXECUTED RACKS LOCKED` — evidence: direct user authority 2026-10-28, Decision Log D-064; implemented/qualified through RACK-V2-05 and RACK-V2-07.

## Spec conformance

- [ ] Implementation matches the approved spec.
- [ ] Approved deviations are recorded in `spec.md` and the handoff.
- [ ] Unresolved requirements are listed, not hidden.
- [ ] Evidence classifications in `spec.md` §39.3 remain accurate (source guidance / gameplay calibration / game-design derivation / model-scene derivation); no unlabelled pseudo-empirical claim was introduced.
- [ ] Existing rack/guided/teaching gates changed or retired are recorded with reasons and G3 review (plan F-10).
- [ ] No unapproved migration or schema change; Reference Future v1 untouched.

## Verdict

Use one of:

`OBJECTIVE 1 RACK PLANNING & FIELD MARKING V2 QUALIFIED — READY FOR USER TEST`

`NEEDS CHANGES`

`BLOCKED — IMPLEMENTATION`

`NEEDS PRODUCT DECISION`

- **Spec conformance:** PASS / FAIL / NOT VERIFIED
- **Implementation correctness:** PASS / FAIL / NOT VERIFIED
- **Notes:**
