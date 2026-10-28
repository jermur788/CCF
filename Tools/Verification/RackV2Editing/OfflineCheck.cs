using System;
using System.Collections.Generic;

// Offline (plain .NET) check of RackEditing, the Unity-free editing rules behind RACK-V2-05 (RackEditing.cs + RackGeometry.cs only).
// It proves the rules in isolation: entrance resolution onto the headland, the validity/negative cases, that snapping is soft and
// advisory, that moving keeps shape, and the reset-matches-suggestion test. It proves nothing about the live plan, the facade,
// the map or the pointer: those are the Unity gate's job.
public static class OfflineCheck
{
    private static int checks, failures;

    private static void Check(bool ok, string message)
    {
        checks++;
        if (ok) return;
        failures++;
        Console.WriteLine("FAIL " + message);
    }

    private static bool Near(float a, float b, float tolerance = 0.001f) => Math.Abs(a - b) <= tolerance;
    private static bool Near(RackPoint a, RackPoint b, float tolerance = 0.001f) => Near(a.X, b.X, tolerance) && Near(a.Z, b.Z, tolerance);

    // The qualified Enlarged80 values, as the manager supplies them: headland x -35.4, strip z +-37.7, stand half side 40.
    private static readonly RackEditBounds Bounds = new RackEditBounds(-35.4f, -37.7f, 37.7f, 40f);
    private static RackPoint P(float x, float z) => new RackPoint(x, z);

    public static int Main()
    {
        Entrances();
        Creation();
        Moving();
        Validation();
        Snapping();
        Matching();
        Console.WriteLine(failures == 0 ? "RACK_EDITING_OFFLINE_PASS checks=" + checks : "RACK_EDITING_OFFLINE_FAIL failures=" + failures + " of " + checks);
        return failures == 0 ? 0 : 1;
    }

    private static void Entrances()
    {
        RackPoint e = RackEditing.SnapEntrance(Bounds, P(-10f, 12.5f));
        Check(Near(e.X, -35.4f) && Near(e.Z, 12.5f), "an interior point snaps onto the headland at the same z");
        Check(Near(RackEditing.SnapEntrance(Bounds, P(0f, 39.9f)).Z, 37.7f), "past the north end of the strip clamps to the strip end");
        Check(Near(RackEditing.SnapEntrance(Bounds, P(0f, -39.9f)).Z, -37.7f), "past the south end of the strip clamps to the strip end");
        Check(RackEditing.EntranceOnHeadland(Bounds, P(-35.4f, 0f)), "the headland line is a valid entrance");
        Check(RackEditing.EntranceOnHeadland(Bounds, P(-35.405f, 37.7f)), "the strip end is a valid entrance within tolerance");
        Check(!RackEditing.EntranceOnHeadland(Bounds, P(-35.0f, 0f)), "0.4 m off the headland is not an entrance");
        Check(!RackEditing.EntranceOnHeadland(Bounds, P(-35.4f, 39f)), "beyond the strip is not an entrance");
    }

    private static void Creation()
    {
        // Pressed at the headland, dragged into the stand.
        RackEditing.ResolveCreation(Bounds, P(-34.0f, 5f), P(20f, 7f), out RackPoint entrance, out RackPoint far);
        Check(Near(entrance, P(-35.4f, 5f)) && Near(far, P(20f, 7f)), "press near the headland, drag inward: the pressed end is the entrance");
        // Pressed inside, dragged to the headland.
        RackEditing.ResolveCreation(Bounds, P(20f, 7f), P(-34.0f, 5f), out entrance, out far);
        Check(Near(entrance, P(-35.4f, 5f)) && Near(far, P(20f, 7f)), "press inside, drag to the headland: the dragged end is the entrance");
        // Equal distance: the pressed end wins (deterministic). Exactly representable numbers make a true tie.
        var tidy = new RackEditBounds(-32f, -37.7f, 37.7f, 40f);
        RackEditing.ResolveCreation(tidy, P(-12f, 1f), P(-52f, 2f), out entrance, out far);
        Check(Near(entrance.Z, 1f) && Near(far.Z, 2f), "a tie goes to the pressed end");
    }

