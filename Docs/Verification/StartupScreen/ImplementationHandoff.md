# Scenario One minimal startup / title screen — implementation handoff

**CLOUD HANDOFF — NOT UNITY-VERIFIED.** Implemented and statically reviewed in a cloud session with no Unity Editor and no C# compiler. No Unity gate has been run. Nothing here claims compile success, rendering, or regression results.

- Task: Scenario One minimal startup / title screen (private-test Stage-A entry).
- Base: `23f84f9ac0d4a02d385869d3985e11a24cc5a23f` (`origin/task/enlarged80-area-calibration`); branch fast-forwarded to exactly this base before any edit.
- Context commit: `f4535caf606bca46f35f02408920408d6b9f20d3`. The canonical context set is byte-identical between the context commit and the base (checked by `git diff`); no canonical context file, `active-tasks.md`, scene, prefab, material, ProjectSettings or package file was touched.
- Branch: the packet names `task/scenario-one-startup-screen`; this session was assigned `claude/laughing-galileo-bbzv0j` and pushed only there. The Manager can create the task branch from the reported HEAD.

## Player flow

Player launch → startup screen → **Start New Scenario** or **Continue** → stand. F10 in the stand still opens the existing session card.

Startup screen content, exactly: `CCF`, `Scenario One`, `Sitka Plantation to Continuous-Cover Forest`, buttons **Start New Scenario / Continue / Controls / Help / Quit**, one concise reason line when Continue is disabled, and a small `Private test build · <build ID>` line. No ecology, no management advice, no instructions on what to fell, thin, plant or how to win.

## Architecture (reuse, no second session or save system)

The title is a second *card* on the existing `StandaloneSessionMenu`, so it reuses that menu's pause, input isolation (player, save shortcuts, forest UI disabled; time scale 0; mouse forced free; `ScenarioOneManager.Update` yields via `StandaloneSessionMenu.IsOpen`) and restore logic. It is a thin layer over existing actions:

| Button | Calls |
|---|---|
| Start New Scenario | `Close()` — enters the world the scene already generated. New-game setup (RNG/regeneration/growth/storm stack, `ApplyStandGeometry(StandGeometryPolicy.NewGameModelForSession)`, `InitializeNewScenario()`) runs in `ScenarioOneManager.Awake`, and the stand in `ForestStartingStand.Awake`, both synchronous before any `Start()`. The title restates no policy constant. With a save on disk it first asks, and says the save is kept until saved over (F5 / F10 Save). |
| Continue | `ForestSaveController.CanLoad` (offer / re-check at click), then `Close()` and the existing `Load()` — the same order the F10 menu's own load button uses. |
| Controls / Help | A short key list (WASD, Shift, Space, mouse, E, M, Tab, O, F1, Esc, F5/F9, F10) and Back. |
| Quit | The existing `Quit()` (`CCF_SESSION_NORMAL_QUIT`, `Application.Quit(0)`). No confirmation: nothing is unsaved at the title. |

### Files

| File | Change |
|---|---|
| `Assets/ForestPrototype/UI/StandaloneSessionMenu.cs` | `partial`; `Bootstrap` policy; `Start` opens the title when asked; `Open`/`Close` pick the card / clear the flag; F10 ignored while the title is up; `ActionButton` returns the button. Session card untouched. |
| `Assets/ForestPrototype/UI/StandaloneSessionMenu.Startup.cs` (+ `.meta`) | New: the title, confirmation and controls cards and their handlers. |
| `Assets/ForestPrototype/ForestSaveController.cs` | `Load()` and `LoadData()` split so their existing pre-mutation checks are shared helpers (`TryReadSave`, `FindLoadProblem`), plus a read-only `CanLoad(out problem)` using the same two helpers. **Behaviour-preserving extraction**: same checks, same order, same messages. |
| `Tools/Verification/Startup/` | Disposable gate `StartupScreenVerification.cs`, `run_startup.py`, `README.md`. |
| `Tools/Verification/S1B/StandalonePlayerSmoke.cs` | The diagnostic Player smoke now leaves the title through `StartNewScenario()` / `ContinueSavedForest()` instead of `Close()`. |
| `Tools/Build/S1B/README.md` | One sentence. |

Overlap with calibration-owned files: **none** (`ScenarioOneDefinition.cs`, `ScenarioOneObjectives.cs`, `AnnualReviewView.cs`, `ScenarioOneUiFacts.cs`, `ScenarioOneManager.cs` untouched). No save schema, definition-version, geometry, calibration, RNG, regeneration, growth or storm change. `ForestSaveData.CurrentVersion` stays 20.

### Player vs Editor (explicit)

| Where | Launch behaviour |
|---|---|
| Standalone Player | `Bootstrap` always creates the menu and opens the title. No switch exists in a Player. |
| Editor Play Mode (default) | Unchanged: nothing bootstrapped, Play Mode enters the stand directly; every existing gate behaves as before. |
| Editor + `CCF_STARTUP_SCREEN=1` | The same real `Bootstrap` opens the title. Compiled out of Players (`#if UNITY_EDITOR`). |

Gates that need a menu (`SessionMenuVerification`, the new gate) construct their own `StandaloneSessionMenu`; there is no scene-state detection. `SessionMenuVerification` builds the menu without asking for the title, so its contract (session card, "Start a fresh…" and "Quit" buttons) is unchanged by construction.

