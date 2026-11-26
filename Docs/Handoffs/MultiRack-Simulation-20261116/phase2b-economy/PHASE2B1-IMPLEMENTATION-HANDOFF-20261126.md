# CCF Multi 2D — Phase 2B-1 Economy, Labour & D-062 Timber Engine — Implementation Handoff

- **Date:** 2026-11-26
- **Branch:** `feat/multi-2d-phase2b-economy-core-20261126-v1`
- **Base Commit:** `5648813d9ccbe8d38e9fbe5df38d62c810af4a22` (`origin/integration/2d-sim`, S0b tip)
- **Implementation Commit:** `204fcdeb783dc5e8879c1a95275f224d852b9712` (followed by this handoff commit on the same feature branch)
- **Worktree:** `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-2D-Phase2B-20261126`
- **Authority Scope:** Build + verification only on feature branch. Zero merges to `integration/2d-sim` or `main`; zero PRs; zero root-checkout edits.

---

## 1. File-by-File Change Summary

### Spec Quartet (`specs/multi-2d-phase2b-economy-v1/`)
- `specs/multi-2d-phase2b-economy-v1/spec.md` — Normative Phase 2B-1 specification covering opt-in switch and Off-inertness, locked integer-cent rules, C-ECON lifecycle, D-062 assortment/pricing semantics, 0.64 ha calibration stand, explicit evidence labels, and Save v3 deferral.
- `specs/multi-2d-phase2b-economy-v1/plan.md` — File plan and verification strategy.
- `specs/multi-2d-phase2b-economy-v1/tasks.md` — Task checklist (`T-00`–`T-08`).
- `specs/multi-2d-phase2b-economy-v1/checklists/requirements.md` — Requirements traceability matrix (`P2B-01`–`P2B-10`).

### New Economy Engine Files (`Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/Economy/`)
- `EconomyModels.cs` — `EconomyModelVersion { Off = 0, Enabled = 1 }` (default `Off`), `EconomyOptions`, `EconomyIdentity`, `WorkExecutor`, `HarvestThinningStage`, `EconomyOrderKind`, `EconomyOrderKey`, `EconomyEvidenceLabel`, `EconomyDefaults`, `EconomyEvidence`.
- `D062TimberAssortment.cs` — Pure .NET port of D-062 Oak/Beech/Sitka assortment ladders, DBH-anchored paraboloid taper with strict stem-volume conservation (`SaleVolumeCm3 + ResidualVolumeCm3 == StemVolumeCm3`), recorded pruning quality bridge (no price multiplier), and `890 kg/m³` fresh-volume work-equivalent proxy conversion (`€/m³ → whole €/t`).
- `EconomyRecords.cs` — Immutable `MultiAreaEconomyState`, quote/approval/settlement/ledger/year-end/view DTOs, and `EconomyHistoryQuery` (returning `NOT RECORDED` and `null` cents when Economy is `Off`, never `€0`).
- `EconomyQuoteCalculator.cs` — Pure read-only `ComputePending` / `ComputeForApproved`, upfront-cash / landowner-allowance / contractor-only-Fell validation, atomic `BuildSettlement`, and `EconomyAnnualSummary.BuildYearEnd`.
- `EconomyCalibrationFixture.cs` — Deterministic 0.64 ha calibration stand (`80 × 80 m`, 5 lanes `MA-L1`..`MA-L5`, `1,344` stems = `2,100 stems/ha`, integer SplitMix64, `€12,000` opening cash).
- `MultiAreaContinuation.Commands.Economy.cs` — Fenced economy commands/reads on `MultiAreaContinuation` (`ReadEconomyView`, `QuoteWorkPlan`, `ReadEconomyHistoryYear`, `SetOrderExecutor`, `SetFellExecutor`, `SetHarvestStageOverride`).

