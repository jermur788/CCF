using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using SitkaP1.Core;

namespace SitkaP1.Ui;

internal sealed partial class PlayerApi
{
    private async Task MultiCommandAsync<TRequest>(HttpContext context, IReadOnlyList<string> required,
        Func<MultiSessionPair, TRequest, MultiAreaCommandResult> command) where TRequest : class
    {
        var request = await ReadMultiRequestAsync<TRequest>(context, required);
        if (request is null) return;
        var pair = CurrentMulti;
        var result = command(pair, request);
        var composite = pair.Adapter.CompositeFor(result.View);
        int totalRecordedYears = pair.Session.ReadHistoryIndex(pageSize: 1).TotalRecordedYears;
        var economy = TryBuildEconomySnapshot(pair.Session, result.View);
        if (composite is not null)
        {
            var bounded = BoundSnapshotForResponse(result.View, composite.Stage4, composite.Ecology, totalRecordedYears, economy);
            await WriteJsonAsync(context, StatusOf(result.RefusalCode), new MultiCommandPayload(bounded.View, bounded.Scenes,
                NoticeOf(result.RefusalCode, result.RefusalMessage), result.Execution, result.Annual,
                bounded.Stage4, bounded.Ecology, bounded.HistoryTruncated, bounded.TotalRecordedYears,
                bounded.ProjectionCompacted, bounded.Stage4Compacted, bounded.PayloadBudgetExceeded, economy));
        }
        else
        {
            await WriteJsonAsync(context, StatusOf(result.RefusalCode), new MultiCommandPayload(result.View, ScenesOf(result.View),
                NoticeOf(result.RefusalCode, result.RefusalMessage), result.Execution, result.Annual,
                null, null, false, totalRecordedYears, Economy: economy));
        }
    }

    // Read-only revision-fenced Economy snapshot (GET api/multi/economy/view). Refuses 409 EconomyModelRequired when Off.
    private Task ReadMultiEconomyViewAsync(HttpContext context)
    {
        if (context.Request.Query.Count > 0)
            return WriteErrorAsync(context, StatusCodes.Status400BadRequest, "BadRequest",
                "economy view does not accept query parameters; nothing changed");
        var pair = CurrentMulti;
        try
        {
            var econView = pair.Session.ReadEconomyView();
            EconomyWorkPlanQuote? quote = econView.ApprovedQuote?.Quote;
            bool frozen = econView.ApprovedQuote is not null;
            if (!frozen)
            {
                var fence = new MultiAreaFence(econView.SessionId, econView.Source, econView.PlanStateRevision);
                var pending = pair.Session.QuoteWorkPlan(fence);
                if (pending.HarvestJob is not null || pending.NonHarvestLines.Count > 0) quote = pending;
            }
            var dto = BuildEconomySnapshot(econView, quote, frozen);
            return WriteJsonAsync(context, StatusCodes.Status200OK, new MultiEconomyViewPayload(
                econView.SessionId, econView.PlanStateRevision, econView.Source, dto, Refusal: null));
        }
        catch (P1Refusal refusal)
        {
            var view = pair.Adapter.Read();
            return WriteJsonAsync(context, StatusCodes.Status409Conflict, new MultiEconomyViewPayload(
                view.SessionId, view.PlanStateRevision, view.Source, Economy: null, new Notice(refusal.Code, refusal.Message)));
        }
    }

    // Launch-time Money On/Off toggle (D2a): starts a fresh Year-0 session of the active stand under multiSwapSync.
    // Requires explicit confirmReset=true when the current session is past Year 0 or has pending/approved/executed work.
    private async Task SwitchMultiEconomySessionAsync(HttpContext context)
    {
        var request = await ReadMultiRequestAsync<MultiEconomySessionRequest>(context, MultiEconomySessionMembers);
        if (request is null) return;
        MultiSessionPair activePair;
        Notice? refusal = null;
        lock (multiSwapSync)
        {
            var current = CurrentMulti;
            var currentView = current.Adapter.Read();
            refusal = ValidateFenceNotice(request.Fence!, currentView);
            if (refusal is null)
            {
                bool currentEnabled = current.Session.SelectedModels.IsEconomyEnabled;
                if (request.Enabled != currentEnabled)
                {
                    var snap = current.Adapter.ReadCompositeUiSnapshot();
                    bool dirty = currentView.Source.EcologicalYear > 0
                        || currentView.Pending.Count > 0
                        || currentView.Approved is not null
                        || currentView.Events.Count > 0
                        || (snap.Readout?.PendingPrunes.Count ?? 0) > 0
                        || (snap.Readout?.ApprovedPrunes.Count ?? 0) > 0
                        || (snap.Ecology?.PendingClear.Count ?? 0) > 0
                        || (snap.Ecology?.ApprovedClear.Count ?? 0) > 0
                        || (snap.Ecology?.PendingPlanting.Count ?? 0) > 0
                        || (snap.Ecology?.ApprovedPlanting.Count ?? 0) > 0;
                    if (dirty && !request.ConfirmReset)
                    {
                        refusal = new Notice("EconomyResetConfirmationRequired",
                            "switching Money mode starts a fresh Year-0 session and discards unsaved progress; confirm reset to proceed");
                    }
                    else
                    {
                        var fresh = OpenFreshMultiSession(current.Adapter.ExperimentKey, request.Enabled);
                        Volatile.Write(ref multiCurrent, fresh);
                    }
                }
            }
            activePair = CurrentMulti;
        }
        var outSnap = activePair.Adapter.ReadCompositeUiSnapshot();
        int totalRecordedYears = activePair.Session.ReadHistoryIndex(pageSize: 1).TotalRecordedYears;
        MultiReviewPreviewDto? reviewPreview = TryPreviewForView(activePair, outSnap.View);
        var economy = TryBuildEconomySnapshot(activePair.Session, outSnap.View);
        var bounded = BoundSnapshotForResponse(outSnap, totalRecordedYears, economy);
        await WriteJsonAsync(context, StatusOf(refusal?.Code), new MultiEconomySessionPayload(
            bounded.View, bounded.Scenes, MultiLabels, bounded.Stage4, bounded.Ecology,
            refusal, bounded.HistoryTruncated, bounded.TotalRecordedYears, reviewPreview,
            bounded.ProjectionCompacted, bounded.Stage4Compacted, bounded.PayloadBudgetExceeded, economy));
    }

