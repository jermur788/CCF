# Scenario One UI Visual System V2 — disposable render harness

Test-only harness for `SCENARIO-ONE-UI-VISUAL-SYSTEM-V2`. It is **not** auto-started, is not a
build/player component, and exists only for a deliberate `-executeMethod` run.

- Harness source: `Tools/Verification/ScenarioOneVisualSystemV2/VisualSystemV2Shots.cs`
- Comparison tool: `Tools/Verification/ScenarioOneVisualSystemV2/make_comparisons.py`
- Installed path used by Unity: `Assets/ForestPrototype/VisualSystemV2Shots.cs`
- Install/remove by copy only. Remove the copied `.cs` and its generated `.cs.meta` after the run.
- No repo, scene, prefab, ProjectSettings, save or package file is modified.

## What it captures

Real `ScreenCapture.CaptureScreenshotAsTexture` renders at **1280x720** and **1920x1080**:

- startup session title and its Controls / Help card;
- normal walking HUD;
- HUD minimap (cropped from the same real render);
- waypoint HUD;
- selected-tree inspection (estimate panel);
- Crop Tree competitor assessment, selected numbered tag and resolved contrast (required);
- Objectives;
- Work Plan with the current-stage locked section;
- in-game Help (walking HUD);
- F10 session menu;
- Stand Map and its overview;
- Annual Review after the existing `ui.AdvanceFromWorkPlan()` path.

Every shot asserts the requested render size and the actual texture size, checks the capture is
not blank, audits major panel/title containment and one-line text fit, and records computed
text/background colours plus font asset names to JSON and CSV.

## State, input and hash checks

- Normal startup → `StartNewScenario()` → existing introduction closed → HUD.
- No teaching preferences are injected. First-use help that opens over a shot is closed through
  the existing `ui.CloseHelp()` before the clean capture.
- Work Plan, Objectives, Stand Map and Annual Review route through
  `ScenarioOneManager.SetWorkPlanOpen` / `SetAuxiliaryPanelOpen` → `ApplyPanelInputState`
  (`player.enabled = !AnyPanelOpen`; cursor unlocked and visible). The harness asserts that real
  manager/player/cursor state, then the restored walking state (player enabled, cursor locked).
- Help sets `manager.ModalOpen`; `ScenarioOneUiRoot.ShowHelp`/`Update` also sets the auxiliary
  panel flag, so `ApplyPanelInputState` disables the player and frees the cursor while the UI
  root remains enabled. The harness asserts that complete call path.
- The F10 session menu is the one path that disables the player and UI root and pauses time; the
  harness asserts those flags, the free cursor while open, and that Close restores player/UI/time
  (Close does not restore the cursor, so that is recorded rather than asserted).
- World hash checkpoint before and after pure UI navigation (HUD, waypoint, inspection,
  Objectives, Work Plan, Help, session menu, Stand Map, minimap) must be identical.
- Competitor evidence temporarily marks a Crop Tree and one neighbour with the normal marking
  API, then restores their original marks and asserts the world hash is restored. A numbered
  competitor is selected through the real number-key handler (queued New Input System event).
  If that key is not delivered, the accepted harness technique sets `SelectedIndex` only so the
  selected-tag visual evidence still exists; the key failure remains recorded.
- The selected competitor tag is rendered in the world layer. The harness captures the full HUD
  with the selected tag plus a padded crop of it, asserts the selected state/number, and computes
  WCAG contrast from the tag's own resolved text/background colours (no palette hard-coding).
- `ui.AdvanceFromWorkPlan()` must increment ecological year and report count by exactly one; review/UI capture afterwards
  must not change it again.
- No disk Save is called.

## Run — safe launcher (normal startup + isolated XDG)

Use the existing qualified launcher from the project root. `--normal-startup` is mandatory: it
keeps `CCF_TEST_DIRECT_ENTRY` unset so the real Bootstrap startup screen opens. The launcher also
creates `XDG_CONFIG_HOME=<out>/config`, copies the Unity licence there (never uses the user's
licence/config), runs under a private Xephyr display, and removes the staged harness file and its
`.meta` in its `finally` cleanup.

