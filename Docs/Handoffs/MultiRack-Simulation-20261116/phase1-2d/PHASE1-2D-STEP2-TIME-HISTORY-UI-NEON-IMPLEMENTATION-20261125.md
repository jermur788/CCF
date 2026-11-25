# CCF Multi 2D — Phase 1 Step 2 (Time Pacing, Decision→Outcome History & Woodland Structure UI/HTTP + NB-2): Implementation and Verification Handoff

**Date:** 2026-11-25 (desktop-local lane, WSL2 Ubuntu + Windows Microsoft Edge headless CDP).
**Role & sole writer:** Primary Implementer — Gemini 4 Neon (sole writer on the dedicated Step 2 worktree and branch; zero subagents spawned).
**Specification:** [`specs/multi-2d-phase1-time-history-ui-v1/spec.md`](../../../../specs/multi-2d-phase1-time-history-ui-v1/spec.md) (`SPEC-multi-2d-phase1-time-history-ui-V1`).
**Repair pass basis:** Independent Opus review (`origin/review/multi-2d-phase1-time-history-ui-20261125-v1:Docs/Handoffs/MultiRack-Simulation-20261116/phase1-2d/PHASE1-2D-STEP2-TIME-HISTORY-UI-OPUS-REVIEW-20261125.md`, verdict `NEEDS CHANGES` on reviewed HEAD `513355c4bcef881413d517aa62feed4676234319`) and binding **Owner G5 Decision** (Year 30 horizon on both `MA-1` and `MA-1-MIXED`, projection-only reductions preserving existing field names and map/table/UI behaviour, omit fields rather than fabricating empty lists, explicit `payloadBudgetExceeded` + server stderr log if ever over budget).

> **EXPLICIT STATE:** This work is **IMPLEMENTED ON TASK BRANCH** only. It is **NOT REVIEWED**, **NOT INTEGRATED**, and **NOT ACCEPTED**. Every checkbox in `specs/multi-2d-phase1-time-history-ui-v1/checklists/requirements.md` remains unchecked `[ ]` pending independent re-review of the repair pass.

| Label | State |
|---|---|
| IMPLEMENTED | **YES — IMPLEMENTED ON TASK BRANCH** (Initial Step 2 implementation + bounded repair pass closing `F-1`..`F-6` and optional nits `N-1`, `N-3`, `N-4` under binding Owner G5 Year 30 horizon) |
| REVIEWED | **NO — NOT REVIEWED** (independent re-review of repair commit pending; all 12 checklist boxes remain `[ ]`) |
| INTEGRATED | **NO — NOT INTEGRATED** (task branch only; zero merge to `main`, zero PR, zero tags) |
| VERIFIED (author gates) | **YES for Step 2 author gates (repair pass)** (`0/0` warnings/errors on `.sln`, `Phase1Save.Tests`, `SitkaP1.Ui`; Core `277/277`; Save `50/50`; `ui-smoke-multiarea.sh` `362/362`; `phase1-save-ui/run-edge-e2e.sh` `63/63`; `phase1-time-history-ui/run-step2-edge.sh` `64/64` including clean Year `0..30` sweeps on both `MA-1` and `MA-1-MIXED`; all 9 frozen anchors & `.p0ref` hash exact) |
| ACCEPTED | **NO — NOT ACCEPTED** |

---

## 1. Lane, Base, Toolchain & Commits

- **Repository root:** `/home/jer/Development/CCF-HP`
- **Worktree:** `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-2D-Phase1-TimeHistoryUI-20261125`
- **Branch:** `feat/multi-2d-phase1-time-history-ui-20261125-v1`
- **Base commit (exact):** `5fe2fabe9073d7b72711f4ab48553c785351d7bc` (Step 1 Core, independently APPROVED at `6c83d16e`)
- **Prior reviewed HEAD:** `513355c4bcef881413d517aa62feed4676234319`
- **Repair commit:** linear commit immediately above `513355c4bcef881413d517aa62feed4676234319` (`fix(ui): close Step 2 review findings for Year 30 payload and history safety`), no amend and no force-push
- **Toolchain:** `~/.dotnet/dotnet` SDK `8.0.134` (`global.json` pins `8.0.133` + `latestPatch`; system `dotnet` 10.x was NOT used); Linux Node `v26.13.1` (`/home/jer/.local/share/vite-plus/bin/node`); Windows Node `v26.13.1` (`/mnt/c/Program Files/nodejs/node.exe`); installed Windows Microsoft Edge (`C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe`)
- **External `TMPDIR`:** `/home/jer/ccf-p1ui-tmp` (outside repository)
- **External evidence archive (mode `700`, outside repository):** `/home/jer/Development/CCF-Multi-Phase1-Step2-Evidence-20261125` (zero capability tokens/URLs committed to Git)
- **Single-writer check:** `git worktree list` confirmed `CCF-Multi-2D-Phase1-TimeHistoryUI-20261125` is the sole checkout of `feat/multi-2d-phase1-time-history-ui-20261125-v1`, with zero competing writer processes.

### Commit list (oldest → newest above `5fe2fabe…`)