    private static void Moving()
    {
        RackEditing.MoveAlongHeadland(Bounds, P(-35.4f, -10f), P(40f, -6f), 4f, out RackPoint e, out RackPoint f);
        Check(Near(e, P(-35.4f, 4f)), "a move slides the entrance along the headland");
        Check(Near(RackGeometry.LengthM(e, f), RackGeometry.LengthM(P(-35.4f, -10f), P(40f, -6f))), "a move keeps the length");
        Check(Near(RackEditing.DirectedAngleDegrees(e, f), RackEditing.DirectedAngleDegrees(P(-35.4f, -10f), P(40f, -6f)), 0.01f), "a move keeps the angle");
        Check(Near(f.X, 40f) && Near(f.Z, 8f), "the far end shifts by exactly the entrance step");

        // A stray entrance x re-snaps and the far end moves with it.
        RackEditing.MoveAlongHeadland(Bounds, P(-30f, 0f), P(40f, 0f), 0f, out e, out f);
        Check(Near(e.X, -35.4f) && Near(f.X, 34.6f), "an entrance off the headland re-snaps and carries the far end");

        // Past the strip end the move stops at the end.
        RackEditing.MoveAlongHeadland(Bounds, P(-35.4f, 30f), P(40f, 30f), 50f, out e, out f);
        Check(Near(e.Z, 37.7f) && Near(f.Z, 37.7f), "a move past the strip end stops at the end");
    }

    private static void Validation()
    {
        var none = new List<RackEditOther>();
        Check(RackEditing.Validate(Bounds, P(-35.4f, 0f), P(40f, 0f), none).IsValid, "a full-width east rack is valid");
        Check(RackEditing.Validate(Bounds, P(-35.4f, 0f), P(-35.4f + 5.01f, 0f), none).IsValid, "a rack just over the minimum length is valid");

        RackEditValidation v = RackEditing.Validate(Bounds, P(-35.4f, 0f), P(-35.4f, 0f), none);
        Check(v.Problem == RackEditProblem.TooShort, "zero length is refused as too short");
        v = RackEditing.Validate(Bounds, P(-35.4f, 0f), P(-34.4f, 0f), none);
        Check(v.Problem == RackEditProblem.TooShort, "near-zero length is refused as too short");
        v = RackEditing.Validate(Bounds, P(-35.4f, 0f), P(-35.4f + 4.9f, 0f), none);
        Check(v.Problem == RackEditProblem.TooShort && v.Message.Contains("shorter than 5 m"), "just under the minimum is refused with the length in the message");

        v = RackEditing.Validate(Bounds, P(-35.4f, 0f), P(float.NaN, 0f), none);
        Check(v.Problem == RackEditProblem.NonFinite, "a NaN endpoint is refused");
        v = RackEditing.Validate(Bounds, P(float.PositiveInfinity, 0f), P(10f, 0f), none);
        Check(v.Problem == RackEditProblem.NonFinite, "an infinite endpoint is refused");

        v = RackEditing.Validate(Bounds, P(-35.4f, 0f), P(40.5f, 0f), none);
        Check(v.Problem == RackEditProblem.OutsideStand, "a far end past the east boundary is outside the stand");
        v = RackEditing.Validate(Bounds, P(-35.4f, 0f), P(10f, 41f), none);
        Check(v.Problem == RackEditProblem.OutsideStand, "a far end past the north boundary is outside the stand");
        v = RackEditing.Validate(Bounds, P(-35.4f, 0f), P(40f, 0f), null);
        Check(v.IsValid, "no other racks is the same as an empty list");

        v = RackEditing.Validate(Bounds, P(-30f, 0f), P(30f, 0f), none);
        Check(v.Problem == RackEditProblem.EntranceOffHeadland, "an entrance inside the stand, off the headland, is refused");
        v = RackEditing.Validate(Bounds, P(-35.4f, 39f), P(30f, 39f), none);
        Check(v.Problem == RackEditProblem.EntranceOffHeadland, "an entrance past the strip end is refused");

        var others = new List<RackEditOther> { new RackEditOther(7, P(-35.4f, 0f), P(40f, 0f)) };
        v = RackEditing.Validate(Bounds, P(-35.4f, 0f), P(40f, 0f), others);
        Check(v.Problem == RackEditProblem.Duplicate && v.OtherRackId == 7 && v.Message.Contains("duplicates rack 7"), "an identical rack is a duplicate and names the rack");
        v = RackEditing.Validate(Bounds, P(-35.4f, 0.3f), P(40f, 0.3f), others);
        Check(v.Problem == RackEditProblem.Duplicate, "a rack within the duplicate tolerance is a duplicate");
        v = RackEditing.Validate(Bounds, P(-35.4f, 1.0f), P(40f, 1.0f), others);
        Check(v.Problem == RackEditProblem.Overlap && v.OtherRackId == 7, "a parallel rack 1 m away overlaps (corridors are 2.4 m wide)");
        v = RackEditing.Validate(Bounds, P(-35.4f, 2.3f), P(40f, 2.3f), others);
        Check(v.Problem == RackEditProblem.Overlap, "2.3 m away still overlaps");
        v = RackEditing.Validate(Bounds, P(-35.4f, 2.4f), P(40f, 2.4f), others);
        Check(v.IsValid, "exactly one corridor width away is valid");
        v = RackEditing.Validate(Bounds, P(-35.4f, -20f), P(40f, 20f), others);
        Check(v.Problem == RackEditProblem.Overlap, "a rack that crosses another overlaps it");
        v = RackEditing.Validate(Bounds, P(-35.4f, 10f), P(40f, 10f), others);
        Check(v.IsValid, "a rack 10 m away is valid");
        // Order of rules: outside is reported before overlap.
        v = RackEditing.Validate(Bounds, P(-35.4f, 0f), P(41f, 0f), others);
        Check(v.Problem == RackEditProblem.OutsideStand, "outside the stand is reported before a duplicate");

        Check(Near(RackEditing.SegmentDistance(P(0f, 0f), P(10f, 0f), P(0f, 3f), P(10f, 3f)), 3f), "parallel segments: the gap");
        Check(Near(RackEditing.SegmentDistance(P(0f, 0f), P(10f, 10f), P(0f, 10f), P(10f, 0f)), 0f), "crossing segments: zero");
        Check(Near(RackEditing.SegmentDistance(P(0f, 0f), P(10f, 0f), P(12f, 3f), P(20f, 3f)), (float)Math.Sqrt(13.0)), "end to end: the corner gap");
        Check(Near(RackEditing.SegmentDistance(P(0f, 0f), P(0f, 0f), P(3f, 4f), P(3f, 4f)), 5f), "point to point");
    }

