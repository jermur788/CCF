#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Disposable Editor-only gate: stage with the runner, then remove source and .meta.
// This is a live-stem/order adapter on a confirmed in-memory Rack V2 fixture, not the future production MFM adapter.
// It never calls disk Save/Load, approves/executes work, or renders an MFM UI.
public static class MfmUnityCompileCheck
{
    public static void Begin()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        EditorApplication.isPlaying = true;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (Environment.GetCommandLineArgs().Any(a => a == "MfmUnityCompileCheck.Begin"))
            new GameObject("Disposable MFM compile and non-mutation gate").AddComponent<MfmUnityCompileCheckRunner>();
    }
}

public sealed class MfmUnityCompileCheckRunner : MonoBehaviour
{
    private const string CalibrationSignature =
        "rule=MFM-V1-RULE-1 cal=MFM03-CAL-1 segment=12 density=0.2 dbh=0.15 ci=0.2 tailGuard=none minStems=30 boundary=[0.65,1.5] candidateSpacing=16 areaStep=0.1 secondSample=NotRequired:1";
    private static readonly string[] Entrances = { "PO0203", "PO1204", "PO2003", "PO2803", "PO3803" };
    private static readonly float[] QualifiedZ = { -35.2711754f, -16.4217644f, 0.2382353f, 15.7935295f, 35.2323532f };
    private ScenarioOneUiRoot ui;
    private ScenarioOneManager manager;
    private ForestEcologyController ecology;
    private ForestSaveController saves;
    private ForestSaveData original;
    private World originalIdentity;
    private bool saveControllerWasEnabled;
    private int checks;

    private void Check(bool ok, string what, string detail = "")
    {
        if (!ok) throw new InvalidOperationException(what + " " + detail);
        checks++;
        Debug.Log("MFM_UNITY_CHECK " + what + (detail.Length == 0 ? "" : " [" + detail + "]"));
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
        // Restore even after a setup/check exception. No disk save is loaded or written.
        if (original != null)
        {
            try
            {
                Check(saves.LoadData(original, false), "original world restored in memory");
                AssertUnchanged(originalIdentity, "fixture restoration");
            }
            catch (Exception error) { failure = failure == null ? error : new AggregateException(failure, error); }
            saves.enabled = saveControllerWasEnabled;
        }
        Debug.Log(failure == null ? "MFM_UNITY_PASS checks=" + checks : "MFM_UNITY_FAIL checks=" + checks + " " + failure);
        EditorApplication.Exit(failure == null ? 0 : 1);
    }

