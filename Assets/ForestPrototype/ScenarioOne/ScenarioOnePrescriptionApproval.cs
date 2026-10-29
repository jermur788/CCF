using System;
using System.Collections.Generic;
using System.Text;
using Stem = ScenarioOneCoverage.Stem;

// The proposal state and prescription-approval boundary (MFM-07, D-065, spec sections 11-13). Plain data, no UnityEngine, so the
// boundary can be checked on plain .NET (Tools/Verification/MfmApprovalBoundary) and gives the same answer everywhere.
//
// What this layer is:
//   * PROPOSE   - the release the stored prescription would make, recomputed deterministically from the stored prescription and the
//                 CURRENT confirmed state. A proposal is plain data: never a TreeMarkType.Fell mark, never a work order, never saved.
//   * VALIDATE  - the mandatory MFM-06 review condition. Before any prescription-generated order is written, its prescription, coverage
//                 unit and Crop Tree are checked against the prescription as it stands NOW. Any stale or invalid link fails closed.
//   * BUILD     - the full prescription approval check. Either every generated order is valid and returned together, or nothing is.
//
// Nothing here writes state. The manager (ScenarioOneManager.PrescriptionApproval.cs) is the only writer and writes only after Build
// returned Ok, in one step. Prescription approval creates PENDING ordinary work; the Work Plan stays the commitment gate.
//
// Player precedence (spec 11.2): a manual Fell mark or any open work order on a tree always wins. The tree is left out of the proposal
// and listed as a conflict so it is surfaced, never silently overwritten or silently dropped.
public static class ScenarioOnePrescriptionApproval
{
    // Bounded, closed vocabulary for "why was this refused". A message is built from these and short ids only.
    public enum Refusal
    {
        None = 0,
        NoPrescription,
        WrongPrescription,
        RuleVersionMismatch,
        CalibrationMismatch,
        NotAccepted,
        SampleNotDerived,
        SampleChanged,
        CoverageInvalid,
        UnitUnknown,
        UnitIsSource,
        UnitNotCovered,
        UnitNotAccepted,
        TargetMissing,
        TargetNotInUnit,
        TargetNotEligible,
        TargetHasManualState,
        CropTreeMissing,
        CropTreeNotConfirmed,
        CropTreeUnitNotAccepted,
        ReasonUnknown,
        NotInProposal,
        SafeguardExceeded,
        NothingToApprove
    }

    // A prescription's persisted choices and the live world it must still agree with. Built by the caller from stored data and live state.
    public sealed class Context
    {
        public int PrescriptionId;
        public string RuleVersion = "", CalibrationVersion = "";
        // A PrescriptionApprovalState value.
        public int ApprovalState;
        // Stored sample identity: what the prescription was derived from.
        public int SourceRackId;
        public float SourceAnchorX, SourceAnchorZ;
        public IReadOnlyCollection<string> SampleCropTreeIds = new string[0], SampleRemovalIds = new string[0];
        // The player's coverage choices (PrescriptionCoverageChoice) by coverage unit id. Only deliberate choices are stored.
        public IReadOnlyDictionary<string, int> CoverageChoices = new Dictionary<string, int>();
        // The rule re-derived from the live sample now, and the live coverage / stems it must still fit.
        public ScenarioOnePrescription.Derivation Derivation;
        public ScenarioOneCoverage.Result Coverage;
        public IReadOnlyList<Stem> Stems = new Stem[0];
        // Trees that already carry an open work order of any kind (manual or from any prescription): player/other state wins.
        public IReadOnlyCollection<string> OpenOrderTreeIds = new string[0];
        // The subset of those that are open FELLING orders: their basal area counts against each unit's ceiling together with the proposal.
        public IReadOnlyCollection<string> OpenFellingTreeIds = new string[0];
    }

    // What an order will carry, as written into the work order's provenance fields.
    public readonly struct OrderLink
    {
        public readonly int PrescriptionId;
        public readonly string TargetTreeId, CoverageUnitId, CropTreeId;
        public readonly int ReasonCode;
        public OrderLink(int prescriptionId, string targetTreeId, string coverageUnitId, string cropTreeId, int reasonCode)
        {
            PrescriptionId = prescriptionId; TargetTreeId = targetTreeId ?? ""; CoverageUnitId = coverageUnitId ?? "";
            CropTreeId = cropTreeId ?? ""; ReasonCode = reasonCode;
        }
        public string Key => PrescriptionId + ":" + TargetTreeId + "@" + CoverageUnitId + ">" + CropTreeId + "#" + ReasonCode;
    }

