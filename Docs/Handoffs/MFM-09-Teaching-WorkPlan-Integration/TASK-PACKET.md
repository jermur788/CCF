# MFM-09 — Teaching Sequence / Work Plan Integration

## Task packet status

**PREPARED / NOT RELEASED — #151 SATISFIED; WAITING #156**  
Browser preparation claim: #180  
Parent: #91  
Browser lane: #162  
Required gate on completion: `SCENARIO ONE EXTENSION FLOW QUALIFIED`

This packet does **not** authorize implementation until the Overall Manager releases it after #156 MFM-08 is Unity-qualified, Manager-approved and integrated onto the Scenario One lineage, and this packet is repinned to that exact combined SHA.

Current browser reconciliation reference: `ce6737146a7aa9ea953d7db0ca93b103f6c2c996`.

S1-D T-05 #151 is already `APPROVED / INTEGRATED / CLOSED`; exact production integration merge `46fc6d55315bbddcdfa7667f0da490378d584023`, docs/integration successor `ce6737146a7aa9ea953d7db0ca93b103f6c2c996`.

Detailed browser reconciliation: `Docs/Handoffs/MFM-09-Teaching-WorkPlan-Integration/BROWSER-RECONCILIATION-20261030.md`.

## Purpose

Add the approved MFM-09 `Extend your treatment` teaching lesson after the existing Scenario One Lesson 5 and connect prescription-generated Pending work to the ordinary Work Plan flow without changing the established commitment, contractor, landowner, economy, save, Reference Future or treatment-review semantics.

## Classification

SPEC REQUIRED — approved cross-system teaching / Work Plan integration under Multi-Scale Forestry Management V1.

Difficulty: **Medium** — product semantics are approved, but the work crosses the teaching sequence and the accepted generic Work Plan presentation.

Best-suited implementation lane: **SOL/CODEX local lane** for C#/Unity integration and verification. Use an independent provider for implementation review where required/useful.

## Preconditions — all mandatory

Before local production start, browser release must confirm:

- #156 MFM-08 Unity Tier-2 complete;
- #156 final Manager verdict `APPROVED` and integration complete;
- #151 and #156 integrations are ancestors of the exact current Scenario One integration SHA;
- fresh `Docs/Project/coordination/active-tasks.md` has no conflicting teaching / Work Plan writer;
- accepted post-#156 teaching, Work Plan and prescription interfaces have been inspected;
- exact production ownership/start SHA has been repinned here;
- fresh Unity machine preflight is performed before the Unity phase.

If any item is false, STOP and return to the Overall Manager.

## Release fields

Exact start / combined integration SHA: `TBD AFTER #156 INTEGRATION`  
Substantive local claim: `TBD AT ACTUAL START`  
Owner/model/provider: `TBD AT ACTUAL START`  
Branch: `feature/mfm-09-teaching-workplan-integration-v1`  
Worktree: `/media/jer/Videos/CCF-MFM09`  
Unity lock: **not claimed by this prepared packet**

Suggested branch/worktree names are not ownership grants. Claims track actual execution presence only.

## Authority

Implement only approved Multi-Scale Forestry Management V1 MFM-09 requirements, especially:

- preserve teaching calibration and existing lesson behaviour;
- prescription approval versus Work Plan approval, Pending-only creation, normal contractor/landowner/economy path and durable provenance;
- Legacy40, pre-feature saves and frozen Reference Future compatibility;
- semantic distinction, manual management, stable old IDs and stable new extension ID(s);
- extension after existing Lesson 5 and required before external testing;
- no false S1-D completion claims;
- evidence/wording discipline;
- no Windows build.

No local interpretation may weaken those requirements.

## Exact current teaching authority

Correct source at `ce673714...`:

`Assets/ForestPrototype/UI/ScenarioOneTeachingSequence.cs`

Existing identifiers and order are:

1. `access` — Open access
2. `future-trees` — Choose future trees
3. `release` — Release crowns
4. `habitat` — Retain habitat structure
5. `response` — Review the response
6. `recruitment` — Natural regeneration
7. `establishment` — Protect establishment
8. `transformation` — Transformation ready

The source explicitly describes these as stable **presentation identifiers**, not success-state fields. Preserve all eight exactly. Visible numbering may change.

### Separate accepted v24 completion latch

Do not conflate lesson IDs with durable completion state.

Accepted D-066 / S1-D T-04P uses one separate monotone saved fact:

- `ScenarioOneSaveData.firstGuidedRackReviewComplete`;
- shared read `ScenarioOneManager.FirstGuidedRackReviewComplete`.

It is earned only at the genuine pre-latch `response` → `recruitment` crossing after the real guided rack review. Full saves round-trip it while world/reference/protected identity hashes exclude it. MFM-09 has **no authority** to rename, reset, duplicate or alter that save contract.

The extension must preserve the meaning of an already-completed first guided review across reload and must not reopen it merely because visible teaching indices change.

## Teaching sequence contract

