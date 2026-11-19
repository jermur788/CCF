# Multi-Stage5-Player-WorkPlan-UX-Preflight — One-area 2D player flow, source adapter and acceptance map

**Date:** 19 November 2026 (UTC)  
**Work claim:** [Multi-Player-WorkPlan-UX-Preflight / GitHub #845](https://github.com/jermur788/CCF/issues/845)  
**Author:** OpenAI ChatGPT GPT-6; GitHub/browser docs-only worker, not the #841 engine author or an independent implementation qualifier  
**Manager / decision owner:** [Multi 2D #730](https://github.com/jermur788/CCF/issues/730)  
**Classification:** DOCS-ONLY elaboration of an APPROVED Stage5 **design**; later UI implementation is SPEC REQUIRED and separately gated  
**Writable lane:** branch \`Multi-stage5-player-workplan-ux-preflight-20261119-v1\` from observed \`main\` \`7758e17c882fad01970d2f2fb8c09586bbcab5c1\`; this *one new handoff file only*  
**Result status:** **PREFLIGHT FOR MANAGER REVIEW — NO UI SOURCE, NATIVE TEST, RENDER OR RUNTIME CLAIM**

## 1. Binding decision, observed source and status distinctions

This preflight specifies the **smallest genuine player demonstration**: inspect one committed source frame for the original **30×30 m, 0.09 ha, 100-Sitka synthetic area**; deliberately select **individual living source-tree IDs** with reasons; add ordinary pending Fell orders; explicitly approve a complete Work Plan **without felling**; separately Execute **one atomic authoritative Fell batch**; then advance **one real ecological year** from the actually surviving trees and compare real source observations. A second management cycle and no-work are inspectable when supported. The management area is **one undivided whole stand**, not a physical extraction rack or an automatic all-100-tree operation.

**Authority.** The [APPROVED Stage5 spec](https://github.com/jermur788/CCF/blob/docs/multi-730-837-g2-contract-decision-draft-20261119-v1/specs/multirack-sitka-2d-intervention-workflow-v1/spec.md), [plan](https://github.com/jermur788/CCF/blob/docs/multi-730-837-g2-contract-decision-draft-20261119-v1/specs/multirack-sitka-2d-intervention-workflow-v1/plan.md), [tasks](https://github.com/jermur788/CCF/blob/docs/multi-730-837-g2-contract-decision-draft-20261119-v1/specs/multirack-sitka-2d-intervention-workflow-v1/tasks.md) and [26R+24X **unchecked** checklist](https://github.com/jermur788/CCF/blob/docs/multi-730-837-g2-contract-decision-draft-20261119-v1/specs/multirack-sitka-2d-intervention-workflow-v1/checklists/requirements.md) control the intended workflow; this document does **not** amend them or promote the proposed §8 ABI to formally signed source authority. [#805 source-admission/candidate spec](https://github.com/jermur788/CCF/blob/docs/multi-805-committed-frame-admission-matrix-20261119-v1/specs/multirack-sitka-2d-read-contract-and-candidate-ux-v1/spec.md) is **DRAFT**, useful for read-boundary negatives, not a completed player bridge.

**Source read now possible.** Directly fetched the GitHub [#841 exact source review branch](https://github.com/jermur788/CCF/tree/review/multi-841-g2-qualified-engine-8741a328-20261119-v1), whose observed HEAD is **\`8741a32843b739b0587e4c6f3a3ee7fc890ea7f6\`**, matching the [native author + independent Claude Opus 5.5 handoff](https://github.com/jermur788/CCF/blob/docs/multi-841-internal-g2-one-parent-result-20261119-v1/Docs/Handoffs/MultiRack-Simulation-20261116/841-orchestration/Multi-841-INTERNAL-G2-ACTUAL-FELL-WORKPLAN-YEAR1PLUS-IMPLEMENTATION-AND-INDEPENDENT-Q-RESULT-20261119.md). Direct source read:
- [WorkContract.cs](https://github.com/jermur788/CCF/blob/8741a32843b739b0587e4c6f3a3ee7fc890ea7f6/Tools/MultiRackSitkaP1Lab/src/Core/WorkContract.cs) blob \`79cf27e56d5a289cc6b1598761d88562f8dea9e1\`;
- [AnnualContinuation.cs](https://github.com/jermur788/CCF/blob/8741a32843b739b0587e4c6f3a3ee7fc890ea7f6/Tools/MultiRackSitkaP1Lab/src/Core/AnnualContinuation.cs) blob \`63c41542e875581c7ae75b41746b05e3960d7b54\`;
- [AnnualEngine.cs](https://github.com/jermur788/CCF/blob/8741a32843b739b0587e4c6f3a3ee7fc890ea7f6/Tools/MultiRackSitkaP1Lab/src/Core/AnnualEngine.cs) blob \`625a3b44a9b1e8c9276ff5b0ee8c7b57c10f8738\`.

**Important source boundary:** \`AnnualEngine.Open(P1Domain, fixtureId, fixtureDirectory)\` is public, but \`AnnualEngine.ContinueInMemory()\` and **\`AnnualContinuation\` are internal**. So \`Frame\`, \`CurrentToken\`, \`AddPendingFell\`, \`RemovePendingFell\`, \`ApprovePlan\`, \`CancelApprovedPlan\`, \`Execute\` and the two \`Advance\` overloads below are **actual inspected internal methods**, *not* an authorised public Viewer, HTTP, Node, Host or browser API. There is no demonstrated 2D player bridge. Do not call the legacy \`AnnualEngine.Advance()\` as a substitute for the continued Work Plan flow; it is a one-year experiment and not the G2 player transaction.

**Evidence tier, not a new test:** #841 reports internal source implementation, clean 62/62 checks, independent Tier-2 PASS and 17 extra nonauthor probes / 2,113 checks / 0 failures. [#730 Manager verdict](https://github.com/jermur788/CCF/blob/docs/multi-730-841-manager-qualified-engine-verdict-20261119-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-730-841-G2-IMPLEMENTATION-QUALIFICATION-MANAGER-VERDICT-20261119.md) accepts that **as scoped evidence** but keeps unconditional release **NEEDS CHANGES**. This chat has **not** rebuilt, run, rendered or independently requalified the source.

**V-01 update, narrowly resolved:** [current Scenario One source owner #844 explicitly APPROVED IN PRINCIPLE](https://github.com/jermur788/CCF/issues/844#issuecomment-7024491671) prospective read/compile-only use of unchanged \`Assets/ForestPrototype/SitkaGrowthModel.cs\` blob \`e4c73402d46bb3b75a9cba2e4622857a913fac92\` and \`Assets/ForestPrototype/SimulationRandom.cs\` blob \`bb83d7f8468002ab13a6aec6caf548e0bf3ad34b\`. #730 [recorded that source-owner half as resolved](https://github.com/jermur788/CCF/issues/730#issuecomment-7024511368). Product-owner half remains worker-reported, not directly reproduced here; permission is **prospective only**, not a retroactive #841 waiver, source edit, lease or integration grant. **A-1 remains blocking for any real new UI caller.**

**G1:** DISABLED, Manual/NotCovered. #842/#843 supplied no sufficient positive comparable-area evidence; #828 is DRAFT. Do not infer comparability from species, age, size, area membership, a local sample or map proximity. The owner's broader **real multi-area** objective remains a separate near-term research/spec task, **not** Stage5 V1 implementation.

## 2. Source provenance and safe adapter contract

### 2.1 What an inspected committed session can actually provide

| Data / action | Inspected type and member | Player-facing interpretation and limitation |
|---|---|---|
| Current admitted root | internal \`AnnualContinuation.Frame : CommittedFrame\` | An immutable projection of the **one committed** session root, not editable raw engine state or a detached preview |
| Binding/identity | \`CommittedFrame.ManagementScopeId, Domain, FixtureId, FixtureFileSha256, FixtureContentHash, EcologicalYear, AnnualRevision, WorkRevision\` | Show exact year/scope and abbreviated but expandable source signature; compare full source token internally for commands |
| Real tree rows | \`CommittedFrame.Trees : IReadOnlyList<AnnualTree>\`; \`AnnualTree.TreeId, XM, ZM, AgeYears, DbhCm, HeightM, CrownRadiusM, Death, IsLiving\` | Exact case-sensitive ID and source-backed geometry/size/liveness; dimensions are typed numbers, not licence to infer timber grade, site, wind or suitability |
| Source aggregates | \`CommittedFrame.Stand : AnnualStand\`; \`LivingCount, StemsPerHa, BasalAreaM2PerHa, QuadraticMeanDbhCm, RelativeDensity\` | Display **actual current engine** count and m²/ha BA; don't calculate a competing result from canvas pixels |
| Executed history | \`CommittedFrame.Events : IReadOnlyList<WorkEvent>\` with typed \`Fells\`, \`Before\`, \`After\`, \`EcologicalYear\`, \`WorkRevision\` | Same-session, committed work history only; each actual Fell shows \`fell/manual-work-plan\` versus naturally caused \`AnnualDeath\` |
| Separate binding token | \`AnnualContinuation.CurrentToken : WorkSourceToken\` | Carries fixture SHA/content hash, model lineage, scope, ecological year, annual revision **and work revision**; the UI cannot silently drop one revision clock |
| Pending mutation | \`AddPendingFell(string treeId, string reason)\`, \`RemovePendingFell(string treeId)\` | Exact live ID, per-ID reason, no actual Fell. The current \`Frame\` **does not expose pending order list** |
| Approval/cancellation | \`ApprovePlan(string planId) : WorkPlan\`; \`CancelApprovedPlan()\` | Approve produces immutable whole batch, zero Fell; cancelling the approved batch removes it, does not quietly convert it back into editable PendingWork. UI must force fresh review/re-add |
| Authoritative actuation | \`Execute(WorkPlan plan, string executionKey, Action? beforePublish = null) : WorkExecutionResult\` | Only an actually approved bound plan; **production adapter must never expose/pass the test-only fault callback**. Success gives typed \`Event\` and \`Replayed\` |
| Genuine next year | **safe** \`Advance(int expectedYear, int expectedRevision, int expectedWorkRevision, bool consentToCancelWork, Action? beforeCommit = null)\` | Bind **both revisions** and make consent an explicit separate player choice. **Never expose/pass the test-only callback** |
| Stale old caller | \`Advance(int expectedYear, int expectedRevision, Action? beforeCommit = null)\` | **A-1 BLOCKER:** after work has changed, this overload silently takes the current work revision rather than fencing the caller's previously observed work revision. **No UI/Host/secondary caller wiring until repaired and independently delta-qualified** |

The **fixture's 170 mm \`ActionEligibleDbhThresholdMm\` is informational only** under D-093; no UI disablement, default selection suppression or claimed forestry law based on it. Fell that would leave **zero** living stems must refuse the **whole** batch.

### 2.2 Data boundary that a future player adapter must implement (PROPOSAL, not API already present)

\`\`\`text
trusted exact fixture / committed session WorkRoot
      │  source-validated read only (one root, year + 2 revisions)
      ▼
thin player-facing session adapter [NOT YET IMPLEMENTED]
      ├─ atomic read: frame + full token + pending/approved status + prior events
      ├─ explicit commands: AddPending / RemovePending / Approve / Cancel
      ├─ Execute approved plan by one stable in-process execution identity
      └─ Advance with both revisions + explicit work-cancel consent
      │
      ▼
2D accessible UI: inspect → deliberate per-ID selection/reason
    → PendingWork → batch review → distinct approval → distinct Execute
    → actual same-year receipt → actual Year+1 report/history
\`\`\`

Adapter rules for later review:
1. Use the **same** \`AnnualContinuation\` session and engine authority throughout a live interaction. A UI should not maintain a duplicate set of "felled" trees, counters, a separate ecology engine, its own RNG, independent source clock or hypothetical executed year. Tree selection in the screen is a **local form draft only**, not yet PendingWork.
2. Provide one *atomic read projection* that includes \`Frame\`, \`CurrentToken\`, real pending orders and approved-unexecuted state from the **same lock/root**. The inspected \`Frame\` lacks pending/approved states; do not reconstruct them from browser storage or infer them from \`Events\`. Exact interface fields, accessibility serialization and process ownership are **UNKNOWN until this adapter is designed, approved and implemented**.
3. A UI-supplied expected token must be checked against the exact current session before **Add to Work Plan** and **approval**, not only at Execute; current \`AddPendingFell\` and \`ApprovePlan\` do **not** themselves accept the displayed expected source token as parameters. Preserve this uncovered UI race as an adapter requirement; do not call the current internal functions blindly on an asynchronously stale view.
4. No browser, CLI or external UI may directly mutate \`AnnualState\` or copy an \`AnnualTree\` with \`Death\` set as a substitute for \`Execute\`. Bridge security/channel and multi-reader policy are **out of scope / no Host**; an in-process explicit adapter is preferable for a future minimal demo *after* A-1 and real source authorisation.
5. Derive every reported "actual" value from a new committed frame, \`WorkExecutionResult.Event\`, or the actual \`AnnualReport\`; keep local drafts, diagnostics and G1-disabled possible treatments **visually and semantically separate**. Preserve \`#805\` source admission: only committed Genesis/AfterMortality may be current; Before/PostGrowth diagnostics are subordinate read-only and never legal work bases.
6. **No persistence promises.** Session-local events, execution key map and plans disappear with process/session loss; do not use localStorage as transaction authority or imply same-key replay survives a restart.
7. Distinguish source-validation error from a UI-only stale callback. No default synthetic fallback forest or guessed partial 99-row frame; a missing authentic source blocks committing work but can display a clearly unavailable/diagnostic explanation.

### 2.3 Minimal UI state vocabulary

| State (UI-owned unless source proves otherwise) | Source effect | Allowed transition / safe guard |
|---|---|---|
| SOURCE_REFUSED / SOURCE_UNAVAILABLE | none | Show reason; no inspect-as-current, Add, Approve, Execute or Advance |
| CURRENT_INSPECT | read only | Open exact source tree detail; no IDs selected by management-area scope |
| DRAFT_SELECTION / DRAFT_SAMPLE | none | Exact per-ID source-backed reason, retain/competitor intent; an optional unqualified sample is **not a transfer policy** |
| PENDING_WORK | ordinary source-side pending Fell IDs, **zero Fell** | Manual reviewed **Add to Work Plan**; remove/review before approval. G1 recommendations unavailable |
| WORK_PLAN_APPROVED | immutable approved whole batch, **zero Fell** | Distinct approval with explicit confirmation and current token; editing needs **Cancel approved batch → re-add/re-review → reapprove** |
| EXECUTING | no intermediate committed state exposed | Lock destructive controls; use stable execution key, handle refusal without optimistic tree removal |
| EXECUTED_YEAR_Y | actual Felled trees + typed event + work revision | Show committed same-year before/after count/BA, per-ID Fell cause and recorded reason; no year increment from Fell |
| YEAR_Y_PLUS_1_COMMITTED | actual new biology, incremented ecological year/revision | Show actual survivors' new growth, competition-derived annual report, natural deaths; no new Fell by year advance |
| CANCELLED / STALE | no Fell from unexecuted orders | Warn+consent; no-consent abort leaves year/orders unchanged; consent can cancel at zero Fell and advance real year |

**No physical rack state**; no automatic area-wide Fell; no G1 matched targets. Distinguish local *DRAFT* form deletion from engine-side \`RemovePendingFell\`, and both from actual irreversible \`Execute\`.

## 3. Screens and complete keyboard/touch-friendly flow

**S0 — Committed source / status, or fail closed**

\`\`\`text
MULTI 2D · Original Sitka 30×30 m · ONE MANAGEMENT AREA
Committed Year: y          Annual revision: r      Work revision: w
Fixture: [exact catalog ID]  Source: [expand checksum/lineage]
Live: [actual] / 100       BA: [actual] m²/ha
[Explore all tree IDs]     [Review previous actual year]
Comparison extension: DISABLED — Manual / NotCovered
Data provenance: Committed (not preview) | Diagnostics: Read only
\`\`\`

With missing/untrusted/uncommitted source: "Current work source unavailable" + actual category and inspect-only explanation; no pretend previous year, tree action, or automatic source substitution. A 2D map is an optional coordinate-backed companion to a **searchable semantic table**; the table is authoritative for accessibility and ID precision. Selecting whole stand never bulk-selects rows.

**S1 — Exact per-tree inspection and player reason**

\`\`\`text
Tree: [EXACT SOURCE ID]       Living / Natural dead / Player felled
Location: X,Z (source)  DBH: cm  Height: m  Crown radius: m
Available competition/neighbourhood facts: value + source, or UNAVAILABLE
Treatment intent: [Retain / inspect only] [Nominate Fell]
Reason for each nominated Fell: [required text]    [Add to Work Plan]
[Back to list] [Clear local draft]
\`\`\`

Retain/crop/release/competitor *reasoning* may be annotated locally as teaching, but **Fell is the only currently inspected executable \`WorkAction\`**. Do **not** claim a CropTree marking/persistence command, pruning, planting, or grade-based quality score exists. No default Fell selection; require deliberate exact live-ID choice. A local sample may collect evidence-backed IDs with separate rationale; its status is **Unqualified for automatic extension** until G1 proof, without blocking manual orders. Show this source-vs-intent difference in text, not merely colour.

**S2 — Pending Work and source-bound batch preview**

\`\`\`text
Pending Fell (no trees changed): n
[Exact tree ID] | action Fell | recorded reason | live source status
Source year y / r / w and full token digest [details]
[Remove this pending order] [Review whole Work Plan]
Comparator suggestions: None — G1 DISABLED / Manual/NotCovered
Costs / timber receipts / budgets: UNAVAILABLE (not €0)
\`\`\`

Manual \`AddPendingFell\` enters ordinary pending work directly; no fictional prescription approval is required. A future **G1-enabled** proposal, *if separately approved later*, would first require distinct prescription review/approval before materialising pending, invalidate that approval when edited and reconcile exact duplicate/conflicting IDs. In current Stage5 one-area preflight, **do not render actionable prescription approval or auto-propagation**.

**S3 — Review and explicitly approve whole Work Plan**

\`\`\`text
APPROVE WORK PLAN — this is NOT execution
Source y / r / w  | scope MS-P30-UNDIVIDED-30M-V1
Exact Fell IDs + all reasons, n orders, predicted effect: NOT ACTUAL
[Back to edit]      [Approve this whole batch]
On success: APPROVED — 0 Fell performed. [Proceed to Execute review]
\`\`\`

Approval must be a separate intentional event and must not be inferred from view-open, marking, proposal acceptance or time skip. Re-read source/token and live rows. Do not show an unqualified authoritative financial forecast; a preview can only be explicitly labelled hypothetical.

**S4 — Separately confirm atomic Execute**

\`\`\`text
EXECUTE APPROVED PLAN — irreversible biological action
This will Fell precisely n live named IDs in Year y, or refuse ALL.
[Cancel without felling]      [Execute approved batch]
\`\`\`

Disable while in flight, use one stable same-session execution key for safe retry, do not optimistically alter the map. On success, *then* render committed event and source frame. On refusal preserve the existing tree list and show the exact reason and recoverable next action. Before trying the action, check it cannot remove the final live stem (D-093), while retaining the **engine's** final authoritative atomic check.

**S5 — Same-year executed receipt, not annual growth**

\`\`\`text
YEAR y · WORK EXECUTED (ecological year unchanged)
Execution ID / Plan ID | Work revision w → w+1 | replay: yes/no
Fell IDs (each cause "fell", mechanism "manual-work-plan")
Before live + BA → after live + BA, from committed WorkEvent
[Inspect survivors] [Advance a real ecological year]
\`\`\`

Display one typed batch event; if identical-key replay, say "Already executed — no additional Fell", not a second event/sale. Avoid invented cash or harvest quality. Natural-death events in later annual reports must not be mislabelled as player Fell.

**S6 — Year advance consent, negative and no-work paths**

- If ordinary no-work state with no pending/approved unexecuted work: "Advance to Year y+1" is a **distinct** action; run genuine annual biology with the full token and work revision.
- If pending/approved-unexecuted work exists: a separate warning announces that **consenting cancels all such unexecuted orders at zero Fell** and then performs one real year advance. Two explicit choices: "Keep orders — stay in Year y" (no mutation) or "Cancel unexecuted work and advance one year" (consent). After consent, a fresh source/year/reason/reapproval is required; never silently carry the old batch.
- If A-1 still open: this entire connected action is **NOT ELIGIBLE TO IMPLEMENT**, even if the newer four-argument engine overload appears safe. Use an approved, source-verified adapter after the mandatory A-1 patch and independent qualification.

**S7 — After genuine Year+1, read-only actual history**

\`\`\`text
YEAR y+1 · ECOLOGY COMMITTED (real biological step)
Source before: y (post-Fell survivors)  → current: y+1
Live / BA / Dq / relative density: source actual, units shown
Annual report: natural deaths [actual cause], survivor updates/CI where available
Work event: year y Fell IDs separate from year y+1 natural mortality
[Compare actual years] [Inspect survivors] [Start a fresh Work Plan]
\`\`\`

Make "Previous actual", "Current actual" and "Proposed hypothetical" visibly separate. Do not call an old preview "Year+1"; source Year+1 must be genuinely committed before it appears as current. Any unsupported per-tree competition field is **UNAVAILABLE** rather than guessed; a simple source-confirmed survivor count/BA delta is sufficient for the minimal authentic demonstration, with a source-authenticated annual report for causality.

## 4. Refusal, warnings and deliberately negative states

All user-visible refusal labels are **UX proposals mapped to the inspected engine's codes**, not claims that an adapter or screen already handles them. Every rejected action should announce **what was refused, why, which year/source remains current, whether anything changed, and one lawful recovery action**.

| Condition | Inspected source status / code where applicable | Required UX behaviour, no hidden partial changes |
|---|---|---|
| Missing, rejected, tampered, uncommitted/mixed source; 99/101 IDs | Read-adapter categories in #805 **DRAFT**, exact runtime adapter UNKNOWN | Source unavailable/refused; no current editable frame, PendingWork, Approve or Execute. Show provenance audit detail; no fallback forest |
| Old callback/current token changed; annual vs work revision | \`WorkSourceToken\`; \`WorkPlanStaleSource\`, \`ContinuationYearMismatch\`, \`ContinuationRevisionMismatch\`, \`ContinuationWorkRevisionMismatch\` | Discard stale draft/refresh; **two clocks** remain distinct. Future Add/Approve adapter must check displayed token too |
| Tree ID absent, wrong case, empty or already felled | \`UnknownTreeId\` on \`AddPendingFell\`; \`WorkTreeNotLive\` at Execute | Fail closed; show "Exact live ID required". **A-4:** current Add cannot distinguish prior Fell from typo; a verified read comparison may clarify if the ID is exact-known-and-dead; never infer from case-insensitive match |
| Required reason missing | \`WorkReasonRequired\` | Keep focus in reason field; no PendingWork |
| Duplicate pending/approved ID | \`WorkDuplicateTreeId\` | Identify duplicate by exact ID, no second order or partial batch |
| Already-approved plan edited | \`WorkPlanAlreadyApproved\` | Ask consent to cancel immutable approval; then re-add/review and reapprove, never mutate approved batch in place |
| Empty plan or missing approval/plan identity | \`WorkEmptyPlan\`, \`WorkPlanNotApproved\`, \`WorkPlanIdRequired\` | No execution. Approve and Execute remain separate controls |
| Wrong/missing/reused execution identity | \`WorkExecutionKeyRequired\`, \`WorkExecutionKeyConflict\` | Show stable receipt for genuine same-payload retry; conflicting reuse refused. **A-2:** same-key record equivalence currently ignores PlanId, so don't claim robust cross-plan identity until hardened |
| All living trees selected for Fell | \`WorkWouldLeaveNoLivingStems\` | Show "This would leave no living trees; none were felled". Do not shrink the batch silently or relabel as legal clearfell |
| Batch fault before commit or result append inconsistency | G2 one-root atomic path, test-only \`beforePublish\` fault seam | No optimistic UI mutation, re-read committed source; if source cannot be re-read, show **outcome unconfirmed**, not success. Fault hook **A-3 must never be exposed to UI** |
| Re-execute a truly executed plan with another key | \`WorkPlanNotApproved\` or \`WorkTreeNotLive\` as appropriate | No repeat Fell/event. Same-key genuine \`Replayed=true\` is read-only receipt, not new work |
| Pending/approved-unexecuted work and year advance | \`WorkPendingConsentRequired\` | Refusal without consent leaves year/work unchanged; with consent cancels at zero Fell then genuinely advances |
| G1 sample/target not sourced or comparable | G1 **DISABLED**; no comparison API | Manual/NotCovered, no recommendation, match/coverage %, or false tree quality. Manual selections continue |
| 170 mm metadata boundary | D-093, no Fell eligibility rule | Show as fixture metadata if useful; **do not disable** live trees on either side |
| Missing economic source | G4 deferred | Say **UNAVAILABLE**, never €0/free, fabricated timber prices or affordability gates |
| Restart / session loss | in-memory only | Do not promise prior approved orders, executed-key recovery or history durability; disclose session boundary |
| Old \`Advance(year, revision)\` caller | **A-1 outstanding** | BLOCK SOURCE-CONNECTED UI implementation until repair + independent delta qualification; cannot be softened into a mere warning |

## 5. Accessibility and responsive UI acceptance preflight

**Must be independently tested in a real browser later; nothing in this section is a PASS claim.**

- **Keyboard:** every tree row/map equivalent, per-ID reason, pending removal, work review, approval, separate Execute, warning consent, year advance and history is reachable in logical Tab order; Enter/Space activation and Escape where applicable; no mouse-only tree markers or hover-only evidence. A tree map must have a full labelled table/list equivalent for 100 IDs.
- **Screen reader:** each tree row exposes exact ID, live/dead and selection state, measure + unit and reason; headings delineate draft/pending/approved/executed/annual results. Use a semantic alert/error summary for refusal and nonintrusive live status for committed transitions, with action-specific text and focus returned to the failed field or alert. No colour-only red Fell/blue Crop Tree authority.
- **Touch:** large, distinguishable targets, explicit destructive confirmation, no hover dependency, no gestures that accidentally approve/execute or bulk-select, and clear per-ID reasons on small screens.
- **Narrow/zoom:** at **360 CSS px viewport** and **200% browser zoom**, stack frame provenance, searchable tree list, details and work summary without hidden mandatory actions, clipped IDs, inaccessible modals or horizontal scrolling of core actions; long SHA strings wrap or use copy/details.
- **Focus and async:** source refresh must not silently change selected identity; loading state must not signal Fell success; source change interrupts unsafe action and announces staleness. Approval and Execute are two separate focusable operations, never a double-click shortcut.
- **Truthfulness:** every numeric label declares m²/ha, stems/ha, cm, m, current year and source status; unknown/NotEvaluated/UNAVAILABLE are distinct from true zero. A diagnostic PostGrowth overlay never has a Fell button.
- **Human QA:** use keyboard-only traversal, NVDA or VoiceOver screen reader, touch interaction (device or emulation) and 360 px/200% on the **real connected candidate**, with evidence of actual focus/announcement and a refusal; author-only markup review is insufficient.

## 6. Complete Stage5 requirement trace — **26 R + 24 X, none accepted as run**

This is a **documentation trace**, not checklist mutation. The authoritative [requirements checklist](https://github.com/jermur788/CCF/blob/docs/multi-730-837-g2-contract-decision-draft-20261119-v1/specs/multirack-sitka-2d-intervention-workflow-v1/checklists/requirements.md) has **all 50 unchecked**. "Design mapped" means only this preflight covers it. Engine behaviour separately reported by #841 cannot count as completed integrated UX/engine X tests.

### 6.1 R-01…R-26: mapping and remaining evidence

| R | Proposed UI/adapter checkpoint | Status / later evidence needed |
|---|---|---|
| R-01 | S0 one whole undivided area, no rack/Confirm/corridor; separate Multi follow-on | Design mapped; S0 negative render needed |
| R-02 | S0 explicit 100-tree Multi identity, not Compact30/Enlarged80/Legacy/Reference | Design mapped; source/label assertion needed |
| R-03 | S0 committed frame/token, exact 100 IDs, fixture/hash/lineage/both revisions | Source fields inspected; real adapter admission X-01/02 needed |
| R-04 | S0 diagnostic Stage tab read-only; no Before/PostGrowth current | #805 DRAFT boundary; source bridge/rejection proof needed |
| R-05 | S1 individual live-ID reason, no select-all; local sample manual only | UI X-04/05 needed |
| R-06 | S1 sample ID/neighbour evidence or Unqualified | Real source-neighbour data provenance + X-05 |
| R-07 | G1 DISABLED / Manual/NotCovered | Conditional N/A only by #730 with positive X-05 proof; not PASS |
| R-08 | No target suggestions or claimed match/exclusion in disabled G1 | Conditional N/A only by #730 with X-05 proof; not PASS |
| R-09 | S1 per-ID choice/override; no hidden quality | X-04/05 plus G1 disabled tests |
| R-10 | #805 REMOVE_CANDIDATE never source Fell/approval | X-06 negative and boundary inspection |
| R-11 | S2 manual Add→pending; G1 prescription unavailable; source-side pending read adapter needed | X-08, optional X-09 N/A disposition |
| R-12 | S3 distinct approval, S4 distinct Execute | X-11/12 actual UI+engine proof |
| R-13 | Full token/live+batch checks at Add, proposal materialisation if enabled, Approve, Execute | **Adapter gap** at Add/Approve: expected displayed token not yet accepted; X-02/08/12 |
| R-14 | S5 actual one-root source Fell/event/count/BA | Inspected G2 core; X-12/13 integrated witness |
| R-15 | S5 exact-key same-process replay, conflicting key refusal | #841 scoped evidence; X-14 adapter + A-2 limitation |
| R-16 | S4 atomic all-or-nothing fault including event | #841 scoped evidence; X-13 on exact integrated candidate |
| R-17 | S2/3 edit warnings; S6 explicit stale-cancel consent | X-15 UI/engine positive+negative |
| R-18 | S6 no-work; S7 real post-Fell survivor next year | X-16/17; **A-1 blocker** |
| R-19 | S5/S7 session history separated from draft and natural death | X-18 and session reset warning |
| R-20 | S4 no zero-live Fell; S7 natural vs Fell, no fabricated means | D-093 fixed; X-19 needed |
| R-21 | S2/S3/S5 explicit UNAVAILABLE economics | X-20; no finance implementation |
| R-22 | S0–S7 keyboard, SR, touch, zoom/360px and errors | X-21 independent browser accessibility |
| R-23 | Session-local only; no save, cross-process identity or Host | X-23 and source-scope audit |
| R-24 | Original fixture/no-work oracle, Assets, #805, main protected | X-22 source compare; no prohibited writes |
| R-25 | Approved design distinct from source A-1/V-01/V-02/L4/main gates and near-term multi-area G1 | X-24 actual actor/ownership/qualified candidate |
| R-26 | S0/S1/S4/S6 typed refusal with zero source delta | All negative X cases, announced exact reason |

### 6.2 X-01…X-24: future positive **and** negative qualification cases

| X | Minimum positive witness | Negative case that must refuse / fail |
|---|---|---|
| X-01 | Authenticated original 100 exact case-sensitive IDs, original fixture/digests | 99/101/mixed/duplicate/wrong-case/tamper → entire source refused, not repaired |
| X-02 | Admitted committed Genesis or AfterMortality, correct types/revision | Before/PostGrowth, aborted/uncommitted, stale or precision-altered current → no work |
| X-03 | One-area S0 with per-ID action, no automatic selection | Any physical rack/corridor/Confirm/clearance or Fell-all shortcut |
| X-04 | Multiple live source IDs with individual reasons, deliberate addition | Dead/unknown/case-shifted/duplicate/unselected/implicit whole stand → no order |
| X-05 | Actual local source-evidenced sample or explicitly unqualified sample and functioning manual path | Same species/age/area proximity treated as approved comparable target |
| X-06 | #805 local candidate/undo has zero source change | Candidate or stale preview becomes Fell, charge, PendingWork without deliberate revalidation |
| X-07 | **Conditional** if G1 proven, matched ID + evidence; otherwise explicitly G1-disabled UX | Different/unknown conditions auto-suggested. If G1 disabled: later #730 N/A, **not PASS** |
| X-08 | Deliberate manual Add creates exact pending ID/reason, zero Fell | Unreviewed/stale/duplicate/conflicting add mutates pending/biology |
| X-09 | **Conditional** G1-proposal reviewed + separately approved prescription becomes pending only | Unapproved/edited proposal produces order/Fell; if disabled: #730 N/A, not PASS |
| X-10 | Consolidated distinct-ID ordinary PendingWork list | Manual/proposed collision, hidden second rack route or all-tree default |
| X-11 | Whole-plan approval creates immutable plan, **0 Fell** | Open/mark/tick/advance/approve prescription triggers Fell |
| X-12 | Separate Execute commits exactly named live IDs + same-year typed result | No approval/wrong year/unknown/dead/duplicate ID causes partial Fell |
| X-13 | One authoritative root/event/count/BA transaction | Mid-batch or event fault leaves tree/event/revision partly mutated |
| X-14 | Identical key/payload replay \`Replayed=true\`, zero new event | Changed payload/key targeting dead ID succeeds or repeats charge |
| X-15 | Explicit edit cancellation/reapproval, consented stale-cancel + real advance | No-consent advances, silent carry-over, cancelled approval executes or Fell resurrects |
| X-16 | After Year y Fell, Year y+1 actual survivor biology changes from same engine | Relabelled old report, phantom Fell, no recomputation, double-year click |
| X-17 | Ordinary no-work preserves ten Year0–10 digests, no Fell | Auto-thin, unconsented order loss, old batch executes next year, altered golden literals |
| X-18 | Two source years with fresh decisions + read-only actual history | Historical event editable, stale prior approval reused or made-up prior year |
| X-19 | Player Fell cause separate from natural deaths, BA/count source values | All-live batch partial/terminal fake Dq or natural death charged as Fell |
| X-20 | Costs/revenue/budget explicitly UNAVAILABLE absent source | €0/free or invented price/affordability/settlement/retry charge |
| X-21 | Real keyboard + screen-reader + touch + 360px/200% interaction for all S0–S7 | Focus trap, unannounced refusal, red-only state, hidden Execute |
| X-22 | Source/fixture/#784/#805/no-work/P0/Assets protected and hash-checked | Intentional golden or fixture flip passes unnoticed/rebaseline |
| X-23 | Session-only operations, no additional Host/save/economy/rack/Unity | Any implied cross-process replay/save, new rack editor or secondary engine |
| X-24 | Actual source publication, #844 bounded permission, A-1 fix+independent delta, exact lane + physical permission + independent integrated UI qualification | Missing grant or handoff-only assertion labelled release PASS |

**Conditional G1 policy:** R-07, R-08, X-07 and X-09 remain **unchecked**; if G1 stays DISABLED, later qualification may label them **NOT APPLICABLE — G1 DISABLED**, with positive verified X-05 Manual/NotCovered fallback and **explicit #730 adjudication**. This document cannot tick or decide N/A. All other R/X require their own real evidence and recorded negative case.

## 7. Exact proposed *later* implementation file scope (NOT AN AUTHORISATION)

Repository inspection found \`Tools/MultiRackSitkaP1Lab/src/Core\`, \`Tools/MultiRackSitkaP1Lab/src/Cli\`, and separate historic \`Tools/MultiRackSitkaLab/src/Ui\` (the latter is the P0 lab, **not a pre-existing P1 Stage5 Work Plan interface**). There is currently **no \`Tools/MultiRackSitkaP1Lab/src/Ui\` folder** in the inspected #841 branch. A future Manager packet should pin a new full source base and explicit writer/physical claims before opening any path.

| Proposed allowlist / ownership | Purpose and required review | Explicit exclusion |
|---|---|---|
| First, **separate bounded A-1 repair writer**: \`Tools/MultiRackSitkaP1Lab/src/Core/AnnualContinuation.cs\` and narrowly related \`tests/Core.Tests/**\` | Fence legacy year/annual-revision advance against stale work revision, add negative; rerun preserved 62/62+10 digest and independent changed-candidate delta review. **Not the UI author.** | Do not mutate qualified \`8741a328\` in place, lower test assertions or alter annual JSON shapes |
| Later single **adapter/2D UI feature writer**, proposed *new* \`Tools/MultiRackSitkaP1Lab/src/Core/PlayerWorkPlanAdapter.cs\` (name only, not existing), *new* \`Tools/MultiRackSitkaP1Lab/src/Ui/**\` and explicitly enumerated matching \`tests/**\` | Minimal session-owned read/command facade over **same** current root, version-fenced Add/Approve, safe Execute and Advance, accessible 2D list/map, no second model. If project files/build registration become necessary, enumerate their exact paths in the later packet after inspection | No unrestricted \`src/Core/**\`, \`src/Cli/**\`, legacy P0 \`Tools/MultiRackSitkaLab/**\`, fixtures, \`.p0ref\`, binary archive or protected model files |
| Future read adapter review | Proven sourceKinds/committed-only admission, actual tree/stand/event data, atomic pending/approved read and typed UI refusals | No fake API signatures, no direct user JSON mutation or unsafe internal reflection |
| Separate independent UI+engine qualifier | 26R+24X actual traces, accessibility, stale/cancel/atomic negatives, no-work digests on exact changed candidate | Implementer cannot self-certify; old #841 Tier-2 PASS not transferable to changed UI/adapter bytes |

**Approval gates before any source-connected UI code:** (i) current #730 source-ABI and exact candidate review; (ii) source-owner V-01 same pinned hashes (now **prospectively APPROVE IN PRINCIPLE**; still verify identity at actual use), product-owner half and historical variance separate; (iii) independent A-1 correction/delta qualification; (iv) accepted/current read source contract or explicit reviewed minimal adapter boundary; (v) approved exact future implementation packet, one actual source writer, live coordination/physical Ops grants, reviewer; (vi) preserve #804 PARK / main BLOCKED and L4 FAIL38 until separately resolved. No fresh source/API permission arises from this file.

## 8. Shortest genuine player demonstration and ordered qualification

**D0 — admissible starting frame:** Load the exact original accepted 100 IDs from a trustworthy **committed** frame. Show one area, current year, actual living count and real BA. Use one runtime-inspected living exact ID; **do not hard-code which one is alive** for every ecological year. Display its real position/DBH/height (where supported) and capture an explicit player reason. Confirm no work is preselected.

**D1 — deliberate pending + approval (zero biology):** Select an actual live ID and reason; Add it to ordinary PendingWork. Capture current frame/event/count/BA + both revisions before and after Add. Show the approval confirmation separately, produce one \`WorkPlan\` and demonstrate **no Fell, no event and no ecological clock advance** from approval.

**D2 — real Fell and receipt:** Execute the approved plan **once**. Re-read same-year committed frame and \`WorkEvent\`: selected ID has typed player Fell, its actual status changed, living count decreases, BA changes from actual stand calculation, work revision increases **once**, annual year/revision remain unchanged. Re-submit identical key/payload to show a no-op receipt, not second Fell. Attempt a conflicting key/payload and an all-live batch in separately reset/source-appropriate safe qualification fixtures: exact zero mutation.

**D3 — real Year+1:** With no unexecuted orders, explicitly advance one year via the **fully fenced** future adapter and inspect actual survivors and new \`AnnualReport\` (year, counts/BA, natural deaths and any genuine competition/growth fields available). Ensure Fell tree is not among biological survivors and never reappears; compare to the true post-Fell roster, not an old pre-Fell forecast. Show Year y work beside Year y+1 ecology. A no-work separate run must retain the unchanged ten yearly golden digests.

**D4 — necessary refusals:** Try exact wrong-case ID, an already-felled ID, duplicate, blank reason, stale source/work revision, no-approval Execute, final-live-tree batch and missing source. Each must display a distinct *truthful* refusal/no-mutation outcome, while acknowledging current A-4 source-code ambiguity. With pending unexecuted work, exercise year advance **deny** (no mutation) and **consent** (zero Fell cancel, then one real annual step). Verify keyboard/screen reader/touch and 360px/200% against the real screen.

**Minimum evidence for #730 to call the later UI slice playable:** an exact published feature candidate, nonauthor actual qualification of **the whole connected UI+adapter+engine**, source frame/plan/event/annual before-after traces and failure cases, keyboard/touch/SR/zoom evidence, unchanged protected no-work and fixture, accepted Manager verdict. A documentation-only mockup, test method names, old #841 PASS or a screenshot of a changing number is **not** gameplay completion. One-area Stage5 success remains **not yet genuine Multi cross-area simulation**.

## 9. Gate register, owner decisions and handoff to #730

| Subject | Preflight disposition |
|---|---|
| Owner D-093: 170 mm informational; no zero-live batch | **FIXED PRODUCT DECISION**, no reopening |
| Stage5 one undivided original 100 tree area | **APPROVED DESIGN**; no physical racks or automatic blanket marking |
| G1 | **DISABLED** for current one-area feature; near-term distinct multi-area evidence/spec obligation remains |
| G2 | **INSPECTED SOURCE available on exact review ref**; independently tested according to #841 handoff, not integrated/wired |
| V-01 current Scenario One source-owner half | **APPROVE IN PRINCIPLE prospectively** at [#844 direct decision](https://github.com/jermur788/CCF/issues/844#issuecomment-7024491671); verify pinned hashes at real use; no re-request needed for same blobs |
| Product-owner V-01 half | #841 worker-reported; underlying direct wording not independently available here; no retrospective cure |
| A-1 old Advance overload | **BLOCKING** before any new player caller / Host / multi-reader; independent changed-candidate proof still required |
| A-2 / A-3 / A-4 | A-2 PlanId retry ambiguity must be controlled; A-3 test-only hook not exposed; A-4 correct "felled vs typo" UX pending source enhancement |
| #804 / historical V-01/V-02 / L4 FAIL38 / main | **PARK / BLOCKED / UNWAIVED**, no integration verdict here |
| Stage5 26R+24X | **ALL UNCHECKED; NOT RUN by this task** |
| Local code / native /.NET / Unity / ZIP / Windows / Drive / dashboard | **NOT USED / NOT VERIFIED** by this task |

**Manager review request:** Treat this as **source-informed UX/design-and-acceptance preflight**, not source ABI signoff or player feature acceptance. Review the uncovered **Add/Approve displayed-token adapter fence** and internal-vs-public boundary; preserve A-1 mandatory independent repair before UI code. After lawful source/physical admission and corrected candidate, authorise the **smallest actual source-connected player UI slice** with one writer/one nonauthor reviewer; do not let this doc create redundant simulation architecture or reopen settled G0/D-093. Source sharing #844 is resolved for same hashes and needs no further generic audit.

**Handoff completion criteria for #845:** this exact Markdown on its own branch, file readback equality and base→branch compare proving **one docs-only addition**, direct return to #730; #845 may then be **HANDED-OFF**, while future UI implementation remains **not running**. There is **no owner action required for this preflight** and no claim that later implementation is approved or complete.

**Paste-ready continuation for #730 / future dispatch (after Manager review, not a production grant):**

> **Multi-Stage5-Player-WorkPlan-UX — browser preflight HANDOFF, not code.** Direct GitHub artifact: https://github.com/jermur788/CCF/blob/Multi-stage5-player-workplan-ux-preflight-20261119-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-Stage5-Player-WorkPlan-UX-Preflight-20261119.md . Provider: OpenAI GPT-6 GitHub/browser docs-only; branch \`Multi-stage5-player-workplan-ux-preflight-20261119-v1\` based on \`7758e17c882fad01970d2f2fb8c09586bbcab5c1\`; claim #845. Outcome: source-inspected exact \`8741a328\` internal one-area G2 API, proposed accessible committed-frame → exact per-ID reason → PendingWork → distinct whole-plan approval → separate atomic Execute → genuine Year+1 path, 26R+24X future acceptance and UI allowlist. #844 V-01 same-blobs source-owner prospective approval recorded; **A-1 must be independently repaired and delta-qualified before any actual UI caller**. No source/spec/test/main/Unity/physical write or native run; no 26R/24X PASS. Next #730 review and then exact gated UI packet/one writer/independent reviewer, with #804 PARK, L4 FAIL38 and historic variance intact. Owner action: none for doc review.
