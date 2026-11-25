namespace SitkaP1.Core;

using SitkaP1.Core.Phase1;

/// <summary>One area's committed state inside the root: its roster (with the shared ecological year) and its Crop marks.</summary>
internal sealed record MultiAreaAreaState(string AreaId, AnnualState State, IReadOnlyList<string> CropTreeIds);

/// <summary>An executed key, kept so an identical retry is answered with its receipt instead of another Fell.</summary>
internal sealed record MultiAreaExecutedWork(MultiAreaSourceToken Source, IReadOnlyList<MultiAreaWorkOrder> Orders,
    MultiAreaWorkEvent Event);

/// <summary>
/// The ONE immutable commit root of the whole experiment (MA-1 spec §3). Every area's roster and Crop marks, the shared
/// year, both clocks, the work revision, the plan-state revision, the pending orders, the approved plan, the event ledger,
/// the executed-key map and the annual history are published together by a single reference assignment, so no reader
/// can see area A changed while area B is not.
/// </summary>
internal sealed record MultiAreaRoot(
    int EcologicalYear,
    int AnnualRevision,
    int WorkRevision,
    int PlanStateRevision,
    IReadOnlyList<MultiAreaAreaState> Areas,
    IReadOnlyList<MultiAreaWorkEvent> Events,
    IReadOnlyDictionary<string, MultiAreaExecutedWork> Executed,
    IReadOnlyList<MultiAreaWorkOrder> Pending,
    MultiAreaApprovedPlan? Approved,
    IReadOnlyList<MultiAreaYearRecord> AnnualHistory,
    MultiAreaStage4State? Stage4 = null, // Stage 4 sessions only (D-095); null on every model 0 / Stage 1–3 root
    MultiAreaStage5State? Stage5 = null, // Stage 5 (1/1/1/1) sessions only; null on every Stage 1–4 root
    MultiAreaPhase2State? Phase2 = null) // Phase 2 (1/1/1/1/1) sessions only; null whenever Species is 0
{
    /// <summary>True when any Fell, Prune, ClearUndergrowth or planting order is pending or approved and not yet executed.</summary>
    internal bool HasUnexecutedWork => Pending.Count > 0 || Approved is not null || (Stage4?.PendingPrunes.Count ?? 0) > 0 ||
        (Stage5?.PendingClears.Count ?? 0) > 0 || (Phase2?.PendingOrders.Count ?? 0) > 0;
}

/// <summary>The per-area annual step: the unchanged AnnualRules.Calculate and the same next-state construction as AnnualContinuation.</summary>
internal static class MultiAreaAnnual
{
    internal static (AnnualReport Report, AnnualState Next) Step(AnnualState state, int simulationSeed, float areaHa) =>
        Step(state, simulationSeed, areaHa, CrownModelVersion.LegacyFrozen, woodland: null);

    /// <summary>
    /// The same step with an explicit crown model. For CrownModel 1 <paramref name="woodland"/> is the connected-woodland
    /// neighbourhood read from the start-of-year root (never from another area's freshly stepped state), so area order
    /// cannot change any result and no area mutates another.
    /// </summary>
    internal static (AnnualReport Report, AnnualState Next) Step(AnnualState state, int simulationSeed, float areaHa,
        CrownModelVersion crownModel, AnnualTree[]? woodland)
    {
        var report = AnnualRules.Calculate(state, simulationSeed, areaHa, crownModel, woodland);
        var next = new AnnualState(report.YearAfter, Array.AsReadOnly(report.Trees.Select(row => row.After).ToArray()));
        return (report, next);
    }

    /// <summary>The Stage 4 step of one rack: woodland-driven growth, this rack's own density mortality, same next-state rule.</summary>
    internal static (AnnualReport Report, AnnualState Next) StepStage4(AnnualState state, int simulationSeed, float areaHa,
        AnnualTree[] woodland, Func<string, int> pruningLiftsOf, Stage4Options options)
    {
        var report = AnnualRules.CalculateStage4(state, simulationSeed, areaHa, woodland, pruningLiftsOf, options);
        var next = new AnnualState(report.YearAfter, Array.AsReadOnly(report.Trees.Select(row => row.After).ToArray()));
        return (report, next);
    }
}

/// <summary>
/// In-memory, no-durability MA-1 session over several isolated areas (owner-approved MA-1 spec §2–§4): one shared
/// ecological year in which each area runs the unchanged annual rules on its own trees, area and seed; one manual
/// cross-area Work Plan (approve fells nothing; a separate Execute commits every area once or refuses all); and the
/// owner's safeguards. Every read and command takes <c>sync</c> exactly once, so the fence it checks, the transition it
/// performs and the view it returns all come from one root. This is NOT two composed AnnualContinuation sessions.
/// </summary>
internal sealed partial class MultiAreaContinuation
{
    private sealed record AreaInfo(string AreaId, string Label, string StockingNote, int Seed, float AreaHa,
        string AreaM2Exact, IReadOnlyList<MultiAreaVertex> Polygon, PlayerMapExtent Extent);

    private readonly object sync = new();
    private readonly CrownModelVersion crownModel;
    private readonly MultiAreaModelSelection models;
    private readonly Stage4Options? stage4Options; // non-null only for the full Stage 4 selection
    private readonly MultiAreaExperiment definition; // immutable genesis inputs; a save's identity comes from here
    private readonly string areaModelVersion;
    private readonly IReadOnlyList<AreaInfo> areas; // ordinal by AreaId, the same order as root.Areas
    private readonly MultiAreaExperimentInfo experiment;
    private readonly string experimentBadge;
    private readonly string areaIds;
    // One random identity per in-memory session. It is never part of a token, report or manifest. A Load mints a NEW one,
    // so every fence a page held against the replaced session is refused from then on (Execute replay included).
    private string sessionId = NewSessionId();
    private MultiAreaRoot root;
    // Session-only canopy/light read-outs (#900), one per light grid, valid for one committed tree state. Tree state changes
    // only on Execute (WorkRevision) or an annual step (AnnualRevision: natural death and crown growth), so Pending, Approve,
    // Crop marks and reads never recompute or change them. Never saved and never part of a token.
    private readonly Dictionary<double, (int AnnualRevision, int WorkRevision, CanopyLightReport Report)> canopyCache = new();
    private int canopyRecomputeCount;
    // The /multi ground cells (per-cell shape, cover fraction and contributing trees) for the cached 2.5 m report. Same
    // validity rule as canopyCache; session-only.
    private (int AnnualRevision, int WorkRevision, IReadOnlyList<IReadOnlyList<MultiAreaGroundCell>> Ground)? groundCache;

    internal MultiAreaContinuation(MultiAreaExperiment definition) : this(definition, CrownModelVersion.LegacyFrozen)
    {
    }

    /// <summary>
    /// Start a session with an explicit crown model. Model 0 is exactly the frozen MA-1 path. Model 1 (C2) derives each
    /// living tree's year-0 radius as r_0 = r_target(CI_0) over the connected genesis woodland; the experiment manifest, its
    /// content hash and every other genesis value are unchanged (a derived session path, not a new fixture).
    /// </summary>
    internal MultiAreaContinuation(MultiAreaExperiment definition, CrownModelVersion crownModel)
        : this(definition, new MultiAreaModelSelection(AdultModelVersion.Legacy, MortalityModelVersion.Legacy, crownModel))
    {
    }

