# PLAYER INPUT SETTINGS AND FEEDBACK V1 — Plan

- **Spec ID:** `PLAYER-INPUT-SETTINGS-AND-FEEDBACK-V1`
- **Status:** **DRAFT** — derived from a DRAFT spec; authorises nothing. Do not start implementation until the spec is `APPROVED` and the decisions in `spec.md` §5 (at least Q1, Q2, Q4, Q5, Q7, Q13) are answered. Authority hierarchy: `spec.md` §8. Implementers must follow the stop-and-ask conditions S-1…S-8 in `spec.md` §9 (notably any save-schema need).
- **Last updated:** 2026-11-04
- **Evidence base:** read-only inspection of `origin/main` `8f0f2301` [M] and candidate `72b4efbd` [C]; citation convention in `spec.md` header (bare `file:line` = [M]). No Unity or runtime was available; every effort/risk figure below is an estimate, not a measurement.

## 1. Architecture direction (PROPOSAL)

1. **Keep settings separate from the game save.** Settings live in their own versioned file (or `PlayerPrefs` for the first slice) in `Application.persistentDataPath`; `ForestSaveData` is untouched (spec REQ-S4/S5). Any change to this needs separate save-policy approval.
2. **Use the Input System's built-in actions and rebinding**, not a custom framework: one `InputActionAsset` dedicated to CCF gameplay (new asset; the template `InputSystem_Actions.inputactions` is unused by code, spec O-8), runtime binding overrides saved as JSON via the Input System's override APIs. Exact API names/versions must be confirmed against the installed package (UNVERIFIED).
3. **One small, plain C# settings class** (learner-maintainable, no singleton framework): loads/validates/saves settings, exposes `MouseSensitivity` and "action → bindings". `ForestPlayer` reads it with the serialized value as default.
4. **Migrate consumers incrementally behind a thin accessor** (e.g. `Controls.WasPressed(ControlAction.Interact)`) so each file changes one call at a time and the legacy hard-coded keys remain the default bindings until a file is migrated. Do not bulk-rewrite all consumer files (14 on [M], 16 on [C]; `spec.md` O-7a) in one change.
5. **Context layer** (spec §3.3) is introduced only when the first overlapping binding is actually made rebindable; do not build a general input-stack framework speculatively.
6. **Help text** continues to come from `ControlsReference`, but its labels are generated from the live bindings with the current strings as fallback; offline checks updated alongside.
7. **Opt-in summary** is a late, separate slice reading from the settings class, writing only the REQ-F2 fields to a local file; no network code is added anywhere.

## 2. Slices (each independently shippable; each needs its own approval)

| Slice | Content | Touches | Save schema? | Risk |
|---|---|---|---|---|
| **M0 — Decisions** | Answer spec §5 questions; approve spec | docs only | no | none |
| **M1 — Sensitivity** | Settings class (sensitivity only), minimal settings page reachable from F10 session menu, keyboard-operable slider, reset, persistence, `ForestPlayer` reads it | `ForestPlayer.cs` (1 line + field role), session menu UI, new settings file/class, `CameraLookDiagnosis` check | **no** | low–medium: field is reflected by a verification tool; new UI must be keyboard-focusable (UNVERIFIED capability) |
| **M2 — Rebinding core** | Action asset; accessor; migrate a subset (movement, run, jump, interact, X, C, M, Tab, O) first; capture mode; conflict detection; unbind; reset; live help labels | ~10 consumer files (subset; N/P candidate-only keys are M4) incl. `ForestPlayer`, `ScenarioOneUiRoot`, `ScenarioOneManager`, `ControlsReference`; offline UX checks | no | **high**: Esc/overlay ordering (spec O-13), Tab double use, label assertions |
| **M3 — Multi-binding + import/export** | Multiple bindings, validated import/export, failure modes, schema version | settings class, settings page | no | medium |
| **M4 — Remaining consumers** | G, U, B, P, V, J, K, R, F, Delete/Backspace, rack tool, saw pit, storage | remaining files | no | medium–high (rack tool, Map editor shift/delete) |
| **M5 — Gamepad UI focus (optional, Q3)** | Gamepad navigation of settings UI; device-change handling | UI layer | no | medium; needs device |
| **M6 — Opt-in local summary (optional, Q9)** | Off-by-default consent, view/export/delete, no identity, no network | new class + panel | no | privacy/consent wording; **requires explicit user approval of wording** |

Recommended order: **M0 → M1 → M2 → M3 → M4**, with M5/M6 only on explicit request.

## 3. Verification strategy

- **Static/offline (no Unity):** extend the plain-.NET checks in `Tools/Verification/ScenarioOneUx/OfflineLogic` for label generation, import validation, schema versioning and corrupt-file handling (pure logic, no UnityEngine, same pattern as `ControlsReference`).
- **Unity Editor:** per spec §6 matrix; each slice names the AT rows it must pass. Use the documented disposable-harness workflow in `Tools/Verification/`; harnesses must not leave scripts or `.meta` files under `Assets/ForestPrototype/` and must not disturb `forest-save.json`.
- **Player/device:** gamepad rows, non-QWERTY layout, network-silence check (AT-25) need a built Player and hardware; not available tonight.
- **Human:** keyboard-only and sensitivity-feel rows need at least a few testers; their feedback is anecdotal (spec §3.7).
- No slice is "verified" on compilation alone.

## 4. Risks and mitigations

| Risk | Mitigation |
|---|---|
| Esc/overlay ordering regressions (O-13) | Do not touch Esc in M1–M2; fixed key; AT-22 regression row |
| Tab = Work Plan vs UI focus | Contexts (REQ-C2); decide Tab handling in settings page before M1 UI is built |
| UI Toolkit focus capability unknown (O-16) | Spike inside M1 behind a Unity session; if not focusable, add explicit key handlers, not a framework |
| Reflection access to `mouseSensitivity` (O-4) | Keep field name, or update diagnostic in same change |
| Candidate vs main diverge (O-12: P and N keys, line shifts, save version 21 vs 24) | Re-run the O-7a command against the actual base before M2; stop condition S-5 if a consumer is unlisted |
| Load-time binding collisions (REQ-P3a) | Deterministic rule + AT-17a/17b; never silently reassign a valid user override; no path leaves a required action unbound |
| Misuse of preference data | Spec §3.7 wording; no "optimal" claims; N always shown |
| Consent/privacy | Opt-in only, wording approved by user, no network code (REQ-F4) |
| Scope creep to gamepad gameplay | Q3 decision; non-goal 5 |

## 5. Rollback

Settings are additive and local. Rollback = delete the settings file / revert the change; game saves are never altered, so no save migration or rollback is needed (while REQ-S5 holds).
