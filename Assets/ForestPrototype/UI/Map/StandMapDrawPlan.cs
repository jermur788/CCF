using System;
using System.Collections.Generic;

// Which of the two vector layers an item belongs to. The static layer holds everything that does not move while you walk (so it is
// laid out once per change, not once per step); the dynamic layer holds you, the destination and the rack being planned.
public enum MapDrawLayer { Static, Dynamic }

// One thing to paint: a stroked line (open or closed) or a filled shape, already placed in pixels.
public sealed class MapDrawItem
{
    public MapSymbol Symbol;
    public MapDrawLayer Layer;
    public List<MapPosition> Points = new List<MapPosition>();
    public bool Closed, Filled;
    // A line's width. For a filled shape this is unused; its outline is Stroke / StrokePx.
    public float WidthPx;
    // The stroke colour of a line, or the fill colour of a filled shape.
    public MapColour Colour;
    // An outline for a filled shape (StrokePx 0 = none).
    public MapColour Stroke;
    public float StrokePx;
    // A dash pattern along a line, in pixels (both 0 = solid). The canvas cuts the dashes when it paints, so one rack stays ONE item.
    public float DashPx, GapPx;
    // The world position this item stands for (a marker's centre, or a line's start), kept so a check can compare the
    // same world point across presentations.
    public RackPoint World;
    // Where that world point lands on this map, in pixels (the marker's centre, a line's first point). The same world point has
    // the same Anchor / side on every presentation.
    public MapPosition Anchor;
    // Index into the model list the item came from, or -1.
    public int SourceIndex = -1;
}

// Turns the shared model into shapes for one presentation: the single place that decides WHAT is drawn WHERE. The Unity canvas
// only paints these shapes, so the full map and the minimap cannot place anything differently. Mini and Full differ only in the
// symbol filter and in the side length handed in.
//
// The forest itself (the canopy) is not in this plan: it is a picture drawn from the same model by StandMapCanopyRaster, laid under
// these shapes with the same transform.
public static class StandMapDrawPlan
{
    private const int CirclePoints = 20;
    private const float WedgeHalfAngleDegrees = 30f;
    private const int WedgeArcPoints = 6;

    // You and the destination scale a little with the map, so they take about the same share of a small and a large map, but they
    // stay small: the map is the thing to read, not the marker.
    public static float MarkerRadiusPx(float sidePx) => Math.Max(4f, sidePx * 0.0075f);
    public static float WaypointRadiusPx(float sidePx) => Math.Max(5f, sidePx * 0.0115f);
    // How far the facing wedge reaches ahead of you.
    public static float WedgeLengthPx(float sidePx) => MarkerRadiusPx(sidePx) * 4.2f;
    // The rack-editing handle and entrance marker. The pointer test uses the same radius (plus a little slack), so what is drawn is
    // what can be grabbed.
    public static float HandleRadiusPx(float sidePx) => Math.Max(6f, sidePx * 0.0112f);
    public static float EntranceHalfPx(float sidePx) => Math.Max(4.5f, sidePx * 0.0075f);

    // The whole plan, static layer first, then the dynamic layer on top.
    public static List<MapDrawItem> Build(StandMapModel model, float sidePx, MapDetail detail, MapOverlay overlay = MapOverlay.None)
    {
        List<MapDrawItem> plan = BuildStatic(model, sidePx, detail, overlay);
        plan.AddRange(BuildDynamic(model, sidePx, detail));
        return plan;
    }

