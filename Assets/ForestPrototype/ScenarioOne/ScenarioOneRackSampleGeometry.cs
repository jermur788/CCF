using System;
using System.Collections.Generic;
using System.Globalization;

// Rack-served sample and coverage geometry for Multi-Scale Forestry Management (MFM-02, D-065). Plain numbers, no UnityEngine, so it can
// be checked on plain .NET (Tools/Verification/ScenarioOneNonUi) and gives the same answer everywhere. It only READS confirmed Rack
// Planning V2 rack records (RackPlanRack). It stores nothing and decides nothing: no save field, prescription, proposal or coverage
// policy lives here. Which rack is "the" teaching rack and where its strip ends are geometry, not forestry.
//
// Two different questions, answered separately on purpose:
//   * SAMPLE - where is the fixed teaching footprint that Lessons 2-5 are played in? It is built on the THIRD of the five confirmed
//     racks, counted across their common normal. Its width is a fixed ScenarioOneRackBay.TeachingHalfWidthM on each side of that rack
//     and never depends on the neighbouring racks.
//   * COVERAGE - which strip does each rack serve? Interior boundaries lie HALFWAY between neighbouring rack centrelines. Beyond the
//     outermost strips the two outer remainders are explicit data, never silently given to the nearest rack.
//
// Ground plane, metres: x east, z north. The common axis is the racks' shared direction as an angle in [0, 180) degrees (RackGeometry
// convention); the common normal is that axis turned 90 degrees to the LEFT. An "offset" is a signed distance along the normal from the
// world origin (axis east: normal north, offset = z). Offsets grow to the left, so the first ordered rack is the right-most.
//
// Flagged choices (spec section 4.2 leaves edge treatment to implementation): a unit's outer edge on a side with no neighbouring rack
// mirrors the half-gap on its other side, and a unit is one full-length strip per rack (no segmenting along the rack). Whether a unit
// is covered, not covered, excluded or manual is MFM-03's question, not answered here.
public static class ScenarioOneRackSampleGeometry
{
    // The accepted Scenario One plan is exactly five racks (ScenarioOneManager.RackPlanTargetCount); the teaching rack is the third.
    public const int TeachingRackCount = 5;
    public const int TeachingRackIndex = 2;
    // Offsets that agree to the nearest millimetre are tied, and a tie is broken by rack id.
    private const double OffsetResolutionPerM = 1000.0;
    private const double CrossingToleranceM = 1e-3;

    public enum Side { Right, Left }
    public enum PlacementKind { OutsideStand, RackCorridor, CoverageUnit, OuterRemainder }

    public readonly struct RackInput
    {
        public readonly int Id;
        public readonly RackPoint Entrance, Far;
        public readonly float WidthM;
        public RackInput(int id, RackPoint entrance, RackPoint far, float widthM)
        { Id = id; Entrance = entrance; Far = far; WidthM = widthM; }
    }

    // One confirmed rack in its place across the common normal.
    public sealed class OrderedRack
    {
        public readonly int Id;
        // 0 = right-most (lowest offset).
        public readonly int Index;
        public readonly RackPoint Entrance, Far;
        public readonly float WidthM, LengthM;
        // Offset of the rack's centreline midpoint along the common normal.
        public readonly float OffsetM;
        // How far the rack's own direction is from the common axis (0 for a perfectly parallel rack).
        public readonly float AxisDeviationDegrees;
        public OrderedRack(int id, int index, RackPoint entrance, RackPoint far, float widthM, float lengthM, float offsetM, float axisDeviationDegrees)
        {
            Id = id; Index = index; Entrance = entrance; Far = far; WidthM = widthM; LengthM = lengthM;
            OffsetM = offsetM; AxisDeviationDegrees = axisDeviationDegrees;
        }
    }

