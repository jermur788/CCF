using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using SitkaP1.Core;

namespace SitkaP1.Ui;

internal sealed partial class PlayerApi
{
    // GET api/multi/stand (overview) or GET api/multi/stand?lanes=MA-A,MA-B,MA-C (lane-filtered detail) under C-FENCE.
    // Every read acquires the Core session lock ONCE, carries the full MultiFenceMembers fence, refuses stale fences
    // with the existing typed refusal codes, and NEVER returns an over-budget payload (HTTP 503 instead).
    private Task ReadMultiStandAsync(HttpContext context)
    {
        string? lanesParam = null;
        string? qSessionId = null;
        int? qPlanRev = null;
        int? qYear = null;
        int? qAnnualRev = null;
        int? qWorkRev = null;

        foreach (var (key, values) in context.Request.Query)
        {
            if (values.Count != 1)
                return WriteErrorAsync(context, StatusCodes.Status400BadRequest, "BadRequest",
                    FormattableString.Invariant($"duplicate query parameter '{key}'; nothing changed"));
            string raw = values[0] ?? "";
            switch (key)
            {
                case "lanes":
                    lanesParam = raw;
                    break;
                case "sessionId":
                    qSessionId = raw;
                    break;
                case "planStateRevision":
                    if (!TryParseExactInt(raw, allowNegative: false, out int pRev))
                        return WriteErrorAsync(context, StatusCodes.Status400BadRequest, "BadRequest", "planStateRevision must be a non-negative integer");
                    qPlanRev = pRev;
                    break;
                case "ecologicalYear":
                    if (!TryParseExactInt(raw, allowNegative: false, out int yr))
                        return WriteErrorAsync(context, StatusCodes.Status400BadRequest, "BadRequest", "ecologicalYear must be a non-negative integer");
                    qYear = yr;
                    break;
                case "annualRevision":
                    if (!TryParseExactInt(raw, allowNegative: false, out int aRev))
                        return WriteErrorAsync(context, StatusCodes.Status400BadRequest, "BadRequest", "annualRevision must be a non-negative integer");
                    qAnnualRev = aRev;
                    break;
                case "workRevision":
                    if (!TryParseExactInt(raw, allowNegative: false, out int wRev))
                        return WriteErrorAsync(context, StatusCodes.Status400BadRequest, "BadRequest", "workRevision must be a non-negative integer");
                    qWorkRev = wRev;
                    break;
                default:
                    return WriteErrorAsync(context, StatusCodes.Status400BadRequest, "BadRequest",
                        FormattableString.Invariant($"unknown query parameter '{key}'; nothing changed"));
            }
        }

        if (lanesParam is null)
            return WriteStandOverviewAsync(context);

        string[] rawLanes = lanesParam.Split(',', StringSplitOptions.None);
        if (!TryNormalizeRequestedLanes(rawLanes, out var requestedLanes, out string? laneError))
            return WriteErrorAsync(context, StatusCodes.Status400BadRequest, "BadRequest", laneError!);

        return WriteStandLanesAsync(context, requestedLanes!, fullFence: null, qSessionId, qPlanRev, qYear, qAnnualRev, qWorkRev);
    }

    // POST api/multi/stand — full MultiFenceMembers-fenced lane-filtered stand read under C-FENCE.
    private async Task ReadMultiStandPostAsync(HttpContext context)
    {
        var request = await ReadMultiRequestAsync<MultiStandPostRequest>(context, MultiStandPostMembers);
        if (request is null) return;
        string? laneError = null;
        if (request.Lanes is null || !TryNormalizeRequestedLanes(request.Lanes, out var requestedLanes, out laneError))
        {
            await WriteErrorAsync(context, StatusCodes.Status400BadRequest, "BadRequest", laneError ?? "lanes must be a non-empty array of lane IDs");
            return;
        }
        await WriteStandLanesAsync(context, requestedLanes!, request.Fence, null, null, null, null, null);
    }

