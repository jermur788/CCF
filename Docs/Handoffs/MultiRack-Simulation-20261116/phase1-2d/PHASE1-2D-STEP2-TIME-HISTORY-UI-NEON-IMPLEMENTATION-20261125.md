# CCF Multi 2D — Phase 1 Step 2 (Time Pacing, Decision→Outcome History & Woodland Structure UI/HTTP + NB-2): Implementation and Verification Handoff

**Date:** 2026-11-25 (desktop-local lane, WSL2 Ubuntu + Windows Microsoft Edge headless CDP).
**Role & sole writer:** Primary Implementer — Gemini 4 Neon (sole writer on the dedicated Step 2 worktree and branch; zero subagents spawned).
**Specification:** [`specs/multi-2d-phase1-time-history-ui-v1/spec.md`](../../../../specs/multi-2d-phase1-time-history-ui-v1/spec.md) (`SPEC-multi-2d-phase1-time-history-ui-V1`).

> **EXPLICIT STATE:** This work is **IMPLEMENTED ON TASK BRANCH** only. It is **NOT REVIEWED**, **NOT INTEGRATED**, and **NOT ACCEPTED**. Every checkbox in `specs/multi-2d-phase1-time-history-ui-v1/checklists/requirements.md` remains unchecked `[ ]` pending independent review.

| Label | State |
|---|---|
| IMPLEMENTED | **YES — IMPLEMENTED ON TASK BRANCH** (Spec quartet + 5 HTTP endpoints + bounded multi-decade `api/multi/view` + `/multi` UI controls + NB-2 Plant-drawer polish + smoke & real-Edge suites) |
| REVIEWED | **NO — NOT REVIEWED** (independent review pending; `P1-UI-REVIEW` unchecked) |
| INTEGRATED | **NO — NOT INTEGRATED** (task branch only; zero merge to `main`, zero PR, zero tags) |
| VERIFIED (author gates) | **YES for Step 2 author gates** (`0/0` warnings/errors on `.sln`, `Phase1Save.Tests`, `SitkaP1.Ui`; Core `277/277`; Save `50/50`; `ui-smoke-multiarea.sh` `352/352`; `phase1-save-ui/run-edge-e2e.sh` `63/63`; `phase1-time-history-ui/run-step2-edge.sh` `59/59`; all 9 frozen anchors & `.p0ref` hash exact) |
| ACCEPTED | **NO — NOT ACCEPTED** |

---

## 1. Lane, Base, Toolchain & Commits

- **Repository root:** `/home/jer/Development/CCF-HP`
- **Worktree:** `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-2D-Phase1-TimeHistoryUI-20261125`
- **Branch:** `feat/multi-2d-phase1-time-history-ui-20261125-v1`
- **Base commit (exact):** `5fe2fabe9073d7b72711f4ab48553c785351d7bc` (Step 1 Core, independently APPROVED at `6c83d16e`)
- **Verified code + spec + test HEAD:** `6d4c406fa37dbba0e5a66cdf44c30bc30bc217f3` (this handoff commit sits immediately above it, adding only this Markdown file)
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
| 6 | *(this handoff commit)* | `docs(phase1-2d): record Step 2 implementation and verification handoff` |

---

## 2. Files Changed & Diff Stat

### `git diff --stat 5fe2fabe9073d7b72711f4ab48553c785351d7bc..6d4c406fa37dbba0e5a66cdf44c30bc30bc217f3`

```text
 Tools/MultiRackSitkaP1Lab/src/Ui/PlayerMultiApi.cs | 1439 +++++++++++++++++++-
 .../tests/phase1-time-history-ui/drive-step2.mjs   |  790 +++++++++++
 .../tests/phase1-time-history-ui/run-step2-edge.sh |  127 ++
 .../tests/ui-smoke-multiarea.sh                    |  277 +++-
 .../checklists/requirements.md                     |   70 +
 specs/multi-2d-phase1-time-history-ui-v1/plan.md   |   95 ++
 specs/multi-2d-phase1-time-history-ui-v1/spec.md   |  249 ++++
 specs/multi-2d-phase1-time-history-ui-v1/tasks.md  |   34 +
 8 files changed, 3051 insertions(+), 30 deletions(-)
```

Plus this handoff file in the final docs commit:
- `Docs/Handoffs/MultiRack-Simulation-20261116/phase1-2d/PHASE1-2D-STEP2-TIME-HISTORY-UI-NEON-IMPLEMENTATION-20261125.md` (new)