    public readonly struct LinkCheck
    {
        public readonly Refusal Refusal;
        public readonly string Message;
        public bool Ok => Refusal == Refusal.None;
        public LinkCheck(Refusal refusal, string message) { Refusal = refusal; Message = message ?? ""; }
        public static readonly LinkCheck Pass = new LinkCheck(Refusal.None, "");
    }

    public enum ConflictKind { ManualFellMark, OpenWorkOrder }

    public sealed class Conflict
    {
        public string TreeId = "", CoverageUnitId = "", CropTreeId = "";
        public ConflictKind Kind;
    }

    public sealed class ProposalSet
    {
        public bool Valid;
        public Refusal Refusal;
        public string Message = "";
        // Sorted by tree id; each tree at most once. Preview only: not marks, not orders, not saved.
        public readonly List<OrderLink> Links = new List<OrderLink>();
        public readonly List<ScenarioOnePrescription.Proposal> Proposals = new List<ScenarioOnePrescription.Proposal>();
        // Sorted by tree id: trees the rule would have proposed but that the player's own state holds.
        public readonly List<Conflict> Conflicts = new List<Conflict>();
        public ScenarioOnePrescription.ReleasePlan Plan;
        public ScenarioOnePrescription.SafeguardReport Safeguard = new ScenarioOnePrescription.SafeguardReport();

        public string Signature()
        {
            if (!Valid) return "INVALID " + Refusal;
            var sb = new StringBuilder("ok");
            foreach (OrderLink link in Links) sb.Append(' ').Append(link.Key);
            foreach (Conflict c in Conflicts) sb.Append(" x ").Append(c.TreeId).Append('@').Append(c.CoverageUnitId).Append(':').Append(c.Kind);
            return sb.ToString();
        }
    }

    public sealed class ApprovalPlan
    {
        public bool Ok;
        public Refusal Refusal;
        public string Message = "";
        // Every order to write, or empty. Never a prefix of the valid ones.
        public readonly List<OrderLink> Orders = new List<OrderLink>();
        public ProposalSet Proposals;
    }

    // ---------------------------------------------------------------------------------------------------------------- the prescription

    // The checks that tie a stored prescription to the rule, calibration and sample of NOW.
    public static LinkCheck ValidatePrescription(Context ctx, int expectedPrescriptionId)
    {
        if (ctx == null) return Fail(Refusal.NoPrescription, "The prescription does not exist.");
        if (expectedPrescriptionId < 1 || ctx.PrescriptionId != expectedPrescriptionId)
            return Fail(Refusal.WrongPrescription, "The order names prescription " + expectedPrescriptionId + ", not the stored one.");
        if (ctx.RuleVersion != ScenarioOnePrescriptionCalibration.RuleVersion)
            return Fail(Refusal.RuleVersionMismatch, "The prescription was made under rule " + Clip(ctx.RuleVersion) + ", not the current rule.");
        if (ctx.Coverage == null || !ctx.Coverage.Valid)
            return Fail(Refusal.CoverageInvalid, "Coverage cannot be read now" + (ctx.Coverage != null ? ": " + Clip(ctx.Coverage.Problem) : "."));
        if (ctx.CalibrationVersion != ctx.Coverage.Calibration.Version)
            return Fail(Refusal.CalibrationMismatch, "The prescription was made under calibration " + Clip(ctx.CalibrationVersion) + ".");
        if (ctx.ApprovalState != PrescriptionApprovalState.Accepted)
            return Fail(Refusal.NotAccepted, "The prescription is not waiting for approval.");
        ScenarioOnePrescription.Derivation d = ctx.Derivation;
        if (d == null || !d.Ready)
            return Fail(Refusal.SampleNotDerived, "The source sample cannot be read now" + (d != null ? ": " + Clip(d.Problem) : "."));
        if (d.RuleVersion != ctx.RuleVersion)
            return Fail(Refusal.RuleVersionMismatch, "The sample was read under a different rule version.");
        if (d.TeachingRackId != ctx.SourceRackId
            || Math.Abs(d.AnchorX - ctx.SourceAnchorX) > 1e-3 || Math.Abs(d.AnchorZ - ctx.SourceAnchorZ) > 1e-3
            || !SameSet(d.SampleCropTreeIds, ctx.SampleCropTreeIds) || !SameSet(d.SampleRemovalIds, ctx.SampleRemovalIds))
            return Fail(Refusal.SampleChanged, "The source sample no longer matches the one this prescription was made from.");
        return LinkCheck.Pass;
    }

