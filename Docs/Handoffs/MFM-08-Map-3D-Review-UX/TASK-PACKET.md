# MFM-08 — Stand Map / 3D Review UX Implementation Packet

**Claim:** #156  
**Parent:** #91 / #106  
**Authority:** approved `specs/multiscale-forestry-management-v1/` MFM-08 + `Docs/Handoffs/MFM-08-UX-Readiness-20261029/BROWSER-RESULT.md`  
**Status:** **BLOCKED ON #129 MANAGER-APPROVED / INTEGRATED LINEAGE — DO NOT START PRODUCTION**

## Purpose

Implement and Tier-2 qualify the smallest faithful MFM-08 Stand Map + 3D review UX for Multi-Scale Forestry Management V1.

The player must be able to review the demonstrated sample treatment, understand comparable-area coverage and reasons, inspect/confirm spacing-only Crop Tree candidates in 3D, review noncommitted removal proposals, complete the required extrapolated-unit 3D spot-check, and approve the prescription while clearly understanding that Work Plan approval is still required.

This packet does not author the MFM-09 teaching sequence and does not change simulation, coverage calibration, persistence or Work Plan commitment semantics.

## Classification

**SPEC REQUIRED — implementation task under the already APPROVED MFM V1 spec.**

Difficulty: **4/5** — substantial two-surface UX plus interaction/visual qualification; bounded to established MFM-04/05/07 state and the final #129 read-only coverage façade.

Best-suited model/provider: **local SOL/Codex high-reasoning Unity/UI lane**, or another capable local Unity worker with repository + Editor access.

## Planned lane

- Branch: `feature/mfm-08-map-3d-review-ux-v1`
- Worktree: `/media/jer/Videos/CCF-MFM08`
- Starting HEAD: **TBD — must be repinned to the exact Manager-approved/integrated #129 umbrella SHA before `[RUNNING]`**
- Claim status before production: `[BLOCKED]`
- Claim status at actual start: mark `[RUNNING]` with model/provider, branch, worktree and exact starting HEAD.

Do not create the worktree/branch against the current local #129 candidate.

## Hard prerequisites

Before any production edit:

1. refresh live `Docs/Project/coordination/active-tasks.md` and #91/#106/#129/#147/#151/#156;
2. verify #129 has a pushed exact candidate, required Unity/regression evidence, Manager `APPROVED` verdict and integration into the current Scenario One player-test umbrella;
3. repin this packet/claim to that exact integrated SHA;
4. verify no overlapping UI writer;
5. if `ScenarioOneUiRoot.cs` or any #147-owned seam is required, #147 must have handed off/released it or Manager must provide an explicit API/ownership disposition;
6. verify the local worktree is clean at the pinned start;
7. acquire the machine-wide Unity lock only for the allowed qualification phases and only after checking required free-space floor/process state.

If any prerequisite is false, remain BLOCKED and do not edit production.

## Accepted semantic contract

Do not rediscover or alter these product decisions:

- wording is **apply your demonstrated/sample treatment to comparable areas**;
- `operator selection` is not presented as formal Irish terminology;
- `Covered` means comparable/eligible, not selected, approved or committed;
- `Not Covered`, `Excluded` and `Manual` remain explicit distinct states;
- coverage reasons are readable;
- spacing-only candidates are prompts to inspect, not quality recommendations;
- every candidate confirmation is grounded in 3D player judgement;
- no bulk Crop Tree confirmation;
- proposals are not Fell marks and are recomputed current-state data;
- player state/conflicts win over a proposal;
- at least one extrapolated target unit must be spot-checked in 3D before prescription approval;
- prescription approval creates ordinary Pending work only;
- Work Plan approval remains the commitment gate;
- MFM-09 owns teaching/curriculum and prescription-specific Work Plan teaching.

## Required API contract

### Final #129 coverage read

Consume the exact final integrated **`ScenarioOneManager.ReadCoverage`** façade after inspecting its integrated signature.

It must remain the thin side-effect-free read-only path already approved by Manager:

- same authoritative world read used by production prescription approval;
- same `ScenarioOneCoverage.Classify` semantics;
- no cache;
- no persistence;
- no hidden fallback;
- no mutable exposure;
- no alternate UI coverage adapter.

**Do not** rebuild masks/features in UI, duplicate path/clearing radii, use reflection, sample protected polygons, or make coverage depend on current player position.

If the final integrated #129 lineage does not expose this façade, STOP. The only permitted API request is the smallest thin read-only `ReadCoverage` accessor matching the approved semantics; do not implement an alternate adapter inside MFM-08.

### MFM-04 coverage data

Use `ScenarioOneCoverage.Result` / `Area` / `Remainder` read data directly for:

- source sample identity;
- Covered / NotCovered / Excluded / Manual classification;
- reasons and flags;
- area ids/rack/segment identity;
- evidence/metrics required to explain the result.

Do not change calibration/tolerances or core classification semantics.

### MFM-05 candidates

Use the existing `ScenarioOneCropTreeCandidates` deterministic batch and inspection helpers:

- `TryGetInspectionTarget(...)`;
- `TryMoveInspectionTarget(... Previous/Next ...)`;
- `InspectionTarget` tree/area/order/X/Z/reason;
- Open / Confirmed / Rejected state.

Confirmation uses the existing authoritative Crop Tree designation path. Rejection is review/session state; do not add save/schema or a second tree mark. The map does not confirm candidates.

### MFM-07 prescriptions / proposals

Use the existing boundary:

- `FindPrescription(int)`;
- `CreatePrescription(...)`;
- read-only `PreviewPrescription(int)`;
- read-only `CheckPrescriptionLink(...)` where useful;
- `ApprovePrescription(int)` only at the final MFM-08 prescription approval action.

Do not store proposal visuals. Do not convert proposals into Fell marks. Do not bypass the current pre-write validation.

## Player-facing implementation requirements

### A. Stand Map treatment-review surface

Implement a bounded **Treatment review** surface using the existing Stand Map interface convention (layer/tool inside the map, not a new permanent global shortcut).

It must show:

1. **Sample — demonstrated treatment** as a source footprint distinct from extrapolated coverage;
2. all current target areas/remainders with classification;
3. readable classification/exclusion reasons;
4. separate player draft choice for `Covered` units: **Apply sample treatment here** / **Leave manual**;
5. candidate states and navigation;
6. proposal states/reasons/conflicts once a stored Accepted prescription exists;
7. review progress, especially the 3D spot-check requirement.

An invalid production coverage read shows the returned problem and disables actions; it never falls back to alternate coverage.

### B. Coverage wording and reasons

Use plain language. At minimum map the closed core reasons as specified in `BROWSER-RESULT.md`.

Keep informational flags (`NaturalRegeneration`, `PlayerCropTrees`) semantically separate from exclusions.

Do not fabricate `ProtectedArea` evidence. If production coverage fails closed because authoritative protected geometry cannot be represented, show the problem and stop actions.

### C. Draft area choices

Area Include/Manual choices remain presentation draft state until the user explicitly prepares a prescription snapshot.

Only `Covered` units may be included. `NotCovered` and `Excluded` are inspectable but cannot be force-included by this UI.

### D. Candidate queue and 3D judgement

Map candidate card/actions:

- Candidate state: Open / Confirmed / Rejected;
- spacing-only warning;
- Previous / Next;
- Inspect in forest.

Do not confirm from the map.

In 3D, make the contextual inspection state clear without replacing authoritative tree facts. The player must be able to confirm via the normal Crop Tree designation path or reject the candidate through a contextual review action. Do not add a permanent global shortcut merely for MFM.

The current normal selection indicator/inspection grammar should be reused where possible. Exact contextual key/button may follow the existing tree-inspection convention; it is an implementation detail, provided it is discoverable in the contextual hint and not a global shortcut accumulation.

### E. Accepted prescription snapshot

After area choices and relevant Crop Tree judgements, expose **Prepare prescription for review** (or equivalent explicit non-commitment wording) and call `CreatePrescription(...)`.

