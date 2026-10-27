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

// Disposable in-Editor gate for S1-C T-02 (Annual Review year browsing).
//
// Builds a real multi-year session through the normal new-game path and the manager's own work APIs (four completed years:
// work, no work, work, planting only), then drives the real Annual Review view. It checks the ten items in
// Docs/Handoffs/S1-C-Implementation-02/TASK-PACKET.md: every completed year is reachable, the review opens on the latest
// year, browsing changes no simulation or save state, an older year neither shows nor performs the acknowledgement, the
// current review still gates advancement, completion-review acknowledgement behaves as before, work and no-work years
// both render coherently, and the review is legible at 1280x720 and 1920x1080.
//
// It also dumps the review's text for every state it visits (review-texts/*.txt) and a per-state gate table
// (gate-states.tsv). The same harness runs unchanged on the baseline commit (CCF_T02_MODE=baseline), so a text and gate
// diff proves the latest-year content did not regress. Baseline mode skips only the checks that need the new behaviour.
//
// No save is written to disk; saves are captured in memory only. The harness mutates the in-memory manager (extra years,
// and in the last phase deliberately removed records) and never restores it, so run it only in a throwaway Editor session.
//
// Run (graphical Editor on a private display; the parent launcher run_t02_gate.py handles display, config and cleanup):
//   CCF_DIAG_DIR=<folder outside the repo> CCF_UI_SIZES=1280x720,1920x1080 [CCF_T02_MODE=baseline]
//   <Unity 6000.6.0f1> -projectPath <checkout> -executeMethod AnnualReviewYearBrowsingVerification.Begin -logFile <log>
//
// Install by copying this file into Assets/ForestPrototype/, run, then remove it together with its generated .meta. The
// runtime installer only acts for the exact -executeMethod argument, so it can never auto-start in a normal session.
//
// LIMITATION: callback/API driving of the real UI state and renderer, not OS-native mouse input. Buttons are activated
// through their registered Clickable callback (the technique MenuTutorialVerification uses because Editor synthetic pointer
// events do not reach the runtime panel). It proves state, visibility, gate and navigation transitions and the rendered
// pixels; it does not prove OS-level click targeting.
public static class AnnualReviewYearBrowsingVerification
{
    private const string ExitKey = "CCF.S1C.T02.PendingExit";

    public static void Begin()
    {
        SessionState.EraseInt(ExitKey);
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        AnnualReviewYearBrowsingRunner.SetGameSize(1280, 720);
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
        if (!Environment.GetCommandLineArgs().Any(a => a == "AnnualReviewYearBrowsingVerification.Begin")) return;
        new GameObject("S1-C T-02 year browsing verification").AddComponent<AnnualReviewYearBrowsingRunner>();
    }
}

public sealed class AnnualReviewYearBrowsingRunner : MonoBehaviour
{
    private const string EarlierText = "◀ Earlier year";
    private const string LaterText = "Later year ▶";

