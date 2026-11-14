using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SitkaLab.Core;

namespace SitkaLab.Host;

public sealed class LabHostOptions
{
    /// <summary>Directory that contains fixtures/synthetic and (created on demand) the gitignored .lab-data root.</summary>
    public string ToolRoot { get; init; } = "";
    /// <summary>Static UI files. Defaults to wwwroot next to the host binary.</summary>
    public string? WebRoot { get; init; }
    /// <summary>0 picks a free loopback port (tests).</summary>
    public int Port { get; init; } = 5187;
}

public sealed record ActionResponse(bool Ok, string Code, string Message, string? PlanId, IReadOnlyList<string>? TreeIds, SessionView? State);

/// <summary>Single in-process lab: catalog, data store and the one running session behind a lock.</summary>
public sealed class LabState
{
    public readonly object Gate = new();
    public FixtureCatalog Catalog { get; }
    public LabDataStore Store { get; }
    public LabSession Session { get; set; }

    public LabState(string toolRoot)
    {
        Catalog = FixtureCatalog.LoadFrom(Path.Combine(toolRoot, "fixtures", "synthetic"));
        Store = new LabDataStore(toolRoot);
        Store.EnsureRoot();
        var first = Catalog.TryGet("sitka-flat-demo-v1") ?? Catalog.All[0];
        Session = LabSession.FromFixture(first);
    }
}

