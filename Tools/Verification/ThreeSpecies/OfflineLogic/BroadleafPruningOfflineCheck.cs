using System;
using System.Collections.Generic;
using System.Linq;
using CCF.Forestry.TimberYield;
using CCF.Forestry.WorkEconomy;

// Real pruning method bodies + definition + economy adapter; fixture-only world/visual collaborators.
public static class BroadleafPruningOfflineCheck
{
    private static int checks, failures;
    private static void Check(bool ok, string message)
    {
        checks++;
        if (!ok) { failures++; Console.WriteLine("FAIL " + message); }
    }
    private static ForestTree Tree(string species, string id = "tree") => new ForestTree
        { species = new TreeSpeciesDefinition { SpeciesId = species }, TreeId = id };
    private static bool Near(float a, float b) => Math.Abs(a - b) < 0.000001f;

    public static int Main()
    {
        var definition = new ScenarioOneDefinition();
        foreach (string species in new[] { "sessile-oak", "beech" })
        {
            var tree = Tree(species);
            Check(definition.NextPruningTargetHeightM(tree, true) == 3.5f, species + " first target");
            Check(tree.TryPrune(2.5f, 0, true) != null && tree.PruningLifts == 0, "wrong first target rejected without mutation");
            tree.Height = 3.5f / 0.6f;
            Check(tree.CanPrune(3.5f, 0, true) != null, "60 percent height boundary excluded");
            tree.Height = 24f;
            Check(tree.TryPrune(3.5f, 0, true) == null, "first high-pruning lift");
            Check(tree.PruningLifts == 1 && tree.CrownBaseHeightM == 3.5f && tree.LastPruningYear == 0
                && Near(tree.CrownRadius, 5f * 0.92f) && Near(tree.PrunedCrownTargetFactor, 0.92f), "first lift state/radius");
            Check(definition.NextPruningTargetHeightM(tree, true) == 6f, "second target");
            Check(tree.TryPrune(6f, 0, true) != null && tree.TryPrune(6f, 2, true) != null
                && tree.PruningLifts == 1 && tree.LastPruningYear == 0, "same year and year two reject without mutation");
            Check(tree.TryPrune(6f, 3, true) == null, "exact year-three interval is eligible");
            Check(tree.PruningLifts == 2 && tree.CrownBaseHeightM == 6f && tree.LastPruningYear == 3
                && Near(tree.CrownRadius, 5f * 0.92f * 0.92f) && Near(tree.PrunedCrownTargetFactor, 0.92f * 0.92f), "second lift state/radius");
            Check(definition.NextPruningTargetHeightM(tree, true) < 0 && tree.TryPrune(6.5f, 6, true) != null, "no third broadleaf lift");
            var restored = Tree(species);
            restored.RestorePruningHistory(tree.PruningLifts, tree.CrownBaseHeightM, tree.LastPruningYear);
            Check(restored.PruningLifts == 2 && restored.CrownBaseHeightM == 6f && restored.LastPruningYear == 3
                && restored.CanPrune(6.5f, 10, true) != null, "existing history fields restore cap");
            restored.RestorePruningHistory(1, 2.5f, 0);
            Check(definition.NextPruningTargetHeightM(restored, true) == 6f && restored.CanPrune(6f, 3, true) == null,
                "existing one-lift model2 history continues to second high-pruning lift");
            restored.RestorePruningHistory(2, 5f, 0);
            Check(definition.NextPruningTargetHeightM(restored, true) < 0 && restored.CanPrune(6f, 3, true) != null,
                "existing two-lift model2 history cannot acquire a third treatment");
            restored.RestorePruningHistory(3, 6.5f, 0);
            Check(definition.NextPruningTargetHeightM(restored, true) < 0 && restored.PruningLifts == 3 && restored.CrownBaseHeightM == 6.5f,
                "existing three-lift history preserved, not clamped or reinterpreted");
            var dead = Tree(species); dead.CanChop = false;
            Check(dead.TryPrune(3.5f, 0, true) != null && dead.PruningLifts == 0, "ineligible tree cannot prune");
            foreach (float target in new[] { float.NaN, float.PositiveInfinity, 0f, -1f, 14.4f })
                Check(Tree(species).TryPrune(target, 0, true) != null, "invalid/height-capped target rejected");
        }
        LegacyDifferential(definition);
        Adapter(definition);
        Console.WriteLine($"PARITY06_PRUNING_{(failures == 0 ? "PASS" : "FAIL")} checks={checks} failures={failures} NOT_UNITY_VERIFIED");
        return failures == 0 ? 0 : 1;
    }

