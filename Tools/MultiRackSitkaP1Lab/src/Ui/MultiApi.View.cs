using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using SitkaP1.Core;

namespace SitkaP1.Ui;

internal sealed partial class PlayerApi
{
    // Preserves the exact canonical manifest body and its trailing contentHash verification (M07), appending truthful
    // Pass 1 experiment badge and active model tuple metadata in pure ASCII JSON after contentHash.
    private Task WriteMultiManifestAsync(HttpContext context)
    {
        var pair = CurrentMulti;
        byte[] raw = pair.Adapter.ManifestBytes();
        var snap = pair.Adapter.ReadCompositeUiSnapshot();
        var m = snap.Ecology?.Models ?? new MultiAreaEcologyModels(
            snap.Readout.Models.AdultModelVersion, snap.Readout.Models.MortalityModelVersion, snap.Readout.Models.CrownModelVersion, 0, 0);
        string suffix = FormattableString.Invariant(
            $",\"experimentKey\":\"{JsonEncodedText.Encode(pair.Adapter.ExperimentKey)}\",\"experimentBadge\":\"{JsonEncodedText.Encode(pair.Adapter.ExperimentBadge)}\",\"models\":{{\"adult\":{m.Adult},\"mortality\":{m.Mortality},\"crown\":{m.Crown},\"regeneration\":{m.Regeneration},\"species\":{m.Species}}}}}");
        byte[] suffixBytes = Encoding.ASCII.GetBytes(suffix);
        var combined = new byte[raw.Length - 1 + suffixBytes.Length];
        Buffer.BlockCopy(raw, 0, combined, 0, raw.Length - 1);
        Buffer.BlockCopy(suffixBytes, 0, combined, raw.Length - 1, suffixBytes.Length);
        return WriteAssetAsync(context, combined, JsonContentType);
    }

    // Composite read: view, stage4 and ecology share one exact Core snapshot lock; totalRecordedYears and reviewPreview
    // are read-only metadata queries. Bounded to <= MaxViewPayloadBytes (512 KiB) raw UTF-8 and <= MaxEcologyPayloadBytes
    // (150 KiB) ecology slice via projection-only reductions while keeping the authoritative Core root untouched.
    private Task ReadMultiViewAsync(HttpContext context)
    {
        MultiAreaCompositeUiSnapshot snapshot;
        int totalRecordedYears;
        MultiReviewPreviewDto? reviewPreview;
        bool economyEnabled;
        MultiEconomySnapshotDto? economy;
        lock (multiSwapSync)
        {
            var pair = CurrentMulti;
            snapshot = pair.Adapter.ReadCompositeUiSnapshot();
            totalRecordedYears = pair.Session.ReadHistoryIndex(pageSize: 1).TotalRecordedYears;
            reviewPreview = TryPreviewForView(pair, snapshot.View);
            economyEnabled = pair.Session.SelectedModels.IsEconomyEnabled;
            economy = TryBuildEconomySnapshot(pair.Session, snapshot.View);
        }
        SerializeBoundedMultiView(snapshot, totalRecordedYears, reviewPreview, out byte[] bytes, economyEnabled, economy);
        return WriteRawJsonAsync(context, StatusCodes.Status200OK, bytes);
    }

    private sealed record BoundedMultiSnapshot(
        object View,
        object Scenes,
        object? Stage4,
        MultiAreaEcologyDto? Ecology,
        bool HistoryTruncated,
        int TotalRecordedYears,
        bool ProjectionCompacted,
        bool Stage4Compacted,
        bool PayloadBudgetExceeded);

    private static double R3(double v) => double.IsFinite(v) ? Math.Round(v, 3, MidpointRounding.AwayFromZero) : v;
    private static float R3f(float v) => float.IsFinite(v) ? MathF.Round(v, 3, MidpointRounding.AwayFromZero) : v;

    // Lossless ecology projection for HTTP transport: dead unpruned trees can never be pruned and are omitted from
    // pruneStatusByTreeId while every living tree and any dead tree with lifts > 0 or a planState stays intact.
    private static MultiAreaEcologyDto? ProjectEcologyForView(MultiAreaEcologyDto? ecology, MultiAreaSessionView view)
    {
        if (ecology is null || ecology.PruneStatusByTreeId.Count == 0) return ecology;
        var livingIds = new HashSet<string>(StringComparer.Ordinal);
        foreach (var area in view.Areas)
        {
            foreach (var tree in area.Trees)
            {
                if (tree.IsLiving) livingIds.Add(tree.TreeId);
            }
        }
        var filtered = new Dictionary<string, MultiAreaSpeciesPruneStatus>(livingIds.Count, StringComparer.Ordinal);
        bool droppedAny = false;
        foreach (var (treeId, status) in ecology.PruneStatusByTreeId)
        {
            if (livingIds.Contains(treeId) || status.Lifts > 0 || status.PlanState is not null)
                filtered[treeId] = status;
            else
                droppedAny = true;
        }
        return droppedAny ? ecology with { PruneStatusByTreeId = filtered } : ecology;
    }

    // Projection-only bounded serialization for GET api/multi/view (Owner G5 Year 30 horizon: raw <= 524,288 B,
    // ecology <= 153,600 B). Never mutates or truncates the committed Core root. Preserves at least the latest work
    // event and latest annual record whenever present; trims stage4.pruneLog before dropping them; omits rather than
    // fabricates empty lists; and never returns over-budget bytes silently (sets payloadBudgetExceeded + logs to stderr).
    private static BoundedMultiSnapshot SerializeBoundedMultiView(
        MultiAreaCompositeUiSnapshot snapshot,
        int totalRecordedYears,
        MultiReviewPreviewDto? reviewPreview,
        out byte[] finalBytes,
        bool economyEnabled = false,
        MultiEconomySnapshotDto? economy = null) =>
        SerializeBoundedMultiView(snapshot.View, snapshot.Readout, snapshot.Ecology, totalRecordedYears, reviewPreview, out finalBytes, economyEnabled, economy);

