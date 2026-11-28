# Manager Handoff — Year-30 Deep-History Payload Compaction (Current State)

- **Task**: Fix the pre-existing Gate Stage B Year-30 payload-budget defect without raising `MultiViewBudget`.
- **State**: Implementation candidate **COMPLETE and pushed**. Independent review **NOT RUN** (owner-directed stop before review). **No INTEGRATION DECISION issued.**
- **Integration**: **NOT merged** to `integration/2d-sim` or `main`.
- **Protected baseline**: `origin/main` = `bdeb6a324acaf4129c10fb0846faac8b64974360` untouched.
- **User acceptance**: not requested, not given.
- **Date**: 2026-11-28 (UTC).

---

## 1. Candidate identity

| Item | Value |
|---|---|
| Base branch / SHA | `origin/integration/2d-sim` @ `f4dee8023d3492a25e61451c7e2a49515e507ed3` |
| Feature branch | `feat/multi-2d-year30-history-compaction-v1` |
| Implementation commit | `54783fc99b77fc67694d695f5894f1e63a5c11a0` |
| Branch head (handoff commit) | `9946276a79de55a1cb9a5aead4e2595e97e906fe` |
| Feature worktree | `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-2D-Year30Compaction-20261128` |
| Handoff (implementation) | `Docs/Handoffs/MultiRack-Simulation-20261116/year30-history-compaction/IMPLEMENTATION-HANDOFF.md` |
| This manager handoff branch | `docs/multi-2d-year30-history-compaction-manager-handoff-20261128` |

GitHub:
- Branch tree: https://github.com/jermur788/CCF/tree/9946276a79de55a1cb9a5aead4e2595e97e906fe
- Implementation commit: https://github.com/jermur788/CCF/commit/54783fc99b77fc67694d695f5894f1e63a5c11a0
- Implementation handoff: https://github.com/jermur788/CCF/blob/9946276a79de55a1cb9a5aead4e2595e97e906fe/Docs/Handoffs/MultiRack-Simulation-20261116/year30-history-compaction/IMPLEMENTATION-HANDOFF.md
- `MultiApi.View.cs` at candidate: https://github.com/jermur788/CCF/blob/9946276a79de55a1cb9a5aead4e2595e97e906fe/Tools/MultiRackSitkaP1Lab/src/Ui/MultiApi.View.cs

---

## 2. TL;DR for the manager

1. **The original premise was wrong, and the implementer proved it.** `GET /api/multi/view` did *not* exceed 524,288 B at Year 7. The sweep aborted because **`POST /api/multi/year/advance`** returned HTTP 503: its `annual.report.trees` array duplicated two full per-tree `AnnualTree` snapshots (`Before`/`After`) already present in `view.areas[].trees` in the same response (**107,663 B at Year 1 → ~295 KB at Year 30** on two-area stands, **~707 KB** on the 0.64 ha 5-lane stand).
2. **The candidate fixes three coupled defects**: (a) projects the annual report on HTTP command responses, omitting only the redundant `report.trees` while keeping year bounds, all stand summaries, density pressure, mean vulnerability and every `AnnualDeath`; (b) makes `SerializeBoundedMultiView` budget the **exact outer wire envelope** across all view-carrying routes; (c) makes `historyTruncated` truthful by separating it from a new `pruneLogTruncated` flag.
3. **Acceptance evidence passes**: build 0/0, `Core.Tests` 314/314, `ui-smoke-multiarea` 367/0, `run-step2-edge` **67/0 (base was 55/10)**, both Year 0–30 sweeps PASS. `MultiViewBudget` is byte-for-byte unchanged.
4. **The risk is headroom, not correctness.** `MA-1-MIXED` finishes just **135 B** under budget on the advance route (`524,153` vs `524,288`) and **864 B** under on `GET /view` (`523,424`). There is no independent review, and no integration decision.

---

## 3. Authority and scope

Authority: owner-approved Scheduled Maintenance Follow-Up recorded in the Playable Milestone 1 closeout `674c7c9e` and `2D_Simulator_Strategic_Roadmap.md` v1.22. Locked owner rule: **compact the projection; never raise `MultiViewBudget` above 524,288 B**.