### Minimal Additive Core Wiring (`Tools/MultiRackSitkaP1Lab/src/Core/`)
- `src/Core/Stage4Models.cs` — Appended trailing optional `EconomyModelVersion Economy = EconomyModelVersion.Off` to `MultiAreaModelSelection`, plus `IsEconomyEnabled`, `Phase2WithEconomy`, and support check in `RequireSupported()`.
- `src/Core/MultiArea/MultiAreaContracts.cs` — Appended optional `EconomySettlementReceipt? Settlement = null` to `MultiAreaExecutionResult`.
- `src/Core/MultiArea/AnnualPipeline.cs` — Added `YearInProgress.NextEconomy` and wired `ReservedPhase2BYearEndSummaryStep.Run` (strict no-op when `Start.Economy is null`; resets annual 40.0 h landowner allowance and appends year-end summary when Enabled). `AnnualPipeline.Steps` order and length are untouched.
- `src/Core/MultiArea/MultiAreaContinuation.cs` — Appended `MultiAreaEconomyState? Economy = null` last on `MultiAreaRoot` (stays `null` when Off); appended optional `EconomySettlementReceipt? Settlement = null` on `MultiAreaExecutedWork`; extended `HasUnexecutedWork`, `RequireSameOptions` (`SameEconomy`), `Advance` single root publish, and added typed `SaveEconomyRequiresV3` refusal in `CaptureSave`, `SaveExpectation`, and `Restore` when Economy is Enabled.
- `src/Core/MultiArea/MultiAreaContinuation.Commands.Plan.cs` — Froze quote into `ApprovedQuote` inside `Approve`; cleared on `CancelApproved`; settled atomically inside `Execute` under `sync` and the same single `root` assignment; returned stored `executed.Settlement` on idempotent replay without re-settling.
- `src/Core/MultiArea/MultiAreaContinuation.Commands.{Fell,Prune,Clear,Planting}.cs` — Pruned removed pending order keys from `PendingOrderExecutors` when Economy is Enabled (inert when `null`).
- `src/Core/MultiArea/MultiAreaHistoryQuery.cs` — Added `KeyFinancialLedger` (`"FinancialLedger"`) `NOT RECORDED` disclosure in `ReadYear` when `root.Economy is null` (zero new properties added to existing `History*` DTOs).

### Tests (`Tools/MultiRackSitkaP1Lab/tests/Core.Tests/`)
- `tests/Core.Tests/Phase2BEconomyTests.cs` — Exact 8-case minimum required suite.
- `tests/Core.Tests/Program.cs` — Single registration line `Phase2BEconomyTests.Register();` beside `Phase2Tests.Register(manifestPath);`.

---

## 2. Verification Commands & Observed Summary Lines

All builds/tests ran in WSL Ubuntu with `/home/jer/.dotnet/dotnet` (SDK `8.0.134`) and `.p0ref/` seeded from `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-2D-Integration-20261125/Tools/MultiRackSitkaP1Lab/.p0ref`.

