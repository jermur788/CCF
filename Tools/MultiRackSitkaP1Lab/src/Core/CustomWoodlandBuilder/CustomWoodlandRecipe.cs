using System.Globalization;
using System.Text;
using SitkaP1.Core.SpeciesWave2;

namespace SitkaP1.Core.CustomWoodlandBuilder;

/// <summary>
/// One complete custom woodland request: boundary, lanes, moisture, deer and mix. Immutable.
///
/// Nothing here reads a clock, a file or ambient randomness. <see cref="Seed"/> is the only source of variation in
/// placement and species assignment, so the same recipe always produces the same stand.
/// </summary>
public sealed record CustomWoodlandRecipe(
    WoodlandShapeSpec Boundary,
    int LaneCount,
    MoisturePatternSpec MoisturePattern,
    IReadOnlyList<MoisturePaintOverride> PaintOverrides,
    DeerBrowsePreset DeerBrowsing,
    IReadOnlyList<SpeciesMixEntry> SpeciesMix,
    int StartAgeYears,
    int StockingStemsPerHectare,
    int EdgeSeedTreeCount,
    ulong Seed);

/// <summary>The built custom woodland: the validated boundary, clipped lanes, moisture grid, deer setting and stand.</summary>
public sealed record CustomWoodlandBuild(
    CustomWoodlandRecipe Recipe,
    WoodlandPolygon Boundary,
    LanePlan Lanes,
    MoistureGrid Moisture,
    DeerBrowseSetting Deer,
    GeneratedStand Stand)
{
    public bool HasHabitatProtectionCaution => Moisture.HasHabitatProtectionCaution;

    /// <summary>
    /// Canonical text for the whole build. Doubles and floats enter as raw IEEE-754 bit patterns, so a digest over this
    /// text changes when any position, class or count changes and never changes for formatting reasons.
    /// </summary>
    public string CanonicalText { get; } =
        CustomWoodlandCanonicalText.Write(Boundary, Lanes, Moisture, Deer, Stand, Recipe.SpeciesMix, Recipe.Seed);
}

/// <summary>
/// Validates a recipe and builds the custom woodland. Validation is fail-closed and returns every problem it can see:
/// <see cref="Build"/> throws with all of them rather than repairing the input.
/// </summary>
public static class CustomWoodlandBuilder
{
    public static CustomWoodlandBuild Build(CustomWoodlandRecipe recipe)
    {
        if (recipe is null)
            throw new ArgumentNullException(nameof(recipe));

        IReadOnlyList<string> errors = Validate(recipe);
        if (errors.Count > 0)
            throw new ArgumentException("invalid custom woodland recipe: " + string.Join(" ", errors), nameof(recipe));

        WoodlandPolygon polygon = WoodlandPolygon.FromShape(recipe.Boundary);
        LanePlan lanes = LaneClipper.Clip(polygon, recipe.LaneCount);
        MoistureGrid grid = MoistureGrid.Build(polygon, recipe.MoisturePattern, recipe.PaintOverrides);
        DeerBrowseSetting deer = DeerBrowsePresets.Get(recipe.DeerBrowsing);
        GeneratedStand stand = CustomWoodlandStandGenerator.Generate(recipe, polygon, lanes, grid);

        return new CustomWoodlandBuild(recipe, polygon, lanes, grid, deer, stand);
    }

