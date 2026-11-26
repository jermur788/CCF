namespace SitkaP1.Core.SpeciesWave2;

public enum ReadinessStatus { ReadyForInitialDesign, PartialWithAbstractions }

/// <summary>
/// How ready a species is for initial simulation design, and over what stand domain its sources hold. Numeric domains are
/// null when the sources give no defensible domain; null never means "unbounded". Metadata only — nothing reads it at runtime.
/// </summary>
public sealed record ReadinessProfile(
    ReadinessStatus Status,
    ValueBand? AgeRangeYears,
    ValueBand? StemsPerHectare,
    ValueBand? BasalAreaM2PerHa,
    double? MinimumTopHeightM,
    int? IrishYieldClassPotential,
    string Trajectory,
    string Notes);

/// <summary>Where new trees of a species may come from. AuthoredOnly: off-map arrival happens only when a scenario says so.</summary>
public enum ExternalColonisation { AuthoredOnly }

public enum ShadeTolerance { Light, LightModerate, Moderate }

public enum Level { Low, LowMedium, Medium, MediumHigh, High }

public enum PruningRelevance { Low, Moderate, High }

public enum TimberRole { PrimaryCommercialSoftwood, SecondaryCommercial }

public enum Sortment { Sawlog, SmallRoundwood, PulpChip, Bulk }

/// <summary>Seed-source rules. No hard wind-distance ceiling is encoded on any card.</summary>
public sealed record ColonisationProfile(
    ExternalColonisation ExternalColonisation,
    bool LocalOnMapSeedDispersalAllowed,
    bool WaterAssistedDispersalEligible,
    double? HardWindDispersalCeilingM);

/// <summary>A provisional price band for one sortment, in EUR per green tonne. Metadata only — not read by the economy.</summary>
public sealed record SortmentBand(Sortment Sortment, ValueBand EurPerTonne, EvidenceTag Evidence);

/// <summary>Read-only timber metadata. Deliberately NOT wired into Phase 2B economy or any price table.</summary>
public sealed record TimberProfile(
    TimberRole Role,
    double GreenDensityTPerM3,
    EvidenceTag GreenDensityEvidence,
    ValueBand GreenDensitySensitivity,
    IReadOnlyList<SortmentBand> SortmentBands,
    bool PremiumBuyerRequired,
    ValueBand? QualityStemInspectionTriggerDbhCm,
    ValueBand? HeartRotInspectionRiskOnsetYears,
    string Notes);

public sealed record SilvicultureProfile(
    Level LightDemand,
    Level BrowseSensitivity,
    Level? FormSensitivity,
    Level? BrowseRecovery,
    PruningRelevance PruningRelevance,
    ValueBand? PruningTargetHeightM,
    bool SelfPruning,
    bool ThinningMoreImportantThanPruning,
    string Notes);

/// <summary>
/// Crown model for one Wave 2 species. Potential (open-grown) radius comes from a species-specific fit; the competition
/// response is shared: r_target = r_pot / sqrt(1 + ci / ci50), where ci is the Hegyi competition index.
/// </summary>
public abstract record CrownProfile(
    ShadeTolerance ShadeTolerance,
    EvidenceTag PotentialRadiusEvidence,
    string PotentialRadiusSource,
    double Ci50Default,
    ValueBand Ci50Band,
    EvidenceTag Ci50Evidence)
{
    /// <summary>Lower bound on any potential radius so very small stems stay finite and positive (Calibration).</summary>
    public const double MinimumRadiusM = 0.10;

    public static EvidenceTag MinimumRadiusEvidence { get; } = EvidenceTag.Of(EvidenceClass.Calibration);

    /// <summary>Potential (uncompeted) crown radius in metres for a stem diameter at breast height in cm.</summary>
    public abstract double ComputePotentialCrownRadiusM(double dbhCm);

    /// <summary>Competition-reduced target radius: r_pot / sqrt(1 + ci / ci50). Negative ci is treated as zero.</summary>
    public double ComputeTargetCrownRadiusM(double dbhCm, double hegyiCi)
    {
        double ci = hegyiCi > 0.0 ? hegyiCi : 0.0;
        return ComputePotentialCrownRadiusM(dbhCm) / Math.Sqrt(1.0 + ci / Ci50Default);
    }

    /// <summary>Non-finite or negative diameters are treated as zero.</summary>
    protected static double SanitiseDbh(double dbhCm) => double.IsFinite(dbhCm) && dbhCm > 0.0 ? dbhCm : 0.0;
}