    /// <summary>
    /// Start a session with explicit adult, mortality, crown and regeneration models (Stage 4, D-095; Stage 5). Only the
    /// frozen selection, the Stage 1–3 crown selection, the full Stage 4 selection and Stage 4 + regeneration 1 are accepted.
    /// <paramref name="stage4"/> carries the neutral site-productivity field and the species profile; it must be null unless
    /// the selection is Stage 4. <paramref name="stage5"/> carries the Stage 5 grid, browse pressure and mast seed; it must be
    /// null unless the selection is 1/1/1/1.
    /// </summary>
    internal MultiAreaContinuation(MultiAreaExperiment definition, MultiAreaModelSelection models, Stage4Options? stage4 = null,
        Stage5Options? stage5 = null, Phase2Options? phase2 = null)
    {
        ArgumentNullException.ThrowIfNull(definition);
        ArgumentNullException.ThrowIfNull(models);
        models.RequireSupported();
        if (stage4 is not null && !models.IsStage4)
            throw new P1Refusal("Stage4ModelRequired", "Stage 4 options need the full Stage 4 model selection");
        if (stage5 is not null && !models.IsStage5)
            throw new P1Refusal("Stage5ModelRequired", "Stage 5 options need the 1/1/1/1 model selection");
        if (phase2 is not null && !models.IsPhase2)
            throw new P1Refusal("Phase2ModelRequired", "Phase 2 options need the 1/1/1/1/1 model selection");
        var crownModel = models.Crown;
        definition.Validate();
        this.definition = definition;
        this.models = models;
        this.stage4Options = models.IsStage4 ? (stage4 ?? Stage4Options.Default).RequireValid() : null;
        this.crownModel = crownModel;
        bool connected = crownModel != CrownModelVersion.LegacyFrozen;
        // Phase 2 appends its species version ONLY when it is not 0 (owner Q1), so every earlier token is unchanged.
        areaModelVersion = Phase2Identity.AreaModelVersion(Stage5Identity.AreaModelVersion(models.IsStage4 ? MultiAreaIdentity.AreaModelVersionStage4
            : connected ? MultiAreaIdentity.AreaModelVersionConnectedCrownC2 : MultiAreaIdentity.AreaModelVersion, models.Regeneration),
            models.Species);
        string contentHash = definition.ContentHash();
        experiment = new MultiAreaExperimentInfo(definition.ExperimentId, MultiAreaIdentity.Format, MultiAreaIdentity.SchemaVersion,
            MultiAreaIdentity.Domain, MultiAreaIdentity.SourceClass, contentHash, definition.GeneratorId,
            definition.GeneratorVersion, definition.GeneratorSeed, MultiAreaIdentity.ReviewStatus,
            definition.DisclosureOverride ?? MultiAreaIdentity.Disclosure,
            models.IsStage4 ? MultiAreaIdentity.EdgeModelStage4
                : connected ? MultiAreaIdentity.EdgeModelConnectedCrownC2 : MultiAreaIdentity.EdgeModel,
            models.IsStage4 ? MultiAreaIdentity.SiteNoteStage4 : MultiAreaIdentity.SiteNote);
        experimentBadge = string.Equals(definition.ExperimentId, MultiAreaDemoExperiments.Ma1MixedExperimentId, StringComparison.Ordinal)
            ? MultiAreaDemoExperiments.Ma1MixedBadge
            : MultiAreaDemoExperiments.Ma1Badge;
        areas = Array.AsReadOnly(definition.Areas.Select(InfoOf).ToArray());
        areaIds = string.Join(",", definition.Areas.Select(area => area.AreaId));
        var genesis = definition.Areas.Select(MultiAreaExperiment.GenesisTrees).ToArray();
        if (connected)
        {
            var woodland = CrownModel.Woodland(genesis.SelectMany(trees => trees));
            genesis = genesis.Select(trees => CrownModel.Equilibrate(SpeciesProfile.Sitka, trees, woodland)).ToArray();
            // Phase 2 only: a genesis Oak or Beech takes its own species' C2 equilibrium r_0 = r_target(CI_0). CI_0 reads
            // positions and DBH only, so the same genesis woodland serves every species.
            if (models.IsPhase2)
                genesis = genesis.Select(trees => Phase2GenesisCrowns(trees, woodland, (phase2 ?? Phase2Options.Default).RequireValid()))
                    .ToArray();
        }
        var states = definition.Areas.Select((area, i) => new MultiAreaAreaState(area.AreaId,
            new AnnualState(0, genesis[i]), Array.Empty<string>())).ToArray();
        // Stage 5 only: year-0 ground cells and undergrowth from the genesis woodland's light. The woodland mast seed is the
        // experiment's generator seed unless the options name one.
        var stage5State = models.IsStage5
            ? Stage5Annual.Genesis((stage5 ?? Stage5Options.Default).RequireValid(),
                (stage5 ?? Stage5Options.Default).MastSeed ?? unchecked((int)definition.GeneratorSeed),
                Stage5Inputs(states.Select(state => state.State.Trees).ToArray()))
            : null;
        MultiAreaPhase2State? phase2State = null;
        if (models.IsPhase2)
        {
            var options = (phase2 ?? Phase2Options.Default).RequireValid();
            var known = genesis.SelectMany(trees => trees).Select(tree => tree.TreeId).ToHashSet(StringComparer.Ordinal);
            if (options.GenesisSpecies.Keys.Any(id => !known.Contains(id)))
                throw new P1Refusal("Phase2GenesisSpeciesInvalid", "genesis species name a tree that is not in the experiment");
            phase2State = Phase2Annual.Genesis(options, stage5State!.MastSeed, stage5State, options.GenesisSpecies);
        }
        root = new MultiAreaRoot(0, 0, 0, 0, Array.AsReadOnly(states), Array.Empty<MultiAreaWorkEvent>(),
            new Dictionary<string, MultiAreaExecutedWork>(StringComparer.Ordinal), Array.Empty<MultiAreaWorkOrder>(), null,
            Array.Empty<MultiAreaYearRecord>(), models.IsStage4 ? MultiAreaStage4State.Empty : null, stage5State, phase2State);
    }

    // Phase 2 genesis crowns: every living Oak/Beech of one area gets its species' C2 equilibrium; Sitka is returned unchanged.
    private static IReadOnlyList<AnnualTree> Phase2GenesisCrowns(IReadOnlyList<AnnualTree> trees, AnnualTree[] woodland, Phase2Options options) =>
        Array.AsReadOnly(trees.Select(tree =>
            tree.IsLiving && options.GenesisSpecies.TryGetValue(tree.TreeId, out var id) && Phase2Species.BroadleafOrNull(id) is { } species
                ? tree with { CrownRadiusM = Phase2Crown.TargetRadiusM(species, tree.DbhCm, CrownModel.WoodlandCompetition(tree, woodland).Ci) }
                : tree).ToArray());

    internal string SessionId => sessionId;

    internal CrownModelVersion SelectedCrownModel => crownModel;

    internal MultiAreaModelSelection SelectedModels => models;

    /// <summary>The whole view, built from ONE root reference under ONE lock acquisition.</summary>
    internal MultiAreaSessionView ReadView()
    {
        lock (sync) return ViewOf(root);
    }

    /// <summary>
    /// The Stage 4 read-out (life states, pruning, per-rack RD, natural deaths and deadwood) of ONE root under ONE lock.
    /// It is a separate read so the existing session view keeps its exact shape; it changes nothing. On a model 0 or
    /// Stage 1–3 session it still reports life states and RD, with no pruning or deadwood (those exist only in Stage 4).
    /// </summary>
    internal MultiAreaStage4View ReadStage4View()
    {
        lock (sync) return Stage4ViewOf(root);
    }

    /// <summary>
    /// The Stage 5 read-out (ground cells, undergrowth, regeneration bands, clear orders and annual accounts) of ONE root under
    /// ONE lock. Refused on a session that is not 1/1/1/1. It changes nothing; every list in it is already immutable.
    /// </summary>
    internal MultiAreaStage5View ReadStage5View()
    {
        lock (sync)
        {
            var stage5 = RequireStage5();
            return new MultiAreaStage5View(sessionId, root.EcologicalYear, root.PlanStateRevision, Token(root), models, stage5.Options,
                stage5.MastSeed, Stage5Evidence.All, stage5.Areas, stage5.PendingClears, stage5.ApprovedClears, stage5.ClearLog,
                stage5.Accounts, stage5.Blocks);
        }
    }

