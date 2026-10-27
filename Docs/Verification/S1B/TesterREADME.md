# CCF private candidate: Scenario One, full loop

Build: **@BUILD_ID@**
Source: `@GIT_SHA@`
Platform: Windows x86-64. This is a private technical-test candidate, not cleared for a beginner pilot and not for redistribution. Native Windows behaviour has not been verified yet; your run is part of that check.

## Start

Extract the whole archive into a writable folder, keeping CCF.exe, CCF_Data, MonoBleedingEdge and the supplied DLLs together. No Unity Editor is required. Launch CCF.exe (the program is not code-signed, so Windows may show a "Windows protected your PC" prompt; note it if it appears). The title screen shows the build ID; check that it matches this README.

- Start New Scenario begins a fresh forest. If you already have a saved forest it asks you to confirm; the saved file stays on disk until you save over it.
- Continue loads your saved forest (greyed out when there is no usable save).
- Controls / Help lists the keys. Quit leaves the program; nothing is saved automatically.

## What to play

Play Scenario One through to completion. Completion needs two management cycles. One cycle is: plan work, review its cost, approve it, advance one year, read the Annual Review (choose "I've read the annual results"), then walk the forest and look at what changed. Choose your own forestry decisions; this is not a prescribed solution or an optimisation test. Open the objectives with O to see what is still to do. When every objective is met after the second reviewed cycle, the Annual Review reports "Scenario objectives met" and points on to the Century Review.

Cycle 1, racks first. The plantation follows an authored extraction-rack plan: five evenly spaced racks across the planted rows, with seven rows left standing between racks. Press B in the forest, aim at a planted row (a violet corridor shows the rack), turn it with the mouse wheel and click to add it to the Work Plan. After your first rack you can also open the Stand Map (M) and choose "Plan rack" to add the rest. Press Tab to open the Work Plan: each rack is one group with its trees, timber and income. Check the costs, choose "Approve Work Plan", then "Advance one year". With the five authored racks the first harvest does not cover the contractor's costs, so expect cash to fall after the first year; note the figures if they surprise you.

Cycle 2, selective management. Once the first cycle is reviewed, inspect trees with E, mark trees to remove (X) or Crop Trees to keep and favour (C), plan any planting or clearance you want, then approve, advance and read the Annual Review again.

## Controls

W A S D walk (Shift run, Space jump); mouse look; E or left-click inspect (V for more detail); B extraction rack; X fell mark; C Crop Tree mark; G planting mode; U plan clearance; M Stand Map; Tab Work Plan; O objectives and lessons; F1 Help for the screen you are on; Esc frees the mouse or closes the open panel (click to take the mouse back). While the first-use or Help window is open it takes the keyboard: close it first (Continue, Esc or F1).

F5 saves and F9 loads. F10 opens the session menu and pauses play: "Continue current forest", "Load your same-build save", "Save current forest and return…", "Start a fresh Scenario One test…" (discards unsaved progress; the saved file stays until you save), "Reset tutorial/help progress…" and "Quit CCF…" (asks you to confirm). Quit from the title screen, or through F10 and Confirm. Quitting does not save for you.

## Saves

Save with F5 or from the F10 menu, quit, relaunch the SAME build, and choose Continue (or F9). Use only saves made by this exact build: no save carries over between builds, and saves from other builds may not load. Keep a copy of any save you want to keep before replacing a build. Starting a new scenario does not delete your save, but saving later replaces it.

## Log and bug reports

Windows normally keeps Player.log in `%USERPROFILE%\AppData\LocalLow\DefaultCompany\CCF\` with forest-save.json beside it; this location comes from Unity's defaults and has not yet been confirmed on a real Windows run. To put the log somewhere obvious, start the game from Command Prompt:

`CCF.exe -logFile "%USERPROFILE%\Desktop\CCF-Player.log"`

Copy the log before the next launch overwrites it. It records the build ID and full Git SHA, Windows version, CPU/RAM, GPU and graphics API, resolution and the actual save path. Nothing is uploaded and there is no telemetry; send files to the test organiser yourself. A useful report gives the build ID, Windows version, CPU/RAM, GPU, resolution, display scaling, what you did, what you expected and saw, whether it repeats, and the log. Mention stalls, input failures and crashes with the action that triggered them. Please also say whether all text is legible and whether clicks on buttons and tabs land where you click.

## Known limits

- Save version 20, simulation RNG 1, regeneration model 2, growth model 2; storms are off.
- Native Windows mouse and keyboard, text rendering, display scaling, GPU and driver behaviour, clean-machine launch and performance are unverified until a native run is recorded. Any Wine result is WINE COMPATIBILITY SMOKE, NOT NATIVE WINDOWS VERIFICATION.
- No automated check has played both cycles through to completion with racks first; your run is the first end-to-end one.
- No beginner distribution before Manager review, independent review and the native Windows check pass. Assets and runtime binaries are embedded for this private test: do not redistribute the build, raw art or development sources.
