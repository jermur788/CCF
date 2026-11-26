# CCF Multi 2D — Phase 2B-2a Economy UI & `/multi` Routes — Implementation Plan

- **Spec ID:** `SPEC-multi-2d-phase2b-economy-ui-V1`
- **Branch:** `feat/multi-2d-phase2b2a-economy-ui-20261126-v1`
- **Base:** `6d72c30538b1fbe30d3cbcf083b5b368733662b8`

---

## 1. File Plan (Strictly Inside Allowed Fence)

### Spec Quartet (`specs/multi-2d-phase2b-economy-ui-v1/`)
1. `spec.md` — Normative UI/route specification recording D1b, D2a, D3, D4, `2B2a-01..08` + `2B2a-REG`, zero-JS-math, contractor-only Fell, €2,500 floor, single History drawer, and Save v3 deferral.
2. `plan.md` — This file plan and verification strategy.
3. `tasks.md` — Execution checklist.
4. `checklists/requirements.md` — Traceability matrix from `2B2a-01..08` + `2B2a-REG` to verification checks.

### Server Endpoint Layer (`Tools/MultiRackSitkaP1Lab/src/Ui/`)
1. `PlayerApi.cs` — Single-line constructor change to initialize `multiCurrent` (`MultiSessionPair`) for lock-safe D2a session swap; zero edits to `/api/*` or guards.
2. `MultiRoutes.cs` — Replace readonly `multiArea`/`multiSession` fields with lock-safe `MultiSessionPair` swap (`Volatile.Read` + `multiSwapSync`), register the 5 economy routes (`GET api/multi/economy/view`, `POST api/multi/economy/session`, `POST api/multi/economy/quote`, `POST api/multi/economy/executor`, `POST api/multi/economy/history/year`), add bounded member lists and request records, and add `kind`/`targetKey`/`executor` to `RequireBoundedText`. Keep `MultiPageModules` strictly byte-unchanged (14 modules).
3. `MultiApi.View.cs` — Snapshot `MultiSessionPair` once per read; add `[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] MultiEconomySnapshotDto? Economy = null` to view/aux payloads (strictly `null`/omitted when Economy is Off); implement server-side DTO formatting for cash, landowner hours (`X / 40 hrs`), harvest stage, €2,500 floor flag, assortments, non-harvest lines, issues, settlements, and ledger entries (zero JS math).
4. `MultiApi.Commands.cs` — Snapshot `MultiSessionPair` once per command; attach `MultiEconomySnapshotDto?` to `MultiCommandPayload` only when Economy is On; implement `ReadMultiEconomyViewAsync`, `SwitchMultiEconomySessionAsync`, `QuoteMultiEconomyAsync`, and `SetMultiOrderExecutorAsync`.
5. `MultiApi.History.cs` — Snapshot `MultiSessionPair` once per request; attach `MultiEconomySnapshotDto?` to `MultiSkipPayload` only when Economy is On; implement `ReadMultiEconomyHistoryYearAsync` (`POST api/multi/economy/history/year`). Leave `ReadMultiHistoryYearAsync` (`HistoryYearDetail`) unchanged when Economy is Off.
6. `MultiApi.Saves.cs` — Snapshot `MultiSessionPair` once per request; catch `P1Refusal` (`SaveEconomyRequiresV3`) in `ReadMultiSavesAsync` so `GET api/multi/saves` returns HTTP 200 with empty slots + typed refusal notice instead of HTTP 500 when Economy is On.

