using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Disposable Tier-2 gate for RACK-V2-03C: the authoritative rack runtime and D-064 work-order lifecycle.
// Copy into Assets/ForestPrototype, run RackV2RuntimeVerification.Begin, then remove the copy and its
// .meta. It never writes the player's save file and never saves the scene: every round trip is in memory.
//
// Covers: create/confirm, confirm never approves or executes, reconfirm unchanged, edit/delete before
// approval with pending reconciliation, approved-but-unexecuted warn/cancel/recreate, completed-linked
// locking, entrance-mark state and geometry invalidation, save/load of confirmed geometry and links,
// and the model-0 legacy path after a pre-v21 load.
public static class RackV2RuntimeVerification
{
#if UNITY_EDITOR
    public static void Begin()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        EditorApplication.isPlaying = true;
    }
#endif

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (Environment.GetCommandLineArgs().Any(a => a == "RackV2RuntimeVerification.Begin"))
            new GameObject("Disposable rack v2 runtime verification").AddComponent<RackV2RuntimeVerificationRunner>();
    }
}

public sealed class RackV2RuntimeVerificationRunner : MonoBehaviour
{
    private ForestEcologyController ecology;
    private ScenarioOneManager manager;
    private ForestSaveController saves;
    private RackPlanner planner;
    private readonly List<string> log = new List<string>();

    private static void Check(bool ok, string message) { if (!ok) throw new InvalidOperationException(message); }
    private void Pass(string message) { log.Add(message); Debug.Log("RACKV2RUNTIME_CHECK " + message); }

