namespace SitkaP1.Core.SpeciesWave2.ScotsPine;

/// <param name="WoodlandSeed">Stable seed identity shared by the woodland's gaps.</param>
/// <param name="ElapsedYear">Whole years since the woodland's year zero.</param>
/// <param name="OldestOnMapSeedTreeAgeYears">Age of the oldest local Scots pine seed tree, or null when absent.</param>
/// <param name="CanopyOpenness">Gap openness from 0 (closed) to 1 (open).</param>
/// <param name="UndergrowthCover">Competing cover from 0 (none) to 1 (full).</param>
/// <param name="Moisture">Site moisture at the candidate opening.</param>
public readonly record struct ScotsPineRegenerationRequest(
    ulong WoodlandSeed,
    int ElapsedYear,
    int? OldestOnMapSeedTreeAgeYears,
    double CanopyOpenness,
    double UndergrowthCover,
    SiteMoistureClass Moisture);

public enum ScotsPineRegenerationStatus
{
    Eligible,
    NoMatureLocalSeedTree,
    PoorSeedYear,
    CanopyTooDark,
    TooMuchUndergrowth,
}

public readonly record struct ScotsPineRegenerationResult(
    SiteMoistureResult Site,
    bool GoodSeedYear,
    ScotsPineRegenerationStatus Status)
{
    public bool CanRegenerate => Status == ScotsPineRegenerationStatus.Eligible;
    public double EstablishmentMultiplier => CanRegenerate ? Site.Cell.Response.EstablishmentMultiplier : 0.0;
    public EvidenceTag Evidence => EvidenceTag.Of(EvidenceClass.Calibration);
}

public static partial class ScotsPineEngine
{
    // [Calibration] A gap becomes eligible once all three local thresholds are met in a good seed year.
    public const int MinimumSeedTreeAgeYears = 18;
    public const double MinimumCanopyOpenness = 0.60;
    public const double MaximumUndergrowthCover = 0.30;

    private const ulong GoodSeedYearSalt = 0x1A96C03752AE8D4BUL;
    private const int SeedYearBlockLength = 4;

    public static ScotsPineRegenerationResult EvaluateRegeneration(ScotsPineRegenerationRequest request)
    {
        if (request.OldestOnMapSeedTreeAgeYears is < 0)
            throw new ArgumentOutOfRangeException(nameof(request), "Seed tree age must be non-negative.");
        if (!(request.CanopyOpenness is >= 0.0 and <= 1.0))
            throw new ArgumentOutOfRangeException(nameof(request), "Canopy openness must be between 0 and 1.");
        if (!(request.UndergrowthCover is >= 0.0 and <= 1.0))
            throw new ArgumentOutOfRangeException(nameof(request), "Undergrowth cover must be between 0 and 1.");

        SiteMoistureResult site = SiteMoistureEngine.ForSpecies(Profile.SpeciesId, request.Moisture);
        bool goodSeedYear = IsGoodSeedYear(request.WoodlandSeed, request.ElapsedYear);
        ScotsPineRegenerationStatus status;
        if (request.OldestOnMapSeedTreeAgeYears is null or < MinimumSeedTreeAgeYears)
            status = ScotsPineRegenerationStatus.NoMatureLocalSeedTree;
        else if (!goodSeedYear)
            status = ScotsPineRegenerationStatus.PoorSeedYear;
        else if (request.CanopyOpenness < MinimumCanopyOpenness)
            status = ScotsPineRegenerationStatus.CanopyTooDark;
        else if (request.UndergrowthCover > MaximumUndergrowthCover)
            status = ScotsPineRegenerationStatus.TooMuchUndergrowth;
        else
            status = ScotsPineRegenerationStatus.Eligible;

        return new ScotsPineRegenerationResult(site, goodSeedYear, status);
    }

    /// <summary>[Calibration] Each four-year block has a good year at offset 0 or 1, so gaps span 3–5 years.</summary>
    public static bool IsGoodSeedYear(ulong woodlandSeed, int elapsedYear)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(elapsedYear);

        ulong block = (ulong)(elapsedYear / SeedYearBlockLength);
        ulong mixed;
        unchecked
        {
            // The SplitMix64 finalizer spreads differences in woodland seeds and block indices across the bits.
            mixed = (woodlandSeed ^ GoodSeedYearSalt) + block * 0x9E3779B97F4A7C15UL;
            mixed = (mixed ^ (mixed >> 30)) * 0xBF58476D1CE4E5B9UL;
            mixed = (mixed ^ (mixed >> 27)) * 0x94D049BB133111EBUL;
            mixed ^= mixed >> 31;
        }

        return elapsedYear % SeedYearBlockLength == (int)(mixed & 1UL);
    }
}
