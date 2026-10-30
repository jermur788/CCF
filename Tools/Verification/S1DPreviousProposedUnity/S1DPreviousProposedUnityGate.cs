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
using Approval = ScenarioOnePrescriptionApproval;

// Disposable Editor-only rendered gate for S1-D T-05 (Work Plan previous versus proposed presentation), claim #151.
// Stage with run_unity_gate.py (gate "s1d-t05"), then remove source and .meta. Verification only: no production edit, no disk save.
//
// The world is built the way a player would (the MFM-07 gate's production path): rack access cycle, sample-release cycle, a real MFM
// prescription approved into ordinary Pending work, then an idle third cycle so that work is genuinely carried over. Only the final
// classification fixtures (an Approved order, a future-year order, a record with no history year) set createdYear/status in memory,
// because the real approval API approves every Pending order at once. Those edits are labelled in the report and nothing is saved.
public static class S1DPreviousProposedUnityGate
{
    private const string ExitKey = "CCF.S1D.T05.PendingExit";

    public static void Begin()
    {
        SessionState.EraseInt(ExitKey);
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        S1DPreviousProposedUnityGateRunner.SetGameSize(1280, 720);
        EditorApplication.isPlaying = true;
    }

    public static void Finish(bool pass)
    {
        // Closing the Editor in the coroutine that requests a Play Mode exit can race domain teardown; exit once back in Edit Mode.
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
        if (!Environment.GetCommandLineArgs().Any(a => a == "S1DPreviousProposedUnityGate.Begin")) return;
        new GameObject("Disposable S1-D T-05 rendered gate").AddComponent<S1DPreviousProposedUnityGateRunner>();
    }
}

public sealed class S1DPreviousProposedUnityGateRunner : MonoBehaviour
{
    private static readonly string[] Entrances = { "PO0203", "PO1204", "PO2003", "PO2803", "PO3803" };
    private static readonly float[] QualifiedZ = { -35.2711754f, -16.4217644f, 0.2382353f, 15.7935295f, 35.2323532f };
    private static readonly string[] SampleCrops = { "P0810", "P1206", "P1214" };

    private string output;
    private Vector2Int size = new Vector2Int(1280, 720);
    private int passes, failures, shots;
    private ScenarioOneUiRoot ui;
    private ScenarioOneManager manager;
    private ForestEcologyController ecology;
    private ForestSaveController saves;
    private ForestTreeMarkingManager marks;
    private ForestSaveData original;
    private bool saveControllerWasEnabled;
    private readonly List<string> errors = new List<string>();
    private readonly List<string> notes = new List<string>();
    private readonly List<string> results = new List<string>();

