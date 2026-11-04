# PLAYER INPUT SETTINGS AND FEEDBACK V1 — Specification

- **Spec ID:** `PLAYER-INPUT-SETTINGS-AND-FEEDBACK-V1`
- **Status:** **DRAFT** — not approved; authorises no implementation, no save-schema change and no data collection
- **Owner:** Overall Manager (to assign); drafted by design sub-agent (Sonnet 5.5, medium) under #257
- **Created / last updated:** 2026-11-04
- **Intent sources (cited, not extended):** GitHub issue #257 body and its owner comment of 2026-11-04T00:15Z ("SPEC DRAFT ONLY … no telemetry collection, runtime UI implementation or save policy changes"); user feedback relayed in #257 (mouse sensitivity, configurable keybinding, per-tester persistence, aggregated preferences to inform future defaults).
- **Decision log references:** none yet. Approval would need a new decision-log entry; none is claimed here.
- **Code baseline read:** `origin/main` `8f0f2301` (primary) and candidate `72b4efbd` (`integration/scenario-one-player-test-ready-v1`) via `git show`. No Unity, no checkout of the candidate, no runtime.
- **Companion files:** `plan.md`, `tasks.md`, `checklists/requirements.md`.

> **Labels used.** **OBSERVED** = read in code at the cited file:line. **PROPOSAL** = this draft's suggestion, not decided. **UNVERIFIED** = needs Unity/runtime/input-device evidence that was unavailable. **DECISION NEEDED** = belongs to the user/Manager.

## 1. Purpose

Testers have asked for mouse sensitivity, rebindable keys and a way to say what they prefer. Today none of those exist as player-facing features. This spec describes what a small, safe first version would be, what it would cost in persistence and architecture, and what must be decided before any code is written. It does **not** start implementation.

## 2. Observed current state (OBSERVED CODE FACT — separate from proposals)

### 2.1 Mouse sensitivity

- O-1. Sensitivity is one serialized private field, `[SerializeField, Min(0.01f)] private float mouseSensitivity = 0.12f;` — `Assets/ForestPrototype/ForestPlayer.cs:16` (main). It is a prefab/scene-serialized value, not read from any player setting.
- O-2. It is used once, `Vector2 look = rawDelta * mouseSensitivity;` — `ForestPlayer.cs:307` (main; line 310 on the candidate, which adds 3 lines). `rawDelta` is `mouse.delta.ReadValue()` (`:296`) in pixels/frame, deliberately not multiplied by deltaTime (`:295`). The same sensitivity scales yaw and pitch; there is no separate Y, invert, or smoothing option.
- O-3. Nothing saves it. A search of `Assets/ForestPrototype/*.cs` finds `PlayerPrefs` only in `UI/MenuHelpView.cs:64,70,156` (prefix `CCF.MenuHelp.v1.`, `:11` of that class) and `UI/LearningObjectivesView.cs:83-102` (prefix `CCF.Learning.v1.`, `LearningObjectivesView.Curriculum.cs:9`). Both store per-device "seen/done" ints; neither stores a control or sensitivity.
- O-4. Verification tooling already reaches the field by reflection: `Tools/Verification/ScenarioOneUxPedagogy/CameraLookDiagnosis.cs:122` reads `"mouseSensitivity"` via `GetField(..., NonPublic | Instance)`. Renaming or moving the field would break that tool.
- O-5. The camera recentre logic (`ForestPlayer.cs:311-334`) measures `rawDelta.magnitude` in pixels against `Screen` size. A sensitivity UI does not change that budget, but a future gamepad-look path would need separate handling (see §6).

### 2.2 Input paths — no rebinding layer exists