**Total files changed relative to `5fe2fabe…`: 9 files — every file strictly inside the write allowlist. ZERO Core edits were required.**

| File | Summary of Change |
|---|---|
| `specs/multi-2d-phase1-time-history-ui-v1/spec.md` | Step 2 specification (`SPEC-multi-2d-phase1-time-history-ui-V1`) |
| `specs/multi-2d-phase1-time-history-ui-v1/plan.md` | Step 2 architecture and verification plan |
| `specs/multi-2d-phase1-time-history-ui-v1/tasks.md` | Step 2 bounded task breakdown |
| `specs/multi-2d-phase1-time-history-ui-v1/checklists/requirements.md` | Acceptance checklist (`P1-UI-01`..`P1-UI-08`, `P1-UI-REG`, `P1-UI-REVIEW`), all 12 boxes unchecked `[ ]` |
| `Tools/MultiRackSitkaP1Lab/src/Ui/PlayerMultiApi.cs` | Five new `/multi` HTTP endpoints in `AddMultiAreaRoutes()`, projection-only bounded `SerializeBoundedMultiView` (`<= 524,288` B), embedded `MultiAreaPage` Skip to Review control, on-demand History slide-over drawer, on-demand Structure section inside `#pane-details`, and NB-2 Plant-drawer top-of-panel `#plant-result` polish |
| `Tools/MultiRackSitkaP1Lab/tests/ui-smoke-multiarea.sh` | Strictly additive Step 2 HTTP contract/guard checks (`W186`..`W251`), `M14` (`MA-1-MIXED` structure + Year-0 preview), `pageFetch` query preservation + `null-structure` stand-in fault, and `S2a`..`S2i` DOM witness checks (all 276 prior checks untouched) |
| `Tools/MultiRackSitkaP1Lab/tests/phase1-time-history-ui/run-step2-edge.sh` | Real headless Microsoft Edge Step 2 runner (3 independent WSL2 server processes: `ma1`, `mixed-a`, `mixed-b` restart, clean SIGINT teardown, capability-redacted logs) |
| `Tools/MultiRackSitkaP1Lab/tests/phase1-time-history-ui/drive-step2.mjs` | Real CDP driver exercising the 4-cell resolution×experiment matrix, NB-2 at `1280×720` & `1920×1080`, Skip pacing, History drawer paging & honesty, Structure, measured multi-decade payload budgets & active truncation, and Save → quit → restart → Load continuity |
| `Docs/Handoffs/MultiRack-Simulation-20261116/phase1-2d/PHASE1-2D-STEP2-TIME-HISTORY-UI-NEON-IMPLEMENTATION-20261125.md` | This durable implementation and verification handoff |

**Forbidden paths verified 100% untouched (`git diff --name-only 5fe2fabe..HEAD`):**
- `Tools/MultiRackSitkaP1Lab/src/Core/**` (including `src/Core/Phase1/**`, `src/Core/MultiArea/**`, biology, Step 1 math, anchors) — **0 changes**
- `Tools/MultiRackSitkaP1Lab/src/Ui/wwwroot/**` (single-area UI) — **0 changes**
- `Tools/MultiRackSitkaP1Lab/.p0ref/**` — **0 changes**
- `Assets/**`, `Packages/**`, `ProjectSettings/**`, `Docs/Project/coordination/**`, `.vscode/**`, `main` — **0 changes**

**Hygiene checks:**
- `data:image/webp` occurrences in `src/Ui/PlayerMultiApi.cs`: **0**
- Capability tokens (`/s/[0-9a-f]{64}`) in `git diff 5fe2fabe..HEAD`: **0**
- Checked boxes (`[x]`) in `specs/multi-2d-phase1-time-history-ui-v1/checklists/requirements.md`: **0** (all **12** remain `[ ]`)
- `git status --short` (before handoff commit): clean

---

## 3. Five HTTP Endpoints Implemented (Exact Contracts & Status Codes)

All five routes sit under `/s/<capability>/` inside `PlayerApi.AddMultiAreaRoutes()` and pass through the unchanged `HandleAsync` guard chain (`Host` `403`, capability `404`, method `405`, `Origin` `403`, `Content-Type` `415`, `16 KiB` body cap `413`, strict camelCase JSON `400`, unhandled fault `500 OutcomeUnconfirmed`):

