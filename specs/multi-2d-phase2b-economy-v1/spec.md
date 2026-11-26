# CCF Multi 2D — Phase 2B-1 Economy, Labour & D-062 Timber Engine — Specification

- **Spec ID:** `SPEC-multi-2d-phase2b-economy-V1`
- **Feature ID:** `multi-2d-phase2b-economy-v1`
- **Status:** `IMPLEMENTING` (Owner approved for Phase 2B-1 build + verification on feature branch)
- **Base Commit:** `5648813d9ccbe8d38e9fbe5df38d62c810af4a22` (`origin/integration/2d-sim`, S0b tip)
- **Branch:** `feat/multi-2d-phase2b-economy-core-20261126-v1`
- **Normative Upstream Contracts:** `specs/multi-2d-s0-seam-extraction-v1/frozen-contracts-s0b.md` (§1 C-V3, §2 C-ECON, §3 C-FENCE, §4 integrator checklist, §5 ownership), `Docs/Project/decision-log.md` (D-056, D-057, D-059, D-062, D-073).
- **Scope:** Pure Core engine (`Tools/MultiRackSitkaP1Lab/src/Core/`) and minimal targeted Core tests (`tests/Core.Tests/Phase2BEconomyTests.cs`). Zero UI/HTTP edits, zero Save v2 (`src/Core/Phase1/**`) edits, zero Unity `Assets/` edits.

---

## 1. Opt-In Switch and Off-Inertness

- `EconomyModelVersion { Off = 0, Enabled = 1 }` defaults to **`Off`** on `MultiAreaModelSelection`.
- When `Economy == EconomyModelVersion.Off`:
  - `MultiAreaRoot.Economy` is strictly `null`.
  - `MultiAreaSourceToken.AreaModelVersion` is unchanged (no `+economy-v1` suffix), keeping all 9/9 frozen SHA-256 anchors and Save v2 golden bytes byte-identical.
  - Zero financial state is allocated, mutated, or settled across Work Plan commands or `Advance`.
  - `ReservedPhase2BYearEndSummaryStep` executes as a no-op (`Start.Economy is null`).
  - History money fields read **`NOT RECORDED`** (`HistoryProvenance.NotRecorded`, numeric cents `null`) — **never `€0`** or `0`.
  - Save v2 (`CaptureSave` / `Restore`) operates unchanged and byte-identically.
- When `Economy == EconomyModelVersion.Enabled`:
  - `MultiAreaSourceToken.AreaModelVersion` appends `"+economy-v1"`.
  - `MultiAreaRoot.Economy` holds an immutable `MultiAreaEconomyState` initialized at genesis.

## 2. Locked Economic, Labour, Material & Harvest Rules (Integer Cents)

All monetary quantities use checked `long` integer euro cents (ex VAT). No floating-point currency arithmetic is permitted.