    private Task WriteStandOverviewAsync(HttpContext context)
    {
        MultiAreaCompositeUiSnapshot snap;
        int totalRecordedYears;
        int genesisStems;
        MultiReviewPreviewDto? reviewPreview;
        bool economyEnabled;
        MultiEconomySnapshotDto? economy;
        bool hasUnsavedProgress;
        string experimentKey;
        string experimentBadge;
        MultiWoodlandRecipeDto? recipeDto;

        lock (multiHost.SyncLock)
        {
            var active = multiHost.Current;
            snap = active.Adapter.ReadCompositeUiSnapshot();
            totalRecordedYears = active.Session.ReadHistoryIndex(pageSize: 1).TotalRecordedYears;
            genesisStems = active.Session.GenesisStemCount;
            reviewPreview = TryPreviewForView(active, snap.View);
            economyEnabled = active.Session.SelectedModels.IsEconomyEnabled;
            economy = TryBuildEconomySnapshot(active.Session, snap.View);
            hasUnsavedProgress = active.HasUnsavedProgress;
            experimentKey = active.ExperimentKey;
            experimentBadge = active.Badge;
            recipeDto = active.Recipe is null ? null : ProjectRecipeDto(active.Recipe);
        }

        var view = snap.View;
        var fence = FenceOf(view);
        var speciesByTree = snap.Phase2?.TreeSpecies;
        var areaSummaries = new MultiStandOverviewAreaDto[view.Areas.Count];
        for (int i = 0; i < view.Areas.Count; i++)
        {
            var area = view.Areas[i];
            var s4Canopy = snap.Readout.Areas[i].Canopy;
            var canopyLite = new MultiStandOverviewCanopyDto(
                s4Canopy.PolygonAreaM2, s4Canopy.GeometricCoverFraction, s4Canopy.OpeningCount,
                s4Canopy.RelativeDensity, s4Canopy.AnnualNaturalDeaths, s4Canopy.CumulativeNaturalDeaths,
                s4Canopy.FallenDeadwoodCount, s4Canopy.MeanLightIndex);
            var stems = new MultiMinimapStemDto[area.Trees.Count];
            for (int t = 0; t < area.Trees.Count; t++)
            {
                var tr = area.Trees[t];
                int spCode = 0;
                if (speciesByTree is not null && speciesByTree.TryGetValue(tr.TreeId, out var spId))
                {
                    spCode = spId == Phase2Species.OakId ? 1 : spId == Phase2Species.BeechId ? 2 : ScotsPineLive.IsPine(spId) ? 3 : 0;
                }
                stems[t] = new MultiMinimapStemDto(R3f(tr.XM), R3f(tr.ZM), R3f(tr.CrownRadiusM), spCode, tr.IsLiving ? 1 : 0);
            }
            areaSummaries[i] = new MultiStandOverviewAreaDto(
                area.AreaId, area.Label, area.StockingNote, area.AreaHa, area.AreaM2Exact,
                area.Polygon, area.Extent, area.Stand, area.CropTreeIds, canopyLite, Array.AsReadOnly(stems));
        }

        var payload = new MultiStandOverviewPayload(
            fence,
            view.Experiment,
            experimentKey,
            experimentBadge,
            recipeDto,
            hasUnsavedProgress,
            snap.Readout.Models,
            MultiLabels,
            Array.AsReadOnly(areaSummaries),
            view.Pending,
            view.Approved,
            TailOf(view.Events, 10),
            TailOf(view.AnnualHistory, 10),
            totalRecordedYears,
            reviewPreview,
            economyEnabled,
            economy);

        var budget = ActiveLaneBudget(view.Areas.Count, genesisStems);
        return WriteBoundedJsonAsync(context, StatusCodes.Status200OK, payload, budget);
    }

