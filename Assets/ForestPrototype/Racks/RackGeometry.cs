using System;
using System.Collections.Generic;

// Extraction-rack geometry on the ground plane: plain numbers, no UnityEngine, so the selection can be checked offline on plain
// .NET (Tools/Verification/ScenarioOneRackUx/OfflineLogic) and gives the same answer everywhere. Positions are metres: x east,
// z north (the Unity X and Z axes). An angle of 0 degrees runs east, 90 degrees runs north.
//
// A rack is a straight corridor of trees to be felled so that machinery can drive along it now and in later thinnings. The
// geometry only says WHICH trees lie in a corridor; whether a tree can be felled, is a Crop Tree or is already planned is
// supplied by the caller. Nothing here is a forestry rule: the width and the choice of rows are management settings.
public readonly struct RackPoint
{
    public readonly float X, Z;
    public RackPoint(float x, float z) { X = x; Z = z; }
    public override string ToString() => "(" + X.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture) + ", " + Z.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture) + ")";
}

// One tree as the selector sees it.
public readonly struct RackTree
{
    public readonly string Id;
    public readonly float X, Z;
    // Living and fellable now. Anything else in the corridor is ignored (a stump or a dead stem is not part of a rack).
    public readonly bool CanBeFelled;
    // A Crop Tree is kept by the player's own choice, so a rack never takes one; it is counted so the player is told.
    public readonly bool IsCropTree;
    // Already in an open felling order: left out so nothing is planned twice, and counted.
    public readonly bool AlreadyPlanned;

    public RackTree(string id, float x, float z, bool canBeFelled = true, bool isCropTree = false, bool alreadyPlanned = false)
    {
        Id = id; X = x; Z = z; CanBeFelled = canBeFelled; IsCropTree = isCropTree; AlreadyPlanned = alreadyPlanned;
    }
}

// The trees a corridor would take, in a fixed order, with the corridor itself.
public sealed class RackSelection
{
    public RackPoint Start, End;
    public float WidthM;
    public float LengthM;
    // Selected for felling: ordered along the line from Start, ties by tree id (ordinal), so the order never depends on the
    // order the caller listed the trees in.
    public readonly List<RackTree> Trees = new List<RackTree>();
    // Inside the corridor but left out.
    public int SkippedCropTrees, AlreadyPlanned;
}

public static class RackGeometry
{
    // [D] Scenario setting, not a forestry constant: one planted row (rows are 1.9 m apart in Scenario One, so a half width of
    // 1.2 m takes a row and none of its neighbours).
    public const float DefaultWidthM = 2.4f;
    public const float MinWidthM = 1f;
    public const float MaxWidthM = 8f;
    private const float Epsilon = 1e-4f;

    public static float NormaliseAngle(float degrees)
    {
        float a = degrees % 180f;
        return a < 0f ? a + 180f : a;
    }

    public static RackPoint Direction(float angleDegrees)
    {
        double r = angleDegrees * Math.PI / 180.0;
        return new RackPoint((float)Math.Cos(r), (float)Math.Sin(r));
    }

    // Signed distance of a point from the line through the origin at this angle (positive to the left of the direction).
    public static float Across(RackPoint p, float angleDegrees)
    {
        RackPoint d = Direction(angleDegrees);
        return -p.X * d.Z + p.Z * d.X;
    }

    public static float Along(RackPoint p, float angleDegrees)
    {
        RackPoint d = Direction(angleDegrees);
        return p.X * d.X + p.Z * d.Z;
    }

    // The line through a point at an angle, cut where it enters and leaves the square stand (|x| and |z| at most half the
    // stand size). A rack by default runs from boundary to boundary of the stand; false when the line misses the stand.
    public static bool ClipLineToSquare(RackPoint through, float angleDegrees, float halfExtentM, out RackPoint start, out RackPoint end)
    {
        RackPoint d = Direction(angleDegrees);
        float tMin = float.NegativeInfinity, tMax = float.PositiveInfinity;
        if (!Slab(through.X, d.X, halfExtentM, ref tMin, ref tMax) || !Slab(through.Z, d.Z, halfExtentM, ref tMin, ref tMax)
            || tMax - tMin < Epsilon)
        {
            start = end = through;
            return false;
        }
        start = new RackPoint(through.X + d.X * tMin, through.Z + d.Z * tMin);
        end = new RackPoint(through.X + d.X * tMax, through.Z + d.Z * tMax);
        return true;
    }