| Rule / Parameter | Value | Evidence Label | Notes / Provenance |
|---|---|---|---|
| Opening cash (0.64 ha calibration stand) | `€12,000` (`1,200,000` cents) | `GAMEPLAY CALIBRATION` | Aligned with 3D Enlarged80 `ScenarioOneDefinition.StartingCashCents` (D-056/D-057). |
| Upfront cash / no debt | `CashBalanceCents >= UpfrontCashRequiredCents` before Approve and Execute | `SIMULATION ABSTRACTION` | Expected timber sale revenue never pre-finances contractor or material costs. Balance never negative; no overdraft or debt. |
| Annual landowner allowance | `40.0 h/year` (`2,400` min/year) | `GAMEPLAY CALIBRATION` | Physical resource, `€0` cash wage (`SIMULATION ABSTRACTION`). Depletes on Execute; resets on annual `Advance` via `ReservedPhase2BYearEndSummaryStep`. |
| Landowner-eligible actions | Low-risk `Plant`, `Shelter`, `Prune` (≤ 6.5 m target), `ClearUndergrowth`, `SpotClear` | `MANAGEMENT GUIDANCE` / `GAMEPLAY CALIBRATION` | Eligibility reflects low-risk manual tasks; `Fell` is never eligible. |
| `Fell` execution method | **Contractor-only** | `MANAGEMENT GUIDANCE` | Commercial production felling is not ordinary owner DIY. Assigning `Fell` to `Landowner` is refused with typed code `EconomyFellContractorOnly`. |
| Non-harvest contractor labour | `€45.00/hr` (`4,500` cents/hr) | `GAMEPLAY CALIBRATION` | Pro-rated by task minutes (`RoundRatio(minutes, 4500, 60)`). No national Irish manual tariff claimed. |
| Tree shelter (on demand) | `€5.00` (`500` cents) | `GAMEPLAY CALIBRATION` | Purchased on demand in the Work Plan (no nursery/shed inventory). Not the €2.56 grant allowance. |
| Beech sapling (on demand) | `€4.50` (`450` cents) | `GAMEPLAY CALIBRATION` | Purchased on demand in the Work Plan; aligned with `ScenarioOneDefinition`. |
| Oak sapling (on demand) | `€5.50` (`550` cents) | `GAMEPLAY CALIBRATION` | Purchased on demand in the Work Plan; aligned with `ScenarioOneDefinition`. |
| Grouped harvest pricing by stage | First thinning `€20/t`, second `€23/t`, later `€20/t`, clearfell `€15/t` (range `€15–€23/t`) | `EMPIRICAL` (IFA combined harvest+forward range) / `GAMEPLAY CALIBRATION` (stage selection) | One grouped contractor job per executed felling Work Plan. Combined harvest + forwarding rate. |
| Harvest minimum job floor | `€2,500` (`250,000` cents) per executed felling Work Plan | `GAMEPLAY CALIBRATION` | Floor (`max(250000, variable)`), not an additive mobilisation surcharge. Two separate felling plans in the same year each incur their own floor. |
| Fresh-volume work-equivalent proxy | `890 kg/m³` | `EMPIRICAL` (Sitka mill-delivery anchor) / `GAMEPLAY CALIBRATION` (broadleaf unit convention) | Unit convention for harvest work mass and broadleaf `€/m³ → €/t` conversion; **not** a claimed Oak or Beech green density. |

### Non-Harvest Task Durations (Conservative Recovery)
- `Plant`: `10 min/sapling` (`GAMEPLAY CALIBRATION`, recovered from `ScenarioOneDefinition.cs`).
- `Shelter`: `60 min / 45 shelters` (`CeilRatio(count, 60, 45)`, `GAMEPLAY CALIBRATION`, recovered from `Stage1EconomyDefaults.cs`).
- `Prune`: `4 min/tree` (`60 min / 15 trees`, `GAMEPLAY CALIBRATION`, recovered from `Stage1EconomyDefaults.cs`).
- `ClearUndergrowth`: `12 min/cell` (2.5 m / 6.25 m² cell, `GAMEPLAY CALIBRATION`, conservative 2D choice).
- `SpotClear`: `4 min/patch` (1 m² patch, `GAMEPLAY CALIBRATION`, conservative 2D choice).

## 3. C-ECON Lifecycle: `Pending → Quote → Approve → Execute → Settle`

1. **Quote (`QuoteWorkPlan`):** Pure, read-only calculation over pending orders and committed `MultiAreaRoot`, bound to `PlanStateRevision` and `MultiAreaSourceToken`. Zero state mutation; repeated calls on the same root/revision produce identical results.
2. **Approve (`Approve`):** Validates quote eligibility (contractor-only Fell, upfront cash cover without pre-financing from expected revenue, landowner allowance cover) under `sync` and freezes an immutable `EconomyApprovedQuote` into `root.Economy` inside the same single `root` assignment. Later re-quotes cannot mutate the frozen approval; edits to pending executors or harvest stage are refused while approved (`WorkPlanAlreadyApproved`).
3. **Cancel (`CancelApproved`):** Clears `ApprovedQuote = null` atomically with `Approved = null`.
4. **Execute & Settle (`Execute`):** Revalidates all orders and economic preconditions all-or-nothing under the **same `sync` lock acquisition** as physical forestry work. Settles cash and landowner minutes and appends ledger/settlement records inside the **same single `root` assignment**.
5. **Idempotency & Replay:** Keyed by the engine-minted execution key (`planId + ":execute"`). Replaying `Execute` with the same key, plan ID, and original source returns the stored receipt **including** its `EconomySettlementReceipt` and never re-settles or modifies cash/allowance. Key conflicts are refused (`WorkExecutionKeyConflict`).
6. **Annual Boundary (`ReservedPhase2BYearEndSummaryStep`):** The sole annual hook. `AnnualPipeline.Steps` order and length are untouched. On `Advance`, when `Start.Economy is not null`, appends an immutable `EconomyYearEndSummary`, resets `RemainingLandownerMinutes` to `2400` (`40.0 h`), resets `UsedLandownerMinutesThisYear` to `0`, and clears cancelled pending/approved economy state on consented cancellation.

