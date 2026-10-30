using System;
using System.Collections.Generic;
using System.Globalization;
using Cov = ScenarioOneCoverage;
using Cand = ScenarioOneCropTreeCandidates;
using Appr = ScenarioOnePrescriptionApproval;

// MFM-08 treatment review: the plain-data side of the Stand Map "Treatment review" layer and its 3D companion. No UnityEngine.
//
// It turns what production already decided into player wording and review state. It decides nothing itself:
//   * coverage classification comes only from the ScenarioOneCoverage.Result the caller got from ScenarioOneManager.ReadCoverage;
//   * candidates come only from ScenarioOneCropTreeCandidates (spacing-only inspection prompts);
//   * proposals come only from ScenarioOneManager.PreviewPrescription (recomputed, never stored, never a Fell mark);
//   * prescription approval stays ScenarioOneManager.ApprovePrescription, and the Work Plan approval stays a separate gate.
// The Session below is presentation state only. It is never saved and it is replaced with the view on a reload.
public static class ScenarioOneTreatmentReview
{
    // ---------------------------------------------------------------------------------------------------------------- wording

    public const string ApplyWording = "Apply your sample treatment to comparable areas.";
    public const string NothingCommittedNote = "Nothing here is marked, planned or approved yet. Approving a prescription only adds Pending tasks; the Work Plan still needs your approval.";
    public const string SampleLabel = "Sample — demonstrated treatment";
    public const string CoveredNote = "Covered means comparable, so it is available to include. It does not mean selected, approved or committed.";
    public const string CandidateWarning = "Spacing identifies where to inspect. Judge the tree in 3D before confirming it as a Crop Tree.";
    public const string CandidateChip = "CANDIDATE — inspection prompt";
    public const string ProposalChip = "PROPOSAL — not a Fell mark and not yet work";
    public const string ConflictChip = "CONFLICT — held, not proposed";
    public const string ConfirmedChip = "CONFIRMED CROP TREE";
    public const string ApprovedMessage = "Prescription approved. Pending tasks were added to the Work Plan. They are not approved for execution yet.";
    public const string ConfirmKeyHint = "[C] Confirm as Crop Tree";
    public const string RejectKeyHint = "[N] Reject candidate";
    public const string ReopenKeyHint = "[N] Reopen candidate";

    public static string StateLabel(Cov.State state)
    {
        switch (state)
        {
            case Cov.State.Covered: return "COVERED";
            case Cov.State.NotCovered: return "NOT COVERED";
            case Cov.State.Excluded: return "EXCLUDED";
            default: return "MANUAL";
        }
    }

    public static string StateMeaning(Cov.State state)
    {
        switch (state)
        {
            case Cov.State.Covered: return "Comparable — available to include";
            case Cov.State.NotCovered: return "Not comparable — cannot be included";
            case Cov.State.Excluded: return "Excluded from extrapolation";
            default: return "Manual management";
        }
    }

    // The closed ScenarioOneCoverage vocabulary in plain language (BROWSER-RESULT section 4.3).
    public static string ReasonText(Cov.Reason reason)
    {
        switch (reason)
        {
            case Cov.Reason.SourceSample: return "Demonstrated sample treatment";
            case Cov.Reason.PlayerChoseManual: return "You chose to manage this area manually";
            case Cov.Reason.RemainderLeftForManualManagement: return "Remainder — manual management";
            case Cov.Reason.PlantedJuvenile: return "Existing planted juvenile(s)";
            case Cov.Reason.Shelter: return "Existing tree protection/shelter";
            case Cov.Reason.ClearancePatch: return "Existing vegetation-treatment/clearance patch";
            case Cov.Reason.ProtectedArea: return "Protected area";
            case Cov.Reason.DeadwoodRecord: return "Retained/fallen deadwood";
            case Cov.Reason.ActiveManagementWork: return "Existing management work";
            case Cov.Reason.PlayerFellMark: return "Existing player Fell mark";
            case Cov.Reason.PlayerOpenWork: return "Existing management work (open felling order)";
            case Cov.Reason.PlayerCompletedWork: return "Existing management work (felling already done)";
            case Cov.Reason.NonSitkaAdult: return "Contains non-Sitka adult tree(s) — V1 extrapolation is Sitka-only";
            case Cov.Reason.TooFewStems: return "Too few comparable stems";
            case Cov.Reason.DensityDiffers: return "Stand density differs from the sample";
            case Cov.Reason.MeanDbhDiffers: return "Mean tree size differs from the sample";
            case Cov.Reason.MeanCompetitionDiffers: return "Competition differs from the sample";
            default: return reason.ToString();
        }
    }

