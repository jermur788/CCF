# CCF Multi 2D — Phase 1 Step 2: Time Pacing, Decision→Outcome History & Woodland Structure UI/HTTP Integration — Implementation Plan

- **Spec ID:** `SPEC-multi-2d-phase1-time-history-ui-V1`
- **Spec status:** `IMPLEMENTING`
- **Plan owner:** Primary Implementer (Gemini 4 Neon, sole writer on `feat/multi-2d-phase1-time-history-ui-20261125-v1`)
- **Base commit:** `5fe2fabe9073d7b72711f4ab48553c785351d7bc`
- **Created:** 2026-11-25

---

## 1. Architecture and Minimal-Touch Design

### 1.1 Zero Core Edits
`MultiAreaContinuation` already exposes `internal` methods accessible to `SitkaP1.Ui`:
- `PreviewNextReviewYear(MultiAreaFence)`
- `AdvanceYears(MultiAreaFence, int, CancellationToken)`
- `ReadHistoryIndex(int, int?)`
- `ReadHistoryYear(int)`
- `ComputeStructure()`

All Step 2 server endpoints, payload bounding, and `/multi` page edits stay cleanly inside `Tools/MultiRackSitkaP1Lab/src/Ui/PlayerMultiApi.cs` (and optional narrow helper files under `src/Ui/` if helpful). Zero Core files, zero `src/Core/Phase1/**` files, and zero single-area `src/Ui/wwwroot/**` files are touched.

### 1.2 Five HTTP Endpoints & Bounded View Payload (`src/Ui/PlayerMultiApi.cs`)
1. **Route registration** in `AddMultiAreaRoutes()`:
   - `POST api/multi/year/preview-review` -> `PreviewMultiReviewAsync`
   - `POST api/multi/year/skip` -> `SkipMultiYearsAsync`
   - `GET api/multi/history/index` -> `ReadMultiHistoryIndexAsync`
   - `POST api/multi/history/year` -> `ReadMultiHistoryYearAsync`
   - `GET api/multi/structure` -> `ReadMultiStructureAsync`
2. **Strict request parsing:**
   - Reuse `ReadMultiRequestAsync<T>` (enforces `application/json`, `<= 16 KiB` body, unique JSON members, required member paths, bounded strings, and `UnmappedMemberHandling.Disallow`).
   - Add `MultiSkipMembers = MultiFenceMembers.Append("targetYear").ToArray()` and `MultiHistoryYearMembers = MultiFenceMembers.Append("year").ToArray()`.
   - Strict query parser for `GET api/multi/history/index`: allows only `pageSize` and `beforeYear`, rejects unknown/duplicate/non-integer/negative values with `400 BadRequest`, clamps integer `pageSize` to `[1, 50]` (default `10`).
3. **Read-only fence & bounds check for `POST api/multi/history/year`:**
   - Validates `SessionId`, `Source` record equality, and `PlanStateRevision` against `multiArea.Read()` read-only BEFORE calling `multiSession.ReadHistoryYear(year)`.
   - Returns `409` with fresh bounded view on stale fence, `year < 0` (`HistoryYearInvalid`), `year > currentYear` (`HistoryYearNotRecorded`, checked before calling Core so `year = int.MaxValue` never overflows), or `!detail.HasRecordedData` (`HistoryYearNotRecorded`).
4. **Skip handler (`POST api/multi/year/skip`):**
   - Calls `multiSession.AdvanceYears(request.Fence!, request.TargetYear!.Value, context.RequestAborted)` (never `consentToCancelWork = true`).
   - Returns `409` + fresh bounded `view`/`scenes`/`stage4`/`ecology` on stale fence, `WorkPlanOpen`, `InvalidTargetYear`, or zero-step refusal.
   - Returns `200` + skip DTO (`startYear`, `destinationYear`, `requestedTargetYear`, `completedYears`, `reachedTarget`, `stopReason`, `stopDetail`, `isObservationPeriod`, `anchorFellYear`, `calibrationNote`) + fresh bounded `view`/`scenes`/`stage4`/`ecology` + `historyTruncated`/`totalRecordedYears`/`projectionCompacted`/`stage4Compacted`/`payloadBudgetExceeded` when target reached or `>= 1` year committed.
