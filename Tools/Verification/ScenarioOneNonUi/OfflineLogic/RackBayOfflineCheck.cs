// Offline check for the rack-area lesson (Objectives 2-5): the analysis (ScenarioOneRackBay), the lesson order
// (ScenarioOneTeachingSequence.Current) and the wording (ScenarioOneRackBayText). Pure logic on plain .NET against a realistic Enlarged80
// starting stand (core-stand-replica.csv: the 336 legacy-core trees, positions and diameters reproduced from the generator).
//
// It proves the rules: that the old trivial route no longer completes Objectives 2-5, that progress belongs to the chosen rack area, that
// the answer does not depend on listing order and that planned work, finished work and growth behave as the lesson says. It does not prove
// the live game feeds these rules the right data: the Unity gate RackBayLessonVerification.cs does.
//
// Prints NONUI_RACKBAY_PASS / NONUI_RACKBAY_FAIL.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

public static class RackBayOfflineCheck
{
    private static int failures, checks;
    private static void Check(bool ok, string name, string detail = "")
    {
        checks++;
        Console.WriteLine($"{(ok ? "PASS" : "FAIL")} {name} {detail}");
        if (!ok) failures++;
    }

    // The authored five-rack layout of the 80 m stand (rows 1.9 m apart, every eighth row), the centre lines of its racks.
    private static readonly float[] Lines = { -30.8f, -15.2f, 0.0f, 15.2f, 30.8f };

    private sealed class Stand
    {
        public List<ScenarioOneRackBay.Tree> Trees = new List<ScenarioOneRackBay.Tree>();
        public List<ScenarioOneRackBay.Order> Orders = new List<ScenarioOneRackBay.Order>();
        public ScenarioOneRackBay.Analysis Analyse() => ScenarioOneRackBay.Analyse(Lines, Trees, Orders);
        public int IndexOf(string id) => Trees.FindIndex(t => t.Id == id);
        public ScenarioOneRackBay.Tree Get(string id) => Trees[IndexOf(id)];
        public void Replace(string id, Func<ScenarioOneRackBay.Tree, ScenarioOneRackBay.Tree> change) { int i = IndexOf(id); Trees[i] = change(Trees[i]); }
        public void Crop(string id, bool on) => Replace(id, t => new ScenarioOneRackBay.Tree(t.Id, t.X, t.Z, t.DbhCm, t.Living, on, t.Marked));
        public void Mark(string id, bool on) => Replace(id, t => new ScenarioOneRackBay.Tree(t.Id, t.X, t.Z, t.DbhCm, t.Living, t.Crop, on));
        public void Plan(string id, ScenarioOneRackBay.Disposition d) { Orders.RemoveAll(o => o.TargetId == id); Orders.Add(new ScenarioOneRackBay.Order(id, true, false, d, -1)); }
        // The year passes: the planned tree becomes a stump with the diameter it was felled at, and its order is completed.
        public void Execute(string id, int year)
        {
            ScenarioOneRackBay.Order planned = Orders.First(o => o.TargetId == id && o.Open);
            Orders.Remove(planned);
            Orders.Add(new ScenarioOneRackBay.Order(id, false, true, planned.Disposition, year));
            Replace(id, t => new ScenarioOneRackBay.Tree(t.Id, t.X, t.Z, t.DbhCm, false, false));
        }
        public void ExecuteAll(int year) { foreach (string id in Orders.Where(o => o.Open).Select(o => o.TargetId).ToList()) Execute(id, year); }
        public Stand Clone() => new Stand { Trees = new List<ScenarioOneRackBay.Tree>(Trees), Orders = new List<ScenarioOneRackBay.Order>(Orders) };
    }

    // The stand as the lesson meets it: the rack work is done, so every tree on a rack line is a stump (not living, not a removal).
    private static Stand Load(string root)
    {
        string path = Path.Combine(root, "Tools", "Verification", "ScenarioOneNonUi", "OfflineLogic", "core-stand-replica.csv");
        var stand = new Stand();
        foreach (string line in File.ReadAllLines(path).Skip(1))
        {
            string[] c = line.Split(',');
            float x = float.Parse(c[1], CultureInfo.InvariantCulture), z = float.Parse(c[2], CultureInfo.InvariantCulture), d = float.Parse(c[3], CultureInfo.InvariantCulture);
            bool onRack = Lines.Any(l => Math.Abs(z - l) <= 1.2f);
            stand.Trees.Add(new ScenarioOneRackBay.Tree(c[0], x, z, d, !onRack, false));
            if (onRack) stand.Orders.Add(new ScenarioOneRackBay.Order(c[0], false, true, ScenarioOneRackBay.Disposition.Sold, 0));
        }
        return stand;
    }

