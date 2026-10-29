#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Disposable Tier-2 RACK-V2-09 gate. In-memory save round trips only; no Save(), annual
// advance, scene save, reference/archive write. Production Update receives OS P/click input.
public static class RackV2FieldMarkingVerification
{
    public const string Marker = "RackV2FieldMarkingVerification.Begin";
    public static void Begin()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        RackV2FieldMarkingRunner.SetGameSize(1280, 720);
        EditorApplication.isPlaying = true;
    }
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Prepare()
    {
        if (!Environment.GetCommandLineArgs().Contains(Marker)) return;
        foreach (MenuHelpView.Menu menu in Enum.GetValues(typeof(MenuHelpView.Menu))) PlayerPrefs.SetInt(MenuHelpView.PreferencePrefix + menu, 1);
    }
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Install()
    {
        if (Environment.GetCommandLineArgs().Contains(Marker))
            new GameObject("Disposable Rack V2 field marking gate").AddComponent<RackV2FieldMarkingRunner>();
    }
}

public sealed class RackV2FieldMarkingRunner : MonoBehaviour
{
    ScenarioOneUiRoot ui;
    ScenarioOneManager manager;
    ForestTreeMarkingManager marks;
    ForestSaveController saves;
    ForestPlayer player;
    ForestEcologyController ecology;
    string output;
    int checks, shots;
    readonly RackAuthoringNativeInput input = new RackAuthoringNativeInput();
    readonly List<string> captures = new List<string>();
    static readonly BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;
    void Check(bool ok, string why)
    {
        if (!ok) throw new InvalidOperationException(why);
        checks++; Debug.Log("RACKFIELD_CHECK " + why);
    }
    static IEnumerator Settle(int frames = 16) { for (int i = 0; i < frames; i++) yield return null; }
    ForestTree Tree(string id) => FindObjectsByType<ForestTree>(FindObjectsSortMode.None).Single(t => t.TreeId == id);
    Transform Paint(ForestTree tree) => tree.transform.Find("Orange Rack Entrance Mark");
    string Orders() => string.Join("|", manager.WorkOrders.Select(o => o.workOrderId + ":" + o.status + ":" + o.targetTreeId));
    string SaveJson() => JsonUtility.ToJson(saves.CaptureData());

    public static void SetGameSize(int width, int height)
    {
        Assembly assembly = typeof(Editor).Assembly;
        Type sizesType = assembly.GetType("UnityEditor.GameViewSizes");
        Type singleton = typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
        object sizes = singleton.GetProperty("instance", BindingFlags.Public | BindingFlags.Static).GetValue(null);
        MethodInfo getGroup = sizesType.GetMethod("GetGroup");
        object group = getGroup.Invoke(sizes, new[] { Enum.ToObject(getGroup.GetParameters()[0].ParameterType, 0) });
        Type sizeType = assembly.GetType("UnityEditor.GameViewSize"), kind = assembly.GetType("UnityEditor.GameViewSizeType");
        object size = Activator.CreateInstance(sizeType, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
            null, new object[] { Enum.ToObject(kind, 1), width, height, "RackField " + width + "x" + height }, null);
        int index = (int)group.GetType().GetMethod("GetTotalCount").Invoke(group, null);
        group.GetType().GetMethod("AddCustomSize").Invoke(group, new[] { size });
        Type viewType = assembly.GetType("UnityEditor.GameView"); EditorWindow view = EditorWindow.GetWindow(viewType);
        viewType.GetProperty("selectedSizeIndex", Private | BindingFlags.Public).SetValue(view, index);
        var gizmos = viewType.GetProperty("showGizmos", Private | BindingFlags.Public);
        if (gizmos != null && gizmos.CanWrite) gizmos.SetValue(view, false);
        view.maximized = true; view.Show(); view.Focus(); view.Repaint();
    }

