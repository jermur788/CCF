using System.Collections.Generic;
using System.Linq;

// The progressive teaching pictures (the scenario introduction and the eight lessons) as plain data. No UnityEngine here, so the
// progression can be checked offline (Tools/Verification/ScenarioOneUxPedagogy/OfflineLogic) and the painter (TeachingDiagram) stays dumb.
//
// One conceptual stand anchors every picture: 84 equal Sitka crowns in 14 columns x 6 rows, north up. Nothing moves between lessons; a lesson
// only reveals the layer it teaches, or changes one explicit thing (two competitors are felled, one crown has grown). Earlier layers stay on the
// picture as quiet context; the focus of the lesson is drawn at full strength. The observation lessons (5 and 7) add consequences, never a new
// decision. The first lesson shows racks and standing trees only.
//
// The pictures are original teaching diagrams in the top-down style of a stand plan, inspired by the useful visual language of the Pro Silva
// continuous-cover book's stand diagrams (pp. 66-67); none of its artwork, dimensions, ages or species percentages is reproduced. The counts are
// diagram counts, not a prescription or a prediction for the playable forest, and the geometry is in design units (not metres). No white is used.
public static class TeachingDiagramPlan
{
    public enum Role { Panel, Canopy, CanopyEdge, Muted, Accent, Inspect, Rack, Crop, Fell, Good, Warn, Wood, Opening, Beech, Oak }
    public enum Kind { Crown, Rack, Opening, Log, Young, Shelter, Clock, Repeat }
    public enum YoungShape { Circle, Oval, Diamond }
    [System.Flags] public enum Marks { None = 0, Band = 1, Ring = 2, Slash = 4 }

    public const int Columns = 14, Rows = 6;
    public const float FirstX = 65f, ColumnStep = 64f, FirstY = 116f, RowStep = 52f, CrownRadius = 22f;
    public const float LessonWidth = 1000f, LessonHeight = 520f, IntroWidth = 1200f, IntroHeight = 600f;
    // Quiet layers are drawn at this strength so the focus of the lesson stands out from its context.
    public const float QuietAlpha = 0.57f;
    // Stroke widths in design units; a stroke never gets thinner than Minimum however small a panel is scaled.
    public const float StrokeNormal = 3.8f, StrokeMark = 4.5f, StrokeMinimum = 3.8f;
    public const float RackFillAlpha = 0.26f, OpeningFillAlpha = 0.28f, FailedDiamondAlpha = 0.4f;
    public static readonly int[] RackColumns = { 4, 9 };

    // Stage numbers: 0 is the introduction, 1..8 are the lessons. For(lesson) takes the teaching sequence's own index (-1, 0..7).
    public const int IntroStage = 0, LastStage = 8;
    private const string FutureA = "t02_06", FutureB = "t04_11", CompetitorA = "t02_07", CompetitorB = "t03_07";

    // Per stage (introduction, lessons 1-8): A = not drawn yet, Q = quiet context, E = emphasised focus.
    //                                       intro 1 2 3 4 5 6 7 8
    private const string CrownsShow       = "EEQQQQQQQ";
    private const string RacksShow        = "AEQQQQQQQ";
    private const string FutureAShow      = "AAEEQQQQQ";   // the inspected future tree: blue band and amber inspection ring
    private const string FutureBShow      = "AAEEQEQQQ";   // a second future tree: blue band; its crown has grown from lesson 5 on
    private const string SlashesShow      = "AAAEAAAAA";   // the two selected competitors, marked for removal in lesson 3, felled from lesson 4
    private const string OpeningShow      = "AAAEQEQQQ";
    private const string LogShow          = "AAAAEQQQQ";
    private const string ClockShow        = "AAAAAEQEQ";
    private const string ExistingShow     = "AAAAAAEQQ";
    private const string PlantedShow      = "AAAAAAEEE";
    private const string ShelterShow      = "AAAAAAEQQ";
    private const string FailureShow      = "AAAAAAAEE";
    private const string RepeatShow       = "AAAAAAAAE";
    private const float GrownCrownRadius = 27f;
    private const int GrownFromStage = 5, FelledFromStage = 4;