    public static List<MapDrawItem> BuildStatic(StandMapModel model, float sidePx, MapDetail detail, MapOverlay overlay = MapOverlay.None)
    {
        var plan = new List<MapDrawItem>();
        if (model == null || !model.HasProperty || sidePx <= 0f) return plan;
        StandMapTransform t = model.TransformFor(sidePx);
        float ppm = t.PixelsPerMetre;
        bool full = detail == MapDetail.Full;

        // The local grid: fine lines every 10 m (full map only).
        if (StandMapSymbols.IsDrawn(MapSymbol.GridLine, detail))
        {
            MapSymbolStyle style = StandMapSymbols.Get(MapSymbol.GridLine);
            foreach (float d in StandMapGridPlan.InteriorOffsets(model.SizeM))
            {
                plan.Add(Line(MapSymbol.GridLine, style, 1f, t.WorldToPixels(new RackPoint(model.MinX + d, model.MinZ)), t.WorldToPixels(new RackPoint(model.MinX + d, model.MinZ + model.SizeM))));
                plan.Add(Line(MapSymbol.GridLine, style, 1f, t.WorldToPixels(new RackPoint(model.MinX, model.MinZ + d)), t.WorldToPixels(new RackPoint(model.MinX + model.SizeM, model.MinZ + d))));
            }
        }

        // The clearing: a pale patch of open ground with a fine edge.
        if (StandMapSymbols.IsDrawn(MapSymbol.Landmark, detail))
            for (int i = 0; i < model.Landmarks.Count; i++)
            {
                MapLandmark landmark = model.Landmarks[i];
                MapSymbolStyle style = StandMapSymbols.Get(MapSymbol.Landmark);
                var item = new MapDrawItem { Symbol = MapSymbol.Landmark, Layer = MapDrawLayer.Static, Closed = true, Filled = true, Colour = style.Fill, Stroke = style.Colour, StrokePx = style.MinPx, SourceIndex = i, World = landmark.At };
                float radius = Math.Max(3f, t.MetresToPixels(landmark.RadiusM));
                MapPosition c = t.WorldToPixels(landmark.At);
                for (int k = 0; k < CirclePoints; k++)
                {
                    double a = 2.0 * Math.PI * k / CirclePoints;
                    item.Points.Add(new MapPosition(c.X + radius * (float)Math.Cos(a), c.Y + radius * (float)Math.Sin(a)));
                }
                item.Anchor = t.WorldToPixels(item.World);
                plan.Add(item);
            }

        // Rack corridors: the real width of the strip a rack opens (2.4 m), under everything else on the ground.
        for (int i = 0; i < model.Racks.Count; i++)
        {
            MapRack rack = model.Racks[i];
            if ((rack.MiniOnly && full) || (rack.FullOnly && !full) || StandMapModel.IsDynamicRack(rack.Symbol) || !StandMapSymbols.IsDrawn(rack.Symbol, detail)) continue;
            plan.Add(RackBand(model, t, rack, i, StandMapSymbols.Get(rack.Symbol), MapDrawLayer.Static));
        }

        // The forest path: a dark edge, then the tan band at its real width.
        if (StandMapSymbols.IsDrawn(MapSymbol.Path, detail))
            for (int i = 0; i < model.Paths.Count; i++)
            {
                MapPath path = model.Paths[i];
                if (path.Points.Count < 2) continue;
                MapSymbolStyle style = StandMapSymbols.Get(MapSymbol.Path), edge = StandMapSymbols.Get(MapSymbol.PathCasing);
                float core = Math.Max(style.MinPx, t.MetresToPixels(path.WidthM));
                foreach ((MapSymbol symbol, MapSymbolStyle s, float width) in new[] { (MapSymbol.PathCasing, edge, core + 2.6f), (MapSymbol.Path, style, core) })
                {
                    var item = new MapDrawItem { Symbol = symbol, Layer = MapDrawLayer.Static, WidthPx = width, Colour = s.Colour, SourceIndex = i, World = path.Points[0] };
                    foreach (RackPoint p in path.Points) item.Points.Add(t.WorldToPixels(p));
                    item.Anchor = t.WorldToPixels(item.World);
                    plan.Add(item);
                }
            }

        // Rack centre lines, by state: dashed to plan, solid in the Work Plan, thin pale once harvested, dotted off the layout.
        for (int i = 0; i < model.Racks.Count; i++)
        {
            MapRack rack = model.Racks[i];
            if ((rack.MiniOnly && full) || (rack.FullOnly && !full) || StandMapModel.IsDynamicRack(rack.Symbol) || !StandMapSymbols.IsDrawn(rack.Symbol, detail)) continue;
            plan.Add(RackLine(t, rack, i, StandMapSymbols.Get(rack.Symbol), MapDrawLayer.Static));
        }

        // Planted juveniles at their exact places (full map).
        if (StandMapSymbols.IsDrawn(MapSymbol.Juvenile, detail))
        {
            MapSymbolStyle style = StandMapSymbols.Get(MapSymbol.Juvenile);
            for (int i = 0; i < model.Juveniles.Count; i++)
            {
                MapJuvenile j = model.Juveniles[i];
                var at = new RackPoint(j.X, j.Z);
                plan.Add(Disc(MapSymbol.Juvenile, MapDrawLayer.Static, t, at, Math.Max(2.6f, t.MetresToPixels(0.45f)), style.Colour, new MapColour(18, 28, 14, 220), 1.1f, i));
            }
        }

        // The Browsing layer: a ring at each shelter, blue while it still protects and grey once it has lapsed.
        if (full && overlay == MapOverlay.Browsing)
            for (int i = 0; i < model.Shelters.Count; i++)
            {
                MapShelter s = model.Shelters[i];
                MapSymbol symbol = s.Effective ? MapSymbol.Shelter : MapSymbol.ShelterLapsed;
                MapSymbolStyle style = StandMapSymbols.Get(symbol);
                plan.Add(Ring(symbol, MapDrawLayer.Static, t, new RackPoint(s.X, s.Z), Math.Max(5.5f, t.MetresToPixels(0.9f)), style.Colour, style.MinPx, i));
            }

        // The Marks layer: a ring on every marked tree, red to fell, blue for a Crop Tree. The ring is drawn at the crown's own size.
        if (full && overlay == MapOverlay.Marks)
            for (int i = 0; i < model.Trees.Count; i++)
            {
                MapTree tree = model.Trees[i];
                if (tree.Mark == MapTreeMark.None || tree.State != MapTreeState.Living) continue;
                MapSymbol symbol = tree.Mark == MapTreeMark.Fell ? MapSymbol.TreeMarkFell : MapSymbol.TreeMarkCrop;
                MapSymbolStyle style = StandMapSymbols.Get(symbol);
                plan.Add(Ring(symbol, MapDrawLayer.Static, t, new RackPoint(tree.X, tree.Z), Math.Max(6f, t.MetresToPixels(tree.CrownRadiusM) * 0.62f), style.Colour, style.MinPx, i));
            }

        // The property boundary: a dashed off-white line.
        {
            MapSymbolStyle style = StandMapSymbols.Get(MapSymbol.Property);
            var item = new MapDrawItem
            {
                Symbol = MapSymbol.Property, Layer = MapDrawLayer.Static, Closed = true, WidthPx = full ? style.MinPx : 1.6f, Colour = style.Colour,
                DashPx = full ? style.DashPx : 5f, GapPx = full ? style.GapPx : 4f, World = new RackPoint(model.MinX, model.MinZ)
            };
            item.Points.Add(t.WorldToPixels(new RackPoint(model.MinX, model.MinZ + model.SizeM)));
            item.Points.Add(t.WorldToPixels(new RackPoint(model.MinX + model.SizeM, model.MinZ + model.SizeM)));
            item.Points.Add(t.WorldToPixels(new RackPoint(model.MinX + model.SizeM, model.MinZ)));
            item.Points.Add(t.WorldToPixels(new RackPoint(model.MinX, model.MinZ)));
            item.Anchor = t.WorldToPixels(item.World);
            plan.Add(item);
        }

        // North arrow and scale bar.
        StandMapGridPlan.Furniture furniture = StandMapGridPlan.Layout(model.SizeM, sidePx, detail);
        if (StandMapSymbols.IsDrawn(MapSymbol.NorthArrow, detail))
        {
            MapSymbolStyle style = StandMapSymbols.Get(MapSymbol.NorthArrow);
            var arrow = new MapDrawItem { Symbol = MapSymbol.NorthArrow, Layer = MapDrawLayer.Static, Closed = true, Filled = true, Colour = style.Colour, Stroke = new MapColour(14, 18, 16, 230), StrokePx = 1f };
            float s = furniture.ArrowSize;
            arrow.Points.Add(new MapPosition(furniture.ArrowX, furniture.ArrowY - s));
            arrow.Points.Add(new MapPosition(furniture.ArrowX + s * 0.46f, furniture.ArrowY + s * 0.62f));
            arrow.Points.Add(new MapPosition(furniture.ArrowX, furniture.ArrowY + s * 0.24f));
            arrow.Points.Add(new MapPosition(furniture.ArrowX - s * 0.46f, furniture.ArrowY + s * 0.62f));
            arrow.Anchor = new MapPosition(furniture.ArrowX, furniture.ArrowY);
            plan.Add(arrow);
        }
        if (StandMapSymbols.IsDrawn(MapSymbol.ScaleBar, detail))
        {
            MapSymbolStyle style = StandMapSymbols.Get(MapSymbol.ScaleBar);
            var bar = new MapDrawItem { Symbol = MapSymbol.ScaleBar, Layer = MapDrawLayer.Static, WidthPx = 2f, Colour = style.Colour };
            bar.Points.Add(new MapPosition(furniture.BarX0, furniture.BarY - 6f));
            bar.Points.Add(new MapPosition(furniture.BarX0, furniture.BarY));
            bar.Points.Add(new MapPosition(furniture.BarX1, furniture.BarY));
            bar.Points.Add(new MapPosition(furniture.BarX1, furniture.BarY - 6f));
            bar.Anchor = new MapPosition(furniture.BarX0, furniture.BarY);
            plan.Add(bar);
        }
        return plan;
    }

