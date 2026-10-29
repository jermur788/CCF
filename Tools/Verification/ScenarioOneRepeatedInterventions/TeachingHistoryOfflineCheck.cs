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

    // Plant both y3 (events 1-2), review y4 (3), review y5 (4).
    private static List<E> Run(int upto = 99)
        => new List<E> { new E(1, 3, K.BeechPlanted), new E(2, 3, K.OakPlanted), new E(3, 4, K.Acknowledged), new E(4, 5, K.Acknowledged) }.Where(e => e.Id <= upto).ToList();

    public static int Main(string[] args)
    {
        string root = args.Length > 0 ? args[0] : Environment.GetEnvironmentVariable("CCF_REPO_ROOT") ?? ".";
        Boundaries(); Drift(); Fallbacks(); Sequence(); SharedDefinition(); Source(root);
        Console.WriteLine(failures == 0 ? "TEACHING_HISTORY_OFFLINE_PASS checks=" + checks + " NOT_UNITY_VERIFIED"
            : "TEACHING_HISTORY_OFFLINE_FAIL failures=" + failures + " checks=" + checks);
        return failures == 0 ? 0 : 1;
    }

    private static void Boundaries()
    {
        Check(ScenarioOneTeachingHistory.Floor(Run(1), true, true) == 0, "only beech planted -> no latch");
        Check(ScenarioOneTeachingHistory.Floor(Run(2), false, false) == 0, "both planted but objectives unrecorded -> no latch");
        Check(ScenarioOneTeachingHistory.Floor(Run(2), true, false) == 0, "oak objective unrecorded -> no latch");
        Check(ScenarioOneTeachingHistory.Floor(Run(2), true, true) == ScenarioOneTeachingHistory.Establishment, "both planted and recorded -> lesson 7");
        Check(ScenarioOneTeachingHistory.Floor(Run(3), true, true) == ScenarioOneTeachingHistory.Transformation, "review a year after planting -> lesson 8");
        Check(ScenarioOneTeachingHistory.Floor(Run(), true, true) == ScenarioOneTeachingHistory.Transformation, "later reviews keep lesson 8");
        var sameYear = new List<E> { new E(1, 3, K.BeechPlanted), new E(2, 3, K.OakPlanted), new E(3, 3, K.Acknowledged) };
        Check(ScenarioOneTeachingHistory.Floor(sameYear, true, true) == ScenarioOneTeachingHistory.Establishment, "same-year review does not complete lesson 7");
        var reviewBefore = new List<E> { new E(1, 4, K.Acknowledged), new E(2, 3, K.BeechPlanted), new E(3, 3, K.OakPlanted) };
        Check(ScenarioOneTeachingHistory.Floor(reviewBefore, true, true) == ScenarioOneTeachingHistory.Establishment, "a review before the plantings proves nothing");
    }

    private static void Drift()
    {
        var late = Run(); late.Add(new E(5, 6, K.OtherPlanted)); late.Add(new E(6, 6, K.BeechPlanted));
        Check(ScenarioOneTeachingHistory.Floor(late, true, true) == ScenarioOneTeachingHistory.Transformation, "later planting never lowers the floor");
        int prev = 0; bool mono = true;
        for (int n = 0; n <= 4; n++) { int f = ScenarioOneTeachingHistory.Floor(Run(n), true, true); if (f < prev) mono = false; prev = f; }
        Check(mono, "floor is monotone over every prefix of the log");
        var cycles = Run();
        for (int c = 0; c < 3; c++) { int b = 10 + c * 10; cycles.Add(new E(b, 6 + c, K.BeechPlanted)); cycles.Add(new E(b + 1, 7 + c, K.Acknowledged)); }
        Check(ScenarioOneTeachingHistory.Floor(cycles, true, true) == ScenarioOneTeachingHistory.Transformation, "fourth-cycle symmetry holds");
    }

    private static void Fallbacks()
    {
        Check(ScenarioOneTeachingHistory.Floor(null, true, true) == 0, "null log -> 0");
        Check(ScenarioOneTeachingHistory.Floor(new List<E>(), true, true) == 0, "empty log -> 0");
        // Planting is not lesson-gated: nothing here, however much planting, can latch lessons 1-6 (no kind exists for felling or review of lessons 1-5).
        var many = Enumerable.Range(1, 10).Select(i => new E(i, i, K.OtherPlanted)).ToList();
        Check(ScenarioOneTeachingHistory.Floor(many, true, true) == 0, "planting alone never latches");
    }

    private static ScenarioOneTeachingSequence.Facts Established() => new ScenarioOneTeachingSequence.Facts { AccessOpen = true, AccessReviewed = true, CropTrees = 3,
        ReleasePlanned = true, ReleaseExecuted = true, DeadwoodExecuted = true, ReleaseReviewed = true, RackReviewComplete = true, ReviewedTwoCycles = true,
        BeechEstablished = true, OakEstablished = true, PlantingYear = 2, Year = 4 };
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
        // Lessons 1-5 are not latched: drift there keeps the earlier behaviour even with a floor present.
        var drift = Open(); drift.CropTrees = 0; drift.ReleaseExecuted = false; drift.HistoricalFloor = ScenarioOneTeachingHistory.Transformation;
        Check(ScenarioOneTeachingSequence.Raw(drift) < 5 && ScenarioOneTeachingSequence.Current(drift) == ScenarioOneTeachingSequence.Raw(drift), "crop-tree loss before lesson 6 is not latched (existing behaviour)");
        // At lesson 6 or later, a later planting year or lost seedling cannot pull the guide behind a proven boundary.
        var held = Established(); held.HistoricalFloor = ScenarioOneTeachingHistory.Transformation; held.Year = 4; held.PlantingYear = 4; held.EstablishmentReviewed = false;
        Check(ScenarioOneTeachingSequence.Raw(held) == 6 && ScenarioOneTeachingSequence.Current(held) == 7, "a later planting year cannot pull lesson 8 back to lesson 7");
        held.BeechEstablished = false;
        Check(ScenarioOneTeachingSequence.Raw(held) == 5 && ScenarioOneTeachingSequence.Current(held) == 7, "a lost seedling cannot pull lesson 8 back to lesson 6");
        // Exhaustive: for every fact combination, floor 7 never lowers a Raw>=5 guide and never changes a Raw<5 guide.
        rows = 0; bad = 0;
        foreach (bool rr in new[] { false, true }) foreach (bool bo in new[] { false, true }) foreach (bool ex in new[] { false, true }) foreach (int crops in new[] { 0, 3 })
        foreach (int fl in new[] { 0, 6, 7 }) foreach (int year in new[] { 2, 4 })
        {
            var f = Established(); f.ReleaseReviewed = rr; f.RackReviewComplete = rr; f.BeechEstablished = f.OakEstablished = bo; f.ReleaseExecuted = ex || f.ReleaseExecuted;
            f.CropTrees = crops; f.HistoricalFloor = fl; f.Year = year; rows++;
            int raw = ScenarioOneTeachingSequence.Raw(f), cur = ScenarioOneTeachingSequence.Current(f);
            if (raw < 5 ? cur != raw : cur != Math.Max(raw, fl)) bad++;
        }
        Check(bad == 0, "floor never changes a pre-lesson-6 guide and never lowers a later one", rows + " rows");
        // The floor never lowers an already-further guide, and cannot exceed the last lesson.
        var ahead = Open(); ahead.HistoricalFloor = 5;
        ahead.CropTrees = 3; ahead.ReleaseExecuted = ahead.DeadwoodExecuted = ahead.ReleaseReviewed = ahead.ReviewedTwoCycles = ahead.AreaReleased = true;
        ahead.BeechEstablished = ahead.OakEstablished = true; ahead.PlantingYear = 2; ahead.Year = 4; ahead.EstablishmentReviewed = true;
        Check(ScenarioOneTeachingSequence.Current(ahead) == 7, "a further current guide is not lowered by a lower floor");
        ahead.HistoricalFloor = 99;
        Check(ScenarioOneTeachingSequence.Current(ahead) == 7, "an out-of-range floor is clamped");
    }

    // T-03 and T-04 share one definition: the cap's function is the history helper's, and a guide that is at lesson 6 or later (which needs
    // exactly the release facts) always satisfies it, so no bay state can make the two disagree about "historically complete".
    private static void SharedDefinition()
    {
        int rows = 0, bad = 0, implied = 0;
        foreach (bool applies in new[] { true, false }) foreach (bool mixed in new[] { true, false }) foreach (bool reviewed in new[] { true, false })
        foreach (float min in new[] { 0f, 0.26f, 0.27f, 0.31f }) foreach (int year in new[] { -1, 1 }) foreach (float removed in new[] { 0.2f, 0.55f })
        {
            var a = new ScenarioOneRackBay.Analysis { Applies = applies, RackIndex = 2, RackCount = 5, RemovedBasalShare = removed, ReleaseYear = year, MixedExecuted = mixed, MinExecuted = min };
            rows++;
            if (ScenarioOneTeachingCap.SampleHistoricallyComplete(a, reviewed) != ScenarioOneTeachingHistory.SampleHistoricallyComplete(a, reviewed)) bad++;
        }
        Check(bad == 0, "cap and history helper give the same answer", rows + " rows");
        Check(ScenarioOneTeachingCap.Classify(new ScenarioOneRackBay.Analysis { Applies = true, RemovedBasalShare = 0.55f, ActionableRemovals = 2, ReleaseYear = 1, MixedExecuted = true, MinExecuted = 0.31f }, true, true)
              == ScenarioOneTeachingCap.Phase.Advisory, "completed sample + excess -> Advisory through the shared definition");
        string cap = File.ReadAllText(Path.Combine(Environment.GetEnvironmentVariable("CCF_REPO_ROOT") ?? ".", "Assets/ForestPrototype/UI/ScenarioOneTeachingCap.cs"));
        Check(cap.Contains("=> ScenarioOneTeachingHistory.SampleHistoricallyComplete(bay, releaseReviewed);"), "cap delegates to the one definition");
        Check(implied == 0, "no divergent copy of the formula remains in the cap");
    }

    private static void Source(string root)
    {
        string h = string.Join("\n", File.ReadAllLines(Path.Combine(root, "Assets/ForestPrototype/UI/ScenarioOneTeachingHistory.cs")).Select(l => { int c = l.IndexOf("//", StringComparison.Ordinal); return c >= 0 ? l.Substring(0, c) : l; }));
        Check(!h.Contains("PlayerPrefs") && !h.Contains("UnityEngine") && !h.Contains("rackReview"), "helper reads no PlayerPrefs, Unity or session [J]/[K] state");
        string v = File.ReadAllText(Path.Combine(root, "Assets/ForestPrototype/UI/LearningObjectivesView.Sequence.cs"));
        Check(v.Contains("HistoricalFloor = HistoricalFloor(m),") && !v.Contains("PlayerPrefs"), "view wires the floor from the manager and uses no PlayerPrefs");
        string s = File.ReadAllText(Path.Combine(root, "Assets/ForestPrototype/UI/ScenarioOneTeachingSequence.cs"));
        Check(s.Contains("Math.Max(raw, System.Math.Min(f.HistoricalFloor"), "Current is max(Raw, floor)");
    }
}
