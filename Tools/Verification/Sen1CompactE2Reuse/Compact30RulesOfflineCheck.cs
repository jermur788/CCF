using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

// Offline verification for the Compact30 E2 candidate fixtures and the site rules.
// This compiles Compact30SiteRules.cs directly (no UnityEngine types) and needs no Editor.
// It is a SOURCE-ONLY gate: it does not prove Unity compilation, scene binding or gameplay.

internal static class Compact30RulesOfflineCheck
{
    private static int checks;
    private static int failures;

    private static int Main(string[] args)
    {
        string root = args.Length > 0 ? args[0] : Directory.GetCurrentDirectory();
        string rosterPath = Path.Combine(root, "Compact30Roster.json");
        string sitePath = Path.Combine(root, "Compact30Sites.json");
        if (!File.Exists(rosterPath) || !File.Exists(sitePath))
        {
            Console.Error.WriteLine("fixtures not found under " + root);
            return 2;
        }

        var options = new JsonSerializerOptions { IncludeFields = true };
        Compact30RosterFile roster = JsonSerializer.Deserialize<Compact30RosterFile>(File.ReadAllText(rosterPath), options);
        Compact30SiteFile sites = JsonSerializer.Deserialize<Compact30SiteFile>(File.ReadAllText(sitePath), options);

        Check("roster contains 175 rows", roster != null && roster.rows.Count == 175);
        Check("roster digest reproduces #676 candidate", Compact30Rules.RosterSha256(roster) == Compact30Rules.ExpectedRosterSha256);
        Check("roster validates", Compact30Rules.RosterIsValid(roster, out string rosterProblem), rosterProblem);
        Check("all roster rows are Sitka", roster.rows.All(r => r.species == "sitka-spruce"));
        Check("roster ids are unique", roster.rows.Select(r => r.id).Distinct().Count() == roster.rows.Count);
        Check("four G1 sites", sites.sites.Count == 4);
        Check("2 Oak + 2 Beech", sites.sites.Count(s => s.species == "sessile-oak") == 2
            && sites.sites.Count(s => s.species == "beech") == 2);

        var stems = roster.rows.Select(r => new Compact30Point(r.x_m, r.z_m)).ToList();
        var legalSites = new List<Compact30Point>();
        foreach (Compact30SiteRow site in sites.sites)
        {
            float clearance = Compact30Rules.PathClearance(new Compact30Point(site.x_m, site.z_m), sites);
            Check(site.id + " cell matches 6x6 grid", Compact30Rules.CellIndex(site.x_m, site.z_m) == site.cell);
            Check(site.id + " path clearance is non-negative", clearance >= 0f);
            Check(site.id + " recorded clearance matches the anchors",
                Math.Abs(clearance - site.clearance_m) <= 0.05f);
            Check(site.id + " is outside H1", !Compact30Rules.InsideH1(site.x_m, site.z_m, sites.H1, sites.shelter_footprint_radius_m));
            Check(site.id + " is legal at designation", Compact30Rules.ValidateSite(site, sites, stems, legalSites, out string reason), reason);
            legalSites.Add(new Compact30Point(site.x_m, site.z_m));
        }

        // Negative witnesses: each must fail for the stated reason, proving the checks are live.
        Compact30SiteRow o1 = sites.sites[0];
        Compact30SiteRow badSpecies = Clone(o1);
        badSpecies.species = "beech"; // still has the oak stock item
        Check("wrong species/stock refused", !Compact30Rules.ValidateSite(badSpecies, sites, stems, legalSites, out string r1) && r1.Contains("stock item"));

        Compact30SiteRow onPath = Clone(o1);
        onPath.x_m = 2.8f;
        onPath.z_m = -2.25f;
        onPath.cell = Compact30Rules.CellIndex(onPath.x_m, onPath.z_m);
        Check("site on the path refused", !Compact30Rules.ValidateSite(onPath, sites, stems, new List<Compact30Point>(), out string r2) && r2.Contains("pedestrian path"));

        Compact30SiteRow inH1 = Clone(o1);
        inH1.x_m = 0f;
        inH1.z_m = -8f;
        inH1.cell = Compact30Rules.CellIndex(inH1.x_m, inH1.z_m);
        inH1.clearance_m = Compact30Rules.PathClearance(new Compact30Point(inH1.x_m, inH1.z_m), sites);
        Check("site in H1 refused", !Compact30Rules.ValidateSite(inH1, sites, stems, new List<Compact30Point>(), out string r3) && r3.Contains("H1"));

        Compact30SiteRow nearStem = Clone(sites.sites[0]);
        var syntheticStem = new List<Compact30Point> { new Compact30Point(nearStem.x_m + 0.2f, nearStem.z_m) };
        Check("site against a stem refused", !Compact30Rules.ValidateSite(nearStem, sites, syntheticStem, new List<Compact30Point>(), out string r4) && r4.Contains("stem"));

        Check("duplicate site position refused", !Compact30Rules.ValidateSite(o1, sites, stems, new List<Compact30Point> { new Compact30Point(o1.x_m, o1.z_m) }, out string r5) && r5.Contains("too close"));

        Compact30SiteRow drift = Clone(o1);
        drift.clearance_m += 0.5f;
        Check("clearance drift refused", !Compact30Rules.ValidateSite(drift, sites, stems, new List<Compact30Point>(), out string r6) && r6.Contains("drifted"));

        Compact30RosterFile edited = CloneRoster(roster);
        edited.rows[0].height_m += 0.01f;
        Check("edited roster refused", !Compact30Rules.RosterIsValid(edited, out string r7) && r7.Contains("digest"));

        Compact30RosterFile duplicate = CloneRoster(roster);
        duplicate.rows[1].id = duplicate.rows[0].id;
        Check("duplicate roster id refused", !Compact30Rules.RosterIsValid(duplicate, out string r8) && r8.Contains("Duplicate"));

        Compact30RosterFile shortRoster = CloneRoster(roster);
        shortRoster.rows.RemoveAt(shortRoster.rows.Count - 1);
        Check("short roster refused", !Compact30Rules.RosterIsValid(shortRoster, out string r9) && r9.Contains("175"));

        Console.WriteLine($"checks={checks} failures={failures}");
        return failures == 0 ? 0 : 1;
    }

    private static Compact30SiteRow Clone(Compact30SiteRow source) => new Compact30SiteRow
    {
        id = source.id,
        species = source.species,
        stock_item = source.stock_item,
        x_m = source.x_m,
        z_m = source.z_m,
        cell = source.cell,
        clearance_m = source.clearance_m,
        label = source.label
    };

    private static Compact30RosterFile CloneRoster(Compact30RosterFile source) => new Compact30RosterFile
    {
        schema = source.schema,
        status = source.status,
        source = source.source,
        roster_sha256 = source.roster_sha256,
        count = source.count,
        rows = source.rows.Select(r => new Compact30TreeRow
        {
            id = r.id, species = r.species, x_m = r.x_m, z_m = r.z_m,
            age_y = r.age_y, dbh_cm = r.dbh_cm, height_m = r.height_m, crown_m = r.crown_m
        }).ToList()
    };

    private static void Check(string name, bool passed, string detail = "")
    {
        checks++;
        if (!passed) failures++;
        Console.WriteLine((passed ? "PASS " : "FAIL ") + name + (passed || detail.Length == 0 ? "" : " — " + detail));
    }
}
