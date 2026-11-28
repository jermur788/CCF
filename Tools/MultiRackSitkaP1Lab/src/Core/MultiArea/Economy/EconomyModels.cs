using System.Globalization;

namespace SitkaP1.Core;

// CCF Multi 2D — Phase 2B-1 Economy, Labour & D-062 Timber Engine (spec multi-2d-phase2b-economy-v1).
// Pure .NET + System.Text.Json only; zero UI/HTTP/Unity dependencies; all monetary amounts in checked integer euro cents.

/// <summary>Opt-in switch for the Phase 2B Economy engine. Default everywhere is <see cref="Off"/>.</summary>
internal enum EconomyModelVersion
{
    /// <summary>Economy disabled: MultiAreaRoot.Economy stays null; zero financial state or mutation; history money fields read NOT RECORDED.</summary>
    Off = 0,

    /// <summary>Phase 2B Economy enabled: atomic Work Plan Quote → Approve → Execute → Settle under C-ECON.</summary>
    Enabled = 1,
}

/// <summary>Evidence provenance tier for every economic parameter and rule (spec multi-2d-phase2b-economy-v1 §2).</summary>
internal enum EconomyEvidenceLabel
{
    Empirical,
    ManagementGuidance,
    GameplayCalibration,
    SimulationAbstraction,
}

/// <summary>Who performs a Work Plan action. <see cref="Fell"/> is strictly <see cref="Contractor"/>-only.</summary>
internal enum WorkExecutor
{
    Contractor = 0,
    Landowner = 1,
}

/// <summary>Engine-derived selective CCF thinning stage for grouped contractor harvest pricing (€20–€23/t).</summary>
internal enum HarvestThinningStage
{
    FirstThinning = 0,
    SecondThinning = 1,
    LaterThinning = 2,
}

/// <summary>Work Plan order family priced by the Economy engine.</summary>
internal enum EconomyOrderKind
{
    Fell = 0,
    Prune = 1,
    ClearUndergrowth = 2,
    Plant = 3,
    Shelter = 4,
    SpotClear = 5,
}

/// <summary>Stable canonical key identifying one pending or approved Work Plan order for labour assignment.</summary>
internal readonly record struct EconomyOrderKey(EconomyOrderKind Kind, string AreaId, string TargetKey) : IComparable<EconomyOrderKey>
{
    internal static EconomyOrderKey ForFell(string areaId, string treeId) => new(EconomyOrderKind.Fell, areaId, treeId);
    internal static EconomyOrderKey ForPrune(string areaId, string treeId) => new(EconomyOrderKind.Prune, areaId, treeId);
    internal static EconomyOrderKey ForClear(string areaId, int column, int row) =>
        new(EconomyOrderKind.ClearUndergrowth, areaId, FormattableString.Invariant($"{column},{row}"));
    internal static EconomyOrderKey ForPlanting(MultiAreaPlantingOrder order) => order switch
    {
        MultiAreaPlantOrder => new(EconomyOrderKind.Plant, order.AreaId, order.OrderId),
        MultiAreaShelterOrder => new(EconomyOrderKind.Shelter, order.AreaId, order.OrderId),
        MultiAreaSpotClearOrder => new(EconomyOrderKind.SpotClear, order.AreaId, order.OrderId),
        _ => throw new P1Refusal("EconomyOrderUnknown", "unknown planting order kind"),
    };

    public int CompareTo(EconomyOrderKey other)
    {
        int cmp = Kind.CompareTo(other.Kind);
        if (cmp != 0) return cmp;
        cmp = string.CompareOrdinal(AreaId, other.AreaId);
        return cmp != 0 ? cmp : string.CompareOrdinal(TargetKey, other.TargetKey);
    }
}

/// <summary>Deterministic configuration options for an Economy-enabled session. Defaults match the 0.64 ha calibration stand.</summary>
internal sealed record EconomyOptions
{
    /// <summary>Opening cash balance in integer euro cents. Default €12,000.00 (1,200,000 cents) [GAMEPLAY CALIBRATION].</summary>
    public long OpeningCashCents { get; init; } = EconomyDefaults.DefaultOpeningCashCents;

    /// <summary>Annual landowner labour allowance in integer minutes. Default 2,400 min (40.0 h/year) [GAMEPLAY CALIBRATION].</summary>
    public int AnnualLandownerAllowanceMinutes { get; init; } = EconomyDefaults.DefaultAnnualLandownerAllowanceMinutes;

    /// <summary>Contractor non-harvest hourly rate in integer cents/hr. Default €45.00/hr (4,500 cents) [GAMEPLAY CALIBRATION].</summary>
    public long ContractorHourlyRateCents { get; init; } = EconomyDefaults.ContractorHourlyRateCents;

    /// <summary>Minimum contractor harvest job floor per executed felling Work Plan. Default €2,500.00 (250,000 cents) [GAMEPLAY CALIBRATION].</summary>
    public long MinimumHarvestJobCents { get; init; } = EconomyDefaults.MinimumHarvestJobCents;

    public static readonly EconomyOptions Default = new();

