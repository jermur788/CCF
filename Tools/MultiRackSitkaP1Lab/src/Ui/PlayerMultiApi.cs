using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using SitkaP1.Core;

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
    private static readonly string[] MultiHitMembers = MultiFenceMembers.Concat(new[] { "areaId", "u", "v" }).ToArray();
    private static readonly string[] MultiCellMembers = MultiFenceMembers.Concat(new[] { "areaId", "column", "row" }).ToArray();
    private static readonly string[] MultiPointMembers = MultiFenceMembers.Concat(new[] { "areaId", "xM", "zM" }).ToArray();
    private static readonly string[] MultiPlantMembers = MultiFenceMembers.Concat(new[] { "areaId", "speciesId", "xM", "zM" }).ToArray();
    private static readonly string[] MultiPlantingRemoveMembers = MultiFenceMembers.Append("orderId").ToArray();

    private static readonly byte[] MultiPageHtml = Encoding.UTF8.GetBytes(MultiAreaPage.Html);
    private static readonly byte[] MultiPageCss = Encoding.UTF8.GetBytes(MultiAreaPage.Css);
    private static readonly byte[] MultiPageJs = Encoding.UTF8.GetBytes(MultiAreaPage.Js);

    // Fixed page text from Core only: the single-area status words and Crop labels, and the MA-1 / Pass 1 labels.
    private static readonly IReadOnlyDictionary<string, string> MultiLabels = new[] { typeof(PlayerLabels), typeof(CropLabels), typeof(MultiAreaLabels) }
        .SelectMany(type => type.GetFields(BindingFlags.Public | BindingFlags.Static))
        .Where(field => field.IsLiteral && field.FieldType == typeof(string))
        .ToDictionary(field => field.Name, field => (string)field.GetRawConstantValue()!, StringComparer.Ordinal);

    private readonly MultiAreaWorkPlanAdapter multiArea;
    private readonly MultiAreaContinuation multiSession;

    private void AddMultiAreaRoutes()
    {
        routes.Add("multi", new("GET", context => WriteAssetAsync(context, MultiPageHtml, "text/html; charset=utf-8")));
        routes.Add("multi.css", new("GET", context => WriteAssetAsync(context, MultiPageCss, "text/css; charset=utf-8")));
        routes.Add("multi.js", new("GET", context => WriteAssetAsync(context, MultiPageJs, "text/javascript; charset=utf-8")));
        routes.Add("api/multi/view", new("GET", ReadMultiViewAsync));
        routes.Add("api/multi/manifest", new("GET", WriteMultiManifestAsync));
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

    // One atomic composite read under one Core lock: view, stage4 and ecology share one exact root.
    private Task ReadMultiViewAsync(HttpContext context)
    {
        var snapshot = multiArea.ReadCompositeUiSnapshot();
        return WriteJsonAsync(context, StatusCodes.Status200OK,
            new MultiViewPayload(snapshot.View, ScenesOf(snapshot.View), MultiLabels, snapshot.Readout, snapshot.Ecology));
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
        var result = multiArea.ReadCellDetail(request.Fence!, request.AreaId!, column, row);
        var snap = result.Snapshot;
        await WriteJsonAsync(context, StatusOf(result.RefusalCode), new MultiCellPayload(snap.View, ScenesOf(snap.View),
            result.Cell, NoticeOf(result.RefusalCode, result.RefusalMessage), snap.Readout, snap.Ecology));
    }

    private async Task ReadMultiCompetitionAsync(HttpContext context)
    {
        var request = await ReadMultiRequestAsync<MultiTreeRequest>(context, MultiTreeMembers);
        if (request is null) return;
        var result = multiArea.Competition(request.Fence!, request.AreaId!, request.TreeId!);
        var composite = multiArea.CompositeFor(result.View);
        await WriteJsonAsync(context, StatusOf(result.RefusalCode), new MultiCompetitionPayload(result.View, ScenesOf(result.View),
            result.AreaId, result.Competition, NoticeOf(result.RefusalCode, result.RefusalMessage),
            composite?.Stage4, composite?.Ecology));
    }

    private async Task MultiHitTestAsync(HttpContext context)
    {
        var request = await ReadMultiRequestAsync<MultiHitRequest>(context, MultiHitMembers);
        if (request is null) return;
        var result = multiArea.HitTest(request.Fence!, request.AreaId!, request.U!.Value, request.V!.Value);
        var composite = multiArea.CompositeFor(result.View);
        await WriteJsonAsync(context, StatusOf(result.RefusalCode), new MultiHitPayload(result.View, ScenesOf(result.View),
            result.AreaId, result.Candidates, NoticeOf(result.RefusalCode, result.RefusalMessage),
            composite?.Stage4, composite?.Ecology));
    }

    private async Task MultiCommandAsync<TRequest>(HttpContext context, IReadOnlyList<string> required,
        Func<TRequest, MultiAreaCommandResult> command) where TRequest : class
    {
        var request = await ReadMultiRequestAsync<TRequest>(context, required);
        if (request is null) return;
        var result = command(request);
        var composite = multiArea.CompositeFor(result.View);
        await WriteJsonAsync(context, StatusOf(result.RefusalCode), new MultiCommandPayload(result.View, ScenesOf(result.View),
            NoticeOf(result.RefusalCode, result.RefusalMessage), result.Execution, result.Annual,
            composite?.Stage4, composite?.Ecology));
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
                     ("orderId", MaxIdLength), ("planId", MaxPlanKeyLength), ("executionKey", MaxPlanKeyLength) })
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

    // Stage4 ("stage4") and Ecology ("ecology") are additive; all existing members keep their exact names and shapes.
    private sealed record MultiViewPayload(MultiAreaSessionView View, IReadOnlyDictionary<string, IReadOnlyList<PlayerMapItem>> Scenes,
        IReadOnlyDictionary<string, string> Labels, MultiAreaUiReadout? Stage4, MultiAreaEcologyDto? Ecology);

    private sealed record MultiCommandPayload(MultiAreaSessionView View, IReadOnlyDictionary<string, IReadOnlyList<PlayerMapItem>> Scenes,
        Notice? Refusal, MultiAreaExecutionResult? Execution, MultiAreaAnnualResult? Annual, MultiAreaUiReadout? Stage4,
        MultiAreaEcologyDto? Ecology);

    private sealed record MultiCompetitionPayload(MultiAreaSessionView View,
        IReadOnlyDictionary<string, IReadOnlyList<PlayerMapItem>> Scenes, string AreaId, PlayerCompetitionReport? Competition,
        Notice? Refusal, MultiAreaUiReadout? Stage4, MultiAreaEcologyDto? Ecology);

    private sealed record MultiHitPayload(MultiAreaSessionView View, IReadOnlyDictionary<string, IReadOnlyList<PlayerMapItem>> Scenes,
        string AreaId, IReadOnlyList<PlayerMapItem> Candidates, Notice? Refusal, MultiAreaUiReadout? Stage4,
        MultiAreaEcologyDto? Ecology);

    private sealed record MultiCellPayload(MultiAreaSessionView View, IReadOnlyDictionary<string, IReadOnlyList<PlayerMapItem>> Scenes,
        MultiAreaCellDetail? Cell, Notice? Refusal, MultiAreaUiReadout? Stage4, MultiAreaEcologyDto? Ecology);
}

/// <summary>
/// The two-area page: pure inline SVG visual system, Stage 4 Owner UX polish preserved, read-only ecology overlays
/// and lazy cell inspector, served from memory under strict CSP (no WebP sprites, no inline JS, no external URLs).
/// </summary>
internal static class MultiAreaPage
{
    internal const string Html = """
<!doctype html>
<html lang="en">
<head>
<meta charset="utf-8">
<meta name="viewport" content="width=device-width, initial-scale=1">
<meta name="color-scheme" content="dark">
<meta name="referrer" content="no-referrer">
<title>Two-area Stand Map</title>
<link rel="stylesheet" href="multi.css">
<script src="multi.js" defer></script>
</head>
<body>
<!-- Two-area Stand Map (Pass 1 Map Integration, spec multi-map-integration-pass1-v1). Pure inline SVG visual system,
     zero WebP sprites, zero Pass 2 write controls. Minimum supported desktop viewport: 1280 x 720 CSS px (ranks 1-5
     and Add remain above the fold without scrolling, including when competitors are blocked). -->
<a class="skip-link" id="skip-trees" href="#trees-heading">Skip to the tree table</a>

<header class="topbar" aria-labelledby="page-title">
  <div class="brand">
    <svg class="brand-mark" viewBox="0 0 30 36" aria-hidden="true" focusable="false">
      <path d="M15 1 6 13h5L3 25h10v9h4v-9h10l-8-12h5Z"></path>
    </svg>
    <div class="brand-text">
      <h1 id="page-title">Two-area Stand Map</h1>
      <p id="experiment-badge" class="subtitle">Loading experiment…</p>
    </div>
  </div>
  <div class="top-actions">
    <p class="year-chip"><span class="year-word">Year</span> <strong id="year-value">–</strong></p>
    <button type="button" id="advance" class="advance">
      <span class="advance-icon" aria-hidden="true">▷</span>
      <span class="advance-text"><span id="advance-label" class="btn-title">Advance one ecological year</span>
        <span class="btn-sub">Both areas · does not execute work</span></span>
    </button>
    <button type="button" id="reread" class="reread" aria-label="Re-read current state" title="Re-read current state"><span aria-hidden="true">↻</span> Re-read</button>
  </div>
</header>

<section class="summary" aria-label="Areas and Work Plan at a glance">
  <div id="area-cards" class="area-cards"></div>
  <div class="card plan-card">
    <div class="plan-card-text">
      <p class="eyebrow">Shared Work Plan · both areas</p>
      <p class="plan-card-main"><strong id="plan-summary">–</strong> <span id="plan-summary-sub" class="plan-card-sub"></span></p>
    </div>
    <button type="button" id="view-plan" class="link-button">View plan <span aria-hidden="true">→</span></button>
  </div>
</section>

<!-- Compact notice line (UX-04); full disclosure and caveats stay verbatim behind Details. -->
<div class="notice" role="note" aria-label="About this experiment">
  <span class="notice-icon" aria-hidden="true">ⓘ</span>
  <p id="notice-line" class="notice-line"></p>
  <details id="notice-details" class="notice-details">
    <summary>Details</summary>
    <div class="notice-body">
      <p class="notice-text"><strong id="disclosure" class="notice-lead"></strong></p>
      <p id="flags" class="flags"></p>
      <p id="session-note" class="session-note"></p>
      <p><button type="button" id="notice-provenance" class="link-button small-link">All experiment and model provenance</button></p>
    </div>
  </details>
</div>

<div class="messages">
  <div id="alert" class="alert" role="alert" tabindex="-1"></div>
</div>

<main id="workspace" class="workspace" data-legend="shown">
  <div class="workspace-head">
    <h2 id="map-heading" tabindex="-1">Stand map</h2>
    <p id="map-subtitle" class="map-subtitle"></p>
    <div class="mode-switch" role="group" aria-label="Map interaction mode">
      <button type="button" id="mode-tree" class="mode-btn" data-map-mode="tree" aria-pressed="true">Select Tree</button>
      <button type="button" id="mode-cell" class="mode-btn" data-map-mode="cell" aria-pressed="false">Inspect Cell</button>
    </div>
    <label class="sr-only" for="overlay-select">Ground overlay</label>
    <select id="overlay-select" class="overlay-select" aria-label="Ground overlay">
      <option value="off">Overlay: Off</option>
      <option value="light">Overlay: Light Guide</option>
      <option value="undergrowth">Overlay: Undergrowth</option>
      <option value="seedlings">Overlay: Natural Seedlings</option>
    </select>
    <p id="announce" class="announce" role="status" aria-live="polite" aria-atomic="true"></p>
    <nav class="view-tabs" aria-label="Side panel views">
      <button type="button" class="view-tab" data-pane-button="tree" aria-controls="pane-tree" aria-pressed="true">Tree</button>
      <button type="button" class="view-tab" data-pane-button="plan" aria-controls="pane-plan" aria-pressed="false">Work Plan</button>
      <button type="button" class="view-tab" data-pane-button="trees" aria-controls="pane-trees" aria-pressed="false">Trees</button>
      <button type="button" class="view-tab" data-pane-button="history" aria-controls="pane-history" aria-pressed="false">Results</button>
      <button type="button" class="view-tab" data-pane-button="details" aria-controls="pane-details" aria-pressed="false">Details</button>
    </nav>
  </div>

  <section class="map-panel" aria-labelledby="map-heading">
    <div id="map-pane" class="map-pane">
      <svg id="map" viewBox="0 0 40 40" role="img" aria-labelledby="map-title map-desc">
        <title id="map-title">Map of the selected management area, north up</title>
        <desc id="map-desc">Loading the current view.</desc>
        <defs>
          <clipPath id="area-clip"><polygon id="area-clip-shape" points="0,0"></polygon></clipPath>
          <radialGradient id="canopy-shadow">
            <stop offset="25%" stop-color="#020b16" stop-opacity="0.62"></stop>
            <stop offset="72%" stop-color="#020b16" stop-opacity="0.34"></stop>
            <stop offset="100%" stop-color="#020b16" stop-opacity="0"></stop>
          </radialGradient>

          <!-- Pure inline SVG visual system (all 21 locked IDs; aligned with Standard_SVG_Asset_Sheet_v2.png and species boards) -->

          <!-- #ccf-sitka: Radial conifer crown, tiered needle boughs, blue-green palette -->
          <symbol id="ccf-sitka" viewBox="-100 -100 200 200">
            <path d="M0-96 14-36 68-68 36-14 96 0 36 14 68 68 14 36 0 96-14 36-68 68-36 14-96 0-36-14-68-68-14-36Z" fill="#0e2c22"></path>
            <path d="M0-94 12-40 48-78 28-24 88-38 38-6 94 14 36 18 76 62 22 30 34 88 2 38-22 92-18 34-70 70-32 16-94 22-38-4-86-42-26-22-56-76-12-34Z" fill="#184333"></path>
            <g stroke="#296449" stroke-width="6.5" stroke-linecap="round">
              <line x1="0" y1="0" x2="0" y2="-90"></line><line x1="0" y1="0" x2="64" y2="-64"></line>
              <line x1="0" y1="0" x2="90" y2="0"></line><line x1="0" y1="0" x2="64" y2="64"></line>
              <line x1="0" y1="0" x2="0" y2="90"></line><line x1="0" y1="0" x2="-64" y2="64"></line>
              <line x1="0" y1="0" x2="-90" y2="0"></line><line x1="0" y1="0" x2="-64" y2="-64"></line>
            </g>
            <g stroke="#3e825a" stroke-width="4.2" stroke-linecap="round">
              <path d="M0-72 -14-56 M0-72 14-56 M0-48 -18-30 M0-48 18-30"></path>
              <path d="M50-50 28-52 M50-50 52-28 M72 0 56-14 M72 0 56 14"></path>
              <path d="M50 50 52 28 M50 50 28 52 M0 72 14 56 M0 72 -14 56"></path>
              <path d="M-50 50 -28 52 M-50 50 -52 28 M-72 0 -56 14 M-72 0 -56-14"></path>
              <path d="M-50-50 -52-28 M-50-50 -28-52"></path>
            </g>
            <path d="M-4-74 6-26 42-46 22-8 66 2 20 12 44 46 8 22-2 66-12 20-48 44-22 8-68-4-22-14-48-48-10-22Z" fill="#2e6f4a"></path>
            <path d="M-6-62 4-18 32-38 14-6 46-4 12 8 24 32 2 14-14 44-10 12-40 24-16 2-56-12-16-14-42-46-12-18Z" fill="#5d9d57"></path>
            <path d="M-8-42 0-12 20-26 8-4 28-6 6 4 10 18-2 8-18 20-10 2-36-10-12-12-30-32-8-14Z" fill="#92c465"></path>
            <circle cx="-3" cy="-4" r="9" fill="#b7d979"></circle>
            <circle cx="-2" cy="-3" r="3.5" fill="#dceca2"></circle>
          </symbol>

          <!-- #ccf-oak: Lobed irregular broadleaf crown, warm green palette, visible gnarled branch structure -->
          <symbol id="ccf-oak" viewBox="-100 -100 200 200">
            <g fill="#1c3b19">
              <circle cx="-36" cy="-44" r="44"></circle><circle cx="28" cy="-48" r="42"></circle>
              <circle cx="54" cy="4" r="40"></circle><circle cx="24" cy="52" r="42"></circle>
              <circle cx="-34" cy="48" r="42"></circle><circle cx="-56" cy="2" r="39"></circle>
              <circle cx="0" cy="0" r="46"></circle>
            </g>
            <g fill="#2f5a22">
              <circle cx="-46" cy="-34" r="30"></circle><circle cx="-10" cy="-62" r="28"></circle>
              <circle cx="44" cy="-36" r="30"></circle><circle cx="62" cy="14" r="28"></circle>
              <circle cx="32" cy="58" r="29"></circle><circle cx="-20" cy="60" r="28"></circle>
              <circle cx="-62" cy="14" r="27"></circle><circle cx="6" cy="4" r="34"></circle>
            </g>
            <!-- Visible gnarled oak branch skeleton -->
            <g fill="none" stroke="#5d4226" stroke-width="6.5" stroke-linecap="round" stroke-linejoin="round">
              <path d="M0 0 Q-18 -14 -44 -34 M0 0 Q14 -20 38 -36 M0 0 Q26 6 52 10 M0 0 Q10 24 26 48 M0 0 Q-20 18 -42 36 M0 0 Q-26 2 -50 4"></path>
            </g>
            <g fill="none" stroke="#805e38" stroke-width="2.6" stroke-linecap="round">
              <path d="M-2 -2 Q-16 -14 -38 -28 M-2 -2 Q12 -18 32 -30 M0 0 Q22 4 44 8 M0 0 Q-16 14 -34 28"></path>
            </g>
            <g fill="#4f7e2d">
              <circle cx="-54" cy="-24" r="19"></circle><circle cx="-28" cy="-58" r="21"></circle>
              <circle cx="14" cy="-58" r="19"></circle><circle cx="52" cy="-22" r="20"></circle>
              <circle cx="56" cy="24" r="19"></circle><circle cx="16" cy="56" r="19"></circle>
              <circle cx="-36" cy="50" r="19"></circle><circle cx="-58" cy="18" r="18"></circle>
              <circle cx="-14" cy="-14" r="22"></circle><circle cx="20" cy="14" r="20"></circle>
            </g>
            <g fill="#7ba33c">
              <circle cx="-48" cy="-34" r="14"></circle><circle cx="-20" cy="-52" r="15"></circle>
              <circle cx="-56" cy="0" r="13"></circle><circle cx="-18" cy="-18" r="16"></circle>
              <circle cx="18" cy="-38" r="14"></circle><circle cx="42" cy="-8" r="13"></circle>
              <circle cx="-24" cy="24" r="14"></circle><circle cx="10" cy="30" r="12"></circle>
            </g>
            <g fill="#a9c653">
              <circle cx="-42" cy="-40" r="9"></circle><circle cx="-18" cy="-56" r="9"></circle>
              <circle cx="-24" cy="-20" r="10"></circle><circle cx="8" cy="-42" r="8"></circle>
              <circle cx="-52" cy="-8" r="8"></circle><circle cx="-4" cy="-6" r="8"></circle>
            </g>
          </symbol>

          <!-- #ccf-beech: Rounded/oval dense broadleaf crown, fine overlapping leaf mass, copper-green highlights -->
          <symbol id="ccf-beech" viewBox="-100 -100 200 200">
            <ellipse cx="0" cy="0" rx="94" ry="91" fill="#18381d"></ellipse>
            <g fill="#295424">
              <ellipse cx="-38" cy="-42" rx="42" ry="36" transform="rotate(-20 -38 -42)"></ellipse>
              <ellipse cx="38" cy="-38" rx="40" ry="34" transform="rotate(24 38 -38)"></ellipse>
              <ellipse cx="48" cy="22" rx="38" ry="36" transform="rotate(-12 48 22)"></ellipse>
              <ellipse cx="-2" cy="54" rx="44" ry="34"></ellipse>
              <ellipse cx="-48" cy="24" rx="38" ry="36" transform="rotate(15 -48 24)"></ellipse>
              <circle cx="0" cy="0" r="56"></circle>
            </g>
            <g fill="#45752e">
              <ellipse cx="-52" cy="-18" rx="24" ry="18" transform="rotate(-25 -52 -18)"></ellipse>
              <ellipse cx="-24" cy="-56" rx="25" ry="18" transform="rotate(-10 -24 -56)"></ellipse>
              <ellipse cx="26" cy="-52" rx="24" ry="18" transform="rotate(18 26 -52)"></ellipse>
              <ellipse cx="58" cy="-8" rx="22" ry="18" transform="rotate(30 58 -8)"></ellipse>
              <ellipse cx="46" cy="40" rx="24" ry="17" transform="rotate(-22 46 40)"></ellipse>
              <ellipse cx="-4" cy="60" rx="26" ry="17"></ellipse>
              <ellipse cx="-48" cy="38" rx="24" ry="17" transform="rotate(25 -48 38)"></ellipse>
              <ellipse cx="-14" cy="-12" rx="30" ry="24" transform="rotate(-15 -14 -12)"></ellipse>
              <ellipse cx="22" cy="14" rx="28" ry="22" transform="rotate(15 22 14)"></ellipse>
            </g>
            <g fill="#6d9439">
              <ellipse cx="-44" cy="-32" rx="17" ry="12" transform="rotate(-30 -44 -32)"></ellipse>
              <ellipse cx="-12" cy="-54" rx="18" ry="12" transform="rotate(-5 -12 -54)"></ellipse>
              <ellipse cx="28" cy="-40" rx="16" ry="11" transform="rotate(20 28 -40)"></ellipse>
              <ellipse cx="-52" cy="10" rx="16" ry="11" transform="rotate(-15 -52 10)"></ellipse>
              <ellipse cx="-18" cy="-16" rx="20" ry="14" transform="rotate(-20 -18 -16)"></ellipse>
              <ellipse cx="18" cy="-6" rx="18" ry="13" transform="rotate(10 18 -6)"></ellipse>
              <ellipse cx="38" cy="24" rx="16" ry="11" transform="rotate(-10 38 24)"></ellipse>
              <ellipse cx="-16" cy="32" rx="17" ry="12" transform="rotate(12 -16 32)"></ellipse>
            </g>
            <!-- Copper-green sunlit fine foliage clusters -->
            <g fill="#9fb047">
              <ellipse cx="-38" cy="-38" rx="12" ry="8" transform="rotate(-35 -38 -38)"></ellipse>
              <ellipse cx="-14" cy="-46" rx="13" ry="8" transform="rotate(-12 -14 -46)"></ellipse>
              <ellipse cx="-24" cy="-18" rx="14" ry="9" transform="rotate(-22 -24 -18)"></ellipse>
              <ellipse cx="6" cy="-28" rx="11" ry="7" transform="rotate(15 6 -28)"></ellipse>
              <ellipse cx="-44" cy="-4" rx="11" ry="7" transform="rotate(-20 -44 -4)"></ellipse>
            </g>
            <g fill="#b99c4e" opacity="0.72">
              <ellipse cx="-32" cy="-26" rx="8" ry="5" transform="rotate(-28 -32 -26)"></ellipse>
              <ellipse cx="-6" cy="-38" rx="7" ry="4.5" transform="rotate(-8 -6 -38)"></ellipse>
              <ellipse cx="14" cy="-16" rx="7" ry="4.5" transform="rotate(18 14 -16)"></ellipse>
              <ellipse cx="-20" cy="4" rx="7" ry="4.5" transform="rotate(-12 -20 4)"></ellipse>
            </g>
          </symbol>

          <!-- #ccf-standing-dead: Bare skeletal deadwood crown, weathered grey-tan branches, zero foliage -->
          <symbol id="ccf-standing-dead" viewBox="-100 -100 200 200">
            <g fill="none" stroke="#2c2620" stroke-width="10" stroke-linecap="round" stroke-linejoin="round" opacity="0.65">
              <path d="M0 0 L-12-78 M0 0 L52-66 M0 0 L84-6 M0 0 L54 62 M0 0 L-8 82 M0 0 L-62 56 M0 0 L-82-12 M0 0 L-58-62"></path>
            </g>
            <g fill="none" stroke="#8f8472" stroke-width="6.5" stroke-linecap="round" stroke-linejoin="round">
              <path d="M0 0 L-8-44 L-26-76 M-8-44 L10-80"></path>
              <path d="M0 0 L34-34 L66-58 M34-34 L48-74"></path>
              <path d="M0 0 L46-4 L84-16 M46-4 L78 18"></path>
              <path d="M0 0 L32 36 L62 64 M32 36 L22 76"></path>
              <path d="M0 0 L-10 46 L-30 80 M-10 46 L8 78"></path>
              <path d="M0 0 L-44 28 L-76 48 M-44 28 L-68 62"></path>
              <path d="M0 0 L-48-10 L-84-26 M-48-10 L-80 12"></path>
              <path d="M0 0 L-36-38 L-66-62 M-36-38 L-44-74"></path>
            </g>
            <g fill="none" stroke="#c8bfb0" stroke-width="2.6" stroke-linecap="round">
              <path d="M-2-2 L-8-42 L-24-72 M0 0 L32-32 L62-54 M0 0 L44-4 L78-14 M0 0 L-44-10 L-78-24 M0 0 L-34-36 L-62-58"></path>
            </g>
            <circle cx="0" cy="0" r="10" fill="#a89e8c" stroke="#4f463a" stroke-width="2.5"></circle>
          </symbol>

          <!-- #ccf-log: Fallen deadwood log (direction schematic, not simulated) -->
          <symbol id="ccf-log" viewBox="-16 -6 32 12">
            <path d="M-13-4 H10 A4 4 0 0 1 10 4 H-13 Z" fill="#06111d" stroke="#06111d" stroke-width="2.2"></path>
            <path d="M-13-3.6 H10 A3.6 3.6 0 0 1 10 3.6 H-13 Z" fill="#8b6b45" stroke="#2c1e10" stroke-width="0.9"></path>
            <line x1="-8" y1="-1.5" x2="8" y2="-1.5" stroke="#61472a" stroke-width="0.7"></line>
            <line x1="-6" y1="1.3" x2="10" y2="1.3" stroke="#61472a" stroke-width="0.7"></line>
            <ellipse cx="-13" cy="0" rx="2.7" ry="3.6" fill="#d9bf8c" stroke="#2c1e10" stroke-width="0.9"></ellipse>
            <ellipse cx="-13" cy="0" rx="1.2" ry="1.8" fill="none" stroke="#6e5130" stroke-width="0.7"></ellipse>
          </symbol>

          <!-- #ccf-crop-ring: Retained Crop Tree — double gold ring + high-contrast C badge (never sawn lumber) -->
          <symbol id="ccf-crop-ring" viewBox="-128 -128 256 256">
            <circle cx="0" cy="0" r="106" fill="none" stroke="#06111d" stroke-width="14" opacity="0.88"></circle>
            <circle cx="0" cy="0" r="110" fill="none" stroke="#f5d57a" stroke-width="4.5"></circle>
            <circle cx="0" cy="0" r="101" fill="none" stroke="#d4aa48" stroke-width="3.2"></circle>
            <circle cx="0" cy="104" r="21" fill="#10202f" stroke="#f5d57a" stroke-width="4.5"></circle>
            <text x="0" y="105" fill="#f5d57a" font-size="25" font-weight="800" text-anchor="middle" dominant-baseline="central">C</text>
          </symbol>

          <!-- Sapling markers for exact planted juveniles (PJ) -->
          <symbol id="ccf-sapling-sitka" viewBox="-24 -24 48 48">
            <ellipse cx="0" cy="15" rx="14" ry="5" fill="#4f3822" stroke="#1e150c" stroke-width="1.2"></ellipse>
            <path d="M0-17 11-2 5-2 14 11 H-14 L-5-2 H-11 Z" fill="#236647" stroke="#0c261a" stroke-width="1.4" stroke-linejoin="round"></path>
            <path d="M0-14 6-3 H2 L8 7 H-8 L-2-3 H-6 Z" fill="#67b479"></path>
            <rect x="-2" y="10" width="4" height="5" fill="#704d2c"></rect>
          </symbol>

          <symbol id="ccf-sapling-oak" viewBox="-24 -24 48 48">
            <ellipse cx="0" cy="15" rx="14" ry="5" fill="#4f3822" stroke="#1e150c" stroke-width="1.2"></ellipse>
            <rect x="-2" y="4" width="4" height="11" fill="#6e4b29"></rect>
            <path d="M0-16 C6-16 10-10 8-6 C15-7 16 1 10 4 C11 9 4 11 0 7 C-4 11 -11 9 -10 4 C-16 1 -15-7 -8-6 C-10-10 -6-16 0-16 Z" fill="#598c32" stroke="#1c3512" stroke-width="1.4"></path>
            <circle cx="-3" cy="-6" r="4.5" fill="#a2c74f"></circle>
          </symbol>

          <symbol id="ccf-sapling-beech" viewBox="-24 -24 48 48">
            <ellipse cx="0" cy="15" rx="14" ry="5" fill="#4f3822" stroke="#1e150c" stroke-width="1.2"></ellipse>
            <rect x="-2" y="5" width="4" height="10" fill="#6e4b29"></rect>
            <ellipse cx="-6" cy="-2" rx="8" ry="6" transform="rotate(-25 -6 -2)" fill="#5f8b34" stroke="#1f3817" stroke-width="1.2"></ellipse>
            <ellipse cx="6" cy="-2" rx="8" ry="6" transform="rotate(25 6 -2)" fill="#759b3c" stroke="#1f3817" stroke-width="1.2"></ellipse>
            <ellipse cx="0" cy="-9" rx="7" ry="6" fill="#a8b84c" stroke="#1f3817" stroke-width="1.2"></ellipse>
          </symbol>

          <!-- #ccf-shelter-active: Solid cyan-blue mesh tube + stakes (< 8 yr) -->
          <symbol id="ccf-shelter-active" viewBox="-28 -28 56 56">
            <ellipse cx="0" cy="19" rx="17" ry="5" fill="#2d3a24" opacity="0.85"></ellipse>
            <path d="M0-22 C-5-16 -7-10 0-8 C7-10 5-16 0-22 Z" fill="#82c44a"></path>
            <rect x="-17" y="-12" width="4" height="31" rx="1.2" fill="#b88b53" stroke="#3c2a15" stroke-width="1"></rect>
            <rect x="13" y="-12" width="4" height="31" rx="1.2" fill="#b88b53" stroke="#3c2a15" stroke-width="1"></rect>
            <rect x="-13" y="-9" width="26" height="26" rx="3" fill="#2779a6" fill-opacity="0.45" stroke="#5ec7f7" stroke-width="2.2"></rect>
            <g stroke="#8be0ff" stroke-width="1" opacity="0.85">
              <line x1="-6" y1="-9" x2="-6" y2="17"></line><line x1="0" y1="-9" x2="0" y2="17"></line><line x1="6" y1="-9" x2="6" y2="17"></line>
              <line x1="-13" y1="-2" x2="13" y2="-2"></line><line x1="-13" y1="5" x2="13" y2="5"></line><line x1="-13" y1="11" x2="13" y2="11"></line>
            </g>
          </symbol>

          <!-- #ccf-shelter-expired: Broken dashed grey tube + X (8+ yr; MAP/LEGEND STATE ONLY, never a Work Plan button) -->
          <symbol id="ccf-shelter-expired" viewBox="-28 -28 56 56">
            <ellipse cx="0" cy="19" rx="17" ry="5" fill="#2a3035" opacity="0.85"></ellipse>
            <rect x="-17" y="-10" width="4" height="29" rx="1" fill="#7c776e" stroke="#2c2a27" stroke-width="1" transform="rotate(-6 -15 6)"></rect>
            <rect x="13" y="-10" width="4" height="29" rx="1" fill="#7c776e" stroke="#2c2a27" stroke-width="1" transform="rotate(7 15 6)"></rect>
            <rect x="-13" y="-9" width="26" height="26" rx="3" fill="#4b5561" fill-opacity="0.5" stroke="#b6c2cf" stroke-width="2.2" stroke-dasharray="4 3"></rect>
            <path d="M-7-2 L7 12 M7-2 L-7 12" stroke="#eef4fa" stroke-width="3.2" stroke-linecap="round"></path>
          </symbol>

          <!-- #ccf-spot-clear: Circular dashed ring (~1 m2, world-scale r=0.564m) over bare soil + twigs, NO seedling -->
          <symbol id="ccf-spot-clear" viewBox="-100 -100 200 200">
            <circle cx="0" cy="0" r="92" fill="#402f1e" fill-opacity="0.86"></circle>
            <g stroke="#9d805c" stroke-width="4.5" stroke-linecap="round" opacity="0.85">
              <line x1="-46" y1="-24" x2="-12" y2="-36"></line><line x1="16" y1="-44" x2="48" y2="-16"></line>
              <line x1="-54" y1="22" x2="-10" y2="34"></line><line x1="6" y1="16" x2="46" y2="42"></line>
              <line x1="-18" y1="-4" x2="26" y2="-10"></line>
            </g>
            <g fill="#73583c">
              <circle cx="-32" cy="8" r="5"></circle><circle cx="28" cy="-26" r="4"></circle>
              <circle cx="8" cy="48" r="5"></circle><circle cx="-20" cy="-48" r="4"></circle>
            </g>
            <circle cx="0" cy="0" r="93" fill="none" stroke="#edc774" stroke-width="9" stroke-dasharray="22 14"></circle>
          </symbol>

          <!-- #ccf-natural-seedlings: Cell-level natural regeneration presence glyph (never individual stem counts) -->
          <symbol id="ccf-natural-seedlings" viewBox="-24 -24 48 48">
            <ellipse cx="0" cy="14" rx="18" ry="6" fill="#463521" stroke="#1e150c" stroke-width="1"></ellipse>
            <g stroke="#3d702a" stroke-width="2" stroke-linecap="round">
              <line x1="-10" y1="12" x2="-10" y2="0"></line>
              <line x1="0" y1="13" x2="0" y2="-8"></line>
              <line x1="10" y1="12" x2="10" y2="2"></line>
            </g>
            <ellipse cx="-14" cy="-2" rx="4.5" ry="2.6" transform="rotate(-25 -14 -2)" fill="#78b648"></ellipse>
            <ellipse cx="-6" cy="-1" rx="4.5" ry="2.6" transform="rotate(25 -6 -1)" fill="#96cc5c"></ellipse>
            <ellipse cx="-5" cy="-10" rx="5.5" ry="3" transform="rotate(-25 -5 -10)" fill="#82be4d"></ellipse>
            <ellipse cx="5" cy="-10" rx="5.5" ry="3" transform="rotate(25 5 -10)" fill="#a8d868"></ellipse>
            <ellipse cx="6" cy="0" rx="4" ry="2.4" transform="rotate(-25 6 0)" fill="#75b346"></ellipse>
            <ellipse cx="14" cy="1" rx="4" ry="2.4" transform="rotate(25 14 1)" fill="#96cc5c"></ellipse>
          </symbol>

          <!-- #ccf-bramble-hatch: Diagonal thorny bramble pattern (distinct geometry + non-colour-only) -->
          <pattern id="ccf-bramble-hatch" patternUnits="userSpaceOnUse" width="1.25" height="1.25">
            <path d="M-0.25 1.0 Q0.35 0.55 1.0 -0.25 M0.0 1.5 Q0.65 0.95 1.5 0.0" fill="none" stroke="#c27292" stroke-width="0.11" stroke-linecap="round"></path>
            <path d="M0.32 0.62 L0.45 0.48 M0.88 0.92 L1.02 0.78" stroke="#e4a4bd" stroke-width="0.08" stroke-linecap="round"></path>
            <circle cx="0.62" cy="0.34" r="0.09" fill="#8b3b62"></circle>
            <circle cx="0.72" cy="0.41" r="0.08" fill="#5e2444"></circle>
          </pattern>

          <!-- #ccf-bracken-hatch: Counter-diagonal feathered bracken chevron pattern -->
          <pattern id="ccf-bracken-hatch" patternUnits="userSpaceOnUse" width="1.25" height="1.25">
            <path d="M0.15 0.15 L1.1 1.1 M0.38 0.62 L0.62 0.38 M0.62 0.86 L0.86 0.62" fill="none" stroke="#9bb852" stroke-width="0.11" stroke-linecap="round"></path>
            <path d="M0.22 0.42 L0.42 0.22 M0.82 1.02 L1.02 0.82" fill="none" stroke="#c3d872" stroke-width="0.08" stroke-linecap="round"></path>
          </pattern>

          <!-- Work Plan action icons (all 6 required IDs; Expired Shelter is NEVER a Work Plan action icon) -->
          <symbol id="ccf-action-fell" viewBox="0 0 32 32">
            <circle cx="16" cy="16" r="14.5" fill="#2b1716" stroke="#e0675c" stroke-width="2.2"></circle>
            <line x1="9" y1="23" x2="21" y2="11" stroke="#c98e4b" stroke-width="3" stroke-linecap="round"></line>
            <path d="M16 9 L24 13 L21 19 L14 14 Z" fill="#cfd8e3" stroke="#536273" stroke-width="1"></path>
          </symbol>

          <symbol id="ccf-action-prune" viewBox="0 0 32 32">
            <circle cx="16" cy="16" r="14.5" fill="#262012" stroke="#edc774" stroke-width="2.2"></circle>
            <path d="M15 16 L22 8 C24 10 23 14 18 16 Z M16 15 L14 8 C11 10 12 14 15 16 Z" fill="#d4dde7"></path>
            <path d="M15 17 L9 24 M17 17 L23 24" stroke="#d9534f" stroke-width="2.8" stroke-linecap="round"></path>
            <circle cx="16" cy="16" r="1.8" fill="#edc774"></circle>
          </symbol>

          <symbol id="ccf-action-clear" viewBox="0 0 32 32">
            <circle cx="16" cy="16" r="14.5" fill="#18291b" stroke="#75c759" stroke-width="2.2"></circle>
            <rect x="8.5" y="8.5" width="15" height="15" rx="1.5" fill="#46331e" stroke="#edc774" stroke-width="1.8" stroke-dasharray="3.5 2"></rect>
            <path d="M13 18 V14 M13 14 L11 12 M13 14 L15 12 M19 19 V15 M19 15 L17 13 M19 15 L21 13" stroke="#8ad45b" stroke-width="1.6" stroke-linecap="round"></path>
          </symbol>

          <symbol id="ccf-action-plant" viewBox="0 0 32 32">
            <circle cx="16" cy="16" r="14.5" fill="#18291b" stroke="#7cd458" stroke-width="2.2"></circle>
            <path d="M8 21 Q16 17 24 21 Q20 25 12 25 Z" fill="#c99568"></path>
            <line x1="16" y1="19" x2="16" y2="11" stroke="#6db344" stroke-width="2" stroke-linecap="round"></line>
            <ellipse cx="12.5" cy="12" rx="3.8" ry="2.2" transform="rotate(-25 12.5 12)" fill="#90d658"></ellipse>
            <ellipse cx="19.5" cy="12" rx="3.8" ry="2.2" transform="rotate(25 19.5 12)" fill="#90d658"></ellipse>
          </symbol>

          <symbol id="ccf-action-shelter" viewBox="0 0 32 32">
            <circle cx="16" cy="16" r="14.5" fill="#122738" stroke="#5ec7f7" stroke-width="2.2"></circle>
            <rect x="11" y="12" width="10" height="12" rx="1.5" fill="#2d8ec0" fill-opacity="0.5" stroke="#78d5ff" stroke-width="1.6"></rect>
            <line x1="9" y1="10" x2="9" y2="24" stroke="#c9985a" stroke-width="2" stroke-linecap="round"></line>
            <line x1="23" y1="10" x2="23" y2="24" stroke="#c9985a" stroke-width="2" stroke-linecap="round"></line>
            <circle cx="16" cy="9" r="2.5" fill="#8ad45b"></circle>
          </symbol>

          <symbol id="ccf-action-spot" viewBox="0 0 32 32">
            <circle cx="16" cy="16" r="14.5" fill="#241a30" stroke="#ba82e6" stroke-width="2.2"></circle>
            <circle cx="16" cy="16" r="8.5" fill="#46331e" stroke="#edc774" stroke-width="1.8" stroke-dasharray="3 2"></circle>
            <line x1="12" y1="14" x2="18" y2="12" stroke="#b39268" stroke-width="1.3" stroke-linecap="round"></line>
            <line x1="13" y1="19" x2="20" y2="17" stroke="#b39268" stroke-width="1.3" stroke-linecap="round"></line>
          </symbol>
        </defs>
        <g id="grid" class="overlay"></g>
        <rect id="ground" class="ground" x="0" y="0" width="40" height="40" clip-path="url(#area-clip)"></rect>
        <g id="grid-in" class="overlay" clip-path="url(#area-clip)"></g>
        <g id="overlay-cells" class="overlay" clip-path="url(#area-clip)" shape-rendering="crispEdges"></g>
        <g id="clear-cells" class="overlay" clip-path="url(#area-clip)"></g>
        <g id="spot-patches" class="overlay" clip-path="url(#area-clip)"></g>
        <polygon id="area-outline" class="area-outline" points="0,0"></polygon>
        <g id="crowns"></g>
        <g id="deadwood" class="overlay"></g>
        <g id="planted-layer" class="overlay"></g>
        <g id="ground-mark" class="overlay"></g>
        <g id="marks" class="overlay"></g>
        <g id="selection" class="overlay"></g>
        <g id="furniture" class="overlay"></g>
      </svg>
      <!-- Competitor ranks 1-5 as real buttons over the map, keyed by exact engine tree ID (UX-02). -->
      <div id="rank-layer" class="rank-layer" role="group" aria-label="Competitor ranks on the map: each button adds or removes that tree in the local Fell draft" hidden></div>

      <!-- Narrow dock (UX-05): Compass N/W/E/S, − Fit +, Area A | Area B. -->
      <div id="map-dock" class="map-dock" role="group" aria-labelledby="dock-title">
        <p class="dock-head"><span id="dock-title">Map navigation</span> <span id="zoom-state" class="zoom-state">Fit</span></p>
        <div class="compass" role="group" aria-label="Pan the map (north up)">
          <button type="button" id="pan-north" class="icon" aria-label="Pan north" title="Pan north"><span class="cmp cmp-n" aria-hidden="true"><span class="arr">↑</span><span>N</span></span></button>
          <button type="button" id="pan-west" class="icon" aria-label="Pan west" title="Pan west"><span class="cmp cmp-w" aria-hidden="true"><span class="arr">←</span><span>W</span></span></button>
          <button type="button" id="pan-east" class="icon" aria-label="Pan east" title="Pan east"><span class="cmp cmp-e" aria-hidden="true"><span>E</span><span class="arr">→</span></span></button>
          <button type="button" id="pan-south" class="icon" aria-label="Pan south" title="Pan south"><span class="cmp cmp-s" aria-hidden="true"><span>S</span><span class="arr">↓</span></span></button>
        </div>
        <div class="zoom-row" role="group" aria-label="Zoom">
          <button type="button" id="zoom-out" class="icon" aria-label="Zoom out" title="Zoom out"><span aria-hidden="true">−</span></button>
          <button type="button" id="zoom-fit" class="icon fit" aria-label="Fit whole area" title="Fit whole area">Fit</button>
          <button type="button" id="zoom-in" class="icon" aria-label="Zoom in" title="Zoom in"><span aria-hidden="true">+</span></button>
        </div>
        <div id="area-switch" class="dock-areas" role="group" aria-label="Choose the area shown on the map and in the tree table (two separate management areas)"></div>
      </div>

      <div class="map-orient">
        <button type="button" id="legend-show" class="legend-show" aria-controls="legend-panel" aria-expanded="false" hidden>Show legend</button>
        <p class="north" aria-hidden="true"><span>N</span><svg viewBox="0 0 12 16" focusable="false"><path d="M6 0 12 16 6 12 0 16Z"></path></svg></p>
      </div>
      <p class="scale" aria-hidden="true"><svg id="scale-bar" class="scale-bar" width="80" height="9" viewBox="0 0 80 9" focusable="false"><path id="scale-path" d="M 1 1 V 8 H 79 V 1"></path></svg> <span id="scale-text"></span></p>
      <p id="map-status" class="map-status" hidden></p>
    </div>

    <!-- Active ground overlay legend strip (shown only when an overlay is selected). -->
    <div id="overlay-legend" class="overlay-legend" hidden></div>

    <div class="map-foot">
      <p id="map-note" class="map-note"></p>
      <p class="map-note map-note-end">Modelled light · not calibrated PAR · Draft choices are not work orders</p>
    </div>
  </section>

  <aside id="legend-panel" class="legend-panel" aria-labelledby="legend-heading">
    <div class="legend-head">
      <h2 id="legend-heading">Map legend</h2>
      <button type="button" id="legend-hide" class="link-button small-link" aria-controls="legend-panel" aria-expanded="true">Hide legend</button>
    </div>
    <ul id="legend" class="legend"></ul>
    <p class="legend-foot">Letters, rings and shapes carry status. Fallen logs: direction schematic, not simulated. Expired shelter: map state only.</p>
  </aside>

  <div id="side" class="side" data-pane="tree">
    <div id="pane-tree" class="pane">
      <!-- Inspect Cell mode: explicitly SWAPS the inspector body (never stacks above the tree panel). -->
      <section id="cell-panel" class="panel cell-panel" aria-labelledby="cell-heading" hidden>
        <div class="ref-title">
          <p class="eyebrow">Inspect cell · read-only</p>
          <button type="button" id="cell-close" class="close-inspect" aria-keyshortcuts="Escape" title="Return to Select Tree mode (Esc)"><span aria-hidden="true">×</span> Exit cell mode</button>
        </div>
        <h2 id="cell-heading" tabindex="-1">Select a 2.5 m cell</h2>
        <div id="cell-summary" class="cell-summary"></div>
        <div id="cell-cohorts" class="cell-cohorts"></div>
        <details id="cell-trees" class="more ground-more" hidden><summary id="cell-trees-summary">Contributing trees</summary>
          <p id="cell-tree-ids" class="mono ids"></p></details>
        <details id="cell-tech" class="more ground-more" hidden><summary>Technical details</summary>
          <dl id="cell-facts" class="pairs ground-facts"></dl>
          <p id="cell-caveat" class="small-note"></p></details>
      </section>

      <!-- Select Tree mode (default): preserves exact Stage 4 R3 above-the-fold layout at 1280 x 720 and 1920 x 1080. -->
      <section id="tree-panel" class="panel tree-panel" aria-labelledby="inspector-heading">
        <div id="inspector-empty" class="inspector-empty">
          <h2 class="eyebrow-heading">Reference tree</h2>
          <p>No tree selected in this area. Click a crown on the map, use the Trees tab, or type an exact tree ID below.</p>
        </div>
        <div id="inspector-body" hidden>
          <div class="ref-card">
            <div class="ref-head">
              <div class="ref-id">
                <p class="ref-top"><span id="insp-role" class="eyebrow">Reference tree</span> <span id="insp-area" class="tag"></span></p>
                <div class="ref-title">
                  <h2 id="inspector-heading" tabindex="-1"><span id="insp-id" class="mono"></span></h2>
                  <span id="insp-chip" class="chip"></span>
                </div>
              </div>
              <button type="button" id="inspect-close" class="close-inspect" aria-keyshortcuts="Escape" title="Close inspection (Esc)"><span aria-hidden="true">×</span> Close inspection</button>
            </div>
            <p id="insp-summary" class="ref-sub"></p>
            <dl id="insp-key" class="key-facts"></dl>
          </div>

          <section id="competition-panel" class="competition" aria-labelledby="competition-heading">
            <h3 id="competition-heading">Competitors for Fell <span id="competition-sub" class="comp-sub"></span></h3>
            <div id="competition-content"></div>
          </section>

          <div id="add-dock" class="add-dock">
            <div id="batch-result" class="batch-result" tabindex="-1" hidden></div>
            <form id="add-form" class="add-form" novalidate hidden>
              <p class="draft-line"><span id="draft-count" class="draft-count"></span></p>
              <p class="add-row"><button type="submit" id="add-button" class="add-button"></button>
                <button type="button" id="draft-clear" class="link-button small-link" aria-label="Clear the Fell draft selection">Clear</button></p>
              <p id="add-caption" class="small-note">Queues proposals only. Approve and Execute are separate.</p>
            </form>
          </div>

          <section class="tree-more" aria-label="More for this tree">
            <details id="reason-details" class="reason-details">
              <summary>Fell reason: <span id="reason-summary"></span> · optional note</summary>
              <p id="release-wrap" class="release-option" hidden>
                <label><input type="checkbox" id="reason-release"><span id="release-label"></span></label>
              </p>
              <label for="reason">Optional note recorded with each Fell order added here</label>
              <textarea id="reason" rows="2" maxlength="2000"></textarea>
              <p id="reason-help" class="small-note">Note takes precedence; otherwise the ticked release intent, otherwise Owner-selected felling.</p>
              <p id="reason-error" class="field-error" role="alert" hidden></p>
              <p id="reason-preview" class="small-note" aria-live="polite"></p>
            </details>
            <div class="ref-actions">
              <p id="insp-pinned" class="small-note"></p>
              <p class="ref-links">
                <button type="button" id="crop-button" class="link-button small-link"></button>
                <span id="add-reference-wrap" hidden><button type="button" id="add-reference" class="link-button small-link"></button></span>
              </p>
            </div>
            <p id="crop-note" class="small-note crop-note"></p>
            <div id="prune-box" class="prune-box" hidden>
              <p class="prune-head"><span class="eyebrow">Pruning</span> <strong id="prune-lifts"></strong></p>
              <p id="prune-status" class="prune-status"></p>
              <p class="prune-actions">
                <button type="button" id="prune-add" class="small prune-add" aria-describedby="prune-why"></button>
                <button type="button" id="prune-remove" class="small secondary" hidden></button>
              </p>
              <p id="prune-why" class="small-note prune-why"></p>
            </div>
            <p id="insp-remove-wrap" class="ref-extra" hidden><button type="button" id="insp-remove" class="secondary"></button></p>
            <p id="insp-approved-note" class="note small-note" hidden>This tree is in the approved plan. Cancel the approved plan to change it.</p>
          </section>

          <details id="competition-details" class="more">
            <summary>Current / planned competition details</summary>
            <div id="competition-more"></div>
          </details>
          <details class="more tree-record"><summary>Tree record</summary>
            <dl id="insp-facts" class="pairs insp-facts"></dl>
            <p id="insp-threshold" class="small-note"></p>
          </details>
        </div>
        <div id="overlap" class="overlap" hidden>
          <h3 id="overlap-heading" tabindex="-1"></h3>
          <ul id="overlap-list" class="list"></ul>
        </div>
        <p class="field quick-id">
          <label for="quick-id">Select by exact tree ID (case-sensitive; press Enter)</label>
          <input id="quick-id" type="search" autocomplete="off" autocapitalize="off" spellcheck="false">
        </p>
      </section>
    </div>

    <div id="pane-plan" class="pane">
      <section class="panel" aria-labelledby="plan-heading">
        <p class="ref-top"><span class="eyebrow">Work Plan</span> <span class="tag">Both areas</span></p>
        <h2 id="plan-heading" tabindex="-1">Work plan</h2>
        <ol class="stepper" aria-label="Work Plan steps">
          <li id="step-pending">Pending</li>
          <li id="step-approved">Approved</li>
          <li id="step-execute">Execute</li>
        </ol>
        <div id="readout-unavailable" class="warning readout-unavailable" hidden>
          <p>The engine read-out for this view could not be paired with it, so Prune orders cannot be shown and Approve and Execute are withdrawn until a re-read.</p>
          <p><button type="button" id="readout-reread" class="small">Re-read current state</button></p>
        </div>
        <div id="pending-box">
          <div class="plan-state pending-state">
            <p id="pending-total" class="plan-total"></p>
            <p class="small-note">Pending orders are not felled. Approving fells zero trees and locks editing.</p>
          </div>
          <p class="plan-actions"><button type="button" id="approve" class="primary wide" hidden></button></p>
          <div id="pending-areas"></div>
        </div>
        <p id="approved-empty" class="note small-note">No approved plan.</p>
        <div id="approved-box" hidden>
          <div class="plan-state approved-state">
            <p id="approved-total" class="plan-total"></p>
            <p id="approved-sub" class="small-note"></p>
          </div>
          <p class="plan-actions"><button type="button" id="execute" class="execute wide"></button></p>
          <p class="small-note">Opens an exact-ID confirmation. No automatic execution.</p>
          <div id="approved-areas"></div>
          <p class="small-note">One shared plan. Each area’s tree IDs and outcomes remain separate.</p>
          <p class="actions"><button type="button" id="cancel-approved" class="secondary"></button></p>
        </div>
        <p id="plan-scope" class="small-note"></p>
        <p class="advisory">Advancing a year is a separate action. If work remains unexecuted, explicit cancellation consent is required.</p>
      </section>
      <section class="panel" aria-labelledby="receipt-heading">
        <div class="ref-top"><h2 id="receipt-heading" tabindex="-1">Latest receipt</h2><span class="wp-pill wp-completed">☑ Completed · History Log</span></div>
        <div id="receipt"></div>
      </section>
    </div>

    <div id="pane-trees" class="pane">
      <section class="panel" aria-labelledby="trees-heading">
        <h2 id="trees-heading" tabindex="-1">Trees</h2>
        <p class="field">
          <label for="tree-search">Filter by exact tree ID text (case-sensitive; Enter selects an exact match)</label>
          <input id="tree-search" type="search" autocomplete="off" autocapitalize="off" spellcheck="false">
        </p>
        <p id="rows-shown" class="small-note"></p>
        <div class="table-wrap" role="region" aria-labelledby="trees-heading" tabindex="0">
          <table id="tree-table">
            <caption id="tree-caption">Every tree of the shown area, with its exact ID</caption>
            <thead>
              <tr>
                <th scope="col">Select</th>
                <th scope="col">Tree ID</th>
                <th scope="col">Species</th>
                <th scope="col">Status</th>
                <th scope="col">DBH (cm)</th>
                <th scope="col">Height (m)</th>
                <th scope="col">Crown (m)</th>
                <th scope="col">X (m)</th>
                <th scope="col">Z (m)</th>
              </tr>
            </thead>
            <tbody id="tree-rows"></tbody>
          </table>
        </div>
      </section>
    </div>

    <div id="pane-history" class="pane">
      <section class="panel" aria-labelledby="year-heading">
        <h2 id="year-heading" tabindex="-1">Year results per area</h2>
        <div id="year-latest"></div>
        <div id="history"></div>
      </section>
    </div>

    <div id="pane-details" class="pane">
      <section class="panel" aria-labelledby="details-heading">
        <h2 id="details-heading" tabindex="-1">Experiment and technical provenance</h2>
        <ul id="statements" class="list statements"></ul>
        <dl id="facts" class="facts"></dl>
        <ul id="stage4-statements" class="list statements"></ul>
        <p id="details-note" class="note small-note"></p>
        <p id="screen-note" class="small-note">Screen size: this page is laid out for desktop browser windows of at least 1280 × 720. There, a selected tree's reference, its ranks 1–5 and Add fit without scrolling. Smaller windows still work but may need scrolling.</p>
      </section>
    </div>
  </div>
</main>

<dialog id="execute-dialog" aria-labelledby="execute-title" aria-describedby="execute-text">
  <h2 id="execute-title"></h2>
  <p id="execute-text"></p>
  <div id="execute-areas"></div>
  <div id="execute-plan" class="note"></div>
  <p class="actions">
    <button type="button" id="execute-back" class="secondary">Back</button>
    <button type="button" id="execute-confirm" class="execute">Confirm felling</button>
  </p>
</dialog>

<dialog id="consent-dialog" aria-labelledby="consent-title" aria-describedby="consent-text">
  <h2 id="consent-title"></h2>
  <p id="consent-text"></p>
  <div id="consent-areas"></div>
  <p id="consent-drafts" class="warning" hidden></p>
  <p class="actions">
    <button type="button" id="consent-keep" class="secondary"></button>
    <button type="button" id="consent-advance" class="danger"></button>
  </p>
</dialog>

<noscript><p class="noscript">This page needs JavaScript to talk to the local session process.</p></noscript>
</body>
</html>
""";