### Save / Legacy40 / Reference

`CanLoad` is read-only and runs the loader's own checks, so Continue is offered only for a save `Load()` would accept; the title never inspects geometry itself (`StandGeometryModel.ForSave` is called only inside the loader, as before). A Legacy40 save therefore loads under its own geometry exactly as with F9. Reference Future handling, `LegacyCompatibleWorldHash` and v20 identity hashes are not touched.

## Static checks actually performed

- Base verified exactly `23f84f9`; context-set files unchanged against `f4535ca`.
- All five touched/added C# files parse with a real C# grammar (tree-sitter); the parser was calibrated on known-good project files and a deliberately broken negative control. This catches syntax errors only.
- Cross-references to existing members (`StandGeometryModelVersion`, `EcologicalYear`, `StandGeometryPolicy.NewGameModelForSession`, `ScenarioOneUiRoot.Player/Manager/Help/CurrentScreen`, `ScenarioReferenceArchive.WorldHash/CurrentWorldHash`, `RegenerationModel.Competition`, …) checked to exist and be public.
- Adversarial re-read for definite assignment, out-parameter lambda capture (`FindLoadProblem` copies `geometry` before the `Exists` predicate), `UnityEngine.Cursor` vs `UIElements.Cursor` ambiguity (qualified in the harness), nested coroutine use (`StartCoroutine`), and input pollers (every world-interaction component gates on `Cursor.lockState == Locked`, which the menu forces to `None` every frame while open).
- `run_startup.py` byte-compiles.

## UNITY GATES STILL REQUIRED

1. Import/compile: zero C# compiler errors (nothing here has been compiled).
2. `StartupScreenVerification` — `--mode batch`, `--mode bootstrap`, `--mode rendered` (needs a display), and `--mode batch --stand-geometry 0` for the explicit Legacy40 path. See `Tools/Verification/Startup/README.md`.
3. `SessionMenuVerification` (StandaloneSessionMenu contract), `MenuTutorialVerification` (current accepted contract), `ScenarioOneInteractionVerification`.
4. Save gates, because `ForestSaveController` changed: `SaveHardeningVerification`, `StandGeometryVerification`, `Enlarged80Verification`, the Reference gate. Expected: identical results and hashes (start `04A78188A6F8E6F9`, Legacy40 `EBE7A228F630AE41`, Model2 `702766DECE591E21`, Reference Y100 `7AD177B3CC2F73C7`, continuation `9CDF21A541C5968D`).
5. Calibration targets unchanged from `23f84f9`: `AreaCalibrationVerification` ×1.
6. Construction: `ScenarioOneInteractionVerification` asserts unavailable sites stay hidden / inactive.
7. P2/P3 only if bootstrap/scene startup is suspected to matter (this change does not alter Editor startup).
8. Normal full regression before integration (Manager requirement). Deferred here: not runnable without Unity.
9. **Human checks** the gates cannot replace: read the rendered captures at 1280 × 720 and 1920 × 1080; real mouse clicks and keyboard navigation (Tab / arrows / Enter / Space, initial focus on Continue or Start New) in an interactive Editor with `CCF_STARTUP_SCREEN=1` and in a Player; confirm the title cannot be left by F10 or any gameplay key; Quit in a Player.

## Expected observable behaviour

- Player launch shows a dark full-screen title; nothing moves, the mouse is free, W/E/Tab/M/O/F1/F5/F9/F10/Esc do nothing to the forest.
- No save: Continue greyed with "No saved forest on this device yet." Start New Scenario enters year 0 (Enlarged80, 80 × 80 m, start anchor `04A78188A6F8E6F9`).
- Valid save: Continue enabled and loads it. Start New asks "Start a new Scenario One?" and states the saved forest stays until saved over.
- Unreadable / invalid save: Continue greyed with the loader's own reason; the world is untouched.
- In the stand F10 opens the familiar session card; "Start a fresh Scenario One test…" still reloads the scene without showing the title again (existing behaviour).

## Known limitations and open points

- **Unverified: everything runtime.** No compile, no run, no render, no Player.
- Controls list is new concise copy (Shift = run, Space = jump verified from `ForestPlayer`); there is no existing controls list to reuse as-is, and `MenuHelpView` copy is tutorial prose the packet says not to repeat.
- The title screen's keyboard navigation depends on UI Toolkit's Input System integration, which the gate cannot exercise (the project's other gates call `Clickable` directly for the same reason).
- `CanLoad` reads and validates the whole save once when the title opens (three Newtonsoft parses plus `JsonUtility`); on a very large save this is a one-off pause behind the title.
- A save changed between the title opening and the click is re-checked at click time; the residual race after the check falls back to the loader's normal message over the fresh stand.
- Quit has no confirmation at the title (nothing unsaved exists there).
- No pilot / B07 / B08 candidate was created or authorised by this change.

## Decisions needed

- Whether Start New Scenario with an existing save should keep its confirmation (current) or be a single click.
- Whether the Editor opt-in (`CCF_STARTUP_SCREEN`) is enough, or the Editor should also offer a menu item.
- Which branch name the Manager wants for integration (`task/scenario-one-startup-screen` vs the assigned `claude/…` branch).
