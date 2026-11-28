using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using SitkaP1.Core;

namespace SitkaP1.Ui;

internal sealed partial class PlayerApi
{
    // ---- Phase 1 Step 2: Time Pacing, Decision -> Outcome History, and Woodland Structure -------------------------------

    // Read-only deterministic preview of the next suggested management review year. Zero mutation.
    private async Task PreviewMultiReviewAsync(HttpContext context)
    {
        var request = await ReadMultiRequestAsync<MultiFenceRequest>(context, MultiFenceMembers);
        if (request is null) return;
        int status;
        MultiPreviewReviewPayload payload;
        MultiViewBudgetLimit budget;
        lock (multiHost.SyncLock)
        {
            var active = multiHost.Current;
            try
            {
                var preview = active.Session.PreviewNextReviewYear(request.Fence!);
                var view = active.Adapter.Read();
                var fence = FenceOf(view);
                var boundedView = BoundSessionViewForAux(view);
                bool isOverdue = string.Equals(preview.Label, MultiAreaTimePacingConstants.OverdueReviewPeriodLabel, StringComparison.Ordinal);
                status = StatusCodes.Status200OK;
                payload = new MultiPreviewReviewPayload(
                    preview.CurrentYear, preview.Eligible, preview.SuggestedTargetYear, preview.SuggestedGapYears,
                    preview.AnchorFellYear, preview.IsObservationPeriod, isOverdue, preview.MaxTicksPerPress, preview.StopReason,
                    preview.CalibrationNote, preview.Label, fence, boundedView, Refusal: null);
                budget = ActiveStandBudget(view.Areas.Count, active.Session.GenesisStemCount);
            }
            catch (P1Refusal refusal)
            {
                var view = active.Adapter.Read();
                status = StatusCodes.Status409Conflict;
                payload = new MultiPreviewReviewPayload(
                    null, null, null, null, null, null, null, null, null, null, null,
                    FenceOf(view), BoundSessionViewForAux(view), new Notice(refusal.Code, refusal.Message));
                budget = ActiveStandBudget(view.Areas.Count, active.Session.GenesisStemCount);
            }
        }
        await WriteBoundedJsonAsync(context, status, payload, budget);
    }

    // Bounded multi-year advance (1..5 years per press) calling Core's AdvanceYears(fence, targetYear, ct).
    // NEVER passes consentToCancelWork = true and never cancels open work.
    private async Task SkipMultiYearsAsync(HttpContext context)
    {
        var request = await ReadMultiRequestAsync<MultiSkipRequest>(context, MultiSkipMembers);
        if (request is null) return;
        AdvanceYearsResult? result = null;
        Notice? refusalNotice = null;
        MultiAreaCompositeUiSnapshot snapshot;
        int totalRecordedYears;
        bool economyEnabled;
        MultiEconomySnapshotDto? economy;
        int genesisStems;
        Core.WoodlandImport.Inspection.BuiltInWoodlandEntry? inspectionBlock;
        lock (multiHost.SyncLock)
        {
            var active = multiHost.Current;
            // Commit-time survey check: the body is already read, and nothing runs between this check and the skip.
            inspectionBlock = InspectionBlockerLocked();
            if (inspectionBlock is null)
            {
                try
                {
                    result = active.Session.AdvanceYears(request.Fence!, request.TargetYear!.Value, context.RequestAborted);
                }
                catch (P1Refusal refusal)
                {
                    refusalNotice = new Notice(refusal.Code, refusal.Message);
                }
            }
            snapshot = active.Adapter.ReadCompositeUiSnapshot();
            totalRecordedYears = active.Session.ReadHistoryIndex(pageSize: 1).TotalRecordedYears;
            economyEnabled = active.Session.SelectedModels.IsEconomyEnabled;
            economy = TryBuildEconomySnapshot(active.Session, snapshot.View);
            genesisStems = active.Session.GenesisStemCount;
        }
        if (inspectionBlock is not null)
        {
            await WriteInspectionCommitRefusalAsync(context, inspectionBlock);
            return;
        }

        var budget = ActiveStandBudget(snapshot.View.Areas.Count, genesisStems);
        if (refusalNotice is not null)
        {
            var refusalBounded = SerializeBoundedMultiView(snapshot, totalRecordedYears, reviewPreview: null, out byte[] refusalBytes,
                economyEnabled, economy, budget,
                envelopeSerializer: frame => JsonSerializer.SerializeToUtf8Bytes(new MultiSkipPayload(
                    null, null, request.TargetYear, Array.Empty<MultiSkipStepDto>(), false, null, null, null, null, null,
                    frame.View, frame.Scenes, frame.Stage4, frame.Ecology,
                    refusalNotice, frame.HistoryTruncated, totalRecordedYears,
                    frame.ProjectionCompacted, frame.Stage4Compacted, frame.PayloadBudgetExceeded, economyEnabled, economy,
                    frame.PruneLogTruncated), ResponseJson));
            await WriteBoundedBytesAsync(context, StatusCodes.Status409Conflict, refusalBytes, budget, refusalBounded.PayloadBudgetExceeded);
            return;
        }

        var completed = result!.CompletedYears
            .Select(step => new MultiSkipStepDto(step.YearBefore, step.YearAfter, step.YearRecord))
            .ToArray();

        bool isRefusal = result.StopReason is TimePacingStopReason.WorkPlanOpen or TimePacingStopReason.InvalidTargetYear
            || (!result.ReachedTarget && completed.Length == 0);
        Notice? notice = isRefusal
            ? new Notice(result.StopReason.ToString(), result.StopDetail ?? DefaultSkipRefusalMessage(result.StopReason))
            : null;
        int status = isRefusal ? StatusCodes.Status409Conflict : StatusCodes.Status200OK;

        var completedReadOnly = Array.AsReadOnly(completed);
        var bounded = SerializeBoundedMultiView(snapshot, totalRecordedYears, reviewPreview: null, out byte[] rawBytes,
            economyEnabled, economy, budget,
            envelopeSerializer: frame => JsonSerializer.SerializeToUtf8Bytes(new MultiSkipPayload(
                result.StartYear, result.DestinationYear, result.RequestedTargetYear,
                completedReadOnly, result.ReachedTarget, result.StopReason, result.StopDetail,
                result.IsObservationPeriod, result.AnchorFellYear, result.CalibrationNote,
                frame.View, frame.Scenes, frame.Stage4, frame.Ecology,
                notice, frame.HistoryTruncated, totalRecordedYears,
                frame.ProjectionCompacted, frame.Stage4Compacted, frame.PayloadBudgetExceeded, economyEnabled, economy,
                frame.PruneLogTruncated), ResponseJson));
        await WriteBoundedBytesAsync(context, status, rawBytes, budget, bounded.PayloadBudgetExceeded);
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
        var page = multiHost.Current.Session.ReadHistoryIndex(pageSize, beforeYear);
        return WriteJsonAsync(context, StatusCodes.Status200OK, page);
    }

