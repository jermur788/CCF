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
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Disposable Editor-only follow-up gate for S1-D T-08 (claim #208). Verification only: no production edit.
// It starts a FRESH Editor session on a brand-new forest, then loads a disk save written by an EARLIER witness session
// (CCF_T08_RESUME_SAVE) through the real ForestSaveController.CanLoad()/Load() path and proves:
//   * the resumed state is identical to what the earlier session fingerprinted before saving (CCF_T08_EXPECT_FINGERPRINT, the sha256
//     of the witness's Fingerprint() text, reproduced here byte for byte) - chronology, orders, history, latch and world survive a process boundary;
//   * ordinary management continues after Completed: an unaffordable harvest is refused by the ordinary approval rule without changing
//     anything, an idle year advances, and Completed/latch/guide are unchanged;
//   * the economy's honest limit is recorded (cash vs the EUR 2,500 harvest minimum) rather than assumed.
public static class S1DResumeProbeGate
{
    private const string ExitKey = "CCF.S1D.T08.ResumeExit";

    public static void Begin()
    {
        SessionState.EraseInt(ExitKey);
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
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
        if (!Environment.GetCommandLineArgs().Any(a => a == "S1DResumeProbeGate.Begin")) return;
        new GameObject("Disposable S1-D T-08 resume probe").AddComponent<S1DResumeProbeRunner>();
    }
}

public sealed class S1DResumeProbeRunner : MonoBehaviour
{
    private ScenarioOneUiRoot ui;
    private ScenarioOneManager manager;
    private ForestSaveController saves;
    private int passes, failures;
    private string output;
    private readonly List<string> results = new List<string>(), errors = new List<string>(), notes = new List<string>();

    private IEnumerator Start()
    {
        output = Environment.GetEnvironmentVariable("CCF_DIAG_DIR");
        if (string.IsNullOrEmpty(output)) throw new InvalidOperationException("CCF_DIAG_DIR must name durable evidence outside the repo");
        Application.logMessageReceived += OnLogMessage;
        Exception failure = null;
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
        if (errors.Count > 0) { failures++; Debug.Log("S1D_T08_RESUME_CHECK_FAIL NO_CONSOLE_ERRORS " + string.Join(" | ", errors.Take(4))); }
        Application.logMessageReceived -= OnLogMessage;
        bool pass = failure == null && failures == 0;
        var sb = new StringBuilder();
        sb.AppendLine("result: " + (pass ? "PASS" : "FAIL") + "  checks " + passes + "  failures " + failures);
        if (failure != null) sb.AppendLine("exception: " + failure);
        foreach (string n in notes) sb.AppendLine("NOTE " + n);
        foreach (string r in results) sb.AppendLine(r);
        File.WriteAllText(Path.Combine(output, "resume-result.txt"), sb.ToString());
        Debug.Log(pass ? $"S1D_T08_RESUME_PASS checks={passes}" : $"S1D_T08_RESUME_FAIL checks={passes} failures={failures} exception={failure}");
        S1DResumeProbeGate.Finish(pass);
    }

    private string SavePath => Path.Combine(Application.persistentDataPath, "forest-save.json");

    private IEnumerator Run()
    {
        float deadline = Time.realtimeSinceStartup + 30f;
        do
        {
            ui = FindFirstObjectByType<ScenarioOneUiRoot>();
            manager = ui != null ? ui.Manager : null;
            saves = FindFirstObjectByType<ForestSaveController>();
            if (ui != null && manager != null && saves != null) break;
            yield return null;
        } while (Time.realtimeSinceStartup < deadline);
        Fatal(ui != null && manager != null && saves != null, "SCENE_COMPONENTS_READY");
        yield return Settle(20);
        string config = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME");
        Fatal(!string.IsNullOrEmpty(config) && Application.persistentDataPath.StartsWith(config.TrimEnd('/') + "/", StringComparison.Ordinal), "ISOLATED_PERSISTENT_PATH", Application.persistentDataPath);
        Check(!File.Exists(SavePath), "ISOLATED_SAVE_SLOT_STARTS_EMPTY");
        saves.enabled = false;
        Check(manager.CurrentEcologicalYear == 0 && manager.Outcome == ScenarioOneOutcome.Active && !manager.FirstGuidedRackReviewComplete, "FRESH_PROCESS_STARTS_A_NEW_YEAR_0_FOREST");

        string source = Environment.GetEnvironmentVariable("CCF_T08_RESUME_SAVE");
        string expectFp = Environment.GetEnvironmentVariable("CCF_T08_EXPECT_FINGERPRINT");
        Fatal(!string.IsNullOrEmpty(source) && File.Exists(source) && !string.IsNullOrEmpty(expectFp), "RESUME_INPUTS_PRESENT", source ?? "<unset>");
        File.Copy(source, SavePath, false);
        string sha = Sha(File.ReadAllBytes(SavePath));
        notes.Add("resumed save " + Path.GetFileName(source) + " sha256 " + sha);
        Fatal(saves.CanLoad(out string problem), "EARLIER_SESSION_SAVE_IS_LOADABLE_BY_THE_REAL_CONTROLLER", problem);
        saves.Load();
        yield return Settle(24);
        ui.CloseAll(); ui.CloseHelp();
        ui.RackLayout.Update(true); ui.Learning.ReloadProgress();
        for (int r = 0; r < 6; r++) ui.Learning.Refresh(true);
        yield return new WaitForSecondsRealtime(0.35f);

        string fp = Fingerprint();
        string fpSha = Sha(Encoding.UTF8.GetBytes(fp));
        File.WriteAllText(Path.Combine(output, "resumed-fingerprint.txt"), fp);
        Check(fpSha == expectFp, "RESUMED_STATE_FINGERPRINT_EQUALS_THE_EARLIER_SESSIONS", fpSha.Substring(0, 12) + " vs " + expectFp.Substring(0, 12));
        Check(manager.CurrentEcologicalYear == 26 && manager.Outcome == ScenarioOneOutcome.Completed && manager.OutcomeYear == 25, "RESUMED_AT_YEAR_26_COMPLETED_AT_25", $"{manager.CurrentEcologicalYear} {manager.Outcome}@{manager.OutcomeYear}");
        Check(manager.FirstGuidedRackReviewComplete && GuideIndex() >= 5, "LATCH_AND_GUIDE_SURVIVE_THE_PROCESS_BOUNDARY", "index=" + GuideIndex());
        Check(manager.AnnualReviewSeen && !ui.NeedsAnnualReview, "ACKNOWLEDGEMENT_SURVIVES_THE_PROCESS_BOUNDARY");
        Check(manager.CanEditWorkPlan && manager.CurrentEcologicalYear < manager.CenturyReviewYear, "MANAGEMENT_STILL_OPEN_BELOW_THE_CENTURY_HORIZON");
        notes.Add($"cash {ScenarioOneManager.FormatMoney(manager.CashCents)}; harvest minimum {ScenarioOneManager.FormatMoney(manager.Definition.MinimumHarvestJobCents)}");

        // An ordinary felling plan that the economy cannot pay for is refused by the ordinary approval rule, changing nothing.
        var tree = FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
            .Where(t => t.IsLiving && t.CanChop && !t.IsCropTree && t.TreeId.StartsWith("P", StringComparison.Ordinal)).OrderBy(t => t.TreeId, StringComparer.Ordinal).First();
        FindFirstObjectByType<ForestTreeMarkingManager>().Mark(tree, TreeMarkType.Fell, false);
        manager.AddMarkedTreesToWorkPlan();
        ui.CloseAll(); ui.ShowWorkPlan(); yield return Settle(10); ui.CloseAll();
        string beforeRefusal = Fingerprint();
        long cash = manager.CashCents;
        var quote = manager.GetHarvestQuote(false);
        notes.Add($"quoted external cost {ScenarioOneManager.FormatMoney(quote.CostCents)} for one marked tree; eligible={quote.Eligible}");
        bool approved = ui.ApproveWorkPlan();
        string feedback = manager.Feedback;
        notes.Add("approval of one harvest job: approved=" + approved + " feedback=\"" + feedback + "\"");
        if (quote.CostCents > cash)
        {
            // The production wording is the harvest quote's own problem text ("Upfront cash is required; future timber receipts do not prefinance the job.").
            Check(!approved && feedback.IndexOf("cash", StringComparison.OrdinalIgnoreCase) >= 0 && Fingerprint() == beforeRefusal, "UNAFFORDABLE_HARVEST_IS_REFUSED_BY_THE_ORDINARY_RULE_WITHOUT_CHANGING_STATE", feedback);
        }
        else
        {
            Check(approved, "AFFORDABLE_HARVEST_APPROVES_NORMALLY", feedback);
            Fatal(ui.AdvanceFromWorkPlan(), "AFFORDABLE_HARVEST_ADVANCES", manager.Feedback);
        }
        // Withdraw the draft, then an idle year still advances and nothing about completion changes.
        foreach (var o in manager.WorkOrders.Where(o => o.status == ScenarioWorkStatus.Pending && o.type == ScenarioWorkType.FellTree && o.rackId == 0 && o.prescriptionId == 0).ToList()) manager.RemovePendingOrder(o.workOrderId);
        FindFirstObjectByType<ForestTreeMarkingManager>().Unmark(tree, false);
        if (!approved)
        {
            int year = manager.CurrentEcologicalYear; int events = manager.ManagementEvents.Count(e => e.eventType == ScenarioManagementEventType.ScenarioCompleted);
            Check(ui.AdvanceFromWorkPlan() && manager.CurrentEcologicalYear == year + 1, "IDLE_YEAR_ADVANCES_AFTER_COMPLETION_IN_A_RESUMED_SESSION", manager.Feedback);
            yield return Settle(12);
            Check(manager.Outcome == ScenarioOneOutcome.Completed && manager.OutcomeYear == 25 && manager.ManagementEvents.Count(e => e.eventType == ScenarioManagementEventType.ScenarioCompleted) == events && manager.CashCents <= cash,
                "COMPLETED_UNCHANGED_AND_NO_SECOND_COMPLETION_EVENT");
            ui.AcknowledgeAnnualReview();
            Check(manager.AnnualReviewSeen && !ui.NeedsAnnualReview, "RESUMED_SESSION_ACKNOWLEDGES_THE_NEW_YEAR");
        }
        saves.Save();
        string p2 = "";
        bool written = File.Exists(SavePath);
        bool loadable = written && saves.CanLoad(out p2);
        Check(written && loadable, "RESUMED_SESSION_SAVES_AND_THE_RESULT_IS_LOADABLE", p2);
        File.Delete(SavePath);
    }

    private ScenarioOneTeachingSequence.Facts LiveFacts()
    {
        MethodInfo method = typeof(LearningObjectivesView).GetMethod("TeachingFacts", BindingFlags.Instance | BindingFlags.NonPublic);
        return (ScenarioOneTeachingSequence.Facts)method.Invoke(ui.Learning, null);
    }

    private int GuideIndex() { ui.Learning.Refresh(true); return ui.Learning.CurrentTopicIndex; }
    private static string F(float v) => v.ToString("0.###", CultureInfo.InvariantCulture);
    private static string Sha(byte[] data) { using (var h = SHA256.Create()) return string.Concat(h.ComputeHash(data).Select(b => b.ToString("x2"))); }
    private string ScenarioJson() => JsonUtility.ToJson(manager.CaptureSaveData());
    private string WorldHash() => ScenarioReferenceArchive.WorldHash(saves.CaptureData());
    private string OrderFields() => string.Join(";", manager.WorkOrders.Select(o => o.workOrderId + ":" + o.status + ":" + o.createdYear + ":" + o.resolvedYear + ":" + o.prescriptionId));

    // Must stay byte-for-byte the witness gate's Fingerprint().
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

    private static IEnumerator Settle(int frames) { for (int i = 0; i < frames; i++) yield return null; }

    private void Check(bool ok, string name, string detail = "")
    {
        if (ok) passes++; else failures++;
        string line = (ok ? "S1D_T08_RESUME_CHECK_PASS " : "S1D_T08_RESUME_CHECK_FAIL ") + name + (string.IsNullOrEmpty(detail) ? "" : " " + detail);
        results.Add(line);
        Debug.Log(line);
    }

    private void Fatal(bool ok, string name, string detail = "")
    {
        Check(ok, name, detail);
        if (!ok) throw new InvalidOperationException("precondition failed: " + name + " " + detail);
    }

    private void OnLogMessage(string condition, string stackTrace, LogType type)
    {
        if (type != LogType.Error && type != LogType.Exception && type != LogType.Assert) return;
        if (errors.Count < 40) errors.Add(type + ": " + condition);
    }
}
#endif
