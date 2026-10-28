#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

// Disposable RACK-V2-05 Unity gate (direct map rack editing). Copy into Assets/ForestPrototype, run Begin, remove it and its .meta
// (Tools/Verification/RackV2Editing/run_gate.py does all three).
//
// Part A, authoritative semantics, no UI: in a fresh model-1 Enlarged80 world it drives the manager facade (CreateRackFromMap,
// ReshapeRackFromMap, DeleteRackFromMap, ResetRacksToSuggestion, UndoLastRackEdit) and proves stable identity, entrance on the
// headland, one-level undo for every edit kind, refused geometry (no mutation of the whole save), no suggestion mutation, no
// counter or work-order duplication, confirmed-plan and executed-rack locking, the approved-work warning (OPEN-3) and the
// model-0 refusal.
//
// Part B, the real pointer: the Stand Map is opened in the graphical Editor and driven by the REAL desktop pointer and keyboard
// (xdotool in a private display, pointer_driver.py), not by invoking callbacks or queuing Input System events. The driver executes
// request files written here; positions are mapped from UI coordinates to screen coordinates by a measured calibration of the OS
// pointer against Mouse.current.position. It draws, selects, moves, rotates, refuses, undoes, deletes (button, Delete, Backspace)
// and resets racks, and captures the rendered map at each requested resolution.
//
// Run: -executeMethod RackV2EditingVerification.Begin   CCF_DIAG_DIR=<outside the repo>   CCF_UI_SIZES=1280x720,1920x1080
//      CCF_DRIVER_DIR=<folder shared with pointer_driver.py>
// Writes only its evidence folder. No Save(), scene save, annual advance or archive write.
public static class RackV2EditingVerification
{
    public const string Marker = "RackV2EditingVerification.Begin";

    public static void Begin()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        Vector2Int[] sizes = RackV2EditingRunner.Sizes();
        RackV2EditingRunner.SetGameSize(sizes[0].x, sizes[0].y);
        EditorApplication.isPlaying = true;
    }

    // First-use help is another harness's business; mark it seen so it does not obscure the map.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Prepare()
    {
        if (!Environment.GetCommandLineArgs().Any(a => a == Marker)) return;
        foreach (MenuHelpView.Menu menu in Enum.GetValues(typeof(MenuHelpView.Menu)))
            PlayerPrefs.SetInt(MenuHelpView.PreferencePrefix + menu, 1);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (Environment.GetCommandLineArgs().Any(a => a == Marker))
            new GameObject("Disposable RACK-V2-05 editing gate").AddComponent<RackV2EditingRunner>();
    }
}

[Serializable] public class PointerOp { public string op; public int x; public int y; public int ms; public string name; public string key; }
[Serializable] public class PointerRequest { public int seq; public PointerOp[] ops; }

public sealed class RackV2EditingRunner : MonoBehaviour
{
    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
    private int checks, shots, seq;
    private ScenarioOneUiRoot ui;
    private ScenarioOneManager manager;
    private ForestEcologyController ecology;
    private ForestSaveController saves;
    private string output, driverDir, freshJson;
    private readonly List<string> manifest = new List<string>(), notes = new List<string>(), stateLines = new List<string>();
    private float ax, bx, ay, by;
    private bool calibrated;
    private StandMapRackEditor RackEd => ui.Map.RackEditor;
    private ScenarioOneManager M => manager;

    private void Check(bool ok, string message)
    {
        if (!ok) throw new InvalidOperationException(message);
        checks++;
        Debug.Log("RACKV2EDIT_CHECK " + message);
    }

    private void Note(string text) { notes.Add(text); Debug.Log("RACKV2EDIT_NOTE " + text); }
    private void State(string text) { stateLines.Add("  " + text); Debug.Log("RACKV2EDIT_STATE " + text); }