In scope: UI view-projection/compaction code, targeted regression tests, handoff note.
Out of scope and verified untouched: `Assets/**`, `Packages/**`, `ProjectSettings/**`, `origin/main`, protected 3D baselines, `.p0ref` binaries, `src/Core/**`, `src/Core/Phase1/**`, `src/Core/SpeciesWave2/**`, `src/Ui/wwwroot/**`, `Docs/Project/**`. No UI redesign, no lens changes, no merge.

---

## 4. Root cause (measured, not inferred)

A disposable out-of-repo probe (not committed) was run against the unmodified base build and the candidate:

- Base `MA-1`: `POST /api/multi/year/advance` = HTTP **503** from Year 8 (`bytes=524551`, budget `524288`); last successful advance was Year 7 at `521,531 B`.
- Base `MA-1-MIXED`: HTTP **503** from Year 7 (`bytes=527778`); last successful advance was Year 6 at `522,654 B`.
- `GET /api/multi/view` returned HTTP 200 on base at every reachable year (max observed `414,386 B` / `415,260 B`), because after the first 503 the state could not advance further.
- Candidate reaches Year 30 on both stands with no 503. `GET /api/multi/view` bytes for Years 0–7 match base exactly, so the view projection itself was not the failing route.
- Year-30 attribution on `MA-1-MIXED`: `POST /api/multi/year/advance` was `815,440 B`, of which ~295 KB was duplicated `report.trees`; `GET /api/multi/view` was `520,631 B`.

**Consequence for the manager:** the literal brief asked to compact `GET /api/multi/view`. The candidate instead fixes the route that actually failed and leaves the (already-compliant) `/view` projection essentially unchanged. The observable owner intent — Stage B passes, budget not raised — is met, but this is a deviation the manager should explicitly accept.

The third defect: both `MultiApi.View.cs` and `MultiApi.Stand.cs` ORed `pruneTrimmed` into `historyTruncated`, so trimming `stage4.pruneLog` could falsely claim that history was truncated (including at Year 0). The candidate separates `historyTruncated` from `pruneLogTruncated`.

---

## 5. Files changed

`git diff --stat f4dee802..9946276a`:

```text
 .../IMPLEMENTATION-HANDOFF.md                      | 204 +++++++++++++++++++++
 .../src/Ui/MultiApi.Commands.cs                    |  39 ++--
 .../src/Ui/MultiApi.History.cs                     |  41 +++--
 .../src/Ui/MultiApi.Saves.cs                       |  20 +-
 .../src/Ui/MultiApi.Stand.cs                       |  70 ++++++---
 .../src/Ui/MultiApi.View.cs                        | 162 ++++++++++++++-----
 .../src/Ui/MultiApi.Woodland.cs                    |  19 +-
 .../tests/ui-smoke-multiarea.sh                    | 133 +++++++++++++++-
 8 files changed, 575 insertions(+), 113 deletions(-)
```

Coordinator scope audit (read-only, on the candidate):
- `git diff --name-only` against the fenced paths returned **empty**.
- `MultiViewBudget.cs` diff is **empty** (constants and scaled formula unchanged).
- `MultiApi.History.cs` hunks are confined to the skip envelopes (lines ~82–118 and ~386); `history/index` and `history/year` handling is untouched.
- `MultiApi.Commands.cs`, `MultiApi.History.cs`, `MultiApi.Saves.cs`, `MultiApi.Woodland.cs` changes are envelope-aware budget wiring, not simulation or save-format changes.

---

## 6. Verification evidence