    // Flags are evidence, never exclusions (BROWSER-RESULT section 4.3).
    public static string FlagText(Cov.Flag flag)
        => flag == Cov.Flag.NaturalRegeneration ? "Note: natural regeneration is present (information only)"
         : "Note: contains player Crop Trees (information only)";

    public static string ReasonTextFor(Cov.Remainder remainder) => ReasonText(remainder.Reason);

    private static string Num(double value, string format) => value.ToString(format, CultureInfo.InvariantCulture);

    // ------------------------------------------------------------------------------------------------------- draft choices

    // The player's draft intent for a Covered unit. Distinct from classification: it never changes what coverage says.
    public enum Choice { Undecided, Include, LeaveManual }

    public static string ChoiceText(Choice choice)
        => choice == Choice.Include ? "Draft: apply sample treatment here"
         : choice == Choice.LeaveManual ? "Draft: leave manual" : "Draft: not chosen yet";

    // ----------------------------------------------------------------------------------------------------------------- rows

    public sealed class AreaRow
    {
        public string Id = "", Title = "";
        public bool IsSource, IsRemainder;
        public Cov.State State;
        public string StateLabel = "", Meaning = "";
        public readonly List<string> Reasons = new List<string>();
        public readonly List<string> Notes = new List<string>();
        public string Metrics = "", Comparison = "";
        public readonly List<string> Evidence = new List<string>();
        public int StemCount;
        public Choice Choice;
        // Only a Covered, non-sample unit may be included. NotCovered/Excluded/Manual are inspectable but never offered the action.
        public bool CanChoose;
        public int RackId;
        public Cov.Segment Segment;
        // The tag printed on the map beside the outline: it says the state in words so the colour is not the only carrier.
        public string MapTag = "";
    }

    public sealed class Presentation
    {
        public bool Valid;
        public string Problem = "";
        public AreaRow Sample;
        public readonly List<AreaRow> Targets = new List<AreaRow>();
        public readonly List<AreaRow> Remainders = new List<AreaRow>();
        public int Covered, NotCovered, Excluded, Manual;
        public string Summary = "";
        public string Signature()
        {
            if (!Valid) return "INVALID " + Problem;
            var sb = new System.Text.StringBuilder();
            foreach (AreaRow row in All()) sb.Append(row.Id).Append('=').Append(row.StateLabel).Append('/').Append(row.Choice).Append('[').Append(string.Join(";", row.Reasons)).Append("] ");
            return sb.ToString();
        }
        public IEnumerable<AreaRow> All()
        {
            if (Sample != null) yield return Sample;
            foreach (AreaRow r in Targets) yield return r;
            foreach (AreaRow r in Remainders) yield return r;
        }
    }

