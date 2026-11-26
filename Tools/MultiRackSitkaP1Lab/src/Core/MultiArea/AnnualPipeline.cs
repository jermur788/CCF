namespace SitkaP1.Core;

/// <summary>
/// One fixed compile-time step in the annual woodland advance pipeline (SPEC-multi-2d-s0-seam-extraction-V1 §6.2).
/// Steps read <see cref="YearInProgress.Start"/> (the immutable start-of-year root) and mutate only scratch accumulators
/// on <see cref="YearInProgress"/>. They never take <c>sync</c> and never assign <c>root</c>.
/// </summary>
internal interface IAnnualStep
{
    string Name { get; }
    void Run(YearInProgress year);
}

/// <summary>Read-only per-area metadata supplied by <see cref="MultiAreaContinuation.Advance"/> to the pipeline.</summary>
internal readonly record struct AnnualAreaSpec(string AreaId, int Seed, float AreaHa);

/// <summary>
/// Scratch accumulator for a single annual step, holding exactly the locals of <see cref="MultiAreaContinuation.Advance"/>
/// and nothing new (SPEC-multi-2d-s0-seam-extraction-V1 §6.2).
/// </summary>
internal sealed class YearInProgress
{
    internal YearInProgress(
        MultiAreaRoot start,
        bool hasWork,
        CrownModelVersion crownModel,
        Stage4Options? stage4Options,
        IReadOnlyList<AnnualAreaSpec> areas,
        Func<IReadOnlyList<IReadOnlyList<AnnualTree>>, Stage5AreaInput[]> buildStage5Inputs,
        Action<string>? afterArea)
    {
        Start = start;
        HasWork = hasWork;
        CrownModel = crownModel;
        Stage4Options = stage4Options;
        Areas = areas;
        BuildStage5Inputs = buildStage5Inputs;
        AfterArea = afterArea;

        Year = start.EcologicalYear;
        Revision = checked(start.AnnualRevision + 1);
        NextAreas = new MultiAreaAreaState[start.Areas.Count];
        Records = new MultiAreaAreaYear[start.Areas.Count];
        Reports = new MultiAreaAreaReport[start.Areas.Count];
        // CrownModel 1 only: the start-of-year connected woodland, shared read-only by every area's step.
        Woodland = crownModel == CrownModelVersion.LegacyFrozen
            ? null
            : SitkaP1.Core.CrownModel.Woodland(start.Areas.SelectMany(area => area.State.Trees));
        // Stage 4 only: deadwood records are numbered in area order, then in the order the rules recorded the deaths.
        Stage4 = start.Stage4;
        Deadwood = new List<FallenDeadwoodRecord>();
        NextDeadwood = Stage4?.NextDeadwoodNumber ?? 1;
        // Phase 2 only: each rack's density route (pure Sitka or two-part mixed) and the species of every adult.
        Phase2Start = start.Phase2;
        Density = new List<Phase2DensityRecord>();
    }

    internal MultiAreaRoot Start { get; }
    internal bool HasWork { get; }
    internal CrownModelVersion CrownModel { get; }
    internal Stage4Options? Stage4Options { get; }
    internal IReadOnlyList<AnnualAreaSpec> Areas { get; }
    internal Func<IReadOnlyList<IReadOnlyList<AnnualTree>>, Stage5AreaInput[]> BuildStage5Inputs { get; }
    internal Action<string>? AfterArea { get; }

    internal int Year { get; }
    internal int Revision { get; }
    internal MultiAreaAreaState[] NextAreas { get; }
    internal MultiAreaAreaYear[] Records { get; }
    internal MultiAreaAreaReport[] Reports { get; }
    internal bool LostCrop { get; set; }
    internal AnnualTree[]? Woodland { get; }
    internal MultiAreaStage4State? Stage4 { get; }
    internal List<FallenDeadwoodRecord> Deadwood { get; }
    internal int NextDeadwood { get; set; }
    internal MultiAreaPhase2State? Phase2Start { get; }
    internal List<Phase2DensityRecord> Density { get; }
    internal MultiAreaYearRecord? YearRecord { get; set; }
    internal MultiAreaStage5State? NextStage5 { get; set; }
    internal MultiAreaPhase2State? NextPhase2 { get; set; }
}

