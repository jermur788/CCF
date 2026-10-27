# Integrated F10 menu triage — Tier 1

Result: **B — stale harness assumption about synthetic Editor input; harness qualified.** Current candidate Player/native F10 remains **UNVERIFIED**, not a proven production failure. No production fix is supported by the available evidence. The precise reason queued F10 state events do not toggle this Editor menu is not established by Tier 1.

Base: `bc81627e8ca47fb619f6fbabcbf618dba0508e11`.
Branch: `fix/integrated-f10-menu-v1`.
Worktree: `/media/jer/Files/CCF-f10-triage-v1` (sparse, code-only; no shared Library).
Packet context: `c6dc136b7f1f8668d9f67f95f48968d290ad5067`.
The commit containing this report is the delivered HEAD; the final worker response supplies its full SHA.

## Exact failure and what it exercised

Both archived logs under `/media/jer/Files/CCF-tier2-smoke-20261015-out/` contain:

```text
UXQ_FAIL INPUT_SESSIONMENU_OPENS_AND_CLOSES SessionMenu: opened=False closed=True playerEnabled=True cursorLock=Locked cursorVisible=False playerMoved=False
```

- `ux-resume/ux-resume.log`: 102 passed assertions, one failure, 24 screenshots.
- `ux-f10-focused/ux-f10-focused.log`: zero passed assertions, one failure. Its external fixture called `EnsureSession()` first, so a missing component alone does not explain the repeated failure.
- Both printed `UX_QUAL_FAILED`, then `UX_QUAL_DONE_MARKER`, and exited 0. Their old JSON receipts incorrectly said PASS. Original logs/receipts were preserved; the revised result parser correctly classifies both archived runs FAIL. A failed synthetic assertion is a failed old harness run, not proof of a production interaction defect.

The path was `InputSmoke` → `Press(Key.F10)` → `InputSystem.QueueStateEvent(Keyboard.current, new KeyboardState(key))`, two coroutine yields, then a queued empty state and two more yields. No OS keyboard event or physical key was supplied. Runtime `StandaloneSessionMenu.Update()` polls `Keyboard.current.f10Key.wasPressedThisFrame`, guards `!startupScreen`, and calls the existing `Open()`/`Close()` methods. Button tests elsewhere invoke registered UI callbacks; those also do not establish keyboard delivery.

Editor direct-entry intentionally does not bootstrap the menu unless `CCF_STARTUP_SCREEN=1`; a Player always bootstraps the startup screen. The selected UX subset omitted RESET_FLOW (which normally calls `EnsureSession`). The missing setup is corrected for future subset runs, but the failed focused follow-up shows it was not the sole limitation.

## Accepted behavior and earlier actual-input evidence

Accepted behavior: F10 does nothing on the title; after Start/Continue, F10 opens the full session menu, pauses and isolates forest controls; F10 closes it and restores control. See [Startup verification handoff](../StartupScreen/VerificationHandoff.md).

That handoff explicitly records **synthetic graphical F10 delivery failure**, qualifies those checks SKIPPED/UNVERIFIED, and separately records an **actual Linux X11 desktop-input pass**. Its [real-input receipt](../StartupScreen/Evidence/StartupScreen/startup-real-input.json), tested HEAD `f19f2170025356d0fac65d41da9bdddcbb5b0d73`, contains:

```text
STARTUP_CHECK real input completed: stand-f10-open
STARTUP_CHECK real F10 opens full session card
STARTUP_CHECK real input completed: stand-f10-close
STARTUP_CHECK real F10 close: menu closed and time running
STARTUP_CHECK real F10 close: player, save shortcuts and forest UI handed back
STARTUP_REAL_INPUT_VERIFY_PASS mouse Tab Enter Back isolation F10 F5 F9
```

The earlier `StartupScreenVerification.InputFlow()` waits for actual desktop interaction; it does not queue InputSystem events or invoke buttons. The receipt describes Linux Editor X11 desktop events, not Windows or proof of a human physical keyboard. Its launcher exited 143 after functional PASS/normal Unity shutdown, so this is functional evidence with a launcher/cleanup caveat, not a clean-launcher or Player-exit claim.

