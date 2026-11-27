using System.Globalization;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using SitkaP1.Core;

namespace SitkaP1.EconomyDispositions.Tests;

/// <summary>
/// Phase 2B-2b ENGINE HALF verification: SellAndExtract / KeepForUse / LeaveAsDeadwood through the REAL session path
/// (AddPending → SetFellingDisposition → Approve → Execute) on the deterministic ma-1-mixed demo with Economy enabled.
/// Fixture stems MA-B-001 (Sitka spruce), MA-B-002 (sessile oak), MA-B-003 (beech) are the first three living MA-B trees.
/// </summary>
internal static class DispositionTests
{
    private static readonly FieldInfo RootField =
        typeof(MultiAreaContinuation).GetField("root", BindingFlags.NonPublic | BindingFlags.Instance)!;

    private const string Area = "MA-B";
    private const string SitkaTree = "MA-B-001";
    private const string OakTree = "MA-B-002";
    private const string BeechTree = "MA-B-003";
    private static readonly string[] ThreeStems = { SitkaTree, OakTree, BeechTree };

    // ---- Pinned Phase 2B-1 numbers for the three-stem plan, captured on the UNMODIFIED base engine fa69d6d3 ----
    private const long PinnedTimberRevenueCents = 15_727L;
    private const long PinnedExternalCostCents = 250_000L;
    private const long PinnedNetCashDeltaCents = -234_273L;
    private const long PinnedVariableHarvestCents = 4_465L;
    private const long PinnedFloorAdjustmentCents = 245_535L;
    private const long PinnedTotalStemVolumeCm3 = 2_389_104L;
    private const long PinnedSaleVolumeCm3 = 2_304_305L;
    private const long PinnedResidualVolumeCm3 = 84_799L;
    private const long PinnedHarvestedGreenGrams = 2_126_303L;
    private const long PinnedCashAfterCents = 965_727L;
    private const long SitkaStemCm3 = 1_330_285L;
    private const long OakStemCm3 = 292_613L;
    private const long BeechStemCm3 = 766_206L;
    private const long SitkaStemRevenueCents = 10_219L;

    private static readonly (EconomyLedgerCategory Category, string Reference, long Cents)[] PinnedLedger =
    {
        (EconomyLedgerCategory.HarvestingAndForwarding, "harvest-variable", -4_465L),
        (EconomyLedgerCategory.MinimumJobAdjustment, "harvest-minimum-floor", -245_535L),
        (EconomyLedgerCategory.TimberSale, "beech:SecondPlanking", 1_519L),
        (EconomyLedgerCategory.TimberSale, "beech:Firewood", 1_458L),
        (EconomyLedgerCategory.TimberSale, "sessile-oak:Fencing", 2_438L),
        (EconomyLedgerCategory.TimberSale, "sessile-oak:Firewood", 93L),
        (EconomyLedgerCategory.TimberSale, "sitka-spruce:Stake", 166L),
        (EconomyLedgerCategory.TimberSale, "sitka-spruce:Pallet", 1_367L),
        (EconomyLedgerCategory.TimberSale, "sitka-spruce:Sawlog", 8_686L),
    };

    /// <summary>SHA-256 of the canonical mixed-plan disposition result (quote + settlement); pinned from the first observed run.</summary>
    private const string PinnedMixedDispositionHash = "d9c7aeddf2eb629b388c6cc3dfa93d5a55bd27c85e6262b323ddcbe6f14498ed";