    private static void Snapping()
    {
        var suggested = new List<float> { 0f };
        var none = new List<RackEditOther>();
        RackPoint entrance = P(-35.4f, 0f);

        // 2 degrees off east: inside the window, pulled to 0 degrees, length kept.
        float length = 50f;
        double rad = 2.0 * Math.PI / 180.0;
        RackPoint pointer = P(entrance.X + length * (float)Math.Cos(rad), entrance.Z + length * (float)Math.Sin(rad));
        RackSnapResult s = RackEditing.SnapFar(Bounds, entrance, pointer, suggested, none, true);
        Check(s.AngleSnapped && s.ParallelToRackId == 0 && Near(RackEditing.DirectedAngleDegrees(entrance, s.Far), 0f, 0.01f), "2 deg off the suggested direction snaps to it");
        Check(Near(RackGeometry.LengthM(entrance, s.Far), length, 0.01f), "an angle snap keeps the length");

        // 6 degrees off: outside the window, left alone (a deliberate deviation).
        rad = 6.0 * Math.PI / 180.0;
        pointer = P(entrance.X + length * (float)Math.Cos(rad), entrance.Z + length * (float)Math.Sin(rad));
        s = RackEditing.SnapFar(Bounds, entrance, pointer, suggested, none, true);
        Check(!s.AngleSnapped && Near(s.Far, pointer, 0.001f), "6 deg off is outside the window: free");
        // The boundary snap can still apply to a free angle, but never changes the direction.
        Check(Near(RackEditing.DirectedAngleDegrees(entrance, s.Far), 6f, 0.05f), "a free angle stays the angle the player chose");

        // Shift: snapping off even inside the window.
        rad = 2.0 * Math.PI / 180.0;
        pointer = P(entrance.X + length * (float)Math.Cos(rad), entrance.Z + length * (float)Math.Sin(rad));
        s = RackEditing.SnapFar(Bounds, entrance, pointer, suggested, none, false);
        Check(!s.Snapped && Near(s.Far, pointer, 0.0001f), "with snapping disabled the pointer position is returned unchanged");

        // Parallel to a nearby player rack at 10 degrees: the nearer cue wins over the suggested direction.
        rad = 10.0 * Math.PI / 180.0;
        var parallel = new List<RackEditOther> { new RackEditOther(3, P(-35.4f, 12f), P(-35.4f + 60f * (float)Math.Cos(rad), 12f + 60f * (float)Math.Sin(rad))) };
        rad = 11.5 * Math.PI / 180.0;
        pointer = P(entrance.X + length * (float)Math.Cos(rad), entrance.Z + length * (float)Math.Sin(rad));
        s = RackEditing.SnapFar(Bounds, entrance, pointer, suggested, parallel, true);
        Check(s.AngleSnapped && s.ParallelToRackId == 3 && Near(RackEditing.DirectedAngleDegrees(entrance, s.Far), 10f, 0.01f), "within the window of a nearby rack's direction it snaps parallel to it");
        // A rack too far along the headland is not nearby.
        var farAway = new List<RackEditOther> { new RackEditOther(4, P(-35.4f, 36f), P(-35.4f + 60f * (float)Math.Cos(rad), 36f)) };
        s = RackEditing.SnapFar(Bounds, entrance, pointer, new List<float>(), farAway, true);
        Check(!s.AngleSnapped, "a rack more than the search distance away is not a parallel target");

        // Boundary snap: a far end 1.5 m short of the east edge goes to the edge.
        s = RackEditing.SnapFar(Bounds, entrance, P(38.5f, 0f), suggested, none, true);
        Check(s.BoundarySnapped && Near(s.Far, P(40f, 0f), 0.001f), "a far end within 2.5 m of the boundary snaps to it");
        s = RackEditing.SnapFar(Bounds, entrance, P(30f, 0f), suggested, none, true);
        Check(!s.BoundarySnapped && Near(s.Far, P(30f, 0f), 0.001f), "a far end well inside the stand stays where it is");

        // Snapping never makes an invalid plan valid: a snap onto a duplicate is still a duplicate.
        var existing = new List<RackEditOther> { new RackEditOther(1, P(-35.4f, 0f), P(40f, 0f)) };
        s = RackEditing.SnapFar(Bounds, P(-35.4f, 0.4f), P(38.5f, 0.9f), suggested, existing, true);
        Check(!RackEditing.Validate(Bounds, P(-35.4f, 0.4f), s.Far, existing).IsValid, "a snapped rack is still validated and can still be refused");

        // Degenerate pointer: on the entrance. No division by zero, no snap.
        s = RackEditing.SnapFar(Bounds, entrance, entrance, suggested, none, true);
        Check(!s.Snapped && Near(s.Far, entrance), "a pointer exactly on the entrance cannot snap");
        s = RackEditing.SnapFar(Bounds, entrance, P(float.NaN, 1f), suggested, none, true);
        Check(!s.Snapped, "a NaN pointer cannot snap");
        Check(Near(RackEditing.AngleDifferenceDegrees(179f, -179f), 2f), "angle difference wraps across 180");
        Check(Near(RackEditing.AngleDifferenceDegrees(10f, 350f), 20f), "angle difference wraps across 0/360");
    }