    /// <summary>
    /// The Phase 2 read-out (broadleaf bands, species of every broadleaf adult, planted juveniles, shelters, spot patches,
    /// planting orders and accounts) of ONE root under ONE lock. Refused on a session that is not 1/1/1/1/1. Changes nothing.
    /// </summary>
    internal MultiAreaPhase2View ReadPhase2View()
    {
        lock (sync)
        {
            var phase2 = RequirePhase2();
            return new MultiAreaPhase2View(sessionId, root.EcologicalYear, root.PlanStateRevision, Token(root), models, phase2.Options,
                phase2.OakMastSeed, phase2.BeechMastSeed, Phase2Evidence.All,
                new Dictionary<string, string>(phase2.TreeSpecies, StringComparer.Ordinal), phase2.Areas, phase2.Juveniles, phase2.Shelters,
                phase2.Patches, phase2.PendingOrders, phase2.ApprovedOrders, phase2.WorkLog, phase2.Accounts);
        }
    }

    /// <summary>
    /// The /multi page read: the session view and its full UI read-out (Stage 4 states, pruning eligibility, canopy summary,
    /// deadwood and the 2.5 m ground cells), both from ONE root under ONE lock. Changes nothing.
    /// </summary>
    internal MultiAreaUiSnapshot ReadUiSnapshot()
    {
        lock (sync)
        {
            var view = ViewOf(root);
            return new MultiAreaUiSnapshot(view, ReadoutOf(view));
        }
    }

    /// <summary>
    /// The UI read-out of the CURRENT root, but only when it is still the root named by <paramref name="source"/> and
    /// <paramref name="planStateRevision"/> (the root a command's view came from); null when anything moved since, so a
    /// read-out is never paired with a view of another root.
    /// </summary>
    internal MultiAreaUiReadout? ReadUiReadout(MultiAreaSourceToken source, int planStateRevision)
    {
        lock (sync)
        {
            if (source != Token(root) || planStateRevision != root.PlanStateRevision) return null;
            return ReadoutOf(ViewOf(root));
        }
    }

    /// <summary>
    /// Pass 1 atomic composite snapshot: <c>View</c>, <c>Stage4</c>, <c>Stage5</c>, <c>Phase2</c>, the Stage 4 UI read-out
    /// and the compact additive <c>Ecology</c> DTO all built from ONE committed root under ONE lock acquisition, sharing
    /// one exact <c>SessionId</c>, <c>Source</c> and <c>PlanStateRevision</c>. Read-only; zero state mutation.
    /// </summary>
    internal MultiAreaCompositeUiSnapshot ReadCompositeUiSnapshot()
    {
        lock (sync) return CompositeSnapshotOf(root);
    }

    /// <summary>
    /// Paired Stage 4 + Ecology UI readouts of the CURRENT root only when it still matches <paramref name="source"/> and
    /// <paramref name="planStateRevision"/>; null as a whole if anything moved concurrently (zero torn frames).
    /// </summary>
    internal MultiAreaCompositeUiReadout? ReadCompositeUiFor(MultiAreaSourceToken source, int planStateRevision)
    {
        lock (sync)
        {
            if (source != Token(root) || planStateRevision != root.PlanStateRevision) return null;
            var snap = CompositeSnapshotOf(root);
            return new MultiAreaCompositeUiReadout(snap.Readout, snap.Ecology);
        }
    }

    /// <summary>
    /// Strictly read-only cell detail inspection for <c>POST api/multi/cell</c>. Checks fence, area and in-polygon cell
    /// under ONE lock acquisition and returns full cell cohorts/undergrowth/light plus a fresh composite snapshot.
    /// </summary>
    internal MultiAreaCellDetailResult ReadCellDetail(MultiAreaFence fence, string areaId, int column, int row)
    {
        lock (sync)
        {
            RequireFence(fence);
            int areaIdx = AreaIndex(areaId);
            var snap = CompositeSnapshotOf(root);
            var ground = snap.Readout.Areas[areaIdx].Ground;
            int cellIdx = -1;
            for (int c = 0; c < ground.Count; c++)
            {
                if (ground[c].Column == column && ground[c].Row == row) { cellIdx = c; break; }
            }
            if (cellIdx < 0)
            {
                for (int other = 0; other < snap.Readout.Areas.Count; other++)
                {
                    if (other == areaIdx) continue;
                    if (snap.Readout.Areas[other].Ground.Any(cell => cell.Column == column && cell.Row == row))
                        throw new P1Refusal("MultiAreaWrongArea",
                            FormattableString.Invariant($"ground cell ({column}, {row}) belongs to {snap.Readout.Areas[other].AreaId}, not {areaId}"));
                }
                throw new P1Refusal("CellNotInArea", "no in-polygon ground cell of this area has that column and row");
            }
            var gCell = ground[cellIdx];
            float? bramble = null, bracken = null, browsePressure = null;
            var cohorts = new List<MultiAreaCellCohort>();
            bool hasPendingClear = false, hasApprovedClear = false;
            if (root.Stage5 is { } s5)
            {
                var s5Cell = s5.Areas[areaIdx].Cells.FirstOrDefault(c => c.Column == column && c.Row == row);
                if (s5Cell is not null)
                {
                    bramble = s5Cell.Bramble;
                    bracken = s5Cell.Bracken;
                    browsePressure = s5.Options.BackgroundBrowsePressure;
                    foreach (var b in s5Cell.Bands)
                    {
                        if (b.Density <= 0f) continue;
                        cohorts.Add(new MultiAreaCellCohort(Phase2Species.SitkaId, Phase2Band.Natural,
                            b.EstablishYear, b.OriginYear, b.Height, b.Density, b.LastBrowsedFraction, b.LastBrowseAssessmentYear));
                    }
                }
                hasPendingClear = s5.PendingClears.Any(o => o.AreaId == areaId && o.Column == column && o.Row == row);
                hasApprovedClear = s5.ApprovedClears.Any(o => o.AreaId == areaId && o.Column == column && o.Row == row);
            }
            if (root.Phase2 is { } p2)
            {
                var p2Cell = p2.Areas[areaIdx].Cells.FirstOrDefault(c => c.Column == column && c.Row == row);
                if (p2Cell is not null)
                {
                    foreach (var b in p2Cell.Bands)
                    {
                        if (b.Density <= 0f) continue;
                        cohorts.Add(new MultiAreaCellCohort(b.SpeciesId, b.Origin,
                            b.EstablishYear, b.OriginYear, b.Height, b.Density, b.LastBrowsedFraction, b.LastBrowseAssessmentYear));
                    }
                }
            }
            cohorts.Sort((a, b) =>
            {
                int cmp = string.CompareOrdinal(a.Species, b.Species);
                if (cmp != 0) return cmp;
                cmp = a.EstablishYear.CompareTo(b.EstablishYear);
                return cmp != 0 ? cmp : a.OriginYear.CompareTo(b.OriginYear);
            });
            string clearState = hasPendingClear ? "PendingClear" : hasApprovedClear ? "ApprovedClear" : "None";
            var detail = new MultiAreaCellDetail(
                areaId, gCell.Column, gCell.Row, gCell.CentreXM, gCell.CentreZM, gCell.CentreU, gCell.CentreV,
                gCell.ClippedAreaM2, gCell.Polygon, gCell.LightIndex, gCell.GeometricCoveredFraction,
                gCell.CrossBorderTrees, gCell.ContributingTreeIds,
                bramble, bracken, browsePressure, cohorts.AsReadOnly(), clearState, hasPendingClear, hasApprovedClear);
            return new MultiAreaCellDetailResult(snap, detail, null, null);
        }
    }

