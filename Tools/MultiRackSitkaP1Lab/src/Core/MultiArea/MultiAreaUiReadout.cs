namespace SitkaP1.Core;

// /multi Stage 1–4 UI catch-up (spec multi-stage4-ui-catchup-v1, owner packet 2026-11-23, D-095, D-020): the read-out the
// page draws. Every number here is computed by the engine from ONE committed root: life states, pruning history and
// eligibility (with the engine's own refusal code and reason), per-rack RD, natural deaths, fallen deadwood, geometric
// canopy cover and the 2.5 m modelled light cells. The page only formats these values; it computes none of them. These are
// NEW internal records: no existing view record changes shape. Session-only: nothing here is saved or enters a token.

/// <summary>Model versions and the honesty text shown with every Stage 4 / light value.</summary>
internal sealed record MultiAreaUiModels(
    int CrownModelVersion,
    int AdultModelVersion,
    int MortalityModelVersion,
    string LightModelVersion,
    string LightLabel,
    bool CalibratedPar,
    string NotCalibratedCaveat,
    string SyntheticStandDisclaimer,
    double LightCellM,
    double ShadeBufferM,
    IReadOnlyList<string> EvidenceLabels,
    IReadOnlyList<string> LightUncertainties);

/// <summary>
/// One tree's Stage 4 read-out. Prune eligibility is the engine's guard result for the next scheduled lift in the current
/// year (<see cref="PruningRules.ScheduledRefusalCode"/>); plan conflicts (an order already queued) are reported separately
/// in <see cref="PrunePlanState"/> and by the engine's refusal when a command is sent.
/// </summary>
internal sealed record MultiAreaUiTree(
    string TreeId,
    LifeState LifeState,
    string? DeathCause,
    int? DeathYear,
    int PruningLifts,
    int MaxPruningLifts,
    float CrownBaseHeightM,
    int LastPruningYear,
    bool PruneEligible,
    string? PruneRefusalCode,
    string? PruneRefusalReason,
    float? NextPruneTargetM,
    int? NextPruneEligibleYear,
    string? PrunePlanState);

/// <summary>One fallen-deadwood record with its engine position and the same position in the area's map metres (U east, V south).</summary>
internal sealed record MultiAreaUiDeadwood(
    string DeadwoodId,
    string TreeId,
    float XM,
    float ZM,
    double U,
    double V,
    int DeathYear,
    string Cause,
    float DbhCm,
    float HeightM,
    float EstimatedVolumeM3);

/// <summary>Per-rack canopy summary: geometric cover (crown union), open ground, largest opening, RD, natural deaths and deadwood.</summary>
internal sealed record MultiAreaCanopySummary(
    double PolygonAreaM2,
    double GeometricCoverFraction,
    double GeometricCoveredM2,
    double GeometricUncoveredM2,
    int OpeningCount,
    double LargestOpeningAreaM2,
    double LargestOpeningWidthM,
    double RelativeDensity,
    int AnnualNaturalDeaths,
    int CumulativeNaturalDeaths,
    int FallenDeadwoodCount,
    double MeanLightIndex,
    float MinLightIndex,
    float MaxLightIndex,
    IReadOnlyList<MultiAreaUiDeadwood> FallenDeadwood);

/// <summary>A vertex of a drawn ground cell, in engine metres and in the area's map metres.</summary>
internal sealed record MultiAreaGroundVertex(double XM, double ZM, double U, double V);

/// <summary>
/// One 2.5 m modelled light cell of one rack: the polygon-clipped cell shape, its light index (Unity k10a10 proxy — not
/// calibrated PAR), the geometric crown cover of its clipped area and the living trees that shade its centre.
/// </summary>
internal sealed record MultiAreaGroundCell(
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
    IReadOnlyList<string> ContributingTreeIds);

internal sealed record MultiAreaUiArea(string AreaId, MultiAreaCanopySummary Canopy, IReadOnlyList<MultiAreaUiTree> Trees,
    IReadOnlyList<MultiAreaGroundCell> Ground);

/// <summary>The whole read-out of one committed root (its token and plan-state revision name that root).</summary>
internal sealed record MultiAreaUiReadout(
    string SessionId,
    int EcologicalYear,
    int PlanStateRevision,
    MultiAreaSourceToken Source,
    MultiAreaUiModels Models,
    IReadOnlyList<MultiAreaUiArea> Areas,
    IReadOnlyList<MultiAreaPruneOrder> PendingPrunes,
    IReadOnlyList<MultiAreaPruneOrder> ApprovedPrunes,
    IReadOnlyList<MultiAreaPruneRecord> PruneLog);

/// <summary>A session view and the read-out of the same root, taken under one lock.</summary>
internal sealed record MultiAreaUiSnapshot(MultiAreaSessionView View, MultiAreaUiReadout Readout);

/// <summary>Builds the read-out from values the engine has already computed for one root. Pure; keeps no state.</summary>
internal static class MultiAreaUiReadouts
{
    internal const string Stage4ModelRequiredReason =
        "pruning needs AdultModelVersion 1, MortalityModelVersion 1 and CrownModelVersion 1; this session is not Stage 4";

    internal static MultiAreaUiModels Models(MultiAreaModelSelection models, LightModelMetadata light) => new(
        (int)models.Crown, (int)models.Adult, (int)models.Mortality, light.Version, light.Label, light.CalibratedPar,
        light.Caveat, Stage4Evidence.SyntheticStands, light.GridCellM, light.ShadeBufferM, Stage4Evidence.All,
        light.Uncertainties);