    internal static void Register()
    {
        // 1
        T.Add("dispositions: default is SellAndExtract for every Fell order", () =>
        {
            var (session, v) = SessionWith(ThreeStems);
            var quote = session.QuoteWorkPlan(Fence(v));
            T.Eq(3, quote.FellDispositions.Count, "every Fell order has a resolved disposition");
            foreach (var id in ThreeStems)
                T.Eq(FellingDisposition.SellAndExtract, quote.FellDispositions[EconomyOrderKey.ForFell(Area, id)], id + " defaults to SellAndExtract");
            T.True(quote.HarvestJob!.StemDispositions.All(l => l.Disposition == FellingDisposition.SellAndExtract), "every stem line is SellAndExtract");
            T.Eq(0, session.ReadEconomyView().PendingFellDispositions.Count, "no explicit choice is stored by default");
            T.Eq(0, (int)default(FellingDisposition), "enum default value is 0");
            T.Eq(FellingDisposition.SellAndExtract, default(FellingDisposition), "enum default is SellAndExtract");

            // Explicitly choosing the default stores nothing; choosing then reverting returns to the default state.
            v = session.SetFellingDisposition(Fence(v), Area, OakTree, FellingDisposition.KeepForUse).View;
            T.Eq(1, session.ReadEconomyView().PendingFellDispositions.Count, "one explicit choice stored");
            v = session.SetFellingDisposition(Fence(v), Area, OakTree, FellingDisposition.SellAndExtract).View;
            T.Eq(0, session.ReadEconomyView().PendingFellDispositions.Count, "reverting to SellAndExtract stores nothing");
            T.Eq(0, Root(session).Economy!.PendingFellDispositions.Count, "state map empty after revert");
        });

        // 2
        T.Add("dispositions: SellAndExtract quote equals the unmodified engine (pinned integers)", () =>
        {
            foreach (bool explicitSell in new[] { false, true })
            {
                var (session, v) = SessionWith(ThreeStems);
                if (explicitSell)
                    foreach (var id in ThreeStems)
                        v = session.SetFellingDisposition(Fence(v), Area, id, FellingDisposition.SellAndExtract).View;
                string tag = explicitSell ? " (explicit Sell)" : " (default)";
                var q = session.QuoteWorkPlan(Fence(v));
                var h = q.HarvestJob!;
                T.Eq(PinnedTimberRevenueCents, q.TimberSaleRevenueCents, "TimberSaleRevenueCents" + tag);
                T.Eq(PinnedExternalCostCents, q.TotalExternalCostCents, "TotalExternalCostCents" + tag);
                T.Eq(PinnedNetCashDeltaCents, q.NetCashDeltaCents, "NetCashDeltaCents" + tag);
                T.Eq(PinnedExternalCostCents, q.UpfrontCashRequiredCents, "UpfrontCashRequiredCents" + tag);
                T.Eq(0, q.TotalOwnerMinutesRequired, "owner minutes" + tag);
                T.Eq(HarvestThinningStage.FirstThinning, h.Stage, "stage" + tag);
                T.Eq(2_100L, h.StageRateCentsPerTonne, "stage rate" + tag);
                T.Eq(3, h.FelledStemCount, "stem count" + tag);
                T.Eq(PinnedTotalStemVolumeCm3, h.TotalStemVolumeCm3, "total stem volume" + tag);
                T.Eq(PinnedSaleVolumeCm3, h.SaleVolumeCm3, "sale volume" + tag);
                T.Eq(PinnedResidualVolumeCm3, h.ResidualVolumeCm3, "residual volume" + tag);
                T.Eq(PinnedHarvestedGreenGrams, h.HarvestedGreenGrams, "harvested green grams" + tag);
                T.Eq(PinnedVariableHarvestCents, h.VariableHarvestCostCents, "variable harvest cost" + tag);
                T.Eq(250_000L, h.MinimumJobFloorCents, "floor" + tag);
                T.Eq(PinnedFloorAdjustmentCents, h.MinimumJobAdjustmentCents, "floor adjustment" + tag);
                T.Eq(PinnedExternalCostCents, h.TotalHarvestContractorCostCents, "contractor harvest cost" + tag);
                T.Eq(PinnedTimberRevenueCents, h.TimberSaleRevenueCents, "harvest-job revenue" + tag);
                AssertLedgerIs(PinnedLedger, q.EstimatedLedger, "quote ledger" + tag);

                v = session.Approve(Fence(v)).View;
                var r = session.Execute(Fence(v), v.Approved!.PlanId, v.Approved.ExecutionKey).Execution!.Settlement!;
                T.Eq(PinnedCashAfterCents, r.CashAfterCents, "cash after" + tag);
                T.Eq(PinnedNetCashDeltaCents, r.NetCashDeltaCents, "settled net" + tag);
                AssertLedgerIs(PinnedLedger, r.LedgerEntries, "settled ledger" + tag);
            }
        });

        // 3
        T.Add("dispositions: SellAndExtract adds 0 retained timber and 0 deadwood intents", () =>
        {
            var (session, v) = SessionWith(ThreeStems);
            var q = session.QuoteWorkPlan(Fence(v));
            T.Eq(0L, q.HarvestJob!.RetainedTimber.RetainedTimberCm3, "quote retained 0");
            T.Eq(0L, q.HarvestJob.RetainedStemVolumeCm3, "quote retained stem volume 0");
            T.Eq(0, q.HarvestJob.DeadwoodIntents.Count, "quote intents 0");
            var (r, after) = ApproveAndExecute(session, v);
            T.Eq(0L, r.RetainedTimberDelta.RetainedTimberCm3, "settled retained delta 0");
            T.Eq(0, r.RetainedTimberDelta.BySpecies.Count, "no species retained");
            T.Eq(0, r.DeadwoodIntents.Count, "settled intents 0");
            T.Eq(0L, session.ReadEconomyView().RetainedTimber.RetainedTimberCm3, "balance stays 0");
            T.Eq(0m, session.ReadEconomyView().RetainedTimber.RetainedTimberM3, "balance stays 0 m³");
        });

        // 4
        T.Add("dispositions: KeepForUse yields revenue 0 and no TimberSale ledger line", () =>
        {
            var (session, v) = SessionWith(ThreeStems);
            v = SetAll(session, v, FellingDisposition.KeepForUse);
            var q = session.QuoteWorkPlan(Fence(v));
            T.Eq(0L, q.TimberSaleRevenueCents, "quote revenue 0");
            T.Eq(0L, q.HarvestJob!.TimberSaleRevenueCents, "job revenue 0");
            T.Eq(0, q.HarvestJob.Assortments.Count, "no sold assortments");
            T.Eq(0L, q.HarvestJob.SaleVolumeCm3, "no sale volume");
            T.True(q.EstimatedLedger.All(e => e.Category != EconomyLedgerCategory.TimberSale), "no TimberSale line in quote");
            T.Eq(-PinnedExternalCostCents, q.NetCashDeltaCents, "net = minus contractor cost");
            var (r, _) = ApproveAndExecute(session, v);
            T.Eq(0L, r.TimberSaleRevenueCents, "settled revenue 0");
            T.True(r.LedgerEntries.All(e => e.Category != EconomyLedgerCategory.TimberSale), "no TimberSale line in settlement");
            T.True(r.LedgerEntries.All(e => e.CashDeltaCents != 0L), "no zero-value cash lines are invented");
        });

        // 5
        T.Add("dispositions: KeepForUse retained delta = exact D-062 stem volume (per stem, species, total; cm³ and m³)", () =>
        {
            var (session, v) = SessionWith(ThreeStems);
            var expected = ThreeStems.ToDictionary(id => id, id => GradeLive(session, id).StemVolumeCm3);
            T.Eq(SitkaStemCm3, expected[SitkaTree], "Sitka graded stem volume");
            T.Eq(OakStemCm3, expected[OakTree], "oak graded stem volume");
            T.Eq(BeechStemCm3, expected[BeechTree], "beech graded stem volume");
            v = SetAll(session, v, FellingDisposition.KeepForUse);
            var q = session.QuoteWorkPlan(Fence(v));
            var (r, _) = ApproveAndExecute(session, v);
            foreach (var tally in new[] { q.HarvestJob!.RetainedTimber, r.RetainedTimberDelta, session.ReadEconomyView().RetainedTimber })
            {
                T.Eq(3, tally.Stems.Count, "three retained stems");
                foreach (var id in ThreeStems)
                    T.Eq(expected[id], tally.Stems.Single(s => s.TreeId == id).StemVolumeCm3, id + " retained cm³");
                T.Eq(PinnedTotalStemVolumeCm3, tally.RetainedTimberCm3, "total retained cm³");
                T.Eq(2.389104m, tally.RetainedTimberM3, "total retained m³ (exact decimal)");
                T.Eq(3, tally.BySpecies.Count, "three species");
                T.Eq(SitkaStemCm3, tally.BySpecies[Phase2Species.SitkaId].RetainedTimberCm3, "Sitka cm³");
                T.Eq(1.330285m, tally.BySpecies[Phase2Species.SitkaId].RetainedTimberM3, "Sitka m³");
                T.Eq(OakStemCm3, tally.BySpecies[Phase2Species.OakId].RetainedTimberCm3, "oak cm³");
                T.Eq(0.292613m, tally.BySpecies[Phase2Species.OakId].RetainedTimberM3, "oak m³");
                T.Eq(BeechStemCm3, tally.BySpecies[Phase2Species.BeechId].RetainedTimberCm3, "beech cm³");
                T.Eq(0.766206m, tally.BySpecies[Phase2Species.BeechId].RetainedTimberM3, "beech m³");
                T.Eq(tally.RetainedTimberCm3, tally.BySpecies.Values.Sum(a => a.RetainedTimberCm3), "species sum = total");
            }
            T.Eq(PinnedTotalStemVolumeCm3, q.HarvestJob.RetainedStemVolumeCm3, "harvest job retained stem volume");
            T.True(r.RetainedTimberDelta.Stems.All(s => s.ExecutionKey == r.ExecutionKey && s.PlanId == r.PlanId &&
                s.WorkRevision == r.WorkRevision && s.EcologicalYear == r.EcologicalYear), "retained stems stamped with execution identity");
        });

        // 6
        T.Add("dispositions: KeepForUse accumulates across two executed plans through Execute", () =>
        {
            var (session, v) = SessionWith(new[] { SitkaTree, OakTree });
            v = SetAll(session, v, FellingDisposition.KeepForUse);
            var (r1, v1) = ApproveAndExecute(session, v);
            T.Eq(SitkaStemCm3 + OakStemCm3, r1.RetainedTimberDelta.RetainedTimberCm3, "first delta");
            T.Eq(SitkaStemCm3 + OakStemCm3, session.ReadEconomyView().RetainedTimber.RetainedTimberCm3, "balance after plan 1");

            string fourth = LivingIds(session).First(id => !ThreeStems.Contains(id));
            v = session.AddPending(Fence(v1), Area, BeechTree, "keep 2").View;
            v = session.AddPending(Fence(v), Area, fourth, "keep 2").View;
            long fourthCm3 = GradeLive(session, fourth).StemVolumeCm3;
            v = SetAll(session, v, FellingDisposition.KeepForUse);
            var (r2, _) = ApproveAndExecute(session, v);
            T.Eq(2, r2.RetainedTimberDelta.Stems.Count, "second delta has exactly the second plan's stems");
            T.True(r2.RetainedTimberDelta.Stems.Select(s => s.TreeId).OrderBy(x => x, StringComparer.Ordinal)
                .SequenceEqual(new[] { BeechTree, fourth }.OrderBy(x => x, StringComparer.Ordinal)), "second delta tree ids");
            T.Eq(BeechStemCm3 + fourthCm3, r2.RetainedTimberDelta.RetainedTimberCm3, "second delta volume");
            var balance = session.ReadEconomyView().RetainedTimber;
            T.Eq(SitkaStemCm3 + OakStemCm3 + BeechStemCm3 + fourthCm3, balance.RetainedTimberCm3, "cumulative balance");
            T.Eq(4, balance.Stems.Count, "four retained stems in the balance");
            T.Eq(balance.RetainedTimberCm3, balance.BySpecies.Values.Sum(a => a.RetainedTimberCm3), "balance species sum");
            T.Eq(2, session.ReadEconomyView().Settlements.Count, "two settlements");
            T.Eq(2, session.ReadEconomyView().ExecutedHarvestPlansCount, "two harvest plans counted");
        });

        // 7
        T.Add("dispositions: KeepForUse contractor cost identical to SellAndExtract (floor, plan #1 and #2 rates)", () =>
        {
            var costs = new Dictionary<FellingDisposition, (EconomyHarvestQuote Q1, EconomyHarvestQuote Q2)>();
            foreach (var d in new[] { FellingDisposition.SellAndExtract, FellingDisposition.KeepForUse })
            {
                var (session, v) = SessionWith(new[] { SitkaTree });
                v = SetAll(session, v, d);
                var q1 = session.QuoteWorkPlan(Fence(v)).HarvestJob!;
                var (_, after) = ApproveAndExecute(session, v);
                v = session.AddPending(Fence(after), Area, OakTree, "plan 2").View;
                v = SetAll(session, v, d);
                var q2 = session.QuoteWorkPlan(Fence(v)).HarvestJob!;
                costs[d] = (q1, q2);
            }
            var sell = costs[FellingDisposition.SellAndExtract];
            var keep = costs[FellingDisposition.KeepForUse];
            T.Eq(2_100L, keep.Q1.StageRateCentsPerTonne, "plan #1 = FirstThinning €21/t");
            T.Eq(2_300L, keep.Q2.StageRateCentsPerTonne, "plan #2 = SecondThinning €23/t");
            foreach (var (s, k, n) in new[] { (sell.Q1, keep.Q1, "#1"), (sell.Q2, keep.Q2, "#2") })
            {
                T.Eq(s.Stage, k.Stage, "stage " + n);
                T.Eq(s.StageRateCentsPerTonne, k.StageRateCentsPerTonne, "rate " + n);
                T.Eq(s.HarvestedGreenGrams, k.HarvestedGreenGrams, "green mass " + n);
                T.Eq(s.VariableHarvestCostCents, k.VariableHarvestCostCents, "variable cost " + n);
                T.Eq(s.MinimumJobAdjustmentCents, k.MinimumJobAdjustmentCents, "floor adjustment " + n);
                T.Eq(250_000L, k.TotalHarvestContractorCostCents, "€2,500 floor " + n);
                T.Eq(s.TotalHarvestContractorCostCents, k.TotalHarvestContractorCostCents, "contractor cost " + n);
            }
        });

        // 8
        T.Add("dispositions: KeepForUse emits no deadwood intents", () =>
        {
            var (session, v) = SessionWith(ThreeStems);
            v = SetAll(session, v, FellingDisposition.KeepForUse);
            int deadwoodBefore = Root(session).Stage4?.Deadwood.Count ?? 0;
            var q = session.QuoteWorkPlan(Fence(v));
            T.Eq(0, q.HarvestJob!.DeadwoodIntents.Count, "quote intents 0");
            T.True(q.HarvestJob.StemDispositions.All(l => !l.EmitsDeadwoodIntent), "no stem line emits an intent");
            T.Eq(0L, q.HarvestJob.DeadwoodStemVolumeCm3, "deadwood stem volume 0");
            var (r, _) = ApproveAndExecute(session, v);
            T.Eq(0, r.DeadwoodIntents.Count, "settled intents 0");
            T.Eq(deadwoodBefore, Root(session).Stage4?.Deadwood.Count ?? 0, "no Stage 4 deadwood appended");
        });

        // 9
        T.Add("dispositions: LeaveAsDeadwood revenue 0, retained 0, contractor cost = SellAndExtract", () =>
        {
            var (sellSession, sv) = SessionWith(ThreeStems);
            var sellJob = sellSession.QuoteWorkPlan(Fence(sv)).HarvestJob!;
            var (session, v) = SessionWith(ThreeStems);
            v = SetAll(session, v, FellingDisposition.LeaveAsDeadwood);
            var q = session.QuoteWorkPlan(Fence(v));
            T.Eq(0L, q.TimberSaleRevenueCents, "revenue 0");
            T.Eq(0L, q.HarvestJob!.RetainedTimber.RetainedTimberCm3, "retained 0");
            T.Eq(PinnedTotalStemVolumeCm3, q.HarvestJob.DeadwoodStemVolumeCm3, "all stem volume left as deadwood");
            T.Eq(sellJob.HarvestedGreenGrams, q.HarvestJob.HarvestedGreenGrams, "same green mass");
            T.Eq(sellJob.VariableHarvestCostCents, q.HarvestJob.VariableHarvestCostCents, "same variable cost");
            T.Eq(sellJob.TotalHarvestContractorCostCents, q.HarvestJob.TotalHarvestContractorCostCents, "same contractor cost");
            T.Eq(PinnedExternalCostCents, q.TotalExternalCostCents, "external cost unchanged");
            var (r, _) = ApproveAndExecute(session, v);
            T.Eq(0L, r.TimberSaleRevenueCents, "settled revenue 0");
            T.Eq(0L, r.RetainedTimberDelta.RetainedTimberCm3, "settled retained 0");
            T.Eq(0L, session.ReadEconomyView().RetainedTimber.RetainedTimberCm3, "balance 0");
            T.Eq(PinnedExternalCostCents, r.TotalExternalCostCents, "settled cost unchanged");
        });

        // 10
        T.Add("dispositions: LeaveAsDeadwood one intent per stem; factory assigns sequential DW ids", () =>
        {
            var (session, v) = SessionWith(ThreeStems);
            var trees = ThreeStems.ToDictionary(id => id, id => LiveTree(session, id));
            v = SetAll(session, v, FellingDisposition.LeaveAsDeadwood);
            int deadwoodBefore = Root(session).Stage4?.Deadwood.Count ?? 0;
            int nextBefore = Root(session).Stage4?.NextDeadwoodNumber ?? 1;
            var (r, _) = ApproveAndExecute(session, v);
            T.Eq(3, r.DeadwoodIntents.Count, "exactly one intent per stem");
            T.True(r.DeadwoodIntents.Select(i => i.TreeId).SequenceEqual(ThreeStems), "canonical area/tree order");
            var species = new Dictionary<string, string>
            {
                [SitkaTree] = Phase2Species.SitkaId, [OakTree] = Phase2Species.OakId, [BeechTree] = Phase2Species.BeechId,
            };
            foreach (var intent in r.DeadwoodIntents)
            {
                var tree = trees[intent.TreeId];
                T.Eq(Area, intent.AreaId, "intent area");
                T.Eq(species[intent.TreeId], intent.SpeciesId, intent.TreeId + " species");
                T.Eq(BitConverter.SingleToInt32Bits(FallenDeadwoodRecord.StemVolumeM3(tree.DbhCm, tree.HeightM)),
                    BitConverter.SingleToInt32Bits(intent.VolumeM3), intent.TreeId + " VolumeM3 bit-exact float32");
                T.Eq(tree.DbhCm, intent.DbhCm, "dbh");
                T.Eq(tree.HeightM, intent.HeightM, "height");
                T.Eq(tree.XM, intent.XM, "x");
                T.Eq(tree.ZM, intent.ZM, "z");
                T.Eq(r.EcologicalYear, intent.EcologicalYear, "intent year");
                T.Eq(r.WorkRevision, intent.WorkRevision, "intent work revision");
                T.Eq(r.ExecutionKey, intent.ExecutionKey, "intent execution key");
                T.Eq(r.PlanId, intent.PlanId, "intent plan id");
            }
            // Engine half: Execute does NOT append to Stage 4 yet (follow-up wiring step).
            T.Eq(deadwoodBefore, Root(session).Stage4?.Deadwood.Count ?? 0, "Stage 4 deadwood unchanged in the engine half");

            var (records, next) = FelledStemDeadwood.Materialise(r.DeadwoodIntents, 7);
            T.Eq(10, next, "next number after three records");
            T.True(records.Select(d => d.DeadwoodId).SequenceEqual(new[] { "DW0007", "DW0008", "DW0009" }), "sequential DW ids");
            for (int i = 0; i < 3; i++)
            {
                var d = records[i];
                var intent = r.DeadwoodIntents[i];
                T.Eq(intent.TreeId, d.TreeId, "record tree");
                T.Eq(intent.AreaId, d.AreaId, "record area");
                T.Eq(intent.SpeciesId, d.SpeciesId, "record species");
                T.Eq(intent.EcologicalYear, d.DeathYear, "record year");
                T.Eq(FellCause.Harvest, d.Cause, "record cause is the Fell cause");
                T.Eq(BitConverter.SingleToInt32Bits(intent.VolumeM3), BitConverter.SingleToInt32Bits(d.EstimatedVolumeM3), "record volume");
            }
            var (fromLive, nextLive) = FelledStemDeadwood.Materialise(r.DeadwoodIntents, nextBefore);
            T.Eq(FallenDeadwoodRecord.IdFor(nextBefore), fromLive[0].DeadwoodId, "factory starts at the supplied NextDeadwoodNumber");
            T.Eq(nextBefore + 3, nextLive, "factory advances by the intent count");
            var (none, same) = FelledStemDeadwood.Materialise(Array.Empty<FelledStemDeadwoodIntent>(), 5);
            T.Eq(0, none.Count, "no intents → no records");
            T.Eq(5, same, "no intents → number unchanged");
            T.True(Throws<InvalidOperationException>(() =>
                FelledStemDeadwood.Materialise(new[] { r.DeadwoodIntents[0], r.DeadwoodIntents[0] }, 1)), "duplicate stem refused");
            T.True(Throws<ArgumentOutOfRangeException>(() => FelledStemDeadwood.Materialise(r.DeadwoodIntents, 0)), "number 0 refused");
        });

        // 11
        T.Add("dispositions: mixed plan (Sell + Keep + Leave) is one job, one floor", () =>
        {
            var (session, v) = MixedSession();
            var q = session.QuoteWorkPlan(Fence(v));
            var h = q.HarvestJob!;
            T.Eq(3, h.FelledStemCount, "one grouped job with all three stems");
            T.Eq(2_100L, h.StageRateCentsPerTonne, "one stage rate");
            T.Eq(PinnedHarvestedGreenGrams, h.HarvestedGreenGrams, "full green mass");
            T.Eq(PinnedVariableHarvestCents, h.VariableHarvestCostCents, "same variable cost");
            T.Eq(250_000L, h.TotalHarvestContractorCostCents, "single €2,500 floor");
            T.Eq(1, q.EstimatedLedger.Count(e => e.Category == EconomyLedgerCategory.MinimumJobAdjustment), "one floor line");
            T.Eq(1, q.EstimatedLedger.Count(e => e.Category == EconomyLedgerCategory.HarvestingAndForwarding), "one harvest line");
            T.Eq(SitkaStemRevenueCents, q.TimberSaleRevenueCents, "revenue only from the Sitka sell stem");
            T.True(q.EstimatedLedger.Where(e => e.Category == EconomyLedgerCategory.TimberSale)
                .All(e => e.ReferenceId.StartsWith(Phase2Species.SitkaId + ":", StringComparison.Ordinal)), "TimberSale lines only for Sitka");
            T.Eq(SitkaStemRevenueCents, q.EstimatedLedger.Where(e => e.Category == EconomyLedgerCategory.TimberSale).Sum(e => e.CashDeltaCents),
                "TimberSale lines sum to the sell stem revenue");
            T.Eq(OakStemCm3, h.RetainedTimber.RetainedTimberCm3, "retained only oak");
            T.Eq(1, h.RetainedTimber.BySpecies.Count, "one retained species");
            T.Eq(1, h.DeadwoodIntents.Count, "one intent");
            T.Eq(BeechTree, h.DeadwoodIntents[0].TreeId, "intent is the beech stem");
            T.Eq(h.TotalStemVolumeCm3, h.SaleVolumeCm3 + h.ResidualVolumeCm3 + h.RetainedStemVolumeCm3 + h.DeadwoodStemVolumeCm3,
                "volume conservation across dispositions");
            T.Eq(SitkaStemCm3, h.SaleVolumeCm3 + h.ResidualVolumeCm3, "sale+residual is the Sitka stem");
            T.Eq(SitkaStemRevenueCents - 250_000L, q.NetCashDeltaCents, "net cash");

            var (r, _) = ApproveAndExecute(session, v);
            T.Eq(SitkaStemRevenueCents, r.TimberSaleRevenueCents, "settled revenue");
            T.Eq(OakStemCm3, r.RetainedTimberDelta.RetainedTimberCm3, "settled retained");
            T.Eq(1, r.DeadwoodIntents.Count, "settled intents");
            T.Eq(EconomyDefaults.DefaultOpeningCashCents + SitkaStemRevenueCents - 250_000L, r.CashAfterCents, "cash after");
            T.Eq(1, session.ReadEconomyView().ExecutedHarvestPlansCount, "one harvest plan executed");
        });

        // 12
        T.Add("dispositions: freeze at Approve; post-approval change refuses; live drift refuses", () =>
        {
            var (session, v) = MixedSession();
            v = session.Approve(Fence(v)).View;
            var frozen = session.ReadEconomyView().ApprovedQuote!;
            T.Eq(FellingDisposition.SellAndExtract, frozen.FrozenFellDispositions[EconomyOrderKey.ForFell(Area, SitkaTree)], "frozen Sell");
            T.Eq(FellingDisposition.KeepForUse, frozen.FrozenFellDispositions[EconomyOrderKey.ForFell(Area, OakTree)], "frozen Keep");
            T.Eq(FellingDisposition.LeaveAsDeadwood, frozen.FrozenFellDispositions[EconomyOrderKey.ForFell(Area, BeechTree)], "frozen Leave");
            T.Refuses("WorkPlanAlreadyApproved", () => session.SetFellingDisposition(Fence(v), Area, OakTree, FellingDisposition.SellAndExtract));

            // Simulate a live disposition change behind the command surface: the oak would become a sale (repricing).
            var approvedRoot = Root(session);
            var econ = approvedRoot.Economy!;
            var oakOrder = approvedRoot.Approved!.Orders.Single(o => o.TreeId == OakTree);
            var tampered = new Dictionary<EconomyOrderKey, FellingDispositionChoice>(econ.PendingFellDispositions);
            tampered.Remove(EconomyOrderKey.ForFell(Area, OakTree));
            RootField.SetValue(session, approvedRoot with { Economy = econ with { PendingFellDispositions = tampered } });
            T.Refuses("EconomyQuoteDrift", () => session.Execute(Fence(v), v.Approved!.PlanId, v.Approved.ExecutionKey));
            T.Eq(EconomyDefaults.DefaultOpeningCashCents, session.ReadEconomyView().CashBalanceCents, "no cash moved on drift");

            tampered[EconomyOrderKey.ForFell(Area, OakTree)] = new FellingDispositionChoice(oakOrder, FellingDisposition.LeaveAsDeadwood);
            RootField.SetValue(session, approvedRoot with { Economy = econ with { PendingFellDispositions = tampered } });
            T.Refuses("EconomyQuoteDrift", () => session.Execute(Fence(v), v.Approved!.PlanId, v.Approved.ExecutionKey));

            // Restored live state settles exactly the frozen amounts.
            RootField.SetValue(session, approvedRoot);
            var r = session.Execute(Fence(v), v.Approved!.PlanId, v.Approved.ExecutionKey).Execution!.Settlement!;
            T.Eq(frozen.Quote.TimberSaleRevenueCents, r.TimberSaleRevenueCents, "settled frozen revenue");
            T.Eq(OakStemCm3, r.RetainedTimberDelta.RetainedTimberCm3, "settled frozen retained");
            T.Eq(0, Root(session).Economy!.PendingFellDispositions.Count, "pending dispositions cleared by settlement");

            // Cancelling an approval drops its orders; a re-added order starts again at the default.
            var (s2, v2) = MixedSession();
            v2 = s2.Approve(Fence(v2)).View;
            v2 = s2.CancelApproved(Fence(v2)).View;
            v2 = s2.AddPending(Fence(v2), Area, OakTree, "re-added after cancel").View;
            var q2 = s2.QuoteWorkPlan(Fence(v2));
            T.Eq(FellingDisposition.SellAndExtract, q2.FellDispositions[EconomyOrderKey.ForFell(Area, OakTree)], "stale choice not resurrected");
            T.Eq(0, s2.ReadEconomyView().PendingFellDispositions.Count, "view shows no stale choice");
        });

        // 13
        T.Add("dispositions: clearfell, whole-area clear and overrides stay impossible", () =>
        {
            var (session, v) = SessionWith(new[] { OakTree });
            T.Refuses("EconomyFellContractorOnly", () => session.SetFellExecutor(Fence(v), Area, OakTree, WorkExecutor.Landowner));
            v = session.SetFellingDisposition(Fence(v), Area, OakTree, FellingDisposition.KeepForUse).View;
            T.Refuses("EconomyFellContractorOnly", () => session.SetFellExecutor(Fence(v), Area, OakTree, WorkExecutor.Landowner));

            // Mark EVERY living MA-B stem for Fell with non-sale dispositions: still refused whole at Execute.
            foreach (var id in LivingIds(session).Where(id => id != OakTree))
                v = session.AddPending(Fence(v), Area, id, "whole-area attempt").View;
            int i = 0;
            foreach (var id in LivingIds(session))
                v = session.SetFellingDisposition(Fence(v), Area, id,
                    i++ % 2 == 0 ? FellingDisposition.KeepForUse : FellingDisposition.LeaveAsDeadwood).View;
            v = session.Approve(Fence(v)).View;
            var before = Root(session);
            T.Refuses("WorkWouldLeaveNoLivingStems", () => session.Execute(Fence(v), v.Approved!.PlanId, v.Approved.ExecutionKey));
            T.True(ReferenceEquals(before, Root(session)), "refused whole-area plan mutates nothing");
            T.Eq(0L, session.ReadEconomyView().RetainedTimber.RetainedTimberCm3, "no retained timber from a refused plan");

            // Surface: no harvest-rate / volume / price input on any felling, disposition, executor or quote command.
            const BindingFlags all = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
            var numeric = new HashSet<Type> { typeof(int), typeof(long), typeof(float), typeof(double), typeof(decimal) };
            foreach (var m in typeof(MultiAreaContinuation).GetMethods(all).Where(m =>
                m.Name.Contains("Disposition", StringComparison.Ordinal) || m.Name.Contains("Fell", StringComparison.Ordinal) ||
                m.Name.Contains("Executor", StringComparison.Ordinal) || m.Name.Contains("Quote", StringComparison.Ordinal)))
            {
                foreach (var p in m.GetParameters())
                {
                    var pt = Nullable.GetUnderlyingType(p.ParameterType) ?? p.ParameterType;
                    T.True(!numeric.Contains(pt), m.Name + "(" + p.Name + ") must not take a numeric rate/volume/price");
                }
            }
            var set = typeof(MultiAreaContinuation).GetMethod("SetFellingDisposition", all)!;
            T.True(set.GetParameters().Select(p => p.ParameterType).SequenceEqual(
                new[] { typeof(MultiAreaFence), typeof(string), typeof(string), typeof(FellingDisposition) }),
                "SetFellingDisposition takes only fence, area, tree and disposition");
            Type[] guarded =
            {
                typeof(MultiAreaContinuation), typeof(MultiAreaEconomyState), typeof(MultiAreaEconomyView), typeof(EconomyApprovedQuote),
                typeof(FellingDispositionChoice), typeof(FelledStemDispositionLine), typeof(RetainedTimberTally),
                typeof(FelledStemDeadwoodIntent), typeof(FellingDispositionRules), typeof(FelledStemDeadwood),
            };
            foreach (var type in guarded)
                foreach (var member in type.GetMembers(all))
                    foreach (var banned in new[] { "Clearf", "ClearFell", "Override", "WholeArea" })
                        T.True(!member.Name.Contains(banned, StringComparison.OrdinalIgnoreCase), type.Name + "." + member.Name + " must not expose " + banned);
            var names = Enum.GetNames<FellingDisposition>();
            T.Eq(3, names.Length, "exactly three dispositions");
            T.True(names.All(n => !n.Contains("clear", StringComparison.OrdinalIgnoreCase)), "no clear-felling disposition");
            T.True(!Enum.IsDefined((FellingDisposition)3), "ordinal 3 undefined");
        });

        // 14
        T.Add("dispositions: Economy Off is inert", () =>
        {
            var (exp, p2, _) = MultiAreaDemoExperiments.CreateMixedDemo();
            var session = new MultiAreaContinuation(exp, MultiAreaModelSelection.Phase2, null, null, p2);
            var v = session.ReadView();
            v = session.AddPending(Fence(v), Area, OakTree, "off").View;
            var before = Root(session);
            T.Refuses("EconomyModelRequired", () => session.SetFellingDisposition(Fence(v), Area, OakTree, FellingDisposition.KeepForUse));
            T.Refuses("EconomyModelRequired", () => session.SetFellingDisposition(Fence(v), Area, OakTree, (FellingDisposition)9));
            T.True(ReferenceEquals(before, Root(session)), "refused command mutates nothing");
            T.True(Root(session).Economy is null, "no economy / disposition / retained state");
            int deadwoodBefore = Root(session).Stage4?.Deadwood.Count ?? 0;
            v = session.Approve(Fence(v)).View;
            var exec = session.Execute(Fence(v), v.Approved!.PlanId, v.Approved.ExecutionKey);
            T.True(exec.Execution!.Settlement is null, "no settlement when Off");
            T.True(Root(session).Economy is null, "Economy still null after Execute");
            T.Eq(deadwoodBefore, Root(session).Stage4?.Deadwood.Count ?? 0, "no deadwood side effect");
            var hist = session.ReadEconomyHistoryYear(0);
            T.True(!hist.EconomyActive, "history inactive");
            T.True(hist.TotalTimberRevenueCents is null && hist.NetCashDeltaCents is null && hist.TotalExternalCostCents is null, "numeric fields null");
            foreach (var display in new[] { hist.OpeningCashDisplay, hist.ClosingCashDisplay, hist.TotalExternalCostDisplay,
                hist.TotalTimberRevenueDisplay, hist.NetCashDeltaDisplay, hist.LandownerMinutesUsedDisplay })
                T.Eq("NOT RECORDED", display, "display reads NOT RECORDED");
        });

        // 15
        T.Add("dispositions: determinism hash stable across runs and fresh sessions", () =>
        {
            var hashes = new List<string>();
            for (int s = 0; s < 2; s++)
            {
                var (session, v) = MixedSession();
                var quotes = Enumerable.Range(0, 3).Select(_ => session.QuoteWorkPlan(Fence(v))).ToArray();
                var quoteHashes = quotes.Select(q => Digest(q, null)).ToArray();
                T.True(quoteHashes.Distinct().Count() == 1, "3 repeated quote evaluations identical");
                var (r, _) = ApproveAndExecute(session, v);
                hashes.Add(Digest(quotes[0], r));
            }
            Console.WriteLine("  determinism hash (mixed plan) = " + hashes[0]);
            T.Eq(hashes[0], hashes[1], "two fresh sessions give the same disposition result hash");
            T.Eq(PinnedMixedDispositionHash, hashes[0], "pinned cross-process hash");
        });

        // 16
        T.Add("dispositions: settlement reconciles for every disposition and the mixed plan", () =>
        {
            var cases = new List<(string Name, MultiAreaContinuation Session, MultiAreaSessionView View)>();
            foreach (var d in Enum.GetValues<FellingDisposition>())
            {
                var (s, v) = SessionWith(ThreeStems);
                cases.Add((d.ToString(), s, SetAll(s, v, d)));
            }
            var (ms, mv) = MixedSession();
            cases.Add(("Mixed", ms, mv));
            foreach (var (name, s, v) in cases)
            {
                var q = s.QuoteWorkPlan(Fence(v));
                T.Eq(q.NetCashDeltaCents, q.EstimatedLedger.Sum(e => e.CashDeltaCents), name + " quote ledger reconciles");
                T.Eq(q.TimberSaleRevenueCents - q.TotalExternalCostCents, q.NetCashDeltaCents, name + " net = revenue - cost");
                var (r, _) = ApproveAndExecute(s, v);
                T.Eq(r.NetCashDeltaCents, r.LedgerEntries.Sum(e => e.CashDeltaCents), name + " settlement ledger reconciles");
                T.Eq(r.CashBeforeCents + r.NetCashDeltaCents, r.CashAfterCents, name + " cash after");
                T.Eq(r.CashAfterCents, s.ReadEconomyView().CashBalanceCents, name + " committed cash");
                T.Eq(s.ReadEconomyView().Ledger.Sum(e => e.CashDeltaCents),
                    s.ReadEconomyView().CashBalanceCents - EconomyDefaults.DefaultOpeningCashCents, name + " whole ledger = cash movement");
                T.True(r.LedgerEntries.All(e => Enum.IsDefined(e.Category)), name + " only existing cash categories");
            }
        });

        // 17
        T.Add("dispositions: command refusals and removal clears the disposition", () =>
        {
            var (session, v) = SessionWith(new[] { OakTree });
            T.Refuses("EconomyDispositionUnknown", () => session.SetFellingDisposition(Fence(v), Area, OakTree, (FellingDisposition)3));
            T.Refuses("EconomyDispositionUnknown", () => session.SetFellingDisposition(Fence(v), Area, OakTree, (FellingDisposition)(-1)));
            T.Refuses("UnknownWorkOrder", () => session.SetFellingDisposition(Fence(v), Area, BeechTree, FellingDisposition.KeepForUse));
            T.Refuses("UnknownWorkOrder", () => session.SetFellingDisposition(Fence(v), "MA-A", OakTree, FellingDisposition.KeepForUse));
            var stale = Fence(v);
            v = session.SetFellingDisposition(Fence(v), Area, OakTree, FellingDisposition.LeaveAsDeadwood).View;
            T.Refuses("PlayerPlanStateStale", () => session.SetFellingDisposition(stale, Area, OakTree, FellingDisposition.KeepForUse));
            T.Eq(FellingDisposition.LeaveAsDeadwood, session.ReadEconomyView().PendingFellDispositions[EconomyOrderKey.ForFell(Area, OakTree)], "choice visible");

            v = session.RemovePending(Fence(v), Area, OakTree).View;
            T.Eq(0, Root(session).Economy!.PendingFellDispositions.Count, "RemovePending clears the disposition");
            T.Refuses("UnknownWorkOrder", () => session.SetFellingDisposition(Fence(v), Area, OakTree, FellingDisposition.KeepForUse));
            v = session.AddPending(Fence(v), Area, OakTree, "re-add").View;
            T.Eq(FellingDisposition.SellAndExtract,
                session.QuoteWorkPlan(Fence(v)).FellDispositions[EconomyOrderKey.ForFell(Area, OakTree)], "re-added order is default");
        });

        // 18
        T.Add("dispositions: consented year-end cancellation clears pending dispositions", () =>
        {
            var (session, v) = MixedSession();
            T.Eq(2, Root(session).Economy!.PendingFellDispositions.Count, "two explicit choices before advance");
            v = session.Advance(Fence(v), consentToCancelWork: true).View;
            T.Eq(0, Root(session).Economy!.PendingFellDispositions.Count, "cleared on cancelled work");
            T.Eq(0L, session.ReadEconomyView().RetainedTimber.RetainedTimberCm3, "cancel never creates retained timber");
        });

        // ---- Repair after independent review (B1, B2): published maps are genuinely read-only ----

        // 19
        T.Add("dispositions: approved frozen dispositions cannot be mutated through a cast (B1)", () =>
        {
            var (session, v) = MixedSession();
            v = session.Approve(Fence(v)).View;
            var oakKey = EconomyOrderKey.ForFell(Area, OakTree);
            var frozen = session.ReadEconomyView().ApprovedQuote!;
            AssertWriteRefused(frozen.FrozenFellDispositions, oakKey, FellingDisposition.LeaveAsDeadwood, "approved frozen map");
            AssertWriteRefused(frozen.Quote.FellDispositions, oakKey, FellingDisposition.SellAndExtract, "approved quote map");
            AssertWriteRefused(session.QuoteWorkPlan(Fence(v)).FellDispositions, oakKey, FellingDisposition.SellAndExtract, "pending quote map");

            var fresh = session.ReadEconomyView().ApprovedQuote!.FrozenFellDispositions;
            T.Eq(3, fresh.Count, "fresh read still has three frozen entries");
            T.Eq(FellingDisposition.SellAndExtract, fresh[EconomyOrderKey.ForFell(Area, SitkaTree)], "fresh read: Sitka still Sell");
            T.Eq(FellingDisposition.KeepForUse, fresh[oakKey], "fresh read: oak still Keep");
            T.Eq(FellingDisposition.LeaveAsDeadwood, fresh[EconomyOrderKey.ForFell(Area, BeechTree)], "fresh read: beech still Leave");

            var result = session.Execute(Fence(v), v.Approved!.PlanId, v.Approved.ExecutionKey);
            T.True(result.Execution is not null && !result.Execution.Replayed, "the approved plan still executes");
            T.Eq(OakStemCm3, result.Execution!.Settlement!.RetainedTimberDelta.RetainedTimberCm3, "settled the frozen Keep");
            T.Eq(1, result.Execution.Settlement.DeadwoodIntents.Count, "settled the frozen Leave");
        });

        // 20
        T.Add("dispositions: pending dispositions on state, view and Empty defaults cannot be mutated (B1)", () =>
        {
            var (session, v) = MixedSession();
            var oakKey = EconomyOrderKey.ForFell(Area, OakTree);
            var beechKey = EconomyOrderKey.ForFell(Area, BeechTree);
            var state = Root(session).Economy!.PendingFellDispositions;
            var oakChoice = state[oakKey];
            AssertWriteRefused(state, oakKey, oakChoice with { Disposition = FellingDisposition.SellAndExtract }, "state choices map");
            AssertWriteRefused(session.ReadEconomyView().PendingFellDispositions, oakKey, FellingDisposition.SellAndExtract, "view map");
            AssertWriteRefused(FellingDispositionRules.EmptyChoices, oakKey, oakChoice, "shared EmptyChoices");
            AssertWriteRefused(FellingDispositionRules.EmptyResolved, oakKey, FellingDisposition.KeepForUse, "shared EmptyResolved");
            T.Eq(0, FellingDispositionRules.EmptyChoices.Count, "EmptyChoices still empty");
            T.Eq(0, FellingDispositionRules.EmptyResolved.Count, "EmptyResolved still empty");

            T.Eq(2, Root(session).Economy!.PendingFellDispositions.Count, "state still holds two explicit choices");
            T.Eq(FellingDisposition.KeepForUse, Root(session).Economy!.PendingFellDispositions[oakKey].Disposition, "state oak still Keep");
            var view = session.ReadEconomyView().PendingFellDispositions;
            T.Eq(2, view.Count, "view still shows two explicit choices");
            T.Eq(FellingDisposition.KeepForUse, view[oakKey], "view oak still Keep");
            T.Eq(FellingDisposition.LeaveAsDeadwood, view[beechKey], "view beech still Leave");

            // After a command (RemovePending) the rebuilt state map is read-only too.
            v = session.RemovePending(Fence(v), Area, SitkaTree).View;
            AssertWriteRefused(Root(session).Economy!.PendingFellDispositions, oakKey, oakChoice, "state map after RemovePending");
            var fresh = NewEconomySession();
            AssertWriteRefused(fresh.ReadEconomyView().PendingFellDispositions, oakKey, FellingDisposition.KeepForUse, "fresh-session view map");
            AssertWriteRefused(Root(fresh).Economy!.PendingFellDispositions, oakKey, oakChoice, "fresh-session state map");
        });

        // 21
        T.Add("dispositions: retained BySpecies on the cumulative tally and the shared Empty cannot be mutated (B2)", () =>
        {
            var empty = RetainedTimberTally.Empty;
            AssertWriteRefused(empty.BySpecies, "sessile-oak", new RetainedTimberAmount("sessile-oak", 1L), "shared Empty tally");
            T.Eq(0, RetainedTimberTally.Empty.BySpecies.Count, "Empty BySpecies still empty");
            T.Eq(0L, RetainedTimberTally.Empty.RetainedTimberCm3, "Empty total still 0");

            var (session, v) = SessionWith(new[] { OakTree });
            AssertWriteRefused(session.ReadEconomyView().RetainedTimber.BySpecies, "sessile-oak",
                new RetainedTimberAmount("sessile-oak", 1L), "fresh-session view tally");
            v = SetAll(session, v, FellingDisposition.KeepForUse);
            var (receipt, _) = ApproveAndExecute(session, v);
            string species = receipt.RetainedTimberDelta.BySpecies.Keys.Single();
            var tampered = new RetainedTimberAmount(species, 1L);
            AssertWriteRefused(session.ReadEconomyView().RetainedTimber.BySpecies, species, tampered, "view cumulative tally");
            AssertWriteRefused(Root(session).Economy!.RetainedTimber.BySpecies, species, tampered, "state cumulative tally");
            AssertWriteRefused(receipt.RetainedTimberDelta.BySpecies, species, tampered, "settlement delta tally");

            foreach (var (tag, tally) in new[]
            {
                ("view", session.ReadEconomyView().RetainedTimber),
                ("state", Root(session).Economy!.RetainedTimber),
                ("receipt", session.ReadEconomyView().Settlements.Single().RetainedTimberDelta),
            })
            {
                T.Eq(OakStemCm3, tally.RetainedTimberCm3, tag + " total unchanged");
                T.Eq(OakStemCm3, tally.BySpecies.Values.Sum(a => a.RetainedTimberCm3), tag + " species sum unchanged");
                T.Eq(OakStemCm3, tally.BySpecies[species].RetainedTimberCm3, tag + " oak amount unchanged");
                T.Eq(1, tally.BySpecies.Count, tag + " species count unchanged");
            }
        });

        // 22
        T.Add("dispositions: read-only wrappers keep ordinal ascending enumeration (hash-neutral)", () =>
        {
            // Frozen disposition map, built with stems added in reverse order and a second area, must enumerate by key.
            var session = NewEconomySession();
            var v = session.ReadView();
            var otherArea = Root(session).Areas.Select(a => a.AreaId).First(id => id != Area);
            var otherTree = Root(session).Areas.Single(a => a.AreaId == otherArea).State.Trees.First(t => t.IsLiving).TreeId;
            foreach (var id in ThreeStems.Reverse())
                v = session.AddPending(Fence(v), Area, id, "order test").View;
            v = session.AddPending(Fence(v), otherArea, otherTree, "order test").View;
            v = session.SetFellingDisposition(Fence(v), Area, OakTree, FellingDisposition.KeepForUse).View;
            v = session.SetFellingDisposition(Fence(v), Area, BeechTree, FellingDisposition.KeepForUse).View;
            v = session.SetFellingDisposition(Fence(v), Area, SitkaTree, FellingDisposition.KeepForUse).View;
            v = session.Approve(Fence(v)).View;
            var frozenKeys = session.ReadEconomyView().ApprovedQuote!.FrozenFellDispositions.Keys.ToArray();
            var expectedKeys = frozenKeys.OrderBy(k => k).ToArray();
            T.True(frozenKeys.SequenceEqual(expectedKeys), "frozen map enumerates in EconomyOrderKey (ordinal) order");
            T.True(frozenKeys.Select(k => k.AreaId + "/" + k.TargetKey).SequenceEqual(
                new[] { otherArea + "/" + otherTree }.Concat(ThreeStems.Select(id => Area + "/" + id))
                    .OrderBy(x => x, StringComparer.Ordinal)), "frozen keys are ordinal ascending by area then tree");
            var viewKeys = session.ReadEconomyView().ApprovedQuote!.Quote.FellDispositions.Keys.ToArray();
            T.True(viewKeys.SequenceEqual(expectedKeys), "fresh read enumerates identically");

            var r = session.Execute(Fence(v), v.Approved!.PlanId, v.Approved.ExecutionKey).Execution!.Settlement!;
            foreach (var (tag, tally) in new[] { ("receipt", r.RetainedTimberDelta), ("balance", session.ReadEconomyView().RetainedTimber) })
            {
                var speciesKeys = tally.BySpecies.Keys.ToArray();
                T.Eq(3, speciesKeys.Length, tag + " three species retained");
                T.True(speciesKeys.SequenceEqual(speciesKeys.OrderBy(x => x, StringComparer.Ordinal)), tag + " BySpecies is ordinal ascending");
                T.True(speciesKeys.SequenceEqual(new[] { "beech", "sessile-oak", "sitka-spruce" }), tag + " BySpecies exact order");
                T.True(tally.BySpecies.Values.Select(a => a.SpeciesId).SequenceEqual(speciesKeys), tag + " values follow key order");
            }
        });
    }

