# Woodland Importer V1 — integration receipt

2026-11-26. Integration writer: Claude Opus 5.5 (single writer). Packet: [MANAGER-INTEGRATION-PACKET-20261126.md @ c2a76ff2](https://github.com/jermur788/CCF/blob/c2a76ff24de1cfa5bbbbd6782128fb6fbb3c9f1a/Docs/Handoffs/MultiRack-Simulation-20261116/woodland-importer/MANAGER-INTEGRATION-PACKET-20261126.md). Owner authorisation: owner message 2026-11-26 ("authorized to integrate Woodland Importer V1 … push to integration/2d-sim … do not merge to main").

**Result: INTEGRATED to `integration/2d-sim` at `c82e4e759c2ceef1c13ddb6a033e034097aef77e`. `main` unchanged. Lane released.**

## Admission (observed)

| Check | Observed |
|---|---|
| `origin/integration/2d-sim` before merge | `6a003a3070cb37b8cc045124b64b4279d84f7c06` (= packet base; Species Wave 2 merge) |
| Reviewed feature | `674f3350c3554aee63cb5f3f411c9858985e538d` (R3 `899ff325` APPROVED FOR INTEGRATION) |
| Merge base (trunk, feature) | `6d72c30538b1fbe30d3cbcf083b5b368733662b8` (= packet common ancestor) |
| `origin/main` | `bdeb6a324acaf4129c10fb0846faac8b64974360` (= canonical context) |
| Integration worktree | `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-2D-Integration-20261125`, branch `integration/2d-sim`, HEAD `6a003a30`, 0 status lines |
| Concurrent integration | none observed: only other 2D worktree (`integrator/2d-phase2b-c1-gate-retirement-20261126-v1`, `45417789`) is clean and already an ancestor of trunk; no running dotnet/git merge/push processes; `active-tasks.md` (live GitHub) has no woodland/2d-sim integration entry |
| RUNNING registration | `wl-int-20261126-01` appended to the portable lane ledger `/home/jer/Development/orchestration/woodland-importer/routing-ledger.jsonl` before merge (validated, 15 lines) |

## Merge

- Merge commit `c82e4e759c2ceef1c13ddb6a033e034097aef77e`, `--no-ff`, ort strategy, no conflicts, no manual edits.
- Parents: `6a003a3070cb37b8cc045124b64b4279d84f7c06` (trunk) and `674f3350c3554aee63cb5f3f411c9858985e538d` (feature).
- Trunk → merge: **23 files, all additions, 5990 insertions, 0 outside the four importer prefixes.**
- Feature tree preserved: `git diff 674f3350 c82e4e75 -- <four prefixes>` is empty.
- Trunk content preserved: diff trunk → merge outside the prefixes is empty; feature → merge outside the prefixes shows exactly the 19 Species Wave 2 files.

## Gates on exact merge `c82e4e75`

SDK `8.0.133` (`global.json` pin). `.p0ref/SitkaLab.Core.dll` sha256 `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3` = `P0CoreExpectedSha256` in `src/Core/SitkaP1.Core.csproj` (provenance `p0QualifiedCommit=a7f41983…`); protected reference not rebuilt.

| Gate | Exit | Result |
|---|---|---|
| `dotnet build MultiRackSitkaP1Lab.sln` | 0 | 0 Warning(s), 0 Error(s) |
| `dotnet restore tests/Core/WoodlandImport/SitkaP1.WoodlandImport.Tests.csproj` | 0 | restored |
| `dotnet build … --no-restore` | 0 | 0 Warning(s), 0 Error(s) |
| `dotnet run … --no-build --no-restore` | 0 | synthetic 25/25, qualified 4/4, **TOTAL 29/29 passed, 0 failed** |

Determinism hashes reported by the run: ie-donadea `72cec3ba…a63` (31903 bytes), lu-mersch `7fd8b963…5bf` (29876 bytes). Worktree status after gates: 0 lines.

Not run, per packet: browser gate, gate self-test, independent review, Core.Tests or other broad suites, Unity, Windows build.

## Push readback (2026-11-26T23:14:51Z)

```
pre-push origin/integration/2d-sim  6a003a3070cb37b8cc045124b64b4279d84f7c06
push                                6a003a30..c82e4e75  (fast-forward, no force)
origin/integration/2d-sim           c82e4e759c2ceef1c13ddb6a033e034097aef77e
origin/main                         bdeb6a324acaf4129c10fb0846faac8b64974360  (unchanged)
```

## Limitations carried forward

- Isolated component only: not wired into `MultiAreaContinuation`, `/multi`, saves, UI, simulation pipeline, species support or Unity. No gameplay admission, calibration or player acceptance claim.
- Publisher IPT archive not qualified (WI-T00/T01/T08 pending real archive download); fixtures are GBIF verbatim records in a constructed DwC-A.
- Local-metres reading of `verbatimCoordinates` and constructed test plot boundaries remain assumptions.
- Deferred non-blocking: NF-07 unrecognised measurements discarded, NF-08 numeric-prefix generic ids may false-refuse, NF-09 case-sensitive `TargetPlotId`.
- RUNNING/RELEASED registration is in the portable lane ledger outside the repo; the shared `active-tasks.md` board was not edited by this lane.

## Lane release

Integration writer lease released after push readback. No PR or tag created. Returned to Manager.
