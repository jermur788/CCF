# S1-D T-05 — Previous versus proposed presentation — Task Packet

## Task identity

- **Spec:** `specs/scenario-one-repeated-interventions-v1/`
- **Task:** T-05 — Previous versus proposed presentation
- **Parent:** #91 PLAYER-TEST-OVERNIGHT-01
- **Purpose:** make repeated-management state understandable by grouping current Work Plan orders as Approved / Pending new / Pending carried over and linking to the existing S1-C history/Annual Review surface, without inventing a second history UI or new provenance fields.
- **Difficulty:** **Medium–High** — bounded presentation work, but it crosses the active S1-C Work Plan/history UI boundary and must preserve ownership/API separation.
- **Default implementation model/provider:** **Claude Sonnet 5.5 Medium / Anthropic**.
- **Review:** Manager/browser source review after handoff; use OpenAI 6.1 Sol independent review if the task expands into shared save/history semantics or other high-risk surfaces.
- **Suggested branch:** `feature/s1-d-previous-proposed-v1`
- **Suggested worktree:** `/media/jer/Videos/CCF-S1D-T05`
- **Claim:** **do not create/start the substantive claim yet**. At release, create `[RUNNING] S1-D T-05 — previous versus proposed presentation` with actual model/provider, branch/worktree and starting HEAD. Packet preparation grants no production ownership.
- **User action required:** none.

## Start gate — all required

Do not start production edits until all are true:

1. #104 S1-C T-05 v22 closure has completed, its exact accepted/integrated API is **published and GitHub-refreshable**, and it has released `WorkPlanView` / Annual Review/history ownership.
2. T-02 remains integrated and its cycle-state/history accessor is available on the starting lineage.
3. T-03/T-04 ownership does not overlap this UI surface; if T-04 is still active, single-writer boundaries are explicitly recorded.
4. Live `Docs/Project/coordination/active-tasks.md` grants this task the bounded Work Plan presentation surface.
5. Starting integration HEAD is recorded exactly.

If #104 is still active on the shared UI/history files, **do not start**; mark the eventual claim `[BLOCKED — S1-C UI OWNERSHIP]`.

## Authority / required behaviour

Implement approved FR-C1..C5 / AC-5 and plan §2.4 only.

- Reuse the existing S1-C history/Annual Review surface and its authoritative intervention record.
- Group open Work Plan orders into player-readable categories:
  - **Approved** — already committed/approved ordinary work;
  - **Pending (new)** — current-cycle/newly-created pending work;
  - **Pending (carried over)** — stale/carried pending work according to the existing authoritative stale-order rule.
- Link or navigate to the relevant S1-C Annual Review/history year where that information already exists.
- Use existing recorded spatial history/waypoint surfaces only where locations are actually recorded.
- Keep wording descriptive; do not imply Crop Tree designation time when no such datum exists.

## Expected owned production surface

Reconfirm exact files after #104 closes. Expected bounded surface:

- `Assets/ForestPrototype/UI/WorkPlanView.cs`
- the smallest integration hook needed to navigate to the existing S1-C Annual Review/history surface;
- bounded presentation verification.

`AnnualReviewView.cs`, history helpers or S1-C query code remain S1-C-owned unless the refreshed API proves a minimal integration hook is required. Stop for Manager ownership/API review before editing those files.

## Hard exclusions

- no new persisted field, including Crop Tree designation year;
- no schema/version bump;
- no second history database/query/store or duplicate history UI;
- no dated S1-D past-work map layer;
- no automatic cancellation/approval;
- no causal claims not supported by recorded data;
- no change to completion rules/objectives/economy/ecology/Rack/MFM mechanics;
- no Reference Future v1 change;
- no Windows build.

## Required verification

### Static/offline

- exact classification matrix for Approved / Pending new / Pending carried over across at least three cycles;
- stale-order rule uses the existing authoritative definition, not a new age threshold;
- history link targets the authoritative S1-C year/record;
- no duplicate history query/store introduced;
- wording does not claim designation time or causality that is not recorded;
- T-02 cycle-state and S1-C history/query regressions remain green;
- protected completion-rule hash unchanged.

### Unity / rendered

Under the machine-wide lock after static review:

- three-cycle fixture showing all required classes distinctly;
- interaction/navigation from Work Plan grouping into the existing S1-C review/history surface;
- rendered review at **1280×720** and **1920×1080**;
- import/compile cleanly on the exact candidate;
- prove existing S1-C Annual Review/history remains the sole history surface.

## Stop conditions

Stop and return to Manager if:

- #104's final S1-C design already owns the grouping/card or provides a different authoritative integration seam — integrate through it, do not duplicate it;
- a new persisted provenance field appears necessary;
- an edit to active S1-C history-query internals would create overlapping ownership;
- a dated past-work map layer is required;
- completion or economy mechanics would need to change.

## Handoff

Publish:

`Docs/Handoffs/S1-D-Implementation-05/HANDOFF.md`

Include exact SHA, refreshed S1-C API seam, owned files, grouping decision table, rendered captures, navigation evidence, offline/Unity results, no-duplicate-history proof and remaining T-08 dependency status.

Success token:

`S1-D T-05 PREVIOUS/PROPOSED PRESENTATION QUALIFIED — READY FOR MANAGER INTEGRATION REVIEW`

## Copy/paste dispatch line

`Run S1-D T-05 from Docs/Handoffs/S1-D-Implementation-05/TASK-PACKET.md. Refresh published integration, #91, #104 and live active-tasks first. Do not start until #104 releases WorkPlan/AnnualReview/history ownership and its exact integrated API is published. Use Claude Sonnet 5.5 Medium by default. Reuse the S1-C history surface; no new persistence, no duplicate history UI, no Windows build.`