This creates an Accepted snapshot only. No work and no tree mark is created by this step.

Do not add amend/delete persistence. If the player revises choices after preview, a new Accepted snapshot may be created; prior unapproved snapshots remain inert and must never be automatically approved.

### F. Proposal review

For the selected Accepted prescription, use `PreviewPrescription` and show each proposal with:

- target tree;
- target area;
- confirmed Crop Tree served;
- reason: high-share competitor around a confirmed Crop Tree;
- useful current explanation (share/basal area where legible);
- conflicts/holds surfaced by production logic.

Every proposal must be explicitly labelled as **proposal / not a Fell mark / not yet work**.

### G. Required 3D spot-check

Prescription approval remains disabled until at least one extrapolated unit in the active prescription has been actually reviewed in 3D.

Implement the presentation/session gate:

1. select target unit;
2. Inspect in forest;
3. walk to the area using existing waypoint/navigation behaviour — no teleport;
4. inspect a relevant candidate/proposal/tree in that target unit;
5. mark that target unit spot-checked in presentation/session state;
6. show completion on return to Treatment review.

Setting a map waypoint alone is not completion.

Do not persist the spot-check latch. Reset it when the active snapshot is replaced or after reload.

### H. Prescription approval

Only after valid active snapshot + current proposal preview + completed 3D spot-check, expose **Approve prescription**.

Call the existing MFM-07 `ApprovePrescription` boundary. On success, clearly state:

**Pending tasks were added to the Work Plan; they are not approved for execution yet.**

Do not auto-open/approve/fund/execute the Work Plan as part of this action unless the existing generic UI already opens it purely for navigation without mutation. MFM-09 owns prescription-specific Work Plan teaching.

## Visual separation requirements

Do not invent a new product-level final palette. The final implementation may choose bounded UI colours/icons during implementation, but semantic separation must be proven by **form + text/pattern**, not hue alone.

### Allowed review grammar

- map area outlines/fills/hatching + legend;
- hollow/outlined candidate and proposal glyphs with different shapes/patterns;
- existing neutral selected-tree ground ring/trunk outline for the item currently being inspected;
- existing waypoint/navigation treatment;
- explicit chips/labels (`CANDIDATE`, `PROPOSAL`, `CONFLICT`, `CONFIRMED CROP TREE`).

### Forbidden collisions

Before authoritative state changes, MFM review visuals must not reuse:

- Fell-mark paint/symbol grammar for proposals;
- Crop Tree trunk-band/mark grammar for open candidates;
- Rack V2 physical entrance-mark or rack-line grammar for coverage/candidate/proposal state;
- identical symbols distinguished only by colour.

Transition semantics:

- Open candidate = review glyph only;
- Confirmed candidate = normal authoritative Crop Tree mark becomes truth;
- Rejected candidate = no management mark;
- Proposal = review glyph only;
- Prescription approval = ordinary Pending work/provenance becomes authoritative; proposal overlay is no longer a pseudo-mark.

## Ownership boundaries

### Expected MFM-08 ownership after release

Primary:

- `Assets/ForestPrototype/UI/StandMapView.cs`;
- smallest new MFM-specific presentation/helper file(s);
- only the `Assets/ForestPrototype/UI/Map/` draw/model files strictly required for the treatment-review overlay;
- bounded `TreeInspectionView.cs` / inspection-navigation presentation only if necessary.

Prefer no `ScenarioOneUiRoot.cs` edit. If needed, stop and refresh live ownership before touching it.

### Explicitly out of scope

