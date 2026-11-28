using System.Globalization;
using SitkaP1.Core.SpeciesWave2;
using SitkaP1.Core.SpeciesWave2.ScotsPine;

namespace SitkaP1.Core;

// Scots pine and site moisture, Part 2 live wiring (SPEC-multi-2d-scots-pine-moisture-V1, Option A). Every modifier and map
// rule here is read from the Part 1 module (SiteMoistureEngine / SpeciesMoistureTable); nothing is re-declared.

/// <summary>How a woodland's site moisture is laid out.</summary>
internal enum SiteMoistureMode
{
    /// <summary>One class for the whole woodland (NeutralLegacy is the default and changes nothing).</summary>
    Whole = 0,

    /// <summary>The Part 1 Dry Ridge &amp; Moist Slope map: x below the stand midpoint is dry, the midpoint and above is fresh moist.</summary>
    DryRidgeMoistSlope = 1,
}

/// <summary>
/// The site-moisture choice of one woodland. <see cref="Neutral"/> (Whole + NeutralLegacy) is exactly (1.0, 1.0) for every
/// species, so a woodland that never chose a class runs bit for bit as before.
/// </summary>
internal sealed record SiteMoistureSetting(SiteMoistureMode Mode, SiteMoistureClass WholeClass)
{
    public static readonly SiteMoistureSetting Neutral = new(SiteMoistureMode.Whole, SiteMoistureClass.NeutralLegacy);
    public static readonly SiteMoistureSetting DryRidgeMoistSlope = new(SiteMoistureMode.DryRidgeMoistSlope, SiteMoistureClass.NeutralLegacy);

    public static SiteMoistureSetting Whole(SiteMoistureClass moisture) => new(SiteMoistureMode.Whole, moisture);

    public bool IsNeutral => Mode == SiteMoistureMode.Whole && WholeClass == SiteMoistureClass.NeutralLegacy;

    internal SiteMoistureSetting RequireValid()
    {
        if (!Enum.IsDefined(Mode) || !Enum.IsDefined(WholeClass))
            throw new P1Refusal("SiteMoistureInvalid", "unknown site-moisture mode or class");
        if (Mode == SiteMoistureMode.DryRidgeMoistSlope && WholeClass != SiteMoistureClass.NeutralLegacy)
            throw new P1Refusal("SiteMoistureInvalid", "a two-zone woodland carries no whole-woodland class");
        return this;
    }

    public static string ModeWire(SiteMoistureMode mode) => mode switch
    {
        SiteMoistureMode.Whole => "Whole",
        SiteMoistureMode.DryRidgeMoistSlope => "DryRidgeMoistSlope",
        _ => throw new P1Refusal("SiteMoistureInvalid", "unknown site-moisture mode"),
    };

    public static SiteMoistureMode ParseMode(string value) => value switch
    {
        "Whole" => SiteMoistureMode.Whole,
        "DryRidgeMoistSlope" => SiteMoistureMode.DryRidgeMoistSlope,
        _ => throw new P1Refusal("SiteMoistureInvalid", "unknown site-moisture mode: " + value),
    };

    public static string ClassWire(SiteMoistureClass moisture) =>
        Enum.IsDefined(moisture) ? moisture.ToString() : throw new P1Refusal("SiteMoistureInvalid", "unknown site-moisture class");

    public static SiteMoistureClass ParseClass(string value)
    {
        foreach (var moisture in SpeciesMoistureTable.MoistureClasses)
            if (string.Equals(moisture.ToString(), value, StringComparison.Ordinal)) return moisture;
        throw new P1Refusal("SiteMoistureInvalid", "unknown site-moisture class: " + value);
    }
}

/// <summary>
/// The site-moisture field of one woodland: its setting plus the stand bounds the two-zone map splits on. Pure and immutable;
/// it answers the class and the separate establishment and growth modifiers at any point inside the stand.
/// </summary>
internal sealed class SiteMoistureField
{
    private readonly DryRidgeAndMoistSlopeMap? map;

    public SiteMoistureField(SiteMoistureSetting setting, StandBounds bounds)
    {
        Setting = setting.RequireValid();
        Bounds = bounds;
        map = setting.Mode == SiteMoistureMode.DryRidgeMoistSlope ? SiteMoistureEngine.CreateDryRidgeAndMoistSlope(bounds) : null;
    }

    public SiteMoistureSetting Setting { get; }
    public StandBounds Bounds { get; }
    public bool IsNeutral => Setting.IsNeutral;

    /// <summary>The class at a point (map coordinates x east, z south). Points on the closed bounds belong to the stand.</summary>
    public SiteMoistureClass At(double xM, double zM)
    {
        if (map is null) return Setting.WholeClass;
        double x = Math.Clamp(xM, Bounds.MinX, Bounds.MaxX);
        double z = Math.Clamp(zM, Bounds.MinY, Bounds.MaxY);
        return map.At(x, z).Moisture;
    }

