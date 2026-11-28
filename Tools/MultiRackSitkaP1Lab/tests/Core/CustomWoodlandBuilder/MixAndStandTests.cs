using SitkaP1.Core.SpeciesWave2;

namespace SitkaP1.Core.CustomWoodlandBuilder.Tests;

internal static class MixAndStandTests
{
    public static void Register()
    {
        TestRunner.Add("Mix", "MIX-01 largest-remainder allocation is exact and sums to the target", AllocationExact);
        TestRunner.Add("Mix", "MIX-02 bad mixes are refused", BadMixesRefused);
        TestRunner.Add("Mix", "MIX-03 start age and stocking limits are enforced at both ends", AgeAndStockingLimits);
        TestRunner.Add("Mix", "MIX-04 every six-species mix member appears in a large stand", AllSixSpeciesPlaced);
        TestRunner.Add("Stand", "GEN-01 the planted count is exact and every tree is inside the field", ExactPlantedCount);
        TestRunner.Add("Stand", "GEN-02 every tree sits in the lane it was assigned", TreesInLanes);
        TestRunner.Add("Stand", "GEN-03 edge seed trees are mature, flagged and clear of the boundary", EdgeSeedTrees);
        TestRunner.Add("Stand", "GEN-04 a perimeter too short for the edge trees is refused", EdgeSeedTreesRefused);
        TestRunner.Add("Stand", "GEN-05 the same seed reproduces the stand; a new seed changes it", SeedDeterminism);
        TestRunner.Add("Stand", "GEN-06 trees on bog cells carry the habitat caution", BogTreesFlagged);
        TestRunner.Add("Stand", "CAP-01 a target above the living-tree cap is refused", CapRefused);
        TestRunner.Add("Stand", "CAP-02 a stand at 2,500 trees reports the near-cap warning", NearCapReported);
    }

    private static void AllocationExact()
    {
        int[] exact = CustomWoodlandStandGenerator.AllocateCounts(Fixtures.StandardMix, 1000);
        TestRunner.Eq(500, exact[0], "50 percent of 1000");
        TestRunner.Eq(200, exact[1], "20 percent of 1000");
        TestRunner.Eq(100, exact[2], "10 percent of 1000");
        TestRunner.Eq(100, exact[3], "10 percent of 1000");
        TestRunner.Eq(50, exact[4], "5 percent of 1000");
        TestRunner.Eq(50, exact[5], "5 percent of 1000");

        int[] rounded = CustomWoodlandStandGenerator.AllocateCounts(Fixtures.StandardMix, 7);
        TestRunner.Eq(7, rounded.Sum(), "the allocation still sums to the target");
        TestRunner.Eq(4, rounded[0], "the largest weight receives the first remainder");
        TestRunner.Eq(1, rounded[1], "20 percent of 7 rounds up once");
        TestRunner.Eq(1, rounded[2], "the first 10 percent tie wins by species order");
        TestRunner.Eq(1, rounded[3], "the second 10 percent tie follows");
        TestRunner.Eq(0, rounded[4], "5 percent of 7 receives nothing");
        TestRunner.Eq(0, rounded[5], "5 percent of 7 receives nothing");
    }