```bash
OUT=/media/jer/Files/CCF/Verification/ScenarioOneUiVisualSystemV2-$(date -u +%Y%m%dT%H%M%SZ)
python3 Tools/Verification/ScenarioOneVisualSystem/run_gate.py \
  --name ui-visual-system-v2 \
  --method VisualSystemV2Shots.Begin \
  --files ScenarioOneVisualSystemV2/VisualSystemV2Shots.cs \
  --out "$OUT" \
  --env CCF_UI_SIZES=1280x720,1920x1080 \
  --normal-startup --private-display --screen 2560x1440 --timeout 1800
```

The launcher sets `CCF_DIAG_DIR="$OUT/evidence"` and `CCF_ACCEPTANCE_OUTPUT="$OUT/evidence"`.
The runtime installer only activates when the command line contains exactly
`VisualSystemV2Shots.Begin`. The harness exits Play Mode and returns `0` on pass, `1` on failure.

If invoking Unity directly instead, do not use the user's config; set an isolated
`XDG_CONFIG_HOME` and `CCF_DIAG_DIR` yourself. The launcher path above is preferred.

### Mandatory cleanup

The launcher removes the staged file, but verify before handoff:

```bash
test ! -e Assets/ForestPrototype/VisualSystemV2Shots.cs
test ! -e Assets/ForestPrototype/VisualSystemV2Shots.cs.meta
git status --short   # no VisualSystemV2Shots entry
```

Unity may still rewrite `.vscode/settings.json` and the SectionFive forb materials. Do not commit
those; restore only those exact unrelated paths if they changed. Never leave the harness installed
in `Assets/` or in a playable build.

## Section 14.1 comparison sheets

`make_comparisons.py` builds one `APPROVED REFERENCE | ACTUAL UNITY RENDER` PNG per real harness
screenshot. It is read-only over the renders and refuses to create anything when no real PNGs
exist. The actual capture is pasted unmodified at native size; the locked reference is fitted with
aspect-preserving letterboxing; labels and SHA-256 prefixes are drawn on the sheet. The output is
`2 × actual width + narrow divider` so the actual panel is never rescaled.

```bash
python3 Tools/Verification/ScenarioOneVisualSystemV2/make_comparisons.py \
  --reference specs/scenario-one-ui-visual-system-v2/references/griffins-peak-ui-reference.png \
  --renders "$OUT/evidence" \
  --out "$OUT/comparisons"
```

The locked manifest SHA-256
`1f93c9062b0188834f968c4caeb4a63d70ed48c1ca7f05ddc8c6cd84557b7c4a` is enforced by default. The
script writes `comparison-manifest.json` (source/actual/output hashes) and
`comparison-sha256.txt`, or exits non-zero without images if the reference differs or the render
set is empty/unreadable.

Required log markers:

```text
VISUAL_V2_CHECK_PASS ... / VISUAL_V2_CHECK_FAIL ...
UI_VISUAL_SYSTEM_V2_HARNESS_PASS shots=<n> passes=<n> failures=0
```

or:

```text
UI_VISUAL_SYSTEM_V2_HARNESS_FAIL shots=<n> passes=<n> failures=<n> exception=<...>
```

## Outputs

Written only to `CCF_DIAG_DIR`:

- `<screen>-<width>x<height>.png`, including `hud-tree-competitors`,
  `hud-tree-competitors-selected` and the padded `competitor-tag-selected` crop;
- `visual-system-v2-result.json` (verdict, hashes, captures, notes, console errors);
- `style-audit.json` (same structured payload, convenient name);
- `style-audit.csv` (per visible text/panel style rows);
- `notes.txt`.

## Explicit limitation

This is callback/API smoke through the real UI state and renderer, **not OS-native input**. It
does not deliver OS-level keyboard or mouse events, so it does not prove click targeting, hover,
focus routing or OS key delivery. The competitor number key is queued through the project's real
New Input System handler, which proves that handler and the selected style/state, not an OS
keypress. OS-level behaviour remains for a separate real-input or player run.
