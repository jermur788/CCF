# TASK PACKET — RACK-V2-COMBINED-REPAIR-01

**Manager:** ChatGPT Game Dev / Overall Manager  
**Feature:** Scenario One Objective 1 Rack Planning & Field Marking V2 (D-064)  
**Task class:** PACKET ONLY — bounded verification repair  
**Status:** READY / DISPATCHED  
**Target lane:** TTree Code — local Unity-capable Game Dev repair lane  
**Branch:** `integration/scenario-one-rack-v2-v1`  
**Required starting publication:** `9127c025e539ee09ffa883f0cb8510167fedcc14`  
**Exact failed tested candidate:** `96d9abe2b32a53e744b30c79c2ac7b194350dcfc`

## Purpose

Repair the measured combined Rack V2 teaching/progression failure without redesigning Rack V2 or weakening any verification gate.

On the exact combined candidate, five confirmed V2 racks create 149 rack-owned orders and all 149 become Approved, but the legacy rack-layout consumer remains `0/5`. `rack.approve` is therefore never recorded and the required chain stops before execution, `rack.result`, subsequent teaching and the completed-state save round trip.

Manager review: `Docs/Handoffs/RACK-V2-Combined-Integration/MANAGER-REVIEW.md`  
Returned evidence: `Docs/Handoffs/RACK-V2-Combined-Integration/HANDOFF.md`

## Authority and boundary

D-064 / approved spec §39.7 OPEN-2 already requires the sequence:

`Plan Racks → Confirm Rack Plan → Pending rack work → field marking → rack.approve → execution → rack.result`.

This repair implements that already-approved boundary for authoritative V2 rack state. It is not permission to redesign progression.

**OPEN-3 remains OPEN.** Do not change the rules for reopening a rack after approval, partial execution or complete execution. If this task requires such a change: **STOP — OPEN-3 PRODUCT DECISION REQUIRED**.

Tasks 04–10 remain separate. Do not add map editing, minimap V2 propagation, first-person authoring removal, orange marking, Help copy or other pending player-facing Rack V2 scope merely to make this repair pass.

## Allowed repair surface

Primary inspected coupling:

- `Assets/ForestPrototype/UI/RackLayoutTracker.cs`
- `Assets/ForestPrototype/UI/LearningObjectivesView.cs`
- `Assets/ForestPrototype/UI/LearningObjectivesView.Sequence.cs`

Narrow supporting changes are allowed only if required to consume an existing authoritative confirmed-rack API/state from the already-qualified 03C runtime. Do not introduce a second rack model, persisted objective counter or duplicate work lifecycle.

Preserve model-0/Legacy40 behavior. For V2, derive progress from authoritative persisted/confirmed rack objects and rack-owned work/result state rather than historical authored layout lines.

## Required behaviour

On fresh model-1 Enlarged80 / Rack V2:

1. confirmed V2 rack objects provide the authoritative rack count/progression basis;
2. five valid confirmed racks can satisfy the rack-layout teaching boundary without requiring the historical fixed authored lines;
3. approval of the rack-owned Work Plan records `rack.approve` when no rack-owned work remains Pending and approved/completed rack-owned work exists;
4. execution/result can record `rack.result` using defensible authoritative V2 completion/result state;
5. subsequent Scenario One teaching remains reachable;
6. save/load and cold Continue do not rely on session-only cache facts;
7. player-facing copy must not falsely claim tasks 04–10 are implemented.

Do not infer completion from unrelated ordinary FellTree orders. Prefer rack IDs/confirmed-rack ownership already present in the qualified runtime.

## Verification

Use the machine-wide Unity lock normally. No force-release and no Windows build.

On the **exact repaired HEAD**, rerun at minimum:

1. compile/import clean;
2. the real combined write + separate cold-start Continue chain;
3. prove the same five rack IDs/state survive cold Continue;
4. mark/approval path produces the correct 149 approved rack-owned orders (or explain any deterministic count change caused by a separately justified repair; do not rebaseline casually);
5. `rack.create`, rack-layout progression and `rack.approve` record correctly;
6. continue through execution and prove `rack.result`;
7. reach the subsequent teaching state required by the combined packet;
8. prove the completed-state second save round trip;
9. authoritative runtime gate PASS;
10. RackV2Save PASS;
11. headland/access gates PASS;
12. all seven protected fingerprints remain exact:
   - `5546B9B3B50673B2`
   - `94589C749852F359`
   - `92F9635566BF6AFE`
   - `7AD177B3CC2F73C7`
   - `BFFF66015A6619DA`
   - `9C77EB4BEE427FB9`
   - `702766DECE591E21`;
13. focused model-0/Legacy40 progression regression demonstrating the repair did not route legacy saves through V2 predicates;
14. inspect relevant 1280×720 and 1920×1080 captures if the repair changes visible objective/HUD/progress copy.

Do not weaken, delete, bypass or force-credit a failing assertion merely to obtain green. Any harness change that alters what is measured must be separately identified for G3 review.

## Publication / handoff

Publish:

`Docs/Handoffs/RACK-V2-Combined-Repair-01/HANDOFF.md`

Include:

- starting publication SHA and exact repaired/tested HEAD;
- files changed and why;
- precise root cause;
- before/after lesson predicate behavior;
- commands/gates and exact results;
- protected fingerprint table;
- model-0/Legacy40 result;
- cold Continue → approve → execute → `rack.result` → subsequent-objective evidence;
- completed-state save-round-trip evidence;
- rendered evidence if presentation changed;
- whether OPEN-3 was untouched;
- recommendation `APPROVE` / `NEEDS CHANGES` / `REJECT` for the repaired Rack V2 core candidate.

Push the integration branch without force. Do not merge to `main`.

## Exit token

`RACK V2 COMBINED REPAIR QUALIFIED — READY FOR MANAGER REVIEW`

or a precise blocker.

No user acceptance, whole Objective 1 playability, external-demo readiness, S1-D completion, Multi-Scale Forestry Management readiness or Windows-build readiness is implied by this repair alone.
