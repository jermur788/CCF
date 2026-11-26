namespace SitkaP1.Core;

// CCF Multi 2D — Phase 1 Step 1 (spec multi-2d-phase1-time-history-core-v1 §5):
// Pure read-only Decision -> Outcome history query over one immutable committed MultiAreaRoot.
// Zero mutation, zero ecological re-simulation, zero RNG, zero economy/euro/weather fields.

/// <summary>
/// Six-value provenance vocabulary aligned with D-073 / S1-C tiers.
/// </summary>
internal enum HistoryProvenance
{
    /// <summary>Committed at the time the work event or annual transition was recorded (D-073 L1).</summary>
    Recorded,
    /// <summary>Live committed state of the current root (D-073 L2).</summary>
    Current,
    /// <summary>Computed deterministically at query time from recorded and/or current state (D-073 L3).</summary>
    Derived,
    /// <summary>Assembled by cross-referencing related committed records rather than a dedicated log field (D-073 L3).</summary>
    Reconstructed,
    /// <summary>Ephemeral session read-out or cache not stored as a durable per-year historical archive.</summary>
    SessionOnly,
    /// <summary>Absent from recorded 2D history; explicitly disclosed and never fabricated from current state.</summary>
    NotRecorded,
}

/// <summary>One provenance vocabulary entry with its canonical wire label and D-073 tier mapping.</summary>
internal sealed record HistoryProvenanceDescriptor(
    HistoryProvenance Provenance,
    string Label,
    string D073Tier,
    string Description);

/// <summary>Canonical wire labels and D-073 tier mappings for the six <see cref="HistoryProvenance"/> values.</summary>
internal static class HistoryProvenanceCatalog
{
    internal static string LabelOf(HistoryProvenance provenance) => provenance switch
    {
        HistoryProvenance.Recorded => "RECORDED",
        HistoryProvenance.Current => "CURRENT",
        HistoryProvenance.Derived => "DERIVED",
        HistoryProvenance.Reconstructed => "RECONSTRUCTED",
        HistoryProvenance.SessionOnly => "SESSION-ONLY",
        HistoryProvenance.NotRecorded => "NOT RECORDED",
        _ => throw new ArgumentOutOfRangeException(nameof(provenance), provenance, "Unknown provenance value."),
    };

    internal static string D073TierOf(HistoryProvenance provenance) => provenance switch
    {
        HistoryProvenance.Recorded => "L1 Recorded historical fact",
        HistoryProvenance.Current => "L2 Current observed state",
        HistoryProvenance.Derived => "L3 Derived comparison",
        HistoryProvenance.Reconstructed => "L3 Reconstructed from related committed records",
        HistoryProvenance.SessionOnly => "L2 Session-only (not persisted as a historical archive)",
        HistoryProvenance.NotRecorded => "Absent (NOT RECORDED — never fabricated)",
        _ => throw new ArgumentOutOfRangeException(nameof(provenance), provenance, "Unknown provenance value."),
    };

    internal static readonly IReadOnlyList<HistoryProvenanceDescriptor> All = Array.AsReadOnly(new[]
    {
        new HistoryProvenanceDescriptor(HistoryProvenance.Recorded, LabelOf(HistoryProvenance.Recorded),
            D073TierOf(HistoryProvenance.Recorded),
            "Committed at the time work executed or an annual step closed."),
        new HistoryProvenanceDescriptor(HistoryProvenance.Current, LabelOf(HistoryProvenance.Current),
            D073TierOf(HistoryProvenance.Current),
            "Read directly from the current committed root."),
        new HistoryProvenanceDescriptor(HistoryProvenance.Derived, LabelOf(HistoryProvenance.Derived),
            D073TierOf(HistoryProvenance.Derived),
            "Computed deterministically from recorded and/or current state at query time."),
        new HistoryProvenanceDescriptor(HistoryProvenance.Reconstructed, LabelOf(HistoryProvenance.Reconstructed),
            D073TierOf(HistoryProvenance.Reconstructed),
            "Cross-referenced from related committed records (for example tracing planted juveniles by ID)."),
        new HistoryProvenanceDescriptor(HistoryProvenance.SessionOnly, LabelOf(HistoryProvenance.SessionOnly),
            D073TierOf(HistoryProvenance.SessionOnly),
            "Session-scoped read-out cache (for example current canopy/light) not recorded per past year."),
        new HistoryProvenanceDescriptor(HistoryProvenance.NotRecorded, LabelOf(HistoryProvenance.NotRecorded),
            D073TierOf(HistoryProvenance.NotRecorded),
            "Not stored by the 2D engine; disclosed honestly and never back-filled from current state."),
    });
}

/// <summary>Truthful fixed text for Phase 1 Step 1 history queries.</summary>
internal static class MultiAreaHistoryLabels
{
    internal const string NonCausalComparisonStatement =
        "Observed changes reflect ongoing natural growth, competition, seeding, and light alongside recorded " +
        "management — not a single-cause effect.";

    internal const string PastOverheadCanopyNotRecorded =
        "Full past-year overhead canopy cover is NOT RECORDED; current canopy/light is a session-only derived " +
        "read-out and is never back-filled into past years.";

    internal const string PastTreeStructureNotRecorded =
        "Past tree-by-tree diameter, height, crown, and spatial structure snapshots are NOT RECORDED; spatial " +
        "structure descriptors apply to the current committed stand only.";

    internal const string PerAreaPlantedSplitNotRecorded =
        "Phase2PlantedAccount records species-only whole-woodland totals; per-area annual planted-juvenile splits " +
        "are NOT RECORDED and are never fabricated.";
}

/// <summary>Counts of executed work actions across the six 2D action types.</summary>
internal sealed record ExecutedActionCounts(
    int Fell,
    int Prune,
    int ClearUndergrowth,
    int Plant,
    int Shelter,
    int SpotClear)
{
    internal static readonly ExecutedActionCounts Zero = new(0, 0, 0, 0, 0, 0);

    internal int Total => Fell + Prune + ClearUndergrowth + Plant + Shelter + SpotClear;

    internal bool HasAny => Total > 0;
}