    // Caller holds sync. Builds the full composite snapshot of ONE root without reacquiring any lock.
    private MultiAreaCompositeUiSnapshot CompositeSnapshotOf(MultiAreaRoot current)
    {
        var view = ViewOf(current);
        var s4View = Stage4ViewOf(current);
        var canopy = ReadCanopyLight(CanopyLightEvaluator.DefaultLightCellM);
        var ground = EnsureGroundCache(view, canopy);
        var readout = MultiAreaUiReadouts.Build(view, s4View, canopy, models, ground, current.Phase2);
        var s5View = current.Stage5 is { } s5
            ? new MultiAreaStage5View(sessionId, current.EcologicalYear, current.PlanStateRevision, Token(current), models,
                s5.Options, s5.MastSeed, Stage5Evidence.All, s5.Areas, s5.PendingClears, s5.ApprovedClears, s5.ClearLog,
                s5.Accounts, s5.Blocks)
            : null;
        var p2View = current.Phase2 is { } p2
            ? new MultiAreaPhase2View(sessionId, current.EcologicalYear, current.PlanStateRevision, Token(current), models,
                p2.Options, p2.OakMastSeed, p2.BeechMastSeed, Phase2Evidence.All,
                new Dictionary<string, string>(p2.TreeSpecies, StringComparer.Ordinal), p2.Areas, p2.Juveniles, p2.Shelters,
                p2.Patches, p2.PendingOrders, p2.ApprovedOrders, p2.WorkLog, p2.Accounts)
            : null;
        var ecology = MultiAreaEcologyBuilder.Build(view, s4View, current.Stage5, current.Phase2, canopy, models, experimentBadge);
        return new MultiAreaCompositeUiSnapshot(view, readout, ecology, s4View, s5View, p2View);
    }

    // Caller holds sync. The canopy report and the ground cells are cached per committed tree state (AnnualRevision and
    // WorkRevision), like ReadCanopyLight: Pending, Approve, Crop marks and reads reuse them.
    private IReadOnlyList<IReadOnlyList<MultiAreaGroundCell>> EnsureGroundCache(MultiAreaSessionView view, CanopyLightReport canopy)
    {
        if (groundCache is null || groundCache.Value.AnnualRevision != root.AnnualRevision ||
            groundCache.Value.WorkRevision != root.WorkRevision)
        {
            var woodland = CrownModel.Woodland(view.Areas.SelectMany(area => area.Trees));
            var ground = view.Areas.Select((area, i) => MultiAreaUiReadouts.GroundOf(area, canopy.Areas[i], woodland)).ToArray();
            groundCache = (root.AnnualRevision, root.WorkRevision, Array.AsReadOnly(ground));
        }
        return groundCache.Value.Ground;
    }

    private MultiAreaUiReadout ReadoutOf(MultiAreaSessionView view)
    {
        var canopy = ReadCanopyLight(CanopyLightEvaluator.DefaultLightCellM);
        return MultiAreaUiReadouts.Build(view, Stage4ViewOf(root), canopy, models, EnsureGroundCache(view, canopy), root.Phase2);
    }

    // Caller holds sync. The body of ReadStage4View, unchanged, over the root passed in.
    private MultiAreaStage4View Stage4ViewOf(MultiAreaRoot current)
    {
        var stage4 = current.Stage4 ?? MultiAreaStage4State.Empty;
        var last = current.AnnualHistory.Count == 0 ? null : current.AnnualHistory[^1];
        var areaViews = new MultiAreaStage4AreaView[current.Areas.Count];
        for (int i = 0; i < areaViews.Length; i++)
        {
            var area = current.Areas[i];
            var trees = area.State.Trees.Select(tree =>
            {
                var pruning = stage4.PruningOf(tree.TreeId);
                return new MultiAreaStage4TreeView(tree.TreeId, LifeStates.Of(tree),
                    LifeStates.Of(tree) == LifeState.BiologicallyDead ? tree.Death!.Cause : null,
                    LifeStates.Of(tree) == LifeState.BiologicallyDead ? tree.Death!.EcologicalYear : null,
                    pruning.Lifts, pruning.CrownBaseHeightM, pruning.LastPruningYear, tree.CrownRadiusM);
            }).ToArray();
            int annual = last?.Areas[i].NaturalDeaths.Count ?? 0;
            int cumulative = current.AnnualHistory.Sum(year => year.Areas[i].NaturalDeaths.Count);
            areaViews[i] = new MultiAreaStage4AreaView(area.AreaId, areas[i].AreaHa,
                WorkRules.Summarize(area.State.Trees, areas[i].AreaHa).RelativeDensity, annual, cumulative,
                Array.AsReadOnly(stage4.Deadwood.Where(record => record.AreaId == area.AreaId).ToArray()),
                Array.AsReadOnly(trees));
        }
        return new MultiAreaStage4View(sessionId, current.EcologicalYear, current.PlanStateRevision, Token(current),
            models.Adult, models.Mortality, models.Crown, Stage4Evidence.All, Array.AsReadOnly(areaViews),
            Array.AsReadOnly(stage4.PendingPrunes.ToArray()), Array.AsReadOnly(stage4.ApprovedPrunes.ToArray()),
            Array.AsReadOnly(stage4.PruneLog.ToArray()));
    }

    // ------------------------------------------------------------------------------------------------ the shared year