| Check | Exact Command | Observed Summary |
|---|---|---|
| Solution build | `dotnet build MultiRackSitkaP1Lab.sln -c Debug --disable-build-servers` | `Build succeeded. 0 Warning(s) 0 Error(s)` |
| Core tests build | `dotnet build tests/Core.Tests/SitkaP1.Core.Tests.csproj -c Debug --disable-build-servers` | `Build succeeded. 0 Warning(s) 0 Error(s)` |
| Core tests run | `dotnet tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests.dll` | **`290/290 passed, 0 failed`** (baseline 282 + 8 new) |
| Save tests build | `dotnet build tests/Phase1.Save.Tests/Phase1Save.Tests.csproj -c Debug --disable-build-servers` | `Build succeeded. 0 Warning(s) 0 Error(s)` |
| Save tests run | `dotnet tests/Phase1.Save.Tests/bin/Debug/net8.0/Phase1Save.Tests.dll` | **`50/50 passed, 0 failed`** (`PASS golden: pinned SHA-256 of the fixed Phase 2 approved sample encoding`) |
| UI compile check | `dotnet build src/Ui/SitkaP1.Ui.csproj -c Debug --disable-build-servers` | `Build succeeded. 0 Warning(s) 0 Error(s)` |
| 9/9 frozen anchors (G3 check) | Read-only Python cross-check against `build/2d-gate/frozen-anchors.json` + Core stdout | **`CHECK G3 (9/9 frozen SHA-256 anchors): PASS`** |
| Save v2 & Phase1 freeze (G4 check) | Read-only Python check (`src/Core/Phase1/` diff empty, zero `: IFeatureBlockCodec`, zero `SchemaVersion = 3`) | **`CHECK G4 (v2 golden, zero IFeatureBlockCodec impl, Phase1 untouched): PASS`** |
| Determinism check (G6 subset) | Read-only F1/F2 regex scan over `src/Core/MultiArea/Economy/*.cs` | **`CHECK G6 (Economy files determinism F1/F2 clean): PASS`** |
| Protected paths (G7 check) | Read-only diff check against `build/2d-gate/protected-paths.txt` (`5648813d...HEAD`) | **`CHECK G7 (protected paths, 22 changed paths): PASS`** |

### Observed Core Stdout Anchors (#7 & #9)
- Anchor #7 (`P2-03 M-08 Q6`): `776a68df048a640c849801e743db3372d9d6d76808828d0e0623222ae35ce22d | 776a68df048a640c849801e743db3372d9d6d76808828d0e0623222ae35ce22d` (exact match).
- Anchor #9 (`P2-10 MA-1-MIXED-v1 30-year fingerprint`): `93e9abda0670cb7a7f56fda91cc1e96e8bc931b330f57d99ec1b588600b9251f` (exact match).
- Anchors #1–#6 & #8: all code-pinned literals and `.p0ref/SitkaLab.Core.dll` (`9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3`) verified intact.

### All 8 Phase 2B-1 Economy Tests
1. `PASS economy: Off is inert`
2. `PASS economy: quote is pure and revision-bound`
3. `PASS economy: approve freezes the quote`
4. `PASS economy: execute settles atomically and replay never re-settles`
5. `PASS economy: upfront cash and no debt`
6. `PASS economy: 40h landowner allowance and Fell is contractor-only`
7. `PASS economy: grouped harvest floor per felling plan`
8. `PASS economy: 0.64 ha calibration and D-062 assortment`

---

## 3. Canonical Gate (`2d-gate.sh`) Result & Diagnosis

Command executed on clean committed HEAD `204fcdeb783dc5e8879c1a95275f224d852b9712`:
```bash
Tools/MultiRackSitkaP1Lab/build/2d-gate.sh   --base 5648813d9ccbe8d38e9fbe5df38d62c810af4a22   --p0ref-dir /home/jer/Development/CCF-HP-Worktrees/CCF-Multi-2D-Integration-20261125/Tools/MultiRackSitkaP1Lab/.p0ref   --headless-only   /home/jer/Development/CCF-Multi-2D-Phase2B1-Evidence-20261126/implementer-gate
```

- **Exit Code:** `3` (`PREREQUISITE MISSING`)
- **Failure Reason:** `ERROR [G0]: required tool missing on PATH: node` — WSL Ubuntu has no Linux `node` executable installed (only Windows `node.exe` exists under `/mnt/c/Program Files/nodejs`).
- **Additional Integrator Note:** Even on a host with Linux `node`, `build/2d-gate/s0a-move-table.json` currently still sets `"gateEnforce": true` from S0b, which asserts verbatim S0b line-multisets on `MultiAreaContinuation.cs`, `Commands.*.cs`, and an empty `ReservedPhase2BYearEndSummaryStep.Run` body. Per S0 ownership rules, `build/2d-gate/**` was NOT edited by this lane; the integrator will retire the S0a/S0b move-proof lock when integrating post-S0 feature work.

