# MFM-09 Browser Readiness — Teaching / Work Plan Integration

Date: 2026-10-30  
Browser claim: #180  
Parent milestone: #91  
Browser coordination lane: #162  
Prepared by: Overall Manager / OpenAI GPT-5.6 Sol  
Status: **PREPARED — NOT RELEASED FOR LOCAL IMPLEMENTATION**

## Verdict

`MFM-09 BROWSER PACKET PREPARED — WAITING FOR #151 + #156 INTEGRATION`

MFM-09 can be prepared in the browser now, but production implementation must not start until both prerequisite feature lanes have completed their mandatory Unity qualification, received Manager approval, and been integrated onto one exact Scenario One candidate lineage.

This document is preparation evidence only. It does not grant a production lock, authorize a local worktree, claim Unity verification, or release MFM-09 implementation.

## Authority

Primary task authority is the approved Multi-Scale Forestry Management V1 spec, especially MFM-09 in `specs/multiscale-forestry-management-v1/tasks.md` and requirements C1–C9, I1–I6, K7–K9, L1/L6–L8, N1–N6, and O1–O9 in `specs/multiscale-forestry-management-v1/checklists/requirements.md`.

Required MFM-09 gate: `SCENARIO ONE EXTENSION FLOW QUALIFIED`.

The next downstream gate remains MFM-10 `SPECIES GUARD QUALIFIED`; PARITY-09 must stay blocked until that MFM-10 qualification is durably published.

## Preparation baseline — not an implementation start SHA

Current published integration reference during browser preparation:

`8fa83bff22ea1b4943689bee1074eed8eae65d2f`

This SHA is a reference point only. It is **not** the MFM-09 implementation start SHA because #151 and #156 are not yet both accepted and integrated.

Prerequisite candidates observed during preparation:

- #151 — S1-D T-05 previous/proposed intervention presentation: exact handed-off head `4ddf4aa535d4ec5267be46d2635679411d595e0f`. Offline/static phase approved; mandatory Unity qualification is still running. Browser inspection shows this candidate owns `WorkPlanView.cs` plus Annual Review / Scenario One UI presentation and therefore directly affects the MFM-09 integration surface.
- #156 — MFM-08 map / 3D review UX: production `d55ec86a3f0bf13820f2080197b6b0f43d832e06`, documentation tip `d70cec1781c104794a6f225c8609847c55df1510`. Static browser review is approved; mandatory Unity Tier-2 still waits behind #151. Its production surface is the treatment-review / Stand Map / Tree Inspection presentation lane and must not be duplicated by MFM-09.

## Product semantics already fixed by the approved spec

MFM-09 must:

1. insert the new `Extend your treatment` teaching step **after the existing Lesson 5**;
2. preserve every existing saved teaching step ID and existing Lessons 2–5 behaviour;
3. add new stable IDs for the extension rather than renaming or reusing old saved keys;
4. allow visible lesson numbering to change without changing persisted identifiers;
5. connect prescription-generated work to the ordinary Work Plan rather than inventing a parallel commitment path;
6. preserve MFM-07 semantics: prescription approval creates ordinary **Pending** work only; it does not create Fell marks, approve the Work Plan, or execute work;
7. preserve Work Plan approval as the commitment gate;
8. preserve the existing contractor / landowner / economy path as authoritative after commitment;
9. keep MFM-08 sample, comparable-area, candidate and proposal review semantics separate from committed work;
10. prevent treatment work outside the original teaching sample from falsely satisfying the original sample lesson credit.

## #151 integration surface

Browser inspection of the exact #151 candidate confirms that its `WorkPlanView.cs` presentation establishes the generic order classes that MFM-09 must build on:

- Approved;
- Pending new;
- Pending carried over.

Prescription-generated work must remain inside those ordinary status classes. MFM-09 may explain or group prescription provenance, but it must not create a fourth operational state, weaken approval semantics, or replace #151's previous/proposed intervention presentation.

The same candidate uses prescription provenance when a valid `prescriptionId` exists. MFM-09 should consume that accepted provenance rather than invent a second provenance system.

Potential overlap with #151 must be re-inspected after #151 is integrated. `WorkPlanView.cs` is expected to be a shared edit surface; `ScenarioOneUiRoot.cs` should be avoided unless the accepted integrated architecture makes a minimal adapter unavoidable.

## #156 boundary

MFM-09 must not absorb or recreate MFM-08's treatment-review responsibilities. In particular, MFM-09 should not implement:

- comparable-area coverage review;
- map treatment overlays;
- candidate tree browsing / inspection controls;
- proposal review or conflict presentation;
- sample-treatment geometry;
- proposal-to-Fell conversion.

MFM-09 starts from the accepted prescription / pending-work boundary and teaches how that work enters and proceeds through the ordinary Work Plan.

## Prepared ownership hypothesis

The smallest likely production surface is:

- the existing Scenario One teaching sequence definition / controller;
- the corresponding learning-objectives presentation that renders that sequence;
- `Assets/ForestPrototype/UI/WorkPlanView.cs` for minimal prescription-origin explanation/grouping on top of #151's accepted generic status presentation;
- one small adapter only if the accepted post-#151/#156 architecture proves it necessary.

