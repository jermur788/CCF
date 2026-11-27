namespace SitkaP1.Core.NaturalProcesses.Tests;

internal static class SalvageAccountingTests
{
    private const string Suite = "salvage";

    private static readonly SalvagePricing Pricing = new(42.50m, 2500m);

    private static List<FallenDeadwoodRecord> Stock() => new()
    {
        new("DW-WINDTHROW-108-T0001", "T0001", "windthrow", 108, DeadwoodForm.Fallen, 24f, 30f),
        new("DW-WINDTHROW-108-T0002", "T0002", "windthrow", 108, DeadwoodForm.Fallen, 22f, 27f),
        new("DW-RETAINED-90-T0099", "T0099", "felled-retained", 90, DeadwoodForm.Fallen, 18f, 22f),
    };

    private static SalvageResult Apply(IReadOnlyList<SalvageOrder> orders, IReadOnlyList<string>? extracted = null, List<FallenDeadwoodRecord>? stock = null) =>
        SalvageAccounting.Apply(new SalvageRequest(stock ?? Stock(), extracted ?? Array.Empty<string>(), orders, Pricing));

    public static void Register()
    {
        TestRunner.Add(Suite, "JW-T40 no orders: nothing is salvaged automatically (default LeaveAsFallenDeadwood)", () =>
        {
            var r = Apply(Array.Empty<SalvageOrder>());
            TestRunner.True(r.Refusal is null, "ok");
            TestRunner.Eq(3, r.RemainingDeadwood.Count, "all deadwood remains");
            TestRunner.Eq(0, r.ExtractedDeadwoodIds.Count, "nothing extracted");
            TestRunner.Eq(0m, r.GrossSaleRevenue, "no revenue");
            TestRunner.Eq(SalvageChoice.LeaveAsFallenDeadwood, default(SalvageChoice), "enum default is leave");
        });

        TestRunner.Add(Suite, "JW-T41 SellAndExtract/KeepForUse without an approved Work Plan are refused and change nothing", () =>
        {
            foreach (var choice in new[] { SalvageChoice.SellAndExtract, SalvageChoice.KeepForUse })
            foreach (var plan in new string?[] { null, "", "  " })
            {
                var r = Apply(new[] { new SalvageOrder("DW-WINDTHROW-108-T0001", choice, plan, 1.2) });
                TestRunner.Eq(NaturalProcessRefusalCode.SalvageRequiresApprovedWorkPlan, r.Refusal?.Code ?? default, "code");
                TestRunner.Eq(3, r.RemainingDeadwood.Count, "unchanged");
                TestRunner.Eq(0, r.ExtractedDeadwoodIds.Count, "nothing extracted");
            }
        });

        TestRunner.Add(Suite, "JW-T42 SellAndExtract earns revenue at the unchanged price; pricing echoed unchanged", () =>
        {
            var r = Apply(new[] { new SalvageOrder("DW-WINDTHROW-108-T0001", SalvageChoice.SellAndExtract, "WP-0007", 1.2) });
            TestRunner.True(r.Refusal is null, "ok");
            TestRunner.Eq(51.00m, r.GrossSaleRevenue, "1.2 m3 x 42.50");
            TestRunner.Eq(1.2, r.SoldVolumeM3, "sold");
            TestRunner.Eq(0.0, r.RetainedTimberM3, "no retained");
            TestRunner.Eq(Pricing, r.PricingApplied, "price and contractor minimum unchanged");
            TestRunner.Eq(1.00m, SalvageAccounting.PriceMultiplier, "multiplier 1.00");
            TestRunner.True(!r.RemainingDeadwood.Any(d => d.DeadwoodId == "DW-WINDTHROW-108-T0001"), "record removed");
            TestRunner.Eq("DW-WINDTHROW-108-T0001", r.ExtractedDeadwoodIds.Single(), "extracted id recorded");
            TestRunner.Eq("WP-0007", r.Lines.Single().ApprovedWorkPlanId, "plan reference kept");
        });

        TestRunner.Add(Suite, "JW-T43 KeepForUse yields retained timber and no revenue", () =>
        {
            var r = Apply(new[] { new SalvageOrder("DW-WINDTHROW-108-T0002", SalvageChoice.KeepForUse, "WP-0008", 0.9) });
            TestRunner.True(r.Refusal is null, "ok");
            TestRunner.Eq(0m, r.GrossSaleRevenue, "no revenue");
            TestRunner.Eq(0m, r.Lines.Single().SaleRevenue, "line revenue");
            TestRunner.Eq(0.9, r.RetainedTimberM3, "retained");
            TestRunner.Eq(0.0, r.SoldVolumeM3, "not sold");
        });

        TestRunner.Add(Suite, "JW-T44 no duplicate deadwood after extraction (re-order and re-merge both refused)", () =>
        {
            var first = Apply(new[] { new SalvageOrder("DW-WINDTHROW-108-T0001", SalvageChoice.SellAndExtract, "WP-1", 1.0) });
            var reorder = SalvageAccounting.Apply(new SalvageRequest(first.RemainingDeadwood.ToList(), first.ExtractedDeadwoodIds,
                new[] { new SalvageOrder("DW-WINDTHROW-108-T0001", SalvageChoice.SellAndExtract, "WP-2", 1.0) }, Pricing));
            TestRunner.Eq(NaturalProcessRefusalCode.DeadwoodAlreadyExtracted, reorder.Refusal?.Code ?? default, "re-order");
            TestRunner.Eq(0m, reorder.GrossSaleRevenue, "no second revenue");

            var reappear = SalvageAccounting.MergeWindthrowDeadwood(first.RemainingDeadwood, first.ExtractedDeadwoodIds,
                new[] { Stock()[0] });
            TestRunner.Eq(NaturalProcessRefusalCode.DeadwoodAlreadyExtracted, reappear.Refusal?.Code ?? default, "extracted stem cannot reappear");
            TestRunner.Eq(first.RemainingDeadwood.Count, reappear.Deadwood.Count, "stock unchanged");

            var dup = SalvageAccounting.MergeWindthrowDeadwood(Stock(), Array.Empty<string>(), new[] { Stock()[1] });
            TestRunner.Eq(NaturalProcessRefusalCode.DuplicateDeadwoodId, dup.Refusal?.Code ?? default, "duplicate victim deadwood refused");

            var stockWithExtracted = SalvageAccounting.Apply(new SalvageRequest(Stock(), new[] { "DW-WINDTHROW-108-T0001" }, Array.Empty<SalvageOrder>(), Pricing));
            TestRunner.Eq(NaturalProcessRefusalCode.DeadwoodAlreadyExtracted, stockWithExtracted.Refusal?.Code ?? default, "inconsistent stock refused");
        });

        TestRunner.Add(Suite, "JW-T45 storm victims merge once into the deadwood stock", () =>
        {
            var trees = Fixtures.StormTrees(400);
            var storm = StormAdapter.Resolve(new StormRequest(StormModelVersion.WindthrowV1, 108, Fixtures.Seed, trees));
            var added = storm.Victims.Select(v => v.Deadwood).ToArray();
            var merged = SalvageAccounting.MergeWindthrowDeadwood(Array.Empty<FallenDeadwoodRecord>(), Array.Empty<string>(), added);
            TestRunner.True(merged.Refusal is null && merged.Deadwood.Count == storm.Victims.Count, "one record per victim");
            var again = SalvageAccounting.MergeWindthrowDeadwood(merged.Deadwood, Array.Empty<string>(), added);
            TestRunner.Eq(NaturalProcessRefusalCode.DuplicateDeadwoodId, again.Refusal?.Code ?? default, "second merge refused");
        });

        TestRunner.Add(Suite, "JW-T46 other refusals: explicit leave, non-windthrow target, duplicate orders, unknown id, bad volume", () =>
        {
            var leave = Apply(new[] { new SalvageOrder("DW-WINDTHROW-108-T0001", SalvageChoice.LeaveAsFallenDeadwood, null, 0) });
            TestRunner.True(leave.Refusal is null && leave.RemainingDeadwood.Count == 3 && leave.ExtractedDeadwoodIds.Count == 0, "leave needs no plan");
            TestRunner.Eq(NaturalProcessRefusalCode.SalvageTargetNotWindthrow,
                Apply(new[] { new SalvageOrder("DW-RETAINED-90-T0099", SalvageChoice.SellAndExtract, "WP-1", 1.0) }).Refusal?.Code ?? default, "not windthrow");
            TestRunner.Eq(NaturalProcessRefusalCode.DuplicateSalvageOrder, Apply(new[]
            {
                new SalvageOrder("DW-WINDTHROW-108-T0001", SalvageChoice.SellAndExtract, "WP-1", 1.0),
                new SalvageOrder("DW-WINDTHROW-108-T0001", SalvageChoice.KeepForUse, "WP-1", 1.0),
            }).Refusal?.Code ?? default, "duplicate order");
            TestRunner.Eq(NaturalProcessRefusalCode.UnknownDeadwood,
                Apply(new[] { new SalvageOrder("DW-NOPE", SalvageChoice.SellAndExtract, "WP-1", 1.0) }).Refusal?.Code ?? default, "unknown");
            TestRunner.Eq(NaturalProcessRefusalCode.InvalidSalvageVolume,
                Apply(new[] { new SalvageOrder("DW-WINDTHROW-108-T0001", SalvageChoice.SellAndExtract, "WP-1", double.NaN) }).Refusal?.Code ?? default, "volume");
        });

        TestRunner.Add(Suite, "JW-T47 dated non-causal teaching note covers Darragh and Eowyn and makes no unsourced market claim", () =>
        {
            string note = NaturalProcessText.WindblowMarketTeachingNote;
            foreach (string needle in new[] { "Storm Darragh", "December 2024", "Storm Eowyn", "January 2025", "not a forecast or causal model",
                         "written November 2026", "does not model", "makes no claim" })
                TestRunner.True(note.Contains(needle, StringComparison.Ordinal), "note mentions: " + needle);
            foreach (string banned in new[] { "2026 timber market", "was part of the" })
                TestRunner.True(!note.Contains(banned, StringComparison.Ordinal), "note must not assert: " + banned);
        });

        TestRunner.Add(Suite, "JW-T48 merge refuses an extracted ID already in the existing stock, even with a valid new addition", () =>
        {
            var d1 = new FallenDeadwoodRecord("D1", "T1", "windthrow", 108, DeadwoodForm.Fallen, 20f, 25f);
            var d2 = new FallenDeadwoodRecord("D2", "T2", "windthrow", 109, DeadwoodForm.Fallen, 21f, 26f);
            var r = SalvageAccounting.MergeWindthrowDeadwood(new[] { d1 }, new[] { "D1" }, new[] { d2 });
            TestRunner.Eq(NaturalProcessRefusalCode.DeadwoodAlreadyExtracted, r.Refusal?.Code ?? default, "code");
            TestRunner.Eq("D1", string.Join(",", r.Deadwood.Select(d => d.DeadwoodId)), "atomic: existing stock returned unchanged, D2 not added");
            var ok = SalvageAccounting.MergeWindthrowDeadwood(Array.Empty<FallenDeadwoodRecord>(), new[] { "D1" }, new[] { d2 });
            TestRunner.True(ok.Refusal is null, "same extracted list with a clean stock merges");
            TestRunner.Eq("D2", string.Join(",", ok.Deadwood.Select(d => d.DeadwoodId)), "D1 never reappears");
        });

        TestRunner.Add(Suite, "JW-T49 pricing beyond the representable bound is refused atomically (no decimal overflow)", () =>
        {
            var sell = new[] { new SalvageOrder("DW-WINDTHROW-108-T0001", SalvageChoice.SellAndExtract, "WP-1", 2.0) };
            foreach (var pricing in new[]
            {
                new SalvagePricing(decimal.MaxValue, 2500m), new SalvagePricing(SalvageAccounting.MaxMoneyAmount + 0.01m, 2500m),
                new SalvagePricing(42.50m, decimal.MaxValue), new SalvagePricing(-0.01m, 2500m),
            })
            {
                var r = SalvageAccounting.Apply(new SalvageRequest(Stock(), Array.Empty<string>(), sell, pricing));
                TestRunner.Eq(NaturalProcessRefusalCode.InvalidSalvagePricing, r.Refusal?.Code ?? default, "code for " + pricing);
                TestRunner.Eq(3, r.RemainingDeadwood.Count, "unchanged stock");
                TestRunner.Eq(0, r.ExtractedDeadwoodIds.Count + r.Lines.Count, "nothing extracted, no lines");
                TestRunner.Eq(0m, r.GrossSaleRevenue, "no revenue");
            }
            var stock = Enumerable.Range(0, 50).Select(i => new FallenDeadwoodRecord("DW-" + i, "T" + i, "windthrow", 108, DeadwoodForm.Fallen, 20f, 25f)).ToList();
            var orders = stock.Select(d => new SalvageOrder(d.DeadwoodId, SalvageChoice.SellAndExtract, "WP-1", SalvageAccounting.MaxRecoverableVolumeM3)).ToArray();
            var max = SalvageAccounting.Apply(new SalvageRequest(stock, Array.Empty<string>(), orders,
                new SalvagePricing(SalvageAccounting.MaxMoneyAmount, SalvageAccounting.MaxMoneyAmount)));
            TestRunner.True(max.Refusal is null, "bound itself accepted");
            TestRunner.Eq(50m * 1_000m * 1_000_000_000m, max.GrossSaleRevenue, "maximal total is exact");
        });

        TestRunner.Add(Suite, "JW-T50 salvage request/result/merge collections are snapshots: input mutation and output writes change nothing", () =>
        {
            var stock = Stock().ToArray();
            var extracted = new[] { "DW-OTHER" };
            var orders = new[] { new SalvageOrder("DW-WINDTHROW-108-T0001", SalvageChoice.SellAndExtract, "WP-1", 1.0) };
            var request = new SalvageRequest(stock, extracted, orders, Pricing);
            stock[0] = stock[2];
            extracted[0] = "DW-WINDTHROW-108-T0001";
            orders[0] = new SalvageOrder("DW-WINDTHROW-108-T0002", SalvageChoice.KeepForUse, "WP-9", 5.0);
            Fixtures.AssertReadOnly(request.Deadwood, "request.Deadwood");
            Fixtures.AssertReadOnly(request.PreviouslyExtractedDeadwoodIds, "request.PreviouslyExtractedDeadwoodIds");
            Fixtures.AssertReadOnly(request.Orders, "request.Orders");
            var r = SalvageAccounting.Apply(request);
            TestRunner.True(r.Refusal is null, "snapshot request still valid");
            TestRunner.Eq(42.50m, r.GrossSaleRevenue, "original order applied");
            TestRunner.Eq("DW-OTHER,DW-WINDTHROW-108-T0001", string.Join(",", r.ExtractedDeadwoodIds), "original extracted list used");
            Fixtures.AssertReadOnly(r.RemainingDeadwood, "RemainingDeadwood");
            Fixtures.AssertReadOnly(r.Lines, "Lines");
            Fixtures.AssertReadOnly(r.ExtractedDeadwoodIds, "ExtractedDeadwoodIds");
            TestRunner.True(r.ExtractedDeadwoodIds as string[] is null, "extracted IDs cannot be cast to string[]");
            TestRunner.Eq(2, r.RemainingDeadwood.Count, "remaining unchanged by write attempts");

            var added = new[] { new FallenDeadwoodRecord("DW-NEW", "TN", "windthrow", 110, DeadwoodForm.Fallen, 20f, 25f) };
            var merged = SalvageAccounting.MergeWindthrowDeadwood(r.RemainingDeadwood, r.ExtractedDeadwoodIds, added);
            added[0] = added[0] with { DeadwoodId = "DW-MUTATED" };
            Fixtures.AssertReadOnly(merged.Deadwood, "merge Deadwood");
            TestRunner.True(merged.Deadwood.Any(d => d.DeadwoodId == "DW-NEW") && merged.Deadwood.All(d => d.DeadwoodId != "DW-MUTATED"), "merge result snapshot");
        });
    }
}