    // A fail-closed presentation: an invalid or missing coverage read shows the production problem and offers no action.
    public static Presentation Build(Cov.Result coverage, string readProblem, Session session)
    {
        var view = new Presentation();
        if (coverage == null || !coverage.Valid)
        {
            view.Problem = !string.IsNullOrEmpty(readProblem) ? readProblem
                : coverage != null && !string.IsNullOrEmpty(coverage.Problem) ? coverage.Problem : "Coverage could not be read.";
            return view;
        }
        view.Valid = true;
        foreach (Cov.Area area in coverage.Areas)
        {
            AreaRow row = RowOf(area, coverage, session);
            if (area.IsSource) view.Sample = row;
            else view.Targets.Add(row);
            if (area.IsSource) continue;
            switch (area.State)
            {
                case Cov.State.Covered: view.Covered++; break;
                case Cov.State.NotCovered: view.NotCovered++; break;
                case Cov.State.Excluded: view.Excluded++; break;
                default: view.Manual++; break;
            }
        }
        foreach (Cov.Remainder remainder in coverage.Remainders)
        {
            var row = new AreaRow
            {
                Id = remainder.Id, IsRemainder = true, State = remainder.State, StateLabel = StateLabel(remainder.State),
                Meaning = StateMeaning(remainder.State), Title = "Outer remainder (" + (remainder.Side == ScenarioOneRackSampleGeometry.Side.Right ? "right" : "left") + ")",
                StemCount = remainder.Stems,
                Metrics = Num(remainder.AreaM2, "0") + " m²" + (remainder.IsEmpty ? " (empty)" : ", " + remainder.Stems + " stems")
            };
            row.Reasons.Add(ReasonText(remainder.PlayerChoseManual ? Cov.Reason.PlayerChoseManual : remainder.Reason));
            row.MapTag = "REMAINDER · " + row.StateLabel;
            view.Remainders.Add(row);
            view.Manual++;
        }
        int comparable = view.Covered + view.NotCovered + view.Excluded + view.Manual;
        view.Summary = view.Covered + " of " + (coverage.NonSourceCount) + " rack-served areas are comparable with the sample"
            + (comparable > 0 ? " · " + view.NotCovered + " not covered · " + view.Excluded + " excluded" : "") + ".";
        return view;
    }

    private static AreaRow RowOf(Cov.Area area, Cov.Result coverage, Session session)
    {
        var row = new AreaRow
        {
            Id = area.Id, RackId = area.RackId, Segment = area.Segment, IsSource = area.IsSource, State = area.State,
            StateLabel = area.IsSource ? "SAMPLE" : StateLabel(area.State),
            Meaning = area.IsSource ? SampleLabel : StateMeaning(area.State),
            Title = "Rack " + area.RackId + " · " + area.Segment + " strip",
            StemCount = area.Metrics != null ? area.Metrics.Stems : area.StemIds.Count
        };
        if (area.Metrics != null)
            row.Metrics = row.StemCount + " stems · " + Num(area.Metrics.DensityPerHa, "0") + "/ha · mean DBH " + Num(area.Metrics.MeanDbhCm, "0.0") + " cm";
        foreach (Cov.Reason reason in area.Reasons) row.Reasons.Add(ReasonText(reason));
        foreach (Cov.Flag flag in area.Flags) row.Notes.Add(FlagText(flag));
        foreach (string id in area.Evidence) row.Evidence.Add(id);
        if (area.Comparison != null)
            row.Comparison = "Against the sample: density " + Signed(area.Comparison.DeltaDensity, "0") + "/ha, mean DBH " + Signed(area.Comparison.DeltaMeanDbh, "0.0")
                + " cm, competition " + Signed(area.Comparison.DeltaMeanCompetition, "0.00");
        row.CanChoose = !area.IsSource && area.State == Cov.State.Covered;
        row.Choice = row.CanChoose && session != null ? session.ChoiceFor(area.Id) : Choice.Undecided;
        row.MapTag = area.IsSource ? "SAMPLE" : row.StateLabel + (row.CanChoose && row.Choice == Choice.Include ? " · INCLUDE" : row.CanChoose && row.Choice == Choice.LeaveManual ? " · LEAVE MANUAL" : "");
        return row;
    }

