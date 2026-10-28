using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// The Stand Map's direct rack editing (RACK-V2-05), as one narrow facade over the authoritative rack plan in
// ScenarioOneManager.RackPlan.cs. It adds no second rack state: every operation validates the editing rules
// (RackEditing: entrance on the headland, usable length, inside the stand, no duplicate or overlapping rack) and then calls
// the existing AddRack / UpdateRack / DeleteRack, so identity, removal sets, order reconciliation, the entrance-mark reset
// and the completed-work lock all stay exactly where they were. Nothing here is saved or counted separately.
//
// OPEN-3 (Manager decision 2026-10-28): a rack with no executed removals stays editable (pending or approved-but-unexecuted
// work is reconciled through the existing paths, with a warning for approved work); a rack with any executed removal is
// locked and these operations refuse it. Editing never reverses an executed removal.
//
// Undo is exactly one step: the last create, reshape (move or rotate), delete or reset. It is held in memory only and checks
// that the plan is still exactly the state that edit produced, so a load, a confirmation or any other change makes it
// unavailable instead of restoring something stale.
public sealed partial class ScenarioOneManager
{
    private enum RackEditKind { Create, Reshape, Delete, Reset }

    private readonly struct RackGeometryRecord
    {
        public readonly int Id;
        public readonly RackPoint Entrance, Far;

        public RackGeometryRecord(int id, RackPoint entrance, RackPoint far)
        {
            Id = id;
            Entrance = entrance;
            Far = far;
        }

        public bool Same(RackGeometryRecord other)
            => Id == other.Id && Entrance.X == other.Entrance.X && Entrance.Z == other.Entrance.Z
                && Far.X == other.Far.X && Far.Z == other.Far.Z;
    }

    // The geometry of the whole plan plus the id allocator, and what the rack-owned work looks like. An undo restores geometry, and
    // it is only offered while the plan is still exactly as the edit left it: if the plan was confirmed, work approved or executed
    // since, the order content differs and the undo is no longer available (undoing an old edit over new work would be wrong).
    private sealed class RackPlanSnapshot
    {
        public readonly List<RackGeometryRecord> Racks = new List<RackGeometryRecord>();
        public int NextRackId;
        public string Orders = "";

        public static RackPlanSnapshot Capture(ScenarioOneManager manager)
        {
            var snapshot = new RackPlanSnapshot { NextRackId = manager.nextRackId };
            foreach (RackSaveData rack in manager.racks.Where(rack => rack != null).OrderBy(rack => rack.rackId))
                snapshot.Racks.Add(new RackGeometryRecord(rack.rackId, new RackPoint(rack.entranceX, rack.entranceZ), new RackPoint(rack.farX, rack.farZ)));
            // Order content without ids: an order recreated for the same tree after an edit is the same work.
            snapshot.Orders = string.Join(";", manager.workOrders.Where(order => order != null && order.rackId > 0)
                .Select(order => order.rackId + ":" + order.status + ":" + order.targetTreeId).OrderBy(text => text, StringComparer.Ordinal));
            return snapshot;
        }

        public bool Same(RackPlanSnapshot other)
        {
            if (other == null || NextRackId != other.NextRackId || Racks.Count != other.Racks.Count || Orders != other.Orders)
                return false;
            for (int i = 0; i < Racks.Count; i++)
                if (!Racks[i].Same(other.Racks[i]))
                    return false;
            return true;
        }
    }

    private sealed class RackEditUndo
    {
        public readonly RackEditKind Kind;
        public readonly RackPlanSnapshot Before, After;
        public readonly int RackId;

        public RackEditUndo(RackEditKind kind, RackPlanSnapshot before, RackPlanSnapshot after, int rackId)
        {
            Kind = kind;
            Before = before;
            After = after;
            RackId = rackId;
        }
    }

    // Session memory only. Not a serialized field, so it never reaches the scene or a save.
    private RackEditUndo lastRackEdit;

