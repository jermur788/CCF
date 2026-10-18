#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UIElements;

// Disposable RENDERED + FUNCTIONAL qualification for the Scenario One rack-first revision (Slices B-F): the common rack backend, the
// in-world tool, the Stand Map tool, the Work Plan grouping, the rack-first teaching sequence and the authored layout.
//
// It plays one fresh Scenario One game from the first forest view to the harvest of the authored layout and the introduction of
// selective marking, through the real UI where the Editor can deliver input and through the same public calls where it cannot
// (Editor-synthetic mouse clicks do not reach the runtime panel). Every hard check is a numbered packet expectation; MEASURE
// lines record the layout facts the Manager asked for (rows, racks, spacing, trees, volume, cashflow).
//
// Run (graphical Editor):  -executeMethod RackUxRack.Begin   CCF_DIAG_DIR=<folder>  CCF_UI_SIZES=1920x1080,1280x720
public static class RackUxRack
{
    public static void Begin()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        RackUxShots.SetGameSize(RackUxShots.Sizes()[0].x, RackUxShots.Sizes()[0].y);
        EditorApplication.isPlaying = true;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Prepare()
    {
        if (!Environment.GetCommandLineArgs().Any(a => a == "RackUxRack.Begin")) return;
        foreach (MenuHelpView.Menu menu in Enum.GetValues(typeof(MenuHelpView.Menu))) PlayerPrefs.DeleteKey(MenuHelpView.PreferencePrefix + menu);
        foreach (string id in LearningObjectivesView.StepIds) PlayerPrefs.DeleteKey(LearningObjectivesView.PreferencePrefix + id);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (Environment.GetCommandLineArgs().Any(a => a == "RackUxRack.Begin"))
            new GameObject("Rack UX rack").AddComponent<RackUxRackRunner>();
    }
}

public sealed class RackUxRackRunner : MonoBehaviour
{
    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
    private ScenarioOneUiRoot ui;
    private ScenarioOneManager manager;
    private ForestEcologyController ecology;
    private ForestPlayer player;
    private ForestTreeMarkingManager marks;
    private ForestSaveController saves;
    private RackTool tool;
    private RackPlanner planner;
    private string output;
    // The trees of the five authored racks as the planner selected them at the start (the expectation for approval and harvest).
    private List<string> layoutIds = new List<string>();
    private int shots, passed;
    private readonly List<string> failures = new List<string>(), evidence = new List<string>(), unverified = new List<string>();

    private static string F(double v, string f = "0.##") => v.ToString(f, Inv);
    private void Hard(string name, bool ok, string detail = "")
    {
        if (ok) passed++; else failures.Add(name + " " + detail);
        Debug.Log($"RACKUX_{(ok ? "PASS" : "FAIL")} {name} {detail}");
    }
    private void Note(string name, string detail)
    {
        evidence.Add(name + " " + detail);
        Debug.Log($"RACKUX_MEASURE {name} {detail}");
    }
    private void Unverified(string detail) { unverified.Add(detail); Debug.Log("RACKUX_UNVERIFIED " + detail); }

    private IEnumerator Start()
    {
        for (int i = 0; i < 5; i++) yield return null;
        Exception failure = null;
        var stack = new Stack<IEnumerator>();
        stack.Push(Verify());
        while (stack.Count > 0)
        {
            bool more; object current = null;
            try { more = stack.Peek().MoveNext(); if (more) current = stack.Peek().Current; }
            catch (Exception error) { failure = error; break; }
            if (!more) { stack.Pop(); continue; }
            if (current is IEnumerator nested) stack.Push(nested); else yield return current;
        }
        if (output != null) File.WriteAllLines(Path.Combine(output, "rack-evidence.txt"), evidence);
        bool ok = failure == null && failures.Count == 0;
        Debug.Log(ok ? $"RACKUX_RACK_DONE shots={shots} hardChecks={passed} unverified={unverified.Count}"
            : $"RACKUX_RACK_FAILED shots={shots} hardChecks={passed} exception={failure} failures=[{string.Join(" | ", failures)}]");
        Debug.Log("RACKUX_RACK_DONE_MARKER");
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(ok ? 0 : 1);
    }

    // ---------- helpers ----------
    private IEnumerator Press(Key key)
    {
        Keyboard keyboard = Keyboard.current ?? InputSystem.AddDevice<Keyboard>();
        InputSystem.QueueStateEvent(keyboard, new KeyboardState(key)); yield return null; yield return null;
        InputSystem.QueueStateEvent(keyboard, new KeyboardState()); yield return null; yield return null;
    }

    private IEnumerator Wheel(float amount)
    {
        Mouse mouse = Mouse.current ?? InputSystem.AddDevice<Mouse>();
        InputSystem.QueueStateEvent(mouse, new MouseState { scroll = new Vector2(0f, amount) }); yield return null; yield return null;
        InputSystem.QueueStateEvent(mouse, new MouseState()); yield return null; yield return null;
    }

    private IEnumerator Invoke(Button button)
    {
        if (button == null) throw new InvalidOperationException("button not found");
        typeof(Clickable).GetMethod("Invoke", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(button.clickable, new object[] { null });
        yield return null; yield return null;
    }

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

    private VisualElement UiRoot() => ui.RootElement;
    private Button BtnStarts(params string[] texts) => UiRoot().Query<Button>().ToList()
        .FirstOrDefault(b => b.text != null && texts.Any(t => b.text.StartsWith(t, StringComparison.Ordinal)) && Visible(b));
    private static string Texts(VisualElement root)
        => root == null ? "" : string.Join("\n", root.Query<TextElement>().ToList().Where(Visible).Select(t => t.text).Where(s => !string.IsNullOrEmpty(s)));

    private ForestTree[] Living() => FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).Where(t => t.IsLiving).ToArray();

    private IEnumerator AimAtGround(float x, float z, float standoff = 3f)
    {
        Vector3 p = player.transform.position;
        player.transform.position = new Vector3(x, p.y, z - standoff);
        player.LookToward(new Vector3(x, 0f, z));
        for (int i = 0; i < 8; i++) yield return null;
    }