/// <summary>One row in a newest-first <see cref="HistoryIndexPage"/>.</summary>
internal sealed record HistoryIndexEntry(
    int Year,
    bool HasExecutedWork,
    ExecutedActionCounts ExecutedActionCounts,
    bool HasAnnualOutcome,
    int? LiveStemsEndOfYear,
    bool HasObservedOutcomeNextYear);

/// <summary>Bounded, newest-first, cursor-stable page of recorded ecological years.</summary>
internal sealed record HistoryIndexPage(
    IReadOnlyList<HistoryIndexEntry> Items,
    int PageSize,
    int? BeforeYear,
    int? NextCursorBeforeYear,
    int TotalRecordedYears);

/// <summary>Exact ground cell lattice coordinate recorded on a ClearUndergrowth log entry.</summary>
internal sealed record HistoryCellCoord(int Column, int Row);

/// <summary>One recorded Fell from <see cref="MultiAreaFellRecord"/> in WorkYear = Y.</summary>
internal sealed record HistoryFellDetail(
    string TreeId,
    string Reason,
    string Cause,
    string Mechanism,
    int WorkRevision,
    string PlanId,
    string ExecutionKey);

/// <summary>One recorded Prune lift from <see cref="MultiAreaPruneRecord"/> in WorkYear = Y.</summary>
internal sealed record HistoryPruneDetail(
    string TreeId,
    float TargetCrownBaseHeightM,
    float CrownRadiusBeforeM,
    float CrownRadiusAfterM,
    int LiftsAfter,
    int WorkRevision,
    string PlanId,
    string ExecutionKey);

/// <summary>One recorded ClearUndergrowth from <see cref="MultiAreaClearUndergrowthRecord"/> in WorkYear = Y.</summary>
internal sealed record HistoryClearDetail(
    int Column,
    int Row,
    float BrambleBefore,
    float BrackenBefore,
    int BandsKept,
    double AbundanceKept,
    int WorkRevision,
    string PlanId,
    string ExecutionKey);

/// <summary>One recorded Phase 2 planting action (Plant, Shelter, or SpotClear) from <see cref="Phase2WorkRecord"/> in WorkYear = Y.</summary>
internal sealed record HistoryPlantingDetail(
    string OrderId,
    string Kind,
    float XM,
    float ZM,
    string? SpeciesId,
    string? JuvenileId,
    string? ShelterId,
    string? PatchId,
    int WorkRevision,
    string PlanId,
    string ExecutionKey);

/// <summary>Recorded executed work grouped by action type within one area and one WorkYear.</summary>
internal sealed record HistoryActionGroup(
    string ActionType,
    int WorkYear,
    HistoryProvenance Provenance,
    int Count,
    IReadOnlyList<string> TreeIds,
    IReadOnlyList<HistoryCellCoord> Cells,
    IReadOnlyList<string> JuvenileIds,
    IReadOnlyList<string> ShelterIds,
    IReadOnlyList<string> PatchIds,
    IReadOnlyList<string> OrderIds);

/// <summary>All executed work recorded in WorkYear = Y for one management area.</summary>
internal sealed record HistoryAreaWorkRecord(
    string AreaId,
    int WorkYear,
    HistoryProvenance Provenance,
    ExecutedActionCounts Counts,
    IReadOnlyList<HistoryActionGroup> ActionGroups,
    IReadOnlyList<HistoryFellDetail> Fells,
    IReadOnlyList<HistoryPruneDetail> Prunes,
    IReadOnlyList<HistoryClearDetail> Clears,
    IReadOnlyList<HistoryPlantingDetail> PlantingWork);

/// <summary>Per-species natural regeneration summary from one recorded Stage 5 / Phase 2 annual area account.</summary>
internal sealed record HistoryAreaNaturalRegenOutcome(
    string SpeciesId,
    double StartingAbundance,
    double EndingAbundance,
    double EstablishmentAccepted,
    int PromotionsCount,
    IReadOnlyList<string> PromotedTreeIds);

/// <summary>One area's recorded annual ecological outcome for a single annual transition.</summary>
internal sealed record HistoryAreaAnnualOutcome(
    string AreaId,
    AnnualStand Before,
    AnnualStand After,
    int NaturalDeathCount,
    IReadOnlyList<string> NaturalDeathTreeIds,
    bool NoLivingStems,
    double? MeanBramble,
    double? MeanBracken,
    double? MeanRegenStepLightIndex,
    IReadOnlyList<HistoryAreaNaturalRegenOutcome> NaturalRegeneration);

/// <summary>
/// Species-only whole-woodland planted-juvenile annual account from <see cref="Phase2PlantedAccount"/>.
/// Exposed honestly at whole-woodland scope; per-area splits are NOT RECORDED.
/// </summary>
internal sealed record HistoryPlantedSpeciesAnnualAccount(
    string SpeciesId,
    string Scope,
    HistoryProvenance Provenance,
    int Starting,
    int VegetationDeaths,
    int LightDeaths,
    int BrowseDeaths,
    int Remaining,
    int Promoted,
    IReadOnlyList<string> PromotedTreeIds);

/// <summary>One recorded annual transition (<see cref="YearBefore"/> → <see cref="YearAfter"/>).</summary>
internal sealed record HistoryAnnualOutcome(
    int YearBefore,
    int YearAfter,
    int AnnualRevisionAfter,
    int WorkRevision,
    bool CancelledUnexecutedWork,
    HistoryProvenance Provenance,
    IReadOnlyList<HistoryAreaAnnualOutcome> Areas,
    IReadOnlyList<HistoryPlantedSpeciesAnnualAccount> WholeWoodlandPlantedAccounts,
    HistoryProvenance PerAreaPlantedSplitProvenance,
    string PerAreaPlantedSplitNote);

