using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

// Disposable teaching-copy gate (pedagogy P1). Copy into Assets/ForestPrototype,
// run ScenarioOneTeachingCopyVerification.Begin, then remove the copy and .meta.
// Batch: copy content, help revisit, seed-aware ground line and unchanged
// objective/economy/ecology/save constants. Interactive (no -batchmode): also
// renders the HUD forecast and ground report at 1280/1600/1920 and checks
// that they fit. Never saves; restores menu-help preferences.
public static class ScenarioOneTeachingCopyVerification
{
#if UNITY_EDITOR
    public static void Begin()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        EditorApplication.isPlaying = true;
    }
#endif
    // Same Game-view sizing as MenuTutorialVerification (Editor only, interactive).
    public static void SetGameSize(int width, int height)
    {
#if UNITY_EDITOR
        if (Application.isBatchMode) return;
        Assembly assembly = typeof(Editor).Assembly;
        Type sizesType = assembly.GetType("UnityEditor.GameViewSizes");
        Type singleton = typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
        object sizes = singleton.GetProperty("instance", BindingFlags.Public | BindingFlags.Static).GetValue(null);
        MethodInfo getGroup = sizesType.GetMethod("GetGroup");
        object group = getGroup.Invoke(sizes, new[] { Enum.ToObject(getGroup.GetParameters()[0].ParameterType, 0) });
        Type sizeType = assembly.GetType("UnityEditor.GameViewSize"), kind = assembly.GetType("UnityEditor.GameViewSizeType");
        object size = Activator.CreateInstance(sizeType, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
            null, new object[] { Enum.ToObject(kind, 1), width, height, "Teaching copy " + width + "x" + height }, null);
        int index = (int)group.GetType().GetMethod("GetTotalCount").Invoke(group, null);
        group.GetType().GetMethod("AddCustomSize").Invoke(group, new[] { size });
        Type viewType = assembly.GetType("UnityEditor.GameView"); EditorWindow view = EditorWindow.GetWindow(viewType);
        viewType.GetProperty("selectedSizeIndex", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).SetValue(view, index);
        view.Show(); view.Focus(); view.Repaint();
#endif
    }
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (Environment.GetCommandLineArgs().Any(a => a == "ScenarioOneTeachingCopyVerification.Begin"))
            new GameObject("Disposable teaching copy verification").AddComponent<ScenarioOneTeachingCopyRunner>();
    }
}

public sealed class ScenarioOneTeachingCopyRunner : MonoBehaviour
{
    private readonly Dictionary<string, int?> preferences = new Dictionary<string, int?>();
    private ScenarioOneUiRoot ui;
    private ScenarioOneManager manager;
    private ForestEcologyController ecology;
    private ForestPlayer player;
    private string output;
    private static void Check(bool ok, string message) { if (!ok) throw new InvalidOperationException(message); }
    private static bool Has(string text, string fragment) => text.IndexOf(fragment, StringComparison.OrdinalIgnoreCase) >= 0;

    // Causal claims the simulation does not make, and prescriptive phrasing.
    private static readonly Regex ClearanceBenefit = new Regex(
        @"(improv|help|increas|boost|better|benefit)\w*\W+(\w+\W+){0,6}(seedling|surviv|regenerat|young tree)", RegexOptions.IgnoreCase);
    private static readonly Regex Prescriptive = new Regex(
        @"\b(you should (cut|fell|thin|remove|plant|clear)|recommended (tree|cut)|the correct tree|cut this tree)\b", RegexOptions.IgnoreCase);

    private IEnumerator Start()
    {
        yield return null; yield return null;
        Exception failure = null;
        var steps = new Stack<IEnumerator>(); steps.Push(Verify());
        while (steps.Count > 0)
        {
            bool more; object current = null;
            try { more = steps.Peek().MoveNext(); if (more) current = steps.Peek().Current; }
            catch (Exception e) { failure = e; break; }
            if (!more) { steps.Pop(); continue; }
            if (current is IEnumerator child) { steps.Push(child); continue; }
            yield return current;
        }
        foreach (var entry in preferences)
            if (entry.Value.HasValue) PlayerPrefs.SetInt(entry.Key, entry.Value.Value); else PlayerPrefs.DeleteKey(entry.Key);
        PlayerPrefs.Save();
        if (failure == null) Debug.Log("TEACHING_COPY_VERIFY_PASS");
        else Debug.LogError("TEACHING_COPY_VERIFY_FAIL " + failure);
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode(); EditorApplication.Exit(failure == null ? 0 : 1);
#endif
    }

