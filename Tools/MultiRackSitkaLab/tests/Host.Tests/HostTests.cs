using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using SitkaLab.Core;
using SitkaLab.Host;
using SitkaLab.Testing;

namespace SitkaLab.Host.Tests;

/// <summary>A real host on a real loopback port with a throw-away tool root (fixtures copied, fresh .lab-data).</summary>
internal sealed class TestHost : IDisposable
{
    public readonly string Root;
    public readonly LabHost.Running Running;
    public readonly HttpClient Http = new(new SocketsHttpHandler { UseProxy = false }) { Timeout = TimeSpan.FromSeconds(20) };
    public string Base => Running.BaseUrl;
    public string LabData => Path.Combine(Root, ".lab-data");

    public TestHost(ILabFileAccess? fileAccess = null)
    {
        Root = Directory.CreateTempSubdirectory("sitkalab-host-").FullName;
        Directory.CreateDirectory(Path.Combine(Root, "fixtures", "synthetic"));
        foreach (var f in Directory.GetFiles(ToolPaths.FixtureDir, "*.sitkalab.json"))
            File.Copy(f, Path.Combine(Root, "fixtures", "synthetic", Path.GetFileName(f)));
        Running = LabHost.Build(new LabHostOptions { ToolRoot = Root, WebRoot = Path.Combine(ToolPaths.ToolRoot, "src", "Ui", "wwwroot"), Port = 0, FileAccess = fileAccess });
        Running.App.StartAsync().GetAwaiter().GetResult();
    }

    public HttpRequestMessage Req(HttpMethod m, string path, object? body = null, bool good = true, string? host = null, string? origin = null, bool labHeader = true, string contentType = "application/json")
    {
        var r = new HttpRequestMessage(m, Base + path);
        if (host is not null) r.Headers.Host = host;
        if (m == HttpMethod.Post)
        {
            if (good) r.Headers.TryAddWithoutValidation("Origin", Base);
            if (origin is not null) { r.Headers.Remove("Origin"); r.Headers.TryAddWithoutValidation("Origin", origin); }
            if (labHeader) r.Headers.TryAddWithoutValidation("X-Sitka-Lab", "1");
            r.Content = new StringContent(body is null ? "{}" : (body as string ?? JsonSerializer.Serialize(body)), Encoding.UTF8, contentType);
        }
        else if (origin is not null) r.Headers.TryAddWithoutValidation("Origin", origin);
        return r;
    }

    public async Task<(HttpStatusCode Status, JsonElement Json)> Post(string path, object? body = null)
    {
        var res = await Http.SendAsync(Req(HttpMethod.Post, path, body));
        string text = await res.Content.ReadAsStringAsync();
        return (res.StatusCode, text.Length > 0 && text[0] == '{' ? JsonDocument.Parse(text).RootElement.Clone() : default);
    }

    public async Task<JsonElement> GetJson(string path) => JsonDocument.Parse(await Http.GetStringAsync(Base + path)).RootElement.Clone();

    public void Dispose()
    {
        Running.App.StopAsync().GetAwaiter().GetResult();
        Running.App.DisposeAsync().AsTask().GetAwaiter().GetResult();
        Http.Dispose();
        try { Directory.Delete(Root, true); } catch { /* best effort */ }
    }
}

/// <summary>
/// Real lab file access with one-shot hooks around the atomic write, so a test can change the lab folder at an exact point:
/// just before the write opens the folder (pre-commit) or just after the rename has committed (post-commit).
/// </summary>
internal sealed class HookedFileAccess : ILabFileAccess
{
    private readonly RealLabFileAccess _real = new();
    public Action? BeforeWrite;
    public Action? AfterWrite;
    public bool DirectoryExists(string path) => _real.DirectoryExists(path);
    public string? LinkTarget(string path) => _real.LinkTarget(path);
    public IReadOnlyList<string> ListFileNames(string dir) => _real.ListFileNames(dir);
    public LabEntryKind Classify(string dir, string fileName) => _real.Classify(dir, fileName);
    public byte[] ReadRegularFile(string dir, string fileName, long maxBytes) => _real.ReadRegularFile(dir, fileName, maxBytes);
    public void CreateDirectory(string path) => _real.CreateDirectory(path);
    public void WriteAtomic(string dir, string fileName, byte[] bytes)
    {
        var before = BeforeWrite; BeforeWrite = null; before?.Invoke();
        _real.WriteAtomic(dir, fileName, bytes);
        var after = AfterWrite; AfterWrite = null; after?.Invoke();
    }
}

public static class HostTests
{
    private static T R<T>(Func<Task<T>> f) => f().GetAwaiter().GetResult();

    [Test]
    public static void Host_ListensOnLoopbackOnly()
    {
        using var h = new TestHost();
        var addrs = h.Running.App.Urls.ToList();
        T.True(addrs.Count == 1 && addrs[0].StartsWith("http://127.0.0.1:"), "bound addresses: " + string.Join(",", addrs));
        var health = h.GetJson("/api/health").GetAwaiter().GetResult();
        T.Eq("127.0.0.1", health.GetProperty("bind").GetString(), "reported local address");
        var listening = File.ReadAllLines("/proc/net/tcp").Concat(File.ReadAllLines("/proc/net/tcp6")).Skip(1)
            .Select(l => l.Split(' ', StringSplitOptions.RemoveEmptyEntries)).Where(p => p.Length > 3 && p[3] == "0A")
            .Where(p => p[1].EndsWith(":" + h.Running.Port.ToString("X4"))).Select(p => p[1]).ToList();
        T.True(listening.Count >= 1, "found the listening socket in /proc/net/tcp");
        T.True(listening.All(a => a.StartsWith("0100007F:")), "every listener for this port is 127.0.0.1 (0100007F): " + string.Join(",", listening));
    }

