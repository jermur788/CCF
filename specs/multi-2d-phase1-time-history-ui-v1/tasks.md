# CCF Multi 2D — Phase 1 Step 2: Time Pacing, History & Structure UI/HTTP Integration — Bounded Tasks

- **Spec ID:** `SPEC-multi-2d-phase1-time-history-ui-V1`
- **Spec status:** `IMPLEMENTING`
- **Task list owner:** Primary Implementer (Gemini 4 Neon)
- **Base commit:** `5fe2fabe9073d7b72711f4ab48553c785351d7bc`
- **Last updated:** 2026-11-25

---

## T-01 — Commit 1: Spec quartet
- **Goal:** Create `spec.md`, `plan.md`, `tasks.md`, and unticked `checklists/requirements.md` under `specs/multi-2d-phase1-time-history-ui-v1/`.
- **Scope:** `specs/multi-2d-phase1-time-history-ui-v1/**`.
- **Verification:** Zero `TBD`; all checklist rows (`P1-UI-01`..`P1-UI-08`, `P1-UI-REG`, `P1-UI-REVIEW`) unchecked `[ ]`; complete endpoint contracts, bounded view payload rules, UI behaviours, NB-2 polish, honesty labels, CSP/16 KiB constraints, and out-of-scope rules recorded.

## T-02 — Commit 2: Five HTTP endpoints + bounded multi-decade view payload
- **Goal:** Implement `POST api/multi/year/preview-review`, `POST api/multi/year/skip`, `GET api/multi/history/index`, `POST api/multi/history/year`, and `GET api/multi/structure` in `Tools/MultiRackSitkaP1Lab/src/Ui/PlayerMultiApi.cs`, plus projection-only tail truncation on `GET api/multi/view` to guarantee `<= 524288` B raw and `ecology <= 153600` B.
- **Scope:** `Tools/MultiRackSitkaP1Lab/src/Ui/PlayerMultiApi.cs` (zero Core edits, zero single-area `wwwroot/**` edits).
- **Verification:** `dotnet build src/Ui/SitkaP1.Ui.csproj --disable-build-servers` (`0 W / 0 E`), strict 400/403/404/405/409/413/415/500 guards verified.

## T-03 — Commit 3: `/multi` UI (Skip to Review, History drawer, Structure panel, NB-2 Plant-drawer polish)
- **Goal:** Embed Skip to Review controls, on-demand History drawer, on-demand Structure panel inside Details, and NB-2 Plant-drawer partial-refusal layout fix in `MultiAreaPage` inside `Tools/MultiRackSitkaP1Lab/src/Ui/PlayerMultiApi.cs`.
- **Scope:** `Tools/MultiRackSitkaP1Lab/src/Ui/PlayerMultiApi.cs` (`MultiAreaPage` HTML, CSS, JS only).
- **Verification:** `SitkaP1.Ui.csproj` clean build (`0 W / 0 E`), served `multi.js` passes `node --check`, zero inline `<script>` / `style=`, zero `data:image/webp`, zero CSP violations.

## T-04 — Commit 4: Step 2 API, payload, smoke, and real-Edge verification gates
- **Goal:** Extend `tests/ui-smoke-multiarea.sh` additively with Step 2 endpoint/guard/payload/DOM checks (preserving all existing assertions) and add `tests/phase1-time-history-ui/run-step2-edge.sh` + `drive-step2.mjs` covering `P1-UI-01` through `P1-UI-07` in real headless Microsoft Edge across `1920×1080` and `1280×720` on both `MA-1` and `MA-1-MIXED`, measured multi-decade payload budgets, NB-2 fold geometry, and Save → quit → restart → Load continuity.
- **Scope:** `Tools/MultiRackSitkaP1Lab/tests/ui-smoke-multiarea.sh`, `Tools/MultiRackSitkaP1Lab/tests/phase1-time-history-ui/{run-step2-edge.sh,drive-step2.mjs}`.
- **Verification:** Full builds (`0 W / 0 E`), Core `277/277`, Save `50/50`, `ui-smoke-multiarea.sh` PASS, `phase1-save-ui/run-edge-e2e.sh` PASS, `phase1-time-history-ui/run-step2-edge.sh` PASS, all 9 frozen anchors exact, `.p0ref` hash exact, ephemeral servers/Edge cleaned up, protected listeners `:33767`, `:43649`, `:46593` alive.

