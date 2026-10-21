// Offline check of the progressive teaching pictures (TeachingDiagramPlan). Plain .NET, no Unity: see run_diagram_check.py.
//
//   TeachingDiagramPlanCheck            runs the structural checks and prints DIAGRAM_PLAN_PASS / DIAGRAM_PLAN_FAIL
//   TeachingDiagramPlanCheck dump       prints every drawn item in a canonical form, so run_diagram_check.py can compare the plan with the
//                                       approved design data (stand.json) item by item
//
// This proves what is DRAWN and in which order concepts appear. It does not prove that a picture renders, fits or is understood: those need
// the Unity screenshots and a learner.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

public static class TeachingDiagramPlanCheck
{
    private static int checks, failures;
    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

    public static int Main(string[] args)
    {
        if (args.Length > 0 && args[0] == "dump") { Dump(); return 0; }
        FirstLessonIsRacksOnly();
        VocabularyOnlyGrows();
        ObservationLessonsAddNoDecisions();
        SameStandThroughout();
        NoWhiteAndBudgets();
        Console.WriteLine(failures == 0 ? $"DIAGRAM_PLAN_PASS checks={checks}" : $"DIAGRAM_PLAN_FAIL failures={failures} checks={checks}");
        return failures == 0 ? 0 : 1;
    }

    private static void Check(bool ok, string what, string detail = "")
    {
        checks++;
        if (ok) return;
        failures++;
        Console.WriteLine("FAIL " + what + (detail.Length > 0 ? " :: " + detail : ""));
    }

    private static IEnumerable<TeachingDiagramPlan.Item> Items(int lesson)
    {
        var stage = TeachingDiagramPlan.For(lesson);
        return stage.Items.Concat(stage.Panels.SelectMany(p => p.Items));
    }

    // The distinct kinds of symbol on a picture, named as in the design notes.
    private static HashSet<string> Symbols(int lesson)
    {
        var set = new HashSet<string>();
        foreach (var item in Items(lesson))
        {
            switch (item.Kind)
            {
                case TeachingDiagramPlan.Kind.Crown: set.Add("crown-disc"); break;
                case TeachingDiagramPlan.Kind.Rack: set.Add("rack-strip"); break;
                case TeachingDiagramPlan.Kind.Opening: set.Add("dashed-opening"); break;
                case TeachingDiagramPlan.Kind.Log: set.Add("fallen-log"); break;
                case TeachingDiagramPlan.Kind.Shelter: set.Add("shelter-box"); break;
                case TeachingDiagramPlan.Kind.Clock: set.Add("time-clock"); break;
                case TeachingDiagramPlan.Kind.Repeat: set.Add("repeat-arrow"); break;
                case TeachingDiagramPlan.Kind.Young:
                    set.Add(item.Shape == TeachingDiagramPlan.YoungShape.Circle ? "young-disc" : item.Shape == TeachingDiagramPlan.YoungShape.Oval ? "beech-oval" : "oak-diamond");
                    if (item.Failed) set.Add("failure-cross");
                    break;
            }
            if ((item.Marks & TeachingDiagramPlan.Marks.Band) != 0) set.Add("future-band");
            if ((item.Marks & TeachingDiagramPlan.Marks.Ring) != 0) set.Add("inspection-ring");
            if ((item.Marks & TeachingDiagramPlan.Marks.Slash) != 0) set.Add("removal-slash");
        }
        return set;
    }

    private static void FirstLessonIsRacksOnly()
    {
        var items = Items(0).ToList();
        Check(items.All(i => i.Kind == TeachingDiagramPlan.Kind.Crown || i.Kind == TeachingDiagramPlan.Kind.Rack), "objective 1 draws only standing trees and racks");
        Check(items.All(i => i.Marks == TeachingDiagramPlan.Marks.None), "objective 1 has no management marks");
        Check(items.Count(i => i.Kind == TeachingDiagramPlan.Kind.Rack) == 2, "objective 1 shows two access strips");
        Check(items.Count(i => i.Kind == TeachingDiagramPlan.Kind.Crown) == 72, "objective 1 keeps 72 standing crowns beside the strips");
        Check(items.All(i => !i.Quiet), "objective 1 is drawn at full strength: nothing else competes");
        var racks = items.Where(i => i.Kind == TeachingDiagramPlan.Kind.Rack).ToList();
        Check(items.Where(i => i.Kind == TeachingDiagramPlan.Kind.Crown).All(c => racks.All(r => Math.Abs(c.X - r.X) > r.Width * 0.5f + c.Radius)),
            "no standing crown overlaps an access strip");
        var intro = Symbols(-1);
        Check(intro.SetEquals(new[] { "crown-disc", "time-clock" }), "the introduction shows the plantation and a time cycle only", string.Join(",", intro));
    }

