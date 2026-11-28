using SitkaP1.Core.SpeciesWave2;
using SitkaP1.Core.SpeciesWave2.BirchAlder;
using SitkaP1.Core.SpeciesWave2.ScotsPine;

namespace SitkaP1.Core.NaturalProcesses.Senescence;

/// <summary>Input for Wave 2 smooth late-life DBH growth evaluation.</summary>
public readonly record struct LateLifeGrowthRequest(
    string SpeciesId,
    int StandAgeYears,
    double StemsPerHectare,
    double BasalAreaM2PerHa,
    double TopHeightM,
    SiteMoistureClass Moisture);

/// <summary>
/// Unified result for Wave 2 growth inside and beyond the calibration/teaching age cutoff.
/// <see cref="IsWithinCalibrationWindow"/> is true only when the age lies within the existing species window and the
/// non-age stand domain checks pass; beyond the cutoff it is false while increments remain smoothly available inside
/// the species' stocking, basal-area and top-height domain.
/// </summary>
public readonly record struct LateLifeGrowthResult(
    string SpeciesId,
    SiteMoistureClass Moisture,
    SiteResponse SiteResponse,
    double? PotentialDbhIncrementCm,
    double? SiteAdjustedDbhIncrementCm,
    bool IsWithinCalibrationWindow)
{
    public bool IsAvailable => PotentialDbhIncrementCm.HasValue;

    public EvidenceTag Evidence => EvidenceTag.Of(EvidenceClass.Calibration);
}

/// <summary>
/// Smooth C0/C1 late-life DBH growth continuation for the three Wave 2 species only
/// (<c>scots-pine</c>, <c>common-alder</c>, <c>downy-birch</c>).
///
/// Inside each species' existing window (<c>age &lt;= cutoff</c>), calls delegate directly to
/// <see cref="ScotsPineEngine.EvaluateGrowth"/> or <see cref="BirchAlderEngine.EvaluateGrowth"/> so existing values
/// remain bit-exact without editing any Wave 2 source file.
///
/// Beyond the cutoff (<c>age &gt; cutoff</c>), nominal annual DBH increment tapers smoothly as
/// <c>A * exp(-k * (age - cutoff))</c>, where <c>A</c> is the delegated zero-crowding <c>NeutralLegacy</c> potential
/// returned by the existing evaluator at <c>cutoff</c> (never hardcoded) and <c>k = (-windowSlope) / A</c>.
///
/// IEEE-754 note: for <c>common-alder</c>, delegating to <see cref="BirchAlderEngine.EvaluateGrowth"/> at age 60 yields
/// <c>0.19999999999999996</c> (2 ULPs below <c>0.20</c>) purely because of IEEE-754 representation of the existing
/// linear window (<c>0.95 - 0.75</c>), not because of a model discontinuity. Anchoring the exponential continuation to
/// that exact delegated value preserves bit-exact continuity across the cutoff with zero artificial jump.
///
/// <c>sitka-spruce</c>, <c>sessile-oak</c> and <c>european-beech</c> are deliberately refused so their existing growth
/// equations remain completely untouched.
/// </summary>
public static class LateLifeGrowth
{
    private sealed record Wave2ContinuationSpec(
        string SpeciesId,
        int WindowLowAgeYears,
        int CutoffAgeYears,
        ValueBand StemsPerHectare,
        ValueBand BasalAreaM2PerHa,
        double MinimumTopHeightM,
        double CutoffAnchorCm,
        double WindowSlopeCmPerYear,
        double DecayRateK,
        double MaximumCrowdingReduction);

    public static IReadOnlyList<string> SupportedSpecies { get; } = Array.AsReadOnly(new[]
    {
        SpeciesMoistureTable.ScotsPine,
        SpeciesMoistureTable.CommonAlder,
        SpeciesMoistureTable.DownyBirch,
    });

    private static readonly IReadOnlyDictionary<string, Wave2ContinuationSpec> SpecsById = BuildSpecs();

