#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using CCF.Forestry.WorkEconomy;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UIElements;

// Disposable Editor-only gate for S1-D T-08 (multi-cycle integrated witness), claim #208. Verification only: no production edit.
// Stage with Tools/Verification/S1DMultiCycleWitness/run_t08_gate.py (gate "witness"), then remove source and .meta.
//
// One production Enlarged80 / Rack Planning V2 forest is managed for many real years through the real UI actions
// (ui.ApproveWorkPlan / ui.AdvanceFromWorkPlan / ui.AcknowledgeAnnualReview), the real manager APIs the marking tools call, queued
// keyboard input for the first guided rack review, and the real ForestSaveController.Save()/Load() disk path (isolated config).
// Nothing is faked: completion comes from the ordinary completion rules (all goals + a reviewed two-cycle sequence). The only
// deterministic setup is the choice of which trees to mark and where to plant.
public static class S1DMultiCycleWitnessGate
{
    private const string ExitKey = "CCF.S1D.T08.PendingExit";

    public static void Begin()
    {
        SessionState.EraseInt(ExitKey);
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        S1DMultiCycleWitnessRunner.SetGameSize(1280, 720);
        EditorApplication.isPlaying = true;
    }

    public static void Finish(bool pass)
    {
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
        if (!Environment.GetCommandLineArgs().Any(a => a == "S1DMultiCycleWitnessGate.Begin")) return;
        new GameObject("Disposable S1-D T-08 witness gate").AddComponent<S1DMultiCycleWitnessRunner>();
    }
}

[Serializable]
public sealed class S1DWitnessRow
{
    public string stage = "";
    public int ecologicalYear;
    public int guideIndex;
    public string guideLesson = "";
    public bool latch;
    public string approved = "", pendingNew = "", pendingCarried = "";
    public string approvedThisCycle = "", executedThisCycle = "";
    public int reviewAckedYear = -1, historyYearViewed = -1;
    public string checkpoint = "", checkpointSha256 = "", fingerprintSha256 = "";
    public long cashBeforeCents, cashAfterCents, minCashCents;
    public string d057 = "";
    public string outcome = "";
    public string laterManagement = "";
}

[Serializable]
public sealed class S1DWitnessTable { public List<S1DWitnessRow> rows = new List<S1DWitnessRow>(); }

public sealed class S1DMultiCycleWitnessRunner : MonoBehaviour
{
    private static readonly string[] Entrances = { "PO0203", "PO1204", "PO2003", "PO2803", "PO3803" };
    private static readonly float[] QualifiedZ = { -35.2711754f, -16.4217644f, 0.2382353f, 15.7935295f, 35.2323532f };
    private static readonly string[] SampleCrops = { "P0810", "P1206", "P1214" };
    private const string Oak = "sessile-oak-sapling", Beech = "beech-sapling";

    private string output;
    private Vector2Int size = new Vector2Int(1280, 720);
    private int passes, failures, shots;
    private ScenarioOneUiRoot ui;
    private ScenarioOneManager manager;
    private ForestEcologyController ecology;
    private ForestSaveController saves;
    private ForestTreeMarkingManager marks;
    private ForestPlayer player;
    private ForestSaveData original;
    private bool saveControllerWasEnabled;
    private long minCash = long.MaxValue;
    private int bestGuideIndex;
    private int lastGuideIndex = -1;
    private bool latchSeen;
    private readonly S1DWitnessTable table = new S1DWitnessTable();
    private readonly List<string> errors = new List<string>(), notes = new List<string>(), results = new List<string>();
    private readonly List<string> checkpointLog = new List<string>();
    private string lastCheckpoint = "", lastCheckpointSha = "";

