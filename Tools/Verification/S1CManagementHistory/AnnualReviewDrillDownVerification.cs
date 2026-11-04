#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;
using CCF.Forestry.WorkEconomy;

// Disposable T-03 rendered fixture, adapted from the unchanged protected T-02 harness.
// Copy/run/remove via run_t03_gate.py. Never install permanently. Exact executeMethod guard only.
// Real APIs: felling/planting/approval/advance/cancellation. Two approved harvest targets are made
// absent in memory to exercise real failure resolution; one resulting event reason is blanked to
// represent an old incomplete record. No disk save is written. World hash + serialized scenario
// records + counts/acknowledgement are compared before/after every browsing/expansion phase.
// UI callbacks drive the actual UI Toolkit view and renderer; OS-native pointer targeting is untested.
public static class AnnualReviewDrillDownVerification
{
    private const string ExitKey = "CCF.S1C.T03.PendingExit";

    public static void Begin()
    {
        SessionState.EraseInt(ExitKey);
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        AnnualReviewDrillDownRunner.SetGameSize(1280, 720);
        EditorApplication.isPlaying = true;
    }

    public static void Finish(bool pass)
    {
        // Closing the Editor in the same coroutine that requests a Play Mode exit can race native domain teardown.
        // SessionState survives that reload so the Editor closes only once it is safely back in Edit Mode.
        SessionState.SetInt(ExitKey, pass ? 0 : 1);
        ResumePendingExit();
        EditorApplication.ExitPlaymode();
    }

    [InitializeOnLoadMethod]
    private static void ResumePendingExit()
    {
        if (SessionState.GetInt(ExitKey, -1) < 0) return;
        EditorApplication.update -= ExitWhenStopped;
        EditorApplication.update += ExitWhenStopped;
    }

    private static void ExitWhenStopped()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling || EditorApplication.isUpdating) return;
        int code = SessionState.GetInt(ExitKey, -1);
        if (code < 0) return;
        SessionState.EraseInt(ExitKey);
        EditorApplication.update -= ExitWhenStopped;
        EditorApplication.delayCall += () => EditorApplication.Exit(code);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (!Environment.GetCommandLineArgs().Any(a => a == "AnnualReviewDrillDownVerification.Begin")) return;
        new GameObject("S1-C T-03 year browsing verification").AddComponent<AnnualReviewDrillDownRunner>();
    }
}

public sealed class AnnualReviewDrillDownRunner : MonoBehaviour
{
    private const string EarlierText = "◀ Earlier year";
    private const string LaterText = "Later year ▶";

    private string output, mode;
    private Vector2Int size = new Vector2Int(1280, 720);
    private int passes, failures, shots;
    private ScenarioOneUiRoot ui;
    private ScenarioOneManager manager;
    private ForestSaveController saves;
    private ForestEcologyController ecology;
    private ForestTreeMarkingManager marks;
    private StandaloneSessionMenu menu;
    private readonly List<string> errors = new List<string>();
    private readonly List<string> notes = new List<string>();
    private readonly List<string> results = new List<string>();
    private readonly StringBuilder gateTable = new StringBuilder();
    private readonly Dictionary<string, string> texts = new Dictionary<string, string>();

    // ---------------------------------------------------------------- Editor plumbing

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
        object gameSize = Activator.CreateInstance(sizeType, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
            null, new object[] { Enum.ToObject(kind, 1), width, height, "S1C T03 " + width + "x" + height }, null);
        int index = (int)group.GetType().GetMethod("GetTotalCount").Invoke(group, null);
        group.GetType().GetMethod("AddCustomSize").Invoke(group, new[] { gameSize });
        Type viewType = assembly.GetType("UnityEditor.GameView"); EditorWindow view = EditorWindow.GetWindow(viewType);
        viewType.GetProperty("selectedSizeIndex", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).SetValue(view, index);
        PropertyInfo gizmos = viewType.GetProperty("showGizmos", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (gizmos != null && gizmos.CanWrite) gizmos.SetValue(view, false);
        view.Show(); view.Focus(); view.Repaint();
    }

