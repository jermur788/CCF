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

// Disposable RACK-V2-07 Unity gate (confirm, reopen and minimap). Copy into Assets/ForestPrototype, run Begin, remove it and its .meta
// (Tools/Verification/RackV2ConfirmMinimap/run_gate.py does all three).
//
// Part A, authoritative semantics, no pointer: the exact-five-and-valid confirmation gate, confirm exactly once creating pending work only
// (no approval, execution or marks), the derived counter switch from Racks planned to Rack entrances marked, reopen with pending work and
// with approved-but-unexecuted work (warning, decline changes nothing, consent cancels and recreates only the edited rack), per-rack mark
// invalidation, an executed rack staying locked while the others stay editable, and the model-0 path never entering V2 confirm/reopen/
// minimap. It also proves the shared map model reads only the authoritative sources (suggestions, rack objects) and that painting
// either map changes no authoritative state.
//
// Part B, rendered and driven by the REAL desktop pointer (xdotool in a private display, Tools/Verification/RackV2Editing/pointer_driver.py):
// the first-time minimap note and dashed guidance, the Stand Map Confirm/Reopen/consent controls, the Objective 1 counter text, the
// minimap's solid confirmed plan and pale harvested rack, captured at each requested resolution. The full flow runs at the first size;
// later sizes repeat the note, guidance, confirm and solid-minimap states.
//
// Run: -executeMethod RackV2ConfirmMinimapVerification.Begin   CCF_DIAG_DIR=<outside the repo>   CCF_UI_SIZES=1280x720,1920x1080
//      CCF_DRIVER_DIR=<folder shared with pointer_driver.py>
// Writes only its evidence folder. No Save(), scene save, annual advance or archive write.
internal static class RackPlanResultExtensions
{
    public static bool Applied(this RackPlanOperationResult result) => result.Status == RackPlanOperationStatus.Applied;
}

public static class RackV2ConfirmMinimapVerification
{
    public const string Marker = "RackV2ConfirmMinimapVerification.Begin";

    public static void Begin()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        Vector2Int[] sizes = RackV2ConfirmMinimapRunner.Sizes();
        RackV2ConfirmMinimapRunner.SetGameSize(sizes[0].x, sizes[0].y);
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
            new GameObject("Disposable RACK-V2-07 confirm and minimap gate").AddComponent<RackV2ConfirmMinimapRunner>();
    }
}

[Serializable] public class PointerOp { public string op; public int x; public int y; public int ms; public string name; public string key; }
[Serializable] public class PointerRequest { public int seq; public PointerOp[] ops; }

