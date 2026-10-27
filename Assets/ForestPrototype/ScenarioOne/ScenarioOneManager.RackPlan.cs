using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Rack Planning & Field Marking V2 plan state (save v21, D-064). This is the authoritative runtime:
// one persisted rack object set, the confirmation state, the stable id allocator, and the only code
// that creates or reconciles rack-owned work orders. Confirm creates/refreshes Pending work only; it
// never approves or executes. Editing before approval reconciles the affected Pending orders without
// duplicates; editing approved but unexecuted work needs an explicit warning and cancels/recreates
// those orders. Any linked Completed order locks that rack's geometry and membership.
//
// Everything else about a rack (angle, length, width, spacing, validity, lock state, counters, order
// set, suggested layout, headland) is derived from these records and the live world. Map, minimap,
// field marking and MFM read the snapshot APIs below rather than rebuilding rack geometry.
public sealed partial class ScenarioOneManager
{
    // Scenario One gameplay calibration (D-064), not a forestry rule.
    public const int RackPlanTargetCount = 5;

    [SerializeField] private int rackPlanningModel = RackPlanningModel.Legacy;
    [SerializeField] private bool rackPlanConfirmed;
    [SerializeField] private int nextRackId = 1;
    [SerializeField] private List<RackSaveData> racks = new List<RackSaveData>();

    // Named ModelVersion (like ecology.GrowthModelVersion) so it never shadows the RackPlanningModel constants.
    public int RackPlanningModelVersion => rackPlanningModel;
    public bool RackPlanConfirmed => rackPlanConfirmed;
    public int NextRackId => nextRackId;
    public IReadOnlyList<RackSaveData> Racks => racks;

    // The one runtime answer to "is this a V2 rack world?". True only when the world is Enlarged80 AND
    // its Scenario One state explicitly identifies as model 1. Never inferred from the save version,
    // from the geometry alone, from a non-null rack list, from authored layout lines or from objective
    // progress. Legacy40, Reference Future v1, pre-v21 saves and pre-V2 Enlarged80 continuations are
    // not V2 worlds even though some of their rack code paths still exist.
    public bool RackPlanningV2Active =>
        rackPlanningModel == RackPlanningModel.RackPlanningFieldMarkingV2
        && ecology != null
        && ecology.StandGeometryModelVersion == StandGeometryModel.Enlarged80;

    // Derived counters. There is no independently incremented planned or marked counter anywhere.
    public int PlannedRackCount => racks.Count;
    public int MarkedRackEntranceCount => rackPlanConfirmed ? racks.Count(RackHasCurrentMark) : 0;
    // The legacy tool's session count still reads from the one rack state for its own workflow model.
    public int RackObjectCount => RackPlanningV2Active ? racks.Count : rackGroups.Count;

    // Read-only snapshots for the map, minimap, field marking and MFM. Ordered by stable rack id.
    public IReadOnlyList<RackPlanRack> RackPlan => DescribeRacks();
    public IReadOnlyList<RackPlanRack> ConfirmedRacks
        => rackPlanConfirmed ? DescribeRacks() : Array.Empty<RackPlanRack>();

    public bool TryGetRack(int rackId, out RackPlanRack rack)
    {
        RackSaveData record = FindRack(rackId);
        if (record == null)
        {
            rack = null;
            return false;
        }
        rack = DescribeRack(record);
        return true;
    }

    public RackOrderLifecycle RackOrderState(int rackId)
    {
        int pending = 0, approved = 0, completed = 0, failed = 0;
        foreach (ScenarioOneWorkOrder order in workOrders)
        {
            if (order == null || order.rackId != rackId || rackId <= 0)
                continue;
            switch (order.status)
            {
                case ScenarioWorkStatus.Pending: pending++; break;
                case ScenarioWorkStatus.Approved: approved++; break;
                case ScenarioWorkStatus.Completed: completed++; break;
                default: failed++; break;
            }
        }
        return new RackOrderLifecycle(pending, approved, completed, failed);
    }

