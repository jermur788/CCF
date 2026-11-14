using SitkaLab.Host;

// Usage: dotnet run --project src/Host [-- --port 5187]
// Binds 127.0.0.1 only. No accounts, telemetry or outbound network access.
int port = 5187;
for (int i = 0; i < args.Length; i++)
    if (args[i] == "--port" && i + 1 < args.Length && int.TryParse(args[i + 1], out int p) && p >= 0 && p <= 65535) port = p;

static string FindToolRoot()
{
    var dir = new DirectoryInfo(AppContext.BaseDirectory);
    while (dir is not null)
    {
        if (Directory.Exists(Path.Combine(dir.FullName, "fixtures", "synthetic")) && File.Exists(Path.Combine(dir.FullName, "global.json"))) return dir.FullName;
        dir = dir.Parent;
    }
    throw new DirectoryNotFoundException("MultiRackSitkaLab tool root (fixtures/synthetic) not found above " + AppContext.BaseDirectory);
}

var running = LabHost.Build(new LabHostOptions { ToolRoot = FindToolRoot(), Port = port });
running.App.Lifetime.ApplicationStarted.Register(() =>
{
    Console.WriteLine($"Synthetic Sitka test stand (flat) — SYNTHETIC TEST FOREST — NO BIOLOGICAL GROWTH");
    Console.WriteLine($"Listening on {running.BaseUrl}  (loopback only; Ctrl+C to stop)");
});
await running.App.RunAsync();
return 0;