- Existing Scenario One Lessons 2–5 stay functionally unchanged.
- Existing IDs remain exactly unchanged and are never reused for new meaning.
- `Extend your treatment` appears after `response` and before `recruitment`.
- Add one or more new unique stable extension ID(s) only as required by the existing teaching architecture.
- The approved spec defines one player-facing lesson with staged activities but does not mandate the exact count/literal spelling of code-level new IDs. Record the chosen new ID(s) in the implementation claim/handoff and mutation-test rename/reuse; do not manufacture old-ID changes to fit them.
- Work outside the original teaching sample must not satisfy original sample lesson credit.
- The extension teaches comparable-area treatment without presenting sample calibration as stand-wide stocking guidance.
- If the existing implementation cannot insert the extension without changing/reusing an old ID or changing the D-066 latch contract, STOP.

The approved `Extend your treatment` lesson covers:

- derive a prescription from executed sample decisions;
- show fit back to the player's sample;
- review covered/not-covered/excluded/manual areas;
- inspect and confirm/reject spacing-only Crop Tree candidates;
- review proposed release treatment;
- complete at least one extrapolated-unit 3D spot-check;
- revise/exclude/use manual work or another sample where needed;
- approve the prescription;
- create ordinary Pending work with provenance;
- continue to the ordinary Work Plan, which remains the commitment gate.

## Exact accepted Work Plan authority

At current integration, `Assets/ForestPrototype/UI/WorkPlanView.cs` already groups open ordinary work as:

- `Approved`;
- `Pending new`;
- `Pending carried over`.

`OrderClass(...)` owns that classification. Future-created records are surfaced as inconsistent, not invented as a fourth class.

`AddOrderOrigin(...)` already shows prescription provenance when `prescriptionId > 0`, including prescription, coverage-unit, Crop Tree and reason-code origin. Prescription origin is **explanation/provenance, not an operational status**.

MFM-09 may add only the minimum teaching/explanation needed to tell the player why prescription-generated Pending work is present and that Work Plan approval is still required. It must preserve #151 grouping/status/history semantics.

## Commitment boundary

Preserve MFM-07 exactly:

1. `CreatePrescription(...)` stores accepted coverage decisions; it does not create committed work.
2. `PreviewPrescription(...)` is read-only; proposals are not marks/orders/saved proposal state.
3. `ApprovePrescription(...)` creates ordinary **Pending** felling orders with prescription provenance.
4. It does not approve, fund or execute those orders.
5. Work Plan `ApprovePendingWork` remains the commitment gate.
6. Existing contractor, landowner, cash, quote, minimum-cost, affordability and execution rules remain authoritative.
7. No teaching action may auto-approve, auto-execute, silently confirm marks or bypass the ordinary Work Plan.

## MFM-08 boundary

Do not duplicate or redesign:

- sample-treatment review;
- comparable-area coverage classification;
- Stand Map treatment overlays;
- candidate browsing/inspection;
- proposal/conflict review;
- treatment footprint geometry;
- proposal-to-Fell conversion.

Consume accepted MFM-08/MFM-07 outputs only.

## Expected production ownership

Final release must refresh this after #156 integration. Current expected smallest set:

- `Assets/ForestPrototype/UI/ScenarioOneTeachingSequence.cs`;
- `Assets/ForestPrototype/UI/LearningObjectivesView.Sequence.cs` and only the smallest existing teaching companion required;
- `Assets/ForestPrototype/UI/WorkPlanView.cs` for minimal prescription-origin teaching/explanation layered onto accepted generic status groups;
- at most one minimal adapter if genuinely required by accepted architecture.

Avoid `ScenarioOneUiRoot.cs`. If a fresh post-#156 source inspection proves it unavoidable, STOP before editing and obtain explicit Manager overlap disposition.

## Explicit exclusions

No changes to:

- save version/schema/migration/validators/compatibility hashes or D-066 latch semantics;
- frozen Reference Future v1;
- Annual Review/history behaviour;
- #151 status semantics or exact-year history semantics;
- MFM-08 treatment-review / Stand Map / Tree Inspection files unless a separate approved dependency repair is issued;
- Rack V2 geometry/persistence;
- proposal-to-Fell conversion;
- broadleaf automation policy;
- contractor/landowner/economy calculation semantics;
- existing teaching IDs or existing Lessons 2–5 meaning;
- Windows build/release.

## Implementation approach

Use the smallest compatible change:

1. At actual start, re-enumerate the eight existing IDs and inspect #156's accepted delta.
2. Record old Lessons 2–5 completion predicates and D-066 latch boundary before edits.
3. Inspect accepted #151 Work Plan grouping/provenance and MFM-07/MFM-08 interfaces.
4. Add new stable extension ID(s) without touching old IDs.
5. Insert `Extend your treatment` after `response` and before `recruitment` using the existing teaching infrastructure.
6. Explain/route into accepted treatment review/prescription flow and ordinary Work Plan flow without claiming proposal == work.
7. Add only minimum Work Plan provenance teaching/explanation; preserve generic status classification.
8. Add focused deterministic verification before Unity.
9. Run Tier-2 Unity qualification against the exact candidate.
10. Publish the required GitHub handoff with exact evidence.

