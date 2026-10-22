using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

// Disposable live gate for the forest track's approach (StandApproachPath / StandApproachRoute / StandWorldBoundary.ShapeSouthRidge):
// SCENARIO-ONE-PLAYER-TEST-NON-UI-REMEDIATION-V1, item 1. The new-game world is Enlarged80; this checks the planks, the opening in the
// south ridge, the cutting, the starting stand around the track, the Stand Map's path, and that Legacy40 is left exactly as authored.
// Copy into Assets/ForestPrototype with run_unity_harness.py (it stages and removes the copy and its .meta), run
// StandApproachVerification.Begin in a graphical Editor under the Unity lock. It never writes the player's save or the scene.
//
// Prints APPROACH_CHECK <what> per check, then APPROACH_PASS checks=N or APPROACH_FAIL <reason>.
public static class StandApproachVerification
{
#if UNITY_EDITOR
    public static void Begin()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        EditorApplication.isPlaying = true;
    }
#endif

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (Environment.GetCommandLineArgs().Any(a => a == "StandApproachVerification.Begin"))
            new GameObject("Disposable stand approach verification").AddComponent<StandApproachVerificationRunner>();
    }
}

public sealed class StandApproachVerificationRunner : MonoBehaviour
{
    private const float Corridor = 1.5f;
    private ForestEcologyController ecology;
    private ScenarioOneUiRoot ui;
    private int checks;

    private static string F(double v) => v.ToString("0.###", CultureInfo.InvariantCulture);

    private void Check(bool ok, string what, string detail = "")
    {
        if (!ok) throw new InvalidOperationException(what + (detail.Length > 0 ? " [" + detail + "]" : ""));
        checks++;
        Debug.Log("APPROACH_CHECK " + what + (detail.Length > 0 ? " [" + detail + "]" : ""));
    }

    private IEnumerator Start()
    {
        for (int i = 0; i < 8; i++) yield return null;
        Exception failure = null;
        var stack = new Stack<IEnumerator>();
        stack.Push(Run());
        while (stack.Count > 0)
        {
            bool more; object current = null;
            try { more = stack.Peek().MoveNext(); if (more) current = stack.Peek().Current; }
            catch (Exception error) { failure = error; break; }
            if (!more) { stack.Pop(); continue; }
            if (current is IEnumerator nested) stack.Push(nested); else yield return current;
        }
        Debug.Log(failure == null ? "APPROACH_PASS checks=" + checks : "APPROACH_FAIL " + failure);
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(failure == null ? 0 : 1);
#endif
    }

    private static Transform[] AllTransforms() => FindObjectsByType<Transform>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
    private static float Planar(Vector3 a, Vector3 b) => Vector2.Distance(new Vector2(a.x, a.z), new Vector2(b.x, b.z));

