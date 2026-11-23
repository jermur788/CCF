using System.Reflection;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using SitkaP1.Core;

namespace SitkaP1.Ui;

/// <summary>
/// The MA-1 two-area simulator (owner-approved MA-1 spec §4, Multi #885): its own page and JSON routes under the same
/// /s/&lt;capability&gt;/ prefix of the same single listener. Every request still passes the unchanged guards of
/// <see cref="HandleAsync"/> first (Host, capability, route and method, Origin), and every POST the same content-type and
/// body-size checks. Each endpoint is one call into <see cref="MultiAreaWorkPlanAdapter"/>; this part keeps no tree, plan,
/// revision or session state of its own, and adds no CORS header, listener or file read.
/// </summary>
/// <remarks>
/// Multi-area POST bodies are parsed more strictly than the single-area ones: every member the route needs (the complete
/// fence with every token member, and the route's own members) must be present and not null, duplicate and unknown
/// members are refused, and IDs and reasons are length-checked, all before the adapter is called (400). Status codes are
/// otherwise the same: 200 committed or read, 409 refused by the engine with a fresh view, 500 outcome unconfirmed.
/// </remarks>
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

    // The UI csproj embeds exactly three named page files and is outside the MA-1 write envelope, so the two-area page is
    // compiled from the constants in MultiAreaPage below and served from memory, like the single-area page.
    private static readonly byte[] MultiPageHtml = Encoding.UTF8.GetBytes(MultiAreaPage.Html);
    private static readonly byte[] MultiPageCss = Encoding.UTF8.GetBytes(MultiAreaPage.Css);
    private static readonly byte[] MultiPageJs = Encoding.UTF8.GetBytes(MultiAreaPage.Js);

    // Fixed page text from Core only: the single-area status words and Crop labels, and the MA-1 labels.
    private static readonly IReadOnlyDictionary<string, string> MultiLabels = new[] { typeof(PlayerLabels), typeof(CropLabels), typeof(MultiAreaLabels) }
        .SelectMany(type => type.GetFields(BindingFlags.Public | BindingFlags.Static))
        .Where(field => field.IsLiteral && field.FieldType == typeof(string))
        .ToDictionary(field => field.Name, field => (string)field.GetRawConstantValue()!, StringComparer.Ordinal);

    private readonly MultiAreaWorkPlanAdapter multiArea;

    private void AddMultiAreaRoutes()
    {
        routes.Add("multi", new("GET", context => WriteAssetAsync(context, MultiPageHtml, "text/html; charset=utf-8")));
        routes.Add("multi.css", new("GET", context => WriteAssetAsync(context, MultiPageCss, "text/css; charset=utf-8")));
        routes.Add("multi.js", new("GET", context => WriteAssetAsync(context, MultiPageJs, "text/javascript; charset=utf-8")));
        routes.Add("api/multi/view", new("GET", ReadMultiViewAsync));
        routes.Add("api/multi/manifest", new("GET", context => WriteAssetAsync(context, multiArea.ManifestBytes(), JsonContentType)));
        routes.Add("api/multi/hit", new("POST", MultiHitTestAsync));
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
        // Stage 4 Prune (spec multi-stage4-ui-catchup-v1 FR-4): the Fell pattern exactly - full fence, exact area and tree ID,
        // strict members, engine fence and refusal with a fresh view. The engine chooses the lift height; Approve, Cancel,
        // Execute and Year+1 are the existing shared Work Plan routes.
        routes.Add("api/multi/prune/add", new("POST", context => MultiCommandAsync<MultiTreeRequest>(context, MultiTreeMembers,
            request => multiArea.AddPrune(request.Fence!, request.AreaId!, request.TreeId!))));
        routes.Add("api/multi/prune/remove", new("POST", context => MultiCommandAsync<MultiTreeRequest>(context, MultiTreeMembers,
            request => multiArea.RemovePrune(request.Fence!, request.AreaId!, request.TreeId!))));
    }

    // The view and its Stage 4 / canopy / light read-out are taken from one root under one engine lock.
    private Task ReadMultiViewAsync(HttpContext context)
    {
        var snapshot = multiArea.ReadSnapshot();
        return WriteJsonAsync(context, StatusCodes.Status200OK,
            new MultiViewPayload(snapshot.View, ScenesOf(snapshot.View), MultiLabels, snapshot.Readout));
    }

    // One fenced read and one pure calculation on that same immutable view; never a mutation, never a Crop requirement.
    private async Task ReadMultiCompetitionAsync(HttpContext context)
    {
        var request = await ReadMultiRequestAsync<MultiTreeRequest>(context, MultiTreeMembers);
        if (request is null) return;
        var result = multiArea.Competition(request.Fence!, request.AreaId!, request.TreeId!);
        await WriteJsonAsync(context, StatusOf(result.RefusalCode), new MultiCompetitionPayload(result.View, ScenesOf(result.View),
            result.AreaId, result.Competition, NoticeOf(result.RefusalCode, result.RefusalMessage), multiArea.ReadoutFor(result.View)));
    }

    private async Task MultiHitTestAsync(HttpContext context)
    {
        var request = await ReadMultiRequestAsync<MultiHitRequest>(context, MultiHitMembers);
        if (request is null) return;
        var result = multiArea.HitTest(request.Fence!, request.AreaId!, request.U!.Value, request.V!.Value);
        await WriteJsonAsync(context, StatusOf(result.RefusalCode), new MultiHitPayload(result.View, ScenesOf(result.View),
            result.AreaId, result.Candidates, NoticeOf(result.RefusalCode, result.RefusalMessage), multiArea.ReadoutFor(result.View)));
    }

    // The read-out attached to a command's answer belongs to the same root as its view; if another request moved the session
    // in between, it is null and the page says so instead of pairing values from two roots.
    private async Task MultiCommandAsync<TRequest>(HttpContext context, IReadOnlyList<string> required,
        Func<TRequest, MultiAreaCommandResult> command) where TRequest : class
    {
        var request = await ReadMultiRequestAsync<TRequest>(context, required);
        if (request is null) return; // a guard has already answered
        var result = command(request);
        await WriteJsonAsync(context, StatusOf(result.RefusalCode), new MultiCommandPayload(result.View, ScenesOf(result.View),
            NoticeOf(result.RefusalCode, result.RefusalMessage), result.Execution, result.Annual, multiArea.ReadoutFor(result.View)));
    }

    // Every area's map items, built by Core's unchanged single-area map scene from this same view.
    private static IReadOnlyDictionary<string, IReadOnlyList<PlayerMapItem>> ScenesOf(MultiAreaSessionView view) =>
        view.Areas.ToDictionary(area => area.AreaId, area => MultiAreaProjection.Scene(view, area), StringComparer.Ordinal);

    // Returns null after answering the request itself, when a guard refuses it (415, 413 or 400).
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

    // Exact IDs are 1–64 characters without whitespace or control characters; a reason is 1–2000 characters.
    private static void RequireBoundedText(JsonElement root)
    {
        if (root.ValueKind != JsonValueKind.Object) throw new JsonException("the body must be one JSON object");
        foreach (var (name, limit) in new[] { ("areaId", MaxIdLength), ("treeId", MaxIdLength), ("planId", MaxPlanKeyLength),
                     ("executionKey", MaxPlanKeyLength) })
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

    // Stage4 (JSON "stage4") is additive: every member the page already reads keeps its name and shape.
    private sealed record MultiViewPayload(MultiAreaSessionView View, IReadOnlyDictionary<string, IReadOnlyList<PlayerMapItem>> Scenes,
        IReadOnlyDictionary<string, string> Labels, MultiAreaUiReadout? Stage4);

    private sealed record MultiCommandPayload(MultiAreaSessionView View, IReadOnlyDictionary<string, IReadOnlyList<PlayerMapItem>> Scenes,
        Notice? Refusal, MultiAreaExecutionResult? Execution, MultiAreaAnnualResult? Annual, MultiAreaUiReadout? Stage4);

    private sealed record MultiCompetitionPayload(MultiAreaSessionView View,
        IReadOnlyDictionary<string, IReadOnlyList<PlayerMapItem>> Scenes, string AreaId, PlayerCompetitionReport? Competition,
        Notice? Refusal, MultiAreaUiReadout? Stage4);

    private sealed record MultiHitPayload(MultiAreaSessionView View, IReadOnlyDictionary<string, IReadOnlyList<PlayerMapItem>> Scenes,
        string AreaId, IReadOnlyList<PlayerMapItem> Candidates, Notice? Refusal, MultiAreaUiReadout? Stage4);
}