    private IEnumerator Start()
    {
        output = Environment.GetEnvironmentVariable("CCF_DIAG_DIR");
        if (string.IsNullOrEmpty(output)) throw new InvalidOperationException("CCF_DIAG_DIR must name durable evidence outside the repo");
        Directory.CreateDirectory(output);
        Directory.CreateDirectory(Path.Combine(output, "review-texts"));
        Directory.CreateDirectory(Path.Combine(output, "shots"));
        mode = (Environment.GetEnvironmentVariable("CCF_T02_MODE") ?? "candidate").Trim().ToLowerInvariant();
        gateTable.AppendLine("label\ttitle\tfooterVisible\tneedsAnnualReview\tcompletionAck\tteachingAck\tearlierEnabled\tlaterEnabled\tseen");
        Application.logMessageReceived += OnLogMessage;
        Exception failure = null;
        var stack = new Stack<IEnumerator>();
        stack.Push(Play());
        while (stack.Count > 0)
        {
            bool more; object current = null;
            try { more = stack.Peek().MoveNext(); if (more) current = stack.Peek().Current; }
            catch (Exception error) { failure = error; break; }
            if (!more) { stack.Pop(); continue; }
            if (current is IEnumerator nested) stack.Push(nested); else yield return current;
        }

        if (errors.Count > 0)
        {
            failures++;
            Debug.Log("S1C_T03_CHECK_FAIL NO_CONSOLE_ERRORS " + string.Join(" | ", errors.Take(4)));
        }
        bool pass = failure == null && failures == 0;
        Application.logMessageReceived -= OnLogMessage;
        WriteReports(pass, failure);
        Debug.Log(pass
            ? $"S1C_T03_HARNESS_PASS mode={mode} checks={passes} shots={shots}"
            : $"S1C_T03_HARNESS_FAIL mode={mode} checks={passes} failures={failures} exception={failure}");
        AnnualReviewDrillDownVerification.Finish(pass);
    }

    // ---------------------------------------------------------------- the scenario


    private string scenarioBefore;
    private string hashBefore;
    private int eventsBefore, reportsBefore, yearBefore;
    private long cashBefore;
    private int cancelledId;