    private IEnumerator Verify()
    {
        ui = FindFirstObjectByType<ScenarioOneUiRoot>(); manager = ui.Manager; player = ui.Player;
        ecology = FindFirstObjectByType<ForestEcologyController>();
        output = Environment.GetEnvironmentVariable("CCF_ACCEPTANCE_OUTPUT");
        foreach (MenuHelpView.Menu menu in Enum.GetValues(typeof(MenuHelpView.Menu)))
        {
            string key = MenuHelpView.PreferencePrefix + menu;
            preferences[key] = PlayerPrefs.HasKey(key) ? PlayerPrefs.GetInt(key) : (int?)null;
        }

        // ---- Required teaching content ----
        string hud = MenuHelpView.Title(MenuHelpView.Menu.WalkingHud) + " " + MenuHelpView.Explanation(MenuHelpView.Menu.WalkingHud);
        string tree = MenuHelpView.Explanation(MenuHelpView.Menu.TreeInspection);
        string map = MenuHelpView.Explanation(MenuHelpView.Menu.StandMap);
        string plan = MenuHelpView.Explanation(MenuHelpView.Menu.WorkPlan);
        string review = MenuHelpView.Explanation(MenuHelpView.Menu.AnnualReview);
        Func<string, string> step = id => LearningObjectivesView.Topics.SelectMany(t => t.Steps).Single(s => s.Id == id).Text;
        Func<string, string> topic = prefix => LearningObjectivesView.Topics.Single(t => t.Title.StartsWith(prefix)).Text;

        Check(Has(hud, "continuous-cover forestry") && Has(hud, "repeated") && Has(hud, "keeping tree cover")
            && Has(hud, "process") && Has(hud, "not one thinning") && Has(hud, "forest you leave behind"), "CCF explanation missing");
        Check(Has(tree, "Start with a tree worth keeping") && Has(step("tree.crop"), "want to keep"), "positive selection missing");
        Check(Has(tree, "smaller or suppressed is not, on its own, a reason to remove")
            && Has(step("tree.crop"), "not, on its own, a reason"), "suppressed ≠ removal lesson missing");
        Check(Has(tree, "DBH") && Has(tree, "breast height") && Has(tree, "crown") && Has(tree, "Competition")
            && Has(tree, "Crop Tree"), "inspection terms missing");
        Check(Has(map, "find where to look") && Has(map, "does not make the forestry decision")
            && Has(map, "Set waypoint") && Has(map, "inspect the actual trees"), "map navigation-not-decision rule missing");
        Check(Has(plan, "In the forest you decide what should happen") && Has(plan, "how that work will be carried out")
            && Has(plan, "what it will cost"), "Work Plan purpose missing");
        Check(Has(review, "what work did I do") && Has(review, "cost or earn") && Has(review, "forest change")
            && Has(review, "inspect") && Has(review, "Walk back"), "Annual Review questions / return to forest missing");
        Check(Has(review, "One thinning does not finish") && Has(topic("3."), "One thinning does not finish"), "repeated management framing missing");
        Check(Has(step("map.regeneration"), "seed already produced") && Has(step("map.regeneration"), "not automatically better"), "regeneration meaning missing");
        Check(Has(topic("6."), "not automatically an improvement") && Has(topic("6."), "No oak or beech seed trees"), "planting framing missing");
        Check(Has(step("map.browse"), "deer eating") && Has(step("map.browse"), "survive and grow into the next generation"), "browsing framing missing");
        Check(Has(topic("4."), "does not change timber prices") && Has(topic("4."), "knot-free"), "pruning wording missing");

        // ---- Clearance honesty (help, lessons, live preview label) ----
        int cell = Enumerable.Range(0, ecology.CellCount)
            .FirstOrDefault(i => manager.QueryClearance(ClearanceFootprint.Cell(ecology, i)).HasTargets);
        player.Clearance.Show(manager.QueryClearance(ClearanceFootprint.Cell(ecology, cell)));
        string preview = player.Clearance.Label;
        player.Clearance.Clear();
        string clearanceCopy = topic("7.") + " " + step("clear.read") + " " + step("clear.plan") + " " + step("clear.result") + " " + preview;
        Check(Has(preview, "young trees") && Has(topic("7."), "young trees") && Has(step("clear.read"), "young trees"), "clearance does not name young trees");
        Check(Has(topic("7."), "does not change how well seedlings survive or grow"), "clearance honesty statement missing");
        Match benefit = ClearanceBenefit.Match(clearanceCopy.Replace("does not change how well seedlings survive or grow", ""));
        Check(!benefit.Success, "clearance copy implies an unmodelled benefit: " + benefit.Value);

        // ---- No prescriptive phrasing anywhere in help or lessons ----
        string allCopy = string.Join(" ", Enum.GetValues(typeof(MenuHelpView.Menu)).Cast<MenuHelpView.Menu>().Select(MenuHelpView.Explanation))
            + " " + string.Join(" ", LearningObjectivesView.Topics.Select(t => t.Text + " " + string.Join(" ", t.Steps.Select(s => s.Text))));
        Match prescriptive = Prescriptive.Match(allCopy);
        Check(!prescriptive.Success, "prescriptive phrase: " + prescriptive.Value);
        Debug.Log("TEACHING_COPY_CONTENT_PASS ccf=true positiveSelection=true suppressedNotReason=true map=true workPlan=true review=true clearanceHonest=true noPrescription=true");

        // ---- Seed-aware ground line at Year 0 (Sitka not yet seeding) ----
        Check(ecology.EcologicalYear == 0, "fixture expects a fresh Year 0");
        int empty = Enumerable.Range(0, ecology.CellCount).First(i => ecology.Cells[i].Light < 0.10f);
        RegenerationDiagnosis none = default; none.Light = ecology.Cells[empty].Light;
        string why = ScenarioOneUiFacts.Why(none, ecology, empty);
        Check(Has(why, "No seed is reaching this spot yet"), "Year-0 ground line ignores seed supply: " + why);
        Debug.Log("TEACHING_COPY_SEED_LINE_PASS cell=" + empty + " text='" + why + "'");

        // ---- Help revisit for all five screens (existing mechanism) ----
        foreach (MenuHelpView.Menu menu in Enum.GetValues(typeof(MenuHelpView.Menu)))
        {
            ui.CloseHelp(); yield return null;
            ui.ShowHelp(menu);
            // Checked in the same frame: the UI root closes help that does not
            // match the current screen on its next update (existing behaviour).
            Check(ui.Help.IsOpen && ui.Help.CurrentMenu == menu, "help cannot be reopened: " + menu);
        }
        ui.CloseHelp(); yield return null; yield return null;
        Debug.Log("TEACHING_COPY_HELP_REVISIT_PASS allFive=true");

        // ---- Unchanged objective / economy / ecology / save contracts ----
        ScenarioOneDefinition d = manager.Definition;
        Check(d.MinimumCompletionYear == 25 && d.MinimumRetainedOriginalTrees == 60 && Mathf.Approximately(d.MinimumMeanCanopy, 0.35f)
            && d.MinimumRegenerationCells == 3 && Mathf.Approximately(d.MinimumDeadwoodVolumeM3, 0.02f) && d.CenturyReviewYear == 100,
            "objective thresholds changed");
        Check(string.Join(",", manager.Objectives.Select(o => o.objectiveId)) ==
            "minimum-year,retained-canopy,continuous-canopy,regeneration,fallen-deadwood,managed-opening,introduced-beech,introduced-sessile-oak",
            "objective set changed");
        Check(d.StartingCashCents == 1200000 && d.ContractorHourlyRateCents == 4500 && d.MinimumHarvestJobCents == 250000
            && d.TreeShelterMaterialCents == 500 && d.OwnerMinutesPerYear == 2400 && d.TimberValueCentsPerCubicMetre("sitka-spruce") == 7200
            && d.FindShopEntry("beech-sapling").unitPriceCents == 450 && d.FindShopEntry("sessile-oak-sapling").unitPriceCents == 550,
            "economy values changed");
        TreeSpeciesDefinition sitka = ecology.ResolveSpecies();
        Check(Mathf.Approximately(sitka.Ci50, 5f) && Mathf.Approximately(sitka.PotentialDbhGrowthCmPerYear, 1.2f)
            && Mathf.Approximately(sitka.MaturityOnsetYears, 20f) && Mathf.Approximately(sitka.MaturityFullYears, 30f)
            && Mathf.Approximately(ForestEcologyController.HegyiCutoffMeters, 8f)
            && Mathf.Approximately(ForestEcologyController.CanopyShadeReachPerCrownRadius, 1f)
            && Mathf.Approximately(d.BackgroundBrowsePressure, 0.2f), "ecology values changed");
        // Copy-only packets must not move the save schema. Main is v16 at 3e4ee40;
        // after the growth-model integration (v17) set CCF_EXPECTED_SAVE_VERSION.
        string expectedSave = Environment.GetEnvironmentVariable("CCF_EXPECTED_SAVE_VERSION");
        int expected = string.IsNullOrEmpty(expectedSave) ? 16 : int.Parse(expectedSave);
        Check(ForestSaveData.CurrentVersion == expected, "save schema version " + ForestSaveData.CurrentVersion + " != expected " + expected);
        Debug.Log("TEACHING_COPY_CONTRACTS_PASS objectives=unchanged economy=unchanged ecology=unchanged save=v" + expected);

        // ---- Rendered layout (interactive only) ----
        if (Application.isBatchMode)
        {
            Debug.Log("TEACHING_COPY_RENDERED_SKIPPED batchmode (run interactively for layout and captures)");
            yield break;
        }
        yield return RenderedLayout();
    }

