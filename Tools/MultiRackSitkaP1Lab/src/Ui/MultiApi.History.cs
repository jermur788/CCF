using System.Globalization;
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
        var pair = CurrentMulti;
        try
        {
            var preview = pair.Session.PreviewNextReviewYear(request.Fence!);
            var view = pair.Adapter.Read();
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
            var view = pair.Adapter.Read();
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
        var pair = CurrentMulti;
        AdvanceYearsResult result;
        try
        {
            result = pair.Session.AdvanceYears(request.Fence!, request.TargetYear!.Value, context.RequestAborted);
        }
        catch (P1Refusal refusal)
        {
            var staleSnap = pair.Adapter.ReadCompositeUiSnapshot();
            int staleTotal = pair.Session.ReadHistoryIndex(pageSize: 1).TotalRecordedYears;
            var staleEcon = TryBuildEconomySnapshot(pair.Session, staleSnap.View);
            var staleBounded = BoundSnapshotForResponse(staleSnap, staleTotal, staleEcon);
            await WriteJsonAsync(context, StatusCodes.Status409Conflict, new MultiSkipPayload(
                null, null, request.TargetYear, Array.Empty<MultiSkipStepDto>(), false, null, null, null, null, null,
                staleBounded.View, staleBounded.Scenes, staleBounded.Stage4, staleBounded.Ecology,
                new Notice(refusal.Code, refusal.Message), staleBounded.HistoryTruncated, staleBounded.TotalRecordedYears,
                staleBounded.ProjectionCompacted, staleBounded.Stage4Compacted, staleBounded.PayloadBudgetExceeded, staleEcon));
            return;
        }

        var snapshot = pair.Adapter.ReadCompositeUiSnapshot();
        int totalRecordedYears = pair.Session.ReadHistoryIndex(pageSize: 1).TotalRecordedYears;
        var economy = TryBuildEconomySnapshot(pair.Session, snapshot.View);
        var bounded = BoundSnapshotForResponse(snapshot, totalRecordedYears, economy);
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
            bounded.ProjectionCompacted, bounded.Stage4Compacted, bounded.PayloadBudgetExceeded, economy));
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
        var page = CurrentMulti.Session.ReadHistoryIndex(pageSize, beforeYear);
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
        var pair = CurrentMulti;
        var currentView = pair.Adapter.Read();
        Notice? fenceRefusal = ValidateFenceNotice(request.Fence!, currentView);
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
            detail = pair.Session.ReadHistoryYear(year);
        }
        catch (P1Refusal refusal)
        {
            var fresh = pair.Adapter.Read();
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

    // Read-only per-year Economy financial history (POST api/multi/economy/history/year).
    // Returns NOT RECORDED with null cents when Off; recorded financial ledger rows when On.
    private async Task ReadMultiEconomyHistoryYearAsync(HttpContext context)
    {
        var request = await ReadMultiRequestAsync<MultiHistoryYearRequest>(context, MultiHistoryYearMembers);
        if (request is null) return;
        var pair = CurrentMulti;
        var currentView = pair.Adapter.Read();
        Notice? fenceRefusal = ValidateFenceNotice(request.Fence!, currentView);
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

        try
        {
            var econHistory = pair.Session.ReadEconomyHistoryYear(year);
            await WriteJsonAsync(context, StatusCodes.Status200OK, ProjectEconomyHistoryYear(econHistory));
        }
        catch (P1Refusal refusal)
        {
            var fresh = pair.Adapter.Read();
            await WriteJsonAsync(context, StatusCodes.Status409Conflict,
                new MultiHistoryYearRefusalPayload(FenceOf(fresh), BoundSessionViewForAux(fresh),
                    new Notice(refusal.Code, refusal.Message)));
        }
    }

    // Read-only nearest-neighbour spatial woodland structure descriptors (M_i, T_i) over the current committed root.
    private Task ReadMultiStructureAsync(HttpContext context)
    {
        if (context.Request.Query.Count > 0)
            return WriteErrorAsync(context, StatusCodes.Status400BadRequest, "BadRequest",
                "structure does not accept query parameters; nothing changed");
        var summary = CurrentMulti.Session.ComputeStructure();
        return WriteJsonAsync(context, StatusCodes.Status200OK, new MultiStructurePayload(
            summary.MeanMingling, summary.MeanDiameterDifferentiation, summary.FocalLivingTreeCount,
            summary.ValidDiameterFocalCount, summary.EffectiveNeighbourCount, summary.MaxNeighbourCountK,
            summary.Areas, summary.ProvenanceLabel, summary.ProvenanceLabel, summary.HonestyLabel));
    }

    private static MultiAreaFence FenceOf(MultiAreaSessionView view) =>
        new(view.SessionId, view.Source, view.PlanStateRevision);

    private static MultiReviewPreviewDto? TryPreviewForView(MultiSessionPair pair, MultiAreaSessionView view)
    {
        try
        {
            var p = pair.Session.PreviewNextReviewYear(FenceOf(view));
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
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] MultiEconomySnapshotDto? Economy = null);

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