    private IEnumerator Shoot(string state, params Vector2Int[] sizes)
    {
        foreach (Vector2Int size in sizes.Length > 0 ? sizes : RackUxShots.Sizes())
        {
            RackUxShots.SetGameSize(size.x, size.y);
            for (int i = 0; i < 8; i++) yield return null;
            yield return new WaitForEndOfFrame();
            Texture2D image = ScreenCapture.CaptureScreenshotAsTexture();
            if (image == null || image.width < 600) throw new InvalidOperationException("no rendered capture for " + state);
            File.WriteAllBytes(Path.Combine(output, state + "-" + size.x + "x" + size.y + ".png"), image.EncodeToPNG());
            Destroy(image);
            shots++;
        }
    }

    private static string Ids(IEnumerable<string> ids) => string.Join(",", ids.OrderBy(i => i, StringComparer.Ordinal));
    private int OpenFellOrders() => manager.WorkOrders.Count(o => o.IsOpen && o.type == ScenarioWorkType.FellTree);

    // A top-down view of the whole stand, with the interface hidden, for the layout evidence.
    private IEnumerator TopDown(string state)
    {
        Camera cam = Camera.main;
        Vector3 position = cam.transform.position; Quaternion rotation = cam.transform.rotation; float fov = cam.fieldOfView;
        bool playerWas = player.enabled;
        player.enabled = false;
        UiRoot().style.display = DisplayStyle.None;
        cam.transform.position = new Vector3(0f, 118f, 0f);
        cam.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        cam.fieldOfView = 40f;
        for (int i = 0; i < 6; i++) yield return null;
        yield return Shoot(state, new Vector2Int(1920, 1080));
        UiRoot().style.display = DisplayStyle.Flex;
        cam.transform.position = position; cam.transform.rotation = rotation; cam.fieldOfView = fov;
        player.enabled = playerWas;
        for (int i = 0; i < 4; i++) yield return null;
    }

    // ---------- storyline ----------
    private IEnumerator Verify()
    {
        ui = FindFirstObjectByType<ScenarioOneUiRoot>(); manager = ui.Manager; ecology = ui.Ecology; player = ui.Player; marks = ui.Marking;
        saves = FindFirstObjectByType<ForestSaveController>(); tool = ui.RackTool; planner = tool.Planner;
        if (Application.isBatchMode) throw new InvalidOperationException("RackUxRack needs a graphical Editor");
        string diag = Environment.GetEnvironmentVariable("CCF_DIAG_DIR");
        output = Path.Combine(string.IsNullOrEmpty(diag) ? Application.dataPath + "/../Build/RackUx/evidence" : diag, "rack");
        Directory.CreateDirectory(output);
        Note("START", $"year={ecology.EcologicalYear} geometry={ecology.StandGeometryModelVersion} trees={Living().Length} cash={manager.CashCents} screen={Screen.width}x{Screen.height}");

        yield return Stage1();
        yield return ModalRule();
        yield return Layout();
        yield return Preview();
        yield return ForestTool();
        yield return WorkPlanGroup();
        yield return MapTool();
        yield return SaveLoad();
        yield return CompleteLayout();
        yield return ApproveAndHarvest();
        yield return Stage2();
    }

    // ---- the opening stage: racks first ----
    private IEnumerator Stage1()
    {
        if (ui.Help.IsOpen) { ui.CloseHelp(); yield return null; yield return null; }
        yield return Wait(0.5f);
        Hard("STAGE1_RACK_FIRST_IS_ACTIVE", manager.RackFirstStageActive, "");
        string cue = ui.Learning.Summary;
        Hard("STAGE1_FIRST_CUE_IS_THE_RACK", cue.Contains("Create your first rack"), cue);
        Hard("STAGE1_TOPIC_ONE_IS_RACKS", LearningObjectivesView.Topics[0].Title.EndsWith("Extraction racks") && LearningObjectivesView.Topics[1].Title.EndsWith("Tree inspection and Crop Trees"), LearningObjectivesView.Topics[0].Title);
        // The aimed-at-a-tree prompt does not advertise selective marking in this stage.
        ForestTree aimed = null;
        foreach (ForestTree t in Living().Where(t => !t.TreeId.StartsWith("Q", StringComparison.Ordinal)).Take(30))
        {
            player.transform.position = t.transform.position - new Vector3(0, 0, 3);
            for (int i = 0; i < 8; i++) yield return null;
            if (marks.AimedTree == t) { aimed = t; break; }
        }
        string prompt = player.InteractionPromptText();
        Hard("STAGE1_TREE_PROMPT_OFFERS_THE_RACK_NOT_MARKING", aimed != null && prompt.Contains("[B] Create rack") && !prompt.Contains("[X]") && !prompt.Contains("[C]"), prompt);
        yield return Shoot("19-hud-rack-first");
    }

