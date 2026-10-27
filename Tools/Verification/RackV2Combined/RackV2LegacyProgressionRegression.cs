#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

// Disposable focused regression gate for RACK-V2-COMBINED-REPAIR-01: the repair made the rack lesson predicates read the persisted V2
// rack objects. This proves model-0 worlds (which have no V2 rack objects) still take the original authored-layout predicates.
//
//   model0    a fresh Enlarged80 game regenerated as the HISTORICAL model-0 world (the recipe RackV2HeadlandQualification and the
//             RackUx model-0 variant use). The five authored layout lines are planned through the legacy ConfirmRack path, approved
//             through the real Work Plan, executed by advancing one year; rack.create / rack.layout / rack.approve / rack.result and the
//             move to the next lesson must follow the authored lines. Orders carry no rack id in model 0, so a V2 predicate (rack-owned
//             work only) could never record rack.approve here.
//   legacy40  a fresh Legacy40 game (CCF_STAND_GEOMETRY=0). It has no authored layout, so the tracker stays empty, only rack.create
//             can be recorded and the next lesson opens from the first completed felling.
//
// Both modes finish with an in-process disk round trip and then forget the recorded lessons, to show they are derived from the
// forest state and not from session state. RV2L_TRACE lines hold no timestamps or ids, so the same gate run on the pre-repair
// production bytes and on the repaired HEAD can be compared line by line.
//
// Copy into Assets/ForestPrototype, run RackV2LegacyProgressionRegression.Begin, then remove the copy and its .meta.
// Save safety: refuses to run unless Application.persistentDataPath is inside XDG_CONFIG_HOME (the runner's isolated config).
public static class RackV2LegacyProgressionRegression
{
    public static void Begin()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        EditorApplication.isPlaying = true;
    }

    // Lessons are device-wide PlayerPrefs; start from none so every recorded lesson is earned in this run.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Prepare()
    {
        if (!Environment.GetCommandLineArgs().Any(a => a == "RackV2LegacyProgressionRegression.Begin")) return;
        foreach (string id in LearningObjectivesView.StepIds) PlayerPrefs.DeleteKey(LearningObjectivesView.PreferencePrefix + id);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (Environment.GetCommandLineArgs().Any(a => a == "RackV2LegacyProgressionRegression.Begin"))
            new GameObject("Disposable rack v2 legacy progression regression").AddComponent<RackV2LegacyProgressionRegressionRunner>();
    }
}

public sealed class RackV2LegacyProgressionRegressionRunner : MonoBehaviour
{
    private ForestEcologyController ecology;
    private ScenarioOneManager manager;
    private ForestSaveController saves;
    private ScenarioOneUiRoot ui;
    private RackPlanner planner;
    private string mode;
    private int checks;

    private static void Require(bool ok, string message) { if (!ok) throw new InvalidOperationException(message); }
    private void Check(bool ok, string message) { Require(ok, message); checks++; Debug.Log("RV2L_CHECK " + message); }
    private static string B(bool value) => value ? "1" : "0";

    private IEnumerator Start()
    {
        mode = Environment.GetEnvironmentVariable("CCF_RV2L_MODE") ?? "";
        for (int i = 0; i < 6; i++) yield return null;
        Exception failure = null;
        var stack = new Stack<IEnumerator>();
        stack.Push(Run());
        while (stack.Count > 0)
        {
            bool more; object current = null;
            try { more = stack.Peek().MoveNext(); if (more) current = stack.Peek().Current; }
            catch (Exception error) { failure = error; break; }
            if (!more) { stack.Pop(); continue; }
            if (current is IEnumerator nested) { stack.Push(nested); continue; }
            yield return current;
        }
        string tag = "RV2L_" + mode.ToUpperInvariant();
        Debug.Log(failure == null ? tag + "_PASS checks=" + checks : tag + "_FAIL checks=" + checks + " " + failure);
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(failure == null ? 0 : 1);
    }

    private IEnumerator Run()
    {
        ecology = FindFirstObjectByType<ForestEcologyController>();
        manager = FindFirstObjectByType<ScenarioOneManager>();
        saves = FindFirstObjectByType<ForestSaveController>();
        ui = FindFirstObjectByType<ScenarioOneUiRoot>();
        Require(ecology != null && manager != null && saves != null && ui != null, "not the Scenario One ForestTest scene");
        string config = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME");
        Require(!string.IsNullOrEmpty(config) && Application.persistentDataPath.StartsWith(config, StringComparison.Ordinal),
            "persistentDataPath must be inside the isolated XDG_CONFIG_HOME: " + Application.persistentDataPath);
        if (mode == "model0") yield return Model0();
        else if (mode == "legacy40") yield return Legacy40();
        else throw new InvalidOperationException("CCF_RV2L_MODE must be model0 or legacy40 (was '" + mode + "')");
    }

    // ---- helpers (the same patterns the combined gate uses) ----------------------------------------------------------
    private IEnumerator Frames(int count) { for (int i = 0; i < count; i++) yield return null; }

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

