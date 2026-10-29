#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Approval = ScenarioOnePrescriptionApproval;

// Disposable Editor-only gate for MFM-07: proposal -> prescription approval -> pending ordinary work on a live Rack V2 world.
// Stage with the runner, then remove source and .meta. Disk files are written only into the isolated persistentDataPath.
//
// It builds the sample the same way a player would: confirms the five qualified racks, approves and executes the rack access, designates the
// sample's three Crop Trees, marks and executes the in-footprint release. Only then does a prescription exist to be derived. Every step uses the
// production manager APIs; nothing is injected into the world, and the harness never calls the Windows build.
public static class MfmApprovalBoundaryUnityGate
{
    public static void Begin()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        EditorApplication.isPlaying = true;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (Environment.GetCommandLineArgs().Any(a => a == "MfmApprovalBoundaryUnityGate.Begin"))
            new GameObject("Disposable MFM-07 approval boundary gate").AddComponent<MfmApprovalBoundaryUnityGateRunner>();
    }
}

public sealed class MfmApprovalBoundaryUnityGateRunner : MonoBehaviour
{
    private static readonly string[] Entrances = { "PO0203", "PO1204", "PO2003", "PO2803", "PO3803" };
    private static readonly float[] QualifiedZ = { -35.2711754f, -16.4217644f, 0.2382353f, 15.7935295f, 35.2323532f };
    private static readonly string[] SampleCrops = { "P0810", "P1206", "P1214" };
    private ScenarioOneUiRoot ui;
    private ScenarioOneManager manager;
    private ForestEcologyController ecology;
    private ForestSaveController saves;
    private ForestTreeMarkingManager marks;
    private ForestSaveData original;
    private bool saveControllerWasEnabled;
    private int checks;
    private string SavePath => Path.Combine(Application.persistentDataPath, "forest-save.json");

    private void Check(bool ok, string what, string detail = "")
    {
        if (!ok) throw new InvalidOperationException(what + (detail.Length == 0 ? "" : " [" + detail + "]"));
        checks++;
        Debug.Log("MFM07_CHECK " + what + (detail.Length == 0 ? "" : " [" + detail + "]"));
    }

    private IEnumerator Start()
    {
        Exception failure = null;
        IEnumerator run = Run();
        while (true)
        {
            bool more; object current = null;
            try { more = run.MoveNext(); if (more) current = run.Current; }
            catch (Exception error) { failure = error; break; }
            if (!more) break;
            yield return current;
        }
        if (original != null)
        {
            try { Check(saves.LoadData(original, false), "original world restored in memory"); }
            catch (Exception error) { failure = failure == null ? error : new AggregateException(failure, error); }
            saves.enabled = saveControllerWasEnabled;
        }
        Debug.Log(failure == null ? "MFM07_UNITY_PASS checks=" + checks : "MFM07_UNITY_FAIL checks=" + checks + " " + failure);
        EditorApplication.Exit(failure == null ? 0 : 1);
    }

    // ------------------------------------------------------------------------------------------------------------------- helpers

    private static ForestTree[] LiveTrees() => FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
    private ForestTree Tree(string id) => LiveTrees().FirstOrDefault(t => t.TreeId == id);

    private static List<ScenarioOneCoverage.Stem> LiveStems()
    {
        var result = new List<ScenarioOneCoverage.Stem>();
        foreach (ForestTree tree in LiveTrees())
        {
            if (tree == null || string.IsNullOrEmpty(tree.TreeId) || (!tree.IsLiving && !tree.IsStump)) continue;
            Vector3 p = tree.transform.position;
            result.Add(new ScenarioOneCoverage.Stem(tree.TreeId, p.x, p.z, tree.Diameter, tree.IsLiving,
                tree.Species != null ? tree.Species.SpeciesId : "", tree.IsLiving && tree.IsCropTree, tree.IsLiving && tree.IsMarkedForFell));
        }
        return result;
    }

