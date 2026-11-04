# INPUT-SETTINGS-SPEC-HANDOFF — Issue #257

- **Status:** **DRAFT** (docs only). Not reviewed, not integrated, not accepted. Independent GPT-family review still required (#257 comment).
- **Task:** CCF #257 — mouse sensitivity, rebindable controls, consent-based tester preferences.
- **Branch / worktree:** `docs/overnight-input-settings-spec-20261104` in `/media/jer/Files/CCF/Operations/CCF-Overnight-InputSpec-20261104`, from `origin/main` `8f0f2301`.
- **Deliverable:** `specs/player-input-settings-and-feedback-v1/{spec.md, plan.md, tasks.md, checklists/requirements.md}` plus this file.
- **Not touched:** `active-tasks.md`, `ROUTING-LEDGER.md`, any code/asset/scene/`.meta`, `main`, `integration/scenario-one-player-test-ready-v1`.
- **Not done:** no Unity, no Player build, no runtime, no collection of any data, no save-schema change.

## 1. What was read (VERIFIED by inspection, read-only via `git show`)

`ForestPlayer.cs`, `UI/ControlsReference.cs`, `UI/ScenarioOneUiRoot.cs`, `UI/StandaloneSessionMenu.cs`, `UI/MenuHelpView.cs`, `Racks/RackTool.cs`, `UI/Map/StandMapRackEditor.cs`, `ScenarioOne/ScenarioOneManager.cs`, `ForestSaveController.cs`, `ForestSaveData.cs` (version only), `Assets/InputSystem_Actions.inputactions` (names/paths), `ProjectSettings/ProjectSettings.asset` (`activeInputHandler`), the verification tools referencing sensitivity/`ControlsReference`, and #257 with its two comments. Candidate `72b4efbd` was diffed for the same files; it is not an ancestor of `origin/main` (diverged 451/217 commits).

## 2. Key findings (details with file:line in `spec.md` §2)

1. Sensitivity is one serialized field (`ForestPlayer.cs:16`, used `:307`); nothing saves it. Verification tooling reads it by reflection (`CameraLookDiagnosis.cs:122`).
2. There is **no rebinding layer**: ~75 hard-coded `Keyboard.current`/`Mouse.current` reads across 15 files; no `InputAction`/`PlayerInput` use in `Assets/ForestPrototype/*.cs`; the template `.inputactions` is not used by code (scene/prefab references not inspected).
3. `ControlsReference` is help text only, with offline checks asserting its labels.
4. Esc is handled independently in five places with `RackTool.EscapeFrame` as the only coordination.
5. No settings/controls page exists in the F10 session menu; no keyboard/gamepad UI-focus handling was found; Tab is both Work Plan and the usual UI focus key.
6. Stand Map rack editing is pointer-only (left button, Shift, Delete/Backspace).
7. Only `PlayerPrefs` "seen/done" flags exist as device-level preferences; the game save (`forest-save.json`, v21) holds no settings.
8. Candidate `72b4efbd` adds a `P` key (rack spray marker); main has none — the binding inventory depends on the base.

## 3. Verified vs assumed

| Verified (code read) | Unverified / assumed |
|---|---|
| All O-* facts in spec §2 at the cited lines on main (line shifts of +3 on candidate for ForestPlayer) | UI Toolkit keyboard/gamepad focusability; cursor state when settings open |
| `activeInputHandler: 1` | Whether any scene/prefab uses `InputSystem_Actions.inputactions` |
| #257 text and comments | Input System package version/APIs available in Unity 6000.6.0f1 |
| Save has no settings fields | Non-QWERTY behaviour; gamepad behaviour; Windows `persistentDataPath` |
| | Number/hardware of testers; sensitivity range; any "typical" preference (nothing measured) |
| | All runtime, rendered, input-device and accessibility claims: **UNVERIFIED / UNMEASURED** — Unity was not run |

## 4. Decision list for Manager / user

1. **Approve scope direction:** sensitivity first (M1), then rebinding (M2–M4)? (Q5)
2. **Per-device vs per-profile** settings (recommended per-device); confirm **no save-schema change** — any save change needs separate approval. (Q1, REQ-S5)
3. **Storage:** separate settings file vs `PlayerPrefs` vs input override JSON. (Q2)
4. **Fixed keys:** keep Esc, F1, F5/F9, F10 non-rebindable? (Q4)
5. **Gamepad:** out of V1 except UI focus? (Q3)
6. **Rebinding blast radius:** accept staged migration of 15 files, subset first? (Q7)
7. **Opt-in local preference summary:** wanted at all, when, and who approves the consent wording? Nothing is implemented or claimed. (Q9)
8. **Timing:** before or after the player-test-ready release? (Q11)
9. **Stand Map keyboard editing:** separate follow-up spec? (Q8)
10. **Layout policy** for non-QWERTY users. (Q6)

## 5. Statistics caveat (carried into the spec)

Any summaries would come from a small, self-selected convenience sample, with uncontrolled hardware (DPI, acceleration). They can describe what a few testers chose; they cannot justify "optimal" or "recommended default" values. (Spec §3.7)

## 6. Next steps

Independent review (different provider); Manager answers §4; decision-log entry; only then does a Manager create M1 task packet(s). No implementation should start from this DRAFT.

---

# Appendix — V2 Orchestration Decision Appendix

**Decision unit:** #257 input-settings DRAFT spec. **Decision class: SPECIFICATION DRAFT ONLY.** This appendix records what was done. It approves no model, code, save change or spec status.

| Item | Record |
|---|---|
| Journal entry | **EVENT 006** (ledger text itself not edited; ROUTING-LEDGER.md is the DPC orchestrator's sole-writer file) |
| Pool / provider | `claudeAgent_claude_b_pro` (Claude B, Pro pool) |
| Model / effort | `claude-sonnet-5-5`, `effort: medium`, role `design` (DRAFT-spec writer) |
| Reviewer | not yet run; independent GPT-family review required before any acceptance; this worker cannot approve its own work |
| Base | `origin/main` `8f0f2301`; candidate read `72b4efbd`, via `git show` only |
| Unity | not started, no lease requested, deferred tonight on disk grounds |
| Quota / tokens / context | UNKNOWN; no telemetry exposed and none estimated |
| Retries / failures | none; one tool-status interruption, no effect on content |
| Self-assessed risk | Medium: a draft is only as good as the unrun matrix; unverified UI-focus and Input System API assumptions are flagged, not resolved |
| Routing evidence returned for the DPC orchestrator | Task completed in the same worker session; no escalation needed; no scope breaches |