    // ---------------------------------------------------------------- Editor plumbing (as the accepted T-05 gate)

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
            null, new object[] { Enum.ToObject(kind, 1), width, height, "S1D T08 " + width + "x" + height }, null);
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
        foreach (string sub in new[] { "shots", "texts", "saves" }) Directory.CreateDirectory(Path.Combine(output, sub));
        Application.logMessageReceived += OnLogMessage;
        Exception failure = null;
        // A manual stack so an exception thrown inside any nested step is caught here (Unity would swallow it in a nested coroutine).
        var stack = new Stack<IEnumerator>();
        stack.Push(Run());
        while (stack.Count > 0)
        {
            bool more; object current = null;
            try { more = stack.Peek().MoveNext(); if (more) current = stack.Peek().Current; }
            catch (Exception error) { failure = error; break; }
            if (!more) { stack.Pop(); continue; }
            if (current is IEnumerator nested) stack.Push(nested); else yield return current;
        }
        WriteTable();
        if (original != null)
        {
            try { Check(saves.LoadData(original, false), "ORIGINAL_WORLD_RESTORED_IN_MEMORY"); }
            catch (Exception error) { failure = failure == null ? error : new AggregateException(failure, error); }
            saves.enabled = saveControllerWasEnabled;
        }
        string leftover = Path.Combine(Application.persistentDataPath, "forest-save.json");
        if (errors.Count > 0) { failures++; Debug.Log("S1D_T08_CHECK_FAIL NO_CONSOLE_ERRORS " + string.Join(" | ", errors.Take(4))); }
        Application.logMessageReceived -= OnLogMessage;
        bool pass = failure == null && failures == 0;
        WriteReport(pass, failure);
        Debug.Log(pass ? $"S1D_T08_WITNESS_PASS checks={passes} shots={shots} rows={table.rows.Count}" : $"S1D_T08_WITNESS_FAIL checks={passes} failures={failures} exception={failure}");
        S1DMultiCycleWitnessGate.Finish(pass);
    }

    // ---------------------------------------------------------------- the witness

    private IEnumerator Run()
    {
        yield return Boot();
        yield return Cycle1Access();
        if (StopAfter("cycle1")) yield break;
        yield return Cycle2ReleaseAndLatch();
        if (StopAfter("cycle2")) yield break;
        yield return MfmCarriedAndPlanting();
        if (StopAfter("cycle3")) yield break;
        yield return IdleYearsAndSecondThinning();
        if (StopAfter("idle")) yield break;
        yield return CompletionAndContinuedManagement();
    }

    private bool StopAfter(string stage)
    {
        bool stop = Environment.GetEnvironmentVariable("CCF_T08_STOP_AFTER") == stage;
        if (stop) notes.Add("DEVELOPMENT RUN stopped after " + stage + " (CCF_T08_STOP_AFTER); not a qualification run.");
        return stop;
    }

    // ------------------------------------------------ boot + D-057 definition facts

    private IEnumerator Boot()
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
        Fatal(ui != null && manager != null && ecology != null && saves != null && marks != null && player != null, "SCENE_COMPONENTS_READY");
        yield return Settle(20);
        string config = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME");
        Fatal(!string.IsNullOrEmpty(config) && Application.persistentDataPath.StartsWith(config.TrimEnd('/') + "/", StringComparison.Ordinal), "ISOLATED_PERSISTENT_PATH", Application.persistentDataPath);
        Check(!File.Exists(SavePath), "ISOLATED_SAVE_SLOT_STARTS_EMPTY");
        saveControllerWasEnabled = saves.enabled;
        original = saves.CaptureData();
        saves.enabled = false;   // F5/F9 keyboard polling off; Save()/Load() are called directly, the same methods the keys call
        Fatal(manager.RackPlanningV2Active && ecology.StandGeometryModelVersion == StandGeometryModel.Enlarged80, "PRODUCTION_ENLARGED80_RACK_V2_WORLD");
        Check(ForestSaveData.CurrentVersion == 24 && manager.CurrentEcologicalYear == 0 && manager.Outcome == ScenarioOneOutcome.Active, "FRESH_V24_YEAR0_ACTIVE");
        ui.CloseAll(); ui.CloseHelp();
        yield return Settle(12);

        // D-057 / accepted economy: the definition is read, never written. These are the accepted numbers (decision log D-057, Overview).
        ScenarioOneDefinition def = manager.Definition;
        int g = StandGeometryModel.Enlarged80;
        Check(def.StartingCashCents == 1200000L && manager.CashCents == 1200000L, "D057_STARTING_CASH_EUR_12000", manager.CashCents.ToString());
        Check(def.MinimumHarvestJobCents == 250000L && def.OwnerMinutesPerYear == 2400, "D057_HARVEST_MINIMUM_EUR_2500_AND_2400_OWNER_MINUTES");
        Check(def.MinimumCompletionYear == 25 && def.CenturyReviewYear == 100 && Mathf.Approximately(def.MinimumMeanCanopy, 0.35f) && Mathf.Approximately(def.ReferenceMeanCanopy, 0.65f), "D057_YEAR_25_100_CANOPY_0_35_0_65_UNCHANGED");
        Check(def.EffectiveMinimumRetainedOriginalTrees(g) == 240 && def.EffectiveMinimumRegenerationCells(g) == 12 && Mathf.Abs(def.EffectiveMinimumDeadwoodVolumeM3(g) - 0.08f) < 1e-5f, "D057_ENLARGED80_EFFECTIVE_TARGETS_240_12_0_08");
        Check(def.EffectiveReferenceOriginalTrees(g) == 480 && def.EffectiveReferenceBroadleafPresence(g) == 40 && def.EffectiveReferenceRegenerationCells(g) == 48 && Mathf.Abs(def.EffectiveReferenceDeadwoodVolumeM3(g) - 2.0f) < 1e-5f, "D057_ENLARGED80_CENTURY_TARGETS_480_40_48_2_0");
        Check(def.EffectiveMinimumRetainedOriginalTrees(StandGeometryModel.Legacy40) == 60 && def.EffectiveMinimumRegenerationCells(StandGeometryModel.Legacy40) == 3 && Mathf.Abs(def.EffectiveMinimumDeadwoodVolumeM3(StandGeometryModel.Legacy40) - 0.02f) < 1e-5f, "D057_LEGACY40_BASELINE_TARGETS_60_3_0_02");
        notes.Add("Objective targets at year 0 (live): " + string.Join(", ", manager.Objectives.Select(o => o.objectiveId + "=" + F(o.targetValue))));
        Check(manager.Objectives.Count == 0 || (Mathf.Approximately(manager.Objectives.First(o => o.objectiveId == "retained-canopy").targetValue, 240f)
            && Mathf.Approximately(manager.Objectives.First(o => o.objectiveId == "regeneration").targetValue, 12f)), "LIVE_OBJECTIVE_TARGETS_ARE_THE_EFFECTIVE_ENLARGED80_ONES");
        minCash = manager.CashCents;
        Sample();
    }

    // ------------------------------------------------ cycle 1: rack access (year 0 -> 1)

    private IEnumerator Cycle1Access()
    {
        float half = StandGeometryModel.StandSizeMeters(ecology.StandGeometryModelVersion) * 0.5f;
        for (int i = 0; i < Entrances.Length; i++)
        {
            ForestTree entrance = Tree(Entrances[i]);
            Fatal(entrance != null && Math.Abs(entrance.transform.position.z - QualifiedZ[i]) < 1e-4f, "QUALIFIED_ENTRANCE_PRESENT", Entrances[i]);
            var added = manager.AddRack(new RackPoint(ScenarioOneHeadlandGeometry.HeadlandX, entrance.transform.position.z), new RackPoint(half, entrance.transform.position.z));
            Fatal(added.Succeeded, "RACK_ADDED", added.Message);
        }
        Fatal(manager.ConfirmRackPlan().Succeeded, "RACK_PLAN_CONFIRMED");
        // The Work Plan refuses rack work until the entrances are marked (D-064); mark them through the same manager call the spray marker uses.
        Check(!string.IsNullOrEmpty(ui.RackApprovalProblem) && !ui.ApproveWorkPlan(), "CYCLE1_APPROVAL_REFUSED_UNTIL_ENTRANCES_MARKED", ui.RackApprovalProblem);
        foreach (var rack in manager.RackPlan.ToList()) Fatal(manager.MarkRackEntrance(rack.Id).Succeeded, "RACK_ENTRANCE_MARKED", rack.Id.ToString());
        long cashBefore = manager.CashCents;
        Fatal(ui.ApproveWorkPlan(), "CYCLE1_APPROVE_VIA_UI", manager.Feedback);
        var approvedIds = Ids(manager.WorkOrders.Where(o => o.status == ScenarioWorkStatus.Approved));
        Check(approvedIds.Count == 5 || approvedIds.Count > 0, "CYCLE1_ORDINARY_RACK_WORK_APPROVED", string.Join(",", approvedIds));
        Fatal(ui.AdvanceFromWorkPlan(), "CYCLE1_ADVANCE_VIA_UI", manager.Feedback);
        yield return Settle(12);
        Sample();
        Check(manager.CurrentEcologicalYear == 1 && ui.CurrentScreen == ScenarioOneUiRoot.UiScreen.Review && ReviewYear() == 1, "CYCLE1_REVIEW_OPENS_AT_YEAR_1", ReviewTitle());
        Check(ui.NeedsAnnualReview && !ui.AdvanceFromWorkPlan(), "ADVANCE_WAITS_FOR_REVIEW_ACK_WITHOUT_RESIDUE_LOCK");
        yield return AckLatest("cycle1");
        yield return ObserveHistory("cycle1", 0);
        yield return Checkpoint("cycle1-access-reviewed");
        AddRow("cycle1-rack-access", cashBefore, approvedIds, ExecutedThisYear(), ReviewYear());
    }

    // ------------------------------------------------ cycle 2: release; pre-latch behaviour; first guided review earns the latch

    private IEnumerator Cycle2ReleaseAndLatch()
    {
        foreach (string id in SampleCrops)
        {
            ForestTree crop = Tree(id);
            Fatal(crop != null && crop.IsLiving, "SAMPLE_CROP_ALIVE", id);
            marks.Mark(crop, TreeMarkType.CropTree, false);
        }
        yield return Settle(12);
        ScenarioOneRackBay.Analysis bay = Bay();
        Fatal(bay != null && bay.Applies && bay.HasEnoughFuture, "SAMPLE_QUALIFIES");
        Check(GuideIndex() == 2 && !manager.FirstGuidedRackReviewComplete, "GUIDE_AT_RELEASE_LESSON_BEFORE_LATCH", "index=" + GuideIndex());
        manager.PlanningFellingOutcome = FellingMaterialOutcome.SellAndExtract;
        PlanRelease();
        Fatal(Bay().ReleasePlanned, "RELEASE_PLANNED");
        var open = manager.WorkOrders.Where(o => o.IsOpen && o.type == ScenarioWorkType.FellTree).OrderBy(o => o.workOrderId).ToList();
        Check(manager.SetPendingFellingOutcome(open[0].workOrderId, FellingMaterialOutcome.RetainAsFallenDeadwood), "ONE_RELEASE_ORDER_LEFT_AS_DEADWOOD");
        Check(Bay().MixedPlanned && Bay().CanopyKept, "BOTH_OUTCOMES_PLANNED_WITHIN_CAP");

        // Negative control (accepted pre-latch behaviour, T-03): an over-cap draft is refused by the real UI approve/advance entry points.
        ForestSaveData safePlan = saves.CaptureData();
        { var footprint = Bay(); foreach (var t in LiveTrees().Where(t => t.IsLiving && !t.IsCropTree && footprint.Contains(t.transform.position.x, t.transform.position.z)).ToList()) marks.Mark(t, TreeMarkType.Fell, false); }
        Check(!Bay().CanopyKept && !string.IsNullOrEmpty(ui.RackPlanProblem) && !ui.ApproveWorkPlan() && !ui.AdvanceFromWorkPlan(), "PRE_LATCH_OVER_CAP_DRAFT_REFUSED_BY_APPROVE_AND_ADVANCE");
        Fatal(saves.LoadData(safePlan, false), "RESTORE_VALID_DRAFT_IN_MEMORY"); yield return Settle(20); yield return Settled();
        ui.CloseAll(); ui.CloseHelp(); player = ui.Player; marks = ui.Marking;
        Fatal(Bay().ReleasePlanned && Bay().CanopyKept && string.IsNullOrEmpty(ui.RackPlanProblem), "VALID_DRAFT_RESTORED_AND_NOT_BLOCKED");

        long cashBefore = manager.CashCents;
        Fatal(ui.ApproveWorkPlan(), "CYCLE2_APPROVE_VIA_UI", manager.Feedback);
        var approvedIds = Ids(manager.WorkOrders.Where(o => o.status == ScenarioWorkStatus.Approved));
        Fatal(ui.AdvanceFromWorkPlan(), "CYCLE2_ADVANCE_VIA_UI", manager.Feedback);
        yield return Settle(12); yield return Settled(); Sample();
        Check(manager.CurrentEcologicalYear == 2 && ReviewYear() == 2, "CYCLE2_REVIEW_AT_YEAR_2", ReviewTitle());
        Check(Bay().ReleaseExecuted && Bay().MixedExecuted, "RELEASE_EXECUTED_WITH_SOLD_AND_DEADWOOD");
        // Pre-latch: the guide stays at "response" until the review is genuinely done (T-04P/T-04 accepted behaviour).
        Check(GuideIndex() == 4 && !manager.FirstGuidedRackReviewComplete && !LiveFacts().RackReviewComplete, "PRE_LATCH_GUIDE_AT_RESPONSE_AFTER_RELEASE", "index=" + GuideIndex());
        yield return Checkpoint("cycle2-release-executed-prelatch");
        yield return AckLatest("cycle2");
        Check(LiveFacts().ReleaseReviewed && !manager.FirstGuidedRackReviewComplete && GuideIndex() == 4, "ANNUAL_REVIEW_ALONE_DOES_NOT_EARN_THE_LATCH");
        Check(!LiveFacts().RackReviewComplete && LiveFacts().RevisitedTrees == 0, "GENERIC_ACK_GIVES_NO_TREE_INSPECTION_CREDIT");
        string beforeReads = ScenarioJson();
        for (int i = 0; i < 10; i++) { ui.Learning.Refresh(true); ui.Learning.RackPlanProblem(); ui.Learning.RackPlanAdvisory(); }
        Check(ScenarioJson() == beforeReads && !manager.FirstGuidedRackReviewComplete, "REPEATED_READS_NEVER_EARN_THE_LATCH");
        yield return Checkpoint("cycle2-release-reviewed-prelatch");
        // The historical year-1 review is read-only and the pre-latch guide is unaffected by browsing it.
        yield return ObserveHistory("cycle2", 1);
        Check(GuideIndex() == 4 && !manager.FirstGuidedRackReviewComplete, "BROWSING_HISTORY_DOES_NOT_MOVE_GUIDE_OR_LATCH");

        // The real first guided rack review: inspect the three retained trees and the six stand portions; [J] Retain here.
        yield return ReviewStand();
        Check(LiveFacts().RackReviewComplete && LiveFacts().RevisitedTrees == 3 && LiveFacts().StandPortionsReviewed == 6, "ALL_THREE_INSPECTIONS_AND_SIX_PORTIONS_DECIDED");
        Check(manager.FirstGuidedRackReviewComplete && LiveFacts().FirstGuidedRackReviewComplete && GuideIndex() == 5, "REAL_FINAL_DECISION_WRITES_LATCH_AND_GUIDE_IS_RECRUITMENT", "index=" + GuideIndex());
        latchSeen = true;
        yield return Checkpoint("cycle2-latch-earned");
        Check(manager.FirstGuidedRackReviewComplete && !LiveFacts().RackReviewComplete && GuideIndex() == 5, "RELOAD_KEEPS_LATCH_AND_RECRUITMENT_WITHOUT_SESSION_RECONSTRUCTION");

        // Negative control: the very same world with the latch cleared (a pre-latch save) is still at "response", and reads never write it back.
        ForestSaveData latched = saves.CaptureData();
        ForestSaveData unlatched = JsonUtility.FromJson<ForestSaveData>(JsonUtility.ToJson(latched));
        unlatched.scenarioOne.firstGuidedRackReviewComplete = false;
        Fatal(saves.LoadData(unlatched, false), "UNLATCHED_COPY_OF_SAME_WORLD_LOADS"); yield return Settle(20); yield return Settled();
        ui.CloseAll(); ui.CloseHelp(); player = ui.Player; marks = ui.Marking;
        Check(!manager.FirstGuidedRackReviewComplete && GuideIndex() == 4 && !LiveFacts().RackReviewComplete, "NEGATIVE_CONTROL_UNLATCHED_SAME_WORLD_STAYS_AT_RESPONSE", "index=" + GuideIndex());
        for (int i = 0; i < 25; i++) { ui.Learning.Refresh(true); LiveFacts(); }
        Check(!manager.FirstGuidedRackReviewComplete && GuideIndex() == 4, "NEGATIVE_CONTROL_READS_NEVER_RECONSTRUCT_THE_LATCH");
        Fatal(saves.LoadData(latched, false), "LATCHED_WORLD_RELOADED_AFTER_NEGATIVE_CONTROL"); yield return Settle(20); yield return Settled();
        ui.CloseAll(); ui.CloseHelp(); player = ui.Player; marks = ui.Marking;
        Check(manager.FirstGuidedRackReviewComplete && GuideIndex() == 5, "LATCH_RESTORED_AFTER_NEGATIVE_CONTROL");
        // Post-latch (T-03/T-04): the cap is advisory only and later drift of Crop Trees cannot regress the guide.
        { var footprint = Bay(); foreach (var t in LiveTrees().Where(t => t.IsLiving && !t.IsCropTree && footprint.Contains(t.transform.position.x, t.transform.position.z)).ToList()) marks.Mark(t, TreeMarkType.Fell, false); }
        manager.AddMarkedTreesToWorkPlan();
        Check(!Bay().CanopyKept && string.IsNullOrEmpty(ui.RackPlanProblem) && !string.IsNullOrEmpty(ui.RackPlanAdvisory) && GuideIndex() == 5,
            "POST_LATCH_OVER_CAP_IS_ADVISORY_ONLY_AND_GUIDE_STAYS_AT_RECRUITMENT");
        Fatal(saves.LoadData(latched, false), "RESTORE_LATCHED_WORLD_IN_MEMORY"); yield return Settle(20); yield return Settled();
        ui.CloseAll(); ui.CloseHelp(); player = ui.Player; marks = ui.Marking;
        Check(manager.FirstGuidedRackReviewComplete && GuideIndex() == 5 && !manager.WorkOrders.Any(o => o.IsOpen), "LATCHED_WORLD_RESTORED_WITH_NO_OPEN_WORK");

        // Render the guide after the historical sample completion (objectives screen) and the continuing Annual Review.
        foreach (Vector2Int target in Sizes())
        {
            yield return UseSize(target);
            ui.CloseAll(); ui.ShowObjectives(); yield return Settle(16);
            Check(ui.CurrentScreen == ScenarioOneUiRoot.UiScreen.Objectives && GuideIndex() == 5, "GUIDE_SCREEN_AFTER_SAMPLE_COMPLETION_" + Tag());
            File.WriteAllText(Path.Combine(output, "texts", "objectives-after-latch-" + Tag() + ".txt"), Text(ActiveModal("modal-objectives")));
            yield return Shot("guide-after-sample-completion", "modal-objectives");
            ui.CloseAll(); yield return Settle(6);
        }
        AddRow("cycle2-release-and-latch", cashBefore, approvedIds, ExecutedThisYear(), 1);
    }

    private void PlanRelease()
    {
        ScenarioOneRackBay.Analysis bay;
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
    }

    private IEnumerator ReviewStand()
    {
        ui.CloseAll(); ui.CloseHelp();
        foreach (string id in Bay().Future.Select(t => t.Id).ToArray())
        {
            yield return Inspect(Tree(id));
            yield return Press(Key.J);
        }
        for (int part = 0; part < 6; part++)
        {
            var bay = Bay();
            ForestTree tree = LiveTrees().Where(t => t.IsLiving && t.Species != null && t.Species.SpeciesId == "sitka-spruce" && !t.IsMarkedForFell && !manager.IsPlannedForFelling(t.TreeId)
                && bay.Portion(t.transform.position.x, t.transform.position.z) == part).OrderBy(t => t.TreeId, StringComparer.Ordinal).First();
            yield return Inspect(tree); yield return Press(Key.J);
        }
        if (player.IsInspecting) yield return Press(Key.E);
        yield return Settled();
    }

    // ------------------------------------------------ MFM prescription becomes ordinary Pending, carried one idle year; planting joins; ordinary approval

    private int mfmYear = -1;
    // Orders are re-read from the manager each time: a disk load rebuilds every order object.
    private ScenarioOneWorkOrder[] mfmOrders => manager.WorkOrders.Where(o => o.prescriptionId == 1).ToArray();

    private IEnumerator MfmCarriedAndPlanting()
    {
        // Year 2 (reviewed, latched): a real MFM prescription is accepted, then approved into ordinary Pending work.
        ScenarioOneCoverage.Result coverage = manager.ReadCoverage(out string problem);
        Fatal(coverage != null && coverage.Valid, "COVERAGE_VALID", problem);
        var covered = coverage.Areas.Where(a => !a.IsSource && a.State == ScenarioOneCoverage.State.Covered).Select(a => a.Id).OrderBy(x => x, StringComparer.Ordinal).ToList();
        Fatal(covered.Count > 0, "COVERED_UNITS_EXIST");
        var sampleCrops = LiveStems().Where(s => s.Living && s.Crop).Select(s => s.Id).ToList();
        var suggestions = ScenarioOneCropTreeCandidates.Suggest(coverage, LiveStems(), sampleCrops, null, null, ScenarioOnePrescriptionCalibration.V1);
        Fatal(suggestions.Valid && suggestions.Candidates.Count > 0, "CANDIDATES_SUGGESTED", suggestions.Problem);
        int cropsBefore = LiveTrees().Count(t => t.IsLiving && t.IsCropTree);
        foreach (var candidate in suggestions.Candidates) marks.Mark(Tree(candidate.TreeId), TreeMarkType.CropTree, false);
        yield return Settle(12);
        Check(LiveTrees().Count(t => t.IsLiving && t.IsCropTree) > cropsBefore, "CROP_TREE_COUNT_DRIFTS_UP_AFTER_LATCH");
        Check(manager.FirstGuidedRackReviewComplete && GuideIndex() >= 5, "GUIDE_HOLDS_AFTER_CROP_TREE_COUNT_DRIFT", "index=" + GuideIndex());
        Fatal(manager.CreatePrescription(covered).Succeeded, "PRESCRIPTION_ACCEPTED");
        Check(!manager.WorkOrders.Any(o => o.IsOpen), "PRESCRIPTION_ACCEPTANCE_CREATES_NO_OPEN_WORK");
        mfmYear = manager.CurrentEcologicalYear;
        long cashBeforeMfm = manager.CashCents;
        PrescriptionOperationResult approved = manager.ApprovePrescription(1);
        Fatal(approved.Succeeded && approved.OrdersCreated > 0, "PRESCRIPTION_APPROVED", approved.Message);
        Check(mfmOrders.Length == approved.OrdersCreated && mfmOrders.All(o => o.status == ScenarioWorkStatus.Pending && o.createdYear == mfmYear), "MFM_APPROVAL_CREATES_ORDINARY_PENDING_ORDERS", mfmOrders.Length + " orders");
        Check(manager.CashCents == cashBeforeMfm && mfmOrders.All(o => o.IsOpen && o.status != ScenarioWorkStatus.Approved), "MFM_APPROVAL_IS_NOT_WORK_PLAN_APPROVAL");
        yield return Checkpoint("year2-mfm-pending-new");
        foreach (Vector2Int target in Sizes())
        {
            yield return UseSize(target);
            ui.CloseAll(); ui.ShowWorkPlan(); yield return Settle(12);
            Check(Text(Card()).Contains("Pending work is uncommitted until you approve the Work Plan"), "WORK_PLAN_APPROVAL_IS_THE_COMMITMENT_GATE_" + Tag());
            SetFoldout("workplan-pending-new", true); yield return Settle(6);
            Check(FoldoutText("workplan-pending-new").Contains("prescription 1 ·"), "MFM_PROVENANCE_UNDER_PENDING_NEW_" + Tag());
            Check(Foldout("workplan-approved").text.StartsWith("Approved · 0"), "NOTHING_APPROVED_YET_" + Tag(), Foldout("workplan-approved").text);
            yield return Shot("year2-mfm-pending-new", "modal-workplan");
            SetFoldout("workplan-pending-new", false);
        }
        ui.CloseAll(); yield return Settle(4);

        // An idle year: Work Plan left unapproved. The Pending MFM work is genuinely carried over (created before the current year).
        long cashIdle = manager.CashCents;
        Fatal(ui.AdvanceFromWorkPlan(), "IDLE_YEAR_3_ADVANCE_VIA_UI", manager.Feedback);
        yield return Settle(12); yield return Settled(); Sample();
        int current = manager.CurrentEcologicalYear;
        Check(current == mfmYear + 1 && mfmOrders.All(o => o.IsOpen && o.status == ScenarioWorkStatus.Pending && o.createdYear < current), "MFM_WORK_CARRIED_OVER_UNAPPROVED_AND_UNRESTAMPED");
        Check(manager.CashCents <= cashIdle, "IDLE_YEAR_SPENDS_NOTHING_ON_UNAPPROVED_WORK", manager.CashCents + " vs " + cashIdle);
        yield return AckLatest("idle-year-3");
        yield return ObserveHistory("idle-year-3", mfmYear);
        yield return Checkpoint("year3-carried-mfm-idle");
        AddRow("year3-idle-carried-mfm", cashIdle, new List<int>(), ExecutedThisYear(), ReviewYear());

        // Year 3: plan nursery planting (new Pending) beside the carried MFM work; classification is computed independently of the view.
        long cashBefore = manager.CashCents;
        yield return PlanBroadleafPlanting();
        var open = manager.WorkOrders.Where(o => o.IsOpen).ToList();
        int expApproved = open.Count(o => o.status == ScenarioWorkStatus.Approved);
        int expNew = open.Count(o => o.status == ScenarioWorkStatus.Pending && o.createdYear == current);
        int expCarried = open.Count(o => o.status == ScenarioWorkStatus.Pending && o.createdYear < current);
        Check(expApproved == 0 && expNew > 0 && expCarried == mfmOrders.Length, "YEAR3_HAS_PENDING_NEW_AND_PENDING_CARRIED", $"approved {expApproved} new {expNew} carried {expCarried}");
        var historyYears = manager.HistoryYears.Select(y => y.Year).OrderBy(y => y).ToList();
        Check(historyYears.Contains(mfmYear), "CREATION_YEAR_EXISTS_IN_HISTORY", string.Join(",", historyYears));
        // Opening the Work Plan adopts marks and revalidates open orders (production behaviour). Do that once so the read-only baseline excludes it.
        ui.CloseAll(); ui.ShowWorkPlan(); yield return Settle(12); ui.CloseAll(); yield return Settle(4);
        string hashBefore = WorldHash(); string scenarioBefore = ScenarioJson(); string fieldsBefore = OrderFields();
        int eventsBefore = manager.ManagementEvents.Count, reportsBefore = manager.AnnualReports.Count, ordersBefore = manager.WorkOrders.Count;
        bool seenBefore = manager.AnnualReviewSeen; long cashSnap = manager.CashCents; int yearSnap = manager.CurrentEcologicalYear;
        foreach (Vector2Int target in Sizes())
        {
            yield return UseSize(target);
            ui.CloseAll(); ui.ShowWorkPlan(); yield return Settle(12);
            VisualElement card = Card(); Fatal(card != null, "GROUP_CARD_PRESENT_" + Tag());
            var folds = new[] { "workplan-approved", "workplan-pending-new", "workplan-pending-carried-over" };
            Check(Foldout("workplan-approved").text == $"Approved · {expApproved}" && Foldout("workplan-pending-new").text == $"Pending new · {expNew}"
                && Foldout("workplan-pending-carried-over").text == $"Pending carried over · {expCarried}", "GROUP_LABELS_AND_COUNTS_" + Tag(), string.Join(" | ", folds.Select(n => Foldout(n).text)));
            foreach (string n in folds) SetFoldout(n, true);
            yield return Settle(8);
            string newText = FoldoutText("workplan-pending-new"), carriedText = FoldoutText("workplan-pending-carried-over");
            Check(mfmOrders.All(o => carriedText.Contains($"#{o.workOrderId} ·") && carriedText.Contains($"created in year {o.createdYear}") && !newText.Contains($"#{o.workOrderId} ·")), "CARRIED_GROUP_HOLDS_THE_MFM_ORDERS_WITH_CREATION_YEAR_" + Tag());
            Check(open.Where(o => o.createdYear == current).All(o => newText.Contains($"#{o.workOrderId} ·") && newText.Contains($"created in year {current}") && !carriedText.Contains($"#{o.workOrderId} ·")), "NEW_GROUP_HOLDS_ONLY_THIS_YEAR_ORDERS_" + Tag());
            Check(carriedText.Contains("prescription 1 ·"), "MFM_PROVENANCE_STILL_ORDINARY_PENDING_CARRIED_" + Tag());
            var buttons = card.Query<Button>().ToList().Where(b => b.name != null && b.name.StartsWith("workplan-origin-")).ToList();
            var expectedLinks = open.Where(o => o.createdYear < current && historyYears.Contains(o.createdYear)).Select(o => "workplan-origin-" + o.workOrderId).OrderBy(x => x).ToList();
            Check(buttons.Select(b => b.name).OrderBy(x => x).SequenceEqual(expectedLinks) && expectedLinks.Count == mfmOrders.Length, "LINKS_ONLY_FOR_CARRIED_ORDERS_WITH_EXISTING_CREATION_YEAR_" + Tag(), string.Join(",", buttons.Select(b => b.name)) + " vs " + string.Join(",", expectedLinks));
            Check(buttons.All(b => b.text == $"Creation-year review · Year {mfmYear}" && !b.text.ToLowerInvariant().Contains("approv") && !b.text.ToLowerInvariant().Contains("caus")), "LINK_LABELS_SAY_CREATION_YEAR_NOT_APPROVAL_OR_CAUSE_" + Tag(), string.Join("|", buttons.Select(b => b.text)));
            foreach (string n in folds) SetFoldout(n, false);
            yield return Settle(8);
            yield return Shot("year3-groups-collapsed-counts", "modal-workplan");
            SetFoldout("workplan-pending-carried-over", true); yield return Settle(8);
            yield return Shot("year3-carried-group-expanded", "modal-workplan");
            SetFoldout("workplan-pending-carried-over", false); SetFoldout("workplan-pending-new", true); yield return Settle(8);
            yield return Shot("year3-new-group-expanded", "modal-workplan");
            foreach (string n in folds) SetFoldout(n, true);
            yield return Settle(8);
            // Exact creation-year navigation, read-only, no fallback.
            Button link = Card().Q<Button>("workplan-origin-" + mfmOrders[0].workOrderId);
            Fatal(link != null, "LINK_PRESENT_" + Tag());
            yield return Click(link);
            Check(ui.CurrentScreen == ScenarioOneUiRoot.UiScreen.Review && ReviewYear() == mfmYear, "LINK_ARRIVES_ON_EXACT_CREATION_YEAR_" + Tag(), ReviewTitle());
            yield return Settle(30);
            Check(ReviewYear() == mfmYear && !Visible(ui.RootElement.Q<Button>("annual-review-acknowledge")), "YEAR_STAYS_AND_NO_ACK_ON_HISTORICAL_REVIEW_" + Tag(), ReviewTitle());
            AssertReadOnly("AFTER_CREATION_YEAR_NAVIGATION_" + Tag(), hashBefore, scenarioBefore, eventsBefore, reportsBefore, ordersBefore, seenBefore, cashSnap, yearSnap, fieldsBefore);
            File.WriteAllText(Path.Combine(output, "texts", $"creation-year-review-year{mfmYear}-{Tag()}.txt"), Text(Review()));
            yield return Shot("creation-year-review-year" + mfmYear, "modal-review");
            Audit("creation-year-review-year" + mfmYear, "modal-review");
            ui.CloseAll(); ui.ShowWorkPlan(); yield return Settle(10);
            var screenBefore = ui.CurrentScreen;
            int missing = Enumerable.Range(-5, 6).First(y => y < current && !historyYears.Contains(y));
            Check(!ui.TryShowReviewYear(missing) && !ui.TryShowReviewYear(current + 4) && !ui.TryShowReviewYear(-1), "MISSING_FUTURE_NEGATIVE_YEARS_REFUSED_NO_FALLBACK_" + Tag());
            Check(ui.CurrentScreen == screenBefore && manager.WorkPlanOpen, "REFUSED_YEAR_LEAVES_WORK_PLAN_OPEN_" + Tag());
            AssertReadOnly("AFTER_REFUSED_YEARS_" + Tag(), hashBefore, scenarioBefore, eventsBefore, reportsBefore, ordersBefore, seenBefore, cashSnap, yearSnap, fieldsBefore);
            ui.CloseAll(); yield return Settle(6);
        }
        yield return Checkpoint("year3-carried-mfm-plus-new-planting-pending");
        Check(OrderFields() == fieldsBefore, "CHECKPOINT_RELOAD_DID_NOT_RESTAMP_CREATION_YEARS");

        // Ordinary Work Plan approval is the commitment: it approves carried and new Pending alike and re-stamps nothing.
        var preApproval = manager.WorkOrders.Where(o => o.IsOpen).ToDictionary(o => o.workOrderId, o => o.createdYear);
        ui.CloseAll(); ui.ShowWorkPlan(); yield return Settle(10);
        Fatal(ui.ApproveWorkPlan(), "YEAR3_APPROVE_VIA_UI", manager.Feedback);
        var approvedIds = Ids(manager.WorkOrders.Where(o => o.status == ScenarioWorkStatus.Approved));
        Check(mfmOrders.All(o => o.status == ScenarioWorkStatus.Approved) && manager.WorkOrders.Where(o => o.IsOpen).All(o => o.status == ScenarioWorkStatus.Approved), "WORK_PLAN_APPROVAL_COMMITS_CARRIED_MFM_AND_NEW_WORK");
        Check(manager.WorkOrders.Where(o => o.IsOpen).All(o => preApproval.TryGetValue(o.workOrderId, out int y) && y == o.createdYear), "APPROVAL_INVENTS_NO_APPROVAL_YEAR_AND_KEEPS_CREATION_YEAR");
        foreach (Vector2Int target in Sizes())
        {
            yield return UseSize(target);
            ui.CloseAll(); ui.ShowWorkPlan(); yield return Settle(12);
            Check(Foldout("workplan-approved").text == $"Approved · {approvedIds.Count}" && Foldout("workplan-pending-new").text == "Pending new · 0" && Foldout("workplan-pending-carried-over").text == "Pending carried over · 0", "GROUPS_AFTER_APPROVAL_" + Tag(),
                string.Join(" | ", new[] { "workplan-approved", "workplan-pending-new", "workplan-pending-carried-over" }.Select(n => Foldout(n).text)));
            SetFoldout("workplan-approved", true); yield return Settle(8);
            yield return Shot("year3-approved-work", "modal-workplan");
            Audit("year3-approved-work", "modal-workplan");
        }
        ui.CloseAll(); yield return Settle(4);
        long cashApproved = manager.CashCents;
        yield return Checkpoint("year3-approved-before-advance");
        Fatal(ui.AdvanceFromWorkPlan(), "YEAR3_ADVANCE_EXECUTES_APPROVED_WORK_VIA_UI", manager.Feedback);
        yield return Settle(12); yield return Settled(); Sample();
        var executed = ExecutedThisYear();
        Check(mfmOrders.All(o => o.status == ScenarioWorkStatus.Completed || o.status == ScenarioWorkStatus.Failed) && executed.Count >= mfmOrders.Length, "APPROVED_WORK_RESOLVED_BY_NORMAL_ADVANCE", $"executed {executed.Count}");
        Check(manager.Outcome == ScenarioOneOutcome.Active, "STILL_ACTIVE_AFTER_THIRD_MANAGEMENT_CYCLE");
        yield return AckLatest("cycle3");
        yield return ObserveHistory("cycle3", mfmYear);
        Check(manager.FirstGuidedRackReviewComplete && GuideIndex() >= 5, "GUIDE_MONOTONE_AFTER_MFM_AND_PLANTING_EXECUTION", "index=" + GuideIndex());
        yield return Checkpoint("cycle3-executed-reviewed");
        AddRow("cycle3-mfm-and-planting", cashBefore, approvedIds, executed, ReviewYear());
    }

    private IEnumerator PlanBroadleafPlanting()
    {
        Fatal(manager.TryPurchaseStock(Oak, 8) && manager.TryPurchaseStock(Beech, 8), "NURSERY_STOCK_PURCHASED", manager.Feedback);
        int[] bright = Enumerable.Range(0, ecology.CellCount).OrderByDescending(i => ecology.Cells[i].Light).ThenBy(i => i).Take(8).ToArray();
        int placed = 0;
        for (int k = 0; k < bright.Length; k++) placed += PlantPair(bright[k], k < 4 ? Oak : Beech);
        Fatal(placed >= 8, "PLANTING_ORDERS_DESIGNATED", placed.ToString());
        yield return Settle(6);
    }

    private int PlantPair(int cell, string item)
    {
        Vector2 c = ecology.Cells[cell].Center;
        int placed = 0;
        Vector3? first = null;
        for (int ix = -2; ix <= 2 && placed < 2; ix++)
        for (int iz = -2; iz <= 2 && placed < 2; iz++)
        {
            var p = new Vector3(c.x + ix * 0.9f, 0f, c.y + iz * 0.9f);
            if (ecology.GetCellIndex(p) != cell || (first.HasValue && Vector3.Distance(first.Value, p) < 0.8f)) continue;
            bool owner = placed == 0;
            if (!manager.TryDesignateExactPlanting(item, p, owner ? WorkExecutionMethod.LandownerSimulated : WorkExecutionMethod.Contractor, owner)) continue;
            first = first ?? p;
            placed++;
        }
        return placed;
    }

    // ------------------------------------------------ idle years, second thinning cycle, drift after latch

    private IEnumerator IdleYearsAndSecondThinning()
    {
        // Idle years until year 9, with a mid-idle disk round trip.
        while (manager.CurrentEcologicalYear < 9)
        {
            long before = manager.CashCents;
            Fatal(ui.AdvanceFromWorkPlan(), "IDLE_ADVANCE_YEAR_" + (manager.CurrentEcologicalYear + 1), manager.Feedback);
            yield return Settle(6); Sample();
            yield return AckLatest("idle-" + manager.CurrentEcologicalYear);
            Check(manager.Outcome == ScenarioOneOutcome.Active, "ACTIVE_DURING_IDLE_YEARS_" + manager.CurrentEcologicalYear);
            if (manager.CurrentEcologicalYear == 6) { yield return Checkpoint("idle-year6"); AddRow("idle-year6", before, new List<int>(), ExecutedThisYear(), ReviewYear()); }
        }
        // Cycle 4 (year 9): ordinary second thinning; Crop Trees drift; guide stays monotone.
        long cashBefore = manager.CashCents;
        var crops = LiveTrees().Where(t => t.IsLiving && t.IsCropTree).OrderBy(t => t.TreeId, StringComparer.Ordinal).ToArray();
        Fatal(crops.Length > 0, "CROP_TREES_PRESENT_FOR_DRIFT", crops.Length.ToString());
        marks.Unmark(crops[0], false); yield return Settled();
        Check(manager.FirstGuidedRackReviewComplete && GuideIndex() >= 5, "CROP_TREE_REMOVED_AFTER_LATCH_CANNOT_REGRESS_GUIDE", "index=" + GuideIndex());
        List<ForestTree> thin = LiveTrees().Where(t => t.IsLiving && t.CanChop && !t.IsCropTree && t.TreeId.StartsWith("P", StringComparison.Ordinal) && !manager.IsPlannedForFelling(t.TreeId))
            .OrderBy(t => t.TreeId, StringComparer.Ordinal).Where((t, i) => i % 40 == 0).Take(10).ToList();
        manager.PlanningFellingOutcome = FellingMaterialOutcome.SellAndExtract;
        foreach (var t in thin) marks.Mark(t, TreeMarkType.Fell, false);
        manager.AddMarkedTreesToWorkPlan();
        Fatal(manager.WorkOrders.Count(o => o.IsOpen && o.type == ScenarioWorkType.FellTree) >= thin.Count, "SECOND_THINNING_PLANNED", thin.Count.ToString());
        Check(manager.FirstGuidedRackReviewComplete && GuideIndex() >= 5, "LATER_FELLING_PLAN_CANNOT_REGRESS_GUIDE", "index=" + GuideIndex());
        Fatal(ui.ApproveWorkPlan(), "CYCLE4_APPROVE_VIA_UI", manager.Feedback);
        var approvedIds = Ids(manager.WorkOrders.Where(o => o.status == ScenarioWorkStatus.Approved));
        Fatal(ui.AdvanceFromWorkPlan(), "CYCLE4_ADVANCE_VIA_UI", manager.Feedback);
        yield return Settle(12); yield return Settled(); Sample();
        yield return AckLatest("cycle4");
        yield return ObserveHistory("cycle4", 1);
        Check(manager.FirstGuidedRackReviewComplete && GuideIndex() >= 5, "GUIDE_MONOTONE_AFTER_SECOND_THINNING", "index=" + GuideIndex());
        yield return Checkpoint("cycle4-second-thinning-reviewed");
        AddRow("cycle4-second-thinning", cashBefore, approvedIds, ExecutedThisYear(), ReviewYear());
        // More idle years up to just before the completion horizon, with a checkpoint partway.
        while (manager.CurrentEcologicalYear < 24)
        {
            long before = manager.CashCents;
            Fatal(ui.AdvanceFromWorkPlan(), "IDLE_ADVANCE_YEAR_" + (manager.CurrentEcologicalYear + 1), manager.Feedback);
            yield return Settle(6); Sample();
            yield return AckLatest("idle-" + manager.CurrentEcologicalYear);
            Check(manager.Outcome == ScenarioOneOutcome.Active, "ACTIVE_BEFORE_YEAR_25_" + manager.CurrentEcologicalYear);
            if (manager.CurrentEcologicalYear == 16) { yield return Checkpoint("idle-year16"); AddRow("idle-year16", before, new List<int>(), ExecutedThisYear(), ReviewYear()); }
        }
    }

    // ------------------------------------------------ completion by ordinary rules, then continued management

    private IEnumerator CompletionAndContinuedManagement()
    {
        long cashBefore = manager.CashCents;
        int guard = 0;
        while (manager.Outcome == ScenarioOneOutcome.Active && guard++ < 16)
        {
            Fatal(ui.AdvanceFromWorkPlan(), "ADVANCE_TOWARD_COMPLETION_YEAR_" + (manager.CurrentEcologicalYear + 1), manager.Feedback);
            yield return Settle(6); Sample();
            if (manager.Outcome == ScenarioOneOutcome.Active) yield return AckLatest("pre-completion-" + manager.CurrentEcologicalYear);
        }
        Notes_Objectives("at end of completion loop");
        Fatal(manager.Outcome == ScenarioOneOutcome.Completed, "COMPLETED_THROUGH_ORDINARY_COMPLETION_RULES", "outcome=" + manager.Outcome + " year=" + manager.CurrentEcologicalYear + " " + manager.OutcomeReason);
        int completedYear = manager.OutcomeYear;
        Check(completedYear >= 25 && manager.Objectives.All(o => o.achieved) && ScenarioOneCompletionRules.HasReviewedTwoCycleSequence(manager.ManagementEvents, manager.WorkOrders), "COMPLETION_IS_ALL_GOALS_PLUS_REVIEWED_TWO_CYCLES", "year " + completedYear);
        Check(manager.ManagementEvents.Count(e => e.eventType == ScenarioManagementEventType.ScenarioCompleted) == 1 && manager.CompletionReview != null, "ONE_SCENARIO_COMPLETED_EVENT_AND_SUMMARY");
        Check(!manager.NeedsCompletionReviewAcknowledgement, "NO_RESIDUAL_COMPLETION_ACK_NEEDED_BECAUSE_TWO_CYCLES_WERE_REVIEWED");
        Check(manager.CanEditWorkPlan, "WORK_PLAN_STILL_EDITABLE_AT_COMPLETION");
        yield return Checkpoint("completion-boundary-unacknowledged");
        Check(manager.Outcome == ScenarioOneOutcome.Completed && manager.OutcomeYear == completedYear && manager.FirstGuidedRackReviewComplete && GuideIndex() >= 5, "COMPLETION_AND_GUIDE_SURVIVE_RELOAD", "index=" + GuideIndex());
        // The Annual Review acknowledgement for the completion year goes through the real UI.
        ui.ShowReview(); yield return Settle(12);
        int acksBefore = manager.ManagementEvents.Count(e => e.eventType == ScenarioManagementEventType.AnnualReviewAcknowledged);
        ui.AcknowledgeAnnualReview(); reviewAckedYear = manager.CurrentEcologicalYear; yield return Settle(6);
        int acksAfter = manager.ManagementEvents.Count(e => e.eventType == ScenarioManagementEventType.AnnualReviewAcknowledged);
        ui.AcknowledgeAnnualReview();
        Check(acksAfter == acksBefore + 1 && manager.ManagementEvents.Count(e => e.eventType == ScenarioManagementEventType.AnnualReviewAcknowledged) == acksAfter && manager.AnnualReviewSeen, "COMPLETION_YEAR_ACK_RECORDED_ONCE_AND_IDEMPOTENT", $"{acksBefore}->{acksAfter}");
        foreach (Vector2Int target in Sizes())
        {
            yield return UseSize(target);
            ui.CloseAll(); ui.ShowReview(); yield return Settle(16);
            File.WriteAllText(Path.Combine(output, "texts", "completion-review-" + Tag() + ".txt"), Text(Review()));
            Check(Review().Q("completion-review") != null, "COMPLETION_REVIEW_CARD_SHOWN_" + Tag());
            yield return Shot("completion-year-review", "modal-review");
            Audit("completion-year-review", "modal-review");
            ui.CloseAll(); ui.ShowObjectives(); yield return Settle(16);
            yield return Shot("completion-objectives-guide", "modal-objectives");
            Audit("completion-objectives-guide", "modal-objectives");
            ui.CloseAll(); yield return Settle(4);
        }
        yield return Checkpoint("completion-acknowledged");
        AddRow("completion-year-" + completedYear, cashBefore, new List<int>(), ExecutedThisYear(), ReviewYear());

        // Continued ordinary management after Completed: not a terminal lockout.
        long cashPost = manager.CashCents;
        List<ForestTree> more = LiveTrees().Where(t => t.IsLiving && t.CanChop && !t.IsCropTree && t.TreeId.StartsWith("P", StringComparison.Ordinal) && !manager.IsPlannedForFelling(t.TreeId))
            .OrderBy(t => t.TreeId, StringComparer.Ordinal).Where((t, i) => i % 55 == 0).Take(6).ToList();
        foreach (var t in more) marks.Mark(t, TreeMarkType.Fell, false);
        manager.AddMarkedTreesToWorkPlan();
        Check(manager.Outcome == ScenarioOneOutcome.Completed && manager.CanEditWorkPlan && manager.WorkOrders.Count(o => o.IsOpen) >= more.Count, "POST_COMPLETION_WORK_CAN_STILL_BE_PLANNED");
        foreach (Vector2Int target in Sizes())
        {
            yield return UseSize(target);
            ui.CloseAll(); ui.ShowWorkPlan(); yield return Settle(12);
            SetFoldout("workplan-pending-new", true); yield return Settle(6);
            yield return Shot("post-completion-pending-work", "modal-workplan");
            Audit("post-completion-pending-work", "modal-workplan");
        }
        ui.CloseAll(); ui.ShowWorkPlan(); yield return Settle(8);
        Fatal(ui.ApproveWorkPlan(), "POST_COMPLETION_APPROVE_VIA_UI", manager.Feedback);
        var approvedIds = Ids(manager.WorkOrders.Where(o => o.status == ScenarioWorkStatus.Approved));
        ui.CloseAll();
        Fatal(ui.AdvanceFromWorkPlan(), "POST_COMPLETION_ADVANCE_VIA_UI", manager.Feedback);
        yield return Settle(12); yield return Settled(); Sample();
        Check(manager.Outcome == ScenarioOneOutcome.Completed && manager.OutcomeYear == completedYear && manager.CurrentEcologicalYear == completedYear + 1, "COMPLETED_STAYS_COMPLETED_AFTER_FURTHER_MANAGEMENT");
        Check(ExecutedThisYear().Count > 0 && manager.ManagementEvents.Count(e => e.eventType == ScenarioManagementEventType.ScenarioCompleted) == 1, "POST_COMPLETION_WORK_EXECUTED_AND_NO_SECOND_COMPLETION_EVENT");
        yield return AckLatest("post-completion");
        yield return ObserveHistory("post-completion", completedYear);
        Check(manager.FirstGuidedRackReviewComplete && GuideIndex() >= 5, "GUIDE_MONOTONE_AFTER_COMPLETION_AND_CONTINUED_MANAGEMENT", "index=" + GuideIndex());
        yield return Checkpoint("post-completion-managed-reviewed");
        foreach (Vector2Int target in Sizes())
        {
            yield return UseSize(target);
            ui.CloseAll(); ui.ShowReview(); yield return Settle(16);
            yield return Shot("post-completion-year-review", "modal-review");
            Audit("post-completion-year-review", "modal-review");
            string hist = ScenarioJson();
            Check(ui.TryShowReviewYear(completedYear), "COMPLETION_YEAR_OPENS_AS_HISTORY_" + Tag());
            yield return Settle(20);
            Check(ReviewYear() == completedYear && !Visible(ui.RootElement.Q<Button>("annual-review-acknowledge")) && ScenarioJson() == hist, "COMPLETION_YEAR_HISTORY_EXACT_READ_ONLY_" + Tag(), ReviewTitle());
            yield return Shot("post-completion-history-year" + completedYear, "modal-review");
            ui.CloseAll(); yield return Settle(4);
        }
        AddRow("post-completion-year-" + manager.CurrentEcologicalYear, cashPost, approvedIds, ExecutedThisYear(), ReviewYear());
        Check(manager.CanEditWorkPlan && manager.Outcome == ScenarioOneOutcome.Completed, "FURTHER_MANAGEMENT_REMAINS_POSSIBLE_UNTIL_HORIZON");
        // Cycle/solvency summary.
        Check(minCash > 0 && manager.AnnualReports.All(r => r.closingCashCents > 0), "D057_SOLVENT_THROUGHOUT_NO_FAILURE_AND_POSITIVE_CASH", "min " + minCash);
        notes.Add("Minimum observed cash: " + manager.FormatMoneyText(minCash) + "; closing cash " + manager.FormatMoneyText(manager.CashCents));
        // Latch/guide monotone across the whole table.
        int prev = -1; bool monotone = true;
        foreach (var row in table.rows) { if (row.guideIndex < prev) monotone = false; prev = Math.Max(prev, row.guideIndex); }
        Check(monotone, "GUIDE_INDEX_NON_DECREASING_ACROSS_ALL_ROWS", string.Join(",", table.rows.Select(r => r.guideIndex)));
    }

    // ---------------------------------------------------------------- checkpoints (real disk save/load)

    private string SavePath => Path.Combine(Application.persistentDataPath, "forest-save.json");

    private IEnumerator Checkpoint(string label)
    {
        ui.CloseAll(); ui.CloseHelp(); yield return Settle(4);
        string before = Fingerprint();
        int guideBefore = GuideIndex();
        bool latchBefore = manager.FirstGuidedRackReviewComplete;
        saves.Save();
        Fatal(File.Exists(SavePath), "CHECKPOINT_FILE_WRITTEN_" + label);
        byte[] bytes = File.ReadAllBytes(SavePath);
        string sha = Sha(bytes);
        File.Copy(SavePath, Path.Combine(output, "saves", label + ".json"), true);
        Check(Fingerprint() == before, "SAVE_DID_NOT_MUTATE_GAME_STATE_" + label);
        Fatal(saves.CanLoad(out string problem), "CHECKPOINT_LOADABLE_" + label, problem);
        Check(Fingerprint() == before, "CAN_LOAD_PROBE_IS_READ_ONLY_" + label);
        saves.Load();
        yield return Settle(24);
        ui.CloseAll(); ui.CloseHelp(); player = ui.Player; marks = ui.Marking;
        ui.RackLayout.Update(true); ui.Learning.ReloadProgress();
        for (int r = 0; r < 6; r++) ui.Learning.Refresh(true);
        yield return Settled();
        string after = Fingerprint();
        Check(after == before, "DISK_LOAD_RESTORES_CHRONOLOGY_ORDERS_HISTORY_LATCH_EXACTLY_" + label, FirstDifference(before, after));
        Check(manager.FirstGuidedRackReviewComplete == latchBefore, "LATCH_UNCHANGED_BY_LOAD_" + label, latchBefore.ToString());
        Check(GuideIndex() == guideBefore, "GUIDE_INDEX_UNCHANGED_BY_LOAD_" + label, $"{guideBefore} -> {GuideIndex()}");
        Check(!LiveFacts().RackReviewComplete, "LOAD_DOES_NOT_RECONSTRUCT_SESSION_REVIEW_FACTS_" + label);
        for (int i = 0; i < 10; i++) { ui.Learning.Refresh(true); LiveFacts(); }
        Check(Fingerprint() == before, "TEN_REFRESHES_AFTER_LOAD_WRITE_NOTHING_" + label);
        saves.Save();
        string sha2 = Sha(File.ReadAllBytes(SavePath));
        Check(sha2 == sha, "RESAVE_AFTER_LOAD_IS_BYTE_IDENTICAL_" + label, sha.Substring(0, 12) + " vs " + sha2.Substring(0, 12));
        File.Delete(SavePath);   // keep the isolated slot empty between checkpoints; the copy lives in evidence
        lastCheckpoint = label; lastCheckpointSha = sha;
        checkpointLog.Add(label + " year=" + manager.CurrentEcologicalYear + " sha256=" + sha + " fingerprint=" + Sha(Encoding.UTF8.GetBytes(before)));
        Debug.Log("S1D_T08_CHECKPOINT " + label + " sha256=" + sha);
    }

    private string Fingerprint()
    {
        var sb = new StringBuilder();
        sb.Append("year=").Append(manager.CurrentEcologicalYear).Append(";cash=").Append(manager.CashCents).Append(";outcome=").Append(manager.Outcome).Append('@').Append(manager.OutcomeYear)
          .Append(";seen=").Append(manager.AnnualReviewSeen).Append(";latch=").Append(manager.FirstGuidedRackReviewComplete).Append('\n');
        sb.Append("events=").Append(string.Join(",", manager.ManagementEvents.Select(e => $"{e.eventId}:{e.eventType}:{e.year}:{e.outcome}:{e.workOrderId}"))).Append('\n');
        sb.Append("orders=").Append(OrderFields()).Append('\n');
        sb.Append("history=").Append(string.Join(",", manager.HistoryYears.Select(y => y.Year))).Append('\n');
        ScenarioCycleState cs = manager.CycleState;
        sb.Append("cycle=").Append(cs.ResolvedInterventionCount).Append('/').Append(cs.LatestInterventionYear).Append('/').Append(cs.LatestInterventionReview).Append('/')
          .Append(string.Join("|", cs.Components.Select(c => c.Id + ":" + c.Present + ":" + c.Count + ":" + string.Join("+", c.OrderIds)))).Append('\n');
        sb.Append("objectives=").Append(string.Join(",", manager.Objectives.Select(o => o.objectiveId + ":" + F(o.currentValue) + "/" + F(o.targetValue) + ":" + o.achieved))).Append('\n');
        sb.Append("scenario=").Append(Sha(Encoding.UTF8.GetBytes(ScenarioJson()))).Append(";world=").Append(WorldHash());
        return sb.ToString();
    }

    private static string FirstDifference(string a, string b)
    {
        if (a == b) return "";
        string[] la = a.Split('\n'), lb = b.Split('\n');
        for (int i = 0; i < Math.Min(la.Length, lb.Length); i++)
            if (la[i] != lb[i]) return "line " + i + ": [" + Short(la[i], 160) + "] vs [" + Short(lb[i], 160) + "]";
        return "length";
    }

    private static string Short(string t, int n) => t.Length <= n ? t : t.Substring(0, n) + "…";
    private static string Sha(byte[] data) { using (var h = SHA256.Create()) return string.Concat(h.ComputeHash(data).Select(b => b.ToString("x2"))); }
    private string ScenarioJson() => JsonUtility.ToJson(manager.CaptureSaveData());
    private string WorldHash() => ScenarioReferenceArchive.WorldHash(saves.CaptureData());
    private string OrderFields() => string.Join(";", manager.WorkOrders.Select(o => o.workOrderId + ":" + o.status + ":" + o.createdYear + ":" + o.resolvedYear + ":" + o.prescriptionId));

    // ---------------------------------------------------------------- Annual Review / history through the accepted S1-C surface

    private IEnumerator AckLatest(string label)
    {
        if (ui.CurrentScreen != ScenarioOneUiRoot.UiScreen.Review) { ui.ShowReview(); yield return Settle(10); }
        Check(ReviewYear() == manager.CurrentEcologicalYear, "LATEST_REVIEW_SHOWN_FOR_ACK_" + label, ReviewTitle());
        int year = manager.CurrentEcologicalYear;
        int before = manager.ManagementEvents.Count(e => e.eventType == ScenarioManagementEventType.AnnualReviewAcknowledged && e.year == year);
        ui.AcknowledgeAnnualReview();
        reviewAckedYear = year;
        yield return Settle(4);
        int after = manager.ManagementEvents.Count(e => e.eventType == ScenarioManagementEventType.AnnualReviewAcknowledged && e.year == year);
        Check(manager.AnnualReviewSeen && after == before + 1, "ACK_RECORDS_EXACTLY_ONE_EVENT_" + label, $"{before}->{after}");
        ui.AcknowledgeAnnualReview();
        Check(manager.ManagementEvents.Count(e => e.eventType == ScenarioManagementEventType.AnnualReviewAcknowledged && e.year == year) == after, "SECOND_ACK_IS_IDEMPOTENT_" + label);
        Check(!ui.NeedsAnnualReview, "ADVANCE_NO_LONGER_BLOCKED_AFTER_ACK_" + label);
        ui.CloseAll(); yield return Settle(4);
    }

    // View the creation/earlier year (and step to the neighbouring years) through TryShowReviewYear and the Earlier/Later buttons; nothing may change.
    private IEnumerator ObserveHistory(string label, int wantedYear)
    {
        var years = manager.HistoryYears.Select(y => y.Year).OrderBy(y => y).ToList();
        if (!years.Contains(wantedYear)) wantedYear = years.First();
        string hash = WorldHash(), scenario = ScenarioJson(), fields = OrderFields();
        int events = manager.ManagementEvents.Count, reports = manager.AnnualReports.Count, orders = manager.WorkOrders.Count; bool seen = manager.AnnualReviewSeen; long cash = manager.CashCents; int year = manager.CurrentEcologicalYear;
        Check(ui.TryShowReviewYear(wantedYear), "HISTORY_YEAR_OPENS_" + label + "_" + wantedYear);
        yield return Settle(20);
        Check(ReviewYear() == wantedYear, "HISTORY_YEAR_IS_EXACT_" + label, ReviewTitle());
        if (wantedYear < manager.CurrentEcologicalYear) Check(!Visible(ui.RootElement.Q<Button>("annual-review-acknowledge")), "HISTORICAL_REVIEW_HAS_NO_ACK_" + label);
        File.WriteAllText(Path.Combine(output, "texts", $"history-{label}-year{wantedYear}.txt"), Text(Review()));
        Button later = Review().Query<Button>().ToList().FirstOrDefault(b => b.text.StartsWith("Later year"));
        Button earlier = Review().Query<Button>().ToList().FirstOrDefault(b => b.text.StartsWith("◀ Earlier year"));
        if (earlier != null && Visible(earlier) && earlier.enabledInHierarchy) { int y = ReviewYear(); yield return Click(earlier); Check(ReviewYear() < y, "EARLIER_YEAR_STEPS_BACK_" + label, ReviewTitle()); }
        later = Review().Query<Button>().ToList().FirstOrDefault(b => b.text.StartsWith("Later year"));
        if (later != null && Visible(later) && later.enabledInHierarchy) { int y = ReviewYear(); yield return Click(later); Check(ReviewYear() > y, "LATER_YEAR_STEPS_FORWARD_" + label, ReviewTitle()); }
        AssertReadOnly("HISTORY_BROWSING_" + label, hash, scenario, events, reports, orders, seen, cash, year, fields);
        historyYearViewed = wantedYear;
        ui.CloseAll(); yield return Settle(4);
    }

    private int reviewAckedYear = -1, historyYearViewed = -1;

    private void AssertReadOnly(string phase, string hash, string scenario, int events, int reports, int orders, bool seen, long cash, int year, string fields)
        => Check(WorldHash() == hash && ScenarioJson() == scenario && manager.ManagementEvents.Count == events && manager.AnnualReports.Count == reports
            && manager.WorkOrders.Count == orders && manager.AnnualReviewSeen == seen && manager.CashCents == cash && manager.CurrentEcologicalYear == year && OrderFields() == fields,
            "WORLD_SAVE_ORDERS_ACK_UNCHANGED_" + phase);

    // ---------------------------------------------------------------- table rows

    private void AddRow(string stage, long cashBefore, IEnumerable<int> approvedThisCycle, IEnumerable<int> executedThisCycle, int reviewViewed)
    {
        var open = manager.WorkOrders.Where(o => o.IsOpen).ToList();
        int current = manager.CurrentEcologicalYear;
        int guide = GuideIndex();
        latchSeen |= manager.FirstGuidedRackReviewComplete;
        var row = new S1DWitnessRow
        {
            stage = stage, ecologicalYear = current, guideIndex = guide, guideLesson = ScenarioOneTeachingSequence.Lessons[guide].Id, latch = manager.FirstGuidedRackReviewComplete,
            approved = Compact(Ids(open.Where(o => o.status == ScenarioWorkStatus.Approved))),
            pendingNew = Compact(Ids(open.Where(o => o.status == ScenarioWorkStatus.Pending && o.createdYear == current))),
            pendingCarried = Compact(Ids(open.Where(o => o.status == ScenarioWorkStatus.Pending && o.createdYear < current))),
            approvedThisCycle = Compact(approvedThisCycle), executedThisCycle = Compact(executedThisCycle),
            reviewAckedYear = reviewAckedYear, historyYearViewed = historyYearViewed,
            checkpoint = lastCheckpoint, checkpointSha256 = lastCheckpointSha, fingerprintSha256 = Sha(Encoding.UTF8.GetBytes(Fingerprint())),
            cashBeforeCents = cashBefore, cashAfterCents = manager.CashCents, minCashCents = minCash, d057 = D057(), outcome = manager.Outcome + (manager.Outcome == ScenarioOneOutcome.Active ? "" : "@" + manager.OutcomeYear),
            laterManagement = "CanEditWorkPlan=" + manager.CanEditWorkPlan + ";outcome!=Failed=" + (manager.Outcome != ScenarioOneOutcome.Failed) + ";year<" + manager.CenturyReviewYear
        };
        table.rows.Add(row);
        Check(guide >= lastGuideIndex, "GUIDE_NEVER_BELOW_PREVIOUS_ROW_" + stage, $"{lastGuideIndex} -> {guide}");
        if (latchSeen) Check(guide >= 5, "GUIDE_AT_OR_ABOVE_RECRUITMENT_ONCE_LATCHED_" + stage, "index " + guide);
        lastGuideIndex = Math.Max(lastGuideIndex, guide);
        Debug.Log("S1D_T08_ROW " + JsonUtility.ToJson(row));
    }

    private string D057() => $"cash={manager.CashCents};min={minCash};outcome={manager.Outcome};" + string.Join(",", manager.Objectives.Select(o => o.objectiveId + ":" + F(o.currentValue) + "/" + F(o.targetValue) + (o.achieved ? "+" : "-")));

    private void Notes_Objectives(string where) => notes.Add("Objectives " + where + " (year " + manager.CurrentEcologicalYear + "): " + D057());

    private void Sample()
    {
        minCash = Math.Min(minCash, manager.CashCents);
        if (manager.AnnualReports.Count > 0) minCash = Math.Min(minCash, manager.AnnualReports.Min(r => r.closingCashCents));
    }

    // "n=18 [150-167]" for a run of consecutive ids, otherwise the ids themselves.
    private static string Compact(IEnumerable<int> ids)
    {
        var list = ids.OrderBy(x => x).ToList();
        if (list.Count == 0) return "-";
        bool run = list.Count > 3 && list.Zip(list.Skip(1), (a, b) => b - a).All(d => d == 1);
        return run ? $"n={list.Count} [{list[0]}-{list[list.Count - 1]}]" : (list.Count > 8 ? $"n={list.Count} [{string.Join(",", list.Take(6))},…]" : string.Join(",", list));
    }

    private List<int> ExecutedThisYear() => manager.WorkOrders.Where(o => o.resolvedYear == manager.CurrentEcologicalYear && (o.status == ScenarioWorkStatus.Completed || o.status == ScenarioWorkStatus.Failed)).Select(o => o.workOrderId).OrderBy(x => x).ToList();
    private static List<int> Ids(IEnumerable<ScenarioOneWorkOrder> orders) => orders.Select(o => o.workOrderId).OrderBy(x => x).ToList();

    private void WriteTable()
    {
        File.WriteAllText(Path.Combine(output, "cycle-table.json"), JsonUtility.ToJson(table, true));
        File.WriteAllLines(Path.Combine(output, "checkpoints.txt"), checkpointLog);
        var md = new StringBuilder();
        md.AppendLine("| # | stage | year | guide (index) | latch | approved | pending new | pending carried | approved this cycle | executed this cycle | review acked / history viewed | checkpoint | cash before → after (¢) | min cash (¢) | outcome |");
        md.AppendLine("|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|");
        int i = 1;
        foreach (var r in table.rows)
            md.AppendLine($"| {i++} | {r.stage} | {r.ecologicalYear} | {r.guideLesson} ({r.guideIndex}) | {r.latch} | {r.approved} | {r.pendingNew} | {r.pendingCarried} | {r.approvedThisCycle} | {r.executedThisCycle} | {r.reviewAckedYear + "/" + r.historyYearViewed} | {r.checkpoint} `{(r.checkpointSha256.Length > 12 ? r.checkpointSha256.Substring(0, 12) : "")}` | {r.cashBeforeCents} → {r.cashAfterCents} | {r.minCashCents} | {r.outcome} |");
        File.WriteAllText(Path.Combine(output, "cycle-table.md"), md.ToString());
        File.WriteAllLines(Path.Combine(output, "d057-cash-and-objectives.txt"), table.rows.Select(r =>
            $"{r.stage} | year {r.ecologicalYear} | cash {r.cashBeforeCents / 100.0:0.00} -> {r.cashAfterCents / 100.0:0.00} EUR | min {r.minCashCents / 100.0:0.00} EUR | {r.d057}"));
    }

    // ---------------------------------------------------------------- guide / facts / scene helpers

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

    private ScenarioOneRackSampleGeometry.Result Geometry()
        => ScenarioOneRackSampleGeometry.Derive(manager.ConfirmedRacks, StandGeometryModel.StandSizeMeters(ecology.StandGeometryModelVersion) * 0.5f);
    private ScenarioOneRackBay.Analysis Bay() => ScenarioOneRackBayReader.Read(LiveTrees(), manager.WorkOrders, Geometry());

    private ScenarioOneTeachingSequence.Facts LiveFacts()
    {
        MethodInfo method = typeof(LearningObjectivesView).GetMethod("TeachingFacts", BindingFlags.Instance | BindingFlags.NonPublic);
        return (ScenarioOneTeachingSequence.Facts)method.Invoke(ui.Learning, null);
    }

    private int GuideIndex() { ui.Learning.Refresh(true); return ui.Learning.CurrentTopicIndex; }

    private IEnumerator Press(Key key)
    {
        Keyboard keyboard = Keyboard.current ?? InputSystem.AddDevice<Keyboard>();
        InputSystem.QueueStateEvent(keyboard, new KeyboardState(key)); yield return null; yield return null;
        InputSystem.QueueStateEvent(keyboard, new KeyboardState()); yield return null; yield return null;
    }

    private static readonly Vector3[] InspectionDirections =
    {
        Vector3.back, Vector3.forward, Vector3.right, Vector3.left,
        new Vector3(-1f, 0f, -1f).normalized, new Vector3(1f, 0f, -1f).normalized,
        new Vector3(-1f, 0f, 1f).normalized, new Vector3(1f, 0f, 1f).normalized
    };
    private static readonly float[] InspectionDistances = { 3f, 2.5f, 2f };
    private readonly Collider[] inspectionOverlaps = new Collider[16];

    private bool OverlapsOtherTree(Vector3 standPoint, ForestTree target)
    {
        int count = Physics.OverlapSphereNonAlloc(standPoint + Vector3.up * 0.9f, 0.35f, inspectionOverlaps);
        for (int i = 0; i < count; i++)
        {
            Transform hit = inspectionOverlaps[i].transform;
            if (hit == player.transform || hit.IsChildOf(player.transform)) continue;
            ForestTree tree = inspectionOverlaps[i].GetComponentInParent<ForestTree>();
            if (tree == target) continue;
            return true;
        }
        return false;
    }

    private IEnumerator AimAt(ForestTree tree)
    {
        Vector3 aimPoint = tree.transform.position + Vector3.up * 1.4f;
        var attempts = new List<string>();
        for (int direction = 0; direction < InspectionDirections.Length; direction++)
            for (int distance = 0; distance < InspectionDistances.Length; distance++)
            {
                Vector3 standPoint = tree.transform.position + InspectionDirections[direction] * InspectionDistances[distance];
                if (OverlapsOtherTree(standPoint, tree)) { attempts.Add(direction + "/" + distance + " blocked"); continue; }
                player.transform.position = standPoint;
                player.LookToward(aimPoint);
                yield return Settle(8);
                ForestTree aimed = marks.AimedTree;
                if (aimed == tree) yield break;
                attempts.Add(direction + "/" + distance + " aimed " + (aimed != null ? aimed.TreeId : "nothing"));
            }
        throw new InvalidOperationException("could not aim at " + tree.TreeId + " [" + string.Join("; ", attempts) + "]");
    }

    private IEnumerator Inspect(ForestTree tree)
    {
        if (player.IsInspecting) yield return Press(Key.E);
        yield return AimAt(tree);
        yield return Press(Key.E);
        yield return Settle(8);
        ui.CloseHelp();   // a fresh profile opens the first-use Tree Inspection help here; closing it is the player's own action
        yield return Settle(4);
        if (!player.IsInspecting || player.InspectedTree != tree) throw new InvalidOperationException("could not inspect " + tree.TreeId);
    }

    // ---------------------------------------------------------------- UI helpers

    private VisualElement Card() => ui.RootElement.Q(name: "workplan-order-groups");
    private Foldout Foldout(string name) => ui.RootElement.Q<Foldout>(name);
    private void SetFoldout(string name, bool open) { Foldout f = Foldout(name); if (f != null) f.value = open; }
    private string FoldoutText(string name) { Foldout f = Foldout(name); return f == null ? "" : Text(f); }
    private VisualElement Review() => ui.RootElement.Q(className: "modal-review");
    private VisualElement ActiveModal(string cls) => ui.RootElement.Query(className: cls).ToList().FirstOrDefault(Visible);
    private string ReviewTitle() { VisualElement r = Review(); Label t = r == null ? null : r.Q<Label>(className: "title"); return t == null ? "" : t.text; }
    private int ReviewYear()
    {
        string[] parts = ReviewTitle().Split(new[] { "Year " }, StringSplitOptions.None);
        if (parts.Length < 2) return int.MinValue;
        return int.TryParse(new string(parts[1].TakeWhile(char.IsDigit).ToArray()), out int y) ? y : int.MinValue;
    }
    private string Tag() => size.x + "x" + size.y;
    private static string F(float v) => v.ToString("0.###", CultureInfo.InvariantCulture);
    private static string Text(VisualElement element) => element == null ? "" : string.Join("\n", element.Query<TextElement>().ToList().Where(e => e.resolvedStyle.display != DisplayStyle.None).Select(e => e.text));

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

    private IEnumerator UseSize(Vector2Int target)
    {
        size = target;
        SetGameSize(target.x, target.y);
        for (int frame = 0; frame < 120 && (Screen.width != target.x || Screen.height != target.y); frame++) yield return null;
        yield return Settle(24);
        Check(Screen.width == target.x && Screen.height == target.y, $"TARGET_RENDER_SIZE_{target.x}x{target.y}", $"actual={Screen.width}x{Screen.height}");
    }

    private IEnumerator Shot(string name, string modalClass)
    {
        VisualElement modal = ActiveModal(modalClass);
        Check(modal != null, "MODAL_VISIBLE_" + name + "_" + Tag(), modalClass);
        ScrollView scroll = modal == null ? null : modal.Q<ScrollView>();
        if (scroll != null) { scroll.verticalScroller.value = scroll.verticalScroller.lowValue; yield return Settle(6); }
        // A capture can come back blank for the first frames after a Game-view resize. Retry a few times (the assertion below still has to
        // hold for the stored image); if the screen stays blank the Editor's rendering is unavailable and the run stops rather than
        // spending minutes producing captures that cannot be judged.
        Texture2D image = null;
        for (int attempt = 0; attempt < 4; attempt++)
        {
            yield return new WaitForEndOfFrame();
            image = ScreenCapture.CaptureScreenshotAsTexture();
            if (image == null || !LooksBlank(image)) break;
            if (attempt < 3) { Destroy(image); image = null; yield return new WaitForSecondsRealtime(1f); yield return Settle(10); }
        }
        if (image == null) { Check(false, "SHOT_" + name, "no rendered capture"); yield break; }
        Check(image.width == size.x && image.height == size.y, "CAPTURE_SIZE_" + name + "_" + Tag(), $"capture={image.width}x{image.height} target={size.x}x{size.y}");
        bool blank = LooksBlank(image);
        Check(!blank, "SHOT_NOT_BLANK_" + name + "_" + Tag());
        if (blank) throw new InvalidOperationException("screen capture stayed blank after retries (rendering unavailable): " + name + " " + Tag());
        File.WriteAllBytes(Path.Combine(output, "shots", $"{name}-{size.x}x{size.y}.png"), image.EncodeToPNG());
        Destroy(image);
        shots++;
        Audit(name, modalClass);
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

    private readonly HashSet<string> audited = new HashSet<string>();

    // Readability: every visible text element stays inside the panel horizontally and is not vertically clipped; every visible button is usable.
    private void Audit(string name, string modalClass)
    {
        if (!audited.Add(name + "|" + Tag() + "|" + modalClass)) return;
        VisualElement modal = ActiveModal(modalClass);
        if (modal == null) { Check(false, "AUDIT_PANEL_" + name + "_" + Tag()); return; }
        Rect panel = modal.worldBound;
        var overflow = new List<string>(); var clipped = new List<string>(); int count = 0;
        foreach (TextElement element in modal.Query<TextElement>().ToList())
        {
            if (string.IsNullOrEmpty(element.text) || !Visible(element)) continue;
            count++;
            Rect r = element.worldBound;
            if (r.xMin < panel.xMin - 1.5f || r.xMax > panel.xMax + 1.5f) overflow.Add(Short(element.text, 40) + $" [{r.xMin:0}..{r.xMax:0} vs {panel.xMin:0}..{panel.xMax:0}]");
            if (element.contentRect.width > 1f)
            {
                // Auto-sized labels sit at exactly their text width; a 2 px tolerance avoids sub-pixel wrap false positives (as the accepted T-05 gate).
                Vector2 measured = element.MeasureTextSize(element.text, element.contentRect.width + 2f, VisualElement.MeasureMode.Exactly, 0, VisualElement.MeasureMode.Undefined);
                if (measured.y > element.contentRect.height + 1.5f) clipped.Add(Short(element.text, 40) + $" [measured {measured.y:0.0} vs content {element.contentRect.height:0.0} w {element.contentRect.width:0.0} {element.name}]");
            }
        }
        Check(count > 0, $"[{Tag()}]_AUDIT_HAS_TEXT_{name}");
        Check(overflow.Count == 0, $"[{Tag()}]_TEXT_INSIDE_PANEL_{name}", string.Join(" | ", overflow.Take(3)));
        Check(clipped.Count == 0, $"[{Tag()}]_TEXT_NOT_CLIPPED_{name}", string.Join(" | ", clipped.Take(3)));
        foreach (Button b in modal.Query<Button>().ToList().Where(Visible))
            Check(b.worldBound.width >= 24f && b.worldBound.height >= 16f, $"[{Tag()}]_BUTTON_USABLE_{name}_{b.name}_{Short(b.text, 20)}", $"{b.worldBound.width:0}x{b.worldBound.height:0}");
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

    private static IEnumerator Settle(int frames) { for (int i = 0; i < frames; i++) yield return null; }
    private IEnumerator Settled() { yield return new WaitForSecondsRealtime(0.35f); }

    // ---------------------------------------------------------------- bookkeeping

    private void Check(bool ok, string name, string detail = "")
    {
        if (ok) passes++; else failures++;
        string line = (ok ? "S1D_T08_CHECK_PASS " : "S1D_T08_CHECK_FAIL ") + name + (string.IsNullOrEmpty(detail) ? "" : " " + detail);
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
        sb.AppendLine("checks passed: " + passes + "  failed: " + failures + "  shots: " + shots + "  rows: " + table.rows.Count);
        if (failure != null) sb.AppendLine("exception: " + failure);
        sb.AppendLine(); sb.AppendLine("NOTES"); foreach (string note in notes) sb.AppendLine("- " + note);
        if (errors.Count > 0) { sb.AppendLine(); sb.AppendLine("CONSOLE ERRORS"); foreach (string e in errors) sb.AppendLine("- " + e); }
        sb.AppendLine(); sb.AppendLine("CHECK LIST"); foreach (string line in results) sb.AppendLine(line);
        File.WriteAllText(Path.Combine(output, "t08-result.txt"), sb.ToString());
    }
}

// FormatMoney is public static on the manager; this keeps the call sites readable.
internal static class S1DWitnessMoney
{
    public static string FormatMoneyText(this ScenarioOneManager m, long cents) => ScenarioOneManager.FormatMoney(cents);
}
#endif
