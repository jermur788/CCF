using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using SitkaP1.Core;

namespace SitkaP1.Ui;

/// <summary>
/// The request guards and the thin JSON endpoints of the UI-A presentation process (spec MULTI-STANDMAP-PLAYABLE-2D-V1
/// §3.6, plan P3 step 3). Each endpoint is one call into <see cref="PlayerWorkPlanAdapter"/>. This class keeps no tree,
/// Work Plan, revision or session state of its own: every response carries the adapter's own view, plus the pure map
/// scene that Core builds from that same view, so the page can only draw what the engine committed.
/// </summary>
/// <remarks>
/// Status codes: 200 the read succeeded or the command committed; 409 the engine refused, nothing changed and a fresh view
/// is attached; 400, 403, 404, 405, 413 and 415 a guard refused the request before the adapter was called; 500 the outcome
/// is unconfirmed, so the page must re-read. Guard order: Host, capability, route and method, Origin, then for a POST the
/// content type, the body size and the JSON shape.
/// </remarks>
internal sealed class PlayerApi
{
    /// <summary>The largest request body accepted (spec §3.6 item 3).</summary>
    internal const long MaxBodyBytes = 16 * 1024;

    private const string JsonContentType = "application/json; charset=utf-8";

    // The page is self-contained: its own script, style and API only. No inline code, frames, forms or other origins.
    private const string PagePolicy = "default-src 'none'; script-src 'self'; style-src 'self'; connect-src 'self'; " +
        "base-uri 'none'; form-action 'none'; frame-ancestors 'none'";

    private static readonly byte[] PageHtml = EmbeddedAsset("index.html");
    private static readonly byte[] PageCss = EmbeddedAsset("app.css");
    private static readonly byte[] PageJs = EmbeddedAsset("app.js");

    // Every fixed player-facing string, read from Core's PlayerLabels, so the page invents no label of its own (spec §7.2).
    private static readonly IReadOnlyDictionary<string, string> Labels = typeof(PlayerLabels)
        .GetFields(BindingFlags.Public | BindingFlags.Static)
        .Where(field => field.IsLiteral && field.FieldType == typeof(string))
        .ToDictionary(field => field.Name, field => (string)field.GetRawConstantValue()!, StringComparer.Ordinal);