    // Casts a published map to both dictionary interfaces and proves every write path throws NotSupportedException
    // and leaves the map unchanged.
    private static void AssertWriteRefused<TKey, TValue>(IReadOnlyDictionary<TKey, TValue> published, TKey key, TValue value, string what)
        where TKey : notnull
    {
        var before = published.ToArray();
        T.True(published is IDictionary<TKey, TValue>, what + " is castable to IDictionary (so the test exercises the cast)");
        var generic = (IDictionary<TKey, TValue>)published;
        T.True(generic.IsReadOnly, what + " reports IsReadOnly");
        T.True(Throws<NotSupportedException>(() => generic[key] = value), what + ": indexer set throws NotSupportedException");
        T.True(Throws<NotSupportedException>(() => generic.Add(key, value)), what + ": Add throws NotSupportedException");
        T.True(Throws<NotSupportedException>(() => generic.Remove(key)), what + ": Remove throws NotSupportedException");
        T.True(Throws<NotSupportedException>(() => generic.Clear()), what + ": Clear throws NotSupportedException");
        var nonGeneric = (System.Collections.IDictionary)published;
        T.True(Throws<NotSupportedException>(() => nonGeneric[key] = value), what + ": non-generic indexer set throws NotSupportedException");
        T.True(published.ToArray().SequenceEqual(before), what + " unchanged after the write attempts");
    }

