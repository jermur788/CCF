using System;
using System.Collections.Generic;
using System.Linq;

// Offline check of the shared map layer: StandMapTransform, StandMapModel, StandMapSymbols, StandMapDrawPlan, StandMapGridPlan,
// StandMapCanopyRaster, StandMapOverlayColours and StandMapLegend. These are the PRODUCTION files (they use no UnityEngine).
// B8: one map, two presentations - identical world locations must give identical relative positions in the full map and in the minimap,
// in the vector shapes AND in the forest picture.
//
// Changes from the Lane B version (SCENARIO-ONE-PROFESSIONAL-MAP-REDESIGN-V1), each because the presentation it tested was replaced:
//   * the "player arrow tip" checks became wedge-heading and player-dot checks (the arrow was replaced by a dot and a facing wedge);
//   * the "quiet style" checks were dropped (that style existed only to keep numbers readable in the old cell grid, which is gone);
//   * the draw plan is split into static and dynamic layers, and checks for that, the grid, the dashes, the rack band width, the marks and
//     the forest picture were added.
// Every other Lane B check is kept as it was.
public static class StandMapOfflineCheck
{
    static int passed, failed;
    static void Check(bool ok, string name, string detail = "") { if (ok) passed++; else failed++; Console.WriteLine((ok ? "PASS " : "FAIL ") + name + (detail.Length > 0 ? " " + detail : "")); }
    static bool Near(float a, float b, float tol = 1e-4f) => Math.Abs(a - b) <= tol;

    // A model like Enlarged80 Scenario One: 80 m property, 16 x 16 cells, a path, five layout racks, one off-layout rack, a preview.
    static StandMapModel Scenario(bool withPreview)
    {
        var m = new StandMapModel { HasProperty = true, MinX = -40f, MinZ = -40f, SizeM = 80f, CellsPerAxis = 16, CellSizeM = 5f };
        var path = new MapPath { WidthM = 2.6f };
        for (int i = 0; i < 52; i++) path.Points.Add(new RackPoint(-0.93f + 0.06f * i, -14.75f + 0.5f * i));
        m.Paths.Add(path);
        m.Landmarks.Add(new MapLandmark { At = new RackPoint(-2.77f, 11f), RadiusM = 4.5f, Name = "Forest Clearing" });
        float[] across = { -30.4f, -15.2f, 0.1f, 15.4f, 30.6f };
        MapSymbol[] states = { MapSymbol.RackOpen, MapSymbol.RackPlanned, MapSymbol.RackToDo, MapSymbol.RackToDo, MapSymbol.RackToDo };
        for (int i = 0; i < 5; i++)
            m.Racks.Add(new MapRack { Start = new RackPoint(-40f, across[i]), End = new RackPoint(40f, across[i]), Symbol = states[i], LayoutIndex = i });
        m.Racks.Add(new MapRack { Start = new RackPoint(7.3f, -40f), End = new RackPoint(7.3f, 40f), Symbol = MapSymbol.RackOther });
        if (withPreview)
            m.Racks.Add(new MapRack { Start = new RackPoint(-40f, 22.1f), End = new RackPoint(40f, 22.1f), Symbol = MapSymbol.RackPreview });
        m.HasWaypoint = true; m.Waypoint = new RackPoint(12.5f, -22.5f);
        m.HasPlayer = true; m.Player = new RackPoint(-6.2f, 3.4f); m.PlayerHeadingDegrees = 37f;
        return m;
    }

    static MapTree Tree(float x, float z, float crown, MapTreeKind kind = MapTreeKind.Conifer, MapTreeState state = MapTreeState.Living, MapTreeMark mark = MapTreeMark.None, int seed = 1)
        => new MapTree { X = x, Z = z, CrownRadiusM = crown, HeightM = 10f, DbhCm = 20f, Kind = kind, State = state, Mark = mark, Seed = seed };