    public bool RackIsLocked(int rackId) => RackOrderState(rackId).IsLocked;

    // New-game identity, called from the fresh-game setup right after InitializeNewScenario (which is
    // model-neutral because legacy loads call it too). A brand-new Enlarged80 Scenario One game is a
    // V2 world from its first frame - with zero racks, unconfirmed, nextRackId 1. A brand-new Legacy40
    // game stays model 0.
    private void InitializeRackPlanForNewWorld()
    {
        ClearRackPlan();
        if (ecology != null && ecology.StandGeometryModelVersion == StandGeometryModel.Enlarged80)
            rackPlanningModel = RackPlanningModel.RackPlanningFieldMarkingV2;
    }

    // The empty/legacy plan: model 0, unconfirmed, no racks. RestoreSaveData(null) and every fresh
    // scenario reset land here, so a legacy load can never accidentally activate V2.
    private void ClearRackPlan()
    {
        rackPlanningModel = RackPlanningModel.Legacy;
        rackPlanConfirmed = false;
        nextRackId = 1;
        racks.Clear();
    }

    // ---- authoring operations -------------------------------------------------------------------
    //
    // AddRack and UpdateRack author geometry and its frozen candidate membership. No work order is
    // created until ConfirmRackPlan, except when UpdateRack revises a rack that already has work:
    // then the affected orders are reconciled immediately so a confirmed plan never drifts from its
    // geometry. Every operation is atomic: validation happens before any mutation.

    public RackPlanOperationResult AddRack(RackPoint entrance, RackPoint far)
    {
        if (!RackPlanningV2Active)
            return RackPlanOperationResult.Invalid("Rack Planning V2 is not active in this world.");
        if (racks.Count >= RackPlanTargetCount)
            return RackPlanOperationResult.Invalid($"The rack plan already holds {RackPlanTargetCount} racks.");

        if (!TryValidateRackGeometry(entrance, far, out string geometryProblem))
            return RackPlanOperationResult.Invalid(geometryProblem);
        if (!TrySelectRemovalSet(entrance, far, 0, out List<string> removalIds, out string entranceTreeId, out string selectionProblem))
            return RackPlanOperationResult.Invalid(selectionProblem);
        if (RemovalSetOverlapsOtherRack(0, removalIds))
            return RackPlanOperationResult.Invalid("Another rack already removes a tree on this line.");

        int rackId = nextRackId++;
        racks.Add(new RackSaveData
        {
            rackId = rackId,
            entranceX = entrance.X,
            entranceZ = entrance.Z,
            farX = far.X,
            farZ = far.Z,
            removalTreeIds = removalIds,
            entranceTreeId = entranceTreeId,
            entranceMarked = false
        });
        if (rackPlanConfirmed && racks.Count != RackPlanTargetCount)
            rackPlanConfirmed = false;
        feedback = $"Added rack {rackId} to the plan ({removalIds.Count} tree{(removalIds.Count == 1 ? "" : "s")}). "
            + "Confirm the rack plan to put its removal work in the Work Plan.";
        return RackPlanOperationResult.Applied(feedback, rackId, racksAdded: 1);
    }