    // ------------------------------------------------------------------------------------------------------------------------ propose

    // The release this prescription would make now. Only coverage units the player accepted are served; only confirmed Crop Trees are
    // released; the player's marks and open orders hold their trees. A proposal set is recomputed on demand and never stored.
    public static ProposalSet Propose(Context ctx)
    {
        var set = new ProposalSet();
        LinkCheck header = ValidatePrescription(ctx, ctx != null ? ctx.PrescriptionId : 0);
        if (!header.Ok) return Refuse(set, header);

        var accepted = AcceptedCoverage(ctx);
        var living = new Dictionary<string, Stem>(StringComparer.Ordinal);
        foreach (Stem s in ctx.Stems) if (s.Living && s.Id.Length > 0) living[s.Id] = s;

        // Every living Crop Tree the player holds. Ones in unaccepted units, or the sample's own, are reported by the plan and get nothing.
        var crops = new List<string>();
        foreach (Stem s in living.Values) if (s.Crop) crops.Add(s.Id);
        crops.Sort(StringComparer.Ordinal);

        // The tree-level player state that wins: a manual Fell mark, or any open order.
        var openOrders = new HashSet<string>(ctx.OpenOrderTreeIds ?? new string[0], StringComparer.Ordinal);
        var marked = new HashSet<string>(StringComparer.Ordinal);
        foreach (Stem s in living.Values) if (s.FellMark) marked.Add(s.Id);

        // What the rule would do with no player state in the way, to name exactly the trees the player's state held back.
        var unmarked = new List<Stem>(ctx.Stems.Count);
        foreach (Stem s in ctx.Stems)
            unmarked.Add(new Stem(s.Id, s.X, s.Z, s.DbhCm, s.Living, s.SpeciesId, s.Crop, false));
        ScenarioOnePrescription.ReleasePlan ideal = ScenarioOnePrescription.ProposeRelease(ctx.Derivation.Rule, accepted, unmarked, crops,
            ctx.SampleCropTreeIds, null);
        if (!ideal.Valid) return Refuse(set, new LinkCheck(Refusal.CoverageInvalid, "Proposals cannot be computed: " + Clip(ideal.Problem)));

        ScenarioOnePrescription.ReleasePlan plan = ScenarioOnePrescription.ProposeRelease(ctx.Derivation.Rule, accepted, ctx.Stems, crops,
            ctx.SampleCropTreeIds, openOrders);
        if (!plan.Valid) return Refuse(set, new LinkCheck(Refusal.CoverageInvalid, "Proposals cannot be computed: " + Clip(plan.Problem)));
        set.Plan = plan;

        foreach (ScenarioOnePrescription.Proposal p in ideal.Proposals)
        {
            bool isMarked = marked.Contains(p.TreeId), isOpen = openOrders.Contains(p.TreeId);
            if (!isMarked && !isOpen) continue;
            set.Conflicts.Add(new Conflict
            {
                TreeId = p.TreeId, CoverageUnitId = p.AreaId, CropTreeId = p.ForCropTreeId,
                Kind = isMarked ? ConflictKind.ManualFellMark : ConflictKind.OpenWorkOrder
            });
        }
        foreach (ScenarioOnePrescription.Proposal p in plan.Proposals)
        {
            set.Proposals.Add(p);
            set.Links.Add(new OrderLink(ctx.PrescriptionId, p.TreeId, p.AreaId, p.ForCropTreeId, ReasonCodeOf(p.Reason)));
        }

        // The ceiling counts the player's own pending felling in each accepted unit as well as the proposed removals. Only trees that lie in
        // an accepted unit can be charged to one; the rest are outside this prescription's reach.
        var inAccepted = new HashSet<string>(StringComparer.Ordinal);
        foreach (ScenarioOneCoverage.Area area in accepted.Areas) foreach (string id in area.StemIds) inAccepted.Add(id);
        var charged = new SortedSet<string>(StringComparer.Ordinal);
        foreach (ScenarioOnePrescription.Proposal p in plan.Proposals) charged.Add(p.TreeId);
        foreach (string id in marked) if (inAccepted.Contains(id)) charged.Add(id);
        foreach (string id in ctx.OpenFellingTreeIds ?? new string[0]) if (inAccepted.Contains(id)) charged.Add(id);
        set.Safeguard = ScenarioOnePrescription.EvaluateSafeguard(accepted, ctx.Stems, charged, ctx.Derivation.Rule.BasalCeilingShare);
        set.Valid = true;
        return set;
    }