    // ---- the modal rule: the help and first-use window owns the keyboard while it is open ----
    private IEnumerator ModalRule()
    {
        string prefix = MenuHelpView.PreferencePrefix;
        // The Work Plan and Objectives are not on first use here, so closing the window and pressing their keys shows only the screen.
        PlayerPrefs.SetInt(prefix + MenuHelpView.Menu.WorkPlan, 1);
        ui.ShowHelp(MenuHelpView.Menu.WalkingHud);
        yield return null; yield return null;
        Hard("MODAL_RULE_WINDOW_IS_OPEN_AND_FLAGGED_ON_THE_MANAGER", ui.Help.IsOpen && manager.ModalOpen, "");
        yield return Press(Key.Tab);
        Hard("MODAL_BLOCKS_TAB_WORK_PLAN", ui.Help.IsOpen && !manager.WorkPlanOpen, $"help={ui.Help.IsOpen} plan={manager.WorkPlanOpen}");
        yield return Press(Key.M);
        Hard("MODAL_BLOCKS_MAP_SHORTCUT", ui.Help.IsOpen && ui.CurrentScreen == ScenarioOneUiRoot.UiScreen.None, $"help={ui.Help.IsOpen} screen={ui.CurrentScreen}");
        yield return Press(Key.O);
        Hard("MODAL_BLOCKS_OBJECTIVES_SHORTCUT", ui.Help.IsOpen && ui.CurrentScreen == ScenarioOneUiRoot.UiScreen.None, $"help={ui.Help.IsOpen} screen={ui.CurrentScreen}");
        Hard("MODAL_KEEPS_ITS_OWN_SCREEN", ui.Help.CurrentMenu == MenuHelpView.Menu.WalkingHud, ui.Help.CurrentMenu.ToString());
        yield return Invoke(ui.Help.Root.Q<Button>("menu-help-close"));
        Hard("CLOSING_THE_WINDOW_CLEARS_THE_FLAG", !ui.Help.IsOpen && !manager.ModalOpen, $"help={ui.Help.IsOpen} flag={manager.ModalOpen}");
        yield return Press(Key.Tab);
        Hard("SHORTCUTS_RESUME_TAB_OPENS_WORK_PLAN", manager.WorkPlanOpen && !ui.Help.IsOpen, $"plan={manager.WorkPlanOpen} help={ui.Help.IsOpen}");
        yield return Press(Key.Tab);
        Hard("SHORTCUTS_RESUME_TAB_CLOSES_WORK_PLAN", !manager.WorkPlanOpen, "");
        yield return Press(Key.O);
        Hard("SHORTCUTS_RESUME_O_OPENS_OBJECTIVES", ui.CurrentScreen == ScenarioOneUiRoot.UiScreen.Objectives && !ui.Help.IsOpen, ui.CurrentScreen.ToString());
        yield return Press(Key.O);
        Hard("SHORTCUTS_RESUME_O_CLOSES_OBJECTIVES", ui.CurrentScreen == ScenarioOneUiRoot.UiScreen.None, ui.CurrentScreen.ToString());
        // F1 is the same window: it opens it, and closes it again.
        yield return Press(Key.F1);
        bool opened = ui.Help.IsOpen && manager.ModalOpen;
        yield return Press(Key.F1);
        Hard("F1_OPENS_AND_CLOSES_THE_WINDOW_AND_THE_FLAG_FOLLOWS", opened && !ui.Help.IsOpen && !manager.ModalOpen, $"opened={opened} help={ui.Help.IsOpen} flag={manager.ModalOpen}");
        // Leave the first-use state as the rest of the run expects it.
        PlayerPrefs.DeleteKey(prefix + MenuHelpView.Menu.WorkPlan);
        yield return Wait(0.3f);
    }

    // ---- the authored layout, from the real stand ----
    private IEnumerator Layout()
    {
        planner.Refresh();
        IReadOnlyList<float> lines = planner.LayoutLines;
        IReadOnlyList<RackRow> rows = planner.Rows(0f);
        int living = Living().Length;
        Hard("LAYOUT_PLANTATION_ROWS", rows.Count == 41, "rows=" + rows.Count);
        Hard("LAYOUT_HAS_FIVE_RACKS", lines.Count == 5, "racks=" + lines.Count);
        var indices = lines.Select(z => RackRows.Nearest(rows, z)).ToList();
        Note("LAYOUT_ROWS", $"rows={rows.Count} racks={lines.Count} rowIndices=[{string.Join(",", indices)}] rowsBetween={string.Join(",", indices.Zip(indices.Skip(1), (a, b) => (b - a - 1).ToString()))} centresZ=[{string.Join(", ", lines.Select(z => F(z, "0.00")))}] spacingM=[{string.Join(", ", lines.Zip(lines.Skip(1), (a, b) => F(b - a, "0.0")))}]");
        var all = new List<string>();
        for (int i = 0; i < lines.Count; i++)
        {
            RackProposal p = planner.Propose(new RackPoint(0f, lines[i]), 0f);
            all.AddRange(p.TreeIds);
            Note("LAYOUT_RACK_" + (i + 1), $"trees={p.TreeCount} stemM3={F(p.StemVolumeM3, "0.00")} soldM3={F(p.SoldVolumeM3, "0.00")} revenue={p.RevenueCents} cost={p.CostCents} net={p.NetCents} start={p.Selection.Start} end={p.Selection.End}");
            Hard($"LAYOUT_RACK_{i + 1}_CLIPS_EXACTLY_TO_THE_STAND_BOUNDARY",
                Mathf.Abs(Mathf.Abs(p.Selection.Start.X) - 40f) < 0.01f && Mathf.Abs(Mathf.Abs(p.Selection.End.X) - 40f) < 0.01f
                && Mathf.Abs(p.Selection.Start.Z - p.Selection.End.Z) < 0.01f, $"{p.Selection.Start} → {p.Selection.End}");
            Hard($"LAYOUT_RACK_{i + 1}_ALL_TREES_ON_THE_LINE_AND_INSIDE_THE_STAND", p.TreeCount > 20 && p.Selection.Trees.All(t => Mathf.Abs(t.Z - lines[i]) <= 1.2001f && Mathf.Abs(t.X) <= 40f), $"trees={p.TreeCount}");
            RackProposal again = planner.Propose(new RackPoint(0f, lines[i]), 0f);
            Hard($"LAYOUT_RACK_{i + 1}_SELECTION_IS_DETERMINISTIC", Ids(again.TreeIds) == Ids(p.TreeIds) && p.TreeIds.SequenceEqual(again.TreeIds), "");
            RackProposal fromTree = planner.Propose(new RackPoint(7.3f, lines[i] + 0.55f), 0f);
            Hard($"LAYOUT_RACK_{i + 1}_SNAPS_TO_THE_SAME_ROW_FROM_ANY_POINT_ON_IT", Ids(fromTree.TreeIds) == Ids(p.TreeIds), $"{fromTree.TreeCount} vs {p.TreeCount}");
        }
        layoutIds = all.OrderBy(i => i, StringComparer.Ordinal).ToList();
        ScenarioHarvestJob job = manager.QuoteTrees(all);
        long net = job.RevenueCents - job.CostCents;
        Note("LAYOUT_TOTAL", $"racks={lines.Count} trees={all.Count} ofLiving={living} shareOfStems={F(100.0 * all.Count / living, "0.0")}% stemM3={F(job.TotalStemVolumeCm3 / 1e6, "0.00")} soldM3={F(job.SoldVolumeCm3 / 1e6, "0.00")} revenue={job.RevenueCents} cost={job.CostCents} net={net} cashNow={manager.CashCents} cashAfter={manager.CashCents + net} eligible={job.Eligible}");
        Hard("LAYOUT_TOTAL_QUOTE_IS_ELIGIBLE", job.Eligible && job.Orders.Count == all.Count, $"orders={job.Orders.Count} problem={job.Problem}");
        Hard("LAYOUT_RACKS_DO_NOT_OVERLAP", all.Distinct().Count() == all.Count, "");
        yield return null;
    }