    private string output, mode;
    private bool candidate;
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
            null, new object[] { Enum.ToObject(kind, 1), width, height, "S1C T02 " + width + "x" + height }, null);
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
        if (string.IsNullOrEmpty(output)) output = Path.Combine(Application.temporaryCachePath, "s1c-t02");
        Directory.CreateDirectory(output);
        Directory.CreateDirectory(Path.Combine(output, "review-texts"));
        Directory.CreateDirectory(Path.Combine(output, "shots"));
        mode = (Environment.GetEnvironmentVariable("CCF_T02_MODE") ?? "candidate").Trim().ToLowerInvariant();
        candidate = mode != "baseline";
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
            Debug.Log("S1C_T02_CHECK_FAIL NO_CONSOLE_ERRORS " + string.Join(" | ", errors.Take(4)));
        }
        bool pass = failure == null && failures == 0;
        Application.logMessageReceived -= OnLogMessage;
        WriteReports(pass, failure);
        Debug.Log(pass
            ? $"S1C_T02_HARNESS_PASS mode={mode} checks={passes} shots={shots}"
            : $"S1C_T02_HARNESS_FAIL mode={mode} checks={passes} failures={failures} exception={failure}");
        AnnualReviewYearBrowsingVerification.Finish(pass);
    }

    // ---------------------------------------------------------------- the scenario

    private IEnumerator Play()
    {
        yield return StartSession();
        yield return BuildFixture();

        IReadOnlyList<YearHistoryView> years = manager.HistoryYears;
        int latestYear = years[years.Count - 1].Year;
        Check(years.Count >= 3, "FIXTURE_HAS_AT_LEAST_THREE_COMPLETED_YEARS", "count=" + years.Count);
        Check(years.Select(y => y.Year).SequenceEqual(years.Select(y => y.Year).OrderBy(y => y)) && years.Select(y => y.Year).Distinct().Count() == years.Count,
            "HISTORY_YEARS_OLDEST_FIRST_UNIQUE");
        notes.Add("completed years: " + string.Join(",", years.Select(y => y.Year)));
        Check(ui.NeedsAnnualReview && !manager.AnnualReviewSeen, "FIRST_REVIEW_UNACKNOWLEDGED_AFTER_FIXTURE");
        Check(manager.NeedsCompletionReviewAcknowledgement, "COMPLETION_REVIEW_ACK_PENDING_AFTER_FIXTURE",
            "plans resolved with no acknowledgement event yet");

        // Everything after this point must leave the simulation and save untouched until the explicit acknowledgement phase.
        string hash0 = WorldHash();
        int events0 = manager.ManagementEvents.Count, reports0 = manager.AnnualReports.Count, calendar0 = manager.CurrentEcologicalYear;
        long cash0 = manager.CashCents;
        notes.Add("world hash after fixture: " + hash0);

        // ---- 2. opens on the latest year by default ----
        ui.CloseAll(); yield return Settle(4);
        ui.ShowReview(); yield return Settle(8);
        Check(ui.CurrentScreen == ScenarioOneUiRoot.UiScreen.Review, "REVIEW_OPENS");
        Check(Title().Contains("Year " + latestYear + " complete") && !Title().Contains("earlier year"), "OPENS_ON_LATEST_YEAR", Title());
        Check(IsEnabled(EarlierText) && !IsEnabled(LaterText), "LATEST_HAS_EARLIER_BUT_NO_LATER_CONTROL");
        yield return RecordState("latest-default");
        Check(FooterVisible(), "ACK_FOOTER_VISIBLE_ON_LATEST_WHILE_UNACKNOWLEDGED");
        string latestBefore = texts["latest-default"];

        // ---- 1/3. every completed year is reachable by stepping back, and back again, without changing state ----
        var visited = new List<int> { latestYear };
        for (int step = years.Count - 2; step >= 0; step--)
        {
            yield return ClickButton(EarlierText);
            int expected = years[step].Year;
            visited.Add(expected);
            Check(Title().Contains("Year " + expected) && Title().Contains("earlier year") && Title().Contains("latest is Year " + latestYear),
                "STEP_EARLIER_SHOWS_YEAR_" + expected, Title());
            yield return RecordState("year" + expected);
            Check(!FooterVisible(), "ACK_FOOTER_HIDDEN_ON_YEAR_" + expected);
            Check(IsEnabled(LaterText), "LATER_ENABLED_ON_YEAR_" + expected);
            Check(texts["year" + expected] != latestBefore, "YEAR_" + expected + "_CONTENT_DIFFERS_FROM_LATEST");
            CheckHistoricalContent(expected);
        }
        Check(!IsEnabled(EarlierText), "EARLIER_DISABLED_AT_FIRST_COMPLETED_YEAR");
        Check(visited.OrderBy(y => y).SequenceEqual(years.Select(y => y.Year)), "ALL_COMPLETED_YEARS_VISITED", string.Join(",", visited));
        CheckWorkAndNoWorkYears(years);

        for (int step = 1; step < years.Count; step++)
        {
            yield return ClickButton(LaterText);
            Check(Title().Contains("Year " + years[step].Year), "STEP_LATER_SHOWS_YEAR_" + years[step].Year, Title());
        }
        Check(Title().Contains("Year " + latestYear + " complete") && !Title().Contains("earlier year"), "BACK_ON_LATEST_AFTER_STEPPING_BACK", Title());
        yield return RecordState("latest-after-roundtrip");
        Check(texts["latest-after-roundtrip"] == latestBefore, "LATEST_CONTENT_UNCHANGED_AFTER_ROUND_TRIP");
        Check(FooterVisible(), "ACK_FOOTER_RETURNS_ON_LATEST");
        CheckUnchanged("AFTER_BROWSING", hash0, events0, reports0, calendar0, cash0);

        // Closing and reopening returns to the latest year even after browsing back.
        yield return ClickButton(EarlierText);
        yield return ClickButton(EarlierText);
        ui.CloseAll(); yield return Settle(4);
        ui.ShowReview(); yield return Settle(8);
        Check(Title().Contains("Year " + latestYear + " complete"), "REOPEN_RETURNS_TO_LATEST", Title());
        CheckUnchanged("AFTER_CLOSE_REOPEN", hash0, events0, reports0, calendar0, cash0);

        // ---- 5/6. an older year neither shows nor triggers the acknowledgement; the current review still gates advancement ----
        yield return ClickButton(EarlierText);
        yield return ClickButton(EarlierText);
        Check(Title().Contains("earlier year") && !FooterVisible(), "OLDER_YEAR_HAS_NO_ACK_FOOTER", Title());
        if (candidate)
        {
            // The registered callback itself must refuse while an older year is showing.
            Button ack = ui.RootElement.Q<Button>("annual-review-acknowledge");
            Check(ack != null, "ACK_BUTTON_EXISTS");
            yield return ClickButton(ack, false);
            Check(!manager.AnnualReviewSeen && manager.ManagementEvents.Count == events0 && ui.NeedsAnnualReview,
                "OLDER_YEAR_ACK_CALLBACK_DOES_NOTHING", $"seen={manager.AnnualReviewSeen} events={manager.ManagementEvents.Count}/{events0}");
        }
        Check(string.IsNullOrEmpty(ui.RackPlanProblem), "NO_RACK_PLAN_PROBLEM_SO_THE_GATE_IS_THE_REVIEW", ui.RackPlanProblem);
        Check(!ui.AdvanceFromWorkPlan() && manager.CurrentEcologicalYear == calendar0, "ADVANCE_BLOCKED_WHILE_REVIEW_UNACKNOWLEDGED_OLDER_VIEW");
        yield return Settle(4);
        Check(Title().Contains("earlier year"), "BLOCKED_ADVANCE_DOES_NOT_JUMP_THE_VIEW", Title());
        CheckUnchanged("AFTER_OLDER_YEAR_GATE_PROBES", hash0, events0, reports0, calendar0, cash0);
        ui.CloseAll(); yield return Settle(4);
        ui.ShowReview(); yield return Settle(8);
        Check(!ui.AdvanceFromWorkPlan() && manager.CurrentEcologicalYear == calendar0, "ADVANCE_BLOCKED_WHILE_REVIEW_UNACKNOWLEDGED_LATEST_VIEW");
        Check(Title().Contains("Year " + latestYear + " complete"), "REVIEW_SHOWN_ON_LATEST_WHEN_ADVANCE_IS_BLOCKED", Title());

        // ---- 7. completion-review acknowledgement, isolated from the first-review flag ----
        // Mark the first review as seen WITHOUT an acknowledgement event so only the completion-review rule is still pending.
        SetSeen(true); yield return Settle(4); ui.CloseAll(); yield return Settle(4); ui.ShowReview(); yield return Settle(8);
        Check(!ui.NeedsAnnualReview && manager.NeedsCompletionReviewAcknowledgement, "ISOLATED_COMPLETION_ACK_PENDING");
        Check(FooterVisible(), "COMPLETION_ACK_FOOTER_VISIBLE_ON_LATEST");
        yield return RecordState("completion-only-latest");
        yield return ClickButton(EarlierText);
        Check(!FooterVisible(), "COMPLETION_ACK_FOOTER_HIDDEN_ON_OLDER_YEAR");
        yield return RecordState("completion-only-older");
        SetSeen(false); yield return Settle(4); ui.CloseAll(); yield return Settle(4); ui.ShowReview(); yield return Settle(8);

        // ---- 9. rendered legibility at both resolutions (state: first review unacknowledged, as a player sees it) ----
        foreach (Vector2Int target in Sizes())
        {
            yield return UseSize(target);
            ui.CloseAll(); yield return Settle(4); ui.ShowReview(); yield return Settle(10);
            yield return Shot("year" + latestYear + "-latest-top", false);
            yield return Shot("year" + latestYear + "-latest-lower", true);
            for (int step = years.Count - 2; step >= 0; step--)
            {
                yield return ClickButton(EarlierText);
                yield return Shot("year" + years[step].Year + "-earlier-top", false);
                if (step == 0) yield return Shot("year" + years[step].Year + "-earlier-lower", true);
            }
        }
        CheckUnchanged("AFTER_RENDER_PASS", hash0, events0, reports0, calendar0, cash0);

        // ---- 6. the real acknowledgement on the latest review releases the gate exactly as before ----
        ui.CloseAll(); yield return Settle(4); ui.ShowReview(); yield return Settle(8);
        Button latestAck = ui.RootElement.Q<Button>("annual-review-acknowledge");
        Check(FooterVisible() && latestAck != null, "ACK_AVAILABLE_ON_LATEST_BEFORE_CLICK");
        yield return ClickButton(latestAck, true);
        Check(manager.AnnualReviewSeen && !ui.NeedsAnnualReview, "LATEST_ACK_MARKS_REVIEW_SEEN");
        Check(manager.ManagementEvents.Count == events0 + 1
            && manager.ManagementEvents[manager.ManagementEvents.Count - 1].eventType == ScenarioManagementEventType.AnnualReviewAcknowledged,
            "LATEST_ACK_RECORDS_EXACTLY_ONE_ACKNOWLEDGED_EVENT", "events=" + manager.ManagementEvents.Count);
        yield return Settle(4);
        Check(!ui.NeedsAnnualReview && !manager.NeedsCompletionReviewAcknowledgement, "ACK_CLEARS_BOTH_PENDING_FLAGS");
        yield return RecordState("latest-after-ack");
        Check(ui.AdvanceFromWorkPlan() && manager.CurrentEcologicalYear == calendar0 + 1, "ADVANCE_ALLOWED_AFTER_ACK");
        yield return Settle(10);
        int newest = manager.HistoryYears[manager.HistoryYears.Count - 1].Year;
        Check(newest == latestYear + 1 && Title().Contains("Year " + newest + " complete"), "NEW_YEAR_OPENS_ON_NEWEST_REVIEW", Title());
        Check(!ui.NeedsAnnualReview && !manager.NeedsCompletionReviewAcknowledgement, "NO_RECURRING_REVIEW_GATE_AFTER_FIRST_ACK");
        yield return RecordState("new-year-after-ack");

        // ---- 2b. a new completed year returns an older view to the latest, rather than leaving it on the old year ----
        yield return ClickButton(EarlierText);
        yield return ClickButton(EarlierText);
        Check(Title().Contains("earlier year"), "BROWSING_OLDER_BEFORE_NEW_YEAR", Title());
        Check(manager.AdvanceYear(), "ADVANCE_WHILE_REVIEW_OPEN_ON_OLDER_YEAR");
        yield return Settle(10);
        int newest2 = manager.HistoryYears[manager.HistoryYears.Count - 1].Year;
        Check(Title().Contains("Year " + newest2 + " complete") && !Title().Contains("earlier year"), "NEW_COMPLETED_YEAR_RETURNS_VIEW_TO_LATEST", Title());

        // ---- 8. defensive record cases stay deterministic (new behaviour, so candidate only) ----
        if (candidate) yield return DefensiveCases();
        yield return TeachingAcknowledgementCases();

        notes.Add(Limitation);
    }

    private const string Limitation =
        "Callback/API driving of the real UI state and renderer: no OS-native pointer or key delivery was exercised.";

    private IEnumerator TeachingAcknowledgementCases()
    {
        // Isolate the teaching term of the existing footer predicate after first/completion acknowledgement is clear.
        // These cached-fact fixtures test UI routing only, not teaching progression or Work Plan unlocks. In particular,
        // never call RackPlanProblem/WorkPlan.Refresh here: those correctly discard cached facts and read the real stand.
        ui.CloseAll(); yield return Settle(4); ui.ShowReview(); yield return Settle(8);
        Check(!ui.NeedsAnnualReview && !manager.NeedsCompletionReviewAcknowledgement, "TEACHING_FIXTURE_OTHER_ACK_FLAGS_CLEAR");
        string hash = WorldHash();
        int eventCount = manager.ManagementEvents.Count;
        object review = typeof(ScenarioOneUiRoot).GetField("review", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(ui);
        var refresh = review.GetType().GetMethod("Refresh");
        Type learning = typeof(LearningObjectivesView);
        FieldInfo cached = learning.GetField("sequenceFacts", BindingFlags.Instance | BindingFlags.NonPublic);
        FieldInfo expiry = learning.GetField("nextSequenceRefresh", BindingFlags.Instance | BindingFlags.NonPublic);
        FieldInfo version = learning.GetField("teachingWorldVersion", BindingFlags.Instance | BindingFlags.NonPublic);
        foreach (int stage in new[] { 1, 4, 6 })
        {
            var facts = new ScenarioOneTeachingSequence.Facts
            {
                AccessOpen = true, AccessReviewed = stage > 1, CropTrees = 3,
                ReleaseExecuted = stage >= 4, DeadwoodExecuted = stage >= 4, ReleaseReviewed = stage > 4,
                ReviewedTwoCycles = true, BeechEstablished = true, OakEstablished = true,
                PlantingYear = manager.CurrentEcologicalYear - 1, Year = manager.CurrentEcologicalYear
            };
            version.SetValue(ui.Learning, manager.TeachingWorldVersion);
            cached.SetValue(ui.Learning, facts);
            expiry.SetValue(ui.Learning, float.MaxValue);
            refresh.Invoke(review, new object[] { true }); yield return Settle(4);
            Check(ui.Learning.CurrentTopicIndex == stage && ui.Learning.NeedsReviewAcknowledgement,
                "TEACHING_BOUNDARY_" + stage + "_PENDING");
            Check(FooterVisible(), "TEACHING_ONLY_FOOTER_ON_LATEST_" + stage);
            yield return RecordState("teaching-only-" + stage + "-latest");
            yield return ClickButton(EarlierText);
            Check(!FooterVisible(), "TEACHING_ONLY_FOOTER_HIDDEN_ON_OLDER_" + stage);
            if (candidate)
            {
                yield return ClickButton(ui.RootElement.Q<Button>("annual-review-acknowledge"), false);
                Check(manager.ManagementEvents.Count == eventCount, "TEACHING_OLDER_CALLBACK_DOES_NOT_ACK_" + stage);
            }
            yield return RecordState("teaching-only-" + stage + "-older");
            yield return ClickButton(LaterText);
        }
        Check(WorldHash() == hash && manager.ManagementEvents.Count == eventCount, "TEACHING_BROWSING_LEAVES_SAVE_UNCHANGED");
        yield return ClickButton(ui.RootElement.Q<Button>("annual-review-acknowledge"), true);
        Check(manager.ManagementEvents.Count == eventCount + 1
            && manager.ManagementEvents.Last().eventType == ScenarioManagementEventType.AnnualReviewAcknowledged,
            "TEACHING_LATEST_CALLBACK_RECORDS_ONE_ACK");
        Check(!ui.Learning.NeedsReviewAcknowledgement && !FooterVisible(), "TEACHING_ACK_RETURNS_TO_REAL_STAND_FACTS");
        notes.Add("Teaching stages 1/4/6 use isolated cached-fact UI fixtures; no curriculum progression or Work Plan unlock qualification is claimed.");
    }

    private IEnumerator DefensiveCases()
    {
        int baselineErrors = errors.Count;
        IReadOnlyList<YearHistoryView> years = manager.HistoryYears;
        int latest = years[years.Count - 1].Year;
        int target = years[1].Year;   // an interior year: neither first nor latest

        // (a) a completed year whose annual report is missing: still reachable, shown honestly, nothing thrown
        List<ScenarioAnnualReport> reports = PrivateList<ScenarioAnnualReport>("annualReports");
        int reportIndex = reports.FindIndex(r => r.year == target);
        ScenarioAnnualReport removedReport = reports[reportIndex];
        reports.RemoveAt(reportIndex);
        Check(manager.HistoryYears.Any(y => y.Year == target && y.Report == null), "DEFENSIVE_YEAR_STILL_COMPLETED_WITHOUT_REPORT");
        ui.CloseAll(); yield return Settle(4); ui.ShowReview(); yield return Settle(8);
        int guard = 0;
        while (!Title().Contains("Year " + target) && guard++ < 10) yield return ClickButton(EarlierText);
        Check(Title().Contains("Year " + target), "DEFENSIVE_NO_REPORT_YEAR_IS_REACHABLE", Title());
        yield return RecordState("defensive-no-report");
        Check(texts["defensive-no-report"].Contains("No annual report was recorded for this year"), "DEFENSIVE_NO_REPORT_SAYS_SO");
        Check(!FooterVisible(), "DEFENSIVE_NO_REPORT_NOT_ACKNOWLEDGEABLE");
        yield return ClickButton(EarlierText);
        Check(Title().Contains("earlier year"), "DEFENSIVE_NAVIGATION_CONTINUES_PAST_THE_GAP", Title());
        reports.Insert(reportIndex, removedReport);

        // (b) a completed year whose ecological snapshot is missing: say so; never borrow the latest snapshot
        List<ScenarioEcologicalSnapshot> snaps = PrivateList<ScenarioEcologicalSnapshot>("ecologicalSnapshots");
        int snapIndex = snaps.FindIndex(s => s.year == target);
        ScenarioEcologicalSnapshot removedSnap = snaps[snapIndex];
        snaps.RemoveAt(snapIndex);
        ui.CloseAll(); yield return Settle(4); ui.ShowReview(); yield return Settle(8);
        guard = 0;
        while (!Title().Contains("Year " + target) && guard++ < 10) yield return ClickButton(EarlierText);
        yield return RecordState("defensive-no-snapshot");
        Check(texts["defensive-no-snapshot"].Contains("No ecological snapshot was recorded for this year"), "DEFENSIVE_NO_SNAPSHOT_SAYS_SO");
        Check(!texts["defensive-no-snapshot"].Contains("Fallen deadwood on site"), "DEFENSIVE_NO_SNAPSHOT_DOES_NOT_BORROW_LATEST", "no latest-snapshot line under an old year");
        snaps.Insert(snapIndex, removedSnap);

        // (c) a remembered year that is not a completed year falls back to the latest deterministically
        ui.CloseAll(); yield return Settle(4); ui.ShowReview(); yield return Settle(8);
        object review = typeof(ScenarioOneUiRoot).GetField("review", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(ui);
        FieldInfo remembered = review.GetType().GetField("viewYear", BindingFlags.Instance | BindingFlags.NonPublic);
        remembered.SetValue(review, 999);
        review.GetType().GetMethod("Refresh").Invoke(review, new object[] { true });
        yield return Settle(6);
        Check(Title().Contains("Year " + latest + " complete") && !Title().Contains("earlier year") && (int)remembered.GetValue(review) == -1,
            "DEFENSIVE_UNKNOWN_YEAR_FALLS_BACK_TO_LATEST", Title());
        Check(errors.Count == baselineErrors, "DEFENSIVE_CASES_RAISE_NO_ERRORS", string.Join(" | ", errors.Skip(baselineErrors).Take(3)));
    }

    // ---------------------------------------------------------------- startup and fixture

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

    private IEnumerator BuildFixture()
    {
        // Year 1: thin three trees and plant one sheltered oak (work year).
        yield return PlanFelling(3);
        yield return PlanPlanting(true);
        AdvanceWithWork("year 1");
        // Year 2: nothing planned (no-work year).
        Check(manager.AdvanceYear(), "FIXTURE_YEAR_2_IDLE");
        // Year 3: thin two more trees (second grouped harvest).
        yield return PlanFelling(2);
        AdvanceWithWork("year 3");
        // Year 4: planting only (latest year; a different kind of work year).
        yield return PlanPlanting(false);
        AdvanceWithWork("year 4");
        yield return Settle(6);
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

    private IEnumerator PlanPlanting(bool shelter)
    {
        Check(manager.TryPurchaseStock("sessile-oak-sapling", 1), "FIXTURE_BUY_OAK_STOCK");
        bool placed = false;
        foreach (Vector3 point in PlantingCandidates())
            if (manager.TryDesignateExactPlanting("sessile-oak-sapling", point, WorkExecutionMethod.LandownerSimulated, shelter)) { placed = true; break; }
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

    // ---------------------------------------------------------------- content checks

    private void CheckHistoricalContent(int year)
    {
        string text = texts["year" + year];
        // Live-only lines would describe today's forest under an old date.
        Check(!text.Contains("Retained usable timber (stock)"), "YEAR_" + year + "_OMITS_LIVE_STOCK_LINE");
        Check(!text.Contains("Browsing pressure") && !text.Contains("planted juvenile(s) protected by shelters"), "YEAR_" + year + "_OMITS_LIVE_BROWSING_LINES");
        Check(text.Contains("(current state)"), "YEAR_" + year + "_LABELS_OBJECTIVES_AS_CURRENT_STATE");
        // The closing cash shown must be that year's own annual-report figure (oracle: the raw recorded report).
        ScenarioAnnualReport report = manager.AnnualReports.First(r => r.year == year);
        string cash = "Closing cash";
        int at = text.IndexOf(cash, StringComparison.Ordinal);
        Check(at >= 0 && text.Substring(at, Math.Min(80, text.Length - at)).Contains(UiMoney(report.closingCashCents)), "YEAR_" + year + "_SHOWS_ITS_OWN_CLOSING_CASH",
            "expected " + UiMoney(report.closingCashCents));
    }

    private void CheckWorkAndNoWorkYears(IReadOnlyList<YearHistoryView> years)
    {
        int latestYear = years[years.Count - 1].Year;
        foreach (YearHistoryView y in years)
        {
            string text = texts[y.Year == latestYear ? "latest-default" : "year" + y.Year];
            bool hasWork = y.ResolvedWork.Any(r => r.Event.outcome == ScenarioManagementOutcome.Succeeded);
            if (hasWork) Check(!text.Contains("No work was resolved this year."), "WORK_YEAR_" + y.Year + "_LISTS_ITS_WORK");
            else Check(text.Contains("No work was resolved this year.") && text.Contains("No tasks failed."), "NO_WORK_YEAR_" + y.Year + "_SAYS_SO");
        }
        Check(years.Any(y => y.ResolvedWork.Count == 0) && years.Any(y => y.ResolvedWork.Count > 0), "FIXTURE_COVERS_BOTH_WORK_AND_NO_WORK_YEARS");
        string firstYearText = texts["year" + years[0].Year];
        Check(firstYearText.Contains("tree(s) thinned (contractor)") && firstYearText.Contains("planted ("), "FIRST_YEAR_SHOWS_THINNING_AND_PLANTING");
    }

    private void CheckUnchanged(string when, string hash0, int events0, int reports0, int calendar0, long cash0)
    {
        string hash = WorldHash();
        Check(hash == hash0 && manager.ManagementEvents.Count == events0 && manager.AnnualReports.Count == reports0
            && manager.CurrentEcologicalYear == calendar0 && manager.CashCents == cash0 && !manager.AnnualReviewSeen,
            "SIM_AND_SAVE_STATE_UNCHANGED_" + when,
            $"hash={hash} events={manager.ManagementEvents.Count}/{events0} reports={manager.AnnualReports.Count}/{reports0} year={manager.CurrentEcologicalYear}/{calendar0} seen={manager.AnnualReviewSeen}");
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

    private void SetSeen(bool value)
    {
        FieldInfo field = typeof(ScenarioOneManager).GetField("annualReviewSeen", BindingFlags.Instance | BindingFlags.NonPublic);
        if (field == null) throw new MissingFieldException("annualReviewSeen");
        field.SetValue(manager, value);
    }

    private List<T> PrivateList<T>(string name)
    {
        FieldInfo field = typeof(ScenarioOneManager).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);
        if (field == null) throw new MissingFieldException(name);
        return (List<T>)field.GetValue(manager);
    }

    private static string UiMoney(long cents) => UiKit.Money(cents);

    // ---------------------------------------------------------------- rendering

    private IEnumerator UseSize(Vector2Int target)
    {
        size = target;
        SetGameSize(target.x, target.y);
        for (int frame = 0; frame < 120 && (Screen.width != target.x || Screen.height != target.y); frame++) yield return null;
        yield return Settle(24);
        Check(Screen.width == target.x && Screen.height == target.y, $"TARGET_RENDER_SIZE_{target.x}x{target.y}", $"actual={Screen.width}x{Screen.height}");
    }

    private IEnumerator Shot(string name, bool scrolledToEnd)
    {
        ScrollView scroll = Modal().Q<ScrollView>();
        if (scroll != null)
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
        Debug.Log($"S1C_T02_SHOT {file}");
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
        string line = (ok ? "S1C_T02_CHECK_PASS " : "S1C_T02_CHECK_FAIL ") + name + (string.IsNullOrEmpty(detail) ? "" : " " + detail);
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
        File.WriteAllText(Path.Combine(output, "t02-result.txt"), sb.ToString());
    }
}
#endif