## T-05 — Commit 5: Implementation handoff and branch push
- **Goal:** Write durable implementation and verification handoff at `Docs/Handoffs/MultiRack-Simulation-20261116/phase1-2d/PHASE1-2D-STEP2-TIME-HISTORY-UI-NEON-IMPLEMENTATION-20261125.md` with all measured evidence (zero secrets/capability URLs), commit cleanly, verify clean `git status --short`, and push `feat/multi-2d-phase1-time-history-ui-20261125-v1` to `origin`.
- **Scope:** `Docs/Handoffs/MultiRack-Simulation-20261116/phase1-2d/PHASE1-2D-STEP2-TIME-HISTORY-UI-NEON-IMPLEMENTATION-20261125.md`.
- **Verification:** Clean status; zero checklist boxes ticked; explicit `IMPLEMENTED ON TASK BRANCH — NOT REVIEWED, NOT INTEGRATED, NOT ACCEPTED` statement; pushed without force, merge, PR, or tags.

## T-06 — Bounded Repair Pass (Independent Review Findings F-1..F-6, G5 Year 30 Horizon & Nits N-1/N-3/N-4)
- **Goal:** Close every finding from `PHASE1-2D-STEP2-TIME-HISTORY-UI-OPUS-REVIEW-20261125.md` under the binding Owner G5 Year 30 horizon:
  - **F-1 & F-4:** Honest projection-only reductions keeping `GET api/multi/view <= 524288` B and `ecology <= 153600` B at every year 0..30 on both `MA-1` and `MA-1-MIXED` with executed work; explicit `projectionCompacted`, optional honest `stage4Compacted` (omitting `contributingTreeIds` via null omission rather than `[]`, keeping `{xm, zm, u, v}`), `pruneLog` included in `historyTruncated`, and explicit `payloadBudgetExceeded` + server stderr log (never silent over-budget).
  - **F-2:** Retain at least the latest work event and latest annual record whenever present (trimming `stage4.pruneLog` before dropping them); truthful Results-pane copy pointing to History drawer when `annualHistory` is empty while `totalRecordedYears > 0` or `historyTruncated`.
  - **F-3:** Reject `year < 0` (`409 HistoryYearInvalid`) and `year > currentYear` (`409 HistoryYearNotRecorded`) in `ReadMultiHistoryYearAsync` before calling Core so `year = int.MaxValue` returns `409` with zero stderr.
  - **F-5:** Add per-year Year 0..30 payload sweep on both experiments, `>= 1` event/annual retention check, Results-pane zero-history check, `history/year` `PlayerSourceStale` / `PlayerPlanStateStale` / `int.MaxValue` checks, Load-over-live-session (History + Structure open) real-Edge check, and NB-2 order 2 (`Shelter`) partial refusal at `1280×720` and `1920×1080`.
  - **F-6:** Guard `refreshStructure` against stale tree tags (`structureRequest++` in `applyPayload`, request-time tag capture/check) and guard `loadHistoryYear` against stale detail answers (`historyDetailRequest++` on `loadHistoryIndex` reset).
  - **Nits N-1 / N-3 / N-4:** Server-computed `isOverdue` (zero browser pacing math), consistent bounded composite payloads carrying `historyTruncated`, and server-driven provenance pills in History cards.
- **Scope:** Existing Step 2 write allowlist only (`specs/multi-2d-phase1-time-history-ui-v1/**`, `Tools/MultiRackSitkaP1Lab/src/Ui/**`, `Tools/MultiRackSitkaP1Lab/tests/**`, Step 2 handoff; zero Core edits).
- **Verification:** Re-run all 7 build/test/smoke/Edge gates + Year 0..30 per-year sweeps on both experiments; commit cleanly (no amend, no force) and push feature branch.