    private IEnumerator Run()
    {
        ecology = FindFirstObjectByType<ForestEcologyController>();
        ui = FindFirstObjectByType<ScenarioOneUiRoot>();
        ForestPlayer player = FindFirstObjectByType<ForestPlayer>();
        Check(ecology != null && ui != null && player != null, "the scene has its ecology, UI and player");
        Check(ecology.StandGeometryModelVersion == StandGeometryModel.Enlarged80, "a new game starts in the Enlarged80 world");

        // ---- the planks ----------------------------------------------------------------------------------------------------------
        Transform[] all = AllTransforms();
        var authored = all.Where(t => Regex.IsMatch(t.name, @"^Dirt Path \d\d$")).OrderBy(t => t.name, StringComparer.Ordinal).ToList();
        var inside = all.Where(t => t.name.StartsWith("Dirt Path X", StringComparison.Ordinal)).OrderBy(t => t.name, StringComparer.Ordinal).ToList();
        var outside = all.Where(t => t.name.StartsWith("Approach Track ", StringComparison.Ordinal)).OrderBy(t => t.name, StringComparer.Ordinal).ToList();
        Check(authored.Count == 52, "the 52 authored track planks are all still there", "n=" + authored.Count);
        Check(inside.Count >= 40 && outside.Count >= 10, "the extension has planks inside the property and in the cutting", "inside " + inside.Count + ", outside " + outside.Count);
        Transform plank00 = authored[0];
        Check(Planar(plank00.position, inside[0].position) < 0.01f, "the extension starts exactly where the authored track ends");
        Check(inside[inside.Count - 1].position.z >= -40.0f && inside[inside.Count - 1].position.z <= -39.4f, "the inside planks run right up to the south property edge", "z " + F(inside[inside.Count - 1].position.z));
        Check(outside.All(t => t.position.z < -40f), "the cutting planks all lie beyond the edge");
        Check(inside.Concat(outside).All(t => t.GetComponent<Collider>() == null), "the planks have no colliders (the track is not a wall or a ramp)");
        Material trackMaterial = plank00.GetComponent<Renderer>().sharedMaterial;
        Check(inside.Concat(outside).All(t => t.GetComponent<Renderer>().sharedMaterial == trackMaterial), "the extension uses the authored track's material");
        Check(inside.Concat(outside).All(t => Mathf.Approximately(t.lossyScale.x, plank00.lossyScale.x)), "the extension is as wide as the authored track");
        Check(Mathf.Abs(player.transform.position.x - -0.43f) < 0.05f && Mathf.Abs(player.transform.position.z - 5.25f) < 0.05f
            && authored.Any(t => Planar(t.position, player.transform.position) < 2f), "the player still starts on the authored track");

        // ---- the starting stand around the track ------------------------------------------------------------------------------------------
        List<ForestTree> trees = FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).Where(t => t.IsLiving).ToList();
        Check(trees.Count == 1344, "the stand still has 1,344 living trees", "n=" + trees.Count);
        var every = all.Where(t => t.name.StartsWith("Dirt Path", StringComparison.Ordinal)).ToList();
        Check(trees.All(t => every.All(p => Planar(t.transform.position, p.position) > Corridor)), "no tree stands within the corridor of any track plank, old or new");
        var legacyId = new Regex(@"^P\d{4}$"); var outerId = new Regex(@"^PO\d{4}$");
        List<string> core = trees.Select(t => t.TreeId).Where(id => legacyId.IsMatch(id)).ToList();
        Check(core.Count == 336 && trees.Count(t => outerId.IsMatch(t.TreeId)) == 1008, "336 core + 1,008 outer trees");
        Check(!core.Contains("P0009") && core.Contains("P1200"), "the core differs from Legacy40 only by the tree the track clears and the one the mortality budget restores", "P0009 out, P1200 in");

        // ---- the opening in the south ridge and the cutting beyond it ----------------------------------------------------------------------------
        GameObject west = GameObject.Find("South Ridge"), east = GameObject.Find("South Ridge (east of gate)");
        Check(west != null && east != null, "the south ridge is in two pieces");
        float westMax = west.transform.position.x + west.transform.localScale.x * 0.5f, westMin = west.transform.position.x - west.transform.localScale.x * 0.5f;
        float eastMin = east.transform.position.x - east.transform.localScale.x * 0.5f, eastMax = east.transform.position.x + east.transform.localScale.x * 0.5f;
        Check(Mathf.Abs(westMin - -41f) < 0.01f && Mathf.Abs(eastMax - 41f) < 0.01f, "the two pieces still reach the corners");
        Check(Mathf.Abs((eastMin - westMax) - StandApproachRoute.GateWidthM) < 0.01f, "the opening is the stated width", "gap " + F(eastMin - westMax));
        float gateX = (westMax + eastMin) * 0.5f;
        Check(Mathf.Abs(gateX - inside[inside.Count - 1].position.x) < 0.5f, "the opening is where the track meets the edge", "gate x " + F(gateX));
        Check(Mathf.Approximately(west.transform.position.y, east.transform.position.y) && Mathf.Approximately(west.transform.localScale.y, east.transform.localScale.y)
            && Mathf.Approximately(west.transform.localScale.z, east.transform.localScale.z), "both pieces keep the ridge's height and thickness");
        Physics.SyncTransforms();
        Check(Physics.Raycast(new Vector3(gateX, 1f, -35f), Vector3.back, out RaycastHit through, 15f) && through.collider.name == "Approach Gate Blocker",
            "walking south through the opening is stopped by the invisible blocker");
        Check(Physics.Raycast(new Vector3(gateX + 6f, 1f, -35f), Vector3.back, out RaycastHit eastHit, 15f) && eastHit.collider.name == "South Ridge (east of gate)", "the east piece is solid");
        Check(Physics.Raycast(new Vector3(gateX - 6f, 1f, -35f), Vector3.back, out RaycastHit westHit, 15f) && westHit.collider.name == "South Ridge", "the west piece is solid");
        GameObject floor = GameObject.Find("Approach Ground");
        Check(floor != null && floor.GetComponent<Collider>() == null && Mathf.Abs(floor.transform.position.y + floor.transform.localScale.y * 0.5f) < 0.001f, "the cutting has a ground whose top is level with the property ground");
        foreach (string wall in new[] { "Approach Wall West", "Approach Wall East", "Approach Wall End" })
        {
            GameObject w = GameObject.Find(wall);
            Check(w != null && w.GetComponent<Collider>() != null && Mathf.Approximately(w.transform.localScale.y, west.transform.localScale.y), wall + " is a ridge-height solid wall");
        }
        Check(player.transform.position.z > -40f, "the player is inside the property");