    // The strip one rack serves: from halfway to its right-hand neighbour to halfway to its left-hand neighbour.
    public sealed class CoverageUnit
    {
        public readonly int Index, RackId;
        public readonly float RackOffsetM, LowerOffsetM, UpperOffsetM;
        // True on the two outermost units' far edges, which have no neighbouring rack to halve the distance to.
        public readonly bool LowerIsOuterEdge, UpperIsOuterEdge;
        // The strip clipped to the stand square, and the part of it that is the rack's own corridor (excluded from treatment).
        public readonly float AreaM2, CorridorAreaM2;
        public readonly IReadOnlyList<RackPoint> Polygon;
        public float WidthM => UpperOffsetM - LowerOffsetM;
        public CoverageUnit(int index, int rackId, float rackOffsetM, float lowerOffsetM, float upperOffsetM, bool lowerIsOuterEdge,
            bool upperIsOuterEdge, float areaM2, float corridorAreaM2, IReadOnlyList<RackPoint> polygon)
        {
            Index = index; RackId = rackId; RackOffsetM = rackOffsetM; LowerOffsetM = lowerOffsetM; UpperOffsetM = upperOffsetM;
            LowerIsOuterEdge = lowerIsOuterEdge; UpperIsOuterEdge = upperIsOuterEdge;
            AreaM2 = areaM2; CorridorAreaM2 = corridorAreaM2; Polygon = polygon;
        }
    }

    // The stand beyond an outermost strip. It may be empty (the strip already reaches the stand edge) but it is always reported.
    public sealed class OuterRemainder
    {
        public readonly Side Side;
        // Offset where the outermost strip ends on this side, and the stand's own limit on that side (both signed offsets).
        public readonly float StripEdgeOffsetM, StandLimitOffsetM;
        public readonly float AreaM2;
        public readonly IReadOnlyList<RackPoint> Polygon;
        public bool IsEmpty => AreaM2 <= 1e-3f;
        public OuterRemainder(Side side, float stripEdgeOffsetM, float standLimitOffsetM, float areaM2, IReadOnlyList<RackPoint> polygon)
        { Side = side; StripEdgeOffsetM = stripEdgeOffsetM; StandLimitOffsetM = standLimitOffsetM; AreaM2 = areaM2; Polygon = polygon; }
    }

    public readonly struct Placement
    {
        public readonly PlacementKind Kind;
        // Index of the coverage unit (CoverageUnit) or -1.
        public readonly int UnitIndex;
        // The rack whose strip (CoverageUnit) or corridor (RackCorridor) holds the point, else 0.
        public readonly int RackId;
        public readonly Side Side;
        public Placement(PlacementKind kind, int unitIndex, int rackId, Side side)
        { Kind = kind; UnitIndex = unitIndex; RackId = rackId; Side = side; }
    }

    public sealed class Result
    {
        public bool Valid;
        public string Problem = "";
        public float StandHalfExtentM;
        public float AxisAngleDegrees, AxisX, AxisZ, NormalX, NormalZ;
        public float MaxAxisDeviationDegrees;
        // All racks in order across the common normal (right-most first), ties by rack id.
        public IReadOnlyList<OrderedRack> Racks = new OrderedRack[0];
        public OrderedRack TeachingRack;
        // Closest point on the teaching rack's centreline to the stand centre (0,0), moved along the rack if needed so the whole
        // teaching length stays on the segment. UnclampedAlongM is where the closest point lay, measured from the entrance.
        public RackPoint Anchor;
        public float AnchorAlongM, UnclampedAlongM;
        public bool AnchorClamped;
        // The fixed teaching footprint (ScenarioOneRackBay.Analyse* consumes this).
        public ScenarioOneRackBay.Frame Teaching;
        // Rack-served strips in the same order as Racks, then the two explicit outer remainders.
        public IReadOnlyList<CoverageUnit> Units = new CoverageUnit[0];
        public OuterRemainder RightRemainder, LeftRemainder;