    // ------------------------------------------------------------------------------------------------ helpers

    private static MultiAreaContinuation NewEconomySession()
    {
        var (exp, p2, _) = MultiAreaDemoExperiments.CreateMixedDemo();
        return new MultiAreaContinuation(exp, MultiAreaModelSelection.Phase2WithEconomy, null, null, p2);
    }

    private static (MultiAreaContinuation Session, MultiAreaSessionView View) SessionWith(IEnumerable<string> treeIds)
    {
        var session = NewEconomySession();
        var v = session.ReadView();
        foreach (var id in treeIds)
            v = session.AddPending(Fence(v), Area, id, "disposition test").View;
        return (session, v);
    }

    private static (MultiAreaContinuation Session, MultiAreaSessionView View) MixedSession()
    {
        var (session, v) = SessionWith(ThreeStems);
        v = session.SetFellingDisposition(Fence(v), Area, SitkaTree, FellingDisposition.SellAndExtract).View;
        v = session.SetFellingDisposition(Fence(v), Area, OakTree, FellingDisposition.KeepForUse).View;
        v = session.SetFellingDisposition(Fence(v), Area, BeechTree, FellingDisposition.LeaveAsDeadwood).View;
        return (session, v);
    }

    private static MultiAreaSessionView SetAll(MultiAreaContinuation session, MultiAreaSessionView v, FellingDisposition d)
    {
        foreach (var order in Root(session).Pending)
            v = session.SetFellingDisposition(Fence(v), order.AreaId, order.TreeId, d).View;
        return v;
    }