    private static void BadMixesRefused()
    {
        TestRunner.True(
            CustomWoodlandBuilder.Validate(Fixtures.Recipe(mix: Fixtures.Mix(
                (SpeciesMoistureTable.SitkaSpruce, 50.0),
                (SpeciesMoistureTable.SessileOak, 20.0),
                (SpeciesMoistureTable.EuropeanBeech, 10.0),
                (SpeciesMoistureTable.ScotsPine, 10.0),
                (SpeciesMoistureTable.DownyBirch, 5.0),
                (SpeciesMoistureTable.CommonAlder, 4.9)))).Count > 0,
            "99.9 percent is refused");

        TestRunner.True(
            CustomWoodlandBuilder.Validate(Fixtures.Recipe(mix: Fixtures.Mix(
                (SpeciesMoistureTable.SitkaSpruce, 50.0),
                (SpeciesMoistureTable.SessileOak, 20.0),
                (SpeciesMoistureTable.EuropeanBeech, 10.0),
                (SpeciesMoistureTable.ScotsPine, 10.0),
                (SpeciesMoistureTable.DownyBirch, 10.0)))).Count > 0,
            "a five-species mix is refused");

        TestRunner.True(
            CustomWoodlandBuilder.Validate(Fixtures.Recipe(mix: Fixtures.Mix(
                (SpeciesMoistureTable.SitkaSpruce, 50.0),
                (SpeciesMoistureTable.SitkaSpruce, 20.0),
                (SpeciesMoistureTable.EuropeanBeech, 10.0),
                (SpeciesMoistureTable.ScotsPine, 10.0),
                (SpeciesMoistureTable.DownyBirch, 5.0),
                (SpeciesMoistureTable.CommonAlder, 5.0)))).Count > 0,
            "a duplicated species is refused");

        TestRunner.True(
            CustomWoodlandBuilder.Validate(Fixtures.Recipe(mix: Fixtures.Mix(
                (SpeciesMoistureTable.SitkaSpruce, 50.0),
                (SpeciesMoistureTable.SessileOak, 20.0),
                (SpeciesMoistureTable.EuropeanBeech, 10.0),
                (SpeciesMoistureTable.ScotsPine, 10.0),
                (SpeciesMoistureTable.DownyBirch, 15.0),
                ("douglas-fir", 5.0)))).Count > 0,
            "an unaccepted species is refused");

        TestRunner.True(
            CustomWoodlandBuilder.Validate(Fixtures.Recipe(mix: Fixtures.Mix(
                (SpeciesMoistureTable.SitkaSpruce, 110.0),
                (SpeciesMoistureTable.SessileOak, -20.0),
                (SpeciesMoistureTable.EuropeanBeech, 10.0),
                (SpeciesMoistureTable.ScotsPine, 0.0),
                (SpeciesMoistureTable.DownyBirch, 0.0),
                (SpeciesMoistureTable.CommonAlder, 0.0)))).Count > 0,
            "a negative weight is refused");
    }

    private static void AgeAndStockingLimits()
    {
        TestRunner.True(CustomWoodlandBuilder.Validate(Fixtures.Recipe(startAge: 10)).Count == 0,
            "10 years is the youngest accepted start age");
        TestRunner.True(CustomWoodlandBuilder.Validate(Fixtures.Recipe(startAge: 60)).Count == 0,
            "60 years is the oldest accepted start age");
        TestRunner.True(CustomWoodlandBuilder.Validate(Fixtures.Recipe(startAge: 9)).Count > 0,
            "9 years is refused");
        TestRunner.True(CustomWoodlandBuilder.Validate(Fixtures.Recipe(startAge: 61)).Count > 0,
            "61 years is refused");

        TestRunner.True(CustomWoodlandBuilder.Validate(Fixtures.Recipe(stocking: 500)).Count == 0,
            "500 stems per hectare is accepted");
        TestRunner.True(CustomWoodlandBuilder.Validate(Fixtures.Recipe(stocking: 3000)).Count == 0,
            "3,000 stems per hectare is accepted on an 0.48 ha field");
        TestRunner.True(CustomWoodlandBuilder.Validate(Fixtures.Recipe(stocking: 499)).Count > 0,
            "499 stems per hectare is refused");
        TestRunner.True(CustomWoodlandBuilder.Validate(Fixtures.Recipe(stocking: 3001)).Count > 0,
            "3,001 stems per hectare is refused");
    }

    private static void AllSixSpeciesPlaced()
    {
        CustomWoodlandBuild build = CustomWoodlandBuilder.Build(Fixtures.Recipe(stocking: 2000));
        var seen = new HashSet<string>(StringComparer.Ordinal);
        foreach (GeneratedTree tree in build.Stand.Trees)
            seen.Add(tree.SpeciesId);

        TestRunner.Eq(6, seen.Count, "a large mixed stand contains all six species");
        TestRunner.True(seen.SetEquals(SpeciesMoistureTable.SpeciesIds), "the species are the accepted table ids");
    }