| Endpoint | Request | Core Call | Exact Status Codes & Behaviour |
|---|---|---|---|
| `POST api/multi/year/preview-review` | `{ fence }` (`MultiFenceMembers`) | `multiSession.PreviewNextReviewYear(fence)` | `200`: `currentYear`, `eligible`, `suggestedTargetYear`, `suggestedGapYears`, `anchorFellYear`, `isObservationPeriod`, `maxTicksPerPress`, `stopReason`, `calibrationNote`, `label`, fresh `fence` & bounded `view`, `refusal: null`. `409`: stale fence + fresh `fence`/`view`. `400/403/404/405/413/415`: guard refusals. Read-only, zero mutation. |
| `POST api/multi/year/skip` | `{ fence, targetYear }` (`MultiSkipMembers`) | `multiSession.AdvanceYears(fence, targetYear, ct)` (NEVER passes `consentToCancelWork = true`) | `200`: `startYear`, `destinationYear`, `requestedTargetYear`, `completedYears`, `reachedTarget`, `stopReason` (`ReachedReviewYear` etc.), `stopDetail`, `isObservationPeriod`, `anchorFellYear`, `calibrationNote`, fresh bounded `view`, `scenes`, `stage4`, `ecology`, `historyTruncated`, `totalRecordedYears`, `refusal: null`. `409`: stale fence, `WorkPlanOpen` (Pending or Approved untouched), `InvalidTargetYear`, or 0-step refusal + fresh composite payload. `400/403/404/405/413/415`: guards. `500`: `OutcomeUnconfirmed`. |
| `GET api/multi/history/index` | Query `?pageSize=&beforeYear=` | `multiSession.ReadHistoryIndex(pageSize, beforeYear)` | `200`: newest-first `items`, clamped `pageSize` (`1..50`, default `10`), `beforeYear`, `nextCursorBeforeYear`, `totalRecordedYears`. `400`: unknown, duplicate, or malformed query params. `403/404/405`: guards. Read-only, zero mutation. |
| `POST api/multi/history/year` | `{ fence, year }` (`MultiHistoryYearMembers`) | Read-only fence check vs `multiArea.Read()`, then `multiSession.ReadHistoryYear(year)` | `200`: full `HistoryYearDetail` (`workYear = Y`, `observedYear = Y + 1`, `workByArea`, `annualOutcomeNextYear`, `plantedWorkFollowUp`, `laterObservedComparison`, `notRecordedDisclosures`, `provenanceGlossary`). `409`: stale fence (`PlayerSessionMismatch`/`PlayerSourceStale`/`PlayerPlanStateStale`), negative year (`HistoryYearInvalid`), or unrecorded year (`HistoryYearNotRecorded`) + fresh `fence`/`view`. `400/403/404/405/413/415`: guards. Read-only. |
| `GET api/multi/structure` | No query params | `multiSession.ComputeStructure()` | `200`: `meanMingling`, `meanDiameterDifferentiation`, `focalLivingTreeCount`, `validDiameterFocalCount`, `effectiveNeighbourCount`, `maxNeighbourCountK: 3`, `areas`, `provenance: "DERIVED"`, `provenanceLabel: "DERIVED"`, exact `honestyLabel`. `400`: unexpected query params. `403/404/405`: guards. Read-only. |

---

## 4. Measured `GET api/multi/view` & `ecology` Payload Budgets (`P1-UI-04`)

Hard budgets: raw UTF-8 `GET api/multi/view` `<= 524,288` B (512 KiB) and `ecology` slice `<= 153,600` B (150 KiB).

Measured directly over live loopback HTTP during `tests/phase1-time-history-ui/run-step2-edge.sh` (`edge-step2/run.log`):

