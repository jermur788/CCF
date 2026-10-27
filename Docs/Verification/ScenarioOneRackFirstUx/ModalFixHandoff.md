# Rack-first modal stacking fix — handoff

Follow-up to `Handoff.md`. Branch `feature/scenario-one-rack-first-ux-v1`, base for this fix `1b1ce97` (code head before docs `cd0692a`), fix commits `7a886a7` (production and rack harness) and `2d9e3fb` (UX-gate expectation, tools only). Production code is identical between those two commits. Docs commit follows them.

## The defect and the rule

Reproduced in the real Linux Player (build `CCF-S1-20261018-L02-cd0692a-SMOKE`, 1920×1080) with the rack first-use window open:

- **Tab** opened the Work Plan behind the window.
- **M** replaced the rack window with the Stand Map first-use window (the rack window was closed without being dismissed).
- **O** closed the window and opened Learning objectives.

Cause: only the help window knew it was open. `ScenarioOneManager.Update` toggled the Work Plan on Tab with no check, and `ScenarioOneUiRoot.HandleKeys` opened or closed the map and objectives on M and O, calling `CloseHelp()` as part of switching screens.

One rule, not a rack-specific case: **while the help and first-use window is open it owns the keyboard.**

- `MenuHelpView` sets `ScenarioOneManager.ModalOpen` when it displays and closes (the single source).
- `ScenarioOneManager.Update` ignores Tab while `ModalOpen`.
- `ScenarioOneUiRoot.HandleKeys` returns after the existing Esc and F1 handling while the window is open, so M and O do nothing.
- Continue, More/Less detail, Esc and F1 work as before and close the window; the shortcuts work again at once. F1 is the window's own toggle (it closes the window, it does not stack another).
- Pointer and wheel input already belonged to the window (the backdrop stops pointer and wheel events); this change does not touch that. Earlier Player runs showed clicks on controls behind a window being ignored; this run tested only the keyboard paths.

No new UI framework, no rack-specific code. Rack geometry, layout, economics, ecology, growth, save schema, Scenario Two and tutorial progression are untouched.

Behaviour change worth knowing: with a screen's own first-use window up (for example the Stand Map's), pressing that screen's key a second time no longer closes the screen; dismiss the window first (Continue, Esc or F1). The UX gate pressed M twice and relied on the old behaviour; its expectation now dismisses the window first.

## Results

| Item | Result |
|---|---|
| Modal stacking reproduced before fix | YES (Player, `modal-fix/player-before/`) |
| Tab blocked while modal active | PASS (Player `player-after/a2`; rack harness `MODAL_BLOCKS_TAB_WORK_PLAN`, `RACK_MODAL_BLOCKS_TAB_WORK_PLAN`) |
| Other panel shortcuts blocked while modal active | PASS for M and O (Player `a3`, `a4`; harness `MODAL_BLOCKS_MAP_SHORTCUT`, `MODAL_BLOCKS_OBJECTIVES_SHORTCUT`, rack-window variants). F1 deliberately closes the window. A second Tab while the Work Plan's own first-use window was open was also ignored (`b3`) |
| Normal shortcuts restored after close | PASS (Player `b2`: Tab opens the Work Plan after Continue; harness `SHORTCUTS_RESUME_*`, `CLOSING_THE_WINDOW_CLEARS_THE_FLAG`) |
| Rack first-year flow | PASS in the Player: B, preview, wheel turn (30°), click confirm ("Rack added to the Work Plan: 32 trees"), Tab (grouped DRAFT rack), Approve Work Plan (APPROVED), Advance one year, Annual Review, acknowledge, Walk the forest (`player-after/c*`, `d*`) |
| Selective marking appears afterwards | PASS: after the year the HUD line changes to "Marked: FELL 0 · CROP TREE 0 · [X] Fell [C] Crop [G] Plant" (`d3`); the rack harness `STAGE2_*` checks pass |

Editor verification (Unity 6000.6.0f1, lane U1, committed tree):

- Compile and import: batch compile exit 0, zero `error CS` (`modal-fix/compile/`).
- Rack harness at `7a886a7`: PASS, **111 hard checks** (96 before, 15 new modal checks), 0 unverified.
- Rendered/UI harness at `7a886a7`: PASS, 119 hard checks (overlap, pick, hit-test at four sizes).
- UX qualification at `2d9e3fb`: PASS, 198 hard checks, 0 unverified. At `7a886a7` it FAILED two checks (`INPUT_MAP_OPENS_AND_CLOSES`, `INPUT_OBJECTIVES_OPENS_AND_CLOSES`, the second a knock-on of the first) for the reason above; the gate expectation was changed, not the behaviour.
- Menu, U4 guidance, Startup, P3, P2, Interaction gates at `7a886a7`: PASS.
- Not run: Tier-3; native Windows. A pre-fix Editor run of the new checks was attempted and abandoned (the harness references the new `ModalOpen` property, so the pre-fix tree does not compile); the pre-fix evidence is the Player run.

Player verification: Linux Player `CCF-S1-20261018-L03-2d9e3fb-SMOKE`, 1920×1080 borderless, fresh config, xdotool with human-speed clicks: the sequence in the task, evidence in `modal-fix/player-after/` (`a-grid.png` Tab/M/O over the rack window, `b-grid.png` dismissal and Tab, `c-grid.png` rack and Work Plan, `d-grid.png` approval, review, marking line).

Save schema changed: NO (version 20). Unrelated production behaviour changed: NO (only keyboard handling while the help and first-use window is open).

Evidence root: `/media/jer/ZX20/CCF-Verification/ScenarioOneRackFirstUx-20261017/modal-fix/` (`player-before/`, `player-after/`, `compile/`, `rack/`, `shots/`, `regression/`, `build/`).
