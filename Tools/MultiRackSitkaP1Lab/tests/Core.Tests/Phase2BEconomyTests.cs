using System.Reflection;
using SitkaP1.Core;
using SitkaP1.Core.Phase1;

namespace SitkaP1.Core.Tests;

/// <summary>
/// CCF Multi 2D — Phase 2B-1 Economy, Labour & D-062 Timber Engine verification suite (exact 8-case minimum set).
/// </summary>
internal static class Phase2BEconomyTests
{
    private static readonly FieldInfo RootField =
        typeof(MultiAreaContinuation).GetField("root", BindingFlags.NonPublic | BindingFlags.Instance)!;

    internal static void Register()
    {
        // 1. Off is inert
        T.Add("economy: Off is inert", () =>
        {
            T.Eq(EconomyModelVersion.Off, MultiAreaModelSelection.Phase2.Economy, "default Economy switch is Off");

            var (exp, p2Opts, _) = MultiAreaDemoExperiments.CreateMixedDemo();
            var session = new MultiAreaContinuation(exp, MultiAreaModelSelection.Phase2, null, null, p2Opts);
            var root0 = GetRoot(session);
            T.True(root0.Economy is null, "MultiAreaRoot.Economy is null when Economy is Off");
            T.True(!session.ReadView().Source.AreaModelVersion.Contains("+economy-", StringComparison.Ordinal),
                "Off token carries no +economy suffix");

            T.Refuses("EconomyModelRequired", () => session.ReadEconomyView());
            T.Refuses("EconomyModelRequired", () => session.QuoteWorkPlan(Fence(session.ReadView())));
            T.Refuses("EconomyModelRequired", () => new MultiAreaContinuation(exp, MultiAreaModelSelection.Phase2, null, null, p2Opts, EconomyOptions.Default));

            // Run a full Work Plan + Advance round-trip with Economy Off.
            var v = session.ReadView();
            string treeId = v.Areas[0].Trees.First(t => t.IsLiving).TreeId;
            v = session.AddPending(Fence(v), "MA-A", treeId, "Off-inertness fell").View;
            v = session.Approve(Fence(v)).View;
            var exec = session.Execute(Fence(v), v.Approved!.PlanId, v.Approved.ExecutionKey);
            T.True(exec.RefusalCode is null && exec.Execution is not null, "execute succeeded with Off");
            T.True(exec.Execution!.Settlement is null, "no settlement when Economy is Off");
            v = session.Advance(Fence(exec.View), consentToCancelWork: false).View;

            T.True(GetRoot(session).Economy is null, "Economy stays null after plan + annual advance");

            // History money fields read NOT RECORDED — never €0 or 0.
            var econHist = session.ReadEconomyHistoryYear(0);
            T.True(!econHist.EconomyActive, "history reports Economy inactive");
            T.Eq(HistoryProvenance.NotRecorded, econHist.Provenance, "history provenance is NOT RECORDED");
            T.Eq("NOT RECORDED", econHist.ProvenanceLabel, "history label is NOT RECORDED");
            T.True(econHist.OpeningCashCents is null && econHist.ClosingCashCents is null &&
                econHist.TotalExternalCostCents is null && econHist.TotalTimberRevenueCents is null &&
                econHist.NetCashDeltaCents is null && econHist.LandownerMinutesUsed is null,
                "all numeric cents fields are null, never 0");
            foreach (var display in new[]
            {
                econHist.OpeningCashDisplay, econHist.ClosingCashDisplay, econHist.TotalExternalCostDisplay,
                econHist.TotalTimberRevenueDisplay, econHist.NetCashDeltaDisplay, econHist.LandownerMinutesUsedDisplay,
            })
            {
                T.Eq("NOT RECORDED", display, "display field reads NOT RECORDED");
                T.True(!display.Contains("€0", StringComparison.Ordinal), "never fabricates €0");
            }

            var detail0 = session.ReadHistoryYear(0);
            var finDisc = detail0.NotRecordedDisclosures.FirstOrDefault(d => d.FieldKey == MultiAreaHistoryQuery.KeyFinancialLedger);
            T.True(finDisc is not null && finDisc.Provenance == HistoryProvenance.NotRecorded && finDisc.ProvenanceLabel == "NOT RECORDED",
                "HistoryYearDetail discloses FinancialLedger as NOT RECORDED");

            // Save v2 works unchanged when Off, and refuses with typed SaveEconomyRequiresV3 when Enabled.
            byte[] v2Bytes = Phase1SaveCodec.Encode(session.CaptureSave(Fence(v), "off-save", 1));
            T.True(v2Bytes.Length > 0, "v2 save succeeds when Economy is Off");
            var econSession = new MultiAreaContinuation(exp, MultiAreaModelSelection.Phase2WithEconomy, null, null, p2Opts);
            T.Refuses("SaveEconomyRequiresV3", () => econSession.CaptureSave(Fence(econSession.ReadView()), "econ-save", 1));
            T.Refuses("SaveEconomyRequiresV3", () => econSession.SaveExpectation());
            var decodedOff = Phase1SaveCodec.Decode(v2Bytes);
            T.Refuses("SaveEconomyRequiresV3", () => econSession.Restore(Fence(econSession.ReadView()), decodedOff));
        });

        // 2. Quote is pure and revision-bound
        T.Add("economy: quote is pure and revision-bound", () =>
        {
            var session = MixedEconomySession();
            var v = session.ReadView();
            string treeId = v.Areas[0].Trees.First(t => t.IsLiving).TreeId;
            v = session.AddPending(Fence(v), "MA-A", treeId, "quote purity fell").View;

            var rootBefore = GetRoot(session);
            int revBefore = v.PlanStateRevision;

            var q1 = session.QuoteWorkPlan(Fence(v));
            var q2 = session.QuoteWorkPlan(Fence(v));

            T.True(ReferenceEquals(rootBefore, GetRoot(session)), "QuoteWorkPlan mutates zero root state");
            T.Eq(revBefore, q1.PlanStateRevision, "quote bound to current PlanStateRevision");
            T.Eq(q1.PlanStateRevision, q2.PlanStateRevision, "identical PlanStateRevision");
            T.Eq(q1.TotalExternalCostCents, q2.TotalExternalCostCents, "identical external cost");
            T.Eq(q1.TimberSaleRevenueCents, q2.TimberSaleRevenueCents, "identical timber revenue");
            T.Eq(q1.NetCashDeltaCents, q2.NetCashDeltaCents, "identical net cash delta");
            T.Eq(q1.TotalOwnerMinutesRequired, q2.TotalOwnerMinutesRequired, "identical owner minutes");
            T.Eq(q1.EstimatedLedger.Count, q2.EstimatedLedger.Count, "identical ledger count");

            // Stale PlanStateRevision fence is refused.
            var staleFence = Fence(v);
            string treeId2 = v.Areas[0].Trees.Where(t => t.IsLiving && t.TreeId != treeId).First().TreeId;
            var v2 = session.AddPending(staleFence, "MA-A", treeId2, "bump revision").View;
            T.Refuses("PlayerPlanStateStale", () => session.QuoteWorkPlan(staleFence));
            var q3 = session.QuoteWorkPlan(Fence(v2));
            T.Eq(v2.PlanStateRevision, q3.PlanStateRevision, "new quote tracks new revision");
            T.Eq(2, q3.HarvestJob!.FelledStemCount, "new quote reflects updated pending orders");
        });

        // 3. Approve freezes the quote
        T.Add("economy: approve freezes the quote", () =>
        {
            var session = MixedEconomySession();
            var v = session.ReadView();
            string treeId = v.Areas[0].Trees.First(t => t.IsLiving).TreeId;
            v = session.AddPending(Fence(v), "MA-A", treeId, "freeze quote fell").View;

            var preApproveQuote = session.QuoteWorkPlan(Fence(v));
            T.True(preApproveQuote.IsEligible, "pre-approve quote eligible");

            v = session.Approve(Fence(v)).View;
            var econAfterApprove = session.ReadEconomyView();
            T.True(econAfterApprove.ApprovedQuote is not null, "approved quote frozen into Economy state");
            var frozen = econAfterApprove.ApprovedQuote!;
            T.Eq(v.Approved!.PlanId, frozen.PlanId, "frozen PlanId matches approved plan");
            T.Eq(v.Approved.ExecutionKey, frozen.ExecutionKey, "frozen ExecutionKey matches approved plan");
            T.Eq(preApproveQuote.TotalExternalCostCents, frozen.Quote.TotalExternalCostCents, "frozen cost matches pre-approve quote");
            T.Eq(preApproveQuote.TimberSaleRevenueCents, frozen.Quote.TimberSaleRevenueCents, "frozen revenue matches pre-approve quote");

            // A later re-quote on the now-empty pending queue cannot change the approved plan's frozen amount.
            var postApproveQuote = session.QuoteWorkPlan(Fence(v));
            T.Eq(0L, postApproveQuote.TotalExternalCostCents, "re-quote over empty pending queue is 0");
            var econAfterRequote = session.ReadEconomyView();
            T.Eq(preApproveQuote.TotalExternalCostCents, econAfterRequote.ApprovedQuote!.Quote.TotalExternalCostCents,
                "frozen approved quote unchanged by later re-quote");
            T.Eq(preApproveQuote.TimberSaleRevenueCents, econAfterRequote.ApprovedQuote!.Quote.TimberSaleRevenueCents,
                "frozen revenue unchanged by later re-quote");

            // Edits to harvest stage or executors are refused while approved.
            T.Refuses("WorkPlanAlreadyApproved", () => session.SetHarvestStageOverride(Fence(v), HarvestThinningStage.Clearfell));

            // Executing settles the exact frozen amounts.
            var exec = session.Execute(Fence(v), v.Approved.PlanId, v.Approved.ExecutionKey);
            T.Eq(preApproveQuote.TotalExternalCostCents, exec.Execution!.Settlement!.TotalExternalCostCents, "settled exact frozen cost");
            T.Eq(preApproveQuote.TimberSaleRevenueCents, exec.Execution.Settlement.TimberSaleRevenueCents, "settled exact frozen revenue");
        });

        // 4. Execute settles atomically and replay never re-settles
        T.Add("economy: execute settles atomically and replay never re-settles", () =>
        {
            var session = MixedEconomySession();
            var v = session.ReadView();
            string treeId = v.Areas[0].Trees.First(t => t.IsLiving).TreeId;
            v = session.AddPending(Fence(v), "MA-A", treeId, "atomic settle fell").View;
            var expectedQuote = session.QuoteWorkPlan(Fence(v));
            v = session.Approve(Fence(v)).View;

            var executeFence = Fence(v);
            string planId = v.Approved!.PlanId;
            string execKey = v.Approved.ExecutionKey;
            T.Eq(planId + ":execute", execKey, "engine-minted execution key");

            // Fault before single root publish: neither tree nor cash changes.
            T.Refuses("TestFaultBeforePublish", () => session.Execute(executeFence, planId, execKey,
                beforePublish: () => throw new P1Refusal("TestFaultBeforePublish", "simulated fault")));
            T.Eq(EconomyDefaults.DefaultOpeningCashCents, session.ReadEconomyView().CashBalanceCents,
                "cash untouched after pre-publish fault");
            T.True(session.ReadView().Areas[0].Trees.Single(t => t.TreeId == treeId).IsLiving,
                "tree untouched after pre-publish fault");

            // Real first Execute: work and money settle together atomically.
            var first = session.Execute(executeFence, planId, execKey);
            T.True(first.RefusalCode is null && first.Execution is not null, "first execute succeeded");
            T.True(!first.Execution!.Replayed, "first execute is not a replay");
            var s1 = first.Execution.Settlement!;
            long expectedCashAfter = EconomyDefaults.DefaultOpeningCashCents - expectedQuote.TotalExternalCostCents + expectedQuote.TimberSaleRevenueCents;
            T.Eq(EconomyDefaults.DefaultOpeningCashCents, s1.CashBeforeCents, "cash before = €12,000");
            T.Eq(expectedCashAfter, s1.CashAfterCents, "cash after matches quote");
            T.Eq(expectedCashAfter, session.ReadEconomyView().CashBalanceCents, "committed cash matches settlement");
            T.Eq(1, session.ReadEconomyView().Settlements.Count, "exactly one settlement recorded");

            // Replay with the SAME execution key, planId and original source returns the stored receipt and never re-settles.
            var replay = session.Execute(executeFence, planId, execKey);
            T.True(replay.RefusalCode is null && replay.Execution is not null, "replay succeeded");
            T.True(replay.Execution!.Replayed, "replay flag true");
            T.Eq(s1, replay.Execution.Settlement, "replay returns identical stored settlement receipt");
            T.Eq(expectedCashAfter, session.ReadEconomyView().CashBalanceCents, "cash moved exactly once");
            T.Eq(1, session.ReadEconomyView().Settlements.Count, "still exactly one settlement recorded");

            // Key conflict (same key, different planId) is refused.
            T.Refuses("WorkExecutionKeyConflict", () => session.Execute(executeFence, planId + "-other", execKey));
        });

        // 5. Upfront cash and no debt
        T.Add("economy: upfront cash and no debt", () =>
        {
            // Give the stand €2,000 (< €2,500 harvest floor) even if timber revenue from a big harvest would exceed €500!
            var lowCashOpts = EconomyOptions.Default with { OpeningCashCents = 200_000L };
            var (exp, p2Opts, _) = MultiAreaDemoExperiments.CreateMixedDemo();
            var session = new MultiAreaContinuation(exp, MultiAreaModelSelection.Phase2WithEconomy, null, null, p2Opts, lowCashOpts);

            var v = session.ReadView();
            // Queue several large trees so expected timber revenue is substantial (> €500), yet upfront cash is €2,000 < €2,500.
            var largeTrees = v.Areas[1].Trees.Where(t => t.IsLiving).OrderByDescending(t => t.DbhCm).Take(12).ToArray();
            foreach (var tree in largeTrees)
                v = session.AddPending(Fence(v), "MA-B", tree.TreeId, "upfront cash test").View;

            var quote = session.QuoteWorkPlan(Fence(v));
            T.True(quote.TimberSaleRevenueCents > 50_000L, "expected timber revenue exceeds the €500 cash shortfall");
            T.True(quote.UpfrontCashRequiredCents >= 250_000L, "upfront cost is at least €2,500 floor");
            T.True(!quote.IsEligible, "quote ineligible because expected timber revenue cannot pre-finance work");
            T.True(quote.Issues.Any(i => i.Code == "EconomyInsufficientCash"), "quote reports EconomyInsufficientCash");

            var rootBeforeRefusal = GetRoot(session);
            T.Refuses("EconomyInsufficientCash", () => session.Approve(Fence(v)));
            T.True(ReferenceEquals(rootBeforeRefusal, GetRoot(session)), "insufficient cash refusal mutates nothing");
            T.Eq(200_000L, session.ReadEconomyView().CashBalanceCents, "balance unchanged and non-negative");
            T.True(session.ReadEconomyView().CashBalanceCents >= 0L, "balance never negative");

            // Also verify zero-cash refuses a single on-demand oak sapling (€5.50) even when assigned to Landowner.
            var zeroCashSession = new MultiAreaContinuation(exp, MultiAreaModelSelection.Phase2WithEconomy, null, null, p2Opts,
                EconomyOptions.Default with { OpeningCashCents = 0L });
            var pt = Phase2Support.FindPoint(zeroCashSession, "MA-B", 1.0f);
            var zv = zeroCashSession.AddPlant(Fence(zeroCashSession.ReadView()), "MA-B", Phase2Species.OakId, pt.X, pt.Z).View;
            var plantOrder = zeroCashSession.ReadPhase2View().PendingOrders[0];
            zv = zeroCashSession.SetOrderExecutor(Fence(zv), EconomyOrderKey.ForPlanting(plantOrder), WorkExecutor.Landowner).View;
            var zq = zeroCashSession.QuoteWorkPlan(Fence(zv));
            T.Eq(550L, zq.MaterialCostCents, "oak sapling on-demand material cost is €5.50");
            T.Eq(0L, zq.ContractorNonHarvestLabourCents, "landowner labour cash cost is €0");
            T.Refuses("EconomyInsufficientCash", () => zeroCashSession.Approve(Fence(zv)));
            T.Eq(0L, zeroCashSession.ReadEconomyView().CashBalanceCents, "zero-cash balance never goes negative");
        });

        // 6. 40h landowner allowance and Fell is contractor-only
        T.Add("economy: 40h landowner allowance and Fell is contractor-only", () =>
        {
            var session = MixedEconomySession();
            var v = session.ReadView();
            T.Eq(2400, session.ReadEconomyView().RemainingLandownerMinutes, "starts with 40.0 h (2,400 min) annual allowance");

            // Fell is strictly contractor-only: assigning Landowner is refused with typed code.
            string fellTreeId = v.Areas[0].Trees.First(t => t.IsLiving).TreeId;
            v = session.AddPending(Fence(v), "MA-A", fellTreeId, "contractor-only test").View;
            T.Refuses("EconomyFellContractorOnly", () => session.SetFellExecutor(Fence(v), "MA-A", fellTreeId, WorkExecutor.Landowner));
            T.Refuses("EconomyFellContractorOnly", () => session.SetOrderExecutor(Fence(v), EconomyOrderKey.ForFell("MA-A", fellTreeId), WorkExecutor.Landowner));
            v = session.RemovePending(Fence(v), "MA-A", fellTreeId).View;

            // Queue eligible low-risk actions (Plant + Shelter + SpotClear + Prune + ClearUndergrowth) and assign Landowner.
            var snap = session.ReadCompositeUiSnapshot();
            var pt = Phase2Support.FindPoint(session, "MA-B", 1.0f);
            var cell = snap.Readout.Areas[1].Ground[0];
            string pruneTree = snap.View.Areas[1].Trees.First(t => t.IsLiving &&
                snap.Ecology!.PruneStatusByTreeId[t.TreeId].Eligible).TreeId;

            v = session.AddSpotClear(Fence(v), "MA-B", pt.X, pt.Z).View;
            v = session.AddPlant(Fence(v), "MA-B", Phase2Species.BeechId, pt.X, pt.Z).View;
            v = session.AddShelter(Fence(v), "MA-B", pt.X, pt.Z).View;
            v = session.AddScheduledPrune(Fence(v), "MA-B", pruneTree).View;
            v = session.AddClearUndergrowth(Fence(v), "MA-B", cell.Column, cell.Row).View;

            var p2Pending = session.ReadPhase2View().PendingOrders;
            foreach (var po in p2Pending)
                v = session.SetOrderExecutor(Fence(v), EconomyOrderKey.ForPlanting(po), WorkExecutor.Landowner).View;
            v = session.SetOrderExecutor(Fence(v), EconomyOrderKey.ForPrune("MA-B", pruneTree), WorkExecutor.Landowner).View;
            v = session.SetOrderExecutor(Fence(v), EconomyOrderKey.ForClear("MA-B", cell.Column, cell.Row), WorkExecutor.Landowner).View;

            var quote = session.QuoteWorkPlan(Fence(v));
            T.True(quote.IsEligible, "all five low-risk actions eligible for Landowner");
            T.Eq(0L, quote.ContractorNonHarvestLabourCents, "landowner DIY incurs €0 contractor labour");
            T.Eq(950L, quote.MaterialCostCents, "beech €4.50 + shelter €5.00 = €9.50 materials on demand");
            int expectedMinutes = 4 + 10 + 2 + 4 + 12; // SpotClear(4) + Plant(10) + Shelter(ceil 60/45=2) + Prune(4) + Clear(12) = 32 min
            T.Eq(expectedMinutes, quote.TotalOwnerMinutesRequired, "expected landowner minutes = 32");

            v = session.Approve(Fence(v)).View;
            var exec = session.Execute(Fence(v), v.Approved!.PlanId, v.Approved.ExecutionKey);
            T.Eq(2400 - expectedMinutes, session.ReadEconomyView().RemainingLandownerMinutes, "allowance depleted by 32 min");
            T.Eq(expectedMinutes, session.ReadEconomyView().UsedLandownerMinutesThisYear, "used minutes recorded");

            // Annual advance resets landowner allowance to 40.0 h (2,400 min) via ReservedPhase2BYearEndSummaryStep.
            v = session.Advance(Fence(exec.View), consentToCancelWork: false).View;
            var afterAdvance = session.ReadEconomyView();
            T.Eq(2400, afterAdvance.RemainingLandownerMinutes, "allowance reset to 2,400 min on annual advance");
            T.Eq(0, afterAdvance.UsedLandownerMinutesThisYear, "used minutes reset to 0");
            T.Eq(1, afterAdvance.YearEndSummaries.Count, "year-end summary recorded");
            T.Eq(expectedMinutes, afterAdvance.YearEndSummaries[0].YearLandownerMinutesUsed, "summary records prior year usage");

            // Exceeding remaining allowance refuses cleanly with EconomyInsufficientLandownerAllowance.
            var (exp, p2Opts, _) = MultiAreaDemoExperiments.CreateMixedDemo();
            var tightSession = new MultiAreaContinuation(exp, MultiAreaModelSelection.Phase2WithEconomy, null, null, p2Opts,
                EconomyOptions.Default with { AnnualLandownerAllowanceMinutes = 5 });
            var tv = tightSession.ReadView();
            var tpt = Phase2Support.FindPoint(tightSession, "MA-B", 1.0f);
            tv = tightSession.AddPlant(Fence(tv), "MA-B", Phase2Species.OakId, tpt.X, tpt.Z).View;
            var tOrder = tightSession.ReadPhase2View().PendingOrders[0];
            tv = tightSession.SetOrderExecutor(Fence(tv), EconomyOrderKey.ForPlanting(tOrder), WorkExecutor.Landowner).View;
            T.Refuses("EconomyInsufficientLandownerAllowance", () => tightSession.Approve(Fence(tv)));
        });

        // 7. Grouped harvest floor per felling plan
        T.Add("economy: grouped harvest floor per felling plan", () =>
        {
            var session = MixedEconomySession();

            // First felling plan (small 1-tree harvest): FirstThinning €20/t, variable < €2,500 -> floor charges €2,500.
            var v = session.ReadView();
            var liveA = v.Areas[0].Trees.Where(t => t.IsLiving).Take(2).ToArray();
            v = session.AddPending(Fence(v), "MA-A", liveA[0].TreeId, "small harvest 1").View;
            var q1 = session.QuoteWorkPlan(Fence(v));
            T.Eq(HarvestThinningStage.FirstThinning, q1.HarvestJob!.Stage, "first felling plan is FirstThinning");
            T.Eq(2_000L, q1.HarvestJob.StageRateCentsPerTonne, "FirstThinning rate = €20/t");
            T.True(q1.HarvestJob.VariableHarvestCostCents < 250_000L, "small harvest variable cost below floor");
            T.Eq(250_000L, q1.HarvestJob.TotalHarvestContractorCostCents, "small harvest charged exact €2,500 floor");
            T.Eq(250_000L - q1.HarvestJob.VariableHarvestCostCents, q1.HarvestJob.MinimumJobAdjustmentCents, "floor adjustment exact");

            v = session.Approve(Fence(v)).View;
            v = session.Execute(Fence(v), v.Approved!.PlanId, v.Approved.ExecutionKey).View;

            // Second separate felling plan in the SAME ecological year: SecondThinning €23/t, incurs a SECOND €2,500 floor!
            T.Eq(0, v.Source.EcologicalYear, "still same ecological year");
            v = session.AddPending(Fence(v), "MA-A", liveA[1].TreeId, "small harvest 2 same year").View;
            var q2 = session.QuoteWorkPlan(Fence(v));
            T.Eq(HarvestThinningStage.SecondThinning, q2.HarvestJob!.Stage, "second felling plan is SecondThinning");
            T.Eq(2_300L, q2.HarvestJob.StageRateCentsPerTonne, "SecondThinning rate = €23/t");
            T.Eq(250_000L, q2.HarvestJob.TotalHarvestContractorCostCents, "second separate plan incurs its own €2,500 floor");

            v = session.Approve(Fence(v)).View;
            v = session.Execute(Fence(v), v.Approved!.PlanId, v.Approved.ExecutionKey).View;
            var econ = session.ReadEconomyView();
            T.Eq(2, econ.ExecutedHarvestPlansCount, "two executed harvest plans");
            T.Eq(500_000L, econ.Settlements.Sum(s => s.TotalExternalCostCents), "two separate plans paid €5,000 total floors");

            // Third felling plan auto-progresses to LaterThinning (€20/t), and explicit override reaches Clearfell (€15/t).
            string thirdTree = v.Areas[0].Trees.First(t => t.IsLiving).TreeId;
            v = session.AddPending(Fence(v), "MA-A", thirdTree, "stage rate check").View;
            var q3 = session.QuoteWorkPlan(Fence(v));
            T.Eq(HarvestThinningStage.LaterThinning, q3.HarvestJob!.Stage, "third harvest auto-selects LaterThinning");
            T.Eq(2_000L, q3.HarvestJob.StageRateCentsPerTonne, "LaterThinning rate = €20/t");

            v = session.SetHarvestStageOverride(Fence(v), HarvestThinningStage.Clearfell).View;
            var qClear = session.QuoteWorkPlan(Fence(v));
            T.Eq(HarvestThinningStage.Clearfell, qClear.HarvestJob!.Stage, "explicit Clearfell stage");
            T.Eq(1_500L, qClear.HarvestJob.StageRateCentsPerTonne, "Clearfell rate = €15/t");
        });

        // 8. 0.64 ha calibration and D-062 assortment
        T.Add("economy: 0.64 ha calibration and D-062 assortment", () =>
        {
            // Verify deterministic 80x80 m / 5-lane / 1,344-stem calibration fixture & €12,000 opening cash.
            var f1 = EconomyCalibrationFixture.Create();
            var f2 = EconomyCalibrationFixture.Create();
            T.Eq(f1.ContentHash(), f2.ContentHash(), "0.64 ha fixture is deterministic");
            T.Eq(5, f1.Areas.Count, "5 extraction lanes");
            int totalStems = f1.Areas.Sum(a => a.Trees.Count);
            T.Eq(1_344, totalStems, "exact 1,344 stems");
            double totalAreaHa = f1.Areas.Sum(a => (double)a.AreaHa);
            T.True(Math.Abs(totalAreaHa - 0.64) < 1e-6, "total stand area = 0.64 ha (80x80 m)");

            var calibSession = new MultiAreaContinuation(f1, MultiAreaModelSelection.Phase2WithEconomy);
            T.Eq(1_200_000L, calibSession.ReadEconomyView().CashBalanceCents, "opening cash on 0.64 ha stand = €12,000");

            // Verify 890 kg/m³ proxy conversion table (€/m³ -> whole €/t).
            T.Eq(25_800L, D062TimberPriceBook.FindPrice(Phase2Species.OakId, TimberAssortmentKind.VeneerPlanking).SelectedCentsPerTonne, "Oak veneer €230/m³ -> €258/t");
            T.Eq(16_300L, D062TimberPriceBook.FindPrice(Phase2Species.OakId, TimberAssortmentKind.Beam).SelectedCentsPerTonne, "Oak beam €145/m³ -> €163/t");
            T.Eq(11_200L, D062TimberPriceBook.FindPrice(Phase2Species.OakId, TimberAssortmentKind.Fencing).SelectedCentsPerTonne, "Oak fencing €100/m³ -> €112/t");
            T.Eq(6_700L, D062TimberPriceBook.FindPrice(Phase2Species.OakId, TimberAssortmentKind.SecondPlanking).SelectedCentsPerTonne, "Oak 2nd planking €60/m³ -> €67/t");
            T.Eq(3_400L, D062TimberPriceBook.FindPrice(Phase2Species.OakId, TimberAssortmentKind.Firewood).SelectedCentsPerTonne, "Oak firewood €30/m³ -> €34/t");
            T.Eq(6_700L, D062TimberPriceBook.FindPrice(Phase2Species.BeechId, TimberAssortmentKind.SecondPlanking).SelectedCentsPerTonne, "Beech 2nd planking €60/m³ -> €67/t");
            T.Eq(3_400L, D062TimberPriceBook.FindPrice(Phase2Species.BeechId, TimberAssortmentKind.Firewood).SelectedCentsPerTonne, "Beech firewood €30/m³ -> €34/t");

            // D-062 ladders: large stem (DBH 52 cm, H 28 m) tested across unpruned Oak, pruned Oak, pruned Beech, and Sitka.
            var largeStem = new AnnualTree("T-LARGE", 10f, 10f, 80, 52f, 28f, 4.5f);
            var prunedState = new TreePruningState(Lifts: 1, CrownBaseHeightM: 3.5f, LastPruningYear: 1);

            var oakUnpruned = D062TimberAssortmentEngine.GradeStem(largeStem, Phase2Species.OakId, TreePruningState.Unpruned);
            var oakPruned = D062TimberAssortmentEngine.GradeStem(largeStem, Phase2Species.OakId, prunedState);
            var beechPruned = D062TimberAssortmentEngine.GradeStem(largeStem, Phase2Species.BeechId, prunedState);
            var sitkaUnpruned = D062TimberAssortmentEngine.GradeStem(largeStem, Phase2Species.SitkaId, TreePruningState.Unpruned);
            var sitkaPruned = D062TimberAssortmentEngine.GradeStem(largeStem, Phase2Species.SitkaId, prunedState);

            // Conservation holds on every graded stem.
            foreach (var y in new[] { oakUnpruned, oakPruned, beechPruned, sitkaUnpruned, sitkaPruned })
                T.Eq(y.StemVolumeCm3, y.SaleVolumeCm3 + y.ResidualVolumeCm3, y.SpeciesId + " volume conserved");

            // Unpruned Oak cannot produce VeneerPlanking (produces Beam instead).
            T.True(oakUnpruned.Logs.All(l => l.Assortment != TimberAssortmentKind.VeneerPlanking), "unpruned Oak has no VeneerPlanking");
            T.True(oakUnpruned.Logs.Any(l => l.Assortment == TimberAssortmentKind.Beam), "unpruned large Oak produces Beam");

            // Pruned Oak gains VeneerPlanking eligibility without any unit price change.
            T.True(oakPruned.Logs.Any(l => l.Assortment == TimberAssortmentKind.VeneerPlanking), "pruned Oak gains VeneerPlanking");
            T.Eq(25_800L, oakPruned.Logs.First(l => l.Assortment == TimberAssortmentKind.VeneerPlanking).UnitPriceCentsPerTonne,
                "pruned Oak uses unchanged VeneerPlanking unit price");
            T.True(oakPruned.TimberRevenueCents > oakUnpruned.TimberRevenueCents, "pruned Oak earns more via assortment eligibility");

            // Beech NEVER inherits Oak's VeneerPlanking, Beam, or Fencing classes — even at 52 cm pruned or 22 cm small end.
            var smallStem = new AnnualTree("T-SMALL", 10f, 10f, 35, 22f, 18f, 2.5f);
            var oakSmall = D062TimberAssortmentEngine.GradeStem(smallStem, Phase2Species.OakId, TreePruningState.Unpruned);
            var beechSmall = D062TimberAssortmentEngine.GradeStem(smallStem, Phase2Species.BeechId, TreePruningState.Unpruned);
            T.True(oakSmall.Logs.Any(l => l.Assortment == TimberAssortmentKind.Fencing), "22 cm Oak produces Fencing");
            T.True(beechPruned.Logs.All(l => l.Assortment is TimberAssortmentKind.SecondPlanking or TimberAssortmentKind.Firewood),
                "pruned large Beech only has SecondPlanking/Firewood (no veneer/beam/fencing)");
            T.True(beechSmall.Logs.All(l => l.Assortment is TimberAssortmentKind.SecondPlanking or TimberAssortmentKind.Firewood),
                "small Beech only has SecondPlanking/Firewood (no fencing)");
            T.True(beechPruned.Logs.Any(l => l.Assortment == TimberAssortmentKind.SecondPlanking), "large Beech produces SecondPlanking");

            // Sitka sees only Sitka rules and is unaffected by pruning or broadleaf rules.
            T.True(sitkaUnpruned.Logs.All(l => l.Assortment is TimberAssortmentKind.Sawlog or TimberAssortmentKind.Pallet or TimberAssortmentKind.Stake or TimberAssortmentKind.Pulp),
                "Sitka uses Sitka-only assortments");
            T.Eq(sitkaUnpruned.TimberRevenueCents, sitkaPruned.TimberRevenueCents, "Sitka timber valuation unchanged by pruning");
            T.Eq(sitkaUnpruned.Logs.Count, sitkaPruned.Logs.Count, "Sitka log allocation unchanged by pruning");
        });
    }

    private static MultiAreaContinuation MixedEconomySession()
    {
        var (exp, p2Opts, _) = MultiAreaDemoExperiments.CreateMixedDemo();
        return new MultiAreaContinuation(exp, MultiAreaModelSelection.Phase2WithEconomy, null, null, p2Opts);
    }

    private static MultiAreaFence Fence(MultiAreaSessionView v) => new(v.SessionId, v.Source, v.PlanStateRevision);

    private static MultiAreaRoot GetRoot(MultiAreaContinuation c) => (MultiAreaRoot)RootField.GetValue(c)!;
}
