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

<div class="notice" role="note" aria-label="About this experiment">
  <span class="notice-icon" aria-hidden="true">ⓘ</span>
  <p class="notice-text"><strong id="disclosure" class="notice-lead"></strong> <span id="flags" class="flags"></span></p>
  <button type="button" id="notice-details" class="link-button small-link">Details</button>
  <p id="session-note" class="session-note"></p>
</div>

<div class="messages">
  <div id="alert" class="alert" role="alert" tabindex="-1"></div>
</div>

<main id="workspace" class="workspace" data-legend="shown">
  <div class="workspace-head">
    <h2 id="map-heading" tabindex="-1">Stand map</h2>
    <p id="map-subtitle" class="map-subtitle"></p>
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
        <polygon id="area-outline" class="area-outline" points="0,0"></polygon>
        <g id="crowns"></g>
        <g id="marks" class="overlay"></g>
        <g id="selection" class="overlay"></g>
        <g id="furniture" class="overlay"></g>
      </svg>

      <div id="map-dock" class="map-dock" role="group" aria-labelledby="dock-title">
        <p class="dock-head"><span id="dock-title">Map navigation</span> <span id="zoom-state" class="zoom-state">Fit</span></p>
        <div class="dock-nav">
          <div class="dpad" role="group" aria-label="Pan the map">
            <button type="button" id="pan-north" class="icon" aria-label="Pan north" title="Pan north"><span aria-hidden="true">↑ N</span></button>
            <button type="button" id="pan-west" class="icon" aria-label="Pan west" title="Pan west"><span aria-hidden="true">W ←</span></button>
            <span class="dpad-centre" aria-hidden="true">⌖</span>
            <button type="button" id="pan-east" class="icon" aria-label="Pan east" title="Pan east"><span aria-hidden="true">→ E</span></button>
            <button type="button" id="pan-south" class="icon" aria-label="Pan south" title="Pan south"><span aria-hidden="true">S ↓</span></button>
          </div>
          <div class="zoom" role="group" aria-label="Zoom">
            <button type="button" id="zoom-in" class="icon" aria-label="Zoom in" title="Zoom in"><span aria-hidden="true">+</span></button>
            <button type="button" id="zoom-out" class="icon" aria-label="Zoom out" title="Zoom out"><span aria-hidden="true">−</span></button>
            <button type="button" id="zoom-fit" class="icon fit" aria-label="Fit whole area" title="Fit whole area">Fit</button>
          </div>
        </div>
        <div id="area-switch" class="dock-areas" role="group" aria-label="Choose the area shown on the map and in the tree table"></div>
        <p class="dock-note">Two separate management areas</p>
      </div>

      <div class="map-orient">
        <button type="button" id="legend-show" class="legend-show" aria-controls="legend-panel" aria-expanded="false" hidden>Show legend</button>
        <p class="north" aria-hidden="true"><span>N</span><svg viewBox="0 0 12 16" focusable="false"><path d="M6 0 12 16 6 12 0 16Z"></path></svg></p>
      </div>
      <p class="scale" aria-hidden="true"><svg id="scale-bar" class="scale-bar" width="80" height="9" viewBox="0 0 80 9" focusable="false"><path id="scale-path" d="M 1 1 V 8 H 79 V 1"></path></svg> <span id="scale-text"></span></p>
      <p id="map-status" class="map-status" hidden></p>
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
      <section class="panel tree-panel" aria-labelledby="inspector-heading">
        <div id="inspector-empty" class="inspector-empty">
          <h2 class="eyebrow-heading">Reference tree</h2>
          <p>No tree selected in this area. Click a crown on the map, use the Trees tab, or type an exact tree ID below.</p>
        </div>
        <div id="inspector-body" hidden>
          <div class="ref-card">
            <p class="ref-top"><span id="insp-role" class="eyebrow">Reference tree</span> <span id="insp-area" class="tag"></span></p>
            <div class="ref-title">
              <h2 id="inspector-heading" tabindex="-1"><span id="insp-id" class="mono"></span></h2>
              <span id="insp-chip" class="chip"></span>
            </div>
            <p id="insp-summary" class="ref-sub"></p>
            <dl id="insp-key" class="key-facts"></dl>
            <div class="ref-actions">
              <p id="insp-pinned" class="small-note"></p>
              <p class="ref-links">
                <button type="button" id="crop-button" class="link-button small-link"></button>
                <span id="add-reference-wrap" hidden><button type="button" id="add-reference" class="link-button small-link"></button></span>
              </p>
            </div>
            <p id="crop-note" class="small-note crop-note"></p>
            <p id="insp-remove-wrap" class="ref-extra" hidden><button type="button" id="insp-remove" class="secondary"></button></p>
            <p id="insp-approved-note" class="note small-note" hidden>This tree is in the approved plan. Cancel the approved plan to change it.</p>
          </div>

          <section id="competition-panel" class="competition" aria-labelledby="competition-heading">
            <h3 id="competition-heading">Choose competitors for Fell</h3>
            <p id="competition-sub" class="small-note"></p>
            <div id="competition-content"></div>
          </section>

          <div class="add-dock">
          <div id="batch-result" class="batch-result" tabindex="-1" hidden></div>
          <form id="add-form" class="add-form" novalidate hidden>
            <p class="draft-line"><span id="draft-count" class="draft-count"></span>
              <button type="button" id="draft-clear" class="link-button small-link">Clear selection</button></p>
            <p><button type="submit" id="add-button" class="add-button"></button></p>
            <p id="add-caption" class="small-note">Queues proposals only. Approve and Execute are separate.</p>
            <details id="reason-details" class="reason-details">
              <summary>Reason: <span id="reason-summary"></span> · optional note</summary>
              <p id="release-wrap" class="release-option" hidden>
                <label><input type="checkbox" id="reason-release"><span id="release-label"></span></label>
              </p>
              <label for="reason">Optional note recorded with each Fell order added here</label>
              <textarea id="reason" rows="2" maxlength="2000"></textarea>
              <p id="reason-help" class="small-note">Note takes precedence; otherwise the ticked release intent, otherwise Owner-selected felling.</p>
              <p id="reason-error" class="field-error" role="alert" hidden></p>
              <p id="reason-preview" class="small-note" aria-live="polite"></p>
            </details>
          </form>
          </div>

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
        <p class="note small-note">Light, seed, regeneration, site, money and saving: NOT MODELLED. Each area's ecology uses only its own trees.</p>
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
  --exterior-1: #e4ecf2; --exterior-2: #cbd8e3; --exterior-ink: #2a4157; --interior: #263b49;
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

