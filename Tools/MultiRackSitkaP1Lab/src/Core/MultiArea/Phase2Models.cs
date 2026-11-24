namespace SitkaP1.Core;

// MULTI Phase 2 (spec multi-phase2-oak-beech-v1, APPROVED 2026-11-24): the species model switch, its options and every
// Sessile Oak / European Beech constant the Phase 2 rules read. Session-only: nothing here is saved, and Species 0 never
// reads any of it.
//
// Evidence tags used below (spec §11). None is IRISH DIRECT; the light index is a modelled index, not calibrated PAR.
//   TRANSFER               a value copied from a Unity species asset or Unity code at 5b757d9b (gameplay values).
//   GAMEPLAY CALIBRATION   a Unity [C] calibration or a spec gameplay choice. Not a measured Irish value.
//   ORDINAL                a coarse ordinal class step (D-061 growing-space weights), magnitude itself a calibration.
//   SIMULATION ABSTRACTION a rule this engine needs that has no field measurement behind it.
//   NEW OWNER RULE         an owner decision with no Unity source (the 0.7 m adult spacing).

/// <summary>Which species set a multi-area session uses. The default everywhere is <see cref="Off"/> (Sitka only).</summary>
internal enum SpeciesModelVersion
{
    /// <summary>Sitka only: the session is byte-identical to the Stage 5 (or earlier) path it selects.</summary>
    Off = 0,

    /// <summary>Phase 2: Sitka plus Sessile Oak and European Beech. Needs 1/1/1/1.</summary>
    ThreeSpeciesMixed = 1,
}

/// <summary>The Phase 2 inputs that are not model switches. <see cref="Default"/> is the production setting.</summary>
internal sealed record Phase2Options
{
    /// <summary>
    /// Seed of the PHASE2-MAST-sessile-oak stream. Null means the session's Stage 5 mast seed (the streams stay independent
    /// because their keys differ). SIMULATION ABSTRACTION, as Stage 5's woodland mast stream.
    /// </summary>
    public int? OakMastSeed { get; init; }

    /// <summary>Seed of the PHASE2-MAST-beech stream. Null means the session's Stage 5 mast seed.</summary>
    public int? BeechMastSeed { get; init; }

    /// <summary>
    /// Genesis species of named starting stems (TreeId → "sessile-oak" or "beech"). Every other stem is Sitka. Empty by
    /// default: MA-1 is all Sitka. Test fixtures (MA-1-MIXED-v1) use this; no generator or default changes.
    /// </summary>
    public IReadOnlyDictionary<string, string> GenesisSpecies { get; init; } =
        new Dictionary<string, string>(StringComparer.Ordinal);

    public static readonly Phase2Options Default = new();

    internal Phase2Options RequireValid()
    {
        ArgumentNullException.ThrowIfNull(GenesisSpecies);
        foreach (var (treeId, speciesId) in GenesisSpecies)
            if (string.IsNullOrWhiteSpace(treeId) || Phase2Species.BroadleafOrNull(speciesId) is null)
                throw new P1Refusal("Phase2GenesisSpeciesInvalid", "genesis species must name a tree and sessile-oak or beech");
        return this;
    }
}

/// <summary>
/// One broadleaf species' constants (Unity TreeSpeciesDefinition, assets SessileOak.asset and BeechSpecies.asset at
/// 5b757d9b, verified field by field in Step B Stage 1). Float32 exactly as serialised.
/// </summary>
internal sealed record Phase2Species
{
    public required string SpeciesId { get; init; }

    // ---- Adult growth (GAMEPLAY CALIBRATION, Unity [C] ordinal production calibration v1) ----
    public required float PotentialDbhGrowthCm { get; init; }
    public required float MaxDbhCm { get; init; }
    public required float PotentialHeightGrowthM { get; init; }
    public required float MaxHeightM { get; init; }
    public required float Ci50 { get; init; }
    public required float CrownRelaxation { get; init; }

    // ---- Growing-space weight (ORDINAL, D-061 class High 2.0 / Intermediate 1.5) ----
    public required double GrowingSpaceWeight { get; init; }

    // ---- Crown potential (TRANSFER). Linear unless UsesPowerLawCrown; Beech's serialised linear fields are never stored. ----
    public required bool UsesPowerLawCrown { get; init; }
    public float CrownRadiusIntercept { get; init; }
    public float CrownRadiusPerCmDbh { get; init; }
    public float CrownAreaLogIntercept { get; init; }
    public float CrownAreaLogSlope { get; init; }
    public const float CrownRadiusFloorM = 0.1f;