    private static string Signed(double value, string format) => (value > 0 ? "+" : value < 0 ? "−" : "") + Num(Math.Abs(value), format);

    // ------------------------------------------------------------------------------------------------------------ session

    // Presentation/session state for one open review. Never persisted; a reload starts a new one. Holds draft choices, the player's
    // candidate rejections, the candidate being looked at, the prescription under review and the 3D spot-check latch.
    public sealed class Session
    {
        private readonly HashSet<string> included = new HashSet<string>(StringComparer.Ordinal);
        private readonly HashSet<string> manual = new HashSet<string>(StringComparer.Ordinal);
        private readonly HashSet<string> rejected = new HashSet<string>(StringComparer.Ordinal);
        private readonly HashSet<string> confirmed = new HashSet<string>(StringComparer.Ordinal);
        private readonly HashSet<string> offered = new HashSet<string>(StringComparer.Ordinal);
        private readonly SortedSet<string> spotChecked = new SortedSet<string>(StringComparer.Ordinal);

        public string CurrentCandidateTreeId = "";
        public string SelectedAreaId = "";
        public string Message = "";
        // 0 = no prescription under review.
        public int ActivePrescriptionId { get; private set; }
        public bool ActiveApproved { get; private set; }
        // The most recent plain-data reads, held only so the 3D inspection card can describe the tree you are looking at.
        public Cand.Result LastCandidates;
        public Appr.ProposalSet LastPreview;

        public IReadOnlyCollection<string> SpotCheckedUnits => spotChecked;
        public IReadOnlyCollection<string> RejectedCandidates => rejected;

        public Choice ChoiceFor(string areaId)
            => included.Contains(areaId) ? Choice.Include : manual.Contains(areaId) ? Choice.LeaveManual : Choice.Undecided;

        // Only a Covered target unit takes a draft choice; anything else is refused here as well as hidden in the view.
        public bool SetChoice(Cov.Result coverage, string areaId, Choice choice)
        {
            Cov.Area area = coverage != null && coverage.Valid ? coverage.Find(areaId) : null;
            if (area == null || area.IsSource || area.State != Cov.State.Covered) return false;
            included.Remove(areaId);
            manual.Remove(areaId);
            if (choice == Choice.Include) included.Add(areaId);
            else if (choice == Choice.LeaveManual) manual.Add(areaId);
            return true;
        }

        // A unit that stopped being Covered cannot keep a draft choice.
        public void PruneTo(Cov.Result coverage)
        {
            if (coverage == null || !coverage.Valid) return;
            included.RemoveWhere(id => { Cov.Area a = coverage.Find(id); return a == null || a.IsSource || a.State != Cov.State.Covered; });
            manual.RemoveWhere(id => { Cov.Area a = coverage.Find(id); return a == null || a.IsSource || a.State != Cov.State.Covered; });
        }

        public int IncludedCount => included.Count;
        public List<string> IncludedUnitIds() { var l = new List<string>(included); l.Sort(StringComparer.Ordinal); return l; }
        public List<string> LeftManualUnitIds() { var l = new List<string>(manual); l.Sort(StringComparer.Ordinal); return l; }

        // ----- candidates

        // Reads the live Crop Tree state into the confirmed/rejected sets the candidate batch needs. A tree the player was offered and
        // has since marked as a Crop Tree is Confirmed; one they unmarked goes back to Open. A confirmation cancels a rejection.
        public void ObserveStems(IReadOnlyList<Cov.Stem> stems)
        {
            if (stems == null) return;
            foreach (Cov.Stem stem in stems)
            {
                if (!offered.Contains(stem.Id)) continue;
                if (stem.Living && stem.Crop) { confirmed.Add(stem.Id); rejected.Remove(stem.Id); }
                else confirmed.Remove(stem.Id);
            }
        }