### Frontend (`Tools/MultiRackSitkaP1Lab/src/Ui/multi/`, 14 Existing Modules Only)
1. `multi.html` — Add `#economy-toggle` button in `.top-actions`, `<section id="economy-strip" hidden>`, `<section id="economy-quote-panel" hidden>` inside `#pane-plan`, `<div id="execute-economy-summary" hidden>` inside `#execute-dialog`, and `<dialog id="economy-toggle-dialog">`. Zero inline `<script>`, zero inline `style=`.
2. `multi.css` — Dark woodland theme styles for `.economy-toggle`, `.economy-strip`, `.economy-quote`, `.econ-floor-highlight`, `.econ-verdict-ok`, `.econ-verdict-blocked`, `.exec-pill`, `.exec-locked`, `.exec-select`, `.history-card-finance`, with compact 1280×720 rules preserving zero horizontal scroll and single-row topbar.
3. `store.js` — Add economy state fields (`economy`, `economyToggleTarget`, `historyEconomyDetail`, `historyEconomyError`) and `isEconomyEnabled()` helper.
4. `api.js` — Capture `payload.economy || null` in `applyPayload`, add economy refusal codes to `recoveryFor`, and enforce `#approve` / `#execute` and executor control gating in `renderCommandGate`.
5. `commands.js` — Implement launch-time toggle dialog + `POST api/multi/economy/session` reset flow, `onSetOrderExecutor` (`POST api/multi/economy/executor`), and frozen quote summary in `openExecuteDialog`.
6. `panels/work-plan.js` — Render `#economy-quote-panel`, contractor-only Fell badge + tooltip, non-harvest `Contractor`/`Landowner` selector (avoiding `.mono` inside `#pending-areas` / `#approved-areas`), financial settlement in `renderReceipt`, and economy provenance in `renderDetails` when On.
7. `panels/time-history.js` — Fetch `POST api/multi/economy/history/year` and render `Financial Ledger — Year Y` inside the existing History drawer when On; strictly untouched when Off.
8. `dialogs/saves.js` — Surface `SaveEconomyRequiresV3` notice cleanly and disable saving while Money is On; export `resetSessionUiState` for clean session swaps.
9. `main.js` — Render `#economy-toggle` and `#economy-strip` in `renderStatus`, and wire toggle dialog and per-order executor controls.

### New Headless Edge Economy Test Suite (`Tools/MultiRackSitkaP1Lab/tests/phase2b2a-economy-ui/`)
1. `run-economy-edge.sh` — Starts ephemeral `sitkap1-ui` processes (with PID-scoped cleanup only), runs `drive-economy.mjs` in headless Microsoft Edge over CDP against both `ma-1` and `ma-1-mixed` at both `1920×1080` and `1280×720`, writes evidence outside the repository.
2. `drive-economy.mjs` — Proves `2B2a-01..08` end-to-end across both viewports with screenshots, console/CSP/HTTP checks, atomic settlement + replay no-double-charge check, €2,500 floor highlight, upfront-cash block, landowner hours deduction + 40 h reset on Advance 1 Year, and single History drawer Off (`NOT RECORDED`) vs On (financial ledger rows).

---

## 2. Verification Strategy

Using pinned `.NET 8.0.133` (`$HOME/.dotnet-8.0.133/dotnet`) and verified external `.p0ref` seed:
1. Build `MultiRackSitkaP1Lab.sln`, `SitkaP1.Core.Tests`, `Phase1Save.Tests`, and `SitkaP1.Ui` (0 warnings, 0 errors).
2. Run `SitkaP1.Core.Tests` (`291/291`) and `Phase1Save.Tests` (`50/50`).
3. Run `tests/ui-smoke-multiarea.sh` (`363/0`) and `tests/ui-smoke.sh` (`250` pass, only `{L01,L02,L06,J31}` fail).
4. Commit final changes, then run all browser suites against the final commit:
   - `tests/phase1-save-ui/run-edge-e2e.sh` (`63/63`)
   - `tests/phase1-time-history-ui/run-step2-edge.sh` (`67/67`)
   - `tests/phase2b2a-economy-ui/run-economy-edge.sh` (both viewports PASS)
5. Run `build/2d-gate.sh --base 6d72c30538b1fbe30d3cbcf083b5b368733662b8 --p0ref-dir $SEED --headless-only` (exit 0).
6. Verify forbidden-path diff check, capability-token grep, clean `git status --short`, and push `feat/multi-2d-phase2b2a-economy-ui-20261126-v1` to `origin`.
