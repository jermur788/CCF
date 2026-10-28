using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

// Direct manipulation of the player's racks on the Stand Map (RACK-V2-05). It turns the pointer into rack edits and nothing more:
// the plan itself is ScenarioOneManager.RackPlan, every change goes through the facade in ScenarioOneManager.RackEditing.cs (which
// applies the editing rules and the existing AddRack / UpdateRack / DeleteRack), and the rules and snapping are RackEditing. This
// class keeps only presentation state: which rack is selected, the gesture under the pointer, the ghost drawn while dragging, the
// status line and a pending reset confirmation. None of it is saved.
//
// The gestures, all with the left button:
//   draw    press on empty map, drag, release   the end nearer the headland becomes the entrance, the other end the far end
//   select  click a rack                         the rack gets a bright outline, an entrance square and a round far-end handle
//   move    drag the body of a rack              it slides along the headland; its entrance re-snaps onto the headland
//   rotate  drag the round handle                the far end follows the pointer; the entrance stays put
// While dragging, the proposal is drawn as a dashed ghost (white if the rules accept it, red if they refuse it) and the plan is
// untouched; releasing applies it, or reports why it was refused. Snapping is soft and advisory; holding Shift turns it off.
public sealed class StandMapRackEditor
{
    private enum Gesture { None, Create, Move, Reshape }

    // A press that moves less than this is a click, not a drag.
    private const float DragStartPx = 4f;
    // How near the pointer must be to a rack's centre line to pick it, at least (its own half width can be more).
    private const float BodySlopPx = 9f;
    // Extra reach around the drawn handle, so the handle is easy to grab.
    private const float HandleSlackPx = 6f;

    private readonly ScenarioOneUiRoot ui;
    private readonly VisualElement layer;
    private readonly Action changed, repaint;
    private Gesture gesture = Gesture.None;
    private int gestureRackId;
    private int pointerId = -1;
    private Vector2 pressPx;
    private RackPoint pressWorld, originEntrance, originFar;
    private bool dragged;
    private List<float> suggestedAngles = new List<float>();
    private bool active;
    private RackEditGhost? ghost;

    // The rack whose outline and handle are shown, or 0.
    public int SelectedRackId { get; private set; }
    // The last thing an edit reported, and whether it was a refusal.
    public string Status { get; private set; } = "";
    public bool StatusIsProblem { get; private set; }
    // Bumped when the status changes for a reason other than a drag in progress, so the side card knows to rebuild.
    public int StatusVersion { get; private set; }
    // True while the reset confirmation is showing.
    public bool ResetPending { get; private set; }
    // A change that would cancel approved but unexecuted removal work waits here for an explicit yes (RACK-V2-07). The authoritative
    // plan has already refused it with a warning and changed nothing; "yes" repeats the same call with the consent flag, "no" drops it.
    // Presentation state only, never saved.
    private Func<RackPlanOperationResult> consentRetry;
    private string consentSuccess = "";
    public bool ConsentPending => consentRetry != null;
    public string ConsentMessage { get; private set; } = "";
    public bool Active => active;
    public bool Dragging => gesture != Gesture.None && dragged;
    public RackEditGhost? Ghost => ghost;
    // Raised for a status change that should only update the status label (a drag in progress), not rebuild the card.
    public event Action LiveStatusChanged;

    // `changed` rebuilds the map and its side card after a discrete action; `repaint` redraws just the map surface, for the many pointer
    // moves of a drag.
    public StandMapRackEditor(ScenarioOneUiRoot ui, VisualElement layer, Action changed, Action repaint)
    {
        this.ui = ui;
        this.layer = layer;
        this.changed = changed ?? (() => { });
        this.repaint = repaint ?? (() => { });
        layer.RegisterCallback<PointerDownEvent>(OnPointerDown);
        layer.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        layer.RegisterCallback<PointerUpEvent>(OnPointerUp);
        layer.RegisterCallback<PointerCancelEvent>(_ => Cancel());
        layer.RegisterCallback<PointerCaptureOutEvent>(_ => { if (gesture != Gesture.None) Cancel(); });
    }

    private ScenarioOneManager Manager => ui.Manager;

    // A string that changes whenever the side card has to be rebuilt.
    public string StateKey
        => SelectedRackId + "|" + (Manager != null ? Manager.PlannedRackCount + "|" + Manager.CanUndoRackEdit + "|" + Manager.RackEditingOpen : "")
            + "|" + ResetPending + "|" + ConsentPending + "|" + StatusVersion + "|" + active;

