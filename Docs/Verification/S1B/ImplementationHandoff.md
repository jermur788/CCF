# S1-B Standalone-Build Readiness — implementation handoff

**Implementation complete / required local verification green. Ready for Manager review: YES. Stage-A standalone readiness: CONDITIONAL. Remaining release gate: native Windows technical smoke.** Wine visual text rendering is partial and is explicitly included in that remaining gate; no native Windows acceptance is claimed.

Task: S1-B Standalone-Build Readiness. Branch: `task/s1-b-standalone-readiness`.
Base/context: `4a68d528bb9d01036de9d4df1739b7de2c8c6002`.
Implementation/build HEAD: `b3ba9b8900b1f62b721aee6508aba6d64db93bfd`. The final branch tip adds evidence/documentation only; its SHA is reported with the handoff. Editor gates ran at `85e3a80a21731ecc511e1a1b868e539713d29e52`; `git diff 85e3a80 b3ba9b8900b1f62b721aee6508aba6d64db93bfd -- Assets Packages ProjectSettings` is empty. Verification began 2026-10-09; final records were completed 2026-10-10 (Europe/Dublin). B06 retains its explicitly assigned 20261009 test-series date.

## What changed / player-facing result

A standalone-only session menu exposes the exact build ID, fresh Scenario One, continue, existing save/load APIs, explicit unsaved-progress/reset and overwrite confirmations, and normal quit without autosave. F10 pauses controls/time and hides the forest UI; a single ScenarioOneManager input guard yields while this menu owns input. Reference preview cannot be saved through the session menu. Restart reloads the authored ForestTest scene and retains saved bytes and local Help/Learning preferences.

Build tooling targets Windows x86-64/Mono with Unity 6000.6.0f1 and the existing licensed Ultimate Nature dependency. It stamps clean committed source; stages/removes disposable helper/identity/diagnostic sources; restores reviewed Unity-generated source bytes; separates diagnostic and tester Players; excludes generated performance-test metadata, four empty-job package test assemblies and DontShip backups; copies notices; generates a SHA-256 manifest/archive. The helper checks all remaining managed assemblies have zero references to the excluded test DLLs. Installed packages/asmdefs are unchanged.

The private build temporarily opts out of Unity Engine Diagnostics, checks effective statistics/upload flags and restores the original source setting. Both generated build configurations have UnityConnect, PerformanceReporting, Analytics, CrashReporting and Insights false. Local Player.log hardware/build diagnostics remain available. No CCF telemetry or automatic bug-report upload was added.

Files: new `Assets/ForestPrototype/UI/StandaloneSessionMenu.cs` and its meta; one-line guard in `Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.cs`; `Tools/Build/S1B/`; `Tools/Verification/S1B/`; `Docs/Verification/S1B/` including records. Serialized Unity files committed: only the new script meta. No scenes, prefabs, materials, settings or package manifests were changed in the branch.

Simulation/save impact: none. Save 19 / RNG 1 / regeneration 2 / growth 1 / storm 0; StormModel1 remains dormant. No objectives/completion/economy/ecology/calibration, multi-species density, coppice/pollard or S1-C/S1-D implementation change.

## Exact candidate / reproduction

Build ID: `CCF-S1-20261009-B06-b3ba9b8`. Full compiled source: `b3ba9b8900b1f62b721aee6508aba6d64db93bfd`.
Folder: `/home/jer/CCF-s1b/Build/S1B/CCF-S1-20261009-B06-b3ba9b8-Windows-x64/`.
Archive: `/home/jer/CCF-s1b/Build/S1B/CCF-S1-20261009-B06-b3ba9b8-Windows-x64.zip`.
Archive bytes: 536599724; SHA-256: `c7d4da9d5316c3bb419b39f16e84827bc1a41ab07c75ade01316d063a1d22188`.
The archive includes a completed post-build rights/content audit; compiled binaries/build identity remain from the exact source above. Compiled binaries are ignored and not committed to Git.