/// <summary>Scots pine: Turkish open-grown quadratic crown-width fit [Transfer], clamped at its upper fitted range.</summary>
public sealed record ScotsPineCrownProfile() : CrownProfile(
    ShadeTolerance.LightModerate,
    EvidenceTag.Of(EvidenceClass.Transfer),
    "Turkish open-grown Scots pine crown-width fit: CW = 0.6874 + 0.2358 D - 0.0011 D^2 (m, D in cm); r = CW / 2",
    4.75,
    new ValueBand(4.0, 5.5),
    EvidenceTag.Of(EvidenceClass.Calibration))
{
    public const double A0 = 0.6874;
    public const double A1 = 0.2358;
    public const double A2 = -0.0011;

    /// <summary>The quadratic is not extrapolated above this diameter; larger stems use the radius at the clamp.</summary>
    public const double DbhClampCm = 58.6;

    /// <summary>Below about this diameter the source fit has no data; the MinimumRadiusM floor guarantees positivity.</summary>
    public const double JuvenileContinuityBelowCm = 3.2;

    public override double ComputePotentialCrownRadiusM(double dbhCm)
    {
        double d = Math.Min(SanitiseDbh(dbhCm), DbhClampCm);
        double crownWidthM = A0 + A1 * d + A2 * d * d;
        return Math.Max(MinimumRadiusM, crownWidthM / 2.0);
    }
}

/// <summary>
/// Power-law crown radius r = exp(b0) * D^b1 * exp(bias), with a log-back-transform bias correction term.
/// </summary>
public sealed record PowerLawCrownProfile(
    ShadeTolerance Tolerance,
    EvidenceTag Evidence,
    string Source,
    double Intercept,
    double Exponent,
    double BiasCorrection,
    double Ci50,
    ValueBand Band) : CrownProfile(Tolerance, Evidence, Source, Ci50, Band, EvidenceTag.Of(EvidenceClass.Calibration))
{
    public override double ComputePotentialCrownRadiusM(double dbhCm)
    {
        double d = SanitiseDbh(dbhCm);
        double radius = Math.Exp(Intercept) * Math.Pow(d, Exponent) * Math.Exp(BiasCorrection);
        return Math.Max(MinimumRadiusM, radius);
    }
}

public sealed record SpeciesProfileCard(
    string SpeciesId,
    string DisplayName,
    ReadinessProfile Readiness,
    ColonisationProfile Colonisation,
    CrownProfile Crown,
    SilvicultureProfile Silviculture,
    TimberProfile Timber);

/// <summary>
/// Read-only profile cards for the three Wave 2 native species. None of these is consumed by the annual loop, economy,
/// saves or UI; they exist so later wiring can be reviewed against one cited, evidence-tagged source.
/// No Sitka constant is reused here: every value below is specific to its species.
/// </summary>
public static class FutureSpeciesCatalog
{
    private static EvidenceTag Cal => EvidenceTag.Of(EvidenceClass.Calibration);

    public static SpeciesProfileCard ScotsPine { get; } = new(
        SpeciesMoistureTable.ScotsPine,
        "Scots pine",
        new ReadinessProfile(
            ReadinessStatus.ReadyForInitialDesign,
            new ValueBand(10, 60), new ValueBand(100, 5000), new ValueBand(10, 100), 5.0, null,
            "Open-grown crown fit applied to an initial-design even-aged stand within the Irish GROWFOR domain.",
            "Domain from Irish GROWFOR: age 10-60 years, 100-5,000 stems/ha, 10-100 m2/ha basal area, top height >= 5 m. " +
            "Crown fit transferred from Turkish open-grown stands; Irish provenance/site response is not separately fitted."),
        new ColonisationProfile(ExternalColonisation.AuthoredOnly, true, false, null),
        new ScotsPineCrownProfile(),
        new SilvicultureProfile(
            Level.High, Level.MediumHigh, null, null,
            PruningRelevance.High, new ValueBand(5.0, 6.0), false, false,
            "Light-demanding; high pruning relevance for clear butt logs, target 5-6 m."),
        new TimberProfile(
            TimberRole.PrimaryCommercialSoftwood,
            1.02, EvidenceTag.Of(EvidenceClass.Empirical, EvidenceClass.Inference), new ValueBand(0.85, 1.05),
            Array.AsReadOnly(new[]
            {
                new SortmentBand(Sortment.Sawlog, new ValueBand(50, 85), Cal),
                new SortmentBand(Sortment.SmallRoundwood, new ValueBand(30, 60), Cal),
                new SortmentBand(Sortment.PulpChip, new ValueBand(25, 50), Cal),
            }),
            false, null, null,
            "Provisional Irish bands, EUR per green tonne; metadata only, not wired to the economy."));