    private IEnumerator Run()
    {
        float deadline = Time.realtimeSinceStartup + 30f;
        do
        {
            ui = FindFirstObjectByType<ScenarioOneUiRoot>();
            manager = ui != null ? ui.Manager : null;
            ecology = FindFirstObjectByType<ForestEcologyController>();
            saves = FindFirstObjectByType<ForestSaveController>();
            if (ui != null && manager != null && ecology != null && saves != null) break;
            yield return null;
        } while (Time.realtimeSinceStartup < deadline);
        Check(ui != null && manager != null && ecology != null && saves != null, "scene components ready");
        // Let scene Start methods settle before creating any fixture. No yields occur during the measured calls.
        for (int i = 0; i < 20; i++) yield return null;
        string config = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME");
        Check(!string.IsNullOrEmpty(config) && Application.persistentDataPath.StartsWith(config.TrimEnd('/') + "/", StringComparison.Ordinal),
            "isolated persistentDataPath", Application.persistentDataPath);
        saveControllerWasEnabled = saves.enabled;
        original = saves.CaptureData();
        originalIdentity = Capture();
        saves.enabled = false; // Prevent the F5/F9 keyboard handler during this disposable run.
        Check(manager.RackPlanningV2Active && ecology.StandGeometryModelVersion == StandGeometryModel.Enlarged80,
            "production Enlarged80 Rack V2 world");
        float half = StandGeometryModel.StandSizeMeters(ecology.StandGeometryModelVersion) * 0.5f;
        if (!manager.RackPlanConfirmed)
        {
            Check(manager.PlannedRackCount == 0, "fixture starts with an empty rack plan");
            for (int i = 0; i < Entrances.Length; i++)
            {
                ForestTree tree = LiveTrees().SingleOrDefault(t => t.TreeId == Entrances[i] && t.IsLiving);
                Check(tree != null, "live fixture entrance exists", Entrances[i]);
                float z = tree.transform.position.z;
                Check(Math.Abs(z - QualifiedZ[i]) < 1e-4f, "live entrance matches qualified Rack V2 row", Entrances[i]);
                var added = manager.AddRack(new RackPoint(ScenarioOneHeadlandGeometry.HeadlandX, z), new RackPoint(half, z));
                Check(added.Succeeded && added.RacksAdded == 1, "fixture rack added in memory", added.Message);
            }
            var confirmed = manager.ConfirmRackPlan();
            Check(confirmed.Succeeded, "fixture plan confirmed in memory (pending access orders only)", confirmed.Message);
            Debug.Log("MFM_UNITY_NOTE INPUT=FIXTURE: live entrance-derived plan confirmed through Rack V2 APIs before baseline; no lesson work executed");
        }
        else Debug.Log("MFM_UNITY_NOTE INPUT=LIVE: existing confirmed Rack V2 geometry");
        Check(manager.RackPlanConfirmed && manager.ConfirmedRacks.Count == 5, "five confirmed racks available");
        World baseline = Capture();
        var calibration = ScenarioOnePrescriptionCalibration.V1;
        Check(calibration.Version == "MFM03-CAL-1" && ScenarioOnePrescriptionCalibration.RuleVersion == "MFM-V1-RULE-1"
            && calibration.Signature() == CalibrationSignature, "exact reviewed calibration signature", calibration.Signature());
        var geometry = ReadOnly("RackSampleGeometry.Derive", () => ScenarioOneRackSampleGeometry.Derive(manager.ConfirmedRacks, half));
        Check(geometry.Valid, "confirmed geometry valid", geometry.Problem);
        // Read through the existing live Rack Bay adapter too, rather than inventing a separate geometry convention.
        var bay = ReadOnly("RackBayReader.Read", () => ScenarioOneRackBayReader.Read(LiveTrees(), manager.WorkOrders, geometry));
        Check(bay != null && bay.Applies && bay.TeachingRackId == geometry.TeachingRack.Id, "Rack Bay agrees with confirmed geometry");
        var stems = LiveStems();
        Check(stems.Any(s => s.Living), "live stems available", stems.Count.ToString());
        var input = new ScenarioOneCoverage.Input
        {
            Geometry = geometry, Stems = stems, Orders = LiveOrders(),
            Masks = new ScenarioOneCoverage.Mask[0], Features = new ScenarioOneCoverage.Feature[0], PlayerManualAreas = new string[0]
        };
        Debug.Log("MFM_UNITY_NOTE ADAPTER=PARTIAL: live stems/marks/orders and confirmed geometry; masks/features/manual choices omitted; not post-Lesson-5 qualification");
        var coverage = ReadOnly("Coverage.Classify", () => ScenarioOneCoverage.Classify(input, calibration));
        Check(coverage != null && coverage.Valid, "coverage valid on labelled live Rack fixture", coverage?.Problem ?? "null");
        Debug.Log("MFM_UNITY_NOTE covered=" + coverage.CoveredCount + "/" + coverage.NonSourceCount + " source_stems=" + coverage.Source.Stems);
        var repeated = ReadOnly("Coverage.Classify repeat", () => ScenarioOneCoverage.Classify(input, calibration));
        Check(repeated.Signature() == coverage.Signature(), "identical input gives identical coverage signature");
        var empty = new string[0];
        var sampleCrops = stems.Where(s => s.Living && s.Crop && geometry.Teaching.Contains(s.X, s.Z)).Select(s => s.Id).ToArray();
        var suggestions = ReadOnly("CropTreeCandidates.Suggest", () => ScenarioOneCropTreeCandidates.Suggest(coverage, stems, sampleCrops, empty, empty, calibration));
        Check(suggestions.Valid && suggestions.Candidates.Count > 0, "valid coverage yields inspection candidates", suggestions.Problem);
        Check(suggestions.RuleVersion == "MFM-V1-RULE-1" && suggestions.CalibrationVersion == "MFM03-CAL-1", "suggestions carry reviewed versions");
        foreach (var candidate in suggestions.Candidates)
        {
            Check(candidate.Decision == ScenarioOneCropTreeCandidates.Decision.Open
                && candidate.Reason == ScenarioOneCropTreeCandidates.CandidateReason.SpacingOnlyInspectToJudge,
                "candidate is open and inspection-only", candidate.TreeId);
            ScenarioOneCropTreeCandidates.InspectionTarget target = default;
            bool found = ReadOnly("TryGetInspectionTarget " + candidate.TreeId,
                () => ScenarioOneCropTreeCandidates.TryGetInspectionTarget(suggestions, candidate.TreeId, out target));
            Check(found && target.TreeId == candidate.TreeId && target.AreaId == candidate.AreaId
                && target.X == candidate.X && target.Z == candidate.Z && target.Order == candidate.Order
                && target.Reason == ScenarioOneCropTreeCandidates.CandidateReason.SpacingOnlyInspectToJudge,
                "inspection target matches candidate", candidate.TreeId);
            ForestTree tree = LiveTrees().SingleOrDefault(t => t.TreeId == candidate.TreeId);
            Check(tree != null && !tree.IsCropTree && !tree.IsMarkedForFell, "suggestion did not designate or mark its tree", candidate.TreeId);
        }
        ScenarioOneCropTreeCandidates.InspectionTarget missing = default;
        Check(!ReadOnly("TryGetInspectionTarget unknown", () => ScenarioOneCropTreeCandidates.TryGetInspectionTarget(suggestions, "NO-SUCH-TREE", out missing)), "unknown inspection id fails safely");
        foreach (var direction in new[] { ScenarioOneCropTreeCandidates.Navigation.Next, ScenarioOneCropTreeCandidates.Navigation.Previous })
        {
            var expected = direction == ScenarioOneCropTreeCandidates.Navigation.Next
                ? suggestions.Candidates.Select(c => c.TreeId).ToList() : suggestions.Candidates.Select(c => c.TreeId).Reverse().ToList();
            string current = "";
            foreach (string id in expected)
            {
                ScenarioOneCropTreeCandidates.InspectionTarget target = default;
                Check(ReadOnly("TryMoveInspectionTarget " + direction, () => ScenarioOneCropTreeCandidates.TryMoveInspectionTarget(suggestions, current, direction, out target))
                    && target.TreeId == id, "navigation visits candidate in order", id);
                current = target.TreeId;
            }
            Check(!ReadOnly("TryMoveInspectionTarget end", () => ScenarioOneCropTreeCandidates.TryMoveInspectionTarget(suggestions, current, direction, out missing)), "navigation does not wrap");
        }
        Check(!ReadOnly("TryMoveInspectionTarget unknown", () => ScenarioOneCropTreeCandidates.TryMoveInspectionTarget(suggestions, "NO-SUCH-TREE", ScenarioOneCropTreeCandidates.Navigation.Next, out missing)), "unknown navigation id fails safely");
        var proposal = new[] { stems.First(s => s.Living).Id };
        var closed = ReadOnly("EvaluateSafeguard null", () => ScenarioOnePrescription.EvaluateSafeguard(null, stems, proposal, 0.4));
        Check(!closed.AllWithin && closed.InvalidInput, "null coverage fails closed for a nonempty proposal", closed.InvalidReason);
        var none = ReadOnly("EvaluateSafeguard valid empty", () => ScenarioOnePrescription.EvaluateSafeguard(coverage, stems, empty, 0.4));
        Check(!none.InvalidInput && none.AllWithin && none.Areas.Count == coverage.CoveredCount, "valid empty proposal stays within all covered areas");
        var covered = coverage.Areas.FirstOrDefault(a => a.State == ScenarioOneCoverage.State.Covered && a.StemIds.Count > 0);
        Check(covered != null, "at least one covered area has a live target stem");
        var one = ReadOnly("EvaluateSafeguard valid covered stem", () => ScenarioOnePrescription.EvaluateSafeguard(coverage, stems, new[] { covered.StemIds[0] }, 0.4));
        Check(!one.InvalidInput && one.OutsideCoveredAreas.Count == 0 && one.Areas.Any(a => a.ProposedBasalAreaM2 > 0), "covered removal accounted in basal-area report");
        var outside = ReadOnly("EvaluateSafeguard unknown stem", () => ScenarioOnePrescription.EvaluateSafeguard(coverage, stems, new[] { "NO-SUCH-TREE" }, 0.4));
        Check(!outside.AllWithin && outside.OutsideCoveredAreas.Count == 1, "unknown proposal target is outside coverage");
        var invalid = ReadOnly("Coverage.Classify null", () => ScenarioOneCoverage.Classify(null, calibration));
        var invalidReport = ReadOnly("EvaluateSafeguard invalid coverage", () => ScenarioOnePrescription.EvaluateSafeguard(invalid, stems, proposal, 0.4));
        Check(!invalid.Valid && invalidReport.InvalidInput && !invalidReport.AllWithin, "invalid coverage object fails closed");
        AssertUnchanged(baseline, "all MFM checks since confirmed-fixture baseline");
    }