    /// <summary>The Part 1 table cell for a live species ID at a point.</summary>
    public MoistureCell CellFor(string liveSpeciesId, double xM, double zM) =>
        SpeciesMoistureTable.Cell(SiteMoistureLabels.TableSpeciesId(liveSpeciesId), At(xM, zM));

    public float GrowthMultiplier(string liveSpeciesId, double xM, double zM) =>
        (float)CellFor(liveSpeciesId, xM, zM).Response.GrowthMultiplier;

    public float EstablishmentMultiplier(string liveSpeciesId, double xM, double zM) =>
        (float)CellFor(liveSpeciesId, xM, zM).Response.EstablishmentMultiplier;

    /// <summary>Area of each class inside the stand bounds (square metres), in table declaration order, zero classes omitted.</summary>
    public IReadOnlyList<MoistureArea> ZoneAreas()
    {
        decimal width = (decimal)(Bounds.MaxX - Bounds.MinX), length = (decimal)(Bounds.MaxY - Bounds.MinY);
        if (map is null) return Array.AsReadOnly(new[] { new MoistureArea(Setting.WholeClass, width * length) });
        decimal dryWidth = (decimal)(map.MidpointX - Bounds.MinX);
        return Array.AsReadOnly(new[]
        {
            new MoistureArea(SiteMoistureClass.DryFreeDraining, dryWidth * length),
            new MoistureArea(SiteMoistureClass.FreshMoistMineral, (width - dryWidth) * length),
        });
    }

    /// <summary>Stand bounds of the union of area polygons.</summary>
    public static StandBounds BoundsOf(IEnumerable<IReadOnlyList<MultiAreaVertex>> polygons)
    {
        double minX = double.PositiveInfinity, minZ = double.PositiveInfinity, maxX = double.NegativeInfinity, maxZ = double.NegativeInfinity;
        foreach (var polygon in polygons)
            foreach (var v in polygon)
            {
                minX = Math.Min(minX, v.XM); maxX = Math.Max(maxX, v.XM);
                minZ = Math.Min(minZ, v.ZM); maxZ = Math.Max(maxZ, v.ZM);
            }
        return new StandBounds(minX, minZ, maxX, maxZ);
    }
}

/// <summary>Plain-language labels for site moisture (pure .NET; the UI only renders these strings).</summary>
internal static class SiteMoistureLabels
{
    /// <summary>The locked whole-stand label of a 50/50 two-zone woodland. It never collapses to one class.</summary>
    public const string TwoZoneLabel = "Two-Zone (Dry Ridge & Moist Slope)";

    public const string HabitatCautionText =
        "Habitat protection caution: stagnant poor bog is a habitat-protection question first. Trees surviving here " +
        "never implies planting approval; intact bog should not be drained or planted.";

    public const string Evidence =
        "Site-moisture tiers and multipliers (1.00 / 0.70 / 0.35 / 0.10) are [Calibration] design-table values, not measured " +
        "Irish yields. Establishment and growth modifiers are separate; NeutralLegacy is exactly (1.0, 1.0).";

    public static string ClassLabel(SiteMoistureClass moisture) => moisture switch
    {
        SiteMoistureClass.NeutralLegacy => "Neutral (legacy, no moisture effect)",
        SiteMoistureClass.DryFreeDraining => "Dry free-draining",
        SiteMoistureClass.FreshMoistMineral => "Fresh moist mineral",
        SiteMoistureClass.SeasonallyWetAerated => "Seasonally wet (aerated)",
        SiteMoistureClass.WetMovingAlluvial => "Wet moving-water alluvial",
        SiteMoistureClass.StagnantPoorBog => "Stagnant poor bog",
        _ => throw new P1Refusal("SiteMoistureInvalid", "unknown site-moisture class"),
    };

    /// <summary>
    /// The whole-stand label. A two-zone woodland always reads <see cref="TwoZoneLabel"/>: the Part 1 dominant-class tie rule
    /// (declaration order) would otherwise call an exact 50/50 split DryFreeDraining, which hides the moist half.
    /// </summary>
    public static string WholeStandLabel(SiteMoistureSetting setting) =>
        setting.Mode == SiteMoistureMode.DryRidgeMoistSlope ? TwoZoneLabel : ClassLabel(setting.WholeClass);

    /// <summary>True when any part of the woodland is stagnant poor bog (carried from the Part 1 table, never computed in JS).</summary>
    public static bool HabitatCaution(SiteMoistureSetting setting) =>
        setting.Mode == SiteMoistureMode.Whole && new MoistureSelection(setting.WholeClass).HabitatProtectionCaution;

    /// <summary>The Part 1 table species ID of a live species ID (the live Beech ID is "beech").</summary>
    public static string TableSpeciesId(string liveSpeciesId) => liveSpeciesId switch
    {
        Phase2Species.BeechId => SpeciesMoistureTable.EuropeanBeech,
        _ => liveSpeciesId,
    };

    public static string Modifier(double value) => value.ToString("0.00", CultureInfo.InvariantCulture);
}
