// Offline logic check for UI/CropTreeCompetition.cs (P2). Runs on the .NET
// runtime against a compiled Assembly-CSharp.dll, without the Unity Editor.
// It exercises only pure arithmetic (Vector2.Distance, Mathf, HegyiTerm).
// The in-Editor gate CropTreeCompetitorVerification.cs checks the same
// behaviour against live ForestTree objects and GetCompetitionIndex.
//
// Usage: run_offline_check.py builds and runs it. Prints P2_OFFLINE_* tokens.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;

public static class CropTreeCompetitionOfflineCheck
{
    private static int failures;

    public static int Main(string[] args)
    {
        string evidence = args.Length > 0 ? args[0] : ".";
        Fixtures();
        MultiCrop();
        Year0(evidence);
        Performance();
        Console.WriteLine(failures == 0 ? "P2_OFFLINE_PASS" : $"P2_OFFLINE_FAIL failures={failures}");
        return failures == 0 ? 0 : 1;
    }

    private static void Check(bool condition, string name, string detail = "")
    {
        Console.WriteLine($"{(condition ? "PASS" : "FAIL")} {name} {detail}");
        if (!condition) failures++;
    }

    // Independent re-statement of ForestEcologyController.UpdateCompetition's
    // inner loop (inline expression, same pair set, id order).
    private static float AuthoritativeCi(IReadOnlyList<CompetitionTree> trees, int target, Func<CompetitionTree, bool> include = null)
    {
        float ci = 0f;
        for (int j = 0; j < trees.Count; j++)
        {
            if (j == target || (include != null && !include(trees[j]))) continue;
            float distance = Vector2.Distance(trees[target].Position, trees[j].Position);
            if (distance > 8f) continue;
            ci += (trees[j].DbhCm / Mathf.Max(1f, trees[target].DbhCm)) / Mathf.Max(0.5f, distance);
        }
        return ci;
    }

    private static List<CompetitionTree> Sorted(IEnumerable<CompetitionTree> trees)
        => trees.OrderBy(t => t.Id, StringComparer.Ordinal).ToList();

    // Crop Tree A, a small suppressed neighbour B close by, a larger neighbour C
    // a little further away, a ring of ordinary neighbours, and a tree D well
    // outside the 8 m radius.
    private static List<CompetitionTree> FixtureStand(bool markB, bool markC, bool markD)
    {
        var trees = new List<CompetitionTree>
        {
            new CompetitionTree("A", new Vector2(0f, 0f), 25f, false, true),
            new CompetitionTree("B", new Vector2(1.6f, 0.4f), 7f, markB, false),
            new CompetitionTree("C", new Vector2(-2.4f, 1.5f), 30f, markC, false),
            new CompetitionTree("D", new Vector2(15f, 0f), 28f, markD, false),
        };
        for (int i = 0; i < 8; i++)
        {
            float angle = i * Mathf.PI / 4f + 0.3f;
            trees.Add(new CompetitionTree("N" + i, new Vector2(Mathf.Cos(angle) * 4.5f, Mathf.Sin(angle) * 4.5f), 18f + i, false, false));
        }
        return Sorted(trees);
    }

