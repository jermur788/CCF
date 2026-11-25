# CCF Multi 2D — Phase 1 Step 2: Time Pacing, Decision→Outcome History & Woodland Structure UI/HTTP Integration — Specification

- **Spec ID:** `SPEC-multi-2d-phase1-time-history-ui-V1`
- **Feature ID:** `MULTI-2D-PHASE1-TIME-HISTORY-UI-V1`
- **Status:** `IMPLEMENTING`
- **Owner:** Project owner (product decisions); Primary Implementer (Gemini 4 Neon) under single-writer lane custody
- **Created:** 2026-11-25
- **Last updated:** 2026-11-25
- **Base commit:** `5fe2fabe9073d7b72711f4ab48553c785351d7bc` (`feat/multi-2d-phase1-time-history-core-20261124-v1`, Step 1 Core, independently APPROVED at `6c83d16e`)
- **Task branch:** `feat/multi-2d-phase1-time-history-ui-20261125-v1`
- **Prerequisite spec:** `specs/multi-2d-phase1-time-history-core-v1/spec.md` (`SPEC-multi-2d-phase1-time-history-core-V1`)
- **Decision log references:** D-004 (core loop), D-008 (discrete annual steps), D-020 (presentation authority — browser computes zero forestry/pacing/structure math), D-030 (state honesty), D-063 (selective SDD), D-073 (decision → outcome history priority & 6 provenance tiers), D-074 (multi-year CCF intervention spacing framing), D-075 (dual time-advance workflow), D-077 (4-year suggested gap / 5-tick cap GAMEPLAY CALIBRATION, no silent cancellation of open work, year-by-year authoritative stepping)
- **Milestone:** MultiRack Sitka 2D standalone simulator (`Tools/MultiRackSitkaP1Lab`) — Phase 1 Step 2 (HTTP API, bounded multi-decade view payload, `/multi` UI controls, and NB-2 Plant-drawer polish)

---

## 1. Authority

This specification is task-level implementation authority for Phase 1 Step 2 only. It remains subordinate to:

1. Latest direct user / product-owner instruction;
2. `Docs/Project/decision-log.md` (notably D-020, D-030, D-073, D-074, D-075, D-077);
3. `Docs/Project/current-milestone.md`;
4. `Docs/Project/game-brief.md`.

Implementation truth remains the inspected repository state (`Tools/MultiRackSitkaP1Lab/src/Ui/`). A specification states intended behaviour and acceptance criteria; it never proves runtime verification or acceptance.

---

## 2. Product intent and architecture

Phase 1 Step 1 (`5fe2fabe…`) delivered three pure, authoritative Core modules in `SitkaP1.Core`:
- `MultiAreaTimePacing.cs` (`PreviewNextReviewYear`, `AdvanceYears`)
- `MultiAreaHistoryQuery.cs` (`ReadHistoryIndex`, `ReadHistoryYear`)
- `WoodlandStructureMetrics.cs` (`ComputeStructure`)

Phase 1 Step 2 exposes those three Core modules on the two-area `/multi` loopback HTTP server (`Tools/MultiRackSitkaP1Lab/src/Ui/PlayerMultiApi.cs`) and embeds their interactive controls inside the `/multi` page (`MultiAreaPage`, embedded HTML/CSS/JS inside `PlayerMultiApi.cs`), while keeping `GET api/multi/view` strictly bounded over multi-decade runs and fixing NB-2 (partial refusal visibility in the Plant drawer).

**Critical architectural boundary:**
- The `/multi` page is served from `MultiAreaPage` inside `Tools/MultiRackSitkaP1Lab/src/Ui/PlayerMultiApi.cs`.
- `Tools/MultiRackSitkaP1Lab/src/Ui/wwwroot/{index.html,app.css,app.js}` belong to the single-area page and MUST remain untouched.
- Zero Core biology, Step 1 math, Save SchemaVersion 2 (`src/Core/Phase1/**`), or anchor edits are permitted; zero Core edits are preferred (`MultiAreaContinuation` already exposes `internal` methods accessible to `SitkaP1.Ui`).

---

## 3. Out of scope and strict prohibitions

Phase 1 Step 2 MUST NOT modify or introduce any of the following:

