namespace SitkaP1.Core.Phase1;

/// <summary>What <see cref="Phase1RootMapping.Restore"/> rebuilds: the engine root plus the session inputs held outside it.</summary>
internal sealed record Phase1RestoredRoot(MultiAreaRoot Root, MultiAreaModelSelection Models, Stage4Options? Stage4Options);

/// <summary>
/// INTERNAL A1 SEAM — pure field copies between the engine's commit root and the public save DTO. Nothing here locks,
/// fences, publishes or validates against the engine; it is not called by any engine code in A0. A1 must call
/// <see cref="Capture"/> under the continuation's single root lock (it needs the private root, model selection and
/// Stage 4 options) and must run its own full restore validation, fresh session ID and approved→draft conversion around
/// <see cref="Restore"/>.
/// <para>
/// A0 status: compile-verified only. Every engine record is built positionally in <see cref="Restore"/>, so a missing or
/// mistyped field fails the build; but the separate test assembly cannot reach these internal types (no
/// InternalsVisibleTo by packet rule), so the round trip root → DTO → root is NOT runtime-tested in A0.
/// </para>
/// </summary>
internal static class Phase1RootMapping
{
    // ------------------------------------------------------------------------------------------------ identity

    internal static Phase1ModelVector VectorOf(MultiAreaModelSelection m) =>
        new((int)m.Adult, (int)m.Mortality, (int)m.Crown, (int)m.Regeneration, (int)m.Species);

    internal static MultiAreaModelSelection SelectionOf(Phase1ModelVector v) =>
        new MultiAreaModelSelection((AdultModelVersion)v.Adult, (MortalityModelVersion)v.Mortality, (CrownModelVersion)v.Crown,
            (MultiAreaRegenerationModelVersion)v.Regeneration, (SpeciesModelVersion)v.Species).RequireSupported();

    /// <summary>The immutable genesis identity of an experiment definition (content hash and deterministic inputs).</summary>
    internal static Phase1GenesisIdentity GenesisOf(MultiAreaExperiment definition) =>
        new(definition.ExperimentId, definition.ContentHash(), definition.GeneratorId, definition.GeneratorVersion,
            definition.GeneratorSeed,
            Array.AsReadOnly(definition.Areas.Select(a => new Phase1GenesisArea(a.AreaId, a.SimulationSeed, a.AreaHa)).ToArray()));

    // ------------------------------------------------------------------------------------------------ capture