- `AnnualReviewView.cs`, Management History or prior-treatment guidance (#147 / S1-C);
- generic previous-vs-proposed Work Plan grouping/history navigation (#151 / S1-D T-05);
- MFM-09 lesson/curriculum/learning IDs/`Extend your treatment` sequence;
- prescription-specific Work Plan teaching/grouping;
- save/schema/validator/world-hash/reference compatibility;
- `ScenarioOneCoverage` calibration/tolerances;
- #129 world-adapter implementation;
- growth/ecology/economy simulation rules;
- Windows build.

If a required production edit crosses one of these boundaries, STOP and return to Manager rather than absorbing ownership.

## Verification required

### Offline/source checks

Add focused checks sufficient to prove:

- MFM review uses the final `ReadCoverage` façade rather than an alternate adapter;
- all four coverage states + source sample + reasons render into deterministic presentation data;
- Covered classification is distinct from Include/Manual draft choice;
- candidate Next/Previous remains non-wrapping and no map action confirms a candidate;
- proposal presentation consumes read-only preview and never sets Fell marks;
- spot-check gate cannot be satisfied by map navigation alone;
- prescription approval is blocked before spot-check and uses MFM-07 after it;
- no Work Plan approval is performed by MFM-08;
- no save/schema/hash/reference files changed.

Do not weaken existing MFM-04/05/06/07 tests to get green.

### Unity Tier-2 feature qualification

On the exact handed-off candidate, with machine lock and free-space floor satisfied:

1. import/compile cleanly;
2. render Stand Map treatment review at **1280×720** and **1920×1080**;
3. verify source sample + four coverage states + readable reasons + legend;
4. verify Covered vs Include/Manual is visually/semantically distinct;
5. navigate candidate Previous/Next; inspect in forest; confirm one via authoritative Crop Tree path; reject another; verify no bulk action;
6. verify candidate/proposal review visuals are distinguishable from existing Fell/Crop/Rack marks by form/text, not only colour;
7. prepare an Accepted prescription and verify no work order/Fell mark is created by that step;
8. preview proposals and inspect at least one in 3D; verify no Fell mark appears from preview;
9. prove map waypoint alone does not satisfy spot-check; actual 3D inspection does;
10. approve prescription after spot-check and verify ordinary Pending work is created with provenance while Work Plan remains unapproved;
11. exercise conflict/held proposal presentation for at least manual Fell mark/open work/unit no longer covered where feasible through deterministic fixture/setup;
12. rerun affected Stand Map/Rack V2, MFM-04/05/06/07 and relevant UI regressions;
13. confirm Reference Future v1 compatibility/protected hashes remain unchanged where existing gates cover them;
14. no Windows build.

Record exact resolution screenshots/render captures and interaction evidence in the GitHub handoff; large raw logs/screenshots may use external verification storage but the handoff must contain enough evidence for remote Manager review.

## Stop conditions

STOP without further production edits if:

- #129 is not Manager-approved/integrated;
- final integrated `ReadCoverage` is absent or has semantics wider/different than approved;
- a save/schema change appears necessary;
- a new coverage calibration/tolerance is needed;
- a shared UI file is actively owned by #147/#151/another task;
- faithful spot-check would require persisted state;
- faithful revision requires destructive mutation/deletion of v23 prescription records;
- the UI would need to make proposals into Fell marks;
- MFM-09 teaching/curriculum becomes necessary to make the implementation compile/work;
- Unity lock/free-space safety checks fail;
- any required existing regression must be weakened to pass.

Return the specific blocker to Overall Manager.

## Handoff

Publish:

`Docs/Handoffs/MFM-08-Map-3D-Review-UX/HANDOFF.md`

The handoff must state:

- exact start and candidate SHAs;
- files changed;
- final `ReadCoverage` signature consumed;
- exact UX behaviour and any bounded visual choices made;
- offline and Unity gates actually run/results;
- screenshots/render evidence at both resolutions;
- remaining issues/risks;
- confirmation that save/schema/calibration/Work Plan teaching/history/Windows build were untouched.

Return token:

`MULTI-SCALE REVIEW UX QUALIFIED — READY FOR MANAGER REVIEW`

## Dispatch

**Do not dispatch yet.** After #129 is approved/integrated and this packet is repinned, use:

`Run claim #156 from Docs/Handoffs/MFM-08-Map-3D-Review-UX/TASK-PACKET.md on the exact Manager-approved/integrated #129 lineage; implement and Tier-2 qualify the bounded MFM-08 Stand Map/3D review UX only.`

## User action

None now. #156 is intentionally blocked.