/// <summary>
/// Integrator-owned fixed compile-time annual step list (SPEC-multi-2d-s0-seam-extraction-V1 §6.2).
/// No reflection, dynamic loading, DI container, service locator, or runtime registration API exists.
/// </summary>
internal static class AnnualPipeline
{
    internal static readonly IAnnualStep[] Steps =
    {
        AdultGrowthStep.Instance,
        ReservedPhase3DisturbanceStep.Instance,
        DeadwoodStep.Instance,
        RegenerationStep.Instance,
        YearRecordStep.Instance,
        ReservedPhase2BYearEndSummaryStep.Instance,
    };
}

/// <summary>
/// Step 1: per-area adult growth, density mortality, Crop-mark pruning, and the test-only <c>afterArea</c> seam.
/// Stage 4 deadwood capture is hoisted to <see cref="DeadwoodStep"/> (permitted relocation M1).
/// </summary>
internal sealed class AdultGrowthStep : IAnnualStep
{
    internal static readonly AdultGrowthStep Instance = new();
    private AdultGrowthStep() { }

    public string Name => "AdultGrowth";

    public void Run(YearInProgress progress)
    {
        var start = progress.Start;
        var stage4 = progress.Stage4;
        var phase2Start = progress.Phase2Start;
        for (int i = 0; i < start.Areas.Count; i++)
        {
            var area = start.Areas[i];
            var info = progress.Areas[i];
            AnnualState next;
            if (area.State.Trees.Any(tree => tree.IsLiving))
            {
                AnnualReport report;
                AnnualState stepped;
                if (phase2Start is not null)
                {
                    var (mixedReport, mixedNext, record) = Phase2MixedDensity.StepRack(area.AreaId, area.State, info.Seed, info.AreaHa,
                        progress.Woodland!, treeId => stage4!.PruningOf(treeId).Lifts, progress.Stage4Options!, phase2Start.BroadleafOf);
                    (report, stepped) = (mixedReport, mixedNext);
                    progress.Density.Add(record);
                }
                else
                    (report, stepped) = stage4 is not null
                        ? MultiAreaAnnual.StepStage4(area.State, info.Seed, info.AreaHa, progress.Woodland!,
                            treeId => stage4.PruningOf(treeId).Lifts, progress.Stage4Options!)
                        : progress.CrownModel == CrownModelVersion.LegacyFrozen
                            ? MultiAreaAnnual.Step(area.State, info.Seed, info.AreaHa)
                            : MultiAreaAnnual.Step(area.State, info.Seed, info.AreaHa, progress.CrownModel, progress.Woodland);
                next = stepped;
                progress.Records[i] = new MultiAreaAreaYear(area.AreaId, report.Before, report.After,
                    Array.AsReadOnly(report.Deaths.ToArray()), NoLivingStems: false);
                progress.Reports[i] = new MultiAreaAreaReport(area.AreaId, report);
            }
            else
            {
                // Naturally empty (owner D-5): the engine is not run for this area; its dead roster is carried forward.
                next = new AnnualState(checked(progress.Year + 1), area.State.Trees);
                var empty = WorkRules.Summarize(area.State.Trees, info.AreaHa);
                progress.Records[i] = new MultiAreaAreaYear(area.AreaId, empty, empty, Array.Empty<AnnualDeath>(), NoLivingStems: true);
                progress.Reports[i] = new MultiAreaAreaReport(area.AreaId, null);
            }
            // Crop marks are plan state: a mark on a tree that died this year is cleared in the same publish.
            var livingIds = next.Trees.Where(tree => tree.IsLiving).Select(tree => tree.TreeId).ToHashSet(StringComparer.Ordinal);
            var crops = area.CropTreeIds.Where(livingIds.Contains).ToArray();
            progress.LostCrop |= crops.Length != area.CropTreeIds.Count;
            progress.NextAreas[i] = new MultiAreaAreaState(area.AreaId, next, Array.AsReadOnly(crops));
            progress.AfterArea?.Invoke(area.AreaId);
        }
    }
}

