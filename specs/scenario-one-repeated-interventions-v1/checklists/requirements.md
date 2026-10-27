# Scenario One Second and Subsequent Interventions (S1-D) Requirements / Acceptance Checklist

- **Spec ID:** SPEC-scenario-one-repeated-interventions-V1
- **Spec status:** DRAFT (the checklist applies once status is APPROVED/IMPLEMENTING/VERIFYING/VERIFIED)
- **Checklist owner:** Overall Manager; independent reviewer to be assigned (T-09)
- **Implementation under test:** none — no implementation exists; every item below is **NOT STARTED**
- **Date:** 2026-10-27

Use this checklist against the finished implementation. A checked item needs evidence, not assertion. Unchecked items with a stated reason are honest; unexplained gaps are not. Nothing in this draft was run in Unity; no box may be ticked from this draft's static reading alone.

## Pre-conditions

- [ ] `OPEN-1` decided and recorded canonically (baseline lineage and status of candidate S1-C/S1-D code) — evidence:
- [ ] All blocking decisions resolved or the dependent requirement removed: `OPEN-1`, `OPEN-4`, `OPEN-7` — evidence:
- [ ] Spec status `APPROVED` by the Manager — evidence:
- [ ] T-01 baseline table attached, with the real implementation SHA replacing `c6342f7` — evidence:
- [ ] S1-C draft reconciled with spec §9 (EI-1..EI-7) — evidence:

## Functional requirements

### A. Re-entry and action availability
- [ ] FR-A1: further cycles in the same save with nothing lost or reset — evidence:
- [ ] FR-A2: every first-cycle management action works in later years with no lesson/`PlayerPrefs`/session state — evidence:
- [ ] FR-A3: no teaching rule blocks Approve/Advance after its lesson (or the permanent rule is explained) — evidence:

### B. Progression state
- [ ] FR-B1: guide does not regress on Crop Tree count change, later in-footprint felling, executed-share drift or load — evidence:
- [ ] FR-B2: no saved ID renamed or reused; new IDs only — evidence (ID diff):
- [ ] FR-B3: no `PlayerPrefs` authority for cycle/progression — evidence:

### C. Previous work versus current proposals
- [ ] FR-C1: Executed / Approved / Pending / Pending carried over / unplanned Fell mark are distinguishable — evidence:
- [ ] FR-C2: carried-over orders revalidated and surfaced; never auto-cancelled or auto-approved — evidence:
- [ ] FR-C3: executed work never shown as a current proposal; failures keep their reason — evidence:
- [ ] FR-C4: no provenance claimed where none is persisted — evidence:
- [ ] FR-C5: earlier interventions locatable spatially (as decided under `OPEN-6`) — evidence:

### D. Cycle state and readiness
- [ ] FR-D1: derived explainable cycle state from persisted records only — evidence:
- [ ] FR-D2: no instruction to intervene, no universal interval, waiting remains legitimate — evidence:
- [ ] FR-D3: advisory unless a gate was explicitly decided — evidence:

### E. Completion
- [ ] FR-E1: completion remains reachable after any number of cycles; no teaching-only state required — evidence:
- [ ] FR-E2: objective predicates, D-057 targets, horizons and the N3 normalised-source hash unchanged — evidence (N3 offline output):
- [ ] FR-E3: ambiguous event IDs rejected or repaired, never silently un-completable — evidence:
- [ ] FR-E4: minimum-cycles rule as decided under `OPEN-8` — evidence:

### F. Save and history re-entry
- [ ] FR-F1: state, history and next-step text reproduced after save/load; reading writes nothing — evidence:
- [ ] FR-F2: any new field uses the next free schema version, has a default, fabricates nothing — evidence or N/A reason:
- [ ] FR-F3: Reference Future v1 and Legacy40 anchors unchanged — evidence:

### G. Neighbouring work
- [ ] FR-G1: MFM-generated orders treated as ordinary orders; no dependency on MFM — evidence:
- [ ] FR-G2: Objective 1 rack workflow does not repeat; rack availability as decided under `OPEN-9` — evidence:
- [ ] FR-G3: no Sitka-only assumption or species coefficient added — evidence:

### H. Compatibility
- [ ] FR-H1: Legacy40 and Reference keep their viable path; scope per `OPEN-10` — evidence:
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

## Save / backward compatibility

- [ ] Save impact matches spec §14 (none by default; conditional fields only if approved).
- [ ] Old saves, missing fields and Reference Future behaviour are correct.
- [ ] No unapproved migration or schema change; world-hash compatibility path preserved if a field was added.

If not applicable, state why:

## Security / write-back

- [ ] Load validation rejects without world or disk mutation (CanLoad/LoadData parity) for the event-ID cases.

If not applicable, state why:

## Acceptance criteria

- [ ] AC-1: three real cycles, idle years between, all FR-A2 actions work in cycle 3 — evidence (Unity gate):
- [ ] AC-2: Approve/Advance not blocked by the teaching footprint rule outside `OPEN-5`'s decision; message complete when blocked — evidence:
- [ ] AC-3: guide stable under the four regression triggers — evidence:
- [ ] AC-4: same save loaded twice gives identical history, cycle state and text; reads write nothing — evidence:
- [ ] AC-5: five work classes visibly distinct; no unpersisted provenance stated — evidence (captures at 1280×720 and 1920×1080):
- [ ] AC-6: ≥3-cycle path completes and stays solvent; predicates, targets and N3 hash unchanged; D-057 witness re-run — evidence:
- [ ] AC-7: duplicate or non-positive event IDs handled without silent un-completability — evidence:
- [ ] AC-8: protected anchors and Reference contract unchanged; no unapproved schema field — evidence:
- [ ] AC-9: readiness never blocks play or instructs on a timetable — evidence:
- [ ] AC-10: independent review finds no hidden automation and no causal overclaim — evidence:

## Spec conformance

- [ ] Implementation matches the approved spec.
- [ ] Approved deviations are recorded in `spec.md` and the hand-off.
- [ ] Unresolved requirements are listed, not hidden.
- [ ] The S1-D completion claim is made only against these acceptance criteria; MFM or S1-C completion is not accepted as a substitute (D-065, D-060).

## Verdict

- **Spec conformance:** NOT VERIFIED (no implementation)
- **Implementation correctness:** NOT VERIFIED (no implementation)
- **Notes:** Draft checklist only. Static reading of `c6342f7` informed the spec; it is not verification of any requirement.