    // ---- preview is inert, rotation changes the selection ----
    private IEnumerator Preview()
    {
        int orders = manager.WorkOrders.Count, fell = marks.GetMarkedIds().Count, crop = marks.GetCropTreeIds().Count, living = Living().Length;
        IReadOnlyList<float> lines = planner.LayoutLines;
        RackProposal a = planner.Propose(new RackPoint(0f, lines[0]), 0f);
        RackProposal b = planner.Propose(new RackPoint(0f, lines[0]), 15f);
        RackProposal c = planner.Propose(new RackPoint(0f, lines[0]), 90f);
        Hard("PREVIEW_ROTATING_CHANGES_THE_SELECTION", Ids(a.TreeIds) != Ids(b.TreeIds) && Ids(b.TreeIds) != Ids(c.TreeIds), $"{a.TreeCount}/{b.TreeCount}/{c.TreeCount} trees");
        Hard("PREVIEW_ROTATED_RACKS_END_ON_THE_STAND_BOUNDARY",
            new[] { b, c }.All(p => (Mathf.Abs(Mathf.Abs(p.Selection.Start.X) - 40f) < 0.01f || Mathf.Abs(Mathf.Abs(p.Selection.Start.Z) - 40f) < 0.01f)
                && (Mathf.Abs(Mathf.Abs(p.Selection.End.X) - 40f) < 0.01f || Mathf.Abs(Mathf.Abs(p.Selection.End.Z) - 40f) < 0.01f)),
            $"{b.Selection.Start}→{b.Selection.End} {c.Selection.Start}→{c.Selection.End}");
        Hard("PREVIEW_CHANGES_NOTHING", manager.WorkOrders.Count == orders && marks.GetMarkedIds().Count == fell && marks.GetCropTreeIds().Count == crop && Living().Length == living, "");
        // A bounded rack (explicit start and end) takes only the trees between them.
        RackProposal part = planner.ProposeBetween(new RackPoint(-10f, lines[0]), new RackPoint(10f, lines[0]));
        Hard("BOUNDED_RACK_TAKES_ONLY_THE_STRETCH_BETWEEN_START_AND_END", part.TreeCount > 5 && part.TreeCount < a.TreeCount / 2 && part.Selection.Trees.All(t => t.X >= -10.001f && t.X <= 10.001f), $"{part.TreeCount} of {a.TreeCount}");
        yield return null;
    }