public sealed class RackV2ConfirmMinimapRunner : MonoBehaviour
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
    // Every pointer event the edit layer sees (trickle-down, before the editor's own handlers), with the frame it arrived in: lets a
    // failure be read as "the Editor delivered these events at these times" instead of guessed at.
    private readonly List<string> pointerTrace = new List<string>();
    private bool traceRegistered;
    private int pointerDownCount;
    private RackPoint lastPointerDownWorld;

    private void RegisterPointerTrace()
    {
        if (traceRegistered) return;
        traceRegistered = true;
        VisualElement layer = ui.Map.EditLayer;
        layer.RegisterCallback<PointerDownEvent>(e =>
        {
            pointerDownCount++;
            lastPointerDownWorld = RackEd.FramePxToWorld(e.localPosition);
            Trace("down", e.localPosition, e.button);
        }, TrickleDown.TrickleDown);
        layer.RegisterCallback<PointerMoveEvent>(e => Trace("move", e.localPosition, e.pressedButtons), TrickleDown.TrickleDown);
        layer.RegisterCallback<PointerUpEvent>(e => Trace("up", e.localPosition, e.button), TrickleDown.TrickleDown);
    }

    private void Trace(string kind, Vector2 local, int info)
    {
        RackPoint w = RackEd.FramePxToWorld(local);
        pointerTrace.Add(string.Format(Inv, "t={0:F3} frame={1} {2} info={3} local=({4:F1},{5:F1}) world=({6:F2},{7:F2}) mouse={8} ghostZ={9}",
            Time.realtimeSinceStartup, Time.frameCount, kind, info, local.x, local.y, w.X, w.Z,
            Mouse.current != null ? Mouse.current.position.ReadValue().ToString() : "none",
            RackEd.Ghost.HasValue ? RackEd.Ghost.Value.Start.Z.ToString("F2", Inv) : "-"));
    }
    private StandMapRackEditor RackEd => ui.Map.RackEditor;
    private ScenarioOneManager M => manager;

    private void Check(bool ok, string message)
    {
        if (!ok) throw new InvalidOperationException(message);
        checks++;
        Debug.Log("RACKV2CM_CHECK " + message);
    }

    private void Note(string text) { notes.Add(text); Debug.Log("RACKV2CM_NOTE " + text); }
    private void State(string text) { stateLines.Add("  " + text); Debug.Log("RACKV2CM_STATE " + text); }

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

    // The Editor draws a few frames a second under software rendering and applies queued pointer events in batches, so a state read the
    // instant a gesture reaches a waypoint can be a frame or two behind the pointer. Wait (bounded) for the state, then assert it.
    private static IEnumerator WaitFor(Func<bool> condition, float seconds = 5f)
    {
        double until = Time.realtimeSinceStartupAsDouble + seconds;
        while (!condition() && Time.realtimeSinceStartupAsDouble < until) yield return null;
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
        if (now == baseline) { checks++; Debug.Log("RACKV2CM_CHECK " + message); return; }
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
        if (string.IsNullOrEmpty(output)) output = Path.Combine(Application.temporaryCachePath, "rack-v2-confirm-minimap");
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
        File.WriteAllLines(Path.Combine(output, "pointer-trace.txt"), pointerTrace);
        if (!string.IsNullOrEmpty(driverDir)) File.WriteAllText(Path.Combine(driverDir, "finished"), failure == null ? "pass" : "fail");
        Debug.Log(failure == null ? "RACK_V2_CONFIRM_MINIMAP_UNITY_PASS checks=" + checks + " shots=" + shots
            : "RACK_V2_CONFIRM_MINIMAP_UNITY_FAIL checks=" + checks + " shots=" + shots + " exception=" + failure);
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
    // Part A — authoritative semantics and the shared sources, no pointer
    // =================================================================================================================
    private static string Marks(ScenarioOneManager m) => string.Join(",", m.RackPlan.Select(r => r.Id + ":" + (r.EntranceMarked ? "1" : "0")));

    private static void ResetTeachingNote(HudMinimapView minimap)
    {
        BindingFlags all = BindingFlags.Instance | BindingFlags.NonPublic;
        typeof(HudMinimapView).GetField("noteDone", all).SetValue(minimap, false);
        typeof(HudMinimapView).GetField("noteSeconds", all).SetValue(minimap, 0f);
        typeof(HudMinimapView).GetField("noteLastTick", all).SetValue(minimap, -1f);
    }

    private StandMapModel Remodel()
    {
        ui.MapSource.MarkRacksDirty();
        return ui.MapSource.Rebuild();
    }

    private IEnumerator PartA()
    {
        IReadOnlyList<SuggestedRackSegment> suggestion = manager.GetSuggestedRackLayout();
        Check(suggestion.Count == 5, "five suggested routes");
        string suggestionBefore = SuggestionSig(suggestion);
        string baseSave = SaveJson();

        // ---- the confirmation gate: exactly five, and valid ---------------------------------------------------------
        Check(!manager.CanConfirmRackPlan(out string reason0) && reason0.Contains("exactly 5") && reason0.Contains("has 0"), "confirmation is refused with no racks, with the honest reason: " + reason0);
        RackPlanOperationResult none = manager.ConfirmRackPlanFromMap();
        Check(none.Status == RackPlanOperationStatus.Invalid && SaveJson() == baseSave && !manager.RackPlanConfirmed, "confirming an empty plan is refused and changes nothing");

        Check(manager.ResetRacksToSuggestion(suggestion).Succeeded && manager.PlannedRackCount == 5, "five draft racks from the suggested layout");
        int[] ids = manager.RackPlan.Select(r => r.Id).ToArray();
        Check(manager.CanConfirmRackPlan(out string reason5) && reason5 == "", "five valid racks may be confirmed");
        Check(manager.WorkOrders.Count(o => o.rackId > 0) == 0, "drafting created no work order");

        Check(manager.DeleteRackFromMap(ids[4]).Applied() && manager.PlannedRackCount == 4, "a rack is deleted: four remain");
        string fourSave = SaveJson();
        Check(!manager.CanConfirmRackPlan(out string reason4) && reason4.Contains("has 4"), "four racks cannot be confirmed: " + reason4);
        Check(manager.ConfirmRackPlanFromMap().Status == RackPlanOperationStatus.Invalid && SaveJson() == fourSave, "confirming four racks is refused and changes nothing (also through ConfirmRackPlan)");
        Check(manager.ConfirmRackPlan().Status == RackPlanOperationStatus.Invalid && SaveJson() == fourSave, "the authoritative ConfirmRackPlan refuses four racks too");
        Check(manager.UndoLastRackEdit().Succeeded && manager.PlannedRackCount == 5 && Rack(ids[4]) != null, "undo brings the deleted rack back with its id");
        RackPlanOperationResult sixth = manager.CreateRackFromMap(E(-34f), P(38f, -34f));
        Check(sixth.Status == RackPlanOperationStatus.Invalid && manager.PlannedRackCount == 5 && manager.CanConfirmRackPlan(out _), "a sixth rack is refused, so more than five can never be confirmed");

        // An invalid rack (fixture: an entrance moved off the headland) blocks confirmation with the rack named.
        RackSaveData first = manager.Racks.First(r => r.rackId == ids[0]);
        float savedX = first.entranceX;
        first.entranceX = -20f;
        Check(!manager.CanConfirmRackPlan(out string invalidReason) && invalidReason.Contains("Rack " + ids[0]), "an invalid rack blocks confirmation and is named: " + invalidReason);
        Check(manager.ConfirmRackPlanFromMap().Status == RackPlanOperationStatus.Invalid && !manager.RackPlanConfirmed, "confirming a plan with an invalid rack is refused");
        first.entranceX = savedX;
        Check(manager.CanConfirmRackPlan(out _), "the fixture is restored: the plan is confirmable again");

        // ---- the minimap and the Stand Map read the same sources while the plan is a draft ----------------------------------
        yield return MapSourcesBeforeConfirm(suggestion, ids);

        // ---- confirm: once, pending work only, no approval, no execution, no marks ---------------------------------------
        string saveBeforeRead = SaveJson(), planBeforeRead = PlanSig();
        Remodel(); ui.Minimap.Refresh(); ui.Map.Refresh(true);
        Check(SaveJson() == saveBeforeRead && PlanSig() == planBeforeRead, "building the map model and painting both maps changed no authoritative state");
        RackPlanOperationResult confirmed = manager.ConfirmRackPlanFromMap();
        Check(confirmed.Status == RackPlanOperationStatus.Applied && manager.RackPlanConfirmed && manager.ConfirmedRacks.Count == 5, "a valid five-rack plan confirms: " + confirmed.Message);
        Check(manager.WorkOrders.Count(o => o.rackId > 0) > 0 && manager.WorkOrders.Where(o => o.rackId > 0).All(o => o.status == ScenarioWorkStatus.Pending),
            "confirming created pending rack work only (nothing approved, nothing executed)");
        Check(OrdersUnique() && manager.RackPlan.All(r => r.Orders.Open > 0 && r.Orders.Approved == 0 && !r.IsLocked), "one order per tree and every rack owns pending work");
        Check(manager.MarkedRackEntranceCount == 0 && manager.RackPlan.All(r => !r.EntranceMarked), "confirming marked no entrance");
        yield return MapSourcesAfterConfirm();
        string confirmedOrders = OrderSig(), confirmedSave = SaveJson();
        Check(manager.ConfirmRackPlanFromMap().Status == RackPlanOperationStatus.Invalid && manager.ConfirmRackPlan().Status == RackPlanOperationStatus.NoChange
            && OrderSig() == confirmedOrders && SaveJson() == confirmedSave, "confirming again is a no-op: no duplicate orders, the save is byte-identical");
        Check(!manager.RackEditingOpen && manager.CreateRackFromMap(E(-34f), P(38f, -34f)).Status == RackPlanOperationStatus.Invalid, "a confirmed plan is closed to geometry editing");

        // ---- the counter switches and is derived -------------------------------------------------------------------------
        Check(ui.RackLayout.V2CounterText == "Rack entrances marked: 0 / 5", "after confirmation the counter reads Rack entrances marked: " + ui.RackLayout.V2CounterText);
        Check(ui.Learning.CurrentInstruction.Contains("Rack entrances marked: 0 / 5") && !ui.Learning.CurrentInstruction.Contains("Racks planned"), "the Objective 1 text switched to entrances marked");
        Check(ui.Learning.Summary.Contains("Rack entrances marked: 0 / 5"), "the objective summary line switched too");
        foreach (int id in ids.Take(4)) Check(manager.MarkRackEntrance(id).Status == RackPlanOperationStatus.Applied, "fixture: entrance of rack " + id + " marked through the authoritative API");
        Check(manager.MarkedRackEntranceCount == manager.Racks.Count(r => r.entranceMarked) && manager.MarkedRackEntranceCount == 4
            && ui.RackLayout.V2CounterText == "Rack entrances marked: 4 / 5", "the marked counter is the number of marked rack objects: " + ui.RackLayout.V2CounterText);

        // ---- reopen with pending work and zero executed removals ---------------------------------------------------------
        ScenarioOneManager.RackReopenSummary s0 = manager.DescribeRackReopen();
        Check(s0.RackCount == 5 && s0.LockedRacks == 0 && s0.EditableRacks == 5 && s0.RacksWithApprovedWork == 0 && !s0.NothingEditable, "the reopen summary: five editable racks, none approved, none locked");
        string ordersAtReopen = OrderSig(), marksAtReopen = Marks(manager);
        Check(manager.ReopenRackPlan().Status == RackPlanOperationStatus.Applied && !manager.RackPlanConfirmed && manager.RackEditingOpen, "the plan reopens");
        Check(OrderSig() == ordersAtReopen && Marks(manager) == marksAtReopen, "reopening alone created, removed or changed no order and cleared no mark");
        Check(ui.RackLayout.V2CounterText == "Racks planned: 5 / 5" && manager.RackPlan.Count(r => r.EntranceMarked) == 4, "the counter returns to Racks planned while the four marks are kept in the rack objects");
        Check(manager.ReopenRackPlan().Status == RackPlanOperationStatus.NoChange, "reopening an open plan is a no-op");
        Check(manager.ConfirmRackPlanFromMap().Applied() && OrderSig() == ordersAtReopen && OrdersUnique() && manager.MarkedRackEntranceCount == 4 && Marks(manager) == marksAtReopen,
            "reopen then confirm unchanged fabricates and duplicates nothing: the same orders, the same marks");

        // ---- change one rack: only its mark goes -----------------------------------------------------------------------------
        Check(manager.ReopenRackPlan().Succeeded, "reopened again");
        int a = ids[0];
        RackPlanRack a0 = Rack(a);
        string otherOrders = OtherOrdersSig(a);
        RackPlanOperationResult edited = manager.ReshapeRackFromMap(a, a0.Entrance, P(a0.Far.X - 6f, a0.Far.Z));
        Check(edited.Status == RackPlanOperationStatus.Applied && edited.MarksCleared == 1, "changing the marked rack applies and reports exactly one mark cleared: " + edited.Message);
        Check(!Rack(a).EntranceMarked && ids.Skip(1).Take(3).All(i => Rack(i).EntranceMarked) && !Rack(ids[4]).EntranceMarked, "only rack " + a + " lost its mark; the other racks keep theirs");
        Check(OtherOrdersSig(a) == otherOrders && OrdersUnique() && manager.WorkOrders.Where(o => o.rackId == a).All(o => o.status == ScenarioWorkStatus.Pending), "only that rack's pending orders were reconciled, without duplicates");
        Check(manager.ConfirmRackPlanFromMap().Applied() && manager.MarkedRackEntranceCount == 3 && ui.RackLayout.V2CounterText == "Rack entrances marked: 3 / 5", "reconfirmed: three marks remain and the counter says so");

        // ---- approved but unexecuted work: warn, consent, decline ---------------------------------------------------------------
        Check(manager.ApprovePendingWork(), "fixture: the pending work is approved");
        ScenarioOneManager.RackReopenSummary s1 = manager.DescribeRackReopen();
        Check(s1.RacksWithApprovedWork == 5 && s1.LockedRacks == 0, "the reopen summary counts five racks with approved work");
        Check(manager.ReopenRackPlan().Succeeded, "the plan reopens with approved work outstanding (reopening cancels nothing)");
        int b = ids[1];
        RackPlanRack b0 = Rack(b);
        string approvedOrders = OrderSig(), approvedPlan = PlanSig(), approvedMarks = Marks(manager);
        RackPlanOperationResult warned = manager.ReshapeRackFromMap(b, b0.Entrance, P(b0.Far.X - 6f, b0.Far.Z));
        Check(warned.RequiresApprovalWarning && warned.Message.Contains("approved"), "editing a rack with approved work asks first: " + warned.Message);
        Check(OrderSig() == approvedOrders && PlanSig() == approvedPlan && Marks(manager) == approvedMarks && !manager.CanUndoRackEdit, "the warning changed nothing; declining is simply not repeating the call");
        RackPlanOperationResult consented = manager.ReshapeRackFromMap(b, b0.Entrance, P(b0.Far.X - 6f, b0.Far.Z), true);
        Check(consented.Status == RackPlanOperationStatus.Applied && consented.OrdersCancelled > 0 && consented.MarksCleared == 1, "with consent the approved work is cancelled and recreated and one mark clears: " + consented.Message);
        Check(manager.WorkOrders.Where(o => o.rackId == b && o.IsOpen).All(o => o.status == ScenarioWorkStatus.Pending) && OrdersUnique(), "the recreated work is pending, once per tree");
        Check(manager.WorkOrders.Where(o => o.rackId > 0 && o.rackId != b && o.IsOpen).All(o => o.status == ScenarioWorkStatus.Approved), "the other racks keep their approved work");
        Check(Rack(ids[2]).EntranceMarked && Rack(ids[3]).EntranceMarked && !Rack(b).EntranceMarked, "only rack " + b + " lost its mark");
        Check(manager.ConfirmRackPlanFromMap().Applied() && manager.MarkedRackEntranceCount == 2, "reconfirmed after consent: two marks remain");

        // ---- an executed removal locks that rack; the others stay editable ---------------------------------------------------
        int lockedId = ids[3];
        ScenarioOneWorkOrder executed = manager.WorkOrders.First(o => o.rackId == lockedId);
        int executedOrderId = executed.workOrderId;
        executed.status = ScenarioWorkStatus.Completed;
        executed.resolvedYear = 0;
        ScenarioOneManager.RackReopenSummary s2 = manager.DescribeRackReopen();
        Check(manager.RackIsLocked(lockedId) && s2.LockedRacks == 1 && s2.EditableRacks == 4 && !s2.NothingEditable, "fixture: one executed removal locks one rack and leaves four editable");
        Check(manager.ReopenRackPlan().Succeeded && manager.RackEditingOpen, "the plan reopens around the locked rack (the accepted per-rack boundary needs no second lifecycle)");
        string lockedPlan = PlanSig(), lockedOrders = OrderSig(), lockedMarks = Marks(manager);
        RackPlanRack l0 = Rack(lockedId);
        Check(manager.ReshapeRackFromMap(lockedId, l0.Entrance, P(l0.Far.X - 5f, l0.Far.Z), true).Status == RackPlanOperationStatus.Locked, "the executed rack cannot be moved, even with consent");
        Check(manager.DeleteRackFromMap(lockedId, true).Status == RackPlanOperationStatus.Locked, "the executed rack cannot be deleted, even with consent");
        Check(PlanSig() == lockedPlan && OrderSig() == lockedOrders && Marks(manager) == lockedMarks && Rack(lockedId).EntranceMarked, "the refusals changed no geometry, order or mark: a geometry edit cannot invalidate a locked rack's state");
        int free = ids[4];
        RackPlanRack f0 = Rack(free);
        Check(manager.ReshapeRackFromMap(free, f0.Entrance, P(f0.Far.X - 4f, f0.Far.Z), true).Succeeded, "another rack stays editable while the locked one is refused");
        RackPlanOperationResult reconfirm = manager.ConfirmRackPlanFromMap(true);
        Check(reconfirm.Succeeded && manager.RackPlanConfirmed, "the plan confirms again around the executed rack: " + reconfirm.Message);
        Check(manager.WorkOrders.Count(o => o.rackId == lockedId && o.status == ScenarioWorkStatus.Completed) == 1
            && manager.WorkOrders.Any(o => o.workOrderId == executedOrderId && o.status == ScenarioWorkStatus.Completed) && OrdersUnique(),
            "the executed removal is untouched: still exactly one completed order, never reversed or recreated");
        Check(Rack(lockedId).IsLocked && Rack(lockedId).EntranceMarked, "the locked rack is still locked and keeps its mark");
        foreach (ScenarioOneWorkOrder order in manager.WorkOrders.Where(o => o.rackId > 0 && o.rackId != lockedId).ToList())
        { order.status = ScenarioWorkStatus.Completed; order.resolvedYear = 0; }
        Check(manager.DescribeRackReopen().NothingEditable && manager.DescribeRackReopen().LockedRacks == 5, "fixture: with every rack executed there is nothing left to reopen");

        yield return MapSourcesAfterExecution(lockedId);

        // ---- the legacy / model-0 path never enters V2 confirmation, reopen or the minimap curriculum -------------------------
        yield return ReloadFresh();
        ForestSaveData legacy = saves.CaptureData();
        legacy.version = 20;
        legacy.scenarioOne.rackPlanningModel = RackPlanningModel.Legacy;
        legacy.scenarioOne.rackPlanConfirmed = false;
        legacy.scenarioOne.nextRackId = 1;
        legacy.scenarioOne.racks = new List<RackSaveData>();
        foreach (ScenarioOneWorkOrder order in legacy.scenarioOne.workOrders) order.rackId = 0;
        Check(saves.LoadData(legacy, false), "a projected pre-v21 (model 0) save loads");
        yield return Settle(8);
        Check(!manager.RackPlanningV2Active && !ui.RackLayout.RackPlanV2, "a model-0 world is not a V2 world");
        string legacySave = SaveJson();
        Check(!manager.CanConfirmRackPlan(out string legacyReason) && legacyReason.Contains("not active"), "confirmation is unavailable in a model-0 world: " + legacyReason);
        Check(manager.ConfirmRackPlanFromMap().Status == RackPlanOperationStatus.Invalid && manager.ReopenRackPlan().Status == RackPlanOperationStatus.Invalid, "confirm and reopen are refused in a model-0 world");
        Check(manager.DescribeRackReopen().RackCount == 0 && SaveJson() == legacySave, "nothing changed in the model-0 world");
        StandMapModel legacyModel = Remodel();
        Check(!legacyModel.Racks.Any(r => r.Symbol == MapSymbol.RackSuggested || r.Symbol == MapSymbol.RackPlayer || r.Symbol == MapSymbol.RackHarvested), "no V2 rack symbol enters a model-0 map model");
        ui.RackLayout.Update(true);
        legacyModel = Remodel();
        Check(legacyModel.Racks.Count(r => r.LayoutIndex >= 0) == ui.RackLayout.Lines.Count, "the model-0 map model still carries exactly the authored layout lines (" + ui.RackLayout.Lines.Count + ")");
        ResetTeachingNote(ui.Minimap); ui.Minimap.Refresh();
        Check(!ui.Minimap.TeachingNoteVisible && ui.Minimap.TeachingNoteText == "", "the dashed/solid note never appears in a model-0 world");
        Check(!ui.Learning.CurrentInstruction.Contains("Racks planned") && !ui.Learning.CurrentInstruction.Contains("Rack entrances marked") && !ui.Learning.Summary.Contains("Rack entrances"), "the model-0 objective keeps its own wording");
        State("part A complete: confirm gate, counters, reopen, consent, executed lock, marks and legacy isolation verified");
        yield return null;
    }

    // The two presentations read the shared model, which reads the authoritative sources only.
    private IEnumerator MapSourcesBeforeConfirm(IReadOnlyList<SuggestedRackSegment> suggestion, int[] ids)
    {
        StandMapModel m = Remodel();
        var suggested = m.MiniGuidance.ToList();
        Check(suggested.Count == 5 && suggested.All(r => r.Symbol == MapSymbol.RackSuggested) && !m.Racks.Any(r => r.Symbol == MapSymbol.RackSuggested),
            "an open plan carries five suggestions as minimap guidance in the shared model, and Racks holds none outside rack planning");
        Check(suggested.Select(r => F(r.Start.X) + "," + F(r.Start.Z) + ">" + F(r.End.X) + "," + F(r.End.Z)).SequenceEqual(
            suggestion.Select(x => F(x.Entrance.X) + "," + F(x.Entrance.Z) + ">" + F(x.Far.X) + "," + F(x.Far.Z))), "the model's suggestions are the foundation's routes, not a copy");
        Check(!m.Racks.Any(r => r.LayoutIndex >= 0 || r.Symbol == MapSymbol.RackOther), "a V2 model carries no legacy layout line or session rack group");
        var draft = m.Racks.Where(r => r.Symbol == MapSymbol.RackPlayer).ToList();
        Check(draft.Count == 5 && draft.All(r => r.FullOnly) && draft.Select(r => r.RackId).OrderBy(x => x).SequenceEqual(ids.OrderBy(x => x)), "draft racks are the plan's own objects and are marked full-map-only");
        List<MapDrawItem> mini = StandMapDrawPlan.BuildStatic(m, 196f, MapDetail.Mini), full = StandMapDrawPlan.BuildStatic(m, 688f, MapDetail.Full);
        Check(mini.Count(i => i.Symbol == MapSymbol.RackSuggested) == 5 && mini.Where(i => i.Symbol == MapSymbol.RackSuggested).All(i => i.DashPx > 0 && i.GapPx > 0),
            "the minimap draws the five suggestions dashed");
        Check(mini.Count(i => i.Symbol == MapSymbol.RackPlayer) == 0, "the minimap draws no solid line for a draft plan");
        Check(full.Count(i => i.Symbol == MapSymbol.RackSuggested) == 0 && full.Count(i => i.Symbol == MapSymbol.RackPlayer) == 5, "the Stand Map outside planning draws the draft racks and hides the guidance");
        Check(SuggestionSig(manager.GetSuggestedRackLayout()) == SuggestionSig(suggestion), "the suggestion source is not mutated by drawing it");
        yield return null;
    }

    private IEnumerator MapSourcesAfterConfirm()
    {
        StandMapModel m = Remodel();
        Check(!m.Racks.Any(r => r.Symbol == MapSymbol.RackSuggested) && m.MiniGuidance.Count == 0, "a confirmed plan removes the dashed guidance from the model");
        var player = m.Racks.Where(r => r.Symbol == MapSymbol.RackPlayer || r.Symbol == MapSymbol.RackHarvested).ToList();
        Check(player.Count == 5 && player.All(r => !r.FullOnly), "the confirmed plan's five racks are drawn on both maps");
        foreach (RackPlanRack rack in manager.RackPlan)
        {
            MapRack shown = player.Single(r => r.RackId == rack.Id);
            Check(Same(shown.Start, rack.Entrance) && Same(shown.End, rack.Far), "rack " + rack.Id + ": the map draws exactly the authoritative geometry");
        }
        List<MapDrawItem> mini = StandMapDrawPlan.BuildStatic(m, 196f, MapDetail.Mini), full = StandMapDrawPlan.BuildStatic(m, 688f, MapDetail.Full);
        var miniIds = mini.Where(i => i.Symbol == MapSymbol.RackPlayer || i.Symbol == MapSymbol.RackHarvested).Select(i => m.Racks[i.SourceIndex].RackId).OrderBy(x => x).ToList();
        var fullIds = full.Where(i => i.Symbol == MapSymbol.RackPlayer || i.Symbol == MapSymbol.RackHarvested).Select(i => m.Racks[i.SourceIndex].RackId).OrderBy(x => x).ToList();
        Check(miniIds.SequenceEqual(fullIds) && miniIds.SequenceEqual(manager.RackPlan.Select(r => r.Id).OrderBy(x => x)), "the minimap and the Stand Map draw the same rack identities");
        Check(mini.Where(i => i.Symbol == MapSymbol.RackPlayer).All(i => i.DashPx == 0 && i.GapPx == 0) && mini.Count(i => i.Symbol == MapSymbol.RackSuggested) == 0, "the confirmed plan is solid on the minimap and no suggestion remains");
        yield return null;
    }

    private IEnumerator MapSourcesAfterExecution(int lockedId)
    {
        StandMapModel m = Remodel();
        List<MapDrawItem> mini = StandMapDrawPlan.BuildStatic(m, 196f, MapDetail.Mini), full = StandMapDrawPlan.BuildStatic(m, 688f, MapDetail.Full);
        Check(mini.Count(i => i.Symbol == MapSymbol.RackHarvested) == 5 && full.Count(i => i.Symbol == MapSymbol.RackHarvested) == 5 && mini.Count(i => i.Symbol == MapSymbol.RackPlayer) == 0,
            "executed racks are pale harvested strips on both maps (rack " + lockedId + " among them)");
        yield return null;
    }

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
        // A successful screenshot API call can still return a black texture on this Editor/display combination. Never count that as
        // rendered evidence. Sample across the image; the map and its panels must contribute visible pixels.
        bool visible = false;
        for (int y = 0; y < image.height && !visible; y += Math.Max(1, image.height / 32))
            for (int x = 0; x < image.width && !visible; x += Math.Max(1, image.width / 32))
            {
                Color c = image.GetPixel(x, y);
                visible = c.r > 0.01f || c.g > 0.01f || c.b > 0.01f;
            }
        Destroy(image);
        if (!visible) throw new InvalidOperationException("capture is black: " + file);
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
        yield return Do(new[] { Move(at), Sleep(300), Move(at + new Vector2(1, 0)), Sleep(300), Move(at), Sleep(500), Down(), Sleep(250), Up(), Sleep(300) });
        yield return Wait(0.5f);
        yield return Settle(6);
    }

    private Button Control(string name) => ui.Map.Root.Q<Button>(name);
    private string StatusText() { Label l = ui.Map.Root.Q<Label>("map-edit-status"); return l != null ? l.text : ""; }

    // A drag in world coordinates, in steps, optionally holding Shift and pausing mid-way for a capture.
    // syncAt = the step after which the button is held for the harness to inspect (default: two thirds of the way; steps = at the end,
    // for a state that only exists at the destination, such as a too-short proposal).
    private IEnumerator DragWorld(RackPoint from, RackPoint to, string syncName = null, Func<IEnumerator> onSync = null, bool shift = false, int steps = 14, int syncAt = -1)
    {
        if (syncAt < 0) syncAt = steps * 2 / 3;
        var ops = new List<PointerOp>();
        int downBefore = pointerDownCount;
        Vector2 a = ScreenOfWorld(from), b = ScreenOfWorld(to);
        ops.Add(Move(a)); ops.Add(Sleep(300)); ops.Add(Move(a + Vector2.one)); ops.Add(Sleep(300)); ops.Add(Move(a)); ops.Add(Sleep(500));
        if (shift) ops.Add(KeyDown("shift"));
        ops.Add(Down()); ops.Add(Sync("pressed"));
        for (int i = 1; i <= steps; i++)
        {
            ops.Add(Move(Vector2.Lerp(a, b, (float)i / steps), 0));
            ops.Add(Sleep(35));
            // Repeat the final waypoint before any end-of-drag inspection; the low-frame-rate Editor can coalesce the last move.
            if (i == steps)
            {
                ops.Add(Sleep(300));
                ops.Add(Move(b + Vector2.one));
                ops.Add(Sleep(300));
                ops.Add(Move(b));
                ops.Add(Sleep(300));
            }
            if (syncName != null && i == syncAt) { ops.Add(Sleep(500)); ops.Add(Sync(syncName)); }
        }
        ops.Add(Sleep(900));
        ops.Add(Up());
        if (shift) ops.Add(KeyUp("shift"));
        ops.Add(Sleep(200));
        var syncs = new Dictionary<string, Func<IEnumerator>>();
        syncs["pressed"] = () => WaitForPointerDown(downBefore, from);
        if (syncName != null && onSync != null) syncs[syncName] = onSync;
        yield return Do(ops.ToArray(), syncs);
        yield return Wait(0.5f);
        yield return Settle(6);
    }

    private IEnumerator WaitForPointerDown(int before, RackPoint intended)
    {
        yield return WaitFor(() => pointerDownCount > before);
        Check(pointerDownCount > before, "the map received the real pointer down before the drag moved");
        Check(RackGeometry.LengthM(lastPointerDownWorld, intended) < 1f, "the real pointer down landed at the intended drag start");
    }

    private IEnumerator ClickWorld(RackPoint at)
    {
        Vector2 s = ScreenOfWorld(at);
        yield return Do(new[] { Move(s), Sleep(300), Move(s + Vector2.right), Sleep(300), Move(s), Sleep(500), Down(), Sleep(250), Up(), Sleep(300) });
        yield return Wait(0.5f);
        yield return Settle(6);
    }

    private IEnumerator PressKey(string key)
    {
        yield return Do(new[] { Sleep(100), KeyTap(key), Sleep(250) });
        yield return Wait(0.5f);
        yield return Settle(6);
    }

    // A point on a rack's centre line, a given distance from its entrance.
    private RackPoint OnRack(RackPlanRack rack, float fraction)
        => P(rack.Entrance.X + (rack.Far.X - rack.Entrance.X) * fraction, rack.Entrance.Z + (rack.Far.Z - rack.Entrance.Z) * fraction);


    // =================================================================================================================
    // Part B — the Stand Map and the minimap, rendered, driven by the real pointer
    // =================================================================================================================
    private Label MapLabel(string name) => ui.Map.Root.Q<Label>(name);
    private bool Has(string name) { VisualElement e = ui.Map.Root.Q(name); return e != null && Visible(e); }
    private MapDrawItem[] MiniItems(MapSymbol symbol) => ui.Minimap.Canvas.LastPlan.Where(i => i.Symbol == symbol).ToArray();

    private IEnumerator EnsurePlanning()
    {
        if (!ui.Map.RackMode) yield return Click(Control("map-plan-rack"));
        Check(ui.Map.RackMode, "Plan Racks is on");
    }

    private IEnumerator OpenMap(string tag)
    {
        ui.CloseAll(); yield return Settle(10);
        ui.ShowMap(); yield return Settle(20);
        if (ui.Help != null && ui.Help.IsOpen) { ui.CloseHelp(); yield return Settle(12); }
        Check(ui.CurrentScreen == ScenarioOneUiRoot.UiScreen.Map, tag + ": Stand Map open");
        yield return Wait(0.5f);
    }

    private IEnumerator ShowHud(string tag)
    {
        ui.CloseAll(); yield return Settle(10);
        yield return Wait(1.2f);
        Check(ui.Minimap.Visible, tag + ": the minimap is showing");
    }

    // The minimap's rendered rectangle and the note's must sit inside the screen, with the note inside the minimap panel.
    private void CheckNoteLayout(string tag)
    {
        Label note = ui.Minimap.Root.Q<Label>(className: "hud-minimap-note");
        Check(note != null && Visible(note), tag + ": the note is rendered");
        float s = PixelsPerUnit;
        Rect panel = ui.Minimap.Root.worldBound, text = note.worldBound;
        Check(panel.xMin * s >= 0 && panel.yMin * s >= 0 && panel.xMax * s <= Screen.width + 1 && panel.yMax * s <= Screen.height + 1, tag + ": the minimap panel is fully on screen");
        Check(text.yMin >= panel.yMin && text.yMax <= panel.yMax + 1 && text.xMin >= panel.xMin && text.xMax <= panel.xMax + 1, tag + ": the note sits inside the minimap panel");
        float fontPx = note.resolvedStyle.fontSize * s;
        Check(fontPx >= 9.5f, tag + ": the note text is " + F(fontPx) + " screen px, readable");
        State(tag + " note rect(screen px)=" + F(text.xMin * s) + "," + F(text.yMin * s) + "," + F(text.width * s) + "," + F(text.height * s) + " fontPx=" + F(fontPx) + " minimapPx=" + F(panel.width * s) + "x" + F(panel.height * s));
    }

    private IEnumerator PartB()
    {
        Vector2Int[] sizes = Sizes();
        for (int index = 0; index < sizes.Length; index++)
        {
            Vector2Int size = sizes[index];
            bool fullFlow = index == 0;
            string tag = size.x + "x" + size.y;
            if (index > 0) yield return ReloadFresh();
            SetGameSize(size.x, size.y);
            yield return Wait(1.5f);
            ui.CloseAll(); yield return Settle(20);
            // The first-run intro covers the HUD; page through it so the minimap and its note are what is captured.
            for (int page = 0; page < 4 && ui.Help != null && ui.Help.Onboarding.IsOpen; page++) { ui.Help.Onboarding.Next(); yield return Settle(10); }
            Check(ui.Help == null || !ui.Help.Onboarding.IsOpen, tag + ": the first-run intro is closed");
            ui.CloseAll(); yield return Settle(10);
            ResetTeachingNote(ui.Minimap);
            yield return ShowHud(tag);
            Check(Screen.width == size.x && Screen.height == size.y, tag + ": game view at the requested size");

            // ---- the first-time minimap note and the dashed guidance ------------------------------------------------------------
            Check(ui.Minimap.TeachingNoteVisible && ui.Minimap.TeachingNoteText == "Dashed lines show suggested rack positions. Solid lines show your confirmed plan.",
                tag + ": the first-time minimap note names dashed suggestions and the solid confirmed plan");
            CheckNoteLayout(tag);
            Check(MiniItems(MapSymbol.RackSuggested).Length == 5 && MiniItems(MapSymbol.RackSuggested).All(i => i.DashPx > 0) && MiniItems(MapSymbol.RackPlayer).Length == 0,
                tag + ": the minimap shows five dashed suggestions and no solid line yet");
            yield return Capture(tag, "01-hud-minimap-suggestions-note");

            yield return OpenMap(tag);
            RegisterPointerTrace();
            yield return Calibrate();
            yield return EnsurePlanning();

            // ---- an empty plan cannot confirm ------------------------------------------------------------------------------------
            Check(MapLabel("map-edit-counter").text == "Racks planned: 0 / 5", tag + ": the planning counter reads Racks planned: 0 / 5");
            Button confirmButton = Control("map-rack-plan-confirm");
            Check(confirmButton != null && !confirmButton.enabledSelf && MapLabel("map-rack-plan-confirm-reason").text.Contains("exactly 5"), tag + ": Confirm Rack Plan is disabled with the honest reason");
            yield return Capture(tag, "02-confirm-disabled-empty");

            // ---- five draft racks, then four ------------------------------------------------------------------------------------------
            yield return Click(Control("map-edit-reset"));
            Check(manager.PlannedRackCount == 5 && Control("map-rack-plan-confirm").enabledSelf && MapLabel("map-edit-counter").text == "Racks planned: 5 / 5", tag + ": a real click on Reset gave five racks and enabled Confirm");
            Check(manager.WorkOrders.Count(o => o.rackId > 0) == 0, tag + ": drafting created no work order");
            yield return Capture(tag, "03-five-draft-confirm-enabled");

            int[] ids = manager.RackPlan.Select(r => r.Id).ToArray();
            RackPlanRack target = Rack(ids[4]);
            yield return ClickWorld(OnRack(target, 0.5f));
            Check(RackEd.SelectedRackId == ids[4] && Control("map-edit-delete").enabledSelf, tag + ": a real click selected rack " + ids[4]);
            yield return Click(Control("map-edit-delete"));
            Check(manager.PlannedRackCount == 4, tag + ": a real click on Delete left four racks");
            Check(!Control("map-rack-plan-confirm").enabledSelf && MapLabel("map-rack-plan-confirm-reason").text.Contains("has 4"), tag + ": four racks: Confirm is disabled and says why");
            string fourPlan = PlanSig(), fourOrders = OrderSig();
            yield return Click(Control("map-rack-plan-confirm"));
            Check(!manager.RackPlanConfirmed && PlanSig() == fourPlan && OrderSig() == fourOrders, tag + ": a real click on the disabled Confirm button changed nothing");
            yield return Capture(tag, "04-four-racks-confirm-refused");
            yield return Click(Control("map-edit-undo"));
            Check(manager.PlannedRackCount == 5 && Rack(ids[4]) != null, tag + ": Undo restored the fifth rack");

            // ---- confirm ---------------------------------------------------------------------------------------------------------------
            yield return Click(Control("map-rack-plan-confirm"));
            Check(manager.RackPlanConfirmed && manager.ConfirmedRacks.Count == 5, tag + ": a real click confirmed the five-rack plan");
            Check(manager.WorkOrders.Where(o => o.rackId > 0).All(o => o.status == ScenarioWorkStatus.Pending) && manager.WorkOrders.Count(o => o.rackId > 0) > 0 && OrdersUnique(),
                tag + ": confirmation created pending work once per tree and approved nothing");
            string confirmedOrders = OrderSig();
            Check(!Visible(ui.Map.EditLayer) && !RackEd.Active, tag + ": the pointer layer is gone: the confirmed plan cannot be edited");
            Check(Control("map-rack-plan-reopen") != null && Control("map-rack-plan-reopen").enabledSelf && Control("map-rack-plan-confirm") == null, tag + ": the card now offers Reopen Rack Plan");
            Check(MapLabel("map-edit-counter").text == "Rack entrances marked: 0 / 5", tag + ": the map counter switched to Rack entrances marked: 0 / 5");
            Check(ui.MapSource.Model.Racks.Count(r => r.Symbol == MapSymbol.RackSuggested) == 0 && ui.MapSource.Model.Racks.Count(r => r.Symbol == MapSymbol.RackPlayer) == 5, tag + ": the Stand Map shows the five solid racks and no dashed guidance");
            yield return Capture(tag, "05-confirmed-stand-map");

            ui.ShowObjectives(); yield return Settle(20);
            Check(ui.Learning.CurrentInstruction.Contains("Rack entrances marked: 0 / 5"), tag + ": the Objectives screen states Rack entrances marked: 0 / 5");
            yield return Capture(tag, "06-objectives-entrances-marked");

            yield return ShowHud(tag);
            Check(!ui.Minimap.TeachingNoteVisible, tag + ": the note is gone once the plan is confirmed");
            var solid = MiniItems(MapSymbol.RackPlayer);
            Check(solid.Length == 5 && solid.All(i => i.DashPx == 0) && MiniItems(MapSymbol.RackSuggested).Length == 0, tag + ": the minimap shows five solid racks and no dashed suggestion");
            Check(solid.Select(i => ui.MapSource.Model.Racks[i.SourceIndex].RackId).OrderBy(x => x).SequenceEqual(ids.OrderBy(x => x)), tag + ": the minimap draws the same five rack identities as the plan");
            yield return Capture(tag, "07-hud-minimap-confirmed-solid");
            if (!fullFlow) continue;

            // ---- reopen before any marking; marks preserved through an edit of one rack ----------------------------------------------------------
            foreach (int id in ids.Take(2)) Check(manager.MarkRackEntrance(id).Status == RackPlanOperationStatus.Applied, tag + ": fixture mark on rack " + id);
            yield return OpenMap(tag);
            yield return EnsurePlanning();
            Check(MapLabel("map-edit-counter").text == "Rack entrances marked: 2 / 5", tag + ": the counter follows the marked rack objects: 2 / 5");
            yield return Click(Control("map-rack-plan-reopen"));
            Check(!manager.RackPlanConfirmed && manager.RackEditingOpen && !Has("map-reopen-panel"), tag + ": a real click reopened the plan at once (no approved work to warn about)");
            Check(OrderSig() == confirmedOrders && manager.RackPlan.Count(r => r.EntranceMarked) == 2, tag + ": reopening alone changed no order and cleared no mark");
            Check(MapLabel("map-edit-counter").text == "Racks planned: 5 / 5" && Visible(ui.Map.EditLayer), tag + ": the counter and the editing layer are back");
            yield return Capture(tag, "08-reopened-pending");
            RackPlanRack ra = Rack(ids[0]);
            yield return ClickWorld(OnRack(ra, 0.5f));
            Check(RackEd.SelectedRackId == ids[0], tag + ": rack " + ids[0] + " is selected after the reopen");
            string spacing = MapLabel("map-edit-spacing").text;
            Check(spacing.Contains("Nearest rack") && ui.Map.Root.Q("map-edit-card").Query<Label>().ToList().Any(l => l.text.Contains("16–20 m")), tag + ": RACK-V2-06 spacing teaching still works after a reopen: " + spacing);
            string othersBefore = OtherOrdersSig(ids[0]);
            yield return DragWorld(OnRack(ra, 0.5f), P(OnRack(ra, 0.5f).X, OnRack(ra, 0.5f).Z + 3f));
            Check(Rack(ids[0]).Entrance.Z > ra.Entrance.Z + 1.5f && !Rack(ids[0]).EntranceMarked && Rack(ids[1]).EntranceMarked, tag + ": a real drag moved rack " + ids[0] + ": its mark cleared and rack " + ids[1] + " kept its mark");
            Check(OtherOrdersSig(ids[0]) == othersBefore && OrdersUnique(), tag + ": only that rack's pending orders were reconciled");
            yield return Capture(tag, "09-edited-after-reopen-marks-preserved");
            yield return Click(Control("map-rack-plan-confirm"));
            Check(manager.RackPlanConfirmed && manager.MarkedRackEntranceCount == 1 && MapLabel("map-edit-counter").text == "Rack entrances marked: 1 / 5", tag + ": reconfirmed: one mark remains and the counter says 1 / 5");

            // ---- approved but unexecuted work: warning on reopen, decline, consent on edit -------------------------------------------------
            Check(manager.ApprovePendingWork(), tag + ": fixture: the pending work is approved");
            yield return Settle(10);
            yield return OpenMap(tag);
            yield return EnsurePlanning();
            string approvedPlan = PlanSig(), approvedOrders = OrderSig();
            yield return Click(Control("map-rack-plan-reopen"));
            Check(manager.RackPlanConfirmed && Has("map-reopen-panel"), tag + ": a real click on Reopen shows the approved-work warning and does not reopen yet");
            yield return Capture(tag, "10-reopen-warning");
            yield return Click(Control("map-reopen-decline"));
            Check(manager.RackPlanConfirmed && PlanSig() == approvedPlan && OrderSig() == approvedOrders && !Has("map-reopen-panel"), tag + ": declining kept the plan confirmed and changed nothing");
            yield return Click(Control("map-rack-plan-reopen"));
            yield return Click(Control("map-reopen-accept"));
            Check(!manager.RackPlanConfirmed && OrderSig() == approvedOrders, tag + ": accepting reopened the plan and still cancelled nothing");
            string reopenedPlan = PlanSig();
            int c = ids[2];
            RackPlanRack rc = Rack(c);
            yield return ClickWorld(OnRack(rc, 0.5f));
            yield return DragWorld(OnRack(rc, 0.5f), P(OnRack(rc, 0.5f).X, OnRack(rc, 0.5f).Z + 3f));
            Check(Has("map-consent-panel") && Rack(c).Entrance.Z == rc.Entrance.Z && OrderSig() == approvedOrders, tag + ": dragging a rack with approved work shows the consent warning and changes nothing yet");
            yield return Capture(tag, "11-edit-consent-warning");
            yield return Click(Control("map-consent-decline"));
            Check(!Has("map-consent-panel") && Rack(c).Entrance.Z == rc.Entrance.Z && OrderSig() == approvedOrders && PlanSig() == reopenedPlan, tag + ": declining the edit changed nothing");
            yield return ClickWorld(OnRack(rc, 0.5f));
            yield return DragWorld(OnRack(rc, 0.5f), P(OnRack(rc, 0.5f).X, OnRack(rc, 0.5f).Z + 3f));
            Check(Has("map-consent-panel"), tag + ": the same edit asks again");
            yield return Click(Control("map-consent-accept"));
            Check(Rack(c).Entrance.Z > rc.Entrance.Z + 1.5f && OrdersUnique(), tag + ": consenting applied the edit");
            Check(manager.WorkOrders.Where(o => o.rackId == c && o.IsOpen).All(o => o.status == ScenarioWorkStatus.Pending)
                && manager.WorkOrders.Where(o => o.rackId > 0 && o.rackId != c && o.IsOpen).All(o => o.status == ScenarioWorkStatus.Approved), tag + ": only rack " + c + " went back to pending; every other rack keeps its approved work");
            yield return Capture(tag, "12-after-consent");

            // ---- an executed rack stays locked while another is editable --------------------------------------------------------------------------
            yield return Click(Control("map-rack-plan-confirm"));
            Check(manager.RackPlanConfirmed, tag + ": confirmed again");
            int lockedId = ids[3];
            ScenarioOneWorkOrder executed = manager.WorkOrders.First(o => o.rackId == lockedId);
            executed.status = ScenarioWorkStatus.Completed; executed.resolvedYear = 0;
            yield return OpenMap(tag);
            yield return EnsurePlanning();
            Check(manager.RackIsLocked(lockedId), tag + ": fixture: rack " + lockedId + " has an executed removal");
            yield return Click(Control("map-rack-plan-reopen"));
            if (Has("map-reopen-panel")) yield return Click(Control("map-reopen-accept"));
            Check(!manager.RackPlanConfirmed && manager.RackEditingOpen, tag + ": the plan reopens around the executed rack");
            string lockedPlan = PlanSig(), lockedOrders = OrderSig();
            RackPlanRack rl = Rack(lockedId);
            yield return ClickWorld(OnRack(rl, 0.5f));
            yield return DragWorld(OnRack(rl, 0.5f), P(OnRack(rl, 0.5f).X, OnRack(rl, 0.5f).Z + 3f));
            Check(PlanSig() == lockedPlan && OrderSig() == lockedOrders && Rack(lockedId).IsLocked, tag + ": a real drag of the executed rack changed nothing: it stays locked");
            Check(!Has("map-consent-panel"), tag + ": no consent is offered for a locked rack");
            yield return Capture(tag, "13-executed-rack-locked");
            int d = ids[4];
            RackPlanRack rd = Rack(d);
            yield return ClickWorld(OnRack(rd, 0.5f));
            yield return DragWorld(OnRack(rd, 0.5f), P(OnRack(rd, 0.5f).X, OnRack(rd, 0.5f).Z - 3f));
            if (Has("map-consent-panel")) yield return Click(Control("map-consent-accept"));
            Check(Rack(d).Entrance.Z < rd.Entrance.Z - 1.5f, tag + ": another rack is still editable while the executed rack is locked");
            yield return Click(Control("map-rack-plan-confirm"));
            Check(manager.RackPlanConfirmed && manager.WorkOrders.Count(o => o.rackId == lockedId && o.status == ScenarioWorkStatus.Completed) == 1
                && manager.WorkOrders.Any(o => o.workOrderId == executed.workOrderId && o.status == ScenarioWorkStatus.Completed) && OrdersUnique(), tag + ": reconfirmed; the executed removal is untouched and nothing was duplicated");
            yield return ShowHud(tag);
            Check(MiniItems(MapSymbol.RackHarvested).Length == 1 && MiniItems(MapSymbol.RackPlayer).Length == 4, tag + ": the minimap shows the harvested rack pale and the four others solid");
            yield return Capture(tag, "14-hud-minimap-harvested");
        }
    }
}
#endif
