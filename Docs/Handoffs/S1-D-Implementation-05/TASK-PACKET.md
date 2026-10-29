# S1-D T-05 — Previous versus Proposed Presentation — Task Packet

## Task identity

- **Spec:** `specs/scenario-one-repeated-interventions-v1/`
- **Task:** T-05 — Previous versus proposed presentation
- **Parent:** #91 / #106
- **Status:** **READY AFTER #147 — DO NOT START PRODUCTION UNTIL S1-C HISTORY/REVIEW OWNERSHIP IS RELEASED**
- **Purpose:** make repeated-management state understandable by grouping current Work Plan orders as Approved / Pending new / Pending carried over and linking carried/prior work to the existing S1-C Annual Review/history surface, without inventing a second history UI or new provenance fields.
- **Difficulty:** **4/5** — bounded presentation work, but the faithful year-link needs one minimal hook in the S1-C Annual Review surface that is currently owned by #147.
- **Preferred implementation model/provider:** OpenAI Sol/Codex high reasoning or Claude Sonnet 5.5 Medium/Ultra; one primary writer only.

Suggested lane after release:

- branch `feature/s1-d-t05-previous-proposed-v1`
- worktree `/media/jer/Videos/CCF-S1D-T05`

## Reviewed implementation authority

MFM-07 is integrated/qualified at exact production umbrella:

`c8cfae8c4025cec6e960a050edf4bfde2a981de5`

Its prescription approval boundary is binding:

- accepting/creating a prescription does not create Work Plan work;
- proposal preview is read-only;
- **prescription approval** creates ordinary `ScenarioOneWorkOrder` records in `Pending` status with prescription provenance (`prescriptionId`, coverage unit, Crop Tree and reason);
- those orders remain uncommitted until the existing **Work Plan approval** changes ordinary work to `Approved`;
- proposal state is not a Fell mark and must not be presented as committed work.

The current Work Plan already reads authoritative `ScenarioOneWorkOrder` records and uses `ScenarioWorkStatus.Pending` versus `ScenarioWorkStatus.Approved`. Work orders already carry `createdYear`; no new date field is authorised or needed.

