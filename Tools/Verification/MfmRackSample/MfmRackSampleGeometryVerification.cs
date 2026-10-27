#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

// Disposable Tier-2 gate for MFM-02: the rack-served teaching sample and coverage geometry (ScenarioOneRackSampleGeometry) as the live
// game uses it. Lessons 2-5 are driven through the PRODUCTION fact path (LearningObjectivesView.TeachingFacts), so what is asserted is what
// the player's Learning panel sees. Select the mode with CCF_MFM02_MODE:
//
//   v2        a fresh Enlarged80 Rack Planning V2 world. The five racks of the qualified Rack V2 headland fixture are planned (the same
//             entrance trees as RACK-V2-COMBINED: PO0203 PO1204 PO2003 PO2803 PO3803; headland x -35.4 to the east edge), confirmed, and
//             then: draft/reopened plans have NO teaching area; the third rack is selected; anchor, +-12 m footprint, 9.0 m half-width
//             and actual corridor exclusion are compared with an independent re-computation for EVERY living tree; the selection survives
//             a JSON save/load round trip; the same geometry planned in a different id order selects the same physical rack; then the
//             real Lessons 2-5 chain runs (Crop Trees at 8 m, 30% release, 40% safeguard, deadwood/sold, Work Plan, Annual Review,
//             J/K return review, reload).
//   model0    the historical model-0 Enlarged80 world (a fresh game is now V2, so it is regenerated with the rack plan cleared). The same
//             Lessons 2-5 chain on the authored middle line: the footprint must still be the accepted 69 trees / 8.00385 m half-width.
//   legacy40  a fresh Legacy40 game (CCF_STAND_GEOMETRY=0): no authored layout, so there is no teaching area, as before.
//
// Copy into Assets/ForestPrototype, run MfmRackSampleGeometryVerification.Begin, then remove the copy and its .meta. Queued InputSystem events
// are synthetic. Save safety: refuses to run unless Application.persistentDataPath is inside XDG_CONFIG_HOME (the runner's isolated config);
// the gate only saves/loads in memory and never writes a save file.
public static class MfmRackSampleGeometryVerification
{
    public static void Begin()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        EditorApplication.isPlaying = true;
    }

    // Lessons are derived from forest state; start every process with none recorded.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Prepare()
    {
        if (!Environment.GetCommandLineArgs().Any(a => a == "MfmRackSampleGeometryVerification.Begin")) return;
        foreach (string id in LearningObjectivesView.StepIds) PlayerPrefs.DeleteKey(LearningObjectivesView.PreferencePrefix + id);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (Environment.GetCommandLineArgs().Any(a => a == "MfmRackSampleGeometryVerification.Begin"))
            new GameObject("Disposable MFM-02 rack sample geometry verification").AddComponent<MfmRackSampleGeometryVerificationRunner>();
    }
}

public sealed class MfmRackSampleGeometryVerificationRunner : MonoBehaviour
{
    // The five entrance trees RACK-V2-02A/02B-R qualified on the headland and the persisted rack z values they gave (rv2c-state.json).
    private static readonly string[] QualifiedEntrances = { "PO0203", "PO1204", "PO2003", "PO2803", "PO3803" };
    private static readonly double[] QualifiedZ = { -35.2711754, -16.4217644, 0.2382353, 15.7935295, 35.2323532 };

    private ScenarioOneUiRoot ui;
    private ScenarioOneManager manager;
    private ForestPlayer player;
    private ForestTreeMarkingManager marks;
    private ForestSaveController saves;
    private ForestEcologyController ecology;
    private RackPlanner planner;
    private string mode, output;
    private int checks;
    private readonly StringBuilder measurements = new StringBuilder();

    private void Check(bool ok, string what, string detail = "")
    {
        if (!ok) throw new InvalidOperationException(what + (detail.Length > 0 ? " [" + detail + "]" : ""));
        checks++;
        Debug.Log("MFM02_CHECK " + what + (detail.Length > 0 ? " [" + detail + "]" : ""));
    }

    private void Note(string name, string detail)
    {
        Debug.Log("MFM02_MEASURE " + name + " " + detail);
        measurements.AppendLine(name + " " + detail);
    }

    private IEnumerator Start()
    {
        mode = Environment.GetEnvironmentVariable("CCF_MFM02_MODE") ?? "";
        for (int i = 0; i < 8; i++) yield return null;
        Exception failure = null;
        var stack = new Stack<IEnumerator>();
        stack.Push(Run());
        while (stack.Count > 0)
        {
            bool more; object current = null;
            try { more = stack.Peek().MoveNext(); if (more) current = stack.Peek().Current; }
            catch (Exception error) { failure = error; break; }
            if (!more) { stack.Pop(); continue; }
            if (current is IEnumerator nested) stack.Push(nested); else yield return current;
        }
        string tag = "MFM02_" + mode.ToUpperInvariant();
        try
        {
            if (!string.IsNullOrEmpty(output))
                File.WriteAllText(Path.Combine(output, "mfm02-" + mode + "-measurements.txt"), measurements.ToString());
        }
        catch (Exception error) { Debug.Log("MFM02_NOTE could not write measurements: " + error.Message); }
        Debug.Log(failure == null ? tag + "_PASS checks=" + checks : tag + "_FAIL checks=" + checks + " " + failure);
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(failure == null ? 0 : 1);
    }