    [Test]
    public static void Hostile_HostOriginMethodAndHeaderCases_AreRefused()
    {
        using var h = new TestHost();
        int Code(HttpRequestMessage r) => (int)R(() => h.Http.SendAsync(r)).StatusCode;
        T.Eq(403, Code(h.Req(HttpMethod.Get, "/api/state", host: "evil.example")), "foreign Host");
        T.Eq(403, Code(h.Req(HttpMethod.Get, "/api/state", host: $"localhost:{h.Running.Port}")), "localhost alias (DNS-rebinding shape)");
        T.Eq(403, Code(h.Req(HttpMethod.Get, "/api/state", host: $"127.0.0.1:{h.Running.Port + 1}")), "wrong port in Host");
        T.Eq(403, Code(h.Req(HttpMethod.Get, "/api/state", origin: "http://evil.example")), "foreign Origin on GET");
        T.Eq(403, Code(h.Req(HttpMethod.Post, "/api/step/advance", origin: "http://evil.example")), "foreign Origin on POST");
        T.Eq(403, Code(h.Req(HttpMethod.Post, "/api/step/advance", good: false)), "POST with no Origin");
        T.Eq(403, Code(h.Req(HttpMethod.Post, "/api/step/advance", labHeader: false)), "POST without the lab header");
        T.Eq(415, Code(h.Req(HttpMethod.Post, "/api/step/advance", contentType: "text/plain")), "POST with the wrong content type");
        var preflight = h.Req(HttpMethod.Options, "/api/state"); preflight.Headers.TryAddWithoutValidation("Origin", "http://evil.example"); preflight.Headers.TryAddWithoutValidation("Access-Control-Request-Method", "POST");
        var pr = R(() => h.Http.SendAsync(preflight));
        T.Eq(405, (int)pr.StatusCode, "CORS preflight");
        T.False(pr.Headers.Contains("Access-Control-Allow-Origin"), "no CORS allow header");
        foreach (var m in new[] { HttpMethod.Put, HttpMethod.Delete, HttpMethod.Patch, HttpMethod.Trace })
            T.Eq(405, Code(h.Req(m, "/api/state")), "method " + m);
        var ok = R(() => h.Http.SendAsync(h.Req(HttpMethod.Get, "/api/state")));
        T.Eq(200, (int)ok.StatusCode, "control: a well-formed request works");
        T.False(ok.Headers.Contains("Access-Control-Allow-Origin"), "no CORS header on success either");
        T.True(ok.Headers.GetValues("Content-Security-Policy").Single().Contains("default-src 'none'"), "restrictive CSP");
        T.True(ok.Headers.GetValues("Cache-Control").Single().Contains("no-store"), "no-store");
        T.Eq(0, h.Running.State.Session.Events.Count, "hostile requests changed nothing");
    }

    [Test]
    public static void Responses_CannotReadUnrelatedFiles_AndNoEndpointTouchesProductionSaves()
    {
        using var h = new TestHost();
        // Sentinels a hostile request might try to reach: next to the tool root, in the lab root's parent, and a CCF-save look-alike.
        string secret = "SECRET-SENTINEL-" + Guid.NewGuid().ToString("N");
        var ccfSave = Path.Combine(Path.GetDirectoryName(h.Root)!, "forest-save-" + Guid.NewGuid().ToString("N") + ".json");
        File.WriteAllText(ccfSave, secret); var stamp = File.GetLastWriteTimeUtc(ccfSave);
        File.WriteAllText(Path.Combine(h.Root, "secret.txt"), secret);
        try
        {
            string[] paths = { "/secret.txt", "/../secret.txt", "/%2e%2e/secret.txt", "/..%2fsecret.txt", "/fixtures/synthetic/oracle-quarter-ha.sitkalab.json", "/.lab-data/", "/appsettings.json",
                "/SitkaLab.Host.dll", "/SitkaLab.Host.deps.json", "/api/", "/api/../secret.txt", "/index.html/../../secret.txt", "/etc/passwd", "//etc/passwd", "/app.js/../../../../etc/passwd" };
            foreach (var p in paths)
            {
                var res = R(() => h.Http.SendAsync(h.Req(HttpMethod.Get, p)));
                string body = R(() => res.Content.ReadAsStringAsync());
                T.False(body.Contains(secret), $"GET {p} must not leak files");
                T.False(body.Contains("root:x:") || body.Contains("\"trees\":["), $"GET {p} must not serve system or fixture files");
                T.True((int)res.StatusCode is 400 or 404 or 405 or 301 or 308, $"GET {p} → {(int)res.StatusCode}");
            }
            foreach (var name in new[] { ccfSave, "../" + Path.GetFileName(ccfSave), Path.GetFileName(ccfSave), "/etc/passwd", "forest-save.json", "ForestSaveData.json", "../../.." })
            {
                var (st, _) = R(() => h.Post("/api/saves/load", new { fileName = name }));
                T.True((int)st is 403 or 400, $"load '{name}' refused ({(int)st})");
                var (st2, _) = R(() => h.Post("/api/saves/save", new { name }));
                T.Eq(400, (int)st2, $"save as '{name}' refused");
            }
            T.Eq(secret, File.ReadAllText(ccfSave), "CCF-save look-alike untouched"); T.Eq(stamp, File.GetLastWriteTimeUtc(ccfSave), "…and never rewritten");
            T.Eq(0, Directory.GetFiles(h.LabData).Length, "nothing was written under .lab-data");
            T.False(Directory.GetFiles(Path.GetDirectoryName(h.Root)!, "*.sitkalab.json", SearchOption.TopDirectoryOnly).Any(), "nothing escaped next to the tool root");
        }
        finally { File.Delete(ccfSave); }
    }

