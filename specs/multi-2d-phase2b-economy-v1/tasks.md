# CCF Multi 2D — Phase 2B-1 Economy Engine — Task Checklist

- [x] **T-00** Run Step 0 worktree guard (`GUARD OK` on `feat/multi-2d-phase2b-economy-core-20261126-v1` @ `5648813d`).
- [x] **T-01** Read frozen contracts (`frozen-contracts-s0b.md`), S0 spec, `MultiAreaContinuation`, `MultiAreaContracts`, `MultiAreaWorkPlanAdapter`, 3D references (`Stage1EconomyDefaults`, `TimberYieldConfiguration`), and gate manifests.
- [x] **T-02** Author concise spec quartet under `specs/multi-2d-phase2b-economy-v1/`.
- [x] **T-03** Implement `src/Core/MultiArea/Economy/` (`EconomyModels`, `D062TimberAssortment`, `EconomyRecords`, `EconomyQuoteCalculator`, `EconomyCalibrationFixture`, `MultiAreaContinuation.Commands.Economy`).
- [x] **T-04** Apply minimal additive Core wiring (`Stage4Models`, `MultiAreaContracts`, `AnnualPipeline`, `MultiAreaContinuation`, `Commands.Plan`, `Commands.{Fell,Prune,Clear,Planting}`, `MultiAreaHistoryQuery`).
- [x] **T-05** Add `tests/Core.Tests/Phase2BEconomyTests.cs` (exact 8 cases) and single registration line in `tests/Core.Tests/Program.cs`.
- [ ] **T-06** Verify Core tests (`>= 290`, 0 failed), Save tests (`>= 50`, 0 failed), 9/9 frozen anchors, and `2d-gate.sh --headless-only`.
- [ ] **T-07** Write implementation handoff under `Docs/Handoffs/MultiRack-Simulation-20261116/phase2b-economy/PHASE2B1-IMPLEMENTATION-HANDOFF-20261126.md`.
- [ ] **T-08** Commit and push feature branch `feat/multi-2d-phase2b-economy-core-20261126-v1` only (no merge, no PR).