| # | SHA | Message |
|---|---|---|
| 1 | `f16c86db34ade7411d705c8757b414fa5e36fa63` | `spec: add multi-2d phase1 time-history UI quartet` |
| 2 | `d24a89df675d8dc1d719c5353a91db498a257b75` | `feat(ui): add review preview, skip, history and structure endpoints with bounded view payload` |
| 3 | `176ab838c7bae993a6c493379a517d0edbc70267` | `feat(ui): add Skip, History drawer, Structure panel and NB-2 Plant-drawer polish` |
| 4 | `a241c9a055071b628048e8894b5cdf28fe5003a2` | `fix(ui): clean Skip summary null, pin History Load-older footer and fit Structure table at 1280x720` |
| 5 | `6d4c406fa37dbba0e5a66cdf44c30bc30bc217f3` | `test(ui): add Step 2 API, payload, smoke and real-Edge gates` |
| 6 | `513355c4bcef881413d517aa62feed4676234319` | `docs(phase1-2d): record Step 2 implementation and verification handoff` |
| 7 | *(repair commit)* | `fix(ui): close Step 2 review findings for Year 30 payload and history safety` |

---

## 2. Opus Review Findings & Dispositions (`F-1`..`F-6`, `N-1`..`N-5`)

| ID | Severity | Finding Summary | Disposition | Verification Evidence |
|---|---|---|---|---|
| **F-1** | BLOCKER | `GET api/multi/view` exceeded `524,288 B` past Year 23 (`MA-1-MIXED`) and `ecology` grew past `153,600 B` by Year 38; over-budget bytes were returned silently. | **FIXED** under Owner G5 Year 30 horizon: added `ProjectEcologyForView` (filters `ecology.pruneStatusByTreeId` to living stems + any dead stem with `lifts > 0` or `planState != null`, keeping `ecology <= 49,685 B` through Year 30 on both experiments), monotonic projection-only `SerializeBoundedMultiView` Tiers 0–3 (`Tier 1` compacts non-living/display precision in `view.areas[].trees`, `scenes`, `stage4.areas[].trees`, `fallenDeadwood` while keeping full untouched `stage4.areas[].ground`), and explicit over-budget signalling (`payloadBudgetExceeded: true` + `Console.Error` log line, never silent). | Clean per-year Year `0..30` HTTP sweeps on BOTH `MA-1` (max `524,279 B`, max eco `43,320 B`, `over=false`) and `MA-1-MIXED` (max `524,032 B`, max eco `49,685 B`, `over=false`), plus in-Edge Year 0..30 checks in `repair-edge-step2/run.log`. |
| **F-2** | MAJOR | Zero-history fallback dropped ALL events and annual records before trimming `stage4.pruneLog`, and `renderHistory()` falsely displayed `"No ecological year has been advanced in this session."` next to the truncation notice. | **FIXED**: `SerializeBoundedMultiView` trims `stage4.pruneLog` BEFORE dropping below `1` latest work event / `1` latest annual record and preserves `>= 1` latest event & `>= 1` latest annual record whenever present. `renderHistory()` now checks `(state.totalRecordedYears > 0 || state.historyTruncated || view.source.ecologicalYear > 0)` when `annualHistory.length === 0` and displays `"Compact view omits annual history rows to stay within payload budget; open History in the header to inspect all N recorded years."` instead of claiming zero years advanced. | Verified `>= 1` event & `>= 1` annual record at every Year `1..30` in both sweeps + Edge runs, and DOM-verified the zero-history Results-pane copy in `ui-smoke-multiarea.sh` (`S2j`). |
| **F-3** | MINOR | `POST api/multi/history/year` with `year = int.MaxValue` (`2147483647`) hit `checked(year + 1)` in Core, returning HTTP `500 OutcomeUnconfirmed` and logging `OverflowException` to stderr. | **FIXED (zero Core edits)**: `ReadMultiHistoryYearAsync` checks `if (year > currentView.Source.EcologicalYear)` before calling Core and returns HTTP `409 HistoryYearNotRecorded` with fresh `fence` and bounded `view`. | Verified in `ui-smoke-multiarea.sh` (`W251` `int.MinValue` -> `409 HistoryYearInvalid`; `W253` `int.MaxValue` -> `409 HistoryYearNotRecorded`; `Z02-api` empty stderr). |
| **F-4** | MINOR | Unflagged, non-monotonic compact `stage4` omitted vertex `xm`/`zm`, fabricated empty `contributingTreeIds: []`, and ignored `pruneLog` trimming in `historyTruncated`. | **FIXED**: Documented exact monotonic Tiers 0–3 in `spec.md`/`plan.md`; added explicit `projectionCompacted`, `stage4Compacted`, and `payloadBudgetExceeded` flags; Tier 1 keeps full untouched `stage4.areas[].ground` (`stage4Compacted = false` at EVERY Year `0..30` on both `MA-1` and `MA-1-MIXED`!); if Tier 2 (`stage4Compacted = true`) is ever needed beyond Year 30, it omits `contributingTreeIds` via `[JsonIgnore(WhenWritingNull)]` (never fake `[]`) and preserves all 4 vertex fields `{ xm, zm, u, v }` rounded to 3 decimals; `historyTruncated` now truthfully includes `stage4.pruneLog` trimming. | Verified monotonic `proj=False -> True`, `s4c=False` across all 31 years on both experiments, and spec/code alignment. |
| **F-5** | MINOR | Suites stopped at first truncation, did not test `>= 1` event/annual retention or Results-pane zero-history copy, tested only `PlayerSessionMismatch` on `history/year`, lacked Load-over-live-session test, and omitted NB-2 order 2. | **FIXED**: Extended `ui-smoke-multiarea.sh` (`W246` `PlayerSourceStale`, `W248` `PlayerPlanStateStale`, `W251` `int.MinValue`, `W253` `int.MaxValue`, `W254` `isOverdue=True`, `W258` retention/flags, `S2j` zero-history copy, `S2k`/`S2l` F-6 stale-response discards), `drive-step2.mjs` (NB-2 order 2 Shelter refusal at BOTH `1280×720` & `1920×1080`, per-year budget & retention loops through Year 30, Load-over-live-session with History drawer + Structure open), and `run-step2-edge.sh` (clean Year `0..30` sweeps on both `MA-1` & `MA-1-MIXED` writing JSON evidence). | All extended checks ran and passed: `repair-smoke` `362/362`, `repair-edge-step2` `64/64`. |
| **F-6** | MINOR | In-flight `GET api/multi/structure` or `POST api/multi/history/year` responses could overwrite newer state after a tree-tag change or `loadHistoryIndex` reset. | **FIXED**: `applyPayload` bumps `state.structureRequest++` and clears `state.structureLoading = false` on tree-tag change; `refreshStructure` captures `reqTag` at request time and discards responses whose tag or counter no longer matches; `loadHistoryIndex(..., append=false)` bumps `state.historyDetailRequest++` and clears `state.historyDetailLoading = false`; `loadHistoryYear` captures `reqSession` and guards on return. | Verified directly in DOM witness `ui-smoke-multiarea.sh` (`S2k` stale structure response discarded cleanly without getting stuck loading; `S2l` stale history/year response discarded cleanly after index reset). |
| **N-1** | NIT (opt.) | `renderSkipControl()` computed overdue state in browser JS instead of reading from server/Core. | **FIXED**: Server computes `bool IsOverdue` from Core's `OverdueReviewPeriodLabel` on `preview-review` (`MultiReviewPreviewDto` & `MultiPreviewReviewPayload`); JS reads `Boolean(p.isOverdue)` directly with zero pacing arithmetic. | Verified in `ui-smoke-multiarea.sh` (`W254` `isOverdue=True`) and `repair-edge-step2` (`ma1-full` Year 8 overdue label + `mixed-play-save` Year 15 overdue label). |
| **N-2** | NIT (opt.) | `totalRecordedYears` / `reviewPreview` / `history/year` fence+read use adjacent read-only Core calls rather than a single combined Core lock. | **DEFERRED (documented)**: Preserves zero Core edits (`src/Core/**` untouched); all endpoints are read-only and serialised cleanly through `MultiAreaSession` thread-safe locks; low risk as noted in review §11. | Verified zero `src/Core/**` diff. |
| **N-3** | NIT (opt.) | Non-view mutation/load/skip responses were unbounded and omitted `historyTruncated` (allowing stale truncation notes). | **FIXED**: Wired `BoundSnapshotForResponse` into `SkipMultiYearsAsync`, `MultiLoadAsync`, `ReadMultiCellAsync`, `ReadMultiCompetitionAsync`, `MultiHitTestAsync`, and `MultiCommandAsync` (preserving `stage4: null` / `ecology: null` when `CompositeFor` is null for F-2), attaching explicit `historyTruncated`, `totalRecordedYears`, `projectionCompacted`, `stage4Compacted`, and `payloadBudgetExceeded`; bounded `view` in `preview-review` & `history/year` refusals via `BoundSessionViewForAux`. | Verified across `repair-smoke` (`362/362`), `repair-edge-save` (`63/63`), and `repair-edge-step2` (`64/64`). |
| **N-4** | NIT (opt.) | Observed Outcome pill hard-coded `RECORDED`; Later Observed Comparison card header carried a misplaced `SESSION-ONLY` pill. | **FIXED**: Observed Outcome header reads `(d.annualOutcomeNextYear && d.annualOutcomeNextYear.provenance) || 'RECORDED'`; removed `SESSION-ONLY` pill from Later Observed Comparison card header (kept solely on the dedicated session-only canopy/light note below). | Verified in `ui-smoke-multiarea.sh` (`S2c`) and `repair-edge-step2` (`mixed-history-drawer-yr0-*.png`). |
| **N-5** | NIT (opt.) | History drawer uses `aria-modal="false"` without a JS focus trap. | **DEFERRED (documented)**: Optional accessibility polish; Escape focus-return to `#history-open` and non-destructive backdrop dismissal are verified in both `repair-smoke` (`S2d`) and `repair-edge-step2`. | Verified `S2d` and `repair-edge-step2` Escape focus-return. |