    private static void ExactPlantedCount()
    {
        CustomWoodlandBuild build = CustomWoodlandBuilder.Build(Fixtures.Recipe(stocking: 1500));
        WoodlandPolygon polygon = build.Boundary;

        TestRunner.Eq(720, build.Stand.PlantedStemCount, "1,500 stems per hectare on 0.48 ha is 720 stems");
        TestRunner.Eq(720, build.Stand.Trees.Count, "no edge trees were requested");
        TestRunner.Eq(720, build.Stand.LivingTreeCount, "living count matches");
        TestRunner.ApproxEq(1500.0, build.Stand.StemsPerHectareAchieved, 1e-6, "achieved stocking");
        TestRunner.True(!build.Stand.NearSafetyCap, "720 trees is nowhere near the cap");

        foreach (GeneratedTree tree in build.Stand.Trees)
        {
            TestRunner.True(polygon.ContainsPoint(tree.Position), "every tree is inside the boundary");
            TestRunner.True(!tree.IsEdgeSeedTree, "no tree is flagged as an edge seed tree");
            TestRunner.Eq(25, tree.AgeYears, "planted trees start at the recipe age");
            TestRunner.True(tree.Moisture != SiteMoistureClass.NeutralLegacy, "every tree has a real moisture class");
        }
    }

    private static void TreesInLanes()
    {
        CustomWoodlandBuild build = CustomWoodlandBuilder.Build(Fixtures.Recipe(laneCount: 5, stocking: 1500));

        foreach (GeneratedTree tree in build.Stand.Trees)
        {
            TestRunner.True(tree.LaneIndex >= 0 && tree.LaneIndex < build.Lanes.LaneCount,
                "the lane index is in range");
            WorkLane lane = build.Lanes.Lanes[tree.LaneIndex];
            TestRunner.True(Fixtures.RingContains(lane.Boundary, tree.Position),
                $"tree {tree.Index} sits inside its assigned lane {tree.LaneIndex}");
            TestRunner.Eq(tree.LaneIndex, build.Lanes.LaneIndexAt(tree.Position),
                "the lane index agrees with the band arithmetic");
        }
    }

    private static void EdgeSeedTrees()
    {
        CustomWoodlandBuild build = CustomWoodlandBuilder.Build(
            Fixtures.Recipe(edgeSeedTrees: 12, stocking: 1500));

        TestRunner.Eq(732, build.Stand.Trees.Count, "720 planted plus 12 edge seed trees");
        TestRunner.Eq(12, build.Stand.EdgeSeedTreeCount, "edge count is reported");

        int flagged = 0;
        foreach (GeneratedTree tree in build.Stand.Trees)
        {
            if (!tree.IsEdgeSeedTree)
                continue;

            flagged++;
            TestRunner.Eq(60, tree.AgeYears, "edge seed trees are mature");
            TestRunner.True(tree.Position.IsFinite, "edge tree coordinates are finite");
            TestRunner.True(
                build.Boundary.ContainsWithClearance(tree.Position, CustomWoodlandStandGenerator.EdgeSeedTreeClearanceM),
                "edge seed trees keep at least 0.5 m clear of every edge");
            TestRunner.True(
                tree.SpeciesId is not null && SpeciesMoistureTable.SpeciesIds.Contains(tree.SpeciesId),
                "edge seed trees use an accepted species");
        }

        TestRunner.Eq(12, flagged, "every requested edge tree was placed and flagged");
    }

    private static void EdgeSeedTreesRefused()
    {
        CustomWoodlandRecipe recipe = Fixtures.Recipe(
            boundary: WoodlandShapeSpec.Rectangle(30.0, 30.0),
            stocking: 800,
            edgeSeedTrees: 200);

        IReadOnlyList<string> errors = CustomWoodlandBuilder.Validate(recipe);
        TestRunner.True(errors.Count > 0, "a 30 x 30 m perimeter cannot seat 200 edge trees");
        TestRunner.True(
            errors.Any(error => error.Contains("edge seed trees", StringComparison.OrdinalIgnoreCase)),
            "the message names the edge seed trees");
        TestRunner.Throws<ArgumentException>(
            () => CustomWoodlandBuilder.Build(recipe), "the build fails closed");
    }