    // The editing bounds for the live world: the approved headland line and the stand square. Read from the headland geometry
    // and the stand geometry model; nothing is copied.
    public RackEditBounds RackEditBounds => new RackEditBounds(
        ScenarioOneHeadlandGeometry.HeadlandX, ScenarioOneHeadlandGeometry.SouthZ, ScenarioOneHeadlandGeometry.NorthZ,
        ecology != null ? StandGeometryModel.StandSizeMeters(ecology.StandGeometryModelVersion) * 0.5f : 40f);

    // True when racks may be changed directly on the map: a V2 world whose plan is not confirmed. A confirmed plan is fixed until
    // it is reopened (ReopenRackPlan, offered on the Stand Map by RACK-V2-07).
    public bool RackEditingOpen => RackPlanningV2Active && !rackPlanConfirmed;

    // ---- confirm and reopen (RACK-V2-07) -----------------------------------------------------------
    //
    // Read-only questions the Stand Map asks before it offers Confirm Rack Plan or Reopen Rack Plan. Neither stores anything: the
    // answers come from the rack objects and their orders, and the actual changes still go through ConfirmRackPlan / ReopenRackPlan.

    // Whether Confirm Rack Plan is available right now, and if not, the honest reason. A plan is confirmable when it holds exactly
    // RackPlanTargetCount racks and every rack that can still change passes the same editing rules the map applies to a drag. The
    // frozen removal set and entrance tree are checked by ConfirmRackPlan itself, which refuses before changing anything.
    public bool CanConfirmRackPlan(out string reason)
    {
        reason = "";
        if (!RackPlanningV2Active)
            reason = "Rack Planning V2 is not active in this world.";
        else if (rackPlanConfirmed)
            reason = "The rack plan is already confirmed.";
        else if (racks.Count != RackPlanTargetCount)
            reason = $"A confirmed plan needs exactly {RackPlanTargetCount} racks. The plan has {racks.Count}.";
        else
        {
            foreach (RackSaveData rack in racks.OrderBy(rack => rack.rackId))
            {
                // A harvested rack is fixed whatever its geometry says; only racks that can still change are re-checked.
                if (rack == null || RackIsLocked(rack.rackId)) continue;
                RackEditValidation check = ValidateRackEdit(rack.rackId, new RackPoint(rack.entranceX, rack.entranceZ), new RackPoint(rack.farX, rack.farZ));
                if (!check.IsValid)
                {
                    reason = $"Rack {rack.rackId} is not valid: {check.Message}";
                    break;
                }
            }
        }
        return reason.Length == 0;
    }

    // What reopening a confirmed plan would mean for the work already linked to the racks. Reopening itself never changes orders or
    // marks (ReopenRackPlan); this only lets the map warn first. Racks with executed removals stay geometry-locked; racks with
    // approved but unexecuted work would need an explicit cancel-and-recreate consent before one of them is changed.
    public readonly struct RackReopenSummary
    {
        public readonly int RackCount, LockedRacks, EditableRacks, RacksWithApprovedWork;

        public RackReopenSummary(int rackCount, int lockedRacks, int racksWithApprovedWork)
        {
            RackCount = rackCount;
            LockedRacks = lockedRacks;
            EditableRacks = rackCount - lockedRacks;
            RacksWithApprovedWork = racksWithApprovedWork;
        }

        public bool NothingEditable => EditableRacks <= 0;
    }

    public RackReopenSummary DescribeRackReopen()
    {
        int locked = 0, approved = 0, count = 0;
        foreach (RackSaveData rack in racks)
        {
            if (rack == null) continue;
            count++;
            RackOrderLifecycle lifecycle = RackOrderState(rack.rackId);
            if (lifecycle.IsLocked) locked++;
            else if (lifecycle.HasApprovedWork) approved++;
        }
        return new RackReopenSummary(count, locked, approved);
    }

    public bool CanUndoRackEdit
        => lastRackEdit != null && RackEditingOpen && RackPlanSnapshot.Capture(this).Same(lastRackEdit.After);