/// <summary>
/// Cross-referenced follow-up linking a juvenile planted in WorkYear = Y to its current <see cref="PlantedJuvenile"/>
/// record in the committed root (tagged <see cref="HistoryProvenance.Reconstructed"/>).
/// </summary>
internal sealed record HistoryPlantedWorkFollowUp(
    string JuvenileId,
    string AreaId,
    string SpeciesId,
    int WorkYear,
    int RecordedPlantingYear,
    bool CurrentlyAlive,
    bool CurrentlyActive,
    string? PromotedTreeId,
    int DeathYear,
    string? DeathCause,
    HistoryProvenance Provenance);

/// <summary>Current committed stand observation for one area plus derived deltas against ObservedYear = Y + 1.</summary>
internal sealed record HistoryAreaCurrentObservation(
    string AreaId,
    HistoryProvenance CurrentProvenance,
    HistoryProvenance DeltaProvenance,
    int CurrentLiveStems,
    double CurrentBasalAreaM2,
    double? CurrentBasalAreaM2PerHa,
    int? LiveStemDeltaSinceObservedYear,
    double? BasalAreaM2PerHaDeltaSinceObservedYear,
    int? CurrentDeadwoodCount,
    double? CurrentDeadwoodVolumeM3,
    int? NaturalCohortCellsWithPresence,
    double? NaturalCohortTotalAbundance,
    IReadOnlyDictionary<string, double>? NaturalCohortAbundanceBySpecies,
    int? PlantedJuvenilesTotal,
    int? PlantedJuvenilesAliveActive,
    int? PlantedJuvenilesPromoted,
    int? PlantedJuvenilesDead);

/// <summary>Later observed comparison block with explicit CURRENT/DERIVED provenance and non-causal wording.</summary>
internal sealed record HistoryCurrentComparison(
    int AsOfEcologicalYear,
    HistoryProvenance CurrentProvenance,
    HistoryProvenance DeltaProvenance,
    string NonCausalStatement,
    IReadOnlyList<HistoryAreaCurrentObservation> Areas,
    int TotalCurrentLiveStems,
    double TotalCurrentBasalAreaM2,
    int? TotalCurrentDeadwoodCount,
    double? TotalCurrentDeadwoodVolumeM3,
    int? TotalPlantedJuvenilesAliveActive,
    int? TotalPlantedJuvenilesPromoted,
    int? TotalPlantedJuvenilesDead);

/// <summary>Explicit disclosure of a historical quantity that is NOT RECORDED in 2D Phase 1.</summary>
internal sealed record HistoryNotRecordedDisclosure(
    string FieldKey,
    HistoryProvenance Provenance,
    string ProvenanceLabel,
    string Explanation);

/// <summary>
/// Complete read-only Decision → Outcome view for <see cref="WorkYear"/> = Y and <see cref="ObservedYear"/> = Y + 1,
/// preserving real 2D timing, explicit provenance tiers, and honest NOT RECORDED disclosures.
/// </summary>
internal sealed record HistoryYearDetail(
    int WorkYear,
    int ObservedYear,
    int CurrentEcologicalYear,
    bool HasRecordedData,
    HistoryProvenance WorkProvenance,
    ExecutedActionCounts TotalWorkCounts,
    IReadOnlyList<HistoryAreaWorkRecord> WorkByArea,
    HistoryAnnualOutcome? AnnualOutcomeNextYear,
    HistoryAnnualOutcome? AnnualRecordEndingInYear,
    IReadOnlyList<HistoryPlantedWorkFollowUp> PlantedWorkFollowUp,
    HistoryCurrentComparison LaterObservedComparison,
    IReadOnlyList<HistoryNotRecordedDisclosure> NotRecordedDisclosures,
    IReadOnlyList<HistoryProvenanceDescriptor> ProvenanceGlossary);

/// <summary>Pure, deterministic, non-mutating history query over one immutable <see cref="MultiAreaRoot"/>.</summary>
internal static class MultiAreaHistoryQuery
{
    internal const string ActionFell = "Fell";
    internal const string ActionPrune = "Prune";
    internal const string ActionClearUndergrowth = "ClearUndergrowth";
    internal const string ActionPlant = "Plant";
    internal const string ActionShelter = "Shelter";
    internal const string ActionSpotClear = "SpotClear";

    internal const string KeyPastOverheadCanopyCover = "PastOverheadCanopyCover";
    internal const string KeyPastTreeByTreeStructure = "PastTreeByTreeStructure";
    internal const string KeyPerAreaPlantedAccountSplits = "PerAreaPlantedAccountSplits";
    internal const string KeyFinancialLedger = "FinancialLedger";

    private static readonly IReadOnlyList<HistoryNotRecordedDisclosure> StandardNotRecorded = Array.AsReadOnly(new[]
    {
        new HistoryNotRecordedDisclosure(
            KeyPastOverheadCanopyCover,
            HistoryProvenance.NotRecorded,
            HistoryProvenanceCatalog.LabelOf(HistoryProvenance.NotRecorded),
            MultiAreaHistoryLabels.PastOverheadCanopyNotRecorded),
        new HistoryNotRecordedDisclosure(
            KeyPastTreeByTreeStructure,
            HistoryProvenance.NotRecorded,
            HistoryProvenanceCatalog.LabelOf(HistoryProvenance.NotRecorded),
            MultiAreaHistoryLabels.PastTreeStructureNotRecorded),
        new HistoryNotRecordedDisclosure(
            KeyPerAreaPlantedAccountSplits,
            HistoryProvenance.NotRecorded,
            HistoryProvenanceCatalog.LabelOf(HistoryProvenance.NotRecorded),
            MultiAreaHistoryLabels.PerAreaPlantedSplitNotRecorded),
    });