    /// <summary>
    /// One genuine shared ecological year. Each populated area runs the unchanged AnnualRules.Calculate on its own trees,
    /// seed and area; an area with no living tree is carried forward unchanged. Unexecuted work needs explicit consent and
    /// is then cancelled for the whole experiment at zero Fell. Refused only when every area is empty.
    /// <paramref name="afterArea"/> and <paramref name="beforeCommit"/> are test-only fault seams.
    /// </summary>
    internal MultiAreaCommandResult Advance(MultiAreaFence fence, bool consentToCancelWork, Action<string>? afterArea = null,
        Action? beforeCommit = null)
    {
        lock (sync)
        {
            RequireFence(fence);
            bool hasWork = root.HasUnexecutedWork;
            if (hasWork && !consentToCancelWork)
                throw new P1Refusal("WorkPendingConsentRequired",
                    "advancing the year with unexecuted work needs explicit consent to cancel all of it at zero Fell");
            // Stage 5: a woodland with juvenile regeneration still has a year to advance even if no adult is alive.
            if (root.Areas.All(area => !area.State.Trees.Any(tree => tree.IsLiving)) && !(root.Stage5?.HasAbundance ?? false) &&
                !(root.Phase2?.HasJuveniles ?? false))
                throw new P1Refusal("ContinuationNoLivingStems", "every area has no living stems; there is no year to advance");

            int year = root.EcologicalYear;
            int revision = checked(root.AnnualRevision + 1);
            var nextAreas = new MultiAreaAreaState[root.Areas.Count];
            var records = new MultiAreaAreaYear[root.Areas.Count];
            var reports = new MultiAreaAreaReport[root.Areas.Count];
            bool lostCrop = false;
            // CrownModel 1 only: the start-of-year connected woodland, shared read-only by every area's step.
            AnnualTree[]? woodland = crownModel == CrownModelVersion.LegacyFrozen
                ? null
                : CrownModel.Woodland(root.Areas.SelectMany(area => area.State.Trees));
            // Stage 4 only: deadwood records are numbered in area order, then in the order the rules recorded the deaths.
            var stage4 = root.Stage4;
            var deadwood = new List<FallenDeadwoodRecord>();
            int nextDeadwood = stage4?.NextDeadwoodNumber ?? 1;
            // Phase 2 only: each rack's density route (pure Sitka or two-part mixed) and the species of every adult.
            var phase2Start = root.Phase2;
            var density = new List<Phase2DensityRecord>();
            for (int i = 0; i < root.Areas.Count; i++)
            {
                var area = root.Areas[i];
                var info = areas[i];
                AnnualState next;
                if (area.State.Trees.Any(tree => tree.IsLiving))
                {
                    AnnualReport report;
                    AnnualState stepped;
                    if (phase2Start is not null)
                    {
                        var (mixedReport, mixedNext, record) = Phase2MixedDensity.StepRack(area.AreaId, area.State, info.Seed, info.AreaHa,
                            woodland!, treeId => stage4!.PruningOf(treeId).Lifts, stage4Options!, phase2Start.BroadleafOf);
                        (report, stepped) = (mixedReport, mixedNext);
                        density.Add(record);
                    }
                    else
                        (report, stepped) = stage4 is not null
                            ? MultiAreaAnnual.StepStage4(area.State, info.Seed, info.AreaHa, woodland!,
                                treeId => stage4.PruningOf(treeId).Lifts, stage4Options!)
                            : crownModel == CrownModelVersion.LegacyFrozen
                                ? MultiAreaAnnual.Step(area.State, info.Seed, info.AreaHa)
                                : MultiAreaAnnual.Step(area.State, info.Seed, info.AreaHa, crownModel, woodland);
                    next = stepped;
                    if (stage4 is not null)
                    {
                        var byId = stepped.Trees.ToDictionary(tree => tree.TreeId, StringComparer.Ordinal);
                        foreach (var death in report.Deaths)
                        {
                            var dead = byId[death.TreeId];
                            // Every density death carries its species (Phase 2); the volume form is the same for all three.
                            deadwood.Add(new FallenDeadwoodRecord(FallenDeadwoodRecord.IdFor(nextDeadwood++), dead.TreeId,
                                area.AreaId, phase2Start?.SpeciesIdOf(dead.TreeId) ?? SpeciesProfile.Sitka.SpeciesId, dead.XM, dead.ZM,
                                death.EcologicalYear, death.Cause, dead.DbhCm, dead.HeightM,
                                FallenDeadwoodRecord.StemVolumeM3(dead.DbhCm, dead.HeightM)));
                        }
                    }
                    records[i] = new MultiAreaAreaYear(area.AreaId, report.Before, report.After,
                        Array.AsReadOnly(report.Deaths.ToArray()), NoLivingStems: false);
                    reports[i] = new MultiAreaAreaReport(area.AreaId, report);
                }
                else
                {
                    // Naturally empty (owner D-5): the engine is not run for this area; its dead roster is carried forward.
                    next = new AnnualState(checked(year + 1), area.State.Trees);
                    var empty = WorkRules.Summarize(area.State.Trees, info.AreaHa);
                    records[i] = new MultiAreaAreaYear(area.AreaId, empty, empty, Array.Empty<AnnualDeath>(), NoLivingStems: true);
                    reports[i] = new MultiAreaAreaReport(area.AreaId, null);
                }
                // Crop marks are plan state: a mark on a tree that died this year is cleared in the same publish.
                var livingIds = next.Trees.Where(tree => tree.IsLiving).Select(tree => tree.TreeId).ToHashSet(StringComparer.Ordinal);
                var crops = area.CropTreeIds.Where(livingIds.Contains).ToArray();
                lostCrop |= crops.Length != area.CropTreeIds.Count;
                nextAreas[i] = new MultiAreaAreaState(area.AreaId, next, Array.AsReadOnly(crops));
                afterArea?.Invoke(area.AreaId);
            }
            var yearRecord = new MultiAreaYearRecord(year, checked(year + 1), revision, root.WorkRevision,
                Array.AsReadOnly(records), hasWork);

            // Stage 5 only (spec §3 steps 5a–12): after every area's adult step (an adult-empty area included), one pure step
            // over the start-of-year Stage 5 state and the post-adult-step woodland. Promoted adults join their area's roster
            // in ordinal TreeId order; consented cancellation clears pending and approved ClearUndergrowth orders too.
            MultiAreaStage5State? nextStage5 = null;
            MultiAreaPhase2State? nextPhase2 = null;
            if (root.Stage5 is { } phase2Stage5 && phase2Start is not null)
            {
                // Phase 2 (spec §4 steps 3d–5): all three species' regeneration and the planted juveniles in one pure step.
                var stepped = Phase2Annual.Step(phase2Stage5, phase2Start, Stage5Inputs(nextAreas.Select(area => area.State.Trees).ToArray()),
                    checked(year + 1), density);
                for (int i = 0; i < nextAreas.Length; i++)
                {
                    var promoted = stepped.PromotedTrees[nextAreas[i].AreaId];
                    if (promoted.Count == 0) continue;
                    var roster = nextAreas[i].State.Trees.Concat(promoted).OrderBy(tree => tree.TreeId, StringComparer.Ordinal).ToArray();
                    nextAreas[i] = nextAreas[i] with { State = new AnnualState(checked(year + 1), Array.AsReadOnly(roster)) };
                }
                nextStage5 = stepped.NextStage5 with
                {
                    PendingClears = hasWork ? Array.Empty<MultiAreaClearUndergrowthOrder>() : stepped.NextStage5.PendingClears,
                    ApprovedClears = hasWork ? Array.Empty<MultiAreaClearUndergrowthOrder>() : stepped.NextStage5.ApprovedClears,
                };
                nextPhase2 = stepped.NextPhase2 with
                {
                    PendingOrders = hasWork ? Array.Empty<MultiAreaPlantingOrder>() : stepped.NextPhase2.PendingOrders,
                    ApprovedOrders = hasWork ? Array.Empty<MultiAreaPlantingOrder>() : stepped.NextPhase2.ApprovedOrders,
                };
            }
            else if (root.Stage5 is { } stage5Start)
            {
                var stepped = Stage5Annual.Step(stage5Start, Stage5Inputs(nextAreas.Select(area => area.State.Trees).ToArray()),
                    checked(year + 1));
                for (int i = 0; i < nextAreas.Length; i++)
                {
                    var promoted = stepped.PromotedTrees[nextAreas[i].AreaId];
                    if (promoted.Count == 0) continue;
                    var roster = nextAreas[i].State.Trees.Concat(promoted).OrderBy(tree => tree.TreeId, StringComparer.Ordinal).ToArray();
                    nextAreas[i] = nextAreas[i] with { State = new AnnualState(checked(year + 1), Array.AsReadOnly(roster)) };
                }
                nextStage5 = stepped.Next with
                {
                    PendingClears = hasWork ? Array.Empty<MultiAreaClearUndergrowthOrder>() : stepped.Next.PendingClears,
                    ApprovedClears = hasWork ? Array.Empty<MultiAreaClearUndergrowthOrder>() : stepped.Next.ApprovedClears,
                };
            }

            // Test-only fault seam: every area is calculated, nothing is published.
            beforeCommit?.Invoke();

            root = root with
            {
                EcologicalYear = checked(year + 1),
                AnnualRevision = revision,
                Areas = Array.AsReadOnly(nextAreas),
                AnnualHistory = Array.AsReadOnly(root.AnnualHistory.Append(yearRecord).ToArray()),
                Pending = hasWork ? Array.Empty<MultiAreaWorkOrder>() : root.Pending,
                Approved = hasWork ? null : root.Approved,
                PlanStateRevision = checked(root.PlanStateRevision + (hasWork || lostCrop ? 1 : 0)),
                // Stage 4: consented cancellation clears Prune orders with the Fells; deadwood is appended, never rewritten.
                Stage4 = stage4 is null ? null : stage4 with
                {
                    PendingPrunes = hasWork ? Array.Empty<MultiAreaPruneOrder>() : stage4.PendingPrunes,
                    ApprovedPrunes = hasWork ? Array.Empty<MultiAreaPruneOrder>() : stage4.ApprovedPrunes,
                    Deadwood = Array.AsReadOnly(stage4.Deadwood.Concat(deadwood).ToArray()),
                    NextDeadwoodNumber = nextDeadwood,
                },
                Stage5 = nextStage5,
                Phase2 = nextPhase2,
            };
            return WithView(null, new MultiAreaAnnualResult(year, checked(year + 1), Array.AsReadOnly(reports)));
        }
    }

    // ------------------------------------------------------------------------------------------------ save and load