    private static bool Slab(float origin, float direction, float half, ref float tMin, ref float tMax)
    {
        if (Math.Abs(direction) < 1e-6f)
            return Math.Abs(origin) <= half;
        float t1 = (-half - origin) / direction, t2 = (half - origin) / direction;
        if (t1 > t2) { float swap = t1; t1 = t2; t2 = swap; }
        tMin = Math.Max(tMin, t1);
        tMax = Math.Min(tMax, t2);
        return tMax >= tMin;
    }

    // The trees inside the corridor from start to end, width wide. Bounded on purpose: start and end are explicit, so a later
    // scenario can use a rack that does not cross the whole stand.
    public static RackSelection Select(IReadOnlyList<RackTree> trees, RackPoint start, RackPoint end, float widthM)
    {
        float width = Math.Max(MinWidthM, Math.Min(MaxWidthM, widthM));
        float dx = end.X - start.X, dz = end.Z - start.Z;
        float length = (float)Math.Sqrt(dx * dx + dz * dz);
        var selection = new RackSelection { Start = start, End = end, WidthM = width, LengthM = length };
        if (length < Epsilon || trees == null)
            return selection;
        float ux = dx / length, uz = dz / length, half = width * 0.5f;
        var found = new List<KeyValuePair<float, RackTree>>();
        for (int i = 0; i < trees.Count; i++)
        {
            RackTree t = trees[i];
            if (!t.CanBeFelled)
                continue;
            float px = t.X - start.X, pz = t.Z - start.Z;
            float along = px * ux + pz * uz;
            float across = -px * uz + pz * ux;
            if (along < -Epsilon || along > length + Epsilon || Math.Abs(across) > half + Epsilon)
                continue;
            if (t.IsCropTree) { selection.SkippedCropTrees++; continue; }
            if (t.AlreadyPlanned) { selection.AlreadyPlanned++; continue; }
            found.Add(new KeyValuePair<float, RackTree>(along, t));
        }
        found.Sort((a, b) =>
        {
            int c = a.Key.CompareTo(b.Key);
            return c != 0 ? c : string.CompareOrdinal(a.Value.Id, b.Value.Id);
        });
        foreach (KeyValuePair<float, RackTree> pair in found)
            selection.Trees.Add(pair.Value);
        return selection;
    }

    // The corridor as a polygon for drawing, cut to the square stand so it never reaches past the stand boundary.
    public static List<RackPoint> CorridorPolygon(RackPoint start, RackPoint end, float widthM, float halfExtentM)
    {
        float dx = end.X - start.X, dz = end.Z - start.Z;
        float length = (float)Math.Sqrt(dx * dx + dz * dz);
        var polygon = new List<RackPoint>();
        if (length < Epsilon)
            return polygon;
        float nx = -dz / length * widthM * 0.5f, nz = dx / length * widthM * 0.5f;
        polygon.Add(new RackPoint(start.X + nx, start.Z + nz));
        polygon.Add(new RackPoint(end.X + nx, end.Z + nz));
        polygon.Add(new RackPoint(end.X - nx, end.Z - nz));
        polygon.Add(new RackPoint(start.X - nx, start.Z - nz));
        // Sutherland-Hodgman against the four sides of the stand square.
        polygon = Clip(polygon, p => p.X <= halfExtentM, (a, b) => Cut(a, b, true, halfExtentM));
        polygon = Clip(polygon, p => p.X >= -halfExtentM, (a, b) => Cut(a, b, true, -halfExtentM));
        polygon = Clip(polygon, p => p.Z <= halfExtentM, (a, b) => Cut(a, b, false, halfExtentM));
        polygon = Clip(polygon, p => p.Z >= -halfExtentM, (a, b) => Cut(a, b, false, -halfExtentM));
        return polygon;
    }

    private static RackPoint Cut(RackPoint a, RackPoint b, bool onX, float value)
    {
        float da = onX ? a.X : a.Z, db = onX ? b.X : b.Z;
        float t = Math.Abs(db - da) < 1e-9f ? 0f : (value - da) / (db - da);
        return new RackPoint(a.X + (b.X - a.X) * t, a.Z + (b.Z - a.Z) * t);
    }