    [Test]
    public static void RequestBodies_AreBoundedAndValidated()
    {
        using var h = new TestHost();
        var big = new string('x', 1_200_000);
        var res = R(() => h.Http.SendAsync(h.Req(HttpMethod.Post, "/api/preview", "{\"treeIds\":[\"" + big + "\"]}")));
        T.Eq(413, (int)res.StatusCode, "oversized body refused with 413");
        T.False(R(() => res.Content.ReadAsStringAsync()).Contains("Exception"), "no server exception text leaks to the client");
        var bad = R(() => h.Http.SendAsync(h.Req(HttpMethod.Post, "/api/marks/mark", "{not json")));
        T.Eq(400, (int)bad.StatusCode, "malformed JSON → 400");
        var (st, js) = R(() => h.Post("/api/marks/mark", new { treeIds = new[] { "bad id with spaces" } }));
        T.Eq(409, (int)st, "unsafe ID refused"); T.Eq("InvalidId", js.GetProperty("code").GetString(), "code");
        var (st2, _) = R(() => h.Post("/api/plans/..%2f..%2fx/execute"));
        T.True((int)st2 is 400 or 404, "plan id path trick refused: " + (int)st2);
        var (st3, js3) = R(() => h.Post("/api/plans/P0001/explode"));
        T.Eq(404, (int)st3, "unknown action"); T.Eq("UnknownAction", js3.GetProperty("code").GetString(), "code");
        var (st4, js4) = R(() => h.Post("/api/fixture/load", new { fixtureId = "oracle-quarter-ha", confirm = false }));
        T.Eq(400, (int)st4, "reset requires confirmation"); T.Eq("ConfirmationRequired", js4.GetProperty("code").GetString(), "code");
        var (st5, js5) = R(() => h.Post("/api/fixture/load", new { fixtureId = "../../x", confirm = true }));
        T.Eq(400, (int)st5, "unknown fixture"); T.Eq("UnknownFixture", js5.GetProperty("code").GetString(), "no path-based fixture loading");
    }

    [Test]
    public static void HttpWorkflow_MarkToExecute_MatchesCoreAndIsAtomic()
    {
        using var h = new TestHost();
        var st0 = R(() => h.GetJson("/api/state"));
        var trees = st0.GetProperty("trees").EnumerateArray().Where(t => t.GetProperty("eligible").GetBoolean()).Take(5).Select(t => t.GetProperty("treeId").GetString()!).ToList();
        var n0 = st0.GetProperty("metrics").GetProperty("eligibleLiveN").GetInt32();
        var prev = R(() => h.Post("/api/preview", new { treeIds = trees })).Json;
        T.True(prev.GetProperty("valid").GetBoolean(), "preview valid");
        T.Eq(0, R(() => h.GetJson("/api/state")).GetProperty("eventCount").GetInt32(), "preview wrote nothing");
        foreach (var (path, body) in new (string, object?)[] { ("/api/marks/mark", new { treeIds = trees }), ("/api/plans/create", new { treeIds = trees }) })
            T.Eq(HttpStatusCode.OK, R(() => h.Post(path, body)).Status, path);
        foreach (var a in new[] { "propose", "approve", "queue" }) T.Eq(HttpStatusCode.OK, R(() => h.Post($"/api/plans/P0001/{a}")).Status, a);
        var mid = R(() => h.GetJson("/api/state"));
        T.Eq(n0, mid.GetProperty("metrics").GetProperty("eligibleLiveN").GetInt32(), "nothing felled before Execute");
        var done = R(() => h.Post("/api/plans/P0001/execute"));
        T.Eq(HttpStatusCode.OK, done.Status, "execute");
        var st1 = done.Json.GetProperty("state");
        T.Eq(n0 - 5, st1.GetProperty("metrics").GetProperty("eligibleLiveN").GetInt32(), "N after");
        T.Eq(prev.GetProperty("after").GetProperty("baM2").GetDouble(), st1.GetProperty("metrics").GetProperty("baM2").GetDouble(), "BA after == Core preview");
        var again = R(() => h.Post("/api/plans/P0001/execute"));
        T.Eq(HttpStatusCode.Conflict, again.Status, "second execute refused"); T.Eq("PlanAlreadyExecuted", again.Json.GetProperty("code").GetString(), "code");
        T.Eq(n0 - 5, again.Json.GetProperty("state").GetProperty("metrics").GetProperty("eligibleLiveN").GetInt32(), "nothing more felled");
        var replay = R(() => h.GetJson("/api/replay"));
        T.True(replay.GetProperty("ok").GetBoolean() && replay.GetProperty("matchesCurrentState").GetBoolean(), "server-side replay matches");
    }