    /// <summary>
    /// One immutable capture of the whole committed root under the single lock, ready for the strict save codec. The
    /// caller supplies the header label and generation; the year, model vector, genesis identity and engine lineage come
    /// from this session, so a save can never claim another session's identity. A stale fence refuses (nothing is
    /// captured); the capture itself changes nothing.
    /// </summary>
    internal Phase1SaveEnvelope CaptureSave(MultiAreaFence fence, string label, long saveGeneration)
    {
        lock (sync)
        {
            RequireFence(fence);
            var vector = Phase1RootMapping.VectorOf(models);
            var header = new Phase1SaveHeader(label, Phase1SaveHeader.FormatUtc(DateTime.UtcNow), saveGeneration,
                root.EcologicalYear, vector, Phase1RootMapping.GenesisOf(definition), Phase1EngineLineage.Current(vector));
            return new Phase1SaveEnvelope(header, Phase1RootMapping.Capture(root, models, stage4Options));
        }
    }

    /// <summary>
    /// What a load into THIS session requires: its exact model vector, genesis identity and engine lineage. A save made on
    /// another experiment, another model selection or another engine build is refused with a typed code, never migrated.
    /// </summary>
    internal Phase1SaveExpectation SaveExpectation()
    {
        var vector = Phase1RootMapping.VectorOf(models);
        return new Phase1SaveExpectation(vector, Phase1RootMapping.GenesisOf(definition), Phase1EngineLineage.Current(vector));
    }

    /// <summary>Mints a fresh 128-bit session id for a loaded forest (never derived from the file).</summary>
    internal static string NewSessionId() =>
        Convert.ToHexString(System.Security.Cryptography.RandomNumberGenerator.GetBytes(16)).ToLowerInvariant();

    /// <summary>
    /// Replace the whole live forest with a fully verified save, or refuse and change nothing (owner load policy A).
    /// The sequence inside ONE lock acquisition: check the caller's fence, require the exact save identity, rebuild a
    /// complete candidate root, revalidate every restored unexecuted order with the engine's own rules (A0 defect NB2),
    /// convert any saved approval into an unapproved visible Pending draft, and only then publish the candidate together
    /// with a NEW random session id and dropped read-out caches. Executed-key receipts stay as recorded history; no live
    /// fence survives, so an old page cannot Execute or Advance the restored forest.
    /// </summary>
    internal MultiAreaLoadResult Restore(MultiAreaFence fence, Phase1DecodedSave decoded)
    {
        ArgumentNullException.ThrowIfNull(decoded);
        lock (sync)
        {
            RequireFence(fence); // a stale tab may not replace a newer session
            Phase1SaveCodec.RequireExpected(decoded.Envelope.Header, SaveExpectation());
            RequireSameOptions(decoded.Envelope.State.Options);
            var restored = Phase1RootMapping.Restore(decoded.Envelope.State, Phase1RootMapping.VectorOf(models));
            // The saved approval's orders become pending FIRST, so the engine re-check below covers every order the restored
            // Work Plan will actually hold — orders that were approved in the file included (independent review finding 1).
            var candidate = WithSavedApprovalAsDraft(restored.Root, restored.SavedApproval);
            RequireLegalRestoredOrders(candidate);
            root = candidate;
            sessionId = NewSessionId();
            // The canopy and ground read-outs are cached per (AnnualRevision, WorkRevision) only, and the restored forest can
            // carry the same pair as the replaced one: dropping them here is what keeps a loaded map from showing the old trees.
            canopyCache.Clear();
            groundCache = null;
            return new MultiAreaLoadResult(ViewOf(root), DraftOrderCount(restored.SavedApproval), decoded.Checksum, null, null);
        }
    }

    // The saved deterministic options must be exactly this session's options. A forest is never continued under a different
    // Stage 4 growth-phase table, Stage 5 grid/browse/mast seed, or Phase 2 seed/species map (review finding: the Stage 4
    // phases used to be silently taken from the running session while the other options came from the file).
    private void RequireSameOptions(Phase1Options saved)
    {
        var mine = Phase1RootMapping.OptionsOf(root, stage4Options);
        bool same = SameStage4(mine.Stage4, saved.Stage4) && SameStage5(mine.Stage5, saved.Stage5) &&
            SamePhase2(mine.Phase2, saved.Phase2);
        if (!same)
            throw new P1Refusal("SaveOptionsMismatch",
                "this save was made with different Stage 4 growth phases, Stage 5 grid/browse/mast options or Phase 2 seeds " +
                "than this session has; options are never substituted or re-derived on load");
    }

    private static bool SameStage4(Phase1Stage4Options? a, Phase1Stage4Options? b)
    {
        if (a is null || b is null) return a is null && b is null;
        if (!string.Equals(a.SiteProductivity, b.SiteProductivity, StringComparison.Ordinal)) return false;
        var x = a.GrowthAgePhases;
        var y = b.GrowthAgePhases;
        if (x is null || y is null) return x is null && y is null;
        if (x.Count != y.Count) return false;
        for (int i = 0; i < x.Count; i++)
            if (F32.Bits(x[i].StartAgeYears) != F32.Bits(y[i].StartAgeYears) ||
                F32.Bits(x[i].DbhIncrementFactor) != F32.Bits(y[i].DbhIncrementFactor) ||
                F32.Bits(x[i].HeightIncrementFactor) != F32.Bits(y[i].HeightIncrementFactor)) return false;
        return true;
    }

    private static bool SameStage5(Phase1Stage5Options? a, Phase1Stage5Options? b)
    {
        if (a is null || b is null) return a is null && b is null;
        return a.GridCellM == b.GridCellM && F32.Bits(a.BackgroundBrowsePressure) == F32.Bits(b.BackgroundBrowsePressure) &&
            a.MastSeed == b.MastSeed;
    }

    private static bool SamePhase2(Phase1Phase2Options? a, Phase1Phase2Options? b)
    {
        if (a is null || b is null) return a is null && b is null;
        if (a.OakMastSeed != b.OakMastSeed || a.BeechMastSeed != b.BeechMastSeed) return false;
        if (a.GenesisSpecies.Count != b.GenesisSpecies.Count) return false;
        for (int i = 0; i < a.GenesisSpecies.Count; i++)
            if (!string.Equals(a.GenesisSpecies[i].TreeId, b.GenesisSpecies[i].TreeId, StringComparison.Ordinal) ||
                !string.Equals(a.GenesisSpecies[i].SpeciesId, b.GenesisSpecies[i].SpeciesId, StringComparison.Ordinal)) return false;
        return true;
    }

    // NB2: every restored but unexecuted order is revalidated against the candidate root with the engine's own rules,
    // before anything is published, so an illegal restored plan fails the WHOLE load instead of being repaired or dropped.
    private void RequireLegalRestoredOrders(MultiAreaRoot candidate)
    {
        foreach (var order in candidate.Pending)
        {
            if (order.Action != WorkAction.Fell)
                throw new P1Refusal("WorkActionInvalid", "a restored Fell order must be a Fell action");
            int index = AreaIndex(order.AreaId);
            RequireLivingIn(candidate, index, order.TreeId, "WorkTreeNotLive",
                "a restored Fell order names no living exact tree of its area");
            if (candidate.Areas[index].CropTreeIds.Contains(order.TreeId, StringComparer.Ordinal))
                throw new P1Refusal("CropTreeRetained", "a restored Fell order names a retained Crop Tree");
            if (string.IsNullOrWhiteSpace(order.Reason))
                throw new P1Refusal("WorkReasonRequired", "a restored Fell order has no recorded reason");
        }
        if (candidate.Stage4 is { } stage4)
            foreach (var order in stage4.PendingPrunes)
            {
                int index = AreaIndex(order.AreaId);
                RequireLivingIn(candidate, index, order.TreeId, "PruneTreeNotLiving",
                    "a restored Prune order names no living exact tree of its area");
                RequirePrunable(candidate, FindTreeIn(candidate, index, order.TreeId)!, stage4.PruningOf(order.TreeId),
                    order.TargetCrownBaseHeightM);
            }
        if (candidate.Stage5 is { } stage5)
            foreach (var order in stage5.PendingClears)
                RequireClearCell(stage5, order.AreaId, order.Column, order.Row);
        if (candidate.Phase2 is { } phase2 && phase2.PendingOrders.Count > 0)
        {
            var living = candidate.Areas.SelectMany(area => area.State.Trees).Where(tree => tree.IsLiving).ToArray();
            Phase2Planting.RequireValidBatch(phase2.PendingOrders, areaId => areas[AreaIndex(areaId)].Polygon, living, phase2,
                Phase2Annual.Locator(candidate.Stage5!), checked(candidate.EcologicalYear + 1));
        }
    }