    private static float Dist(ScenarioOneRackBay.Tree a, ScenarioOneRackBay.Tree b) => (float)Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Z - b.Z) * (a.Z - b.Z));

    // Three trees a careful player would choose: big, not overtopped, 8 m or more apart, inside the area.
    private static readonly string[] Future = { "P0714", "P1109", "P0706" };

    // The neighbours of a tree strongest first (largest competition term), living and not a future tree.
    private static List<string> Strongest(Stand stand, string id, IEnumerable<string> keep)
    {
        var target = stand.Get(id);
        var keepSet = new HashSet<string>(keep);
        return stand.Trees.Where(t => t.Living && t.Id != id && !keepSet.Contains(t.Id) && Dist(t, target) <= 8f)
            .OrderByDescending(t => ForestEcologyController.HegyiTerm(t.DbhCm, target.DbhCm, Dist(t, target))).Select(t => t.Id).ToList();
    }

    // Plan removals (strongest first) until every future tree reaches the share, the way a player would, one disposition for all.
    private static void PlanUntil(Stand stand, string[] future, float share, ScenarioOneRackBay.Disposition d)
    {
        for (int guard = 0; guard < 200; guard++)
        {
            var a = stand.Analyse();
            var weakest = a.Future.OrderBy(t => t.ReleaseShare).First();
            if (weakest.ReleaseShare >= share) return;
            string next = Strongest(stand, weakest.Id, future).First(id => !stand.Orders.Any(o => o.TargetId == id && o.Open));
            stand.Plan(next, d);
        }
        throw new InvalidOperationException("could not reach the release share");
    }

    public static int Run(string root)
    {
        SourceParity(root);
        Stand fresh = Load(root);
        Geometry(fresh);
        FutureTrees(fresh);
        OldTrivialRoute(fresh);
        Release(fresh);
        Dispositions(fresh);
        Execution(fresh);
        FurtherRelease(fresh);
        Robustness(fresh);
        Sequence();
        Wording(fresh);
        Console.WriteLine(failures == 0 ? $"NONUI_RACKBAY_PASS checks={checks}" : $"NONUI_RACKBAY_FAIL failures={failures} checks={checks}");
        return failures == 0 ? 0 : 1;
    }

    // The stand-in must be the game's real formula: read the real source and compare the two expressions.
    private static void SourceParity(string root)
    {
        string source = File.ReadAllText(Path.Combine(root, "Assets", "ForestPrototype", "ForestEcologyController.cs"));
        Check(Regex.IsMatch(source, @"public const float HegyiCutoffMeters = 8f;"), "the real competition reach is still 8 m");
        Check(source.Contains("return (neighbourDbhCm / Mathf.Max(1f, targetDbhCm)) / Mathf.Max(0.5f, distanceMeters);"), "the real Hegyi term is still the one the offline stand-in copies");
        Check(ScenarioOneRackBay.FutureTreeSpacingM == ForestEcologyController.HegyiCutoffMeters, "future-tree spacing is exactly the competition reach");
    }

    private static void Geometry(Stand fresh)
    {
        var none = ScenarioOneRackBay.Analyse(new float[0], fresh.Trees, fresh.Orders);
        Check(!none.Applies && none.FutureCount == 0 && !none.ReleasePlanned && !none.ReleaseExecuted, "no rack layout (a 40 m stand): the area rules do not apply");
        var a = fresh.Analyse();
        Check(a.Applies && a.RackCount == 5 && a.RackIndex == 2 && Math.Abs(a.CentreZ) < 1e-4f, "the area belongs to the middle rack of five");
        Check(Math.Abs(a.HalfWidthM - 7.6f) < 1e-3f, "it reaches half way to the neighbouring racks: 7.6 m either side", a.HalfWidthM.ToString("0.00", CultureInfo.InvariantCulture));
        Check(ScenarioOneRackBay.HalfWidth(new[] { 0f, 10f }, 0) == 5f && ScenarioOneRackBay.HalfWidth(new[] { 0f }, 0) == ScenarioOneRackBay.DefaultHalfWidthM, "half width uses the nearer neighbour, with a default for a lone rack");
        Check(a.FutureCount == 0 && !a.ReleasePlanned && !a.ReleaseExecuted && !a.FurtherPlanned && !a.FurtherExecuted && !a.MixedPlanned && !a.MixedExecuted, "a fresh stand with the racks open has nothing of the lesson done");
        Check(a.ReleaseYear == -1 && a.ExecutedRemovals == 0, "the rack work itself is not counted as release");
    }

    private static void FutureTrees(Stand fresh)
    {
        // Every candidate satisfies the rules before they are used.
        Stand s = fresh.Clone();
        foreach (string id in Future) s.Crop(id, true);
        var a = s.Analyse();
        Check(a.FutureCount == 3 && a.HasEnoughFuture && a.TooClose.Count == 0 && a.Overtopped.Count == 0, "three spaced, not-overtopped Crop Trees in the area are three future trees");
        Check(a.Future.Select(t => t.Id).OrderBy(x => x).SequenceEqual(Future.OrderBy(x => x)), "they are the three the player marked");
        Stand two = fresh.Clone(); two.Crop(Future[0], true); two.Crop(Future[1], true);
        Check(!two.Analyse().HasEnoughFuture && two.Analyse().FutureCount == 2, "two are not enough");
        // Spacing: a Crop Tree within 8 m of an accepted one does not count, and the answer does not depend on which was marked first.
        Stand close = s.Clone();
        // A smaller tree than the future tree it stands beside (the biggest of two close Crop Trees is the one that counts).
        var near = close.Trees.Where(t => t.Living && !t.Crop && Math.Abs(t.X) <= 12f && Math.Abs(t.Z) <= 7.6f && Math.Abs(t.Z) > 1.2f
            && t.DbhCm < close.Get(Future[0]).DbhCm - 1f && Dist(t, close.Get(Future[0])) < 7f && Dist(t, close.Get(Future[0])) > 3f).OrderBy(t => t.Id).First();
        close.Crop(near.Id, true);
        var c = close.Analyse();
        Check(c.FutureCount == 3 && (c.TooClose.Contains(near.Id) || c.Overtopped.Contains(near.Id)), "a Crop Tree within 8 m of a future tree does not count", near.Id);
        // Overtopped: the smallest tree in the area, surrounded by bigger ones, is not worth retaining.
        Stand low = fresh.Clone();
        var inArea = low.Trees.Where(t => t.Living && Math.Abs(t.X) <= 12 && Math.Abs(t.Z) <= 7.6f && Math.Abs(t.Z) > 1.2f).OrderBy(t => t.DbhCm).First();
        low.Crop(inArea.Id, true);
        Check(low.Analyse().Overtopped.Contains(inArea.Id) && low.Analyse().FutureCount == 0, "the smallest tree in the area is overtopped, so it is not a future tree", inArea.Id + " " + inArea.DbhCm.ToString("0.0", CultureInfo.InvariantCulture) + " cm");
        // Outside the area: the same good tree in a neighbouring rack's strip does not count here, and is reported.
        Stand away = fresh.Clone();
        var far = away.Trees.Where(t => t.Living && t.Z > 9f && t.Z < 14f).OrderByDescending(t => t.DbhCm).First();
        away.Crop(far.Id, true);
        Check(away.Analyse().FutureCount == 0 && away.Analyse().CropElsewhere == 1, "a Crop Tree outside the rack area is not counted but is reported", far.Id);
        Stand alongRack = fresh.Clone();
        var beyond = alongRack.Trees.Where(t => t.Living && Math.Abs(t.Z) < 7f && Math.Abs(t.Z) > 1.2f && Math.Abs(t.X) > 13f).OrderByDescending(t => t.DbhCm).First();
        alongRack.Crop(beyond.Id, true);
        Check(alongRack.Analyse().FutureCount == 0 && alongRack.Analyse().CropElsewhere == 1, "the area ends 12 m either side of the stand centre along the rack", beyond.Id);
    }

    // The old route: one Crop Tree, two removals, one deadwood decision. It must not complete any of Objectives 2-5 now.
    private static void OldTrivialRoute(Stand fresh)
    {
        Stand s = fresh.Clone();
        s.Crop("P0714", true);
        var tree = s.Get("P0714");
        var two = Strongest(s, "P0714", new[] { "P0714" }).Take(2).ToList();
        s.Plan(two[0], ScenarioOneRackBay.Disposition.Deadwood); s.Plan(two[1], ScenarioOneRackBay.Disposition.Sold);
        var a = s.Analyse();
        Check(!a.HasEnoughFuture, "old route: one Crop Tree is not enough future trees");
        Check(!a.ReleasePlanned && !a.FurtherPlanned, "old route: two removals do not release the future trees");
        s.ExecuteAll(1);
        a = s.Analyse();
        Check(!a.ReleaseExecuted && !a.FurtherExecuted, "old route: two executed removals do not complete the release");
        Check(a.MixedExecuted, "old route: one deadwood and one sold removal are a decision, but not a release");
        var facts = FactsFrom(a, accessReviewed: true, reviewed: true, twoCycles: true);
        Check(ScenarioOneTeachingSequence.Current(facts) <= 1, "old route: the player is still on Objective 2 (choose future trees)", "index " + ScenarioOneTeachingSequence.Current(facts));
        // Even with three future trees marked, two removals total do not reach Objective 4.
        Stand three = fresh.Clone();
        foreach (string id in Future) three.Crop(id, true);
        three.Plan(Strongest(three, Future[0], Future).First(), ScenarioOneRackBay.Disposition.Deadwood);
        three.Plan(Strongest(three, Future[1], Future).First(), ScenarioOneRackBay.Disposition.Sold);
        three.ExecuteAll(1);
        facts = FactsFrom(three.Analyse(), accessReviewed: true, reviewed: true, twoCycles: true);
        Check(ScenarioOneTeachingSequence.Current(facts) == 2, "old route with three future trees: still on Objective 3 (release them)", "index " + ScenarioOneTeachingSequence.Current(facts));
    }

    private static ScenarioOneTeachingSequence.Facts FactsFrom(ScenarioOneRackBay.Analysis a, bool accessReviewed, bool reviewed, bool twoCycles)
        => new ScenarioOneTeachingSequence.Facts
        {
            AccessOpen = true, Year = 1, AccessReviewed = accessReviewed, CropTrees = a.HasEnoughFuture ? a.FutureCount : 0,
            ReleasePlanned = a.ReleasePlanned, ReleaseExecuted = a.ReleaseExecuted, DeadwoodExecuted = a.MixedExecuted,
            ReleaseReviewed = reviewed && a.ReleaseYear >= 0, AreaReleased = a.FurtherExecuted, ReviewedTwoCycles = twoCycles, Bay = a
        };

    private static void Release(Stand fresh)
    {
        Stand s = fresh.Clone();
        foreach (string id in Future) s.Crop(id, true);
        var none = s.Analyse();
        Check(none.MinWithMarks == 0f && !none.ReleasePlanned, "no removals: nothing is released");
        // Loose Fell marks are progress for the player but not planned work.
        string first = Strongest(s, Future[0], Future).First();
        s.Mark(first, true);
        var marked = s.Analyse();
        Check(marked.MarkedRemovals == 1 && marked.PlannedRemovals == 0 && marked.Future.First(t => t.Id == Future[0]).MarkedShare > 0f && marked.Future.First(t => t.Id == Future[0]).PlannedShare == 0f, "a loose mark is counted as marked, not as planned");
        Check(!marked.ReleasePlanned, "marks alone never complete the planning step (the Work Plan is part of the lesson)");
        s.Mark(first, false);
        // Plan the release the way a player would.
        PlanUntil(s, Future, ScenarioOneRackBay.FirstReleaseShare, ScenarioOneRackBay.Disposition.Sold);
        var a = s.Analyse();
        Check(a.ReleasePlanned && a.CanopyKept && a.MinPlannedAndExecuted >= ScenarioOneRackBay.FirstReleaseShare, "planned removals that give every future tree its share complete the planning step",
            "removals " + a.PlannedRemovals + ", least released " + a.MinPlannedAndExecuted.ToString("0.000", CultureInfo.InvariantCulture));
        Check(a.PlannedRemovals >= 10, "that takes real management, not a token marking", a.PlannedRemovals + " removals");
        Check(a.RemovedBasalShare < ScenarioOneRackBay.MaxRemovedBasalShare, "and it leaves the area a forest", a.RemovedBasalShare.ToString("0.000", CultureInfo.InvariantCulture));
        Check(!a.ReleaseExecuted, "planned is not done");
        // Taking one order out undoes it for the tree it served.
        var weakest = a.Future.OrderBy(t => t.ReleaseShare).First();
        Stand fewer = s.Clone();
        string drop = fewer.Orders.Where(o => o.Open).Select(o => o.TargetId).OrderByDescending(id => ForestEcologyController.HegyiTerm(fewer.Get(id).DbhCm, weakest.DbhCm, Dist(fewer.Get(id), fewer.Get(weakest.Id)))).First(id => Dist(fewer.Get(id), fewer.Get(weakest.Id)) <= 8f);
        fewer.Orders.RemoveAll(o => o.TargetId == drop);
        Check(fewer.Analyse().Future.First(t => t.Id == weakest.Id).ReleaseShare < weakest.ReleaseShare, "cancelling a removal lowers the share of the tree it released");
        // The canopy limit: planning almost every tree in the area is not a release.
        Stand clear = fresh.Clone();
        foreach (string id in Future) clear.Crop(id, true);
        foreach (var t in clear.Trees.Where(t => t.Living && !t.Crop && Math.Abs(t.X) <= 12 && Math.Abs(t.Z) <= 7.6f && Math.Abs(t.Z) > 1.2f).ToList()) clear.Plan(t.Id, ScenarioOneRackBay.Disposition.Sold);
        var all = clear.Analyse();
        Check(all.MinPlannedAndExecuted >= 0.5f && !all.CanopyKept && !all.ReleasePlanned, "clearing the area is not a release: the canopy limit stops it", "removed " + all.RemovedBasalShare.ToString("0.00", CultureInfo.InvariantCulture));
        // Shared competitors: one removal between two future trees releases both.
        string shared = s.Trees.Where(t => t.Living && !t.Crop && Dist(t, s.Get(Future[0])) <= 8f && Dist(t, s.Get(Future[1])) <= 8f).Select(t => t.Id).FirstOrDefault();
        Check(shared != null, "two of the future trees share at least one competitor (so one removal can serve both)");
        if (shared != null)
        {
            Stand one = fresh.Clone(); foreach (string id in Future) one.Crop(id, true);
            one.Plan(shared, ScenarioOneRackBay.Disposition.Sold);
            var o = one.Analyse();
            Check(o.Future.First(t => t.Id == Future[0]).PlannedShare > 0f && o.Future.First(t => t.Id == Future[1]).PlannedShare > 0f, "that one removal counts toward both future trees");
        }
        // Rack-corridor stumps and removals far from every future tree change nothing.
        var farTree = s.Trees.First(t => t.Living && Future.All(f => Dist(t, s.Get(f)) > 9f) && Math.Abs(t.X) > 14f);
        Stand elsewhere = s.Clone();
        elsewhere.Plan(farTree.Id, ScenarioOneRackBay.Disposition.Sold);
        Check(Math.Abs(elsewhere.Analyse().MinPlannedAndExecuted - a.MinPlannedAndExecuted) < 1e-6f, "a removal far from every future tree does not count toward the release");
    }

    private static void Dispositions(Stand fresh)
    {
        Stand s = fresh.Clone();
        foreach (string id in Future) s.Crop(id, true);
        PlanUntil(s, Future, ScenarioOneRackBay.FirstReleaseShare, ScenarioOneRackBay.Disposition.Sold);
        var a = s.Analyse();
        Check(a.SoldPlanned == a.PlannedRemovals && a.DeadwoodPlanned == 0 && !a.MixedPlanned, "all sold: not yet a considered choice");
        string one = s.Orders.First(o => o.Open).TargetId;
        s.Plan(one, ScenarioOneRackBay.Disposition.Deadwood);
        a = s.Analyse();
        Check(a.DeadwoodPlanned == 1 && a.SoldPlanned == a.PlannedRemovals - 1 && a.MixedPlanned, "some sold and one left as deadwood is a mixed plan");
        foreach (string id in s.Orders.Where(o => o.Open).Select(o => o.TargetId).ToList()) s.Plan(id, ScenarioOneRackBay.Disposition.Deadwood);
        a = s.Analyse();
        Check(a.SoldPlanned == 0 && !a.MixedPlanned, "all left as deadwood: also not mixed (the lesson asks to compare both)");
        // 'Keep for use' is neither.
        s.Plan(one, ScenarioOneRackBay.Disposition.Other);
        Check(s.Analyse().DeadwoodPlanned == s.Analyse().PlannedRemovals - 1, "a third kind of outcome is counted as neither sold nor deadwood");
    }

    private static void Execution(Stand fresh)
    {
        Stand s = fresh.Clone();
        foreach (string id in Future) s.Crop(id, true);
        PlanUntil(s, Future, ScenarioOneRackBay.FirstReleaseShare, ScenarioOneRackBay.Disposition.Sold);
        s.Plan(s.Orders.First(o => o.Open).TargetId, ScenarioOneRackBay.Disposition.Deadwood);
        var planned = s.Analyse();
        s.ExecuteAll(2);
        var a = s.Analyse();
        Check(a.ReleaseExecuted && a.ExecutedRemovals == planned.PlannedRemovals && a.PlannedRemovals == 0, "carrying the plan out completes the release");
        Check(a.MixedExecuted && a.SoldExecuted >= 1 && a.DeadwoodExecuted == 1, "and both fates were used");
        Check(a.ReleaseYear == 2, "the release year is the year the work was done");
        Check(a.ReleasePlanned == false || a.ReleaseExecuted, "nothing planned any more, but the work stands");
        // Growth: the survivors grow while stumps keep their felling diameter, so the removed share thins out. A finished release must survive that.
        foreach (double growth in new[] { 1.01, 1.03, 1.06 })
        {
            Stand grown = s.Clone();
            for (int i = 0; i < grown.Trees.Count; i++) { var t = grown.Trees[i]; if (t.Living) grown.Trees[i] = new ScenarioOneRackBay.Tree(t.Id, t.X, t.Z, (float)(t.DbhCm * growth), true, t.Crop, t.Marked); }
            Check(grown.Analyse().ReleaseExecuted, "a finished release is not undone by " + ((growth - 1) * 100).ToString("0") + "% growth of the survivors");
        }
        Stand much = s.Clone();
        for (int i = 0; i < much.Trees.Count; i++) { var t = much.Trees[i]; if (t.Living) much.Trees[i] = new ScenarioOneRackBay.Tree(t.Id, t.X, t.Z, t.DbhCm * 1.3f, true, t.Crop, t.Marked); }
        Check(!much.Analyse().ReleaseExecuted || much.Analyse().MinExecuted >= ScenarioOneRackBay.FirstReleaseShare * ScenarioOneRackBay.ExecutedTolerance, "the tolerance is bounded: it never lets a real shortfall pass");
        // Lesson order through the whole stage.
        var facts = FactsFrom(a, true, false, true);
        Check(ScenarioOneTeachingSequence.Current(facts) == 4, "released and done, not yet reviewed, one more year of ... : the lesson is on Objective 5 (review)", "index " + ScenarioOneTeachingSequence.Current(facts));
    }

    private static void FurtherRelease(Stand fresh)
    {
        Stand s = fresh.Clone();
        foreach (string id in Future) s.Crop(id, true);
        PlanUntil(s, Future, ScenarioOneRackBay.FirstReleaseShare, ScenarioOneRackBay.Disposition.Sold);
        s.Plan(s.Orders.First(o => o.Open).TargetId, ScenarioOneRackBay.Disposition.Deadwood);
        s.ExecuteAll(2);
        var first = s.Analyse();
        Check(first.ReleaseExecuted && !first.FurtherExecuted && !first.FurtherPlanned, "the first release is not yet the further release");
        PlanUntil(s, Future, ScenarioOneRackBay.FurtherReleaseShare, ScenarioOneRackBay.Disposition.Sold);
        var planned = s.Analyse();
        Check(planned.FurtherPlanned && planned.CanopyKept, "a second round of planned removals reaches the further release", planned.MinPlannedAndExecuted.ToString("0.000", CultureInfo.InvariantCulture));
        Check(planned.RemovedBasalShare <= ScenarioOneRackBay.MaxRemovedBasalShare, "and still keeps the canopy", planned.RemovedBasalShare.ToString("0.000", CultureInfo.InvariantCulture));
        s.ExecuteAll(3);
        var done = s.Analyse();
        Check(done.FurtherExecuted && done.ReleaseYear == 3 && done.ExecutedRemovals > first.ExecutedRemovals, "carrying it out completes Objective 5's release, and the release year moves on", "year " + done.ReleaseYear);
        var facts = FactsFrom(done, true, false, true);
        Check(ScenarioOneTeachingSequence.Current(facts) == 4, "until it is reviewed the lesson stays on Objective 5");
        facts = FactsFrom(done, true, true, true);
        facts.BeechEstablished = facts.OakEstablished = false;
        Check(ScenarioOneTeachingSequence.Current(facts) == 5, "once reviewed the lesson moves on to the next generation (Objective 6)");
        // If the first pass already gave the further level, no second round is needed.
        Stand thorough = fresh.Clone();
        foreach (string id in Future) thorough.Crop(id, true);
        PlanUntil(thorough, Future, ScenarioOneRackBay.FurtherReleaseShare, ScenarioOneRackBay.Disposition.Sold);
        thorough.Plan(thorough.Orders.First(o => o.Open).TargetId, ScenarioOneRackBay.Disposition.Deadwood);
        thorough.ExecuteAll(2);
        var t = thorough.Analyse();
        Check(t.FurtherExecuted && t.MixedExecuted, "a player who releases further in the first round has nothing left to repeat (the lesson adapts to the forest)");
    }

    private static void Robustness(Stand fresh)
    {
        Stand s = fresh.Clone();
        foreach (string id in Future) s.Crop(id, true);
        PlanUntil(s, Future, ScenarioOneRackBay.FirstReleaseShare, ScenarioOneRackBay.Disposition.Sold);
        s.Plan(s.Orders.First(o => o.Open).TargetId, ScenarioOneRackBay.Disposition.Deadwood);
        s.Mark(Strongest(s, Future[2], Future).First(id => !s.Orders.Any(o => o.TargetId == id)), true);
        string Fingerprint(ScenarioOneRackBay.Analysis a) => string.Join("|", a.Applies, a.FutureCount, a.ReleasePlanned, a.ReleaseExecuted, a.FurtherPlanned, a.MixedPlanned, a.PlannedRemovals, a.MarkedRemovals,
            a.RemovedBasalShare.ToString("R", CultureInfo.InvariantCulture), a.MinPlannedAndExecuted.ToString("R", CultureInfo.InvariantCulture), string.Join(",", a.Future.Select(t => t.Id)));
        string baseline = Fingerprint(s.Analyse());
        var rnd = new Random(7);
        bool same = true;
        for (int round = 0; round < 25; round++)
        {
            Stand shuffled = s.Clone();
            shuffled.Trees = shuffled.Trees.OrderBy(_ => rnd.Next()).ToList();
            shuffled.Orders = shuffled.Orders.OrderBy(_ => rnd.Next()).ToList();
            if (Fingerprint(shuffled.Analyse()) != baseline) same = false;
        }
        Check(same, "the answer does not depend on the order the trees and orders are listed (25 shuffles)");
        Check(Fingerprint(s.Analyse()) == baseline, "the same forest always gives the same answer");
        // Orders for trees that are not in the stand, and a stump without an order, are harmless.
        Stand odd = s.Clone();
        odd.Orders.Add(new ScenarioOneRackBay.Order("P9999", false, true, ScenarioOneRackBay.Disposition.Sold, 1));
        odd.Trees.Add(new ScenarioOneRackBay.Tree("PX", 1f, 1f, 15f, false, false));
        Check(Fingerprint(odd.Analyse()) == baseline, "a completed order for a missing tree and an unrecorded stump change nothing");
        // A future tree that dies is no longer counted, and the rest are judged on their own.
        Stand lost = s.Clone();
        lost.Replace(Future[2], t => new ScenarioOneRackBay.Tree(t.Id, t.X, t.Z, t.DbhCm, false, false));
        Check(lost.Analyse().FutureCount == 2, "a future tree that is lost stops counting");
    }

    private static void Sequence()
    {
        var f = new ScenarioOneTeachingSequence.Facts();
        void Stage(int n, string why) => Check(ScenarioOneTeachingSequence.Current(f) == n, why, "index " + ScenarioOneTeachingSequence.Current(f));
        Check(f.AreaReleased, "the further-release fact is true by default (stages injected by earlier harnesses keep their meaning)");
        Stage(0, "access first"); f.AccessOpen = true; f.Year = 1; Stage(1, "after the racks: observe and choose");
        f.AccessReviewed = true; Stage(1, "no future trees yet");
        f.CropTrees = 3; Stage(2, "three future trees: release them");
        f.ReleasePlanned = true; Stage(3, "release planned: decide the wood and carry it out");
        f.ReleaseExecuted = true; Stage(3, "release done but no considered choice about the wood");
        f.DeadwoodExecuted = true; Stage(4, "done with both fates: review the response");
        f.ReleaseReviewed = true; f.ReviewedTwoCycles = true; Stage(5, "reviewed: next generation (the future trees were released enough)");
        f.AreaReleased = false; Stage(4, "reviewed but a future tree is still crowded: release further");
        f.AreaReleased = true; f.ReviewedTwoCycles = false; Stage(4, "the protected two-cycle review still has to hold");
        f.ReviewedTwoCycles = true; f.ReleaseReviewed = false; Stage(4, "a later release has to be reviewed again");
        f.ReleaseReviewed = true; f.BeechEstablished = f.OakEstablished = true; f.PlantingYear = 2; f.Year = 3; f.EstablishmentReviewed = true; Stage(7, "the later lessons are unchanged");
    }

    private static void Wording(Stand fresh)
    {
        var lessons = ScenarioOneTeachingSequence.Lessons;
        Check(lessons.Length == 8 && lessons.Select(l => l.Id).Distinct().Count() == 8, "still eight lessons with stable ids");
        Check(lessons[1].Id == "future-trees" && lessons[2].Id == "release" && lessons[3].Id == "habitat" && lessons[4].Id == "response", "ids of Objectives 2-5 are unchanged");
        foreach (int i in new[] { 1, 2, 3, 4 })
            Check(lessons[i].Title.StartsWith((i + 1) + ". ") && lessons[i].Concept.Length > 60 && lessons[i].Action.Length > 60 && lessons[i].Caption.Length > 10 && lessons[i].HudAction.Length > 10, "lesson " + (i + 1) + " has its title, reason, action, caption and HUD line");
        Check(lessons[1].Action.Contains("[E]") && lessons[1].Action.Contains("[C]") && lessons[2].Action.Contains("[1–5]") && lessons[2].Action.Contains("[X]") && lessons[2].Action.Contains("[Tab]"), "the lessons name the keys they ask for");
        Stand s = fresh.Clone();
        foreach (string id in Future.Take(2)) s.Crop(id, true);
        var f = FactsFrom(s.Analyse(), true, false, true);
        string text = ScenarioOneRackBayText.Instruction(1, f), progress = ScenarioOneRackBayText.Progress(1, f), hud = ScenarioOneRackBayText.Hud(1, f);
        Check(text != null && text.Contains("2 of 3") && progress.Contains("future trees 2 of 3") && progress.Contains("middle rack") && hud.Contains("2 of 3"), "Objective 2 says how many future trees there are of how many wanted");
        Check(progress.Length <= 80, "the progress line stays short enough to fit its label", progress.Length + " characters");
        s.Crop("P0706", true);
        var near = s.Trees.Where(t => t.Living && !t.Crop && Math.Abs(t.X) <= 12f && Math.Abs(t.Z) <= 7.6f && Math.Abs(t.Z) > 1.2f
            && t.DbhCm < s.Get("P0714").DbhCm - 1f && Dist(t, s.Get("P0714")) > 3f && Dist(t, s.Get("P0714")) < 7f).OrderBy(t => t.Id).First();
        s.Crop(near.Id, true);
        text = ScenarioOneRackBayText.Instruction(1, FactsFrom(s.Analyse(), true, false, true));
        Check(text.Contains("does not count") || text.Contains("do not count"), "a marked tree that does not count says so and why");
        Stand r = fresh.Clone();
        foreach (string id in Future) r.Crop(id, true);
        PlanUntil(r, Future, 0.20f, ScenarioOneRackBay.Disposition.Sold);
        var rf = FactsFrom(r.Analyse(), true, false, true);
        Check(Current(rf) == 2, "partially released: Objective 3");
        text = ScenarioOneRackBayText.Instruction(2, rf); progress = ScenarioOneRackBayText.Progress(2, rf);
        Check(text.Contains("Released so far") && text.Contains("P0714") && progress.StartsWith("Released:") && progress.Contains("about 30% each") && progress.Length <= 80, "Objective 3 reports each future tree's release against the target", progress);
        PlanUntil(r, Future, ScenarioOneRackBay.FirstReleaseShare, ScenarioOneRackBay.Disposition.Sold);
        rf = FactsFrom(r.Analyse(), true, false, true);
        text = ScenarioOneRackBayText.Instruction(3, rf);
        Check(Current(rf) == 3 && text.Contains("different fates") && ScenarioOneRackBayText.Progress(3, rf).Contains("deadwood 0"), "Objective 4 asks for different fates when everything is sold");
        r.Plan(r.Orders.First(o => o.Open).TargetId, ScenarioOneRackBay.Disposition.Deadwood);
        rf = FactsFrom(r.Analyse(), true, false, true);
        Check(ScenarioOneRackBayText.Instruction(3, rf).Contains("Approve the work"), "with both fates planned, Objective 4 asks to approve and advance");
        r.ExecuteAll(2);
        rf = FactsFrom(r.Analyse(), true, false, true);
        Check(ScenarioOneRackBayText.Instruction(4, rf).Contains("Annual Review"), "after the work, Objective 5 first asks for the review");
        rf = FactsFrom(r.Analyse(), true, true, true);
        text = ScenarioOneRackBayText.Instruction(4, rf);
        Check(!rf.AreaReleased && text.Contains("further") && text.Contains("40%") && ScenarioOneRackBayText.Progress(4, rf).Contains("about 40% each"), "then it asks to release further, with the target");
        Check(ScenarioOneRackBayText.Instruction(1, new ScenarioOneTeachingSequence.Facts()) == null && ScenarioOneRackBayText.Hud(2, new ScenarioOneTeachingSequence.Facts()) == null && ScenarioOneRackBayText.Progress(3, null) == null,
            "with no analysis (an injected stage or a 40 m stand) the wording falls back to the earlier text");
        foreach (var l in lessons)
            foreach (string txt in new[] { l.Concept, l.Action, l.Caption, l.HudAction, l.Practice })
                Check(!Regex.IsMatch(txt ?? "", @"(?i)\b(must fell|should fell|fell this)\b"), "the wording never names a tree to fell: " + l.Id);
    }

    private static int Current(ScenarioOneTeachingSequence.Facts f) => ScenarioOneTeachingSequence.Current(f);
}