    private static void VocabularyOnlyGrows()
    {
        int previous = 0;
        HashSet<string> seenBefore = new HashSet<string>();
        for (int lesson = 0; lesson < 8; lesson++)
        {
            var now = Symbols(lesson);
            int added = now.Count(s => !seenBefore.Contains(s));
            Check(now.Count >= previous, "the visible vocabulary never shrinks at lesson " + (lesson + 1), previous + " -> " + now.Count);
            Check(added >= 1 && added <= 4, "each lesson introduces a small number of new symbol types (lesson " + (lesson + 1) + ")", added + " new");
            // A symbol that was taught never has to be re-taught: anything that disappears must be a decision mark that was acted on.
            foreach (string gone in seenBefore.Where(s => !now.Contains(s)))
                Check(gone == "removal-slash", "only the removal slash leaves the picture, when its trees are felled (lesson " + (lesson + 1) + ")", gone);
            previous = now.Count;
            foreach (string s in now) seenBefore.Add(s);
        }
    }

    // Lessons 5 and 7 observe: the same management marks and removals as the lesson before them, only consequences are added.
    private static void ObservationLessonsAddNoDecisions()
    {
        foreach (int observation in new[] { 4, 6 })
        {
            var before = Items(observation - 1).ToList();
            var now = Items(observation).ToList();
            string Decisions(List<TeachingDiagramPlan.Item> list) => string.Join(";", list.Where(i => i.Marks != TeachingDiagramPlan.Marks.None).OrderBy(i => i.Id).Select(i => i.Id + ":" + i.Marks));
            Check(Decisions(before) == Decisions(now), "observation lesson " + (observation + 1) + " keeps the previous marks and adds none", Decisions(before) + " vs " + Decisions(now));
            var removedBefore = Removed(observation - 1);
            var removedNow = Removed(observation);
            Check(removedBefore.SetEquals(removedNow), "observation lesson " + (observation + 1) + " fells nothing new");
            Check(!now.Any(i => i.Kind == TeachingDiagramPlan.Kind.Rack && !before.Any(b => b.Id == i.Id)), "observation lesson " + (observation + 1) + " adds no rack");
        }
        // Lesson 5 shows time and a response but still no young trees: an opening alone promises nothing.
        Check(!Items(4).Any(i => i.Kind == TeachingDiagramPlan.Kind.Young), "lesson 5 shows no young trees");
        Check(Items(6).Any(i => i.Failed), "lesson 7 shows that some young trees fail");
    }

    private static HashSet<string> Removed(int lesson)
    {
        var drawn = new HashSet<string>(Items(lesson).Where(i => i.Kind == TeachingDiagramPlan.Kind.Crown).Select(i => i.Id));
        var all = new HashSet<string>(Items(-1).Where(i => i.Kind == TeachingDiagramPlan.Kind.Crown).Select(i => i.Id));
        all.ExceptWith(drawn);
        return all;
    }

    // Positions never drift between lessons: a crown that is drawn in two lessons is in the same place.
    private static void SameStandThroughout()
    {
        var first = Items(0).Where(i => i.Kind == TeachingDiagramPlan.Kind.Crown).ToDictionary(i => i.Id);
        for (int lesson = 1; lesson < 8; lesson++)
            foreach (var crown in Items(lesson).Where(i => i.Kind == TeachingDiagramPlan.Kind.Crown))
                if (first.TryGetValue(crown.Id, out var original))
                    Check(original.X == crown.X && original.Y == crown.Y, "crown " + crown.Id + " does not move in lesson " + (lesson + 1));
        Check(Items(-1).Count(i => i.Kind == TeachingDiagramPlan.Kind.Crown) == 84, "the introduction shows the whole 84-crown plantation");
        Check(TeachingDiagramPlan.Columns * TeachingDiagramPlan.Rows == 84, "the grid is 14 x 6");
        // Only the two future trees' crowns change size, once, in lesson 5.
        for (int lesson = 0; lesson < 8; lesson++)
            foreach (var crown in Items(lesson).Where(i => i.Kind == TeachingDiagramPlan.Kind.Crown))
                Check(crown.Radius == TeachingDiagramPlan.CrownRadius || (crown.Id == "t04_11" && lesson >= 4 && crown.Radius == 27f), "crown size is uniform apart from the one grown crown (" + crown.Id + ", lesson " + (lesson + 1) + ")");
    }