    private static void SeedDeterminism()
    {
        CustomWoodlandRecipe recipe = Fixtures.Recipe(stocking: 1200, edgeSeedTrees: 4, seed: 41UL);
        CustomWoodlandBuild first = CustomWoodlandBuilder.Build(recipe);
        CustomWoodlandBuild second = CustomWoodlandBuilder.Build(recipe);

        TestRunner.Eq(first.CanonicalText, second.CanonicalText, "the same recipe is byte-identical");
        TestRunner.Eq(first.Stand.Trees.Count, second.Stand.Trees.Count, "the same tree count");

        CustomWoodlandBuild reseeded = CustomWoodlandBuilder.Build(
            Fixtures.Recipe(stocking: 1200, edgeSeedTrees: 4, seed: 42UL));
        TestRunner.True(first.CanonicalText != reseeded.CanonicalText, "a different seed changes the stand");
        TestRunner.Eq(first.Stand.Trees.Count, reseeded.Stand.Trees.Count, "the seed does not change the count");
    }

    private static void BogTreesFlagged()
    {
        CustomWoodlandBuild build = CustomWoodlandBuilder.Build(Fixtures.Recipe(
            boundary: Fixtures.SmallShape,
            stocking: 1000,
            pattern: MoisturePatternSpec.WetHollowBogPocket(10.0)));

        TestRunner.True(build.HasHabitatProtectionCaution, "the grid reports the bog caution");

        int cautioned = 0;
        foreach (GeneratedTree tree in build.Stand.Trees)
        {
            TestRunner.True(build.Moisture.TryClassAtPoint(tree.Position, out SiteMoistureClass cellClass),
                "every tree sits in an active grid cell");
            TestRunner.Eq(cellClass, tree.Moisture, "the tree carries its cell's class");
            TestRunner.Eq(
                SpeciesMoistureTable.HabitatProtectionCaution(cellClass),
                tree.HabitatProtectionCaution,
                "the tree carries the table's caution decision");
            if (tree.HabitatProtectionCaution)
                cautioned++;
        }

        TestRunner.True(cautioned > 0, "some trees stand on bog and are flagged");
        TestRunner.True(cautioned < build.Stand.Trees.Count, "not every tree stands on bog");
    }

    private static void CapRefused()
    {
        CustomWoodlandRecipe recipe = Fixtures.Recipe(
            boundary: WoodlandShapeSpec.Rectangle(120.0, 120.0),
            stocking: 3000);

        IReadOnlyList<string> errors = CustomWoodlandBuilder.Validate(recipe);
        TestRunner.True(errors.Count > 0, "4,320 target stems exceed the cap");
        TestRunner.True(
            errors.Any(error => error.Contains("living-tree safety cap", StringComparison.Ordinal)),
            "the message names the safety cap");
        TestRunner.Throws<ArgumentException>(
            () => CustomWoodlandBuilder.Build(recipe), "the build refuses instead of trimming the mix");
    }

    private static void NearCapReported()
    {
        CustomWoodlandBuild build = CustomWoodlandBuilder.Build(Fixtures.Recipe(
            boundary: WoodlandShapeSpec.Rectangle(100.0, 100.0),
            stocking: 2500));

        TestRunner.Eq(2500, build.Stand.PlantedStemCount, "2,500 stems per hectare on 1.0 ha");
        TestRunner.True(build.Stand.NearSafetyCap, "the near-cap warning is reported");
        TestRunner.Eq(
            CustomWoodlandStandGenerator.LivingTreeSafetyCap,
            build.Stand.LivingTreeSafetyCap,
            "the cap is reported with the stand");
    }
}
