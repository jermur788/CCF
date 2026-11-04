# PLAYER INPUT SETTINGS AND FEEDBACK V1 — Requirements Checklist

- **Spec ID:** `PLAYER-INPUT-SETTINGS-AND-FEEDBACK-V1`
- **Status:** **DRAFT** traceability checklist
- **Last updated:** 2026-11-04

All boxes are unchecked: spec drafting is not implementation evidence. Nothing has been built or run.

## Constraints

- [ ] **CON-001** No production code, UI, scene, prefab or `.inputactions` change from this draft. *Spec §4.1*
- [ ] **CON-002** No telemetry, network, background collection or claim of consent. *§3.6, §4.2*
- [ ] **CON-003** No save-schema or `forest-save.json` change; `CurrentVersion` unchanged. *REQ-S5, §4.3. Any change needs separate approval.*
- [ ] **CON-004** New Input System only; no legacy `UnityEngine.Input`; no new package. *§4.4, O-6*

## Sensitivity

- [ ] **REQ-S1** Setting with range, reset, preview — AT-01
- [ ] **REQ-S2** Optional vertical/invert (should) — AT-01
- [ ] **REQ-S3** `ForestPlayer` uses setting; serialized value is default — AT-01, AT-27
- [ ] **REQ-S4** Scope decided (per-device recommended) — Q1
- [ ] **REQ-S5** No save change — AT-03

## Rebinding

- [ ] **REQ-B1** Action layer using built-in Input System — AT-22
- [ ] **REQ-B2** Curated rebindable list; fixed keys decided — Q4
- [ ] **REQ-B3** Conflict detection — AT-05, AT-06
- [ ] **REQ-B4** Unbinding with last-binding guard — AT-07
- [ ] **REQ-B5** Reset (action/group/all) — AT-08
- [ ] **REQ-B6** Validated import/export, all-or-nothing — AT-09, AT-10
- [ ] **REQ-B7** Help reflects live bindings — AT-20, AT-28
- [ ] **REQ-B8** Multiple bindings — AT-11

## Contexts

- [ ] **REQ-C1** Context priority defined — AT-22
- [ ] **REQ-C2** Same control in exclusive contexts only — AT-06
- [ ] **REQ-C3** Capture mode modal — AT-12
- [ ] **REQ-C4** Text-entry suppression — AT-13

## Accessibility

- [ ] **REQ-A1** Keyboard-only (and gamepad if built) settings — AT-14, AT-15
- [ ] **REQ-A2** Keyboard-adjustable sensitivity — AT-14
- [ ] **REQ-A3** Capture/conflict text feedback — AT-12
- [ ] **REQ-A4** Hold/toggle (could)
- [ ] **REQ-A5** Map pointer-only gap recorded, not claimed fixed — Q8
- [ ] **REQ-A6** Gamepad gameplay out of V1 unless decided — Q3

## Preservation / failure

- [ ] **REQ-P1** Survives restart, save/load, new scenario, scene switch — AT-02..04
- [ ] **REQ-P2** Device change safe — AT-16
- [ ] **REQ-P3** Missing/invalid binding handled — AT-17, AT-18
- [ ] **REQ-P4** Settings schema version and forward-compat — AT-19
- [ ] **REQ-P5** Layout handling decided — AT-21
- [ ] **REQ-P6** Atomic settings write — AT-17

## Opt-in local summary (only if approved)

- [ ] **REQ-F1** Off by default, reversible — AT-23
- [ ] **REQ-F2** Limited field list — AT-24
- [ ] **REQ-F3** No identity — AT-24
- [ ] **REQ-F4** No automatic transmission — AT-25
- [ ] **REQ-F5** View/export/delete — AT-24
- [ ] **REQ-F6** Aggregation tool external and separate
- [ ] **REQ-F7** Approved consent wording — Q9

## Statistics honesty

- [ ] **REQ-X1** No "optimal" claims from convenience sample — AT-26
- [ ] **REQ-X2** N and selection always stated — AT-26
- [ ] **REQ-X3** Hardware confounds acknowledged — AT-26

## Governance

- [ ] **GOV-001** Spec approved and Decision Log updated before any task starts
- [ ] **GOV-002** Independent different-provider review done (not self-approved)
- [ ] **GOV-003** Open questions Q1–Q12 answered or explicitly deferred