    // Shows or hides the pointer layer. Off whenever the map is not in V2 rack planning, or the plan cannot be edited.
    public void SetActive(bool on)
    {
        if (active == on) return;
        active = on;
        layer.style.display = on ? DisplayStyle.Flex : DisplayStyle.None;
        if (!on)
        {
            Cancel(false);
            ResetPending = false;
            consentRetry = null;
            ConsentMessage = "";
        }
    }

    public void ClearSelection()
    {
        SelectedRackId = 0;
        ui.MapSource.SelectedRackId = 0;
    }

    // Drops a selection whose rack no longer exists (deleted, or the world changed).
    public void Validate()
    {
        if (SelectedRackId > 0 && (Manager == null || !Manager.TryGetRack(SelectedRackId, out _)))
            ClearSelection();
    }

    public bool Select(int rackId)
    {
        if (Manager == null || !Manager.TryGetRack(rackId, out _)) return false;
        SelectedRackId = rackId;
        ui.MapSource.SelectedRackId = rackId;
        return true;
    }

    // ---- coordinates ----------------------------------------------------------------------------

    private StandMapTransform Transform => ui.MapSource.Model.TransformFor(StandMapView.MapSidePx);

    public Vector2 WorldToFramePx(RackPoint world)
    {
        MapPosition p = Transform.WorldToPixels(world);
        return new Vector2(p.X, p.Y);
    }

    public RackPoint FramePxToWorld(Vector2 framePx) => Transform.PixelsToWorld(new MapPosition(framePx.x, framePx.y));

    // ---- hit testing (the same geometry the draw plan paints) -----------------------------------

    public bool OnHandle(Vector2 framePx, RackPlanRack rack)
    {
        if (rack == null || rack.IsLocked) return false;
        float reach = StandMapDrawPlan.HandleRadiusPx(StandMapView.MapSidePx) + HandleSlackPx;
        return (WorldToFramePx(rack.Far) - framePx).magnitude <= reach;
    }

    // The player rack under the pointer, or 0. The nearest wins; a rack is picked within its own corridor or a little more.
    public int RackBodyAt(Vector2 framePx)
    {
        if (Manager == null) return 0;
        StandMapTransform t = Transform;
        int best = 0;
        float bestDistance = float.MaxValue;
        foreach (RackPlanRack rack in Manager.RackPlan)
        {
            Vector2 a = WorldToFramePx(rack.Entrance), b = WorldToFramePx(rack.Far);
            float distance = DistanceToSegment(framePx, a, b);
            float reach = Mathf.Max(BodySlopPx, t.MetresToPixels(rack.WidthM * 0.5f));
            if (distance <= reach && distance < bestDistance)
            {
                best = rack.Id;
                bestDistance = distance;
            }
        }
        return best;
    }

    private static float DistanceToSegment(Vector2 p, Vector2 a, Vector2 b)
    {
        Vector2 ab = b - a;
        float lengthSquared = ab.sqrMagnitude;
        float u = lengthSquared > 1e-6f ? Mathf.Clamp01(Vector2.Dot(p - a, ab) / lengthSquared) : 0f;
        return (p - (a + ab * u)).magnitude;
    }

    // ---- pointer --------------------------------------------------------------------------------

    private void OnPointerDown(PointerDownEvent e)
    {
        if (!active || e.button != 0 || gesture != Gesture.None || Manager == null) return;
        Vector2 px = e.localPosition;
        pressPx = px;
        pressWorld = FramePxToWorld(px);
        dragged = false;
        ghost = null;
        ResetPending = false;
        consentRetry = null;
        ConsentMessage = "";
        ScenarioOneManager manager = Manager;

        Gesture next = Gesture.None;
        if (SelectedRackId > 0 && manager.TryGetRack(SelectedRackId, out RackPlanRack selected) && OnHandle(px, selected))
        {
            next = Gesture.Reshape;
            BeginRackGesture(selected);
        }
        else
        {
            int hit = RackBodyAt(px);
            if (hit > 0 && manager.TryGetRack(hit, out RackPlanRack rack))
            {
                Select(hit);
                if (rack.IsLocked)
                {
                    SetStatus("Rack " + hit + " has been harvested. Its geometry is locked and cannot be changed.", true);
                }
                else
                {
                    next = Gesture.Move;
                    BeginRackGesture(rack);
                }
            }
            else
            {
                ClearSelection();
                if (manager.PlannedRackCount >= ScenarioOneManager.RackPlanTargetCount)
                    SetStatus("The plan already holds " + ScenarioOneManager.RackPlanTargetCount + " racks. Delete one to draw another.", false);
                else
                {
                    next = Gesture.Create;
                    gestureRackId = 0;
                }
            }
        }

        if (next != Gesture.None)
        {
            gesture = next;
            pointerId = e.pointerId;
            // The suggested orientation is read once per gesture: it is a scan of the plantation, too heavy for every pointer move.
            suggestedAngles = SuggestedAngles(manager);
            layer.CapturePointer(e.pointerId);
        }
        e.StopPropagation();
        changed();
    }