    // Owner load policy A / reviewer ruling B1: a saved approved plan never comes back approved. Its orders re-enter as an
    // ordinary unapproved Pending draft the player must approve again under the fresh session; its engine-minted PlanId,
    // execution key and approval token were never saved, so no execution authority is restored either.
    private static MultiAreaRoot WithSavedApprovalAsDraft(MultiAreaRoot at, Phase1SavedApprovalDraft? draft)
    {
        if (draft is null) return at;
        return at with
        {
            Pending = Array.AsReadOnly(at.Pending.Concat(draft.Fells).ToArray()),
            Stage4 = at.Stage4 is null || draft.Prunes.Count == 0 ? at.Stage4
                : at.Stage4 with { PendingPrunes = Array.AsReadOnly(at.Stage4.PendingPrunes.Concat(draft.Prunes).ToArray()) },
            Stage5 = at.Stage5 is null || draft.Clears.Count == 0 ? at.Stage5
                : at.Stage5 with { PendingClears = Array.AsReadOnly(at.Stage5.PendingClears.Concat(draft.Clears).ToArray()) },
            Phase2 = at.Phase2 is null || draft.Planting.Count == 0 ? at.Phase2
                : at.Phase2 with { PendingOrders = Array.AsReadOnly(at.Phase2.PendingOrders.Concat(draft.Planting).ToArray()) },
        };
    }

    private static int DraftOrderCount(Phase1SavedApprovalDraft? draft) =>
        draft is null ? 0 : draft.Fells.Count + draft.Prunes.Count + draft.Clears.Count + draft.Planting.Count;

    // ------------------------------------------------------------------------------------------------ canopy and light (#900)

    /// <summary>
    /// Read-only canopy cover, growing-space and modelled light read-out of the CURRENT committed trees, per area polygon,
    /// with the connected woodland as the neighbourhood. Computed at most once per committed tree state and light grid
    /// (load, Execute, natural death, annual crown growth); Pending, Approve, Crop marks and repeated reads return the same
    /// cached value. Session-only: nothing is saved and no root, token or revision changes.
    /// </summary>
    internal CanopyLightReport ReadCanopyLight(double lightCellM = CanopyLightEvaluator.DefaultLightCellM)
    {
        CanopyLightEvaluator.RequireSupportedGrid(lightCellM);
        lock (sync)
        {
            if (canopyCache.TryGetValue(lightCellM, out var cached) &&
                cached.AnnualRevision == root.AnnualRevision && cached.WorkRevision == root.WorkRevision)
                return cached.Report;
            var input = new CanopyArea[root.Areas.Count];
            for (int i = 0; i < input.Length; i++)
                input[i] = new CanopyArea(areas[i].AreaId,
                    Array.AsReadOnly(areas[i].Polygon.Select(v => new CanopyVertex(v.XM, v.ZM)).ToArray()), areas[i].AreaHa,
                    root.Areas[i].State.Trees);
            var report = CanopyLightEvaluator.Evaluate(input, lightCellM, crownModel);
            canopyCache[lightCellM] = (root.AnnualRevision, root.WorkRevision, report);
            canopyRecomputeCount++;
            return report;
        }
    }

    /// <summary>How many times a canopy/light read-out has been computed in this session (for recompute-trigger tests).</summary>
    internal int CanopyRecomputeCount
    {
        get { lock (sync) return canopyRecomputeCount; }
    }

    // ------------------------------------------------------------------------------------------------ read-only inspection

    /// <summary>
    /// Read-only competition inspection of ANY living tree of one area, before or after any Crop designation. It runs the
    /// existing single-area diagnostic unchanged on this area's own trees (so neighbours across a boundary are excluded,
    /// exactly as in the annual rules) and never changes the root.
    /// </summary>
    internal MultiAreaCompetitionResult Competition(MultiAreaFence fence, string areaId, string treeId)
    {
        MultiAreaSessionView view;
        int index;
        lock (sync)
        {
            RequireFence(fence);
            index = AreaIndex(areaId);
            if (string.IsNullOrWhiteSpace(treeId))
                throw new P1Refusal("CompetitionTreeUnknown", "a tree ID is required");
            if (FindTree(index, treeId) is null && OwnerOf(treeId) is string owner)
                throw new P1Refusal("MultiAreaWrongArea", "tree " + treeId + " belongs to " + owner + ", not " + areaId);
            view = ViewOf(root);
        }
        // Pure arithmetic over the immutable view; it needs no lock and keeps no state.
        var report = PlayerCompetitionDiagnostics.Measure(MultiAreaProjection.Of(view, view.Areas[index]), treeId);
        return new MultiAreaCompetitionResult(view, areaId, report, null, null);
    }

    /// <summary>Read-only exact-ID hit-test on one area's CURRENT map; a stale fence is refused.</summary>
    internal MultiAreaHitResult HitTest(MultiAreaFence fence, string areaId, double u, double v)
    {
        MultiAreaSessionView view;
        int index;
        lock (sync)
        {
            RequireFence(fence);
            index = AreaIndex(areaId);
            view = ViewOf(root);
        }
        var scene = MultiAreaProjection.Scene(view, view.Areas[index]);
        return new MultiAreaHitResult(view, areaId, PlayerMapScene.HitTest(scene, u, v), null, null);
    }

    // ------------------------------------------------------------------------------------------------ helpers (caller holds sync)

    // Fence order: session, then the full source token, then the plan-state revision.
    private void RequireFence(MultiAreaFence fence)
    {
        RequireSession(fence);
        if (fence.Source != Token(root))
            throw new P1Refusal("PlayerSourceStale",
                "the source, year or a revision changed since this view was read; nothing changed, re-read the current view");
        if (fence.PlanStateRevision != root.PlanStateRevision)
            throw new P1Refusal("PlayerPlanStateStale",
                "the Work Plan changed since this view was read; nothing changed, re-read the current view");
    }

    private void RequireSession(MultiAreaFence fence)
    {
        if (fence is null || !string.Equals(fence.SessionId, sessionId, StringComparison.Ordinal))
            throw new P1Refusal("PlayerSessionMismatch", "this request does not belong to the current multi-area session");
    }

    private MultiAreaStage4State RequireStage4() => root.Stage4 ??
        throw new P1Refusal("Stage4ModelRequired",
            "pruning needs AdultModelVersion 1, MortalityModelVersion 1 and CrownModelVersion 1; this session is not Stage 4");

    private MultiAreaStage5State RequireStage5() => root.Stage5 ??
        throw new P1Refusal("Stage5ModelRequired",
            "regeneration and ClearUndergrowth need the 1/1/1/1 model selection; this session is not Stage 5");

    private MultiAreaPhase2State RequirePhase2() => root.Phase2 ??
        throw new P1Refusal("Phase2ModelRequired",
            "Oak, Beech, Plant, Shelter and SpotClear need the 1/1/1/1/1 model selection; this session is not Phase 2");

    // The pruning guards of one tree: the unchanged Sitka rules, or the broadleaf high-pruning rules at Phase 2.
    private void RequirePrunable(AnnualTree tree, TreePruningState state, float target) =>
        RequirePrunable(root, tree, state, target);

