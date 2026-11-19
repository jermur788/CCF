# Multi Stage 5 — Source-authoritative 2D forestry intervention workflow (V1)

- **Spec ID:** MULTI-812-STAGE5-2D-INTERVENTIONS-V1 (corrected draft under #817)
- **Status:** **DRAFT — NOT APPROVED / NO IMPLEMENTATION AUTHORITY**
- **Date:** 19 November 2026; #812 original, #817 four-file owner-aligned correction
- **Owner:** Multi Manager [#730](https://github.com/jermur788/CCF/issues/730); #817 browser/GitHub documentation writer, no local worktree
- **Authority:** latest direct owner [one-area clarification](https://github.com/jermur788/CCF/blob/docs/multi-730-817-owner-one-area-clarification-20261119-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-730-817-OWNER-CLARIFICATION-ONE-UNDIVIDED-AREA-NO-RACK-GEOMETRY-20261119.md) **supersedes rack-specific parts** of [#816 independent review](https://github.com/jermur788/CCF/blob/docs/multi-816-independent-stage5-one-rack-review-20261119-v1/Docs/Handoffs/MultiRack-Simulation-20261116/816-orchestration/Multi-816-INDEPENDENT-STAGE5-ONE-RACK-SPEC-AND-OWNER-DECISION-REVIEW-20261119.md) and [original #817 packet](https://github.com/jermur788/CCF/blob/docs/multi-730-817-stage5-four-file-correction-20261119-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-730-817-STAGE5-ONE-RACK-FOUR-FILE-DRAFT-CORRECTION-HANDOFF-20261119.md). Canonical Decision Log D-010/D-064/D-065/D-071/D-076 and separately scoped D-082/D-083 provide context, not a literal Multi rack design.
- **Companion source contract:** [#805 four-file corrected DRAFT, especially §6.3](https://github.com/jermur788/CCF/blob/docs/multi-805-committed-frame-admission-matrix-20261119-v1/specs/multirack-sitka-2d-read-contract-and-candidate-ux-v1/spec.md); neither approval nor an executable Fell authorisation.
- **Review:** focused nonauthor correction-delta review, then #730 formal spec decision; implementation/feature tests **NOT RUN**.

## 1. Product intent and binding G0

The first-pass standalone synthetic **30 × 30 m, 0.09 ha, 100-Sitka 2D stand is ONE UNDIVIDED MANAGEMENT AREA**. Its full stand is in scope for player inspection and planning, **not** a command to treat all 100 trees. Valid felling, retention and sample decisions attach to exact, individually validated, live source-tree IDs. The player demonstrates judgement by manually selecting trees or selecting a representative local sample; optional comparison-based extension produces suggestions only when a separately proved G1 predicate permits it. A proposal is not Fell.

**G0 RESOLVED by latest owner:** no physical rack layout, entry, corridor, served-tree eligibility, rack Confirm, rack-clearing PendingWork or rack geometry edit/lock in this V1. The prior expression “one rack” meant one management area. **Do not invent a decorative or functional rack.** A later larger stand may have multiple management/rack-served areas, and genuine extraction-rack features only if separately designed/approved. V1 source/work records should carry a management-scope identifier with a single current value (conceptual schema only), so eventual multiple areas need not change tree-ID or approved-work semantics. No speculative topology, numeric corridor or hidden spatial partition.

### 1.1 Historical-to-Multi interpretation

| Source authority / provenance | First-pass Multi use | Non-transfer boundary |
|---|---|---|
| **Latest owner clarification / G0** | One undivided synthetic management area with per-ID player decisions; no rack mechanism | No confirmation, clearance, rack entry or corridor state |
| **D-064 / D-071, historical Enlarged80** | Historical conceptual lesson: planning and pending work are distinct from final execution | Five parallel headland racks, 16–20 m spacing, 3D entrance spray, rack persistence, Legacy40/Reference Future save identities and third-rack lessons are not Multi geometry |
| **D-065** | Source-backed player-selected sample/manual judgement, independently justified comparable-only candidate proposals, explicit pending orders and Work Plan approval | No universal Irish method called “operator selection”; no automatic treatment of one whole stand or hidden Crop Tree quality labels |
| **D-076** | Fresh representative sample or manual work in each actual management cycle; preserve history of source, choices, proposals and outcomes | No fixed 24 m sample, 3 Crop Trees, sample-rack geometry or automatic per-cycle propagation |
| **Prior Multi P1 / #805 §6.3** | Distinct draft/proposal, optional prescription approval, PendingWork, Work Plan approval, once-only executed result | #805 REMOVE_CANDIDATE and a read-only preview remain ephemeral, not Fell or a persistent work ledger |

**Evidence boundary:** Irish/UK forestry supports control/example marking and constrained delegated treatment as *analogues*, not this simulator’s exact process. One-area lab policy and work states are accepted owner direction plus proposed gameplay architecture. Comparator thresholds, density assumptions, economics and behavioural constants are **not** sourced or adopted by this DRAFT.

## 2. Source-of-truth and stand-scope rules

- Preserve the **closed synthetic original 100 exact case-sensitive tree identities** and fixture/hash lineage. This is neither the Compact30 175-tree Unity beta nor historical Enlarged80 Scenario One, and it is not a measured Irish forest forecast.
- Editable observation can use only the **source-proved committed Genesis or AfterMortality end-year** admitted under #805 sourceKind/lineage/revision and exact f32/f64 word rules. Before/PostGrowth are explanatory diagnostics only. Invalid or uncommitted source means **refuse planning/execution**, no invented current year.
- Validate the exact live tree ID, life state, year, generation, fixture, model/revision and available biological/XY/competition data from **one** current committed source. Missing fields are UNAVAILABLE, never synthesized from a displayed map or rounded values. A 2D position aids inspection but does **not** create physical rack service.
- **Scope is exactly one area, not all-tree action:** an explicit action list must enumerate each selected live ID; no broadcast Fell and no implicit “stand wide = Fell all”. Future-area identifier is a distinct contract dimension, not an invented subdivision in V1.
- #805 local REMOVE_CANDIDATE is reversible, stale-fenced preview state. It can enter a Stage 5 draft only after deliberate current-source revalidation and explicit player choice; its display or undo stack cannot authorise work.
- A **single audited authoritative engine** owns actual Fell, death cause, post-work roster/counts/BA and year/event recording. UI must not delete a tree visually, mint committed frames, invoke a hidden duplicate execution route or change the protected original/no-work model.

## 3. Player workflow and required state boundaries

1. **Inspect.** Show accepted source/experiment/fixture, committed year, management area (one undivided area), live/dead exact IDs and genuinely available current measurements. Read-only inspection remains available even when work is refused.
2. **Select manually or demonstrate a local sample.** The player selects individual living trees, inspects retained/crop/release/competitor relationships and records a reason per decision. A sample consists of explicit source-backed exact IDs and current-neighbourhood evidence when available; it does not create any smaller management area or rack. A sample with inadequate evidence is labelled **unqualified**; per-ID manual selection remains available.
3. **Optional treatment proposals (G1-gated).** A transparent source-based sample-derived comparison may suggest specific additional current-tree IDs **only** after independent G1 proof of similarity features, predicate, exclusions and calibration. Same species/age or membership of the undivided area does not qualify. If G1 is absent, extension is **DISABLED**, all such targets remain Manual/NotCovered; manual-only full workflow is valid.
4. **Review choices.** Show exact IDs, source year/sample, player-authored versus suggested status, reasons, exclusion/match evidence and uncertainty. Player may accept/reject proposed IDs individually and manually select alternatives. No out-of-sample CropTree declaration without separate inspection. Candidates and proposals have **zero biology, finance or executed-history effect**.
5. **Create PendingWork.** For optional sample-derived treatment, a **separate prescription review/approval** creates or reconciles source-bound ordinary PendingWork orders only. For manual selections, an explicit reviewed **Add to Work Plan** action produces PendingWork directly; manual work does not require a fictional prescription approval. Duplicate and contradictory exact-ID orders must be reconciled explicitly before approval or fail closed; neither path can Fell.
6. **Work Plan approval: sole commitment gate.** Show consolidated pending manual/prescription work, exact current source/ID/revision, available execution path and truthful cost availability. Player must **explicitly approve the whole immutable batch**; editing already approved but unexecuted work requires visible warning, consent, cancellation/review and a new approval. Opening a view, clicking a candidate, adding an order or advancing a year never approves/executes.
7. **G2-gated actuation.** Separate explicit Execute runs a proven authoritative **whole-batch atomic** transaction in the bound current source/year. All IDs validated again at execution. One unique source-year + batch-revision + execution identity causes at most one real Fell per valid live ID, an actual Fell cause distinct from natural death, post-work state and exactly one committed result/event. Same-process identical-key replay returns already executed/no-op; changed payload or different key for dead/already-felled IDs refuses. Any invalid ID, stale source, conflict, exception or inability to commit result causes **zero partial Fell and zero history**. G2 must prove the write/rollback/event seam *before* coding.
8. **Real annual biology and review.** Explicit advance of the next ecological year consumes the actual post-Fell survivor roster, recalculates actual competition/growth/biological mortality and commits the genuine next-year source. Report “year y work executed” separately from “year y+1 ecology committed”, preserve year-specific read-only actual history and permit a fresh cycle or a no-work year. No preview, count relabel or fabricated next-year change qualifies.

### 3.1 Conceptual states (not an existing ABI)

- **READ_CURRENT** (admitted source only) → **DRAFT_MANUAL / DRAFT_SAMPLE** (player selections, no authoritative work).
- **PROPOSED_TREATMENT** (optional G1-proven suggestions, always hypothetical) → **PRESCRIPTION_REVIEWED / PRESCRIPTION_APPROVED** (optional for proposals; only reconciles pending orders).
- **PENDING_WORK** (manual “Add to Work Plan” or approved prescription), editable/cancellable by explicit action; **WORK_PLAN_APPROVED** immutable approved batch awaiting separate execution.
- **EXECUTED** is source-committed and its dead archive/history cannot be reversed; **FAILED_OR_REFUSED / CANCELLED** make no partial biological changes; **NO_WORK** creates no Fell. Stale edits invalidate pending/approval and require new current-source review. A future second management-area value is not instantiated here.

Screen-readable statuses must distinguish draft, proposed, pending, approved, executed and next-year committed without relying on colour. The exact event order and atomicity require G2 independent source qualification, not a UI-only state diagram.

## 4. Functional requirements — draft contract

- **FR-01 — Source:** one admitted committed Genesis/AfterMortality year, exact original fixture/model/generation and row/ID/precision forms each planning and execution binding.
- **FR-02 — Player judgement:** manual per-live-ID selection and optional representative same-source sample with explicit rationale; no imposed universally correct marking.
- **FR-03 — G0 one area:** the 30 m original stand is one **undivided management area**. No first-pass physical rack, entry, corridor, Confirm, clearing or derived rack order. Selecting whole-area scope never selects/marks/fells every tree. Future multi-area is separately specified.
- **FR-04 — G1 comparability:** only independent source-audited feature/predicate and accepted calibration may offer optional comparable-target proposals. Unknown/nonmatching = Manual/NotCovered, no automatic extension. Manual-only remains executable without G1.
- **FR-05 — Separation:** preview/candidate/proposal/sample and prescription alone never set Fell, commit biology or assert financial/history results.
- **FR-06 — Review/provenance:** source/area/sample/year/exact ID, player versus algorithm authorship, match/exclusion reasons, honest measurements and explicit per-ID overrides.
- **FR-07 — Order/approval gates:** manual selections deliberately enter ordinary PendingWork; proposals require separate prescription approval to become PendingWork. One consolidated Work Plan and one distinct **explicit approval** is the sole commitment gate; separate Execute is required.
- **FR-08 — Validation:** before prescription materialisation, Work Plan approval and actual execution, refuse untrusted/mismatched source/year/revision, wrong/dead/missing/case-shifted IDs, duplicates/conflicts, stale approvals and illegal action, with no partial effect.
- **FR-09 — Atomic once-only Fell (G2):** single engine transaction with actual Fell cause, roster/counts/BA and exact-year append-only result committed together. Same-process replay of identical execution key no-ops; changed key/payload collision refuses. Never invent persistent restart recovery.
- **FR-10 — Editing:** draft/pending changes explicit; already-approved unexecuted edits require visible warning, consent, cancel/re-review and new Work Plan approval; executed results irrevocable. No geometry edits because no rack exists.
- **FR-11 — Year:** execute current-year work only under approval; a separate next genuine annual growth/mortality step starts from actual survivors. No auto-thin on no-work/advance or weakened baseline.
- **FR-12 — History:** session-local, immutable, source-year linked actual management result and read-only previous-versus-current review. Distinguish sample/proposal/approved/actual and zero-work years; do not claim save/load.
- **FR-13 — Economics (G4):** **no new Stage 5 economics**. Only amounts already proven authoritative in Multi may appear; otherwise **UNAVAILABLE**, not zero/free. No price/cost/affordability gate or settlement invented.
- **FR-14 — Zero live/refusal:** final-survivor execution only if source G2 proves legal terminal behavior; no zero-denominator false metrics or resurrection. Otherwise explicit refusal/Manager policy question, no partial deletion.
- **FR-15 — Accessibility:** labelled keyboard/touch/screen reader selection, review, warning, Work Plan approval and execution states; 360 px/200% zoom, focus and errors, no color-only authority.
- **FR-16 — Non-scope:** no protected original/fixture/#784/#789/#805 changes, no Source/Host/save/H2, Unity, physical Ops, main merge, Reference Future/Legacy40 adaptation or production claim under this DRAFT.

## 5. Proposed data and execution contracts — source audit required

Conceptual records only, **not authorised API, persistent schema or implementation**:

| Record | Minimum source-linked fields and invariants |
|---|---|
| **ManagementScope** | Current V1 singleton “whole undivided 30 m stand”; future distinct area IDs may be accommodated in interface design but no area partition or physical rack exists today |
| **SampleRecord / ManualSelection** | experiment/fixture/model/current committed year/revision; exact selected live tree IDs, per-ID retain/release decision/rationale, actual evidence and qualification status; not an order |
| **ProposalRecord (optional G1)** | parent source/sample revision, itemised exact current target IDs, predicate/version, matching/exclusion/unknown evidence, user accept/reject; never authoritative Fell |
| **PendingWorkOrder** | exact current tree ID, action, explicit player/manual or approved-prescription provenance, source year, one management scope, no direct biological mutation |
| **WorkPlanRevision** | unique immutable batch/revision/source/year, de-duplicated reviewed order set, distinct explicit approval and unchanged-content identity; **no invented affordability check** |
| **ExecutedWorkEvent** | single source-authoritative atomic event identity, actual live→Fell transitions, death-cause distinction, before/after roster/count/BA where real, year and provenance; already-executed same-process identity and append-only history |

No proposed session-local key proves crash-durable semantics, Host authority, storage format or file locking. If G2 cannot prove a single atomic current-process writer plus rollback and genuine annual scheduling, **STOP** and return a narrowly evidenced design issue; neither a visual deletion nor a modelled “successful” operation is allowed.

## 6. Acceptance contract — all NOT RUN

- **AC-01** committed exact 100-tree source and all protected original hashes unaffected by drafts/proposals/no-work.
- **AC-02** #805 local REMOVE_CANDIDATE never has work authority, even after Undo/Discard or stale callbacks.
- **AC-03** whole original stand shows **one undivided management area**, without rack/Confirm/corridor/clearance controls or implicit all-ID treatment.
- **AC-04** player can select valid live individual IDs manually, optionally demonstrate qualified local source sample, and inspect retain/release rationale.
- **AC-05** missing G1 comparator disables automatic proposal extension; manual source-backed orders work independently and no undivided-area blanket proposal occurs.
- **AC-06** unapproved candidates/prescriptions/pending orders produce zero Fell; only explicit current Work Plan approval followed by separate G2 execution may cause work.
- **AC-07** no duplicate/contradictory/stale/dead/case-mismatched source IDs pass the whole-batch validator; refusal changes zero biological or history state.
- **AC-08** one real source-authoritative atomic Fell/current-year event, identical same-process replay no-op, different-key/payload replay refused, natural mortality distinct.
- **AC-09** after actual Fell, next *genuine* year recomputes ecology from real survivors; unchanged no-work Year0–10 and zero-live policies witnessed independently.
- **AC-10** approved-but-unexecuted edits warn and require consent/reapproval; executed trees/history immutable, session-local review is read-only, no save durability claim.
- **AC-11** all source/browser/accessibility, positive and negative tests independently witnessed against a fixed candidate; no L4 waiver, engine or fixture rewrite.
- **AC-12** focused nonauthor draft delta review, #730 formal spec approval, G1 if comparison enabled, G2 source/atomicity proof, actual live ownership/physical permits and independent feature qualification **before** any implementation, integration or acceptance claim.

Detailed **26 R / 24 X**, all initially unchecked, are in [requirements checklist](checklists/requirements.md). No case has been run or passed by #817.

## 7. Decided boundaries, open technical gates and stops

- **G0 — RESOLVED by latest owner:** one undivided management area, **no rack geometry/mechanism** for current synthetic stand. Larger stand/multiple management areas and possible actual extraction racks are a **future separately approved scope**, not a code dependency now.
- **G1 — OPEN, optional:** independent source-backed similarity features, comparator rules and calibration. Without qualified proof, no transfer proposals. Manual selection/Work Plan/G2 execution remain the complete first-pass route. Same Sitka species and age do not suffice.
- **G2 — OPEN, mandatory before source coding:** identify current authoritative engine Fell legal write/cause, atomic whole-batch + event commit and rollback, same-process exactly-once identity, precise year ordering, next genuine annual biology, zero-live behavior and no-work oracle protection. Require independent review. If unavailable, report BLOCKED; no shortcut.
- **G3 — DECIDED / DEFERRED TO STAGE 7:** no save/resume, Host/H2, localStorage authority, migration or crash/restart replay promises in Stage 5. Within current process/year only. Stage 7 needs separate security/provenance/journal and source authority.
- **G4 — DECIDED / DEFERRED:** no new financial/economics model. Unsupported amounts displayed as **UNAVAILABLE** and not assumed zero, free, budgeted or settled.
- **Dependencies:** #805 read contract and Stage 4 independently accepted candidate; #789 actual unchanged baseline; independent G2 proof; approved spec; exact source ownership/GitHub and physical Ops/lease gate before prospective native implementation.
- **STOP** on stale/unknown source, unproven G2, assumed G1, invalid per-tree eligibility, duplicate work identity, ownership conflict, missing physical permit, unapproved save/economy, need to weaken protected original/no-work or any attempt to add rack geometry to first pass.

**#817 delivers a CORRECTED DRAFT, not an accepted specification, implemented feature, tested forestry operation or source/Unity release.**