    private Task WriteStandLanesAsync(
        HttpContext context,
        IReadOnlyList<string> requestedLanes,
        MultiAreaFence? fullFence,
        string? qSessionId,
        int? qPlanRev,
        int? qYear,
        int? qAnnualRev,
        int? qWorkRev)
    {
        MultiAreaCompositeUiSnapshot snap;
        int totalRecordedYears;
        int genesisStems;
        bool economyEnabled;
        MultiEconomySnapshotDto? economy;
        bool hasUnsavedProgress;
        MultiWoodlandRecipeDto? recipeDto;
        Notice? refusal = null;

        lock (multiHost.SyncLock)
        {
            var active = multiHost.Current;
            snap = active.Adapter.ReadCompositeUiSnapshot();
            totalRecordedYears = active.Session.ReadHistoryIndex(pageSize: 1).TotalRecordedYears;
            genesisStems = active.Session.GenesisStemCount;
            economyEnabled = active.Session.SelectedModels.IsEconomyEnabled;
            economy = TryBuildEconomySnapshot(active.Session, snap.View);
            hasUnsavedProgress = active.HasUnsavedProgress;
            recipeDto = active.Recipe is null ? null : ProjectRecipeDto(active.Recipe);

            // Fence check under the exact same snapshot lock (C-FENCE).
            if (fullFence is not null)
            {
                refusal = ValidateFenceNotice(fullFence, snap.View);
            }
            else if (qSessionId is not null || qPlanRev is not null || qYear is not null || qAnnualRev is not null || qWorkRev is not null)
            {
                if (qSessionId is not null && !string.Equals(qSessionId, snap.View.SessionId, StringComparison.Ordinal))
                    refusal = new Notice("PlayerSessionMismatch", "this request does not belong to the current multi-area session");
                else if ((qYear is not null && qYear.Value != snap.View.Source.EcologicalYear) ||
                         (qAnnualRev is not null && qAnnualRev.Value != snap.View.Source.AnnualRevision) ||
                         (qWorkRev is not null && qWorkRev.Value != snap.View.Source.WorkRevision))
                    refusal = new Notice("PlayerSourceStale", "the source, year or a revision changed since this view was read; re-read the current view");
                else if (qPlanRev is not null && qPlanRev.Value != snap.View.PlanStateRevision)
                    refusal = new Notice("PlayerPlanStateStale", "the Work Plan changed since this view was read; re-read the current view");
            }
        }

        var budget = ActiveLaneBudget(snap.View.Areas.Count, genesisStems);
        if (refusal is not null)
        {
            return WriteBoundedJsonAsync(context, StatusCodes.Status409Conflict,
                new MultiStandRefusalPayload(FenceOf(snap.View), refusal), budget);
        }

        var knownIds = snap.View.Areas.Select(a => a.AreaId).ToHashSet(StringComparer.Ordinal);
        foreach (string lane in requestedLanes)
        {
            if (!knownIds.Contains(lane))
            {
                return WriteBoundedJsonAsync(context, StatusCodes.Status409Conflict,
                    new MultiStandRefusalPayload(FenceOf(snap.View),
                        new Notice("MultiAreaUnknownArea", "no area of this experiment has exact ID '" + lane + "'")), budget);
            }
        }

        var requestedSet = requestedLanes.ToHashSet(StringComparer.Ordinal);
        var filteredView = FilterViewToLanes(snap.View, requestedSet);
        var filteredStage4 = FilterStage4ToLanes(snap.Readout, requestedSet);
        var filteredEcology = FilterEcologyToLanes(snap.Ecology, snap.View, filteredView, requestedSet);

        var (bytes, exceeded) = SerializeBoundedLaneStand(
            FenceOf(snap.View), requestedLanes, filteredView, filteredStage4, filteredEcology,
            totalRecordedYears, economyEnabled, economy, hasUnsavedProgress, recipeDto, budget);

        return WriteBoundedBytesAsync(context, StatusCodes.Status200OK, bytes, budget, exceeded);
    }

    private static bool TryNormalizeRequestedLanes(IReadOnlyList<string> rawLanes, out IReadOnlyList<string>? normalized, out string? error)
    {
        normalized = null;
        error = null;
        if (rawLanes.Count == 0)
        {
            error = "lanes must name at least one lane ID";
            return false;
        }
        var seen = new HashSet<string>(StringComparer.Ordinal);
        var list = new List<string>(rawLanes.Count);
        foreach (string raw in rawLanes)
        {
            string id = (raw ?? "").Trim();
            if (id.Length == 0 || id.Length > MaxIdLength || id.Any(ch => char.IsWhiteSpace(ch) || char.IsControl(ch)))
            {
                error = "each lane ID must be 1-64 non-whitespace characters";
                return false;
            }
            if (!seen.Add(id))
            {
                error = "duplicate lane ID in request: " + id;
                return false;
            }
            list.Add(id);
        }
        list.Sort(StringComparer.Ordinal);
        normalized = list.AsReadOnly();
        return true;
    }

