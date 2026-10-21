using System;
using System.Collections.Generic;
using System.Linq;

// Offline check of the shared map layer: StandMapTransform, StandMapModel, StandMapSymbols and StandMapDrawPlan. These are the
// PRODUCTION files (they use no UnityEngine). B8: one map, two presentations - identical world locations must give identical
// relative positions in the full map and in the minimap.
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

    public static int Main()
    {
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
        var rackColours = new[] { MapSymbol.RackToDo, MapSymbol.RackPlanned, MapSymbol.RackOpen, MapSymbol.RackOther, MapSymbol.RackPreview }.Select(s => StandMapSymbols.Get(s).Colour).Select(k => (k.R, k.G, k.B)).ToList();
        Check(rackColours.Distinct().Count() == rackColours.Count, "each rack state has its own colour");
        Check(!StandMapSymbols.IsDrawn(MapSymbol.RackPreview, MapDetail.Mini) && StandMapSymbols.IsDrawn(MapSymbol.RackPreview, MapDetail.Full), "rack-planning preview is full-map only");
        Check(Enum.GetValues(typeof(MapSymbol)).Cast<MapSymbol>().Where(s => StandMapSymbols.IsDrawn(s, MapDetail.Mini)).All(s => StandMapSymbols.IsDrawn(s, MapDetail.Full)), "everything on the minimap is also on the full map");
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
            MapDrawItem f = full.FirstOrDefault(x => x.Symbol == m.Symbol && x.SourceIndex == m.SourceIndex);
            if (f == null) { allMatched = false; continue; }
            float da = Math.Max(Math.Abs(m.Anchor.X / 200f - f.Anchor.X / 608f), Math.Abs(m.Anchor.Y / 200f - f.Anchor.Y / 608f));
            worstAnchor = Math.Max(worstAnchor, da);
            if (da > 1e-5f) anchorsSame = false;
            bool isLine = m.Symbol == MapSymbol.Path || m.Symbol == MapSymbol.Property || m.Symbol.ToString().StartsWith("Rack");
            if (isLine && m.Points.Count == f.Points.Count)
                for (int k = 0; k < m.Points.Count; k++)
                {
                    float dp = Math.Max(Math.Abs(m.Points[k].X / 200f - f.Points[k].X / 608f), Math.Abs(m.Points[k].Y / 200f - f.Points[k].Y / 608f));
                    worstPoint = Math.Max(worstPoint, dp);
                    if (dp > 1e-5f) linesSame = false;
                }
            else if (isLine) linesSame = false;
        }
        Check(allMatched, "every minimap item has a counterpart on the full map");
        Check(anchorsSame, "every item's anchor is at the same fractional position in both", worstAnchor.ToString("0.0e+0"));
        Check(linesSame, "every path, rack and boundary point is at the same fractional position in both", worstPoint.ToString("0.0e+0"));
        MapDrawItem pMini = small.First(i => i.Symbol == MapSymbol.Player), pFull = full.First(i => i.Symbol == MapSymbol.Player);
        RackPoint pBack = model.TransformFor(200f).PixelsToWorld(pMini.Anchor);
        Check(Near(pBack.X, -6.2f, 1e-3f) && Near(pBack.Z, 3.4f, 1e-3f), "the minimap player marker maps back to the player's world position");
        Check(Near(pMini.Points[0].X - pMini.Anchor.X, (float)Math.Sin(37 * Math.PI / 180) * StandMapDrawPlan.MarkerRadiusPx(200f) * 1.4f, 1e-3f), "player arrow tip points along the heading (minimap)");
        Check(Near(pFull.Points[0].X - pFull.Anchor.X, (float)Math.Sin(37 * Math.PI / 180) * StandMapDrawPlan.MarkerRadiusPx(608f) * 1.4f, 1e-3f), "player arrow tip points along the heading (full map)");
        Check(small.Count(i => i.Symbol == MapSymbol.RackToDo) == 3 && small.Count(i => i.Symbol == MapSymbol.RackPlanned) == 1 && small.Count(i => i.Symbol == MapSymbol.RackOpen) == 1 && small.Count(i => i.Symbol == MapSymbol.RackOther) == 1,
            "rack states are all carried to the minimap (3 to do, 1 planned, 1 open, 1 other)");

        // ---- the minimap shows no numbers or labels: plan items carry geometry and a symbol only ----
        Check(typeof(MapDrawItem).GetFields().All(f => f.FieldType != typeof(string)), "a draw item has no text field, so a presentation cannot print values from it");

        // ---- a change in the data reaches both from the same source ----
        int before = model.ContentHash();
        StandMapModel changed = Scenario(true);
        changed.Racks[3].Symbol = MapSymbol.RackPlanned;
        List<MapDrawItem> smallChanged = StandMapDrawPlan.Build(changed, 200f, MapDetail.Mini), fullChanged = StandMapDrawPlan.Build(changed, 608f, MapDetail.Full);
        Check(changed.ContentHash() != before, "confirming a rack changes the model's content hash");
        Check(smallChanged.Count(i => i.Symbol == MapSymbol.RackToDo) == 2 && smallChanged.Count(i => i.Symbol == MapSymbol.RackPlanned) == 2
            && fullChanged.Count(i => i.Symbol == MapSymbol.RackToDo) == 2 && fullChanged.Count(i => i.Symbol == MapSymbol.RackPlanned) == 2, "after the change both presentations show it (2 to do, 2 planned)");
        StandMapModel still = Scenario(true);
        still.Player = new RackPoint(-6.19f, 3.41f);
        Check(still.ContentHash() == before, "standing still (movement under the rounding step) does not change the hash");
        still.Player = new RackPoint(-5.0f, 3.4f);
        Check(still.ContentHash() != before, "walking changes the hash");
        List<MapDrawItem> again = StandMapDrawPlan.Build(model, 608f, MapDetail.Full);
        Check(again.Count == full.Count && again.Zip(full, (x, y) => x.Symbol == y.Symbol && x.Points.Count == y.Points.Count && Near(x.Anchor.X, y.Anchor.X, 1e-6f)).All(b => b), "the plan is deterministic");

        // ---- quiet style (the full map's analytical layers print numbers over the map) ----
        List<MapDrawItem> quiet = StandMapDrawPlan.Build(model, 608f, MapDetail.Full, true);
        bool sameShapes = quiet.Count == full.Count;
        for (int i = 0; sameShapes && i < quiet.Count; i++)
        {
            sameShapes = quiet[i].Symbol == full[i].Symbol && quiet[i].Points.Count == full[i].Points.Count && Near(quiet[i].Anchor.X, full[i].Anchor.X, 1e-6f) && Near(quiet[i].Anchor.Y, full[i].Anchor.Y, 1e-6f);
            for (int k = 0; sameShapes && k < quiet[i].Points.Count; k++)
                sameShapes = Near(quiet[i].Points[k].X, full[i].Points[k].X, 1e-6f) && Near(quiet[i].Points[k].Y, full[i].Points[k].Y, 1e-6f);
        }
        Check(sameShapes, "quiet style draws the same shapes at the same places");
        Check(quiet.Where(i => i.Symbol == MapSymbol.Path || i.Symbol == MapSymbol.RackToDo || i.Symbol == MapSymbol.RackPlanned).All(i => i.WidthPx <= 2f && i.Colour.A < 255),
            "quiet style: paths and racks are thin and translucent so a number under them stays readable");
        Check(quiet.Where(i => i.Symbol == MapSymbol.Player || i.Symbol == MapSymbol.Waypoint).All(i => !i.Filled), "quiet style: you and the destination are outlines, not solid");
        Check(quiet.Where(i => i.Symbol == MapSymbol.RackPreview).All(i => i.WidthPx == full.First(f => f.Symbol == MapSymbol.RackPreview).WidthPx), "quiet style leaves the rack being planned at full strength");
        Check(StandMapDrawPlan.Build(model, 200f, MapDetail.Mini).Zip(small, (a, b) => a.WidthPx == b.WidthPx && a.Filled == b.Filled).All(b => b), "the normal plan is not affected by the quiet option");

        // ---- degenerate inputs ----
        Check(StandMapDrawPlan.Build(new StandMapModel(), 200f, MapDetail.Mini).Count == 0, "no property: nothing drawn");
        Check(StandMapDrawPlan.Build(model, 0f, MapDetail.Mini).Count == 0, "zero size: nothing drawn");
        var noPlayer = Scenario(false); noPlayer.HasPlayer = false; noPlayer.HasWaypoint = false;
        Check(!StandMapDrawPlan.Build(noPlayer, 200f, MapDetail.Mini).Any(i => i.Symbol == MapSymbol.Player || i.Symbol == MapSymbol.Waypoint), "no player and no waypoint: neither is drawn");

        Console.WriteLine($"{passed} passed, {failed} failed");
        Console.WriteLine(failed == 0 ? "STANDMAP_OFFLINE_PASS" : "STANDMAP_OFFLINE_FAIL");
        return failed == 0 ? 0 : 1;
    }
}
