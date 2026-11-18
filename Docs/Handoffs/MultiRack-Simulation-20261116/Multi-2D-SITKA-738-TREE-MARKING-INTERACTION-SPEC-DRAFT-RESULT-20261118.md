# Multi #738 Lane C — Tree-marking interaction and reversible proposed-thinning UX

**STATUS: DRAFT — NOT APPROVED, NOT IMPLEMENTED, NOT TESTED.** 18 November 2026.  
**Worker:** OpenAI GPT-6 / ChatGPT Game Dev GitHub-connector, browser/API-only harness. No native host, physical worktree, board, lease, Unity or known reasoning-effort configuration.  
**Claim:** [#738](https://github.com/jermur788/CCF/issues/738), RUNNING registered before editing. **Manager:** [#730](https://github.com/jermur788/CCF/issues/730).  
**Isolated documentation branch:** `docs/multi-738-c-spec-draft-20261118-v1` from freshly checked main `e8e84027faa45d42316cd01bda34ad36393fd069`. **Only authorised output:** this Markdown.  
**Purpose:** Design how a player inspects single/multiple Sitka trees, records and revises tentative removal selections, unmarks, cancels, compares the current forest with the proposed marks, and learns what those choices mean — **without any execution, biological change, authoritative plan, economy or new save**.

## 1. Authority and evidence

This is an interaction **proposal subordinate to the accepted P1 product/technical specification**, not a new work-authorising spec. The [owner-approved four-file P1 specification](https://github.com/jermur788/CCF/blob/docs/multi-p1-637-owner-approved-four-file-20261116-v1/specs/multirack-sitka-p1-adult-rules-comparator-v1/spec.md), especially `5.2, already defines the future P1 Draft → Proposed → Approved → PendingWork → Executed lifecycle, explicit Rejected/Cancelled/Stale terminals, **no cut on marking/proposal/approval/queue**, and execution eligibility of living trees at current authoritative DBH meeting the imported threshold. It does **not** prove that UI, Work/Host, save or command service currently exists. Its four files received explicit owner approval on 16 November 2026; the later UI and shared contracts proposed here have *not* received product approval. Main Decision Log **D-084 concerns Scenario One Compact30**, while the historical unmerged Multi owner approval used the same number; [#740](https://github.com/jermur788/CCF/issues/740) correctly identifies this unresolved canonical collision. Until #730 resolves it, refer to the dated owner approval by link, not an unqualified D-084 number. Main **D-082** already establishes the synthetic/non-forecast disclosure and, for *future real open plans*, conditional staleness on successful load.

### Evidence inspected

| Source | Supported boundary | Important limit |
| --- | --- | --- |
| [Complete #738 packet](https://github.com/jermur788/CCF/blob/docs/multi-730-parallel-workstreams-20261118-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-738-TREE-MARKING-INTERACTION-SPEC-DRAFT-PACKET-20261118.md), #738 and live main coordination | Separate C-only docs result, no source, native, shared file, board, tests/build, Unity, Windows, merge or L4 waiver | No source ownership or physical rights created by this claim |
| [#725 local Year1](https://github.com/jermur788/CCF/blob/6e51dcb6c86eab78b99b4b05e08c2c44e0ffdfa9/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-725-EXACT-R-ADULT-SITKA-YEAR1-IMPLEMENTATION-RESULT-20261118.md), [#729 independent review](https://github.com/jermur788/CCF/blob/8310a38086425a5b223315c8f83181dd77eee1eb/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-729-INDEPENDENT-EXACT-R-YEAR1-FIVE-FILE-SOURCE-AND-NUMERICAL-REVIEW-20261118.md), [#732 independent focused native qualification](https://github.com/jermur788/CCF/blob/88e82cd0cc3e60dcc777720332200a88231d4b8e/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-732-EXACT-R-YEAR1-FOCUSED-INDEPENDENT-NATIVE-QUALIFICATION-RESULT-20261118.md), [#735 custody](https://github.com/jermur788/CCF/blob/816387ae05ffed4ac24e2560a08256068e459efd/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-735-725-QUALIFIED-YEAR1-SOURCE-CUSTODY-RETENTION-RESULT-20261118.md) | 100 adult Sitka in accepted synthetic 30 × 30 m input; all 100 live Year0→Year1, age 30→31, no deaths; 12 initially under the 170mm action threshold still participate in all-living growth/density | Local Year1 code is **not integrated**; #732 passed focused offline resume after an initial stopped network attempt; not Unity parity, save or actual mark implementation. #729 M-1 dead-tree filter must be fixed *before Year2* |
| [#740 shared immutable snapshot DRAFT](https://github.com/jermur788/CCF/blob/docs/multi-740-e-spec-draft-20261118-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-740-SHARED-SNAPSHOT-AND-INTEGRATION-READINESS-SPEC-DRAFT-RESULT-20261118.md) | Proposes detached versioned E-owned tree/stand read model with domain, fixture digest, generation, snapshot/revision, ecological year/stage, ordinal tree ID, X/Z metres, exact numeric representation | No public ABI/adapter or work command exists yet |
| [#736 viewer A DRAFT](https://github.com/jermur788/CCF/blob/docs/multi-736-a-spec-draft-20261118-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-736-READONLY-2D-WOODLAND-VIEWER-SPEC-DRAFT-RESULT-20261118.md) | A owns pan/zoom, source-to-screen transform, hit tests, primary selected ID, read-only inspector and a provisional selection callback | No C marks in A; selecting a tree cannot mean marking it |
| [#737 engine B DRAFT](https://github.com/jermur788/CCF/blob/docs/multi-737-b-spec-draft-20261118-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-737-MULTIYEAR-ANNUAL-ENGINE-SPEC-DRAFT-RESULT-20261118.md); [#739 history/persistence D DRAFT](https://github.com/jermur788/CCF/blob/docs/multi-739-d-spec-draft-20261118-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-739-HISTORY-AND-PERSISTENCE-DESIGN-SPEC-DRAFT-RESULT-20261118.md) | B sole biological writer and later live/dead succession; D future event/history/archive/save grammar | No multi-year, plan event persistence, secure Host or replay implementation demonstrated |

**Evidence classes:** 100-tree/zero-death Year1 is experiment evidence; P1 `5.2 and D-082 are accepted intent; the input gestures, colours, state machine, undo rules and UI layouts below are **design proposals/UX calibration**, not forestry-standard prescriptions or tested features.

### Binding non-effects and refusal rules

1. **Selected ≠ marked ≠ proposed Work Plan ≠ approved ≠ queued ≠ executed.** A candidate for removal is not dead, Felled or missing from the forest. A future natural death is not a player Fell.
2. Every UI operation is read-only against the forest. No click, batch, undo, Compare, discard or draft validation alters `alive`, DBH, crown, competition, ecological year, source stand metrics, event/history tip, approved work or RNG. A local UI edit cannot call AnnualEngine or ExecuteWork.
3. Tree identity is case-sensitive ordinal `treeId` under source domain + fixture digest + experiment/generation + ecological year + snapshot/stage/revision. Screen pixels, display sort order, list index, CSS radius and pointer position are NEVER forest identity.
4. The P1 adult experiment remains **synthetic, experimental, pure Sitka, not an Irish field forecast and not Unity numerical/geometry parity** on every draft/compare/trace view. No inferred health score, timber revenue, post-harvest BA, biodiversity, canopy-light or future growth prediction is allowed.
5. All living trees, including those below the work threshold, stay inspectable and biologically included. **Current authoritative DBH** and living status control future ordinary work eligibility under the already-approved P1 spec; do not use rounded inspector labels, year-zero flags or biological inclusion as permission to propose executable work.
6. A read or command refusal must fail closed. Current #725 local source may not be imported ad hoc; protected #710/#725 custody, accepted fixture and P0 remain untouched. Default .NET **L4 FAIL38 UNWAIVED**.
7. The UX proposal is local-session ephemeral state only, not an event-sourced history, save schema, completed work record, origin/CSRF policy, or promise of an executable Plan service.

## 2. Proposed user journey and visual contract

**Initial mode: Inspect.** A's source-faithful top-down 30 × 30 m map shows unchanged crown disks and distinct stem centres. A owns selection and inspector. Pressing a stem displays its stable ID and exact-sourced fields; no mark is made. Enter **Draft treatment** explicitly to open a non-authoritative local candidate set. The screen and inspector continue to display every original stem. A synthetic-source banner stays visible.

Suggested desktop arrangement (text wireframe, not a screenshot):

~~~text
┌──────────────────────────────────────────────────────────────────────┐
│ Synthetic Sitka experiment · Year 0 · NOT AN IRISH FIELD FORECAST    │
│ NOT UNITY PARITY                           [Inspect] [Draft treatment]│
├───────────────────────────────────────────┬──────────────────────────┤
│ TREE MAP — source stem centres and crowns │ INSPECT                  │
│                                           │ ID: P30-001             │
│   ●   ●  ●  ⊖●  ●   ●                     │ Living; DBH, H, crown   │
│   ●  ◎●  ●    ●                            │ Source year and stage   │
│                                           │ No health score        │
│ ⊖ dashed removal-candidate ring          ├──────────────────────────┤
│ ◎ selected/focused halo ONLY              │ LOCAL DRAFT · NO WORK   │
│ [Fit] [Zoom] [Search ID]                  │ Selected: [ID set]     │
│                                           │ Removal candidates: 2  │
│                                           │ [Mark] [Unmark]         │
│                                           │ [Undo] [Redo] [Compare] │
│                                           │ [Discard draft]        │
├───────────────────────────────────────────┴──────────────────────────┤
│ Compare = same live source + draft marks, NOT post-thinning outcome. │
└──────────────────────────────────────────────────────────────────────┘
~~~

**Visual layers:** A draws the immutable forest boundary, sourced crowns, all stem centres and alive/dead/not-actionable symbols. It then paints C's **separate candidate-overlay glyphs**, not source objects. Suggested removal candidate: dashed red outer ring **and** text/icon "Proposed removal" in inspector/legend; primary inspection: double or solid halo with ID; explicit focus outline; dead: cross/word only if source says alive=false; below-threshold: distinguishable notice, never hidden. Colours are secondary, not the only signifier. Retain-priority/blue CropTree annotation is **OPEN-C2**, not silently imported from Scenario One D-013 or the unimplemented P1 plan.

**Modes and controls:**

- **Inspect**: tap/click stem, keyboard ID search or Prev/Next to inspect; hover is optional and not the only route. Escape clears inspection focus (not draft marks). A's hit testing is stem-centre based, deterministic with ordinal ID tie-break; C never reimplements hit testing.
- **Batch select**: accessible ordered-ID checklist with count, including keyboard toggles; Ctrl/Cmd-click may add/remove IDs only if a richer A activation event is approved. Shift ordinal range must explicitly indicate inclusion, never secretly include filtered/hidden IDs. A future Select Area mode may enable marquee, but not conflict with drag-to-pan. Touch uses checkboxes/tap actions rather than relying on modifier keys.
- **Mark selected**: explicit toolbar/button action for selected, currently eligible IDs, after full-batch validation; **never** a side effect of selection, double-click, hover, pan, zoom or year switch.
- **Unmark selected**: remove only existing local annotations; unmarking is not reviving a tree. A blank/no-op application changes no meaningful edit history.
- **Undo/Redo**: local draft-mark changes, grouped by toolbar action, not forest history. New edit after Undo invalidates Redo branch. An implementation needs a deterministic bounded memory policy; size is OPEN-C5 rather than an ungrounded unlimited stack.
- **Compare**: read-only *same source* shown with overlay on/off or two side-by-side panels, plus candidate ID additions/removals. It must say "0 executed trees" unless separate real source work says otherwise. It must not show simulated stumps, released crowns, sales, residual RD or future years.
- **Discard**: confirm only if candidates present, clear local draft/undo/checklist and return to Inspect. Cancelling the confirmation retains everything. A future real PlanCancelled event is outside this UX slice.
- **Review draft**: local candidate ID list, source key and validation warnings, labelled **not submitted, not approved, not executed**. No "Cut", "Approve", "Execute", "Advance" or "Save forest" button in this Lane C slice.

Suggested microcopy: **"These are trees you might remove. The forest is unchanged. Review, revise or discard your proposed marks."** Each ring: **"Proposed removal — no felling performed."** Compare footer: **"This view compares your choices, not predicted forestry outcomes."**

## 3. Six worked flows

**F1 / single tree.** Start from one verified, complete E read snapshot. Select a tree by A map or ID search. Inspector displays exact sourced DBH, status, year and provenance. Enter Draft treatment; C opens an empty local candidate map bound to this snapshot. Press Mark selected. *If* current authoritative work eligibility passes, a dashed annotation is added in local state. Stand living count/BA/ages/source revision/crowns remain exactly equal before and after.

**F2 / multi-select and reversible edit.** Use ordered-ID checkboxes to select A and B, Mark both as one transaction, then Unmark B. Candidate count goes 0→2→1 and the changes are both undoable. Undo the unmark restores B; undo the original mark removes A and B together. A changes neither primary hit-test nor view camera. A viewer event with only a primary ID cannot implement Ctrl/Cmd batch intent; the fallback checklist works without modifying A.

**F3 / below-threshold but biologically present.** Inspect `P30-053`, one of the 12 accepted Year-0 trees initially below 170mm. It remains visible, alive and in every biological calculation. When source-qualified current DBH is below the **imported** P1 work threshold, the conservative C proposal disables ordinary removal marking and explains "Below action threshold for this year; still in the forest." It does **not** call it dead or remove it from RD. Whether an explicitly non-executable warning-only sketch should instead be permitted is OPEN-C3. For Year1, reevaluate current bits, not old Year0 threshold membership.

**F4 / compare with no invented forecast.** Source panel A shows the same complete 100-ID snapshot as panel B. B merely adds dashed rings to C's candidate IDs. It reports candidate **count** and **exact IDs**, not a biological or economic post-thinning value. An actual Year0↔Year1 historical comparison is owned by D #739 and must be separately labelled; it is not proof that unexecuted marks caused growth.

**F5 / changed source revision.** A loads an independently available, valid Year1 snapshot and can continue inspecting the *same stable ID* on that read frame. C's old Year0 proposal becomes **STALE_READ_ONLY**, with copy "Draft belongs to Year 0. Review or start a new draft for Year 1." No silent rebase, authority transfer, auto-mark, saved plan or execution. If Year1 payload is missing/mismatched, A disables Year1; C retains the last valid frame and cannot fabricate new DBH or eligibility.

**F6 / only future executed work.** With **separately approved** P1 Work/command semantics, a revised proposal may later be turned into an authoritative Draft/Proposed plan, reviewed and approved by the player, queued PendingWork, then executed once against current live IDs/DBH/year/revision/session. Only a valid executed command changes alive status and creates actual fell provenance; annual ecology is a further distinct step using B's pre-growth live set. This future route depends on B multi-year and M-1 correction, D events/save policy, E identity/security, an approved management spec, physical source gate and independent qualification. No such route is implemented by C.

## 4. Formal local UI state machine — proposal, not engine semantics

### 4.1 Independent axes

| Axis | States | Owner | Biological effect |
| --- | --- | --- | --- |
| Read | NONE, VALID, REFUSED, REVISION_CHANGED | E snapshot, A display | None |
| Inspection | NONE, HOVER_ID, PRIMARY_ID | A / #736 | None |
| Batch focus | set of exact case-sensitive ordinal IDs, independently of primary | C / #738 | None |
| Draft | NONE, EDITABLE_EMPTY, EDITABLE_MARKED, REVIEWING, STALE_READ_ONLY, DISCARDED | C / #738 | None |
| Per-ID draft intent | UNMARKED, REMOVE_CANDIDATE (optional RETAIN_PRIORITY = OPEN-C2) | C | None |
| Future plan | Draft, Proposed, Approved, PendingWork, Executed; Rejected/Cancelled/Stale | Future separately authorised P1 plan/event owner | **Only Executed work changes tree biology** |
| Forest annual | committed state and ecological year | B / #737 future engine | Yes, only when independently approved and committed |

### 4.2 Transitions

| Input | Guard | Proposed UI result | Must not happen |
| --- | --- | --- | --- |
| Source snapshot accepted | E complete, provenance-verified, unique ID roster | Inspect default; no local mark | No annual step, no new actual plan |
| Activate/select tree | ID and exact E revision match | A primary inspected ID, optional C batch gesture | No mark on activation |
| Open Draft | VALID read and explicit user action | local EDITABLE_EMPTY envelope with base key | No engine event |
| Mark one/many | EDITABLE; entire ordered batch valid on same source revision | atomic add/remove candidate-map entries, one grouped Undo record | No automatic partial marking or cutting |
| Unmark | EDITABLE, exact ID subset | remove specified annotations only; no-op if none | No tree revive or work reversal |
| Undo/Redo | EDITABLE; valid local stack and matching base | restore a previous local mark map; new edit clears redo | No authoritative state rollback |
| Compare | VALID read, draft base matches read | REVIEWING overlay comparison; close returns previous draft | No after-fell simulation |
| Cancel comparison / discard dialog | modal open | close modal only, mark state unchanged | No implicit data loss |
| Confirm Discard | explicitly confirmed | clear local draft marks, batch and undo; return to Inspect | No PlanCancelled event or save |
| Switch year, new fixture, live reload/generation | E base key changes | C draft STALE_READ_ONLY; block further edits pending explicit resolution | No silent rebinding/cross-year mark carry |
| Source refusal/invalid revision | typed error, missing/out-of-order ID, invalid numeric, incomplete roster | last valid frame retained, new edit refused, visible reason | No fallback to untrusted data |
| Future Submit/Approve/Execute | **NOT AVAILABLE IN C** | unavailable until separate reviewed P1 command/service contract | No inferred felling power |

### 4.3 Ephemeral data envelope (illustrative, not a public API or save)

~~~text
LocalTreatmentDraftV0
  baseSnapshotKey:
    experimentId, generation, domain, fixtureId, fixtureByteSha256
    snapshotId, snapshotVersion, ecologicalYear, sourceStage
  localDraftToken (UI token, never a work-command/plan ID)
  editRevision (monotonic local UI number)
  marksByTreeId (case-exact ordinal treeId -> REMOVE_CANDIDATE, editSeq)
  inspectedPrimaryId (A UI only)
  batchSelectionIds (C UI only)
  undoRedo (bounded list of reversible local mark-map edits)
  provenanceReference (E source read ref; no new biological values)
~~~

Draft markers **never** have authority to alter the model, archive, plan/approval session epoch, source hash, RNG, or inventory. Selected IDs may be absent from marks; marked IDs may be absent from the current selection. One annotation per ID, no duplicates; display ordering must be ordinal. The local token cannot become a durable command ID or Plan ID by renaming it. The UI state must not be persisted through browser localStorage or P1 file save until a separately approved D/command policy exists.

**Atomic batch default:** Prevalidate every ID in the submitted set using the *same source revision* and accepted eligibility representation. On one invalid/ineligible ID, refuse the **whole batch** with per-ID reasons and a way to adjust the selection. Never quietly mark N-1 and report N marked. Partial apply is a separate unresolved UX/product choice. Skip no-op calls in undo history. Marking disabled if eligibility is unknown; an unsupported source hint must not be treated as a real work right.

## 5. Small A / C / E / B / D interface proposal

These are **logical ports only, not existing methods, namespaces, Host endpoints or assigned implementation files**.

| Interface / event (provisional) | Producer / consumer | Minimal contract | Who decides |
| --- | --- | --- | --- |
| `ReadCurrentSnapshot`, `ReadTree`, `ReadComparison` | E → A,C,D | immutable read projection with complete IDs, alive, exact current DBH bits, source X/Z metres, snapshot identity/revision, stage, eligibility provenance or explicit Unknown | #740 owns read schema and adapter, not C |
| `ReadonlyViewer.onSelectionChanged(treeId,snapshotRevision)` | A → C | one primary inspected ID + read revision; no pixel or mutable model reference | A #736 |
| `TreeActivationIntent` (richer proposal only) | A/host adapter → C | stable ID, exact snapshot key, activation kind and modifier/selection intent | OPEN-C6: A/E/#730 decide; current A proposal emits only single-ID |
| `DraftMarksChanged` | C → A | base snapshot identity, local editRevision, immutable ordered (treeId, candidate glyph) list | C owns marks, A only paints overlay; E owns identity |
| `DraftSummaryRequested` | C local UI | ordered candidate IDs, source stage/year, warnings and "no work executed" provenance | C |
| `FutureSubmitProposal`/`FutureExecuteWork` | **NOT EXPOSED** | requires current-authority validation, explicit approval, idempotent command, work ledger, C↔D↔B integration, safe Host policy | Separate approved P1 command specification and owner grants |

**A/C conflict resolution:** A's current proposal owns stem hit-test, transform, click/keyboard inspector and one primary selection callback. C owns batch selection and proposed marks; it **must not** change A files or introduce its own hit-test. A's single-ID event lacks Ctrl/Cmd/Shift intent: C V0 can use an independent accessible ordered-ID checklist. If direct map multi-click is desired, E and A must review a richer event before C depends on it. No duplicate event APIs invented in production. A annotation primitive is a presentation-only derivative; it never modifies E `TreeRow`. C refuses callbacks from prior source revisions or IDs not in its bound complete roster.

**Future work submission boundary:** A local mark overlay is not the approved P1 Draft/Proposed object. Submission needs a separately versioned command grammar and current whole-set eligibility, E source token, D event/approval history, valid session epoch and B state owner. An already executed tree can never be re-executed by a stale overlay. A loaded open Approved/PendingWork plan must follow owner-confirmed D-082: on successful load, exactly one `PlanStale(SessionLoaded)` per such plan and **zero load events** if none open; C's local draft does not create that event. Actual load/replay and idempotence remain outside #738.

**UI-only errors** (names proposed, not engine refusal strings): `StaleSnapshot`, `UnknownTreeId`, `DeadTreeNotActionable`, `BelowActionThreshold`, `EligibilityUnknown`, `BatchContainsInvalidCandidate`, `SourceRevisionMismatch`, `DraftUnavailable`. E/P1 engine refusal vocabulary remains separate, including existing `OriginalAnnualRefused`, `AnnualAlreadyAdvanced` and `FileHashMismatch`; never convert them to success states.

## 6. Illustrative mock cases

### M1 — proposal without felling

~~~text
Source: synthetic 30m Year0, 100 living
Inspect P30-001 using A. Actionable status: consult CURRENT reviewed E source.
Enter Draft → Mark selected (only if eligible):
  Local marks: { P30-001: REMOVE_CANDIDATE }
  Display: same stem and same crown + dashed "proposed" ring
  Counts: 1 candidate / 0 executed fell / unchanged source living N
Unmark / Undo / Discard: remove only the local ring.
Never say: timber sold, stump, tree dead, revenue, projected growth.
~~~

This example **does not claim** independently checked current DBH or eligibility for P30-001; eligibility must be verified from the exact source snapshot.

### M2 — known below-threshold source tree

~~~text
P30-053 at Year0: living; in the accepted initially sub-170mm set.
Inspect/select: YES; part of ALL-LIVING growth and RD: YES.
Conservative removal candidate: NO while below current imported threshold.
Explain: "Below the action threshold for this year; still in the forest."
Do not label dead, hide from density, or invent a future Fell.
~~~

### M3 — true current-vs-proposed comparison

| Panel | Same source input | What is permitted |
| --- | --- | --- |
| Current forest | Exact bound read snapshot; all stems and crowns | Source metrics, actual year/stage |
| Proposed treatment | **Identical** read snapshot with C's local candidate-ID rings | Which IDs might be removed; count; undo/revise |
| Future actual-after-work (UNAVAILABLE) | Requires separately approved executed-state witness | No invented post-thinning stand, canopy or economy |

Year0→Year1 is a D-owned **historical** read comparison. It is not caused by a Year0 local proposed mark.

## 7. Testable acceptance cases for later implementation (NONE RUN here)

| ID | Acceptance or negative case | Required invariant |
| --- | --- | --- |
| C-01 | Valid 100-ID source opens | All IDs inspectable, no marks or events, visible synthetic disclosure |
| C-02 | Map pick / ID search / Next ID | Same ordinal identity irrespective of camera, overlapping crowns or list order |
| C-03 | Hover/click/pan/zoom/keyboard focus without Mark | Zero marks; selection does not mean work |
| C-04 | Keyboard-only ID checklist, mouse and touch | Same explicit selected ID set, visible focus and accessible count |
| C-05 | Explicit Mark eligible batch | Atomic local map update and one Undo entry, **zero source mutation** |
| C-06 | Mark same ID twice / Unmark absent ID | Idempotent local no-op; no duplicate records or history inflation |
| C-07 | Unmark selected subset | Only local annotations removed; biological state untouched |
| C-08 | Undo grouped batch / Redo / new edit after Undo | Exact local reversible set; redo branch cleared after new edit |
| C-09 | Escape/Cancel dialog | Draft retained; does not silently discard |
| C-10 | Explicit confirm Discard | Local draft cleared; no real PlanCancelled/history/save event |
| C-11 | Compare source vs candidate overlay | Identical source crowns, N/BA/DBH/year in both; no post-fell or growth forecast |
| C-12 | Known Year0 sub-170mm P30-053 | Inspectable, biologically included, conservatively removal-ineligible if current source still below threshold |
| C-13 | DBH label rounds across 170mm boundary | Use authoritative current DBH bits and rule, not rounded label or initial threshold flag |
| C-14 | Known dead ID from independently accepted future vector | Still inspectable, cannot be ordinary removable living tree; no phantom natural cause |
| C-15 | Mixed valid+invalid batch | All-or-nothing block with per-ID reasons; no hidden partial marks |
| C-16 | Verified Year switch while local marks exist | Inspector can retain stable ID; C draft becomes STALE_READ_ONLY, never auto-rebased |
| C-17 | Year1 payload absent/untrusted/hash-mismatched | View disabled, no fabricated Year1 values or eligibility |
| C-18 | New experiment/fixture/domain/generation | Draft cannot silently cross the identity fence |
| C-19 | Duplicate/case-swapped ID, invalid numeric, missing complete roster or unknown schema | Refuse new mark and unsafe snapshot; last valid state unaffected |
| C-20 | Out-of-order A selection callback | Reject stale revision/ID; no old-frame marking |
| C-21 | Filtered or overlapping stems | Accessible complete ID roster; do not select unseen range silently |
| C-22 | 360 CSS px, 1280×720, 200% zoom and high-contrast mode | Controls, count, labels and glyph meanings remain usable |
| C-23 | Draft review panel | Shows exact IDs/base source and not-submitted status; **no Approve/Execute button** |
| C-24 | No work pressed then normal read navigation | No real Draft/Proposed/Approved plan or event tip change |
| C-25 | Future genuine work (different approved scope) | Whole reserved set validated at current state, actual execution once; no fell on approve alone |
| C-26 | Year2 in the future | Must be blocked until B fixes/qualifies #729 M-1 dead-stem exclusion |
| C-27 | Successful future load with open plans | D-082 conditional stale event per open approved/pending plan, zero otherwise, not by local C draft |
| C-28 | Security/integration status | L4 FAIL38 not waived, original source not touched, UI proof not mislabeled Unity/field parity |

These are **future UI state/accessible interaction and interface tests**, not test results. Focused scripted UI checks plus a small rendered keyboard/pointer inspection should precede any full system work-plan integration. A complete .NET L4 or Unity run is not warranted solely by authoring this document; future source and security grants must be explicit.

## 8. Open decisions and blockers to Manager #730

| Ref | Decision / missing authority | Proposed default and owner |
| --- | --- | --- |
| OPEN-C1 | Exact E snapshot/schema/adapter, source stage, action-eligibility projection and import path | #740 and Manager approve first; C cannot create competing Core API |
| OPEN-C2 | Candidate intent taxonomy: just proposed removal or priority-retain/crop-tree as well | Recommend one REMOVE_CANDIDATE type first. #730/owner decide any broader Multi intent; Scenario One mark modes are not automatically inherited |
| OPEN-C3 | Hard-disabled known-ineligible IDs vs warning-only non-executable sketch | Recommend hard-disable with exact current-source reason; P1 approved ordinary work rule remains controlling |
| OPEN-C4 | Cross-year/restart draft rebase vs discard | Conservative V0 stale/freeze/discard and new draft; any explicit rebase needs E/D/Manager approved semantics |
| OPEN-C5 | Local undo cap, draft lifetime, page reload policy | Local session only, no file/localStorage/save. C/D/Manager choose memory cap and UX |
| OPEN-C6 | A single-ID callback insufficient for on-map modifier multi-select | Fallback accessible ID checklist; richer tree activation event must be jointly reviewed A/E |
| OPEN-C7 | Future P1 command grammar, authoritative plan ID, validation errors, approval epoch, atomic cancel, audit | P1 work-command owner with D/E/Manager; not C local code |
| OPEN-C8 | Actual host/static viewer/loopback choice and legally safe Year1 source import | Owner + #730/#740; no Host/port/data claim here |
| OPEN-C9 | Beyond proposal: execution and true next-year consequences | Separate approved P1 management spec + B multi-year M-1 fix + D event/save + security/physical source gates |
| OPEN-C10 | Canonical D-084 collision in historical Multi owner approval | #730/owner, not this C worker |
| OPEN-C11 | Release/security: local #725 unintegrated; P0 protection, L4 FAIL38 | Native custodian and independent reviewer, no C waiver or integration |

**Blocking before first UI code:** review E/A shared ports, approved/current source and legal read-only data delivery, agreed mark vocabulary, independent spec/product acceptance as required, exact native implementation writer and allowed files. **Blocking before real felling:** a separately approved management spec, B and D implementations with negative tests, source/CCF-Ops ownership, L4 policy, independent work-command security review. Do not infer approvals from a successful documentation handoff.

## 9. Staged implementation plan (proposed only)

| Stage | Prerequisites | Sole future implementation owner and deliverable | Qualification |
| --- | --- | --- | --- |
| P0 shared read foundation | #730 reconciles E schema/host and legally vetted source payload | E-owned detached read projection, A-owned read-only renderer/inspector | ID, f32/f64, provenance, read-only, negative input/refusal checks |
| P1 local marking | Independent #738 draft review and explicit product/UI approval; new claimed source/physical writer where necessary | C owns local batch/draft/Undo/Redo/Discard and proposal view; A renders overlay only | C-01…C-24 targeted UI and accessibility checks; no engine calls |
| P2 honest compare | Same verified immutable source, E/A annotation contract | C exact candidate-set diff, A same-source map, D separate Year0/1 history compare | No postwork forecast, stale revisions, prominent disclosure |
| P3 real work-service gate | Separate approved plan/command spec, B Year2 with M-1 fixed, D events/save and security/physical authority | One assigned P1 Core writer builds Draft→Proposed→Approved→PendingWork→Executed, no C-owned biological mutations | Whole-set current eligibility, idempotent commands, session staleness, no cut until actual Execute |
| P4 consequence and history | Actual executed work and independently qualified annual successor | B computes postwork next year; D records actual work/history; C may only display sourced completed results | Real Fell vs natural mortality, revised live neighbours, correct annual history and safe replay/load |

Start with a **Year0-only** inspected/draft view if Year1 has not been safely exported from local custody; no rounded-table reconstruction. This sequencing is not a permission to build the interface. Every source implementation requires a new approved packet, actual writer/branch/worktree/ownership and mandatory evidence. The owner's no-arbitrary-agent-expiry policy requires physical custodian reconciliation; this docs lane allocates no timer, lease, board row or native host.

## 10. Handoff and evidence limits

**Changed:** exactly this one new DRAFT GitHub Markdown on the isolated docs branch; after publication its contents/one-file delta must be read back. **What passed/failed here:** only documentation/source inspection and future acceptance specification; no source/tests/build, runtime/UI, Unity, Windows, physical Ops, files/save, integration, release or L4 test was performed. #725 source remains local/unintegrated; #729 M-1 blocks Year2. Original #710/#725 protected source and P0 were not accessed physically.

**Next:** Manager #730 independently reviews this DRAFT alongside A #736, B #737, D #739 and E #740, returns **APPROVED / NEEDS CHANGES / REJECTED for document quality**, resolves interface/product decisions and only then issues a separate source packet if safe. DRAFT status itself conveys **no production authority**. **User action now:** none for the documentary handoff. **Dashboard/Drive synchronisation:** unverified from this browser/GitHub-only worker.
