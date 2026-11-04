# PLAYER INPUT SETTINGS AND FEEDBACK V1 — Tasks

- **Spec ID:** `PLAYER-INPUT-SETTINGS-AND-FEEDBACK-V1`
- **Status:** **DRAFT** — task list for a DRAFT spec. **Nothing here is authorised or started.** Tasks become claimable only after the spec is `APPROVED`, the relevant §5 questions are answered, and the Manager creates task packets.
>
> **Authority and stop rules.** Source precedence is `spec.md` §8 (fixed keys > valid user setting, which includes an applied import > scene/prefab serialized value > built-in default; a game save never supplies input settings). Every task is bound by the stop-and-ask conditions in `spec.md` §9: in particular, **if a task would require any change to `ForestSaveData`, `forest-save.json`, `CurrentVersion` or save/load code (S-1), stop and ask the Manager before editing.** So too for any package, API-assumption failure, fixed-key change, unlisted consumer, or network/identity code (S-2…S-6).
- **Last updated:** 2026-11-04

All tasks are `NOT STARTED`. Estimates are rough and unmeasured. Verification references use the AT rows in `spec.md` §6.

## M0 — Decisions (Manager/user)

- [ ] **IPS-000** Answer open questions Q1–Q13; record accepted decisions in the Decision Log; set spec status. *Owner: Manager/user.*
- [ ] **IPS-001** Independent review of this DRAFT by a different provider family (required by #257 comment). *Owner: Manager.*

## M1 — Persistent sensitivity (no save-schema change)

- [ ] **IPS-010** Decide settings storage (Q2) and write a one-paragraph design note. Docs.
- [ ] **IPS-011** Spike (Unity session): are UI Toolkit buttons/sliders keyboard-focusable in `UiKit`; what does Tab do; can settings open while the mouse is locked. Output: measured findings. AT-14 precursor.
- [ ] **IPS-012** Plain settings class: load/validate/save sensitivity with version, atomic write, `.bak`, defaults on failure. Offline check for corrupt/missing file. AT-17/18/19.
- [ ] **IPS-013** `ForestPlayer` reads the setting (serialized value stays the default; keep reflection field or update `CameraLookDiagnosis`). AT-01/27.
- [ ] **IPS-014** Settings page from F10 session menu: slider with keyboard steps, numeric readout, reset. AT-01/02/14.
- [ ] **IPS-015** Persistence checks across restart, F5/F9, New Scenario. AT-02/03/04.

## M2 — Rebinding core

- [ ] **IPS-020a** Obtain the reviewer's exact "control-access" (Method B) command and append it to `spec.md` O-7a so 73/61/16 on `72b4efbd` is reproducible. *Owner: Manager/reviewer.*
- [ ] **IPS-020** Re-inventory every key consumer against the actual base commit (main vs `72b4efbd`; spec O-7, O-7a, O-12) by re-running the O-7a Method A command and recording its output. Docs table. Include the candidate-only P and N consumers if the base is the candidate.
- [ ] **IPS-021** Create CCF action asset + thin accessor; no behaviour change (defaults equal current keys).
- [ ] **IPS-022** Migrate first subset: movement, run, jump, interact, X, C. AT-22.
- [ ] **IPS-023** Context layer for the migrated actions; binding-capture modal. AT-12/13.
- [ ] **IPS-024** Conflict detection, unbind rules, reset, and the load-time deterministic conflict rule (REQ-P3a, incl. the total required-action rule: required-vs-required resolved by table order, else whole set rejected; one function shared with import). AT-05/06/07/08/17a/17b.
- [ ] **IPS-025** Live help labels from bindings; update `ControlsReference` consumers, the separate `ScenarioOneTreatmentReview` `[C]`/`[N]` hint constants (candidate) and offline UX assertions. AT-20/28.
- [ ] **IPS-026** Settings page: rebinding list, keyboard-only operation. AT-14.

## M3 — Multiple bindings, import/export

- [ ] **IPS-030** Multiple bindings per action in model and UI. AT-11.
- [ ] **IPS-031** Export/import with validation, all-or-nothing, version field. AT-09/10.
- [ ] **IPS-032** Device-change and missing-binding handling. AT-16/17.

## M4 — Remaining consumers

- [ ] **IPS-040** Migrate G, U, R, F, B/RackTool, P and N (candidate only; N is contextual to the inspect card, AT-22a), V, J, K, Map Delete/Backspace/Shift, saw pit, storage, buildable, competitor, ecology. Regression AT-22/28.
- [ ] **IPS-041** Non-QWERTY test and decision implementation (Q6). AT-21.

## M5 — Optional: gamepad UI navigation (only if Q3 says so)

- [ ] **IPS-050** Gamepad focus navigation of settings. AT-15.

## M6 — Optional: opt-in local summary (only if Q9 says so)

- [ ] **IPS-060** User-approved consent copy. *Owner: user/Manager.*
- [ ] **IPS-061** Local summary writer limited to REQ-F2 fields; off by default. AT-23.
- [ ] **IPS-062** View/export/delete UI; no identity; no network. AT-24/25.
- [ ] **IPS-063** Reporting guide with statistics caveats (REQ-X1–X3). AT-26.

## Out of scope for these tasks

Save-schema change, telemetry, gamepad gameplay, Map keyboard editing (separate spec, Q8), Esc architecture rewrite, new packages.