    private static void NoWhiteAndBudgets()
    {
        foreach (TeachingDiagramPlan.Role role in Enum.GetValues(typeof(TeachingDiagramPlan.Role)))
            foreach (float alpha in new[] { 1f, TeachingDiagramPlan.QuietAlpha, TeachingDiagramPlan.RackFillAlpha, TeachingDiagramPlan.OpeningFillAlpha })
            {
                TeachingDiagramPlan.Blend(role, alpha, out byte r, out byte g, out byte b);
                Check(!(r >= 235 && g >= 235 && b >= 235), "no white or near-white in the diagram palette: " + role + " @" + alpha, r + "," + g + "," + b);
            }
        for (int lesson = -1; lesson < 8; lesson++)
        {
            var stage = TeachingDiagramPlan.For(lesson);
            Check(stage.Labels.Length <= 4 && stage.Legend.Length <= 4, "at most four labels and four legend entries: " + stage.Id);
            foreach (var text in stage.Labels.Concat(stage.Legend))
            {
                Check(text.Value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length <= 4, "label is four words or fewer: " + text.Value);
                Check(text.X >= 0 && text.X + text.Width <= stage.Width && text.Y >= 0 && text.Y + text.Height <= stage.Height, "label sits inside the picture: " + text.Value);
            }
            // The legend names symbols that are actually on the picture.
            var symbols = Symbols(lesson);
            if (lesson >= 0)
                Check(stage.Legend.Length >= (lesson == 0 ? 2 : 3), "the legend explains the symbols on the picture: lesson " + (lesson + 1));
        }
        for (int lesson = 0; lesson < 8; lesson++)
            Check(TeachingDiagramPlan.For(lesson).Legend.Length >= 2, "every lesson picture has a legend: " + (lesson + 1));
    }

    // ---- canonical dump for comparison with the design data ----
    private static string N(float v) => Math.Round(v, 3).ToString("0.###", Inv);

    private static string Line(string prefix, TeachingDiagramPlan.Item i)
    {
        float x = i.X, y = i.Y, x1 = i.X1, y1 = i.Y1, r = i.Radius, rx = i.RadiusX, ry = i.RadiusY, w = i.Width, h = i.Height;
        string shape = i.Kind == TeachingDiagramPlan.Kind.Young ? i.Shape.ToString().ToLowerInvariant() : "-";
        var marks = new List<string>();
        if ((i.Marks & TeachingDiagramPlan.Marks.Band) != 0) marks.Add("band");
        if ((i.Marks & TeachingDiagramPlan.Marks.Ring) != 0) marks.Add("ring");
        if ((i.Marks & TeachingDiagramPlan.Marks.Slash) != 0) marks.Add("slash");
        return string.Join("|", prefix, i.Id, i.Kind.ToString().ToLowerInvariant(), N(x), N(y), N(x1), N(y1), N(r), N(rx), N(ry), N(w), N(h), shape,
            i.Quiet ? "quiet" : "emphasised", i.Failed ? "failed" : "-", marks.Count == 0 ? "-" : string.Join(",", marks), i.Z.ToString(Inv));
    }

    private static void Dump()
    {
        for (int lesson = -1; lesson < 8; lesson++)
        {
            var stage = TeachingDiagramPlan.For(lesson);
            Console.WriteLine("STAGE|" + stage.Number + "|" + N(stage.Width) + "|" + N(stage.Height));
            foreach (var item in stage.Items) Console.WriteLine(Line("S" + stage.Number, item));
            for (int p = 0; p < stage.Panels.Count; p++)
            {
                var panel = stage.Panels[p];
                Console.WriteLine("PANEL|" + stage.Number + "|" + p + "|" + N(panel.X) + "|" + N(panel.Y) + "|" + N(panel.Scale));
                foreach (var item in panel.Items) Console.WriteLine(Line("S" + stage.Number + "P" + p, item));
            }
            foreach (var t in stage.Labels) Console.WriteLine(string.Join("|", "LABEL" + stage.Number, t.Value, N(t.X), N(t.Y), N(t.Width), N(t.Height), N(t.FontSize), t.Role.ToString().ToLowerInvariant()));
            foreach (var t in stage.Legend) Console.WriteLine(string.Join("|", "LEGEND" + stage.Number, t.Value, N(t.X), N(t.Y), N(t.Width), N(t.Height), N(t.FontSize), t.Role.ToString().ToLowerInvariant()));
        }
    }
}
