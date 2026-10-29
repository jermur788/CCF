// Offline check for S1-D T-04 (AC-3): the guide is monotone from persisted history. Compiles the PRODUCTION
// ScenarioOneTeachingHistory.cs and ScenarioOneTeachingSequence.cs on the plain .NET SDK. Does not prove any Unity behaviour.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using K = ScenarioOneTeachingHistory.Kind;
using E = ScenarioOneTeachingHistory.Entry;

public static class TeachingHistoryOfflineCheck
{
    private static int checks, failures;
    private static void Check(bool ok, string name, string detail = "")
    {
        checks++; Console.WriteLine((ok ? "PASS " : "FAIL ") + name + (detail.Length > 0 ? " [" + detail + "]" : ""));
        if (!ok) failures++;
    }

    // A faithful run: felling (sold+deadwood) y1, review, plant both y2, review y3 (establishment seen), review y4.
    private static List<E> Run(int upto = 99)
    {
        var l = new List<E> {
            new E(1, 1, K.SoldFelling), new E(2, 1, K.DeadwoodFelling), new E(3, 2, K.Acknowledged),
            new E(4, 3, K.BeechPlanted), new E(5, 3, K.OakPlanted), new E(6, 4, K.Acknowledged), new E(7, 5, K.Acknowledged) };
        return l.Where(e => e.Id <= upto).ToList();
    }

    public static int Main(string[] args)
    {
        string root = args.Length > 0 ? args[0] : Environment.GetEnvironmentVariable("CCF_REPO_ROOT") ?? ".";
        Boundaries(); Drift(); Fallbacks(); Sequence(); Source(root);
        Console.WriteLine(failures == 0 ? "TEACHING_HISTORY_OFFLINE_PASS checks=" + checks + " NOT_UNITY_VERIFIED"
            : "TEACHING_HISTORY_OFFLINE_FAIL failures=" + failures + " checks=" + checks);
        return failures == 0 ? 0 : 1;
    }

    private static void Boundaries()
    {
        Check(ScenarioOneTeachingHistory.Floor(Run(2), true, true) == 0, "felling without a review -> no latch");
        Check(ScenarioOneTeachingHistory.Floor(Run(3), true, true) == 0, "release reviewed, nothing planted -> no latch (lesson 5 evidence is session-only)");
        Check(ScenarioOneTeachingHistory.Floor(Run(4), false, false) == ScenarioOneTeachingHistory.Recruitment, "first planting after review -> lesson 6");
        Check(ScenarioOneTeachingHistory.Floor(Run(5), false, false) == ScenarioOneTeachingHistory.Recruitment, "both planted, goals unrecorded -> stays lesson 6");
        Check(ScenarioOneTeachingHistory.Floor(Run(5), true, true) == ScenarioOneTeachingHistory.Establishment, "both planted and recorded -> lesson 7");
        Check(ScenarioOneTeachingHistory.Floor(Run(6), true, true) == ScenarioOneTeachingHistory.Transformation, "review a year after planting -> lesson 8");
        Check(ScenarioOneTeachingHistory.Floor(Run(), true, true) == ScenarioOneTeachingHistory.Transformation, "later reviews keep lesson 8");
        // Same-year review does not establish: ack year must exceed the planting year.
        var sameYear = Run(5); sameYear.Add(new E(6, 3, K.Acknowledged));
        Check(ScenarioOneTeachingHistory.Floor(sameYear, true, true) == ScenarioOneTeachingHistory.Establishment, "same-year review does not complete lesson 7");
    }

    // Drift after completion cannot lower the floor: the helper never sees Crop Trees, shares or footprint counts, and later events only add.
    private static void Drift()
    {
        var late = Run(); late.Add(new E(8, 6, K.SoldFelling)); late.Add(new E(9, 6, K.DeadwoodFelling)); late.Add(new E(10, 7, K.OtherPlanted));
        Check(ScenarioOneTeachingHistory.Floor(late, true, true) == ScenarioOneTeachingHistory.Transformation, "later felling and later planting never lower the floor");
        var outside = Run(); outside.Add(new E(8, 6, K.SoldFelling, false));
        Check(ScenarioOneTeachingHistory.Floor(outside, true, true) == ScenarioOneTeachingHistory.Transformation, "later out-of-footprint felling is irrelevant");
        // Prefix monotonicity: extending the log never lowers the floor.
        int prev = 0; bool mono = true;
        for (int n = 0; n <= 7; n++) { int f = ScenarioOneTeachingHistory.Floor(Run(n), true, true); if (f < prev) mono = false; prev = f; }
        Check(mono, "floor is monotone over every prefix of the log");
        // Repeated cycles: a fourth cycle repeats the same shape without changing the answer.
        var cycles = Run();
        for (int c = 0; c < 3; c++) { int b = 10 + c * 10; cycles.Add(new E(b, 6 + c, K.SoldFelling)); cycles.Add(new E(b + 1, 6 + c, K.DeadwoodFelling)); cycles.Add(new E(b + 2, 7 + c, K.Acknowledged)); }
        Check(ScenarioOneTeachingHistory.Floor(cycles, true, true) == ScenarioOneTeachingHistory.Transformation, "fourth-cycle symmetry holds");
    }