    private IEnumerator Play()
    {
        yield return StartSession();
        yield return BuildFixture();
        Check(manager.HistoryYears.Count == 5, "FIVE_COMPLETED_YEARS");
        Check(manager.CycleState != null, "DEFERRED_S1D_T02_CYCLE_STATE_RUNTIME_IMPORT");
        Check(manager.HistoryYears.First(y => y.Year == 1).HarvestSettlement.GroupedOrderIds.Count == 12, "TWELVE_ORDERS_ONE_REAL_JOB");
        var plants = manager.HistoryYears.First(y => y.Year == 2).ResolvedWork;
        Check(plants.Count(r => r.Event.outcome == ScenarioManagementOutcome.Succeeded) == 2
            && plants.Select(r => r.Event.executionMethod).Distinct().Count() == 2, "PLANTING_OWNER_AND_CONTRACTOR_REAL_RESOLUTION");
        Check(!manager.WorkOrders.Any(o => o.workOrderId == cancelledId), "CANCELLED_ORDER_REMOVED_FROM_REAL_MANAGER");
        hashBefore = WorldHash();
        scenarioBefore = JsonUtility.ToJson(manager.CaptureSaveData());
        eventsBefore = manager.ManagementEvents.Count; reportsBefore = manager.AnnualReports.Count;
        yearBefore = manager.CurrentEcologicalYear; cashBefore = manager.CashCents;
        notes.Add("world hash after fixture: " + hashBefore);
        notes.Add("Failure fixture: two approved target IDs replaced in memory before real AdvanceYear; second recorded failure reason then blanked to model absent provenance.");

        ui.CloseAll(); yield return Settle(4); ui.ShowReview(); yield return Settle(8);
        yield return GoYear(1);
        Check(Modal().Query(className: "work-detail").ToList().All(e => e.resolvedStyle.display == DisplayStyle.None), "AGGREGATE_FIRST_DETAILS_COLLAPSED");
        yield return Toggle("done-FellTree");
        Check(IsOpen("done-FellTree"), "TOGGLE_OPENS");
        yield return GoYear(2); yield return Toggle("done-PlantJuvenile");
        Check(IsOpen("done-PlantJuvenile"), "OTHER_YEAR_OPENS_OWN_GROUP");
        yield return GoYear(1); Check(IsOpen("done-FellTree"), "YEAR1_OPEN_RESTORED");
        yield return Toggle("done-FellTree"); Check(!IsOpen("done-FellTree"), "TOGGLE_CLOSES");
        yield return GoYear(2); Check(IsOpen("done-PlantJuvenile"), "YEAR2_OPEN_RESTORED");
        yield return GoYear(1); Check(!IsOpen("done-FellTree"), "YEAR1_CLOSED_RESTORED");
        yield return Toggle("done-FellTree");
        ui.CloseAll(); yield return Settle(4); ui.ShowReview(); yield return Settle(8);
        Check(Title().Contains("Year 5 complete"), "REOPEN_STARTS_LATEST");
        yield return GoYear(1); Check(!IsOpen("done-FellTree"), "REOPEN_CLEARS_PRIOR_YEAR_PRESENTATION");
        yield return GoYear(2); Check(!IsOpen("done-PlantJuvenile"), "REOPEN_CLEARS_ALL_YEAR_PRESENTATION");
        AssertReadOnly("INTERACTION_ROUNDTRIP");

        foreach (Vector2Int target in Sizes())
        {
            yield return UseSize(target);
            ui.CloseAll(); yield return Settle(4); ui.ShowReview(); yield return Settle(8);
            yield return GoYear(1);
            Check(Modal().Query(name: "annual-harvest-settlement").ToList().Count == 1, "ONE_SETTLEMENT_" + target);
            var settlement = manager.HistoryYears.First(y => y.Year == 1).HarvestSettlement;
            string settlementText = Text(Modal().Q(name: "annual-harvest-settlement"));
            Check(settlementText.Contains("12 harvest order(s)") && settlementText.Contains("Timber income " + UiKit.Money(settlement.SettlementRevenueCents))
                && settlementText.Contains("harvesting cost " + UiKit.Money(settlement.SettlementContractorCostCents)), "SETTLEMENT_EXACT_RECORDED_MONEY_" + target);
            yield return RecordState("harvest-collapsed-" + target);
            yield return Shot("harvest-collapsed", false);
            yield return Toggle("done-FellTree");
            var harvest = Detail("done-FellTree");
            string ht = Text(harvest);
            Check(ht.Contains("No money is recorded for a single order or tree.") && !ht.Contains("€")
                && !ht.Contains("contractor cost") && !ht.Contains("materials") && !ht.Contains("your time"), "NO_PER_ORDER_HARVEST_MONEY_" + target);
            // 12 task entries + the grouped-harvest note + the T-04 provenance row (RECORDED) at the top.
            Check(harvest.childCount == 14 && !ht.Contains("not recorded"), "ALL_TWELVE_RECORDED_TARGETS_PRESENT_" + target);
            yield return RecordState("harvest-expanded-" + target);
            yield return Shot("harvest-expanded", false);
            yield return FocusShot("harvest-expanded-scroll", harvest.Children().Last());
            Check(Modal().Q<ScrollView>().verticalScroller.highValue > 0, "LARGE_YEAR_SCROLL_AVAILABLE_" + target);
            yield return Toggle("done-FellTree");
            yield return GoYear(2); yield return Toggle("done-PlantJuvenile");
            string pt = Text(Detail("done-PlantJuvenile"));
            Check(pt.Contains("done by you") && pt.Contains("done by contractor") && pt.Contains("your time")
                && pt.Contains("materials") && pt.Contains("contractor cost") && !pt.Contains("not recorded"), "PLANTING_OWN_RECORDED_VALUES_" + target);
            Check(Modal().Q(name: "annual-harvest-settlement") == null, "NO_SETTLEMENT_NON_HARVEST_YEAR_" + target);
            yield return RecordState("planting-expanded-" + target);
            yield return Shot("planting-expanded", false);
            yield return FocusShot("planting-expanded-scroll", Detail("done-PlantJuvenile").Children().Last());
            yield return GoYear(3); yield return Toggle("cancelled");
            string ct = Text(Detail("cancelled"));
            Check(ct.Contains("CANCELLED") && ct.Contains("cancelled in year 3") && ct.Contains("order " + cancelledId)
                && ct.Contains("execution method not recorded") && !ct.Contains("Reason:") && !ct.Contains("€"), "CANCELLATION_SURVIVING_EVENT_ONLY_" + target);
            yield return RecordState("cancelled-expanded-" + target);
            yield return Shot("cancelled-expanded", false);
            yield return GoYear(4); yield return Toggle("failed");
            string ft = Text(Detail("failed"));
            Check(Detail("failed").Query<Label>().ToList().Count(e => e.text == "FAILED") == 2, "FAILED_CHIPS_BOTH_" + target);
            Check(ft.Contains("Reason: Cancelled: target missing") && ft.Contains("Reason: not recorded")
                && ft.Split(new[] { "not recorded" }, StringSplitOptions.None).Length == 2 && !ft.Contains("€"), "FAILED_REASONS_EXACT_AND_ABSENT_ONLY_" + target);
            yield return RecordState("failed-expanded-" + target);
            yield return Shot("failed-expanded", false);
            yield return FocusShot("failed-expanded-scroll", Detail("failed").Children().Last());
            yield return GoYear(5);
            Check(Text(Modal()).Contains("No work was resolved this year.") && Modal().Query<Button>().ToList().All(b => !b.name.StartsWith("work-detail-toggle-")), "NO_WORK_YEAR_NO_FALSE_DETAILS_" + target);
            yield return RecordState("no-work-" + target);
            AssertReadOnly("RENDER_PASS_" + target);
        }
        yield return GoYear(1);
        Check(!FooterVisible(), "OLDER_YEAR_FOOTER_HIDDEN");
        yield return ClickButton(ui.RootElement.Q<Button>("annual-review-acknowledge"), false);
        AssertReadOnly("OLDER_YEAR_ACK_CALLBACK");
        Check(!ui.AdvanceFromWorkPlan() && manager.CurrentEcologicalYear == yearBefore, "LATEST_ACK_STILL_BLOCKS_ADVANCE_FROM_OLDER_VIEW");
        yield return GoYear(5);
        Check(FooterVisible() && ui.NeedsAnnualReview, "LATEST_ACK_PENDING");
        yield return ClickButton(ui.RootElement.Q<Button>("annual-review-acknowledge"), true);
        Check(manager.AnnualReviewSeen && !ui.NeedsAnnualReview && manager.ManagementEvents.Count == eventsBefore + 1
            && manager.ManagementEvents.Last().eventType == ScenarioManagementEventType.AnnualReviewAcknowledged, "LATEST_ACK_EXACTLY_ONE_EVENT");
        Check(ui.AdvanceFromWorkPlan() && manager.CurrentEcologicalYear == yearBefore + 1, "LATEST_ACK_RELEASES_ADVANCE");
        notes.Add("Callback/API driven rendered check; no OS-native pointer delivery qualification.");
    }