/// <summary>
/// The two-area page: markup, style and script, served from memory at multi, multi.css and multi.js. The page uses the
/// same strict content security policy as the single-area page (its own script and style only; no inline code or style).
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
<!-- MA-1 two-area manual forestry page (Multi #885). Every value is filled in by multi.js from the server view. At 1024 CSS
     px and wider the map, the legend and the side panel share one screen; below that everything stacks: map, side panel,
     legend. -->
<a class="skip-link" id="skip-trees" href="#trees-heading">Skip to the tree table</a>

<header class="topbar" aria-labelledby="page-title">
  <div class="brand">
    <svg class="brand-mark" viewBox="0 0 30 36" aria-hidden="true" focusable="false">
      <path d="M15 1 6 13h5L3 25h10v9h4v-9h10l-8-12h5Z"></path>
    </svg>
    <div class="brand-text">
      <h1 id="page-title">Two-area Stand Map</h1>
      <p class="subtitle">MA-1 · two synthetic Sitka areas · one Work Plan · north up</p>
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

<!-- Owner UX repair UX-04: one unobtrusive line; every existing caveat stays, verbatim, behind Details. -->
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
    <button type="button" id="light-toggle" class="light-toggle" aria-pressed="false" aria-controls="light-legend"><span aria-hidden="true">☀</span> <span id="light-toggle-text">Light Guide: Off</span></button>
    <button type="button" id="ground-mode" class="ground-toggle" aria-pressed="false" aria-controls="ground-panel" hidden><span aria-hidden="true">⌖</span> <span id="ground-mode-text">Inspect ground: Off</span></button>
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
        </defs>
        <g id="grid" class="overlay"></g>
        <rect id="ground" class="ground" x="0" y="0" width="40" height="40" clip-path="url(#area-clip)"></rect>
        <g id="grid-in" class="overlay" clip-path="url(#area-clip)"></g>
        <g id="light-cells" class="overlay" clip-path="url(#area-clip)" shape-rendering="crispEdges"></g>
        <polygon id="area-outline" class="area-outline" points="0,0"></polygon>
        <g id="crowns"></g>
        <g id="deadwood" class="overlay"></g>
        <g id="ground-mark" class="overlay"></g>
        <g id="marks" class="overlay"></g>
        <g id="selection" class="overlay"></g>
        <g id="furniture" class="overlay"></g>
      </svg>
      <!-- UX-02: the competitor ranks 1-5 as real buttons over the map, keyed by exact engine tree ID (never by position). -->
      <div id="rank-layer" class="rank-layer" role="group" aria-label="Competitor ranks on the map: each button adds or removes that tree in the local Fell draft" hidden></div>

      <!-- UX-05: a narrow dock. Compass N/W/E/S in a 3 x 3 grid, then one − Fit + row, then one Area A | Area B row. -->
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
    <!-- UX-06/UX-09: a short continuous ramp drawn with the same colour mapping as the map cells; no band labels. -->
    <div id="light-legend" class="light-legend" hidden>
      <p id="light-legend-title" class="light-legend-title">Light Guide</p>
      <span class="ramp-end" aria-hidden="true">Shaded</span>
      <svg id="light-ramp" class="light-ramp" viewBox="0 0 100 10" preserveAspectRatio="none" role="img" aria-label="Light Guide colour ramp: no colour where the modelled light is zero, a faint muted rose for very low light, coral in the middle and pale peach for the brightest ground"></svg>
      <span class="ramp-end" aria-hidden="true">Bright</span>
      <p class="light-legend-note">Inspect ground reads the value at a spot.</p>
    </div>
    <div class="map-foot">
      <p id="map-note" class="map-note"></p>
      <p class="map-note map-note-end">Draft choices are not work orders</p>
    </div>
  </section>

  <aside id="legend-panel" class="legend-panel" aria-labelledby="legend-heading">
    <div class="legend-head">
      <h2 id="legend-heading">Map legend</h2>
      <button type="button" id="legend-hide" class="link-button small-link" aria-controls="legend-panel" aria-expanded="true">Hide legend</button>
    </div>
    <ul id="legend" class="legend"></ul>
    <p class="legend-foot">Letters, rings and shapes carry status. Colour is never the only cue.</p>
  </aside>

  <div id="side" class="side" data-pane="tree">
    <div id="pane-tree" class="pane">
      <!-- UX-07/UX-08: shown only while Inspect ground is on. A concise summary first; IDs and provenance on demand. -->
      <section id="ground-panel" class="panel ground-panel" aria-labelledby="ground-heading" hidden>
        <div class="ref-title">
          <p class="eyebrow">Inspect ground</p>
          <button type="button" id="ground-close" class="close-inspect" title="Stop inspecting ground (Esc)"><span aria-hidden="true">×</span> Stop</button>
        </div>
        <h2 id="ground-heading" tabindex="-1"></h2>
        <div id="ground-summary" class="ground-summary"></div>
        <details id="ground-trees" class="more ground-more"><summary id="ground-trees-summary">Contributing trees</summary>
          <p id="ground-tree-ids" class="mono ids"></p></details>
        <details id="ground-tech" class="more ground-more"><summary>Technical details</summary>
          <dl id="ground-facts" class="pairs ground-facts"></dl>
          <p id="ground-caveat" class="small-note"></p></details>
      </section>
      <section class="panel tree-panel" aria-labelledby="inspector-heading">
        <div id="inspector-empty" class="inspector-empty">
          <h2 class="eyebrow-heading">Reference tree</h2>
          <p>No tree selected in this area. Click a crown on the map, use the Trees tab, or type an exact tree ID below.</p>
        </div>
        <!-- UX-01: the primary flow first (compact reference, ranks 1-5, Add selected); pruning, Crop, reason and records below. -->
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

          <div class="add-dock">
          <div id="batch-result" class="batch-result" tabindex="-1" hidden></div>
          <form id="add-form" class="add-form" novalidate hidden>
            <p class="draft-line"><span id="draft-count" class="draft-count"></span></p>
            <p class="add-row"><button type="submit" id="add-button" class="add-button"></button>
              <button type="button" id="draft-clear" class="link-button small-link" aria-label="Clear the Fell draft selection">Clear</button></p>
            <p id="add-caption" class="small-note">Queues proposals only. Approve and Execute are separate.</p>
          </form>
          </div>

          <!-- UX-01/UX-08: while a tree is inspected, the ground panel moves here, below the primary block, so it never pushes
               ranks 1-5 or Add down. With no tree inspected it sits at the top of this pane. -->
          <div id="ground-slot" class="ground-slot"></div>

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
        <!-- F-2: a view whose engine read-out could not be paired (stage4: null) is "unavailable", never model 0. -->
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
        <h2 id="receipt-heading" tabindex="-1">Latest receipt</h2>
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
        <p id="details-note" class="note small-note">Light, seed, regeneration, site, money and saving: NOT MODELLED. Each area's ecology uses only its own trees.</p>
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
/* MA-1 two-area Stand Map (Multi #885), final navy design. Navy structure, a pale blue-grey map exterior, a slate stand
   interior and cool natural spruce. Amber marks a choice or unexecuted work, never an ecological judgement. Colour is
   never the only cue: every tree state also has a ring pattern, a letter, a number, a tick or text. At 1024 CSS px and
   wider, map, legend and side panel share one screen; below that the page is one stacked column with no sideways scroll. */
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
  --crop: #a9d4f5; --reference: #ffffff; --draft: #f2ad76; --pending: #edc774; --approved: #88c5e8;
  --dead: #c4c9c6; --stump: #cbb48c; --stump-mark: #3a2a17;
  --gap: 12px; --gutter: 24px; --legend-w: 232px; --side-w: 380px;
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

/* ---- top bar, summaries, notice --------------------------------------------------------------------------------- */
.topbar { display: flex; flex-wrap: wrap; align-items: center; gap: .6rem 1.2rem; padding: .75rem var(--gutter); border-bottom: 1px solid var(--edge); background: var(--top); }
.brand { display: flex; align-items: center; gap: .85rem; min-width: 0; margin-right: auto; }
.brand-mark { flex: none; width: 1.75rem; height: 2.1rem; fill: #9cc8ee; }
.subtitle { margin: .15rem 0 0; color: var(--muted); font-size: .8rem; }
.top-actions { display: flex; flex-wrap: wrap; align-items: center; gap: .6rem; }
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
.area-card { justify-content: space-between; }
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

/* UX-04: one quiet line; the full caveats open below it on demand. */
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
.workspace-head { grid-area: head; display: flex; flex-wrap: wrap; align-items: center; gap: .4rem 1rem; min-width: 0; }
.workspace-head h2 { margin: 0; font-size: 1.05rem; }
.map-subtitle { margin: 0; color: var(--muted); font-size: .86rem; }
/* The polite status line sits in the header row, so announcements never push the map down. */
.announce { flex: 1 1 10rem; min-width: 0; margin: 0; overflow: hidden; color: var(--muted); font-size: .8rem; white-space: nowrap; text-overflow: ellipsis; }
.announce:not(:empty) { padding: .2rem .6rem; border: 1px solid var(--line); border-radius: 6px; background: var(--panel); color: var(--text); }
.view-tabs { display: flex; flex-wrap: wrap; gap: .25rem; margin-left: auto; }
.view-tab { display: inline-flex; align-items: center; gap: .45rem; padding: .35rem .8rem; border-color: transparent; background: transparent; font-weight: 500; }
.view-tab:hover { background: var(--panel); }
.view-tab[aria-pressed="true"] { border-color: var(--blue-edge); background: var(--blue); font-weight: 600; }
.count { display: inline-grid; place-items: center; min-width: 1.35rem; padding: 0 .3rem; border-radius: 4px; background: var(--panel-3); color: var(--text); font-size: .74rem; font-variant-numeric: tabular-nums; }
.map-panel { grid-area: map; display: flex; flex-direction: column; min-width: 0; overflow: hidden; border: 1px solid var(--edge); border-radius: 8px; background: var(--panel); }
.legend-panel { grid-area: legend; min-width: 0; padding: .8rem .9rem; border: 1px solid var(--edge); border-radius: 8px; background: var(--panel); }
.side { grid-area: side; display: grid; align-content: start; gap: var(--gap); min-width: 0; }
.pane { display: none; align-content: start; gap: var(--gap); min-width: 0; }
.side[data-pane="tree"] #pane-tree, .side[data-pane="plan"] #pane-plan, .side[data-pane="trees"] #pane-trees,
.side[data-pane="history"] #pane-history, .side[data-pane="details"] #pane-details { display: grid; }
.panel { min-width: 0; padding: .9rem 1rem; border: 1px solid var(--edge); border-radius: 8px; background: var(--panel); }

/* ---- the map ------------------------------------------------------------------------------------------------------- */
.map-pane { position: relative; flex: 1 1 auto; min-height: 0; height: clamp(24rem, 72vh, 44rem); overflow: hidden; background: radial-gradient(ellipse at 55% 45%, var(--exterior-1), var(--exterior-2) 110%); }
#map { position: absolute; inset: 0; display: block; width: 100%; height: 100%; touch-action: manipulation; }
.map-foot { display: flex; flex-wrap: wrap; justify-content: space-between; gap: .2rem 1rem; padding: .45rem .9rem; border-top: 1px solid var(--edge); }
.map-note { margin: 0; color: var(--muted); font-size: .76rem; }
/* UX-05: three 44 px columns wide (the original pointer targets), so the dock covers far less of the stand. */
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
/* UX-02: rank tiles are real toggle buttons placed over the map; the tick corner and aria-pressed carry the state. */
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
.deco, .deco * { pointer-events: none; }
.tree.dimmed { opacity: .25; }
.stump-disc { fill: var(--stump); stroke: #6e5a3c; stroke-width: .035; }
.stump-ring { fill: none; stroke: #8a7350; stroke-width: .03; }

/* Status marks: ring widths are screen pixels (non-scaling) so they read the same at every zoom level. */
.ring { fill: none; vector-effect: non-scaling-stroke; stroke-linecap: round; stroke-linejoin: round; pointer-events: none; }
.ring-halo { stroke: #06111d; stroke-opacity: .85; stroke-width: 6; }
.ring-reference { stroke: var(--reference); stroke-width: 3; }
.ring-inspect { stroke: var(--reference); stroke-width: 2; stroke-dasharray: 2 3; }
.ring-competitor { stroke: #e3d3a4; stroke-width: 2; }
.ring-draft { stroke: var(--draft); stroke-width: 2.5; }
.ring-pending { stroke: var(--pending); stroke-width: 2; stroke-dasharray: 5 4; }
.ring-approved { stroke: var(--approved); stroke-width: 2; }
.badge-bg { fill: #10202f; stroke-width: 1.5; vector-effect: non-scaling-stroke; }
.badge-text { font-weight: 800; text-anchor: middle; dominant-baseline: central; }
.b-crop .badge-bg { stroke: var(--crop); } .b-crop .badge-text { fill: var(--crop); }
.b-reference .badge-bg, .b-inspect .badge-bg { stroke: #fff; } .b-reference .badge-text, .b-inspect .badge-text { fill: #fff; }
.b-pending .badge-bg { stroke: var(--pending); } .b-pending .badge-text { fill: var(--pending); }
.b-approved .badge-bg { stroke: var(--approved); } .b-approved .badge-text { fill: var(--approved); }
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

/* ---- legend -------------------------------------------------------------------------------------------------------- */
.legend-head { display: flex; flex-wrap: wrap; align-items: center; justify-content: space-between; gap: 0 .5rem; padding-bottom: .35rem; border-bottom: 1px solid var(--edge); }
.legend-head h2 { white-space: nowrap; }
.legend-head h2 { margin: 0; font-size: 1rem; }
.legend { display: grid; gap: 0; margin: 0; padding: 0; list-style: none; }
.legend li { display: grid; grid-template-columns: 40px minmax(0, 1fr); align-items: center; gap: .6rem; padding: .45rem 0; border-bottom: 1px solid var(--edge); }
.legend svg { width: 40px; height: 40px; overflow: visible; }
.legend-name { display: block; font-size: .84rem; font-weight: 600; }
.legend-desc { display: block; color: var(--muted); font-size: .72rem; line-height: 1.3; }
.legend-foot { margin: .6rem 0 0; color: var(--muted); font-size: .72rem; }
.legend .swatch-ground { fill: var(--interior); }
.legend .legend-exterior { fill: #14263a; }
.legend .legend-area { fill: var(--interior); stroke: #7f97ae; stroke-width: 1.4; stroke-dasharray: 3 2.5; vector-effect: non-scaling-stroke; }

/* ---- side panel: reference tree and competitors ------------------------------------------------------------------- */
.tree-panel { padding: 0; overflow: visible; }
.inspector-empty { padding: .9rem 1rem; }
.ref-card { margin: 0; padding: .9rem 1rem .7rem; border-bottom: 1px solid var(--edge); }
.ref-top { display: flex; align-items: center; justify-content: space-between; gap: .5rem; margin: 0 0 .4rem; }
.ref-title { display: flex; flex-wrap: wrap; align-items: center; gap: .4rem; }
.ref-title h2 { margin: 0; font-size: 1.55rem; font-weight: 650; letter-spacing: -.01em; }
.ref-title .eyebrow { margin: 0; }
.ref-head { display: flex; align-items: center; gap: .5rem; }
.ref-id { flex: 1 1 auto; min-width: 0; }
.ref-id .ref-top { justify-content: flex-start; white-space: nowrap; }
/* The status chip shares the ID line and shortens with an ellipsis (full text in its tooltip and the Tree record), so a
   Crop or Pending reference never pushes the competitor rows below the fold. */
.ref-id .ref-title { flex-wrap: nowrap; }
.ref-id .ref-title h2 { flex: none; }
.ref-id .chip { display: block; min-width: 0; overflow: hidden; text-overflow: ellipsis; white-space: nowrap; }
/* UX-03: a prominent exit next to the reference ID; Escape does the same. */
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
.comp-id { display: block; font-weight: 650; overflow-wrap: anywhere; }
.comp-id .rank { margin-right: .35rem; color: var(--gold-edge); font-style: italic; font-weight: 700; }
.comp-meta { display: block; color: var(--muted); font-size: .72rem; }
.comp-why { display: block; color: var(--pending); font-size: .72rem; font-weight: 600; }
.comp-num { text-align: right; font-variant-numeric: tabular-nums; font-size: .84rem; }
.comp-strength { display: block; color: var(--muted); font-size: .68rem; }
.comp-meta { white-space: nowrap; }
.add-row { display: flex; align-items: center; gap: .5rem; }
/* One line whatever the count, so the sticky Add bar never grows over rank 5 (UX-01). */
.add-row .add-button { flex: 1 1 auto; width: auto; padding: .45rem .6rem; white-space: nowrap; }
.add-row .link-button { flex: none; }
.comp-empty { margin: .3rem 0; color: var(--muted); }
.batch-result { max-height: 11rem; overflow-y: auto; margin: .5rem 1rem 0; padding: .55rem .75rem; border: 1px solid var(--line); border-left: 4px solid var(--approved); border-radius: 6px; background: var(--panel-2); font-size: .84rem; }
.batch-result.partial { border-left-color: var(--pending); }
.batch-result.failed { border-left-color: var(--alert-line); }
.batch-result p { margin: .2rem 0; }
.batch-result .alert-title { font-weight: 700; }
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

/* ---- side panel: Work Plan, receipts, tables ------------------------------------------------------------------- */
.stepper { display: flex; flex-wrap: wrap; gap: .3rem; margin: 0 0 .6rem; padding: 0; list-style: none; counter-reset: step; font-size: .74rem; color: var(--muted); }
.stepper li { counter-increment: step; }
.stepper li::before { content: counter(step) " "; }
.stepper li + li::before { content: "→ " counter(step) " "; }
.stepper li.now { color: var(--text); font-weight: 700; }
.plan-state { margin: .3rem 0 .5rem; padding: .7rem .9rem; border: 1px solid var(--line); border-radius: 6px; }
.pending-state { border-color: var(--pending); background: #25231a; }
.approved-state { border-color: #5b8fa6; background: #1b3a40; }
.plan-total { margin: 0; font-size: 1.1rem; font-weight: 650; }
.plan-actions { margin: .5rem 0; }
.plan-area { margin: .5rem 0; padding-top: .4rem; border-top: 1px solid var(--edge); }
.plan-area h4 { margin: .1rem 0 .3rem; color: var(--muted); font-weight: 500; }
.orders li { display: grid; grid-template-columns: minmax(0, 1fr) auto; align-items: center; gap: .1rem .6rem; padding: .35rem 0; border-bottom: 1px solid var(--edge); }
.orders li:last-child { border-bottom: 0; }
.orders .mono { font-weight: 650; }
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
table { width: 100%; border-collapse: collapse; font-size: .86rem; }
caption { padding: .3rem 0; color: var(--muted); text-align: left; caption-side: top; }
th, td { padding: .3rem .45rem; border-bottom: 1px solid var(--edge); text-align: left; vertical-align: middle; }
#tree-table td, #tree-table tbody th { white-space: nowrap; }
thead th { position: sticky; top: 0; z-index: 1; background: var(--panel-2); }
tr.current { background: var(--panel-3); }
tr.current > * { border-top: 2px solid var(--reference); border-bottom: 2px solid var(--reference); }
dialog { width: min(38rem, calc(100vw - 2rem)); max-height: calc(100vh - 2rem); overflow: auto; padding: 1.2rem; border: 1px solid var(--edge); border-radius: 10px; background: var(--panel); color: var(--text); }
dialog::backdrop { background: rgba(3, 9, 17, .72); }
dialog .actions { justify-content: flex-end; margin-top: 1rem; }

/* ---- Stage 1–4 read-outs (spec multi-stage4-ui-catchup-v1): every value is an engine value, formatted only ------- */
.area-card { flex-wrap: wrap; row-gap: .25rem; }
.area-readouts { display: flex; flex-wrap: wrap; flex-basis: 100%; gap: .1rem .9rem; margin: 0; padding-top: .3rem; border-top: 1px solid var(--edge); font-size: .76rem; }
.area-readouts > div { display: flex; align-items: baseline; gap: .3rem; min-width: 0; }
.area-readouts dt { color: var(--muted); font-size: .7rem; }
.area-readouts dd { margin: 0; font-weight: 650; font-variant-numeric: tabular-nums; white-space: nowrap; }
.area-readouts dd small { color: var(--muted); font-weight: 500; }
.model-badge { display: inline-flex; flex-wrap: wrap; align-items: center; gap: .25rem; }
.model-badge abbr { padding: 0 .35rem; border: 1px solid var(--line); border-radius: 4px; background: var(--panel-2); color: var(--text); font-size: .68rem; font-weight: 600; text-decoration: none; white-space: nowrap; }
.light-toggle, .ground-toggle { min-height: 36px; padding: .2rem .75rem; font-size: .84rem; }
.light-toggle[aria-pressed="true"] { border-color: #ffd9bd; background: #3a2a2a; }
.ground-toggle[aria-pressed="true"] { border-color: #ffd9bd; background: #4a3029; }
/* UX-06/UX-09: one short continuous ramp. Each light cell's colour and opacity are set from its own engine value. */
.light-legend { display: flex; flex-wrap: wrap; align-items: center; gap: .2rem .6rem; padding: .3rem .9rem; border-top: 1px solid var(--edge); font-size: .74rem; }
.light-legend-title { margin: 0; font-weight: 650; }
.light-ramp { display: block; width: 9rem; height: 12px; border: 1px solid var(--line); border-radius: 3px; }
.ramp-end { color: var(--muted); }
.light-legend-note { margin: 0 0 0 auto; color: var(--muted); }
.light-cell { stroke: none; pointer-events: none; }
/* UX-08: crowns keep the pointer (a crown click still selects); only open ground shows the crosshair. */
.map-pane.ground-mode #map { cursor: crosshair; }
.ground-pick { fill: none; stroke: #ffffff; stroke-width: 2; stroke-dasharray: 4 3; vector-effect: non-scaling-stroke; }
.ground-pick-halo { fill: none; stroke: #06111d; stroke-opacity: .8; stroke-width: 5; vector-effect: non-scaling-stroke; }
.log-body { fill: #8a6a43; stroke: #2a1d0e; stroke-width: 1.4; vector-effect: non-scaling-stroke; }
.log-end { fill: #d9bf8c; stroke: #2a1d0e; stroke-width: 1.2; vector-effect: non-scaling-stroke; }
.log-ring { fill: none; stroke: #6e5130; stroke-width: 1; vector-effect: non-scaling-stroke; }
.log-halo { fill: none; stroke: #06111d; stroke-opacity: .8; stroke-width: 4.5; vector-effect: non-scaling-stroke; }
.ground-panel h2 { margin: .1rem 0 .3rem; font-size: 1rem; }
.ground-slot > .ground-panel { margin: .5rem .8rem 0; padding: .6rem .8rem; background: var(--panel-2); }
.ground-level { display: flex; align-items: center; gap: .5rem; margin: 0 0 .3rem; font-size: 1.05rem; font-weight: 650; }
.ground-swatch { flex: none; width: 22px; height: 22px; border: 1px solid var(--line); border-radius: 3px; }
.ground-summary dl { display: grid; grid-template-columns: auto minmax(0, 1fr); gap: .15rem .7rem; margin: 0; }
.ground-summary dd { font-variant-numeric: tabular-nums; }
.ground-summary .small-note { margin: .2rem 0 0; }
.ground-more { margin: .35rem 0 0; }
.ground-more summary { min-height: 36px; }
.ground-facts { display: grid; grid-template-columns: minmax(0, 1fr); gap: .2rem; }
.ground-facts > div { display: grid; grid-template-columns: 8.5rem minmax(0, 1fr); gap: .5rem; }
.ground-facts dd { font-variant-numeric: tabular-nums; }
.ids { overflow-wrap: normal; }
.ids .id { white-space: nowrap; }
.readout-unavailable p { margin: .2rem 0; }
.prune-box { margin: .45rem 0 0; padding: .45rem .6rem; border: 1px solid var(--line); border-radius: 6px; background: var(--panel-2); }
.prune-box p { margin: .15rem 0; }
.prune-head { display: flex; align-items: baseline; justify-content: space-between; gap: .5rem; }
.prune-head strong { font-variant-numeric: tabular-nums; }
.prune-status { font-size: .82rem; }
.prune-actions { display: flex; flex-wrap: wrap; gap: .4rem; }
.prune-add { border-color: var(--approved); }
.prune-why { margin: 0; }
.order-kind { display: inline-block; margin-right: .4rem; padding: 0 .3rem; border: 1px solid var(--line); border-radius: 3px; font-size: .7rem; font-weight: 600; }
.order-kind.k-prune { border-color: var(--approved); color: var(--approved); }

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
}
/* Compact desktop (about 1280 × 720): narrower columns, tighter gaps; targets stay at least 44 px. */
@media (min-width: 1024px) and (max-width: 1599px) {
  :root { --gap: 8px; --gutter: 16px; --legend-w: 176px; --side-w: 310px; }
  body { font-size: .9rem; }
  h1 { font-size: 1.25rem; }
  .topbar { padding: .5rem var(--gutter); }
  .brand-mark { width: 1.5rem; height: 1.8rem; }
  .card { padding: .5rem .9rem; }
  .area-card-count strong { font-size: 1.25rem; }
  .plan-card-main strong { font-size: 1.05rem; }
  .notice { padding: .2rem .8rem; font-size: .76rem; }
  .legend-panel { padding: .45rem .55rem; }
  .legend-head h2 { font-size: .88rem; }
  .legend-head .link-button { min-height: 32px; padding: .2rem 0; font-size: .74rem; }
  .legend li { grid-template-columns: 24px minmax(0, 1fr); gap: .4rem; padding: .1rem 0; }
  .legend svg { width: 24px; height: 24px; }
  .legend-desc { display: none; }
  .legend-name { font-size: .74rem; }
  .legend-foot { margin-top: .35rem; font-size: .66rem; }
  .ref-sub { display: none; }
  .key-facts dt { font-size: .62rem; }
  #add-caption { font-size: .72rem; }
  .view-tab { padding: .3rem .55rem; }
  .ref-card { padding: .6rem .8rem .5rem; }
  .competition { padding: .5rem .8rem .1rem; }
  .add-form { padding: .45rem .8rem .55rem; }
  .batch-result { max-height: 7rem; margin: .4rem .8rem 0; }
  .ref-title h2 { font-size: 1.3rem; }
  .key-facts dd { font-size: .98rem; }
  .comp-head, .comp-row label { grid-template-columns: 1.7rem minmax(0, 1fr) 3.3rem 3.7rem; gap: .3rem; }
  .comp-row label { min-height: 48px; padding: .25rem .45rem; font-size: .84rem; }
  .map-dock { left: 10px; top: 10px; }
  .close-inspect { min-height: 34px; padding: .15rem .55rem; font-size: .78rem; }
  .area-readouts { gap: 0 .6rem; padding-top: .2rem; font-size: .7rem; }
  .area-readouts dt { font-size: .66rem; }
  .light-legend { padding: .25rem .8rem; font-size: .7rem; }
  .ground-facts > div { grid-template-columns: minmax(0, 1fr); gap: 0; }
}
/* Short desktop (about 1280 x 720): the reference summary, ranks 1-5 and Add selected must fit without scrolling (UX-01). */
@media (min-width: 1024px) and (max-height: 820px) {
  .topbar { padding-top: .4rem; padding-bottom: .4rem; }
  .btn-sub { font-size: .7rem; }
  .area-tab { min-height: 46px; }
  .area-thumb { width: 26px; height: 15px; }
  .crop-note { display: none; }
  .competition .small-note { margin-bottom: .3rem; }
  #insp-pinned { display: none; }
  #add-caption { display: none; }
  .ref-card { padding: .45rem .8rem .4rem; }
  .ref-top { margin-bottom: .1rem; }
  .ref-id .tag { display: none; } /* the map subtitle and the pressed Area button already name the area */
  .ref-title h2 { font-size: 1.15rem; }
  .key-facts { margin-top: .2rem; }
  .key-facts > div { display: flex; align-items: baseline; gap: .25rem; min-width: 0; }
  .key-facts dd { font-size: .9rem; }
  .competition { padding: .4rem .8rem 0; }
  .competition h3 { margin-bottom: .25rem; font-size: .92rem; }
  .comp-head { padding-bottom: .1rem; }
  .comp-rows { gap: .3rem; }
  .comp-row label { min-height: 38px; padding: .1rem .45rem; line-height: 1.2; }
  .add-dock { margin-top: .3rem; }
  .add-form { padding: .3rem .8rem .4rem; }
  .add-form p { margin: 0; }
  .draft-line { display: none; }
  .card { padding: .35rem .8rem; }
  .area-card-count { margin-top: 0; }
  .area-card-count strong { font-size: 1.1rem; }
  .area-card-side strong { font-size: 1rem; }
  .notice-details > summary { min-height: 26px; }
  .notice { font-size: .76rem; }
  .ref-links .link-button { min-height: 32px; }
  .add-button { min-height: 44px; }
  .add-row .add-button { font-size: .86rem; }
  .reason-details summary { min-height: 36px; padding: .45rem .6rem; }
}
/* Narrow / 200% zoom: map, then side panel, then the legend, which stays visible (two columns where it fits). */
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
// MA-1 two-area manual forestry page (owner-approved MA-1 spec §4; Multi #885), served from memory by the same loopback
// process as the single-area page, behind the same guards.
//
// The server's view is the only authority. This page draws, lists and inspects only what the latest view says, sends exact
// area and tree IDs with the fence of the view it displayed, and itself converts nothing but screen pixels into map
// metres. It computes no forestry statistic: competition, stands, receipts and year results all come from the server. It
// keeps no copy of felled trees, counters, clocks or randomness, uses no web storage and loads nothing from another origin.
// Every text node is set with textContent, never parsed as HTML.
//
// The only page-local state beyond display choices is the per-area Fell draft: exact competitor IDs ticked for the current
// reference tree. A draft is not a work order. "Add selected" sends them one at a time through the existing pending/add
// command, each with the fence of the latest confirmed view, and stops at the first refusal or unconfirmed outcome.
//
// Stage 1–4 catch-up (spec multi-stage4-ui-catchup-v1): every payload also carries "stage4", the engine's read-out of the
// same committed root (life states, pruning eligibility and its reason, canopy cover, open ground, RD, natural deaths,
// fallen deadwood and the 2.5 m modelled light cells). The page formats those values and computes none of them; mapping a
// light index to a colour and opacity, naming it with a level word and multiplying a fraction by 100 for "%" are display
// formatting only.
//
// Owner UX repair, Phase A (spec multi-stage4-owner-ux-20261123-v1): compact inspector, map rank toggles on exact tree IDs,
// Close inspection / Escape, one-line notice, narrow navigation, a continuous coral-to-peach Light Guide under the crowns,
// an explicit Inspect ground mode, and an "unavailable" state for a view whose read-out could not be paired (F-2).
'use strict';

(function () {
  const SVG_NS = 'http://www.w3.org/2000/svg';
  const STATUS_LABEL = { Living: 'StatusLiving', PendingFell: 'StatusPendingFell', ApprovedFell: 'StatusApprovedFell',
    Felled: 'StatusFelled', NaturalDead: 'StatusNaturalDead' };
  const ZOOMS = Array.from({ length: 11 }, (_, i) => Math.pow(4, i / 10)); // reversible ~1.15x steps
  const PANES = ['tree', 'plan', 'trees', 'history', 'details'];
  const PANE_NAMES = { tree: 'Tree', plan: 'Work Plan', trees: 'Trees', history: 'Results', details: 'Details' };
  // Server commands whose buttons a re-read lock or a running add sequence disables. The draft Add button has its own rule.
  const COMMAND_IDS = ['crop-button', 'add-reference', 'insp-remove', 'approve', 'cancel-approved', 'execute',
    'execute-confirm', 'advance', 'consent-advance', 'prune-add', 'prune-remove'];
  // Light Guide presentation (a VISUAL mapping, not a simulation law). The engine's light index L in [0, 1] is eased by
  // t = L^LIGHT_EASE, which keeps the low end visible, and then sets both the opacity (LIGHT_ALPHA_MAX * t) and the colour
  // along muted rose -> coral -> pale peach. L = 0 draws nothing; any L > 0 gets a strictly positive opacity (no floor,
  // no threshold: the mapping is continuous at 0, and opacityText never writes a positive value as "0"); both rise
  // continuously and monotonically up to L = 1.
  const LIGHT_STOPS = [[0x7f, 0x3b, 0x4a], [0xda, 0x79, 0x6d], [0xff, 0xd9, 0xbd]];
  const LIGHT_EASE = 0.65;
  const LIGHT_ALPHA_MAX = 0.9;
  // Level words for the ground inspector's text (the map itself is continuous; colour is never the only cue).
  const LIGHT_WORDS = [[0.1, 'Very shaded'], [0.35, 'Low'], [0.65, 'Moderate'], [Infinity, 'Bright']];
  const STATEMENTS = ['MultiSyntheticStatement', 'MultiNotComparable', 'MultiSiteUnknown', 'MultiEdgeExcluded',
    'MultiNoTransfer', 'MultiSessionOnly'];
  // Notice chips: a short form shown, its full statement as the tooltip (the Details pane lists the full statements).
  const CHIPS = [['MultiChipSynthetic', 'MultiSyntheticStatement'], ['MultiChipNotComparable', 'MultiNotComparable'],
    ['MultiChipSiteUnknown', 'MultiSiteUnknown'], ['MultiChipEdgeExcluded', 'MultiEdgeExcluded'],
    ['MultiChipNoTransfer', 'MultiNoTransfer']];
  const DEFAULT_REASON = 'Owner-selected felling';
  const LITE_CROWN_PX = 11; // below this on-screen crown radius the spruce drops its needle strokes

  const state = {
    view: null, // the latest multi-area view from the server
    scenes: {}, // per area: the map items the server built from that same view
    labels: null,
    areaId: null, // the area shown on the map and in the tree table
    selected: {}, // per area: the UI-only selected exact tree ID (a living one is that area's reference), never an authority
    drafts: {}, // per area: { ref, ids } — exact competitor IDs ticked for reference ref; local only, never a work order
    draftNotice: null, // { areaId, text } — why draft choices were dropped after a newer view
    overlap: null, // { areaId, candidates } from the latest map hit-test
    competition: null, // { areaId, report } read from the server for competitionTarget
    competitionTarget: null, // { areaId, treeId }
    competitionTag: null,
    competitionRequest: 0,
    busy: false,
    // Generation of map selection attempts. A map click takes the next number; Escape, Close inspection, an area switch and
    // any other selection advance it, so a hit-test answer that arrives after one of those is discarded unseen.
    mapPick: 0,
    batch: null, // the add sequence in flight, if any; one operation-wide guard for its whole duration
    batchResult: null, // the latest add sequence's outcome, shown until the area's reference changes
    lock: null, // set when the page can no longer trust what it shows; only a re-read lifts it
    lockCount: 0,
    confirmation: null, // the open Execute confirmation: a frozen copy of the plan and fence it shows
    consentWork: false,
    opener: null,
    lastExecution: null,
    lastWasReplay: false,
    zoom: 1,
    centre: null, // centre of the usable map area (beside the navigation dock), in map metres
    legendHidden: false,
    stage4: null, // the engine's read-out of the same root as view (null if the server could not pair one)
    lightOn: false, // Light Guide overlay: Off by default
    groundMode: false, // Inspect ground: a click with no crown under it reads the ground (crowns still select; Light Guide on only)
    ground: null, // { areaId, column, row } of the inspected ground cell, or { areaId, missing: true } when no sample is there
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

  function pair(term, value, className) {
    return el('div', className ? { class: className } : null, el('dt', null, term), el('dd', null, value));
  }

  function label(name) {
    return state.labels && typeof state.labels[name] === 'string' ? state.labels[name] : '';
  }

  function fill(template, values) {
    return template.replace(/\{(\w+)\}/g, (match, name) => (name in values ? String(values[name]) : match));
  }

  // Display rounding only: the value itself always comes from the server view.
  function fixed(value, digits) {
    return typeof value === 'number' ? value.toFixed(digits) : String(value);
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

  // ---- lookups in the latest view ---------------------------------------------------------------------------------------

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
  function areaName(areaId) {
    const area = areaView(areaId);
    return area ? area.label : String(areaId);
  }
  // Table rows use the same plain area names as the other routine surfaces.
  function areaShort(areaId) {
    const area = areaView(areaId);
    return area ? area.label : String(areaId);
  }
  function selectedId() { return state.areaId ? state.selected[state.areaId] || null : null; }
  function ordersIn(orders, areaId) { return orders.filter((order) => order.areaId === areaId); }

  // ---- the engine's Stage 4 read-out (same root as the view) -------------------------------------------------------------
  function s4Area(areaId) {
    const readout = state.stage4;
    return readout && areaId ? readout.areas.find((area) => area.areaId === areaId) || null : null;
  }
  function s4Tree(areaId, treeId) {
    const area = s4Area(areaId);
    return area && treeId ? area.trees.find((tree) => tree.treeId === treeId) || null : null;
  }
  function pendingPrunes() { return state.stage4 ? state.stage4.pendingPrunes : []; }
  function approvedPrunes() { return state.stage4 ? state.stage4.approvedPrunes : []; }
  function isStage4() { return Boolean(state.stage4 && state.stage4.models.adultModelVersion === 1); }
  // F-2: 'stage4' (paired 1/1/1 read-out), 'legacy' (a paired read-out of another model) or 'unavailable' (the server sent
  // stage4: null because the session moved between the command and its read-out). Unavailable is never shown as model 0.
  function readoutState() { return !state.stage4 ? 'unavailable' : isStage4() ? 'stage4' : 'legacy'; }
  // The Light Guide paint of one engine light index: null for L = 0 (nothing drawn), else a colour and an opacity string.
  function lightPaint(index) {
    if (!(index > 0)) return null;
    const t = Math.pow(Math.min(1, index), LIGHT_EASE);
    const segment = t <= 0.5 ? 0 : 1;
    const f = segment === 0 ? t / 0.5 : (t - 0.5) / 0.5;
    const rgb = LIGHT_STOPS[segment].map((low, i) => Math.round(low + (LIGHT_STOPS[segment + 1][i] - low) * f));
    return { colour: '#' + rgb.map((v) => v.toString(16).padStart(2, '0')).join(''), opacity: opacityText(LIGHT_ALPHA_MAX * t) };
  }
  // A positive opacity as text that never reads "0": four decimals (as before) down to 0.01, then enough decimals for three
  // significant figures, and exponent form only for values too small for fixed notation (an L far below any real cell).
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
  function baseStatusText(item) {
    const template = label(STATUS_LABEL[item.status]);
    return template ? fill(template, { y: item.deathYear }) : String(item.status);
  }
  function statusText(item) {
    return item.isCrop ? label('CropRetained') : baseStatusText(item);
  }

  // A living selected tree is its area's reference: competition is read for it, and its draft belongs to it.
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

  // The competition report for the reference now shown, or null while it is being read, locked or for another tree.
  function currentReport() {
    const comp = state.competition;
    if (!comp || state.lock || comp.areaId !== state.areaId) return null;
    return comp.report && comp.report.targetTreeId === referenceIn(state.areaId) ? comp.report : null;
  }

  // Why an exact tree cannot be a Fell draft choice right now, from the latest view only (null when it can).
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
    const s4 = s4Tree(areaId, treeId);
    if (s4 && s4.prunePlanState) return 'In the Prune plan';
    if (treeId === referenceIn(areaId)) return 'Reference tree';
    return null;
  }

  // ---- talking to the local session process ---------------------------------------------------------------------------

  // The fence is echoed exactly as the latest view gave it; the page never edits it.
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
    try {
      payload = await response.json();
    } catch {
      payload = null;
    }
    if (response.status === 200 && payload && payload.view) return { kind: 'ok', payload };
    if (response.status === 409 && payload && payload.view && payload.refusal) return { kind: 'refused', payload };
    if (payload && payload.error) {
      const unconfirmed = payload.error.code === 'OutcomeUnconfirmed';
      return { kind: unconfirmed ? 'unconfirmed' : 'guard', status: response.status, error: payload.error };
    }
    return { kind: 'transport', detail: 'HTTP ' + response.status };
  }

  // Shows a newly received view. A selection is kept only if its exact ID is still in that area of the new view.
  function applyPayload(payload, keepOverlap) {
    state.view = payload.view;
    state.scenes = payload.scenes || {};
    state.stage4 = payload.stage4 || null;
    if (payload.labels) state.labels = payload.labels;
    if (state.ground && !state.ground.missing && !groundCell(state.ground.areaId, state.ground.column, state.ground.row)) state.ground = null;
    if (!areaView(state.areaId)) state.areaId = state.view.areas.length > 0 ? state.view.areas[0].areaId : null;
    for (const areaId of Object.keys(state.selected)) {
      if (!treeIn(areaId, state.selected[areaId])) delete state.selected[areaId];
    }
    if (!keepOverlap) state.overlap = null;
    const target = state.competitionTarget;
    if (target && !livingIn(target.areaId, target.treeId)) clearCompetition();
    const refresh = state.competitionTarget !== null && state.competitionTag !== competitionTag();
    // While an add sequence runs, the report it started from stays on screen (its rows show each tree's new status), so an
    // intermediate view cannot reset the sequence's context; the sequence re-reads competition once it has finished.
    if (refresh && !state.batch) state.competition = null;
    reconcileDrafts();
    renderAll();
    const confirmation = state.confirmation;
    if (confirmation && !byId('execute-dialog').open) state.confirmation = null;
    else if (confirmation && !confirmationStillShown(confirmation, state.view)) withdrawConfirmation(confirmation);
    if (refresh && !state.lock && !state.batch) refreshCompetition();
  }

  // Runs one fenced command. Nothing on the page changes until the server answers; then the answer is drawn as it is.
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

  // ---- alerts, announcements and the re-read lock -------------------------------------------------------------------------

  function clearAlert() {
    const box = byId('alert');
    box.replaceChildren();
    box.classList.remove('shown');
  }

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
        return 'That tree belongs to another area. Switch to its area and select it there.';
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

  // Whether "Add selected" can send now, and the plain reason when it cannot.
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
    // F-2: without a paired read-out the Prune orders of this view are unknown, so nothing may be approved or executed.
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

  // ---- the Execute confirmation -------------------------------------------------------------------------------------------
  //
  // The dialog opens on a frozen copy of the plan and fence the page displayed, and Confirm sends exactly that copy. If a
  // newer view no longer shows that plan under that fence, the confirmation is withdrawn unsent and the page must re-read.

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

  // The approved Prune orders now shown must be exactly those the confirmation froze. No read-out: cannot confirm at all
  // (F-2), because the Prune orders of this view are then unknown, not absent.
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
    renderAreaSwitch();
    renderMap();
    renderOverlap();
    renderGround();
    renderLightLegend();
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
    // Re-created controls get their focus back by a stable key (an exact ID), so a redraw is not a focus reset.
    if (focusKey) {
      const again = document.querySelector('[data-focus-key="' + CSS.escape(focusKey) + '"]');
      if (again && again !== document.activeElement) again.focus();
    }
  }

  function renderStatus() {
    const view = state.view;
    document.title = 'Two-area Stand Map — ' + label('MultiTitle');
    byId('page-title').textContent = label('MultiTitle');
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
        el('span', null, 'm²/ha basal area · ' + fixed(area.areaHa, 4) + ' ha')),
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
    byId('notice-line').textContent = 'Synthetic Sitka experiment · ' + plural(view.areas.length, 'area', 'areas') + ' · Site unknown';
    const readout = readoutState();
    // The full statements stay word for word behind Details. With no paired read-out the model-specific statements (edge
    // exclusion or connected woodland) are not claimed either way: the page says the read-out is unavailable.
    byId('disclosure').textContent = readout === 'stage4' ? label('MultiDisclosureStage4') : readout === 'legacy' ? label('MultiDisclosure') :
      'Model read-out unavailable for this view, so the model statements are not shown. Choose Re-read. ' +
      [label('MultiSyntheticStatement'), label('MultiNotComparable'), label('MultiSiteUnknown'), label('MultiNoTransfer')].join('. ') + '.';
    const chips = [];
    const chipList = readout === 'stage4' ? CHIPS.map((pair) => pair[0] === 'MultiChipEdgeExcluded' ? ['MultiChipConnectedWoodland', 'MultiConnectedWoodland'] : pair)
      : readout === 'legacy' ? CHIPS : CHIPS.filter((pair) => pair[0] !== 'MultiChipEdgeExcluded');
    chipList.forEach(([chip, full], index) => {
      if (index > 0) chips.push(' · ');
      chips.push(el('abbr', { title: label(full) }, label(chip)));
    });
    byId('flags').replaceChildren(...chips);
    byId('session-note').replaceChildren(el('abbr', { title: label('MultiSessionOnly') }, label('MultiChipSessionOnly')));
  }

  // The four rack readouts: canopy cover and open ground, stand density (RD), natural deaths / fallen deadwood and the model
  // badge. Engine values from the read-out of this same root, formatted only.
  function areaReadouts(areaId) {
    const s4 = s4Area(areaId);
    const readout = state.stage4;
    if (!s4 || !readout) {
      return el('p', { class: 'area-readouts' }, 'Stage 4 read-out unavailable for this view — choose Re-read.');
    }
    const c = s4.canopy;
    const m = readout.models;
    return el('dl', { class: 'area-readouts' },
      el('div', null, el('dt', null, 'Canopy cover'), el('dd', null, pct(c.geometricCoverFraction), ' ',
        el('small', null, '· open ' + fixed(c.geometricUncoveredM2, 0) + ' m²'))),
      el('div', null, el('dt', null, 'Stand density'), el('dd', null, 'RD ' + fixed(c.relativeDensity, 2))),
      el('div', null, el('dt', null, 'Natural deaths'), el('dd', null, c.annualNaturalDeaths + ' this year', ' ',
        el('small', null, '· ' + plural(c.fallenDeadwoodCount, 'log', 'logs') + ' total'))),
      el('div', null, el('dt', { class: 'sr-only' }, 'Model'), el('dd', { class: 'model-badge' },
        el('abbr', { title: 'CrownModelVersion ' + m.crownModelVersion + ', AdultModelVersion ' + m.adultModelVersion +
          ', MortalityModelVersion ' + m.mortalityModelVersion + ', light ' + m.lightModelVersion + ' (' + fixed(m.lightCellM, 1) + ' m cells)' },
        'Model ' + m.crownModelVersion + '/' + m.adultModelVersion + '/' + m.mortalityModelVersion),
        el('abbr', { title: m.syntheticStandDisclaimer }, 'Synthetic stand'),
        el('abbr', { title: m.notCalibratedCaveat }, 'Light: not PAR'))));
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

  // ---- the map: one SVG user unit is one metre; U runs east from the extent's west edge, V south from its north edge ----
  //
  // The SVG fills the map pane and its viewBox always has the pane's aspect ratio, so one metre is the same number of pixels
  // across and down. At Fit the whole area box fits in the part of the pane beside (or below) the navigation dock; zooming
  // and panning move only that usable part. Screen-to-map conversion for hit-testing still comes from the browser's own
  // transform (getScreenCTM), so this framing never changes which tree a click hits.

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

  // One axis of the view: start of the view, and whether it can still move towards lower or higher values.
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
    const scale = frame.fit * state.zoom; // CSS pixels per metre
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

  // Lays out the reference tree and its five competitors for display: each crown enlarged to twice its size, with
  // room around it for its brackets and tiles. The reference stays where it is; competitors move outward along their
  // true bearing until they barely touch. Competitors that would leave the visible map (or slip under the navigation
  // panel) are held wholly inside it. Positions are in metres and depend on zoom, because marks have a fixed screen size.
  const FOCUS_SCALE = 2; // every focus group is enlarged by the same factor, so sizes stay comparable between clicks
  const FOCUS_GAP_PX = 0; // extra screen gap between enlarged crowns (owner: 30, then 15, then 0)
  const FOCUS_TOUCH = .8; // crowns may overlap by 20% of their radii: the sprites' needle tips are sparse
  function spreadFocus(areaId, report, mpp, box) {
    const ids = [report.targetTreeId, ...report.topFive.map((n) => n.treeId)];
    const base = ids.map((id) => itemOf(areaId, id)).filter(Boolean);
    if (!base.length || base[0].treeId !== report.targetTreeId) return new Map();
    const lo = { u: box.x + box.rl + 34 * mpp, v: box.y + box.rt + 34 * mpp };
    const hi = { u: box.x + box.w - 34 * mpp, v: box.y + box.h - 48 * mpp };
    const nodes = base.map((item) => ({ ...item, radiusM: item.radiusM * FOCUS_SCALE }));
    // Near the map edge or the navigation panel, hold each competitor's whole crown inside the visible map (the group is
    // never shrunk), then separate again; a few rounds settle both rules.
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

  // Pushes overlapping discs apart (the first disc never moves) until each pair is at least pad apart.
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

  function renderMap() {
    const area = currentArea();
    if (!area) return;
    const side = area.extent.sideM;
    const box = viewBox(area);
    const mpp = 1 / box.scale; // metres per CSS pixel: status marks keep a constant on-screen size
    byId('map').setAttribute('viewBox', [box.x, box.y, box.w, box.h].map((n) => n.toFixed(4)).join(' '));
    setAttributes(byId('ground'), { x: 0, y: 0, width: side, height: side });
    const toU = (x) => x - area.extent.minXM;
    const toV = (z) => area.extent.minZM + side - z;
    const outline = area.polygon.map((p) => toU(p.xm) + ',' + toV(p.zm)).join(' ');
    byId('area-outline').setAttribute('points', outline);
    byId('area-clip-shape').setAttribute('points', outline);
    drawGrid(area, mpp, box.scale);

    const report = currentReport();
    const reference = referenceIn(area.areaId);
    const draft = new Set(draftOf(area.areaId));
    const focusIds = report ? new Set([report.targetTreeId, ...report.topFive.map((n) => n.treeId)]) : null;
    const crowns = [];
    const focusCrowns = [];
    const marks = [];
    const ranks = []; // { treeId, rank, x, y, chosen } for the rank toggle buttons (map metres)
    // The reference tree and its competitors are drawn at twice their crown size, on top, and pushed apart so each one
    // and its marks can be read. Display only: a faint line joins each moved crown to its true trunk position.
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
      const focused = item !== raw;
      const dimmed = focusIds && !focused;
      const lite = item.radiusM * box.scale < LITE_CROWN_PX;
      // A tree's drawing is rebuilt only when what it shows changes; otherwise the same nodes stay in place.
      const cacheKey = area.areaId + '|' + item.treeId;
      const drawKey = [item.status, item.u, item.v, item.radiusM, lite].join('|');
      let cached = crownCache.get(cacheKey);
      if (!cached || cached.drawKey !== drawKey) {
        const group = svg('g', { 'data-tree-id': item.treeId });
        group.append(svg('circle', { class: 'crown', cx: item.u, cy: item.v, r: item.radiusM }));
        group.append(...crownDecoration(item, lite));
        cached = { drawKey, node: group };
        crownCache.set(cacheKey, cached);
      }
      const className = 'tree s-' + item.status + (dimmed ? ' dimmed' : '');
      if (cached.node.getAttribute('class') !== className) cached.node.setAttribute('class', className);
      (focused ? focusCrowns : crowns).push(cached.node);
      marks.push(...statusMarks(item, mpp), ...pruneMarks(area.areaId, item, mpp));
    }
    if (report) {
      // Each rank tile (and its tick box) takes the first corner around its crown that does not cover another tile, the
      // Reference tag or the middle of a highlighted crown.
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
        // The SVG keeps the underline; the numbered tile is a real button on the rank layer, keyed by the exact tree ID.
        marks.push(underline(item, chosen ? 'ring-draft' : 'ring-competitor', mpp, 0));
        ranks.push({ treeId: entry.treeId, rank: rank + 1, x, y, chosen });
      });
    }
    crowns.push(...focusCrowns);
    const host = byId('crowns');
    const unchanged = host.childNodes.length === crowns.length && crowns.every((node, index) => host.childNodes[index] === node);
    if (!unchanged) host.replaceChildren(...crowns);
    // Fallen deadwood at the engine's map position of each record (a fixed-size symbol: not to scale, no fall direction).
    const s4 = s4Area(area.areaId);
    byId('deadwood').replaceChildren(...(s4 ? s4.canopy.fallenDeadwood.flatMap((record) => logMark(record.u, record.v, mpp)) : []));
    renderLightCells(area.areaId);
    renderGroundMark(area.areaId);
    byId('marks').replaceChildren(...marks);
    renderRankLayer(ranks, box);
    const chosen = selectedId() ? items.get(selectedId()) : null;
    const lit = chosen && !state.lock ? (chosen.treeId === reference ? referenceMarks(chosen, mpp, true) : inspectMarks(chosen, mpp, true)) : [];
    byId('selection').replaceChildren(...lit);
    byId('map-desc').textContent = 'North-up map of ' + areaName(area.areaId) + ', ' + area.areaM2Exact + ' square metres. ' +
      'Ecological year ' + state.view.source.ecologicalYear + '. Living trees ' + area.stand.livingCount + '. ' +
      (selectedId() ? 'Selected tree ' + selectedId() + '. ' : '') + 'Use the Trees tab to select a tree by keyboard.';
    byId('map-subtitle').textContent = area.label + ' · ' + fixed(area.areaHa, 4) + ' ha';
    // The experiment caveats live once, in the notice and its Details (UX-04/AC07); the map foot names only the view.
    byId('map-note').textContent = areaName(area.areaId) + ' · ' + (state.zoom === 1 ? 'Fit whole area' :
      fixed(state.zoom, 2) + '× Fit · view ' + fixed(box.uw, 1) + ' × ' + fixed(box.uh, 1) + ' m');
    byId('zoom-state').textContent = state.zoom === 1 ? 'Fit' : fixed(state.zoom, 2) + '×';
    renderScale(box.scale);
    renderMapStatus(reference, draft.size);
  }

  // A metric scale bar of a round length, following the current zoom.
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
    if (state.groundMode && state.lightOn) text = 'Inspect ground is on: click open ground to read its light · crowns still select trees · Esc stops';
    else if (state.batch) text = 'Adding ' + (state.batch.confirmed.length + 1) + ' of ' + state.batch.ids.length + ' to the Fell plan…';
    else if (view.approved && approvedPrunes().length > 0) text = workTotals(view.approved.orders.length, approvedPrunes().length) + ' approved · nothing done yet';
    else if (view.approved) text = plural(view.approved.orders.length, 'approved Fell', 'approved Fell') + ' · still standing';
    else if (reference) text = 'Reference ' + reference + ' · ' + plural(drafts, 'draft choice', 'draft choices');
    box.hidden = text === '';
    box.textContent = text;
  }

  // Original procedural Sitka spruce studies for this page, top-down: a dark crown mass, four tiers of whorled boughs lit
  // from the north-west, feathered needle sprays and a pale leader. Six seeded variants are drawn once into shared symbols
  // at unit radius; each tree reuses one, scaled to its true server crown radius and turned by its own ID seed. Shape and
  // tint are drawing only: no age, health, genetics or habitat value is inferred from them.
  // Top-down spruce crowns cut from the owner-supplied concept board "Spruce Crown Assets - Top-Down Concept Board"
  // (AI-generated image provided by the owner, 22 Nov 2026, #885). Three crowns, each a 256 px WebP with alpha, cut to
  // the crown radius. Variants 3-5 are mirrored copies. Artistic only: no health, age or habitat meaning.
  const SPRUCE_SPRITES = [
    'data:image/webp;base64,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',
    'data:image/webp;base64,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',
    'data:image/webp;base64,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',
  ];
  const SPRUCE_VARIANTS = 6;
  const SLATE = [27, 36, 48];
  const crownCache = new Map(); // area|tree → { drawKey, node }: drawing only, rebuilt whenever the server's item changes

  function canopyHash(a, b, c) {
    let h = (Math.imul(a, 0x9e3779b1) + Math.imul(b, 0x85ebca6b) + Math.imul(c, 0xc2b2ae35) + 0x27d4eb2f) >>> 0;
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

  function studyRandom(variant) {
    let state = (885 + variant * 7919) >>> 0;
    return () => { state = (Math.imul(state, 1664525) + 1013904223) >>> 0; return state / 4294967296; };
  }

  // One crown variant at unit radius: the sprite fills the square -1..1, mirrored for variants 3-5.
  function drawSpruce(variant) {
    const g = svg('g', { id: 'spruce-' + variant });
    g.append(svg('image', { href: SPRUCE_SPRITES[variant % SPRUCE_SPRITES.length], x: -1, y: -1, width: 2, height: 2,
      transform: variant >= SPRUCE_SPRITES.length ? 'scale(-1 1)' : null, preserveAspectRatio: 'none' }));
    return g;
  }

  function drawDeadSpruce(variant) {
    const random = studyRandom(variant + 80);
    let d = '';
    for (let b = 0; b < 10; b++) {
      const a = b * Math.PI / 5 + variant * .49, r = .65 + random() * .25, u = Math.cos(a), v = Math.sin(a);
      d += 'M0 0L' + n3(u * r) + ' ' + n3(v * r);
      for (const t of [.36, .58, .76]) for (const side of [-1, 1]) {
        const w = (1 - t) * .21 * side;
        d += 'M' + n3(u * r * t) + ' ' + n3(v * r * t) + 'L' + n3(u * r * (t + .08) - v * w) + ' ' + n3(v * r * (t + .08) + u * w);
      }
    }
    const g = svg('g', { id: 'bare-branches-' + variant });
    g.append(svg('path', { d, fill: 'none', stroke: '#a4ada9', 'stroke-width': .028, 'stroke-linecap': 'round' }),
      svg('circle', { r: .038, fill: '#c3c6ba' }));
    return g;
  }

  function drawCanopyDefs() {
    const defs = byId('map').querySelector('defs');
    const shadow = svg('radialGradient', { id: 'canopy-shadow' });
    shadow.append(svg('stop', { offset: '25%', 'stop-color': '#020b16', 'stop-opacity': .62 }),
      svg('stop', { offset: '70%', 'stop-color': '#020b16', 'stop-opacity': .38 }),
      svg('stop', { offset: '100%', 'stop-color': '#020b16', 'stop-opacity': 0 }));
    defs.append(shadow);
    // Two periodic value-noise lattices at different scales give the slate stand floor a subtle, meaningless texture.
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
    for (let variant = 0; variant < SPRUCE_VARIANTS; variant++) defs.append(drawSpruce(variant), drawDeadSpruce(variant));
  }

  // The tree's drawing, painted over its transparent, true-radius hit disc. Decorations never take pointer events.
  function crownDecoration(item, lite) {
    const { u, v } = item;
    const r = item.radiusM;
    if (item.status === 'Felled') return [
      svg('circle', { class: 'stump-disc deco', cx: u, cy: v, r }),
      svg('circle', { class: 'stump-ring deco', cx: u, cy: v, r: r * .65 }),
      svg('circle', { class: 'stump-ring deco', cx: u, cy: v, r: r * .32 }),
    ];
    const seed = canopySeed(item.treeId);
    const variant = seed % SPRUCE_VARIANTS;
    const stretch = .92 + .16 * canopyRandom(seed, 13);
    const turn = Math.round(canopyRandom(seed, 21) * 360);
    const dead = item.status === 'NaturalDead';
    const id = dead ? 'bare-branches-' + (seed % 6) : 'spruce-' + variant;
    return [svg('circle', { class: 'deco', cx: n3(u + r * .18), cy: n3(v + r * .22), r: n3(r * 1.12),
      fill: 'url(#canopy-shadow)', opacity: dead ? .4 : .9 }),
      svg('use', { class: 'deco', href: '#' + id,
        transform: 'translate(' + n3(u) + ' ' + n3(v) + ') rotate(' + turn + ') scale(' +
          n3(r * Math.min(1, stretch)) + ' ' + n3(r * Math.min(1, 1 / stretch)) + ')' })];
  }

  // ---- status marks: shared by the map and the legend; sizes in CSS pixels via mpp (metres per pixel) ---------------

  // Crown Brackets marks (#885 owner choice, design by Astra): corner frames, side brackets, short underlines and square
  // tiles instead of full rings, so the crown itself stays visible. Sizes are CSS pixels converted with mpp.
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

  // Each kind of tile has its own corner so combined states never overpaint: rank north-east, work state south-east,
  // crop south-west, reference or inspected north-west.
  const TILE = { rank: -45, status: 45, crop: 135, reference: -135 };
  const RANK_ANGLES = [-45, 45, -135, 135, 0, 180, -90, 90]; // rank tile corners, tried in this order

  function tileCentre(item, angleDeg, mpp) {
    const angle = angleDeg * Math.PI / 180, distance = item.radiusM + 9 * mpp;
    return [item.u + Math.cos(angle) * distance, item.v + Math.sin(angle) * distance];
  }

  function badge(item, angleDeg, text, kind, mpp, extra) {
    const [cx, cy] = tileCentre(item, angleDeg, mpp);
    const g = svg('g', { class: 'badge b-' + kind + (extra ? ' ' + extra : '') });
    g.append(svg('rect', { class: 'badge-bg', x: n3(cx - 10 * mpp), y: n3(cy - 10 * mpp), width: n3(20 * mpp), height: n3(20 * mpp),
      rx: n3(mpp) }),
      svg('text', { class: 'badge-text', x: n3(cx), y: n3(cy + .5 * mpp), 'font-size': n3(11 * mpp) }, text));
    return g;
  }

  function tag(x, y, text, mpp) {
    const width = (text.length * 6.4 + 14) * mpp, height = 19 * mpp;
    const g = svg('g', { class: 'map-tag' });
    g.append(svg('rect', { x: n3(x - width / 2), y: n3(y - height / 2), width: n3(width), height: n3(height), rx: n3(4 * mpp) }),
      svg('text', { x: n3(x), y: n3(y + .5 * mpp), 'font-size': n3(11 * mpp) }, text));
    return g;
  }

  // A ticked draft choice (legend icon): a filled rank tile with a tick disc on its corner, as on the map's rank buttons.
  // A tick means "in my draft", never approval.
  function tick(item, mpp, angle) {
    const [rx, ry] = tileCentre(item, angle ?? TILE.rank, mpp);
    const cx = rx + 11 * mpp, cy = ry - 11 * mpp;
    const p = (dx, dy) => n3(cx + dx * mpp) + ' ' + n3(cy + dy * mpp);
    return [svg('circle', { class: 'tick-box', cx: n3(cx), cy: n3(cy), r: n3(7 * mpp) }),
      svg('path', { class: 'tick', d: 'M ' + p(-3, 0) + ' L ' + p(-1, 2.5) + ' L ' + p(3.5, -3) })];
  }

  function statusMarks(item, mpp) {
    const marks = [];
    if (item.isCrop) marks.push(badge(item, TILE.crop, 'C', 'crop', mpp));
    if (item.status === 'PendingFell') {
      marks.push(underline(item, 'ring-pending', mpp, 4), badge(item, TILE.status, 'P', 'pending', mpp));
    } else if (item.status === 'ApprovedFell') {
      marks.push(underline(item, 'ring-approved', mpp, 4), underline(item, 'ring-approved', mpp, 8),
        badge(item, TILE.status, 'A', 'approved', mpp));
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

  // Legend icons only: on the map the numbered tile is a rank toggle button (renderRankLayer).
  function competitorMarks(item, rank, chosen, mpp, angle) {
    const marks = [underline(item, chosen ? 'ring-draft' : 'ring-competitor', mpp, 0)];
    marks.push(badge(item, angle ?? TILE.rank, String(rank), 'rank', mpp, chosen ? 'chosen' : null));
    if (chosen) marks.push(...tick(item, mpp, angle));
    return marks;
  }

  // UX-02: the five rank tiles as real toggle buttons over the map. Each carries the exact engine tree ID of its
  // competitor (data-rank-toggle); a click toggles only that ID in the local Fell draft, through the same toggleDraft as the
  // inspector checkbox, so the two always agree. Positions are display only (the enlarged, spread focus layout) and are
  // never used to find a tree. A click here never reaches the map: no hit-test, no reference change, no request.
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
      // CSSOM positioning (allowed by the page's style-src policy, unlike a style attribute). Map metres to pane pixels.
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

  // Stage 4 Work Plan state of a living tree, from the engine read-out: a scissors tile in the work-state corner, with the
  // Fell convention for the line (dashed = pending, double = approved). A tree never holds both a Fell and a Prune order.
  function pruneMarks(areaId, item, mpp) {
    const s4 = s4Tree(areaId, item.treeId);
    if (!s4 || !s4.prunePlanState) return [];
    return s4.prunePlanState === 'PendingPrune'
      ? [underline(item, 'ring-pending', mpp, 4), badge(item, TILE.status, '✂', 'pending', mpp)]
      : [underline(item, 'ring-approved', mpp, 4), underline(item, 'ring-approved', mpp, 8), badge(item, TILE.status, '✂', 'approved', mpp)];
  }

  // A fallen log: a short horizontal stem with an end-grain disc, a fixed 26 × 8 px symbol. Its shape (not its colour)
  // tells it apart from a standing dead crown (D) and a stump (X). It implies no length, direction, decay or value.
  function logMark(u, v, mpp) {
    const half = 13 * mpp, h = 4 * mpp;
    const body = 'M ' + n3(u - half) + ' ' + n3(v - h) + ' H ' + n3(u + half - h) + ' A ' + n3(h) + ' ' + n3(h) + ' 0 0 1 ' +
      n3(u + half - h) + ' ' + n3(v + h) + ' H ' + n3(u - half) + ' Z';
    const g = svg('g', { class: 'log' });
    g.append(svg('path', { class: 'log-halo', d: body }), svg('path', { class: 'log-body', d: body }),
      svg('ellipse', { class: 'log-end', cx: n3(u - half), cy: n3(v), rx: n3(h * .75), ry: n3(h) }),
      svg('ellipse', { class: 'log-ring', cx: n3(u - half), cy: n3(v), rx: n3(h * .35), ry: n3(h * .5) }));
    return [g];
  }

  // The Light Guide (UX-06): light on the forest floor. Every polygon-clipped 2.5 m cell of the shown area with a positive
  // engine light index is filled with its own continuous colour and opacity (lightPaint); a cell with L = 0 is not drawn at
  // all. The layer lies under the crowns, takes no pointer events and carries no numbers. Values come from the read-out.
  function renderLightCells(areaId) {
    const host = byId('light-cells');
    const s4 = s4Area(areaId);
    if (!state.lightOn || !s4) { host.replaceChildren(); return; }
    const nodes = [];
    for (const cell of s4.ground) {
      const paint = lightPaint(cell.lightIndex);
      if (!paint) continue;
      nodes.push(svg('polygon', { class: 'light-cell', 'data-cell': cell.column + ':' + cell.row, 'data-light': cell.lightIndex,
        fill: paint.colour, 'fill-opacity': paint.opacity, points: cell.polygon.map((p) => n3(p.u) + ',' + n3(p.v)).join(' ') }));
    }
    host.replaceChildren(...nodes);
  }

  // The outline of the inspected ground cell, above the crowns so it can be found; display only.
  function renderGroundMark(areaId) {
    const g = state.ground;
    const cell = state.lightOn && state.groundMode && g && !g.missing && g.areaId === areaId ? groundCell(areaId, g.column, g.row) : null;
    const points = cell ? cell.polygon.map((p) => n3(p.u) + ',' + n3(p.v)).join(' ') : null;
    byId('ground-mark').replaceChildren(...(points ? [svg('polygon', { class: 'ground-pick-halo', points }),
      svg('polygon', { class: 'ground-pick', points })] : []));
  }

  // The legend ramp (UX-09): the same lightPaint, sampled densely at the low end, over the map's floor colour.
  const RAMP_SAMPLES = [0, 0.002, 0.005, 0.01, 0.02, 0.05, 0.1, 0.2, 0.35, 0.5, 0.65, 0.8, 0.9, 1];
  function renderLightLegend() {
    const legend = byId('light-legend');
    legend.hidden = !state.lightOn;
    byId('light-toggle').setAttribute('aria-pressed', String(state.lightOn));
    byId('light-toggle-text').textContent = 'Light Guide: ' + (state.lightOn ? 'On' : 'Off');
    const ground = byId('ground-mode');
    ground.hidden = !state.lightOn;
    ground.setAttribute('aria-pressed', String(state.lightOn && state.groundMode));
    byId('ground-mode-text').textContent = 'Inspect ground: ' + (state.lightOn && state.groundMode ? 'On' : 'Off');
    const ramp = byId('light-ramp');
    if (!state.lightOn || ramp.childNodes.length > 0) return;
    const gradient = svg('linearGradient', { id: 'light-ramp-fill', x1: 0, y1: 0, x2: 1, y2: 0 });
    for (const value of RAMP_SAMPLES) {
      const paint = lightPaint(value);
      gradient.append(svg('stop', { offset: value, 'data-l': value, 'stop-color': paint ? paint.colour : '#7f3b4a',
        'stop-opacity': paint ? paint.opacity : 0 }));
    }
    const defs = svg('defs');
    defs.append(gradient);
    ramp.append(defs, svg('rect', { x: 0, y: 0, width: 100, height: 10, fill: 'rgb(' + SLATE.join(',') + ')' }),
      svg('rect', { x: 0, y: 0, width: 100, height: 10, fill: 'url(#light-ramp-fill)' }));
  }

  // The ground inspector (UX-07): a short summary of one engine cell, with the IDs and provenance behind two disclosures.
  // Inspecting the ground never changes a selection, a Fell draft or the Light Guide. Nothing is estimated for a spot
  // that has no cell.
  function renderGround() {
    const panel = byId('ground-panel');
    const on = state.lightOn && state.groundMode;
    panel.hidden = !on;
    byId('map-pane').classList.toggle('ground-mode', on);
    placeGroundPanel(panel);
    if (!on) return;
    const g = state.ground && state.ground.areaId === state.areaId ? state.ground : null;
    const cell = g && !g.missing ? groundCell(g.areaId, g.column, g.row) : null;
    const summary = [];
    if (!state.stage4) {
      byId('ground-heading').textContent = areaName(state.areaId);
      summary.push(el('p', null, 'The ground light read-out is unavailable for this view. Choose Re-read; nothing is estimated.'));
    } else if (!g) {
      byId('ground-heading').textContent = areaName(state.areaId) + ' · choose a spot';
      summary.push(el('p', null, 'Click open ground (a spot with no crown over it) inside the area to read its modelled ground light. ' +
        'Clicking a crown still selects that tree.'));
    } else if (!cell) {
      byId('ground-heading').textContent = areaName(state.areaId) + ' · no sample';
      summary.push(el('p', null, 'No modelled ground sample at that spot (it is outside this area\'s light cells). No value is shown or estimated.'));
    } else {
      byId('ground-heading').textContent = areaName(g.areaId) + ' · ground at X ' + fixed(cell.centreXM, 1) + ', Z ' + fixed(cell.centreZM, 1);
      const paint = lightPaint(cell.lightIndex);
      const swatch = svg('svg', { class: 'ground-swatch', viewBox: '0 0 10 10', 'aria-hidden': 'true', focusable: 'false' });
      swatch.append(svg('rect', { width: 10, height: 10, fill: 'rgb(' + SLATE.join(',') + ')' }));
      if (paint) swatch.append(svg('rect', { width: 10, height: 10, fill: paint.colour, 'fill-opacity': paint.opacity }));
      const n = cell.contributingTreeIds.length;
      summary.push(el('p', { class: 'ground-level' }, swatch, lightWord(cell.lightIndex)),
        el('dl', null,
          el('dt', null, 'Light index'), el('dd', null, fixed(cell.lightIndex, 3) + ' on a 0–1 scale (modelled; 1 = unshaded)'),
          el('dt', null, 'Canopy cover'), el('dd', null, pct(cell.geometricCoveredFraction) + ' of this cell under living crowns'),
          el('dt', null, 'Shading trees'), el('dd', null, n === 0 ? 'none' : String(n))),
        el('p', { class: 'small-note' }, 'Light and canopy cover are separate engine quantities.'));
    }
    byId('ground-summary').replaceChildren(...summary);
    byId('ground-trees').hidden = !cell;
    byId('ground-tech').hidden = !cell;
    if (!cell) return;
    const models = state.stage4.models;
    const contributors = cell.contributingTreeIds;
    byId('ground-trees-summary').textContent = 'Contributing trees (' + contributors.length + ')';
    byId('ground-tree-ids').replaceChildren(...(contributors.length === 0 ? ['none (no living crown shades this cell centre)'] :
      contributors.flatMap((id, i) => [i > 0 ? ', ' : null, el('span', { class: 'id' }, id)].filter((part) => part !== null))));
    byId('ground-facts').replaceChildren(
      pair('Modelled light index', fixed(cell.lightIndex, 4) + ' (0–1)'),
      pair('Overhead cover fraction', fixed(cell.geometricCoveredFraction, 4)),
      pair('Cell', 'column ' + cell.column + ', row ' + cell.row),
      pair('Grid', fixed(models.lightCellM, 1) + ' m cells · ' + fixed(cell.clippedAreaM2, 2) + ' m² of this cell inside the area'),
      pair('Cell centre', 'X ' + fixed(cell.centreXM, 2) + ' m east, Z ' + fixed(cell.centreZM, 2) + ' m north'),
      pair('Cross-border trees', String(cell.crossBorderTrees)),
      pair('Light model', models.lightLabel + ' · ' + models.lightModelVersion),
      pair('Calibrated PAR', models.calibratedPar ? 'yes' : 'no'));
    byId('ground-caveat').textContent = label('MultiLightGuideLabel') + '. ' + models.notCalibratedCaveat;
  }

  // Where the ground panel sits (UX-01 over UX-08): while a tree is inspected in this area, directly below its primary
  // block (reference, ranks 1-5, Add), so Inspect ground never pushes those below the fold; otherwise at the top of the
  // Tree pane. Moving it keeps keyboard focus if focus was inside it.
  function placeGroundPanel(panel) {
    const id = selectedId();
    const inspected = Boolean(id && treeIn(state.areaId, id) && itemOf(state.areaId, id));
    const slot = byId('ground-slot');
    const pane = byId('pane-tree');
    const home = inspected ? slot : pane;
    if (panel.parentNode === home && (inspected || pane.firstElementChild === panel)) return;
    const focused = panel.contains(document.activeElement) ? document.activeElement : null;
    if (inspected) slot.append(panel);
    else pane.prepend(panel);
    if (focused) focused.focus({ preventScroll: true });
  }

  // A ground action (reading a spot; turning Inspect ground on with no tree inspected) brings the ground panel into view
  // with the smallest scroll; the next tree selection puts the inspector back at the top.
  function focusGroundPanel() {
    byId('ground-heading').focus({ preventScroll: true });
    byId('ground-panel').scrollIntoView({ block: 'nearest' });
  }

  // Light Guide visibility. Turning it off also ends Inspect ground; turning it on never changes what a click does.
  function setLightGuide(on) {
    state.lightOn = on;
    if (!on) { state.groundMode = false; state.ground = null; }
    renderAll();
    announce(on ? 'Light Guide on. Clicking a crown still selects the tree; choose Inspect ground to read the light at a spot.' :
      'Light Guide off.');
  }

  // Inspect ground (UX-08): an explicit mode, only with the Light Guide on. While it is on, a map click with no crown under
  // it reads the ground there; dialogs, rank buttons and crowns still come first (see onMapClick). Leaving it keeps the
  // Light Guide exactly as it was.
  function setGroundMode(on, moveFocus) {
    if (on && !state.lightOn) return;
    state.groundMode = on;
    if (!on) state.ground = null;
    renderAll();
    // With a tree inspected the panel waits below the primary block, so focus stays on the pressed toggle (its On state,
    // the status line and the announcement say what a click now does) and the inspector does not scroll.
    if (moveFocus && on && byId('ground-panel').parentNode !== byId('ground-slot')) focusGroundPanel();
    else if (moveFocus) byId('ground-mode').focus();
    announce(on ? 'Inspect ground on: click open ground with no crown over it to read its light. Clicking a crown still selects the tree; press Escape or Stop to return.' :
      'Inspect ground off: clicking open ground no longer reads its light. The Light Guide is still ' + (state.lightOn ? 'on' : 'off') + '.');
  }

  // Which engine light cell (if any) contains a map point. The cells are the read-out's own clipped squares; this only
  // matches a click to one of them, like the hit-test matches a click to a crown.
  function insidePolygon(points, u, v) {
    let inside = false;
    for (let i = 0, j = points.length - 1; i < points.length; j = i++) {
      const a = points[i], b = points[j];
      if ((a.v > v) !== (b.v > v) && u < (b.u - a.u) * (v - a.v) / (b.v - a.v) + a.u) inside = !inside;
    }
    return inside;
  }

  function inspectPoint(areaId, u, v) {
    const s4 = s4Area(areaId);
    const cell = s4 ? s4.ground.find((c) => insidePolygon(c.polygon, u, v)) || null : null;
    state.ground = cell ? { areaId, column: cell.column, row: cell.row } : { areaId, missing: true };
    showPane('tree');
    renderAll();
    focusGroundPanel();
    announce(cell ? 'Ground at X ' + fixed(cell.centreXM, 1) + ', Z ' + fixed(cell.centreZM, 1) + ': ' + lightWord(cell.lightIndex) +
      ', modelled light index ' + fixed(cell.lightIndex, 3) + ', canopy cover ' + pct(cell.geometricCoveredFraction) + ', ' +
      plural(cell.contributingTreeIds.length, 'shading tree', 'shading trees') + '. Your selection and Fell draft are unchanged.'
      : s4 ? 'No modelled ground sample at that spot; nothing is estimated.' : 'The ground light read-out is unavailable for this view.');
  }

  // Grid lines every 5 m (10 m when zoomed far out) of the experiment frame across the light exterior, faintly inside the
  // stand, labelled in the same X (east) and Z (north) metres as the tree table.
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

  // ---- the legend: the same drawings and marks as the map, at a fixed small scale -------------------------------------

  function legendIcon(draw) {
    const icon = svg('svg', { viewBox: '-1.9 -1.9 3.8 3.8', 'aria-hidden': 'true', focusable: 'false' });
    const mpp = .075;
    const item = { u: 0, v: 0, radiusM: 1, treeId: 'legend', status: 'Living', isCrop: false };
    icon.append(svg('circle', { class: 'swatch-ground', cx: 0, cy: 0, r: 1.85 }));
    icon.append(...draw(item, mpp));
    return icon;
  }

  function legendTree(item, status) {
    return crownDecoration(Object.assign({}, item, { status, treeId: 'legend-' + status }), false);
  }

  function renderLegend() {
    const rows = [
      ['Living spruce', 'Standing, no work order', (item) => legendTree(item, 'Living')],
      ['Crop tree · C', 'Retained management tree', (item, mpp) => [...legendTree(item, 'Living'),
        ...statusMarks(Object.assign({}, item, { isCrop: true }), mpp)]],
      ['Reference · R', 'Competition is measured here', (item, mpp) => [...legendTree(item, 'Living'), ...referenceMarks(item, mpp, false)]],
      ['Inspected tree · I', 'Dotted side brackets', (item, mpp) => [...legendTree(item, 'Living'), ...inspectMarks(item, mpp, false)]],
      ['Competitor · 1–5', 'Ranked current neighbours', (item, mpp) => [...legendTree(item, 'Living'), ...competitorMarks(item, 1, false, mpp)]],
      ['Fell choice · ✓', 'Local draft; not yet queued', (item, mpp) => [...legendTree(item, 'Living'), ...competitorMarks(item, 1, true, mpp)]],
      ['Pending Fell · P', 'Queued, not approved', (item, mpp) => [...legendTree(item, 'Living'),
        ...statusMarks(Object.assign({}, item, { status: 'PendingFell' }), mpp)]],
      ['Approved Fell · A', 'Still standing until Execute', (item, mpp) => [...legendTree(item, 'Living'),
        ...statusMarks(Object.assign({}, item, { status: 'ApprovedFell' }), mpp)]],
      ['Felled tree · X', 'Executed work; stump remains', (item, mpp) => [...legendTree(item, 'Felled'),
        ...statusMarks(Object.assign({}, item, { status: 'Felled' }), mpp)]],
      ['Naturally dead · D', 'Annual mortality, not felling', (item, mpp) => [...legendTree(item, 'NaturalDead'),
        ...statusMarks(Object.assign({}, item, { status: 'NaturalDead' }), mpp)]],
      ['Fallen deadwood · log', 'One log per natural death; not to scale', (item, mpp) => logMark(0, 0, mpp * 1.6)],
      ['Pending Prune · ✂', 'Queued; no crown cut yet', (item, mpp) => [...legendTree(item, 'Living'),
        underline(item, 'ring-pending', mpp, 4), badge(item, TILE.status, '✂', 'pending', mpp)]],
      ['Approved Prune · ✂', 'Crown cut only on Execute', (item, mpp) => [...legendTree(item, 'Living'),
        underline(item, 'ring-approved', mpp, 4), underline(item, 'ring-approved', mpp, 8), badge(item, TILE.status, '✂', 'approved', mpp)]],
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

  function renderOverlap() {
    const box = byId('overlap');
    const overlap = state.overlap;
    if (!overlap || overlap.areaId !== state.areaId) {
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
        ' — ' + statusText(candidate) + (index === 0 ? ' (topmost)' : ''));
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
    const isReference = referenceIn(areaId) === id;
    byId('insp-role').textContent = isReference ? 'Reference tree' : 'Selected tree';
    byId('insp-area').textContent = areaName(areaId);
    byId('insp-id').textContent = tree.treeId;
    const chip = byId('insp-chip');
    chip.textContent = item.isCrop ? 'C · ' + label('CropRetained') : baseStatusText(item);
    chip.setAttribute('class', 'chip s-' + item.status + (item.isCrop ? ' crop' : ''));
    chip.setAttribute('title', chip.textContent);
    chip.hidden = !item.isCrop && item.status === 'Living';
    byId('insp-summary').textContent = 'Sitka spruce · ' + baseStatusText(item);
    // Compact reference summary (UX-01); age, position and the full record stay in "Tree record" below.
    byId('insp-key').replaceChildren(
      keyFact('DBH', fixed(tree.dbhCm, 1), 'cm'),
      keyFact('Height', fixed(tree.heightM, 1), 'm'),
      keyFact('Crown r', fixed(tree.crownRadiusM, 2), 'm'));
    byId('inspect-close').setAttribute('aria-label', 'Close inspection of ' + tree.treeId + ' (Escape)');
    byId('insp-pinned').textContent = isReference ? 'Reference stays pinned while choosing' : '';
    const facts = [
      pair('Species', 'Sitka spruce'),
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
    if (s4) {
      facts.push(pair('Life state', s4.lifeState + (s4.deathCause ? ' · ' + s4.deathCause + ', year ' + s4.deathYear : '')));
      facts.push(pair('Pruning lifts', s4.pruningLifts + ' / ' + s4.maxPruningLifts + (s4.pruningLifts > 0
        ? ' · crown base ' + fixed(s4.crownBaseHeightM, 1) + ' m · last lift year ' + s4.lastPruningYear : '')));
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
    renderPruneBox(areaId, tree, item, s4);
  }

  // Pruning of the inspected living tree: lifts so far, and the engine's eligibility for the next scheduled lift with its own
  // reason. The page decides nothing about eligibility; it only refuses to send while the plan state already says no.
  function renderPruneBox(areaId, tree, item, s4) {
    const box = byId('prune-box');
    box.hidden = !tree.isLiving || !s4;
    if (box.hidden) return;
    byId('prune-lifts').textContent = 'Pruning lifts ' + s4.pruningLifts + ' / ' + s4.maxPruningLifts;
    let status;
    if (s4.prunePlanState === 'PendingPrune') status = '✂ Pending Prune — no crown is cut until the approved plan is executed.';
    else if (s4.prunePlanState === 'ApprovedPrune') status = '✂ Approved Prune — the crown is cut only when the plan is executed.';
    else if (s4.pruneEligible) status = 'Eligible now: next lift raises the crown base to ' + fixed(s4.nextPruneTargetM, 1) + ' m.';
    else status = 'Not eligible: ' + s4.pruneRefusalReason + (s4.nextPruneEligibleYear !== null ? ' (next lift possible in year ' +
      s4.nextPruneEligibleYear + ')' : '') + '.';
    byId('prune-status').textContent = status;
    const add = byId('prune-add');
    add.textContent = '✂ ' + label('MultiAddToPrunePlan');
    add.hidden = Boolean(s4.prunePlanState);
    const remove = byId('prune-remove');
    remove.hidden = s4.prunePlanState !== 'PendingPrune';
    remove.textContent = 'Remove Prune from Work Plan';
    remove.setAttribute('aria-label', 'Remove the pending Prune of ' + tree.treeId);
    byId('prune-why').textContent = pruneAvailability(areaId, item, s4).why || label('MultiPrunePendingNote') + '.';
  }

  // Whether "Add to Prune plan" can send now, and the plain reason when not: the engine's refusal reason first.
  function pruneAvailability(areaId, item, s4) {
    if (!s4 || !item) return { ok: false, why: 'Select a living tree.' };
    if (!s4.pruneEligible) return { ok: false, why: 'Not eligible (engine): ' + s4.pruneRefusalReason + '.' };
    if (s4.prunePlanState) return { ok: false, why: 'Already in the Work Plan as a Prune.' };
    if (item.status === 'PendingFell' || item.status === 'ApprovedFell') return { ok: false, why: 'This tree has a Fell order; one order per tree.' };
    if (state.view.approved) return { ok: false, why: 'The plan is approved, so editing is locked. Cancel the approved plan to change it.' };
    return { ok: true, why: null };
  }

  function renderPruneGate() {
    const areaId = state.areaId;
    const id = selectedId();
    const s4 = s4Tree(areaId, id);
    byId('prune-add').disabled = !pruneAvailability(areaId, id ? itemOf(areaId, id) : null, s4).ok;
  }

  // ---- read-only competition: any living tree, before or after any Crop designation ------------------------------------

  function competitionTag() {
    return JSON.stringify([state.view ? currentFence() : null, state.competitionTarget]);
  }

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
    // A late answer for an older selection, area or view can never paint numbers on a newer one. While a command is in
    // flight the read-only diagnostic draws nothing (not even a refusal's view): the command's answer, or its lock, decides
    // what is shown, and that answer re-reads the diagnostic.
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

  // The release reason is offered only for top-five competitors of an inspected, retained Crop reference in this area.
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
      // Fewer than five is shown as it is: no padding rows.
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
            return el('li', { class: 'comp-row' + (ticked ? ' chosen' : '') + (why ? ' blocked' : '') },
              el('label', null, input,
                el('span', null,
                  el('span', { class: 'comp-id' }, el('span', { class: 'rank' }, String(index + 1)), el('span', { class: 'mono' }, n.treeId)),
                  el('span', { class: 'comp-meta' }, fixed(n.dbhCm, 1) + ' cm · ' + fixed(n.sharePercent, 1) + '%'),
                  why ? el('span', { class: 'comp-why' }, why) : null),
                el('span', { class: 'comp-num' }, fixed(n.distanceM, 2) + ' m'),
                el('span', { class: 'comp-num' }, fixed(n.contribution, 4), el('span', { class: 'comp-strength' }, n.strength))));
          })));
      }
      more.push(el('p', { class: 'competition-totals' }, 'Current total Hegyi CI ' + fixed(report.currentTotal, 4) +
        (report.targetPlannedForFell ? ' · This tree is planned for Fell: no release estimate.' :
          ' → ' + fixed(report.hypotheticalTotal, 4) + ' if its pending or approved neighbours were felled.')));
      more.push(el('p', { class: 'small-note' }, 'The planned figure counts only queued or approved orders, never unsent draft choices.'));
      if (isStage4()) more.push(el('p', { class: 'small-note' }, label('MultiCompetitionAreaOnly') + '.'));
      more.push(el('p', { class: 'small-note' }, 'Other neighbours: ' + report.remainingCount + ' · combined ' +
        fixed(report.remainingSharePercent, 1) + '% share. Rank is current competition rank, not health or a felling recommendation; ' +
        'strength describes share of CI, not tree health.'));
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

  // ---- the reason picker -----------------------------------------------------------------------------------------------

  function reasonToSend(forReferenceItself) {
    // Explicit precedence: note if present, else the ticked release label, else Owner-selected felling.
    // This is operator intent only; the existing API still requires a nonempty reason.
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
    box.textContent = message === null ? '' : message;
    for (const id of ['reason-release', 'reason']) if (message === null) byId(id).removeAttribute('aria-invalid');
    if (message !== null) byId('reason').setAttribute('aria-invalid', 'true');
  }

  function resetReasonPicker() {
    byId('reason-release').checked = false;
    byId('reason-details').open = false;
    byId('reason').value = '';
    renderReasonPicker();
    showReasonError(null);
  }

  // ---- the local Fell draft --------------------------------------------------------------------------------------------

  // Drops draft choices the latest view or competition reading no longer allows, and says which and why. Draft choices are
  // never re-added silently; a reference that is no longer living takes its draft with it.
  function reconcileDrafts() {
    const dropped = [];
    for (const areaId of Object.keys(state.drafts)) {
      const draft = state.drafts[areaId];
      const reference = referenceIn(areaId);
      if (!reference || draft.ref !== reference) {
        if (draft.ids.length > 0) dropped.push(draft.ids.join(', ') + ' (reference ' + draft.ref + ' is no longer a living selection)');
        delete state.drafts[areaId];
        continue;
      }
      const comp = state.competition;
      const report = comp && comp.areaId === areaId && comp.report && comp.report.targetTreeId === reference ? comp.report : null;
      const ranked = report ? new Set(report.topFive.map((n) => n.treeId)) : null;
      draft.ids = draft.ids.filter((id) => {
        const why = blockedReason(areaId, id) || (ranked && !ranked.has(id) ? 'no longer among the five strongest' : null);
        if (why) dropped.push(id + ' (' + why.toLowerCase() + ')');
        return !why;
      });
    }
    if (dropped.length === 0) return '';
    const text = 'Removed from the Fell draft: ' + dropped.join('; ') + '.';
    state.draftNotice = { areaId: state.areaId, text };
    return text;
  }

  function toggleDraft(treeId, on) {
    const areaId = state.areaId;
    const reference = referenceIn(areaId);
    const report = currentReport();
    if (!reference || state.batch || state.lock || !report) return;
    if (on && (blockedReason(areaId, treeId) || !report.topFive.some((n) => n.treeId === treeId))) return;
    let draft = state.drafts[areaId];
    if (!draft || draft.ref !== reference) draft = state.drafts[areaId] = { ref: reference, ids: [] };
    draft.ids = draft.ids.filter((id) => id !== treeId);
    if (on) draft.ids.push(treeId);
    state.draftNotice = null;
    renderAll();
    const n = draft.ids.length;
    announce((on ? 'Ticked ' : 'Unticked ') + treeId + '. ' + plural(n, 'tree', 'trees') + ' selected for Fell, draft only.');
  }

  function clearDraft() {
    const areaId = state.areaId;
    const n = draftOf(areaId).length;
    if (state.batch || n === 0) return;
    delete state.drafts[areaId];
    state.draftNotice = null;
    renderAll();
    announce('Cleared ' + plural(n, 'draft choice', 'draft choices') + '. Nothing was sent; the Work Plan is unchanged.');
    const first = document.querySelector('[data-draft]:not(:disabled)');
    if (first) first.focus();
  }

  // ---- adding exact trees to the Work Plan, one fenced command at a time ------------------------------------------------
  //
  // The tree list is frozen when Add is chosen. One operation-wide guard holds for the whole sequence: area and reference
  // switches, plan edits, year advance and other writes wait, while pan, zoom and reading stay available. Each request
  // carries the fence of the latest confirmed view. The sequence stops at the first refusal or unconfirmed outcome and never
  // retries a write by itself; it reports confirmed, refused or unconfirmed, and not-attempted IDs separately.

  async function addSequence(areaId, reference, ids, reason, what) {
    const result = { areaId, ref: reference, ids: Object.freeze(ids.slice()), reason, what, confirmed: [], refused: null,
      guard: null, unconfirmed: null, unattempted: [] };
    state.batch = result;
    state.batchResult = null;
    state.busy = true;
    document.documentElement.classList.add('busy');
    clearAlert();
    renderAll();
    // firstNotSent: index of the first ID that was not sent at all (everything from there on is "not attempted").
    let firstNotSent = result.ids.length;
    for (let index = 0; index < result.ids.length; index++) {
      const treeId = result.ids[index];
      const outcome = await call('api/multi/pending/add', { fence: currentFence(), areaId, treeId, reason });
      if (outcome.kind === 'ok') {
        result.confirmed.push(treeId);
        const draft = state.drafts[areaId];
        if (draft) draft.ids = draft.ids.filter((id) => id !== treeId);
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
        firstNotSent = index; // the lock stopped it before sending
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
    const complete = result.confirmed.length === result.ids.length;
    if (complete) resetReasonPicker();
    if (result.unconfirmed) {
      lockCommands(label('MultiOutcomeUnconfirmed') || 'Outcome unconfirmed — re-read current state',
        'Adding ' + result.unconfirmed.treeId + ' to the Work Plan: no confirmed result was received. Do not assume that it ' +
        'happened, or that it did not.' + (result.unattempted.length ? ' Not attempted: ' + listIds(result.unattempted) + '.' : ''));
    } else {
      renderAll();
    }
    announce(batchSummary(result));
    if (!result.unconfirmed) {
      const add = byId('add-button');
      if (add.disabled || add.closest('[hidden]')) byId('batch-result').focus();
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
    return 'Stopped after ' + result.confirmed.length + ' of ' + n + ': ' + stopped + '. Added: ' + listIds(result.confirmed) +
      '. Not attempted: ' + listIds(result.unattempted) + '.';
  }

  function renderBatchResult() {
    const box = byId('batch-result');
    const result = state.batchResult;
    const show = result && result.areaId === state.areaId && result.ref === referenceIn(state.areaId);
    box.hidden = !show;
    if (!show) { box.replaceChildren(); return; }
    const n = result.ids.length;
    const complete = result.confirmed.length === n;
    box.setAttribute('class', 'batch-result' + (complete ? '' : result.confirmed.length > 0 ? ' partial' : ' failed'));
    const parts = [el('p', { class: 'alert-title' }, complete ? 'Added ' + plural(n, 'tree', 'trees') + ' to the Fell plan' :
      'Stopped after ' + result.confirmed.length + ' of ' + n)];
    parts.push(el('p', null, 'Added as pending (not felled): ', el('span', { class: 'mono' }, listIds(result.confirmed))));
    if (result.refused) {
      parts.push(el('p', null, 'Refused: ', el('span', { class: 'mono' }, result.refused.treeId), ' — ',
        el('code', null, result.refused.code), ' ' + result.refused.message + ' ' + recoveryFor(result.refused.code)));
    }
    if (result.guard) {
      parts.push(el('p', null, 'Refused by the local page server: ', el('span', { class: 'mono' }, result.guard.treeId), ' — ',
        el('code', null, result.guard.code), ' (HTTP ' + result.guard.status + ') ' + result.guard.message + ' It did not reach the engine.'));
    }
    if (result.unconfirmed) {
      parts.push(el('p', null, 'Unconfirmed: ', el('span', { class: 'mono' }, result.unconfirmed.treeId),
        ' — no confirmed result. Re-read before choosing again; the page will not resend it by itself.'));
    }
    if (result.unattempted.length > 0) {
      parts.push(el('p', null, 'Not attempted (still in your draft if eligible): ', el('span', { class: 'mono' }, listIds(result.unattempted))));
    }
    if (!complete) parts.push(el('p', { class: 'small-note' }, 'Orders already added stay pending; nothing was rolled back.'));
    box.replaceChildren(...parts);
  }

  // ---- the combined Work Plan, the receipt and the year results ----------------------------------------------------------

  function orderList(orders, className, removable) {
    return el('ul', { class: 'list orders ' + className }, ...orders.map((order) => el('li', null,
      el('span', { class: 'mono' }, order.treeId), el('span', { class: 'order-reason' }, order.reason),
      removable ? el('button', { type: 'button', class: 'small', 'data-remove': order.treeId, 'data-area': order.areaId,
        'data-focus-key': 'remove:' + order.treeId, 'aria-label': label('MultiRemoveFromPlan') + ': ' + order.treeId },
      label('MultiRemoveFromPlan')) : null)));
  }

  // Prune orders: icon and text name the state (never colour alone); the target height is the engine's scheduled lift.
  function pruneList(orders, approved, removable) {
    return el('ul', { class: 'list orders prune-orders' }, ...orders.map((order) => el('li', null,
      el('span', null, el('span', { class: 'order-kind k-prune' }, approved ? '✂ Approved Prune' : '✂ Pending Prune'),
        el('span', { class: 'mono' }, order.treeId)),
      el('span', { class: 'order-reason' }, 'one lift · crown base to ' + fixed(order.targetCrownBaseHeightM, 1) + ' m' +
        (approved ? ' · cut on Execute' : ' · not cut')),
      removable ? el('button', { type: 'button', class: 'small', 'data-prune-remove': order.treeId, 'data-area': order.areaId,
        'data-focus-key': 'prune-remove:' + order.treeId, 'aria-label': 'Remove the pending Prune of ' + order.treeId },
      label('MultiRemoveFromPlan')) : null)));
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
    const pendingCount = view.pending.length + pendingP.length;
    byId('plan-heading').textContent = plan ? 'Approved work plan' : 'Work plan';
    byId('step-pending').className = !plan && pendingCount > 0 ? 'now' : '';
    byId('step-approved').className = plan ? 'now' : '';
    byId('plan-scope').textContent = label('MultiOnePlan') + '. ' + label('MultiLastLivingRefused') + '.';
    byId('readout-unavailable').hidden = readoutState() !== 'unavailable';
    byId('pending-box').hidden = plan !== null && pendingCount === 0;
    byId('pending-total').textContent = view.pending.length === 0 && pendingP.length === 0 ? 'No pending Fell' :
      pendingP.length === 0 ? view.pending.length + ' pending Fell · not felled (' + perAreaCounts(view.pending) + ')' :
        workTotals(view.pending.length, pendingP.length) + ' pending · nothing felled or cut (' +
          state.view.areas.map((area) => area.label + ' ' + (ordersIn(view.pending, area.areaId).length + ordersIn(pendingP, area.areaId).length)).join(' · ') + ')';
    byId('pending-areas').replaceChildren(...view.areas.map((area) => {
      const orders = ordersIn(view.pending, area.areaId);
      const prunes = ordersIn(pendingP, area.areaId);
      return el('section', { class: 'plan-area', 'aria-label': 'Pending in ' + areaName(area.areaId) },
        el('h4', null, areaName(area.areaId) + ' · ' + plural(orders.length + prunes.length, 'pending order', 'pending orders')),
        orders.length === 0 && prunes.length === 0 ? el('p', { class: 'small-note' }, 'No pending Fell in this area.') : null,
        orders.length === 0 ? null : orderList(orders, 'pending-orders', true),
        prunes.length === 0 ? null : pruneList(prunes, false, true));
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
    byId('approved-total').textContent = approvedP.length === 0 ? plan.orders.length + ' approved · still standing' :
      workTotals(plan.orders.length, approvedP.length) + ' approved · nothing felled or cut yet';
    byId('approved-sub').textContent = 'Execution has not happened. Year remains ' + view.source.ecologicalYear + '. ' +
      state.view.areas.map((area) => area.label + ' ' + (ordersIn(plan.orders, area.areaId).length + ordersIn(approvedP, area.areaId).length)).join(' · ') +
      ' · approved in year ' + plan.source.ecologicalYear + '.';
    byId('approved-areas').replaceChildren(...view.areas.map((area) => {
      const orders = ordersIn(plan.orders, area.areaId);
      const prunes = ordersIn(approvedP, area.areaId);
      const n = orders.length + prunes.length;
      return el('section', { class: 'plan-area', 'aria-label': 'Approved in ' + areaName(area.areaId) },
        el('h4', null, areaName(area.areaId) + ' · ' + (n === 0 ? 'no orders' : plural(n, 'order', 'orders'))),
        orders.length === 0 ? null : orderList(orders, 'approved-orders', false),
        prunes.length === 0 ? null : pruneList(prunes, true, false));
    }), technicalPlan(plan.planId, plan.executionKey));
    byId('cancel-approved').textContent = label('MultiCancelApproved');
    byId('execute').textContent = readoutState() === 'unavailable' ? label('MultiExecute') + ' · re-read needed' :
      label('MultiExecute') + ' · ' + plural(plan.orders.length + approvedP.length, 'order', 'orders');
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
      parts.push(el('p', null, 'One receipt for the whole plan · ecological year ' + event.ecologicalYear + ' · work revision ' + revision + ' · ' +
        event.areas.reduce((sum, change) => sum + change.fells.length, 0) + ' trees felled'));
      parts.push(table('Receipt per area (actual before → actual after)', ['Area', 'Living trees', 'Basal area (m²/ha)', 'Felled (recorded reason)'],
        event.areas.map((change) => [areaShort(change.areaId), change.before.livingCount + ' → ' + change.after.livingCount,
          fixed(change.before.basalAreaM2PerHa, 2) + ' → ' + fixed(change.after.basalAreaM2PerHa, 2),
          change.fells.length === 0 ? 'none' : change.fells.map((fell) => fell.treeId + ' (' + fell.cause + '; ' + fell.reason + ')').join('; ')])));
      const pruned = state.stage4 ? state.stage4.pruneLog.filter((record) => record.executionKey === event.executionKey) : [];
      if (pruned.length > 0) {
        parts.push(table('Pruned in this Execute (engine record)', ['Tree', 'Area', 'Crown radius (m)', 'Crown base', 'Lifts'],
          pruned.map((record) => [record.treeId, areaShort(record.areaId), fixed(record.crownRadiusBeforeM, 3) + ' → ' + fixed(record.crownRadiusAfterM, 3),
            fixed(record.targetCrownBaseHeightM, 1) + ' m', record.liftsAfter + ' / 3'])));
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
      const carried = record.areas.some((year) => year.noLivingStems);
      latest.replaceChildren(
        el('p', null, 'Latest year ' + record.yearBefore + ' → ' + record.yearAfter + (record.cancelledUnexecutedWork ?
          ' · unexecuted work in both areas was cancelled at zero Fell' : '') + (carried
          ? '. Each populated area ran the annual rules; an area with no living stems was carried forward unchanged.'
          : readoutState() === 'stage4' ? '. Stage 4 rules: growth and crowns over the connected woodland; density mortality from each area\'s own stems.'
            : readoutState() === 'legacy' ? '. Each area ran the unchanged annual rules on its own trees.'
              : '. The model read-out of this view is unavailable; choose Re-read for the model statement.')),
        table('Year ' + record.yearBefore + ' → ' + record.yearAfter + ' per area', ['Area', 'Living trees', 'Basal area (m²/ha)', 'Natural deaths'],
          record.areas.map((year) => [areaShort(year.areaId), year.before.livingCount + ' → ' + year.after.livingCount,
            fixed(year.before.basalAreaM2PerHa, 2) + ' → ' + fixed(year.after.basalAreaM2PerHa, 2),
            year.noLivingStems ? label('MultiAreaNoLivingStems') : year.naturalDeaths.length === 0 ? label('MultiNoNaturalDeaths') :
              year.naturalDeaths.map((death) => death.treeId + ' (' + death.mechanism + ')').join('; ')])));
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
        el('td', null, statusText(item)),
        el('td', null, fixed(tree.dbhCm, 1)),
        el('td', null, fixed(tree.heightM, 1)),
        el('td', null, fixed(tree.crownRadiusM, 2)),
        el('td', null, fixed(tree.xm, 2)),
        el('td', null, fixed(tree.zm, 2))));
    }
    byId('tree-rows').replaceChildren(...rows);
    byId('tree-caption').textContent = 'Every tree of ' + areaName(area.areaId) + ', with its exact ID';
    byId('rows-shown').textContent = rows.length + ' rows shown for ' + area.label + (query !== '' ? ' (filtered)' : '');
  }

  function renderDetails() {
    const view = state.view;
    const e = view.experiment;
    const s = view.source;
    const readout = readoutState();
    // F-2: legacy (model 0) statements only for a paired legacy read-out; an unavailable one claims neither model.
    const statements = readout === 'stage4' ? STATEMENTS.map((name) => (name === 'MultiEdgeExcluded' ? 'MultiConnectedWoodland' : name))
      : readout === 'legacy' ? STATEMENTS : STATEMENTS.filter((name) => name !== 'MultiEdgeExcluded');
    const lead = readout === 'stage4' ? ['MultiDisclosureStage4'] : readout === 'legacy' ? ['MultiDisclosure'] : [];
    byId('statements').replaceChildren(...(readout === 'unavailable'
      ? [el('li', null, 'Model read-out unavailable for this view (the engine could not pair it with this view). Choose Re-read.')] : []),
    ...lead.concat(statements, ['MultiReviewStatus', 'MultiEmptyAreaRule'])
      .map((name) => label(name)).filter((text) => text !== '').map((text) => el('li', null, text)));
    const m = state.stage4 ? state.stage4.models : null;
    byId('stage4-statements').replaceChildren(...(m ? [
      'Models: CrownModelVersion ' + m.crownModelVersion + ' · AdultModelVersion ' + m.adultModelVersion + ' · MortalityModelVersion ' +
        m.mortalityModelVersion + ' · light ' + m.lightModelVersion + ' (' + fixed(m.lightCellM, 1) + ' m cells, shade buffer b = ' + fixed(m.shadeBufferM, 1) + ' m)',
      m.lightLabel + ': ' + m.notCalibratedCaveat,
      label('MultiDeadwoodNote'),
    ].concat(m.evidenceLabels, m.lightUncertainties) : []).map((text) => el('li', null, text)));
    byId('details-note').textContent = readout === 'stage4' ? label('MultiNotModelledStage4') : readout === 'legacy' ?
      'Light, seed, regeneration, site, money and saving: NOT MODELLED. Each area\'s ecology uses only its own trees.' :
      'Seed, regeneration, site, money and saving: NOT MODELLED. Which light and neighbourhood model this view used cannot be stated until a re-read.';
    byId('facts').replaceChildren(
      pair('Experiment', e.experimentId), pair('Format', e.format + ' v' + e.schemaVersion), pair('Domain', e.domain),
      pair('Source class', e.sourceClass), pair('Manifest content hash', expandable(e.manifestContentHash)),
      pair('Generator', e.generatorId + ' v' + e.generatorVersion + ', seed ' + e.generatorSeed),
      pair('Review status', e.reviewStatus), pair('Edge model', e.edgeModel, 'wide'), pair('Site', e.siteNote, 'wide'),
      ...view.areas.map((area) => pair(area.label + ' (' + area.areaId + ')', area.areaM2Exact + ' m² (' + fixed(area.areaHa, 4) + ' ha) · seed ' +
        area.simulationSeed + ' · ' + area.trees.length + ' tree records')),
      pair('Models', 'RNG v' + s.rngModelVersion + ' · growth model v' + s.growthModelVersion + ' · rules ' + s.p1AlgorithmVersion +
        ' · ' + s.areaModelVersion), pair('Ecological year', s.ecologicalYear), pair('Annual revision', s.annualRevision),
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

  // ---- selection --------------------------------------------------------------------------------------------------------

  function waitForBatch() {
    announce('Please wait: the selected trees are still being added to the Fell plan.');
  }

  function showArea(areaId) {
    if (!areaView(areaId) || areaId === state.areaId) return;
    if (state.batch) { waitForBatch(); return; }
    state.mapPick += 1; // a hit-test still in flight was for the other area
    state.areaId = areaId;
    state.zoom = ZOOMS[0];
    state.centre = null;
    state.overlap = null;
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

  // Selecting any living tree immediately reads its competitors (read-only) and makes it this area's reference; it never
  // marks Crop or proposes Fell. Choosing a different reference clears the old reference's unsent draft, visibly.
  function chooseTree(areaId, treeId, fromList) {
    if (!treeIn(areaId, treeId)) return;
    if (state.batch) { waitForBatch(); return; }
    state.mapPick += 1; // this selection is newer than any map click still waiting for its hit-test
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
    // UX-01: every new selection starts the inspector at the top, so the reference, ranks 1-5 and Add are in view even if
    // the previous inspection had been scrolled down.
    byId('pane-tree').scrollTop = 0;
    announce('Selected tree ' + treeId + ' in ' + areaName(areaId) + '.' + cleared);
    if (state.competitionTarget) refreshCompetition();
  }

  // Map click precedence (UX-08, the approved spec order, confirmed by the owner on 2026-11-23):
  //   modal dialog > rank toggle button > tree crown selection > ground inspection (only while Inspect ground is on).
  // An open dialog takes every click (it is modal); a rank button handles its own click and never reaches the map. Every
  // other map click goes to the server's read-only hit-test, with Inspect ground on or off, and a crown there is selected
  // exactly as usual. Only when Inspect ground is on and the hit-test reports no crown at that point does the click read
  // the local ground cell (changing no selection or draft). A batch, lock, busy, refused, guarded or failed hit-test never
  // falls through to a ground read.
  // Stale answers: each click takes a new mapPick number. If Escape, Close inspection, an area switch or another selection
  // happened while the hit-test was in flight, mapPick has moved on and the late answer is dropped whole: no view applied,
  // no tree selected, no draft cleared, no ground read, no alert and no announcement.
  async function onMapClick(event) {
    if (state.batch) { waitForBatch(); return; }
    if (!state.view || refusedByLock('Map selection') || state.busy) return;
    const area = currentArea();
    const matrix = byId('map').getScreenCTM();
    if (!area || !matrix) return;
    const point = new DOMPoint(event.clientX, event.clientY).matrixTransform(matrix.inverse());
    const pick = ++state.mapPick;
    state.busy = true;
    const outcome = await call('api/multi/hit', { fence: currentFence(), areaId: area.areaId, u: point.x, v: point.y });
    state.busy = false;
    if (pick !== state.mapPick) {
      // A selection made while this hit-test was in flight may have had its competitor read dropped (refreshCompetition
      // draws nothing while busy); read it again now, read-only, for that newer selection.
      if (state.competitionTarget && !state.competition && !state.lock && !state.batch) refreshCompetition();
      return;
    }
    if (outcome.kind === 'ok') {
      clearAlert();
      const candidates = outcome.payload.candidates;
      if (candidates.length === 0) {
        // An empty click keeps the reference and its draft: only a deliberate choice of another tree replaces them.
        state.overlap = null;
        applyPayload(outcome.payload, false);
        // Re-checked after the await: Escape or Stop may have ended Inspect ground while the hit-test was in flight.
        if (state.lightOn && state.groundMode) { inspectPoint(area.areaId, point.x, point.y); return; }
        announce('No crown at this point — the selection is unchanged.');
        return;
      }
      if (state.lightOn && state.groundMode) state.ground = null; // a crown hit is a selection, not a ground read
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

  // Close inspection (UX-03): clears this area's selection, its reference's unsent draft choices and the enlarged focus
  // drawing, so the map returns to normal. Zoom, pan, area, Light Guide, Crop designations, queued or approved orders and
  // the year are untouched; nothing is sent. (Removing a Crop designation is a separate, explicit command.)
  function closeInspection() {
    state.mapPick += 1; // a map click still waiting for its hit-test must not reopen what is being closed
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
    announce('Closed the inspection of ' + id + '.' + (drafts ? ' Cleared ' + plural(drafts, 'unsent draft choice', 'unsent draft choices') + '.' : '') +
      ' Zoom, area, Crop designations and the Work Plan are unchanged.');
    return true;
  }

  // Escape precedence (UX-03/UX-08): an open dialog closes itself first (the browser's own Escape on a modal dialog), and
  // this handler then does nothing in that same key press; a search box with text clears itself first; next, Inspect
  // ground stops; next, the tree inspection closes. The Light Guide is never switched off by Escape.
  function onEscape(event) {
    if (event.key !== 'Escape') return;
    // Any Escape abandons a map click still waiting for its hit-test (it stays immediately responsive: nothing waits).
    state.mapPick += 1;
    if (event.defaultPrevented) return;
    if (document.querySelector('dialog[open]')) return;
    const target = event.target;
    if (target && target.localName === 'input' && target.getAttribute('type') === 'search' && target.value !== '') return;
    if (state.lightOn && state.groundMode) {
      event.preventDefault();
      setGroundMode(false, false);
      return;
    }
    if (closeInspection()) event.preventDefault();
  }

  // Enter in an exact-ID box selects only an exact, case-sensitive match, in whichever area holds it.
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

  // ---- commands -----------------------------------------------------------------------------------------------------------

  function onCrop() {
    const areaId = state.areaId;
    const treeId = selectedId();
    if (!treeId || !state.view || state.batch) return;
    const marked = areaView(areaId).cropTreeIds.includes(treeId);
    run(marked ? 'api/multi/crop/unmark' : 'api/multi/crop/mark', { fence: currentFence(), areaId, treeId },
      label(marked ? 'MultiUnmarkCrop' : 'MultiMarkCrop') + ' (' + treeId + ')',
      () => announce((marked ? 'Removed Crop designation: ' : 'Retained Crop Tree: ') + treeId + ' in ' + areaName(areaId) + '.'));
  }

  // Add the ticked competitors, in the current ranked order, as exact IDs of the shown area.
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

  // Add the reference tree itself (a living, non-Crop tree), through the same fenced path.
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
      () => announce('Done: ' + label('MultiRemoveFromPlan') + ' (' + treeId + '). Pending order removed; no draft changed.'), 'plan-heading');
  }

  // Queue one Prune for the inspected living tree. The engine chooses the lift height and checks every guard again.
  function onPruneAdd() {
    const areaId = state.areaId;
    const treeId = selectedId();
    const s4 = s4Tree(areaId, treeId);
    if (!state.view || state.busy || state.batch || !treeId || !s4) return;
    if (refusedByLock('Add ' + treeId + ' to the Prune plan')) return;
    if (!pruneAvailability(areaId, itemOf(areaId, treeId), s4).ok) return;
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
        '. Nothing was felled or cut. Living trees unchanged: ' + payload.view.areas.map((a) => a.label + ' ' + a.stand.livingCount).join(', ') + '.');
    }, 'plan-heading');
  }

  function onCancelApproved() {
    run('api/multi/plan/cancel', { fence: currentFence() }, label('MultiCancelApproved'),
      () => announce('Done: ' + label('MultiCancelApproved') + '. Its orders were discarded, not returned to pending.'), 'plan-heading');
  }

  function openDialog(dialog, initialFocus) {
    state.opener = document.activeElement;
    dialog.showModal();
    initialFocus.focus();
  }

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
      // F-2: never offer a confirmation whose Prune orders are unknown (it would look empty or Fell-only).
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
        orders.length === 0 ? null : orderList(orders, 'approved-orders', false),
        prunes.length === 0 ? null : pruneList(prunes, true, false));
    }));
    byId('execute-plan').replaceChildren(label('MultiLastLivingRefused') + '. The ecological year does not change. ',
      technicalPlan(confirmation.planId, confirmation.executionKey));
    openDialog(byId('execute-dialog'), byId('execute-back'));
  }

  function onExecuteConfirmed() {
    const confirmation = state.confirmation;
    if (state.busy && confirmation) {
      announce('Still waiting for the local session process. Nothing was sent.');
      return;
    }
    state.confirmation = null;
    byId('execute-dialog').close();
    if (!confirmation || refusedByLock(label('MultiExecute'))) return;
    if (!confirmationStillShown(confirmation, state.view)) {
      withdrawConfirmation(confirmation);
      return;
    }
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
      if (execution.replayed) {
        announce(label('MultiAlreadyExecuted') + '.');
        return;
      }
      const pruned = payload.stage4 ? payload.stage4.pruneLog.filter((record) => record.executionKey === execution.executionKey) : [];
      announce('Done: ' + label('MultiExecute') + '. ' + execution.event.areas.map((change) => areaName(change.areaId) + ' living ' +
        change.before.livingCount + ' → ' + change.after.livingCount + ' (felled ' + (change.fells.map((f) => f.treeId).join(', ') || 'none') + ')').join('; ') + '.' +
        (pruned.length ? ' Pruned: ' + pruned.map((r) => r.treeId + ' crown ' + fixed(r.crownRadiusBeforeM, 2) + ' → ' + fixed(r.crownRadiusAfterM, 2) + ' m').join('; ') +
          '. The year did not change.' : ''));
    }, 'receipt-heading');
    showPane('plan');
  }

  function sendAdvance(consent) {
    run('api/multi/year/advance', { fence: currentFence(), consentToCancelWork: consent }, label('MultiAdvanceYear'), (payload) => {
      const drafts = draftTotal();
      state.drafts = {};
      state.draftNotice = null;
      state.batchResult = null;
      renderAll();
      const record = payload.view.annualHistory[payload.view.annualHistory.length - 1];
      announce('Done: year ' + record.yearBefore + ' → ' + record.yearAfter + '. ' + record.areas.map((year) => areaName(year.areaId) + ' living ' +
        year.before.livingCount + ' → ' + year.after.livingCount + (year.noLivingStems ? ' (no living stems)' : ', natural deaths ' + year.naturalDeaths.length))
        .join('; ') + '.' + (record.cancelledUnexecutedWork ? ' Unexecuted work was cancelled at zero Fell.' : '') +
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

  // Unexecuted work needs the server's explicit consent to cancel it; unsent local drafts get their own, separate warning.
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
        // While locked, no diagnostic was read; read it now for the tree still being inspected.
        if (state.competitionTarget && !state.competition) refreshCompetition();
      }
      if (announceIt && !state.lock) {
        announce('Re-read: ecological year ' + state.view.source.ecologicalYear + ', work revision ' + state.view.source.workRevision +
          ', plan-state revision ' + state.view.planStateRevision + '.' + (state.draftNotice ? ' ' + state.draftNotice.text : ''));
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

  // ---- start ----------------------------------------------------------------------------------------------------------

  function wire() {
    byId('area-switch').addEventListener('click', (event) => {
      const button = event.target.closest('[data-area]');
      if (button) showArea(button.getAttribute('data-area'));
    });
    byId('map').addEventListener('click', onMapClick);
    byId('crop-button').addEventListener('click', onCrop);
    byId('reread').addEventListener('click', () => reread(true));
    byId('advance').addEventListener('click', onAdvance);
    byId('view-plan').addEventListener('click', () => { showPane('plan'); byId('plan-heading').focus(); });
    byId('notice-provenance').addEventListener('click', () => { showPane('details'); byId('details-heading').focus(); });
    byId('rank-layer').addEventListener('click', onRankToggle);
    byId('inspect-close').addEventListener('click', closeInspection);
    byId('ground-mode').addEventListener('click', () => setGroundMode(!state.groundMode, true));
    byId('readout-reread').addEventListener('click', () => reread(true));
    // On the root element (every key event bubbles to it), after the dialogs' own handling.
    document.documentElement.addEventListener('keydown', onEscape);
    byId('legend-hide').addEventListener('click', () => setLegendHidden(true, true));
    byId('legend-show').addEventListener('click', () => setLegendHidden(false, true));
    byId('competition-content').addEventListener('change', (event) => {
      const box = event.target.closest('[data-draft]');
      if (box) toggleDraft(box.getAttribute('data-draft'), box.checked);
    });
    byId('draft-clear').addEventListener('click', clearDraft);
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
      const button = event.target.closest('[data-remove]');
      if (button) removePending(button.getAttribute('data-area'), button.getAttribute('data-remove'));
    });
    byId('pending-areas').addEventListener('click', (event) => {
      const button = event.target.closest('[data-prune-remove]');
      if (button) removePrune(button.getAttribute('data-area'), button.getAttribute('data-prune-remove'));
    });
    byId('prune-add').addEventListener('click', onPruneAdd);
    byId('prune-remove').addEventListener('click', () => removePrune(state.areaId, selectedId()));
    byId('light-toggle').addEventListener('click', () => setLightGuide(!state.lightOn));
    byId('ground-close').addEventListener('click', () => setGroundMode(false, true));
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
    // The map frame follows the pane's size (window resize, legend shown or hidden, narrow layout).
    let frame = 0;
    const reframe = () => {
      if (frame) return;
      frame = window.requestAnimationFrame(() => { frame = 0; if (state.view && state.labels) { renderMap(); renderMapControls(); } });
    };
    if (typeof ResizeObserver === 'function') new ResizeObserver(reframe).observe(byId('map-pane'));
    else window.addEventListener('resize', reframe);
  }

  drawCanopyDefs();
  renderLegend();
  wire();
  reread(false);
})();
""";
}
