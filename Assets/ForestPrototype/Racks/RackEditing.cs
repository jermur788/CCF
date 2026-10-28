using System;
using System.Collections.Generic;

// Rack editing rules for the Stand Map (RACK-V2-05): where a rack entrance may sit, which geometry is refused, and how the
// pointer's position is softly nudged. Plain numbers with no UnityEngine, like RackGeometry, so the rules can be checked offline
// on plain .NET (Tools/Verification/RackV2Editing/OfflineCheck.cs). Positions are metres: x east, z north.
//
// Nothing here changes a rack. The authoritative plan is ScenarioOneManager.RackPlan (RackSaveData records); these functions only
// answer "is this geometry acceptable?" and "where would a snap put the far end?". Snapping is ADVISORY: it moves a point a short
// way toward a tidy value, never forces a rail, and a geometry that fails Validate is refused rather than silently repaired.
//
// The thresholds below are Scenario One gameplay calibration for the editing feel (D-064), not forestry rules.

// Where racks may begin and how large the stand is. The caller reads the headland from ScenarioOneHeadlandGeometry and the half
// side from StandGeometryModel; this file keeps no copy of either.
public readonly struct RackEditBounds
{
    public readonly float HeadlandX, SouthZ, NorthZ, HalfExtentM;

    public RackEditBounds(float headlandX, float southZ, float northZ, float halfExtentM)
    {
        HeadlandX = headlandX;
        SouthZ = southZ;
        NorthZ = northZ;
        HalfExtentM = halfExtentM;
    }

    public bool IsValid => HalfExtentM > 0f && NorthZ > SouthZ;
}

// Another rack as the editing rules see it: just an id and a centreline.
public readonly struct RackEditOther
{
    public readonly int Id;
    public readonly RackPoint Entrance, Far;

    public RackEditOther(int id, RackPoint entrance, RackPoint far)
    {
        Id = id;
        Entrance = entrance;
        Far = far;
    }
}

public enum RackEditProblem
{
    None,
    NonFinite,
    TooShort,
    OutsideStand,
    EntranceOffHeadland,
    Duplicate,
    Overlap
}

public readonly struct RackEditValidation
{
    public readonly RackEditProblem Problem;
    // The rack this one duplicates or overlaps, or 0.
    public readonly int OtherRackId;
    public readonly string Message;

    public RackEditValidation(RackEditProblem problem, string message = "", int otherRackId = 0)
    {
        Problem = problem;
        Message = message ?? "";
        OtherRackId = otherRackId;
    }

    public bool IsValid => Problem == RackEditProblem.None;
    public static RackEditValidation Ok => new RackEditValidation(RackEditProblem.None);
}

// What a soft snap did to the far end (for the pointer feedback), and where the far end now is.
public readonly struct RackSnapResult
{
    public readonly RackPoint Far;
    public readonly bool AngleSnapped, BoundarySnapped;
    // The rack the angle was made parallel to, or 0 when it followed the suggested orientation (or nothing snapped).
    public readonly int ParallelToRackId;

    public RackSnapResult(RackPoint far, bool angleSnapped, int parallelToRackId, bool boundarySnapped)
    {
        Far = far;
        AngleSnapped = angleSnapped;
        ParallelToRackId = parallelToRackId;
        BoundarySnapped = boundarySnapped;
    }

    public bool Snapped => AngleSnapped || BoundarySnapped;
}

public static class RackEditing
{
    // A rack shorter than this is not a corridor a machine can use. [D] gameplay calibration for the editing guard.
    public const float MinLengthM = 5f;
    // The entrance must sit on the headland line to this tolerance.
    public const float EntranceToleranceM = 0.01f;
    // Both ends within this of another rack's ends = the same rack drawn twice.
    public const float DuplicateToleranceM = 0.5f;
    // Corridors that come closer than their own width overlap materially. Equal to the rack corridor width.
    public const float OverlapClearanceM = RackGeometry.DefaultWidthM;
    // Soft-snap windows. Inside them the angle (or length) is pulled to the tidy value; outside them the player is free.
    public const float AngleSnapDegrees = 4f;
    public const float BoundarySnapM = 2.5f;
    // A rack counts as "nearby" for parallel snapping when its entrance is this close along the headland.
    public const float ParallelSearchM = 30f;
    // Reset-to-suggested treats a layout as unedited when every rack is within this of a suggested route.
    public const float SuggestedMatchToleranceM = 0.01f;

    private const float Epsilon = 1e-4f;