    // ---- the in-world tool ----
    private IEnumerator ForestTool()
    {
        IReadOnlyList<float> lines = planner.LayoutLines;
        yield return AimAtGround(-12f, lines[0]);
        // The real key path: B (first use on this device shows the short explanation first).
        yield return Press(Key.B);
        bool introShown = ui.Help.IsOpen && ui.Help.CurrentMenu == MenuHelpView.Menu.Rack;
        if (introShown)
        {
            Hard("B_FIRST_USE_SHOWS_THE_RACK_EXPLANATION", Texts(ui.Help.Root).Contains("permanent routes used to remove timber from the stand")
                && Texts(ui.Help.Root).Contains("machinery access for this and future thinnings"), Texts(ui.Help.Root).Replace("\n", " / "));
            yield return Shoot("20-rack-intro");
            Button rackMore = ui.Help.Root.Q<Button>("menu-help-more");
            string rackBrief = Texts(ui.Help.Root);
            yield return Invoke(rackMore);
            Hard("MORE_DETAIL_OPENS_THE_FULL_RACK_EXPLANATION", Texts(ui.Help.Root).Length > rackBrief.Length + 100 && rackMore.text == MenuHelpView.LessDetailLabel && Texts(ui.Help.Root).Contains("planted row"), "");
            yield return Invoke(rackMore);
            Hard("LESS_DETAIL_RETURNS_TO_THE_SHORT_RACK_TEXT", Texts(ui.Help.Root) == rackBrief, "");
            // The modal owns the keyboard: Tab, M and O change nothing while the rack screen is open (before the fix Tab opened the
            // Work Plan behind it, M replaced it with the Stand Map screen and O closed it).
            yield return Press(Key.Tab);
            Hard("RACK_MODAL_BLOCKS_TAB_WORK_PLAN", ui.Help.IsOpen && !manager.WorkPlanOpen, $"help={ui.Help.IsOpen} plan={manager.WorkPlanOpen}");
            yield return Press(Key.M);
            Hard("RACK_MODAL_BLOCKS_MAP_SHORTCUT", ui.Help.IsOpen && ui.CurrentScreen == ScenarioOneUiRoot.UiScreen.None, $"help={ui.Help.IsOpen} screen={ui.CurrentScreen}");
            yield return Press(Key.O);
            Hard("RACK_MODAL_BLOCKS_OBJECTIVES_SHORTCUT", ui.Help.IsOpen && ui.CurrentScreen == ScenarioOneUiRoot.UiScreen.None, $"help={ui.Help.IsOpen} screen={ui.CurrentScreen}");
            Hard("RACK_MODAL_STAYS_THE_RACK_SCREEN", ui.Help.CurrentMenu == MenuHelpView.Menu.Rack && Texts(ui.Help.Root) == rackBrief, ui.Help.CurrentMenu.ToString());
            yield return Invoke(ui.Help.Root.Q<Button>("menu-help-close"));
            yield return Wait(0.4f);
        }
        else Unverified("B key did not open the first-use rack screen in the Editor (input delivery); the tool is entered through its public method instead");
        if (!tool.Active) { Unverified("B key path did not open the tool; entering through RackTool.Enter()"); PlayerPrefs.SetInt(MenuHelpView.PreferencePrefix + MenuHelpView.Menu.Rack, 1); tool.Enter(); }
        else Hard("B_OPENS_THE_RACK_TOOL", true, "");
        yield return AimAtGround(-12f, lines[0]);
        RackProposal p = tool.Proposal;
        Hard("TOOL_PREVIEW_FOLLOWS_THE_AIM_ON_PLANNED_RACK_1", p != null && p.LayoutIndex == 0 && p.TreeCount > 20, p == null ? "no proposal" : $"layout={p.LayoutIndex} trees={p.TreeCount} angle={p.AngleDegrees}");
        Hard("TOOL_DEFAULT_DIRECTION_FOLLOWS_THE_PLANTATION_ROWS", p != null && Mathf.Abs(Mathf.DeltaAngle(p.AngleDegrees, 0f)) < 0.5f, p == null ? "" : "angle=" + p.AngleDegrees);
        GameObject preview = GameObject.Find("Extraction rack preview (runtime)");
        MeshRenderer corridor = preview != null ? preview.GetComponentInChildren<MeshRenderer>() : null;
        Hard("TOOL_SHOWS_A_TRANSLUCENT_CORRIDOR", corridor != null && corridor.enabled && corridor.sharedMaterial != null && corridor.sharedMaterial.color.a < 0.6f && corridor.bounds.size.x > 70f, corridor == null ? "none" : $"alpha={corridor.sharedMaterial.color.a} sizeX={corridor.bounds.size.x}");
        Hard("TOOL_CORRIDOR_STAYS_INSIDE_THE_STAND", corridor != null && corridor.bounds.min.x >= -40.01f && corridor.bounds.max.x <= 40.01f && corridor.bounds.min.z >= -40.01f && corridor.bounds.max.z <= 40.01f, corridor == null ? "" : corridor.bounds.ToString());
        int active = preview != null ? preview.GetComponentsInChildren<LineRenderer>().Count(l => l.gameObject.activeSelf) : 0;
        Hard("TOOL_HIGHLIGHTS_EVERY_TREE_THE_RACK_WOULD_TAKE", p != null && active == p.TreeCount * 2, $"lines={active} trees={(p == null ? 0 : p.TreeCount)}");
        Hard("TOOL_PANEL_SHOWS_THE_FIGURES", Texts(UiRoot().Q<VisualElement>("hud-rack-panel")).Contains("Extraction rack") && Texts(UiRoot().Q<VisualElement>("hud-rack-panel")).Contains("Estimated timber") && Texts(UiRoot().Q<VisualElement>("hud-rack-panel")).Contains("Estimated net return"), Texts(UiRoot().Q<VisualElement>("hud-rack-panel")).Replace("\n", " / "));
        yield return Shoot("21-rack-preview");

        // The mouse wheel turns the corridor in steps; the selection follows.
        float before = tool.AngleDegrees; string idsBefore = Ids(tool.Proposal.TreeIds);
        yield return Wheel(120f);
        bool wheel = Mathf.Abs(Mathf.DeltaAngle(tool.AngleDegrees, before)) > 1f;
        if (!wheel) { Unverified("Editor-synthetic scroll was not delivered; rotating through RackTool.Turn"); tool.Turn(1); yield return null; yield return null; }
        else Hard("WHEEL_TURNS_THE_CORRIDOR_ONE_STEP", Mathf.Abs(Mathf.Abs(Mathf.DeltaAngle(tool.AngleDegrees, before)) - RackTool.RotateStepDegrees) < 0.01f, $"{before}→{tool.AngleDegrees}");
        yield return null; yield return null;
        Hard("WHEEL_ROTATION_UPDATES_THE_SELECTED_TREES", tool.Proposal != null && Ids(tool.Proposal.TreeIds) != idsBefore && tool.Proposal.LayoutIndex == -1, tool.Proposal == null ? "" : $"{tool.Proposal.TreeCount} trees, layout={tool.Proposal.LayoutIndex}");
        yield return Shoot("22-rack-rotated", new Vector2Int(1920, 1080));
        tool.Turn(-1); yield return null; yield return null;
        Hard("TURNING_BACK_RESTORES_THE_SAME_RACK", tool.Proposal != null && Ids(tool.Proposal.TreeIds) == idsBefore, "");

        // Cancel changes nothing.
        int orders = manager.WorkOrders.Count, fell = marks.GetMarkedIds().Count, living = Living().Length;
        tool.Exit(); yield return null; yield return null;
        Hard("CANCEL_EXITS_THE_TOOL_AND_CHANGES_NOTHING", !tool.Active && !RackTool.AnyActive && manager.WorkOrders.Count == orders && marks.GetMarkedIds().Count == fell && Living().Length == living
            && manager.RackGroups.Count == 0, $"orders {orders}→{manager.WorkOrders.Count}");
        Hard("CANCEL_HIDES_THE_PREVIEW", GameObject.Find("Extraction rack preview (runtime)") == null || !GameObject.Find("Extraction rack preview (runtime)").activeSelf, "");
        // The key path for Esc as well: B again, then Esc.
        tool.Enter(); yield return AimAtGround(-12f, lines[0]);
        yield return Press(Key.Escape);
        Hard("ESC_LEAVES_THE_TOOL", !tool.Active, $"active={tool.Active}");
        Hard("ESC_CANCELS_WITHOUT_FREEING_THE_MOUSE", UnityEngine.Cursor.lockState == CursorLockMode.Locked, $"cursor={UnityEngine.Cursor.lockState}");
        if (tool.Active) tool.Exit();
        UnityEngine.Cursor.lockState = CursorLockMode.Locked; UnityEngine.Cursor.visible = false;
        yield return null; yield return null;
        // Confirm: the rack joins the DRAFT Work Plan.
        yield return AimAtGround(-12f, lines[0]);
        tool.Enter(); yield return AimAtGround(-12f, lines[0]);
        string expected = Ids(tool.Proposal.TreeIds); int expectedCount = tool.Proposal.TreeCount;
        bool done = tool.Confirm();
        yield return null;
        List<ScenarioOneWorkOrder> added = manager.WorkOrders.Where(o => o.IsOpen && o.type == ScenarioWorkType.FellTree).ToList();
        Hard("CONFIRM_ADDS_THE_RACK_TREES_AS_DRAFT_FELLING_ORDERS", done && added.Count == expectedCount && added.All(o => o.status == ScenarioWorkStatus.Pending) && Ids(added.Select(o => o.targetTreeId)) == expected, $"added={added.Count} expected={expectedCount}");
        Hard("CONFIRM_MAKES_ONE_RACK_GROUP", manager.RackGroups.Count == 1 && manager.RackGroups[0].TreeCount == expectedCount && manager.RackGroups[0].LayoutIndex == 0, $"groups={manager.RackGroups.Count}");
        Hard("CONFIRM_NEEDS_NO_INDIVIDUAL_MARKS", marks.GetMarkedIds().Count == 0 && marks.GetCropTreeIds().Count == 0, "");
        Hard("CONFIRM_FIRES_THE_FIRST_LESSON", ui.Learning.IsDone("rack.create"), "");
        tool.Exit();
        for (int i = 0; i < 4; i++) yield return null;
        yield return Wait(0.8f);
        Hard("RACK_TREES_CARRY_VIOLET_PLANNED_MARKERS", ui.PlannedMarkers.RackMarkerCount == expectedCount && ui.PlannedMarkers.MarkerCount == expectedCount, $"markers={ui.PlannedMarkers.MarkerCount} rack={ui.PlannedMarkers.RackMarkerCount}");
        yield return AimAtGround(-12f, lines[0], 6f);
        yield return Shoot("23-rack-confirmed-forest");
        yield return Wait(0.6f);
        Note("CUE_AFTER_FIRST_RACK", ui.Learning.Summary);
        Hard("CUE_MOVES_ON_TO_THE_LAYOUT", ui.Learning.Summary.Contains("Complete the rack layout"), ui.Learning.Summary);
    }

