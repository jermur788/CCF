#if UNITY_EDITOR
using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class StormCoreVerification
{
    public static void Begin() { EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity"); EditorApplication.isPlaying = true; }
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    { if (Environment.GetCommandLineArgs().Contains("StormCoreVerification.Begin")) new GameObject("Disposable storm core gate").AddComponent<StormCoreVerificationRunner>(); }
}
public sealed class StormCoreVerificationRunner : MonoBehaviour
{
    int checks;
    void Check(bool ok, string reason) { checks++; if (!ok) throw new Exception(reason); }
    ForestSaveData Clone(ForestSaveData value) => JsonUtility.FromJson<ForestSaveData>(JsonUtility.ToJson(value));
    string Hash(ForestSaveController saves) => ScenarioReferenceArchive.WorldHash(saves.CaptureData());
    IEnumerator Start()
    {
        yield return null; yield return null;
        Exception failure = null;
        try { Verify(); Debug.Log("STORM_CORE_VERIFICATION_PASS checks=" + checks); }
        catch (Exception error) { failure = error; Debug.LogError("STORM_CORE_VERIFICATION_FAIL " + error); }
        EditorApplication.ExitPlaymode(); EditorApplication.Exit(failure == null ? 0 : 1);
    }
    void Verify()
    {
        var e = FindFirstObjectByType<ForestEcologyController>();
        var m = FindFirstObjectByType<ScenarioOneManager>();
        var saves = FindFirstObjectByType<ForestSaveController>();
        var initial = saves.CaptureData();
        Check(initial.version == 20 && initial.stormModel == 0 && m.StormEvents.Count == 0, "new games remain storms-off v20");
        Check(ScenarioReferenceArchive.LegacyV18WorldHash(initial) == "FA855239CDDA32D8", "accepted model2 start retains exact v18 layout anchor");
        Check(saves.LoadData(initial, false), "accepted loaded-start anchor fixture");
        e.Browsing.BackgroundPressure = .2f;
        e.AdvanceOneYear();
        Debug.Log("STORM_OFF_ANCHOR year=" + ScenarioReferenceArchive.LegacyV18WorldHash(saves.CaptureData()));
        Check(m.StormEvents.Count == 0 && e.LastStormPerformance.Victims == 0, "model0 does no storm work");
        Check(ScenarioReferenceArchive.LegacyV18WorldHash(saves.CaptureData()) == "8333BAA4126E8A09", "storms-off year matches independently verified clean-main state");
        Check(saves.LoadData(initial, false), "restore initial");
        e.StormModelVersion = 1;
        var trees = FindObjectsByType<ForestTree>(FindObjectsSortMode.None).Where(tree => tree.IsLiving).ToArray();
        foreach (ForestTree tree in trees) tree.SetMark(TreeMarkType.CropTree);
        var storm = new StormEventRecord { year = 1, severity = .18f, directionDegrees = 135f };
        var ordered = e.EvaluateStorm(storm, trees);
        var shuffled = e.EvaluateStorm(storm, trees.Reverse().ToArray());
        Check(ordered.Select(value => value.Tree.TreeId + ":" + value.Victim).SequenceEqual(shuffled.Select(value => value.Tree.TreeId + ":" + value.Victim)), "victims independent of tree enumeration");
        var expected = ordered.Where(value => value.Victim).Select(value => value.Tree.TreeId).ToArray();
        Check(expected.Length > 0 && expected.Length < trees.Length, "bounded forced event has nonzero partial damage");
        e.ForceStormNextYear(.18f, 135f); e.AdvanceOneYear();
        Check(e.LastStormPerformance.Victims == expected.Length && m.StormEvents.Count == 1, "one resolved event matches pure preview");
        Check(e.LastStormPerformance.CanopyRebuilds == 1 && e.LastStormPerformance.SeedRebuilds == 1, "one canopy and one seed rebuild for all storm victims");
        Check(m.StormEvents[0].cropTreesLost == expected.Length, "approved compact crop-loss history captured before marks clear");
        var resolved = saves.CaptureData();
        foreach (string id in expected)
        {
            var tree = resolved.trees.Single(value => value.treeId == id);
            Check(tree.biologicallyDead && tree.mortalityCause == "windthrow" && tree.mortalityYear == 1 && tree.markType == 0, "victim persisted as dead with cause/year and erased marks");
            Check(resolved.scenarioOne.deadwoodRecords.Count(value => value.treeId == id) == 1, "one deadwood record per victim");
            var objectTree = trees.Single(value => value.TreeId == id);
            Check(!objectTree.IsLiving && !objectTree.gameObject.activeSelf && !objectTree.ApplyMortality("windthrow", 1), "victim stops living and cannot die twice");
        }
        Check(ForestSaveValidation.Validate(resolved, trees.Length, e.CellCount) == null, "resolved event validates");
        Check(ForestSaveValidation.ValidateStormJson(JsonUtility.ToJson(resolved), resolved) == null, "explicit raw v19 validates");
        Check(ScenarioReferenceArchive.LegacyV18WorldHash(resolved) == null && ScenarioReferenceArchive.LegacyV17WorldHash(resolved) == null, "active storms never impersonate historical layouts");
        string anchor = Hash(saves);
        Check(saves.LoadData(resolved, false) && Hash(saves) == anchor, "storm world survives same-process full save/load");
        Check(m.StormEvents[0].cropTreesLost == expected.Length, "crop loss persists after load");
        var bad = Clone(resolved); bad.scenarioOne.deadwoodRecords.RemoveAll(record => record.treeId == expected[0]);
        Check(!saves.LoadData(bad, false) && Hash(saves) == anchor, "missing windthrow record rejects atomically");
        bad = Clone(resolved); bad.scenarioOne.deadwoodRecords.Add(bad.scenarioOne.deadwoodRecords.Single(record => record.treeId == expected[0]));
        Check(!saves.LoadData(bad, false) && Hash(saves) == anchor, "duplicate windthrow record rejects atomically");
        foreach (float invalid in new[] { -1f, float.NaN, float.PositiveInfinity })
        { bad = Clone(resolved); bad.scenarioOne.deadwoodRecords.Single(record => record.treeId == expected[0]).remainingVolumeM3 = invalid; Check(!saves.LoadData(bad, false) && Hash(saves) == anchor, "invalid remaining windthrow material rejects atomically"); }
        bad = Clone(resolved); bad.scenarioOne.stormEvents[0].cropTreesLost = expected.Length + 1;
        Check(!saves.LoadData(bad, false) && Hash(saves) == anchor, "excess crop losses reject atomically");
        bad = Clone(resolved); bad.scenarioOne.stormEvents.Add(bad.scenarioOne.stormEvents[0]);
        Check(!saves.LoadData(bad, false) && Hash(saves) == anchor, "duplicate year rejects atomically");
        foreach (float invalid in new[] { 0f, -1f, 1.1f, float.NaN, float.PositiveInfinity })
        { bad = Clone(resolved); bad.scenarioOne.stormEvents[0].severity = invalid; Check(!saves.LoadData(bad, false) && Hash(saves) == anchor, "invalid severity rejects atomically"); }
        bad = Clone(resolved); bad.stormModel = 0;
        Check(!saves.LoadData(bad, false) && Hash(saves) == anchor, "storms-off cannot carry resolved storm history");
        var token = Newtonsoft.Json.Linq.JObject.Parse(JsonUtility.ToJson(resolved));
        token.Remove("stormModel");
        Check(ForestSaveValidation.ValidateStormJson(token.ToString(), resolved) != null, "v19 omitted model rejected");
        token = Newtonsoft.Json.Linq.JObject.Parse(JsonUtility.ToJson(resolved));
        ((Newtonsoft.Json.Linq.JObject)token["scenarioOne"]["stormEvents"][0]).Remove("cropTreesLost");
        Check(ForestSaveValidation.ValidateStormJson(token.ToString(), resolved) != null, "v19 omitted crop count rejected");
        e.AdvanceOneYear(); string continued = Hash(saves);
        Check(saves.LoadData(resolved, false), "restore storm checkpoint"); e.AdvanceOneYear();
        Check(Hash(saves) == continued, "future after loaded storm matches uninterrupted world");
        var old = Clone(initial); old.version = 18;
        Check(saves.LoadData(old, false) && e.StormModelVersion == 0 && m.StormEvents.Count == 0, "v18 remains storms-off");
        string output = Environment.GetEnvironmentVariable("CCF_STORM_OUTPUT");
        File.WriteAllText(Path.Combine(output, "resolved_storm.json"), JsonUtility.ToJson(resolved, true));
        File.WriteAllText(Path.Combine(output, "core_summary.json"), JsonUtility.ToJson(new Summary { checks = checks, victims = expected.Length, anchor = anchor, continued = continued, evaluationMs = e.LastStormPerformance.EvaluationMilliseconds }, true));
        Debug.Log("STORM_CORE_ANCHOR resolved=" + anchor + " continued=" + continued + " victims=" + expected.Length);
        Check(saves.LoadData(initial, false), "final restore");
    }
    [Serializable] class Summary { public int checks, victims; public string anchor, continued; public double evaluationMs; }
}
#endif
