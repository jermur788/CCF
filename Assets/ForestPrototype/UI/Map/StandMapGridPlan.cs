using System;
using System.Collections.Generic;
using System.Globalization;

// The survey furniture of the map: the local property grid, its labels, the scale bar and the north arrow. Everything is derived from
// the property's own bounds, so nothing here is invented geography. The grid is LOCAL (metres east and north of the south-west corner
// of the property), not latitude and longitude: Scenario One has no real-world location.
//
// Plain numbers with no UnityEngine, like the rest of the shared map logic, so it can be checked offline.
public static class StandMapGridPlan
{
    public struct GridLabel
    {
        public string Text;
        // 0..1 along the axis it labels: from the west edge for an easting, from the SOUTH edge for a northing.
        public float Fraction;
        public bool Easting;
    }

    // Where the scale bar and the north arrow sit on a map of the given side, in pixels. One answer for the plan and for the label
    // the presentation puts beside the bar.
    public struct Furniture
    {
        public float BarX0, BarX1, BarY, BarMetres;
        public float ArrowX, ArrowY, ArrowSize;
    }

    // A tidy spacing (1, 2 or 5 times a power of ten) that gives about eight lines across the property: 10 m on 80 m, 5 m on 40 m.
    public static float StepM(float sizeM)
    {
        if (sizeM <= 0f) return 10f;
        float target = sizeM / 8f;
        float magnitude = (float)Math.Pow(10.0, Math.Floor(Math.Log10(target)));
        float best = magnitude;
        foreach (float f in new[] { 1f, 2f, 5f, 10f })
            if (Math.Abs(f * magnitude - target) < Math.Abs(best - target)) best = f * magnitude;
        return best;
    }

    // The interior grid lines: distances from the south-west corner, not including the property edges themselves.
    public static List<float> InteriorOffsets(float sizeM)
    {
        var offsets = new List<float>();
        float step = StepM(sizeM);
        for (float d = step; d < sizeM - 0.01f; d += step) offsets.Add(d);
        return offsets;
    }

    // Labels every second line (0, 20, 40, 60, 80 on the 80 m property).
    public static List<GridLabel> Labels(StandMapModel model)
    {
        var labels = new List<GridLabel>();
        if (model == null || !model.HasProperty || model.SizeM <= 0f) return labels;
        float every = StepM(model.SizeM) * 2f;
        for (float d = 0f; d <= model.SizeM + 0.01f; d += every)
        {
            string text = Math.Round(d).ToString(CultureInfo.InvariantCulture);
            labels.Add(new GridLabel { Text = text, Fraction = d / model.SizeM, Easting = true });
            labels.Add(new GridLabel { Text = text, Fraction = d / model.SizeM, Easting = false });
        }
        return labels;
    }

    // A round length for the scale bar, about a quarter of the property (20 m on 80 m).
    public static float ScaleBarMetres(float sizeM)
    {
        if (sizeM <= 0f) return 10f;
        float target = sizeM * 0.25f;
        float magnitude = (float)Math.Pow(10.0, Math.Floor(Math.Log10(target)));
        float best = magnitude;
        foreach (float f in new[] { 1f, 2f, 5f, 10f })
            if (Math.Abs(f * magnitude - target) < Math.Abs(best - target)) best = f * magnitude;
        return best;
    }

    public static string ScaleBarText(float metres) => Math.Round(metres).ToString(CultureInfo.InvariantCulture) + " m";

    public static Furniture Layout(float sizeM, float sidePx, MapDetail detail)
    {
        float inset = detail == MapDetail.Full ? 26f : 8f;
        float ppm = sizeM > 0f ? sidePx / sizeM : 0f;
        float metres = ScaleBarMetres(sizeM);
        var f = new Furniture { BarMetres = metres, BarX0 = inset, BarX1 = inset + metres * ppm, BarY = sidePx - inset };
        f.ArrowSize = detail == MapDetail.Full ? 14f : 7f;
        f.ArrowX = sidePx - inset - (detail == MapDetail.Full ? 4f : 2f);
        f.ArrowY = inset + f.ArrowSize + (detail == MapDetail.Full ? 14f : 6f);
        return f;
    }

    // Splits a polyline into dashes: pairs of end points along it, "dash" long with "gap" between, carrying the pattern across corners.
    // Pixels in, pixels out. dash or gap of zero (or less) returns the whole line as one piece.
    public static void Dashes(IList<MapPosition> points, float dash, float gap, List<MapPosition[]> pieces)
    {
        if (points.Count < 2) return;
        if (dash <= 0f || gap <= 0f)
        {
            var whole = new MapPosition[points.Count];
            for (int i = 0; i < points.Count; i++) whole[i] = points[i];
            pieces.Add(whole);
            return;
        }
        float period = dash + gap, phase = 0f;
        var current = new List<MapPosition>();
        for (int i = 1; i < points.Count; i++)
        {
            MapPosition a = points[i - 1], b = points[i];
            float dx = b.X - a.X, dy = b.Y - a.Y, length = (float)Math.Sqrt(dx * dx + dy * dy);
            if (length < 1e-4f) continue;
            float travelled = 0f;
            while (travelled < length - 1e-4f)
            {
                bool inDash = phase < dash;
                float remainingInState = inDash ? dash - phase : period - phase;
                float step = Math.Min(remainingInState, length - travelled);
                MapPosition from = new MapPosition(a.X + dx * travelled / length, a.Y + dy * travelled / length);
                MapPosition to = new MapPosition(a.X + dx * (travelled + step) / length, a.Y + dy * (travelled + step) / length);
                if (inDash)
                {
                    if (current.Count == 0) current.Add(from);
                    current.Add(to);
                }
                travelled += step;
                phase += step;
                if (phase >= period - 1e-5f) phase = 0f;
                if (inDash && phase >= dash - 1e-5f && current.Count > 0) { pieces.Add(current.ToArray()); current.Clear(); }
            }
        }
        if (current.Count > 1) pieces.Add(current.ToArray());
    }
}
