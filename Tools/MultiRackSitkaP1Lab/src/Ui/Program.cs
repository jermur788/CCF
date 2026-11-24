using System.Net;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using SitkaP1.Core;
using SitkaP1.Core.Phase1;
using SitkaP1.Ui;

// #849 UI-A (spec MULTI-STANDMAP-PLAYABLE-2D-V1 §3.2, §3.5, §3.6): the playable 2D Stand Map as ONE in-memory player
// session, served to a browser on 127.0.0.1 only. This is an HTTP server, allowed only as this single presentation
// process (spec §0.4 Q-1): no second listener, no CORS and no outbound network. The single-area page keeps nothing; the
// two-area page (/multi) keeps only the named saves the player writes into its own server-managed slot folder, which is
// the only file this process writes. Process exit ends both sessions. Exit codes follow the CLI: 0 ok, 2 refused (code
// printed), 3 listener check failed, 64 usage.
return await UiProcess.RunAsync(args);

internal static class UiProcess
{
    // The P0 lab's port. P1 never binds it (lab README), so a listener that somehow landed on it is refused.
    private const int P0LabPort = 5187;

    internal static async Task<int> RunAsync(string[] args)
    {
        if (!TryParseArgs(args, out string fixtureDir, out string experimentKey, out string? error))
        {
            if (error is not null) Console.Error.WriteLine(error);
            Console.Error.WriteLine("usage: sitkap1-ui <derivedFixtureDir> [--multi-experiment=ma-1|ma-1-mixed]");
            return 64;
        }

        PlayerWorkPlanAdapter player;
        MultiAreaWorkPlanAdapter multiArea;
        MultiAreaContinuation multiSession;
        Phase1SaveSlots saveSlots;
        try
        {
            // The only file this process reads: the trusted, catalog-checked fixture, through Core's no-follow reader.
            player = PlayerWorkPlanAdapter.Open(fixtureDir);
            // /multi: defaults to Phase 2 (1/1/1/1/1) on standard synthetic MA-1 (all Sitka), or MA-1-MIXED demo.
            (multiArea, multiSession) = OpenMultiExperiment(experimentKey);
            // The one folder this process writes: the two-area page's named save slots, under the user's own local data
            // directory. The player never supplies a path; the store refuses symlinks, traversal and non-regular files.
            saveSlots = OpenSaveDirectory();
        }
        catch (P1Refusal refusal)
        {
            Console.Error.WriteLine($"REFUSED {refusal.Code}: {refusal.Message}");
            return 2;
        }

        // A fresh 256-bit capability per launch, from the operating system's cryptographic random source (spec §3.6).
        string capability = Convert.ToHexString(RandomNumberGenerator.GetBytes(32)).ToLowerInvariant();

        // The EMPTY builder (rather than the slim one) reads no appsettings file, no environment-variable URLs or Kestrel
        // endpoints and no certificate, starts no file watcher and registers no logging provider. So the single Listen
        // call below is the only listener, whatever the environment says, and the process reads no other file.
        var builder = WebApplication.CreateEmptyBuilder(new WebApplicationOptions { ApplicationName = "sitkap1-ui" });
        builder.WebHost.UseKestrelCore().ConfigureKestrel(kestrel =>
        {
            kestrel.AddServerHeader = false;
            kestrel.Limits.MaxRequestBodySize = PlayerApi.MaxBodyBytes;
            kestrel.Listen(IPAddress.Loopback, 0); // 127.0.0.1 only; port 0 asks the OS for an ephemeral port
        });

        await using var app = builder.Build();
        var api = new PlayerApi(player, multiArea, multiSession, saveSlots, capability);
        app.Run(api.HandleAsync); // one terminal handler: every request passes the same guards (PlayerApi)
        await app.StartAsync();

        // Check what was actually bound before anybody is told the address.
        var bound = app.Urls.Select(url => new Uri(url)).ToArray();
        if (bound.Length != 1 || bound[0].Host != "127.0.0.1" || bound[0].Port == P0LabPort || bound[0].Port < 1024)
        {
            Console.Error.WriteLine("STOP: unexpected listener [" + string.Join(", ", app.Urls) + "]; shutting down");
            await app.StopAsync();
            return 3;
        }

        int port = bound[0].Port;
        api.AdmitRequestsFor(port);
        Console.WriteLine($"{PlayerLabels.Title}. {PlayerLabels.SessionOnly}.");
        Console.WriteLine($"Open http://127.0.0.1:{port}/s/{capability}/");
        string multiTag = string.Equals(experimentKey, MultiAreaDemoExperiments.Ma1MixedKey, StringComparison.Ordinal)
            ? "MA-1-MIXED" : "MA-1";
        Console.WriteLine($"Two-area {multiTag} simulator: http://127.0.0.1:{port}/s/{capability}/multi");
        Console.WriteLine($"Experiment: {multiArea.ExperimentBadge}");
        Console.WriteLine($"Saved games folder (two-area page only): {saveSlots.Directory}");
        Console.WriteLine("Ctrl+C ends the session; the two-area page's named saves stay in that folder.");

        // Ctrl+C (SIGINT) or SIGTERM stops the host gracefully; then the in-memory sessions are simply dropped.
        await app.WaitForShutdownAsync();
        Console.WriteLine(api.SavesWritten
            ? "Session ended. Saved games remain in their slot files."
            : "Session ended; nothing was saved.");
        return 0;
    }