    private static void Matching()
    {
        var suggestion = new List<KeyValuePair<RackPoint, RackPoint>>();
        var racks = new List<RackEditOther>();
        for (int i = 0; i < 5; i++)
        {
            float z = -30f + i * 15f;
            suggestion.Add(new KeyValuePair<RackPoint, RackPoint>(P(-35.4f, z), P(40f, z)));
            racks.Add(new RackEditOther(10 + i, P(-35.4f, z), P(40f, z)));
        }
        Check(RackEditing.MatchesSuggestion(racks, suggestion), "the plan equal to the suggestion matches (ids are irrelevant)");
        racks.Reverse();
        Check(RackEditing.MatchesSuggestion(racks, suggestion), "order does not matter");
        racks[0] = new RackEditOther(99, P(-35.4f, racks[0].Entrance.Z + 0.5f), racks[0].Far);
        Check(!RackEditing.MatchesSuggestion(racks, suggestion), "one rack moved 0.5 m no longer matches");
        racks.RemoveAt(0);
        Check(!RackEditing.MatchesSuggestion(racks, suggestion), "a missing rack does not match");
        Check(!RackEditing.MatchesSuggestion(null, suggestion) && !RackEditing.MatchesSuggestion(racks, null), "null never matches");
        Check(RackEditing.MatchesSuggestion(new List<RackEditOther>(), new List<KeyValuePair<RackPoint, RackPoint>>()), "an empty plan matches an empty suggestion");
        Check(Near(RackEditing.DistanceToRack(P(5f, 3f), P(0f, 0f), P(10f, 0f)), 3f), "pointer distance to a rack is the distance to its centre line");
    }
}
