using SitkaP1.Core.SpeciesWave2;

namespace SitkaP1.Core.CustomWoodlandBuilder.Tests;

internal static class MoistureTests
{
    public static void Register()
    {
        TestRunner.Add("Moisture", "MOIST-01 the grid is 2.5 m and aligned to the global lattice", LatticeAligned);
        TestRunner.Add("Moisture", "MOIST-02 only cells whose centre is inside the field are active", ActiveRule);
        TestRunner.Add("Moisture", "MOIST-03 uniform pattern fills every active cell", UniformPattern);
        TestRunner.Add("Moisture", "MOIST-04 ridge and slope catena bands are present and ordered", RidgeAndSlope);
        TestRunner.Add("Moisture", "MOIST-05 stream corridor and its banks are classified", StreamCorridor);
        TestRunner.Add("Moisture", "MOIST-06 bog hollow raises the habitat caution", BogHollow);
        TestRunner.Add("Moisture", "MOIST-07 no active cell is NeutralLegacy and the table identity is untouched", NoLegacyCells);
        TestRunner.Add("Moisture", "MOIST-08 paint overrides apply on top, later wins, bad targets refused", PaintOverrides);
        TestRunner.Add("Moisture", "MOIST-09 invalid patterns and parameters are refused", InvalidPatterns);
    }

    private static WoodlandPolygon SmallPolygon() =>
        WoodlandPolygon.FromShape(Fixtures.SmallShape);

    private static MoistureGrid BuildGrid(MoisturePatternSpec pattern, IReadOnlyList<MoisturePaintOverride>? overrides = null)
    {
        CustomWoodlandBuild build = CustomWoodlandBuilder.Build(Fixtures.Recipe(
            boundary: Fixtures.SmallShape,
            stocking: 1000,
            pattern: pattern,
            overrides: overrides));
        return build.Moisture;
    }

    private static void LatticeAligned()
    {
        WoodlandPolygon polygon = WoodlandPolygon.FromShape(
            WoodlandShapeSpec.AngledStreamEdge(70.0, 50.0, streamSlantM: -20.0));
        MoistureGrid grid = CustomWoodlandBuilder.Build(Fixtures.Recipe(
            boundary: WoodlandShapeSpec.AngledStreamEdge(70.0, 50.0, streamSlantM: -20.0),
            stocking: 1000)).Moisture;

        TestRunner.Eq(2.5, MoistureGrid.CellSizeM, "the cell size matches the simulation grid");
        TestRunner.True(grid.OriginX <= polygon.MinX, "the lattice starts at or below the field");
        TestRunner.True(polygon.MinX - grid.OriginX < MoistureGrid.CellSizeM, "the offset is less than one cell");
        TestRunner.ApproxEq(
            Math.Round(grid.OriginX / MoistureGrid.CellSizeM),
            grid.OriginX / MoistureGrid.CellSizeM,
            1e-12,
            "the origin sits on a 2.5 m multiple");
        TestRunner.ApproxEq(
            Math.Round(grid.OriginZ / MoistureGrid.CellSizeM),
            grid.OriginZ / MoistureGrid.CellSizeM,
            1e-12,
            "the origin sits on a 2.5 m multiple");

        PlanPoint first = grid.CellCentre(0, 0);
        TestRunner.ApproxEq(grid.OriginX + 1.25, first.X, 1e-12, "cell centres are offset by half a cell");
        TestRunner.ApproxEq(grid.OriginZ + 1.25, first.Z, 1e-12, "cell centres are offset by half a cell");
    }