    private IEnumerator BuildFixture()
    {
        yield return PlanFelling(12); AdvanceWithWork("harvest year 1");
        yield return PlanPlanting(true, WorkExecutionMethod.LandownerSimulated);
        yield return PlanPlanting(false, WorkExecutionMethod.Contractor);
        AdvanceWithWork("planting year 2");
        Check(manager.AdvanceYear(), "FIXTURE_IDLE_YEAR3");
        yield return PlanFelling(1);
        var cancel = manager.WorkOrders.Last(o => o.status == ScenarioWorkStatus.Pending);
        cancelledId = cancel.workOrderId;
        Check(manager.RemovePendingOrder(cancelledId), "FIXTURE_CANCEL_REAL_API_AT_YEAR3");
        yield return PlanFelling(2);
        Check(manager.ApprovePendingWork(), "FIXTURE_APPROVE_FAILURES");
        var failures = manager.WorkOrders.Where(o => o.status == ScenarioWorkStatus.Approved).ToList();
        Check(failures.Count == 2, "FIXTURE_TWO_APPROVED_FAILURE_TARGETS");
        for (int i = 0; i < failures.Count; i++) failures[i].targetTreeId = "T03-MISSING-" + i;
        Check(manager.AdvanceYear(), "FIXTURE_RESOLVE_INVALID_TARGETS_YEAR4");
        var failed = manager.ManagementEvents.Where(e => e.year == 4 && e.eventType == ScenarioManagementEventType.WorkResolved).ToList();
        Check(failed.Count == 2 && failed.All(e => e.outcome == ScenarioManagementOutcome.Failed && e.failureReason.Length > 0), "REAL_FAILURE_RESOLUTION_RECORDED");
        failed.Last().failureReason = "";
        Check(manager.AdvanceYear(), "FIXTURE_IDLE_YEAR5");
        yield return Settle(8);
    }

