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
    }

    private Task ReadMultiViewAsync(HttpContext context)
    {
        var view = multiArea.Read();
        return WriteJsonAsync(context, StatusCodes.Status200OK, new MultiViewPayload(view, ScenesOf(view), MultiLabels));
    }

    // One fenced read and one pure calculation on that same immutable view; never a mutation, never a Crop requirement.
    private async Task ReadMultiCompetitionAsync(HttpContext context)
    {
        var request = await ReadMultiRequestAsync<MultiTreeRequest>(context, MultiTreeMembers);
        if (request is null) return;
        var result = multiArea.Competition(request.Fence!, request.AreaId!, request.TreeId!);
        await WriteJsonAsync(context, StatusOf(result.RefusalCode), new MultiCompetitionPayload(result.View, ScenesOf(result.View),
            result.AreaId, result.Competition, NoticeOf(result.RefusalCode, result.RefusalMessage)));
    }

    private async Task MultiHitTestAsync(HttpContext context)
    {
        var request = await ReadMultiRequestAsync<MultiHitRequest>(context, MultiHitMembers);
        if (request is null) return;
        var result = multiArea.HitTest(request.Fence!, request.AreaId!, request.U!.Value, request.V!.Value);
        await WriteJsonAsync(context, StatusOf(result.RefusalCode), new MultiHitPayload(result.View, ScenesOf(result.View),
            result.AreaId, result.Candidates, NoticeOf(result.RefusalCode, result.RefusalMessage)));
    }

    private static async Task MultiCommandAsync<TRequest>(HttpContext context, IReadOnlyList<string> required,
        Func<TRequest, MultiAreaCommandResult> command) where TRequest : class
    {
        var request = await ReadMultiRequestAsync<TRequest>(context, required);
        if (request is null) return; // a guard has already answered
        var result = command(request);
        await WriteJsonAsync(context, StatusOf(result.RefusalCode), new MultiCommandPayload(result.View, ScenesOf(result.View),
            NoticeOf(result.RefusalCode, result.RefusalMessage), result.Execution, result.Annual));
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

    private sealed record MultiViewPayload(MultiAreaSessionView View, IReadOnlyDictionary<string, IReadOnlyList<PlayerMapItem>> Scenes,
        IReadOnlyDictionary<string, string> Labels);

    private sealed record MultiCommandPayload(MultiAreaSessionView View, IReadOnlyDictionary<string, IReadOnlyList<PlayerMapItem>> Scenes,
        Notice? Refusal, MultiAreaExecutionResult? Execution, MultiAreaAnnualResult? Annual);

    private sealed record MultiCompetitionPayload(MultiAreaSessionView View,
        IReadOnlyDictionary<string, IReadOnlyList<PlayerMapItem>> Scenes, string AreaId, PlayerCompetitionReport? Competition,
        Notice? Refusal);

    private sealed record MultiHitPayload(MultiAreaSessionView View, IReadOnlyDictionary<string, IReadOnlyList<PlayerMapItem>> Scenes,
        string AreaId, IReadOnlyList<PlayerMapItem> Candidates, Notice? Refusal);
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
<!-- MA-1 two-area manual forestry page (Multi #885). Every value is filled in by multi.js from the server view. Below
     1024 CSS px everything is one stacked column; at 1024 CSS px and wider the map and the side panel share one screen. -->
<a class="skip-link" id="skip-trees" href="#trees-heading">Skip to the tree table</a>

<header class="status-bar" aria-labelledby="page-title">
  <div class="brand">
    <svg class="brand-mark" viewBox="0 0 24 24" aria-hidden="true" focusable="false">
      <path d="M12 2 L17 9 H14.5 L19 15 H15.5 L20 21 H4 L8.5 15 H5 L9.5 9 H7 Z"></path>
    </svg>
    <div class="brand-text">
      <h1 id="page-title">Two-area Stand Map</h1>
      <p class="subtitle">MA-1 · two synthetic Sitka areas · one Work Plan · north up</p>
    </div>
  </div>
  <dl id="rail" class="rail"></dl>
  <p class="rail-actions">
    <button type="button" id="advance" class="primary">Advance one ecological year (both areas)</button>
    <button type="button" id="reread" aria-label="Re-read current state" title="Re-read current state">Re-read</button>
  </p>
  <div class="disclosure-strip">
    <p id="disclosure" class="disclosure"></p>
    <ul id="flags" class="flags"></ul>
  </div>
</header>

<div class="messages">
  <div id="alert" class="alert" role="alert" tabindex="-1"></div>
</div>

<main class="layout">
  <section class="panel map-panel" aria-labelledby="map-heading">
    <div class="map-head">
      <h2 id="map-heading" tabindex="-1">Area map</h2>
      <div id="area-switch" class="area-switch" role="group" aria-label="Choose the area shown on the map and in the tree table"></div>
      <p id="announce" class="announce" role="status" aria-live="polite" aria-atomic="true"></p>
      <details class="legend-box">
        <summary>Legend</summary>
        <div id="legend" class="legend"></div>
      </details>
    </div>
    <div class="map-viewport">
      <div id="map-pane" class="map-pane">
        <svg id="map" viewBox="0 0 40 40" role="img" aria-labelledby="map-title map-desc">
          <title id="map-title">Map of the selected management area, north up</title>
          <desc id="map-desc">Loading the current view.</desc>
          <defs>
            <pattern id="hatch" patternUnits="userSpaceOnUse" width="0.5" height="0.5" patternTransform="rotate(45)">
              <rect class="hatch-ground" width="0.5" height="0.5"></rect>
              <line class="hatch-line" x1="0.12" y1="0" x2="0.12" y2="0.5"></line>
            </pattern>
            <radialGradient id="crown-fill" cx="0.42" cy="0.38" r="0.7">
              <stop class="crown-stop-hi" offset="0"></stop>
              <stop class="crown-stop-lo" offset="1"></stop>
            </radialGradient>
          </defs>
          <rect id="ground" class="ground" x="0" y="0" width="40" height="40"></rect>
          <g id="grid" class="overlay"></g>
          <polygon id="area-outline" class="area-outline" points="0,0"></polygon>
          <g id="crowns"></g>
          <g id="marks" class="overlay"></g>
          <g id="selection" class="overlay"></g>
          <g id="furniture" class="overlay"></g>
        </svg>
      </div>
      <div class="map-controls" role="group" aria-label="Zoom">
        <button type="button" id="zoom-in" class="icon" aria-label="Zoom in" title="Zoom in"><span aria-hidden="true">+</span></button>
        <button type="button" id="zoom-out" class="icon" aria-label="Zoom out" title="Zoom out"><span aria-hidden="true">−</span></button>
        <button type="button" id="zoom-fit" class="icon fit" aria-label="Fit whole area" title="Fit whole area">Fit</button>
      </div>
      <div class="map-dpad" role="group" aria-label="Pan the map">
        <span class="d-c" aria-hidden="true">Pan</span>
        <button type="button" id="pan-north" class="icon" aria-label="Pan north" title="Pan north"><span aria-hidden="true">▲ N</span></button>
        <button type="button" id="pan-west" class="icon" aria-label="Pan west" title="Pan west"><span aria-hidden="true">◀ W</span></button>
        <button type="button" id="pan-east" class="icon" aria-label="Pan east" title="Pan east"><span aria-hidden="true">E ▶</span></button>
        <button type="button" id="pan-south" class="icon" aria-label="Pan south" title="Pan south"><span aria-hidden="true">S ▼</span></button>
      </div>
      <figure class="overview" aria-labelledby="overview-caption">
        <svg id="overview" viewBox="0 0 10 10" aria-hidden="true" focusable="false"></svg>
        <figcaption id="overview-caption" class="sr-only">Both areas in the experiment frame</figcaption>
      </figure>
    </div>
    <p id="map-note" class="hint map-note"></p>
  </section>

  <div id="side" class="side" data-pane="tree">
    <nav class="view-tabs" aria-label="Side panel views">
      <button type="button" class="view-tab" data-pane-button="tree" aria-controls="pane-tree" aria-pressed="true">Tree</button>
      <button type="button" class="view-tab" data-pane-button="plan" aria-controls="pane-plan" aria-pressed="false">Work Plan</button>
      <button type="button" class="view-tab" data-pane-button="trees" aria-controls="pane-trees" aria-pressed="false">Trees</button>
      <button type="button" class="view-tab" data-pane-button="history" aria-controls="pane-history" aria-pressed="false">Results</button>
      <button type="button" class="view-tab" data-pane-button="details" aria-controls="pane-details" aria-pressed="false">Details</button>
    </nav>

    <div id="pane-tree" class="pane">
      <section class="panel" aria-labelledby="inspector-heading">
        <h2 id="inspector-heading" tabindex="-1">Selected tree</h2>
        <div id="inspector-empty">
          <p>No tree selected in this area. Click a crown on the map, use the tree table, or type an exact tree ID.</p>
        </div>
        <div id="inspector-body" hidden>
          <p class="selected-area" id="insp-area"></p>
          <p class="selected-id">Tree <strong id="insp-id" class="mono"></strong> <span id="insp-chip" class="chip"></span></p>
          <p id="insp-summary" class="small-note insp-summary"></p>
          <form id="add-form" class="add-form" novalidate hidden>
            <p><button type="submit" id="add-button" class="primary"></button></p>
            <p id="release-wrap" class="field release-option" hidden>
              <label><input type="checkbox" id="reason-release"><span id="release-label"></span></label>
            </p>
            <details id="reason-other-wrap" class="optional-note">
              <summary>Add a note (optional)</summary>
              <label for="reason">Optional note recorded with this Fell order</label>
              <textarea id="reason" rows="2" maxlength="2000"></textarea>
              <p id="reason-help" class="note small-note">Note takes precedence; otherwise the ticked release intent, otherwise Owner-selected felling.</p>
            </details>
            <p id="reason-error" class="field-error" role="alert" hidden></p>
            <p id="reason-preview" class="note small-note" aria-live="polite"></p>
          </form>
          <section id="competition-panel" aria-labelledby="competition-heading">
            <h3 id="competition-heading">Competition (read-only)</h3>
            <div id="competition-content" aria-live="polite"></div>
          </section>
          <p class="actions"><button type="button" id="crop-button"></button></p>
          <p id="crop-note" class="note small-note"></p>
          <p id="insp-remove-wrap" hidden><button type="button" id="insp-remove"></button></p>
          <p id="insp-approved-note" class="note" hidden>This tree is in the approved plan. Cancel the approved plan to change it.</p>
          <details class="tree-record"><summary>Tree record</summary>
            <dl id="insp-facts" class="pairs insp-facts"></dl>
          </details>
          <p id="insp-threshold" class="note small-note"></p>
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
        <h2 id="plan-heading" tabindex="-1">Work Plan — both areas</h2>
        <p id="plan-scope" class="note small-note"></p>
        <h3>Pending Fell <span class="step-note">(not felled)</span></h3>
        <p id="pending-total" class="plan-total"></p>
        <p class="plan-actions"><button type="button" id="approve" hidden></button></p>
        <div id="pending-areas"></div>
        <h3>Approved plan <span class="step-note">(still standing until executed)</span></h3>
        <p id="approved-empty" class="note">No approved plan.</p>
        <div id="approved-box" hidden>
          <p id="approved-total" class="plan-total"></p>
          <p class="actions plan-actions">
            <button type="button" id="cancel-approved"></button>
            <button type="button" id="execute" class="danger"></button>
          </p>
          <div id="approved-areas"></div>
        </div>
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
        <p id="rows-shown" class="hint"></p>
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
        <h2 id="details-heading">Experiment and technical provenance</h2>
        <ul id="statements" class="list statements"></ul>
        <dl id="facts" class="facts"></dl>
        <p class="note">Light, seed, regeneration, site, money and saving: NOT MODELLED. Each area's ecology uses only its own trees.</p>
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
    <button type="button" id="execute-back">Back</button>
    <button type="button" id="execute-confirm" class="danger">Confirm felling</button>
  </p>
</dialog>

<dialog id="consent-dialog" aria-labelledby="consent-title" aria-describedby="consent-text">
  <h2 id="consent-title"></h2>
  <p id="consent-text"></p>
  <div id="consent-areas"></div>
  <p class="actions">
    <button type="button" id="consent-keep"></button>
    <button type="button" id="consent-advance" class="danger"></button>
  </p>
</dialog>

<noscript><p class="noscript">This page needs JavaScript to talk to the local session process.</p></noscript>
</body>
</html>
""";

    internal const string Css = """
/* MA-1 two-area Stand Map (Multi #885). Same charcoal/slate tokens as the single-area page. Colour is never the only cue:
   every tree state also has a pattern, a ring, a letter badge or text. Below 1024 CSS px the page is one stacked column;
   at 1024 CSS px and wider the map and the side panel share one screen and long content scrolls inside its pane. */
:root {
  color-scheme: dark;
  --bg: #0f171b; --panel: #151c1e; --panel-raised: #1b2326; --panel-edge: #465157; --line: #526168;
  --text: #ebede9; --muted: #aeb9bd; --accent: #6ea39d; --focus: #ffd166;
  --button: #1e272a; --button-line: #6d7d84; --primary: #38594b; --primary-line: #97bca5;
  --danger: #71352f; --danger-line: #dd9187; --alert-bg: #3b1c15; --alert-line: #dd9187;
  --announce-bg: #1b2c30; --announce-line: #6ea39d;
  --floor: #3c3a2d; --grid: #6d7168; --boundary: #e2ece5;
  --crown-hi: #4c8667; --crown-lo: #173a2d; --crown-line: #6f8f7c;
  --pending: #ffd166; --approved: #ff9b7a; --dead-line: #b0b8b2; --hatch-ground: #2e3531;
  --stump: #c9a46c; --stump-mark: #24170a; --crop: #4682e6; --select: #ffffff; --current-row: #2c3b42;
}
*, *::before, *::after { box-sizing: border-box; }
html { -webkit-text-size-adjust: 100%; text-size-adjust: 100%; }
html.busy, html.busy * { cursor: progress; }
body { margin: 0; background: var(--bg); color: var(--text); font: 1rem/1.45 system-ui, -apple-system, "Segoe UI", Roboto, Ubuntu, Arial, sans-serif; }
[hidden] { display: none !important; }
h1 { margin: 0; font-size: 1.12rem; line-height: 1.2; }
h2 { margin: 0 0 .5rem; font-size: 1.08rem; }
h3 { margin: .8rem 0 .35rem; font-size: 1rem; }
h4 { margin: .6rem 0 .3rem; font-size: .95rem; }
p { margin: .4rem 0; }
:focus { outline: none; }
:focus-visible { outline: 3px solid var(--focus); outline-offset: 2px; }
.mono, code { font-family: ui-monospace, "Cascadia Mono", Consolas, "DejaVu Sans Mono", monospace; overflow-wrap: anywhere; }
.sr-only { position: absolute; width: 1px; height: 1px; margin: -1px; padding: 0; overflow: hidden; clip: rect(0 0 0 0); white-space: nowrap; border: 0; }
.skip-link { position: absolute; left: .5rem; top: -5rem; z-index: 20; padding: .7rem 1rem; border-radius: 6px; background: var(--focus); color: #000; font-weight: 700; }
.skip-link:focus { top: .5rem; }
button, input, select, textarea, summary { font: inherit; color: inherit; }
button { min-width: 44px; min-height: 44px; padding: .45rem .9rem; border: 2px solid var(--button-line); border-radius: 8px; background: var(--button); color: #fff; font-weight: 600; cursor: pointer; }
button:hover { filter: brightness(1.15); }
button.small { padding: .3rem .65rem; }
button.primary { border-color: var(--primary-line); background: var(--primary); }
button.danger { border-color: var(--danger-line); background: var(--danger); }
button:disabled, button:disabled:hover { border-style: dashed; background: var(--panel-raised); color: var(--muted); filter: none; cursor: not-allowed; }
input, select, textarea { width: 100%; min-height: 44px; padding: .45rem .6rem; border: 2px solid var(--line); border-radius: 8px; background: var(--panel-raised); color: var(--text); }
label { display: block; margin-bottom: .25rem; color: var(--muted); }
.field { margin: .4rem 0; }
.status-bar { display: flex; flex-wrap: wrap; align-items: center; gap: .25rem .8rem; padding: .3rem .8rem; border-bottom: 1px solid var(--panel-edge); background: linear-gradient(180deg, #1b2326, var(--panel)); }
.brand { display: flex; align-items: center; gap: .5rem; min-width: 0; }
.brand-mark { flex: none; width: 1.6rem; height: 1.6rem; fill: var(--accent); }
.subtitle { margin: 0; color: var(--muted); font-size: .76rem; }
.rail { display: flex; flex-wrap: wrap; gap: .3rem .4rem; margin: 0; }
.rail > div { min-width: 0; padding: 0 .5rem; border: 1px solid var(--panel-edge); border-radius: 8px; background: rgba(10, 18, 15, .55); }
.rail dt { font-size: .62rem; letter-spacing: .03em; text-transform: uppercase; }
.rail dd { font-size: .86rem; font-weight: 700; }
.rail-actions { display: flex; flex-wrap: wrap; gap: .4rem; margin: 0 0 0 auto; }
.rail-actions button { padding: .3rem .75rem; }
.disclosure-strip { display: flex; flex-wrap: wrap; align-items: center; gap: .2rem .6rem; flex-basis: 100%; min-width: 0; }
.disclosure { margin: 0; font-size: .68rem; font-weight: 700; color: var(--focus); overflow-wrap: anywhere; }
.flags { display: flex; flex-wrap: wrap; gap: .3rem; margin: 0; padding: 0; list-style: none; }
.flags li { padding: 0 .5rem; border: 1px solid var(--line); border-radius: 999px; background: var(--panel-raised); font-size: .7rem; }
dt { color: var(--muted); font-size: .85rem; }
dd { margin: 0; overflow-wrap: anywhere; }
.pairs { display: grid; gap: .3rem; margin: .4rem 0; }
.facts { display: grid; grid-template-columns: repeat(auto-fill, minmax(min(100%, 10rem), 1fr)); gap: .45rem 1rem; margin: .6rem 0; }
details.expand summary { display: inline-flex; align-items: center; min-height: 44px; cursor: pointer; }
details.expand .full { display: block; padding: .2rem 0 .4rem; }
.messages { padding: 0 1rem; }
.alert.shown { margin-top: .5rem; padding: .6rem 1rem; border: 2px solid var(--alert-line); border-radius: 8px; background: var(--alert-bg); }
.alert-title { font-weight: 700; }
/* The polite status line sits in the map header row, so announcements never push the map down. */
.announce { flex: 1 1 12rem; min-width: 0; margin: 0; overflow: hidden; color: var(--muted); font-size: .82rem; white-space: nowrap; text-overflow: ellipsis; }
.announce:not(:empty) { padding: .15rem .6rem; border: 1px solid var(--announce-line); border-radius: 8px; background: var(--announce-bg); color: var(--text); }
.noscript { margin: 1rem; padding: 1rem; border: 2px solid var(--alert-line); background: var(--alert-bg); }
.layout { display: grid; grid-template-columns: minmax(0, 1fr); gap: .75rem; padding: .75rem; }
.side { display: grid; align-content: start; gap: .75rem; min-width: 0; }
.pane { display: grid; align-content: start; gap: .75rem; min-width: 0; }
.panel { min-width: 0; padding: .8rem; border: 1px solid var(--panel-edge); border-radius: 10px; background: linear-gradient(180deg, var(--panel-raised), var(--panel) 5rem); }
.hint, .note { color: var(--muted); }
.small-note { font-size: .86rem; }
.step-note { color: var(--muted); font-weight: 400; font-size: .85rem; }
.warning { padding: .45rem .7rem; border: 2px solid var(--alert-line); border-radius: 6px; background: var(--alert-bg); font-weight: 600; }
.actions { display: flex; flex-wrap: wrap; gap: .6rem; }
.list { display: grid; gap: .4rem; margin: .3rem 0; padding: 0; list-style: none; }
.list li { overflow-wrap: anywhere; }
.orders li { padding: .35rem .6rem; border-left: 3px solid var(--line); border-radius: 4px; background: rgba(0, 0, 0, .18); }
.pending-orders li { border-left-color: var(--pending); }
.approved-orders li { border-left-color: var(--approved); }
.order-reason { display: block; color: var(--muted); }
.plan-area { margin: .3rem 0 .6rem; padding: .3rem .6rem; border: 1px solid var(--line); border-radius: 8px; }
.plan-total { font-weight: 700; }
.selected-area { margin: 0; color: var(--accent); font-weight: 700; }
.selected-id { margin: 0; font-size: 1.1rem; }
.insp-summary { margin: .1rem 0 .3rem; }
.chip { display: inline-block; margin-left: .35rem; padding: .05rem .55rem; border: 1px solid var(--line); border-radius: 999px; background: var(--panel-raised); font-size: .85rem; font-weight: 600; }
.chip.s-PendingFell { border-color: var(--pending); color: var(--pending); }
.chip.s-ApprovedFell { border-color: var(--approved); color: var(--approved); }
.chip.s-Felled { border-color: var(--stump); color: var(--stump); }
.insp-facts { display: flex; flex-wrap: wrap; gap: .3rem .9rem; }
.insp-facts > div { flex: 1 1 6.5rem; }
.insp-facts > div.wide { flex-basis: 100%; }
.add-form { margin-top: .4rem; padding-top: .3rem; border-top: 1px solid var(--line); }
.field-error { margin: .25rem 0; padding: .3rem .6rem; border-left: 4px solid var(--alert-line); background: var(--alert-bg); font-weight: 600; }
[aria-invalid="true"] { border-color: var(--alert-line); }
.competition-totals { padding: .35rem .5rem; border-left: 3px solid var(--accent); }
.competitors { list-style: none; padding: 0; display: grid; gap: .3rem; }
.competitors li { padding: .2rem .4rem; border: 1px solid var(--line); border-radius: 5px; }
.competitors li.focused { border: 2px solid var(--select); }
.competitors span { display: block; color: var(--muted); font-size: .84rem; }
.competitors button { width: 100%; text-align: left; }
.view-tabs { display: none; flex-wrap: wrap; gap: .3rem; }
.view-tab { flex: 1 1 auto; padding: .35rem .5rem; border-color: var(--line); background: var(--panel); }
.view-tab[aria-pressed="true"] { border-color: var(--accent); background: var(--primary); }
.map-head { display: flex; flex-wrap: wrap; align-items: center; gap: .4rem 1rem; margin-bottom: .4rem; }
.map-head h2 { margin: 0; }
.area-switch { display: flex; flex-wrap: wrap; gap: .4rem; }
.area-tab { display: flex; flex-direction: column; align-items: flex-start; padding: .2rem .8rem; border-color: var(--line); background: var(--panel); }
.area-tab[aria-pressed="true"] { border-color: var(--accent); background: var(--primary); }
.area-meta { font-size: .76rem; font-weight: 400; color: var(--muted); }
.area-tab[aria-pressed="true"] .area-meta { color: var(--text); }
.map-viewport { position: relative; min-width: 0; }
.map-pane { overflow: hidden; border: 1px solid var(--panel-edge); border-radius: 8px; background: radial-gradient(ellipse at center, #242b2d, #1d1f1a); }
#map { display: block; width: 100%; height: auto; aspect-ratio: 1 / 1; max-height: 80vh; touch-action: manipulation; }
.map-controls { position: absolute; top: .5rem; right: .5rem; z-index: 3; display: flex; flex-direction: column; gap: 3px; padding: 4px; border: 1px solid var(--line); border-radius: 8px; background: var(--panel); box-shadow: 0 2px 10px #0008; }
/* The pad itself is transparent: only its four buttons are drawn, so its empty corners never hide the map. */
.map-dpad { position: absolute; right: .5rem; bottom: .5rem; z-index: 3; display: grid; grid-template-columns: repeat(3, 44px); grid-template-rows: repeat(3, 44px); gap: 2px; pointer-events: none; }
.map-dpad button { pointer-events: auto; background: var(--panel); box-shadow: 0 2px 8px #0009; }
#pan-north { grid-column: 2; grid-row: 1; }
#pan-west { grid-column: 1; grid-row: 2; }
#pan-east { grid-column: 3; grid-row: 2; }
#pan-south { grid-column: 2; grid-row: 3; }
button.icon { display: grid; place-items: center; width: 44px; height: 44px; padding: 0; font-size: .9rem; line-height: 1; }
.overview { position: absolute; left: .5rem; bottom: .5rem; z-index: 2; width: 6.5rem; margin: 0; padding: 3px; border: 1px solid var(--line); border-radius: 6px; background: rgba(15, 23, 27, .88); pointer-events: none; }
.overview svg { display: block; width: 100%; height: auto; }
.ov-area { fill: #2b3a30; stroke: var(--muted); stroke-width: .6; }
.ov-area.current { fill: var(--primary); stroke: var(--focus); stroke-width: 1; }
.ov-label { fill: var(--text); font-size: 5px; font-weight: 700; text-anchor: middle; dominant-baseline: central; }
.map-note { margin: .35rem 0; font-size: .8rem; }
.overlay { pointer-events: none; }
.ground { fill: var(--floor); pointer-events: none; }
.grid-line { stroke: var(--grid); stroke-width: .05; stroke-opacity: .7; }
.grid-label, .furniture-label { fill: var(--muted); font-size: 1.1px; paint-order: stroke; stroke: #1d1f1a; stroke-width: .3px; stroke-linejoin: round; }
.furniture-label { fill: var(--text); }
.area-outline { fill: none; stroke: var(--boundary); stroke-width: .14; stroke-dasharray: .7 .45; pointer-events: none; }
.crown { fill: url(#crown-fill); fill-opacity: .82; stroke: var(--crown-line); stroke-width: .07; pointer-events: fill; cursor: pointer; }
.tip { fill: #94ae82; fill-opacity: .8; pointer-events: none; }
.tree.dimmed { opacity: .35; }
.s-NaturalDead .crown { fill: url(#hatch); fill-opacity: 1; stroke: var(--dead-line); stroke-dasharray: .35 .25; }
.s-Felled .crown { fill: var(--stump); fill-opacity: 1; stroke: var(--stump-mark); }
.crown-stop-hi { stop-color: var(--crown-hi); }
.crown-stop-lo { stop-color: var(--crown-lo); }
.hatch-ground { fill: var(--hatch-ground); }
.hatch-line { stroke: var(--dead-line); stroke-width: .14; }
.ring-pending { fill: none; stroke: var(--pending); stroke-width: .2; stroke-dasharray: .55 .35; }
.ring-approved { fill: none; stroke: var(--approved); stroke-width: .15; }
.crop-ring { fill: none; stroke: var(--crop); stroke-width: .22; }
.glyph-bg { fill: #0b1410; stroke-width: .08; }
.glyph-pending { stroke: var(--pending); }
.glyph-approved { stroke: var(--approved); }
.glyph-crop { stroke: #82b1ff; }
.glyph-text { font-size: .62px; font-weight: 800; text-anchor: middle; dominant-baseline: central; }
.glyph-text.p { fill: var(--pending); }
.glyph-text.a { fill: var(--approved); }
.glyph-text.c { fill: #a9caff; }
.stump-x { fill: none; stroke: var(--stump-mark); stroke-width: .11; stroke-linecap: round; }
.competitor-ring { fill: none; stroke: #e5b66a; stroke-width: .12; stroke-dasharray: .3 .18; }
.competitor-rank { fill: #fff; stroke: #151c1e; stroke-width: .14; paint-order: stroke; font-size: .95px; font-weight: 800; text-anchor: middle; dominant-baseline: central; }
.inspection-ring { fill: none; stroke: var(--accent); stroke-width: .2; }
.select-halo { fill: none; stroke: #000; stroke-width: .5; }
.select-ring { fill: none; stroke: var(--select); stroke-width: .24; }
.north-arrow { fill: var(--text); stroke: var(--text); stroke-width: .12; }
.scale-bar { fill: none; stroke: var(--text); stroke-width: .14; }
/* The legend opens as a panel over the map, so it takes no height from the map pane. */
.legend-box { position: relative; flex: none; }
.legend-box summary { min-height: 44px; display: flex; align-items: center; padding: 0 .6rem; border: 2px solid var(--line); border-radius: 8px; cursor: pointer; }
.legend-box[open] .legend { position: absolute; right: 0; top: calc(100% + 4px); z-index: 6; width: min(24rem, 80vw); max-height: 60vh; overflow-y: auto; padding: .6rem .8rem; border: 1px solid var(--line); border-radius: 8px; background: var(--panel); box-shadow: 0 8px 24px #000a; }
.legend ul { display: grid; gap: .25rem; margin: 0; padding: 0; list-style: none; }
.legend li { font-size: .86rem; }
.overlap { margin-top: .5rem; padding: .6rem; border: 2px dashed var(--pending); border-radius: 8px; }
.statements li { padding: .25rem .5rem; border-left: 3px solid var(--focus); background: rgba(0, 0, 0, .18); }
.table-wrap { max-width: 100%; overflow-x: auto; }
table { width: 100%; border-collapse: collapse; }
caption { padding: .3rem 0; color: var(--muted); text-align: left; caption-side: top; }
th, td { padding: .3rem .45rem; border-bottom: 1px solid var(--line); text-align: left; vertical-align: middle; }
#tree-table td, #tree-table tbody th { white-space: nowrap; }
thead th { position: sticky; top: 0; z-index: 1; background: var(--panel-raised); }
tr.current { background: var(--current-row); }
tr.current > * { border-top: 2px solid var(--select); border-bottom: 2px solid var(--select); }
dialog { width: min(38rem, calc(100vw - 2rem)); max-height: calc(100vh - 2rem); overflow: auto; padding: 1.2rem; border: 2px solid var(--panel-edge); border-radius: 12px; background: var(--panel); color: var(--text); }
dialog::backdrop { background: rgba(0, 0, 0, .65); }
.technical-plan summary { min-height: 44px; display: flex; align-items: center; cursor: pointer; }
.technical-plan { font-size: .85rem; color: var(--muted); }
@media (min-width: 1024px) {
  body { display: flex; flex-direction: column; height: 100vh; min-height: 36rem; }
  .status-bar, .messages { flex: none; }
  .layout { flex: 1 1 auto; min-height: 0; padding: .6rem; grid-template-columns: minmax(0, 1fr) clamp(24rem, 34vw, 31rem); grid-template-rows: minmax(0, 1fr); gap: .6rem; }
  .map-panel { display: flex; flex-direction: column; min-height: 0; padding: .6rem; }
  .map-viewport { flex: 1 1 auto; min-height: 0; }
  .map-pane { height: 100%; }
  #map { width: 100%; height: 100%; aspect-ratio: auto; max-height: none; }
  .overlap { max-height: 9rem; overflow-y: auto; }
  .side { display: flex; flex-direction: column; min-height: 0; gap: .5rem; }
  .view-tabs { display: flex; flex: none; }
  .pane { display: none; flex: 1 1 auto; min-height: 0; overflow-y: auto; gap: .6rem; padding-right: .15rem; }
  .side[data-pane="tree"] #pane-tree, .side[data-pane="plan"] #pane-plan, .side[data-pane="trees"] #pane-trees,
  .side[data-pane="history"] #pane-history, .side[data-pane="details"] #pane-details { display: grid; }
}

/* #885 owner map: palette/whorls from Unity; the transparent first-child discs keep server hit geometry. */
:root { --floor: rgb(70, 66, 50); --boundary: #eeeee2eb; --stump: rgb(206, 188, 150); }
.ground { fill: url(#canopy-ground); }
.tree .crown { fill: #1e4032; fill-opacity: .07; stroke: none; pointer-events: fill; }
.tree.s-NaturalDead .crown, .tree.s-Felled .crown { fill: #645e54; fill-opacity: .03; stroke: none; }
.deco, .deco *, .reference-tag, .candidate-tag { pointer-events: none; }
.tree.dimmed { opacity: .70; }
.stump-disc { fill: rgb(206, 188, 150); fill-opacity: .92; stroke: #5f513a; stroke-width: .035; }
.stump-ring { fill: none; stroke: #786848; stroke-width: .03; }
.inspection-ring { stroke-dasharray: .28 .18; stroke: #eaece6; }
.reference-tag, .candidate-tag { fill: #fff; stroke: #10191d; stroke-width: .22; paint-order: stroke; font-size: .72px; font-weight: 750; }
.glyph-text.d, .glyph-text.x { fill: #eeeadd; }
.glyph-dead, .glyph-executed { stroke: #eeeadd; }
.map-dpad { position: absolute; left: .5rem; top: .5rem; right: auto; bottom: auto; z-index: 3;
  display: grid; grid-template-columns: repeat(3, 44px); grid-template-rows: repeat(3, 44px); gap: 2px; pointer-events: none; }
.map-dpad button { pointer-events: auto; background: var(--panel); box-shadow: 0 2px 8px #0009; }
.map-dpad .d-c { grid-column: 2; grid-row: 2; display: grid; place-items: center; font-size: .7rem;
  text-shadow: 0 1px 3px #000; pointer-events: none; }
#pan-north { grid-column: 2; grid-row: 1; }
#pan-west { grid-column: 1; grid-row: 2; }
#pan-east { grid-column: 3; grid-row: 2; }
#pan-south { grid-column: 2; grid-row: 3; }
.map-zoom, .map-controls { pointer-events: none; }
.map-zoom button, .map-controls button { pointer-events: auto; }
.release-option label { display: flex; align-items: center; gap: .5rem; font-size: .84rem; }
.release-option input { flex: none; width: 44px; height: 44px; min-height: 44px; margin: 0; }
.optional-note summary { min-height: 44px; display: flex; align-items: center; cursor: pointer; color: var(--muted); }
.competitors { gap: .2rem; }
.competitors li { display: grid; grid-template-columns: minmax(7rem, .85fr) minmax(0, 1.2fr); gap: 0 .4rem; align-items: center; }
.competitors button { grid-row: span 2; min-width: 0; overflow-wrap: anywhere; padding: .25rem .4rem; }
.competitors span { font-size: .75rem; }
.competition-totals { margin: .25rem 0; font-size: .84rem; }
.add-form { margin: .3rem 0; }
.add-form .primary { width: 100%; }
.small-note { font-size: .80rem; }
.plan-actions { position: sticky; top: 0; z-index: 2; padding: .25rem 0; background: var(--panel); }
.plan-area { padding: .2rem .4rem; }
.plan-area h4 { margin-top: .2rem; }
.orders li { padding: .2rem .4rem; }
@media (max-width: 1023px) {
  .map-viewport { min-height: 19rem; }
  #map.fit, #map { min-height: 19rem; }
  .view-tabs { display: flex; }
}
@media (min-width: 1024px) {
  .panel { padding: .6rem; }
  .pane .panel { padding: .55rem; }
  .pane h3 { margin-top: .45rem; }
  .map-viewport { min-height: 0; }
  .pane { scrollbar-gutter: stable; }
}


.tree-record summary { min-height: 44px; display: flex; align-items: center; cursor: pointer; color: var(--muted); }
/* The map owns its viewport; zooming never creates a second scrolling surface. */
.map-pane { overflow: hidden; }
.map-aside .legend-box[open] { overflow: visible; max-height: none; }
.map-aside .legend-box { position: relative; }
.map-aside .legend-box[open] .legend { position: absolute; bottom: 100%; right: 0; z-index: 6;
  width: min(25rem, 80vw); max-height: 65vh; overflow-y: auto; padding: .6rem; background: var(--panel);
  border: 1px solid var(--line); border-radius: 8px; }
@media (min-width: 1024px) {
  .view-tab { min-width: 0; padding: .3rem .4rem; }
  .quick-id label { font-size: .8rem; }
  #map.fit { width: 100%; height: 100%; max-height: none; }
}


.map-dpad button.icon { width: 44px; height: 44px; }
.map-zoom { display: flex; flex-direction: column; flex-wrap: nowrap; }
/* At narrow widths all sections stack in document order; desktop tabs select one scrolling pane. */
@media (max-width: 1023px) { .view-tabs { display: none; } }
@media (min-width: 1024px) {
  .status-bar { padding: .3rem .7rem; gap: .2rem .6rem; }
  .rail { gap: .25rem .4rem; }
  .rail > div { padding: 0 .4rem; }
  .rail dt { font-size: .64rem; }
  .rail dd { font-size: .86rem; }
  .brand h1 { font-size: 1.12rem; }
  .disclosure { font-size: .7rem; }
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
'use strict';

(function () {
  const SVG_NS = 'http://www.w3.org/2000/svg';
  const STATUS_LABEL = { Living: 'StatusLiving', PendingFell: 'StatusPendingFell', ApprovedFell: 'StatusApprovedFell',
    Felled: 'StatusFelled', NaturalDead: 'StatusNaturalDead' };
  const ZOOMS = Array.from({ length: 11 }, (_, i) => Math.pow(4, i / 10)); // reversible ~1.15x steps
  const PANES = ['tree', 'plan', 'trees', 'history', 'details'];
  const COMMAND_IDS = ['crop-button', 'add-button', 'insp-remove', 'approve', 'cancel-approved', 'execute',
    'execute-confirm', 'advance', 'consent-advance'];
  const STATEMENTS = ['MultiSyntheticStatement', 'MultiNotComparable', 'MultiSiteUnknown', 'MultiEdgeExcluded',
    'MultiNoTransfer', 'MultiSessionOnly'];
  // Header chips: a short form shown, its full statement as the tooltip (the Details pane lists the full statements).
  const CHIPS = [['MultiChipSynthetic', 'MultiSyntheticStatement'], ['MultiChipNotComparable', 'MultiNotComparable'],
    ['MultiChipSiteUnknown', 'MultiSiteUnknown'], ['MultiChipEdgeExcluded', 'MultiEdgeExcluded'],
    ['MultiChipNoTransfer', 'MultiNoTransfer'], ['MultiChipSessionOnly', 'MultiSessionOnly']];

  const state = {
    view: null, // the latest multi-area view from the server
    scenes: {}, // per area: the map items the server built from that same view
    labels: null,
    areaId: null, // the area shown on the map and in the tree table
    selected: {}, // per area: the UI-only selected exact tree ID, never an authority
    overlap: null, // { areaId, candidates } from the latest map hit-test
    competition: null, // { areaId, report } read from the server for competitionTarget
    competitionTarget: null, // { areaId, treeId }
    competitionTag: null,
    competitionRequest: 0,
    busy: false,
    lock: null, // set when the page can no longer trust what it shows; only a re-read lifts it
    lockCount: 0,
    confirmation: null, // the open Execute confirmation: a frozen copy of the plan and fence it shows
    opener: null,
    lastExecution: null,
    lastWasReplay: false,
    zoom: 1,
    centre: null,
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
    return area ? area.label + ' (' + area.areaId + ')' : String(areaId);
  }
  // A compact form for table rows in the narrow side panel, still with the exact area ID.
  function areaShort(areaId) {
    const area = areaView(areaId);
    return area ? area.label.replace(/^Area /, '') + ' · ' + area.areaId : String(areaId);
  }
  function selectedId() { return state.areaId ? state.selected[state.areaId] || null : null; }
  function ordersIn(orders, areaId) { return orders.filter((order) => order.areaId === areaId); }
  function statusText(item) {
    const template = label(STATUS_LABEL[item.status]);
    return item.isCrop ? label('CropRetained') : template ? fill(template, { y: item.deathYear }) : String(item.status);
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
    if (payload.labels) state.labels = payload.labels;
    if (!areaView(state.areaId)) state.areaId = state.view.areas.length > 0 ? state.view.areas[0].areaId : null;
    for (const areaId of Object.keys(state.selected)) {
      if (!treeIn(areaId, state.selected[areaId])) delete state.selected[areaId];
    }
    if (!keepOverlap) state.overlap = null;
    const target = state.competitionTarget;
    if (target && !livingIn(target.areaId, target.treeId)) clearCompetition();
    const refresh = state.competitionTarget !== null && state.competitionTag !== competitionTag();
    if (refresh) state.competition = null;
    renderAll();
    const confirmation = state.confirmation;
    if (confirmation && !byId('execute-dialog').open) state.confirmation = null;
    else if (confirmation && !confirmationStillShown(confirmation, state.view)) withdrawConfirmation(confirmation);
    if (refresh && !state.lock) refreshCompetition();
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
    renderCommandGate();
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

  function renderCommandGate() {
    const locked = state.lock !== null;
    for (const id of COMMAND_IDS) byId(id).disabled = locked;
    for (const button of document.querySelectorAll('[data-remove]')) button.disabled = locked;
  }

  function keepFocusOr(id) {
    const active = document.activeElement;
    const usable = active && active !== document.body && active.isConnected && !active.closest('[hidden]');
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
        order.treeId === confirmation.orders[index].treeId && order.reason === confirmation.orders[index].reason);
  }

  function withdrawConfirmation(confirmation) {
    state.confirmation = null;
    const dialog = byId('execute-dialog');
    if (dialog.open) dialog.close();
    lockCommands('Confirmation withdrawn — the approved plan changed',
      'The Execute confirmation showed ' + confirmation.orders.map((order) => order.treeId).join(', ') + '. The view that ' +
      'has just arrived no longer shows that plan with that fence, so nothing was sent. Review the plan now shown, then ' +
      'choose Re-read; after a successful re-read you can choose Execute again.');
  }

  // ---- rendering ------------------------------------------------------------------------------------------------------

  function renderAll() {
    if (!state.view || !state.labels) return;
    const active = document.activeElement;
    const focusKey = active && active.getAttribute ? active.getAttribute('data-focus-key') : null;
    renderStatus();
    renderAreaSwitch();
    renderMap();
    renderOverview();
    renderOverlap();
    renderInspector();
    renderCompetition();
    renderPlan();
    renderReceipt();
    renderTable();
    renderHistory();
    renderDetails();
    renderPaneButtons();
    renderCommandGate();
    renderMapControls();
    // Re-created buttons get their focus back by a stable key (an exact ID), so a redraw is not a focus reset.
    if (focusKey) {
      const again = document.querySelector('[data-focus-key="' + CSS.escape(focusKey) + '"]');
      if (again) again.focus();
    }
  }

  function renderStatus() {
    const view = state.view;
    document.title = 'Two-area Stand Map — ' + label('MultiTitle');
    byId('page-title').textContent = label('MultiTitle');
    byId('disclosure').textContent = label('MultiDisclosure');
    const approved = view.approved ? view.approved.orders.length : 0;
    const planState = approved > 0 ? 'approved, ' + approved + ' to execute' : view.pending.length > 0 ? view.pending.length + ' pending' : 'empty';
    byId('rail').replaceChildren(
      pair('Year', view.source.ecologicalYear),
      ...view.areas.map((area) => pair(area.label + ' living', area.stand.livingCount + ' · BA ' + fixed(area.stand.basalAreaM2PerHa, 1) + ' m²/ha')),
      pair('Work Plan', planState));
    byId('flags').replaceChildren(...CHIPS.map(([chip, full]) => el('li', { title: label(full) }, label(chip))));
    byId('advance').textContent = label('MultiAdvanceYear');
  }

  function renderAreaSwitch() {
    const view = state.view;
    byId('area-switch').replaceChildren(...view.areas.map((area) => {
      const inPlan = ordersIn(view.pending, area.areaId).length + (view.approved ? ordersIn(view.approved.orders, area.areaId).length : 0);
      return el('button', { type: 'button', class: 'area-tab', 'data-area': area.areaId, 'data-focus-key': 'area:' + area.areaId,
        'aria-pressed': String(area.areaId === state.areaId) },
        el('span', { class: 'area-name' }, area.label + ' · ' + area.areaId),
        el('span', { class: 'area-meta' }, area.stand.livingCount + ' living · ' + fixed(area.areaHa, 4) + ' ha' +
          (inPlan > 0 ? ' · ' + inPlan + ' in plan' : '')));
    }));
  }

  // ---- the map: one SVG user unit is one metre; U runs east from the extent's west edge, V south from its north edge ----

  function fullBox(area) {
    const side = area.extent.sideM;
    return { x: -3.4, y: -2.2, w: side + 6.0, h: side + 5.4 };
  }

  function viewBox(area) {
    const full = fullBox(area);
    const w = full.w / state.zoom;
    const h = full.h / state.zoom;
    const centre = state.centre || { u: full.x + full.w / 2, v: full.y + full.h / 2 };
    const x = Math.min(full.x + full.w - w, Math.max(full.x, centre.u - w / 2));
    const y = Math.min(full.y + full.h - h, Math.max(full.y, centre.v - h / 2));
    return { x, y, w, h };
  }

  function renderMap() {
    const area = currentArea();
    if (!area) return;
    const side = area.extent.sideM;
    const box = viewBox(area);
    byId('map').setAttribute('viewBox', [box.x, box.y, box.w, box.h].join(' '));
    setAttributes(byId('ground'), { x: 0, y: 0, width: side, height: side });
    const toU = (x) => x - area.extent.minXM;
    const toV = (z) => area.extent.minZM + side - z;
    byId('area-outline').setAttribute('points', area.polygon.map((p) => toU(p.xm) + ',' + toV(p.zm)).join(' '));
    drawGrid(area);

    const report = state.competition && !state.lock && state.competition.areaId === area.areaId ? state.competition.report : null;
    const focusIds = report ? new Set([report.targetTreeId, ...report.topFive.map((n) => n.treeId)]) : null;
    const crowns = [];
    const marks = [];
    const items = new Map(sceneOf(area.areaId).map((item) => [item.treeId, item]));
    for (const item of sceneOf(area.areaId).slice().sort((a, b) => a.paintOrder - b.paintOrder)) {
      const dimmed = focusIds && !focusIds.has(item.treeId);
      const group = svg('g', { class: 'tree s-' + item.status + (dimmed ? ' dimmed' : ''), 'data-tree-id': item.treeId });
      group.append(svg('circle', { class: 'crown', cx: item.u, cy: item.v, r: item.radiusM }));
      group.append(...crownDecoration(item));
      crowns.push(group);
      if (item.isCrop) {
        marks.push(svg('circle', { class: 'crop-ring', cx: item.u, cy: item.v, r: item.radiusM + 0.2 }), ...badge(item, 'C'));
      }
      if (item.status === 'PendingFell') {
        marks.push(svg('circle', { class: 'ring-pending', cx: item.u, cy: item.v, r: item.radiusM + 0.25 }), ...badge(item, 'P'));
      } else if (item.status === 'ApprovedFell') {
        marks.push(svg('circle', { class: 'ring-approved', cx: item.u, cy: item.v, r: item.radiusM + 0.2 }),
          svg('circle', { class: 'ring-approved', cx: item.u, cy: item.v, r: item.radiusM + 0.5 }), ...badge(item, 'A'));
      } else if (item.status === 'NaturalDead') {
        marks.push(...badge(item, 'D'));
      } else if (item.status === 'Felled') {
        marks.push(...badge(item, 'X'));
        const d = 0.24;
        marks.push(svg('path', { class: 'stump-x', d: 'M ' + (item.u - d) + ' ' + (item.v - d) + ' L ' + (item.u + d) + ' ' +
          (item.v + d) + ' M ' + (item.u - d) + ' ' + (item.v + d) + ' L ' + (item.u + d) + ' ' + (item.v - d) }));
      }
    }
    if (report) {
      report.topFive.forEach((entry, rank) => {
        const item = items.get(entry.treeId);
        if (!item) return;
        marks.push(svg('circle', { class: 'competitor-ring', cx: item.u, cy: item.v, r: item.radiusM + 0.35 }),
          svg('text', { class: 'competitor-rank', x: item.u, y: item.v }, rank + 1));
      });
      const target = items.get(report.targetTreeId);

    }
    const reference = state.competitionTarget && state.competitionTarget.areaId === area.areaId
      ? items.get(state.competitionTarget.treeId) : null;
    if (reference) marks.push(...referenceMarks(reference));
    byId('crowns').replaceChildren(...crowns);
    byId('marks').replaceChildren(...marks);
    const chosen = selectedId() ? items.get(selectedId()) : null;
    byId('selection').replaceChildren(...(chosen ? [
      svg('circle', { class: 'select-halo', cx: chosen.u, cy: chosen.v, r: chosen.radiusM + 0.45 }),
      svg('circle', { class: 'select-ring', cx: chosen.u, cy: chosen.v, r: chosen.radiusM + 0.45 }),
      svg('text', { class: 'candidate-tag', x: chosen.u, y: chosen.v + chosen.radiusM + 1, 'text-anchor': 'middle' },
        reference && reference.treeId !== chosen.treeId ? 'Fell candidate' : 'Selected'),
    ] : []));
    byId('map-desc').textContent = 'North-up map of ' + areaName(area.areaId) + ', ' + area.areaM2Exact + ' square metres. ' +
      'Ecological year ' + state.view.source.ecologicalYear + '. Living trees ' + area.stand.livingCount + '. ' +
      (selectedId() ? 'Selected tree ' + selectedId() + '. ' : '') + 'Use the tree table to select a tree by keyboard.';
    byId('map-note').textContent = areaName(area.areaId) + ' · ' + (state.zoom === 1 ? 'Fit whole area' :
      fixed(state.zoom, 2) + '× Fit · viewport ' + fixed(box.w, 1) + ' × ' + fixed(box.h, 1) + ' m') +
      ' · 5 m grid · site ' + area.site + ' · neighbours across area edges excluded.';
    renderLegend();
  }


  // SVG adaptation of StandMapCanopyRaster and StandMapSymbols: five reusable seeded whorl variants,
  // true server radius, north-west light L=(-.52,+.56,+.64), ambient .56, diffuse .66. Shape/tint are drawing only.
  // Raster leaf pixels are approximated by lit branch facets and needles; no biological values are inferred.
  const CANOPY_PALETTE = {
    ground: [70, 66, 50], conifer: [30, 64, 50], beech: [92, 128, 70], oak: [116, 128, 64],
    otherBroadleaf: [104, 140, 80], dead: [100, 94, 84], stump: [206, 188, 150],
  }; // Only conifer is used: the current server supplies synthetic Sitka, not broadleaf species.

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

  function canopyColour(base, shade, warm = 0) {
    return 'rgb(' + base.map((v, i) => Math.max(0, Math.min(255,
      Math.round(v * shade * (i === 0 ? 1 + warm : i === 2 ? 1 - warm * .9 : 1))))).join(',') + ')';
  }

  function canopyLight(x, y) {
    const z = Math.sqrt(Math.max(0, 1 - x * x - y * y));
    return .56 + .66 * Math.max(0, x * -.52 - y * .56 + z * .64);
  }

  function groundNoise(x, y, cells, seed) {
    const fx = x * cells, fy = y * cells;
    const ix = Math.floor(fx), iy = Math.floor(fy);
    const smooth = (t) => t * t * (3 - 2 * t);
    const tx = smooth(fx - ix), ty = smooth(fy - iy);
    const at = (a, b) => (canopyHash(a % cells, b % cells, seed) & 0xffffff) / 16777216;
    const lerp = (a, b, t) => a + (b - a) * t;
    return lerp(lerp(at(ix, iy), at(ix + 1, iy), tx), lerp(at(ix, iy + 1), at(ix + 1, iy + 1), tx), ty);
  }

  function drawCanopyDefs() {
    const defs = byId('map').querySelector('defs');
    const shadow = svg('radialGradient', { id: 'canopy-shadow' });
    shadow.append(svg('stop', { offset: '0%', 'stop-color': '#000', 'stop-opacity': .40 }),
      svg('stop', { offset: '48%', 'stop-color': '#000', 'stop-opacity': .40 }),
      svg('stop', { offset: '100%', 'stop-color': '#000', 'stop-opacity': 0 }));
    defs.append(shadow);
    // Two periodic value-noise lattices at different scales, sampled once into shared SVG tiles.
    [12.8, 9.7].forEach((period, index) => {
      const tile = svg('pattern', { id: 'moss-' + index, patternUnits: 'userSpaceOnUse', width: period, height: period });
      const seed = index === 0 ? 0x51ed270b : 0x9e3779b9;
      const size = 24, cell = period / size;
      for (let y = 0; y < size; y++) for (let x = 0; x < size; x++) {
        const n = 255 * (.65 * groundNoise(x / size, y / size, 14, seed) +
          .35 * groundNoise(x / size, y / size, 5, seed ^ 0xabcd)) - 128;
        const colour = CANOPY_PALETTE.ground.map((v, i) => Math.round(v + n / [7, 6, 8][i]));
        tile.append(svg('rect', { x: x * cell, y: y * cell, width: cell + .005, height: cell + .005,
          fill: 'rgb(' + colour.join(',') + ')' }));
      }
      defs.append(tile);
    });
    const floor = svg('pattern', { id: 'canopy-ground', patternUnits: 'userSpaceOnUse', width: 124.16, height: 124.16 });
    floor.append(svg('rect', { width: 124.16, height: 124.16, fill: 'url(#moss-0)' }),
      svg('rect', { width: 124.16, height: 124.16, fill: 'url(#moss-1)', opacity: .4 }));
    defs.append(floor);
    for (let variant = 0; variant < 5; variant++) {
      const seed = canopyHash(977, 16, variant + 7);
      const tint = .87 + .26 * canopyRandom(seed, 1);
      const warm = -.07 + .14 * canopyRandom(seed, 3);
      const gradient = svg('radialGradient', { id: 'whorl-light-' + variant, cx: '.31', cy: '.27', r: '.75' });
      [[0, 1.24], [.45, canopyLight(-.25, -.25)], [1, .42]].forEach(([offset, shade]) =>
        gradient.append(svg('stop', { offset, 'stop-color': canopyColour(CANOPY_PALETTE.conifer, shade * tint, warm) })));
      defs.append(gradient);
      [false, true].forEach((small) => {
        const tips = small ? 6 + variant % 3 : 9 + variant;
        const valley = small ? .62 : .70;
        const symbol = svg('g', { id: 'whorl-' + (small ? 'small-' : '') + variant });
        const points = [];
        const point = (a, r) => [Math.cos(a) * r, Math.sin(a) * r];
        for (let k = 0; k < tips * 2; k++) {
          const angle = k * Math.PI / tips - Math.PI / 2 + Math.PI * 2 * canopyRandom(seed, 15);
          const rag = .94 + .035 * Math.cos(2 * angle + variant) + .025 * Math.cos(3 * angle + seed);
          points.push(point(angle, rag * (k % 2 === 0 ? 1 : valley)));
        }
        symbol.append(svg('polygon', { points: points.map((p) => p.join(',')).join(' '), fill: 'url(#whorl-light-' + variant + ')' }));
        let needles = '';
        for (let branch = 0; branch < tips; branch++) {
          const tip = points[branch * 2];
          const notch = points[(branch * 2 + 1) % points.length];
          const shade = canopyLight(tip[0] * .6, tip[1] * .6) * tint;
          symbol.append(svg('path', { d: 'M 0 0 L ' + tip.join(' ') + ' L ' + notch.join(' ') + ' Z',
            fill: canopyColour(CANOPY_PALETTE.conifer, shade * 1.1, warm), opacity: .40 }));
          for (let step = 2; step <= 5; step++) {
            const t = step / 7;
            const x = tip[0] * t, y = tip[1] * t;
            const width = (1 - t) * .09;
            needles += 'M ' + (x - tip[1] * width) + ' ' + (y + tip[0] * width) + ' L ' +
              (x + tip[0] * .12) + ' ' + (y + tip[1] * .12) + ' L ' +
              (x + tip[1] * width) + ' ' + (y - tip[0] * width) + ' ';
          }
        }
        symbol.append(svg('path', { d: needles, fill: 'none', stroke: canopyColour(CANOPY_PALETTE.conifer, 1.5 * tint, warm),
          'stroke-width': .014, opacity: small ? .30 : .52 }),
          svg('circle', { cx: -.025, cy: -.025, r: .075, fill: canopyColour(CANOPY_PALETTE.conifer, 1.8 * tint, warm) }));
        defs.append(symbol);
      });
      const dead = svg('g', { id: 'bare-branches-' + variant, opacity: .68 });
      let branches = '';
      for (let k = 0; k < 9; k++) {
        const angle = k * Math.PI * 2 / 9 + variant;
        const x = Math.cos(angle) * (.64 + .14 * canopyRandom(seed, k + 20));
        const y = Math.sin(angle) * (.64 + .14 * canopyRandom(seed, k + 20));
        branches += 'M 0 0 L ' + x + ' ' + y + ' M ' + x * .55 + ' ' + y * .55 +
          ' l ' + (-y * .19) + ' ' + (x * .19) + ' M ' + x * .72 + ' ' + y * .72 +
          ' l ' + (y * .15) + ' ' + (-x * .15) + ' ';
      }
      dead.append(svg('path', { d: branches, fill: 'none', stroke: canopyColour(CANOPY_PALETTE.dead, .88),
        'stroke-width': .045, 'stroke-linecap': 'round' }));
      defs.append(dead);
    }
  }

  function crownDecoration(item) {
    const { u, v } = item;
    const r = item.radiusM;
    if (item.status === 'Felled') return [
      svg('circle', { class: 'stump-disc deco', cx: u, cy: v, r }),
      svg('circle', { class: 'stump-ring deco', cx: u, cy: v, r: r * .65 }),
      svg('circle', { class: 'stump-ring deco', cx: u, cy: v, r: r * .32 }),
    ];
    const seed = canopySeed(item.treeId);
    const variant = seed % 5;
    const stretch = .90 + .20 * canopyRandom(seed, 13);
    const dead = item.status === 'NaturalDead';
    const id = dead ? 'bare-branches-' + variant : 'whorl-' + (r < .65 ? 'small-' : '') + variant;
    return [svg('circle', { class: 'deco', cx: u + r * .30, cy: v + r * .30, r: r * 1.15,
      fill: 'url(#canopy-shadow)', opacity: dead ? .45 : 1 }),
      svg('use', { class: 'deco', href: '#' + id, opacity: .90 + .10 * canopyRandom(seed, 1),
        transform: 'translate(' + u + ' ' + v + ') scale(' +
          (r * Math.min(1, stretch)) + ' ' + (r * Math.min(1, 1 / stretch)) + ')' })];
  }

  function referenceMarks(item) {
    return [svg('circle', { class: 'inspection-ring', cx: item.u, cy: item.v, r: item.radiusM + .6 }),
      svg('text', { class: 'reference-tag', x: item.u, y: item.v - item.radiusM - .85, 'text-anchor': 'middle' }, 'Reference')];
  }

  function badge(item, letter) {
    const angle = -Math.PI / 4;
    const cx = item.u + Math.cos(angle) * (item.radiusM + 0.25);
    const cy = item.v + Math.sin(angle) * (item.radiusM + 0.25);
    const kind = letter === 'P' ? 'pending' : letter === 'C' ? 'crop' : letter === 'D' ? 'dead' : letter === 'X' ? 'executed' : 'approved';
    return [svg('circle', { class: 'glyph-bg glyph-' + kind, cx, cy, r: 0.42 }),
      svg('text', { class: 'glyph-text ' + letter.toLowerCase(), x: cx, y: cy }, letter)];
  }

  // Grid lines every 5 m of the experiment frame, labelled in the same X (east) and Z (north) metres as the tree table.
  function drawGrid(area) {
    const side = area.extent.sideM;
    const minX = area.extent.minXM;
    const minZ = area.extent.minZM;
    const lines = [];
    const labels = [];
    for (let x = Math.ceil(minX / 5) * 5; x <= minX + side + 1e-9; x += 5) {
      const u = x - minX;
      lines.push(svg('line', { class: 'grid-line', x1: u, y1: 0, x2: u, y2: side }));
      labels.push(svg('text', { class: 'grid-label', x: u, y: side + 1.5, 'text-anchor': 'middle' }, x));
    }
    for (let z = Math.ceil(minZ / 5) * 5; z <= minZ + side + 1e-9; z += 5) {
      const v = minZ + side - z;
      lines.push(svg('line', { class: 'grid-line', x1: 0, y1: v, x2: side, y2: v }));
      labels.push(svg('text', { class: 'grid-label', x: -0.5, y: v + 0.4, 'text-anchor': 'end' }, z));
    }
    labels.push(svg('text', { class: 'grid-label', x: side / 2, y: side + 2.9, 'text-anchor': 'middle' }, 'X metres east'));
    byId('grid').replaceChildren(...lines);
    byId('furniture').replaceChildren(...labels,
      svg('path', { class: 'north-arrow', d: 'M ' + (side + 1.4) + ' 3.2 L ' + (side + 1.4) + ' 1.0 M ' + (side + 1.4) + ' 0.1 L ' +
        (side + 0.8) + ' 1.2 L ' + (side + 2.0) + ' 1.2 Z' }),
      svg('text', { class: 'furniture-label', x: side + 1.4, y: -0.8, 'text-anchor': 'middle' }, 'N'),
      svg('path', { class: 'scale-bar', d: 'M 0 ' + (side + 2.4) + ' L 5 ' + (side + 2.4) }),
      svg('text', { class: 'furniture-label', x: 5.5, y: side + 2.75 }, '5 m'));
  }

  // Both polygons in the experiment frame, so the shared boundary (and its excluded neighbours) is visible at a glance.
  function renderOverview() {
    const areas = state.view.areas;
    const points = areas.flatMap((area) => area.polygon);
    if (points.length === 0) return;
    const minX = Math.min(...points.map((p) => p.xm));
    const maxX = Math.max(...points.map((p) => p.xm));
    const minZ = Math.min(...points.map((p) => p.zm));
    const maxZ = Math.max(...points.map((p) => p.zm));
    const pad = 3;
    const overview = byId('overview');
    overview.setAttribute('viewBox', [minX - pad, -maxZ - pad, maxX - minX + 2 * pad, maxZ - minZ + 2 * pad].join(' '));
    overview.replaceChildren(...areas.flatMap((area) => {
      const cx = area.polygon.reduce((sum, p) => sum + p.xm, 0) / area.polygon.length;
      const cz = area.polygon.reduce((sum, p) => sum + p.zm, 0) / area.polygon.length;
      return [svg('polygon', { class: 'ov-area' + (area.areaId === state.areaId ? ' current' : ''),
        points: area.polygon.map((p) => p.xm + ',' + (-p.zm)).join(' ') }),
        svg('text', { class: 'ov-label', x: cx, y: -cz }, area.label.replace(/^Area /, ''))];
    }));
  }

  function renderLegend() {
    byId('legend').replaceChildren(el('ul', null,
      el('li', null, 'Green crown — living Sitka spruce (true crown radius, server positions)'),
      el('li', null, 'Blue ring + C — retained Crop Tree (a designation; changes no biology)'),
      el('li', null, 'Dashed yellow ring + P — pending Fell, still standing'),
      el('li', null, 'Double orange ring + A — approved Fell, still standing'),
      el('li', null, 'Light stump + rings + X — executed Fell; canopy gap open'),
      el('li', null, 'Bare branches + D — natural death'),
      el('li', null, 'Ranks 1–5 — real competitors; dashed halo + Reference tag — reference tree'),
      el('li', null, 'Thick white ring + Fell candidate tag — selected neighbour; no felling until execution'),
      el('li', null, 'Dashed white outline — this area’s boundary; the inset shows both areas')));
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

  function renderInspector() {
    const areaId = state.areaId;
    const id = selectedId();
    const tree = treeIn(areaId, id);
    const item = tree ? itemOf(areaId, id) : null;
    byId('inspector-empty').hidden = Boolean(tree && item);
    byId('inspector-body').hidden = !(tree && item);
    if (!tree || !item) return;
    byId('insp-area').textContent = 'In ' + areaName(areaId);
    byId('insp-id').textContent = tree.treeId;
    const chip = byId('insp-chip');
    chip.textContent = statusText(item);
    chip.setAttribute('class', 'chip s-' + item.status);
    byId('insp-summary').textContent = 'DBH ' + fixed(tree.dbhCm, 1) + ' cm · height ' + fixed(tree.heightM, 1) + ' m · crown ' +
      fixed(tree.crownRadiusM, 2) + ' m · age ' + tree.ageYears;
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
    const reason = recordedReason(areaId, tree.treeId);
    if (reason !== null) facts.push(pair('Recorded reason', reason, 'wide'));
    byId('insp-facts').replaceChildren(...facts);
    const crop = byId('crop-button');
    crop.textContent = label(item.isCrop ? 'MultiUnmarkCrop' : 'MultiMarkCrop');
    crop.hidden = !tree.isLiving;
    byId('crop-note').textContent = item.isCrop ? label('MultiCropFellRefused') + '.' :
      item.status === 'PendingFell' || item.status === 'ApprovedFell' ? 'Remove the pending Fell or cancel its approved plan before designating Crop.' :
        tree.isLiving ? label('MultiCropSeparate') + '.' : 'Dead trees cannot retain a Crop designation.';
    byId('add-form').hidden = item.status !== 'Living' || item.isCrop;
    byId('add-button').textContent = 'Add tree ' + tree.treeId + ' to Fell plan';
    byId('insp-remove-wrap').hidden = item.status !== 'PendingFell';
    byId('insp-remove').textContent = label('MultiRemoveFromPlan');
    byId('insp-approved-note').hidden = item.status !== 'ApprovedFell';
    byId('insp-threshold').textContent = label('MultiThresholdInformational');
    byId('insp-threshold').hidden = !tree.isLiving;
    renderReasonPicker();
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
      renderAll();
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

  // The release reason is offered only for a listed top-five competitor of an inspected, retained Crop Tree in this area.
  function releaseReason() {
    const comp = inspectedCrop();
    return comp && comp.areaId === state.areaId && comp.report.topFive.some((n) => n.treeId === selectedId())
      ? 'Competition release for retained Crop Tree ' + comp.report.targetTreeId + ' (operator intent)' : '';
  }

  function renderCompetition() {
    const box = byId('competition-content');
    const areaId = state.areaId;
    const id = selectedId();
    const comp = state.lock ? null : state.competition;
    const report = comp && comp.areaId === areaId ? comp.report : null;
    const parts = [];
    if (report) {
      const isCrop = areaView(areaId).cropTreeIds.includes(report.targetTreeId);
      parts.push(el('p', { class: 'small-note' }, (isCrop ? 'Retained Crop reference ' : 'Reference tree ') + report.targetTreeId +
        ' · ' + report.neighbours.length + ' living neighbours in ' + areaName(areaId) + ' within ' + report.cutoffM + ' m.'));
      parts.push(el('p', { class: 'competition-totals' }, 'Current total Hegyi CI ' + fixed(report.currentTotal, 4) +
        (report.targetPlannedForFell ? ' · This tree is planned for Fell: no release estimate.' :
          ' → ' + fixed(report.hypotheticalTotal, 4) + ' if its pending or approved neighbours were felled.')));
      parts.push(el('ol', { class: 'competitors' }, ...report.topFive.map((n, i) => {
        const focused = id === n.treeId;
        return el('li', { class: focused ? 'focused' : null },
          el('button', { type: 'button', 'data-competitor': n.treeId, 'data-focus-key': 'competitor:' + n.treeId, 'aria-pressed': String(focused) },
            (i + 1) + ' · ' + n.treeId),
          el('span', null, fixed(n.dbhCm, 1) + ' cm DBH · ' + fixed(n.distanceM, 2) + ' m away · CI term ' + fixed(n.contribution, 4)),
          el('span', null, fixed(n.sharePercent, 1) + '% share · ' + n.strength + (n.plannedRemoval === 'None' ? '' : ' · ' + n.plannedRemoval + ' Fell')));
      })));
      if (report.topFive.length === 0) parts.push(el('p', null, 'No living competitor inside the 8 m cutoff.'));
      parts.push(el('p', { class: 'small-note' }, 'Other neighbours: ' + report.remainingCount + ' · combined ' +
        fixed(report.remainingSharePercent, 1) + '% share. Strength describes share of CI, not tree health.'));
      if (id !== report.targetTreeId) {
        parts.push(el('button', { type: 'button', 'data-competitor': report.targetTreeId, 'data-focus-key': 'competitor-return' },
          'Return to reference tree ' + report.targetTreeId));
      }
    } else if (id && livingIn(areaId, id) && state.competitionTarget) {
      parts.push(el('p', null, 'Reading competition…'));
    } else if (id && !livingIn(areaId, id)) {
      parts.push(el('p', null, 'Competition is shown for living trees only.'));
    }
    box.replaceChildren(...parts);
    renderReasonPicker();
  }

  // ---- the reason picker -----------------------------------------------------------------------------------------------

  function reasonToSend() {
    // Explicit precedence: note if present, else the ticked release label, else Owner-selected felling.
    // This is operator intent only; the existing API still requires a nonempty reason.
    const note = byId('reason').value.trim();
    return note || (byId('reason-release').checked ? releaseReason() : '') || 'Owner-selected felling';
  }

  function renderReasonPicker() {
    const release = releaseReason();
    const checkbox = byId('reason-release');
    checkbox.disabled = release === '';
    if (checkbox.disabled) checkbox.checked = false;
    byId('release-wrap').hidden = release === '';
    byId('release-label').textContent = release === '' ? '' :
      'Record this as competition release for retained Crop Tree ' + state.competition.report.targetTreeId;
    byId('reason-preview').textContent = 'Recorded reason: ' + reasonToSend();
    showReasonError(null);
  }

  function showReasonError(message) {
    const box = byId('reason-error');
    box.hidden = message === null;
    box.textContent = message === null ? '' : message;
    for (const id of ['reason-release', 'reason']) if (message === null) byId(id).removeAttribute('aria-invalid');
    if (message !== null) reasonField().setAttribute('aria-invalid', 'true');
  }

  function reasonField() {
    return byId('reason');
  }

  function resetReasonPicker() {
    byId('reason-release').checked = false;
    byId('reason-other-wrap').open = false;
    byId('reason').value = '';
    renderReasonPicker();
    showReasonError(null);
  }

  // ---- the combined Work Plan, the receipt and the year results ----------------------------------------------------------

  function orderList(orders, className, removable) {
    return el('ul', { class: 'list orders ' + className }, ...orders.map((order) => el('li', null,
      el('span', { class: 'mono' }, order.treeId), el('span', { class: 'order-reason' }, 'Reason: ' + order.reason),
      removable ? el('button', { type: 'button', class: 'small', 'data-remove': order.treeId, 'data-area': order.areaId,
        'data-focus-key': 'remove:' + order.treeId, 'aria-label': label('MultiRemoveFromPlan') + ': ' + order.treeId },
      label('MultiRemoveFromPlan')) : null)));
  }

  function perAreaCounts(orders) {
    return state.view.areas.map((area) => area.label + ' ' + ordersIn(orders, area.areaId).length).join(' · ');
  }

  function renderPlan() {
    const view = state.view;
    byId('plan-scope').textContent = label('MultiOnePlan') + '. ' + label('MultiLastLivingRefused') + '.';
    byId('pending-total').textContent = 'Total pending: ' + view.pending.length + ' (' + perAreaCounts(view.pending) + ')';
    byId('pending-areas').replaceChildren(...view.areas.map((area) => {
      const orders = ordersIn(view.pending, area.areaId);
      return el('section', { class: 'plan-area', 'aria-label': 'Pending in ' + areaName(area.areaId) },
        el('h4', null, areaName(area.areaId) + ' — ' + orders.length + ' pending'),
        orders.length === 0 ? el('p', { class: 'note' }, 'No pending Fell in this area.') : orderList(orders, 'pending-orders', true));
    }));
    const approve = byId('approve');
    approve.textContent = label('MultiApprove');
    approve.hidden = view.pending.length === 0 || view.approved !== null;
    const plan = view.approved;
    byId('approved-empty').hidden = plan !== null;
    byId('approved-box').hidden = plan === null;
    if (plan === null) {
      byId('approved-areas').replaceChildren();
      byId('approved-total').textContent = '';
      return;
    }
    byId('approved-total').textContent = 'Approved, still standing: ' + plan.orders.length + ' (' + perAreaCounts(plan.orders) +
      ') · Year ' + plan.source.ecologicalYear;
    byId('approved-areas').replaceChildren(...view.areas.map((area) => {
      const orders = ordersIn(plan.orders, area.areaId);
      return el('section', { class: 'plan-area', 'aria-label': 'Approved in ' + areaName(area.areaId) },
        el('h4', null, areaName(area.areaId) + ' — ' + orders.length + ' approved'),
        orders.length === 0 ? el('p', { class: 'note' }, 'No Fell in this area.') : orderList(orders, 'approved-orders', false));
    }), technicalPlan(plan.planId, plan.executionKey));
    byId('cancel-approved').textContent = label('MultiCancelApproved');
    byId('execute').textContent = label('MultiExecute');
  }

  function renderReceipt() {
    const view = state.view;
    const parts = [];
    if (view.events.length === 0) {
      parts.push(el('p', null, 'No work has been executed in this session.'));
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
      parts.push(technicalPlan(event.planId, event.executionKey));
    }
    byId('receipt').replaceChildren(...parts);
  }

  function renderHistory() {
    const view = state.view;
    const years = view.annualHistory;
    const latest = byId('year-latest');
    if (years.length === 0) {
      latest.replaceChildren(el('p', null, 'No ecological year has been advanced in this session.'));
    } else {
      const record = years[years.length - 1];
      const carried = record.areas.some((year) => year.noLivingStems);
      latest.replaceChildren(
        el('p', null, 'Latest year ' + record.yearBefore + ' → ' + record.yearAfter + (record.cancelledUnexecutedWork ?
          ' · unexecuted work in both areas was cancelled at zero Fell' : '') + (carried
          ? '. Each populated area ran the unchanged annual rules on its own trees; an area with no living stems was carried forward unchanged.'
          : '. Each area ran the unchanged annual rules on its own trees.')),
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
    byId('statements').replaceChildren(...STATEMENTS.concat(['MultiReviewStatus', 'MultiEmptyAreaRule']).map((name) => el('li', null, label(name))));
    byId('facts').replaceChildren(
      pair('Experiment', e.experimentId), pair('Format', e.format + ' v' + e.schemaVersion), pair('Domain', e.domain),
      pair('Source class', e.sourceClass), pair('Manifest content hash', expandable(e.manifestContentHash)),
      pair('Generator', e.generatorId + ' v' + e.generatorVersion + ', seed ' + e.generatorSeed),
      pair('Review status', e.reviewStatus), pair('Edge model', e.edgeModel, 'wide'), pair('Site', e.siteNote, 'wide'),
      ...view.areas.map((area) => pair(areaName(area.areaId), area.areaM2Exact + ' m² (' + fixed(area.areaHa, 4) + ' ha) · seed ' +
        area.simulationSeed + ' · ' + area.trees.length + ' tree records')),
      pair('Models', 'RNG v' + s.rngModelVersion + ' · growth model v' + s.growthModelVersion + ' · rules ' + s.p1AlgorithmVersion +
        ' · ' + s.areaModelVersion), pair('Ecological year', s.ecologicalYear), pair('Annual revision', s.annualRevision),
      pair('Work revision', s.workRevision), pair('Plan-state revision', view.planStateRevision),
      pair('Session', expandable(view.sessionId)));
  }

  function renderPaneButtons() {
    const view = state.view;
    const inPlan = view.pending.length + (view.approved ? view.approved.orders.length : 0);
    const text = { tree: 'Tree', plan: 'Work Plan (' + inPlan + ')', trees: 'Trees', history: 'Results (' + view.annualHistory.length + ')', details: 'Details' };
    for (const button of document.querySelectorAll('[data-pane-button]')) {
      const name = button.getAttribute('data-pane-button');
      if (button.textContent !== text[name]) button.textContent = text[name];
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
    const box = viewBox(area), full = fullBox(area);
    byId('zoom-in').disabled = state.zoom === ZOOMS[ZOOMS.length - 1];
    byId('zoom-out').disabled = fit;
    byId('pan-north').disabled = fit || box.y <= full.y;
    byId('pan-south').disabled = fit || box.y + box.h >= full.y + full.h - .001;
    byId('pan-west').disabled = fit || box.x <= full.x;
    byId('pan-east').disabled = fit || box.x + box.w >= full.x + full.w - .001;
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
      const box = viewBox(area);
      state.centre = { u: box.x + box.w / 2, v: box.y + box.h / 2 };
    }
    state.zoom = next;
    if (next === ZOOMS[0]) state.centre = null;
    renderMap();
    renderMapControls();
  }

  function panMap(east, south) {
    const area = currentArea();
    if (!area || state.zoom === ZOOMS[0]) return;
    const box = viewBox(area);
    state.centre = { u: box.x + box.w / 2 + east * box.w / 4, v: box.y + box.h / 2 + south * box.h / 4 };
    const clamped = viewBox(area);
    state.centre = { u: clamped.x + clamped.w / 2, v: clamped.y + clamped.h / 2 };
    renderMap();
    renderMapControls();
  }

  // Keep the reference and quick-picked neighbour visible together, at a fixed reversible zoom level.
  function frameInspection() {
    const area = currentArea();
    const target = state.competitionTarget;
    const a = target ? itemOf(target.areaId, target.treeId) : null;
    const b = itemOf(state.areaId, selectedId());
    if (!area || !a || !b || state.zoom === 1) return;
    const left = Math.min(a.u - a.radiusM, b.u - b.radiusM) - 1.5;
    const right = Math.max(a.u + a.radiusM, b.u + b.radiusM) + 1.5;
    const top = Math.min(a.v - a.radiusM, b.v - b.radiusM) - 1.5;
    const bottom = Math.max(a.v + a.radiusM, b.v + b.radiusM) + 1.5;
    while (state.zoom > 1 && (viewBox(area).w < right - left || viewBox(area).h < bottom - top))
      state.zoom = ZOOMS[Math.max(0, ZOOMS.indexOf(state.zoom) - 1)];
    state.centre = { u: (left + right) / 2, v: (top + bottom) / 2 };
  }

  // ---- selection --------------------------------------------------------------------------------------------------------

  function showArea(areaId) {
    if (!areaView(areaId) || areaId === state.areaId) return;
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
    announce('Showing ' + areaName(areaId) + '.' + (id ? ' Selected tree ' + id + '.' : ''));
  }

  // Selecting any living tree immediately reads its competitors (read-only); it never marks Crop or proposes Fell.
  function chooseTree(areaId, treeId, fromList) {
    if (!treeIn(areaId, treeId)) return;
    if (areaId !== state.areaId) { state.areaId = areaId; state.zoom = ZOOMS[0]; state.centre = null; }
    state.selected[areaId] = treeId;
    if (fromList) state.overlap = null;
    resetReasonPicker();
    if (livingIn(areaId, treeId)) state.competitionTarget = { areaId, treeId };
    else clearCompetition();
    showPane('tree');
    renderAll();
    announce('Selected tree ' + treeId + ' in ' + areaName(areaId) + '.');
    if (state.competitionTarget) refreshCompetition();
  }

  async function onMapClick(event) {
    if (!state.view || refusedByLock('Map selection') || state.busy) return;
    const area = currentArea();
    const matrix = byId('map').getScreenCTM();
    if (!area || !matrix) return;
    const point = new DOMPoint(event.clientX, event.clientY).matrixTransform(matrix.inverse());
    state.busy = true;
    const outcome = await call('api/multi/hit', { fence: currentFence(), areaId: area.areaId, u: point.x, v: point.y });
    state.busy = false;
    if (outcome.kind === 'ok') {
      clearAlert();
      const candidates = outcome.payload.candidates;
      if (candidates.length === 0) {
        delete state.selected[area.areaId];
        state.overlap = null;
        clearCompetition();
        applyPayload(outcome.payload, false);
        announce('No crown at this point — nothing selected.');
        return;
      }
      state.overlap = candidates.length > 1 ? { areaId: area.areaId, candidates } : null;
      applyPayload(outcome.payload, true);
      chooseTree(area.areaId, candidates[0].treeId, false);
      if (candidates.length > 1) announce('Selected tree ' + candidates[0].treeId + '. ' + fill(label('CrownsOverlap'), { n: candidates.length }) + '.');
    } else if (outcome.kind === 'refused') {
      delete state.selected[area.areaId];
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
        byId('inspector-heading').focus();
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
    if (!treeId || !state.view) return;
    const marked = areaView(areaId).cropTreeIds.includes(treeId);
    run(marked ? 'api/multi/crop/unmark' : 'api/multi/crop/mark', { fence: currentFence(), areaId, treeId },
      label(marked ? 'MultiUnmarkCrop' : 'MultiMarkCrop') + ' (' + treeId + ')',
      () => announce((marked ? 'Removed Crop designation: ' : 'Retained Crop Tree: ') + treeId + ' in ' + areaName(areaId) + '.'));
  }

  function onAdd(event) {
    event.preventDefault();
    const areaId = state.areaId;
    const treeId = selectedId();
    if (treeId === null || refusedByLock('Add ' + treeId + ' to the Work Plan')) return;
    const reason = reasonToSend();
    run('api/multi/pending/add', { fence: currentFence(), areaId, treeId, reason }, 'Add ' + treeId + ' to the Work Plan', (payload) => {
      resetReasonPicker();
      announce('Done: ' + label('MultiAddToPlan') + ' (' + treeId + ' in ' + areaName(areaId) + ', reason: ' + reason + '). Total pending ' +
        payload.view.pending.length + '.');
    }, 'inspector-heading');
  }

  function removePending(areaId, treeId) {
    if (!areaId || !treeId) return;
    run('api/multi/pending/remove', { fence: currentFence(), areaId, treeId }, 'Remove ' + treeId + ' from the Work Plan',
      () => announce('Done: ' + label('MultiRemoveFromPlan') + ' (' + treeId + ').'), 'plan-heading');
  }

  function onApprove() {
    run('api/multi/plan/approve', { fence: currentFence() }, label('MultiApprove'), (payload) => {
      announce('Done: ' + label('MultiApprove') + '. Approved: ' + payload.view.approved.orders.map((o) => o.treeId).join(', ') +
        '. Living trees unchanged: ' + payload.view.areas.map((a) => a.label + ' ' + a.stand.livingCount).join(', ') + '.');
    }, 'plan-heading');
  }

  function onCancelApproved() {
    run('api/multi/plan/cancel', { fence: currentFence() }, label('MultiCancelApproved'),
      () => announce('Done: ' + label('MultiCancelApproved') + '. Its orders were not returned to pending.'), 'plan-heading');
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
    if (!view || !view.approved || refusedByLock(label('MultiExecute'))) return;
    const confirmation = confirmationOf(view);
    state.confirmation = confirmation;
    const n = confirmation.orders.length;
    const areas = new Set(confirmation.orders.map((order) => order.areaId)).size;
    byId('execute-title').textContent = label('MultiExecute');
    byId('execute-text').textContent = fill(label(n === 1 ? 'MultiExecuteConfirmationOneTree' : 'MultiExecuteConfirmation'),
      { n, a: areas, y: confirmation.year }) + '.';
    byId('execute-areas').replaceChildren(...view.areas.map((area) => {
      const orders = confirmation.orders.filter((order) => order.areaId === area.areaId);
      return el('section', { class: 'plan-area' }, el('h3', null, areaName(area.areaId) + ' — ' + orders.length),
        orders.length === 0 ? el('p', { class: 'note' }, 'No Fell in this area.') : orderList(orders, 'approved-orders', false));
    }));
    byId('execute-plan').replaceChildren(label('MultiLastLivingRefused') + '. ', technicalPlan(confirmation.planId, confirmation.executionKey));
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
      announce('Done: ' + label('MultiExecute') + '. ' + execution.event.areas.map((change) => change.areaId + ' living ' +
        change.before.livingCount + ' → ' + change.after.livingCount + ' (felled ' + (change.fells.map((f) => f.treeId).join(', ') || 'none') + ')').join('; ') + '.');
    }, 'receipt-heading');
    showPane('plan');
  }

  function sendAdvance(consent) {
    run('api/multi/year/advance', { fence: currentFence(), consentToCancelWork: consent }, label('MultiAdvanceYear'), (payload) => {
      const record = payload.view.annualHistory[payload.view.annualHistory.length - 1];
      announce('Done: year ' + record.yearBefore + ' → ' + record.yearAfter + '. ' + record.areas.map((year) => year.areaId + ' living ' +
        year.before.livingCount + ' → ' + year.after.livingCount + (year.noLivingStems ? ' (no living stems)' : ', natural deaths ' + year.naturalDeaths.length))
        .join('; ') + '.' + (record.cancelledUnexecutedWork ? ' Unexecuted work was cancelled at zero Fell.' : ''));
      showPane('history');
    }, 'year-heading');
  }

  function onAdvance() {
    const view = state.view;
    if (!view || refusedByLock(label('MultiAdvanceYear'))) return;
    if (view.pending.length > 0 || view.approved !== null) openConsentDialog();
    else sendAdvance(false);
  }

  function openConsentDialog() {
    const view = state.view;
    byId('consent-title').textContent = label('MultiAdvanceYear');
    byId('consent-text').textContent = 'The Work Plan has unexecuted work. Advancing the year needs your explicit choice; cancelling ' +
      'removes ALL pending and approved orders in BOTH areas and fells nothing.';
    byId('consent-areas').replaceChildren(...view.areas.map((area) => {
      const pending = ordersIn(view.pending, area.areaId).map((order) => order.treeId);
      const approved = view.approved ? ordersIn(view.approved.orders, area.areaId).map((order) => order.treeId) : [];
      return el('p', null, areaName(area.areaId) + ': ' + (pending.length ? 'pending ' + pending.join(', ') : 'no pending orders') + '; ' +
        (approved.length ? 'approved ' + approved.join(', ') : 'no approved orders') + '.');
    }));
    byId('consent-keep').textContent = fill(label('MultiConsentKeep'), { y: view.source.ecologicalYear });
    byId('consent-advance').textContent = label('MultiConsentCancel');
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
    byId('competition-content').addEventListener('click', (event) => {
      const button = event.target.closest('[data-competitor]');
      if (!button || state.lock || !state.competition) return;
      // Selecting a listed competitor keeps the inspected tree as the competition target, so a release reason can be stated.
      state.selected[state.competition.areaId] = button.getAttribute('data-competitor');
      resetReasonPicker();
      frameInspection();
      renderAll();
      showPane('tree');
      if (selectedId() === state.competition.report.targetTreeId) byId('inspector-heading').focus();
      announce('Selected ' + button.getAttribute('data-competitor') + '; competition target remains ' + state.competition.report.targetTreeId + '.');
    });
    byId('overlap-list').addEventListener('click', (event) => {
      const button = event.target.closest('[data-pick]');
      if (button) chooseTree(state.areaId, button.getAttribute('data-pick'), false);
    });
    byId('tree-rows').addEventListener('click', (event) => {
      const button = event.target.closest('[data-select]');
      if (button) {
        chooseTree(state.areaId, button.getAttribute('data-select'), true);
        byId('inspector-heading').focus();
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
      announce(label('MultiNothingChanged') + ' — ' + fill(label('MultiConsentKeep'), { y: state.view.source.ecologicalYear }) + '.');
    });
    byId('consent-advance').addEventListener('click', () => { byId('consent-dialog').close(); sendAdvance(true); });
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
  }

  drawCanopyDefs();
  wire();
  reread(false);
})();
""";
}