    public RackPlanOperationResult UpdateRack(int rackId, RackPoint entrance, RackPoint far, bool allowCancelApproved = false)
    {
        if (!RackPlanningV2Active)
            return RackPlanOperationResult.Invalid("Rack Planning V2 is not active in this world.", rackId);
        RackSaveData rack = FindRack(rackId);
        if (rack == null)
            return RackPlanOperationResult.Invalid($"There is no rack {rackId}.", rackId);

        RackOrderLifecycle lifecycle = RackOrderState(rackId);
        if (lifecycle.IsLocked)
            return RackPlanOperationResult.Locked("This rack has completed work and can no longer be changed.", rackId);

        RackPoint oldEntrance = new RackPoint(rack.entranceX, rack.entranceZ);
        RackPoint oldFar = new RackPoint(rack.farX, rack.farZ);
        if (RackGeometry.SameLine(oldEntrance, oldFar, entrance, far))
            return RackPlanOperationResult.NoChange("The rack geometry is unchanged.", rackId);

        if (!TryValidateRackGeometry(entrance, far, out string geometryProblem))
            return RackPlanOperationResult.Invalid(geometryProblem, rackId);
        if (!TrySelectRemovalSet(entrance, far, rackId, out List<string> removalIds, out string entranceTreeId, out string selectionProblem))
            return RackPlanOperationResult.Invalid(selectionProblem, rackId);
        if (RemovalSetOverlapsOtherRack(rackId, removalIds))
            return RackPlanOperationResult.Invalid("Another rack already removes a tree on this line.", rackId);
        if (lifecycle.HasApprovedWork && !allowCancelApproved)
            return RackPlanOperationResult.Warning(
                "Changing this rack would cancel approved but unexecuted removal work. Confirm that you want to cancel and recreate it.",
                rackId);

        bool markCleared = rack.entranceMarked;
        rack.entranceX = entrance.X;
        rack.entranceZ = entrance.Z;
        rack.farX = far.X;
        rack.farZ = far.Z;
        rack.removalTreeIds = removalIds;
        rack.entranceTreeId = entranceTreeId;
        // A changed line invalidates the field mark; the marking lane clears the obsolete paint.
        rack.entranceMarked = false;

        RackOrderChange change = ApplyRackOrderChange(rack, removalIds, lifecycle.HasApprovedWork);
        feedback = $"Updated rack {rackId}: {change.Created} order{(change.Created == 1 ? "" : "s")} added, "
            + $"{change.Removed} pending removed, {change.Cancelled} approved cancelled.";
        return RackPlanOperationResult.Applied(feedback, rackId, change.Created, change.Removed, change.Cancelled,
            racksUpdated: 1, marksCleared: markCleared ? 1 : 0);
    }

    public RackPlanOperationResult DeleteRack(int rackId, bool allowCancelApproved = false)
    {
        if (!RackPlanningV2Active)
            return RackPlanOperationResult.Invalid("Rack Planning V2 is not active in this world.", rackId);
        RackSaveData rack = FindRack(rackId);
        if (rack == null)
            return RackPlanOperationResult.NoChange($"There is no rack {rackId}.", rackId);

        RackOrderLifecycle lifecycle = RackOrderState(rackId);
        if (lifecycle.IsLocked)
            return RackPlanOperationResult.Locked("This rack has completed work and cannot be deleted.", rackId);
        if (lifecycle.HasApprovedWork && !allowCancelApproved)
            return RackPlanOperationResult.Warning(
                "Deleting this rack would cancel approved but unexecuted removal work. Confirm that you want to cancel it.",
                rackId);

        RackOrderChange change = new RackOrderChange();
        foreach (ScenarioOneWorkOrder order in workOrders.Where(order => order != null && order.rackId == rackId).ToList())
        {
            if (order.status == ScenarioWorkStatus.Pending)
            {
                if (RemovePendingOrderCore(order)) change.Removed++;
            }
            else if (order.status == ScenarioWorkStatus.Approved)
            {
                if (CancelApprovedOrderCore(order)) change.Cancelled++;
            }
            else
            {
                // Failed work is history, not a live link: keep the order but detach the removed rack.
                order.rackId = 0;
            }
        }
        racks.Remove(rack);
        if (rackPlanConfirmed && racks.Count != RackPlanTargetCount)
            rackPlanConfirmed = false;
        GetHarvestQuote(false);
        feedback = $"Deleted rack {rackId} from the plan ({change.Removed} pending removed, {change.Cancelled} approved cancelled).";
        return RackPlanOperationResult.Applied(feedback, rackId, 0, change.Removed, change.Cancelled, racksRemoved: 1);
    }