    private VisualElement Detail(string key) => Modal().Q(name: "work-detail-" + key);
    private bool IsOpen(string key) => Detail(key) != null && Detail(key).resolvedStyle.display == DisplayStyle.Flex;
    private static string Text(VisualElement element) => string.Join("\n", element.Query<TextElement>().ToList().Where(Visible).Select(e => e.text));
    private IEnumerator Toggle(string key)
    {
        Button button = Modal().Q<Button>("work-detail-toggle-" + key);
        Check(button != null && Detail(key) != null, "NAMED_TOGGLE_AND_BODY_" + key);
        if (button == null) throw new InvalidOperationException("missing toggle " + key);
        bool before = IsOpen(key);
        yield return ClickButton(button, true);
        Check(IsOpen(key) != before, "TOGGLE_CHANGED_DISPLAY_" + key);
    }
    private IEnumerator GoYear(int target)
    {
        for (int guard = 0; guard < 10; guard++)
        {
            string title = Title();
            int current = int.Parse(title.Split(new[] { "Year " }, StringSplitOptions.None)[1].Split(' ')[0]);
            if (current == target) yield break;
            yield return ClickButton(current > target ? EarlierText : LaterText);
        }
        throw new InvalidOperationException("cannot navigate to year " + target);
    }
    private void AssertReadOnly(string phase)
    {
        Check(WorldHash() == hashBefore && JsonUtility.ToJson(manager.CaptureSaveData()) == scenarioBefore
            && manager.ManagementEvents.Count == eventsBefore && manager.AnnualReports.Count == reportsBefore
            && manager.CurrentEcologicalYear == yearBefore && manager.CashCents == cashBefore && !manager.AnnualReviewSeen,
            "WORLD_SAVE_COUNTS_ACK_UNCHANGED_" + phase);
    }
    private IEnumerator FocusShot(string label, VisualElement target)
    {
        var scroll = Modal().Q<ScrollView>();
        scroll.ScrollTo(target); yield return Settle(8);
        Check(target.worldBound.Overlaps(scroll.contentViewport.worldBound), "SCROLL_REACHES_LAST_ENTRY_" + label + size);
        yield return Shot(label, false, false);
    }
    private IEnumerator StartSession()
    {
        for (int frame = 0; frame < 900 && (menu == null || !StandaloneSessionMenu.IsOpen); frame++)
        {
            if (menu == null) menu = UnityEngine.Object.FindFirstObjectByType<StandaloneSessionMenu>();
            yield return null;
        }
        if (menu == null || !StandaloneSessionMenu.IsOpen) throw new InvalidOperationException("the startup screen did not open");
        for (int frame = 0; frame < 180 && !Application.isFocused; frame++) yield return null;
        notes.Add("Application focused at start: " + Application.isFocused);

        menu.StartNewScenario();
        for (int frame = 0; frame < 600 && (ui == null || ui.Help == null || !ui.Help.IsOpen); frame++)
        {
            if (ui == null) ui = UnityEngine.Object.FindFirstObjectByType<ScenarioOneUiRoot>();
            yield return null;
        }
        if (ui == null || ui.Help == null) throw new InvalidOperationException("Scenario One UI missing after Start New");
        ui.CloseHelp();
        yield return Settle(20);
        manager = ui.Manager;
        saves = UnityEngine.Object.FindFirstObjectByType<ForestSaveController>();
        ecology = UnityEngine.Object.FindFirstObjectByType<ForestEcologyController>();
        marks = ui.Marking != null ? ui.Marking : UnityEngine.Object.FindFirstObjectByType<ForestTreeMarkingManager>();
        if (manager == null || saves == null || ecology == null || marks == null)
            throw new InvalidOperationException("manager, save controller, ecology or marking manager missing");
        Check(manager.AnnualReports.Count == 0 && manager.HistoryYears.Count == 0, "NEW_GAME_HAS_NO_COMPLETED_YEARS");
        notes.Add("stand geometry model " + ecology.StandGeometryModelVersion + "; trees at start " + Trees().Length);
        yield return RecordState("before-first-year");
    }

    private IEnumerator PlanFelling(int count)
    {
        manager.PlanningFellingOutcome = FellingMaterialOutcome.SellAndExtract;
        ForestTree[] trees = Trees();
        if (trees.Length < count) throw new InvalidOperationException("not enough thinnable trees");
        for (int i = 0; i < count; i++) marks.Mark(trees[i], TreeMarkType.Fell, false);
        manager.AddMarkedTreesToWorkPlan();
        yield return null;
    }