| Experiment | Ecological Year | Executed Work Present | Raw `GET api/multi/view` (B) | Limit (B) | `ecology` Slice (B) | Limit (B) | `historyTruncated` | `totalRecordedYears` | Status |
|---|---|---|---|---|---|---|---|---|---|
| `MA-1` | Year 0 | None (fresh) | **389,762** | 524,288 | **43,428** | 153,600 | `false` | 0 | **PASS** |
| `MA-1` | Year 5 | Year-4 Fell | **414,220** | 524,288 | **45,444** | 153,600 | `false` | 6 | **PASS** |
| `MA-1` | Year 15 | Year-4 Fell | **458,842** | 524,288 | **50,576** | 153,600 | `false` | 16 | **PASS** |
| `MA-1` | Year 26 (truncation proof) | Years 4, 15, 18, 22 Fells | **522,939** | 524,288 | **59,835** | 153,600 | **`true`** | 27 | **PASS** |
| `MA-1-MIXED` | Year 0 | None (fresh) | **391,026** | 524,288 | **44,371** | 153,600 | `false` | 0 | **PASS** |
| `MA-1-MIXED` | Year 5 | Year-0 all 6 action families | **419,173** | 524,288 | **47,668** | 153,600 | `false` | 6 | **PASS** |
| `MA-1-MIXED` | Year 15 | Year-0 all 6 action families | **516,691** | 524,288 | **63,723** | 153,600 | `false` | 16 | **PASS** |
| `MA-1-MIXED` | Year 20 (stage-2 projection compaction) | Year-0 all 6 action families | **511,193** | 524,288 | **80,223** | 153,600 | `false` | 21 | **PASS** |

- At `MA-1` Year 26 (`historyTruncated: true`, `522,939 B <= 524,288 B`), `GET api/multi/history/index?pageSize=50` still returned all `totalRecordedYears = 27` years and `POST api/multi/history/year` for Year 4 returned the complete committed Year-4 record with zero Core root truncation.
- At `MA-1-MIXED` Year 20, Stage 2 projection compaction (`CompactStage4ForBudget`) reduced redundant summary-only projection fields (`contributingTreeIds` still served in full by `POST api/multi/cell`, 3-decimal ground vertex rounding, null-omitted compact tree projection) so the full response stayed `511,193 B <= 524,288 B` with zero Core mutation and zero visual change.

---

## 5. Exact Build, Test, Smoke & Real-Edge Verification Results

Environment: WSL2 Ubuntu, `PATH="$HOME/.dotnet:$HOME/.local/share/vite-plus/bin:$PATH"` (`dotnet 8.0.134`, Linux `node v26.13.1`), Windows `node.exe v26.13.1`, Windows Microsoft Edge (`--headless=new`), `TMPDIR=/home/jer/ccf-p1ui-tmp`.

| Gate | Exact Command | Result |
|---|---|---|
| Solution build | `dotnet build MultiRackSitkaP1Lab.sln --disable-build-servers` | **Build succeeded — 0 Warning(s), 0 Error(s)** |
| Core tests | `dotnet tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests.dll` | **277/277 passed, 0 failed** |
| Save tests build + run | `dotnet build tests/Phase1.Save.Tests/Phase1Save.Tests.csproj --disable-build-servers && dotnet run --no-build --project tests/Phase1.Save.Tests/Phase1Save.Tests.csproj` | **0 Warning(s), 0 Error(s); 50/50 passed, 0 failed** |
| UI build | `dotnet build src/Ui/SitkaP1.Ui.csproj --disable-build-servers` | **Build succeeded — 0 Warning(s), 0 Error(s)** |
| Multi-area HTTP + DOM smoke | `TMPDIR=/home/jer/ccf-p1ui-tmp tests/ui-smoke-multiarea.sh <evidence>/smoke` | **OVERALL: PASS (352 passed, 0 failed)** (all 276 existing + 76 new Step 2 checks; clean SIGINT; empty stderr; tree & git status unchanged) |
| Existing Save/Load real-Edge E2E | `TMPDIR=/home/jer/ccf-p1ui-tmp tests/phase1-save-ui/run-edge-e2e.sh <evidence>/edge-save` | **OVERALL: PASS (63 passed, 0 failed)** (4 server processes, all SIGINT exit 0, empty stderr) |
| New Step 2 real-Edge suite | `TMPDIR=/home/jer/ccf-p1ui-tmp tests/phase1-time-history-ui/run-step2-edge.sh <evidence>/edge-step2` | **OVERALL: PASS (59 passed, 0 failed)** (`ma1-full` 29/29, `mixed-play-save` 19/19, `mixed-restart-load` 5/5, plus 6 SIGINT/stderr checks) |

### 4-Cell Real-Edge Matrix (`P1-UI-07`)

