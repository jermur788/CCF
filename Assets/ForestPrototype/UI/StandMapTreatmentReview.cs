using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Review = ScenarioOneTreatmentReview;
using Cov = ScenarioOneCoverage;
using Cand = ScenarioOneCropTreeCandidates;
using Appr = ScenarioOnePrescriptionApproval;

// The Stand Map "Treatment review" layer's card and its 3D companion (MFM-08). Presentation only.
//
// It reads production state through the existing boundaries and writes nothing of its own:
//   coverage     ScenarioOneManager.ReadCoverage (the thin side-effect-free read; no second adapter, no Classify call here)
//   candidates   ScenarioOneCropTreeCandidates via Review.Session (spacing-only prompts; the map never confirms one)
//   prescription CreatePrescription / PreviewPrescription / ApprovePrescription (MFM-07). A proposal is never a Fell mark.
// The Work Plan is never opened, approved, funded or run from here. Confirming a Crop Tree is the normal [C] mark in the forest.
public sealed class StandMapTreatmentReview
{
    private readonly ScenarioOneUiRoot ui;
    private readonly Func<float, float, bool> setWaypointAt;
    private readonly Action refreshMap;
    private readonly Review.Session session = new Review.Session();
    private Cov.Result coverage;
    private string coverageProblem = "";
    private List<Cov.Stem> stems = new List<Cov.Stem>();
    private string lastNotedTreeId = "";
    private int version;

    public Review.Session Session => session;
    // Changes whenever the review state changes, so the map knows to rebuild the card.
    public int Version => version;
    public Cov.Result Coverage => coverage;

    public StandMapTreatmentReview(ScenarioOneUiRoot ui, Func<float, float, bool> setWaypointAt, Action refreshMap)
    {
        this.ui = ui;
        this.setWaypointAt = setWaypointAt;
        this.refreshMap = refreshMap;
    }

    private void Touch() => version++;

    // ----------------------------------------------------------------------------------------------------------- reading

    // One read of the production coverage, the live trees and (if a prescription is under review) its read-only preview.
    private void ReadAll()
    {
        ScenarioOneManager manager = ui.Manager;
        coverage = manager != null ? manager.ReadCoverage(out coverageProblem) : null;
        if (manager == null) coverageProblem = "The Scenario One manager is not available.";
        stems = ReadStems();
        session.PruneTo(coverage);
        if (coverage != null && coverage.Valid) session.Suggest(coverage, stems);
        else session.LastCandidates = null;
        if (session.ActivePrescriptionId > 0)
        {
            PrescriptionSaveData record = manager.FindPrescription(session.ActivePrescriptionId);
            if (record == null) session.ReplaceActivePrescription(0);
            else
            {
                if (record.approvalState == PrescriptionApprovalState.Approved) session.MarkApproved();
                session.LastPreview = session.ActiveApproved ? null : manager.PreviewPrescription(session.ActivePrescriptionId);
            }
        }
    }

