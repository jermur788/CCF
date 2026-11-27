# Phase 2B-2b ENGINE HALF: Three-Way Felling Dispositions (handoff)

**Date:** 2026-11-27 · **Role:** implementer (Claude Opus 5.5) · **Status:** HANDED OFF, NOT MERGED, NOT REVIEWED

| Item | Value |
|---|---|
| Approved base | `origin/integration/2d-sim` = `fa69d6d3d8c1789af531b53adefb5f426e90bbed` (checked before branching; still equal at the fence gate) |
| Feature branch | `feat/multi-2d-phase2b2b-felling-dispositions-engine-20261127-v1` |
| Engine commit | `1a1ee151`: `feat(economy): Phase 2B-2b felling dispositions engine (Sell/Keep/Leave)` |
| Test commit | `fcd7541f`: `test(economy): standalone 18-case felling-dispositions runner` |
| Review branch (not created by me) | `review/multi-2d-phase2b2b-felling-dispositions-engine-20261127-v1` |
| Unity | NOT RUN. This lane is pure .NET and offline. |

## 1. Objective

Add three felling dispositions to the pure .NET Phase 2B Economy engine: **SellAndExtract** (default, unchanged behaviour), **KeepForUse** and **LeaveAsDeadwood**. Each one is chosen per Fell order. The engine computes a retained-timber balance that it alone controls (no player override), and it emits deadwood intents. The UI, saves and Stage 4 deadwood are not touched.

## 2. `git diff --stat origin/integration/2d-sim...HEAD` (code commits)

```
 .../Dispositions/DispositionsAssemblyInfo.cs       |   5 +
 .../Economy/Dispositions/FelledStemDeadwood.cs     |  64 ++
 .../Economy/Dispositions/FellingDisposition.cs     | 124 ++++
 .../Economy/Dispositions/RetainedTimber.cs         |  92 +++
 .../MultiArea/Economy/EconomyQuoteCalculator.cs    | 102 ++-
 .../src/Core/MultiArea/Economy/EconomyRecords.cs   |  61 +-
 .../MultiAreaContinuation.Commands.Economy.cs      |  60 +-
 .../Core/EconomyDispositions/DispositionTests.cs   | 691 +++++++++++++++++++++
 .../tests/Core/EconomyDispositions/Program.cs      |   5 +
 .../SitkaP1.EconomyDispositions.Tests.csproj       |  16 +
 .../tests/Core/EconomyDispositions/TestRunner.cs   |  41 ++
 11 files changed, 1245 insertions(+), 16 deletions(-)
```

The handoff commit adds this file as well.

## 3. File-by-file changes

All paths are under `Tools/MultiRackSitkaP1Lab/`.

**New: `src/Core/MultiArea/Economy/Dispositions/`**
- `DispositionsAssemblyInfo.cs`: `[assembly: InternalsVisibleTo("SitkaP1.EconomyDispositions.Tests")]`. `SitkaP1.Core.csproj` is untouched.
- `FellingDisposition.cs` contains:
  - the `FellingDisposition` enum (0/1/2);
  - `FellingDispositionChoice(Order, Disposition)`;
  - the per-stem record `FelledStemDispositionLine`;
  - the evidence strings in `FellingDispositionEvidence`;
  - `FellingDispositionRules`: `Resolve`, `ExplicitFor`, `SameResolved` and `Of`.
- `RetainedTimber.cs`: `RetainedTimberStem`, `RetainedTimberAmount` and `RetainedTimberTally` (`FromStems`, `Plus`, `StampedWith`), plus the exact helper `RetainedTimber.ToM3`.
- `FelledStemDeadwood.cs`: `FelledStemDeadwoodIntent`, and the static `FelledStemDeadwood` with `IntentFor` and the pure `Materialise(intents, nextDeadwoodNumber)`.