    /// <summary>All validation errors, in a stable order. An empty list means the recipe can be built.</summary>
    public static IReadOnlyList<string> Validate(CustomWoodlandRecipe recipe)
    {
        var errors = new List<string>();
        if (recipe is null)
        {
            errors.Add("a recipe is required.");
            return errors.AsReadOnly();
        }

        if (recipe.Boundary is null)
            errors.Add("a boundary shape is required.");

        if (recipe.LaneCount < LaneClipper.MinimumLaneCount || recipe.LaneCount > LaneClipper.MaximumLaneCount)
            errors.Add($"the lane count must be {LaneClipper.MinimumLaneCount}-{LaneClipper.MaximumLaneCount}.");

        if (recipe.MoisturePattern is null)
            errors.Add("a site moisture pattern is required.");

        if (recipe.PaintOverrides is null)
            errors.Add("the paint override list is required (it may be empty).");

        if (!Enum.IsDefined(recipe.DeerBrowsing))
            errors.Add("the deer browsing preset is not one of the four approved settings.");

        ValidateSpeciesMix(recipe.SpeciesMix, errors);

        if (recipe.StartAgeYears < CustomWoodlandStandGenerator.MinimumStartAgeYears ||
            recipe.StartAgeYears > CustomWoodlandStandGenerator.MaximumStartAgeYears)
            errors.Add(
                $"the start age must be {CustomWoodlandStandGenerator.MinimumStartAgeYears}-" +
                $"{CustomWoodlandStandGenerator.MaximumStartAgeYears} years.");

        if (recipe.StockingStemsPerHectare < CustomWoodlandStandGenerator.MinimumStockingStemsPerHectare ||
            recipe.StockingStemsPerHectare > CustomWoodlandStandGenerator.MaximumStockingStemsPerHectare)
            errors.Add(
                $"the stocking must be {CustomWoodlandStandGenerator.MinimumStockingStemsPerHectare}-" +
                $"{CustomWoodlandStandGenerator.MaximumStockingStemsPerHectare} stems per hectare.");

        if (recipe.EdgeSeedTreeCount < 0 ||
            recipe.EdgeSeedTreeCount > CustomWoodlandStandGenerator.MaximumEdgeSeedTreeCount)
            errors.Add($"the edge seed tree count must be 0-{CustomWoodlandStandGenerator.MaximumEdgeSeedTreeCount}.");

        // Everything below depends on a shape, a mix and a moisture pattern that survived the cheap checks.
        if (errors.Count > 0)
            return errors.AsReadOnly();

        WoodlandShapeSpec boundary = recipe.Boundary!;
        MoisturePatternSpec pattern = recipe.MoisturePattern!;
        IReadOnlyList<MoisturePaintOverride> overrides = recipe.PaintOverrides!;

        WoodlandPolygon polygon;
        try
        {
            polygon = WoodlandPolygon.FromShape(boundary);
        }
        catch (ArgumentException exception)
        {
            errors.Add(exception.Message);
            return errors.AsReadOnly();
        }

        try
        {
            LanePlan lanes = LaneClipper.Clip(polygon, recipe.LaneCount);
            if (!lanes.IsValid)
                errors.Add(
                    $"lane {lanes.FailedLaneIndex} is not a usable work lane: {Describe(lanes.Rejection)}. " +
                    "Lanes are never dropped silently; change the field shape or the lane count.");
        }
        catch (ArgumentException exception)
        {
            errors.Add(exception.Message);
        }

        try
        {
            MoistureGrid grid = MoistureGrid.Build(polygon, pattern, overrides);
            if (recipe.EdgeSeedTreeCount > 0)
            {
                int capacity = CustomWoodlandStandGenerator.EdgeSeedTreeCapacity(polygon, grid);
                if (recipe.EdgeSeedTreeCount > capacity)
                    errors.Add(
                        $"this boundary can seat only {capacity} mature edge seed trees at a " +
                        $"{CustomWoodlandStandGenerator.EdgeSeedTreeInsetM:F1} m inset, " +
                        $"but {recipe.EdgeSeedTreeCount} were requested.");
            }
        }
        catch (ArgumentException exception)
        {
            errors.Add(exception.Message);
        }

        int target = CustomWoodlandStandGenerator.TargetPlantedStemCount(polygon, recipe.StockingStemsPerHectare);
        int living = target + recipe.EdgeSeedTreeCount;
        if (living > CustomWoodlandStandGenerator.LivingTreeSafetyCap)
            errors.Add(
                $"the recipe asks for {living} living trees ({target} planted + {recipe.EdgeSeedTreeCount} edge seed), " +
                $"above the {CustomWoodlandStandGenerator.LivingTreeSafetyCap} living-tree safety cap.");

        return errors.AsReadOnly();
    }

    private static void ValidateSpeciesMix(IReadOnlyList<SpeciesMixEntry>? mix, List<string> errors)
    {
        if (mix is null)
        {
            errors.Add("a species mix is required.");
            return;
        }

        IReadOnlyList<string> accepted = SpeciesMoistureTable.SpeciesIds;
        if (mix.Count != accepted.Count)
            errors.Add($"the mix must list all {accepted.Count} accepted species exactly once, with zero allowed.");

        var seen = new HashSet<string>(StringComparer.Ordinal);
        double total = 0.0;

        foreach (SpeciesMixEntry entry in mix)
        {
            if (entry is null)
            {
                errors.Add("a mix entry is null.");
                continue;
            }

            if (!accepted.Contains(entry.SpeciesId, StringComparer.Ordinal))
                errors.Add($"'{entry.SpeciesId}' is not one of the accepted species.");
            else if (!seen.Add(entry.SpeciesId))
                errors.Add($"'{entry.SpeciesId}' appears more than once in the mix.");

            if (!double.IsFinite(entry.Percent) || entry.Percent < 0.0 || entry.Percent > 100.0)
                errors.Add($"the weight for '{entry.SpeciesId}' must be 0-100 percent.");
            else
                total += entry.Percent;
        }

        if (seen.Count == accepted.Count && Math.Abs(total - 100.0) > 1e-9)
            errors.Add($"the mix weights must sum to 100 percent, got {total.ToString("R", CultureInfo.InvariantCulture)}.");
    }