From clean source at that SHA with its own Library, installed module/licence and the existing licensed pack, the tested build command was:

```sh
python3 Tools/Build/S1B/with_demo_hdr_quarantined.py python3 Tools/Build/S1B/build_windows.py --date 20261009 --sequence B06
```

The wrapper's HDR exclusion is authorised only for this S1-B checkout. Do not apply it to another checkout without the appropriate ownership/approval. It restores the exact pair after Unity exits. Do not reopen Unity just to check clean status after restoration. Output-folder reuse is refused; choose another B sequence for another artifact. Rebuilding from the final evidence-only branch tip will use that new tip in its identity; check out the recorded candidate SHA to reproduce its source identity. ZIP byte-for-byte equality is not promised. Final post-build audit/manifest completion is recorded in `Records/package-audit.json` and the packaged notices.

## Required local gates

| Gate | Result |
|---|---|
| Unity import/compile with approved HDR quarantine | PASS, zero compiler errors |
| SessionMenuVerification | PASS, pause/input isolation/read-only hash/restore/reference-preview guard |
| S1-A Teaching Copy batch + rendered | PASS |
| Accepted MenuTutorial fixture | PASS targeted + full-regression independent run |
| Clearance + Removal | PASS targeted + full regression |
| P2 focused batch ×2 + interactive | PASS, F58FB0B1A421D28B |
| P3 focused batch ×2 + interactive | PASS, F7C2FC966816BBAD |
| Full current regression | 24/24 PASS; all established modern anchors unchanged |
| Model2 compatibility | 702766DECE591E21 unchanged |
| Reference Future year 100 | 7AD177B3CC2F73C7 unchanged |
| Reference continuation | 9CDF21A541C5968D unchanged |
| B06 Windows tester build | PASS, 0 errors, 170 warnings, 108.77 s cached build |
| Separate B06 diagnostic build | PASS, 0 errors, 177 warnings, 69.68 s |
| Remaining managed dependency audit | PASS, 0 references to excluded package test DLLs |
| Effective private-build service flags | PASS, all five false |
| Package manifest/archive/content/rights audit | PASS |

Warnings include existing obsolete API/unused-field warnings and Editor build-time JobTempAlloc diagnostics. These are not native Windows performance evidence. Earlier import OOM and rejected B01–B05 packaging/helper iterations remain as ignored local logs; they are not the current candidate.

## WINE COMPATIBILITY SMOKE — NOT NATIVE WINDOWS VERIFICATION

Host: Linux, Wine 9.0, Intel i5-6300HQ (4 cores), ~15.5 GiB RAM, physical Intel HD 530 + NVIDIA GTX 980M. Wine reports NVIDIA GTX 980 / Direct3D 11.0 level 11.1; this is not a native GPU/driver claim. Window 1280×720. An isolated task-owned Wine prefix was used; existing Wine settings/saves were preserved.

The actual tester executable starts and logs `CCF-S1-20261009-B06-b3ba9b8` and the full SHA. The bounded 35.3 s launch observation was then deliberately terminated; exit 1 from that termination is not normal-quit evidence.

The separate diagnostic Player (`CCF-S1-20261009-B06-b3ba9b8-SMOKE`, same source/runtime code plus staged API fixture) completed a fresh year-zero game, Help/Learning/Map/Work Plan API routes, selected management work, approval, year-one Annual Review, post-intervention observation, save and normal quit (exit 0, 14.8 s). A fresh process running the same diagnostic executable loaded the exact saved world hash `1CFD3EE7279EC000`, advanced ordinarily to year two, reset to a fresh year-zero test without changing saved bytes, and quit normally (exit 0, 15.4 s).

**Visual limitation:** screenshots have forest/panels/buttons but no readable UI text. Player.log repeatedly reports inability to load the built-in `LegacyRuntime` font face; the Wine prefix's Windows Fonts directory is empty. The effect on native Windows is unknown. API PASS does not prove readable Help, labels or visible build ID. No font/package dependency was introduced to repair Wine. Require readable build ID and all UI text on native Windows before release; if native Windows reproduces the issue, investigate the bounded runtime-font dependency with rights/context review.

