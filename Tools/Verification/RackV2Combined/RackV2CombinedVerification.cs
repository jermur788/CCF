#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UIElements;

// Disposable Tier-2 gate for RACK-V2-COMBINED-INTEGRATION: the behaviour neither source lane could test alone.
//   02B-R (fresh model-1 world, headland/feeder/pad)  x  03C (authoritative rack runtime and D-064 work-order lifecycle)
//
// Run once per mode (set CCF_RV2C_MODE and CCF_RV2C_STATE), each in its own Editor process:
//   write     fresh V2 world with the headland -> five racks from the qualified headland entrances -> entrance trees match the
//             02B-R qualified entrances -> Confirm (Pending work only) -> mark two entrances -> a REAL save to disk.
//   continue  a cold Editor on the real startup screen -> the real Continue path against the file `write` produced ->
//             exact state comparison -> approve through the Work Plan UI -> execute -> rack.approve / rack.result ->
//             Stage 2 -> a second disk round trip of the completed state. Rendered captures at 1280x720 and 1920x1080.
//   probe     INFORMATIONAL ONLY: the same lifecycle with V2 racks that are NOT on the authored layout lines, to measure
//             what the legacy RackLayoutTracker (which gates rack.approve / rack.result) does with them.
//
// Copy into Assets/ForestPrototype, run RackV2CombinedVerification.Begin, then remove the copy and its .meta.
// Save safety: every mode refuses to run unless Application.persistentDataPath is inside XDG_CONFIG_HOME (the runner's
// isolated config), so the player's own forest-save.json can never be written or read.
public static class RackV2CombinedVerification
{
    public static void Begin()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        EditorApplication.isPlaying = true;
    }

    // Lessons are derived from forest state (they must survive a load), so start every process with none recorded.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Prepare()
    {
        if (!Environment.GetCommandLineArgs().Any(a => a == "RackV2CombinedVerification.Begin")) return;
        foreach (string id in LearningObjectivesView.StepIds) PlayerPrefs.DeleteKey(LearningObjectivesView.PreferencePrefix + id);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (Environment.GetCommandLineArgs().Any(a => a == "RackV2CombinedVerification.Begin"))
            new GameObject("Disposable rack v2 combined verification").AddComponent<RackV2CombinedVerificationRunner>();
    }

    public static void SetGameSize(int width, int height)
    {
        Assembly assembly = typeof(Editor).Assembly;
        Type sizesType = assembly.GetType("UnityEditor.GameViewSizes");
        Type singleton = typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
        object sizes = singleton.GetProperty("instance", BindingFlags.Public | BindingFlags.Static).GetValue(null);
        MethodInfo getGroup = sizesType.GetMethod("GetGroup");
        object group = getGroup.Invoke(sizes, new[] { Enum.ToObject(getGroup.GetParameters()[0].ParameterType, 0) });
        Type sizeType = assembly.GetType("UnityEditor.GameViewSize"), kind = assembly.GetType("UnityEditor.GameViewSizeType");
        object size = Activator.CreateInstance(sizeType, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null,
            new object[] { Enum.ToObject(kind, 1), width, height, "Rack V2 combined " + width + "x" + height }, null);
        int index = (int)group.GetType().GetMethod("GetTotalCount").Invoke(group, null);
        group.GetType().GetMethod("AddCustomSize").Invoke(group, new[] { size });
        Type viewType = assembly.GetType("UnityEditor.GameView");
        EditorWindow view = EditorWindow.GetWindow(viewType);
        viewType.GetProperty("selectedSizeIndex", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).SetValue(view, index);
        view.Show(); view.Focus(); view.Repaint();
    }
}

[Serializable]
public sealed class RackV2CombinedState
{
    public string signature, worldHash, livingIdsSha, saveSha256, headlandSurface;
    public int living, planned, marked, saveVersion;
    public string[] entranceTrees;
    public int[] pendingOrderIds, rackIds;
}

public sealed class RackV2CombinedVerificationRunner : MonoBehaviour
{
    // The five entrance trees RACK-V2-02A/02B-R qualified on the headland, in layout-line order (north-west to south-west).
    private static readonly string[] QualifiedEntrances = { "PO0203", "PO1204", "PO2003", "PO2803", "PO3803" };
    private static readonly Vector2Int[] Sizes = { new Vector2Int(1280, 720), new Vector2Int(1920, 1080) };

    private ForestEcologyController ecology;
    private ScenarioOneManager manager;
    private ForestSaveController saves;
    private ScenarioOneUiRoot ui;
    private ForestPlayer player;
    private ForestTreeMarkingManager marks;
    private RackPlanner planner;
    private string mode, output, statePath, savePath;
    private int checks;

