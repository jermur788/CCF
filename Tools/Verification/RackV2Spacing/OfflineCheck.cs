using System;
using System.Collections.Generic;

public static class OfflineCheck
{
    static int checks;
    static void Check(bool condition, string message)
    {
        checks++;
        if (!condition) throw new Exception(message);
    }
    static RackPoint P(float x, float z) => new RackPoint(x, z);
    static RackEditOther Rack(int id, float z) => new RackEditOther(id, P(0, z), P(40, z));
    public static int Main()
    {
        var a = P(0, 0); var b = P(40, 0);
        var racks = new List<RackEditOther> { Rack(9, 18), Rack(3, -18), Rack(7, 22) };
        Check(RackEditing.NearestRack(a, b, racks, out int id, out float distance) && id == 3 && distance == 18, "tie by id");
        racks.Reverse();
        Check(RackEditing.NearestRack(a, b, racks, out id, out distance) && id == 3 && distance == 18, "order independent");
        Check(!RackEditing.NearestRack(a, b, new List<RackEditOther>(), out id, out distance), "no neighbour");
        Check(RackEditing.SpacingStatus(15.99f).StartsWith("closer"), "below");
        Check(RackEditing.SpacingStatus(16).StartsWith("within"), "16 inclusive");
        Check(RackEditing.SpacingStatus(18).StartsWith("within"), "inside");
        Check(RackEditing.SpacingStatus(20).StartsWith("within"), "20 inclusive");
        Check(RackEditing.SpacingStatus(20.01f).StartsWith("wider"), "above");
        Check(RackEditing.SpacingStatus(18).Contains("for this stand"), "site framing");
        var converging = new List<RackEditOther> { new RackEditOther(2, P(0, 20), P(40, 10)) };
        Check(RackEditing.NearestRack(a, b, converging, out id, out distance) && distance == 10, "rotation uses closest centre lines");
        Check(converging[0].Entrance.Z == 20 && converging[0].Far.Z == 10 && a.Z == 0 && b.Z == 0, "geometry unchanged");
        Console.WriteLine("RACK_SPACING_OFFLINE_PASS checks=" + checks);
        return 0;
    }
}