/// <summary>
/// Step 2 (RESERVED, EMPTY in S0b): provisional slot owned by the Phase 3 disturbance spec.
/// Provably a true no-op: computes nothing and touches no field.
/// </summary>
internal sealed class ReservedPhase3DisturbanceStep : IAnnualStep
{
    internal static readonly ReservedPhase3DisturbanceStep Instance = new();
    private ReservedPhase3DisturbanceStep() { }

    public string Name => "ReservedPhase3Disturbance";

    public void Run(YearInProgress year) { }
}

/// <summary>
/// Step 3 (permitted relocation M1 — deadwood hoist): records Stage 4 fallen-deadwood entries in exact area index order
/// and death report order using each area's post-adult-step roster and <see cref="YearInProgress.NextDeadwood"/>.
/// </summary>
internal sealed class DeadwoodStep : IAnnualStep
{
    internal static readonly DeadwoodStep Instance = new();
    private DeadwoodStep() { }

    public string Name => "Deadwood";

    public void Run(YearInProgress progress)
    {
        if (progress.Stage4 is null) return;
        var phase2Start = progress.Phase2Start;
        for (int i = 0; i < progress.Start.Areas.Count; i++)
        {
            var report = progress.Reports[i].Report;
            if (report is null) continue;
            var area = progress.Start.Areas[i];
            var stepped = progress.NextAreas[i].State;
            var byId = stepped.Trees.ToDictionary(tree => tree.TreeId, StringComparer.Ordinal);
            foreach (var death in report.Deaths)
            {
                var dead = byId[death.TreeId];
                // Every density death carries its species (Phase 2); the volume form is the same for all three.
                progress.Deadwood.Add(new FallenDeadwoodRecord(FallenDeadwoodRecord.IdFor(progress.NextDeadwood++), dead.TreeId,
                    area.AreaId, phase2Start?.SpeciesIdOf(dead.TreeId) ?? SpeciesProfile.Sitka.SpeciesId, dead.XM, dead.ZM,
                    death.EcologicalYear, death.Cause, dead.DbhCm, dead.HeightM,
                    FallenDeadwoodRecord.StemVolumeM3(dead.DbhCm, dead.HeightM)));
            }
        }
    }
}

/// <summary>
/// Step 4: Stage 5 / Phase 2 regeneration over the start-of-year regeneration state and post-adult-step woodland,
/// merging promoted adults into rosters in ordinal <c>TreeId</c> order and clearing cancelled pending/approved orders.
/// </summary>
internal sealed class RegenerationStep : IAnnualStep
{
    internal static readonly RegenerationStep Instance = new();
    private RegenerationStep() { }

    public string Name => "Regeneration";

