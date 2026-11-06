// Runs the REAL TeachingDiagram.cs (with the real TeachingDiagramPlan.cs) against RecordingUnityStubs.cs and checks what it builds and draws.
// Issue #296 bounded repair: B-1 (which Objective 1 picture is shown in which world) and M-2 (the painter itself, not only the plan data).
//
//   PainterProbe dumpall               every lesson x constructor x frame width: element tree + Painter2D command stream (for base comparison)
//   PainterProbe check <base-dump>     selection, legacy byte-identity against <base-dump>, and five-rack painter semantics; FIVE_RACK_PAINTER_PASS/FAIL
//
// The probe constructs the diagram as LearningObjectivesView does, fires one geometry event at the given frame width, gives the art element the
// frame's height, and invokes its generateVisualContent callback. It proves the painter's commands, not Unity's rendering (see the stubs file).
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

public static class PainterProbe
{
    private const string Caption = "probe caption";
    private static readonly float[] Widths = { 810f, 788f, 900f };   // about 1280x720, 1920x1080 and the 900-unit maximum frame
    private static readonly string[] Constructors = { "two-arg", "false", "true" };
    private static int checks, failures;
    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

    public static int Main(string[] args)
    {
        if (args.Length >= 1 && args[0] == "dumpall")
        {
            foreach (string line in DumpAll()) Console.WriteLine(line);
            return 0;
        }
#if PAINTER_CHECK
        if (args.Length >= 2 && args[0] == "check")
        {
            Check(args[1]);
            Console.WriteLine(failures == 0 ? $"FIVE_RACK_PAINTER_PASS checks={checks}" : $"FIVE_RACK_PAINTER_FAIL failures={failures} checks={checks}");
            return failures == 0 ? 0 : 1;
        }
#endif
        Console.WriteLine("usage: PainterProbe dumpall | check <base-dump>");
        return 2;
    }

    private static void Result(bool ok, string what, string detail = "")
    {
        checks++;
        if (!ok) failures++;
        Console.WriteLine((ok ? "PASS " : "FAIL ") + "PAINTER " + what + (detail.Length > 0 ? " :: " + detail : ""));
    }

    // ---- building and recording ----

    private sealed class Built
    {
        public VisualElement Root, Frame, Art;
        public List<string> Draw = new List<string>();
        public TeachingDiagramPlan.Stage Stage;
        public float Scale;
    }