**Changed existing files (inside the fence).** No positional parameter was added or removed; every new field is an `init` property with an empty or zero default. The 16 removed lines are of two kinds. Some are record terminators `);` that became `)` followed by `{ … }` so an init-property body could be attached. The others were rewritten inside method bodies, as listed below.
- `EconomyRecords.cs` gains these init properties:
  - `EconomyHarvestQuote`: `StemDispositions`, `RetainedStemVolumeCm3`, `DeadwoodStemVolumeCm3`, `RetainedTimber` and `DeadwoodIntents`.
  - `EconomyWorkPlanQuote`: `FellDispositions`, the resolved map covering every Fell order.
  - `EconomyApprovedQuote`: a computed getter `FrozenFellDispositions => Quote.FellDispositions`.
  - `EconomySettlementReceipt`: `RetainedTimberDelta` and `DeadwoodIntents`.
  - `MultiAreaEconomyState`: `PendingFellDispositions` and `RetainedTimber`.
  - `MultiAreaEconomyView`: `PendingFellDispositions` (explicit choices only) and `RetainedTimber`.
- `EconomyQuoteCalculator.cs`:
  - `ComputePending` and `ComputeForApproved` now pass a disposition map. The pending map is resolved against `root.Pending`; the approved path uses the frozen map.
  - `ComputeCore` has a new parameter, and its stem loop splits the economics by disposition.
  - `BuildSettlement` runs an additional disposition drift check. It stamps the retained delta and the intents onto the receipt, adds the delta to the cumulative balance, and clears the pending choices.
  - `EconomyAnnualSummary.BuildYearEnd` clears pending choices when work is cancelled.
- `MultiAreaContinuation.Commands.Economy.cs`:
  - new `SetFellingDisposition(fence, areaId, treeId, disposition)`;
  - `ReadEconomyView` fills the two new view properties;
  - `WithoutPendingExecutor`, which `RemovePending` already calls, now also drops that order's disposition.

**New: `tests/Core/EconomyDispositions/`**: `SitkaP1.EconomyDispositions.Tests.csproj`, `Program.cs`, `TestRunner.cs` and `DispositionTests.cs`.

## 4. Disposition rules as implemented

| Rule | SellAndExtract (0, default) | KeepForUse (1) | LeaveAsDeadwood (2) |
|---|---|---|---|
| Contractor felling and extraction cost | grouped job, engine stage rate, one €2,500 floor | **identical** | **identical** |
| Green mass used for the cost | full stem | full stem | full stem |
| Timber revenue and `TimberSale` lines | sum of `D062StemYield.TimberRevenueCents` | 0, no line | 0, no line |
| `SaleVolumeCm3` / `ResidualVolumeCm3` / `Assortments` | this stem's | not included | not included |
| Retained timber | 0 | `D062StemYield.StemVolumeCm3` | 0 |
| Deadwood intents | 0 | 0 | exactly 1 |

- A felling Work Plan is still **one** grouped contractor job. Mixed dispositions inside one plan are legal, and the plan still pays one floor and uses one stage rate.
- Volume is conserved: `TotalStemVolumeCm3 == Sale + Residual + RetainedStemVolumeCm3 + DeadwoodStemVolumeCm3`. This is tested.
- Stage rates are unchanged: first plan 2,100 c/t, second 2,300 c/t, later 2,000 c/t, chosen by `ExecutedHarvestPlansCount`.
- There is **no fell-only rate**. A cheaper rate for felling without extraction would be a future calibration decision, and I did not guess one.
- The guards are unchanged:
  - Fell can only be done by the contractor (`EconomyFellContractorOnly`).
  - `WorkWouldLeaveNoLivingStems` still refuses a plan that fells a whole area, even when every stem is set to Keep or Leave. This is tested on all 73 MA-B stems.
  - No command takes a rate, volume or price.
- **Command `SetFellingDisposition`**, refusals in check order:
  1. `PlayerPlanStateStale` (fence)
  2. `EconomyModelRequired` (Economy Off)
  3. `EconomyDispositionUnknown` (undefined enum value)
  4. `WorkPlanAlreadyApproved`
  5. `UnknownWorkOrder` (no pending Fell order for that area and tree)

  A successful call bumps `PlanStateRevision`. Choosing `SellAndExtract` deletes the stored choice, so the map only ever holds explicit non-default choices.
- **Which order a choice belongs to.** A choice is tied to the exact pending `MultiAreaWorkOrder` instance (reference identity). It is cleared in all of these cases:
  - `RemovePending` explicitly drops it.
  - `CancelApproved` drops the orders, so the choice no longer matches any order and is never resurrected if the tree is re-added. This is tested.
  - A consented year-end cancellation clears it.
  - Settlement clears it.