public static class LabHost
{
    public sealed class Running
    {
        public required WebApplication App { get; init; }
        public required LabState State { get; init; }
        public int Port { get; internal set; }
        public string BaseUrl => $"http://127.0.0.1:{Port}";
    }

    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web) { WriteIndented = false };

    public static Running Build(LabHostOptions o)
    {
        var state = new LabState(o.ToolRoot);
        string webRoot = o.WebRoot ?? Path.Combine(AppContext.BaseDirectory, "wwwroot");
        if (!Directory.Exists(webRoot)) throw new DirectoryNotFoundException("UI files not found: " + webRoot);

        var builder = WebApplication.CreateBuilder(new WebApplicationOptions { ApplicationName = "SitkaLab.Host", ContentRootPath = AppContext.BaseDirectory, WebRootPath = webRoot, EnvironmentName = Environments.Production });
        builder.Logging.ClearProviders();
        builder.Logging.AddSimpleConsole(c => { c.SingleLine = true; });
        builder.Logging.SetMinimumLevel(LogLevel.Warning);
        // Loopback only. Never 0.0.0.0, never IPv6-any, no HTTPS/telemetry/outbound clients registered.
        builder.WebHost.ConfigureKestrel(k =>
        {
            k.Listen(IPAddress.Loopback, o.Port);
            k.AddServerHeader = false;
            k.Limits.MaxRequestBodySize = 1_000_000;
            k.Limits.MaxRequestHeadersTotalSize = 16_384;
            k.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(10);
        });
        var app = builder.Build();
        var running = new Running { App = app, State = state, Port = o.Port };
        app.Lifetime.ApplicationStarted.Register(() =>
        {
            var addr = app.Services.GetRequiredService<IServer>().Features.Get<IServerAddressesFeature>()!.Addresses.First();
            running.Port = new Uri(addr).Port;
        });

        app.Use((HttpContext ctx, Func<Task> next) => Guard(ctx, running, next));
        app.UseDefaultFiles(new DefaultFilesOptions { FileProvider = new PhysicalFileProvider(webRoot) });
        app.UseStaticFiles(new StaticFileOptions { FileProvider = new PhysicalFileProvider(webRoot), ServeUnknownFileTypes = false });
        MapApi(app, state);
        return running;
    }

    // ---- request guard: methods, Host, Origin, CSRF header, content type, response hardening -------------------
    private static async Task Guard(HttpContext ctx, Running r, Func<Task> next)
    {
        var h = ctx.Response.Headers;
        h["Cache-Control"] = "no-store";
        h["X-Content-Type-Options"] = "nosniff";
        h["X-Frame-Options"] = "DENY";
        h["Referrer-Policy"] = "no-referrer";
        h["Cross-Origin-Resource-Policy"] = "same-origin";
        h["Content-Security-Policy"] = "default-src 'none'; script-src 'self'; style-src 'self'; img-src 'self' data:; connect-src 'self'; base-uri 'none'; form-action 'none'; frame-ancestors 'none'";

        async Task Deny(int code, string why)
        {
            ctx.Response.StatusCode = code;
            ctx.Response.ContentType = "application/json";
            await ctx.Response.WriteAsync(JsonSerializer.Serialize(new { ok = false, code = CodeForStatus(code), message = why }, Json));
        }

        string expectedHost = $"127.0.0.1:{r.Port}";
        string expectedOrigin = $"http://127.0.0.1:{r.Port}";
        var method = ctx.Request.Method;
        if (method != "GET" && method != "POST" && method != "HEAD") { await Deny(405, "method not allowed"); return; }
        if (ctx.Connection.RemoteIpAddress is { } ip && !IPAddress.IsLoopback(ip)) { await Deny(403, "non-loopback client"); return; }
        if (!string.Equals(ctx.Request.Headers.Host.ToString(), expectedHost, StringComparison.OrdinalIgnoreCase)) { await Deny(403, "unexpected Host header"); return; }
        if (ctx.Request.Headers.TryGetValue("Origin", out var origin) && !string.Equals(origin.ToString(), expectedOrigin, StringComparison.Ordinal)) { await Deny(403, "unexpected Origin"); return; }
        if (ctx.Request.Headers.TryGetValue("Sec-Fetch-Site", out var site) && site.ToString() is not ("same-origin" or "none")) { await Deny(403, "cross-site request"); return; }
        if (method == "POST")
        {
            if (!ctx.Request.Headers.TryGetValue("Origin", out _)) { await Deny(403, "POST requires a same-origin Origin header"); return; }
            if (ctx.Request.Headers["X-Sitka-Lab"].ToString() != "1") { await Deny(403, "missing X-Sitka-Lab header"); return; }
            if (!(ctx.Request.ContentType ?? "").StartsWith("application/json", StringComparison.OrdinalIgnoreCase)) { await Deny(415, "application/json required"); return; }
        }
        try { await next(); }
        catch (Microsoft.AspNetCore.Http.BadHttpRequestException ex)
        {
            // e.g. body larger than the 1 MB limit: answer cleanly instead of surfacing a server exception
            if (!ctx.Response.HasStarted) await Deny(ex.StatusCode, ex.StatusCode == 413 ? "request body too large" : "bad request");
        }
        catch (Exception)
        {
            // never leak stack traces or file paths to a client
            if (!ctx.Response.HasStarted) await Deny(500, "internal error");
        }
    }

    // ---- API ------------------------------------------------------------------------------------------------
    private sealed class IdsRequest { public List<string>? TreeIds { get; set; } }
    private sealed class FixtureRequest { public string? FixtureId { get; set; } public bool Confirm { get; set; } }
    private sealed class SaveRequest { public string? Name { get; set; } }
    private sealed class LoadRequest { public string? FileName { get; set; } }

    private static async Task<T?> Body<T>(HttpContext ctx) where T : class
    {
        try { return await ctx.Request.ReadFromJsonAsync<T>(Json); }
        catch (JsonException) { return null; }
    }

    private static IResult Reply(int status, object body) => Results.Json(body, Json, "application/json", status);

    private static IResult Act(LabState st, Func<OpResult> op)
    {
        lock (st.Gate)
        {
            var r = op();
            return Reply(r.Ok ? 200 : 409, new ActionResponse(r.Ok, r.Code, r.Message, r.PlanId, r.TreeIds, Views.Build(st.Session)));
        }
    }

    /// <summary>Machine-readable code that matches the HTTP status of a refused request (never "Forbidden" for a 405/413/415/500).</summary>
    private static string CodeForStatus(int status) => status switch
    {
        400 => "BadRequest", 403 => "Forbidden", 404 => "NotFound", 405 => "MethodNotAllowed", 409 => "Conflict",
        413 => "PayloadTooLarge", 415 => "UnsupportedMediaType", 500 => "InternalError", 501 => "NotImplemented", _ => "Error",
    };

    /// <summary>HTTP status for a lab-store refusal. Bad names and content/format problems stay 400 (as before); the rest say who is at fault.</summary>
    private static int StoreStatus(string code) => code switch
    {
        "SymlinkRejected" or "HardLinkRejected" => 403,
        "NotRegularFile" => 409,
        "WriteFailed" or "ReadFailed" => 500,
        "PlatformUnsupported" => 501,
        _ => 400,
    };

    private static IResult Bad(string code, string message, int status = 400) => Reply(status, new ActionResponse(false, code, message, null, null, null));

    private static void MapApi(WebApplication app, LabState st)
    {
        app.MapGet("/api/health", (HttpContext c) => Reply(200, new { status = "ok", bind = c.Connection.LocalIpAddress?.ToString(), port = c.Connection.LocalPort, banner = LabConstants.SourceBanner }));

        app.MapGet("/api/state", () => { lock (st.Gate) return Reply(200, Views.Build(st.Session)); });

        app.MapGet("/api/fixtures", () => Reply(200, st.Catalog.All.Select(f => new
        {
            f.FixtureId, ForestId = f.Snapshot.ForestId, TreeCount = f.Snapshot.Trees.Count, AreaHa = (double)(Geometry.AreaM2(f.Snapshot.Polygon) / 10_000m),
            f.SnapshotHash, f.FileSha256,
        })));

        app.MapGet("/api/saves", () =>
        {
            try { return Reply(200, new { root = ".lab-data", files = st.Store.ListSaves() }); }
            catch (LabException ex) { return Bad(ex.Code, ex.Message, StoreStatus(ex.Code)); }
        });

        app.MapPost("/api/fixture/load", async (HttpContext c) =>
        {
            var req = await Body<FixtureRequest>(c);
            if (req is null || req.FixtureId is null) return Bad("BadRequest", "fixtureId required");
            if (!req.Confirm) return Bad("ConfirmationRequired", "resetting a fixture needs confirm=true");
            var fx = st.Catalog.TryGet(req.FixtureId);
            if (fx is null) return Bad("UnknownFixture", "no such checked-in fixture");
            lock (st.Gate) { st.Session = LabSession.FromFixture(fx); return Reply(200, new ActionResponse(true, "ok", "fixture loaded; all earlier in-memory plans are gone", null, null, Views.Build(st.Session))); }
        });

        app.MapPost("/api/saves/save", async (HttpContext c) =>
        {
            var req = await Body<SaveRequest>(c);
            if (req?.Name is null) return Bad("BadRequest", "name required");
            try
            {
                lock (st.Gate)
                {
                    string file = st.Store.WriteSave(req.Name, LabFile.Serialize(st.Session.ToFile()));
                    return Reply(200, new { ok = true, code = "ok", message = "saved " + file, fileName = file, files = st.Store.ListSaves() });
                }
            }
            catch (LabException ex) { return Bad(ex.Code, ex.Message, StoreStatus(ex.Code)); }
        });

        app.MapPost("/api/saves/load", async (HttpContext c) =>
        {
            var req = await Body<LoadRequest>(c);
            if (req?.FileName is null) return Bad("BadRequest", "fileName required");
            try
            {
                lock (st.Gate)
                {
                    var bytes = st.Store.ReadSave(req.FileName);          // name/symlink/root checks happen before the file is opened
                    var content = LabFile.Parse(bytes);                    // header gate first, then strict body
                    var session = LabSession.FromFile(content, st.Catalog);
                    st.Session = session;
                    return Reply(200, new ActionResponse(true, "ok", "loaded; approved and pending plans were made STALE", null, null, Views.Build(session)));
                }
            }
            catch (LabException ex) { return Bad(ex.Code, ex.Message, ex.Code is "NotListed" or "PathRejected" ? 403 : StoreStatus(ex.Code)); }
        });

        app.MapPost("/api/preview", async (HttpContext c) =>
        {
            var req = await Body<IdsRequest>(c);
            if (req?.TreeIds is null) return Bad("BadRequest", "treeIds required");
            lock (st.Gate) return Reply(200, Views.ToView(st.Session.Preview(req.TreeIds)));
        });

        app.MapPost("/api/marks/mark", async (HttpContext c) => { var r = await Body<IdsRequest>(c); return r?.TreeIds is null ? Bad("BadRequest", "treeIds required") : Act(st, () => st.Session.MarkTrees(r.TreeIds)); });
        app.MapPost("/api/marks/unmark", async (HttpContext c) => { var r = await Body<IdsRequest>(c); return r?.TreeIds is null ? Bad("BadRequest", "treeIds required") : Act(st, () => st.Session.UnmarkTrees(r.TreeIds)); });
        app.MapPost("/api/plans/create", async (HttpContext c) => { var r = await Body<IdsRequest>(c); return r?.TreeIds is null ? Bad("BadRequest", "treeIds required") : Act(st, () => st.Session.CreatePlan(r.TreeIds)); });

        app.MapPost("/api/plans/{planId}/{action}", (string planId, string action) =>
        {
            if (!LabConstants.IsSafeId(planId)) return Bad("InvalidId", "invalid plan id");
            Func<OpResult>? op = action switch
            {
                "propose" => () => st.Session.ProposePlan(planId),
                "approve" => () => st.Session.ApprovePlan(planId),
                "queue" => () => st.Session.QueuePlan(planId),
                "execute" => () => st.Session.ExecutePlan(planId),
                "reject" => () => st.Session.RejectPlan(planId),
                "cancel" => () => st.Session.CancelPlan(planId),
                _ => null,
            };
            return op is null ? Bad("UnknownAction", "unknown plan action", 404) : Act(st, op);
        });

        app.MapPost("/api/step/advance", () => Act(st, () => st.Session.AdvanceExperimentStep()));

        app.MapGet("/api/replay", () =>
        {
            lock (st.Gate)
            {
                var r = st.Session.VerifyReplay();
                return Reply(200, new { ok = r.Ok, finalHash = r.FinalHash, eventCount = r.Steps.Count, error = r.Error, matchesCurrentState = r.Ok && r.FinalHash == st.Session.StateHash });
            }
        });

        app.MapGet("/api/export", () =>
        {
            lock (st.Gate) return Results.Text(Views.ExportJson(st.Session), "application/json");
        });
    }
}