    // ---- Seed and mast (GAMEPLAY CALIBRATION) ----
    public required float MaturityOnsetYears { get; init; }
    public required float MaturityFullYears { get; init; }
    public required float SeedSaturationS50 { get; init; }
    public required float SeedDispersalScaleM { get; init; }
    public required float SeedDispersalCutoffM { get; init; }
    public required float MastGoodProbability { get; init; }
    public required float MastPoorProbability { get; init; }
    public required float MastGoodMultiplier { get; init; }
    public required float MastNormalMultiplier { get; init; }
    public required float MastPoorMultiplier { get; init; }

    // ---- Juveniles (GAMEPLAY CALIBRATION) ----
    public required float RegenInitialHeightM { get; init; }
    public required float RegenHeightGrowthMPerYear { get; init; }
    public required float PromotionHeightM { get; init; }
    public required float PromotionMinimumLight { get; init; }
    public required float[] GrowthLight { get; init; }
    public required float[] GrowthFactor { get; init; }
    /// <summary>Unity useDistinctJuvenileLightResponses: distinct survival/establishment tables (Oak) or the shared one (Beech).</summary>
    public required bool UsesDistinctJuvenileLightResponses { get; init; }
    public float[] SurvivalLight { get; init; } = Array.Empty<float>();
    public float[] SurvivalFactor { get; init; } = Array.Empty<float>();
    public float[] EstablishmentLight { get; init; } = Array.Empty<float>();
    public float[] EstablishmentFactor { get; init; } = Array.Empty<float>();
    public required float RegenMortalityUnderPoorLight { get; init; }
    public required float RegenPoorLightThreshold { get; init; }
    public required float RegenDensityPerEstablishment { get; init; }
    public required float RegenDensityMax { get; init; }

    // ---- Browsing (GAMEPLAY CALIBRATION, asset values) ----
    public required float BrowsePalatability { get; init; }
    public required float BrowseFullVulnerabilityHeightM { get; init; }
    public required float BrowseEscapeHeightM { get; init; }

    public const string OakId = "sessile-oak";
    public const string BeechId = "beech";
    public const string SitkaId = "sitka-spruce";

    /// <summary>Sessile Oak (SessileOak.asset).</summary>
    public static readonly Phase2Species Oak = new()
    {
        SpeciesId = OakId,
        PotentialDbhGrowthCm = 0.8f, MaxDbhCm = 120f, PotentialHeightGrowthM = 0.65f, MaxHeightM = 42.5f, Ci50 = 5f,
        CrownRelaxation = 0.12f, GrowingSpaceWeight = 2.0,
        UsesPowerLawCrown = false, CrownRadiusIntercept = 0.1f, CrownRadiusPerCmDbh = 0.1045f,
        MaturityOnsetYears = 40f, MaturityFullYears = 60f, SeedSaturationS50 = 5f, SeedDispersalScaleM = 4f, SeedDispersalCutoffM = 80f,
        MastGoodProbability = 0.15f, MastPoorProbability = 0.35f, MastGoodMultiplier = 1f, MastNormalMultiplier = 0.3f, MastPoorMultiplier = 0.075f,
        RegenInitialHeightM = 0.18f, RegenHeightGrowthMPerYear = 0.35f, PromotionHeightM = 3.5f, PromotionMinimumLight = 0.2f,
        GrowthLight = new[] { 0f, 0.01f, 0.05f, 0.1f, 0.15f, 0.2f, 0.3f, 0.4f, 0.5f, 1f },
        GrowthFactor = new[] { 0f, 0f, 0.02f, 0.07f, 0.15f, 0.3f, 0.6f, 0.8f, 1f, 1f },
        UsesDistinctJuvenileLightResponses = true,
        SurvivalLight = new[] { 0f, 0.01f, 0.05f, 0.1f, 0.15f, 0.2f, 0.3f, 1f },
        SurvivalFactor = new[] { 0.2f, 0.3f, 0.55f, 0.75f, 0.9f, 1f, 1f, 1f },
        EstablishmentLight = new[] { 0f, 0.01f, 0.05f, 0.1f, 1f },
        EstablishmentFactor = new[] { 0.75f, 0.8f, 0.95f, 1f, 1f },
        RegenMortalityUnderPoorLight = 0.2f, RegenPoorLightThreshold = 0.15f, RegenDensityPerEstablishment = 0.5f, RegenDensityMax = 1.5f,
        BrowsePalatability = 1f, BrowseFullVulnerabilityHeightM = 1.2f, BrowseEscapeHeightM = 1.8f,
    };