- **Freezing.** Approve is outside the fence and was not edited. It already stores `ComputePending(...)` as the frozen quote, and that quote now carries `FellDispositions`, so the frozen map comes along automatically, just like `FrozenOrderExecutors`. `ComputeForApproved` reprices using the frozen map. `BuildSettlement` refuses with `EconomyQuoteDrift` if any of these differ from the frozen values:
  - the live choices resolved against `approvedPlan.Orders`;
  - the recomputed resolved map;
  - the retained cm³;
  - the number of deadwood intents.

  Tested: after Approve, removing the oak's Keep choice or changing it to Leave through the root makes Execute refuse with no cash moving. Restoring the root lets Execute settle the frozen amounts.

## 5. Retained-timber contract

- **Unit basis:** `long` cubic centimetres, summed from `D062StemYield.StemVolumeCm3`. That value comes from `D062TimberAssortmentEngine.GradeStem` on the live tree, with its live species and pruning, inside `ComputeCore`.
- **Exposed fields:** `RetainedTimberCm3` (long), `RetainedTimberM3` (decimal, `cm3 / 1_000_000m`, exact for every long), and `BySpecies`. `BySpecies` is a `SortedDictionary` with ordinal species-id keys, holding `RetainedTimberAmount(SpeciesId, RetainedTimberCm3)` with its own `RetainedTimberM3`. `Stems` is the per-stem list, carrying the execution identity.
- **Where it lives:**
  - `EconomyHarvestQuote.RetainedTimber` is the expected delta.
  - `EconomySettlementReceipt.RetainedTimberDelta` is the delta for each settlement, stamped with year, work revision, execution key and plan id.
  - `MultiAreaEconomyState.RetainedTimber` and `MultiAreaEconomyView.RetainedTimber` hold the cumulative balance, which starts empty.
- **Why no override is possible:**
  - No command, constructor path or option accepts a volume.
  - `SetFellingDisposition` takes only the fence, area, tree and disposition; a reflection test asserts its exact signature and that no felling, disposition, executor or quote method takes a numeric parameter.
  - Tallies are only built inside the engine, by `FromStems` from graded stems.
- **Not cash:** retained timber never produces a cash delta and never a ledger line, of any category. The cash reconciliation `sum(ledger) == Net` is unchanged and is tested for every disposition and for the mixed plan.
- **Not persisted:** saving with Economy enabled still refuses with `SaveEconomyRequiresV3`. No Version 3 or codec work was done.

## 6. Deadwood-intent contract and the single follow-up wiring step

- `FelledStemDeadwoodIntent(AreaId, TreeId, SpeciesId, XM, ZM, DbhCm, HeightM, VolumeM3, EcologicalYear, WorkRevision, ExecutionKey, PlanId)`.
  - `VolumeM3` is `FallenDeadwoodRecord.StemVolumeM3(DbhCm, HeightM)` as float32, the same expression that natural deaths use. The tests check it bit for bit.
  - Intents are ordered by area, then tree.
  - The settlement receipt carries them stamped with the committed identity. The intents on the quote carry the identity from quote time, which is empty before approval.
- `FelledStemDeadwood.Materialise(intents, nextDeadwoodNumber)` is pure and returns `(records, nextNumber)`.
  - Ids are `DW####`, numbered sequentially from the supplied number.
  - `Cause` is `FellCause.Harvest` (`"fell"`), and `DeathYear` is the intent's year.
  - It refuses a repeated (area, tree) pair and any number below 1.