    /// <summary>
    /// Newest-first, cursor-stable page of recorded years over <paramref name="root"/>.
    /// Recorded years are defined honestly as the union of years reachable from <c>AnnualHistory</c>
    /// (<c>YearBefore</c>/<c>YearAfter</c>) and committed event/work-log years.
    /// </summary>
    internal static HistoryIndexPage ReadIndex(MultiAreaRoot root, int pageSize = 10, int? beforeYear = null)
    {
        ArgumentNullException.ThrowIfNull(root);
        if (pageSize < 1)
            throw new P1Refusal("HistoryPageSizeInvalid", "history page size must be at least 1");

        var yearSet = new HashSet<int>();
        var annualByYearAfter = new Dictionary<int, MultiAreaYearRecord>();
        var annualByYearBefore = new Dictionary<int, MultiAreaYearRecord>();

        foreach (var yr in root.AnnualHistory)
        {
            yearSet.Add(yr.YearBefore);
            yearSet.Add(yr.YearAfter);
            annualByYearAfter[yr.YearAfter] = yr;
            annualByYearBefore[yr.YearBefore] = yr;
        }

        var countsByYear = new Dictionary<int, ActionCountAccumulator>();
        ActionCountAccumulator CountsFor(int y)
        {
            yearSet.Add(y);
            if (!countsByYear.TryGetValue(y, out var acc))
                countsByYear[y] = acc = new ActionCountAccumulator();
            return acc;
        }

        foreach (var ev in root.Events)
        {
            yearSet.Add(ev.EcologicalYear);
            foreach (var area in ev.Areas)
            {
                foreach (var fell in area.Fells)
                    CountsFor(fell.EcologicalYear).Fell++;
            }
        }

        if (root.Stage4 is { } stage4)
        {
            foreach (var prune in stage4.PruneLog)
                CountsFor(prune.EcologicalYear).Prune++;
        }

        if (root.Stage5 is { } stage5)
        {
            foreach (var clear in stage5.ClearLog)
                CountsFor(clear.EcologicalYear).ClearUndergrowth++;
        }

        if (root.Phase2 is { } phase2)
        {
            foreach (var work in phase2.WorkLog)
            {
                var acc = CountsFor(work.EcologicalYear);
                switch (work.Kind)
                {
                    case ActionPlant: acc.Plant++; break;
                    case ActionShelter: acc.Shelter++; break;
                    case ActionSpotClear: acc.SpotClear++; break;
                }
            }
        }

        var orderedYears = yearSet.OrderByDescending(y => y).ToArray();
        int totalRecordedYears = orderedYears.Length;

        var filtered = beforeYear is int cursor
            ? orderedYears.Where(y => y < cursor).ToArray()
            : orderedYears;

        int take = Math.Min(pageSize, filtered.Length);
        var items = new HistoryIndexEntry[take];
        for (int i = 0; i < take; i++)
        {
            int y = filtered[i];
            var counts = countsByYear.TryGetValue(y, out var acc) ? acc.Build() : ExecutedActionCounts.Zero;
            bool hasAnnualEnding = annualByYearAfter.TryGetValue(y, out var endingRecord);
            bool hasNextObserved = annualByYearBefore.TryGetValue(y, out var nextRecord) && nextRecord.YearAfter == checked(y + 1);
            int? liveStemsEndOfYear = hasAnnualEnding
                ? endingRecord!.Areas.Sum(a => a.After.LivingCount)
                : null;
            items[i] = new HistoryIndexEntry(
                Year: y,
                HasExecutedWork: counts.HasAny,
                ExecutedActionCounts: counts,
                HasAnnualOutcome: hasAnnualEnding,
                LiveStemsEndOfYear: liveStemsEndOfYear,
                HasObservedOutcomeNextYear: hasNextObserved);
        }

        int? nextCursor = filtered.Length > take && take > 0
            ? items[take - 1].Year
            : null;

        return new HistoryIndexPage(Array.AsReadOnly(items), pageSize, beforeYear, nextCursor, totalRecordedYears);
    }