    IEnumerator Start()
    {
        yield return Settle(10);
        output = Environment.GetEnvironmentVariable("CCF_DIAG_DIR");
        Directory.CreateDirectory(output);
        var stack = new Stack<IEnumerator>(); stack.Push(Run()); Exception failure = null;
        while (stack.Count > 0)
        {
            bool more; object current = null;
            try { more = stack.Peek().MoveNext(); if (more) current = stack.Peek().Current; }
            catch (Exception error) { failure = error; break; }
            if (!more) stack.Pop(); else if (current is IEnumerator next) stack.Push(next); else yield return current;
        }
        if (failure != null && marks != null)
        {
            File.WriteAllText(Path.Combine(output, "failure-state.txt"), "count=" + manager.MarkedRackEntranceCount
                + " message=" + marks.TransientMessage + " prompt=" + marks.RackSprayPrompt()
                + " camera=" + Camera.main.transform.position + " player=" + player.transform.position + "\n" + failure);
            yield return Capture("FAIL-state");
        }
        File.WriteAllLines(Path.Combine(output, "captures.txt"), captures);
        File.WriteAllText(Path.Combine(Environment.GetEnvironmentVariable("CCF_DRIVER_DIR"), "finished"), failure == null ? "pass" : "fail");
        Debug.Log(failure == null ? "RACK_V2_FIELD_MARKING_PASS checks=" + checks + " shots=" + shots
            : "RACK_V2_FIELD_MARKING_FAIL checks=" + checks + " exception=" + failure);
        EditorApplication.ExitPlaymode(); EditorApplication.Exit(failure == null ? 0 : 1);
    }

    IEnumerator Aim(ForestTree tree, float distance = 2.2f, Vector3 direction = default)
    {
        if (direction == default) direction = Vector3.left;
        var controller = player.GetComponent<CharacterController>();
        if (controller != null) controller.enabled = false;
        Vector3 p = tree.transform.position + direction.normalized * distance;
        player.transform.position = new Vector3(p.x, tree.transform.position.y + 0.2f, p.z);
        if (controller != null) controller.enabled = true;
        player.LookToward(tree.transform.position + Vector3.up * 1.4f);
        Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false;
        Physics.SyncTransforms();
        yield return Settle();
        player.LookToward(tree.transform.position + Vector3.up * 1.4f);
        yield return Settle(3);
    }

    // Establish an independent long ray before testing the production short ray. The original
    // westward 6 m pose looked through a nearer unrelated plantation tree (field-03 evidence).
    ForestTree FirstTreeOnLongRay()
    {
        foreach (RaycastHit hit in Physics.RaycastAll(Camera.main.transform.position, Camera.main.transform.forward, 9f,
                     Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore).OrderBy(h => h.distance))
        {
            if (hit.collider.transform.IsChildOf(player.transform)) continue;
            if (hit.collider.GetComponentInParent<ScenarioHabitatVisuals>() != null) continue;
            if (hit.collider.GetComponentInParent<ForestEcologyController>() != null
                && hit.collider.GetComponentInParent<ForestTree>() == null) continue;
            return hit.collider.GetComponentInParent<ForestTree>();
        }
        return null;
    }

    IEnumerator Capture(string name)
    {
        yield return new WaitForEndOfFrame();
        Texture2D image = ScreenCapture.CaptureScreenshotAsTexture();
        Check(image != null && image.width == Screen.width && image.height == Screen.height, "rendered size " + name);
        string file = name + "-" + Screen.width + "x" + Screen.height + ".png";
        File.WriteAllBytes(Path.Combine(output, file), image.EncodeToPNG());
        bool visible = false;
        for (int y = 0; y < image.height && !visible; y += 25)
            for (int x = 0; x < image.width && !visible; x += 25)
            { Color c = image.GetPixel(x, y); visible = c.maxColorComponent > 0.05f; }
        Destroy(image); Check(visible, "capture has visible pixels " + name);
        captures.Add(file); shots++;
    }

