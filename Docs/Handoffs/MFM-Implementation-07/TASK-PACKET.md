# MFM-07 — Proposal state and approval boundary — Task Packet

## Task identity

- **Spec:** `specs/multiscale-forestry-management-v1/`
- **Task:** MFM-07 — Proposal state and approval boundary
- **Parent:** #91 PLAYER-TEST-OVERNIGHT-01
- **Purpose:** implement a distinct, non-committing prescription proposal state and the prescription-approval boundary, while preserving Work Plan approval as the only commitment gate.
- **Difficulty:** **High** — commitment semantics, provenance and conflict handling across prescription state and ordinary work orders.
- **Default implementation model/provider:** **Claude Sonnet 5.5 Medium / Anthropic**.
- **Independent review:** **OpenAI 6.1 Sol** recommended/required before integration because this boundary can accidentally bypass ordinary Work Plan commitment semantics.
- **Suggested branch:** `feature/mfm-07-proposal-approval-boundary-v1`
- **Suggested worktree:** `/media/jer/Videos/CCF-MFM07`
- **Claim:** create only when start gate is satisfied: `[RUNNING] MFM-07 — proposal state and approval boundary` with actual model/provider, branch/worktree and starting HEAD.
- **User action required:** none.

## Start gate — all required

1. MFM-04/05 are published and accepted on the current integration lineage.
2. MFM-06 is **published, independently reviewed, Unity-qualified and integrated**, with a stable prescription/provenance schema/API.
3. No active Work Plan/order writer overlaps this task; #104 S1-C and S1-D T-05 ownership must be refreshed before start.
4. Live `active-tasks.md` grants one writer for the proposal/order integration surface.
5. Exact starting integration SHA and save version are recorded.

## Critical invariants

These are mandatory and must have explicit negative tests:

- a prescription proposal is **not** `TreeMarkType.Fell`;
- proposals are not saved or represented as though the player manually marked trees;
- opening the Work Plan cannot import unapproved proposals;
- approving the Work Plan cannot commit unapproved proposals;
- prescription approval is separate from Work Plan approval;
- prescription approval creates **pending ordinary work only**, carrying MFM-06 provenance;
- Work Plan approval remains required afterward before execution/commitment;
- manual player state wins proposal conflicts;
- proposal conflicts are surfaced/reconciled, never silently overwritten;
- existing contractor/landowner/economy execution remains authoritative.

## Required implementation boundary

Implement the minimum core/integration logic needed to represent proposal state and convert an explicitly approved prescription into ordinary pending work with provenance.

Use the published MFM-04 deterministic prescription output, MFM-05 player-confirmed candidate state where applicable, and MFM-06 durable IDs/provenance. Do not reimplement those authorities.

The exact files are determined from the published MFM-06 API. Stop if the clean boundary cannot be maintained.

## Hard exclusions

- no bulk Crop Tree confirmation;
- no hidden quality ranking or auto-selection;
- no proposal stored as Fell/manual mark;
- no Work Plan auto-approval or execution;
- no bypass of normal contractor/landowner/economy path;
- no new save/schema bump unless MFM-06's published contract proves one is unavoidable — in that case STOP for Manager decision rather than silently expanding scope;
- no map/3D UI beyond minimal test hooks;
- no teaching-sequence insertion;
- no broadleaf quantitative treatment;
- no Reference Future/Legacy40 changes;
- no Windows build.

## Required verification

### Core/offline

- deterministic proposal creation from the accepted prescription state;
- every proposal has closed-vocabulary reason/provenance required by the spec;
- proposal does not mutate authoritative manual mark/Crop Tree state;
- opening Work Plan leaves unapproved proposals untouched and unimported;
- Work Plan approval with unapproved proposals present does not import/commit them;
- prescription approval creates pending ordinary work only;
- generated work carries prescription ID/version/source/coverage/reason provenance;
- generated pending work still requires ordinary Work Plan approval;
- manual state conflict wins and is surfaced;
- repeated approval is idempotent/no duplicate work;
- stale/reloaded proposal state follows the published MFM-06 contract;
- existing economy/work-order regressions remain green.

### Unity/runtime

Under the machine-wide lock:

- create proposals in a live Scenario One state and prove marks/orders do not change before prescription approval;
- open/approve Work Plan before prescription approval and prove no leakage;
- approve prescription and inspect newly-created pending ordinary work;
- then approve Work Plan through the normal path and prove ordinary execution path remains authoritative;
- save/reload around both approval boundaries;
- manual-conflict case;
- import/compile cleanly on exact candidate.

### Independent review

OpenAI 6.1 Sol reviews the exact published candidate and negative-test matrix before integration, focusing on bypass/leakage, mutation, duplicate work and provenance integrity.

## Stop conditions

Stop if:

- unapproved proposal state cannot be kept distinct from authoritative marks/orders;
- MFM-06 schema/API is insufficient and a new schema bump appears necessary;
- Work Plan integration would require overlapping edits with an active #104/S1-D writer;
- manual state cannot cleanly win a conflict without a product decision;
- existing execution/economy semantics would need to change.

## Handoff

Publish:

`Docs/Handoffs/MFM-Implementation-07/HANDOFF.md`

Include exact SHA, state-transition table, proposal-vs-mark data model, approval boundary, conflict precedence, Work Plan leakage negative tests, provenance examples, save/load evidence, Unity results and independent-review verdict.

Success token:

`PRESCRIPTION APPROVAL BOUNDARY QUALIFIED — READY FOR MANAGER INTEGRATION REVIEW`

## Copy/paste dispatch line

`Run MFM-07 from Docs/Handoffs/MFM-Implementation-07/TASK-PACKET.md. Refresh published integration, #91, MFM-06 and live active-tasks first. Start only after MFM-06 is published/integrated and shared Work Plan ownership is free. Proposals are never Fell marks; prescription approval creates pending ordinary work only; Work Plan approval remains the commitment gate. Claude Sonnet 5.5 Medium by default; OpenAI 6.1 Sol independent boundary review. No Windows build.`