        // Where one ground point falls. A rack corridor wins over the strip it lies in; outside the stand wins over everything.
        public Placement Classify(float x, float z)
        {
            if (!Valid || Math.Abs(x) > StandHalfExtentM || Math.Abs(z) > StandHalfExtentM)
                return new Placement(PlacementKind.OutsideStand, -1, 0, Side.Right);
            for (int i = 0; i < Racks.Count; i++)
            {
                OrderedRack rack = Racks[i];
                if (new ScenarioOneRackBay.Corridor(rack.Entrance, rack.Far, rack.WidthM * 0.5f).Contains(x, z))
                    return new Placement(PlacementKind.RackCorridor, i, rack.Id, Side.Right);
            }
            double offset = x * (double)NormalX + z * (double)NormalZ;
            if (offset < Units[0].LowerOffsetM) return new Placement(PlacementKind.OuterRemainder, -1, 0, Side.Right);
            if (offset > Units[Units.Count - 1].UpperOffsetM) return new Placement(PlacementKind.OuterRemainder, -1, 0, Side.Left);
            for (int i = 0; i < Units.Count; i++)
            {
                CoverageUnit unit = Units[i];
                if ((offset >= unit.LowerOffsetM && offset < unit.UpperOffsetM) || (i == Units.Count - 1 && offset <= unit.UpperOffsetM))
                    return new Placement(PlacementKind.CoverageUnit, i, unit.RackId, Side.Right);
            }
            return new Placement(PlacementKind.OutsideStand, -1, 0, Side.Right);
        }

        // Every derived number as text, so two derivations can be compared exactly (shuffled input, repeated runs).
        public string Signature()
        {
            if (!Valid) return "INVALID " + Problem;
            string F(double v) => v.ToString("R", CultureInfo.InvariantCulture);
            var parts = new List<string> { "axis=" + F(AxisAngleDegrees) };
            foreach (OrderedRack rack in Racks) parts.Add("rack" + rack.Id + "@" + F(rack.OffsetM));
            parts.Add("teach=" + TeachingRack.Id + " anchor=" + F(Anchor.X) + "," + F(Anchor.Z) + " along=" + F(AnchorAlongM));
            foreach (CoverageUnit unit in Units) parts.Add("unit" + unit.Index + "[" + F(unit.LowerOffsetM) + "," + F(unit.UpperOffsetM) + "]" + F(unit.AreaM2));
            parts.Add("rem[" + F(RightRemainder.StripEdgeOffsetM) + "," + F(RightRemainder.AreaM2) + "|" + F(LeftRemainder.StripEdgeOffsetM) + "," + F(LeftRemainder.AreaM2) + "]");
            return string.Join(" ", parts);
        }
    }

    // Convenience for the live game: the persisted rack snapshots (RackPlanRack) of a CONFIRMED plan. The caller decides whether the
    // plan is confirmed; an unconfirmed plan must never be passed here (the sample suspends rather than rebinding).
    public static Result Derive(IEnumerable<RackPlanRack> racks, float standHalfExtentM)
    {
        var inputs = new List<RackInput>();
        if (racks != null)
            foreach (RackPlanRack rack in racks)
                if (rack != null) inputs.Add(new RackInput(rack.Id, rack.Entrance, rack.Far, rack.WidthM));
        return Derive(inputs, standHalfExtentM);
    }