The existing `StartupScreenVerification.NoSaveFlow()` already uses failed synthetic F10 as a delivery control: record unverified, exercise `Open()` directly, and require separate actual input. The UX harness had not retained that evidence boundary.

## Candidate integration comparison

Compared the earlier real-input HEAD with the exact candidate base:

- `StandaloneSessionMenu.Bootstrap`, `Update` (the full F10 handler) and `Open` are byte-identical sections.
- `Close` only additionally clears `sessionNote`; input/pause/restore operations are unchanged.
- Other session-menu changes are wording and tutorial/help reset controls (`1285411`, `0021925`, `e9592dc`). Startup changes replace the local controls-copy array with shared `ControlsReference.Rows`.
- Both package manifests/locks, `ProjectSettings.asset` and `InputManager.asset` are byte-identical. Input System remains 1.20.0.

This rules out a changed F10 handler/configuration as the observed integration delta. It does not prove current native behavior or exclude every scene/lifecycle interaction. No new native input was exercised during this Tier-1 triage.

## Changes

Only verification files changed; no production menu, scene, input architecture, package, save or density change.

1. UX harness ensures the session component exists for the F10 subset, checks synthetic open/close, and records `UXQ_UNVERIFIED` when delivery is not demonstrated. It independently hard-checks direct menu opening, pause/input isolation, closing and restored control. Direct calls are explicitly not keyboard passes. RESET_FLOW uses the same qualification boundary when queued F10 cannot open/close it. Other hard assertions remain failures.
2. Added `CCF_UXQ_PHASES=F10_INPUT` for any later focused Editor check; it does not run the broad UX matrix.
3. UX harness now exits nonzero on assertion failure, not just exceptions.
4. Generic qualification runner retains `UXQ_` markers, rejects `UXQ_FAIL`/`UX_QUAL_FAILED` even with exit 0/completion marker, and emits `PASS_WITH_UNVERIFIED` plus `unverified_checks` when appropriate. Exit 0 for that qualified status means execution/mechanical checks passed; it does not mean native input was verified. Assertion failures take precedence over unverified checks.

## Tier-1 validation

- Five Python receipt regression tests PASS: archived F10 failure with exit 0, assertion without summary, explicit unverified result, unverified plus an independent hard failure, unchanged success behavior, and launch/compile/tree failures.
- Replayed the result classifier against both complete archived failure logs: FAIL in both cases. No archived evidence was rewritten.
- Updated C# disposable harness compiles with Unity's Roslyn compiler against the existing candidate's reference assemblies: exit 0, zero C# errors. This single-file compile writes only to external output; it does not launch the Editor or modify Library. Sixteen warnings: eleven obsolete API uses, four local-vs-imported harness type warnings (the prior test assembly also contains the old disposable types), one netstandard reference compatibility warning. No runtime pass is inferred.
- `git diff --check` PASS. No Unity run, broad UX rerun, Player build or Windows execution.
- Earlier verification worktree's ten pre-existing tracked modifications remain byte-identical to the takeover snapshot. Main/candidate source branches and existing evidence were not altered.

Evidence and compiler/test outputs: `/media/jer/Files/CCF-f10-triage-20261015-out/` (`evidence.json`, `typecheck-result.json`, `f10-harness-typecheck.log`, test log). Section hashes and archived log SHA-256 values are recorded there.

## Decision and remaining check

Remove the synthetic F10 failure as proof of a production defect. Keep current candidate **Player/native F10 UNVERIFIED** until actual-input smoke: launch the actual Player, confirm F10 cannot leave the title, enter the forest, press F10 to open the full session card, confirm paused/isolated controls and free cursor, then press F10 to close and confirm time and forest control restore. Perform the native Windows portion on Windows; Linux Editor results cannot substitute.

The updated C# qualification path is compile-checked, not newly runtime-verified. If an Editor follow-up is needed later, run only `F10_INPUT` using the documented disposable staging workflow and isolated config; do not copy its script into a playable build. A `PASS_WITH_UNVERIFIED` result still requires actual input. No fresh Unity run is required to establish the evidence-boundary correction in this Tier-1 task.

The separate density concern from the previous Tier-2 handoff is unaffected. This triage does not authorize or establish overall Tier-3 readiness.
