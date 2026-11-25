using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using SitkaP1.Core;
using SitkaP1.Core.Phase1;

namespace SitkaP1.Ui;

/// <summary>
/// The two-area simulator (<c>/multi</c>; Pass 2 Map Integration, spec <c>multi-map-integration-pass2-v1</c>): its own
/// page and JSON routes under the same <c>/s/&lt;capability&gt;/</c> prefix of the same single listener. Every request
/// passes the unchanged guards of <see cref="HandleAsync"/> first (Host, capability, route and method, Origin), and
/// every POST the same content-type and body-size checks. Every view/command readout is obtained atomically under one
/// Core lock so <c>view</c>, <c>stage4</c> and <c>ecology</c> never form torn frames.
/// </summary>
internal sealed partial class PlayerApi
{
    private const int MaxIdLength = 64;
    private const int MaxPlanKeyLength = 256;
    private const int MaxReasonLength = 2000;
    private const int MaxViewPayloadBytes = 524_288;
    private const int MaxEcologyPayloadBytes = 153_600;
    private const int DefaultHistoryPageSize = 10;
    private const int MinHistoryPageSize = 1;
    private const int MaxHistoryPageSize = 50;

    private static readonly string[] MultiFenceMembers =
    {
        "fence", "fence.sessionId", "fence.planStateRevision", "fence.source", "fence.source.domain",
        "fence.source.experimentId", "fence.source.manifestContentHash", "fence.source.areaIds", "fence.source.ecologicalYear",
        "fence.source.annualRevision", "fence.source.workRevision", "fence.source.rngModelVersion",
        "fence.source.growthModelVersion", "fence.source.p1AlgorithmVersion", "fence.source.areaModelVersion",
        "fence.source.actionEligibleDbhThresholdMm",
    };
    private static readonly string[] MultiTreeMembers = MultiFenceMembers.Concat(new[] { "areaId", "treeId" }).ToArray();
    private static readonly string[] MultiAddMembers = MultiTreeMembers.Append("reason").ToArray();
    private static readonly string[] MultiExecuteMembers = MultiFenceMembers.Concat(new[] { "planId", "executionKey" }).ToArray();
    private static readonly string[] MultiAdvanceMembers = MultiFenceMembers.Append("consentToCancelWork").ToArray();
    private static readonly string[] MultiSkipMembers = MultiFenceMembers.Append("targetYear").ToArray();
    private static readonly string[] MultiHistoryYearMembers = MultiFenceMembers.Append("year").ToArray();
    private static readonly string[] MultiHitMembers = MultiFenceMembers.Concat(new[] { "areaId", "u", "v" }).ToArray();
    private static readonly string[] MultiCellMembers = MultiFenceMembers.Concat(new[] { "areaId", "column", "row" }).ToArray();
    private static readonly string[] MultiPointMembers = MultiFenceMembers.Concat(new[] { "areaId", "xM", "zM" }).ToArray();
    private static readonly string[] MultiPlantMembers = MultiFenceMembers.Concat(new[] { "areaId", "speciesId", "xM", "zM" }).ToArray();
    private static readonly string[] MultiPlantingRemoveMembers = MultiFenceMembers.Append("orderId").ToArray();
    private static readonly string[] MultiSaveMembers = MultiFenceMembers.Concat(new[] { "slot", "replaceExisting" }).ToArray();
    private static readonly string[] MultiLoadMembers = MultiFenceMembers.Append("slot").ToArray();
    private static readonly string[] MultiRecoverMembers = new[] { "slot" };

    private static readonly byte[] MultiPageHtml = MultiEmbeddedAsset("SitkaP1.Ui.multi.multi.html");
    private static readonly byte[] MultiPageCss = MultiEmbeddedAsset("SitkaP1.Ui.multi.multi.css");
    private static readonly byte[] MultiPageJs = MultiEmbeddedAsset("SitkaP1.Ui.multi.multi.js");

    // Fixed page text from Core only: the single-area status words and Crop labels, and the MA-1 / Pass 1 labels.
    private static readonly IReadOnlyDictionary<string, string> MultiLabels = new[] { typeof(PlayerLabels), typeof(CropLabels), typeof(MultiAreaLabels) }
        .SelectMany(type => type.GetFields(BindingFlags.Public | BindingFlags.Static))
        .Where(field => field.IsLiteral && field.FieldType == typeof(string))
        .ToDictionary(field => field.Name, field => (string)field.GetRawConstantValue()!, StringComparer.Ordinal);

    private readonly MultiAreaWorkPlanAdapter multiArea;
    private readonly MultiAreaContinuation multiSession;
    private readonly Phase1SaveSlots saveSlots;
    private int savesWritten; // successful slot writes in this process, only for the truthful shutdown line

    /// <summary>True when this process wrote at least one save slot (the shutdown line must not claim nothing was saved).</summary>
    internal bool SavesWritten => Volatile.Read(ref savesWritten) > 0;