    // Freezes every rack's membership, creates/refreshes its Pending removal orders, and only then
    // marks the plan confirmed. It never approves or executes work.
    public RackPlanOperationResult ConfirmRackPlan(bool allowCancelApproved = false)
    {
        if (!RackPlanningV2Active)
            return RackPlanOperationResult.Invalid("Rack Planning V2 is not active in this world.");
        if (racks.Count != RackPlanTargetCount)
            return RackPlanOperationResult.Invalid($"A confirmed rack plan needs exactly {RackPlanTargetCount} racks (the plan has {racks.Count}).");

        var preparations = new List<RackPreparation>(racks.Count);
        var seenIds = new HashSet<int>();
        foreach (RackSaveData rack in racks)
        {
            if (rack == null || !seenIds.Add(rack.rackId))
                return RackPlanOperationResult.Invalid("The rack plan contains a missing or duplicate rack record.");
            RackPoint entrance = new RackPoint(rack.entranceX, rack.entranceZ);
            RackPoint far = new RackPoint(rack.farX, rack.farZ);
            if (!TryValidateRackGeometry(entrance, far, out string geometryProblem))
                return RackPlanOperationResult.Invalid(geometryProblem, rack.rackId);

            var removalIds = rack.removalTreeIds != null ? new List<string>(rack.removalTreeIds) : new List<string>();
            string entranceTreeId = rack.entranceTreeId ?? "";
            bool changed = false;
            if (RackOrderState(rack.rackId).IsLocked && removalIds.Count == 0)
                return RackPlanOperationResult.Locked("A rack with completed work cannot have its frozen removal set rebuilt.", rack.rackId);
            // A draft that has never held work freezes its membership at this confirmation, from the
            // live stand. A reopened confirmed rack keeps its frozen membership: reconfirming unchanged
            // must never silently add or drop trees.
            bool draftMembership = !rackPlanConfirmed && !RackHasAnyOrder(rack.rackId);
            if (removalIds.Count == 0 || draftMembership)
            {
                if (!TrySelectRemovalSet(entrance, far, rack.rackId, out removalIds, out entranceTreeId, out string selectionProblem))
                    return RackPlanOperationResult.Invalid(selectionProblem, rack.rackId);
                changed = true;
            }
            else if (string.IsNullOrEmpty(entranceTreeId))
            {
                entranceTreeId = FindEntranceTree(entrance, removalIds);
                if (string.IsNullOrEmpty(entranceTreeId))
                    return RackPlanOperationResult.Invalid("RACK INVALID — ADJUST PLAN: no suitable entrance tree stands on this rack.", rack.rackId);
                changed = true;
            }
            if (!removalIds.Contains(entranceTreeId))
                return RackPlanOperationResult.Invalid("RACK INVALID — ADJUST PLAN: the entrance tree is not in the rack removal set.", rack.rackId);

            preparations.Add(new RackPreparation(rack, removalIds, entranceTreeId, changed));
        }

        var claimed = new HashSet<string>(StringComparer.Ordinal);
        foreach (RackPreparation preparation in preparations)
        {
            foreach (string treeId in preparation.RemovalTreeIds)
            {
                if (!claimed.Add(treeId))
                    return RackPlanOperationResult.Invalid($"Tree {treeId} is claimed by more than one rack.", preparation.Rack.rackId);
            }
        }

        foreach (RackPreparation preparation in preparations)
        {
            RackOrderLifecycle lifecycle = RackOrderState(preparation.Rack.rackId);
            if (!lifecycle.HasApprovedWork)
                continue;
            var desired = new HashSet<string>(preparation.RemovalTreeIds, StringComparer.Ordinal);
            bool approvedOutsideSet = workOrders.Any(order => order != null && order.rackId == preparation.Rack.rackId
                && order.IsOpen && order.status == ScenarioWorkStatus.Approved
                && !desired.Contains(order.targetTreeId ?? ""));
            if (preparation.Changed || approvedOutsideSet)
            {
                if (!allowCancelApproved)
                    return RackPlanOperationResult.Warning(
                        "Confirming this edited plan would cancel approved but unexecuted removal work. Confirm that you want to cancel and recreate it.",
                        preparation.Rack.rackId);
            }
        }

        int created = 0, removed = 0, cancelled = 0, marksCleared = 0;
        bool stateChanged = !rackPlanConfirmed;
        foreach (RackPreparation preparation in preparations)
        {
            RackSaveData rack = preparation.Rack;
            RackOrderLifecycle lifecycle = RackOrderState(rack.rackId);
            if (preparation.Changed)
            {
                stateChanged = true;
                rack.removalTreeIds = preparation.RemovalTreeIds;
                rack.entranceTreeId = preparation.EntranceTreeId;
                if (rack.entranceMarked)
                {
                    rack.entranceMarked = false;
                    marksCleared++;
                }
            }
            // Approved work is only cancelled/recreated for a rack whose membership actually changed;
            // reconfirming an unchanged rack must leave its approved orders alone.
            RackOrderChange change = ApplyRackOrderChange(rack, preparation.RemovalTreeIds,
                lifecycle.HasApprovedWork && preparation.Changed);
            created += change.Created;
            removed += change.Removed;
            cancelled += change.Cancelled;
        }
        rackPlanConfirmed = true;
        feedback = $"Confirmed the rack plan: {RackPlanTargetCount} racks, {created} new pending order{(created == 1 ? "" : "s")}. Review and approve the work in the Work Plan.";
        return stateChanged || created > 0 || removed > 0 || cancelled > 0 || marksCleared > 0
            ? RackPlanOperationResult.Applied(feedback, 0, created, removed, cancelled, racksUpdated: racks.Count, marksCleared: marksCleared)
            : RackPlanOperationResult.NoChange("The rack plan is already confirmed and unchanged.");
    }