- O-6. `ProjectSettings/ProjectSettings.asset:938` has `activeInputHandler: 1` (New Input System only; legacy `UnityEngine.Input` is disabled), consistent with `AGENTS.md`.
- O-7. Gameplay and UI read **hard-coded device controls** directly, e.g. `Keyboard.current.escapeKey.wasPressedThisFrame` (`ForestPlayer.cs:227`), `eKey`/`fKey`/`gKey`/`digit1Key`/`digit2Key`/`uKey`/`rKey` (`:256-263`), `wKey/aKey/sKey/dKey`, `spaceKey`, shift keys (`:287-291`). Other consumers: `ForestBuildable.cs:81`, `ForestSawPit.cs:74`, `ForestWoodStorage.cs:77`, `ForestTreeMarkingManager.cs:150,169,182`, `Racks/RackTool.cs:76` (B, Esc at `:99,105`), `RecentAssetReviewControls.cs:12`, `ForestEcologyController.cs:232`, `ForestSaveController.cs:26,33-35` (F5/F9), `ScenarioOne/ScenarioOneManager.cs:581-594` (Tab, Esc), `UI/ScenarioOneUiRoot.cs:203-210,311-352` (V, J, K, F1, O, M, Esc, Tab, arrows), `UI/StandaloneSessionMenu.cs:70` (F10), `UI/CompetitorAssessment.cs:277`, `UI/Map/StandMapRackEditor.cs:252,445-449` (Shift, Delete, Backspace). 15 files, ~75 input matches.
- O-8. There are **no `InputAction`, `InputActionAsset`, `PlayerInput` references in `Assets/ForestPrototype/*.cs`** (grep). `Assets/InputSystem_Actions.inputactions` exists (Unity template: `Player` map with Move, Look, Attack, Interact, Crouch, Jump, Previous, Next, Sprint, with keyboard+mouse, gamepad, touch, XR and joystick bindings, e.g. `<Pointer>/delta` for Look). Whether any scene/prefab references it was **not inspected** (UNVERIFIED); gameplay code does not use it.
- O-9. Consequence: gameplay has **no gamepad support** and no multi-binding. Move is WASD only (not the template's arrow keys). Mouse look is `Mouse.current.delta` only.

### 2.3 Legacy controls reference

- O-10. `UI/ControlsReference.cs` is the single human-readable list of keys: `Rows` (`:23-42`, 17 rows), `KeysLine` (`:45-47`), plus `RowsFor` / `KeysLineFor` (`:9-21`) which hide `B` and relabel `M` when Rack Planning V2 is active. It is **documentation text, not a binding source**; changing a key in code and forgetting this file makes the help wrong. Offline checks assert specific labels (`Tools/Verification/ScenarioOneUx/OfflineLogic/ScenarioOneUxOfflineCheck.cs:212-233`; `Qualification/UxQualification.cs:692,1164-1179`), so a rebinding feature that makes labels dynamic must update those checks.
- O-11. Help text is built from the same class: `MenuHelpView.Text.cs:100`, `MenuHelpView.cs:132`, `StandaloneSessionMenu.Startup.cs:33-35`.
- O-12. Candidate `72b4efbd` adds a `P` row ("Equip / stow the rack spray marker…") to `ControlsReference` and reads `pKey` in `ForestTreeMarkingManager.RackEntrances.cs:88`; main has no `pKey` use. The binding inventory therefore depends on which commit ships; an implementation must re-inventory against its own base.

### 2.4 Esc, F1, mouse capture, UI focus

- O-13. Esc is multi-purpose and handled in **at least five independent places**, with ad-hoc ordering: `ForestPlayer.cs:227-238` (rack tool active → nothing; planting mode → exit; inspecting → close card; else free the mouse); `RackTool.cs:105-107` (sets `EscapeFrame` so the player skips its own Esc that frame); `ScenarioOneUiRoot.cs:317,321,348` (onboarding back; close help; close Map/Objectives); `ScenarioOneManager.cs:586,593` (end reference preview; close Work Plan). Order is by script execution, coordinated only by `RackTool.EscapeFrame`/`AnyActive`.
- O-14. F1 toggles help (`ScenarioOneUiRoot.cs:326-330`); it cannot skip onboarding (`:316`); help is modal (`:331-334`). F10 opens the session menu (`StandaloneSessionMenu.cs:70`), which forces the cursor free every frame while open (`:75`). The session menu has save/load/new/quit; **no settings or controls page** was found.
- O-15. Mouse capture: click re-locks the cursor unless the press is on a HUD button (`ForestPlayer.cs:239-254`); focus loss unlocks (`:203-206`). Movement and look only run while locked (`:283`). Keyboard-only players can only use Esc to free the cursor; UI Toolkit panels need the pointer once free.
- O-16. **Real UI focus handling (keyboard/gamepad navigation of UI Toolkit controls, EventSystem, focus ring) was not found in the files read and was not tested.** Whether `UiKit` buttons are focusable by Tab/arrows, and what Tab does there, is UNVERIFIED. Note Tab is also the Work Plan key (`ScenarioOneManager.cs:591`), a direct conflict with Tab as UI focus-next.

### 2.5 Stand Map rack interaction

- O-17. Stand Map rack editing is **pointer-primary**: left button only (`StandMapRackEditor.cs:178`, `e.button != 0`, and `:271`), Shift as modifier (`:252`, also `EventModifiers.Shift`), and Delete/Backspace to remove the selected rack (`:445-449`, called from `ScenarioOneUiRoot.cs:335-337`). There is no keyboard-only way to place, move or select a rack. The legacy forest rack tool (`RackTool.cs`) uses B and left/right mouse and Esc and is disabled when Rack Planning V2 is active (`:70-75`).

### 2.6 Persistence available today

- O-18. Game save: `forest-save.json` under `Application.persistentDataPath` (`ForestSaveController.cs:9,15`), atomic with `.tmp`/`.bak` (`:17-19,55-58`), `ForestSaveData.CurrentVersion = 21` (`ForestSaveData.cs:7`). Session-lifecycle values are deliberately excluded from the save schema (`ScenarioOneManager.cs:597`). No field there holds a setting; **adding one is a save-schema change.**
- O-19. Device-level prefs: `PlayerPrefs` with versioned string prefixes (O-3). On Linux these live in the user profile, outside the game save.
- O-20. There is no concept of "profile" or "tester" in the repository (not found). F5/F9 operate on a single slot.

## 3. Product requirements (PROPOSAL — all DRAFT)

Priority tags: **M** must for V1, **S** should, **C** could (defer if costly).

### 3.1 Persistent mouse sensitivity

- REQ-S1 (M). A player-facing sensitivity setting, with a sensible range clamped around the current default 0.12 (range to be set from UNMEASURED comfort testing, not invented here), reset-to-default, and immediate preview while adjusting.
- REQ-S2 (S). Optional separate vertical sensitivity and "invert Y". Default off/equal so behaviour is unchanged for existing testers.
- REQ-S3 (M). The setting is applied by `ForestPlayer` in place of the serialized constant; the serialized value remains the **default**, preserving the reflection access in O-4 (or the diagnostic is updated deliberately).
- REQ-S4 (M). **Scope — recommended: per-device, local, outside the game save** (a settings file or `PlayerPrefs` with a versioned prefix such as `CCF.Input.v1.`), because sensitivity and bindings belong to the person's hardware, not to a forest world: loading an old save or starting a new scenario must not change how the mouse feels. **Per-profile** (selected tester name) is only worth building if several testers share one machine (**DECISION NEEDED**, Q1). **Per-save** is rejected: it would change the save schema and tie hardware preference to a world.
- REQ-S5 (M). **Save/schema statement.** The recommended design **does not** touch `ForestSaveData` or `forest-save.json` and needs no save-version bump. Any alternative that does (per-save or per-world settings) is a separate save-policy decision requiring explicit user approval and is out of scope of this draft.

### 3.2 Rebindable actions

- REQ-B1 (M). Introduce a named **action layer** for the keys listed in O-7 so gameplay asks "was Interact pressed" rather than "was E pressed". The migration is the largest cost (15 files); see `plan.md` for staging. The Unity-built-in route (an `InputActionAsset` with `PerformInteractiveRebinding` and binding-override JSON) is preferred over a custom framework, per `AGENTS.md` ("prefer built-in features").
- REQ-B2 (M). **Rebinding applies to a curated action list**, not every key. Proposed rebindable: movement, run, jump, interact/inspect, X mark, C crop tree, G plant, U clearance, M map, Tab work plan, O objectives, B rack, P spray marker (candidate only), V detail, J/K rack decisions. **Fixed (not rebindable) by default:** Esc (reserved cancel/free-mouse), F1 help, F5/F9/F10 safety/save keys — to keep the always-available escape routes and the documented recovery path (DECISION NEEDED, Q4).
- REQ-B3 (M). **Conflict detection.** Binding a control already used by another action *in a context that can be active at the same time* must be refused or must offer "swap / unbind the other / cancel". Never silently double-bind. Contexts are defined in §3.3.
- REQ-B4 (M). **Unbinding.** A player may clear a binding, except that no *required* action may end with zero bindings on every device class in use (e.g. cannot unbind the last "Move forward" binding); the UI says why.
- REQ-B5 (M). **Reset to defaults**, per action, per context group, and whole-set, each with a confirmation for the whole-set reset.
- REQ-B6 (S). **Import/export** of bindings + sensitivity as a human-readable file the player chooses to save/open (local file only, user-initiated, no network). Import is validated: unknown actions ignored with a message, unknown/invalid paths rejected, version field checked; an import never partially applies (all-or-nothing) and never executes content.
- REQ-B7 (M). Help text (`ControlsReference` rows, `KeysLine`, onboarding, F1 help) shows the **player's current bindings**, not hard-coded labels; the default text is the fallback. The offline checks in O-10 are updated in the same task.
- REQ-B8 (S). **Multiple bindings per action** (e.g. keyboard key + a gamepad button, or two keys). The settings UI lists all bindings for an action; adding a second never removes the first; each can be cleared independently. Conflicts (REQ-B3) are checked per binding.

### 3.3 Gameplay vs UI context conflicts

- REQ-C1 (M). Define explicit **input contexts**, highest priority first: (1) modal (onboarding, help, session menu, binding-capture), (2) full-screen panels (Stand Map, Objectives, Work Plan), (3) walking with mouse locked, (4) walking with mouse free. Only the highest active context consumes bindings; lower contexts see nothing. This replaces the current ad-hoc Esc ordering (O-13) *for rebindable actions*; Esc handling is consolidated only under a separate task with its own tests.
- REQ-C2 (M). The same physical control may be bound to different actions in **different, mutually exclusive contexts** (e.g. Tab = Work Plan while walking, Tab = focus-next inside a settings panel). It may **not** be bound to two actions in the same context. Conflict detection (REQ-B3) is computed against the context groups that can be simultaneously active.
- REQ-C3 (M). **Binding capture mode** is itself a modal context: while waiting for a key, gameplay and global keys (including F1/F10) do not fire; Esc cancels the capture, and a visible timeout returns control.
- REQ-C4 (M). Text-entry fields (profile name, import path if any) suppress all gameplay bindings while focused.

### 3.4 Accessibility and no-mouse operation

- REQ-A1 (M). The settings screen is fully operable with **keyboard only** (visible focus indicator, Tab/Shift+Tab or arrow navigation, Enter/Space to activate, Esc to back out) and, if gamepad support is added, with **gamepad only** (D-pad/stick navigate, South confirm, East cancel). A player with the mouse captured must be able to open settings and release the mouse without a pointer (e.g. via F10 or a documented key). O-15/O-16 show this path is not demonstrated today (UNVERIFIED).
- REQ-A2 (M). Sensitivity is adjustable by keyboard (arrow keys step the slider; a numeric value is shown; Home/End jump to min/default).
- REQ-A3 (S). Binding-capture shows what was captured and allows "cancel" without a pointer; an on-screen conflict message is announced as text (no colour-only signalling).
- REQ-A4 (C). Hold/toggle options for hold-to-act inputs (U clearance hold, Shift run) — only if testers request them.
- REQ-A5 (M). Stand Map rack editing is pointer-only today (O-17). This spec **does not require** fixing that, but records it as an accessibility gap with a follow-up question (Q8) and does not claim the settings feature makes the Map keyboard-accessible.
- REQ-A6. Gamepad **gameplay** (look/move/interact on a pad) is **not** in V1 scope unless the user decides otherwise (Q3); only UI focus navigation and device-change handling are specified.

### 3.5 Preservation and failure modes

- REQ-P1 (M). Bindings and sensitivity survive: quit/restart, F5/F9 save-load, New Scenario, scene switch, startup→game transition, and window focus loss. They are **not** reset by loading or deleting `forest-save.json`.
- REQ-P2 (M). **Device change.** Bindings are stored per control scheme/device class (keyboard+mouse vs gamepad). Plugging in/out a gamepad, or the mouse becoming `null` (`Mouse.current == null`, already guarded e.g. `ForestPlayer.cs:293`), must not clear or corrupt bindings; actions with no live binding on the *current* device class fall back to the default for that class.
- REQ-P3 (M). **Missing/invalid saved binding** (action removed in a later version, malformed JSON, unknown control path e.g. a device layout not present, binding now colliding with a newly added default): the loader (a) never throws to the player, (b) keeps all valid entries, (c) replaces only the bad entry with the default, (d) shows one non-blocking notice listing what was reset, and (e) keeps the previous file as `.bak` before rewriting. A completely unreadable file → defaults + notice, never a blocked start.
- REQ-P4 (M). Settings carry a **settings-schema version** independent of the game-save version, with a forward-compatibility rule: an older build opening newer settings ignores unknown fields and does not delete them on rewrite where practical (or documents that it resets).
- REQ-P5 (M). Layout-sensitive keys: binding by **physical key position vs character** (Input System supports both; non-QWERTY layouts matter for WASD). DECISION NEEDED (Q6). Display names must use the layout-correct label.
- REQ-P6 (S). Settings writes are atomic (temp+swap, as `ForestSaveController.cs:55-58` already does for saves); write on explicit Apply/close, not every frame.

### 3.6 Optional local preference summary (OPT-IN, LOCAL ONLY)

> **No collection is implemented or claimed by this spec.** The following is a proposal for a feature that does not exist. Nothing in the repository today records mouse sensitivity, bindings or any preference for any purpose other than the device-local "seen" flags in O-3.

- REQ-F1 (S). Off by default. A single, clearly worded, reversible opt-in control ("Keep a local summary of my control settings so I can share it if I choose"). Declining or never opening it records nothing and degrades nothing.
- REQ-F2 (S). When on, the game keeps only an **aggregate-shaped local file** on the player's machine, containing: settings-schema version; current sensitivity (and optional vertical/invert flag); for each rebindable action, whether it is at default or changed, and the *new control path* if changed; counts of conflicts refused and resets performed; control scheme used (keyboard+mouse/gamepad). **Nothing else**: no tester name or profile name, no machine name, account name, IP, device serial/product strings, no timestamps finer than a coarse "saved on the Nth day of use" counter or none at all, no gameplay or save content, no screen/layout information beyond what is needed to interpret a key label.
- REQ-F3 (S). **No identity.** The file has no identifier that could link two exports from one person unless the player themselves joins them; any random install ID is *not* created.
- REQ-F4 (S). **No automatic transmission.** There is no network code, background upload, crash-report attachment or clipboard push. The only way data leaves the machine is the player choosing "Export summary", viewing the file's exact contents in the UI first, and saving/sending it themselves.
- REQ-F5 (S). The player can view, export, and **delete** the summary at any time; turning the opt-in off offers to delete it.
- REQ-F6 (S). A Manager/user-side tool (a later, separate task) may sum many exported files into counts; that tool is outside the game and outside this spec.
- REQ-F7 (M). **Consent and communication statement** shown at opt-in: what is stored, that it stays local, that it is not sent anywhere by the game, that sharing is optional, and that it is not tied to the player's identity. The exact wording requires Manager/user approval (Q9); no consent copy is drafted as final here.

### 3.7 Statistical honesty (binding on any use of the summaries)

- REQ-X1 (M). Exported summaries come from a **small, self-selected convenience sample** (testers who chose to opt in and share). They cannot support claims such as "optimal", "best" or "most players prefer" sensitivity/bindings. At most they may support **descriptive** statements ("of N shared files, k changed X") with N always stated.
- REQ-X2. Default changes may be *informed by* such counts together with direct tester conversation and accessibility principles, but must be justified on usability grounds, not on the sample alone. Any report must show N, state the selection mechanism, and not report percentages for small N without the raw counts.
- REQ-X3. Hardware confounds are not captured (mouse DPI, OS pointer acceleration, display size, polling rate), so sensitivity values are **not comparable across testers**. Preference summaries must not be read as a calibrated target. Any DPI-normalised "degrees per cm" would need explicit extra data and is out of scope.

## 4. Non-goals (explicit)

1. No production code, UI, scene, prefab or `.inputactions` edit in this task; this spec is documentation only.
2. No telemetry, analytics, crash-report, network, background collection or auto-upload — now or implied.
3. No save-schema or `forest-save.json` change, no `ForestSaveData.CurrentVersion` bump, no new profile/account concept without separate approval.
4. No new package (no Rewired, InControl, third-party rebinding UI); built-in Input System only.
5. No gamepad/touch/XR gameplay support in V1 beyond what Q3 later decides.
6. No change to frozen Scenario One Reference Future v1.
7. No rewrite of Esc/overlay architecture as part of rebinding V1.
8. No claim of Unity, input-device, rendered or accessibility-tool verification.
9. No default recommendation changes. Defaults stay as observed (sensitivity 0.12; current key map) until separately decided.
10. No keyboard-only Stand Map rack editing (recorded gap, Q8).

## 5. Open questions (DECISION NEEDED unless noted)

| # | Question | Draft position |
|---|---|---|
| Q1 | Per-device (shared by all on a machine) or per-profile settings? | Per-device first; profiles only if shared-machine testing is real. |
| Q2 | Settings location: `PlayerPrefs`, a JSON file beside the save, or an `InputActionAsset` override JSON? | JSON file in `persistentDataPath`, separate from the save, with `.bak` (supports import/export and versioning); `PlayerPrefs` for sensitivity alone is the cheapest slice. |
| Q3 | Is gamepad **gameplay** in or out of V1? | Out; UI focus navigation only. |
| Q4 | Which keys are fixed (Esc, F1, F5/F9, F10)? | Fixed, to preserve recovery and documented help. |
| Q5 | Slice order: sensitivity-only first, then rebinding? | Yes (see plan.md). A sensitivity slider is small and independent. |
| Q6 | Physical-key or character-based binding for non-QWERTY layouts? | Physical position for movement; character for mnemonic keys; needs a test on a non-QWERTY layout. |
| Q7 | Is it acceptable to touch 15 consumer files in one program, or must rebinding be limited to a subset first (movement/interact/mark)? | Subset first (M1 in plan.md). |
| Q8 | Is keyboard/gamepad Stand Map rack editing a follow-up spec? | Yes, separate (pointer-only today, O-17). |
| Q9 | Is the opt-in preference summary wanted at all, and who approves consent wording? Does the user want it in the first release? | Defer to a later milestone after sensitivity and rebinding; wording by Manager. |
| Q10 | Is a tester-name "profile" desirable given privacy (REQ-F3)? | No identity in summaries; a local profile name (if built) is never exported. |
| Q11 | Does the first slice ship in the player-test-ready integration branch or after it? | Not decided; this draft assumes after, to avoid late changes to the candidate. |
| Q12 | Sensitivity range and step size? | UNMEASURED; to be set during first Unity session. |

## 6. Acceptance-test matrix

Verification tier: **S** static/offline (no Unity), **U** Unity Editor, **P** Player build/device, **H** human tester. All rows are **UNRUN** — nothing in this spec has been tested.

| ID | Requirement | Scenario | Expected | Tier |
|---|---|---|---|---|
| AT-01 | S1,S3 | Set sensitivity to min/default/max, look 400 px | Yaw ≈ 400×value°, monotonic; default reproduces current feel (0.12) | U,H |
| AT-02 | S1,P1 | Change sensitivity, quit, relaunch | Value retained | U |
| AT-03 | S5,P1 | F9-load a pre-change save | Sensitivity unchanged; save loads; `ForestSaveData.CurrentVersion` unchanged | U,S |
| AT-04 | S4 | New Scenario | Sensitivity/bindings unchanged | U |
| AT-05 | B3 | Bind Interact to key already used by Map | Refused or swap offered; no silent double-bind | U |
| AT-06 | C2 | Bind Tab to Work Plan and to settings focus-next | Allowed only because contexts are exclusive; verified in both contexts | U |
| AT-07 | B4 | Unbind last Move-forward binding | Refused with reason | U |
| AT-08 | B5 | Reset one action / group / all | Only that scope returns to default; all-reset asks to confirm | U |
| AT-09 | B6 | Export, edit file (bad path), import | All-or-nothing rejection with message; settings unchanged | S,U |
| AT-10 | B6 | Import valid file | Applies; help text shows new keys | U |
| AT-11 | B8 | Add second binding, clear first | Both work independently; clearing one leaves other | U |
| AT-12 | C3 | In capture mode press F1, F10, Esc | F1/F10 do not fire; Esc cancels capture | U |
| AT-13 | C4 | Type in a text field using bound letters | No gameplay action fires | U |
| AT-14 | A1,A2 | Complete settings task with keyboard only (mouse unplugged/not touched) | Reach settings, change sensitivity, rebind, reset, exit | U,H |
| AT-15 | A1 | Same with gamepad only, if pad UI supported | As above | P,H |
| AT-16 | P2 | Unplug gamepad / mouse mid-session | No exception; bindings intact; fallbacks apply | U,P |
| AT-17 | P3 | Hand-corrupt settings file; remove one action from schema; unknown control path | Valid entries kept; bad ones defaulted; one notice; `.bak` written; no crash | S,U |
| AT-18 | P3 | Delete settings file | Defaults; start succeeds | U |
| AT-19 | P4 | Older build opens newer-version settings | Ignored/unmodified or documented reset; no crash | S |
| AT-20 | B7 | Rebind E; open F1 help, startup controls card | Both show the new key; offline label assertions updated | S,U |
| AT-21 | P5 | AZERTY layout: default WASD movement | Documented behaviour per Q6 | P,H |
| AT-22 | Esc/overlays | Esc with rack tool, planting, inspect card, help, map, work plan | Same results as today (O-13) after rebinding migration | U |
| AT-23 | F1-F4 | Opt-in off (default) | No summary file created; no network traffic | S,U |
| AT-24 | F1-F5 | Opt in → view → export → delete | Contents exactly as REQ-F2; no identity fields; delete removes file | U |
| AT-25 | F4 | Run with network monitor/offline guard | Zero outbound traffic from settings/summary code | P |
| AT-26 | X1 | Review any summary report | States N and selection; no "optimal/most players" wording | S (doc review) |
| AT-27 | O-4 | Run `CameraLookDiagnosis` after change | Still finds sensitivity or is deliberately updated | U |
| AT-28 | Regression | Existing UX offline/qualification checks | Pass with updated label assertions | S,U |

## 7. Assumptions and unverified items

- UNVERIFIED: whether UI Toolkit controls in this project are keyboard/gamepad focusable; what the cursor/lock state is when a settings page opens; behaviour on non-QWERTY layouts; whether any scene/prefab references `InputSystem_Actions.inputactions`; Input System package version and its `rebinding` APIs in Unity 6000.6.0f1 as installed; how `Application.persistentDataPath` behaves on the Windows test build.
- ASSUMED: the player-test-ready candidate `72b4efbd` is the base testers use; the consumer list may differ on later commits.
- ASSUMED: testers are few; no number is available, and none is invented here.
- No Unity, runtime, device, accessibility-tool or network-traffic measurement exists for any claim above.