    // ---- the Work Plan groups a rack ----
    private IEnumerator WorkPlanGroup()
    {
        ui.ShowWorkPlan(); yield return null; yield return null;
        if (ui.Help.IsOpen) yield return Invoke(ui.Help.Root.Q<Button>("menu-help-close"));
        yield return Wait(0.5f);
        string text = Texts(UiRoot());
        Hard("WORKPLAN_SHOWS_ONE_RACK_GROUP", text.Contains("Extraction rack 1") && text.Contains("DRAFT"), "");
        Hard("WORKPLAN_GROUP_HAS_TIMBER_AND_INCOME", text.Contains("Timber") && text.Contains("estimated income"), "");
        Hard("WORKPLAN_HAS_NO_OLD_HEADING_OR_ADD_BUTTON", !text.Contains("WHAT ARE YOU DOING") && BtnStarts("Add marked trees") == null && BtnStarts("Approve Work Plan") != null, "");
        Hard("WORKPLAN_STILL_REVIEWS_THE_WHOLE_JOB", text.Contains("Harvest and salvage job"), "");
        yield return Shoot("24-workplan-rack");
        // The group opens to its trees and a click removes the whole rack, nothing hidden.
        Button show = BtnStarts("Show trees");
        yield return Invoke(show);
        Hard("WORKPLAN_GROUP_EXPANDS_TO_THE_INDIVIDUAL_TREES", BtnStarts("Hide trees") != null, "");
        yield return Invoke(BtnStarts("Hide trees"));
        int before = OpenFellOrders();
        yield return Invoke(BtnStarts("Remove rack"));
        Hard("REMOVE_RACK_TAKES_THE_WHOLE_GROUP_OUT", OpenFellOrders() == 0 && manager.RackGroups.Count == 0 && before > 20, $"{before}→{OpenFellOrders()}");
        // Put it back through the tool so the next phases have it.
        ui.CloseAll(); yield return null; yield return null;
        planner.Refresh();
        Hard("REMOVED_RACK_CAN_BE_PLANNED_AGAIN", manager.ConfirmRack(planner.Propose(new RackPoint(0f, planner.LayoutLines[0]), 0f), "forest") > 20, "");
        yield return null;
    }

    // ---- the Stand Map tool, same backend ----
    private IEnumerator MapTool()
    {
        ui.ShowMap(); yield return null; yield return null;
        if (ui.Help.IsOpen) yield return Invoke(ui.Help.Root.Q<Button>("menu-help-close"));
        yield return Wait(0.5f);
        Button plan = BtnStarts("Plan rack");
        Hard("MAP_OFFERS_PLAN_RACK_AFTER_THE_FIRST_RACK", plan != null, "");
        yield return Invoke(plan);
        Hard("MAP_PLAN_RACK_MODE_IS_ON", ui.Map.RackMode, "");
        int n = ecology.CellsPerAxis;
        float z1 = planner.LayoutLines[1];
        int cell = ecology.GetCellIndex(new Vector3(0f, 0f, z1));
        var cells = UiRoot().Query<Button>(className: "map-cell").ToList();
        // The grid is built from the top row (north) down, west to east.
        int zIndex = cell / n, xIndex = cell % n;
        Button target = cells[(n - 1 - zIndex) * n + xIndex];
        yield return Invoke(target);
        yield return Wait(0.3f);
        RackProposal preview = ui.Map.RackPreview;
        Hard("MAP_CLICKING_A_SQUARE_PREVIEWS_THE_RACK_ON_THE_PLANNED_ROW", preview != null && preview.CanConfirm && preview.TreeCount > 15 && preview.LayoutIndex == 1, preview == null ? "none" : $"layout={preview.LayoutIndex} trees={preview.TreeCount}");
        yield return Shoot("25-map-plan-rack");
        string panel = Texts(UiRoot().Q<VisualElement>("map-rack-card"));
        Hard("MAP_PANEL_SHOWS_THE_SAME_FIGURES", panel.Contains("Estimated timber") && panel.Contains("Estimated net return") && panel.Contains("Add rack to Work Plan"), panel.Replace("\n", " / "));
        // Row nudge reaches a row the square's centre was not nearest to.
        string idsBefore = Ids(preview.TreeIds);
        yield return Invoke(BtnStarts("Row ▶"));
        Hard("MAP_ROW_NUDGE_MOVES_TO_THE_NEXT_ROW", ui.Map.RackPreview != null && Ids(ui.Map.RackPreview.TreeIds) != idsBefore, "");
        yield return Invoke(BtnStarts("◀ Row"));
        Hard("MAP_ROW_NUDGE_BACK_RETURNS", ui.Map.RackPreview != null && Ids(ui.Map.RackPreview.TreeIds) == idsBefore, "");
        int groups = manager.RackGroups.Count;
        yield return Invoke(UiRoot().Q<Button>("map-rack-confirm"));
        Hard("MAP_ADDS_THE_RACK_THROUGH_THE_SAME_BACKEND", manager.RackGroups.Count == groups + 1 && manager.RackGroups.Any(g => g.LayoutIndex == 1) && ui.Learning.IsDone("rack.map"), $"groups={manager.RackGroups.Count}");
        yield return Invoke(UiRoot().Q<Button>("map-rack-cancel"));
        Hard("MAP_CANCEL_LEAVES_RACK_MODE", !ui.Map.RackMode, "");
        ui.CloseAll(); yield return null; yield return null;
    }

