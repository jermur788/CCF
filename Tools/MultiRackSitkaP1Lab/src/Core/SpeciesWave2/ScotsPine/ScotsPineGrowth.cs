namespace SitkaP1.Core.SpeciesWave2.ScotsPine;

public readonly record struct ScotsPineGrowthRequest(
    int StandAgeYears,
    double StemsPerHectare,
    double BasalAreaM2PerHa,
    double TopHeightM,
    SiteMoistureClass Moisture);

public readonly record struct ScotsPineGrowthResult(
    SiteMoistureResult Site,
    double? PotentialDbhIncrementCm,
    double? SiteAdjustedDbhIncrementCm)
{
    public bool IsWithinCalibrationDomain => PotentialDbhIncrementCm.HasValue;
    public EvidenceTag Evidence => EvidenceTag.Of(EvidenceClass.Calibration);
}

public static partial class ScotsPineEngine
{
    public static SpeciesProfileCard Profile => FutureSpeciesCatalog.ScotsPine;

    // [Calibration] Annual DBH increments for the younger and older ends of the supported stand-age range.
    private const double YoungStandIncrementCm = 0.60;
    private const double OlderStandIncrementCm = 0.20;
    private const double MaximumCrowdingReduction = 0.50;

    /// <summary>Estimates one year's DBH increment for a representative adult stem inside the readiness domain.</summary>
    public static ScotsPineGrowthResult EvaluateGrowth(ScotsPineGrowthRequest request)
    {
        SiteMoistureResult site = SiteMoistureEngine.ForSpecies(Profile.SpeciesId, request.Moisture);
        var readiness = Profile.Readiness;
        if (readiness.AgeRangeYears is not { } ages ||
            readiness.StemsPerHectare is not { } stocking ||
            readiness.BasalAreaM2PerHa is not { } basalArea ||
            readiness.MinimumTopHeightM is not { } minimumTopHeight ||
            !ages.Contains(request.StandAgeYears) ||
            !stocking.Contains(request.StemsPerHectare) ||
            !basalArea.Contains(request.BasalAreaM2PerHa) ||
            !double.IsFinite(request.TopHeightM) || request.TopHeightM < minimumTopHeight)
        {
            return new ScotsPineGrowthResult(site, null, null);
        }

        double ageProgress = (request.StandAgeYears - ages.Low) / (ages.High - ages.Low);
        double stockingPressure = (request.StemsPerHectare - stocking.Low) / (stocking.High - stocking.Low);
        double basalAreaPressure = (request.BasalAreaM2PerHa - basalArea.Low) / (basalArea.High - basalArea.Low);
        double crowding = Math.Max(stockingPressure, basalAreaPressure);
        double ageIncrement = YoungStandIncrementCm - ageProgress * (YoungStandIncrementCm - OlderStandIncrementCm);
        double potentialIncrement = ageIncrement * (1.0 - MaximumCrowdingReduction * crowding);

        return new ScotsPineGrowthResult(
            site, potentialIncrement, potentialIncrement * site.Cell.Response.GrowthMultiplier);
    }
}
