using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

// Disposable #876 marker-scale gate. Kept under Tools/Verification/Sen1MarkerScale; copy into
// Assets/ForestPrototype/ for one run, then remove it with its generated .meta. It writes no
// scene and calls no save API. Non-unit root scales it applies are play-mode only.
//
// SEN1_MARKER_MODE = compact | historical. historical opens Assets/Scenes/ForestTest.unity; pick
// Legacy40/Enlarged80 with CCF_STAND_GEOMETRY=0/1 and set CCF_TEST_DIRECT_ENTRY=1.
// SEN1_MARKER_OUT = directory for markers.csv and verdict.txt.
//
// What it proves: Crop (C) and Fell (X) mark visuals have world-metre sizes and heights on
// ~12x Compact Sitka roots, on unit roots and on a non-uniform root; marks follow a moved tree;
// unmarking destroys the visual; marks carry no collider; the mark state is the one requested.
// It does NOT prove: rendered look, player input, year advance, save/load or owner acceptance.
public static class Sen1MarkerScaleVerification
{
    public const string CompactScene = "Assets/ForestPrototype/Scenes/ScenarioOneCompact30.unity";
    public const string HistoricalScene = "Assets/Scenes/ForestTest.unity";

    public static string Mode => (Environment.GetEnvironmentVariable("SEN1_MARKER_MODE") ?? "compact").Trim();

    public static void Begin()
    {
        EditorSceneManager.OpenScene(Mode == "historical" ? HistoricalScene : CompactScene);
        EditorApplication.isPlaying = true;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (Environment.GetEnvironmentVariable("SEN1_MARKER_MODE") == null) return;
        if (UnityEngine.Object.FindFirstObjectByType<Sen1MarkerScaleRunner>() != null) return;
        new GameObject("Sen1 #876 marker scale verification").AddComponent<Sen1MarkerScaleRunner>();
    }
}

public sealed class Sen1MarkerScaleRunner : MonoBehaviour
{
    private readonly List<string> log = new List<string>();
    private readonly List<string> rows = new List<string>
    {
        "case,tree,type,rootScaleX,rootScaleY,rootScaleZ,height,markerLossyX,markerLossyY,markerLossyZ,discWidthX,discWidthZ,discThick,discY,diamondY,diamondExpectedY,diamondSize,bandY,bandExpectedY,bandWidth,bandExpectedWidth,visualTopY,diamondMinusTop,colliders"
    };
    private int checks, failures;

    private void Check(bool condition, string message)
    {
        checks++;
        if (condition) log.Add("PASS " + message);
        else { failures++; log.Add("FAIL " + message); }
    }

    private IEnumerator Start()
    {
        Exception crash = null;
        IEnumerator test = Test();
        while (true)
        {
            bool more = false;
            object current = null;
            try { more = test.MoveNext(); if (more) current = test.Current; }
            catch (Exception exception) { crash = exception; }
            if (crash != null || !more) break;
            yield return current;
        }
        if (crash != null) { failures++; log.Add("FAIL crash " + crash); }
        string verdict = (failures == 0 ? "SEN1_MARKER_VERIFY_PASS" : "SEN1_MARKER_VERIFY_FAIL")
            + " mode=" + Sen1MarkerScaleVerification.Mode
            + " geometry=" + (Environment.GetEnvironmentVariable("CCF_STAND_GEOMETRY") ?? "default")
            + " checks=" + checks + " failures=" + failures;
        foreach (string line in log) Debug.Log("SEN1_MARKER " + line);
        Debug.Log(verdict);
        string outDir = Environment.GetEnvironmentVariable("SEN1_MARKER_OUT");
        if (!string.IsNullOrEmpty(outDir))
        {
            Directory.CreateDirectory(outDir);
            File.WriteAllLines(Path.Combine(outDir, "markers.csv"), rows);
            File.WriteAllLines(Path.Combine(outDir, "verdict.txt"), log.Append(verdict));
        }
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(failures == 0 ? 0 : 1);
    }

