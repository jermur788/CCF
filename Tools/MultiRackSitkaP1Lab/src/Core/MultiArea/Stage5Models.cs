namespace SitkaP1.Core;

// Multi Stage 5 (spec multi-stage5-sitka-regeneration-v1, APPROVED 2026-11-23): the regeneration model switch, its options
// and every constant the Stage 5 rules read. Session-only: nothing here is saved, and Regeneration 0 never reads any of it.
//
// Evidence tags used below (spec §11):
//   TRANSFER               a value copied from the Unity Sitka asset or Unity code at main 5e79e986 (gameplay values).
//   GAMEPLAY CALIBRATION   a Unity [C] calibration or a spec gameplay choice. Not a measured Irish value.
//   SIMULATION ABSTRACTION a rule this engine needs that has no field measurement behind it (grid scaling, block cap, ...).
// Nothing here is a measured Irish establishment probability, and the light index is not calibrated PAR.

/// <summary>Which natural-regeneration model a multi-area session uses. The default everywhere is <see cref="Off"/>.</summary>
internal enum MultiAreaRegenerationModelVersion
{
    /// <summary>No regeneration: the session is byte-identical to the Stage 4 (or earlier) path it selects.</summary>
    Off = 0,

    /// <summary>
    /// Stage 5: Sitka seed rain, establishment into abundance bands, bramble/bracken competition, light browsing and
    /// promotion into CrownModel 1 adults. Needs Adult 1, Mortality 1 and Crown 1 together.
    /// </summary>
    SitkaCohortCompetitionBrowse = 1,
}

/// <summary>The Stage 5 inputs that are not model switches. <see cref="Default"/> is the production setting.</summary>
internal sealed record Stage5Options
{
    /// <summary>Ground-cell size: 2.5 m (default) or 5.0 m, the two grids the canopy/light evaluator supports.</summary>
    public double GridCellM { get; init; } = CanopyLightEvaluator.DefaultLightCellM;

    /// <summary>Background browsing pressure, 0..1. GAMEPLAY CALIBRATION default 0.2 (spec §3.2).</summary>
    public float BackgroundBrowsePressure { get; init; } = Stage5Constants.DefaultBrowsePressure;

    /// <summary>
    /// Seed of the woodland-wide STAGE5-MAST stream. Null means the experiment's generator seed. SIMULATION ABSTRACTION:
    /// Unity rolls mast from the stand RNG; Stage 5 uses one isolated woodland stream instead (owner Q1).
    /// </summary>
    public int? MastSeed { get; init; }

    public static readonly Stage5Options Default = new();

    internal Stage5Options RequireValid()
    {
        CanopyLightEvaluator.RequireSupportedGrid(GridCellM);
        if (!float.IsFinite(BackgroundBrowsePressure) || BackgroundBrowsePressure < 0f || BackgroundBrowsePressure > 1f)
            throw new P1Refusal("Stage5BrowsePressureInvalid", "background browse pressure must be a finite value in [0, 1]");
        return this;
    }
}

/// <summary>Every constant the Stage 5 rules read, with its evidence tag.</summary>
internal static class Stage5Constants
{
    // ---- Juvenile Sitka (TRANSFER: Unity SitkaSpruce.asset @ 5e79e986) ----
    public const float RegenHeightGrowthMPerYear = 0.35f;
    public const float RegenInitialHeightM = 0.15f;
    public const float PromotionHeightM = 3.5f;
    /// <summary>Owner Q2: Unity's promotionMinimumLight is not serialised in SitkaSpruce.asset, so it reads 0 (no gate).</summary>
    public const float PromotionMinimumLight = 0f;
    public const float RegenDensityPerEstablishment = 0.5f;
    public const float RegenDensityMax = 1.5f;
    public const float RegenMortalityUnderPoorLight = 0.2f;
    public const float RegenPoorLightThreshold = 0.15f;
    public static readonly float[] LightResponseLight = { 0f, 0.1f, 0.2f, 0.25f, 0.55f, 1f };
    public static readonly float[] LightResponseFactor = { 0f, 0.1f, 0.3f, 0.5f, 0.9f, 0.8f };

    // ---- Seed (TRANSFER: SitkaSpruce.asset) ----
    public const float MaturityOnsetYears = 20f;
    public const float MaturityFullYears = 30f;
    public const float SeedPotentialPerMatureTree = 1f;
    public const float SeedDispersalScaleM = 20f;
    public const float SeedDispersalCutoffM = 150f;
    public const float SeedSaturationS50 = 10f;
    /// <summary>TRANSFER (Unity ComputeSpeciesSeedRain): crown factor clamp01(crownRadius / 4 m).</summary>
    public const float SeedCrownReferenceRadiusM = 4f;

    // ---- Mast (TRANSFER: SitkaSpruce.asset; owner Q1 cumulative thresholds) ----
    public const float MastGoodProbability = 0.15f;
    public const float MastPoorProbability = 0.35f;
    public const float MastGoodMultiplier = 1.5f;
    public const float MastPoorMultiplier = 0.4f;
    public const float MastNormalMultiplier = 1f;

