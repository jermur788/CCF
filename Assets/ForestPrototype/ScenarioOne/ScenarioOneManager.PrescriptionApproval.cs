using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Stem = ScenarioOneCoverage.Stem;
using Approval = ScenarioOnePrescriptionApproval;

// MFM-07 prescription approval boundary. The only writer of prescription records and prescription-generated work.
//
//   * A proposal is NOT stored here. It is recomputed on demand (PreviewPrescription) from the stored prescription and the live, confirmed
//     state; it is never a tree mark, never an order, never saved, and nothing in the Work Plan reads it.
//   * ApprovePrescription is all-or-nothing. It decides everything first (ScenarioOnePrescriptionApproval.Build, then a second validation of
//     every order's prescription, coverage unit and Crop Tree against a fresh read immediately before the first write) and only then writes
//     ordinary PENDING felling orders with prescription provenance. A refusal changes nothing: no order, no id, no event, no record.
//   * It never approves, funds or executes anything. The Work Plan approval (ApprovePendingWork) remains the separate commitment gate and
//     the existing contractor, cash and execution rules apply to the generated orders unchanged.
public sealed class PrescriptionOperationResult
{
    public bool Succeeded;
    public ScenarioOnePrescriptionApproval.Refusal Refusal;
    public string Message = "";
    public int PrescriptionId;
    public int OrdersCreated;
    // Trees the rule would have proposed but the player's own marks or orders hold; surfaced, never overwritten.
    public int ConflictsSurfaced;
}

public sealed partial class ScenarioOneManager
{
    // The save validator's cap on stored prescriptions (ForestSaveValidation.MaxPrescriptions).
    private const int MaxStoredPrescriptions = 1024;

    public PrescriptionSaveData FindPrescription(int prescriptionId)
        => prescriptions.FirstOrDefault(item => item != null && item.prescriptionId == prescriptionId);

    // ---------------------------------------------------------------------------------------------------------------- create (accept)

    // Records the player's coverage choices for the current confirmed sample as an ACCEPTED prescription. No proposal is made a mark or an
    // order by this: acceptance only stores which units the player lets the rule serve. Units are named explicitly; none is implied.
    public PrescriptionOperationResult CreatePrescription(IReadOnlyCollection<string> acceptedUnitIds, IReadOnlyCollection<string> manualUnitIds = null)
    {
        if (!CanManage()) return Reject(Approval.Refusal.NotAccepted, feedback);
        if (prescriptions.Count >= MaxStoredPrescriptions) return Reject(Approval.Refusal.NoPrescription, "Too many prescriptions are stored.");
        var accepted = new SortedSet<string>(acceptedUnitIds ?? new string[0], StringComparer.Ordinal);
        var manual = new SortedSet<string>(manualUnitIds ?? new string[0], StringComparer.Ordinal);
        if (accepted.Count == 0) return Reject(Approval.Refusal.NothingToApprove, "Choose at least one covered area to accept.");
        if (accepted.Overlaps(manual)) return Reject(Approval.Refusal.UnitNotAccepted, "A coverage unit cannot be both accepted and left manual.");

        if (!TryReadPrescriptionWorld(manual, out PrescriptionWorld world, out string problem))
            return Reject(Approval.Refusal.SampleNotDerived, problem);
        foreach (string id in accepted)
        {
            ScenarioOneCoverage.Area area = world.Coverage.Find(id);
            if (area == null) return Reject(Approval.Refusal.UnitUnknown, "Coverage unit " + id + " does not exist.");
            if (area.IsSource) return Reject(Approval.Refusal.UnitIsSource, "Coverage unit " + id + " is the sample itself.");
            if (area.State != ScenarioOneCoverage.State.Covered) return Reject(Approval.Refusal.UnitNotCovered, "Coverage unit " + id + " is not a covered area.");
        }
        foreach (string id in manual)
            if (world.Coverage.Find(id) == null) return Reject(Approval.Refusal.UnitUnknown, "Coverage unit " + id + " does not exist.");

        ScenarioOnePrescription.Derivation d = world.Derivation;
        var record = new PrescriptionSaveData
        {
            prescriptionId = nextPrescriptionId,
            ruleVersion = d.RuleVersion,
            calibrationVersion = d.CalibrationVersion,
            sourceRackId = d.TeachingRackId,
            sourceYear = CurrentYear,
            sourceAnchorX = d.AnchorX,
            sourceAnchorZ = d.AnchorZ,
            sampleCropTreeIds = d.SampleCropTreeIds.OrderBy(id => id, StringComparer.Ordinal).ToList(),
            sampleRemovalIds = d.SampleRemovalIds.OrderBy(id => id, StringComparer.Ordinal).ToList(),
            approvalState = PrescriptionApprovalState.Accepted,
            approvalYear = -1
        };
        foreach (string id in accepted)
            record.coverageDecisions.Add(new PrescriptionCoverageDecisionSaveData { unitId = id, choice = PrescriptionCoverageChoice.AcceptedCovered });
        foreach (string id in manual)
            record.coverageDecisions.Add(new PrescriptionCoverageDecisionSaveData { unitId = id, choice = PrescriptionCoverageChoice.LeftManual });
        record.coverageDecisions.Sort((a, b) => string.CompareOrdinal(a.unitId, b.unitId));

        prescriptions.Add(record);
        nextPrescriptionId = Mathf.Max(nextPrescriptionId, record.prescriptionId) + 1;
        feedback = $"Prescription {record.prescriptionId} accepted for {accepted.Count} area{(accepted.Count == 1 ? "" : "s")}. Nothing is marked or planned yet.";
        return new PrescriptionOperationResult { Succeeded = true, PrescriptionId = record.prescriptionId, Message = feedback };
    }