    // The stand is the square |x| and |z| at most standHalfExtentM about the world origin, which is also the stand centre.
    public static Result Derive(IReadOnlyList<RackInput> racks, float standHalfExtentM)
    {
        var result = new Result { StandHalfExtentM = standHalfExtentM };
        if (racks == null || racks.Count != TeachingRackCount)
            return Fail(result, "A rack-served sample needs exactly " + TeachingRackCount + " confirmed racks (found " + (racks == null ? 0 : racks.Count) + ").");
        if (!IsFinite(standHalfExtentM) || standHalfExtentM <= 0f)
            return Fail(result, "The stand size is not a positive finite number.");

        // Everything below is computed from the racks sorted by id, so the caller's order can never change an answer.
        var byId = new List<RackInput>(racks);
        byId.Sort((a, b) => a.Id.CompareTo(b.Id));
        for (int i = 0; i < byId.Count; i++)
        {
            RackInput r = byId[i];
            if (r.Id <= 0) return Fail(result, "Rack ids must be positive (found " + r.Id + ").");
            if (i > 0 && byId[i - 1].Id == r.Id) return Fail(result, "Rack id " + r.Id + " appears more than once.");
            if (!IsFinite(r.Entrance.X) || !IsFinite(r.Entrance.Z) || !IsFinite(r.Far.X) || !IsFinite(r.Far.Z))
                return Fail(result, "Rack " + r.Id + " has a non-finite endpoint.");
            if (!IsFinite(r.WidthM) || r.WidthM <= 0f) return Fail(result, "Rack " + r.Id + " has no corridor width.");
            if (RackGeometry.LengthM(r.Entrance, r.Far) < 1e-3f) return Fail(result, "Rack " + r.Id + " has no length.");
        }

        // The common axis is the length-weighted mean DIRECTION of the racks. Doubling every angle makes "east" and "west" the same
        // line, so the entrance/far orientation each rack happens to have cannot matter.
        double sumCos = 0, sumSin = 0, totalLength = 0;
        foreach (RackInput r in byId)
        {
            double dx = r.Far.X - r.Entrance.X, dz = r.Far.Z - r.Entrance.Z, length = Math.Sqrt(dx * dx + dz * dz);
            sumCos += (dx * dx - dz * dz) / length;
            sumSin += 2.0 * dx * dz / length;
            totalLength += length;
        }
        if (Math.Sqrt(sumCos * sumCos + sumSin * sumSin) < 1e-6 * totalLength)
            return Fail(result, "The racks do not share a direction, so there is no common normal to order them across.");
        double axisDegrees = 0.5 * Math.Atan2(sumSin, sumCos) * 180.0 / Math.PI;
        if (axisDegrees < 0.0) axisDegrees += 180.0;
        if (axisDegrees >= 180.0) axisDegrees -= 180.0;
        double axisRadians = axisDegrees * Math.PI / 180.0;
        double ax = Math.Cos(axisRadians), az = Math.Sin(axisRadians), nx = -az, nz = ax;
        result.AxisAngleDegrees = (float)axisDegrees;
        result.AxisX = (float)ax; result.AxisZ = (float)az; result.NormalX = (float)nx; result.NormalZ = (float)nz;

        // Order across the normal by the offset of each rack's centreline midpoint, ties (to the millimetre) by rack id.
        var measured = new List<Measured>(byId.Count);
        foreach (RackInput r in byId)
        {
            double offsetEntrance = r.Entrance.X * nx + r.Entrance.Z * nz, offsetFar = r.Far.X * nx + r.Far.Z * nz;
            double dx = r.Far.X - r.Entrance.X, dz = r.Far.Z - r.Entrance.Z, length = Math.Sqrt(dx * dx + dz * dz);
            double cosine = Math.Min(1.0, Math.Abs((dx * ax + dz * az) / length));
            measured.Add(new Measured
            {
                Input = r, Length = length, Offset = (offsetEntrance + offsetFar) * 0.5,
                Low = Math.Min(offsetEntrance, offsetFar), High = Math.Max(offsetEntrance, offsetFar),
                Deviation = Math.Acos(cosine) * 180.0 / Math.PI
            });
        }
        measured.Sort((a, b) =>
        {
            int c = Math.Round(a.Offset * OffsetResolutionPerM).CompareTo(Math.Round(b.Offset * OffsetResolutionPerM));
            return c != 0 ? c : a.Input.Id.CompareTo(b.Input.Id);
        });

        // Defensible ordering: no rack may reach past its neighbour across the normal, wherever along the stand you look. Collinear
        // racks touch exactly (a tie); crossing or interleaved racks are refused rather than ordered arbitrarily.
        for (int k = 0; k + 1 < measured.Count; k++)
            if (measured[k].High > measured[k + 1].Low + CrossingToleranceM)
                return Fail(result, "Racks " + measured[k].Input.Id + " and " + measured[k + 1].Input.Id
                    + " cross or overlap across the common normal, so they have no defensible order.");

        var ordered = new List<OrderedRack>(measured.Count);
        var corridors = new List<ScenarioOneRackBay.Corridor>(measured.Count);
        float maxDeviation = 0f;
        for (int k = 0; k < measured.Count; k++)
        {
            Measured m = measured[k];
            ordered.Add(new OrderedRack(m.Input.Id, k, m.Input.Entrance, m.Input.Far, m.Input.WidthM, (float)m.Length, (float)m.Offset, (float)m.Deviation));
            corridors.Add(new ScenarioOneRackBay.Corridor(m.Input.Entrance, m.Input.Far, m.Input.WidthM * 0.5f));
            maxDeviation = Math.Max(maxDeviation, (float)m.Deviation);
        }
        result.Racks = ordered; result.MaxAxisDeviationDegrees = maxDeviation;

        // ---- sample: third rack, closest-point anchor ----
        Measured teaching = measured[TeachingRackIndex];
        if (teaching.Length < 2.0 * ScenarioOneRackBay.HalfLengthM)
            return Fail(result, "The teaching rack " + teaching.Input.Id + " is shorter than the " + (2f * ScenarioOneRackBay.HalfLengthM) + " m teaching length.");
        result.TeachingRack = ordered[TeachingRackIndex];
        double ex = teaching.Input.Entrance.X, ez = teaching.Input.Entrance.Z;
        double ux = (teaching.Input.Far.X - ex) / teaching.Length, uz = (teaching.Input.Far.Z - ez) / teaching.Length;
        double along = (0.0 - ex) * ux + (0.0 - ez) * uz;
        double clamped = Math.Min(Math.Max(along, ScenarioOneRackBay.HalfLengthM), teaching.Length - ScenarioOneRackBay.HalfLengthM);
        result.UnclampedAlongM = (float)along; result.AnchorAlongM = (float)clamped; result.AnchorClamped = clamped != along;
        result.Anchor = new RackPoint((float)(ex + ux * clamped), (float)(ez + uz * clamped));
        result.Teaching = ScenarioOneRackBay.Frame.Anchored(result.Anchor, (float)ux, (float)uz, ScenarioOneRackBay.TeachingHalfWidthM, corridors);

        // ---- coverage: half-distance strips and explicit outer remainders ----
        double h = standHalfExtentM, standExtent = h * (Math.Abs(nx) + Math.Abs(nz));
        var offsets = new double[measured.Count];
        for (int k = 0; k < offsets.Length; k++) offsets[k] = measured[k].Offset;
        var lower = new double[offsets.Length]; var upper = new double[offsets.Length];
        for (int k = 0; k < offsets.Length; k++)
        {
            lower[k] = k == 0 ? offsets[0] - (offsets[1] - offsets[0]) * 0.5 : (offsets[k - 1] + offsets[k]) * 0.5;
            upper[k] = k == offsets.Length - 1 ? offsets[k] + (offsets[k] - offsets[k - 1]) * 0.5 : (offsets[k] + offsets[k + 1]) * 0.5;
        }
        var units = new List<CoverageUnit>(offsets.Length);
        for (int k = 0; k < offsets.Length; k++)
        {
            List<P> strip = Strip(lower[k], upper[k], ax, az, h);
            Measured m = measured[k];
            double corridorArea = Area(ToPoints(RackGeometry.CorridorPolygon(m.Input.Entrance, m.Input.Far, m.Input.WidthM, standHalfExtentM)));
            units.Add(new CoverageUnit(k, m.Input.Id, (float)offsets[k], (float)lower[k], (float)upper[k], k == 0, k == offsets.Length - 1,
                (float)Area(strip), (float)corridorArea, ToRackPoints(strip)));
        }
        result.Units = units;
        List<P> rightStrip = Strip(-standExtent - 1.0, lower[0], ax, az, h), leftStrip = Strip(upper[offsets.Length - 1], standExtent + 1.0, ax, az, h);
        result.RightRemainder = new OuterRemainder(Side.Right, (float)lower[0], (float)-standExtent, (float)Area(rightStrip), ToRackPoints(rightStrip));
        result.LeftRemainder = new OuterRemainder(Side.Left, (float)upper[offsets.Length - 1], (float)standExtent, (float)Area(leftStrip), ToRackPoints(leftStrip));
        result.Valid = true;
        return result;
    }