    public static bool IsFinite(RackPoint p) => !float.IsNaN(p.X) && !float.IsNaN(p.Z) && !float.IsInfinity(p.X) && !float.IsInfinity(p.Z);

    // The entrance for a point near the headland: on the headland line, and no further along it than the strip runs. Moving a rack
    // or starting one re-snaps its entrance this way; nothing else about the rack is adjusted.
    public static RackPoint SnapEntrance(RackEditBounds bounds, RackPoint near)
    {
        float z = Math.Max(bounds.SouthZ, Math.Min(bounds.NorthZ, near.Z));
        return new RackPoint(bounds.HeadlandX, z);
    }

    public static bool EntranceOnHeadland(RackEditBounds bounds, RackPoint entrance)
        => Math.Abs(entrance.X - bounds.HeadlandX) <= EntranceToleranceM
            && entrance.Z >= bounds.SouthZ - EntranceToleranceM && entrance.Z <= bounds.NorthZ + EntranceToleranceM;

    // A rack drawn by dragging between two points: the end nearer the headland line is the entrance (snapped onto the headland) and
    // the other end is the far end. Starting at the headland and dragging into the stand, or starting inside and dragging back to
    // the headland, both make the same rack.
    public static void ResolveCreation(RackEditBounds bounds, RackPoint pressed, RackPoint dragged, out RackPoint entrance, out RackPoint far)
    {
        bool pressedIsEntrance = Math.Abs(pressed.X - bounds.HeadlandX) <= Math.Abs(dragged.X - bounds.HeadlandX);
        entrance = SnapEntrance(bounds, pressedIsEntrance ? pressed : dragged);
        far = pressedIsEntrance ? dragged : pressed;
    }

    // Moving a rack: it slides along the headland. The entrance re-snaps onto the headland and the far end shifts by exactly the
    // same step, so the rack keeps its length and angle. Where it would run past the end of the strip the move stops at the end.
    public static void MoveAlongHeadland(RackEditBounds bounds, RackPoint entrance, RackPoint far, float desiredEntranceZ,
        out RackPoint newEntrance, out RackPoint newFar)
    {
        newEntrance = SnapEntrance(bounds, new RackPoint(bounds.HeadlandX, desiredEntranceZ));
        float dx = newEntrance.X - entrance.X, dz = newEntrance.Z - entrance.Z;
        newFar = new RackPoint(far.X + dx, far.Z + dz);
    }

    // The editing rules a rack must meet before it may enter the plan. Nothing is repaired: the first failing rule is reported.
    // `others` are the other racks in the plan (never the rack being edited).
    public static RackEditValidation Validate(RackEditBounds bounds, RackPoint entrance, RackPoint far, IReadOnlyList<RackEditOther> others)
    {
        const string prefix = "RACK INVALID — ADJUST PLAN: ";
        if (!bounds.IsValid)
            return new RackEditValidation(RackEditProblem.OutsideStand, prefix + "the stand is not known.");
        if (!IsFinite(entrance) || !IsFinite(far))
            return new RackEditValidation(RackEditProblem.NonFinite, prefix + "the rack has a non-finite endpoint.");
        if (RackGeometry.LengthM(entrance, far) < MinLengthM)
            return new RackEditValidation(RackEditProblem.TooShort,
                prefix + "the rack is shorter than " + MinLengthM.ToString("0", System.Globalization.CultureInfo.InvariantCulture) + " m.");
        float half = bounds.HalfExtentM + 1e-3f;
        if (Math.Abs(entrance.X) > half || Math.Abs(entrance.Z) > half || Math.Abs(far.X) > half || Math.Abs(far.Z) > half)
            return new RackEditValidation(RackEditProblem.OutsideStand, prefix + "the rack leaves the stand.");
        if (!EntranceOnHeadland(bounds, entrance))
            return new RackEditValidation(RackEditProblem.EntranceOffHeadland, prefix + "the rack entrance must be on the headland.");

        if (others != null)
        {
            for (int i = 0; i < others.Count; i++)
            {
                RackEditOther other = others[i];
                if (IsDuplicate(entrance, far, other.Entrance, other.Far))
                    return new RackEditValidation(RackEditProblem.Duplicate, prefix + "this rack duplicates rack " + other.Id + ".", other.Id);
            }
            for (int i = 0; i < others.Count; i++)
            {
                RackEditOther other = others[i];
                if (SegmentDistance(entrance, far, other.Entrance, other.Far) < OverlapClearanceM)
                    return new RackEditValidation(RackEditProblem.Overlap, prefix + "this rack overlaps rack " + other.Id + ".", other.Id);
            }
        }
        return RackEditValidation.Ok;
    }