    // Only what the candidate rule needs: the living trees with their position, species and Crop Tree / Fell mark state.
    private static List<Cov.Stem> ReadStems()
    {
        var list = new List<Cov.Stem>();
        foreach (ForestTree tree in UnityEngine.Object.FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
        {
            if (tree == null || string.IsNullOrEmpty(tree.TreeId) || !tree.IsLiving) continue;
            Vector3 p = tree.transform.position;
            list.Add(new Cov.Stem(tree.TreeId, p.x, p.z, tree.Diameter, true, tree.Species != null ? tree.Species.SpeciesId : "", tree.IsCropTree, tree.IsMarkedForFell));
        }
        return list;
    }

    private List<string> AcceptedUnits(PrescriptionSaveData record)
    {
        var ids = new List<string>();
        if (record != null)
            foreach (PrescriptionCoverageDecisionSaveData d in record.coverageDecisions)
                if (d.choice == PrescriptionCoverageChoice.AcceptedCovered) ids.Add(d.unitId);
        return ids;
    }

    // ---------------------------------------------------------------------------------------------------------- the map layer

    public void HideLayer(StandMapReviewLayer layer) => layer.Hide();

    public void ShowLayer(StandMapReviewLayer layer, StandMapModel model, Review.Presentation view)
    {
        List<ReviewFootprint> footprints = Footprints(model, view);
        List<ReviewShape> shapes = StandMapReviewPlan.Areas(footprints);
        if (view.Valid && model != null && model.HasProperty)
        {
            StandMapTransform t = model.TransformFor(StandMapView.MapSidePx);
            if (session.LastCandidates != null && session.LastCandidates.Valid)
                foreach (Cand.Candidate c in session.LastCandidates.Candidates)
                {
                    MapPosition at = t.WorldToPixels(new RackPoint(c.X, c.Z));
                    Cand.Decision d = session.DecisionNow(c.TreeId, c.Decision, IsCrop(c.TreeId));
                    shapes.AddRange(StandMapReviewPlan.CandidateGlyph(c.TreeId, at.X, at.Y, d, c.TreeId == session.CurrentCandidateTreeId));
                }
            Appr.ProposalSet preview = session.LastPreview;
            if (preview != null && preview.Valid)
            {
                var position = new Dictionary<string, Cov.Stem>(StringComparer.Ordinal);
                foreach (Cov.Stem s in stems) position[s.Id] = s;
                foreach (ScenarioOnePrescription.Proposal p in preview.Proposals)
                    if (position.TryGetValue(p.TreeId, out Cov.Stem s))
                    {
                        MapPosition at = t.WorldToPixels(new RackPoint(s.X, s.Z));
                        shapes.AddRange(StandMapReviewPlan.ProposalGlyph(p.TreeId, at.X, at.Y, false));
                    }
                foreach (Appr.Conflict c in preview.Conflicts)
                    if (!string.IsNullOrEmpty(c.TreeId) && position.TryGetValue(c.TreeId, out Cov.Stem s))
                    {
                        MapPosition at = t.WorldToPixels(new RackPoint(s.X, s.Z));
                        shapes.AddRange(StandMapReviewPlan.ProposalGlyph(c.TreeId, at.X, at.Y, true));
                    }
            }
        }
        layer.Show(shapes, footprints);
    }

    private bool IsCrop(string treeId)
    {
        foreach (Cov.Stem s in stems) if (s.Id == treeId) return s.Crop;
        return false;
    }

    // Footprints are drawn from the SAME pure rack geometry production coverage is built on; their state, reasons and tags come only from
    // the coverage result. A footprint that does not match its coverage area (rack id) is not drawn rather than guessed.
    private List<ReviewFootprint> Footprints(StandMapModel model, Review.Presentation view)
    {
        var list = new List<ReviewFootprint>();
        if (!view.Valid || model == null || !model.HasProperty || ui.Ecology == null || ui.Manager == null) return list;
        float half = StandGeometryModel.StandSizeMeters(ui.Ecology.StandGeometryModelVersion) * 0.5f;
        ScenarioOneRackSampleGeometry.Result geometry = ScenarioOneRackSampleGeometry.Derive(ui.Manager.ConfirmedRacks, half);
        if (!geometry.Valid) return list;
        StandMapTransform t = model.TransformFor(StandMapView.MapSidePx);
        foreach (Review.AreaRow row in view.All())
        {
            IReadOnlyList<RackPoint> polygon = null;
            if (row.IsRemainder)
            {
                foreach (Cov.Remainder r in coverage.Remainders)
                {
                    if (r.Id != row.Id) continue;
                    ScenarioOneRackSampleGeometry.OuterRemainder shape = r.Side == ScenarioOneRackSampleGeometry.Side.Right ? geometry.RightRemainder : geometry.LeftRemainder;
                    if (shape != null && !shape.IsEmpty) polygon = shape.Polygon;
                }
            }
            else
            {
                Cov.Area area = coverage.Find(row.Id);
                foreach (ScenarioOneRackSampleGeometry.CoverageUnit unit in geometry.Units)
                    if (area != null && unit.Index == area.UnitIndex && unit.RackId == area.RackId) polygon = unit.Polygon;
            }
            if (polygon == null || polygon.Count < 3) continue;
            var fp = new ReviewFootprint { AreaId = row.Id, State = row.State, IsSource = row.IsSource, Tag = row.MapTag, Selected = row.Id == session.SelectedAreaId };
            foreach (RackPoint p in polygon)
            {
                MapPosition m = t.WorldToPixels(p);
                fp.Polygon.Add(new ReviewPoint(m.X, m.Y));
            }
            list.Add(fp);
        }
        return list;
    }

    // World centre of an area's footprint, for Inspect in forest.
    private bool TryAreaCentre(string areaId, bool remainder, out float x, out float z)
    {
        x = z = 0f;
        if (ui.Ecology == null || ui.Manager == null || coverage == null) return false;
        float half = StandGeometryModel.StandSizeMeters(ui.Ecology.StandGeometryModelVersion) * 0.5f;
        ScenarioOneRackSampleGeometry.Result geometry = ScenarioOneRackSampleGeometry.Derive(ui.Manager.ConfirmedRacks, half);
        if (!geometry.Valid) return false;
        IReadOnlyList<RackPoint> polygon = null;
        if (!remainder)
        {
            Cov.Area area = coverage.Find(areaId);
            foreach (ScenarioOneRackSampleGeometry.CoverageUnit unit in geometry.Units)
                if (area != null && unit.Index == area.UnitIndex && unit.RackId == area.RackId) polygon = unit.Polygon;
        }
        if (polygon == null || polygon.Count == 0) return false;
        foreach (RackPoint p in polygon) { x += p.X; z += p.Z; }
        x /= polygon.Count;
        z /= polygon.Count;
        return true;
    }

    // ------------------------------------------------------------------------------------------------------------- the card

    public Review.Presentation BuildCard(VisualElement side)
    {
        ReadAll();
        Review.Presentation view = Review.Build(coverage, coverageProblem, session);
        ScrollView scroll = new ScrollView(ScrollViewMode.Vertical) { name = "map-review-scroll" };
        scroll.style.flexGrow = 1;
        // The card is as tall as the map frame at most, so a long review scrolls inside it instead of running off the window at 720p.
        scroll.style.maxHeight = StandMapView.MapSidePx - 40f;
        side.Add(scroll);
        VisualElement body = scroll.contentContainer;

        UiKit.Add(body, "Treatment review", "heading");
        UiKit.Add(body, Review.ApplyWording, "body");
        UiKit.Add(body, Review.NothingCommittedNote, "faint");
        if (!string.IsNullOrEmpty(session.Message)) UiKit.Add(body, session.Message, "map-note").name = "map-review-message";

        if (!view.Valid)
        {
            // A failed production read is shown as it came back and every action stays off. There is no fallback coverage.
            UiKit.Add(body, "Coverage cannot be shown: " + view.Problem, "body").name = "map-review-problem";
            UiKit.Add(body, "Treatment review stays off until this is resolved. Nothing was changed.", "muted");
            return view;
        }

        BuildSample(body, view);
        BuildLegend(body);
        BuildAreas(body, view);
        BuildCandidates(body);
        BuildPrescription(body, view);
        return view;
    }

    private void BuildSample(VisualElement body, Review.Presentation view)
    {
        Review.AreaRow sample = view.Sample;
        UiKit.Add(body, Review.SampleLabel, "heading").name = "map-review-sample";
        if (sample == null) { UiKit.Add(body, "No sample unit was found.", "muted"); return; }
        UiKit.Add(body, sample.Title + " · " + sample.Metrics, "body");
        UiKit.Add(body, "This is the treatment you demonstrated. It is the source, not a target.", "faint");
    }

    private static void BuildLegend(VisualElement body)
    {
        UiKit.Add(body, "Map key (shape and pattern, not colour alone)", "heading");
        UiKit.Add(body, "SAMPLE: heavy outline, crossed diagonal hatch · COVERED: solid outline, one diagonal hatch · NOT COVERED: dashed outline · "
            + "EXCLUDED: solid outline, square-grid hatch · MANUAL: dotted outline.", "muted");
        UiKit.Add(body, "CANDIDATE: hollow square (inspection prompt) · PROPOSAL: hollow triangle (not a Fell mark) · CONFLICT: triangle with a bar.", "muted");
        UiKit.Add(body, Review.CoveredNote, "faint");
    }

    private void BuildAreas(VisualElement body, Review.Presentation view)
    {
        UiKit.Add(body, "Comparable areas", "heading");
        UiKit.Add(body, view.Summary, "body").name = "map-review-summary";
        foreach (Review.AreaRow row in view.Targets) BuildAreaRow(body, row);
        foreach (Review.AreaRow row in view.Remainders) BuildAreaRow(body, row);
    }

    private void BuildAreaRow(VisualElement body, Review.AreaRow row)
    {
        VisualElement box = UiKit.Box("card", "rack-panel");
        box.name = "map-review-area-" + row.Id;
        if (row.Id == session.SelectedAreaId) box.style.borderLeftWidth = 4;
        UiKit.Add(box, "[" + row.StateLabel + "] " + row.Title, "rack-heading");
        UiKit.Add(box, row.Meaning, "muted");
        foreach (string reason in row.Reasons) UiKit.Add(box, "• " + reason, "body");
        foreach (string note in row.Notes) UiKit.Add(box, note, "faint");
        if (!string.IsNullOrEmpty(row.Comparison)) UiKit.Add(box, row.Comparison, "faint");
        if (!string.IsNullOrEmpty(row.Metrics)) UiKit.Add(box, row.Metrics, "faint");
        if (row.CanChoose)
        {
            // Classification (above) and the player's draft choice (here) are separate things.
            UiKit.Add(box, Review.ChoiceText(row.Choice), "muted");
            VisualElement choices = UiKit.Row(box, "row-wrap");
            string id = row.Id;
            choices.Add(UiKit.Button("Apply sample treatment here", () => Choose(id, Review.Choice.Include), true,
                row.Choice == Review.Choice.Include ? new[] { "btn-selected" } : new string[0]));
            choices.Add(UiKit.Button("Leave manual", () => Choose(id, Review.Choice.LeaveManual), true,
                row.Choice == Review.Choice.LeaveManual ? new[] { "btn-selected" } : new string[0]));
        }
        VisualElement nav = UiKit.Row(box, "row-wrap");
        string areaId = row.Id;
        bool remainder = row.IsRemainder;
        nav.Add(UiKit.Button("Show on map", () => { session.SelectedAreaId = areaId; Touch(); refreshMap(); }, true, "btn-ghost"));
        if (!row.IsSource && !row.IsRemainder)
            nav.Add(UiKit.Button("Inspect in forest", () => InspectArea(areaId, remainder), true, "btn-ghost"));
        body.Add(box);
    }

    private void Choose(string areaId, Review.Choice choice)
    {
        // Presentation draft only: no mark, order or record changes here.
        session.SetChoice(coverage, areaId, choice);
        session.Message = "";
        Touch();
        refreshMap();
    }

    private void InspectArea(string areaId, bool remainder)
    {
        session.SelectedAreaId = areaId;
        if (TryAreaCentre(areaId, remainder, out float x, out float z) && setWaypointAt(x, z))
            session.Message = "Waypoint set. Walk there yourself, then inspect a tree in that area. Setting a waypoint does not count as the 3D spot-check.";
        else
            session.Message = "That area could not be placed on the waypoint grid.";
        Touch();
        refreshMap();
    }

    private void BuildCandidates(VisualElement body)
    {
        UiKit.Add(body, "Crop Tree candidates", "heading");
        UiKit.Add(body, Review.CandidateWarning, "body");
        Review.CandidateCard card = Review.BuildCandidateCard(session.LastCandidates, session, IsCrop);
        if (!card.Valid) { UiKit.Add(body, "Candidates are not available: " + card.Problem, "muted"); return; }
        if (card.Total == 0) { UiKit.Add(body, "No spacing candidates in the covered areas.", "muted"); return; }
        UiKit.Add(body, card.Open + " open · " + card.Confirmed + " confirmed · " + card.Rejected + " rejected", "muted").name = "map-review-candidate-counts";
        VisualElement box = UiKit.Box("card", "rack-panel");
        box.name = "map-review-candidate";
        if (card.Current == null)
            UiKit.Add(box, "Use Next to step through the candidates one at a time.", "body");
        else
        {
            UiKit.Add(box, Review.CandidateChip, "rack-heading");
            UiKit.Add(box, card.Position + " · tree " + card.Current.TreeId + " · " + card.Current.AreaId, "body");
            UiKit.Add(box, card.StateText, "body");
        }
        VisualElement nav = UiKit.Row(box, "row-wrap");
        nav.Add(UiKit.Button("◀ Previous", () => MoveCandidate(Cand.Navigation.Previous), card.CanPrevious));
        nav.Add(UiKit.Button("Next ▶", () => MoveCandidate(Cand.Navigation.Next), card.CanNext));
        Button inspect = UiKit.Button("Inspect in forest", InspectCandidate, card.Current != null, "btn-primary");
        inspect.name = "map-review-candidate-inspect";
        box.Add(inspect);
        UiKit.Add(box, "To judge: walk there, inspect the tree, then press [C] to confirm it as a Crop Tree or [N] to reject the candidate. The map never confirms a tree, and there is no confirm-all.", "faint");
        body.Add(box);
    }

    private void MoveCandidate(Cand.Navigation direction)
    {
        session.Move(direction);
        session.Message = "";
        Touch();
        refreshMap();
    }

    private void InspectCandidate()
    {
        Cand.Result batch = session.LastCandidates;
        if (!Cand.TryGetInspectionTarget(batch, session.CurrentCandidateTreeId, out Cand.InspectionTarget target))
        {
            session.Message = "That candidate has already been decided.";
        }
        else if (setWaypointAt(target.X, target.Z))
            session.Message = "Waypoint set near tree " + target.TreeId + ". Walk there yourself and inspect it in the forest.";
        else
            session.Message = "That candidate could not be placed on the waypoint grid.";
        Touch();
        refreshMap();
    }

    private void BuildPrescription(VisualElement body, Review.Presentation view)
    {
        ScenarioOneManager manager = ui.Manager;
        UiKit.Add(body, "Prescription", "heading");
        int included = session.IncludedCount;
        UiKit.Add(body, included + " area" + (included == 1 ? "" : "s") + " chosen to apply the sample treatment (draft).", "body");
        Button prepare = UiKit.Button("Prepare prescription for review", PreparePrescription, included > 0, "btn-primary");
        prepare.name = "map-review-prepare";
        body.Add(prepare);
        UiKit.Add(body, "This saves your coverage choices for review only. It marks nothing and adds no work. Changing your choices later prepares a new one.", "faint");

        if (session.ActivePrescriptionId <= 0)
        {
            PrescriptionSaveData latest = null;
            foreach (PrescriptionSaveData p in manager.Prescriptions)
                if (p != null && p.approvalState == PrescriptionApprovalState.Accepted) latest = p;
            if (latest != null)
            {
                int id = latest.prescriptionId;
                body.Add(UiKit.Button("Review prepared prescription " + id, () => { session.ReplaceActivePrescription(id); session.Message = ""; Touch(); refreshMap(); }, true, "btn-ghost"));
            }
            return;
        }

        PrescriptionSaveData record = manager.FindPrescription(session.ActivePrescriptionId);
        UiKit.Add(body, "Reviewing prescription " + session.ActivePrescriptionId + (session.ActiveApproved ? " (approved)" : " (prepared, not approved)"), "rack-heading").name = "map-review-active";
        BuildSpotCheck(body, view, record);
        BuildProposals(body);
        BuildApproval(body);
    }

    private void PreparePrescription()
    {
        PrescriptionOperationResult result = ui.Manager.CreatePrescription(session.IncludedUnitIds(), session.LeftManualUnitIds());
        session.Message = result.Message;
        if (result.Succeeded) session.ReplaceActivePrescription(result.PrescriptionId);
        lastNotedTreeId = "";
        Touch();
        refreshMap();
    }

    private void BuildSpotCheck(VisualElement body, Review.Presentation view, PrescriptionSaveData record)
    {
        UiKit.Add(body, "3D spot-check", "heading");
        bool done = session.SpotCheckDone;
        UiKit.Add(body, done ? "3D spot-check complete" : "3D spot-check not done yet: inspect a tree in at least one of these areas in the forest.", "body").name = "map-review-spotcheck";
        if (session.ActiveApproved) return;
        foreach (string unitId in AcceptedUnits(record))
        {
            bool checkedUnit = false;
            foreach (string c in session.SpotCheckedUnits) if (c == unitId) checkedUnit = true;
            string title = unitId;
            foreach (Review.AreaRow row in view.Targets) if (row.Id == unitId) title = row.Title;
            VisualElement line = UiKit.Row(body, "row-wrap");
            line.Add(UiKit.Text((checkedUnit ? "Checked in 3D: " : "Not checked: ") + title, "body"));
            string id = unitId;
            if (!checkedUnit) line.Add(UiKit.Button("Inspect in forest", () => InspectArea(id, false), true, "btn-ghost"));
        }
    }

    private void BuildProposals(VisualElement body)
    {
        UiKit.Add(body, "Proposals", "heading");
        if (session.ActiveApproved) { UiKit.Add(body, "These are now ordinary Pending tasks in the Work Plan.", "muted"); return; }
        Appr.ProposalSet preview = session.LastPreview;
        if (preview == null || !preview.Valid)
        {
            UiKit.Add(body, "Proposals cannot be read now: " + (preview != null ? preview.Message : "no preview"), "muted");
            return;
        }
        List<Review.ProposalRow> rows = Review.BuildProposalRows(preview);
        if (rows.Count == 0) UiKit.Add(body, "The rule proposes nothing for the chosen areas right now.", "muted");
        foreach (Review.ProposalRow row in rows)
        {
            VisualElement box = UiKit.Box("card", "rack-panel");
            UiKit.Add(box, row.Label, "rack-heading");
            UiKit.Add(box, "Tree " + (string.IsNullOrEmpty(row.TreeId) ? "(whole unit)" : row.TreeId) + " · " + row.AreaId + (string.IsNullOrEmpty(row.CropTreeId) ? "" : " · serves Crop Tree " + row.CropTreeId), "body");
            UiKit.Add(box, row.Reason, "body");
            if (!string.IsNullOrEmpty(row.Quantity)) UiKit.Add(box, row.Quantity, "faint");
            body.Add(box);
        }
    }

    private void BuildApproval(VisualElement body)
    {
        Review.ApprovalGate gate = Review.EvaluateGate(session, session.LastPreview);
        UiKit.Add(body, "Approve prescription", "heading");
        Button approve = UiKit.Button("Approve prescription", ApprovePrescription, gate.CanApprove, "btn-primary");
        approve.name = "map-review-approve";
        body.Add(approve);
        if (!gate.CanApprove && !string.IsNullOrEmpty(gate.Reason)) UiKit.Add(body, gate.Reason, "muted").name = "map-review-gate";
        UiKit.Add(body, "Approving adds Pending tasks only. They are not approved for execution until you approve the Work Plan.", "faint");
    }

    private void ApprovePrescription()
    {
        // The gate is checked again here: the button state alone is not the rule.
        Review.ApprovalGate gate = Review.EvaluateGate(session, session.LastPreview);
        if (!gate.CanApprove) { session.Message = gate.Reason; Touch(); refreshMap(); return; }
        PrescriptionOperationResult result = ui.Manager.ApprovePrescription(session.ActivePrescriptionId);
        if (result.Succeeded)
        {
            session.MarkApproved();
            session.Message = Review.ApprovedMessage + " (" + result.OrdersCreated + " task" + (result.OrdersCreated == 1 ? "" : "s") + (result.ConflictsSurfaced > 0 ? ", " + result.ConflictsSurfaced + " held" : "") + ")";
        }
        else session.Message = result.Message;
        Touch();
        refreshMap();
    }

    // ------------------------------------------------------------------------------------------------------- the 3D companion

    // Called by the tree inspection card for the tree the player is looking at. Null when it is neither a candidate nor a proposal target.
    public Review.TreeContext ContextFor(ForestTree tree)
    {
        if (tree == null) return null;
        Review.TreeContext context = Review.DescribeTree(tree.TreeId, tree.IsCropTree, session);
        return context.Any ? context : null;
    }

    // Called once per frame by the tree inspection card while a tree is inspected. Records the 3D spot-check (an actual inspection of a
    // tree in an accepted, still Covered unit) and handles the contextual reject key for a candidate. [C] stays the normal Crop Tree mark.
    public void OnInspected(ForestTree tree)
    {
        if (tree == null || !tree.IsLiving || ui.Help == null) return;
        Keyboard keys = Keyboard.current;
        if (keys != null && keys.nKey.wasPressedThisFrame && !ui.Help.IsOpen)
        {
            Review.TreeContext context = Review.DescribeTree(tree.TreeId, tree.IsCropTree, session);
            if (context.IsCandidate)
            {
                if (context.Decision == Cand.Decision.Open) session.Reject(tree.TreeId);
                else if (context.Decision == Cand.Decision.Rejected) session.Reopen(tree.TreeId);
                Touch();
            }
        }
        if (session.ActivePrescriptionId <= 0 || session.ActiveApproved || tree.TreeId == lastNotedTreeId) return;
        lastNotedTreeId = tree.TreeId;
        PrescriptionSaveData record = ui.Manager.FindPrescription(session.ActivePrescriptionId);
        Cov.Result fresh = ui.Manager.ReadCoverage(out _);
        if (record != null && record.approvalState == PrescriptionApprovalState.Accepted && session.RecordSpotCheck(tree.TreeId, fresh, AcceptedUnits(record)))
            Touch();
    }
}