    private sealed class Measured
    {
        public RackInput Input;
        public double Length, Offset, Low, High, Deviation;
    }

    private static Result Fail(Result result, string problem)
    {
        result.Valid = false;
        result.Problem = problem;
        return result;
    }

    private static bool IsFinite(float value) => !float.IsNaN(value) && !float.IsInfinity(value);

    // ---- plain polygon helpers (doubles; the stand square is |x|,|z| <= h) ----
    private struct P
    {
        public double X, Z;
        public P(double x, double z) { X = x; Z = z; }
    }

    // The band between two offsets along the normal, cut to the stand square. Empty when the band lies outside it or has no width.
    private static List<P> Strip(double lowerOffset, double upperOffset, double ax, double az, double h)
    {
        if (upperOffset <= lowerOffset) return new List<P>();
        double reach = 3.0 * h, nx = -az, nz = ax;
        P At(double along, double offset) => new P(ax * along + nx * offset, az * along + nz * offset);
        var polygon = new List<P> { At(-reach, lowerOffset), At(reach, lowerOffset), At(reach, upperOffset), At(-reach, upperOffset) };
        polygon = Clip(polygon, 1.0, 0.0, h);
        polygon = Clip(polygon, -1.0, 0.0, h);
        polygon = Clip(polygon, 0.0, 1.0, h);
        polygon = Clip(polygon, 0.0, -1.0, h);
        return polygon.Count < 3 ? new List<P>() : polygon;
    }