    private List<ScenarioOneRackBay.Order> LiveOrders()
    {
        var result = new List<ScenarioOneRackBay.Order>();
        foreach (ScenarioOneWorkOrder order in manager.WorkOrders)
        {
            if (order == null || order.type != ScenarioWorkType.FellTree) continue;
            bool open = order.IsValidOpenFelling, done = order.status == ScenarioWorkStatus.Completed;
            if (!open && !done) continue;
            var disposition = order.fellingOutcome == FellingMaterialOutcome.SellAndExtract ? ScenarioOneRackBay.Disposition.Sold
                : order.fellingOutcome == FellingMaterialOutcome.RetainAsFallenDeadwood ? ScenarioOneRackBay.Disposition.Deadwood : ScenarioOneRackBay.Disposition.Other;
            result.Add(new ScenarioOneRackBay.Order(order.targetTreeId, open, done, disposition, order.resolvedYear));
        }
        return result;
    }

    // Everything a refused approval must leave alone, as comparable text.
    private string Identity()
    {
        ForestSaveData data = saves.CaptureData();
        return string.Join("\n", new[]
        {
            ScenarioReferenceArchive.WorldHash(data),
            JsonUtility.ToJson(data.scenarioOne),
            string.Join(",", marks.GetMarkedIds().OrderBy(x => x, StringComparer.Ordinal)),
            string.Join(",", LiveTrees().Where(t => t.IsCropTree).Select(t => t.TreeId).OrderBy(x => x, StringComparer.Ordinal)),
            manager.WorkOrders.Count + "/" + manager.ManagementEvents.Count + "/" + manager.NextPrescriptionId + "/" + manager.CashCents
        });
    }

    private ScenarioOneRackSampleGeometry.Result Geometry()
        => ScenarioOneRackSampleGeometry.Derive(manager.ConfirmedRacks, StandGeometryModel.StandSizeMeters(ecology.StandGeometryModelVersion) * 0.5f);

    private ScenarioOneRackBay.Analysis Bay() => ScenarioOneRackBayReader.Read(LiveTrees(), manager.WorkOrders, Geometry());

    private ScenarioOneCoverage.Result Coverage()
    {
        var geometry = Geometry();
        return ScenarioOneCoverage.Classify(new ScenarioOneCoverage.Input
        {
            Geometry = geometry, Stems = LiveStems(), Orders = LiveOrders(), Masks = new ScenarioOneCoverage.Mask[0],
            Features = new ScenarioOneCoverage.Feature[0], PlayerManualAreas = new string[0]
        }, ScenarioOnePrescriptionCalibration.V1);
    }

    private IEnumerator Settle() { for (int i = 0; i < 12; i++) yield return null; }

    // ----------------------------------------------------------------------------------------------------------------------- the run