    /// <summary>
    /// Full read-only Decision → Outcome detail for <c>WorkYear = year</c> and <c>ObservedYear = year + 1</c>.
    /// </summary>
    internal static HistoryYearDetail ReadYear(MultiAreaRoot root, int year,
        IReadOnlyDictionary<string, float>? areaHaById = null)
    {
        ArgumentNullException.ThrowIfNull(root);
        if (year < 0)
            throw new P1Refusal("HistoryYearInvalid", "history year must be non-negative");

        int observedYear = checked(year + 1);
        var areaOrder = root.Areas.Select(a => a.AreaId).ToArray();

        // 1. Executed work in WorkYear = year, grouped by area and action type.
        var fellsByArea = new Dictionary<string, List<HistoryFellDetail>>(StringComparer.Ordinal);
        var prunesByArea = new Dictionary<string, List<HistoryPruneDetail>>(StringComparer.Ordinal);
        var clearsByArea = new Dictionary<string, List<HistoryClearDetail>>(StringComparer.Ordinal);
        var plantingByArea = new Dictionary<string, List<HistoryPlantingDetail>>(StringComparer.Ordinal);

        foreach (var areaId in areaOrder)
        {
            fellsByArea[areaId] = new List<HistoryFellDetail>();
            prunesByArea[areaId] = new List<HistoryPruneDetail>();
            clearsByArea[areaId] = new List<HistoryClearDetail>();
            plantingByArea[areaId] = new List<HistoryPlantingDetail>();
        }

        static List<T> Bucket<T>(Dictionary<string, List<T>> map, string areaId)
        {
            if (!map.TryGetValue(areaId, out var list))
                map[areaId] = list = new List<T>();
            return list;
        }

        foreach (var ev in root.Events)
        {
            foreach (var area in ev.Areas)
            {
                foreach (var fell in area.Fells)
                {
                    if (fell.EcologicalYear != year) continue;
                    Bucket(fellsByArea, fell.AreaId).Add(new HistoryFellDetail(
                        fell.TreeId, fell.Reason, fell.Cause, fell.Mechanism, fell.WorkRevision, fell.PlanId, fell.ExecutionKey));
                }
            }
        }

        if (root.Stage4 is { } stage4)
        {
            foreach (var prune in stage4.PruneLog)
            {
                if (prune.EcologicalYear != year) continue;
                Bucket(prunesByArea, prune.AreaId).Add(new HistoryPruneDetail(
                    prune.TreeId, prune.TargetCrownBaseHeightM, prune.CrownRadiusBeforeM, prune.CrownRadiusAfterM,
                    prune.LiftsAfter, prune.WorkRevision, prune.PlanId, prune.ExecutionKey));
            }
        }

        if (root.Stage5 is { } stage5)
        {
            foreach (var clear in stage5.ClearLog)
            {
                if (clear.EcologicalYear != year) continue;
                Bucket(clearsByArea, clear.AreaId).Add(new HistoryClearDetail(
                    clear.Column, clear.Row, clear.BrambleBefore, clear.BrackenBefore, clear.BandsKept, clear.AbundanceKept,
                    clear.WorkRevision, clear.PlanId, clear.ExecutionKey));
            }
        }

        var plantedJuvenileIdsInWorkYear = new List<string>();
        if (root.Phase2 is { } phase2)
        {
            foreach (var work in phase2.WorkLog)
            {
                if (work.EcologicalYear != year) continue;
                Bucket(plantingByArea, work.AreaId).Add(new HistoryPlantingDetail(
                    work.OrderId, work.Kind, work.XM, work.ZM, work.SpeciesId, work.JuvenileId, work.ShelterId, work.PatchId,
                    work.WorkRevision, work.PlanId, work.ExecutionKey));
                if (string.Equals(work.Kind, ActionPlant, StringComparison.Ordinal) && work.JuvenileId is not null)
                    plantedJuvenileIdsInWorkYear.Add(work.JuvenileId);
            }
        }

        var allAreaIds = areaOrder
            .Concat(fellsByArea.Keys)
            .Concat(prunesByArea.Keys)
            .Concat(clearsByArea.Keys)
            .Concat(plantingByArea.Keys)
            .Distinct(StringComparer.Ordinal)
            .OrderBy(id => id, StringComparer.Ordinal)
            .ToArray();

        int totalFell = 0, totalPrune = 0, totalClear = 0, totalPlant = 0, totalShelter = 0, totalSpotClear = 0;
        var workByArea = new List<HistoryAreaWorkRecord>(allAreaIds.Length);
        foreach (var areaId in allAreaIds)
        {
            var fells = fellsByArea.TryGetValue(areaId, out var fl) ? fl : new List<HistoryFellDetail>();
            var prunes = prunesByArea.TryGetValue(areaId, out var pl) ? pl : new List<HistoryPruneDetail>();
            var clears = clearsByArea.TryGetValue(areaId, out var cl) ? cl : new List<HistoryClearDetail>();
            var planting = plantingByArea.TryGetValue(areaId, out var p2l) ? p2l : new List<HistoryPlantingDetail>();

            var plants = planting.Where(p => string.Equals(p.Kind, ActionPlant, StringComparison.Ordinal)).ToArray();
            var shelters = planting.Where(p => string.Equals(p.Kind, ActionShelter, StringComparison.Ordinal)).ToArray();
            var spotClears = planting.Where(p => string.Equals(p.Kind, ActionSpotClear, StringComparison.Ordinal)).ToArray();

            var counts = new ExecutedActionCounts(fells.Count, prunes.Count, clears.Count, plants.Length, shelters.Length, spotClears.Length);
            totalFell += counts.Fell;
            totalPrune += counts.Prune;
            totalClear += counts.ClearUndergrowth;
            totalPlant += counts.Plant;
            totalShelter += counts.Shelter;
            totalSpotClear += counts.SpotClear;

            var groups = new List<HistoryActionGroup>();
            if (fells.Count > 0)
            {
                groups.Add(new HistoryActionGroup(
                    ActionFell, year, HistoryProvenance.Recorded, fells.Count,
                    TreeIds: Array.AsReadOnly(fells.Select(f => f.TreeId).OrderBy(id => id, StringComparer.Ordinal).ToArray()),
                    Cells: Array.Empty<HistoryCellCoord>(),
                    JuvenileIds: Array.Empty<string>(),
                    ShelterIds: Array.Empty<string>(),
                    PatchIds: Array.Empty<string>(),
                    OrderIds: Array.Empty<string>()));
            }
            if (prunes.Count > 0)
            {
                groups.Add(new HistoryActionGroup(
                    ActionPrune, year, HistoryProvenance.Recorded, prunes.Count,
                    TreeIds: Array.AsReadOnly(prunes.Select(p => p.TreeId).OrderBy(id => id, StringComparer.Ordinal).ToArray()),
                    Cells: Array.Empty<HistoryCellCoord>(),
                    JuvenileIds: Array.Empty<string>(),
                    ShelterIds: Array.Empty<string>(),
                    PatchIds: Array.Empty<string>(),
                    OrderIds: Array.Empty<string>()));
            }
            if (clears.Count > 0)
            {
                groups.Add(new HistoryActionGroup(
                    ActionClearUndergrowth, year, HistoryProvenance.Recorded, clears.Count,
                    TreeIds: Array.Empty<string>(),
                    Cells: Array.AsReadOnly(clears.Select(c => new HistoryCellCoord(c.Column, c.Row))
                        .OrderBy(c => c.Column).ThenBy(c => c.Row).ToArray()),
                    JuvenileIds: Array.Empty<string>(),
                    ShelterIds: Array.Empty<string>(),
                    PatchIds: Array.Empty<string>(),
                    OrderIds: Array.Empty<string>()));
            }
            if (plants.Length > 0)
            {
                groups.Add(new HistoryActionGroup(
                    ActionPlant, year, HistoryProvenance.Recorded, plants.Length,
                    TreeIds: Array.Empty<string>(),
                    Cells: Array.Empty<HistoryCellCoord>(),
                    JuvenileIds: Array.AsReadOnly(plants.Where(p => p.JuvenileId is not null).Select(p => p.JuvenileId!)
                        .OrderBy(id => id, StringComparer.Ordinal).ToArray()),
                    ShelterIds: Array.Empty<string>(),
                    PatchIds: Array.Empty<string>(),
                    OrderIds: Array.AsReadOnly(plants.Select(p => p.OrderId).OrderBy(id => id, StringComparer.Ordinal).ToArray())));
            }
            if (shelters.Length > 0)
            {
                groups.Add(new HistoryActionGroup(
                    ActionShelter, year, HistoryProvenance.Recorded, shelters.Length,
                    TreeIds: Array.Empty<string>(),
                    Cells: Array.Empty<HistoryCellCoord>(),
                    JuvenileIds: Array.AsReadOnly(shelters.Where(s => s.JuvenileId is not null).Select(s => s.JuvenileId!)
                        .OrderBy(id => id, StringComparer.Ordinal).ToArray()),
                    ShelterIds: Array.AsReadOnly(shelters.Where(s => s.ShelterId is not null).Select(s => s.ShelterId!)
                        .OrderBy(id => id, StringComparer.Ordinal).ToArray()),
                    PatchIds: Array.Empty<string>(),
                    OrderIds: Array.AsReadOnly(shelters.Select(s => s.OrderId).OrderBy(id => id, StringComparer.Ordinal).ToArray())));
            }
            if (spotClears.Length > 0)
            {
                groups.Add(new HistoryActionGroup(
                    ActionSpotClear, year, HistoryProvenance.Recorded, spotClears.Length,
                    TreeIds: Array.Empty<string>(),
                    Cells: Array.Empty<HistoryCellCoord>(),
                    JuvenileIds: Array.Empty<string>(),
                    ShelterIds: Array.Empty<string>(),
                    PatchIds: Array.AsReadOnly(spotClears.Where(s => s.PatchId is not null).Select(s => s.PatchId!)
                        .OrderBy(id => id, StringComparer.Ordinal).ToArray()),
                    OrderIds: Array.AsReadOnly(spotClears.Select(s => s.OrderId).OrderBy(id => id, StringComparer.Ordinal).ToArray())));
            }

            workByArea.Add(new HistoryAreaWorkRecord(
                areaId, year, HistoryProvenance.Recorded, counts, Array.AsReadOnly(groups.ToArray()),
                Array.AsReadOnly(fells.ToArray()), Array.AsReadOnly(prunes.ToArray()),
                Array.AsReadOnly(clears.ToArray()), Array.AsReadOnly(planting.ToArray())));
        }

        var totalWorkCounts = new ExecutedActionCounts(totalFell, totalPrune, totalClear, totalPlant, totalShelter, totalSpotClear);

        // 2. Annual outcomes: ObservedYear = Y + 1 (YearBefore == Y && YearAfter == Y + 1), plus Year Y annual record when present.
        var nextYearRecord = root.AnnualHistory.FirstOrDefault(r => r.YearBefore == year && r.YearAfter == observedYear);
        var endingYearRecord = root.AnnualHistory.FirstOrDefault(r => r.YearAfter == year);

        HistoryAnnualOutcome? annualOutcomeNextYear = nextYearRecord is null ? null : BuildAnnualOutcome(root, nextYearRecord);
        HistoryAnnualOutcome? annualRecordEndingInYear = endingYearRecord is null ? null : BuildAnnualOutcome(root, endingYearRecord);

        // 3. Reconstructed follow-up for juveniles planted in WorkYear = Y.
        var followUps = new List<HistoryPlantedWorkFollowUp>();
        if (root.Phase2 is { } p2ForFollowUp && plantedJuvenileIdsInWorkYear.Count > 0)
        {
            var juvenileById = p2ForFollowUp.Juveniles.ToDictionary(j => j.JuvenileId, StringComparer.Ordinal);
            foreach (var juvenileId in plantedJuvenileIdsInWorkYear.OrderBy(id => id, StringComparer.Ordinal))
            {
                if (!juvenileById.TryGetValue(juvenileId, out var j)) continue;
                followUps.Add(new HistoryPlantedWorkFollowUp(
                    j.JuvenileId, j.AreaId, j.SpeciesId, year, j.PlantingYear,
                    j.Alive, j.IsActive, j.PromotedTreeId, j.DeathYear, j.DeathCause,
                    HistoryProvenance.Reconstructed));
            }
        }

        // 4. Later observed comparison (CURRENT + DERIVED, exact non-causal statement).
        var comparison = BuildCurrentComparison(root, annualOutcomeNextYear, areaHaById);

        bool hasRecordedData = totalWorkCounts.HasAny || annualOutcomeNextYear is not null || annualRecordEndingInYear is not null;

        return new HistoryYearDetail(
            WorkYear: year,
            ObservedYear: observedYear,
            CurrentEcologicalYear: root.EcologicalYear,
            HasRecordedData: hasRecordedData,
            WorkProvenance: HistoryProvenance.Recorded,
            TotalWorkCounts: totalWorkCounts,
            WorkByArea: Array.AsReadOnly(workByArea.ToArray()),
            AnnualOutcomeNextYear: annualOutcomeNextYear,
            AnnualRecordEndingInYear: annualRecordEndingInYear,
            PlantedWorkFollowUp: Array.AsReadOnly(followUps.ToArray()),
            LaterObservedComparison: comparison,
            NotRecordedDisclosures: root.Economy is null
                ? Array.AsReadOnly(StandardNotRecorded.Append(new HistoryNotRecordedDisclosure(
                    KeyFinancialLedger,
                    HistoryProvenance.NotRecorded,
                    HistoryProvenanceCatalog.LabelOf(HistoryProvenance.NotRecorded),
                    "Economy model is Off; cash balance, contractor charges, timber receipts, and landowner hours are NOT RECORDED.")).ToArray())
                : StandardNotRecorded,
            ProvenanceGlossary: HistoryProvenanceCatalog.All);
    }

