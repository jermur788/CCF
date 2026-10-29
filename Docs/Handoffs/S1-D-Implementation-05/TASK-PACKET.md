# S1-D T-05 — Previous versus Proposed Presentation — Task Packet

## Task identity

- **Spec:** `specs/scenario-one-repeated-interventions-v1/`
- **Task:** T-05 — Previous versus proposed presentation
- **Parent:** #91 `PLAYER-TEST-OVERNIGHT-01`
- **Status:** **PREPARED / WAIT FOR MFM-07 API — DO NOT DISPATCH YET**
- **Purpose:** make repeated-management state understandable by grouping current Work Plan orders as Approved / Pending new / Pending carried over and linking to the existing S1-C history/Annual Review surface, without inventing a second history UI or new provenance fields.
- **Difficulty:** **4/5** — bounded presentation work, but it crosses the shared Work Plan seam and must consume both S1-C history and MFM-generated pending-work provenance correctly.
- **Preferred implementation model/provider:** **Claude Sonnet 5.5 Medium or stronger Claude Code lane**.
- **Review:** Manager/browser source review after handoff; use an independent OpenAI Sol/Codex reviewer if the diff expands into save/history semantics or other high-risk shared state.

Suggested lane after release:

- branch `feature/s1-d-t05-previous-proposed-v1`
- worktree `/media/jer/Videos/CCF-S1D-T05`

## Current readiness

The former S1-C T-05/#104 blocker is closed: the qualified production lineage already contains the v22 objective-history S1-C surface and S1-D T-02 cycle-state work.

Exact current production base before MFM-07 is:

`b971f4da0540e606bfee9702b0c99f81a0a89355`

Do not dispatch T-05 from that pre-MFM-07 base. T-05 needs the **accepted MFM-07 pending-work/provenance API** so prescription-generated pending work is grouped using the real final API rather than a guessed seam.

## Start gate — all required

1. MFM-07 #121 returns a GitHub-published handoff and exact candidate.
2. Browser/Manager review verdict is `APPROVED` and the candidate is integrated/published or explicitly approved as this task's base.
3. S1-D T-02 remains present.
4. S1-C history/Annual Review API remains published and no active S1-C writer owns `WorkPlanView` or the exact hook T-05 needs.
5. S1-D T-04 may still be executing only if exact files are disjoint; T-04 must own teaching/current-state, not `WorkPlanView`.
6. Live `active-tasks.md` grants T-05 the bounded Work Plan presentation surface.

Create a substantive claim only when this gate is true. At actual start record model/provider, branch/worktree and exact starting SHA.

## Authority / required behaviour

Implement approved FR-C1..C5 / AC-5 and plan §2.4 only.

Reuse the existing S1-C history/Annual Review surface and the MFM-07 ordinary pending-work/provenance API.

Group Work Plan state into:

- **Approved** — already committed/approved ordinary work;
- **Pending (new)** — current-cycle newly created pending work;
- **Pending (carried over)** — pending work carried from earlier years according to the existing authoritative stale/carried rule.

Prescription-generated work is still ordinary pending work after MFM-07 prescription approval; its provenance may explain its source, but it does not become approved Work Plan work until the player approves the Work Plan.

Link/navigate to the relevant S1-C Annual Review/history year where that record exists. Use recorded spatial history/waypoints only where locations actually exist. Do not imply a Crop Tree designation year when no such datum is recorded.

## Expected owned production surface while RUNNING

Primary:

- `Assets/ForestPrototype/UI/WorkPlanView.cs`
- smallest integration hook required to navigate to the existing S1-C Annual Review/history surface
- bounded presentation verification

`AnnualReviewView.cs`, S1-C history/query helpers and MFM proposal/approval production code remain owned by their respective slices unless a minimal API hook is demonstrably required. Stop for Manager ownership/API review before editing those files.

## Hard exclusions

- no new persisted field, including Crop Tree designation year;
- no schema/version bump;
- no second history database/query/store or duplicate history UI;
- no dated S1-D past-work map layer;
- no automatic cancellation/approval or Work Plan commitment;
- no causal claims not supported by recorded data;
- no change to completion rules/objectives/economy/ecology/Rack or MFM proposal semantics;
- no Reference Future v1 change;
- no Windows build.

## Required verification

### Static/offline

- classification matrix for Approved / Pending new / Pending carried over across at least three cycles;
- ordinary manual pending work and MFM prescription-generated pending work follow the same commitment boundary;
- MFM provenance explains generated work without turning it into approved work;
- stale/carried rule reuses existing authoritative logic, not a new threshold;
- history link targets the authoritative S1-C year/record;
- no duplicate history query/store;
- wording does not claim unrecorded designation time or causality;
- T-02 cycle-state and S1-C history/query regressions stay green;
- protected completion-rule hash remains unchanged.

### Unity/rendered

Under the machine-wide lock:

- three-cycle fixture showing all required classes distinctly;
- include at least one MFM-generated pending ordinary work item if MFM-07 exposes the needed path;
- navigation from Work Plan grouping into existing S1-C review/history;
- 1280×720 and 1920×1080 rendered review;
- import/compile cleanly;
- prove S1-C remains the sole history surface and Work Plan approval remains the sole commitment gate.

## Stop conditions

Stop and return to Manager if:

- MFM-07 returns a materially different pending/provenance contract than this packet assumes;
- the S1-C design already owns the proposed grouping/card — integrate through it, do not duplicate;
- a new persisted provenance field appears necessary;
- an edit to active S1-C history internals or MFM proposal internals would create overlapping ownership;
- a dated past-work map layer is required;
- completion/economy mechanics would need to change.

## Handoff

Publish `Docs/Handoffs/S1-D-Implementation-05/HANDOFF.md` with actual model/provider, exact start/end SHA, refreshed S1-C and MFM-07 API seams, owned files, grouping decision table, rendered captures, navigation evidence, offline/Unity results, no-duplicate-history proof and remaining T-08 dependency status.

Success token:

`S1-D T-05 PREVIOUS/PROPOSED PRESENTATION QUALIFIED — READY FOR MANAGER REVIEW`

## Copy/paste dispatch line — use only after Manager release

`Run S1-D T-05 from Docs/Handoffs/S1-D-Implementation-05/TASK-PACKET.md only after #121 MFM-07 is Manager-APPROVED and its exact pending-work/provenance API is published. Root from that approved candidate, use Claude Sonnet 5.5 Medium or stronger, own only the bounded WorkPlan presentation seam, and reuse S1-C history plus ordinary MFM-generated pending work. No new persistence, no duplicate history UI, no automatic Work Plan approval, no Windows build.`

**User action required now:** none. Browser #122 will create/release the claim after the MFM-07 return review establishes the exact API/base.
