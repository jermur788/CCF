#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using CCF.Forestry.WorkEconomy;

// Disposable in-Editor gate: Scenario One management history and repeated interventions (S1-C / S1-D).
//
// STATUS: original Unity run stopped at an obsolete hint assertion; the Phase-2 correction rerun PASSED on
// 2026-10-16 at tooling HEAD ec9068371eae998774ca93dc75c8ff5b74d5a743 — SCENARIO_HISTORY_VERIFY_PASS assertions=43,
// exit 0, clean shutdown (evidence: ScenarioOneCandidateV2-Phase2FocusedRerun-20261016T070954Z).
// The derivation rules are executed offline against the real record classes (OfflineLogic/ScenarioHistoryOfflineCheck);
// this gate covers what only Unity can: that real play produces the records those rules read, that three cycles behave
// the same as one, that no recurring review gate appears, that old open orders are surfaced and kept, and that the history
// is identical after a save and load because it is derived from persisted records.
//
// Copy into Assets/ForestPrototype, run with
//   CCF_STAND_GEOMETRY=0 <UNITY> -batchmode -nographics -executeMethod ScenarioHistoryVerification.Begin
// then remove the copy and its .meta. Saves are captured/loaded in memory only.
public static class ScenarioHistoryVerification
{
    public static void Begin()
    {
        EditorPrefs.SetBool("ScenarioHistoryVerification.Run", true);
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        EditorApplication.isPlaying = true;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (!EditorPrefs.GetBool("ScenarioHistoryVerification.Run", false)) return;
        EditorPrefs.SetBool("ScenarioHistoryVerification.Run", false);
        new GameObject("Scenario history gate").AddComponent<ScenarioHistoryGate>();
    }
}

public sealed class ScenarioHistoryGate : MonoBehaviour
{
    private ScenarioOneManager manager;
    private ForestSaveController saves;
    private ForestEcologyController ecology;
    private ForestTreeMarkingManager marks;
    private ForestSaveData original;
    private string originalHash;
    private int assertions;
    private readonly List<string> markedTargets = new List<string>();

    private void Check(bool ok, string what) { assertions++; Debug.Log($"SCENARIO_HISTORY_{(ok ? "PASS" : "FAIL")} {what}"); if (!ok) throw new InvalidOperationException(what); }
    private ForestSaveData Clone(ForestSaveData d) => JsonUtility.FromJson<ForestSaveData>(JsonUtility.ToJson(d));

    private IEnumerator Start()
    {
        yield return null;
        manager = FindFirstObjectByType<ScenarioOneManager>(); saves = FindFirstObjectByType<ForestSaveController>();
        ecology = FindFirstObjectByType<ForestEcologyController>(); marks = FindFirstObjectByType<ForestTreeMarkingManager>();
        original = saves.CaptureData(); originalHash = ScenarioReferenceArchive.WorldHash(original);
        Exception failure = null;
        var stack = new Stack<IEnumerator>(); stack.Push(Verify());
        while (stack.Count > 0)
        {
            bool more; object current = null;
            try { more = stack.Peek().MoveNext(); if (more) current = stack.Peek().Current; }
            catch (Exception error) { failure = error; break; }
            if (!more) { stack.Pop(); continue; }
            if (current is IEnumerator nested) stack.Push(nested); else yield return current;
        }
        bool restored = saves.LoadData(original, false);
        yield return null; yield return null;
        if (!restored || ScenarioReferenceArchive.WorldHash(saves.CaptureData()) != originalHash)
            failure = failure ?? new InvalidOperationException("Gate failed to restore the original world.");
        if (failure == null) Debug.Log("SCENARIO_HISTORY_VERIFY_PASS assertions=" + assertions);
        else Debug.LogError("SCENARIO_HISTORY_VERIFY_FAIL " + failure);
        EditorApplication.ExitPlaymode(); EditorApplication.Exit(failure == null ? 0 : 1);
    }

    private ForestTree[] Trees() => FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
        .Where(tree => tree.IsLiving && tree.TreeId.StartsWith("P", StringComparison.Ordinal)).OrderBy(tree => tree.TreeId, StringComparer.Ordinal).ToArray();

    private ScenarioOneWorkOrder Mark(ForestTree tree, FellingMaterialOutcome outcome)
    {
        markedTargets.Add(tree.TreeId);
        manager.PlanningFellingOutcome = outcome; marks.Mark(tree, TreeMarkType.Fell, false);
        manager.AddMarkedTreesToWorkPlan();
        return manager.WorkOrders.Single(order => order.type == ScenarioWorkType.FellTree && order.targetTreeId == tree.TreeId && order.IsOpen);
    }

