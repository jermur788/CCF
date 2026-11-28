using System.Text.Json.Serialization;

namespace SitkaP1.Core;

// MULTI Map Integration Pass 1 (spec multi-map-integration-pass1-v1):
// Strictly additive, read-only Core DTOs, species-aware prune readout helper, atomic composite snapshot records,
// read-only cell detail, and the demo-only MA-1-MIXED factory. Zero biology, growth, mortality, crown, regeneration
// or RNG changes; zero references to test assemblies; session-only (nothing is saved).

/// <summary>Model selection tuple exposed in the additive <c>ecology</c> DTO.</summary>
internal sealed record MultiAreaEcologyModels(int Adult, int Mortality, int Crown, int Regeneration, int Species);

/// <summary>
/// Engine-authoritative species-aware pruning readout for one tree.
/// Sitka: max 3 lifts at 2.5 / 5.0 / 6.5 m, 5-year wait.
/// Oak / Beech: max 2 lifts at 3.5 / 6.0 m, 3-year wait.
/// </summary>
internal sealed record MultiAreaSpeciesPruneStatus(
    string SpeciesId,
    bool Eligible,
    int Lifts,
    int MaxLifts,
    int MinIntervalYears,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] float? NextTargetM,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] int? NextEligibleYear,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? RefusalCode,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? RefusalReason,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? PlanState);

/// <summary>Per-area cell counts summarised for the additive <c>ecology</c> DTO.</summary>
internal sealed record MultiAreaEcologyCellCounts(int Total, int WithNatural, int WithBramble, int WithBracken);

/// <summary>Per-rack species summary in the additive <c>ecology</c> DTO.</summary>
internal sealed record MultiAreaEcologyAreaSummary(
    string AreaId,
    IReadOnlyDictionary<string, int> LiveAdultsBySpecies,
    IReadOnlyDictionary<string, double> BasalAreaM2BySpecies,
    IReadOnlyDictionary<string, int> DeadwoodBySpecies,
    IReadOnlyDictionary<string, int> PlantedAliveBySpecies,
    MultiAreaEcologyCellCounts CellCounts);

/// <summary>Read-only planted juvenile record with both engine and map coordinates.</summary>
internal sealed record MultiAreaEcologyPlanted(
    string JuvenileId,
    string AreaId,
    string SpeciesId,
    float XM,
    float ZM,
    double U,
    double V,
    int PlantingYear,
    float AgeYears,
    float HeightM,
    bool Alive,
    bool IsActive,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? PromotedTreeId,
    int DeathYear,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? DeathCause,
    bool LastYearBrowsed,
    double LastBrowseProbability,
    double LastVegetationLossProbability,
    int LastAssessmentYear);

/// <summary>Read-only tree shelter record with engine-derived <see cref="ExpiryYear"/> and <see cref="IsEffective"/>.</summary>
internal sealed record MultiAreaEcologyShelter(
    string ShelterId,
    string JuvenileId,
    string AreaId,
    float XM,
    float ZM,
    double U,
    double V,
    int InstalledYear,
    int EffectiveYears,
    int ExpiryYear,
    int FailedYear,
    bool IsEffective);

/// <summary>Read-only circular spot-clear patch (~1 m², <c>radiusM ≈ 0.564</c>) with engine and map coordinates.</summary>
internal sealed record MultiAreaEcologyPatch(
    string PatchId,
    string AreaId,
    float XM,
    float ZM,
    double U,
    double V,
    float RadiusM,
    int CreatedYear,
    int UpdatedYear,
    double Bramble,
    double Bracken);

/// <summary>Flat read-only projection of a fenced Phase 2 planting order for display in the Work Plan.</summary>
internal sealed record MultiAreaEcologyPlantingOrder(
    string OrderId,
    string Kind,
    string AreaId,
    float XM,
    float ZM,
    double U,
    double V,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? SpeciesId);