---

## 3. Files Changed & Diff Stat

All changes relative to base `5fe2fabe9073d7b72711f4ab48553c785351d7bc` remain strictly within the Step 2 write allowlist (**9 files total; ZERO Core edits**):

- `specs/multi-2d-phase1-time-history-ui-v1/spec.md` — updated with Owner G5 Year 30 horizon, `isOverdue`, `year > currentYear -> 409`, monotonic projection-only tiers (`projectionCompacted`, optional honest `stage4Compacted`, `pruneLog` in `historyTruncated`, `>= 1` event & `>= 1` annual record retention, explicit `payloadBudgetExceeded` + stderr log), and Results-pane zero-history honesty copy.
- `specs/multi-2d-phase1-time-history-ui-v1/plan.md` — updated payload bounding tier architecture and verification matrix.
- `specs/multi-2d-phase1-time-history-ui-v1/tasks.md` — recorded bounded repair tasks.
- `specs/multi-2d-phase1-time-history-ui-v1/checklists/requirements.md` — clarified `P1-UI-04` Year 30 horizon and `P1-UI-02` out-of-range 409 wording; **all 12 checkboxes remain unchecked `[ ]`**.
- `Tools/MultiRackSitkaP1Lab/src/Ui/PlayerMultiApi.cs` — `F-1`, `F-2`, `F-3`, `F-4`, `F-6`, `N-1`, `N-3`, `N-4` repairs.
- `Tools/MultiRackSitkaP1Lab/tests/ui-smoke-multiarea.sh` — `F-5` HTTP + DOM witness extensions (`362/362`).
- `Tools/MultiRackSitkaP1Lab/tests/phase1-time-history-ui/drive-step2.mjs` — `F-5` NB-2 order 2 at `1280×720` & `1920×1080`, Year 0..30 per-year budget & retention assertions, Load-over-live-session test.
- `Tools/MultiRackSitkaP1Lab/tests/phase1-time-history-ui/run-step2-edge.sh` — clean Year `0..30` HTTP sweeps for both `MA-1` and `MA-1-MIXED` saving structured JSON evidence.
- `Docs/Handoffs/MultiRack-Simulation-20261116/phase1-2d/PHASE1-2D-STEP2-TIME-HISTORY-UI-NEON-IMPLEMENTATION-20261125.md` — this updated handoff.