    private static void LegacyDifferential(ScenarioOneDefinition definition)
    {
        int compared = 0;
        foreach (string species in new[] { "sitka-spruce", "sessile-oak", "beech", "unregistered-species" })
        foreach (bool gate in new[] { false, true })
        {
            if (gate && (species == "sessile-oak" || species == "beech")) continue;
            foreach (int lifts in new[] { 0, 1, 2, 3 })
            foreach (int elapsed in new[] { 0, 2, 3, 4, 5, 6 })
            foreach (float target in new[] { -1f, 0.5f, 2.5f, 3.5f, 5f, 6f, 6.5f, 14.4f, float.NaN })
            {
                var current = Tree(species);
                var legacy = new LegacyForestTree { species = current.Species };
                float crownBase = lifts == 0 ? 0f : lifts == 1 ? 2.5f : lifts == 2 ? 5f : 6.5f;
                current.RestorePruningHistory(lifts, crownBase, lifts == 0 ? -1 : 0);
                legacy.RestorePruningHistory(lifts, crownBase, lifts == 0 ? -1 : 0);
                Check(definition.NextPruningTargetHeightM(current, gate) == definition.NextPruningTargetHeightM(lifts), "legacy target equality");
                Check(current.TryPrune(target, elapsed, gate) == legacy.TryPrune(target, elapsed), "legacy rejection equality");
                Check(current.PruningLifts == legacy.PruningLifts && current.LastPruningYear == legacy.LastPruningYear
                    && current.CrownBaseHeightM == legacy.CrownBaseHeightM && current.CrownRadius == legacy.CrownRadius
                    && current.PrunedCrownTargetFactor == legacy.PrunedCrownTargetFactor, "legacy mutation equality");
                compared++;
            }
        }
        Check(definition.PruningLiftTargetHeightsM.SequenceEqual(new[] { 2.5f, 5f, 6.5f }), "Sitka target ladder exact");
        Check(Tree("sessile-oak").TryPrune(0.5f, 0) == null, "two-argument API remains legacy");
        Console.WriteLine($"PARITY06_LEGACY_DIFFERENTIAL_PASS cases={compared} (Sitka gate off/on; all species gate off; unknown species no broadleaf inheritance)");
    }

    private static ScenarioHarvestJob Quote(ScenarioOneDefinition definition, bool market, params ForestTree[] trees)
    {
        var orders = trees.Select((t, i) => new ScenarioOneWorkOrder { workOrderId = i + 1, targetTreeId = t.TreeId,
            type = ScenarioWorkType.FellTree, executionMethod = WorkExecutionMethod.Contractor,
            fellingOutcome = FellingMaterialOutcome.SellAndExtract }).ToArray();
        return ScenarioOneEconomyAdapter.QuoteHarvest(orders, trees.ToDictionary(t => t.TreeId), definition, 10, 0, 10000000, broadleafMarket: market);
    }

    private static void Adapter(ScenarioOneDefinition definition)
    {
        Check(definition.TimberValueCentsPerCubicMetre("sitka-spruce") == 7200, "dormant Sitka value preserved");
        foreach (string species in new[] { "sessile-oak", "beech", "unknown", "", null })
        {
            bool rejected = false;
            try { definition.TimberValueCentsPerCubicMetre(species); } catch (InvalidOperationException) { rejected = true; }
            Check(rejected, "flat broadleaf/unknown value blocked");
        }
        var oak = Tree("sessile-oak", "oak"); var beech = Tree("beech", "beech"); var sitka = Tree("sitka-spruce", "sitka");
        var off = Quote(definition, false, oak, beech, sitka);
        var on = Quote(definition, true, oak, beech, sitka);
        Check(off.Eligible && on.Eligible && off.HasUnmarketedSpecies && !on.HasUnmarketedSpecies, "adapter market opt-in");
        Check(off.Yield.Stems.Where(s => s.SpeciesId != "sitka-spruce").All(s => s.Logs.Length == 0), "old models broadleaf unmarketed");
        Check(on.CostCents == off.CostCents && on.Task.SiteCostBasisPoints == 10000 && off.Task.SiteCostBasisPoints == 10000,
            "adapter keeps same whole-stem generic cost with no broadleaf multiplier");
        long Sold(ScenarioHarvestJob q, string species) => q.Resolution.Quote.Timber.Where(t => t.Batch.SpeciesId == species).Sum(t => t.SaleRevenueCents);
        Check(Sold(on, "sitka-spruce") == Sold(off, "sitka-spruce") && Sold(on, "sessile-oak") > 0 && Sold(on, "beech") > 0,
            "adapter species-explicit revenue and Sitka preserved");
        long before = Sold(on, "sessile-oak");
        Check(oak.TryPrune(3.5f, 0, true) == null && oak.TryPrune(6f, 3, true) == null, "prune real method before grading");
        var pruned = Quote(definition, true, oak, beech, sitka);
        var sections = ScenarioOneEconomyAdapter.PrunedSections(oak, 24000);
        Check(sections.Length == 1 && sections[0].EndHeightMm == 6000 && sections[0].Flags == StemQualityFlags.Pruned,
            "adapter quality is exactly recorded pruning section");
        Check(pruned.Yield.Stems.Single(s => s.TreeId == "oak").Logs.Count(l => l.Assortment == TimberAssortment.VeneerPlanking) == 2
            && Sold(pruned, "sessile-oak") > before, "two high lifts reach two covered veneer logs on synthetic large Oak");
        Check(pruned.Yield.Stems.Single(s => s.TreeId == "beech").Logs.All(l => l.Assortment == TimberAssortment.SecondPlanking || l.Assortment == TimberAssortment.Firewood), "Beech remains two-rung");
        var settled = ScenarioOneEconomyAdapter.ReResolveHarvest(pruned, definition, 10000000);
        Check(settled.Resolved && settled.Quote.TimberSaleRevenueCents == pruned.RevenueCents
            && ForestryWorkCalculator.SumLedger(settled.Ledger) == settled.Quote.NetCashCents, "re-resolution uses same book and ledger conserves cash");
        Check(pruned.TotalStemVolumeCm3 == pruned.SoldVolumeCm3 + pruned.RetainedVolumeCm3 + pruned.DeadwoodVolumeCm3 + pruned.ResidualVolumeCm3,
            "adapter stem material conservation");
        Console.WriteLine($"PARITY06_ADAPTER_MEASUREMENT unprunedOakCents={before} prunedOakCents={Sold(pruned, "sessile-oak")} totalCostCents={pruned.CostCents} (synthetic fixture, not a completion/solvency anchor)");
    }
}
