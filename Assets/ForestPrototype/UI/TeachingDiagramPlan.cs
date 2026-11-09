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
//
// Exception (D-072 FR-T, #296): in a Rack V2 world only, Objective 1 is drawn from ObjectiveOneFigure instead of the conceptual stand above
// (ShowsFiveRackFigure). Every other world keeps For(0). See the section at the end.
public static class TeachingDiagramPlan
{
    // Roles and kinds after Oak / Chevron belong to the Objective 1 five-rack figure only; the earlier values keep their order.
    public enum Role { Panel, Canopy, CanopyEdge, Muted, Accent, Inspect, Rack, Crop, Fell, Good, Warn, Wood, Opening, Beech, Oak,
        Track, TrackCasing, Entrance, FieldMark, Harvested }
    public enum Kind { Crown, Rack, Opening, Log, Young, Shelter, Clock, Repeat, BroadleafCrown, Chevron,
        Corridor, Track, Pad, Entrance, FieldMark, Stump, NorthArrow, Boundary, Clearing }
    // A five-rack figure corridor: a proposal (dashed edges), a plan (solid edges, trees still inside) or a harvested, open strip.
    public enum CorridorState { Draft, Planned, Harvested }
    public enum YoungShape { Circle, Oval, Diamond }
    [System.Flags] public enum Marks { None = 0, Band = 1, Ring = 2, Slash = 4 }