    private static string Describe(LaneRejectionReason reason) => reason switch
    {
        LaneRejectionReason.TooFewVertices => "the band does not overlap the field",
        LaneRejectionReason.BelowMinimumArea =>
            $"its area is below {LaneClipper.MinimumLaneAreaM2:F0} m2",
        LaneRejectionReason.BelowMinimumWidth =>
            $"its mean width is below {LaneClipper.MinimumLaneWidthM:F1} m",
        _ => "the reason was not evaluated",
    };
}

/// <summary>Canonical text writer. Private to the build result so the format has exactly one owner.</summary>
internal static class CustomWoodlandCanonicalText
{
    internal static string Write(
        WoodlandPolygon boundary,
        LanePlan lanes,
        MoistureGrid moisture,
        DeerBrowseSetting deer,
        GeneratedStand stand,
        IReadOnlyList<SpeciesMixEntry> mix,
        ulong seed)
    {
        var text = new StringBuilder(4096);
        text.Append("CWB1\n");

        text.Append("boundary template=").Append(boundary.Vertices.Count).Append(' ').Append(boundary.AreaM2.ToString("R", CultureInfo.InvariantCulture)).Append('\n');
        foreach (PlanPoint vertex in boundary.Vertices)
            text.Append("bv ").Append(Bits(vertex.X)).Append(' ').Append(Bits(vertex.Z)).Append('\n');

        text.Append("lanes count=").Append(lanes.LaneCount)
            .Append(" band=").Append(Bits(lanes.BandHeightM))
            .Append(" south=").Append(Bits(lanes.SouthZ))
            .Append(" valid=").Append(lanes.IsValid ? 1 : 0).Append('\n');
        foreach (WorkLane lane in lanes.Lanes)
        {
            text.Append("lane ").Append(lane.Index)
                .Append(" area=").Append(Bits(lane.AreaM2))
                .Append(" width=").Append(Bits(lane.MeanWidthM))
                .Append(" rejection=").Append((int)lane.Rejection).Append('\n');
            foreach (PlanPoint vertex in lane.Boundary)
                text.Append("lv ").Append(Bits(vertex.X)).Append(' ').Append(Bits(vertex.Z)).Append('\n');
        }

        text.Append("moisture pattern=").Append(moisture.Pattern.Kind)
            .Append(" columns=").Append(moisture.Columns)
            .Append(" rows=").Append(moisture.Rows)
            .Append(" origin=").Append(Bits(moisture.OriginX)).Append(',').Append(Bits(moisture.OriginZ))
            .Append(" active=").Append(moisture.ActiveCellCount)
            .Append(" caution=").Append(moisture.CautionCellCount).Append('\n');
        for (int cellZ = 0; cellZ < moisture.Rows; cellZ++)
        {
            for (int cellX = 0; cellX < moisture.Columns; cellX++)
            {
                if (moisture.IsActive(cellX, cellZ))
                    text.Append("cell ").Append(cellX).Append(' ').Append(cellZ).Append(' ')
                        .Append((int)moisture.ClassAt(cellX, cellZ)).Append('\n');
            }
        }

        text.Append("deer preset=").Append(deer.Preset)
            .Append(" pressure=").Append(BitConverter.SingleToInt32Bits(deer.BackgroundBrowsePressure)).Append('\n');

        text.Append("mix seed=").Append(seed.ToString(CultureInfo.InvariantCulture)).Append('\n');
        foreach (SpeciesMixEntry entry in mix)
            text.Append("mix ").Append(entry.SpeciesId).Append(' ').Append(Bits(entry.Percent)).Append('\n');

        text.Append("stand target=").Append(stand.TargetStemsPerHectare)
            .Append(" planted=").Append(stand.PlantedStemCount)
            .Append(" edge=").Append(stand.EdgeSeedTreeCount)
            .Append(" achieved=").Append(Bits(stand.StemsPerHectareAchieved))
            .Append(" cap=").Append(stand.LivingTreeSafetyCap)
            .Append(" near=").Append(stand.NearSafetyCap ? 1 : 0).Append('\n');
        foreach (GeneratedTree tree in stand.Trees)
        {
            text.Append("tree ").Append(tree.Index)
                .Append(" x=").Append(Bits(tree.Position.X))
                .Append(" z=").Append(Bits(tree.Position.Z))
                .Append(" species=").Append(tree.SpeciesId)
                .Append(" lane=").Append(tree.LaneIndex)
                .Append(" moisture=").Append((int)tree.Moisture)
                .Append(" edge=").Append(tree.IsEdgeSeedTree ? 1 : 0)
                .Append(" age=").Append(tree.AgeYears)
                .Append(" caution=").Append(tree.HabitatProtectionCaution ? 1 : 0)
                .Append('\n');
        }

        return text.ToString();
    }

    private static string Bits(double value) =>
        BitConverter.DoubleToInt64Bits(value).ToString(CultureInfo.InvariantCulture);
}