| Gate | Command | Result |
|---|---|---|
| Clean build | `dotnet build src/Ui/SitkaP1.Ui.csproj --nologo -warnaserror` | **PASS — 0 warnings, 0 errors** |
| Core tests | `dotnet run --project tests/Core.Tests/SitkaP1.Core.Tests.csproj -c Debug` | **PASS — 314/314 passed, 0 failed** (9 frozen anchors included; `.p0ref` sha256 `9a99f781…` unchanged) |
| UI smoke | `NODE=/home/jer/.local/share/vite-plus/bin/node bash tests/ui-smoke-multiarea.sh …` | **PASS — 367 passed, 0 failed** (base parity requirement ≥363) |
| Gate Stage B | `bash tests/phase1-time-history-ui/run-step2-edge.sh …` | **PASS — 67 passed, 0 failed** (base was 55 passed / 10 failed) |
| Sweep `ma-1` | inside Stage B | **PASS — all 31 years ≤ 524,288 B**, max `517,010 B`; ecology max `43,320 B` |
| Sweep `ma-1-mixed` | inside Stage B | **PASS — all 31 years ≤ 524,288 B**, max `523,424 B`; ecology max `49,685 B` |
| Correctness anchors | Stage B per-year retention | `≥1` event and `≥1` annual record retained where present; `payloadBudgetExceeded=false` |
| Module graph / map-first lenses | not run | **NOT RUN** — no file under `src/Ui/multi/**` changed; excluded by the verification diet |
| Save tests, selftest, full browser gate | not run | **SKIPPED** by the verification diet |

Implementation run provenance: requested and resolved `antigravity_2 / gemini-4-neon` at `thought_level: high`; **executed model UNKNOWN** (not observable). The delegated run ended `interrupted` with `result_available`; all artifacts, commits, push and evidence were present and were reconciled by the coordinator.

---

## 7. Before / after byte counts

**Advance route (`POST /api/multi/year/advance`)** — the actual failing route:

| Stand | Base | Candidate |
|---|---|---|
| `MA-1` | 503 from Y08 (first `524,551 B`); last 200 was Y07 `521,531 B` | 200 all Y01–Y30; max `516,707 B` (Y28) |
| `MA-1-MIXED` | 503 from Y07 (first `527,778 B`); last 200 was Y06 `522,654 B` | 200 all Y01–Y30; max `524,153 B` (Y17) |

**`GET /api/multi/view`** — never the failing route:

| Stand | Base (reachable years only) | Candidate (all 31 years) |
|---|---|---|
| `MA-1` | Y00–Y07 max `414,386 B`; Y08+ unreachable | max `517,010 B` (Y27) |
| `MA-1-MIXED` | Y00–Y06 max `415,260 B`; Y07+ unreachable | max `523,424 B` (Y29) |

**0.64 ha 5-lane probe** (worker-measured, not independently re-run by the coordinator):

| Route | Base Y30 | Candidate Y30 |
|---|---|---|
| `year/advance` | `2,587,264 B` | `1,879,923 B` (−707,341 B) |
| `GET /view` | `1,883,462 B` | `1,883,462 B` (unchanged) |
| 3-lane read | `632,262 B` | `632,262 B` (unchanged) |

All figures are raw UTF-8 JSON bytes. Budget: `524,288 B` for two-area stands; scaled formula unchanged for ≥3-area stands.

---

## 8. Honesty and inspector preservation

- `historyTruncated` is now set **only** when `view.events` or `view.annualHistory` entries are actually omitted. It is not set by projection compaction, `stage4.pruneLog` trimming, or at Year 0 when nothing is omitted.
- A new `pruneLogTruncated` flag (omitted from JSON when false) records prune-log trimming separately. The UI notice still keys off `historyTruncated`, so it no longer makes a false claim.
- Dead trees remain in the projected roster with Core's exact `LifeStates.Of(tree)` (`BiologicallyDead` vs `DeadFallen`), death year/cause/mechanism, and the pruning-lift fields. `ui-smoke` M15 and the Year 0/15/20 checks passed.
- Full on-demand history remains `GET /api/multi/history/index` + `POST /api/multi/history/year`; those handlers were not changed. No Core or save data was removed.

---

## 9. Risks and limitations the manager must weigh

