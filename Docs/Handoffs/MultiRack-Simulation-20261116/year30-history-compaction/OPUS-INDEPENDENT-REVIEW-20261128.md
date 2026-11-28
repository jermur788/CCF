# Independent Review — Year-30 Deep-History Payload Compaction

- **Reviewer**: Claude Opus 5.5 (`claude-opus-5-5`). Separate top-level, read-only review thread. Different provider from the Gemini 4 Neon implementer. No self-review.
- **Candidate reviewed**: `9946276a79de55a1cb9a5aead4e2595e97e906fe` (implementation commit `54783fc99b77fc67694d695f5894f1e63a5c11a0`) on `feat/multi-2d-year30-history-compaction-v1`.
- **Base**: `f4dee8023d3492a25e61451c7e2a49515e507ed3` (`origin/integration/2d-sim` when the branch was cut). It is an ancestor of the candidate.
- **Protected `origin/main`**: `bdeb6a324acaf4129c10fb0846faac8b64974360`. Re-read at review start; unchanged.
- **Date**: 2026-11-28.

## INTEGRATION DECISION

**NEEDS CHANGES**

The root-cause fix is correct and a clear improvement. The base fails Stage B at Year 7/8, while the candidate passes all author gates, and I reproduced every one of them. But the review brief asks me to stress the margin under realistic Year 0–30 player work, and that stress test fails:

- **Steady yearly thinning on `MA-1-MIXED` makes `GET /api/multi/view` return HTTP 503 (`MultiViewPayloadBudgetExceeded`, `bytes=525092`, budget `524288`) at Year 30.** The workload is 2 fells and 3 prunes per area per year, each fell with a 500-character operator reason, economy Off.
- The Year-30 `POST /api/multi/year/advance` before it returned **200** (520,443 B). So Core commits Year 30, and then every page load or re-read of the stand fails.
- The result is deterministic: two independent runs gave byte-identical results.

A small additional trim is needed before integration. See §4 for the scope I recommend.

## 1. Root cause and envelope fix — verified

| Claim | Verdict | Evidence |
|---|---|---|
| `annual.report.trees` duplicated the tree roster and caused the advance 503 | **Confirmed** | `ProjectAnnualForBudget` (`MultiApi.View.cs`) keeps `yearBefore/yearAfter` and, per area, `before/postGrowth/after`, `postGrowthDensityPressure`, `meanVulnerability` and `deaths`. It drops only `Trees`. Advance responses shrink to ~1.5–3.9 KB of `annual`. Nothing under `src/Ui/multi/**` reads `payload.annual` (grep). |
| Core and save history are untouched | **Confirmed** | `git diff --name-status f4dee802..9946276a` lists only 7 `src/Ui/MultiApi.*.cs` files, the smoke script and the handoff. The projection is applied at the HTTP edge only. |
| Budget is measured on the exact outer envelope | **Confirmed** | Every view-carrying route (command, economy session, cell, competition, hit, skip success and refusal, load, woodland/new) passes an `envelopeSerializer`. The fitted bytes are the wire bytes. `WriteBoundedBytesAsync` re-checks `bytes.Length > budget.MaxViewBytes`, so no over-budget body can be sent as 200 or 409. In 12 probe runs across 3 stands, no 200/409 body exceeded the budget. |
| `/history/index` and `/history/year` are intact | **Confirmed** | `MultiApi.History.cs` hunks touch only the skip envelopes and the `MultiSkipPayload` record. |
| `historyTruncated` is honest; `pruneLogTruncated` is separate | **Confirmed** | `historyTruncated` is now computed only from `events`/`annualHistory` keep counts in `View.cs` (Tiers 0–3) and in `Stand.cs` (the hard-coded `histTrunc: true` is gone). Smoke `M15` passes. Across all 12 probe runs, no Year-0 response had `historyTruncated=true`. |
| Dead-tree life-state and inspector fields are preserved | **Confirmed (inspection)** | `ProjectViewForBudget` / `ProjectStage4ForBudget` bodies are unchanged by this diff; only their call sites moved. `AnnualDeath` entries are kept in full. |

Nits (non-blocking):

- The non-composite command branch still uses `WriteBoundedJsonAsync` without the envelope-aware tiering. It is never reached for a two-area composite session, but it is a leftover asymmetry.
- `GET /api/multi/view` adds about 9 KB (`labels` 8,622 B plus `reviewPreview`) that command envelopes do not carry. This is why the advance fits and the next view read does not (§2).

## 2. Margin stress check — the 135 B number is not the real risk; the Tier-3 floor is

**Reading the 135 B / 864 B figures correctly.** `SerializeBoundedMultiView` adapts. It tries Tier 0, then Tier 1 (projection), then Tier 2 (`compactGround`). Within each tier it binary-searches how much history tail to keep, and it packs right up to the budget. A response landing a few bytes under 524,288 B therefore means only that the packer filled the space. I saw 5–8 B "margins" in many routine responses. Growth simply moves the response down a tier or trims inline history. The real headroom is the distance between the **irreducible floor** (Tier 2 compact ground plus 1 event and 1 annual record) and the budget. Once the floor crosses the budget, the route returns 503.

