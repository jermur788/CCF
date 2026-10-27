using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
public static class RackV2HeadlandQualification
{
#if UNITY_EDITOR
    public static void Begin() { EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity"); EditorApplication.isPlaying = true; }
#endif
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install() { if (Environment.GetCommandLineArgs().Any(a => a == "RackV2HeadlandQualification.Begin")) new GameObject("Disposable headland gate").AddComponent<RackV2HeadlandQualificationRunner>(); }
    public static void Size(int width, int height)
    {
#if UNITY_EDITOR
        var assembly = typeof(Editor).Assembly;
        Type sizesType = assembly.GetType("UnityEditor.GameViewSizes");
        Type singleton = typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
        object sizes = singleton.GetProperty("instance", BindingFlags.Public | BindingFlags.Static).GetValue(null);
        MethodInfo getGroup = sizesType.GetMethod("GetGroup");
        object group = getGroup.Invoke(sizes, new[] { Enum.ToObject(getGroup.GetParameters()[0].ParameterType, 0) });
        Type sizeType = assembly.GetType("UnityEditor.GameViewSize"), kind = assembly.GetType("UnityEditor.GameViewSizeType");
        object size = Activator.CreateInstance(sizeType, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new object[] { Enum.ToObject(kind, 1), width, height, "Headland " + width + "x" + height }, null);
        int index = (int)group.GetType().GetMethod("GetTotalCount").Invoke(group, null);
        group.GetType().GetMethod("AddCustomSize").Invoke(group, new[] { size });
        Type viewType = assembly.GetType("UnityEditor.GameView");
        EditorWindow view = EditorWindow.GetWindow(viewType);
        viewType.GetProperty("selectedSizeIndex", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).SetValue(view, index);
        view.Show(); view.Focus(); view.Repaint();
#endif
    }
}
public sealed class RackV2HeadlandQualificationRunner : MonoBehaviour
{
    private static readonly string[] removed = { "PO0102", "PO0202", "PO0302", "PO0402", "PO0601", "PO0602", "PO0603", "PO0604", "PO0605", "PO0606", "PO0609", "PO0610", "PO0611", "PO0613", "PO0614", "PO0616", "PO0718", "PO0902", "PO1002", "PO1102", "PO1202", "PO1502", "PO1602", "PO1702", "PO1902", "PO2102", "PO2202", "PO2302", "PO2402", "PO2502", "PO2702", "PO2802", "PO2902", "PO3102", "PO3202", "PO3302", "PO3502", "PO3602", "PO3702", "PO3802", "PO3902", "PO4002" };
    private static readonly string[] added = { "PO0026", "PO0032", "PO0106", "PO0307", "PO0325", "PO0329", "PO0533", "PO0707", "PO0816", "PO0836", "PO0911", "PO0920", "PO1407", "PO1435", "PO1507", "PO1539", "PO1606", "PO1607", "PO1634", "PO1703", "PO1734", "PO1739", "PO1938", "PO2001", "PO2005", "PO2038", "PO2205", "PO2508", "PO2535", "PO2540", "PO2935", "PO3321", "PO3414", "PO3501", "PO3535", "PO3601", "PO3638", "PO3716", "PO3719", "PO3912", "PO3924", "PO4001" };
    private static readonly string[] entrance = { "PO0203", "PO1204", "PO2003", "PO2803", "PO3803" };
    private static readonly float[] entranceZ = { -35.403477f, -16.015210f, 0.014705888f, 16.022398f, 35.436646f };
    private int checks;
    private void Check(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); checks++; Debug.Log("HEADLAND_CHECK " + message); }
    private static List<ForestTree> Living() => FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).Where(t => t.IsLiving).ToList();
    private static HashSet<string> Ids() => new HashSet<string>(Living().Select(t => t.TreeId));
    private IEnumerator Start()
    {
        for (int i = 0; i < 5; i++) yield return null;
        Exception failure = null; var stack = new Stack<IEnumerator>(); stack.Push(Run());
        while (stack.Count > 0) { bool more; object current = null; try { more = stack.Peek().MoveNext(); if (more) current = stack.Peek().Current; } catch (Exception error) { failure = error; break; } if (!more) { stack.Pop(); continue; } if (current is IEnumerator nested) stack.Push(nested); else yield return current; }
        Debug.Log(failure == null ? "HEADLAND_QUALIFICATION_PASS checks=" + checks : "HEADLAND_QUALIFICATION_FAIL checks=" + checks + " " + failure);
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode(); EditorApplication.Exit(failure == null ? 0 : 1);
#endif
    }
    private IEnumerator Capture(string name)
    {
        yield return new WaitForEndOfFrame();
        Texture2D tex = ScreenCapture.CaptureScreenshotAsTexture();
        string output = Environment.GetEnvironmentVariable("CCF_ACCEPTANCE_OUTPUT");
        File.WriteAllBytes(Path.Combine(output, name + ".png"), tex.EncodeToPNG());
        Destroy(tex);
    }
    private IEnumerator Run()
    {
        var manager = FindFirstObjectByType<ScenarioOneManager>(); var ecology = FindFirstObjectByType<ForestEcologyController>(); var stand = FindFirstObjectByType<ForestStartingStand>();
        var saves = FindFirstObjectByType<ForestSaveController>(); var player = FindFirstObjectByType<ForestPlayer>(); var ui = FindFirstObjectByType<ScenarioOneUiRoot>();
        Check(manager != null && ecology != null && stand != null && saves != null && player != null && ui != null, "scene components exist");
        Vector3 initialPlayerPosition = player.transform.position;
        Check(manager.RackPlanningV2Active && manager.RackPlanningModelVersion == 1 && ecology.StandGeometryModelVersion == 1, "fresh model 1 identity");
        var ids = Ids(); Check(ids.Count == 1344, "fresh model 1 has 1344 living stems");
        Check(removed.All(id => !ids.Contains(id)), "42 qualified baseline stems absent");
        Check(added.All(id => ids.Contains(id)), "42 qualified replacement stems present");
        Check(Living().All(t => !ScenarioOneHeadlandGeometry.ContainsStemCentre(t.transform.position)), "all starting stems clear guarded route");
        var surface = GameObject.Find("Scenario One V2 Headland");
        Check(surface != null && surface.GetComponent<MeshRenderer>() != null && surface.GetComponent<Collider>() == null, "model 1 route surface exists without collider");
        for (int i = 0; i < entrance.Length; i++) { var tree = Living().SingleOrDefault(t => t.TreeId == entrance[i]); Check(tree != null && Vector2.Distance(new Vector2(tree.transform.position.x, tree.transform.position.z), new Vector2(-35.4f, entranceZ[i])) < 8f, "rack entrance " + entrance[i] + " reachable from headland"); }
        // Detect colliders in the player's standing envelope along the path and headland.
        for (int part = 0; part < 2; part++) for (int i = 0; i <= (part == 0 ? 33 : 70); i++)
        {
            float x = part == 0 ? -2.436166f + (-35.4f + 2.436166f) * i / 33f : -35.4f;
            float z = part == 0 ? -27.4f : -35.0f + 70f * i / 70f;
            var blockers = Physics.OverlapCapsule(new Vector3(x, 0.45f, z), new Vector3(x, 1.45f, z), 0.27f, ~0, QueryTriggerInteraction.Ignore)
                .Where(c => c != null && !c.transform.IsChildOf(player.transform)).ToArray();
            Check(blockers.Length == 0, "route collider clearance " + part + "/" + i + (blockers.Length == 0 ? "" : " " + blockers[0].name));
        }
        for (int pass = 0; pass < 2; pass++)
        {
            int w = pass == 0 ? 1280 : 1920, h = pass == 0 ? 720 : 1080; RackV2HeadlandQualification.Size(w,h);
            for(int k=0;k<20;k++){ui.CloseHelp(); yield return null;}
            float py = player.transform.position.y;
            player.transform.position = new Vector3(-5f,py,-27.4f); player.LookToward(new Vector3(-25f,1.3f,-27.4f)); for(int k=0;k<12;k++)yield return null; yield return Capture("feeder-"+w);
            player.transform.position = new Vector3(-35.4f,py,-24f); player.LookToward(new Vector3(-35.4f,1.3f,5f)); for(int k=0;k<12;k++)yield return null; yield return Capture("headland-"+w);
        }
        var saved = saves.CaptureData(); Check(saves.LoadData(saved,false), "model 1 save load accepted"); for(int i=0;i<3;i++)yield return null;
        Check(manager.RackPlanningV2Active && Ids().SetEquals(ids) && GameObject.Find("Scenario One V2 Headland") != null, "model 1 load retains IDs and presentation");
        // Re-generate under explicit model 0, before capturing a v20-style legacy fixture.
        player.transform.position = initialPlayerPosition;
        manager.InitializeNewScenario(); foreach(var t in FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None)) DestroyImmediate(t.gameObject);
        ecology.ApplyStandGeometry(StandGeometryModel.Enlarged80); ecology.ResetForDeterministicRun(); stand.Generate(); for(int i=0;i<3;i++)yield return null;
        var oldIds=Ids(); Check(!manager.RackPlanningV2Active && oldIds.Count==1344 && removed.All(oldIds.Contains) && added.All(id=>!oldIds.Contains(id)), "fresh model 0 historical omission set");
        Check(GameObject.Find("Scenario One V2 Headland") == null, "model 0 has no V2 surface");
        var legacy=saves.CaptureData(); legacy.version=20; Check(saves.LoadData(legacy,false), "v20 legacy save load accepted"); for(int i=0;i<3;i++)yield return null;
        Check(!manager.RackPlanningV2Active && Ids().SetEquals(oldIds) && GameObject.Find("Scenario One V2 Headland") == null, "v20 legacy load remains historical");
        var resaved=saves.CaptureData(); Check(resaved.scenarioOne.rackPlanningModel == 0, "legacy re-save stays model 0");
    }
}