    // The same guards against an explicit root: a Load revalidates its candidate before publishing it.
    private static void RequirePrunable(MultiAreaRoot at, AnnualTree tree, TreePruningState state, float target)
    {
        if (at.Phase2 is { } phase2)
            Phase2Pruning.Require(tree, state, target, at.EcologicalYear, phase2.BroadleafOf(tree.TreeId));
        else
            PruningRules.Require(tree, state, target, at.EcologicalYear);
    }

    // Validates a planting batch against the current root: area polygons, every living adult stem, existing juveniles and
    // shelters, and the ground lattice. The install year is the first annual step after Execute.
    private void RequireValidPlantingBatch(MultiAreaPhase2State phase2, IReadOnlyList<MultiAreaPlantingOrder> batch)
    {
        var living = root.Areas.SelectMany(area => area.State.Trees).Where(tree => tree.IsLiving).ToArray();
        Phase2Planting.RequireValidBatch(batch, areaId => areas[AreaIndex(areaId)].Polygon, living, phase2,
            Phase2Annual.Locator(root.Stage5!), checked(root.EcologicalYear + 1));
    }

    // A clear target must be one existing in-polygon ground cell of its own area. A cell of the other area is refused as
    // MultiAreaWrongArea (as for trees); a cell of no area (outside every polygon, or no such lattice cell) as ClearCellNotInArea.
    private static void RequireClearCell(MultiAreaStage5State stage5, string areaId, int column, int row)
    {
        if (stage5.CellOf(areaId, column, row) is not null) return;
        foreach (var area in stage5.Areas)
            if (area.AreaId != areaId && stage5.CellOf(area.AreaId, column, row) is not null)
                throw new P1Refusal("MultiAreaWrongArea",
                    FormattableString.Invariant($"ground cell ({column}, {row}) belongs to {area.AreaId}, not {areaId}"));
        throw new P1Refusal("ClearCellNotInArea", "no in-polygon ground cell of this area has that column and row");
    }

    // The Stage 5 inputs of every area, in area order, for the given rosters (one per area, same order).
    private Stage5AreaInput[] Stage5Inputs(IReadOnlyList<IReadOnlyList<AnnualTree>> rosters) =>
        areas.Select((info, i) => new Stage5AreaInput(info.AreaId, info.Seed, info.Polygon, info.AreaHa, rosters[i])).ToArray();

    private int AreaIndex(string areaId)
    {
        for (int i = 0; i < areas.Count; i++)
            if (string.Equals(areas[i].AreaId, areaId, StringComparison.Ordinal)) return i;
        throw new P1Refusal("MultiAreaUnknownArea", "no area of this experiment has that exact ID");
    }

    private AnnualTree? FindTree(int areaIndex, string treeId)
    {
        foreach (var tree in root.Areas[areaIndex].State.Trees)
            if (string.Equals(tree.TreeId, treeId, StringComparison.Ordinal)) return tree;
        return null;
    }

    private static AnnualTree? FindTreeIn(MultiAreaRoot at, int areaIndex, string treeId)
    {
        foreach (var tree in at.Areas[areaIndex].State.Trees)
            if (string.Equals(tree.TreeId, treeId, StringComparison.Ordinal)) return tree;
        return null;
    }

    // A Load's candidate is not reachable yet, so its refused order is reported without the wrong-area refinement.
    private static void RequireLivingIn(MultiAreaRoot at, int areaIndex, string treeId, string code, string message)
    {
        var tree = string.IsNullOrEmpty(treeId) ? null : FindTreeIn(at, areaIndex, treeId);
        if (tree is null || !tree.IsLiving) throw new P1Refusal(code, message);
    }

    private string? OwnerOf(string treeId)
    {
        for (int i = 0; i < root.Areas.Count; i++)
            if (FindTree(i, treeId) is not null) return root.Areas[i].AreaId;
        return null;
    }

    // A tree of another area is refused as MultiAreaWrongArea, before any unknown/dead refusal, so a wrong-area request
    // can never be mistaken for a typo.
    private void RequireLivingInArea(int areaIndex, string treeId, string unknownCode, string deadCode, string message)
    {
        var tree = string.IsNullOrEmpty(treeId) ? null : FindTree(areaIndex, treeId);
        if (tree is null)
        {
            if (!string.IsNullOrEmpty(treeId) && OwnerOf(treeId) is string owner)
                throw new P1Refusal("MultiAreaWrongArea",
                    "tree " + treeId + " belongs to " + owner + ", not " + root.Areas[areaIndex].AreaId);
            throw new P1Refusal(unknownCode, message);
        }
        if (!tree.IsLiving) throw new P1Refusal(deadCode, message);
    }

    private MultiAreaSourceToken Token(MultiAreaRoot current) => new(MultiAreaIdentity.Domain, experiment.ExperimentId,
        experiment.ManifestContentHash, areaIds, current.EcologicalYear, current.AnnualRevision, current.WorkRevision,
        DerivedManifest.RngModelVersion, DerivedManifest.GrowthModelVersion, DerivedManifest.P1AlgorithmVersion,
        areaModelVersion, MultiAreaIdentity.ActionEligibleDbhThresholdMm);

    // The one execution key the engine mints for a plan. The view shows it and a first Execute requires it.
    private static string MintedExecutionKey(string planId) => planId + ":execute";

    private MultiAreaCommandResult WithView(MultiAreaExecutionResult? execution, MultiAreaAnnualResult? annual) =>
        new(ViewOf(root), execution, annual, null, null);

    // Every exposed collection is a fresh read-only copy taken from the single root passed in.
    private MultiAreaSessionView ViewOf(MultiAreaRoot current)
    {
        var areaViews = new MultiAreaAreaView[current.Areas.Count];
        for (int i = 0; i < current.Areas.Count; i++)
        {
            var info = areas[i];
            var state = current.Areas[i];
            var trees = Array.AsReadOnly(state.State.Trees.ToArray());
            areaViews[i] = new MultiAreaAreaView(info.AreaId, info.Label, info.StockingNote, MultiAreaIdentity.Site, info.Seed,
                info.AreaHa, info.AreaM2Exact, Array.AsReadOnly(info.Polygon.ToArray()), info.Extent, trees,
                WorkRules.Summarize(trees, info.AreaHa), Array.AsReadOnly(state.CropTreeIds.ToArray()));
        }
        var approved = current.Approved is null ? null : current.Approved with
        {
            Orders = Array.AsReadOnly(current.Approved.Orders.ToArray()),
        };
        return new MultiAreaSessionView(sessionId, current.PlanStateRevision, Token(current), experiment,
            Array.AsReadOnly(areaViews), Array.AsReadOnly(current.Pending.ToArray()), approved,
            Array.AsReadOnly(current.Events.ToArray()), Array.AsReadOnly(current.AnnualHistory.ToArray()));
    }

    private static IReadOnlyList<MultiAreaAreaState> Replace(IReadOnlyList<MultiAreaAreaState> list, int index,
        MultiAreaAreaState value)
    {
        var copy = list.ToArray();
        copy[index] = value;
        return Array.AsReadOnly(copy);
    }

    private static AreaInfo InfoOf(MultiAreaDefinition area)
    {
        var polygon = Array.AsReadOnly(area.PolygonMm.Select(p => new MultiAreaVertex(p.XMm / 1000.0, p.YMm / 1000.0)).ToArray());
        var (minX, minY, maxX, maxY) = MultiAreaGeometry.Bounds(area.PolygonMm);
        // A square map around the polygon with a 1 m margin, in the single-area map convention (U east, V south).
        long side = Math.Max(maxX - minX, maxY - minY) + 2_000;
        var extent = new PlayerMapExtent((minX - 1_000) / 1000.0, (minY - 1_000) / 1000.0, side / 1000.0, area.AreaHa);
        return new AreaInfo(area.AreaId, area.Label, area.StockingNote, area.SimulationSeed, area.AreaHa, area.AreaM2Exact,
            polygon, extent);
    }
}
