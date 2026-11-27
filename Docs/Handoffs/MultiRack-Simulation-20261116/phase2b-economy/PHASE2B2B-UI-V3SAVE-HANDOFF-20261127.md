# CCF 2D Simulator — Phase 2B-2b Part 2 (Dispositions UI + Save v3 Economy Block + Deadwood Wiring) Handoff

- **Date:** 2026-11-27
- **Status:** Ready for independent review (pushed to feature branch; NOT merged to `integration/2d-sim`; `origin/main` untouched)
- **Lane / Worktree:** `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-2D-Phase2B2b-UI-20261127`
- **Branch:** `feat/multi-2d-phase2b2b-dispositions-ui-v3save-20261127-v1`
- **Branch URL:** https://github.com/jermur788/CCF/tree/feat/multi-2d-phase2b2b-dispositions-ui-v3save-20261127-v1
- **Base Commit (`origin/integration/2d-sim`):** `24871551ae906b6911b48bb17c252c023bdea502` ([commit](https://github.com/jermur788/CCF/commit/24871551ae906b6911b48bb17c252c023bdea502))
- **Implementation Commits (prior to this handoff commit):**
  - Core / Deadwood / S1 / Save v3 codec: `7ab0afad6b5f83b9248adb9bdf371a5e04f66c47` ([commit](https://github.com/jermur788/CCF/commit/7ab0afad6b5f83b9248adb9bdf371a5e04f66c47))
  - `/multi` routes / UI / Tests: `2f0f2a1f8fc7eb7251db5917a083f2b448f1896f` ([commit](https://github.com/jermur788/CCF/commit/2f0f2a1f8fc7eb7251db5917a083f2b448f1896f))
- **Frozen `origin/main` (untouched):** `bdeb6a324acaf4129c10fb0846faac8b64974360`

---

## 1. Scope Summary & Fence Compliance

All changes stay strictly inside the authorised writable set (including the two Manager fence extensions). Zero files were modified under `Assets/**`, `Packages/**`, `ProjectSettings/**`, `src/Ui/wwwroot/**`, `SaveV3Format.cs`, `SaveV3Slots.cs`, `MultiSessionHost.cs`, `SitkaP1.Core.csproj`, `build/2d-gate/**`, or `Docs/Project/**`. Zero new `.js` modules were added under `Tools/MultiRackSitkaP1Lab/src/Ui/multi/**`. The new Save v3 economy codec file lives physically under `Tools/MultiRackSitkaP1Lab/src/Core/Phase1/EconomyFeatureBlockCodec.cs` (namespace `SitkaP1.Core.Saves`).

### Diff Stat vs Base (`24871551ae906b6911b48bb17c252c023bdea502`, code & test commits)

```text
 Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/Economy/EconomyQuoteCalculator.cs                   |   3 +
 Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/Economy/MultiAreaContinuation.Commands.Economy.cs   |   3 +
 Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/MultiAreaContinuation.Commands.Plan.cs              |  13 +
 Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/MultiAreaContinuation.cs                            |  57 +-
 Tools/MultiRackSitkaP1Lab/src/Core/Phase1/EconomyFeatureBlockCodec.cs                            | 981 +++++++++++++++++++++
 Tools/MultiRackSitkaP1Lab/src/Core/Phase1/Phase1SaveFormat.cs                                    |   8 +
 Tools/MultiRackSitkaP1Lab/src/Core/Saves/FeatureBlockRegistry.cs                                 |   7 +-
 Tools/MultiRackSitkaP1Lab/src/Core/Saves/GenesisRecipeBlockCodec.cs                              |   3 -
 Tools/MultiRackSitkaP1Lab/src/Core/Saves/SaveV3Codec.cs                                          |  18 +-
 Tools/MultiRackSitkaP1Lab/src/Ui/MultiApi.Commands.cs                                            |  13 +-
 Tools/MultiRackSitkaP1Lab/src/Ui/MultiApi.Saves.cs                                               |  49 +-
 Tools/MultiRackSitkaP1Lab/src/Ui/MultiApi.View.cs                                                | 224 ++++-
 Tools/MultiRackSitkaP1Lab/src/Ui/MultiRoutes.cs                                                  |  12 +-
 Tools/MultiRackSitkaP1Lab/src/Ui/multi/api.js                                                    |   7 +-
 Tools/MultiRackSitkaP1Lab/src/Ui/multi/commands.js                                               |  23 +-
 Tools/MultiRackSitkaP1Lab/src/Ui/multi/dialogs/saves.js                                          |  18 +-
 Tools/MultiRackSitkaP1Lab/src/Ui/multi/main.js                                                   |  27 +-
 Tools/MultiRackSitkaP1Lab/src/Ui/multi/multi.css                                                 |  14 +-
 Tools/MultiRackSitkaP1Lab/src/Ui/multi/multi.html                                                |  10 +-
 Tools/MultiRackSitkaP1Lab/src/Ui/multi/panels/time-history.js                                    |  14 +
 Tools/MultiRackSitkaP1Lab/src/Ui/multi/panels/work-plan.js                                       |  76 +-
 Tools/MultiRackSitkaP1Lab/tests/Core.Tests/Phase2A2Tests.cs                                      | 113 ++-
 Tools/MultiRackSitkaP1Lab/tests/Core.Tests/Phase2BEconomyTests.cs                                |  70 ++
 Tools/MultiRackSitkaP1Lab/tests/Core/EconomyDispositions/DispositionTests.cs                     |  16 +-
 Tools/MultiRackSitkaP1Lab/tests/Phase1.Save.Tests/Program.cs                                     |  73 ++
 Tools/MultiRackSitkaP1Lab/tests/phase2b2b-ui/drive-phase2b2b.mjs                                 | 572 ++++++++++++
 Tools/MultiRackSitkaP1Lab/tests/phase2b2b-ui/run-phase2b2b-edge.sh                               | 130 +++
 27 files changed, 2440 insertions(+), 114 deletions(-)
```
Plus this handoff document (`Docs/Handoffs/MultiRack-Simulation-20261116/phase2b-economy/PHASE2B2B-UI-V3SAVE-HANDOFF-20261127.md`, 28 changed files total).

### Changed Paths (`git diff --name-only` vs `24871551ae906b6911b48bb17c252c023bdea502`)

- `Docs/Handoffs/MultiRack-Simulation-20261116/phase2b-economy/PHASE2B2B-UI-V3SAVE-HANDOFF-20261127.md`
- `Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/Economy/EconomyQuoteCalculator.cs`
- `Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/Economy/MultiAreaContinuation.Commands.Economy.cs`
- `Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/MultiAreaContinuation.Commands.Plan.cs`
- `Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/MultiAreaContinuation.cs`
- `Tools/MultiRackSitkaP1Lab/src/Core/Phase1/EconomyFeatureBlockCodec.cs`
- `Tools/MultiRackSitkaP1Lab/src/Core/Phase1/Phase1SaveFormat.cs`
- `Tools/MultiRackSitkaP1Lab/src/Core/Saves/FeatureBlockRegistry.cs`
- `Tools/MultiRackSitkaP1Lab/src/Core/Saves/GenesisRecipeBlockCodec.cs`
- `Tools/MultiRackSitkaP1Lab/src/Core/Saves/SaveV3Codec.cs`
- `Tools/MultiRackSitkaP1Lab/src/Ui/MultiApi.Commands.cs`
- `Tools/MultiRackSitkaP1Lab/src/Ui/MultiApi.Saves.cs`
- `Tools/MultiRackSitkaP1Lab/src/Ui/MultiApi.View.cs`
- `Tools/MultiRackSitkaP1Lab/src/Ui/MultiRoutes.cs`
- `Tools/MultiRackSitkaP1Lab/src/Ui/multi/api.js`
- `Tools/MultiRackSitkaP1Lab/src/Ui/multi/commands.js`
- `Tools/MultiRackSitkaP1Lab/src/Ui/multi/dialogs/saves.js`
- `Tools/MultiRackSitkaP1Lab/src/Ui/multi/main.js`
- `Tools/MultiRackSitkaP1Lab/src/Ui/multi/multi.css`
- `Tools/MultiRackSitkaP1Lab/src/Ui/multi/multi.html`
- `Tools/MultiRackSitkaP1Lab/src/Ui/multi/panels/time-history.js`
- `Tools/MultiRackSitkaP1Lab/src/Ui/multi/panels/work-plan.js`
- `Tools/MultiRackSitkaP1Lab/tests/Core.Tests/Phase2A2Tests.cs`
- `Tools/MultiRackSitkaP1Lab/tests/Core.Tests/Phase2BEconomyTests.cs`
- `Tools/MultiRackSitkaP1Lab/tests/Core/EconomyDispositions/DispositionTests.cs`
- `Tools/MultiRackSitkaP1Lab/tests/Phase1.Save.Tests/Program.cs`
- `Tools/MultiRackSitkaP1Lab/tests/phase2b2b-ui/drive-phase2b2b.mjs`
- `Tools/MultiRackSitkaP1Lab/tests/phase2b2b-ui/run-phase2b2b-edge.sh`

---

## 2. Per-File Summary

### Core Engine, S1 Policy & Deadwood Materialisation
- **`src/Core/MultiArea/Economy/MultiAreaContinuation.Commands.Economy.cs`**: Refuses `SetFellingDisposition(..., FellingDisposition.LeaveAsDeadwood)` with typed `P1Refusal("EconomyDeadwoodRequiresStage4", ...)` whenever `root.Stage4 is null`.
- **`src/Core/MultiArea/Economy/EconomyQuoteCalculator.cs`**: Fail-closed quote/approve-time guard throwing `EconomyDeadwoodRequiresStage4` if a `LeaveAsDeadwood` disposition is evaluated when `root.Stage4 is null`.
- **`src/Core/MultiArea/MultiAreaContinuation.Commands.Plan.cs`**: In `Execute`, right after `EconomyQuoteCalculator.BuildSettlement`, materialises settled `DeadwoodIntents` via `FelledStemDeadwood.Materialise(settlement.DeadwoodIntents, nextStage4.NextDeadwoodNumber)` into `nextStage4.Deadwood` and updates `nextStage4.NextDeadwoodNumber`, refusing fail-closed (`EconomyDeadwoodRequiresStage4`) if `nextStage4 is null`.

### Save v3 Economy Block Codec & Save/Load Routing
- **`src/Core/Phase1/EconomyFeatureBlockCodec.cs`** *(new, namespace `SitkaP1.Core.Saves`)*: Implements `IFeatureBlockCodec` for `blocks.economy` (`blockVersion = 1`, `CanRead(v) => v == 1`) with canonical compact UTF-8 JSON encoding/decoding, strict key-order & canonical byte verification, full monetary/volume/hour/ledger/retained-timber invariant validation, settlement re-attachment onto `Executed`, and `[assembly: InternalsVisibleTo("Phase1Save.Tests")]` for direct Save v3 round-trip tests in `Phase1.Save.Tests`.
- **`src/Core/Phase1/Phase1SaveFormat.cs`**: Adds `internal static Phase1EngineLineage Current(Phase1ModelVector models, EconomyModelVersion economy)` overload so Economy v3 saves stamp `+economy-v1` in `AreaModelVersion` (matching executed receipt tokens validated by `Phase1SaveValidator`) without changing public `Current(Phase1ModelVector)` or Save v2 bytes.
- **`src/Core/Saves/FeatureBlockRegistry.cs`**: Replaces the reserved `"economy"` slot in `CreateDefault()` with `registry.Register(new EconomyFeatureBlockCodec())`.
- **`src/Core/Saves/GenesisRecipeBlockCodec.cs`**: Removes only the stale 3-line `SaveEconomyRequiresV3` refusal in `Encode`.
- **`src/Core/Saves/SaveV3Codec.cs`**: Removes the stale `"economy reserved"` refusal in `VerifyRecipeGenesis`, enforces `genesisRecipe.EconomyEnabled` ↔ `blocks.economy` consistency in both `Encode` and `Decode`, and computes expected lineage with `EconomyModelVersion.Enabled` when `genesis.EconomyEnabled` is true.
- **`src/Core/MultiArea/MultiAreaContinuation.cs`**: Allows `CaptureSave` for Forest Maker + Economy sessions (writes Save v3 with `blocks.economy`) while keeping built-in (`GenesisRecipe is null`) Economy sessions refused with `SaveEconomyRequiresV3`; wires `RestoreFromV3` to instantiate `MultiAreaModelSelection.Phase2WithEconomy` with decoded `EconomyOptions`; checks `RequireSameOptions` including `EconomyOptions` inside `RestoreVerifiedV3IntoFreshSession`.

### `/multi` HTTP Routes, DTOs & Existing Frontend Modules (Zero New `.js` Modules)
- **`src/Ui/MultiRoutes.cs`**: Registers `POST api/multi/economy/disposition` with full `MultiFenceMembers` + `areaId`, `treeId`, `disposition` bounded-text validation.
- **`src/Ui/MultiApi.Commands.cs`**: Implements `SetMultiFellingDispositionAsync` (strict case-sensitive enum parse throwing `EconomyDispositionUnknown`) and passes `active.Session.IsForestMaker` to `BuildEconomySnapshot`.
- **`src/Ui/MultiApi.Saves.cs`**: Lists both v2 and v3 slots regardless of whether Money is On or Off; marks v3 Forest Maker saves (Money On or Off) as `status="valid"`, `canLoad=true`, carrying `economyEnabled`; marks v2 slots incompatible with `SaveEconomyRequiresV3` when Money is On; surfaces top-level `SaveEconomyRequiresV3` only for built-in (`!IsForestMaker`) Economy sessions.
- **`src/Ui/MultiApi.View.cs`**: Projects server-authoritative retained timber (cumulative + delta + per-species + formatted m³), per-stem dispositions, deadwood intent counts, `CanSaveV3`, History drawer `RetainedTimberAddedCm3` / `RetainedTimberAddedDisplay` (`"NOT RECORDED"` when Off) / `RetainedTimberBySpecies` / `StemDispositions`, updated disclaimers/tooltips, `FellingDispositionEvidence.All` labels, and `EconomyDeadwoodRequiresStage4` plain-English explanation.
- **`src/Ui/multi/multi.html`**, **`multi.css`**, **`main.js`**, **`commands.js`**, **`api.js`**, **`panels/work-plan.js`**, **`panels/time-history.js`**, **`dialogs/saves.js`**:
  - Top resource strip adds `Retained` (`#economy-retained`, species breakdown tooltip) alongside `Cash`, `Landowner` (`X / 40 hrs`), and `Harvest stage` when Money is On.
  - Pending Fell orders show 3-way segmented disposition buttons (`Sell & Extract`, `Keep for Own Use`, `Leave as Deadwood`) and server-supplied read-only readout (ZERO `<input>` volume box under Keep for Own Use). Approved Fell orders lock to a read-only disposition pill.
  - Execute confirmation dialog and Execution receipt include server-calculated Retained timber added and Fallen deadwood created.
  - Existing single History drawer (`#history-open` count = 1) adds Retained timber added and a `Felling dispositions — Year Y` table inside the Financial Ledger section only when Money is On; Money Off stays untouched (`FinancialLedger` + `NOT RECORDED`, zero `€`).
  - Saved games dialog enables Save v3 for Forest Maker Money-On sessions (`canSaveV3`), badges v3 Economy slots with `v3` and `Money`, and keeps built-in Money-On saves cleanly blocked with guidance.

### Tests
- **`tests/Core.Tests/Phase2A2Tests.cs`**: Updated C-V3 economy test to verify default registry registration, custom reserved-slot behavior, Forest Maker Economy Save v3 → Decode → `RestoreFromV3` + 2-year deterministic continuation, built-in `SaveEconomyRequiresV3` refusal, `SaveOptionsMismatch`, and fail-closed tampered/missing/bad-version economy blocks.
- **`tests/Core.Tests/Phase2BEconomyTests.cs`**: Added tests for Stage 4 `DW####` deadwood materialisation + replay idempotency and S1 Legacy / Stage3CrownC2 `EconomyDeadwoodRequiresStage4` refusal.
- **`tests/Core/EconomyDispositions/DispositionTests.cs`**: Updated case #10 to assert Stage 4 `Deadwood` appends 3 `DW####` (`FellCause.Harvest`) records and increments `NextDeadwoodNumber` by 3 on Execute.
- **`tests/Phase1.Save.Tests/Program.cs`**: Preserved frozen v2 golden sample (`24,703 B`) and added Save v3 Economy Save → fresh `SaveV3Slots` reload → `RestoreFromV3` round-trip and fail-closed tests.
- **`tests/phase2b2b-ui/run-phase2b2b-edge.sh`** & **`drive-phase2b2b.mjs`**: Targeted headless Microsoft Edge acceptance suite at `1920x1080` and `1280x720` including full Save v3 → SIGINT server stop → fresh server restart → Load v3 verification.

---

## 3. Dispositions, Retained m³, Deadwood & S1 Rules

1. **Three dispositions on pending Fell orders**: `SellAndExtract` (default), `KeepForUse`, `LeaveAsDeadwood`.
2. **Server-authoritative, zero JS math**: All quotes, graded stem volumes (`RetainedTimberCm3` / `RetainedTimberM3`), revenues, €2,500 contractor harvest minimum floor checks, upfront-cash blocks, landowner hour deductions, and deadwood intents come strictly from `.NET Core`. Under `KeepForUse`, there is NO editable volume input in the UI.
3. **Strict Work Plan lifecycle**: Changing a disposition calls `POST api/multi/economy/disposition`, increments `planStateRevision`, and returns a fresh server quote. Changing a disposition while approved is refused with `409 WorkPlanAlreadyApproved`; stale fences refuse with `409 PlayerPlanStateStale` / `PlayerSourceStale` / `PlayerSessionMismatch`.
4. **Deadwood materialisation**: On `Execute`, settled `LeaveAsDeadwood` intents are materialised via `FelledStemDeadwood.Materialise(...)` into `Stage4.Deadwood` with sequential `DW####` IDs, `Cause = FellCause.Harvest` (`"fell"`), exact float32 `VolumeM3` bits, and incremented `Stage4.NextDeadwoodNumber`. Replaying Execute returns the stored receipt without duplicating deadwood.
5. **S1 Legacy / non-Stage4 policy**: When `root.Stage4 is null` (e.g., `Legacy` or `Stage3CrownC2` with Economy enabled), `LeaveAsDeadwood` is refused fail-closed with typed `P1Refusal("EconomyDeadwoodRequiresStage4", ...)` in `SetFellingDisposition`, `EconomyQuoteCalculator.ComputeCore` (quote/approve-time guard), and `Execute`. `SellAndExtract` and `KeepForUse` continue to work normally without Stage 4.

---

## 4. Save v3 `blocks.economy` Schema (`blockVersion = 1`)

- **Block key:** `"economy"` (ordinal-sorted before `"genesisRecipe"` inside top-level `"blocks"`)
- **Wrapper:** `{"blockVersion":1,"data":{...}}`
- **Format:** Compact canonical UTF-8 JSON (no whitespace, fixed key order, `i64` encoded as `{"k":"i64","b":"<16 lowercase hex>"}`, `f32` encoded as `{"k":"f32","b":"<8 lowercase hex>"}`, verified by canonical re-encode equality on decode).
- **Exact key order of `blocks.economy.data`:**
  1. `options`: `{ openingCashCents (i64), annualLandownerAllowanceMinutes (int), contractorHourlyRateCents (i64), minimumHarvestJobCents (i64) }`
  2. `cashBalanceCents` (`i64`)
  3. `usedLandownerMinutesThisYear` (`int`)
  4. `remainingLandownerMinutes` (`int`)
  5. `executedHarvestPlansCount` (`int`)
  6. `pendingOrderExecutors`: array sorted strictly by `EconomyOrderKey`, each `{ kind, areaId, targetKey, executor }`
  7. `retainedTimber`: `{ retainedTimberCm3 (i64), bySpecies: [{ speciesId, retainedTimberCm3 (i64) }] (ordinal-sorted by speciesId), stems: [{ areaId, treeId, speciesId, stemVolumeCm3 (i64), ecologicalYear, workRevision, executionKey, planId }] }`
  8. `settlements`: array of `{ executionKey, planId, ecologicalYear, workRevision, cashBeforeCents (i64), totalExternalCostCents (i64), timberSaleRevenueCents (i64), netCashDeltaCents (i64), cashAfterCents (i64), ownerMinutesBefore, ownerMinutesConsumed, ownerMinutesAfter, harvestJob (null | HarvestJob), nonHarvestLines: [...], ledgerEntries: [...], retainedTimberDelta (RetainedTally), deadwoodIntents: [...] }`
     - `HarvestJob`: `{ stage, stageRateCentsPerTonne (i64), felledStemCount, totalStemVolumeCm3 (i64), saleVolumeCm3 (i64), residualVolumeCm3 (i64), retainedStemVolumeCm3 (i64), deadwoodStemVolumeCm3 (i64), harvestedGreenGrams (i64), variableHarvestCostCents (i64), minimumJobFloorCents (i64), minimumJobAdjustmentCents (i64), totalHarvestContractorCostCents (i64), timberSaleRevenueCents (i64), stems: [...], assortments: [...], stemDispositions: [...], retainedTimber (RetainedTally), deadwoodIntents: [...] }`
  9. `ledger`: array of `{ category, referenceId, cashDeltaCents (i64), ecologicalYear, workRevision, executionKey, planId }`
  10. `yearEndSummaries`: array of `{ yearBefore, yearAfter, openingCashCents (i64), closingCashCents (i64), yearExternalCostCents (i64), yearTimberRevenueCents (i64), yearNetCashDeltaCents (i64), yearLandownerMinutesUsed, yearLandownerMinutesRemainingBeforeReset, annualLandownerAllowanceMinutesAfterReset, harvestPlansExecutedInYear, totalPlansSettledInYear }`
- **Restore semantics:**
  - Policy A preserved: `ApprovedQuote` is always `null` on restore and any saved approved orders re-enter as unapproved `Pending`.
  - Per engine handoff §11 item 1: `PendingFellDispositions` starts empty (`FellingDispositionRules.EmptyChoices`) on restore rather than binding onto rebuilt `MultiAreaWorkOrder` instances; players explicitly re-select any non-default pending disposition before re-approving.
  - Restored `Settlements` are re-attached onto `Executed` by `ExecutionKey`.
  - Invariants verified on both encode and decode: valid `EconomyOptions`, non-negative cash & minutes, `used + remaining == annualAllowance`, per-settlement `revenue - cost == net`, `cashBefore + net == cashAfter`, `sum(settlement.ledgerEntries) == net`, harvest stem volume conservation (`sale + residual + retained + deadwood == total`), `executedHarvestPlansCount == settled harvest count`, `retainedTimber` exact match to stems/species/settlement deltas, and `openingCash + sum(ledger) == cashBalanceCents`.
  - Built-in (`MA-1` / `MA-1-MIXED`) sessions with Economy enabled still refuse save with `SaveEconomyRequiresV3`. Frozen Save v2 stays completely unchanged (`24,703 B`, SHA-256 `c0ea8a460d9ebee53dc6e2f03bd88c72032d2916fa9614f1fc370532b5b7bee7`).

---

## 5. Verification (Actually Run, 2026-11-27)

Toolchain pinned for all builds/runs: `dotnet --version` = `8.0.134` (`$HOME/.dotnet/dotnet`), `node --version` = `v26.13.1`, Windows Node + Microsoft Edge over CDP for browser checks. (Worktree gitignored `.p0ref/` was populated from the verified `#589` `p0ref-seed` matching `SitkaP1.Core.csproj`'s pinned `SitkaLab.Core.dll` SHA-256 `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3`.)

| Check | Exact Command | Result |
|---|---|---|
| Build UI (`SitkaP1.Ui`) | `dotnet build src/Ui/SitkaP1.Ui.csproj -c Debug --disable-build-servers` | **PASS** — 0 warnings, 0 errors |
| Build Core & Test projects | `dotnet build tests/Core.Tests/SitkaP1.Core.Tests.csproj -c Debug --disable-build-servers` (+ `EconomyDispositions`, `Phase1.Save.Tests`) | **PASS** — 0 warnings, 0 errors |
| `EconomyDispositions.Tests` | `dotnet run --project tests/Core/EconomyDispositions/SitkaP1.EconomyDispositions.Tests.csproj -c Debug --no-build` | **PASS** — **22/22 passed, 0 failed** (determinism hash `d9c7aeddf2eb629b388c6cc3dfa93d5a55bd27c85e6262b323ddcbe6f14498ed`; case #10 Stage 4 deadwood verified) |
| `Phase1.Save.Tests` | `dotnet run --project tests/Phase1.Save.Tests/Phase1Save.Tests.csproj -c Debug --no-build` | **PASS** — **52/52 passed, 0 failed** (v2 golden sample byte-identical `24,703 B`, NB1c passed first attempt with 0 flakes, Save v3 Economy Save → fresh reload → `RestoreFromV3` passed) |
| `Core.Tests` | `dotnet run --project tests/Core.Tests/SitkaP1.Core.Tests.csproj -c Debug --no-build` | **PASS** — **314/314 passed, 0 failed** (all 9 frozen simulation anchors + Anchors 10–11 + B-01 file-API scan + Phase 2B Economy + Save v3 Economy + deadwood materialisation + S1 Legacy/Stage3 refusal) |
| Headless Edge UI (`1920x1080` & `1280x720`, Save v3 → restart → Load v3) | `DOTNET=$HOME/.dotnet/dotnet bash tests/phase2b2b-ui/run-phase2b2b-edge.sh` | **PASS** — **66/66 passed, 0 failed** (**29/29** at `1920x1080`, **29/29** at `1280x720`, **8/8** clean SIGINT stops + empty stderr; zero console errors, zero CSP violations, zero 404/5xx, zero horizontal scroll, `<= 3` columns, 1 History button). Evidence dir: `/mnt/c/Users/hp/AppData/Local/Temp/ccf-phase2b2b-edge-20261127T162924Z` |
| Ports & server cleanup | `ss -ltn` & `pgrep -fa sitkap1-ui` | **PASS** — No lingering `sitkap1-ui` server; ports `33767`, `43649`, `46593` untouched/free |

### Explicitly SKIPPED per 2026-11-27 Minimum Necessary Testing Rule
- **SKIPPED:** `build/2d-gate/selftest.sh`
- **SKIPPED:** Full `build/2d-gate.sh --browser`
- **SKIPPED:** Legacy Edge suites outside the targeted Phase 2B-2b runner (including `tests/phase2b2a-economy-ui/run-economy-edge.sh`, which explicitly asserts the older Phase 2B-2a `"dispositions are deferred"` text that Phase 2B-2b intentionally replaces)

### Not Verified
- Unity 6000.6.0f1 3D editor/build/runtime (zero Unity files touched).
- Full `build/2d-gate.sh` sweep or cross-OS browser checks outside Windows Microsoft Edge over CDP.
- Built-in stand (`MA-1` / `MA-1-MIXED`) Save dialog UI flow in headless Edge (covered by unit tests in `Phase2A2Tests` & `Phase1.Save.Tests`, not driven as a separate browser phase in `drive-phase2b2b.mjs`).

---

## 6. Residual Risks & Assumptions Adopted

### Residual Risks
1. **Legacy `tests/phase2b2a-economy-ui/drive-economy.mjs` expectation drift**: That earlier Phase 2B-2a browser script asserted `hasKeepForUse === false` and `"sell-and-extract"` / `"Save v3 deferred"` strings that are now superseded by Phase 2B-2b. Any future run of `phase2b2a-economy-ui` will need those Phase 2B-2a-era deferral assertions retired or replaced by `tests/phase2b2b-ui/`.
2. **Pending Fell dispositions across Save v3 load**: By design (`PendingFellDispositions` starts empty on restore per engine handoff §11 item 1), any unexecuted pending Fell order with a non-default disposition resets to `SellAndExtract` after Save → Load; the UI makes the active disposition explicit on each pending row so the player sees it before approving.
3. **`.p0ref` deterministic hash under SDK 8.0.134**: Building P0 `SitkaLab.Core.dll` under SDK `8.0.134` (or current Ubuntu ref packs) produces a different PE metadata hash than `SitkaP1.Core.csproj`'s pinned `9a99f781...`; local runs used the verified `#589` `p0ref-seed` binary without modifying `SitkaP1.Core.csproj`.

### Assumptions Adopted
1. **Manager fence extensions**:
   - `Tools/MultiRackSitkaP1Lab/src/Core/Saves/GenesisRecipeBlockCodec.cs` — removed only the stale 3-line Economy refusal in `Encode`.
   - `Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/Economy/MultiAreaContinuation.Commands.Economy.cs` & `EconomyQuoteCalculator.cs` — added the S1 `EconomyDeadwoodRequiresStage4` refusal in `SetFellingDisposition` and quote/approve-time `ComputeCore`.
2. **S1 policy decision**: Adopted the owner-authorised refusal (`EconomyDeadwoodRequiresStage4`) whenever `LeaveAsDeadwood` is requested or evaluated on a session without `Stage4`, rather than silently dropping deadwood intents.
3. **`Phase1.Save.Tests` visibility**: Added `[assembly: InternalsVisibleTo("Phase1Save.Tests")]` at the top of `src/Core/Phase1/EconomyFeatureBlockCodec.cs` (inside the authorised `src/Core/Phase1/**` fence, leaving `SitkaP1.Core.csproj` untouched) so `Phase1.Save.Tests` directly proves the Save v3 Economy Save → fresh slot reload → `RestoreFromV3` round-trip alongside the frozen v2 golden sample.