    public static int Main()
    {
        // V2 lines are presentation only, and the minimap retains the existing legacy symbols.
        var v2 = new StandMapModel { HasProperty = true, MinX = -40, MinZ = -40, SizeM = 80 };
        v2.Racks.Add(new MapRack { Start = new RackPoint(-35, 0), End = new RackPoint(40, 0), Symbol = MapSymbol.RackSuggested });
        v2.Racks.Add(new MapRack { Start = new RackPoint(-35, 10), End = new RackPoint(40, 10), Symbol = MapSymbol.RackPlayer });
        v2.Racks.Add(new MapRack { Start = new RackPoint(-40, 20), End = new RackPoint(40, 20), Symbol = MapSymbol.RackToDo, MiniOnly = true });
        var v2Full = StandMapDrawPlan.BuildStatic(v2, 688, MapDetail.Full);
        var v2Mini = StandMapDrawPlan.BuildStatic(v2, 144, MapDetail.Mini);
        var suggested = v2Full.Single(x => x.Symbol == MapSymbol.RackSuggested);
        var player = v2Full.Single(x => x.Symbol == MapSymbol.RackPlayer);
        Check(suggested.DashPx > 0 && suggested.GapPx > 0 && player.DashPx == 0 && player.GapPx == 0, "V2 suggested is dashed and player rack solid");
        Check(!v2Full.Any(x => x.Symbol == MapSymbol.RackToDo) && v2Mini.Any(x => x.Symbol == MapSymbol.RackToDo), "legacy minimap-only rack is hidden from full map");
        Check(!v2Mini.Any(x => x.Symbol == MapSymbol.RackSuggested || x.Symbol == MapSymbol.RackPlayer), "V2 presentation does not expand the minimap");
        var v2Legend = StandMapLegend.Entries(v2, MapOverlay.None, true);
        Check(v2Legend.Any(x => x.Text == "Dashed = suggested") && v2Legend.Any(x => x.Text == "Solid = your planned rack"), "V2 legend names both line meanings");
        Check(!v2Legend.Any(x => x.Symbol == MapSymbol.RackToDo), "V2 full-map legend excludes minimap-only legacy rack");
        Check(StandMapLegend.Entries(v2, MapOverlay.None).Any(x => x.Text == "Solid = your planned rack"), "player rack has a legend outside planning mode");

        // ---- transform ----
        var t = new StandMapTransform(-40f, -40f, 80f, 608f);
        MapPosition nw = t.WorldToPixels(new RackPoint(-40f, 40f)), se = t.WorldToPixels(new RackPoint(40f, -40f)), c = t.WorldToPixels(new RackPoint(0f, 0f));
        Check(Near(nw.X, 0f) && Near(nw.Y, 0f), "north-west corner of the property is the top-left of the map");
        Check(Near(se.X, 608f) && Near(se.Y, 608f), "south-east corner is the bottom-right");
        Check(Near(c.X, 304f) && Near(c.Y, 304f), "the property centre is the map centre");
        MapPosition north = t.WorldToPixels(new RackPoint(0f, 20f)), east = t.WorldToPixels(new RackPoint(20f, 0f));
        Check(north.Y < c.Y && Near(north.X, c.X), "north is up the map");
        Check(east.X > c.X && Near(east.Y, c.Y), "east is to the right");
        Check(Near(t.PixelsPerMetre, 7.6f), "pixels per metre", t.PixelsPerMetre.ToString());

        var rng = new Random(20261021);
        float worst = 0f, worstRound = 0f;
        var mini = new StandMapTransform(-40f, -40f, 80f, 200f);
        for (int i = 0; i < 2000; i++)
        {
            var w = new RackPoint((float)(rng.NextDouble() * 80 - 40), (float)(rng.NextDouble() * 80 - 40));
            MapPosition a = t.WorldToPixels(w), b = mini.WorldToPixels(w);
            worst = Math.Max(worst, Math.Max(Math.Abs(a.X / 608f - b.X / 200f), Math.Abs(a.Y / 608f - b.Y / 200f)));
            RackPoint back = t.PixelsToWorld(a);
            worstRound = Math.Max(worstRound, Math.Max(Math.Abs(back.X - w.X), Math.Abs(back.Z - w.Z)));
            MapPosition fr = t.WorldToFraction(w);
            if (!Near(fr.X * 608f, a.X, 1e-2f) || !Near(fr.Y * 608f, a.Y, 1e-2f)) { worst = 1f; break; }
        }
        Check(worst < 1e-5f, "2000 random world points: identical fractional position at 608 px and at 200 px", worst.ToString("0.0e+0"));
        Check(worstRound < 1e-3f, "pixel -> world round trip", worstRound.ToString("0.0e+0"));

        // ---- the ecology cell grid sits on the same transform ----
        foreach (int n in new[] { 8, 16 })
        {
            float size = n * 5f;
            var tg = new StandMapTransform(-size / 2f, -size / 2f, size, 608f);
            float cellPx = tg.CellSidePx(n);
            bool tiles = true, centres = true;
            for (int index = 0; index < n * n; index++)
            {
                int x = index % n, z = index / n;
                MapPosition tl = tg.CellTopLeft(index, n);
                if (!Near(tl.X, x * cellPx, 1e-2f) || !Near(tl.Y, (n - 1 - z) * cellPx, 1e-2f)) tiles = false;
                // The ecology cell centre formula: origin + (index + 0.5) * cell.
                MapPosition centre = tg.WorldToPixels(new RackPoint(-size / 2f + (x + 0.5f) * 5f, -size / 2f + (z + 0.5f) * 5f));
                if (!Near(centre.X, tl.X + cellPx / 2f, 1e-2f) || !Near(centre.Y, tl.Y + cellPx / 2f, 1e-2f)) centres = false;
            }
            Check(tiles, $"{n}x{n} cells tile the map exactly (row 0 at the bottom, north at the top)");
            Check(centres, $"{n}x{n}: each cell's centre is where the transform puts the cell's world centre");
        }

        // ---- orientation ----
        Check(Near(StandMapTransform.HeadingOf(0f, 1f), 0f) && Near(StandMapTransform.HeadingOf(1f, 0f), 90f)
            && Near(Math.Abs(StandMapTransform.HeadingOf(0f, -1f)), 180f) && Near(StandMapTransform.HeadingOf(-1f, 0f), -90f), "world direction -> compass heading (N 0, E 90, S 180, W -90)");
        MapPosition dn = StandMapTransform.HeadingToDirection(0f), de = StandMapTransform.HeadingToDirection(90f), ds = StandMapTransform.HeadingToDirection(180f), dw = StandMapTransform.HeadingToDirection(270f);
        Check(Near(dn.X, 0f) && Near(dn.Y, -1f) && Near(de.X, 1f) && Near(de.Y, 0f) && Near(ds.Y, 1f) && Near(dw.X, -1f), "heading -> map direction (north is up the screen)");

        // ---- symbols ----
        foreach (MapSymbol s in Enum.GetValues(typeof(MapSymbol)))
            Check(StandMapSymbols.Get(s) != null && StandMapSymbols.Get(s).ColourWord.Length > 0 && StandMapSymbols.Get(s).Meaning.Length > 0, "symbol has one style and legend words: " + s);
        var rackStates = new[] { MapSymbol.RackToDo, MapSymbol.RackPlanned, MapSymbol.RackOpen, MapSymbol.RackOther, MapSymbol.RackPreview };
        var rackColours = rackStates.Select(s => StandMapSymbols.Get(s).Colour).Select(k => (k.R, k.G, k.B)).ToList();
        Check(rackColours.Distinct().Count() == rackColours.Count, "each rack state has its own colour");
        var rackLooks = rackStates.Select(s => StandMapSymbols.Get(s)).Select(k => (k.DashPx, k.GapPx, k.MinPx)).ToList();
        Check(rackLooks.Distinct().Count() == rackLooks.Count, "each rack state also differs by line pattern or width, so colour never carries the meaning alone");
        Check(StandMapSymbols.Get(MapSymbol.RackToDo).DashPx > 0f && StandMapSymbols.Get(MapSymbol.RackPlanned).DashPx == 0f && StandMapSymbols.Get(MapSymbol.RackOther).DashPx > 0f && StandMapSymbols.Get(MapSymbol.RackOther).DashPx <= 3f,
            "rack to plan is dashed, rack in the Work Plan is solid, off-layout rack is dotted");
        Check(!StandMapSymbols.IsDrawn(MapSymbol.RackPreview, MapDetail.Mini) && StandMapSymbols.IsDrawn(MapSymbol.RackPreview, MapDetail.Full), "rack-planning preview is full-map only");
        Check(Enum.GetValues(typeof(MapSymbol)).Cast<MapSymbol>().Where(s => StandMapSymbols.IsDrawn(s, MapDetail.Mini)).All(s => StandMapSymbols.IsDrawn(s, MapDetail.Full)), "everything on the minimap is also on the full map");
        var fullOnly = new[] { MapSymbol.GridLine, MapSymbol.ScaleBar, MapSymbol.TreeMarkFell, MapSymbol.TreeMarkCrop, MapSymbol.Juvenile, MapSymbol.Shelter, MapSymbol.ShelterLapsed, MapSymbol.RackPreview };
        Check(fullOnly.All(s => !StandMapSymbols.IsDrawn(s, MapDetail.Mini)), "the minimap hides the grid, the scale, marks, juveniles, shelters and the rack being planned");
        Check(StandMapSymbols.Legend(MapDetail.Full).Contains("rack you are planning") && !StandMapSymbols.Legend(MapDetail.Mini).Contains("rack you are planning"), "legend follows the same filter");

        // ---- one model, two presentations ----
        StandMapModel model = Scenario(true);
        List<MapDrawItem> full = StandMapDrawPlan.Build(model, 608f, MapDetail.Full), small = StandMapDrawPlan.Build(model, 200f, MapDetail.Mini);
        Check(full.Count > 0 && small.Count > 0, "both presentations draw something", $"full {full.Count}, mini {small.Count}");
        Check(small.All(i => i.Symbol != MapSymbol.RackPreview), "the minimap draws no rack-planning preview");
        Check(full.Any(i => i.Symbol == MapSymbol.RackPreview), "the full map draws the rack-planning preview");
        Check(small.Select(i => i.Symbol).Distinct().All(s => StandMapSymbols.Get(s).InMini), "every minimap item is a symbol the shared table marks for the minimap");
        bool allMatched = true, anchorsSame = true, linesSame = true; float worstAnchor = 0f, worstPoint = 0f;
        foreach (MapDrawItem m in small)
        {
            // The north arrow is page furniture at a fixed inset, not a place on the property.
            if (m.Symbol == MapSymbol.NorthArrow) continue;
            MapDrawItem f = full.FirstOrDefault(x => x.Symbol == m.Symbol && x.SourceIndex == m.SourceIndex);
            if (f == null) { allMatched = false; continue; }
            float da = Math.Max(Math.Abs(m.Anchor.X / 200f - f.Anchor.X / 608f), Math.Abs(m.Anchor.Y / 200f - f.Anchor.Y / 608f));
            worstAnchor = Math.Max(worstAnchor, da);
            if (da > 1e-5f) anchorsSame = false;
            bool isShape = m.Symbol == MapSymbol.Path || m.Symbol == MapSymbol.PathCasing || m.Symbol == MapSymbol.Property || m.Symbol.ToString().StartsWith("Rack");
            if (isShape && m.Points.Count == f.Points.Count)
                for (int k = 0; k < m.Points.Count; k++)
                {
                    float dp = Math.Max(Math.Abs(m.Points[k].X / 200f - f.Points[k].X / 608f), Math.Abs(m.Points[k].Y / 200f - f.Points[k].Y / 608f));
                    worstPoint = Math.Max(worstPoint, dp);
                    if (dp > 1e-5f) linesSame = false;
                }
            else if (isShape) linesSame = false;
        }
        Check(allMatched, "every minimap item has a counterpart on the full map");
        Check(anchorsSame, "every item's anchor is at the same fractional position in both", worstAnchor.ToString("0.0e+0"));
        Check(linesSame, "every path, rack, rack corridor and boundary point is at the same fractional position in both", worstPoint.ToString("0.0e+0"));
        MapDrawItem pMini = small.First(i => i.Symbol == MapSymbol.Player), pFull = full.First(i => i.Symbol == MapSymbol.Player);
        MapDrawItem dMini = small.First(i => i.Symbol == MapSymbol.PlayerDot), dFull = full.First(i => i.Symbol == MapSymbol.PlayerDot);
        RackPoint pBack = model.TransformFor(200f).PixelsToWorld(dMini.Anchor);
        Check(Near(pBack.X, -6.2f, 1e-3f) && Near(pBack.Z, 3.4f, 1e-3f), "the minimap player dot maps back to the player's world position");
        Check(Near(pMini.Points[0].X, dMini.Anchor.X, 1e-3f) && Near(pMini.Points[0].Y, dMini.Anchor.Y, 1e-3f) && Near(pFull.Points[0].X, dFull.Anchor.X, 1e-3f), "the facing wedge's apex is exactly on the player dot");
        float hMini = StandMapDrawPlan.HeadingOfWedge(pMini), hFull = StandMapDrawPlan.HeadingOfWedge(pFull);
        Check(Near(hMini, 37f, 0.05f) && Near(hFull, 37f, 0.05f), "the wedge points along the heading, read back from the drawn shape", $"mini {hMini:0.###} full {hFull:0.###}");
        for (float heading = -180f; heading < 180f; heading += 15f)
        {
            StandMapModel hm = Scenario(false); hm.PlayerHeadingDegrees = heading;
            float read = StandMapDrawPlan.HeadingOfWedge(StandMapDrawPlan.BuildDynamic(hm, 608f, MapDetail.Full).First(i => i.Symbol == MapSymbol.Player));
            float diff = Math.Abs(((read - heading + 540f) % 360f) - 180f);
            if (diff > 0.05f) { Check(false, "wedge heading round trip", $"heading {heading} read {read}"); break; }
            if (heading >= 165f) Check(true, "wedge heading round trip every 15 degrees");
        }
        Check(Math.Abs(StandMapDrawPlan.WedgeLengthPx(608f) - StandMapDrawPlan.MarkerRadiusPx(608f) * 4.2f) < 1e-3f && StandMapDrawPlan.MarkerRadiusPx(608f) <= 6f, "you are a small dot (about 5 px on the full map), not an oversized marker");
        Check(small.Count(i => i.Symbol == MapSymbol.RackToDo) == 3 && small.Count(i => i.Symbol == MapSymbol.RackPlanned) == 1 && small.Count(i => i.Symbol == MapSymbol.RackOpen) == 1 && small.Count(i => i.Symbol == MapSymbol.RackOther) == 1,
            "rack states are all carried to the minimap (3 to do, 1 planned, 1 open, 1 other)");

        // ---- the minimap shows no numbers or labels: plan items carry geometry and a symbol only ----
        Check(typeof(MapDrawItem).GetFields().All(f => f.FieldType != typeof(string)), "a draw item has no text field, so a presentation cannot print values from it");

        // ---- static and dynamic layers ----
        List<MapDrawItem> st = StandMapDrawPlan.BuildStatic(model, 608f, MapDetail.Full), dy = StandMapDrawPlan.BuildDynamic(model, 608f, MapDetail.Full);
        Check(st.All(i => i.Layer == MapDrawLayer.Static) && dy.All(i => i.Layer == MapDrawLayer.Dynamic), "items are tagged with their layer");
        var dynamicSymbols = new HashSet<MapSymbol> { MapSymbol.Player, MapSymbol.PlayerDot, MapSymbol.Waypoint, MapSymbol.RackPreview };
        Check(dy.All(i => dynamicSymbols.Contains(i.Symbol) || (i.Symbol == MapSymbol.RackBand && model.Racks[i.SourceIndex].Symbol == MapSymbol.RackPreview)), "the dynamic layer holds only you, the destination and the rack being planned");
        Check(st.All(i => !dynamicSymbols.Contains(i.Symbol) && !(i.Symbol == MapSymbol.RackBand && model.Racks[i.SourceIndex].Symbol == MapSymbol.RackPreview)), "the static layer holds none of them");
        Check(st.Count + dy.Count == full.Count, "static plus dynamic is the whole plan", $"{st.Count}+{dy.Count} vs {full.Count}");

        // ---- versions: a step repaints only the dynamic layer ----
        var v = Scenario(true);
        v.Restamp();
        int s0 = v.StaticVersion, d0 = v.DynamicVersion, ver0 = v.Version;
        v.Player = new RackPoint(-6.19f, 3.41f); v.Restamp();
        Check(v.StaticVersion == s0 && v.DynamicVersion == d0 && v.Version == ver0, "standing still (movement under the rounding step) moves no version");
        v.Player = new RackPoint(-4.0f, 3.4f); v.Restamp();
        Check(v.StaticVersion == s0 && v.DynamicVersion == d0 + 1 && v.Version == ver0 + 1, "walking moves the dynamic version and not the static one");
        v.Racks[3].Symbol = MapSymbol.RackPlanned; v.Restamp();
        Check(v.StaticVersion == s0 + 1 && v.DynamicVersion == d0 + 1, "confirming a rack moves the static version and not the dynamic one");
        v.Racks.RemoveAll(r => r.Symbol == MapSymbol.RackPreview); v.Restamp();
        Check(v.DynamicVersion == d0 + 2 && v.StaticVersion == s0 + 1, "dropping the rack being planned moves only the dynamic version");
        v.Trees.Add(Tree(1f, 1f, 2f)); bool canopy = v.StampTrees(); v.Restamp();
        Check(canopy && v.CanopyVersion == 1, "a new tree moves the canopy version");
        int cv = v.CanopyVersion, mv = v.MarkVersion;
        MapTree marked = v.Trees[0]; marked.Mark = MapTreeMark.Fell; v.Trees[0] = marked;
        bool canopyAgain = v.StampTrees();
        Check(!canopyAgain && v.CanopyVersion == cv && v.MarkVersion == mv + 1, "marking a tree moves the mark version and does NOT redraw the canopy picture");
        MapTree grown = v.Trees[0]; grown.CrownRadiusM = 2.6f; v.Trees[0] = grown;
        Check(v.StampTrees() && v.CanopyVersion == cv + 1, "a tree growing moves the canopy version");
        int before = model.ContentHash();
        StandMapModel changed = Scenario(true);
        changed.Racks[3].Symbol = MapSymbol.RackPlanned;
        List<MapDrawItem> smallChanged = StandMapDrawPlan.Build(changed, 200f, MapDetail.Mini), fullChanged = StandMapDrawPlan.Build(changed, 608f, MapDetail.Full);
        Check(changed.ContentHash() != before, "confirming a rack changes the model's content hash");
        Check(smallChanged.Count(i => i.Symbol == MapSymbol.RackToDo) == 2 && smallChanged.Count(i => i.Symbol == MapSymbol.RackPlanned) == 2
            && fullChanged.Count(i => i.Symbol == MapSymbol.RackToDo) == 2 && fullChanged.Count(i => i.Symbol == MapSymbol.RackPlanned) == 2, "after the change both presentations show it (2 to do, 2 planned)");
        List<MapDrawItem> again = StandMapDrawPlan.Build(model, 608f, MapDetail.Full);
        Check(again.Count == full.Count && again.Zip(full, (x, y) => x.Symbol == y.Symbol && x.Points.Count == y.Points.Count && Near(x.Anchor.X, y.Anchor.X, 1e-6f)).All(b => b), "the plan is deterministic");

        // ---- local grid, scale and north: derived, not invented ----
        Check(Near(StandMapGridPlan.StepM(80f), 10f) && Near(StandMapGridPlan.StepM(40f), 5f), "grid step: 10 m on the 80 m property, 5 m on the 40 m property");
        Check(StandMapGridPlan.InteriorOffsets(80f).Count == 7 && StandMapGridPlan.InteriorOffsets(80f).SequenceEqual(new[] { 10f, 20f, 30f, 40f, 50f, 60f, 70f }), "the 80 m property has seven interior grid lines each way");
        List<StandMapGridPlan.GridLabel> labels = StandMapGridPlan.Labels(model);
        Check(labels.Where(l => l.Easting).Select(l => l.Text).SequenceEqual(new[] { "0", "20", "40", "60", "80" }) && labels.Where(l => !l.Easting).Select(l => l.Text).SequenceEqual(new[] { "0", "20", "40", "60", "80" }), "grid labels are local metres from the south-west corner: 0, 20, 40, 60, 80");
        Check(Near(model.LocalEasting(model.MinX), 0f) && Near(model.LocalNorthing(model.MinZ + model.SizeM), 80f) && Near(model.LocalEasting(0f), 40f), "local easting and northing are measured from the south-west corner");
        Check(StandMapGridPlan.ScaleBarMetres(80f) == 20f && StandMapGridPlan.ScaleBarText(20f) == "20 m" && StandMapGridPlan.ScaleBarMetres(40f) == 10f, "scale bar: 20 m on 80 m, 10 m on 40 m");
        StandMapGridPlan.Furniture f608 = StandMapGridPlan.Layout(80f, 608f, MapDetail.Full);
        Check(Near((f608.BarX1 - f608.BarX0) / t.PixelsPerMetre, 20f, 1e-3f), "the scale bar is exactly 20 m long at the map's own scale");
        var gridLines = st.Where(i => i.Symbol == MapSymbol.GridLine).ToList();
        Check(gridLines.Count == 14 && StandMapDrawPlan.Build(model, 200f, MapDetail.Mini).All(i => i.Symbol != MapSymbol.GridLine && i.Symbol != MapSymbol.ScaleBar), "14 grid lines on the full map; the minimap has no grid and no scale bar");
        Check(st.Any(i => i.Symbol == MapSymbol.NorthArrow) && small.Any(i => i.Symbol == MapSymbol.NorthArrow), "north is marked on both");
        // dashes
        var pieces = new List<MapPosition[]>();
        StandMapGridPlan.Dashes(new[] { new MapPosition(0, 0), new MapPosition(100, 0) }, 10f, 5f, pieces);
        Check(pieces.Count == 7 && Near(pieces.Sum(p => p[p.Length - 1].X - p[0].X), 70f, 1e-3f), "a 100 px line with 10 px dashes and 5 px gaps has 7 dashes totalling 70 px", $"{pieces.Count}");
        var bent = new List<MapPosition[]>();
        StandMapGridPlan.Dashes(new[] { new MapPosition(0, 0), new MapPosition(50, 0), new MapPosition(50, 50) }, 10f, 5f, bent);
        float bentLength = bent.Sum(p => { float s = 0; for (int i = 1; i < p.Length; i++) s += (float)Math.Sqrt((p[i].X - p[i - 1].X) * (p[i].X - p[i - 1].X) + (p[i].Y - p[i - 1].Y) * (p[i].Y - p[i - 1].Y)); return s; });
        Check(Near(bentLength, 70f, 1e-2f), "the dash pattern carries around a corner (same 70 px of dash in 100 px of line)", bentLength.ToString());
        var solid = new List<MapPosition[]>();
        StandMapGridPlan.Dashes(new[] { new MapPosition(0, 0), new MapPosition(100, 0) }, 0f, 0f, solid);
        Check(solid.Count == 1 && solid[0].Length == 2, "no pattern returns the whole line");

        // ---- racks: corridor at its real width; paths with an edge ----
        MapDrawItem band = st.First(i => i.Symbol == MapSymbol.RackBand && model.Racks[i.SourceIndex].LayoutIndex == 2);
        float bandWidthPx = (float)Math.Sqrt((band.Points[0].X - band.Points[3].X) * (band.Points[0].X - band.Points[3].X) + (band.Points[0].Y - band.Points[3].Y) * (band.Points[0].Y - band.Points[3].Y));
        Check(Near(bandWidthPx / t.PixelsPerMetre, RackGeometry.DefaultWidthM, 1e-3f), "a rack corridor is drawn at the real rack width", $"{bandWidthPx / t.PixelsPerMetre:0.###} m vs {RackGeometry.DefaultWidthM}");
        Check(st.Count(i => i.Symbol == MapSymbol.RackBand) == model.Racks.Count(r => r.Symbol != MapSymbol.RackPreview) && dy.Count(i => i.Symbol == MapSymbol.RackBand) == 1, "one corridor per rack, the one being planned in the dynamic layer");
        Check(st.Count(i => i.Symbol == MapSymbol.PathCasing) == st.Count(i => i.Symbol == MapSymbol.Path) && st.First(i => i.Symbol == MapSymbol.PathCasing).WidthPx > st.First(i => i.Symbol == MapSymbol.Path).WidthPx, "the path has a wider dark edge under it");
        Check(Near(st.First(i => i.Symbol == MapSymbol.Path).WidthPx, 2.6f * t.PixelsPerMetre, 1e-3f), "the path is drawn at its real width");
        Check(st.First(i => i.Symbol == MapSymbol.RackToDo).DashPx > 0f && st.First(i => i.Symbol == MapSymbol.RackPlanned).DashPx == 0f, "the dash pattern travels with the rack item, so one rack stays one item");
        Check(st.First(i => i.Symbol == MapSymbol.Property).DashPx > 0f && st.First(i => i.Symbol == MapSymbol.Property).Closed, "the property boundary is a closed dashed line");

        // ---- analytical layers are overlays on the same plan ----
        var layered = Scenario(false);
        layered.Trees.Add(Tree(0f, 0f, 2f, mark: MapTreeMark.Fell)); layered.Trees.Add(Tree(5f, 5f, 2f, mark: MapTreeMark.Crop)); layered.Trees.Add(Tree(9f, 9f, 2f));
        layered.Trees.Add(Tree(12f, 12f, 2f, mark: MapTreeMark.Fell, state: MapTreeState.Stump));
        layered.Shelters.Add(new MapShelter { X = 3f, Z = 3f, Effective = true }); layered.Shelters.Add(new MapShelter { X = 4f, Z = 4f, Effective = false });
        layered.Juveniles.Add(new MapJuvenile { X = 2f, Z = 2f, HeightM = 0.4f, Kind = MapTreeKind.Beech });
        List<MapDrawItem> none = StandMapDrawPlan.BuildStatic(layered, 608f, MapDetail.Full, MapOverlay.None), marksPlan = StandMapDrawPlan.BuildStatic(layered, 608f, MapDetail.Full, MapOverlay.Marks), browsePlan = StandMapDrawPlan.BuildStatic(layered, 608f, MapDetail.Full, MapOverlay.Browsing);
        Check(none.All(i => i.Symbol != MapSymbol.TreeMarkFell && i.Symbol != MapSymbol.TreeMarkCrop && i.Symbol != MapSymbol.Shelter && i.Symbol != MapSymbol.ShelterLapsed), "the ordinary plan carries no mark or shelter rings");
        Check(marksPlan.Count(i => i.Symbol == MapSymbol.TreeMarkFell) == 1 && marksPlan.Count(i => i.Symbol == MapSymbol.TreeMarkCrop) == 1, "the Marks layer rings each marked LIVING tree (a marked stump gets none)");
        Check(browsePlan.Count(i => i.Symbol == MapSymbol.Shelter) == 1 && browsePlan.Count(i => i.Symbol == MapSymbol.ShelterLapsed) == 1, "the Browsing layer rings each shelter, effective and lapsed apart");
        Check(none.Count(i => i.Symbol == MapSymbol.Juvenile) == 1 && none.Count(i => i.Symbol != MapSymbol.TreeMarkFell && i.Symbol != MapSymbol.TreeMarkCrop) == marksPlan.Count(i => i.Symbol != MapSymbol.TreeMarkFell && i.Symbol != MapSymbol.TreeMarkCrop), "a layer adds to the same base plan; it never replaces it");
        Check(StandMapDrawPlan.Build(layered, 200f, MapDetail.Mini, MapOverlay.Marks).All(i => i.Symbol != MapSymbol.TreeMarkFell && i.Symbol != MapSymbol.Juvenile && i.Symbol != MapSymbol.Shelter), "the minimap draws no marks, juveniles or shelters even when asked for a layer");
        MapDrawItem ring = marksPlan.First(i => i.Symbol == MapSymbol.TreeMarkFell);
        RackPoint ringBack = layered.TransformFor(608f).PixelsToWorld(ring.Anchor);
        Check(Near(ringBack.X, 0f, 1e-3f) && Near(ringBack.Z, 0f, 1e-3f), "a mark ring is centred on its tree's world position");
        MapColour dark = StandMapOverlayColours.Light(0f), open = StandMapOverlayColours.Light(1f), mid = StandMapOverlayColours.Light(0.5f);
        Check(dark.B > dark.R && dark.A > 120 && open.R > open.B && mid.A < dark.A && mid.A < open.A, "Light: shade washes blue, open ground warm, the middle is nearly clear");
        Check(StandMapOverlayColours.Regeneration(0f, false, false).A == 0 && StandMapOverlayColours.Regeneration(0.3f, true, false).A < StandMapOverlayColours.Regeneration(3f, true, false).A, "Regeneration: none is clear and the wash strengthens with the tallest cohort");
        Check(StandMapOverlayColours.Regeneration(0f, false, true).A > 0, "a planted juvenile alone lifts a square");

        // ---- the forest picture ----
        var forest = new StandMapModel { HasProperty = true, MinX = -40f, MinZ = -40f, SizeM = 80f, CellsPerAxis = 16, CellSizeM = 5f };
        forest.Trees.Add(Tree(-20f, 12f, 3f, MapTreeKind.Beech, seed: 5));
        forest.Trees.Add(Tree(25f, -18f, 2.4f, MapTreeKind.Conifer, seed: 9));
        forest.Trees.Add(Tree(0f, 30f, 0.2f, MapTreeKind.Conifer, MapTreeState.Stump, seed: 11));
        foreach (int side in new[] { StandMapCanopyRaster.FullSidePx, StandMapCanopyRaster.MiniSidePx })
        {
            MapDetail detail = side == StandMapCanopyRaster.FullSidePx ? MapDetail.Full : MapDetail.Mini;
            float ppm = side / 80f;
            // RELATIVE placement and scale. Two identical crowns placed point-symmetrically about the origin: the light from the north-west biases
            // where a crown's brightest pixels are, but identically for both, so the bias cancels in the difference of their centroids.
            var pair = new StandMapModel { HasProperty = true, MinX = -40f, MinZ = -40f, SizeM = 80f, CellsPerAxis = 16, CellSizeM = 5f };
            pair.Trees.Add(Tree(-21.5f, 13f, 3f, MapTreeKind.Beech, seed: 5));
            pair.Trees.Add(Tree(21.5f, -13f, 3f, MapTreeKind.Beech, seed: 5));
            byte[] px = StandMapCanopyRaster.Render(pair, side, detail);
            (double x, double y, int n) Centroid(bool firstHalf)
            {
                double sx = 0, sy = 0; int count = 0;
                for (int y = 0; y < side; y++) for (int x = 0; x < side; x++)
                {
                    bool inHalf = firstHalf ? (x < side / 2) : (x >= side / 2);
                    int o = (y * side + x) * 4;
                    if (inHalf && px[o + 1] > 95 && px[o + 1] > px[o + 2] + 28 && px[o] > 50) { sx += x + 0.5; sy += y + 0.5; count++; }
                }
                return count > 0 ? (sx / count, sy / count, count) : (0, 0, 0);
            }
            var westC = Centroid(true); var eastC = Centroid(false);
            double ddx = eastC.x - westC.x, ddy = eastC.y - westC.y;
            Check(westC.n > 0 && eastC.n > 0 && Math.Abs(ddx - 43f * ppm) < 1.0 && Math.Abs(ddy - (-26f * ppm)) < 1.0 && Math.Abs(westC.n - eastC.n) <= westC.n / 20,
                $"forest picture {side}px: two crowns 43 m east and 26 m south of each other are that far apart on the picture", $"got ({ddx:0.0},{ddy:0.0}) expected ({43f * ppm:0.0},{-26f * ppm:0.0}); {westC.n}/{eastC.n} px");
            Check(westC.n > 0 && Math.Abs(Math.Sqrt(westC.n / Math.PI) - 3f * ppm) < 0.28 * 3f * ppm, $"forest picture {side}px: the crown is drawn at the tree's own radius", westC.n > 0 ? $"drawn radius {Math.Sqrt(westC.n / Math.PI) / ppm:0.00} m vs 3.00 m" : "");
            int farOffset = (int)((7f + 40f) * ppm) * side * 4 + (int)((-33f + 40f) * ppm) * 4;
            Check(Math.Abs(px[farOffset] - StandMapSymbols.Ground.R) < 22 && Math.Abs(px[farOffset + 1] - StandMapSymbols.Ground.G) < 22 && px[farOffset + 3] == 255, $"forest picture {side}px: open ground is the ground colour, fully opaque");
        }
        // ABSOLUTE placement (full map): a stump is an unlit pale disc, so its centroid is exactly where the world says it stands.
        {
            int side = StandMapCanopyRaster.FullSidePx; float ppm = side / 80f;
            var one = new StandMapModel { HasProperty = true, MinX = -40f, MinZ = -40f, SizeM = 80f, CellsPerAxis = 16, CellSizeM = 5f };
            one.Trees.Add(Tree(-12.3f, 21.7f, 0.2f, MapTreeKind.Conifer, MapTreeState.Stump));
            byte[] px = StandMapCanopyRaster.Render(one, side, MapDetail.Full);
            double sx = 0, sy = 0; int n = 0;
            for (int y = 0; y < side; y++) for (int x = 0; x < side; x++) { int o = (y * side + x) * 4; if (px[o] > 140 && px[o + 1] > 120) { sx += x + 0.5; sy += y + 0.5; n++; } }
            double ex = (-12.3 + 40) * ppm, ey = (21.7 + 40) * ppm;
            Check(n > 0 && Math.Abs(sx / n - ex) < 0.6 && Math.Abs(sy / n - ey) < 0.6, "forest picture 768px: a stump is exactly where the transform says its world position is", n > 0 ? $"got ({sx / n:0.00},{sy / n:0.00}) expected ({ex:0.00},{ey:0.00})" : "no stump pixels");
        }
        byte[] fullPx = StandMapCanopyRaster.Render(forest, StandMapCanopyRaster.FullSidePx, MapDetail.Full), miniPx = StandMapCanopyRaster.Render(forest, StandMapCanopyRaster.MiniSidePx, MapDetail.Mini);
        int stumpFull = ((int)((30f + 40f) * StandMapCanopyRaster.FullSidePx / 80f) * StandMapCanopyRaster.FullSidePx + (int)(40f * StandMapCanopyRaster.FullSidePx / 80f)) * 4;
        int stumpMini = ((int)((30f + 40f) * StandMapCanopyRaster.MiniSidePx / 80f) * StandMapCanopyRaster.MiniSidePx + (int)(40f * StandMapCanopyRaster.MiniSidePx / 80f)) * 4;
        Check(fullPx[stumpFull] > 150 && miniPx[stumpMini] < 90, "a stump is drawn pale on the full map and left off the minimap");
        Check(StandMapCanopyRaster.Render(forest, 256, MapDetail.Mini).SequenceEqual(miniPx), "the forest picture is deterministic: the same trees give the same bytes");
        // time-sliced drawing gives the same picture as drawing at once, and the job does not see later changes to the model
        var sliced = new byte[256 * 256 * 4];
        var job = new StandMapCanopyRaster.Job(forest, 256, MapDetail.Mini, sliced);
        int steps = 0; while (!job.Step(1, 64)) steps++;
        Check(sliced.SequenceEqual(miniPx) && steps >= 4, "drawing a few trees at a time gives exactly the same picture as drawing at once", $"{steps + 1} steps");
        var snap = new byte[256 * 256 * 4];
        var snapJob = new StandMapCanopyRaster.Job(forest, 256, MapDetail.Mini, snap);
        forest.Trees.Clear();
        snapJob.Finish();
        Check(snap.SequenceEqual(miniPx), "a redraw works from its own copy of the trees, so a change to the live list cannot tear it");
        byte[] emptyPicture = StandMapCanopyRaster.Render(new StandMapModel(), 64, MapDetail.Mini);
        Check(emptyPicture.Length == 64 * 64 * 4 && Enumerable.Range(0, 64 * 64).All(i => emptyPicture[i * 4 + 3] == 255 && Math.Abs(emptyPicture[i * 4] - StandMapSymbols.Ground.R) < 22), "no property: just the ground colour, no trees and no failure");
        var crowded = new StandMapModel { HasProperty = true, MinX = -40f, MinZ = -40f, SizeM = 80f, CellsPerAxis = 16, CellSizeM = 5f };
        for (int i = 0; i < 1400; i++) crowded.Trees.Add(Tree((float)(rng.NextDouble() * 80 - 40), (float)(rng.NextDouble() * 80 - 40), 1.2f + (float)rng.NextDouble() * 1.5f, MapTreeKind.Conifer, seed: rng.Next()));
        var swatch = new System.Diagnostics.Stopwatch(); swatch.Start();
        StandMapCanopyRaster.Render(crowded, StandMapCanopyRaster.MiniSidePx, MapDetail.Mini); swatch.Stop();
        Check(swatch.ElapsedMilliseconds < 200, "1,400 trees draw on the minimap picture quickly (diagnostic, plain .NET)", swatch.ElapsedMilliseconds + " ms");
        byte[] crownSwatch = StandMapCanopyRaster.RenderCrownSwatch(MapTreeKind.Oak, MapTreeState.Living, 22);
        Check(crownSwatch.Length == 22 * 22 * 4 && crownSwatch[(11 * 22 + 11) * 4 + 3] == 255 && crownSwatch[3] == 0, "a legend crown swatch is a crown on a transparent square");

        // ---- stand figures ----
        var figures = new StandMapModel { HasProperty = true, MinX = -40f, MinZ = -40f, SizeM = 80f };
        for (int i = 0; i < 64; i++) figures.Trees.Add(Tree(i, i, 2f));   // DBH 20 cm each
        figures.Trees.Add(Tree(1f, 2f, 2f, state: MapTreeState.StandingDead)); figures.Trees.Add(Tree(2f, 3f, 0f, state: MapTreeState.Stump));
        StandMapStats stats = figures.ComputeStats();
        Check(stats.LivingTrees == 64 && stats.StandingDead == 1 && stats.Stumps == 1 && Near(stats.MeanDbhCm, 20f) && Near(stats.StemsPerHectare, 100f, 1e-3f), "stand figures count living, dead and stumps separately", $"{stats.LivingTrees} {stats.MeanDbhCm} {stats.StemsPerHectare}");
        Check(Near(stats.BasalAreaM2PerHectare, (float)(64 * Math.PI * 0.1 * 0.1 / 0.64), 1e-3f), "basal area is the cross-section of the living stems per hectare");

        // ---- the legend names only what exists ----
        var legendModel = Scenario(false);
        List<LegendEntry> bare = StandMapLegend.Entries(legendModel, MapOverlay.None);
        string[] words = bare.Select(e => e.Text.ToLowerInvariant()).ToArray();
        Check(!bare.Any(e => e.Kind == LegendKind.Crown || e.Kind == LegendKind.Stump || e.Kind == LegendKind.Dot), "no trees, stumps or juveniles on the map: no rows for them");
        Check(words.Any(w => w.Contains("you")) && words.Any(w => w.Contains("destination")) && words.Any(w => w.Contains("boundary")) && words.Any(w => w.Contains("grid")), "you, destination, boundary and grid are listed");
        string[] forbidden = { "contour", "stream", "water", "pond", "wetland", "rock", "track", "trail", "building", "yard", "elevation" };
        Check(words.All(w => !forbidden.Any(f => w.Contains(f))), "the legend lists nothing the scenario does not have (no contours, water, rock, tracks or buildings)");
        legendModel.Trees.Add(Tree(0, 0, 2f)); legendModel.Trees.Add(Tree(3, 3, 2f, MapTreeKind.Beech)); legendModel.Trees.Add(Tree(6, 6, 2f, state: MapTreeState.StandingDead)); legendModel.Trees.Add(Tree(7, 7, 0f, state: MapTreeState.Stump));
        legendModel.Juveniles.Add(new MapJuvenile { X = 1, Z = 1, Kind = MapTreeKind.Oak });
        legendModel.KindLabels[(int)MapTreeKind.Conifer] = "Sitka spruce"; legendModel.KindLabels[(int)MapTreeKind.Beech] = "European beech";
        List<LegendEntry> stocked = StandMapLegend.Entries(legendModel, MapOverlay.None);
        Check(stocked.Any(e => e.Text == "Sitka spruce") && stocked.Any(e => e.Text == "European beech") && !stocked.Any(e => e.Text.Contains("Oak") && e.Kind == LegendKind.Crown) && stocked.Any(e => e.Kind == LegendKind.Stump) && stocked.Any(e => e.Text == "Standing dead tree") && stocked.Any(e => e.Kind == LegendKind.Dot), "trees are named by their species, and only the kinds that stand are listed");
        // Light adds its ramp; the base legend has no ramp or ring rows. The Marks and Browsing ring rows are tested below, one category at a time.
        Check(StandMapLegend.Entries(legendModel, MapOverlay.Light).Any(e => e.Kind == LegendKind.Ramp) && StandMapLegend.Entries(legendModel, MapOverlay.None).All(e => e.Kind != LegendKind.Ramp && e.Kind != LegendKind.Ring), "a layer adds its own legend rows and the base legend has none of them");
        // The Marks and Browsing legends list a ring only for a category that is on the map (the draw plan rings exactly these things).
        Func<StandMapModel, MapOverlay, string> ringRows = (m, o) => string.Join("|", StandMapLegend.Entries(m, o).Where(e => e.Kind == LegendKind.Ring).Select(e => e.Symbol.ToString()));
        var plain = Scenario(false);
        Check(ringRows(plain, MapOverlay.Marks) == "" && ringRows(plain, MapOverlay.Browsing) == "", "nothing marked and no shelters: the Marks and Browsing legends have no ring rows");
        var felled = Scenario(false); felled.Trees.Add(Tree(1, 1, 2f, mark: MapTreeMark.Fell));
        var cropped = Scenario(false); cropped.Trees.Add(Tree(1, 1, 2f, mark: MapTreeMark.Crop));
        var mixedMarks = Scenario(false); mixedMarks.Trees.Add(Tree(1, 1, 2f, mark: MapTreeMark.Fell)); mixedMarks.Trees.Add(Tree(4, 4, 2f, mark: MapTreeMark.Crop));
        Check(ringRows(felled, MapOverlay.Marks) == "TreeMarkFell" && ringRows(cropped, MapOverlay.Marks) == "TreeMarkCrop" && ringRows(mixedMarks, MapOverlay.Marks) == "TreeMarkFell|TreeMarkCrop", "the Marks legend lists fell, crop or both, exactly what is marked");
        var markedButGone = Scenario(false); markedButGone.Trees.Add(Tree(1, 1, 2f, state: MapTreeState.StandingDead, mark: MapTreeMark.Fell)); markedButGone.Trees.Add(Tree(2, 2, 0f, state: MapTreeState.Stump, mark: MapTreeMark.Crop));
        Check(ringRows(markedButGone, MapOverlay.Marks) == "" && !StandMapDrawPlan.Build(markedButGone, 608f, MapDetail.Full, MapOverlay.Marks).Any(i => i.Symbol == MapSymbol.TreeMarkFell || i.Symbol == MapSymbol.TreeMarkCrop), "a mark on a tree that is no longer living is neither drawn nor listed");
        mixedMarks.Trees.RemoveAt(0);
        Check(ringRows(mixedMarks, MapOverlay.Marks) == "TreeMarkCrop", "when the last fell mark goes, its legend row goes");
        mixedMarks.Trees.Clear();
        Check(ringRows(mixedMarks, MapOverlay.Marks) == "", "when the last mark goes, the Marks legend has no ring rows");
        var effectiveOnly = Scenario(false); effectiveOnly.Shelters.Add(new MapShelter { X = 2, Z = 2, Effective = true });
        var lapsedOnly = Scenario(false); lapsedOnly.Shelters.Add(new MapShelter { X = 2, Z = 2, Effective = false });
        var bothShelters = Scenario(false); bothShelters.Shelters.Add(new MapShelter { X = 2, Z = 2, Effective = true }); bothShelters.Shelters.Add(new MapShelter { X = 6, Z = 6, Effective = false });
        Check(ringRows(effectiveOnly, MapOverlay.Browsing) == "Shelter" && ringRows(lapsedOnly, MapOverlay.Browsing) == "ShelterLapsed" && ringRows(bothShelters, MapOverlay.Browsing) == "Shelter|ShelterLapsed", "the Browsing legend lists effective, lapsed or both, exactly what is ringed");
        bothShelters.Shelters.RemoveAt(0);
        Check(ringRows(bothShelters, MapOverlay.Browsing) == "ShelterLapsed", "when the last effective shelter goes, its legend row goes");
        Check(ringRows(felled, MapOverlay.Browsing) == "" && ringRows(effectiveOnly, MapOverlay.Marks) == "" && ringRows(felled, MapOverlay.None) == "", "a ring row appears only on its own layer");

        // ---- degenerate inputs ----
        Check(StandMapDrawPlan.Build(new StandMapModel(), 200f, MapDetail.Mini).Count == 0, "no property: nothing drawn");
        Check(StandMapDrawPlan.Build(model, 0f, MapDetail.Mini).Count == 0, "zero size: nothing drawn");
        var noPlayer = Scenario(false); noPlayer.HasPlayer = false; noPlayer.HasWaypoint = false;
        Check(!StandMapDrawPlan.Build(noPlayer, 200f, MapDetail.Mini).Any(i => i.Symbol == MapSymbol.Player || i.Symbol == MapSymbol.PlayerDot || i.Symbol == MapSymbol.Waypoint), "no player and no waypoint: neither is drawn");
        Check(StandMapLegend.Entries(new StandMapModel(), MapOverlay.None).Count == 0, "no property: an empty legend");

        Console.WriteLine($"{passed} passed, {failed} failed");
        Console.WriteLine(failed == 0 ? "STANDMAP_OFFLINE_PASS" : "STANDMAP_OFFLINE_FAIL");
        return failed == 0 ? 0 : 1;
    }
}