- **Not appended in this half.** `MultiAreaStage4State` is unchanged, and the tests assert that `Stage4.Deadwood.Count` does not change on Execute. Extracted and kept stems never create intents. A felled tree is marked dead with the `fell` cause, which `AnnualPipeline` never turns into deadwood, so materialising later cannot duplicate a record.
- **Follow-up wiring step (one edit, in `MultiAreaContinuation.Commands.Plan.cs` `Execute`, after `BuildSettlement`):**
  ```csharp
  if (settlement is { DeadwoodIntents.Count: > 0 } && nextStage4 is not null)
  {
      var (records, next) = FelledStemDeadwood.Materialise(settlement.DeadwoodIntents, nextStage4.NextDeadwoodNumber);
      nextStage4 = nextStage4 with
      {
          Deadwood = Array.AsReadOnly(nextStage4.Deadwood.Concat(records).ToArray()),
          NextDeadwoodNumber = next,
      };
  }
  ```
  Two things must be decided before that wiring can be done:
  1. Whether felled deadwood belongs in the Phase 1 save validator rule `NextDeadwoodNumber == Deadwood.Count + 1`. That rule only bites if felled deadwood is ever saved; saving is blocked today while Economy is on.
  2. Whether Stage 4 can be null in Economy sessions. It is not null in the mixed demo.

## 7. Evidence labels

| Rule | Label |
|---|---|
| Same contractor job, rate and floor for all dispositions; no fell-only rate | SIMULATION ABSTRACTION / GAMEPLAY CALIBRATION |
| Stage rates €21/€23/€20 per t and the €2,500 floor (unchanged) | EMPIRICAL range / GAMEPLAY CALIBRATION selection (Phase 2B-1) |
| Keep-for-use earns no cash; its volume is the D-062 graded stem volume | SIMULATION ABSTRACTION |
| The D-062 stem-volume form (`dbh² × 0.00007854 × h × 0.5`) | SIMULATION ABSTRACTION (Unity biological stem volume; the 0.5 form factor is the Sitka ratio applied to every species) |
| A felled stem left on site becomes fallen deadwood | MANAGEMENT GUIDANCE (retaining deadwood) / SIMULATION ABSTRACTION (no decay, habitat score or value) |
| Felling stays contractor-only; no clearfell; no whole-area felling | MANAGEMENT GUIDANCE |
| A choice is tied to its order instance; freezing at Approve; drift refuses | SIMULATION ABSTRACTION (engine contract) |

These evidence strings are in `FellingDispositionEvidence.All`. They are **not** added to `EconomyEvidence.All`, because changing that list would change every quote's `EvidenceLabels`.

## 8. Verification (actually run, 2026-11-27, on this machine)

```
cd Tools/MultiRackSitkaP1Lab
bash build/build-p0-core-ref.sh   → P0 Core reference built … sha256=9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3
dotnet --version                  → 8.0.133
dotnet restore tests/Core/EconomyDispositions/SitkaP1.EconomyDispositions.Tests.csproj  → All projects are up-to-date for restore.
dotnet build   tests/Core/EconomyDispositions/SitkaP1.EconomyDispositions.Tests.csproj --no-restore → 0 Warning(s) 0 Error(s)
dotnet run --project tests/Core/EconomyDispositions/SitkaP1.EconomyDispositions.Tests.csproj --no-build --no-restore
                                  → 18/18 passed, 0 failed (exit 0)
dotnet build tests/Core.Tests/SitkaP1.Core.Tests.csproj → 0 Warning(s) 0 Error(s)   (build only; suite NOT run)
```

**Determinism hash** (SHA-256 of the canonical mixed-plan quote and settlement text, excluding session ids and execution keys): `d9c7aeddf2eb629b388c6cc3dfa93d5a55bd27c85e6262b323ddcbe6f14498ed`.
- It is the same across 3 repeated quotes and 2 fresh sessions.
- It is pinned in the test, so a different process giving a different result would fail.
- It was pinned from the first observed run.

**Mutation checks** (temporary, reverted, and confirmed byte-identical with `cmp`):
- Letting KeepForUse credit revenue: 10 cases fail.
- Disabling the live-disposition drift check: case 12 fails.
- Dropping LeaveAsDeadwood mass from the contractor basis: cases 9, 11 and 15 fail.

The 18 cases are the 16 required ones plus #17 (refusal codes, stale fence, removal clears the choice) and #18 (a consented year-end cancellation clears choices).

## 9. Pinned SellAndExtract integers (zero drift)

Fixture: the `ma-1-mixed` demo with Economy enabled, felling MA-B-001 (Sitka spruce), MA-B-002 (sessile oak) and MA-B-003 (beech) as one plan, with the default €12,000 opening cash.