    private static MultiAreaSessionView FilterViewToLanes(MultiAreaSessionView fullView, HashSet<string> lanes) =>
        fullView with
        {
            Areas = Array.AsReadOnly(fullView.Areas.Where(a => lanes.Contains(a.AreaId)).ToArray()),
        };

    private static MultiAreaUiReadout FilterStage4ToLanes(MultiAreaUiReadout fullStage4, HashSet<string> lanes) =>
        fullStage4 with
        {
            Areas = Array.AsReadOnly(fullStage4.Areas.Where(a => lanes.Contains(a.AreaId)).ToArray()),
        };

    private static MultiAreaEcologyDto? FilterEcologyToLanes(
        MultiAreaEcologyDto? fullEcology,
        MultiAreaSessionView fullView,
        MultiAreaSessionView filteredView,
        HashSet<string> lanes)
    {
        if (fullEcology is null) return null;
        var treeIds = new HashSet<string>(StringComparer.Ordinal);
        foreach (var area in filteredView.Areas)
        {
            foreach (var tr in area.Trees) treeIds.Add(tr.TreeId);
        }

        // Map old areaIndex -> new filtered areaIndex for CellsCompact rows where row[0] == areaIndex.
        var oldToNewAreaIdx = new Dictionary<int, int>();
        int nextIdx = 0;
        for (int i = 0; i < fullView.Areas.Count; i++)
        {
            if (lanes.Contains(fullView.Areas[i].AreaId))
                oldToNewAreaIdx[i] = nextIdx++;
        }

        var filteredSpecies = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var (tid, sp) in fullEcology.SpeciesByTreeId)
        {
            if (treeIds.Contains(tid)) filteredSpecies[tid] = sp;
        }

        var filteredPrune = new Dictionary<string, MultiAreaSpeciesPruneStatus>(StringComparer.Ordinal);
        foreach (var (tid, status) in fullEcology.PruneStatusByTreeId)
        {
            if (treeIds.Contains(tid)) filteredPrune[tid] = status;
        }

        var filteredCells = new List<double[]>();
        foreach (var row in fullEcology.CellsCompact)
        {
            if (row.Length == 0) continue;
            int oldIdx = (int)row[0];
            if (!oldToNewAreaIdx.TryGetValue(oldIdx, out int newIdx)) continue;
            var copy = (double[])row.Clone();
            copy[0] = newIdx;
            filteredCells.Add(copy);
        }