    private static void ActiveRule()
    {
        WoodlandPolygon polygon = SmallPolygon();
        MoistureGrid grid = BuildGrid(MoisturePatternSpec.Uniform(SiteMoistureClass.FreshMoistMineral));

        TestRunner.True(grid.ActiveCellCount > 0, "a 40 x 40 m field has active cells");

        int counted = 0;
        for (int cellZ = 0; cellZ < grid.Rows; cellZ++)
        {
            for (int cellX = 0; cellX < grid.Columns; cellX++)
            {
                bool expected = polygon.ContainsPoint(grid.CellCentre(cellX, cellZ));
                TestRunner.Eq(expected, grid.IsActive(cellX, cellZ),
                    $"cell ({cellX}, {cellZ}) activity follows its centre");
                if (expected)
                    counted++;
            }
        }

        TestRunner.Eq(counted, grid.ActiveCellCount, "the active count is exact");
        TestRunner.Throws<ArgumentOutOfRangeException>(
            () => grid.ClassAt(grid.Columns, 0), "a cell outside the grid has no class");
    }

    private static void UniformPattern()
    {
        MoistureGrid grid = BuildGrid(MoisturePatternSpec.Uniform(SiteMoistureClass.SeasonallyWetAerated));

        int checkedCells = 0;
        for (int cellZ = 0; cellZ < grid.Rows; cellZ++)
        {
            for (int cellX = 0; cellX < grid.Columns; cellX++)
            {
                if (!grid.IsActive(cellX, cellZ))
                    continue;
                TestRunner.Eq(SiteMoistureClass.SeasonallyWetAerated, grid.ClassAt(cellX, cellZ),
                    "uniform cells carry the chosen class");
                checkedCells++;
            }
        }

        TestRunner.Eq(grid.ActiveCellCount, checkedCells, "every active cell was checked");
        TestRunner.Eq(0, grid.CautionCellCount, "uniform fresh ground carries no bog caution");
        TestRunner.True(!grid.HasHabitatProtectionCaution, "no caution flag");
    }

    private static void RidgeAndSlope()
    {
        WoodlandPolygon polygon = SmallPolygon();
        MoistureGrid grid = BuildGrid(MoisturePatternSpec.RidgeAndSlope());

        int south = 0, middle = 0, north = 0;
        for (int cellZ = 0; cellZ < grid.Rows; cellZ++)
        {
            for (int cellX = 0; cellX < grid.Columns; cellX++)
            {
                if (!grid.IsActive(cellX, cellZ))
                    continue;

                double position = (grid.CellCentre(cellX, cellZ).Z - polygon.MinZ) / polygon.LengthM;
                SiteMoistureClass moisture = grid.ClassAt(cellX, cellZ);
                if (position < 0.25)
                {
                    TestRunner.Eq(SiteMoistureClass.SeasonallyWetAerated, moisture, "the southern toe is seasonally wet");
                    south++;
                }
                else if (position < 0.75)
                {
                    TestRunner.Eq(SiteMoistureClass.FreshMoistMineral, moisture, "the slope is fresh mineral");
                    middle++;
                }
                else
                {
                    TestRunner.Eq(SiteMoistureClass.DryFreeDraining, moisture, "the ridge is dry");
                    north++;
                }
            }
        }

        TestRunner.True(south > 0 && middle > 0 && north > 0, "all three catena bands contain active cells");
        TestRunner.True(middle > south, "the slope band is the widest");
        TestRunner.Eq(0, grid.CautionCellCount, "a dry ridge carries no bog caution");
    }

