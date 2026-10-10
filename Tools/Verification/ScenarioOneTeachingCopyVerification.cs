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

// Disposable S1-A gate. No file saves; local learning/help preferences restored.
// Batch checks content, authoritative projections and read-only presentation.
// Interactive additionally captures real UI at 1280x720 and 1920x1080.
public static class ScenarioOneTeachingCopyVerification
{
    public static void Begin()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        SetGameSize(1600, 900);
        EditorApplication.isPlaying = true;
    }
    public static void SetGameSize(int width, int height)
    {
        if (Application.isBatchMode) return;
        Assembly a = typeof(Editor).Assembly;
        Type t = a.GetType("UnityEditor.GameViewSizes");
        object sizes = typeof(ScriptableSingleton<>).MakeGenericType(t).GetProperty("instance", BindingFlags.Public | BindingFlags.Static).GetValue(null);
        MethodInfo get = t.GetMethod("GetGroup");
        object group = get.Invoke(sizes, new[] { Enum.ToObject(get.GetParameters()[0].ParameterType, 0) });
        Type sizeType = a.GetType("UnityEditor.GameViewSize"), kind = a.GetType("UnityEditor.GameViewSizeType");
        object size = Activator.CreateInstance(sizeType, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
            null, new object[] { Enum.ToObject(kind, 1), width, height, "S1-A " + width + "x" + height }, null);
        int index = (int)group.GetType().GetMethod("GetTotalCount").Invoke(group, null);
        group.GetType().GetMethod("AddCustomSize").Invoke(group, new[] { size });
        Type viewType = a.GetType("UnityEditor.GameView"); EditorWindow view = EditorWindow.GetWindow(viewType);
        viewType.GetProperty("selectedSizeIndex", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).SetValue(view, index);
        var gizmos = viewType.GetProperty("showGizmos", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (gizmos != null && gizmos.CanWrite) gizmos.SetValue(view, false);
        view.Show(); view.Focus(); view.Repaint();
    }
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Install()
    {
        if (Environment.GetCommandLineArgs().Contains("ScenarioOneTeachingCopyVerification.Begin"))
            new GameObject("Disposable S1-A verification").AddComponent<ScenarioOneTeachingCopyRunner>();
    }
}
public sealed class ScenarioOneTeachingCopyRunner : MonoBehaviour
{
    ScenarioOneUiRoot ui; ScenarioOneManager m; ForestEcologyController eco; ForestSaveController saves;
    string output; int checks;
    readonly Dictionary<string, int?> prefs = new Dictionary<string, int?>();
    void Check(bool ok, string message) { checks++; if (!ok) throw new Exception(message); }
    static string Text(VisualElement element) => string.Join("\n", element.Query<Label>().ToList().Select(l => l.text));
    static bool Has(string text, string part) => text.IndexOf(part, StringComparison.OrdinalIgnoreCase) >= 0;
    static object Field(object obj, string name) => obj.GetType().GetField(name, BindingFlags.NonPublic | BindingFlags.Instance).GetValue(obj);
    static void Set(object obj, string name, object value) => obj.GetType().GetField(name, BindingFlags.NonPublic | BindingFlags.Instance).SetValue(obj, value);
    IEnumerator Start()
    {
        for (int i = 0; i < 5; i++) yield return null;
        ui = FindFirstObjectByType<ScenarioOneUiRoot>(); m = ui.Manager; eco = ui.Ecology; saves = FindFirstObjectByType<ForestSaveController>();
        output = Environment.GetEnvironmentVariable("CCF_ACCEPTANCE_OUTPUT"); Directory.CreateDirectory(output);
        foreach (string key in Enum.GetValues(typeof(MenuHelpView.Menu)).Cast<MenuHelpView.Menu>().Select(menu => MenuHelpView.PreferencePrefix + menu)
            .Concat(LearningObjectivesView.StepIds.Select(id => LearningObjectivesView.PreferencePrefix + id)))
            prefs[key] = PlayerPrefs.HasKey(key) ? PlayerPrefs.GetInt(key) : (int?)null;
        Exception failure = null; var stack = new Stack<IEnumerator>(); stack.Push(Verify());
        while (stack.Count > 0)
        {
            bool more = false; object next = null;
            try { more = stack.Peek().MoveNext(); if (more) next = stack.Peek().Current; } catch (Exception e) { failure = e; break; }
            if (!more) { stack.Pop(); continue; }
            if (next is IEnumerator child) { stack.Push(child); continue; }
            yield return next;
        }
        foreach (var entry in prefs) if (entry.Value.HasValue) PlayerPrefs.SetInt(entry.Key, entry.Value.Value); else PlayerPrefs.DeleteKey(entry.Key);
        PlayerPrefs.Save();
        if (failure == null) Debug.Log("S1A_TEACHING_VERIFY_PASS checks=" + checks);
        else Debug.LogError("S1A_TEACHING_VERIFY_FAIL " + failure);
        EditorApplication.ExitPlaymode(); EditorApplication.Exit(failure == null ? 0 : 1);
    }
    IEnumerator Verify()
    {
        ui.CloseAll(); ui.CloseHelp();
        string original = ScenarioReferenceArchive.WorldHash(saves.CaptureData());
        Check(ForestSaveData.CurrentVersion == 20 && eco.RngModelVersion == 1 && eco.RegenerationModelVersion == 2
            && eco.GrowthModelVersion == 1 && eco.StormModelVersion == 0, "accepted model stack");
        string help = MenuHelpView.Explanation(MenuHelpView.Menu.WalkingHud), tree = MenuHelpView.Explanation(MenuHelpView.Menu.TreeInspection);
        Check(Has(help, "repeated process") && Has(help, "keeping tree cover") && Has(help, "not one thinning")
            && Has(help, "More felling is not automatically better") && Has(help, "leave behind"), "repeated CCF and residual stand");
        Check(Has(tree, "Start with a tree worth keeping") && Has(tree, "competitors") && Has(tree, "suppressed does not by itself"), "positive selection without suppression prescription");
        string lessons = string.Join(" ", LearningObjectivesView.Topics.Select(t => t.Text + " " + string.Join(" ", t.Steps.Select(s => s.Text))));
        Check(Has(lessons, "opportunity for natural regeneration") && Has(lessons, "needs seed") && Has(lessons, "not guaranteed")
            && Has(lessons, "not required for every useful management outcome"), "contingent regeneration");
        Check(Has(lessons, "timber quality") && Has(lessons, "no timber-price premium"), "pruning without price promise");
        Check(Has(MenuHelpView.Explanation(MenuHelpView.Menu.WorkPlan), "execution") && Has(MenuHelpView.Explanation(MenuHelpView.Menu.WorkPlan), "does not decide the forest structure"), "plan purpose");
        Check(Has(MenuHelpView.Explanation(MenuHelpView.Menu.AnnualReview), "reassess") && Has(MenuHelpView.Explanation(MenuHelpView.Menu.AnnualReview), "Walk back"), "return to inspect");
        Check(Has(LearningObjectivesView.ClearanceExplanation(2), "reduce the survival") && Has(LearningObjectivesView.ClearanceExplanation(2), "removes young trees")
            && Has(LearningObjectivesView.ClearanceExplanation(2), "can return"), "three-part Model2 clearance meaning");
        foreach (int legacy in new[] { 0, 1 }) Check(Has(LearningObjectivesView.ClearanceExplanation(legacy), "does not change young-tree survival"), "legacy clearance truth");
        Check(m.Objectives.Count == 8 && string.Join(",", m.Objectives.Select(o => o.objectiveId)) ==
            "minimum-year,retained-canopy,continuous-canopy,regeneration,fallen-deadwood,managed-opening,introduced-beech,introduced-sessile-oak", "objective identities");
        ui.ShowObjectives(); ui.Learning.Refresh(true); yield return null;
        var success = ui.Learning.Root.Q("scenario-success-objectives");
        Check(success != null && m.Objectives.All(o => Text(success).Contains(ScenarioOneUiFacts.ObjectiveLine(o))), "all authoritative objectives visible at year zero");
        Check(!Text(success).Contains("sessile-oak") && Has(Text(success), "sessile oak"), "human-readable objective species");
        Check(ScenarioOneUiFacts.SpeciesName("sitka-spruce") == "Sitka spruce" && ScenarioOneUiFacts.SpeciesName("sessile-oak") == "Sessile oak", "display species lookup");
        int empty = Enumerable.Range(0, eco.CellCount).First(i => !eco.Cells[i].SeedRainBySpecies.Values.Any(v => v > 0));
        RegenerationDiagnosis none = default; none.Light = eco.Cells[empty].Light;
        Check(Has(ScenarioOneUiFacts.Why(none, eco, empty), "No seed"), "lack of seed is distinguished even in shade");
        eco.Cells[empty].SetSeedRain("sitka-spruce", 1f);
        Check(!Has(ScenarioOneUiFacts.Why(none, eco, empty), "No seed"), "seed present must not claim absence");
        eco.RecomputeSeedRain();
        ui.CloseAll();
        Check(ScenarioReferenceArchive.WorldHash(saves.CaptureData()) == original, "views and seed diagnosis do not alter forest state");
        Debug.Log("S1A_READONLY_HASH " + original);
        long priorCash = m.CashCents;
        var offer = m.Definition.FindShopEntry("sessile-oak-sapling");
        try
        {
            Set(m, "cashCents", m.Definition.MinimumHarvestJobCents + 100L);
            string warning = WorkPlanView.NurseryPurchaseSummary(m, offer, 25);
            Check(warning.Contains(UiKit.Money(25L * offer.unitPriceCents)) && (Has(warning, "less than") || Has(warning, "below")) && Has(warning, "minimum")
                && Has(warning, "Stock remains"), "pre-purchase exact cost and existing minimum-risk warning");
            Check(m.TryPurchaseStock(offer.itemId, 25) && m.CashCents < m.Definition.MinimumHarvestJobCents,
                "economically unwise affordable purchase remains allowed");
            Check(m.PlantedJuveniles.Count == 0 && m.GetStockQuantity(offer.itemId) == 25, "purchase does not plant");
        }
        finally { Set(m, "cashCents", priorCash); }
        Debug.Log("S1A_NURSERY_PASS advisory=true actualCost=true noPlanting=true");
        if (Application.isBatchMode) yield break;
        yield return Render();
    }
    IEnumerator Settle()
    { for (int i = 0; i < 12; i++) yield return null; }
    void Fits(VisualElement element, string name)
    {
        Rect view = ui.RootElement.worldBound, b = element.worldBound;
        Check(element.resolvedStyle.display != DisplayStyle.None && b.width > 0 && b.height > 0 && view.Contains(b.min) && view.Contains(b.max), name + " outside viewport: " + b);
    }
    IEnumerator Capture(string name)
    {
        yield return new WaitForEndOfFrame();
        Texture2D image = ScreenCapture.CaptureScreenshotAsTexture();
        Check(image != null && image.width >= 1280, "rendered screenshot " + name);
        File.WriteAllBytes(Path.Combine(output, name + ".png"), image.EncodeToPNG()); Destroy(image);
        Debug.Log("S1A_RENDERED_PASS " + name);
    }
    IEnumerator Render()
    {
        var player = ui.Player;
        MethodInfo inspect = player.GetType().GetMethod("InspectTree", BindingFlags.NonPublic | BindingFlags.Instance);
        ForestTree crop = FindObjectsByType<ForestTree>(FindObjectsSortMode.None).Single(t => t.TreeId == "P0707");
        yield return new WaitForSecondsRealtime(5f); // Let first interactive shader compilation settle before captures.
        foreach (var size in new[] { new Vector2Int(1280, 720), new Vector2Int(1920, 1080) })
        {
            ScenarioOneTeachingCopyVerification.SetGameSize(size.x, size.y);
            ui.CloseAll(); ui.CloseHelp(); ui.ShowHelp(MenuHelpView.Menu.WalkingHud); yield return Settle();
            Fits(ui.Help.Root.Children().First(), "help"); yield return Capture("s1a-help-" + size.x);
            ui.CloseHelp(); ui.ShowObjectives(); ui.Learning.Refresh(true);
            foreach (Foldout f in ui.Learning.Root.Query<Foldout>().ToList()) f.value = f.name == "scenario-success-objectives";
            ui.Learning.Root.Q<ScrollView>().scrollOffset = Vector2.zero; yield return Settle();
            Fits(ui.Learning.Root.Children().First(), "learning");
            var objectives = ui.Learning.Root.Q("scenario-success-objectives"); Fits(objectives, "objectives");
            Check(Has(Text(objectives), "sessile oak"), "rendered human species"); yield return Capture("s1a-objectives-" + size.x);
            foreach (Foldout f in ui.Learning.Root.Query<Foldout>().ToList()) f.value = f.text.StartsWith("2. Tree");
            ui.Learning.Root.Q<ScrollView>().scrollOffset = Vector2.zero; yield return Settle(); yield return Capture("s1a-learning-" + size.x);
            ui.CloseAll(); ui.Marking.Mark(crop, TreeMarkType.CropTree, false);
            player.transform.position = crop.transform.position - new Vector3(0, 0, 2.2f); player.LookToward(crop.transform.position + Vector3.up * 1.3f);
            for (int i = 0; i < 20; i++) { UnityEngine.Cursor.lockState = CursorLockMode.Locked; inspect.Invoke(player, new object[] { crop }); yield return null; }
            ui.CloseHelp(); yield return Settle();
            Fits(ui.RootElement.Q(className: "inspect"), "inspection"); Fits(ui.RootElement.Q("competitor-panel"), "competitors");
            yield return Capture("s1a-inspection-" + size.x);
            Set(player, "inspectedTree", null); Set(player, "isInspecting", false); ui.ShowWorkPlan(); yield return Settle(); ui.CloseHelp(); yield return Settle();
            Check(!ui.Help.IsOpen, "plan capture must expose job cards");
            VisualElement plan = (VisualElement)((WorkPlanView)Field(ui, "workPlan")).Root;
            Fits(plan.Children().First(), "work plan");
            Check(plan.Query<Label>(className: "muted").ToList().All(l => l.resolvedStyle.fontSize >= 15), "supporting font increase applied");
            yield return Capture("s1a-workplan-" + size.x);
            var wp = (WorkPlanView)Field(ui, "workPlan"); long cash = m.CashCents;
            Set(m, "cashCents", m.Definition.MinimumHarvestJobCents + 100L); wp.Refresh(true);
            ScrollView scroll = plan.Q<ScrollView>(); yield return Settle();
            var warning = plan.Q("nursery-warning-sessile-oak-sapling"); scroll.ScrollTo(warning); yield return Settle();
            Check(warning.worldBound.Overlaps(scroll.contentViewport.worldBound), "nursery warning visible");
            yield return Capture("s1a-nursery-" + size.x); Set(m, "cashCents", cash); wp.Refresh(true);
            ui.CloseAll();
        }
        ui.ShowWorkPlan(); ui.CloseHelp(); Check(ui.AdvanceFromWorkPlan(), "first ecological year advances"); yield return Settle();
        ui.ShowReview(); ui.CloseHelp(); yield return Settle();
        foreach (var size in new[] { new Vector2Int(1280, 720), new Vector2Int(1920, 1080) })
        {
            ScenarioOneTeachingCopyVerification.SetGameSize(size.x, size.y); yield return Settle();
            VisualElement review = ui.RootElement.Query<VisualElement>(className: "modal").ToList().Single(v => v.parent.resolvedStyle.display != DisplayStyle.None && Text(v).Contains("Annual review"));
            Fits(review, "annual review"); yield return Capture("s1a-review-" + size.x);
        }
    }
}
#endif
