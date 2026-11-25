# CCF Multi 2D — Phase 1 Step 2: Time Pacing, History & Structure UI/HTTP Integration — Requirements Checklist

- **Spec ID:** `SPEC-multi-2d-phase1-time-history-ui-V1`
- **Spec status:** `IMPLEMENTING`
- **Checklist owner:** Independent Reviewer / Overall Manager (every box starts unchecked `[ ]`; only an independent reviewer or Manager ticks rows after inspecting evidence)
- **Implementation under test:** `feat/multi-2d-phase1-time-history-ui-20261125-v1` (base `5fe2fabe9073d7b72711f4ab48553c785351d7bc`)
- **Date:** 2026-11-25

> **Rule:** Every row below MUST remain unchecked `[ ]` until the independent reviewer returns a verdict. The Primary Implementer records observed evidence in `Docs/Handoffs/MultiRack-Simulation-20261116/phase1-2d/PHASE1-2D-STEP2-TIME-HISTORY-UI-NEON-IMPLEMENTATION-20261125.md` without ticking any checklist box.

## Acceptance rows

- [ ] **P1-UI-01 — Preview & Skip endpoints and UI (observation, post-Fell anchor, overdue fallback, WorkPlanOpen, stale fence, honesty labels):**
  - `POST api/multi/year/preview-review` and `POST api/multi/year/skip` enforce strict `MultiFenceMembers` / `+ targetYear`, 16 KiB body cap, Host/capability/Origin/CSP guards, and never pass `consentToCancelWork = true`.
  - UI shows target year from preview beside Advance (`Skip → Yr 4 (Obs.)` pre-first-Fell, `Skip → Yr 8` post-Fell, plus overdue fallback) and refreshes after every mutation and Load.
  - `WorkPlanOpen` returns HTTP 409 (and clear inline UI refusal) without cancelling open Pending/Approved work; stale fence returns HTTP 409 + fresh view; invalid target year returns HTTP 409.
  - `GAMEPLAY CALIBRATION` and quiet observation period labels are visibly shown, and a concise stepped-years/stop-reason summary appears after skip.
  - Evidence:

- [ ] **P1-UI-02 — History endpoints & on-demand History drawer (15+ year paging, 2D timing, 6 provenance pills, NOT RECORDED honesty, non-causal note, zero euro rows):**
  - `GET api/multi/history/index?pageSize=&beforeYear=` clamps `pageSize` `1..50` (default `10`), returns `400` on invalid query params, and paginates newest-first over 15+ years via `Load older years`.
  - `POST api/multi/history/year` validates fence read-only before calling `ReadHistoryYear(year)`, returning `409` on stale fence or unrecorded/out-of-range year with zero mutation.
  - Drawer clearly separates **Work Executed (Year Y)** vs **Observed Outcome (Year Y+1)**, shows later `CURRENT`/`DERIVED` comparison, renders all six provenance pills (`RECORDED`, `CURRENT`, `DERIVED`, `RECONSTRUCTED`, `SESSION-ONLY`, `NOT RECORDED`), displays the exact non-causal note with zero weather mention, explicitly discloses `NOT RECORDED` for past canopy cover, past tree-by-tree structure, and per-area planted splits, and contains zero euro/economy rows.
  - Evidence:

- [ ] **P1-UI-03 — Structure endpoint & on-demand Details/Structure section (`MA-1` & `MA-1-MIXED`, `Unavailable` for `null`, disclaimer):**
  - `GET api/multi/structure` returns read-only per-area and whole-woodland `meanMingling`, `meanDiameterDifferentiation`, `focalLivingTreeCount`, `maxNeighbourCountK = 3`, provenance `DERIVED`, and exact spatial-structure-only disclaimer.
  - UI loads on demand inside the existing Details panel (no permanent fourth column), shows $M_i = 0.00$ on `MA-1`, $M_i > 0$ on `MA-1-MIXED`, renders `Unavailable` when `null` (never `0.00`), and displays the exact disclaimer.
  - Evidence:

- [ ] **P1-UI-04 — Measured bounded multi-decade `GET api/multi/view` payload budgets (Owner G5 Year 30 horizon):**
  - Measured raw UTF-8 `GET api/multi/view` `<= 524288` bytes and `ecology` `<= 153600` bytes at **every year 0..30** on both `MA-1` and `MA-1-MIXED` with executed work.
  - At least the latest work event and latest annual record are retained whenever present (trimming `stage4.pruneLog` before dropping them); `historyTruncated` reflects `events`, `annualHistory`, and `pruneLog` trimming; `projectionCompacted` / optional `stage4Compacted` omit rather than fabricate empty lists and keep vertex field names `{xm, zm, u, v}`; over-budget responses are never silent (`payloadBudgetExceeded: true` + server log); Results pane points to History drawer instead of falsely claiming zero years advanced.
  - Evidence:

- [ ] **P1-UI-05 — Real Edge End-to-End Save → Quit → Restart → Load continuity:**
  - Real headless Microsoft Edge flow: Plant/Fell → Approve → Execute → Skip to Review → Open History & Structure → Save → quit server (SIGINT) → restart fresh server → Load → restored Preview, History pages, and Structure match.
  - Evidence:

- [ ] **P1-UI-06 — NB-2 Plant-drawer partial-refusal visibility polish at `1280×720` and `1920×1080`:**
  - After partial refusal on order 2 or 3 in `renderPlantDrawer`, both the exact Core refusal code AND the `Already queued` list with working `Remove` buttons are immediately visible inside the viewport without scrolling at both `1280×720` and `1920×1080`, proved in real headless Edge.
  - Evidence:

- [ ] **P1-UI-07 — Real headless Edge matrix (`1920×1080` & `1280×720` on `MA-1` & `MA-1-MIXED`):**
  - Zero horizontal scroll across all four matrix combinations.
  - 5-competitor R3 above-the-fold gate preserved at `1280×720`.
  - `Escape` closes History drawer without clearing draft Fell ticks.
  - Zero console errors, zero CSP violations, zero 5xx.
  - Evidence:

- [ ] **P1-UI-08 — Scope, asset, and security hygiene compliance:**
  - Zero edits under `src/Core/Phase1/`, `Assets/`, `src/Ui/wwwroot/`, or `main` relative to `5fe2fabe…` (and zero Core biology/Step 1 math/anchor edits).
  - Zero `data:image/webp` in source; zero capability URLs/tokens in Git.
  - Evidence:

- [ ] **P1-UI-REG — Clean builds, full regression, 9 frozen anchors & runtime hygiene:**
  - Clean builds (`--disable-build-servers`, external `TMPDIR`, `~/.dotnet` SDK 8.0.134, `0 W / 0 E`) for `MultiRackSitkaP1Lab.sln` and `SitkaP1.Ui.csproj`.
  - Core `277/277`, Save `50/50`, `ui-smoke-multiarea.sh`, `phase1-save-ui/run-edge-e2e.sh`, and Step 2 Edge suite PASS (`0` failed).
  - All 9 frozen anchors exact; `.p0ref/SitkaLab.Core.dll` SHA-256 exact (`9a99f781…34b3`); all ephemeral servers and Edge processes stopped; protected listeners `:33767`, `:43649`, `:46593` alive before and after.
  - Evidence:

- [ ] **P1-UI-REVIEW — Independent review and verdict:**
  - Independent reviewer verification complete; no self-review accepted.
  - Evidence:

## Verdict

- **Spec conformance:** NOT VERIFIED (pending independent review)
- **Implementation correctness:** NOT VERIFIED (pending independent review)
- **State:** `IMPLEMENTING` (at handoff: `IMPLEMENTED ON TASK BRANCH — NOT REVIEWED, NOT INTEGRATED, NOT ACCEPTED`)