/// <summary>
/// Compact additive camelCase <c>ecology</c> DTO attached alongside <c>view</c>/<c>scenes</c>/<c>stage4</c> whenever
/// <c>SpeciesModelVersion != 0</c> (null when <c>SpeciesModelVersion == 0</c>; never fabricates zeros).
/// </summary>
internal sealed record MultiAreaEcologyDto(
    string SessionId,
    int PlanStateRevision,
    MultiAreaSourceToken Source,
    MultiAreaEcologyModels Models,
    string ExperimentId,
    string ExperimentBadge,
    double GridCellM,
    IReadOnlyDictionary<string, string> SpeciesByTreeId,
    IReadOnlyDictionary<string, MultiAreaSpeciesPruneStatus> PruneStatusByTreeId,
    IReadOnlyList<MultiAreaEcologyAreaSummary> Areas,
    IReadOnlyList<string> CellSchema,
    IReadOnlyList<double[]> CellsCompact,
    IReadOnlyList<MultiAreaEcologyPlanted> Planted,
    IReadOnlyList<MultiAreaEcologyShelter> Shelters,
    IReadOnlyList<MultiAreaEcologyPatch> Patches,
    IReadOnlyList<MultiAreaClearUndergrowthOrder> PendingClear,
    IReadOnlyList<MultiAreaClearUndergrowthOrder> ApprovedClear,
    IReadOnlyList<MultiAreaEcologyPlantingOrder> PendingPlanting,
    IReadOnlyList<MultiAreaEcologyPlantingOrder> ApprovedPlanting,
    // Scots pine & site moisture (Part 2): present on Forest Maker woodlands only; absent everywhere else (unchanged JSON).
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] MultiAreaScotsPineEcology? ScotsPine = null);

/// <summary>One natural regeneration cohort band in a cell detail response.</summary>
internal sealed record MultiAreaCellCohort(
    string Species,
    string Origin,
    int EstablishYear,
    int OriginYear,
    float HeightM,
    float Density,
    float LastBrowsedFraction,
    int LastBrowseAssessmentYear);

/// <summary>Full read-only cell detail returned by <c>POST api/multi/cell</c>.</summary>
internal sealed record MultiAreaCellDetail(
    string AreaId,
    int Column,
    int Row,
    float CentreXM,
    float CentreZM,
    double CentreU,
    double CentreV,
    double ClippedAreaM2,
    IReadOnlyList<MultiAreaGroundVertex> Polygon,
    float LightIndex,
    double GeometricCoveredFraction,
    int CrossBorderTrees,
    IReadOnlyList<string> ContributingTreeIds,
    float? Bramble,
    float? Bracken,
    float? BrowsePressure,
    IReadOnlyList<MultiAreaCellCohort> Cohorts,
    string ClearState,
    bool HasPendingClear,
    bool HasApprovedClear);

/// <summary>
/// Paired Stage 4 + Ecology UI readouts taken from ONE committed root under ONE engine lock.
/// Returns null as a whole if the session moved concurrently, so a view is never paired with a different root.
/// </summary>
internal sealed record MultiAreaCompositeUiReadout(MultiAreaUiReadout Stage4, MultiAreaEcologyDto? Ecology);

/// <summary>
/// Atomic composite snapshot of ONE committed root under ONE engine lock: <see cref="View"/>, <see cref="Stage4"/>,
/// <see cref="Stage5"/>, <see cref="Phase2"/>, the UI <see cref="Readout"/> and compact <see cref="Ecology"/> DTO
/// all share one exact <c>SessionId</c>, <c>Source</c> and <c>PlanStateRevision</c>.
/// </summary>
internal sealed record MultiAreaCompositeUiSnapshot(
    MultiAreaSessionView View,
    MultiAreaUiReadout Readout,
    MultiAreaEcologyDto? Ecology,
    MultiAreaStage4View Stage4,
    MultiAreaStage5View? Stage5,
    MultiAreaPhase2View? Phase2);

