using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

// Disposable API-only gate. Copy into Assets/ForestPrototype, run Begin, remove it and its .meta.
// All fixtures are in memory. No Save(), scene save, archive write or annual ecological advance.
public static class SuggestedLayoutVerification
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
        if (Environment.GetCommandLineArgs().Contains("SuggestedLayoutVerification.Begin"))
            new GameObject("Disposable suggested-layout API gate").AddComponent<SuggestedLayoutVerificationRunner>();
    }
}

public sealed class SuggestedLayoutVerificationRunner : MonoBehaviour
{
    private int checks;
    private ScenarioOneManager manager;
    private ForestEcologyController ecology;
    private ForestSaveController saves;

    private void Check(bool ok, string message)
    {
        if (!ok) throw new InvalidOperationException(message);
        checks++;
        Debug.Log("SUGGESTED_LAYOUT_CHECK " + message);
    }

    private static string Hash(byte[] bytes)
    {
        using (SHA256 hash = SHA256.Create())
            return string.Concat(hash.ComputeHash(bytes).Select(b => b.ToString("x2")));
    }

    private static List<RackTree> Trees()
        => UnityEngine.Object.FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .Where(t => t != null).Select(t => new RackTree(t.TreeId, t.transform.position.x, t.transform.position.z,
                t.CanChop, t.IsCropTree, t.IsMarkedForFell)).ToList();

    private IReadOnlyList<SuggestedRackSegment> Read(IReadOnlyList<RackTree> trees)
        => ScenarioOneSuggestedRackLayout.Get(manager.RackPlanningModelVersion, ecology.StandGeometryModelVersion, trees);

    private void CheckUnchangedReads(string fixture, int expectedCount)
    {
        // No frame boundary between snapshots: unrelated animation timers cannot change underneath the comparison.
        string before = JsonUtility.ToJson(saves.CaptureData());
        string worldBefore = ScenarioReferenceArchive.CurrentWorldHash(saves.CaptureData());
        string counters = manager.PlannedRackCount + "/" + manager.MarkedRackEntranceCount + "/" + manager.NextRackId;
        var first = Read(Trees());
        Check(first.Count == expectedCount, fixture + " suggestion count " + expectedCount);
        for (int read = 0; read < 100; read++)
        {
            List<RackTree> trees = Trees();
            if (read % 2 == 0) trees.Reverse();
            if (!first.SequenceEqual(Read(trees))) throw new InvalidOperationException(fixture + " nondeterministic read " + read);
        }
        Check(true, fixture + " 100 repeated/reordered reads identical");
        string after = JsonUtility.ToJson(saves.CaptureData());
        Check(before == after, fixture + " full serialized save identical: rack IDs, geometry, marks, confirmation, work-order IDs/status and world");
        Check(worldBefore == ScenarioReferenceArchive.CurrentWorldHash(saves.CaptureData()), fixture + " world hash unchanged");
        Check(counters == manager.PlannedRackCount + "/" + manager.MarkedRackEntranceCount + "/" + manager.NextRackId,
            fixture + " counters and ID allocator unchanged");
        Debug.Log("SUGGESTED_LAYOUT_STATE fixture=" + fixture + " before=" + Hash(System.Text.Encoding.UTF8.GetBytes(before))
            + " after=" + Hash(System.Text.Encoding.UTF8.GetBytes(after)) + " world=" + worldBefore + " counters=" + counters
            + " confirmed=" + manager.RackPlanConfirmed + " orders=" + manager.WorkOrders.Count);
    }

    private IEnumerator Start()
    {
        for (int i = 0; i < 6; i++) yield return null;
        Exception failure = null;
        var stack = new Stack<IEnumerator>(); stack.Push(Run());
        while (stack.Count > 0)
        {
            bool more; object current = null;
            try { more = stack.Peek().MoveNext(); if (more) current = stack.Peek().Current; }
            catch (Exception error) { failure = error; break; }
            if (!more) { stack.Pop(); continue; }
            if (current is IEnumerator nested) stack.Push(nested); else yield return current;
        }
        Debug.Log(failure == null ? "SUGGESTED_LAYOUT_UNITY_PASS checks=" + checks : "SUGGESTED_LAYOUT_UNITY_FAIL " + failure);
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(failure == null ? 0 : 1);
#endif
    }