    private static BoundedMultiSnapshot SerializeBoundedMultiView(
        MultiAreaSessionView fullView,
        MultiAreaUiReadout? fullStage4,
        MultiAreaEcologyDto? fullEcology,
        int totalRecordedYears,
        MultiReviewPreviewDto? reviewPreview,
        out byte[] finalBytes,
        bool economyEnabled = false,
        MultiEconomySnapshotDto? economy = null)
    {
        var fullScenes = ScenesOf(fullView);
        var projEco = ProjectEcologyForView(fullEcology, fullView);
        int ecoBytes = projEco is null ? 0 : JsonSerializer.SerializeToUtf8Bytes(projEco, ResponseJson).Length;

        int minEvents = fullView.Events.Count > 0 ? 1 : 0;
        int minAnnual = fullView.AnnualHistory.Count > 0 ? 1 : 0;
        int maxHist = Math.Max(fullView.Events.Count, fullView.AnnualHistory.Count);

        byte[] Serialize(object v, object sc, object? s4, bool histTrunc, bool projCompact, bool s4Compact, bool exceeded) =>
            JsonSerializer.SerializeToUtf8Bytes(
                new MultiViewPayload(v, sc, MultiLabels, s4, projEco, histTrunc, totalRecordedYears, reviewPreview,
                    projCompact, s4Compact, exceeded, economyEnabled, economy),
                ResponseJson);

        MultiAreaSessionView TailRaw(int keepEvents, int keepAnnual) => fullView with
        {
            Events = TailOf(fullView.Events, keepEvents),
            AnnualHistory = TailOf(fullView.AnnualHistory, keepAnnual),
        };

        // Helper: given fixed scenes and a stage4 builder over pruneLog cap, search the largest history tail >= (minEvents, minAnnual)
        // that fits within MaxViewPayloadBytes. Trims stage4.pruneLog BEFORE dropping below 1 event / 1 annual record.
        bool TryFitTier(
            bool projectView,
            object scenesObj,
            bool projCompact,
            bool s4Compact,
            Func<int, object?> stage4WithPruneCap,
            out BoundedMultiSnapshot? fit,
            out byte[]? fitBytes)
        {
            fit = null;
            fitBytes = null;
            if (ecoBytes > MaxEcologyPayloadBytes) return false;

            int fullPruneCount = fullStage4?.PruneLog.Count ?? 0;
            // Try full pruneLog first, then capped pruneLog (10 -> 1 -> 0) before giving up on keeping >= 1 event & annual record.
            var pruneCaps = new List<int> { fullPruneCount };
            if (fullPruneCount > 10) pruneCaps.Add(10);
            if (fullPruneCount > 1) pruneCaps.Add(1);
            if (fullPruneCount > 0) pruneCaps.Add(0);

            object MakeView(int keepEv, int keepAn)
            {
                var tv = (keepEv >= fullView.Events.Count && keepAn >= fullView.AnnualHistory.Count)
                    ? fullView
                    : TailRaw(keepEv, keepAn);
                return projectView ? ProjectViewForBudget(tv) : tv;
            }

            foreach (int pruneCap in pruneCaps)
            {
                object? s4Obj = fullStage4 is null ? null : stage4WithPruneCap(pruneCap);
                bool pruneTrimmed = pruneCap < fullPruneCount;

                // Check if full history fits under this pruneCap.
                object fullV = MakeView(fullView.Events.Count, fullView.AnnualHistory.Count);
                byte[] fullB = Serialize(fullV, scenesObj, s4Obj, pruneTrimmed, projCompact, s4Compact, exceeded: false);
                if (fullB.Length <= MaxViewPayloadBytes)
                {
                    fit = new BoundedMultiSnapshot(fullV, scenesObj, s4Obj, projEco, pruneTrimmed, totalRecordedYears, projCompact, s4Compact, false);
                    fitBytes = fullB;
                    return true;
                }

                // Check if minimum guaranteed tail (at least 1 event & 1 annual record whenever present) fits under this pruneCap.
                object minV = MakeView(minEvents, minAnnual);
                bool minTrunc = minEvents < fullView.Events.Count || minAnnual < fullView.AnnualHistory.Count || pruneTrimmed;
                byte[] minB = Serialize(minV, scenesObj, s4Obj, minTrunc, projCompact, s4Compact, exceeded: false);
                if (minB.Length > MaxViewPayloadBytes) continue;

                // Binary-search largest tail keep in [1, maxHist) that fits <= MaxViewPayloadBytes.
                int lo = 1;
                int hi = Math.Max(2, maxHist);
                object bestV = minV;
                byte[] bestB = minB;
                bool bestTrunc = minTrunc;
                while (lo + 1 < hi)
                {
                    int mid = lo + ((hi - lo) / 2);
                    int kEv = Math.Min(fullView.Events.Count, Math.Max(minEvents, mid));
                    int kAn = Math.Min(fullView.AnnualHistory.Count, Math.Max(minAnnual, mid));
                    object candV = MakeView(kEv, kAn);
                    bool candTrunc = kEv < fullView.Events.Count || kAn < fullView.AnnualHistory.Count || pruneTrimmed;
                    byte[] candB = Serialize(candV, scenesObj, s4Obj, candTrunc, projCompact, s4Compact, exceeded: false);
                    if (candB.Length <= MaxViewPayloadBytes)
                    {
                        lo = mid;
                        bestV = candV;
                        bestB = candB;
                        bestTrunc = candTrunc;
                    }
                    else
                    {
                        hi = mid;
                    }
                }
                fit = new BoundedMultiSnapshot(bestV, scenesObj, s4Obj, projEco, bestTrunc, totalRecordedYears, projCompact, s4Compact, false);
                fitBytes = bestB;
                return true;
            }

            return false;
        }

        // Tier 0: full uncompacted view/scenes/stage4 with full history (used whenever raw <= 512 KiB and ecology <= 150 KiB).
        if (ecoBytes <= MaxEcologyPayloadBytes)
        {
            byte[] tier0Bytes = Serialize(fullView, fullScenes, fullStage4, histTrunc: false, projCompact: false, s4Compact: false, exceeded: false);
            if (tier0Bytes.Length <= MaxViewPayloadBytes)
            {
                finalBytes = tier0Bytes;
                return new BoundedMultiSnapshot(fullView, fullScenes, fullStage4, projEco,
                    HistoryTruncated: false, TotalRecordedYears: totalRecordedYears,
                    ProjectionCompacted: false, Stage4Compacted: false, PayloadBudgetExceeded: false);
            }
        }

        // Tier 1: honest non-living/display-precision projection compaction on view/scenes/stage4.trees/fallenDeadwood,
        // keeping FULL untouched stage4.areas[].ground. Preserves full history first, then trims stage4.pruneLog before
        // dropping below 1 latest event & 1 latest annual record (monotonic: history is never dropped while dead-tree
        // compaction alone keeps full history within budget).
        var projScenes = ProjectScenesForBudget(fullScenes);
        if (TryFitTier(projectView: true, projScenes, projCompact: true, s4Compact: false,
            pruneCap => fullStage4 is null ? null
                : ProjectStage4ForBudget(fullStage4, hasEcology: projEco is not null, compactGround: false, pruneLogCap: pruneCap),
            out var tier1Fit, out var tier1Bytes))
        {
            finalBytes = tier1Bytes!;
            return tier1Fit!;
        }

        // Tier 2: honest stage4.areas[].ground compaction (omits contributingTreeIds via null omission — never [] — and
        // preserves all four vertex field names { xm, zm, u, v } rounded to 3 decimals; explicitly sets stage4Compacted=true).
        if (TryFitTier(projectView: true, projScenes, projCompact: true, s4Compact: true,
            pruneCap => fullStage4 is null ? null
                : ProjectStage4ForBudget(fullStage4, hasEcology: projEco is not null, compactGround: true, pruneLogCap: pruneCap),
            out var tier2Fit, out var tier2Bytes))
        {
            finalBytes = tier2Bytes!;
            return tier2Fit!;
        }

        // Tier 3: explicit over-budget disclosure — never drops below minEvents/minAnnual and never returns over-budget
        // bytes silently. Keeps at least the latest work event & latest annual record whenever present, sets
        // payloadBudgetExceeded=true, and logs loudly to server stderr.
        object? overStage4 = fullStage4 is null ? null
            : ProjectStage4ForBudget(fullStage4, hasEcology: projEco is not null, compactGround: true, pruneLogCap: 0);
        var overView = ProjectViewForBudget(TailRaw(minEvents, minAnnual));
        bool overTrunc = minEvents < fullView.Events.Count || minAnnual < fullView.AnnualHistory.Count || (fullStage4?.PruneLog.Count ?? 0) > 0;
        byte[] overBytes = Serialize(overView, projScenes, overStage4, overTrunc, projCompact: true, s4Compact: fullStage4 is not null, exceeded: true);
        Console.Error.WriteLine(FormattableString.Invariant(
            $"PAYLOAD BUDGET EXCEEDED GET api/multi/view (year={fullView.Source.EcologicalYear}): raw={overBytes.Length} B (limit {MaxViewPayloadBytes}), ecology={ecoBytes} B (limit {MaxEcologyPayloadBytes})"));
        finalBytes = overBytes;
        return new BoundedMultiSnapshot(overView, projScenes, overStage4, projEco,
            HistoryTruncated: overTrunc, TotalRecordedYears: totalRecordedYears,
            ProjectionCompacted: true, Stage4Compacted: fullStage4 is not null, PayloadBudgetExceeded: true);
    }

    // Honest projection of MultiAreaSessionView: keeps every area, living tree, stump and dead tree with exact field names;
    // omits "death": null on living trees and redundant death.treeId on dead trees; carries Core's exact LifeStates.Of(tr)
    // on dead trees so the inspector renders identical engine LifeState across tiers; rounds dead-tree display floats to 3 decimals.
    private static MultiProjectedSessionViewDto ProjectViewForBudget(MultiAreaSessionView view)
    {
        var areas = new MultiProjectedAreaViewDto[view.Areas.Count];
        for (int a = 0; a < view.Areas.Count; a++)
        {
            var area = view.Areas[a];
            var trees = new MultiProjectedTreeDto[area.Trees.Count];
            for (int t = 0; t < area.Trees.Count; t++)
            {
                var tr = area.Trees[t];
                if (tr.IsLiving)
                {
                    trees[t] = new MultiProjectedTreeDto(tr.TreeId, tr.XM, tr.ZM, tr.AgeYears, tr.DbhCm, tr.HeightM,
                        tr.CrownRadiusM, IsLiving: true, Death: null);
                }
                else
                {
                    var d = tr.Death!;
                    trees[t] = new MultiProjectedTreeDto(tr.TreeId, R3f(tr.XM), R3f(tr.ZM), tr.AgeYears, R3f(tr.DbhCm),
                        R3f(tr.HeightM), R3f(tr.CrownRadiusM), IsLiving: false,
                        new MultiProjectedDeathDto(d.EcologicalYear, d.Cause, d.Mechanism, LifeStates.Of(tr)));
                }
            }
            areas[a] = new MultiProjectedAreaViewDto(area.AreaId, area.Label, area.StockingNote, area.Site, area.SimulationSeed,
                area.AreaHa, area.AreaM2Exact, area.Polygon, area.Extent, Array.AsReadOnly(trees), area.Stand, area.CropTreeIds);
        }
        return new MultiProjectedSessionViewDto(view.SessionId, view.PlanStateRevision, view.Source, view.Experiment,
            Array.AsReadOnly(areas), view.Pending, view.Approved, view.Events, view.AnnualHistory);
    }

