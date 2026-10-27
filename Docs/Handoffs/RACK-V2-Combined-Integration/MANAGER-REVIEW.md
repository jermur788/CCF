# RACK-V2-COMBINED-INTEGRATION — MANAGER REVIEW

**Verdict: NEEDS CHANGES.**

Returned handoff publication: `9127c025e539ee09ffa883f0cb8510167fedcc14`  
Exact tested combined candidate: `96d9abe2b32a53e744b30c79c2ac7b194350dcfc`

## Accepted evidence

The combined candidate successfully preserves the qualified 02B-R and 03C production bytes and demonstrates that the two Rack V2 foundation lanes coexist. The following evidence is accepted for this review:

- authoritative runtime gate PASS;
- RackV2Save PASS;
- headland qualification PASS;
- headland/access PASS;
- Legacy40 compatibility gate PASS;
- corrected historical Enlarged80 gate PASS;
- Reference Future v1 PASS;
- both completion compatibility gates PASS;
- all seven protected fingerprints EXACT;
- real disk write plus cold-start Continue reaches the approved V2 rack work with exact persisted identities/state;
- 149 rack-owned orders become Approved;
- no protected threshold/hash was weakened to obtain green;
- original `RackUxRack` remains unchanged and its retained/replaced/retired mapping is evidence for G3 review, not a fabricated PASS.

The broad `main..candidate` diff is a lineage issue, not evidence of 457 Rack files. Any eventual G4 integration must review that preceding Scenario One lineage deliberately.

## Blocking failure

The required combined progression gate fails after all 149 rack-owned orders are approved:

- legacy RackLayoutTracker remains `0/5`;
- `rack.approve` is not recorded;
- execution, `rack.result`, subsequent teaching and completed-state round trip are not reached.

The failure is caused by V2 teaching/progression consumers still depending on historical authored rack lines. The measured coupling is in/through:

- `Assets/ForestPrototype/UI/RackLayoutTracker.cs`;
- `Assets/ForestPrototype/UI/LearningObjectivesView.cs` (`ObserveRacks` / rack lesson predicates and presentation derived from them);
- `Assets/ForestPrototype/UI/LearningObjectivesView.Sequence.cs` (access/subsequent teaching geometry still derived from legacy layout lines).

The repair must make confirmed Rack V2 snapshots/authoritative V2 state satisfy the already-approved `rack.create` → `rack.layout` → `rack.approve` → execution → `rack.result` sequence while preserving the model-0/Legacy40 path.

## Scope decision

This is a bounded verification repair, not a product redesign. D-064 / spec §39.7 OPEN-2 already accepts `rack.approve` and `rack.result`; the failure occurs at that accepted boundary.

OPEN-3 remains unresolved and is **not** authorised by this repair. Do not change reopening rules after approval/partial execution/completed work. If the repair reaches or requires OPEN-3, STOP and return to Manager/user.

Tasks 04–10 remain separate player-authoring/field-marking/UI work. This foundation is not yet a player-playable replacement Objective 1.

## G3 / RackUxRack disposition

The submitted F-10 mapping is accepted as a measurement/disposition record for this candidate: legacy assertions may be retained, replaced or V2-retired only where the approved spec supersedes that workflow, and model-0 compatibility remains protected. This does **not** convert the failing diagnostic RackUx runs into qualification passes and does not waive replacement coverage still assigned to tasks 04–10.

## Required next action

Execute the bounded repair packet at:

`Docs/Handoffs/RACK-V2-Combined-Repair-01/TASK-PACKET.md`

The repaired exact HEAD must rerun the real cold Continue → approve → execute → `rack.result` → subsequent-objective chain plus focused regression sufficient to prove the change did not disturb runtime/save/headland/protected fingerprints/model-0 compatibility.

Do not merge to `main` until a new handoff returns and the Manager issues a new G4 decision.

**DASHBOARD SYNC: NOT AVAILABLE**  
**Published dashboard reflects this task: NO**