**Forbidden paths verified 100% untouched (`git diff --name-only 5fe2fabe..HEAD`):**
- `Tools/MultiRackSitkaP1Lab/src/Core/**` — **0 changes**
- `Tools/MultiRackSitkaP1Lab/src/Ui/wwwroot/**` — **0 changes**
- `Tools/MultiRackSitkaP1Lab/.p0ref/**` — **0 changes**
- `Assets/**`, `Packages/**`, `ProjectSettings/**`, `Docs/Project/coordination/**`, `.vscode/**`, `main` — **0 changes**

**Hygiene checks:**
- `data:image/webp` occurrences in `src/Ui/PlayerMultiApi.cs`: **0**
- Capability tokens (`/s/[0-9a-f]{64}`) in `git diff 5fe2fabe..HEAD`: **0**
- Checked boxes (`[x]`) in `specs/multi-2d-phase1-time-history-ui-v1/checklists/requirements.md`: **0** (all **12** remain `[ ]`)

---

## 4. Measured Year 0..30 `GET api/multi/view` & `ecology` Payload Tables (`P1-UI-04`, Owner G5 Horizon)

Hard budgets enforced at every Year `0..30`:
- Raw UTF-8 `GET api/multi/view` `<= 524,288` B (512 KiB)
- Raw UTF-8 `ecology` slice `<= 153,600` B (150 KiB)
- At least `1` latest work event (`ev >= 1` once work exists) and `1` latest annual record (`an >= 1` for Year `>= 1`) retained
- `payloadBudgetExceeded === false` (`over = false`) and empty server stderr

Measured directly over live loopback HTTP during `tests/phase1-time-history-ui/run-step2-edge.sh` (`repair-edge-step2/step2-sweep-ma-1.json` and `repair-edge-step2/step2-sweep-ma-1-mixed.json`).

### 4.1 `MA-1` Year 0..30 Sweep (Executed Fell + Prune at Years 0, 10, 20, 30)

Max raw view: **524,279 B** (Year 28, `<= 524,288 B`). Max `ecology`: **43,320 B** (Year 0, `<= 153,600 B`). Monotonic transition at Year 29 (`proj: false -> true`, `s4c: false`, `trunc: false`, `over: false`). All 31/31 years **PASS**; `stage4Compacted` never even activates (`false` at every year `0..30`).