    private static (EconomySettlementReceipt Receipt, MultiAreaSessionView View) ApproveAndExecute(MultiAreaContinuation session, MultiAreaSessionView v)
    {
        v = session.Approve(Fence(v)).View;
        var result = session.Execute(Fence(v), v.Approved!.PlanId, v.Approved.ExecutionKey);
        T.True(result.Execution is not null && !result.Execution.Replayed, "execute succeeded");
        return (result.Execution!.Settlement!, result.View);
    }

    private static IEnumerable<string> LivingIds(MultiAreaContinuation session) =>
        Root(session).Areas.Single(a => a.AreaId == Area).State.Trees.Where(t => t.IsLiving)
            .Select(t => t.TreeId).OrderBy(id => id, StringComparer.Ordinal).ToArray();

    private static AnnualTree LiveTree(MultiAreaContinuation session, string treeId) =>
        Root(session).Areas.Single(a => a.AreaId == Area).State.Trees.Single(t => t.TreeId == treeId);

    private static D062StemYield GradeLive(MultiAreaContinuation session, string treeId)
    {
        var root = Root(session);
        return D062TimberAssortmentEngine.GradeStem(LiveTree(session, treeId),
            root.Phase2?.SpeciesIdOf(treeId) ?? Phase2Species.SitkaId,
            root.Stage4?.PruningOf(treeId) ?? TreePruningState.Unpruned);
    }