    private static void Fixtures()
    {
        List<CompetitionTree> stand = FixtureStand(false, false, false);
        int a = stand.FindIndex(t => t.Id == "A");
        CropTreeCompetitionReport report = CropTreeCompetition.Analyse(stand, a);
        float sum = report.Ranked.Sum(e => e.Contribution);
        float authoritative = AuthoritativeCi(stand, a);
        Check(Mathf.Abs(sum - report.CompetitionNow) < 1e-5f && Mathf.Abs(report.CompetitionNow - authoritative) < 1e-5f,
            "1-contributions-reconcile", $"sum={sum:F6} total={report.CompetitionNow:F6} authoritative={authoritative:F6}");
        Check(Mathf.Abs(report.Ranked.Sum(e => e.Share) - 1f) < 1e-5f, "1b-shares-sum-to-one");

        int rankB = report.Ranked.FindIndex(e => e.Id == "B");
        int rankC = report.Ranked.FindIndex(e => e.Id == "C");
        CompetitorEntry b = report.Ranked[rankB], c = report.Ranked[rankC];
        Check(c.Contribution > 2f * b.Contribution && rankC < rankB, "4/5-suppressed-low-larger-high",
            $"B(7cm,{b.DistanceM:F1}m) {b.Share:P1} rank {rankB + 1} {b.Strength}; C(30cm,{c.DistanceM:F1}m) {c.Share:P1} rank {rankC + 1} {c.Strength}");
        // Bands are relative to this tree's own total, so in an open stand a close
        // small tree can still read Moderate. The requirement is the ordering.
        Check(rankC == 0 && c.Strength == CompetitorStrength.Strong && b.Strength < c.Strength
              && rankB >= report.NeighbourCount / 2,
            "4/5-bands", $"C {c.Strength} rank 1; B {b.Strength} rank {rankB + 1} of {report.NeighbourCount}");
        Check(!report.Ranked.Any(e => e.Id == "D"), "outside-radius-excluded");

        // 2. Marked removals lower the preview by exactly their contribution.
        List<CompetitionTree> markedC = FixtureStand(false, true, false);
        CropTreeCompetitionReport withC = CropTreeCompetition.Analyse(markedC, markedC.FindIndex(t => t.Id == "A"));
        float expected = AuthoritativeCi(markedC, 0, t => !t.PlannedFell);
        Check(Mathf.Abs(withC.CompetitionAfterPlanned - expected) < 1e-5f
              && Mathf.Abs(withC.CompetitionNow - report.CompetitionNow) < 1e-6f
              && Mathf.Abs(withC.CompetitionNow - withC.CompetitionAfterPlanned - c.Contribution) < 1e-5f,
            "2-marked-removal-preview", $"{withC.CompetitionNow:F3} -> {withC.CompetitionAfterPlanned:F3} ({withC.ChangeFraction:P1})");

        // 3. A marked tree outside the radius does not change the preview.
        List<CompetitionTree> markedD = FixtureStand(false, false, true);
        CropTreeCompetitionReport withD = CropTreeCompetition.Analyse(markedD, markedD.FindIndex(t => t.Id == "A"));
        Check(withD.CompetitionAfterPlanned == withD.CompetitionNow && !withD.HasPlannedRemovals, "3-unrelated-removal-no-change");

        // 6. Determinism: identical input -> identical report; shuffled input sorted -> identical.
        string first = Describe(CropTreeCompetition.Analyse(stand, a));
        var shuffled = stand.OrderBy(t => t.Id.GetHashCode() ^ 0x5f3759df).ToList();
        List<CompetitionTree> resorted = Sorted(shuffled);
        string second = Describe(CropTreeCompetition.Analyse(resorted, resorted.FindIndex(t => t.Id == "A")));
        Check(first == second, "6-deterministic");
    }

    private static string Describe(CropTreeCompetitionReport r)
        => string.Join(";", r.Ranked.Select(e => $"{e.Id}:{e.Contribution.ToString("R", CultureInfo.InvariantCulture)}"))
           + "|" + r.CompetitionNow.ToString("R", CultureInfo.InvariantCulture);

    // One Fell-marked tree between two Crop Trees, one beside a single Crop
    // Tree, one far from every Crop Tree.
    private static void MultiCrop()
    {
        var trees = Sorted(new[]
        {
            new CompetitionTree("K1", new Vector2(0f, 0f), 22f, false, true),
            new CompetitionTree("K2", new Vector2(6f, 0f), 22f, false, true),
            new CompetitionTree("F-shared", new Vector2(3f, 0.5f), 20f, true, false),
            new CompetitionTree("F-single", new Vector2(-2f, -2f), 18f, true, false),
            new CompetitionTree("F-none", new Vector2(30f, 30f), 18f, true, false),
            new CompetitionTree("X", new Vector2(2f, 4f), 16f, false, false),
        });
        CropTreeReleaseSummary summary = CropTreeCompetition.Summarise(trees);
        Check(summary.PlannedFells == 3 && summary.PlannedNearACropTree == 2 && summary.PlannedNearSeveralCropTrees == 1,
            "multi-crop-each-stem-once", $"fells={summary.PlannedFells} near={summary.PlannedNearACropTree} several={summary.PlannedNearSeveralCropTrees}");
        Check(summary.CropTrees == 2 && summary.CropTreesReleased == 2 && summary.MeanCompetitionAfter < summary.MeanCompetitionNow,
            "multi-crop-release", $"{summary.MeanCompetitionNow:F3} -> {summary.MeanCompetitionAfter:F3}");
        CropTreeCompetitionReport k1 = CropTreeCompetition.Analyse(trees, trees.FindIndex(t => t.Id == "K1"));
        CropTreeCompetition.CountNearbyCropTrees(trees, k1, CropTreeCompetition.ListedCount);
        CompetitorEntry shared = k1.Ranked.First(e => e.Id == "F-shared");
        Check(shared.OtherCropTreesNearby == 1, "multi-crop-also-near", $"F-shared also near {shared.OtherCropTreesNearby} other Crop Tree(s)");
    }