    public static List<MapDrawItem> BuildDynamic(StandMapModel model, float sidePx, MapDetail detail)
    {
        var plan = new List<MapDrawItem>();
        if (model == null || !model.HasProperty || sidePx <= 0f) return plan;
        StandMapTransform t = model.TransformFor(sidePx);

        // The rack being planned and the edit ghost, last among the racks so they lie on top (full map only).
        for (int i = 0; i < model.Racks.Count; i++)
        {
            MapRack rack = model.Racks[i];
            if (!StandMapModel.IsDynamicRack(rack.Symbol) || !StandMapSymbols.IsDrawn(rack.Symbol, detail)) continue;
            MapSymbolStyle style = StandMapSymbols.Get(rack.Symbol);
            plan.Add(RackBand(model, t, rack, i, style, MapDrawLayer.Dynamic));
            plan.Add(RackLine(t, rack, i, style, MapDrawLayer.Dynamic));
        }

        // The selected player rack (full map only): a bright outline round its corridor, a square where it meets the headland, and
        // - unless its removals have been executed and its geometry is locked - a round handle at the far end to drag and rotate.
        for (int i = 0; i < model.Racks.Count; i++)
        {
            MapRack rack = model.Racks[i];
            if (!rack.Selected || !StandMapSymbols.IsDrawn(MapSymbol.RackSelection, detail)) continue;
            plan.Add(RackOutline(t, rack, i, StandMapSymbols.Get(MapSymbol.RackSelection)));
            MapSymbolStyle entrance = StandMapSymbols.Get(MapSymbol.RackEntrance);
            plan.Add(Square(t, rack.Start, EntranceHalfPx(sidePx), entrance.Colour, new MapColour(14, 24, 32, 245), 1.6f, i));
            if (rack.Locked) continue;
            MapSymbolStyle handle = StandMapSymbols.Get(MapSymbol.RackHandle);
            var disc = Disc(MapSymbol.RackHandle, MapDrawLayer.Dynamic, t, rack.End, HandleRadiusPx(sidePx), handle.Colour, new MapColour(18, 22, 20, 245), 2f, i);
            plan.Add(disc);
        }

        // The destination: a small amber diamond with a dark edge.
        if (model.HasWaypoint && StandMapSymbols.IsDrawn(MapSymbol.Waypoint, detail))
        {
            MapSymbolStyle style = StandMapSymbols.Get(MapSymbol.Waypoint);
            float r = WaypointRadiusPx(sidePx);
            MapPosition c = t.WorldToPixels(model.Waypoint);
            var item = new MapDrawItem { Symbol = MapSymbol.Waypoint, Layer = MapDrawLayer.Dynamic, Closed = true, Filled = true, Colour = style.Colour, Stroke = new MapColour(32, 22, 6, 235), StrokePx = 1.5f, World = model.Waypoint };
            item.Points.Add(new MapPosition(c.X, c.Y - r));
            item.Points.Add(new MapPosition(c.X + r, c.Y));
            item.Points.Add(new MapPosition(c.X, c.Y + r));
            item.Points.Add(new MapPosition(c.X - r, c.Y));
            item.Anchor = t.WorldToPixels(item.World);
            plan.Add(item);
        }

        // You: a facing wedge with its apex AT your position (so the heading is read from the apex to the middle of the arc), then a dot
        // exactly on that position.
        if (model.HasPlayer && StandMapSymbols.IsDrawn(MapSymbol.Player, detail))
        {
            MapSymbolStyle wedgeStyle = StandMapSymbols.Get(MapSymbol.Player), dotStyle = StandMapSymbols.Get(MapSymbol.PlayerDot);
            MapPosition c = t.WorldToPixels(model.Player);
            MapPosition d = StandMapTransform.HeadingToDirection(model.PlayerHeadingDegrees);
            float length = WedgeLengthPx(sidePx);
            var wedge = new MapDrawItem { Symbol = MapSymbol.Player, Layer = MapDrawLayer.Dynamic, Closed = true, Filled = true, Colour = wedgeStyle.Colour, Stroke = wedgeStyle.Colour.WithAlpha(210), StrokePx = 1f, World = model.Player };
            wedge.Points.Add(c);
            for (int k = 0; k <= WedgeArcPoints; k++)
            {
                double a = (-WedgeHalfAngleDegrees + 2.0 * WedgeHalfAngleDegrees * k / WedgeArcPoints) * Math.PI / 180.0;
                float cos = (float)Math.Cos(a), sin = (float)Math.Sin(a);
                wedge.Points.Add(new MapPosition(c.X + length * (d.X * cos - d.Y * sin), c.Y + length * (d.X * sin + d.Y * cos)));
            }
            wedge.Anchor = c;
            plan.Add(wedge);
            plan.Add(Disc(MapSymbol.PlayerDot, MapDrawLayer.Dynamic, t, model.Player, MarkerRadiusPx(sidePx), dotStyle.Colour, new MapColour(255, 255, 255, 245), 1.6f, -1));
        }
        return plan;
    }