    private static Notice? ValidateFenceNotice(MultiAreaFence fence, MultiAreaSessionView currentView) =>
        !string.Equals(fence.SessionId, currentView.SessionId, StringComparison.Ordinal)
            ? new Notice("PlayerSessionMismatch", "this request does not belong to the current multi-area session")
            : fence.Source != currentView.Source
                ? new Notice("PlayerSourceStale",
                    "the source, year or a revision changed since this view was read; nothing changed, re-read the current view")
                : fence.PlanStateRevision != currentView.PlanStateRevision
                    ? new Notice("PlayerPlanStateStale",
                        "the Work Plan changed since this view was read; nothing changed, re-read the current view")
                    : null;

    // Read-only full Decision -> Outcome detail for WorkYear = year and ObservedYear = year + 1.
    // Validates the caller's fence read-only against the current view before querying ReadHistoryYear(year).
    private async Task ReadMultiHistoryYearAsync(HttpContext context)
    {
        var request = await ReadMultiRequestAsync<MultiHistoryYearRequest>(context, MultiHistoryYearMembers);
        if (request is null) return;
        var active = multiHost.Current;
        var currentView = active.Adapter.Read();
        var budget = ActiveStandBudget(currentView.Areas.Count, active.Session.GenesisStemCount);
        Notice? fenceRefusal = ValidateFenceNotice(request.Fence!, currentView);
        if (fenceRefusal is not null)
        {
            await WriteBoundedJsonAsync(context, StatusCodes.Status409Conflict,
                new MultiHistoryYearRefusalPayload(FenceOf(currentView), BoundSessionViewForAux(currentView), fenceRefusal), budget);
            return;
        }

        int year = request.Year!.Value;
        if (year < 0)
        {
            await WriteBoundedJsonAsync(context, StatusCodes.Status409Conflict,
                new MultiHistoryYearRefusalPayload(FenceOf(currentView), BoundSessionViewForAux(currentView),
                    new Notice("HistoryYearInvalid", "history year must be non-negative")), budget);
            return;
        }
        if (year > currentView.Source.EcologicalYear)
        {
            await WriteBoundedJsonAsync(context, StatusCodes.Status409Conflict,
                new MultiHistoryYearRefusalPayload(FenceOf(currentView), BoundSessionViewForAux(currentView),
                    new Notice("HistoryYearNotRecorded",
                        FormattableString.Invariant($"year {year} exceeds the current ecological year ({currentView.Source.EcologicalYear}) and has no recorded data"))), budget);
            return;
        }

        HistoryYearDetail detail;
        try
        {
            detail = active.Session.ReadHistoryYear(year);
        }
        catch (P1Refusal refusal)
        {
            var fresh = active.Adapter.Read();
            await WriteBoundedJsonAsync(context, StatusCodes.Status409Conflict,
                new MultiHistoryYearRefusalPayload(FenceOf(fresh), BoundSessionViewForAux(fresh),
                    new Notice(refusal.Code, refusal.Message)), budget);
            return;
        }

        if (!detail.HasRecordedData)
        {
            await WriteBoundedJsonAsync(context, StatusCodes.Status409Conflict,
                new MultiHistoryYearRefusalPayload(FenceOf(currentView), BoundSessionViewForAux(currentView),
                    new Notice("HistoryYearNotRecorded",
                        FormattableString.Invariant($"year {year} has no recorded work or annual outcome in this session"))), budget);
            return;
        }

        await WriteBoundedJsonAsync(context, StatusCodes.Status200OK, detail, budget);
    }