    private Button BtnStarts(params string[] texts) => ui.RootElement.Query<Button>().ToList()
        .FirstOrDefault(b => b.text != null && texts.Any(t => b.text.StartsWith(t, StringComparison.Ordinal)) && Visible(b));

    private IEnumerator Invoke(Button button)
    {
        Require(button != null, "button not found");
        typeof(Clickable).GetMethod("Invoke", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(button.clickable, new object[] { null });
        yield return null; yield return null;
    }

    private IEnumerator CloseHelp()
    {
        for (int i = 0; i < 6 && ui.Help.IsOpen; i++)
        {
            Button close = ui.Help.Root.Q<Button>("menu-help-close");
            if (close != null) yield return Invoke(close); else ui.CloseHelp();
            yield return null;
        }
    }

    // One deterministic line. It reads only members that also exist on the pre-repair production bytes, so the same gate runs there.
    private string Trace(string step)
    {
        ui.RackLayout.Update(true);
        ui.Learning.Observe();
        ui.Learning.Refresh(true);
        RackLayoutTracker t = ui.RackLayout;
        var orders = manager.WorkOrders;
        string line = "RV2L_TRACE mode=" + mode + " step=" + step
            + " v2=" + B(manager.RackPlanningV2Active) + " model=" + manager.RackPlanningModelVersion + " geometry=" + ecology.StandGeometryModelVersion
            + " total=" + t.Total + " done=" + t.Done + " complete=" + B(t.Complete) + " harvested=" + B(t.Harvested)
            + " progress=" + B(t.HasProgress) + " plannedTrees=" + t.PlannedTrees
            + " create=" + B(ui.Learning.IsDone("rack.create")) + " layout=" + B(ui.Learning.IsDone("rack.layout"))
            + " approve=" + B(ui.Learning.IsDone("rack.approve")) + " result=" + B(ui.Learning.IsDone("rack.result"))
            + " topic=" + ui.Learning.CurrentTopicIndex + " rackGroups=" + manager.RackGroups.Count
            + " rackOwnedOrders=" + orders.Count(o => o.rackId > 0)
            + " fell(pending/approved/completed)=" + orders.Count(o => o.type == ScenarioWorkType.FellTree && o.status == ScenarioWorkStatus.Pending)
            + "/" + orders.Count(o => o.type == ScenarioWorkType.FellTree && o.status == ScenarioWorkStatus.Approved)
            + "/" + orders.Count(o => o.type == ScenarioWorkType.FellTree && o.status == ScenarioWorkStatus.Completed)
            + " interventions=" + manager.Interventions.Count;
        Debug.Log(line);
        return line;
    }

    // Plan one whole-stand rack along the given planted-row line through the OLD per-proposal path.
    private int PlanLegacyRack(float lineZ)
    {
        planner.Refresh();
        RackProposal proposal = planner.Propose(new RackPoint(0f, lineZ), 0f);
        Require(proposal != null && proposal.CanConfirm, "the legacy planner proposes a rack on row z=" + lineZ + ": " + (proposal != null ? proposal.Problem : "null"));
        int added = manager.ConfirmRack(proposal, "verification");
        Require(added > 0, "legacy ConfirmRack adds felling work for row z=" + lineZ + ": " + manager.Feedback);
        return added;
    }

    private IEnumerator ApproveAndAdvance()
    {
        ui.ShowWorkPlan(); yield return Frames(2);
        yield return CloseHelp();
        yield return Wait(0.4f);
        Button approve = BtnStarts("Approve Work Plan");
        Check(approve != null, "the Work Plan offers Approve");
        yield return Invoke(approve);
        yield return Wait(0.7f);
        Trace("approved");
        Button advance = BtnStarts("Advance one year");
        Check(advance != null, "the Work Plan offers Advance one year");
        yield return Invoke(advance);
        yield return Frames(8);
        yield return Wait(0.8f);
        ui.CloseAll(); yield return Frames(2);
    }

    // Forget every recorded lesson and re-derive from the loaded forest; also proves the world kept its model across the disk trip.
    private IEnumerator RoundTripAndRederive(string expectedLessons)
    {
        saves.Save();
        yield return Frames(2);
        Check(saves.CanLoad(out string problem), "the saved forest is loadable: " + problem);
        saves.Load();
        yield return Frames(8);
        yield return CloseHelp();
        foreach (string id in LearningObjectivesView.StepIds) PlayerPrefs.DeleteKey(LearningObjectivesView.PreferencePrefix + id);
        ui.Learning.ReloadProgress();
        Check(!ui.Learning.IsDone("rack.create") && !ui.Learning.IsDone("rack.result"), "lessons were forgotten before re-deriving");
        yield return Wait(0.6f);
        string after = Trace("after-load-rederived");
        Check(!manager.RackPlanningV2Active && manager.RackPlanningModelVersion == 0, "the loaded world is still model 0 (a legacy save never becomes V2)");
        Check(after.Contains(expectedLessons), "the lessons re-derive from the loaded forest: " + expectedLessons);
    }

    // ---- model 0: historical Enlarged80 with the authored layout ----------------------------------------------------------
    private IEnumerator Model0()
    {
        // The historical model-0 world: a fresh game is now a V2 world, so regenerate the enlarged stand with the rack plan cleared.
        var stand = FindFirstObjectByType<ForestStartingStand>();
        Require(stand != null, "ForestStartingStand is present");
        manager.InitializeNewScenario();
        foreach (var tree in FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None)) DestroyImmediate(tree.gameObject);
        ecology.ApplyStandGeometry(StandGeometryModel.Enlarged80);
        ecology.ResetForDeterministicRun();
        stand.Generate();
        yield return Frames(3);
        yield return CloseHelp();
        Check(!manager.RackPlanningV2Active && manager.RackPlanningModelVersion == 0, "the world is historical model 0 (not a Rack Planning V2 world)");
        Check(ecology.StandGeometryModelVersion == StandGeometryModel.Enlarged80, "the world is Enlarged80");
        planner = ui.RackTool.Planner;
        planner.Refresh();
        Check(planner.LayoutLines.Count == 5, "the Enlarged80 stand has five authored layout lines");

        Trace("start");
        Check(ui.RackLayout.Total == 5 && ui.RackLayout.Done == 0 && !ui.RackLayout.Complete, "the tracker counts the five authored lines, none planned");
        Check(!ui.RackLayout.HasProgress, "no progress before anything is planned");

        var lines = planner.LayoutLines.ToList();
        for (int i = 0; i < lines.Count; i++)
        {
            PlanLegacyRack(lines[i]);
            Trace("planned-" + (i + 1));
        }
        Check(manager.RackGroups.Count == 5, "five legacy rack groups were confirmed");
        Check(ui.RackLayout.Complete && ui.RackLayout.Done == 5, "the authored layout is complete once every line is planned");
        Check(ui.Learning.IsDone("rack.create") && ui.Learning.IsDone("rack.layout"), "rack.create and rack.layout follow the authored lines");
        Check(!ui.Learning.IsDone("rack.approve"), "rack.approve is not recorded while the work is still pending");
        Check(manager.WorkOrders.All(o => o.rackId == 0), "model-0 orders carry no rack id");

        yield return ApproveAndAdvance();
        Trace("executed");
        Check(ui.Learning.IsDone("rack.approve"), "rack.approve is recorded once no felling is pending (rack ids play no part in model 0)");
        Check(ui.RackLayout.Harvested, "every authored line is harvested after the year advance");
        Check(ui.Learning.IsDone("rack.result"), "rack.result is recorded after the harvest");
        Check(manager.Interventions.Count == 1, "the first intervention was recorded");
        Check(!ui.Learning.TeachingRacks && ui.Learning.CurrentTopicIndex >= 1, "teaching has moved on from the rack lesson");

        yield return RoundTripAndRederive("create=1 layout=1 approve=1 result=1");
    }