| Year | Raw View (B) | Ecology (B) | `ev` | `an` | `totalRecYrs` | `trunc` | `proj` | `s4c` | `over` | Status |
|---:|---:|---:|---:|---:|---:|:---:|:---:|:---:|:---:|:---:|
| 0 | 391,058 | 43,320 | 1 | 0 | 1 | `false` | `false` | `false` | `false` | PASS |
| 1 | 397,695 | 42,744 | 1 | 1 | 2 | `false` | `false` | `false` | `false` | PASS |
| 2 | 401,585 | 41,571 | 1 | 2 | 3 | `false` | `false` | `false` | `false` | PASS |
| 3 | 403,734 | 41,080 | 1 | 3 | 4 | `false` | `false` | `false` | `false` | PASS |
| 4 | 405,354 | 40,807 | 1 | 4 | 5 | `false` | `false` | `false` | `false` | PASS |
| 5 | 408,715 | 39,874 | 1 | 5 | 6 | `false` | `false` | `false` | `false` | PASS |
| 6 | 411,022 | 39,426 | 1 | 6 | 7 | `false` | `false` | `false` | `false` | PASS |
| 7 | 414,337 | 38,587 | 1 | 7 | 8 | `false` | `false` | `false` | `false` | PASS |
| 8 | 416,825 | 38,010 | 1 | 8 | 9 | `false` | `false` | `false` | `false` | PASS |
| 9 | 419,410 | 37,400 | 1 | 9 | 10 | `false` | `false` | `false` | `false` | PASS |
| 10 | 422,341 | 37,073 | 2 | 10 | 11 | `false` | `false` | `false` | `false` | PASS |
| 11 | 425,212 | 36,335 | 2 | 11 | 12 | `false` | `false` | `false` | `false` | PASS |
| 12 | 427,587 | 35,901 | 2 | 12 | 13 | `false` | `false` | `false` | `false` | PASS |
| 13 | 437,445 | 37,291 | 2 | 13 | 14 | `false` | `false` | `false` | `false` | PASS |
| 14 | 445,400 | 37,815 | 2 | 14 | 15 | `false` | `false` | `false` | `false` | PASS |
| 15 | 447,337 | 36,953 | 2 | 15 | 16 | `false` | `false` | `false` | `false` | PASS |
| 16 | 452,940 | 36,618 | 2 | 16 | 17 | `false` | `false` | `false` | `false` | PASS |
| 17 | 457,005 | 36,255 | 2 | 17 | 18 | `false` | `false` | `false` | `false` | PASS |
| 18 | 464,663 | 35,885 | 2 | 18 | 19 | `false` | `false` | `false` | `false` | PASS |
| 19 | 473,135 | 35,578 | 2 | 19 | 20 | `false` | `false` | `false` | `false` | PASS |
| 20 | 476,298 | 34,214 | 3 | 20 | 21 | `false` | `false` | `false` | `false` | PASS |
| 21 | 486,057 | 35,317 | 3 | 21 | 22 | `false` | `false` | `false` | `false` | PASS |
| 22 | 490,756 | 34,282 | 3 | 22 | 23 | `false` | `false` | `false` | `false` | PASS |
| 23 | 495,290 | 33,672 | 3 | 23 | 24 | `false` | `false` | `false` | `false` | PASS |
| 24 | 499,271 | 33,319 | 3 | 24 | 25 | `false` | `false` | `false` | `false` | PASS |
| 25 | 504,081 | 32,822 | 3 | 25 | 26 | `false` | `false` | `false` | `false` | PASS |
| 26 | 512,286 | 33,195 | 3 | 26 | 27 | `false` | `false` | `false` | `false` | PASS |
| 27 | 516,961 | 32,112 | 3 | 27 | 28 | `false` | `false` | `false` | `false` | PASS |
| 28 | 524,279 | 32,749 | 3 | 28 | 29 | `false` | `false` | `false` | `false` | PASS |
| 29 | 400,967 | 32,108 | 3 | 29 | 30 | `false` | `true` | `false` | `false` | PASS |
| 30 | 407,861 | 32,650 | 4 | 30 | 31 | `false` | `true` | `false` | `false` | PASS |

### 4.2 `MA-1-MIXED` Year 0..30 Sweep (All 6 Action Families Executed at Year 0 + Fell/Prune at Years 10, 20, 30)

Max raw view: **524,032 B** (Year 27, `<= 524,288 B`). Max `ecology`: **49,685 B** (Year 19, `<= 153,600 B`, less than 33% of cap). Monotonic transitions: Tier 0 (`Y00..Y16`) → Tier 1 full history (`Y17..Y26`, `proj: true, trunc: false`) → Tier 1 tail-trimmed history (`Y27..Y30`, `an: 25 -> 19 -> 12 -> 6`, `ev: 3..4`, `s4c: false`, `over: false`). All 31/31 years **PASS**; `stage4Compacted` never activates (`false` at every year `0..30`).