The exact teaching-sequence file and the existing persisted step IDs must be enumerated from the accepted post-integration candidate **before editing**. Browser tree inspection identified `Assets/ForestPrototype/ScenarioOne/ScenarioOneTeachingSequence.cs` in the integration tree, but the connector could not retrieve the blob content at the inspected SHA; therefore no existing ID names are invented in this readiness document. The local worker must inspect and list them exactly at task start.

## Explicit exclusions

Unless a new approved decision/spec amendment says otherwise, MFM-09 must not change:

- save/schema/hash versions or migrations;
- frozen Reference Future v1;
- Annual Review/history semantics;
- MFM-08 Stand Map / Tree Inspection / treatment-review UX;
- Rack V2 geometry or persistence;
- proposal-to-Fell conversion;
- broadleaf automated treatment policy;
- contractor/landowner/economy rules;
- generic Work Plan status semantics established by #151;
- existing teaching IDs or the meaning of existing Lessons 2–5.

## Release gate

Before the prepared local packet may be released, the Overall Manager must verify all of the following against fresh GitHub evidence:

1. #151 has completed Unity qualification, has a final Manager/browser verdict of APPROVED, and is integrated.
2. #156 has completed Unity Tier-2, has a final Manager/browser verdict of APPROVED, and is integrated.
3. Both integrations are ancestors of one exact current Scenario One integration SHA.
4. `Docs/Project/coordination/active-tasks.md` shows no conflicting writer on the teaching sequence / Work Plan surface.
5. The exact accepted `WorkPlanView.cs`, teaching sequence files, stable IDs, and prescription provenance API have been re-inspected.
6. The packet is repinned from this preparation baseline to that exact post-prerequisite integration SHA.
7. A new substantive local implementation claim is created and marked `[RUNNING]` with model/provider, branch, worktree and exact starting HEAD before production edits.
8. The worker fresh-preflights the Unity machine ownership before any Unity phase.

If any prerequisite has changed the required semantics or ownership surface materially, stop and revise the packet before release.

## Prepared qualification design

### Static / offline

The released implementation must prove, at minimum:

- all pre-existing saved teaching step IDs are byte-for-byte unchanged;
- all pre-existing Lessons 2–5 completion conditions remain unchanged;
- new extension IDs are stable and do not reuse old IDs;
- the extension occurs only after existing Lesson 5;
- work outside the original sample cannot award original sample lesson credit;
- prescription-generated orders remain ordinary Pending orders before Work Plan approval;
- prescription provenance is explanatory and does not change operational status;
- #151's Approved / Pending new / Pending carried-over classification remains authoritative;
- no teaching action silently approves the Work Plan, confirms Fell marks, executes work, or advances the economy;
- old/pre-feature saves remain valid without fabricated prescription state;
- Legacy40 remains unaffected;
- frozen Reference Future v1 remains unaffected;
- manual/non-prescription Work Plan operation remains available;
- MFM-08 review files and semantics remain untouched unless a separately approved dependency repair is required.

### Unity Tier-2

The released implementation must then qualify the exact candidate in Unity, including:

1. import/compile on the required project Unity version;
2. live teaching progression into `Extend your treatment` only after the existing Lesson 5;
3. rendered/readable extension teaching and Work Plan presentation at 1280×720 and 1920×1080;
4. prescription approval creating ordinary Pending work with visible provenance but no Work Plan commitment;
5. #151's Approved / Pending new / Pending carried-over grouping still correct, including year-boundary/carry-over cases;
6. Work Plan approval committing through the existing authoritative path only;
7. ordinary contractor/landowner/economy quotation, minimum-cost, affordability and execution behaviour remaining authoritative;
8. save/load retaining stable teaching progress and prescription provenance without changing old IDs;
9. no false original-lesson credit from comparable-area extension work;
10. manual Work Plan path remaining usable;
11. regressions for S1-D T-04/T-05, MFM-05/MFM-07/MFM-08, Rack V2, save/load and frozen Reference Future compatibility;
12. clean save / lane / ownership evidence for the exact qualified candidate.

No Windows build is authorised.

## Stop conditions

Stop and return to the Overall Manager if any of the following is discovered:

- #151 or #156 is not accepted and integrated;
- another owner holds an overlapping production lock;
- the extension requires renaming/reusing an existing saved teaching ID;
- implementation requires a save/schema/hash migration;
- MFM-08 treatment-review semantics need redesign rather than simple consumption;
- a new broadleaf automated treatment rule is required;
- Work Plan commitment/status semantics need changing;
- normal contractor/landowner/economy rules would need replacement;
- tests would have to be weakened to pass.

## Next step

Keep this packet parked until #151 and #156 are both Manager-approved and integrated. Then perform a fresh browser release audit, pin the exact integrated SHA, enumerate the existing teaching IDs and final ownership surface, create the local implementation claim, and dispatch MFM-09.

User action required now: **none**.