/// <summary>Read-only cell detail outcome, carrying the fresh composite snapshot from the same lock acquisition.</summary>
internal sealed record MultiAreaCellDetailResult(
    MultiAreaCompositeUiSnapshot Snapshot,
    MultiAreaCellDetail? Cell,
    string? RefusalCode,
    string? RefusalMessage);

/// <summary>
/// Species-aware prune readout helper shared by <see cref="MultiAreaUiReadouts"/> and <see cref="MultiAreaEcologyBuilder"/>.
/// Delegates directly to <see cref="Phase2Pruning"/> and <see cref="PruningRules"/> so UI readouts match Execute guards.
/// </summary>
internal static class MultiAreaSpeciesPruneHelper
{
    internal static MultiAreaSpeciesPruneStatus Evaluate(AnnualTree tree, TreePruningState state, int ecologicalYear,
        Phase2Species? broadleaf, bool isStage4, string? planState, bool scotsPine = false)
    {
        // Scots pine (Part 2): two lifts, 2.5 m then 5.5 m, five years apart (ScotsPinePruning); otherwise unchanged.
        string speciesId = scotsPine ? ScotsPineLive.SpeciesId : broadleaf?.SpeciesId ?? Phase2Species.SitkaId;
        int maxLifts = scotsPine ? ScotsPineLive.MaxLifts : broadleaf is null ? PruningRules.MaxLifts : Phase2Constants.BroadleafMaxLifts;
        int minInterval = scotsPine || broadleaf is null ? PruningRules.MinimumYearsBetweenLifts : Phase2Constants.BroadleafMinimumYearsBetweenLifts;
        if (!isStage4)
        {
            return new MultiAreaSpeciesPruneStatus(speciesId, false, state.Lifts, maxLifts, minInterval,
                null, null, "Stage4ModelRequired", MultiAreaUiReadouts.Stage4ModelRequiredReason, planState);
        }

        float next = scotsPine ? ScotsPinePruning.NextLiftTargetM(state.Lifts) : Phase2Pruning.NextLiftTargetM(state.Lifts, broadleaf);
        string? code = scotsPine ? ScotsPinePruning.RefusalCode(tree, state, next, ecologicalYear)
            : Phase2Pruning.RefusalCode(tree, state, next, ecologicalYear, broadleaf);
        string? reason = code is null ? null : scotsPine ? ScotsPinePruning.Message(code) : Phase2Pruning.Message(code, broadleaf);
        float? nextTarget = tree.IsLiving && float.IsFinite(next) ? next : null;
        int? nextEligibleYear = code == "PruneIntervalTooShort" ? state.LastPruningYear + minInterval : null;
        return new MultiAreaSpeciesPruneStatus(speciesId, code is null, state.Lifts, maxLifts, minInterval,
            nextTarget, nextEligibleYear, code, reason, planState);
    }
}

/// <summary>
/// Demo-only experiment factory for <c>/multi</c> (<c>--multi-experiment=ma-1|ma-1-mixed</c>).
/// Lives in <c>src/Core/MultiArea</c> and never references test assemblies. Does not touch <see cref="MultiAreaGenerator"/>.
/// </summary>
internal static class MultiAreaDemoExperiments
{
    internal const string Ma1Key = "ma-1";
    internal const string Ma1MixedKey = "ma-1-mixed";
    internal const string Ma1MixedExperimentId = "mad-ma1-mixed-demo-v1";

    internal const string Ma1Badge = "Synthetic MA-1 (all Sitka)";
    internal const string Ma1MixedBadge =
        "Synthetic MA-1-MIXED demo \u2014 Area A Sitka; Area B \u2153 Sitka / \u2153 Oak / \u2153 Beech (not a surveyed Irish forecast)";

