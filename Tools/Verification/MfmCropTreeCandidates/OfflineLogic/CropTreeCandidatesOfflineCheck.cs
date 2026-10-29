// Focused pure-domain MFM-05 checks. Synthetic geometry isolates spacing from coverage calibration.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using C = ScenarioOneCropTreeCandidates;
using V = ScenarioOneCoverage;
using Cal = ScenarioOnePrescriptionCalibration;

public static class CropTreeCandidatesOfflineCheck
{
    private static int checks, failed;
    private static void Check(bool ok, string name)
    {
        checks++;
        Console.WriteLine((ok ? "PASS " : "FAIL ") + name);
        if (!ok) failed++;
    }
    private static V.Stem Stem(string id, float x, float z = 0, bool living = true, string species = V.SitkaSpeciesId,
        bool crop = false, bool fell = false, float dbh = 20)
        => new V.Stem(id, x, z, dbh, living, species, crop, fell);
    private static V.Area Area(int rack, V.Segment segment, params string[] ids)
    {
        var a = new V.Area { Id = V.AreaId(rack, segment), RackId = rack, Segment = segment, State = V.State.Covered };
        a.StemIds.AddRange(ids);
        return a;
    }
    private static V.Result Coverage(params V.Area[] areas)
    {
        var r = new V.Result { Valid = true };
        r.Areas.AddRange(areas);
        return r;
    }
    private static C.Result Suggest(V.Result coverage, IReadOnlyList<V.Stem> stems, string[] sample = null,
        string[] confirmed = null, string[] rejected = null)
        => C.Suggest(coverage, stems, sample, confirmed, rejected, Cal.V1);
    private static string Ids(C.Result r) => string.Join(",", r.Candidates.Select(c => c.TreeId));
    private static string Snapshot(C.Result r) => r.Signature() + string.Join("|", r.Candidates.Select(c =>
        FormattableString.Invariant($"{c.TreeId}:{c.X:R}:{c.Z:R}:{c.NearestBlockerM:R}:{c.Reason}")));

