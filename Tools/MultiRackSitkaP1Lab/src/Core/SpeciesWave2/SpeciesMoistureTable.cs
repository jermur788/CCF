namespace SitkaP1.Core.SpeciesWave2;

/// <summary>
/// Suitability tiers used by the moisture table. Each tier maps to one Calibration multiplier
/// (G = 1.00, M = 0.70, L = 0.35, X = 0.10). X is "very poor", not zero: the table never makes survival impossible.
/// </summary>
public enum SuitabilityTier
{
    /// <summary>G — good.</summary>
    Good,
    /// <summary>M — moderate.</summary>
    Moderate,
    /// <summary>L — limited.</summary>
    Limited,
    /// <summary>X — very poor.</summary>
    VeryPoor,
}

/// <summary>One species x moisture cell: the tiers for establishment and growth, the resolved response and its evidence.</summary>
public readonly record struct MoistureCell(
    SuitabilityTier EstablishmentTier,
    SuitabilityTier GrowthTier,
    SiteResponse Response,
    EvidenceTag Evidence);

/// <summary>
/// Wave 2 species x site-moisture response table for all six species. Read-only lookup; it is not consulted by the live
/// annual loop. The numeric tier values are gameplay Calibration defaults; the tier assignment per cell is the
/// authored design table from the spec, also labelled Calibration until per-cell sources are reviewed.
/// </summary>
public static class SpeciesMoistureTable
{
    public const string SitkaSpruce = "sitka-spruce";
    public const string SessileOak = "sessile-oak";
    public const string EuropeanBeech = "european-beech";
    public const string ScotsPine = "scots-pine";
    public const string DownyBirch = "downy-birch";
    public const string CommonAlder = "common-alder";

    /// <summary>All six species ids in a stable order.</summary>
    public static IReadOnlyList<string> SpeciesIds { get; } = Array.AsReadOnly(new[]
    {
        SitkaSpruce, SessileOak, EuropeanBeech, ScotsPine, DownyBirch, CommonAlder,
    });

    /// <summary>All six moisture classes in declaration order.</summary>
    public static IReadOnlyList<SiteMoistureClass> MoistureClasses { get; } =
        Array.AsReadOnly((SiteMoistureClass[])Enum.GetValues(typeof(SiteMoistureClass)));

    /// <summary>Evidence label for the four tier multipliers and for each authored cell.</summary>
    public static EvidenceTag TierEvidence { get; } = EvidenceTag.Of(EvidenceClass.Calibration);

    /// <summary>Evidence label for the NeutralLegacy identity row (a design invariant, not an ecological claim).</summary>
    public static EvidenceTag NeutralLegacyEvidence { get; } = EvidenceTag.Of(EvidenceClass.Calibration);

    /// <summary>The Calibration multiplier for one tier.</summary>
    public static double TierMultiplier(SuitabilityTier tier) => tier switch
    {
        SuitabilityTier.Good => 1.00,
        SuitabilityTier.Moderate => 0.70,
        SuitabilityTier.Limited => 0.35,
        SuitabilityTier.VeryPoor => 0.10,
        _ => throw new ArgumentOutOfRangeException(nameof(tier), tier, "unknown suitability tier"),
    };

    private const SuitabilityTier G = SuitabilityTier.Good;
    private const SuitabilityTier M = SuitabilityTier.Moderate;
    private const SuitabilityTier L = SuitabilityTier.Limited;
    private const SuitabilityTier X = SuitabilityTier.VeryPoor;

    // Authored tiers (establishment, growth) in the order
    // DryFreeDraining, FreshMoistMineral, SeasonallyWetAerated, WetMovingAlluvial, StagnantPoorBog.
    private static readonly IReadOnlyDictionary<string, (SuitabilityTier Est, SuitabilityTier Grow)[]> AuthoredTiers =
        new Dictionary<string, (SuitabilityTier, SuitabilityTier)[]>(StringComparer.Ordinal)
        {
            [SitkaSpruce] = new[] { (M, L), (G, G), (G, G), (M, M), (L, L) },
            [SessileOak] = new[] { (M, M), (G, G), (M, M), (L, L), (X, X) },
            [EuropeanBeech] = new[] { (M, M), (G, G), (L, L), (X, X), (X, X) },
            [ScotsPine] = new[] { (G, G), (G, G), (M, M), (M, L), (L, L) },
            [DownyBirch] = new[] { (M, M), (G, G), (G, G), (G, M), (M, L) },
            [CommonAlder] = new[] { (L, L), (G, G), (G, G), (G, G), (L, X) },
        };

    /// <summary>The full cell (tiers, response and evidence) for one species and moisture class.</summary>
    public static MoistureCell Cell(string speciesId, SiteMoistureClass moisture)
    {
        if (!AuthoredTiers.TryGetValue(speciesId ?? "", out var row))
            throw new ArgumentException($"unknown species id '{speciesId}'", nameof(speciesId));

        if (moisture == SiteMoistureClass.NeutralLegacy)
            return new MoistureCell(G, G, new SiteResponse(1.0, 1.0), NeutralLegacyEvidence);

        int index = moisture switch
        {
            SiteMoistureClass.DryFreeDraining => 0,
            SiteMoistureClass.FreshMoistMineral => 1,
            SiteMoistureClass.SeasonallyWetAerated => 2,
            SiteMoistureClass.WetMovingAlluvial => 3,
            SiteMoistureClass.StagnantPoorBog => 4,
            _ => throw new ArgumentOutOfRangeException(nameof(moisture), moisture, "unknown moisture class"),
        };
        var (est, grow) = row[index];
        return new MoistureCell(est, grow, new SiteResponse(TierMultiplier(est), TierMultiplier(grow)), TierEvidence);
    }

    /// <summary>The separate establishment and growth multipliers for one species and moisture class.</summary>
    public static SiteResponse Response(string speciesId, SiteMoistureClass moisture) => Cell(speciesId, moisture).Response;

    /// <summary>
    /// True only for StagnantPoorBog. Biological survival on a class never implies planting approval: intact bog is a
    /// habitat-protection question that the table deliberately does not answer.
    /// </summary>
    public static bool HabitatProtectionCaution(SiteMoistureClass moisture) => moisture == SiteMoistureClass.StagnantPoorBog;
}
