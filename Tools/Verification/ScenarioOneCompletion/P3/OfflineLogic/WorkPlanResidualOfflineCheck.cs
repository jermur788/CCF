// Offline logic check for P3 (UI/ResidualStand.cs, UI/CashOutlook.cs). Runs on
// the .NET runtime against a compiled Assembly-CSharp.dll, without the Unity
// Editor. Fixtures A-D and G-I of the P3 packet; E-F (clearance against
// Model 2 state) and "opening the Work Plan writes nothing" need live scene
// objects and are in WorkPlanResidualVerification.cs.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;

public static class WorkPlanResidualOfflineCheck
{
    private static int failures;

    public static int Main(string[] args)
    {
        string evidence = args.Length > 0 ? args[0] : ".";
        Stand(evidence);
        Cash();
        Console.WriteLine(failures == 0 ? "P3_OFFLINE_PASS" : $"P3_OFFLINE_FAIL failures={failures}");
        return failures == 0 ? 0 : 1;
    }

    private static void Check(bool condition, string name, string detail = "")
    {
        Console.WriteLine($"{(condition ? "PASS" : "FAIL")} {name} {detail}");
        if (!condition) failures++;
    }

    private static void Stand(string evidence)
    {
        string trees = Path.Combine(evidence, "input-y0-trees.csv"), json = Path.Combine(evidence, "input-residual-stand.json");
        var rows = File.ReadAllLines(trees).Skip(1).Select(l => l.Split(',')).ToList();
        string text = File.ReadAllText(json);
        string[] crop = Between(text, "\"cropTrees\": [");
        float F(string s) => float.Parse(s, CultureInfo.InvariantCulture);

        List<ResidualTree> Build(ICollection<string> fell, bool duplicateFirst = false)
        {
            var list = rows.Select(r => new ResidualTree(r[1], int.Parse(r[5], CultureInfo.InvariantCulture), F(r[7]), F(r[11]),
                fell.Contains(r[1]), crop.Contains(r[1]), r[17] == "1")).OrderBy(t => t.Id, StringComparer.Ordinal).ToList();
            return list;
        }
        ResidualStandResult Run(ICollection<string> fell) => ResidualStand.Compute(Build(fell), 0.16f, 8, 5f);

        // A. No work: before equals after; baseline equals the Unity snapshot (41.0994 m²/ha, 39.8678 m³).
        ResidualStandResult a = Run(new string[0]);
        Check(a.TreesBefore == 336 && a.TreesAfter == 336 && a.BasalAreaAfter == a.BasalAreaBefore && a.Pattern == OpeningPattern.NoRemoval
              && Mathf.Abs(a.BasalAreaBefore - 41.0994f) < 0.02f && Mathf.Abs(a.VolumeBefore - 39.8678f) < 0.01f
              && a.CropTrees == 16 && a.CropTreesKept == 16,
            "A-no-work", $"trees {a.TreesBefore} BA {a.BasalAreaBefore:F3} vol {a.VolumeBefore:F3} crop {a.CropTreesKept}/{a.CropTrees} seed {a.SeedTreesBefore}");

        // B-D against the Unity harness immediate results for T4, T3, T5 and T2.
        var unity = new Dictionary<string, (int retained, float ba, float vol)>
        {
            ["T4"] = (321, 38.5615f, 37.1234f), ["T3"] = (249, 28.0316f, 26.3472f),
            ["T5"] = (288, 35.5644f, 34.6481f), ["T2"] = (306, 36.2324f, 34.6810f), ["T1"] = (252, 34.8135f, 34.9633f)
        };
        var expectedPattern = new Dictionary<string, OpeningPattern>
        {
            ["T4"] = OpeningPattern.SpreadOut, ["T3"] = OpeningPattern.SeveralOpenings, ["T5"] = OpeningPattern.OneConcentratedOpening,
            ["T2"] = OpeningPattern.NoLabel, ["T1"] = OpeningPattern.NoLabel
        };
        var names = new Dictionary<string, string> { ["T4"] = "B-light-distributed", ["T3"] = "C-heavier", ["T5"] = "D-same-volume-concentrated", ["T2"] = "D-reference-release", ["T1"] = "extra-clean-up" };
        foreach (string id in new[] { "T4", "T3", "T5", "T2", "T1" })
        {
            string[] removed = Between(text, $"\"id\": \"{id}\"");
            ResidualStandResult r = Run(new HashSet<string>(removed));
            float removedBasal = Build(new HashSet<string>(removed)).Where(t => t.PlannedFell).Sum(t => ResidualStand.BasalAreaM2(t.DbhCm)) / 0.16f;
            (int retained, float ba, float vol) u = unity[id];
            Check(r.TreesAfter == u.retained && r.TreesBefore - r.TreesAfter == removed.Length
                  && Mathf.Abs(r.BasalAreaAfter - u.ba) < 0.02f && Mathf.Abs(r.VolumeAfter - u.vol) < 0.01f
                  && Mathf.Abs(r.BasalAreaBefore - r.BasalAreaAfter - removedBasal) < 0.001f && r.CropTreesKept == 16,
                $"{names[id]}-reconciles-with-unity-{id}", $"trees {r.TreesBefore}->{r.TreesAfter} BA {r.BasalAreaBefore:F2}->{r.BasalAreaAfter:F2} vol {r.VolumeAfter:F2}");
            Check(r.Pattern == expectedPattern[id], $"{names[id]}-pattern-{id}",
                $"{r.Pattern} agreement {r.PatternAgreement}/{r.PatternVariants} cells {r.CellsTouched} largest {r.LargestOpeningCells} ({r.LargestOpeningM2:F0} m², {r.ShareOfRemovalInLargestOpening:P0})");
        }

        // No double counting: listing a removal twice in the plan cannot remove two stems.
        string[] t2 = Between(text, "\"id\": \"T2\"");
        var doubled = new List<string>(t2); doubled.AddRange(t2.Take(5));
        Check(Run(new HashSet<string>(doubled)).TreesAfter == 306, "no-double-counting");

        // Same input, same output.
        string Describe(ResidualStandResult r) => $"{r.TreesAfter}|{r.BasalAreaAfter:R}|{r.VolumeAfter:R}|{r.Pattern}|{r.PatternAgreement}";
        Check(Describe(Run(new HashSet<string>(t2))) == Describe(Run(new HashSet<string>(t2.Reverse()))), "deterministic");
    }