        public Cand.Result Suggest(Cov.Result coverage, IReadOnlyList<Cov.Stem> stems)
        {
            ObserveStems(stems);
            var sampleCrops = new List<string>();
            if (coverage != null && coverage.Valid)
            {
                var byId = new Dictionary<string, Cov.Stem>(StringComparer.Ordinal);
                if (stems != null) foreach (Cov.Stem s in stems) byId[s.Id] = s;
                foreach (string id in coverage.SourceStemIds)
                    if (byId.TryGetValue(id, out Cov.Stem s) && s.Living && s.Crop) sampleCrops.Add(id);
            }
            Cand.Result batch = Cand.Suggest(coverage, stems, sampleCrops, confirmed, rejected, coverage != null ? coverage.Calibration : null);
            if (batch.Valid) foreach (Cand.Candidate c in batch.Candidates) offered.Add(c.TreeId);
            LastCandidates = batch;
            return batch;
        }

        // The decision to SHOW for a tree now: a rejected candidate stays rejected; an Open one the player has since marked is Confirmed.
        public Cand.Decision DecisionNow(string treeId, Cand.Decision batchDecision, bool liveCrop)
            => rejected.Contains(treeId) ? Cand.Decision.Rejected : batchDecision == Cand.Decision.Open && liveCrop ? Cand.Decision.Confirmed : batchDecision;

        // Rejection is review state only: no second persistent mark, no tree change. Only an offered candidate can be rejected.
        public bool Reject(string treeId)
        {
            if (string.IsNullOrEmpty(treeId) || !offered.Contains(treeId) || confirmed.Contains(treeId)) return false;
            rejected.Add(treeId);
            return true;
        }

        public bool Reopen(string treeId) => rejected.Remove(treeId);

        // Non-wrapping Previous/Next over the existing MFM-05 queue. Nothing here confirms a candidate.
        public bool Move(Cand.Navigation direction)
        {
            // Next starts the queue; Previous only goes back from a candidate already being looked at.
            if (direction == Cand.Navigation.Previous && string.IsNullOrEmpty(CurrentCandidateTreeId)) return false;
            if (!Cand.TryMoveInspectionTarget(LastCandidates, CurrentCandidateTreeId, direction, out Cand.InspectionTarget target)) return false;
            CurrentCandidateTreeId = target.TreeId;
            return true;
        }

        public bool CanMove(Cand.Navigation direction)
            => !(direction == Cand.Navigation.Previous && string.IsNullOrEmpty(CurrentCandidateTreeId))
            && Cand.TryMoveInspectionTarget(LastCandidates, CurrentCandidateTreeId, direction, out _);

        // ----- prescription under review and the 3D spot-check

        // A new (or no) snapshot starts a new review: the spot-check does not carry over to a different prescription.
        public void ReplaceActivePrescription(int prescriptionId, bool approved = false)
        {
            if (prescriptionId != ActivePrescriptionId) spotChecked.Clear();
            ActivePrescriptionId = prescriptionId;
            ActiveApproved = approved;
            LastPreview = null;
        }

        public void MarkApproved() { ActiveApproved = true; }

        // The only way to satisfy the spot-check: a tree the player actually inspected in 3D that stands in an accepted, still Covered
        // unit of the active prescription. Setting a waypoint, selecting a unit or moving the map never calls this.
        public bool RecordSpotCheck(string inspectedTreeId, Cov.Result coverage, IReadOnlyCollection<string> acceptedUnitIds)
        {
            if (ActivePrescriptionId <= 0 || ActiveApproved || string.IsNullOrEmpty(inspectedTreeId) || coverage == null || !coverage.Valid || acceptedUnitIds == null) return false;
            foreach (string unitId in acceptedUnitIds)
            {
                Cov.Area area = coverage.Find(unitId);
                if (area == null || area.IsSource || area.State != Cov.State.Covered || !area.StemIds.Contains(inspectedTreeId)) continue;
                return spotChecked.Add(unitId);
            }
            return false;
        }

