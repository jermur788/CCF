# MFM-08 — Map / 3D review UX — Task Packet

## Task identity

- **Spec:** `specs/multiscale-forestry-management-v1/`
- **Task:** MFM-08 — Map / 3D review UX
- **Parent:** #91 PLAYER-TEST-OVERNIGHT-01
- **Purpose:** make source sample, coverage, candidates and proposals understandable on the map and in 3D while keeping suggestions visually/semantically distinct from committed marks and requiring individual player judgement.
- **Difficulty:** **Medium–High** — UI/interaction and rendered qualification across map + first-person/3D surfaces, with strict non-commitment semantics.
- **Default implementation model/provider:** **Claude Sonnet 5.5 Medium / Anthropic** for the integrated UI/logic slice; use the existing Unity-capable T3 lane for runtime/rendered qualification.
- **Escalation:** Claude Opus 5.5 Low only if a recorded architecture/UI-state ambiguity cannot be resolved at Sonnet Medium. OpenAI 6.1 Sol may provide independent source/interaction review where useful.
- **Suggested branch:** `feature/mfm-08-map-3d-review-ux-v1`
- **Suggested worktree:** `/media/jer/Videos/CCF-MFM08`
- **Claim:** create only when start gate is satisfied: `[RUNNING] MFM-08 — map/3D review UX` with actual model/provider, branch/worktree and starting HEAD.
- **User action required:** none.

## Start gate — all required

1. MFM-05 spacing-only candidate state/navigation is published, accepted and Unity-compiled.
2. MFM-06 save/provenance foundation is integrated.
3. MFM-07 proposal/approval boundary is published, accepted and integrated so the UX cannot invent commitment semantics.
4. Any active PARITY-07 / S1-D teaching / Stand Map writer has released overlapping UI files or an explicit API boundary is recorded.
5. Live `active-tasks.md` grants this task the bounded map/3D UX file set and the Unity queue is refreshed before rendered execution.

## Player-facing semantics

### Map must show

- the source sample / representative teaching rack;
- coverage states with reasons: Covered / Not covered / Excluded / Manual as applicable;
- spacing-only Crop Tree candidates;
- prescription proposals and reasons;
- clear separation between proposal/suggestion state and approved/committed work.

Use player-facing wording around **applying the demonstrated sample treatment to comparable areas**. Do not present `operator selection` as formal Irish terminology and do not present the simulator's exact staged workflow as an official/universal forestry procedure.

### 3D must support

- navigate/focus one candidate at a time;
- inspect candidate in the actual 3D forest;
- confirm/reject individually through the accepted MFM-05/MFM-07 interfaces;
- inspect proposal reason/provenance;
- perform the required extrapolated-unit spot check before prescription approval can be considered complete by the later teaching/integration flow.

Candidate spacing identifies **where to inspect**. It does not judge stem quality. No automatic or bulk confirmation.

## Visual rules

Candidate and proposal presentation must be clearly distinct from:

- Red Fell marks;
- Blue Crop Tree marks;
- Orange Rack entrance marks.

Do not use a treatment that can be mistaken for committed ordinary work. Avoid giant/glowing geometry or visual language that implies certainty/commitment.

## Implementation boundary

Use existing Stand Map / Management / 3D interaction patterns and the published MFM-05/06/07 APIs. Do not create a parallel simulation state, second mark system or alternate Work Plan.

Exact files must be refreshed from the current integrated base. Stop before editing if an active PARITY/S1-D/Stand Map writer owns the same file set.

## Hard exclusions

- no prescription derivation/calibration change;
- no hidden quality ranking;
- no automatic/bulk candidate confirmation;
- no `TreeMarkType.Fell` proposal representation;
- no Work Plan auto-import/approval/execution;
- no save/schema change beyond consuming the integrated MFM-06 contract;
- no broadleaf quantitative proposal flow;
- no ecology/economy/growth retune;
- no Reference Future/Legacy40 change;
- no Windows build.

## Required verification

### Static/interaction checks

- candidates/proposals use distinct UI/state from Red/Blue/Orange marks;
- all coverage states and reasons are reachable/readable;
- candidate copy says spacing identifies where to inspect and player judgement is required;
- no hidden quality claim in labels/tooltips/help;
- no candidate becomes Crop Tree without individual player action;
- manual alternative remains available;
- proposal reason/provenance matches MFM-07/MFM-06 state;
- opening UX does not mutate Work Plan/marks/proposals.

### Unity functional qualification

Under the machine-wide lock:

- map from source sample through coverage/candidate/proposal states;
- navigate to at least several individual candidates in 3D;
- confirm/reject individually with current-world validation;
- manual alternative/override case;
- proposal-reason inspection;
- required extrapolated-unit spot-check flow;
- save/reload where MFM-06 persistence applies;
- prove no Work Plan commitment before MFM-07 approval boundary;
- import/compile cleanly.

### Rendered review

Capture and inspect all material states at **1280×720** and **1920×1080**, including:

- source sample/teaching rack;
- coverage categories and reason panel;
- candidate selected on map;
- candidate target in 3D;
- confirmed/rejected candidate feedback;
- proposal on map;
- proposal reason/provenance;
- visual comparison against Red Fell, Blue Crop Tree and Orange Rack marks;
- no overlap/clipping that hides commitment status or reason text.

## Stop conditions

Stop if:

- the UX needs new commitment semantics rather than consuming MFM-07;
- candidate confirmation cannot be grounded in an actual 3D/current-world interaction without inventing automation;
- proposal/candidate visuals cannot be made clearly distinct from existing marks within the bounded UI surface;
- overlapping UI ownership is active;
- a new save field appears necessary.

## Handoff

Publish:

`Docs/Handoffs/MFM-Implementation-08/HANDOFF.md`

Include exact SHA, API dependencies, interaction-state table, screenshots/captures at both resolutions, copy/evidence mapping, individual-confirmation evidence, spot-check evidence, non-mutation/commitment proof and residuals for MFM-09.

Success token:

`MULTI-SCALE REVIEW UX QUALIFIED — READY FOR MANAGER INTEGRATION REVIEW`

## Copy/paste dispatch line

`Run MFM-08 from Docs/Handoffs/MFM-Implementation-08/TASK-PACKET.md. Refresh published integration, #91, MFM-05/06/07 and live active-tasks first. Start only after MFM-07 is integrated and overlapping UI ownership is released. Candidate spacing only identifies where to inspect; every candidate decision is individual and grounded in 3D. Proposals remain visually distinct from Fell/Crop/Rack marks and do not imply commitment. Render at 1280x720 and 1920x1080. No Windows build.`
