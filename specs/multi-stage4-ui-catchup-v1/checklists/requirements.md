# Multi Stage 1–4 `/multi` UI Catch-up Requirements / Acceptance Checklist

- **Spec ID:** SPEC-multi-stage4-ui-catchup-V1
- **Spec status:** APPROVED
- **Checklist owner:** Implementation sub-agent; independent reviewer to re-check
- **Implementation under test:** `feat/multi-stage4-ui-catchup-20261123-v1` at code HEAD `541bb898`
- **Date:** 2026-11-23

A checked item needs evidence, not assertion. The evidence column is filled in the implementation handoff (`Docs/Handoffs/MultiRack-Simulation-20261116/900-ui-catchup/MULTI-STAGE4-UI-CATCHUP-IMPLEMENTATION-RESULT-20261123.md`).

## Functional requirements

- [ ] FR-1: `/multi` (via `OpenSynthetic()`) runs 1/1/1; 0/0/0 reachable from Core and tested; `new MultiAreaContinuation(e)` stays model 0 — evidence:
- [ ] FR-2: additive `stage4` payload with models/metadata, per-tree life state + pruning eligibility/reason, per-rack canopy summary + deadwood, per-rack 2.5 m ground cells — evidence:
- [ ] FR-3: Prune target from the engine (Unity Sitka schedule 2.5 / 5 / 6.5 m); client sends none — evidence:
- [ ] FR-4: prune add/remove routes follow the Fell pattern behind unchanged guards; Pending / Approve / Execute semantics — evidence:
- [ ] FR-5: no forestry/canopy/light/RD/eligibility arithmetic in page JS — evidence:
- [ ] FR-6: fallen-log symbols at engine coordinates + legend beside D and X — evidence:
- [ ] FR-7: non-colour cue for every new state — evidence:
- [ ] FR-8: no horizontal scroll at 1920×1080 / 1280×720 in all four legend × Light Guide states; stand clear of dock at Fit — evidence:
- [ ] FR-9: execute-time Prune revalidation, whole-plan atomic refusal test — evidence:

## Player / user workflow

- [ ] Load, Fell, Prune, Light Guide and Year+1 workflows reachable and understandable.
- [ ] Refusals show the engine code and reason; nothing changes on refusal.
- [ ] No unapproved player-facing behaviour (no twin view, no Stage 5 content).

## Authority and evidence

- [ ] Implementation follows the approved spec and plan.
- [ ] Evidence classifications stay accurate (pruning = gameplay calibration; light = proxy, not PAR; synthetic stands).
- [ ] No invented ecological claims.

## Save / backward compatibility

- [ ] No save impact; all frozen fingerprints and the P0 pin unchanged.

## Security / write-back

- [ ] New routes pass the unchanged `PlayerApi` guards; strict member parsing; fenced; 409 fresh view; fail closed.

## Acceptance criteria

- [ ] AC-1 builds 0/0 — evidence:
- [ ] AC-2 Core ≥ 181 / 0 — evidence:
- [ ] AC-3 five anchors unchanged, P0 before/after — evidence:
- [ ] AC-4 UI smoke no regression; J01/J99 triaged — evidence:
- [ ] AC-5 real Edge: loads 1/1/1, 0 console errors / CSP / 5xx, layout checks — evidence:
- [ ] AC-6 Prune gate end to end — evidence:
- [ ] AC-7 Fell + light + years — evidence:
- [ ] AC-8 cross-rack — evidence:
- [ ] AC-9 eight screenshots within size limits, no token — evidence:

## Spec conformance

- [ ] Implementation matches the approved spec.
- [ ] Presentation choices recorded in `spec.md` "Open product decisions" and the handoff.
- [ ] Unresolved requirements listed, not hidden.

## Verdict

- **Spec conformance:** PASS (author self-assessment; AC-4 and AC-9 with the recorded exceptions)
- **Implementation correctness:** PASS (author, Tier 2: builds 0/0, Core 184/184, five anchors unchanged, real Edge 73/73)
- **Notes:** item-by-item evidence is in `Docs/Handoffs/MultiRack-Simulation-20261116/900-ui-catchup/MULTI-STAGE4-UI-CATCHUP-IMPLEMENTATION-RESULT-20261123.md` §6–§11. Boxes stay unticked here until an independent reviewer re-checks them; this file is not a second-party verdict.