        public bool SpotCheckDone => spotChecked.Count > 0;
    }

    // ---------------------------------------------------------------------------------------------------------- candidates

    public sealed class CandidateCard
    {
        public bool Valid;
        public string Problem = "";
        public int Total, Open, Confirmed, Rejected;
        public Cand.Candidate Current;
        public Cand.Decision Decision;
        public string Position = "", StateText = "", Warning = CandidateWarning;
        public bool CanPrevious, CanNext;
    }

    public static CandidateCard BuildCandidateCard(Cand.Result batch, Session session, Func<string, bool> liveCrop)
    {
        var card = new CandidateCard();
        if (batch == null || !batch.Valid)
        {
            card.Problem = batch != null ? batch.Problem : "No candidate batch.";
            return card;
        }
        card.Valid = true;
        card.Total = batch.Candidates.Count;
        foreach (Cand.Candidate c in batch.Candidates)
        {
            Cand.Decision d = session.DecisionNow(c.TreeId, c.Decision, liveCrop != null && liveCrop(c.TreeId));
            if (d == Cand.Decision.Open) card.Open++;
            else if (d == Cand.Decision.Confirmed) card.Confirmed++;
            else card.Rejected++;
            if (c.TreeId == session.CurrentCandidateTreeId) { card.Current = c; card.Decision = d; }
        }
        card.CanPrevious = session.CanMove(Cand.Navigation.Previous);
        card.CanNext = session.CanMove(Cand.Navigation.Next);
        if (card.Current != null)
        {
            card.Position = "Candidate " + card.Current.Order + " of " + card.Total;
            card.StateText = card.Decision == Cand.Decision.Open ? "Open — not yet judged" : card.Decision == Cand.Decision.Confirmed ? "Confirmed as a Crop Tree in the forest" : "Rejected (review only)";
        }
        return card;
    }

    // -------------------------------------------------------------------------------------------------------------- proposals

    public sealed class ProposalRow
    {
        public string TreeId = "", AreaId = "", CropTreeId = "", Label = ProposalChip, Reason = "", Quantity = "";
        public bool IsConflict;
    }

    public static string ReasonOf(ScenarioOnePrescription.ProposalReason reason) => "High-share competitor around a confirmed Crop Tree";

    public static string ConflictText(Appr.ConflictKind kind)
        => kind == Appr.ConflictKind.ManualFellMark ? "Held: you already marked this tree to fell"
         : kind == Appr.ConflictKind.OpenWorkOrder ? "Held: this tree already has open work"
         : "Held: this unit is no longer covered, so it receives no proposal";

    public static List<ProposalRow> BuildProposalRows(Appr.ProposalSet preview)
    {
        var rows = new List<ProposalRow>();
        if (preview == null || !preview.Valid) return rows;
        foreach (ScenarioOnePrescription.Proposal p in preview.Proposals)
            rows.Add(new ProposalRow
            {
                TreeId = p.TreeId, AreaId = p.AreaId, CropTreeId = p.ForCropTreeId, Reason = ReasonOf(p.Reason),
                Quantity = Num(p.ShareOfCropTree * 100.0, "0") + " % of Crop Tree " + p.ForCropTreeId + "'s competition · basal area " + Num(p.BasalAreaM2, "0.000") + " m²"
            });
        foreach (Appr.Conflict c in preview.Conflicts)
            rows.Add(new ProposalRow { TreeId = c.TreeId, AreaId = c.CoverageUnitId, CropTreeId = c.CropTreeId, Label = ConflictChip, Reason = ConflictText(c.Kind), IsConflict = true });
        return rows;
    }

    // -------------------------------------------------------------------------------------------------------------- the gate