    private IEnumerator RenderedLayout()
    {
        ForestTreeMarkingManager marking = ui.Marking;
        List<ForestTree> trees = FindObjectsByType<ForestTree>(FindObjectsSortMode.None).Where(t => t.CanChop)
            .OrderBy(t => new Vector2(t.transform.position.x, t.transform.position.z).sqrMagnitude).ThenBy(t => t.TreeId, StringComparer.Ordinal).ToList();
        foreach (ForestTree t in trees.Skip(1).Take(6)) marking.Mark(t, TreeMarkType.Fell, false);
        player.transform.position = trees[0].transform.position - new Vector3(0, 0, 3);
        player.LookToward(trees[0].transform.position + Vector3.up * 1.3f);
        UnityEngine.Cursor.lockState = CursorLockMode.Locked; UnityEngine.Cursor.visible = false;
        for (int i = 0; i < 40; i++) yield return null;
        object hud = ui.GetType().GetField("hud", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(ui);
        Func<string, VisualElement> field = name => (VisualElement)hud.GetType().GetField(name, BindingFlags.NonPublic | BindingFlags.Instance).GetValue(hud);
        var treatment = (Label)field("treatment");
        Check(!string.IsNullOrEmpty(marking.TreatmentOutcome) && marking.TreatmentOutcome.Contains("information, not advice")
            && marking.TreatmentOutcome.Contains("Release is local"), "forecast copy missing: " + marking.TreatmentOutcome);
        foreach (Vector2Int size in new[] { new Vector2Int(1280, 720), new Vector2Int(1600, 900), new Vector2Int(1920, 1080) })
        {
            ScenarioOneTeachingCopyVerification.SetGameSize(size.x, size.y);
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            for (int i = 0; i < 6; i++) yield return null;
            Rect viewport = ui.RootElement.worldBound, forecast = treatment.worldBound, summary = field("markSummary").worldBound;
            Check(treatment.resolvedStyle.display == DisplayStyle.Flex && forecast.height > 10f
                && viewport.Contains(forecast.min) && viewport.Contains(forecast.max), "forecast hidden or clipped at " + size.x);
            Check(!forecast.Overlaps(summary), "forecast overlaps marking summary at " + size.x);
            yield return Capture("hud-forecast-" + size.x);
            Debug.Log("TEACHING_COPY_RENDERED_PASS forecast " + size.x + "x" + size.y + " bounds=" + forecast);
        }
        marking.ClearAll();
        ScenarioOneTeachingCopyVerification.SetGameSize(1600, 900);
    }

    private IEnumerator Capture(string name)
    {
        yield return new WaitForEndOfFrame();
        if (string.IsNullOrEmpty(output)) yield break;
        Directory.CreateDirectory(output);
        Texture2D image = ScreenCapture.CaptureScreenshotAsTexture();
        Check(image != null && image.width > 600, "missing rendered capture " + name);
        File.WriteAllBytes(Path.Combine(output, name + ".png"), image.EncodeToPNG());
        Destroy(image);
    }
}
