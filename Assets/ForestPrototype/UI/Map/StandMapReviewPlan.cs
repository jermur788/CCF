using System;
using System.Collections.Generic;

// The shapes of the Stand Map "Treatment review" layer, already placed in map pixels. Plain data, no UnityEngine, so the form of each
// state is checked offline. The layer is its own element above the shared map canvas: it adds no symbol to the shared model, the
// shared draw plan or the HUD minimap.
//
// Meaning is carried by FORM first and colour second:
//   area outline + hatching   Sample: heavy outline, 45° crosshatch. Covered: solid outline, one 45° hatch. Not covered: dashed outline,
//                             no hatch. Excluded: solid outline, square grid hatch. Manual: dotted outline, no hatch.
//   candidate glyph           a hollow SQUARE (the Crop Tree / Fell grammar on the map is a ring, and the world marks are paint bands);
//                             rejected adds a diagonal strike, confirmed a second inner square, the one being looked at an outer diamond.
//   proposal glyph            a hollow TRIANGLE; a held conflict adds a bar across it.
// Every glyph and every area also has words on the card, and areas carry a printed tag on the map.
public readonly struct ReviewPoint
{
    public readonly float X, Y;
    public ReviewPoint(float x, float y) { X = x; Y = y; }
}

public enum ReviewShapeKind { AreaOutline, AreaHatch, CandidateGlyph, ProposalGlyph }

public sealed class ReviewShape
{
    public ReviewShapeKind Kind;
    public string Key = "";
    public readonly List<ReviewPoint> Points = new List<ReviewPoint>();
    public bool Closed;
    public float WidthPx, DashPx, GapPx;
    public byte R, G, B, A;
}

// One area's footprint, in map pixels, with the words that go beside it.
public sealed class ReviewFootprint
{
    public string AreaId = "", Tag = "";
    public ScenarioOneCoverage.State State;
    public bool IsSource, Selected;
    public readonly List<ReviewPoint> Polygon = new List<ReviewPoint>();
}

public static class StandMapReviewPlan
{
    public readonly struct Pattern
    {
        public readonly float OutlinePx, DashPx, GapPx, HatchSpacingPx, HatchWidthPx;
        public readonly float[] HatchAnglesDegrees;
        public readonly byte R, G, B;
        public Pattern(float outline, float dash, float gap, float spacing, float hatchWidth, float[] angles, byte r, byte g, byte b)
        { OutlinePx = outline; DashPx = dash; GapPx = gap; HatchSpacingPx = spacing; HatchWidthPx = hatchWidth; HatchAnglesDegrees = angles ?? new float[0]; R = r; G = g; B = b; }
        public string Signature => OutlinePx + "/" + DashPx + "/" + GapPx + "/" + HatchSpacingPx + "/" + string.Join(",", HatchAnglesDegrees);
    }

    // Colours are bounded presentation choices; the rows above differ in line pattern and hatching as well, so colour never decides alone.
    public static Pattern PatternFor(bool isSource, ScenarioOneCoverage.State state)
    {
        if (isSource) return new Pattern(3.5f, 0, 0, 6f, 1.2f, new[] { 45f, 135f }, 255, 226, 120);
        switch (state)
        {
            case ScenarioOneCoverage.State.Covered: return new Pattern(2.2f, 0, 0, 13f, 1.2f, new[] { 45f }, 86, 204, 170);
            case ScenarioOneCoverage.State.NotCovered: return new Pattern(2.2f, 9f, 5f, 0, 0, null, 190, 190, 190);
            case ScenarioOneCoverage.State.Excluded: return new Pattern(2.2f, 0, 0, 10f, 1.1f, new[] { 0f, 90f }, 224, 108, 108);
            default: return new Pattern(2.2f, 2f, 4f, 0, 0, null, 150, 178, 236);
        }
    }