    private void AddMultiAreaRoutes()
    {
        routes.Add("multi", new("GET", context => WriteAssetAsync(context, MultiPageHtml, "text/html; charset=utf-8")));
        routes.Add("multi.css", new("GET", context => WriteAssetAsync(context, MultiPageCss, "text/css; charset=utf-8")));
        routes.Add("multi.js", new("GET", context => WriteAssetAsync(context, MultiPageJs, "text/javascript; charset=utf-8")));
        routes.Add("api/multi/view", new("GET", ReadMultiViewAsync));
        routes.Add("api/multi/manifest", new("GET", WriteMultiManifestAsync));
        // Phase 1 Save/Load: server-managed named slots only (no path, upload or download). Save captures under the engine
        // lock and writes outside it; Load validates and publishes inside it; a refusal changes nothing.
        routes.Add("api/multi/saves", new("GET", ReadMultiSavesAsync));
        routes.Add("api/multi/save", new("POST", MultiSaveAsync));
        routes.Add("api/multi/load", new("POST", MultiLoadAsync));
        routes.Add("api/multi/saves/recover", new("POST", MultiRecoverAsync));
        routes.Add("api/multi/hit", new("POST", MultiHitTestAsync));
        routes.Add("api/multi/cell", new("POST", ReadMultiCellAsync));
        routes.Add("api/multi/competition", new("POST", ReadMultiCompetitionAsync));
        routes.Add("api/multi/crop/mark", new("POST", context => MultiCommandAsync<MultiTreeRequest>(context, MultiTreeMembers,
            request => multiArea.MarkCrop(request.Fence!, request.AreaId!, request.TreeId!))));
        routes.Add("api/multi/crop/unmark", new("POST", context => MultiCommandAsync<MultiTreeRequest>(context, MultiTreeMembers,
            request => multiArea.UnmarkCrop(request.Fence!, request.AreaId!, request.TreeId!))));
        routes.Add("api/multi/pending/add", new("POST", context => MultiCommandAsync<MultiAddRequest>(context, MultiAddMembers,
            request => multiArea.AddPending(request.Fence!, request.AreaId!, request.TreeId!, request.Reason!))));
        routes.Add("api/multi/pending/remove", new("POST", context => MultiCommandAsync<MultiTreeRequest>(context, MultiTreeMembers,
            request => multiArea.RemovePending(request.Fence!, request.AreaId!, request.TreeId!))));
        routes.Add("api/multi/plan/approve", new("POST", context => MultiCommandAsync<MultiFenceRequest>(context, MultiFenceMembers,
            request => multiArea.Approve(request.Fence!))));
        routes.Add("api/multi/plan/cancel", new("POST", context => MultiCommandAsync<MultiFenceRequest>(context, MultiFenceMembers,
            request => multiArea.CancelApproved(request.Fence!))));
        routes.Add("api/multi/plan/execute", new("POST", context => MultiCommandAsync<MultiExecuteRequest>(context, MultiExecuteMembers,
            request => multiArea.Execute(request.Fence!, request.PlanId!, request.ExecutionKey!))));
        routes.Add("api/multi/year/advance", new("POST", context => MultiCommandAsync<MultiAdvanceRequest>(context, MultiAdvanceMembers,
            request => multiArea.Advance(request.Fence!, request.ConsentToCancelWork))));
        // Phase 1 Step 2: dual time pacing (preview/skip), paged Decision -> Outcome history, and woodland spatial structure.
        routes.Add("api/multi/year/preview-review", new("POST", PreviewMultiReviewAsync));
        routes.Add("api/multi/year/skip", new("POST", SkipMultiYearsAsync));
        routes.Add("api/multi/history/index", new("GET", ReadMultiHistoryIndexAsync));
        routes.Add("api/multi/history/year", new("POST", ReadMultiHistoryYearAsync));
        routes.Add("api/multi/structure", new("GET", ReadMultiStructureAsync));
        routes.Add("api/multi/prune/add", new("POST", context => MultiCommandAsync<MultiTreeRequest>(context, MultiTreeMembers,
            request => multiArea.AddPrune(request.Fence!, request.AreaId!, request.TreeId!))));
        routes.Add("api/multi/prune/remove", new("POST", context => MultiCommandAsync<MultiTreeRequest>(context, MultiTreeMembers,
            request => multiArea.RemovePrune(request.Fence!, request.AreaId!, request.TreeId!))));
        // Pass 2 fenced biological Work Plan actions (zero edits to Core biology; all under single Pending -> Approve -> Execute gate).
        routes.Add("api/multi/clear/add", new("POST", context => MultiCommandAsync<MultiCellRequest>(context, MultiCellMembers,
            request => RefusableMulti(() => multiSession.AddClearUndergrowth(request.Fence!, request.AreaId!, request.Column!.Value, request.Row!.Value)))));
        routes.Add("api/multi/clear/remove", new("POST", context => MultiCommandAsync<MultiCellRequest>(context, MultiCellMembers,
            request => RefusableMulti(() => multiSession.RemoveClearUndergrowth(request.Fence!, request.AreaId!, request.Column!.Value, request.Row!.Value)))));
        routes.Add("api/multi/plant/add", new("POST", context => MultiCommandAsync<MultiPlantRequest>(context, MultiPlantMembers,
            request => RefusableMulti(() => multiSession.AddPlant(request.Fence!, request.AreaId!, request.SpeciesId!, (float)request.XM!.Value, (float)request.ZM!.Value)))));
        routes.Add("api/multi/shelter/add", new("POST", context => MultiCommandAsync<MultiPointRequest>(context, MultiPointMembers,
            request => RefusableMulti(() => multiSession.AddShelter(request.Fence!, request.AreaId!, (float)request.XM!.Value, (float)request.ZM!.Value)))));
        routes.Add("api/multi/spot-clear/add", new("POST", context => MultiCommandAsync<MultiPointRequest>(context, MultiPointMembers,
            request => RefusableMulti(() => multiSession.AddSpotClear(request.Fence!, request.AreaId!, (float)request.XM!.Value, (float)request.ZM!.Value)))));
        routes.Add("api/multi/planting/remove", new("POST", context => MultiCommandAsync<MultiPlantingRemoveRequest>(context, MultiPlantingRemoveMembers,
            request => RefusableMulti(() => multiSession.RemovePlanting(request.Fence!, request.OrderId!)))));
    }

    private MultiAreaCommandResult RefusableMulti(Func<MultiAreaCommandResult> command)
    {
        try { return command(); }
        catch (P1Refusal refusal)
        {
            return new MultiAreaCommandResult(multiArea.Read(), null, null, refusal.Code, refusal.Message);
        }
    }

    // Preserves the exact canonical manifest body and its trailing contentHash verification (M07), appending truthful
    // Pass 1 experiment badge and active model tuple metadata in pure ASCII JSON after contentHash.
    private Task WriteMultiManifestAsync(HttpContext context)
    {
        byte[] raw = multiArea.ManifestBytes();
        var snap = multiArea.ReadCompositeUiSnapshot();
        var m = snap.Ecology?.Models ?? new MultiAreaEcologyModels(
            snap.Readout.Models.AdultModelVersion, snap.Readout.Models.MortalityModelVersion, snap.Readout.Models.CrownModelVersion, 0, 0);
        string suffix = FormattableString.Invariant(
            $",\"experimentKey\":\"{JsonEncodedText.Encode(multiArea.ExperimentKey)}\",\"experimentBadge\":\"{JsonEncodedText.Encode(multiArea.ExperimentBadge)}\",\"models\":{{\"adult\":{m.Adult},\"mortality\":{m.Mortality},\"crown\":{m.Crown},\"regeneration\":{m.Regeneration},\"species\":{m.Species}}}}}");
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
        var snapshot = multiArea.ReadCompositeUiSnapshot();
        int totalRecordedYears = multiSession.ReadHistoryIndex(pageSize: 1).TotalRecordedYears;
        MultiReviewPreviewDto? reviewPreview = TryPreviewForView(snapshot.View);
        SerializeBoundedMultiView(snapshot, totalRecordedYears, reviewPreview, out byte[] bytes);
        return WriteRawJsonAsync(context, StatusCodes.Status200OK, bytes);
    }