    private IEnumerator Run()
    {
        ui = FindFirstObjectByType<ScenarioOneUiRoot>(); manager = ui != null ? ui.Manager : null; player = ui != null ? ui.Player : null;
        marks = ui != null ? ui.Marking : null; saves = FindFirstObjectByType<ForestSaveController>(); ecology = FindFirstObjectByType<ForestEcologyController>();
        Check(manager != null && player != null && marks != null && saves != null && ecology != null, "scene has the live lesson dependencies");
        string config = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME");
        Check(!string.IsNullOrEmpty(config) && Application.persistentDataPath.StartsWith(config, StringComparison.Ordinal),
            "persistentDataPath is inside the isolated XDG_CONFIG_HOME", Application.persistentDataPath);
        string diag = Environment.GetEnvironmentVariable("CCF_ACCEPTANCE_OUTPUT");
        output = string.IsNullOrEmpty(diag) ? Application.persistentDataPath : diag;
        Directory.CreateDirectory(output);
        ui.CloseAll(); ui.CloseHelp(); yield return Frames(6);
        if (mode == "v2") yield return V2();
        else if (mode == "model0") yield return Model0();
        else if (mode == "legacy40") yield return Legacy40();
        else throw new InvalidOperationException("CCF_MFM02_MODE must be v2, model0 or legacy40 (was '" + mode + "')");
    }

    // ---- helpers (the patterns RackLessonVerification established) ------------------------------------------------------------
    private IEnumerator Frames(int n) { for (int i = 0; i < n; i++) yield return null; }
    private IEnumerator Settled() { yield return new WaitForSecondsRealtime(0.35f); }
    private static string F(double v) => v.ToString("0.#####", CultureInfo.InvariantCulture);
    private static string R(double v) => v.ToString("R", CultureInfo.InvariantCulture);

    private IEnumerator Press(Key key)
    {
        Keyboard keyboard = Keyboard.current ?? InputSystem.AddDevice<Keyboard>();
        InputSystem.QueueStateEvent(keyboard, new KeyboardState(key)); yield return null; yield return null;
        InputSystem.QueueStateEvent(keyboard, new KeyboardState()); yield return null; yield return null;
    }

    private static ForestTree[] AllLiving() => FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
        .Where(t => t.IsLiving).OrderBy(t => t.TreeId, StringComparer.Ordinal).ToArray();

    private static ForestTree[] Trees() => AllLiving().Where(t => t.Species != null && t.Species.SpeciesId == "sitka-spruce").ToArray();