    // Reopening does not touch work or field marks. It only returns the plan to its editable state;
    // the next ConfirmRackPlan reconciles any edits. A rack with completed work stays locked.
    public RackPlanOperationResult ReopenRackPlan()
    {
        if (!RackPlanningV2Active)
            return RackPlanOperationResult.Invalid("Rack Planning V2 is not active in this world.");
        if (!rackPlanConfirmed)
            return RackPlanOperationResult.NoChange("The rack plan is already open for editing.");
        rackPlanConfirmed = false;
        feedback = "Rack plan reopened. Confirm it again when the five racks are ready.";
        return RackPlanOperationResult.Applied(feedback);
    }

    // Field-marking state only. The orange visual and its tree conflict are the marking lane's job;
    // this proves the entrance identity and records the authoritative mark state.
    public RackPlanOperationResult MarkRackEntrance(int rackId)
    {
        if (!RackPlanningV2Active)
            return RackPlanOperationResult.Invalid("Rack Planning V2 is not active in this world.", rackId);
        if (!rackPlanConfirmed)
            return RackPlanOperationResult.Invalid("Confirm the rack plan before marking rack entrances.", rackId);
        RackSaveData rack = FindRack(rackId);
        if (rack == null)
            return RackPlanOperationResult.Invalid($"There is no rack {rackId}.", rackId);
        if (string.IsNullOrEmpty(rack.entranceTreeId))
            return RackPlanOperationResult.Invalid("RACK INVALID — ADJUST PLAN: this rack has no entrance tree.", rackId);
        if (rack.entranceMarked)
            return RackPlanOperationResult.NoChange("This rack entrance is already marked.", rackId);
        if (RackOrderState(rackId).IsLocked)
            return RackPlanOperationResult.Locked("This rack has already been worked; its entrance cannot be marked now.", rackId);

        Dictionary<string, ForestTree> living = LivingTreesById();
        if (!living.TryGetValue(rack.entranceTreeId, out ForestTree tree) || tree == null)
            return RackPlanOperationResult.Invalid("RACK INVALID — ADJUST PLAN: the entrance tree is no longer standing.", rackId);
        if (tree.IsCropTree)
            return RackPlanOperationResult.Invalid("The entrance tree is a Crop Tree and cannot also carry the rack mark.", rackId);
        if (tree.IsMarkedForFell)
            return RackPlanOperationResult.Invalid("The entrance tree already carries a Fell mark and cannot also carry the rack mark.", rackId);

        rack.entranceMarked = true;
        feedback = $"Marked the entrance tree {rack.entranceTreeId} for rack {rackId}.";
        return RackPlanOperationResult.Applied(feedback, rackId);
    }