    // The single dedicated save-slot folder: <local data>/ccf-multi-2d-saves, created if absent by the store itself. No
    // player input reaches this path, and Phase1SaveSlots refuses it if any component is a symbolic link.
    private static Phase1SaveSlots OpenSaveDirectory() =>
        Phase1SaveSlots.OpenOrCreate(Path.Combine(LocalDataDirectory(), "ccf-multi-2d-saves"));

    // The XDG data directory of this user (XDG spec: $XDG_DATA_HOME, else $HOME/.local/share). Environment.GetFolderPath
    // reports LocalApplicationData as empty when the directory does not exist yet, so the path is derived here instead and
    // the store creates it (mode 0700) on first use.
    private static string LocalDataDirectory()
    {
        string xdg = Environment.GetEnvironmentVariable("XDG_DATA_HOME") ?? "";
        if (Path.IsPathFullyQualified(xdg)) return xdg;
        string home = Environment.GetEnvironmentVariable("HOME") ?? "";
        if (Path.IsPathFullyQualified(home)) return Path.Combine(home, ".local", "share");
        string known = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        if (!string.IsNullOrEmpty(known)) return known;
        throw new P1Refusal("SlotDirectoryInvalid", "this process has no local data directory (XDG_DATA_HOME or HOME) for saves");
    }

    private static (MultiAreaWorkPlanAdapter Adapter, MultiAreaContinuation Session) OpenMultiExperiment(string experimentKey)
    {
        if (string.Equals(experimentKey, MultiAreaDemoExperiments.Ma1Key, StringComparison.Ordinal))
        {
            var experiment = MultiAreaGenerator.Generate();
            var session = new MultiAreaContinuation(experiment, MultiAreaModelSelection.Phase2);
            return (new MultiAreaWorkPlanAdapter(session, experiment, MultiAreaDemoExperiments.Ma1Key, MultiAreaDemoExperiments.Ma1Badge), session);
        }
        if (string.Equals(experimentKey, MultiAreaDemoExperiments.Ma1MixedKey, StringComparison.Ordinal))
        {
            var (experiment, options, badge) = MultiAreaDemoExperiments.CreateMixedDemo();
            var session = new MultiAreaContinuation(experiment, MultiAreaModelSelection.Phase2, null, null, options);
            return (new MultiAreaWorkPlanAdapter(session, experiment, MultiAreaDemoExperiments.Ma1MixedKey, badge), session);
        }
        throw new P1Refusal("MultiAreaUnknownExperiment",
            "supported --multi-experiment values are ma-1 and ma-1-mixed");
    }

    private static bool TryParseArgs(string[] args, out string fixtureDir, out string experimentKey, out string? error)
    {
        const string ExperimentPrefix = "--multi-experiment=";
        fixtureDir = "";
        experimentKey = MultiAreaDemoExperiments.Ma1Key;
        error = null;
        bool seenExperiment = false;
        foreach (string arg in args)
        {
            if (arg.StartsWith(ExperimentPrefix, StringComparison.Ordinal))
            {
                if (seenExperiment) { error = "duplicate --multi-experiment option"; return false; }
                seenExperiment = true;
                string value = arg[ExperimentPrefix.Length..];
                if (!string.Equals(value, MultiAreaDemoExperiments.Ma1Key, StringComparison.Ordinal) &&
                    !string.Equals(value, MultiAreaDemoExperiments.Ma1MixedKey, StringComparison.Ordinal))
                {
                    error = "invalid --multi-experiment value: " + value + " (expected ma-1 or ma-1-mixed)";
                    return false;
                }
                experimentKey = value;
            }
            else if (arg.StartsWith("-", StringComparison.Ordinal))
            {
                error = "unknown option: " + arg;
                return false;
            }
            else
            {
                if (fixtureDir.Length != 0) { error = "unexpected extra argument: " + arg; return false; }
                fixtureDir = arg;
            }
        }
        if (fixtureDir.Length == 0)
        {
            error = "missing <derivedFixtureDir>";
            return false;
        }
        return true;
    }
}
