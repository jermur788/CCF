# CCF Multi 2D — Phase 2B-2a Economy UI & `/multi` Routes — Implementation Handoff

- **Date:** 2026-11-26
- **Task:** `CCF-2D-PHASE2B2A-20261126` (`T-2B2A-R1:20261126T1710Z`, amended by owner decisions D1b, D2a, D3, D4)
- **Branch:** `feat/multi-2d-phase2b2a-economy-ui-20261126-v1` *(LOCAL ONLY — stopped before push per Stop Condition #3; see §3)*
- **Base Commit:** `6d72c30538b1fbe30d3cbcf083b5b368733662b8` (`origin/integration/2d-sim`)
- **Implementation Commit:** `e0d5938217f0bf3a3c886594730e2a5348bba0a4` *(followed by this handoff commit on the same feature branch)*
- **Worktree:** `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-2D-Phase2B2a-20261126`
- **Evidence Root:** `/home/jer/Development/CCF-Multi-2D-Phase2B2a-Evidence-20261126/implementer-gate`
- **Authority Scope:** Build + verify only on feature branch. Zero merges to `integration/2d-sim` or `main`; zero PRs; zero force pushes; zero tags; zero edits to `/home/jer/Development/CCF-HP`.

---

## 1. File-by-File Change Summary

### Spec Quartet (`specs/multi-2d-phase2b-economy-ui-v1/`)
- `specs/multi-2d-phase2b-economy-ui-v1/spec.md` — Normative specification recording amended D1b/D2a/D3/D4 scope, acceptance IDs `2B2a-01..08` + `2B2a-REG`, zero-JS-math (D-020), contractor-only Fell lock, €2,500 floor, single History drawer, and explicit Save v3 deferral to Phase 2B-2b.
- `specs/multi-2d-phase2b-economy-ui-v1/plan.md` — File plan and verification strategy.
- `specs/multi-2d-phase2b-economy-ui-v1/tasks.md` — Task checklist (`T-00`–`T-08`).
- `specs/multi-2d-phase2b-economy-ui-v1/checklists/requirements.md` — Requirements traceability matrix (`2B2a-01..08` + `2B2a-REG`).

### Server Endpoint Layer (`Tools/MultiRackSitkaP1Lab/src/Ui/`)
- `PlayerApi.cs` — Single-line constructor change initializing `multiCurrent = new MultiSessionPair(multiArea, multiSession)` for lock-safe D2a session swap. Zero changes to single-area `/api/*`, Host/Origin/capability/CSP guards, or single-listener rules.
- `MultiRoutes.cs` — Replaced readonly `multiArea`/`multiSession` fields with lock-safe `MultiSessionPair` (`Volatile.Read` + `multiSwapSync`), added `OpenFreshMultiSession(experimentKey, economyEnabled)` using existing genesis paths (`MultiAreaGenerator.Generate()` + `EconomyOptions.Default` for `ma-1`; `MultiAreaDemoExperiments.CreateMixedDemo()` + `EconomyOptions.Default` for `ma-1-mixed`), registered the 5 economy routes (`GET api/multi/economy/view`, `POST api/multi/economy/session`, `POST api/multi/economy/quote`, `POST api/multi/economy/executor`, `POST api/multi/economy/history/year`), added bounded member lists and `kind`/`targetKey`/`executor` to `RequireBoundedText`. `MultiPageModules` (14 modules) and `SitkaP1.Ui.csproj` remained byte-unchanged.
- `MultiApi.View.cs` — Snapshots `CurrentMulti` once per request; adds `[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] MultiEconomySnapshotDto? Economy = null` to view/aux payloads (strictly `null`/omitted when Economy is Off, keeping Off payloads byte-comparable and G5 budgets intact); implements server-authoritative formatting for cash, landowner hours (`X / 40 hrs`), harvest thinning stage, €2,500 floor highlight flag/text, assortments by species/grade, non-harvest lines, upfront-cash verdict & plain-English issue explanations, settlements, and ledger entries (zero JS math).
- `MultiApi.Commands.cs` — Snapshots `CurrentMulti` once per command; attaches `Economy` (omitted when Off) to `MultiCommandPayload`; implements `ReadMultiEconomyViewAsync`, `SwitchMultiEconomySessionAsync` (enforcing `confirmReset` when dirty via 409 `EconomyResetConfirmationRequired` and atomic `multiSwapSync` swap), `QuoteMultiEconomyAsync`, and `SetMultiOrderExecutorAsync` (enforcing `EconomyFellContractorOnly` / `WorkPlanAlreadyApproved` / `EconomyModelRequired`).
- `MultiApi.History.cs` — Snapshots `CurrentMulti` once per request; extracts shared `ValidateFenceNotice`; attaches `Economy` (omitted when Off) to `MultiSkipPayload`; implements `ReadMultiEconomyHistoryYearAsync` (`POST api/multi/economy/history/year`). Leaves `POST api/multi/history/year` (`HistoryYearDetail`) unchanged when Off.
- `MultiApi.Saves.cs` — Snapshots `CurrentMulti` once per request; catches `P1Refusal` (`SaveEconomyRequiresV3`) in `ReadMultiSavesAsync` so `GET api/multi/saves` returns HTTP 200 with empty slots + typed refusal notice (zero HTTP 500s / zero stderr) while Money is On; attaches `Economy` (omitted when Off) to `MultiLoadPayload`.

### Frontend (`Tools/MultiRackSitkaP1Lab/src/Ui/multi/`, Strictly 14 Existing ES Modules)
- `multi.html` — Added `#economy-toggle` in `.top-actions`, `<section id="economy-strip" hidden>` top bar strip, `<section id="economy-quote-panel" hidden>` inside `#pane-plan`, `<div id="execute-economy-summary" hidden>` inside `#execute-dialog`, and `<dialog id="economy-toggle-dialog">` confirmation modal. Zero inline `<script>`, zero inline `style=`.
- `multi.css` — Dark woodland theme styling for `.economy-toggle`, `.economy-strip`, `.economy-quote`, `.econ-verdict-ok/blocked`, `.econ-floor-highlight`, `.econ-summary`, `.exec-row/pill/locked/group/btn`, `.economy-exec-summary`, `.history-card-finance`, with compact 1280×720 responsive rules preserving single-row `.topbar`, zero horizontal scroll, zero 4th permanent column, and R3 fold geometry.
- `store.js` — Added economy UI state (`economy`, `economyToggleTarget`, `historyEconomyDetail`, `historyEconomyError`), `isEconomyEnabled()`, and shared `resetSessionUiState()`.
- `api.js` — Reads `payload.economy || null` in `applyPayload`, adds plain-English `recoveryFor` entries for economy refusal codes, and enforces `#approve`/`#execute` blocking when `activeQuote.isEligible === false` plus executor button gating in `renderCommandGate`.
- `commands.js` — Added `openEconomyToggleDialog`, `onEconomyToggleConfirmed` (with dirty-progress warning & full UI state reset), `onSetOrderExecutor`, and frozen financial summary inside `openExecuteDialog`.
- `panels/work-plan.js` — Renders `#economy-quote-panel` from server strings only (zero JS math), locked `Contractor only` Fell pill + tooltip, per-order `Contractor`/`Landowner` controls for non-harvest orders (without `.mono` inside `#pending-areas`/`#approved-areas`), atomic financial settlement block in `renderReceipt`, and economy evidence notes in `renderDetails` when On.
- `panels/time-history.js` — When Money is On, fetches `POST api/multi/economy/history/year` in parallel and folds a `Financial Ledger — Year Y` (`RECORDED`) card into the existing History drawer (zero second History button). Completely inert when Money is Off.
- `dialogs/saves.js` — Surfaces `SaveEconomyRequiresV3` notice cleanly and disables `#save-do` while Money is On; delegates session reset to `resetSessionUiState`.
- `main.js` — Renders `#economy-toggle` and `#economy-strip` in `renderStatus` and wires toggle dialog and per-order executor controls.

### New Headless Edge Economy Suite (`Tools/MultiRackSitkaP1Lab/tests/phase2b2a-economy-ui/`)
- `run-economy-edge.sh` — Launches ephemeral WSL2 `sitkap1-ui` processes (PID-scoped cleanup only), runs `drive-economy.mjs` at `1920x1080` (`ma-1-mixed`), `1280x720` (`ma-1-mixed`), and `1280x720`/`1920x1080` (`ma-1`), writing all evidence outside the repository.
- `drive-economy.mjs` — Headless Microsoft Edge CDP driver proving `2B2a-01..08` across both viewports with layout/scroll/topbar checks, screenshots, dirty toggle confirmation, stale fence refusal, €2,500 floor highlight, contractor-only Fell lock + 409 refusal, Landowner toggle + deduction + 40 h reset on Advance 1 Year, Execute replay zero-double-charge proof, upfront-cash UI block + 409 `EconomyInsufficientCash`, single History drawer Off (`NOT RECORDED`) vs On (`Financial Ledger`), Save v3 dialog honesty, and zero console/CSP/404/5xx errors.

---

## 2. Verification Commands & Observed Summary Lines (Commit `e0d59382`)

All builds and tests ran in WSL Ubuntu with pinned `$HOME/.dotnet-8.0.133/dotnet` (`8.0.133`), Linux `node v26.13.1`, Windows `node.exe` + Microsoft Edge over CDP, and external verified seed `/home/jer/Development/CCF-Multi-2D-Phase2B2a-Evidence-20261126/p0ref-seed` (`9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3`).

| Gate | Exact Command | Observed Summary Line |
|---|---|---|
| Solution build | `$DOTNET build MultiRackSitkaP1Lab.sln -c Debug --disable-build-servers` | `Build succeeded. 0 Warning(s) 0 Error(s)` (exit 0) |
| Core tests | `$DOTNET build tests/Core.Tests/SitkaP1.Core.Tests.csproj -c Debug --disable-build-servers` && `$DOTNET tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests.dll` | `291/291 passed, 0 failed` (exit 0) |
| Save tests | `$DOTNET build tests/Phase1.Save.Tests/Phase1Save.Tests.csproj -c Debug --disable-build-servers` && `$DOTNET tests/Phase1.Save.Tests/bin/Debug/net8.0/Phase1Save.Tests.dll` | `50/50 passed, 0 failed` (exit 0) |
| UI build | `$DOTNET build src/Ui/SitkaP1.Ui.csproj -c Debug --disable-build-servers` | `Build succeeded. 0 Warning(s) 0 Error(s)` (exit 0) |
| Multiarea smoke | `DOTNET=$DOTNET tests/ui-smoke-multiarea.sh $EV/implementer-gate/ui-smoke-multiarea` | `OVERALL: PASS (363 passed, 0 failed)` (exit 0) |
| Single-area smoke | `DOTNET=$DOTNET tests/ui-smoke.sh $EV/implementer-gate/ui-smoke` | `OVERALL: FAIL (250 passed, 4 failed)` — exact known `{L01, L02, L06, J31}` only |
| Save Edge | `DOTNET=$DOTNET tests/phase1-save-ui/run-edge-e2e.sh $EV/implementer-gate/save-edge` | `OVERALL: PASS (63 passed, 0 failed)` (exit 0) |
| Step 2 Edge | `DOTNET=$DOTNET tests/phase1-time-history-ui/run-step2-edge.sh $EV/implementer-gate/step2-edge` | `OVERALL: PASS (67 passed, 0 failed)` (G5 Year 0..30 sweeps PASS: `ma-1` max raw `524279 B`, `ma-1-mixed` max raw `523375 B`; exit 0) |
| Economy Edge (new) | `DOTNET=$DOTNET tests/phase2b2a-economy-ui/run-economy-edge.sh $EV/implementer-gate/economy-edge` | `VIEWPORT 1920x1080: 24 passed, 0 failed` / `VIEWPORT 1280x720: 41 passed, 0 failed` / `OVERALL: PASS (71 passed, 0 failed)` (exit 0) |
| Canonical gate | `build/2d-gate.sh --base 6d72c30538b1fbe30d3cbcf083b5b368733662b8 --p0ref-dir $SEED --headless-only $EV/implementer-gate/2d-gate` | **`VERDICT: FAIL (exit=1)`** at G2 sub-check #8 (`S0c mode: multi.css no longer matches T0 R_css`) — see §3 |
| Forbidden paths | `git diff --name-only 6d72c30538b1fbe30d3cbcf083b5b368733662b8..HEAD` | Only §1 allowed paths (0 lines changed in `src/Core`, `tests/Core.Tests`, `SitkaP1.Ui.csproj`, `wwwroot`, `build`, `Assets`, `Packages`, `ProjectSettings`, `Docs/Project`) |
| Capability tokens | `git diff 6d72c30538b1fbe30d3cbcf083b5b368733662b8..HEAD \| grep -oE '[0-9a-f]{64}'` | `NONE` |
| JS module count | `find Tools/MultiRackSitkaP1Lab/src/Ui/multi -name '*.js' \| wc -l` | `14` (unchanged per D4) |

---

## 3. Canonical Gate (`build/2d-gate.sh`) Result & Stop Condition #3 Diagnosis

Command executed on clean committed `e0d5938217f0bf3a3c886594730e2a5348bba0a4`:
```bash
build/2d-gate.sh --base 6d72c30538b1fbe30d3cbcf083b5b368733662b8 \
  --p0ref-dir /home/jer/Development/CCF-Multi-2D-Phase2B2a-Evidence-20261126/p0ref-seed \
  --headless-only /home/jer/Development/CCF-Multi-2D-Phase2B2a-Evidence-20261126/implementer-gate/2d-gate
```

- **Exit Code:** `1` (`VERDICT: FAIL (exit=1)`)
- **Passing Sub-Checks Before Stop:**
  - `GATE G0 PASS`
  - `GATE G1 PASS (P0 REFERENCE: PRE-SEEDED VERIFIED 9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3)`
  - `NETWORK ISOLATION: PROVEN` (`unshare -rn`)
  - `G2: Core.Tests PASS (291/291 passed, 0 failed)`
  - `G2: Phase1.Save.Tests PASS (50/50 passed, 0 failed)`
  - `G2: ui-smoke-multiarea.sh PASS (363 passed, 0 failed)`
  - `G2: ui-smoke.sh PASS-BASELINE-DRIFT (250 passed, 4 known T0 #885 failures: L01,L02,L06,J31; G11b PASS)`
  - `G2: cli-safety.sh PASS (23 passed, 0 failed)`
  - `G2: build-output-safety.sh PASS (C1-C6+P0 PASS; HOST-NA: C7-cold-build,C8-rebuild; .p0ref restored & re-verified)`
  - `G2: p0-isolation.sh PASS (decoy=13/13, protected hits=0, unresolved=0)`
- **Exact Blocker (G2 Sub-Check #8):**
  ```text
  G2: running S0a /multi asset identity sub-check...
  S0c mode: multi.css no longer matches T0 R_css
  ERROR [G2]: S0a /multi asset identity failed:
  GATE G2 FAIL
  ```
- **Root Cause:** When integration duty C1 retired `build/2d-gate/s0a-move-table.json` (`"gateEnforce": false`), **`build/2d-gate/s0a-multi-asset-identity.json` remained active with `"mode": "s0c-css-plus-html-delta"`**. In `build/2d-gate.sh:1031-1060`, that S0c mode requires `Tools/MultiRackSitkaP1Lab/src/Ui/multi/multi.css` to remain byte-identical to T0 (`f3b2b68a...00a5`) and `multi.html` to differ from T0 by only the single `<script>` module tag line. Any feature change to `multi.html` or `multi.css` (explicitly required by Phase 2B-2a) trips this unretired S0c lock.
- **Stop Condition #3 Action Taken:** Because `build/2d-gate/**` is a **forbidden path** outside the Phase 2B-2a file fence, this lane made **zero edits** to `build/2d-gate/**`, weakened zero tests, left the worktree clean, and **stopped before pushing** so the integrator/owner can either retire `s0a-multi-asset-identity.json` on `integration/2d-sim` (parallel to C1) or authorize pushing the feature branch with this documented S0c lock.

| Stage | Status | Notes |
|---|---|---|
| G0 | **PASS** | All host prerequisites present (`dotnet 8.0.133`, `node v26.13.1`, `python3`, `git`, `unshare -rn`) |
| G1 | **PASS** | Verified `.p0ref` seed (`9a99f781...34b3`), clean build |
| G2 | **FAIL** | Sub-checks #1–#7 ALL PASS (`291/291`, `50/50`, `363/0`, `250/4`, `23/0`, `build-output-safety`, `p0-isolation`); blocked solely at sub-check #8 (`s0a-multi-asset-identity.json` S0c lock on `multi.css`/`multi.html`) |
| G3–G7 | **NOT RUN** | Halted by G2 exit before G3–G7 ran; G5 payload budgets verified via `run-step2-edge.sh` Year 0..30 sweeps; forbidden paths verified via `git diff --name-only` |
| B | **NOT RUN** | Skipped (`--headless-only`); browser acceptance verified directly via `run-edge-e2e.sh` (`63/63`), `run-step2-edge.sh` (`67/67`), and `run-economy-edge.sh` (`71/71`) |

---

## 4. Evidence Labels Inventory

All displayed economic evidence labels come verbatim from `SitkaP1.Core.EconomyEvidence.All`:
- **`EMPIRICAL`** — IFA 2024 Sitka roadside price ranges; published IFA combined harvesting+forwarding range; 890 kg/m³ Sitka mill-delivery anchor.
- **`MANAGEMENT GUIDANCE`** — Commercial production felling is contractor-only (`EconomyFellContractorOnly`); Teagasc hardwood grading guidance and nominal Sitka dimensions.
- **`GAMEPLAY CALIBRATION`** — €12,000 opening cash; 40.0 h/year landowner allowance; €45/hr contractor non-harvest labour; on-demand materials (€5.00 shelter, €4.50 beech, €5.50 oak); selective CCF thinning rates (€21/€23/€20/t); €2,500 minimum harvest job floor; broadleaf reference prices and 890 kg/m³ proxy convention.
- **`SIMULATION ABSTRACTION`** — Upfront cash / no borrowing / no pre-financing from expected revenue; €0 cash wage for landowner hours; atomic `Quote → Approve → Execute → Settle`; sell-and-extract timber settlement path (dispositions deferred per D1b).

---

## 5. Assumptions & Discrepancies

### Assumptions
1. **Top strip placement:** Placed `#economy-strip` immediately below `.topbar` (hidden when Money is Off, 0 px layout impact) while adding a compact `Money: Off/On` pill inside `.top-actions`. This preserves the single-row `.topbar`, zero horizontal scroll, and the Stage 4 R3 5-competitor above-the-fold geometry at `1280×720` when Money is Off while showing Cash + `X / 40 hrs` prominently at the top when Money is On.
2. **Saved games while Money is On:** `GET api/multi/saves` catches `SaveEconomyRequiresV3` cleanly and returns HTTP 200 with empty slots + typed refusal notice (rather than throwing HTTP 500 to stderr), while `POST api/multi/save` and `POST api/multi/load` still refuse with HTTP 409 `SaveEconomyRequiresV3`.
3. **Launch-time toggle confirmation (D2a):** Clicking `Money: Off/On` always opens `#economy-toggle-dialog` to confirm starting a fresh Year-0 session; when the session is dirty (Year > 0, or pending/approved/executed work, or unsent drafts), the dialog displays a prominent unsaved-progress warning listing what will be discarded, and `POST api/multi/economy/session` independently enforces `confirmReset: true` (409 `EconomyResetConfirmationRequired` otherwise).

### Discrepancies
1. **Unretired S0c asset-identity gate (`build/2d-gate/s0a-multi-asset-identity.json`):** As documented in §3, C1 retired `s0a-move-table.json` (`gateEnforce: false`) for post-S0 Core work, but `s0a-multi-asset-identity.json` still enforces `mode: "s0c-css-plus-html-delta"` (`multi.css` byte-identical to T0 and `multi.html` 1-line delta). This blocks `build/2d-gate.sh` at G2 sub-check #8 for any feature that edits `multi.html` or `multi.css`.
2. **Core Off-mode `FinancialLedger` disclosure wording:** Core's `MultiAreaHistoryQuery.cs:712` (read-only at base `6d72c305`) uses `"Economy model is Off; cash balance, contractor charges, timber receipts, and landowner hours are NOT RECORDED."` (contains the English word `cash`, with zero `€` glyph and zero numeric values). `run-step2-edge.sh` (`67/67`) passes cleanly against this exact string, and `drive-economy.mjs` verifies `FinancialLedger` + `NOT RECORDED` + zero `€` glyph + no Financial Ledger card when Off.

---

## 6. NOT TESTED

- `build/2d-gate.sh` stages **G3, G4, G5, G6, G7** inside the gate script itself (did not execute because G2 halted at S0c asset-identity sub-check #8; G5 Year 0..30 sweeps were verified directly via `run-step2-edge.sh`, and forbidden paths/tokens were verified directly via `git diff`).
- `build/2d-gate.sh --browser` tier (headless-only requested; Edge suites ran directly via `run-edge-e2e.sh`, `run-step2-edge.sh`, and `run-economy-edge.sh`).
- Push to `origin` (held per Stop Condition #3 pending resolution of `s0a-multi-asset-identity.json`).
- Save v3 economy persistence (`SaveEconomyRequiresV3` deferred to Phase 2B-2b).
- Timber felling dispositions / `KeepForUse` m³ (deferred per owner decision D1b).
- Unity Editor / 3D project (`Assets/` untouched).