    private static void Require(bool ok, string message) { if (!ok) throw new InvalidOperationException(message); }
    private void Pass(string message) { checks++; Debug.Log("RACKV2COMBINED_CHECK " + message); }
    private void Check(bool ok, string message) { Require(ok, message); Pass(message); }
    private static void Note(string name, string detail) => Debug.Log("RACKV2COMBINED_MEASURE " + name + " " + detail);

    private ForestTree[] Living() => FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).Where(t => t.IsLiving).ToArray();
    private static string Sha(IEnumerable<string> values) => Sha(string.Join(",", values.OrderBy(v => v, StringComparer.Ordinal)));
    private static string Sha(string text)
    {
        using (SHA256 sha = SHA256.Create()) return string.Concat(sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(text)).Select(b => b.ToString("x2")));
    }
    private static string FileSha(string path)
    {
        using (SHA256 sha = SHA256.Create()) return string.Concat(sha.ComputeHash(File.ReadAllBytes(path)).Select(b => b.ToString("x2")));
    }

    private IEnumerator Start()
    {
        mode = Environment.GetEnvironmentVariable("CCF_RV2C_MODE") ?? "";
        for (int i = 0; i < 6; i++) yield return null;
        Exception failure = null;
        var stack = new Stack<IEnumerator>();
        stack.Push(Run());
        while (stack.Count > 0)
        {
            bool more; object current = null;
            try { more = stack.Peek().MoveNext(); if (more) current = stack.Peek().Current; }
            catch (Exception error) { failure = error; break; }
            if (!more) { stack.Pop(); continue; }
            if (current is IEnumerator nested) { stack.Push(nested); continue; }
            yield return current;
        }
        string tag = "RACKV2COMBINED_" + mode.ToUpperInvariant();
        Debug.Log(failure == null ? tag + "_PASS checks=" + checks : tag + "_FAIL checks=" + checks + " " + failure);
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(failure == null ? 0 : 1);
    }

    private IEnumerator Run()
    {
        ecology = FindFirstObjectByType<ForestEcologyController>();
        manager = FindFirstObjectByType<ScenarioOneManager>();
        saves = FindFirstObjectByType<ForestSaveController>();
        ui = FindFirstObjectByType<ScenarioOneUiRoot>();
        player = FindFirstObjectByType<ForestPlayer>();
        Require(ecology != null && manager != null && saves != null && ui != null && player != null, "not the Scenario One ForestTest scene");
        marks = ui.Marking;

        // Save safety first: never let this gate near the player's own save slot.
        string config = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME");
        Require(!string.IsNullOrEmpty(config) && Application.persistentDataPath.StartsWith(config, StringComparison.Ordinal),
            "persistentDataPath must be inside the isolated XDG_CONFIG_HOME: " + Application.persistentDataPath);
        savePath = Path.Combine(Application.persistentDataPath, "forest-save.json");
        statePath = Environment.GetEnvironmentVariable("CCF_RV2C_STATE");
        Require(!string.IsNullOrEmpty(statePath), "CCF_RV2C_STATE must name the cross-process state file");
        string diag = Environment.GetEnvironmentVariable("CCF_ACCEPTANCE_OUTPUT");
        output = string.IsNullOrEmpty(diag) ? Path.GetDirectoryName(statePath) : diag;
        Directory.CreateDirectory(output);
        Pass("isolated save slot: " + Application.persistentDataPath);

        if (mode == "write") yield return Write();
        else if (mode == "continue") yield return Continue();
        else if (mode == "probe") yield return Probe();
        else throw new InvalidOperationException("CCF_RV2C_MODE must be write, continue or probe (was '" + mode + "')");
    }

    // ---- shared helpers ------------------------------------------------------------------------------------------
    private string Signature()
    {
        var parts = new List<string>();
        foreach (RackPlanRack rack in manager.RackPlan)
            parts.Add(rack.Id + ":" + rack.Entrance.X.ToString("R") + "/" + rack.Entrance.Z.ToString("R") + ":"
                + rack.Far.X.ToString("R") + "/" + rack.Far.Z.ToString("R") + ":" + rack.Status + ":" + rack.EntranceTreeId + ":"
                + rack.EntranceMarked + ":" + string.Join(",", rack.RemovalTreeIds) + ":" + rack.Orders);
        foreach (ScenarioOneWorkOrder order in manager.WorkOrders.OrderBy(o => o.workOrderId))
            parts.Add(order.workOrderId + ":" + order.status + ":" + order.targetTreeId + ":" + order.rackId);
        parts.Add("planned=" + manager.PlannedRackCount + " marked=" + manager.MarkedRackEntranceCount + " confirmed=" + manager.RackPlanConfirmed
            + " next=" + manager.NextRackId + " model=" + manager.RackPlanningModelVersion);
        return string.Join("|", parts);
    }

    private IEnumerator Frames(int count) { for (int i = 0; i < count; i++) yield return null; }

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

    private Button BtnStarts(params string[] texts) => ui.RootElement.Query<Button>().ToList()
        .FirstOrDefault(b => b.text != null && texts.Any(t => b.text.StartsWith(t, StringComparison.Ordinal)) && Visible(b));

    private static string Texts(VisualElement root)
        => root == null ? "" : string.Join("\n", root.Query<TextElement>().ToList().Where(Visible).Select(t => t.text).Where(s => !string.IsNullOrEmpty(s)));

    private IEnumerator Invoke(Button button)
    {
        Require(button != null, "button not found");
        typeof(Clickable).GetMethod("Invoke", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(button.clickable, new object[] { null });
        yield return null; yield return null;
    }

    private IEnumerator Press(Key key)
    {
        Keyboard keyboard = Keyboard.current ?? InputSystem.AddDevice<Keyboard>();
        InputSystem.QueueStateEvent(keyboard, new KeyboardState(key)); yield return null; yield return null;
        InputSystem.QueueStateEvent(keyboard, new KeyboardState()); yield return null; yield return null;
    }

    private IEnumerator CloseHelp()
    {
        for (int i = 0; i < 6 && ui.Help.IsOpen; i++)
        {
            Button close = ui.Help.Root.Q<Button>("menu-help-close");
            if (close != null) yield return Invoke(close); else ui.CloseHelp();
            yield return null;
        }
    }

    private IEnumerator Shoot(string state)
    {
        foreach (Vector2Int size in Sizes)
        {
            RackV2CombinedVerification.SetGameSize(size.x, size.y);
            for (int i = 0; i < 10; i++) yield return null;
            yield return new WaitForEndOfFrame();
            Texture2D image = ScreenCapture.CaptureScreenshotAsTexture();
            Require(image != null && image.width >= 600, "no rendered capture for " + state);
            File.WriteAllBytes(Path.Combine(output, state + "-" + size.x + "x" + size.y + ".png"), image.EncodeToPNG());
            Destroy(image);
        }
    }

    private IEnumerator TopDown(string state)
    {
        Camera cam = Camera.main;
        Vector3 position = cam.transform.position; Quaternion rotation = cam.transform.rotation; float fov = cam.fieldOfView;
        bool playerWas = player.enabled;
        player.enabled = false;
        ui.RootElement.style.display = DisplayStyle.None;
        cam.transform.position = new Vector3(0f, 118f, 0f);
        cam.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        cam.fieldOfView = 40f;
        yield return Frames(6);
        yield return Shoot(state);
        ui.RootElement.style.display = DisplayStyle.Flex;
        cam.transform.position = position; cam.transform.rotation = rotation; cam.fieldOfView = fov;
        player.enabled = playerWas;
        yield return Frames(4);
    }

    // The qualified headland fixture is distinct from the older authored layout. Its access endpoint
    // is on the headland, not the property edge west of it. Keep the entrance assertions independent.
    private List<int> AddQualifiedHeadlandRacks()
    {
        planner = new RackPlanner(manager, ecology);
        planner.Refresh();
        ForestTree[] living = Living();
        var ids = new List<int>();
        foreach (string entranceId in QualifiedEntrances)
        {
            ForestTree tree = living.Single(t => t.TreeId == entranceId);
            float z = tree.transform.position.z;
            RackPlanOperationResult add = manager.AddRack(
                new RackPoint(ScenarioOneHeadlandGeometry.HeadlandX, z), new RackPoint(planner.HalfExtentM, z));
            Require(add.Succeeded && add.RacksAdded == 1, "qualified headland rack adds: " + add.Message);
            ids.Add(add.RackId);
        }
        return ids;
    }

    // Deliberately legacy authored lines, used only by the informational coupling probe.
    private List<int> AddSuggestedRacks(float zShift = 0f, float angle = 0f)
    {
        planner = new RackPlanner(manager, ecology);
        planner.Refresh();
        Require(planner.LayoutLines.Count == 5, "the Enlarged80 stand has five authored layout lines");
        var ids = new List<int>();
        foreach (float line in planner.LayoutLines)
        {
            Require(RackGeometry.ClipLineToSquare(new RackPoint(0f, line + zShift), angle, planner.HalfExtentM, out RackPoint start, out RackPoint end),
                "layout line clips to the stand");
            RackPlanOperationResult add = manager.AddRack(start, end);
            Require(add.Succeeded && add.RacksAdded == 1, "rack adds: " + add.Message);
            ids.Add(add.RackId);
        }
        return ids;
    }

    // ---- WRITE ---------------------------------------------------------------------------------------------------
    private IEnumerator Write()
    {
        Check(!File.Exists(savePath), "the isolated save slot starts empty");
        Check(StandGeometryPolicy.NewGameModelForSession == StandGeometryModel.Enlarged80 && ecology.StandGeometryModelVersion == StandGeometryModel.Enlarged80,
            "a fresh game is the production Enlarged80 world");
        Check(manager.RackPlanningV2Active && manager.RackPlanningModelVersion == RackPlanningModel.RackPlanningFieldMarkingV2,
            "the fresh Enlarged80 world is a Rack Planning V2 world");
        Check(manager.PlannedRackCount == 0 && !manager.RackPlanConfirmed && manager.NextRackId == 1, "a fresh V2 world starts with an empty, unconfirmed plan");
        Check(manager.RackFirstStageActive, "the rack-first teaching stage is active in a fresh V2 world (RackUxRack STAGE1_RACK_FIRST_IS_ACTIVE, V2 form)");
        Note("STAGE1_CUE", "cue=" + ui.Learning.Summary.Replace("\n", " / ") + " | topic1=" + LearningObjectivesView.Topics[0].Title);

        // 02B-R x 03C: the headland world is really present underneath the rack runtime.
        ForestTree[] living = Living();
        Check(living.Length == 1344, "fresh model-1 world has 1344 living starting stems (02B-R)");
        Check(living.All(t => !ScenarioOneHeadlandGeometry.ContainsStemCentre(t.transform.position)), "no starting stem inside the guarded headland/feeder/pad");
        GameObject surface = GameObject.Find("Scenario One V2 Headland");
        Check(surface != null && surface.GetComponent<MeshRenderer>() != null && surface.GetComponent<Collider>() == null,
            "headland surface exists with a renderer and no collider");

        List<int> rackIds = AddQualifiedHeadlandRacks();
        Check(manager.PlannedRackCount == 5 && manager.RackPlan.Count == 5 && !manager.RackPlanConfirmed, "five draft racks are authoritative and unconfirmed");

        // The cross-lane contract: 03C's derived entrance tree per rack == the entrance tree 02B-R qualified for that line.
        List<RackPlanRack> draft = manager.RackPlan.OrderBy(r => r.Id).ToList();
        string[] actual = draft.Select(r => r.EntranceTreeId).ToArray();
        Note("ENTRANCE_TREES", "runtime=[" + string.Join(",", actual) + "] qualified=[" + string.Join(",", QualifiedEntrances) + "]");
        for (int i = 0; i < 5; i++)
        {
            Check(actual[i] == QualifiedEntrances[i], "rack " + (i + 1) + " entrance tree " + actual[i] + " is the 02B-R qualified " + QualifiedEntrances[i]);
            ForestTree entrance = living.SingleOrDefault(t => t.TreeId == actual[i]);
            Check(entrance != null && Mathf.Abs(entrance.transform.position.x - (-35.4f)) < 8f, "entrance tree " + actual[i] + " exists and is reachable from the headland");
            Check(draft[i].RemovalTreeIds.Contains(actual[i]), "entrance tree " + actual[i] + " is in its rack's removal set");
        }

        int livingBefore = Living().Length;
        RackPlanOperationResult confirm = manager.ConfirmRackPlan();
        Check(confirm.Succeeded && confirm.Status == RackPlanOperationStatus.Applied && manager.RackPlanConfirmed && manager.ConfirmedRacks.Count == 5, "Confirm applies: " + confirm.Message);
        Check(manager.WorkOrders.Count(o => o.rackId > 0) == manager.RackPlan.Sum(r => r.RemovalTreeIds.Count), "one rack-owned order per frozen removal tree");
        Check(manager.WorkOrders.Where(o => o.rackId > 0).All(o => o.status == ScenarioWorkStatus.Pending), "Confirm creates Pending work only");
        Check(Living().Length == livingBefore, "Confirm fells no tree");

        Check(manager.MarkRackEntrance(rackIds[0]).Succeeded && manager.MarkRackEntrance(rackIds[2]).Succeeded, "two entrances marked");
        Check(manager.PlannedRackCount == 5 && manager.MarkedRackEntranceCount == 2, "derived counters: planned 5, marked 2");

        // A REAL write through the game's own save path.
        saves.Save();
        yield return Frames(2);
        Require(File.Exists(savePath), "Save() wrote the save file");
        ForestSaveData onDisk = JsonUtility.FromJson<ForestSaveData>(File.ReadAllText(savePath));
        Check(onDisk != null && onDisk.version == ForestSaveData.CurrentVersion, "the save on disk is the current schema version " + ForestSaveData.CurrentVersion);
        Check(ForestSaveValidation.ValidateRackV2(onDisk) == null, "the save on disk passes V2 rack validation");
        Check(onDisk.scenarioOne.rackPlanningModel == RackPlanningModel.RackPlanningFieldMarkingV2 && onDisk.scenarioOne.rackPlanConfirmed && onDisk.scenarioOne.racks.Count == 5,
            "the save on disk records model 1, a confirmed plan and five racks");

        var state = new RackV2CombinedState
        {
            signature = Signature(),
            worldHash = ScenarioReferenceArchive.CurrentWorldHash(saves.CaptureData()),
            livingIdsSha = Sha(Living().Select(t => t.TreeId)),
            living = Living().Length,
            planned = manager.PlannedRackCount,
            marked = manager.MarkedRackEntranceCount,
            saveVersion = onDisk.version,
            saveSha256 = FileSha(savePath),
            headlandSurface = surface.name,
            entranceTrees = actual,
            pendingOrderIds = manager.WorkOrders.Where(o => o.rackId > 0).Select(o => o.workOrderId).ToArray(),
            rackIds = rackIds.ToArray(),
        };
        File.WriteAllText(statePath, JsonUtility.ToJson(state, true));
        Note("WRITE_STATE", "worldHash=" + state.worldHash + " living=" + state.living + " saveSha256=" + state.saveSha256 + " orders=" + state.pendingOrderIds.Length);
        Pass("cross-process state written");
    }

    // ---- CONTINUE ------------------------------------------------------------------------------------------------
    private IEnumerator Continue()
    {
        Require(File.Exists(statePath), "state file from the write process is missing");
        RackV2CombinedState state = JsonUtility.FromJson<RackV2CombinedState>(File.ReadAllText(statePath));
        Check(File.Exists(savePath) && FileSha(savePath) == state.saveSha256, "the persisted save file is byte-identical to what the write process wrote");

        // A genuinely cold process: the real startup screen is up (this mode does NOT opt out of Bootstrap).
        StandaloneSessionMenu menu = null;
        for (int frame = 0; frame < 600; frame++)
        {
            menu = FindFirstObjectByType<StandaloneSessionMenu>();
            if (menu != null && StandaloneSessionMenu.IsOpen && menu.StartupScreenActive) break;
            yield return null;
        }
        Require(menu != null && StandaloneSessionMenu.IsOpen && menu.StartupScreenActive, "the real startup screen opened");
        yield return Frames(3);
        Check(manager.PlannedRackCount == 0 && !manager.RackPlanConfirmed, "before Continue the cold process holds a fresh V2 game with no racks");
        Check(menu.ContinueAvailable, "Continue is offered for the persisted V2 save");
        Button cont = menu.GetComponent<UIDocument>().rootVisualElement.Query<Button>().ToList().FirstOrDefault(b => b.text == "Continue");
        Check(cont != null && cont.enabledInHierarchy, "the Continue button is enabled");
        yield return Invoke(cont);
        yield return Frames(6);
        Check(!StandaloneSessionMenu.IsOpen && Time.timeScale == 1f, "Continue closed the startup screen and the game is running");
        yield return CloseHelp();

        // Everything the write process saved is back, exactly.
        Check(manager.RackPlanningV2Active && manager.RackPlanConfirmed, "the loaded world is a confirmed V2 world");
        Check(Signature() == state.signature, "rack ids, geometry, frozen sets, entrance trees, marks, order ids/status and counters are exact after Continue");
        Check(manager.PlannedRackCount == state.planned && manager.MarkedRackEntranceCount == state.marked, "derived counters survive: planned " + state.planned + ", marked " + state.marked);
        Check(Living().Length == state.living && Sha(Living().Select(t => t.TreeId)) == state.livingIdsSha, "the same living stems are back (headland world restored)");
        Check(ScenarioReferenceArchive.CurrentWorldHash(saves.CaptureData()) == state.worldHash, "the world hash after Continue equals the hash at save");
        Check(GameObject.Find(state.headlandSurface) != null, "the headland presentation is shown after Continue");
        Check(manager.WorkOrders.Where(o => o.rackId > 0).All(o => o.status == ScenarioWorkStatus.Pending) && manager.RackPlan.All(r => !r.IsLocked), "rack work is still Pending and unlocked");

        // The disk fixture intentionally stops after two marks. Complete field marking before
        // approval, as D-064 requires, through the authoritative API (the spray UI is a later lane).
        foreach (RackPlanRack rack in manager.ConfirmedRacks.Where(r => !r.EntranceMarked))
            Check(manager.MarkRackEntrance(rack.Id).Succeeded, "remaining entrance marked for rack " + rack.Id);
        Check(manager.MarkedRackEntranceCount == 5, "all five entrances marked before approval");

        yield return Shoot("c01-after-continue");
        yield return TopDown("c02-topdown-confirmed-pending");
        // F-8 consumers read RackGroups / the authored layout. Measure (do not assert) what they show for a confirmed V2 plan.
        int pendingRackOrders = manager.WorkOrders.Count(o => o.rackId > 0 && o.IsOpen);
        Note("F8_PLANNED_MARKERS", "markers=" + ui.PlannedMarkers.MarkerCount + " rackMarkers=" + ui.PlannedMarkers.RackMarkerCount + " pendingRackOrders=" + pendingRackOrders
            + " legacyRackGroups=" + manager.RackGroups.Count);

        // ---- approve through the real Work Plan UI --------------------------------------------------------------
        ui.RackLayout.Update(true);
        Note("RACK_LAYOUT_TRACKER", "done=" + ui.RackLayout.Done + "/" + ui.RackLayout.Total + " complete=" + ui.RackLayout.Complete);
        ui.ShowWorkPlan(); yield return Frames(2);
        yield return CloseHelp();
        yield return Wait(0.4f);
        string workPlanText = Texts(ui.RootElement);
        Note("F8_WORKPLAN", "hasExtractionRackGroup=" + workPlanText.Contains("Extraction rack") + " hasDraft=" + workPlanText.Contains("DRAFT")
            + " hasTimberIncome=" + (workPlanText.Contains("Timber") && workPlanText.Contains("estimated income")) + " hasWholeJobReview=" + workPlanText.Contains("Harvest and salvage job")
            + " statusLine5of5=" + workPlanText.Contains("Extraction racks: 5 of 5 in the Work Plan"));
        List<int> rackIds = state.rackIds.ToList();
        List<string> removalUnion = manager.ConfirmedRacks.SelectMany(r => r.RemovalTreeIds).ToList();
        yield return Shoot("c03-workplan-pending");
        Check(manager.WorkOrders.Count(o => o.IsOpen && o.type == ScenarioWorkType.FellTree) == removalUnion.Count, "the draft Work Plan holds exactly the frozen rack trees");

        Button approve = BtnStarts("Approve Work Plan");
        Check(approve != null, "the Work Plan offers Approve");
        yield return Invoke(approve);
        var approved = manager.WorkOrders.Where(o => o.status == ScenarioWorkStatus.Approved).ToList();
        Check(approved.Count == removalUnion.Count && approved.All(o => o.type == ScenarioWorkType.FellTree && o.rackId > 0)
            && approved.Select(o => o.targetTreeId).OrderBy(i => i, StringComparer.Ordinal).SequenceEqual(removalUnion.OrderBy(i => i, StringComparer.Ordinal))
            && manager.WorkOrders.Count(o => o.status == ScenarioWorkStatus.Pending) == 0, "Approve approves exactly the frozen rack trees and nothing is left Pending");
        Check(rackIds.All(id => manager.RackOrderState(id).HasApprovedWork && !manager.RackIsLocked(id)), "every rack reports approved, unlocked work");
        yield return Wait(0.7f);
        yield return Shoot("c04-workplan-approved");
        Note("LESSONS_AFTER_APPROVE", "create=" + ui.Learning.IsDone("rack.create") + " layout=" + ui.Learning.IsDone("rack.layout") + " approve=" + ui.Learning.IsDone("rack.approve")
            + " | summary=" + ui.Learning.Summary.Replace("\n", " / "));
        Check(ui.Learning.IsDone("rack.create"), "lesson rack.create is recorded from the loaded V2 plan");
        Check(ui.Learning.IsDone("rack.approve"), "lesson rack.approve is recorded after approval of the V2 rack work");

        // Approved-but-unexecuted edits must warn and change nothing (03C semantics, now on a cold-loaded world).
        string approvedSignature = Signature();
        RackPlanRack edited = manager.ConfirmedRacks.First(r => !r.EntranceMarked);
        RackPlanOperationResult warn = manager.UpdateRack(edited.Id, edited.Entrance, new RackPoint(edited.Far.X, edited.Far.Z + 0.6f));
        Check(warn.Status == RackPlanOperationStatus.RequiresApprovalWarning, "an approved-but-unexecuted edit requires the warning: " + warn.Message);
        Check(Signature() == approvedSignature, "the refused approved-order edit changed nothing");

        // ---- execute: advance one year ---------------------------------------------------------------------------
        ScenarioHarvestJob quote = manager.GetHarvestQuote(true);
        long expectedNet = quote.RevenueCents - quote.CostCents, cashBefore = manager.CashCents;
        int livingBefore = Living().Length, year = ecology.EcologicalYear;
        Button advance = BtnStarts("Advance one year");
        Check(advance != null, "the Work Plan offers Advance one year");
        yield return Invoke(advance);
        Check(ecology.EcologicalYear == year + 1, "Advance resolved the approved racks (year " + year + " -> " + ecology.EcologicalYear + ")");
        yield return Frames(8);
        Check(Living().Length == livingBefore - removalUnion.Count, "the harvest felled exactly the rack trees: " + livingBefore + " -> " + Living().Length);
        HashSet<string> stumps = FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).Where(t => t.IsStump).Select(t => t.TreeId).ToHashSet();
        Check(removalUnion.All(stumps.Contains), "every frozen rack tree is now a stump");
        Check(manager.CashCents - cashBefore == expectedNet, "cash change equals the quoted net: " + (manager.CashCents - cashBefore) + " vs " + expectedNet);
        Check(manager.WorkOrders.Where(o => o.rackId > 0).All(o => o.status == ScenarioWorkStatus.Completed), "all rack-owned orders are Completed");
        Check(rackIds.All(id => manager.RackIsLocked(id) && manager.RackOrderState(id).HasCompletedWork), "every rack is now locked by completed work");
        RackPlanRack lockedRack = manager.RackPlan.First(r => r.Id == rackIds[1]);
        Check(manager.UpdateRack(lockedRack.Id, lockedRack.Entrance, new RackPoint(lockedRack.Far.X, lockedRack.Far.Z + 0.6f)).Status == RackPlanOperationStatus.Locked,
            "a completed-linked rack refuses a plausible edit because it is locked");
        Check(manager.DeleteRack(rackIds[1]).Status == RackPlanOperationStatus.Locked, "a completed-linked rack refuses deletion");
        Check(manager.PlannedRackCount == 5 && manager.MarkedRackEntranceCount == 5, "counters are unchanged by execution (planned 5, marked 5)");
        yield return Wait(0.8f);

        yield return Shoot("c05-annual-review");
        Button acknowledge = ui.RootElement.Q<Button>("annual-review-acknowledge");
        if (acknowledge != null) yield return Invoke(acknowledge);
        ui.CloseAll(); yield return Frames(2);
        yield return TopDown("c06-topdown-harvested");

        // Eye level along the first rack from the headland end.
        RackPlanRack first = manager.RackPlan.OrderBy(r => r.Id).First();
        player.transform.position = new Vector3(first.Entrance.X + 1.0f, player.transform.position.y, first.Entrance.Z - 0.9f);
        player.LookToward(new Vector3(first.Far.X, 1.6f, first.Far.Z - 0.9f));
        yield return Frames(8);
        yield return Shoot("c07-rack-corridor-eye-level");
        yield return Wait(0.8f);

        Note("LESSONS_AFTER_RESULT", "approve=" + ui.Learning.IsDone("rack.approve") + " result=" + ui.Learning.IsDone("rack.result")
            + " harvested=" + ui.RackLayout.Harvested + " reports=" + manager.AnnualReports.Count + " interventions=" + manager.Interventions.Count);
        Check(ui.Learning.IsDone("rack.result"), "lesson rack.result is recorded after the V2 rack harvest");
        Check(manager.AnnualReviewSeen && manager.Interventions.Count == 1, "the first intervention and annual review were recorded");

        // ---- Stage 2: subsequent Scenario One progression ------------------------------------------------------
        Check(!manager.RackFirstStageActive, "the rack-first stage ends after the first intervention");
        Check(ui.Learning.Summary.Contains("Inspect a living tree"), "the cue now introduces inspection: " + ui.Learning.Summary.Replace("\n", " / "));
        ForestTree aimed = null;
        foreach (ForestTree t in Living().Where(t => !t.TreeId.StartsWith("Q", StringComparison.Ordinal) && Mathf.Abs(t.transform.position.x) < 20f).Take(40))
        {
            player.transform.position = t.transform.position - new Vector3(0, 0, 3);
            player.LookToward(t.transform.position + Vector3.up * 1.5f);
            yield return Frames(8);
            if (marks.AimedTree == t) { aimed = t; break; }
        }
        string prompt = player.InteractionPromptText();
        Check(aimed != null && prompt.Contains("[X] Fell") && prompt.Contains("[C] Crop Tree"), "the tree prompt now offers selective marking: " + prompt.Replace("\n", " / "));
        yield return Press(Key.X);
        Check(aimed.IsMarkedForFell, "X marks a tree for felling after the rack stage");
        yield return Press(Key.X);
        yield return Press(Key.C);
        Check(aimed.IsCropTree, "C marks a Crop Tree after the rack stage");
        yield return Press(Key.C);

        // ---- the completed state also survives a disk round trip ---------------------------------------------------
        string completedSignature = Signature();
        saves.Save();
        yield return Frames(2);
        ForestSaveData completedOnDisk = JsonUtility.FromJson<ForestSaveData>(File.ReadAllText(savePath));
        Check(ForestSaveValidation.ValidateRackV2(completedOnDisk) == null && completedOnDisk.scenarioOne.racks.Count == 5, "the completed V2 state passes validation on disk");
        Check(saves.CanLoad(out string problem), "the completed save is loadable: " + problem);
        saves.Load();
        yield return Frames(6);
        Check(Signature() == completedSignature, "ids, geometry, marks, order states and locks survive a disk round trip after execution");
        Check(rackIds.All(manager.RackIsLocked), "completed-linked locks survive the disk round trip");
    }

    // ---- PROBE (informational) -----------------------------------------------------------------------------------
    private IEnumerator Probe()
    {
        Check(manager.RackPlanningV2Active, "probe starts in a fresh V2 world");
        planner = new RackPlanner(manager, ecology);
        planner.Refresh();
        // V2 racks that are NOT on the authored layout lines. Try a few offsets until the planner accepts all five.
        List<int> ids = null;
        float usedShift = 0f, usedAngle = 0f;
        foreach (float shift in new[] { 2.0f, -2.0f, 3.0f, -3.0f, 1.5f })
        {
            try { ids = AddSuggestedRacks(shift, 0f); usedShift = shift; break; }
            catch (InvalidOperationException error)
            {
                Note("PROBE_OFFSET_REFUSED", "shift=" + shift + " " + error.Message);
                foreach (int stale in manager.RackPlan.Select(r => r.Id).ToList()) manager.DeleteRack(stale);
            }
        }
        Require(ids != null, "the probe could not build five off-layout racks");
        Note("PROBE_GEOMETRY", "zShift=" + usedShift + " angle=" + usedAngle + " racks=" + ids.Count);
        Require(manager.ConfirmRackPlan().Succeeded, "probe plan confirms");
        int removal = manager.ConfirmedRacks.Sum(r => r.RemovalTreeIds.Count);
        ui.RackLayout.Update(true);
        Note("PROBE_RACK_LAYOUT_TRACKER", "done=" + ui.RackLayout.Done + "/" + ui.RackLayout.Total + " complete=" + ui.RackLayout.Complete + " removalTrees=" + removal);

        ui.ShowWorkPlan(); yield return Frames(2);
        yield return CloseHelp();
        yield return Wait(0.4f);
        Button approve = BtnStarts("Approve Work Plan");
        Note("PROBE_APPROVE_BUTTON", approve == null ? "absent" : "present");
        if (approve != null) yield return Invoke(approve);
        yield return Wait(0.8f);
        Note("PROBE_LESSONS_AFTER_APPROVE", "create=" + ui.Learning.IsDone("rack.create") + " layout=" + ui.Learning.IsDone("rack.layout") + " approve=" + ui.Learning.IsDone("rack.approve")
            + " approvedRackOrders=" + manager.WorkOrders.Count(o => o.rackId > 0 && o.status == ScenarioWorkStatus.Approved));
        Button advance = BtnStarts("Advance one year");
        if (advance != null) yield return Invoke(advance);
        yield return Frames(8);
        yield return Wait(0.8f);
        Note("PROBE_LESSONS_AFTER_RESULT", "approve=" + ui.Learning.IsDone("rack.approve") + " result=" + ui.Learning.IsDone("rack.result")
            + " harvestedTracker=" + ui.RackLayout.Harvested + " interventions=" + manager.Interventions.Count + " reports=" + manager.AnnualReports.Count
            + " completedRackOrders=" + manager.WorkOrders.Count(o => o.rackId > 0 && o.status == ScenarioWorkStatus.Completed));
        Note("PROBE_PROGRESSION", "rackFirstStageActive=" + manager.RackFirstStageActive + " cue=" + ui.Learning.Summary.Replace("\n", " / "));
        Debug.Log("RACKV2COMBINED_PROBE_DONE");
    }
}
#endif
