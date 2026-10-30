# MFM-09 — Teaching Sequence / Work Plan Integration

## Task packet status

**PREPARED / NOT RELEASED**  
Browser preparation claim: #180  
Parent: #91  
Browser lane: #162  
Required gate on completion: `SCENARIO ONE EXTENSION FLOW QUALIFIED`

This packet is intentionally parked. It does **not** authorize implementation until the Overall Manager republishes/releases it after #151 and #156 are both Unity-qualified, Manager-approved and integrated onto one exact Scenario One lineage.

Preparation reference only: `8fa83bff22ea1b4943689bee1074eed8eae65d2f`.

The release revision must replace every `TBD AFTER PREREQUISITE INTEGRATION` field below with exact evidence.

## Purpose

Add the approved MFM-09 `Extend your treatment` teaching step after the existing Scenario One Lesson 5 and connect prescription-generated Pending work to the ordinary Work Plan flow without changing the established commitment, contractor, landowner, economy, save, Reference Future or treatment-review semantics.

## Classification

SPEC REQUIRED — this is an approved cross-system teaching / Work Plan integration task under Multi-Scale Forestry Management V1.

Difficulty: **Medium** — product semantics are already approved, but the work crosses persisted teaching-step identity and a Work Plan presentation surface currently being changed by #151.

Best-suited implementation lane: **SOL/CODEX local lane** for C#/Unity integration and verification. Use an independent provider for implementation review when useful.

## Preconditions — all mandatory

Do not create the local production claim or edit production until browser release confirms:

- #151 S1-D T-05 Unity qualification complete;
- #151 final Manager verdict APPROVED and integration complete;
- #156 MFM-08 Unity Tier-2 complete;
- #156 final Manager verdict APPROVED and integration complete;
- both integrations are ancestors of the exact current Scenario One integration SHA;
- fresh `Docs/Project/coordination/active-tasks.md` has no conflicting writer on the teaching sequence / Work Plan surface;
- accepted post-integration `WorkPlanView.cs`, teaching-sequence files, persisted existing step IDs and prescription provenance API have been inspected;
- exact production ownership has been repinned here;
- fresh Unity machine preflight is performed before any Unity phase.

If any item is false, STOP and return to the Overall Manager.

## Release fields

Exact start / candidate integration SHA: `TBD AFTER PREREQUISITE INTEGRATION`  
Substantive local claim: `TBD AT RELEASE`  
Owner/model/provider: `TBD AT RELEASE`  
Branch: `feature/mfm-09-teaching-workplan-integration-v1`  
Worktree: `/media/jer/Videos/CCF-MFM09`  
Unity lock: **not claimed by this prepared packet**

Suggested branch/worktree names are not ownership grants. The released task must verify them against live coordination.

## Authority

Implement only the approved Multi-Scale Forestry Management V1 MFM-09 task and its checklist requirements, especially:

- C1–C9 — preserve teaching calibration and existing lesson behaviour;
- I1–I6 — prescription approval versus Work Plan approval, Pending-only creation, ordinary contractor/landowner/economy path, durable provenance;
- K7–K9 — Legacy40, pre-feature saves and frozen Reference Future compatibility;
- L1, L6–L8 — semantic distinction, manual management, stable old IDs and stable new extension IDs;
- N1–N6 — extension after existing Lesson 5, required before external testing, no false S1-D completion claims, manual path, no Windows build;
- O1–O9 — wording/evidence discipline.

No local interpretation may weaken those requirements.

## Product contract

### Teaching sequence

- Existing Scenario One Lessons 2–5 stay functionally unchanged.
- Every existing persisted teaching step ID must remain exactly unchanged.
- Do not reuse an old saved ID for new meaning.
- Add new stable ID(s) for `Extend your treatment`.
- The new extension appears after the existing Lesson 5.
- Visible numbering may change; persisted identity may not.
- Work completed outside the original teaching sample must not satisfy original sample lesson credit.
- The extension must teach continuation into comparable-area treatment without pretending the sample exercise was stand-wide stocking guidance.

At task start, before edits, enumerate the exact existing saved IDs from the accepted candidate in the claim/handoff. If the existing implementation cannot add the extension without changing an old ID, STOP.

### Work Plan integration

The accepted #151 generic operational grouping remains authoritative:

- Approved;
- Pending new;
- Pending carried over.