    public static List<ReviewShape> Areas(IReadOnlyList<ReviewFootprint> footprints)
    {
        var shapes = new List<ReviewShape>();
        if (footprints == null) return shapes;
        foreach (ReviewFootprint fp in footprints)
        {
            if (fp.Polygon.Count < 3) continue;
            Pattern p = PatternFor(fp.IsSource, fp.State);
            if (p.HatchSpacingPx > 0f && IsConvex(fp.Polygon))
                foreach (float angle in p.HatchAnglesDegrees)
                    foreach (var segment in Hatch(fp.Polygon, angle, p.HatchSpacingPx))
                    {
                        var h = new ReviewShape { Kind = ReviewShapeKind.AreaHatch, Key = fp.AreaId, WidthPx = p.HatchWidthPx, R = p.R, G = p.G, B = p.B, A = 150 };
                        h.Points.Add(segment.Item1);
                        h.Points.Add(segment.Item2);
                        shapes.Add(h);
                    }
            var outline = new ReviewShape
            {
                Kind = ReviewShapeKind.AreaOutline, Key = fp.AreaId, Closed = true, WidthPx = p.OutlinePx + (fp.Selected ? 2f : 0f),
                DashPx = p.DashPx, GapPx = p.GapPx, R = p.R, G = p.G, B = p.B, A = 235
            };
            outline.Points.AddRange(fp.Polygon);
            shapes.Add(outline);
        }
        return shapes;
    }

    public const float GlyphHalfPx = 5.5f;

    public static List<ReviewShape> CandidateGlyph(string key, float x, float y, ScenarioOneCropTreeCandidates.Decision decision, bool current)
    {
        var shapes = new List<ReviewShape>();
        // Violet, hollow, square: the candidate grammar. Dashed while still open (nothing is decided).
        byte r = 178, g = 140, b = 255;
        float h = GlyphHalfPx;
        ReviewShape square = Poly(ReviewShapeKind.CandidateGlyph, key, true, 1.8f, decision == ScenarioOneCropTreeCandidates.Decision.Open ? 3f : 0f,
            decision == ScenarioOneCropTreeCandidates.Decision.Open ? 2f : 0f, r, g, b,
            new ReviewPoint(x - h, y - h), new ReviewPoint(x + h, y - h), new ReviewPoint(x + h, y + h), new ReviewPoint(x - h, y + h));
        shapes.Add(square);
        if (decision == ScenarioOneCropTreeCandidates.Decision.Rejected)
            shapes.Add(Poly(ReviewShapeKind.CandidateGlyph, key, false, 1.8f, 0, 0, r, g, b, new ReviewPoint(x - h, y + h), new ReviewPoint(x + h, y - h)));
        else if (decision == ScenarioOneCropTreeCandidates.Decision.Confirmed)
        {
            float i = h * 0.45f;
            shapes.Add(Poly(ReviewShapeKind.CandidateGlyph, key, true, 1.6f, 0, 0, r, g, b,
                new ReviewPoint(x - i, y - i), new ReviewPoint(x + i, y - i), new ReviewPoint(x + i, y + i), new ReviewPoint(x - i, y + i)));
        }
        if (current)
        {
            float d = h * 1.9f;
            shapes.Add(Poly(ReviewShapeKind.CandidateGlyph, key, true, 2.2f, 0, 0, 255, 236, 200,
                new ReviewPoint(x, y - d), new ReviewPoint(x + d, y), new ReviewPoint(x, y + d), new ReviewPoint(x - d, y)));
        }
        return shapes;
    }

    public static List<ReviewShape> ProposalGlyph(string key, float x, float y, bool conflict)
    {
        var shapes = new List<ReviewShape>();
        // Pink, hollow, triangle: the proposal grammar. A conflict is the same triangle with a bar through it.
        byte r = 244, g = 132, b = 204;
        float h = GlyphHalfPx + 1f;
        shapes.Add(Poly(ReviewShapeKind.ProposalGlyph, key, true, 1.8f, 0, 0, r, g, b,
            new ReviewPoint(x, y - h), new ReviewPoint(x + h, y + h * 0.8f), new ReviewPoint(x - h, y + h * 0.8f)));
        if (conflict)
            shapes.Add(Poly(ReviewShapeKind.ProposalGlyph, key, false, 1.8f, 0, 0, r, g, b, new ReviewPoint(x - h, y + h * 0.2f), new ReviewPoint(x + h, y + h * 0.2f)));
        return shapes;
    }