    private static HistoryAnnualOutcome BuildAnnualOutcome(MultiAreaRoot root, MultiAreaYearRecord yearRecord)
    {
        var stage5Year = root.Stage5?.Accounts.FirstOrDefault(a => a.YearBefore == yearRecord.YearBefore && a.YearAfter == yearRecord.YearAfter);
        var phase2Year = root.Phase2?.Accounts.FirstOrDefault(a => a.YearBefore == yearRecord.YearBefore && a.YearAfter == yearRecord.YearAfter);

        var areaOutcomes = new HistoryAreaAnnualOutcome[yearRecord.Areas.Count];
        for (int i = 0; i < yearRecord.Areas.Count; i++)
        {
            var areaYear = yearRecord.Areas[i];
            var s5Area = stage5Year?.Areas.FirstOrDefault(a => string.Equals(a.AreaId, areaYear.AreaId, StringComparison.Ordinal));
            var naturalRegen = new List<HistoryAreaNaturalRegenOutcome>();
            if (s5Area is not null)
            {
                naturalRegen.Add(new HistoryAreaNaturalRegenOutcome(
                    Phase2Species.SitkaId,
                    s5Area.StartingAbundance,
                    s5Area.EndingAbundance,
                    s5Area.EstablishmentAccepted,
                    s5Area.Promotions.Count,
                    Array.AsReadOnly(s5Area.Promotions.Select(p => p.TreeId).OrderBy(id => id, StringComparer.Ordinal).ToArray())));
            }
            if (phase2Year is not null)
            {
                foreach (var p2Nat in phase2Year.Natural
                    .Where(n => string.Equals(n.AreaId, areaYear.AreaId, StringComparison.Ordinal))
                    .OrderBy(n => n.SpeciesId, StringComparer.Ordinal))
                {
                    naturalRegen.Add(new HistoryAreaNaturalRegenOutcome(
                        p2Nat.SpeciesId,
                        p2Nat.StartingAbundance,
                        p2Nat.EndingAbundance,
                        p2Nat.EstablishmentAccepted,
                        p2Nat.Promotions.Count,
                        Array.AsReadOnly(p2Nat.Promotions.Select(p => p.TreeId).OrderBy(id => id, StringComparer.Ordinal).ToArray())));
                }
            }

            areaOutcomes[i] = new HistoryAreaAnnualOutcome(
                areaYear.AreaId,
                areaYear.Before,
                areaYear.After,
                areaYear.NaturalDeaths.Count,
                Array.AsReadOnly(areaYear.NaturalDeaths.Select(d => d.TreeId).OrderBy(id => id, StringComparer.Ordinal).ToArray()),
                areaYear.NoLivingStems,
                s5Area?.MeanBramble,
                s5Area?.MeanBracken,
                s5Area?.MeanLight,
                Array.AsReadOnly(naturalRegen.ToArray()));
        }

        var plantedAccounts = phase2Year is null
            ? Array.Empty<HistoryPlantedSpeciesAnnualAccount>()
            : phase2Year.Planted
                .OrderBy(p => p.SpeciesId, StringComparer.Ordinal)
                .Select(p => new HistoryPlantedSpeciesAnnualAccount(
                    p.SpeciesId,
                    Scope: "WholeWoodland",
                    Provenance: HistoryProvenance.Recorded,
                    p.Starting,
                    p.VegetationDeaths,
                    p.LightDeaths,
                    p.BrowseDeaths,
                    p.Remaining,
                    p.Promoted,
                    Array.AsReadOnly(p.PromotedTreeIds.OrderBy(id => id, StringComparer.Ordinal).ToArray())))
                .ToArray();

        return new HistoryAnnualOutcome(
            yearRecord.YearBefore,
            yearRecord.YearAfter,
            yearRecord.AnnualRevisionAfter,
            yearRecord.WorkRevision,
            yearRecord.CancelledUnexecutedWork,
            HistoryProvenance.Recorded,
            Array.AsReadOnly(areaOutcomes),
            Array.AsReadOnly(plantedAccounts),
            HistoryProvenance.NotRecorded,
            MultiAreaHistoryLabels.PerAreaPlantedSplitNotRecorded);
    }