    private static IReadOnlyDictionary<string, Wave2ContinuationSpec> BuildSpecs()
    {
        var specs = new Dictionary<string, Wave2ContinuationSpec>(StringComparer.Ordinal);

        // Scots pine: read readiness domain and anchors directly from existing catalog/engine so nothing is duplicated.
        ReadinessProfile pineReadiness = FutureSpeciesCatalog.ScotsPine.Readiness;
        ValueBand pineAges = pineReadiness.AgeRangeYears!.Value;
        ValueBand pineStocking = pineReadiness.StemsPerHectare!.Value;
        ValueBand pineBasalArea = pineReadiness.BasalAreaM2PerHa!.Value;
        double pineMinTopHeight = pineReadiness.MinimumTopHeightM!.Value;
        int pineLow = (int)pineAges.Low;
        int pineCutoff = (int)pineAges.High;

        double pineYoung = ScotsPineEngine.EvaluateGrowth(new(
            pineLow, pineStocking.Low, pineBasalArea.Low, pineMinTopHeight, SiteMoistureClass.NeutralLegacy))
            .PotentialDbhIncrementCm!.Value;
        double pineAnchor = ScotsPineEngine.EvaluateGrowth(new(
            pineCutoff, pineStocking.Low, pineBasalArea.Low, pineMinTopHeight, SiteMoistureClass.NeutralLegacy))
            .PotentialDbhIncrementCm!.Value;
        double pineMaxCrowded = ScotsPineEngine.EvaluateGrowth(new(
            pineCutoff, pineStocking.High, pineBasalArea.High, pineMinTopHeight, SiteMoistureClass.NeutralLegacy))
            .PotentialDbhIncrementCm!.Value;
        double pineMaxReduction = 1.0 - pineMaxCrowded / pineAnchor;
        double pineSlope = (pineAnchor - pineYoung) / (pineCutoff - pineLow);
        specs[SpeciesMoistureTable.ScotsPine] = new Wave2ContinuationSpec(
            SpeciesMoistureTable.ScotsPine,
            pineLow,
            pineCutoff,
            pineStocking,
            pineBasalArea,
            pineMinTopHeight,
            pineAnchor,
            pineSlope,
            (-pineSlope) / pineAnchor,
            pineMaxReduction);

        // Common alder and Downy birch: reuse BirchAlderEngine.GrowthDomain and delegate at cutoff for anchor A.
        specs[SpeciesMoistureTable.CommonAlder] = BuildBirchAlderSpec(
            SpeciesMoistureTable.CommonAlder, BirchAlderEngine.AlderMaximumCrowdingReduction);
        specs[SpeciesMoistureTable.DownyBirch] = BuildBirchAlderSpec(
            SpeciesMoistureTable.DownyBirch, BirchAlderEngine.BirchMaximumCrowdingReduction);

        return specs;
    }

    private static Wave2ContinuationSpec BuildBirchAlderSpec(string speciesId, double maximumCrowdingReduction)
    {
        GrowthTeachingDomain domain = BirchAlderEngine.GrowthDomain(speciesId);
        int low = (int)domain.AgeYears.Low;
        int cutoff = (int)domain.AgeYears.High;

        double young = BirchAlderEngine.EvaluateGrowth(new(
            speciesId, low, domain.StemsPerHectare.Low, domain.BasalAreaM2PerHa.Low, domain.MinimumTopHeightM, SiteMoistureClass.NeutralLegacy))
            .PotentialDbhIncrementCm!.Value;
        double anchor = BirchAlderEngine.EvaluateGrowth(new(
            speciesId, cutoff, domain.StemsPerHectare.Low, domain.BasalAreaM2PerHa.Low, domain.MinimumTopHeightM, SiteMoistureClass.NeutralLegacy))
            .PotentialDbhIncrementCm!.Value;
        double slope = (anchor - young) / (cutoff - low);

        return new Wave2ContinuationSpec(
            speciesId,
            low,
            cutoff,
            domain.StemsPerHectare,
            domain.BasalAreaM2PerHa,
            domain.MinimumTopHeightM,
            anchor,
            slope,
            (-slope) / anchor,
            maximumCrowdingReduction);
    }

    public static bool IsSupportedSpecies(string? speciesId) =>
        speciesId is not null && SpecsById.ContainsKey(speciesId);

    private static Wave2ContinuationSpec RequireSpec(string? speciesId, string paramName)
    {
        if (speciesId is null || !SpecsById.TryGetValue(speciesId, out Wave2ContinuationSpec? spec))
        {
            throw new ArgumentException(
                $"Late-life growth continuation supports only scots-pine, common-alder, and downy-birch; refused '{speciesId}'.",
                paramName);
        }
        return spec;
    }

    /// <summary>Upper age of the species' existing Wave 2 calibration/teaching window (60 pine/alder, 70 birch).</summary>
    public static int CutoffAgeYears(string speciesId) => RequireSpec(speciesId, nameof(speciesId)).CutoffAgeYears;

    /// <summary>In-window linear DBH increment slope (cm/yr per year of age, negative).</summary>
    public static double WindowSlopeCmPerYear(string speciesId) => RequireSpec(speciesId, nameof(speciesId)).WindowSlopeCmPerYear;

    /// <summary>Exponential continuation decay rate <c>k = (-windowSlope) / A</c> (1/yr).</summary>
    public static double ExponentialDecayRatePerYear(string speciesId) => RequireSpec(speciesId, nameof(speciesId)).DecayRateK;

