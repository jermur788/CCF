# Startup hash diagnostic handoff

Branch: `task/scenario-one-startup-screen`  
HEAD before diagnostic: `1edf818866fe384e25440f86d62dbd5bdee05828`  
Canonical base/context: `e8c73f25e6d93db818abdc6c794887f8f7c5ba43`

**Classification: Case A.** The startup implementation did not change the starting world. The original fixture compared an eight-frame natural-scene capture with an anchor established by a different deterministic reset fixture.

| Check | Result |
|---|---|
| Unchanged authoritative Enlarged80Verification | PASS, 12 groups |
| Established START | `04A78188A6F8E6F9` |
| Established YEAR3 | `081DB58CE3034648` |
| Startup failed expected hash | `04A78188A6F8E6F9` |
| Startup actual hash / initial freshHash | `EEED1DE3EDC65517` / `EEED1DE3EDC65517` |
| Geometry / ecological year / trees | 1 / 0 / 1344 |
| Serialized save cells / live ecology cells | 0 / 256 |
| Base natural-scene CurrentWorldHash | `EEED1DE3EDC65517` |
| Branch natural-scene CurrentWorldHash | `EEED1DE3EDC65517` |
| Identical capture-only harness / captures | YES / YES |
| CanLoad no-save, valid-save, rejected-save mutation | NONE in world or primary/backup/temp file bytes |

The baseline used a separate detached worktree at exactly e8c73f2 and its own freshly imported Library. The same capture-only harness ran once on each checkout before any menu, without save operations or deliberate world mutation. Source SHA-256: `fd4a730371e18c133f238fea26bc95616786117d40224f5efe28faea8d8f7055`.

Production files changed during diagnosis: **none**. No established anchor changed and the natural-scene hash was not adopted as a new anchor.

Verification-only corrections were committed separately:

- `cfcb1374f8dfeb44e17563458b6bb8489cba7b7a`: retain the startup fixture's own captured-world invariance checks; leave exact physical anchors to the unchanged authoritative Enlarged80 gate.
- `df743cc071b98f569694c334c321baf60ab966b0`: wait two frames after restoring a historical fixture before hashing. Identical base/branch probes show 16 pending inactive historical trees in the immediate capture, followed by the exact fresh hash/tree count after Unity completes deferred Destroy.
- `f19f2170025356d0fac65d41da9bdddcbb5b0d73`: account for the existing Walking Help introduction after title handoff. A disposable probe showed the player held by Walking Help while save/UI components were enabled; closing that existing panel restored player control with an unchanged world hash. Actual-input mode requires real Escape to close that introduction if shown.

- `06f28e40898e7c799331adf0a4dbcf3764a0a73a`: report undelivered synthetic-key checks explicitly as unverified/skipped; retain callback/save/rendered checks and require separate actual-input evidence.

Verification files changed during diagnosis: `Tools/Verification/Startup/StartupScreenVerification.cs` and `Tools/Verification/Startup/README.md`. Diagnostic logging/source remained disposable.

The corrected startup batch, bootstrap, rendered layouts, Legacy40 startup, boundary/restart, save/UI, calibration and full 24/24 regression passed. All required compatibility anchors remain exact. Actual Linux Editor mouse/keyboard checks passed functionally; their launcher exited 143 after PASS and normal Unity shutdown, which is recorded separately rather than claimed as an automated zero-exit result. No production fix was required.

Next action: Manager inspect the exact completed local branch and evidence commit. Built Player profiling/startup and native Windows technical smoke remain later release gates; no Stage-A readiness claim.

Raw diagnostic evidence is preserved under `/media/jer/Files/CCF-startup-verify-local-output/Build/StartupHashDiagnostic`; the baseline capture and timing probe are under `/media/jer/Files/CCF-startup-natural-baseline/Build/StartupHashDiagnostic`. All diagnostic logging was disposable and remains outside production/Git source.