    // ---- Phase 1 Step 2: Time Pacing, Decision -> Outcome History, and Woodland Structure -------------------------------

    // Read-only deterministic preview of the next suggested management review year. Zero mutation.
    private async Task PreviewMultiReviewAsync(HttpContext context)
    {
        var request = await ReadMultiRequestAsync<MultiFenceRequest>(context, MultiFenceMembers);
        if (request is null) return;
        try
        {
            var preview = multiSession.PreviewNextReviewYear(request.Fence!);
            var view = multiArea.Read();
            var fence = FenceOf(view);
            var boundedView = BoundSessionViewForAux(view);
            bool isOverdue = string.Equals(preview.Label, MultiAreaTimePacingConstants.OverdueReviewPeriodLabel, StringComparison.Ordinal);
            await WriteJsonAsync(context, StatusCodes.Status200OK, new MultiPreviewReviewPayload(
                preview.CurrentYear, preview.Eligible, preview.SuggestedTargetYear, preview.SuggestedGapYears,
                preview.AnchorFellYear, preview.IsObservationPeriod, isOverdue, preview.MaxTicksPerPress, preview.StopReason,
                preview.CalibrationNote, preview.Label, fence, boundedView, Refusal: null));
        }
        catch (P1Refusal refusal)
        {
            var view = multiArea.Read();
            await WriteJsonAsync(context, StatusCodes.Status409Conflict, new MultiPreviewReviewPayload(
                null, null, null, null, null, null, null, null, null, null, null,
                FenceOf(view), BoundSessionViewForAux(view), new Notice(refusal.Code, refusal.Message)));
        }
    }

    // Bounded multi-year advance (1..5 years per press) calling Core's AdvanceYears(fence, targetYear, ct).
    // NEVER passes consentToCancelWork = true and never cancels open work.
    private async Task SkipMultiYearsAsync(HttpContext context)
    {
        var request = await ReadMultiRequestAsync<MultiSkipRequest>(context, MultiSkipMembers);
        if (request is null) return;
        AdvanceYearsResult result;
        try
        {
            result = multiSession.AdvanceYears(request.Fence!, request.TargetYear!.Value, context.RequestAborted);
        }
        catch (P1Refusal refusal)
        {
            var staleSnap = multiArea.ReadCompositeUiSnapshot();
            int staleTotal = multiSession.ReadHistoryIndex(pageSize: 1).TotalRecordedYears;
            var staleBounded = BoundSnapshotForResponse(staleSnap, staleTotal);
            await WriteJsonAsync(context, StatusCodes.Status409Conflict, new MultiSkipPayload(
                null, null, request.TargetYear, Array.Empty<MultiSkipStepDto>(), false, null, null, null, null, null,
                staleBounded.View, staleBounded.Scenes, staleBounded.Stage4, staleBounded.Ecology,
                new Notice(refusal.Code, refusal.Message), staleBounded.HistoryTruncated, staleBounded.TotalRecordedYears,
                staleBounded.ProjectionCompacted, staleBounded.Stage4Compacted, staleBounded.PayloadBudgetExceeded));
            return;
        }

        var snapshot = multiArea.ReadCompositeUiSnapshot();
        int totalRecordedYears = multiSession.ReadHistoryIndex(pageSize: 1).TotalRecordedYears;
        var bounded = BoundSnapshotForResponse(snapshot, totalRecordedYears);
        var completed = result.CompletedYears
            .Select(step => new MultiSkipStepDto(step.YearBefore, step.YearAfter, step.YearRecord))
            .ToArray();

        bool isRefusal = result.StopReason is TimePacingStopReason.WorkPlanOpen or TimePacingStopReason.InvalidTargetYear
            || (!result.ReachedTarget && completed.Length == 0);
        Notice? notice = isRefusal
            ? new Notice(result.StopReason.ToString(), result.StopDetail ?? DefaultSkipRefusalMessage(result.StopReason))
            : null;
        int status = isRefusal ? StatusCodes.Status409Conflict : StatusCodes.Status200OK;

        await WriteJsonAsync(context, status, new MultiSkipPayload(
            result.StartYear, result.DestinationYear, result.RequestedTargetYear,
            Array.AsReadOnly(completed), result.ReachedTarget, result.StopReason, result.StopDetail,
            result.IsObservationPeriod, result.AnchorFellYear, result.CalibrationNote,
            bounded.View, bounded.Scenes, bounded.Stage4, bounded.Ecology,
            notice, bounded.HistoryTruncated, bounded.TotalRecordedYears,
            bounded.ProjectionCompacted, bounded.Stage4Compacted, bounded.PayloadBudgetExceeded));
    }

    // Read-only newest-first paged index of recorded years. Clamps pageSize to 1..50 (default 10). 400 on invalid query params.
    private Task ReadMultiHistoryIndexAsync(HttpContext context)
    {
        int pageSize = DefaultHistoryPageSize;
        int? beforeYear = null;
        foreach (var (key, values) in context.Request.Query)
        {
            if (values.Count != 1)
                return WriteErrorAsync(context, StatusCodes.Status400BadRequest, "BadRequest",
                    FormattableString.Invariant($"duplicate query parameter '{key}'; nothing changed"));
            string raw = values[0] ?? "";
            if (string.Equals(key, "pageSize", StringComparison.Ordinal))
            {
                if (!TryParseExactInt(raw, allowNegative: true, out int parsed))
                    return WriteErrorAsync(context, StatusCodes.Status400BadRequest, "BadRequest",
                        "pageSize must be a base-10 integer; nothing changed");
                pageSize = Math.Clamp(parsed, MinHistoryPageSize, MaxHistoryPageSize);
            }
            else if (string.Equals(key, "beforeYear", StringComparison.Ordinal))
            {
                if (!TryParseExactInt(raw, allowNegative: false, out int parsed))
                    return WriteErrorAsync(context, StatusCodes.Status400BadRequest, "BadRequest",
                        "beforeYear must be a non-negative integer; nothing changed");
                beforeYear = parsed;
            }
            else
            {
                return WriteErrorAsync(context, StatusCodes.Status400BadRequest, "BadRequest",
                    FormattableString.Invariant($"unknown query parameter '{key}'; nothing changed"));
            }
        }
        var page = multiSession.ReadHistoryIndex(pageSize, beforeYear);
        return WriteJsonAsync(context, StatusCodes.Status200OK, page);
    }