    public RackPlanOperationResult ClearRackEntranceMark(int rackId)
    {
        if (!RackPlanningV2Active)
            return RackPlanOperationResult.Invalid("Rack Planning V2 is not active in this world.", rackId);
        RackSaveData rack = FindRack(rackId);
        if (rack == null)
            return RackPlanOperationResult.Invalid($"There is no rack {rackId}.", rackId);
        if (!rack.entranceMarked)
            return RackPlanOperationResult.NoChange("This rack entrance is not marked.", rackId);
        rack.entranceMarked = false;
        feedback = $"Cleared the entrance mark for rack {rackId}.";
        return RackPlanOperationResult.Applied(feedback, rackId, marksCleared: 1);
    }

    // ---- internals ------------------------------------------------------------------------------

    private RackSaveData FindRack(int rackId)
        => rackId > 0 ? racks.FirstOrDefault(rack => rack != null && rack.rackId == rackId) : null;

    private bool RackHasAnyOrder(int rackId)
        => workOrders.Any(order => order != null && order.rackId == rackId);

    private IReadOnlyList<RackPlanRack> DescribeRacks()
    {
        var result = new List<RackPlanRack>(racks.Count);
        foreach (RackSaveData rack in racks.Where(rack => rack != null).OrderBy(rack => rack.rackId))
            result.Add(DescribeRack(rack));
        return result.AsReadOnly();
    }

    private RackPlanRack DescribeRack(RackSaveData rack)
        => new RackPlanRack(rack.rackId, new RackPoint(rack.entranceX, rack.entranceZ),
            new RackPoint(rack.farX, rack.farZ), RackGeometry.DefaultWidthM,
            rackPlanConfirmed ? RackPlanStatus.Confirmed : RackPlanStatus.Draft,
            rack.removalTreeIds, rack.entranceTreeId, rack.entranceMarked, RackOrderState(rack.rackId));

    private static bool RackHasCurrentMark(RackSaveData rack)
        => rack != null && rack.entranceMarked && !string.IsNullOrEmpty(rack.entranceTreeId);

    private bool TryValidateRackGeometry(RackPoint entrance, RackPoint far, out string problem)
    {
        problem = "";
        if (!IsFinite(entrance.X) || !IsFinite(entrance.Z) || !IsFinite(far.X) || !IsFinite(far.Z))
        {
            problem = "RACK INVALID — ADJUST PLAN: the rack has a non-finite endpoint.";
            return false;
        }
        if (RackGeometry.LengthM(entrance, far) < 1e-3f)
        {
            problem = "RACK INVALID — ADJUST PLAN: the rack has no length.";
            return false;
        }
        float halfExtent = ecology != null
            ? StandGeometryModel.StandSizeMeters(ecology.StandGeometryModelVersion) * 0.5f
            : 40f;
        if (Math.Abs(entrance.X) > halfExtent + 1e-3f || Math.Abs(entrance.Z) > halfExtent + 1e-3f
            || Math.Abs(far.X) > halfExtent + 1e-3f || Math.Abs(far.Z) > halfExtent + 1e-3f)
        {
            problem = "RACK INVALID — ADJUST PLAN: the rack leaves the stand.";
            return false;
        }
        return true;
    }

    private static bool IsFinite(float value) => !float.IsNaN(value) && !float.IsInfinity(value);