    // ----------------------------------------------------------------------------------------------------------------------- preview

    // The proposals the stored prescription makes right now. Read-only and recomputed every call: it writes no mark, order, record or id.
    public Approval.ProposalSet PreviewPrescription(int prescriptionId)
    {
        PrescriptionSaveData record = FindPrescription(prescriptionId);
        if (record == null)
            return new Approval.ProposalSet { Refusal = Approval.Refusal.NoPrescription, Message = "The prescription does not exist." };
        Approval.Context ctx = BuildApprovalContext(record, out string problem);
        if (ctx == null)
            return new Approval.ProposalSet { Refusal = Approval.Refusal.SampleNotDerived, Message = problem };
        return Approval.Propose(ctx);
    }

    // The mandatory pre-write link check, exposed for one order: does this prescription, coverage unit and Crop Tree still hold for the tree
    // in the world as it stands now? ApprovePrescription runs the same check on every order it would write. Read-only.
    public Approval.LinkCheck CheckPrescriptionLink(int prescriptionId, string targetTreeId, string coverageUnitId, string cropTreeId, int reasonCode)
    {
        PrescriptionSaveData record = FindPrescription(prescriptionId);
        if (record == null) return new Approval.LinkCheck(Approval.Refusal.NoPrescription, "The prescription does not exist.");
        Approval.Context ctx = BuildApprovalContext(record, out string problem);
        if (ctx == null) return new Approval.LinkCheck(Approval.Refusal.SampleNotDerived, problem);
        return Approval.ValidateLink(ctx, new Approval.OrderLink(prescriptionId, targetTreeId, coverageUnitId, cropTreeId, reasonCode));
    }

    // ----------------------------------------------------------------------------------------------------------------------- approve