I captured these with a throwaway probe on the **unmodified base engine** (`fa69d6d3` plus only the attribute-only `InternalsVisibleTo` file), before any engine change. Test #2 reproduces them for both an implicit and an explicit SellAndExtract.

| Field | Value |
|---|---|
| `TimberSaleRevenueCents` | 15,727 |
| `TotalExternalCostCents` / `UpfrontCashRequiredCents` | 250,000 |
| `NetCashDeltaCents` | −234,273 |
| Owner minutes | 0 |
| Stage / rate | FirstThinning / 2,100 c/t |
| `TotalStemVolumeCm3` / sale / residual | 2,389,104 / 2,304,305 / 84,799 |
| `HarvestedGreenGrams` | 2,126,303 |
| Variable harvest / floor adjustment / contractor total | 4,465 / 245,535 / 250,000 |
| Ledger (9 lines, in order) | HarvestingAndForwarding −4,465; MinimumJobAdjustment −245,535; TimberSale beech:SecondPlanking 1,519; beech:Firewood 1,458; sessile-oak:Fencing 2,438; sessile-oak:Firewood 93; sitka-spruce:Stake 166; sitka-spruce:Pallet 1,367; sitka-spruce:Sawlog 8,686 |
| Settled `CashAfterCents` | 965,727 |
| Per-stem `StemVolumeCm3` (Sitka / oak / beech) | 1,330,285 / 292,613 / 766,206 (Sitka revenue 10,219) |

## 10. Not verified

- **Unity:** not run, and not needed by this pure .NET lane.
- **The full `tests/Core.Tests` suite:** not run, as the packet instructed. It was only **built**. In particular, the existing 9 `Phase2BEconomyTests` cases and the 9 frozen simulation anchors were not executed in this lane. My reasoning that they are unaffected:
  - I did not touch `AnnualPipeline.cs`, `RngSalts.cs` or any digest.
  - Every new property is an init property with an empty default.
  - The SellAndExtract quote is shown identical to the base by test #2.
  - New member names avoid the reflection bans in Phase2BEconomyTests case 9 (`StageOverride`, `HarvestStage`, `Clearf`).

  This is reasoning, not a run.
- **The 2D gate, browser suites and the Phase 2B-2a UI:** not run. The UI can't set dispositions yet; `src/Ui/**` is untouched.
- **Deadwood wiring into Stage 4:** not implemented, by design (section 6).

## 11. Residual risks and notes

1. **Reference identity.** A disposition choice is tied to its order by reference. If a future code path rebuilds `root.Pending` with new instances, for example a restore path once Version 3 saves exist, the choices would quietly fall back to SellAndExtract. That is safe, but it would surprise the player. Version 3 save work must persist the choices explicitly.
2. **After Approve, the choices stay on the state.** Approve is outside the fence and was not edited, so it does not clear `PendingFellDispositions`. The choices still point at the now-approved order instances, which is what the live drift check compares against. They are cleared at settlement, never match anything after `CancelApproved`, and are cleared on a consented year-end cancellation.
3. **The meaning of `SaleVolumeCm3`, `ResidualVolumeCm3` and `Assortments` has narrowed** to SellAndExtract stems only. For SellAndExtract-only plans nothing changes. A UI written for a mixed plan must not assume `Total == Sale + Residual`.
4. **`EconomyHarvestQuote.Stems` still lists every graded stem,** including kept and left ones, with their graded `TimberRevenueCents`. Only `StemDispositions[*].CreditedTimberRevenueCents` and the top-level revenue fields reflect actual sale.
5. **The ProjectReference path is deeper than the packet said.** The packet gave `../../src/Core/SitkaP1.Core.csproj`, but from `tests/Core/EconomyDispositions/` that resolves to `tests/src/…`, which does not exist. The csproj uses `../../../src/Core/SitkaP1.Core.csproj`, which is the same target the packet meant.
6. **Edits to `EconomyQuoteCalculator.cs` and `Commands.Economy.cs` are not purely added lines.** Some existing method bodies were rewritten (section 3), all inside the allowed files. No positional record shape changed.
7. **No fell-only rate.** A future calibration might make LeaveAsDeadwood cheaper than SellAndExtract, since there is no extraction. That is an open calibration decision.