    private static ReviewShape Poly(ReviewShapeKind kind, string key, bool closed, float width, float dash, float gap, byte r, byte g, byte b, params ReviewPoint[] points)
    {
        var shape = new ReviewShape { Kind = kind, Key = key, Closed = closed, WidthPx = width, DashPx = dash, GapPx = gap, R = r, G = g, B = b, A = 240 };
        shape.Points.AddRange(points);
        return shape;
    }

    // ------------------------------------------------------------------------------------------------------------- geometry

    public static ReviewPoint Centroid(IReadOnlyList<ReviewPoint> polygon)
    {
        float x = 0, y = 0;
        foreach (ReviewPoint p in polygon) { x += p.X; y += p.Y; }
        int n = Math.Max(1, polygon.Count);
        return new ReviewPoint(x / n, y / n);
    }

    public static bool IsConvex(IReadOnlyList<ReviewPoint> polygon)
    {
        int n = polygon.Count, sign = 0;
        if (n < 3) return false;
        for (int i = 0; i < n; i++)
        {
            ReviewPoint a = polygon[i], b = polygon[(i + 1) % n], c = polygon[(i + 2) % n];
            double cross = (double)(b.X - a.X) * (c.Y - b.Y) - (double)(b.Y - a.Y) * (c.X - b.X);
            int s = cross > 1e-6 ? 1 : cross < -1e-6 ? -1 : 0;
            if (s == 0) continue;
            if (sign == 0) sign = s; else if (s != sign) return false;
        }
        return sign != 0;
    }

    // Parallel lines at angle (degrees) every spacing pixels, clipped to a CONVEX polygon (Cyrus–Beck). Deterministic.
    public static List<Tuple<ReviewPoint, ReviewPoint>> Hatch(IReadOnlyList<ReviewPoint> polygon, float angleDegrees, float spacingPx)
    {
        var lines = new List<Tuple<ReviewPoint, ReviewPoint>>();
        if (polygon.Count < 3 || spacingPx < 1f) return lines;
        double a = angleDegrees * Math.PI / 180.0, dx = Math.Cos(a), dy = Math.Sin(a), nx = -dy, ny = dx;
        double lo = double.MaxValue, hi = double.MinValue, reach = 0;
        foreach (ReviewPoint p in polygon)
        {
            double d = p.X * nx + p.Y * ny;
            lo = Math.Min(lo, d); hi = Math.Max(hi, d);
            reach = Math.Max(reach, Math.Abs(p.X) + Math.Abs(p.Y));
        }
        double area2 = 0;
        for (int i = 0; i < polygon.Count; i++)
        {
            ReviewPoint p = polygon[i], q = polygon[(i + 1) % polygon.Count];
            area2 += (double)p.X * q.Y - (double)q.X * p.Y;
        }
        double orient = area2 >= 0 ? 1 : -1;
        for (double offset = Math.Ceiling(lo / spacingPx) * spacingPx; offset < hi; offset += spacingPx)
        {
            double bx = nx * offset, by = ny * offset, far = reach * 2 + 10;
            double t0 = -far, t1 = far;
            bool ok = true;
            for (int i = 0; i < polygon.Count && ok; i++)
            {
                ReviewPoint p = polygon[i], q = polygon[(i + 1) % polygon.Count];
                // Inward normal of this edge for the polygon's winding.
                double ex = q.X - p.X, ey = q.Y - p.Y, inX = -ey * orient, inY = ex * orient;
                double denom = dx * inX + dy * inY, num = (bx - p.X) * inX + (by - p.Y) * inY;
                if (Math.Abs(denom) < 1e-9) { if (num < 0) ok = false; continue; }
                double t = -num / denom;
                if (denom > 0) t0 = Math.Max(t0, t); else t1 = Math.Min(t1, t);
            }
            if (!ok || t1 - t0 < 1.0) continue;
            lines.Add(Tuple.Create(new ReviewPoint((float)(bx + dx * t0), (float)(by + dy * t0)), new ReviewPoint((float)(bx + dx * t1), (float)(by + dy * t1))));
        }
        return lines;
    }
}