    private static List<RackPoint> Clip(List<RackPoint> input, Func<RackPoint, bool> inside, Func<RackPoint, RackPoint, RackPoint> cut)
    {
        var output = new List<RackPoint>();
        for (int i = 0; i < input.Count; i++)
        {
            RackPoint current = input[i], previous = input[(i + input.Count - 1) % input.Count];
            bool currentIn = inside(current), previousIn = inside(previous);
            if (currentIn)
            {
                if (!previousIn) output.Add(cut(previous, current));
                output.Add(current);
            }
            else if (previousIn)
                output.Add(cut(previous, current));
        }
        return output;
    }
}

// One planted row as the points show it: the trees that line up across the same perpendicular coordinate.
public sealed class RackRow
{
    // Perpendicular coordinate of the row's centre line (see RackGeometry.Across), the mean of its trees.
    public float Centre;
    public int Count;
    // Where along the row its first and last tree stand.
    public float MinAlong, MaxAlong;
}

// Finding the plantation rows from the tree positions alone, so a rack can follow the rows of any plantation without the
// generator's lattice numbers being repeated here.
public static class RackRows
{
    // Rows are separated by more than this between the nearest trees of neighbouring rows' centre bands (planting jitter in
    // Scenario One is under a metre against a 1.9 m row spacing).
    public const float DefaultGapM = 0.5f;

    // The angle (degrees, [0, 180)) along which the points line up best, found in 5 degree steps. A square planting has two
    // such angles 90 degrees apart and either may come first (the lower one only on an exact tie); the caller chooses between
    // the angle and its right angle, for example by the direction the player is facing.
    public static float DominantAngle(IReadOnlyList<RackPoint> points)
    {
        float best = 0f; long bestScore = -1;
        for (int step = 0; step < 36; step++)
        {
            float angle = step * 5f;
            var bins = new Dictionary<int, int>();
            for (int i = 0; i < points.Count; i++)
            {
                int bin = (int)Math.Floor(RackGeometry.Across(points[i], angle) / 0.2f);
                bins.TryGetValue(bin, out int n);
                bins[bin] = n + 1;
            }
            long score = 0;
            foreach (int n in bins.Values) score += (long)n * n;
            if (score > bestScore) { bestScore = score; best = angle; }
        }
        return best;
    }

    // The rows when the points are read across this angle, ordered by their coordinate.
    public static List<RackRow> Find(IReadOnlyList<RackPoint> points, float angleDegrees, float gapM = DefaultGapM)
    {
        var order = new List<KeyValuePair<float, float>>(points.Count);
        for (int i = 0; i < points.Count; i++)
            order.Add(new KeyValuePair<float, float>(RackGeometry.Across(points[i], angleDegrees), RackGeometry.Along(points[i], angleDegrees)));
        order.Sort((a, b) => a.Key.CompareTo(b.Key));
        var rows = new List<RackRow>();
        RackRow row = null; double sum = 0;
        for (int i = 0; i < order.Count; i++)
        {
            if (row == null || order[i].Key - order[i - 1].Key > gapM)
            {
                if (row != null) row.Centre = (float)(sum / row.Count);
                row = new RackRow { MinAlong = order[i].Value, MaxAlong = order[i].Value };
                rows.Add(row); sum = 0;
            }
            row.Count++; sum += order[i].Key;
            row.MinAlong = Math.Min(row.MinAlong, order[i].Value);
            row.MaxAlong = Math.Max(row.MaxAlong, order[i].Value);
        }
        if (row != null) row.Centre = (float)(sum / row.Count);
        return rows;
    }

    // Index of the row whose centre is nearest this coordinate, or -1 when there are no rows.
    public static int Nearest(IReadOnlyList<RackRow> rows, float across)
    {
        int best = -1; float distance = float.MaxValue;
        for (int i = 0; i < rows.Count; i++)
        {
            float d = Math.Abs(rows[i].Centre - across);
            if (d < distance) { distance = d; best = i; }
        }
        return best;
    }
}

// An even rack layout over a number of rows: every (rowsBetween + 1)th row, centred in the block. Evenly spaced, not a rule:
// the number of rows left standing between racks is a scenario choice made by the caller.
public static class RackLayout
{
    public static List<int> RowIndices(int rowCount, int rowsBetween)
    {
        var rows = new List<int>();
        int step = Math.Max(1, rowsBetween + 1);
        int count = rowCount / step;
        if (count < 1)
            return rows;
        int first = (rowCount - 1 - (count - 1) * step) / 2;
        for (int i = 0; i < count; i++) rows.Add(first + i * step);
        return rows;
    }
}