    internal const string Ma1MixedDisclosure =
        "SYNTHETIC MA-1-MIXED DEMO - AREA A SITKA; AREA B 1/3 SITKA / 1/3 OAK / 1/3 BEECH (NOT A SURVEYED IRISH FORECAST). " +
        "AREAS NOT ASSESSED COMPARABLE. SITE UNKNOWN. CONNECTED WOODLAND: GROWTH, CROWNS AND SEED RAIN USE BOTH AREAS; " +
        "DENSITY MORTALITY, WORK PLANS AND SUMMARIES STAY PER AREA. NO TREATMENT TRANSFER BETWEEN AREAS. " +
        "NOT UNITY NUMERICAL OR GEOMETRY PARITY.";

    /// <summary>
    /// Deterministic MA-1-MIXED demo fixture (spec §5.2):
    /// Area A stays 100 % sitka-spruce; Area B's adult TreeIds sorted with <see cref="StringComparer.Ordinal"/> are assigned
    /// round-robin: <c>i % 3 == 0</c> sitka-spruce, <c>1</c> sessile-oak (age 60), <c>2</c> beech (age 60) via
    /// <see cref="Phase2Options.GenesisSpecies"/>.
    /// </summary>
    internal static (MultiAreaExperiment Experiment, Phase2Options Options, string Badge) CreateMixedDemo()
    {
        var ma1 = MultiAreaGenerator.Generate();
        var species = new Dictionary<string, string>(StringComparer.Ordinal);
        var areas = new MultiAreaDefinition[ma1.Areas.Count];
        for (int a = 0; a < ma1.Areas.Count; a++)
        {
            var area = ma1.Areas[a];
            if (!string.Equals(area.AreaId, "MA-B", StringComparison.Ordinal))
            {
                areas[a] = area;
                continue;
            }
            var sortedTrees = area.Trees.OrderBy(t => t.TreeId, StringComparer.Ordinal).ToArray();
            var reassigned = new MultiAreaTreeDefinition[sortedTrees.Length];
            for (int i = 0; i < sortedTrees.Length; i++)
            {
                var tree = sortedTrees[i];
                switch (i % 3)
                {
                    case 0:
                        reassigned[i] = tree; // sitka-spruce: retains MA-1 age
                        break;
                    case 1:
                        species[tree.TreeId] = Phase2Species.OakId;
                        reassigned[i] = tree with { AgeYears = 60 };
                        break;
                    default:
                        species[tree.TreeId] = Phase2Species.BeechId;
                        reassigned[i] = tree with { AgeYears = 60 };
                        break;
                }
            }
            areas[a] = area with
            {
                StockingNote = "synthetic mixed demo: Area B round-robin 1/3 Sitka, 1/3 Sessile Oak (age 60), 1/3 Beech (age 60)",
                Trees = Array.AsReadOnly(reassigned),
            };
        }
        var experiment = new MultiAreaExperiment(Ma1MixedExperimentId, ma1.GeneratorId, ma1.GeneratorVersion, ma1.GeneratorSeed,
            Array.AsReadOnly(areas))
        {
            DisclosureOverride = Ma1MixedDisclosure,
        };
        experiment.Validate();
        return (experiment, Phase2Options.Default with { GenesisSpecies = species }, Ma1MixedBadge);
    }

    internal static string BadgeFor(string experimentKey) => experimentKey switch
    {
        Ma1Key => Ma1Badge,
        Ma1MixedKey => Ma1MixedBadge,
        _ => throw new P1Refusal("MultiAreaUnknownExperiment",
            "supported --multi-experiment values are ma-1 and ma-1-mixed"),
    };
}

/// <summary>Builds the compact additive <see cref="MultiAreaEcologyDto"/> from one committed root. Pure; keeps no state.</summary>
internal static class MultiAreaEcologyBuilder
{
    internal static readonly IReadOnlyList<string> CellSchema = Array.AsReadOnly(new[]
    {
        "areaIndex", "col", "row", "light", "bramble", "bracken", "speciesMask", "bandCount",
    });

    private static readonly string[] AllSpecies = { Phase2Species.SitkaId, Phase2Species.OakId, Phase2Species.BeechId };