    private void BeginRackGesture(RackPlanRack rack)
    {
        gestureRackId = rack.Id;
        originEntrance = rack.Entrance;
        originFar = rack.Far;
    }

    private static List<float> SuggestedAngles(ScenarioOneManager manager)
    {
        var angles = new List<float>();
        foreach (SuggestedRackSegment segment in manager.GetSuggestedRackLayout())
            angles.Add(RackEditing.DirectedAngleDegrees(segment.Entrance, segment.Far));
        return angles;
    }

    private static bool ShiftHeld(EventModifiers modifiers)
        => (modifiers & EventModifiers.Shift) != 0 || (Keyboard.current != null && Keyboard.current.shiftKey.isPressed);

    private void OnPointerMove(PointerMoveEvent e)
    {
        if (gesture == Gesture.None || e.pointerId != pointerId) return;
        Vector2 px = e.localPosition;
        if (!dragged && (px - pressPx).sqrMagnitude < DragStartPx * DragStartPx) return;
        dragged = true;
        Propose(px, !ShiftHeld(e.modifiers), out RackPoint entrance, out RackPoint far, out RackSnapResult snap, out RackEditValidation check);
        ghost = new RackEditGhost(entrance, far, RackGeometry.DefaultWidthM, check.IsValid);
        ui.MapSource.EditGhost = ghost;
        ui.MapSource.MarkRackEditDirty();
        repaint();
        SetLiveStatus(Describe(entrance, far, snap, check), !check.IsValid);
        e.StopPropagation();
    }

    private void OnPointerUp(PointerUpEvent e)
    {
        if (e.button != 0 || gesture == Gesture.None || e.pointerId != pointerId) return;
        Gesture finished = gesture;
        int rackId = gestureRackId;
        bool wasDragged = dragged;
        // Releasing applies the proposal the player was last shown (the ghost), not a position read from the release event: when the
        // last pointer moves and the release land in one frame, the release can still carry the position from before them.
        // What you see when you let go is what you get.
        RackPoint entrance = default, far = default;
        bool haveProposal = ghost.HasValue;
        if (haveProposal)
        {
            entrance = ghost.Value.Start;
            far = ghost.Value.End;
        }
        EndGesture();
        if (wasDragged && haveProposal)
            Apply(finished, rackId, entrance, far);
        e.StopPropagation();
        changed();
    }

    private void Cancel(bool notify = true)
    {
        if (gesture == Gesture.None && !ghost.HasValue) return;
        EndGesture();
        if (notify) changed();
    }

    private void EndGesture()
    {
        // Clear the gesture BEFORE releasing capture: releasing raises PointerCaptureOut, whose handler cancels any live gesture.
        int captured = pointerId;
        gesture = Gesture.None;
        pointerId = -1;
        dragged = false;
        ghost = null;
        ui.MapSource.EditGhost = null;
        ui.MapSource.MarkRackEditDirty();
        if (captured >= 0 && layer.HasPointerCapture(captured))
            layer.ReleasePointer(captured);
    }

    // Where the proposal would be for a pointer at `px`, and whether the editing rules accept it.
    private void Propose(Vector2 px, bool snap, out RackPoint entrance, out RackPoint far, out RackSnapResult snapResult, out RackEditValidation check)
    {
        ScenarioOneManager manager = Manager;
        RackEditBounds bounds = manager.RackEditBounds;
        RackPoint pointer = FramePxToWorld(px);
        snapResult = default;
        switch (gesture)
        {
            case Gesture.Move:
                RackEditing.MoveAlongHeadland(bounds, originEntrance, originFar, originEntrance.Z + (pointer.Z - pressWorld.Z), out entrance, out far);
                break;
            case Gesture.Reshape:
                entrance = originEntrance;
                snapResult = RackEditing.SnapFar(bounds, entrance, pointer, suggestedAngles, manager.RackEditOthers(gestureRackId), snap);
                far = snapResult.Far;
                break;
            default:
            {
                RackEditing.ResolveCreation(bounds, pressWorld, pointer, out entrance, out RackPoint rawFar);
                snapResult = RackEditing.SnapFar(bounds, entrance, rawFar, suggestedAngles, manager.RackEditOthers(0), snap);
                far = snapResult.Far;
                break;
            }
        }
        check = manager.ValidateRackEdit(gesture == Gesture.Create ? 0 : gestureRackId, entrance, far);
    }