    public static bool IsDuplicate(RackPoint entranceA, RackPoint farA, RackPoint entranceB, RackPoint farB)
        => (RackGeometry.LengthM(entranceA, entranceB) <= DuplicateToleranceM && RackGeometry.LengthM(farA, farB) <= DuplicateToleranceM)
            || (RackGeometry.LengthM(entranceA, farB) <= DuplicateToleranceM && RackGeometry.LengthM(farA, entranceB) <= DuplicateToleranceM);

    // Centre-line distance to the nearest player rack. The caller supplies only authoritative plan racks,
    // excluding the rack being edited. Ties use the stable rack ID, independent of enumeration order.
    public static bool NearestRack(RackPoint entrance, RackPoint far, IReadOnlyList<RackEditOther> others,
        out int rackId, out float distanceM)
    {
        rackId = 0;
        distanceM = float.PositiveInfinity;
        if (others == null) return false;
        foreach (RackEditOther other in others)
        {
            float distance = SegmentDistance(entrance, far, other.Entrance, other.Far);
            if (distance < distanceM || (distance == distanceM && (rackId == 0 || other.Id < rackId)))
            {
                rackId = other.Id;
                distanceM = distance;
            }
        }
        return rackId != 0;
    }

    // Scenario One gameplay calibration, not a general forestry rule. Boundaries are inclusive.
    public static string SpacingStatus(float distanceM)
        => distanceM < 16f ? "closer than recommended for this stand"
            : distanceM > 20f ? "wider than recommended for this stand"
            : "within the recommended range for this stand";

    // The shortest distance between two segments (0 when they cross or touch).
    public static float SegmentDistance(RackPoint a0, RackPoint a1, RackPoint b0, RackPoint b1)
    {
        if (SegmentsCross(a0, a1, b0, b1))
            return 0f;
        float d = PointToSegment(a0, b0, b1);
        d = Math.Min(d, PointToSegment(a1, b0, b1));
        d = Math.Min(d, PointToSegment(b0, a0, a1));
        d = Math.Min(d, PointToSegment(b1, a0, a1));
        return d;
    }

    public static float PointToSegment(RackPoint p, RackPoint a, RackPoint b)
    {
        float dx = b.X - a.X, dz = b.Z - a.Z, lengthSquared = dx * dx + dz * dz;
        float t = lengthSquared > Epsilon * Epsilon ? ((p.X - a.X) * dx + (p.Z - a.Z) * dz) / lengthSquared : 0f;
        t = Math.Max(0f, Math.Min(1f, t));
        float ex = p.X - (a.X + t * dx), ez = p.Z - (a.Z + t * dz);
        return (float)Math.Sqrt(ex * ex + ez * ez);
    }

    private static bool SegmentsCross(RackPoint a0, RackPoint a1, RackPoint b0, RackPoint b1)
    {
        float d1 = Cross(b0, b1, a0), d2 = Cross(b0, b1, a1), d3 = Cross(a0, a1, b0), d4 = Cross(a0, a1, b1);
        return ((d1 > 0f && d2 < 0f) || (d1 < 0f && d2 > 0f)) && ((d3 > 0f && d4 < 0f) || (d3 < 0f && d4 > 0f));
    }

    private static float Cross(RackPoint o, RackPoint a, RackPoint p) => (a.X - o.X) * (p.Z - o.Z) - (a.Z - o.Z) * (p.X - o.X);

    // The direction a rack points from its entrance, in degrees from east, counter-clockwise, in (-180, 180].
    public static float DirectedAngleDegrees(RackPoint entrance, RackPoint far)
        => (float)(Math.Atan2(far.Z - entrance.Z, far.X - entrance.X) * 180.0 / Math.PI);

    // The smallest turn between two directions, in degrees (0..180).
    public static float AngleDifferenceDegrees(float a, float b)
    {
        float d = Math.Abs(a - b) % 360f;
        return d > 180f ? 360f - d : d;
    }

