# Minimal startup/title screen — verification handoff

**Ready for Manager inspection.** No implementation change was required during diagnosis/verification. The ported production and Player smoke files remain byte-identical to the Claude source. This is Editor verification; the real Player path and native Windows/performance remain later release gates.

- Task: port + verify minimal Scenario One startup/title screen.
- Branch/worktree: `task/scenario-one-startup-screen`, `/home/jer/CCF-startup-verify`.
- Base/context: `e8c73f25e6d93db818abdc6c794887f8f7c5ba43`.
- Claude source HEAD: `cfb8c173cee5ed758c0479576220e1bea8f80de2`.
- Ported commits: `d19fa8e6c627e01cbf8cccdfd9b14b0671c7cfe9`, `4dcb86340004cc27830a6f8ab9c93623191aca63`.
- Diagnostic starting HEAD: `1edf818866fe384e25440f86d62dbd5bdee05828`.
- Final verification-code HEAD: `06f28e40898e7c799331adf0a4dbcf3764a0a73a`. The following evidence commit changes verification documentation/evidence only; its exact SHA is supplied in the final worker handoff.

| Required check | Result |
|---|---|
| Static review | PASS: one existing session/menu architecture; Editor direct-entry preserved; Player title/F10 guard; existing validated Load; read-only CanLoad; no save deletion; no calibration/geometry/model values in title; non-prescriptive controls |
| Unity 6000.6.0f1 import/compile | Zero C# errors / zero new importer errors; exact accepted Search exception only |
| Startup batch / bootstrap / explicit Legacy40 batch | PASS / PASS / PASS |
| No-save / existing-save / E80 Continue / Legacy40 Continue | PASS; save kept by Start New confirmation and Back |
| Invalid/rejected Continue / changed-file revalidation | PASS; world and geometry unchanged on rejection |
| CanLoad no-mutation | PASS: no-save, valid current v20, historical Legacy40, malformed JSON, duplicate-ID rejection; separate diagnostic also compares primary/backup/temp bytes |
| Controls/Help and F10 session menu | PASS; title F10 ignored, full stand session menu retained |
| Confirmed fresh-test restart | PASS: one menu host, one overlay, exact fresh identity, restored controls |
| Actual Linux Editor input | Functional PASS: real mouse Help/Back/Start; Tab/Enter/Back; gameplay/F10 isolation; stand F10; real F5 save and exact F9 restore |
| SaveHardening / StandGeometry / Enlarged80 / Reference | PASS / PASS / PASS / PASS |
| Existing SessionMenu / MenuTutorial / Interaction | PASS / PASS / PASS |
| AreaCalibration | PASS; E80 targets 240 retained, 12 regen, 0.08 deadwood; century 480 originals, 40 broadleaf per species, 48 regen, 2.0 deadwood |
| Complete normal Legacy40 regression | **24/24 PASS**, all established hash sets unchanged |
| Rendered 1280×720 / 1920×1080 | PASS / PASS; eight title/no-save/save/confirmation/controls captures visually inspected, legible with no clipping or overlap |

| Required compatibility anchor | Exact result |
|---|---|
| Enlarged80 START | `04A78188A6F8E6F9` |
| Enlarged80 YEAR3 | `081DB58CE3034648` |
| P2 (batch-only, twice) | `F58FB0B1A421D28B` |
| P3 (batch-only, twice) | `F7C2FC966816BBAD` |
| Model2 | `702766DECE591E21` |
| Reference Y100 | `7AD177B3CC2F73C7` |
| Reference compatibility continuation | `9CDF21A541C5968D` |

## Attribution and verification-only changes

The hash diagnostic established **Case A**. The same eight-frame capture-only harness produced `EEED1DE3EDC65517` on canonical base and startup branch; the unchanged authoritative reset gate retained both established Enlarged80 anchors. Natural captures contain 1344 trees, geometry 1, year 0, zero serialized save cells and 256 live ecology cells. This diagnostic value is not a new anchor. See [HashDiagnosticHandoff.md](HashDiagnosticHandoff.md).

Verification changes from Claude source are confined to `Tools/Verification/Startup/{StartupScreenVerification.cs,run_startup.py,README.md}` and this verification documentation/evidence. They add read-only/preflight, revalidation, actual-input and restart checks; separate the natural-capture contract from the reset anchor; wait for deferred tree destruction before cleanup hash checks; account for the existing Walking Help modal; and explicitly mark undelivered synthetic-key checks unverified. Production/save semantics/anchors were unchanged.

Synthetic keyboard delivery failed its F10 control in the graphical Editor. Those checks are explicitly SKIPPED/UNVERIFIED, not claimed from callbacks. Separate actual desktop input proved the required input behaviour. No arrow-key behaviour is claimed.

The real-input launcher exited **143** after the complete functional PASS marker and normal Unity shutdown; it did not write its automatic JSON or execute cleanup. Its evidence records this limitation honestly. Disposable source/meta cleanup was performed only after confirming Unity had exited. No native Player/Windows performance or normal Player-exit claim follows from this Editor smoke.

## Cleanliness and remaining boundaries

Exact accepted Unity/URP material and workspace-settings normalization was attributed, preserved in task-local audit output, and restored to HEAD bytes. Final audit found no scene/prefab/material/ProjectSettings/package drift, no unexpected tracked/untracked files, and no disposable Assets harness/meta. The original player save/backup/temp state was preserved byte-for-byte. Startup and baseline worktrees were clean before collecting this documentation; the final evidence commit is followed by a final clean-status check.

`/home/jer/CCF-main` and unrelated work were untouched. Task-owned cache links were removed for clean status; underlying Library/Build output remains preserved on the larger drive. Baseline used its own freshly imported Library. Existing missing pack-prefab warnings and the accepted Search exception were not repaired or silently suppressed.

No canonical/context update, main merge, push, Windows candidate or Player build was performed. Manager decisions on confirmation and Editor opt-in were retained. Decisions needed for startup implementation: none. Remaining release gates: built Player startup/profiling and native Windows technical smoke; Stage A readiness is not claimed. The real-input launcher termination remains an evidence/tooling limitation.

Evidence: JSON receipts and SHA-256 manifest under [Evidence](Evidence); visually inspected captures under `Evidence/Rendered`; actual-input screenshots under `Evidence/RealInput`. Full raw logs, disposable diagnostic sources and normalization backups remain under `/media/jer/Files/CCF-startup-verify-local-output/Build`, with baseline evidence under `/media/jer/Files/CCF-startup-natural-baseline/Build/StartupHashDiagnostic`.