    internal EconomyOptions RequireValid()
    {
        if (OpeningCashCents < 0)
            throw new P1Refusal("EconomyOptionsInvalid", "opening cash balance cannot be negative");
        if (AnnualLandownerAllowanceMinutes < 0)
            throw new P1Refusal("EconomyOptionsInvalid", "annual landowner allowance cannot be negative");
        if (ContractorHourlyRateCents < 0)
            throw new P1Refusal("EconomyOptionsInvalid", "contractor hourly rate cannot be negative");
        if (MinimumHarvestJobCents < 0)
            throw new P1Refusal("EconomyOptionsInvalid", "minimum harvest job floor cannot be negative");
        return this;
    }
}

/// <summary>Locked Phase 2B-1 defaults and evidence labels (all money in integer euro cents, ex VAT).</summary>
internal static class EconomyDefaults
{
    // ---- Locked calibration constants ----
    internal const long DefaultOpeningCashCents = 1_200_000L;               // €12,000.00 [GAMEPLAY CALIBRATION]
    internal const int DefaultAnnualLandownerAllowanceMinutes = 2_400;      // 40.0 h/year [GAMEPLAY CALIBRATION]
    internal const long ContractorHourlyRateCents = 4_500L;                 // €45.00/hr [GAMEPLAY CALIBRATION]
    internal const long MinimumHarvestJobCents = 250_000L;                  // €2,500.00 floor per felling Work Plan [GAMEPLAY CALIBRATION]

    // ---- On-demand materials in Work Plan (no separate nursery inventory) ----
    internal const string ShelterMaterialId = "tree-shelter";
    internal const string BeechSaplingMaterialId = "beech-sapling";
    internal const string OakSaplingMaterialId = "sessile-oak-sapling";

    internal const long ShelterMaterialCents = 500L;                        // €5.00 [GAMEPLAY CALIBRATION]
    internal const long BeechSaplingMaterialCents = 450L;                   // €4.50 [GAMEPLAY CALIBRATION]
    internal const long OakSaplingMaterialCents = 550L;                     // €5.50 [GAMEPLAY CALIBRATION]

    // ---- Grouped harvest contractor rates (selective CCF thinning; combined harvest + forward) ----
    internal const long FirstThinningCentsPerTonne = 2_100L;                // €21/t (3D selected reference within €20–22 IFA range; calibration tier) [EMPIRICAL range / GAMEPLAY CALIBRATION selection]
    internal const long SecondThinningCentsPerTonne = 2_300L;               // €23/t [EMPIRICAL range / GAMEPLAY CALIBRATION selection]
    internal const long LaterThinningCentsPerTonne = 2_000L;                // €20/t [EMPIRICAL range / GAMEPLAY CALIBRATION selection]

    // ---- Low-risk non-harvest task durations (integer minutes) ----
    internal const int PlantMinutesPerSapling = 10;                         // [GAMEPLAY CALIBRATION] ScenarioOneDefinition.cs
    internal const int ShelterReferenceItems = 45;                          // [GAMEPLAY CALIBRATION] Stage1EconomyDefaults.cs (60 min / 45 items)
    internal const int ShelterMinutesPerReference = 60;
    internal const int PruneReferenceItems = 15;                            // [GAMEPLAY CALIBRATION] Stage1EconomyDefaults.cs (60 min / 15 items = 4 min/tree)
    internal const int PruneMinutesPerReference = 60;
    internal const int ClearUndergrowthMinutesPerCell = 12;                 // [GAMEPLAY CALIBRATION] conservative 2.5 m (6.25 m²) cell manual clear
    internal const int SpotClearMinutesPerPatch = 4;                        // [GAMEPLAY CALIBRATION] conservative 1 m² planting spot clear
    internal const float LowRiskPruneMaxHeightM = 6.5f;                     // [MANAGEMENT GUIDANCE / GAMEPLAY CALIBRATION] covers scheduled Sitka (6.5 m) & broadleaf (6.0 m) lifts

    internal const long GramsPerTonne = 1_000_000L;

    internal static long HarvestRateCentsPerTonne(HarvestThinningStage stage) => stage switch
    {
        HarvestThinningStage.FirstThinning => FirstThinningCentsPerTonne,
        HarvestThinningStage.SecondThinning => SecondThinningCentsPerTonne,
        HarvestThinningStage.LaterThinning => LaterThinningCentsPerTonne,
        _ => throw new P1Refusal("EconomyHarvestStageUnknown", "unknown harvest thinning stage"),
    };

    internal static HarvestThinningStage AutoHarvestStage(int executedHarvestPlansCount) => executedHarvestPlansCount switch
    {
        <= 0 => HarvestThinningStage.FirstThinning,
        1 => HarvestThinningStage.SecondThinning,
        _ => HarvestThinningStage.LaterThinning,
    };

