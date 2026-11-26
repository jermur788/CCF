# CCF Multi 2D — Phase 2B-1 Economy Engine — Implementation Plan

- **Spec ID:** `SPEC-multi-2d-phase2b-economy-V1`
- **Branch:** `feat/multi-2d-phase2b-economy-core-20261126-v1`
- **Base:** `5648813d9ccbe8d38e9fbe5df38d62c810af4a22`

## 1. File Plan

### New Engine Files (`Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/Economy/`)
1. `EconomyModels.cs` — `EconomyModelVersion`, `EconomyOptions`, `EconomyIdentity`, `WorkExecutor`, `HarvestThinningStage`, `EconomyOrderKind`, `EconomyOrderKey`, `EconomyEvidenceLabel`, `EconomyDefaults`.
2. `D062TimberAssortment.cs` — Paraboloid stem volume-budgeted log allocator, Sitka/Oak/Beech ladders, pruning quality bridge, `890 kg/m³` proxy conversion, valuation.
3. `EconomyRecords.cs` — Immutable `MultiAreaEconomyState`, quote/approval/settlement/ledger/year-end/view DTOs, and `EconomyHistoryQuery` (`NOT RECORDED` when Off).
4. `EconomyQuoteCalculator.cs` — Pure quote engine, upfront-cash/allowance/contractor-Fell validation, atomic settlement builder, and year-end summary + allowance reset builder.
5. `EconomyCalibrationFixture.cs` — Deterministic 80×80 m / 5-lane / 1,344-stem 0.64 ha calibration fixture.
6. `MultiAreaContinuation.Commands.Economy.cs` — Fenced economy read/quote/history/executor/harvest-stage commands on `MultiAreaContinuation`.

### Minimal Additive Wiring (Shared Core Files)
1. `src/Core/Stage4Models.cs` — Trailing default `EconomyModelVersion Economy = EconomyModelVersion.Off` on `MultiAreaModelSelection` + `IsEconomyEnabled` + `Phase2WithEconomy`.
2. `src/Core/MultiArea/MultiAreaContracts.cs` — Trailing default `EconomySettlementReceipt? Settlement = null` on `MultiAreaExecutionResult`.
3. `src/Core/MultiArea/AnnualPipeline.cs` — `YearInProgress.NextEconomy` + `ReservedPhase2BYearEndSummaryStep` hook (no-op when `Start.Economy is null`; step array untouched).
4. `src/Core/MultiArea/MultiAreaContinuation.cs` — Append `MultiAreaEconomyState? Economy = null` last on `MultiAreaRoot`; optional `Settlement = null` on `MultiAreaExecutedWork`; extend `HasUnexecutedWork`, `RequireSameOptions`, `Advance` root publish, and `CaptureSave`/`SaveExpectation`/`Restore` typed `SaveEconomyRequiresV3` refusal.
5. `src/Core/MultiArea/MultiAreaContinuation.Commands.Plan.cs` — Freeze quote in `Approve`, clear in `CancelApproved`, settle atomically in `Execute` under `sync` and return stored settlement on replay.
6. `src/Core/MultiArea/MultiAreaContinuation.Commands.{Fell,Prune,Clear,Planting}.cs` — Prune removed pending order from `PendingOrderExecutors` when Economy is enabled.
7. `src/Core/MultiArea/MultiAreaHistoryQuery.cs` — Include `FinancialLedger` `NOT RECORDED` disclosure in `ReadYear` when `root.Economy is null` (zero new properties on existing `History*` DTOs).

### Tests (Minimum Required Set)
1. `tests/Core.Tests/Phase2BEconomyTests.cs` — Exactly 8 test cases.
2. `tests/Core.Tests/Program.cs` — Single registration line `Phase2BEconomyTests.Register();`.

## 2. Verification Strategy
1. Seed `.p0ref` from the verified read-only seed in the integration worktree (git-ignored, never committed).
2. Build and run `SitkaP1.Core.Tests` (`>= 290` passed, `0` failed; all 9/9 frozen anchors intact).
3. Build and run `Phase1Save.Tests` (`>= 50` passed, `0` failed).
4. Commit cleanly and run `build/2d-gate.sh --base 5648813d9ccbe8d38e9fbe5df38d62c810af4a22 --p0ref-dir ... --headless-only` to observe and report every gate stage truthfully.