    // Sutherland-Hodgman against one half-plane: keeps the points with nx*x + nz*z <= limit.
    private static List<P> Clip(List<P> input, double nx, double nz, double limit)
    {
        var output = new List<P>();
        for (int i = 0; i < input.Count; i++)
        {
            P current = input[i], previous = input[(i + input.Count - 1) % input.Count];
            double dc = nx * current.X + nz * current.Z - limit, dp = nx * previous.X + nz * previous.Z - limit;
            bool currentIn = dc <= 0.0, previousIn = dp <= 0.0;
            if (currentIn != previousIn)
            {
                double t = dp / (dp - dc);
                output.Add(new P(previous.X + (current.X - previous.X) * t, previous.Z + (current.Z - previous.Z) * t));
            }
            if (currentIn) output.Add(current);
        }
        return output;
    }

    private static double Area(List<P> polygon)
    {
        double twice = 0.0;
        for (int i = 0; i < polygon.Count; i++)
        {
            P a = polygon[i], b = polygon[(i + 1) % polygon.Count];
            twice += a.X * b.Z - b.X * a.Z;
        }
        return Math.Abs(twice) * 0.5;
    }

    private static List<P> ToPoints(List<RackPoint> points)
    {
        var polygon = new List<P>(points.Count);
        foreach (RackPoint p in points) polygon.Add(new P(p.X, p.Z));
        return polygon;
    }

    private static List<RackPoint> ToRackPoints(List<P> polygon)
    {
        var points = new List<RackPoint>(polygon.Count);
        foreach (P p in polygon) points.Add(new RackPoint((float)p.X, (float)p.Z));
        return points;
    }
}