    /// <summary>Copies one committed root into the DTO. Refuses state that has no exact data form.</summary>
    internal static Phase1SaveState Capture(MultiAreaRoot root, MultiAreaModelSelection models, Stage4Options? stage4Options)
    {
        ArgumentNullException.ThrowIfNull(root);
        ArgumentNullException.ThrowIfNull(models);
        if ((stage4Options is not null) != models.IsStage4)
            throw new P1Refusal("Phase1CaptureInvalid", "Stage 4 options are captured exactly for the Stage 4 selections");

        var eventsByKey = root.Events.ToDictionary(e => e.ExecutionKey, StringComparer.Ordinal);
        var executed = root.Executed.OrderBy(kv => kv.Key, StringComparer.Ordinal).Select(kv =>
        {
            // The engine stores the SAME event object in the ledger and the receipt; the DTO keeps it once (in the ledger).
            if (!eventsByKey.TryGetValue(kv.Key, out var ledger) || !ReferenceEquals(ledger, kv.Value.Event))
                throw new P1Refusal("Phase1CaptureUnrepresentable", "an executed-key receipt does not share its ledger event");
            return new Phase1ExecutedWork(kv.Key, Token(kv.Value.Source), Orders(kv.Value.Orders));
        }).ToArray();

        var options = new Phase1Options(
            stage4Options is null ? null : Stage4Options(stage4Options),
            root.Stage5 is null ? null : new Phase1Stage5Options(root.Stage5.Options.GridCellM, root.Stage5.Options.BackgroundBrowsePressure,
                root.Stage5.Options.MastSeed),
            root.Phase2 is null ? null : new Phase1Phase2Options(root.Phase2.Options.OakMastSeed, root.Phase2.Options.BeechMastSeed,
                Species(root.Phase2.Options.GenesisSpecies)));

        return new Phase1SaveState(options, root.EcologicalYear, root.AnnualRevision, root.WorkRevision, root.PlanStateRevision,
            L(root.Areas.Select(a => new Phase1AreaState(a.AreaId, a.State.EcologicalYear, L(a.State.Trees.Select(Tree)), L(a.CropTreeIds)))),
            L(root.Events.Select(Event)), L(executed), Orders(root.Pending),
            root.Approved is null ? null
                : new Phase1ApprovedPlan(root.Approved.PlanId, root.Approved.ExecutionKey, Token(root.Approved.Source), Orders(root.Approved.Orders)),
            L(root.AnnualHistory.Select(y => new Phase1YearRecord(y.YearBefore, y.YearAfter, y.AnnualRevisionAfter, y.WorkRevision,
                L(y.Areas.Select(a => new Phase1AreaYear(a.AreaId, Stand(a.Before), Stand(a.After), L(a.NaturalDeaths.Select(Death)), a.NoLivingStems))),
                y.CancelledUnexecutedWork))),
            root.Stage4 is null ? null : Stage4(root.Stage4),
            root.Stage5 is null ? null : Stage5(root.Stage5),
            root.Phase2 is null ? null : Phase2(root.Phase2));
    }

    private static Phase1Stage4Options Stage4Options(Stage4Options o)
    {
        if (o.SiteProductivity != (SiteProductivityField)SitkaP1.Core.Stage4Options.Uniform)
            throw new P1Refusal("Phase1SiteProductivityNotSerializable",
                "only the neutral uniform site-productivity field can be saved; a custom field is code with no data form");
        return new Phase1Stage4Options(Phase1SaveFormat.SiteProductivityUniform,
            o.GrowthAgePhases is null ? null
                : L(o.GrowthAgePhases.Select(p => new Phase1GrowthAgePhase(p.StartAgeYears, p.DbhIncrementFactor, p.HeightIncrementFactor))));
    }

    private static Phase1Tree Tree(AnnualTree t) =>
        new(t.TreeId, t.XM, t.ZM, t.AgeYears, t.DbhCm, t.HeightM, t.CrownRadiusM, t.Death is null ? null : Death(t.Death));

    private static Phase1Death Death(AnnualDeath d) => new(d.TreeId, d.EcologicalYear, d.Cause, d.Mechanism);

    private static Phase1Stand Stand(AnnualStand s) =>
        new(s.LivingCount, s.StemsPerHa, s.BasalAreaM2PerHa, s.QuadraticMeanDbhCm, s.RelativeDensity);

    private static Phase1SourceToken Token(MultiAreaSourceToken t) =>
        new(t.Domain, t.ExperimentId, t.ManifestContentHash, t.AreaIds, t.EcologicalYear, t.AnnualRevision, t.WorkRevision,
            t.RngModelVersion, t.GrowthModelVersion, t.P1AlgorithmVersion, t.AreaModelVersion, t.ActionEligibleDbhThresholdMm);

    private static IReadOnlyList<Phase1WorkOrder> Orders(IEnumerable<MultiAreaWorkOrder> orders) =>
        L(orders.Select(o => new Phase1WorkOrder(o.AreaId, o.TreeId, o.Action, o.Reason)));

    private static Phase1WorkEvent Event(MultiAreaWorkEvent e) =>
        new(e.ExecutionKey, e.PlanId, e.EcologicalYear, e.WorkRevision, L(e.Areas.Select(c => new Phase1AreaChange(c.AreaId,
            Stand(c.Before), Stand(c.After), L(c.Fells.Select(f => new Phase1FellRecord(f.AreaId, f.TreeId, f.Cause, f.Mechanism, f.Reason,
                f.EcologicalYear, f.WorkRevision, f.ExecutionKey, f.PlanId)))))));

