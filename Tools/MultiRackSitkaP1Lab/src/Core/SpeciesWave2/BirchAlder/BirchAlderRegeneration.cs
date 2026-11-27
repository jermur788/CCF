namespace SitkaP1.Core.SpeciesWave2.BirchAlder;

/// <param name="SpeciesId">Downy birch or Common alder.</param>
/// <param name="WoodlandSeed">Stable seed identity shared by the woodland's candidate gaps.</param>
/// <param name="ElapsedYear">Whole years since the woodland's year zero.</param>
/// <param name="OldestOnMapSeedTreeAgeYears">Age of the oldest local seed tree, or null when absent.</param>
/// <param name="CanopyOpenness">Gap openness from 0 (closed) to 1 (open).</param>
/// <param name="UndergrowthCover">Competing cover from 0 (none) to 1 (full).</param>
/// <param name="Moisture">Site moisture at the candidate opening.</param>
/// <param name="ExposedMineralGround">Whether the ground is exposed mineral soil rather than intact vegetated surface.</param>
public readonly record struct BirchAlderRegenerationRequest(
    string SpeciesId,
    ulong WoodlandSeed,
    int ElapsedYear,
    int? OldestOnMapSeedTreeAgeYears,
    double CanopyOpenness,
    double UndergrowthCover,
    SiteMoistureClass Moisture,
    bool ExposedMineralGround = false);

public enum BirchAlderRegenerationStatus
{
    Eligible,
    NoMatureLocalSeedTree,
    PoorSeedYear,
    CanopyTooDark,
    MoistureTooLow,
    BogNotAForestCrop,
    NoExposedMineralGround,
    TooMuchUndergrowth,
}

public readonly record struct BirchAlderRegenerationResult(
    string SpeciesId,
    BirchAlderSite Site,
    bool GoodSeedYear,
    BirchAlderRegenerationStatus Status)
{
    public bool CanRegenerate => Status == BirchAlderRegenerationStatus.Eligible;

    public double EstablishmentMultiplier => CanRegenerate ? Site.Response.EstablishmentMultiplier : 0.0;

    /// <summary>Bog always reports the habitat caution, whether or not the gap is eligible.</summary>
    public bool HabitatProtectionCaution => Site.HabitatProtectionCaution;

    public EvidenceTag Evidence => EvidenceTag.Of(EvidenceClass.Calibration);
}

public static partial class BirchAlderEngine
{
    // [Calibration] Teaching thresholds. Birch wants a bright opening and clean ground; alder also needs wet, moving
    // water and exposed mineral soil. Neither value is an empirical recruitment threshold.
    public const int BirchMinimumSeedTreeAgeYears = 12;
    public const double BirchMinimumCanopyOpenness = 0.65;
    public const double BirchMaximumUndergrowthCover = 0.35;

    public const int AlderMinimumSeedTreeAgeYears = 10;
    public const double AlderMinimumCanopyOpenness = 0.55;
    public const double AlderMaximumUndergrowthCover = 0.40;

    private const ulong BirchGoodSeedYearSalt = 0x6B1C40AE9D3F2F17UL;
    private const ulong AlderGoodSeedYearSalt = 0xA1DE2C53914B7D0DUL;
    private const int SeedYearBlockLength = 4;

    public static int MinimumSeedTreeAgeYears(string speciesId) =>
        BirchAlderSpecies.RequireSupported(speciesId, nameof(speciesId)) == BirchAlderSpecies.DownyBirch
            ? BirchMinimumSeedTreeAgeYears
            : AlderMinimumSeedTreeAgeYears;