    // The rack's frozen membership derived from its centreline and the live stand. Trees already claimed
    // by another rack (even a harvested one) or by an independent open order are left out, as are Crop
    // Trees; the caller never has to deduplicate. The first tree nearest the entrance end becomes the
    // entrance tree.
    private bool TrySelectRemovalSet(RackPoint entrance, RackPoint far, int ownRackId,
        out List<string> removalIds, out string entranceTreeId, out string problem)
    {
        removalIds = new List<string>();
        entranceTreeId = "";
        problem = "";
        if (RackGeometry.LengthM(entrance, far) < 1e-3f)
        {
            problem = "RACK INVALID — ADJUST PLAN: the rack has no length.";
            return false;
        }

        var claimed = new HashSet<string>(StringComparer.Ordinal);
        foreach (RackSaveData other in racks)
            if (other != null && other.rackId != ownRackId && other.removalTreeIds != null)
                foreach (string treeId in other.removalTreeIds)
                    if (!string.IsNullOrEmpty(treeId)) claimed.Add(treeId);
        foreach (ScenarioOneWorkOrder order in workOrders)
            if (order != null && order.IsOpen && order.type == ScenarioWorkType.FellTree
                && order.rackId != ownRackId && !string.IsNullOrEmpty(order.targetTreeId))
                claimed.Add(order.targetTreeId);

        var trees = new List<RackTree>();
        foreach (KeyValuePair<string, ForestTree> pair in LivingTreesById())
        {
            ForestTree tree = pair.Value;
            if (tree == null) continue;
            Vector3 position = tree.transform.position;
            trees.Add(new RackTree(pair.Key, position.x, position.z, tree.CanChop, tree.IsCropTree, claimed.Contains(pair.Key)));
        }
        RackSelection selection = RackGeometry.Select(trees, entrance, far, RackGeometry.DefaultWidthM);
        if (selection.Trees.Count == 0)
        {
            problem = "RACK INVALID — ADJUST PLAN: no suitable rack-removal tree lies on this line.";
            return false;
        }
        foreach (RackTree tree in selection.Trees)
            removalIds.Add(tree.Id);
        entranceTreeId = FindEntranceTree(entrance, removalIds);
        if (string.IsNullOrEmpty(entranceTreeId))
        {
            problem = "RACK INVALID — ADJUST PLAN: no suitable entrance tree lies on this line.";
            removalIds.Clear();
            return false;
        }
        return true;
    }

    // The rack-removal tree nearest the path/headland end of the rack. The entrance endpoint is the
    // persisted access end, so this is the first removal tree in from the access.
    private string FindEntranceTree(RackPoint entrance, IReadOnlyList<string> removalIds)
    {
        Dictionary<string, ForestTree> living = LivingTreesById();
        string best = "";
        float bestDistance = float.MaxValue;
        for (int i = 0; i < removalIds.Count; i++)
        {
            string treeId = removalIds[i];
            if (string.IsNullOrEmpty(treeId) || !living.TryGetValue(treeId, out ForestTree tree) || tree == null || !tree.CanChop)
                continue;
            Vector3 position = tree.transform.position;
            float dx = position.x - entrance.X, dz = position.z - entrance.Z;
            float distance = dx * dx + dz * dz;
            if (distance < bestDistance - 1e-6f
                || (Math.Abs(distance - bestDistance) <= 1e-6f && string.CompareOrdinal(treeId, best) < 0))
            {
                bestDistance = distance;
                best = treeId;
            }
        }
        return best;
    }

    private bool RemovalSetOverlapsOtherRack(int ownRackId, IReadOnlyList<string> removalIds)
    {
        foreach (RackSaveData other in racks)
        {
            if (other == null || other.rackId == ownRackId || other.removalTreeIds == null)
                continue;
            foreach (string treeId in removalIds)
                if (other.removalTreeIds.Contains(treeId))
                    return true;
        }
        return false;
    }

    private struct RackOrderChange
    {
        public int Created, Removed, Cancelled;
    }