    // Pure read-only Work Plan economic quote (POST api/multi/economy/quote). Refuses 409 when Off or fence is stale.
    private async Task QuoteMultiEconomyAsync(HttpContext context)
    {
        var request = await ReadMultiRequestAsync<MultiFenceRequest>(context, MultiFenceMembers);
        if (request is null) return;
        var pair = CurrentMulti;
        try
        {
            var quote = pair.Session.QuoteWorkPlan(request.Fence!);
            var view = pair.Adapter.Read();
            var econ = TryBuildEconomySnapshot(pair.Session, view);
            await WriteJsonAsync(context, StatusCodes.Status200OK, new MultiEconomyQuotePayload(
                ProjectEconomyQuote(quote), FenceOf(view), BoundSessionViewForAux(view), econ, Refusal: null));
        }
        catch (P1Refusal refusal)
        {
            var view = pair.Adapter.Read();
            var econ = TryBuildEconomySnapshot(pair.Session, view);
            await WriteJsonAsync(context, StatusCodes.Status409Conflict, new MultiEconomyQuotePayload(
                Quote: null, FenceOf(view), BoundSessionViewForAux(view), econ, new Notice(refusal.Code, refusal.Message)));
        }
    }

    // Per-order executor assignment (POST api/multi/economy/executor). Refuses Fell->Landowner (EconomyFellContractorOnly)
    // and edits while approved (WorkPlanAlreadyApproved); state unchanged on refusal.
    private Task SetMultiOrderExecutorAsync(HttpContext context) =>
        MultiCommandAsync<MultiEconomyExecutorRequest>(context, MultiEconomyExecutorMembers, (pair, request) =>
            RefusableMulti(pair, () =>
            {
                if (!Enum.TryParse<EconomyOrderKind>(request.Kind, ignoreCase: false, out var kind) || !Enum.IsDefined(kind))
                    throw new P1Refusal("EconomyOrderUnknown", "unknown work order kind: " + request.Kind);
                if (!Enum.TryParse<WorkExecutor>(request.Executor, ignoreCase: false, out var executor) || !Enum.IsDefined(executor))
                    throw new P1Refusal("EconomyExecutorUnknown", "unknown work executor: " + request.Executor);
                var key = new EconomyOrderKey(kind, request.AreaId!, request.TargetKey!);
                return pair.Session.SetOrderExecutor(request.Fence!, key, executor);
            }));

    private sealed record MultiCommandPayload(object View, object Scenes,
        Notice? Refusal, MultiAreaExecutionResult? Execution, MultiAreaAnnualResult? Annual, object? Stage4,
        MultiAreaEcologyDto? Ecology, bool HistoryTruncated = false, int TotalRecordedYears = 0,
        bool ProjectionCompacted = false, bool Stage4Compacted = false, bool PayloadBudgetExceeded = false,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] MultiEconomySnapshotDto? Economy = null);

    private sealed record MultiEconomyViewPayload(
        string SessionId,
        int PlanStateRevision,
        MultiAreaSourceToken Source,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] MultiEconomySnapshotDto? Economy,
        Notice? Refusal);

    private sealed record MultiEconomySessionPayload(
        object View,
        object Scenes,
        IReadOnlyDictionary<string, string> Labels,
        object? Stage4,
        MultiAreaEcologyDto? Ecology,
        Notice? Refusal,
        bool HistoryTruncated = false,
        int TotalRecordedYears = 0,
        MultiReviewPreviewDto? ReviewPreview = null,
        bool ProjectionCompacted = false,
        bool Stage4Compacted = false,
        bool PayloadBudgetExceeded = false,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] MultiEconomySnapshotDto? Economy = null);

    private sealed record MultiEconomyQuotePayload(
        MultiEconomyQuoteDto? Quote,
        MultiAreaFence Fence,
        object View,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] MultiEconomySnapshotDto? Economy,
        Notice? Refusal);
}
