# Baseline red-gate investigation: Clearance, MenuTutorial, Removal (Workstream F)

**Status:** diagnosis with harness-only fixes committed on this task branch. **No production file was changed.**

## 0. Summary

| Gate | Reproduced on `3e4ee40` (batch)? | Root cause | Class | Product defect? | Fix on this branch |
|---|---|---|---|---|---|
| **ScenarioOneRemovalVerification** | Yes | The harness's in-world **U** assertion still expects the retired species-specific removal order. The accepted clearance correction (`8c5380d`) made U plan a **speciesless area clearance** from the previewed cell | **TEST ASSUMPTION INVALIDATED BY ACCEPTED DESIGN** (stale test) | **No** | **Harness updated to the current contract.** Batch PASS |
| **ClearanceVerification** | Yes | (1) Since `466456f` a fresh profile opens the first-use HUD introduction, which sets `AnyPanelOpen` and, **by design**, suppresses the walking clearance preview. The harness predates this. (2) In batch mode `Cursor.lockState` cannot become `Locked`, and the walking ray exits early when unlocked | (1) **STALE TEST**; (2) **HARNESS ENVIRONMENT** | **No** | **Harness dismisses the introduction (preferences restored) and fails fast in batch mode with an explicit message.** Interactive PASS |
| **MenuTutorialVerification** | Yes | Batch mode cannot deliver keyboard input to play mode: by default the Input System routes keys only to a focused Game View. Even with routing forced, the cursor cannot lock in batch mode, so inspection closes at once | **HARNESS ENVIRONMENT** (+ input/focus) | **No** | **Fail-fast batch guard only.** Unmodified interactive run: full PASS |

**Conclusion:** none of the three red gates indicates a production or teaching defect. Two are environment-dependent (they must run in a rendered, interactive Editor, as their own acceptance records state: "default-rendered"). One was stale against an accepted design. The regression launcher used during the Regeneration Model 1 integration ran all three with `-batchmode -nographics`.

## 1. Reproduction (F1)

**Environment:** Unity 6000.6.0f1 (`f7f8ed4d1e24`), Linux, worktree `/home/jer/CCF-pedagogy` @ `3e4ee40` (clean apart from the staged harness), isolated `XDG_CONFIG_HOME` per run, `systemd-run` 7 GiB memory cap, `nice -n 15`, `-job-worker-count 1`.

**Command shape** (scratchpad launcher `gate.sh`):

```bash
cp Tools/Verification/<Gate>.cs Assets/ForestPrototype/
XDG_CONFIG_HOME=<isolated> CCF_ACCEPTANCE_OUTPUT=<dir> \
  /media/jer/ZX20/Unity/6000.6.0f1/Editor/Unity -batchmode -nographics -projectPath /home/jer/CCF-pedagogy \
  -job-worker-count 1 -logFile <log> -executeMethod <Gate>.Begin
rm Assets/ForestPrototype/<Gate>.cs Assets/ForestPrototype/<Gate>.cs.meta
```

Interactive runs drop `-batchmode -nographics` (`DISPLAY=:0`).

| Gate | Expected | Actual on `3e4ee40` (batch, unmodified harness) | Stack (top frame) |
|---|---|---|---|
| Removal | `SCENARIO_ONE_REMOVAL_VERIFY_PASS` | `SCENARIO_ONE_REMOVAL_VERIFY_FAIL: in-world U did not create a contractor order without uprooting the cohort` | `ScenarioOneRemovalVerification.cs` Verify (U block) |
| Clearance | `CLEARANCE_ACCEPTANCE_PASS` | `CLEARANCE_VERIFY_FAIL walking ray preview did not use authoritative cell query` | `ClearanceVerification.cs` Verify (walking-ray block) |
| MenuTutorial | `MENU_TUTORIAL_PLAYTHROUGH_PASS` | after `MENU_RENDERED_PASS WalkingHud ×3`: `MENU_TUTORIAL_FAIL F1 does not reopen HUD help` | `MenuTutorialVerification.cs` Verify (F1 press) |

All three messages match the earlier baseline logs recorded on `402a2b4` during the Regeneration Model 1 integration, so they predate D-047 as stated.

Additional diagnostic runs (scratch copies outside the repository; not committed):