    private IEnumerator Test()
    {
        ForestTreeMarkingManager marking = null;
        ForestTree[] trees = new ForestTree[0];
        for (int frame = 0; frame < 600; frame++)
        {
            yield return null;
            marking = UnityEngine.Object.FindFirstObjectByType<ForestTreeMarkingManager>();
            trees = UnityEngine.Object.FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
                .Where(t => t != null && t.IsLiving && !string.IsNullOrEmpty(t.TreeId)).OrderBy(t => t.TreeId).ToArray();
            if (marking != null && trees.Length >= 8 && frame > 10) break;
        }
        Check(marking != null, "tree marking manager present");
        Check(trees.Length >= 8, "at least 8 living trees present (found " + trees.Length + ")");
        if (marking == null || trees.Length < 8) yield break;
        Check(!trees.Any(t => t.MarkType != TreeMarkType.None), "no marks at start");

        if (Sen1MarkerScaleVerification.Mode == "historical") yield return Historical(marking, trees);
        else yield return Compact(marking, trees);

        // Cleanup: every mark is removed and no mark visual survives.
        foreach (ForestTree tree in trees) if (tree.MarkType != TreeMarkType.None) marking.Unmark(tree, false);
        yield return null;
        Check(CountMarkVisuals() == 0, "no Crop/Harvest mark visual left after unmarking everything");
    }

    private IEnumerator Compact(ForestTreeMarkingManager marking, ForestTree[] trees)
    {
        ForestTree[] sitka = trees.Where(t => t.Species != null && t.Species.SpeciesId == "sitka-spruce"
            && t.transform.lossyScale.y > 5f).ToArray();
        Check(sitka.Length >= 9, "at least 9 scaled (>5x) living Sitka roots (found " + sitka.Length + ")");
        if (sitka.Length < 9) yield break;
        ScenarioOneManager manager = UnityEngine.Object.FindFirstObjectByType<ScenarioOneManager>();

        // C on three scaled Sitka: the actual F-1 reproduction.
        for (int i = 0; i < 3; i++) marking.Mark(sitka[i], TreeMarkType.CropTree);
        yield return null;
        for (int i = 0; i < 3; i++) Measure("compact-crop", sitka[i], TreeMarkType.CropTree);
        if (manager != null) Check(manager.CropTreeCount == 3, "Crop Tree count is 3 (" + manager.CropTreeCount + ")");

        // X on two other scaled Sitka.
        for (int i = 3; i < 5; i++) marking.Mark(sitka[i], TreeMarkType.Fell);
        yield return null;
        for (int i = 3; i < 5; i++)
        {
            Measure("compact-fell", sitka[i], TreeMarkType.Fell);
            Check(sitka[i].IsMarkedForFell, sitka[i].TreeId + " is marked for felling");
        }
        Check(CountMarkVisuals() == 5, "exactly 5 mark visuals for 5 marks (" + CountMarkVisuals() + ")");
        if (Environment.GetEnvironmentVariable("SEN1_MARKER_CAPTURE") == "1")
        {
            Capture(sitka[0], 9f, 1.7f, 4f, "crop-close");
            Capture(sitka[0], 30f, 12f, 6f, "crop-wide");
            Capture(sitka[3], 9f, 1.7f, 4f, "fell-close");
        }

        // Repeated unmark / re-mark keeps one visual and the right state.
        for (int round = 0; round < 3; round++)
        {
            marking.Unmark(sitka[0], false);
            yield return null;
            Check(sitka[0].MarkType == TreeMarkType.None && MarkVisual(sitka[0]) == null,
                "round " + round + ": unmark clears state and destroys visual");
            marking.Mark(sitka[0], TreeMarkType.CropTree, false);
            yield return null;
            Check(sitka[0].IsCropTree && MarkVisualCount(sitka[0]) == 1,
                "round " + round + ": re-mark restores exactly one visual");
        }
        Measure("compact-recrop", sitka[0], TreeMarkType.CropTree);

        // A mark follows its tree if the tree moves.
        Vector3 start = sitka[1].transform.position;
        sitka[1].transform.position = start + new Vector3(2f, 0f, 0f);
        yield return null;
        Transform disc = MarkVisual(sitka[1])?.Find("Disc");
        Check(disc != null && Mathf.Abs(disc.position.x - (start.x + 2f)) < 0.01f, "Crop mark follows a moved tree");
        sitka[1].transform.position = start;

        // Unit-scaled and non-uniform roots (play-mode only; scale restored afterwards).
        yield return WithScale(marking, sitka[5], Vector3.one, TreeMarkType.CropTree, "unit-crop");
        yield return WithScale(marking, sitka[6], Vector3.one, TreeMarkType.Fell, "unit-fell");
        yield return WithScale(marking, sitka[7], new Vector3(6f, 14f, 9f), TreeMarkType.CropTree, "nonuniform-crop");
        yield return WithScale(marking, sitka[8], new Vector3(6f, 14f, 9f), TreeMarkType.Fell, "nonuniform-fell");
    }