    [Test]
    public static void Saves_RoundTripOverHttp_AndWrongFormatContentFailsAtTheHeaderGate()
    {
        using var h = new TestHost();
        var ids = R(() => h.GetJson("/api/state")).GetProperty("trees").EnumerateArray().Where(t => t.GetProperty("eligible").GetBoolean()).Take(3).Select(t => t.GetProperty("treeId").GetString()!).ToList();
        R(() => h.Post("/api/marks/mark", new { treeIds = ids })); R(() => h.Post("/api/plans/create", new { treeIds = ids }));
        foreach (var a in new[] { "propose", "approve" }) R(() => h.Post($"/api/plans/P0001/{a}"));
        var before = R(() => h.GetJson("/api/state"));
        var saved = R(() => h.Post("/api/saves/save", new { name = "http-run" }));
        T.Eq(HttpStatusCode.OK, saved.Status, "save"); T.Eq("http-run.sitkalab.json", saved.Json.GetProperty("fileName").GetString(), "server appended suffix");
        T.True(File.Exists(Path.Combine(h.LabData, "http-run.sitkalab.json")), "file landed under .lab-data");
        var listed = R(() => h.GetJson("/api/saves")).GetProperty("files").EnumerateArray().Select(e => e.GetString()).ToList();
        T.Seq(new[] { "http-run.sitkalab.json" }, listed!, "lab list");
        R(() => h.Post("/api/fixture/load", new { fixtureId = "oracle-quarter-ha", confirm = true }));
        var loaded = R(() => h.Post("/api/saves/load", new { fileName = "http-run.sitkalab.json" }));
        T.Eq(HttpStatusCode.OK, loaded.Status, "load");
        var plans = loaded.Json.GetProperty("state").GetProperty("plans").EnumerateArray().ToList();
        T.Eq("STALE", plans[0].GetProperty("planState").GetString(), "approved plan stale after load");
        T.True(loaded.Json.GetProperty("state").GetProperty("stateHash").GetString() != before.GetProperty("stateHash").GetString(), "stale transition changes the hash");
        T.Eq(before.GetProperty("eventCount").GetInt32() + 1, loaded.Json.GetProperty("state").GetProperty("eventCount").GetInt32(), "one activation event");

        // CCF-shaped JSON placed inside the lab root: rejected at the header gate, nothing written, nothing changed.
        File.WriteAllText(Path.Combine(h.LabData, "ccf-look-alike.sitkalab.json"), "{\"saveVersion\":3,\"trees\":[{\"species\":\"Oak\"}],BROKEN");
        var entries = Directory.GetFileSystemEntries(h.LabData).OrderBy(x => x).ToList();
        var hash = R(() => h.GetJson("/api/state")).GetProperty("stateHash").GetString();
        var bad = R(() => h.Post("/api/saves/load", new { fileName = "ccf-look-alike.sitkalab.json" }));
        T.Eq(HttpStatusCode.BadRequest, bad.Status, "wrong format refused"); T.Eq("WrongFormat", bad.Json.GetProperty("code").GetString(), "header gate code (not JsonInvalid)");
        T.Seq(entries, Directory.GetFileSystemEntries(h.LabData).OrderBy(x => x), "no file created or changed");
        T.Eq(hash, R(() => h.GetJson("/api/state")).GetProperty("stateHash").GetString(), "session untouched by the failed load");

        // a symlink in the lab root is neither listed nor loadable
        File.CreateSymbolicLink(Path.Combine(h.LabData, "link.sitkalab.json"), Path.Combine(h.Root, "fixtures", "synthetic", "oracle-quarter-ha.sitkalab.json"));
        T.False(R(() => h.GetJson("/api/saves")).GetProperty("files").EnumerateArray().Any(e => e.GetString() == "link.sitkalab.json"), "symlink not offered");
        var viaLink = R(() => h.Post("/api/saves/load", new { fileName = "link.sitkalab.json" }));
        T.Eq(HttpStatusCode.Forbidden, viaLink.Status, "symlink load refused");
    }

    // ---- MULTI-004: special files, controlled save errors and honest error codes ------------------------------------------

