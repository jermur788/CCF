namespace SitkaP1.Core.SpeciesWave2.BirchAlder;

/// <param name="SpeciesId">Downy birch or Common alder.</param>
/// <param name="WoodlandSeed">Stable seed identity shared by the woodland's candidate gaps.</param>
/// <param name="ElapsedYear">Whole years since the woodland's year zero.</param>
/// <param name="OldestOnMapSeedTreeAgeYears">
/// Age of the oldest local seed tree <em>of the requested species</em>, or null when absent. A birch seed tree does not
/// seed alder.
/// </param>
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
    /// <summary>Zero value: the gap has not been evaluated. It must never read as a successful decision.</summary>
    NotEvaluated = 0,
    Eligible,
    NoMatureLocalSeedTree,
    PoorSeedYear,
    CanopyTooDark,
    /// <summary>Alder only: the ground drains freely, so the site is too dry for a wet-site specialist.</summary>
    MoistureTooLow,
    /// <summary>Alder only: the site carries no moisture class (NeutralLegacy), so suitability is unknown.</summary>
    MoistureUnknown,
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
        else if (!birch && !IsAlderMoistureSuitable(request.Moisture))
            status = request.Moisture switch
            {
                SiteMoistureClass.StagnantPoorBog => BirchAlderRegenerationStatus.BogNotAForestCrop,
                SiteMoistureClass.DryFreeDraining => BirchAlderRegenerationStatus.MoistureTooLow,
                // NeutralLegacy carries no moisture class. Alder must not recruit on unknown ground just because the
                // table's identity multipliers are (1.0, 1.0).
                _ => BirchAlderRegenerationStatus.MoistureUnknown,
            };
        else if (!birch && !request.ExposedMineralGround)
            status = BirchAlderRegenerationStatus.NoExposedMineralGround;
        else if (request.UndergrowthCover > maximumUndergrowth)
            status = BirchAlderRegenerationStatus.TooMuchUndergrowth;
        else
            status = BirchAlderRegenerationStatus.Eligible;

        return new BirchAlderRegenerationResult(speciesId, site, goodSeedYear, status);
    }

    /// <summary>
    /// Common alder recruits only on the three moisture classes FR-4 names. Anything else, including
    /// <see cref="SiteMoistureClass.NeutralLegacy"/> ground with no moisture class at all, is refused.
    /// </summary>
    private static bool IsAlderMoistureSuitable(SiteMoistureClass moisture) =>
        moisture is SiteMoistureClass.WetMovingAlluvial
            or SiteMoistureClass.FreshMoistMineral
            or SiteMoistureClass.SeasonallyWetAerated;

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