    private IEnumerator Run()
    {
        float deadline = Time.realtimeSinceStartup + 30f;
        do
        {
            ui = FindFirstObjectByType<ScenarioOneUiRoot>();
            manager = ui != null ? ui.Manager : null;
            ecology = FindFirstObjectByType<ForestEcologyController>();
            saves = FindFirstObjectByType<ForestSaveController>();
            marks = ui != null ? ui.Marking : null;
            if (ui != null && manager != null && ecology != null && saves != null && marks != null) break;
            yield return null;
        } while (Time.realtimeSinceStartup < deadline);
        Check(ui != null && manager != null && ecology != null && saves != null && marks != null, "scene components ready");
        for (int i = 0; i < 20; i++) yield return null;
        string config = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME");
        Check(!string.IsNullOrEmpty(config) && Application.persistentDataPath.StartsWith(config.TrimEnd('/') + "/", StringComparison.Ordinal),
            "isolated persistentDataPath", Application.persistentDataPath);
        Check(!File.Exists(SavePath), "isolated save slot starts empty");
        saveControllerWasEnabled = saves.enabled;
        original = saves.CaptureData();
        saves.enabled = false; // no F5/F9 keyboard handler during this run
        Check(manager.RackPlanningV2Active && ecology.StandGeometryModelVersion == StandGeometryModel.Enlarged80, "production Enlarged80 Rack V2 world");
        Check(ForestSaveData.CurrentVersion == 23 && manager.Prescriptions.Count == 0 && manager.NextPrescriptionId == 1, "v23 world starts with no prescription state");
        ui.CloseAll(); ui.CloseHelp();
        yield return Settle();

        // ---- 1. the sample, built through production APIs ----
        float half = StandGeometryModel.StandSizeMeters(ecology.StandGeometryModelVersion) * 0.5f;
        for (int i = 0; i < Entrances.Length; i++)
        {
            ForestTree entrance = Tree(Entrances[i]);
            Check(entrance != null && Math.Abs(entrance.transform.position.z - QualifiedZ[i]) < 1e-4f, "qualified entrance tree present", Entrances[i]);
            var added = manager.AddRack(new RackPoint(ScenarioOneHeadlandGeometry.HeadlandX, entrance.transform.position.z), new RackPoint(half, entrance.transform.position.z));
            Check(added.Succeeded, "rack added", added.Message);
        }
        Check(manager.ConfirmRackPlan().Succeeded, "rack plan confirmed");
        Check(manager.ApprovePendingWork(), "rack access approved through the Work Plan", manager.Feedback);
        Check(manager.AdvanceYear(), "rack access executed", manager.Feedback);
        yield return Settle();
        manager.MarkAnnualReviewSeen();
        foreach (string id in SampleCrops)
        {
            ForestTree crop = Tree(id);
            Check(crop != null && crop.IsLiving, "sample Crop Tree is alive after access", id);
            marks.Mark(crop, TreeMarkType.CropTree, false);
        }
        yield return Settle();
        ScenarioOneRackBay.Analysis bay = Bay();
        Check(bay != null && bay.Applies && bay.HasEnoughFuture, "three distributed sample Crop Trees qualify");
        for (int guard = 0; guard < 150; guard++)
        {
            bay = Bay();
            if (bay.MinPlannedAndExecuted >= ScenarioOneRackBay.FirstReleaseShare) break;
            var f = bay.Future.OrderBy(t => t.ReleaseShare).First();
            ForestTree next = LiveTrees().Where(t => t.IsLiving && !t.IsCropTree && !manager.IsPlannedForFelling(t.TreeId)
                    && bay.Contains(t.transform.position.x, t.transform.position.z)
                    && Vector2.Distance(new Vector2(f.X, f.Z), new Vector2(t.transform.position.x, t.transform.position.z)) <= 8)
                .OrderByDescending(t => ForestEcologyController.HegyiTerm(t.Diameter, f.DbhCm,
                    Vector2.Distance(new Vector2(f.X, f.Z), new Vector2(t.transform.position.x, t.transform.position.z))))
                .ThenBy(t => t.TreeId, StringComparer.Ordinal).First();
            marks.Mark(next, TreeMarkType.Fell, false);
            manager.AddMarkedTreesToWorkPlan();
        }
        bay = Bay();
        Check(bay.ReleasePlanned && bay.CanopyKept, "in-footprint 30% release planned within the canopy limit");
        Check(manager.ApprovePendingWork(), "sample release approved through the Work Plan", manager.Feedback);
        Check(manager.AdvanceYear(), "sample release executed", manager.Feedback);
        yield return Settle();
        manager.MarkAnnualReviewSeen();
        Check(Bay().ReleaseExecuted, "the sample's own release is executed and passed");

        // ---- 2. the player confirms Crop Trees outside the sample (production designation path) and chooses coverage ----
        ScenarioOneCoverage.Result coverage = Coverage();
        Check(coverage.Valid, "coverage valid on the live sample", coverage.Problem);
        var covered = coverage.Areas.Where(a => !a.IsSource && a.State == ScenarioOneCoverage.State.Covered).Select(a => a.Id).OrderBy(x => x, StringComparer.Ordinal).ToList();
        Check(covered.Count > 0, "at least one covered area", covered.Count + " of " + coverage.NonSourceCount);
        var sampleCrops = LiveStems().Where(s => s.Living && s.Crop).Select(s => s.Id).ToList();
        var suggestions = ScenarioOneCropTreeCandidates.Suggest(coverage, LiveStems(), sampleCrops, null, null, ScenarioOnePrescriptionCalibration.V1);
        Check(suggestions.Valid && suggestions.Candidates.Count > 0, "candidate Crop Trees suggested", suggestions.Problem);
        foreach (var candidate in suggestions.Candidates) marks.Mark(Tree(candidate.TreeId), TreeMarkType.CropTree, false);
        yield return Settle();
        Debug.Log("MFM07_NOTE confirmed " + suggestions.Candidates.Count + " candidate Crop Trees; covered units " + string.Join(",", covered));

        // ---- 3. accepting a prescription writes no mark and no order ----
        string before = Identity();
        int ordersBefore = manager.WorkOrders.Count, eventsBefore = manager.ManagementEvents.Count;
        var none = manager.CreatePrescription(new string[0]);
        Check(!none.Succeeded && Identity() == before, "accepting no unit is refused and changes nothing", none.Message);
        var bogus = manager.CreatePrescription(new[] { "Z9Z" });
        Check(!bogus.Succeeded && bogus.Refusal == Approval.Refusal.UnitUnknown && Identity() == before, "accepting an unknown unit is refused and changes nothing", bogus.Message);
        var source = coverage.Areas.First(a => a.IsSource);
        var sourceTry = manager.CreatePrescription(new[] { source.Id });
        Check(!sourceTry.Succeeded && sourceTry.Refusal == Approval.Refusal.UnitIsSource && Identity() == before, "the sample itself cannot be accepted", sourceTry.Message);
        var created = manager.CreatePrescription(covered);
        Check(created.Succeeded && created.PrescriptionId == 1, "prescription 1 accepted for every covered unit", created.Message);
        PrescriptionSaveData record = manager.FindPrescription(1);
        Check(record != null && record.approvalState == PrescriptionApprovalState.Accepted && record.approvalYear == -1
              && record.ruleVersion == ScenarioOnePrescriptionCalibration.RuleVersion && record.calibrationVersion == ScenarioOnePrescriptionCalibration.V1.Version
              && record.sampleCropTreeIds.Count == SampleCrops.Length, "the stored prescription is Accepted with rule, calibration and sample identity");
        Check(manager.WorkOrders.Count == ordersBefore && manager.ManagementEvents.Count == eventsBefore && marks.GetMarkedIds().Count == 0,
            "accepting a prescription creates no order, event or Fell mark");
        Check(manager.WorkOrders.All(o => o.prescriptionId == 0), "every existing order is still ordinary (no provenance)");

        // ---- 4. proposals are preview-only plain data ----
        string beforePreview = Identity();
        Approval.ProposalSet preview = manager.PreviewPrescription(1);
        Check(preview.Valid && preview.Links.Count > 0, "the prescription proposes removals", preview.Links.Count + " " + preview.Message);
        Check(Identity() == beforePreview, "previewing proposals changes nothing: no mark, order, event, id or cash");
        Check(preview.Links.All(l => { ForestTree t = Tree(l.TargetTreeId); return t != null && t.IsLiving && !t.IsMarkedForFell && !t.IsCropTree; }),
            "no proposed tree carries a Fell mark or Crop designation");
        Check(!manager.WorkOrders.Any(o => o.IsOpen), "no work order is open: proposals are not work");
        Check(manager.PreviewPrescription(1).Signature() == preview.Signature(), "the proposal is deterministic across calls");
        Check(preview.Safeguard.AllWithin, "the proposal is within every unit's basal-area ceiling");

        // ---- 5. the Work Plan boundary before prescription approval ----
        manager.OpenWorkPlan(true);
        yield return Settle();
        manager.OpenWorkPlan(false);
        Check(manager.WorkOrders.Count == ordersBefore && !manager.WorkOrders.Any(o => o.IsOpen), "opening the Work Plan imports no unapproved proposal");
        bool approvedEmpty = manager.ApprovePendingWork();
        Check(!approvedEmpty && manager.WorkOrders.Count == ordersBefore && !manager.WorkOrders.Any(o => o.IsOpen), "approving the Work Plan commits no unapproved proposal", manager.Feedback);
        Check(manager.PreviewPrescription(1).Signature() == preview.Signature(), "the proposal state is unchanged by opening and approving the Work Plan");

        // ---- 6. manual state wins: the player marks a proposed tree ----
        string manualId = preview.Links[0].TargetTreeId;
        marks.Mark(Tree(manualId), TreeMarkType.Fell, false);
        Approval.ProposalSet withManual = manager.PreviewPrescription(1);
        string manualUnit = preview.Links[0].CoverageUnitId;
        Check(withManual.Valid && withManual.Links.All(l => l.TargetTreeId != manualId)
              && (withManual.Conflicts.Any(c => c.TreeId == manualId && c.Kind == Approval.ConflictKind.ManualFellMark)
                  || withManual.Conflicts.Any(c => c.CoverageUnitId == manualUnit && c.Kind == Approval.ConflictKind.UnitNoLongerCovered)),
            "a manually marked tree is held back and the hold is surfaced as a conflict (the tree, or its unit when coverage excludes the unit for player management)",
            "valid=" + withManual.Valid + " marked=" + Tree(manualId).IsMarkedForFell + " inLinks=" + withManual.Links.Any(l => l.TargetTreeId == manualId)
            + " unit=" + manualUnit + " state=" + Coverage().Find(manualUnit)?.State + " conflicts=" + string.Join(",", withManual.Conflicts.Select(c => c.TreeId + "@" + c.CoverageUnitId + ":" + c.Kind)) + " " + withManual.Message);
        manager.OpenWorkPlan(true);            // ordinary behaviour: the player's own mark is imported as ordinary work
        yield return Settle();
        manager.OpenWorkPlan(false);
        ScenarioOneWorkOrder manualOrder = manager.WorkOrders.SingleOrDefault(o => o.targetTreeId == manualId && o.IsOpen);
        Check(manualOrder != null && manualOrder.prescriptionId == 0 && manualOrder.status == ScenarioWorkStatus.Pending && manager.WorkOrders.Count == ordersBefore + 1,
            "the player's own mark became exactly one ordinary pending order and nothing else was imported");
        int manualOrderId = manualOrder.workOrderId;
        Approval.ProposalSet withOrder = manager.PreviewPrescription(1);
        Check(withOrder.Links.All(l => l.TargetTreeId != manualId) && withOrder.Conflicts.Any(c => (c.TreeId == manualId && c.Kind == Approval.ConflictKind.OpenWorkOrder)
                || (c.CoverageUnitId == manualUnit && c.Kind == Approval.ConflictKind.UnitNoLongerCovered)),
            "a tree with an open order is held back and the hold is surfaced as a conflict");

        // ---- 7. the mandatory link check fails closed, and a refusal changes nothing ----
        Approval.OrderLink good = withOrder.Links[0];
        Check(manager.CheckPrescriptionLink(1, good.TargetTreeId, good.CoverageUnitId, good.CropTreeId, good.ReasonCode).Ok, "a link the prescription proposes validates");
        string probeIdentity = Identity();
        void Refused(string what, Approval.LinkCheck got, Approval.Refusal want)
        {
            Check(!got.Ok && got.Refusal == want && got.Message.Length > 0 && got.Message.Length <= 220, what, got.Refusal + " " + got.Message);
        }
        Refused("a missing prescription is refused", manager.CheckPrescriptionLink(99, good.TargetTreeId, good.CoverageUnitId, good.CropTreeId, good.ReasonCode), Approval.Refusal.NoPrescription);
        Refused("an unknown coverage unit is refused", manager.CheckPrescriptionLink(1, good.TargetTreeId, "Z9Z", good.CropTreeId, good.ReasonCode), Approval.Refusal.UnitUnknown);
        Refused("the sample unit is refused", manager.CheckPrescriptionLink(1, good.TargetTreeId, source.Id, good.CropTreeId, good.ReasonCode), Approval.Refusal.UnitIsSource);
        Refused("an order with no Crop Tree is refused", manager.CheckPrescriptionLink(1, good.TargetTreeId, good.CoverageUnitId, "", good.ReasonCode), Approval.Refusal.CropTreeMissing);
        Refused("a sample Crop Tree is not a valid Crop Tree for an order", manager.CheckPrescriptionLink(1, good.TargetTreeId, good.CoverageUnitId, SampleCrops[0], good.ReasonCode), Approval.Refusal.CropTreeNotConfirmed);
        Refused("an unconfirmed tree is not a valid Crop Tree", manager.CheckPrescriptionLink(1, good.TargetTreeId, good.CoverageUnitId, good.TargetTreeId, good.ReasonCode), Approval.Refusal.CropTreeNotConfirmed);
        Refused("a tree the player already ordered is refused", manager.CheckPrescriptionLink(1, manualId, preview.Links[0].CoverageUnitId, preview.Links[0].CropTreeId, good.ReasonCode), Approval.Refusal.TargetHasManualState);
        Refused("an unknown reason code is refused", manager.CheckPrescriptionLink(1, good.TargetTreeId, good.CoverageUnitId, good.CropTreeId, 0), Approval.Refusal.ReasonUnknown);
        Refused("an unknown tree is refused", manager.CheckPrescriptionLink(1, "P-none", good.CoverageUnitId, good.CropTreeId, good.ReasonCode), Approval.Refusal.TargetMissing);
        Check(Identity() == probeIdentity, "link checks are read-only");

        // Stale Crop Tree: the player un-designates the Crop Tree an order serves, after the preview.
        ForestTree servedCrop = Tree(good.CropTreeId);
        marks.Unmark(servedCrop, false);
        Refused("a Crop Tree un-designated after the preview is refused", manager.CheckPrescriptionLink(1, good.TargetTreeId, good.CoverageUnitId, good.CropTreeId, good.ReasonCode), Approval.Refusal.CropTreeNotConfirmed);
        marks.Mark(servedCrop, TreeMarkType.CropTree, false);
        Check(manager.CheckPrescriptionLink(1, good.TargetTreeId, good.CoverageUnitId, good.CropTreeId, good.ReasonCode).Ok, "the Crop Tree designation restored makes the link valid again");

        // A corrupted stored prescription is refused whole and leaves the world and the record exactly as it was.
        string preCorruption = Identity();
        string recordJson = JsonUtility.ToJson(record);
        void Corrupt(string what, Action<PrescriptionSaveData> damage, Approval.Refusal want)
        {
            damage(record);
            PrescriptionOperationResult result = manager.ApprovePrescription(1);
            JsonUtility.FromJsonOverwrite(recordJson, record);
            Check(!result.Succeeded && result.Refusal == want && result.OrdersCreated == 0 && result.Message.Length > 0 && result.Message.Length <= 220,
                "approval refused whole: " + what, result.Refusal + " " + result.Message);
            Check(Identity() == preCorruption && manager.WorkOrders.All(o => o.prescriptionId == 0), "no partial order, event, id or mark after: " + what);
        }
        Corrupt("rule version is not the current rule", r => r.ruleVersion = "MFM-V0-RULE-0", Approval.Refusal.RuleVersionMismatch);
        Corrupt("calibration is not the current calibration", r => r.calibrationVersion = "MFM03-CAL-0", Approval.Refusal.CalibrationMismatch);
        Corrupt("source rack is not the sample", r => r.sourceRackId = 1, Approval.Refusal.SampleChanged);
        Corrupt("sample Crop Trees no longer match", r => r.sampleCropTreeIds.Add("P-none"), Approval.Refusal.SampleChanged);
        Corrupt("no unit is accepted any more", r => { foreach (var d in r.coverageDecisions) d.choice = PrescriptionCoverageChoice.LeftManual; }, Approval.Refusal.NothingToApprove);
        Corrupt("already approved", r => { r.approvalState = PrescriptionApprovalState.Approved; r.approvalYear = ecology.EcologicalYear; }, Approval.Refusal.NotAccepted);
        Check(manager.FindPrescription(1).approvalState == PrescriptionApprovalState.Accepted, "the stored prescription is intact and still Accepted after every refusal");
        Check(manager.ApprovePrescription(7).Refusal == Approval.Refusal.NoPrescription, "approving an unknown prescription is refused");

        // ---- 8. prescription approval: pending ordinary work only ----
        Approval.ProposalSet expected = manager.PreviewPrescription(1);
        int expectedCount = expected.Links.Count;
        long cashBefore = manager.CashCents;
        int yearBefore = ecology.EcologicalYear;
        var openBefore = manager.WorkOrders.Where(o => o.IsOpen).Select(o => o.workOrderId).ToList();
        string markedBefore = string.Join(",", marks.GetMarkedIds().OrderBy(x => x, StringComparer.Ordinal));
        PrescriptionOperationResult approved = manager.ApprovePrescription(1);
        Check(approved.Succeeded && approved.OrdersCreated == expectedCount && expectedCount > 0, "prescription approval created exactly the previewed orders", approved.OrdersCreated + "/" + expectedCount + " " + approved.Message);
        var generated = manager.WorkOrders.Where(o => o.prescriptionId == 1).ToList();
        Check(generated.Count == expectedCount && generated.All(o => o.type == ScenarioWorkType.FellTree && o.status == ScenarioWorkStatus.Pending),
            "every generated order is an ordinary PENDING felling order (not approved, not completed)");
        Check(generated.Select(o => o.targetTreeId).OrderBy(x => x, StringComparer.Ordinal).SequenceEqual(expected.Links.Select(l => l.TargetTreeId)),
            "the generated orders fell exactly the previewed trees");
        Check(generated.All(o => !string.IsNullOrEmpty(o.prescriptionCoverageUnitId) && !string.IsNullOrEmpty(o.prescriptionCropTreeId)
              && o.prescriptionReason == PrescriptionReasonCode.CompetitorHighShareAroundConfirmedCropTree && o.createdYear == yearBefore && o.rackId == 0
              && string.IsNullOrEmpty(o.validationMessage)), "every generated order carries prescription, coverage unit, Crop Tree and closed reason");
        Check(generated.All(o => { var l = expected.Links.First(x => x.TargetTreeId == o.targetTreeId); return l.CoverageUnitId == o.prescriptionCoverageUnitId && l.CropTreeId == o.prescriptionCropTreeId; }),
            "each order's provenance equals the previewed link");
        Check(generated.All(o => LiveTrees().First(t => t.TreeId == o.targetTreeId).IsLiving), "no tree was felled: approval does not execute");
        Check(manager.CashCents == cashBefore && ecology.EcologicalYear == yearBefore, "no cash was spent and no year advanced");
        Check(manager.WorkOrders.Where(o => o.IsOpen && o.prescriptionId == 0).Select(o => o.workOrderId).OrderBy(x => x).SequenceEqual(openBefore.OrderBy(x => x)),
            "the player's own order is untouched", "order #" + manualOrderId);
        Check(string.Join(",", marks.GetMarkedIds().OrderBy(x => x, StringComparer.Ordinal)) == markedBefore, "no Fell mark was created or removed by approval");
        var createdEvents = manager.ManagementEvents.Where(e => e.prescriptionId == 1).ToList();
        Check(createdEvents.Count == expectedCount && createdEvents.All(e => e.eventType == ScenarioManagementEventType.OrderCreated), "history records one OrderCreated event per order, linked to the prescription");
        PrescriptionSaveData stored = manager.FindPrescription(1);
        Check(stored.approvalState == PrescriptionApprovalState.Approved && stored.approvalYear == yearBefore, "the prescription is recorded Approved in the current year");
        Check(manager.PreviewPrescription(1).Valid == false && manager.PreviewPrescription(1).Refusal == Approval.Refusal.NotAccepted, "an approved prescription has no live proposals left");
        string afterApproval = Identity();
        PrescriptionOperationResult again = manager.ApprovePrescription(1);
        Check(!again.Succeeded && again.Refusal == Approval.Refusal.NotAccepted && Identity() == afterApproval, "approving twice is refused and adds nothing");

        // ---- 9. the Work Plan is still the commitment gate ----
        Check(generated.All(o => o.status == ScenarioWorkStatus.Pending) && !manager.WorkOrders.Any(o => o.prescriptionId == 1 && o.status != ScenarioWorkStatus.Pending), "before the Work Plan approval every generated order is still pending");
        Check(manager.ApprovePendingWork(), "ordinary Work Plan approval still required, and it succeeds", manager.Feedback);
        Check(generated.Count(o => o.status == ScenarioWorkStatus.Approved) == expectedCount, "Work Plan approval approved the generated orders through the ordinary path");
        Check(manager.WorkOrders.Single(o => o.workOrderId == manualOrderId).status == ScenarioWorkStatus.Approved, "and the player's own order alongside them");
        Check(generated.All(o => LiveTrees().First(t => t.TreeId == o.targetTreeId).IsLiving), "approved is not executed: every tree still stands");

        // ---- 10. v23 save / load keeps the record, the provenance and the world ----
        string saveBefore = Identity();
        string hashBefore = ScenarioReferenceArchive.WorldHash(saves.CaptureData());
        saves.Save();
        Check(File.Exists(SavePath), "Save() wrote the isolated slot");
        string disk = File.ReadAllText(SavePath);
        Check(disk.Contains("\"prescriptionId\": 1") || disk.Contains("\"prescriptionId\":1"), "the saved file carries the prescription provenance");
        Check(saves.CanLoad(out string loadProblem), "CanLoad accepts the saved v23 file", loadProblem);
        saves.Load();
        yield return Settle();
        yield return Settle();
        Check(Identity() == saveBefore && ScenarioReferenceArchive.WorldHash(saves.CaptureData()) == hashBefore, "save/load round trip preserves the whole world and its hash");
        var reloaded = manager.WorkOrders.Where(o => o.prescriptionId == 1).ToList();
        Check(reloaded.Count == expectedCount && reloaded.All(o => o.status == ScenarioWorkStatus.Approved && o.prescriptionReason == 1
              && !string.IsNullOrEmpty(o.prescriptionCoverageUnitId) && !string.IsNullOrEmpty(o.prescriptionCropTreeId)), "generated orders reload with their status and provenance");
        PrescriptionSaveData back = manager.FindPrescription(1);
        Check(back != null && back.approvalState == PrescriptionApprovalState.Approved && back.approvalYear == yearBefore && manager.NextPrescriptionId == 2, "the Approved prescription and its id allocator reload");
        Check(manager.ManagementEvents.Count(e => e.prescriptionId == 1) == expectedCount, "history linkage reloads");
        Check(!manager.WorkOrders.Any(o => o.IsOpen && o.status == ScenarioWorkStatus.Pending), "reloading creates no new pending work and no proposal");
        Check(manager.PreviewPrescription(1).Refusal == Approval.Refusal.NotAccepted, "no proposal reappears after reload");
        File.Delete(SavePath);

        // ---- 11. ordinary execution is unchanged: the approved orders resolve by the existing economy rules ----
        bool advanced = manager.AdvanceYear();
        yield return Settle();
        Debug.Log("MFM07_NOTE ordinary AdvanceYear after Work Plan approval: " + advanced + " " + manager.Feedback);
        var resolved = manager.WorkOrders.Where(o => o.prescriptionId == 1).ToList();
        Check(advanced && resolved.All(o => o.status == ScenarioWorkStatus.Completed || o.status == ScenarioWorkStatus.Failed),
            "the approved prescription orders were resolved by the ordinary annual execution", string.Join(",", resolved.GroupBy(o => o.status).Select(g => g.Key + "=" + g.Count())));
        Check(manager.WorkOrders.Where(o => o.prescriptionId == 1).All(o => o.prescriptionId == 1 && o.prescriptionReason == 1), "provenance survives execution");
        ForestSaveData executed = saves.CaptureData();
        Check(ForestSaveValidation.ValidatePrescriptions(executed) == null && saves.LoadData(executed, false), "the executed world still validates and reloads as v23");
    }
}
#endif