## 4. D-062 Timber Assortment & Pricing (Ported Semantics)

Semantics are ported cleanly into pure .NET without importing Unity files.
- **Stem taper & volume budget (`SIMULATION ABSTRACTION`):** DBH-anchored paraboloid (`BreastHeightMm = 1300`, `StumpHeightMm = 350`, `CuttingLossMm = 5`, `UnallocatedScanStepMm = 100`) over `FallenDeadwoodRecord.StemVolumeM3(dbhCm, heightM)` floored to `cm³`. Strict material conservation: `SaleVolumeCm3 + ResidualVolumeCm3 == StemVolumeCm3`.
- **Quality bridge (`GAMEPLAY CALIBRATION`):** Recorded pruning history (`Stage4.PruningOf(treeId)`) yields a `Pruned` quality section `[0, round(CrownBaseHeightM * 1000))` when `Lifts > 0`. Pruning affects assortment eligibility only — never unit price multipliers.
- **Species-explicit ladders:**
  - **Sitka spruce (`sitka-spruce`):** `Sawlog` (≥200 mm small end, 4.9 m; `€95/t` [range `€88–105/t` `EMPIRICAL`, selected `GAMEPLAY CALIBRATION`]) → `Pallet` (≥140 mm, 3.7/3.4/3.1/2.5 m; `€68/t`) → `Stake` (70–130 mm, 3.7/3.4 m; `€47/t`) → `Pulp` (≥70 mm, 3.0 m; `€38/t`). Sitka stems see only Sitka rules.
  - **Sessile Oak (`sessile-oak`):** `VeneerPlanking` (requires `Pruned`, small ≥400 mm, mid ≥450 mm, 2.8 m; `€230/m³` ref [range `€180–280/m³` `MANAGEMENT GUIDANCE`, ref & roadside `GAMEPLAY CALIBRATION`] → `€258/t`) → `Beam` (small ≥250 mm, mid ≥300 mm, 2.6 m; `€145/m³` → `€163/t`) → `Fencing` (small 120–250 mm, 2.4/1.8 m; `€100/m³` → `€112/t`) → `SecondPlanking` (small ≥250 mm, 2.4 m; `€60/m³` → `€67/t`) → `Firewood` (small ≥70 mm, 2.0 m; `€30/m³` [range `€20–40/m³` `GAMEPLAY CALIBRATION`] → `€34/t`).
  - **European Beech (`beech`):** `SecondPlanking` (small ≥250 mm, 2.4 m; `€60/m³` → `€67/t`) → `Firewood` (small ≥70 mm, 2.0 m; `€30/m³` → `€34/t`) **only**. Beech never inherits Oak's `VeneerPlanking`, `Beam`, or `Fencing` classes.
- **Proxy conversion:** `CentsPerTonneAtProxy(centsPerM3) = round((centsPerM3 * 1000 / 890) / 100, AwayFromZero) * 100`. No Irish broadleaf price series is claimed.

## 5. 0.64 ha Calibration Stand Fixture

- Deterministic `80 × 80 m` (`0.64 ha`) five-lane (`MA-L1`..`MA-L5`, each `16 × 80 m = 0.128 ha`) fixture with `1,344` stems (`2,100 stems/ha`, `269 × 4 + 268`), pure integer SplitMix64 generation, opening cash `€12,000` (`GAMEPLAY CALIBRATION`).

## 6. Persistence (Deferred to Phase 2A C-V3)

- Save v2 (`src/Core/Phase1/**`, `SchemaVersion = 2`) remains frozen read-only. Zero `IFeatureBlockCodec` implementations ship in Phase 2B-1.
- When `Economy == Enabled`, `CaptureSave`, `SaveExpectation`, and `Restore` refuse fail-closed with typed code **`SaveEconomyRequiresV3`** so economy state is never silently dropped.
- `RequireSameOptions` includes `SameEconomy` so an Economy-enabled session can never continue from a v2 save.
