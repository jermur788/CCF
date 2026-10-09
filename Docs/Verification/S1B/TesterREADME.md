# CCF private first-cycle candidate

Build: **@BUILD_ID@**
Source: `@GIT_SHA@`
Platform: Windows x86-64. This is a private technical-test candidate, not yet cleared for a beginner pilot. Native Windows technical smoke and performance remain open release gates.

Extract the whole archive into a writable folder, keeping CCF.exe, CCF_Data, MonoBleedingEdge and the supplied DLLs together. Launch CCF.exe; confirm the build ID on the initial menu. No Unity Editor is required. Use Start a fresh Scenario One test and confirm to enter the forest. Continue current forest resumes the current forest. This creates a fresh forest without deleting your saved file. Confirming Save replaces the single saved forest; do not overwrite a save you need to keep.

Observe one management cycle: inspect the forest, choose trees to favour or remove, mark/plan work, review costs and the residual stand, approve work, advance, read Annual Review, then walk and observe the changed forest. Choose your own forestry decisions; this is not a prescribed solution or an objective-optimisation test. Stop after post-intervention observation. Later repeated interventions are outside this pilot.

Controls: WASD walk, mouse look, Shift run, E inspect, X fell mark, C crop-tree mark; F1 Help, O Learning, M Stand Map, Tab Work Plan. Close Help before operating the underlying screen. Esc releases the mouse or closes the current screen; click to capture the mouse again. F10 opens the session menu and pauses player control. Learning introductions remain remembered on this device; revisit them with Help/F1.

Save with F5 or the session menu. Quit through F10 → Quit → Confirm; quitting does not save automatically. Relaunch the SAME executable/build, select Load your same-build save, confirm identifiable state, then continue an ordinary interaction or year advance. F9 also loads the existing save. Use only saves created by this exact build. No cross-build persistence or compatibility is promised. Keep another copy of a save before replacing a build or deliberately overwriting it.

For a new test: F10 → Start a fresh Scenario One test → Confirm. Current unsaved forest progress is discarded, but the saved file remains until you choose Save. The existing Help/Learning preferences are retained. To quit after restarting, use F10 → Quit → Confirm.

Windows Unity normally writes Player.log under `%USERPROFILE%\AppData\LocalLow\DefaultCompany\CCF\Player.log`, and forest-save.json beside it. This default location is derived from Unity documentation and current company/product settings; access from an actual Windows run must still be verified. To make log retrieval explicit, launch from Command Prompt with `CCF.exe -logFile "%USERPROFILE%\Desktop\CCF-Player.log"`. Preserve that log before another launch overwrites it. The log contains build ID/full Git SHA, OS, CPU/RAM, GPU/graphics API, resolution and actual persistent-data path. There is no telemetry or automatic upload.

A useful bug report includes build ID, Windows version, CPU/RAM, GPU, resolution, what you did, expected/actual result, whether it repeats, Player.log and (if relevant) a copy of the same-build save. Send these manually to the test organiser. Mention stalls, input failures or crashes and the interaction that triggered them.

Known limits: Save 19 / RNG 1 / regeneration 2 / growth 1 / storm 0; storms remain dormant. Native Windows mouse/keyboard, GPU/driver behaviour, clean-machine launch and performance are unverified until the technical smoke is recorded. Any Wine evidence is WINE COMPATIBILITY SMOKE — NOT NATIVE WINDOWS VERIFICATION. Stage A is conditional; no beginner distribution before Manager review, independent pre-pilot review and the native Windows gate pass. Assets and runtime binaries are embedded for this private test; do not redistribute raw art or development sources.