    public sealed class Item
    {
        public string Id = "";
        public Kind Kind;
        public Role Role;
        public float X, Y;                 // centre (crown, opening, young tree, shelter, clock, repeat) or first end (rack, log)
        public float X1, Y1;               // second end (rack, log)
        public float Radius;               // crown, young tree, clock, repeat
        public float RadiusX, RadiusY;     // opening ellipse
        public float Width, Height;        // rack strip and log thickness (Width), shelter box
        public YoungShape Shape;
        public bool Quiet, Failed;
        public Marks Marks;
        public int Z;
        public float Alpha => Quiet ? QuietAlpha : 1f;
    }

    public sealed class Panel
    {
        public float X, Y, Scale = 1f;
        public List<Item> Items = new List<Item>();
    }

    public sealed class Text
    {
        public readonly string Value;
        public readonly float X, Y, Width, Height, FontSize;
        public readonly Role Role;
        public Text(string value, float x, float y, float width, float height, float fontSize, Role role)
        { Value = value; X = x; Y = y; Width = width; Height = height; FontSize = fontSize; Role = role; }
    }

    public sealed class Stage
    {
        public int Number;
        public string Id = "";
        public float Width, Height;
        public List<Item> Items = new List<Item>();      // lessons
        public List<Panel> Panels = new List<Panel>();   // introduction: the same plantation at a different scale, and the time cycle
        public Text[] Labels = new Text[0], Legend = new Text[0];
    }

    private static readonly string[] Ids =
        { "intro", "access", "future-trees", "release", "habitat", "response", "recruitment", "establishment", "transformation" };
    private static readonly Stage[] built = new Stage[LastStage + 1];

    public static string IdOf(int lesson) => Ids[lesson + 1];
    public static Stage For(int lesson)
    {
        int stage = lesson + 1;
        if (stage < IntroStage || stage > LastStage) stage = LastStage;
        return built[stage] ?? (built[stage] = Build(stage));
    }

    // The role colours, pre-blended against the picture's own dark surface for a given strength, so overlapping shapes never accumulate alpha.
    private static readonly byte[][] Palette =
    {
        new byte[] { 14, 22, 16 },      // Panel
        new byte[] { 61, 100, 68 },     // Canopy
        new byte[] { 132, 166, 119 },   // CanopyEdge
        new byte[] { 176, 190, 168 },   // Muted
        new byte[] { 232, 206, 120 },   // Accent
        new byte[] { 193, 162, 83 },    // Inspect: muted amber (inspection ring, shelter frame)
        new byte[] { 182, 118, 232 },   // Rack
        new byte[] { 70, 130, 230 },    // Crop: the blue future-tree band
        new byte[] { 214, 70, 58 },     // Fell: the red removal slash
        new byte[] { 150, 214, 140 },   // Good: existing young tree
        new byte[] { 240, 186, 110 },   // Warn: observed failure
        new byte[] { 170, 126, 75 },    // Wood
        new byte[] { 181, 159, 89 },    // Opening
        new byte[] { 169, 191, 116 },   // Beech
        new byte[] { 114, 182, 145 },   // Oak
    };

    public static void Blend(Role role, float alpha, out byte r, out byte g, out byte b)
    {
        byte[] c = Palette[(int)role], bg = Palette[(int)Role.Panel];
        r = (byte)System.Math.Round(c[0] * alpha + bg[0] * (1f - alpha));
        g = (byte)System.Math.Round(c[1] * alpha + bg[1] * (1f - alpha));
        b = (byte)System.Math.Round(c[2] * alpha + bg[2] * (1f - alpha));
    }

    private static bool Drawn(string layer, int stage) => layer[stage] != 'A';
    private static bool IsQuiet(string layer, int stage) => layer[stage] == 'Q';

    private static Item Crown(int row, int column, int stage, string layer)
    {
        return new Item
        {
            Id = "t" + row.ToString("00") + "_" + column.ToString("00"), Kind = Kind.Crown, Role = Role.Canopy,
            X = FirstX + column * ColumnStep, Y = FirstY + row * RowStep, Radius = CrownRadius, Z = 20, Quiet = IsQuiet(layer, stage)
        };
    }