    public static int Main(string[] args)
    {
        var stems = new List<V.Stem> { Stem("a", 0), Stem("b", 16), Stem("c", 32) };
        var coverage = Coverage(Area(1, V.Segment.Low, "a", "b", "c"));
        var original = Suggest(coverage, stems);
        Check(original.Valid && Ids(original) == "b,a,c", "centre first, x tie break, exact 16 m boundary accepted");
        Check(original.Candidates.Select(c => c.Order).SequenceEqual(new[] { 1, 2, 3 }), "one-based queue order");
        Check(original.Candidates.All(c => c.Decision == C.Decision.Open && c.Reason == C.CandidateReason.SpacingOnlyInspectToJudge), "open inspection suggestions only");
        Check(original.SpacingM == 16 && Cal.V1.CandidateSpacingM == 16, "reuse versioned MFM-03 spacing");
        Check(Suggest(coverage, new[] { Stem("a", 0), Stem("b", 15.999f), Stem("c", 32) }).Candidates.Count == 2, "strictly closer than spacing blocked");
        var ties = Suggest(Coverage(Area(1, V.Segment.Low, "z", "a")), new[] { Stem("z", 0), Stem("a", 0) });
        Check(Ids(ties) == "a", "coincident stems use ordinal id tie break");
        var ztie = Suggest(Coverage(Area(1, V.Segment.Low, "up", "down")), new[] { Stem("up", 0, 8), Stem("down", 0, -8) });
        Check(Ids(ztie) == "down,up", "z tie break after x");

        foreach (var excluded in new[] { Stem("b", 16, living: false), Stem("b", 16, fell: true),
            Stem("b", 16, species: "sessile-oak"), Stem("b", 16, species: "beech"), Stem("b", 16, crop: true) })
        {
            var r = Suggest(coverage, new[] { stems[0], excluded, stems[2] });
            Check(!r.Candidates.Any(c => c.TreeId == "b"), "exclude nonliving/fell/broadleaf/manual crop " + excluded.SpeciesId + excluded.Living + excluded.FellMark + excluded.Crop);
        }
        Check(Ids(Suggest(coverage, new[] { stems[0], stems[2] })) == "a,c", "missing current stem excluded");
        Check(!Suggest(coverage, stems, new[] { "b" }).Candidates.Any(c => c.TreeId == "b"), "explicit sample Crop Tree excluded even without Crop flag");
        foreach (var state in new[] { V.State.NotCovered, V.State.Excluded, V.State.Manual })
        {
            coverage.Areas[0].State = state;
            Check(Suggest(coverage, stems).Candidates.Count == 0, "no candidates in " + state);
        }
        coverage.Areas[0].State = V.State.Covered;
        coverage.Areas[0].IsSource = true;
        Check(Suggest(coverage, stems).Candidates.Count == 0, "sample/source area excluded defensively");
        coverage.Areas[0].IsSource = false;
        Check(Suggest(coverage, stems.Append(Stem("outside", 100)).ToArray()).Signature() == original.Signature(), "stem outside covered membership never a candidate");
        var blocker = Stem("manual", -1, crop: true);
        var blocked = Suggest(coverage, stems.Append(blocker).ToArray());
        Check(!blocked.Candidates.Any(c => c.TreeId == "a" || c.TreeId == "manual"), "manual designation outside area blocks spacing");
        Check(Suggest(coverage, stems.Append(Stem("sample", -1)).ToArray(), new[] { "sample" }).Candidates.All(c => c.TreeId != "a"), "sample crop blocks spacing without Crop flag");

        var decidedStems = new[] { stems[0], Stem("b", 16, crop: true), stems[2] };
        var decided = Suggest(coverage, decidedStems, confirmed: new[] { "b" }, rejected: new[] { "a" });
        Check(Ids(decided) == Ids(original), "decisions retain batch slots, no replacement");
        Check(decided.Candidates[0].Decision == C.Decision.Confirmed && decided.Candidates[1].Decision == C.Decision.Rejected, "decisions echoed individually");
        Check(!C.TryGetInspectionTarget(decided, "b", out _) && !C.TryGetInspectionTarget(decided, "a", out _), "decided entries cannot be selected again");
        Check(C.TryMoveInspectionTarget(decided, "b", C.Navigation.Next, out var target) && target.TreeId == "c", "next skips rejected and confirmed");
        Check(!C.TryMoveInspectionTarget(decided, "c", C.Navigation.Previous, out _), "previous skips all decided entries");
        Check(C.TryGetInspectionTarget(original, "b", out target) && target.TreeId == "b" && target.AreaId == "R1L" && target.Order == 1 && target.X == 16 && target.Z == 0
            && target.Reason == C.CandidateReason.SpacingOnlyInspectToJudge, "selection returns single navigation target with inspection reason");
        Check(C.TryMoveInspectionTarget(original, null, C.Navigation.Next, out target) && target.TreeId == "b", "start next at first");
        Check(C.TryMoveInspectionTarget(original, "", C.Navigation.Previous, out target) && target.TreeId == "c", "start previous at last");
        Check(C.TryMoveInspectionTarget(original, "a", C.Navigation.Previous, out target) && target.TreeId == "b", "previous follows queue");
        Check(!C.TryMoveInspectionTarget(original, "c", C.Navigation.Next, out _) && !C.TryMoveInspectionTarget(original, "b", C.Navigation.Previous, out _), "queue ends do not wrap");
        Check(!C.TryGetInspectionTarget(original, "missing", out _) && !C.TryMoveInspectionTarget(original, "missing", C.Navigation.Next, out _), "noncandidate selection/navigation refused");
        Check(!C.TryMoveInspectionTarget(original, null, (C.Navigation)999, out _), "unknown navigation refused");
        Check(Snapshot(original) == Snapshot(Suggest(coverage, stems)) && stems.All(s => !s.Crop && !s.FellMark), "selection/generation neither designate nor mark, no input mutation");
        Check(!C.TryGetInspectionTarget(null, "b", out _) && !C.TryMoveInspectionTarget(new C.Result(), null, C.Navigation.Next, out _), "null/invalid result selection refused");
        var empty = Suggest(Coverage(Area(1, V.Segment.Low)), stems);
        Check(empty.Valid && empty.AreasWithoutCandidate.SequenceEqual(new[] { "R1L" }) && !C.TryMoveInspectionTarget(empty, null, C.Navigation.Next, out _), "empty queue handled");
        var allDecided = Suggest(coverage, stems, rejected: new[] { "a", "b", "c" });
        Check(!C.TryMoveInspectionTarget(allDecided, null, C.Navigation.Next, out _) && Ids(allDecided) == Ids(original), "all rejected has no actionable target or replacement");
        var stale = Suggest(coverage, new[] { stems[0], Stem("b", 16, living: false), stems[2] }, confirmed: new[] { "b", "gone" });
        Check(stale.StaleDecisions.SequenceEqual(new[] { "b", "gone" }), "nonliving/missing confirmed records reported stale");
        Check(!Suggest(coverage, stems, confirmed: new[] { "b" }, rejected: new[] { "b" }).Valid, "contradictory decisions refused");
        Check(!Suggest(coverage, stems.Append(stems[0]).ToArray()).Valid, "duplicate stem identity refused");
        Check(!Suggest(null, stems).Valid && !Suggest(new V.Result(), stems).Valid, "invalid coverage refused");
        foreach (double spacing in new[] { 0, -1, double.NaN, double.PositiveInfinity })
            Check(!C.Suggest(coverage, stems, null, null, null, Cal.V1.WithCandidateSpacing(spacing)).Valid, "invalid spacing refused " + spacing);
        Check(!C.Suggest(coverage, stems, null, null, null, null).Valid, "missing calibration refused");

        // Shuffle all input orders, including the lists exposed by coverage, without changing its meaning.
        var mixed = Coverage(Area(10, V.Segment.Low, "r10"), Area(2, V.Segment.High, "high"), Area(2, V.Segment.Low, "a", "b", "c"));
        var world = stems.Concat(new[] { Stem("high", 100), Stem("r10", 200) }).ToList();
        var baseline = Suggest(mixed, world);
        Check(Ids(baseline) == "b,a,c,high,r10", "numeric rack order then Low/Centre/High");
        var random = new Random(405);
        for (int i = 0; i < 32; i++)
        {
            mixed.Areas.Reverse();
            foreach (var area in mixed.Areas)
            {
                var ids = area.StemIds.OrderBy(_ => random.Next()).ToList();
                area.StemIds.Clear(); area.StemIds.AddRange(ids);
            }
            world = world.OrderBy(_ => random.Next()).ToList();
            Check(Snapshot(Suggest(mixed, world)) == Snapshot(baseline), "shuffle stable " + i);
        }
        coverage.Areas[0].StemIds.Add("b");
        Check(Snapshot(Suggest(coverage, stems)) == Snapshot(original), "duplicate membership does not bias centre or repeat target");
        Check(Snapshot(Suggest(coverage, stems.Select((s, i) => Stem(s.Id, s.X, s.Z, dbh: i == 0 ? 1 : 999)).ToList())) == Snapshot(original), "arbitrary diameter changes do not rank quality");

        string source = File.ReadAllText(Path.Combine(args[0], "Assets/ForestPrototype/ScenarioOne/ScenarioOneCropTreeCandidates.cs"));
        source = Regex.Replace(source, @"//[^\r\n]*|/\*[\s\S]*?\*/", "");
        foreach (string token in new[] { "UnityEngine", "TreeMarkType", "ScenarioOneWorkOrder", "ScenarioOneManager", "ForestTree", "SaveData", "PlayerPrefs", "File.", "DbhCm", "Height", "Competition", "Share", "BasalArea" })
            Check(!source.Contains(token), "domain source has no " + token);
        Check(typeof(C).GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static).Select(m => m.Name)
            .OrderBy(n => n).SequenceEqual(new[] { "Suggest", "TryGetInspectionTarget", "TryMoveInspectionTarget" }), "public API has no confirmation/bulk/approval/write operation");
        Console.WriteLine($"MFM05_CROP_TREE_CANDIDATES_{(failed == 0 ? "PASS" : "FAIL")} checks={checks} failed={failed}");
        return failed == 0 ? 0 : 1;
    }
}