    public string RackEditUndoDescription
    {
        get
        {
            if (!CanUndoRackEdit) return "";
            switch (lastRackEdit.Kind)
            {
                case RackEditKind.Create: return "adding rack " + lastRackEdit.RackId;
                case RackEditKind.Reshape: return "changing rack " + lastRackEdit.RackId;
                case RackEditKind.Delete: return "deleting rack " + lastRackEdit.RackId;
                default: return "resetting to the suggested layout";
            }
        }
    }

    // The suggested routes for this world, read-only guidance that never touches the plan. This is the one place the plantation
    // tree snapshot the foundation needs is taken (stumps included: positions, not felling state, define the guidance).
    public IReadOnlyList<SuggestedRackSegment> GetSuggestedRackLayout()
    {
        if (!RackPlanningV2Active || ecology == null)
            return Array.Empty<SuggestedRackSegment>();
        var trees = new List<RackTree>();
        foreach (ForestTree tree in UnityEngine.Object.FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
        {
            if (tree == null || string.IsNullOrEmpty(tree.TreeId) || !tree.TreeId.StartsWith("P", StringComparison.Ordinal)) continue;
            Vector3 position = tree.transform.position;
            trees.Add(new RackTree(tree.TreeId, position.x, position.z));
        }
        return ScenarioOneSuggestedRackLayout.Get(rackPlanningModel, ecology.StandGeometryModelVersion, trees);
    }

    // The racks the editing rules compare against: every rack in the plan except the one being edited.
    public List<RackEditOther> RackEditOthers(int excludeRackId = 0)
        => racks.Where(rack => rack != null && rack.rackId != excludeRackId)
            .OrderBy(rack => rack.rackId)
            .Select(rack => new RackEditOther(rack.rackId, new RackPoint(rack.entranceX, rack.entranceZ), new RackPoint(rack.farX, rack.farZ)))
            .ToList();

    // The editing rules for a proposed rack, without changing anything. The map uses this to show a proposal as valid or refused.
    public RackEditValidation ValidateRackEdit(int ownRackId, RackPoint entrance, RackPoint far)
        => RackEditing.Validate(RackEditBounds, entrance, far, RackEditOthers(ownRackId));

    // True when the plan already holds exactly the suggested routes (a reset would change nothing and lose nothing).
    public bool RackLayoutMatchesSuggestion(IReadOnlyList<SuggestedRackSegment> suggestion)
    {
        if (suggestion == null) return false;
        var routes = new List<KeyValuePair<RackPoint, RackPoint>>(suggestion.Count);
        foreach (SuggestedRackSegment segment in suggestion)
            routes.Add(new KeyValuePair<RackPoint, RackPoint>(segment.Entrance, segment.Far));
        return RackEditing.MatchesSuggestion(RackEditOthers(), routes);
    }

    private bool RackEditGate(int rackId, out RackPlanOperationResult refusal)
    {
        refusal = default;
        if (!RackPlanningV2Active)
        {
            refusal = RackPlanOperationResult.Invalid("Rack Planning V2 is not active in this world.", rackId);
            return false;
        }
        if (rackPlanConfirmed)
        {
            refusal = RackPlanOperationResult.Invalid("The rack plan is confirmed. Reopen it to change a rack.", rackId);
            return false;
        }
        return true;
    }

    // ---- operations -----------------------------------------------------------------------------

    // Adds a rack drawn on the map. The caller has already resolved the entrance onto the headland (RackEditing.ResolveCreation).
    public RackPlanOperationResult CreateRackFromMap(RackPoint entrance, RackPoint far)
    {
        if (!RackEditGate(0, out RackPlanOperationResult refusal)) return refusal;
        RackEditValidation check = ValidateRackEdit(0, entrance, far);
        if (!check.IsValid) return RackPlanOperationResult.Invalid(check.Message);
        RackPlanSnapshot before = RackPlanSnapshot.Capture(this);
        RackPlanOperationResult result = AddRack(entrance, far);
        if (result.Status == RackPlanOperationStatus.Applied)
            lastRackEdit = new RackEditUndo(RackEditKind.Create, before, RackPlanSnapshot.Capture(this), result.RackId);
        return result;
    }

    // Moves or rotates one rack (the same operation: a new centreline for the same identity).
    public RackPlanOperationResult ReshapeRackFromMap(int rackId, RackPoint entrance, RackPoint far, bool allowCancelApproved = false)
    {
        if (!RackEditGate(rackId, out RackPlanOperationResult refusal)) return refusal;
        if (FindRack(rackId) == null) return RackPlanOperationResult.Invalid($"There is no rack {rackId}.", rackId);
        if (RackIsLocked(rackId))
            return RackPlanOperationResult.Locked("This rack has completed work and can no longer be changed.", rackId);
        RackEditValidation check = ValidateRackEdit(rackId, entrance, far);
        if (!check.IsValid) return RackPlanOperationResult.Invalid(check.Message, rackId);
        RackPlanSnapshot before = RackPlanSnapshot.Capture(this);
        RackPlanOperationResult result = UpdateRack(rackId, entrance, far, allowCancelApproved);
        if (result.Status == RackPlanOperationStatus.Applied)
            lastRackEdit = new RackEditUndo(RackEditKind.Reshape, before, RackPlanSnapshot.Capture(this), rackId);
        return result;
    }

    // Confirms the plan from the map. This is the authoritative ConfirmRackPlan (it creates or refreshes Pending removal work only and
    // never approves or executes anything); the map's undo memory is dropped so a later reopen cannot undo a pre-confirmation edit.
    public RackPlanOperationResult ConfirmRackPlanFromMap(bool allowCancelApproved = false)
    {
        if (!CanConfirmRackPlan(out string reason))
            return RackPlanOperationResult.Invalid(reason);
        RackPlanOperationResult result = ConfirmRackPlan(allowCancelApproved);
        if (result.Succeeded) lastRackEdit = null;
        return result;
    }

    public RackPlanOperationResult DeleteRackFromMap(int rackId, bool allowCancelApproved = false)
    {
        if (!RackEditGate(rackId, out RackPlanOperationResult refusal)) return refusal;
        RackPlanSnapshot before = RackPlanSnapshot.Capture(this);
        RackPlanOperationResult result = DeleteRack(rackId, allowCancelApproved);
        if (result.Status == RackPlanOperationStatus.Applied)
            lastRackEdit = new RackEditUndo(RackEditKind.Delete, before, RackPlanSnapshot.Capture(this), rackId);
        return result;
    }

    // Replaces the player's racks with the suggested routes. Every route is checked first (the editing rules, the live stand's
    // removal trees as they would be once the current racks are gone); only then is anything changed. A rack with completed work
    // cannot be replaced, so such a plan is refused. The suggestion list is read, never modified.
    public RackPlanOperationResult ResetRacksToSuggestion(IReadOnlyList<SuggestedRackSegment> suggestion, bool allowCancelApproved = false)
    {
        if (!RackEditGate(0, out RackPlanOperationResult refusal)) return refusal;
        if (suggestion == null || suggestion.Count != RackPlanTargetCount)
            return RackPlanOperationResult.Invalid("There is no suggested layout to restore.");
        foreach (RackSaveData rack in racks)
            if (rack != null && RackIsLocked(rack.rackId))
                return RackPlanOperationResult.Locked("A rack with completed work cannot be replaced, so the plan cannot be reset to the suggested layout.", rack.rackId);
        if (RackLayoutMatchesSuggestion(suggestion))
            return RackPlanOperationResult.NoChange("The plan already matches the suggested layout.");
        if (!allowCancelApproved && racks.Any(rack => rack != null && RackOrderState(rack.rackId).HasApprovedWork))
            return RackPlanOperationResult.Warning(
                "Resetting would cancel approved but unexecuted removal work. Confirm that you want to cancel and recreate it.");

        RackEditBounds bounds = RackEditBounds;
        var currentIds = new HashSet<int>(racks.Where(rack => rack != null).Select(rack => rack.rackId));
        var chosen = new List<RackEditOther>(suggestion.Count);
        for (int i = 0; i < suggestion.Count; i++)
        {
            SuggestedRackSegment route = suggestion[i];
            RackEditValidation check = RackEditing.Validate(bounds, route.Entrance, route.Far, chosen);
            if (!check.IsValid)
                return RackPlanOperationResult.Invalid($"Suggested route {i + 1}: {check.Message}");
            if (!TryValidateRackGeometry(route.Entrance, route.Far, out string geometryProblem))
                return RackPlanOperationResult.Invalid($"Suggested route {i + 1}: {geometryProblem}");
            if (!TrySelectRemovalSet(route.Entrance, route.Far, 0, out _, out _, out string selectionProblem, currentIds))
                return RackPlanOperationResult.Invalid($"Suggested route {i + 1}: {selectionProblem}");
            chosen.Add(new RackEditOther(i + 1, route.Entrance, route.Far));
        }

        RackPlanSnapshot before = RackPlanSnapshot.Capture(this);
        foreach (RackSaveData rack in racks.Where(rack => rack != null).OrderBy(rack => rack.rackId).ToList())
        {
            RackPlanOperationResult removed = DeleteRack(rack.rackId, allowCancelApproved);
            if (removed.Status != RackPlanOperationStatus.Applied)
                return RollBackRackEdit(before, "Could not clear rack " + rack.rackId + ": " + removed.Message);
        }
        foreach (SuggestedRackSegment route in suggestion)
        {
            RackPlanOperationResult added = AddRack(route.Entrance, route.Far);
            if (added.Status != RackPlanOperationStatus.Applied)
                return RollBackRackEdit(before, "Could not add a suggested route: " + added.Message);
        }
        lastRackEdit = new RackEditUndo(RackEditKind.Reset, before, RackPlanSnapshot.Capture(this), 0);
        feedback = "Reset the rack plan to the suggested layout.";
        return RackPlanOperationResult.Applied(feedback, 0, racksAdded: suggestion.Count, racksRemoved: before.Racks.Count);
    }

    // Reverses exactly the last edit, if the plan is still exactly as that edit left it.
    public RackPlanOperationResult UndoLastRackEdit(bool allowCancelApproved = false)
    {
        if (!RackEditGate(0, out RackPlanOperationResult refusal)) return refusal;
        if (!CanUndoRackEdit)
        {
            lastRackEdit = null;
            return RackPlanOperationResult.NoChange("There is no rack edit to undo.");
        }
        RackEditUndo undo = lastRackEdit;
        RackPlanOperationResult result;
        switch (undo.Kind)
        {
            case RackEditKind.Create:
                result = DeleteRack(undo.RackId, allowCancelApproved);
                if (result.Status == RackPlanOperationStatus.Applied) nextRackId = undo.Before.NextRackId;
                break;
            case RackEditKind.Reshape:
            {
                RackGeometryRecord previous = undo.Before.Racks.First(rack => rack.Id == undo.RackId);
                result = UpdateRack(undo.RackId, previous.Entrance, previous.Far, allowCancelApproved);
                break;
            }
            case RackEditKind.Delete:
            {
                RackGeometryRecord previous = undo.Before.Racks.First(rack => rack.Id == undo.RackId);
                result = AddRackWithId(previous.Id, previous.Entrance, previous.Far);
                break;
            }
            default:
                if (!allowCancelApproved && racks.Any(rack => rack != null && RackOrderState(rack.rackId).HasApprovedWork))
                    return RackPlanOperationResult.Warning(
                        "Undoing the reset would cancel approved but unexecuted removal work. Confirm that you want to cancel and recreate it.");
                result = UndoReset(undo, allowCancelApproved);
                break;
        }
        if (result.Status == RackPlanOperationStatus.Applied)
        {
            lastRackEdit = null;
            feedback = "Undid " + DescribeUndone(undo.Kind, undo.RackId) + ".";
            return RackPlanOperationResult.Applied(feedback, undo.RackId, result.OrdersCreated, result.OrdersRemoved, result.OrdersCancelled,
                result.RacksAdded, result.RacksRemoved, result.RacksUpdated, result.MarksCleared);
        }
        // A warning leaves the plan untouched, so the undo stays available; any other refusal leaves nothing safe to retry.
        if (result.Status != RackPlanOperationStatus.RequiresApprovalWarning)
            lastRackEdit = null;
        return result;
    }

    private static string DescribeUndone(RackEditKind kind, int rackId)
    {
        switch (kind)
        {
            case RackEditKind.Create: return "adding rack " + rackId;
            case RackEditKind.Reshape: return "the change to rack " + rackId;
            case RackEditKind.Delete: return "deleting rack " + rackId;
            default: return "the reset to the suggested layout";
        }
    }

    private RackPlanOperationResult UndoReset(RackEditUndo undo, bool allowCancelApproved)
    {
        foreach (RackSaveData rack in racks.Where(rack => rack != null).OrderBy(rack => rack.rackId).ToList())
        {
            RackPlanOperationResult removed = DeleteRack(rack.rackId, allowCancelApproved);
            if (removed.Status != RackPlanOperationStatus.Applied)
                return RollBackRackEdit(undo.After, "Could not clear rack " + rack.rackId + ": " + removed.Message);
        }
        foreach (RackGeometryRecord previous in undo.Before.Racks)
        {
            RackPlanOperationResult added = AddRackWithId(previous.Id, previous.Entrance, previous.Far);
            if (added.Status != RackPlanOperationStatus.Applied)
                return RollBackRackEdit(undo.After, "Could not restore rack " + previous.Id + ": " + added.Message);
        }
        nextRackId = undo.Before.NextRackId;
        return RackPlanOperationResult.Applied("", 0, racksAdded: undo.Before.Racks.Count, racksRemoved: undo.After.Racks.Count);
    }

    // Puts the plan back to a captured geometry state after an operation stopped part-way. Only reached if a step the
    // up-front checks said would work fails anyway; it reports the failure instead of leaving a half-changed plan unmentioned.
    private RackPlanOperationResult RollBackRackEdit(RackPlanSnapshot target, string reason)
    {
        foreach (RackSaveData rack in racks.Where(rack => rack != null).OrderBy(rack => rack.rackId).ToList())
            if (!target.Racks.Any(record => record.Id == rack.rackId && record.Entrance.X == rack.entranceX && record.Entrance.Z == rack.entranceZ
                    && record.Far.X == rack.farX && record.Far.Z == rack.farZ))
                DeleteRack(rack.rackId, true);
        foreach (RackGeometryRecord record in target.Racks)
            if (FindRack(record.Id) == null)
                AddRackWithId(record.Id, record.Entrance, record.Far);
        nextRackId = target.NextRackId;
        lastRackEdit = null;
        return RackPlanOperationResult.Invalid(reason + " The rack plan was restored.");
    }

    // Re-adds a rack under an id it already held (undoing a delete or a reset), under the same checks as AddRack. A new rack
    // never reuses an id: this is only for returning the plan to a state it was already in, and the id must be one that was
    // allocated before (below nextRackId) and is free now.
    private RackPlanOperationResult AddRackWithId(int rackId, RackPoint entrance, RackPoint far)
    {
        if (!RackPlanningV2Active)
            return RackPlanOperationResult.Invalid("Rack Planning V2 is not active in this world.", rackId);
        if (rackId <= 0 || rackId >= nextRackId || FindRack(rackId) != null)
            return RackPlanOperationResult.Invalid($"Rack {rackId} cannot be restored.", rackId);
        if (racks.Count >= RackPlanTargetCount)
            return RackPlanOperationResult.Invalid($"The rack plan already holds {RackPlanTargetCount} racks.", rackId);
        if (!TryValidateRackGeometry(entrance, far, out string geometryProblem))
            return RackPlanOperationResult.Invalid(geometryProblem, rackId);
        if (!TrySelectRemovalSet(entrance, far, 0, out List<string> removalIds, out string entranceTreeId, out string selectionProblem))
            return RackPlanOperationResult.Invalid(selectionProblem, rackId);
        if (RemovalSetOverlapsOtherRack(0, removalIds))
            return RackPlanOperationResult.Invalid("Another rack already removes a tree on this line.", rackId);
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
        feedback = $"Restored rack {rackId} to the plan.";
        return RackPlanOperationResult.Applied(feedback, rackId, racksAdded: 1);
    }
}
