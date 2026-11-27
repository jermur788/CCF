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

        TestRunner.Add(Suite, "JW-T47 dated non-causal teaching note covers Darragh, Eowyn and the 2026 windblow market", () =>
        {
            string note = NaturalProcessText.WindblowMarketTeachingNote;
            foreach (string needle in new[] { "Storm Darragh", "December 2024", "Storm Eowyn", "January 2025", "2026", "not a forecast or causal model", "written November 2026" })
                TestRunner.True(note.Contains(needle, StringComparison.Ordinal), "note mentions: " + needle);
        });
    }
}
