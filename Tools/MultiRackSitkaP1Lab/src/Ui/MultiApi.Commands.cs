using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using SitkaP1.Core;

namespace SitkaP1.Ui;

internal sealed partial class PlayerApi
{
    private async Task MultiCommandAsync<TRequest>(HttpContext context, IReadOnlyList<string> required,
        Func<MultiSessionState, TRequest, MultiAreaCommandResult> command) where TRequest : class
    {
        var request = await ReadMultiRequestAsync<TRequest>(context, required);
        if (request is null) return;
        MultiAreaCommandResult result;
        (MultiAreaUiReadout? Stage4, MultiAreaEcologyDto? Ecology)? composite;
        int totalRecordedYears;
        bool economyEnabled;
        MultiEconomySnapshotDto? economy;
        bool hasUnsavedProgress;
        int genesisStems;
        lock (multiHost.SyncLock)
        {
            var active = multiHost.Current;
            result = command(active, request);
            var comp = active.Adapter.CompositeFor(result.View);
            composite = comp is null ? null : (comp.Stage4, comp.Ecology);
            totalRecordedYears = active.Session.ReadHistoryIndex(pageSize: 1).TotalRecordedYears;
            economyEnabled = active.Session.SelectedModels.IsEconomyEnabled;
            economy = TryBuildEconomySnapshot(active.Session, result.View);
            hasUnsavedProgress = active.HasUnsavedProgress;
            genesisStems = active.Session.GenesisStemCount;
        }
        var budget = ActiveStandBudget(result.View.Areas.Count, genesisStems);
        if (composite is { } c)
        {
            var bounded = SerializeBoundedMultiView(result.View, c.Stage4, c.Ecology, totalRecordedYears, reviewPreview: null,
                out byte[] rawBytes, economyEnabled, economy, budget, hasUnsavedProgress);
            await WriteBoundedJsonAsync(context, StatusOf(result.RefusalCode), new MultiCommandPayload(bounded.View, bounded.Scenes,
                NoticeOf(result.RefusalCode, result.RefusalMessage), result.Execution, result.Annual,
                bounded.Stage4, bounded.Ecology, bounded.HistoryTruncated, bounded.TotalRecordedYears,
                bounded.ProjectionCompacted, bounded.Stage4Compacted, bounded.PayloadBudgetExceeded, economyEnabled, economy,
                hasUnsavedProgress), budget, bounded.PayloadBudgetExceeded, rawBytes.Length);
        }
        else
        {
            await WriteBoundedJsonAsync(context, StatusOf(result.RefusalCode), new MultiCommandPayload(result.View, ScenesOf(result.View),
                NoticeOf(result.RefusalCode, result.RefusalMessage), result.Execution, result.Annual,
                null, null, false, totalRecordedYears, EconomyEnabled: economyEnabled, Economy: economy,
                HasUnsavedProgress: hasUnsavedProgress), budget);
        }
    }

    // Read-only revision-fenced Economy snapshot (GET api/multi/economy/view). Refuses 409 EconomyModelRequired when Off.
    private Task ReadMultiEconomyViewAsync(HttpContext context)
    {
        if (context.Request.Query.Count > 0)
            return WriteErrorAsync(context, StatusCodes.Status400BadRequest, "BadRequest",
                "economy view does not accept query parameters; nothing changed");
        int status;
        MultiEconomyViewPayload payload;
        lock (multiHost.SyncLock)
        {
            var active = multiHost.Current;
            bool economyEnabled = active.Session.SelectedModels.IsEconomyEnabled;
            try
            {
                var econView = active.Session.ReadEconomyView();
                EconomyWorkPlanQuote? quote = econView.ApprovedQuote?.Quote;
                bool frozen = econView.ApprovedQuote is not null;
                if (!frozen)
                {
                    var fence = new MultiAreaFence(econView.SessionId, econView.Source, econView.PlanStateRevision);
                    var pending = active.Session.QuoteWorkPlan(fence);
                    if (pending.HarvestJob is not null || pending.NonHarvestLines.Count > 0) quote = pending;
                }
                var dto = BuildEconomySnapshot(econView, quote, frozen, active.Session.IsForestMaker);
                status = StatusCodes.Status200OK;
                payload = new MultiEconomyViewPayload(
                    econView.SessionId, econView.PlanStateRevision, econView.Source, economyEnabled, dto, Refusal: null);
            }
            catch (P1Refusal refusal)
            {
                var view = active.Adapter.Read();
                status = StatusCodes.Status409Conflict;
                payload = new MultiEconomyViewPayload(
                    view.SessionId, view.PlanStateRevision, view.Source, economyEnabled, Economy: null, new Notice(refusal.Code, refusal.Message));
            }
        }
        return WriteJsonAsync(context, status, payload);
    }