    /// <summary>European Beech (BeechSpecies.asset). promotionMinimumLight and useDistinctJuvenileLightResponses are not
    /// serialised, so they read their defaults 0 and false; the survival rule is the legacy poor-light rule.</summary>
    public static readonly Phase2Species Beech = new()
    {
        SpeciesId = BeechId,
        PotentialDbhGrowthCm = 0.9f, MaxDbhCm = 120f, PotentialHeightGrowthM = 0.75f, MaxHeightM = 42.5f, Ci50 = 6.5f,
        CrownRelaxation = 0.15f, GrowingSpaceWeight = 1.5,
        UsesPowerLawCrown = true, CrownAreaLogIntercept = 0.05f, CrownAreaLogSlope = 1.01f,
        MaturityOnsetYears = 40f, MaturityFullYears = 60f, SeedSaturationS50 = 5f, SeedDispersalScaleM = 6f, SeedDispersalCutoffM = 50f,
        MastGoodProbability = 0.15f, MastPoorProbability = 0.35f, MastGoodMultiplier = 1f, MastNormalMultiplier = 0.4f, MastPoorMultiplier = 0.1f,
        RegenInitialHeightM = 0.15f, RegenHeightGrowthMPerYear = 0.35f, PromotionHeightM = 3.5f, PromotionMinimumLight = 0f,
        GrowthLight = new[] { 0f, 0.02f, 0.05f, 0.1f, 0.2f, 0.35f, 1f },
        GrowthFactor = new[] { 0f, 0f, 0.15f, 0.35f, 0.65f, 1f, 1f },
        UsesDistinctJuvenileLightResponses = false,
        RegenMortalityUnderPoorLight = 0.2f, RegenPoorLightThreshold = 0.15f, RegenDensityPerEstablishment = 0.5f, RegenDensityMax = 1.5f,
        BrowsePalatability = 0.5f, BrowseFullVulnerabilityHeightM = 1.2f, BrowseEscapeHeightM = 1.8f,
    };

    /// <summary>The two broadleaves in ordinal SpeciesId order ("beech" &lt; "sessile-oak").</summary>
    public static readonly IReadOnlyList<Phase2Species> Broadleaves = Array.AsReadOnly(new[] { Beech, Oak });

    /// <summary>The broadleaf with this exact ID, or null (Sitka and unknown IDs).</summary>
    public static Phase2Species? BroadleafOrNull(string? speciesId) => speciesId switch
    {
        OakId => Oak,
        BeechId => Beech,
        _ => null,
    };

    /// <summary>
    /// Open-grown potential crown radius in metres (Unity PotentialCrownRadiusM, float32): Oak linear
    /// max(0.1, 0.1 + 0.1045·DBH); Beech power law CPA = exp(0.05 + 1.01·ln(max(0.1, DBH))), r = max(0.1, sqrt(CPA/π)).
    /// Unity Mathf.Exp/Log/Sqrt evaluate in double and cast to float, so the same is done here.
    /// </summary>
    public float PotentialCrownRadiusM(float dbhCm)
    {
        if (UsesPowerLawCrown)
        {
            float dbh = MathF.Max(0.1f, dbhCm);
            float crownArea = (float)Math.Exp(CrownAreaLogIntercept + CrownAreaLogSlope * (float)Math.Log(dbh));
            return MathF.Max(CrownRadiusFloorM, (float)Math.Sqrt(crownArea / MathF.PI));
        }
        return MathF.Max(CrownRadiusFloorM, CrownRadiusIntercept + CrownRadiusPerCmDbh * dbhCm);
    }
}