    /// <summary>
    /// Nominal (zero-crowding, <c>NeutralLegacy</c>) annual DBH increment at or beyond the species cutoff.
    /// At <c>ageYears == cutoff</c>, returns the exact delegated anchor bits <c>A</c> with zero discrepancy.
    /// </summary>
    public static double NominalIncrementAtOrBeyondCutoff(string speciesId, int ageYears)
    {
        Wave2ContinuationSpec spec = RequireSpec(speciesId, nameof(speciesId));
        ArgumentOutOfRangeException.ThrowIfLessThan(ageYears, spec.CutoffAgeYears);

        if (ageYears == spec.CutoffAgeYears)
            return spec.CutoffAnchorCm;

        return spec.CutoffAnchorCm * Math.Exp(-spec.DecayRateK * (ageYears - spec.CutoffAgeYears));
    }

    /// <summary>
    /// Evaluates one year's DBH increment for a supported Wave 2 species.
    /// Delegates bit-exactly to the existing Wave 2 evaluator for <c>age &lt;= cutoff</c>, and applies smooth
    /// exponential continuation with the same crowding, domain, and single site-moisture multiplier rules for
    /// <c>age &gt; cutoff</c>.
    /// </summary>
    public static LateLifeGrowthResult EvaluateGrowth(LateLifeGrowthRequest request)
    {
        Wave2ContinuationSpec spec = RequireSpec(request.SpeciesId, nameof(request));

        if (request.StandAgeYears <= spec.CutoffAgeYears)
            return EvaluateInWindowDelegated(spec, request);

        // Beyond cutoff: reuse SpeciesMoistureTable.Cell once and apply the exact same non-age domain guards.
        MoistureCell cell = SpeciesMoistureTable.Cell(spec.SpeciesId, request.Moisture);
        if (!spec.StemsPerHectare.Contains(request.StemsPerHectare) ||
            !spec.BasalAreaM2PerHa.Contains(request.BasalAreaM2PerHa) ||
            !double.IsFinite(request.TopHeightM) || request.TopHeightM < spec.MinimumTopHeightM)
        {
            return new LateLifeGrowthResult(
                spec.SpeciesId, request.Moisture, cell.Response, null, null, IsWithinCalibrationWindow: false);
        }

        double stockingPressure = (request.StemsPerHectare - spec.StemsPerHectare.Low) /
                                  (spec.StemsPerHectare.High - spec.StemsPerHectare.Low);
        double basalAreaPressure = (request.BasalAreaM2PerHa - spec.BasalAreaM2PerHa.Low) /
                                   (spec.BasalAreaM2PerHa.High - spec.BasalAreaM2PerHa.Low);
        double crowding = Math.Max(stockingPressure, basalAreaPressure);

        double nominal = NominalIncrementAtOrBeyondCutoff(spec.SpeciesId, request.StandAgeYears);
        double potential = nominal * (1.0 - spec.MaximumCrowdingReduction * crowding);
        double siteAdjusted = potential * cell.Response.GrowthMultiplier;

        return new LateLifeGrowthResult(
            spec.SpeciesId,
            request.Moisture,
            cell.Response,
            potential,
            siteAdjusted,
            IsWithinCalibrationWindow: false);
    }

    private static LateLifeGrowthResult EvaluateInWindowDelegated(Wave2ContinuationSpec spec, LateLifeGrowthRequest request)
    {
        if (spec.SpeciesId == SpeciesMoistureTable.ScotsPine)
        {
            ScotsPineGrowthResult delegated = ScotsPineEngine.EvaluateGrowth(new ScotsPineGrowthRequest(
                request.StandAgeYears,
                request.StemsPerHectare,
                request.BasalAreaM2PerHa,
                request.TopHeightM,
                request.Moisture));
            return new LateLifeGrowthResult(
                spec.SpeciesId,
                request.Moisture,
                delegated.Site.Cell.Response,
                delegated.PotentialDbhIncrementCm,
                delegated.SiteAdjustedDbhIncrementCm,
                delegated.IsWithinCalibrationDomain);
        }

        BirchAlderGrowthResult birchAlder = BirchAlderEngine.EvaluateGrowth(new BirchAlderGrowthRequest(
            spec.SpeciesId,
            request.StandAgeYears,
            request.StemsPerHectare,
            request.BasalAreaM2PerHa,
            request.TopHeightM,
            request.Moisture));
        return new LateLifeGrowthResult(
            spec.SpeciesId,
            request.Moisture,
            birchAlder.Site.Response,
            birchAlder.PotentialDbhIncrementCm,
            birchAlder.SiteAdjustedDbhIncrementCm,
            birchAlder.IsWithinTeachingDomain);
    }
}