    private static void StreamCorridor()
    {
        WoodlandPolygon polygon = SmallPolygon();
        MoistureGrid grid = BuildGrid(MoisturePatternSpec.StreamCorridor(8.0));
        double midZ = polygon.MinZ + polygon.LengthM / 2.0;

        int corridor = 0, banks = 0, slopes = 0;
        for (int cellZ = 0; cellZ < grid.Rows; cellZ++)
        {
            for (int cellX = 0; cellX < grid.Columns; cellX++)
            {
                if (!grid.IsActive(cellX, cellZ))
                    continue;

                double distance = Math.Abs(grid.CellCentre(cellX, cellZ).Z - midZ);
                SiteMoistureClass moisture = grid.ClassAt(cellX, cellZ);
                if (distance <= 4.0)
                {
                    TestRunner.Eq(SiteMoistureClass.WetMovingAlluvial, moisture, "the corridor is moving alluvial");
                    corridor++;
                }
                else if (distance <= 10.0)
                {
                    TestRunner.Eq(SiteMoistureClass.SeasonallyWetAerated, moisture, "the banks are seasonally wet");
                    banks++;
                }
                else
                {
                    TestRunner.Eq(SiteMoistureClass.FreshMoistMineral, moisture, "away from the stream is fresh mineral");
                    slopes++;
                }
            }
        }

        TestRunner.True(corridor > 0, "the corridor has active cells");
        TestRunner.True(banks > 0, "the banks have active cells");
        TestRunner.True(slopes > 0, "the slopes have active cells");
        TestRunner.Eq(0, grid.CautionCellCount, "a flowing corridor is not bog");
    }

    private static void BogHollow()
    {
        WoodlandPolygon polygon = SmallPolygon();
        MoistureGrid grid = BuildGrid(MoisturePatternSpec.WetHollowBogPocket(10.0));

        TestRunner.True(grid.TryClassAtPoint(polygon.Centroid, out SiteMoistureClass centreClass),
            "the centroid falls in an active cell");
        TestRunner.Eq(SiteMoistureClass.StagnantPoorBog, centreClass, "the hollow centre is bog");
        TestRunner.True(grid.HasHabitatProtectionCaution, "bog raises the habitat protection caution");
        TestRunner.True(grid.CautionCellCount > 0, "the caution counts the affected cells");

        TestRunner.True(grid.TryClassAtPoint(new PlanPoint(1.25, 1.25), out SiteMoistureClass cornerClass),
            "the south-west cell is active");
        TestRunner.Eq(SiteMoistureClass.FreshMoistMineral, cornerClass, "far from the hollow the ground is fresh mineral");

        bool sawRing = false;
        for (int cellZ = 0; cellZ < grid.Rows; cellZ++)
        {
            for (int cellX = 0; cellX < grid.Columns; cellX++)
            {
                if (!grid.IsActive(cellX, cellZ))
                    continue;
                if (grid.ClassAt(cellX, cellZ) == SiteMoistureClass.SeasonallyWetAerated)
                    sawRing = true;
            }
        }

        TestRunner.True(sawRing, "the seasonally wet ring around the hollow exists");
    }

    private static void NoLegacyCells()
    {
        MoistureGrid grid = BuildGrid(MoisturePatternSpec.Uniform(SiteMoistureClass.WetMovingAlluvial));

        for (int cellZ = 0; cellZ < grid.Rows; cellZ++)
        {
            for (int cellX = 0; cellX < grid.Columns; cellX++)
            {
                if (!grid.IsActive(cellX, cellZ))
                    continue;

                TestRunner.True(
                    grid.ClassAt(cellX, cellZ) != SiteMoistureClass.NeutralLegacy,
                    "an active custom cell is never NeutralLegacy");
                TestRunner.True(
                    MoistureGrid.IsRealMoistureClass(grid.ClassAt(cellX, cellZ)),
                    "an active custom cell carries a real moisture class");
            }
        }

        foreach (string speciesId in SpeciesMoistureTable.SpeciesIds)
        {
            SiteResponse response = SpeciesMoistureTable.Response(speciesId, SiteMoistureClass.NeutralLegacy);
            TestRunner.BitEq(1.0, response.EstablishmentMultiplier, "legacy establishment stays exactly 1.0");
            TestRunner.BitEq(1.0, response.GrowthMultiplier, "legacy growth stays exactly 1.0");
        }

        TestRunner.True(
            SpeciesMoistureTable.HabitatProtectionCaution(SiteMoistureClass.StagnantPoorBog),
            "the table still cautions bog");
        TestRunner.True(
            !SpeciesMoistureTable.HabitatProtectionCaution(SiteMoistureClass.WetMovingAlluvial),
            "moving alluvial is not a caution");
    }