/// <summary>Phase 2 constants that are not per species.</summary>
internal static class Phase2Constants
{
    // ---- Mixed density, generic component (GAMEPLAY CALIBRATION: Unity MixedDensityModel [C]) ----
    public const double OccupancyNormalization = 55.0;
    public const double OnsetOccupancy = 1.0;
    public const double GenericResponseStrength = 1.60;
    public const double GenericMaximumAnnualLoss = 0.50;
    public const double GenericResponseExponent = 2.0;
    public const double GenericAnnualBasalAreaBudgetShare = 0.125;
    public const double VulnerabilityExponent = 4.0;
    /// <summary>One Hegyi half-saturation for every species in victim choice (species-neutral), never the growth Ci50.</summary>
    public const double NeutralHalfSaturationCi = 5.0;
    /// <summary>ORDINAL: the Sitka reference growing-space weight.</summary>
    public const double SitkaGrowingSpaceWeight = 1.0;

    // ---- Broadleaf pruning (TRANSFER: Unity ForestTree.CanPrune species-aware branch; Teagasc 3.5/6 m lifts) ----
    public const int BroadleafMaxLifts = 2;
    public const int BroadleafMinimumYearsBetweenLifts = 3;
    public static readonly IReadOnlyList<float> BroadleafLiftTargetHeightsM = Array.AsReadOnly(new[] { 3.5f, 6f });

    // ---- Planting (TRANSFER: Unity ForestEcologyController nursery defaults; ScenarioOneManager spacing) ----
    public const int PlantedJuvenileAgeYears = 3;
    public const float PlantedJuvenileHeightM = 0.6f;
    public const float MinimumPlantingSpacingM = 0.5f;
    /// <summary>NEW OWNER RULE (owner decision 3): no Unity source; not a transfer.</summary>
    public const float MinimumAdultStemSpacingM = 0.7f;

    // ---- Shelters (TRANSFER: Unity BrowsingConditions / ScenarioOneManager; the 8-year life is a Unity [G] working assumption) ----
    public const float ShelterMatchRadiusM = 0.25f;
    public const int ShelterEffectiveYears = 8;

    // ---- Spot clearance (TRANSFER: Unity VegetationClearance.Planting, Mathf.Sqrt(1f / Mathf.PI), a circle of 1 m²) ----
    public static readonly float SpotClearRadiusM = (float)Math.Sqrt(1f / MathF.PI);
}

/// <summary>Evidence labels shown with Phase 2 output (spec §11, hygiene H2–H4).</summary>
internal static class Phase2Evidence
{
    public const string Calibration =
        "Oak and Beech growth, crown, maturity, mast, dispersal, light tables and browse constants: Gameplay Calibration / Transfer from Unity, not measured Irish values";
    public const string Ordinal = "Growing-space weights Sitka 1.0, Beech 1.5, Oak 2.0: Ordinal (D-061); magnitude Gameplay Calibration";
    public const string Density =
        "Occupancy normaliser 55, generic response 0.5 / 1.60 / exponent 2, basal-area budget 0.125, victim Ci50 5 and exponent 4: Gameplay Calibration, not a self-thinning law";
    public const string Shelter = "Shelter 8-year life, 0.25 m match radius and binary access: Transfer of a Unity working assumption, not a measured shelter life";
    public const string AdultSpacing = "The 0.7 m distance from living adult stems is a new owner rule with no Unity source (not a transfer)";
    public const string NotResolvePlanting =
        "Plant, Shelter and SpotClear are not a port of Unity ResolvePlanting: no nursery stock, cash, owner-minutes or offer identity exist here";
    public const string Abstraction =
        "Shared natural capacity, one natural promotion per 5 m block across all species, C2 crowns for planted recruits and selective spot clearance: Simulation Abstraction";
    public const string NotBeneficial = "Planting and mixing are not assumed beneficial; outcomes are consequences of the stated rules only";

    public static readonly IReadOnlyList<string> All =
        Array.AsReadOnly(new[] { Calibration, Ordinal, Density, Shelter, AdultSpacing, NotResolvePlanting, Abstraction, NotBeneficial });
}

/// <summary>The Phase 2 lineage in the staleness token (owner Q1).</summary>
internal static class Phase2Identity
{
    /// <summary>
    /// The area-model version with the species version appended ONLY when it is not 0, so every 0/0/0/0 … 1/1/1/1 token (and
    /// every hash built from one) stays byte-identical.
    /// </summary>
    public static string AreaModelVersion(string stage5OrEarlier, SpeciesModelVersion species) =>
        species == SpeciesModelVersion.Off
            ? stage5OrEarlier
            : stage5OrEarlier + "+species-v" + ((int)species).ToString(System.Globalization.CultureInfo.InvariantCulture);
}