    // Scots pine sapling material: the Part 1 [Calibration] EUR 0.45 per tree (ScotsPineEngine.SaplingMaterialCostEurPerTree).
    internal const string ScotsPineSaplingMaterialId = "scots-pine-sapling";
    internal static long ScotsPineSaplingMaterialCents =>
        checked((long)(SitkaP1.Core.SpeciesWave2.ScotsPine.ScotsPineEngine.SaplingMaterialCostEurPerTree * 100m));

    internal static (string MaterialId, long UnitCents) SaplingMaterialFor(string speciesId) => speciesId switch
    {
        Phase2Species.BeechId => (BeechSaplingMaterialId, BeechSaplingMaterialCents),
        Phase2Species.OakId => (OakSaplingMaterialId, OakSaplingMaterialCents),
        ScotsPineLive.SpeciesId => (ScotsPineSaplingMaterialId, ScotsPineSaplingMaterialCents),
        _ => throw new P1Refusal("EconomySaplingMaterialUnknown", "no on-demand sapling price exists for species " + speciesId),
    };

    internal static bool IsLandownerEligible(EconomyOrderKind kind, float pruneTargetHeightM = 0f) => kind switch
    {
        EconomyOrderKind.Fell => false,
        EconomyOrderKind.Plant => true,
        EconomyOrderKind.Shelter => true,
        EconomyOrderKind.ClearUndergrowth => true,
        EconomyOrderKind.SpotClear => true,
        EconomyOrderKind.Prune => pruneTargetHeightM <= LowRiskPruneMaxHeightM,
        _ => false,
    };

    internal static long RoundRatio(long quantity, long rate, long denominator) =>
        checked((long)decimal.Round((decimal)quantity * rate / denominator, 0, MidpointRounding.AwayFromZero));

    internal static long CeilRatio(long quantity, long rate, long denominator) =>
        checked((long)decimal.Ceiling((decimal)quantity * rate / denominator));

    internal static string FormatEuros(long cents)
    {
        bool negative = cents < 0;
        long abs = negative ? checked(-cents) : cents;
        long euros = abs / 100L;
        long rem = abs % 100L;
        return string.Create(CultureInfo.InvariantCulture, $"{(negative ? "-" : string.Empty)}€{euros:N0}.{rem:D2}");
    }
}

/// <summary>Evidence disclosures attached to Economy quotes, views, and specs.</summary>
internal static class EconomyEvidence
{
    internal const string OpeningCash =
        "[GAMEPLAY CALIBRATION] Opening cash €12,000 on the 0.64 ha calibration stand (80×80 m / 5-lane / 1,344 stems) is a gameplay calibration, not a farm-forestry balance sheet.";
    internal const string UpfrontCash =
        "[SIMULATION ABSTRACTION] Upfront cash must cover all external contractor and material costs before approval/execution; expected timber receipts never pre-finance work; no debt or overdraft.";
    internal const string LandownerAllowance =
        "[GAMEPLAY CALIBRATION] Annual 40.0 h (2,400 min) landowner allowance is a physical time resource at €0 cash wage; commercial production felling is contractor-only [MANAGEMENT GUIDANCE].";
    internal const string HarvestPricing =
        "[EMPIRICAL / GAMEPLAY CALIBRATION] Grouped contractor selective CCF thinning harvest by engine-derived stage (first €21/t [3D selected reference within €20–22 range], second €23/t, later €20/t; calibration tiers from a single published IFA range, not an empirical national tariff) with a €2,500 minimum job floor per executed felling Work Plan.";
    internal const string MaterialsAndManualLabour =
        "[GAMEPLAY CALIBRATION] Non-harvest contractor labour €45/hr and on-demand Work Plan materials (shelter €5.00, beech €4.50, oak €5.50) are gameplay calibrations; no separate nursery inventory is modelled.";
    internal const string BroadleafD062 =
        "[MANAGEMENT GUIDANCE / GAMEPLAY CALIBRATION] D-062 Oak and Beech assortment ladders derive eligibility from stem size and recorded pruning history, never a species price multiplier; selected reference prices and roadside basis are GAMEPLAY CALIBRATION, not an Irish broadleaf price series.";
    internal const string VolumeProxy =
        "[EMPIRICAL / GAMEPLAY CALIBRATION] Fresh-volume work-equivalent proxy 890 kg/m³ (Sitka mill-delivery anchor) is used as a unit convention to convert €/m³ to whole €/t and harvest work mass; it is NOT a claimed Oak or Beech green density.";

    internal static readonly IReadOnlyList<string> All = Array.AsReadOnly(new[]
    {
        OpeningCash,
        UpfrontCash,
        LandownerAllowance,
        HarvestPricing,
        MaterialsAndManualLabour,
        BroadleafD062,
        VolumeProxy,
    });
}

/// <summary>Model token lineage helper mirroring <see cref="Phase2Identity"/>: inert when Economy is Off.</summary>
internal static class EconomyIdentity
{
    internal static string AreaModelVersion(string phase2OrEarlier, EconomyModelVersion economy) =>
        economy == EconomyModelVersion.Off
            ? phase2OrEarlier
            : phase2OrEarlier + "+economy-v" + ((int)economy).ToString(CultureInfo.InvariantCulture);
}