    // A soft snap of the far end, advisory only. With the pointer at `desiredFar` and the entrance fixed:
    //   1. if the direction is within AngleSnapDegrees of the suggested orientation, or of a nearby rack's direction, the direction is
    //      pulled to it (the length is kept);
    //   2. if the rack then ends within BoundarySnapM of the stand boundary along that direction, it is pulled to the boundary.
    // Outside those windows the far end is returned unchanged, and `enabled = false` (the player holds Shift) returns it unchanged
    // always, so a deliberate deviation is always possible. The result is NOT validated: a snapped rack can still be refused.
    public static RackSnapResult SnapFar(RackEditBounds bounds, RackPoint entrance, RackPoint desiredFar,
        IReadOnlyList<float> suggestedAnglesDegrees, IReadOnlyList<RackEditOther> nearbyRacks, bool enabled)
    {
        RackPoint far = desiredFar;
        if (!enabled || !bounds.IsValid || !IsFinite(entrance) || !IsFinite(desiredFar))
            return new RackSnapResult(far, false, 0, false);
        float length = RackGeometry.LengthM(entrance, desiredFar);
        if (length < Epsilon)
            return new RackSnapResult(far, false, 0, false);

        float angle = DirectedAngleDegrees(entrance, desiredFar);
        float best = AngleSnapDegrees + Epsilon, snapAngle = angle;
        int parallelTo = 0;
        bool angleSnapped = false;

        if (nearbyRacks != null)
        {
            for (int i = 0; i < nearbyRacks.Count; i++)
            {
                RackEditOther other = nearbyRacks[i];
                if (Math.Abs(other.Entrance.Z - entrance.Z) > ParallelSearchM || RackGeometry.LengthM(other.Entrance, other.Far) < Epsilon)
                    continue;
                float candidate = DirectedAngleDegrees(other.Entrance, other.Far);
                float difference = AngleDifferenceDegrees(angle, candidate);
                if (difference < best)
                {
                    best = difference;
                    snapAngle = candidate;
                    parallelTo = other.Id;
                    angleSnapped = true;
                }
            }
        }
        if (suggestedAnglesDegrees != null)
        {
            for (int i = 0; i < suggestedAnglesDegrees.Count; i++)
            {
                float difference = AngleDifferenceDegrees(angle, suggestedAnglesDegrees[i]);
                // A tie goes to the nearby rack: parallel to the player's own racks is the stronger cue.
                if (difference < best)
                {
                    best = difference;
                    snapAngle = suggestedAnglesDegrees[i];
                    parallelTo = 0;
                    angleSnapped = true;
                }
            }
        }
        if (angleSnapped)
        {
            double radians = snapAngle * Math.PI / 180.0;
            far = new RackPoint(entrance.X + length * (float)Math.Cos(radians), entrance.Z + length * (float)Math.Sin(radians));
        }

        bool boundarySnapped = false;
        float directed = DirectedAngleDegrees(entrance, far);
        // ClipForwardToSquare takes the full 360 degree direction (RackGeometry.Direction is cos/sin of it), which this angle is.
        if (RackGeometry.ClipForwardToSquare(entrance, directed, bounds.HalfExtentM, out RackPoint end))
        {
            float toBoundary = RackGeometry.LengthM(entrance, end);
            if (Math.Abs(toBoundary - RackGeometry.LengthM(entrance, far)) <= BoundarySnapM && toBoundary >= MinLengthM)
            {
                far = end;
                boundarySnapped = true;
            }
        }
        return new RackSnapResult(far, angleSnapped, angleSnapped ? parallelTo : 0, boundarySnapped);
    }

    // True when the racks in the plan are exactly the suggested routes (so a reset would change nothing and loses nothing).
    public static bool MatchesSuggestion(IReadOnlyList<RackEditOther> racks, IReadOnlyList<KeyValuePair<RackPoint, RackPoint>> suggested)
    {
        if (racks == null || suggested == null || racks.Count != suggested.Count)
            return false;
        var used = new bool[suggested.Count];
        for (int i = 0; i < racks.Count; i++)
        {
            bool found = false;
            for (int j = 0; j < suggested.Count && !found; j++)
            {
                if (used[j]) continue;
                if (RackGeometry.LengthM(racks[i].Entrance, suggested[j].Key) <= SuggestedMatchToleranceM
                    && RackGeometry.LengthM(racks[i].Far, suggested[j].Value) <= SuggestedMatchToleranceM)
                {
                    used[j] = true;
                    found = true;
                }
            }
            if (!found) return false;
        }
        return true;
    }

    // The closest approach between a rack's centreline and a pointer position, for hit testing.
    public static float DistanceToRack(RackPoint pointer, RackPoint entrance, RackPoint far) => PointToSegment(pointer, entrance, far);
}