| Stage | Status | Notes |
|---|---|---|
| G0 | **FAIL** (exit 3) | Missing Linux `node` binary on `PATH` in WSL Ubuntu |
| G1 | NOT RUN | Verified independently via clean `dotnet build` (0 warnings, 0 errors) |
| G2 | NOT RUN | Core (`290/290`) & Save (`50/50`) verified directly; `ui-smoke*.sh`, `cli-safety.sh`, `build-output-safety.sh`, `p0-isolation.sh` NOT TESTED in gate |
| G3 | NOT RUN | 9/9 frozen anchors independently verified PASS |
| G4 | NOT RUN | v2 golden, zero `IFeatureBlockCodec` impls, `src/Core/Phase1/` untouched independently verified PASS |
| G5 | NOT RUN | Payload budgets NOT TESTED (requires `ui-smoke-multiarea.sh` + Linux `node`) |
| G6 | NOT RUN | Economy files F1/F2 determinism independently verified PASS |
| G7 | NOT RUN | Protected-path guard independently verified PASS across all changed paths |
| B | NOT RUN | Browser tier skipped (`--headless-only`, UI untouched) |

---

## 4. Evidence Labels Inventory

| Label | Items Covered |
|---|---|
| **`EMPIRICAL`** | IFA 2024 Sitka roadside price ranges (€36–40/t pulp, €42–52/t stake, €48–75/t pallet, €88–105/t sawlog); IFA combined harvesting+forwarding range (€15–€23/t); Sitka fresh mill-delivery density anchor (~890 kg/m³). |
| **`MANAGEMENT GUIDANCE`** | Commercial production felling is contractor-only (not ordinary owner DIY); Teagasc hardwood grading guidance for Oak veneer-planking (>2.7 m, mid ≥45 cm) and beam (>2.5 m, mid ≥30 cm); Teagasc typical hardwood price quotations (€180–280/m³, €145/m³, €100/m³, €60/m³); Teagasc Sitka log nominal dimensions. |
| **`GAMEPLAY CALIBRATION`** | Opening cash €12,000 on the 0.64 ha (80×80 m / 5-lane / 1,344-stem) stand; 40.0 h/year (2,400 min) landowner allowance; €45/hr non-harvest contractor labour; on-demand materials (shelter €5.00, beech €4.50, oak €5.50); €2,500 minimum harvest job floor per felling Work Plan; stage rate selections (€20/€23/€20/€15/t); broadleaf selected reference prices (e.g., €230/m³ midpoint, €30/m³ firewood), roadside interpretation, and 890 kg/m³ broadleaf work-equivalent unit convention; pruning-only quality proxy and non-harvest task durations. |
| **`SIMULATION ABSTRACTION`** | Upfront cash / no debt / no pre-financing from expected timber receipts; €0 cash wage for landowner hours; DBH-anchored paraboloid stem taper & 350 mm stump / 5 mm kerf / 100 mm scan step; atomic Work Plan Quote → Approve → Execute → Settle binding. |

---

## 5. Assumptions

1. **Default non-harvest executor:** Pending non-harvest orders (`Prune`, `ClearUndergrowth`, `Plant`, `Shelter`, `SpotClear`) default to `WorkExecutor.Contractor` unless explicitly assigned to `WorkExecutor.Landowner` via `SetOrderExecutor`. This prevents silently consuming limited owner hours without explicit player choice.
2. **Low-risk pruning boundary:** `Prune` orders with `TargetCrownBaseHeightM <= 6.5f` are eligible for `Landowner` execution (covering all scheduled Sitka 2.5/5.0/6.5 m and broadleaf 3.5/6.0 m lifts).
3. **Automatic harvest thinning stage progression:** When no explicit `SetHarvestStageOverride` is set, the stage derives from `ExecutedHarvestPlansCount` (`0 → FirstThinning €20/t`, `1 → SecondThinning €23/t`, `>= 2 → LaterThinning €20/t`), matching `ScenarioOneEconomyAdapter.cs:164`; `Clearfell (€15/t)` is selectable via `SetHarvestStageOverride`.
4. **0.64 ha calibration stand layout:** Implemented as 5 adjacent 16 × 80 m lanes (`MA-L1`..`MA-L5`, 0.128 ha each, total 0.64 ha) with `269 × 4 + 268 = 1,344` stems (`2,100 stems/ha`) using ASCII-only canonical manifest metadata (required by `CanonicalWriter.Str`).