    // Requests are parsed strictly: exact camelCase names, no unknown members, numbers only as JSON numbers.
    private static readonly JsonSerializerOptions RequestJson = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
    };

    // A committed result must never fail to serialise, so a non-finite number would be written as a string, not thrown.
    private static readonly JsonSerializerOptions ResponseJson = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
        Converters = { new JsonStringEnumConverter() },
    };

    private readonly PlayerWorkPlanAdapter player;
    private readonly string capability;
    private readonly byte[] capabilityBytes;
    private readonly Dictionary<string, Route> routes;
    private volatile Listener? listener; // set once, after the bind is checked and before the URL is printed

    internal PlayerApi(PlayerWorkPlanAdapter player, string capability)
    {
        this.player = player;
        this.capability = capability;
        capabilityBytes = Encoding.ASCII.GetBytes(capability);

        // Core refuses a missing fence exactly like another session's fence (PlayerSessionMismatch), and a missing ID,
        // reason, PlanId or key exactly like a blank one, so absent values are passed through for the engine to refuse
        // with its own codes. The page never sends a plan object, and no route here can build one.
        routes = new Dictionary<string, Route>(StringComparer.Ordinal)
        {
            [""] = new("GET", context => WriteAssetAsync(context, PageHtml, "text/html; charset=utf-8")),
            ["app.css"] = new("GET", context => WriteAssetAsync(context, PageCss, "text/css; charset=utf-8")),
            ["app.js"] = new("GET", context => WriteAssetAsync(context, PageJs, "text/javascript; charset=utf-8")),
            ["api/view"] = new("GET", ReadViewAsync),
            ["api/hit"] = new("POST", HitTestAsync),
            ["api/pending/add"] = new("POST", context => CommandAsync<AddRequest>(context,
                request => player.AddPending(request.Fence!, request.TreeId ?? "", request.Reason ?? ""))),
            ["api/pending/remove"] = new("POST", context => CommandAsync<TreeRequest>(context,
                request => player.RemovePending(request.Fence!, request.TreeId ?? ""))),
            ["api/plan/approve"] = new("POST", context => CommandAsync<FenceRequest>(context,
                request => player.Approve(request.Fence!))),
            ["api/plan/cancel"] = new("POST", context => CommandAsync<FenceRequest>(context,
                request => player.CancelApproved(request.Fence!))),
            ["api/plan/execute"] = new("POST", context => CommandAsync<ExecuteRequest>(context,
                request => player.Execute(request.Fence!, request.PlanId ?? "", request.ExecutionKey ?? ""))),
            ["api/year/advance"] = new("POST", context => CommandAsync<AdvanceRequest>(context,
                request => player.Advance(request.Fence!, request.ConsentToCancelWork))),
        };
    }

    /// <summary>Called once the loopback bind is verified: from then on a request must name exactly this host and origin.</summary>
    internal void AdmitRequestsFor(int port) => listener = new Listener(
        FormattableString.Invariant($"127.0.0.1:{port}"), FormattableString.Invariant($"http://127.0.0.1:{port}"));

    /// <summary>The single terminal request handler: the guards first, then exactly one route.</summary>
    internal async Task HandleAsync(HttpContext context)
    {
        var request = context.Request;
        var response = context.Response;
        SetSecurityHeaders(response.Headers);

        var admitted = listener;
        if (admitted is null)
        {
            response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            return;
        }

        // DNS-rebinding guard: the browser must have addressed exactly this loopback listener. No body.
        if (!string.Equals(request.Headers.Host.ToString(), admitted.Host, StringComparison.Ordinal))
        {
            response.StatusCode = StatusCodes.Status403Forbidden;
            return;
        }

        // Capability guard: /s/<capability>/<route>. Anything else is 404 with no body.
        if (!TrySplitPath(request.Path.Value ?? "", out string candidate, out string? routeName) ||
            !CryptographicOperations.FixedTimeEquals(Encoding.ASCII.GetBytes(candidate), capabilityBytes))
        {
            response.StatusCode = StatusCodes.Status404NotFound;
            return;
        }
        if (routeName is null)
        {
            // "/s/<capability>" without the final slash: send its holder to the page, so relative URLs resolve.
            response.StatusCode = StatusCodes.Status308PermanentRedirect;
            response.Headers.Location = "/s/" + capability + "/";
            return;
        }

        if (!routes.TryGetValue(routeName, out var route))
        {
            await WriteErrorAsync(context, StatusCodes.Status404NotFound, "NotFound", "no such page or endpoint");
            return;
        }
        if (!string.Equals(request.Method, route.Method, StringComparison.Ordinal))
        {
            response.Headers.Allow = route.Method;
            await WriteErrorAsync(context, StatusCodes.Status405MethodNotAllowed, "MethodNotAllowed",
                "use " + route.Method + " for this endpoint; nothing changed");
            return;
        }

        // Cross-origin guard: every POST must come from this page's own origin, and any request that names an origin must
        // name this one. No CORS header is ever sent.
        string? origin = request.Headers.Origin.Count == 0 ? null : request.Headers.Origin.ToString();
        if ((origin is not null || route.Method == "POST") && !string.Equals(origin, admitted.Origin, StringComparison.Ordinal))
        {
            await WriteErrorAsync(context, StatusCodes.Status403Forbidden, "OriginRefused",
                "this request did not come from the Stand Map page; nothing changed");
            return;
        }

        try
        {
            await route.Handle(context);
        }
        catch (BadHttpRequestException bad) when (!response.HasStarted)
        {
            // One of Kestrel's own limits, such as a body over MaxBodyBytes: the adapter was not called.
            bool tooLarge = bad.StatusCode == StatusCodes.Status413PayloadTooLarge;
            await WriteErrorAsync(context, bad.StatusCode, tooLarge ? "RequestTooLarge" : "BadRequest",
                bad.Message + "; nothing changed");
        }
        catch (Exception fault) when (!response.HasStarted && fault is not OperationCanceledException)
        {
            // Not a refusal: the engine may or may not have committed, so the page must re-read (fail closed, spec §6.6).
            Console.Error.WriteLine($"OUTCOME UNCONFIRMED {request.Method} {routeName}: {fault.GetType().Name}: {fault.Message}");
            await WriteErrorAsync(context, StatusCodes.Status500InternalServerError, "OutcomeUnconfirmed",
                PlayerLabels.OutcomeUnconfirmed);
        }
    }

    private Task ReadViewAsync(HttpContext context)
    {
        var view = player.Read();
        return WriteJsonAsync(context, StatusCodes.Status200OK, new ViewPayload(view, PlayerMapScene.Build(view), Labels));
    }

    private async Task HitTestAsync(HttpContext context)
    {
        var request = await ReadRequestAsync<HitRequest>(context);
        if (request is null) return; // a guard has already answered
        // A point with a missing coordinate is malformed. It is refused rather than read as 0, so a broken request can
        // never quietly select whatever tree covers a map corner.
        if (request.U is not double u || request.V is not double v)
        {
            await WriteErrorAsync(context, StatusCodes.Status400BadRequest, "BadRequest",
                "a map point needs both u and v in map metres; nothing was selected");
            return;
        }
        var result = player.HitTest(request.Fence!, u, v);
        await WriteJsonAsync(context, StatusOf(result.RefusalCode), new HitPayload(result.View,
            PlayerMapScene.Build(result.View), result.Candidates, NoticeOf(result.RefusalCode, result.RefusalMessage)));
    }

    private static async Task CommandAsync<TRequest>(HttpContext context, Func<TRequest, PlayerCommandResult> command)
        where TRequest : class
    {
        var request = await ReadRequestAsync<TRequest>(context);
        if (request is null) return; // a guard has already answered
        var result = command(request);
        await WriteJsonAsync(context, StatusOf(result.RefusalCode), new CommandPayload(result.View,
            PlayerMapScene.Build(result.View), NoticeOf(result.RefusalCode, result.RefusalMessage),
            result.Execution, result.Annual));
    }

    // A refusal is never a silent 200: it is 409 with the engine's code, its message and the fresh current view.
    private static int StatusOf(string? refusalCode) =>
        refusalCode is null ? StatusCodes.Status200OK : StatusCodes.Status409Conflict;

    private static Notice? NoticeOf(string? code, string? message) => code is null ? null : new Notice(code, message ?? "");

    // Returns null after answering the request itself, when a guard refuses it (415, 413 or 400).
    private static async Task<T?> ReadRequestAsync<T>(HttpContext context) where T : class
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

    private static bool IsJsonContentType(string? contentType) =>
        string.Equals(contentType, "application/json", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(contentType, "application/json; charset=utf-8", StringComparison.OrdinalIgnoreCase);

    // Reads at most MaxBodyBytes. Returns null when the body is larger (Kestrel's own limit may refuse it first).
    private static async Task<byte[]?> ReadBoundedBodyAsync(HttpRequest request, CancellationToken cancel)
    {
        if (request.ContentLength > MaxBodyBytes) return null;
        var buffer = new byte[MaxBodyBytes + 1];
        int total = 0;
        while (total < buffer.Length)
        {
            int read = await request.Body.ReadAsync(buffer.AsMemory(total), cancel);
            if (read == 0) return buffer[..total];
            total += read;
        }
        return null;
    }

    private static async Task WriteJsonAsync<T>(HttpContext context, int status, T payload)
    {
        // Serialise first, so that a failure here leaves the response unstarted and becomes "outcome unconfirmed".
        byte[] bytes = JsonSerializer.SerializeToUtf8Bytes(payload, ResponseJson);
        context.Response.StatusCode = status;
        context.Response.ContentType = JsonContentType;
        context.Response.ContentLength = bytes.Length;
        await context.Response.Body.WriteAsync(bytes, context.RequestAborted);
    }

    private static Task WriteErrorAsync(HttpContext context, int status, string code, string message) =>
        WriteJsonAsync(context, status, new ErrorPayload(new Notice(code, message)));

    private static async Task WriteAssetAsync(HttpContext context, byte[] bytes, string contentType)
    {
        context.Response.StatusCode = StatusCodes.Status200OK;
        context.Response.ContentType = contentType;
        context.Response.ContentLength = bytes.Length;
        await context.Response.Body.WriteAsync(bytes, context.RequestAborted);
    }

    private static void SetSecurityHeaders(IHeaderDictionary headers)
    {
        headers.CacheControl = "no-store";
        headers.XContentTypeOptions = "nosniff";
        headers.XFrameOptions = "DENY";
        headers.ContentSecurityPolicy = PagePolicy;
        headers["Referrer-Policy"] = "no-referrer"; // the capability is in the path; never leak it
        headers["Cross-Origin-Resource-Policy"] = "same-origin";
    }

    // "/s/<capability>/<route>" gives the capability candidate and the route ("" is the page itself). When the slash after
    // the capability is missing, the route is null.
    private static bool TrySplitPath(string path, out string candidate, out string? route)
    {
        const string Prefix = "/s/";
        candidate = "";
        route = null;
        if (!path.StartsWith(Prefix, StringComparison.Ordinal)) return false;
        int slash = path.IndexOf('/', Prefix.Length);
        candidate = slash < 0 ? path[Prefix.Length..] : path[Prefix.Length..slash];
        route = slash < 0 ? null : path[(slash + 1)..];
        return true;
    }

    private static byte[] EmbeddedAsset(string name)
    {
        using var stream = typeof(PlayerApi).Assembly.GetManifestResourceStream("SitkaP1.Ui.wwwroot." + name)
            ?? throw new InvalidOperationException("the page asset " + name + " is not embedded in this build");
        using var copy = new MemoryStream();
        stream.CopyTo(copy);
        return copy.ToArray();
    }

    private sealed record Route(string Method, Func<HttpContext, Task> Handle);

    private sealed record Listener(string Host, string Origin);

    // Request bodies: the fence the page displayed, plus exact IDs or the engine-minted PlanId and key it echoes.
    private sealed record FenceRequest(PlayerFence? Fence);

    private sealed record TreeRequest(PlayerFence? Fence, string? TreeId);

    private sealed record AddRequest(PlayerFence? Fence, string? TreeId, string? Reason);

    private sealed record ExecuteRequest(PlayerFence? Fence, string? PlanId, string? ExecutionKey);

    private sealed record AdvanceRequest(PlayerFence? Fence, bool ConsentToCancelWork);

    private sealed record HitRequest(PlayerFence? Fence, double? U, double? V);

    // Response bodies. "refusal" is the engine's own refusal; "error" is a guard or an unconfirmed outcome.
    private sealed record Notice(string Code, string Message);

    private sealed record ErrorPayload(Notice Error);

    private sealed record ViewPayload(PlayerSessionView View, IReadOnlyList<PlayerMapItem> Scene,
        IReadOnlyDictionary<string, string> Labels);

    private sealed record HitPayload(PlayerSessionView View, IReadOnlyList<PlayerMapItem> Scene,
        IReadOnlyList<PlayerMapItem> Candidates, Notice? Refusal);

    private sealed record CommandPayload(PlayerSessionView View, IReadOnlyList<PlayerMapItem> Scene, Notice? Refusal,
        WorkExecutionResult? Execution, AnnualReport? Annual);
}