    private static HistoryCurrentComparison BuildCurrentComparison(MultiAreaRoot root,
        HistoryAnnualOutcome? observedNextYear, IReadOnlyDictionary<string, float>? areaHaById)
    {
        var areaObservations = new HistoryAreaCurrentObservation[root.Areas.Count];
        int totalLive = 0;
        double totalBasalAreaM2 = 0.0;
        int? totalDeadwoodCount = root.Stage4 is null ? null : 0;
        double? totalDeadwoodVolM3 = root.Stage4 is null ? null : 0.0;
        int? totalPlantedActive = root.Phase2 is null ? null : 0;
        int? totalPlantedPromoted = root.Phase2 is null ? null : 0;
        int? totalPlantedDead = root.Phase2 is null ? null : 0;

        for (int i = 0; i < root.Areas.Count; i++)
        {
            var area = root.Areas[i];
            int liveCount = 0;
            double basalAreaM2 = 0.0;
            foreach (var tree in area.State.Trees)
            {
                if (!tree.IsLiving) continue;
                liveCount++;
                basalAreaM2 += (double)tree.DbhCm * tree.DbhCm * (Math.PI / 40000.0);
            }
            totalLive += liveCount;
            totalBasalAreaM2 += basalAreaM2;

            double? basalAreaPerHa = areaHaById is not null && areaHaById.TryGetValue(area.AreaId, out float areaHa) && areaHa > 0f
                ? WorkRules.Summarize(area.State.Trees, areaHa).BasalAreaM2PerHa
                : null;

            var observedArea = observedNextYear?.Areas.FirstOrDefault(a => string.Equals(a.AreaId, area.AreaId, StringComparison.Ordinal));
            int? liveDelta = observedArea is null ? null : liveCount - observedArea.After.LivingCount;
            double? baPerHaDelta = observedArea is null || basalAreaPerHa is null
                ? null
                : basalAreaPerHa.Value - observedArea.After.BasalAreaM2PerHa;

            int? dwCount = null;
            double? dwVol = null;
            if (root.Stage4 is { } s4)
            {
                int c = 0;
                double v = 0.0;
                foreach (var dw in s4.Deadwood)
                {
                    if (!string.Equals(dw.AreaId, area.AreaId, StringComparison.Ordinal)) continue;
                    c++;
                    v += dw.EstimatedVolumeM3;
                }
                dwCount = c;
                dwVol = v;
                totalDeadwoodCount = totalDeadwoodCount!.Value + c;
                totalDeadwoodVolM3 = totalDeadwoodVolM3!.Value + v;
            }

            int? cellsWithNatural = null;
            double? totalNaturalAbundance = null;
            IReadOnlyDictionary<string, double>? naturalBySpecies = null;
            if (root.Stage5 is { } s5)
            {
                var s5Area = s5.Areas.FirstOrDefault(a => string.Equals(a.AreaId, area.AreaId, StringComparison.Ordinal));
                var p2Area = root.Phase2?.Areas.FirstOrDefault(a => string.Equals(a.AreaId, area.AreaId, StringComparison.Ordinal));
                var bySpecies = new Dictionary<string, double>(StringComparer.Ordinal);
                int presentCells = 0;
                double sumAbundance = 0.0;
                if (s5Area is not null)
                {
                    for (int cIdx = 0; cIdx < s5Area.Cells.Count; cIdx++)
                    {
                        var s5Cell = s5Area.Cells[cIdx];
                        double sitkaAb = s5Cell.Abundance;
                        if (sitkaAb > 0.0)
                        {
                            bySpecies[Phase2Species.SitkaId] = bySpecies.GetValueOrDefault(Phase2Species.SitkaId) + sitkaAb;
                            sumAbundance += sitkaAb;
                        }

                        double broadleafAb = 0.0;
                        if (p2Area is not null && cIdx < p2Area.Cells.Count)
                        {
                            foreach (var band in p2Area.Cells[cIdx].Bands)
                            {
                                if (band.Density <= 0f) continue;
                                broadleafAb += band.Density;
                                bySpecies[band.SpeciesId] = bySpecies.GetValueOrDefault(band.SpeciesId) + band.Density;
                                sumAbundance += band.Density;
                            }
                        }

                        if (sitkaAb > 0.0 || broadleafAb > 0.0)
                            presentCells++;
                    }
                }
                cellsWithNatural = presentCells;
                totalNaturalAbundance = sumAbundance;
                naturalBySpecies = bySpecies;
            }

            int? pjTotal = null, pjActive = null, pjPromoted = null, pjDead = null;
            if (root.Phase2 is { } p2)
            {
                int t = 0, a = 0, pr = 0, d = 0;
                foreach (var j in p2.Juveniles)
                {
                    if (!string.Equals(j.AreaId, area.AreaId, StringComparison.Ordinal)) continue;
                    t++;
                    if (j.PromotedTreeId is not null) pr++;
                    else if (j.IsActive) a++;
                    else d++;
                }
                pjTotal = t;
                pjActive = a;
                pjPromoted = pr;
                pjDead = d;
                totalPlantedActive = totalPlantedActive!.Value + a;
                totalPlantedPromoted = totalPlantedPromoted!.Value + pr;
                totalPlantedDead = totalPlantedDead!.Value + d;
            }

            areaObservations[i] = new HistoryAreaCurrentObservation(
                area.AreaId,
                HistoryProvenance.Current,
                HistoryProvenance.Derived,
                liveCount,
                basalAreaM2,
                basalAreaPerHa,
                liveDelta,
                baPerHaDelta,
                dwCount,
                dwVol,
                cellsWithNatural,
                totalNaturalAbundance,
                naturalBySpecies,
                pjTotal,
                pjActive,
                pjPromoted,
                pjDead);
        }

        return new HistoryCurrentComparison(
            AsOfEcologicalYear: root.EcologicalYear,
            CurrentProvenance: HistoryProvenance.Current,
            DeltaProvenance: HistoryProvenance.Derived,
            NonCausalStatement: MultiAreaHistoryLabels.NonCausalComparisonStatement,
            Areas: Array.AsReadOnly(areaObservations),
            TotalCurrentLiveStems: totalLive,
            TotalCurrentBasalAreaM2: totalBasalAreaM2,
            TotalCurrentDeadwoodCount: totalDeadwoodCount,
            TotalCurrentDeadwoodVolumeM3: totalDeadwoodVolM3,
            TotalPlantedJuvenilesAliveActive: totalPlantedActive,
            TotalPlantedJuvenilesPromoted: totalPlantedPromoted,
            TotalPlantedJuvenilesDead: totalPlantedDead);
    }