---

## 6. Discrepancies

1. **`MultiAreaDecisionHistory` does not exist at `5648813d`:**
   - The real history query surface at `5648813d` is `MultiAreaHistoryQuery` (`Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/MultiAreaHistoryQuery.cs`), which returns `HistoryYearDetail`.
   - Furthermore, existing Phase 1 test `P1-TH-05` (`tests/Core.Tests/Phase1TimeHistoryTests.cs:425–447`) uses reflection to forbid any property containing `Money`, `Price`, `Cost`, `Cash`, `Euro`, `Revenue`, `Budget`, `Objective`, or `Weather` on all existing `History*` DTOs.
   - **Resolution used:** Added `MultiAreaHistoryQuery.KeyFinancialLedger` (`"FinancialLedger"`, `HistoryProvenance.NotRecorded`, `"NOT RECORDED"`, no `'€'` glyph) to `HistoryYearDetail.NotRecordedDisclosures` when `root.Economy is null` (preserving all existing DTO shapes and passing `P1-TH-05`), AND added a dedicated `EconomyHistoryQuery.ReadYear` / `MultiAreaContinuation.ReadEconomyHistoryYear(int year)` returning `EconomyYearHistory` whose money display fields explicitly read `"NOT RECORDED"` and numeric cents are `null` (never `€0`) when Economy is `Off`.
2. **Task brief truncated at locked-rule item 4:**
   - **Recovered from in-repo 3D sources:**
     - `ScenarioOneDefinition.cs`: opening cash `€12,000`, `2,400` owner min/year (`40.0 h`), contractor hourly `€45/hr`, harvest minimum floor `€2,500`, shelter `€5.00`, beech `€4.50`, oak `€5.50`, `Plant` `10 min/sapling`.
     - `Stage1EconomyDefaults.cs`: Sitka roadside prices (€38/€47/€68/€95/t), D-062 broadleaf prices & `890 kg/m³` proxy conversion (`CentsPerTonneAtProxy`), harvest stage ranges, `Prune` `60 min / 15 items` (4 min/tree), `Shelter` `60 min / 45 items` (`CeilRatio`).
     - `TimberYieldConfiguration.cs`, `MerchantableStemModel.cs`, `TimberYieldCalculator.cs`, `ScenarioOneEconomyAdapter.cs`: exact paraboloid taper, stump/kerf/scan constants, species-explicit Sitka/Oak/Beech ladders, pruning-to-`StemQualitySection` bridge, and `ExecutedHarvestPlansCount` stage progression.
   - **Chosen conservatively:**
     - `FirstThinning` rate set to `€20/t` (`2,000` cents/t) per the locked packet rule (`first ≈ €20`), noting `Stage1EconomyDefaults.cs` sets its reference midpoint at `2,100` within the `2,000–2,200` IFA range.
     - `ClearUndergrowth` (2.5 m / 6.25 m² cell) set to `12 min/cell` and `SpotClear` (1 m² patch) set to `4 min/patch` (`GAMEPLAY CALIBRATION`).
     - Low-risk pruning cap set to `6.5 m` (`LowRiskPruneMaxHeightM`).
     - Non-harvest orders default to `Contractor` until explicitly assigned to `Landowner`.
