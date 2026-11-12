#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

// Invoke from a separately authorised qualification runner on authentic post-teaching snapshots.
// No auto-start, disk writes or save-slot access. The caller owns restoring its original world.
public static class CadenceParity
{
    private static void Require(bool condition, string message)
    { if (!condition) throw new Exception("CADENCE FAIL: " + message); }
    private static void Set(ScenarioOneUiRoot ui, string field, object value)
        => typeof(ScenarioOneUiRoot).GetField(field, BindingFlags.NonPublic | BindingFlags.Instance).SetValue(ui, value);
    private static string Json(ForestSaveData data)
    { ScenarioReferenceArchive.Canonicalize(data); return JsonUtility.ToJson(data); }
    private static ForestSaveData Clone(ForestSaveData data) => JsonUtility.FromJson<ForestSaveData>(JsonUtility.ToJson(data));
    private static string Normalize(ForestSaveData data)
    {
        data = Clone(data);
        data.scenarioOne.managementEvents.RemoveAll(e => e.eventType == ScenarioManagementEventType.AnnualReviewAcknowledged);
        int id = 1;
        foreach (var e in data.scenarioOne.managementEvents) e.eventId = id++;
        data.scenarioOne.nextManagementEventId = id;
        // Review flags are NOT filtered. Compare both paths immediately BEFORE the destination acknowledgment.
        return Json(data);
    }

    // Run twice: authentic quiet post-teaching and authentic post-felling state, each N=1..5.
    // The test-only target override exercises shorter ranges without altering production year suggestions.
    public static IEnumerator Run(ScenarioOneUiRoot ui, ForestSaveController saves, ForestSaveData authentic)
    {
        Require(authentic != null, "authentic fixture required");
        for (int n = 1; n <= 5; n++)
        {
            Require(saves.LoadData(Clone(authentic), false), "restore direct");
            var direct = new List<ForestSaveData>();
            for (int i = 0; i < n; i++)
            { Require(ui.Manager.AdvanceYear(), "direct tick"); direct.Add(saves.CaptureData()); }
            Require(saves.LoadData(Clone(authentic), false), "restore skip");
            ui.ShowWorkPlan(); ui.Learning.Refresh(true);
            Require(string.IsNullOrEmpty(ui.SkipProblem()), "fixture must genuinely qualify: " + ui.SkipProblem());
            Require(ui.BeginSkip(ui.SkipTarget(out _)), "begin skip");
            Require(!ui.BeginSkip(ui.SkipTarget(out _)), "double click rejected");
            Set(ui, "skipTarget", authentic.ecologicalYear + n);
            int previous = authentic.ecologicalYear, k = 0;
            while (ui.Skipping)
            {
                yield return null;
                if (ui.Manager.CurrentEcologicalYear == previous) continue;
                Require(ui.Manager.CurrentEcologicalYear == previous + 1, "one tick per frame");
                var actual = saves.CaptureData();
                Require(Json(Clone(direct[k])) == Json(Clone(actual)), "A full JSON tick " + k);
                Require(ScenarioReferenceArchive.CurrentWorldHash(direct[k]) == ScenarioReferenceArchive.CurrentWorldHash(actual), "A hash tick " + k);
                previous++; k++;
            }
            Require(k > 0 && k <= n, "actual stop length");
            var skipped = saves.CaptureData();
            Require(saves.LoadData(Clone(authentic), false), "restore UI manual");
            for (int i = 0; i < k; i++)
            {
                ui.ShowWorkPlan();
                Require(ui.AdvanceFromWorkPlan(), "manual UI tick");
                if (i < k - 1) ui.AcknowledgeAnnualReview(); // genuine route, never synthesized by skip
            }
            Require(Normalize(skipped) == Normalize(saves.CaptureData()), "B normalized UI parity");
            ui.AcknowledgeAnnualReview();
            Require(ui.Manager.AnnualReviewSeen, "real destination acknowledgment");
            Debug.Log("CADENCE PARITY A+B PASS requested=" + n + " actual=" + k);
        }
    }

    public static void WorkOrderNegatives(ScenarioOneUiRoot ui)
    {
        var original = JsonUtility.ToJson(ui.Manager.CaptureSaveData());
        try
        {
            foreach (ScenarioWorkStatus status in new[] { ScenarioWorkStatus.Pending, ScenarioWorkStatus.Approved })
                foreach (bool invalid in new[] { false, true })
                {
                    var state = JsonUtility.FromJson<ScenarioOneSaveData>(original);
                    state.workOrders.Add(new ScenarioOneWorkOrder { workOrderId = state.nextWorkOrderId++, status = status,
                        type = ScenarioWorkType.FellTree, prescriptionId = 1, validationMessage = invalid ? "invalid fixture" : "" });
                    ui.Manager.RestoreSaveData(state);
                    ui.Learning.Refresh(true);
                    Reject(ui, "Pending and Approved");
                }
        }
        finally { ui.Manager.RestoreSaveData(JsonUtility.FromJson<ScenarioOneSaveData>(original)); ui.Learning.Refresh(true); }
    }

    // Injection is a test boundary: mutate only the disposable fixture, then expect no further annual tick.
    public static IEnumerator RunStop(ScenarioOneUiRoot ui, Action inject, string reason, int completedTicks = 0)
    {
        var original = UnityEngine.Object.FindObjectsByType<Behaviour>(FindObjectsSortMode.None)
            .Where(b => b is ForestSaveController || b is StandaloneSessionMenu || b is ForestPlayer
                || b is ForestTreeMarkingManager || b is RackTool).ToDictionary(b => b, b => b.enabled);
        Require(ui.BeginSkip(ui.SkipTarget(out _)), "begin stop fixture");
        int start = ui.Manager.CurrentEcologicalYear;
        Require(original.Keys.All(b => !b.enabled), "all input components locked");
        while (ui.Skipping && ui.Manager.CurrentEcologicalYear < start + completedTicks) yield return null;
        Require(ui.Skipping, "fixture stopped before injection");
        inject();
        int before = ui.Manager.CurrentEcologicalYear;
        while (ui.Skipping) yield return null;
        Require(ui.Manager.CurrentEcologicalYear == before, "stop before next tick: " + reason);
        Require(original.All(pair => pair.Key.enabled == pair.Value), "input enabled states restored: " + reason);
    }

    public static IEnumerator Cancel(ScenarioOneUiRoot ui, int afterTicks)
    {
        var test = RunStop(ui, () => Set(ui, "cancelSkip", true), "cancel", afterTicks);
        while (test.MoveNext()) yield return test.Current;
    }

    // Caller supplies authentic or deliberately injected states, never rewrites production completion rules.
    public static void Reject(ScenarioOneUiRoot ui, string expectedReason)
    {
        string before = JsonUtility.ToJson(ui.Manager.CaptureSaveData());
        Require(ui.SkipProblem().Contains(expectedReason), "negative reason " + expectedReason);
        Require(!ui.BeginSkip(ui.SkipTarget(out _)), "negative cannot begin");
        Require(before == JsonUtility.ToJson(ui.Manager.CaptureSaveData()), "guard is read only");
    }
}
#endif