    public void Run(YearInProgress progress)
    {
        var start = progress.Start;
        var phase2Start = progress.Phase2Start;
        var nextAreas = progress.NextAreas;
        bool hasWork = progress.HasWork;
        int year = progress.Year;

        // Stage 5 only (spec §3 steps 5a–12): after every area's adult step (an adult-empty area included), one pure step
        // over the start-of-year Stage 5 state and the post-adult-step woodland. Promoted adults join their area's roster
        // in ordinal TreeId order; consented cancellation clears pending and approved ClearUndergrowth orders too.
        if (start.Stage5 is { } phase2Stage5 && phase2Start is not null)
        {
            // Phase 2 (spec §4 steps 3d–5): all three species' regeneration and the planted juveniles in one pure step.
            var stepped = Phase2Annual.Step(phase2Stage5, phase2Start,
                progress.BuildStage5Inputs(nextAreas.Select(area => area.State.Trees).ToArray()),
                checked(year + 1), progress.Density);
            for (int i = 0; i < nextAreas.Length; i++)
            {
                var promoted = stepped.PromotedTrees[nextAreas[i].AreaId];
                if (promoted.Count == 0) continue;
                var roster = nextAreas[i].State.Trees.Concat(promoted).OrderBy(tree => tree.TreeId, StringComparer.Ordinal).ToArray();
                nextAreas[i] = nextAreas[i] with { State = new AnnualState(checked(year + 1), Array.AsReadOnly(roster)) };
            }
            progress.NextStage5 = stepped.NextStage5 with
            {
                PendingClears = hasWork ? Array.Empty<MultiAreaClearUndergrowthOrder>() : stepped.NextStage5.PendingClears,
                ApprovedClears = hasWork ? Array.Empty<MultiAreaClearUndergrowthOrder>() : stepped.NextStage5.ApprovedClears,
            };
            progress.NextPhase2 = stepped.NextPhase2 with
            {
                PendingOrders = hasWork ? Array.Empty<MultiAreaPlantingOrder>() : stepped.NextPhase2.PendingOrders,
                ApprovedOrders = hasWork ? Array.Empty<MultiAreaPlantingOrder>() : stepped.NextPhase2.ApprovedOrders,
            };
        }
        else if (start.Stage5 is { } stage5Start)
        {
            var stepped = Stage5Annual.Step(stage5Start,
                progress.BuildStage5Inputs(nextAreas.Select(area => area.State.Trees).ToArray()),
                checked(year + 1));
            for (int i = 0; i < nextAreas.Length; i++)
            {
                var promoted = stepped.PromotedTrees[nextAreas[i].AreaId];
                if (promoted.Count == 0) continue;
                var roster = nextAreas[i].State.Trees.Concat(promoted).OrderBy(tree => tree.TreeId, StringComparer.Ordinal).ToArray();
                nextAreas[i] = nextAreas[i] with { State = new AnnualState(checked(year + 1), Array.AsReadOnly(roster)) };
            }
            progress.NextStage5 = stepped.Next with
            {
                PendingClears = hasWork ? Array.Empty<MultiAreaClearUndergrowthOrder>() : stepped.Next.PendingClears,
                ApprovedClears = hasWork ? Array.Empty<MultiAreaClearUndergrowthOrder>() : stepped.Next.ApprovedClears,
            };
        }
    }
}

/// <summary>
/// Step 5 (permitted relocation M2 — year-record placement): constructs <see cref="MultiAreaYearRecord"/> from the
/// adult-step rows (<see cref="YearInProgress.Records"/>), which are unmodified by regeneration.
/// </summary>
internal sealed class YearRecordStep : IAnnualStep
{
    internal static readonly YearRecordStep Instance = new();
    private YearRecordStep() { }

    public string Name => "YearRecord";

    public void Run(YearInProgress progress)
    {
        progress.YearRecord = new MultiAreaYearRecord(progress.Year, checked(progress.Year + 1), progress.Revision,
            progress.Start.WorkRevision, Array.AsReadOnly(progress.Records), progress.HasWork);
    }
}

/// <summary>
/// Step 6 (RESERVED, EMPTY in S0b): read-only year-end summary slot reserved for Phase 2B Economy.
/// Provably a true no-op: computes nothing and touches no field.
/// </summary>
internal sealed class ReservedPhase2BYearEndSummaryStep : IAnnualStep
{
    internal static readonly ReservedPhase2BYearEndSummaryStep Instance = new();
    private ReservedPhase2BYearEndSummaryStep() { }

    public string Name => "ReservedPhase2BYearEndSummary";

    public void Run(YearInProgress year) { }
}