    private static Stage Build(int stage)
    {
        var result = new Stage { Number = stage, Id = Ids[stage] };
        result.Labels = LabelsFor(stage);
        result.Legend = LegendFor(stage);
        if (stage == IntroStage)
        {
            result.Width = IntroWidth; result.Height = IntroHeight;
            // The whole unmarked plantation, smaller, beside a large time cycle: the stand you inherit, and that managing it takes years.
            var plantation = new Panel { X = 22f, Y = 115f, Scale = 0.79f };
            for (int row = 0; row < Rows; row++)
                for (int column = 0; column < Columns; column++)
                    plantation.Items.Add(Crown(row, column, stage, CrownsShow));
            result.Panels.Add(plantation);
            var cycle = new Panel();
            cycle.Items.Add(new Item { Id = "intro-cycle", Kind = Kind.Clock, Role = Role.Accent, X = 1000f, Y = 290f, Radius = 94f, Z = 30 });
            result.Panels.Add(cycle);
            return result;
        }

        result.Width = LessonWidth; result.Height = LessonHeight;
        var items = result.Items;
        for (int row = 0; row < Rows; row++)
            for (int column = 0; column < Columns; column++)
            {
                if (System.Array.IndexOf(RackColumns, column) >= 0) continue;     // the access strips replace these two columns of crowns
                Item crown = Crown(row, column, stage, CrownsShow);
                if ((crown.Id == CompetitorA || crown.Id == CompetitorB) && stage >= FelledFromStage) continue;   // felled
                if (crown.Id == CompetitorA || crown.Id == CompetitorB)
                {
                    if (Drawn(SlashesShow, stage)) { crown.Marks = Marks.Slash; crown.Quiet = IsQuiet(SlashesShow, stage); }
                }
                else if (crown.Id == FutureA && Drawn(FutureAShow, stage))
                {
                    crown.Marks = Marks.Band | Marks.Ring; crown.Quiet = IsQuiet(FutureAShow, stage);
                }
                else if (crown.Id == FutureB && Drawn(FutureBShow, stage))
                {
                    crown.Marks = Marks.Band; crown.Quiet = IsQuiet(FutureBShow, stage);
                    if (stage >= GrownFromStage) crown.Radius = GrownCrownRadius;
                }
                items.Add(crown);
            }
        if (Drawn(RacksShow, stage))
            foreach (int column in RackColumns)
                items.Add(new Item
                {
                    Id = "rack" + column, Kind = Kind.Rack, Role = Role.Rack, X = FirstX + column * ColumnStep, Y = 80f,
                    X1 = FirstX + column * ColumnStep, Y1 = 414f, Width = 42f, Z = 5, Quiet = IsQuiet(RacksShow, stage)
                });
        if (Drawn(OpeningShow, stage))
            items.Add(new Item { Id = "release-space", Kind = Kind.Opening, Role = Role.Accent, X = 513f, Y = 253f, RadiusX = 35f, RadiusY = 64f, Z = 10, Quiet = IsQuiet(OpeningShow, stage) });
        if (Drawn(LogShow, stage))
            items.Add(new Item { Id = "log1", Kind = Kind.Log, Role = Role.Wood, X = 490f, Y = 301f, X1 = 533f, Y1 = 298f, Width = 12f, Z = 25, Quiet = IsQuiet(LogShow, stage) });
        if (Drawn(ExistingShow, stage))
            items.Add(new Item { Id = "natural1", Kind = Kind.Young, Role = Role.Good, Shape = YoungShape.Circle, X = 506f, Y = 218f, Radius = 10f, Z = 30, Quiet = IsQuiet(ExistingShow, stage) });
        if (Drawn(PlantedShow, stage))
        {
            bool failed = Drawn(FailureShow, stage);
            items.Add(new Item { Id = "beech1", Kind = Kind.Young, Role = Role.Beech, Shape = YoungShape.Oval, X = 522f, Y = 252f, Radius = 13f, Z = 30, Quiet = IsQuiet(PlantedShow, stage), Failed = failed });
            items.Add(new Item { Id = "oak1", Kind = Kind.Young, Role = Role.Oak, Shape = YoungShape.Diamond, X = 504f, Y = 285f, Radius = 12f, Z = 30, Quiet = IsQuiet(PlantedShow, stage) });
        }
        if (Drawn(ShelterShow, stage))
            items.Add(new Item { Id = "shelter1", Kind = Kind.Shelter, Role = Role.Inspect, X = 522f, Y = 252f, Width = 34f, Height = 38f, Z = 35, Quiet = IsQuiet(ShelterShow, stage) });
        if (Drawn(ClockShow, stage))
            items.Add(new Item { Id = "time1", Kind = Kind.Clock, Role = Role.Accent, X = 920f, Y = 443f, Radius = 19f, Z = 40, Quiet = IsQuiet(ClockShow, stage) });
        if (Drawn(RepeatShow, stage))
            items.Add(new Item { Id = "repeat1", Kind = Kind.Repeat, Role = Role.Accent, X = 833f, Y = 443f, Radius = 23f, Z = 40, Quiet = IsQuiet(RepeatShow, stage) });
        result.Items = items.OrderBy(i => i.Z).ToList();     // stable: shapes of equal depth keep their insertion order
        return result;
    }