    internal const string Css = """
/* Two-area Stand Map (Pass 1 Map Integration, spec multi-map-integration-pass1-v1). Preserves Stage 4 Owner UX R3
   layout and fold geometry at 1280 x 720 and 1920 x 1080 with pure inline SVG symbols and read-only ecology overlays. */
:root {
  color-scheme: dark;
  --page: #0b1523; --top: #101d2e; --panel: #152438; --panel-2: #1a2c43; --panel-3: #20354f; --edge: #33475e; --line: #3d546d;
  --text: #e2ecf7; --muted: #b6c6d8; --faint: #8fa4ba;
  --blue: #2c4f70; --blue-edge: #91c6fa; --blue-hover: #355d82; --focus: #a8d4ff;
  --gold: #d9b866; --gold-edge: #f1d58e; --gold-text: #1c1708;
  --exec: #704c36; --exec-edge: #c99673; --exec-text: #fbf2e6;
  --danger: #6e3a35; --danger-edge: #e0a097;
  --alert-bg: #3a1f22; --alert-line: #e0a097;
  --notice-bg: #262a26; --notice-line: #5c5639; --notice-text: #f0d690;
  --exterior-1: #14263a; --exterior-2: #0f1c2b; --exterior-ink: #b6c6d8; --interior: #1b2430;
  --crop: #f5d57a; --reference: #ffffff; --draft: #f2ad76; --pending: #edc774; --approved: #88c5e8;
  --dead: #c4c9c6; --stump: #cbb48c; --stump-mark: #3a2a17;
  --gap: 12px; --gutter: 24px; --legend-w: 242px; --side-w: 380px;
}
*, *::before, *::after { box-sizing: border-box; }
html { -webkit-text-size-adjust: 100%; text-size-adjust: 100%; }
html.busy, html.busy * { cursor: progress; }
body { margin: 0; background: var(--page); color: var(--text); font: 0.95rem/1.45 system-ui, -apple-system, "Segoe UI", Roboto, Ubuntu, Arial, sans-serif; }
[hidden] { display: none !important; }
h1 { margin: 0; font-size: 1.5rem; line-height: 1.15; font-weight: 650; letter-spacing: -.01em; }
h2 { margin: 0 0 .5rem; font-size: 1.08rem; font-weight: 650; }
h3 { margin: .7rem 0 .3rem; font-size: 1rem; font-weight: 650; }
h4 { margin: .5rem 0 .25rem; font-size: .92rem; font-weight: 600; }
p { margin: .35rem 0; }
:focus { outline: none; }
:focus-visible { outline: 3px solid var(--focus); outline-offset: 3px; }
.mono, code { font-family: ui-monospace, "Cascadia Mono", Consolas, "DejaVu Sans Mono", monospace; overflow-wrap: anywhere; }
.sr-only { position: absolute; width: 1px; height: 1px; margin: -1px; padding: 0; overflow: hidden; clip: rect(0 0 0 0); white-space: nowrap; border: 0; }
.skip-link { position: absolute; left: .5rem; top: -5rem; z-index: 30; padding: .7rem 1rem; border-radius: 6px; background: var(--focus); color: #04121f; font-weight: 700; }
.skip-link:focus { top: .5rem; }
button, input, select, textarea, summary { font: inherit; color: inherit; }
button { min-width: 44px; min-height: 44px; padding: .45rem .95rem; border: 1px solid var(--line); border-radius: 8px; background: var(--panel-2); color: var(--text); font-weight: 600; cursor: pointer; }
button:hover { background: var(--panel-3); }
button.primary { border-color: var(--blue-edge); background: var(--blue); }
button.primary:hover { background: var(--blue-hover); }
button.secondary { border-color: var(--line); background: var(--panel-2); }
button.danger { border-color: var(--danger-edge); background: var(--danger); }
button.execute { border-color: var(--exec-edge); background: var(--exec); color: var(--exec-text); }
button.execute:hover { background: #845a40; }
button.wide { width: 100%; }
button.small { min-height: 36px; padding: .25rem .65rem; font-size: .86rem; }
button:disabled, button:disabled:hover { border-style: dashed; border-color: var(--line); background: var(--panel); color: var(--faint); cursor: not-allowed; }
.link-button { min-width: 0; padding: .35rem .2rem; border: 0; border-radius: 4px; background: none; color: var(--text); font-weight: 500; text-decoration: underline; text-underline-offset: 3px; text-decoration-color: var(--faint); }
.link-button:hover { background: none; text-decoration-color: var(--blue-edge); }
.link-button:disabled, .link-button:disabled:hover { border: 0; background: none; text-decoration: none; }
.small-link { font-size: .84rem; }
input, select, textarea { width: 100%; min-height: 44px; padding: .45rem .6rem; border: 1px solid var(--line); border-radius: 8px; background: var(--page); color: var(--text); }
textarea { min-height: 3.5rem; }
label { display: block; margin-bottom: .25rem; color: var(--muted); font-size: .86rem; }
.field { margin: .4rem 0; }
.eyebrow, .eyebrow-heading { margin: 0; color: var(--muted); font-size: .7rem; font-weight: 600; letter-spacing: .09em; text-transform: uppercase; }
.small-note { color: var(--muted); font-size: .8rem; }
.note { color: var(--muted); }
.tag { display: inline-flex; align-items: center; padding: .05rem .45rem; border: 1px solid var(--line); border-radius: 4px; background: var(--panel-2); color: var(--text); font-size: .72rem; }
.warning { padding: .45rem .7rem; border: 1px solid var(--pending); border-radius: 6px; background: #2b2718; font-weight: 600; }
.actions { display: flex; flex-wrap: wrap; gap: .6rem; }
.list { display: grid; gap: .4rem; margin: .3rem 0; padding: 0; list-style: none; }
.list li { overflow-wrap: anywhere; }

/* ---- Work Plan non-colour state pills (locked visual-semantics rule 3) ----------------------------------------- */
.wp-pill { display: inline-flex; align-items: center; gap: .3rem; padding: .1rem .5rem; border-radius: 999px; font-size: .72rem; font-weight: 700; white-space: nowrap; }
.wp-pill svg { width: 15px; height: 15px; flex: none; }
.wp-pending { border: 1.5px dashed var(--pending); background: #241f12; color: var(--pending); }
.wp-approved { border: 1.5px solid #bfe0f5; background: var(--approved); color: #071928; }
.wp-completed { border: 1px solid #637991; background: #233345; color: #d8e5f2; }

/* ---- top bar, summaries, notice --------------------------------------------------------------------------------- */
.topbar { display: flex; flex-wrap: wrap; align-items: center; gap: .6rem 1.2rem; padding: .75rem var(--gutter); border-bottom: 1px solid var(--edge); background: var(--top); }
.brand { display: flex; align-items: center; gap: .85rem; min-width: 0; flex: 1 1 20rem; }
.brand-text { min-width: 0; }
.brand-mark { flex: none; width: 1.75rem; height: 2.1rem; fill: #9cc8ee; }
.subtitle { margin: .15rem 0 0; color: var(--notice-text); font-size: .78rem; font-weight: 500; overflow: hidden; text-overflow: ellipsis; white-space: nowrap; }
.top-actions { display: flex; flex-wrap: wrap; align-items: center; gap: .6rem; flex: none; }
.year-chip { display: flex; align-items: baseline; gap: .5rem; margin: 0 .4rem 0 0; padding-right: 1rem; border-right: 1px solid var(--edge); }
.year-word { color: var(--muted); font-size: .78rem; letter-spacing: .06em; text-transform: uppercase; }
.year-chip strong { font-size: 1.55rem; font-weight: 650; font-variant-numeric: tabular-nums; }
.advance { display: flex; align-items: center; gap: .6rem; padding: .4rem 1.1rem; border-color: var(--blue-edge); background: var(--blue); text-align: left; }
.advance:hover { background: var(--blue-hover); }
.advance-text { display: grid; }
.btn-title { font-size: .92rem; }
.btn-sub { color: var(--muted); font-size: .74rem; font-weight: 500; }
.advance:disabled .btn-sub { color: var(--faint); }
.reread { padding: .45rem 1.1rem; }

.summary { display: grid; grid-template-columns: repeat(3, minmax(0, 1fr)); gap: var(--gap); padding: var(--gap) var(--gutter) 0; }
.area-cards { display: contents; }
.card { display: flex; align-items: center; gap: .8rem; min-width: 0; padding: .7rem 1.1rem; border: 1px solid var(--edge); border-radius: 8px; background: var(--panel); }
.area-card { justify-content: space-between; flex-wrap: wrap; row-gap: .25rem; }
.area-card .dot { flex: none; width: .5rem; height: .5rem; border-radius: 50%; background: var(--faint); }
.area-card.current .dot { background: var(--blue-edge); box-shadow: 0 0 0 3px #91c6fa33; }
.area-card-main { flex: 1 1 auto; min-width: 0; }
.area-card-count { margin: .15rem 0 0; }
.area-card-count strong { font-size: 1.45rem; font-weight: 650; font-variant-numeric: tabular-nums; }
.area-card-count span { color: var(--muted); font-size: .82rem; }
.area-card-side { text-align: right; }
.area-card-side strong { display: block; font-size: 1.12rem; font-weight: 600; font-variant-numeric: tabular-nums; }
.area-card-side span { color: var(--muted); font-size: .72rem; }
.plan-card { justify-content: space-between; }
.plan-card-text { min-width: 0; }
.plan-card-main { margin: .2rem 0 0; }
.plan-card-main strong { font-size: 1.2rem; font-weight: 650; }
.plan-card-sub { color: var(--muted); font-size: .82rem; }
#view-plan { flex: none; font-size: .95rem; text-decoration: none; }
#view-plan:hover { text-decoration: underline; }

.notice { display: flex; flex-wrap: wrap; align-items: center; gap: 0 .6rem; margin: var(--gap) var(--gutter) 0; padding: 0 .9rem; border: 1px solid var(--edge); border-radius: 8px; background: var(--panel); font-size: .8rem; }
.notice-icon { color: var(--muted); }
.notice-line { margin: 0; color: var(--muted); }
.notice-details > summary { display: inline-flex; align-items: center; min-height: 30px; padding: 0 .3rem; color: var(--text); text-decoration: underline; text-underline-offset: 3px; text-decoration-color: var(--faint); cursor: pointer; list-style: none; }
.notice-details > summary::-webkit-details-marker { display: none; }
.notice-details[open] { flex-basis: 100%; padding-bottom: .4rem; }
.notice-details[open] > summary { text-decoration-color: var(--blue-edge); }
.notice-body { padding: .1rem 0 0; }
.notice-text { margin: .2rem 0; color: var(--text); }
.notice-lead { color: var(--notice-text); font-weight: 600; }
.flags { margin: .2rem 0; color: var(--text); }
.flags abbr { text-decoration: none; }
.session-note { margin: .2rem 0; color: var(--text); }
.messages { padding: 0 var(--gutter); }
.alert.shown { margin-top: var(--gap); padding: .6rem 1rem; border: 1px solid var(--alert-line); border-radius: 8px; background: var(--alert-bg); }
.alert-title { font-weight: 700; }
.noscript { margin: 1rem; padding: 1rem; border: 2px solid var(--alert-line); background: var(--alert-bg); }

/* ---- workspace grid ------------------------------------------------------------------------------------------------ */
.workspace { display: grid; grid-template-columns: minmax(0, 1fr); grid-template-areas: "head" "map" "side" "legend"; gap: var(--gap); padding: var(--gap) var(--gutter); }
.workspace-head { grid-area: head; display: flex; flex-wrap: wrap; align-items: center; gap: .35rem .7rem; min-width: 0; }
.workspace-head h2 { margin: 0; font-size: 1.05rem; }
.map-subtitle { margin: 0; color: var(--muted); font-size: .84rem; white-space: nowrap; }
.mode-switch { display: inline-flex; border: 1px solid var(--line); border-radius: 8px; overflow: hidden; background: var(--panel); }
.mode-btn { min-width: 0; min-height: 36px; padding: .2rem .65rem; border: 0; border-radius: 0; background: transparent; font-size: .82rem; font-weight: 600; white-space: nowrap; }
.mode-btn + .mode-btn { border-left: 1px solid var(--line); }
.mode-btn[aria-pressed="true"] { background: var(--blue); color: #ffffff; }
.overlay-select { width: auto; min-height: 36px; padding: .2rem .6rem; font-size: .82rem; background: var(--panel-2); border-color: var(--line); border-radius: 8px; cursor: pointer; }
.announce { flex: 1 1 8rem; min-width: 0; margin: 0; overflow: hidden; color: var(--muted); font-size: .8rem; white-space: nowrap; text-overflow: ellipsis; }
.announce:not(:empty) { padding: .2rem .55rem; border: 1px solid var(--line); border-radius: 6px; background: var(--panel); color: var(--text); }
.view-tabs { display: flex; flex-wrap: wrap; gap: .2rem; margin-left: auto; }
.view-tab { display: inline-flex; align-items: center; gap: .4rem; min-height: 36px; padding: .25rem .7rem; border-color: transparent; background: transparent; font-weight: 500; }
.view-tab:hover { background: var(--panel); }
.view-tab[aria-pressed="true"] { border-color: var(--blue-edge); background: var(--blue); font-weight: 600; }
.count { display: inline-grid; place-items: center; min-width: 1.3rem; padding: 0 .3rem; border-radius: 4px; background: var(--panel-3); color: var(--text); font-size: .74rem; font-variant-numeric: tabular-nums; }
.map-panel { grid-area: map; display: flex; flex-direction: column; min-width: 0; overflow: hidden; border: 1px solid var(--edge); border-radius: 8px; background: var(--panel); }
.legend-panel { grid-area: legend; min-width: 0; padding: .75rem .85rem; border: 1px solid var(--edge); border-radius: 8px; background: var(--panel); }
.side { grid-area: side; display: grid; align-content: start; gap: var(--gap); min-width: 0; }
.pane { display: none; align-content: start; gap: var(--gap); min-width: 0; }
.side[data-pane="tree"] #pane-tree, .side[data-pane="plan"] #pane-plan, .side[data-pane="trees"] #pane-trees,
.side[data-pane="history"] #pane-history, .side[data-pane="details"] #pane-details { display: grid; }
.panel { min-width: 0; padding: .9rem 1rem; border: 1px solid var(--edge); border-radius: 8px; background: var(--panel); }

/* ---- the map ------------------------------------------------------------------------------------------------------- */
.map-pane { position: relative; flex: 1 1 auto; min-height: 0; height: clamp(24rem, 72vh, 44rem); overflow: hidden; background: radial-gradient(ellipse at 55% 45%, var(--exterior-1), var(--exterior-2) 110%); }
.map-pane.cell-mode #map { cursor: crosshair; }
#map { position: absolute; inset: 0; display: block; width: 100%; height: 100%; touch-action: manipulation; }
.map-foot { display: flex; flex-wrap: wrap; justify-content: space-between; gap: .2rem 1rem; padding: .4rem .9rem; border-top: 1px solid var(--edge); }
.map-note { margin: 0; color: var(--muted); font-size: .74rem; }
.map-dock { position: absolute; left: 14px; top: 14px; z-index: 3; width: auto; padding: .45rem .5rem .5rem; border: 1px solid var(--edge); border-radius: 8px; background: rgba(21, 36, 56, .97); box-shadow: 0 6px 18px rgba(8, 20, 34, .35); }
.dock-head { display: flex; justify-content: space-between; gap: .4rem; width: 140px; margin: 0 0 .35rem; color: var(--muted); font-size: .6rem; font-weight: 600; letter-spacing: .08em; text-transform: uppercase; }
.zoom-state { color: var(--text); letter-spacing: .04em; }
.compass { display: grid; grid-template-columns: repeat(3, 44px); grid-template-rows: repeat(3, 44px); gap: 4px 4px; }
#pan-north { grid-column: 2; grid-row: 1; }
#pan-west { grid-column: 1; grid-row: 2; }
#pan-east { grid-column: 3; grid-row: 2; }
#pan-south { grid-column: 2; grid-row: 3; }
.cmp { display: flex; flex-direction: column; align-items: center; justify-content: center; gap: 1px; font-size: .92rem; font-weight: 700; line-height: 1; }
.cmp-w, .cmp-e { flex-direction: row; gap: 3px; }
.cmp .arr { color: var(--muted); font-size: .72rem; font-weight: 600; }
.zoom-row { display: grid; grid-template-columns: repeat(3, 44px); gap: 4px; margin-top: 6px; padding-top: 6px; border-top: 1px solid var(--edge); }
button.icon { display: grid; place-items: center; width: 44px; height: 44px; padding: 0; border-color: var(--line); background: var(--panel-2); font-size: .78rem; line-height: 1; }
.zoom-row button.icon { font-size: 1.15rem; }
.zoom-row button.fit { font-size: .8rem; }
.map-dock button.icon:disabled { border-style: solid; border-color: #2b3d52; background: transparent; color: #5f7590; }
.map-dock button.icon:disabled .arr { color: inherit; }
.dock-areas { display: grid; grid-template-columns: 1fr 1fr; gap: 4px; width: 140px; margin-top: 6px; padding-top: 6px; border-top: 1px solid var(--edge); }
.area-tab { display: grid; justify-items: center; align-content: center; gap: .1rem; min-width: 0; min-height: 52px; padding: .25rem .15rem; border-color: var(--line); background: var(--panel-2); font-weight: 500; font-size: .78rem; line-height: 1.15; }
.area-tab[aria-pressed="true"] { border-color: var(--blue-edge); background: var(--blue); font-weight: 650; }
.area-thumb { width: 30px; height: 18px; }
.area-thumb polygon { fill: #6f8fb0; stroke: #dbe9f6; stroke-width: 1.2; vector-effect: non-scaling-stroke; }
.area-tab[aria-pressed="true"] .area-thumb polygon { fill: #a9cdef; stroke: #fff; }
.area-plan { color: var(--pending); font-size: .66rem; }
.rank-layer { position: absolute; inset: 0; z-index: 2; pointer-events: none; }
.rank-toggle { position: absolute; display: grid; place-items: center; width: 28px; height: 28px; min-width: 0; min-height: 0; padding: 0; transform: translate(-50%, -50%); border: 2px solid #e3d3a4; border-radius: 5px; background: #10202f; color: #f1e2b6; font-size: .84rem; font-weight: 800; line-height: 1; pointer-events: auto; box-shadow: 0 1px 4px rgba(2, 8, 16, .75); }
.rank-toggle:hover { background: #22384e; }
.rank-toggle[aria-pressed="true"] { border-color: #fff6ec; background: var(--draft); color: #1c1708; }
.rank-toggle[aria-pressed="true"]::after { content: "✓"; position: absolute; top: -9px; right: -9px; display: grid; place-items: center; width: 17px; height: 17px; border: 1.5px solid #fff6ec; border-radius: 50%; background: #10202f; color: #fff6ec; font-size: .62rem; font-weight: 800; }
.rank-toggle[aria-disabled="true"] { border-style: dashed; color: var(--faint); cursor: not-allowed; }
.rank-toggle:focus-visible { outline: 3px solid var(--focus); outline-offset: 2px; }
.map-orient { position: absolute; top: 12px; right: 14px; z-index: 2; display: flex; flex-direction: column; align-items: flex-end; gap: .5rem; }
.north { display: grid; justify-items: center; gap: 2px; margin: 0 .35rem 0 0; color: var(--exterior-ink); font-size: .72rem; font-weight: 600; }
.north svg { width: 12px; height: 18px; fill: var(--exterior-ink); }
.legend-show { padding: .35rem .8rem; border-color: var(--edge); background: var(--panel); box-shadow: 0 4px 12px rgba(8, 20, 34, .3); font-size: .84rem; }
.scale { position: absolute; left: 16px; bottom: 12px; z-index: 2; display: flex; align-items: center; gap: .45rem; margin: 0; color: var(--exterior-ink); font-size: .72rem; pointer-events: none; }
.scale-bar { display: block; overflow: visible; }
.scale-bar path { fill: none; stroke: var(--exterior-ink); stroke-width: 1.5; }
.map-status { position: absolute; right: 14px; bottom: 12px; z-index: 2; max-width: calc(100% - 28px); margin: 0; padding: .4rem .8rem; border: 1px solid var(--edge); border-radius: 6px; background: rgba(16, 29, 46, .95); color: var(--text); font-size: .78rem; pointer-events: none; }
.overlay { pointer-events: none; }
.ground { fill: url(#slate-ground); pointer-events: none; }
.grid-line { stroke: #3d546d; stroke-width: 1; vector-effect: non-scaling-stroke; stroke-opacity: .6; }
#grid-in .grid-line { stroke: #ffffff; stroke-opacity: .06; }
.grid-label { fill: #7f97ae; font-size: .9px; paint-order: stroke; stroke: var(--exterior-1); stroke-width: .25px; stroke-linejoin: round; }
.area-outline { fill: none; stroke: #7f97ae; stroke-width: 1.4; stroke-dasharray: 6 4; vector-effect: non-scaling-stroke; pointer-events: none; }
.tree .crown { fill: #000; fill-opacity: 0; stroke: none; pointer-events: fill; cursor: pointer; }
.map-pane.cell-mode .tree .crown { pointer-events: none; }
.deco, .deco * { pointer-events: none; }
.tree.dimmed { opacity: .25; }
.stump-disc { fill: var(--stump); stroke: #6e5a3c; stroke-width: .035; }
.stump-ring { fill: none; stroke: #8a7350; stroke-width: .03; }

/* Status marks */
.ring { fill: none; vector-effect: non-scaling-stroke; stroke-linecap: round; stroke-linejoin: round; pointer-events: none; }
.ring-halo { stroke: #06111d; stroke-opacity: .85; stroke-width: 6; }
.ring-reference { stroke: var(--reference); stroke-width: 3; }
.ring-inspect { stroke: var(--reference); stroke-width: 2; stroke-dasharray: 2 3; }
.ring-competitor { stroke: #e3d3a4; stroke-width: 2; }
.ring-draft { stroke: var(--draft); stroke-width: 2.5; }
.ring-pending { stroke: var(--pending); stroke-width: 2.2; stroke-dasharray: 5 4; }
.ring-approved { stroke: var(--approved); stroke-width: 2.2; }
.badge-bg { fill: #10202f; stroke-width: 1.5; vector-effect: non-scaling-stroke; }
.badge-text { font-weight: 800; text-anchor: middle; dominant-baseline: central; }
.b-crop .badge-bg { stroke: var(--crop); } .b-crop .badge-text { fill: var(--crop); }
.b-reference .badge-bg, .b-inspect .badge-bg { stroke: #fff; } .b-reference .badge-text, .b-inspect .badge-text { fill: #fff; }
.b-pending .badge-bg { fill: #231e11; stroke: var(--pending); stroke-dasharray: 3 2; } .b-pending .badge-text { fill: var(--pending); }
.b-approved .badge-bg { fill: var(--approved); stroke: #dff2ff; } .b-approved .badge-text { fill: #071928; }
.b-dead .badge-bg, .b-executed .badge-bg { fill: #1b2229; stroke: #e5e1d6; } .b-dead .badge-text, .b-executed .badge-text { fill: #eeeadd; }
.b-rank .badge-bg { stroke: #e3d3a4; } .b-rank .badge-text { fill: #f1e2b6; }
.b-rank.chosen .badge-bg { stroke: #fff6ec; fill: var(--draft); } .b-rank.chosen .badge-text { fill: #1c1708; }
.spread-leader { fill: none; stroke: #d5e2ee; stroke-opacity: .55; stroke-width: 1.2; stroke-dasharray: 2 3; vector-effect: non-scaling-stroke; pointer-events: none; }
.spread-origin { fill: #d5e2ee; fill-opacity: .7; pointer-events: none; }
.tick-box { fill: #10202f; stroke: var(--draft); stroke-width: 1.5; vector-effect: non-scaling-stroke; }
.tick { fill: none; stroke: #fff6ec; stroke-width: 2.2; stroke-linecap: round; stroke-linejoin: round; vector-effect: non-scaling-stroke; }
.stump-x { fill: none; stroke: var(--stump-mark); stroke-width: 2; stroke-linecap: round; vector-effect: non-scaling-stroke; }
.map-tag rect { fill: #10202f; fill-opacity: .94; stroke: #e2ecf7; stroke-opacity: .6; stroke-width: 1; vector-effect: non-scaling-stroke; }
.map-tag text { fill: #f2f7fc; font-weight: 600; text-anchor: middle; dominant-baseline: central; }

/* Ground overlays, ClearUndergrowth cell highlight, Natural Seedling badge */
.overlay-legend { display: flex; flex-wrap: wrap; align-items: center; gap: .2rem .65rem; padding: .3rem .9rem; border-top: 1px solid var(--edge); background: var(--panel-2); font-size: .74rem; }
.overlay-legend-title { margin: 0; font-weight: 700; }
.light-ramp { display: block; width: 8.5rem; height: 12px; border: 1px solid var(--line); border-radius: 3px; }
.ramp-end { color: var(--muted); }
.overlay-swatch { width: 22px; height: 14px; border: 1px solid var(--line); border-radius: 3px; vertical-align: middle; }
.overlay-legend-note { margin: 0 0 0 auto; color: var(--muted); }
.light-cell { stroke: none; pointer-events: none; }
.seedling-tag { fill: #e9f5d0; font-weight: 700; text-anchor: middle; dominant-baseline: central; paint-order: stroke; stroke: #07131e; stroke-width: .18px; }
.clear-cell-pending { fill: #4f3a1c; fill-opacity: .34; stroke: var(--pending); stroke-width: 2; stroke-dasharray: 5 3; vector-effect: non-scaling-stroke; }
.clear-cell-approved { fill: #2a5572; fill-opacity: .36; stroke: var(--approved); stroke-width: 2.2; vector-effect: non-scaling-stroke; }
.ground-pick { fill: none; stroke: #ffffff; stroke-width: 2; stroke-dasharray: 4 3; vector-effect: non-scaling-stroke; }
.ground-pick-halo { fill: none; stroke: #06111d; stroke-opacity: .85; stroke-width: 5; vector-effect: non-scaling-stroke; }

/* ---- legend -------------------------------------------------------------------------------------------------------- */
.legend-head { display: flex; flex-wrap: wrap; align-items: center; justify-content: space-between; gap: 0 .5rem; padding-bottom: .35rem; border-bottom: 1px solid var(--edge); }
.legend-head h2 { margin: 0; font-size: 1rem; white-space: nowrap; }
.legend { display: grid; gap: 0; margin: 0; padding: 0; list-style: none; }
.legend li { display: grid; grid-template-columns: 36px minmax(0, 1fr); align-items: center; gap: .55rem; padding: .34rem 0; border-bottom: 1px solid var(--edge); }
.legend svg { width: 36px; height: 36px; overflow: visible; }
.legend-name { display: block; font-size: .8rem; font-weight: 600; }
.legend-desc { display: block; color: var(--muted); font-size: .7rem; line-height: 1.25; }
.legend-foot { margin: .5rem 0 0; color: var(--muted); font-size: .7rem; line-height: 1.3; }
.legend .swatch-ground { fill: var(--interior); }
.legend .legend-exterior { fill: #14263a; }
.legend .legend-area { fill: var(--interior); stroke: #7f97ae; stroke-width: 1.4; stroke-dasharray: 3 2.5; vector-effect: non-scaling-stroke; }

/* ---- side panel: reference tree, competitors, and cell inspector -------------------------------------------------- */
.tree-panel { padding: 0; overflow: visible; }
.inspector-empty { padding: .9rem 1rem; }
.ref-card { margin: 0; padding: .9rem 1rem .7rem; border-bottom: 1px solid var(--edge); }
.ref-top { display: flex; align-items: center; justify-content: space-between; gap: .5rem; margin: 0 0 .4rem; }
.ref-title { display: flex; flex-wrap: wrap; align-items: center; gap: .4rem; }
.ref-title h2 { margin: 0; font-size: 1.55rem; font-weight: 650; letter-spacing: -.01em; }
.ref-title .eyebrow { margin: 0; }
.ref-head { display: flex; align-items: center; gap: .5rem; }
.ref-id { flex: 1 1 auto; min-width: 0; overflow: hidden; }
.ref-id .ref-top { justify-content: flex-start; white-space: nowrap; overflow: hidden; }
.ref-id .ref-top .eyebrow { min-width: 0; overflow: hidden; text-overflow: ellipsis; }
.ref-id .ref-title { flex-wrap: nowrap; }
.ref-id .ref-title h2 { flex: none; }
.ref-id .chip { display: block; min-width: 0; overflow: hidden; text-overflow: ellipsis; white-space: nowrap; }
.close-inspect { display: inline-flex; align-items: center; gap: .3rem; min-height: 36px; margin-left: auto; padding: .2rem .65rem; border-color: var(--line); background: var(--panel-2); font-size: .82rem; font-weight: 600; white-space: nowrap; }
.close-inspect > span[aria-hidden] { font-size: 1.15rem; line-height: 1; }
.chip { display: inline-flex; align-items: center; padding: .15rem .55rem; border: 1px solid var(--line); border-radius: 4px; background: var(--panel-2); font-size: .76rem; font-weight: 600; }
.chip.crop { border-color: var(--crop); color: var(--crop); }
.chip.s-PendingFell { border-color: var(--pending); color: var(--pending); }
.chip.s-ApprovedFell { border-color: var(--approved); color: var(--approved); }
.chip.s-Felled { border-color: var(--stump); color: var(--stump); }
.chip.s-NaturalDead { border-color: var(--dead); color: var(--dead); }
.ref-sub { margin: .1rem 0 .55rem; color: var(--muted); font-size: .86rem; }
.key-facts { display: grid; grid-template-columns: repeat(3, minmax(0, 1fr)); gap: .3rem .6rem; margin: 0; }
.key-facts dt { color: var(--muted); font-size: .68rem; white-space: nowrap; }
.key-facts dd { margin: 0; font-size: 1.12rem; font-weight: 650; font-variant-numeric: tabular-nums; white-space: nowrap; }
.key-facts dd small { color: var(--muted); font-size: .72rem; font-weight: 500; }
.ref-actions { display: flex; flex-wrap: wrap; align-items: center; justify-content: space-between; gap: 0 .6rem; margin-top: .35rem; }
.ref-actions p { margin: 0; }
.ref-links { display: flex; flex-wrap: wrap; gap: 0 .8rem; margin-left: auto; }
.ref-links .link-button { min-height: 36px; }
.crop-note { margin: .1rem 0 0; }
.ref-extra { margin: .5rem 0 0; }
.ref-extra button { width: 100%; }
.competition { margin: 0; padding: .75rem 1rem .2rem; }
.competition h3 { margin: 0 0 .35rem; }
.comp-sub { color: var(--muted); font-size: .76rem; font-weight: 500; }
.competition .small-note { margin: .15rem 0 .45rem; }
.tree-more { margin: .4rem 1rem 0; padding-top: .3rem; border-top: 1px solid var(--edge); }
.tree-more .ref-actions { margin-top: .4rem; }
.comp-head { display: grid; grid-template-columns: 2rem minmax(0, 1fr) 3.5rem 3.9rem; gap: .35rem; padding: 0 .55rem .2rem; color: var(--muted); font-size: .68rem; white-space: nowrap; }
.comp-head span:nth-child(n+3) { text-align: right; }
.comp-rows { display: grid; gap: .4rem; margin: 0; padding: 0; list-style: none; }
.comp-row label { display: grid; grid-template-columns: 2rem minmax(0, 1fr) 3.5rem 3.9rem; align-items: center; gap: .35rem; min-height: 50px; margin: 0; padding: .3rem .55rem; border: 1px solid var(--line); border-radius: 6px; background: var(--panel-2); color: var(--text); font-size: .9rem; cursor: pointer; }
.comp-row label:hover { border-color: var(--faint); }
.comp-row.chosen label { border-color: var(--gold); background: #2c2a23; }
.comp-row.blocked label { border-style: dashed; background: var(--panel); cursor: not-allowed; }
.comp-row input { width: 22px; height: 22px; min-height: 0; margin: 0 auto; accent-color: var(--gold); cursor: inherit; }
.comp-rows > .comp-row > label { grid-template-rows: auto auto; row-gap: 0; }
.comp-row input { grid-row: 1 / span 2; }
.comp-id { display: block; grid-column: 2; grid-row: 1; min-width: 0; font-weight: 650; overflow-wrap: anywhere; }
.comp-id .rank { margin-right: .35rem; color: var(--gold-edge); font-style: italic; font-weight: 700; }
.comp-dist { grid-column: 3; grid-row: 1; }
.comp-ci { grid-column: 4; grid-row: 1 / span 2; }
.comp-line { display: block; grid-column: 2 / span 2; grid-row: 2; min-width: 0; overflow: hidden; color: var(--muted); font-size: .72rem; white-space: nowrap; text-overflow: ellipsis; }
.comp-why { color: var(--pending); font-weight: 600; }
.comp-num { text-align: right; font-variant-numeric: tabular-nums; font-size: .84rem; }
.comp-strength { display: block; color: var(--muted); font-size: .68rem; }
.add-row { display: flex; align-items: center; gap: .5rem; }
.add-row .add-button { flex: 1 1 auto; width: auto; padding: .45rem .6rem; white-space: nowrap; }
.add-row .link-button { flex: none; }
.comp-empty { margin: .3rem 0; color: var(--muted); }
.batch-result { max-height: 11rem; overflow-y: auto; margin: .5rem 1rem 0; padding: .55rem .75rem; border: 1px solid var(--line); border-left: 4px solid var(--approved); border-radius: 6px; background: var(--panel-2); font-size: .84rem; }
.batch-result.partial { border-left-color: var(--pending); }
.batch-result.failed { border-left-color: var(--alert-line); }
.batch-result p { margin: .2rem 0; }
.batch-result .alert-title { font-weight: 700; }
.batch-head { display: flex; align-items: center; gap: .5rem; }
.batch-head .alert-title { flex: 1 1 auto; min-width: 0; margin: 0; }
.batch-head .link-button { flex: none; min-height: 28px; padding: .1rem .2rem; font-size: .8rem; }
.batch-more { display: none; }
.add-dock { position: sticky; bottom: 0; z-index: 2; margin-top: .5rem; background: var(--panel); box-shadow: 0 -8px 14px rgba(11, 21, 35, .55); }
.add-dock:has(> :not([hidden])) { border-top: 1px solid var(--edge); }
.add-form { margin: 0; padding: .55rem 1rem .7rem; }
.add-form p { margin: .25rem 0; }
.draft-line { display: flex; align-items: center; justify-content: space-between; gap: .5rem; }
.draft-count { color: var(--gold-edge); font-size: .86rem; font-weight: 600; }
.add-button { width: 100%; min-height: 48px; border-color: var(--gold-edge); background: var(--gold); color: var(--gold-text); font-size: .98rem; font-weight: 700; }
.add-button:hover { background: #e4c475; }
.add-button:disabled, .add-button:disabled:hover { border-color: var(--line); background: var(--panel); color: var(--faint); }
.reason-details { margin-top: .2rem; border: 1px solid var(--line); border-radius: 6px; background: var(--page); }
.reason-details summary { min-height: 40px; padding: .6rem .65rem; color: var(--text); font-size: .8rem; line-height: 1.3; cursor: pointer; }
.reason-details[open] { padding-bottom: .5rem; }
.reason-details > :not(summary) { margin-left: .65rem; margin-right: .65rem; }
.release-option label { display: flex; align-items: center; gap: .5rem; color: var(--text); font-size: .82rem; }
.release-option input { flex: none; width: 24px; height: 24px; min-height: 0; margin: 10px; accent-color: var(--gold); }
.field-error { margin: .25rem 0; padding: .3rem .6rem; border-left: 4px solid var(--alert-line); background: var(--alert-bg); font-weight: 600; }
[aria-invalid="true"] { border-color: var(--alert-line); }
.more { margin: .6rem 1rem 0; border-top: 1px solid var(--edge); }
.more summary { min-height: 44px; display: flex; align-items: center; color: var(--muted); font-size: .84rem; cursor: pointer; }
.more[open] { padding-bottom: .4rem; }
.competition-totals { margin: .2rem 0; padding: .3rem .55rem; border-left: 3px solid var(--approved); font-size: .82rem; }
.pairs { display: grid; gap: .3rem; margin: .3rem 0; }
dt { color: var(--muted); font-size: .8rem; }
dd { margin: 0; overflow-wrap: anywhere; }
.insp-facts { display: flex; flex-wrap: wrap; gap: .3rem .9rem; }
.insp-facts > div { flex: 1 1 6.5rem; }
.insp-facts > div.wide { flex-basis: 100%; }
.overlap { margin: .6rem 1rem 0; padding: .6rem; border: 1px dashed var(--pending); border-radius: 6px; }
.overlap h3 { margin-top: 0; font-size: .9rem; }
.quick-id { margin: .6rem 1rem .9rem; padding-top: .5rem; border-top: 1px solid var(--edge); }

/* Cell Inspector */
.cell-panel h2 { margin: .15rem 0 .45rem; font-size: 1.05rem; }
.cell-summary dl { display: grid; grid-template-columns: auto minmax(0, 1fr); gap: .2rem .75rem; margin: .35rem 0; }
.cell-summary dd { font-variant-numeric: tabular-nums; }
.cell-cohorts { margin-top: .55rem; }
.ground-level { display: flex; align-items: center; gap: .5rem; margin: 0 0 .3rem; font-size: 1.02rem; font-weight: 650; }
.ground-swatch { flex: none; width: 22px; height: 22px; border: 1px solid var(--line); border-radius: 3px; }
.ground-more { margin: .45rem 0 0; }
.ground-more summary { min-height: 36px; }
.ground-facts { display: grid; grid-template-columns: minmax(0, 1fr); gap: .2rem; }
.ground-facts > div { display: grid; grid-template-columns: 8.5rem minmax(0, 1fr); gap: .5rem; }
.ground-facts dd { font-variant-numeric: tabular-nums; }
.ids { overflow-wrap: normal; }
.ids .id { white-space: nowrap; }

/* ---- side panel: Work Plan, receipts, tables ------------------------------------------------------------------- */
.stepper { display: flex; flex-wrap: wrap; gap: .3rem; margin: 0 0 .6rem; padding: 0; list-style: none; counter-reset: step; font-size: .74rem; color: var(--muted); }
.stepper li { counter-increment: step; }
.stepper li::before { content: counter(step) " "; }
.stepper li + li::before { content: "→ " counter(step) " "; }
.stepper li.now { color: var(--text); font-weight: 700; }
.plan-state { margin: .3rem 0 .5rem; padding: .7rem .9rem; border: 1px solid var(--line); border-radius: 6px; }
.pending-state { border-color: var(--pending); border-style: dashed; background: #25231a; }
.approved-state { border-color: #5b8fa6; background: #1b3a40; }
.plan-total { margin: 0; font-size: 1.1rem; font-weight: 650; }
.plan-actions { margin: .5rem 0; }
.plan-area { margin: .5rem 0; padding-top: .4rem; border-top: 1px solid var(--edge); }
.plan-area h4 { margin: .1rem 0 .3rem; color: var(--muted); font-weight: 500; }
.orders li { display: grid; grid-template-columns: minmax(0, 1fr) auto; align-items: center; gap: .15rem .6rem; padding: .35rem 0; border-bottom: 1px solid var(--edge); }
.orders li:last-child { border-bottom: 0; }
.orders .mono { font-weight: 650; }
.order-head { display: flex; flex-wrap: wrap; align-items: center; gap: .35rem; }
.order-reason { grid-column: 1; color: var(--muted); font-size: .76rem; }
.orders button { grid-column: 2; grid-row: 1 / span 2; }
.advisory { margin-top: .8rem; padding: .6rem .8rem; border-radius: 6px; background: var(--panel-3); font-size: .8rem; }
.statements li { padding: .25rem .5rem; border-left: 3px solid var(--notice-line); background: rgba(0, 0, 0, .18); font-size: .86rem; }
.facts { display: grid; grid-template-columns: repeat(auto-fill, minmax(min(100%, 10rem), 1fr)); gap: .45rem 1rem; margin: .6rem 0; }
details.expand summary { display: inline-flex; align-items: center; min-height: 44px; cursor: pointer; }
details.expand .full { display: block; padding: .2rem 0 .4rem; }
.technical-plan summary { min-height: 44px; display: flex; align-items: center; cursor: pointer; }
.technical-plan { font-size: .82rem; color: var(--muted); }
.table-wrap { max-width: 100%; overflow-x: auto; }
table { width: 100%; border-collapse: collapse; font-size: .84rem; }
caption { padding: .3rem 0; color: var(--muted); text-align: left; caption-side: top; }
th, td { padding: .28rem .42rem; border-bottom: 1px solid var(--edge); text-align: left; vertical-align: middle; }
#tree-table td, #tree-table tbody th { white-space: nowrap; }
thead th { position: sticky; top: 0; z-index: 1; background: var(--panel-2); }
tr.current { background: var(--panel-3); }
tr.current > * { border-top: 2px solid var(--reference); border-bottom: 2px solid var(--reference); }
dialog { width: min(38rem, calc(100vw - 2rem)); max-height: calc(100vh - 2rem); overflow: auto; padding: 1.2rem; border: 1px solid var(--edge); border-radius: 10px; background: var(--panel); color: var(--text); }
dialog::backdrop { background: rgba(3, 9, 17, .72); }
dialog .actions { justify-content: flex-end; margin-top: 1rem; }

/* ---- Area readouts and prune box -------------------------------------------------------------------------------- */
.area-readouts { display: flex; flex-wrap: wrap; flex-basis: 100%; gap: .1rem .75rem; margin: 0; padding-top: .3rem; border-top: 1px solid var(--edge); font-size: .74rem; }
.area-readouts > div { display: flex; align-items: baseline; gap: .25rem; min-width: 0; }
.area-readouts dt { color: var(--muted); font-size: .68rem; }
.area-readouts dd { margin: 0; font-weight: 650; font-variant-numeric: tabular-nums; white-space: nowrap; }
.area-readouts dd small { color: var(--muted); font-weight: 500; }
.model-badge { display: inline-flex; flex-wrap: wrap; align-items: center; gap: .25rem; }
.model-badge abbr { padding: 0 .35rem; border: 1px solid var(--line); border-radius: 4px; background: var(--panel-2); color: var(--text); font-size: .66rem; font-weight: 600; text-decoration: none; white-space: nowrap; }
.readout-unavailable p { margin: .2rem 0; }
.prune-box { margin: .45rem 0 0; padding: .45rem .6rem; border: 1px solid var(--line); border-radius: 6px; background: var(--panel-2); }
.prune-box p { margin: .15rem 0; }
.prune-head { display: flex; align-items: baseline; justify-content: space-between; gap: .5rem; }
.prune-head strong { font-variant-numeric: tabular-nums; }
.prune-status { font-size: .82rem; }
.prune-actions { display: flex; flex-wrap: wrap; gap: .4rem; }
.prune-add { border-color: var(--approved); }
.prune-why { margin: 0; }

/* ---- desktop: one screen, panes scroll inside ------------------------------------------------------------------ */
@media (min-width: 1024px) {
  body { display: flex; flex-direction: column; height: 100vh; min-height: 38rem; }
  .topbar, .summary, .notice, .messages { flex: none; }
  .workspace { flex: 1 1 auto; min-height: 0; grid-template-columns: minmax(0, 1fr) var(--legend-w) var(--side-w);
    grid-template-rows: auto minmax(0, 1fr); grid-template-areas: "head head head" "map legend side"; }
  .workspace[data-legend="hidden"] { grid-template-columns: minmax(0, 1fr) var(--side-w); grid-template-areas: "head head" "map side"; }
  .workspace[data-legend="hidden"] .legend-panel { display: none; }
  .map-pane { height: auto; }
  .legend-panel { display: flex; flex-direction: column; min-height: 0; overflow-y: auto; }
  .side { display: flex; flex-direction: column; min-height: 0; }
  .pane { flex: 1 1 auto; min-height: 0; overflow-y: auto; scrollbar-gutter: stable; }
  .side[data-pane="tree"] #pane-tree, .side[data-pane="plan"] #pane-plan, .side[data-pane="trees"] #pane-trees,
  .side[data-pane="history"] #pane-history, .side[data-pane="details"] #pane-details { display: block; }
  .pane > .panel + .panel { margin-top: var(--gap); }
  .overlap { max-height: 9rem; overflow-y: auto; }
  .add-dock.showing-result .add-form { display: none; }
}
/* Compact desktop (1280 x 720): narrower columns, tighter gaps; R3 fold margin preserved. */
@media (min-width: 1024px) and (max-width: 1599px) {
  :root { --gap: 8px; --gutter: 16px; --legend-w: 184px; --side-w: 310px; }
  body { font-size: .9rem; }
  h1 { font-size: 1.22rem; }
  .subtitle { font-size: .72rem; }
  .topbar { padding: .48rem var(--gutter); }
  .brand-mark { width: 1.45rem; height: 1.75rem; }
  .card { padding: .45rem .85rem; }
  .area-card-count strong { font-size: 1.2rem; }
  .plan-card-main strong { font-size: 1.02rem; }
  .notice { padding: .15rem .8rem; font-size: .75rem; }
  .legend-panel { padding: .45rem .55rem; }
  .legend-head h2 { font-size: .88rem; }
  .legend-head .link-button { min-height: 32px; padding: .2rem 0; font-size: .74rem; }
  .legend li { grid-template-columns: 24px minmax(0, 1fr); gap: .4rem; padding: .12rem 0; }
  .legend svg { width: 24px; height: 24px; }
  .legend-desc { display: none; }
  .legend-name { font-size: .72rem; }
  .legend-foot { margin-top: .35rem; font-size: .66rem; }
  .ref-sub { display: none; }
  .key-facts dt { font-size: .62rem; }
  #add-caption { font-size: .72rem; }
  .view-tab { padding: .25rem .5rem; font-size: .84rem; }
  .mode-btn, .overlay-select { min-height: 32px; font-size: .78rem; padding: .15rem .5rem; }
  .ref-card { padding: .55rem .8rem .45rem; }
  .competition { padding: .45rem .8rem .1rem; }
  .add-form { padding: .4rem .8rem .5rem; }
  .batch-result { max-height: 7rem; margin: .4rem .8rem 0; }
  .ref-title h2 { font-size: 1.3rem; }
  .key-facts dd { font-size: .98rem; }
  .comp-head, .comp-row label { grid-template-columns: 1.7rem minmax(0, 1fr) 3.3rem 3.7rem; gap: .3rem; }
  .comp-row label { min-height: 48px; padding: .25rem .45rem; font-size: .84rem; }
  .map-dock { left: 10px; top: 10px; }
  .close-inspect { min-height: 34px; padding: .15rem .55rem; font-size: .78rem; }
  .area-readouts { gap: 0 .38rem; padding-top: .18rem; font-size: .65rem; }
  .area-readouts dt { font-size: .62rem; }
  .overlay-legend { padding: .22rem .8rem; font-size: .7rem; }
  .ground-facts > div { grid-template-columns: minmax(0, 1fr); gap: 0; }
}
/* Short desktop (1280 x 720): reference, ranks 1-5 and Add stay above the fold without scrolling (Stage 4 R3 gate). */
@media (min-width: 1024px) and (max-height: 820px) {
  .topbar { padding-top: .36rem; padding-bottom: .36rem; }
  .btn-sub { font-size: .7rem; }
  .area-tab { min-height: 46px; }
  .area-thumb { width: 26px; height: 15px; }
  .crop-note { display: none; }
  .competition .small-note { margin-bottom: .3rem; }
  #insp-pinned { display: none; }
  #add-caption { display: none; }
  .ref-card { padding: .38rem .8rem .28rem; }
  .ref-top { margin-bottom: .1rem; }
  .ref-id .tag { display: none; }
  .ref-title h2 { font-size: 1.15rem; }
  .key-facts { margin-top: .18rem; }
  .key-facts > div { display: flex; align-items: baseline; gap: .25rem; min-width: 0; }
  .key-facts dd { font-size: .9rem; }
  .competition { padding: .24rem .8rem 0; }
  .competition h3 { margin-bottom: .14rem; font-size: .92rem; }
  .comp-head { padding-bottom: 0; }
  .comp-rows { gap: .24rem; }
  .comp-row label { min-height: 38px; padding: .1rem .45rem; line-height: 1.2; }
  .add-dock { margin-top: .28rem; }
  .batch-result { max-height: none; overflow: visible; margin: .3rem .8rem; padding: .2rem .5rem; }
  .batch-head .alert-title { overflow: hidden; white-space: nowrap; text-overflow: ellipsis; }
  .batch-more { display: inline-flex; }
  .batch-body { display: none; }
  .batch-result.open .batch-body { display: block; max-height: 6rem; overflow-y: auto; }
  .add-form { padding: .28rem .8rem .38rem; }
  .add-form p { margin: 0; }
  .draft-line { display: none; }
  .card { padding: .32rem .75rem; }
  .area-card-count { margin-top: 0; }
  .area-card-count strong { font-size: 1.08rem; }
  .area-card-side strong { font-size: .98rem; }
  .notice-details > summary { min-height: 24px; }
  .notice { font-size: .74rem; }
  .ref-links .link-button { min-height: 32px; }
  .add-button { min-height: 44px; }
  .add-row .add-button { font-size: .86rem; }
  .reason-details summary { min-height: 36px; padding: .45rem .6rem; }
  .area-readouts .model-badge-extra { display: none; }
}
@media (max-width: 1023px) {
  :root { --gap: 10px; --gutter: 12px; }
  .summary { grid-template-columns: minmax(0, 1fr); }
  .top-actions { width: 100%; }
  .year-chip { margin-right: auto; }
  .view-tabs { width: 100%; margin-left: 0; }
  .legend { grid-template-columns: repeat(auto-fill, minmax(min(100%, 14rem), 1fr)); column-gap: 1rem; }
  .workspace[data-legend="hidden"] .legend-panel { display: none; }
  .add-dock { position: static; box-shadow: none; }
  .session-note { margin-left: 0; }
}
@media (max-width: 520px) {
  .key-facts { grid-template-columns: repeat(2, minmax(0, 1fr)); }
  .map-pane { height: clamp(30rem, 90vh, 40rem); }
  .comp-head, .comp-row label { grid-template-columns: 2.2rem minmax(0, 1fr) 3.6rem 3.2rem; }
}
""";