    private IEnumerator PlanPlanting(bool shelter, WorkExecutionMethod method = WorkExecutionMethod.LandownerSimulated)
    {
        Check(manager.TryPurchaseStock("sessile-oak-sapling", 1), "FIXTURE_BUY_OAK_STOCK");
        bool placed = false;
        foreach (Vector3 point in PlantingCandidates())
            if (manager.TryDesignateExactPlanting("sessile-oak-sapling", point, method, shelter)) { placed = true; break; }
        Check(placed, "FIXTURE_DESIGNATE_PLANTING");
        yield return null;
    }

    private void AdvanceWithWork(string what)
    {
        bool approved = manager.ApprovePendingWork();
        Check(approved, "FIXTURE_APPROVE_" + what.Replace(' ', '_'), manager.Feedback);
        bool advanced = manager.AdvanceYear();
        Check(advanced, "FIXTURE_ADVANCE_" + what.Replace(' ', '_'), manager.Feedback);
        if (!approved || !advanced) throw new InvalidOperationException("fixture could not complete " + what);
    }

    private ForestTree[] Trees() => UnityEngine.Object.FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
        .Where(tree => tree != null && tree.IsLiving && tree.CanChop && tree.TreeId.StartsWith("P", StringComparison.Ordinal))
        .OrderBy(tree => tree.TreeId, StringComparer.Ordinal).ToArray();

    private IEnumerable<Vector3> PlantingCandidates()
    {
        ForestTree[] trees = Trees();
        foreach (var cell in ecology.Cells.OrderByDescending(c => c.Light).ThenBy(c => c.Center.x).ThenBy(c => c.Center.y))
            foreach (float x in new[] { -1.5f, 0f, 1.5f })
                foreach (float z in new[] { -1.5f, 0f, 1.5f })
                {
                    var point = new Vector3(cell.Center.x + x, 0f, cell.Center.y + z);
                    if (trees.All(tree => Vector2.Distance(new Vector2(tree.transform.position.x, tree.transform.position.z), new Vector2(point.x, point.z)) >= 0.8f))
                        yield return point;
                }
    }

    // ---------------------------------------------------------------- UI reading and driving

    private VisualElement Modal() => ui.RootElement.Q(className: "modal-review");

    private string Title()
    {
        VisualElement modal = Modal();
        Label title = modal == null ? null : modal.Q<Label>(className: "title");
        return title == null ? "" : title.text;
    }

    private Button FindButton(string text) => Modal().Query<Button>().ToList().FirstOrDefault(b => b.text == text);

    private bool IsEnabled(string text) { Button b = FindButton(text); return b != null && b.enabledInHierarchy; }

    private bool FooterVisible()
    {
        Button ack = ui.RootElement.Q<Button>("annual-review-acknowledge");
        return ack != null && Visible(ack);
    }

    private static bool Visible(VisualElement element)
    {
        if (element == null) return false;
        for (VisualElement p = element; p != null; p = p.parent)
            if (p.resolvedStyle.display == DisplayStyle.None || p.resolvedStyle.visibility == Visibility.Hidden) return false;
        return element.resolvedStyle.width > 0.5f && element.resolvedStyle.height > 0.5f;
    }

    private IEnumerator ClickButton(string text)
    {
        Button button = FindButton(text);
        Check(button != null, "BUTTON_PRESENT_" + text);
        if (button != null) yield return ClickButton(button, true);
    }

    private IEnumerator ClickButton(Button button, bool requireEnabled)
    {
        if (requireEnabled) Check(button.enabledInHierarchy, "BUTTON_ENABLED_" + button.text);
        MethodInfo invoke = typeof(Clickable).GetMethod("Invoke", BindingFlags.Instance | BindingFlags.NonPublic);
        if (invoke == null) throw new MissingMethodException("Clickable.Invoke is unavailable in this Unity version");
        invoke.Invoke(button.clickable, new object[] { null });
        yield return Settle(6);
    }