    // Launch-time Money On/Off toggle (D2a): starts a fresh Year-0 session of the active stand (built-in OR Forest Maker)
    // under multiHost.SyncLock. Requires explicit confirmReset=true when the current session is dirty.
    private async Task SwitchMultiEconomySessionAsync(HttpContext context)
    {
        var request = await ReadMultiRequestAsync<MultiEconomySessionRequest>(context, MultiEconomySessionMembers);
        if (request is null) return;
        MultiAreaCompositeUiSnapshot outSnap;
        int totalRecordedYears;
        MultiReviewPreviewDto? reviewPreview;
        bool economyEnabled;
        MultiEconomySnapshotDto? economy;
        bool hasUnsavedProgress;
        int genesisStems;
        Notice? refusal = null;
        lock (multiHost.SyncLock)
        {
            var current = multiHost.Current;
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
                        var fresh = MultiSessionHost.BuildFreshFromActive(current, request.Enabled);
                        multiHost.SwapLocked(fresh);
                    }
                }
            }
            var active = multiHost.Current;
            outSnap = active.Adapter.ReadCompositeUiSnapshot();
            totalRecordedYears = active.Session.ReadHistoryIndex(pageSize: 1).TotalRecordedYears;
            reviewPreview = TryPreviewForView(active, outSnap.View);
            economyEnabled = active.Session.SelectedModels.IsEconomyEnabled;
            economy = TryBuildEconomySnapshot(active.Session, outSnap.View);
            hasUnsavedProgress = active.HasUnsavedProgress;
            genesisStems = active.Session.GenesisStemCount;
        }
        var budget = ActiveStandBudget(outSnap.View.Areas.Count, genesisStems);
        var bounded = SerializeBoundedMultiView(outSnap, totalRecordedYears, reviewPreview, out byte[] rawBytes,
            economyEnabled, economy, budget, hasUnsavedProgress);
        await WriteBoundedJsonAsync(context, StatusOf(refusal?.Code), new MultiEconomySessionPayload(
            bounded.View, bounded.Scenes, MultiLabels, bounded.Stage4, bounded.Ecology,
            refusal, bounded.HistoryTruncated, bounded.TotalRecordedYears, reviewPreview,
            bounded.ProjectionCompacted, bounded.Stage4Compacted, bounded.PayloadBudgetExceeded, economyEnabled, economy,
            hasUnsavedProgress), budget, bounded.PayloadBudgetExceeded, rawBytes.Length);
    }

    // Pure read-only Work Plan economic quote (POST api/multi/economy/quote). Refuses 409 when Off or fence is stale.
    private async Task QuoteMultiEconomyAsync(HttpContext context)
    {
        var request = await ReadMultiRequestAsync<MultiFenceRequest>(context, MultiFenceMembers);
        if (request is null) return;
        int status;
        MultiEconomyQuotePayload payload;
        MultiViewBudgetLimit budget;
        lock (multiHost.SyncLock)
        {
            var active = multiHost.Current;
            bool economyEnabled = active.Session.SelectedModels.IsEconomyEnabled;
            try
            {
                var quote = active.Session.QuoteWorkPlan(request.Fence!);
                var econOptions = economyEnabled ? active.Session.ReadEconomyView().Options : null;
                var view = active.Adapter.Read();
                var econ = TryBuildEconomySnapshot(active.Session, view);
                status = StatusCodes.Status200OK;
                payload = new MultiEconomyQuotePayload(
                    ProjectEconomyQuote(quote, econOptions), FenceOf(view), BoundSessionViewForAux(view), economyEnabled, econ, Refusal: null);
                budget = ActiveStandBudget(view.Areas.Count, active.Session.GenesisStemCount);
            }
            catch (P1Refusal refusal)
            {
                var view = active.Adapter.Read();
                var econ = TryBuildEconomySnapshot(active.Session, view);
                status = StatusCodes.Status409Conflict;
                payload = new MultiEconomyQuotePayload(
                    Quote: null, FenceOf(view), BoundSessionViewForAux(view), economyEnabled, econ, new Notice(refusal.Code, refusal.Message));
                budget = ActiveStandBudget(view.Areas.Count, active.Session.GenesisStemCount);
            }
        }
        await WriteBoundedJsonAsync(context, status, payload, budget);
    }

    // Per-order executor assignment (POST api/multi/economy/executor). Refuses Fell->Landowner (EconomyFellContractorOnly)
    // and edits while approved (WorkPlanAlreadyApproved); state unchanged on refusal.
    private Task SetMultiOrderExecutorAsync(HttpContext context) =>
        MultiCommandAsync<MultiEconomyExecutorRequest>(context, MultiEconomyExecutorMembers, (active, request) =>
            RefusableMulti(active, () =>
            {
                if (!Enum.TryParse<EconomyOrderKind>(request.Kind, ignoreCase: false, out var kind) || !Enum.IsDefined(kind))
                    throw new P1Refusal("EconomyOrderUnknown", "unknown work order kind: " + request.Kind);
                if (!Enum.TryParse<WorkExecutor>(request.Executor, ignoreCase: false, out var executor) || !Enum.IsDefined(executor))
                    throw new P1Refusal("EconomyExecutorUnknown", "unknown work executor: " + request.Executor);
                var key = new EconomyOrderKey(kind, request.AreaId!, request.TargetKey!);
                return active.Session.SetOrderExecutor(request.Fence!, key, executor);
            }));

    // Per-Fell-order felling disposition (POST api/multi/economy/disposition). Refuses unknown enum, edits while approved
    // (WorkPlanAlreadyApproved), or LeaveAsDeadwood without Stage 4 (EconomyDeadwoodRequiresStage4); state unchanged on refusal.
    private Task SetMultiFellingDispositionAsync(HttpContext context) =>
        MultiCommandAsync<MultiEconomyDispositionRequest>(context, MultiEconomyDispositionMembers, (active, request) =>
            RefusableMulti(active, () =>
            {
                if (!Enum.TryParse<FellingDisposition>(request.Disposition, ignoreCase: false, out var disposition) || !Enum.IsDefined(disposition))
                    throw new P1Refusal("EconomyDispositionUnknown", "unknown felling disposition: " + request.Disposition);
                return active.Session.SetFellingDisposition(request.Fence!, request.AreaId!, request.TreeId!, disposition);
            }));

    private sealed record MultiCommandPayload(object View, object Scenes,
        Notice? Refusal, MultiAreaExecutionResult? Execution, MultiAreaAnnualResult? Annual, object? Stage4,
        MultiAreaEcologyDto? Ecology, bool HistoryTruncated = false, int TotalRecordedYears = 0,
        bool ProjectionCompacted = false, bool Stage4Compacted = false, bool PayloadBudgetExceeded = false,
        bool EconomyEnabled = false,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] MultiEconomySnapshotDto? Economy = null,
        bool HasUnsavedProgress = false);

    private sealed record MultiEconomyViewPayload(
        string SessionId,
        int PlanStateRevision,
        MultiAreaSourceToken Source,
        bool EconomyEnabled,
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
        bool EconomyEnabled = false,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] MultiEconomySnapshotDto? Economy = null,
        bool HasUnsavedProgress = false);

    private sealed record MultiEconomyQuotePayload(
        MultiEconomyQuoteDto? Quote,
        MultiAreaFence Fence,
        object View,
        bool EconomyEnabled,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] MultiEconomySnapshotDto? Economy,
        Notice? Refusal);
}