        // ---- the Stand Map shows the same track -----------------------------------------------------------------------------------------------
        StandMapModel model = ui.MapSource.Model;
        Check(model.Paths.Count == 2, "the map draws the track as the authored line plus the extension", "paths " + model.Paths.Count);
        MapPath extension = model.Paths[1];
        Check(Vector2.Distance(new Vector2(extension.Points[0].X, extension.Points[0].Z), new Vector2(plank00.position.x, plank00.position.z)) < 0.05f, "the map's extension line starts at the authored end");
        Check(extension.Points[extension.Points.Count - 1].Z >= -40f && extension.Points[extension.Points.Count - 1].Z <= -39.4f, "the map's extension line reaches the property edge");
        Check(Mathf.Abs(extension.WidthM - model.Paths[0].WidthM) < 0.05f, "both map lines have the same width");

        // ---- Legacy40 is left exactly as authored, and the approach comes back identically -------------------------------------------------------------
        var before = inside.Concat(outside).ToDictionary(t => t.name, t => t.position);
        ecology.ApplyStandGeometry(StandGeometryModel.Legacy40);
        yield return null; yield return null;
        Check(GameObject.Find("Stand Approach") == null && AllTransforms().Count(t => t.name.StartsWith("Dirt Path X", StringComparison.Ordinal)) == 0, "Legacy40 has no extension planks");
        Check(AllTransforms().Count(t => Regex.IsMatch(t.name, @"^Dirt Path \d\d$")) == 52, "Legacy40 keeps the 52 authored planks");
        GameObject legacyRidge = GameObject.Find("South Ridge");
        Check(GameObject.Find("South Ridge (east of gate)") == null && Mathf.Approximately(legacyRidge.transform.localScale.x, 42f) && Mathf.Approximately(legacyRidge.transform.position.x, 0f)
            && Mathf.Approximately(legacyRidge.transform.position.z, -20f), "Legacy40 has the authored whole south ridge at 20 m");
        Check(GameObject.Find("Approach Ground") == null && GameObject.Find("Approach Gate Blocker") == null, "Legacy40 has no cutting or blocker");
        Check(ui.MapSource.Model.Paths.Count == 1, "the Legacy40 map draws only the authored line", "paths " + ui.MapSource.Model.Paths.Count);
        ecology.ApplyStandGeometry(StandGeometryModel.Enlarged80);
        yield return null; yield return null;
        var after = AllTransforms().Where(t => t.name.StartsWith("Dirt Path X", StringComparison.Ordinal) || t.name.StartsWith("Approach Track ", StringComparison.Ordinal)).ToDictionary(t => t.name, t => t.position);
        Check(after.Count == before.Count && before.All(kv => after.ContainsKey(kv.Key) && Vector3.Distance(after[kv.Key], kv.Value) < 0.001f), "going back to Enlarged80 rebuilds the very same track", "planks " + after.Count);
        Check(GameObject.Find("South Ridge (east of gate)") != null && ui.MapSource.Model.Paths.Count == 2, "the opening and the map's extension return too");
    }
}
