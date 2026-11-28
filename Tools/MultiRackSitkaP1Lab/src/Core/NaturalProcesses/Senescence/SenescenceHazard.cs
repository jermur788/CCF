using SitkaP1.Core.SpeciesWave2;

namespace SitkaP1.Core.NaturalProcesses.Senescence;

/// <summary>
/// Six-species natural old-age residual annual mortality hazard curves (SPEC-multi-2d-senescence-late-growth-v1).
///
/// Layered on top of existing density/suppression and storm mortality: never replaces them, never double-counts them,
/// and never depends on crowding, growth, moisture, or storm inputs.
///
/// Evidence disclosure (D-059): ordinal longevity ordering (pioneers alder/birch &lt; mid-longevity sitka/beech &lt;
/// long-lived scots pine/sessile oak) reflects general European/British species-longevity knowledge ([I]/[T]), not a
/// fitted Irish mortality dataset. All numeric onset ages, ramp widths, and maximum annual hazards are gameplay
/// calibration ([C]).
///
/// Separation from timber defects: Common alder heart rot (<c>BirchAlderTimber.cs</c>) remains strictly a
/// timber-quality downgrade probability on felled stems and is NEVER converted into an annual mortality probability.
/// </summary>
public static class SenescenceHazard
{
    private static readonly EvidenceTag Calibration = EvidenceTag.Of(EvidenceClass.Calibration);

    // [Calibration] Common alder ramp is 60 years (onset 55, ceiling 0.030) — chosen explicitly so pioneer hazard stays
    // modest through the age 60-70 commercial rotation window (h(70) = 0.0046875 < 0.005, survival over ages 60..70
    // ~ 0.974) without a cliff, while survival over ages 70..150 remains ~ 0.140 inside the (0.05, 0.60) target band.
    public static SenescenceSpeciesProfile CommonAlder { get; } = new(
        SpeciesMoistureTable.CommonAlder, OnsetAgeYears: 55, RampYears: 60, MaximumAnnualHazard: 0.030, Calibration);

    public static SenescenceSpeciesProfile DownyBirch { get; } = new(
        SpeciesMoistureTable.DownyBirch, OnsetAgeYears: 60, RampYears: 55, MaximumAnnualHazard: 0.026, Calibration);

    public static SenescenceSpeciesProfile SitkaSpruce { get; } = new(
        SpeciesMoistureTable.SitkaSpruce, OnsetAgeYears: 90, RampYears: 55, MaximumAnnualHazard: 0.020, Calibration);

    public static SenescenceSpeciesProfile EuropeanBeech { get; } = new(
        SpeciesMoistureTable.EuropeanBeech, OnsetAgeYears: 115, RampYears: 60, MaximumAnnualHazard: 0.016, Calibration);

    public static SenescenceSpeciesProfile ScotsPine { get; } = new(
        SpeciesMoistureTable.ScotsPine, OnsetAgeYears: 150, RampYears: 70, MaximumAnnualHazard: 0.010, Calibration);

    public static SenescenceSpeciesProfile SessileOak { get; } = new(
        SpeciesMoistureTable.SessileOak, OnsetAgeYears: 160, RampYears: 75, MaximumAnnualHazard: 0.009, Calibration);

    /// <summary>All six species profiles in ascending onset order.</summary>
    public static IReadOnlyList<SenescenceSpeciesProfile> Profiles { get; } = Array.AsReadOnly(new[]
    {
        CommonAlder,
        DownyBirch,
        SitkaSpruce,
        EuropeanBeech,
        ScotsPine,
        SessileOak,
    });

    /// <summary>All six supported species IDs (reused directly from <see cref="SpeciesMoistureTable"/>).</summary>
    public static IReadOnlyList<string> SupportedSpecies { get; } = Array.AsReadOnly(new[]
    {
        SpeciesMoistureTable.CommonAlder,
        SpeciesMoistureTable.DownyBirch,
        SpeciesMoistureTable.SitkaSpruce,
        SpeciesMoistureTable.EuropeanBeech,
        SpeciesMoistureTable.ScotsPine,
        SpeciesMoistureTable.SessileOak,
    });

    private static readonly IReadOnlyDictionary<string, SenescenceSpeciesProfile> ProfilesById =
        new Dictionary<string, SenescenceSpeciesProfile>(StringComparer.Ordinal)
        {
            [CommonAlder.SpeciesId] = CommonAlder,
            [DownyBirch.SpeciesId] = DownyBirch,
            [SitkaSpruce.SpeciesId] = SitkaSpruce,
            [EuropeanBeech.SpeciesId] = EuropeanBeech,
            [ScotsPine.SpeciesId] = ScotsPine,
            [SessileOak.SpeciesId] = SessileOak,
        };

    public static bool IsSupportedSpecies(string? speciesId) =>
        speciesId is not null && ProfilesById.ContainsKey(speciesId);

    public static SenescenceSpeciesProfile GetProfile(string speciesId)
    {
        if (speciesId is null || !ProfilesById.TryGetValue(speciesId, out SenescenceSpeciesProfile? profile))
            throw new ArgumentException($"Unknown senescence species id '{speciesId}'.", nameof(speciesId));
        return profile;
    }

    /// <summary>
    /// Computes the residual annual old-age mortality probability for a whole-year age.
    /// Returns bit-exact <c>0.0</c> for <c>ageYears &lt;= onset</c>, then follows a C1 smoothstep ramp
    /// <c>s(t) = t * t * (3 - 2 * t)</c> to a flat modest ceiling <c>MaximumAnnualHazard</c>.
    /// </summary>
    public static double AnnualHazard(string speciesId, int ageYears)
    {
        SenescenceSpeciesProfile profile = GetProfile(speciesId);
        ArgumentOutOfRangeException.ThrowIfNegative(ageYears);

        if (ageYears <= profile.OnsetAgeYears)
            return 0.0;

        double t = Math.Min(1.0, (ageYears - profile.OnsetAgeYears) / (double)profile.RampYears);
        double smoothstep = t * t * (3.0 - 2.0 * t);
        return profile.MaximumAnnualHazard * smoothstep;
    }

    /// <summary>
    /// Cumulative survival probability under old-age hazard alone: the product of <c>1 - h(age)</c> over every whole-year
    /// age from <paramref name="startAgeInclusive"/> through <paramref name="endAgeInclusive"/>, both ends included.
    /// "Survival from 70 to 150" in the spec means <c>SurvivalProbability(species, 70, 150)</c>.
    /// </summary>
    public static double SurvivalProbability(string speciesId, int startAgeInclusive, int endAgeInclusive)
    {
        _ = GetProfile(speciesId);
        ArgumentOutOfRangeException.ThrowIfNegative(startAgeInclusive);
        if (endAgeInclusive < startAgeInclusive)
            throw new ArgumentOutOfRangeException(nameof(endAgeInclusive), endAgeInclusive, "End age must be >= start age.");

        double survival = 1.0;
        for (int age = startAgeInclusive; age <= endAgeInclusive; age++)
            survival *= 1.0 - AnnualHazard(speciesId, age);
        return survival;
    }
}