- **Single-area UI (`src/Ui/wwwroot/**`):** zero edits.
- **Core biology, Step 1 math, Save SchemaVersion 2 (`src/Core/Phase1/**`), fixtures, `.p0ref`, and frozen anchors:** zero edits to `src/Core/Phase1/**`, `AnnualRules`, `CrownModel`, `Stage4*`, `Stage5*`, `Phase2*`, `MultiAreaGenerator`, `MultiAreaTimePacing`, `MultiAreaHistoryQuery`, `WoodlandStructureMetrics`, or `.p0ref/SitkaLab.Core.dll` (`9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3`).
- **Unity project, packages, dependencies, and governance:** zero edits to `Assets/**`, `Packages/**`, `ProjectSettings/**`, `Docs/Project/coordination/**`, `.vscode/**`, or `main`. Zero new NuGet/npm packages, zero external CDNs, zero inline `<script>` or `style=` attributes, zero `data:image/webp` URIs.
- **Unmodelled 2D concepts:** economy, money, prices, revenue, cash, budget, `€`/`€0` placeholders, weather, storms, teaching-gate states, unread-review locks, scenario year caps, and objective/outcome triggers MUST NOT be added anywhere in API payloads or UI copy.
- **Security regressions:** no relaxation of `127.0.0.1` loopback binding, 256-bit capability URL path, exact `Host` / `Origin` checks, `16 KiB` (`16384` byte) POST body cap, strict camelCase JSON disallowing unknown/duplicate keys, or strict CSP (`default-src 'none'; script-src 'self'; style-src 'self'; img-src data:; connect-src 'self'; base-uri 'none'; form-action 'none'; frame-ancestors 'none'`).
- **Secrets in Git:** capability URLs or tokens MUST NEVER be committed to Git.

---

## 4. Five HTTP Endpoint Contracts

All five endpoints are registered in `PlayerApi.AddMultiAreaRoutes()` under `/s/<capability>/` on the single loopback listener and pass through the unchanged `PlayerApi.HandleAsync` guard chain:
- `Host` check (`403` on mismatch)
- Capability check (`404` on mismatch)
- Route & HTTP method check (`404` / `405`)
- `Origin` check (`403` on foreign origin or POST without exact origin)
- For POST routes: `Content-Type: application/json` (`415`), `<= 16384` byte body (`413`), strict UTF-8 JSON with unique members, required members, bounded text, and `UnmappedMemberHandling.Disallow` (`400` on malformed JSON, unknown keys, duplicate keys, missing/null required keys, or wrong value types).
- Unhandled server fault: `500` `OutcomeUnconfirmed` (page locks fail-closed until a re-read succeeds).

### 4.1 `POST api/multi/year/preview-review`