    private static IReadOnlyList<Phase1TreeSpecies> Species(IReadOnlyDictionary<string, string> map) =>
        L(map.OrderBy(kv => kv.Key, StringComparer.Ordinal).Select(kv => new Phase1TreeSpecies(kv.Key, kv.Value)));

    private static Phase1Stage4State Stage4(MultiAreaStage4State s) =>
        new(L(s.Pruning.OrderBy(kv => kv.Key, StringComparer.Ordinal)
                .Select(kv => new Phase1TreePruning(kv.Key, kv.Value.Lifts, kv.Value.CrownBaseHeightM, kv.Value.LastPruningYear))),
            L(s.PendingPrunes.Select(Prune)), L(s.ApprovedPrunes.Select(Prune)),
            L(s.PruneLog.Select(r => new Phase1PruneRecord(r.AreaId, r.TreeId, r.TargetCrownBaseHeightM, r.EcologicalYear, r.WorkRevision,
                r.ExecutionKey, r.PlanId, r.CrownRadiusBeforeM, r.CrownRadiusAfterM, r.LiftsAfter))),
            L(s.Deadwood.Select(d => new Phase1Deadwood(d.DeadwoodId, d.TreeId, d.AreaId, d.SpeciesId, d.XM, d.ZM, d.DeathYear, d.Cause,
                d.DbhCm, d.HeightM, d.EstimatedVolumeM3))),
            s.NextDeadwoodNumber);

    private static Phase1PruneOrder Prune(MultiAreaPruneOrder o) => new(o.AreaId, o.TreeId, o.TargetCrownBaseHeightM);

    private static Phase1Stage5State Stage5(MultiAreaStage5State s) =>
        new(s.MastSeed,
            L(s.Areas.Select(a => new Phase1Stage5AreaCells(a.AreaId, L(a.Cells.Select(c => new Phase1Stage5Cell(c.Column, c.Row, c.CentreXM,
                c.CentreZM, c.ClippedAreaM2, c.Light, c.Bramble, c.Bracken, L(c.Bands.Select(b => new Phase1Stage5Band(b.Density, b.Height,
                    b.EstablishYear, b.OriginYear, b.LastBrowsedFraction, b.LastBrowseAssessmentYear))))))))),
            L(s.PendingClears.Select(Clear)), L(s.ApprovedClears.Select(Clear)),
            L(s.ClearLog.Select(r => new Phase1ClearRecord(r.AreaId, r.Column, r.Row, r.EcologicalYear, r.WorkRevision, r.ExecutionKey,
                r.PlanId, r.BrambleBefore, r.BrackenBefore, r.BandsKept, r.AbundanceKept))),
            L(s.Accounts.Select(y => new Phase1Stage5YearAccount(y.YearBefore, y.YearAfter, y.MastRoll, y.MastLabel, y.MastMultiplier,
                L(y.Areas.Select(a => new Phase1Stage5AreaAccount(a.AreaId, a.Cells, a.SeedArrival, a.StartingAbundance, a.VegetationLoss,
                    a.LightLoss, a.BrowseLoss, a.EstablishmentRequested, a.EstablishmentAccepted, a.CapacityRejected, a.BandsCreated,
                    a.BandMerges, a.ThresholdCrossings, a.QualifyingCells, a.BlockCapDeferredCells, a.PromotionExported, a.EndingAbundance,
                    a.MeanRelativeOccupancy, a.MeanBramble, a.MeanBracken, a.MeanLight, L(a.Promotions.Select(Promotion)),
                    a.CohortBandsExported, a.BudgetWithheldBlocks, L(a.CellEstablishmentAccepted))))))),
            L(s.Blocks.Select(b => new Phase1BlockCredit(b.BlockX, b.BlockZ, b.InPolygonCells, b.Credit))));

