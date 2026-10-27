# RACK-V2-COMBINED-REPAIR-01 — MANAGER REVIEW

**Verdict: APPROVED FOR CANONICAL INTEGRATION.**

Returned handoff branch tip: `1147f527bd4adb931152f4d198b3bbe13876a836`  
Exact repaired / tested production HEAD: `7b557fd43ca9468a1be22c161c24064eec9f955a`  
Claim: GitHub issue #7 — `[HANDED-OFF]`, closed.

## G2 acceptance

The bounded repair closes the blocking combined-integration defect on the exact tested HEAD:

- cold Continue restores the same five V2 rack IDs/state;
- all 149 rack-owned orders become Approved and then Completed;
- `rack.create`, rack-layout progression, `rack.approve` and `rack.result` record correctly;
- lesson 2 becomes reachable after execution;
- the completed-state second save round trip passes;
- authoritative runtime, RackV2Save, headland and access gates pass;
- all seven protected fingerprints remain exact;
- model-0 Enlarged80 and Legacy40 progression traces are identical before/after the repair;
- compile/import is clean;
- the player save hash is unchanged;
- rendered 1280×720 and 1920×1080 captures reviewed by the worker are consistent with measured state.

The repair is bounded to three production files in the packet-authorised consumer surface. The Rack runtime, persistence model, headland generation and save schema are unchanged.

## G3 — harness corrections

**ACCEPTED.** Neither correction weakens a protected threshold, fingerprint or behavioural requirement.

1. The combined Stage-2 cue assertion formerly required the stale optional-practice title `Inspect a living tree`. The replacement checks the actual sequential-teaching state: Rack teaching has ended, lesson index 1 is active and the cue contains the lesson-2 title. The following `[X]` / `[C]` interaction checks remain unchanged.
2. The new Legacy40 regression initially expected `rack.create` to rederive after reload. Running the same gate bytes on the pre-repair candidate proved that this is not historical behaviour. The corrected expectation records the measured baseline and the repaired trace is identical.

The new Legacy40/model-0 regression adds coverage and does not replace or suppress an existing protected gate.

## G4 / canonical-integration decision

**APPROVED.** Current `main` differs from the combined branch's merge base only by management/coordination documentation. No production file changed on `main` after that merge base. Therefore canonical integration may merge `integration/scenario-one-rack-v2-v1` into current `main` provided the merge is clean and a post-merge comparison proves the production tree is byte-equivalent to the qualified branch tip/tested production state except for documentation-only handoff commits.

No new Unity run is required solely for the docs-only `main` delta. If the merge introduces a production conflict or changes any production byte relative to the qualified candidate, STOP and requalify the affected surface.

## Teaching-area compatibility decision

The returned handoff correctly identified that Objectives 2–5 still derive `ScenarioOneRackBay` from the historical middle authored rack line.

Existing authority already fixes the intent: the accepted rack-bay lesson manages **one teaching area associated with one rack**, while Rack V2 makes rack geometry player-authored. Therefore, before full Rack V2 Objective-1 functional qualification:

- **V2:** the Objectives 2–5 teaching bay must use a deterministic **central confirmed rack** from the authoritative confirmed V2 rack set;
- deterministic selection should preserve the old "middle rack" intent by ordering confirmed racks across the stand/headland and taking the median, with stable rack ID only as a tie-breaker;
- **model-0 / Legacy40:** retain the historical middle authored layout-rack behaviour unchanged;
- no new forestry target, quota, save field or persisted lesson state is authorised;
- if the V2 confirmed set is missing/invalid at the lesson boundary, fail visibly/return to the existing rack-plan correction path rather than silently falling back to an unrelated authored line.

This is a compatibility adaptation of the existing accepted lesson design, not a new management mechanic. It must be implemented and verified before RACK-V2-12 functional qualification; it does not block integration of the qualified Rack V2 core foundation.

## Remaining limitations

- Rack V2 player authoring/field-marking workflow tasks 04–10 are still incomplete.
- Legacy `[B]` / map copy and Stand Map/minimap V2 presentation remain pending within those tasks.
- The current core was driven through APIs/synthetic input, not a human playthrough.
- OPEN-3 (reopen after approval/partial/completed execution) remains unresolved and untouched.
- No Windows build is authorised.

**DASHBOARD SYNC: NOT AVAILABLE**  
**Published dashboard reflects this task: NO**