| Experiment | Viewport | Horizontal Scroll | Topbar Layout | R3 5-Competitor Above-the-Fold (1280×720) | Console / Exception / CSP / 5xx | Screenshots (`<evidence>/edge-step2/`) |
|---|---|---|---|---|---|---|
| `MA-1` | `1920×1080` | **0** (`1920 <= 1920`) | Single row (`h=78px`, `delta=7px`) | N/A (fits comfortably) | **0** | `ma1-desktop-1920x1080.png`, `nb2-plant-partial-refusal-1920x1080.png` |
| `MA-1` | `1280×720` | **0** (`1280 <= 1280`) | Single row (`h=53px`, `delta=3px`) | **PASS** (`scrollTop=0`, `rowCount=5`, `refTop=239`, `addBottom=632 <= 712`) | **0** | `ma1-r3-fold-1280x720.png`, `nb2-plant-partial-refusal-1280x720.png`, `ma1-skip-workplan-open-1280x720.png`, `ma1-skip-yr4-obs-1280x720.png`, `ma1-structure-1280x720.png` |
| `MA-1-MIXED` | `1920×1080` | **0** (`1920 <= 1920`) | Single row (`h=78px`, `delta=7px`) | N/A (fits comfortably) | **0** | `mixed-desktop-1920x1080.png`, `mixed-history-drawer-yr0-1920x1080.png`, `restored-history-1920x1080.png` |
| `MA-1-MIXED` | `1280×720` | **0** (`1280 <= 1280`) | Single row (`h=53px`, `delta=3px`) | **PASS** (`scrollTop=0`, `rowCount=5`, `refTop=239`, `addBottom=632 <= 712`) | **0** | `mixed-r3-fold-1280x720.png`, `mixed-history-drawer-yr0-1280x720.png`, `mixed-structure-1280x720.png`, `restored-structure-1280x720.png`, `restored-history-1280x720.png` |

### NB-2 Plant-Drawer Partial-Refusal Proof (`P1-UI-06`)

Reproduced in real headless Microsoft Edge by pre-queueing `SpotClear` at a valid open point, then queueing `Plant + Shelter + SpotClear` at the same point (step 3 hits `409 Phase2DuplicateOrder` leaving steps 1 & 2 queued):

- **`1280×720`:** `pane.scrollTop === 0`, `#plant-result` rect `top=469..bottom=701` inside `#pane-tree` (`331..712`) and viewport (`0..720`), `<code>Phase2DuplicateOrder</code>` visible, `"Already queued (one-click remove below):"` visible, both `[data-planting-remove]` Remove buttons (`P2W0002` `top=611..641`, `P2W0003` `top=656..686`, `right=1215`) strictly visible without scrolling, and real clicks on both Remove buttons cleared the partial orders cleanly. Visually verified in `nb2-plant-partial-refusal-1280x720.png`.
- **`1920×1080`:** `pane.scrollTop === 0`, `#plant-result` rect `top=570..bottom=807` inside `#pane-tree` (`450..1068`) and viewport (`0..1080`), both Remove buttons (`P2W0005` `top=698..734`, `P2W0006` `top=750..786`, `right=1844`) strictly visible without scrolling, and real clicks cleared both partial orders. Visually verified in `nb2-plant-partial-refusal-1920x1080.png`.

### Save → Quit → Restart → Load Continuity Proof (`P1-UI-05`)

- `mixed-a` (`MA-1-MIXED`): executed all 6 action families (`Fell`, `Prune`, `ClearUndergrowth`, `Plant`, `Shelter`, `SpotClear`) in Year 0 → Skip to Year 4 → Advance to Year 5 → Skip to Year 15 → opened History drawer (`Load older years` across 16 years, Year 0 `Work Executed` vs `Observed Outcome` Year 1, `RECONSTRUCTED` `PJ0001 -> Promoted -> PL-PJ0001`, all 6 provenance pills, exact non-causal note, zero weather, zero economy, `Escape` preserved 1 draft Fell tick) → opened Structure (`M_i = 0.3790`, `T_i = 0.2594`, Area A `0.0034`, Area B `0.7508`, table `261 <= 295px`) → saved slot `step2-e2e` durably → stopped `mixed-a` cleanly with `SIGINT`.
- `mixed-b` (fresh server process, Year 0, new `sessionId`): loaded slot `step2-e2e` via Saved games UI → restored Preview (`currentYear: 15`, `suggestedTargetYear: 19`, `anchorFellYear: 0`, `eligible: true`), History index (`totalRecordedYears: 16`, exact Year 15..0 order, exact Year 0 Work/Outcome counts), and Structure (`M_i = 0.3790`, `T_i = 0.2594`, `n = 197`, per-area values) matched pre-save exactly; visually verified in `restored-structure-1280x720.png`, `restored-history-1280x720.png`, and `restored-history-1920x1080.png`.