    private IEnumerator Historical(ForestTreeMarkingManager marking, ForestTree[] trees)
    {
        ForestEcologyController ecology = UnityEngine.Object.FindFirstObjectByType<ForestEcologyController>();
        if (ecology != null) log.Add("INFO geometry model " + ecology.StandGeometryModelVersion);
        ForestTree[] unit = trees.Where(t => (t.transform.lossyScale - Vector3.one).sqrMagnitude < 1e-6f).ToArray();
        Check(unit.Length == trees.Length, "historical tree roots are all unit-scaled (" + unit.Length + "/" + trees.Length + ")");
        if (unit.Length < 4) yield break;
        ForestTree crop = unit.OrderByDescending(t => t.Height).First();
        ForestTree fell = unit.OrderByDescending(t => t.Height).Skip(1).First();
        marking.Mark(crop, TreeMarkType.CropTree, false);
        marking.Mark(fell, TreeMarkType.Fell, false);
        yield return null;
        Measure("historical-crop", crop, TreeMarkType.CropTree);
        Measure("historical-fell", fell, TreeMarkType.Fell);
        // On a unit root the fix sets local scale (1,1,1): identical to the pre-fix transform.
        Check(MarkVisual(crop).localScale == Vector3.one && MarkVisual(fell).localScale == Vector3.one,
            "historical mark local scale is exactly (1,1,1), as before the fix");
    }

    private IEnumerator WithScale(ForestTreeMarkingManager marking, ForestTree tree, Vector3 scale, TreeMarkType type, string label)
    {
        Vector3 original = tree.transform.localScale;
        tree.transform.localScale = scale;
        marking.Mark(tree, type, false);
        yield return null;
        Measure(label, tree, type);
        marking.Unmark(tree, false);
        yield return null;
        Check(MarkVisual(tree) == null, label + ": unmark destroys the visual");
        tree.transform.localScale = original;
    }

    private void Measure(string label, ForestTree tree, TreeMarkType type)
    {
        Transform marker = MarkVisual(tree);
        string id = label + " " + tree.TreeId;
        Check(marker != null && marker.parent == tree.transform, id + ": mark visual exists under its tree");
        if (marker == null) return;
        Check(tree.MarkType == type, id + ": mark state is " + type);
        float ground = tree.transform.position.y;
        Vector3 lossy = marker.lossyScale;
        Bounds disc = Bounds(marker, "Disc");
        Bounds diamond = Bounds(marker, "Diamond");
        Transform bandT = marker.Find("Blue Trunk Band");
        Bounds band = bandT != null ? Bounds(marker, "Blue Trunk Band") : new Bounds();
        float expectDiamond = ground + tree.Height + 0.6f;
        float expectBand = ground + Mathf.Min(1.6f, tree.Height * 0.5f);
        float expectBandWidth = Mathf.Max(0.25f, tree.Diameter / 100f + 0.08f);
        float top = VisualTop(tree, marker);
        int colliders = marker.GetComponentsInChildren<Collider>(true).Length;

        Check(Mathf.Abs(lossy.x - 1f) < 0.01f && Mathf.Abs(lossy.y - 1f) < 0.01f && Mathf.Abs(lossy.z - 1f) < 0.01f,
            id + ": mark world scale is 1 (" + lossy.ToString("F3") + ")");
        Check(Mathf.Abs(disc.size.x - 1.2f) < 0.1f && Mathf.Abs(disc.size.z - 1.2f) < 0.1f && disc.size.y < 0.1f,
            id + ": ground disc is 1.2 m wide (" + disc.size.ToString("F3") + ")");
        Check(Mathf.Abs(disc.center.y - (ground + 0.05f)) < 0.05f, id + ": disc sits on the ground");
        Check(Mathf.Abs(diamond.center.y - expectDiamond) < 0.05f && diamond.size.y < 0.8f,
            id + ": diamond is small and 0.6 m above tree height (" + diamond.center.y.ToString("F2") + " vs " + expectDiamond.ToString("F2") + ")");
        Check(diamond.center.y > ground + 0.5f, id + ": diamond is above ground");
        if (type == TreeMarkType.CropTree)
            Check(bandT != null && Mathf.Abs(band.center.y - expectBand) < 0.05f && Mathf.Abs(band.size.x - expectBandWidth) < 0.05f,
                id + ": blue trunk band at chest height with trunk width");
        else
            Check(bandT == null, id + ": Fell mark has no trunk band");
        Check(colliders == 0, id + ": mark has no colliders");

        rows.Add(string.Join(",", new[]
        {
            label, tree.TreeId, type.ToString(),
            F(tree.transform.lossyScale.x), F(tree.transform.lossyScale.y), F(tree.transform.lossyScale.z), F(tree.Height),
            F(lossy.x), F(lossy.y), F(lossy.z), F(disc.size.x), F(disc.size.z), F(disc.size.y), F(disc.center.y - ground),
            F(diamond.center.y - ground), F(expectDiamond - ground), F(diamond.size.y),
            bandT != null ? F(band.center.y - ground) : "", type == TreeMarkType.CropTree ? F(expectBand - ground) : "",
            bandT != null ? F(band.size.x) : "", type == TreeMarkType.CropTree ? F(expectBandWidth) : "",
            F(top - ground), F(diamond.center.y - top), colliders.ToString()
        }));
    }