5. **Structure handler (`GET api/multi/structure`):**
   - Calls `multiSession.ComputeStructure()` and serializes `MultiStructurePayload` with exact `provenance: "DERIVED"`, `provenanceLabel: "DERIVED"`, `maxNeighbourCountK: 3`, `honestyLabel`, whole-woodland and per-area `meanMingling`, `meanDiameterDifferentiation`, `focalLivingTreeCount`, `validDiameterFocalCount`, `effectiveNeighbourCount`.
6. **Bounded `GET api/multi/view` payload — Owner G5 Year 30 horizon (`524288` B raw / `153600` B `ecology`):**
   - `ProjectEcologyForView` keeps living trees plus pruned/planned non-living trees in `ecology.pruneStatusByTreeId` (keeping `ecology <= 153600` B across Years 0..30+).
   - `SerializeBoundedMultiView` applies ordered projection-only tiers (Tier 0 full → Tier 1 history tail trim with `stage4.pruneLog` trimmed before dropping below 1 event/annual record → Tier 2 honest non-living/display-precision projection compaction with `projectionCompacted: true` preserving all living trees, stumps, field names, and full `stage4.areas[].ground` → Tier 3 optional `stage4.areas[].ground` compaction with `stage4Compacted: true` omitting `contributingTreeIds` via null omission rather than `[]` and keeping `{xm, zm, u, v}` → Tier 4 explicit `payloadBudgetExceeded: true` + stderr log, never silent over-budget).
   - Retains at least the latest work event and latest annual record whenever present across Tiers 0–3, includes `pruneLog` trimming in `historyTruncated`, and uses the same bounded snapshot across composite endpoints so `historyTruncated` never goes stale. Authoritative Core state is never truncated.

### 1.3 `/multi` UI & NB-2 Polish (`MultiAreaPage` in `src/Ui/PlayerMultiApi.cs`)
1. **Topbar layout budget (1280×720 single-row safe):**
   - Add `#history-open` beside `#saves-open` and `#skip-review` beside `#advance`.
   - Tighten `.brand` min-width and compact-desktop topbar button padding under `@media (min-width: 1024px) and (max-width: 1599px)` so `.topbar` stays single-row at `1280×720` with zero height growth, preserving the 5-competitor R3 above-the-fold gate and zero horizontal scroll.
2. **Skip to Review flow:**
   - Displays `Skip → Yr 4 (Obs.)` pre-first-Fell and `Skip → Yr N` post-Fell, with visible `GAMEPLAY CALIBRATION` and quiet observation period subtext/tooltips.
   - Seeds from `payload.reviewPreview` on startup (keeping initial startup to 1 GET request so existing `J01` stays intact), then refreshes via `POST api/multi/year/preview-review` after every mutation, Load, and explicit Re-read.
   - On click: runs `POST api/multi/year/preview-review` first; if `WorkPlanOpen`, shows inline refusal without cancelling open work; if `eligible === true`, executes `POST api/multi/year/skip` and renders concise stepped-years/stop-reason summary.
3. **On-demand History drawer (`#history-drawer`):**
   - Fixed slide-over overlay (`position: fixed; right: 0; top: 0; bottom: 0; z-index: 40`) — zero fourth column.
   - Opens on `#history-open` click, fetches `GET api/multi/history/index?pageSize=10`, supports `Load older years`, and fetches `POST api/multi/history/year` on year click.
   - Clearly separates **Work Executed (Year Y)** vs **Observed Outcome (Year Y+1)**, renders `Later Observed Comparison` (`CURRENT` / `DERIVED`) with exact non-causal statement (no weather), renders all 6 provenance pills (`RECORDED`, `CURRENT`, `DERIVED`, `RECONSTRUCTED`, `SESSION-ONLY`, `NOT RECORDED`), renders explicit `NOT RECORDED` disclosures, and renders zero euro/economy rows.
   - `Escape` closes the drawer without clearing draft Fell ticks; Load resets and refreshes open History/Preview/Structure.