---

## 6. Acceptance Criteria Author Status (`P1-UI-01`..`08`, `P1-UI-REG`, `P1-UI-REVIEW`)

| Item | Author Result | Evidence |
|---|---|---|
| **P1-UI-01** | **PASS** | `ui-smoke-multiarea.sh` (`W186`–`W234`, `M14`, `S2g`–`S2i`) + `run-step2-edge.sh` (`ma1-full`): Year-0 Obs. `+4`, post-Fell anchor `Y+4`, overdue fallback, Pending & Approved `WorkPlanOpen` 409 + inline refusal without cancelling open work, stale fence 409 + fresh view, `GAMEPLAY CALIBRATION` & quiet observation labels visible. |
| **P1-UI-02** | **PASS** | `ui-smoke-multiarea.sh` (`W235`–`W249`, `S2b`–`S2d`) + `run-step2-edge.sh` (`mixed-play-save`): 16-year newest-first keyset paging (`Load older years`), separate `Work Executed — Year Y` vs `Observed Outcome — Year Y → Year Y+1`, `RECONSTRUCTED` planted follow-up, all 6 provenance pills (`RECORDED`, `CURRENT`, `DERIVED`, `RECONSTRUCTED`, `SESSION-ONLY`, `NOT RECORDED`), explicit `NOT RECORDED` disclosures, exact non-causal note (zero weather), zero euro/economy rows, 409 on stale/invalid/unrecorded year. |
| **P1-UI-03** | **PASS** | `ui-smoke-multiarea.sh` (`W250`, `M14`, `S2e`–`S2f`) + `run-step2-edge.sh` (`ma1-full`, `mixed-play-save`): `MA-1` (`M_i = 0.00`, `T_i = 0.16`) & `MA-1-MIXED` (`M_i = 0.3790`, `T_i = 0.2594`), `DERIVED` provenance, exact spatial-structure-only disclaimer, table fits cleanly (`261 <= 295px`) at `1280×720`, and `null` renders `Unavailable` (never `0.00`, verified in `S2f`). |
| **P1-UI-04** | **PASS** | Measured raw UTF-8 `GET api/multi/view` `<= 524,288` B and `ecology <= 153,600` B at Years 0, 5, 15, 20, and 26 across `MA-1` & `MA-1-MIXED` with executed work; active `historyTruncated: true` verified at `MA-1` Year 26 (`522,939 B`) with full `totalRecordedYears = 27` & Year 4 detail intact via `api/multi/history/*`. |
| **P1-UI-05** | **PASS** | `run-step2-edge.sh` (`mixed-play-save` → SIGINT quit → `mixed-restart-load`): restored Preview, History pages/detail, and Structure match pre-save exactly across genuine server restart. |
| **P1-UI-06** | **PASS** | `run-step2-edge.sh` (`ma1-full`): NB-2 proven in real Edge at BOTH `1280×720` and `1920×1080` (`scrollTop = 0`, `Phase2DuplicateOrder`, `"Already queued"`, and both working `Remove` buttons strictly inside viewport/pane). |
| **P1-UI-07** | **PASS** | `run-step2-edge.sh`: full 4-cell matrix (`1920×1080` & `1280×720` × `MA-1` & `MA-1-MIXED`) zero h-scroll, single-row topbar, R3 5-competitor fold preserved, `Escape` closes History preserving draft Fell ticks, zero console/CSP/exception/5xx errors. |
| **P1-UI-08** | **PASS** | `git diff --name-only 5fe2fabe..HEAD`: zero edits under `src/Core/**`, `src/Ui/wwwroot/**`, `Assets/**`, or `main`; zero `data:image/webp`; zero capability tokens in Git. |
| **P1-UI-REG** | **PASS** | All builds `0/0`, Core `277/277`, Save `50/50`, `ui-smoke-multiarea.sh` `352/352`, `run-edge-e2e.sh` `63/63`, `run-step2-edge.sh` `59/59`, all 9 anchors & `.p0ref` exact, zero lingering ephemeral servers/Edge processes, `:33767`/`:43649`/`:46593` untouched. |
| **P1-UI-REVIEW** | **NOT TESTED** | Independent reviewer pending. Every row in `specs/multi-2d-phase1-time-history-ui-v1/checklists/requirements.md` remains `[ ]`. |