    // Picture labels and the (at most four) legend entries per picture: text, position and size in design units, from the approved design.
    private static Text[] LabelsFor(int stage)
    {
        switch (stage)
        {
            case 0: return new[]
            {
                new Text("Even-aged Sitka plantation", 40f, 43f, 610f, 30f, 28f, Role.Accent),
                new Text("Keep continuous tree cover", 40f, 485f, 650f, 30f, 28f, Role.Accent),
                new Text("Decide, observe, adapt", 854f, 120f, 340f, 30f, 28f, Role.Accent),
                new Text("Over many years", 886f, 449f, 280f, 30f, 28f, Role.Accent),
            };
            case 1: return new[]
            {
                new Text("Permanent access", 40f, 24f, 440f, 30f, 23f, Role.Accent),
                new Text("Retained rows", 530f, 24f, 440f, 30f, 23f, Role.Accent),
            };
            case 2: return new[]
            {
                new Text("Inspect, then choose", 40f, 24f, 440f, 30f, 23f, Role.Accent),
                new Text("Retain for development", 530f, 24f, 440f, 30f, 23f, Role.Accent),
            };
            case 3: return new[]
            {
                new Text("Remove selected competitors", 40f, 24f, 440f, 30f, 23f, Role.Accent),
                new Text("Keep the future tree", 530f, 24f, 440f, 30f, 23f, Role.Accent),
                new Text("Space after work", 40f, 435f, 440f, 30f, 23f, Role.Accent),
            };
            case 4: return new[]
            {
                new Text("Keep some fallen wood", 40f, 24f, 440f, 30f, 23f, Role.Accent),
                new Text("Forgo this timber sale", 530f, 24f, 440f, 30f, 23f, Role.Accent),
                new Text("Retained forest remains", 40f, 435f, 440f, 30f, 23f, Role.Accent),
            };
            case 5: return new[]
            {
                new Text("Observe the response", 40f, 24f, 440f, 30f, 23f, Role.Accent),
                new Text("Opening still unfilled", 530f, 24f, 440f, 30f, 23f, Role.Accent),
                new Text("Compare with intention", 40f, 435f, 440f, 30f, 23f, Role.Accent),
            };
            case 6: return new[]
            {
                new Text("Inspect existing regeneration", 40f, 24f, 440f, 30f, 23f, Role.Accent),
                new Text("Plant where justified", 530f, 24f, 440f, 30f, 23f, Role.Accent),
                new Text("Light, browsing, vegetation", 40f, 435f, 440f, 30f, 23f, Role.Accent),
                new Text("Optional shelter", 530f, 435f, 440f, 30f, 23f, Role.Accent),
            };
            case 7: return new[]
            {
                new Text("Compare with planting-year", 40f, 24f, 440f, 30f, 23f, Role.Accent),
                new Text("Some survive; some fail", 530f, 24f, 440f, 30f, 23f, Role.Accent),
                new Text("Read browsing, vegetation", 40f, 435f, 440f, 30f, 23f, Role.Accent),
            };
            case 8: return new[]
            {
                new Text("Retained forest continues", 40f, 24f, 440f, 30f, 23f, Role.Accent),
                new Text("Reassess before more work", 530f, 24f, 440f, 30f, 23f, Role.Accent),
                new Text("Young trees need review", 40f, 435f, 440f, 30f, 23f, Role.Accent),
            };
            default: return new Text[0];
        }
    }

