#if UNITY_EDITOR
using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class WindthrowVisualVerification
{
    public static void Begin()
    {
        const string path = "Assets/ForestPrototype/ScenarioOne/Resources/WindthrowVisualCatalog.asset";
        var catalog = AssetDatabase.LoadAssetAtPath<WindthrowVisualCatalog>(path);
        if (catalog == null)
        {
            catalog = ScriptableObject.CreateInstance<WindthrowVisualCatalog>();
            catalog.freshRootPlate = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/ForestPrototype/Prefabs/Forestry/Ground/SS_WindthrowBase_Fresh_01.prefab");
            catalog.weatheredRootPlate = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/ForestPrototype/Prefabs/Forestry/Ground/SS_WindthrowBase_Weathered_01.prefab");
            if (catalog.freshRootPlate == null || catalog.weatheredRootPlate == null) throw new Exception("Existing root-plate assets unavailable");
            AssetDatabase.CreateAsset(catalog, path);
        }
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity"); EditorApplication.isPlaying = true;
    }
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    { if (Environment.GetCommandLineArgs().Contains("WindthrowVisualVerification.Begin")) new GameObject("Disposable windthrow visual verification").AddComponent<WindthrowVisualVerificationRunner>(); }
}
public sealed class WindthrowVisualVerificationRunner : MonoBehaviour
{
    int checks;
    void Check(bool ok, string message) { checks++; if (!ok) throw new Exception(message); }
    IEnumerator Start()
    {
        yield return null; yield return null;
        Exception failure = null;
        IEnumerator work = Verify();
        while (true)
        {
            bool more = false; object current = null;
            try { more = work.MoveNext(); if (more) current = work.Current; }
            catch (Exception error) { failure = error; Debug.LogError("WINDTHROW_VISUAL_VERIFICATION_FAIL " + error); break; }
            if (!more) break;
            yield return current;
        }
        if (failure == null) Debug.Log("WINDTHROW_VISUAL_VERIFICATION_PASS checks=" + checks);
        EditorApplication.ExitPlaymode(); EditorApplication.Exit(failure == null ? 0 : 1);
    }
    IEnumerator Verify()
    {
        var e = FindFirstObjectByType<ForestEcologyController>(); var m = FindFirstObjectByType<ScenarioOneManager>(); var saves = FindFirstObjectByType<ForestSaveController>();
        var original = saves.CaptureData();
        Check(e.StormModelVersion == 0, "visual gate starts from storms-off main");
        e.StormModelVersion = 1; e.ForceStormNextYear(.7f, 135); e.AdvanceOneYear(); yield return null;
        var snapshot = saves.CaptureData();
        var budget = m.GetComponent<ScenarioWindthrowVisualBudget>();
        var visuals = m.GetComponentsInChildren<ScenarioWindthrowVisual>();
        Check(visuals.Length == e.LastStormPerformance.Victims && visuals.Length > 20, "each victim has one visible directed windthrow root");
        foreach (var visual in visuals)
        {
            var record = snapshot.scenarioOne.deadwoodRecords.Single(value => value.treeId == visual.TreeId);
            Check(Vector3.Distance(visual.transform.position, record.worldPosition) < .001f, "root plate anchored at victim base");
            Check(Mathf.Abs(Mathf.DeltaAngle(visual.transform.eulerAngles.y, StormWindthrow.FallBearing(m.StormEvents[0], visual.TreeId, e.RngModelVersion, e.SimulationSeed))) < .001f, "recorded event determines fall direction");
            Check(visual.transform.Find("Uprooted root plate") != null && visual.transform.Find("Directed fallen stem") != null, "existing authored root plate and log wired");
            Check(visual.GetComponent<BoxCollider>().isTrigger, "fallen work target does not obstruct walking");
        }
        budget.RefreshAt(Vector3.zero); Check(budget.ActiveCrowns == 20, "near fallen-crown budget is bounded at20"); yield return null;
        Screenshot("fresh-windthrow-overview.png", new Vector3(-26, 17, -26), new Vector3(0, 2, 0));
        var nearest = visuals.OrderBy(value => value.transform.position.sqrMagnitude).First();
        Vector3 basePosition = nearest.transform.position;
        budget.RefreshAt(basePosition);
        Screenshot("fresh-root-plate-close.png", basePosition + new Vector3(-5, 3, -5), basePosition + new Vector3(0, .5f, 2));
        budget.RefreshAt(new Vector3(1000, 0, 1000)); Check(budget.ActiveCrowns == 0 && visuals.All(value => !value.HasCrown), "distant crowns released without losing roots or logs"); yield return null;
        string anchor = ScenarioReferenceArchive.WorldHash(snapshot);
        Check(saves.LoadData(snapshot, false), "windthrow visual save reload"); yield return null;
        Check(ScenarioReferenceArchive.WorldHash(saves.CaptureData()) == anchor, "visual reconstruction leaves saved world unchanged");
        visuals = m.GetComponentsInChildren<ScenarioWindthrowVisual>();
        Check(visuals.Length == snapshot.trees.Count(tree => tree.biologicallyDead && tree.mortalityCause == "windthrow"), "reload reconstructs all windthrow visuals once");
        foreach (var record in m.DeadwoodRecords)
        { record.remainingVolumeM3 = record.originalVolumeM3 * .7f; record.lastDecayYear = record.fallenYear + 4; m.transform.Find("Fallen Log " + record.deadwoodId)?.GetComponent<ScenarioWindthrowVisual>()?.Refresh(record); }
        budget.RefreshAt(Vector3.zero); Check(budget.ActiveCrowns == 0, "weathered records do not carry fresh living crowns"); yield return null;
        Screenshot("weathered-root-plate-close.png", basePosition + new Vector3(-5, 3, -5), basePosition + new Vector3(0, .5f, 2));
        File.WriteAllText(Path.Combine(Environment.GetEnvironmentVariable("CCF_STORM_OUTPUT"), "visual-summary.json"), "{\"checks\":" + checks + ",\"victims\":" + visuals.Length + ",\"maximumCrowns\":20}");
        Check(saves.LoadData(original, false), "restore visual fixture"); yield return null;
    }
    void Screenshot(string file, Vector3 position, Vector3 target)
    {
        var obj = new GameObject("Disposable render camera"); var camera = obj.AddComponent<Camera>(); camera.CopyFrom(Camera.main); camera.enabled = false;
        obj.transform.position = position; obj.transform.LookAt(target);
        var texture = new RenderTexture(1600, 900, 24); camera.targetTexture = texture; camera.Render();
        RenderTexture previous = RenderTexture.active; RenderTexture.active = texture;
        var pixels = new Texture2D(1600, 900, TextureFormat.RGB24, false); pixels.ReadPixels(new Rect(0, 0, 1600, 900), 0, 0); pixels.Apply();
        File.WriteAllBytes(Path.Combine(Environment.GetEnvironmentVariable("CCF_STORM_OUTPUT"), file), pixels.EncodeToPNG());
        camera.targetTexture = null; RenderTexture.active = previous; texture.Release(); Destroy(texture); Destroy(pixels); Destroy(obj);
    }
}
#endif