    // The finally check also detects a mutation when the called method throws; either condition fails the gate.
    private T ReadOnly<T>(string name, Func<T> call)
    {
        World before = Capture();
        try { return call(); }
        finally { AssertUnchanged(before, name); }
    }

    private static ForestTree[] LiveTrees() => FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

    private static List<ScenarioOneCoverage.Stem> LiveStems()
    {
        var result = new List<ScenarioOneCoverage.Stem>();
        foreach (ForestTree tree in LiveTrees())
        {
            if (tree == null || string.IsNullOrEmpty(tree.TreeId) || (!tree.IsLiving && !tree.IsStump)) continue;
            Vector3 p = tree.transform.position;
            result.Add(new ScenarioOneCoverage.Stem(tree.TreeId, p.x, p.z, tree.Diameter, tree.IsLiving,
                tree.Species != null ? tree.Species.SpeciesId : "", tree.IsCropTree, tree.IsMarkedForFell));
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

    private struct World { public string Hash, ScenarioJson, SaveJson, Marks, Orders; public int Trees, WorkOrders; }

    private World Capture()
    {
        ForestSaveData data = saves.CaptureData();
        var trees = FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        string marks = string.Join(",", trees.Where(t => t != null).OrderBy(t => t.TreeId, StringComparer.Ordinal)
            .Select(t => t.TreeId + ":crop=" + t.IsCropTree + ":fell=" + t.IsMarkedForFell));
        string orders = string.Join("\n", manager.WorkOrders.Select(o => o == null ? "null" : JsonUtility.ToJson(o)));
        return new World
        {
            Hash = ScenarioReferenceArchive.WorldHash(data), ScenarioJson = JsonUtility.ToJson(data.scenarioOne), SaveJson = JsonUtility.ToJson(data),
            Marks = marks, Orders = orders, Trees = trees.Length, WorkOrders = manager.WorkOrders.Count
        };
    }

    private void AssertUnchanged(World before, string name)
    {
        World after = Capture();
        Check(!string.IsNullOrEmpty(after.Hash) && after.Hash == before.Hash, name + ": WorldHash unchanged", after.Hash);
        Check(after.ScenarioJson == before.ScenarioJson, name + ": scenario JSON unchanged");
        Check(after.SaveJson == before.SaveJson, name + ": complete captured save unchanged");
        Check(after.Marks == before.Marks && after.Trees == before.Trees, name + ": tree marks/count unchanged", after.Trees.ToString());
        Check(after.Orders == before.Orders && after.WorkOrders == before.WorkOrders, name + ": orders/approval/count unchanged", after.WorkOrders.ToString());
    }
}
#endif