- **Purpose:** Deterministic, read-only preview of the next suggested management review year. Zero state mutation.
- **Request body:** `{ "fence": <MultiAreaFence> }`, required members `MultiFenceMembers`.
- **Execution:** Calls `multiSession.PreviewNextReviewYear(request.Fence!)`.
- **200 OK response (camelCase):**
  - `currentYear` (int)
  - `eligible` (bool)
  - `suggestedTargetYear` (int)
  - `suggestedGapYears` (int, `4`)
  - `anchorFellYear` (int | null)
  - `isObservationPeriod` (bool)
  - `isOverdue` (bool — server-computed from Core's overdue label; zero pacing arithmetic in the browser)
  - `maxTicksPerPress` (int, `5`)
  - `stopReason` (string | null — `"WorkPlanOpen"` | `"StandCannotAdvance"` | `null`)
  - `calibrationNote` (string — exact `MultiAreaTimePacingConstants.CalibrationNote`)
  - `label` (string — exact Core period/refusal label)
  - `fence` (`MultiAreaFence` — fresh current fence)
  - `view` (bounded `MultiAreaSessionView`)
  - `refusal` (`null`)
- **409 Conflict response:** Stale or mismatched fence (`PlayerSessionMismatch`, `PlayerSourceStale`, or `PlayerPlanStateStale`), returning `refusal`, fresh `fence`, and fresh bounded `view`. Zero state mutation.
- **400 / 403 / 404 / 405 / 413 / 415:** Standard `HandleAsync` / `ReadMultiRequestAsync` guard refusals.

### 4.2 `POST api/multi/year/skip`

- **Purpose:** Bounded multi-year advance (1–5 years per press) calling the authoritative year-by-year engine loop. NEVER passes `consentToCancelWork = true` and NEVER cancels open work.
- **Request body:** `{ "fence": <MultiAreaFence>, "targetYear": <int> }`, required members `MultiFenceMembers` + `"targetYear"`.
- **Execution:** Calls `multiSession.AdvanceYears(request.Fence!, request.TargetYear!.Value, context.RequestAborted)`.
- **200 OK response (when `reachedTarget == true` or `completedYears.Count > 0`):**
  - `startYear` (int)
  - `destinationYear` (int)
  - `requestedTargetYear` (int)
  - `completedYears` (array of committed annual step summaries: `{ yearBefore, yearAfter, yearRecord }`)
  - `reachedTarget` (bool)
  - `stopReason` (string — one of the exact 6 `TimePacingStopReason` values)
  - `stopDetail` (string | null)
  - `isObservationPeriod` (bool)
  - `anchorFellYear` (int | null)
  - `calibrationNote` (string)
  - `view` (fresh bounded `MultiAreaSessionView`)
  - `scenes` (fresh per-area `PlayerMapItem[]`)
  - `stage4` (fresh `MultiAreaUiReadout`)
  - `ecology` (fresh `MultiAreaEcologyDto`)
  - `historyTruncated` (bool)
  - `totalRecordedYears` (int)
  - `refusal` (`null`)
- **409 Conflict response:**
  - Stale/mismatched fence (`PlayerSessionMismatch`, `PlayerSourceStale`, `PlayerPlanStateStale`), OR
  - `WorkPlanOpen` (any Pending or Approved work of any of the 6 action types; zero mutation; open work untouched), OR
  - `InvalidTargetYear` (`targetYear <= currentYear` or `targetYear - currentYear > 5`; zero mutation), OR
  - Zero-step `StandCannotAdvance` / `AnnualStepRefused`.
  - Always returns `refusal: { code, message }` plus fresh `view` (bounded), `scenes`, `stage4`, `ecology`, `historyTruncated`, `totalRecordedYears` (and skip fields when returned by `AdvanceYears`).
- **400 / 403 / 404 / 405 / 413 / 415 / 500:** Standard guard refusals; `500 OutcomeUnconfirmed` on unconfirmed failure.

### 4.3 `GET api/multi/history/index?pageSize=&beforeYear=`

- **Purpose:** Read-only, newest-first, cursor-stable paged index of recorded ecological years. Zero state mutation.
- **Query parameters:**
  - `pageSize` (optional): must parse as a base-10 integer (no whitespace, decimals, or empty value); clamped to `[1, 50]`; defaults to `10` when omitted.
  - `beforeYear` (optional): must parse as a non-negative base-10 integer (`>= 0`); omitted returns from newest recorded year.
  - Any unknown query parameter, duplicate parameter key, or malformed integer value returns **400 BadRequest**.
- **Execution:** Calls `multiSession.ReadHistoryIndex(pageSize, beforeYear)`.
- **200 OK response:**
  - `items`: newest-first array of `HistoryIndexEntry` (`year`, `hasExecutedWork`, `executedActionCounts`, `hasAnnualOutcome`, `liveStemsEndOfYear`, `hasObservedOutcomeNextYear`)
  - `pageSize` (int, clamped `1..50`)
  - `beforeYear` (int | null)
  - `nextCursorBeforeYear` (int | null)
  - `totalRecordedYears` (int)

### 4.4 `POST api/multi/history/year`

- **Purpose:** Read-only Decision → Outcome detail for `WorkYear = year` and `ObservedYear = year + 1`. Zero state mutation.
- **Request body:** `{ "fence": <MultiAreaFence>, "year": <int> }`, required members `MultiFenceMembers` + `"year"`.
- **Fence & year validation (read-only):**
  - Reads the current session view read-only and validates `fence.SessionId == view.SessionId`, `fence.Source == view.Source`, and `fence.PlanStateRevision == view.PlanStateRevision` BEFORE calling `ReadHistoryYear(year)`.
  - Mismatch returns **409 Conflict** (`PlayerSessionMismatch` / `PlayerSourceStale` / `PlayerPlanStateStale`) with fresh bounded `view`, fresh `fence`, and `refusal`.
  - `year < 0` returns **409 Conflict** (`HistoryYearInvalid`) with fresh bounded `view`, fresh `fence`, and `refusal`.
  - `year > currentView.Source.EcologicalYear` returns **409 Conflict** (`HistoryYearNotRecorded`) with fresh bounded `view`, fresh `fence`, and `refusal` BEFORE calling Core (preventing integer overflow on `year = int.MaxValue`).
  - Calls `multiSession.ReadHistoryYear(year)`. If `!detail.HasRecordedData` (unrecorded year `<= currentYear`), returns **409 Conflict** (`HistoryYearNotRecorded`) with fresh bounded `view`, fresh `fence`, and `refusal`.
- **200 OK response:** Full `HistoryYearDetail` (`workYear`, `observedYear`, `currentEcologicalYear`, `hasRecordedData`, `workProvenance`, `totalWorkCounts`, `workByArea`, `annualOutcomeNextYear`, `annualRecordEndingInYear`, `plantedWorkFollowUp`, `laterObservedComparison`, `notRecordedDisclosures`, `provenanceGlossary`).

### 4.5 `GET api/multi/structure`

- **Purpose:** Read-only spatial woodland structure descriptors ($M_i$, $T_i$) over the current committed living adult woodland. Zero state mutation.
- **Execution:** Calls `multiSession.ComputeStructure()` (which snapshots the root under one lock and delegates to `WoodlandStructureMetrics.Compute`).
- **200 OK response:**
  - `meanMingling` (double | null)
  - `meanDiameterDifferentiation` (double | null)
  - `focalLivingTreeCount` (int)
  - `validDiameterFocalCount` (int)
  - `effectiveNeighbourCount` (int)
  - `maxNeighbourCountK` (`3`)
  - `areas`: array of per-area summaries (`areaId`, `meanMingling`, `meanDiameterDifferentiation`, `focalLivingTreeCount`, `validDiameterFocalCount`, `effectiveNeighbourCount`, `maxNeighbourCountK`)
  - `provenance`: `"DERIVED"`
  - `provenanceLabel`: `"DERIVED"`
  - `honestyLabel`: `"Spatial structure descriptor (up to k=3 nearest living trees) — not measured biodiversity, habitat quality, or an Irish forecast."`

---

## 5. Bounded Multi-Decade View Payload (`GET api/multi/view`) — Owner G5 Horizon = Year 30

- **Hard measured budgets (binding G5 Year 30 horizon):**
  - Raw UTF-8 serialized body of `GET api/multi/view` MUST stay `<= 524288` bytes (512 KiB) at **every ecological year from Year 0 through Year 30** on BOTH `MA-1` and `MA-1-MIXED`, with executed work across action families.
  - Raw UTF-8 serialized `ecology` JSON slice inside `GET api/multi/view` MUST stay `<= 153600` bytes (150 KiB) at **every ecological year from Year 0 through Year 30** on BOTH `MA-1` and `MA-1-MIXED`, with executed work.
- **Additive metadata fields on `GET api/multi/view` (and composite command/skip/load responses):**
  - `historyTruncated` (bool): `true` whenever ANY of `view.events`, `view.annualHistory`, or `stage4.pruneLog` is trimmed relative to the full committed Core root; `false` otherwise.
  - `totalRecordedYears` (int): total count of recorded years matching `GET api/multi/history/index`.
  - `projectionCompacted` (bool): `true` when projection-only non-living / display-precision reductions are active on `view.areas[].trees`, `scenes`, or `stage4.areas[].trees` / `fallenDeadwood`; `false` on uncompacted responses.
  - `stage4Compacted` (bool): `true` ONLY when `stage4.areas[].ground` omits `contributingTreeIds` (via null omission — NEVER emitting a fake empty `[]`) and rounds vertex floats while preserving all vertex field names `{ xm, zm, u, v }`; `false` otherwise.
  - `payloadBudgetExceeded` (bool): `false` whenever `raw <= 524288` and `ecology <= 153600`; if a future multi-decade state ever exceeds either budget after all honest reductions, the server MUST NEVER return over-budget bytes silently — it MUST set `payloadBudgetExceeded: true` AND write a loud `PAYLOAD BUDGET EXCEEDED` log line to server stderr.
  - `reviewPreview` (read-only preview DTO from the same committed root, on `GET api/multi/view`): seeds the initial Skip button label without adding an extra startup HTTP request before `J01`, while mutations, Load, explicit Re-read, and Skip clicks query `POST api/multi/year/preview-review`.
- **Ordered projection-only bounding rules (Core root is NEVER mutated or truncated):**
  - **Ecology projection (always-on, lossless for UI):** `ecology.pruneStatusByTreeId` retains all living trees plus any non-living tree with `lifts > 0` or a non-null `planState`; unpruned dead trees cannot be pruned and are omitted from `pruneStatusByTreeId`.
  - **Tier 0 (Full uncompacted):** full `view`, full `scenes`, full `stage4`, projected `ecology`, `historyTruncated: false`, `projectionCompacted: false`, `stage4Compacted: false`. Used whenever `raw <= 524288` and `ecology <= 153600`.
  - **Tier 1 (History tail trim on full `stage4`):** trims `stage4.pruneLog` BEFORE dropping below 1 event / 1 annual record, and binary-searches the largest recent tail of `view.events` + `view.annualHistory` while preserving **at least the latest work event and latest annual record whenever present**. Sets `historyTruncated: true`.
  - **Tier 2 (Honest non-living / display-precision projection compaction, full `stage4.areas[].ground` preserved):** omits unpruned dead entries from `stage4.areas[].trees` (UI falls back honestly to `tree.death` + `fallenDeadwood` for `Life state`), omits null optional tree fields and redundant `death.treeId`, omits unused `scenes` fields (`heightM`/`dbhCm`/`deathCause`, default `isCrop: false`, null `deathYear`) while preserving every living tree, stump, and dead-tree position/status, and rounds display floats (`u`, `v`, `radiusM`, dead-tree/deadwood floats) to 3 decimals. Trims `stage4.pruneLog` before dropping below 1 event / 1 annual record and retains at least the latest work event and latest annual record whenever present. Sets `projectionCompacted: true` and exact `historyTruncated`.
  - **Tier 3 (Honest `stage4.areas[].ground` compaction — only if Tier 2 with `>= 1` event/annual record exceeds 512 KiB):** omits `contributingTreeIds` (property omitted via `JsonIgnore(WhenWritingNull)` — NEVER fabricated as `[]`; `POST api/multi/cell` always serves full `contributingTreeIds`) and rounds ground vertices while keeping ALL four field names `{ xm, zm, u, v }`. Sets `projectionCompacted: true`, `stage4Compacted: true`, and exact `historyTruncated`.
  - **Tier 4 (Explicit over-budget disclosure):** if even Tier 3 still breaches `524288` B raw or `153600` B `ecology`, sets `payloadBudgetExceeded: true` and logs loudly to server stderr; never fails silently.
  - **Results-pane honesty (D-030):** if `view.annualHistory` is ever empty while `totalRecordedYears > 0` or `historyTruncated` (or `ecologicalYear > 0`), the Results pane MUST NOT say `"No ecological year has been advanced in this session."`; it MUST point the player to the History drawer with the recorded year count.
  - Complete historical records always remain accessible via `GET api/multi/history/index` and `POST api/multi/history/year`. All existing field names and map/table/inspector UI behaviours remain preserved.

---

## 6. `/multi` UI Specification (`MultiAreaPage`)

### 6.1 Skip to Review (beside Advance)

- Placed in `.topbar .top-actions` beside `#advance` without wrapping `.topbar` onto a second row at `1280×720` or `1920×1080`.
- Displays the engine-computed target year from preview: e.g., `Skip → Yr 4 (Obs.)` before first Fell and `Skip → Yr 8` after a Year-4 Fell.
- Visibly displays `GAMEPLAY CALIBRATION` and (pre-first-Fell) quiet observation period labels.
- Refreshed via `POST api/multi/year/preview-review` after every mutation, Load, and explicit Re-read.
- On click:
  1. First invokes `POST api/multi/year/preview-review` with the current fence.
  2. Executes `POST api/multi/year/skip` ONLY when the preview confirms `eligible === true` and no open work exists.
  3. When `WorkPlanOpen`, does NOT call `skip` and NEVER cancels open work; displays a clear inline refusal explaining that open Pending/Approved work must be executed, cancelled, or removed first.
  4. On committed skip, updates the page from the fresh composite payload, refreshes preview-review, refreshes open History/Structure views, and displays a concise stepped-years and stop-reason summary (including `GAMEPLAY CALIBRATION` and quiet observation framing when applicable).

### 6.2 On-Demand History Drawer

- Opened via a `History` button (`#history-open`) in `.topbar .top-actions`.
- Implemented as an on-demand slide-over drawer (`#history-drawer`, not a permanent fourth column) so the 3-region layout (`map | legend | side`) and 1280×720 R3 fold are completely unaffected.
- On open: issues `GET api/multi/history/index?pageSize=10` and renders the newest-first recorded year list.
- Supports `Load older years` keyset pagination (`beforeYear = nextCursorBeforeYear`).
- Clicking a recorded year issues `POST api/multi/history/year` with `{ fence, year }` and renders:
  1. **Work Executed (Year Y)** (`RECORDED`) clearly separated from **Observed Outcome (Year Y+1)** (`RECORDED`), preserving real 2D timing without relabelling Year `Y` work as Year `Y+1`.
  2. **Planted Juvenile Follow-Up** (`RECONSTRUCTED`) when planting occurred in Year `Y`.
  3. **Later Observed Comparison** (`CURRENT` state + `DERIVED` deltas) with the exact non-causal note (`"Observed changes reflect ongoing natural growth, competition, seeding, and light alongside recorded management — not a single-cause effect."`) and zero weather mention.
  4. **Provenance pills** visibly displaying all six provenance labels (`RECORDED`, `CURRENT`, `DERIVED`, `RECONSTRUCTED`, `SESSION-ONLY`, `NOT RECORDED`) with D-073 tier explanations and non-colour-only badges.
  5. **Explicit `NOT RECORDED` disclosures** for past overhead canopy cover, past tree-by-tree structure, and per-area planted splits (alongside honest whole-woodland `Phase2PlantedAccount` rows when present).
  6. **Zero euro, currency, price, revenue, or economy rows.**
- **Escape handling:** Pressing `Escape` while the History drawer is open closes the drawer and returns focus to `#history-open` WITHOUT clearing draft Fell ticks, tree selection, map mode, or overlay.
- **Load / state reset:** Loading a saved game clears stale History drawer index/detail, Structure readout, and Skip preview/summary, refreshes preview-review, and refreshes History/Structure if currently open.

### 6.3 On-Demand Structure Section (inside existing Details panel)

- Placed inside the existing `#pane-details` side-panel tab (with a jump link inside the collapsible `#notice-details` Details summary) — zero permanent fourth column.
- Loads `GET api/multi/structure` on demand when the Details tab / Structure section is opened or refreshed (zero startup overhead).
- Renders per-area (`MA-A`, `MA-B`) and whole-woodland Species Mingling ($M_i$) and Diameter Differentiation ($T_i$), living adult count, effective $k$ / max $k = 3$, provenance pill `DERIVED`, and the exact spatial-structure-only disclaimer:
  `"Spatial structure descriptor (up to k=3 nearest living trees) — not measured biodiversity, habitat quality, or an Irish forecast."`
- Renders `Unavailable` whenever `meanMingling` or `meanDiameterDifferentiation` is `null` (NEVER `0.00` for `null`).

### 6.4 NB-2 Plant-Drawer Partial-Refusal Polish

- In `#plant-panel` and `renderPlantDrawer()`, `#plant-result` is placed prominently at the top of the Plant drawer (immediately below the drawer heading) and scrolled cleanly to the top (`scrollTop = 0`) when a partial refusal occurs on sequential order 2 (`Shelter`) or order 3 (`SpotClear`).
- After a partial refusal, the exact Core refusal code (e.g. `Phase2DuplicateOrder`) AND the **"Already queued"** list with working **Remove** buttons are immediately visible inside the viewport without scrolling at BOTH `1280×720` and `1920×1080`.

---

## 7. Acceptance Criteria (`P1-UI-01`..`P1-UI-08`, `P1-UI-REG`, `P1-UI-REVIEW`)

- **P1-UI-01:** `preview-review` and `skip` endpoints + UI for Year-0 observation (`+4`, `Obs.`) and post-Fell anchor (`Y + 4` + overdue fallback); `WorkPlanOpen` returns 409 and inline UI refusal without cancelling open work; stale fence returns 409 + fresh view; `GAMEPLAY CALIBRATION` and quiet observation period labels visible.
- **P1-UI-02:** 15+ year keyset paging with `Load older years`; `WorkYear = Y` vs `ObservedYear = Y + 1` clearly separated; all six provenance labels (`RECORDED`, `CURRENT`, `DERIVED`, `RECONSTRUCTED`, `SESSION-ONLY`, `NOT RECORDED`); explicit `NOT RECORDED` disclosures (past canopy cover, past tree-by-tree structure, per-area planted splits); exact non-causal note with zero weather mention; zero euro/economy rows; stale fence and unrecorded/out-of-range year return 409.
- **P1-UI-03:** `GET api/multi/structure` endpoint and Details/Structure UI on `MA-1` ($M_i = 0.00$) and `MA-1-MIXED` ($M_i > 0$); `null` renders `Unavailable` (never `0.00`); provenance `DERIVED` and exact spatial-structure-only disclaimer displayed.
- **P1-UI-04:** Measured raw UTF-8 `GET api/multi/view` `<= 524288` bytes and `ecology <= 153600` bytes at **every year 0..30** (binding G5 Year 30 horizon) on both `MA-1` and `MA-1-MIXED` with executed work; at least the latest work event and latest annual record retained whenever present; honest flagged projection (`projectionCompacted`, optional `stage4Compacted` omitting `contributingTreeIds` without fake `[]` and keeping `{xm,zm,u,v}`), `historyTruncated` including `pruneLog` trimming, explicit `payloadBudgetExceeded` + server stderr log (never silent over-budget), and truthful Results-pane zero-history text pointing to History drawer.
- **P1-UI-05:** Real headless Microsoft Edge End-to-End: Plant/Fell → Approve → Execute → Skip to Review → Open History & Structure → Save → quit server (SIGINT) → restart fresh server → Load → restored Preview, History pages, and Structure match.
- **P1-UI-06:** NB-2 Plant-drawer polish proved in real headless Microsoft Edge at `1280×720` and `1920×1080`: exact refusal code AND `Already queued` list + working `Remove` buttons immediately visible in viewport without scrolling (`scrollTop === 0`).
- **P1-UI-07:** Real headless Microsoft Edge matrix (`1920×1080` and `1280×720` on both `MA-1` and `MA-1-MIXED`): zero horizontal scroll, 5-competitor R3 above-the-fold gate preserved, Escape closes History without clearing draft Fell ticks, zero console errors, zero CSP violations, zero 5xx.
- **P1-UI-08:** Scope & hygiene compliance: zero edits under `src/Core/Phase1/`, `Assets/`, `src/Ui/wwwroot/`, or `main`; zero `data:image/webp` in source; zero capability URLs/tokens in Git.
- **P1-UI-REG:** Clean builds (`--disable-build-servers`, external `TMPDIR`, `~/.dotnet` SDK 8.0.134, 0 warnings / 0 errors), Core `277/277`, Save `50/50`, `ui-smoke-multiarea.sh`, `phase1-save-ui/run-edge-e2e.sh`, and new Step 2 Edge suite PASS, all 9 frozen anchors exact, `.p0ref` SHA-256 exact, all ephemeral servers and Edge processes stopped, protected listeners `:33767`, `:43649`, `:46593` alive before and after.
- **P1-UI-REVIEW:** Independent reviewer verification and checklist sign-off (every row remains unchecked `[ ]` until independent review returns a verdict).

---

## 8. Stop Conditions

Stop immediately and report (never work around) if:
1. Base HEAD (`5fe2fabe…`), branch, clean status, or `.p0ref/SitkaLab.Core.dll` hash (`9a99f781…34b3`) does not match.
2. Implementing Step 2 requires editing `src/Core/Phase1/`, Core biology, Save SchemaVersion 2, `Assets/`, frozen anchors, or `main`.
3. Windows Edge or Node is unavailable or real-browser gates cannot run.
4. Any protected test or anchor fails for a reason unfixable within the write allowlist.
5. Branch push credentials fail.