    private IEnumerator RecordState(string label)
    {
        yield return Settle(4);
        VisualElement modal = Modal();
        var sb = new StringBuilder();
        sb.AppendLine("TITLE: " + Title());
        if (modal != null)
            foreach (TextElement element in modal.Query<TextElement>().ToList())
                if (!string.IsNullOrEmpty(element.text) && Visible(element))
                    sb.AppendLine((element is Button ? "[button] " : "") + element.text);
        texts[label] = sb.ToString();
        File.WriteAllText(Path.Combine(output, "review-texts", label + ".txt"), texts[label]);

        bool teaching = ui.Learning != null && ui.Learning.NeedsReviewAcknowledgement;
        bool footer = FooterVisible(), needs = ui.NeedsAnnualReview, completion = manager.NeedsCompletionReviewAcknowledgement;
        bool historical = Title().Contains("earlier year");
        gateTable.AppendLine(string.Join("\t", label, Title(), footer, needs, completion, teaching,
            modal != null && IsEnabled(EarlierText), modal != null && IsEnabled(LaterText), manager.AnnualReviewSeen));
        // The acknowledgement footer follows the existing gate rule: any of the three flags, and only on the newest review.
        bool reviewOpen = ui.CurrentScreen == ScenarioOneUiRoot.UiScreen.Review && manager.AnnualReports.Count > 0;
        if (reviewOpen)
            Check(footer == ((needs || completion || teaching) && !historical), "GATE_RULE_HOLDS_" + label,
                $"footer={footer} needs={needs} completion={completion} teaching={teaching} historical={historical}");
    }

    // ---------------------------------------------------------------- rendering

    private IEnumerator UseSize(Vector2Int target)
    {
        size = target;
        SetGameSize(target.x, target.y);
        for (int frame = 0; frame < 120 && (Screen.width != target.x || Screen.height != target.y); frame++) yield return null;
        yield return Settle(24);
        Check(Screen.width == target.x && Screen.height == target.y, $"TARGET_RENDER_SIZE_{target.x}x{target.y}", $"actual={Screen.width}x{Screen.height}");
    }

    private IEnumerator Shot(string name, bool scrolledToEnd, bool reset = true)
    {
        ScrollView scroll = Modal().Q<ScrollView>();
        if (scroll != null && reset)
        {
            scroll.verticalScroller.value = scrolledToEnd ? scroll.verticalScroller.highValue : scroll.verticalScroller.lowValue;
            yield return Settle(6);
        }
        yield return new WaitForEndOfFrame();
        Texture2D image = ScreenCapture.CaptureScreenshotAsTexture();
        if (image == null) { Check(false, "SHOT_" + name, "no rendered capture"); yield break; }
        Check(image.width == size.x && image.height == size.y, "CAPTURE_SIZE_" + name, $"capture={image.width}x{image.height} target={size.x}x{size.y}");
        Check(!LooksBlank(image), "SHOT_NOT_BLANK_" + name);
        string file = $"{name}-{size.x}x{size.y}.png";
        File.WriteAllBytes(Path.Combine(output, "shots", file), image.EncodeToPNG());
        Destroy(image);
        shots++;
        AuditLayout(name);
        Debug.Log($"S1C_T03_SHOT {file}");
    }

    // Horizontal containment of every visible text element and no overlap between the title and the header buttons.
    private void AuditLayout(string name)
    {
        VisualElement modal = Modal();
        Rect panel = modal.worldBound;
        var overflow = new List<string>();
        foreach (TextElement element in modal.Query<TextElement>().ToList())
        {
            if (string.IsNullOrEmpty(element.text) || !Visible(element)) continue;
            Rect r = element.worldBound;
            if (r.xMin < panel.xMin - 1.5f || r.xMax > panel.xMax + 1.5f) overflow.Add(Short(element.text) + $" [{r.xMin:0}..{r.xMax:0} vs {panel.xMin:0}..{panel.xMax:0}]");
        }
        Check(overflow.Count == 0, $"[{size.x}x{size.y}]_TEXT_INSIDE_PANEL_" + name, string.Join(" | ", overflow.Take(3)));

        foreach (Button toggle in modal.Query<Button>().ToList().Where(b => b.name.StartsWith("work-detail-toggle-") && Visible(b)))
        {
            Vector2 measured = toggle.MeasureTextSize(toggle.text, toggle.contentRect.width, VisualElement.MeasureMode.Exactly,
                0, VisualElement.MeasureMode.Undefined);
            Check(measured.y <= toggle.contentRect.height + 1.5f && toggle.worldBound.xMin >= toggle.parent.worldBound.xMin
                && toggle.worldBound.xMax <= toggle.parent.worldBound.xMax + 1.5f,
                $"[{size.x}x{size.y}]_DETAIL_LABEL_FITS_{name}_{toggle.name}",
                $"textHeight={measured.y:0.0} contentHeight={toggle.contentRect.height:0.0} width={toggle.contentRect.width:0.0}");
        }
        Label title = modal.Q<Label>(className: "title");
        var clashes = new List<string>();
        if (title != null)
        {
            Rect t = Shrink(title.worldBound, 1f);
            foreach (Button b in modal.Query<Button>().ToList())
                if (Visible(b) && b.parent != null && b.parent.ClassListContains("screen-actions") && t.Overlaps(Shrink(b.worldBound, 1f))) clashes.Add(b.text);
        }
        Check(clashes.Count == 0, $"[{size.x}x{size.y}]_TITLE_CLEAR_OF_HEADER_BUTTONS_" + name, string.Join(",", clashes));
    }

