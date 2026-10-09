# S1-B Windows candidate reproduction

Use Unity 6000.6.0f1 plus its Windows Build Support (Mono) module. Start from the exact committed source SHA recorded by the candidate, in a clean dedicated worktree with its own Library. Import the existing licensed Ultimate Nature – Starter package as documented in Assets/ForestPrototype/Docs/UltimateNatureSetup.md; keep that dependency gitignored and preserve original GUIDs. No new Unity packages are required.

On the current Linux build host:

```
python3 Tools/Build/S1B/build_windows.py --date 20261009 --sequence B01
```

Set `--unity` when the Editor executable is elsewhere. The script requires clean committed source, embeds the full commit and dated build ID in the standalone menu and diagnostic log, builds the enabled ForestTest scene for Windows x86-64/Mono, then removes only its disposable Editor helper and resource stamp. Candidate folder/archive, detailed log, included-assets.txt and SHA-256 package manifest live under ignored Build/S1B. It refuses an existing output folder. Use a new B sequence for another artifact; this does not promise cross-build saves. Build binaries are not committed.

Session controls bootstrap in standalone builds only, preserving ordinary Editor gates. F10 pauses time/player/save shortcut input and hides the existing forest UI while the session menu is open. The scenario input loop yields to that menu. Saving/loading calls the existing save APIs. Restart reloads the same authored scene and retains the saved file; confirmation makes unsaved-progress loss explicit. Reference preview must be closed before opening the session menu, so it cannot be saved through this route. No schema or simulation change is introduced.

Run the accepted S1-A runner for teaching/menu/clearance/removal, focused P2/P3 and the current 24-gate regression; stage/remove disposable sources according to its existing workflow. Record source SHA and environment. Restore only attributable import-generated material/settings normalisations after Editors exit; do not absorb them in the task.

Inspect final included assets, supplied package/runtime notices and archive contents. RightsAudit.md is a working evidence record, not an automatic distribution green flag. Stage A remains conditional until Manager inspection/integration, combined independent review and native Windows technical smoke. Wine can provide supplementary evidence only and must be labelled WINE COMPATIBILITY SMOKE — NOT NATIVE WINDOWS VERIFICATION.