    // Read-only per-year Economy financial history (POST api/multi/economy/history/year).
    // Returns NOT RECORDED with null cents when Off; recorded financial ledger rows when On.
    private async Task ReadMultiEconomyHistoryYearAsync(HttpContext context)
    {
        var request = await ReadMultiRequestAsync<MultiHistoryYearRequest>(context, MultiHistoryYearMembers);
        if (request is null) return;
        int status;
        object payload;
        MultiViewBudgetLimit budget;
        lock (multiHost.SyncLock)
        {
            var active = multiHost.Current;
            var currentView = active.Adapter.Read();
            budget = ActiveStandBudget(currentView.Areas.Count, active.Session.GenesisStemCount);
            Notice? fenceRefusal = ValidateFenceNotice(request.Fence!, currentView);
            if (fenceRefusal is not null)
            {
                status = StatusCodes.Status409Conflict;
                payload = new MultiHistoryYearRefusalPayload(FenceOf(currentView), BoundSessionViewForAux(currentView), fenceRefusal);
            }
            else if (request.Year!.Value < 0)
            {
                status = StatusCodes.Status409Conflict;
                payload = new MultiHistoryYearRefusalPayload(FenceOf(currentView), BoundSessionViewForAux(currentView),
                    new Notice("HistoryYearInvalid", "history year must be non-negative"));
            }
            else if (request.Year!.Value > currentView.Source.EcologicalYear)
            {
                status = StatusCodes.Status409Conflict;
                payload = new MultiHistoryYearRefusalPayload(FenceOf(currentView), BoundSessionViewForAux(currentView),
                    new Notice("HistoryYearNotRecorded",
                        FormattableString.Invariant($"year {request.Year.Value} exceeds the current ecological year ({currentView.Source.EcologicalYear}) and has no recorded data")));
            }
            else
            {
                try
                {
                    var econHistory = active.Session.ReadEconomyHistoryYear(request.Year.Value);
                    var options = active.Session.SelectedModels.IsEconomyEnabled ? active.Session.ReadEconomyView().Options : null;
                    status = StatusCodes.Status200OK;
                    payload = ProjectEconomyHistoryYear(econHistory, options);
                }
                catch (P1Refusal refusal)
                {
                    var fresh = active.Adapter.Read();
                    status = StatusCodes.Status409Conflict;
                    payload = new MultiHistoryYearRefusalPayload(FenceOf(fresh), BoundSessionViewForAux(fresh),
                        new Notice(refusal.Code, refusal.Message));
                }
            }
        }
        await WriteBoundedJsonAsync(context, status, payload, budget);
    }

    // Read-only nearest-neighbour spatial woodland structure descriptors (M_i, T_i) over the current committed root.
    private Task ReadMultiStructureAsync(HttpContext context)
    {
        if (context.Request.Query.Count > 0)
            return WriteErrorAsync(context, StatusCodes.Status400BadRequest, "BadRequest",
                "structure does not accept query parameters; nothing changed");
        var summary = multiHost.Current.Session.ComputeStructure();
        return WriteJsonAsync(context, StatusCodes.Status200OK, new MultiStructurePayload(
            summary.MeanMingling, summary.MeanDiameterDifferentiation, summary.FocalLivingTreeCount,
            summary.ValidDiameterFocalCount, summary.EffectiveNeighbourCount, summary.MaxNeighbourCountK,
            summary.Areas, summary.ProvenanceLabel, summary.ProvenanceLabel, summary.HonestyLabel));
    }

    private static MultiAreaFence FenceOf(MultiAreaSessionView view) =>
        new(view.SessionId, view.Source, view.PlanStateRevision);

    private static MultiReviewPreviewDto? TryPreviewForView(MultiSessionState active, MultiAreaSessionView view)
    {
        try
        {
            var p = active.Session.PreviewNextReviewYear(FenceOf(view));
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
        bool PayloadBudgetExceeded = false,
        bool EconomyEnabled = false,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] MultiEconomySnapshotDto? Economy = null,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] bool PruneLogTruncated = false);

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
}