.notice { display: flex; flex-wrap: wrap; align-items: center; gap: .1rem .8rem; margin: var(--gap) var(--gutter) 0; padding: .3rem 1rem; border: 1px solid var(--notice-line); border-radius: 8px; background: var(--notice-bg); font-size: .8rem; }
.notice-icon { color: var(--notice-text); }
.notice-text { flex: 1 1 24rem; min-width: 0; margin: 0; color: var(--text); }
.notice-lead { color: var(--notice-text); }
.flags { color: var(--text); }
.flags abbr { text-decoration: none; }
.session-note { margin: 0 0 0 auto; color: var(--text); }
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
.map-dock { position: absolute; left: 14px; top: 14px; z-index: 3; width: 212px; padding: .55rem .7rem .5rem; border: 1px solid var(--edge); border-radius: 8px; background: rgba(21, 36, 56, .97); box-shadow: 0 6px 18px rgba(8, 20, 34, .35); }
.dock-head { display: flex; justify-content: space-between; margin: 0 0 .45rem; color: var(--muted); font-size: .64rem; font-weight: 600; letter-spacing: .1em; text-transform: uppercase; }
.zoom-state { color: var(--text); letter-spacing: .04em; }
.dock-nav { display: flex; justify-content: space-between; gap: 8px; }
.dpad { display: grid; grid-template-columns: repeat(3, 44px); grid-template-rows: repeat(3, 44px); gap: 2px; }
.dpad .dpad-centre { grid-column: 2; grid-row: 2; display: grid; place-items: center; color: var(--faint); }
#pan-north { grid-column: 2; grid-row: 1; }
#pan-west { grid-column: 1; grid-row: 2; }
#pan-east { grid-column: 3; grid-row: 2; }
#pan-south { grid-column: 2; grid-row: 3; }
.zoom { display: grid; grid-template-rows: repeat(3, 44px); gap: 2px; padding-left: 8px; border-left: 1px solid var(--edge); }
button.icon { display: grid; place-items: center; width: 44px; height: 44px; padding: 0; border-color: var(--line); background: var(--panel-2); font-size: .78rem; line-height: 1; }
.zoom button.icon { font-size: 1.15rem; }
.zoom button.fit { font-size: .8rem; }
.map-dock button.icon:disabled { border-style: solid; border-color: #2b3d52; background: transparent; color: #5f7590; }
.dock-areas { display: grid; grid-template-columns: 1fr 1fr; gap: 6px; margin-top: .55rem; padding-top: .55rem; border-top: 1px solid var(--edge); }
.area-tab { display: grid; justify-items: center; gap: .15rem; min-height: 68px; padding: .35rem .2rem; border-color: var(--line); background: var(--panel-2); font-weight: 500; font-size: .82rem; }
.area-tab[aria-pressed="true"] { border-color: var(--blue-edge); background: var(--blue); font-weight: 650; }
.area-thumb { width: 44px; height: 26px; }
.area-thumb polygon { fill: #6f8fb0; stroke: #dbe9f6; stroke-width: 1.2; vector-effect: non-scaling-stroke; }
.area-tab[aria-pressed="true"] .area-thumb polygon { fill: #a9cdef; stroke: #fff; }
.area-plan { color: var(--pending); font-size: .7rem; }
.dock-note { margin: .4rem 0 0; color: var(--muted); font-size: .68rem; text-align: center; }
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
.grid-line { stroke: #9fb3c5; stroke-width: 1; vector-effect: non-scaling-stroke; stroke-opacity: .55; }
#grid-in .grid-line { stroke: #ffffff; stroke-opacity: .06; }
.grid-label { fill: #4b6177; font-size: .9px; paint-order: stroke; stroke: var(--exterior-1); stroke-width: .25px; stroke-linejoin: round; }
.area-outline { fill: none; stroke: #20384c; stroke-width: 1.4; stroke-dasharray: 6 4; vector-effect: non-scaling-stroke; pointer-events: none; }
.tree .crown { fill: #000; fill-opacity: 0; stroke: none; pointer-events: fill; cursor: pointer; }
.deco, .deco * { pointer-events: none; }
.tree.dimmed { opacity: .72; }
.stump-disc { fill: var(--stump); stroke: #6e5a3c; stroke-width: .035; }
.stump-ring { fill: none; stroke: #8a7350; stroke-width: .03; }

/* Status marks: ring widths are screen pixels (non-scaling) so they read the same at every zoom level. */
.ring { fill: none; vector-effect: non-scaling-stroke; }
.ring-halo { stroke: #06111d; stroke-opacity: .55; stroke-width: 5; }
.ring-crop { stroke: var(--crop); stroke-width: 2.4; }
.ring-reference { stroke: var(--reference); stroke-width: 2.2; stroke-dasharray: 3 3; }
.ring-inspect { stroke: var(--reference); stroke-width: 3; }
.ring-competitor { stroke: #e3d3a4; stroke-width: 1.6; stroke-dasharray: 1.5 3; stroke-linecap: round; }
.ring-draft { stroke: var(--draft); stroke-width: 2.6; }
.ring-pending { stroke: var(--pending); stroke-width: 2.4; stroke-dasharray: 7 4; }
.ring-approved { stroke: var(--approved); stroke-width: 2; }
.badge-bg { stroke-width: 1.3; vector-effect: non-scaling-stroke; }
.badge-text { font-weight: 800; text-anchor: middle; dominant-baseline: central; }
.b-crop .badge-bg { fill: #0f2133; stroke: var(--crop); } .b-crop .badge-text { fill: var(--crop); }
.b-reference .badge-bg { fill: #0f2133; stroke: #fff; } .b-reference .badge-text { fill: #fff; }
.b-pending .badge-bg { fill: #2a2312; stroke: var(--pending); } .b-pending .badge-text { fill: var(--pending); }
.b-approved .badge-bg { fill: #0f2133; stroke: var(--approved); } .b-approved .badge-text { fill: var(--approved); }
.b-dead .badge-bg, .b-executed .badge-bg { fill: #1b2229; stroke: #e5e1d6; } .b-dead .badge-text, .b-executed .badge-text { fill: #eeeadd; }
.b-rank .badge-bg { fill: #10202f; stroke: #e3d3a4; } .b-rank .badge-text { fill: #f1e2b6; }
.b-rank.chosen .badge-bg { fill: var(--draft); stroke: #fff3e6; } .b-rank.chosen .badge-text { fill: #2a1606; }
.tick { fill: none; stroke: #fff6ec; stroke-width: 2.6; stroke-linecap: round; stroke-linejoin: round; vector-effect: non-scaling-stroke; paint-order: stroke; }
.tick-shadow { fill: none; stroke: #2a1606; stroke-opacity: .6; stroke-width: 5; stroke-linecap: round; stroke-linejoin: round; vector-effect: non-scaling-stroke; }
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
.legend .legend-exterior { fill: #d6e1ea; }
.legend .legend-area { fill: var(--interior); stroke: #20384c; stroke-width: 1.4; stroke-dasharray: 3 2.5; vector-effect: non-scaling-stroke; }

/* ---- side panel: reference tree and competitors ------------------------------------------------------------------- */
.tree-panel { padding: 0; overflow: visible; }
.inspector-empty { padding: .9rem 1rem; }
.ref-card { margin: 0; padding: .9rem 1rem .7rem; border-bottom: 1px solid var(--edge); }
.ref-top { display: flex; align-items: center; justify-content: space-between; gap: .5rem; margin: 0 0 .4rem; }
.ref-title { display: flex; flex-wrap: wrap; align-items: center; justify-content: space-between; gap: .4rem; }
.ref-title h2 { margin: 0; font-size: 1.55rem; font-weight: 650; letter-spacing: -.01em; }
.chip { display: inline-flex; align-items: center; padding: .15rem .55rem; border: 1px solid var(--line); border-radius: 4px; background: var(--panel-2); font-size: .76rem; font-weight: 600; }
.chip.crop { border-color: var(--crop); color: var(--crop); }
.chip.s-PendingFell { border-color: var(--pending); color: var(--pending); }
.chip.s-ApprovedFell { border-color: var(--approved); color: var(--approved); }
.chip.s-Felled { border-color: var(--stump); color: var(--stump); }
.chip.s-NaturalDead { border-color: var(--dead); color: var(--dead); }
.ref-sub { margin: .1rem 0 .55rem; color: var(--muted); font-size: .86rem; }
.key-facts { display: grid; grid-template-columns: repeat(4, minmax(0, 1fr)); gap: .3rem .6rem; margin: 0; }
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
.competition h3 { margin: 0; }
.competition .small-note { margin: .15rem 0 .45rem; }
.comp-head { display: grid; grid-template-columns: 2.2rem minmax(0, 1fr) 3.5rem 3.3rem; gap: .35rem; padding: 0 .55rem .2rem; color: var(--muted); font-size: .68rem; }
.comp-head span:nth-child(n+3) { text-align: right; }
.comp-rows { display: grid; gap: .4rem; margin: 0; padding: 0; list-style: none; }
.comp-row label { display: grid; grid-template-columns: 2.2rem minmax(0, 1fr) 3.5rem 3.3rem; align-items: center; gap: .35rem; min-height: 50px; margin: 0; padding: .3rem .55rem; border: 1px solid var(--line); border-radius: 6px; background: var(--panel-2); color: var(--text); font-size: .9rem; cursor: pointer; }
.comp-row label:hover { border-color: var(--faint); }
.comp-row.chosen label { border-color: var(--gold); background: #2c2a23; }
.comp-row.blocked label { border-style: dashed; background: var(--panel); cursor: not-allowed; }
.comp-row input { width: 22px; height: 22px; min-height: 0; margin: 0 auto; accent-color: var(--gold); cursor: inherit; }
.comp-id { display: block; font-weight: 650; overflow-wrap: anywhere; }
.comp-id .rank { margin-right: .35rem; color: var(--gold-edge); font-style: italic; font-weight: 700; }
.comp-meta { display: block; color: var(--muted); font-size: .72rem; }
.comp-why { display: block; color: var(--pending); font-size: .72rem; font-weight: 600; }
.comp-num { text-align: right; font-variant-numeric: tabular-nums; font-size: .84rem; }
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
.reason-details { margin-top: .35rem; border: 1px solid var(--line); border-radius: 6px; background: var(--page); }
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
  .comp-head, .comp-row label { grid-template-columns: 2.2rem minmax(0, 1fr) 3.8rem 3.3rem; gap: .3rem; }
  .comp-row label { min-height: 48px; padding: .25rem .45rem; font-size: .84rem; }
  .map-dock { left: 10px; top: 10px; }
}
@media (min-width: 1024px) and (max-height: 820px) {
  .topbar { padding-top: .4rem; padding-bottom: .4rem; }
  .btn-sub { font-size: .7rem; }
  .dock-note { display: none; }
  .area-tab { min-height: 56px; }
  .crop-note { display: none; }
  .competition .small-note { margin-bottom: .3rem; }
  #insp-pinned { display: none; }
  #add-caption { display: none; }
  .ref-top { margin-bottom: .2rem; }
  .ref-title h2 { font-size: 1.2rem; }
  .key-facts dd { font-size: .92rem; }
  .ref-links .link-button { min-height: 32px; }
  .add-button { min-height: 44px; }
  .reason-details summary { min-height: 36px; padding: .45rem .6rem; }
  .comp-row label { min-height: 46px; }
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
    'execute-confirm', 'advance', 'consent-advance'];
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
    if (payload.labels) state.labels = payload.labels;
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
    for (const button of document.querySelectorAll('[data-remove]')) button.disabled = blocked;
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
    renderOverlap();
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
        el('span', null, 'm²/ha basal area · ' + fixed(area.areaHa, 4) + ' ha')))));
    const approved = view.approved ? view.approved.orders.length : 0;
    const drafts = draftTotal();
    byId('plan-summary').textContent = approved > 0 ? plural(approved, 'approved Fell', 'approved Fell') :
      view.pending.length > 0 ? view.pending.length + ' pending Fell' : 'No queued work';
    byId('plan-summary-sub').textContent = [approved > 0 ? 'still standing' : view.pending.length > 0 ? 'not approved' : '',
      drafts > 0 ? plural(drafts, 'draft choice', 'draft choices') : ''].filter(Boolean).join(' · ');
    byId('disclosure').textContent = label('MultiDisclosure');
    const chips = [];
    CHIPS.forEach(([chip, full], index) => {
      if (index > 0) chips.push(' · ');
      chips.push(el('abbr', { title: label(full) }, label(chip)));
    });
    byId('flags').replaceChildren(...chips);
    byId('session-note').replaceChildren(el('abbr', { title: label('MultiSessionOnly') }, label('MultiChipSessionOnly')));
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
      const inPlan = ordersIn(view.pending, area.areaId).length + (view.approved ? ordersIn(view.approved.orders, area.areaId).length : 0);
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
    const marks = [];
    const items = new Map(sceneOf(area.areaId).map((item) => [item.treeId, item]));
    for (const item of sceneOf(area.areaId).slice().sort((a, b) => a.paintOrder - b.paintOrder)) {
      const dimmed = focusIds && !focusIds.has(item.treeId);
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
      crowns.push(cached.node);
      marks.push(...statusMarks(item, mpp));
    }
    if (report) {
      report.topFive.forEach((entry, rank) => {
        const item = items.get(entry.treeId);
        if (item) marks.push(...competitorMarks(item, rank + 1, draft.has(entry.treeId), mpp));
      });
    }
    const host = byId('crowns');
    const unchanged = host.childNodes.length === crowns.length && crowns.every((node, index) => host.childNodes[index] === node);
    if (!unchanged) host.replaceChildren(...crowns);
    byId('marks').replaceChildren(...marks);
    const chosen = selectedId() ? items.get(selectedId()) : null;
    const lit = chosen && !state.lock ? (chosen.treeId === reference ? referenceMarks(chosen, mpp, true) : inspectMarks(chosen, mpp, true)) : [];
    byId('selection').replaceChildren(...lit);
    byId('map-desc').textContent = 'North-up map of ' + areaName(area.areaId) + ', ' + area.areaM2Exact + ' square metres. ' +
      'Ecological year ' + state.view.source.ecologicalYear + '. Living trees ' + area.stand.livingCount + '. ' +
      (selectedId() ? 'Selected tree ' + selectedId() + '. ' : '') + 'Use the Trees tab to select a tree by keyboard.';
    byId('map-subtitle').textContent = area.label + ' · ' + fixed(area.areaHa, 4) + ' ha';
    byId('map-note').textContent = areaName(area.areaId) + ' · ' + (state.zoom === 1 ? 'Fit whole area' :
      fixed(state.zoom, 2) + '× Fit · view ' + fixed(box.uw, 1) + ' × ' + fixed(box.uh, 1) + ' m') +
      ' · site ' + area.site + ' · neighbours across area edges excluded';
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
    if (state.batch) text = 'Adding ' + (state.batch.confirmed.length + 1) + ' of ' + state.batch.ids.length + ' to the Fell plan…';
    else if (view.approved) text = plural(view.approved.orders.length, 'approved Fell', 'approved Fell') + ' · still standing';
    else if (reference) text = 'Reference ' + reference + ' · ' + plural(drafts, 'draft choice', 'draft choices');
    box.hidden = text === '';
    box.textContent = text;
  }

  // Original procedural Sitka spruce studies for this page, top-down: a dark crown mass, four tiers of whorled boughs lit
  // from the north-west, feathered needle sprays and a pale leader. Six seeded variants are drawn once into shared symbols
  // at unit radius; each tree reuses one, scaled to its true server crown radius and turned by its own ID seed. Shape and
  // tint are drawing only: no age, health, genetics or habitat value is inferred from them.
  const SPRUCE_TIERS = ['#315657', '#3f6967', '#507b75', '#6a9185', '#8da99a'];
  const SPRUCE_VARIANTS = 6;
  const SLATE = [38, 59, 73];
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

  function spruceBranches(variant) {
    const seed = canopyHash(885, 16, variant + 7);
    let channel = 100;
    const random = () => canopyRandom(seed, channel++);
    const mass = [];
    for (let k = 0; k < 48; k++) {
      const a = k * Math.PI / 24, r = (23 + (random() - .5) * 5) / 29;
      mass.push(n3(Math.cos(a) * r) + ',' + n3(Math.sin(a) * r));
    }
    const branches = [];
    for (let tier = 0; tier < 4; tier++) {
      const count = 11 - tier, reach = (29 - tier * 5.8) / 29, phase = variant * .51 + tier * .57;
      for (let b = 0; b < count; b++) {
        const a = b * Math.PI * 2 / count + phase + (random() - .5) * .22;
        const r = reach * (.84 + random() * .16);
        const sprays = [];
        for (let step = 1; step <= 6; step++) {
          for (const side of [-1, 1]) sprays.push({ t: .16 + step * .11, side, length: .29 + random() * .11 });
        }
        branches.push({ tier, a, r, sprays });
      }
    }
    return { mass, branches };
  }

  // Paths of one colour within one tier are merged, so each shared symbol stays about thirty elements however many boughs
  // and needles it draws; tiers still paint bottom to top.
  function drawSpruce(variant, plan, lite) {
    const g = svg('g', { id: 'spruce-' + (lite ? 'lite-' : 'full-') + variant });
    g.append(svg('polygon', { points: plan.mass.join(' '), fill: 'url(#spruce-mass-' + variant + ')' }));
    let axes = '';
    for (let tier = 0; tier < 4; tier++) {
      const boughs = ['', ''], twigs = ['', ''], lights = ['', ''];
      let dark = '';
      for (const branch of plan.branches) {
        if (branch.tier !== tier) continue;
        const ux = Math.cos(branch.a), uy = Math.sin(branch.a), nx = -uy, ny = ux;
        const r = branch.r, lit = ux + uy < 0 ? 1 : 0;
        const wide = (1.7 - tier * .2) / 29, bow = 2 / 29;
        boughs[lit] += 'M' + n3(nx * wide) + ' ' + n3(ny * wide) + 'Q' + n3(ux * r * .52 + nx * bow) + ' ' +
          n3(uy * r * .52 + ny * bow) + ' ' + n3(ux * r) + ' ' + n3(uy * r) + 'Q' + n3(ux * r * .52 - nx * bow) + ' ' +
          n3(uy * r * .52 - ny * bow) + ' ' + n3(-nx * wide) + ' ' + n3(-ny * wide) + 'Z';
        axes += 'M0 0L' + n3(ux * r) + ' ' + n3(uy * r);
        if (lite) continue;
        for (const spray of branch.sprays) {
          const bx = ux * r * spray.t, by = uy * r * spray.t;
          const length = (1 - spray.t) * r * spray.length;
          const tx = bx + nx * spray.side * length + ux * length * .52, ty = by + ny * spray.side * length + uy * length * .52;
          twigs[lit] += 'M' + n3(bx) + ' ' + n3(by) + 'L' + n3(tx) + ' ' + n3(ty);
          const dx = tx - bx, dy = ty - by, dl = Math.hypot(dx, dy) || 1, vx = dx / dl, vy = dy / dl;
          for (let n = 1; n < 5; n++) {
            const z = n / 5, px = bx + dx * z, py = by + dy * z;
            const needle = (.6 + (1 - z) * 1.15) * (1 - tier * .08) / 29;
            lights[lit] += 'M' + n3(px) + ' ' + n3(py) + 'L' + n3(px + vx * needle * .8 - vy * needle) + ' ' + n3(py + vy * needle * .8 + vx * needle);
            dark += 'M' + n3(px) + ' ' + n3(py) + 'L' + n3(px + vx * needle * .8 + vy * needle) + ' ' + n3(py + vy * needle * .8 - vx * needle);
          }
        }
      }
      [0, 1].forEach((lit) => {
        if (boughs[lit]) g.append(svg('path', { d: boughs[lit], fill: SPRUCE_TIERS[Math.min(3, tier + lit)], opacity: .94 }));
      });
      if (lite) continue;
      const stroke = (d, colour, width, opacity) => d && g.append(svg('path', { d, fill: 'none', stroke: colour, 'stroke-width': width,
        opacity, 'stroke-linecap': 'round' }));
      [0, 1].forEach((lit) => stroke(twigs[lit], SPRUCE_TIERS[Math.min(4, 1 + lit)], .019, .8));
      stroke(dark, '#173540', .021, .9);
      [0, 1].forEach((lit) => stroke(lights[lit], SPRUCE_TIERS[Math.min(4, 2 + lit + (tier > 1 ? 1 : 0))], .015, .72));
    }
    g.append(svg('path', { d: axes, stroke: '#9db29d', 'stroke-width': .011, 'stroke-opacity': .22 }),
      svg('circle', { cx: -.021, cy: -.028, r: .036, fill: '#b4c6b3' }));
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
    for (let variant = 0; variant < SPRUCE_VARIANTS; variant++) {
      const mass = svg('radialGradient', { id: 'spruce-mass-' + variant, cx: '.33', cy: '.28', r: '.78' });
      mass.append(svg('stop', { offset: 0, 'stop-color': '#628680' }), svg('stop', { offset: '.42', 'stop-color': '#30575a' }),
        svg('stop', { offset: 1, 'stop-color': '#0f2530' }));
      defs.append(mass);
      const plan = spruceBranches(variant);
      defs.append(drawSpruce(variant, plan, false), drawSpruce(variant, plan, true));
      const seed = canopyHash(977, 16, variant + 7);
      const dead = svg('g', { id: 'bare-branches-' + variant, opacity: .9 });
      let branches = '';
      for (let k = 0; k < 9; k++) {
        const angle = k * Math.PI * 2 / 9 + variant;
        const x = Math.cos(angle) * (.64 + .14 * canopyRandom(seed, k + 20));
        const y = Math.sin(angle) * (.64 + .14 * canopyRandom(seed, k + 20));
        branches += 'M 0 0 L ' + n3(x) + ' ' + n3(y) + ' M ' + n3(x * .55) + ' ' + n3(y * .55) +
          ' l ' + n3(-y * .19) + ' ' + n3(x * .19) + ' M ' + n3(x * .72) + ' ' + n3(y * .72) +
          ' l ' + n3(y * .15) + ' ' + n3(-x * .15) + ' ';
      }
      dead.append(svg('path', { d: branches, fill: 'none', stroke: '#b9bfbd', 'stroke-width': .05, 'stroke-linecap': 'round' }));
      defs.append(dead);
    }
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
    const id = dead ? 'bare-branches-' + (seed % 6) : 'spruce-' + (lite ? 'lite-' : 'full-') + variant;
    return [svg('circle', { class: 'deco', cx: n3(u + r * .18), cy: n3(v + r * .22), r: n3(r * 1.12),
      fill: 'url(#canopy-shadow)', opacity: dead ? .4 : .9 }),
      svg('use', { class: 'deco', href: '#' + id,
        transform: 'translate(' + n3(u) + ' ' + n3(v) + ') rotate(' + turn + ') scale(' +
          n3(r * Math.min(1, stretch)) + ' ' + n3(r * Math.min(1, 1 / stretch)) + ')' })];
  }

  // ---- status marks: shared by the map and the legend; sizes in CSS pixels via mpp (metres per pixel) ---------------

  function ring(item, cls, gapPx, mpp) {
    return svg('circle', { class: 'ring ' + cls, cx: n3(item.u), cy: n3(item.v), r: n3(item.radiusM + gapPx * mpp) });
  }

  function badge(item, angleDeg, text, kind, mpp, extra) {
    const angle = angleDeg * Math.PI / 180;
    const distance = item.radiusM + 2 * mpp;
    const cx = item.u + Math.cos(angle) * distance, cy = item.v + Math.sin(angle) * distance;
    const g = svg('g', { class: 'badge b-' + kind + (extra ? ' ' + extra : '') });
    g.append(svg('circle', { class: 'badge-bg', cx: n3(cx), cy: n3(cy), r: n3(8.5 * mpp) }),
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

  function tick(item, mpp) {
    const p = (dx, dy) => n3(item.u + dx * mpp) + ' ' + n3(item.v + dy * mpp);
    const d = 'M ' + p(-5, 0) + ' L ' + p(-1.5, 3.8) + ' L ' + p(5.5, -4.5);
    return [svg('path', { class: 'tick-shadow', d }), svg('path', { class: 'tick', d })];
  }

  function statusMarks(item, mpp) {
    const marks = [];
    if (item.isCrop) marks.push(ring(item, 'ring-crop', 3, mpp), badge(item, 45, 'C', 'crop', mpp));
    if (item.status === 'PendingFell') {
      marks.push(ring(item, 'ring-pending', 3, mpp), badge(item, 45, 'P', 'pending', mpp));
    } else if (item.status === 'ApprovedFell') {
      marks.push(ring(item, 'ring-approved', 2.5, mpp), ring(item, 'ring-approved', 6.5, mpp), badge(item, 45, 'A', 'approved', mpp));
    } else if (item.status === 'NaturalDead') {
      marks.push(badge(item, 45, 'D', 'dead', mpp));
    } else if (item.status === 'Felled') {
      const d = Math.min(item.radiusM * .45, 6 * mpp);
      marks.push(svg('path', { class: 'stump-x', d: 'M ' + n3(item.u - d) + ' ' + n3(item.v - d) + ' L ' + n3(item.u + d) + ' ' +
        n3(item.v + d) + ' M ' + n3(item.u - d) + ' ' + n3(item.v + d) + ' L ' + n3(item.u + d) + ' ' + n3(item.v - d) }),
        badge(item, 45, 'X', 'executed', mpp));
    }
    return marks;
  }

  function competitorMarks(item, rank, chosen, mpp) {
    const marks = [ring(item, chosen ? 'ring-draft' : 'ring-competitor', 4.5, mpp)];
    if (chosen) marks.push(...tick(item, mpp));
    marks.push(badge(item, -50, String(rank), 'rank', mpp, chosen ? 'chosen' : null));
    return marks;
  }

  function referenceMarks(item, mpp, withTag) {
    const marks = [ring(item, 'ring-halo', 8.5, mpp), ring(item, 'ring-reference', 8.5, mpp), badge(item, 135, 'R', 'reference', mpp)];
    if (withTag) marks.push(tag(item.u, item.v - item.radiusM - 22 * mpp, 'Reference' + (item.isCrop ? ' · Crop' : ''), mpp));
    return marks;
  }

  function inspectMarks(item, mpp, withTag) {
    const marks = [ring(item, 'ring-halo', 7.5, mpp), ring(item, 'ring-inspect', 7.5, mpp)];
    if (withTag) marks.push(tag(item.u, item.v - item.radiusM - 22 * mpp, 'Selected', mpp));
    return marks;
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
      ['Inspected tree', 'Solid white selection halo', (item, mpp) => [...legendTree(item, 'Living'), ...inspectMarks(item, mpp, false)]],
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
    chip.hidden = !item.isCrop && item.status === 'Living';
    byId('insp-summary').textContent = 'Sitka spruce · ' + baseStatusText(item);
    byId('insp-key').replaceChildren(
      keyFact('DBH', fixed(tree.dbhCm, 1), 'cm'),
      keyFact('Height', fixed(tree.heightM, 1), 'm'),
      keyFact('Crown radius', fixed(tree.crownRadiusM, 2), 'm'),
      keyFact('Age', String(tree.ageYears), 'years'));
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
      sub = plural(report.neighbours.length, 'living neighbour', 'living neighbours') + ' within ' + report.cutoffM + ' m · strongest ' +
        report.topFive.length;
      if (report.topFive.length === 0) {
        parts.push(el('p', { class: 'comp-empty' }, 'No living competitor inside the ' + report.cutoffM + ' m cutoff.'));
      } else {
        parts.push(el('p', { class: 'comp-head', 'aria-hidden': 'true' }, el('span', null, ''), el('span', null, 'Tree / DBH / CI share'),
          el('span', null, 'Distance'), el('span', null, 'CI term')));
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
                  el('span', { class: 'comp-meta' }, fixed(n.dbhCm, 1) + ' cm · ' + fixed(n.sharePercent, 1) + '% · ' + n.strength),
                  why ? el('span', { class: 'comp-why' }, why) : null),
                el('span', { class: 'comp-num' }, fixed(n.distanceM, 2) + ' m'),
                el('span', { class: 'comp-num' }, fixed(n.contribution, 4))));
          })));
      }
      more.push(el('p', { class: 'competition-totals' }, 'Current total Hegyi CI ' + fixed(report.currentTotal, 4) +
        (report.targetPlannedForFell ? ' · This tree is planned for Fell: no release estimate.' :
          ' → ' + fixed(report.hypotheticalTotal, 4) + ' if its pending or approved neighbours were felled.')));
      more.push(el('p', { class: 'small-note' }, 'The planned figure counts only queued or approved orders, never unsent draft choices.'));
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
    if (!reference || state.batch || state.lock || !currentReport()) return;
    if (on && blockedReason(areaId, treeId)) return;
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

  function perAreaCounts(orders) {
    return state.view.areas.map((area) => area.label + ' ' + ordersIn(orders, area.areaId).length).join(' · ');
  }

  function renderPlan() {
    const view = state.view;
    const plan = view.approved;
    byId('plan-heading').textContent = plan ? 'Approved work plan' : 'Work plan';
    byId('step-pending').className = !plan && view.pending.length > 0 ? 'now' : '';
    byId('step-approved').className = plan ? 'now' : '';
    byId('plan-scope').textContent = label('MultiOnePlan') + '. ' + label('MultiLastLivingRefused') + '.';
    byId('pending-box').hidden = plan !== null && view.pending.length === 0;
    byId('pending-total').textContent = view.pending.length === 0 ? 'No pending Fell' :
      view.pending.length + ' pending Fell · not felled (' + perAreaCounts(view.pending) + ')';
    byId('pending-areas').replaceChildren(...view.areas.map((area) => {
      const orders = ordersIn(view.pending, area.areaId);
      return el('section', { class: 'plan-area', 'aria-label': 'Pending in ' + areaName(area.areaId) },
        el('h4', null, areaName(area.areaId) + ' · ' + plural(orders.length, 'pending order', 'pending orders')),
        orders.length === 0 ? el('p', { class: 'small-note' }, 'No pending Fell in this area.') : orderList(orders, 'pending-orders', true));
    }));
    const approve = byId('approve');
    approve.textContent = label('MultiApprove') + ' · ' + plural(view.pending.length, 'order', 'orders');
    approve.hidden = view.pending.length === 0 || plan !== null;
    byId('approved-empty').hidden = plan !== null;
    byId('approved-box').hidden = plan === null;
    if (plan === null) {
      byId('approved-areas').replaceChildren();
      byId('approved-total').textContent = '';
      return;
    }
    byId('approved-total').textContent = plan.orders.length + ' approved · still standing';
    byId('approved-sub').textContent = 'Execution has not happened. Year remains ' + view.source.ecologicalYear + '. ' +
      perAreaCounts(plan.orders) + ' · approved in year ' + plan.source.ecologicalYear + '.';
    byId('approved-areas').replaceChildren(...view.areas.map((area) => {
      const orders = ordersIn(plan.orders, area.areaId);
      return el('section', { class: 'plan-area', 'aria-label': 'Approved in ' + areaName(area.areaId) },
        el('h4', null, areaName(area.areaId) + ' · ' + (orders.length === 0 ? 'no orders' : plural(orders.length, 'order', 'orders'))),
        orders.length === 0 ? null : orderList(orders, 'approved-orders', false));
    }), technicalPlan(plan.planId, plan.executionKey));
    byId('cancel-approved').textContent = label('MultiCancelApproved');
    byId('execute').textContent = label('MultiExecute') + ' · ' + plural(plan.orders.length, 'order', 'orders');
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
    byId('statements').replaceChildren(...['MultiDisclosure'].concat(STATEMENTS, ['MultiReviewStatus', 'MultiEmptyAreaRule'])
      .map((name) => label(name)).filter((text) => text !== '').map((text) => el('li', null, text)));
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
    const counts = { plan: view.pending.length + (view.approved ? view.approved.orders.length : 0), history: view.annualHistory.length };
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
    announce('Selected tree ' + treeId + ' in ' + areaName(areaId) + '.' + cleared);
    if (state.competitionTarget) refreshCompetition();
  }

  async function onMapClick(event) {
    if (state.batch) { waitForBatch(); return; }
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
        // An empty click keeps the reference and its draft: only a deliberate choice of another tree replaces them.
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

  function onApprove() {
    run('api/multi/plan/approve', { fence: currentFence() }, label('MultiApprove'), (payload) => {
      announce('Done: ' + label('MultiApprove') + '. Approved: ' + payload.view.approved.orders.map((o) => o.treeId).join(', ') +
        '. Nothing was felled. Living trees unchanged: ' + payload.view.areas.map((a) => a.label + ' ' + a.stand.livingCount).join(', ') + '.');
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
      announce('Done: ' + label('MultiExecute') + '. ' + execution.event.areas.map((change) => areaName(change.areaId) + ' living ' +
        change.before.livingCount + ' → ' + change.after.livingCount + ' (felled ' + (change.fells.map((f) => f.treeId).join(', ') || 'none') + ')').join('; ') + '.');
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
    const work = view.pending.length > 0 || view.approved !== null;
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
      const pending = ordersIn(view.pending, area.areaId).map((order) => order.treeId);
      const approved = view.approved ? ordersIn(view.approved.orders, area.areaId).map((order) => order.treeId) : [];
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
    byId('notice-details').addEventListener('click', () => { showPane('details'); byId('details-heading').focus(); });
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
