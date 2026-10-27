# Scenario One Second and Subsequent Interventions (S1-D) Requirements / Acceptance Checklist

- **Spec ID:** SPEC-scenario-one-repeated-interventions-V1
- **Spec status:** **APPROVED** (Manager review `457461b`; amendment packet `7716201`; decisions RD-1…RD-12 in spec §12)
- **Checklist owner:** Overall Manager; independent reviewer to be assigned (T-09)
- **Implementation under test:** none — no implementation exists; every implementation item below is **NOT STARTED**
- **Date:** 2026-10-27

Use this checklist against the finished implementation. A checked item needs evidence, not assertion. Unchecked items with a stated reason are honest; unexplained gaps are not. Nothing in the approved spec was run in Unity; no implementation box may be ticked from the spec's static reading alone.

## Pre-conditions

- [x] Decisions `OPEN-1`…`OPEN-11` resolved and recorded as RD-1…RD-11 — evidence: `MANAGER-REVIEW.md` @ `457461b`, spec §12.
- [x] Management-event-ID policy resolved (RD-12: reject before mutation, no repair) — evidence: `MANAGER-REVIEW.md` "Additional Manager decision"; spec FR-E3.
- [x] Spec status `APPROVED` by the Manager — evidence: spec header; `AMENDMENT-PACKET.md` @ `7716201`.
- [x] S1-D §9 reconciled to the accepted S1-C contract boundaries (EI-1..EI-7) — evidence: spec §9.1–§9.4; amendment packet §2.
- [ ] T-01 baseline table attached, with the real integrated implementation SHA replacing `c6342f7` — evidence:
- [ ] The S1-C contract is `APPROVED` on the integration base — evidence:
- [ ] Exactly one derived-history implementation confirmed on the integration base — evidence (T-01, AC-11):

## Resolved decisions (RD-1…RD-12)

- [ ] RD-1: the `c6342f7` derived history, two-cycle rule, completion guidance and sequential guide are treated as a partial implementation baseline, not feature completion — evidence (T-01 finding):
- [ ] RD-2: later cycles are bounded hybrid — ordinary player-directed management with advisory guidance; no per-cycle prescriptive objective — evidence:
- [ ] RD-3: a later Annual Review acknowledgement is advisory and never blocks Approve/Advance — evidence:
- [ ] RD-4: readiness is an advisory component/reason state; no gate, no timetable, no minimum interval — evidence:
- [ ] RD-5: the 40 % teaching-footprint cap is hard only during its owning activity and advisory/non-blocking afterwards — evidence:
- [ ] RD-6: previous/new provenance derives from existing orders/events/history; no Crop Tree designation-year field and no dated past-work map layer — evidence:
- [ ] RD-7: the guide derives from persisted history; no new save progress field; no `PlayerPrefs` or session `[J]/[K]` authority after historical completion — evidence:
- [ ] RD-8: the two-reviewed-cycle completion minimum is unchanged; management may continue after `Completed` — evidence (N3 hash + witness):
- [ ] RD-9: rack reopening/additional-rack authoring is deferred to Rack V2; Objective 1 rack teaching does not automatically repeat — evidence:
- [ ] RD-10: geometry-independent features may work for Legacy40 where records exist; no Enlarged80-only migration; Reference Future v1 frozen — evidence:
- [ ] RD-11: no second MFM prescription in S1-D V1; MFM orders are ordinary orders — evidence:
- [ ] RD-12: duplicate/non-positive management-event IDs are rejected before mutation, without repair — evidence (negative fixtures + STOP check):

## Functional requirements

### A. Re-entry and action availability
- [ ] FR-A1: further cycles in the same save with nothing lost or reset — evidence:
- [ ] FR-A2: every first-cycle management action works in later years with no lesson/`PlayerPrefs`/session state — evidence:
- [ ] FR-A3: the 40 % rule is hard only during its owning activity; afterwards it warns but does not disable Approve/Advance — evidence:

### B. Progression state
- [ ] FR-B1: guide does not regress on Crop Tree count change, later in-footprint felling, executed-share drift or load; no invented latch — evidence:
- [ ] FR-B2: no saved ID renamed or reused; new IDs only — evidence (ID diff):
- [ ] FR-B3: no `PlayerPrefs` authority for cycle/progression; no `[J]/[K]` persistence — evidence:

### C. Previous work versus current proposals
- [ ] FR-C1: Executed / Approved / Pending / Pending carried over / unplanned Fell mark are distinguishable — evidence:
- [ ] FR-C2: carried-over orders revalidated and surfaced; never auto-cancelled or auto-approved — evidence:
- [ ] FR-C3: executed work never shown as a current proposal; failures keep their reason — evidence:
- [ ] FR-C4: no designation time stated or implied where none is persisted — evidence:
- [ ] FR-C5: no Crop Tree designation-year field and no dated past-work map layer; spatial review only via existing S1-C records where locations exist — evidence:

### D. Cycle state and readiness
- [ ] FR-D1: derived, explainable, advisory cycle state from persisted records only, naming components and reasons — evidence:
- [ ] FR-D2: no instruction to intervene, no universal interval, waiting remains legitimate — evidence:
- [ ] FR-D3: an outstanding later acknowledgement is surfaced but never blocks Approve/Advance; the first-advance gate is unchanged — evidence:
- [ ] FR-D4: later interventions are ordinary player-directed management; no per-cycle objective or implied single correct treatment — evidence:

### E. Completion
- [ ] FR-E1: completion remains reachable after any number of cycles; no teaching-only state required — evidence:
- [ ] FR-E2: objective predicates, D-057 targets, horizons and the N3 normalised-source hash unchanged — evidence (N3 offline output):
- [ ] FR-E3: duplicate or non-positive event IDs are rejected before any world/disk mutation, with a clear message, and never repaired or renumbered; ambiguous shipped fixture triggers STOP — evidence:
- [ ] FR-E4: two reviewed successful intervention cycles in distinct years remain the minimum; no increase; continued management after `Completed` — evidence:

### F. Save and history re-entry
- [ ] FR-F1: state, history and next-step text reproduced after save/load; reading writes nothing — evidence:
- [ ] FR-F2: S1-D V1 adds no persisted field; no fixed future version named — evidence:
- [ ] FR-F3: Reference Future v1 and Legacy40 anchors unchanged — evidence:

### G. Neighbouring work
- [ ] FR-G1: MFM-generated orders treated as ordinary orders; no MFM dependency; no second prescription — evidence:
- [ ] FR-G2: Objective 1 rack workflow does not repeat; rack reopening deferred to Rack V2 — evidence:
- [ ] FR-G3: no Sitka-only assumption or species coefficient added — evidence:
- [ ] FR-G4: S1-C history contract consumed; no competing store, identity scheme or duplicated UI — evidence:

### H. Compatibility
- [ ] FR-H1: Legacy40 works for geometry-independent features where records exist; no Enlarged80-only migration; Reference frozen — evidence:
- [ ] FR-H2: pre-feature Enlarged80 saves load with no fabricated state — evidence:

## Player / user workflow

- [ ] The repeat cycle (spec §8.1) is reachable and understandable for cycles 2, 3 and later.
- [ ] Required feedback, affordances and failure/empty/edge states behave as specified: idle year, all tasks failed, stale order, unapproved plan at advance, cash exactly zero.
- [ ] The implementation does not add unapproved player-facing behaviour (no scoring, no prescribed second treatment, no interval).

## Authority and evidence

- [ ] Implementation follows the approved spec and plan.
- [ ] Evidence classifications in spec §13 remain accurate (no new ecological claim).
- [ ] No unlabelled pseudo-empirical coefficients or invented ecological facts were introduced.
- [ ] Every history or guide sentence avoids claiming causation (`ManagementHistoryText.NotCausalNote` respected).
- [ ] All protected behaviour in spec §10.1 is unchanged.

## Save / backward compatibility

- [ ] Save impact matches spec §14: no persisted field and no schema change in V1.
- [ ] Old saves, missing fields and Reference Future behaviour are correct; no fabricated history or provenance.
- [ ] No unapproved migration or schema change; world-hash compatibility path preserved.

If not applicable, state why:

## Security / write-back

- [ ] Load validation rejects without world or disk mutation (CanLoad/LoadData parity) for the event-ID cases.

If not applicable, state why:

## Acceptance criteria

- [ ] AC-1: three real cycles, idle years between, all FR-A2 actions work in cycle 3 — evidence (Unity gate):
- [ ] AC-2: after the owning activity, exceeded footprint shows an advisory and Approve/Advance remain enabled; during the activity the hard constraint and its complete message remain — evidence:
- [ ] AC-3: guide stable under Crop Tree count change, Crop Tree loss, a second in-footprint felling year, executed-share drift and save/load at lessons 6–8; in-lesson behaviour unchanged — evidence:
- [ ] AC-4: same save loaded twice gives identical history, cycle state and text; reads write nothing — evidence:
- [ ] AC-5: five work classes visibly distinct; no unpersisted provenance stated; no dated map layer — evidence (captures at 1280×720 and 1920×1080):
- [ ] AC-6: ≥3-cycle path completes and stays solvent; predicates, targets and N3 hash unchanged; D-057 witness re-run — evidence:
- [ ] AC-7: duplicate or non-positive event IDs rejected before mutation, not repaired; CanLoad/LoadData agree; STOP rule exercised — evidence:
- [ ] AC-8: protected anchors and Reference contract unchanged; no persisted field added — evidence:
- [ ] AC-9: readiness reports components/reasons, never blocks play or instructs on a timetable; unreviewed state does not disable Approve/Advance — evidence:
- [ ] AC-10: independent review finds no hidden automation and no causal overclaim — evidence:
- [ ] AC-11: exactly one derived-history implementation; no second store, identity scheme or duplicated history UI; adapter read-only and `UnityEngine`-free — evidence:
- [ ] AC-12: Objective 1 rack teaching does not re-trigger; no rack-reopening behaviour added — evidence:
- [ ] AC-13: Legacy40 derived presentation works where records exist; no Enlarged80-only feature appears; Reference unaffected — evidence:
- [ ] AC-14: after `Completed`, management continues to the existing horizon/terminal condition; Work Plan not locked — evidence:
- [ ] AC-15: MFM-generated orders behave as ordinary orders; no second-prescription logic — evidence:

## Spec conformance

- [ ] Implementation matches the approved spec.
- [ ] Approved deviations are recorded in `spec.md` and the hand-off.
- [ ] Unresolved requirements are listed, not hidden.
- [ ] The S1-D completion claim is made only against these acceptance criteria; MFM or S1-C completion is not accepted as a substitute (D-065, D-060).

## Verdict

- **Spec conformance:** NOT VERIFIED (no implementation)
- **Implementation correctness:** NOT VERIFIED (no implementation)
- **Notes:** The spec package is APPROVED. Static reading of `c6342f7` informed the spec; it is not verification of any requirement, and all implementation items remain NOT STARTED.