    internal static MultiAreaUiReadout Build(MultiAreaSessionView view, MultiAreaStage4View stage4, CanopyLightReport canopy,
        MultiAreaModelSelection models, IReadOnlyList<IReadOnlyList<MultiAreaGroundCell>> ground,
        MultiAreaPhase2State? phase2 = null)
    {
        var areas = new MultiAreaUiArea[view.Areas.Count];
        for (int i = 0; i < areas.Length; i++)
        {
            var area = view.Areas[i];
            var s4 = stage4.Areas[i];
            var report = canopy.Areas[i];
            if (s4.AreaId != area.AreaId || report.AreaId != area.AreaId)
                throw new InvalidOperationException("read-out areas are not in view order");
            var deadwood = s4.Deadwood.Select(d => new MultiAreaUiDeadwood(d.DeadwoodId, d.TreeId, d.XM, d.ZM,
                MapU(area.Extent, d.XM), MapV(area.Extent, d.ZM), d.DeathYear, d.Cause, d.DbhCm, d.HeightM, d.EstimatedVolumeM3)).ToArray();
            var summary = new MultiAreaCanopySummary(report.PolygonAreaM2, report.Cover.CoverFraction, report.Cover.CoveredM2,
                report.Cover.UncoveredM2, report.Cover.OpeningCount, report.Cover.LargestOpeningAreaM2, report.Cover.LargestOpeningWidthM,
                s4.RelativeDensity, s4.AnnualNaturalDeaths, s4.CumulativeNaturalDeaths, deadwood.Length, report.Light.MeanLightIndex,
                report.Light.MinLightIndex, report.Light.MaxLightIndex, Array.AsReadOnly(deadwood));
            var trees = area.Trees.Select((tree, t) => TreeOf(tree, s4.Trees[t], stage4, models.IsStage4,
                phase2?.BroadleafOf(tree.TreeId))).ToArray();
            areas[i] = new MultiAreaUiArea(area.AreaId, summary, Array.AsReadOnly(trees), ground[i]);
        }
        return new MultiAreaUiReadout(view.SessionId, view.Source.EcologicalYear, view.PlanStateRevision, view.Source,
            Models(models, canopy.Metadata), Array.AsReadOnly(areas), stage4.PendingPrunes, stage4.ApprovedPrunes, stage4.PruneLog);
    }

    /// <summary>The ground cells of one rack: one per light cell of the canopy report, in its order.</summary>
    internal static IReadOnlyList<MultiAreaGroundCell> GroundOf(MultiAreaAreaView area, CanopyAreaReport report,
        IReadOnlyList<AnnualTree> woodland)
    {
        var polygon = area.Polygon.Select(p => new CanopyVertex(p.XM, p.ZM)).ToArray();
        double cell = report.Light.CellM;
        var cells = new MultiAreaGroundCell[report.Light.Cells.Count];
        for (int i = 0; i < cells.Length; i++)
        {
            var light = report.Light.Cells[i];
            var shape = CanopyLightEvaluator.ClippedSquare(polygon, light.Column * cell, light.Row * cell,
                (light.Column + 1) * cell, (light.Row + 1) * cell);
            var (covered, _) = CanopyLightEvaluator.CellCover(polygon, woodland, light.Column, light.Row, cell);
            cells[i] = new MultiAreaGroundCell(light.Column, light.Row, light.CentreXM, light.CentreZM,
                MapU(area.Extent, light.CentreXM), MapV(area.Extent, light.CentreZM), light.ClippedAreaM2,
                Array.AsReadOnly(shape.Select(p => new MultiAreaGroundVertex(p.XM, p.ZM, MapU(area.Extent, p.XM), MapV(area.Extent, p.ZM))).ToArray()),
                light.LightIndex, light.ClippedAreaM2 > 0 ? Math.Min(1.0, covered / light.ClippedAreaM2) : 0.0, light.CrossBorderTrees,
                CanopyLightEvaluator.ContributorsAt(light.CentreXM, light.CentreZM, woodland));
        }
        return Array.AsReadOnly(cells);
    }

    // The area's map metres, in the same convention as the single-area map scene: U east from the extent's west edge, V south
    // from its north edge.
    internal static double MapU(PlayerMapExtent extent, double xM) => xM - extent.MinXM;

    internal static double MapV(PlayerMapExtent extent, double zM) => extent.MinZM + extent.SideM - zM;

    private static MultiAreaUiTree TreeOf(AnnualTree tree, MultiAreaStage4TreeView s4, MultiAreaStage4View stage4, bool isStage4,
        Phase2Species? broadleaf)
    {
        if (s4.TreeId != tree.TreeId) throw new InvalidOperationException("read-out trees are not in view order");
        string? planState = stage4.PendingPrunes.Any(o => o.TreeId == tree.TreeId) ? "PendingPrune"
            : stage4.ApprovedPrunes.Any(o => o.TreeId == tree.TreeId) ? "ApprovedPrune" : null;
        var status = MultiAreaSpeciesPruneHelper.Evaluate(tree,
            new TreePruningState(s4.PruningLifts, s4.CrownBaseHeightM, s4.LastPruningYear),
            stage4.EcologicalYear, broadleaf, isStage4, planState);
        return new MultiAreaUiTree(tree.TreeId, s4.LifeState, s4.DeathCause, s4.DeathYear, s4.PruningLifts, status.MaxLifts,
            s4.CrownBaseHeightM, s4.LastPruningYear, status.Eligible, status.RefusalCode, status.RefusalReason,
            status.NextTargetM, status.NextEligibleYear, planState);
    }
}
