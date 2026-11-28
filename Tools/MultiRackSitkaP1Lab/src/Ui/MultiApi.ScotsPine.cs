using Microsoft.AspNetCore.Http;
using SitkaP1.Core;

namespace SitkaP1.Ui;

internal sealed partial class PlayerApi
{
    // POST api/multi/scots-pine/tree — strictly read-only site and Scots pine detail of one exact tree for the inspector:
    // moisture class and the species' separate establishment/growth modifiers, and for a Scots pine its Part 1 growth-domain
    // status and increment, pruning lifts and timber grade preview. Every number comes from Core; the page computes nothing.
    // 400 on malformed JSON or missing members; 409 with a typed refusal on a stale fence, unknown or wrong-area tree.
    private async Task ReadMultiScotsPineTreeAsync(HttpContext context)
    {
        var request = await ReadMultiRequestAsync<MultiTreeRequest>(context, MultiTreeMembers);
        if (request is null) return;
        ScotsPineTreeView? tree = null;
        Notice? refusal = null;
        try
        {
            lock (multiHost.SyncLock)
                tree = multiHost.Current.Session.ReadScotsPineTree(request.Fence!, request.AreaId ?? "", request.TreeId ?? "");
        }
        catch (P1Refusal r)
        {
            refusal = new Notice(r.Code, r.Message);
        }
        await WriteJsonAsync(context, refusal is null ? StatusCodes.Status200OK : StatusCodes.Status409Conflict,
            new MultiScotsPineTreePayload(tree, refusal));
    }

    private sealed record MultiScotsPineTreePayload(ScotsPineTreeView? Tree, Notice? Refusal);
}