| Diagnostic | Result | Meaning |
|---|---|---|
| Clearance (batch) logging cursor state right after setting `Locked` | `CLEARANCE_DIAG lockState=None batch=True focused=False` | Batch mode cannot hold a cursor lock. `ForestPlayer.UpdateTreeInspection` returns early unless locked |
| MenuTutorial (batch) forcing `InputSystem.settings.editorInputBehaviorInPlayMode = AllDeviceInputAlwaysGoesToGameView` | F1/Escape now pass, then `MENU_TUTORIAL_FAIL Help Escape also closed inspection` | Keyboard routing was the first blocker. Cursor lock is the second: inspection closes when unlocked |
| Clearance **interactive**, unmodified harness | still `walking ray preview…` FAIL | Not only a batch issue: the first-use help is open |
| Clearance **interactive**, help dismissed (fix) | `CLEARANCE_ACCEPTANCE_PASS` with all A–J, circle, 64-cell matrix and annual markers | Stale-test cause confirmed and fixed |
| MenuTutorial **interactive**, unmodified harness | `MENU_TUTORIAL_PLAYTHROUGH_PASS` (all HUD, inspection, map, waypoint, annual gate, learning objectives and preference markers) | No product or teaching defect |
| Removal (batch), fixed harness | `SCENARIO_ONE_REMOVAL_VERIFY_PASS`, detail `cell=43 density=1.000 cost=675` | Fix confirmed |

### Final verification of the committed harnesses (`3e4ee40` + this branch's harness changes)

| Run | Mode | Result |
|---|---|---|
| ScenarioOneRemovalVerification | batch | **PASS** (`SCENARIO_ONE_REMOVAL_VERIFY_PASS`) |
| ClearanceVerification | batch | explicit fail-fast: "requires an interactive (non -batchmode) Editor…" (intended) |
| ClearanceVerification | interactive | **PASS** (`CLEARANCE_WALKING_PREVIEW_PASS`, `CLEARANCE_FIXTURES_PASS A–J`, `CLEARANCE_CIRCLE_PASS`, `CLEARANCE_REPEAT_MATRIX_PASS cells=64`, `CLEARANCE_ANNUAL_PASS`, `CLEARANCE_ACCEPTANCE_PASS`) |
| MenuTutorialVerification | batch | explicit fail-fast (intended) |
| MenuTutorialVerification | interactive | **PASS** (`MENU_TUTORIAL_PLAYTHROUGH_PASS`, all intermediate markers) |

Interactive runs used `DISPLAY=:0`, an isolated profile, no `--capture` (no screenshots were needed for diagnosis), and staged/removed harness copies. Editor-generated material and settings rewrites were reverted after each run.

## 2. Removal (F5)

- **ROOT CAUSE:** The U block sets the player's aim fields, calls `UpdateUprooting(true)`, and expects a `RemoveRegeneration` order whose `speciesId` equals the selected cohort's species. Since the accepted clearance correction (`Docs/Scenario1ClearanceCorrection.md`: "New area orders use the existing RemoveRegeneration type with an empty species field… Current player controls call TryDesignateVegetationClearance"), `ForestPlayer.UpdateUprooting` in Scenario One creates an order only when the **clearance preview** holds targets. The harness never populated the preview, so no order was created and the assertion failed.
- **Architecture tested by the old assertion:** the retired species-selective walking control. The **API** `TryDesignateRegenerationRemoval(species, cell)` is still valid for legacy/saved callers, and the harness's first half still tests it (passes).
- **PRODUCT OR TEST?** Test (stale against accepted design).
- **SEVERITY:** Low for the product. Medium for process: a permanently red gate hides real regressions.
- **FILES:** `Tools/Verification/ScenarioOneRemovalVerification.cs` only.
- **FIX (committed):** the U block now (a) shows the authoritative cell query on the player's clearance preview — the walking ray cannot run in batch mode, so the same query is supplied directly; (b) presses U; (c) asserts exactly one new order: `RemoveRegeneration`, **empty species**, same cell, open; no cash change; cell regeneration density unchanged; (d) cancels it and clears the preview. Coverage is preserved: it still proves that U plans contractor work without immediate removal or charge, now against the accepted contract.
- **EXPECTED:** `SCENARIO_ONE_REMOVAL_VERIFY_PASS` in batch mode.
- **REGRESSION TEST:** the gate itself, in batch.
- **IMPLEMENTATION RISK:** low. Reflection on the private `UpdateUprooting` and aim fields is the harness's existing approach.
- **OVERLAP RISK:** none with Sol (no ecology/save files). It touches the same harness family as the clearance owner's tests.

## 3. Clearance (F3)

