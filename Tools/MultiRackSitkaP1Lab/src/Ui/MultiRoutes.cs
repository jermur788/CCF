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
    private static readonly (string Route, byte[] Asset)[] MultiPageModules =
    {
        ("multi/api.js", MultiEmbeddedAsset("SitkaP1.Ui.multi.api.js")),
        ("multi/commands.js", MultiEmbeddedAsset("SitkaP1.Ui.multi.commands.js")),
        ("multi/dialogs/execute-confirm.js", MultiEmbeddedAsset("SitkaP1.Ui.multi.dialogs.execute-confirm.js")),
        ("multi/dialogs/saves.js", MultiEmbeddedAsset("SitkaP1.Ui.multi.dialogs.saves.js")),
        ("multi/dom.js", MultiEmbeddedAsset("SitkaP1.Ui.multi.dom.js")),
        ("multi/main.js", MultiEmbeddedAsset("SitkaP1.Ui.multi.main.js")),
        ("multi/map/contract.js", MultiEmbeddedAsset("SitkaP1.Ui.multi.map.contract.js")),
        ("multi/map/renderer-svg.js", MultiEmbeddedAsset("SitkaP1.Ui.multi.map.renderer-svg.js")),
        ("multi/modes/interactive.js", MultiEmbeddedAsset("SitkaP1.Ui.multi.modes.interactive.js")),
        ("multi/panels/fell-draft.js", MultiEmbeddedAsset("SitkaP1.Ui.multi.panels.fell-draft.js")),
        ("multi/panels/inspector.js", MultiEmbeddedAsset("SitkaP1.Ui.multi.panels.inspector.js")),
        ("multi/panels/time-history.js", MultiEmbeddedAsset("SitkaP1.Ui.multi.panels.time-history.js")),
        ("multi/panels/work-plan.js", MultiEmbeddedAsset("SitkaP1.Ui.multi.panels.work-plan.js")),
        ("multi/store.js", MultiEmbeddedAsset("SitkaP1.Ui.multi.store.js")),
    };

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
        foreach (var (route, asset) in MultiPageModules)
            routes.Add(route, new("GET", context => WriteAssetAsync(context, asset, "text/javascript; charset=utf-8")));
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
}
