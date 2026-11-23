namespace SitkaP1.Core;

/// <summary>
/// The per-species constants the annual rules read (Multi #899 Stage 1). <see cref="Sitka"/> holds exactly the frozen values
/// the engine used before this profile existed, so CrownModel 0 output is unchanged bit for bit (pinned by
/// Model0FingerprintTests). Adding a profile is not a calibration: it only gives the existing numbers one named home.
/// </summary>
public sealed record SpeciesProfile
{
    public required string SpeciesId { get; init; }

    // Open-grown linear crown potential r_pot = max(floor, intercept + perCm * DBH_cm), metres (Tabbush & White 1988 via the
    // Scenario One Sitka asset; see CcfSourcePins).
    public required float CrownRadiusIntercept { get; init; }
    public required float CrownRadiusPerCmDbh { get; init; }
    public required float CrownRadiusFloorM { get; init; }

    /// <summary>Hegyi index at which growth response is halved (and, for CrownModel 1, crown area is halved).</summary>
    public required float Ci50 { get; init; }

    /// <summary>Annual crown relaxation rate k: the share of the remaining gap to the target closed in one year.</summary>
    public required float CrownRelaxation { get; init; }

    public required float PotentialDbhGrowthCm { get; init; }
    public required float DbhGrowthScaleCm { get; init; }

    // Hegyi neighbourhood: neighbours within the cutoff count; distance is floored to avoid a singularity at contact.
    public required float HegyiCutoffM { get; init; }
    public required float HegyiDistanceFloorM { get; init; }

    /// <summary>Unpruned Sitka spruce, exactly the pre-profile frozen values.</summary>
    public static readonly SpeciesProfile Sitka = new()
    {
        SpeciesId = "sitka-spruce",
        CrownRadiusIntercept = CcfSourcePins.CrownRadiusIntercept, // 0.9415
        CrownRadiusPerCmDbh = CcfSourcePins.CrownRadiusPerCmDbh,   // 0.07635
        CrownRadiusFloorM = CcfSourcePins.CrownRadiusFloorM,       // 0.1
        Ci50 = 5f,
        CrownRelaxation = 0.15f,
        PotentialDbhGrowthCm = 1.2f,
        DbhGrowthScaleCm = 120f,
        HegyiCutoffM = 8f,
        HegyiDistanceFloorM = 0.5f,
    };

    /// <summary>Open-grown potential crown radius in metres, evaluated in float32 exactly as the frozen rule.</summary>
    public float PotentialCrownRadiusM(float dbhCm) => MathF.Max(CrownRadiusFloorM, CrownRadiusIntercept + CrownRadiusPerCmDbh * dbhCm);
}

/// <summary>
/// Neutral site-condition slots (Multi #899 Stage 1). Every slot is a multiplier fixed at exactly 1 and NO rule reads any of
/// them in this pass: no site effect is modelled, so behaviour is unchanged. They exist so a later, separately approved
/// slice has one named place for site effects. A non-neutral value is refused rather than silently ignored.
/// </summary>
public sealed record SiteConditions
{
    public float MoistureDrainage { get; init; } = 1f;
    public float Fertility { get; init; } = 1f;
    public float ExposureClimate { get; init; } = 1f;
    public float RootingStability { get; init; } = 1f;

    public static readonly SiteConditions Neutral = new();

    public bool IsNeutral => MoistureDrainage == 1f && Fertility == 1f && ExposureClimate == 1f && RootingStability == 1f;

    /// <summary>Refuses any non-neutral value: no site effect is approved, so accepting one would hide that it does nothing.</summary>
    public SiteConditions RequireNeutral()
    {
        if (!IsNeutral)
            throw new P1Refusal("SiteEffectNotModelled", "no site effect is approved in this engine; only neutral site conditions are accepted");
        return this;
    }
}