- **ROOT CAUSE 1 — stale test:** `466456f` (menu teaching) added the first-use HUD introduction. `ScenarioOneUiRoot.Update` calls `help.Introduce(WalkingHud)` on a profile without `CCF.MenuHelp.v1.WalkingHud`, and sets the auxiliary panel open. `ForestPlayer.UpdateTreeInspection` shows the clearance preview only when `!scenario.AnyPanelOpen` (correct, accepted behaviour: modal screens must not leave a preview). The harness was written at `8c5380d`, before the introduction existed, and its isolated profile always triggers it.
- **ROOT CAUSE 2 — environment:** in `-batchmode`, `Cursor.lockState` stays `None` (diagnostic above). The walking ray requires `Locked`.
- **Authoritative footprint, preview, approval, exact targets, regeneration/understorey, current semantics:** all pass interactively with the fix (fixtures A–J, circle, repeat matrix, annual). **The accepted area-based clearance model is unchanged; nothing was reverted.**
- **PRODUCT OR TEST?** Test (1) + environment (2).
- **SEVERITY:** as for Removal.
- **FILES:** `Tools/Verification/ClearanceVerification.cs` only.
- **FIX (committed):** dismiss the first-use introduction as a player would (`ScenarioOneUiRoot.CloseHelp()`), assert `!AnyPanelOpen`, and restore all five `CCF.MenuHelp.v1.*` preferences on exit. Add a fail-fast guard with an explicit message when run in batch mode.
- **EXPECTED:** interactive PASS (`run_clearance_gate.py ClearanceVerification --interactive [--capture]`); batch → explicit "requires an interactive Editor" failure, not a misleading assertion.
- **REGRESSION TEST:** the gate, interactive.
- **IMPLEMENTATION RISK:** low.
- **OVERLAP RISK:** none with ecology or save. Note the interaction with Packet 1, which proposes not showing the clearance preview on every ground glance: if Packet 1 is accepted, this harness's walking-ray step must be updated **in the same packet**.

## 4. MenuTutorial (F4)

- **ROOT CAUSE — environment:** with no Input System settings asset, defaults apply. In the Editor, keyboard events reach play mode only when the Game View has focus; in `-batchmode` there is no focused Game View (`SetGameSize`/`Focus()` returns early in batch mode). The first `Press(Key.F1)` is never delivered. With routing forced, the next blocker is the cursor lock (inspection closes when unlocked).
- **Screen introduction state, help state, objective state, UI Toolkit lifecycle, focus, timing, input:** all pass interactively; rendered bounds pass even in batch mode.
- **Actual teaching defect or obsolete expectation?** **Neither.** The harness is valid. It must run interactively, which is how its acceptance evidence was produced (`Docs/Verification/Scenario1WaypointHud/main-integration-results.json`: mode `default-rendered`, PASS).
- **FILES:** `Tools/Verification/MenuTutorialVerification.cs` (fail-fast guard only).
- **FIX (committed):** guard only. **Not done:** forcing Input System routing in the harness. It would let part of the test run in batch mode, but it hides focus behaviour and still cannot pass without a cursor lock.
- **EXPECTED:** interactive PASS; batch → explicit message.
- **REGRESSION TEST / RISK / OVERLAP:** as for Clearance; overlaps any Packet 1 change to help/introduction behaviour.

## 5. Process fix packet (not implemented here)

**Problem:** integration regressions ran every gate with one batch command, so rendered/interactive gates were misreported as red.

**Proposal:** record each gate's required mode in one place and make the launcher refuse a wrong mode.

| Gate | Mode |
|---|---|
| ClearanceVerification | interactive (rendered); `--capture` for images |
| MenuTutorialVerification | interactive (rendered) |
| ScenarioOnePresentationReview | rendered |
| everything else currently in `Tools/Verification` | batch |

Owner: integrator/tooling. Files: `Tools/Verification/run_clearance_gate.py` (or a new small manifest it reads), the Unity Project Overview verification section. **Overlap:** coordination only.

## 6. Side observations from this work

- **Interactive runs rewrite 11 `Art/SectionFive` material files** (texture bindings) and `.vscode/settings.json` in the worktree. They were reverted after every run here. This matches earlier records ("verification-generated fungi import differences were restored"). These are Editor import/normalisation side effects, not changes to keep.
- The batch logs contain an Editor `ArgumentOutOfRangeException` from `UnityEditor.Search.SearchDatabase`. It is benign and unrelated (already noted in earlier records).