    private ScenarioOneWorkOrder approvedOrder, carriedMfm, newOrder, futureOrder, missingOrder;
    private List<ScenarioOneWorkOrder> mfmOrders = new List<ScenarioOneWorkOrder>();

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
            null, new object[] { Enum.ToObject(kind, 1), width, height, "S1D T05 " + width + "x" + height }, null);
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
        Directory.CreateDirectory(Path.Combine(output, "shots"));
        Directory.CreateDirectory(Path.Combine(output, "texts"));
        Application.logMessageReceived += OnLogMessage;
        Exception failure = null;
        IEnumerator run = Run();
        while (true)
        {
            bool more; object current = null;
            try { more = run.MoveNext(); if (more) current = run.Current; }
            catch (Exception error) { failure = error; break; }
            if (!more) break;
            yield return current;
        }
        if (original != null)
        {
            try { Check(saves.LoadData(original, false), "ORIGINAL_WORLD_RESTORED_IN_MEMORY"); }
            catch (Exception error) { failure = failure == null ? error : new AggregateException(failure, error); }
            saves.enabled = saveControllerWasEnabled;
        }
        if (errors.Count > 0) { failures++; Debug.Log("S1D_T05_UNITY_CHECK_FAIL NO_CONSOLE_ERRORS " + string.Join(" | ", errors.Take(4))); }
        Application.logMessageReceived -= OnLogMessage;
        bool pass = failure == null && failures == 0;
        WriteReport(pass, failure);
        Debug.Log(pass ? $"S1D_T05_UNITY_PASS checks={passes} shots={shots}" : $"S1D_T05_UNITY_FAIL checks={passes} failures={failures} exception={failure}");
        S1DPreviousProposedUnityGate.Finish(pass);
    }

    // ---------------------------------------------------------------- scenario

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
            if (ui != null && manager != null && ecology != null && saves != null && marks != null) break;
            yield return null;
        } while (Time.realtimeSinceStartup < deadline);
        Fatal(ui != null && manager != null && ecology != null && saves != null && marks != null, "SCENE_COMPONENTS_READY");
        yield return Settle(20);
        string config = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME");
        Fatal(!string.IsNullOrEmpty(config) && Application.persistentDataPath.StartsWith(config.TrimEnd('/') + "/", StringComparison.Ordinal), "ISOLATED_PERSISTENT_PATH", Application.persistentDataPath);
        Check(!File.Exists(Path.Combine(Application.persistentDataPath, "forest-save.json")), "ISOLATED_SAVE_SLOT_STARTS_EMPTY");
        saveControllerWasEnabled = saves.enabled;
        original = saves.CaptureData();
        saves.enabled = false;
        Fatal(manager.RackPlanningV2Active && ecology.StandGeometryModelVersion == StandGeometryModel.Enlarged80, "PRODUCTION_ENLARGED80_RACK_V2_WORLD");
        ui.CloseAll(); ui.CloseHelp();
        yield return Settle(12);

        // ---- cycle 1: rack access, real manager APIs ----
        float half = StandGeometryModel.StandSizeMeters(ecology.StandGeometryModelVersion) * 0.5f;
        for (int i = 0; i < Entrances.Length; i++)
        {
            ForestTree entrance = Tree(Entrances[i]);
            Fatal(entrance != null && Math.Abs(entrance.transform.position.z - QualifiedZ[i]) < 1e-4f, "QUALIFIED_ENTRANCE_PRESENT", Entrances[i]);
            var added = manager.AddRack(new RackPoint(ScenarioOneHeadlandGeometry.HeadlandX, entrance.transform.position.z), new RackPoint(half, entrance.transform.position.z));
            Fatal(added.Succeeded, "RACK_ADDED", added.Message);
        }
        Fatal(manager.ConfirmRackPlan().Succeeded, "RACK_PLAN_CONFIRMED");
        Fatal(manager.ApprovePendingWork(), "CYCLE1_APPROVE", manager.Feedback);
        Fatal(manager.AdvanceYear(), "CYCLE1_ADVANCE", manager.Feedback);
        yield return Settle(12); manager.MarkAnnualReviewSeen();
        foreach (string id in SampleCrops)
        {
            ForestTree crop = Tree(id);
            Fatal(crop != null && crop.IsLiving, "SAMPLE_CROP_ALIVE", id);
            marks.Mark(crop, TreeMarkType.CropTree, false);
        }
        yield return Settle(12);

        // ---- cycle 2: in-footprint release ----
        ScenarioOneRackBay.Analysis bay = Bay();
        Fatal(bay != null && bay.Applies && bay.HasEnoughFuture, "SAMPLE_QUALIFIES");
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
        Fatal(Bay().ReleasePlanned, "RELEASE_PLANNED");
        Fatal(manager.ApprovePendingWork(), "CYCLE2_APPROVE", manager.Feedback);
        Fatal(manager.AdvanceYear(), "CYCLE2_ADVANCE", manager.Feedback);
        yield return Settle(12); manager.MarkAnnualReviewSeen();

        // ---- a real MFM prescription becomes ordinary Pending work ----
        ScenarioOneCoverage.Result coverage = manager.ReadCoverage(out string problem);
        Fatal(coverage != null && coverage.Valid, "COVERAGE_VALID", problem);
        var covered = coverage.Areas.Where(a => !a.IsSource && a.State == ScenarioOneCoverage.State.Covered).Select(a => a.Id).OrderBy(x => x, StringComparer.Ordinal).ToList();
        Fatal(covered.Count > 0, "COVERED_UNITS_EXIST");
        var sampleCrops = LiveStems().Where(s => s.Living && s.Crop).Select(s => s.Id).ToList();
        var suggestions = ScenarioOneCropTreeCandidates.Suggest(coverage, LiveStems(), sampleCrops, null, null, ScenarioOnePrescriptionCalibration.V1);
        Fatal(suggestions.Valid && suggestions.Candidates.Count > 0, "CANDIDATES_SUGGESTED", suggestions.Problem);
        foreach (var candidate in suggestions.Candidates) marks.Mark(Tree(candidate.TreeId), TreeMarkType.CropTree, false);
        yield return Settle(12);
        Fatal(manager.CreatePrescription(covered).Succeeded, "PRESCRIPTION_ACCEPTED");
        Check(!manager.WorkOrders.Any(o => o.IsOpen), "PRESCRIPTION_ACCEPTANCE_CREATES_NO_OPEN_WORK");
        int yearAtPrescription = manager.CurrentEcologicalYear;
        long cashBeforeMfm = manager.CashCents;
        PrescriptionOperationResult approved = manager.ApprovePrescription(1);
        Fatal(approved.Succeeded && approved.OrdersCreated > 0, "PRESCRIPTION_APPROVED", approved.Message);
        mfmOrders = manager.WorkOrders.Where(o => o.prescriptionId == 1).ToList();
        Check(mfmOrders.Count == approved.OrdersCreated && mfmOrders.All(o => o.status == ScenarioWorkStatus.Pending && o.createdYear == yearAtPrescription),
            "MFM_APPROVAL_CREATES_ORDINARY_PENDING_ORDERS", mfmOrders.Count + " orders");
        Check(manager.CashCents == cashBeforeMfm && mfmOrders.All(o => o.IsOpen && o.status != ScenarioWorkStatus.Approved), "MFM_APPROVAL_IS_NOT_WORK_PLAN_APPROVAL");
        foreach (Vector2Int target in Sizes())
        {
            yield return UseSize(target);
            ui.CloseAll(); ui.ShowWorkPlan(); yield return Settle(12);
            string tag = Tag();
            VisualElement card = Card(); Check(card != null, "GROUP_CARD_PRESENT_MFM_PENDING_" + tag);
            if (card == null) continue;
            Check(Text(card).Contains("Pending work is uncommitted until you approve the Work Plan"), "WORK_PLAN_APPROVAL_IS_THE_COMMITMENT_GATE_" + tag);
            SetFoldout("workplan-pending-new", true); yield return Settle(6);
            Check(FoldoutText("workplan-pending-new").Contains("prescription 1 ·"), "MFM_PROVENANCE_UNDER_PENDING_NEW_" + tag);
            Check(Foldout("workplan-approved").text.StartsWith("Approved · 0"), "NOTHING_APPROVED_YET_" + tag, Foldout("workplan-approved").text);
            yield return Shot("mfm-pending-new-year" + yearAtPrescription, true);
            Audit("mfm-pending-new");
            SetFoldout("workplan-pending-new", false);
        }

        // ---- cycle 3: an idle year; the unapproved MFM work is genuinely carried over ----
        Fatal(manager.AdvanceYear(), "CYCLE3_ADVANCE_IDLE", manager.Feedback);
        yield return Settle(12); manager.MarkAnnualReviewSeen();
        int current = manager.CurrentEcologicalYear;
        Check(current == yearAtPrescription + 1 && mfmOrders.All(o => o.IsOpen && o.status == ScenarioWorkStatus.Pending && o.createdYear < current), "MFM_WORK_CARRIED_OVER_UNAPPROVED");
        var historyYears = manager.HistoryYears.Select(y => y.Year).OrderBy(y => y).ToList();
        notes.Add("History years present at year " + current + ": " + string.Join(",", historyYears));
        Check(historyYears.Count >= 3, "THREE_COMPLETED_CYCLES_IN_HISTORY", string.Join(",", historyYears));
        int linkYear = historyYears.First(y => y < current);
        int missingYear = Enumerable.Range(-5, 6).First(y => y < current && !historyYears.Contains(y));

        // ---- final classification fixtures ----
        Check(marks.GetMarkedIds().Count == 0, "NO_FELL_MARK_BEFORE_UNPLANNED_FIXTURE");
        ForestTree[] free = LiveTrees().Where(t => t.IsLiving && t.CanChop && !t.IsCropTree && !manager.IsPlannedForFelling(t.TreeId) && t.TreeId.StartsWith("P", StringComparison.Ordinal))
            .OrderBy(t => t.TreeId, StringComparer.Ordinal).ToArray();
        Fatal(free.Length >= 6, "FREE_TREES_FOR_FIXTURE", free.Length.ToString());
        manager.PlanningFellingOutcome = FellingMaterialOutcome.SellAndExtract;
        // Approved order: created through the real path, approved with the carried MFM orders, then those are returned to Pending.
        // Real approval approves every Pending order at once, so the carried/Approved split is set on the order objects in memory.
        marks.Mark(free[0], TreeMarkType.Fell, false); manager.AddMarkedTreesToWorkPlan();
        approvedOrder = manager.WorkOrders.First(o => o.IsOpen && o.targetTreeId == free[0].TreeId);
        approvedOrder.status = ScenarioWorkStatus.Approved; approvedOrder.createdYear = linkYear;
        marks.Mark(free[1], TreeMarkType.Fell, false); manager.AddMarkedTreesToWorkPlan();
        newOrder = manager.WorkOrders.First(o => o.IsOpen && o.targetTreeId == free[1].TreeId);
        marks.Mark(free[2], TreeMarkType.Fell, false); manager.AddMarkedTreesToWorkPlan();
        futureOrder = manager.WorkOrders.First(o => o.IsOpen && o.targetTreeId == free[2].TreeId); futureOrder.createdYear = current + 4;
        marks.Mark(free[3], TreeMarkType.Fell, false); manager.AddMarkedTreesToWorkPlan();
        missingOrder = manager.WorkOrders.First(o => o.IsOpen && o.targetTreeId == free[3].TreeId); missingOrder.createdYear = missingYear;
        carriedMfm = mfmOrders[0];
        notes.Add($"In-memory fixture edits only: order #{approvedOrder.workOrderId} Approved createdYear {linkYear}; #{futureOrder.workOrderId} createdYear {current + 4}; #{missingOrder.workOrderId} createdYear {missingYear}. Nothing saved.");
        ForestTree unplanned = free[4];
        marks.Mark(unplanned, TreeMarkType.Fell, false);
        // Distinct fifth class: a Fell mark that is not yet on the plan has no order and appears in no group.
        Check(unplanned.IsMarkedForFell && !manager.WorkOrders.Any(o => o.targetTreeId == unplanned.TreeId), "UNPLANNED_FELL_MARK_HAS_NO_ORDER");
        yield return Settle(6);
        // Opening the Work Plan adopts marks as ordinary Pending work (production behaviour); that adoption is the only way a mark joins a group.
        ui.CloseAll(); ui.ShowWorkPlan(); yield return Settle(12); ui.CloseAll(); yield return Settle(4);
        ScenarioOneWorkOrder adopted = manager.WorkOrders.FirstOrDefault(o => o.IsOpen && o.targetTreeId == unplanned.TreeId);
        Check(adopted != null && adopted.status == ScenarioWorkStatus.Pending && adopted.createdYear == current, "UNPLANNED_MARK_BECOMES_ORDINARY_PENDING_NEW_ONLY_WHEN_WORK_PLAN_OPENS");

        // Expected classification computed independently from the recorded fields (spec rule), not from the view's own helper.
        List<ScenarioOneWorkOrder> open = manager.WorkOrders.Where(o => o.IsOpen).ToList();
        int expApproved = open.Count(o => o.status == ScenarioWorkStatus.Approved);
        int expNew = open.Count(o => o.status == ScenarioWorkStatus.Pending && o.createdYear == current);
        int expCarried = open.Count(o => o.status == ScenarioWorkStatus.Pending && o.createdYear < current);
        notes.Add($"Expected groups at year {current}: Approved {expApproved}, Pending new {expNew}, Pending carried over {expCarried}; future-year {open.Count(o => o.createdYear > current)}");
        Check(expApproved == 1 && expNew == 2 && expCarried == mfmOrders.Count + 1, "FIXTURE_HAS_ALL_THREE_GROUPS_PLUS_FUTURE_AND_MISSING");

        string hashBefore = WorldHash(); string scenarioBefore = JsonUtility.ToJson(manager.CaptureSaveData());
        int eventsBefore = manager.ManagementEvents.Count, reportsBefore = manager.AnnualReports.Count, ordersBefore = manager.WorkOrders.Count;
        bool seenBefore = manager.AnnualReviewSeen; long cashBefore = manager.CashCents; int yearBefore = manager.CurrentEcologicalYear;
        string orderFieldsBefore = OrderFields();

        foreach (Vector2Int target in Sizes())
        {
            yield return UseSize(target);
            string tag = Tag();
            ui.CloseAll(); ui.ShowWorkPlan(); yield return Settle(12);
            VisualElement card = Card(); Fatal(card != null, "GROUP_CARD_PRESENT_" + tag);

            // Three explicit groups, collapsed by default, counts faithful to the recorded data.
            var folds = new[] { "workplan-approved", "workplan-pending-new", "workplan-pending-carried-over" };
            Check(folds.All(n => Foldout(n) != null && !Foldout(n).value), "GROUPS_COLLAPSED_BY_DEFAULT_" + tag);
            Check(Foldout("workplan-approved").text == $"Approved · {expApproved}" && Foldout("workplan-pending-new").text == $"Pending new · {expNew}"
                && Foldout("workplan-pending-carried-over").text == $"Pending carried over · {expCarried}", "GROUP_LABELS_AND_COUNTS_" + tag,
                string.Join(" | ", folds.Select(n => Foldout(n).text)));
            Check(card.Query<Foldout>().ToList().Count == 3, "EXACTLY_THREE_GROUPS_" + tag);
            yield return Shot("groups-collapsed", true);
            Audit("groups-collapsed");
            foreach (string n in folds) SetFoldout(n, true);
            yield return Settle(8);

            string approvedText = FoldoutText("workplan-approved"), newText = FoldoutText("workplan-pending-new"), carriedText = FoldoutText("workplan-pending-carried-over");
            Check(approvedText.Contains($"#{approvedOrder.workOrderId} ·") && !newText.Contains($"#{approvedOrder.workOrderId} ·") && !carriedText.Contains($"#{approvedOrder.workOrderId} ·"), "APPROVED_ORDER_ONLY_IN_APPROVED_" + tag);
            Check(newText.Contains($"#{newOrder.workOrderId} ·") && newText.Contains($"created in year {current}") && !approvedText.Contains($"#{newOrder.workOrderId} ·") && !carriedText.Contains($"#{newOrder.workOrderId} ·"), "PENDING_NEW_ONLY_CREATED_THIS_YEAR_" + tag);
            Check(mfmOrders.All(o => carriedText.Contains($"#{o.workOrderId} ·") && carriedText.Contains($"created in year {o.createdYear}")) && carriedText.Contains($"#{missingOrder.workOrderId} ·"), "CARRIED_OVER_HAS_MFM_AND_MISSING_YEAR_ORDERS_" + tag);
            Check(carriedText.Contains("prescription 1 ·"), "MFM_PROVENANCE_STILL_ORDINARY_PENDING_CARRIED_" + tag);
            foreach (ScenarioOneWorkOrder o in new[] { futureOrder })
                Check(!approvedText.Contains($"#{o.workOrderId} ·") && !newText.Contains($"#{o.workOrderId} ·") && !carriedText.Contains($"#{o.workOrderId} ·"), "FUTURE_YEAR_ORDER_IN_NO_GROUP_" + tag);
            Check(Text(card).Contains($"Inconsistent record · order #{futureOrder.workOrderId}: creation year {futureOrder.createdYear} is after current year {current}"), "FUTURE_YEAR_SURFACED_AS_INCONSISTENT_" + tag);
            Check(!newText.Contains($"created in year {futureOrder.createdYear}") && !carriedText.Contains($"created in year {futureOrder.createdYear}"), "FUTURE_YEAR_NOT_LABELLED_NEW_OR_CARRIED_" + tag);
            Check(newText.Contains($"#{adopted.workOrderId} ·") && !approvedText.Contains($"#{adopted.workOrderId} ·") && !carriedText.Contains($"#{adopted.workOrderId} ·"), "ADOPTED_MARK_IS_IN_PENDING_NEW_ONLY_" + tag);

            // Links: only prior creation years that exist in history; labelled creation/origin, never approval or cause.
            var buttons = card.Query<Button>().ToList().Where(b => b.name != null && b.name.StartsWith("workplan-origin-")).ToList();
            var expectedLinks = open.Where(o => o.createdYear < current && historyYears.Contains(o.createdYear)).Select(o => "workplan-origin-" + o.workOrderId).OrderBy(x => x).ToList();
            Check(buttons.Select(b => b.name).OrderBy(x => x).SequenceEqual(expectedLinks), "LINKS_ONLY_FOR_EXISTING_CREATION_YEARS_" + tag, string.Join(",", buttons.Select(b => b.name)) + " vs " + string.Join(",", expectedLinks));
            Check(buttons.All(b => b.text.StartsWith("Creation-year review · Year ") && !b.text.ToLowerInvariant().Contains("approv") && !b.text.ToLowerInvariant().Contains("caus")), "LINK_LABELS_SAY_CREATION_YEAR_NOT_APPROVAL_OR_CAUSE_" + tag, string.Join("|", buttons.Select(b => b.text)));
            Check(!buttons.Any(b => b.name == "workplan-origin-" + newOrder.workOrderId) && !buttons.Any(b => b.name == "workplan-origin-" + missingOrder.workOrderId) && !buttons.Any(b => b.name == "workplan-origin-" + futureOrder.workOrderId), "NO_LINK_FOR_NEW_MISSING_OR_FUTURE_" + tag);
            Check(carriedText.Contains("Creation-year review not recorded; no historical link available."), "MISSING_YEAR_SAYS_NOT_RECORDED_" + tag);
            yield return Shot("groups-expanded", true);
            Audit("groups-expanded");
            yield return ScrollToEnd();
            yield return Shot("groups-expanded-scrolled", false);
            Audit("groups-expanded-scrolled");

            // Navigation to the exact creation year.
            Check(carriedMfm.createdYear < current, "NAV_PRECONDITION_" + tag);
            foreach (ScenarioOneWorkOrder o in open.Where(x => x.createdYear < current && historyYears.Contains(x.createdYear)).GroupBy(x => x.createdYear).Select(g => g.First()))
            {
                ui.CloseAll(); ui.ShowWorkPlan(); yield return Settle(10);
                Button link = Card().Q<Button>("workplan-origin-" + o.workOrderId);
                if (link == null) { Check(false, $"LINK_PRESENT_order{o.workOrderId}_{tag}"); continue; }
                yield return Click(link);
                Check(ui.CurrentScreen == ScenarioOneUiRoot.UiScreen.Review, $"LINK_OPENS_ANNUAL_REVIEW_year{o.createdYear}_{tag}");
                yield return Settle(20);
                Check(ReviewYear() == o.createdYear, $"REVIEW_ARRIVES_ON_EXACT_CREATION_YEAR_{o.createdYear}_{tag}", "title=" + ReviewTitle());
                yield return Settle(30);
                Check(ReviewYear() == o.createdYear && ui.CurrentScreen == ScenarioOneUiRoot.UiScreen.Review, $"YEAR_NOT_RESET_TO_LATEST_AFTER_REFRESH_{o.createdYear}_{tag}", "title=" + ReviewTitle());
                Check(ReviewTitle().Contains("earlier year") || o.createdYear == historyYears.Max(), $"HISTORICAL_TITLE_{o.createdYear}_{tag}", ReviewTitle());
                Check(Visible(ui.RootElement.Q<Button>("annual-review-acknowledge")) == false, $"HISTORICAL_REVIEW_HAS_NO_ACKNOWLEDGE_{o.createdYear}_{tag}");
                AssertReadOnly($"AFTER_NAVIGATION_{o.createdYear}_{tag}", hashBefore, scenarioBefore, eventsBefore, reportsBefore, ordersBefore, seenBefore, cashBefore, yearBefore, orderFieldsBefore);
                string reviewText = Text(Review());
                Check(reviewText.Contains("Work done") || reviewText.Contains("WORK DONE"), $"REVIEW_SHOWS_EXECUTED_WORK_SECTION_{o.createdYear}_{tag}");
                File.WriteAllText(Path.Combine(output, "texts", $"review-year{o.createdYear}-{size.x}x{size.y}.txt"), reviewText);
                yield return Shot($"creation-year-review-year{o.createdYear}", true);
                AuditReview($"creation-year-review-year{o.createdYear}");
            }

            // Missing / future years have no destination and no fallback.
            ui.CloseAll(); ui.ShowWorkPlan(); yield return Settle(10);
            var screenBefore = ui.CurrentScreen;
            Check(!ui.TryShowReviewYear(missingYear), "MISSING_YEAR_REFUSED_" + tag, "year " + missingYear);
            Check(!ui.TryShowReviewYear(current + 4), "FUTURE_YEAR_REFUSED_" + tag);
            Check(!ui.TryShowReviewYear(-1), "NEGATIVE_YEAR_REFUSED_" + tag);
            yield return Settle(10);
            Check(ui.CurrentScreen == screenBefore && screenBefore == ScenarioOneUiRoot.UiScreen.WorkPlan, "REFUSED_YEAR_LEAVES_WORK_PLAN_OPEN_NO_LATEST_FALLBACK_" + tag, ui.CurrentScreen.ToString());
            AssertReadOnly("AFTER_REFUSED_YEARS_" + tag, hashBefore, scenarioBefore, eventsBefore, reportsBefore, ordersBefore, seenBefore, cashBefore, yearBefore, orderFieldsBefore);

            // Ordinary latest-entry route still works and still shows the latest year with its footer rules untouched.
            ui.CloseAll(); ui.ShowReview(); yield return Settle(20);
            Check(ReviewYear() == historyYears.Max(), "ORDINARY_REVIEW_ENTRY_STILL_OPENS_LATEST_" + tag, ReviewTitle());
            ui.CloseAll(); yield return Settle(6);
        }
        notes.Add("AC-5: Executed work is shown only by Annual Review 'Work done' (S1-C history); Work Plan groups hold open work only. An unplanned Fell mark exists as a mark with no order and appears in no group until the Work Plan adopts it.");
        notes.Add("UI callbacks drive the actual UI Toolkit controls; OS-native pointer targeting is not qualified by this gate.");
    }

    // ---------------------------------------------------------------- helpers

    private static ForestTree[] LiveTrees() => FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
    private ForestTree Tree(string id) => LiveTrees().FirstOrDefault(t => t.TreeId == id);

    private static List<ScenarioOneCoverage.Stem> LiveStems()
    {
        var result = new List<ScenarioOneCoverage.Stem>();
        foreach (ForestTree tree in LiveTrees())
        {
            if (tree == null || string.IsNullOrEmpty(tree.TreeId) || (!tree.IsLiving && !tree.IsStump)) continue;
            Vector3 p = tree.transform.position;
            result.Add(new ScenarioOneCoverage.Stem(tree.TreeId, p.x, p.z, tree.Diameter, tree.IsLiving,
                tree.Species != null ? tree.Species.SpeciesId : "", tree.IsLiving && tree.IsCropTree, tree.IsLiving && tree.IsMarkedForFell));
        }
        return result;
    }

    private List<ScenarioOneRackBay.Order> LiveOrders()
    {
        var result = new List<ScenarioOneRackBay.Order>();
        foreach (ScenarioOneWorkOrder order in manager.WorkOrders)
        {
            if (order == null || order.type != ScenarioWorkType.FellTree) continue;
            bool open = order.IsValidOpenFelling, done = order.status == ScenarioWorkStatus.Completed;
            if (!open && !done) continue;
            var disposition = order.fellingOutcome == FellingMaterialOutcome.SellAndExtract ? ScenarioOneRackBay.Disposition.Sold
                : order.fellingOutcome == FellingMaterialOutcome.RetainAsFallenDeadwood ? ScenarioOneRackBay.Disposition.Deadwood : ScenarioOneRackBay.Disposition.Other;
            result.Add(new ScenarioOneRackBay.Order(order.targetTreeId, open, done, disposition, order.resolvedYear));
        }
        return result;
    }

    private ScenarioOneRackSampleGeometry.Result Geometry()
        => ScenarioOneRackSampleGeometry.Derive(manager.ConfirmedRacks, StandGeometryModel.StandSizeMeters(ecology.StandGeometryModelVersion) * 0.5f);
    private ScenarioOneRackBay.Analysis Bay() => ScenarioOneRackBayReader.Read(LiveTrees(), manager.WorkOrders, Geometry());

    private VisualElement Card() => ui.RootElement.Q(name: "workplan-order-groups");
    private Foldout Foldout(string name) => ui.RootElement.Q<Foldout>(name);
    private void SetFoldout(string name, bool open) { Foldout f = Foldout(name); if (f != null) f.value = open; }
    private string FoldoutText(string name) { Foldout f = Foldout(name); return f == null ? "" : Text(f); }
    private VisualElement Review() => ui.RootElement.Q(className: "modal-review");
    private string ReviewTitle() { VisualElement r = Review(); Label t = r == null ? null : r.Q<Label>(className: "title"); return t == null ? "" : t.text; }
    private int ReviewYear()
    {
        string title = ReviewTitle();
        string[] parts = title.Split(new[] { "Year " }, StringSplitOptions.None);
        if (parts.Length < 2) return int.MinValue;
        string digits = new string(parts[1].TakeWhile(char.IsDigit).ToArray());
        return int.TryParse(digits, out int y) ? y : int.MinValue;
    }
    private string Tag() => size.x + "x" + size.y;
    private static string Text(VisualElement element) => string.Join("\n", element.Query<TextElement>().ToList().Where(e => e.resolvedStyle.display != DisplayStyle.None).Select(e => e.text));

    private static bool Visible(VisualElement element)
    {
        if (element == null) return false;
        for (VisualElement p = element; p != null; p = p.parent)
            if (p.resolvedStyle.display == DisplayStyle.None || p.resolvedStyle.visibility == Visibility.Hidden) return false;
        return element.resolvedStyle.width > 0.5f && element.resolvedStyle.height > 0.5f;
    }

    private IEnumerator Click(Button button)
    {
        Check(button.enabledInHierarchy, "BUTTON_ENABLED_" + button.name);
        MethodInfo invoke = typeof(Clickable).GetMethod("Invoke", BindingFlags.Instance | BindingFlags.NonPublic);
        if (invoke == null) throw new MissingMethodException("Clickable.Invoke is unavailable in this Unity version");
        invoke.Invoke(button.clickable, new object[] { null });
        yield return Settle(6);
    }

    private string OrderFields()
        => string.Join(";", manager.WorkOrders.Select(o => o.workOrderId + ":" + o.status + ":" + o.createdYear + ":" + o.prescriptionId));

    private void AssertReadOnly(string phase, string hash, string scenario, int events, int reports, int orders, bool seen, long cash, int year, string fields)
        => Check(WorldHash() == hash && JsonUtility.ToJson(manager.CaptureSaveData()) == scenario && manager.ManagementEvents.Count == events && manager.AnnualReports.Count == reports
            && manager.WorkOrders.Count == orders && manager.AnnualReviewSeen == seen && manager.CashCents == cash && manager.CurrentEcologicalYear == year && OrderFields() == fields,
            "WORLD_SAVE_ORDERS_ACK_UNCHANGED_" + phase);

    private string WorldHash() => ScenarioReferenceArchive.WorldHash(saves.CaptureData());

    // ---------------------------------------------------------------- rendering

    private IEnumerator UseSize(Vector2Int target)
    {
        size = target;
        SetGameSize(target.x, target.y);
        for (int frame = 0; frame < 120 && (Screen.width != target.x || Screen.height != target.y); frame++) yield return null;
        yield return Settle(24);
        Check(Screen.width == target.x && Screen.height == target.y, $"TARGET_RENDER_SIZE_{target.x}x{target.y}", $"actual={Screen.width}x{Screen.height}");
    }

    private ScrollView WorkPlanScroll() => Card() == null ? null : Card().GetFirstAncestorOfType<ScrollView>();

    private IEnumerator ScrollToEnd()
    {
        ScrollView scroll = WorkPlanScroll();
        if (scroll != null) { scroll.verticalScroller.value = scroll.verticalScroller.highValue; yield return Settle(8); }
    }

    private IEnumerator Shot(string name, bool top)
    {
        ScrollView scroll = WorkPlanScroll();
        if (scroll == null) scroll = Review() == null ? null : Review().Q<ScrollView>();
        if (scroll != null && top) { scroll.verticalScroller.value = scroll.verticalScroller.lowValue; yield return Settle(6); }
        yield return new WaitForEndOfFrame();
        Texture2D image = ScreenCapture.CaptureScreenshotAsTexture();
        if (image == null) { Check(false, "SHOT_" + name, "no rendered capture"); yield break; }
        Check(image.width == size.x && image.height == size.y, "CAPTURE_SIZE_" + name, $"capture={image.width}x{image.height} target={size.x}x{size.y}");
        Check(!LooksBlank(image), "SHOT_NOT_BLANK_" + name);
        string file = $"{name}-{size.x}x{size.y}.png";
        File.WriteAllBytes(Path.Combine(output, "shots", file), image.EncodeToPNG());
        Destroy(image);
        shots++;
        Debug.Log("S1D_T05_SHOT " + file);
    }

    private static bool LooksBlank(Texture2D image)
    {
        Color32[] pixels = image.GetPixels32();
        int step = Mathf.Max(1, pixels.Length / 4000), different = 0;
        Color32 first = pixels[0];
        for (int i = 0; i < pixels.Length; i += step)
            if (Mathf.Abs(pixels[i].r - first.r) + Mathf.Abs(pixels[i].g - first.g) + Mathf.Abs(pixels[i].b - first.b) > 24) different++;
        return different < 40;
    }

    // Readability: every visible text element stays inside the scrolling panel horizontally and is not vertically clipped.
    private void Audit(string name)
    {
        VisualElement card = Card();
        ScrollView scroll = WorkPlanScroll();
        if (card == null || scroll == null) { Check(false, "AUDIT_PANEL_" + name); return; }
        AuditElements(card, scroll.contentViewport.worldBound, name);
    }

    private void AuditReview(string name)
    {
        VisualElement review = Review();
        if (review == null) { Check(false, "AUDIT_REVIEW_PANEL_" + name); return; }
        AuditElements(review, review.worldBound, name);
    }

    private void AuditElements(VisualElement scope, Rect panel, string name)
    {
        var overflow = new List<string>(); var clipped = new List<string>(); int count = 0;
        foreach (TextElement element in scope.Query<TextElement>().ToList())
        {
            if (string.IsNullOrEmpty(element.text) || !Visible(element)) continue;
            count++;
            Rect r = element.worldBound;
            if (r.xMin < panel.xMin - 1.5f || r.xMax > panel.xMax + 1.5f) overflow.Add(Short(element.text) + $" [{r.xMin:0}..{r.xMax:0} vs {panel.xMin:0}..{panel.xMax:0}]");
            if (element.contentRect.width > 1f)
            {
                Vector2 measured = element.MeasureTextSize(element.text, element.contentRect.width, VisualElement.MeasureMode.Exactly, 0, VisualElement.MeasureMode.Undefined);
                if (measured.y > element.contentRect.height + 1.5f) clipped.Add(Short(element.text));
            }
        }
        Check(count > 0, $"[{Tag()}]_AUDIT_HAS_TEXT_{name}");
        Check(overflow.Count == 0, $"[{Tag()}]_TEXT_INSIDE_PANEL_{name}", string.Join(" | ", overflow.Take(3)));
        Check(clipped.Count == 0, $"[{Tag()}]_TEXT_NOT_CLIPPED_{name}", string.Join(" | ", clipped.Take(3)));
        foreach (Button b in scope.Query<Button>().ToList().Where(Visible))
            Check(b.worldBound.width >= 24f && b.worldBound.height >= 16f, $"[{Tag()}]_BUTTON_USABLE_{name}_{b.name}", $"{b.worldBound.width:0}x{b.worldBound.height:0}");
    }

    private static string Short(string text) => text.Length <= 40 ? text : text.Substring(0, 40) + "…";

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

    private static IEnumerator Settle(int frames) { for (int i = 0; i < frames; i++) yield return null; }

    // ---------------------------------------------------------------- bookkeeping

    private void Check(bool ok, string name, string detail = "")
    {
        if (ok) passes++; else failures++;
        string line = (ok ? "S1D_T05_UNITY_CHECK_PASS " : "S1D_T05_UNITY_CHECK_FAIL ") + name + (string.IsNullOrEmpty(detail) ? "" : " " + detail);
        results.Add(line);
        Debug.Log(line);
    }

    // A fixture precondition: without it the later checks mean nothing, so stop the run.
    private void Fatal(bool ok, string name, string detail = "")
    {
        Check(ok, name, detail);
        if (!ok) throw new InvalidOperationException("fixture precondition failed: " + name + " " + detail);
    }

    private void OnLogMessage(string condition, string stackTrace, LogType type)
    {
        if (type != LogType.Error && type != LogType.Exception && type != LogType.Assert) return;
        if (errors.Count < 40) errors.Add(type + ": " + condition);
    }

    private void WriteReport(bool pass, Exception failure)
    {
        var sb = new StringBuilder();
        sb.AppendLine("result: " + (pass ? "PASS" : "FAIL"));
        sb.AppendLine("checks passed: " + passes + "  failed: " + failures + "  shots: " + shots);
        if (failure != null) sb.AppendLine("exception: " + failure);
        sb.AppendLine(); sb.AppendLine("NOTES"); foreach (string note in notes) sb.AppendLine("- " + note);
        if (errors.Count > 0) { sb.AppendLine(); sb.AppendLine("CONSOLE ERRORS"); foreach (string e in errors) sb.AppendLine("- " + e); }
        sb.AppendLine(); sb.AppendLine("CHECK LIST"); foreach (string line in results) sb.AppendLine(line);
        File.WriteAllText(Path.Combine(output, "t05-result.txt"), sb.ToString());
    }
}
#endif
