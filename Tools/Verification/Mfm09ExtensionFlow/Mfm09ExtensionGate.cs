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
using UnityEngine.UIElements;
using Approval = ScenarioOnePrescriptionApproval;
using Cand = ScenarioOneCropTreeCandidates;

// Disposable Tier-2 gate for MFM-09 "Extend your treatment". Verification only: it drives the production UI and manager APIs and writes
// nothing but evidence files (and one slot in the ISOLATED config directory for the disk round trip). Stage with the runner, which removes
// the source, .meta files and licence copy afterwards.
//
// World building and the Treatment review are the accepted MFM-08 Tier-2 path (same production APIs, same declared fixtures). What this
// gate adds: the live teaching path through Lessons 1-5 (a real [E] inspection and real [J] decisions earn the v24 first-guided-review
// latch), the extension appearing only after that, completion derived from the approved prescription, ordinary Pending work with provenance
// in the Work Plan, Work Plan approval as the only commitment step, a year boundary, a disk save/load, and non-back-credit of the sample.
public static class Mfm09ExtensionGate
{
    public const string Marker = "Mfm09ExtensionGate.Begin";

    public static void Begin()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        Mfm09ExtensionRunner.SetGameSize(1280, 720);
        EditorApplication.isPlaying = true;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Prepare()
    {
        if (!Environment.GetCommandLineArgs().Contains(Marker)) return;
        foreach (MenuHelpView.Menu menu in Enum.GetValues(typeof(MenuHelpView.Menu))) PlayerPrefs.SetInt(MenuHelpView.PreferencePrefix + menu, 1);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (Environment.GetCommandLineArgs().Contains(Marker))
            new GameObject("Disposable MFM-09 extension gate").AddComponent<Mfm09ExtensionRunner>();
    }
}

public sealed class Mfm09ExtensionRunner : MonoBehaviour
{
    private static readonly string[] Entrances = { "PO0203", "PO1204", "PO2003", "PO2803", "PO3803" };
    private static readonly float[] QualifiedZ = { -35.2711754f, -16.4217644f, 0.2382353f, 15.7935295f, 35.2323532f };
    private static readonly string[] SampleCrops = { "P0810", "P1206", "P1214" };
    private static readonly Vector2Int[] Sizes = { new Vector2Int(1280, 720), new Vector2Int(1920, 1080) };
    private static readonly BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;

    private ScenarioOneUiRoot ui;
    private ScenarioOneManager manager;
    private ForestEcologyController ecology;
    private ForestSaveController saves;
    private ForestTreeMarkingManager marks;
    private ForestPlayer player;
    private ForestSaveData original;
    private bool saveControllerWasEnabled;
    private string output;
    private int checks, shots;
    private readonly RackAuthoringNativeInput input = new RackAuthoringNativeInput();
    private readonly List<string> captures = new List<string>();
    private readonly List<string> notes = new List<string>();

    private void Check(bool ok, string what, string detail = "")
    {
        detail = detail ?? "";
        if (!ok) throw new InvalidOperationException(what + (detail.Length == 0 ? "" : " [" + detail + "]"));
        checks++;
        Debug.Log("MFM09_CHECK " + what + (detail.Length == 0 ? "" : " [" + detail + "]"));
    }