Prescription origin is provenance/explanation, **not a fourth status**.

MFM-09 may add the minimum presentation needed to help the player understand that a prescription generated ordinary Pending work and where that work sits in the normal Work Plan. It must not change operational classification.

### Commitment boundary

Preserve the MFM-07 contract exactly:

1. treatment review / prescription preview is not committed work;
2. prescription approval creates ordinary Pending work only;
3. Pending work is not a Fell mark and is not approved/executed work;
4. Work Plan approval remains the commitment gate;
5. after commitment, existing contractor / landowner / economy systems remain authoritative;
6. no teaching action may auto-approve, auto-execute, silently confirm marks, or bypass affordability / quotation / minimum-cost behaviour.

### MFM-08 boundary

Do not duplicate or redesign:

- sample-treatment review;
- comparable-area coverage classification;
- Stand Map treatment overlays;
- candidate tree browsing / inspection;
- proposal/conflict review;
- treatment footprint geometry;
- proposal-to-Fell conversion.

Consume the accepted MFM-08/MFM-07 output boundary only.

## Expected production ownership

Final release must narrow this from fresh inspection. Expected smallest set:

- existing Scenario One teaching sequence definition/controller;
- corresponding learning-objectives view/sequence presentation;
- `Assets/ForestPrototype/UI/WorkPlanView.cs` for minimal prescription-origin explanation layered onto #151's accepted generic status groups;
- at most one minimal adapter if genuinely required by the accepted architecture.

Avoid `ScenarioOneUiRoot.cs` unless fresh inspection proves a small adapter is unavoidable; if so, stop before editing and have the Overall Manager dispose the overlap explicitly.

The browser preparation identified `Assets/ForestPrototype/ScenarioOne/ScenarioOneTeachingSequence.cs` in the integration tree, but the connector could not retrieve that blob content from the inspected SHA. Therefore this prepared packet deliberately does not invent the existing saved IDs or exact companion file path. The local pre-edit inspection must provide those facts.

## Explicit exclusions

No changes to:

- save version, schema, migration, validators, compatibility hashes;
- frozen Reference Future v1;
- Annual Review/history behaviour;
- #151 status semantics or exact-year history semantics;
- MFM-08 treatment-review / Stand Map / Tree Inspection files unless an approved dependency repair is separately issued;
- Rack V2 geometry/persistence;
- proposal-to-Fell conversion;
- broadleaf automation policy;
- contractor/landowner/economy calculation semantics;
- existing teaching step identities or existing Lessons 2–5 meaning;
- Windows build/release.

## Implementation approach

Use the smallest compatible change:

1. Inspect and record all existing teaching-step IDs and old Lessons 2–5 completion predicates.
2. Inspect accepted #151 Work Plan grouping/provenance behaviour and accepted MFM-07/MFM-08 interfaces.
3. Add new stable extension ID(s) without touching old IDs.
4. Insert `Extend your treatment` after existing Lesson 5 using existing teaching infrastructure rather than a parallel tutorial system.
5. Make the extension explain/route into accepted treatment review/prescription flow and ordinary Work Plan flow without claiming proposal == work.
6. Add only the minimum Work Plan provenance explanation needed for prescription-generated Pending work; preserve generic Approved / Pending new / Pending carried-over grouping.
7. Add focused deterministic verification before entering Unity.
8. Run the Unity Tier-2 qualification against the exact production candidate.
9. Publish a GitHub Markdown handoff with exact commands, outputs, screenshots/captures, candidate SHA, cleanliness/ownership evidence and residuals.

## Offline / static qualification

The candidate must demonstrate all of the following before Unity:

- exact inventory of old teaching IDs before and after, proving no rename/reuse;
- old Lessons 2–5 predicates unchanged;
- extension ID(s) stable and unique;
- extension ordered after existing Lesson 5;
- outside-sample/comparable-area work cannot award original sample lesson completion;
- prescription-generated orders enter as ordinary Pending;
- prescription provenance does not alter status classification;
- #151 Approved / Pending new / Pending carried-over classification remains intact;
- future/inconsistent-year behaviour from #151 remains intact;
- no extension action approves the Work Plan;
- no extension action confirms Fell marks or executes work;
- normal manual/non-prescription Work Plan operation still works;
- old/pre-feature saves remain valid without fabricated prescription state;
- Legacy40 unaffected;
- frozen Reference Future v1 unaffected;
- no excluded MFM-08 review file or save/hash/reference change.