## Offline / static qualification

Before Unity prove:

- exact old-ID inventory unchanged before/after;
- old Lessons 2–5 predicates and D-066 latch semantics unchanged;
- extension ID(s) unique/stable and ordered after `response`;
- already-completed first guided review stays completed after reload/index changes;
- outside-sample/comparable-area work cannot award original sample lesson completion;
- prescription-generated orders enter ordinary Pending;
- prescription provenance does not alter status classification;
- #151 Approved / Pending new / Pending carried-over and future/inconsistent-year behaviour remain intact;
- no extension action approves Work Plan, confirms Fell marks or executes work;
- manual/non-prescription Work Plan operation remains valid;
- old/pre-feature saves remain valid without fabricated prescription state;
- Legacy40 and frozen Reference Future v1 unaffected;
- no excluded MFM-08 review file or save/hash/reference change.

Mutation tests must catch at least:

- renamed old ID;
- reused old ID;
- extension inserted before/completing without `response`;
- D-066 first-review latch reset/reinterpreted by extension insertion;
- outside-sample work incorrectly satisfying original lesson credit;
- prescription work incorrectly classified as Approved/fourth status;
- teaching action incorrectly calling Work Plan approval;
- provenance incorrectly changing operational status.

Do not weaken tests to get green.

## Unity Tier-2 qualification

After static qualification and fresh serialized Unity preflight, qualify:

- required Unity version import/compile clean;
- live path through existing lessons into `Extend your treatment` only after existing Lesson 5;
- completed first guided review remains remembered after save/load;
- rendered/readable teaching and Work Plan at 1280×720 and 1920×1080;
- treatment review/prescription approval creates visible ordinary Pending work with provenance and does **not** commit it;
- prescription-origin work remains inside accepted #151 generic status groups;
- Pending new vs carried-over stays correct across an ecological-year boundary;
- Work Plan approval is the only commitment step;
- normal contractor/landowner/economy quote, minimum-cost, affordability and execution path remains authoritative;
- save/load retains accepted teaching completion and prescription provenance without new MFM-09 schema;
- comparable-area work does not back-credit original sample lesson;
- manual/non-prescription management remains possible;
- MFM-08 review remains visually/semantically distinct from committed Work Plan work;
- regressions for S1-D T-04/T-05, MFM-05/MFM-07/MFM-08, Rack V2, save/load and frozen Reference Future compatibility;
- clean scene/save/lane/lock evidence.

Rendered/runtime claims require captured Unity evidence.

## Acceptance

Final Manager verdict is one of:

- `APPROVED`;
- `NEEDS CHANGES`;
- `REJECTED`.

`SCENARIO ONE EXTENSION FLOW QUALIFIED` may be recorded only after required static/offline evidence, Tier-2 Unity evidence, review and accepted exact candidate are durable.

Successful MFM-09 is prerequisite to MFM-10. It does **not** release PARITY-09 by itself.

## Stop conditions

STOP before production repair/expansion if:

- #156 is not accepted/integrated;
- live coordination shows overlap;
- an old teaching ID would need rename/reuse;
- D-066 latch/save semantics would need alteration;
- save/schema/hash changes are otherwise required;
- MFM-08 review semantics require redesign;
- proposal-to-Fell or broadleaf automation is required;
- Work Plan status/commitment or contractor/economy semantics need changing;
- a production defect is found during a verification-only phase outside owned surface;
- tests would need weakening.

Publish exact blocker and return ownership to Overall Manager.

## Handoff

Required path:

`Docs/Handoffs/MFM-09-Teaching-WorkPlan-Integration/HANDOFF.md`

Include exact start/candidate SHAs, owned files, old/new IDs, implementation summary, offline/mutation evidence, Unity/render evidence, Work Plan commitment/provenance witnesses, save/load/compatibility results, regressions, lane/worktree/lock/cleanliness and residuals.

Expected success token:

`SCENARIO ONE EXTENSION FLOW QUALIFIED`

## Copy/paste dispatch line — HOLD, DO NOT USE UNTIL RELEASED

> Run MFM-09 from `Docs/Handoffs/MFM-09-Teaching-WorkPlan-Integration/TASK-PACKET.md` only after the Overall Manager changes status from PREPARED/NOT RELEASED and pins the exact post-#156 combined integration SHA and claim. Preserve `access`, `future-trees`, `release`, `habitat`, `response`, `recruitment`, `establishment`, `transformation` and the accepted v24 first-guided-review latch; insert new `Extend your treatment` teaching after `response` and before `recruitment`; keep prescription work ordinary Pending until Work Plan approval; use normal contractor/landowner/economy; do not touch save/schema/hash/Reference/MFM-08/Rack/Windows scope; publish `HANDOFF.md` with offline + Unity Tier-2 evidence and `SCENARIO ONE EXTENSION FLOW QUALIFIED` only if every gate passes.

User action required: **none while this packet remains unreleased**.