    // ---- Legacy40: no authored layout ------------------------------------------------------------------------------------
    private IEnumerator Legacy40()
    {
        Check(StandGeometryPolicy.NewGameModelForSession == StandGeometryModel.Legacy40 && ecology.StandGeometryModelVersion == StandGeometryModel.Legacy40,
            "a fresh game is the Legacy40 world (CCF_STAND_GEOMETRY=0)");
        yield return CloseHelp();
        Check(!manager.RackPlanningV2Active && manager.RackPlanningModelVersion == 0, "Legacy40 is model 0");
        planner = ui.RackTool.Planner;
        planner.Refresh();
        Check(planner.LayoutLines.Count == 0, "Legacy40 has no authored layout lines");

        Trace("start");
        Check(ui.RackLayout.Total == 0 && !ui.RackLayout.Complete && !ui.RackLayout.HasProgress, "the tracker is empty on a stand with no layout");

        var rows = planner.Rows(0f);
        Require(rows.Count > 2, "Legacy40 has planted rows");
        PlanLegacyRack(rows[rows.Count / 2].Centre);
        Trace("planned");
        Check(manager.RackGroups.Count == 1 && ui.Learning.IsDone("rack.create"), "rack.create follows the legacy rack group");
        Check(!ui.Learning.IsDone("rack.layout") && !ui.Learning.IsDone("rack.approve"), "no layout, so no layout/approve credit");

        yield return ApproveAndAdvance();
        Trace("executed");
        Check(!ui.Learning.IsDone("rack.layout") && !ui.Learning.IsDone("rack.approve") && !ui.Learning.IsDone("rack.result"),
            "Legacy40 never records layout/approve/result (the tracker has no layout)");
        Check(manager.Interventions.Count == 1, "the first intervention was recorded");
        Check(!ui.Learning.TeachingRacks && ui.Learning.CurrentTopicIndex >= 1, "teaching moves on from the first completed felling");

        yield return RoundTripAndRederive("create=1 layout=0 approve=0 result=0");
    }
}
#endif