    private static Phase1ClearOrder Clear(MultiAreaClearUndergrowthOrder o) => new(o.AreaId, o.Column, o.Row);

    private static Phase1PromotionRecord Promotion(Stage5PromotionRecord p) =>
        new(p.TreeId, p.AreaId, p.Column, p.Row, p.BlockX, p.BlockZ, p.EstablishYear, p.AgeYears, p.HeightM, p.DbhCm, p.CrownRadiusM,
            p.WoodlandCi, p.XM, p.ZM, p.CentreFallback, p.ExportedDensity, p.CohortBandsExported, p.CohortDensityExported);

    private static Phase1Phase2State Phase2(MultiAreaPhase2State s) =>
        new(s.OakMastSeed, s.BeechMastSeed, Species(s.TreeSpecies),
            L(s.Areas.Select(a => new Phase1Phase2AreaBands(a.AreaId, L(a.Cells.Select(c => new Phase1Phase2CellBands(c.Column, c.Row,
                L(c.Bands.Select(b => new Phase1Phase2Band(b.SpeciesId, b.Origin, b.Density, b.Height, b.EstablishYear, b.OriginYear,
                    b.LastBrowsedFraction, b.LastBrowseAssessmentYear))))))))),
            L(s.Juveniles.Select(j => new Phase1PlantedJuvenile(j.JuvenileId, j.AreaId, j.SpeciesId, j.XM, j.ZM, j.PlantingYear, j.AgeYears,
                j.HeightM, j.Alive, j.PromotedTreeId, j.DeathYear, j.DeathCause, j.LastYearBrowsed, j.LastBrowseProbability,
                j.LastVegetationLossProbability, j.LastAssessmentYear))),
            L(s.Shelters.Select(h => new Phase1Shelter(h.ShelterId, h.JuvenileId, h.XM, h.ZM, h.InstalledYear, h.EffectiveYears, h.FailedYear))),
            L(s.Patches.Select(p => new Phase1SpotPatch(p.PatchId, p.AreaId, p.XM, p.ZM, p.RadiusM, p.CreatedYear, p.UpdatedYear, p.Bramble,
                p.Bracken))),
            L(s.PendingOrders.Select(Planting)), L(s.ApprovedOrders.Select(Planting)),
            L(s.WorkLog.Select(r => new Phase1Phase2WorkRecord(r.OrderId, r.Kind, r.AreaId, r.XM, r.ZM, r.SpeciesId, r.JuvenileId, r.ShelterId,
                r.PatchId, r.EcologicalYear, r.WorkRevision, r.ExecutionKey, r.PlanId))),
            s.NextOrderNumber, s.NextJuvenileNumber, s.NextPatchNumber,
            L(s.Accounts.Select(y => new Phase1Phase2YearAccount(y.YearBefore, y.YearAfter,
                L(y.Mast.Select(m => new Phase1MastRecord(m.SpeciesId, m.MastSeed, m.Roll, m.Label, m.Multiplier))),
                L(y.Natural.Select(a => new Phase1SpeciesAreaAccount(a.AreaId, a.SpeciesId, a.SeedArrival, a.StartingAbundance, a.VegetationLoss,
                    a.LightLoss, a.BrowseLoss, a.EstablishmentRequested, a.EstablishmentAccepted, a.CapacityRejected, a.BandsCreated,
                    a.BandMerges, a.ThresholdCrossings, a.QualifyingCells, a.BlockCapDeferredCells, a.PromotionExported, a.EndingAbundance,
                    a.CohortBandsExported, a.BudgetWithheldBlocks, L(a.Promotions.Select(Promotion))))),
                L(y.Planted.Select(p => new Phase1PlantedAccount(p.SpeciesId, p.Starting, p.VegetationDeaths, p.LightDeaths, p.BrowseDeaths,
                    p.Remaining, p.Promoted, L(p.PromotedTreeIds)))),
                L(y.Density.Select(d => new Phase1DensityRecord(d.AreaId, d.Mixed, d.Stems, d.SitkaSubset, d.SitkaHazardDeaths,
                    d.SitkaBoundaryDeaths, d.Occupancy, d.GenericResponse, d.NonReferenceShare, d.EligibleForGeneric, d.ExpectedGeneric,
                    d.GenericQuota, d.QuotaRemainder, d.QuotaRoll, d.GenericDeaths, d.BudgetPassedOver, d.SingleStemFallback,
                    d.StandBasalAreaM2, d.GenericBasalAreaM2, L(d.GenericVictims))))))));