    private static void AssertLedgerIs((EconomyLedgerCategory Category, string Reference, long Cents)[] expected,
        IReadOnlyList<EconomyLedgerEntry> actual, string what)
    {
        T.Eq(expected.Length, actual.Count, what + " count");
        for (int i = 0; i < expected.Length; i++)
        {
            T.Eq(expected[i].Category, actual[i].Category, what + " [" + i + "] category");
            T.Eq(expected[i].Reference, actual[i].ReferenceId, what + " [" + i + "] reference");
            T.Eq(expected[i].Cents, actual[i].CashDeltaCents, what + " [" + i + "] cents");
        }
    }

    private static bool Throws<TEx>(Action a) where TEx : Exception
    {
        try { a(); } catch (TEx) { return true; }
        return false;
    }

    /// <summary>
    /// Canonical, session-independent text of a disposition result (session ids and execution keys are excluded because
    /// they embed the per-session id), hashed with SHA-256. Floats are written as their exact IEEE-754 bit patterns.
    /// </summary>
    private static string Digest(EconomyWorkPlanQuote q, EconomySettlementReceipt? r)
    {
        var sb = new StringBuilder();
        static string F(float x) => BitConverter.SingleToInt32Bits(x).ToString("X8", CultureInfo.InvariantCulture);
        void Line(FormattableString s) => sb.Append(FormattableString.Invariant(s)).Append('\n');
        Line($"quote rev={q.TimberSaleRevenueCents} ext={q.TotalExternalCostCents} net={q.NetCashDeltaCents}");
        foreach (var (k, d) in q.FellDispositions) Line($"disp {k.AreaId}/{k.TargetKey}={d}");
        foreach (var e in q.EstimatedLedger) Line($"ledger {e.Category} {e.ReferenceId} {e.CashDeltaCents}");
        var h = q.HarvestJob!;
        Line($"job {h.Stage} {h.StageRateCentsPerTonne} {h.TotalStemVolumeCm3} {h.SaleVolumeCm3} {h.ResidualVolumeCm3} {h.RetainedStemVolumeCm3} {h.DeadwoodStemVolumeCm3} {h.HarvestedGreenGrams}");
        foreach (var l in h.StemDispositions)
            Line($"stem {l.AreaId}/{l.TreeId} {l.SpeciesId} {l.Disposition} {l.StemVolumeCm3} {l.HarvestedGreenGrams} {l.CreditedTimberRevenueCents} {l.RetainedTimberCm3} {l.EmitsDeadwoodIntent}");
        void Tally(string tag, RetainedTimberTally t)
        {
            Line($"{tag} {t.RetainedTimberCm3} {t.RetainedTimberM3}");
            foreach (var (sp, a) in t.BySpecies) Line($"{tag} {sp} {a.RetainedTimberCm3} {a.RetainedTimberM3}");
            foreach (var s in t.Stems) Line($"{tag} stem {s.AreaId}/{s.TreeId} {s.SpeciesId} {s.StemVolumeCm3} {s.EcologicalYear} {s.WorkRevision}");
        }
        void Intents(string tag, IReadOnlyList<FelledStemDeadwoodIntent> list)
        {
            foreach (var i in list)
                Line($"{tag} {i.AreaId}/{i.TreeId} {i.SpeciesId} {F(i.XM)} {F(i.ZM)} {F(i.DbhCm)} {F(i.HeightM)} {F(i.VolumeM3)} {i.EcologicalYear} {i.WorkRevision}");
        }
        Tally("qretained", h.RetainedTimber);
        Intents("qintent", h.DeadwoodIntents);
        if (r is not null)
        {
            Line($"settle {r.CashBeforeCents} {r.CashAfterCents} {r.NetCashDeltaCents} {r.EcologicalYear} {r.WorkRevision}");
            foreach (var e in r.LedgerEntries) Line($"sledger {e.Category} {e.ReferenceId} {e.CashDeltaCents} {e.WorkRevision}");
            Tally("sretained", r.RetainedTimberDelta);
            Intents("sintent", r.DeadwoodIntents);
            var (records, next) = FelledStemDeadwood.Materialise(r.DeadwoodIntents, 1);
            foreach (var d in records) Line($"record {d.DeadwoodId} {d.TreeId} {d.Cause} {F(d.EstimatedVolumeM3)}");
            Line($"next {next}");
        }
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(sb.ToString()))).ToLowerInvariant();
    }

    private static MultiAreaFence Fence(MultiAreaSessionView v) => new(v.SessionId, v.Source, v.PlanStateRevision);

    private static MultiAreaRoot Root(MultiAreaContinuation c) => (MultiAreaRoot)RootField.GetValue(c)!;
}