    IEnumerator Run()
    {
        ui = FindFirstObjectByType<ScenarioOneUiRoot>(); manager = ui.Manager; marks = ui.Marking;
        saves = FindFirstObjectByType<ForestSaveController>(); player = ui.Player; ecology = ui.Ecology;
        Check(Path.GetFullPath(Application.persistentDataPath).StartsWith(Path.GetFullPath(Environment.GetEnvironmentVariable("XDG_CONFIG_HOME")) + Path.DirectorySeparatorChar), "isolated save/config path");
        while (ui.Help.Onboarding.IsOpen) { ui.Help.Onboarding.Next(); yield return Settle(2); }
        ui.CloseAll(); Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false;
        Check(manager.RackPlanningV2Active && ForestSaveData.CurrentVersion == 22, "fresh V2 with unchanged save v22");
        Check(!marks.EquipRackSpray(true), "cannot equip before confirmation");
        string fresh = SaveJson();
        Check(manager.ResetRacksToSuggestion(manager.GetSuggestedRackLayout()).Succeeded, "use existing qualified suggested geometry");
        Check(manager.ConfirmRackPlanFromMap().Succeeded, "map confirms five rack records");
        int[] ids = manager.RackPlan.Select(r => r.Id).ToArray();
        foreach (RackPlanRack rack in manager.ConfirmedRacks)
        {
            Check(manager.TryGetRackEntranceTarget(rack.Id, out ForestTree target, out _), "rack " + rack.Id + " resolves a real target");
            Check(rack.RemovalTreeIds.Contains(target.TreeId), "target is in authoritative removal set " + rack.Id);
            string independentlyNearest = rack.RemovalTreeIds.Select(Tree).Where(t => t.CanChop)
                .OrderBy(t => (new Vector2(t.transform.position.x - rack.Entrance.X, t.transform.position.z - rack.Entrance.Z)).sqrMagnitude)
                .ThenBy(t => t.TreeId, StringComparer.Ordinal).First().TreeId;
            Check(target.TreeId == independentlyNearest, "nearest access-end tree " + rack.Id);
            Vector2 start = new Vector2(rack.Entrance.X, rack.Entrance.Z), end = new Vector2(rack.Far.X, rack.Far.Z);
            Vector2 unit = (end - start).normalized, offset = new Vector2(target.transform.position.x, target.transform.position.z) - start;
            Check(Mathf.Abs(offset.x * -unit.y + offset.y * unit.x) <= rack.WidthM * 0.5f + 0.0001f, "target near existing centreline " + rack.Id);
        }
        yield return Settle();
        Check(FindFirstObjectByType<PlannedFellMarkers>().MarkerCount == 0, "V2 pending rack orders create no automatic ground discs/diamonds");
        Check(marks.CueRackId == ids[0], "cue uses first real unmarked rack");
        var target0 = Tree(manager.RackPlan[0].EntranceTreeId);
        yield return Aim(target0);
        yield return input.Send(RackAuthoringNativeInput.Op.Key("p"));
        Check(marks.RackSprayEquipped, "real P key equips spray marker");
        Check(marks.RackSprayPrompt().Contains("Spray orange"), "valid target feedback before spray");
        yield return Capture("01-valid-target");

        var unrelated = FindObjectsByType<ForestTree>(FindObjectsSortMode.None)
            .Where(t => t.IsLiving && !manager.Racks.Any(r => r.removalTreeIds.Contains(t.TreeId)))
            .OrderBy(t => (t.transform.position - target0.transform.position).sqrMagnitude).First();
        yield return Aim(unrelated);
        yield return input.Send(RackAuthoringNativeInput.Op.Click());
        Check(manager.MarkedRackEntranceCount == 0 && marks.TransientMessage.Contains("not a rack entrance"), "native spray unrelated tree rejected");
        bool distantView = false;
        for (int pose = 0; pose < 32 && !distantView; pose++)
        {
            float angle = pose * Mathf.PI / 16f;
            yield return Aim(target0, 5f, new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)));
            distantView = FirstTreeOnLongRay() == target0
                && Vector3.Distance(Camera.main.transform.position, target0.transform.position + Vector3.up * 1.4f) > 4.5f;
        }
        Check(distantView, "unobstructed real entrance beyond reach is independently established by a 9 m ray");
        Check(marks.RackSprayPrompt().Contains("3.5"), "unobstructed out-of-range entrance offers distance feedback");
        Debug.Log("RACKFIELD_RANGE before camera=" + Camera.main.transform.position + " target=" + target0.transform.position
            + " centre-distance=" + Vector3.Distance(Camera.main.transform.position, target0.transform.position + Vector3.up * 1.4f)
            + " prompt=" + marks.RackSprayPrompt());
        yield return input.Send(RackAuthoringNativeInput.Op.Click());
        Debug.Log("RACKFIELD_RANGE after count=" + manager.MarkedRackEntranceCount + " message=" + marks.TransientMessage
            + " camera=" + Camera.main.transform.position + " prompt=" + marks.RackSprayPrompt());
        Check(manager.MarkedRackEntranceCount == 0 && marks.TransientMessage.Contains("3.5"), "native spray beyond range rejected");
        yield return Aim(target0);
        player.LookToward(player.transform.position + Vector3.up * 10f + player.transform.forward);
        yield return Settle(3); yield return input.Send(RackAuthoringNativeInput.Op.Click());
        Check(manager.MarkedRackEntranceCount == 0, "native spray aimed away rejected");
        yield return Aim(target0);
        target0.SetMark(TreeMarkType.CropTree); // deliberately conflicting fixture; production prevents adding Crop to frozen removal set
        yield return input.Send(RackAuthoringNativeInput.Op.Click());
        Check(manager.MarkedRackEntranceCount == 0 && target0.IsCropTree && marks.TransientMessage.Contains("Crop Tree"), "Crop Tree cannot be sprayed orange");
        target0.SetMark(TreeMarkType.None);
        marks.Mark(target0, TreeMarkType.Fell, false);
        yield return input.Send(RackAuthoringNativeInput.Op.Click());
        Check(manager.MarkedRackEntranceCount == 0 && target0.IsMarkedForFell && marks.TransientMessage.Contains("Fell"), "Fell cannot be sprayed orange");
        marks.Unmark(target0, false);
        // A physical blocker between camera and target must defeat the use-time ray too.
        var blocker = GameObject.CreatePrimitive(PrimitiveType.Cube);
        blocker.transform.position = Vector3.Lerp(Camera.main.transform.position, target0.transform.position + Vector3.up * 1.4f, 0.5f);
        blocker.transform.localScale = Vector3.one * 0.7f; Physics.SyncTransforms();
        yield return input.Send(RackAuthoringNativeInput.Op.Click());
        Check(manager.MarkedRackEntranceCount == 0, "intervening collider prevents spray through objects");
        Destroy(blocker); yield return Settle(3);
        bool rngUnchanged;
        UnityEngine.Random.State rng = UnityEngine.Random.state;
        Check(marks.TrySprayRackEntrance().Status == RackPlanOperationStatus.Applied, "valid use completes first mark");
        rngUnchanged = JsonUtility.ToJson(rng) == JsonUtility.ToJson(UnityEngine.Random.state);
        Check(rngUnchanged, "spray feedback does not consume simulation RNG");
        Check(marks.RackSprayFeedbackActive && marks.GetComponents<AudioSource>().Any(a => a.isPlaying), "short spray feedback is active");
        yield return Settle();
        Check(Paint(target0) != null && Paint(target0).childCount == 4, "one use creates exactly four orange stripes");
        Check(target0.MarkType == TreeMarkType.None && !marks.IsMarked(target0), "orange is rack-owned, never a loose Fell/Crop enum or ID");
        Check(marks.CueRackId == ids[1], "successful spray clears first cue and points to next real target");
        marks.Mark(target0, TreeMarkType.Fell, false); marks.Mark(target0, TreeMarkType.CropTree, false); marks.Unmark(target0, false);
        Check(marks.HasRackEntranceMark(target0) && target0.MarkType == TreeMarkType.None && Paint(target0).childCount == 4, "reciprocal Fell/Crop/unmark cannot overwrite orange");
        marks.ClearAll(); Check(marks.HasRackEntranceMark(target0), "clearing loose Fell treatment cannot erase rack mark");
        yield return Capture("02-orange-first-mark");
        string partial = SaveJson();
        Check(saves.LoadData(JsonUtility.FromJson<ForestSaveData>(partial), false), "focused partial-mark v21 save reload");
        yield return Settle();
        target0 = Tree(manager.RackPlan[0].EntranceTreeId);
        Check(manager.MarkedRackEntranceCount == 1 && marks.HasRackEntranceMark(target0) && Paint(target0) != null, "partial mark count and four-side paint restored from existing rack fields");
        // Rehydration must ignore the previous world's rack locks while restoring incoming ordinary marks.
        var ordinary = JsonUtility.FromJson<ForestSaveData>(fresh);
        ordinary.trees.Single(t => t.treeId == target0.TreeId).markType = (int)TreeMarkType.CropTree;
        Check(saves.LoadData(ordinary, false), "load ordinary Crop world over old orange state");
        Check(Tree(target0.TreeId).IsCropTree, "incoming ordinary Crop preserved despite stale prior rack ownership");
        Check(saves.LoadData(JsonUtility.FromJson<ForestSaveData>(partial), false), "restore partial-mark fixture");
        yield return Settle();
        marks.EquipRackSpray(true);
        // Real native single click on second target; this is the production Update/aim/range path.
        ForestTree second = Tree(manager.RackPlan[1].EntranceTreeId);
        yield return Aim(second);
        yield return input.Send(RackAuthoringNativeInput.Op.Click());
        Check(manager.MarkedRackEntranceCount == 2 && Paint(second) != null && Paint(second).childCount == 4, "one real left-click creates complete mark and increments authoritative derived count");
        Check(!player.IsInspecting, "spray click does not also open inspection");
        foreach (RackPlanRack rack in manager.RackPlan.Skip(2))
        {
            yield return Aim(Tree(rack.EntranceTreeId));
            Check(marks.TrySprayRackEntrance().Status == RackPlanOperationStatus.Applied, "valid spray rack " + rack.Id);
        }
        yield return Settle();
        Check(manager.MarkedRackEntranceCount == manager.Racks.Count(r => r.entranceMarked) && manager.MarkedRackEntranceCount == 5 && marks.CueRackId == 0,
            "all five marked deterministically from rack records; cue gone");
        Check(!marks.TryGetRackFieldTarget(out _, out _, out _), "no target after final mark");
        string full = SaveJson();
        Check(saves.LoadData(JsonUtility.FromJson<ForestSaveData>(full), false), "all-marked existing v21 save reload"); yield return Settle();
        Check(manager.MarkedRackEntranceCount == 5 && manager.Racks.All(r => Paint(Tree(r.entranceTreeId))?.childCount == 4), "all five orange marks survive reload");
        string orders = Orders();
        Check(manager.ReopenRackPlan().Succeeded && Orders() == orders && manager.Racks.All(r => r.entranceMarked), "reopening cancels nothing and retains all marks");
        yield return Settle();
        Check(manager.Racks.All(r => Paint(Tree(r.entranceTreeId)) != null), "reopened unchanged geometry keeps visible paint");
        Check(manager.ConfirmRackPlanFromMap().Succeeded && manager.MarkedRackEntranceCount == 5 && Orders() == orders, "unchanged reconfirm retains marks/orders");
        Check(manager.ReopenRackPlan().Succeeded, "reopen for single geometry edit");
        RackPlanRack first = manager.RackPlan[0]; ForestTree obsolete = Tree(first.EntranceTreeId);
        RackPlanOperationResult changed = manager.ReshapeRackFromMap(first.Id, first.Entrance, new RackPoint(first.Far.X - 6f, first.Far.Z));
        Check(changed.Status == RackPlanOperationStatus.Applied && changed.MarksCleared == 1, "one geometry change clears only its geometry-bound mark");
        yield return Settle();
        Check(!manager.Racks[0].entranceMarked && Paint(obsolete) == null && manager.Racks.Skip(1).All(r => r.entranceMarked && Paint(Tree(r.entranceTreeId)) != null), "obsolete orange removed; four unchanged rack marks retained");
        Check(manager.ConfirmRackPlanFromMap().Succeeded && manager.MarkedRackEntranceCount == 4, "reconfirmed count follows rack objects after edit");
        ScenarioOneWorkOrder completed = manager.WorkOrders.First(o => o.rackId == ids[1]); completed.status = ScenarioWorkStatus.Completed;
        Check(manager.ReopenRackPlan().Succeeded, "reopen with one executed-removal rack");
        RackPlanRack locked = manager.RackPlan[1];
        Check(locked.IsLocked && manager.ReshapeRackFromMap(locked.Id, locked.Entrance, new RackPoint(locked.Far.X - 6f, locked.Far.Z)).Status == RackPlanOperationStatus.Locked,
            "executed-removal geometry remains locked");
        Check(completed.status == ScenarioWorkStatus.Completed && manager.Racks[1].entranceMarked, "executed removal and unchanged mark never reversed");

        // Tampered membership/identity is rejected; no substitute is selected at field-use time.
        Check(saves.LoadData(JsonUtility.FromJson<ForestSaveData>(partial), false), "reset target-negative fixture"); yield return Settle();
        RackSaveData broken = manager.Racks[1]; string original = broken.entranceTreeId;
        broken.entranceTreeId = unrelated.TreeId;
        Check(!manager.TryGetRackEntranceTarget(broken.rackId, out _, out string problem) && problem.StartsWith("RACK INVALID — ADJUST PLAN"), "unrelated identity outside removal set is invalid");
        broken.entranceTreeId = broken.removalTreeIds.Last();
        Check(!manager.TryGetRackEntranceTarget(broken.rackId, out _, out _), "later removal tree cannot substitute for first tree");
        broken.entranceTreeId = original;
        // Representative rendered first-target and all four viewing directions, both required sizes.
        foreach (Vector2Int size in new[] { new Vector2Int(1280,720), new Vector2Int(1920,1080) })
        {
            SetGameSize(size.x, size.y); yield return Settle(20);
            marks.EquipRackSpray(true);
            for (int side = 0; side < 4; side++)
            {
                target0 = Tree(manager.RackPlan[0].EntranceTreeId);
                float angle = side * Mathf.PI * 0.5f;
                yield return Aim(target0, 2.2f, new Vector3(Mathf.Cos(angle),0f,Mathf.Sin(angle)));
                yield return Capture("03-orange-side-" + side);
            }
        }
        // Ordinary Fell/Crop public API outside a rack remains unchanged.
        marks.EquipRackSpray(false);
        marks.Mark(unrelated, TreeMarkType.Fell, false); Check(unrelated.IsMarkedForFell && marks.IsMarked(unrelated), "ordinary Fell still works");
        marks.Mark(unrelated, TreeMarkType.CropTree, false); Check(unrelated.IsCropTree && !marks.IsMarked(unrelated), "ordinary Crop replaces Fell exclusively as before");
        marks.ToggleMark(unrelated); Check(unrelated.IsCropTree, "ordinary Fell toggle cannot overwrite Crop");
        marks.ToggleCropTree(unrelated); Check(unrelated.MarkType == TreeMarkType.None, "ordinary Crop toggle removes Crop");
        Check(saves.LoadData(JsonUtility.FromJson<ForestSaveData>(fresh), false), "reset to empty fresh state");
        // The unchanged v21 save gate covers historical load policy; here switch the actual
        // ecology geometry through its production API and verify field-tool exclusion.
        ecology.ApplyStandGeometry(StandGeometryModel.Legacy40);
        manager.RestoreSaveData(null, 20);
        yield return Settle();
        Check(!manager.RackPlanningV2Active && manager.RackPlanningModelVersion == RackPlanningModel.Legacy && marks.CueRackId == 0
            && !marks.EquipRackSpray(true) && !marks.TryGetRackFieldTarget(out _,out _,out _), "actual Legacy40/model-0 never enters V2 spray or cue");
        Check(!ControlsReference.RowsFor(false).Any(r => r[0] == "P") && ControlsReference.RowsFor(false).Any(r => r[0] == "B"), "legacy controls remain intact");
    }
}
#endif
