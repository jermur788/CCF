namespace SitkaP1.Core.SpeciesWave2;

// Species Wave 2 shared vocabulary (SPEC-multi-2d-species-wave2-profiles-V1).
// Pure, immutable data only: nothing in this namespace is wired into the annual loop, economy, saves or UI.

/// <summary>Coarse site-moisture classes used by the Wave 2 species response table.</summary>
public enum SiteMoistureClass
{
    /// <summary>Legacy sites with no moisture class. Always returns (1.0, 1.0) so Scenario One woodlands never shift.</summary>
    NeutralLegacy,
    DryFreeDraining,
    FreshMoistMineral,
    SeasonallyWetAerated,
    WetMovingAlluvial,
    StagnantPoorBog,
}

/// <summary>How strongly a constant is supported, following the project's evidence vocabulary.</summary>
public enum EvidenceClass
{
    /// <summary>Measured or fitted directly for this species in a comparable setting.</summary>
    Empirical,
    /// <summary>Measured for this species elsewhere (another region or growing condition) and transferred.</summary>
    Transfer,
    /// <summary>Reasoned from a proxy species or qualitative sources rather than a direct fit.</summary>
    Inference,
    /// <summary>A deliberate gameplay or simulation tuning value, not an ecological measurement.</summary>
    Calibration,
}

/// <summary>
/// Evidence label for one constant. Some constants carry a mixed label (for example "Empirical/Inference"), so an
/// optional secondary class is allowed. The primary class is always the stronger claim listed first in the spec table.
/// </summary>
public readonly record struct EvidenceTag(EvidenceClass Primary, EvidenceClass? Secondary = null)
{
    public static EvidenceTag Of(EvidenceClass primary) => new(primary);
    public static EvidenceTag Of(EvidenceClass primary, EvidenceClass secondary) => new(primary, secondary);

    /// <summary>True when either the primary or the secondary class equals <paramref name="evidence"/>.</summary>
    public bool Includes(EvidenceClass evidence) => Primary == evidence || Secondary == evidence;

    public override string ToString() => Secondary is { } second ? $"{Primary}+{second}" : Primary.ToString();
}

/// <summary>
/// The two separate moisture effects on a species. Establishment and growth are deliberately kept apart and must never
/// be pre-multiplied: a site can let seedlings establish well but grow poorly, or the reverse.
/// </summary>
public readonly record struct SiteResponse(double EstablishmentMultiplier, double GrowthMultiplier);

/// <summary>An inclusive numeric range (Low &lt;= High).</summary>
public readonly record struct ValueBand(double Low, double High)
{
    public bool Contains(double value) => value >= Low && value <= High;
}