| Year | Raw View (B) | Ecology (B) | `ev` | `an` | `totalRecYrs` | `trunc` | `proj` | `s4c` | `over` | Status |
|---:|---:|---:|---:|---:|---:|:---:|:---:|:---:|:---:|:---:|
| 0 | 393,452 | 45,045 | 1 | 0 | 1 | `false` | `false` | `false` | `false` | PASS |
| 1 | 400,092 | 44,487 | 1 | 1 | 2 | `false` | `false` | `false` | `false` | PASS |
| 2 | 404,236 | 43,209 | 1 | 2 | 3 | `false` | `false` | `false` | `false` | PASS |
| 3 | 406,621 | 42,618 | 1 | 3 | 4 | `false` | `false` | `false` | `false` | PASS |
| 4 | 408,894 | 42,258 | 1 | 4 | 5 | `false` | `false` | `false` | `false` | PASS |
| 5 | 412,591 | 41,018 | 1 | 5 | 6 | `false` | `false` | `false` | `false` | PASS |
| 6 | 415,211 | 40,471 | 1 | 6 | 7 | `false` | `false` | `false` | `false` | PASS |
| 7 | 418,897 | 39,386 | 1 | 7 | 8 | `false` | `false` | `false` | `false` | PASS |
| 8 | 421,935 | 38,584 | 1 | 8 | 9 | `false` | `false` | `false` | `false` | PASS |
| 9 | 425,122 | 37,770 | 1 | 9 | 10 | `false` | `false` | `false` | `false` | PASS |
| 10 | 429,561 | 37,955 | 2 | 10 | 11 | `false` | `false` | `false` | `false` | PASS |
| 11 | 435,963 | 37,994 | 2 | 11 | 12 | `false` | `false` | `false` | `false` | PASS |
| 12 | 457,675 | 41,736 | 2 | 12 | 13 | `false` | `false` | `false` | `false` | PASS |
| 13 | 475,601 | 44,496 | 2 | 13 | 14 | `false` | `false` | `false` | `false` | PASS |
| 14 | 486,253 | 45,141 | 2 | 14 | 15 | `false` | `false` | `false` | `false` | PASS |
| 15 | 501,355 | 46,008 | 2 | 15 | 16 | `false` | `false` | `false` | `false` | PASS |
| 16 | 512,449 | 44,917 | 2 | 16 | 17 | `false` | `false` | `false` | `false` | PASS |
| 17 | 405,637 | 46,439 | 2 | 17 | 18 | `false` | `true` | `false` | `false` | PASS |
| 18 | 420,563 | 48,378 | 2 | 18 | 19 | `false` | `true` | `false` | `false` | PASS |
| 19 | 435,402 | 49,685 | 2 | 19 | 20 | `false` | `true` | `false` | `false` | PASS |
| 20 | 445,827 | 49,228 | 3 | 20 | 21 | `false` | `true` | `false` | `false` | PASS |
| 21 | 453,638 | 48,112 | 3 | 21 | 22 | `false` | `true` | `false` | `false` | PASS |
| 22 | 460,035 | 45,898 | 3 | 22 | 23 | `false` | `true` | `false` | `false` | PASS |
| 23 | 472,137 | 46,379 | 3 | 23 | 24 | `false` | `true` | `false` | `false` | PASS |
| 24 | 486,062 | 46,895 | 3 | 24 | 25 | `false` | `true` | `false` | `false` | PASS |
| 25 | 498,510 | 47,034 | 3 | 25 | 26 | `false` | `true` | `false` | `false` | PASS |
| 26 | 510,723 | 46,825 | 3 | 26 | 27 | `false` | `true` | `false` | `false` | PASS |
| 27 | 524,032 | 47,475 | 3 | 25 | 28 | `true` | `true` | `false` | `false` | PASS |
| 28 | 522,957 | 46,028 | 3 | 19 | 29 | `true` | `true` | `false` | `false` | PASS |
| 29 | 523,112 | 46,109 | 3 | 12 | 30 | `true` | `true` | `false` | `false` | PASS |
| 30 | 523,968 | 48,441 | 4 | 6 | 31 | `true` | `true` | `false` | `false` | PASS |

In addition, the interactive Edge workflows (`ma1-full` and `mixed-play-save`) independently stepped year-by-year through Year 30 and verified:
- `MA-1` (`ma1-full` Year 9..30): `max_raw = 521,312 B <= 524,288`, `max_eco = 37,807 B <= 153,600`, `>= 1` event & `>= 1` annual record at every year, full history intact (`totalRecordedYears = 31`).
- `MA-1-MIXED` (`mixed-play-save` Year 16..30): `max_raw = 524,272 B <= 524,288`, `max_eco = 48,425 B <= 153,600`, `historyTruncated = true` at Year 30 (`522,472 B`, `ev = 3`, `an = 8`, `s4c = false`, `over = false`), and `POST api/multi/history/year` for Year 0 remained 100% intact.

---

## 5. Exact Build, Test, Smoke & Real-Edge Verification Results (Repair Pass)

Environment: WSL2 Ubuntu, `PATH="$HOME/.dotnet:$HOME/.local/share/vite-plus/bin:$PATH"` (`dotnet 8.0.134`, Linux `node v26.13.1`), Windows `node.exe v26.13.1`, Windows Microsoft Edge (`--headless=new`), `TMPDIR=/home/jer/ccf-p1ui-tmp`.