    // Honest projection of scenes: keeps every living tree, stump and dead tree with exact map position, radius, paint order,
    // status, deathYear (when dead) and isCrop (when Crop); rounds display coords to 3 decimals and omits unused fields.
    private static IReadOnlyDictionary<string, IReadOnlyList<MultiProjectedMapItemDto>> ProjectScenesForBudget(
        IReadOnlyDictionary<string, IReadOnlyList<PlayerMapItem>> scenes)
    {
        var result = new Dictionary<string, IReadOnlyList<MultiProjectedMapItemDto>>(scenes.Count, StringComparer.Ordinal);
        foreach (var (areaId, items) in scenes)
        {
            var arr = new MultiProjectedMapItemDto[items.Count];
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                arr[i] = new MultiProjectedMapItemDto(item.TreeId, item.PaintOrder, R3(item.U), R3(item.V), R3(item.RadiusM),
                    item.Status, item.DeathYear, item.IsCrop);
            }
            result[areaId] = Array.AsReadOnly(arr);
        }
        return result;
    }

    // Honest projection of stage4 readout: keeps all living trees plus any non-living tree with pruningLifts > 0 (unpruned dead
    // trees are omitted; UI reads Core's exact LifeState from tree.death.lifeState), rounds deadwood floats to 3 decimals,
    // and when compactGround is true omits contributingTreeIds via null omission (NEVER fake []) while preserving { xm, zm, u, v }.
    private static MultiProjectedStage4Dto ProjectStage4ForBudget(
        MultiAreaUiReadout stage4, bool hasEcology, bool compactGround, int pruneLogCap)
    {
        var compactAreas = new MultiProjectedStage4AreaDto[stage4.Areas.Count];
        for (int a = 0; a < stage4.Areas.Count; a++)
        {
            var area = stage4.Areas[a];
            var dwList = area.Canopy.FallenDeadwood;
            var dwProj = new MultiProjectedDeadwoodDto[dwList.Count];
            for (int d = 0; d < dwList.Count; d++)
            {
                var dw = dwList[d];
                dwProj[d] = new MultiProjectedDeadwoodDto(dw.DeadwoodId, dw.TreeId, R3f(dw.XM), R3f(dw.ZM), R3(dw.U), R3(dw.V),
                    dw.DeathYear, dw.Cause, R3f(dw.DbhCm), R3f(dw.HeightM), R3f(dw.EstimatedVolumeM3));
            }
            var canopyProj = new MultiProjectedCanopySummaryDto(
                area.Canopy.PolygonAreaM2, area.Canopy.GeometricCoverFraction, area.Canopy.GeometricCoveredM2,
                area.Canopy.GeometricUncoveredM2, area.Canopy.OpeningCount, area.Canopy.LargestOpeningAreaM2,
                area.Canopy.LargestOpeningWidthM, area.Canopy.RelativeDensity, area.Canopy.AnnualNaturalDeaths,
                area.Canopy.CumulativeNaturalDeaths, area.Canopy.FallenDeadwoodCount, area.Canopy.MeanLightIndex,
                area.Canopy.MinLightIndex, area.Canopy.MaxLightIndex, Array.AsReadOnly(dwProj));

            var trees = new List<MultiProjectedUiTreeDto>(area.Trees.Count);
            for (int t = 0; t < area.Trees.Count; t++)
            {
                var tr = area.Trees[t];
                if (tr.LifeState != LifeState.Living && tr.PruningLifts == 0) continue;
                trees.Add(new MultiProjectedUiTreeDto(
                    tr.TreeId, tr.LifeState, tr.DeathCause, tr.DeathYear,
                    tr.PruningLifts, tr.MaxPruningLifts, tr.CrownBaseHeightM, tr.LastPruningYear,
                    tr.PruneEligible, hasEcology ? null : tr.PruneRefusalCode, hasEcology ? null : tr.PruneRefusalReason,
                    tr.NextPruneTargetM, tr.NextPruneEligibleYear, tr.PrunePlanState));
            }

            object groundObj;
            if (!compactGround)
            {
                groundObj = area.Ground;
            }
            else
            {
                var ground = new MultiProjectedGroundCellDto[area.Ground.Count];
                for (int g = 0; g < area.Ground.Count; g++)
                {
                    var cell = area.Ground[g];
                    var poly = new MultiProjectedGroundVertexDto[cell.Polygon.Count];
                    for (int p = 0; p < cell.Polygon.Count; p++)
                    {
                        var v = cell.Polygon[p];
                        poly[p] = new MultiProjectedGroundVertexDto(R3(v.XM), R3(v.ZM), R3(v.U), R3(v.V));
                    }
                    ground[g] = new MultiProjectedGroundCellDto(
                        cell.Column, cell.Row, R3f(cell.CentreXM), R3f(cell.CentreZM),
                        R3(cell.CentreU), R3(cell.CentreV), R3(cell.ClippedAreaM2),
                        Array.AsReadOnly(poly), R3f(cell.LightIndex), R3(cell.GeometricCoveredFraction),
                        cell.CrossBorderTrees, ContributingTreeIds: null);
                }
                groundObj = Array.AsReadOnly(ground);
            }

            compactAreas[a] = new MultiProjectedStage4AreaDto(area.AreaId, canopyProj, trees.AsReadOnly(), groundObj);
        }
        return new MultiProjectedStage4Dto(
            stage4.SessionId, stage4.EcologicalYear, stage4.PlanStateRevision, stage4.Source, stage4.Models,
            Array.AsReadOnly(compactAreas), stage4.PendingPrunes, stage4.ApprovedPrunes, TailOf(stage4.PruneLog, pruneLogCap));
    }

    private static BoundedMultiSnapshot BoundSnapshotForResponse(MultiAreaCompositeUiSnapshot snapshot, int totalRecordedYears,
        bool economyEnabled = false, MultiEconomySnapshotDto? economy = null) =>
        SerializeBoundedMultiView(snapshot.View, snapshot.Readout, snapshot.Ecology, totalRecordedYears, reviewPreview: null, out _, economyEnabled, economy);

    private static BoundedMultiSnapshot BoundSnapshotForResponse(
        MultiAreaSessionView view, MultiAreaUiReadout? stage4, MultiAreaEcologyDto? ecology, int totalRecordedYears,
        bool economyEnabled = false, MultiEconomySnapshotDto? economy = null) =>
        SerializeBoundedMultiView(view, stage4, ecology, totalRecordedYears, reviewPreview: null, out _, economyEnabled, economy);

    private static object BoundSessionViewForAux(MultiAreaSessionView view)
    {
        const int AuxTailCap = 20;
        var trimmed = (view.Events.Count <= AuxTailCap && view.AnnualHistory.Count <= AuxTailCap)
            ? view
            : view with
            {
                Events = TailOf(view.Events, AuxTailCap),
                AnnualHistory = TailOf(view.AnnualHistory, AuxTailCap),
            };
        int deadCount = 0;
        foreach (var area in trimmed.Areas)
        {
            foreach (var tr in area.Trees)
            {
                if (!tr.IsLiving) deadCount++;
            }
        }
        return deadCount > 50 ? ProjectViewForBudget(trimmed) : trimmed;
    }

    private static IReadOnlyList<T> TailOf<T>(IReadOnlyList<T> list, int maxCount)
    {
        if (maxCount <= 0) return Array.Empty<T>();
        if (list.Count <= maxCount) return list;
        int start = list.Count - maxCount;
        var copy = new T[maxCount];
        for (int i = 0; i < maxCount; i++) copy[i] = list[start + i];
        return Array.AsReadOnly(copy);
    }

    private static async Task WriteRawJsonAsync(HttpContext context, int status, byte[] bytes)
    {
        context.Response.StatusCode = status;
        context.Response.ContentType = JsonContentType;
        context.Response.ContentLength = bytes.Length;
        await context.Response.Body.WriteAsync(bytes, context.RequestAborted);
    }

    // Strictly read-only cell detail inspection (spec §5.5): full cell cohorts, undergrowth, light, cover, clear state
    // plus fresh composite view/scenes/stage4/ecology. 400 on malformed JSON/unknown keys; 409 on stale fence or invalid cell.
    private async Task ReadMultiCellAsync(HttpContext context)
    {
        var request = await ReadMultiRequestAsync<MultiCellRequest>(context, MultiCellMembers);
        if (request is null) return;
        if (request.Column is not int column || request.Row is not int row)
        {
            await WriteErrorAsync(context, StatusCodes.Status400BadRequest, "BadRequest",
                "cell inspection requires integer column and row; nothing changed");
            return;
        }
        MultiAreaCellDetailResult result;
        int totalRecordedYears;
        bool economyEnabled;
        MultiEconomySnapshotDto? economy;
        lock (multiSwapSync)
        {
            var pair = CurrentMulti;
            result = pair.Adapter.ReadCellDetail(request.Fence!, request.AreaId!, column, row);
            totalRecordedYears = pair.Session.ReadHistoryIndex(pageSize: 1).TotalRecordedYears;
            economyEnabled = pair.Session.SelectedModels.IsEconomyEnabled;
            economy = TryBuildEconomySnapshot(pair.Session, result.Snapshot.View);
        }
        var bounded = BoundSnapshotForResponse(result.Snapshot, totalRecordedYears, economyEnabled, economy);
        await WriteJsonAsync(context, StatusOf(result.RefusalCode), new MultiCellPayload(bounded.View, bounded.Scenes,
            result.Cell, NoticeOf(result.RefusalCode, result.RefusalMessage), bounded.Stage4, bounded.Ecology,
            bounded.HistoryTruncated, bounded.TotalRecordedYears, bounded.ProjectionCompacted, bounded.Stage4Compacted,
            bounded.PayloadBudgetExceeded, economyEnabled, economy));
    }

    private async Task ReadMultiCompetitionAsync(HttpContext context)
    {
        var request = await ReadMultiRequestAsync<MultiTreeRequest>(context, MultiTreeMembers);
        if (request is null) return;
        MultiAreaCompetitionResult result;
        MultiAreaCompositeUiReadout? composite;
        int totalRecordedYears;
        bool economyEnabled;
        MultiEconomySnapshotDto? economy;
        lock (multiSwapSync)
        {
            var pair = CurrentMulti;
            result = pair.Adapter.Competition(request.Fence!, request.AreaId!, request.TreeId!);
            composite = pair.Adapter.CompositeFor(result.View);
            totalRecordedYears = pair.Session.ReadHistoryIndex(pageSize: 1).TotalRecordedYears;
            economyEnabled = pair.Session.SelectedModels.IsEconomyEnabled;
            economy = TryBuildEconomySnapshot(pair.Session, result.View);
        }
        if (composite is not null)
        {
            var bounded = BoundSnapshotForResponse(result.View, composite.Stage4, composite.Ecology, totalRecordedYears, economyEnabled, economy);
            await WriteJsonAsync(context, StatusOf(result.RefusalCode), new MultiCompetitionPayload(bounded.View, bounded.Scenes,
                result.AreaId, result.Competition, NoticeOf(result.RefusalCode, result.RefusalMessage),
                bounded.Stage4, bounded.Ecology, bounded.HistoryTruncated, bounded.TotalRecordedYears,
                bounded.ProjectionCompacted, bounded.Stage4Compacted, bounded.PayloadBudgetExceeded, economyEnabled, economy));
        }
        else
        {
            await WriteJsonAsync(context, StatusOf(result.RefusalCode), new MultiCompetitionPayload(result.View, ScenesOf(result.View),
                result.AreaId, result.Competition, NoticeOf(result.RefusalCode, result.RefusalMessage),
                null, null, false, totalRecordedYears, EconomyEnabled: economyEnabled, Economy: economy));
        }
    }

    private async Task MultiHitTestAsync(HttpContext context)
    {
        var request = await ReadMultiRequestAsync<MultiHitRequest>(context, MultiHitMembers);
        if (request is null) return;
        MultiAreaHitResult result;
        MultiAreaCompositeUiReadout? composite;
        int totalRecordedYears;
        bool economyEnabled;
        MultiEconomySnapshotDto? economy;
        lock (multiSwapSync)
        {
            var pair = CurrentMulti;
            result = pair.Adapter.HitTest(request.Fence!, request.AreaId!, request.U!.Value, request.V!.Value);
            composite = pair.Adapter.CompositeFor(result.View);
            totalRecordedYears = pair.Session.ReadHistoryIndex(pageSize: 1).TotalRecordedYears;
            economyEnabled = pair.Session.SelectedModels.IsEconomyEnabled;
            economy = TryBuildEconomySnapshot(pair.Session, result.View);
        }
        if (composite is not null)
        {
            var bounded = BoundSnapshotForResponse(result.View, composite.Stage4, composite.Ecology, totalRecordedYears, economyEnabled, economy);
            await WriteJsonAsync(context, StatusOf(result.RefusalCode), new MultiHitPayload(bounded.View, bounded.Scenes,
                result.AreaId, result.Candidates, NoticeOf(result.RefusalCode, result.RefusalMessage),
                bounded.Stage4, bounded.Ecology, bounded.HistoryTruncated, bounded.TotalRecordedYears,
                bounded.ProjectionCompacted, bounded.Stage4Compacted, bounded.PayloadBudgetExceeded, economyEnabled, economy));
        }
        else
        {
            await WriteJsonAsync(context, StatusOf(result.RefusalCode), new MultiHitPayload(result.View, ScenesOf(result.View),
                result.AreaId, result.Candidates, NoticeOf(result.RefusalCode, result.RefusalMessage),
                null, null, false, totalRecordedYears, EconomyEnabled: economyEnabled, Economy: economy));
        }
    }

    // ---- Phase 2B-2a Economy Server-Authoritative Projection & Formatting (zero JS math; omitted when Off) -------------

    private static string FormatEconomyDisclaimer(EconomyOptions options) =>
        "Calibrated teaching estimates (ex VAT; not an empirical national tariff or farm-forestry balance sheet). " +
        $"Small cuts on small woods can net a loss under the {EconomyDefaults.FormatEuros(options.MinimumHarvestJobCents)} contractor harvest minimum floor. " +
        "No automatic grants are modelled. Felled timber is priced and settled through the sell-and-extract engine path; " +
        "retaining timber for own use (dispositions) is deferred.";

    private const string EconomySaveBlockedNote =
        "Saving and loading Economy sessions require Save v3 (deferred to Phase 2B-2b). Switch Money Off to use Save v2 slots.";

    private const string EconomyFellContractorTooltip =
        "Commercial production felling is contractor-only — not ordinary owner DIY. " +
        "Felled timber is priced and settled through the sell-and-extract path (dispositions deferred).";

    private static MultiEconomySnapshotDto? TryBuildEconomySnapshot(MultiAreaContinuation session, MultiAreaSessionView view)
    {
        if (!session.SelectedModels.IsEconomyEnabled) return null;
        try
        {
            var econView = session.ReadEconomyView();
            if (econView.Source != view.Source || econView.PlanStateRevision != view.PlanStateRevision)
                return null;
            EconomyWorkPlanQuote? quote = null;
            bool frozen = false;
            if (econView.ApprovedQuote is not null)
            {
                quote = econView.ApprovedQuote.Quote;
                frozen = true;
            }
            else
            {
                var pendingQuote = session.QuoteWorkPlan(FenceOf(view));
                bool hasPending = pendingQuote.HarvestJob is not null || pendingQuote.NonHarvestLines.Count > 0;
                if (hasPending) quote = pendingQuote;
            }
            return BuildEconomySnapshot(econView, quote, frozen);
        }
        catch (P1Refusal)
        {
            return null;
        }
    }

    private static MultiEconomySnapshotDto BuildEconomySnapshot(
        MultiAreaEconomyView econView, EconomyWorkPlanQuote? activeQuote, bool approvedQuoteFrozen)
    {
        var nextStage = EconomyDefaults.AutoHarvestStage(econView.ExecutedHarvestPlansCount);
        long nextRate = EconomyDefaults.HarvestRateCentsPerTonne(nextStage);
        var executors = econView.PendingOrderExecutors
            .OrderBy(kv => kv.Key)
            .Select(kv => new MultiEconomyExecutorEntryDto(
                kv.Key.Kind.ToString(), kv.Key.AreaId, kv.Key.TargetKey, kv.Value.ToString()))
            .ToArray();
        var latestSettlement = econView.Settlements.Count > 0
            ? ProjectEconomySettlement(econView.Settlements[^1], econView.Options)
            : null;
        string annualAllowanceHours = FormatHoursValue(econView.Options.AnnualLandownerAllowanceMinutes) + " hrs";
        string allowanceTooltip = FormattableString.Invariant(
            $"Remaining annual landowner hours ({annualAllowanceHours}/yr at {EconomyDefaults.FormatEuros(0L)} cash wage; resets on Advance 1 Year)");
        return new MultiEconomySnapshotDto(
            Enabled: true,
            CashBalanceCents: econView.CashBalanceCents,
            CashBalanceDisplay: EconomyDefaults.FormatEuros(econView.CashBalanceCents),
            OpeningCashCents: econView.Options.OpeningCashCents,
            OpeningCashDisplay: EconomyDefaults.FormatEuros(econView.Options.OpeningCashCents),
            UsedLandownerMinutesThisYear: econView.UsedLandownerMinutesThisYear,
            RemainingLandownerMinutes: econView.RemainingLandownerMinutes,
            AnnualLandownerAllowanceMinutes: econView.Options.AnnualLandownerAllowanceMinutes,
            AnnualLandownerAllowanceDisplay: annualAllowanceHours,
            LandownerAllowanceTooltip: allowanceTooltip,
            LandownerHoursDisplay: FormatHoursOfAllowance(econView.RemainingLandownerMinutes, econView.Options.AnnualLandownerAllowanceMinutes),
            UsedLandownerHoursDisplay: FormatMinutesAndHours(econView.UsedLandownerMinutesThisYear),
            RemainingLandownerHoursDisplay: FormatMinutesAndHours(econView.RemainingLandownerMinutes),
            ExecutedHarvestPlansCount: econView.ExecutedHarvestPlansCount,
            NextHarvestStage: nextStage.ToString(),
            NextHarvestStageLabel: FormatStageLabel(nextStage, nextRate),
            MinimumHarvestFloorCents: econView.Options.MinimumHarvestJobCents,
            MinimumHarvestFloorDisplay: EconomyDefaults.FormatEuros(econView.Options.MinimumHarvestJobCents),
            ContractorHourlyRateDisplay: EconomyDefaults.FormatEuros(econView.Options.ContractorHourlyRateCents) + "/hr",
            PendingOrderExecutors: Array.AsReadOnly(executors),
            ActiveQuote: activeQuote is null ? null : ProjectEconomyQuote(activeQuote, econView.Options),
            ApprovedQuoteFrozen: approvedQuoteFrozen,
            LatestSettlement: latestSettlement,
            FellContractorTooltip: EconomyFellContractorTooltip,
            SaveBlockedCode: "SaveEconomyRequiresV3",
            SaveBlockedNote: EconomySaveBlockedNote,
            Disclaimer: FormatEconomyDisclaimer(econView.Options),
            EvidenceLabels: econView.EvidenceLabels);
    }

    private static MultiEconomyQuoteDto ProjectEconomyQuote(EconomyWorkPlanQuote quote, EconomyOptions? options = null)
    {
        var issues = quote.Issues.Select(issue => new MultiEconomyIssueDto(
            issue.Code, issue.Message, PlainIssueExplanation(issue, quote, options), issue.Required, issue.Available)).ToArray();
        bool upfrontCashBlocked = quote.Issues.Any(i => string.Equals(i.Code, "EconomyInsufficientCash", StringComparison.Ordinal));
        string verdict = quote.IsEligible
            ? $"Upfront cash covered: {EconomyDefaults.FormatEuros(quote.AvailableCashBeforeCents)} available for {EconomyDefaults.FormatEuros(quote.UpfrontCashRequiredCents)} required external costs."
            : issues[0].PlainExplanation;
        var lines = quote.NonHarvestLines.Select(line => new MultiEconomyNonHarvestLineDto(
            Kind: line.OrderKey.Kind.ToString(),
            AreaId: line.OrderKey.AreaId,
            TargetKey: line.OrderKey.TargetKey,
            Executor: line.Executor.ToString(),
            PersonMinutes: line.PersonMinutes,
            TimeDisplay: FormatMinutesAndHours(line.PersonMinutes),
            OwnerMinutes: line.OwnerMinutes,
            ContractorMinutes: line.ContractorMinutes,
            ContractorLabourCents: line.ContractorLabourCents,
            ContractorLabourDisplay: EconomyDefaults.FormatEuros(line.ContractorLabourCents),
            MaterialId: line.MaterialId,
            MaterialLabel: FormatMaterialLabel(line.MaterialId, line.MaterialCostCents),
            MaterialCostCents: line.MaterialCostCents,
            MaterialCostDisplay: EconomyDefaults.FormatEuros(line.MaterialCostCents),
            TotalExternalCostCents: line.TotalExternalCostCents,
            TotalExternalCostDisplay: EconomyDefaults.FormatEuros(line.TotalExternalCostCents))).ToArray();
        return new MultiEconomyQuoteDto(
            PlanStateRevision: quote.PlanStateRevision,
            EcologicalYear: quote.EcologicalYear,
            IsEligible: quote.IsEligible,
            UpfrontCashBlocked: upfrontCashBlocked,
            UpfrontCashVerdict: verdict,
            AvailableCashBeforeCents: quote.AvailableCashBeforeCents,
            AvailableCashBeforeDisplay: EconomyDefaults.FormatEuros(quote.AvailableCashBeforeCents),
            AvailableLandownerMinutesBefore: quote.AvailableLandownerMinutesBefore,
            AvailableLandownerHoursBeforeDisplay: FormatMinutesAndHours(quote.AvailableLandownerMinutesBefore),
            HarvestJob: quote.HarvestJob is null ? null : ProjectEconomyHarvestQuote(quote.HarvestJob),
            NonHarvestLines: Array.AsReadOnly(lines),
            ContractorHarvestCostCents: quote.ContractorHarvestCostCents,
            ContractorHarvestCostDisplay: EconomyDefaults.FormatEuros(quote.ContractorHarvestCostCents),
            ContractorNonHarvestLabourCents: quote.ContractorNonHarvestLabourCents,
            ContractorNonHarvestLabourDisplay: EconomyDefaults.FormatEuros(quote.ContractorNonHarvestLabourCents),
            MaterialCostCents: quote.MaterialCostCents,
            MaterialCostDisplay: EconomyDefaults.FormatEuros(quote.MaterialCostCents),
            TotalExternalCostCents: quote.TotalExternalCostCents,
            TotalExternalCostDisplay: EconomyDefaults.FormatEuros(quote.TotalExternalCostCents),
            UpfrontCashRequiredCents: quote.UpfrontCashRequiredCents,
            UpfrontCashRequiredDisplay: EconomyDefaults.FormatEuros(quote.UpfrontCashRequiredCents),
            TimberSaleRevenueCents: quote.TimberSaleRevenueCents,
            TimberSaleRevenueDisplay: EconomyDefaults.FormatEuros(quote.TimberSaleRevenueCents),
            NetCashDeltaCents: quote.NetCashDeltaCents,
            NetCashDeltaDisplay: FormatSignedEuros(quote.NetCashDeltaCents),
            NetCashIsLoss: quote.NetCashDeltaCents < 0L,
            TotalOwnerMinutesRequired: quote.TotalOwnerMinutesRequired,
            TotalOwnerHoursRequiredDisplay: FormatMinutesAndHours(quote.TotalOwnerMinutesRequired),
            TotalContractorMinutes: quote.TotalContractorMinutes,
            TotalContractorHoursDisplay: FormatMinutesAndHours(quote.TotalContractorMinutes),
            Issues: Array.AsReadOnly(issues),
            EvidenceLabels: quote.EvidenceLabels);
    }

    private static MultiEconomyHarvestQuoteDto ProjectEconomyHarvestQuote(EconomyHarvestQuote h)
    {
        bool floorApplied = h.MinimumJobAdjustmentCents > 0L;
        string floorHighlight = floorApplied
            ? $"{EconomyDefaults.FormatEuros(h.MinimumJobFloorCents)} minimum contractor job floor applied (variable {EconomyDefaults.FormatEuros(h.VariableHarvestCostCents)} + {EconomyDefaults.FormatEuros(h.MinimumJobAdjustmentCents)} adjustment = {EconomyDefaults.FormatEuros(h.TotalHarvestContractorCostCents)})."
            : $"Variable harvest fee ({EconomyDefaults.FormatEuros(h.VariableHarvestCostCents)}) exceeds the {EconomyDefaults.FormatEuros(h.MinimumJobFloorCents)} minimum job floor.";
        var assortments = h.Assortments.Select(a => new MultiEconomyAssortmentDto(
            SpeciesId: a.SpeciesId,
            SpeciesLabel: FormatSpeciesLabel(a.SpeciesId),
            Assortment: a.Assortment.ToString(),
            AssortmentLabel: FormatAssortmentLabel(a.Assortment),
            PieceCount: a.PieceCount,
            VolumeCm3: a.VolumeCm3,
            VolumeDisplay: FormatVolumeM3(a.VolumeCm3),
            GreenGrams: a.GreenGrams,
            TonnesDisplay: FormatTonnes(a.GreenGrams),
            UnitPriceCentsPerTonne: a.UnitPriceCentsPerTonne,
            UnitPriceDisplay: EconomyDefaults.FormatEuros(a.UnitPriceCentsPerTonne) + "/t",
            RevenueCents: a.RevenueCents,
            RevenueDisplay: EconomyDefaults.FormatEuros(a.RevenueCents))).ToArray();
        return new MultiEconomyHarvestQuoteDto(
            Stage: h.Stage.ToString(),
            StageLabel: FormatStageLabel(h.Stage, h.StageRateCentsPerTonne),
            StageRateCentsPerTonne: h.StageRateCentsPerTonne,
            StageRateDisplay: EconomyDefaults.FormatEuros(h.StageRateCentsPerTonne) + "/t",
            FelledStemCount: h.FelledStemCount,
            TotalStemVolumeDisplay: FormatVolumeM3(h.TotalStemVolumeCm3),
            SaleVolumeDisplay: FormatVolumeM3(h.SaleVolumeCm3),
            ResidualVolumeDisplay: FormatVolumeM3(h.ResidualVolumeCm3),
            HarvestedTonnesDisplay: FormatTonnes(h.HarvestedGreenGrams),
            VariableHarvestCostCents: h.VariableHarvestCostCents,
            VariableHarvestCostDisplay: EconomyDefaults.FormatEuros(h.VariableHarvestCostCents),
            MinimumJobFloorCents: h.MinimumJobFloorCents,
            MinimumJobFloorDisplay: EconomyDefaults.FormatEuros(h.MinimumJobFloorCents),
            MinimumJobAdjustmentCents: h.MinimumJobAdjustmentCents,
            MinimumJobAdjustmentDisplay: EconomyDefaults.FormatEuros(h.MinimumJobAdjustmentCents),
            MinimumJobFloorApplied: floorApplied,
            MinimumJobFloorHighlight: floorHighlight,
            TotalHarvestContractorCostCents: h.TotalHarvestContractorCostCents,
            TotalHarvestContractorCostDisplay: EconomyDefaults.FormatEuros(h.TotalHarvestContractorCostCents),
            TimberSaleRevenueCents: h.TimberSaleRevenueCents,
            TimberSaleRevenueDisplay: EconomyDefaults.FormatEuros(h.TimberSaleRevenueCents),
            Assortments: Array.AsReadOnly(assortments));
    }

    private static MultiEconomySettlementDto ProjectEconomySettlement(EconomySettlementReceipt r, EconomyOptions? options = null)
    {
        var entries = r.LedgerEntries.Select(e => ProjectEconomyLedgerEntry(e, options)).ToArray();
        return new MultiEconomySettlementDto(
            ExecutionKey: r.ExecutionKey,
            PlanId: r.PlanId,
            EcologicalYear: r.EcologicalYear,
            WorkRevision: r.WorkRevision,
            CashBeforeCents: r.CashBeforeCents,
            CashBeforeDisplay: EconomyDefaults.FormatEuros(r.CashBeforeCents),
            TotalExternalCostCents: r.TotalExternalCostCents,
            TotalExternalCostDisplay: EconomyDefaults.FormatEuros(r.TotalExternalCostCents),
            TimberSaleRevenueCents: r.TimberSaleRevenueCents,
            TimberSaleRevenueDisplay: EconomyDefaults.FormatEuros(r.TimberSaleRevenueCents),
            NetCashDeltaCents: r.NetCashDeltaCents,
            NetCashDeltaDisplay: FormatSignedEuros(r.NetCashDeltaCents),
            CashAfterCents: r.CashAfterCents,
            CashAfterDisplay: EconomyDefaults.FormatEuros(r.CashAfterCents),
            OwnerMinutesBefore: r.OwnerMinutesBefore,
            OwnerMinutesConsumed: r.OwnerMinutesConsumed,
            OwnerMinutesAfter: r.OwnerMinutesAfter,
            OwnerHoursBeforeDisplay: FormatMinutesAndHours(r.OwnerMinutesBefore),
            OwnerHoursConsumedDisplay: FormatMinutesAndHours(r.OwnerMinutesConsumed),
            OwnerHoursAfterDisplay: FormatMinutesAndHours(r.OwnerMinutesAfter),
            HarvestJob: r.HarvestJob is null ? null : ProjectEconomyHarvestQuote(r.HarvestJob),
            LedgerEntries: Array.AsReadOnly(entries));
    }

    private static MultiEconomyLedgerEntryDto ProjectEconomyLedgerEntry(EconomyLedgerEntry e, EconomyOptions? options = null) => new(
        Category: e.Category.ToString(),
        CategoryLabel: FormatLedgerCategoryLabel(e.Category, options),
        ReferenceId: e.ReferenceId,
        CashDeltaCents: e.CashDeltaCents,
        CashDeltaDisplay: FormatSignedEuros(e.CashDeltaCents),
        EcologicalYear: e.EcologicalYear,
        WorkRevision: e.WorkRevision,
        ExecutionKey: e.ExecutionKey,
        PlanId: e.PlanId);

    private static MultiEconomyHistoryYearDto ProjectEconomyHistoryYear(EconomyYearHistory h, EconomyOptions? options = null) => new(
        Year: h.Year,
        EconomyActive: h.EconomyActive,
        Provenance: h.Provenance.ToString(),
        ProvenanceLabel: h.ProvenanceLabel,
        OpeningCashCents: h.OpeningCashCents,
        ClosingCashCents: h.ClosingCashCents,
        TotalExternalCostCents: h.TotalExternalCostCents,
        TotalTimberRevenueCents: h.TotalTimberRevenueCents,
        NetCashDeltaCents: h.NetCashDeltaCents,
        LandownerMinutesUsed: h.LandownerMinutesUsed,
        OpeningCashDisplay: h.OpeningCashDisplay,
        ClosingCashDisplay: h.ClosingCashDisplay,
        TotalExternalCostDisplay: h.TotalExternalCostDisplay,
        TotalTimberRevenueDisplay: h.TotalTimberRevenueDisplay,
        NetCashDeltaDisplay: h.NetCashDeltaCents is long net ? FormatSignedEuros(net) : h.NetCashDeltaDisplay,
        LandownerMinutesUsedDisplay: h.LandownerMinutesUsed is int m ? FormatMinutesAndHours(m) : h.LandownerMinutesUsedDisplay,
        Settlements: Array.AsReadOnly(h.Settlements.Select(s => ProjectEconomySettlement(s, options)).ToArray()),
        Ledger: Array.AsReadOnly(h.Ledger.Select(e => ProjectEconomyLedgerEntry(e, options)).ToArray()),
        Disclosure: h.Disclosure);

    private static string FormatSignedEuros(long cents) =>
        cents > 0L ? "+" + EconomyDefaults.FormatEuros(cents) : EconomyDefaults.FormatEuros(cents);

    private static string FormatHoursValue(int minutes) =>
        minutes % 60 == 0
            ? FormattableString.Invariant($"{minutes / 60}")
            : FormattableString.Invariant($"{minutes / 60m:0.##}");

    private static string FormatHoursOfAllowance(int remainingMinutes, int annualMinutes) =>
        FormattableString.Invariant($"{FormatHoursValue(remainingMinutes)} / {FormatHoursValue(annualMinutes)} hrs");

    private static string FormatMinutesAndHours(int minutes) =>
        FormattableString.Invariant($"{FormatHoursValue(minutes)} hrs ({minutes} min)");

    private static string FormatVolumeM3(long volumeCm3) =>
        FormattableString.Invariant($"{volumeCm3 / 1_000_000m:0.000} m³");

    private static string FormatTonnes(long greenGrams) =>
        FormattableString.Invariant($"{greenGrams / 1_000_000m:0.00} t");

    private static string FormatStageLabel(HarvestThinningStage stage, long rateCentsPerTonne) => stage switch
    {
        HarvestThinningStage.FirstThinning => $"First thinning ({EconomyDefaults.FormatEuros(rateCentsPerTonne)}/t)",
        HarvestThinningStage.SecondThinning => $"Second thinning ({EconomyDefaults.FormatEuros(rateCentsPerTonne)}/t)",
        HarvestThinningStage.LaterThinning => $"Later thinning ({EconomyDefaults.FormatEuros(rateCentsPerTonne)}/t)",
        _ => $"{stage} ({EconomyDefaults.FormatEuros(rateCentsPerTonne)}/t)",
    };

    private static string FormatSpeciesLabel(string speciesId) => speciesId switch
    {
        Phase2Species.SitkaId => "Sitka spruce",
        Phase2Species.OakId => "Sessile oak",
        Phase2Species.BeechId => "European beech",
        _ => speciesId,
    };

    private static string FormatAssortmentLabel(TimberAssortmentKind kind) => kind switch
    {
        TimberAssortmentKind.Sawlog => "Sawlog",
        TimberAssortmentKind.Pallet => "Pallet",
        TimberAssortmentKind.Stake => "Stake",
        TimberAssortmentKind.Pulp => "Pulp",
        TimberAssortmentKind.VeneerPlanking => "Veneer planking",
        TimberAssortmentKind.Beam => "Beam",
        TimberAssortmentKind.Fencing => "Fencing",
        TimberAssortmentKind.SecondPlanking => "Second planking",
        TimberAssortmentKind.Firewood => "Firewood",
        _ => kind.ToString(),
    };

    private static string FormatMaterialLabel(string? materialId, long materialCostCents)
    {
        string name = materialId switch
        {
            null => "None",
            EconomyDefaults.ShelterMaterialId => "Tree shelter",
            EconomyDefaults.BeechSaplingMaterialId => "Beech sapling",
            EconomyDefaults.OakSaplingMaterialId => "Oak sapling",
            _ => materialId,
        };
        return materialId is null || materialCostCents <= 0L
            ? name
            : $"{name} ({EconomyDefaults.FormatEuros(materialCostCents)})";
    }

    private static string FormatLedgerCategoryLabel(EconomyLedgerCategory category, EconomyOptions? options = null) => category switch
    {
        EconomyLedgerCategory.HarvestingAndForwarding => "Harvesting & forwarding",
        EconomyLedgerCategory.MinimumJobAdjustment => options is not null
            ? $"Minimum job floor ({EconomyDefaults.FormatEuros(options.MinimumHarvestJobCents)})"
            : "Minimum job floor",
        EconomyLedgerCategory.ContractorLabour => options is not null
            ? $"Contractor labour ({EconomyDefaults.FormatEuros(options.ContractorHourlyRateCents)}/hr)"
            : "Contractor labour",
        EconomyLedgerCategory.Materials => "Materials & saplings",
        EconomyLedgerCategory.TimberSale => "Timber sale (roadside)",
        _ => category.ToString(),
    };

    private static string PlainIssueExplanation(EconomyQuoteIssue issue, EconomyWorkPlanQuote quote, EconomyOptions? options = null) => issue.Code switch
    {
        "EconomyInsufficientCash" =>
            $"Blocked — Insufficient upfront cash: {EconomyDefaults.FormatEuros(quote.AvailableCashBeforeCents)} available, " +
            $"but {EconomyDefaults.FormatEuros(quote.UpfrontCashRequiredCents)} upfront external cost is required before approval or execution. " +
            $"Expected timber revenue ({EconomyDefaults.FormatEuros(quote.TimberSaleRevenueCents)}) cannot pre-pay work, and no borrowing or overdraft is allowed.",
        "EconomyInsufficientLandownerAllowance" =>
            $"Blocked — Insufficient landowner hours: {FormatMinutesAndHours(quote.AvailableLandownerMinutesBefore)} remaining this year, " +
            $"but {FormatMinutesAndHours(quote.TotalOwnerMinutesRequired)} assigned to Landowner. Switch tasks to Contractor or advance to next year to reset the " +
            (options is not null ? $"{FormatHoursValue(options.AnnualLandownerAllowanceMinutes)}-hour annual allowance." : "annual allowance."),
        "EconomyFellContractorOnly" =>
            "Blocked — Commercial production felling is contractor-only and cannot be assigned to Landowner.",
        _ => issue.Message,
    };

    // Stage4 ("stage4"), Ecology ("ecology"), and Step 2 bounding/preview fields are additive; all existing members keep their exact names and shapes.
    // EconomyEnabled ("economyEnabled") is explicit server truth from SelectedModels.IsEconomyEnabled; Economy ("economy") is omitted when Off.
    private sealed record MultiViewPayload(object View, object Scenes,
        IReadOnlyDictionary<string, string> Labels, object? Stage4, MultiAreaEcologyDto? Ecology,
        bool HistoryTruncated = false, int TotalRecordedYears = 0, MultiReviewPreviewDto? ReviewPreview = null,
        bool ProjectionCompacted = false, bool Stage4Compacted = false, bool PayloadBudgetExceeded = false,
        bool EconomyEnabled = false,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] MultiEconomySnapshotDto? Economy = null);

    private sealed record MultiProjectedDeathDto(int EcologicalYear, string Cause, string Mechanism, LifeState LifeState);

    private sealed record MultiProjectedTreeDto(
        string TreeId,
        float XM,
        float ZM,
        int AgeYears,
        float DbhCm,
        float HeightM,
        float CrownRadiusM,
        bool IsLiving,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] MultiProjectedDeathDto? Death);

    private sealed record MultiProjectedAreaViewDto(
        string AreaId, string Label, string StockingNote, string Site, int SimulationSeed,
        float AreaHa, string AreaM2Exact, IReadOnlyList<MultiAreaVertex> Polygon, PlayerMapExtent Extent,
        IReadOnlyList<MultiProjectedTreeDto> Trees, AnnualStand Stand, IReadOnlyList<string> CropTreeIds);

    private sealed record MultiProjectedSessionViewDto(
        string SessionId, int PlanStateRevision, MultiAreaSourceToken Source,
        MultiAreaExperimentInfo Experiment, IReadOnlyList<MultiProjectedAreaViewDto> Areas,
        IReadOnlyList<MultiAreaWorkOrder> Pending, MultiAreaApprovedPlan? Approved,
        IReadOnlyList<MultiAreaWorkEvent> Events, IReadOnlyList<MultiAreaYearRecord> AnnualHistory);

    private sealed record MultiProjectedMapItemDto(
        string TreeId,
        int PaintOrder,
        double U,
        double V,
        double RadiusM,
        PlayerMapStatus Status,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] int? DeathYear,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] bool IsCrop);

    private sealed record MultiProjectedDeadwoodDto(
        string DeadwoodId, string TreeId, float XM, float ZM, double U, double V,
        int DeathYear, string Cause, float DbhCm, float HeightM, float EstimatedVolumeM3);

    private sealed record MultiProjectedCanopySummaryDto(
        double PolygonAreaM2, double GeometricCoverFraction, double GeometricCoveredM2, double GeometricUncoveredM2,
        int OpeningCount, double LargestOpeningAreaM2, double LargestOpeningWidthM, double RelativeDensity,
        int AnnualNaturalDeaths, int CumulativeNaturalDeaths, int FallenDeadwoodCount,
        double MeanLightIndex, float MinLightIndex, float MaxLightIndex,
        IReadOnlyList<MultiProjectedDeadwoodDto> FallenDeadwood);

    private sealed record MultiProjectedGroundVertexDto(double XM, double ZM, double U, double V);

    private sealed record MultiProjectedGroundCellDto(
        int Column, int Row, float CentreXM, float CentreZM, double CentreU, double CentreV, double ClippedAreaM2,
        IReadOnlyList<MultiProjectedGroundVertexDto> Polygon, float LightIndex, double GeometricCoveredFraction,
        int CrossBorderTrees,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] IReadOnlyList<string>? ContributingTreeIds);

    private sealed record MultiProjectedUiTreeDto(
        string TreeId,
        LifeState LifeState,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? DeathCause,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] int? DeathYear,
        int PruningLifts,
        int MaxPruningLifts,
        float CrownBaseHeightM,
        int LastPruningYear,
        bool PruneEligible,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? PruneRefusalCode,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? PruneRefusalReason,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] float? NextPruneTargetM,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] int? NextPruneEligibleYear,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? PrunePlanState);

    private sealed record MultiProjectedStage4AreaDto(
        string AreaId,
        MultiProjectedCanopySummaryDto Canopy,
        IReadOnlyList<MultiProjectedUiTreeDto> Trees,
        object Ground);

    private sealed record MultiProjectedStage4Dto(
        string SessionId,
        int EcologicalYear,
        int PlanStateRevision,
        MultiAreaSourceToken Source,
        MultiAreaUiModels Models,
        IReadOnlyList<MultiProjectedStage4AreaDto> Areas,
        IReadOnlyList<MultiAreaPruneOrder> PendingPrunes,
        IReadOnlyList<MultiAreaPruneOrder> ApprovedPrunes,
        IReadOnlyList<MultiAreaPruneRecord> PruneLog);

    private sealed record MultiCompetitionPayload(object View,
        object Scenes, string AreaId, PlayerCompetitionReport? Competition,
        Notice? Refusal, object? Stage4, MultiAreaEcologyDto? Ecology,
        bool HistoryTruncated = false, int TotalRecordedYears = 0,
        bool ProjectionCompacted = false, bool Stage4Compacted = false, bool PayloadBudgetExceeded = false,
        bool EconomyEnabled = false,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] MultiEconomySnapshotDto? Economy = null);

    private sealed record MultiHitPayload(object View, object Scenes,
        string AreaId, IReadOnlyList<PlayerMapItem> Candidates, Notice? Refusal, object? Stage4,
        MultiAreaEcologyDto? Ecology, bool HistoryTruncated = false, int TotalRecordedYears = 0,
        bool ProjectionCompacted = false, bool Stage4Compacted = false, bool PayloadBudgetExceeded = false,
        bool EconomyEnabled = false,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] MultiEconomySnapshotDto? Economy = null);

    private sealed record MultiCellPayload(object View, object Scenes,
        MultiAreaCellDetail? Cell, Notice? Refusal, object? Stage4, MultiAreaEcologyDto? Ecology,
        bool HistoryTruncated = false, int TotalRecordedYears = 0,
        bool ProjectionCompacted = false, bool Stage4Compacted = false, bool PayloadBudgetExceeded = false,
        bool EconomyEnabled = false,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] MultiEconomySnapshotDto? Economy = null);

    private sealed record MultiEconomyExecutorEntryDto(string Kind, string AreaId, string TargetKey, string Executor);

    private sealed record MultiEconomyIssueDto(string Code, string Message, string PlainExplanation, long Required, long Available);

    private sealed record MultiEconomyAssortmentDto(
        string SpeciesId,
        string SpeciesLabel,
        string Assortment,
        string AssortmentLabel,
        int PieceCount,
        long VolumeCm3,
        string VolumeDisplay,
        long GreenGrams,
        string TonnesDisplay,
        long UnitPriceCentsPerTonne,
        string UnitPriceDisplay,
        long RevenueCents,
        string RevenueDisplay);

    private sealed record MultiEconomyHarvestQuoteDto(
        string Stage,
        string StageLabel,
        long StageRateCentsPerTonne,
        string StageRateDisplay,
        int FelledStemCount,
        string TotalStemVolumeDisplay,
        string SaleVolumeDisplay,
        string ResidualVolumeDisplay,
        string HarvestedTonnesDisplay,
        long VariableHarvestCostCents,
        string VariableHarvestCostDisplay,
        long MinimumJobFloorCents,
        string MinimumJobFloorDisplay,
        long MinimumJobAdjustmentCents,
        string MinimumJobAdjustmentDisplay,
        bool MinimumJobFloorApplied,
        string MinimumJobFloorHighlight,
        long TotalHarvestContractorCostCents,
        string TotalHarvestContractorCostDisplay,
        long TimberSaleRevenueCents,
        string TimberSaleRevenueDisplay,
        IReadOnlyList<MultiEconomyAssortmentDto> Assortments);

    private sealed record MultiEconomyNonHarvestLineDto(
        string Kind,
        string AreaId,
        string TargetKey,
        string Executor,
        int PersonMinutes,
        string TimeDisplay,
        int OwnerMinutes,
        int ContractorMinutes,
        long ContractorLabourCents,
        string ContractorLabourDisplay,
        string? MaterialId,
        string MaterialLabel,
        long MaterialCostCents,
        string MaterialCostDisplay,
        long TotalExternalCostCents,
        string TotalExternalCostDisplay);

    private sealed record MultiEconomyQuoteDto(
        int PlanStateRevision,
        int EcologicalYear,
        bool IsEligible,
        bool UpfrontCashBlocked,
        string UpfrontCashVerdict,
        long AvailableCashBeforeCents,
        string AvailableCashBeforeDisplay,
        int AvailableLandownerMinutesBefore,
        string AvailableLandownerHoursBeforeDisplay,
        MultiEconomyHarvestQuoteDto? HarvestJob,
        IReadOnlyList<MultiEconomyNonHarvestLineDto> NonHarvestLines,
        long ContractorHarvestCostCents,
        string ContractorHarvestCostDisplay,
        long ContractorNonHarvestLabourCents,
        string ContractorNonHarvestLabourDisplay,
        long MaterialCostCents,
        string MaterialCostDisplay,
        long TotalExternalCostCents,
        string TotalExternalCostDisplay,
        long UpfrontCashRequiredCents,
        string UpfrontCashRequiredDisplay,
        long TimberSaleRevenueCents,
        string TimberSaleRevenueDisplay,
        long NetCashDeltaCents,
        string NetCashDeltaDisplay,
        bool NetCashIsLoss,
        int TotalOwnerMinutesRequired,
        string TotalOwnerHoursRequiredDisplay,
        int TotalContractorMinutes,
        string TotalContractorHoursDisplay,
        IReadOnlyList<MultiEconomyIssueDto> Issues,
        IReadOnlyList<string> EvidenceLabels);

    private sealed record MultiEconomyLedgerEntryDto(
        string Category,
        string CategoryLabel,
        string ReferenceId,
        long CashDeltaCents,
        string CashDeltaDisplay,
        int EcologicalYear,
        int WorkRevision,
        string ExecutionKey,
        string PlanId);

    private sealed record MultiEconomySettlementDto(
        string ExecutionKey,
        string PlanId,
        int EcologicalYear,
        int WorkRevision,
        long CashBeforeCents,
        string CashBeforeDisplay,
        long TotalExternalCostCents,
        string TotalExternalCostDisplay,
        long TimberSaleRevenueCents,
        string TimberSaleRevenueDisplay,
        long NetCashDeltaCents,
        string NetCashDeltaDisplay,
        long CashAfterCents,
        string CashAfterDisplay,
        int OwnerMinutesBefore,
        int OwnerMinutesConsumed,
        int OwnerMinutesAfter,
        string OwnerHoursBeforeDisplay,
        string OwnerHoursConsumedDisplay,
        string OwnerHoursAfterDisplay,
        MultiEconomyHarvestQuoteDto? HarvestJob,
        IReadOnlyList<MultiEconomyLedgerEntryDto> LedgerEntries);

    private sealed record MultiEconomySnapshotDto(
        bool Enabled,
        long CashBalanceCents,
        string CashBalanceDisplay,
        long OpeningCashCents,
        string OpeningCashDisplay,
        int UsedLandownerMinutesThisYear,
        int RemainingLandownerMinutes,
        int AnnualLandownerAllowanceMinutes,
        string AnnualLandownerAllowanceDisplay,
        string LandownerAllowanceTooltip,
        string LandownerHoursDisplay,
        string UsedLandownerHoursDisplay,
        string RemainingLandownerHoursDisplay,
        int ExecutedHarvestPlansCount,
        string NextHarvestStage,
        string NextHarvestStageLabel,
        long MinimumHarvestFloorCents,
        string MinimumHarvestFloorDisplay,
        string ContractorHourlyRateDisplay,
        IReadOnlyList<MultiEconomyExecutorEntryDto> PendingOrderExecutors,
        MultiEconomyQuoteDto? ActiveQuote,
        bool ApprovedQuoteFrozen,
        MultiEconomySettlementDto? LatestSettlement,
        string FellContractorTooltip,
        string SaveBlockedCode,
        string SaveBlockedNote,
        string Disclaimer,
        IReadOnlyList<string> EvidenceLabels);

    private sealed record MultiEconomyHistoryYearDto(
        int Year,
        bool EconomyActive,
        string Provenance,
        string ProvenanceLabel,
        long? OpeningCashCents,
        long? ClosingCashCents,
        long? TotalExternalCostCents,
        long? TotalTimberRevenueCents,
        long? NetCashDeltaCents,
        int? LandownerMinutesUsed,
        string OpeningCashDisplay,
        string ClosingCashDisplay,
        string TotalExternalCostDisplay,
        string TotalTimberRevenueDisplay,
        string NetCashDeltaDisplay,
        string LandownerMinutesUsedDisplay,
        IReadOnlyList<MultiEconomySettlementDto> Settlements,
        IReadOnlyList<MultiEconomyLedgerEntryDto> Ledger,
        string Disclosure);
}
