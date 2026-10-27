using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public static class SuggestedLayoutOfflineCheck
{
    private static int checks;
    private static void Check(bool ok, string message)
    {
        if (!ok) throw new InvalidOperationException(message);
        checks++;
    }

    public static int Main()
    {
        try
        {
            // Independent oracle retained from the qualified headland gate, not read from the new API.
            string[] ids = { "PO0203", "PO1204", "PO2003", "PO2803", "PO3803" };
            float[] zs = { -35.403477f, -16.015210f, 0.014705888f, 16.022398f, 35.436646f };
            var trees = ids.Select((id, i) => new RackTree(id, -33f, zs[i])).ToList();
            trees.Add(new RackTree("unrelated", 0f, 0f));
            RackTree[] before = trees.ToArray();
            var result = ScenarioOneSuggestedRackLayout.Get(1, 1, trees);
            Check(result.Count == 5, "five V2 guidance segments");
            for (int i = 0; i < result.Count; i++)
            {
                var segment = result[i];
                Check(Math.Abs(segment.Entrance.X - -35.4f) <= 0.001f
                    && segment.Entrance.Z == zs[i], "qualified entrance " + i);
                Check(segment.Far.X == 40f && segment.Far.Z == zs[i], "qualified far endpoint " + i);
                Check(segment.WidthM == 2.4f, "qualified corridor width " + i);
            }
            for (int read = 0; read < 100; read++)
            {
                var again = ScenarioOneSuggestedRackLayout.Get(1, 1, trees);
                Check(result.SequenceEqual(again) && trees.SequenceEqual(before), "repeat read preserves values and input " + read);
            }
            trees.Reverse();
            Check(result.SequenceEqual(ScenarioOneSuggestedRackLayout.Get(1, 1, trees)), "input order does not change route order");
            trees = before.Select(t => new RackTree(t.Id, t.X, t.Z, false, true, true)).ToList();
            Check(result.SequenceEqual(ScenarioOneSuggestedRackLayout.Get(1, 1, trees)), "stumps, Crop/Fell and order flags do not redefine guidance");
            Check(typeof(SuggestedRackSegment).GetFields(BindingFlags.Instance | BindingFlags.Public).All(f => f.IsInitOnly), "segment fields immutable");
            Check(typeof(RackPoint).GetFields(BindingFlags.Instance | BindingFlags.Public).All(f => f.IsInitOnly), "nested point fields immutable");
            var writable = result as IList<SuggestedRackSegment>;
            Check(writable != null && writable.IsReadOnly, "returned collection read-only");
            bool refused = false;
            try { writable[0] = default; } catch (NotSupportedException) { refused = true; }
            Check(refused, "collection refuses replacement");
            trees[0] = new RackTree(ids[0], 1f, zs[0] + 0.25f);
            Check(result[0].Entrance.Z == zs[0], "previous result detached from later input changes");
            foreach (int rackModel in new[] { -1, 0, 1, 2 })
                foreach (int geometry in new[] { -1, 0, 1, 2 })
                    if (rackModel != 1 || geometry != 1)
                        Check(ScenarioOneSuggestedRackLayout.Get(rackModel, geometry, before).Count == 0, "reject unsupported world " + rackModel + "/" + geometry);
            Check(ScenarioOneSuggestedRackLayout.Get(1, 1, null).Count == 0, "null snapshot fails closed");
            Check(ScenarioOneSuggestedRackLayout.Get(1, 1, Array.Empty<RackTree>()).Count == 0, "empty snapshot fails closed");
            Check(ScenarioOneSuggestedRackLayout.Get(1, 1, before.Skip(1).ToArray()).Count == 0, "missing qualified tree fails closed");
            Check(ScenarioOneSuggestedRackLayout.Get(1, 1, before.Concat(new[] { before[0] }).ToArray()).Count == 0, "duplicate qualified ID fails closed");
            foreach (float invalid in new[] { float.NaN, float.PositiveInfinity, float.NegativeInfinity, -38f, 38f })
            {
                var bad = before.ToArray(); bad[0] = new RackTree(ids[0], -33f, invalid);
                Check(ScenarioOneSuggestedRackLayout.Get(1, 1, bad).Count == 0, "invalid entrance coordinate fails closed");
            }
            Console.WriteLine("SUGGESTED_LAYOUT_OFFLINE_PASS checks=" + checks);
            return 0;
        }
        catch (Exception error) { Console.Error.WriteLine("SUGGESTED_LAYOUT_OFFLINE_FAIL " + error); return 1; }
    }
}

// StandGeometryModel.ForSave is not exercised offline. Only this unrelated save type is stubbed;
// the actual production layout, geometry, headland, rack identity and stand identity sources compile below.
public sealed class ForestSaveData
{
    public int version, standGeometryModel;
}