    private sealed class ActionCountAccumulator
    {
        public int Fell;
        public int Prune;
        public int ClearUndergrowth;
        public int Plant;
        public int Shelter;
        public int SpotClear;

        public ExecutedActionCounts Build() => new(Fell, Prune, ClearUndergrowth, Plant, Shelter, SpotClear);
    }
}

internal sealed partial class MultiAreaContinuation
{
    /// <summary>
    /// Read-only newest-first paged index of recorded ecological years. Locks once to snapshot the immutable committed
    /// root, then queries purely with zero mutation.
    /// </summary>
    internal HistoryIndexPage ReadHistoryIndex(int pageSize = 10, int? beforeYear = null)
    {
        MultiAreaRoot snapshot;
        lock (sync) snapshot = root;
        return MultiAreaHistoryQuery.ReadIndex(snapshot, pageSize, beforeYear);
    }

    /// <summary>
    /// Read-only Decision → Outcome detail for <paramref name="year"/> (<c>WorkYear = year</c>,
    /// <c>ObservedYear = year + 1</c>). Locks once to snapshot the immutable committed root, then queries purely.
    /// </summary>
    internal HistoryYearDetail ReadHistoryYear(int year)
    {
        MultiAreaRoot snapshot;
        lock (sync) snapshot = root;
        var areaHa = new Dictionary<string, float>(areas.Count, StringComparer.Ordinal);
        foreach (var info in areas)
            areaHa[info.AreaId] = info.AreaHa;
        return MultiAreaHistoryQuery.ReadYear(snapshot, year, areaHa);
    }
}