    // Prescription approval: turns the prescription's validated proposals into ordinary PENDING felling orders carrying provenance.
    // All-or-nothing. It does not approve, fund or execute work; the Work Plan approval is still required.
    public PrescriptionOperationResult ApprovePrescription(int prescriptionId)
    {
        if (!CanManage()) return Reject(Approval.Refusal.NotAccepted, feedback);
        PrescriptionSaveData record = FindPrescription(prescriptionId);
        if (record == null) return Reject(Approval.Refusal.NoPrescription, "The prescription does not exist.");
        Approval.Context ctx = BuildApprovalContext(record, out string problem);
        if (ctx == null) return Reject(Approval.Refusal.SampleNotDerived, problem);

        // 1. Decide everything first. Any stale or invalid prescription, unit, Crop Tree or ceiling stops here with nothing changed.
        Approval.ApprovalPlan plan = Approval.Build(ctx);
        if (!plan.Ok) return Reject(plan.Refusal, plan.Message, prescriptionId);

        // 2. Resolve every target to a live, fellable tree before any write.
        Dictionary<string, ForestTree> living = LivingTreesById();
        foreach (Approval.OrderLink link in plan.Orders)
            if (!living.TryGetValue(link.TargetTreeId, out ForestTree tree) || tree == null || !tree.CanChop)
                return Reject(Approval.Refusal.TargetNotEligible, "Tree " + link.TargetTreeId + " cannot be felled now.", prescriptionId);

        // 3. The mandatory pre-write check, against a fresh read taken immediately before the first order exists.
        Approval.Context fresh = BuildApprovalContext(record, out problem);
        if (fresh == null) return Reject(Approval.Refusal.SampleNotDerived, problem, prescriptionId);
        Approval.LinkCheck check = Approval.ValidateLinks(fresh, plan.Orders, null);
        if (!check.Ok) return Reject(check.Refusal, check.Message, prescriptionId);

        // 4. Write, with a complete rollback so no partial work state can survive an unexpected failure.
        int savedNextOrderId = nextWorkOrderId, savedNextEventId = nextManagementEventId;
        int savedOrderCount = workOrders.Count, savedEventCount = managementEvents.Count;
        int savedApprovalState = record.approvalState, savedApprovalYear = record.approvalYear;
        try
        {
            var created = new List<ScenarioOneWorkOrder>(plan.Orders.Count);
            foreach (Approval.OrderLink link in plan.Orders)
            {
                ScenarioOneWorkOrder order = CreateFellingOrder(living[link.TargetTreeId]);
                order.prescriptionId = link.PrescriptionId;
                order.prescriptionCoverageUnitId = link.CoverageUnitId;
                order.prescriptionCropTreeId = link.CropTreeId;
                order.prescriptionReason = link.ReasonCode;
                created.Add(order);
            }
            foreach (ScenarioOneWorkOrder order in created)
            {
                workOrders.Add(order);
                RecordOrderEvent(order, ScenarioManagementEventType.OrderCreated, ScenarioManagementOutcome.None, CurrentYear).prescriptionId = order.prescriptionId;
            }
            record.approvalState = PrescriptionApprovalState.Approved;
            record.approvalYear = CurrentYear;
            GetHarvestQuote(false);
        }
        catch (Exception exception)
        {
            workOrders.RemoveRange(savedOrderCount, workOrders.Count - savedOrderCount);
            managementEvents.RemoveRange(savedEventCount, managementEvents.Count - savedEventCount);
            nextWorkOrderId = savedNextOrderId;
            nextManagementEventId = savedNextEventId;
            record.approvalState = savedApprovalState;
            record.approvalYear = savedApprovalYear;
            InvalidateEconomyQuotes();
            Debug.LogWarning("Prescription approval rolled back: " + exception.Message);
            return Reject(Approval.Refusal.NotInProposal, "Approval failed and nothing was changed.", prescriptionId);
        }

        int count = plan.Orders.Count;
        feedback = $"Prescription {prescriptionId} approved: {count} pending task{(count == 1 ? "" : "s")} added to the Work Plan. They are not approved yet.";
        return new PrescriptionOperationResult
        {
            Succeeded = true, PrescriptionId = prescriptionId, OrdersCreated = count,
            ConflictsSurfaced = plan.Proposals.Conflicts.Count, Message = feedback
        };
    }

    private PrescriptionOperationResult Reject(Approval.Refusal refusal, string message, int prescriptionId = 0)
    {
        feedback = message;
        return new PrescriptionOperationResult { Succeeded = false, Refusal = refusal, Message = message, PrescriptionId = prescriptionId };
    }

    // ------------------------------------------------------------------------------------------------------------------- the live world

    private sealed class PrescriptionWorld
    {
        public ScenarioOneRackSampleGeometry.Result Geometry;
        public List<Stem> Stems;
        public ScenarioOnePrescription.Derivation Derivation;
        public ScenarioOneCoverage.Result Coverage;
    }