    // Read-only full Decision -> Outcome detail for WorkYear = year and ObservedYear = year + 1.
    // Validates the caller's fence read-only against the current view before querying ReadHistoryYear(year).
    private async Task ReadMultiHistoryYearAsync(HttpContext context)
    {
        var request = await ReadMultiRequestAsync<MultiHistoryYearRequest>(context, MultiHistoryYearMembers);
        if (request is null) return;
        var currentView = multiArea.Read();
        var fence = request.Fence!;
        Notice? fenceRefusal =
            !string.Equals(fence.SessionId, currentView.SessionId, StringComparison.Ordinal)
                ? new Notice("PlayerSessionMismatch", "this request does not belong to the current multi-area session")
                : fence.Source != currentView.Source
                    ? new Notice("PlayerSourceStale",
                        "the source, year or a revision changed since this view was read; nothing changed, re-read the current view")
                    : fence.PlanStateRevision != currentView.PlanStateRevision
                        ? new Notice("PlayerPlanStateStale",
                            "the Work Plan changed since this view was read; nothing changed, re-read the current view")
                        : null;
        if (fenceRefusal is not null)
        {
            await WriteJsonAsync(context, StatusCodes.Status409Conflict,
                new MultiHistoryYearRefusalPayload(FenceOf(currentView), BoundSessionViewForAux(currentView), fenceRefusal));
            return;
        }

        int year = request.Year!.Value;
        if (year < 0)
        {
            await WriteJsonAsync(context, StatusCodes.Status409Conflict,
                new MultiHistoryYearRefusalPayload(FenceOf(currentView), BoundSessionViewForAux(currentView),
                    new Notice("HistoryYearInvalid", "history year must be non-negative")));
            return;
        }
        if (year > currentView.Source.EcologicalYear)
        {
            await WriteJsonAsync(context, StatusCodes.Status409Conflict,
                new MultiHistoryYearRefusalPayload(FenceOf(currentView), BoundSessionViewForAux(currentView),
                    new Notice("HistoryYearNotRecorded",
                        FormattableString.Invariant($"year {year} exceeds the current ecological year ({currentView.Source.EcologicalYear}) and has no recorded data"))));
            return;
        }

        HistoryYearDetail detail;
        try
        {
            detail = multiSession.ReadHistoryYear(year);
        }
        catch (P1Refusal refusal)
        {
            var fresh = multiArea.Read();
            await WriteJsonAsync(context, StatusCodes.Status409Conflict,
                new MultiHistoryYearRefusalPayload(FenceOf(fresh), BoundSessionViewForAux(fresh),
                    new Notice(refusal.Code, refusal.Message)));
            return;
        }

        if (!detail.HasRecordedData)
        {
            await WriteJsonAsync(context, StatusCodes.Status409Conflict,
                new MultiHistoryYearRefusalPayload(FenceOf(currentView), BoundSessionViewForAux(currentView),
                    new Notice("HistoryYearNotRecorded",
                        FormattableString.Invariant($"year {year} has no recorded work or annual outcome in this session"))));
            return;
        }

        await WriteJsonAsync(context, StatusCodes.Status200OK, detail);
    }

    // Read-only nearest-neighbour spatial woodland structure descriptors (M_i, T_i) over the current committed root.
    private Task ReadMultiStructureAsync(HttpContext context)
    {
        if (context.Request.Query.Count > 0)
            return WriteErrorAsync(context, StatusCodes.Status400BadRequest, "BadRequest",
                "structure does not accept query parameters; nothing changed");
        var summary = multiSession.ComputeStructure();
        return WriteJsonAsync(context, StatusCodes.Status200OK, new MultiStructurePayload(
            summary.MeanMingling, summary.MeanDiameterDifferentiation, summary.FocalLivingTreeCount,
            summary.ValidDiameterFocalCount, summary.EffectiveNeighbourCount, summary.MaxNeighbourCountK,
            summary.Areas, summary.ProvenanceLabel, summary.ProvenanceLabel, summary.HonestyLabel));
    }

    private static MultiAreaFence FenceOf(MultiAreaSessionView view) =>
        new(view.SessionId, view.Source, view.PlanStateRevision);

    private MultiReviewPreviewDto? TryPreviewForView(MultiAreaSessionView view)
    {
        try
        {
            var p = multiSession.PreviewNextReviewYear(FenceOf(view));
            bool isOverdue = string.Equals(p.Label, MultiAreaTimePacingConstants.OverdueReviewPeriodLabel, StringComparison.Ordinal);
            return new MultiReviewPreviewDto(p.CurrentYear, p.Eligible, p.SuggestedTargetYear, p.SuggestedGapYears,
                p.AnchorFellYear, p.IsObservationPeriod, isOverdue, p.MaxTicksPerPress, p.StopReason, p.CalibrationNote, p.Label);
        }
        catch (P1Refusal)
        {
            return null;
        }
    }

    private static string DefaultSkipRefusalMessage(TimePacingStopReason reason) => reason switch
    {
        TimePacingStopReason.WorkPlanOpen => MultiAreaTimePacingConstants.WorkPlanOpenLabel,
        TimePacingStopReason.StandCannotAdvance => MultiAreaTimePacingConstants.StandCannotAdvanceLabel,
        TimePacingStopReason.InvalidTargetYear => "requested target year is outside [currentYear + 1, currentYear + 5]",
        _ => "unable to advance years",
    };