    // Reconciles a rack's open orders with the desired membership:
    //   * Pending orders for trees still in the rack are kept (stable ids, no duplicates).
    //   * Pending orders for trees no longer in the rack are removed.
    //   * On an approved-rack edit, all approved orders are cancelled first and the missing desired
    //     trees are recreated as Pending, so no order is partially approved for changed geometry.
    //   * Completed orders are never touched; their rack is locked before this can run.
    private RackOrderChange ApplyRackOrderChange(RackSaveData rack, IReadOnlyList<string> removalIds, bool cancelApproved)
    {
        var change = new RackOrderChange();
        var desired = new HashSet<string>(StringComparer.Ordinal);
        foreach (string treeId in removalIds)
            if (!string.IsNullOrEmpty(treeId)) desired.Add(treeId);

        if (cancelApproved)
        {
            foreach (ScenarioOneWorkOrder order in workOrders.Where(order => order != null && order.rackId == rack.rackId
                         && order.IsOpen && order.status == ScenarioWorkStatus.Approved).ToList())
                if (CancelApprovedOrderCore(order)) change.Cancelled++;
        }

        foreach (ScenarioOneWorkOrder order in workOrders.Where(order => order != null && order.rackId == rack.rackId
                     && order.status == ScenarioWorkStatus.Pending && !desired.Contains(order.targetTreeId ?? "")).ToList())
            if (RemovePendingOrderCore(order)) change.Removed++;

        // A precheck should have stopped any approved order outside the desired set; keep the invariant
        // here as a guard rather than silently leaving a stale approval behind.
        foreach (ScenarioOneWorkOrder order in workOrders.Where(order => order != null && order.rackId == rack.rackId
                     && order.IsOpen && order.status == ScenarioWorkStatus.Approved && !desired.Contains(order.targetTreeId ?? "")).ToList())
            if (CancelApprovedOrderCore(order)) change.Cancelled++;

        var openTrees = new HashSet<string>(StringComparer.Ordinal);
        var completedTrees = new HashSet<string>(StringComparer.Ordinal);
        foreach (ScenarioOneWorkOrder order in workOrders)
        {
            if (order == null || order.rackId != rack.rackId)
                continue;
            if (order.IsOpen) openTrees.Add(order.targetTreeId ?? "");
            else if (order.status == ScenarioWorkStatus.Completed) completedTrees.Add(order.targetTreeId ?? "");
        }

        Dictionary<string, ForestTree> living = null;
        foreach (string treeId in removalIds)
        {
            if (string.IsNullOrEmpty(treeId) || openTrees.Contains(treeId) || completedTrees.Contains(treeId))
                continue;
            if (living == null) living = LivingTreesById();
            if (!living.TryGetValue(treeId, out ForestTree tree) || tree == null || !tree.CanChop || tree.IsCropTree)
                continue;
            if (workOrders.Any(order => order != null && order.IsOpen && order.type == ScenarioWorkType.FellTree
                    && order.targetTreeId == treeId && order.rackId != rack.rackId))
                continue;
            ScenarioOneWorkOrder created = CreateFellingOrder(tree);
            created.rackId = rack.rackId;
            workOrders.Add(created);
            RecordOrderEvent(created, ScenarioManagementEventType.OrderCreated, ScenarioManagementOutcome.None, CurrentYear);
            change.Created++;
        }
        if (change.Created > 0 || change.Removed > 0 || change.Cancelled > 0)
            GetHarvestQuote(false);
        return change;
    }

    private sealed class RackPreparation
    {
        public readonly RackSaveData Rack;
        public readonly List<string> RemovalTreeIds;
        public readonly string EntranceTreeId;
        public readonly bool Changed;

        public RackPreparation(RackSaveData rack, List<string> removalTreeIds, string entranceTreeId, bool changed)
        {
            Rack = rack;
            RemovalTreeIds = removalTreeIds;
            EntranceTreeId = entranceTreeId;
            Changed = changed;
        }
    }
}