    public static BirchAlderRegenerationResult EvaluateRegeneration(BirchAlderRegenerationRequest request)
    {
        string speciesId = BirchAlderSpecies.RequireSupported(request.SpeciesId, nameof(request));
        if (request.OldestOnMapSeedTreeAgeYears is < 0)
            throw new ArgumentOutOfRangeException(nameof(request), "Seed tree age must be non-negative.");
        if (!(request.CanopyOpenness is >= 0.0 and <= 1.0))
            throw new ArgumentOutOfRangeException(nameof(request), "Canopy openness must be between 0 and 1.");
        if (!(request.UndergrowthCover is >= 0.0 and <= 1.0))
            throw new ArgumentOutOfRangeException(nameof(request), "Undergrowth cover must be between 0 and 1.");

        bool birch = speciesId == BirchAlderSpecies.DownyBirch;
        BirchAlderSite site = Site(speciesId, request.Moisture);
        bool goodSeedYear = IsGoodSeedYear(speciesId, request.WoodlandSeed, request.ElapsedYear);

        int minimumSeedAge = birch ? BirchMinimumSeedTreeAgeYears : AlderMinimumSeedTreeAgeYears;
        double minimumOpenness = birch ? BirchMinimumCanopyOpenness : AlderMinimumCanopyOpenness;
        double maximumUndergrowth = birch ? BirchMaximumUndergrowthCover : AlderMaximumUndergrowthCover;

        BirchAlderRegenerationStatus status;
        if (request.OldestOnMapSeedTreeAgeYears is not { } seedTreeAge || seedTreeAge < minimumSeedAge)
            status = BirchAlderRegenerationStatus.NoMatureLocalSeedTree;
        else if (!goodSeedYear)
            status = BirchAlderRegenerationStatus.PoorSeedYear;
        else if (request.CanopyOpenness < minimumOpenness)
            status = BirchAlderRegenerationStatus.CanopyTooDark;
        else if (IsAlderMoistureTooLow(birch, request.Moisture))
            status = BirchAlderRegenerationStatus.MoistureTooLow;
        else if (!birch && request.Moisture == SiteMoistureClass.StagnantPoorBog)
            status = BirchAlderRegenerationStatus.BogNotAForestCrop;
        else if (!birch && !request.ExposedMineralGround)
            status = BirchAlderRegenerationStatus.NoExposedMineralGround;
        else if (request.UndergrowthCover > maximumUndergrowth)
            status = BirchAlderRegenerationStatus.TooMuchUndergrowth;
        else
            status = BirchAlderRegenerationStatus.Eligible;

        return new BirchAlderRegenerationResult(speciesId, site, goodSeedYear, status);
    }

    private static bool IsAlderMoistureTooLow(bool birch, SiteMoistureClass moisture) =>
        !birch && moisture == SiteMoistureClass.DryFreeDraining;

    /// <summary>
    /// [Calibration] One good seed year per four-year block, at offset 0 or 1, so gaps span 3-5 years. Each species
    /// has its own salt, so birch and alder schedules differ inside the same woodland.
    /// </summary>
    public static bool IsGoodSeedYear(string speciesId, ulong woodlandSeed, int elapsedYear)
    {
        string species = BirchAlderSpecies.RequireSupported(speciesId, nameof(speciesId));
        ArgumentOutOfRangeException.ThrowIfNegative(elapsedYear);

        ulong salt = species == BirchAlderSpecies.DownyBirch ? BirchGoodSeedYearSalt : AlderGoodSeedYearSalt;
        ulong block = (ulong)(elapsedYear / SeedYearBlockLength);
        ulong mixed = SplitMix64(woodlandSeed ^ ((block + 1UL) * 0x9E3779B97F4A7C15UL), salt);
        return elapsedYear % SeedYearBlockLength == (int)(mixed & 1UL);
    }

    private static ulong SplitMix64(ulong value, ulong salt)
    {
        unchecked
        {
            // The SplitMix64 finalizer spreads differences in woodland seeds and block indices across the bits.
            ulong mixed = value + salt;
            mixed = (mixed ^ (mixed >> 30)) * 0xBF58476D1CE4E5B9UL;
            mixed = (mixed ^ (mixed >> 27)) * 0x94D049BB133111EBUL;
            return mixed ^ (mixed >> 31);
        }
    }
}