        return fullEcology with
        {
            SpeciesByTreeId = filteredSpecies,
            PruneStatusByTreeId = filteredPrune,
            Areas = Array.AsReadOnly(fullEcology.Areas.Where(a => lanes.Contains(a.AreaId)).ToArray()),
            CellsCompact = filteredCells.AsReadOnly(),
            Planted = Array.AsReadOnly(fullEcology.Planted.Where(j => lanes.Contains(j.AreaId)).ToArray()),
            Shelters = Array.AsReadOnly(fullEcology.Shelters.Where(s => lanes.Contains(s.AreaId)).ToArray()),
            Patches = Array.AsReadOnly(fullEcology.Patches.Where(p => lanes.Contains(p.AreaId)).ToArray()),
        };
    }

    // Lossless compact serialization for lane-filtered /api/multi/stand?lanes=... responses.
    // Compacts repeated ground-cell JSON keys into compact numeric tuples (groundCompact) and deduplicates
    // repeated prune refusal strings via pruneReasonTable so 3 lanes of 0.64 ha fit well under 512 KiB.
    private static (byte[] Bytes, bool Exceeded) SerializeBoundedLaneStand(
        MultiAreaFence fence,
        IReadOnlyList<string> requestedLanes,
        MultiAreaSessionView filteredView,
        MultiAreaUiReadout filteredStage4,
        MultiAreaEcologyDto? filteredEcology,
        int totalRecordedYears,
        bool economyEnabled,
        MultiEconomySnapshotDto? economy,
        bool hasUnsavedProgress,
        MultiWoodlandRecipeDto? recipeDto,
        MultiViewBudgetLimit budget)
    {
        var fullScenes = ScenesOf(filteredView);
        var projEco = ProjectEcologyForView(filteredEcology, filteredView);
        var (compactEco, reasonTable) = CompactEcologyReasons(projEco);

        int minEvents = filteredView.Events.Count > 0 ? 1 : 0;
        int minAnnual = filteredView.AnnualHistory.Count > 0 ? 1 : 0;
        int maxHist = Math.Max(filteredView.Events.Count, filteredView.AnnualHistory.Count);

        byte[] Build(object viewObj, object scenesObj, object s4Obj, object? ecoObj, IReadOnlyList<string>? reasons,
            bool histTrunc, bool projCompact, bool s4Compact, bool pruneTrunc) =>
            JsonSerializer.SerializeToUtf8Bytes(new MultiStandLanesPayload(
                fence, requestedLanes, viewObj, scenesObj, s4Obj, ecoObj, reasons,
                histTrunc, totalRecordedYears, projCompact, s4Compact, economyEnabled, economy,
                hasUnsavedProgress, recipeDto, pruneTrunc), ResponseJson);

        MultiProjectedSessionViewDto MakeView(int keepEv, int keepAn) =>
            (keepEv >= filteredView.Events.Count && keepAn >= filteredView.AnnualHistory.Count)
                ? ProjectViewForBudget(filteredView)
                : ProjectViewForBudget(filteredView with
                {
                    Events = TailOf(filteredView.Events, keepEv),
                    AnnualHistory = TailOf(filteredView.AnnualHistory, keepAn),
                });

        var projScenes = ProjectScenesForBudget(fullScenes);
        int[] pruneCaps = filteredStage4.PruneLog.Count > 0 ? new[] { filteredStage4.PruneLog.Count, 10, 1, 0 } : new[] { 0 };
        foreach (int pCap in pruneCaps)
        {
            var s4CompactObj = ProjectStage4CompactRows(filteredStage4, hasEcology: compactEco is not null, pruneLogCap: pCap);
            bool pTrim = pCap < filteredStage4.PruneLog.Count;

            // HistoryTruncated is truthful: true ONLY when view.events or view.annualHistory entries are actually omitted.
            var vFull = MakeView(filteredView.Events.Count, filteredView.AnnualHistory.Count);
            byte[] bFull = Build(vFull, projScenes, s4CompactObj, compactEco, reasonTable,
                histTrunc: false, projCompact: true, s4Compact: true, pruneTrunc: pTrim);
            if (bFull.Length <= budget.MaxViewBytes)
                return (bFull, false);

            var vMin = MakeView(minEvents, minAnnual);
            bool minTrunc = minEvents < filteredView.Events.Count || minAnnual < filteredView.AnnualHistory.Count;
            byte[] bMin = Build(vMin, projScenes, s4CompactObj, compactEco, reasonTable,
                histTrunc: minTrunc, projCompact: true, s4Compact: true, pruneTrunc: pTrim);
            if (bMin.Length > budget.MaxViewBytes) continue;

            int lo = 1;
            int hi = Math.Max(2, maxHist);
            byte[] bestBytes = bMin;
            while (lo + 1 < hi)
            {
                int mid = lo + ((hi - lo) / 2);
                int kEv = Math.Min(filteredView.Events.Count, Math.Max(minEvents, mid));
                int kAn = Math.Min(filteredView.AnnualHistory.Count, Math.Max(minAnnual, mid));
                var vCand = MakeView(kEv, kAn);
                bool candTrunc = kEv < filteredView.Events.Count || kAn < filteredView.AnnualHistory.Count;
                byte[] bCand = Build(vCand, projScenes, s4CompactObj, compactEco, reasonTable,
                    histTrunc: candTrunc, projCompact: true, s4Compact: true, pruneTrunc: pTrim);
                if (bCand.Length <= budget.MaxViewBytes)
                {
                    lo = mid;
                    bestBytes = bCand;
                }
                else
                {
                    hi = mid;
                }
            }
            return (bestBytes, false);
        }

        var overS4 = ProjectStage4CompactRows(filteredStage4, hasEcology: compactEco is not null, pruneLogCap: 0);
        var overV = MakeView(minEvents, minAnnual);
        bool overHistTrunc = minEvents < filteredView.Events.Count || minAnnual < filteredView.AnnualHistory.Count;
        bool overPruneTrunc = filteredStage4.PruneLog.Count > 0;
        byte[] overBytes = Build(overV, projScenes, overS4, compactEco, reasonTable,
            histTrunc: overHistTrunc, projCompact: true, s4Compact: true, pruneTrunc: overPruneTrunc);
        return (overBytes, overBytes.Length > budget.HardMaxViewBytes);
    }

    // Deduplicates repeated RefusalReason strings into a compact indexed table so 807 trees don't repeat
    // identical refusal sentences across the wire. Transparently expanded in multi/api.js.
    private static (object? CompactEcology, IReadOnlyList<string>? ReasonTable) CompactEcologyReasons(MultiAreaEcologyDto? eco)
    {
        if (eco is null) return (null, null);
        var table = new List<string>();
        var indexByReason = new Dictionary<string, int>(StringComparer.Ordinal);
        var compactPrune = new Dictionary<string, MultiCompactPruneStatusDto>(eco.PruneStatusByTreeId.Count, StringComparer.Ordinal);
        foreach (var (treeId, st) in eco.PruneStatusByTreeId)
        {
            int? reasonIdx = null;
            if (!string.IsNullOrEmpty(st.RefusalReason))
            {
                if (!indexByReason.TryGetValue(st.RefusalReason, out int idx))
                {
                    idx = table.Count;
                    table.Add(st.RefusalReason);
                    indexByReason[st.RefusalReason] = idx;
                }
                reasonIdx = idx;
            }
            compactPrune[treeId] = new MultiCompactPruneStatusDto(
                st.SpeciesId, st.Eligible, st.Lifts, st.MaxLifts, st.MinIntervalYears,
                st.NextTargetM, st.NextEligibleYear, st.RefusalCode, reasonIdx, st.PlanState);
        }

        var dto = new MultiCompactEcologyDto(
            eco.SessionId, eco.PlanStateRevision, eco.Source, eco.Models,
            eco.ExperimentId, eco.ExperimentBadge, eco.GridCellM,
            eco.SpeciesByTreeId, compactPrune, eco.Areas, eco.CellSchema, eco.CellsCompact,
            eco.Planted, eco.Shelters, eco.Patches,
            eco.PendingClear, eco.ApprovedClear, eco.PendingPlanting, eco.ApprovedPlanting, eco.ScotsPine);
        return (dto, table.AsReadOnly());
    }

    // Compact row encoding for lane-filtered stage4.areas[].groundCompact:
    // each cell is [col, row, cx, cz, cu, cv, area, light, cover, crossBorder, polyFlat...] (lossless at 3dp).
    private static MultiCompactStage4Dto ProjectStage4CompactRows(MultiAreaUiReadout stage4, bool hasEcology, int pruneLogCap)
    {
        var compactAreas = new MultiCompactStage4AreaDto[stage4.Areas.Count];
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

            var groundCompact = new double[area.Ground.Count][];
            for (int g = 0; g < area.Ground.Count; g++)
            {
                var cell = area.Ground[g];
                var row = new double[10 + cell.Polygon.Count * 2];
                row[0] = cell.Column;
                row[1] = cell.Row;
                row[2] = R3(cell.CentreXM);
                row[3] = R3(cell.CentreZM);
                row[4] = R3(cell.CentreU);
                row[5] = R3(cell.CentreV);
                row[6] = R3(cell.ClippedAreaM2);
                row[7] = R3(cell.LightIndex);
                row[8] = R3(cell.GeometricCoveredFraction);
                row[9] = cell.CrossBorderTrees;
                int offset = 10;
                for (int p = 0; p < cell.Polygon.Count; p++)
                {
                    var v = cell.Polygon[p];
                    row[offset++] = R3(v.U);
                    row[offset++] = R3(v.V);
                }
                groundCompact[g] = row;
            }
            compactAreas[a] = new MultiCompactStage4AreaDto(area.AreaId, canopyProj, trees.AsReadOnly(), Array.AsReadOnly(groundCompact));
        }

        return new MultiCompactStage4Dto(
            stage4.SessionId, stage4.EcologicalYear, stage4.PlanStateRevision, stage4.Source, stage4.Models,
            Array.AsReadOnly(compactAreas), stage4.PendingPrunes, stage4.ApprovedPrunes, TailOf(stage4.PruneLog, pruneLogCap));
    }

    private sealed record MultiStandPostRequest(MultiAreaFence? Fence, IReadOnlyList<string>? Lanes);

    private sealed record MultiStandRefusalPayload(MultiAreaFence Fence, Notice Refusal);

    private sealed record MultiMinimapStemDto(float X, float Z, float R, int S, int L);

    private sealed record MultiStandOverviewCanopyDto(
        double PolygonAreaM2,
        double GeometricCoverFraction,
        int OpeningCount,
        double RelativeDensity,
        int AnnualNaturalDeaths,
        int CumulativeNaturalDeaths,
        int FallenDeadwoodCount,
        double MeanLightIndex);

    private sealed record MultiStandOverviewAreaDto(
        string AreaId,
        string Label,
        string StockingNote,
        float AreaHa,
        string AreaM2Exact,
        IReadOnlyList<MultiAreaVertex> Polygon,
        PlayerMapExtent Extent,
        AnnualStand Stand,
        IReadOnlyList<string> CropTreeIds,
        MultiStandOverviewCanopyDto Canopy,
        IReadOnlyList<MultiMinimapStemDto> MinimapStems);

    private sealed record MultiStandOverviewPayload(
        MultiAreaFence Fence,
        MultiAreaExperimentInfo Experiment,
        string ExperimentKey,
        string ExperimentBadge,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] MultiWoodlandRecipeDto? Recipe,
        bool HasUnsavedProgress,
        MultiAreaUiModels Models,
        IReadOnlyDictionary<string, string> Labels,
        IReadOnlyList<MultiStandOverviewAreaDto> Areas,
        IReadOnlyList<MultiAreaWorkOrder> Pending,
        MultiAreaApprovedPlan? Approved,
        IReadOnlyList<MultiAreaWorkEvent> Events,
        IReadOnlyList<MultiAreaYearRecord> AnnualHistory,
        int TotalRecordedYears,
        MultiReviewPreviewDto? ReviewPreview,
        bool EconomyEnabled,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] MultiEconomySnapshotDto? Economy);

    private sealed record MultiCompactPruneStatusDto(
        string SpeciesId,
        bool Eligible,
        int Lifts,
        int MaxLifts,
        int MinIntervalYears,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] float? NextTargetM,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] int? NextEligibleYear,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? RefusalCode,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] int? RefusalReasonIndex,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? PlanState);

    private sealed record MultiCompactEcologyDto(
        string SessionId,
        int PlanStateRevision,
        MultiAreaSourceToken Source,
        MultiAreaEcologyModels Models,
        string ExperimentId,
        string ExperimentBadge,
        double GridCellM,
        IReadOnlyDictionary<string, string> SpeciesByTreeId,
        IReadOnlyDictionary<string, MultiCompactPruneStatusDto> PruneStatusByTreeId,
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
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] MultiAreaScotsPineEcology? ScotsPine = null);

    private sealed record MultiCompactStage4AreaDto(
        string AreaId,
        MultiProjectedCanopySummaryDto Canopy,
        IReadOnlyList<MultiProjectedUiTreeDto> Trees,
        IReadOnlyList<double[]> GroundCompact);

    private sealed record MultiCompactStage4Dto(
        string SessionId,
        int EcologicalYear,
        int PlanStateRevision,
        MultiAreaSourceToken Source,
        MultiAreaUiModels Models,
        IReadOnlyList<MultiCompactStage4AreaDto> Areas,
        IReadOnlyList<MultiAreaPruneOrder> PendingPrunes,
        IReadOnlyList<MultiAreaPruneOrder> ApprovedPrunes,
        IReadOnlyList<MultiAreaPruneRecord> PruneLog);

    private sealed record MultiStandLanesPayload(
        MultiAreaFence Fence,
        IReadOnlyList<string> RequestedLanes,
        object View,
        object Scenes,
        object Stage4,
        object? Ecology,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] IReadOnlyList<string>? PruneReasonTable,
        bool HistoryTruncated,
        int TotalRecordedYears,
        bool ProjectionCompacted,
        bool Stage4Compacted,
        bool EconomyEnabled,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] MultiEconomySnapshotDto? Economy,
        bool HasUnsavedProgress,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] MultiWoodlandRecipeDto? Recipe,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] bool PruneLogTruncated = false);
}