    private static void PaintOverrides()
    {
        var painted = new[]
        {
            new MoisturePaintOverride(5, 5, SiteMoistureClass.StagnantPoorBog),
        };
        MoistureGrid bogGrid = BuildGrid(
            MoisturePatternSpec.Uniform(SiteMoistureClass.FreshMoistMineral), painted);
        TestRunner.Eq(SiteMoistureClass.StagnantPoorBog, bogGrid.ClassAt(5, 5), "the painted cell is bog");
        TestRunner.True(bogGrid.HasHabitatProtectionCaution, "painting bog raises the caution");
        TestRunner.Eq(1, bogGrid.CautionCellCount, "exactly one cell is painted bog");
        TestRunner.Eq(1, bogGrid.Overrides.Count, "the override is retained for a later save");
        TestRunner.Eq(SiteMoistureClass.FreshMoistMineral, bogGrid.ClassAt(5, 6), "neighbouring cells keep the base class");

        var overwritten = new[]
        {
            new MoisturePaintOverride(5, 5, SiteMoistureClass.StagnantPoorBog),
            new MoisturePaintOverride(5, 5, SiteMoistureClass.FreshMoistMineral),
        };
        MoistureGrid repainted = BuildGrid(
            MoisturePatternSpec.Uniform(SiteMoistureClass.FreshMoistMineral), overwritten);
        TestRunner.Eq(SiteMoistureClass.FreshMoistMineral, repainted.ClassAt(5, 5), "the later override wins");
        TestRunner.Eq(0, repainted.CautionCellCount, "the caution is recomputed after painting");

        TestRunner.Throws<ArgumentException>(
            () => BuildGrid(MoisturePatternSpec.Uniform(SiteMoistureClass.FreshMoistMineral), new[]
            {
                new MoisturePaintOverride(-1, 0, SiteMoistureClass.WetMovingAlluvial),
            }),
            "a negative cell index is refused");

        TestRunner.Throws<ArgumentException>(
            () => BuildGrid(MoisturePatternSpec.Uniform(SiteMoistureClass.FreshMoistMineral), new[]
            {
                new MoisturePaintOverride(5, 5, SiteMoistureClass.NeutralLegacy),
            }),
            "painting NeutralLegacy is refused");

        CustomWoodlandRecipe inactiveTarget = Fixtures.Recipe(
            boundary: WoodlandShapeSpec.AngledStreamEdge(70.0, 50.0, streamSlantM: -20.0),
            stocking: 1000,
            overrides: new[] { new MoisturePaintOverride(0, 0, SiteMoistureClass.WetMovingAlluvial) });
        TestRunner.Throws<ArgumentException>(
            () => CustomWoodlandBuilder.Build(inactiveTarget),
            "painting a cell outside the field is refused");
    }

    private static void InvalidPatterns()
    {
        TestRunner.Throws<ArgumentException>(
            () => BuildGrid(MoisturePatternSpec.Uniform(SiteMoistureClass.NeutralLegacy)),
            "a uniform legacy grid is refused");

        TestRunner.Throws<ArgumentException>(
            () => BuildGrid(MoisturePatternSpec.StreamCorridor(1.0)),
            "a corridor narrower than 2 m is refused");

        TestRunner.Throws<ArgumentException>(
            () => BuildGrid(MoisturePatternSpec.StreamCorridor(double.NaN)),
            "a non-finite corridor width is refused");

        TestRunner.Throws<ArgumentException>(
            () => BuildGrid(MoisturePatternSpec.WetHollowBogPocket(100.0)),
            "a hollow larger than 50 m is refused");

        TestRunner.Throws<ArgumentException>(
            () => BuildGrid(MoisturePatternSpec.WetHollowBogPocket(double.PositiveInfinity)),
            "a non-finite hollow radius is refused");
    }
}
