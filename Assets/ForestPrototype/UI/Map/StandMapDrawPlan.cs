using System;
using System.Collections.Generic;

// One thing to paint: a stroked line (open or closed) or a filled shape, already placed in pixels.
public sealed class MapDrawItem
{
    public MapSymbol Symbol;
    public List<MapPosition> Points = new List<MapPosition>();
    public bool Closed, Filled;
    public float WidthPx;
    public MapColour Colour;
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
public static class StandMapDrawPlan
{
    private const int CirclePoints = 24;
    // Quiet style, for a map that has text laid over it (the full map's analytical layers print a number in every square): the
    // same shapes in the same places, but lines thin and translucent and markers drawn as outlines, so no number is hidden.
    private const float QuietLinePx = 2f, QuietMarkerLinePx = 2.5f, QuietAlpha = 0.7f;

    // Markers (you, destination) scale with the map so they take the same share of it on a small and a large map.
    public static float MarkerRadiusPx(float sidePx) => Math.Max(6f, sidePx * 0.022f);

    public static List<MapDrawItem> Build(StandMapModel model, float sidePx, MapDetail detail, bool quiet = false)
    {
        var plan = new List<MapDrawItem>();
        if (model == null || !model.HasProperty || sidePx <= 0f) return plan;
        StandMapTransform t = model.TransformFor(sidePx);

        // Property boundary.
        {
            MapSymbolStyle style = StandMapSymbols.Get(MapSymbol.Property);
            var item = new MapDrawItem { Symbol = MapSymbol.Property, Closed = true, WidthPx = style.MinPx, Colour = style.Colour };
            item.World = new RackPoint(model.MinX, model.MinZ);
            item.Points.Add(t.WorldToPixels(new RackPoint(model.MinX, model.MinZ + model.SizeM)));
            item.Points.Add(t.WorldToPixels(new RackPoint(model.MinX + model.SizeM, model.MinZ + model.SizeM)));
            item.Points.Add(t.WorldToPixels(new RackPoint(model.MinX + model.SizeM, model.MinZ)));
            item.Points.Add(t.WorldToPixels(new RackPoint(model.MinX, model.MinZ)));
            item.Anchor = t.WorldToPixels(item.World);
            plan.Add(item);
        }

        if (StandMapSymbols.IsDrawn(MapSymbol.Path, detail))
            for (int i = 0; i < model.Paths.Count; i++)
            {
                MapPath path = model.Paths[i];
                if (path.Points.Count < 2) continue;
                MapSymbolStyle style = StandMapSymbols.Get(MapSymbol.Path);
                var item = new MapDrawItem { Symbol = MapSymbol.Path, WidthPx = Math.Max(style.MinPx, t.MetresToPixels(path.WidthM)), Colour = style.Colour, SourceIndex = i, World = path.Points[0] };
                foreach (RackPoint p in path.Points) item.Points.Add(t.WorldToPixels(p));
                item.Anchor = t.WorldToPixels(item.World);
                plan.Add(item);
            }

        if (StandMapSymbols.IsDrawn(MapSymbol.Landmark, detail))
            for (int i = 0; i < model.Landmarks.Count; i++)
            {
                MapLandmark landmark = model.Landmarks[i];
                MapSymbolStyle style = StandMapSymbols.Get(MapSymbol.Landmark);
                var item = new MapDrawItem { Symbol = MapSymbol.Landmark, Closed = true, WidthPx = style.MinPx, Colour = style.Colour, SourceIndex = i, World = landmark.At };
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

        // Racks: the preview last so it lies on top of the racks beneath it.
        for (int pass = 0; pass < 2; pass++)
            for (int i = 0; i < model.Racks.Count; i++)
            {
                MapRack rack = model.Racks[i];
                bool preview = rack.Symbol == MapSymbol.RackPreview;
                if (preview != (pass == 1) || !StandMapSymbols.IsDrawn(rack.Symbol, detail)) continue;
                MapSymbolStyle style = StandMapSymbols.Get(rack.Symbol);
                var item = new MapDrawItem { Symbol = rack.Symbol, WidthPx = Math.Max(style.MinPx, t.MetresToPixels(style.WidthM)), Colour = style.Colour, SourceIndex = i, World = rack.Start };
                item.Points.Add(t.WorldToPixels(rack.Start));
                item.Points.Add(t.WorldToPixels(rack.End));
                item.Anchor = t.WorldToPixels(item.World);
                plan.Add(item);
            }

        float marker = MarkerRadiusPx(sidePx);
        if (model.HasWaypoint && StandMapSymbols.IsDrawn(MapSymbol.Waypoint, detail))
        {
            MapSymbolStyle style = StandMapSymbols.Get(MapSymbol.Waypoint);
            MapPosition c = t.WorldToPixels(model.Waypoint);
            var item = new MapDrawItem { Symbol = MapSymbol.Waypoint, Closed = true, Filled = true, WidthPx = style.MinPx, Colour = style.Colour, World = model.Waypoint };
            item.Points.Add(new MapPosition(c.X, c.Y - marker));
            item.Points.Add(new MapPosition(c.X + marker, c.Y));
            item.Points.Add(new MapPosition(c.X, c.Y + marker));
            item.Points.Add(new MapPosition(c.X - marker, c.Y));
            item.Anchor = t.WorldToPixels(item.World);
            plan.Add(item);
        }

        if (model.HasPlayer && StandMapSymbols.IsDrawn(MapSymbol.Player, detail))
        {
            MapSymbolStyle style = StandMapSymbols.Get(MapSymbol.Player);
            MapPosition c = t.WorldToPixels(model.Player);
            MapPosition d = StandMapTransform.HeadingToDirection(model.PlayerHeadingDegrees);
            MapPosition side = new MapPosition(-d.Y, d.X);
            var item = new MapDrawItem { Symbol = MapSymbol.Player, Closed = true, Filled = true, WidthPx = style.MinPx, Colour = style.Colour, World = model.Player };
            // An arrow: the tip ahead of the player, a notch behind.
            item.Points.Add(new MapPosition(c.X + d.X * marker * 1.4f, c.Y + d.Y * marker * 1.4f));
            item.Points.Add(new MapPosition(c.X - d.X * marker + side.X * marker * 0.9f, c.Y - d.Y * marker + side.Y * marker * 0.9f));
            item.Points.Add(new MapPosition(c.X - d.X * marker * 0.45f, c.Y - d.Y * marker * 0.45f));
            item.Points.Add(new MapPosition(c.X - d.X * marker - side.X * marker * 0.9f, c.Y - d.Y * marker - side.Y * marker * 0.9f));
            item.Anchor = t.WorldToPixels(item.World);
            plan.Add(item);
        }
        if (quiet) foreach (MapDrawItem item in plan) MakeQuiet(item);
        return plan;
    }

    // Only the look changes: the points, and so the places, stay exactly those of the normal plan. The rack being planned
    // keeps its full strength (it is what the player is working on).
    private static void MakeQuiet(MapDrawItem item)
    {
        switch (item.Symbol)
        {
            case MapSymbol.Path:
            case MapSymbol.RackToDo:
            case MapSymbol.RackPlanned:
            case MapSymbol.RackOpen:
            case MapSymbol.RackOther:
                item.WidthPx = Math.Min(item.WidthPx, QuietLinePx);
                item.Colour = new MapColour(item.Colour.R, item.Colour.G, item.Colour.B, (byte)(item.Colour.A * QuietAlpha));
                break;
            case MapSymbol.Waypoint:
            case MapSymbol.Player:
                item.Filled = false;
                item.WidthPx = QuietMarkerLinePx;
                break;
        }
    }
}