    // The cached facts the Learning panel itself uses (refreshed at most every quarter of a second).
    private ScenarioOneTeachingSequence.Facts LiveFacts()
        => (ScenarioOneTeachingSequence.Facts)typeof(LearningObjectivesView).GetMethod("TeachingFacts", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(ui.Learning, null);

    // The same production method, forced to recompute now (the analysis is only as fresh as the last refresh otherwise).
    private ScenarioOneTeachingSequence.Facts FreshFacts()
    {
        typeof(LearningObjectivesView).GetField("sequenceFacts", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(ui.Learning, null);
        return LiveFacts();
    }

    private ScenarioOneRackBay.Analysis Bay() => FreshFacts().Bay;

    private static readonly Vector3[] InspectionDirections =
    {
        Vector3.back, Vector3.forward, Vector3.right, Vector3.left,
        new Vector3(-1f, 0f, -1f).normalized, new Vector3(1f, 0f, -1f).normalized,
        new Vector3(-1f, 0f, 1f).normalized, new Vector3(1f, 0f, 1f).normalized
    };
    private static readonly float[] InspectionDistances = { 3f, 2.5f, 2f };
    private readonly Collider[] inspectionOverlaps = new Collider[16];

    private bool OverlapsOtherTree(Vector3 standPoint, ForestTree target)
    {
        int count = Physics.OverlapSphereNonAlloc(standPoint + Vector3.up * 0.9f, 0.35f, inspectionOverlaps);
        for (int i = 0; i < count; i++)
        {
            Transform hit = inspectionOverlaps[i].transform;
            if (hit == player.transform || hit.IsChildOf(player.transform)) continue;
            ForestTree tree = inspectionOverlaps[i].GetComponentInParent<ForestTree>();
            if (tree == target) continue;
            return true;
        }
        return false;
    }

    private IEnumerator AimAt(ForestTree tree)
    {
        Vector3 aimPoint = tree.transform.position + Vector3.up * 1.4f;
        var attempts = new List<string>();
        for (int direction = 0; direction < InspectionDirections.Length; direction++)
            for (int distance = 0; distance < InspectionDistances.Length; distance++)
            {
                Vector3 standPoint = tree.transform.position + InspectionDirections[direction] * InspectionDistances[distance];
                if (OverlapsOtherTree(standPoint, tree)) { attempts.Add(direction + "/" + distance + " blocked"); continue; }
                player.transform.position = standPoint;
                player.LookToward(aimPoint);
                yield return Frames(8);
                ForestTree aimed = marks.AimedTree;
                if (aimed == tree) yield break;
                attempts.Add(direction + "/" + distance + " aimed " + (aimed != null ? aimed.TreeId : "nothing"));
            }
        throw new InvalidOperationException("could not aim at " + tree.TreeId + " [" + string.Join("; ", attempts) + "]");
    }

    private IEnumerator Inspect(ForestTree tree)
    {
        if (player.IsInspecting) yield return Press(Key.E);
        yield return AimAt(tree);
        yield return Press(Key.E);
        yield return Frames(8);
        // A fresh profile opens the first-use Tree Inspection help here; close that tutorial window as RackLessonVerification does.
        ui.CloseHelp();
        yield return Frames(4);
        if (!player.IsInspecting || player.InspectedTree != tree) throw new InvalidOperationException("could not inspect " + tree.TreeId);
    }

    private IEnumerator ReviewStand()
    {
        ui.CloseAll(); ui.CloseHelp();
        foreach (string id in Bay().Future.Select(t => t.Id).ToArray())
        {
            yield return Inspect(Trees().First(t => t.TreeId == id));
            yield return Press(Key.J);
        }
        for (int part = 0; part < 6; part++)
        {
            var bay = Bay();
            ForestTree tree = Trees().First(t => !t.IsMarkedForFell && !manager.IsPlannedForFelling(t.TreeId)
                && bay.Portion(t.transform.position.x, t.transform.position.z) == part);
            yield return Inspect(tree); yield return Press(Key.J);
        }
        if (player.IsInspecting) yield return Press(Key.E);
        yield return Settled();
    }

    private static string Sha(IEnumerable<string> values)
    {
        using (SHA256 sha = SHA256.Create())
            return string.Concat(sha.ComputeHash(Encoding.UTF8.GetBytes(string.Join(",", values.OrderBy(v => v, StringComparer.Ordinal)))).Select(b => b.ToString("x2"))).Substring(0, 16);
    }

    private static double BasalArea(ForestTree t) => Math.PI * Math.Pow(t.Diameter / 200.0, 2.0);

    // ---- shared Lessons 2-5 chain, entered with the rack access executed and its Annual Review acknowledged -------------------------
    // crops/cluster are chosen by the caller: model 0 uses the accepted historical trees, V2 picks from the live footprint.
    private IEnumerator LessonChain(Func<ScenarioOneRackBay.Analysis, string[]> pickCluster, Func<ScenarioOneRackBay.Analysis, string[]> pickCrops)
    {
        var bay = Bay();
        Check(bay != null && bay.Applies, "the live teaching area exists once the access is open and reviewed");
        var cluster = pickCluster(bay);
        foreach (string id in cluster) marks.Mark(Trees().First(t => t.TreeId == id), TreeMarkType.CropTree, false);
        yield return Settled();
        Check(!Bay().HasEnoughFuture && ui.Learning.CurrentTopicIndex == 1, "clustered triple cannot complete Objective 2 live", string.Join(",", cluster));
        foreach (var t in Trees().Where(t => t.IsCropTree)) marks.Unmark(t, false);
        var crops = pickCrops(bay);
        foreach (string id in crops) marks.Mark(Trees().First(t => t.TreeId == id), TreeMarkType.CropTree, false);
        yield return Settled(); bay = Bay();
        Check(bay.HasEnoughFuture && bay.NearSelected && bay.MiddleSelected && bay.FarSelected && bay.BothSides, "three distributed retained trees qualify", string.Join(",", crops));
        Note(mode + ".crop-trees", string.Join(",", bay.Future.Select(t => t.Id + "@(" + F(t.X) + "," + F(t.Z) + ")")));
        var picked = bay.Future.ToArray(); bool spaced = true;
        for (int i = 0; i < picked.Length; i++) for (int j = i + 1; j < picked.Length; j++)
            spaced &= Vector2.Distance(new Vector2(picked[i].X, picked[i].Z), new Vector2(picked[j].X, picked[j].Z)) >= ScenarioOneRackBay.FutureTreeSpacingM;
        Check(spaced && bay.TooClose.Count == 0, "the three Crop Trees are at least the 8 m teaching spacing apart");
        Check(ui.Learning.CurrentTopicIndex == 2, "Objective 3 follows distributed selections");
        ForestTree outside = Trees().First(t => !t.IsCropTree && !bay.Contains(t.transform.position.x, t.transform.position.z)
            && bay.Future.Any(f => Vector2.Distance(new Vector2(f.X, f.Z), new Vector2(t.transform.position.x, t.transform.position.z)) <= 8));
        marks.Mark(outside, TreeMarkType.Fell, false);
        Check(Bay().MarkedRemovals == 0 && Bay().MinWithMarks == 0, "nearby outside-footprint marks give no lesson credit", outside.TreeId);
        marks.Unmark(outside, false);
        for (int guard = 0; guard < 150; guard++)
        {
            bay = Bay(); if (bay.MinPlannedAndExecuted >= ScenarioOneRackBay.FirstReleaseShare) break;
            var f = bay.Future.OrderBy(t => t.ReleaseShare).First();
            ForestTree next = Trees().Where(t => !t.IsCropTree && !manager.IsPlannedForFelling(t.TreeId)
                && bay.Contains(t.transform.position.x, t.transform.position.z)
                && Vector2.Distance(new Vector2(f.X, f.Z), new Vector2(t.transform.position.x, t.transform.position.z)) <= 8)
                .OrderByDescending(t => ForestEcologyController.HegyiTerm(t.Diameter, f.DbhCm,
                    Vector2.Distance(new Vector2(f.X, f.Z), new Vector2(t.transform.position.x, t.transform.position.z)))).First();
            marks.Mark(next, TreeMarkType.Fell, false); manager.AddMarkedTreesToWorkPlan();
        }
        bay = Bay();
        Check(bay.ReleasePlanned && bay.CanopyKept, "initial in-footprint 30% plan is feasible live",
            "removals " + bay.PlannedRemovals + " BA share " + F(bay.RemovedBasalShare) + " min " + F(bay.MinPlannedAndExecuted));
        Note(mode + ".initial-release", "removals=" + bay.PlannedRemovals + " baShare=" + R(bay.RemovedBasalShare) + " minShare=" + R(bay.MinPlannedAndExecuted));
        foreach (var f in bay.Future)
        {
            yield return Inspect(Trees().First(t => t.TreeId == f.Id));
            float panelShare = ui.Competitors.Report.PlannedShare;
            Check(Mathf.Abs(panelShare - f.PlannedShare) < .005f, "panel and lesson initial share agree for " + f.Id, "panel " + F(panelShare) + " lesson " + F(f.PlannedShare));
        }
        if (player.IsInspecting) yield return Press(Key.E);
        var order = manager.WorkOrders.First(o => o.IsOpen && o.type == ScenarioWorkType.FellTree);
        Check(manager.SetPendingFellingOutcome(order.workOrderId, FellingMaterialOutcome.RetainAsFallenDeadwood), "set actual deadwood outcome");
        Check(Bay().MixedPlanned, "both actual outcomes counted inside footprint");
        ForestSaveData safePlan = saves.CaptureData();
        foreach (var t in Trees().Where(t => !t.IsCropTree && bay.Contains(t.transform.position.x, t.transform.position.z))) marks.Mark(t, TreeMarkType.Fell, false);
        Check(!Bay().CanopyKept && !ui.ApproveWorkPlan() && !ui.AdvanceFromWorkPlan(), "over-cap interactive approval and execution refused");
        Check(saves.LoadData(safePlan, false), "restore valid in-memory plan"); yield return Frames(12); yield return Settled();
        Check(ui.ApproveWorkPlan(), "approve bounded intervention");
        Check(ui.AdvanceFromWorkPlan(), "execute bounded intervention through Work Plan UI action");
        yield return Frames(30); ui.CloseHelp(); yield return Settled();
        Check(Bay().ReleaseExecuted && Bay().MixedExecuted, "live executed release has sold and deadwood outcomes");
        Check(ui.Learning.CurrentTopicIndex == 4 && !LiveFacts().RackReviewComplete, "Objective 5 requires real return review");
        ui.AcknowledgeAnnualReview(); yield return Settled();
        Check(!LiveFacts().RackReviewComplete && LiveFacts().RevisitedTrees == 0, "generic acknowledgement gives no tree inspection credit");
        int removed = Bay().ExecutedRemovals;
        yield return ReviewStand();
        Check(LiveFacts().RackReviewComplete && LiveFacts().RevisitedTrees == 3 && LiveFacts().StandPortionsReviewed == 6, "all three actual inspections and retained stand decisions complete return review");
        Check(Bay().ExecutedRemovals == removed && ui.Learning.CurrentTopicIndex == 5, "retain-here completes with zero further felling");
        Note(mode + ".executed-release", "removals=" + removed + " baShare=" + R(Bay().RemovedBasalShare) + " minExecuted=" + R(Bay().MinExecuted));
        ForestSaveData completed = saves.CaptureData();
        Check(saves.LoadData(completed, false), "reload after review with no schema additions"); yield return Frames(12); yield return Settled();
        Check(!LiveFacts().RackReviewComplete && LiveFacts().RevisitedTrees == 0 && ui.Learning.CurrentTopicIndex == 4, "reload requires fresh actual return inspection");
        yield return ReviewStand();
        Check(LiveFacts().RackReviewComplete && Bay().ExecutedRemovals == removed, "fresh retain decisions after load restore lesson completion without cutting");
    }

    private IEnumerator ExecuteAccessAndReview()
    {
        Check(ui.ApproveWorkPlan(), "approve the actual rack access work");
        Check(ui.AdvanceFromWorkPlan(), "execute the rack access through the Work Plan UI action");
        yield return Frames(30); ui.RackLayout.Update(true); ui.CloseHelp();
        ui.AcknowledgeAnnualReview(); ui.CloseAll(); yield return Settled();
        Check(LiveFacts().AccessOpen && LiveFacts().AccessReviewed, "live access executed and Annual Review acknowledged");
    }

    // ================================================ V2 =======================================================================
    private ScenarioOneRackSampleGeometry.Result sample;

    private List<int> AddRacks(IEnumerable<int> order, float[] z)
    {
        var ids = new List<int>();
        foreach (int i in order)
        {
            RackPlanOperationResult add = manager.AddRack(new RackPoint(ScenarioOneHeadlandGeometry.HeadlandX, z[i]), new RackPoint(planner.HalfExtentM, z[i]));
            Check(add.Succeeded && add.RacksAdded == 1, "rack adds at z " + F(z[i]), add.Message);
            ids.Add(add.RackId);
        }
        return ids;
    }

    private static bool InRect(RackPlanRack r, float x, float z)
    {
        double dx = r.Far.X - r.Entrance.X, dz = r.Far.Z - r.Entrance.Z, length = Math.Sqrt(dx * dx + dz * dz);
        double px = x - r.Entrance.X, pz = z - r.Entrance.Z;
        double along = (px * dx + pz * dz) / length, across = (-px * dz + pz * dx) / length;
        return along >= -1e-4 && along <= length + 1e-4 && Math.Abs(across) <= r.WidthM * 0.5 + 1e-4;
    }

    // The footprint re-computed from the raw persisted rack records with plain arithmetic (every rack is east-west in this fixture).
    private IEnumerator MeasureTeachingArea(string label, int expectedRackId, float expectedZ, string expectedFootprintSha = null)
    {
        var racks = manager.ConfirmedRacks.OrderBy(r => (r.Entrance.Z + r.Far.Z) * 0.5f).ToList();
        Check(racks.Count == 5 && racks.All(r => Mathf.Abs(r.AngleDegrees) < 1e-3f && Mathf.Abs(r.WidthM - RackGeometry.DefaultWidthM) < 1e-5f), label + ": five confirmed east-west racks of the default 2.4 m width");
        RackPlanRack third = racks[2];
        Check(third.Id == expectedRackId && Mathf.Abs((third.Entrance.Z + third.Far.Z) * 0.5f - expectedZ) < 1e-3f, label + ": independently, the third rack by position is rack " + expectedRackId, "z " + F(third.Entrance.Z));
        var bay = Bay();
        Check(bay != null && bay.Applies, label + ": the confirmed plan has a live teaching area");
        Check(bay.TeachingRackId == third.Id && bay.RackIndex == 2 && bay.RackCount == 5 && ScenarioOneRackBayText.AreaName(bay) == "the middle rack",
            label + ": the teaching area is on the third confirmed rack and is named 'the middle rack'", "rack " + bay.TeachingRackId);
        Check(bay.HalfWidthM == 9f && Mathf.Abs(bay.Geometry.X) < 1e-3f && Mathf.Abs(bay.Geometry.Z - third.Entrance.Z) < 1e-3f && bay.Geometry.AlongX == 1f && bay.Geometry.AlongZ == 0f,
            label + ": anchor is the closest point to stand centre (x 0, on the rack), half-width 9.0 m, along east", "anchor (" + F(bay.Geometry.X) + "," + F(bay.Geometry.Z) + ")");
        float anchorX = bay.Geometry.X, anchorZ = bay.Geometry.Z;
        Check(anchorX - 12f >= Mathf.Min(third.Entrance.X, third.Far.X) && anchorX + 12f <= Mathf.Max(third.Entrance.X, third.Far.X), label + ": the +-12 m footprint ends are on the confirmed segment");

        var living = AllLiving();
        int mismatches = 0, inFootprint = 0, inCorridorBand = 0, outsideNineBand = 0, hist8Band = 0; double ba = 0; var ids = new List<string>();
        foreach (var t in living)
        {
            float x = t.transform.position.x, z = t.transform.position.z;
            bool band = Mathf.Abs(x - anchorX) <= 12f && Mathf.Abs(z - anchorZ) <= 9f;
            bool expected = band && !racks.Any(r => InRect(r, x, z));
            if (expected != bay.Contains(x, z)) mismatches++;
            if (expected) { inFootprint++; ba += BasalArea(t); ids.Add(t.TreeId); if (Mathf.Abs(z - anchorZ) > 8.00385f) hist8Band++; }
            if (band && !expected) inCorridorBand++;
            if (Mathf.Abs(x - anchorX) <= 12f && Mathf.Abs(z - anchorZ) > 9f && Mathf.Abs(z - anchorZ) <= 10f) outsideNineBand++;
        }
        Check(mismatches == 0, label + ": production footprint membership equals the independent re-computation for all " + living.Length + " living trees", "mismatches " + mismatches);
        Check(inFootprint > 0 && inCorridorBand > 0, label + ": rack corridor trees inside the band are excluded, the rest counted", "footprint " + inFootprint + " corridor-excluded " + inCorridorBand);
        string sha = Sha(ids);
        if (expectedFootprintSha != null) Check(sha == expectedFootprintSha, label + ": same physical footprint tree set as the first plan", sha);
        Note("v2." + label + ".teaching-area", "rack=" + third.Id + " anchor=(" + R(anchorX) + "," + R(anchorZ) + ") halfWidth=9 halfLength=12 footprintTrees=" + inFootprint
            + " footprintBA=" + R(ba) + " corridorExcluded=" + inCorridorBand + " treesBetween8.00385and9m=" + hist8Band + " livingOutsideNineMetreBandWithin10m=" + outsideNineBand
            + " footprintSha=" + sha + " (historical model-0: 69 trees, 1.29490 m2, half-width 8.00385 m)");
        measurementSha = sha;
        yield return null;
    }
    private string measurementSha = "";

    private IEnumerator V2()
    {
        Check(manager.RackPlanningV2Active && manager.RackPlanningModelVersion == RackPlanningModel.RackPlanningFieldMarkingV2 && ecology.StandGeometryModelVersion == StandGeometryModel.Enlarged80,
            "a fresh game is the Enlarged80 Rack Planning V2 world");
        Check(manager.PlannedRackCount == 0 && !manager.RackPlanConfirmed, "a fresh V2 world starts with an empty, unconfirmed plan");
        Check(Bay() == null, "no racks yet: no teaching area");
        planner = ui.RackTool.Planner; planner.Refresh();
        Check(Mathf.Abs(planner.HalfExtentM - 40f) < 1e-4f, "the V2 stand half-extent is 40 m");

        // The qualified fixture: the rack rows come from the live qualified entrance trees, then must equal the persisted values.
        var living = AllLiving();
        var z = new float[5];
        for (int i = 0; i < 5; i++)
        {
            ForestTree entrance = living.SingleOrDefault(t => t.TreeId == QualifiedEntrances[i]);
            Check(entrance != null, "qualified entrance tree " + QualifiedEntrances[i] + " exists");
            z[i] = entrance.transform.position.z;
            Check(Math.Abs(z[i] - QualifiedZ[i]) < 1e-4, "entrance row " + QualifiedEntrances[i] + " is the qualified rack z", F(z[i]) + " vs " + F(QualifiedZ[i]));
        }

        // ---- plan one: qualified order, ids 1..5 ----
        var ids = AddRacks(new[] { 0, 1, 2, 3, 4 }, z);
        Check(ids.SequenceEqual(new[] { 1, 2, 3, 4, 5 }), "qualified order gives rack ids 1..5");
        Check(!manager.RackPlanConfirmed && Bay() == null, "a DRAFT plan has no teaching area (nothing is rebound to an authored line)");
        Note("v2.rack-records", string.Join(" | ", manager.RackPlan.Select(r => "id=" + r.Id + " entrance=(" + R(r.Entrance.X) + "," + R(r.Entrance.Z) + ") far=(" + R(r.Far.X) + "," + R(r.Far.Z)
            + ") angle=" + F(r.AngleDegrees) + " width=" + F(r.WidthM) + " length=" + F(r.LengthM))));
        RackPlanOperationResult confirm = manager.ConfirmRackPlan();
        Check(confirm.Succeeded && manager.RackPlanConfirmed && manager.ConfirmedRacks.Count == 5, "Confirm applies to the five-rack plan", confirm.Message);
        yield return MeasureTeachingArea("plan1", 3, z[2]);
        string firstSha = measurementSha;
        float firstAnchorX = Bay().Geometry.X, firstAnchorZ = Bay().Geometry.Z;

        // Coverage geometry from the live confirmed racks (read-only; compared with plain arithmetic).
        sample = ScenarioOneRackSampleGeometry.Derive(manager.ConfirmedRacks, planner.HalfExtentM);
        Check(sample.Valid && sample.TeachingRack.Id == 3, "live coverage derivation is valid and selects rack 3", sample.Problem);
        bool halfway = true;
        for (int k = 0; k < 4; k++) halfway &= Mathf.Abs(sample.Units[k].UpperOffsetM - (z[k] + z[k + 1]) * 0.5f) < 1e-3f;
        double total = sample.Units.Sum(u => (double)u.AreaM2) + sample.RightRemainder.AreaM2 + sample.LeftRemainder.AreaM2;
        Check(halfway && Math.Abs(total - 6400.0) < 0.05 && sample.RightRemainder.IsEmpty && sample.LeftRemainder.IsEmpty
            && Mathf.Abs(sample.Units[2].LowerOffsetM - (firstAnchorZ - 9f)) > 0.5f, "coverage boundaries are halfway between live rack centrelines, partition 6400 m2 and differ from the 9.0 m teaching edges",
            "boundaries " + string.Join(",", sample.Units.Take(4).Select(u => F(u.UpperOffsetM))) + " total " + F(total));
        Note("v2.coverage", "units=" + string.Join(" ", sample.Units.Select(u => "rack" + u.RackId + "[" + F(u.LowerOffsetM) + "," + F(u.UpperOffsetM) + "]area=" + F(u.AreaM2)))
            + " rightRemainder(edge=" + F(sample.RightRemainder.StripEdgeOffsetM) + ",area=" + F(sample.RightRemainder.AreaM2) + ") leftRemainder(edge=" + F(sample.LeftRemainder.StripEdgeOffsetM)
            + ",area=" + F(sample.LeftRemainder.AreaM2) + ")");

        // Save/load stability: the same JSON the disk save writes, loaded back in memory.
        ForestSaveData data = saves.CaptureData();
        string json = JsonUtility.ToJson(data, true);
        string before = RackSignature();
        Check(saves.LoadData(JsonUtility.FromJson<ForestSaveData>(json), false), "the confirmed plan loads back from its JSON save"); yield return Frames(12); yield return Settled();
        Check(manager.RackPlanConfirmed && RackSignature() == before, "rack ids, endpoints and frozen sets survive save/load exactly");
        yield return MeasureTeachingArea("plan1-after-reload", 3, z[2], firstSha);
        Check(Mathf.Abs(Bay().Geometry.X - firstAnchorX) < 1e-6f && Mathf.Abs(Bay().Geometry.Z - firstAnchorZ) < 1e-6f, "the selected rack and anchor are bit-stable through save/load");

        // Confirmation boundary: reopen suspends the teaching area; it does not rebind. Reconfirming restores the same rack.
        RackPlanOperationResult reopen = manager.ReopenRackPlan();
        Check(reopen.Succeeded && !manager.RackPlanConfirmed && manager.Racks.Count == 5, "Reopen leaves the five persisted racks but unconfirms the plan");
        Check(Bay() == null, "a REOPENED plan has no teaching area: it suspends, it does not rebind to an authored line or another rack");
        RackPlanOperationResult again = manager.ConfirmRackPlan();
        Check(again.Succeeded && manager.RackPlanConfirmed, "Confirm again", again.Message);
        yield return MeasureTeachingArea("plan1-reconfirmed", 3, z[2], firstSha);

        // ---- plan two: the same geometry, ids in a different order, so the third by id is NOT the third by position ----
        foreach (int id in ids)
        {
            RackPlanOperationResult delete = manager.DeleteRack(id);
            Check(delete.Succeeded, "delete rack " + id + " (draft work only)", delete.Message);
        }
        Check(manager.Racks.Count == 0 && !manager.RackPlanConfirmed && Bay() == null, "an emptied plan has no teaching area");
        var permuted = AddRacks(new[] { 2, 4, 0, 3, 1 }, z);
        Check(permuted.SequenceEqual(new[] { 6, 7, 8, 9, 10 }), "ids are never reused: the permuted plan holds racks 6..10", string.Join(",", permuted));
        Check(manager.ConfirmRackPlan().Succeeded && manager.RackPlanConfirmed, "the permuted plan confirms");
        int thirdById = permuted[2], thirdByPosition = permuted[0];
        Check(thirdById == 8 && thirdByPosition == 6 && permuted[0] != permuted[2], "rack 8 is third by id but rack 6 (z " + F(z[2]) + ") is third by position");
        yield return MeasureTeachingArea("plan2-permuted-ids", thirdByPosition, z[2], firstSha);
        Check(Mathf.Abs(Bay().Geometry.X - firstAnchorX) < 1e-6f && Mathf.Abs(Bay().Geometry.Z - firstAnchorZ) < 1e-6f, "the same physical rack and anchor are selected whatever the rack ids");
        Note("v2.rack-records-plan2", string.Join(" | ", manager.RackPlan.Select(r => "id=" + r.Id + " z=" + R(r.Entrance.Z))));

        // ---- Lessons 2-5 against the V2 teaching frame ----
        yield return ExecuteAccessAndReview();
        var accessed = Bay();
        Check(accessed != null && accessed.TeachingRackId == thirdByPosition, "the teaching area is unchanged after the rack work is executed");
        double accessBa = AllLiving().Where(t => accessed.Contains(t.transform.position.x, t.transform.position.z)).Sum(BasalArea);
        int accessTrees = AllLiving().Count(t => accessed.Contains(t.transform.position.x, t.transform.position.z));
        Check(accessed.RemovedBasalShare == 0f && accessed.ExecutedRemovals == 0 && accessed.MinExecuted == 0f, "the executed rack access earns no release credit and does not enter the safeguard");
        Note("v2.after-access", "footprintTrees=" + accessTrees + " footprintBA=" + R(accessBa) + " (historical model-0 after access: 69 trees, 1.29490 m2)");
        yield return LessonChain(
            bay => ClusterFor(bay),
            bay => DistributedCrops(bay));
        Check(Bay().TeachingRackId == thirdByPosition && Mathf.Abs(Bay().Geometry.Z - firstAnchorZ) < 1e-6f, "the selected rack is unchanged after the whole Lessons 2-5 chain and a reload");
    }

    private string RackSignature()
        => string.Join("|", manager.RackPlan.Select(r => r.Id + ":" + R(r.Entrance.X) + "/" + R(r.Entrance.Z) + ":" + R(r.Far.X) + "/" + R(r.Far.Z) + ":" + string.Join(",", r.RemovalTreeIds)))
           + "|confirmed=" + manager.RackPlanConfirmed + "|next=" + manager.NextRackId;

    // One Crop Tree per zone, on alternating sides, as a player would choose them: well apart inside the footprint.
    private string[] DistributedCrops(ScenarioOneRackBay.Analysis bay)
    {
        var targets = new[] { new Vector2(-8f, 4.5f), new Vector2(0f, -4.5f), new Vector2(8f, 4.5f) };
        var picked = new List<string>();
        foreach (Vector2 target in targets)
        {
            ForestTree tree = Trees().Where(t => !t.IsCropTree && !picked.Contains(t.TreeId) && bay.Contains(t.transform.position.x, t.transform.position.z))
                .OrderBy(t => Mathf.Pow(bay.Geometry.Along(t.transform.position.x, t.transform.position.z) - target.x, 2f)
                    + Mathf.Pow(bay.Geometry.Across(t.transform.position.x, t.transform.position.z) - target.y, 2f)).First();
            picked.Add(tree.TreeId);
        }
        return picked.ToArray();
    }

    // Three trees inside the footprint standing closer together than the 8 m spacing.
    private string[] ClusterFor(ScenarioOneRackBay.Analysis bay)
    {
        ForestTree seed = Trees().Where(t => bay.Contains(t.transform.position.x, t.transform.position.z))
            .OrderBy(t => Mathf.Pow(bay.Geometry.Along(t.transform.position.x, t.transform.position.z), 2f) + Mathf.Pow(bay.Geometry.Across(t.transform.position.x, t.transform.position.z) - 4.5f, 2f)).First();
        var near = Trees().Where(t => t != seed && bay.Contains(t.transform.position.x, t.transform.position.z))
            .OrderBy(t => Vector3.Distance(t.transform.position, seed.transform.position)).Take(2).ToArray();
        return new[] { seed.TreeId, near[0].TreeId, near[1].TreeId };
    }

    // ============================================== model 0 ===================================================================
    private IEnumerator Model0()
    {
        // The historical model-0 world: a fresh game is now a V2 world, so regenerate the enlarged stand with the rack plan cleared.
        var stand = FindFirstObjectByType<ForestStartingStand>();
        Check(stand != null, "ForestStartingStand is present");
        manager.InitializeNewScenario();
        foreach (var tree in FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None)) DestroyImmediate(tree.gameObject);
        ecology.ApplyStandGeometry(StandGeometryModel.Enlarged80);
        ecology.ResetForDeterministicRun();
        stand.Generate();
        yield return Frames(3); ui.CloseAll(); ui.CloseHelp(); yield return Frames(3);
        Check(!manager.RackPlanningV2Active && manager.RackPlanningModelVersion == 0 && ecology.StandGeometryModelVersion == StandGeometryModel.Enlarged80, "the world is historical model-0 Enlarged80");
        planner = ui.RackTool.Planner; planner.Refresh();
        Check(planner.LayoutLines.Count == 5, "the authored layout has five racks");
        foreach (float across in planner.LayoutLines.ToArray())
        {
            planner.Refresh(); var proposal = planner.Propose(new RackPoint(0, across), 0f);
            if (proposal.CanConfirm) manager.ConfirmRack(proposal, "map");
        }
        Check(manager.RackGroups.Count == 5 && manager.WorkOrders.All(o => o.rackId == 0), "five legacy rack groups; model-0 orders carry no rack id");
        var preAccess = Bay();
        Check(preAccess != null && preAccess.Applies && preAccess.TeachingRackId == 0, "model 0: the area exists from the authored layout before any work");
        Note("model0.before-access", "footprintTrees=" + Trees().Count(t => preAccess.Contains(t.transform.position.x, t.transform.position.z))
            + " footprintBA=" + R(Trees().Where(t => preAccess.Contains(t.transform.position.x, t.transform.position.z)).Sum(BasalArea))
            + " halfWidth=" + R(preAccess.HalfWidthM) + " (offline fixture: 69 trees, 1.29490 m2 initial BA, 8.00385 m)");
        yield return ExecuteAccessAndReview();
        var bay = Bay();
        var standSpacing = new SerializedObject(FindFirstObjectByType<ForestStartingStand>()).FindProperty("latticeSpacingMeters").floatValue;
        Check(Mathf.Abs(standSpacing - 2f) < .00001f, "scene lattice spacing remains 2.0 m", "spacing=" + F(standSpacing));
        int footprintTrees = Trees().Count(t => bay.Contains(t.transform.position.x, t.transform.position.z));
        double footprintBa = Trees().Where(t => bay.Contains(t.transform.position.x, t.transform.position.z)).Sum(BasalArea);
        Note("model0.after-access", "footprintTrees=" + footprintTrees + " footprintBA=" + R(footprintBa) + " halfWidth=" + R(bay.HalfWidthM) + " rackId=" + bay.TeachingRackId);
        Check(footprintTrees == 69, "accepted live teaching footprint has 69 living trees after access", "n=" + footprintTrees);
        Check(Mathf.Abs(bay.HalfWidthM - 8.00385f) < .001f, "accepted live middle-rack half-width remains 8.00385 m", "half width=" + F(bay.HalfWidthM));
        var lines = planner.LayoutLines.OrderBy(v => v).ToArray();
        float expectedHalfWidth = Mathf.Min(lines[2] - lines[1], lines[3] - lines[2]) * .5f;
        Check(bay.TeachingRackId == 0 && bay.RackIndex == 2 && bay.RackCount == 5 && Mathf.Abs(bay.HalfWidthM - expectedHalfWidth) < .001f,
            "model 0: the area is the middle authored line with half the gap to its neighbours and no rack record", "half width " + F(bay.HalfWidthM));
        yield return LessonChain(
            b => new[] { "P0616", "P1112", "P1415" },
            b => new[] { "P0714", "P1109", "P0706" });
    }

    // ============================================== Legacy40 ==================================================================
    private IEnumerator Legacy40()
    {
        Check(StandGeometryPolicy.NewGameModelForSession == StandGeometryModel.Legacy40 && ecology.StandGeometryModelVersion == StandGeometryModel.Legacy40,
            "a fresh game is the Legacy40 world (CCF_STAND_GEOMETRY=0)");
        Check(!manager.RackPlanningV2Active && manager.RackPlanningModelVersion == 0, "Legacy40 is model 0");
        planner = ui.RackTool.Planner; planner.Refresh();
        Check(planner.LayoutLines.Count == 0, "Legacy40 has no authored layout lines");
        Check(Bay() == null && !LiveFacts().AccessOpen, "Legacy40: no teaching area before any work");
        var rows = planner.Rows(0f);
        Check(rows.Count > 2, "Legacy40 has planted rows");
        planner.Refresh(); var proposal = planner.Propose(new RackPoint(0f, rows[rows.Count / 2].Centre), 0f);
        Check(proposal != null && proposal.CanConfirm && manager.ConfirmRack(proposal, "verification") > 0, "the legacy planner adds one rack");
        Check(ui.ApproveWorkPlan() && ui.AdvanceFromWorkPlan(), "approve and execute the legacy rack");
        yield return Frames(30); ui.CloseHelp(); ui.AcknowledgeAnnualReview(); ui.CloseAll(); yield return Settled();
        Check(Bay() == null, "Legacy40: still no teaching area after the rack is harvested");
        Check(LiveFacts().Bay == null && ui.Learning.CurrentTopicIndex >= 1, "Legacy40 keeps the simple no-rack rules and moves on from the first completed felling");
        marks.Mark(Trees().First(), TreeMarkType.CropTree, false); yield return Settled();
        Check(FreshFacts().CropTrees == 1, "Legacy40 counts a Crop Tree by the old simple rule (no footprint)");
        Note("legacy40.topic", "topic=" + ui.Learning.CurrentTopicIndex + " bay=null");
    }
}
#endif