    private static Phase1PlantingOrder Planting(MultiAreaPlantingOrder o) => o switch
    {
        MultiAreaPlantOrder p => new Phase1PlantOrder(p.OrderId, p.AreaId, p.SpeciesId, p.XM, p.ZM),
        MultiAreaShelterOrder h => new Phase1ShelterOrder(h.OrderId, h.AreaId, h.XM, h.ZM),
        MultiAreaSpotClearOrder c => new Phase1SpotClearOrder(c.OrderId, c.AreaId, c.XM, c.ZM),
        _ => throw new P1Refusal("Phase1CaptureUnrepresentable", "unknown planting order type"),
    };

    // ------------------------------------------------------------------------------------------------ restore

    /// <summary>
    /// Rebuilds engine records from a DTO that already passed <see cref="Phase1SaveCodec.Decode(ReadOnlySpan{byte}, Phase1SaveExpectation?, int)"/>.
    /// This is a field copy only: the caller (A1) owns engine revalidation, the fresh session and the approved→draft policy.
    /// </summary>
    internal static Phase1RestoredRoot Restore(Phase1SaveState s, Phase1ModelVector vector)
    {
        ArgumentNullException.ThrowIfNull(s);
        var models = SelectionOf(vector);
        var events = L(s.Events.Select(Event));
        var eventsByKey = events.ToDictionary(e => e.ExecutionKey, StringComparer.Ordinal);
        var executed = new Dictionary<string, MultiAreaExecutedWork>(StringComparer.Ordinal);
        foreach (var x in s.Executed)
            executed.Add(x.ExecutionKey, new MultiAreaExecutedWork(Token(x.RecordedSource), Orders(x.Orders), eventsByKey[x.ExecutionKey]));

        Stage4Options? stage4Options = null;
        if (s.Options.Stage4 is { } o4)
        {
            if (o4.SiteProductivity != Phase1SaveFormat.SiteProductivityUniform)
                throw new P1Refusal("Phase1SiteProductivityNotSerializable", "unknown site-productivity field");
            stage4Options = new Stage4Options
            {
                GrowthAgePhases = o4.GrowthAgePhases is null ? null
                    : L(o4.GrowthAgePhases.Select(p => new GrowthAgePhase(p.StartAgeYears, p.DbhIncrementFactor, p.HeightIncrementFactor))),
            };
        }

        var root = new MultiAreaRoot(s.EcologicalYear, s.AnnualRevision, s.WorkRevision, s.PlanStateRevision,
            L(s.Areas.Select(a => new MultiAreaAreaState(a.AreaId, new AnnualState(a.EcologicalYear, L(a.Trees.Select(Tree))), L(a.CropTreeIds)))),
            events, executed, Orders(s.Pending),
            s.Approved is null ? null
                : new MultiAreaApprovedPlan(s.Approved.PlanId, s.Approved.ExecutionKey, Token(s.Approved.RecordedSource), Orders(s.Approved.Orders)),
            L(s.AnnualHistory.Select(y => new MultiAreaYearRecord(y.YearBefore, y.YearAfter, y.AnnualRevisionAfter, y.WorkRevision,
                L(y.Areas.Select(a => new MultiAreaAreaYear(a.AreaId, Stand(a.Before), Stand(a.After), L(a.NaturalDeaths.Select(Death)),
                    a.NoLivingStems))), y.CancelledUnexecutedWork))),
            s.Stage4 is null ? null : Stage4(s.Stage4),
            s.Stage5 is null ? null : Stage5(s.Stage5, s.Options.Stage5!),
            s.Phase2 is null ? null : Phase2(s.Phase2, s.Options.Phase2!));
        return new Phase1RestoredRoot(root, models, stage4Options);
    }