    internal const string Js = """
// Two-area Stand Map (Pass 1 Map Integration, spec multi-map-integration-pass1-v1).
// Presentation authority (D-020): this script computes zero forestry, canopy, light, RD, DBH->radius or pruning rules.
// Every value comes from the server's atomic composite view/stage4/ecology/cell payload. Pure inline SVG only; zero WebP.
'use strict';

(function () {
  const SVG_NS = 'http://www.w3.org/2000/svg';
  const STATUS_LABEL = { Living: 'StatusLiving', PendingFell: 'StatusPendingFell', ApprovedFell: 'StatusApprovedFell',
    Felled: 'StatusFelled', NaturalDead: 'StatusNaturalDead' };
  const SPECIES_NAME = { 'sitka-spruce': 'Sitka spruce', 'sessile-oak': 'Sessile oak', beech: 'European beech' };
  const SPECIES_SHORT = { 'sitka-spruce': 'S', 'sessile-oak': 'O', beech: 'B' };
  const SPECIES_SYMBOL = { 'sitka-spruce': 'ccf-sitka', 'sessile-oak': 'ccf-oak', beech: 'ccf-beech' };
  const SAPLING_SYMBOL = { 'sitka-spruce': 'ccf-sapling-sitka', 'sessile-oak': 'ccf-sapling-oak', beech: 'ccf-sapling-beech' };
  const ZOOMS = Array.from({ length: 11 }, (_, i) => Math.pow(4, i / 10));
  const PANES = ['tree', 'plan', 'trees', 'history', 'details'];
  const PANE_NAMES = { tree: 'Tree', plan: 'Work Plan', trees: 'Trees', history: 'Results', details: 'Details' };
  const COMMAND_IDS = ['crop-button', 'add-reference', 'insp-remove', 'approve', 'cancel-approved', 'execute',
    'execute-confirm', 'advance', 'consent-advance', 'prune-add', 'prune-remove'];
  const LIGHT_STOPS = [[0x7f, 0x3b, 0x4a], [0xda, 0x79, 0x6d], [0xff, 0xd9, 0xbd]];
  const LIGHT_EASE = 0.65;
  const LIGHT_ALPHA_MAX = 0.9;
  const LIGHT_WORDS = [[0.1, 'Very shaded'], [0.35, 'Low'], [0.65, 'Moderate'], [Infinity, 'Bright']];
  const SLATE = [0x1b, 0x24, 0x30];
  const STATEMENTS = ['MultiSyntheticStatement', 'MultiNotComparable', 'MultiSiteUnknown', 'MultiEdgeExcluded',
    'MultiNoTransfer', 'MultiSessionOnly'];
  const CHIPS = [['MultiChipSynthetic', 'MultiSyntheticStatement'], ['MultiChipNotComparable', 'MultiNotComparable'],
    ['MultiChipSiteUnknown', 'MultiSiteUnknown'], ['MultiChipEdgeExcluded', 'MultiEdgeExcluded'],
    ['MultiChipNoTransfer', 'MultiNoTransfer']];
  const DEFAULT_REASON = 'Owner-selected felling';

  const state = {
    view: null,
    scenes: {},
    labels: null,
    stage4: null,
    ecology: null,
    areaId: null,
    selected: {},
    drafts: {},
    draftNotice: null,
    overlap: null,
    competition: null,
    competitionTarget: null,
    competitionTag: null,
    competitionRequest: 0,
    busy: false,
    mapPick: 0,
    cellPick: 0,
    batchResultOpen: false,
    batch: null,
    batchResult: null,
    lock: null,
    lockCount: 0,
    confirmation: null,
    consentWork: false,
    opener: null,
    lastExecution: null,
    lastWasReplay: false,
    zoom: 1,
    centre: null,
    legendHidden: false,
    mapMode: 'tree', // 'tree' (Select Tree, default, crown wins) | 'cell' (Inspect Cell, swaps right inspector)
    overlay: 'off', // 'off' | 'light' | 'undergrowth' | 'seedlings'
    cellDetail: null, // server cell detail from POST api/multi/cell, or { areaId, missing: true }
    cellLoading: false,
  };

  const byId = (id) => document.getElementById(id);

  // ---- small DOM helpers ----------------------------------------------------------------------------------------------

  function setAttributes(node, attributes) {
    for (const [name, value] of Object.entries(attributes || {})) {
      if (value !== null && value !== undefined && value !== false) node.setAttribute(name, String(value));
    }
  }

  function el(tag, attributes, ...children) {
    const node = document.createElement(tag);
    setAttributes(node, attributes);
    for (const child of children) {
      if (child === null || child === undefined) continue;
      node.append(child instanceof Node ? child : String(child));
    }
    return node;
  }

  function svg(tag, attributes, textValue) {
    const node = document.createElementNS(SVG_NS, tag);
    setAttributes(node, attributes);
    if (textValue !== undefined) node.textContent = String(textValue);
    return node;
  }

  function useIcon(id, size) {
    const box = svg('svg', { viewBox: '0 0 32 32', width: size || 16, height: size || 16, 'aria-hidden': 'true', focusable: 'false' });
    box.append(svg('use', { href: '#' + id, x: 0, y: 0, width: 32, height: 32 }));
    return box;
  }

  function pair(term, value, className) {
    return el('div', className ? { class: className } : null, el('dt', null, term), el('dd', null, value));
  }

  function label(name) {
    return state.labels && typeof state.labels[name] === 'string' ? state.labels[name] : '';
  }

  function fill(template, values) {
    return template.replace(/\{(\w+)\}/g, (match, name) => (name in values ? String(values[name]) : match));
  }

  function fixed(value, digits) {
    return typeof value === 'number' ? value.toFixed(digits) : String(value);
  }

  function lightIndexText(value) {
    return typeof value === 'number' && value > 0 && value < 0.0005 ? '< 0.001' : fixed(value, 3);
  }

  function plural(n, one, many) { return n + ' ' + (n === 1 ? one : many); }
  function listIds(ids) { return ids.length === 0 ? 'none' : ids.join(', '); }
  const n3 = (value) => Math.round(value * 1000) / 1000;

  function expandable(full) {
    const value = String(full);
    return el('details', { class: 'expand' },
      el('summary', null, el('span', { class: 'mono' }, value.slice(0, 12) + '…'), el('span', { class: 'sr-only' }, ' show the full value')),
      el('code', { class: 'full' }, value));
  }

  function technicalPlan(planId, executionKey) {
    return el('details', { class: 'technical-plan' }, el('summary', null, 'Technical reference'),
      el('p', null, 'PlanId ', el('code', null, planId)), el('p', null, 'Execution key ', el('code', null, executionKey)));
  }

  function table(caption, headers, rows) {
    return el('div', { class: 'table-wrap', role: 'region', 'aria-label': caption, tabindex: '0' },
      el('table', null, el('caption', null, caption),
        el('thead', null, el('tr', null, ...headers.map((header) => el('th', { scope: 'col' }, header)))),
        el('tbody', null, ...rows.map((cells) => el('tr', null, el('th', { scope: 'row' }, cells[0]),
          ...cells.slice(1).map((cell) => el('td', null, cell)))))));
  }

  // ---- lookups in the latest atomic composite payload -----------------------------------------------------------------

  function areaView(areaId) {
    return state.view && areaId ? state.view.areas.find((area) => area.areaId === areaId) || null : null;
  }
  function currentArea() { return areaView(state.areaId); }
  function treeIn(areaId, treeId) {
    const area = areaView(areaId);
    return area && treeId ? area.trees.find((tree) => tree.treeId === treeId) || null : null;
  }
  function livingIn(areaId, treeId) {
    const tree = treeIn(areaId, treeId);
    return Boolean(tree && tree.isLiving);
  }
  function areaOfTree(treeId) {
    return state.view ? (state.view.areas.find((area) => area.trees.some((tree) => tree.treeId === treeId)) || null) : null;
  }
  function sceneOf(areaId) { return (state.scenes && state.scenes[areaId]) || []; }
  function itemOf(areaId, treeId) { return sceneOf(areaId).find((item) => item.treeId === treeId) || null; }
  function areaName(areaId) { const area = areaView(areaId); return area ? area.label : String(areaId); }
  function areaShort(areaId) { const area = areaView(areaId); return area ? area.label : String(areaId); }
  function selectedId() { return state.areaId ? state.selected[state.areaId] || null : null; }
  function ordersIn(orders, areaId) { return orders.filter((order) => order.areaId === areaId); }

  function speciesOf(treeId) {
    if (state.ecology && state.ecology.speciesByTreeId && state.ecology.speciesByTreeId[treeId]) {
      return state.ecology.speciesByTreeId[treeId];
    }
    return 'sitka-spruce';
  }
  function speciesLabel(speciesId) { return SPECIES_NAME[speciesId] || speciesId; }

  function s4Area(areaId) {
    const readout = state.stage4;
    return readout && areaId ? readout.areas.find((area) => area.areaId === areaId) || null : null;
  }
  function s4Tree(areaId, treeId) {
    const area = s4Area(areaId);
    return area && treeId ? area.trees.find((tree) => tree.treeId === treeId) || null : null;
  }
  function ecoArea(areaId) {
    const eco = state.ecology;
    return eco && areaId ? eco.areas.find((area) => area.areaId === areaId) || null : null;
  }
  // Engine-authoritative species-aware prune status (prefers ecology.pruneStatusByTreeId, falls back to stage4).
  function pruneStatusOf(areaId, treeId) {
    if (state.ecology && state.ecology.pruneStatusByTreeId && state.ecology.pruneStatusByTreeId[treeId]) {
      return state.ecology.pruneStatusByTreeId[treeId];
    }
    const s4 = s4Tree(areaId, treeId);
    if (!s4) return null;
    return {
      speciesId: speciesOf(treeId),
      eligible: s4.pruneEligible,
      lifts: s4.pruningLifts,
      maxLifts: s4.maxPruningLifts,
      minIntervalYears: 5,
      nextTargetM: s4.nextPruneTargetM,
      nextEligibleYear: s4.nextPruneEligibleYear,
      refusalCode: s4.pruneRefusalCode,
      refusalReason: s4.pruneRefusalReason,
      planState: s4.prunePlanState,
    };
  }
  function pendingPrunes() { return state.stage4 ? state.stage4.pendingPrunes : []; }
  function approvedPrunes() { return state.stage4 ? state.stage4.approvedPrunes : []; }
  function isStage4() { return Boolean(state.stage4 && state.stage4.models.adultModelVersion === 1); }
  function isPhase2() { return Boolean(state.ecology && state.ecology.models && state.ecology.models.species === 1); }
  function readoutState() { return !state.stage4 ? 'unavailable' : isPhase2() ? 'phase2' : isStage4() ? 'stage4' : 'legacy'; }

  function compactCellsFor(areaId) {
    if (!state.ecology || !state.view) return new Map();
    const areaIdx = state.view.areas.findIndex((a) => a.areaId === areaId);
    if (areaIdx < 0) return new Map();
    const map = new Map();
    for (const row of state.ecology.cellsCompact) {
      if (row[0] === areaIdx) map.set(row[1] + ':' + row[2], row);
    }
    return map;
  }

  function lightPaint(index) {
    if (!(index > 0)) return null;
    const t = Math.pow(Math.min(1, index), LIGHT_EASE);
    const segment = t <= 0.5 ? 0 : 1;
    const f = segment === 0 ? t / 0.5 : (t - 0.5) / 0.5;
    const rgb = LIGHT_STOPS[segment].map((low, i) => Math.round(low + (LIGHT_STOPS[segment + 1][i] - low) * f));
    return { colour: '#' + rgb.map((v) => v.toString(16).padStart(2, '0')).join(''), opacity: opacityText(LIGHT_ALPHA_MAX * t) };
  }
  function opacityText(opacity) {
    const decimals = Math.max(4, 2 - Math.floor(Math.log10(opacity)));
    return decimals <= 100 ? opacity.toFixed(decimals) : opacity.toExponential(3);
  }
  function lightWord(index) {
    const word = LIGHT_WORDS.find(([below]) => index < below)[1];
    return index > 0 ? word : word + ' · no modelled light';
  }
  function groundCell(areaId, column, row) {
    const area = s4Area(areaId);
    return area ? area.ground.find((cell) => cell.column === column && cell.row === row) || null : null;
  }
  function pct(fraction) { return fixed(fraction * 100, 0) + '%'; }
  function pct1(fraction) { return fixed(fraction * 100, 1) + '%'; }
  function baseStatusText(item) {
    const template = label(STATUS_LABEL[item.status]);
    return template ? fill(template, { y: item.deathYear }) : String(item.status);
  }
  function statusText(item) { return item.isCrop ? label('CropRetained') : baseStatusText(item); }

  function referenceIn(areaId) {
    const id = state.selected[areaId];
    return id && livingIn(areaId, id) ? id : null;
  }
  function draftOf(areaId) {
    const draft = state.drafts[areaId];
    const reference = referenceIn(areaId);
    return draft && reference && draft.ref === reference ? draft.ids : [];
  }
  function draftTotal() {
    return state.view ? state.view.areas.reduce((sum, area) => sum + draftOf(area.areaId).length, 0) : 0;
  }
  function currentReport() {
    const comp = state.competition;
    if (!comp || state.lock || comp.areaId !== state.areaId) return null;
    return comp.report && comp.report.targetTreeId === referenceIn(state.areaId) ? comp.report : null;
  }

  function blockedReason(areaId, treeId) {
    const item = itemOf(areaId, treeId);
    if (!item) return 'Not in this area';
    if (item.isCrop) return 'Retained Crop — not offered for Fell';
    switch (item.status) {
      case 'PendingFell': return 'Already pending Fell';
      case 'ApprovedFell': return 'In the approved plan';
      case 'Felled': return 'Felled';
      case 'NaturalDead': return 'Dead';
      default: break;
    }
    const ps = pruneStatusOf(areaId, treeId);
    if (ps && ps.planState) return 'In the Prune plan';
    if (treeId === referenceIn(areaId)) return 'Reference tree';
    return null;
  }

  const SHORT_BLOCKED = {
    'Retained Crop — not offered for Fell': 'Retained Crop', 'Already pending Fell': 'Pending Fell',
    'In the approved plan': 'Approved plan', 'In the Prune plan': 'Prune plan', 'Reference tree': 'Reference', 'Not in this area': 'Other area'
  };
  function shortBlockedReason(why) { return SHORT_BLOCKED[why] || why; }

  // ---- talking to the local session process ---------------------------------------------------------------------------

  function currentFence() {
    const view = state.view;
    return { sessionId: view.sessionId, source: view.source, planStateRevision: view.planStateRevision };
  }

  async function call(path, body) {
    if (body !== undefined && state.lock) return { kind: 'locked' };
    const init = { method: body === undefined ? 'GET' : 'POST', cache: 'no-store', credentials: 'omit', redirect: 'error' };
    if (body !== undefined) {
      init.headers = { 'Content-Type': 'application/json' };
      init.body = JSON.stringify(body);
    }
    let response;
    try {
      response = await fetch(path, init);
    } catch (failure) {
      return { kind: 'transport', detail: String((failure && failure.message) || failure) };
    }
    let payload = null;
    try { payload = await response.json(); } catch { payload = null; }
    if (response.status === 200 && payload && payload.view) return { kind: 'ok', payload };
    if (response.status === 409 && payload && payload.view && payload.refusal) return { kind: 'refused', payload };
    if (payload && payload.error) {
      const unconfirmed = payload.error.code === 'OutcomeUnconfirmed';
      return { kind: unconfirmed ? 'unconfirmed' : 'guard', status: response.status, error: payload.error };
    }
    return { kind: 'transport', detail: 'HTTP ' + response.status };
  }

  function applyPayload(payload, keepOverlap) {
    state.view = payload.view;
    state.scenes = payload.scenes || {};
    state.stage4 = payload.stage4 || null;
    state.ecology = payload.ecology || null;
    if (payload.labels) state.labels = payload.labels;
    if (state.cellDetail && !state.cellDetail.missing &&
        !groundCell(state.cellDetail.areaId, state.cellDetail.column, state.cellDetail.row)) {
      state.cellDetail = null;
    }
    if (!areaView(state.areaId)) state.areaId = state.view.areas.length > 0 ? state.view.areas[0].areaId : null;
    for (const areaId of Object.keys(state.selected)) {
      if (!treeIn(areaId, state.selected[areaId])) delete state.selected[areaId];
    }
    if (!keepOverlap) state.overlap = null;
    const target = state.competitionTarget;
    if (target && !livingIn(target.areaId, target.treeId)) clearCompetition();
    const refresh = state.competitionTarget !== null && state.competitionTag !== competitionTag();
    if (refresh && !state.batch) state.competition = null;
    reconcileDrafts();
    renderAll();
    const confirmation = state.confirmation;
    if (confirmation && !byId('execute-dialog').open) state.confirmation = null;
    else if (confirmation && !confirmationStillShown(confirmation, state.view)) withdrawConfirmation(confirmation);
    if (refresh && !state.lock && !state.batch) refreshCompetition();
  }

  async function run(path, body, what, onCommitted, focusAfter) {
    if (!state.view || refusedByLock(what) || state.busy) return;
    state.busy = true;
    document.documentElement.classList.add('busy');
    const outcome = await call(path, body);
    state.busy = false;
    document.documentElement.classList.remove('busy');
    if (outcome.kind === 'ok') {
      clearAlert();
      state.lastWasReplay = Boolean(outcome.payload.execution && outcome.payload.execution.replayed);
      applyPayload(outcome.payload, false);
      if (onCommitted) onCommitted(outcome.payload);
      if (focusAfter) keepFocusOr(focusAfter);
    } else if (outcome.kind === 'refused') {
      applyPayload(outcome.payload, false);
      if (state.lock) showLockAlert('Refused: ' + what + ' (' + outcome.payload.refusal.code + '). Nothing changed.');
      else showRefusal(what, outcome.payload.refusal);
    } else if (outcome.kind === 'guard') {
      showGuardRefusal(what, outcome.status, outcome.error);
    } else if (outcome.kind === 'locked') {
      showLockAlert('Not sent: ' + what + '.');
    } else {
      lockCommands(label('MultiOutcomeUnconfirmed') || 'Outcome unconfirmed — re-read current state',
        what + ': no confirmed result was received. Do not assume that it happened, or that it did not.');
    }
  }

  // ---- alerts, announcements and the re-read lock ---------------------------------------------------------------------

  function clearAlert() { const box = byId('alert'); box.replaceChildren(); box.classList.remove('shown'); }
  function showAlert(parts, focusTarget) {
    const box = byId('alert');
    box.replaceChildren(...parts.filter((part) => part !== null));
    box.classList.add('shown');
    (focusTarget || box).focus();
  }
  function announce(message) {
    const region = byId('announce');
    region.textContent = '';
    window.setTimeout(() => { region.textContent = message; }, 50);
  }
  function rereadButton() {
    const button = el('button', { type: 'button' }, 'Re-read current state');
    button.addEventListener('click', () => reread(true));
    return button;
  }

  function recoveryFor(code) {
    switch (code) {
      case 'PlayerSourceStale':
      case 'PlayerPlanStateStale':
      case 'PlayerSessionMismatch':
        return 'The page now shows the current view. Review it, then try again.';
      case 'WorkReasonRequired':
        return 'Choose or write a reason for this tree, then add it again.';
      case 'WorkPlanAlreadyApproved':
        return 'Cancel the approved plan before editing the Work Plan.';
      case 'WorkPendingConsentRequired':
        return 'Choose whether to keep the orders or cancel all of them before the year advances.';
      case 'WorkWouldLeaveNoLivingStems':
        return label('MultiLastLivingRefused') + '. Cancel the approved plan and approve a smaller one.';
      case 'MultiAreaWrongArea':
        return 'That target belongs to another area. Switch to its area and try again.';
      case 'CropTreeRetained':
        return label('MultiCropFellRefused') + '.';
      case 'ContinuationNoLivingStems':
        return 'Every area has no living tree, so there is no year to advance.';
      default:
        return 'Review the current view, then choose again.';
    }
  }

  function showRefusal(what, refusal) {
    showAlert([
      el('p', { class: 'alert-title' }, 'Refused: ' + what),
      el('p', null, el('code', null, refusal.code), ' — ' + refusal.message),
      el('p', null, label('MultiNothingChanged') + '. Current ecological year ' + state.view.source.ecologicalYear + '.'),
      el('p', null, recoveryFor(refusal.code)),
    ]);
  }

  function showGuardRefusal(what, status, error) {
    showAlert([
      el('p', { class: 'alert-title' }, 'Refused by the local page server: ' + what),
      el('p', null, el('code', null, error.code), ' (HTTP ' + status + ') — ' + error.message),
      el('p', null, 'The request did not reach the engine.'),
      rereadButton(),
    ]);
  }

  function lockCommands(title, detail) {
    state.lock = { title, detail };
    state.lockCount += 1;
    renderAll();
    showLockAlert(null);
  }

  function showLockAlert(extra) {
    const lock = state.lock;
    if (!lock) return;
    showAlert([
      el('p', { class: 'alert-title' }, lock.title),
      el('p', null, lock.detail),
      extra ? el('p', null, extra) : null,
      el('p', null, 'Every command stays unavailable until a re-read of the current state succeeds.'),
      rereadButton(),
    ]);
  }

  function refusedByLock(what) {
    if (!state.lock) return false;
    showLockAlert('Not sent: ' + what + '.');
    return true;
  }

  function addAvailability() {
    const areaId = state.areaId;
    if (state.lock) return { ok: false, why: 'Re-read the current state before adding.' };
    if (state.view && state.view.approved) return { ok: false, why: 'The plan is approved, so editing is locked. Cancel the approved plan to change it.' };
    if (!currentReport()) return { ok: false, why: 'Waiting for the current competition reading.' };
    const eligible = draftOf(areaId).filter((id) => !blockedReason(areaId, id));
    if (eligible.length === 0) return { ok: false, why: 'Tick one or more competitors to add.' };
    return { ok: true, why: null, count: eligible.length };
  }

  function renderCommandGate() {
    const blocked = state.lock !== null || state.batch !== null;
    for (const id of COMMAND_IDS) byId(id).disabled = blocked;
    if (readoutState() === 'unavailable') for (const id of ['approve', 'execute', 'execute-confirm']) byId(id).disabled = true;
    for (const button of document.querySelectorAll('[data-remove]')) button.disabled = blocked;
    for (const button of document.querySelectorAll('[data-prune-remove]')) button.disabled = blocked;
    if (!blocked) renderPruneGate();
    byId('add-button').disabled = state.batch !== null || !addAvailability().ok;
    byId('draft-clear').disabled = state.batch !== null || draftOf(state.areaId).length === 0;
    byId('reread').disabled = state.batch !== null;
    for (const button of document.querySelectorAll('[data-area]')) {
      if (button.classList.contains('area-tab')) button.disabled = state.batch !== null;
    }
  }

  function keepFocusOr(id) {
    const active = document.activeElement;
    const usable = active && active !== document.body && active.isConnected && !active.closest('[hidden]') && !active.disabled;
    if (!usable) byId(id).focus();
  }

  // ---- Execute confirmation -------------------------------------------------------------------------------------------

  function frozen(value) {
    if (value && typeof value === 'object') {
      for (const inner of Object.values(value)) frozen(inner);
      Object.freeze(value);
    }
    return value;
  }

  function confirmationOf(view) {
    const plan = view.approved;
    return frozen({
      sessionId: view.sessionId,
      source: Object.assign({}, view.source),
      planStateRevision: view.planStateRevision,
      planId: plan.planId,
      executionKey: plan.executionKey,
      year: view.source.ecologicalYear,
      orders: plan.orders.map((order) => ({ areaId: order.areaId, treeId: order.treeId, reason: order.reason })),
      prunes: approvedPrunes().map((order) => ({ areaId: order.areaId, treeId: order.treeId,
        targetCrownBaseHeightM: order.targetCrownBaseHeightM })),
    });
  }

  function sameSource(a, b) {
    for (const name of new Set(Object.keys(a).concat(Object.keys(b)))) if (a[name] !== b[name]) return false;
    return true;
  }

  function confirmationStillShown(confirmation, view) {
    const plan = view.approved;
    return view.sessionId === confirmation.sessionId && view.planStateRevision === confirmation.planStateRevision &&
      sameSource(view.source, confirmation.source) && plan !== null && plan.planId === confirmation.planId &&
      plan.executionKey === confirmation.executionKey && plan.orders.length === confirmation.orders.length &&
      plan.orders.every((order, index) => order.areaId === confirmation.orders[index].areaId &&
        order.treeId === confirmation.orders[index].treeId && order.reason === confirmation.orders[index].reason) &&
      samePrunes(confirmation.prunes);
  }

  function samePrunes(frozenPrunes) {
    if (!state.stage4) return false;
    const now = approvedPrunes();
    return now.length === frozenPrunes.length && now.every((order, index) => order.areaId === frozenPrunes[index].areaId &&
      order.treeId === frozenPrunes[index].treeId && order.targetCrownBaseHeightM === frozenPrunes[index].targetCrownBaseHeightM);
  }

  function withdrawConfirmation(confirmation) {
    state.confirmation = null;
    const dialog = byId('execute-dialog');
    if (dialog.open) dialog.close();
    const unpaired = readoutState() === 'unavailable';
    lockCommands(unpaired ? 'Confirmation withdrawn — the read-out of this view is unavailable' : 'Confirmation withdrawn — the approved plan changed',
      'The Execute confirmation showed ' + (confirmation.orders.concat(confirmation.prunes).map((order) => order.treeId).join(', ') || 'no orders') +
      '. ' + (unpaired ? 'The view that has just arrived came without its engine read-out, so its Prune orders cannot be checked'
        : 'The view that has just arrived no longer shows that plan with that fence') + ', so nothing was sent. Review the plan now ' +
      'shown, then choose Re-read; after a successful re-read you can choose Execute again.');
  }

  // ---- rendering ------------------------------------------------------------------------------------------------------

  function renderAll() {
    if (!state.view || !state.labels) return;
    const active = document.activeElement;
    const focusKey = active && active.getAttribute ? active.getAttribute('data-focus-key') : null;
    renderStatus();
    renderModeAndOverlayControls();
    renderOverlayLegend();
    renderAreaSwitch();
    renderMap();
    renderOverlap();
    renderCellInspector();
    renderInspector();
    renderCompetition();
    renderBatchResult();
    renderPlan();
    renderReceipt();
    renderTable();
    renderHistory();
    renderDetails();
    renderPaneButtons();
    renderCommandGate();
    renderMapControls();
    if (focusKey) {
      const again = document.querySelector('[data-focus-key="' + CSS.escape(focusKey) + '"]');
      if (again && again !== document.activeElement) again.focus();
    }
  }

  function experimentBadgeText() {
    if (state.ecology && state.ecology.experimentBadge) return state.ecology.experimentBadge;
    return state.view && state.view.experiment.experimentId === 'mad-ma1-mixed-demo-v1'
      ? 'Synthetic MA-1-MIXED demo — Area A Sitka; Area B ⅓ Sitka / ⅓ Oak / ⅓ Beech (not a surveyed Irish forecast)'
      : 'Synthetic MA-1 (all Sitka)';
  }

  function renderStatus() {
    const view = state.view;
    const badgeText = experimentBadgeText();
    document.title = 'Two-area Stand Map — ' + badgeText;
    byId('page-title').textContent = 'Two-area Stand Map';
    const badgeEl = byId('experiment-badge');
    badgeEl.textContent = badgeText;
    badgeEl.setAttribute('title', badgeText);
    byId('year-value').textContent = String(view.source.ecologicalYear);
    byId('advance-label').textContent = label('MultiAdvanceYear');
    byId('area-cards').replaceChildren(...view.areas.map((area) => el('div', {
      class: 'card area-card' + (area.areaId === state.areaId ? ' current' : '') },
      el('span', { class: 'dot', 'aria-hidden': 'true' }),
      el('div', { class: 'area-card-main' },
        el('p', { class: 'eyebrow' }, area.label + ' · living stand'),
        el('p', { class: 'area-card-count' }, el('strong', null, String(area.stand.livingCount)), ' ', el('span', null, 'living trees'))),
      el('div', { class: 'area-card-side' },
        el('strong', null, fixed(area.stand.basalAreaM2PerHa, 1)),
        el('span', null, 'm²/ha BA · ' + fixed(area.areaHa, 4) + ' ha')),
      areaReadouts(area.areaId))));
    const approved = view.approved ? view.approved.orders.length : 0;
    const approvedP = approvedPrunes().length;
    const pendingP = pendingPrunes().length;
    const drafts = draftTotal();
    byId('plan-summary').textContent = approved + approvedP > 0
      ? [approved > 0 ? plural(approved, 'approved Fell', 'approved Fell') : '', approvedP > 0 ? approvedP + ' approved Prune' : ''].filter(Boolean).join(' · ')
      : view.pending.length + pendingP > 0
        ? [view.pending.length > 0 ? view.pending.length + ' pending Fell' : '', pendingP > 0 ? pendingP + ' pending Prune' : ''].filter(Boolean).join(' · ')
        : 'No queued work';
    byId('plan-summary-sub').textContent = [approved + approvedP > 0 ? 'nothing done yet' : view.pending.length + pendingP > 0 ? 'not approved' : '',
      drafts > 0 ? plural(drafts, 'draft choice', 'draft choices') : ''].filter(Boolean).join(' · ');
    byId('notice-line').textContent = badgeText + ' · Site unknown · Modelled light · not calibrated PAR';
    const readout = readoutState();
    byId('disclosure').textContent = readout === 'phase2' || readout === 'stage4'
      ? (view.experiment.experimentId === 'mad-ma1-mixed-demo-v1' ? view.experiment.disclosure : label('MultiDisclosureStage4'))
      : readout === 'legacy' ? label('MultiDisclosure') :
      'Model read-out unavailable for this view, so the model statements are not shown. Choose Re-read. ' +
      [label('MultiSyntheticStatement'), label('MultiNotComparable'), label('MultiSiteUnknown'), label('MultiNoTransfer')].join('. ') + '.';
    const chips = [];
    const chipList = (readout === 'phase2' || readout === 'stage4')
      ? CHIPS.map((pair) => pair[0] === 'MultiChipEdgeExcluded' ? ['MultiChipConnectedWoodland', 'MultiConnectedWoodland'] : pair)
      : readout === 'legacy' ? CHIPS : CHIPS.filter((pair) => pair[0] !== 'MultiChipEdgeExcluded');
    chipList.forEach(([chip, full], index) => {
      if (index > 0) chips.push(' · ');
      chips.push(el('abbr', { title: label(full) }, label(chip)));
    });
    byId('flags').replaceChildren(...chips);
    byId('session-note').replaceChildren(el('abbr', { title: label('MultiSessionOnly') }, label('MultiChipSessionOnly')));
  }

  function areaReadouts(areaId) {
    const s4 = s4Area(areaId);
    const readout = state.stage4;
    if (!s4 || !readout) {
      return el('p', { class: 'area-readouts' }, 'Engine read-out unavailable for this view — choose Re-read.');
    }
    const c = s4.canopy;
    const m = readout.models;
    const ea = ecoArea(areaId);
    const em = state.ecology ? state.ecology.models : null;
    const tuple = em ? [em.adult, em.mortality, em.crown, em.regeneration, em.species].join('/')
      : [m.adultModelVersion, m.mortalityModelVersion, m.crownModelVersion].join('/');
    let speciesText = null;
    if (ea && ea.liveAdultsBySpecies) {
      const s = ea.liveAdultsBySpecies['sitka-spruce'] || 0;
      const o = ea.liveAdultsBySpecies['sessile-oak'] || 0;
      const b = ea.liveAdultsBySpecies['beech'] || 0;
      speciesText = (o > 0 || b > 0) ? (s + ' S · ' + o + ' O · ' + b + ' B') : (s + ' Sitka');
    }
    return el('dl', { class: 'area-readouts' },
      el('div', null, el('dt', null, 'Cover'), el('dd', null, pct(c.geometricCoverFraction), ' ',
        el('small', null, '· open ' + fixed(c.geometricUncoveredM2, 0) + ' m²'))),
      el('div', null, el('dt', null, 'RD'), el('dd', null, fixed(c.relativeDensity, 2))),
      el('div', null, el('dt', null, 'Deaths'), el('dd', null, c.annualNaturalDeaths + '/yr', ' ',
        el('small', null, '· ' + plural(c.fallenDeadwoodCount, 'log', 'logs')))),
      speciesText ? el('div', null, el('dt', null, 'Species'), el('dd', null, speciesText)) : null,
      el('div', { class: 'model-badge-extra' }, el('dt', { class: 'sr-only' }, 'Model'), el('dd', { class: 'model-badge' },
        el('abbr', { title: 'Model tuple ' + tuple + ' · ' + m.lightModelVersion + ' (' + fixed(m.lightCellM, 1) + ' m cells)' }, 'Model ' + tuple),
        el('abbr', { title: m.notCalibratedCaveat }, 'Light: not PAR'))));
  }

  function renderModeAndOverlayControls() {
    const isCell = state.mapMode === 'cell';
    byId('mode-tree').setAttribute('aria-pressed', String(!isCell));
    byId('mode-cell').setAttribute('aria-pressed', String(isCell));
    byId('map-pane').classList.toggle('cell-mode', isCell);
    const sel = byId('overlay-select');
    if (sel.value !== state.overlay) sel.value = state.overlay;
    // Explicit swap of the right-hand inspector body: Inspect Cell hides #tree-panel and shows #cell-panel (never stacks).
    byId('cell-panel').hidden = !isCell;
    byId('tree-panel').hidden = isCell;
  }

  function areaThumb(area) {
    const xs = area.polygon.map((p) => p.xm);
    const zs = area.polygon.map((p) => -p.zm);
    const minX = Math.min(...xs), minZ = Math.min(...zs);
    const w = Math.max(...xs) - minX || 1, h = Math.max(...zs) - minZ || 1;
    const pad = Math.max(w, h) * .06;
    const thumb = svg('svg', { class: 'area-thumb', viewBox: [minX - pad, minZ - pad, w + 2 * pad, h + 2 * pad].map(n3).join(' '),
      'aria-hidden': 'true', focusable: 'false' });
    thumb.append(svg('polygon', { points: area.polygon.map((p) => n3(p.xm) + ',' + n3(-p.zm)).join(' ') }));
    return thumb;
  }

  function renderAreaSwitch() {
    const view = state.view;
    byId('area-switch').replaceChildren(...view.areas.map((area) => {
      const inPlan = ordersIn(view.pending, area.areaId).length + (view.approved ? ordersIn(view.approved.orders, area.areaId).length : 0) +
        ordersIn(pendingPrunes(), area.areaId).length + ordersIn(approvedPrunes(), area.areaId).length;
      return el('button', { type: 'button', class: 'area-tab', 'data-area': area.areaId, 'data-focus-key': 'area:' + area.areaId,
        'aria-pressed': String(area.areaId === state.areaId) },
        areaThumb(area),
        el('span', { class: 'area-name' }, area.label),
        inPlan > 0 ? el('span', { class: 'area-plan' }, inPlan + ' in plan') : null,
        el('span', { class: 'sr-only' }, ' · ' + area.stand.livingCount + ' living · ' + fixed(area.areaHa, 4) + ' ha'));
    }));
  }

  // ---- the map: 1 SVG user unit = 1 metre; U east, V south ------------------------------------------------------------

  function fullBox(area) {
    const side = area.extent.sideM;
    return { x: -3.4, y: -2.2, w: side + 6.0, h: side + 5.4 };
  }

  function mapFrame(area) {
    const full = fullBox(area);
    const pane = byId('map-pane');
    const dock = byId('map-dock');
    const W = Math.max(pane.clientWidth, 1);
    const H = Math.max(pane.clientHeight, 1);
    const dockRight = dock.offsetLeft + dock.offsetWidth + 10;
    const dockBottom = dock.offsetTop + dock.offsetHeight + 10;
    const besideScale = Math.min((W - dockRight) / full.w, H / full.h);
    const belowScale = Math.min(W / full.w, (H - dockBottom) / full.h);
    const beside = besideScale >= belowScale;
    return { full, W, H, fit: Math.max(beside ? besideScale : belowScale, 1e-3),
      reserveLeft: beside ? dockRight : 0, reserveTop: beside ? 0 : dockBottom };
  }

  function viewAxis(centre, usable, reserve, start, length) {
    if (usable >= length - 1e-9) return { pos: start - reserve - (usable - length) / 2, back: false, forward: false };
    const lo = start - reserve;
    const hi = start + length - usable - reserve;
    const pos = Math.min(hi, Math.max(lo, centre - usable / 2 - reserve));
    return { pos, back: pos > lo + 1e-6, forward: pos < hi - 1e-6 };
  }

  function viewBox(area) {
    const frame = mapFrame(area);
    const full = frame.full;
    const scale = frame.fit * state.zoom;
    const w = frame.W / scale, h = frame.H / scale;
    const rl = frame.reserveLeft / scale, rt = frame.reserveTop / scale;
    const uw = w - rl, uh = h - rt;
    const centre = state.centre || { u: full.x + full.w / 2, v: full.y + full.h / 2 };
    const ax = viewAxis(centre.u, uw, rl, full.x, full.w);
    const ay = viewAxis(centre.v, uh, rt, full.y, full.h);
    return { x: ax.pos, y: ay.pos, w, h, scale, uw, uh, rl, rt,
      west: ax.back, east: ax.forward, north: ay.back, south: ay.forward };
  }

  function usableCentre(box) { return { u: box.x + box.rl + box.uw / 2, v: box.y + box.rt + box.uh / 2 }; }

  const FOCUS_SCALE = 2;
  const FOCUS_GAP_PX = 0;
  const FOCUS_TOUCH = .8;
  function spreadFocus(areaId, report, mpp, box) {
    const ids = [report.targetTreeId, ...report.topFive.map((n) => n.treeId)];
    const base = ids.map((id) => itemOf(areaId, id)).filter(Boolean);
    if (!base.length || base[0].treeId !== report.targetTreeId) return new Map();
    const lo = { u: box.x + box.rl + 34 * mpp, v: box.y + box.rt + 34 * mpp };
    const hi = { u: box.x + box.w - 34 * mpp, v: box.y + box.h - 48 * mpp };
    const nodes = base.map((item) => ({ ...item, radiusM: item.radiusM * FOCUS_SCALE }));
    const within = (x, a, b) => (a > b ? (a + b) / 2 : Math.min(Math.max(x, a), b));
    for (let round = 0; round < 8; round++) {
      separate(nodes, FOCUS_GAP_PX * mpp);
      let held = false;
      for (const n of nodes.slice(1)) {
        const u = within(n.u, lo.u + n.radiusM, hi.u - n.radiusM), v = within(n.v, lo.v + n.radiusM, hi.v - n.radiusM);
        if (u !== n.u || v !== n.v) { n.u = u; n.v = v; held = true; }
      }
      if (!held) break;
    }
    return new Map(nodes.map((node) => [node.treeId, node]));
  }

  function separate(nodes, pad) {
    const anchor = nodes[0];
    for (let pass = 0; pass < 80; pass++) {
      let moved = false;
      for (let i = 0; i < nodes.length; i++) for (let j = i + 1; j < nodes.length; j++) {
        const a = nodes[i], b = nodes[j];
        let dx = b.u - a.u, dy = b.v - a.v, d = Math.hypot(dx, dy);
        if (d < 1e-6) { dx = Math.cos(j * 2.399); dy = Math.sin(j * 2.399); d = 0; } else { dx /= d; dy /= d; }
        const need = (a.radiusM + b.radiusM) * FOCUS_TOUCH + pad - d;
        if (need <= 0) continue;
        moved = true;
        const shareA = a === anchor ? 0 : b === anchor ? 1 : .5;
        a.u -= dx * need * shareA; a.v -= dy * need * shareA;
        b.u += dx * need * (1 - shareA); b.v += dy * need * (1 - shareA);
      }
      if (!moved) break;
    }
    return nodes;
  }

  const crownCache = new Map();

  function canopyHash(a, b, salt) {
    let h = Math.imul(a ^ 0x9e3779b1, 0x85ebca77) ^ Math.imul(b ^ salt, 0xc2b2ae3d);
    h = Math.imul(h ^ (h >>> 15), 0x2c1b3c6d);
    h = Math.imul(h ^ (h >>> 12), 0x297a2d39);
    return (h ^ (h >>> 15)) >>> 0;
  }
  function canopySeed(id) {
    let seed = 7;
    for (const ch of String(id || 'legend')) seed = canopyHash(seed, ch.charCodeAt(0), 7);
    return seed;
  }
  function canopyRandom(seed, channel) { return (canopyHash(seed, channel, 2) & 0xffffff) / 16777216; }

  function groundNoise(x, y, cells, seed) {
    const fx = x * cells, fy = y * cells;
    const ix = Math.floor(fx), iy = Math.floor(fy);
    const smooth = (t) => t * t * (3 - 2 * t);
    const tx = smooth(fx - ix), ty = smooth(fy - iy);
    const at = (a, b) => (canopyHash(a % cells, b % cells, seed) & 0xffffff) / 16777216;
    const lerp = (a, b, t) => a + (b - a) * t;
    return lerp(lerp(at(ix, iy), at(ix + 1, iy), tx), lerp(at(ix, iy + 1), at(ix + 1, iy + 1), tx), ty);
  }

  function initFloorDefs() {
    const defs = byId('map').querySelector('defs');
    if (byId('slate-ground')) return;
    [12.8, 9.7].forEach((period, index) => {
      const tile = svg('pattern', { id: 'slate-' + index, patternUnits: 'userSpaceOnUse', width: period, height: period });
      const seed = index === 0 ? 0x51ed270b : 0x9e3779b9;
      const size = 24, cell = period / size;
      for (let y = 0; y < size; y++) for (let x = 0; x < size; x++) {
        const n = 255 * (.65 * groundNoise(x / size, y / size, 14, seed) +
          .35 * groundNoise(x / size, y / size, 5, seed ^ 0xabcd)) - 128;
        const colour = SLATE.map((v) => Math.round(v + n / 14));
        tile.append(svg('rect', { x: n3(x * cell), y: n3(y * cell), width: n3(cell + .005), height: n3(cell + .005),
          fill: 'rgb(' + colour.join(',') + ')' }));
      }
      defs.append(tile);
    });
    const floor = svg('pattern', { id: 'slate-ground', patternUnits: 'userSpaceOnUse', width: 124.16, height: 124.16 });
    floor.append(svg('rect', { width: 124.16, height: 124.16, fill: 'url(#slate-0)' }),
      svg('rect', { width: 124.16, height: 124.16, fill: 'url(#slate-1)', opacity: .4 }));
    defs.append(floor);
  }

  // Strictly circular crown geometry (spec §5.6): x = u - r, y = v - r, width = height = 2 * r, where r = item.radiusM.
  // Zero elliptical stretch; zero DBH->radius maths in JS. Deterministic seeded rotation around (u, v) is allowed.
  function crownDecoration(item, sp) {
    const u = item.u, v = item.v, r = item.radiusM;
    if (item.status === 'Felled') {
      return [
        svg('circle', { class: 'stump-disc deco', cx: n3(u), cy: n3(v), r: n3(r) }),
        svg('circle', { class: 'stump-ring deco', cx: n3(u), cy: n3(v), r: n3(r * .65) }),
        svg('circle', { class: 'stump-ring deco', cx: n3(u), cy: n3(v), r: n3(r * .32) }),
      ];
    }
    const seed = canopySeed(item.treeId);
    const turn = Math.round(canopyRandom(seed, 21) * 360);
    const dead = item.status === 'NaturalDead';
    const symbolId = dead ? 'ccf-standing-dead' : (SPECIES_SYMBOL[sp] || 'ccf-sitka');
    const diam = n3(2 * r);
    return [
      svg('circle', { class: 'deco', cx: n3(u + r * .16), cy: n3(v + r * .18), r: n3(r * 1.08),
        fill: 'url(#canopy-shadow)', opacity: dead ? .4 : .85 }),
      svg('use', {
        class: 'deco crown-art',
        href: '#' + symbolId,
        x: n3(u - r),
        y: n3(v - r),
        width: diam,
        height: diam,
        transform: 'rotate(' + turn + ' ' + n3(u) + ' ' + n3(v) + ')',
        'data-radius-m': n3(r),
        'data-species': sp,
      }),
    ];
  }

  function renderMap() {
    const area = currentArea();
    if (!area) return;
    const side = area.extent.sideM;
    const box = viewBox(area);
    const mpp = 1 / box.scale;
    byId('map').setAttribute('viewBox', [box.x, box.y, box.w, box.h].map((n) => n.toFixed(4)).join(' '));
    setAttributes(byId('ground'), { x: 0, y: 0, width: side, height: side });
    const toU = (x) => x - area.extent.minXM;
    const toV = (z) => area.extent.minZM + side - z;
    const outline = area.polygon.map((p) => toU(p.xm) + ',' + toV(p.zm)).join(' ');
    byId('area-outline').setAttribute('points', outline);
    byId('area-clip-shape').setAttribute('points', outline);
    drawGrid(area, mpp, box.scale);

    const inTreeMode = state.mapMode === 'tree';
    const report = inTreeMode ? currentReport() : null;
    const reference = referenceIn(area.areaId);
    const draft = new Set(draftOf(area.areaId));
    const focusIds = report ? new Set([report.targetTreeId, ...report.topFive.map((n) => n.treeId)]) : null;
    const crowns = [];
    const focusCrowns = [];
    const marks = [];
    const ranks = [];
    const spread = focusIds ? spreadFocus(area.areaId, report, mpp, box) : new Map();
    const items = new Map(sceneOf(area.areaId).map((item) => [item.treeId, spread.get(item.treeId) || item]));
    for (const [treeId, shown] of spread) {
      const real = items.get(treeId) === shown ? itemOf(area.areaId, treeId) : null;
      if (real && Math.hypot(shown.u - real.u, shown.v - real.v) > 2 * mpp) {
        marks.push(svg('path', { class: 'spread-leader', d: 'M ' + n3(real.u) + ' ' + n3(real.v) + ' L ' + n3(shown.u) + ' ' + n3(shown.v) }),
          svg('circle', { class: 'spread-origin', cx: n3(real.u), cy: n3(real.v), r: n3(2.5 * mpp) }));
      }
    }
    for (const raw of sceneOf(area.areaId).slice().sort((a, b) => a.paintOrder - b.paintOrder)) {
      const item = items.get(raw.treeId);
      const sp = speciesOf(item.treeId);
      const focused = item !== raw;
      const dimmed = focusIds && !focused;
      const cacheKey = area.areaId + '|' + item.treeId;
      const drawKey = [item.status, sp, n3(item.u), n3(item.v), n3(item.radiusM)].join('|');
      let cached = crownCache.get(cacheKey);
      if (!cached || cached.drawKey !== drawKey) {
        const group = svg('g', { 'data-tree-id': item.treeId, 'data-species': sp });
        // Transparent true-radius hit disc preserved underneath crown art
        group.append(svg('circle', { class: 'crown', cx: n3(item.u), cy: n3(item.v), r: n3(item.radiusM) }));
        group.append(...crownDecoration(item, sp));
        cached = { drawKey, node: group };
        crownCache.set(cacheKey, cached);
      }
      const className = 'tree s-' + item.status + (dimmed ? ' dimmed' : '');
      if (cached.node.getAttribute('class') !== className) cached.node.setAttribute('class', className);
      (focused ? focusCrowns : crowns).push(cached.node);
      marks.push(...statusMarks(item, mpp), ...pruneMarks(area.areaId, item, mpp));
    }
    if (report) {
      const taken = [];
      const focus = [...spread.values()];
      for (const f of focus) {
        if (f.isCrop) taken.push(tileCentre(f, TILE.crop, mpp));
        if (f.status === 'PendingFell' || f.status === 'ApprovedFell') taken.push(tileCentre(f, TILE.status, mpp));
      }
      const target = items.get(report.targetTreeId);
      if (target) {
        taken.push(tileCentre(target, TILE.reference, mpp));
        for (const dx of [-30, -10, 10, 30]) taken.push([target.u + dx * mpp, target.v - target.radiusM - 22 * mpp]);
      }
      const free = (x, y) => taken.every(([tx, ty]) => Math.hypot(tx - x, ty - y) >= 34 * mpp) &&
        focus.every((f) => Math.hypot(f.u - x, f.v - y) >= f.radiusM * .55);
      report.topFive.forEach((entry, rank) => {
        const item = items.get(entry.treeId);
        if (!item) return;
        const chosen = draft.has(entry.treeId);
        const fits = (a) => { const [x, y] = tileCentre(item, a, mpp); return free(x, y) && (!chosen || free(x - 23 * mpp, y)); };
        const angle = RANK_ANGLES.find(fits) ?? TILE.rank;
        const [x, y] = tileCentre(item, angle, mpp);
        taken.push([x, y]);
        if (chosen) taken.push([x - 23 * mpp, y]);
        marks.push(underline(item, chosen ? 'ring-draft' : 'ring-competitor', mpp, 0));
        ranks.push({ treeId: entry.treeId, rank: rank + 1, x, y, chosen });
      });
    }
    crowns.push(...focusCrowns);
    const host = byId('crowns');
    const unchanged = host.childNodes.length === crowns.length && crowns.every((node, index) => host.childNodes[index] === node);
    if (!unchanged) host.replaceChildren(...crowns);

    renderOverlayCells(area.areaId, mpp);
    renderClearCells(area.areaId);
    renderSpotPatches(area.areaId);
    renderDeadwoodLayer(area.areaId, mpp);
    renderPlantedLayer(area.areaId, mpp);
    renderGroundMark(area.areaId);
    byId('marks').replaceChildren(...marks);
    renderRankLayer(ranks, box);

    const chosen = inTreeMode && selectedId() ? items.get(selectedId()) : null;
    const lit = chosen && !state.lock ? (chosen.treeId === reference ? referenceMarks(chosen, mpp, true) : inspectMarks(chosen, mpp, true)) : [];
    byId('selection').replaceChildren(...lit);

    byId('map-desc').textContent = 'North-up map of ' + areaName(area.areaId) + ', ' + area.areaM2Exact + ' square metres. ' +
      'Ecological year ' + state.view.source.ecologicalYear + '. Living trees ' + area.stand.livingCount + '.';
    byId('map-subtitle').textContent = area.label + ' · ' + fixed(area.areaHa, 4) + ' ha';
    byId('map-note').textContent = areaName(area.areaId) + ' · ' + (state.zoom === 1 ? 'Fit whole area' :
      fixed(state.zoom, 2) + '× Fit · view ' + fixed(box.uw, 1) + ' × ' + fixed(box.uh, 1) + ' m');
    byId('zoom-state').textContent = state.zoom === 1 ? 'Fit' : fixed(state.zoom, 2) + '×';
    renderScale(box.scale);
    renderMapStatus(reference, draft.size);
  }

  function renderOverlayCells(areaId, mpp) {
    const host = byId('overlay-cells');
    const s4 = s4Area(areaId);
    if (state.overlay === 'off' || !s4) { host.replaceChildren(); return; }
    const nodes = [];
    if (state.overlay === 'light') {
      for (const cell of s4.ground) {
        const paint = lightPaint(cell.lightIndex);
        if (!paint) continue;
        nodes.push(svg('polygon', { class: 'light-cell', 'data-cell': cell.column + ':' + cell.row, 'data-light': cell.lightIndex,
          fill: paint.colour, 'fill-opacity': paint.opacity, points: cell.polygon.map((p) => n3(p.u) + ',' + n3(p.v)).join(' ') }));
      }
    } else if (state.overlay === 'undergrowth') {
      const compact = compactCellsFor(areaId);
      for (const cell of s4.ground) {
        const row = compact.get(cell.column + ':' + cell.row);
        if (!row) continue;
        const bramble = row[4], bracken = row[5];
        const pts = cell.polygon.map((p) => n3(p.u) + ',' + n3(p.v)).join(' ');
        if (bramble > 0) {
          nodes.push(svg('polygon', { class: 'undergrowth-cell', 'data-kind': 'bramble', fill: 'url(#ccf-bramble-hatch)',
            'fill-opacity': opacityText(Math.min(0.95, 0.2 + 0.75 * bramble)), points: pts }));
        }
        if (bracken > 0) {
          nodes.push(svg('polygon', { class: 'undergrowth-cell', 'data-kind': 'bracken', fill: 'url(#ccf-bracken-hatch)',
            'fill-opacity': opacityText(Math.min(0.95, 0.2 + 0.75 * bracken)), points: pts }));
        }
      }
    } else if (state.overlay === 'seedlings') {
      // Cell-level presence glyph only: never fabricated individual stem dots; never treats Density as a literal stem count.
      const compact = compactCellsFor(areaId);
      for (const cell of s4.ground) {
        const row = compact.get(cell.column + ':' + cell.row);
        if (!row || row[7] <= 0) continue;
        const mask = row[6];
        const tags = [];
        if (mask & 1) tags.push('S');
        if (mask & 2) tags.push('O');
        if (mask & 4) tags.push('B');
        const gSize = Math.min(1.6, Math.max(0.9, 18 * mpp));
        nodes.push(svg('polygon', { fill: '#355c2b', 'fill-opacity': '0.32', stroke: '#7bc45c', 'stroke-opacity': '0.55',
          'stroke-width': n3(1 * mpp), points: cell.polygon.map((p) => n3(p.u) + ',' + n3(p.v)).join(' ') }));
        nodes.push(svg('use', { href: '#ccf-natural-seedlings', x: n3(cell.centreU - gSize / 2), y: n3(cell.centreV - gSize * 0.6),
          width: n3(gSize), height: n3(gSize) }));
        if (tags.length > 0) {
          nodes.push(svg('text', { class: 'seedling-tag', x: n3(cell.centreU), y: n3(cell.centreV + gSize * 0.45),
            'font-size': n3(Math.min(0.75, 9 * mpp)) }, tags.join('·')));
        }
      }
    }
    host.replaceChildren(...nodes);
  }

  // Rule 1: ClearUndergrowth highlights the whole 2.5 m cell (sparing seedlings inside), never a tree-crown badge.
  function renderClearCells(areaId) {
    const host = byId('clear-cells');
    if (!state.ecology || !s4Area(areaId)) { host.replaceChildren(); return; }
    const nodes = [];
    const drawClear = (orders, cls) => {
      for (const o of orders) {
        if (o.areaId !== areaId) continue;
        const cell = groundCell(areaId, o.column, o.row);
        if (!cell) continue;
        nodes.push(svg('polygon', { class: cls, 'data-clear-cell': o.column + ':' + o.row,
          points: cell.polygon.map((p) => n3(p.u) + ',' + n3(p.v)).join(' ') }));
      }
    };
    drawClear(state.ecology.pendingClear || [], 'clear-cell-pending');
    drawClear(state.ecology.approvedClear || [], 'clear-cell-approved');
    host.replaceChildren(...nodes);
  }

  // Rule 2: SpotClear (~1 m2): circular dashed ring drawn to exact world scale r = patch.radiusM (~0.564 m)
  // over bare soil/twigs with NO seedling inside, rendered in its own layer BENEATH planted PJ saplings.
  function renderSpotPatches(areaId) {
    const host = byId('spot-patches');
    if (!state.ecology) { host.replaceChildren(); return; }
    const nodes = [];
    for (const p of (state.ecology.patches || [])) {
      if (p.areaId !== areaId) continue;
      const r = p.radiusM;
      nodes.push(svg('use', { href: '#ccf-spot-clear', 'data-patch-id': p.patchId,
        x: n3(p.u - r), y: n3(p.v - r), width: n3(2 * r), height: n3(2 * r) }));
    }
    host.replaceChildren(...nodes);
  }

  function renderDeadwoodLayer(areaId, mpp) {
    const host = byId('deadwood');
    const s4 = s4Area(areaId);
    if (!s4) { host.replaceChildren(); return; }
    const w = 26 * mpp, h = 10 * mpp;
    const nodes = s4.canopy.fallenDeadwood.map((record) => {
      const sp = speciesLabel(speciesOf(record.treeId));
      const g = svg('g', { class: 'log', 'data-deadwood-id': record.deadwoodId, 'data-species': speciesOf(record.treeId) });
      g.append(svg('title', null, record.deadwoodId + ' · ' + sp + ' fallen log (year ' + record.deathYear + ', direction schematic, not simulated)'));
      g.append(svg('use', { href: '#ccf-log', x: n3(record.u - w / 2), y: n3(record.v - h / 2), width: n3(w), height: n3(h) }));
      return g;
    });
    host.replaceChildren(...nodes);
  }

  // Planted juveniles (PJ) and active/expired shelters in their own layer ABOVE SpotClear patches.
  function renderPlantedLayer(areaId, mpp) {
    const host = byId('planted-layer');
    if (!state.ecology) { host.replaceChildren(); return; }
    const nodes = [];
    const sSize = Math.max(0.75, 18 * mpp);
    for (const j of (state.ecology.planted || [])) {
      if (j.areaId !== areaId || !j.isActive) continue;
      const sym = SAPLING_SYMBOL[j.speciesId] || 'ccf-sapling-oak';
      const g = svg('g', { class: 'planted-pj', 'data-juvenile-id': j.juvenileId });
      g.append(svg('title', null, j.juvenileId + ' · ' + speciesLabel(j.speciesId) + ' planted sapling, H ' + fixed(j.heightM, 2) + ' m'));
      g.append(svg('use', { href: '#' + sym, x: n3(j.u - sSize / 2), y: n3(j.v - sSize / 2), width: n3(sSize), height: n3(sSize) }));
      nodes.push(g);
    }
    const shSize = Math.max(0.95, 22 * mpp);
    for (const sh of (state.ecology.shelters || [])) {
      if (sh.areaId !== areaId) continue;
      const sym = sh.isEffective ? 'ccf-shelter-active' : 'ccf-shelter-expired';
      const g = svg('g', { class: 'shelter-mark', 'data-shelter-id': sh.shelterId, 'data-effective': String(sh.isEffective) });
      g.append(svg('title', null, sh.shelterId + ' (' + (sh.isEffective ? 'Active shelter · expires year ' + sh.expiryYear : 'Expired shelter · expired year ' + sh.expiryYear) + ')'));
      g.append(svg('use', { href: '#' + sym, x: n3(sh.u - shSize / 2), y: n3(sh.v - shSize / 2), width: n3(shSize), height: n3(shSize) }));
      nodes.push(g);
    }
    host.replaceChildren(...nodes);
  }

  function renderGroundMark(areaId) {
    const c = state.cellDetail;
    const poly = state.mapMode === 'cell' && c && !c.missing && c.areaId === areaId ? c.polygon : null;
    const points = poly ? poly.map((p) => n3(p.u) + ',' + n3(p.v)).join(' ') : null;
    byId('ground-mark').replaceChildren(...(points ? [svg('polygon', { class: 'ground-pick-halo', points }),
      svg('polygon', { class: 'ground-pick', points })] : []));
  }

  const RAMP_SAMPLES = [0, 0.002, 0.005, 0.01, 0.02, 0.05, 0.1, 0.2, 0.35, 0.5, 0.65, 0.8, 0.9, 1];
  function renderOverlayLegend() {
    const box = byId('overlay-legend');
    if (state.overlay === 'off') { box.hidden = true; box.replaceChildren(); return; }
    box.hidden = false;
    if (state.overlay === 'light') {
      const ramp = svg('svg', { id: 'light-ramp', class: 'light-ramp', viewBox: '0 0 100 10', preserveAspectRatio: 'none', role: 'img',
        'aria-label': 'Light Guide colour ramp: shaded to bright' });
      const grad = svg('linearGradient', { id: 'light-ramp-fill', x1: 0, y1: 0, x2: 1, y2: 0 });
      for (const value of RAMP_SAMPLES) {
        const paint = lightPaint(value);
        grad.append(svg('stop', { offset: value, 'data-l': value, 'stop-color': paint ? paint.colour : '#7f3b4a',
          'stop-opacity': paint ? paint.opacity : 0 }));
      }
      const defs = svg('defs');
      defs.append(grad);
      ramp.append(defs, svg('rect', { x: 0, y: 0, width: 100, height: 10, fill: 'rgb(' + SLATE.join(',') + ')' }),
        svg('rect', { x: 0, y: 0, width: 100, height: 10, fill: 'url(#light-ramp-fill)' }));
      box.replaceChildren(
        el('p', { class: 'overlay-legend-title' }, 'Light Guide'),
        el('span', { class: 'ramp-end', 'aria-hidden': 'true' }, 'Shaded'),
        ramp,
        el('span', { class: 'ramp-end', 'aria-hidden': 'true' }, 'Bright'),
        el('p', { class: 'overlay-legend-note' }, 'Modelled light · not calibrated PAR'));
    } else if (state.overlay === 'undergrowth') {
      const swBramble = svg('svg', { class: 'overlay-swatch', viewBox: '0 0 4 2.5', 'aria-hidden': 'true' });
      swBramble.append(svg('rect', { width: 4, height: 2.5, fill: '#1b2430' }), svg('rect', { width: 4, height: 2.5, fill: 'url(#ccf-bramble-hatch)' }));
      const swBracken = svg('svg', { class: 'overlay-swatch', viewBox: '0 0 4 2.5', 'aria-hidden': 'true' });
      swBracken.append(svg('rect', { width: 4, height: 2.5, fill: '#1b2430' }), svg('rect', { width: 4, height: 2.5, fill: 'url(#ccf-bracken-hatch)' }));
      box.replaceChildren(
        el('p', { class: 'overlay-legend-title' }, 'Undergrowth'),
        el('span', null, swBramble, ' Bramble'),
        el('span', null, swBracken, ' Bracken'),
        el('p', { class: 'overlay-legend-note' }, 'Hatch opacity scales with cell cover (0–1). ClearUndergrowth: Whole 2.5 m cell · spares seedlings.'));
    } else if (state.overlay === 'seedlings') {
      const swSeed = svg('svg', { class: 'overlay-swatch', viewBox: '-24 -24 48 48', 'aria-hidden': 'true' });
      swSeed.append(svg('use', { href: '#ccf-natural-seedlings', x: -24, y: -24, width: 48, height: 48 }));
      box.replaceChildren(
        el('p', { class: 'overlay-legend-title' }, 'Natural Seedlings'),
        el('span', null, swSeed, ' Cell presence (S Sitka · O Oak · B Beech)'),
        el('p', { class: 'overlay-legend-note' }, label('MultiNaturalSeedlingsNote') || 'Cell-level presence only — Density is abstract occupancy, not a literal stem count'));
    }
  }

  function renderScale(scale) {
    const metres = [0.5, 1, 2, 5, 10, 20, 50, 100].find((m) => m * scale >= 56) || 100;
    const width = Math.max(8, Math.round(metres * scale));
    const bar = byId('scale-bar');
    setAttributes(bar, { width, height: 9, viewBox: '0 0 ' + width + ' 9' });
    byId('scale-path').setAttribute('d', 'M 1 1 V 8 H ' + (width - 1) + ' V 1');
    byId('scale-text').textContent = metres + ' m';
  }

  function renderMapStatus(reference, drafts) {
    const box = byId('map-status');
    const view = state.view;
    let text = '';
    if (state.mapMode === 'cell') text = 'Inspect Cell mode: click any 2.5 m cell inside the area · Esc returns to Select Tree (draft Fell ticks kept)';
    else if (state.batch) text = 'Adding ' + (state.batch.confirmed.length + 1) + ' of ' + state.batch.ids.length + ' to the Fell plan…';
    else if (view.approved && approvedPrunes().length > 0) text = workTotals(view.approved.orders.length, approvedPrunes().length) + ' approved · nothing done yet';
    else if (view.approved) text = plural(view.approved.orders.length, 'approved Fell', 'approved Fell') + ' · still standing';
    else if (reference) text = 'Reference ' + reference + ' · ' + plural(drafts, 'draft choice', 'draft choices');
    box.hidden = text === '';
    box.textContent = text;
  }

  // ---- status marks ---------------------------------------------------------------------------------------------------

  function markPath(cls, d) { return svg('path', { class: 'ring ' + cls, d }); }
  function at(item, dx, dy) { return n3(item.u + dx) + ' ' + n3(item.v + dy); }

  function cornerFrame(item, cls, mpp) {
    const h = item.radiusM + 7 * mpp, arm = 10 * mpp;
    let d = '';
    for (const [sx, sy] of [[-1, -1], [1, -1], [1, 1], [-1, 1]]) {
      d += 'M ' + at(item, sx * h, sy * (h - arm)) + ' L ' + at(item, sx * h, sy * h) + ' L ' + at(item, sx * (h - arm), sy * h) + ' ';
    }
    return markPath(cls, d);
  }

  function sideBrackets(item, cls, mpp) {
    const x = item.radiusM + 7 * mpp, y = 9 * mpp, arm = 5 * mpp;
    let d = '';
    for (const s of [-1, 1]) {
      d += 'M ' + at(item, s * (x - arm), -y) + ' L ' + at(item, s * x, -y) + ' L ' + at(item, s * x, y) + ' L ' + at(item, s * (x - arm), y) + ' ';
    }
    return markPath(cls, d);
  }

  function underline(item, cls, mpp, extraPx) {
    const y = item.radiusM * .8 + (extraPx || 0) * mpp, half = 10 * mpp;
    return markPath(cls, 'M ' + at(item, -half, y) + ' L ' + at(item, half, y));
  }

  const TILE = { rank: -45, status: 45, crop: 135, reference: -135 };
  const RANK_ANGLES = [-45, 45, -135, 135, 0, 180, -90, 90];

  function tileCentre(item, angleDeg, mpp) {
    const angle = angleDeg * Math.PI / 180, distance = item.radiusM + 9 * mpp;
    return [item.u + Math.cos(angle) * distance, item.v + Math.sin(angle) * distance];
  }

  function badge(item, angleDeg, text, kind, mpp, extra) {
    const [cx, cy] = tileCentre(item, angleDeg, mpp);
    const g = svg('g', { class: 'badge b-' + kind + (extra ? ' ' + extra : '') });
    g.append(svg('rect', { class: 'badge-bg', x: n3(cx - 10 * mpp), y: n3(cy - 10 * mpp), width: n3(20 * mpp), height: n3(20 * mpp), rx: n3(2 * mpp) }),
      svg('text', { class: 'badge-text', x: n3(cx), y: n3(cy + .5 * mpp), 'font-size': n3(10.5 * mpp) }, text));
    return g;
  }

  function tag(x, y, text, mpp) {
    const width = (text.length * 6.4 + 14) * mpp, height = 19 * mpp;
    const g = svg('g', { class: 'map-tag' });
    g.append(svg('rect', { x: n3(x - width / 2), y: n3(y - height / 2), width: n3(width), height: n3(height), rx: n3(4 * mpp) }),
      svg('text', { x: n3(x), y: n3(y + .5 * mpp), 'font-size': n3(11 * mpp) }, text));
    return g;
  }

  function tick(item, mpp, angle) {
    const [rx, ry] = tileCentre(item, angle ?? TILE.rank, mpp);
    const cx = rx + 11 * mpp, cy = ry - 11 * mpp;
    const p = (dx, dy) => n3(cx + dx * mpp) + ' ' + n3(cy + dy * mpp);
    return [svg('circle', { class: 'tick-box', cx: n3(cx), cy: n3(cy), r: n3(7 * mpp) }),
      svg('path', { class: 'tick', d: 'M ' + p(-3, 0) + ' L ' + p(-1, 2.5) + ' L ' + p(3.5, -3) })];
  }

  function statusMarks(item, mpp) {
    const marks = [];
    // Rule 5: Retained crop tree framed by double gold ring + small C badge (#ccf-crop-ring), never sawn lumber.
    if (item.isCrop) {
      const span = Math.max(item.radiusM * 1.22, item.radiusM + 5 * mpp);
      marks.push(svg('use', { class: 'deco crop-ring', href: '#ccf-crop-ring',
        x: n3(item.u - span), y: n3(item.v - span), width: n3(2 * span), height: n3(2 * span) }));
    }
    // Rule 3: Pending = dashed gold outline + clock/P; Approved = solid pale-blue badge + check/A.
    if (item.status === 'PendingFell') {
      marks.push(underline(item, 'ring-pending', mpp, 4), badge(item, TILE.status, '◷P', 'pending', mpp));
    } else if (item.status === 'ApprovedFell') {
      marks.push(underline(item, 'ring-approved', mpp, 4), underline(item, 'ring-approved', mpp, 8),
        badge(item, TILE.status, '✓A', 'approved', mpp));
    } else if (item.status === 'NaturalDead') {
      marks.push(badge(item, TILE.status, 'D', 'dead', mpp));
    } else if (item.status === 'Felled') {
      const d = Math.min(item.radiusM * .45, 6 * mpp);
      marks.push(svg('path', { class: 'stump-x', d: 'M ' + n3(item.u - d) + ' ' + n3(item.v - d) + ' L ' + n3(item.u + d) + ' ' +
        n3(item.v + d) + ' M ' + n3(item.u - d) + ' ' + n3(item.v + d) + ' L ' + n3(item.u + d) + ' ' + n3(item.v - d) }),
        badge(item, TILE.status, 'X', 'executed', mpp));
    }
    return marks;
  }

  function competitorMarks(item, rank, chosen, mpp, angle) {
    const marks = [underline(item, chosen ? 'ring-draft' : 'ring-competitor', mpp, 0)];
    marks.push(badge(item, angle ?? TILE.rank, String(rank), 'rank', mpp, chosen ? 'chosen' : null));
    if (chosen) marks.push(...tick(item, mpp, angle));
    return marks;
  }

  function renderRankLayer(ranks, box) {
    const layer = byId('rank-layer');
    const active = document.activeElement;
    const focusKey = active && layer.contains(active) ? active.getAttribute('data-focus-key') : null;
    const planLocked = state.view.approved !== null;
    const frozenNow = state.lock !== null || state.batch !== null;
    const buttons = ranks.map((r) => {
      const why = blockedReason(state.areaId, r.treeId) ||
        (planLocked ? 'The plan is approved, so editing is locked' : frozenNow ? 'Please wait' : null);
      const button = el('button', { type: 'button', class: 'rank-toggle', 'data-rank-toggle': r.treeId, 'data-rank': r.rank,
        'data-focus-key': 'rank:' + r.treeId, 'aria-pressed': String(r.chosen && !why), 'aria-disabled': why ? 'true' : null,
        'aria-label': 'Competitor ' + r.rank + ', tree ' + r.treeId + ', Fell draft' + (why ? ' (' + why + ')' : ''),
        title: 'Competitor ' + r.rank + ' · ' + r.treeId + (why ? ' · ' + why : r.chosen ? ' · in the Fell draft (click to remove)' : ' · click to add to the Fell draft') },
      String(r.rank));
      if (button.style) {
        button.style.left = n3((r.x - box.x) * box.scale) + 'px';
        button.style.top = n3((r.y - box.y) * box.scale) + 'px';
      }
      return button;
    });
    layer.replaceChildren(...buttons);
    layer.hidden = buttons.length === 0;
    if (focusKey) {
      const again = layer.querySelector('[data-focus-key="' + CSS.escape(focusKey) + '"]');
      if (again) again.focus();
    }
  }

  function onRankToggle(event) {
    const button = event.target.closest('[data-rank-toggle]');
    if (!button) return;
    event.stopPropagation();
    const treeId = button.getAttribute('data-rank-toggle');
    if (button.getAttribute('aria-disabled') === 'true') {
      announce('Competitor ' + button.getAttribute('data-rank') + ', ' + treeId + ', cannot be chosen: ' +
        (blockedReason(state.areaId, treeId) || (state.view.approved ? 'the plan is approved, so editing is locked' : 'please wait')) + '.');
      return;
    }
    toggleDraft(treeId, button.getAttribute('aria-pressed') !== 'true');
  }

  function referenceMarks(item, mpp, withTag) {
    const marks = [cornerFrame(item, 'ring-halo', mpp), cornerFrame(item, 'ring-reference', mpp),
      badge(item, TILE.reference, 'R', 'reference', mpp)];
    if (withTag) marks.push(tag(item.u, item.v - item.radiusM - 22 * mpp, 'Reference' + (item.isCrop ? ' · Crop' : ''), mpp));
    return marks;
  }

  function inspectMarks(item, mpp, withTag) {
    const marks = [sideBrackets(item, 'ring-halo', mpp), sideBrackets(item, 'ring-inspect', mpp),
      badge(item, TILE.reference, 'I', 'inspect', mpp)];
    if (withTag) marks.push(tag(item.u, item.v - item.radiusM - 22 * mpp, 'Selected', mpp));
    return marks;
  }

  function pruneMarks(areaId, item, mpp) {
    const ps = pruneStatusOf(areaId, item.treeId);
    if (!ps || !ps.planState) return [];
    return ps.planState === 'PendingPrune'
      ? [underline(item, 'ring-pending', mpp, 4), badge(item, TILE.status, '◷✂', 'pending', mpp)]
      : [underline(item, 'ring-approved', mpp, 4), underline(item, 'ring-approved', mpp, 8), badge(item, TILE.status, '✓✂', 'approved', mpp)];
  }

  // ---- Inspect Cell mode (POST api/multi/cell) ------------------------------------------------------------------------

  function setMapMode(mode, moveFocus) {
    if (mode !== 'tree' && mode !== 'cell') return;
    if (state.mapMode === mode) return;
    state.mapMode = mode;
    if (mode === 'tree') {
      state.cellPick += 1;
      state.cellLoading = false;
    }
    showPane('tree');
    renderAll();
    if (moveFocus) {
      if (mode === 'cell') byId('cell-heading').focus();
      else byId('mode-tree').focus();
    }
    announce(mode === 'cell'
      ? 'Inspect Cell mode active: click any 2.5 m cell inside the area to inspect light, undergrowth and natural cohorts. Draft Fell ticks are preserved; press Escape to return.'
      : 'Select Tree mode active: clicking a crown selects that tree. Draft Fell ticks preserved.');
  }

  function setOverlay(value) {
    if (!['off', 'light', 'undergrowth', 'seedlings'].includes(value)) return;
    state.overlay = value;
    renderAll();
    const names = { off: 'Off', light: 'Light Guide (Modelled light · not calibrated PAR)',
      undergrowth: 'Undergrowth (bramble and bracken hatches)',
      seedlings: 'Natural Seedlings (cell-level presence only)' };
    announce('Ground overlay: ' + names[value] + '.');
  }

  function insidePolygon(points, u, v) {
    let inside = false;
    for (let i = 0, j = points.length - 1; i < points.length; j = i++) {
      const a = points[i], b = points[j];
      if ((a.v > v) !== (b.v > v) && u < (b.u - a.u) * (v - a.v) / (b.v - a.v) + a.u) inside = !inside;
    }
    return inside;
  }

  async function inspectCellAtPoint(areaId, u, v) {
    const s4 = s4Area(areaId);
    const cell = s4 ? s4.ground.find((c) => insidePolygon(c.polygon, u, v)) || null : null;
    if (!cell) {
      state.cellDetail = { areaId, missing: true };
      state.cellLoading = false;
      showPane('tree');
      renderAll();
      byId('cell-heading').focus();
      announce('No modelled 2.5 m cell at that point; nothing is estimated.');
      return;
    }
    const pick = ++state.cellPick;
    state.cellLoading = true;
    renderCellInspector();
    const outcome = await call('api/multi/cell', { fence: currentFence(), areaId, column: cell.column, row: cell.row });
    if (pick !== state.cellPick || state.mapMode !== 'cell') return;
    state.cellLoading = false;
    if (outcome.kind === 'ok') {
      clearAlert();
      state.cellDetail = outcome.payload.cell;
      applyPayload(outcome.payload, false);
      byId('cell-heading').focus();
      const d = outcome.payload.cell;
      announce('Inspected cell (' + d.column + ', ' + d.row + ') in ' + areaName(d.areaId) + ': light ' + lightIndexText(d.lightIndex) +
        ', cohorts ' + d.cohorts.length + '. Read-only; draft Fell ticks unchanged.');
    } else if (outcome.kind === 'refused') {
      state.cellDetail = null;
      applyPayload(outcome.payload, false);
      if (state.lock) showLockAlert('Refused: Inspect cell (' + outcome.payload.refusal.code + ').');
      else showRefusal('Inspect cell', outcome.payload.refusal);
    } else if (outcome.kind === 'guard') {
      showGuardRefusal('Inspect cell', outcome.status, outcome.error);
    } else if (outcome.kind === 'locked') {
      showLockAlert('Not sent: Inspect cell.');
    } else {
      showAlert([el('p', { class: 'alert-title' }, 'Cell inspection could not reach the local session; nothing changed.'), rereadButton()]);
    }
  }

  function renderCellInspector() {
    if (state.mapMode !== 'cell') return;
    const summaryBox = byId('cell-summary');
    const cohortsBox = byId('cell-cohorts');
    const treesBox = byId('cell-trees');
    const techBox = byId('cell-tech');
    if (state.cellLoading) {
      byId('cell-heading').textContent = 'Reading cell detail…';
      summaryBox.replaceChildren(el('p', { class: 'small-note' }, 'Querying server cell state (read-only)…'));
      cohortsBox.replaceChildren();
      treesBox.hidden = true;
      techBox.hidden = true;
      return;
    }
    const c = state.cellDetail && state.cellDetail.areaId === state.areaId ? state.cellDetail : null;
    if (!c) {
      byId('cell-heading').textContent = areaName(state.areaId) + ' · choose a 2.5 m cell';
      summaryBox.replaceChildren(
        el('p', null, 'Click any spot inside ' + areaName(state.areaId) + ' to fetch full read-only cell detail via POST api/multi/cell.'),
        el('p', { class: 'small-note' }, 'Read-only: draft Fell ticks and Work Plan are unchanged. Press Escape or Exit cell mode to return to Select Tree.'));
      cohortsBox.replaceChildren();
      treesBox.hidden = true;
      techBox.hidden = true;
      return;
    }
    if (c.missing) {
      byId('cell-heading').textContent = areaName(state.areaId) + ' · outside cells';
      summaryBox.replaceChildren(el('p', null, 'No in-polygon 2.5 m cell at that point. No value is shown or estimated.'));
      cohortsBox.replaceChildren();
      treesBox.hidden = true;
      techBox.hidden = true;
      return;
    }

    byId('cell-heading').textContent = areaName(c.areaId) + ' · cell (' + c.column + ', ' + c.row + ')';
    const paint = lightPaint(c.lightIndex);
    const swatch = svg('svg', { class: 'ground-swatch', viewBox: '0 0 10 10', 'aria-hidden': 'true', focusable: 'false' });
    swatch.append(svg('rect', { width: 10, height: 10, fill: 'rgb(' + SLATE.join(',') + ')' }));
    if (paint) swatch.append(svg('rect', { width: 10, height: 10, fill: paint.colour, 'fill-opacity': paint.opacity }));
    const clearLabel = c.hasPendingClear ? 'Pending ClearUndergrowth (Whole 2.5 m cell · spares seedlings)'
      : c.hasApprovedClear ? 'Approved ClearUndergrowth (Whole 2.5 m cell · spares seedlings)' : 'None';

    summaryBox.replaceChildren(
      el('p', { class: 'ground-level' }, swatch, lightWord(c.lightIndex)),
      el('p', { class: 'small-note' }, 'Modelled light · not calibrated PAR'),
      el('dl', null,
        el('dt', null, 'Light index'), el('dd', null, lightIndexText(c.lightIndex) + ' (0–1 modelled)'),
        el('dt', null, 'Canopy cover'), el('dd', null, pct(c.geometricCoveredFraction)),
        el('dt', null, 'Bramble cover'), el('dd', null, c.bramble === null ? 'not modelled' : pct1(c.bramble)),
        el('dt', null, 'Bracken cover'), el('dd', null, c.bracken === null ? 'not modelled' : pct1(c.bracken)),
        el('dt', null, 'Browse pressure'), el('dd', null, c.browsePressure === null ? 'not modelled' : fixed(c.browsePressure, 2)),
        el('dt', null, 'Clear state'), el('dd', null, clearLabel)));

    const cohorts = c.cohorts || [];
    if (cohorts.length === 0) {
      cohortsBox.replaceChildren(
        el('h3', null, 'Natural regeneration cohorts (0)'),
        el('p', { class: 'small-note' }, 'No natural regeneration bands in this cell yet. Density is abstract occupancy, never a literal stem count.'));
    } else {
      cohortsBox.replaceChildren(
        el('h3', null, 'Natural regeneration cohorts (' + cohorts.length + ')'),
        el('p', { class: 'small-note' }, 'Cell-level cohorts — Density is abstract occupancy, not a literal stem count.'),
        table('Cohorts in cell (' + c.column + ', ' + c.row + ')', ['Species', 'Est. yr', 'H (m)', 'Density', 'Browsed'],
          cohorts.map((b) => [speciesLabel(b.species), String(b.establishYear), fixed(b.heightM, 2), fixed(b.density, 3),
            b.lastBrowseAssessmentYear >= 0 ? pct1(b.lastBrowsedFraction) : '–'])));
    }

    const contributors = c.contributingTreeIds || [];
    treesBox.hidden = false;
    byId('cell-trees-summary').textContent = 'Contributing trees (' + contributors.length + ')';
    byId('cell-tree-ids').replaceChildren(...(contributors.length === 0 ? ['none (no living crown shades this cell centre)'] :
      contributors.flatMap((id, i) => [i > 0 ? ', ' : null, el('span', { class: 'id' }, id)].filter((part) => part !== null))));

    techBox.hidden = false;
    byId('cell-facts').replaceChildren(
      pair('Modelled light index', fixed(c.lightIndex, 4) + ' (0–1)'),
      pair('Overhead cover fraction', fixed(c.geometricCoveredFraction, 4)),
      pair('Clipped cell area', fixed(c.clippedAreaM2, 2) + ' m²'),
      pair('Cell centre', 'X ' + fixed(c.centreXM, 2) + ' m east, Z ' + fixed(c.centreZM, 2) + ' m north'),
      pair('Cross-border trees', String(c.crossBorderTrees)),
      pair('Polygon vertices', String((c.polygon || []).length)));
    byId('cell-caveat').textContent = 'Modelled light · not calibrated PAR. Read-only inspection; zero state or revision change.';
  }

  // ---- grid and legend ------------------------------------------------------------------------------------------------

  function drawGrid(area, mpp, scale) {
    const side = area.extent.sideM;
    const minX = area.extent.minXM;
    const minZ = area.extent.minZM;
    const step = 5 * scale >= 26 ? 5 : 10;
    const reach = 80;
    const lines = [];
    const labels = [];
    const font = n3(10 * mpp);
    for (let x = Math.ceil((minX - reach) / step) * step; x <= minX + side + reach; x += step) {
      const u = x - minX;
      lines.push(svg('line', { class: 'grid-line', x1: n3(u), y1: -reach, x2: n3(u), y2: side + reach }));
      if (x >= minX - 1e-9 && x <= minX + side + 1e-9) {
        labels.push(svg('text', { class: 'grid-label', x: n3(u), y: n3(side + 14 * mpp), 'text-anchor': 'middle', 'font-size': font }, x));
      }
    }
    for (let z = Math.ceil((minZ - reach) / step) * step; z <= minZ + side + reach; z += step) {
      const v = minZ + side - z;
      lines.push(svg('line', { class: 'grid-line', x1: -reach, y1: n3(v), x2: side + reach, y2: n3(v) }));
      if (z >= minZ - 1e-9 && z <= minZ + side + 1e-9) {
        labels.push(svg('text', { class: 'grid-label', x: n3(-7 * mpp), y: n3(v + 3.5 * mpp), 'text-anchor': 'end', 'font-size': font }, z));
      }
    }
    labels.push(svg('text', { class: 'grid-label', x: n3(side / 2), y: n3(side + 30 * mpp), 'text-anchor': 'middle', 'font-size': font },
      'X metres east · Z metres north'));
    byId('grid').replaceChildren(...lines);
    byId('grid-in').replaceChildren(...lines.map((line) => line.cloneNode(false)));
    byId('furniture').replaceChildren(...labels);
  }

  function legendIcon(draw) {
    const icon = svg('svg', { viewBox: '-1.9 -1.9 3.8 3.8', 'aria-hidden': 'true', focusable: 'false' });
    const mpp = .075;
    const item = { u: 0, v: 0, radiusM: 1, treeId: 'legend', status: 'Living', isCrop: false };
    icon.append(svg('circle', { class: 'swatch-ground', cx: 0, cy: 0, r: 1.85 }));
    icon.append(...draw(item, mpp));
    return icon;
  }

  function legendUse(symbolId, span) {
    const s = span || 1.35;
    return [svg('use', { href: '#' + symbolId, x: -s, y: -s, width: 2 * s, height: 2 * s })];
  }

  function renderLegend() {
    const rows = [
      ['Sitka spruce', 'Radial conifer crown', () => legendUse('ccf-sitka', 1.25)],
      ['Sessile oak', 'Lobed broadleaf crown', () => legendUse('ccf-oak', 1.25)],
      ['European beech', 'Dense rounded broadleaf', () => legendUse('ccf-beech', 1.25)],
      ['Retained Crop · C', 'Double gold ring + C badge', (item, mpp) => [...legendUse('ccf-sitka', 1.05),
        ...statusMarks(Object.assign({}, item, { isCrop: true }), mpp)]],
      ['Reference · R / Comp. 1–5', 'Pinned reference & top 5', (item, mpp) => [...legendUse('ccf-sitka', 1.0), ...referenceMarks(item, mpp, false)]],
      ['Pending · ◷', 'Dashed gold outline; queued', (item, mpp) => [...legendUse('ccf-sitka', 1.0),
        ...statusMarks(Object.assign({}, item, { status: 'PendingFell' }), mpp)]],
      ['Approved · ✓', 'Solid pale-blue; awaiting Execute', (item, mpp) => [...legendUse('ccf-sitka', 1.0),
        ...statusMarks(Object.assign({}, item, { status: 'ApprovedFell' }), mpp)]],
      ['Felled stump · X', 'Executed work (History Log)', (item, mpp) => [...crownDecoration(Object.assign({}, item, { status: 'Felled' }), 'sitka-spruce'),
        ...statusMarks(Object.assign({}, item, { status: 'Felled' }), mpp)]],
      ['Standing dead · D', 'Natural mortality crown', (item, mpp) => [...legendUse('ccf-standing-dead', 1.15),
        ...statusMarks(Object.assign({}, item, { status: 'NaturalDead' }), mpp)]],
      ['Fallen log (schematic)', 'Direction schematic, not simulated', () => [svg('use', { href: '#ccf-log', x: -1.45, y: -0.55, width: 2.9, height: 1.1 })]],
      ['Active shelter (< 8 yr)', 'Cyan mesh tube protecting PJ', () => legendUse('ccf-shelter-active', 1.35)],
      ['Expired shelter (8+ yr)', 'Dashed grey + X · map state only', () => legendUse('ccf-shelter-expired', 1.35)],
      ['SpotClear (~1 m²)', 'Cleared soil + twigs, no seedling', () => legendUse('ccf-spot-clear', 1.25)],
      ['ClearUndergrowth', 'Whole 2.5 m cell · spares seedlings', () => legendUse('ccf-action-clear', 1.35)],
      ['Natural seedlings', 'Cell-level presence glyph only', () => legendUse('ccf-natural-seedlings', 1.35)],
    ];
    const items = rows.map(([name, description, draw]) => el('li', null, legendIcon(draw),
      el('span', null, el('span', { class: 'legend-name' }, name), el('span', { class: 'legend-desc' }, description))));
    const boundary = svg('svg', { viewBox: '-1.9 -1.9 3.8 3.8', 'aria-hidden': 'true', focusable: 'false' });
    boundary.append(svg('rect', { class: 'legend-exterior', x: -1.85, y: -1.85, width: 3.7, height: 3.7, rx: .3 }),
      svg('polygon', { class: 'legend-area', points: '-1.3,-1 1.2,-1.35 1.35,1.2 -1.2,1.3' }));
    items.push(el('li', null, boundary, el('span', null, el('span', { class: 'legend-name' }, 'Area boundary'),
      el('span', { class: 'legend-desc' }, 'Actual management extent'))));
    byId('legend').replaceChildren(...items);
  }

  function setLegendHidden(hidden, moveFocus) {
    state.legendHidden = hidden;
    byId('workspace').setAttribute('data-legend', hidden ? 'hidden' : 'shown');
    byId('legend-show').hidden = !hidden;
    byId('legend-show').setAttribute('aria-expanded', String(!hidden));
    byId('legend-hide').setAttribute('aria-expanded', String(!hidden));
    if (moveFocus) byId(hidden ? 'legend-show' : 'legend-hide').focus();
    announce(hidden ? 'Legend hidden. Show legend is at the top right of the map.' : 'Legend shown.');
  }

  // ---- Tree inspector, Prune box (species-aware) and Competition ------------------------------------------------------

  function renderOverlap() {
    const box = byId('overlap');
    const overlap = state.overlap;
    if (!overlap || overlap.areaId !== state.areaId || state.mapMode !== 'tree') {
      box.hidden = true;
      byId('overlap-list').replaceChildren();
      return;
    }
    box.hidden = false;
    byId('overlap-heading').textContent = fill(label('CrownsOverlap'), { n: overlap.candidates.length });
    byId('overlap-list').replaceChildren(...overlap.candidates.map((candidate, index) => {
      const current = candidate.treeId === selectedId();
      return el('li', null, el('button', { type: 'button', class: 'small', 'data-pick': candidate.treeId,
        'data-focus-key': 'pick:' + candidate.treeId, 'aria-label': (current ? 'Selected ' : 'Select ') + candidate.treeId },
        current ? 'Selected' : 'Select'), ' ', el('span', { class: 'mono' }, candidate.treeId),
        ' (' + speciesLabel(speciesOf(candidate.treeId)) + ') — ' + statusText(candidate) + (index === 0 ? ' (topmost)' : ''));
    }));
  }

  function recordedReason(areaId, treeId) {
    const view = state.view;
    const pending = view.pending.find((order) => order.areaId === areaId && order.treeId === treeId);
    if (pending) return pending.reason + ' (pending order)';
    const approved = view.approved ? view.approved.orders.find((order) => order.areaId === areaId && order.treeId === treeId) : null;
    if (approved) return approved.reason + ' (approved order)';
    for (const event of view.events) {
      for (const change of event.areas) {
        const fell = change.fells.find((record) => record.treeId === treeId);
        if (fell) return fell.reason + ' (executed Fell, work revision ' + fell.workRevision + ')';
      }
    }
    return null;
  }

  function keyFact(term, value, unit) {
    return el('div', null, el('dt', null, term), el('dd', null, value, ' ', el('small', null, unit)));
  }

  function renderInspector() {
    const areaId = state.areaId;
    const id = selectedId();
    const tree = treeIn(areaId, id);
    const item = tree ? itemOf(areaId, id) : null;
    byId('inspector-empty').hidden = Boolean(tree && item);
    byId('inspector-body').hidden = !(tree && item);
    if (!tree || !item) return;
    const sp = speciesOf(tree.treeId);
    const spName = speciesLabel(sp);
    const isReference = referenceIn(areaId) === id;
    byId('insp-role').textContent = (isReference ? 'Reference tree' : 'Selected tree') + ' · ' + spName;
    byId('insp-area').textContent = areaName(areaId);
    byId('insp-id').textContent = tree.treeId;
    const chip = byId('insp-chip');
    chip.textContent = item.isCrop ? 'C · ' + label('CropRetained') : baseStatusText(item);
    chip.setAttribute('class', 'chip s-' + item.status + (item.isCrop ? ' crop' : ''));
    chip.setAttribute('title', chip.textContent);
    chip.hidden = !item.isCrop && item.status === 'Living';
    byId('insp-summary').textContent = spName + ' · ' + baseStatusText(item);
    byId('insp-key').replaceChildren(
      keyFact('DBH', fixed(tree.dbhCm, 1), 'cm'),
      keyFact('Height', fixed(tree.heightM, 1), 'm'),
      keyFact('Crown r', fixed(tree.crownRadiusM, 2), 'm'));
    byId('inspect-close').setAttribute('aria-label', 'Close inspection of ' + tree.treeId + ' (Escape)');
    byId('insp-pinned').textContent = isReference ? 'Reference stays pinned while choosing' : '';
    const facts = [
      pair('Species', spName + ' (' + sp + ')'),
      pair('DBH', fixed(tree.dbhCm, 2) + ' cm'),
      pair('Height', fixed(tree.heightM, 2) + ' m'),
      pair('Crown radius', fixed(tree.crownRadiusM, 3) + ' m'),
      pair('Age', tree.ageYears + ' years'),
      pair('Position', 'X ' + fixed(tree.xm, 2) + ' m east, Z ' + fixed(tree.zm, 2) + ' m north', 'wide'),
    ];
    if (tree.death) {
      facts.push(pair('Death year', tree.death.ecologicalYear));
      facts.push(pair('Cause', tree.death.cause + ' (' + tree.death.mechanism + ')'));
    }
    const s4 = s4Tree(areaId, tree.treeId);
    const ps = pruneStatusOf(areaId, tree.treeId);
    if (s4) {
      facts.push(pair('Life state', s4.lifeState + (s4.deathCause ? ' · ' + s4.deathCause + ', year ' + s4.deathYear : '')));
    }
    if (ps) {
      facts.push(pair('Pruning lifts', ps.lifts + ' / ' + ps.maxLifts + ' (' + ps.minIntervalYears + '-yr wait)' +
        (s4 && s4.pruningLifts > 0 ? ' · crown base ' + fixed(s4.crownBaseHeightM, 1) + ' m · last lift year ' + s4.lastPruningYear : ''), 'wide'));
    }
    const dw = s4Area(areaId) ? s4Area(areaId).canopy.fallenDeadwood.find((d) => d.treeId === tree.treeId) : null;
    if (dw) {
      facts.push(pair('Fallen deadwood', dw.deadwoodId + ' · ' + spName + ' · vol ' + fixed(dw.estimatedVolumeM3, 3) + ' m³ · direction schematic, not simulated', 'wide'));
    }
    const reason = recordedReason(areaId, tree.treeId);
    if (reason !== null) facts.push(pair('Recorded reason', reason, 'wide'));
    byId('insp-facts').replaceChildren(...facts);
    const crop = byId('crop-button');
    crop.textContent = label(item.isCrop ? 'MultiUnmarkCrop' : 'MultiMarkCrop');
    crop.hidden = !tree.isLiving;
    byId('crop-note').textContent = item.isCrop ? label('MultiCropFellRefused') + '.' :
      item.status === 'PendingFell' || item.status === 'ApprovedFell' ? 'Remove the pending Fell or cancel its approved plan before designating Crop.' :
        tree.isLiving ? label('MultiCropSeparate') + '.' : 'Dead trees cannot retain a Crop designation.';
    const addSelf = item.status === 'Living' && !item.isCrop;
    byId('add-reference-wrap').hidden = !addSelf;
    byId('add-reference').textContent = 'Fell this tree instead';
    byId('add-reference').setAttribute('aria-label', 'Add ' + tree.treeId + ' itself to the Fell plan');
    byId('insp-remove-wrap').hidden = item.status !== 'PendingFell';
    byId('insp-remove').textContent = label('MultiRemoveFromPlan');
    byId('insp-approved-note').hidden = item.status !== 'ApprovedFell';
    byId('insp-threshold').textContent = label('MultiThresholdInformational');
    byId('insp-threshold').hidden = !tree.isLiving;
    renderPruneBox(areaId, tree, item, ps);
  }

  function renderPruneBox(areaId, tree, item, ps) {
    const box = byId('prune-box');
    box.hidden = !tree.isLiving || !ps;
    if (box.hidden) return;
    byId('prune-lifts').textContent = speciesLabel(ps.speciesId) + ' · ' + ps.lifts + ' / ' + ps.maxLifts + ' lifts (' + ps.minIntervalYears + '-yr wait)';
    let status;
    if (ps.planState === 'PendingPrune') status = 'Pending Prune — no crown is cut until the approved plan is executed.';
    else if (ps.planState === 'ApprovedPrune') status = 'Approved Prune — the crown is cut only when the plan is executed.';
    else if (ps.eligible) status = 'Eligible now: next lift raises the crown base to ' + fixed(ps.nextTargetM, 1) + ' m.';
    else status = 'Not eligible: ' + ps.refusalReason + (ps.nextEligibleYear !== undefined && ps.nextEligibleYear !== null
      ? ' (next lift possible in year ' + ps.nextEligibleYear + ')' : '') + '.';
    byId('prune-status').textContent = status;
    const add = byId('prune-add');
    add.textContent = label('MultiAddToPrunePlan');
    add.hidden = Boolean(ps.planState);
    const remove = byId('prune-remove');
    remove.hidden = ps.planState !== 'PendingPrune';
    remove.textContent = 'Remove Prune from Work Plan';
    remove.setAttribute('aria-label', 'Remove the pending Prune of ' + tree.treeId);
    byId('prune-why').textContent = pruneAvailability(areaId, item, ps).why || label('MultiPrunePendingNote') + '.';
  }

  function pruneAvailability(areaId, item, ps) {
    if (!ps || !item) return { ok: false, why: 'Select a living tree.' };
    if (!ps.eligible) return { ok: false, why: 'Not eligible (engine): ' + ps.refusalReason + '.' };
    if (ps.planState) return { ok: false, why: 'Already in the Work Plan as a Prune.' };
    if (item.status === 'PendingFell' || item.status === 'ApprovedFell') return { ok: false, why: 'This tree has a Fell order; one order per tree.' };
    if (state.view.approved) return { ok: false, why: 'The plan is approved, so editing is locked. Cancel the approved plan to change it.' };
    return { ok: true, why: null };
  }

  function renderPruneGate() {
    const areaId = state.areaId;
    const id = selectedId();
    const ps = id ? pruneStatusOf(areaId, id) : null;
    byId('prune-add').disabled = !pruneAvailability(areaId, id ? itemOf(areaId, id) : null, ps).ok;
  }

  // ---- Competition and Fell draft -------------------------------------------------------------------------------------

  function competitionTag() { return JSON.stringify([state.view ? currentFence() : null, state.competitionTarget]); }
  function clearCompetition() {
    state.competitionTarget = null;
    state.competition = null;
    state.competitionTag = null;
    state.competitionRequest += 1;
  }

  async function refreshCompetition() {
    const target = state.competitionTarget;
    if (!state.view || !target || state.lock) return;
    const tag = competitionTag();
    const request = ++state.competitionRequest;
    state.competition = null;
    renderAll();
    const outcome = await call('api/multi/competition', { fence: currentFence(), areaId: target.areaId, treeId: target.treeId });
    if (request !== state.competitionRequest || tag !== competitionTag() || state.lock || state.busy) return;
    if (outcome.kind === 'ok') {
      state.competition = { areaId: outcome.payload.areaId, report: outcome.payload.competition };
      state.competitionTag = tag;
      const dropped = reconcileDrafts();
      renderAll();
      if (dropped) announce(dropped);
    } else {
      clearCompetition();
      if (outcome.kind === 'refused') applyPayload(outcome.payload, false);
      renderAll();
      byId('competition-content').append(el('p', { class: 'note' }, 'Competition unavailable or the view changed. Select the tree again.'));
    }
  }

  function inspectedCrop() {
    const comp = state.competition;
    if (!comp || state.lock) return null;
    const area = areaView(comp.areaId);
    return area && area.cropTreeIds.includes(comp.report.targetTreeId) && !comp.report.targetPlannedForFell ? comp : null;
  }

  function releaseReason() {
    const comp = inspectedCrop();
    return comp && comp.areaId === state.areaId && comp.report.targetTreeId === referenceIn(state.areaId)
      ? 'Competition release for retained Crop Tree ' + comp.report.targetTreeId + ' (operator intent)' : '';
  }

  function renderCompetition() {
    const box = byId('competition-content');
    const areaId = state.areaId;
    const id = selectedId();
    const report = currentReport();
    const parts = [];
    const more = [];
    let sub = '';
    if (report) {
      const chosen = new Set(draftOf(areaId));
      const planLocked = state.view.approved !== null;
      const frozenNow = state.lock !== null || state.batch !== null;
      sub = '· ' + (report.topFive.length < 5 ? report.topFive.length + ' of 5 · ' : '') +
        report.neighbours.length + ' within ' + report.cutoffM + ' m';
      if (report.topFive.length === 0) {
        parts.push(el('p', { class: 'comp-empty' }, 'No living competitor inside the ' + report.cutoffM + ' m cutoff.'));
      } else {
        parts.push(el('p', { class: 'comp-head', 'aria-hidden': 'true' }, el('span', null, ''), el('span', null, 'Tree · DBH · CI share'),
          el('span', null, 'Distance'), el('span', null, 'Hegyi CI')));
        parts.push(el('ol', { class: 'comp-rows', 'aria-label': 'Five strongest competitors of ' + report.targetTreeId },
          ...report.topFive.map((n, index) => {
            const why = blockedReason(areaId, n.treeId);
            const ticked = chosen.has(n.treeId) && !why;
            const disabled = Boolean(why) || planLocked || frozenNow;
            const input = el('input', { type: 'checkbox', 'data-draft': n.treeId, 'data-focus-key': 'draft:' + n.treeId,
              checked: ticked, disabled });
            const spTag = SPECIES_SHORT[speciesOf(n.treeId)] || 'S';
            const meta = spTag + ' · ' + fixed(n.dbhCm, 1) + ' cm · ' + fixed(n.sharePercent, 1) + '%';
            return el('li', { class: 'comp-row' + (ticked ? ' chosen' : '') + (why ? ' blocked' : '') },
              el('label', why ? { title: 'Competitor ' + (index + 1) + ' · ' + n.treeId + ' · ' + why + ' · ' + meta } : null, input,
                el('span', { class: 'comp-id' }, el('span', { class: 'rank' }, String(index + 1)), el('span', { class: 'mono' }, n.treeId)),
                el('span', { class: 'comp-num comp-dist' }, fixed(n.distanceM, 2) + ' m'),
                el('span', { class: 'comp-num comp-ci' }, fixed(n.contribution, 4), el('span', { class: 'comp-strength' }, n.strength)),
                el('span', { class: 'comp-line' },
                  why ? el('span', { class: 'comp-why', 'aria-hidden': 'true' }, shortBlockedReason(why)) : null,
                  why ? el('span', { class: 'sr-only' }, 'Cannot be chosen: ' + why + '. ') : null,
                  why ? el('span', { 'aria-hidden': 'true' }, ' · ') : null,
                  el('span', { class: 'comp-meta' }, meta))));
          })));
      }
      more.push(el('p', { class: 'competition-totals' }, 'Current total Hegyi CI ' + fixed(report.currentTotal, 4) +
        (report.targetPlannedForFell ? ' · This tree is planned for Fell: no release estimate.' :
          ' → ' + fixed(report.hypotheticalTotal, 4) + ' if its pending or approved neighbours were felled.')));
      more.push(el('p', { class: 'small-note' }, 'The planned figure counts only queued or approved orders, never unsent draft choices.'));
      if (isStage4()) more.push(el('p', { class: 'small-note' }, label('MultiCompetitionAreaOnly') + '.'));
      more.push(el('p', { class: 'small-note' }, 'Other neighbours: ' + report.remainingCount + ' · combined ' +
        fixed(report.remainingSharePercent, 1) + '% share.'));
    } else if (id && livingIn(areaId, id) && state.competitionTarget && !state.lock) {
      parts.push(el('p', { class: 'comp-empty' }, 'Reading competition…'));
    } else if (id && !livingIn(areaId, id)) {
      parts.push(el('p', { class: 'comp-empty' }, 'Competition is shown for living trees only.'));
    } else if (state.lock) {
      parts.push(el('p', { class: 'comp-empty' }, 'Re-read the current state to see competition again.'));
    }
    byId('competition-sub').textContent = sub;
    byId('competition-sub').hidden = sub === '';
    box.replaceChildren(...parts);
    byId('competition-more').replaceChildren(...more);
    byId('competition-details').hidden = more.length === 0;
    renderAddForm(report);
  }

  function renderAddForm(report) {
    const areaId = state.areaId;
    const form = byId('add-form');
    form.hidden = !report || report.topFive.length === 0;
    if (form.hidden) return;
    const count = draftOf(areaId).filter((id) => !blockedReason(areaId, id)).length;
    const availability = addAvailability();
    byId('draft-count').textContent = state.batch ? 'Adding — please wait' : plural(count, 'selected', 'selected') + ' · draft only';
    const notice = state.draftNotice && state.draftNotice.areaId === areaId ? state.draftNotice.text : '';
    byId('add-button').textContent = state.batch
      ? 'Adding ' + Math.min(state.batch.confirmed.length + 1, state.batch.ids.length) + ' of ' + state.batch.ids.length + '…'
      : count > 0 ? 'Add selected ' + count + ' to Fell plan' : 'Add selected to Fell plan';
    byId('add-caption').textContent = notice || (availability.ok || count === 0 && !state.view.approved && !state.lock
      ? 'Queues proposals only. Approve and Execute are separate.' : availability.why);
    renderReasonPicker();
  }

  function reasonToSend(forReferenceItself) {
    const note = byId('reason').value.trim();
    return note || (!forReferenceItself && byId('reason-release').checked ? releaseReason() : '') || DEFAULT_REASON;
  }

  function renderReasonPicker() {
    const release = releaseReason();
    const checkbox = byId('reason-release');
    checkbox.disabled = release === '' || state.batch !== null;
    if (release === '') checkbox.checked = false;
    byId('release-wrap').hidden = release === '';
    byId('release-label').textContent = release === '' ? '' :
      'Record this as competition release for retained Crop Tree ' + state.competition.report.targetTreeId;
    byId('reason').disabled = state.batch !== null;
    const reason = reasonToSend(false);
    const summary = reason.length > 42 ? reason.slice(0, 40) + '…' : reason;
    if (byId('reason-summary').textContent !== summary) byId('reason-summary').textContent = summary;
    const preview = 'Recorded reason for each tree added here: ' + reason;
    if (byId('reason-preview').textContent !== preview) byId('reason-preview').textContent = preview;
    showReasonError(null);
  }

  function showReasonError(message) {
    const box = byId('reason-error');
    box.hidden = message === null;
    box.textContent = message || '';
    byId('reason').setAttribute('aria-invalid', String(message !== null));
  }

  function resetReasonPicker() {
    byId('reason').value = '';
    byId('reason-release').checked = false;
    byId('reason-details').open = false;
    showReasonError(null);
  }

  function reconcileDrafts() {
    const notes = [];
    for (const areaId of Object.keys(state.drafts)) {
      const draft = state.drafts[areaId];
      if (!draft) continue;
      if (!livingIn(areaId, draft.ref)) {
        if (draft.ids.length > 0) notes.push('Cleared draft for ' + draft.ref + ' (no longer living).');
        delete state.drafts[areaId];
        continue;
      }
      const keep = draft.ids.filter((id) => !blockedReason(areaId, id));
      if (keep.length !== draft.ids.length) {
        const dropped = draft.ids.filter((id) => !keep.includes(id));
        notes.push('Dropped from draft (' + areaName(areaId) + '): ' + dropped.join(', ') + '.');
        draft.ids = keep;
      }
    }
    state.draftNotice = notes.length ? { areaId: state.areaId, text: notes.join(' ') } : null;
    return notes.join(' ');
  }

  function toggleDraft(treeId, on) {
    const areaId = state.areaId;
    const ref = referenceIn(areaId);
    if (!ref || state.batch || state.lock || state.view.approved) return;
    if (on && blockedReason(areaId, treeId)) return;
    if (state.batchResult) dismissBatchResult(false);
    const draft = state.drafts[areaId] && state.drafts[areaId].ref === ref ? state.drafts[areaId] : { ref, ids: [] };
    const had = draft.ids.includes(treeId);
    if (on && !had) draft.ids = draft.ids.concat(treeId);
    else if (!on && had) draft.ids = draft.ids.filter((id) => id !== treeId);
    state.drafts[areaId] = draft;
    state.draftNotice = null;
    renderAll();
    announce((on ? 'Added ' : 'Removed ') + treeId + (on ? ' to' : ' from') + ' the Fell draft (' + draft.ids.length + ' selected; not yet queued).');
  }

  function clearDraft() {
    const areaId = state.areaId;
    const ref = referenceIn(areaId);
    if (!ref || state.batch) return;
    const count = draftOf(areaId).length;
    if (count === 0) return;
    state.drafts[areaId] = { ref, ids: [] };
    state.draftNotice = null;
    renderAll();
    announce('Cleared ' + plural(count, 'draft choice', 'draft choices') + ' for ' + ref + '.');
  }

  async function addSequence(areaId, ref, ids, reason, kind) {
    const result = { areaId, ref, kind, ids: ids.slice(), reason, confirmed: [], refused: null, guard: null, unconfirmed: null, unattempted: [] };
    state.batch = result;
    state.batchResult = null;
    state.batchResultOpen = false;
    state.busy = true;
    document.documentElement.classList.add('busy');
    renderAll();
    let firstNotSent = ids.length;
    for (let index = 0; index < ids.length; index++) {
      const treeId = ids[index];
      const outcome = await call('api/multi/pending/add', { fence: currentFence(), areaId, treeId, reason });
      if (outcome.kind === 'ok') {
        result.confirmed.push(treeId);
        if (state.drafts[areaId] && state.drafts[areaId].ref === ref) {
          state.drafts[areaId].ids = state.drafts[areaId].ids.filter((id) => id !== treeId);
        }
        applyPayload(outcome.payload, false);
        continue;
      }
      firstNotSent = index + 1;
      if (outcome.kind === 'refused') {
        result.refused = { treeId, code: outcome.payload.refusal.code, message: outcome.payload.refusal.message };
        applyPayload(outcome.payload, false);
      } else if (outcome.kind === 'guard') {
        result.guard = { treeId, status: outcome.status, code: outcome.error.code, message: outcome.error.message };
      } else if (outcome.kind === 'locked') {
        firstNotSent = index;
      } else {
        result.unconfirmed = { treeId, detail: outcome.kind === 'unconfirmed' ? outcome.error.message : outcome.detail };
      }
      break;
    }
    result.unattempted = result.ids.slice(firstNotSent);
    state.batch = null;
    state.busy = false;
    document.documentElement.classList.remove('busy');
    state.batchResult = result;
    if (result.confirmed.length === result.ids.length) resetReasonPicker();
    if (result.unconfirmed) {
      lockCommands(label('MultiOutcomeUnconfirmed') || 'Outcome unconfirmed — re-read current state',
        'Adding ' + result.unconfirmed.treeId + ' to the Work Plan: no confirmed result was received.');
    } else {
      renderAll();
    }
    announce(batchSummary(result));
    if (!result.unconfirmed) {
      const add = byId('add-button');
      const rendered = typeof add.getClientRects !== 'function' || add.getClientRects().length > 0;
      if (add.disabled || add.closest('[hidden]') || !rendered) byId('batch-result').focus();
      else add.focus();
    }
    if (!state.lock && state.competitionTarget && state.competitionTag !== competitionTag()) refreshCompetition();
  }

  function batchSummary(result) {
    const n = result.ids.length;
    if (result.confirmed.length === n) {
      return 'Added ' + plural(n, 'tree', 'trees') + ' to the Fell plan as pending orders (not felled): ' + listIds(result.confirmed) +
        '. Total pending ' + state.view.pending.length + '.';
    }
    const stopped = result.refused ? 'refused ' + result.refused.treeId : result.guard ? 'refused ' + result.guard.treeId :
      result.unconfirmed ? 'no confirmed result for ' + result.unconfirmed.treeId : 'not sent';
    return 'Stopped after ' + result.confirmed.length + ' of ' + n + ': ' + stopped + '.';
  }

  function renderBatchResult() {
    const box = byId('batch-result');
    const result = state.batchResult;
    const show = result && result.areaId === state.areaId && result.ref === referenceIn(state.areaId);
    box.hidden = !show;
    byId('add-dock').classList.toggle('showing-result', Boolean(show));
    if (!show) { box.replaceChildren(); return; }
    const n = result.ids.length;
    const complete = result.confirmed.length === n;
    box.setAttribute('class', 'batch-result' + (complete ? '' : result.confirmed.length > 0 ? ' partial' : ' failed') +
      (state.batchResultOpen ? ' open' : ''));
    const stopped = result.refused ? ' · refused ' + result.refused.treeId : result.guard ? ' · refused ' + result.guard.treeId :
      result.unconfirmed ? ' · unconfirmed ' + result.unconfirmed.treeId : '';
    const title = complete ? 'Added ' + plural(n, 'tree', 'trees') + ' to the Fell plan' : 'Stopped after ' + result.confirmed.length + ' of ' + n + stopped;
    const head = el('div', { class: 'batch-head' }, el('p', { class: 'alert-title', title }, title),
      el('button', { type: 'button', class: 'link-button batch-more', 'data-batch': 'more', 'aria-expanded': String(Boolean(state.batchResultOpen)),
        'aria-controls': 'batch-body' }, state.batchResultOpen ? 'Hide details' : 'Details'),
      el('button', { type: 'button', class: 'link-button', 'data-batch': 'dismiss', 'aria-label': 'Dismiss the Add result' }, '× Dismiss'));
    const parts = [el('p', null, 'Added as pending (not felled): ', el('span', { class: 'mono' }, listIds(result.confirmed)))];
    if (result.refused) {
      parts.push(el('p', null, 'Refused: ', el('span', { class: 'mono' }, result.refused.treeId), ' — ',
        el('code', null, result.refused.code), ' ' + result.refused.message));
    }
    if (result.guard) {
      parts.push(el('p', null, 'Refused by server: ', el('span', { class: 'mono' }, result.guard.treeId), ' — ',
        el('code', null, result.guard.code), ' (HTTP ' + result.guard.status + ') ' + result.guard.message));
    }
    if (result.unconfirmed) {
      parts.push(el('p', null, 'Unconfirmed: ', el('span', { class: 'mono' }, result.unconfirmed.treeId)));
    }
    if (result.unattempted.length > 0) {
      parts.push(el('p', null, 'Not attempted: ', el('span', { class: 'mono' }, listIds(result.unattempted))));
    }
    box.replaceChildren(head, el('div', { id: 'batch-body', class: 'batch-body' }, ...parts));
  }

  function dismissBatchResult(focusAdd) {
    if (!state.batchResult) return;
    state.batchResult = null;
    state.batchResultOpen = false;
    renderBatchResult();
    if (focusAdd) {
      const add = byId('add-button');
      if (!add.disabled && !add.closest('[hidden]')) add.focus();
      else byId('inspector-heading').focus();
    }
  }

  function onBatchResultClick(event) {
    const button = event.target.closest('[data-batch]');
    if (!button) return;
    if (button.getAttribute('data-batch') === 'dismiss') {
      const added = state.batchResult ? state.batchResult.confirmed.length : 0;
      dismissBatchResult(true);
      announce('Dismissed the Add result.' + (added > 0 ? ' Orders already added stay pending in the Work Plan.' : ''));
      return;
    }
    state.batchResultOpen = !state.batchResultOpen;
    renderBatchResult();
    const again = byId('batch-result').querySelector('[data-batch="more"]');
    if (again) again.focus();
  }

  // ---- Work Plan, Receipt, Results, Trees, Details --------------------------------------------------------------------

  function statePill(approved, actionIconId, actionWord) {
    return el('span', { class: 'wp-pill ' + (approved ? 'wp-approved' : 'wp-pending') },
      useIcon(actionIconId, 15),
      approved ? ('✓ Approved ' + actionWord) : ('◷ Pending ' + actionWord));
  }

  function orderList(orders, approved, removable) {
    return el('ul', { class: 'list orders ' + (approved ? 'approved-orders' : 'pending-orders') }, ...orders.map((order) => el('li', null,
      el('span', { class: 'order-head' }, statePill(approved, 'ccf-action-fell', 'Fell'),
        el('span', { class: 'mono' }, order.treeId)),
      el('span', { class: 'order-reason' }, order.reason),
      removable ? el('button', { type: 'button', class: 'small', 'data-remove': order.treeId, 'data-area': order.areaId,
        'data-focus-key': 'remove:' + order.treeId, 'aria-label': label('MultiRemoveFromPlan') + ': ' + order.treeId },
      label('MultiRemoveFromPlan')) : null)));
  }

  function pruneList(orders, approved, removable) {
    return el('ul', { class: 'list orders prune-orders' }, ...orders.map((order) => el('li', null,
      el('span', { class: 'order-head' }, statePill(approved, 'ccf-action-prune', 'Prune'),
        el('span', { class: 'mono' }, order.treeId)),
      el('span', { class: 'order-reason' }, 'one lift · crown base to ' + fixed(order.targetCrownBaseHeightM, 1) + ' m' +
        (approved ? ' · cut on Execute' : ' · not cut')),
      removable ? el('button', { type: 'button', class: 'small', 'data-prune-remove': order.treeId, 'data-area': order.areaId,
        'data-focus-key': 'prune-remove:' + order.treeId, 'aria-label': 'Remove the pending Prune of ' + order.treeId },
      label('MultiRemoveFromPlan')) : null)));
  }

  // Read-only display of any engine-queued ClearUndergrowth / Plant / Shelter / SpotClear orders (zero Pass 2 write controls).
  function readOnlyEcoOrderList(clears, plantings, approved) {
    if (clears.length === 0 && plantings.length === 0) return null;
    const rows = [];
    for (const c of clears) {
      rows.push(el('li', null,
        el('span', { class: 'order-head' }, statePill(approved, 'ccf-action-clear', 'Clear'),
          el('span', { class: 'mono' }, 'Cell (' + c.column + ', ' + c.row + ')')),
        el('span', { class: 'order-reason' }, 'Whole 2.5 m cell · spares seedlings (read-only)')));
    }
    for (const p of plantings) {
      const icon = p.kind === 'Plant' ? 'ccf-action-plant' : p.kind === 'Shelter' ? 'ccf-action-shelter' : 'ccf-action-spot';
      rows.push(el('li', null,
        el('span', { class: 'order-head' }, statePill(approved, icon, p.kind),
          el('span', { class: 'mono' }, p.orderId)),
        el('span', { class: 'order-reason' }, (p.speciesId ? speciesLabel(p.speciesId) + ' · ' : '') +
          'X ' + fixed(p.xm, 1) + ', Z ' + fixed(p.zm, 1) + ' (read-only)')));
    }
    return el('ul', { class: 'list orders eco-orders' }, ...rows);
  }

  function workTotals(fells, prunes) {
    return [fells > 0 ? fells + ' Fell' : '', prunes > 0 ? prunes + ' Prune' : ''].filter(Boolean).join(' · ');
  }
  function perAreaCounts(orders) {
    return state.view.areas.map((area) => area.label + ' ' + ordersIn(orders, area.areaId).length).join(' · ');
  }

  function renderPlan() {
    const view = state.view;
    const plan = view.approved;
    const pendingP = pendingPrunes();
    const approvedP = approvedPrunes();
    const pendingC = state.ecology ? (state.ecology.pendingClear || []) : [];
    const approvedC = state.ecology ? (state.ecology.approvedClear || []) : [];
    const pendingPl = state.ecology ? (state.ecology.pendingPlanting || []) : [];
    const approvedPl = state.ecology ? (state.ecology.approvedPlanting || []) : [];
    const pendingCount = view.pending.length + pendingP.length + pendingC.length + pendingPl.length;
    byId('plan-heading').textContent = plan ? 'Approved work plan' : 'Work plan';
    byId('step-pending').className = !plan && pendingCount > 0 ? 'now' : '';
    byId('step-approved').className = plan ? 'now' : '';
    byId('plan-scope').textContent = label('MultiOnePlan') + '. ' + label('MultiLastLivingRefused') + '.';
    byId('readout-unavailable').hidden = readoutState() !== 'unavailable';
    byId('pending-box').hidden = plan !== null && pendingCount === 0;
    byId('pending-total').textContent = pendingCount === 0 ? 'No pending orders' :
      pendingP.length === 0 && pendingC.length === 0 && pendingPl.length === 0
        ? view.pending.length + ' pending Fell · not felled (' + perAreaCounts(view.pending) + ')'
        : (workTotals(view.pending.length, pendingP.length) || (pendingCount + ' orders')) + ' pending · nothing executed yet';
    byId('pending-areas').replaceChildren(...view.areas.map((area) => {
      const orders = ordersIn(view.pending, area.areaId);
      const prunes = ordersIn(pendingP, area.areaId);
      const clears = ordersIn(pendingC, area.areaId);
      const plants = ordersIn(pendingPl, area.areaId);
      const n = orders.length + prunes.length + clears.length + plants.length;
      return el('section', { class: 'plan-area', 'aria-label': 'Pending in ' + areaName(area.areaId) },
        el('h4', null, areaName(area.areaId) + ' · ' + plural(n, 'pending order', 'pending orders')),
        n === 0 ? el('p', { class: 'small-note' }, 'No pending orders in this area.') : null,
        orders.length === 0 ? null : orderList(orders, false, true),
        prunes.length === 0 ? null : pruneList(prunes, false, true),
        readOnlyEcoOrderList(clears, plants, false));
    }));
    const approve = byId('approve');
    approve.textContent = label('MultiApprove') + ' · ' + plural(pendingCount, 'order', 'orders');
    approve.hidden = pendingCount === 0 || plan !== null;
    byId('approved-empty').hidden = plan !== null;
    byId('approved-box').hidden = plan === null;
    if (plan === null) {
      byId('approved-areas').replaceChildren();
      byId('approved-total').textContent = '';
      return;
    }
    const approvedCount = plan.orders.length + approvedP.length + approvedC.length + approvedPl.length;
    byId('approved-total').textContent = approvedP.length === 0 && approvedC.length === 0 && approvedPl.length === 0
      ? plan.orders.length + ' approved · still standing'
      : (workTotals(plan.orders.length, approvedP.length) || (approvedCount + ' orders')) + ' approved · nothing executed yet';
    byId('approved-sub').textContent = 'Execution has not happened. Year remains ' + view.source.ecologicalYear + ' · approved in year ' + plan.source.ecologicalYear + '.';
    byId('approved-areas').replaceChildren(...view.areas.map((area) => {
      const orders = ordersIn(plan.orders, area.areaId);
      const prunes = ordersIn(approvedP, area.areaId);
      const clears = ordersIn(approvedC, area.areaId);
      const plants = ordersIn(approvedPl, area.areaId);
      const n = orders.length + prunes.length + clears.length + plants.length;
      return el('section', { class: 'plan-area', 'aria-label': 'Approved in ' + areaName(area.areaId) },
        el('h4', null, areaName(area.areaId) + ' · ' + (n === 0 ? 'no orders' : plural(n, 'order', 'orders'))),
        orders.length === 0 ? null : orderList(orders, true, false),
        prunes.length === 0 ? null : pruneList(prunes, true, false),
        readOnlyEcoOrderList(clears, plants, true));
    }), technicalPlan(plan.planId, plan.executionKey));
    byId('cancel-approved').textContent = label('MultiCancelApproved');
    byId('execute').textContent = readoutState() === 'unavailable' ? label('MultiExecute') + ' · re-read needed' :
      label('MultiExecute') + ' · ' + plural(approvedCount, 'order', 'orders');
  }

  function renderReceipt() {
    const view = state.view;
    const parts = [];
    if (view.events.length === 0) {
      parts.push(el('p', { class: 'small-note' }, 'No work has been executed in this session.'));
    } else {
      const event = view.events[view.events.length - 1];
      if (state.lastWasReplay) parts.push(el('p', { class: 'warning' }, label('MultiAlreadyExecuted')));
      const seen = state.lastExecution;
      const revision = seen && seen.executionKey === event.executionKey ? seen.workRevisionBefore + ' → ' + seen.workRevisionAfter : String(event.workRevision);
      parts.push(el('p', null, 'Completed · ecological year ' + event.ecologicalYear + ' · work revision ' + revision + ' · ' +
        event.areas.reduce((sum, change) => sum + change.fells.length, 0) + ' trees felled'));
      parts.push(table('Receipt per area (actual before → actual after)', ['Area', 'Living trees', 'Basal area (m²/ha)', 'Felled (recorded reason)'],
        event.areas.map((change) => [areaShort(change.areaId), change.before.livingCount + ' → ' + change.after.livingCount,
          fixed(change.before.basalAreaM2PerHa, 2) + ' → ' + fixed(change.after.basalAreaM2PerHa, 2),
          change.fells.length === 0 ? 'none' : change.fells.map((fell) => fell.treeId + ' (' + fell.cause + '; ' + fell.reason + ')').join('; ')])));
      const pruned = state.stage4 ? state.stage4.pruneLog.filter((record) => record.executionKey === event.executionKey) : [];
      if (pruned.length > 0) {
        parts.push(table('Pruned in this Execute (engine record)', ['Tree', 'Species', 'Crown radius (m)', 'Crown base', 'Lifts'],
          pruned.map((record) => {
            const ps = pruneStatusOf(record.areaId, record.treeId);
            const maxLifts = ps ? ps.maxLifts : 3;
            return [record.treeId, speciesLabel(speciesOf(record.treeId)), fixed(record.crownRadiusBeforeM, 3) + ' → ' + fixed(record.crownRadiusAfterM, 3),
              fixed(record.targetCrownBaseHeightM, 1) + ' m', record.liftsAfter + ' / ' + maxLifts];
          })));
      }
      parts.push(technicalPlan(event.planId, event.executionKey));
    }
    byId('receipt').replaceChildren(...parts);
  }

  function renderHistory() {
    const view = state.view;
    const years = view.annualHistory;
    const latest = byId('year-latest');
    if (years.length === 0) {
      latest.replaceChildren(el('p', { class: 'small-note' }, 'No ecological year has been advanced in this session.'));
    } else {
      const record = years[years.length - 1];
      latest.replaceChildren(
        el('p', null, 'Latest year ' + record.yearBefore + ' → ' + record.yearAfter + (record.cancelledUnexecutedWork ?
          ' · unexecuted work in both areas was cancelled at zero Fell.' : '.')),
        table('Year ' + record.yearBefore + ' → ' + record.yearAfter + ' per area', ['Area', 'Living trees', 'Basal area (m²/ha)', 'Natural deaths'],
          record.areas.map((year) => [areaShort(year.areaId), year.before.livingCount + ' → ' + year.after.livingCount,
            fixed(year.before.basalAreaM2PerHa, 2) + ' → ' + fixed(year.after.basalAreaM2PerHa, 2),
            year.noLivingStems ? label('MultiAreaNoLivingStems') : year.naturalDeaths.length === 0 ? label('MultiNoNaturalDeaths') :
              year.naturalDeaths.map((death) => death.treeId + ' (' + speciesLabel(speciesOf(death.treeId)) + ', ' + death.mechanism + ')').join('; ')])));
    }
    const history = [];
    if (years.length > 0) {
      history.push(table('Every committed year, per area', ['Year', 'Area', 'Living trees', 'Basal area (m²/ha)', 'Natural deaths'],
        years.flatMap((record) => record.areas.map((year) => [record.yearBefore + ' → ' + record.yearAfter, areaShort(year.areaId),
          year.before.livingCount + ' → ' + year.after.livingCount,
          fixed(year.before.basalAreaM2PerHa, 2) + ' → ' + fixed(year.after.basalAreaM2PerHa, 2),
          year.noLivingStems ? 'no living stems' : String(year.naturalDeaths.length)]))));
    }
    if (view.events.length > 0) {
      history.push(el('p', null, el('span', { class: 'wp-pill wp-completed' }, '☑ Completed · History Log')));
      history.push(table('Executed work events', ['Year', 'Work revision', 'Felled per area'],
        view.events.map((event) => [String(event.ecologicalYear), String(event.workRevision),
          event.areas.map((change) => change.areaId + ': ' + (change.fells.length === 0 ? 'none' : change.fells.map((fell) => fell.treeId).join(', '))).join(' · ')])));
    }
    byId('history').replaceChildren(...history);
  }

  function renderTable() {
    const area = currentArea();
    if (!area) return;
    const query = byId('tree-search').value;
    const rows = [];
    for (const tree of area.trees) {
      const item = itemOf(area.areaId, tree.treeId);
      if (!item || (query !== '' && !tree.treeId.includes(query))) continue;
      const selected = tree.treeId === selectedId();
      rows.push(el('tr', { class: selected ? 'current' : null, 'aria-current': selected ? 'true' : null },
        el('td', null, el('button', { type: 'button', class: 'small', 'data-select': tree.treeId, 'data-focus-key': 'select:' + tree.treeId,
          'aria-label': (selected ? 'Selected ' : 'Select ') + tree.treeId }, selected ? 'Selected' : 'Select')),
        el('th', { scope: 'row', class: 'mono' }, tree.treeId),
        el('td', null, speciesLabel(speciesOf(tree.treeId))),
        el('td', null, statusText(item)),
        el('td', null, fixed(tree.dbhCm, 1)),
        el('td', null, fixed(tree.heightM, 1)),
        el('td', null, fixed(tree.crownRadiusM, 2)),
        el('td', null, fixed(tree.xm, 2)),
        el('td', null, fixed(tree.zm, 2))));
    }
    byId('tree-rows').replaceChildren(...rows);
    byId('tree-caption').textContent = 'Every tree of ' + areaName(area.areaId) + ', with its exact ID and species';
    byId('rows-shown').textContent = rows.length + ' rows shown for ' + area.label + (query !== '' ? ' (filtered)' : '');
  }

  function renderDetails() {
    const view = state.view;
    const e = view.experiment;
    const s = view.source;
    const readout = readoutState();
    const statements = (readout === 'phase2' || readout === 'stage4')
      ? STATEMENTS.map((name) => (name === 'MultiEdgeExcluded' ? 'MultiConnectedWoodland' : name))
      : readout === 'legacy' ? STATEMENTS : STATEMENTS.filter((name) => name !== 'MultiEdgeExcluded');
    byId('statements').replaceChildren(
      el('li', null, experimentBadgeText()),
      el('li', null, e.disclosure),
      ...(readout === 'unavailable' ? [el('li', null, 'Model read-out unavailable for this view. Choose Re-read.')] : []),
      ...statements.concat(['MultiReviewStatus', 'MultiEmptyAreaRule'])
        .map((name) => label(name)).filter((text) => text !== '').map((text) => el('li', null, text)));
    const m = state.stage4 ? state.stage4.models : null;
    const em = state.ecology ? state.ecology.models : null;
    byId('stage4-statements').replaceChildren(...(m ? [
      'Models: ' + (em ? [em.adult, em.mortality, em.crown, em.regeneration, em.species].join('/') : [m.adultModelVersion, m.mortalityModelVersion, m.crownModelVersion].join('/')) +
        ' · light ' + m.lightModelVersion + ' (' + fixed(m.lightCellM, 1) + ' m cells, shade buffer b = ' + fixed(m.shadeBufferM, 1) + ' m)',
      'Modelled light · not calibrated PAR — ' + m.notCalibratedCaveat,
      'Fallen deadwood: ' + label('MultiDeadwoodNote') + ' (direction schematic, not simulated).',
    ].concat(m.evidenceLabels, m.lightUncertainties) : []).map((text) => el('li', null, text)));
    byId('details-note').textContent = readout === 'phase2' ? label('MultiNotModelledPhase2') :
      readout === 'stage4' ? label('MultiNotModelledStage4') :
        readout === 'legacy' ? 'Light, seed, regeneration, site, money and saving: NOT MODELLED.' :
          'Model read-out unavailable for this view; choose Re-read to see what the active engine models.';
    byId('facts').replaceChildren(
      pair('Experiment', e.experimentId), pair('Badge', experimentBadgeText(), 'wide'),
      pair('Format', e.format + ' v' + e.schemaVersion), pair('Domain', e.domain),
      pair('Source class', e.sourceClass), pair('Manifest content hash', expandable(e.manifestContentHash)),
      pair('Generator', e.generatorId + ' v' + e.generatorVersion + ', seed ' + e.generatorSeed),
      pair('Review status', e.reviewStatus), pair('Edge model', e.edgeModel, 'wide'), pair('Site', e.siteNote, 'wide'),
      ...view.areas.map((area) => pair(area.label + ' (' + area.areaId + ')', area.areaM2Exact + ' m² (' + fixed(area.areaHa, 4) + ' ha) · seed ' +
        area.simulationSeed + ' · ' + area.trees.length + ' tree records')),
      pair('Models', 'RNG v' + s.rngModelVersion + ' · growth model v' + s.growthModelVersion + ' · rules ' + s.p1AlgorithmVersion +
        ' · ' + s.areaModelVersion, 'wide'),
      pair('Ecological year', s.ecologicalYear), pair('Annual revision', s.annualRevision),
      pair('Work revision', s.workRevision), pair('Plan-state revision', view.planStateRevision),
      pair('Session', expandable(view.sessionId)));
  }

  function renderPaneButtons() {
    const view = state.view;
    const counts = { plan: view.pending.length + (view.approved ? view.approved.orders.length : 0) + pendingPrunes().length +
      approvedPrunes().length, history: view.annualHistory.length };
    for (const button of document.querySelectorAll('[data-pane-button]')) {
      const name = button.getAttribute('data-pane-button');
      const wanted = PANE_NAMES[name] + (name in counts ? '|' + counts[name] : '');
      if (button.getAttribute('data-text') === wanted) continue;
      button.setAttribute('data-text', wanted);
      button.replaceChildren(PANE_NAMES[name], ...(name in counts ? [' ', el('span', { class: 'count' }, String(counts[name]))] : []));
    }
  }

  function showPane(name) {
    if (!PANES.includes(name)) return;
    byId('side').setAttribute('data-pane', name);
    for (const button of document.querySelectorAll('[data-pane-button]')) {
      button.setAttribute('aria-pressed', String(button.getAttribute('data-pane-button') === name));
    }
  }

  function renderMapControls() {
    const area = currentArea();
    if (!area) return;
    const fit = state.zoom === ZOOMS[0];
    const box = viewBox(area);
    byId('zoom-in').disabled = state.zoom === ZOOMS[ZOOMS.length - 1];
    byId('zoom-out').disabled = fit;
    byId('pan-north').disabled = fit || !box.north;
    byId('pan-south').disabled = fit || !box.south;
    byId('pan-west').disabled = fit || !box.west;
    byId('pan-east').disabled = fit || !box.east;
  }

  function zoomMap(step) {
    const area = currentArea();
    if (!area) return;
    const index = ZOOMS.indexOf(state.zoom);
    const next = ZOOMS[Math.min(ZOOMS.length - 1, Math.max(0, index + step))];
    if (next === state.zoom) return;
    if (state.zoom === ZOOMS[0]) {
      const item = selectedId() ? itemOf(area.areaId, selectedId()) : null;
      state.centre = item ? { u: item.u, v: item.v } : null;
    } else {
      state.centre = usableCentre(viewBox(area));
    }
    state.zoom = next;
    if (next === ZOOMS[0]) state.centre = null;
    else state.centre = usableCentre(viewBox(area));
    renderMap();
    renderMapControls();
  }

  function panMap(east, south) {
    const area = currentArea();
    if (!area || state.zoom === ZOOMS[0]) return;
    const box = viewBox(area);
    const centre = usableCentre(box);
    state.centre = { u: centre.u + east * box.uw / 4, v: centre.v + south * box.uh / 4 };
    state.centre = usableCentre(viewBox(area));
    renderMap();
    renderMapControls();
  }

  // ---- selection & Escape precedence ----------------------------------------------------------------------------------

  function waitForBatch() { announce('Please wait: the selected trees are still being added to the Fell plan.'); }

  function showArea(areaId) {
    if (!areaView(areaId) || areaId === state.areaId) return;
    if (state.batch) { waitForBatch(); return; }
    state.mapPick += 1;
    state.cellPick += 1;
    state.areaId = areaId;
    state.zoom = ZOOMS[0];
    state.centre = null;
    state.overlap = null;
    state.cellDetail = null;
    state.cellLoading = false;
    resetReasonPicker();
    const id = selectedId();
    if (id && livingIn(areaId, id)) {
      state.competitionTarget = { areaId, treeId: id };
      renderAll();
      refreshCompetition();
    } else {
      clearCompetition();
      renderAll();
    }
    const drafts = draftOf(areaId).length;
    announce('Showing ' + areaName(areaId) + '.' + (id ? ' Selected tree ' + id + '.' : '') +
      (drafts ? ' ' + plural(drafts, 'draft choice', 'draft choices') + ' kept for this reference.' : ''));
  }

  function chooseTree(areaId, treeId, fromList) {
    if (!treeIn(areaId, treeId)) return;
    if (state.batch) { waitForBatch(); return; }
    state.mapPick += 1;
    if (state.mapMode === 'cell') state.mapMode = 'tree';
    if (areaId !== state.areaId) { state.areaId = areaId; state.zoom = ZOOMS[0]; state.centre = null; }
    let cleared = '';
    const draft = state.drafts[areaId];
    if (draft && draft.ref !== treeId) {
      if (draft.ids.length > 0) cleared = ' Cleared ' + plural(draft.ids.length, 'unsent draft choice', 'unsent draft choices') +
        ' for the previous reference ' + draft.ref + '.';
      delete state.drafts[areaId];
    }
    if (state.batchResult && state.batchResult.areaId === areaId && state.batchResult.ref !== treeId) state.batchResult = null;
    state.draftNotice = cleared ? { areaId, text: cleared.trim() } : null;
    state.selected[areaId] = treeId;
    if (fromList) state.overlap = null;
    resetReasonPicker();
    if (livingIn(areaId, treeId)) state.competitionTarget = { areaId, treeId };
    else clearCompetition();
    showPane('tree');
    renderAll();
    byId('pane-tree').scrollTop = 0;
    announce('Selected tree ' + treeId + ' (' + speciesLabel(speciesOf(treeId)) + ') in ' + areaName(areaId) + '.' + cleared);
    if (state.competitionTarget) refreshCompetition();
  }

  async function onMapClick(event) {
    if (state.batch) { waitForBatch(); return; }
    if (!state.view || refusedByLock('Map selection') || state.busy) return;
    const area = currentArea();
    const matrix = byId('map').getScreenCTM();
    if (!area || !matrix) return;
    const point = new DOMPoint(event.clientX, event.clientY).matrixTransform(matrix.inverse());

    // Inspect Cell mode: click queries POST api/multi/cell cleanly without altering selection or Fell draft.
    if (state.mapMode === 'cell') {
      await inspectCellAtPoint(area.areaId, point.x, point.y);
      return;
    }

    // Select Tree mode (default): crown clicks win via POST api/multi/hit.
    const pick = ++state.mapPick;
    state.busy = true;
    const outcome = await call('api/multi/hit', { fence: currentFence(), areaId: area.areaId, u: point.x, v: point.y });
    state.busy = false;
    if (pick !== state.mapPick) {
      if (state.competitionTarget && !state.competition && !state.lock && !state.batch) refreshCompetition();
      return;
    }
    if (outcome.kind === 'ok') {
      clearAlert();
      const candidates = outcome.payload.candidates;
      if (candidates.length === 0) {
        state.overlap = null;
        applyPayload(outcome.payload, false);
        announce('No crown at this point — the selection is unchanged.');
        return;
      }
      state.overlap = candidates.length > 1 ? { areaId: area.areaId, candidates } : null;
      applyPayload(outcome.payload, true);
      chooseTree(area.areaId, candidates[0].treeId, false);
      if (candidates.length > 1) announce('Selected tree ' + candidates[0].treeId + '. ' + fill(label('CrownsOverlap'), { n: candidates.length }) + '.');
    } else if (outcome.kind === 'refused') {
      applyPayload(outcome.payload, false);
      if (state.lock) showLockAlert('Refused: Map selection (' + outcome.payload.refusal.code + '). Nothing was selected.');
      else showRefusal('Map selection', outcome.payload.refusal);
    } else if (outcome.kind === 'guard') {
      showGuardRefusal('Map selection', outcome.status, outcome.error);
    } else if (outcome.kind === 'locked') {
      showLockAlert('Not sent: Map selection.');
    } else {
      showAlert([el('p', { class: 'alert-title' }, 'Map selection could not be checked with the local session; nothing was selected.'), rereadButton()]);
    }
  }

  function closeInspection() {
    state.mapPick += 1;
    const areaId = state.areaId;
    const id = selectedId();
    if (!id) return false;
    if (state.batch) { waitForBatch(); return true; }
    const drafts = draftOf(areaId).length;
    delete state.selected[areaId];
    delete state.drafts[areaId];
    if (state.batchResult && state.batchResult.areaId === areaId) state.batchResult = null;
    state.draftNotice = null;
    state.overlap = null;
    clearCompetition();
    resetReasonPicker();
    renderAll();
    byId('map-heading').focus();
    announce('Closed the inspection of ' + id + '.' + (drafts ? ' Cleared ' + plural(drafts, 'unsent draft choice', 'unsent draft choices') + '.' : ''));
    return true;
  }

  // Escape precedence (spec §5.7 + Stage 4 R3 F-05):
  // 1) open modal dialog closes itself (leaves pending map hit-test alone);
  // 2) non-empty search box clears itself;
  // 3) Inspect Cell mode exits to Select Tree mode WITHOUT clearing draft Fell ticks;
  // 4) tree inspection closes. Ground overlay is never cleared by Escape.
  function onEscape(event) {
    if (event.key !== 'Escape') return;
    if (document.querySelector('dialog[open]')) return;
    state.mapPick += 1;
    state.cellPick += 1;
    if (event.defaultPrevented) return;
    const target = event.target;
    if (target && target.localName === 'input' && target.getAttribute('type') === 'search' && target.value !== '') return;
    if (state.mapMode === 'cell') {
      event.preventDefault();
      setMapMode('tree', true);
      return;
    }
    if (closeInspection()) event.preventDefault();
  }

  function exactIdKey(inputId) {
    return (event) => {
      if (event.key !== 'Enter') return;
      event.preventDefault();
      const typed = byId(inputId).value;
      const area = state.view ? areaOfTree(typed) : null;
      if (area) {
        clearAlert();
        chooseTree(area.areaId, typed, true);
        if (!state.batch) byId('inspector-heading').focus();
      } else {
        showAlert([el('p', { class: 'alert-title' }, 'No exact match — the selection is unchanged'),
          el('p', null, 'No tree in either area has the exact case-sensitive ID "' + typed + '".')], byId(inputId));
      }
    };
  }

  // ---- Stage 4 commands (Fell, Prune, Crop, Approve, Cancel, Execute, Year +1; zero Pass 2 writes) --------------------

  function onCrop() {
    const areaId = state.areaId;
    const treeId = selectedId();
    if (!treeId || !state.view || state.batch) return;
    const marked = areaView(areaId).cropTreeIds.includes(treeId);
    run(marked ? 'api/multi/crop/unmark' : 'api/multi/crop/mark', { fence: currentFence(), areaId, treeId },
      label(marked ? 'MultiUnmarkCrop' : 'MultiMarkCrop') + ' (' + treeId + ')',
      () => announce((marked ? 'Removed Crop designation: ' : 'Retained Crop Tree: ') + treeId + ' in ' + areaName(areaId) + '.'));
  }

  function onAdd(event) {
    event.preventDefault();
    const areaId = state.areaId;
    const reference = referenceIn(areaId);
    const report = currentReport();
    if (!state.view || state.busy || state.batch || !reference || !report) return;
    if (refusedByLock('Add selected trees to the Work Plan')) return;
    if (!addAvailability().ok) return;
    const chosen = new Set(draftOf(areaId));
    const ids = report.topFive.map((n) => n.treeId).filter((id) => chosen.has(id) && !blockedReason(areaId, id));
    if (ids.length === 0) return;
    addSequence(areaId, reference, ids, reasonToSend(false), 'competitors');
  }

  function onAddReference() {
    const areaId = state.areaId;
    const treeId = selectedId();
    const item = treeId ? itemOf(areaId, treeId) : null;
    if (!state.view || state.busy || state.batch || !item || item.status !== 'Living' || item.isCrop) return;
    if (refusedByLock('Add ' + treeId + ' to the Work Plan')) return;
    if (state.view.approved) {
      showAlert([el('p', { class: 'alert-title' }, 'The plan is approved'), el('p', null, recoveryFor('WorkPlanAlreadyApproved'))]);
      return;
    }
    addSequence(areaId, referenceIn(areaId), [treeId], reasonToSend(true), 'reference');
  }

  function removePending(areaId, treeId) {
    if (!areaId || !treeId || state.batch) return;
    run('api/multi/pending/remove', { fence: currentFence(), areaId, treeId }, 'Remove ' + treeId + ' from the Work Plan',
      () => announce('Done: ' + label('MultiRemoveFromPlan') + ' (' + treeId + ').'), 'plan-heading');
  }

  function onPruneAdd() {
    const areaId = state.areaId;
    const treeId = selectedId();
    const ps = pruneStatusOf(areaId, treeId);
    if (!state.view || state.busy || state.batch || !treeId || !ps) return;
    if (refusedByLock('Add ' + treeId + ' to the Prune plan')) return;
    if (!pruneAvailability(areaId, itemOf(areaId, treeId), ps).ok) return;
    run('api/multi/prune/add', { fence: currentFence(), areaId, treeId }, 'Add ' + treeId + ' to the Prune plan', (payload) => {
      const order = payload.stage4 ? payload.stage4.pendingPrunes.find((o) => o.treeId === treeId) : null;
      announce('Done: ' + treeId + ' added as a pending Prune' + (order ? ' (crown base to ' + fixed(order.targetCrownBaseHeightM, 1) + ' m)' : '') +
        '. No crown was cut; Approve and Execute are separate.');
    }, 'prune-remove');
  }

  function removePrune(areaId, treeId) {
    if (!areaId || !treeId || state.batch) return;
    run('api/multi/prune/remove', { fence: currentFence(), areaId, treeId }, 'Remove the pending Prune of ' + treeId,
      () => announce('Done: pending Prune of ' + treeId + ' removed. Nothing was cut.'), 'plan-heading');
  }

  function onApprove() {
    run('api/multi/plan/approve', { fence: currentFence() }, label('MultiApprove'), (payload) => {
      const prunes = payload.stage4 ? payload.stage4.approvedPrunes.map((o) => o.treeId) : [];
      announce('Done: ' + label('MultiApprove') + '. Approved: ' + payload.view.approved.orders.map((o) => o.treeId).concat(prunes).join(', ') +
        '. Nothing was felled or cut.');
    }, 'plan-heading');
  }

  function onCancelApproved() {
    run('api/multi/plan/cancel', { fence: currentFence() }, label('MultiCancelApproved'),
      () => announce('Done: ' + label('MultiCancelApproved') + '. Its orders were discarded, not returned to pending.'), 'plan-heading');
  }

  function openDialog(dialog, initialFocus) { state.opener = document.activeElement; dialog.showModal(); initialFocus.focus(); }
  function returnFocus() {
    const opener = state.opener;
    state.opener = null;
    if (byId('alert').contains(document.activeElement)) return;
    if (opener && opener.isConnected && !opener.closest('[hidden]') && !opener.disabled) opener.focus();
  }

  function openExecuteDialog() {
    const view = state.view;
    if (!view || !view.approved || state.batch || refusedByLock(label('MultiExecute'))) return;
    if (readoutState() === 'unavailable') {
      showAlert([el('p', { class: 'alert-title' }, 'Execute needs a re-read'),
        el('p', null, 'The engine read-out for this view could not be paired with it, so the approved plan cannot be confirmed exactly. Nothing was sent.'),
        rereadButton()]);
      return;
    }
    const confirmation = confirmationOf(view);
    state.confirmation = confirmation;
    const n = confirmation.orders.length;
    const m = confirmation.prunes.length;
    const areas = new Set(confirmation.orders.map((order) => order.areaId)).size;
    byId('execute-title').textContent = label('MultiExecute');
    const fellText = n === 0 ? '' : fill(label(n === 1 ? 'MultiExecuteConfirmationOneTree' : 'MultiExecuteConfirmation'),
      { n, a: areas, y: confirmation.year }) + '.';
    const pruneText = m === 0 ? '' : n === 0 ? fill(label('MultiExecutePruneConfirmation'), { n: m, y: confirmation.year })
      .replace('named trees', m === 1 ? 'named tree' : 'named trees') + '.'
      : 'It also prunes exactly ' + plural(m, 'named tree', 'named trees') + ' (one lift each) in the same all-or-nothing Execute.';
    byId('execute-text').textContent = [fellText, pruneText].filter(Boolean).join(' ');
    byId('execute-confirm').textContent = n > 0 && m > 0 ? 'Confirm felling and pruning' : m > 0 ? 'Confirm pruning' : 'Confirm felling';
    byId('execute-areas').replaceChildren(...view.areas.map((area) => {
      const orders = confirmation.orders.filter((order) => order.areaId === area.areaId);
      const prunes = confirmation.prunes.filter((order) => order.areaId === area.areaId);
      return el('section', { class: 'plan-area' }, el('h3', null, areaName(area.areaId) + ' — ' + (orders.length + prunes.length)),
        orders.length === 0 && prunes.length === 0 ? el('p', { class: 'note' }, 'No Fell in this area.') : null,
        orders.length === 0 ? null : orderList(orders, true, false),
        prunes.length === 0 ? null : pruneList(prunes, true, false));
    }));
    byId('execute-plan').replaceChildren(label('MultiLastLivingRefused') + '. The ecological year does not change. ',
      technicalPlan(confirmation.planId, confirmation.executionKey));
    openDialog(byId('execute-dialog'), byId('execute-back'));
  }

  function onExecuteConfirmed() {
    const confirmation = state.confirmation;
    if (state.busy && confirmation) { announce('Still waiting for the local session process. Nothing was sent.'); return; }
    state.confirmation = null;
    byId('execute-dialog').close();
    if (!confirmation || refusedByLock(label('MultiExecute'))) return;
    if (!confirmationStillShown(confirmation, state.view)) { withdrawConfirmation(confirmation); return; }
    const request = {
      fence: { sessionId: confirmation.sessionId, source: confirmation.source, planStateRevision: confirmation.planStateRevision },
      planId: confirmation.planId,
      executionKey: confirmation.executionKey,
    };
    run('api/multi/plan/execute', request, label('MultiExecute'), (payload) => {
      const execution = payload.execution;
      state.lastExecution = { executionKey: execution.executionKey, workRevisionBefore: request.fence.source.workRevision,
        workRevisionAfter: execution.workRevision };
      renderReceipt();
      if (execution.replayed) { announce(label('MultiAlreadyExecuted') + '.'); return; }
      announce('Done: ' + label('MultiExecute') + '. Completed · History Log updated.');
    }, 'receipt-heading');
    showPane('plan');
  }

  function sendAdvance(consent) {
    run('api/multi/year/advance', { fence: currentFence(), consentToCancelWork: consent }, label('MultiAdvanceYear'), (payload) => {
      const drafts = draftTotal();
      state.drafts = {};
      state.draftNotice = null;
      state.batchResult = null;
      state.cellDetail = null;
      renderAll();
      const record = payload.view.annualHistory[payload.view.annualHistory.length - 1];
      announce('Done: year ' + record.yearBefore + ' → ' + record.yearAfter + '.' +
        (drafts ? ' ' + plural(drafts, 'unsent draft choice was', 'unsent draft choices were') + ' cleared.' : ''));
      showPane('history');
    }, 'year-heading');
  }

  function onAdvance() {
    const view = state.view;
    if (!view || state.batch || refusedByLock(label('MultiAdvanceYear'))) return;
    const work = view.pending.length > 0 || view.approved !== null || pendingPrunes().length > 0;
    const drafts = draftTotal();
    if (work || drafts > 0) openConsentDialog(work, drafts);
    else sendAdvance(false);
  }

  function openConsentDialog(work, drafts) {
    const view = state.view;
    state.consentWork = work;
    byId('consent-title').textContent = label('MultiAdvanceYear');
    byId('consent-text').textContent = work ? 'The Work Plan has unexecuted work. Advancing the year needs your explicit choice; cancelling ' +
      'removes ALL pending and approved orders in BOTH areas and fells nothing.' : 'The Work Plan has no unexecuted work. Advancing the year fells nothing.';
    byId('consent-areas').replaceChildren(...(work ? view.areas.map((area) => {
      const pending = ordersIn(view.pending, area.areaId).map((order) => order.treeId)
        .concat(ordersIn(pendingPrunes(), area.areaId).map((order) => order.treeId + ' (Prune)'));
      const approved = (view.approved ? ordersIn(view.approved.orders, area.areaId).map((order) => order.treeId) : [])
        .concat(ordersIn(approvedPrunes(), area.areaId).map((order) => order.treeId + ' (Prune)'));
      return el('p', null, areaName(area.areaId) + ': ' + (pending.length ? 'pending ' + pending.join(', ') : 'no pending orders') + '; ' +
        (approved.length ? 'approved ' + approved.join(', ') : 'no approved orders') + '.');
    }) : []));
    const draftsBox = byId('consent-drafts');
    draftsBox.hidden = drafts === 0;
    draftsBox.textContent = drafts === 0 ? '' : 'You also have ' + plural(drafts, 'unsent draft choice', 'unsent draft choices') +
      '. Draft choices are local selections, not orders; they will be cleared when the year advances.';
    byId('consent-keep').textContent = work ? fill(label('MultiConsentKeep'), { y: view.source.ecologicalYear }) :
      'Stay in year ' + view.source.ecologicalYear;
    byId('consent-advance').textContent = work ? label('MultiConsentCancel') : 'Discard draft choices and advance';
    openDialog(byId('consent-dialog'), byId('consent-keep'));
  }

  async function reread(announceIt) {
    if (state.busy) return;
    const lockCountAtStart = state.lockCount;
    state.busy = true;
    const outcome = await call('api/multi/view');
    state.busy = false;
    if (outcome.kind === 'ok') {
      clearAlert();
      state.lastWasReplay = false;
      applyPayload(outcome.payload, false);
      if (state.lock && state.lockCount === lockCountAtStart) {
        state.lock = null;
        renderAll();
        if (state.competitionTarget && !state.competition) refreshCompetition();
      }
      if (announceIt && !state.lock) {
        announce('Re-read: ecological year ' + state.view.source.ecologicalYear + ', work revision ' + state.view.source.workRevision +
          ', plan-state revision ' + state.view.planStateRevision + '.');
      }
    } else if (state.lock) {
      showLockAlert(outcome.kind === 'guard' ? 'The re-read was refused (' + outcome.error.code + '). Try again.' :
        'The re-read got no answer from the local session process. Try again.');
    } else if (outcome.kind === 'guard') {
      showGuardRefusal('Re-read', outcome.status, outcome.error);
    } else {
      showAlert([el('p', { class: 'alert-title' }, 'The local session process did not answer the read.'), rereadButton()]);
    }
  }

  function wire() {
    byId('area-switch').addEventListener('click', (event) => {
      const button = event.target.closest('[data-area]');
      if (button) showArea(button.getAttribute('data-area'));
    });
    byId('map').addEventListener('click', onMapClick);
    byId('mode-tree').addEventListener('click', () => setMapMode('tree', false));
    byId('mode-cell').addEventListener('click', () => setMapMode('cell', true));
    byId('cell-close').addEventListener('click', () => setMapMode('tree', true));
    byId('overlay-select').addEventListener('change', (event) => setOverlay(event.target.value));
    byId('crop-button').addEventListener('click', onCrop);
    byId('reread').addEventListener('click', () => reread(true));
    byId('advance').addEventListener('click', onAdvance);
    byId('view-plan').addEventListener('click', () => { showPane('plan'); byId('plan-heading').focus(); });
    byId('notice-provenance').addEventListener('click', () => { showPane('details'); byId('details-heading').focus(); });
    byId('rank-layer').addEventListener('click', onRankToggle);
    byId('inspect-close').addEventListener('click', closeInspection);
    byId('readout-reread').addEventListener('click', () => reread(true));
    document.documentElement.addEventListener('keydown', onEscape);
    byId('legend-hide').addEventListener('click', () => setLegendHidden(true, true));
    byId('legend-show').addEventListener('click', () => setLegendHidden(false, true));
    byId('competition-content').addEventListener('change', (event) => {
      const box = event.target.closest('[data-draft]');
      if (box) toggleDraft(box.getAttribute('data-draft'), box.checked);
    });
    byId('draft-clear').addEventListener('click', clearDraft);
    byId('batch-result').addEventListener('click', onBatchResultClick);
    byId('add-reference').addEventListener('click', onAddReference);
    byId('overlap-list').addEventListener('click', (event) => {
      const button = event.target.closest('[data-pick]');
      if (button) chooseTree(state.areaId, button.getAttribute('data-pick'), false);
    });
    byId('tree-rows').addEventListener('click', (event) => {
      const button = event.target.closest('[data-select]');
      if (button) {
        chooseTree(state.areaId, button.getAttribute('data-select'), true);
        if (!state.batch) byId('inspector-heading').focus();
      }
    });
    byId('tree-search').addEventListener('input', renderTable);
    byId('tree-search').addEventListener('keydown', exactIdKey('tree-search'));
    byId('quick-id').addEventListener('keydown', exactIdKey('quick-id'));
    byId('reason-release').addEventListener('change', renderReasonPicker);
    byId('reason').addEventListener('input', renderReasonPicker);
    byId('add-form').addEventListener('submit', onAdd);
    byId('insp-remove').addEventListener('click', () => removePending(state.areaId, selectedId()));
    byId('pending-areas').addEventListener('click', (event) => {
      const rm = event.target.closest('[data-remove]');
      if (rm) { removePending(rm.getAttribute('data-area'), rm.getAttribute('data-remove')); return; }
      const pr = event.target.closest('[data-prune-remove]');
      if (pr) removePrune(pr.getAttribute('data-area'), pr.getAttribute('data-prune-remove'));
    });
    byId('prune-add').addEventListener('click', onPruneAdd);
    byId('prune-remove').addEventListener('click', () => removePrune(state.areaId, selectedId()));
    byId('approve').addEventListener('click', onApprove);
    byId('cancel-approved').addEventListener('click', onCancelApproved);
    byId('execute').addEventListener('click', openExecuteDialog);
    byId('execute-back').addEventListener('click', () => { state.confirmation = null; byId('execute-dialog').close(); });
    byId('execute-confirm').addEventListener('click', onExecuteConfirmed);
    byId('execute-dialog').addEventListener('cancel', () => { state.confirmation = null; });
    byId('execute-dialog').addEventListener('close', () => {
      if (!byId('execute-dialog').open) state.confirmation = null;
      returnFocus();
    });
    byId('consent-keep').addEventListener('click', () => {
      byId('consent-dialog').close();
      announce(label('MultiNothingChanged') + ' — still year ' + state.view.source.ecologicalYear + '.');
    });
    byId('consent-advance').addEventListener('click', () => { byId('consent-dialog').close(); sendAdvance(state.consentWork); });
    byId('consent-dialog').addEventListener('close', returnFocus);
    document.querySelector('.view-tabs').addEventListener('click', (event) => {
      const button = event.target.closest('[data-pane-button]');
      if (button) showPane(button.getAttribute('data-pane-button'));
    });
    byId('skip-trees').addEventListener('click', () => showPane('trees'));
    byId('zoom-in').addEventListener('click', () => zoomMap(1));
    byId('zoom-out').addEventListener('click', () => zoomMap(-1));
    byId('zoom-fit').addEventListener('click', () => { state.zoom = ZOOMS[0]; state.centre = null; renderMap(); renderMapControls(); });
    byId('pan-north').addEventListener('click', () => panMap(0, -1));
    byId('pan-south').addEventListener('click', () => panMap(0, 1));
    byId('pan-west').addEventListener('click', () => panMap(-1, 0));
    byId('pan-east').addEventListener('click', () => panMap(1, 0));
    let frame = 0;
    const reframe = () => {
      if (frame) return;
      frame = window.requestAnimationFrame(() => { frame = 0; if (state.view && state.labels) { renderMap(); renderMapControls(); } });
    };
    if (typeof ResizeObserver === 'function') new ResizeObserver(reframe).observe(byId('map-pane'));
    else window.addEventListener('resize', reframe);
  }

  initFloorDefs();
  renderLegend();
  wire();
  reread(false);
})();
""";
}
