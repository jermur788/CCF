using Microsoft.AspNetCore.Http;
using SitkaP1.Core;

namespace SitkaP1.Ui;

internal sealed partial class PlayerApi
{
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

    private sealed record MultiCommandPayload(object View, object Scenes,
        Notice? Refusal, MultiAreaExecutionResult? Execution, MultiAreaAnnualResult? Annual, object? Stage4,
        MultiAreaEcologyDto? Ecology, bool HistoryTruncated = false, int TotalRecordedYears = 0,
        bool ProjectionCompacted = false, bool Stage4Compacted = false, bool PayloadBudgetExceeded = false);
}