    private static AnnualTree Tree(Phase1Tree t) =>
        new(t.TreeId, t.XM, t.ZM, t.AgeYears, t.DbhCm, t.HeightM, t.CrownRadiusM, t.Death is null ? null : Death(t.Death));

    private static AnnualDeath Death(Phase1Death d) => new(d.TreeId, d.EcologicalYear, d.Cause, d.Mechanism);

    private static AnnualStand Stand(Phase1Stand s) =>
        new(s.LivingCount, s.StemsPerHa, s.BasalAreaM2PerHa, s.QuadraticMeanDbhCm, s.RelativeDensity);

    private static MultiAreaSourceToken Token(Phase1SourceToken t) =>
        new(t.Domain, t.ExperimentId, t.ManifestContentHash, t.AreaIds, t.EcologicalYear, t.AnnualRevision, t.WorkRevision,
            t.RngModelVersion, t.GrowthModelVersion, t.P1AlgorithmVersion, t.AreaModelVersion, t.ActionEligibleDbhThresholdMm);

    private static IReadOnlyList<MultiAreaWorkOrder> Orders(IEnumerable<Phase1WorkOrder> orders) =>
        L(orders.Select(o => new MultiAreaWorkOrder(o.AreaId, o.TreeId, o.Action, o.Reason)));

    private static MultiAreaWorkEvent Event(Phase1WorkEvent e) =>
        new(e.ExecutionKey, e.PlanId, e.EcologicalYear, e.WorkRevision, L(e.Areas.Select(c => new MultiAreaAreaChange(c.AreaId,
            Stand(c.Before), Stand(c.After), L(c.Fells.Select(f => new MultiAreaFellRecord(f.AreaId, f.TreeId, f.Cause, f.Mechanism, f.Reason,
                f.EcologicalYear, f.WorkRevision, f.ExecutionKey, f.PlanId)))))));

    private static IReadOnlyDictionary<string, string> SpeciesMap(IReadOnlyList<Phase1TreeSpecies> list) =>
        list.ToDictionary(e => e.TreeId, e => e.SpeciesId, StringComparer.Ordinal);

    private static MultiAreaStage4State Stage4(Phase1Stage4State s) =>
        new(s.Pruning.ToDictionary(p => p.TreeId, p => new TreePruningState(p.Lifts, p.CrownBaseHeightM, p.LastPruningYear), StringComparer.Ordinal),
            L(s.PendingPrunes.Select(Prune)), L(s.ApprovedPrunes.Select(Prune)),
            L(s.PruneLog.Select(r => new MultiAreaPruneRecord(r.AreaId, r.TreeId, r.TargetCrownBaseHeightM, r.EcologicalYear, r.WorkRevision,
                r.ExecutionKey, r.PlanId, r.CrownRadiusBeforeM, r.CrownRadiusAfterM, r.LiftsAfter))),
            L(s.Deadwood.Select(d => new FallenDeadwoodRecord(d.DeadwoodId, d.TreeId, d.AreaId, d.SpeciesId, d.XM, d.ZM, d.DeathYear, d.Cause,
                d.DbhCm, d.HeightM, d.EstimatedVolumeM3))),
            s.NextDeadwoodNumber);

    private static MultiAreaPruneOrder Prune(Phase1PruneOrder o) => new(o.AreaId, o.TreeId, o.TargetCrownBaseHeightM);

