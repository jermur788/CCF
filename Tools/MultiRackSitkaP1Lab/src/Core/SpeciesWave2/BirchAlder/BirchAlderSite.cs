namespace SitkaP1.Core.SpeciesWave2.BirchAlder;

/// <summary>
/// The two native wet-site broadleaves this module owns. Both ids come from the existing table, so there is no second
/// species vocabulary to keep in step.
/// </summary>
public static class BirchAlderSpecies
{
    public const string DownyBirch = SpeciesMoistureTable.DownyBirch;
    public const string CommonAlder = SpeciesMoistureTable.CommonAlder;

    /// <summary>True only for the two species this module implements.</summary>
    public static bool IsSupported(string? speciesId) =>
        string.Equals(speciesId, DownyBirch, StringComparison.Ordinal) ||
        string.Equals(speciesId, CommonAlder, StringComparison.Ordinal);

    internal static string RequireSupported(string? speciesId, string parameterName)
    {
        if (!IsSupported(speciesId))
            throw new ArgumentException($"'{speciesId}' is not a Downy birch or Common alder id.", parameterName);
        return speciesId!;
    }
}

/// <summary>
/// One species on one moisture class: the existing table cell plus the class it came from. The cell is the table's own
/// value, so `NeutralLegacy` stays bit-exact and no modifier is pre-multiplied or copied.
/// </summary>
public readonly record struct BirchAlderSite(SiteMoistureClass Moisture, MoistureCell Cell)
{
    public SiteResponse Response => Cell.Response;

    /// <summary>Bog is a habitat-protection question, never a planting recommendation.</summary>
    public bool HabitatProtectionCaution => SpeciesMoistureTable.HabitatProtectionCaution(Moisture);
}

public static partial class BirchAlderEngine
{
    public static SpeciesProfileCard BirchProfile => FutureSpeciesCatalog.DownyBirch;

    public static SpeciesProfileCard AlderProfile => FutureSpeciesCatalog.CommonAlder;

    /// <summary>The reused profile card for a supported species id.</summary>
    public static SpeciesProfileCard Profile(string speciesId) =>
        BirchAlderSpecies.RequireSupported(speciesId, nameof(speciesId)) == BirchAlderSpecies.DownyBirch
            ? BirchProfile
            : AlderProfile;

    /// <summary>Reads one species/class cell straight from the existing Wave 2 table.</summary>
    public static BirchAlderSite Site(string speciesId, SiteMoistureClass moisture) =>
        new(moisture, SpeciesMoistureTable.Cell(BirchAlderSpecies.RequireSupported(speciesId, nameof(speciesId)), moisture));

    /// <summary>Both species ids in a stable order.</summary>
    public static IReadOnlyList<string> SupportedSpecies { get; } = Array.AsReadOnly(new[]
    {
        BirchAlderSpecies.DownyBirch, BirchAlderSpecies.CommonAlder,
    });
}