    /// <summary>
    /// Rounds a non-negative fraction/index to 4 decimal places for compact JSON transport while guaranteeing that a
    /// strictly positive non-zero value is never rounded down to <c>0</c>.
    /// </summary>
    internal static double CompactPositive(double value)
    {
        if (value <= 0.0 || !double.IsFinite(value)) return 0.0;
        double rounded = Math.Round(value, 4, MidpointRounding.AwayFromZero);
        return rounded > 0.0 ? rounded : 0.0001;
    }

    internal static MultiAreaEcologyDto? Build(
        MultiAreaSessionView view,
        MultiAreaStage4View stage4,
        MultiAreaStage5State? stage5,
        MultiAreaPhase2State? phase2,
        CanopyLightReport canopy,
        MultiAreaModelSelection models,
        string experimentBadge)
    {
        // Spec §5.4: ecology is null whenever SpeciesModelVersion == 0; never fabricate zeros.
        if (models.Species == SpeciesModelVersion.Off || stage5 is null || phase2 is null) return null;

        var areaExtentById = view.Areas.ToDictionary(a => a.AreaId, a => a.Extent, StringComparer.Ordinal);

        // Sparse species map covering living, standing-dead, fallen-dead and promoted adults; missing = sitka-spruce.
        var speciesByTreeId = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var (treeId, sp) in phase2.TreeSpecies.OrderBy(kv => kv.Key, StringComparer.Ordinal))
        {
            if (!string.Equals(sp, Phase2Species.SitkaId, StringComparison.Ordinal))
                speciesByTreeId[treeId] = sp;
        }
        foreach (var s4Area in stage4.Areas)
        {
            foreach (var dw in s4Area.Deadwood)
            {
                if (!string.Equals(dw.SpeciesId, Phase2Species.SitkaId, StringComparison.Ordinal))
                    speciesByTreeId[dw.TreeId] = dw.SpeciesId;
            }
        }

        // Species-aware prune status for every tree in every area.
        var pendingPruneIds = stage4.PendingPrunes.Select(o => o.TreeId).ToHashSet(StringComparer.Ordinal);
        var approvedPruneIds = stage4.ApprovedPrunes.Select(o => o.TreeId).ToHashSet(StringComparer.Ordinal);
        var pruneStatusByTreeId = new Dictionary<string, MultiAreaSpeciesPruneStatus>(StringComparer.Ordinal);
        for (int i = 0; i < view.Areas.Count; i++)
        {
            var area = view.Areas[i];
            var s4Area = stage4.Areas[i];
            for (int t = 0; t < area.Trees.Count; t++)
            {
                var tree = area.Trees[t];
                var s4Tree = s4Area.Trees[t];
                string? planState = pendingPruneIds.Contains(tree.TreeId) ? "PendingPrune"
                    : approvedPruneIds.Contains(tree.TreeId) ? "ApprovedPrune" : null;
                var pruneState = new TreePruningState(s4Tree.PruningLifts, s4Tree.CrownBaseHeightM, s4Tree.LastPruningYear);
                pruneStatusByTreeId[tree.TreeId] = MultiAreaSpeciesPruneHelper.Evaluate(
                    tree, pruneState, stage4.EcologicalYear, phase2.BroadleafOf(tree.TreeId), models.IsStage4, planState,
                    ScotsPineLive.IsPine(phase2.SpeciesIdOf(tree.TreeId)));
            }
        }