    private static bool TryParseExactInt(string raw, bool allowNegative, out int value)
    {
        value = 0;
        if (raw.Length == 0) return false;
        int start = 0;
        if (raw[0] == '-')
        {
            if (!allowNegative || raw.Length == 1) return false;
            start = 1;
        }
        for (int i = start; i < raw.Length; i++)
        {
            if (raw[i] < '0' || raw[i] > '9') return false;
        }
        var styles = allowNegative ? NumberStyles.AllowLeadingSign : NumberStyles.None;
        return int.TryParse(raw, styles, CultureInfo.InvariantCulture, out value);
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
        out byte[] finalBytes) =>
        SerializeBoundedMultiView(snapshot.View, snapshot.Readout, snapshot.Ecology, totalRecordedYears, reviewPreview, out finalBytes);

    private static BoundedMultiSnapshot SerializeBoundedMultiView(
        MultiAreaSessionView fullView,
        MultiAreaUiReadout? fullStage4,
        MultiAreaEcologyDto? fullEcology,
        int totalRecordedYears,
        MultiReviewPreviewDto? reviewPreview,
        out byte[] finalBytes)
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
                    projCompact, s4Compact, exceeded),
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

    private static BoundedMultiSnapshot BoundSnapshotForResponse(MultiAreaCompositeUiSnapshot snapshot, int totalRecordedYears) =>
        SerializeBoundedMultiView(snapshot.View, snapshot.Readout, snapshot.Ecology, totalRecordedYears, reviewPreview: null, out _);

    private static BoundedMultiSnapshot BoundSnapshotForResponse(
        MultiAreaSessionView view, MultiAreaUiReadout? stage4, MultiAreaEcologyDto? ecology, int totalRecordedYears) =>
        SerializeBoundedMultiView(view, stage4, ecology, totalRecordedYears, reviewPreview: null, out _);

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

    // ---- Phase 1 Save/Load ----------------------------------------------------------------------------------------------

    // The slot list. Every slot is read and fully verified here, and the page is told a state word, never a filesystem
    // path: "valid" (loadable into this exact session), "incompatible" (another forest, model or engine build), "corrupt"
    // (failed its own verification) or "missing" (listed but no longer present). A valid backup is reported separately, so
    // a corrupt primary with a good backup is never shown as a silent success or an empty slot.
    private Task ReadMultiSavesAsync(HttpContext context)
    {
        var slots = new List<MultiSlotInfo>();
        var expectation = multiSession.SaveExpectation();
        foreach (string slot in saveSlots.ListSlots())
        {
            var inspection = saveSlots.Inspect(slot);
            string status = "corrupt";
            string? refusal = inspection.PrimaryRefusal;
            string? label = null, savedUtc = null, models = null;
            int? year = null;
            if (!inspection.PrimaryExists)
            {
                status = "missing";
                refusal = "SlotMissing";
            }
            else if (inspection.PrimaryRefusal is null)
            {
                try
                {
                    var header = saveSlots.Load(slot, expectation).Envelope.Header;
                    status = "valid";
                    refusal = null;
                    label = header.Label;
                    year = header.EcologicalYear;
                    savedUtc = header.SavedUtc;
                    models = header.Models.ToString();
                }
                catch (P1Refusal failure)
                {
                    status = IsIdentityRefusal(failure.Code) ? "incompatible" : "corrupt";
                    refusal = failure.Code;
                }
            }
            bool primaryVerifies = inspection.PrimaryExists && inspection.PrimaryRefusal is null;
            bool backupLoadable = inspection.BackupExists && inspection.BackupRefusal is null;
            slots.Add(new MultiSlotInfo(slot, label, year, savedUtc, models, status, status == "valid",
                inspection.BackupExists, backupLoadable, !primaryVerifies && backupLoadable, refusal));
        }
        return WriteJsonAsync(context, StatusCodes.Status200OK, new MultiSavesPayload(slots.AsReadOnly(), null));
    }

    // Save the current fenced forest into a named slot. The engine captures the whole root under its own lock; the strict
    // codec verifies the bytes; the slot store writes outside the lock with its atomic temp/backup/rename sequence. An
    // absent slot is always published no-clobber, and a slot that already holds a save is replaced only when the player
    // confirmed it (replaceExisting). A failed or indeterminate write is never reported as a save.
    private async Task MultiSaveAsync(HttpContext context)
    {
        var request = await ReadMultiRequestAsync<MultiSaveRequest>(context, MultiSaveMembers);
        if (request is null) return;
        string slot = request.Slot!;
        string label = string.IsNullOrEmpty(request.Label) ? slot : request.Label;
        try
        {
            MultiSavePayload payload;
            // The slot gate covers the generation read, the engine capture and the write so two saves of one slot
            // can neither interleave nor record the same generation, and is released before the response is awaited
            // (LockSlot holds a thread-affine Monitor, which must never span an await).
            using (saveSlots.LockSlot(slot))
            {
                var (exists, generation) = saveSlots.NextGeneration(slot);
                if (exists && !request.ReplaceExisting)
                    throw new P1Refusal("SlotExists",
                        "slot '" + slot + "' already holds a save; replacing it needs explicit confirmation");
                // Capture inside the scope (engine lock), then encode and write (no engine lock held).
                var envelope = multiSession.CaptureSave(request.Fence!, label, generation);
                byte[] bytes = Phase1SaveCodec.Encode(envelope);
                var result = saveSlots.Save(slot, bytes, request.ReplaceExisting);
                Interlocked.Increment(ref savesWritten);
                var header = envelope.Header;
                payload = new MultiSavePayload(new MultiSaveOutcome(result.SlotName,
                    result.Status == Phase1SlotCommitStatus.DurablyCommitted ? "durablyCommitted" : "durabilityUnconfirmed",
                    result.BackupUpdated, result.Checksum, result.Bytes, header.SaveGeneration, header.Label, header.EcologicalYear,
                    header.SavedUtc), null);
            }
            await WriteJsonAsync(context, StatusCodes.Status200OK, payload);
        }
        catch (P1Refusal refusal)
        {
            await WriteJsonAsync(context, StatusCodes.Status409Conflict,
                new MultiSavePayload(null, new Notice(refusal.Code, refusal.Message)));
        }
    }

    // Load a slot: the store verifies the file and the exact identity, then the engine rebuilds, revalidates and publishes
    // the whole candidate root under one lock with a NEW session id. Any failure (stale fence, wrong forest, corrupt file,
    // illegal restored order) answers 409 with the code and the UNCHANGED current view, so the page never loses authority
    // over a session that was not replaced.
    private async Task MultiLoadAsync(HttpContext context)
    {
        var request = await ReadMultiRequestAsync<MultiLoadRequest>(context, MultiLoadMembers);
        if (request is null) return;
        MultiAreaLoadResult result;
        try
        {
            result = multiSession.Restore(request.Fence!, saveSlots.Load(request.Slot!, multiSession.SaveExpectation()));
        }
        catch (P1Refusal refusal)
        {
            result = new MultiAreaLoadResult(multiSession.ReadView(), 0, null, refusal.Code, refusal.Message);
        }
        var snapshot = multiArea.ReadCompositeUiSnapshot();
        int totalRecordedYears = multiSession.ReadHistoryIndex(pageSize: 1).TotalRecordedYears;
        var bounded = BoundSnapshotForResponse(snapshot, totalRecordedYears);
        var info = result.RefusalCode is null ? new MultiLoadInfo(result.DraftOrderCount, result.Checksum!) : null;
        await WriteJsonAsync(context, StatusOf(result.RefusalCode), new MultiLoadPayload(bounded.View, bounded.Scenes,
            MultiLabels, bounded.Stage4, bounded.Ecology, info, NoticeOf(result.RefusalCode, result.RefusalMessage),
            bounded.HistoryTruncated, bounded.TotalRecordedYears, bounded.ProjectionCompacted, bounded.Stage4Compacted,
            bounded.PayloadBudgetExceeded));
    }

    // Restore a slot's verified prior-good backup over its primary. This is a deliberate, separate player action and the
    // store refuses it while the primary still verifies (SlotPrimaryValid), so it can never roll a good newer save back.
    private async Task MultiRecoverAsync(HttpContext context)
    {
        var request = await ReadMultiRequestAsync<MultiSlotRequest>(context, MultiRecoverMembers);
        if (request is null) return;
        try
        {
            var result = saveSlots.RecoverFromBackup(request.Slot!);
            await WriteJsonAsync(context, StatusCodes.Status200OK,
                new MultiRecoverPayload(result.SlotName, result.Checksum, result.Bytes, null));
        }
        catch (P1Refusal refusal)
        {
            await WriteJsonAsync(context, StatusCodes.Status409Conflict,
                new MultiRecoverPayload(request.Slot!, null, null, new Notice(refusal.Code, refusal.Message)));
        }
    }

    // A save made for another experiment, model selection or engine build is compatible-looking but not loadable here.
    private static bool IsIdentityRefusal(string code) =>
        code is "SaveModelMismatch" or "SaveGenesisMismatch" or "SaveLineageMismatch";

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
        var result = multiArea.ReadCellDetail(request.Fence!, request.AreaId!, column, row);
        var snap = result.Snapshot;
        int totalRecordedYears = multiSession.ReadHistoryIndex(pageSize: 1).TotalRecordedYears;
        var bounded = BoundSnapshotForResponse(snap, totalRecordedYears);
        await WriteJsonAsync(context, StatusOf(result.RefusalCode), new MultiCellPayload(bounded.View, bounded.Scenes,
            result.Cell, NoticeOf(result.RefusalCode, result.RefusalMessage), bounded.Stage4, bounded.Ecology,
            bounded.HistoryTruncated, bounded.TotalRecordedYears, bounded.ProjectionCompacted, bounded.Stage4Compacted,
            bounded.PayloadBudgetExceeded));
    }

    private async Task ReadMultiCompetitionAsync(HttpContext context)
    {
        var request = await ReadMultiRequestAsync<MultiTreeRequest>(context, MultiTreeMembers);
        if (request is null) return;
        var result = multiArea.Competition(request.Fence!, request.AreaId!, request.TreeId!);
        var composite = multiArea.CompositeFor(result.View);
        int totalRecordedYears = multiSession.ReadHistoryIndex(pageSize: 1).TotalRecordedYears;
        if (composite is not null)
        {
            var bounded = BoundSnapshotForResponse(result.View, composite.Stage4, composite.Ecology, totalRecordedYears);
            await WriteJsonAsync(context, StatusOf(result.RefusalCode), new MultiCompetitionPayload(bounded.View, bounded.Scenes,
                result.AreaId, result.Competition, NoticeOf(result.RefusalCode, result.RefusalMessage),
                bounded.Stage4, bounded.Ecology, bounded.HistoryTruncated, bounded.TotalRecordedYears,
                bounded.ProjectionCompacted, bounded.Stage4Compacted, bounded.PayloadBudgetExceeded));
        }
        else
        {
            await WriteJsonAsync(context, StatusOf(result.RefusalCode), new MultiCompetitionPayload(result.View, ScenesOf(result.View),
                result.AreaId, result.Competition, NoticeOf(result.RefusalCode, result.RefusalMessage),
                null, null, false, totalRecordedYears));
        }
    }

    private async Task MultiHitTestAsync(HttpContext context)
    {
        var request = await ReadMultiRequestAsync<MultiHitRequest>(context, MultiHitMembers);
        if (request is null) return;
        var result = multiArea.HitTest(request.Fence!, request.AreaId!, request.U!.Value, request.V!.Value);
        var composite = multiArea.CompositeFor(result.View);
        int totalRecordedYears = multiSession.ReadHistoryIndex(pageSize: 1).TotalRecordedYears;
        if (composite is not null)
        {
            var bounded = BoundSnapshotForResponse(result.View, composite.Stage4, composite.Ecology, totalRecordedYears);
            await WriteJsonAsync(context, StatusOf(result.RefusalCode), new MultiHitPayload(bounded.View, bounded.Scenes,
                result.AreaId, result.Candidates, NoticeOf(result.RefusalCode, result.RefusalMessage),
                bounded.Stage4, bounded.Ecology, bounded.HistoryTruncated, bounded.TotalRecordedYears,
                bounded.ProjectionCompacted, bounded.Stage4Compacted, bounded.PayloadBudgetExceeded));
        }
        else
        {
            await WriteJsonAsync(context, StatusOf(result.RefusalCode), new MultiHitPayload(result.View, ScenesOf(result.View),
                result.AreaId, result.Candidates, NoticeOf(result.RefusalCode, result.RefusalMessage),
                null, null, false, totalRecordedYears));
        }
    }

    private async Task MultiCommandAsync<TRequest>(HttpContext context, IReadOnlyList<string> required,
        Func<TRequest, MultiAreaCommandResult> command) where TRequest : class
    {
        var request = await ReadMultiRequestAsync<TRequest>(context, required);
        if (request is null) return;
        var result = command(request);
        var composite = multiArea.CompositeFor(result.View);
        int totalRecordedYears = multiSession.ReadHistoryIndex(pageSize: 1).TotalRecordedYears;
        if (composite is not null)
        {
            var bounded = BoundSnapshotForResponse(result.View, composite.Stage4, composite.Ecology, totalRecordedYears);
            await WriteJsonAsync(context, StatusOf(result.RefusalCode), new MultiCommandPayload(bounded.View, bounded.Scenes,
                NoticeOf(result.RefusalCode, result.RefusalMessage), result.Execution, result.Annual,
                bounded.Stage4, bounded.Ecology, bounded.HistoryTruncated, bounded.TotalRecordedYears,
                bounded.ProjectionCompacted, bounded.Stage4Compacted, bounded.PayloadBudgetExceeded));
        }
        else
        {
            await WriteJsonAsync(context, StatusOf(result.RefusalCode), new MultiCommandPayload(result.View, ScenesOf(result.View),
                NoticeOf(result.RefusalCode, result.RefusalMessage), result.Execution, result.Annual,
                null, null, false, totalRecordedYears));
        }
    }

    private static IReadOnlyDictionary<string, IReadOnlyList<PlayerMapItem>> ScenesOf(MultiAreaSessionView view) =>
        view.Areas.ToDictionary(area => area.AreaId, area => MultiAreaProjection.Scene(view, area), StringComparer.Ordinal);

    private static async Task<T?> ReadMultiRequestAsync<T>(HttpContext context, IReadOnlyList<string> required) where T : class
    {
        if (!IsJsonContentType(context.Request.ContentType))
        {
            await WriteErrorAsync(context, StatusCodes.Status415UnsupportedMediaType, "UnsupportedMediaType",
                "send Content-Type: application/json; nothing changed");
            return null;
        }
        byte[]? body = await ReadBoundedBodyAsync(context.Request, context.RequestAborted);
        if (body is null)
        {
            await WriteErrorAsync(context, StatusCodes.Status413PayloadTooLarge, "RequestTooLarge",
                FormattableString.Invariant($"request bodies are limited to {MaxBodyBytes} bytes; nothing changed"));
            return null;
        }
        T? request = null;
        string problem = "the body must be one JSON object";
        try
        {
            using var document = JsonDocument.Parse(body);
            RequireUniqueMembers(document.RootElement);
            foreach (string path in required) RequireMember(document.RootElement, path);
            RequireBoundedText(document.RootElement);
            request = JsonSerializer.Deserialize<T>(body, RequestJson);
        }
        catch (JsonException parse)
        {
            problem = parse.Message;
        }
        if (request is null)
            await WriteErrorAsync(context, StatusCodes.Status400BadRequest, "BadRequest", problem + "; nothing changed");
        return request;
    }

    private static void RequireMember(JsonElement root, string path)
    {
        var current = root;
        foreach (string name in path.Split('.'))
        {
            if (current.ValueKind != JsonValueKind.Object || !current.TryGetProperty(name, out var next) ||
                next.ValueKind == JsonValueKind.Null)
                throw new JsonException("missing or null member: " + path);
            current = next;
        }
    }

    private static void RequireBoundedText(JsonElement root)
    {
        if (root.ValueKind != JsonValueKind.Object) throw new JsonException("the body must be one JSON object");
        foreach (var (name, limit) in new[] { ("areaId", MaxIdLength), ("treeId", MaxIdLength), ("speciesId", MaxIdLength),
                     ("orderId", MaxIdLength), ("planId", MaxPlanKeyLength), ("executionKey", MaxPlanKeyLength),
                     ("slot", 48), ("label", 64) })
        {
            if (!root.TryGetProperty(name, out var value) || value.ValueKind != JsonValueKind.String) continue;
            string text = value.GetString()!;
            if (text.Length == 0 || text.Length > limit || text.Any(ch => char.IsWhiteSpace(ch) || char.IsControl(ch)))
                throw new JsonException(FormattableString.Invariant($"{name} must be 1-{limit} characters without whitespace"));
        }
        if (root.TryGetProperty("reason", out var reason) && reason.ValueKind == JsonValueKind.String &&
            reason.GetString()!.Length > MaxReasonLength)
            throw new JsonException(FormattableString.Invariant($"reason is limited to {MaxReasonLength} characters"));
    }

    private sealed record MultiFenceRequest(MultiAreaFence? Fence);

    private sealed record MultiTreeRequest(MultiAreaFence? Fence, string? AreaId, string? TreeId);

    private sealed record MultiAddRequest(MultiAreaFence? Fence, string? AreaId, string? TreeId, string? Reason);

    private sealed record MultiExecuteRequest(MultiAreaFence? Fence, string? PlanId, string? ExecutionKey);

    private sealed record MultiAdvanceRequest(MultiAreaFence? Fence, bool ConsentToCancelWork);

    private sealed record MultiSkipRequest(MultiAreaFence? Fence, int? TargetYear);

    private sealed record MultiHistoryYearRequest(MultiAreaFence? Fence, int? Year);

    private sealed record MultiHitRequest(MultiAreaFence? Fence, string? AreaId, double? U, double? V);

    private sealed record MultiCellRequest(MultiAreaFence? Fence, string? AreaId, int? Column, int? Row);

    private sealed record MultiPlantRequest(
        MultiAreaFence? Fence,
        string? AreaId,
        string? SpeciesId,
        [property: JsonPropertyName("xM")] double? XM,
        [property: JsonPropertyName("zM")] double? ZM);

    private sealed record MultiPointRequest(
        MultiAreaFence? Fence,
        string? AreaId,
        [property: JsonPropertyName("xM")] double? XM,
        [property: JsonPropertyName("zM")] double? ZM);

    private sealed record MultiPlantingRemoveRequest(MultiAreaFence? Fence, string? OrderId);

    private static byte[] MultiEmbeddedAsset(string logicalName)
    {
        using var stream = typeof(PlayerApi).Assembly.GetManifestResourceStream(logicalName)
            ?? throw new InvalidOperationException("the /multi page asset " + logicalName + " is not embedded in this build");
        using var copy = new MemoryStream();
        stream.CopyTo(copy);
        return copy.ToArray();
    }

    // Phase 1 Save/Load request and response bodies. `replaceExisting` is required on every save so the page must state
    // whether the player confirmed replacing an existing slot.
    private sealed record MultiSaveRequest(MultiAreaFence? Fence, string? Slot, string? Label, bool ReplaceExisting);

    private sealed record MultiLoadRequest(MultiAreaFence? Fence, string? Slot);

    private sealed record MultiSlotRequest(string? Slot);

    private sealed record MultiSlotInfo(string Slot, string? Label, int? EcologicalYear, string? SavedUtc, string? Models,
        string Status, bool CanLoad, bool BackupExists, bool BackupLoadable, bool Recoverable, string? RefusalCode);

    private sealed record MultiSavesPayload(IReadOnlyList<MultiSlotInfo> Slots, Notice? Refusal);

    private sealed record MultiSaveOutcome(string Slot, string Status, bool BackupUpdated, string Checksum, long Bytes,
        long SaveGeneration, string Label, int EcologicalYear, string SavedUtc);

    private sealed record MultiSavePayload(MultiSaveOutcome? Saved, Notice? Refusal);

    private sealed record MultiLoadInfo(int DraftOrderCount, string Checksum);

    private sealed record MultiLoadPayload(object View, object Scenes,
        IReadOnlyDictionary<string, string> Labels, object? Stage4, MultiAreaEcologyDto? Ecology, MultiLoadInfo? Restored,
        Notice? Refusal, bool HistoryTruncated = false, int TotalRecordedYears = 0,
        bool ProjectionCompacted = false, bool Stage4Compacted = false, bool PayloadBudgetExceeded = false);

    private sealed record MultiRecoverPayload(string Slot, string? Checksum, long? Bytes, Notice? Refusal);

    // Stage4 ("stage4"), Ecology ("ecology"), and Step 2 bounding/preview fields are additive; all existing members keep their exact names and shapes.
    private sealed record MultiViewPayload(object View, object Scenes,
        IReadOnlyDictionary<string, string> Labels, object? Stage4, MultiAreaEcologyDto? Ecology,
        bool HistoryTruncated = false, int TotalRecordedYears = 0, MultiReviewPreviewDto? ReviewPreview = null,
        bool ProjectionCompacted = false, bool Stage4Compacted = false, bool PayloadBudgetExceeded = false);

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

    private sealed record MultiReviewPreviewDto(
        int CurrentYear,
        bool Eligible,
        int SuggestedTargetYear,
        int SuggestedGapYears,
        int? AnchorFellYear,
        bool IsObservationPeriod,
        bool IsOverdue,
        int MaxTicksPerPress,
        TimePacingStopReason? StopReason,
        string CalibrationNote,
        string Label);

    private sealed record MultiPreviewReviewPayload(
        int? CurrentYear,
        bool? Eligible,
        int? SuggestedTargetYear,
        int? SuggestedGapYears,
        int? AnchorFellYear,
        bool? IsObservationPeriod,
        bool? IsOverdue,
        int? MaxTicksPerPress,
        TimePacingStopReason? StopReason,
        string? CalibrationNote,
        string? Label,
        MultiAreaFence Fence,
        object View,
        Notice? Refusal);

    private sealed record MultiSkipStepDto(int YearBefore, int YearAfter, MultiAreaYearRecord YearRecord);

    private sealed record MultiSkipPayload(
        int? StartYear,
        int? DestinationYear,
        int? RequestedTargetYear,
        IReadOnlyList<MultiSkipStepDto> CompletedYears,
        bool ReachedTarget,
        TimePacingStopReason? StopReason,
        string? StopDetail,
        bool? IsObservationPeriod,
        int? AnchorFellYear,
        string? CalibrationNote,
        object View,
        object Scenes,
        object? Stage4,
        MultiAreaEcologyDto? Ecology,
        Notice? Refusal,
        bool HistoryTruncated = false,
        int TotalRecordedYears = 0,
        bool ProjectionCompacted = false,
        bool Stage4Compacted = false,
        bool PayloadBudgetExceeded = false);

    private sealed record MultiHistoryYearRefusalPayload(
        MultiAreaFence Fence,
        object View,
        Notice Refusal);

    private sealed record MultiStructurePayload(
        double? MeanMingling,
        double? MeanDiameterDifferentiation,
        int FocalLivingTreeCount,
        int ValidDiameterFocalCount,
        int EffectiveNeighbourCount,
        int MaxNeighbourCountK,
        IReadOnlyList<WoodlandAreaStructureSummary> Areas,
        string Provenance,
        string ProvenanceLabel,
        string HonestyLabel);

    private sealed record MultiCommandPayload(object View, object Scenes,
        Notice? Refusal, MultiAreaExecutionResult? Execution, MultiAreaAnnualResult? Annual, object? Stage4,
        MultiAreaEcologyDto? Ecology, bool HistoryTruncated = false, int TotalRecordedYears = 0,
        bool ProjectionCompacted = false, bool Stage4Compacted = false, bool PayloadBudgetExceeded = false);

    private sealed record MultiCompetitionPayload(object View,
        object Scenes, string AreaId, PlayerCompetitionReport? Competition,
        Notice? Refusal, object? Stage4, MultiAreaEcologyDto? Ecology,
        bool HistoryTruncated = false, int TotalRecordedYears = 0,
        bool ProjectionCompacted = false, bool Stage4Compacted = false, bool PayloadBudgetExceeded = false);

    private sealed record MultiHitPayload(object View, object Scenes,
        string AreaId, IReadOnlyList<PlayerMapItem> Candidates, Notice? Refusal, object? Stage4,
        MultiAreaEcologyDto? Ecology, bool HistoryTruncated = false, int TotalRecordedYears = 0,
        bool ProjectionCompacted = false, bool Stage4Compacted = false, bool PayloadBudgetExceeded = false);

    private sealed record MultiCellPayload(object View, object Scenes,
        MultiAreaCellDetail? Cell, Notice? Refusal, object? Stage4, MultiAreaEcologyDto? Ecology,
        bool HistoryTruncated = false, int TotalRecordedYears = 0,
        bool ProjectionCompacted = false, bool Stage4Compacted = false, bool PayloadBudgetExceeded = false);
}
