using SitkaP1.Core;

// P1-F0 command-line scaffold. No command writes outside the path given to generate-manifest, and no command runs an
// ecological year. Exit codes: 0 ok, 2 refused (code printed), 64 usage.
return Cli.Run(args);

static class Cli
{
    public static int Run(string[] args)
    {
        try
        {
            switch (args.FirstOrDefault())
            {
                case "generate-manifest" when args.Length == 2:
                {
                    var bytes = ManifestGenerator.Generate().FileBytes();
                    File.WriteAllBytes(args[1], bytes);
                    Console.WriteLine($"wrote {bytes.Length} bytes sha256={DerivedManifest.Sha256Hex(bytes)}");
                    return 0;
                }
                case "verify-derived" when args.Length == 3:
                {
                    var load = FixtureAccess.LoadDerived(args[1], File.ReadAllBytes(args[2]), TrustUse.ReviewEvidenceOnly);
                    var m = load.Manifest;
                    Console.WriteLine(P1Disclosure.Derived30);
                    Console.WriteLine($"domain={P1DomainWire.Name(load.Record.Domain)} id={m.DerivedFixtureId} trust={load.TrustLabel}");
                    Console.WriteLine($"trees={m.Trees.Count} belowActionThreshold={m.Trees.Count(t => t.DbhMm < m.ActionEligibleDbhThresholdMm)} contentHash={m.ContentHash}");
                    Console.WriteLine($"derivedCatalogDigest={DerivedCatalog.Digest()}");
                    return 0;
                }
                case "inspect-original" when args.Length is 3 or 4:
                {
                    var use = args.Length == 4 && args[3] == "--review-evidence" ? TrustUse.ReviewEvidenceOnly : TrustUse.RequireAccepted;
                    var r = FixtureAccess.InspectOriginal(args[1], args[2], use);
                    Console.WriteLine(r.Disclosure);
                    Console.WriteLine(r.ToString());
                    Console.WriteLine($"originalTrustTableDigest={OriginalTrustTable.Digest()}");
                    return 0;
                }
                case "annual" when args.Length == 3:
                {
                    var domain = P1DomainWire.Parse(args[1]);
                    throw AnnualGate.Check(domain, DerivedCatalog.Find(args[2]));
                }
                default:
                    Console.Error.WriteLine("usage: sitkap1 generate-manifest <out> | verify-derived <id> <file> | inspect-original <p0FixtureDir> <id> [--review-evidence] | annual <domain> <id>");
                    return 64;
            }
        }
        catch (P1Refusal e)
        {
            Console.Error.WriteLine($"REFUSED {e.Code}: {e.Message}");
            return 2;
        }
    }
}