    // The direction the player item points, in compass degrees, read back from the shape: from the apex (your position) to the middle of
    // the arc. Presentations and checks use this instead of trusting the model, so what is verified is what is drawn.
    public static float HeadingOfWedge(MapDrawItem wedge)
    {
        if (wedge == null || wedge.Points.Count < 3) return float.NaN;
        MapPosition apex = wedge.Points[0], mid = wedge.Points[1 + WedgeArcPoints / 2];
        return StandMapTransform.HeadingOf(mid.X - apex.X, -(mid.Y - apex.Y));
    }

    private static MapDrawItem Line(MapSymbol symbol, MapSymbolStyle style, float widthPx, MapPosition a, MapPosition b)
    {
        var item = new MapDrawItem { Symbol = symbol, Layer = MapDrawLayer.Static, WidthPx = widthPx, Colour = style.Colour, Anchor = a };
        item.Points.Add(a);
        item.Points.Add(b);
        return item;
    }

    private static MapDrawItem RackBand(StandMapModel model, StandMapTransform t, MapRack rack, int index, MapSymbolStyle rackStyle, MapDrawLayer layer)
    {
        float dx = rack.End.X - rack.Start.X, dz = rack.End.Z - rack.Start.Z, length = (float)Math.Sqrt(dx * dx + dz * dz);
        float half = rack.WidthM * 0.5f;
        // Never thinner than the band's own minimum on a small map: a rack must still be seen on the minimap.
        float minHalf = StandMapSymbols.Get(MapSymbol.RackBand).MinPx * 0.5f / Math.Max(0.0001f, t.PixelsPerMetre);
        if (half < minHalf) half = minHalf;
        float px = length > 1e-4f ? -dz / length * half : 0f, pz = length > 1e-4f ? dx / length * half : 0f;
        var item = new MapDrawItem { Symbol = MapSymbol.RackBand, Layer = layer, Closed = true, Filled = true, Colour = rackStyle.HasFill ? rackStyle.Fill : new MapColour(0, 0, 0, 0), SourceIndex = index, World = rack.Start };
        item.Points.Add(t.WorldToPixels(new RackPoint(rack.Start.X + px, rack.Start.Z + pz)));
        item.Points.Add(t.WorldToPixels(new RackPoint(rack.End.X + px, rack.End.Z + pz)));
        item.Points.Add(t.WorldToPixels(new RackPoint(rack.End.X - px, rack.End.Z - pz)));
        item.Points.Add(t.WorldToPixels(new RackPoint(rack.Start.X - px, rack.Start.Z - pz)));
        item.Anchor = t.WorldToPixels(item.World);
        return item;
    }