    public readonly struct ApprovalGate
    {
        public readonly bool CanApprove, HasPrescription, PreviewValid, HasProposals, SpotCheckDone, AlreadyApproved;
        public readonly string Reason;
        public ApprovalGate(bool hasPrescription, bool alreadyApproved, bool previewValid, bool hasProposals, bool spotCheckDone)
        {
            HasPrescription = hasPrescription; AlreadyApproved = alreadyApproved; PreviewValid = previewValid; HasProposals = hasProposals; SpotCheckDone = spotCheckDone;
            CanApprove = hasPrescription && !alreadyApproved && previewValid && hasProposals && spotCheckDone;
            Reason = !hasPrescription ? "Prepare a prescription for review first."
                : alreadyApproved ? "This prescription is already approved."
                : !previewValid ? "The proposals cannot be read now."
                : !hasProposals ? "There are no proposals to approve."
                : !spotCheckDone ? "Spot-check at least one extrapolated area in 3D first: use Inspect in forest, walk there and inspect a tree in it."
                : "";
        }
    }

    // Prescription approval needs a valid active snapshot, a current read-only preview with proposals, and a real 3D spot-check.
    public static ApprovalGate EvaluateGate(Session session, Appr.ProposalSet preview)
        => new ApprovalGate(session.ActivePrescriptionId > 0, session.ActiveApproved, preview != null && preview.Valid,
            preview != null && preview.Valid && preview.Links.Count > 0, session.SpotCheckDone);

    // -------------------------------------------------------------------------------------------------------- 3D companion

    // What the normal tree-inspection card adds for a tree that is a candidate or a proposal target. Plain data.
    public sealed class TreeContext
    {
        public bool IsCandidate, IsProposal, IsConflict;
        public Cand.Decision Decision;
        public string Chip = "", Line = "", Hint = "", Key = "";
        public bool Any => IsCandidate || IsProposal || IsConflict;
    }

    public static TreeContext DescribeTree(string treeId, bool liveCrop, Session session)
    {
        var context = new TreeContext();
        if (string.IsNullOrEmpty(treeId) || session == null) return context;
        if (session.LastCandidates != null && session.LastCandidates.Valid)
            foreach (Cand.Candidate c in session.LastCandidates.Candidates)
            {
                if (c.TreeId != treeId) continue;
                context.IsCandidate = true;
                context.Decision = session.DecisionNow(treeId, c.Decision, liveCrop);
                context.Chip = context.Decision == Cand.Decision.Confirmed ? ConfirmedChip + " (was a candidate)" : context.Decision == Cand.Decision.Rejected ? "CANDIDATE — rejected" : CandidateChip;
                context.Line = context.Decision == Cand.Decision.Open ? CandidateWarning : context.Decision == Cand.Decision.Rejected ? "You rejected this candidate. No mark was made." : "";
                context.Hint = context.Decision == Cand.Decision.Open ? ConfirmKeyHint + "  " + RejectKeyHint
                    : context.Decision == Cand.Decision.Rejected ? ReopenKeyHint : "";
                context.Key = "cand|" + c.Order + "|" + context.Decision;
                break;
            }
        Appr.ProposalSet preview = session.LastPreview;
        if (preview != null && preview.Valid && !session.ActiveApproved)
        {
            foreach (ScenarioOnePrescription.Proposal p in preview.Proposals)
                if (p.TreeId == treeId)
                {
                    context.IsProposal = true;
                    context.Chip = ProposalChip;
                    context.Line = ReasonOf(p.Reason) + ": " + Num(p.ShareOfCropTree * 100.0, "0") + " % of Crop Tree " + p.ForCropTreeId + "'s competition. Nothing is marked or planned.";
                    context.Key += "|prop|" + p.ForCropTreeId;
                    return context;
                }
            foreach (Appr.Conflict c in preview.Conflicts)
                if (c.TreeId == treeId)
                {
                    context.IsConflict = true;
                    context.Chip = ConflictChip;
                    context.Line = ConflictText(c.Kind) + ".";
                    context.Key += "|conf|" + c.Kind;
                    return context;
                }
        }
        return context;
    }
}