    private static void Cash()
    {
        const long min = 250000;
        // G. A plan that leaves adequate cash: €12,000; thinning cost €2,500 with €250 revenue; planting €216.
        CashOutlook g = CashOutlook.Evaluate(new CashOutlookInput(1200000, min, 0, 0, 250000, 25000, 0, 21600));
        Check(g.State == CashOutlookState.Comfortable && g.ExpectedIfAllApproved == 953400 && g.Headroom == 703400, "G-adequate-cash",
            $"{g.State} expected {g.ExpectedIfAllApproved} headroom {g.Headroom}");

        // H. Crosses the threshold: €4,000 cash; pending planting €1,600 and a thinning costing €2,500 with €250 revenue.
        CashOutlook h = CashOutlook.Evaluate(new CashOutlookInput(400000, min, 0, 0, 250000, 25000, 0, 160000));
        Check(h.State == CashOutlookState.PendingCrossesMinimum && h.ExpectedIfAllApproved == 15000, "H-crosses-dead-end",
            $"{h.State} expected {h.ExpectedIfAllApproved}");

        // I. The harvest itself earns enough: €3,000 cash; cost €2,600, revenue €3,200 -> €3,600 expected; a cost-only
        // rule would wrongly predict €400.
        CashOutlook i = CashOutlook.Evaluate(new CashOutlookInput(300000, min, 0, 0, 260000, 320000, 0, 0));
        Check(i.State == CashOutlookState.Comfortable && i.ExpectedIfAllApproved == 360000, "I-revenue-avoids-false-warning",
            $"{i.State} expected {i.ExpectedIfAllApproved}");

        // Approved work alone crosses; cancelling remains possible.
        CashOutlook approved = CashOutlook.Evaluate(new CashOutlookInput(300000, min, 0, 0, 0, 0, 100000, 0));
        Check(approved.State == CashOutlookState.ApprovedCrossesMinimum, "approved-crosses", approved.State.ToString());

        // Already below the minimum with no approved harvest: no harvest can be commissioned.
        CashOutlook below = CashOutlook.Evaluate(new CashOutlookInput(240000, min, 0, 0, 0, 0, 0, 0));
        Check(below.State == CashOutlookState.BelowMinimumNow, "below-minimum-now", below.State.ToString());

        // Below the minimum but a harvest is already approved (its revenue is still coming): judged on the outcome.
        CashOutlook coming = CashOutlook.Evaluate(new CashOutlookInput(240000, min, 230000, 300000, 230000, 300000, 0, 0));
        Check(coming.State == CashOutlookState.Comfortable && coming.ExpectedAfterApproved == 310000, "approved-harvest-revenue-counts", coming.State.ToString());

        // Low cash but nothing planned and above the minimum: no warning merely because cash < some round number.
        CashOutlook quiet = CashOutlook.Evaluate(new CashOutlookInput(260000, min, 0, 0, 0, 0, 0, 0));
        Check(quiet.State == CashOutlookState.Comfortable, "no-warning-without-cause", quiet.State.ToString());

        // J. Windthrow salvage shares the authoritative grouped harvest quote and contractor minimum.
        var salvage = new ScenarioOneWorkOrder { type = ScenarioWorkType.SalvageDeadwood };
        var fell = new ScenarioOneWorkOrder { type = ScenarioWorkType.FellTree };
        var planting = new ScenarioOneWorkOrder { type = ScenarioWorkType.PlantJuvenile };
        Check(ScenarioOneManager.IsHarvestOrder(salvage) && ScenarioOneManager.IsHarvestOrder(fell)
              && !ScenarioOneManager.IsHarvestOrder(planting), "J-salvage-classified-with-harvest-orders");
    }

    private static string[] Between(string text, string start)
    {
        int s = text.IndexOf(start, StringComparison.Ordinal);
        if (s < 0) return new string[0];
        int open = text.IndexOf('[', start.EndsWith("[") ? s + start.Length - 1 : s + start.Length);
        int close = text.IndexOf(']', open);
        return text.Substring(open + 1, close - open - 1).Split(',').Select(x => x.Trim().Trim('"')).Where(x => x.Length > 0).ToArray();
    }
}