    // The outline of a rack's corridor (no fill), for the selected rack.
    private static MapDrawItem RackOutline(StandMapTransform t, MapRack rack, int index, MapSymbolStyle style)
    {
        float dx = rack.End.X - rack.Start.X, dz = rack.End.Z - rack.Start.Z, length = (float)Math.Sqrt(dx * dx + dz * dz);
        float half = Math.Max(rack.WidthM * 0.5f, style.MinPx * 0.5f / Math.Max(0.0001f, t.PixelsPerMetre));
        float px = length > 1e-4f ? -dz / length * half : 0f, pz = length > 1e-4f ? dx / length * half : 0f;
        var item = new MapDrawItem { Symbol = MapSymbol.RackSelection, Layer = MapDrawLayer.Dynamic, Closed = true, WidthPx = style.MinPx, Colour = style.Colour, SourceIndex = index, World = rack.Start };
        item.Points.Add(t.WorldToPixels(new RackPoint(rack.Start.X + px, rack.Start.Z + pz)));
        item.Points.Add(t.WorldToPixels(new RackPoint(rack.End.X + px, rack.End.Z + pz)));
        item.Points.Add(t.WorldToPixels(new RackPoint(rack.End.X - px, rack.End.Z - pz)));
        item.Points.Add(t.WorldToPixels(new RackPoint(rack.Start.X - px, rack.Start.Z - pz)));
        item.Anchor = t.WorldToPixels(item.World);
        return item;
    }