    private static void MkFifo(string path) { var p = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("mkfifo", path) { UseShellExecute = false })!; p.WaitForExit(); T.Eq(0, p.ExitCode, "mkfifo available"); }

    private static (HttpStatusCode Status, JsonElement Json, string Raw) Send(TestHost h, HttpRequestMessage r, int seconds = 5)
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(seconds));
        try
        {
            var res = h.Http.SendAsync(r, cts.Token).GetAwaiter().GetResult();
            string text = res.Content.ReadAsStringAsync(cts.Token).GetAwaiter().GetResult();
            return (res.StatusCode, text.Length > 0 && text[0] == '{' ? JsonDocument.Parse(text).RootElement.Clone() : default, text);
        }
        catch (OperationCanceledException) { throw new TestFailure($"{r.Method} {r.RequestUri?.AbsolutePath} did not answer within {seconds} s (the host is hung)"); }
    }

    [Test]
    public static void FifoInTheLabFolder_NeverHangsTheHost_EvenWithConcurrentStateRequests()
    {
        using var h = new TestHost();
        string fifo = Path.Combine(h.LabData, "pipe.sitkalab.json"); MkFifo(fifo);
        T.False(R(() => h.GetJson("/api/saves")).GetProperty("files").EnumerateArray().Any(e => e.GetString() == "pipe.sitkalab.json"), "FIFO is not offered");
        // Loads of the FIFO name run concurrently with state reads. Before the fix the load blocked while holding the session lock and every other request timed out.
        var loads = Enumerable.Range(0, 4).Select(_ => Task.Run(() => Send(h, h.Req(HttpMethod.Post, "/api/saves/load", new { fileName = "pipe.sitkalab.json" })))).ToList();
        var states = Enumerable.Range(0, 12).Select(_ => Task.Run(() => Send(h, h.Req(HttpMethod.Get, "/api/state")))).ToList();
        T.True(Task.WaitAll(loads.Concat(states).Cast<Task>().ToArray(), TimeSpan.FromSeconds(8)), "every request finished within the bound");
        foreach (var l in loads) { T.Eq(HttpStatusCode.Forbidden, l.Result.Status, "FIFO load refused"); T.Eq("NotListed", l.Result.Json.GetProperty("code").GetString(), "refused by the listing layer"); }
        foreach (var st in states) T.Eq(HttpStatusCode.OK, st.Result.Status, "state stays available");
        T.Eq(0, h.Running.State.Session.Events.Count, "session untouched");
        T.True(File.Exists(fifo) && new RealLabFileAccess().Classify(h.LabData, "pipe.sitkalab.json") == LabEntryKind.Fifo, "the FIFO was not deleted or replaced by the host");
        // ordinary lab saves still round-trip next to the FIFO
        T.Eq(HttpStatusCode.OK, Send(h, h.Req(HttpMethod.Post, "/api/saves/save", new { name = "beside-fifo" })).Status, "save works");
        T.Eq(HttpStatusCode.OK, Send(h, h.Req(HttpMethod.Post, "/api/saves/load", new { fileName = "beside-fifo.sitkalab.json" })).Status, "load works");
    }

    [Test]
    public static void SaveOntoAFolderOrFifo_IsAControlled409_NotAnUnhandled500_AndLeavesNoTempFile()
    {
        using var h = new TestHost();
        string dir = Path.Combine(h.LabData, "squat.sitkalab.json"); Directory.CreateDirectory(dir); File.WriteAllText(Path.Combine(dir, "keep.txt"), "KEEP");
        MkFifo(Path.Combine(h.LabData, "pipe2.sitkalab.json"));
        foreach (var name in new[] { "squat", "pipe2" })
        {
            var r = Send(h, h.Req(HttpMethod.Post, "/api/saves/save", new { name }));
            T.Eq(HttpStatusCode.Conflict, r.Status, name + " → 409");
            T.Eq("NotRegularFile", r.Json.GetProperty("code").GetString(), name + " code");
            T.False(r.Json.GetProperty("ok").GetBoolean(), name + " ok=false");
            T.False(r.Raw.Contains(h.Root), name + ": no path in the body");
        }
        T.Eq(0, Directory.GetFiles(h.LabData, "*.tmp-*").Length, "no leftover temp file");
        T.Eq("KEEP", File.ReadAllText(Path.Combine(dir, "keep.txt")), "the folder and its content are untouched");
        T.Eq(HttpStatusCode.OK, Send(h, h.Req(HttpMethod.Get, "/api/state")).Status, "host still healthy");
    }

    [Test]
    public static void SaveThatCannotBeWritten_IsAControlled500_WithAMatchingCode_AndNoTempFileOrPathLeak()
    {
        if (!OperatingSystem.IsLinux()) return;   // the lab file layer is Linux-only; directory modes are used here to provoke a write failure
        using var h = new TestHost();
        var original = File.GetUnixFileMode(h.LabData);
        File.SetUnixFileMode(h.LabData, UnixFileMode.UserRead | UnixFileMode.UserExecute);   // read-only lab folder
        try
        {
            bool canStillWrite;
            try { File.WriteAllText(Path.Combine(h.LabData, "probe.tmp"), "x"); File.Delete(Path.Combine(h.LabData, "probe.tmp")); canStillWrite = true; } catch (UnauthorizedAccessException) { canStillWrite = false; }
            if (canStillWrite) return;   // running with privileges that ignore directory modes: this failure cannot be provoked here
            var r = Send(h, h.Req(HttpMethod.Post, "/api/saves/save", new { name = "denied" }));
            T.Eq(HttpStatusCode.InternalServerError, r.Status, "write failure → 500");
            T.Eq("WriteFailed", r.Json.GetProperty("code").GetString(), "code names the problem, not 'Forbidden'");
            T.False(r.Raw.Contains(h.Root) || r.Raw.Contains("Exception"), "no path or exception text leaks");
        }
        finally { File.SetUnixFileMode(h.LabData, original); }
        T.Eq(0, Directory.GetFiles(h.LabData).Length, "nothing was left behind");
        T.Eq(HttpStatusCode.OK, Send(h, h.Req(HttpMethod.Post, "/api/saves/save", new { name = "after" })).Status, "saving works again once the folder is writable");
    }

    [Test]
    public static void RefusalCodes_MatchTheirHttpStatus()
    {
        using var h = new TestHost();
        void Expect(string what, HttpRequestMessage req, HttpStatusCode status, string code)
        {
            var r = Send(h, req);
            T.Eq(status, r.Status, what + " status");
            T.Eq(code, r.Json.GetProperty("code").GetString(), what + " code");
            T.False(r.Json.GetProperty("ok").GetBoolean(), what + " ok=false");
        }
        Expect("foreign Host", h.Req(HttpMethod.Get, "/api/state", host: "evil.example"), HttpStatusCode.Forbidden, "Forbidden");
        Expect("PUT", h.Req(HttpMethod.Put, "/api/state"), HttpStatusCode.MethodNotAllowed, "MethodNotAllowed");
        Expect("text/plain POST", h.Req(HttpMethod.Post, "/api/step/advance", contentType: "text/plain"), HttpStatusCode.UnsupportedMediaType, "UnsupportedMediaType");
        Expect("oversized body", h.Req(HttpMethod.Post, "/api/preview", "{\"treeIds\":[\"" + new string('x', 1_200_000) + "\"]}"), HttpStatusCode.RequestEntityTooLarge, "PayloadTooLarge");
        // An unexpected server-side failure (the lab folder was replaced by a plain file behind the host's back) is a 500 with its own code and no detail.
        Directory.Delete(h.LabData, true); File.WriteAllText(h.LabData, "not a folder");
        Expect("broken lab folder", h.Req(HttpMethod.Get, "/api/saves"), HttpStatusCode.InternalServerError, "InternalError");
        T.False(Send(h, h.Req(HttpMethod.Get, "/api/saves")).Raw.Contains(h.Root), "no path leaks from the generic 500");
    }

    [Test]
    public static void Host_SourceNeverReferencesProductionSavesUnityOrOutboundNetwork()
    {
        var banned = new[] { "ForestSaveData", "forest-save", "persistentDataPath", "UnityEngine", "Assets/", "HttpClient", "WebRequest", "TcpClient", "System.Net.Http", "System.Net.Sockets", "AddHttpClient", "Application Insights" };
        var files = Directory.GetFiles(Path.Combine(ToolPaths.ToolRoot, "src"), "*.*", SearchOption.AllDirectories)
            .Where(f => (f.EndsWith(".cs") || f.EndsWith(".csproj") || f.EndsWith(".js") || f.EndsWith(".html")) && !f.Contains("/bin/") && !f.Contains("/obj/")).ToList();
        T.True(files.Count > 10, "scanned the production source tree");
        foreach (var f in files)
        {
            string text = File.ReadAllText(f);
            foreach (var b in banned)
            {
                if (text.Contains(b, StringComparison.Ordinal))
                    throw new TestFailure($"{Path.GetRelativePath(ToolPaths.ToolRoot, f)} mentions '{b}'");
            }
        }
        foreach (var f in Directory.GetFiles(Path.Combine(ToolPaths.ToolRoot, "src", "Ui", "wwwroot"), "*.js")) // browser must not do external fetches
            T.False(System.Text.RegularExpressions.Regex.IsMatch(File.ReadAllText(f).Replace("http://www.w3.org/2000/svg", ""), @"https?://"), "no absolute URLs in browser code (the SVG XML namespace identifier is not a network address)");
    }

    [Test]
    public static void Browser_CodeContainsNoForestryArithmetic()
    {
        // The front end may format and draw but must never compute BA, density, Dq or removal fractions.
        string js = File.ReadAllText(Path.Combine(ToolPaths.ToolRoot, "src", "Ui", "wwwroot", "app.js"));
        foreach (var banned in new[] { "Math.PI", "Math.sqrt", "Math.pow", "** 2", "basalArea(", "reduce((a, b) => a + b", ".reduce(" })
            T.False(js.Contains(banned), "app.js must not contain " + banned);
        T.True(js.Contains("NO forestry arithmetic"), "header comment states the rule");
    }
    // ---- MULTI-006 R-1: saves over HTTP while a same-user process keeps exchanging the lab folder with a link -----------------

    [System.Runtime.InteropServices.DllImport("libc", EntryPoint = "renameat2", SetLastError = true)]
    private static extern int RenameAt2(int oldDirFd, [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPUTF8Str)] string oldPath, int newDirFd, [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPUTF8Str)] string newPath, uint flags);
    private static bool Exchange(string a, string b) => RenameAt2(-100, a, -100, b, 2 /* RENAME_EXCHANGE */) == 0;

    [Test]
    public static void SavesDuringAConcurrentLabFolderExchange_NeverWriteTheAlternateFolder_LeaveNoTempFiles_AndReportHonestly()
    {
        using var h = new TestHost();
        // A VALID lab save from a different fixture sits in the attacker's alternate folder (the reviewer's sentinel shape).
        T.Eq(HttpStatusCode.OK, R(() => h.Post("/api/fixture/load", new { fixtureId = "oracle-quarter-ha", confirm = true })).Status, "switch fixture");
        T.Eq(HttpStatusCode.OK, Send(h, h.Req(HttpMethod.Post, "/api/saves/save", new { name = "w" })).Status, "make a valid lab save");
        string alternate = Path.Combine(h.Root, "alternate"); Directory.CreateDirectory(alternate);
        File.Move(Path.Combine(h.LabData, "w.sitkalab.json"), Path.Combine(alternate, "w.sitkalab.json"));
        byte[] sentinel = File.ReadAllBytes(Path.Combine(alternate, "w.sitkalab.json"));
        T.Eq(HttpStatusCode.OK, R(() => h.Post("/api/fixture/load", new { fixtureId = "sitka-flat-demo-v1", confirm = true })).Status, "back to the demo fixture");
        string link = Path.Combine(h.Root, "swaplink"); Directory.CreateSymbolicLink(link, alternate);

        int stop = 0; long swaps = 0;
        var swapper = new Thread(() => { while (Volatile.Read(ref stop) == 0) if (Exchange(h.LabData, link)) swaps++; }) { IsBackground = true };
        var outcomes = new Dictionary<string, int>(); int alternateChanges = 0, okButAlternate = 0, okListNotRefreshed = 0, okWithoutFileName = 0;
        swapper.Start();
        try
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            while (sw.Elapsed < TimeSpan.FromSeconds(3))
            {
                var r = Send(h, h.Req(HttpMethod.Post, "/api/saves/save", new { name = "w" }), seconds: 8);
                string k = $"{(int)r.Status}:{(r.Json.ValueKind == JsonValueKind.Object ? r.Json.GetProperty("code").GetString() : "?")}";
                outcomes[k] = outcomes.GetValueOrDefault(k) + 1;
                if (r.Status == HttpStatusCode.OK)
                {
                    // #567: a 200 always names the committed file; a list refresh refused by the swap is reported beside it, never as a failed save.
                    if (r.Json.GetProperty("fileName").GetString() != "w.sitkalab.json") okWithoutFileName++;
                    if (!r.Json.GetProperty("listRefreshed").GetBoolean()) okListNotRefreshed++;
                }
                if (!File.ReadAllBytes(Path.Combine(alternate, "w.sitkalab.json")).AsSpan().SequenceEqual(sentinel))
                {
                    alternateChanges++; if (r.Status == HttpStatusCode.OK) okButAlternate++;
                    File.WriteAllBytes(Path.Combine(alternate, "w.sitkalab.json"), sentinel);
                }
            }
        }
        finally { Volatile.Write(ref stop, 1); swapper.Join(); }
        if (new DirectoryInfo(h.LabData).LinkTarget is not null) T.True(Exchange(h.LabData, link), "swap back");
        string summary = $"swaps={swaps} responses={string.Join(",", outcomes.Select(kv => kv.Key + "=" + kv.Value))} alternate-overwrites={alternateChanges} (reported 200: {okButAlternate}) 200-with-list-not-refreshed={okListNotRefreshed}";
        Console.WriteLine("    INFO " + summary);
        T.True(swaps > 50, "the swapper really raced the host: " + summary);
        T.True(outcomes.GetValueOrDefault("200:ok") > 0, "some saves succeeded: " + summary);
        T.True(outcomes.Keys.All(k => k is "200:ok" or "403:SymlinkRejected" or "500:WriteFailed"), "only controlled outcomes: " + summary);
        T.Eq(0, alternateChanges, "alternate-folder overwrites: " + summary);
        T.Eq(0, okWithoutFileName, "every 200 names the committed file: " + summary);
        T.Seq(new[] { "w.sitkalab.json" }, Directory.GetFileSystemEntries(alternate).Select(Path.GetFileName), "nothing created in the alternate folder (no temp files there)");
        T.Eq(0, Directory.GetFiles(h.LabData, "*.tmp-*").Length, "no temp file orphaned in the lab folder");
        var load = Send(h, h.Req(HttpMethod.Post, "/api/saves/load", new { fileName = "w.sitkalab.json" }));
        T.True(load.Status == HttpStatusCode.OK && load.Raw.Contains("\"fixtureId\":\"sitka-flat-demo-v1\""), "the lab folder holds a genuine demo-fixture save that loads");
        T.Eq(HttpStatusCode.OK, Send(h, h.Req(HttpMethod.Get, "/api/state")).Status, "host still healthy");
    }

    // ---- #567 (historical #517 R-3): the save response reports the committed save, not the optional list refresh that follows it -------------

    /// <summary>Lab folder with a sibling "alternate" folder holding one sentinel file, and a symlink to it that <see cref="Exchange"/> can swap in.</summary>
    private static (string Alternate, string Link, byte[] Sentinel) AlternateFolder(TestHost h)
    {
        string alternate = Path.Combine(h.Root, "alternate"); Directory.CreateDirectory(alternate);
        byte[] sentinel = Encoding.UTF8.GetBytes("ALTERNATE-SENTINEL-" + Guid.NewGuid().ToString("N"));
        File.WriteAllBytes(Path.Combine(alternate, "keep.sitkalab.json"), sentinel);
        string link = Path.Combine(h.Root, "swaplink"); Directory.CreateSymbolicLink(link, alternate);
        return (alternate, link, sentinel);
    }

    private static void AssertAlternateUntouched(string alternate, byte[] sentinel)
    {
        T.Seq(new[] { "keep.sitkalab.json" }, Directory.GetFileSystemEntries(alternate).Select(Path.GetFileName), "nothing created in the alternate folder");
        T.True(File.ReadAllBytes(Path.Combine(alternate, "keep.sitkalab.json")).AsSpan().SequenceEqual(sentinel), "alternate sentinel unchanged");
    }

    [Test]
    public static void SaveThatCommits_ThenMeetsASwappedLabFolder_Reports200WithTheFile_AndSaysTheListWasNotRefreshed()
    {
        if (!OperatingSystem.IsLinux()) return;
        var io = new HookedFileAccess();
        using var h = new TestHost(io);
        var (alternate, link, sentinel) = AlternateFolder(h);
        // The rename has committed; a same-user process now exchanges the lab folder with the link before the host refreshes the list.
        io.AfterWrite = () => T.True(Exchange(h.LabData, link), "exchange after commit");
        var r = Send(h, h.Req(HttpMethod.Post, "/api/saves/save", new { name = "committed" }));
        try
        {
            T.Eq(HttpStatusCode.OK, r.Status, "committed save → 200 (was a false 403 before #567): " + r.Raw);
            T.True(r.Json.GetProperty("ok").GetBoolean(), "ok=true");
            T.Eq("ok", r.Json.GetProperty("code").GetString(), "save code");
            T.Eq("committed.sitkalab.json", r.Json.GetProperty("fileName").GetString(), "file name of the committed save");
            T.False(r.Json.GetProperty("listRefreshed").GetBoolean(), "list refresh reported as not done");
            T.Eq("SymlinkRejected", r.Json.GetProperty("listCode").GetString(), "why the list was not refreshed");
            T.Eq(JsonValueKind.Null, r.Json.GetProperty("files").ValueKind, "no file list offered from a folder that failed the checks");
            T.False(r.Raw.Contains(h.Root) || r.Raw.Contains("Exception"), "no path or exception text leaks");
            // The listing endpoint itself still refuses the swapped folder: nothing about the security check was relaxed.
            var list = Send(h, h.Req(HttpMethod.Get, "/api/saves"));
            T.Eq(HttpStatusCode.Forbidden, list.Status, "GET /api/saves still refuses the swapped-in link");
            T.Eq("SymlinkRejected", list.Json.GetProperty("code").GetString(), "listing refusal code");
        }
        finally { if (new DirectoryInfo(h.LabData).LinkTarget is not null) T.True(Exchange(h.LabData, link), "swap back"); }
        AssertAlternateUntouched(alternate, sentinel);
        T.Seq(new[] { "committed.sitkalab.json" }, Directory.GetFileSystemEntries(h.LabData).Select(Path.GetFileName), "the save is in the real lab folder, with no temp file");
        var load = Send(h, h.Req(HttpMethod.Post, "/api/saves/load", new { fileName = "committed.sitkalab.json" }));
        T.True(load.Status == HttpStatusCode.OK && load.Raw.Contains("\"fixtureId\":\"sitka-flat-demo-v1\""), "the committed save loads: " + load.Status);
    }

    [Test]
    public static void SaveThatCommits_ThenCannotListForAnUnexpectedIoReason_StillReports200_WithAGenericListCode()
    {
        if (!OperatingSystem.IsLinux()) return;
        var io = new HookedFileAccess();
        using var h = new TestHost(io);
        string moved = h.LabData + "-moved";
        // After the commit the lab folder is moved away and a plain file takes its name, so the refresh fails with an ordinary IOException.
        io.AfterWrite = () => { Directory.Move(h.LabData, moved); File.WriteAllText(h.LabData, "not a folder"); };
        var r = Send(h, h.Req(HttpMethod.Post, "/api/saves/save", new { name = "io-after" }));
        File.Delete(h.LabData); Directory.Move(moved, h.LabData);
        T.Eq(HttpStatusCode.OK, r.Status, "committed save → 200 (was a generic 500 before #567): " + r.Raw);
        T.True(r.Json.GetProperty("ok").GetBoolean(), "ok=true");
        T.Eq("io-after.sitkalab.json", r.Json.GetProperty("fileName").GetString(), "file name");
        T.False(r.Json.GetProperty("listRefreshed").GetBoolean(), "list not refreshed");
        T.Eq("ListFailed", r.Json.GetProperty("listCode").GetString(), "generic code, no exception detail");
        T.False(r.Raw.Contains(h.Root) || r.Raw.Contains("Exception") || r.Raw.Contains("not a folder"), "no path or exception text leaks");
        T.Seq(new[] { "io-after.sitkalab.json" }, Directory.GetFileSystemEntries(h.LabData).Select(Path.GetFileName), "the save is in the lab folder, with no temp file");
    }

    [Test]
    public static void SaveThatMeetsASwappedLabFolderBeforeCommit_StillFails403_AndWritesNothingAnywhere()
    {
        if (!OperatingSystem.IsLinux()) return;
        var io = new HookedFileAccess();
        using var h = new TestHost(io);
        var (alternate, link, sentinel) = AlternateFolder(h);
        // The name/classification checks have passed; the folder is exchanged just before the write opens it.
        io.BeforeWrite = () => T.True(Exchange(h.LabData, link), "exchange before commit");
        var r = Send(h, h.Req(HttpMethod.Post, "/api/saves/save", new { name = "precommit" }));
        try
        {
            T.Eq(HttpStatusCode.Forbidden, r.Status, "pre-commit refusal still fails: " + r.Raw);
            T.False(r.Json.GetProperty("ok").GetBoolean(), "ok=false");
            T.Eq("SymlinkRejected", r.Json.GetProperty("code").GetString(), "refusal code");
            T.False(r.Json.TryGetProperty("fileName", out _), "no file name is claimed for a refused save");
            T.False(r.Json.TryGetProperty("listRefreshed", out _), "no list-refresh field on a refusal");
        }
        finally { if (new DirectoryInfo(h.LabData).LinkTarget is not null) T.True(Exchange(h.LabData, link), "swap back"); }
        AssertAlternateUntouched(alternate, sentinel);
        T.Eq(0, Directory.GetFileSystemEntries(h.LabData).Length, "nothing written to the lab folder");
        // Control: with no interference the same save succeeds and the list is refreshed as before.
        var ok = Send(h, h.Req(HttpMethod.Post, "/api/saves/save", new { name = "precommit" }));
        T.Eq(HttpStatusCode.OK, ok.Status, "undisturbed save → 200");
        T.True(ok.Json.GetProperty("listRefreshed").GetBoolean(), "list refreshed");
        T.Eq(JsonValueKind.Null, ok.Json.GetProperty("listCode").ValueKind, "no list code when the refresh worked");
        T.Seq(new[] { "precommit.sitkalab.json" }, ok.Json.GetProperty("files").EnumerateArray().Select(e => e.GetString()!), "refreshed list");
    }
}
