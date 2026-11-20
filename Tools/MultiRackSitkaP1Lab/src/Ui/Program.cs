using System.Net;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using SitkaP1.Core;
using SitkaP1.Ui;

// #849 UI-A (spec MULTI-STANDMAP-PLAYABLE-2D-V1 §3.2, §3.5, §3.6): the playable 2D Stand Map as ONE in-memory player
// session, served to a browser on 127.0.0.1 only. This is an HTTP server, allowed only as this single presentation
// process (spec §0.4 Q-1): no persistence, no second listener, no CORS and no outbound network. Process exit ends the
// session; nothing is saved. Exit codes follow the CLI: 0 ok, 2 refused (code printed), 3 listener check failed, 64 usage.
return await UiProcess.RunAsync(args);

internal static class UiProcess
{
    // The P0 lab's port. P1 never binds it (lab README), so a listener that somehow landed on it is refused.
    private const int P0LabPort = 5187;

    internal static async Task<int> RunAsync(string[] args)
    {
        if (args.Length != 1)
        {
            Console.Error.WriteLine("usage: sitkap1-ui <derivedFixtureDir>");
            return 64;
        }

        PlayerWorkPlanAdapter player;
        try
        {
            // The only file this process reads: the trusted, catalog-checked fixture, through Core's no-follow reader.
            player = PlayerWorkPlanAdapter.Open(args[0]);
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
        var api = new PlayerApi(player, capability);
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
        Console.WriteLine("Ctrl+C ends the session; nothing is saved.");

        // Ctrl+C (SIGINT) or SIGTERM stops the host gracefully; then the in-memory session is simply dropped.
        await app.WaitForShutdownAsync();
        Console.WriteLine("Session ended; nothing was saved.");
        return 0;
    }
}