    private static MapDrawItem Square(StandMapTransform t, RackPoint at, float halfPx, MapColour fill, MapColour stroke, float strokePx, int index)
    {
        var item = new MapDrawItem { Symbol = MapSymbol.RackEntrance, Layer = MapDrawLayer.Dynamic, Closed = true, Filled = true, Colour = fill, Stroke = stroke, StrokePx = strokePx, SourceIndex = index, World = at };
        MapPosition c = t.WorldToPixels(at);
        item.Points.Add(new MapPosition(c.X - halfPx, c.Y - halfPx));
        item.Points.Add(new MapPosition(c.X + halfPx, c.Y - halfPx));
        item.Points.Add(new MapPosition(c.X + halfPx, c.Y + halfPx));
        item.Points.Add(new MapPosition(c.X - halfPx, c.Y + halfPx));
        item.Anchor = c;
        return item;
    }

    private static MapDrawItem RackLine(StandMapTransform t, MapRack rack, int index, MapSymbolStyle style, MapDrawLayer layer)
    {
        var item = new MapDrawItem
        {
            Symbol = rack.Symbol, Layer = layer, WidthPx = Math.Max(style.MinPx, t.MetresToPixels(style.WidthM)), Colour = style.Colour,
            DashPx = style.DashPx, GapPx = style.GapPx, SourceIndex = index, World = rack.Start
        };
        item.Points.Add(t.WorldToPixels(rack.Start));
        item.Points.Add(t.WorldToPixels(rack.End));
        item.Anchor = t.WorldToPixels(item.World);
        return item;
    }

    private static MapDrawItem Disc(MapSymbol symbol, MapDrawLayer layer, StandMapTransform t, RackPoint at, float radiusPx, MapColour fill, MapColour stroke, float strokePx, int index)
    {
        var item = new MapDrawItem { Symbol = symbol, Layer = layer, Closed = true, Filled = true, Colour = fill, Stroke = stroke, StrokePx = strokePx, SourceIndex = index, World = at };
        MapPosition c = t.WorldToPixels(at);
        for (int k = 0; k < CirclePoints; k++)
        {
            double a = 2.0 * Math.PI * k / CirclePoints;
            item.Points.Add(new MapPosition(c.X + radiusPx * (float)Math.Cos(a), c.Y + radiusPx * (float)Math.Sin(a)));
        }
        item.Anchor = c;
        return item;
    }

    private static MapDrawItem Ring(MapSymbol symbol, MapDrawLayer layer, StandMapTransform t, RackPoint at, float radiusPx, MapColour colour, float widthPx, int index)
    {
        var item = new MapDrawItem { Symbol = symbol, Layer = layer, Closed = true, WidthPx = widthPx, Colour = colour, SourceIndex = index, World = at };
        MapPosition c = t.WorldToPixels(at);
        for (int k = 0; k < CirclePoints; k++)
        {
            double a = 2.0 * Math.PI * k / CirclePoints;
            item.Points.Add(new MapPosition(c.X + radiusPx * (float)Math.Cos(a), c.Y + radiusPx * (float)Math.Sin(a)));
        }
        item.Anchor = c;
        return item;
    }
}