    // ------------------------------------------------------------------------------------------------------------- the order link check

    // The mandatory MFM-06 review condition, for one order about to be written. A link is valid only when the prescription, its coverage
    // unit and its Crop Tree each hold now AND the link is one the prescription would itself generate now.
    public static LinkCheck ValidateLink(Context ctx, OrderLink link) => ValidateLinks(ctx, new[] { link }, null);

    // Validates several links against one recomputation. Returns the first failure, so the answer is deterministic by link order.
    public static LinkCheck ValidateLinks(Context ctx, IReadOnlyList<OrderLink> links, ProposalSet precomputed)
    {
        if (links == null || links.Count == 0) return Fail(Refusal.NothingToApprove, "There is no order to check.");
        ProposalSet current = precomputed;
        foreach (OrderLink link in links)
        {
            LinkCheck header = ValidatePrescription(ctx, link.PrescriptionId);
            if (!header.Ok) return header;
            if (!PrescriptionReasonCode.IsKnown(link.ReasonCode))
                return Fail(Refusal.ReasonUnknown, "Order for " + Clip(link.TargetTreeId) + " has an unknown reason code.");

            // Coverage unit.
            ScenarioOneCoverage.Area unit = string.IsNullOrEmpty(link.CoverageUnitId) ? null : ctx.Coverage.Find(link.CoverageUnitId);
            if (unit == null) return Fail(Refusal.UnitUnknown, "Coverage unit " + Clip(link.CoverageUnitId) + " does not exist now.");
            if (unit.IsSource) return Fail(Refusal.UnitIsSource, "Coverage unit " + Clip(unit.Id) + " is the sample itself.");
            if (unit.State != ScenarioOneCoverage.State.Covered)
                return Fail(Refusal.UnitNotCovered, "Coverage unit " + Clip(unit.Id) + " is no longer a covered area.");
            if (!IsAccepted(ctx, unit.Id))
                return Fail(Refusal.UnitNotAccepted, "Coverage unit " + Clip(unit.Id) + " was not accepted for generated work.");

            // The tree the order fells.
            Stem target = default;
            bool targetFound = false;
            foreach (Stem s in ctx.Stems) if (s.Living && s.Id == link.TargetTreeId) { target = s; targetFound = true; break; }
            if (!targetFound) return Fail(Refusal.TargetMissing, "Tree " + Clip(link.TargetTreeId) + " is not a living tree now.");
            if (!unit.StemIds.Contains(target.Id))
                return Fail(Refusal.TargetNotInUnit, "Tree " + Clip(target.Id) + " is not in coverage unit " + Clip(unit.Id) + ".");
            if (!target.IsSitka || target.Crop || Contains(ctx.SampleCropTreeIds, target.Id))
                return Fail(Refusal.TargetNotEligible, "Tree " + Clip(target.Id) + " cannot be removed by this prescription.");
            if (target.FellMark || Contains(ctx.OpenOrderTreeIds, target.Id))
                return Fail(Refusal.TargetHasManualState, "Tree " + Clip(target.Id) + " is already marked or planned by the player.");

            // The confirmed Crop Tree the order serves.
            if (string.IsNullOrEmpty(link.CropTreeId))
                return Fail(Refusal.CropTreeMissing, "Order for " + Clip(target.Id) + " names no Crop Tree.");
            Stem crop = default;
            bool cropFound = false;
            foreach (Stem s in ctx.Stems) if (s.Living && s.Id == link.CropTreeId) { crop = s; cropFound = true; break; }
            if (!cropFound || !crop.IsSitka || !crop.Crop || Contains(ctx.SampleCropTreeIds, crop.Id))
                return Fail(Refusal.CropTreeNotConfirmed, "Crop Tree " + Clip(link.CropTreeId) + " is not a confirmed Crop Tree now.");
            ScenarioOneCoverage.Area home = HomeArea(ctx.Coverage, crop.Id);
            if (home == null || home.State != ScenarioOneCoverage.State.Covered || !IsAccepted(ctx, home.Id))
                return Fail(Refusal.CropTreeUnitNotAccepted, "Crop Tree " + Clip(crop.Id) + " is not in an accepted coverage unit.");

            // Finally: the prescription must itself generate exactly this order now (reach, ceiling, order and determinism).
            if (current == null) current = Propose(ctx);
            if (!current.Valid) return Fail(current.Refusal, current.Message);
            bool listed = false;
            foreach (OrderLink l in current.Links) if (l.Key == link.Key) { listed = true; break; }
            if (!listed) return Fail(Refusal.NotInProposal, "Order for " + Clip(target.Id) + " is not one this prescription proposes now.");
        }
        return LinkCheck.Pass;
    }