---

## 7. Nine Frozen Anchors, `.p0ref` Pin & Protected Listeners

All 9 frozen anchors were re-observed exact in `core-tests.log` and `sha256sum`:

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

- **Protected listeners (verified before and after, untouched):**
  - `127.0.0.1:33767` (`LISTEN`, PID `1750123`)
  - `127.0.0.1:43649` (`LISTEN`, PID `1806361`)
  - `127.0.0.1:46593` (`LISTEN`, PID `839989`)
- **Ephemeral server & headless Edge cleanup:** Verified zero leftover `sitkap1-ui.dll` processes from this lane and zero lingering `ccf-step2-edge`/`ccf-edge-` `msedge.exe` processes.

---

## 8. Deviations & Residual Risks

### Deviations (disclosed transparently)
1. **Follow-up polish commit (`a241c9a0`) instead of amending Commit 3:** Testing and real-Edge visual inspection after Commit 3 caught three small UI polish issues: (a) `box.replaceChildren(..., null)` in `renderSkipSummary()` emitted literal `"null"` when `stopDetail` was `null`; (b) `#history-load-older` scrolled off-screen inside `#history-index-pane` at `1280×720` when 10 items were listed (pinned below a scrolling `.history-year-list`); and (c) the 5th column (`T_i`) of the Structure table clipped slightly at `1280×720` (tightened headers/padding so `261 <= 295px` fits with zero clipping). Per the strict no-amend/no-rewrite rule, those 12 lines were committed as linear follow-up commit `a241c9a0` prior to `test(ui)` (`6d4c406f`), and all 7 build/test/smoke/Edge gates were re-run after it.
2. **Projection-only Stage 2 compaction in `SerializeBoundedMultiView`:** On `MA-1-MIXED` at Year 20+, dense broadleaf recruitment expands `stage4` (`trees` + `ground`) beyond `~330 KB` even before history lists. When Stage 1 tail-trimming of `events`/`annualHistory` alone cannot reach `<= 524,288` B, `SerializeBoundedMultiView` switches `stage4` to a compact summary projection (omitting redundant per-cell `contributingTreeIds` — still returned in full by `POST api/multi/cell` — rounding ground cell `{u, v}` vertices to 3 decimals, and omitting null prune-refusal fields) while leaving the authoritative Core root and Year 0–15 responses untouched.
3. **`null` Structure metric rendering tested via DOM stand-in fault (`S2f`) in addition to Core unit tests (`P1-TH-06b`):** Because the live `/multi` API refuses felling an area to zero living stems (`WorkWouldLeaveNoLivingStems`), a live MA-1/MA-1-MIXED session never produces `< 2` living trees in 15–20 years. The served `multi.js` `formatStructureCell(null) -> "Unavailable"` path was therefore verified directly in `ui-smoke-multiarea.sh` (`S2f`) via a one-shot `'null-structure'` response transform in the DOM stand-in (mirroring the existing F-2 `'unpaired'` witness pattern) with zero test hooks added to production code.

### Residual Risks
1. **Multi-decade `ecology` slice growth past Year 30+:** At `MA-1-MIXED` Year 20, `ecology` is `80,223 B` (52% of the `153,600 B` cap). Very long multi-decade runs (40–50+ years) with extreme planted/promoted juvenile counts could eventually approach the `150 KiB` `ecology` slice cap if Phase 2 records grow unboundedly; current Phase 1 Step 2 runs through Year 20–26 remain comfortably under both caps.
2. **Pending independent review:** All acceptance items remain unchecked `[ ]` until independent review (`P1-UI-REVIEW`) inspects `feat/multi-2d-phase1-time-history-ui-20261125-v1`.

---

## 9. Evidence Archive Inventory (Outside Repository)

Path: `/home/jer/Development/CCF-Multi-Phase1-Step2-Evidence-20261125` (mode `700`):
- `core-tests.log` (Core `277/277`)
- `save-tests.log` (Save `50/50`)
- `smoke/ui-smoke-multiarea.txt` (`352/352`, capability-redacted)
- `edge-save/run.log` + 12 PNG screenshots (`63/63`)
- `edge-step2/run.log` + `step2-pre-save.json` + 3 capability-redacted server logs + 15 PNG screenshots (`59/59`)