4. **On-demand Structure section (`#structure-section` inside `#pane-details`):**
   - Fetched on demand via `GET api/multi/structure` when `Details` tab is opened or `Refresh` is clicked (plus quick jump link from `#notice-details`).
   - Displays per-area and whole-woodland $M_i$ and $T_i$, renders `Unavailable` for `null` (never `0.00`), shows `DERIVED` pill and exact spatial-structure-only disclaimer.
5. **NB-2 Plant-drawer polish:**
   - Relocate `#plant-result` to the top of `#plant-panel` (directly below `#plant-heading`), tighten compact-desktop `.alert.shown` spacing slightly, scroll `#pane-tree` to `0` on partial refusal, and label the queued list `Already queued` with working one-click `Remove` buttons so exact refusal code + `Already queued` + `Remove` are immediately visible without scrolling at both `1280×720` and `1920×1080`.

---

## 2. Commit Sequence

1. **Commit 1:** `spec: add multi-2d phase1 time-history UI quartet`
   - `specs/multi-2d-phase1-time-history-ui-v1/{spec.md,plan.md,tasks.md,checklists/requirements.md}`
2. **Commit 2:** `feat(ui): add review preview, skip, history and structure endpoints with bounded view payload`
   - `Tools/MultiRackSitkaP1Lab/src/Ui/PlayerMultiApi.cs` (endpoints, strict request/query guards, response DTOs, bounded view serializer)
3. **Commit 3:** `feat(ui): add Skip, History drawer, Structure panel and NB-2 Plant-drawer polish`
   - `Tools/MultiRackSitkaP1Lab/src/Ui/PlayerMultiApi.cs` (`MultiAreaPage` HTML, CSS, JS)
4. **Commit 4:** `test(ui): add Step 2 API, payload, smoke and real-Edge gates`
   - `Tools/MultiRackSitkaP1Lab/tests/ui-smoke-multiarea.sh` (additive Step 2 endpoint/guard/payload/DOM checks; zero weakened assertions)
   - `Tools/MultiRackSitkaP1Lab/tests/phase1-time-history-ui/run-step2-edge.sh` and `drive-step2.mjs` (new real-Edge E2E, 15+ yr paging, payload budget, NB-2 visibility, and `1280×720` / `1920×1080` × `MA-1` / `MA-1-MIXED` matrix)
5. **Commit 5:** `docs(phase1-2d): record Step 2 implementation and verification handoff`
   - `Docs/Handoffs/MultiRack-Simulation-20261116/phase1-2d/PHASE1-2D-STEP2-TIME-HISTORY-UI-NEON-IMPLEMENTATION-20261125.md`

---

## 3. Verification Plan

Using `~/.dotnet` (SDK `8.0.134`), external `TMPDIR=/home/jer/ccf-p1ui-tmp`, evidence archive `/home/jer/Development/CCF-Multi-Phase1-Step2-Evidence-20261125`, Linux Node (for smoke witness), and Windows Node + Microsoft Edge (for real-browser CDP suites):

1. `dotnet build MultiRackSitkaP1Lab.sln --disable-build-servers` → 0 W / 0 E.
2. `dotnet tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests.dll` → `277/277` PASS, all 9 frozen anchors exact.
3. `dotnet run --project tests/Phase1.Save.Tests/Phase1Save.Tests.csproj` → `50/50` PASS.
4. `dotnet build src/Ui/SitkaP1.Ui.csproj --disable-build-servers` → 0 W / 0 E.
5. `TMPDIR="$TMPDIR" tests/ui-smoke-multiarea.sh /home/jer/Development/CCF-Multi-Phase1-Step2-Evidence-20261125/smoke` → PASS.
6. `tests/phase1-save-ui/run-edge-e2e.sh /home/jer/Development/CCF-Multi-Phase1-Step2-Evidence-20261125/edge-save` → PASS.
7. `tests/phase1-time-history-ui/run-step2-edge.sh /home/jer/Development/CCF-Multi-Phase1-Step2-Evidence-20261125/edge-step2` → PASS (`P1-UI-01`..`P1-UI-07` real-Edge matrix, measured payload bytes, NB-2 fold proof, Save→restart→Load continuity).
8. Scope, hygiene, `.p0ref` hash, zero leftover servers/Edge processes, and protected listeners `:33767`, `:43649`, `:46593` alive before and after.