    // Optional evidence image: a temporary camera placed `distance` m from the tree on the side facing
    // the stand centre's opposite (open edge), eye height `eye`, looking at `lookHeight` on the trunk.
    private void Capture(ForestTree tree, float distance, float eye, float lookHeight, string name)
    {
        string outDir = Environment.GetEnvironmentVariable("SEN1_MARKER_OUT");
        if (string.IsNullOrEmpty(outDir)) return;
        Vector3 centre = Vector3.zero;
        ForestTree[] all = UnityEngine.Object.FindObjectsByType<ForestTree>(FindObjectsSortMode.None);
        foreach (ForestTree t in all) centre += t.transform.position;
        centre /= Mathf.Max(1, all.Length);
        Vector3 outward = tree.transform.position - centre; outward.y = 0f;
        if (outward.sqrMagnitude < 0.01f) outward = Vector3.back;
        Vector3 basePos = tree.transform.position;
        var go = new GameObject("Sen1 #876 capture camera");
        Camera cam = go.AddComponent<Camera>();
        cam.fieldOfView = 60f; cam.nearClipPlane = 0.1f; cam.farClipPlane = 600f;
        go.transform.position = basePos + outward.normalized * distance + Vector3.up * eye;
        go.transform.LookAt(basePos + Vector3.up * lookHeight);
        var rt = new RenderTexture(1280, 720, 24);
        cam.targetTexture = rt;
        cam.Render();
        RenderTexture.active = rt;
        var tex = new Texture2D(1280, 720, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, 1280, 720), 0, 0);
        tex.Apply();
        File.WriteAllBytes(Path.Combine(outDir, name + ".png"), tex.EncodeToPNG());
        RenderTexture.active = null;
        cam.targetTexture = null;
        Destroy(rt); Destroy(tex); Destroy(go);
        log.Add("INFO captured " + name + " for " + tree.TreeId);
    }

    private static string F(float value) => value.ToString("F3", System.Globalization.CultureInfo.InvariantCulture);

    private static Bounds Bounds(Transform marker, string child)
    {
        Transform part = marker.Find(child);
        return part != null && part.TryGetComponent(out Renderer renderer) ? renderer.bounds : new Bounds(Vector3.one * -999f, Vector3.zero);
    }

    // Highest rendered point of the tree itself (mark visual excluded), for the diamond-to-crown report.
    private static float VisualTop(ForestTree tree, Transform marker)
    {
        float top = float.NegativeInfinity;
        foreach (Renderer renderer in tree.GetComponentsInChildren<Renderer>())
            if (!renderer.transform.IsChildOf(marker) && renderer.enabled && renderer.gameObject.activeInHierarchy)
                top = Mathf.Max(top, renderer.bounds.max.y);
        return float.IsInfinity(top) ? tree.transform.position.y : top;
    }

    private static Transform MarkVisual(ForestTree tree)
    {
        foreach (Transform child in tree.transform)
            if (child.name == "Crop Tree Mark" || child.name == "Harvest Mark") return child;
        return null;
    }

    private static int MarkVisualCount(ForestTree tree)
    {
        int count = 0;
        foreach (Transform child in tree.transform)
            if (child.name == "Crop Tree Mark" || child.name == "Harvest Mark") count++;
        return count;
    }

    private static int CountMarkVisuals() =>
        UnityEngine.Object.FindObjectsByType<Transform>(FindObjectsSortMode.None)
            .Count(t => t.name == "Crop Tree Mark" || t.name == "Harvest Mark");
}