    // ---- Establishment and bands (TRANSFER: Unity code literals, RegenerationModel / ForestEcologyCell) ----
    /// <summary>Establishment at or below this value creates nothing (Unity EstablishNewBands literal 0.01f).</summary>
    public const float MinimumEstablishment = 0.01f;
    /// <summary>Unity RegenerationModel.RepresentationThreshold, defined for a 5 m cell; scaled per §4.</summary>
    public const float RepresentationThreshold = 0.01f;
    /// <summary>Unity ForestEcologyCell.MaxBandsPerSpeciesOrigin: represented bands per cell (the accumulator is extra).</summary>
    public const int MaxRepresentedBands = 4;
    /// <summary>Unity AdmitDensity floor on the capacity divisor (Mathf.Max(0.01f, RegenDensityMax)).</summary>
    public const float CapacityFloor = 0.01f;

    // ---- Browsing (GAMEPLAY CALIBRATION: Unity JuvenileEcologyRules [C]; palatability/heights TRANSFER from the asset) ----
    public const float DefaultBrowsePressure = 0.2f;
    public const float BrowsePalatability = 0.15f;
    public const float BrowseFullVulnerabilityHeightM = 0.5f;
    public const float BrowseEscapeHeightM = 0.8f;
    public const float BrowseProbabilityCap = 0.95f;
    public const float BrowseHeightIncrementLoss = 0.9f;
    public const float BrowseExtraMortality = 0.04f;

    // ---- Bramble/bracken (GAMEPLAY CALIBRATION: Unity UnderstoreyCompetitionCalibration [C]) ----
    public const float UndergrowthStrength = 0.35f;
    public const float UndergrowthFullHeightM = 0.3f;
    public const float UndergrowthEscapeHeightM = 1.5f;
    public const float UndergrowthGain = 0.30f;
    public const float UndergrowthLoss = 0.45f;
    public const float UndergrowthInitialFraction = 0.25f;
    public const float UndergrowthTargetMaximum = 0.8f;
    public const float UndergrowthShadeTargetFraction = 0.1f;

    // ---- Neutral site (SIMULATION ABSTRACTION: no soil map, no opening-decay history is authored; spec §3 step 11) ----
    public const float SiteProductivity = 1f;
    public const float SoilStability = 1f;
    public const float EstablishmentSuitability = 1f;

    // ---- Grid scaling and promotion (SIMULATION ABSTRACTION, owner-approved §4 / Q5, amended by owner Option A 2026-11-23) ----
    /// <summary>Unity's own cell size; the area factor is a = (cell / 5 m)², so a 5.0 m run reproduces Unity's scaling.</summary>
    public const double UnityCellM = 5.0;
    /// <summary>
    /// The aligned promotion block (world frame): at most one promotion per block per year, the block's other bands of the
    /// promoted cohort leave with it, and its yearly budget is (in-polygon cells) × a (spec §4, Option A). The block credit
    /// needs no constant of its own: its unit, the number of cells in a full block, follows from this size and the grid.
    /// </summary>
    public const double PromotionBlockM = 5.0;
    /// <summary>Jitter attempts inside the source cell before falling back to the (in-polygon) cell centre.</summary>
    public const int PromotionPlacementAttempts = 8;

    // ---- Promoted-tree size (GAMEPLAY CALIBRATION: Unity JuvenileEcologyRules.PromotionDbhCm, a [C] juvenile-to-tree proportion) ----
    public const float PromotionDbhCmPerMetreHeight = 1.5f;
    public const float PromotionDbhMinCm = 2f;
    public const float PromotionDbhMaxCm = 20f;
}

/// <summary>Evidence labels shown with Stage 5 output (spec §11).</summary>
internal static class Stage5Evidence
{
    public const string Calibration =
        "Seed kernel, light response, mast, browse and bramble/bracken constants: Gameplay Calibration / Transfer from Unity, not measured Irish values";
    public const string Light = "Light is the modelled Unity k10a10 index, not calibrated PAR";
    public const string GridScaling =
        "2.5 m grid scaling (capacity, request and representation threshold × (cell/5 m)²; at most one promotion per 5 m block per year, " +
        "the block's other bands of that cohort exported into the same tree, and a block budget proportional to its in-polygon cells): Simulation Abstraction";
    public const string NeutralSite = "Site productivity, soil stability and establishment suitability are a neutral 1.0 (no soil or history data)";
    public const string NotBeneficial =
        "Regeneration output is a consequence of the stated rules only; it is not an ecological forecast and not a claim that regeneration is beneficial";
    public const string ClearingTemporary =
        "ClearUndergrowth is selective and temporary: bramble and bracken regrow toward their light target from the next year";

    public static readonly IReadOnlyList<string> All =
        Array.AsReadOnly(new[] { Calibration, Light, GridScaling, NeutralSite, NotBeneficial, ClearingTemporary });
}