    private static Rect Shrink(Rect r, float by) => new Rect(r.x + by, r.y + by, Mathf.Max(0f, r.width - 2f * by), Mathf.Max(0f, r.height - 2f * by));
    private static string Short(string text) => text.Length <= 40 ? text : text.Substring(0, 40) + "…";

    private static bool LooksBlank(Texture2D image)
    {
        Color32[] pixels = image.GetPixels32();
        int step = Mathf.Max(1, pixels.Length / 4000), different = 0;
        Color32 first = pixels[0];
        for (int i = 0; i < pixels.Length; i += step)
            if (Mathf.Abs(pixels[i].r - first.r) + Mathf.Abs(pixels[i].g - first.g) + Mathf.Abs(pixels[i].b - first.b) > 24) different++;
        return different < 40;
    }

    private static IEnumerable<Vector2Int> Sizes()
    {
        string text = Environment.GetEnvironmentVariable("CCF_UI_SIZES");
        var list = new List<Vector2Int>();
        foreach (string part in (string.IsNullOrEmpty(text) ? "1280x720,1920x1080" : text).Split(','))
        {
            string[] xy = part.Trim().Split('x');
            if (xy.Length == 2 && int.TryParse(xy[0], out int w) && int.TryParse(xy[1], out int h)) list.Add(new Vector2Int(w, h));
        }
        if (!list.Contains(new Vector2Int(1280, 720)) || !list.Contains(new Vector2Int(1920, 1080)))
            throw new InvalidOperationException("CCF_UI_SIZES must include 1280x720 and 1920x1080");
        return list;
    }

    // ---------------------------------------------------------------- bookkeeping

    private static IEnumerator Settle(int frames) { for (int i = 0; i < frames; i++) yield return null; }

    private string WorldHash()
    {
        if (saves == null) throw new InvalidOperationException("save controller missing for world hash");
        return ScenarioReferenceArchive.WorldHash(saves.CaptureData());
    }

    private void Check(bool ok, string name, string detail = "")
    {
        if (ok) passes++; else failures++;
        string line = (ok ? "S1C_T03_CHECK_PASS " : "S1C_T03_CHECK_FAIL ") + name + (string.IsNullOrEmpty(detail) ? "" : " " + detail);
        results.Add(line);
        Debug.Log(line);
    }

    private void OnLogMessage(string condition, string stackTrace, LogType type)
    {
        if (type != LogType.Error && type != LogType.Exception && type != LogType.Assert) return;
        if (errors.Count < 40) errors.Add(type + ": " + condition);
    }

    private void WriteReports(bool pass, Exception failure)
    {
        File.WriteAllText(Path.Combine(output, "gate-states.tsv"), gateTable.ToString());
        var sb = new StringBuilder();
        sb.AppendLine("mode: " + mode);
        sb.AppendLine("result: " + (pass ? "PASS" : "FAIL"));
        sb.AppendLine("checks passed: " + passes + "  failed: " + failures + "  shots: " + shots);
        if (failure != null) sb.AppendLine("exception: " + failure);
        sb.AppendLine(); sb.AppendLine("NOTES");
        foreach (string note in notes) sb.AppendLine("- " + note);
        if (errors.Count > 0) { sb.AppendLine(); sb.AppendLine("CONSOLE ERRORS"); foreach (string e in errors) sb.AppendLine("- " + e); }
        sb.AppendLine(); sb.AppendLine("CHECKS");
        foreach (string line in results) sb.AppendLine(line);
        File.WriteAllText(Path.Combine(output, "t03-result.txt"), sb.ToString());
    }
}
#endif