    private string Describe(RackPoint entrance, RackPoint far, RackSnapResult snap, RackEditValidation check)
    {
        if (!check.IsValid) return check.Message;
        float length = RackGeometry.LengthM(entrance, far);
        string text = UiKit.F(length, "0.0") + " m, pointing " + UiKit.F(Bearing(entrance, far), "0") + "° from north";
        if (snap.AngleSnapped)
            text += snap.ParallelToRackId > 0 ? " · parallel to rack " + snap.ParallelToRackId : " · along the suggested direction";
        if (snap.BoundarySnapped) text += " · to the stand edge";
        return text;
    }

    // Read the live ghost during a gesture, otherwise the selected authoritative rack. Suggestions are never included.
    public string SpacingText()
    {
        if (Manager == null) return "";
        RackPoint entrance, far;
        int ownId;
        if (Dragging && ghost.HasValue)
        {
            entrance = ghost.Value.Start;
            far = ghost.Value.End;
            ownId = gesture == Gesture.Create ? 0 : gestureRackId;
        }
        else if (SelectedRackId > 0 && Manager.TryGetRack(SelectedRackId, out RackPlanRack selected))
        {
            entrance = selected.Entrance;
            far = selected.Far;
            ownId = selected.Id;
        }
        else return "Select or draw a rack to see its nearest neighbour.";

        if (!RackEditing.NearestRack(entrance, far, Manager.RackEditOthers(ownId), out int neighbour, out float distance))
            return "No neighbouring player rack yet.";
        return "Nearest rack " + neighbour + ": " + UiKit.F(distance, "0.0") + " m — " + RackEditing.SpacingStatus(distance)
            + " (closest centre-line distance).";
    }

    // The compass bearing of a rack, in degrees from north (90 = east), as the map labels it.
    public static float Bearing(RackPoint entrance, RackPoint far)
    {
        float angle = RackEditing.DirectedAngleDegrees(entrance, far);
        return ((90f - angle) % 360f + 360f) % 360f;
    }

    // ---- applying an edit -----------------------------------------------------------------------

    private void Apply(Gesture finished, int rackId, RackPoint entrance, RackPoint far)
    {
        ScenarioOneManager manager = Manager;
        RackPlanOperationResult result;
        switch (finished)
        {
            case Gesture.Create:
                result = manager.CreateRackFromMap(entrance, far);
                if (result.Status == RackPlanOperationStatus.Applied)
                {
                    Select(result.RackId);
                    SetStatus("Added rack " + result.RackId + ".", false);
                }
                else Report(result, "");
                break;
            default:
                result = manager.ReshapeRackFromMap(rackId, entrance, far);
                Report(result, finished == Gesture.Move ? "Moved rack " + rackId + "." : "Rotated rack " + rackId + ".",
                    () => manager.ReshapeRackFromMap(rackId, entrance, far, true));
                break;
        }
        ui.MapSource.MarkRacksDirty();
    }

    private void Report(RackPlanOperationResult result, string success, Func<RackPlanOperationResult> retryWithConsent = null)
    {
        consentRetry = null;
        ConsentMessage = "";
        if (result.RequiresApprovalWarning && retryWithConsent != null)
        {
            // Nothing has changed yet. Ask, and keep the exact call to repeat if the player agrees.
            consentRetry = retryWithConsent;
            consentSuccess = success;
            ConsentMessage = result.Message;
            SetStatus("Waiting for your decision: " + result.Message, false);
            return;
        }
        string text = result.Status == RackPlanOperationStatus.Applied && !string.IsNullOrEmpty(success) ? success : result.Message;
        if (result.Status == RackPlanOperationStatus.NoChange && string.IsNullOrEmpty(text)) text = "Nothing changed.";
        SetStatus(text, !result.Succeeded);
    }

    private void SetStatus(string text, bool problem)
    {
        Status = text ?? "";
        StatusIsProblem = problem;
        StatusVersion++;
    }

    private void SetLiveStatus(string text, bool problem)
    {
        Status = text ?? "";
        StatusIsProblem = problem;
        LiveStatusChanged?.Invoke();
    }

