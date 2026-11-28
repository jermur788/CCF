namespace SitkaP1.Core.SpeciesWave2.BirchAlder;

/// <summary>
/// Provisional teaching window for the adult growth shape. Neither profile card carries a numeric readiness domain, so
/// this is deliberately our own calibration window and must never be described as an Irish empirical domain.
/// </summary>
public readonly record struct GrowthTeachingDomain(
    ValueBand AgeYears,
    ValueBand StemsPerHectare,
    ValueBand BasalAreaM2PerHa,
    double MinimumTopHeightM);

/// <param name="SpeciesId">Downy birch or Common alder.</param>
/// <param name="StandAgeYears">Stand age in whole years.</param>
/// <param name="StemsPerHectare">Living stems per hectare.</param>
/// <param name="BasalAreaM2PerHa">Stand basal area in square metres per hectare.</param>
/// <param name="TopHeightM">Stand top height in metres; treated as a domain boundary.</param>
/// <param name="Moisture">Site moisture at the stem.</param>
public readonly record struct BirchAlderGrowthRequest(
    string SpeciesId,
    int StandAgeYears,
    double StemsPerHectare,
    double BasalAreaM2PerHa,
    double TopHeightM,
    SiteMoistureClass Moisture);

public readonly record struct BirchAlderGrowthResult(
    string SpeciesId,
    BirchAlderSite Site,
    double? PotentialDbhIncrementCm,
    double? SiteAdjustedDbhIncrementCm)
{
    public bool IsWithinTeachingDomain => PotentialDbhIncrementCm.HasValue;

    public EvidenceTag Evidence => EvidenceTag.Of(EvidenceClass.Calibration);
}

public static partial class BirchAlderEngine
{
    // [Calibration] Provisional teaching windows. Ages are a gameplay window, not a published readiness domain.
    public static GrowthTeachingDomain BirchGrowthDomain { get; } = new(
        new ValueBand(5, 70), new ValueBand(200, 5000), new ValueBand(5, 60), 4.0);

    public static GrowthTeachingDomain AlderGrowthDomain { get; } = new(
        new ValueBand(5, 60), new ValueBand(200, 4000), new ValueBand(5, 55), 5.0);

    // [Calibration] Annual DBH increments at the young and old ends of each window.
    public const double BirchJuvenileIncrementCm = 0.85;
    public const double BirchAdultIncrementCm = 0.25;
    public const double AlderJuvenileIncrementCm = 0.95;
    public const double AlderAdultIncrementCm = 0.20;

    // [Calibration] Birch loses more diameter growth to crowding than alder: it is the more light-hungry pioneer.
    public const double BirchMaximumCrowdingReduction = 0.70;
    public const double AlderMaximumCrowdingReduction = 0.65;

    public static GrowthTeachingDomain GrowthDomain(string speciesId) =>
        BirchAlderSpecies.RequireSupported(speciesId, nameof(speciesId)) == BirchAlderSpecies.DownyBirch
            ? BirchGrowthDomain
            : AlderGrowthDomain;

    /// <summary>
    /// Estimates one year's DBH increment for a representative adult stem. The shape is a linear decline from the
    /// juvenile increment to the adult increment across the teaching window; crowding scales it down by the larger of
    /// the normalised stocking and basal-area pressures. The site growth modifier is applied once, never twice.
    /// </summary>
    public static BirchAlderGrowthResult EvaluateGrowth(BirchAlderGrowthRequest request)
    {
        string speciesId = BirchAlderSpecies.RequireSupported(request.SpeciesId, nameof(request));
        BirchAlderSite site = Site(speciesId, request.Moisture);
        GrowthTeachingDomain domain = GrowthDomain(speciesId);

        if (!domain.AgeYears.Contains(request.StandAgeYears) ||
            !domain.StemsPerHectare.Contains(request.StemsPerHectare) ||
            !domain.BasalAreaM2PerHa.Contains(request.BasalAreaM2PerHa) ||
            !double.IsFinite(request.TopHeightM) || request.TopHeightM < domain.MinimumTopHeightM)
        {
            return new BirchAlderGrowthResult(speciesId, site, null, null);
        }

        bool birch = speciesId == BirchAlderSpecies.DownyBirch;
        double juvenile = birch ? BirchJuvenileIncrementCm : AlderJuvenileIncrementCm;
        double adult = birch ? BirchAdultIncrementCm : AlderAdultIncrementCm;
        double maximumReduction = birch ? BirchMaximumCrowdingReduction : AlderMaximumCrowdingReduction;

        double ageProgress = (request.StandAgeYears - domain.AgeYears.Low) /
                             (domain.AgeYears.High - domain.AgeYears.Low);
        double stockingPressure = (request.StemsPerHectare - domain.StemsPerHectare.Low) /
                                  (domain.StemsPerHectare.High - domain.StemsPerHectare.Low);
        double basalAreaPressure = (request.BasalAreaM2PerHa - domain.BasalAreaM2PerHa.Low) /
                                   (domain.BasalAreaM2PerHa.High - domain.BasalAreaM2PerHa.Low);
        double crowding = Math.Max(stockingPressure, basalAreaPressure);

        double ageIncrement = juvenile - ageProgress * (juvenile - adult);
        double potential = ageIncrement * (1.0 - maximumReduction * crowding);

        return new BirchAlderGrowthResult(
            speciesId, site, potential, potential * site.Response.GrowthMultiplier);
    }
}
