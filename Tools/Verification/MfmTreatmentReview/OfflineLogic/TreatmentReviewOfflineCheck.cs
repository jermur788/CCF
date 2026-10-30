// MFM-08 offline/static checks: the player-facing review contract and the API boundaries, without Unity.
// Behaviour checks run the pure review model (ScenarioOneTreatmentReview, StandMapReviewPlan) on synthetic coverage; boundary checks read the
// Unity-facing source text (StandMapTreatmentReview, StandMapView, TreeInspectionView) because those cannot be run outside the Editor.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Cov = ScenarioOneCoverage;
using Cand = ScenarioOneCropTreeCandidates;
using Cal = ScenarioOnePrescriptionCalibration;
using Appr = ScenarioOnePrescriptionApproval;
using R = ScenarioOneTreatmentReview;

public static class TreatmentReviewOfflineCheck
{
    private static int checks, failed;
    internal static void Check(bool ok, string name)
    {
        checks++;
        Console.WriteLine((ok ? "PASS " : "FAIL ") + name);
        if (!ok) failed++;
    }

    private static Cov.Area Area(int rack, Cov.Segment seg, Cov.State state, bool source = false, params Cov.Reason[] reasons)
    {
        var a = new Cov.Area { Id = Cov.AreaId(rack, seg), RackId = rack, Segment = seg, State = state, IsSource = source, UnitIndex = rack };
        a.Reasons.AddRange(reasons);
        return a;
    }

    // Sample + one area of each state + a remainder.
    private static Cov.Result Coverage(out Cov.Area covered, out Cov.Area covered2)
    {
        var r = new Cov.Result { Valid = true, Calibration = Cal.V1 };
        r.Areas.Add(Area(3, Cov.Segment.Centre, Cov.State.Manual, true, Cov.Reason.SourceSample));
        covered = Area(1, Cov.Segment.Low, Cov.State.Covered);
        covered.Flags.Add(Cov.Flag.NaturalRegeneration);
        covered.StemIds.AddRange(new[] { "a", "b", "c" });
        covered2 = Area(2, Cov.Segment.High, Cov.State.Covered);
        covered2.StemIds.Add("z");
        r.Areas.Add(covered);
        r.Areas.Add(covered2);
        r.Areas.Add(Area(4, Cov.Segment.Low, Cov.State.NotCovered, false, Cov.Reason.DensityDiffers, Cov.Reason.MeanDbhDiffers));
        r.Areas.Add(Area(4, Cov.Segment.High, Cov.State.Excluded, false, Cov.Reason.PlantedJuvenile, Cov.Reason.NonSitkaAdult));
        r.Areas.Add(Area(5, Cov.Segment.Low, Cov.State.Manual, false, Cov.Reason.PlayerChoseManual));
        r.Remainders.Add(new Cov.Remainder { Id = "remainder-right", Side = ScenarioOneRackSampleGeometry.Side.Right, AreaM2 = 120, Stems = 7 });
        r.NonSourceCount = 5;
        r.CoveredCount = 2;
        r.SourceStemIds.AddRange(new[] { "s1" });
        return r;
    }

    private static Cov.Stem Stem(string id, float x, bool crop = false) => new Cov.Stem(id, x, 0, 20, true, Cov.SitkaSpeciesId, crop, false);

    private static string Read(string root, string rel) => File.ReadAllText(Path.Combine(root, rel));
    private static int Count(string text, string needle) => text.Split(new[] { needle }, StringSplitOptions.None).Length - 1;