    private static string Hash(string text)
    {
        using (SHA256 hash = SHA256.Create())
            return string.Concat(hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(text)).Select(b => b.ToString("x2")));
    }

    // ---------- Editor window helpers ----------
    public static void SetGameSize(int width, int height)
    {
        if (Application.isBatchMode) return;
        Assembly assembly = typeof(Editor).Assembly;
        Type sizesType = assembly.GetType("UnityEditor.GameViewSizes");
        Type singleton = typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
        object sizes = singleton.GetProperty("instance", BindingFlags.Public | BindingFlags.Static).GetValue(null);
        MethodInfo getGroup = sizesType.GetMethod("GetGroup");
        object group = getGroup.Invoke(sizes, new[] { Enum.ToObject(getGroup.GetParameters()[0].ParameterType, 0) });
        Type sizeType = assembly.GetType("UnityEditor.GameViewSize"), kind = assembly.GetType("UnityEditor.GameViewSizeType");
        object size = Activator.CreateInstance(sizeType, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
            null, new object[] { Enum.ToObject(kind, 1), width, height, "RackV2Edit " + width + "x" + height }, null);
        int index = (int)group.GetType().GetMethod("GetTotalCount").Invoke(group, null);
        group.GetType().GetMethod("AddCustomSize").Invoke(group, new[] { size });
        Type viewType = assembly.GetType("UnityEditor.GameView"); EditorWindow view = EditorWindow.GetWindow(viewType);
        viewType.GetProperty("selectedSizeIndex", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).SetValue(view, index);
        var gizmos = viewType.GetProperty("showGizmos", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (gizmos != null && gizmos.CanWrite) gizmos.SetValue(view, false);
        // A maximised Game view gives the real pointer the most screen pixels to aim with.
        try { view.maximized = true; } catch (Exception) { }
        view.Show(); view.Focus(); view.Repaint();
    }

    public static Vector2Int[] Sizes()
    {
        string text = Environment.GetEnvironmentVariable("CCF_UI_SIZES");
        if (string.IsNullOrEmpty(text)) text = "1280x720,1920x1080";
        return text.Split(',').Select(s => s.Trim().Split('x')).Select(p => new Vector2Int(int.Parse(p[0]), int.Parse(p[1]))).ToArray();
    }

    private static IEnumerator Settle(int frames = 6) { for (int i = 0; i < frames; i++) yield return null; }

    private static IEnumerator Wait(float seconds)
    {
        double until = Time.realtimeSinceStartupAsDouble + seconds;
        while (Time.realtimeSinceStartupAsDouble < until) yield return null;
    }

    private static bool Visible(VisualElement e)
    {
        for (VisualElement p = e; p != null; p = p.parent)
            if (p.resolvedStyle.display == DisplayStyle.None || p.resolvedStyle.visibility == Visibility.Hidden || p.resolvedStyle.opacity <= 0.01f) return false;
        return e.worldBound.width > 0 && e.worldBound.height > 0;
    }

    private float PixelsPerUnit => ui.RootElement.worldBound.width > 0f ? Screen.width / ui.RootElement.worldBound.width : 1f;

    // ---------- state helpers ----------
    private string SaveJson() => JsonUtility.ToJson(saves.CaptureData());

    // The whole save with the rack plan itself neutralised: everything the rack editing must NOT touch.
    private string SaveOutsideRackPlan()
    {
        ForestSaveData data = saves.CaptureData();
        data.scenarioOne.racks = new List<RackSaveData>();
        data.scenarioOne.nextRackId = 0;
        data.scenarioOne.rackPlanConfirmed = false;
        return JsonUtility.ToJson(data);
    }

    private static string F(float value) => value.ToString("R", Inv);

    // Compares the save outside the rack plan with a baseline; on a mismatch both are written to the evidence folder and the first
    // differing field is reported, so a failure explains itself.
    private void CheckOutsideRackPlan(string baseline, string message)
    {
        string now = SaveOutsideRackPlan();
        if (now == baseline) { checks++; Debug.Log("RACKV2EDIT_CHECK " + message); return; }
        int i = 0, n = Math.Min(baseline.Length, now.Length);
        while (i < n && baseline[i] == now[i]) i++;
        File.WriteAllText(Path.Combine(output, "outside-baseline.json"), baseline);
        File.WriteAllText(Path.Combine(output, "outside-now.json"), now);
        int from = Math.Max(0, i - 120);
        throw new InvalidOperationException(message + " | first difference at char " + i + " of " + baseline.Length + "/" + now.Length
            + " | baseline: ..." + baseline.Substring(from, Math.Min(260, baseline.Length - from)) + " | now: ..." + now.Substring(from, Math.Min(260, now.Length - from)));
    }

    private string RackSig(RackPlanRack r) => r.Id + ":" + F(r.Entrance.X) + "," + F(r.Entrance.Z) + ">" + F(r.Far.X) + "," + F(r.Far.Z);
    private string PlanSig() => string.Join(";", manager.RackPlan.Select(RackSig)) + "|next=" + manager.NextRackId + "|confirmed=" + manager.RackPlanConfirmed;
    private static string SuggestionSig(IReadOnlyList<SuggestedRackSegment> s)
        => string.Join(";", s.Select(x => F(x.Entrance.X) + "," + F(x.Entrance.Z) + ">" + F(x.Far.X) + "," + F(x.Far.Z) + "/" + F(x.WidthM)));
    private string OrderSig() => string.Join(";", manager.WorkOrders.OrderBy(o => o.workOrderId).Select(o => o.workOrderId + ":" + o.rackId + ":" + o.status + ":" + o.targetTreeId));
    // The orders without their ids: an order recreated for a tree that was dropped and added back legitimately has a new id.
    private string OrderContentSig() => string.Join(";", manager.WorkOrders.Select(o => o.rackId + ":" + o.status + ":" + o.targetTreeId).OrderBy(x => x, StringComparer.Ordinal));
    private string OtherOrdersSig(int rackId) => string.Join(";", manager.WorkOrders.Where(o => o.rackId != rackId).OrderBy(o => o.workOrderId).Select(o => o.workOrderId + ":" + o.rackId + ":" + o.status + ":" + o.targetTreeId));
    private RackPlanRack Rack(int id) { manager.TryGetRack(id, out RackPlanRack rack); return rack; }
    private static RackPoint E(float z) => new RackPoint(ScenarioOneHeadlandGeometry.HeadlandX, z);
    private static RackPoint P(float x, float z) => new RackPoint(x, z);
    private static bool Near(float a, float b, float tolerance) => Mathf.Abs(a - b) <= tolerance;
    private static bool Same(RackPoint a, RackPoint b, float tolerance = 0.0001f) => Near(a.X, b.X, tolerance) && Near(a.Z, b.Z, tolerance);

    private bool OrdersUnique()
        => !manager.WorkOrders.GroupBy(o => o.workOrderId).Any(g => g.Count() > 1)
            && !manager.WorkOrders.Where(o => o.rackId > 0 && o.IsOpen).GroupBy(o => o.targetTreeId).Any(g => g.Count() > 1);

    private IEnumerator ReloadFresh()
    {
        Check(saves.LoadData(JsonUtility.FromJson<ForestSaveData>(freshJson), false), "the fresh baseline world reloads in memory");
        yield return Settle(8);
        Check(manager.RackPlanningV2Active && manager.PlannedRackCount == 0 && manager.NextRackId == 1 && !manager.RackPlanConfirmed,
            "reloaded baseline is a fresh, empty, unconfirmed V2 plan");
    }

    // ---------- run loop ----------
    private IEnumerator Start()
    {
        for (int i = 0; i < 6; i++) yield return null;
        output = Environment.GetEnvironmentVariable("CCF_DIAG_DIR");
        if (string.IsNullOrEmpty(output)) output = Path.Combine(Application.temporaryCachePath, "rack-v2-editing");
        Directory.CreateDirectory(output);
        driverDir = Environment.GetEnvironmentVariable("CCF_DRIVER_DIR");
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
        File.WriteAllText(Path.Combine(output, "shots.json"),
            "{\n \"sizes\": \"" + string.Join(",", Sizes().Select(s => s.x + "x" + s.y)) + "\",\n \"shots\": [\n"
            + string.Join(",\n", manifest) + "\n ],\n \"states\": [\n" + string.Join(",\n", stateLines) + "\n ]\n}\n");
        File.WriteAllLines(Path.Combine(output, "notes.txt"), notes);
        if (!string.IsNullOrEmpty(driverDir)) File.WriteAllText(Path.Combine(driverDir, "finished"), failure == null ? "pass" : "fail");
        Debug.Log(failure == null ? "RACK_V2_EDITING_UNITY_PASS checks=" + checks + " shots=" + shots
            : "RACK_V2_EDITING_UNITY_FAIL checks=" + checks + " shots=" + shots + " exception=" + failure);
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(failure == null ? 0 : 1);
    }

    private IEnumerator Run()
    {
        ui = FindFirstObjectByType<ScenarioOneUiRoot>();
        manager = FindFirstObjectByType<ScenarioOneManager>();
        ecology = FindFirstObjectByType<ForestEcologyController>();
        saves = FindFirstObjectByType<ForestSaveController>();
        string config = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME");
        Check(!string.IsNullOrEmpty(config) && Path.GetFullPath(Application.persistentDataPath)
            .StartsWith(Path.GetFullPath(config) + Path.DirectorySeparatorChar, StringComparison.Ordinal), "isolated persistent data path");
        Check(ui != null && manager != null && ecology != null && saves != null, "Scenario One scene components present");
        Check(manager.RackPlanningV2Active && ecology.StandGeometryModelVersion == StandGeometryModel.Enlarged80, "fresh model-1 Enlarged80 rack world");
        Check(manager.PlannedRackCount == 0 && manager.NextRackId == 1 && !manager.RackPlanConfirmed, "empty authoritative plan at the start");
        freshJson = SaveJson();
        State("fresh save=" + Hash(freshJson) + " trees=" + ecology.LivingTreeCount);

        yield return PartA();
        yield return ReloadFresh();
        if (string.IsNullOrEmpty(driverDir))
            Note("CCF_DRIVER_DIR not set: the real-pointer part (B) was NOT run");
        else
            yield return PartB();
    }

    // =================================================================================================================
    // Part A — authoritative semantics through the facade
    // =================================================================================================================
    private IEnumerator PartA()
    {
        RackEditBounds bounds = manager.RackEditBounds;
        Check(Near(bounds.HeadlandX, ScenarioOneHeadlandGeometry.HeadlandX, 0f) && Near(bounds.HalfExtentM, 40f, 0.001f), "editing bounds come from the headland and stand geometry");
        Check(manager.RackEditingOpen && !manager.CanUndoRackEdit, "editing is open and there is nothing to undo at the start");

        IReadOnlyList<SuggestedRackSegment> suggestion = manager.GetSuggestedRackLayout();
        Check(suggestion.Count == 5, "five suggested routes");
        string suggestionBefore = SuggestionSig(suggestion);
        string baseSave = SaveJson(), baseOutside = SaveOutsideRackPlan();
        int baseOrders = manager.WorkOrders.Count;

        // ---- refused geometry: nothing changes, the whole save is identical --------------------------------------------
        var negatives = new List<(string name, RackPoint entrance, RackPoint far, string text)>
        {
            ("zero length", E(-24f), E(-24f), "shorter than"),
            ("near-zero length", E(-24f), P(ScenarioOneHeadlandGeometry.HeadlandX + 1f, -24f), "shorter than"),
            ("just under the minimum", E(-24f), P(ScenarioOneHeadlandGeometry.HeadlandX + 4.9f, -24f), "shorter than"),
            ("entrance off the headland", P(-20f, -24f), P(30f, -24f), "must be on the headland"),
            ("entrance past the strip end", E(39f), P(30f, 39f), "must be on the headland"),
            ("far end outside the stand (east)", E(-24f), P(60f, -24f), "leaves the stand"),
            ("far end outside the stand (north)", E(-24f), P(10f, 55f), "leaves the stand"),
            ("non-finite endpoint", E(-24f), P(float.NaN, -24f), "non-finite")
        };
        foreach (var negative in negatives)
        {
            RackPlanOperationResult refused = manager.CreateRackFromMap(negative.entrance, negative.far);
            Check(refused.Status == RackPlanOperationStatus.Invalid && refused.Message.Contains(negative.text),
                negative.name + " is refused: " + refused.Message);
            Check(SaveJson() == baseSave && !manager.CanUndoRackEdit, negative.name + " changed nothing (whole save identical, no undo recorded)");
        }

        // ---- create: stable ids, entrance on the headland -------------------------------------------------------------
        RackPlanOperationResult first = manager.CreateRackFromMap(E(-24f), P(30f, -24f));
        Check(first.Status == RackPlanOperationStatus.Applied && first.RackId == 1, "first rack is created with id 1: " + first.Message);
        RackPlanRack rack1 = Rack(1);
        Check(Near(rack1.Entrance.X, ScenarioOneHeadlandGeometry.HeadlandX, 0.0001f) && rack1.RemovalTreeIds.Count > 0 && !string.IsNullOrEmpty(rack1.EntranceTreeId),
            "a created rack has its entrance on the headland and an entrance tree");
        Check(manager.PlannedRackCount == 1 && manager.CanUndoRackEdit && manager.WorkOrders.Count == baseOrders, "the count derives from the plan; no work order is created");

        string afterOne = SaveJson();
        var positives = new List<(string name, RackPoint entrance, RackPoint far, string text)>
        {
            ("duplicate of rack 1", E(-24f), P(30f, -24f), "duplicates rack 1"),
            ("duplicate within tolerance", E(-23.8f), P(30f, -23.8f), "duplicates rack 1"),
            ("parallel overlap 1 m away", E(-23f), P(30f, -23f), "overlaps rack 1"),
            ("parallel overlap 2.3 m away", E(-21.7f), P(30f, -21.7f), "overlaps rack 1"),
            ("crossing rack 1", E(-10f), P(30f, -35f), "overlaps rack 1")
        };
        foreach (var negative in positives)
        {
            RackPlanOperationResult refused = manager.CreateRackFromMap(negative.entrance, negative.far);
            Check(refused.Status == RackPlanOperationStatus.Invalid && refused.Message.Contains(negative.text), negative.name + " is refused: " + refused.Message);
            Check(SaveJson() == afterOne, negative.name + " changed nothing");
        }

        // A deliberately skewed rack (not the suggested direction) is allowed: freedom to deviate.
        RackPlanOperationResult second = manager.CreateRackFromMap(E(-8f), P(35f, 2f));
        Check(second.Status == RackPlanOperationStatus.Applied && second.RackId == 2, "a rack off the suggested direction is accepted: " + second.Message);
        float skew = Mathf.Abs(RackEditing.DirectedAngleDegrees(Rack(2).Entrance, Rack(2).Far));
        Check(skew > RackEditing.AngleSnapDegrees, "rack 2 deliberately deviates from the suggested direction (" + F(skew) + " deg)");
        foreach (float z in new[] { 12f, 24f, 34f })
            Check(manager.CreateRackFromMap(E(z), P(38f, z)).Succeeded, "rack at z " + z + " is created");
        Check(manager.PlannedRackCount == 5 && manager.NextRackId == 6, "five racks, ids never reused (next id 6)");
        RackPlanOperationResult sixth = manager.CreateRackFromMap(E(-34f), P(38f, -34f));
        Check(sixth.Status == RackPlanOperationStatus.Invalid && sixth.Message.Contains("5 racks") && manager.PlannedRackCount == 5, "a sixth rack is refused");
        Check(manager.WorkOrders.Count == baseOrders && manager.MarkedRackEntranceCount == 0 && OrderSig() == "", "five draft racks create no work order and mark nothing");
        CheckOutsideRackPlan(baseOutside, "everything outside the rack plan in the save is untouched by creation");

        // ---- reshape: move and rotate keep identity; one-level undo ----------------------------------------------------
        string planBeforeMove = PlanSig();
        RackPlanRack two = Rack(2);
        RackEditing.MoveAlongHeadland(bounds, two.Entrance, two.Far, two.Entrance.Z + 3f, out RackPoint movedEntrance, out RackPoint movedFar);
        RackPlanOperationResult moved = manager.ReshapeRackFromMap(2, movedEntrance, movedFar);
        Check(moved.Status == RackPlanOperationStatus.Applied && moved.RackId == 2, "moving rack 2 is applied: " + moved.Message);
        RackPlanRack two2 = Rack(2);
        Check(two2.Id == 2 && Same(two2.Entrance, movedEntrance) && Near(two2.Entrance.X, ScenarioOneHeadlandGeometry.HeadlandX, 0.0001f), "rack 2 keeps its id and its entrance stays on the headland");
        Check(Near(two2.LengthM, two.LengthM, 0.001f) && Near(two2.AngleDegrees, two.AngleDegrees, 0.01f), "a move keeps length and angle");
        Check(manager.WorkOrders.Count == baseOrders && manager.RackOrderState(2).Open == 0, "moving a DRAFT rack creates no work order (orders appear only at Confirm)");
        Check(manager.CanUndoRackEdit && manager.RackEditUndoDescription.Contains("rack 2"), "the move can be undone");
        RackPlanOperationResult undoMove = manager.UndoLastRackEdit();
        Check(undoMove.Status == RackPlanOperationStatus.Applied && PlanSig() == planBeforeMove, "undo restores the exact geometry, ids and id allocator");
        Check(!manager.CanUndoRackEdit && manager.UndoLastRackEdit().Status == RackPlanOperationStatus.NoChange, "undo is exactly one level");

        // Rotation about the entrance.
        two = Rack(2);
        // Rotate downward: an upward turn would swing the far end into rack 3's corridor, which the overlap rule rightly refuses.
        float length = two.LengthM, angle = RackEditing.DirectedAngleDegrees(two.Entrance, two.Far) - 7f;
        RackPoint rotatedFar = P(two.Entrance.X + length * Mathf.Cos(angle * Mathf.Deg2Rad), two.Entrance.Z + length * Mathf.Sin(angle * Mathf.Deg2Rad));
        string planBeforeRotate = PlanSig();
        Check(manager.ReshapeRackFromMap(2, two.Entrance, rotatedFar).Status == RackPlanOperationStatus.Applied, "rotating rack 2 about its entrance is applied");
        Check(Same(Rack(2).Entrance, two.Entrance) && Near(Rack(2).LengthM, length, 0.01f) && Near(Rack(2).AngleDegrees, two.AngleDegrees - 7f, 0.05f), "rotation keeps the entrance and the length and changes the angle");
        Check(manager.UndoLastRackEdit().Succeeded && PlanSig() == planBeforeRotate, "undo restores the rotation exactly");
        Check(manager.ReshapeRackFromMap(2, two.Entrance, two.Far).Status == RackPlanOperationStatus.NoChange && !manager.CanUndoRackEdit, "an unchanged reshape is a no-op and records no undo");

        // Refused reshapes change nothing.
        string planNow = PlanSig(), saveNow = SaveJson();
        foreach (var bad in new List<(string name, int id, RackPoint e, RackPoint f, string text)>
        {
            ("rotate onto a too-short rack", 2, two.Entrance, P(two.Entrance.X + 2f, two.Entrance.Z), "shorter than"),
            ("move rack 2 onto rack 3", 2, E(Rack(3).Entrance.Z + 1f), P(30f, Rack(3).Entrance.Z + 1f), "overlaps rack 3"),
            ("move the entrance off the headland", 2, P(-20f, two.Entrance.Z), two.Far, "must be on the headland"),
            ("rotate outside the stand", 2, two.Entrance, P(45f, two.Entrance.Z), "leaves the stand")
        })
        {
            RackPlanOperationResult refused = manager.ReshapeRackFromMap(bad.id, bad.e, bad.f);
            Check(refused.Status == RackPlanOperationStatus.Invalid && refused.Message.Contains(bad.text), bad.name + " is refused: " + refused.Message);
            Check(PlanSig() == planNow && SaveJson() == saveNow && !manager.CanUndoRackEdit, bad.name + " changed nothing");
        }
        Check(manager.ReshapeRackFromMap(99, E(0f), P(30f, 0f)).Status == RackPlanOperationStatus.Invalid, "an unknown rack id is refused");

        // ---- delete and undo: the same identity returns ----------------------------------------------------------------
        RackPlanRack three = Rack(3);
        string planBeforeDelete = PlanSig();
        Check(manager.DeleteRackFromMap(3).Status == RackPlanOperationStatus.Applied && Rack(3) == null && manager.PlannedRackCount == 4, "rack 3 is deleted");
        Check(manager.NextRackId == 6, "deleting does not give the id back");
        Check(manager.UndoLastRackEdit().Status == RackPlanOperationStatus.Applied && PlanSig() == planBeforeDelete, "undo restores rack 3 with the same id and geometry");
        RackPlanRack three2 = Rack(3);
        Check(three2 != null && Same(three2.Entrance, three.Entrance) && Same(three2.Far, three.Far) && three2.RemovalTreeIds.Count == three.RemovalTreeIds.Count,
            "the restored rack matches the deleted one, including its removal trees");

        // Create then undo: the allocator returns too.
        Check(manager.DeleteRackFromMap(5).Succeeded, "rack 5 deleted to make room");
        string planBeforeCreate = PlanSig();
        RackPlanOperationResult created = manager.CreateRackFromMap(E(34f), P(38f, 34f));
        Check(created.Succeeded && created.RackId == 6 && manager.NextRackId == 7, "a new rack takes the next id (6), never a freed one");
        Check(manager.UndoLastRackEdit().Succeeded && PlanSig() == planBeforeCreate && manager.NextRackId == 6, "undoing a create removes it and returns the allocator");
        Check(manager.UndoLastRackEdit().Status == RackPlanOperationStatus.NoChange, "still only one level");
        Check(manager.CreateRackFromMap(E(34f), P(38f, 34f)).Succeeded && Rack(6) != null, "re-creating after the undo gets id 6 again, never a duplicate");

        // A change made outside the facade makes the undo unavailable instead of stale.
        Check(manager.CanUndoRackEdit, "the create can be undone");
        Check(manager.DeleteRack(1).Succeeded, "rack 1 deleted straight through the plan API");
        Check(!manager.CanUndoRackEdit && manager.UndoLastRackEdit().Status == RackPlanOperationStatus.NoChange, "a change outside the facade makes undo unavailable");
        Check(manager.AddRack(E(-24f), P(30f, -24f)).Succeeded, "rack re-added through the plan API");

        // ---- reset to the suggested layout -------------------------------------------------------------------------------
        Check(!manager.RackLayoutMatchesSuggestion(suggestion), "the edited plan differs from the suggestion (so a player would be asked first)");
        int highestBefore = manager.RackPlan.Max(r => r.Id);
        string planBeforeReset = PlanSig();
        RackPlanOperationResult reset = manager.ResetRacksToSuggestion(suggestion);
        Check(reset.Status == RackPlanOperationStatus.Applied && manager.PlannedRackCount == 5, "reset replaces the plan with five racks: " + reset.Message);
        Check(manager.RackLayoutMatchesSuggestion(suggestion), "after reset the plan is exactly the suggested layout");
        for (int i = 0; i < 5; i++)
            Check(Same(manager.RackPlan[i].Entrance, suggestion[i].Entrance) && Same(manager.RackPlan[i].Far, suggestion[i].Far), "reset rack " + (i + 1) + " has exactly the suggested geometry");
        Check(manager.RackPlan.All(r => r.Id > highestBefore), "reset racks are new objects with new ids (ids are never reused)");
        Check(SuggestionSig(suggestion) == suggestionBefore && SuggestionSig(manager.GetSuggestedRackLayout()) == suggestionBefore, "the suggestion source is not mutated by reset");
        Check(manager.WorkOrders.Count == baseOrders && manager.MarkedRackEntranceCount == 0, "reset creates no work order and no mark");
        Check(manager.ResetRacksToSuggestion(suggestion).Status == RackPlanOperationStatus.NoChange, "resetting an unedited plan is a no-op (no confirmation needed)");
        string planAfterReset = PlanSig();
        RackPlanOperationResult undoReset = manager.UndoLastRackEdit();
        Check(undoReset.Status == RackPlanOperationStatus.Applied && PlanSig() == planBeforeReset, "undo of the reset restores the previous racks with their old ids");
        // The undo returned the id allocator, so the second reset hands out the same ids as the first: identity is the allocator's, not luck.
        Check(manager.ResetRacksToSuggestion(suggestion).Succeeded && manager.RackPlan.Count == 5 && manager.RackLayoutMatchesSuggestion(suggestion) && PlanSig() == planAfterReset,
            "reset again works, and after the undo returned the allocator it reproduces the same ids");
        CheckOutsideRackPlan(baseOutside, "everything outside the rack plan in the save is untouched after create, move, rotate, delete, undo and reset");
        Check(manager.WorkOrders.Count == baseOrders, "no counter or work order was created by drafting");

        // ---- a confirmed plan is fixed; reopening makes it editable; pending work reconciles without duplicates -------------
        string confirmedSave;
        Check(manager.ConfirmRackPlan().Succeeded && manager.RackPlanConfirmed, "the five-rack plan confirms");
        int pending = manager.WorkOrders.Count(o => o.rackId > 0 && o.status == ScenarioWorkStatus.Pending);
        Check(pending > 0 && OrdersUnique(), "confirming created pending rack work, once per tree");
        confirmedSave = SaveJson();
        Check(!manager.RackEditingOpen, "a confirmed plan is not open for editing");
        Check(manager.CreateRackFromMap(E(-34f), P(38f, -34f)).Status == RackPlanOperationStatus.Invalid, "creating on a confirmed plan is refused");
        Check(manager.ReshapeRackFromMap(manager.RackPlan[0].Id, manager.RackPlan[0].Entrance, P(30f, manager.RackPlan[0].Entrance.Z + 1f)).Status == RackPlanOperationStatus.Invalid, "reshaping on a confirmed plan is refused");
        Check(manager.DeleteRackFromMap(manager.RackPlan[0].Id).Status == RackPlanOperationStatus.Invalid && manager.ResetRacksToSuggestion(suggestion).Status == RackPlanOperationStatus.Invalid, "deleting and resetting on a confirmed plan are refused");
        Check(SaveJson() == confirmedSave, "the refused edits changed nothing");

        Check(manager.ReopenRackPlan().Succeeded && manager.RackEditingOpen, "reopening makes the plan editable again");
        int target = manager.RackPlan[1].Id;
        RackPlanRack t0 = Rack(target);
        string ordersBefore = OrderContentSig(), othersBefore = OtherOrdersSig(target);
        RackPlanOperationResult pendingEdit = manager.ReshapeRackFromMap(target, t0.Entrance, P(t0.Far.X - 6f, t0.Far.Z + 0.2f));
        Check(pendingEdit.Status == RackPlanOperationStatus.Applied, "an edit with pending work is applied and reconciled: " + pendingEdit.Message);
        Check(OrdersUnique() && manager.WorkOrders.Where(o => o.rackId == target).All(o => o.status == ScenarioWorkStatus.Pending), "pending work is reconciled without duplicates");
        Check(OtherOrdersSig(target) == othersBefore, "other racks' orders (ids, status, trees) are untouched by the edit");
        Check(manager.UndoLastRackEdit().Succeeded && OrdersUnique() && OrderContentSig() == ordersBefore && OtherOrdersSig(target) == othersBefore,
            "undo of that edit restores the same pending orders per tree (no duplicates, others untouched)");

        // Approved but unexecuted work: warned, not changed; with consent, cancelled and recreated.
        Check(manager.ConfirmRackPlan().Succeeded && manager.ApprovePendingWork(), "plan confirmed and its work approved");
        Check(manager.ReopenRackPlan().Succeeded, "plan reopened with approved work outstanding");
        string approvedOrders = OrderSig(), approvedPlan = PlanSig();
        RackPlanOperationResult warned = manager.ReshapeRackFromMap(target, t0.Entrance, P(t0.Far.X - 6f, t0.Far.Z + 0.2f));
        Check(warned.Status == RackPlanOperationStatus.RequiresApprovalWarning && warned.RequiresApprovalWarning, "editing a rack with approved work asks first: " + warned.Message);
        Check(OrderSig() == approvedOrders && PlanSig() == approvedPlan && !manager.CanUndoRackEdit, "the warning changed nothing and recorded no undo");
        RackPlanOperationResult warnedDelete = manager.DeleteRackFromMap(target);
        Check(warnedDelete.RequiresApprovalWarning && OrderSig() == approvedOrders, "deleting a rack with approved work asks first");
        RackPlanOperationResult warnedReset = manager.ResetRacksToSuggestion(manager.GetSuggestedRackLayout());
        Check(warnedReset.Status == RackPlanOperationStatus.NoChange || warnedReset.RequiresApprovalWarning, "resetting with approved work asks first or is a no-op");
        RackPlanOperationResult consented = manager.ReshapeRackFromMap(target, t0.Entrance, P(t0.Far.X - 6f, t0.Far.Z + 0.2f), true);
        Check(consented.Status == RackPlanOperationStatus.Applied && consented.OrdersCancelled > 0, "with consent the approved work is cancelled and recreated: " + consented.Message);
        Check(OrdersUnique() && manager.WorkOrders.Where(o => o.rackId == target && o.IsOpen).All(o => o.status == ScenarioWorkStatus.Pending), "the recreated work is pending and not duplicated");
        Check(manager.WorkOrders.Where(o => o.rackId > 0 && o.rackId != target && o.IsOpen).All(o => o.status == ScenarioWorkStatus.Approved), "unaffected racks keep their approved work");

        // ---- executed removal locks the rack (OPEN-3) --------------------------------------------------------------------
        Check(manager.ConfirmRackPlan(true).Succeeded, "plan confirmed again");
        int lockedId = manager.RackPlan[3].Id;
        ScenarioOneWorkOrder executed = manager.WorkOrders.First(o => o.rackId == lockedId);
        executed.status = ScenarioWorkStatus.Completed;
        executed.resolvedYear = 0;
        Check(manager.RackIsLocked(lockedId) && manager.ReopenRackPlan().Succeeded, "a rack with an executed removal is locked and the plan reopens around it");
        string lockedPlan = PlanSig(), lockedOrders = OrderSig();
        RackPlanRack lockedRack = Rack(lockedId);
        RackPlanOperationResult lockedMove = manager.ReshapeRackFromMap(lockedId, lockedRack.Entrance, P(lockedRack.Far.X - 5f, lockedRack.Far.Z));
        Check(lockedMove.Status == RackPlanOperationStatus.Locked, "moving or rotating an executed rack is refused: " + lockedMove.Message);
        Check(manager.DeleteRackFromMap(lockedId).Status == RackPlanOperationStatus.Locked, "deleting an executed rack is refused");
        Check(manager.ResetRacksToSuggestion(suggestion).Status == RackPlanOperationStatus.Locked, "a plan holding an executed rack cannot be reset over it");
        Check(PlanSig() == lockedPlan, "the refusals left the rack geometry, ids and allocator unchanged");
        Check(OrderSig() == lockedOrders, "the refusals left every work order (ids, status, trees) unchanged, so the executed removal is untouched");
        Check(!manager.CanUndoRackEdit, "an edit from before the confirmation, approval and execution can no longer be undone over the new work");
        int free = manager.RackPlan.First(r => !r.IsLocked && r.Id != target).Id;
        RackPlanRack freeRack = Rack(free);
        Check(manager.ReshapeRackFromMap(free, freeRack.Entrance, P(freeRack.Far.X - 4f, freeRack.Far.Z), true).Succeeded, "the other racks stay editable");
        Check(OrdersUnique() && manager.WorkOrders.Count(o => o.rackId == lockedId && o.status == ScenarioWorkStatus.Completed) == 1, "the executed order is still exactly one completed order");

        // ---- the model-0 / pre-v21 path never enters V2 editing -------------------------------------------------------------
        ForestSaveData legacy = saves.CaptureData();
        legacy.version = 20;
        legacy.scenarioOne.rackPlanningModel = RackPlanningModel.Legacy;
        legacy.scenarioOne.rackPlanConfirmed = false;
        legacy.scenarioOne.nextRackId = 1;
        legacy.scenarioOne.racks = new List<RackSaveData>();
        foreach (ScenarioOneWorkOrder order in legacy.scenarioOne.workOrders) order.rackId = 0;
        Check(saves.LoadData(legacy, false), "a projected pre-v21 (model 0) save loads");
        yield return Settle(6);
        Check(!manager.RackPlanningV2Active && !manager.RackEditingOpen && manager.Racks.Count == 0, "a model-0 world is not a V2 world and offers no V2 editing");
        string legacySave = SaveJson();
        Check(manager.CreateRackFromMap(E(0f), P(30f, 0f)).Status == RackPlanOperationStatus.Invalid, "the facade refuses to create in a model-0 world");
        Check(manager.DeleteRackFromMap(1).Status == RackPlanOperationStatus.Invalid && manager.ResetRacksToSuggestion(suggestion).Status == RackPlanOperationStatus.Invalid, "delete and reset are refused in a model-0 world");
        Check(manager.GetSuggestedRackLayout().Count == 0, "a model-0 world has no suggestion");
        Check(SaveJson() == legacySave && manager.Racks.Count == 0, "nothing changed in the model-0 world");
        State("part A complete: facade semantics verified, refusals leave the save byte-identical");
    }

    // =================================================================================================================
    // Part B — the real pointer
    // =================================================================================================================
    private IEnumerator Do(PointerOp[] ops, Dictionary<string, Func<IEnumerator>> syncs = null, float timeout = 90f)
    {
        int id = ++seq;
        string name = "request-" + id.ToString("D4") + ".json";
        string temp = Path.Combine(driverDir, name + ".tmp");
        File.WriteAllText(temp, JsonUtility.ToJson(new PointerRequest { seq = id, ops = ops }));
        File.Move(temp, Path.Combine(driverDir, name));
        double until = Time.realtimeSinceStartupAsDouble + timeout;
        string done = Path.Combine(driverDir, "done-" + id.ToString("D4"));
        while (!File.Exists(done))
        {
            if (Time.realtimeSinceStartupAsDouble > until) throw new InvalidOperationException("pointer driver did not finish request " + id + " (is pointer_driver.py running?)");
            foreach (string syncFile in Directory.GetFiles(driverDir, "sync-" + id.ToString("D4") + "-*"))
            {
                string key = Path.GetFileName(syncFile).Substring(("sync-" + id.ToString("D4") + "-").Length);
                string go = Path.Combine(driverDir, "go-" + id.ToString("D4") + "-" + key);
                if (File.Exists(go)) continue;
                if (syncs != null && syncs.TryGetValue(key, out Func<IEnumerator> action)) yield return action();
                File.WriteAllText(go, "go");
            }
            yield return null;
        }
        yield return Settle(4);
    }

    private static PointerOp Move(Vector2 s, int ms = 0) => new PointerOp { op = "move", x = Mathf.RoundToInt(s.x), y = Mathf.RoundToInt(s.y), ms = ms };
    private static PointerOp Down() => new PointerOp { op = "down" };
    private static PointerOp Up() => new PointerOp { op = "up" };
    private static PointerOp Sleep(int ms) => new PointerOp { op = "sleep", ms = ms };
    private static PointerOp Sync(string name) => new PointerOp { op = "sync", name = name };
    private static PointerOp KeyTap(string key) => new PointerOp { op = "key", key = key };
    private static PointerOp KeyDown(string key) => new PointerOp { op = "keydown", key = key };
    private static PointerOp KeyUp(string key) => new PointerOp { op = "keyup", key = key };

    // A UI Toolkit panel point (origin top-left, panel units) to the OS screen coordinates the driver must aim at.
    private Vector2 ScreenOfPanel(Vector2 panel)
    {
        float scale = PixelsPerUnit;
        float mx = panel.x * scale, my = Screen.height - panel.y * scale;
        return new Vector2((mx - bx) / ax, (my - by) / ay);
    }

    private Vector2 ScreenOfFrame(Vector2 framePx) => ScreenOfPanel(ui.Map.Root.Q("map-frame").LocalToWorld(framePx));
    private Vector2 ScreenOfWorld(RackPoint world) => ScreenOfFrame(RackEd.WorldToFramePx(world));
    private Vector2 ScreenOfElement(VisualElement e) => ScreenOfPanel(e.worldBound.center);

    private IEnumerator Calibrate()
    {
        Check(Mouse.current != null, "an Input System mouse exists");
        Vector2[] points = { new Vector2(640, 380), new Vector2(1180, 700), new Vector2(900, 540) };
        var seen = new Vector2[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            yield return Do(new[] { Move(points[i]), Sleep(250), Move(points[i] + Vector2.one, 0), Sleep(250), Move(points[i]) , Sleep(300) });
            yield return Settle(6);
            seen[i] = Mouse.current.position.ReadValue();
            State("calibration point " + i + " screen=" + points[i] + " input=" + seen[i]);
        }
        Check(Mathf.Abs(seen[1].x - seen[0].x) > 50f && Mathf.Abs(seen[1].y - seen[0].y) > 50f,
            "the OS pointer reached the Input System (the desktop pointer moved the game pointer): " + seen[0] + " -> " + seen[1]);
        ax = (seen[1].x - seen[0].x) / (points[1].x - points[0].x);
        bx = seen[0].x - ax * points[0].x;
        ay = (seen[1].y - seen[0].y) / (points[1].y - points[0].y);
        by = seen[0].y - ay * points[0].y;
        Vector2 predicted = new Vector2(ax * points[2].x + bx, ay * points[2].y + by);
        float error = (predicted - seen[2]).magnitude;
        State("calibration ax=" + F(ax) + " bx=" + F(bx) + " ay=" + F(ay) + " by=" + F(by) + " validation-error=" + F(error) + "px game=" + Screen.width + "x" + Screen.height);
        Check(error < 3f, "the pointer calibration predicts a third point to within 3 px (" + F(error) + ")");
        Check(ax > 0f && ay < 0f, "screen x increases with the game x, screen y against the game y (top-left versus bottom-left origin)");
        calibrated = true;
    }

    private IEnumerator Capture(string state, string shot, string extra = "")
    {
        yield return new WaitForEndOfFrame();
        Texture2D image = ScreenCapture.CaptureScreenshotAsTexture();
        if (image == null) throw new InvalidOperationException("no rendered capture for " + shot);
        if (image.width != Screen.width || image.height != Screen.height)
            throw new InvalidOperationException("capture " + image.width + "x" + image.height + " does not match game view " + Screen.width + "x" + Screen.height);
        string file = state + "-" + shot + "-" + Screen.width + "x" + Screen.height + ".png";
        File.WriteAllBytes(Path.Combine(output, file), image.EncodeToPNG());
        Destroy(image);
        shots++;
        VisualElement frame = ui.Map.Root.Q("map-frame");
        float s = PixelsPerUnit; Rect r = frame.worldBound;
        manifest.Add("  {\"file\":\"" + file + "\",\"state\":\"" + state + "\",\"shot\":\"" + shot + "\",\"width\":" + Screen.width + ",\"height\":" + Screen.height
            + ",\"mapFramePx\":[" + F(r.xMin * s) + "," + F(r.yMin * s) + "," + F(r.width * s) + "," + F(r.height * s) + "]"
            + ",\"selectedRack\":" + RackEd.SelectedRackId + ",\"playerRacks\":" + manager.PlannedRackCount
            + ",\"ghost\":" + (RackEd.Ghost.HasValue ? (RackEd.Ghost.Value.Valid ? "\"valid\"" : "\"refused\"") : "null") + (extra.Length > 0 ? "," + extra : "") + "}");
    }

    private IEnumerator Click(VisualElement element)
    {
        if (element == null || !Visible(element)) throw new InvalidOperationException("control not visible for a real click");
        Vector2 at = ScreenOfElement(element);
        yield return Do(new[] { Move(at), Sleep(180), Move(at + new Vector2(1, 0)), Sleep(60), Move(at), Sleep(120), Down(), Sleep(90), Up(), Sleep(200) });
        yield return Settle(6);
    }

    private Button Control(string name) => ui.Map.Root.Q<Button>(name);
    private string StatusText() { Label l = ui.Map.Root.Q<Label>("map-edit-status"); return l != null ? l.text : ""; }

    // A drag in world coordinates, in steps, optionally holding Shift and pausing mid-way for a capture.
    private IEnumerator DragWorld(RackPoint from, RackPoint to, string syncName = null, Func<IEnumerator> onSync = null, bool shift = false, int steps = 14)
    {
        var ops = new List<PointerOp>();
        Vector2 a = ScreenOfWorld(from), b = ScreenOfWorld(to);
        ops.Add(Move(a)); ops.Add(Sleep(150)); ops.Add(Move(a + Vector2.one)); ops.Add(Sleep(60)); ops.Add(Move(a)); ops.Add(Sleep(120));
        if (shift) ops.Add(KeyDown("shift"));
        ops.Add(Down()); ops.Add(Sleep(120));
        for (int i = 1; i <= steps; i++)
        {
            ops.Add(Move(Vector2.Lerp(a, b, (float)i / steps), 0));
            ops.Add(Sleep(35));
            if (syncName != null && i == steps * 2 / 3) { ops.Add(Sleep(120)); ops.Add(Sync(syncName)); }
        }
        ops.Add(Sleep(160));
        ops.Add(Up());
        if (shift) ops.Add(KeyUp("shift"));
        ops.Add(Sleep(200));
        var syncs = new Dictionary<string, Func<IEnumerator>>();
        if (syncName != null && onSync != null) syncs[syncName] = onSync;
        yield return Do(ops.ToArray(), syncs);
        yield return Settle(6);
    }

    private IEnumerator ClickWorld(RackPoint at)
    {
        Vector2 s = ScreenOfWorld(at);
        yield return Do(new[] { Move(s), Sleep(150), Move(s + Vector2.right), Sleep(60), Move(s), Sleep(120), Down(), Sleep(90), Up(), Sleep(200) });
        yield return Settle(6);
    }

    private IEnumerator PressKey(string key)
    {
        yield return Do(new[] { Sleep(100), KeyTap(key), Sleep(250) });
        yield return Settle(6);
    }

    // A point on a rack's centre line, a given distance from its entrance.
    private RackPoint OnRack(RackPlanRack rack, float fraction)
        => P(rack.Entrance.X + (rack.Far.X - rack.Entrance.X) * fraction, rack.Entrance.Z + (rack.Far.Z - rack.Entrance.Z) * fraction);

    private IEnumerator PartB()
    {
        bool lastSize;
        Vector2Int[] sizes = Sizes();
        for (int index = 0; index < sizes.Length; index++)
        {
            Vector2Int size = sizes[index];
            lastSize = index == sizes.Length - 1;
            string tag = size.x + "x" + size.y;
            if (index > 0) yield return ReloadFresh();
            SetGameSize(size.x, size.y);
            yield return Wait(1.5f);
            ui.CloseAll(); yield return Settle(20);
            ui.ShowMap(); yield return Settle(20);
            if (ui.Help != null && ui.Help.IsOpen) { ui.CloseHelp(); yield return Settle(12); }
            Check(ui.CurrentScreen == ScenarioOneUiRoot.UiScreen.Map && Screen.width == size.x && Screen.height == size.y, tag + ": Stand Map open at the requested size");
            yield return Wait(0.6f);
            yield return Calibrate();

            string outsideBaseline = SaveOutsideRackPlan();

            // ---- enter Plan Racks with a real click ---------------------------------------------------------------------
            Check(!ui.Map.RackMode && !RackEd.Active && !Visible(ui.Map.EditLayer), tag + ": planning is off and the pointer layer is hidden at first");
            yield return Click(Control("map-plan-rack"));
            Check(ui.Map.RackMode && RackEd.Active && Visible(ui.Map.EditLayer), tag + ": a real click on Plan Racks entered planning and showed the pointer layer");
            Check(Control("map-plan-rack").text == "Planning Racks — Active", tag + ": the active state is unmistakable");
            Check(Control("map-edit-delete") != null && Control("map-edit-undo") != null && Control("map-edit-reset") != null, tag + ": Delete, Undo and Reset controls exist");
            Check(!Control("map-edit-delete").enabledSelf && !Control("map-edit-undo").enabledSelf, tag + ": nothing selected, nothing to undo");
            Check(ui.MapSource.Model.Racks.Count(r => r.Symbol == MapSymbol.RackSuggested) == 5, tag + ": five dashed suggestions are shown");
            yield return Capture(tag, "01-planning-empty");

            // ---- draw rack 1 with a real drag from the headland: the angle snaps to the suggested direction -----------------
            RackPoint pressed = P(ScenarioOneHeadlandGeometry.HeadlandX + 1.6f, -24f), released = P(36f, -22.9f);
            yield return DragWorld(pressed, released, "drawing", () => DrawingGhost(tag), false);
            Check(manager.PlannedRackCount == 1, tag + ": a real drag created exactly one authoritative rack");
            RackPlanRack r1 = manager.RackPlan[0];
            Check(r1.Id == 1 && Near(r1.Entrance.X, ScenarioOneHeadlandGeometry.HeadlandX, 0.0001f), tag + ": rack 1 has id 1 and its entrance re-snapped onto the headland");
            Check(Near(r1.Entrance.Z, -24f, 0.6f), tag + ": rack 1's entrance is where the drag started along the headland (z " + F(r1.Entrance.Z) + ")");
            Check(Near(r1.Far.Z, r1.Entrance.Z, 0.0001f), tag + ": the drag ended 1.1 m off level over 70 m and soft-snapped to the suggested direction (far z == entrance z)");
            Check(RackEd.SelectedRackId == 1 && ui.MapSource.Model.Racks.Any(r => r.RackId == 1 && r.Selected), tag + ": the new rack is selected and drawn selected");
            Check(manager.WorkOrders.Count == 0 && manager.MarkedRackEntranceCount == 0, tag + ": drawing created no work order and no mark");
            Check(StatusText().StartsWith("Added rack 1"), tag + ": the card says it added rack 1 (" + StatusText() + ")");
            yield return Capture(tag, "02-rack-drawn-selected");

            // ---- a deliberate deviation: outside the snap window the angle is the player's -------------------------------------
            RackPoint press2 = P(ScenarioOneHeadlandGeometry.HeadlandX + 1.2f, -4f), release2 = P(30f, 6f);
            yield return DragWorld(press2, release2);
            Check(manager.PlannedRackCount == 2, tag + ": a second rack was drawn");
            RackPlanRack r2 = Rack(2);
            float a2 = RackEditing.DirectedAngleDegrees(r2.Entrance, r2.Far);
            Check(a2 > 7f && a2 < 11f, tag + ": rack 2 keeps a deliberately skewed angle of " + F(a2) + " deg (outside the 4 deg snap window)");

            // ---- select by click, move by dragging the body -----------------------------------------------------------------
            yield return ClickWorld(OnRack(Rack(1), 0.5f));
            Check(RackEd.SelectedRackId == 1, tag + ": a real click selected rack 1");
            yield return ClickWorld(OnRack(Rack(2), 0.5f));
            Check(RackEd.SelectedRackId == 2, tag + ": a real click selected rack 2");
            Check(Control("map-edit-delete").enabledSelf, tag + ": Delete rack is enabled with a selection");
            RackPlanRack before2 = Rack(2);
            RackPoint grab = OnRack(before2, 0.5f);
            yield return DragWorld(grab, P(grab.X + 6f, grab.Z + 12f), "moving", () => MovingGhost(tag, 2, before2));
            RackPlanRack moved2 = Rack(2);
            Check(moved2.Id == 2 && Near(moved2.Entrance.X, ScenarioOneHeadlandGeometry.HeadlandX, 0.0001f), tag + ": the moved rack keeps id 2 and its entrance re-snapped onto the headland (x unchanged despite the 6 m east drag)");
            Check(Near(moved2.Entrance.Z - before2.Entrance.Z, 12f, 0.7f), tag + ": the rack slid north along the headland by the drag (" + F(moved2.Entrance.Z - before2.Entrance.Z) + " m)");
            Check(Near(moved2.LengthM, before2.LengthM, 0.01f) && Near(moved2.AngleDegrees, before2.AngleDegrees, 0.05f), tag + ": moving kept length and angle");
            Check(manager.CanUndoRackEdit && Control("map-edit-undo").enabledSelf, tag + ": the move can be undone from the card");

            // ---- undo with a real click: exact geometry returns ----------------------------------------------------------------
            yield return Click(Control("map-edit-undo"));
            Check(Same(Rack(2).Entrance, before2.Entrance) && Same(Rack(2).Far, before2.Far), tag + ": 'Undo last rack edit' (real click) restored rack 2 exactly");
            Check(!manager.CanUndoRackEdit && !Control("map-edit-undo").enabledSelf, tag + ": only one level of undo");

            // ---- rotate by the handle: soft snap, then free, then Shift ------------------------------------------------------------
            yield return ClickWorld(OnRack(Rack(1), 0.4f));
            RackPlanRack rot1 = Rack(1);
            Check(RackEd.SelectedRackId == 1 && RackEd.OnHandle(RackEd.WorldToFramePx(rot1.Far), rot1), tag + ": rack 1 is selected and its handle is where the far end is");
            RackPoint nearEast = P(rot1.Far.X, rot1.Far.Z + 1.5f);          // about 1.2 deg off level: inside the window
            yield return DragWorld(rot1.Far, nearEast);
            Check(Same(Rack(1).Entrance, rot1.Entrance) && Near(Rack(1).Far.Z, Rack(1).Entrance.Z, 0.0001f), tag + ": rotating by the handle keeps the entrance, and a near-level release snaps back to level");
            RackPlanRack rot1b = Rack(1);
            RackPoint turned = P(rot1b.Far.X - 4f, rot1b.Far.Z + 13f);      // about 10 deg: outside the window
            yield return DragWorld(rot1b.Far, turned, "rotating", () => RotatingGhost(tag));
            float a1 = RackEditing.DirectedAngleDegrees(Rack(1).Entrance, Rack(1).Far);
            Check(Same(Rack(1).Entrance, rot1.Entrance) && a1 > 6f && a1 < 14f, tag + ": rotating well off level is free (" + F(a1) + " deg), and the entrance did not move");
            Check(Rack(1).Id == 1, tag + ": the rotated rack keeps id 1");
            // Shift turns snapping off: a release 1.2 deg from level stays 1.2 deg.
            yield return Click(Control("map-edit-undo"));                    // back to level for the Shift test
            RackPlanRack lvl = Rack(1);
            Check(Near(lvl.Far.Z, lvl.Entrance.Z, 0.0001f), tag + ": undo returned rack 1 to level");
            bool shiftWorked = true;
            yield return DragWorld(lvl.Far, P(lvl.Far.X, lvl.Far.Z + 1.5f), null, null, true);
            float shiftAngle = RackEditing.DirectedAngleDegrees(Rack(1).Entrance, Rack(1).Far);
            if (!(shiftAngle > 0.4f && shiftAngle < 3f)) shiftWorked = false;
            if (shiftWorked) Check(true, tag + ": holding Shift (real key) turned snapping off: the 1.2 deg release stayed " + F(shiftAngle) + " deg");
            else Note(tag + ": Shift-to-disable-snapping was not observed with the real key (angle " + F(shiftAngle) + " deg): UNVERIFIED");

            // ---- refused proposals: the ghost is red, release changes nothing, the card says why ---------------------------------
            yield return ClickWorld(OnRack(Rack(2), 0.5f));
            Check(RackEd.SelectedRackId == 2, tag + ": rack 2 selected so its handle can be grabbed");
            RackPlanRack ref2 = Rack(2);
            string refusedPlan = PlanSig(), refusedSave = SaveJson();
            yield return DragWorld(ref2.Far, P(ref2.Entrance.X + 2.5f, ref2.Entrance.Z + 0.5f), "refusing", () => RefusedGhost(tag));
            Check(PlanSig() == refusedPlan && SaveJson() == refusedSave, tag + ": a release that makes the rack too short changed nothing (whole save identical)");
            Check(StatusText().Contains("shorter than"), tag + ": the card says why: " + StatusText());
            yield return Capture(tag, "05-after-refusal");
            RackPlanRack ref1 = Rack(1);
            RackPoint overlapAt = OnRack(Rack(2), 0.5f);
            float slide = (ref1.Entrance.Z + 0.9f) - Rack(2).Entrance.Z;       // puts rack 2's entrance 0.9 m from rack 1's
            yield return DragWorld(overlapAt, P(overlapAt.X, overlapAt.Z + slide));
            Check(PlanSig() == refusedPlan, tag + ": dragging a rack onto another is refused and nothing moved (" + StatusText() + ")");
            Check(StatusText().Contains("overlaps rack"), tag + ": the card names the overlap");

            // ---- delete: button, Delete key, Backspace key -------------------------------------------------------------------------
            yield return ClickWorld(OnRack(Rack(2), 0.5f));
            Check(RackEd.SelectedRackId == 2, tag + ": rack 2 selected for deletion");
            string beforeDelete = PlanSig();
            yield return Click(Control("map-edit-delete"));
            Check(Rack(2) == null && manager.PlannedRackCount == 1 && RackEd.SelectedRackId == 0, tag + ": the Delete rack button (real click) deleted rack 2 and cleared the selection");
            yield return Click(Control("map-edit-undo"));
            Check(Rack(2) != null && PlanSig() == beforeDelete, tag + ": Undo last rack edit brought rack 2 back with the same id and geometry");
            yield return ClickWorld(OnRack(Rack(2), 0.5f));
            yield return PressKey("Delete");
            Check(Rack(2) == null, tag + ": the Delete key (real key) deleted the selected rack");
            yield return Click(Control("map-edit-undo"));
            Check(Rack(2) != null && PlanSig() == beforeDelete, tag + ": undo restored it again");
            yield return ClickWorld(OnRack(Rack(1), 0.6f));
            Check(RackEd.SelectedRackId == 1, tag + ": rack 1 selected");
            yield return PressKey("BackSpace");
            Check(Rack(1) == null, tag + ": the Backspace key (real key) deleted the selected rack");
            yield return Click(Control("map-edit-undo"));
            Check(Rack(1) != null && PlanSig() == beforeDelete, tag + ": undo restored rack 1");

            // ---- reset: confirmation first when edits would be lost ----------------------------------------------------------------------
            IReadOnlyList<SuggestedRackSegment> suggestion = manager.GetSuggestedRackLayout();
            string suggestionBefore = SuggestionSig(suggestion);
            string planBeforeReset = PlanSig();
            yield return Click(Control("map-edit-reset"));
            Check(ui.Map.Root.Q("map-edit-reset-panel") != null && Visible(ui.Map.Root.Q("map-edit-reset-panel")) && RackEd.ResetPending, tag + ": 'Reset to suggested layout' asks first because edits would be lost");
            Check(PlanSig() == planBeforeReset, tag + ": nothing changed while the confirmation is showing");
            yield return Capture(tag, "06-reset-confirmation");
            yield return Click(Control("map-edit-reset-cancel"));
            Check(ui.Map.Root.Q("map-edit-reset-panel") == null && PlanSig() == planBeforeReset, tag + ": 'Keep my racks' leaves the plan alone and closes the question");
            yield return Click(Control("map-edit-reset"));
            Check(RackEd.ResetPending, tag + ": the question comes back");
            yield return Click(Control("map-edit-reset-confirm"));
            Check(manager.PlannedRackCount == 5 && manager.RackLayoutMatchesSuggestion(suggestion), tag + ": 'Reset layout' replaced the plan with exactly the five suggested routes");
            Check(SuggestionSig(manager.GetSuggestedRackLayout()) == suggestionBefore, tag + ": the suggestion source was not mutated");
            yield return Capture(tag, "07-after-reset");
            yield return Click(Control("map-edit-reset"));
            Check(!RackEd.ResetPending && StatusText().Contains("already matches"), tag + ": resetting an unedited plan needs no confirmation (" + StatusText() + ")");
            yield return Click(Control("map-edit-undo"));
            Check(PlanSig() == planBeforeReset, tag + ": undo of the reset returned the player's racks with their ids");

            // ---- nothing outside the rack plan moved ----------------------------------------------------------------------------------------
            CheckOutsideRackPlan(outsideBaseline, tag + ": the save and world outside the authoritative rack plan are unchanged after all pointer editing");
            Check(manager.WorkOrders.Count == 0 && manager.MarkedRackEntranceCount == 0, tag + ": no work order and no mark exist");
            yield return Click(Control("map-edit-reset"));
            if (RackEd.ResetPending) yield return Click(Control("map-edit-reset-confirm"));
            Check(manager.PlannedRackCount == 5, tag + ": five racks for the lock test");

            // ---- executed rack, last size only --------------------------------------------------------------------------------------------------
            if (lastSize)
            {
                yield return ExecutedRackPart(tag);
            }

            if (!lastSize)
            {
                yield return Click(Control("map-plan-rack"));
                Check(!ui.Map.RackMode && !RackEd.Active && !Visible(ui.Map.EditLayer), tag + ": a real click on the active tool left planning and hid the pointer layer");
            }
            ui.CloseAll(); yield return Settle(10);
        }
    }

    // Ghost assertions run while the real button is still held down.
    private IEnumerator DrawingGhost(string tag)
    {
        Check(RackEd.Dragging && RackEd.Ghost.HasValue && RackEd.Ghost.Value.Valid, tag + ": while drawing, a valid ghost is shown");
        Check(manager.PlannedRackCount == 0, tag + ": the plan is untouched while the button is still down");
        Check(ui.MapSource.Model.Racks.Any(r => r.Symbol == MapSymbol.RackDraft), tag + ": the ghost is in the map model as a draft rack");
        Check(Near(RackEd.Ghost.Value.Start.X, ScenarioOneHeadlandGeometry.HeadlandX, 0.0001f), tag + ": the ghost's entrance is already on the headland");
        yield return Capture(tag, "03-drawing-ghost");
    }

    private IEnumerator MovingGhost(string tag, int id, RackPlanRack before)
    {
        Check(RackEd.Dragging && RackEd.Ghost.HasValue && RackEd.Ghost.Value.Valid, tag + ": while moving, a valid ghost is shown");
        Check(Same(Rack(id).Entrance, before.Entrance) && Same(Rack(id).Far, before.Far), tag + ": the authoritative rack has not moved while the button is down");
        Check(Near(RackEd.Ghost.Value.Start.X, ScenarioOneHeadlandGeometry.HeadlandX, 0.0001f) && RackEd.Ghost.Value.Start.Z > before.Entrance.Z + 2f, tag + ": the ghost has slid north along the headland");
        yield return Capture(tag, "04-moving-ghost");
    }

    private IEnumerator RotatingGhost(string tag)
    {
        Check(RackEd.Dragging && RackEd.Ghost.HasValue && RackEd.Ghost.Value.Valid, tag + ": while rotating, a valid ghost is shown");
        yield return null;
    }

    private IEnumerator RefusedGhost(string tag)
    {
        Check(RackEd.Dragging && RackEd.Ghost.HasValue && !RackEd.Ghost.Value.Valid, tag + ": a too-short proposal is shown as a refused (red) ghost");
        Check(ui.MapSource.Model.Racks.Any(r => r.Symbol == MapSymbol.RackRefused), tag + ": the refused ghost is in the map model");
        yield return Capture(tag, "04b-refused-ghost");
    }

    private IEnumerator ExecutedRackPart(string tag)
    {
        // Confirm the five-rack plan, execute one rack's removal (the same field the real completion sets), reopen around it.
        Check(manager.ConfirmRackPlan().Succeeded, tag + ": plan confirmed for the lock test");
        int lockedId = manager.RackPlan[2].Id;
        ScenarioOneWorkOrder executed = manager.WorkOrders.First(o => o.rackId == lockedId);
        executed.status = ScenarioWorkStatus.Completed; executed.resolvedYear = 0;
        Check(manager.RackIsLocked(lockedId), tag + ": an executed removal locks rack " + lockedId);
        ui.MapSource.MarkRacksDirty();
        ui.Map.Refresh(true);
        yield return Settle(10);
        Check(!RackEd.Active && !Visible(ui.Map.EditLayer), tag + ": a confirmed plan hides the pointer layer");
        Check(ui.Map.Root.Q<Label>("map-edit-counter") != null, tag + ": the card still shows the derived rack count");
        string confirmedPlan = PlanSig();
        Vector2 press = ScreenOfWorld(OnRack(Rack(lockedId), 0.5f));
        yield return Do(new[] { Move(press), Sleep(150), Down(), Sleep(100), Move(press + new Vector2(0, -40)), Sleep(150), Up(), Sleep(200) });
        Check(PlanSig() == confirmedPlan, tag + ": a real drag on a confirmed plan changes nothing");

        Check(manager.ReopenRackPlan().Succeeded, tag + ": plan reopened around the executed rack");
        ui.MapSource.MarkRacksDirty();
        ui.Map.Refresh(true);
        yield return Settle(8);
        Check(RackEd.Active && ui.MapSource.Model.Racks.Any(r => r.RackId == lockedId && r.Symbol == MapSymbol.RackHarvested && r.Locked), tag + ": the executed rack is drawn as harvested");
        string lockedPlan = PlanSig(), lockedOrders = OrderSig();
        yield return ClickWorld(OnRack(Rack(lockedId), 0.5f));
        Check(RackEd.SelectedRackId == lockedId && StatusText().Contains("harvested"), tag + ": a real click selects the harvested rack and the card says it is locked (" + StatusText() + ")");
        Check(!Control("map-edit-delete").enabledSelf, tag + ": Delete rack is disabled for an executed rack");
        RackPlanRack lr = Rack(lockedId);
        yield return DragWorld(OnRack(lr, 0.5f), P(OnRack(lr, 0.5f).X, OnRack(lr, 0.5f).Z + 8f));
        Check(PlanSig() == lockedPlan && OrderSig() == lockedOrders, tag + ": a real drag on an executed rack moved nothing");
        yield return DragWorld(lr.Far, P(lr.Far.X - 20f, lr.Far.Z + 5f));
        Check(PlanSig() == lockedPlan && OrderSig() == lockedOrders, tag + ": there is no handle to rotate an executed rack");
        yield return PressKey("Delete");
        Check(Rack(lockedId) != null && PlanSig() == lockedPlan && OrderSig() == lockedOrders, tag + ": the Delete key cannot delete an executed rack");
        yield return Capture(tag, "08-harvested-locked");

        int free = manager.RackPlan.First(r => !r.IsLocked).Id;
        RackPlanRack fr = Rack(free);
        yield return ClickWorld(OnRack(fr, 0.5f));
        yield return DragWorld(OnRack(fr, 0.5f), P(OnRack(fr, 0.5f).X, OnRack(fr, 0.5f).Z + 3f));
        Check(Rack(free).Entrance.Z > fr.Entrance.Z + 1.5f && OrdersUnique(), tag + ": another rack is still movable by the pointer, with its pending work reconciled and not duplicated");
        Check(manager.WorkOrders.Count(o => o.rackId == lockedId && o.status == ScenarioWorkStatus.Completed) == 1, tag + ": the executed order is untouched");
    }
}
#endif