    // ---------------------------------------------------------------------------------------------------------------------- the approval

    // Everything a prescription approval needs, decided before any state changes. Ok only when the proposal is within every ceiling and
    // every generated order's link validates; then Orders holds all of them, in tree id order. Otherwise nothing is returned to write.
    public static ApprovalPlan Build(Context ctx)
    {
        var plan = new ApprovalPlan();
        ProposalSet set = Propose(ctx);
        plan.Proposals = set;
        if (!set.Valid) return Stop(plan, set.Refusal, set.Message);
        if (set.Links.Count == 0) return Stop(plan, Refusal.NothingToApprove, "The prescription proposes no removal now.");
        if (!set.Safeguard.AllWithin)
            return Stop(plan, Refusal.SafeguardExceeded, set.Safeguard.InvalidInput
                ? "The basal-area ceiling cannot be checked: " + Clip(set.Safeguard.InvalidReason)
                : "The proposal, with the player's own planned felling, would exceed a coverage unit's basal-area ceiling.");
        LinkCheck check = ValidateLinks(ctx, set.Links, set);
        if (!check.Ok) return Stop(plan, check.Refusal, check.Message);
        plan.Orders.AddRange(set.Links);
        plan.Ok = true;
        return plan;
    }

    // ------------------------------------------------------------------------------------------------------------------------ helpers

    // A closed reason code for a proposal: ProposalReason + 1, because 0 means "no prescription reason" (ordinary order).
    public static int ReasonCodeOf(ScenarioOnePrescription.ProposalReason reason) => (int)reason + 1;

    // A copy of the coverage holding only the covered, non-source units the player accepted. Areas are shared, not changed.
    private static ScenarioOneCoverage.Result AcceptedCoverage(Context ctx)
    {
        var accepted = new ScenarioOneCoverage.Result { Valid = true, Calibration = ctx.Coverage.Calibration, TeachingRackId = ctx.Coverage.TeachingRackId };
        foreach (ScenarioOneCoverage.Area area in ctx.Coverage.Areas)
            if (!area.IsSource && area.State == ScenarioOneCoverage.State.Covered && IsAccepted(ctx, area.Id))
                accepted.Areas.Add(area);
        return accepted;
    }

    private static bool IsAccepted(Context ctx, string unitId)
        => ctx.CoverageChoices != null && ctx.CoverageChoices.TryGetValue(unitId, out int choice) && choice == PrescriptionCoverageChoice.AcceptedCovered;

    private static ScenarioOneCoverage.Area HomeArea(ScenarioOneCoverage.Result coverage, string treeId)
    {
        foreach (ScenarioOneCoverage.Area area in coverage.Areas)
            if (area.StemIds.Contains(treeId)) return area;
        return null;
    }

    private static bool SameSet(IReadOnlyCollection<string> a, IReadOnlyCollection<string> b)
    {
        var left = new SortedSet<string>(a ?? new string[0], StringComparer.Ordinal);
        var right = new SortedSet<string>(b ?? new string[0], StringComparer.Ordinal);
        return left.SetEquals(right);
    }

    private static bool Contains(IReadOnlyCollection<string> ids, string id)
    {
        if (ids != null) foreach (string x in ids) if (x == id) return true;
        return false;
    }

    // Ids and problem text in a message are clipped so a reason stays short and bounded whatever the input.
    private static string Clip(string text)
    {
        if (string.IsNullOrEmpty(text)) return "(none)";
        return text.Length <= 48 ? text : text.Substring(0, 48) + "...";
    }

    private static LinkCheck Fail(Refusal refusal, string message) => new LinkCheck(refusal, message);

    private static ProposalSet Refuse(ProposalSet set, LinkCheck check)
    {
        set.Valid = false;
        set.Refusal = check.Refusal;
        set.Message = check.Message;
        return set;
    }

    private static ApprovalPlan Stop(ApprovalPlan plan, Refusal refusal, string message)
    {
        plan.Ok = false;
        plan.Refusal = refusal;
        plan.Message = message;
        plan.Orders.Clear();
        return plan;
    }
}