    public const int Columns = 14, Rows = 6;
    public const float FirstX = 65f, ColumnStep = 64f, FirstY = 116f, RowStep = 52f, CrownRadius = 22f;
    public const float LessonWidth = 1000f, LessonHeight = 520f, IntroWidth = 1200f, IntroHeight = 520f;
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
        // Five-rack figure only: a track's centreline as x,y pairs; a corridor's state; the source identity a rack or entrance comes from.
        public float[] Points;
        public CorridorState State;
        public string SourceId = "";
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
        // A short tag drawn over the map gets a dark backing so it stays readable over tree symbols (five-rack figure only).
        public readonly bool Backed;
        public Text(string value, float x, float y, float width, float height, float fontSize, Role role)
            : this(value, x, y, width, height, fontSize, role, false) { }
        public Text(string value, float x, float y, float width, float height, float fontSize, Role role, bool backed)
        { Value = value; X = x; Y = y; Width = width; Height = height; FontSize = fontSize; Role = role; Backed = backed; }
    }

    public sealed class Stage
    {
        public int Number;
        public string Id = "";
        public float Width, Height;
        public List<Item> Items = new List<Item>();      // lessons
        public List<Panel> Panels = new List<Panel>();   // introduction: the same plantation at a different scale, and the time cycle
        public Text[] Labels = new Text[0], Legend = new Text[0];
        // Five-rack figure only: a title shown above the picture, notes shown below it, and a larger text floor (UI units) for its words.
        public string Title = "";
        public string[] Notes = new string[0];
        public float MinTextSize;
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
        new byte[] { 190, 158, 110 },   // Track: existing headland, feeder and approach (the Stand Map's tan path band)
        new byte[] { 58, 46, 30 },      // TrackCasing: the dark edge of an existing track
        new byte[] { 150, 214, 255 },   // Entrance: rack entrance square on the headland (the Stand Map's entrance blue)
        new byte[] { 236, 140, 44 },    // FieldMark: orange entrance-tree spray stripes
        new byte[] { 214, 206, 178 },   // Harvested: pale open corridor
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
            var plantation = new Panel { X = 8f, Y = 85f, Scale = 0.46f };
            var destination = new Panel { X = 744f, Y = 85f, Scale = 0.46f };
            for (int row = 0; row < Rows; row++)
                for (int column = 0; column < Columns; column++)
                {
                    plantation.Items.Add(Crown(row, column, stage, CrownsShow));
                    if (System.Array.IndexOf(RackColumns, column) >= 0) continue;
                    Item crown = Crown(row, column, stage, CrownsShow);
                    // Small grouped openings hold the next generation. Counts are diagram composition only.
                    int position = row * Columns + column;
                    bool gap = (row == 1 && column >= 5 && column <= 7)
                        || (row == 2 && column >= 5 && column <= 7)
                        || (row == 4 && column >= 10 && column <= 12)
                        || (row == 5 && column >= 10 && column <= 12)
                        || (row == 3 && column <= 1) || (row == 4 && column <= 1);
                    bool broadleaf = (row <= 1 && column >= 11)
                        || (row >= 4 && column >= 5 && column <= 7);
                    if (gap)
                    {
                        crown.Kind = Kind.Young; crown.Role = Role.Good;
                        crown.Radius = 9f;
                        crown.Shape = position % 7 == 0 ? YoungShape.Oval : YoungShape.Circle;
                        destination.Items.Add(crown);
                        destination.Items.Add(new Item { Id = crown.Id + "-seedling", Kind = Kind.Young, Role = Role.Good,
                            X = crown.X + 14f, Y = crown.Y + 12f, Radius = 5f, Z = 21 });
                    }
                    else
                    {
                        crown.Radius = position % 5 == 0 ? 29f : position % 3 == 0 ? 15f : 22f;
                        if (broadleaf)
                        {
                            crown.Kind = Kind.BroadleafCrown;
                            crown.Shape = row <= 1 ? YoungShape.Oval : YoungShape.Diamond;
                            crown.Role = row <= 1 ? Role.Beech : Role.Oak;
                        }
                        destination.Items.Add(crown);
                    }
                }
            foreach (int column in RackColumns)
                destination.Items.Add(new Item { Id = "intro-rack-" + column, Kind = Kind.Rack, Role = Role.Muted,
                    X = FirstX + column * ColumnStep, Y = 72f, Y1 = 420f, Width = 32f, Z = 5 });
            result.Panels.Add(plantation);
            result.Panels.Add(destination);
            result.Items.Add(new Item { Id = "intro-clock", Kind = Kind.Clock, Role = Role.Accent, X = 600f, Y = 230f, Radius = 48f, Z = 30 });
            result.Items.Add(new Item { Id = "intro-repeat", Kind = Kind.Repeat, Role = Role.Accent, X = 600f, Y = 340f, Radius = 30f, Z = 30 });
            for (int step = 0; step < 3; step++)
                result.Items.Add(new Item { Id = "intro-step-" + step, Kind = Kind.Chevron, Role = Role.Accent,
                    X = 550f + step * 50f, Y = 290f, Radius = 15f, Z = 30 });
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
                new Text("Inherited: one species, one age", 20f, 34f, 425f, 64f, 28f, Role.Accent),
                new Text("Created: many sizes, ages, species", 750f, 34f, 440f, 64f, 28f, Role.Accent),
                new Text("Repeated selective management", 468f, 110f, 270f, 80f, 28f, Role.Accent),
                new Text("Over many years", 475f, 400f, 265f, 40f, 26f, Role.Accent),
                new Text("Best Sitka kept", 768f, 340f, 390f, 34f, 27f, Role.Accent),
                new Text("Direction, not a forecast", 20f, 424f, 440f, 32f, 25f, Role.Muted),
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
            case 0: return new[]
            {
                new Text("Disc: Sitka", 20f, 488f, 190f, 26f, 22f, Role.Muted),
                new Text("Oval, diamond: beech, oak", 230f, 488f, 330f, 26f, 22f, Role.Muted),
                new Text("Small shapes: young trees", 570f, 488f, 330f, 26f, 22f, Role.Muted),
                new Text("Strip: access rack", 910f, 488f, 250f, 26f, 22f, Role.Muted),
            };
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

    // ======== Objective 1: illustrative five-rack plan (D-072 FR-T1..FR-T4, issue #296) ========
    //
    // In a Rack V2 world, Objective 1 shows the real Scenario One layout instead of the conceptual stand: five parallel planned racks entering
    // from the existing western headland, the single existing southern feeder and approach track, and standing trees between, beside and
    // inside the racks. It is static lesson art. It never reads a save or the live Stand Map, never changes either, and adds no road. Its title
    // says so.
    //
    // The headland and feeder exist only in a Rack V2 world, so the figure is shown only there. Legacy40, Reference Future v1, pre-V2 saves and
    // pre-V2 Enlarged80 continuations keep the original Objective 1 picture (For(0)) and its caption, which make no claim about a headland or
    // feeder. The caller decides which world it is (LearningObjectivesView already computes that for the card's words) and passes it in.
    //
    // Where each number comes from:
    // - Headland, feeder, junction pads, stand size and rack width: the C# authorities (ScenarioOneHeadlandGeometry, StandGeometryModel,
    //   RackGeometry). TeachingDiagram passes them in as a Site, so this file holds no second copy of that geometry.
    // - Rack entrance identities and Z: the five ScenarioOneSuggestedRackLayout entrance trees, read from an archived Enlarged80 Rack V2 world
    //   (Files/CCF/<FixtureSnapshotPath>, SHA-256 FixtureSnapshotSha256: save model 1/1, seed 20260914, 1,344 trees). The layout uses each
    //   entrance tree's own Z, so the gaps are uneven (about 18.85, 16.66, 15.56 and 19.44 m) and are kept that way. They describe that world,
    //   not every world: tree positions are seeded per world.
    // - Approach track and clearing: the ForestTest scene's "Dirt Path 51".."00" planks and "Forest Clearing", then StandApproachRoute.Build from
    //   "Dirt Path 00" (it meets the feeder at ScenarioOneHeadlandGeometry.PathX). Every other plank and the bend; cut off just past the edge.
    // Tools/Verification/ScenarioOneFiveRackDiagram re-derives all of these and checks the figure against them.
    //
    // Tree discs are schematic rows, not a census or the real planting pattern. The transform is uniform: FigurePxPerM design units per metre,
    // x east, north up (the Stand Map's orientation), no rotation.

    // The one place that decides which Objective 1 picture is drawn. rackV2World is the caller's existing Rack V2 eligibility for this
    // objective; nothing here looks it up.
    public static bool ShowsFiveRackFigure(int lesson, bool rackV2World) => lesson == 0 && rackV2World;

    public sealed class Site
    {
        public float StandSizeM, HeadlandX, HeadlandSouthZ, HeadlandNorthZ, FeederZ, FeederEndX, PathWidthM, PadRadiusM, RackWidthM;
    }

    public const string FigureTitle = "Illustrative five-rack plan — not your saved Stand Map. Planning or confirming does not clear the routes.";
    public const string FixtureSnapshotPath = "Verification/RackV2-Combined-20261027T1334Z/unity-combined-write/config/unity3d/DefaultCompany/CCF/forest-save.json";
    public const string FixtureSnapshotSha256 = "e8bb96f483e598e7449f370cf56f928503dc79ec7337307758eff23104b10ac5";
    // South to north, as ScenarioOneSuggestedRackLayout orders them.
    public static readonly string[] FixtureEntranceIds = { "PO0203", "PO1204", "PO2003", "PO2803", "PO3803" };
    public static readonly float[] FixtureEntranceZ = { -35.271175384521484f, -16.421764373779297f, 0.23823529481887817f, 15.793529510498047f, 35.23235321044922f };
    // Approach track centreline in metres (x, z pairs), north end at the clearing to the stub just past the south edge.
    public static readonly float[] FixtureTrackXZ =
    {
        -2.714831f, 10.75f, -2.443702f, 9.75f, -2.093611f, 8.75f, -1.675869f, 7.75f, -1.203976f, 6.75f, -0.693179f, 5.75f,
        -0.159984f, 4.75f, 0.378381f, 3.75f, 0.904519f, 2.75f, 1.40143f, 1.75f, 1.853057f, 0.75f, 2.244807f, -0.25f,
        2.564021f, -1.25f, 2.800385f, -2.25f, 2.946262f, -3.25f, 2.996937f, -4.25f, 2.950773f, -5.25f, 2.809261f, -6.25f,
        2.576976f, -7.25f, 2.261421f, -8.25f, 1.872794f, -9.25f, 1.423652f, -10.25f, 0.928509f, -11.25f, 0.403362f, -12.25f,
        -0.134818f, -13.25f, -0.668642f, -14.25f, -0.928509f, -14.75f, -1.3702906f, -15.647071f, -1.7588983f, -16.56828f,
        -2.0655363f, -17.519798f, -2.2757556f, -18.497128f, -2.3915963f, -19.490149f, -2.4327097f, -20.489193f, -2.436166f, -20.989182f,
        -2.436166f, -43.5f,
    };
    public const float FixtureClearingX = -2.771994f, FixtureClearingZ = 11f, FixtureClearingRadiusM = 4.5f;

    // Layout in design units (the same 1000 x 520 picture as the lessons): the map on the left, the state key on the right.
    public const float FigurePxPerM = 5.2f, FigureMapLeft = 64f, FigureMapTop = 40f;
    public const float FigureKeyX = 518f, FigureKeyTextX = 590f, FigureKeyTop = 14f, FigureKeyStep = 70f;
    // New words are never drawn below this many UI units (at 1280 x 720 one UI unit is 0.8 pixel, so 20 units is 16 pixels).
    public const float FigureMinTextSize = 20f, FigureFontSize = 22f;
    // Schematic tree discs: radius and spacing in metres, and the clearance kept from an existing track or a corridor edge.
    public const float TreeRadiusM = 1.5f, TreeColumnStepM = 4.2f, TreeRowStepM = 3.2f, TreeClearanceM = 0.2f;
    public const float EntranceSide = 11f, CasingExtra = 3f;

    public static float FigureX(Site site, float worldX) => FigureMapLeft + (worldX + site.StandSizeM * 0.5f) * FigurePxPerM;
    public static float FigureY(Site site, float worldZ) => FigureMapTop + (site.StandSizeM * 0.5f - worldZ) * FigurePxPerM;

    // The key: one row per state, shape plus words. The map itself shows existing tracks, five planned racks, entrances and standing trees.
    // Each row is two explicit short lines, so it never wraps to a third line and runs into the row below at 1280 x 720 (FigureKeyLineMax).
    // A planned rack is the confirmed plan: work pending, nothing cut. Draft, field-marked and harvested are separate rows with their own shapes.
    public const int FigureKeyLineMax = 27;
    public static readonly string[] FigureKey =
    {
        "Existing: A headland,\nB feeder, C track, clearing",
        "Draft rack: dashed,\na proposal only",
        "Planned = confirmed: solid,\nwork pending, trees stand",
        "Entrance: square\non the headland",
        "Field-marked: stripes,\nnot felled, not a Fell mark",
        "Harvested: open strip\nwith stumps",
        "Disc: standing tree,\nretained, not a tree count",
    };

    public static Stage ObjectiveOneFigure(Site site)
    {
        var stage = new Stage { Number = 1, Id = "access-five-rack", Width = LessonWidth, Height = LessonHeight, MinTextSize = FigureMinTextSize };
        var items = stage.Items;
        float half = site.StandSizeM * 0.5f, rackHalf = site.RackWidthM * 0.5f;
        float[] rackZ = FixtureEntranceZ;

        items.Add(new Item { Id = "property-boundary", Kind = Kind.Boundary, Role = Role.Muted, X = FigureX(site, -half), Y = FigureY(site, half),
            X1 = FigureX(site, half), Y1 = FigureY(site, -half), Z = 1 });
        items.Add(new Item { Id = "existing-clearing", Kind = Kind.Clearing, Role = Role.Track, X = FigureX(site, FixtureClearingX),
            Y = FigureY(site, FixtureClearingZ), Radius = FixtureClearingRadiusM * FigurePxPerM, Z = 2 });

        // Standing trees: rows in each of the six bands between and beside the corridors, and one row along each corridor (those trees
        // stand until approved work removes them).
        for (int band = 0; band <= rackZ.Length; band++)
        {
            float lo = band == 0 ? -half : rackZ[band - 1] + rackHalf;
            float hi = band == rackZ.Length ? half : rackZ[band] - rackHalf;
            float first = lo + TreeRadiusM + TreeClearanceM, last = hi - TreeRadiusM - TreeClearanceM;
            int rows = last <= first ? 1 : 1 + (int)System.Math.Floor((last - first) / TreeRowStepM);
            for (int row = 0; row < rows; row++)
            {
                float z = rows == 1 ? (lo + hi) * 0.5f : first + (last - first) * row / (rows - 1);
                AddTreeRow(site, items, "band" + band + "-r" + row, z, row % 2 == 1);
            }
        }
        for (int rack = 0; rack < rackZ.Length; rack++)
            AddTreeRow(site, items, "rack" + (rack + 1) + "-tree", rackZ[rack], false);

        // Five planned corridors from the headland entrance to the east edge, the layout's own endpoints and width.
        for (int rack = 0; rack < rackZ.Length; rack++)
            items.Add(new Item { Id = "rack" + (rack + 1), SourceId = FixtureEntranceIds[rack], Kind = Kind.Corridor, Role = Role.Rack,
                State = CorridorState.Planned, X = FigureX(site, site.HeadlandX), Y = FigureY(site, rackZ[rack]), X1 = FigureX(site, half),
                Y1 = FigureY(site, rackZ[rack]), Width = site.RackWidthM * FigurePxPerM, Z = 30 });

        // Existing infrastructure: every casing first, then every fill, so no dark edge cuts through where the surfaces join.
        float[] headland = { FigureX(site, site.HeadlandX), FigureY(site, site.HeadlandSouthZ), FigureX(site, site.HeadlandX), FigureY(site, site.HeadlandNorthZ) };
        float[] feeder = { FigureX(site, site.HeadlandX), FigureY(site, site.FeederZ), FigureX(site, site.FeederEndX), FigureY(site, site.FeederZ) };
        var approach = new float[FixtureTrackXZ.Length];
        for (int i = 0; i + 1 < FixtureTrackXZ.Length; i += 2)
        { approach[i] = FigureX(site, FixtureTrackXZ[i]); approach[i + 1] = FigureY(site, FixtureTrackXZ[i + 1]); }
        var surfaces = new[] { ("headland", headland), ("feeder", feeder), ("approach", approach) };
        float trackWidth = site.PathWidthM * FigurePxPerM;
        foreach (bool casing in new[] { true, false })
        {
            foreach (var (name, points) in surfaces)
                items.Add(new Item { Id = "existing-" + name + (casing ? "-casing" : ""), Kind = Kind.Track, Role = casing ? Role.TrackCasing : Role.Track,
                    Points = points, Width = trackWidth + (casing ? CasingExtra : 0f), Z = casing ? 40 : 41 });
            foreach (float padX in new[] { site.HeadlandX, site.FeederEndX })
                items.Add(new Item { Id = "existing-pad" + (padX == site.HeadlandX ? "-headland" : "-approach") + (casing ? "-casing" : ""), Kind = Kind.Pad,
                    Role = casing ? Role.TrackCasing : Role.Track, X = FigureX(site, padX), Y = FigureY(site, site.FeederZ),
                    Radius = site.PadRadiusM * FigurePxPerM + (casing ? CasingExtra * 0.5f : 0f), Z = casing ? 40 : 41 });
        }

        for (int rack = 0; rack < rackZ.Length; rack++)
            items.Add(new Item { Id = "entrance" + (rack + 1), SourceId = FixtureEntranceIds[rack], Kind = Kind.Entrance, Role = Role.Entrance,
                X = FigureX(site, site.HeadlandX), Y = FigureY(site, rackZ[rack]), Width = EntranceSide, Z = 50 });
        items.Add(new Item { Id = "north", Kind = Kind.NorthArrow, Role = Role.Accent, X = 32f, Y = 64f, Radius = 20f, Z = 60 });

        // The state key's shapes, one per row, beside its words.
        float x0 = FigureKeyX, x1 = FigureKeyX + 60f, xm = FigureKeyX + 30f;
        float KeyY(int row) => FigureKeyTop + row * FigureKeyStep + 25f;
        items.Add(new Item { Id = "key-existing-casing", Kind = Kind.Track, Role = Role.TrackCasing, Points = new[] { x0, KeyY(0), x1, KeyY(0) }, Width = trackWidth + CasingExtra, Z = 70 });
        items.Add(new Item { Id = "key-existing", Kind = Kind.Track, Role = Role.Track, Points = new[] { x0, KeyY(0), x1, KeyY(0) }, Width = trackWidth, Z = 71 });
        foreach (int row in new[] { 1, 2 })
        {
            items.Add(KeyTree("key-tree-" + row + "a", x0 + 15f, KeyY(row)));
            items.Add(KeyTree("key-tree-" + row + "b", x1 - 15f, KeyY(row)));
            items.Add(new Item { Id = row == 1 ? "key-draft" : "key-planned", Kind = Kind.Corridor, Role = Role.Rack,
                State = row == 1 ? CorridorState.Draft : CorridorState.Planned, X = x0, Y = KeyY(row), X1 = x1, Y1 = KeyY(row),
                Width = site.RackWidthM * FigurePxPerM, Z = 72 });
        }
        items.Add(new Item { Id = "key-entrance", Kind = Kind.Entrance, Role = Role.Entrance, X = xm, Y = KeyY(3), Width = EntranceSide, Z = 72 });
        items.Add(KeyTree("key-field-marked-tree", xm, KeyY(4)));
        items.Add(new Item { Id = "key-field-mark", Kind = Kind.FieldMark, Role = Role.FieldMark, X = xm, Y = KeyY(4), Radius = TreeRadiusM * FigurePxPerM, Z = 73 });
        items.Add(new Item { Id = "key-harvested", Kind = Kind.Corridor, Role = Role.Harvested, State = CorridorState.Harvested, X = x0, Y = KeyY(5),
            X1 = x1, Y1 = KeyY(5), Width = site.RackWidthM * FigurePxPerM, Z = 72 });
        items.Add(new Item { Id = "key-stump-a", Kind = Kind.Stump, Role = Role.Wood, X = x0 + 15f, Y = KeyY(5), Radius = 3.5f, Z = 73 });
        items.Add(new Item { Id = "key-stump-b", Kind = Kind.Stump, Role = Role.Wood, X = x1 - 15f, Y = KeyY(5), Radius = 3.5f, Z = 73 });
        items.Add(KeyTree("key-retained-tree", xm, KeyY(6)));

        stage.Items = items.OrderBy(i => i.Z).ToList();     // stable: equal depths keep their order

        var labels = new List<Text>();
        for (int row = 0; row < FigureKey.Length; row++)
            labels.Add(new Text(FigureKey[row], FigureKeyTextX, FigureKeyTop + row * FigureKeyStep + 8f, 1000f - FigureKeyTextX - 8f, 54f, FigureFontSize, Role.Muted));
        labels.Add(new Text("N", 21f, 88f, 24f, 28f, FigureFontSize, Role.Accent));
        for (int rack = 0; rack < rackZ.Length; rack++)
            labels.Add(new Text((rack + 1).ToString(System.Globalization.CultureInfo.InvariantCulture), FigureX(site, half) + 6f,
                FigureY(site, rackZ[rack]) - 14f, 24f, 28f, FigureFontSize, Role.Accent));
        float midNorthBand = (FigureY(site, rackZ[2]) + FigureY(site, rackZ[3])) * 0.5f;
        labels.Add(new Text("A", 38f, midNorthBand - 14f, 24f, 28f, FigureFontSize, Role.Accent));
        labels.Add(new Text("B", FigureX(site, -22f), FigureY(site, site.FeederZ) - 38f, 24f, 28f, FigureFontSize, Role.Accent, true));
        labels.Add(new Text("C", FigureX(site, site.FeederEndX) + 9f, FigureY(site, -half) + 2f, 24f, 28f, FigureFontSize, Role.Accent));
        stage.Labels = labels.ToArray();

        stage.Title = FigureTitle;
        stage.Notes = FigureNotes(site);
        return stage;
    }

    public static string[] FigureNotes(Site site)
    {
        float least = float.MaxValue, most = 0f;
        for (int i = 1; i < FixtureEntranceZ.Length; i++)
        {
            float gap = FixtureEntranceZ[i] - FixtureEntranceZ[i - 1];
            least = System.Math.Min(least, gap); most = System.Math.Max(most, gap);
        }
        var inv = System.Globalization.CultureInfo.InvariantCulture;
        // One short note (about two lines at the reading size) so the Objective 1 card still fits the screen without scrolling. What the
        // existing surfaces are, what planned/confirmed means and that trees are not counted is said in the key instead.
        return new[]
        {
            "Only Work Plan approval commits the work. 16–20 m between racks (" + least.ToString("0.0", inv) + "–" + most.ToString("0.0", inv)
                + " m here) is guidance for this plantation, not a hard rule. Approximate scale, north up.",
        };
    }

    private static Item KeyTree(string id, float x, float y) =>
        new Item { Id = id, Kind = Kind.Crown, Role = Role.Canopy, X = x, Y = y, Radius = TreeRadiusM * FigurePxPerM, Z = 70 };

    // One row of schematic trees at world z, from the headland's east side to the east edge, skipping existing tracks, pads and the clearing.
    private static void AddTreeRow(Site site, List<Item> items, string id, float z, bool offset)
    {
        float half = site.StandSizeM * 0.5f, pathHalf = site.PathWidthM * 0.5f;
        float west = site.HeadlandX + pathHalf + TreeRadiusM + TreeClearanceM, east = half - TreeRadiusM - TreeClearanceM;
        int column = 0;
        for (float x = west + (offset ? TreeColumnStepM * 0.5f : 0f); x <= east + 1e-3f; x += TreeColumnStepM, column++)
        {
            if (OnExistingSurface(site, x, z)) continue;
            items.Add(new Item { Id = id + column.ToString("00", System.Globalization.CultureInfo.InvariantCulture), Kind = Kind.Crown, Role = Role.Canopy,
                X = FigureX(site, x), Y = FigureY(site, z), Radius = TreeRadiusM * FigurePxPerM, Z = 20 });
        }
    }

    // True where a tree disc would touch the existing feeder, pads, approach track or clearing (metres).
    public static bool OnExistingSurface(Site site, float x, float z)
    {
        float reach = site.PathWidthM * 0.5f + TreeRadiusM + TreeClearanceM;
        if (SegmentDistance(x, z, site.HeadlandX, site.FeederZ, site.FeederEndX, site.FeederZ) < reach) return true;
        float padReach = site.PadRadiusM + TreeRadiusM + TreeClearanceM;
        if (SegmentDistance(x, z, site.HeadlandX, site.FeederZ, site.HeadlandX, site.FeederZ) < padReach
            || SegmentDistance(x, z, site.FeederEndX, site.FeederZ, site.FeederEndX, site.FeederZ) < padReach) return true;
        for (int i = 0; i + 3 < FixtureTrackXZ.Length; i += 2)
            if (SegmentDistance(x, z, FixtureTrackXZ[i], FixtureTrackXZ[i + 1], FixtureTrackXZ[i + 2], FixtureTrackXZ[i + 3]) < reach) return true;
        return SegmentDistance(x, z, FixtureClearingX, FixtureClearingZ, FixtureClearingX, FixtureClearingZ) < FixtureClearingRadiusM + TreeRadiusM * 0.5f;
    }

    public static float SegmentDistance(float x, float z, float ax, float az, float bx, float bz)
    {
        float dx = bx - ax, dz = bz - az, lengthSquared = dx * dx + dz * dz;
        float t = lengthSquared > 0f ? System.Math.Max(0f, System.Math.Min(1f, ((x - ax) * dx + (z - az) * dz) / lengthSquared)) : 0f;
        float ex = x - (ax + t * dx), ez = z - (az + t * dz);
        return (float)System.Math.Sqrt(ex * ex + ez * ez);
    }
}