    public static SpeciesProfileCard DownyBirch { get; } = new(
        SpeciesMoistureTable.DownyBirch,
        "Downy birch",
        new ReadinessProfile(
            ReadinessStatus.PartialWithAbstractions,
            null, null, null, null, 8,
            "Fast pioneer flush -> moderated competition-sensitive adult",
            "Irish YC8 potential. Crown fit is a proxy borrowed from UK silver birch amenity trees; timber value depends on a premium buyer."),
        new ColonisationProfile(ExternalColonisation.AuthoredOnly, true, false, null),
        new PowerLawCrownProfile(
            ShadeTolerance.Light,
            EvidenceTag.Of(EvidenceClass.Inference, EvidenceClass.Calibration),
            "UK silver birch amenity-tree proxy: r = exp(-0.80284) * D^0.64367 * exp(0.04399)",
            -0.80284, 0.64367, 0.04399, 4.25, new ValueBand(3.5, 5.0)),
        new SilvicultureProfile(
            Level.High, Level.MediumHigh, Level.MediumHigh, Level.High,
            PruningRelevance.Moderate, null, false, false,
            "Pruning only on the quality-stem path; browse/form damage is common but recovery is high."),
        new TimberProfile(
            TimberRole.SecondaryCommercial,
            0.93, EvidenceTag.Of(EvidenceClass.Empirical, EvidenceClass.Inference), new ValueBand(0.80, 1.10),
            Array.AsReadOnly(new[] { new SortmentBand(Sortment.Bulk, new ValueBand(25, 60), Cal) }),
            true, new ValueBand(30, 35), null,
            "Bulk band is Calibration; quality stems are flagged for inspection at 30-35 cm DBH."));

    public static SpeciesProfileCard CommonAlder { get; } = new(
        SpeciesMoistureTable.CommonAlder,
        "Common alder",
        new ReadinessProfile(
            ReadinessStatus.PartialWithAbstractions,
            null, null, null, null, null,
            "Very fast early pioneer -> sharp moderation; short-lived with late heart-rot risk",
            "Crown fit from UK amenity alder. Late heart-rot risk is expressed only through the timber heart-rot inspection flag " +
            "(onset 50-60 years), never as a mortality rule."),
        new ColonisationProfile(ExternalColonisation.AuthoredOnly, true, true, null),
        new PowerLawCrownProfile(
            ShadeTolerance.Moderate,
            EvidenceTag.Of(EvidenceClass.Empirical, EvidenceClass.Inference),
            "UK amenity common alder fit: r = exp(-0.38257) * D^0.52772 * exp(0.05982)",
            -0.38257, 0.52772, 0.05982, 4.75, new ValueBand(4.0, 5.5)),
        new SilvicultureProfile(
            Level.High, Level.LowMedium, null, Level.High,
            PruningRelevance.Low, null, true, true,
            "Self-pruning; thinning matters more than pruning for stem quality."),
        new TimberProfile(
            TimberRole.SecondaryCommercial,
            0.842, EvidenceTag.Of(EvidenceClass.Transfer), new ValueBand(0.75, 0.95),
            Array.AsReadOnly(new[] { new SortmentBand(Sortment.Bulk, new ValueBand(25, 60), Cal) }),
            true, null, new ValueBand(50, 60),
            "Heart-rot inspection risk from 50-60 years: flag/downgrade only, never deterministic death."));

    public static IReadOnlyList<SpeciesProfileCard> All { get; } = Array.AsReadOnly(new[] { ScotsPine, DownyBirch, CommonAlder });
}