3. **Save test project filename in packet §10:**
   - Packet §10 listed `tests/Phase1.Save.Tests/Phase1.Save.Tests.csproj` / `Phase1.Save.Tests.dll`; the actual tracked filename is `tests/Phase1.Save.Tests/Phase1Save.Tests.csproj` / `Phase1Save.Tests.dll`.

---

## 7. Bounded Delta — Owner Rulings C3, C4, C5 Closure (Remove Clearfell + Player Stage Override)

### Owner Rulings Recorded
- **C3 — Accepted as-is:** `EconomyModelVersion` stays `Off (0)` by default everywhere; opting in (`EconomyModelVersion.Enabled`) on the frozen `Legacy` (`0/0/0/0`) base is explicitly permitted with no restriction added.
- **C4 — Accepted as-is:** Keep the `FinancialLedger: NOT RECORDED` disclosure in `MultiAreaHistoryQuery` history readouts when Economy is `Off` (alongside `EconomyHistoryQuery` returning `NOT RECORDED` and `null` cents).
- **C5 — Change applied:** CCF is a Continuous-Cover Forestry simulator; clearfelling is not allowed or modelled, and no player-controlled harvest-stage override is permitted. The `€2,500` per-plan minimum harvest job floor is preserved.

### What Was Removed
- `HarvestThinningStage.Clearfell`, `EconomyDefaults.ClearfellCentsPerTonne`, its `HarvestRateCentsPerTonne` switch arm, and all clearfell wording/labels in `EconomyModels.cs` (`EconomyEvidence.HarvestPricing` updated to selective CCF thinning only).
- `MultiAreaContinuation.SetHarvestStageOverride` (`MultiAreaContinuation.Commands.Economy.cs`).
- `EconomyApprovedQuote.HarvestStageOverride`, `MultiAreaEconomyState.PendingHarvestStageOverride`, `MultiAreaEconomyView.PendingHarvestStageOverride` (`EconomyRecords.cs`), and every constructor/call/reset site in `MultiAreaContinuation.Commands.Plan.cs`, `MultiAreaContinuation.Commands.Economy.cs`, and `EconomyQuoteCalculator.cs` (`ComputePending`, `ComputeForApproved`, `ComputeCore`, `EconomyAnnualSummary.BuildYearEnd`).
- All test assertions exercising `SetHarvestStageOverride` or `Clearfell` in `tests/Core.Tests/Phase2BEconomyTests.cs` (replaced with reflection/enum boundary checks and pure auto-derivation checks).

### Authoritative Stage Derivation Rule
- Derived strictly by `EconomyDefaults.AutoHarvestStage(econ.ExecutedHarvestPlansCount)` from authoritative recorded executed felling history on the stand, with zero caller input:
  - `0` prior executed harvest plans → `HarvestThinningStage.FirstThinning` (`2_000L` cents/t = `€20/t`)
  - `1` prior executed harvest plan → `HarvestThinningStage.SecondThinning` (`2_300L` cents/t = `€23/t`)
  - `>= 2` prior executed harvest plans → `HarvestThinningStage.LaterThinning` (`2_000L` cents/t = `€20/t`)
- The `€2,500` (`250_000L` cents) minimum harvest job floor per executed felling Work Plan is preserved unchanged.

### Numeric Rate Reconciliation Note (For Owner Confirmation at Integration)
- The C5 ruling's illustrative parenthetical mentions `"€23/t first / €19/t subsequent"`, which conflicts with **both** the existing Phase 2B-1 code and the verified 3D sources (`ScenarioOneEconomyAdapter.cs` / `Stage1EconomyDefaults.cs`). The same ruling's operative clause instructs keeping "the exact non-clearfell CCF thinning tiers already verified from 3D".
- Following the operative clause, this delta preserves the existing 3D-aligned non-clearfell tier constants unchanged (`FirstThinning = €20/t`, `SecondThinning = €23/t`, `LaterThinning = €20/t`) and records this discrepancy here and in `specs/multi-2d-phase2b-economy-v1/spec.md` §7 for explicit owner confirmation at integration.