The existing S1-C history surface is authoritative for completed-year browsing. `AnnualReviewView` owns its selected historical year privately and currently exposes `ShowLatest()` but no public exact-year navigation entry point. Therefore the smallest faithful AC-5 link requires a minimal read-only/presentation-only exact-year opening hook in `AnnualReviewView.cs` (or an equivalent existing S1-C public hook if #147 adds one before handoff). Do not create a second history view or store.

## Binding presentation classification

For every **open ordinary work order** shown by T-05, classify from authoritative existing records only:

| T-05 class | Authoritative rule | Meaning |
|---|---|---|
| **Approved** | `order.status == ScenarioWorkStatus.Approved` | Work Plan has committed this ordinary work. This is distinct from MFM prescription approval. |
| **Pending new** | `order.status == ScenarioWorkStatus.Pending && order.createdYear == manager.CurrentEcologicalYear` | Draft/uncommitted ordinary work created in the current ecological year. |
| **Pending carried over** | `order.status == ScenarioWorkStatus.Pending && order.createdYear < manager.CurrentEcologicalYear` | Draft/uncommitted ordinary work created in an earlier year and still open. |

Do not invent a separate carry-over timestamp or reuse UI-only stale thresholds. `StaleOrders` may continue to flag orders needing attention, but T-05 carry-over is the direct persisted `createdYear < CurrentEcologicalYear` fact.

If malformed/unexpected state presents `createdYear > CurrentEcologicalYear`, do not silently label it as new; fail closed in presentation/verification and return the inconsistency to Manager rather than inventing semantics.

### Relevant-year link

- For `Pending carried over`, the relevant recorded year is `order.createdYear` **only when that year exists in the existing S1-C `HistoryYears` surface**.
- For `Pending new`, there is no completed Annual Review for the current year yet; do not fabricate one.
- For `Approved`, status alone does not prove a distinct approval year because no separate approval-year field is authorised for T-05. If the order's recorded creation year is a completed S1-C history year, the UI may link to that recorded year as origin context, clearly labelled as creation/origin history rather than approval chronology.
- Spatial navigation may reuse an existing recorded location/waypoint only where the underlying record already contains one. Do not infer past locations from current state.

## Start gate — all required

1. #147 S1-C T-06 has handed off and released `AnnualReviewView.cs` / bounded history-presentation ownership.
2. Manager/browser review of #147 has accepted or otherwise disposed its exact candidate, and T-05 is repinned to one exact lineage containing the required S1-C history API plus integrated MFM-07 semantics.
3. Live `Docs/Project/coordination/active-tasks.md` confirms no conflicting writer on `WorkPlanView.cs`, `AnnualReviewView.cs` or the exact minimal history hook.
4. The T-05 child claim is explicitly changed from BLOCKED/READY-AFTER to RUNNING with actual model/provider, branch, worktree and starting HEAD.
5. MFM-09 has no overlapping Work Plan writer. If MFM-09 begins first, stop for Manager ownership sequencing.

## Owned production surface after release

Primary:

- `Assets/ForestPrototype/UI/WorkPlanView.cs`

Minimal shared presentation hook, only after #147 releases it:

- `Assets/ForestPrototype/UI/AnnualReviewView.cs` — add/reuse only the smallest exact-year opening API needed to display an existing `HistoryYears` year; no history reconstruction, copy redesign or T-06 logic rewrite.

Optional only if current UI routing genuinely requires it and is unowned:

- the smallest `ScenarioOneUiRoot` presentation routing method needed to pass an existing recorded year into `AnnualReviewView`.

Read-only inputs, not T-05 ownership:

- `ScenarioOneManager.WorkOrders`, `ScenarioOneWorkOrder.createdYear/status`;
- `ScenarioOneManager.HistoryYears` and existing S1-C history records;
- MFM prescription provenance already attached to ordinary work orders.

Do not edit MFM prescription/approval production code, S1-C history/query data construction, save DTOs or validation.

## MFM-09 boundary

T-05 owns **generic repeated-intervention readiness presentation only**:

- Approved / Pending new / Pending carried over;
- origin/history-year navigation through the existing S1-C surface;
- generic provenance display only where an existing order already carries it.

MFM-09 remains the owner of:

- `Extend your treatment` teaching insertion/sequence;
- prescription-specific teaching copy;
- grouping/explanation of prescription-generated work as part of the Multi-Scale lesson;
- proposal/candidate/map teaching UX and its Work Plan integration.

T-05 must not absorb MFM-09 curriculum, proposal review, candidate semantics or prescription teaching.

## Hard exclusions

- no new persisted field, including no Crop Tree designation year or approval-year field;
- no schema/version bump, save/validator/hash/reference edit;
- no second history database/query/store or duplicate history UI;
- no dated S1-D past-work map layer;
- no automatic cancellation/approval or change to Work Plan commitment behaviour;
- no causal claims unsupported by recorded data;
- no ecology/economy/completion-rule/Rack changes;
- no MFM proposal/approval semantics changes;
- no MFM-09 teaching implementation;
- no Reference Future v1 change;
- no Windows build.

## Required verification

### Static/offline

- classification matrix proves Approved / Pending new / Pending carried over using only `status`, `createdYear` and current year;
- include ordinary manual work and MFM prescription-generated pending ordinary work; both obey the same Work Plan commitment boundary;
- prescription approval alone remains Pending, never Approved;
- carried-over classification does not depend on the separate stale-attention threshold;
- exact-year navigation opens an existing S1-C `HistoryYears` year and does not mutate or acknowledge history;
- missing history year yields no invented link;
- no duplicate history query/store;
- no wording claims unrecorded approval/designation time or causality;
- focused S1-C T-02/T-04/T-06 and S1-D T-02 regressions remain green;
- protected completion/reference/hash behavior unchanged, no rebaseline.

### Unity/rendered

Under the machine-wide lock:

- three-cycle fixture showing all required classes distinctly;
- include at least one MFM-generated pending ordinary work item with provenance and prove it is still Pending before Work Plan approval;
- Work Plan → relevant historical Annual Review navigation for a carried-over order;
- missing/non-completed year case has no fabricated historical destination;
- 1280×720 and 1920×1080 rendered review;
- import/compile cleanly;
- prove S1-C remains the sole history surface and Work Plan approval remains the sole commitment gate.

## Stop conditions

Stop and return to Manager if:

- #147 changes the history/year navigation API materially, requiring this packet to be repinned;
- another writer owns `WorkPlanView.cs`, `AnnualReviewView.cs` or required UI routing at start;
- implementation needs any new persisted provenance/date field;
- a second history query/store or dated map layer appears necessary;
- MFM-09 is already writing the same Work Plan surface;
- completion/economy/MFM proposal mechanics would need to change.

## Handoff

Publish `Docs/Handoffs/S1-D-Implementation-05/HANDOFF.md` with actual model/provider, exact start/end SHA, owned files, grouping decision table, history-link evidence, MFM provenance witness, rendered captures, offline/Unity results, no-duplicate-history proof and remaining T-08 dependency status.

Success token:

`S1-D T-05 PREVIOUS/PROPOSED PRESENTATION QUALIFIED — READY FOR MANAGER REVIEW`

## Copy/paste dispatch line — only after #147 release and Manager repin

`Run S1-D T-05 from Docs/Handoffs/S1-D-Implementation-05/TASK-PACKET.md only after #147 has handed off/released AnnualReview/history presentation ownership and the Manager has repinned the exact combined lineage. Own WorkPlanView plus only the minimal existing AnnualReview exact-year navigation hook; classify Approved / Pending new / Pending carried over from ordinary order status + createdYear, preserve MFM prescription approval as pending-only until Work Plan approval, and reuse S1-C HistoryYears rather than creating history. No save/schema/MFM-09 teaching/Windows build.`

**User action required now:** none.