| Gate | Exact Command | Result |
|---|---|---|
| 1. Solution build | `dotnet build MultiRackSitkaP1Lab.sln --disable-build-servers` | **Build succeeded — 0 Warning(s), 0 Error(s)** |
| 2. Core tests | `dotnet tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests.dll` | **277/277 passed, 0 failed** (`repair-core-tests.log`) |
| 3. Save tests build + run | `dotnet build tests/Phase1.Save.Tests/Phase1Save.Tests.csproj --disable-build-servers && dotnet run --no-build --project tests/Phase1.Save.Tests/Phase1Save.Tests.csproj` | **0 Warning(s), 0 Error(s); 50/50 passed, 0 failed** (`repair-save-tests.log`) |
| 4. UI build | `dotnet build src/Ui/SitkaP1.Ui.csproj --disable-build-servers` | **Build succeeded — 0 Warning(s), 0 Error(s)** |
| 5. Multi-area HTTP + DOM smoke | `TMPDIR=/home/jer/ccf-p1ui-tmp tests/ui-smoke-multiarea.sh <evidence>/repair-smoke` | **OVERALL: PASS (362 passed, 0 failed)** (`repair-smoke/ui-smoke-multiarea.txt`; clean SIGINT; empty stderr; tree & git status unchanged) |
| 6. Existing Save/Load real-Edge E2E | `TMPDIR=/home/jer/ccf-p1ui-tmp tests/phase1-save-ui/run-edge-e2e.sh <evidence>/repair-edge-save` | **OVERALL: PASS (63 passed, 0 failed)** (`repair-edge-save/run.log`; 4 server processes, all SIGINT exit 0, empty stderr) |
| 7. Step 2 real-Edge + Year 0..30 sweeps | `TMPDIR=/home/jer/ccf-p1ui-tmp tests/phase1-time-history-ui/run-step2-edge.sh <evidence>/repair-edge-step2` | **OVERALL: PASS (64 passed, 0 failed)** (`repair-edge-step2/run.log`: `ma1-full` 28/28, `mixed-play-save` 18/18, `mixed-restart-load` 6/6, `sweep-ma-1` 1/1 [31/31 yrs], `sweep-ma-1-mixed` 1/1 [31/31 yrs], plus 10 SIGINT/stderr checks across all 5 server processes) |

### NB-2 Plant-Drawer Partial-Refusal Proofs (`P1-UI-06`, Orders 2 & 3 at BOTH Resolutions)

- **Order 3 (`SpotClear` duplicate, leaving `Plant` + `Shelter` queued):**
  - `1280×720`: `scrollTop = 0`, `#plant-result` `top=469..701` inside `#pane-tree` (`331..712`), `Phase2DuplicateOrder`, `"Already queued"`, and both Remove buttons (`P2W0002` `611..641`, `P2W0003` `656..686`, `right=1215`) strictly visible without scrolling; real clicks cleared both partial orders (`nb2-plant-partial-refusal-1280x720.png`).
  - `1920×1080`: `scrollTop = 0`, `#plant-result` `top=570..807` inside `#pane-tree` (`450..1068`), both Remove buttons (`P2W0005` `698..734`, `P2W0006` `750..786`, `right=1844`) strictly visible without scrolling (`nb2-plant-partial-refusal-1920x1080.png`).
- **Order 2 (`Shelter` duplicate with pre-scrolled `#pane-tree`, leaving `Plant` queued and `SpotClear` unattempted):**
  - `1280×720`: `scrollTop = 0`, `Stopped at Shelter`, `Phase2DuplicateOrder`, `"Already queued"`, `"Not attempted: SpotClear."`, and Remove button (`P2W0009` `610..640`, `right=1215`) strictly visible without scrolling; real click cleared queued Plant (`nb2-order2-partial-refusal-1280x720.png`).
  - `1920×1080`: `scrollTop = 0`, `Stopped at Shelter`, `Phase2DuplicateOrder`, `"Already queued"`, `"Not attempted: SpotClear."`, and Remove button (`P2W0012` `698..734`, `right=1844`) strictly visible without scrolling (`nb2-order2-partial-refusal-1920x1080.png`).

### Save → Restart → Load & Load-Over-Live-Session Continuity (`P1-UI-05`, `F-5`)

- Fresh server restart (`mixed-b`): loaded Year 15 slot `step2-e2e` → Preview (`target Yr 19, anchor 0`), History (`16` years, exact Year 0 Work/Outcome), and Structure (`M_i = 0.3790`, `T_i = 0.2594`, `n = 197`) matched pre-save exactly.
- Load over live session (`mixed-b` advanced to Year 19 with History drawer open showing 20 years + Year 19 detail + Skip summary visible + Details Structure open with `n = 205`): loading `step2-e2e` atomically refreshed History (`20 -> 16`), cleared stale Year 19 detail and Skip summary, and refreshed Structure (`205 -> 197`) with zero console errors or stale-session requests.

---

## 6. Nine Frozen Anchors, `.p0ref` Pin & Protected Listeners

All 9 frozen anchors were re-observed exact in `repair-core-tests.log` and `sha256sum`:

| # | Anchor | Observed SHA-256 | Match |
|---|---|---|---|
| 1 | Single-area Model 0 | `7e3a2a869ec1662458b74875cadee1e4b16cf2b87b0aaa05df2d9a0061311ece` | **EXACT** |
| 2 | Multi-area `0/0/0/0` | `a692a6d5bc374dd0225153b3bf0e0b0e9467d3c041b0a6b03414f42d3c02233d` | **EXACT** |
| 3 | Multi-area `0/0/1/0` (Crown C2) | `814e81650b62428510fe60c3142b51d9055537d6014303074f08733bf8d46287` | **EXACT** |
| 4 | Stage 4 `1/1/1/0` | `8a926776ffa3ca3dd1611c2e4d462f8c82b4d78f29ad5e83e5a8f8ce1d2b87fb` | **EXACT** |
| 5 | Stage 5 `1/1/1/1` | `c4b670ede36ef133fb7ab9b43992319b340f27827ce85025ae68444c63d2dc35` | **EXACT** |
| 6 | Stage 5 5.0 m purity | `c67edc6b7a8b1633b1b48d4016c3e89022fbd00393121bb9b60d083a16f653d8` | **EXACT** |
| 7 | Phase 2 Q6 (`1/1/1/1/1` vs `1/1/1/1/0`) | `776a68df048a640c849801e743db3372d9d6d76808828d0e0623222ae35ce22d` | **EXACT** |
| 8 | P0 Core binary pin (`.p0ref/SitkaLab.Core.dll`) | `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3` | **EXACT** |
| 9 | MA-1-MIXED 30-year fingerprint | `93e9abda0670cb7a7f56fda91cc1e96e8bc931b330f57d99ec1b588600b9251f` | **EXACT** |

- **Protected listeners (verified alive before and after, untouched):**
  - `127.0.0.1:33767` (`LISTEN`, PID `1750123`)
  - `127.0.0.1:43649` (`LISTEN`, PID `1806361`)
  - `127.0.0.1:46593` (`LISTEN`, PID `839989`)
- **Ephemeral cleanup:** Zero leftover `sitkap1-ui.dll` processes from this lane and zero lingering `ccf-step2-edge`/`ccf-edge-` `msedge.exe` processes.

---

## 7. Deviations & Residual Risks (Disclosed Transparently)

### Deviations
1. **Optional nits `N-2` (adjacent read-only locks) and `N-5` (History drawer focus trap) deferred:** Left intentionally out of this bounded repair pass to preserve zero `src/Core/**` changes and minimal blast radius; both are non-blocking optional nits per review §12.
2. **Tier 2 (`stage4Compacted`) fallback and `null` Structure metric path:** Tier 1 (`projectionCompacted = true, stage4Compacted = false`) keeps every Year `0..30` under `524,288 B` on both `MA-1` and `MA-1-MIXED`, so Tier 2 (`stage4Compacted = true`) is not triggered within the Year 30 horizon (and remains implemented as an honest, spec-documented safety tier omitting `contributingTreeIds` via null omission rather than `[]` and keeping `{ xm, zm, u, v }` vertices). Similarly, live `null -> Unavailable` Structure rendering remains covered by Core `P1-TH-06b` and smoke DOM witness `S2f` (`WorkWouldLeaveNoLivingStems` prevents `< 2` living stems in live play).

### Residual Risks
1. **Horizons far beyond Year 30 (e.g., Year 45–50+ on dense mixed recruitment):** Under the binding Owner G5 horizon (Year 30), `GET api/multi/view` stays `<= 524,288 B` and `ecology <= 49,685 B` (`< 33%` of `153,600 B`) at every year `0..30` on both experiments. If a future phase extends the required UI horizon to Year 50+, Tier 2 (`stage4Compacted = true`) activates automatically and, if even Tier 2 ever exceeded `524,288 B`, Tier 3 explicitly sets `payloadBudgetExceeded = true` and logs to `Console.Error` rather than returning over-budget bytes silently.
2. **Pending independent re-review:** All 12 acceptance checklist boxes remain `[ ]` until independent review verifies this repair pass.

---

## 8. Evidence Archive Inventory (Outside Repository)

Path: `/home/jer/Development/CCF-Multi-Phase1-Step2-Evidence-20261125` (mode `700`, zero capability tokens in Git):
- **Repair pass artifacts:**
  - `repair-core-tests.log` (Core `277/277`)
  - `repair-save-tests.log` (Save `50/50`)
  - `repair-smoke/ui-smoke-multiarea.txt` (`362/362`, capability-redacted)
  - `repair-edge-save/run.log` + 12 PNG screenshots (`63/63`)
  - `repair-edge-step2/run.log` + `step2-sweep-ma-1.json` + `step2-sweep-ma-1-mixed.json` + `step2-pre-save.json` + 5 capability-redacted server logs + 17 PNG screenshots including NB-2 order 2 & order 3 at `1280×720` and `1920×1080` (`64/64`)
- **Initial pass artifacts (preserved untouched):** `core-tests.log`, `save-tests.log`, `smoke/`, `edge-save/`, `edge-step2/`