    private static MultiAreaStage5State Stage5(Phase1Stage5State s, Phase1Stage5Options o) =>
        new(new Stage5Options { GridCellM = o.GridCellM, BackgroundBrowsePressure = o.BackgroundBrowsePressure, MastSeed = o.MastSeed },
            s.MastSeed,
            L(s.Areas.Select(a => new Stage5AreaCells(a.AreaId, L(a.Cells.Select(c => new Stage5Cell(c.Column, c.Row, c.CentreXM, c.CentreZM,
                c.ClippedAreaM2, c.Light, c.Bramble, c.Bracken, L(c.Bands.Select(b => new Stage5Band(b.Density, b.Height, b.EstablishYear,
                    b.OriginYear, b.LastBrowsedFraction, b.LastBrowseAssessmentYear))))))))),
            L(s.PendingClears.Select(Clear)), L(s.ApprovedClears.Select(Clear)),
            L(s.ClearLog.Select(r => new MultiAreaClearUndergrowthRecord(r.AreaId, r.Column, r.Row, r.EcologicalYear, r.WorkRevision,
                r.ExecutionKey, r.PlanId, r.BrambleBefore, r.BrackenBefore, r.BandsKept, r.AbundanceKept))),
            L(s.Accounts.Select(y => new Stage5YearAccount(y.YearBefore, y.YearAfter, y.MastRoll, y.MastLabel, y.MastMultiplier,
                L(y.Areas.Select(a => new Stage5AreaAccount(a.AreaId, a.Cells, a.SeedArrival, a.StartingAbundance, a.VegetationLoss,
                    a.LightLoss, a.BrowseLoss, a.EstablishmentRequested, a.EstablishmentAccepted, a.CapacityRejected, a.BandsCreated,
                    a.BandMerges, a.ThresholdCrossings, a.QualifyingCells, a.BlockCapDeferredCells, a.PromotionExported, a.EndingAbundance,
                    a.MeanRelativeOccupancy, a.MeanBramble, a.MeanBracken, a.MeanLight, L(a.Promotions.Select(Promotion)),
                    a.CohortBandsExported, a.BudgetWithheldBlocks, L(a.CellEstablishmentAccepted))))))),
            L(s.Blocks.Select(b => new Stage5BlockCredit(b.BlockX, b.BlockZ, b.InPolygonCells, b.Credit))));

    private static MultiAreaClearUndergrowthOrder Clear(Phase1ClearOrder o) => new(o.AreaId, o.Column, o.Row);

    private static Stage5PromotionRecord Promotion(Phase1PromotionRecord p) =>
        new(p.TreeId, p.AreaId, p.Column, p.Row, p.BlockX, p.BlockZ, p.EstablishYear, p.AgeYears, p.HeightM, p.DbhCm, p.CrownRadiusM,
            p.WoodlandCi, p.XM, p.ZM, p.CentreFallback, p.ExportedDensity, p.CohortBandsExported, p.CohortDensityExported);