    private static Text[] LegendFor(int stage)
    {
        switch (stage)
        {
            case 1: return new[]
            {
                new Text("Strip: access rack", 40f, 484f, 235f, 24f, 20f, Role.Muted),
                new Text("Disc: standing tree", 280f, 484f, 235f, 24f, 20f, Role.Muted),
            };
            case 2: return new[]
            {
                new Text("Strip: access rack", 40f, 484f, 235f, 24f, 20f, Role.Muted),
                new Text("Band: future tree", 280f, 484f, 235f, 24f, 20f, Role.Muted),
                new Text("Ring: inspect", 520f, 484f, 235f, 24f, 20f, Role.Muted),
            };
            case 3: return new[]
            {
                new Text("Band: future tree", 40f, 484f, 235f, 24f, 20f, Role.Muted),
                new Text("Ring: inspect", 280f, 484f, 235f, 24f, 20f, Role.Muted),
                new Text("Slash: remove", 520f, 484f, 235f, 24f, 20f, Role.Muted),
                new Text("Dashes: future space", 760f, 484f, 235f, 24f, 20f, Role.Muted),
            };
            case 4: return new[]
            {
                new Text("Band: future tree", 40f, 484f, 235f, 24f, 20f, Role.Muted),
                new Text("Ring: inspect", 280f, 484f, 235f, 24f, 20f, Role.Muted),
                new Text("Dashes: opening", 520f, 484f, 235f, 24f, 20f, Role.Muted),
                new Text("Bar: retained log", 760f, 484f, 235f, 24f, 20f, Role.Muted),
            };
            case 5: return new[]
            {
                new Text("Band: future tree", 40f, 484f, 235f, 24f, 20f, Role.Muted),
                new Text("Dashes: opening", 280f, 484f, 235f, 24f, 20f, Role.Muted),
                new Text("Bar: retained log", 520f, 484f, 235f, 24f, 20f, Role.Muted),
                new Text("Clock: time passed", 760f, 484f, 235f, 24f, 20f, Role.Muted),
            };
            case 6: return new[]
            {
                new Text("Small disc: existing", 40f, 484f, 235f, 24f, 20f, Role.Muted),
                new Text("Oval: planted beech", 280f, 484f, 235f, 24f, 20f, Role.Muted),
                new Text("Diamond: planted oak", 520f, 484f, 235f, 24f, 20f, Role.Muted),
                new Text("Box: optional shelter", 760f, 484f, 235f, 24f, 20f, Role.Muted),
            };
            case 7: return new[]
            {
                new Text("Small disc: existing", 40f, 484f, 235f, 24f, 20f, Role.Muted),
                new Text("Oval/diamond: plant", 280f, 484f, 235f, 24f, 20f, Role.Muted),
                new Text("Box: shelter retained", 520f, 484f, 235f, 24f, 20f, Role.Muted),
                new Text("Cross: failed", 760f, 484f, 235f, 24f, 20f, Role.Muted),
            };
            case 8: return new[]
            {
                new Text("Band: future tree", 40f, 484f, 235f, 24f, 20f, Role.Muted),
                new Text("Oval/diamond: plant", 280f, 484f, 235f, 24f, 20f, Role.Muted),
                new Text("Cross: failed", 520f, 484f, 235f, 24f, 20f, Role.Muted),
                new Text("Arrow: reassess", 760f, 484f, 235f, 24f, 20f, Role.Muted),
            };
            default: return new Text[0];
        }
    }
}