    private IEnumerator Run()
    {
        manager = FindFirstObjectByType<ScenarioOneManager>();
        ecology = FindFirstObjectByType<ForestEcologyController>();
        saves = FindFirstObjectByType<ForestSaveController>();
        string config = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME");
        Check(!string.IsNullOrEmpty(config) && Path.GetFullPath(Application.persistentDataPath)
            .StartsWith(Path.GetFullPath(config) + Path.DirectorySeparatorChar, StringComparison.Ordinal), "isolated persistent data path");
        Check(manager != null && ecology != null && saves != null && manager.RackPlanningV2Active, "fresh V2 scene ready");
        Check(manager.PlannedRackCount == 0 && manager.NextRackId == 1 && !manager.RackPlanConfirmed, "empty player plan before guidance");

        // Independent qualification oracle: do not derive these IDs from the API under test.
        string[] ids = { "PO0203", "PO1204", "PO2003", "PO2803", "PO3803" };
        List<RackTree> live = Trees();
        var segments = Read(live);
        Check(segments.Count == 5, "five fresh V2 segments");
        var planner = new RackPlanner(manager, ecology);
        for (int i = 0; i < ids.Length; i++)
        {
            RackTree entrance = live.Single(t => t.Id == ids[i]);
            var expectedEntrance = new RackPoint(ScenarioOneHeadlandGeometry.HeadlandX, entrance.Z);
            var expectedFar = new RackPoint(planner.HalfExtentM, entrance.Z);
            Check(RackGeometry.SameLine(segments[i].Entrance, segments[i].Far, expectedEntrance, expectedFar), "qualified route endpoints " + ids[i]);
            Check(segments[i].WidthM == RackGeometry.DefaultWidthM, "qualified route width " + ids[i]);
            Debug.Log("SUGGESTED_LAYOUT_ROUTE id=" + ids[i] + " entrance=" + segments[i].Entrance + " far=" + segments[i].Far + " width=" + segments[i].WidthM);
        }
        CheckUnchangedReads("fresh-v2", 5);

        // Build a non-empty authoritative fixture explicitly OUTSIDE the getter. Its linked orders/mark
        // make a stronger non-mutation control than checking only the fresh empty collections.
        foreach (var segment in segments)
            Check(manager.AddRack(segment.Entrance, segment.Far).Succeeded, "fixture player rack added");
        Check(manager.ConfirmRackPlan().Succeeded && manager.WorkOrders.Any(o => o.rackId > 0), "fixture confirmed with rack work");
        Check(manager.MarkRackEntrance(manager.RackPlan[0].Id).Succeeded, "fixture contains a marked entrance");
        CheckUnchangedReads("confirmed-marked-v2", 5);

        // The production reset supplies the historical rack identity without changing any geometry policy.
        manager.InitializeNewScenario();
        Check(manager.RackPlanningModelVersion == RackPlanningModel.Legacy
            && ecology.StandGeometryModelVersion == StandGeometryModel.Enlarged80, "model-0 Enlarged80 identity fixture");
        CheckUnchangedReads("model0-enlarged80", 0);

        string archivePath = "Assets/ForestPrototype/ScenarioOne/Resources/ScenarioOneReferenceFutureV1.bytes";
        string archiveBefore = Hash(File.ReadAllBytes(archivePath));
        ScenarioReferenceArchive archive = ScenarioReferenceArchive.Load();
        Check(archive != null && archive.milestones.All(m => m.verifiedFrozenWorld), "original embedded reference worlds verified by production loader");
        Check(archive.AtYear(0).worldHash == "A564039D9B7CE31D" && archive.AtYear(20).worldHash == "F7DF7DAB53B6FD32"
            && archive.AtYear(50).worldHash == "D5E75D6D21D631AC" && archive.AtYear(100).worldHash == "7AD177B3CC2F73C7"
            && archive.scheduleHash == "56C8B99FA1E8DDD1", "existing frozen reference anchors exact");
        Check(manager.TryBeginReferencePreview(0), "genuine frozen Legacy40 reference preview opens");
        yield return null; yield return null;
        Check(ecology.StandGeometryModelVersion == StandGeometryModel.Legacy40
            && manager.RackPlanningModelVersion == RackPlanningModel.Legacy && !manager.RackPlanningV2Active,
            "reference preview is Legacy40 and rack model 0");
        CheckUnchangedReads("reference-legacy40", 0);
        manager.EndReferencePreview();
        yield return null; yield return null;
        Check(ecology.StandGeometryModelVersion == StandGeometryModel.Enlarged80 && !manager.RackPlanningV2Active, "model-0 world restored after reference");
        Check(Hash(File.ReadAllBytes(archivePath)) == archiveBefore, "frozen archive bytes unchanged");
    }
}