    // ---- save and load keep the draft, nothing hidden ----
    private IEnumerator SaveLoad()
    {
        List<string> idsBefore = manager.WorkOrders.Where(o => o.IsOpen && o.type == ScenarioWorkType.FellTree).Select(o => o.targetTreeId).OrderBy(i => i, StringComparer.Ordinal).ToList();
        ForestSaveData data = saves.CaptureData();
        saves.LoadData(data, false);
        for (int i = 0; i < 4; i++) yield return null;
        yield return Wait(0.9f);
        List<string> idsAfter = manager.WorkOrders.Where(o => o.IsOpen && o.type == ScenarioWorkType.FellTree).Select(o => o.targetTreeId).OrderBy(i => i, StringComparer.Ordinal).ToList();
        Hard("LOAD_KEEPS_EVERY_PLANNED_RACK_TREE", idsBefore.Count > 40 && idsBefore.SequenceEqual(idsAfter) && manager.WorkOrders.All(o => o.status == ScenarioWorkStatus.Pending || !o.IsOpen), $"{idsBefore.Count}→{idsAfter.Count}");
        Hard("LOAD_LEAVES_NOTHING_HIDDEN_PLANNED_TREES_STILL_CARRY_MARKERS", ui.PlannedMarkers.MarkerCount == idsAfter.Count, $"markers={ui.PlannedMarkers.MarkerCount} orders={idsAfter.Count}");
        Hard("LOAD_DOES_NOT_ADD_OR_ARCHIVE_ANY_SAVE_FIELD_FOR_RACKS", !typeof(ScenarioOneSaveData).GetFields().Any(f => f.Name.ToLowerInvariant().Contains("rack"))
            && !typeof(ScenarioOneWorkOrder).GetFields().Any(f => f.Name.ToLowerInvariant().Contains("rack")) && ForestSaveData.CurrentVersion == 20, $"saveVersion={ForestSaveData.CurrentVersion}");
        Hard("LOAD_PRESERVES_THE_TEACHING_STAGE", manager.RackFirstStageActive, "");
        Note("LOAD_RACK_GROUPING", $"groupsAfterLoad={manager.RackGroups.Count} (grouping is session state; rack trees are ordinary draft felling jobs after a load)");
        yield return Wait(0.3f);
    }

    // ---- finish the authored layout ----
    private IEnumerator CompleteLayout()
    {
        planner.Refresh();
        for (int i = 0; i < planner.LayoutLines.Count; i++)
        {
            if (manager.RackGroups.Any(g => g.LayoutIndex == i)) continue;
            planner.Refresh();
            RackProposal p = planner.Propose(new RackPoint(0f, planner.LayoutLines[i]), 0f);
            if (p.CanConfirm && p.LayoutIndex == i) manager.ConfirmRack(p, "forest");
        }
        ui.RackLayout.Update(true);
        Hard("LAYOUT_COMPLETE_IN_THE_WORK_PLAN", ui.RackLayout.Complete && ui.RackLayout.Total == 5, $"done={ui.RackLayout.Done}/{ui.RackLayout.Total}");
        yield return Wait(0.7f);
        Hard("LAYOUT_LESSON_RECORDED", ui.Learning.IsDone("rack.layout"), "");
        Hard("CUE_AFTER_THE_LAYOUT_IS_APPROVAL", ui.Learning.Summary.Contains("Approve the rack work"), ui.Learning.Summary);
        Hard("STATUS_LINE_COUNTS_THE_RACKS", Texts(UiRoot()).Contains("Extraction racks: 5 of 5 in the Work Plan"), "");
        Hard("NO_INDIVIDUAL_MARKS_WERE_NEEDED", marks.GetMarkedIds().Count == 0 && marks.GetCropTreeIds().Count == 0, "");
        yield return TopDown("30-topdown-planned");
        ui.ShowMap(); yield return null; yield return null;
        if (ui.Help.IsOpen) yield return Invoke(ui.Help.Root.Q<Button>("menu-help-close"));
        yield return Invoke(BtnStarts("Plan rack"));
        yield return Wait(0.4f);
        yield return Shoot("26-map-layout-complete", new Vector2Int(1920, 1080));
        yield return Invoke(UiRoot().Q<Button>("map-rack-cancel"));
        ui.CloseAll(); yield return null; yield return null;
    }