Physical keyboard/mouse input, walking/inspection usability, native GPU/driver behaviour, clean-machine readiness and Windows FPS/performance are NOT verified. The automated API durations are functional smoke timings, not initial-load/FPS benchmarks. No crash occurred in the two diagnostic flows. The external time tool measured the Wine launcher rather than complete Player memory, so no Player RAM acceptance is claimed. Severe native hitching/performance remains open.

Actual Wine save path: `C:/users/jer/AppData/LocalLow/DefaultCompany/CCF\forest-save.json`.
Explicit Player.log override was produced/read at `/home/jer/CCF-s1b/Build/S1B/wine-evidence-b06/` for candidate/save/load runs. Native default `%USERPROFILE%\AppData\LocalLow\DefaultCompany\CCF\Player.log` comes from Unity documentation/company-product settings; native location/access must be confirmed. README provides a Desktop `-logFile` override and manual bug-report fields.

## Rights, HDR and final source audit

Rights: CLEAR FOR PRIVATE TEST for owner-confirmed project-created/generated art and embedded licensed Ultimate Nature content, subject to the existing Unity licence/tier terms. Required runtime/package notices are supplied (40 licence/notice files plus completed audit). No unresolved included-content redistribution-rights blocker was identified; this is not public distribution approval. Included assets/source-path origins and final manifest hashes are in the committed package audit. No raw source/Unity package/licence secrets/saves/PDB/DontShip output is bundled. A few Unity MonoScript metadata paths are retained for package test source types whose DLLs are excluded; no raw C# or executable test assemblies ship.

“UNS_HDRI.hdr was excluded from the task-local Unity import/build environment because its 16K × 8K import repeatedly exhausted available memory. Dependency audit found no CCF reference to its GUID. It remains preserved in canonical source/package content and was not deleted by S1-B.”

GUID: `83d5c6c30d68b7e4498cfef8c6e2a35a`. Audit: 4 tracked scenes, 143 prefabs, 94 materials and relevant settings/package/build files contain no dependency on it. The pack's own demo skybox references it; that demo skybox and HDR are absent from the Windows build report. All six required pack prefab/meta pairs match original cached package bytes/GUIDs. Both HDR source files are restored with original SHA-256s:
- HDR: `8d19cc94c96d796e050230b1b6c745355ffcadf8fa5c32c1a649dca20e0fb88d`.
- Meta: `5b06744c00c2ebf478db3e175d4cf847225ca8f1526d81c447a4b09755e1dd99`.
Original paths/quarantine location/checksums are in `Records/hdr-restoration-record.json`. No missing CCF references attributable to the HDR omission were found in import/build/Player evidence. Font errors are a separate Wine observation. No Unity operation was run after final restoration.

Tracked Unity-generated editor/material/build-setting changes were reviewed, recorded in ignored diffs and restored in this task checkout. Final source has no disposable Assets helpers/metas/resource stamps, scene/prefab/material/settings/package drift, HDR deletion/modification or unrelated changes. `/home/jer/CCF-main`, unrelated materials/settings/recovery files, dirty worktrees and the licensed source package were not modified. Canonical context remains 4a68d52; S1-B has not been integrated and no mirrors/project context were refreshed.

## Worker assessment / next step

Ready for Manager review: **YES**. Stage-A standalone readiness: **CONDITIONAL**, not ready for beginner distribution. Required local implementation/regression/package/rights work is green; optional Wine rendering is partial. Unresolved issues: native Windows technical smoke (especially font/readability, actual input/walking and performance) and subsequent integration/combined review. No gameplay decision is needed from this worker handoff.

Manager inspects exact branch/commit → integrates/verifies as appropriate → planned Sonnet 5.5 xhigh independent review of combined S1-A/S1-B → genuine blockers only → native Windows technical smoke with candidate ID/hardware/log/repro record → beginner Stage-A pilot only after that gate passes.