Independent HTTP probe (`scripts/margin_probe.py`, outside the repo): scratch fixture and XDG, every view-carrying response recorded each year, Years 0–30:

| Stand / workload | Max advance | Max `/view` | Worst Year-30 state | Result |
|---|---:|---:|---|---|
| `MA-1-MIXED` idle (author's Stage B shape) | 523,342 | 523,998 | Tier 1, `s4c=False` (Tier 2 reserve unused) | PASS |
| `MA-1-MIXED`, 3 prunes/area/yr, no fells | 523,336 | 523,982 | Tier 1, `s4c=False` | PASS |
| `MA-1-MIXED`, 1 fell + 1 prune/area/yr | 524,281 | 523,994 | Tier 1, inline `annualHistory`=2 | PASS |
| `MA-1-MIXED`, 2 fells + 3 prunes/area/yr, 4-char reason | 523,461 | 524,185 | **Tier 2, `annualHistory`=1** (floor reached; ~0.9 KB left) | PASS, at the floor |
| `MA-1-MIXED`, 2 fells + 3 prunes/area/yr, 500-char reason | 524,203 | **503 at Y30** | advance 200 at Tier 2 floor (520,443 B); view floor 525,092 B | **FAIL** (2/2 runs, identical) |
| `MA-1-MIXED`, worked + Economy On (approve refused from Y6 by economy rules) | 523,761 | 523,963 | Tier 2, `annualHistory`=3 | PASS |
| `MA-1`, 2 fells + 3 prunes/area/yr | 524,021 | 524,084 | Tier 1 | PASS |
| `MA-1`, worked + Economy On | 521,013 | 515,762 | Tier 0/1 | PASS |

All other worked-scenario command routes (`pending/add`, `prune/add`, `plan/approve`, `plan/execute`) also stayed ≤ 524,288 B.

**Assessment.** The margin is **not acceptable as-is** for "realistic Year 0–30 player work". The author's sweep is an idle stand, where Tier 2 is never used. With steady thinning, ecology roughly doubles (~45 KB → ~100 KB) and dead/felled rosters grow. Then:

- Tier 2 plus a single retained annual record is the only configuration that fits from about Year 25.
- The view floor crosses 524,288 B at Year 30.
- Inline history has already shrunk to 1–2 records from about Year 21 on these runs. That is honest (`historyTruncated=true`, and full history is on demand), but it shows there is no reserve left.

The base is strictly worse: its advance returns 503 from Year 7/8. So this is not a regression. It is an unmet acceptance target under the stress the brief asked for.

## 3. 0.64 ha 5-lane stand (`fm-064-sitka-edge-broadleaves-v1`)

The premise "≤ 524,288 B on the 5-lane stand" does not apply. `MultiViewBudget.ForStand` scales the cap for ≥3-area stands. With 1,344 genesis stems, ceil(1344/300) = 5 units, so the cap is min(2,621,440, 5 × 524,288) = **2,621,440 B**. This formula is unchanged and `MultiViewBudget.cs` is untouched. Against that cap:

| Workload | Max advance | Max `/view` | Max 3-lane `stand?lanes` | Headroom (view) |
|---|---:|---:|---:|---:|
| idle | 1,879,923 (Y30) | 1,883,462 (Y30) | 632,262 | 737,978 B |
| 2 fells + 3 prunes/yr (500-char reasons) | 2,113,162 (Y30) | 2,117,163 (Y30) | 862,614 | 504,277 B |
| worked + Economy On | 1,921,837 | 1,925,332 | 673,247 | 696,108 B |

Every response stayed in Tier 0 with full history; there was no 503. The idle numbers match the author's table to the byte (advance 1,879,923; view 1,883,462; 3-lane 632,262). The ~707 KB advance reduction is confirmed. **The 5-lane headroom is comfortable.**

## 4. Required change (bounded) and re-review gate

1. Lower the two-area Tier-2/Tier-3 floor enough that the §2 worst case (MA-1-MIXED, 2 fells + 3 prunes/area/yr, 500-char reasons, Years 0–30) returns 200 on `GET /api/multi/view` **and** on every command route, with a stated reserve at Year 30. I suggest ≥ 16 KB below 524,288 B on the floor configuration. Possible levers, all at the UI projection edge with no Core, save, budget or JS-contract change:
   - a more compact Tier-2 projection of dead/felled roster rows or of `deadwood`/`canopy`;
   - keeping static `labels` off the view payload when the client already has them.

   The implementer should choose and measure. **Do not raise `MultiViewBudget`.**
2. Add that worked-thinning scenario (or an equivalent deterministic one) as a regression check that asserts the floor reserve. A plain "≤ budget" check is not enough, because the packer always lands just under the budget.
3. Re-review can be a delta review: the diff plus a rerun of this probe, smoke and Stage B.

## 5. File fence and budget invariant (vs `f4dee802`)

```text
A Docs/Handoffs/MultiRack-Simulation-20261116/year30-history-compaction/IMPLEMENTATION-HANDOFF.md
M Tools/MultiRackSitkaP1Lab/src/Ui/MultiApi.Commands.cs
M Tools/MultiRackSitkaP1Lab/src/Ui/MultiApi.History.cs
M Tools/MultiRackSitkaP1Lab/src/Ui/MultiApi.Saves.cs
M Tools/MultiRackSitkaP1Lab/src/Ui/MultiApi.Stand.cs
M Tools/MultiRackSitkaP1Lab/src/Ui/MultiApi.View.cs
M Tools/MultiRackSitkaP1Lab/src/Ui/MultiApi.Woodland.cs
M Tools/MultiRackSitkaP1Lab/tests/ui-smoke-multiarea.sh
8 files changed, 575 insertions(+), 113 deletions(-)
```

Exactly the 8 claimed files. There are **zero** changes under `src/Core/**`, `src/Ui/wwwroot/**`, `src/Ui/multi/**`, `src/Ui/MultiViewBudget.cs`, `Assets/`, `Packages/`, `ProjectSettings/`, `Docs/Project/**` or `2D_Simulator_Strategic_Roadmap.md`. `origin/main` is still `bdeb6a32…`. `MultiViewBudget` is still `DefaultViewBytes = 524_288`, `DefaultEcologyBytes = 153_600` and `CeilingViewBytes = 2_621_440`.

## 6. Independent verification (fresh `--shared` sparse clone at `9946276a`, outside every worktree)

- Toolchain: .NET SDK 8.0.134 (`~/.dotnet`; `global.json` pins 8.0.133 with `latestPatch`). Never SDK 10. Real Linux node from `~/.local/share/vite-plus/bin`.
- `.p0ref/SitkaLab.Core.dll`: sha256 `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3` (copied read-only from the feature worktree; pin matches).

| Gate | Result |
|---|---|
| `dotnet build MultiRackSitkaP1Lab.sln -warnaserror --no-incremental` | **PASS — 0 warnings, 0 errors** |
| `dotnet build src/Ui/SitkaP1.Ui.csproj -warnaserror --no-incremental` | **PASS — 0 warnings, 0 errors** |
| `Core.Tests` | **PASS — 314/314, 0 failed.** All 64-hex fingerprint/anchor lines are byte-identical to the author's log: Year 1–10 digests, model0/model1, S4, S5, S5-07A, P2-01, P2-03, P2-10 MA-1-MIXED 30-year, P2A-02, and Anchors 10–11. The 9 frozen anchors show no drift. |
| `tests/ui-smoke-multiarea.sh` | **PASS — 367 passed, 0 failed** |
| `tests/phase1-time-history-ui/run-step2-edge.sh` | **PASS — 67 passed, 0 failed.** No NB1c flake; no retry needed. |
| Stage B sweep maxima (Years 0–30) | `MA-1`: view **517,010 B** (Y27), ecology **43,320 B**. `MA-1-MIXED`: view **523,424 B** (Y29), ecology **49,685 B**. Exactly the author's figures. |
| Advance maxima (independent probe, idle) | `MA-1-MIXED` **523,342 B** (Y29). The author's 524,153 B at Y17 was not reproduced byte-for-byte: my probe sends `consentToCancelWork: true` and its request sequence differs, and the packer lands near the cap either way. |
| Skipped per brief | `selftest.sh`, `Phase1Save.Tests`, `run-map-first-lenses-edge.sh`, full `2d-gate.sh --browser`, Unity |

Hygiene: every probe server stopped on SIGINT with empty stderr, apart from the deliberate 503 log lines. There are no leftover processes. The clone's `git status` was clean, and the feature worktree was not touched. I did not touch the protected listeners `:33767`, `:43649` and `:46593`. Evidence root: `/home/jer/.ccf-orch/evidence/2d-year30-review-opus-20261128/` (`logs/`, `scripts/margin_probe.py`). Edge evidence: `C:\Users\hp\AppData\Local\Temp\ccf-y30-opus-g2`, `…\ccf-y30-opus-step2`.

## 7. Provenance

| | Model | Effort |
|---|---|---|
| Requested | Claude Opus 5.5 | medium (escalate only on failure) |
| Selected | Claude Opus 5.5 | medium |
| Resolved | `claude-opus-5-5` | medium |
| Executed | `claude-opus-5-5` (self-reported by the running model) | medium; no escalation |

Thread ID: not exposed to the reviewing model in this session. The review ran in a fresh top-level Claude Code thread.

## 8. D-030 state words

- Candidate `9946276a` was **reviewed on an isolated branch**: `review/multi-2d-year30-history-compaction-v1`, which contains only this file.
- Decision: **NEEDS CHANGES**.
- **NOT merged** to `integration/2d-sim` or `main`. `origin/main` `bdeb6a324acaf4129c10fb0846faac8b64974360` is **untouched**.
- **Not user-accepted.**
