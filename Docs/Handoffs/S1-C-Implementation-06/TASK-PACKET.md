# S1-C T-06 — Prior-Treatment Guidance / Understand Why

**Parent:** #91  
**Spec:** `specs/scenario-one-review-history-v1/` T-06  
**Status:** PREPARED / NOT DISPATCHED  
**Purpose:** add bounded historical guidance that helps the player connect earlier treatment to the next inspection/decision without inventing facts or prescribing one forestry answer.

## Difficulty and routing

- **Difficulty:** Medium — mostly history-query/UI/copy integration, with evidence and waypoint correctness constraints.
- **Preferred worker:** Claude Sonnet 5.5 Medium / Anthropic.
- **Review:** OpenAI 6.1 Sol for cross-provider wording/source review if the implementation materially expands guidance logic.

## Start gate / base

Start only after the exact post-MFM-06 integration lineage is published and S1-C T-04 ownership is either released or explicitly partitioned. Refresh #91/#106/active-tasks and record the exact base SHA.

Suggested branch/worktree: `feature/s1-c-t06-prior-treatment-guidance-v1` / `/media/jer/Videos/CCF-S1C-T06`.

Create a child claim only when dispatchable.

## Required scope

Use only persisted S1-C L1 history plus current L2 observation to produce short L4 teaching guidance. Examples may identify:

- a prior thinning/release in the reviewed area;
- retained deadwood choice;
- planting/protection history;
- recorded storm loss;
- recorded work relevant to present inspection.

Where a recorded location exists, reuse the existing waypoint/navigation mechanism so the player can go and look.

Guidance must remain bounded and non-prescriptive: state what happened and what the player may want to inspect/consider, not that a specific intervention is objectively required.

## Exclusions / invariants

- no new simulation facts or coefficients;
- no universal thinning interval/prescription;
- no waypoint to an invented/unrecorded location;
- no new lesson/curriculum IDs;
- no second history UI;
- no MFM prescription logic;
- preserve S1-C provenance labels, MFM v23 state, Legacy40 and Reference Future;
- no Windows build.

## Verification

- source audit maps every guidance sentence/template to recorded fields and/or current observations;
- at least two distinct intervention histories rendered at 1280×720 and 1920×1080;
- any offered waypoint reaches the recorded target/area;
- old saves without the necessary source record suppress the claim or show the approved not-recorded language;
- causal-language audit against S1-C FR-6/FR-7;
- affected review/history/navigation regressions pass.

## Handoff

Publish `Docs/Handoffs/S1-C-Implementation-06/HANDOFF.md` with exact base/end SHA, source-to-copy matrix, rendered/waypoint evidence and residuals.

Success token: `S1-C T-06 PRIOR-TREATMENT GUIDANCE QUALIFIED`

## Copy/paste dispatch line

`Run S1-C T-06 from Docs/Handoffs/S1-C-Implementation-06/TASK-PACKET.md on the exact current published Scenario One integration lineage. Use Claude Sonnet 5.5 Medium by default. Build bounded prior-treatment guidance only from persisted history plus current observation, reuse recorded-location waypoints where available, and do not invent causal facts or prescriptions. Render two intervention cases at both target resolutions and publish the exact candidate/handoff. No Windows build.`

**User action required now:** none; dependency-gated, not dispatched.