    // Reads the live confirmed rack sample, trees and Work Plan into the pure rules. Read-only. Infrastructure masks, management features
    // and the unit-by-unit manual choices other than the stored ones are NOT read here (a partial adapter, as in the MFM-04/05 gate): an
    // area that would need a mask to be excluded is therefore judged without it. Recorded as a residual in the MFM-07 handoff.
    private bool TryReadPrescriptionWorld(IReadOnlyCollection<string> manualUnitIds, out PrescriptionWorld world, out string problem)
    {
        world = null;
        if (!RackPlanningV2Active || !rackPlanConfirmed || ecology == null)
        {
            problem = "Prescriptions need a confirmed Rack Planning V2 plan.";
            return false;
        }
        float half = StandGeometryModel.StandSizeMeters(ecology.StandGeometryModelVersion) * 0.5f;
        ScenarioOneRackSampleGeometry.Result geometry = ScenarioOneRackSampleGeometry.Derive(ConfirmedRacks, half);
        if (!geometry.Valid)
        {
            problem = "The confirmed rack plan gives no valid sample: " + geometry.Problem;
            return false;
        }
        var stems = new List<Stem>();
        foreach (ForestTree tree in UnityEngine.Object.FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
        {
            if (tree == null || string.IsNullOrEmpty(tree.TreeId) || (!tree.IsLiving && !tree.IsStump)) continue;
            Vector3 p = tree.transform.position;
            stems.Add(new Stem(tree.TreeId, p.x, p.z, tree.Diameter, tree.IsLiving, tree.Species != null ? tree.Species.SpeciesId : "",
                tree.IsLiving && tree.IsCropTree, tree.IsLiving && tree.IsMarkedForFell));
        }
        // The same felling-order view the rack lesson reads (ScenarioOneRackBayReader): open = valid pending/approved, done = completed.
        var bayOrders = new List<ScenarioOneRackBay.Order>();
        foreach (ScenarioOneWorkOrder order in workOrders)
        {
            if (order == null || order.type != ScenarioWorkType.FellTree) continue;
            bool open = order.IsValidOpenFelling, done = order.status == ScenarioWorkStatus.Completed;
            if (!open && !done) continue;
            ScenarioOneRackBay.Disposition disposition = order.fellingOutcome == FellingMaterialOutcome.SellAndExtract ? ScenarioOneRackBay.Disposition.Sold
                : order.fellingOutcome == FellingMaterialOutcome.RetainAsFallenDeadwood ? ScenarioOneRackBay.Disposition.Deadwood
                : ScenarioOneRackBay.Disposition.Other;
            bayOrders.Add(new ScenarioOneRackBay.Order(order.targetTreeId, open, done, disposition, order.resolvedYear));
        }
        ScenarioOnePrescriptionCalibration calibration = ScenarioOnePrescriptionCalibration.V1;
        var coverage = ScenarioOneCoverage.Classify(new ScenarioOneCoverage.Input
        {
            Geometry = geometry, Stems = stems, Orders = bayOrders, Masks = new ScenarioOneCoverage.Mask[0],
            Features = new ScenarioOneCoverage.Feature[0], PlayerManualAreas = manualUnitIds ?? new string[0]
        }, calibration);
        if (!coverage.Valid)
        {
            problem = "Coverage cannot be read now: " + coverage.Problem;
            return false;
        }
        ScenarioOnePrescription.Derivation derivation = ScenarioOnePrescription.Derive(geometry, stems, bayOrders, calibration);
        if (!derivation.Ready)
        {
            problem = "The sample cannot be read now: " + derivation.Problem;
            return false;
        }
        world = new PrescriptionWorld { Geometry = geometry, Stems = stems, Derivation = derivation, Coverage = coverage };
        problem = "";
        return true;
    }

    // The pure-rule context for one stored prescription, read fresh from the live world. Null (with a reason) when the world cannot be read.
    private Approval.Context BuildApprovalContext(PrescriptionSaveData record, out string problem)
    {
        var manualUnits = new List<string>();
        var choices = new Dictionary<string, int>(StringComparer.Ordinal);
        foreach (PrescriptionCoverageDecisionSaveData decision in record.coverageDecisions)
        {
            choices[decision.unitId] = decision.choice;
            if (decision.choice == PrescriptionCoverageChoice.LeftManual) manualUnits.Add(decision.unitId);
        }
        if (!TryReadPrescriptionWorld(manualUnits, out PrescriptionWorld world, out problem)) return null;
        var openOrders = new SortedSet<string>(StringComparer.Ordinal);
        var openFelling = new SortedSet<string>(StringComparer.Ordinal);
        foreach (ScenarioOneWorkOrder order in workOrders)
        {
            if (order == null || !order.IsOpen || string.IsNullOrEmpty(order.targetTreeId)) continue;
            openOrders.Add(order.targetTreeId);
            if (order.type == ScenarioWorkType.FellTree) openFelling.Add(order.targetTreeId);
        }
        return new Approval.Context
        {
            PrescriptionId = record.prescriptionId,
            RuleVersion = record.ruleVersion,
            CalibrationVersion = record.calibrationVersion,
            ApprovalState = record.approvalState,
            SourceRackId = record.sourceRackId,
            SourceAnchorX = record.sourceAnchorX,
            SourceAnchorZ = record.sourceAnchorZ,
            SampleCropTreeIds = record.sampleCropTreeIds,
            SampleRemovalIds = record.sampleRemovalIds,
            CoverageChoices = choices,
            Derivation = world.Derivation,
            Coverage = world.Coverage,
            Stems = world.Stems,
            OpenOrderTreeIds = openOrders,
            OpenFellingTreeIds = openFelling
        };
    }
}