    // ---- approve exactly the rack trees, advance, and see the corridors ----
    private IEnumerator ApproveAndHarvest()
    {
        ui.ShowWorkPlan(); yield return null; yield return null;
        if (ui.Help.IsOpen) yield return Invoke(ui.Help.Root.Q<Button>("menu-help-close"));
        yield return Wait(0.4f);
        // The draft is what is approved: every open felling order, and these must be exactly the five authored racks' trees.
        List<string> rackIds = manager.WorkOrders.Where(o => o.IsOpen && o.type == ScenarioWorkType.FellTree).Select(o => o.targetTreeId).OrderBy(i => i, StringComparer.Ordinal).ToList();
        Hard("DRAFT_IS_EXACTLY_THE_FIVE_AUTHORED_RACKS_BEFORE_APPROVAL", rackIds.SequenceEqual(layoutIds), $"draft={rackIds.Count} racks={layoutIds.Count}");
        yield return Shoot("27-workplan-five-racks", new Vector2Int(1920, 1080));
        yield return Invoke(BtnStarts("Approve Work Plan"));
        var approved = manager.WorkOrders.Where(o => o.status == ScenarioWorkStatus.Approved).ToList();
        Hard("APPROVE_EXECUTES_THE_EXPECTED_RACK_TREES_ONLY", approved.Count == rackIds.Count && approved.All(o => o.type == ScenarioWorkType.FellTree)
            && approved.Select(o => o.targetTreeId).OrderBy(i => i, StringComparer.Ordinal).SequenceEqual(rackIds) && manager.WorkOrders.Count(o => o.status == ScenarioWorkStatus.Pending) == 0, $"approved={approved.Count} racks={rackIds.Count}");
        yield return Wait(0.7f);
        Hard("APPROVAL_LESSON_RECORDED", ui.Learning.IsDone("rack.approve"), "");
        ScenarioHarvestJob quote = manager.GetHarvestQuote(true);
        long expectedNet = quote.RevenueCents - quote.CostCents; long cashBefore = manager.CashCents;
        int livingBefore = Living().Length;
        // Neighbour rows beside every rack must survive the harvest untouched.
        var lines = planner.LayoutLines;
        Func<int> neighbours = () => Living().Count(t => lines.Any(z => Mathf.Abs(t.transform.position.z - z) > 1.2f && Mathf.Abs(t.transform.position.z - z) <= 3.3f));
        int neighboursBefore = neighbours();
        Note("HARVEST_BEFORE", $"trees={livingBefore} rackTrees={rackIds.Count} neighbourTrees={neighboursBefore} cash={cashBefore} quotedNet={expectedNet}");
        int year = ecology.EcologicalYear;
        yield return Invoke(BtnStarts("Advance one year"));
        Hard("ADVANCE_RESOLVES_THE_APPROVED_RACKS", ecology.EcologicalYear == year + 1, $"year={ecology.EcologicalYear}");
        for (int i = 0; i < 8; i++) yield return null;
        Hard("HARVEST_FELLS_EXACTLY_THE_RACK_TREES", Living().Length == livingBefore - rackIds.Count, $"{livingBefore}→{Living().Length}");
        var stumps = FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).Where(t => t.IsStump).Select(t => t.TreeId).ToHashSet();
        Hard("EVERY_RACK_TREE_IS_NOW_A_STUMP", rackIds.All(stumps.Contains), $"stumps={stumps.Count}");
        Hard("NEIGHBOUR_ROWS_ARE_UNTOUCHED", neighbours() == neighboursBefore, $"{neighboursBefore}→{neighbours()}");
        planner.Refresh();
        var status = planner.LayoutStatus();
        Hard("HARVEST_LEAVES_AN_OPEN_CORRIDOR_ALONG_EVERY_RACK", status.Count == 5 && status.All(l => l.TreesLeft == 0 && l.Done), string.Join(";", status.Select(l => l.Index + ":" + l.TreesLeft)));
        long delta = manager.CashCents - cashBefore;
        Note("HARVEST_CASHFLOW", $"cashBefore={cashBefore} cashAfter={manager.CashCents} delta={delta} quotedNet={expectedNet} year1Report={(manager.AnnualReports.Count > 0 ? $"revenue={manager.AnnualReports.Last().timberRevenueCents} contractor={manager.AnnualReports.Last().contractorCostCents} soldM3={manager.AnnualReports.Last().harvestedVolumeM3:0.00}" : "none")}");
        Hard("HARVEST_CASHFLOW_MATCHES_THE_QUOTE", delta == expectedNet, $"delta={delta} quoted={expectedNet}");
        yield return Wait(0.8f);
        Hard("NO_PLANNED_MARKERS_REMAIN_AFTER_THE_HARVEST", ui.PlannedMarkers.MarkerCount == 0 && manager.RackGroups.Count == 0, $"markers={ui.PlannedMarkers.MarkerCount} groups={manager.RackGroups.Count}");
        yield return Shoot("33-annual-review-racks", new Vector2Int(1920, 1080), new Vector2Int(1280, 720));
        Button ack = UiRoot().Q<Button>("annual-review-acknowledge");
        if (ack != null) yield return Invoke(ack);
        ui.CloseAll(); yield return null; yield return null;
        yield return TopDown("31-topdown-harvested");
        // Eye level, looking along the first rack.
        player.transform.position = new Vector3(-36f, player.transform.position.y, lines[0] - 0.9f);
        player.LookToward(new Vector3(36f, 1.6f, lines[0] - 0.9f));
        for (int i = 0; i < 8; i++) yield return null;
        yield return Shoot("32-rack-corridor-eye-level");
        yield return Wait(0.8f);
        Hard("RESULT_LESSON_RECORDED", ui.Learning.IsDone("rack.result"), "");
        Hard("FIRST_CYCLE_WAS_RACK_ONLY", marks.GetMarkedIds().Count == 0 && marks.GetCropTreeIds().Count == 0 && manager.AnnualReviewSeen && manager.Interventions.Count == 1, $"marks={marks.GetMarkedIds().Count} crop={marks.GetCropTreeIds().Count} reviewSeen={manager.AnnualReviewSeen} interventions={manager.Interventions.Count}");
    }

    // ---- afterwards: inspection, Crop Trees and selective marking are introduced ----
    private IEnumerator Stage2()
    {
        Hard("STAGE2_RACK_FIRST_STAGE_ENDS_AFTER_THE_FIRST_INTERVENTION", !manager.RackFirstStageActive, "");
        string cue = ui.Learning.Summary;
        Hard("STAGE2_CUE_NOW_INTRODUCES_INSPECTION", cue.Contains("Inspect a living tree"), cue);
        ForestTree aimed = null;
        foreach (ForestTree t in Living().Where(t => !t.TreeId.StartsWith("Q", StringComparison.Ordinal) && Mathf.Abs(t.transform.position.x) < 20f).Take(40))
        {
            player.transform.position = t.transform.position - new Vector3(0, 0, 3);
            player.LookToward(t.transform.position + Vector3.up * 1.5f);
            for (int i = 0; i < 8; i++) yield return null;
            if (marks.AimedTree == t) { aimed = t; break; }
        }
        string prompt = player.InteractionPromptText();
        Hard("STAGE2_TREE_PROMPT_NOW_OFFERS_SELECTIVE_MARKING", aimed != null && prompt.Contains("[X] Fell") && prompt.Contains("[C] Crop Tree"), prompt);
        yield return Press(Key.X);
        Hard("STAGE2_X_MARKS_A_TREE_FOR_FELLING_AS_BEFORE", aimed != null && aimed.IsMarkedForFell, aimed == null ? "no tree" : aimed.TreeId);
        yield return Press(Key.X);
        yield return Press(Key.C);
        Hard("STAGE2_C_MARKS_A_CROP_TREE_AS_BEFORE", aimed != null && aimed.IsCropTree, "");
        yield return Press(Key.E);
        yield return Wait(0.4f);
        yield return Shoot("34-stage2-selective-inspection", new Vector2Int(1920, 1080));
        yield return Press(Key.E);
        ui.CloseAll();
    }
}
#endif