        // Per-area species summaries + compact numeric cell rows.
        var areaSummaries = new MultiAreaEcologyAreaSummary[view.Areas.Count];
        var cellsCompact = new List<double[]>();
        for (int i = 0; i < view.Areas.Count; i++)
        {
            var area = view.Areas[i];
            var s4Area = stage4.Areas[i];
            var s5Area = stage5.Areas[i];
            var p2Area = phase2.Areas[i];
            var canopyArea = canopy.Areas[i];

            var liveBySpecies = ZeroIntMap();
            var baBySpecies = ZeroDoubleMap();
            foreach (var tree in area.Trees)
            {
                if (!tree.IsLiving) continue;
                string sp = phase2.SpeciesIdOf(tree.TreeId);
                liveBySpecies[sp] = liveBySpecies.GetValueOrDefault(sp) + 1;
                double radiusM = tree.DbhCm / 200.0;
                baBySpecies[sp] = baBySpecies.GetValueOrDefault(sp) + Math.PI * radiusM * radiusM;
            }
            foreach (var sp in baBySpecies.Keys.ToArray()) baBySpecies[sp] = CompactPositive(baBySpecies[sp]);

            var deadwoodBySpecies = ZeroIntMap();
            foreach (var dw in s4Area.Deadwood)
                deadwoodBySpecies[dw.SpeciesId] = deadwoodBySpecies.GetValueOrDefault(dw.SpeciesId) + 1;

            var plantedAliveBySpecies = ZeroIntMap();
            foreach (var pj in phase2.Juveniles)
            {
                if (pj.IsActive && string.Equals(pj.AreaId, area.AreaId, StringComparison.Ordinal))
                    plantedAliveBySpecies[pj.SpeciesId] = plantedAliveBySpecies.GetValueOrDefault(pj.SpeciesId) + 1;
            }

            int withNatural = 0, withBramble = 0, withBracken = 0;
            var s5Cells = s5Area.Cells;
            var p2Cells = p2Area.Cells;
            var lightCells = canopyArea.Light.Cells;
            for (int c = 0; c < s5Cells.Count; c++)
            {
                var s5Cell = s5Cells[c];
                var p2Cell = p2Cells[c];
                float currentLight = lightCells[c].LightIndex;

                int mask = 0;
                int bandCount = 0;
                foreach (var b in s5Cell.Bands)
                {
                    if (b.Density > 0f) { mask |= 1; bandCount++; }
                }
                foreach (var b in p2Cell.Bands)
                {
                    if (b.Density <= 0f) continue;
                    bandCount++;
                    if (string.Equals(b.SpeciesId, Phase2Species.OakId, StringComparison.Ordinal)) mask |= 2;
                    else if (string.Equals(b.SpeciesId, Phase2Species.BeechId, StringComparison.Ordinal)) mask |= 4;
                }

                if (bandCount > 0) withNatural++;
                if (s5Cell.Bramble > 0f) withBramble++;
                if (s5Cell.Bracken > 0f) withBracken++;

                cellsCompact.Add(new[]
                {
                    (double)i,
                    (double)s5Cell.Column,
                    (double)s5Cell.Row,
                    CompactPositive(currentLight),
                    CompactPositive(s5Cell.Bramble),
                    CompactPositive(s5Cell.Bracken),
                    (double)mask,
                    (double)bandCount,
                });
            }

            areaSummaries[i] = new MultiAreaEcologyAreaSummary(
                area.AreaId,
                liveBySpecies,
                baBySpecies,
                deadwoodBySpecies,
                plantedAliveBySpecies,
                new MultiAreaEcologyCellCounts(s5Cells.Count, withNatural, withBramble, withBracken));
        }

        // Exact planted juveniles, shelters (with engine-derived expiryYear & isEffective), spot patches, read-only queues.
        var juvenileAreaById = phase2.Juveniles.ToDictionary(j => j.JuvenileId, j => j.AreaId, StringComparer.Ordinal);
        var planted = phase2.Juveniles.Select(j =>
        {
            var extent = areaExtentById[j.AreaId];
            return new MultiAreaEcologyPlanted(
                j.JuvenileId, j.AreaId, j.SpeciesId, j.XM, j.ZM,
                MultiAreaUiReadouts.MapU(extent, j.XM), MultiAreaUiReadouts.MapV(extent, j.ZM),
                j.PlantingYear, j.AgeYears, j.HeightM, j.Alive, j.IsActive, j.PromotedTreeId,
                j.DeathYear, j.DeathCause, j.LastYearBrowsed,
                CompactPositive(j.LastBrowseProbability), CompactPositive(j.LastVegetationLossProbability), j.LastAssessmentYear);
        }).ToArray();

