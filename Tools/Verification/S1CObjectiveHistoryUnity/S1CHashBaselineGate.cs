#if UNITY_EDITOR
using System;
using System.IO;
using UnityEngine;
using UnityEditor;

// Compare the actual pre-v22 DTO and archive hash methods with the candidate through Unity JsonUtility.
public static class S1CHashBaselineGate
{
    private static string Current(ForestSaveData d) => ScenarioReferenceArchive.WorldHash(d) + "|" + ScenarioReferenceArchive.CurrentWorldHash(d)
        + "|" + ScenarioReferenceArchive.LegacyCompatibleWorldHash(d) + "|" + ScenarioReferenceArchive.LegacyV15WorldHash(d)
        + "|" + ScenarioReferenceArchive.LegacyV16WorldHash(d) + "|" + ScenarioReferenceArchive.LegacyV17WorldHash(d) + "|" + ScenarioReferenceArchive.LegacyV18WorldHash(d);
    private static string Old(PreV22ForestSaveData d) => PreV22Archive.WorldHash(d) + "|" + PreV22Archive.CurrentWorldHash(d)
        + "|" + PreV22Archive.LegacyCompatibleWorldHash(d) + "|" + PreV22Archive.LegacyV15WorldHash(d)
        + "|" + PreV22Archive.LegacyV16WorldHash(d) + "|" + PreV22Archive.LegacyV17WorldHash(d) + "|" + PreV22Archive.LegacyV18WorldHash(d);
    public static void Begin()
    {
        int count = 0;
        try
        {
            foreach (var fixture in HashFixtures.Build())
            {
                ForestSaveData d = fixture.Value;
                var old = JsonUtility.FromJson<PreV22ForestSaveData>(JsonUtility.ToJson(d));
                if (old.version >= 22) old.version = 21; // corresponding pre-v22 writer's schema header
                Compare(fixture.Key, d, old); count++;
                d.scenarioOne.objectiveAchievements = new System.Collections.Generic.List<ScenarioObjectiveAchievement> {
                    new ScenarioObjectiveAchievement { objectiveId="retained-canopy", achievedYear=1, targetValueAtAchievement=3f } };
                Compare(fixture.Key + "-recorded", d, old); count++;
                // Real persisted v20 headers also go through both Unity serializers/hash implementations.
                if (d.scenarioOne.rackPlanningModel == 0 && d.scenarioOne.racks.Count == 0 && !d.scenarioOne.rackPlanConfirmed
                    && d.scenarioOne.workOrders.TrueForAll(o => o.rackId == 0))
                { d.version=20; old.version=20; d.scenarioOne.objectiveAchievements.Clear(); Compare(fixture.Key+"-v20",d,old); count++; }
            }
            if (Accepted() != null && Accepted().Count != count) throw new InvalidOperationException("accepted baseline has " + Accepted().Count + " rows but " + count + " cases ran");
            Debug.Log("T05_BASELINE_HASH_PASS cases=" + count + " serializer=UnityJsonUtility baseline=aca74ac noAnchorsRelabelled=true");
            EditorApplication.Exit(0);
        }
        catch (Exception e) { Debug.LogError("T05_BASELINE_HASH_FAIL " + e); EditorApplication.Exit(1); }
    }
    // T-08 reconciliation (verification-only). On a v23/v24 candidate the live pre-v22 derivation above is no longer a faithful
    // pre-v22 identity: PreV22SaveTypes shares the current work-order/event types, which now serialise the MFM-06 prescription
    // fields that the frozen pre-v22 hashing source cannot strip. When CCF_T08_ACCEPTED_BASELINE names the accepted T-05
    // table (rows produced by the real pre-v22 aca74ac source, each with candidate == baseline), the candidate must
    // reproduce that accepted identity EXACTLY for every case. Without it the original live comparison applies unchanged.
    private static System.Collections.Generic.Dictionary<string, string> accepted;
    private static System.Collections.Generic.Dictionary<string, string> Accepted()
    {
        if (accepted != null) return accepted;
        string path = Environment.GetEnvironmentVariable("CCF_T08_ACCEPTED_BASELINE");
        if (string.IsNullOrEmpty(path)) return null;
        accepted = new System.Collections.Generic.Dictionary<string, string>();
        foreach (string line in File.ReadAllLines(path))
        {
            string[] part = line.Split(' ');
            if (part.Length != 3 || !part[1].StartsWith("candidate=") || !part[2].StartsWith("baseline=") || part[1].Substring(10) != part[2].Substring(9))
                throw new InvalidOperationException("accepted baseline row malformed or candidate != baseline: " + line);
            accepted.Add(part[0], part[2].Substring(9));
        }
        return accepted;
    }
    private static void Compare(string label, ForestSaveData d, PreV22ForestSaveData old)
    {
        string a = Current(d), b = Old(old);
        var table = Accepted();
        string expected = table == null ? b : (table.TryGetValue(label, out var row) ? row : throw new InvalidOperationException(label + " has no accepted baseline row"));
        Debug.Log("T05_BASELINE_HASH_CASE " + label + " candidate=" + a + " baseline=" + expected + (table != null ? " liveDerivation=" + b : ""));
        File.AppendAllText(Path.Combine(Environment.GetEnvironmentVariable("CCF_DIAG_DIR"), "baseline-hashes.txt"), label + " candidate=" + a + " baseline=" + expected + (table != null ? " liveDerivation=" + b : "") + "\n");
        if (a != expected) throw new InvalidOperationException(label + " world identity moved");
    }
}
#endif