1. **No independent review.** The packet required a review on a separate read-only thread; the owner stopped that step. **There is no `INTEGRATION DECISION`.**
2. **Tight headroom.** `MA-1-MIXED` advance `524,153 / 524,288` = **135 B**; `GET /view` `523,424 / 524,288` = **864 B**. A small future payload addition can reintroduce the 503.
3. **API surface change.** `annual.report.trees` is no longer emitted on command responses. Independent grep found no consumer under `src/Ui/multi/**`; `ui-smoke` now asserts its omission. Any external/HTTP consumer would see a smaller response.
4. **Deviation from the literal brief.** The fix does not reduce `GET /api/multi/view`; it fixes the advance envelope and makes all routes budget the outer envelope.
5. **Wider-than-one-file change.** Five route files are touched for envelope wiring. All are UI HTTP projection code, but this is broader than `MultiApi.View.cs` alone.
6. **Base before-numbers for later years are unobservable.** Once base returned 503, the state could not advance, so a true base `GET /view` maximum for Y08–Y30 cannot be measured. The candidate's 0–30 maximum is the only observed 0–30 view maximum.
7. **Tests ran before the commits** (evidence 01:41–01:47; commits 01:48/01:50). The worktree was clean afterwards, so the committed tree is the tested tree as observable; there is no independent post-commit re-run.
8. **Verification diet gaps** are intentional but real: no map-first/module-graph (no `src/Ui/multi/**` change), no `Phase1.Save.Tests`, no `selftest.sh`, no full `2d-gate.sh --browser`.

---

## 10. Decisions requested from the manager

1. Accept the candidate **without** the originally required independent review, or commission that review before integration.
2. Accept the omission of `annual.report.trees` from HTTP command responses.
3. Accept the 135 B / 864 B headroom, or open a new bounded compaction task before integration.
4. Authorise the single integrator to merge (or not). **No merge has been performed.**

---

## 11. Reproduction

```bash
cd /home/jer/Development/CCF-HP-Worktrees/CCF-Multi-2D-Year30Compaction-20261128/Tools/MultiRackSitkaP1Lab
export PATH="/home/jer/.local/share/vite-plus/bin:$PATH"
export DOTNET_ROOT=/home/jer/.dotnet
DOTNET=/home/jer/.dotnet/dotnet

$DOTNET build src/Ui/SitkaP1.Ui.csproj --nologo -warnaserror
$DOTNET run --project tests/Core.Tests/SitkaP1.Core.Tests.csproj -c Debug
NODE=/home/jer/.local/share/vite-plus/bin/node \
  bash tests/ui-smoke-multiarea.sh /mnt/c/Users/hp/AppData/Local/Temp/ccf-y30-g2
bash tests/phase1-time-history-ui/run-step2-edge.sh /mnt/c/Users/hp/AppData/Local/Temp/ccf-y30-step2
```

Stage B uses Microsoft Edge on Windows and Windows `node.exe`; evidence directories must be outside the repository.

---

## 12. Evidence inventory (outside the repo)

| File | SHA-256 |
|---|---|
| `build.log` | `4c2a66041277d3a04ab0e0115da3e7e11c61968962f196ab67ea5f33ede2738a` |
| `core-tests.log` | `ebc8759909c6ec7360614730279f3dd26802357a4e73e6fb45743c6c5cc9ba80` |
| `ui-smoke-multiarea.log` | `80fd2071ffc51255c99ec6e6b067f5bea9cb782d52eb031489a53ee75a521d63` |
| `step2-edge.log` | `d6eacdecf1c48ce2acf094c0e14ad3efbd071b92bbaa4fa9da022e23e192e904` |
| `step2-sweep-ma-1.json` | `037bef2f68cb8c94b426a1b26ee6e0119c88eaf62bbbfc793d3cb5e48f4535c1` |
| `step2-sweep-ma-1-mixed.json` | `182f1532c2a43e63ac580f9c17e7c4f958790456aed9cb17486393f9e9055b11` |
| `probe/measure_base.log` | `0756fd730bab71f1120fa1908ccbc355a21ebefef1d3385b8b70ba246d15d0b9` |
| `probe/measure_after.log` | `a59e2d9aac46c0fc2bf82b77ca792997e1ed546365f2fa04943b6274a5df811a` |

Root: `/home/jer/.ccf-orch/evidence/2d-year30-compaction-20261128/`

---

## 13. D-030 state words

- Candidate **exists**, is **verified by the checks above**, and is **pushed** on `feat/multi-2d-year30-history-compaction-v1`.
- **NOT merged** to `integration/2d-sim` or `main`.
- `origin/main` `bdeb6a324acaf4129c10fb0846faac8b64974360` **untouched**.
- **Review NOT RUN**; **no INTEGRATION DECISION**.
- **Not user-accepted**; no Windows build or external demo.