    private void Note(string text) { notes.Add(text); Debug.Log("MFM09_NOTE " + text); }

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
            null, new object[] { Enum.ToObject(kind, 1), width, height, "MFM09 " + width + "x" + height }, null);
        int index = (int)group.GetType().GetMethod("GetTotalCount").Invoke(group, null);
        group.GetType().GetMethod("AddCustomSize").Invoke(group, new[] { size });
        Type viewType = assembly.GetType("UnityEditor.GameView"); EditorWindow view = EditorWindow.GetWindow(viewType);
        viewType.GetProperty("selectedSizeIndex", Private | BindingFlags.Public).SetValue(view, index);
        var gizmos = viewType.GetProperty("showGizmos", Private | BindingFlags.Public);
        if (gizmos != null && gizmos.CanWrite) gizmos.SetValue(view, false);
        view.maximized = true; view.Show(); view.Focus(); view.Repaint();
    }

    private static IEnumerator Settle(int frames = 14) { for (int i = 0; i < frames; i++) yield return null; }

    private IEnumerator Start()
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
        if (failure != null) yield return Capture("FAIL-state");
        try
        {
            if (original != null) { saves.LoadData(original, false); saves.enabled = saveControllerWasEnabled; }
        }
        catch (Exception error) { failure = failure ?? error; }
        File.WriteAllLines(Path.Combine(output, "captures.txt"), captures);
        File.WriteAllLines(Path.Combine(output, "notes.txt"), notes);
        File.WriteAllText(Path.Combine(Environment.GetEnvironmentVariable("CCF_DRIVER_DIR"), "finished"), failure == null ? "pass" : "fail");
        Debug.Log(failure == null ? "MFM09_UNITY_PASS checks=" + checks + " shots=" + shots : "MFM09_UNITY_FAIL checks=" + checks + " shots=" + shots + " " + failure);
        EditorApplication.ExitPlaymode(); EditorApplication.Exit(failure == null ? 0 : 1);
    }

    // ------------------------------------------------------------------------------------------------------------------- helpers

    private static ForestTree[] LiveTrees() => FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
    private ForestTree Tree(string id) => LiveTrees().FirstOrDefault(t => t.TreeId == id);
    private VisualElement Root => ui.RootElement;
    private StandMapTreatmentReview Review => ui.Map.Review;
    private string SavePath => Path.Combine(Application.persistentDataPath, "forest-save.json");

    // The lesson view caches its facts for a quarter second; the gate forces a recompute so every read reflects the world as it is now.
    // (Reading is the only effect: the facts themselves are derived and never written.)
    private LearningObjectivesView Learn
    {
        get
        {
            typeof(LearningObjectivesView).GetField("sequenceFacts", Private).SetValue(ui.Learning, null);
            return ui.Learning;
        }
    }

    private ScenarioOneTeachingSequence.Facts LiveFacts()
    {
        MethodInfo method = typeof(LearningObjectivesView).GetMethod("TeachingFacts", Private);
        return (ScenarioOneTeachingSequence.Facts)method.Invoke(Learn, null);
    }

    private ScenarioOneRackBay.Analysis Bay()
    {
        float half = StandGeometryModel.StandSizeMeters(ecology.StandGeometryModelVersion) * 0.5f;
        return ScenarioOneRackBayReader.Read(LiveTrees(), manager.WorkOrders, ScenarioOneRackSampleGeometry.Derive(manager.ConfirmedRacks, half));
    }

    private List<Label> Labels() => Root.Query<Label>().ToList().Where(l => l.resolvedStyle.display != DisplayStyle.None).ToList();
    private bool HasText(string fragment) => Labels().Any(l => l.text != null && l.text.Contains(fragment));
    private string AllText() => string.Join("\n", Labels().Select(l => l.text));
    private List<Button> Buttons(string text) => Root.Query<Button>().ToList().Where(b => b.text == text && b.resolvedStyle.display != DisplayStyle.None).ToList();
    private Button Named(string name) => Root.Q<Button>(name);
    private Foldout Fold(string name) => Root.Q<Foldout>(name);
    private static string FoldText(Foldout f) => f == null ? "" : string.Join("\n", f.Query<Label>().ToList().Select(l => l.text));

    private IEnumerator Click(Button button, string what)
    {
        Check(button != null && button.enabledInHierarchy, "UI action available: " + what);
        typeof(Clickable).GetMethod("Invoke", Private).Invoke(button.clickable, new object[] { null });
        yield return Settle(8);
        ui.Map.Refresh(true);
        yield return Settle(4);
    }

    private IEnumerator OpenReview()
    {
        ui.CloseAll();
        ui.ShowMap();
        yield return Settle(10);
        Button tab = Buttons("Treatment review").FirstOrDefault();
        Check(tab != null, "Treatment review tab is offered on the Rack V2 Stand Map");
        if (!tab.ClassListContains("btn-selected")) typeof(Clickable).GetMethod("Invoke", Private).Invoke(tab.clickable, new object[] { null });
        yield return Settle(10);
        ui.Map.Refresh(true);
        yield return Settle(6);
    }

    private IEnumerator CloseMapToForest()
    {
        ui.CloseAll();
        UnityEngine.Cursor.lockState = CursorLockMode.Locked; UnityEngine.Cursor.visible = false;
        yield return Settle(6);
    }

    // Walk to a tree and inspect it with a real [E] key. Tries eight approach directions until the production aim ray names this tree.
    private IEnumerator InspectByKey(ForestTree tree)
    {
        if (player.IsInspecting)
        {
            yield return input.Send(RackAuthoringNativeInput.Op.Key("Escape"));
            yield return Settle(8);
            Check(!player.IsInspecting, "real [Esc] ends the previous inspection");
        }
        for (int side = 0; side < 8; side++)
        {
            float angle = side * Mathf.PI / 4f;
            var controller = player.GetComponent<CharacterController>();
            if (controller != null) controller.enabled = false;
            Vector3 p = tree.transform.position + new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * 2.6f;
            player.transform.position = new Vector3(p.x, tree.transform.position.y + 0.2f, p.z);
            if (controller != null) controller.enabled = true;
            player.LookToward(tree.transform.position + Vector3.up * 1.4f);
            Physics.SyncTransforms();
            yield return Settle(10);
            player.LookToward(tree.transform.position + Vector3.up * 1.4f);
            yield return Settle(3);
            if (player.AimedTree == tree) break;
        }
        Check(player.AimedTree == tree, "production aim ray names the intended tree", tree.TreeId + " aimed=" + (player.AimedTree != null ? player.AimedTree.TreeId : "none"));
        yield return input.Send(RackAuthoringNativeInput.Op.Key("e"));
        yield return Settle(10);
        Check(player.InspectedTree == tree, "real [E] key inspects the tree in the forest", tree.TreeId);
    }

    private IEnumerator Capture(string name)
    {
        yield return new WaitForEndOfFrame();
        Texture2D image = ScreenCapture.CaptureScreenshotAsTexture();
        if (image == null) yield break;
        string file = name + "-" + Screen.width + "x" + Screen.height + ".png";
        File.WriteAllBytes(Path.Combine(output, file), image.EncodeToPNG());
        bool visible = false;
        for (int y = 0; y < image.height && !visible; y += 25)
            for (int x = 0; x < image.width && !visible; x += 25) { Color c = image.GetPixel(x, y); visible = c.maxColorComponent > 0.05f; }
        bool sized = image.width == Screen.width && image.height == Screen.height;
        Destroy(image);
        captures.Add(file + (sized && visible ? "" : " [INVALID]"));
        shots++;
        if (name != "FAIL-state") { Check(sized, "rendered size matches the game view " + file); Check(visible, "capture has visible pixels " + file); }
    }

    // One state at both required sizes. `layout` (optional) is asserted at each size, after the layout has settled.
    private IEnumerator Shot(string name, Action<Vector2Int> layout = null, Action refresh = null)
    {
        foreach (Vector2Int size in Sizes)
        {
            SetGameSize(size.x, size.y);
            yield return Settle(24);
            Check(Screen.width == size.x && Screen.height == size.y, "game view is " + size.x + "x" + size.y, Screen.width + "x" + Screen.height);
            if (refresh != null) { refresh(); yield return Settle(8); }
            if (layout != null) layout(size);
            yield return Capture(name);
        }
    }

    // A labelled element is laid out, non-empty and inside the window at this size (readability that does not depend on a screenshot).
    private void InsideWindow(VisualElement element, string what, Vector2Int size)
    {
        Check(element != null && element.resolvedStyle.display != DisplayStyle.None, what + " exists at " + size.x + "x" + size.y);
        Rect w = element.worldBound, window = Root.worldBound;
        Check(!float.IsNaN(w.width) && w.width > 20f && w.height > 8f, what + " is laid out at " + size.x + "x" + size.y, w.ToString());
        Check(w.xMin >= window.xMin - 1f && w.xMax <= window.xMax + 1f && w.yMin >= window.yMin - 1f, what + " starts inside the window at " + size.x + "x" + size.y, w + " in " + window);
    }

    // True only when the element and every ancestor are displayed (a view that is closed is hidden by an ancestor, not by the element itself).
    private static bool Shown(VisualElement element)
    {
        for (VisualElement e = element; e != null; e = e.parent)
            if (e.resolvedStyle.display == DisplayStyle.None || e.resolvedStyle.visibility == Visibility.Hidden) return false;
        return element != null;
    }

    private string Identity()
    {
        ForestSaveData data = saves.CaptureData();
        return string.Join("\n", new[]
        {
            ScenarioReferenceArchive.WorldHash(data),
            string.Join(",", marks.GetMarkedIds().OrderBy(x => x, StringComparer.Ordinal)),
            manager.WorkOrders.Count + "/" + manager.ManagementEvents.Count + "/" + manager.NextPrescriptionId + "/" + manager.CashCents
        });
    }

    private static string TeachingKey(ScenarioOneTeachingSequence.Facts f)
        => ScenarioOneTeachingSequence.Current(f) + "|" + ScenarioOneTeachingSequence.Presented(f) + "|" + ScenarioOneTeachingSequence.ExtensionOffered(f);

    // ----------------------------------------------------------------------------------------------------------------------- the run

    private IEnumerator Run()
    {
        float deadline = Time.realtimeSinceStartup + 30f;
        do
        {
            ui = FindFirstObjectByType<ScenarioOneUiRoot>();
            manager = ui != null ? ui.Manager : null;
            ecology = FindFirstObjectByType<ForestEcologyController>();
            saves = FindFirstObjectByType<ForestSaveController>();
            marks = ui != null ? ui.Marking : null;
            player = ui != null ? ui.Player : null;
            if (ui != null && manager != null && ecology != null && saves != null && marks != null && player != null) break;
            yield return null;
        } while (Time.realtimeSinceStartup < deadline);
        Check(ui != null && manager != null && ecology != null && saves != null && marks != null && player != null, "scene components ready");
        yield return Settle(20);
        string config = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME");
        Check(!string.IsNullOrEmpty(config) && Application.persistentDataPath.StartsWith(config.TrimEnd('/') + "/", StringComparison.Ordinal), "isolated persistentDataPath", Application.persistentDataPath);
        Check(!File.Exists(SavePath), "the isolated slot starts empty");
        saveControllerWasEnabled = saves.enabled;
        original = saves.CaptureData();
        saves.enabled = false;
        while (ui.Help.Onboarding.IsOpen) { ui.Help.Onboarding.Next(); yield return Settle(2); }
        Check(manager.RackPlanningV2Active && ecology.StandGeometryModelVersion == StandGeometryModel.Enlarged80, "production Enlarged80 Rack V2 world");
        Check(ForestSaveData.CurrentVersion == 24, "unchanged save version 24", ForestSaveData.CurrentVersion.ToString());
        ui.CloseAll(); ui.CloseHelp();
        yield return Settle();

        // ---- 1. the world before the sample exists: the extension is not offered, the lesson count is the extended one ----
        var f0 = LiveFacts();
        Check(f0.ExtensionApplies == (manager.RackPlanningV2Active && manager.RackPlanConfirmed), "ExtensionApplies is derived from the Rack V2 confirmed plan");
        Check(!manager.FirstGuidedRackReviewComplete && !ScenarioOneTeachingSequence.ExtensionOffered(f0), "before any work the first guided review is not complete and the extension is not offered");

        // ---- 2. the sample, built through production APIs (the accepted MFM-08 path) ----
        float half = StandGeometryModel.StandSizeMeters(ecology.StandGeometryModelVersion) * 0.5f;
        for (int i = 0; i < Entrances.Length; i++)
        {
            ForestTree entrance = Tree(Entrances[i]);
            Check(entrance != null && Math.Abs(entrance.transform.position.z - QualifiedZ[i]) < 1e-4f, "qualified entrance tree present", Entrances[i]);
            var added = manager.AddRack(new RackPoint(ScenarioOneHeadlandGeometry.HeadlandX, entrance.transform.position.z), new RackPoint(half, entrance.transform.position.z));
            Check(added.Succeeded, "rack added", added.Message);
        }
        Check(manager.ConfirmRackPlan().Succeeded, "rack plan confirmed");
        Check(LiveFacts().ExtensionApplies, "ExtensionApplies becomes true once the rack plan is confirmed");
        Check(manager.ApprovePendingWork(), "rack access approved through the Work Plan", manager.Feedback);
        Check(manager.AdvanceYear(), "rack access executed", manager.Feedback);
        yield return Settle();
        manager.MarkAnnualReviewSeen();
        foreach (string id in SampleCrops)
        {
            ForestTree crop = Tree(id);
            Check(crop != null && crop.IsLiving, "sample Crop Tree is alive after access", id);
            marks.Mark(crop, TreeMarkType.CropTree, false);
        }
        yield return Settle();
        ScenarioOneRackBay.Analysis bay = Bay();
        Check(bay != null && bay.Applies && bay.HasEnoughFuture, "three distributed sample Crop Trees qualify");
        for (int guard = 0; guard < 150; guard++)
        {
            bay = Bay();
            if (bay.MinPlannedAndExecuted >= ScenarioOneRackBay.FirstReleaseShare) break;
            var f = bay.Future.OrderBy(t => t.ReleaseShare).First();
            ForestTree next = LiveTrees().Where(t => t.IsLiving && !t.IsCropTree && !manager.IsPlannedForFelling(t.TreeId)
                    && bay.Contains(t.transform.position.x, t.transform.position.z)
                    && Vector2.Distance(new Vector2(f.X, f.Z), new Vector2(t.transform.position.x, t.transform.position.z)) <= 8)
                .OrderByDescending(t => ForestEcologyController.HegyiTerm(t.Diameter, f.DbhCm,
                    Vector2.Distance(new Vector2(f.X, f.Z), new Vector2(t.transform.position.x, t.transform.position.z))))
                .ThenBy(t => t.TreeId, StringComparer.Ordinal).First();
            marks.Mark(next, TreeMarkType.Fell, false);
            manager.AddMarkedTreesToWorkPlan();
        }
        bay = Bay();
        Check(bay.ReleasePlanned && bay.CanopyKept, "in-footprint 30% release planned within the canopy limit");
        // The original Lesson 4 also asks for retained deadwood: one planned release removal is kept as fallen deadwood (production setter).
        var releaseOrder = manager.WorkOrders.First(o => o.IsOpen && o.type == ScenarioWorkType.FellTree);
        Check(manager.SetPendingFellingOutcome(releaseOrder.workOrderId, FellingMaterialOutcome.RetainAsFallenDeadwood), "one release removal kept as fallen deadwood");
        Check(Bay().MixedPlanned, "sold and deadwood outcomes both counted inside the sample");

        // ---- 3. comparable-area work BEFORE the first guided review cannot award original sample credit ----
        yield return Settle(8);
        string teachingBeforeOutside = TeachingKey(LiveFacts());
        int futureBefore = bay.Future.Count;
        ScenarioOneRackBay.Analysis sampleBay = Bay();
        ForestTree outside = LiveTrees().Where(t => t.IsLiving && !t.IsCropTree && !manager.IsPlannedForFelling(t.TreeId) && !sampleBay.Contains(t.transform.position.x, t.transform.position.z))
            .OrderBy(t => t.TreeId, StringComparer.Ordinal).First();
        marks.Mark(outside, TreeMarkType.Fell, false);
        int addedOutside = manager.AddMarkedTreesToWorkPlan();
        var outsideOrder = manager.WorkOrders.FirstOrDefault(o => o.IsOpen && o.targetTreeId == outside.TreeId);
        Check(addedOutside >= 1 && outsideOrder != null, "an ordinary Fell order outside the sample exists", outside.TreeId);
        yield return Settle(8);
        var fOutside = LiveFacts();
        Check(TeachingKey(fOutside) == teachingBeforeOutside, "outside-sample work changes no teaching stage and offers no extension", TeachingKey(fOutside));
        Check(Bay().Future.Count == futureBefore && Bay().ExecutedRemovals == 0 && !Bay().ReleaseExecuted && !fOutside.ReleaseExecuted && !fOutside.RackReviewComplete,
            "outside-sample work awards no sample credit (future set, executed removals, release, review)");
        Check(manager.RemovePendingOrder(outsideOrder.workOrderId), "the outside order is removed through the ordinary path");
        marks.Unmark(outside, false);
        yield return Settle(8);

        Check(manager.ApprovePendingWork(), "sample release approved through the Work Plan", manager.Feedback);
        Check(manager.AdvanceYear(), "sample release executed", manager.Feedback);
        yield return Settle();
        manager.MarkAnnualReviewSeen();
        Check(Bay().ReleaseExecuted && Bay().MixedExecuted, "the sample's own release and deadwood outcome are executed");
        yield return Settle(20);

        // ---- 4. Lesson 5 (Review the response): the extension is NOT offered yet; the latch is not earned yet ----
        var f4 = LiveFacts();
        Check(ScenarioOneTeachingSequence.Current(f4) == 4 && Learn.CurrentTopicIndex == 4, "the accepted stage is Review the response (index 4)", Learn.CurrentTopicIndex.ToString());
        Check(ScenarioOneTeachingSequence.Presented(f4) == 4 && !ScenarioOneTeachingSequence.ExtensionOffered(f4), "Lesson 5 is shown, not the extension");
        Check(Learn.CurrentTitle == "5. Return and review", "Lesson 5 keeps its accepted title", Learn.CurrentTitle);
        Check(!Learn.CurrentInstruction.Contains("Treatment review") && !manager.FirstGuidedRackReviewComplete, "no extension wording and no latch before the review");
        ActiveObjectiveHudWidget.Model hud4 = Learn.HudModel();
        Check(hud4.Number == 5 && hud4.Total == 9, "the HUD counts the nine-lesson sequence", hud4.Number + "/" + hud4.Total);
        string worldBeforeReview = ScenarioReferenceArchive.WorldHash(saves.CaptureData());

        // ---- 5. the genuine response review: real [E] inspections and real [J] decisions earn the v24 latch ----
        yield return CloseMapToForest();
        foreach (string id in Bay().Future.Select(t => t.Id).ToArray())
        {
            yield return InspectByKey(Tree(id));
            yield return input.Send(RackAuthoringNativeInput.Op.Key("j"));
            yield return Settle(8);
        }
        Check(!manager.FirstGuidedRackReviewComplete, "three tree decisions alone do not earn the latch (stand portions still missing)", LiveFacts().ReviewDecisions + "/3 " + LiveFacts().StandPortionsReviewed + "/6");
        for (int part = 0; part < 6; part++)
        {
            var current = Bay();
            ForestTree tree = LiveTrees().Where(t => t.IsLiving && !t.IsMarkedForFell && !manager.IsPlannedForFelling(t.TreeId) && current.Portion(t.transform.position.x, t.transform.position.z) == part)
                .OrderBy(t => t.TreeId, StringComparer.Ordinal).FirstOrDefault();
            Check(tree != null, "a tree exists in stand portion " + part);
            yield return InspectByKey(tree);
            yield return input.Send(RackAuthoringNativeInput.Op.Key("j"));
            yield return Settle(8);
        }
        if (player.IsInspecting) { yield return input.Send(RackAuthoringNativeInput.Op.Key("Escape")); yield return Settle(8); }
        yield return Settle(20);
        Check(manager.FirstGuidedRackReviewComplete, "the genuine response review earned the D-066 first-guided-review latch");
        Check(ScenarioReferenceArchive.WorldHash(saves.CaptureData()) == worldBeforeReview, "the review changed no world hash (retain decisions cut nothing)");

        // ---- 6. the extension is now offered: after Lesson 5, before Secure the next generation ----
        var f6 = LiveFacts();
        Check(ScenarioOneTeachingSequence.Current(f6) == 5 && Learn.CurrentTopicIndex == 5, "the accepted stage index is 5 (recruitment); stage numbering is unchanged", Learn.CurrentTopicIndex.ToString());
        Check(ScenarioOneTeachingSequence.ExtensionOffered(f6) && ScenarioOneTeachingSequence.Presented(f6) == 5, "Extend your treatment is the lesson presented");
        Check(Learn.CurrentTitle == "6. Extend your treatment", "title and visible number", Learn.CurrentTitle);
        Check(Learn.CurrentInstruction == ScenarioOneTeachingSequence.Extension.Action, "initial instruction is the authored action", Learn.CurrentInstruction);
        ActiveObjectiveHudWidget.Model hud6 = Learn.HudModel();
        Check(hud6.Number == 6 && hud6.Total == 9 && hud6.Title.Contains("Extend your treatment"), "HUD shows lesson 6 of 9", hud6.Number + "/" + hud6.Total + " " + hud6.Title);
        Note("extension offered at teaching key " + TeachingKey(f6));
        ui.CloseAll(); ui.ShowObjectives(); yield return Settle(12);
        Check(Root.Q<Label>("current-objective-title") != null && Root.Q<Label>("current-objective-title").text == "6. Extend your treatment", "Objectives screen shows the extension as the current objective");
        Check(HasText("sample is teaching calibration, not a stocking prescription"), "the objective says the sample is not a stand-wide stocking prescription");
        Check(HasText("Approving the prescription is not approving the Work Plan"), "the objective says prescription approval is not Work Plan approval");
        Foldout completed = Fold("completed-lessons");
        if (completed != null) completed.value = true;
        yield return Settle(6);
        Check(FoldText(completed).Contains("5. Return and review") && !FoldText(completed).Contains("Extend your treatment"), "Completed lessons lists Lesson 5 and not the extension");
        yield return Shot("01-objectives-extension", size =>
        {
            InsideWindow(Root.Q<Label>("current-objective-title"), "extension title", size);
            Label progress = Root.Q<Label>("current-objective-progress");
            InsideWindow(progress, "extension progress line", size);
            ScrollView scroll = Root.Q<ScrollView>();
            Check(scroll != null, "the Objectives screen scrolls so long copy is never clipped at " + size.x + "x" + size.y);
        });

        // ---- 7. the latch survives the lesson count change and a disk round trip ----
        saves.Save();
        Check(File.Exists(SavePath), "Save() wrote the isolated slot");
        string diskReview = File.ReadAllText(SavePath);
        Check(diskReview.Contains("\"firstGuidedRackReviewComplete\": true"), "the saved slot carries the v24 latch");
        Check(!diskReview.ToLowerInvariant().Contains("extend-treatment") && !diskReview.ToLowerInvariant().Contains("extensionapproved") && !diskReview.ToLowerInvariant().Contains("extensionapplies"),
            "no MFM-09 field or lesson id appears in the save");
        saves.Load(); yield return Settle(20);
        Check(manager.FirstGuidedRackReviewComplete && ScenarioOneTeachingSequence.ExtensionOffered(LiveFacts()) && Learn.CurrentTitle == "6. Extend your treatment",
            "after reload the completed first review stays completed and the extension is still the lesson shown");
        Check(Learn.CurrentTopicIndex == 5, "reload does not reopen Lesson 5 (stage index stays 5)");

        // ---- 8. the Treatment review (accepted MFM-08 UX, consumed not changed) ----
        yield return OpenReview();
        Check(manager.Prescriptions.Count == 0 && manager.NextPrescriptionId == 1, "no prescription exists before the review");
        ScenarioOneCoverage.Result coverage = manager.ReadCoverage(out string problem);
        Check(coverage != null && coverage.Valid, "production coverage is valid", problem);
        int coveredCount = coverage.Areas.Count(a => !a.IsSource && a.State == ScenarioOneCoverage.State.Covered);
        Check(coveredCount > 0, "at least one Covered area", coveredCount + "/" + coverage.NonSourceCount);
        Check(HasText("Nothing here is marked, planned or approved yet"), "the review card keeps its own noncommitment note");
        Check(Buttons("Apply sample treatment here").Count == coveredCount, "Include is offered on Covered areas");
        Check(Learn.CurrentInstruction == ScenarioOneTeachingSequence.Extension.Action, "opening the review is not progress: the extension still asks for the same action");
        yield return Shot("02-treatment-review-card", size =>
        {
            ScrollView scroll = Root.Q<ScrollView>("map-review-scroll");
            Check(scroll != null && scroll.worldBound.height > 40f && scroll.worldBound.yMax <= Root.worldBound.yMax + 1f, "review card laid out inside the window at " + size.x + "x" + size.y);
        }, () => ui.Map.Refresh(true));

        Cand.Result batch = Review.Session.LastCandidates;
        Check(batch != null && batch.Valid && batch.Candidates.Count > 0, "spacing candidates exist in the covered areas", batch != null ? batch.Problem : "null");
        // Fixture (declared, as in the accepted MFM-08 gate): confirm the candidates through the production marking manager so a prescription can exist.
        // The comparable-area Crop Trees are confirmed AFTER the first guided review, so they must not move the sample or the lesson stage.
        string sampleFutureBefore = string.Join(",", Bay().Future.Select(t => t.Id).OrderBy(x => x, StringComparer.Ordinal));
        foreach (Cand.Candidate c in batch.Candidates)
        {
            ForestTree t = Tree(c.TreeId);
            if (t != null && t.IsLiving && !t.IsCropTree) marks.Mark(t, TreeMarkType.CropTree, false);
        }
        yield return Settle();
        Note("fixture: " + batch.Candidates.Count + " candidates confirmed through ForestTreeMarkingManager.Mark; confirmed Crop Trees total=" + LiveTrees().Count(t => t.IsCropTree));
        Check(string.Join(",", Bay().Future.Select(t => t.Id).OrderBy(x => x, StringComparer.Ordinal)) == sampleFutureBefore, "comparable-area Crop Trees do not join the sample's future trees (no back-credit)");
        Check(manager.FirstGuidedRackReviewComplete && ScenarioOneTeachingSequence.ExtensionOffered(LiveFacts()), "confirming comparable-area trees changes neither the latch nor the extension state");

        yield return OpenReview();
        coverage = manager.ReadCoverage(out problem);
        Check(coverage.Valid, "coverage still valid after confirming Crop Trees", problem);
        var coveredIds = coverage.Areas.Where(a => !a.IsSource && a.State == ScenarioOneCoverage.State.Covered).Select(a => a.Id).OrderBy(x => x, StringComparer.Ordinal).ToList();
        Check(coveredIds.Count > 0, "Covered areas remain", string.Join(",", coveredIds));
        for (int i = 0; i < coveredIds.Count; i++)
        {
            Button include = Root.Q<VisualElement>("map-review-area-" + coveredIds[i]).Query<Button>().ToList().First(b => b.text == "Apply sample treatment here");
            yield return Click(include, "Include area " + coveredIds[i]);
        }
        Check(Review.Session.IncludedCount == coveredIds.Count && manager.Prescriptions.Count == 0 && manager.WorkOrders.Count(o => o.IsOpen) == 0, "choosing areas wrote no record, order or mark");
        string identityBeforePrepare = Identity();
        yield return Click(Named("map-review-prepare"), "Prepare prescription for review");
        Check(manager.Prescriptions.Count == 1, "Prepare stored exactly one prescription", manager.Prescriptions.Count.ToString());
        PrescriptionSaveData record = manager.FindPrescription(1);
        Check(record != null && record.approvalState == PrescriptionApprovalState.Accepted, "the stored prescription is Accepted, not approved");
        Check(!manager.WorkOrders.Any(o => o.IsOpen) && marks.GetMarkedIds().Count == 0, "Prepare created no work order and no Fell mark");

        // The teaching lesson reads the stored prescription (read-only) and changes its next step.
        yield return Settle(10);
        var f8 = LiveFacts();
        Check(ScenarioOneTeachingSequence.ExtensionOffered(f8) && !f8.ExtensionApproved, "an Accepted prescription is not extension completion");
        Check(Learn.CurrentInstruction.Contains("Prescription 1 is accepted but not yet approved") && Learn.CurrentInstruction.Contains("approve them in the Work Plan [Tab] to commit the work"),
            "the lesson now asks for review, spot-check and approval, and names the Work Plan as the commitment step", Learn.CurrentInstruction);
        Check(Learn.HudModel().Progress.Contains("Prescription accepted") , "the HUD progress line follows the stored prescription", Learn.HudModel().Progress);
        Approval.ProposalSet preview = Review.Session.LastPreview;
        Check(preview != null && preview.Valid && preview.Proposals.Count > 0, "the rule proposes removals for the chosen areas", preview != null ? preview.Proposals.Count.ToString() : "null");
        Check(Named("map-review-approve") != null && !Named("map-review-approve").enabledSelf, "Approve is off before the 3D spot-check");
        yield return Shot("03-prescription-prepared", size =>
        {
            Check(HasText("PROPOSAL — not a Fell mark and not yet work"), "proposal wording at " + size.x + "x" + size.y);
        }, () => ui.Map.Refresh(true));

        // 3D spot-check: real [E] on a proposed tree in an accepted unit.
        yield return CloseMapToForest();
        string proposedId = Review.Session.LastPreview.Proposals[0].TreeId;
        ForestTree proposed = Tree(proposedId);
        yield return InspectByKey(proposed);
        yield return Settle(8);
        Check(Review.ContextFor(proposed) != null && Review.ContextFor(proposed).IsProposal, "3D inspection recognises a proposal target");
        Check(Review.Session.SpotCheckDone, "a real 3D inspection of an extrapolated tree records the spot-check");
        Check(!proposed.IsMarkedForFell && marks.GetMarkedIds().Count == 0 && !manager.WorkOrders.Any(o => o.IsOpen), "inspecting a proposal creates no mark or work");
        yield return input.Send(RackAuthoringNativeInput.Op.Key("Escape"));
        yield return Settle(6);

        // ---- 9. approve through the UI: ordinary Pending work; completion is derived ----
        yield return OpenReview();
        Button approve = Named("map-review-approve");
        Check(approve != null && approve.enabledSelf, "Approve is available only after the spot-check");
        Approval.ProposalSet expected = manager.PreviewPrescription(1);
        int expectedCount = expected.Links.Count;
        long cash = manager.CashCents; int year = ecology.EcologicalYear;
        string stageBeforeApproval = TeachingKey(LiveFacts());
        yield return Click(approve, "Approve prescription");
        var generated = manager.WorkOrders.Where(o => o.prescriptionId == 1).ToList();
        Check(generated.Count == expectedCount && expectedCount > 0, "approval created exactly the previewed orders", generated.Count + "/" + expectedCount);
        Check(generated.All(o => o.type == ScenarioWorkType.FellTree && o.status == ScenarioWorkStatus.Pending), "every generated order is an ordinary PENDING felling order");
        Check(generated.All(o => !string.IsNullOrEmpty(o.prescriptionCoverageUnitId) && !string.IsNullOrEmpty(o.prescriptionCropTreeId)), "each order carries prescription, coverage-unit and Crop Tree provenance");
        Check(!manager.WorkOrders.Any(o => o.status == ScenarioWorkStatus.Approved), "no order is approved: the Work Plan is still the commitment gate");
        Check(manager.CashCents == cash && ecology.EcologicalYear == year && generated.All(o => Tree(o.targetTreeId).IsLiving) && marks.GetMarkedIds().Count == 0,
            "no cash spent, no year advanced, no tree felled, no Fell mark");
        yield return Settle(12);
        var f9 = LiveFacts();
        Check(f9.ExtensionApproved && !ScenarioOneTeachingSequence.ExtensionOffered(f9), "approval completes the extension, derived from the stored prescription");
        Check(ScenarioOneTeachingSequence.Current(f9) == 5 && Learn.CurrentTopicIndex == 5, "the accepted stage index is still 5: the latch/stage were not touched", Learn.CurrentTopicIndex.ToString());
        Check(Learn.CurrentTitle == "7. Secure the next generation", "the lesson returns to Secure the next generation, now numbered 7", Learn.CurrentTitle);
        Check(manager.FirstGuidedRackReviewComplete, "the first-guided-review latch is untouched by the extension");
        Check(HasText(ScenarioOneTreatmentReview.ApprovedMessage) || Review.Session.ActiveApproved, "the MFM-08 review reports Pending tasks, not approved for execution");
        yield return Shot("04-review-approved-pending", size =>
        {
            Check(HasText("These are now ordinary Pending tasks in the Work Plan."), "the review says the proposals are now ordinary Pending tasks at " + size.x + "x" + size.y);
        }, () => ui.Map.Refresh(true));

        // ---- 10. the Work Plan: ordinary groups, provenance as explanation, still not committed ----
        ui.CloseAll(); ui.ShowWorkPlan(); yield return Settle(12);
        VisualElement card = Root.Q(name: "workplan-order-groups");
        Check(card != null, "the Work Plan group card is present");
        Check(card.Query<Foldout>().ToList().Count == 3, "exactly three status groups (no fourth status)");
        Foldout fApproved = Fold("workplan-approved"), fNew = Fold("workplan-pending-new"), fCarried = Fold("workplan-pending-carried-over");
        Check(fApproved != null && fNew != null && fCarried != null, "Approved / Pending new / Pending carried over foldouts exist");
        Check(fApproved.text == "Approved · 0" && fNew.text == "Pending new · " + generated.Count && fCarried.text == "Pending carried over · 0",
            "prescription work sits in Pending new, nothing Approved", fApproved.text + " | " + fNew.text + " | " + fCarried.text);
        fApproved.value = fNew.value = fCarried.value = true; yield return Settle(8);
        string newText = FoldText(fNew);
        Check(generated.All(o => newText.Contains("#" + o.workOrderId + " ·")), "every prescription order is listed in Pending new");
        Check(newText.Contains("Recorded origin · prescription 1 ·") && newText.Contains("Added by a prescription. Still ordinary Pending work: approve it in this Work Plan to commit it."),
            "origin provenance and the uncommitted explanation are shown");
        Check(Root.Q<Label>("workplan-prescription-note") != null && Root.Q<Label>("workplan-prescription-note").text == WorkPlanView.PrescriptionGroupNote, "the group note states the Work Plan is the commitment gate");
        Check(!Shown(Root.Q<VisualElement>("map-review-layer")) && !Shown(Root.Q<VisualElement>("map-review-scroll")), "the MFM-08 review layer and card are not shown on the Work Plan screen");
        Check(Shown(card), "the Work Plan group card is shown");
        Check(manager.WorkOrders.Where(o => o.prescriptionId == 1).All(o => o.status == ScenarioWorkStatus.Pending), "opening the Work Plan approves nothing");
        yield return Shot("05-workplan-pending-new", size =>
        {
            InsideWindow(Root.Q<Label>("workplan-prescription-note"), "prescription group note", size);
            Check(Root.Q(name: "workplan-order-groups").worldBound.xMax <= Root.worldBound.xMax + 1f, "Work Plan card fits the window width at " + size.x + "x" + size.y);
        }, () => { ui.ShowWorkPlan(); });
        ui.CloseAll();

        // ---- 11. save/load: provenance, completion and latch survive; no new schema ----
        string hashBeforeSave = ScenarioReferenceArchive.WorldHash(saves.CaptureData());
        saves.Save();
        string disk = File.ReadAllText(SavePath);
        Check(disk.Contains("\"prescriptionId\": 1") && disk.Contains("\"firstGuidedRackReviewComplete\": true"), "the slot carries prescription provenance and the latch");
        Check(!disk.ToLowerInvariant().Contains("extend-treatment") && !disk.ToLowerInvariant().Contains("extensionapproved") && !disk.ToLowerInvariant().Contains("extensionapplies"), "still no MFM-09 field or lesson id in the save");
        saves.Load(); yield return Settle(20);
        Check(ScenarioReferenceArchive.WorldHash(saves.CaptureData()) == hashBeforeSave, "disk reload preserves the world hash");
        var reloaded = manager.WorkOrders.Where(o => o.prescriptionId == 1).ToList();
        Check(reloaded.Count == generated.Count && reloaded.All(o => o.status == ScenarioWorkStatus.Pending && !string.IsNullOrEmpty(o.prescriptionCoverageUnitId)), "reloaded orders keep Pending status and provenance");
        var fReload = LiveFacts();
        Check(manager.FirstGuidedRackReviewComplete && fReload.ExtensionApproved && !ScenarioOneTeachingSequence.ExtensionOffered(fReload) && Learn.CurrentTitle == "7. Secure the next generation",
            "reload keeps the latch, the extension completion and the lesson shown");
        File.Delete(SavePath); if (File.Exists(SavePath + ".bak")) File.Delete(SavePath + ".bak");

        // ---- 12. a year boundary: Pending new becomes Pending carried over, still uncommitted ----
        string sampleExecutedBefore = Bay().ExecutedRemovals + "|" + string.Join(",", Bay().Future.Select(t => t.Id).OrderBy(x => x, StringComparer.Ordinal));
        Check(manager.AdvanceYear(), "an idle year passes with prescription work unapproved", manager.Feedback);
        yield return Settle(12);
        manager.MarkAnnualReviewSeen();
        Check(manager.WorkOrders.Where(o => o.prescriptionId == 1).All(o => o.status == ScenarioWorkStatus.Pending && o.IsOpen), "prescription orders stay Pending across the year");
        ui.CloseAll(); ui.ShowWorkPlan(); yield return Settle(12);
        fApproved = Fold("workplan-approved"); fNew = Fold("workplan-pending-new"); fCarried = Fold("workplan-pending-carried-over");
        Check(fApproved.text == "Approved · 0" && fNew.text == "Pending new · 0" && fCarried.text == "Pending carried over · " + generated.Count,
            "after the year boundary the prescription orders are Pending carried over", fApproved.text + " | " + fNew.text + " | " + fCarried.text);
        fCarried.value = true; yield return Settle(8);
        Check(generated.All(o => FoldText(fCarried).Contains("#" + o.workOrderId + " ·")) && FoldText(fCarried).Contains("Recorded origin · prescription 1 ·"), "carried-over rows still show prescription origin");
        Check(Root.Q<Label>("workplan-prescription-note") != null, "the explanation note is still shown for carried-over prescription work");
        var fYear = LiveFacts();
        Check(fYear.ExtensionApproved && Learn.CurrentTitle == "7. Secure the next generation", "the extension stays complete across the year");
        yield return Shot("06-workplan-carried-over", size =>
        {
            InsideWindow(Root.Q<Label>("workplan-prescription-note"), "prescription group note", size);
        }, () => { ui.ShowWorkPlan(); });

        // ---- 13. Work Plan approval is the only commitment step; economy path is authoritative ----
        long cashBeforeApproval = manager.CashCents;
        Check(manager.ApprovePendingWork(), "ordinary Work Plan approval commits the prescription work", manager.Feedback);
        Check(manager.WorkOrders.Where(o => o.prescriptionId == 1).All(o => o.status == ScenarioWorkStatus.Approved), "Work Plan approval approved them through the ordinary path");
        Check(manager.CashCents == cashBeforeApproval && generated.All(o => Tree(o.targetTreeId).IsLiving), "approved is not executed: cash and trees unchanged until the year is executed");
        ui.CloseAll(); ui.ShowWorkPlan(); yield return Settle(12);
        Check(Fold("workplan-approved").text == "Approved · " + generated.Count && Fold("workplan-pending-carried-over").text == "Pending carried over · 0", "after approval the same orders are in the Approved group");
        Fold("workplan-approved").value = true; yield return Settle(8);
        Check(FoldText(Fold("workplan-approved")).Contains("Added by a prescription and since approved in this Work Plan."), "approved prescription work is explained as approved by the Work Plan");
        yield return Shot("07-workplan-approved", null, () => { ui.ShowWorkPlan(); });
        ui.CloseAll();
        Check(manager.AdvanceYear(), "the ordinary year execution runs the approved prescription work", manager.Feedback);
        yield return Settle(12);
        manager.MarkAnnualReviewSeen();
        Check(manager.WorkOrders.Where(o => o.prescriptionId == 1).All(o => o.status == ScenarioWorkStatus.Completed), "executed through the ordinary contractor/economy path", manager.Feedback);
        Check(manager.CashCents != cashBeforeApproval, "cash changed through the ordinary economy path", cashBeforeApproval + " -> " + manager.CashCents);
        Check(generated.All(o => Tree(o.targetTreeId) == null || !Tree(o.targetTreeId).IsLiving), "the prescribed trees were felled by execution only");

        // ---- 14. the sample was not back-credited by comparable-area work; the lesson stage did not move ----
        Check(Bay().ExecutedRemovals + "|" + string.Join(",", Bay().Future.Select(t => t.Id).OrderBy(x => x, StringComparer.Ordinal)) == sampleExecutedBefore,
            "executing comparable-area work left the sample's executed removals and future trees unchanged");
        var fAfter = LiveFacts();
        Check(manager.FirstGuidedRackReviewComplete && ScenarioOneTeachingSequence.Current(fAfter) == 5 && Learn.CurrentTitle == "7. Secure the next generation", "the lesson stays at Secure the next generation and the latch holds");

        // ---- 15. manual / non-prescription management remains possible ----
        ForestTree manual = LiveTrees().Where(t => t.IsLiving && !t.IsCropTree && !manager.IsPlannedForFelling(t.TreeId)).OrderBy(t => t.TreeId, StringComparer.Ordinal).First();
        marks.Mark(manual, TreeMarkType.Fell, false);
        int addedManual = manager.AddMarkedTreesToWorkPlan();
        var manualOrder = manager.WorkOrders.FirstOrDefault(o => o.IsOpen && o.targetTreeId == manual.TreeId);
        Check(addedManual >= 1 && manualOrder != null && manualOrder.prescriptionId <= 0 && manualOrder.status == ScenarioWorkStatus.Pending, "a manual Fell mark still becomes ordinary Pending work with no prescription provenance");
        ui.CloseAll(); ui.ShowWorkPlan(); yield return Settle(12);
        Check(Fold("workplan-pending-new").text == "Pending new · 1", "manual work is Pending new", Fold("workplan-pending-new").text);
        Fold("workplan-pending-new").value = true; yield return Settle(8);
        Check(!FoldText(Fold("workplan-pending-new")).Contains("Added by a prescription") && Root.Q<Label>("workplan-prescription-note") == null,
            "manual work carries no prescription explanation, and the note is absent when no open order has prescription origin");
        yield return Shot("08-workplan-manual", null, () => { ui.ShowWorkPlan(); });
        ui.CloseAll();
        Check(manager.RemovePendingOrder(manualOrder.workOrderId), "the manual order is removed through the ordinary path");
        marks.Unmark(manual, false);
    }
}
#endif
