# Startup / title screen verification

Disposable Editor gate for the standalone startup screen (`StandaloneSessionMenu.Startup.cs`). It follows the project's copy → run → remove workflow: `run_startup.py` stages `StartupScreenVerification.cs` under `Assets/ForestPrototype/`, runs Unity, and removes the copy and its generated `.meta` in a `finally`. Nothing from this folder is part of the game or a Player build.

Side effects: none on the player's data. The runner gives Unity an isolated config/licence directory (so `Application.persistentDataPath`, and `forest-save.json`, are not the real player's) and the gate also backs up and restores any save files it finds. No scene is saved. Run only when no other Unity Editor is running, from a worktree with its own `Library`.

## Player vs Editor policy (the distinction the gate checks)

| Where | What happens at launch |
|---|---|
| Standalone Player (private-test build) | `StandaloneSessionMenu.Bootstrap` always creates the menu and opens the **startup screen**. There is no switch in a Player. |
| Editor Play Mode (default) | Nothing is bootstrapped: Play Mode enters the stand directly, as before, so development and every existing gate behave unchanged. |
| Editor Play Mode with `CCF_STARTUP_SCREEN=1` | The same real Bootstrap runs and opens the startup screen. Editor-only (`#if UNITY_EDITOR`), compiled out of Players. |

Gates that need a menu (`SessionMenuVerification`, this gate) create their own `StandaloneSessionMenu`; they never rely on arbitrary scene detection.

## Run

```sh
python3 Tools/Verification/Startup/run_startup.py --mode batch     --label startup-batch-1
python3 Tools/Verification/Startup/run_startup.py --mode bootstrap --label startup-bootstrap-1
python3 Tools/Verification/Startup/run_startup.py --mode rendered  --label startup-rendered-1   # needs DISPLAY
python3 Tools/Verification/Startup/run_startup.py --mode batch     --label startup-legacy40 --stand-geometry 0
```

Results: `Build/StartupScreen/<label>.json` and `.log` (ignored by Git); rendered captures in `<label>-captures/`.

## What each mode covers

- **batch** — title content is exactly: CCF, Scenario One, subtitle (tail of the scenario definition's display name), four buttons, one reason line when Continue is disabled, and the build line; nothing else (no ecology or advice). Time paused; player, save shortcuts and forest UI disabled; mouse not locked; world hash unchanged while the title is open. Continue disabled with a concise reason when there is no save or the file is unreadable, and never attempts a load. Controls / Help lists the required keys and Back returns. Quit reaches the existing quit path (`CCF_SESSION_NORMAL_QUIT`). Start New Scenario (no save) enters the unchanged year-0 world under the new-game stand geometry policy; with a save on disk it asks first and says the save is kept until saved over. Continue loads through the existing validated path and restores the saved world; the save file is not modified. `CanLoad` agrees with the real loader on a valid save, a truncated file, `{}`, non-JSON and a Legacy40 reference save (the loader, not the title, chooses the geometry). The F10 session card still opens once in the stand.
- **bootstrap** — the real Bootstrap opens the title unaided and Start New Scenario hands the stand back.
- **rendered** — everything above plus: virtual key presses (F10, WASD, E, Tab, M, O, X, C, F1, F5, F9, Esc, Space, 1) do not leave the title or touch the forest, and F10 then works in the stand (also the control proving the presses are delivered); layout checks (fully on screen, no overlap, buttons tall enough, build line smaller than the subtitle) and screenshots at 1280 × 720 and 1920 × 1080 for: title without a save, title with a save, the new-scenario confirmation, and Controls / Help.

## Not covered here (separate, human or Player checks)

- Real mouse clicks and keyboard navigation (Tab / arrows / Enter) on the runtime panel — Editor synthetic events do not reach it, so the gate calls each button's registered callback. Check by hand in an interactive Editor with `CCF_STARTUP_SCREEN=1` and in a Player.
- Player-only behaviour (the real `#if` split, Player.log markers, window sizes other than the two above, native Windows text rendering).
- Reading the screenshots: the layout assertions are not a substitute for looking at the captures.

Markers to look for in the log: `STARTUP_CHECK …` (each passed check), `STARTUP_SKIPPED …`, `STARTUP_PARITY …`, `STARTUP_RENDER_SIZE_PASS …`, and the final `STARTUP_SCREEN_VERIFY_PASS` or `…_FAIL`. The Player emits `CCF_STARTUP_SCREEN_OPEN`, `CCF_STARTUP_NEW_SCENARIO` and `CCF_STARTUP_CONTINUE` for Player.log evidence.

## Local port verification additions

The verification worker adds two bounded modes; production behaviour is unchanged:

```sh
python3 Tools/Verification/Startup/run_startup.py --mode boundary --label startup-boundary
python3 Tools/Verification/Startup/run_startup.py --mode input --label startup-input
```

`boundary` records the current-world hash before and after CanLoad; checks no-save, valid current v20, valid historical Legacy40, malformed JSON and a duplicate-ID validation rejection; uses the actual Continue route; replaces an offered valid file before Continue to check immediate revalidation; and exercises one confirmed F10 fresh-test scene reload with a persistent menu host, checking host/overlay count, fresh identity and restored controls.

`input` requires a graphical Editor and an external operator to deliver actual desktop input within each bounded phase (180 seconds). It does not invoke Clickable callbacks or enqueue InputSystem states. `state.json` in the captures directory reports phase, focused button, current hash and button centers. Phases cover Tab/Enter into Controls, Enter on Back, gameplay/F10 isolation, mouse Help/Back/Start, F10 open/close, F5 save and F9 load. The operator writes `isolation-done` only after delivering and recording the isolation inputs. These are Linux Editor input checks, not native Windows or Player evidence. No arrow-key claim is made.

Both modes retain the original isolated-config and save backup/restoration contracts. The normal `batch`, `bootstrap`, `rendered` and explicit Legacy40 modes remain required. The rendered mode's synthetic keyboard checks remain distinct from the actual-input mode.

## Anchor attribution (Manager-authorised diagnostic)

The startup fixture captures the natural ForestTest scene after eight frames, without regenerating it. It proves that opening/using the title and Start New preserve that fixture's captured fresh world. It does not equate that capture with the established deterministic-reset anchor.

Run the unchanged `Enlarged80Verification` separately for the established start `04A78188A6F8E6F9` and year-3 `081DB58CE3034648` anchors. The diagnostic comparison on canonical base `e8c73f2` and startup HEAD `1edf818` found identical natural-scene captures, while the authoritative reset gate retained both anchors. The logged natural hash is diagnostic evidence, not a replacement anchor. No production code or established anchor changed.

The historical-fixture cleanup hash also waits two frames after LoadData accepts the fresh fixture. LoadData uses Unity's deferred Destroy for trees absent from the restored save, and CaptureData includes inactive trees. An identical base/branch diagnostic reproduced 16 pending historical trees in the immediate capture and an exact fresh-world hash/tree count after two frames. This is a verification timing correction; load semantics and expected world identities are unchanged.

Fresh isolated preferences can show the existing Walking Help introduction after Start New or scene restart. Bootstrap/boundary dismiss that existing panel through `ScenarioOneUiRoot.CloseHelp`, wait for its frame input hold to clear, and require the unchanged fresh hash and restored controls. The actual-input mode instead waits for a real Escape press to dismiss the introduction. This avoids mistaking the existing Help modal for a startup handoff failure. The disposable diagnostic recorded disabled player / enabled save and UI components while Walking Help was open, followed by restored player control and an identical hash after closing Help.