    private int OpenRackOrders() => manager.WorkOrders.Count(order => order.IsOpen && order.rackId > 0);
    private int AllOpenFellOrders() => manager.WorkOrders.Count(order => order.IsOpen && order.type == ScenarioWorkType.FellTree);
    private int LivingTrees() => FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).Count(tree => tree != null && tree.IsLiving);

    private IEnumerator Start()
    {
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
        Debug.Log(failure == null ? "RACKV2RUNTIME_VERIFY_PASS checks=" + log.Count : "RACKV2RUNTIME_VERIFY_FAIL: " + failure);
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(failure == null ? 0 : 1);
#endif
    }

    private IEnumerator Run()
    {
        ecology = FindFirstObjectByType<ForestEcologyController>();
        manager = FindFirstObjectByType<ScenarioOneManager>();
        saves = FindFirstObjectByType<ForestSaveController>();
        Check(ecology != null && manager != null && saves != null, "not the Scenario One ForestTest scene");
        Check(StandGeometryPolicy.NewGameModelForSession == StandGeometryModel.Enlarged80, "gate requires the production Enlarged80 new-game policy");
        Check(ecology.StandGeometryModelVersion == StandGeometryModel.Enlarged80, "fresh world is Enlarged80");
        Check(manager.RackPlanningV2Active && manager.RackPlanningModelVersion == RackPlanningModel.RackPlanningFieldMarkingV2,
            "fresh Enlarged80 world is Rack Planning V2");
        Check(manager.PlannedRackCount == 0 && manager.ConfirmedRacks.Count == 0 && manager.NextRackId == 1,
            "a fresh V2 world starts with an empty draft plan");
        Pass("fresh V2 identity and empty draft plan");

        planner = new RackPlanner(manager, ecology);
        planner.Refresh();
        Check(planner.LayoutLines.Count == 5, "the qualified Enlarged80 stand has five authored layout lines");
        var lines = new List<float>(planner.LayoutLines);
        var original = new List<(RackPoint Start, RackPoint End)>();
        foreach (float line in lines)
        {
            Check(RackGeometry.ClipLineToSquare(new RackPoint(0f, line), 0f, planner.HalfExtentM, out RackPoint start, out RackPoint end),
                "layout line clips to the stand");
            original.Add((start, end));
        }

        // ---- A. create and confirm ---------------------------------------------------------------
        int livingBeforeConfirm = LivingTrees();
        var ids = new List<int>();
        for (int i = 0; i < original.Count; i++)
        {
            RackPlanOperationResult add = manager.AddRack(original[i].Start, original[i].End);
            Check(add.Succeeded && add.RacksAdded == 1, "rack " + (i + 1) + " adds: " + add.Message);
            ids.Add(add.RackId);
        }
        Check(manager.PlannedRackCount == 5 && manager.RackPlan.Count == 5, "five draft racks are authoritative");
        Check(manager.ConfirmedRacks.Count == 0 && !manager.RackPlanConfirmed, "adding racks does not confirm the plan");
        Check(OpenRackOrders() == 0 && AllOpenFellOrders() == 0, "adding racks creates no work orders");
        Check(manager.RackPlan.All(rack => rack.RemovalTreeIds.Count > 0), "every draft rack has a candidate removal set");
        Check(manager.RackPlan.All(rack => rack.RemovalTreeIds.Contains(rack.EntranceTreeId)), "every draft entrance tree is in its removal set");
        Check(manager.RackPlan.All(rack => !rack.IsLocked), "fresh racks are unlocked");
        Pass("five draft racks created without work");

        RackPlanOperationResult confirm = manager.ConfirmRackPlan();
        Check(confirm.Succeeded && confirm.Status == RackPlanOperationStatus.Applied, "confirm applies: " + confirm.Message);
        Check(manager.RackPlanConfirmed && manager.ConfirmedRacks.Count == 5, "confirm marks the plan authoritative");
        Check(manager.RackPlan.All(rack => rack.Status == RackPlanStatus.Confirmed), "all racks report Confirmed");
        Check(OpenRackOrders() == manager.RackPlan.Sum(rack => rack.RemovalTreeIds.Count), "one open order per frozen removal tree");
        Check(manager.WorkOrders.Where(order => order.rackId > 0).All(order => order.status == ScenarioWorkStatus.Pending),
            "confirm creates only Pending work");
        Check(manager.WorkOrders.Where(order => order.rackId > 0).All(order => order.status != ScenarioWorkStatus.Approved
            && order.status != ScenarioWorkStatus.Completed), "confirm never approves or executes");
        Check(LivingTrees() == livingBeforeConfirm, "confirm fells no tree");
        Check(manager.MarkedRackEntranceCount == 0, "no entrance is marked at confirmation");
        Pass("confirm creates Pending rack-owned work only");

        string confirmedSignature = Signature();
        int orderCount = manager.WorkOrders.Count;

        // ---- B. reconfirm unchanged --------------------------------------------------------------
        RackPlanOperationResult again = manager.ConfirmRackPlan();
        Check(again.Status == RackPlanOperationStatus.NoChange, "unchanged reconfirm is a no-op: " + again.Message);
        Check(Signature() == confirmedSignature, "unchanged reconfirm preserves ids, geometry, sets, entrance trees and order ids");
        Check(manager.WorkOrders.Count == orderCount, "unchanged reconfirm creates no duplicate orders");
        Pass("reconfirm unchanged is exact");

        // ---- C. edit before approval -------------------------------------------------------------
        int editedRack = ids[0];
        RackSaveData editedRecord = manager.Racks.First(rack => rack.rackId == editedRack);
        RackPoint editedEntrance = new RackPoint(editedRecord.entranceX, editedRecord.entranceZ);
        RackPoint editedFar = new RackPoint(editedRecord.farX, editedRecord.farZ);
        RackPoint shortenedFar = new RackPoint(
            editedEntrance.X + (editedFar.X - editedEntrance.X) * 0.25f,
            editedEntrance.Z + (editedFar.Z - editedEntrance.Z) * 0.25f);
        var otherOrderIds = manager.WorkOrders.Where(order => order.rackId > 0 && order.rackId != editedRack)
            .Select(order => order.workOrderId).OrderBy(id => id).ToList();
        int oldSetCount = editedRecord.removalTreeIds.Count;

        RackPlanOperationResult edit = manager.UpdateRack(editedRack, editedEntrance, shortenedFar);
        Check(edit.Succeeded && edit.Status == RackPlanOperationStatus.Applied, "pre-approval edit applies: " + edit.Message);
        RackSaveData afterEdit = manager.Racks.First(rack => rack.rackId == editedRack);
        Check(afterEdit.removalTreeIds.Count < oldSetCount && afterEdit.removalTreeIds.Count > 0, "edited rack membership shrinks with its geometry");
        Check(manager.RackOrderState(editedRack).Open == afterEdit.removalTreeIds.Count,
            "edited rack has exactly one open order per removal tree");
        Check(manager.RackOrderState(editedRack).Pending == afterEdit.removalTreeIds.Count
            && manager.RackOrderState(editedRack).Approved == 0, "pre-approval reconciliation keeps work Pending");
        Check(otherOrderIds.SequenceEqual(manager.WorkOrders.Where(order => order.rackId > 0 && order.rackId != editedRack)
            .Select(order => order.workOrderId).OrderBy(id => id)), "editing one rack leaves every other rack's order ids stable");
        var editedTreeIds = new HashSet<string>(afterEdit.removalTreeIds);
        Check(manager.WorkOrders.Where(order => order.rackId == editedRack).All(order => editedTreeIds.Contains(order.targetTreeId)),
            "no stale order remains outside the edited removal set");
        Pass("pre-approval edit reconciles affected pending work without duplicates");

        RackPlanOperationResult restore = manager.UpdateRack(editedRack, editedEntrance, editedFar);
        Check(restore.Succeeded && restore.Status == RackPlanOperationStatus.Applied, "restoring the line applies");
        RackSaveData restored = manager.Racks.First(rack => rack.rackId == editedRack);
        Check(restored.removalTreeIds.Count == oldSetCount, "restoring the line restores the frozen membership");
        Check(manager.RackOrderState(editedRack).Open == oldSetCount, "restoring the line restores one open order per tree");
        Check(!manager.WorkOrders.GroupBy(order => order.workOrderId).Any(group => group.Count() > 1), "order ids are unique after reconciliation");
        Pass("restoring the pre-approval edit recreates only the missing orders");

        // ---- D. delete before approval -----------------------------------------------------------
        int deletedRack = ids[4];
        int deletedTreeCount = manager.Racks.First(rack => rack.rackId == deletedRack).removalTreeIds.Count;
        int beforeDeleteOrders = manager.WorkOrders.Count;
        RackPlanOperationResult delete = manager.DeleteRack(deletedRack);
        Check(delete.Succeeded && delete.RacksRemoved == 1, "delete applies: " + delete.Message);
        Check(manager.Racks.All(rack => rack.rackId != deletedRack), "deleted rack record is gone");
        Check(!manager.WorkOrders.Any(order => order.rackId == deletedRack), "deleted rack leaves no linked order");
        Check(manager.WorkOrders.Count == beforeDeleteOrders - deletedTreeCount, "delete removes exactly the rack's pending orders");
        Check(manager.PlannedRackCount == 4 && !manager.RackPlanConfirmed, "deleting from a confirmed plan reopens it");
        Pass("delete before approval removes the rack and its pending work");

        RackPoint deletedStart = original[4].Start, deletedEnd = original[4].End;
        RackPlanOperationResult readd = manager.AddRack(deletedStart, deletedEnd);
        Check(readd.Succeeded && readd.RackId != deletedRack, "the deleted line can be added again with a new stable id");
        ids[4] = readd.RackId;
        Check(manager.ConfirmRackPlan().Succeeded && manager.RackPlanConfirmed, "the five-rack plan can be confirmed again");
        Check(!manager.WorkOrders.GroupBy(order => order.workOrderId).Any(group => group.Count() > 1), "no duplicate order ids after delete and re-add");
        Pass("delete and re-add preserves stable ids and creates no duplicates");

        // ---- E. approve, then warn/cancel/recreate ------------------------------------------------
        Check(manager.ApprovePendingWork(), "pending rack work approves");
        Check(manager.WorkOrders.Where(order => order.rackId > 0).All(order => order.status == ScenarioWorkStatus.Approved),
            "all rack-owned work is Approved");
        string approvedSignature = Signature();
        int approvedLiving = LivingTrees();
        RackPlanOperationResult approvedReconfirm = manager.ConfirmRackPlan();
        Check(approvedReconfirm.Status == RackPlanOperationStatus.NoChange, "reconfirming approved work is a no-op");
        Check(Signature() == approvedSignature, "confirm never changes approved status or executes work");
        Check(LivingTrees() == approvedLiving, "confirm never fells a tree");

        RackPlanOperationResult warning = manager.UpdateRack(editedRack, editedEntrance, shortenedFar);
        Check(warning.Status == RackPlanOperationStatus.RequiresApprovalWarning, "editing approved work warns before changing anything");
        Check(Signature() == approvedSignature, "the warning path changes nothing");
        Pass("approved-but-unexecuted edit warns before mutation");

        var otherApprovedOrderIds = manager.WorkOrders.Where(order => order.rackId > 0 && order.rackId != editedRack)
            .Select(order => order.workOrderId).OrderBy(id => id).ToList();
        RackPlanOperationResult approvedEdit = manager.UpdateRack(editedRack, editedEntrance, shortenedFar, true);
        Check(approvedEdit.Succeeded && approvedEdit.Status == RackPlanOperationStatus.Applied,
            "approved edit applies with the explicit warning acknowledgement: " + approvedEdit.Message);
        Check(approvedEdit.OrdersCancelled > 0 && approvedEdit.OrdersCreated > 0, "approved work is cancelled and recreated");
        Check(manager.RackOrderState(editedRack).Approved == 0 && manager.RackOrderState(editedRack).Pending > 0,
            "the edited rack returns to Pending work");
        Check(otherApprovedOrderIds.SequenceEqual(manager.WorkOrders.Where(order => order.rackId > 0 && order.rackId != editedRack)
            .Select(order => order.workOrderId).OrderBy(id => id)), "unaffected racks keep their approved order ids");
        Check(manager.WorkOrders.Where(order => order.rackId > 0 && order.rackId != editedRack)
            .All(order => order.status == ScenarioWorkStatus.Approved), "unaffected racks stay Approved");
        Pass("approved-unexecuted edit uses cancel-and-recreate semantics only on the affected rack");

        // ---- F. entrance mark ---------------------------------------------------------------------
        RackPlanOperationResult restoreMarked = manager.UpdateRack(editedRack, editedEntrance, editedFar);
        Check(restoreMarked.Succeeded, "restore edited rack");
        Check(manager.MarkRackEntrance(editedRack).Succeeded && manager.MarkedRackEntranceCount == 1,
            "the confirmed entrance can be marked");
        RackSaveData marked = manager.Racks.First(rack => rack.rackId == editedRack);
        string markedTree = marked.entranceTreeId;
        RackPlanOperationResult markEdit = manager.UpdateRack(editedRack, editedEntrance, shortenedFar);
        Check(markEdit.Succeeded && markEdit.MarksCleared == 1, "geometry change reports the obsolete field mark");
        Check(!manager.Racks.First(rack => rack.rackId == editedRack).entranceMarked && manager.MarkedRackEntranceCount == 0,
            "geometry change clears the entrance-marked state");
        Check(!string.IsNullOrEmpty(markedTree), "the entrance tree identity existed before the change");
        manager.UpdateRack(editedRack, editedEntrance, editedFar);
        manager.MarkRackEntrance(editedRack);
        Check(manager.MarkedRackEntranceCount == 1, "the restored rack can be marked again");
        Pass("entrance mark is authoritative and geometry-bound");

        // ---- G. completed work locks the rack ------------------------------------------------------
        ScenarioOneWorkOrder completed = manager.WorkOrders.First(order => order.rackId == editedRack);
        completed.status = ScenarioWorkStatus.Completed;
        completed.resolvedYear = 0;
        Check(manager.RackIsLocked(editedRack), "a linked Completed order locks the rack");
        int lockedOrders = manager.WorkOrders.Count;
        RackPlanOperationResult lockedEdit = manager.UpdateRack(editedRack, editedEntrance, shortenedFar);
        Check(lockedEdit.Status == RackPlanOperationStatus.Locked, "a locked rack cannot be edited");
        RackPlanOperationResult lockedDelete = manager.DeleteRack(editedRack);
        Check(lockedDelete.Status == RackPlanOperationStatus.Locked, "a locked rack cannot be deleted");
        Check(manager.WorkOrders.Count == lockedOrders, "locked operations change no work order");
        Check(manager.ReopenRackPlan().Succeeded && !manager.RackPlanConfirmed, "the plan can reopen around a locked rack");
        Check(manager.UpdateRack(editedRack, editedEntrance, shortenedFar).Status == RackPlanOperationStatus.Locked,
            "the locked rack stays locked after the plan reopens");
        Check(manager.ConfirmRackPlan().Succeeded && manager.RackPlanConfirmed, "the plan reconfirms with a locked rack");
        Pass("completed-linked rack locking is enforced");

        // ---- H. save/load of the confirmed plan ----------------------------------------------------
        ForestSaveData beforeSave = saves.CaptureData();
        string beforeJson = JsonUtility.ToJson(beforeSave);
        Check(ForestSaveValidation.ValidateRackV2(beforeSave) == null, "the runtime state is a valid v21 rack save");
        Check(saves.LoadData(JsonUtility.FromJson<ForestSaveData>(beforeJson), false), "the runtime state loads in memory");
        yield return null;
        yield return null;
        Check(manager.RackPlanningV2Active && manager.RackPlanConfirmed && manager.Racks.Count == 5,
            "confirmed plan survives save/load");
        string persisted = SignatureFor(beforeSave.scenarioOne);
        string reloaded = SignatureFor(saves.CaptureData().scenarioOne);
        Check(persisted == reloaded, "persisted ids, geometry, sets, entrance marks and links survive exactly");
        Check(manager.RackIsLocked(editedRack), "the completed rack lock survives save/load");
        Pass("confirmed plan, frozen sets, marks and order links survive save/load");

        // ---- I. model-0 legacy path after a pre-v21 load -------------------------------------------
        ForestSaveData legacy = saves.CaptureData();
        legacy.version = 20;
        legacy.scenarioOne.rackPlanningModel = RackPlanningModel.Legacy;
        legacy.scenarioOne.rackPlanConfirmed = false;
        legacy.scenarioOne.nextRackId = 1;
        legacy.scenarioOne.racks = new List<RackSaveData>();
        foreach (ScenarioOneWorkOrder order in legacy.scenarioOne.workOrders)
            order.rackId = 0;
        Check(ForestSaveValidation.ValidateRackV2(legacy) == null, "the projected v20 save carries no V2 state");
        Check(saves.LoadData(legacy, false), "the projected pre-v21 save loads");
        yield return null;
        Check(!manager.RackPlanningV2Active && manager.RackPlanningModelVersion == RackPlanningModel.Legacy && manager.Racks.Count == 0,
            "a pre-v21 load stays model 0 with no V2 racks");
        planner.Refresh();
        RackProposal legacyProposal = null;
        foreach (float offset in new[] { 8f, -8f, 4f, -4f })
        {
            legacyProposal = planner.Propose(new RackPoint(0f, lines[0] + offset), 0f);
            if (legacyProposal != null && legacyProposal.CanConfirm) break;
        }
        Check(legacyProposal != null && legacyProposal.CanConfirm, "a model-0 world can still build a legacy rack proposal");
        int legacyAdded = manager.ConfirmRack(legacyProposal, "verification");
        Check(legacyAdded > 0 && manager.RackGroups.Count > 0, "the legacy model-0 ConfirmRack path still creates a session rack group");
        Check(manager.WorkOrders.Where(order => order.type == ScenarioWorkType.FellTree && order.IsOpen)
            .All(order => order.rackId == 0), "legacy work orders stay outside the V2 rack link");
        Pass("model-0 legacy path remains viable");

        Check(log.Count >= 13, "all runtime phases ran");
    }

    private string Signature()
    {
        var parts = new List<string>();
        foreach (RackPlanRack rack in manager.RackPlan)
        {
            parts.Add(rack.Id + ":" + rack.Entrance.X.ToString("R") + "/" + rack.Entrance.Z.ToString("R") + ":"
                + rack.Far.X.ToString("R") + "/" + rack.Far.Z.ToString("R") + ":"
                + rack.Status + ":" + rack.EntranceTreeId + ":" + rack.EntranceMarked + ":"
                + string.Join(",", rack.RemovalTreeIds) + ":" + rack.Orders);
        }
        foreach (ScenarioOneWorkOrder order in manager.WorkOrders.OrderBy(order => order.workOrderId))
            parts.Add(order.workOrderId + ":" + order.status + ":" + order.targetTreeId + ":" + order.rackId);
        return string.Join("|", parts);
    }

    private static string SignatureFor(ScenarioOneSaveData scenario)
    {
        if (scenario == null) return "";
        var parts = new List<string>();
        foreach (RackSaveData rack in scenario.racks.OrderBy(rack => rack.rackId))
            parts.Add(rack.rackId + ":" + rack.entranceX.ToString("R") + "/" + rack.entranceZ.ToString("R") + ":"
                + rack.farX.ToString("R") + "/" + rack.farZ.ToString("R") + ":"
                + (scenario.rackPlanConfirmed ? RackPlanStatus.Confirmed : RackPlanStatus.Draft) + ":" + rack.entranceTreeId + ":"
                + rack.entranceMarked + ":" + string.Join(",", rack.removalTreeIds));
        foreach (ScenarioOneWorkOrder order in scenario.workOrders.OrderBy(order => order.workOrderId))
            parts.Add(order.workOrderId + ":" + order.status + ":" + order.targetTreeId + ":" + order.rackId);
        return string.Join("|", parts);
    }
}