        int year = view.Source.EcologicalYear;
        var shelters = phase2.Shelters.Select(s =>
        {
            string areaId = juvenileAreaById.TryGetValue(s.JuvenileId, out var aid) ? aid : view.Areas[0].AreaId;
            var extent = areaExtentById[areaId];
            int expiryYear = s.InstalledYear + Math.Max(0, s.EffectiveYears);
            bool isEffective = s.IsEffective(Math.Max(year, s.InstalledYear));
            return new MultiAreaEcologyShelter(
                s.ShelterId, s.JuvenileId, areaId, s.XM, s.ZM,
                MultiAreaUiReadouts.MapU(extent, s.XM), MultiAreaUiReadouts.MapV(extent, s.ZM),
                s.InstalledYear, s.EffectiveYears, expiryYear, s.FailedYear, isEffective);
        }).ToArray();

        var patches = phase2.Patches.Select(p =>
        {
            var extent = areaExtentById[p.AreaId];
            return new MultiAreaEcologyPatch(
                p.PatchId, p.AreaId, p.XM, p.ZM,
                MultiAreaUiReadouts.MapU(extent, p.XM), MultiAreaUiReadouts.MapV(extent, p.ZM),
                p.RadiusM, p.CreatedYear, p.UpdatedYear, CompactPositive(p.Bramble), CompactPositive(p.Bracken));
        }).ToArray();

        var pendingPlanting = phase2.PendingOrders.Select(o => ProjectPlantingOrder(o, areaExtentById[o.AreaId])).ToArray();
        var approvedPlanting = phase2.ApprovedOrders.Select(o => ProjectPlantingOrder(o, areaExtentById[o.AreaId])).ToArray();

        var dtoModels = new MultiAreaEcologyModels(
            (int)models.Adult, (int)models.Mortality, (int)models.Crown, (int)models.Regeneration, (int)models.Species);

        return new MultiAreaEcologyDto(
            view.SessionId,
            view.PlanStateRevision,
            view.Source,
            dtoModels,
            view.Experiment.ExperimentId,
            experimentBadge,
            stage5.Options.GridCellM,
            speciesByTreeId,
            pruneStatusByTreeId,
            Array.AsReadOnly(areaSummaries),
            CellSchema,
            cellsCompact.AsReadOnly(),
            Array.AsReadOnly(planted),
            Array.AsReadOnly(shelters),
            Array.AsReadOnly(patches),
            stage5.PendingClears,
            stage5.ApprovedClears,
            Array.AsReadOnly(pendingPlanting),
            Array.AsReadOnly(approvedPlanting));
    }

    private static MultiAreaEcologyPlantingOrder ProjectPlantingOrder(MultiAreaPlantingOrder order, PlayerMapExtent extent)
    {
        var (kind, speciesId) = order switch
        {
            MultiAreaPlantOrder p => ("Plant", (string?)p.SpeciesId),
            MultiAreaShelterOrder => ("Shelter", null),
            MultiAreaSpotClearOrder => ("SpotClear", null),
            _ => (order.GetType().Name, null),
        };
        return new MultiAreaEcologyPlantingOrder(
            order.OrderId, kind, order.AreaId, order.XM, order.ZM,
            MultiAreaUiReadouts.MapU(extent, order.XM), MultiAreaUiReadouts.MapV(extent, order.ZM),
            speciesId);
    }

    private static Dictionary<string, int> ZeroIntMap() => new(StringComparer.Ordinal)
    {
        [Phase2Species.SitkaId] = 0,
        [Phase2Species.OakId] = 0,
        [Phase2Species.BeechId] = 0,
    };

    private static Dictionary<string, double> ZeroDoubleMap() => new(StringComparer.Ordinal)
    {
        [Phase2Species.SitkaId] = 0.0,
        [Phase2Species.OakId] = 0.0,
        [Phase2Species.BeechId] = 0.0,
    };
}