    private Vector3 PlantPosition()
    {
        var trees = Trees();
        foreach (var cell in ecology.Cells.OrderByDescending(cell => cell.Light).ThenBy(cell => cell.Center.x).ThenBy(cell => cell.Center.y))
            foreach (float x in new[] { -1.5f, 0f, 1.5f }) foreach (float z in new[] { -1.5f, 0f, 1.5f })
            {
                var point = new Vector3(cell.Center.x + x, 0f, cell.Center.y + z);
                if (trees.All(tree => Vector2.Distance(new Vector2(tree.transform.position.x, tree.transform.position.z), new Vector2(point.x, point.z)) >= 0.8f)) return point;
            }
        throw new InvalidOperationException("no planting position");
    }

    private string Fingerprint(IEnumerable<InterventionSummary> list) => string.Join("|", list.Select(i =>
        $"{i.Ordinal}:{i.Year}:{i.Felled}:{i.Pruned}:{i.Planted}:{i.PlantedSheltered}:{i.RegenerationRemoved}:{i.TasksFailed}:{i.CostCents}:{i.RevenueCents}:{i.OtherDeathsInYear}:{i.OtherDeathsSince}:{i.Before?.year}:{i.After?.year}:{i.Latest?.year}"));

    private IEnumerator Verify()
    {
        Check(ForestSaveData.CurrentVersion == 23, "save version unchanged");
        Check(saves.LoadData(Clone(original), false), "restore baseline");
        yield return null; yield return null;
        manager.InitializeNewScenario();
        ScenarioOneUiRoot ui = FindFirstObjectByType<ScenarioOneUiRoot>();
        Check(manager.Interventions.Count == 0 && manager.CompletionReview == null, "a new game has no interventions and no completion review");
        Check(manager.NextStepHint.StartsWith("No intervention yet"), "hint before any intervention");

        // ---- cycle 1: thin three, plant one sheltered oak ----
        var first = Trees();
        Mark(first[0], FellingMaterialOutcome.SellAndExtract); Mark(first[1], FellingMaterialOutcome.SellAndExtract); Mark(first[2], FellingMaterialOutcome.SellAndExtract);
        Check(manager.TryPurchaseStock("sessile-oak-sapling", 1), "buy oak stock");
        Check(manager.TryDesignateExactPlanting("sessile-oak-sapling", PlantPosition(), WorkExecutionMethod.LandownerSimulated, true), "designate sheltered planting");
        Check(manager.ApprovePendingWork() && manager.AdvanceYear(), "cycle 1 resolves");
        yield return null;
        var after1 = manager.Interventions;
        Check(after1.Count == 1 && after1[0].Ordinal == 1 && after1[0].Year == 1 && after1[0].Felled == 3 && after1[0].Planted == 1 && after1[0].PlantedSheltered == 1 && after1[0].TasksFailed == 0,
            $"first intervention derived from real records: felled={after1[0].Felled} planted={after1[0].Planted}");
        Check(after1[0].CostCents > 0 && after1[0].Before != null && after1[0].Before.year == 0 && after1[0].After.year == 1 && after1[0].Latest == null, "money and the before/after snapshots exist; nothing 'since' yet");
        Check(manager.Interventions.Count == 1, "reading the history twice does not duplicate it");

        // ---- the first review is the only gate ----
        if (ui != null) Check(ui.NeedsAnnualReview, "the first review is pending after the first year");
        manager.MarkAnnualReviewSeen();
        if (ui != null) Check(!ui.NeedsAnnualReview, "acknowledging once clears the gate");

        // ---- idle years are not interventions and never gate ----
        Check(manager.AdvanceYear() && manager.AdvanceYear(), "two idle years advance without any further acknowledgement");
        if (ui != null) Check(!ui.NeedsAnnualReview, "no recurring review gate");
        Check(manager.Interventions.Count == 1 && manager.Interventions[0].Latest != null && manager.Interventions[0].Latest.year == 3 && manager.Interventions[0].YearsObserved == 2,
            "idle years add no intervention but extend 'since'");
        Check(manager.CompletionGuidance == CompletionGuidanceState.AnotherIntervention,
            "reviewed first cycle with unmet objectives asks for another intervention");
        Check(manager.NextStepHint == ScenarioOneCompletionGuidance.GuidanceText(CompletionGuidanceState.AnotherIntervention),
            "U4 two-cycle guidance takes priority over the older idle-years hint (acknowledgement year >= first year)");
        string historyOnlyHint = ScenarioInterventionHistory.NextStepHint(manager.Interventions, 3, 0, false, -1, manager.CenturyReviewYear, null);
        Check(historyOnlyHint.Contains("2 years since intervention 1") && historyOnlyHint.Contains("plan intervention 2"),
            "older idle-years wording remains available independently when guidance is null");

        // ---- cycle 2: felling left as deadwood; its trees are not 'other deaths' ----
        var second = Trees();
        Mark(second[0], FellingMaterialOutcome.RetainAsFallenDeadwood); Mark(second[1], FellingMaterialOutcome.RetainAsFallenDeadwood);
        Check(manager.ApprovePendingWork() && manager.AdvanceYear(), "cycle 2 resolves");
        yield return null;
        var after2 = manager.Interventions;
        Check(after2.Count == 2 && after2[1].Ordinal == 2 && after2[1].Year == 4 && after2[1].Felled == 2 && after2[1].Before.year == 3 && after2[1].After.year == 4, "second intervention is a new entry with its own snapshots");
        Check(after2[0].Felled == 3 && after2[0].Year == 1, "the first intervention is unchanged by the second");
        Check(ScenarioOneCompletionRules.HasReviewedTwoCycleSequence(manager.ManagementEvents, manager.WorkOrders)
            && manager.CompletionGuidance == CompletionGuidanceState.None,
            "second resolved cycle follows the review chronologically and clears U4 guidance");

        // ---- an old open order is surfaced and kept ----
        var third = Trees();
        ScenarioOneWorkOrder old = Mark(third[0], FellingMaterialOutcome.SellAndExtract);
        int orderCount = manager.WorkOrders.Count;
        Check(manager.StaleOrders.Count == 0, "a fresh order is not stale");
        Check(manager.AdvanceYear() && manager.AdvanceYear(), "two years pass with the order left pending");
        Check(manager.WorkOrders.Count == orderCount && old.IsOpen, "the old order is neither expired nor deleted");
        var stale = manager.StaleOrders;
        Check(stale.Count == 1 && stale[0].Order.workOrderId == old.workOrderId && stale[0].AgeYears >= 2 && stale[0].Reason.Contains("years ago"), "the old order is surfaced with its age: " + (stale.Count > 0 ? stale[0].Reason : "none"));
        Check(manager.NextStepHint.StartsWith("1 planned order from earlier needs attention"), "hint puts the stale order first");

        // ---- cycle 3 uses the same machinery ----
        Check(manager.ApprovePendingWork() && manager.AdvanceYear(), "cycle 3 resolves the old order");
        yield return null;
        var after3 = manager.Interventions;
        Check(after3.Count == 3 && after3.Select(i => i.Ordinal).SequenceEqual(new[] { 1, 2, 3 }) && after3[2].Felled == 1, "third intervention: no two-cycle special case");
        Check(manager.StaleOrders.Count == 0, "no stale orders remain once it is resolved");
        // Independent recount of 'other deaths' in the second plan's year: deadwood stamped with that year minus the two stems the plan felled.
        var secondFelled = new HashSet<string> { second[0].TreeId, second[1].TreeId };
        int independent = manager.DeadwoodRecords.Count(r => r.fallenYear == 4 && !secondFelled.Contains(r.treeId));
        Check(after3[1].OtherDeathsInYear == independent, $"other deaths in the second plan's year match an independent recount ({independent}); felled-and-left stems are not counted");
        Check(manager.DeadwoodRecords.Count(r => r.fallenYear == 4 && secondFelled.Contains(r.treeId)) == 2, "both stems left as deadwood by plan 2 are on record for that year");

        // ---- the history comes from persisted records: identical after save and load ----
        string before = Fingerprint(manager.Interventions);
        ForestSaveData data = saves.CaptureData();
        Check(data.version == 23, "the save is still version 23");
        Check(saves.LoadData(Clone(data), false), "reload");
        yield return null; yield return null;
        Check(Fingerprint(manager.Interventions) == before, "history is identical after a save and load");
        Check(manager.ManagementEvents.Count == data.scenarioOne.managementEvents.Count, "no extra records appeared on load");
        Check(markedTargets.Count == 6 && markedTargets.All(id => manager.WorkOrders.Any(order =>
            order.type == ScenarioWorkType.FellTree && order.targetTreeId == id && order.status == ScenarioWorkStatus.Completed)),
            "all six marked trees across three cycles retain their completed work orders after reload");

        // ---- completion summary at the actual completion year (the outcome is forced; objective logic is not under test) ----
        Check(manager.CompletionReview == null, "no completion summary while active");
        typeof(ScenarioOneManager).GetMethod("SetOutcome", BindingFlags.Instance | BindingFlags.NonPublic)
            .Invoke(manager, new object[] { ScenarioOneOutcome.Completed, manager.CurrentEcologicalYear, "forced for the gate" });
        CompletionSummary completion = manager.CompletionReview;
        Check(completion != null && completion.Year == manager.CurrentEcologicalYear && completion.Year < manager.CenturyReviewYear, "completion summary is for the completion year, not year 100");
        Check(completion.Interventions.Count == 3 && completion.TotalFelled == after3.Sum(i => i.Felled) && completion.StartingCashCents == manager.Definition.StartingCashCents, "completion totals match the interventions");
        Check(ManagementHistoryText.Completion(completion, new HistoryTextContext()).Count >= 5, "completion wording is produced");
        Check(manager.NextStepHint.StartsWith("Objectives met in year"), "hint after completion points to the summary");
        Check(manager.CanManage_ForGate(), "management stays available after completion until the Century Review");
    }
}

// Reads the private CanManage() without changing production code.
internal static class ScenarioHistoryGateExtensions
{
    public static bool CanManage_ForGate(this ScenarioOneManager manager)
        => (bool)typeof(ScenarioOneManager).GetMethod("CanManage", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(manager, null);
}
#endif