    private static MultiAreaPhase2State Phase2(Phase1Phase2State s, Phase1Phase2Options o) =>
        new(new Phase2Options { OakMastSeed = o.OakMastSeed, BeechMastSeed = o.BeechMastSeed, GenesisSpecies = SpeciesMap(o.GenesisSpecies) },
            s.OakMastSeed, s.BeechMastSeed, SpeciesMap(s.TreeSpecies),
            L(s.Areas.Select(a => new Phase2AreaBands(a.AreaId, L(a.Cells.Select(c => new Phase2CellBands(c.Column, c.Row,
                L(c.Bands.Select(b => new Phase2Band(b.SpeciesId, b.Origin, b.Density, b.Height, b.EstablishYear, b.OriginYear,
                    b.LastBrowsedFraction, b.LastBrowseAssessmentYear))))))))),
            L(s.Juveniles.Select(j => new PlantedJuvenile(j.JuvenileId, j.AreaId, j.SpeciesId, j.XM, j.ZM, j.PlantingYear, j.AgeYears, j.HeightM,
                j.Alive, j.PromotedTreeId, j.DeathYear, j.DeathCause, j.LastYearBrowsed, j.LastBrowseProbability,
                j.LastVegetationLossProbability, j.LastAssessmentYear))),
            L(s.Shelters.Select(h => new BrowseShelter(h.ShelterId, h.JuvenileId, h.XM, h.ZM, h.InstalledYear, h.EffectiveYears, h.FailedYear))),
            L(s.Patches.Select(p => new SpotPatch(p.PatchId, p.AreaId, p.XM, p.ZM, p.RadiusM, p.CreatedYear, p.UpdatedYear, p.Bramble, p.Bracken))),
            L(s.PendingOrders.Select(Planting)), L(s.ApprovedOrders.Select(Planting)),
            L(s.WorkLog.Select(r => new Phase2WorkRecord(r.OrderId, r.Kind, r.AreaId, r.XM, r.ZM, r.SpeciesId, r.JuvenileId, r.ShelterId,
                r.PatchId, r.EcologicalYear, r.WorkRevision, r.ExecutionKey, r.PlanId))),
            s.NextOrderNumber, s.NextJuvenileNumber, s.NextPatchNumber,
            L(s.Accounts.Select(y => new Phase2YearAccount(y.YearBefore, y.YearAfter,
                L(y.Mast.Select(m => new Phase2MastRecord(m.SpeciesId, m.MastSeed, m.Roll, m.Label, m.Multiplier))),
                L(y.Natural.Select(a => new Phase2SpeciesAreaAccount(a.AreaId, a.SpeciesId, a.SeedArrival, a.StartingAbundance, a.VegetationLoss,
                    a.LightLoss, a.BrowseLoss, a.EstablishmentRequested, a.EstablishmentAccepted, a.CapacityRejected, a.BandsCreated,
                    a.BandMerges, a.ThresholdCrossings, a.QualifyingCells, a.BlockCapDeferredCells, a.PromotionExported, a.EndingAbundance,
                    a.CohortBandsExported, a.BudgetWithheldBlocks, L(a.Promotions.Select(Promotion))))),
                L(y.Planted.Select(p => new Phase2PlantedAccount(p.SpeciesId, p.Starting, p.VegetationDeaths, p.LightDeaths, p.BrowseDeaths,
                    p.Remaining, p.Promoted, L(p.PromotedTreeIds)))),
                L(y.Density.Select(d => new Phase2DensityRecord(d.AreaId, d.Mixed, d.Stems, d.SitkaSubset, d.SitkaHazardDeaths,
                    d.SitkaBoundaryDeaths, d.Occupancy, d.GenericResponse, d.NonReferenceShare, d.EligibleForGeneric, d.ExpectedGeneric,
                    d.GenericQuota, d.QuotaRemainder, d.QuotaRoll, d.GenericDeaths, d.BudgetPassedOver, d.SingleStemFallback,
                    d.StandBasalAreaM2, d.GenericBasalAreaM2, L(d.GenericVictims))))))));

    private static MultiAreaPlantingOrder Planting(Phase1PlantingOrder o) => o switch
    {
        Phase1PlantOrder p => new MultiAreaPlantOrder(p.OrderId, p.AreaId, p.SpeciesId, p.XM, p.ZM),
        Phase1ShelterOrder h => new MultiAreaShelterOrder(h.OrderId, h.AreaId, h.XM, h.ZM),
        Phase1SpotClearOrder c => new MultiAreaSpotClearOrder(c.OrderId, c.AreaId, c.XM, c.ZM),
        _ => throw new P1Refusal("Phase1RestoreInvalid", "unknown planting order type"),
    };

    // ------------------------------------------------------------------------------------------------ helpers

    private static IReadOnlyList<T> L<T>(IEnumerable<T> items) => Array.AsReadOnly(items.ToArray());
}