    // ---- commands from the side card and the keyboard -------------------------------------------

    public void HandleKeys(Keyboard keyboard)
    {
        if (keyboard == null || !active || gesture != Gesture.None || SelectedRackId <= 0) return;
        if (keyboard.deleteKey.wasPressedThisFrame || keyboard.backspaceKey.wasPressedThisFrame)
            DeleteSelected();
    }

    public void DeleteSelected()
    {
        ResetPending = false;
        if (Manager == null) return;
        if (SelectedRackId <= 0)
        {
            SetStatus("Click a rack to select it first.", false);
            changed();
            return;
        }
        int id = SelectedRackId;
        RackPlanOperationResult result = Manager.DeleteRackFromMap(id);
        Report(result, "Deleted rack " + id + ".", () => Manager.DeleteRackFromMap(id, true));
        if (result.Status == RackPlanOperationStatus.Applied) ClearSelection();
        ui.MapSource.MarkRacksDirty();
        changed();
    }

    public void UndoLast()
    {
        ResetPending = false;
        if (Manager == null) return;
        RackPlanOperationResult result = Manager.UndoLastRackEdit();
        Report(result, result.Message, () => Manager.UndoLastRackEdit(true));
        if (result.Status == RackPlanOperationStatus.Applied)
        {
            // Show the rack the undo touched, if it still exists.
            if (result.RackId > 0 && Manager.TryGetRack(result.RackId, out _)) Select(result.RackId);
            else Validate();
        }
        ui.MapSource.MarkRacksDirty();
        changed();
    }

    // Reset to the suggested layout. If the plan holds racks that differ from it the player is asked first; with nothing to lose
    // (an empty plan) it applies at once, and with the plan already equal to the suggestion nothing is done.
    public void RequestReset()
    {
        if (Manager == null) return;
        IReadOnlyList<SuggestedRackSegment> suggestion = Manager.GetSuggestedRackLayout();
        if (suggestion.Count == 0)
        {
            SetStatus("There is no suggested layout to restore.", true);
        }
        else if (Manager.RackLayoutMatchesSuggestion(suggestion))
        {
            ResetPending = false;
            SetStatus("The plan already matches the suggested layout.", false);
        }
        else if (Manager.PlannedRackCount > 0)
        {
            ResetPending = true;
            StatusVersion++;
        }
        else
        {
            ApplyReset(suggestion);
        }
        changed();
    }

    public void ConfirmReset()
    {
        if (Manager == null) return;
        ApplyReset(Manager.GetSuggestedRackLayout());
        changed();
    }

    public void CancelReset()
    {
        ResetPending = false;
        StatusVersion++;
        changed();
    }

    // The warning the plan gave for a change that needs the player's consent, shown by a command that is not an edit gesture (Confirm
    // Rack Plan). Same rules as an edit: nothing changed yet; yes repeats the call with consent, no leaves everything as it was.
    public void AskConsent(RackPlanOperationResult warning, string successText, Func<RackPlanOperationResult> retryWithConsent)
    {
        ResetPending = false;
        Report(warning, successText, retryWithConsent);
        changed();
    }

    // Lets the Stand Map report the result of a plan-level command (confirm, reopen) in the same status line.
    public void NoteStatus(string text, bool problem)
    {
        ResetPending = false;
        SetStatus(text, problem);
    }

    public void AcceptConsent()
    {
        if (consentRetry == null) return;
        Func<RackPlanOperationResult> retry = consentRetry;
        string success = consentSuccess;
        consentRetry = null;
        ConsentMessage = "";
        RackPlanOperationResult result = retry();
        Report(result, success);
        if (result.Status == RackPlanOperationStatus.Applied) ClearSelection();
        ui.MapSource.MarkRacksDirty();
        ui.RackLayout?.Update(true);
        changed();
    }

    public void DeclineConsent()
    {
        if (consentRetry == null) return;
        consentRetry = null;
        ConsentMessage = "";
        SetStatus("Nothing was changed. The approved work stays as it is.", false);
        changed();
    }

    private void ApplyReset(IReadOnlyList<SuggestedRackSegment> suggestion)
    {
        ResetPending = false;
        RackPlanOperationResult result = Manager.ResetRacksToSuggestion(suggestion);
        Report(result, "Reset the plan to the suggested layout.", () => Manager.ResetRacksToSuggestion(suggestion, true));
        if (result.Status == RackPlanOperationStatus.Applied) ClearSelection();
        ui.MapSource.MarkRacksDirty();
    }
}