    private static void Fallbacks()
    {
        Check(ScenarioOneTeachingHistory.Floor(null, true, true) == 0, "null log -> 0");
        Check(ScenarioOneTeachingHistory.Floor(new List<E>(), true, true) == 0, "empty log -> 0");
        var outside = new List<E> { new E(1, 1, K.SoldFelling, false), new E(2, 1, K.DeadwoodFelling), new E(3, 2, K.Acknowledged), new E(4, 3, K.BeechPlanted) };
        Check(ScenarioOneTeachingHistory.Floor(outside, true, true) == 0, "sold felling outside the footprint does not prove the release");
        var reviewFirst = new List<E> { new E(1, 1, K.Acknowledged), new E(2, 1, K.SoldFelling), new E(3, 1, K.DeadwoodFelling), new E(4, 2, K.BeechPlanted) };
        Check(ScenarioOneTeachingHistory.Floor(reviewFirst, true, true) == 0, "a review that preceded the felling proves nothing");
        var plantFirst = new List<E> { new E(1, 1, K.BeechPlanted), new E(2, 1, K.OakPlanted), new E(3, 2, K.SoldFelling), new E(4, 2, K.DeadwoodFelling), new E(5, 3, K.Acknowledged) };
        Check(ScenarioOneTeachingHistory.Floor(plantFirst, true, true) == 0, "planting before the release review proves nothing");
        var lateExtra = Run(5); lateExtra.Add(new E(6, 4, K.OtherPlanted)); lateExtra.Add(new E(7, 4, K.Acknowledged));
        Check(ScenarioOneTeachingHistory.Floor(lateExtra, true, true) == ScenarioOneTeachingHistory.Establishment, "a planting in the review's own year blocks lesson 8 at that review");
    }

    private static ScenarioOneTeachingSequence.Facts Open() => new ScenarioOneTeachingSequence.Facts { AccessOpen = true, AccessReviewed = true };

    private static void Sequence()
    {
        string[] ids = { "access", "future-trees", "release", "habitat", "response", "recruitment", "establishment", "transformation" };
        Check(ScenarioOneTeachingSequence.Lessons.Select(l => l.Id).SequenceEqual(ids), "all eight stable lesson IDs unchanged and in order");
        // Pre-completion unchanged: with floor 0, Current == Raw over a grid of facts.
        int rows = 0, bad = 0;
        foreach (bool access in new[] { false, true }) foreach (bool reviewed in new[] { false, true }) foreach (int crops in new[] { 0, 3 })
        foreach (bool planned in new[] { false, true }) foreach (bool executed in new[] { false, true }) foreach (bool dw in new[] { false, true })
        foreach (bool rr in new[] { false, true }) foreach (bool beech in new[] { false, true })
        {
            var f = new ScenarioOneTeachingSequence.Facts { AccessOpen = access, AccessReviewed = reviewed, CropTrees = crops, ReleasePlanned = planned,
                ReleaseExecuted = executed, DeadwoodExecuted = dw, ReleaseReviewed = rr, ReviewedTwoCycles = rr, BeechEstablished = beech, OakEstablished = beech };
            rows++; if (ScenarioOneTeachingSequence.Current(f) != ScenarioOneTeachingSequence.Raw(f)) bad++;
        }
        Check(bad == 0, "floor 0: Current equals the earlier rules", rows + " rows");
        // Drift after completion: current facts regress (crop tree lost, release share drift, felling) yet the guide holds.
        var drift = Open(); drift.CropTrees = 0; drift.ReleaseExecuted = false; drift.HistoricalFloor = ScenarioOneTeachingHistory.Establishment;
        Check(ScenarioOneTeachingSequence.Raw(drift) < 6 && ScenarioOneTeachingSequence.Current(drift) == 6, "crop-tree loss and share drift cannot pull the guide behind lesson 7");
        drift.HistoricalFloor = ScenarioOneTeachingHistory.Transformation; drift.AccessOpen = false;
        Check(ScenarioOneTeachingSequence.Current(drift) == 7, "access reopened by new work cannot pull the guide behind lesson 8");
        // The floor never lowers an already-further guide, and cannot exceed the last lesson.
        var ahead = Open(); ahead.HistoricalFloor = 5;
        ahead.CropTrees = 3; ahead.ReleaseExecuted = ahead.DeadwoodExecuted = ahead.ReleaseReviewed = ahead.ReviewedTwoCycles = ahead.AreaReleased = true;
        ahead.BeechEstablished = ahead.OakEstablished = true; ahead.PlantingYear = 2; ahead.Year = 4; ahead.EstablishmentReviewed = true;
        Check(ScenarioOneTeachingSequence.Current(ahead) == 7, "a further current guide is not lowered by a lower floor");
        ahead.HistoricalFloor = 99;
        Check(ScenarioOneTeachingSequence.Current(ahead) == 7, "an out-of-range floor is clamped");
    }

    private static void Source(string root)
    {
        string h = string.Join("\n", File.ReadAllLines(Path.Combine(root, "Assets/ForestPrototype/UI/ScenarioOneTeachingHistory.cs")).Select(l => { int c = l.IndexOf("//", StringComparison.Ordinal); return c >= 0 ? l.Substring(0, c) : l; }));
        Check(!h.Contains("PlayerPrefs") && !h.Contains("UnityEngine") && !h.Contains("rackReview"), "helper reads no PlayerPrefs, Unity or session [J]/[K] state");
        string v = File.ReadAllText(Path.Combine(root, "Assets/ForestPrototype/UI/LearningObjectivesView.Sequence.cs"));
        Check(v.Contains("HistoricalFloor = HistoricalFloor(m,") && !v.Contains("PlayerPrefs"), "view wires the floor from the manager and uses no PlayerPrefs");
        string s = File.ReadAllText(Path.Combine(root, "Assets/ForestPrototype/UI/ScenarioOneTeachingSequence.cs"));
        Check(s.Contains("Math.Max(Raw(f)"), "Current is max(Raw, floor)");
    }
}