    // Year-0 Scenario One stand as exported from Unity by the pedagogy harness.
    private static void Year0(string evidence)
    {
        string path = Path.Combine(evidence, "input-y0-trees.csv");
        string json = Path.Combine(evidence, "input-residual-stand.json");
        if (!File.Exists(path) || !File.Exists(json))
        {
            Check(false, "year0-inputs-present", path);
            return;
        }
        var rows = File.ReadAllLines(path).Skip(1).Select(l => l.Split(',')).ToList();
        string jsonText = File.ReadAllText(json);
        string[] crop = Between(jsonText, "\"cropTrees\": [", "]");
        var unityCi = rows.ToDictionary(r => r[1], r => float.Parse(r[12], CultureInfo.InvariantCulture));
        List<CompetitionTree> Build(ICollection<string> fell) => Sorted(rows.Select(r => new CompetitionTree(r[1],
            new Vector2(float.Parse(r[3], CultureInfo.InvariantCulture), float.Parse(r[4], CultureInfo.InvariantCulture)),
            float.Parse(r[7], CultureInfo.InvariantCulture), fell.Contains(r[1]), crop.Contains(r[1]))));

        List<CompetitionTree> stand = Build(new string[0]);
        int within = 0;
        for (int i = 0; i < stand.Count; i++)
            if (Mathf.Abs(CropTreeCompetition.Analyse(stand, i).CompetitionNow - unityCi[stand[i].Id]) < 0.01f) within++;
        Check(within >= 320, "year0-ci-matches-unity-export", $"{within}/{stand.Count} within 0.01 (positions exported at 2 dp)");

        int p0707 = stand.FindIndex(t => t.Id == "P0707");
        CropTreeCompetitionReport r = CropTreeCompetition.Analyse(stand, p0707);
        string top = string.Join(" ", r.Ranked.Take(5).Select(e => $"{e.Id}:{e.Share * 100f:F1}%"));
        Check(r.NeighbourCount == 48 && string.Join(",", r.Ranked.Take(5).Select(e => e.Id)) == "P0706,P0607,P0807,P0806,P0708",
            "year0-P0707-top5", $"n={r.NeighbourCount} {top} rest={(1f - r.ShareOfFirst(5)) * 100f:F0}%");
        Console.WriteLine("INFO P0707 distribution: " + CropTreeCompetition.DistributionSentence(r));

        foreach ((string id, float before, float after) in new[] { ("T2", 6.0924f, 4.8916f), ("T5", 6.0924f, 5.1171f) })
        {
            string[] removed = Between(jsonText, $"\"id\": \"{id}\"", "]}").SelectMany(s => s.Split(',')).ToArray();
            CropTreeReleaseSummary summary = CropTreeCompetition.Summarise(Build(new HashSet<string>(removed)));
            Check(Mathf.Abs(summary.MeanCompetitionNow - before) < 0.002f && Mathf.Abs(summary.MeanCompetitionAfter - after) < 0.002f,
                $"year0-{id}-release-matches-unity-harness",
                $"{summary.MeanCompetitionNow:F4} -> {summary.MeanCompetitionAfter:F4} released={summary.CropTreesReleased}/16 near={summary.PlannedNearACropTree}/{summary.PlannedFells} several={summary.PlannedNearSeveralCropTrees}");
        }
    }

    private static string[] Between(string text, string start, string end)
    {
        int s = text.IndexOf(start, StringComparison.Ordinal);
        if (s < 0) return new string[0];
        int open = text.IndexOf('[', s + (start.EndsWith("[") ? start.Length - 1 : start.Length));
        int close = text.IndexOf(']', open);
        return text.Substring(open + 1, close - open - 1).Split(',').Select(x => x.Trim().Trim('"')).Where(x => x.Length > 0).ToArray();
    }

    // Synthetic grids at 336 / 1,300 / 3,000 / 5,000 stems with 16 / 40 / 80 / 120 Crop Trees.
    private static void Performance()
    {
        foreach ((int n, int crops) in new[] { (336, 16), (1300, 40), (3000, 80), (5000, 120) })
        {
            var rng = new System.Random(20261008);
            float side = 40f * Mathf.Sqrt(n / 336f);
            var trees = new List<CompetitionTree>(n);
            for (int i = 0; i < n; i++)
                trees.Add(new CompetitionTree("T" + i.ToString("D5"), new Vector2((float)rng.NextDouble() * side, (float)rng.NextDouble() * side),
                    10f + (float)rng.NextDouble() * 15f, i % 9 == 0, i % (n / crops) == 0));
            trees = Sorted(trees);
            CropTreeCompetition.Analyse(trees, 0); // warm-up
            var watch = Stopwatch.StartNew();
            for (int k = 0; k < 20; k++) CropTreeCompetition.Analyse(trees, k * 7 % n);
            double inspectMs = watch.Elapsed.TotalMilliseconds / 20.0;
            watch.Restart();
            for (int k = 0; k < 5; k++) CropTreeCompetition.Summarise(trees);
            double summaryMs = watch.Elapsed.TotalMilliseconds / 5.0;
            Console.WriteLine($"PERF stems={n} crops={trees.Count(t => t.CropTree)} inspectMs={inspectMs:F3} markChangeSummaryMs={summaryMs:F2}");
        }
    }
}