    private static Built Build(int lesson, string ctor, float width)
    {
        VisualElement root;
        if (ctor == "two-arg") root = (VisualElement)typeof(TeachingDiagram).GetConstructor(new[] { typeof(int), typeof(string) }).Invoke(new object[] { lesson, Caption });
        else
        {
            ConstructorInfo three = typeof(TeachingDiagram).GetConstructor(new[] { typeof(int), typeof(string), typeof(bool) });
            if (three == null) return null;
            root = (VisualElement)three.Invoke(new object[] { lesson, Caption, ctor == "true" });
        }
        var built = new Built { Root = root };
        built.Frame = Find(root, "teaching-diagram-frame");
        built.Art = built.Frame == null ? null : built.Frame.Children.FirstOrDefault(c => c.generateVisualContent != null);
        built.Stage = (TeachingDiagramPlan.Stage)typeof(TeachingDiagram).GetField("stage", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(root);
        if (built.Frame != null)
            foreach (Delegate callback in built.Frame.GeometryCallbacks)
                ((EventCallback<GeometryChangedEvent>)callback)(new GeometryChangedEvent { newRect = new Rect(0f, 0f, width, 0f) });
        if (built.Art != null && built.Stage != null)
        {
            float height = built.Stage.Height * width / built.Stage.Width;
            built.Art.contentRect = new Rect(0f, 0f, width, height);
            var painter = new Painter2D();
            built.Art.generateVisualContent(new MeshGenerationContext { painter2D = painter });
            built.Draw = painter.Log;
            built.Scale = Math.Min(width / built.Stage.Width, height / built.Stage.Height);
        }
        return built;
    }

    private static VisualElement Find(VisualElement e, string name)
    {
        if (e.name == name) return e;
        foreach (VisualElement c in e.Children) { VisualElement f = Find(c, name); if (f != null) return f; }
        return null;
    }

    private static IEnumerable<VisualElement> All(VisualElement e)
    {
        yield return e;
        foreach (VisualElement c in e.Children) foreach (VisualElement d in All(c)) yield return d;
    }

    private static IEnumerable<string> Elements(VisualElement e, string path)
    {
        string text = e is Label l ? l.text : null;
        yield return "EL " + path + " type=" + (e is Label ? "Label" : e.GetType().Name) + " name=" + e.name + " picking=" + e.pickingMode
            + " classes=" + string.Join(".", e.Classes) + " text=" + (text == null ? "-" : text.Replace("\n", "\\n"))
            + " style={" + string.Join(";", e.style.Set.Select(kv => kv.Key + "=" + kv.Value)) + "}"
            + " callbacks=" + e.GeometryCallbacks.Count + " draws=" + (e.generateVisualContent != null);
        for (int i = 0; i < e.Children.Count; i++)
            foreach (string line in Elements(e.Children[i], path + "/" + i)) yield return line;
    }

    private static IEnumerable<string> Section(int lesson, string ctor, float width)
    {
        Built b = Build(lesson, ctor, width);
        yield return "SECTION lesson=" + lesson + " ctor=" + ctor + " width=" + width.ToString("0", Inv);
        if (b == null) { yield return "UNAVAILABLE"; yield break; }
        foreach (string line in Elements(b.Root, "0")) yield return line;
        foreach (string op in b.Draw) yield return "DRAW " + op;
    }

    private static IEnumerable<string> DumpAll()
    {
        for (int lesson = -1; lesson <= 7; lesson++)
            foreach (float width in Widths)
                foreach (string ctor in Constructors)
                    foreach (string line in Section(lesson, ctor, width)) yield return line;
    }

#if PAINTER_CHECK
    // ---- reading the command stream as paths (current sources only: the base build compiles just the dump above) ----

    private sealed class PathOp
    {
        public bool Fill, Closed;
        public int R, G, B, A;
        public float Width;
        public string Cap = "";
        public List<(float x, float y)> Points = new List<(float x, float y)>();
        public float Length
        {
            get
            {
                float total = 0f;
                for (int i = 1; i < Points.Count; i++)
                    total += (float)Math.Sqrt(Math.Pow(Points[i].x - Points[i - 1].x, 2) + Math.Pow(Points[i].y - Points[i - 1].y, 2));
                return total;
            }
        }
        public string Shape => (Fill ? "F" : "S") + Points.Count + (Fill ? (A < 255 ? "t" : "o") : "");
    }

    private static List<PathOp> Paths(IEnumerable<string> log)
    {
        var paths = new List<PathOp>();
        PathOp current = null;
        foreach (string op in log)
        {
            string[] p = op.Split(' ');
            switch (p[0])
            {
                case "BEGIN": current = new PathOp(); break;
                case "M": case "L": current.Points.Add((float.Parse(p[1], Inv), float.Parse(p[2], Inv))); break;
                case "CLOSE": current.Closed = true; break;
                case "FILL":
                case "STROKE":
                {
                    int[] rgba = p[1].Split(',').Select(v => int.Parse(v, Inv)).ToArray();
                    var done = new PathOp { Fill = p[0] == "FILL", Closed = current.Closed, R = rgba[0], G = rgba[1], B = rgba[2], A = rgba[3], Points = current.Points };
                    if (!done.Fill) { done.Width = float.Parse(p[2].Substring(2), Inv); done.Cap = p[3].Substring(4); }
                    paths.Add(done);
                    break;
                }
            }
        }
        return paths;
    }

    // One item drawn on its own, through the painter's real DrawItem, after Draw has set the frame scale.
    private static List<string> DrawOne(Built b, TeachingDiagramPlan.Item item)
    {
        var painter = new Painter2D();
        MethodInfo drawItem = typeof(TeachingDiagram).GetMethod("DrawItem", BindingFlags.NonPublic | BindingFlags.Instance);
        drawItem.Invoke(b.Root, new object[] { painter, item });
        return painter.Log;
    }

    // ---- checks ----

    private static void Check(string baseDumpPath)
    {
        Selection();
        LegacyIsByteIdentical(baseDumpPath);
        foreach (float width in Widths) FigurePainter(width);
    }

    private static void Selection()
    {
        string figureId = "access-five-rack", legacyId = TeachingDiagramPlan.For(0).Id;
        Result(TeachingDiagramPlan.ShowsFiveRackFigure(0, true), "selection seam: Objective 1 in a Rack V2 world shows the five-rack figure");
        Result(!TeachingDiagramPlan.ShowsFiveRackFigure(0, false), "selection seam: Objective 1 outside Rack V2 does not show the five-rack figure");
        for (int lesson = -1; lesson <= 7; lesson++)
            if (lesson != 0)
                Result(!TeachingDiagramPlan.ShowsFiveRackFigure(lesson, true) && !TeachingDiagramPlan.ShowsFiveRackFigure(lesson, false),
                    "selection seam: picture " + lesson + " never shows the five-rack figure, whatever the flag");

        Built v2 = Build(0, "true", 810f), legacy = Build(0, "false", 810f), legacyTwo = Build(0, "two-arg", 810f);
        Result(v2 != null && v2.Stage.Id == figureId, "positive: TeachingDiagram(0, caption, true) draws the five-rack figure", v2?.Stage.Id ?? "no 3-arg constructor");
        Result(legacy != null && legacy.Stage.Id == legacyId, "negative: TeachingDiagram(0, caption, false) draws the original Objective 1 picture", legacy?.Stage.Id ?? "missing");
        Result(legacyTwo.Stage.Id == legacyId, "negative: the old two-argument constructor still draws the original Objective 1 picture (fails closed)", legacyTwo.Stage.Id);
        foreach (Built b in new[] { v2, legacy, legacyTwo })
            Result(b != null && Find(b.Root, "rack-lesson-diagram") != null, "Objective 1 art keeps the element name the rack onboarding checks look for");
        if (legacy == null) return;
        // B-1: nothing in the legacy Objective 1 picture claims the Rack V2 surfaces or the five-rack figure.
        string[] claims = { "headland", "feeder", "five-rack", "stand map", "80 m", "field-marked", "entrance" };
        var texts = All(legacy.Root).OfType<Label>().Select(l => l.text ?? "").ToList();
        var hits = claims.Where(c => texts.Any(t => t.IndexOf(c, StringComparison.OrdinalIgnoreCase) >= 0)).ToList();
        Result(hits.Count == 0, "legacy Objective 1 makes no headland / feeder / five-rack / Stand Map / 80 m claim", string.Join(",", hits));
        Result(!All(legacy.Root).OfType<Label>().Any(l => l.text == TeachingDiagramPlan.FigureTitle), "legacy Objective 1 has no five-rack title");
        Result(legacy.Root.Children.Count == 2 && legacy.Root.Children[0] == legacy.Frame && legacy.Root.Children[1] is Label c && c.text == Caption,
            "legacy Objective 1 is the frame then the lesson caption, nothing above or between");
    }

    private static Dictionary<string, List<string>> Sections(IEnumerable<string> lines)
    {
        var sections = new Dictionary<string, List<string>>();
        List<string> current = null;
        foreach (string line in lines)
        {
            if (line.StartsWith("SECTION ", StringComparison.Ordinal)) sections[line.Substring(8)] = current = new List<string>();
            else current?.Add(line);
        }
        return sections;
    }

    // The base build is the pre-#296 painter and plan (two-argument constructor only). Its output is what Legacy40, Reference Future v1 and
    // pre-V2 saves saw. Every picture drawn without the Rack V2 flag must reproduce it exactly, command for command.
    private static void LegacyIsByteIdentical(string baseDumpPath)
    {
        var baseline = Sections(File.ReadAllLines(baseDumpPath));
        var current = Sections(DumpAll());
        int compared = 0, differ = 0;
        var firstDiffs = new List<string>();
        for (int lesson = -1; lesson <= 7; lesson++)
            foreach (float width in Widths)
            {
                string w = width.ToString("0", Inv);
                if (!baseline.TryGetValue("lesson=" + lesson + " ctor=two-arg width=" + w, out List<string> expected) || expected.Count < 5)
                { Result(false, "base dump has lesson " + lesson + " at width " + w); continue; }
                var wanted = new List<string> { "two-arg", "false" };
                if (lesson != 0) wanted.Add("true");
                foreach (string ctor in wanted)
                {
                    compared++;
                    List<string> got = current["lesson=" + lesson + " ctor=" + ctor + " width=" + w];
                    if (!got.SequenceEqual(expected)) { differ++; if (firstDiffs.Count < 4) firstDiffs.Add("lesson " + lesson + " " + ctor + " " + w); }
                }
                if (lesson == 0)
                {
                    List<string> figure = current["lesson=0 ctor=true width=" + w];
                    Result(!figure.SequenceEqual(expected), "positive: Rack V2 Objective 1 at width " + w + " differs from the original picture");
                }
            }
        Result(compared == 9 * 3 * 2 + 8 * 3 && differ == 0,
            "every picture drawn without the Rack V2 flag is byte-identical to the pre-#296 painter (elements, styles and every draw command)",
            "compared=" + compared + " differ=" + differ + (firstDiffs.Count > 0 ? " first: " + string.Join("; ", firstDiffs) : ""));
        int lines = baseline.Values.Sum(v => v.Count);
        Result(lines > 20000, "the base dump is substantial (not an empty comparison)", lines.ToString(Inv) + " lines");
    }

    private static void FigurePainter(float width)
    {
        string w = " @" + width.ToString("0", Inv);
        Built b = Build(0, "true", width);
        if (b == null || b.Stage == null || b.Draw.Count == 0) { Result(false, "five-rack figure builds and draws" + w); return; }
        var items = b.Stage.Items;
        float scale = b.Scale;
        var perItem = items.ToDictionary(i => i.Id, i => DrawOne(b, i));

        // The painter draws exactly the plan's items, in the plan's order, and nothing else.
        var concat = new List<string>();
        foreach (var item in items) concat.AddRange(perItem[item.Id]);
        Result(b.Draw.SkipWhile(op => op.StartsWith("JOIN", StringComparison.Ordinal) || op.StartsWith("CAP", StringComparison.Ordinal)).SequenceEqual(concat),
            "Draw issues exactly each plan item's commands in plan order" + w);
        var paths = items.ToDictionary(i => i.Id, i => Paths(perItem[i.Id]));

        // Planned corridors: a see-through fill and two continuous solid edges the full length of the rack.
        int plannedFillAlpha = -1;
        foreach (var item in items.Where(i => i.Kind == TeachingDiagramPlan.Kind.Corridor && i.State == TeachingDiagramPlan.CorridorState.Planned))
        {
            var ps = paths[item.Id];
            float length = (float)Math.Sqrt(Math.Pow(item.X1 - item.X, 2) + Math.Pow(item.Y1 - item.Y, 2)) * scale;
            var fills = ps.Where(p => p.Fill).ToList();
            var strokes = ps.Where(p => !p.Fill).ToList();
            Result(fills.Count == 1 && fills[0].Points.Count == 4 && fills[0].A <= 90, "planned " + item.Id + ": one see-through fill (alpha <= 90/255), trees beneath stay visible" + w,
                string.Join(",", fills.Select(f => "a=" + f.A)));
            Result(strokes.Count == 2 && strokes.All(s => s.Points.Count == 2 && s.A == 255 && s.Length >= 0.99f * length),
                "planned " + item.Id + ": two continuous solid edges along the whole rack (not dashed)" + w,
                string.Join(",", strokes.Select(s => s.Points.Count + "pts/" + s.Length.ToString("0.#", Inv) + "of" + length.ToString("0.#", Inv))));
            if (fills.Count == 1) plannedFillAlpha = fills[0].A;
            if (!item.Id.StartsWith("key-", StringComparison.Ordinal))
            {
                // The standing trees along the rack are either drawn on top, or under a see-through fill.
                int at = items.IndexOf(item);
                var under = items.Where(c => c.Kind == TeachingDiagramPlan.Kind.Crown && Math.Abs(c.Y - item.Y) < item.Width * 0.5f && c.X >= item.X && c.X <= item.X1).ToList();
                bool visible = under.All(c => items.IndexOf(c) > at || (fills.Count == 1 && fills[0].A <= 90));
                Result(under.Count >= 10 && visible, "planned " + item.Id + ": its standing trees remain visible (planning does not clear)" + w, under.Count.ToString(Inv));
            }
        }
        var draft = paths["key-draft"];
        var draftItem = items.Single(i => i.Id == "key-draft");
        float draftLength = (draftItem.X1 - draftItem.X) * scale;
        var draftStrokes = draft.Where(p => !p.Fill).ToList();
        Result(draftStrokes.Count >= 6 && draftStrokes.All(s => s.Length <= 0.5f * draftLength), "draft key swatch: dashed edges (many short strokes)" + w, draftStrokes.Count.ToString(Inv));
        Result(draft.Count(p => p.Fill) == 1 && draft.Single(p => p.Fill).A < plannedFillAlpha, "draft key swatch: fainter see-through fill than planned" + w);
        var harvested = paths["key-harvested"];
        Result(harvested.Count == 1 && harvested[0].Fill && harvested[0].A == 255, "harvested key swatch: one opaque open strip, no edges (key only)" + w);

        // Existing surfaces: one continuous opaque band each, round ends, at least the authored width; casings drawn before fills.
        foreach (var item in items.Where(i => i.Kind == TeachingDiagramPlan.Kind.Track))
        {
            var ps = paths[item.Id];
            Result(ps.Count == 1 && !ps[0].Fill && ps[0].A == 255 && ps[0].Cap == "Round" && ps[0].Points.Count == item.Points.Length / 2
                && ps[0].Width >= item.Width * scale - 0.01f, "existing surface " + item.Id + ": one continuous opaque round-ended band at its width" + w);
        }
        int lastCasing = items.FindLastIndex(i => !i.Id.StartsWith("key-", StringComparison.Ordinal) && i.Role == TeachingDiagramPlan.Role.TrackCasing);
        int firstFill = items.FindIndex(i => !i.Id.StartsWith("key-", StringComparison.Ordinal) && (i.Kind == TeachingDiagramPlan.Kind.Track || i.Kind == TeachingDiagramPlan.Kind.Pad) && i.Role == TeachingDiagramPlan.Role.Track);
        Result(lastCasing >= 0 && lastCasing < firstFill && paths[items[lastCasing].Id].Count > 0, "every existing casing is painted before any existing fill" + w);

        foreach (var item in items.Where(i => i.Kind == TeachingDiagramPlan.Kind.Entrance))
        {
            var ps = paths[item.Id];
            Result(ps.Count == 2 && ps[0].Fill && ps[0].Points.Count == 4 && !ps[1].Fill && ps[1].Points.Count == 5, "entrance " + item.Id + ": an outlined square" + w);
        }
        var mark = paths["key-field-mark"];
        Result(mark.Count == 2 && mark.All(p => !p.Fill && p.Points.Count == 2 && Math.Abs(p.Points[0].x - p.Points[1].x) < 0.01f), "field-mark key: two vertical stripes, no fill (a standing marked tree)" + w);
        var boundary = paths["property-boundary"];
        Result(boundary.Count == 1 && !boundary[0].Fill && boundary[0].Points.Count == 5, "property boundary: one continuous outline, not dashed like a draft" + w, boundary.Count.ToString(Inv));
        var north = paths["north"];
        Result(north.Count == 2 && north.Any(p => p.Fill) && north.Any(p => !p.Fill), "north arrow: shaft and head drawn" + w);

        // No Fell colour anywhere: compared by RGB at every strength the figure uses.
        var fell = new HashSet<string>();
        foreach (float a in new[] { 1f, TeachingDiagramPlan.QuietAlpha })
        { TeachingDiagramPlan.Blend(TeachingDiagramPlan.Role.Fell, a, out byte r, out byte g, out byte bb); fell.Add(r + "," + g + "," + bb); }
        var all = Paths(b.Draw);
        Result(!all.Any(p => fell.Contains(p.R + "," + p.G + "," + p.B)), "no Fell-coloured fill or stroke anywhere in the figure" + w);

        // The key's rows differ by shape and pattern alone: colour is ignored in these signatures.
        var rows = new Dictionary<string, string[]>
        {
            ["existing"] = new[] { "key-existing-casing", "key-existing" },
            ["draft"] = new[] { "key-tree-1a", "key-tree-1b", "key-draft" },
            ["planned"] = new[] { "key-tree-2a", "key-tree-2b", "key-planned" },
            ["entrance"] = new[] { "key-entrance" },
            ["field-marked"] = new[] { "key-field-marked-tree", "key-field-mark" },
            ["harvested"] = new[] { "key-harvested", "key-stump-a", "key-stump-b" },
            ["retained"] = new[] { "key-retained-tree" },
        };
        string Signature(string[] ids) => string.Join("+", ids.SelectMany(id => paths[id]).Select(p => p.Shape + (p.Fill ? "" : p.Length < 0.5f * draftLength ? "short" : "long")).OrderBy(s => s, StringComparer.Ordinal));
        var signatures = rows.ToDictionary(kv => kv.Key, kv => Signature(kv.Value));
        Result(signatures.Values.Distinct().Count() == rows.Count, "every key row is drawn with its own shape/pattern, colour ignored" + w,
            string.Join(" ", signatures.Select(kv => kv.Key + "=" + kv.Value.Length)));

        // Words: the title above the picture, the note(s) below it, the lesson caption last; the key and route tags inside the frame.
        var children = b.Root.Children;
        var title = children.Count > 0 ? children[0] as Label : null;
        Result(title != null && title.text == TeachingDiagramPlan.FigureTitle && title.ClassListContains("diagram-label")
            && title.style.Set.TryGetValue("position", out string pos) && pos == "position:Relative" && FontSize(title) >= 20f,
            "title is the first element, above the frame, with the exact approved wording at >= 20 UI units" + w);
        int frameAt = children.IndexOf(b.Frame);
        var notes = children.Skip(frameAt + 1).Take(b.Stage.Notes.Length).OfType<Label>().ToList();
        Result(frameAt == 1 && notes.Count == b.Stage.Notes.Length && b.Stage.Notes.Length >= 1 && notes.Select(n => n.text).SequenceEqual(b.Stage.Notes)
            && notes.All(n => n.ClassListContains("diagram-caption") && FontSize(n) >= 20f), "the figure's note(s) follow the frame, exact text, >= 20 UI units" + w);
        Result(children.Count == 2 + b.Stage.Notes.Length + 1 && children[children.Count - 1] is Label cap && cap.text == Caption,
            "the lesson caption is last and nothing else is added outside the frame" + w);
        var frameLabels = b.Frame.Children.OfType<Label>().ToList();
        var frameTexts = frameLabels.Select(l => l.text).ToList();
        Result(TeachingDiagramPlan.FigureKey.All(k => frameTexts.Count(t => t == k) == 1), "every key row is a label inside the frame" + w);
        Result(new[] { "A", "B", "C", "N", "1", "2", "3", "4", "5" }.All(v => frameTexts.Count(t => t == v) == 1), "route tags A, B, C, rack numbers 1-5 and N are labels inside the frame" + w);
        Result(frameLabels.All(l => FontSize(l) >= 20f), "every figure word inside the frame is >= 20 UI units" + w,
            frameLabels.Min(l => FontSize(l)).ToString("0.##", Inv));
    }

    private static float FontSize(VisualElement e) =>
        e.style.Set.TryGetValue("fontSize", out string v) ? float.Parse(v, Inv) : 0f;
#endif
}