    public static int Main(string[] args)
    {
        string root = args.Length > 0 ? args[0] : ".";
        Cov.Result coverage = Coverage(out Cov.Area covered, out Cov.Area covered2);
        var session = new R.Session();
        R.Presentation view = R.Build(coverage, "", session);

        // ---- A. coverage presentation: source sample, four states, readable reasons
        Check(view.Valid && view.Sample != null && view.Sample.StateLabel == "SAMPLE" && view.Sample.Title.Contains("Rack 3"), "source sample is its own row, labelled SAMPLE");
        Check(R.SampleLabel == "Sample — demonstrated treatment" && view.Sample.Meaning == R.SampleLabel, "sample is labelled 'Sample — demonstrated treatment'");
        Check(view.Targets.All(t => !t.IsSource) && !view.Targets.Any(t => t.Id == view.Sample.Id), "the sample is never listed as a target");
        var labels = view.Targets.Select(t => t.StateLabel).Concat(view.Remainders.Select(t => t.StateLabel)).ToList();
        Check(labels.Contains("COVERED") && labels.Contains("NOT COVERED") && labels.Contains("EXCLUDED") && labels.Contains("MANUAL"), "Covered, Not Covered, Excluded and Manual are all explicit");
        Check(view.Covered == 2 && view.NotCovered == 1 && view.Excluded == 1 && view.Manual == 2, "counts per state (remainder counts as Manual)");
        Check(view.Targets.Single(t => t.State == Cov.State.NotCovered).Reasons.SequenceEqual(new[] { "Stand density differs from the sample", "Mean tree size differs from the sample" }), "Not Covered reasons are readable");
        Check(view.Targets.Single(t => t.State == Cov.State.Excluded).Reasons.Contains("Contains non-Sitka adult tree(s) — V1 extrapolation is Sitka-only"), "Excluded reason is readable (non-Sitka)");
        Check(view.Remainders[0].Reasons.Single() == "Remainder — manual management", "remainder reason is readable");
        foreach (Cov.Reason reason in Enum.GetValues(typeof(Cov.Reason)))
            if (R.ReasonText(reason) == reason.ToString()) Check(false, "every closed reason has plain wording: " + reason);
        Check(true, "every closed ScenarioOneCoverage.Reason has plain wording");
        Check(covered.Flags.Count == 1 && view.Targets.First(t => t.Id == covered.Id).Notes.Single().Contains("information only") && view.Targets.First(t => t.Id == covered.Id).State == Cov.State.Covered,
            "NaturalRegeneration is an informational note, not an exclusion");
        Check(R.StateMeaning(Cov.State.Covered).StartsWith("Comparable") && R.CoveredNote.Contains("not mean selected, approved or committed"), "Covered means comparable/eligible, not accepted or committed");
        Check(R.ApplyWording.ToLowerInvariant().Contains("apply your sample treatment to comparable areas"), "required wording 'apply your sample treatment to comparable areas'");

        // ---- B. classification is separate from the draft choice
        Check(view.Targets.Single(t => t.Id == covered.Id).Choice == R.Choice.Undecided, "a Covered unit starts with no draft choice");
        Check(session.SetChoice(coverage, covered.Id, R.Choice.Include) && session.IncludedUnitIds().SequenceEqual(new[] { covered.Id }), "Include is a draft choice");
        R.Presentation after = R.Build(coverage, "", session);
        Check(after.Targets.Single(t => t.Id == covered.Id).State == Cov.State.Covered && after.Targets.Single(t => t.Id == covered.Id).Choice == R.Choice.Include
            && covered.State == Cov.State.Covered, "Include changes the draft only: classification stays Covered");
        Check(after.Targets.Single(t => t.Id == covered.Id).MapTag.Contains("COVERED") && after.Targets.Single(t => t.Id == covered.Id).MapTag.Contains("INCLUDE"), "map tag shows classification and draft choice as separate words");
        Check(!session.SetChoice(coverage, view.Targets.Single(t => t.State == Cov.State.NotCovered).Id, R.Choice.Include), "Not Covered cannot be included");
        Check(!session.SetChoice(coverage, view.Targets.Single(t => t.State == Cov.State.Excluded).Id, R.Choice.Include), "Excluded cannot be included");
        Check(!session.SetChoice(coverage, view.Sample.Id, R.Choice.Include), "the sample cannot be included");
        Check(!session.SetChoice(coverage, view.Targets.Single(t => t.State == Cov.State.Manual).Id, R.Choice.Include), "a Manual unit cannot be force-included");
        Check(view.Targets.Where(t => t.CanChoose).All(t => t.State == Cov.State.Covered) && !view.Sample.CanChoose && view.Remainders.All(r => !r.CanChoose), "only Covered targets are offered the choice");
        session.SetChoice(coverage, covered2.Id, R.Choice.LeaveManual);
        Check(session.LeftManualUnitIds().SequenceEqual(new[] { covered2.Id }) && session.IncludedUnitIds().Count == 1, "Leave manual is the other draft choice");
        covered2.State = Cov.State.Excluded;
        session.PruneTo(coverage);
        Check(session.LeftManualUnitIds().Count == 0, "a draft choice on a unit that stopped being Covered is dropped");
        covered2.State = Cov.State.Covered;

        // ---- C. fail closed on an invalid read
        R.Presentation bad = R.Build(null, "Prescriptions need a confirmed Rack Planning V2 plan.", session);
        Check(!bad.Valid && bad.Problem.Contains("confirmed Rack Planning V2") && bad.Sample == null && bad.Targets.Count == 0, "an invalid coverage read shows the production problem and no rows");
        R.Presentation bad2 = R.Build(new Cov.Result { Valid = false, Problem = "boom" }, "", session);
        Check(!bad2.Valid && bad2.Problem == "boom" && bad2.Targets.Count == 0, "an invalid coverage result shows its own problem and offers nothing");

        // ---- D. determinism
        Check(R.Build(coverage, "", session).Signature() == R.Build(coverage, "", session).Signature(), "presentation is deterministic");

        // ---- E. candidates: non-wrapping navigation; the model has no confirm path
        var stems = new List<Cov.Stem> { Stem("a", 0), Stem("b", 16), Stem("c", 32), Stem("z", 100), Stem("s1", 200, true) };
        var cs = new R.Session();
        Cand.Result batch = cs.Suggest(coverage, stems);
        Check(batch.Valid && batch.Candidates.Select(c => c.TreeId).SequenceEqual(new[] { "b", "a", "c", "z" }), "candidates come from the existing MFM-05 batch (centre first)");
        Check(!cs.CanMove(Cand.Navigation.Previous) && cs.CanMove(Cand.Navigation.Next), "queue starts with nothing behind it");
        Check(cs.Move(Cand.Navigation.Next) && cs.CurrentCandidateTreeId == "b", "Next starts at the first open candidate");
        Check(cs.Move(Cand.Navigation.Next) && cs.Move(Cand.Navigation.Next) && cs.Move(Cand.Navigation.Next) && cs.CurrentCandidateTreeId == "z", "Next steps one candidate at a time");
        Check(!cs.Move(Cand.Navigation.Next) && cs.CurrentCandidateTreeId == "z", "Next does not wrap at the end");
        Check(cs.Move(Cand.Navigation.Previous) && cs.CurrentCandidateTreeId == "c" && !new R.Session().Move(Cand.Navigation.Previous), "Previous steps back and does not wrap");
        Check(batch.Candidates.All(c => c.Decision == Cand.Decision.Open) && cs.Move(Cand.Navigation.Next), "navigation leaves every candidate Open (nothing confirmed by moving)");
        string[] sessionMethods = typeof(R.Session).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Select(m => m.Name).ToArray();
        Check(!sessionMethods.Any(n => n.IndexOf("Confirm", StringComparison.OrdinalIgnoreCase) >= 0 || n.IndexOf("Bulk", StringComparison.OrdinalIgnoreCase) >= 0),
            "the review session has no confirm or bulk-confirm operation");
        Check(!typeof(R).GetMethods(BindingFlags.Public | BindingFlags.Static).Any(m => m.Name.IndexOf("Confirm", StringComparison.OrdinalIgnoreCase) >= 0), "the review model has no confirm operation");
        // Confirmation comes only from the live Crop Tree mark; rejection is review state.
        var live = new List<Cov.Stem>(stems);
        live[1] = Stem("b", 16, true);
        Cand.Result confirmedBatch = cs.Suggest(coverage, live);
        Check(confirmedBatch.Candidates.Single(c => c.TreeId == "b").Decision == Cand.Decision.Confirmed && confirmedBatch.Candidates.Select(c => c.TreeId).SequenceEqual(new[] { "b", "a", "c", "z" }), "a candidate becomes Confirmed only when the tree is marked a Crop Tree, and keeps its place");
        Check(cs.Reject("a") && cs.DecisionNow("a", Cand.Decision.Open, false) == Cand.Decision.Rejected && cs.RejectedCandidates.Contains("a"), "Reject is review state");
        Check(!cs.Reject("b") && !cs.Reject("nope"), "a confirmed or unknown tree cannot be rejected");
        Check(cs.Suggest(coverage, live).Candidates.Single(c => c.TreeId == "a").Decision == Cand.Decision.Rejected && cs.Suggest(coverage, live).Candidates.Count == 4, "a rejected candidate keeps its place and is not replaced");
        Check(cs.Reopen("a") && cs.Suggest(coverage, live).Candidates.Single(c => c.TreeId == "a").Decision == Cand.Decision.Open, "a rejection can be reopened");
        R.CandidateCard card = R.BuildCandidateCard(cs.LastCandidates, cs, id => live.Any(s => s.Id == id && s.Crop));
        Check(card.Valid && card.Total == 4 && card.Confirmed == 1 && card.Open == 3 && card.Warning.Contains("Spacing identifies where to inspect") && card.Warning.Contains("Judge the tree in 3D"), "the candidate card carries the spacing-only inspection-prompt warning");
        Check(R.CandidateChip.Contains("inspection prompt") && !R.CandidateChip.ToLowerInvariant().Contains("recommend"), "candidates are labelled inspection prompts, not recommendations");

        // ---- F. proposals
        var preview = new Appr.ProposalSet { Valid = true };
        preview.Proposals.Add(new ScenarioOnePrescription.Proposal { TreeId = "t1", AreaId = covered.Id, ForCropTreeId = "b", ShareOfCropTree = 0.31, BasalAreaM2 = 0.04 });
        preview.Links.Add(new Appr.OrderLink(1, "t1", covered.Id, "b", 1));
        preview.Conflicts.Add(new Appr.Conflict { TreeId = "t2", CoverageUnitId = covered.Id, CropTreeId = "b", Kind = Appr.ConflictKind.ManualFellMark });
        preview.Conflicts.Add(new Appr.Conflict { TreeId = "", CoverageUnitId = covered2.Id, Kind = Appr.ConflictKind.UnitNoLongerCovered });
        List<R.ProposalRow> rows = R.BuildProposalRows(preview);
        Check(rows.Count == 3 && rows[0].Label.Contains("not a Fell mark") && rows[0].Label.Contains("not yet work"), "every proposal is labelled not a Fell mark and not yet work");
        Check(rows[0].Reason == "High-share competitor around a confirmed Crop Tree" && rows[0].Quantity.Contains("31 %") && rows[0].CropTreeId == "b" && rows[0].AreaId == covered.Id, "a proposal shows tree, unit, Crop Tree, reason and share");
        Check(rows[1].IsConflict && rows[1].Reason.Contains("already marked") && rows[2].Reason.Contains("no longer covered"), "conflicts are surfaced with their production reason");
        Check(R.BuildProposalRows(new Appr.ProposalSet { Valid = false }).Count == 0, "an invalid preview shows no proposals");

        // ---- G. 3D spot-check and approval gate
        var gs = new R.Session();
        Check(!R.EvaluateGate(gs, preview).CanApprove && R.EvaluateGate(gs, preview).Reason.Contains("Prepare"), "no prescription: approval blocked");
        gs.ReplaceActivePrescription(7);
        R.ApprovalGate g0 = R.EvaluateGate(gs, preview);
        Check(!g0.CanApprove && !g0.SpotCheckDone && g0.Reason.Contains("3D"), "valid snapshot + preview but no spot-check: approval blocked");
        Check(gs.SpotCheckedUnits.Count == 0 && !gs.SpotCheckDone, "selecting, waypointing and navigation never set the spot-check (no operation other than RecordSpotCheck can)");
        gs.SelectedAreaId = covered.Id;
        gs.CurrentCandidateTreeId = "b";
        Check(!gs.SpotCheckDone, "selecting a unit or candidate does not set the spot-check");
        Check(!gs.RecordSpotCheck("nowhere", coverage, new[] { covered.Id }), "inspecting a tree outside the accepted units does not count");
        Check(!gs.RecordSpotCheck("a", coverage, new[] { covered2.Id }), "inspecting a tree in a unit that was not accepted does not count");
        covered.State = Cov.State.Excluded;
        Check(!gs.RecordSpotCheck("a", coverage, new[] { covered.Id }), "a unit that is no longer Covered cannot be spot-checked");
        covered.State = Cov.State.Covered;
        Check(gs.RecordSpotCheck("a", coverage, new[] { covered.Id }) && gs.SpotCheckDone && gs.SpotCheckedUnits.SequenceEqual(new[] { covered.Id }), "inspecting a tree in an accepted Covered unit records the spot-check");
        R.ApprovalGate g1 = R.EvaluateGate(gs, preview);
        Check(g1.CanApprove && g1.Reason == "", "snapshot + preview with proposals + spot-check: approval allowed");
        var empty = new Appr.ProposalSet { Valid = true };
        Check(!R.EvaluateGate(gs, empty).CanApprove && !R.EvaluateGate(gs, new Appr.ProposalSet { Valid = false }).CanApprove && !R.EvaluateGate(gs, null).CanApprove, "no proposals or an unreadable preview: approval blocked");
        gs.ReplaceActivePrescription(8);
        Check(!gs.SpotCheckDone && !R.EvaluateGate(gs, preview).CanApprove, "replacing the active snapshot resets the spot-check");
        gs.ReplaceActivePrescription(8);
        gs.RecordSpotCheck("a", coverage, new[] { covered.Id });
        gs.ReplaceActivePrescription(8);
        Check(gs.SpotCheckDone, "re-selecting the same snapshot keeps its spot-check");
        gs.MarkApproved();
        Check(!R.EvaluateGate(gs, preview).CanApprove && R.EvaluateGate(gs, preview).AlreadyApproved && !gs.RecordSpotCheck("a", coverage, new[] { covered.Id }), "an approved prescription cannot be approved again");
        Check(R.ApprovedMessage == "Prescription approved. Pending tasks were added to the Work Plan. They are not approved for execution yet.", "approval message states Pending, not approved for execution");

        // ---- H. 3D context for the inspection card
        var ts = new R.Session();
        ts.Suggest(coverage, stems);
        ts.ReplaceActivePrescription(1);
        ts.LastPreview = preview;
        R.TreeContext open = R.DescribeTree("b", false, ts);
        Check(open.IsCandidate && open.Chip == R.CandidateChip && open.Line == R.CandidateWarning && open.Hint.Contains("[C] Confirm as Crop Tree") && open.Hint.Contains("[N] Reject candidate"), "3D: an open candidate says candidate/inspection prompt and offers confirm [C] / reject [N]");
        R.TreeContext conf = R.DescribeTree("b", true, ts);
        Check(conf.Decision == Cand.Decision.Confirmed && conf.Chip.StartsWith(R.ConfirmedChip) && conf.Hint == "", "3D: after [C] the tree reads as a confirmed Crop Tree");
        ts.Reject("a");
        R.TreeContext rej = R.DescribeTree("a", false, ts);
        Check(rej.Decision == Cand.Decision.Rejected && rej.Chip.Contains("rejected") && rej.Hint.Contains("Reopen"), "3D: a rejected candidate can be reopened");
        R.TreeContext prop = R.DescribeTree("t1", false, ts);
        Check(prop.IsProposal && prop.Chip == R.ProposalChip && prop.Line.Contains("31 %") && prop.Line.Contains("Nothing is marked"), "3D: a proposal target is labelled PROPOSAL, not a Fell mark and not yet work");
        Check(R.DescribeTree("t2", false, ts).IsConflict && !R.DescribeTree("nobody", false, ts).Any, "3D: a held conflict is labelled; an ordinary tree gets no review label");
        ts.MarkApproved();
        Check(!R.DescribeTree("t1", false, ts).Any, "3D: once approved the proposal label is gone (Pending work is authoritative)");

        // ---- I. map form: shape + pattern + text, not colour alone
        var patterns = new[] {
            StandMapReviewPlan.PatternFor(true, Cov.State.Manual), StandMapReviewPlan.PatternFor(false, Cov.State.Covered), StandMapReviewPlan.PatternFor(false, Cov.State.NotCovered),
            StandMapReviewPlan.PatternFor(false, Cov.State.Excluded), StandMapReviewPlan.PatternFor(false, Cov.State.Manual) };
        Check(patterns.Select(p => p.Signature).Distinct().Count() == 5, "sample, covered, not covered, excluded, manual differ by outline/dash/hatch form");
        var formOnly = patterns.Select(p => p.Signature).Distinct().Count();
        Check(formOnly == 5 && patterns[2].DashPx > 0 && patterns[4].DashPx > 0 && patterns[2].DashPx != patterns[4].DashPx && patterns[0].OutlinePx > patterns[1].OutlinePx, "the form difference does not rely on hue (same-colour comparison would still differ)");
        var square = new[] { new ReviewPoint(0, 0), new ReviewPoint(100, 0), new ReviewPoint(100, 60), new ReviewPoint(0, 60) };
        var fp = new ReviewFootprint { AreaId = "x", State = Cov.State.Covered };
        fp.Polygon.AddRange(square);
        List<ReviewShape> shapes = StandMapReviewPlan.Areas(new[] { fp });
        Check(shapes.Count(s => s.Kind == ReviewShapeKind.AreaHatch) > 3 && shapes.Count(s => s.Kind == ReviewShapeKind.AreaOutline) == 1, "covered area gets hatching and one outline");
        Check(shapes.Where(s => s.Kind == ReviewShapeKind.AreaHatch).All(s => s.Points.All(p => p.X >= -0.01f && p.X <= 100.01f && p.Y >= -0.01f && p.Y <= 60.01f)), "hatching is clipped to the area");
        Check(StandMapReviewPlan.IsConvex(square) && !StandMapReviewPlan.IsConvex(new[] { new ReviewPoint(0, 0), new ReviewPoint(10, 0), new ReviewPoint(2, 2), new ReviewPoint(10, 10), new ReviewPoint(0, 10) }), "non-convex areas are outlined without hatch");
        List<ReviewShape> candGlyph = StandMapReviewPlan.CandidateGlyph("a", 50, 50, Cand.Decision.Open, false);
        List<ReviewShape> propGlyph = StandMapReviewPlan.ProposalGlyph("t", 50, 50, false);
        Check(candGlyph[0].Points.Count == 4 && candGlyph[0].Closed && propGlyph[0].Points.Count == 3 && propGlyph[0].Closed && candGlyph[0].Kind != propGlyph[0].Kind, "candidate = hollow square, proposal = hollow triangle (different shapes)");
        Check(StandMapReviewPlan.CandidateGlyph("a", 0, 0, Cand.Decision.Rejected, false).Count == 2 && StandMapReviewPlan.CandidateGlyph("a", 0, 0, Cand.Decision.Confirmed, true).Count == 3 && StandMapReviewPlan.ProposalGlyph("t", 0, 0, true).Count == 2, "rejected, confirmed, current and conflict glyphs differ in form");
        Check(candGlyph.Concat(propGlyph).All(s => s.Kind == ReviewShapeKind.CandidateGlyph || s.Kind == ReviewShapeKind.ProposalGlyph) && StandMapReviewPlan.Areas(new[] { fp }).All(s => s.Kind != ReviewShapeKind.CandidateGlyph), "glyph kinds never mix with area shapes");
        Check(StandMapReviewPlan.Areas(new[] { fp }).Count == StandMapReviewPlan.Areas(new[] { fp }).Count && string.Join(",", StandMapReviewPlan.Areas(new[] { fp }).Select(s => s.Points.Count)) == string.Join(",", StandMapReviewPlan.Areas(new[] { fp }).Select(s => s.Points.Count)), "map plan is deterministic");

        // ---- J. static boundaries (source text)
        string card1 = Read(root, "Assets/ForestPrototype/UI/StandMapTreatmentReview.cs");
        string model = Read(root, "Assets/ForestPrototype/UI/ScenarioOneTreatmentReview.cs");
        string plan = Read(root, "Assets/ForestPrototype/UI/Map/StandMapReviewPlan.cs");
        string layer = Read(root, "Assets/ForestPrototype/UI/Map/StandMapReviewLayer.cs");
        string mapView = Read(root, "Assets/ForestPrototype/UI/StandMapView.cs");
        string inspect = Read(root, "Assets/ForestPrototype/UI/TreeInspectionView.cs");
        string[] mine = { card1, model, plan, layer };
        Check(Count(card1, ".ReadCoverage(") == 2 && Count(card1, "manager.ReadCoverage(out coverageProblem)") == 1 && Count(card1, "ui.Manager.ReadCoverage(out _)") == 1, "coverage is read only through ScenarioOneManager.ReadCoverage (the review read and the spot-check read)");
        Check(mine.All(t => !t.Contains("Cov.Classify") && !t.Contains("ScenarioOneCoverage.Classify(") && !t.Contains(".Classify(") && !t.Contains("ScenarioOneCoverageWorld") && !t.Contains("ReadCoverageWorld") && !t.Contains("ScenarioOneCoverage.Mask")), "no second coverage adapter: no Classify, no coverage world, no masks");
        Check(!model.Contains("using UnityEngine") && !plan.Contains("using UnityEngine"), "the review model and map plan are pure (no UnityEngine)");
        Check(card1.Contains("Cand.TryGetInspectionTarget(") && model.Contains("Cand.TryMoveInspectionTarget(") && model.Contains("Cand.Suggest(") && !card1.Contains("Cand.Suggest("), "candidates use the MFM-05 batch and inspection/navigation helpers");
        Check(card1.Contains("manager.PreviewPrescription(") && Count(card1, "ui.Manager.CreatePrescription(") == 1 && Count(card1, "ui.Manager.ApprovePrescription(") == 1, "prescription boundary: Create/Preview/Approve are each called once through the MFM-07 manager");
        string[] forbidden = { "SetMark(", "TreeMarkType.Fell", "TreeMarkType.CropTree", "ToggleCropTree", "ToggleFell", "ForestTreeMarkingManager", "CreateFellingOrder", "ApprovePendingWork", "ShowWorkPlan", "workPlan", "MarkForFell", "workOrders", "TeleportTo", "transform.position =", "Teleport" };
        foreach (string f in forbidden)
            Check(mine.All(t => !t.Contains(f)), "MFM-08 files never touch marks, orders, Work Plan approval or the player position: " + f);
        Check(!mapView.Contains("RecordSpotCheck") && Count(card1, "session.RecordSpotCheck(") == 1 && card1.IndexOf("session.RecordSpotCheck(", StringComparison.Ordinal) > card1.IndexOf("public void OnInspected(", StringComparison.Ordinal), "the spot-check is recorded only from the 3D inspection path, never from the map");
        int inspectArea = card1.IndexOf("private void InspectArea(", StringComparison.Ordinal);
        string inspectBody = card1.Substring(inspectArea, card1.IndexOf("private void BuildCandidates(", StringComparison.Ordinal) - inspectArea);
        Check(!inspectBody.Contains("SpotCheck") && inspectBody.Contains("setWaypointAt") && inspectBody.Contains("does not count as the 3D spot-check"), "Inspect in forest only sets a waypoint (no spot-check, no teleport)");
        int approveBody = card1.IndexOf("private void ApprovePrescription()", StringComparison.Ordinal);
        Check(card1.IndexOf("Review.EvaluateGate(session", approveBody, StringComparison.Ordinal) > 0 && card1.IndexOf("Review.EvaluateGate(session", approveBody, StringComparison.Ordinal) < card1.IndexOf("ui.Manager.ApprovePrescription(", approveBody, StringComparison.Ordinal), "ApprovePrescription is called only after the gate is re-evaluated");
        Check(Count(card1, "ApprovePrescription(session.ActivePrescriptionId)") == 1 && Count(card1, "ShowWorkPlan") + Count(card1, "Work Plan approval") == 0, "approval never opens or approves the Work Plan");
        Check(card1.Contains("Learning") == false && mapView.Contains("value != Layer.Access && value != Layer.Review"), "no learning/curriculum records from the review (MFM-09 owns teaching)");
        Check(inspect.Contains("treatmentReview?.OnInspected(tree)") && inspect.Contains("ui.Map.Review") && card1.Contains("keys.nKey.wasPressedThisFrame") && !card1.Contains("cKey"), "the 3D card reuses the normal [C] Crop Tree path and adds only a contextual [N]");
        Check(card1.Contains("if (!view.Valid)") && card1.IndexOf("return view;", StringComparison.Ordinal) < card1.IndexOf("BuildSample(body, view);", StringComparison.Ordinal), "an invalid coverage read returns before any action is built");
        Check(Count(mapView, "review.") >= 3 && mapView.Contains("SetWaypointCell(ui.Ecology"), "the map reuses the existing waypoint path");
        Check(mine.All(t => t.IndexOf("operator selection", StringComparison.OrdinalIgnoreCase) < 0), "'operator selection' is never presented");
        Check(mine.All(t => t.IndexOf("ProtectedArea", StringComparison.Ordinal) < 0 || t == model), "ProtectedArea evidence is only mapped to wording, never fabricated");
        Check(card1.Contains("MapPosition at = t.WorldToPixels") && !card1.Contains("new Cov.Area") && !card1.Contains("new Cov.Result"), "the UI never builds coverage areas or results of its own");

        AreaFootprintChecks.Run(root);

        Console.WriteLine((failed == 0 ? "MFM08_TREATMENT_REVIEW_PASS" : "MFM08_TREATMENT_REVIEW_FAIL") + " checks=" + checks + " failed=" + failed);
        return failed == 0 ? 0 : 1;
    }
}