Run focused mutation tests for at least:

- renamed old ID;
- reused old ID;
- extension inserted before Lesson 5;
- outside-sample work incorrectly satisfying original lesson credit;
- prescription work incorrectly classified as Approved;
- teaching action incorrectly calling Work Plan approval;
- prescription provenance incorrectly creating a new operational status.

Do not weaken existing tests to get green.

## Unity Tier-2 qualification

After static qualification and a fresh serialized Unity preflight, qualify the exact candidate with:

- required Unity version import/compile clean;
- live path through the existing lessons into `Extend your treatment` only after existing Lesson 5;
- rendered/readable teaching and Work Plan presentation at 1280×720 and 1920×1080;
- treatment review/prescription approval creates visible ordinary Pending work with provenance and **does not** commit it;
- Work Plan presents prescription-origin Pending work within accepted #151 generic status groups;
- Pending new versus Pending carried-over remains correct across the ecological-year boundary;
- Work Plan approval is the only commitment step in this flow;
- normal contractor/landowner/economy quote, minimum-cost, affordability and execution path remains authoritative;
- save/load retains teaching progress, old IDs and prescription provenance;
- comparable-area extension work does not back-credit the original sample lesson;
- manual/non-prescription management remains possible;
- MFM-08 treatment review remains visually/semantically distinct from committed Work Plan work;
- regression coverage for S1-D T-04/T-05, MFM-05/MFM-07/MFM-08, Rack V2, save/load and frozen Reference Future compatibility;
- clean scene/save state and clean lane/lock release evidence.

Rendered/runtime claims require captured Unity evidence; static inspection is not enough.

## Acceptance

A final Manager review may return only:

- `APPROVED`;
- `NEEDS CHANGES`;
- `REJECTED`.

`SCENARIO ONE EXTENSION FLOW QUALIFIED` may be recorded only after the required static/offline evidence, Unity Tier-2 evidence, review and accepted exact candidate are all durable.

Successful MFM-09 is a prerequisite to MFM-10. It does **not** itself release PARITY-09.

## Stop conditions

STOP before production repair/expansion if:

- #151 or #156 is not accepted/integrated;
- live coordination shows overlapping ownership;
- an existing saved teaching ID would need rename/reuse;
- save/schema/hash changes are required;
- MFM-08 review semantics require redesign;
- proposal-to-Fell conversion is required;
- broadleaf automated treatment policy is required;
- Work Plan status/commitment semantics need changing;
- contractor/landowner/economy semantics need changing;
- a genuine production defect is found during a verification-only Unity phase outside this packet's owned surface;
- tests would need weakening.

Publish the blocker with exact evidence and return ownership to the Overall Manager.

## Handoff

Required path:

`Docs/Handoffs/MFM-09-Teaching-WorkPlan-Integration/HANDOFF.md`

The handoff must include:

- exact start and candidate SHAs;
- exact owned files;
- old and new teaching IDs;
- implementation summary;
- offline commands/results and mutation evidence;
- Unity commands/results;
- rendered evidence at both required resolutions;
- Work Plan commitment/provenance witnesses;
- save/load and compatibility evidence;
- regression results;
- lane/worktree/Unity-lock/cleanliness state;
- any residuals and evidence classification.

Expected success token:

`SCENARIO ONE EXTENSION FLOW QUALIFIED`

## Copy/paste dispatch line — HOLD, DO NOT USE UNTIL RELEASED

> Run MFM-09 from `Docs/Handoffs/MFM-09-Teaching-WorkPlan-Integration/TASK-PACKET.md` only after the Overall Manager changes its status from PREPARED/NOT RELEASED and pins the exact post-#151/#156 integration SHA and claim. Preserve all existing teaching IDs and Lessons 2–5; add `Extend your treatment` after Lesson 5; keep prescription work ordinary Pending until Work Plan approval; use the normal contractor/landowner/economy path; do not touch save/schema/hash/Reference/MFM-08/Rack/Windows scope; stop on overlap or semantic expansion; publish `HANDOFF.md` with offline + Unity Tier-2 evidence and `SCENARIO ONE EXTENSION FLOW QUALIFIED` only if every gate passes.

User action required: **none while this packet remains unreleased**.
