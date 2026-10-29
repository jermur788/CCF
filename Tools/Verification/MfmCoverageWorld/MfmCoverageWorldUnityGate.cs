#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Cov = ScenarioOneCoverage;

// Disposable Editor-only gate for MFM-08A: the production coverage-world adapter (masks and management features) on a live Rack V2 world.
// Stage with the runner, then remove source and .meta. Disk files are written only into the isolated persistentDataPath.
//
// It builds the sample the same way a player would: confirms the five qualified racks, approves and executes the rack access, designates the
// sample's three Crop Trees, marks and executes the in-footprint release. Only then does a prescription exist to be derived. Every step uses the
// production manager APIs; nothing is injected into the world, and the harness never calls the Windows build.
public static class MfmCoverageWorldUnityGate
{
    public static void Begin()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        EditorApplication.isPlaying = true;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (Environment.GetCommandLineArgs().Any(a => a == "MfmCoverageWorldUnityGate.Begin"))
            new GameObject("Disposable MFM-08A coverage world gate").AddComponent<MfmCoverageWorldUnityGateRunner>();
    }
}

public sealed class MfmCoverageWorldUnityGateRunner : MonoBehaviour
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
        detail = detail ?? "";
        if (!ok) throw new InvalidOperationException(what + (detail.Length == 0 ? "" : " [" + detail + "]"));
        checks++;
        Debug.Log("MFM08A_CHECK " + what + (detail.Length == 0 ? "" : " [" + detail + "]"));
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
        Debug.Log(failure == null ? "MFM08A_UNITY_PASS checks=" + checks : "MFM08A_UNITY_FAIL checks=" + checks + " " + failure);
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

    // MFM-08A: the coverage the prescription rules actually use (masks and management features included), not a second empty-input copy.
    private ScenarioOneCoverage.Result Coverage()
    {
        ScenarioOneCoverage.Result result = manager.ReadCoverage(out string problem);
        if (result == null) throw new InvalidOperationException("production coverage unreadable: " + problem);
        return result;
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

        // ---- 2. production coverage is readable, read-only and deterministic ----
        string beforeRead = Identity();
        Cov.Result production = manager.ReadCoverage(out string readProblem);
        Check(production != null && production.Valid, "production coverage readable on the live sample", readProblem);
        Check(Identity() == beforeRead, "reading production coverage mutates no mark, order, event, id, cash or world hash");
        Check(manager.ReadCoverage(out _).Signature() == production.Signature(), "production coverage is deterministic across reads");

        // ---- 3. masks: an independent construction from the live scene objects must equal production ----
        ForestStartingStand stand = FindFirstObjectByType<ForestStartingStand>(FindObjectsInactive.Include);
        Check(stand != null && stand.PathCorridorRadiusMeters > 0f && stand.ClearingRadiusMeters > 0f, "starting-stand clearance settings readable through the accessors",
            stand != null ? stand.PathCorridorRadiusMeters + "/" + stand.ClearingRadiusMeters : "none");
        var track = new List<Cov.Mask>(); var clearing = new List<Cov.Mask>(); var site = new List<Cov.Mask>(); var headland = new List<Cov.Mask>();
        foreach (Transform t in FindObjectsByType<Transform>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
            if (t.name.StartsWith("Dirt Path", StringComparison.Ordinal)) track.Add(Cov.Mask.Circle(Cov.MaskKind.Track, t.position.x, t.position.z, stand.PathCorridorRadiusMeters));
        GameObject clearingObject = GameObject.Find("Forest Clearing");
        if (clearingObject != null) clearing.Add(Cov.Mask.Circle(Cov.MaskKind.Clearing, clearingObject.transform.position.x, clearingObject.transform.position.z, stand.ClearingRadiusMeters));
        foreach (ForestBuildable b in FindObjectsByType<ForestBuildable>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            site.Add(Cov.Mask.Circle(Cov.MaskKind.Site, b.transform.position.x, b.transform.position.z, stand.ClearingRadiusMeters));
        // ContainsStemCentre's own radii (float arithmetic, guard included).
        float strip = ScenarioOneHeadlandGeometry.Width * 0.5f + ScenarioOneHeadlandGeometry.StemGuard, pad = ScenarioOneHeadlandGeometry.PadRadius + ScenarioOneHeadlandGeometry.StemGuard;
        headland.Add(new Cov.Mask(Cov.MaskKind.Headland, ScenarioOneHeadlandGeometry.HeadlandX, ScenarioOneHeadlandGeometry.SouthZ, ScenarioOneHeadlandGeometry.HeadlandX, ScenarioOneHeadlandGeometry.NorthZ, strip));
        headland.Add(new Cov.Mask(Cov.MaskKind.Headland, ScenarioOneHeadlandGeometry.HeadlandX, ScenarioOneHeadlandGeometry.FeederZ, ScenarioOneHeadlandGeometry.PathX, ScenarioOneHeadlandGeometry.FeederZ, strip));
        headland.Add(Cov.Mask.Circle(Cov.MaskKind.Headland, ScenarioOneHeadlandGeometry.HeadlandX, ScenarioOneHeadlandGeometry.FeederZ, pad));
        headland.Add(Cov.Mask.Circle(Cov.MaskKind.Headland, ScenarioOneHeadlandGeometry.PathX, ScenarioOneHeadlandGeometry.FeederZ, pad));
        Check(track.Count > 0 && clearing.Count == 1 && site.Count > 0, "every infrastructure source exists in the live world", "track=" + track.Count + " clearing=" + clearing.Count + " site=" + site.Count);
        Debug.Log("MFM08A_NOTE masks track=" + track.Count + " clearing=" + clearing.Count + " site=" + site.Count + " headland=" + headland.Count);

        string MaskView(Cov.Result r) => string.Join(";", r.Areas.Select(a => a.Id + "|" + a.Metrics.Stems + "|" + a.Metrics.NetAreaM2.ToString("R", System.Globalization.CultureInfo.InvariantCulture)))
            + "#" + r.Source.Stems + "|" + r.Source.NetAreaM2.ToString("R", System.Globalization.CultureInfo.InvariantCulture);
        Cov.Result Reference(IEnumerable<Cov.Mask> masks) => Cov.Classify(new Cov.Input
        {
            Geometry = Geometry(), Stems = LiveStems(), Orders = LiveOrders(), Masks = masks.ToList(), Features = new Cov.Feature[0], PlayerManualAreas = new string[0]
        }, ScenarioOnePrescriptionCalibration.V1);
        var all = track.Concat(clearing).Concat(site).Concat(headland).ToList();
        string productionView = MaskView(production);
        string referenceView = MaskView(Reference(all));
        if (referenceView != productionView)
        {
            string[] p = productionView.Split(';'), r = referenceView.Split(';');
            for (int i = 0; i < Math.Max(p.Length, r.Length); i++)
                if ((i < p.Length ? p[i] : "") != (i < r.Length ? r[i] : ""))
                    Debug.Log("MFM08A_NOTE first mask-view difference at " + i + ": production=" + (i < p.Length ? p[i] : "-") + " reference=" + (i < r.Length ? r[i] : "-"));
        }
        Check(referenceView == productionView, "production masks equal an independent construction from the live scene (areas, stems, net areas)");
        Check(MaskView(Reference(new Cov.Mask[0])) != productionView, "production is not passing empty masks");
        Check(MaskView(Reference(all.Except(track))) != productionView, "mask kind Track is in the production result");
        Check(MaskView(Reference(all.Except(clearing))) != productionView, "mask kind Clearing is in the production result");
        Check(MaskView(Reference(all.Except(site))) != productionView, "mask kind Site is in the production result");
        Check(MaskView(Reference(all.Except(headland))) != productionView, "mask kind Headland is in the production result");

        // The Site mask never follows the player: moving the player cannot change production coverage.
        ForestPlayer player = FindFirstObjectByType<ForestPlayer>();
        if (player != null)
        {
            Vector3 home = player.transform.position;
            player.transform.position = home + new Vector3(9f, 0f, 6f);
            string moved = MaskView(manager.ReadCoverage(out _));
            player.transform.position = home;
            Check(moved == productionView, "moving the player changes no mask (Site is structures only)");
        }
        else Debug.Log("MFM08A_NOTE no ForestPlayer in the scene; player-movement witness is covered by the static guard");

        // ---- 4. natural regeneration is flagged from the ecology's own cohorts ----
        int naturalCells = 0;
        for (int i = 0; i < ecology.CellCount; i++)
            if (ecology.Cells[i].Regeneration.Any(c => c != null && c.Density > 0f && c.Origin == RegenerationOrigin.Natural)) naturalCells++;
        var flagged = production.Areas.Where(a => a.Flags.Contains(Cov.Flag.NaturalRegeneration)).ToList();
        Debug.Log("MFM08A_NOTE natural regeneration cells=" + naturalCells + " flagged areas=" + flagged.Count);
        Check(naturalCells == 0 || flagged.Count > 0, "natural regeneration cells in the ecology flag the areas that contain them");
        Check(flagged.All(a => a.State != Cov.State.Excluded || a.Reasons.Any()), "natural regeneration alone never excludes an area");
        Check(flagged.SelectMany(a => a.Evidence).Any(e => e.StartsWith("regen-cell-", StringComparison.Ordinal)) || flagged.Count == 0, "natural regeneration evidence ids come from the cell index");

        // ---- 5. production management work and features: plant through the production path ----
        Cov.Area target = production.Areas.Where(a => !a.IsSource && a.State == Cov.State.Covered && a.StemIds.Count > 0).OrderBy(a => a.Id, StringComparer.Ordinal).FirstOrDefault();
        Check(target != null, "a covered area exists to receive management work");
        Cov.Stem anchor = LiveStems().First(s => s.Id == target.StemIds[0]);
        Check(manager.TryPurchaseStock("sessile-oak-sapling", 1), "sapling purchased", manager.Feedback);
        bool designated = false; Vector3 plantAt = Vector3.zero;
        foreach (Vector2 offset in new[] { new Vector2(1.0f, 0.9f), new Vector2(-1.0f, 0.9f), new Vector2(0.9f, -1.0f), new Vector2(-0.9f, -1.0f), new Vector2(1.6f, 0f), new Vector2(0f, 1.6f) })
        {
            plantAt = new Vector3(anchor.X + offset.x, 0f, anchor.Z + offset.y);
            if (manager.TryDesignateExactPlanting("sessile-oak-sapling", plantAt, CCF.Forestry.WorkEconomy.WorkExecutionMethod.LandownerSimulated, true)) { designated = true; break; }
        }
        Check(designated, "planting designated at an exact position in the covered area", manager.Feedback);
        ScenarioOneWorkOrder plantOrder = manager.WorkOrders.Last(o => o.type == ScenarioWorkType.PlantJuvenile && o.IsOpen);
        Cov.Result withOrder = manager.ReadCoverage(out string orderProblem);
        Check(withOrder != null, "coverage readable with open planting work", orderProblem);
        string where = target.Id;
        Cov.Area planted = withOrder.Areas.FirstOrDefault(a => a.Evidence.Contains("order-" + plantOrder.workOrderId));
        Check(planted != null && planted.State == Cov.State.Excluded && planted.Reasons.Contains(Cov.Reason.ActiveManagementWork),
            "an open planting order excludes its area as ActiveManagementWork with its order id as evidence", planted == null ? "no area carries the evidence" : planted.Id + " " + planted.State);
        Debug.Log("MFM08A_NOTE planting at " + plantAt + " lands in area " + (planted != null ? planted.Id : "?") + " (target " + where + ")");

        Check(manager.ApprovePendingWork(), "planting approved through the Work Plan", manager.Feedback);
        Check(manager.AdvanceYear(), "planting executed", manager.Feedback);
        yield return Settle();
        manager.MarkAnnualReviewSeen();
        PlantedJuvenile juvenile = manager.PlantedJuveniles.LastOrDefault(j => j != null && j.alive);
        Check(juvenile != null, "a planted juvenile exists after execution", manager.PlantedJuveniles.Count.ToString());
        Cov.Result afterPlanting = manager.ReadCoverage(out string afterProblem);
        Check(afterPlanting != null, "coverage readable after planting", afterProblem);
        Cov.Area juvArea = afterPlanting.Areas.FirstOrDefault(a => a.Evidence.Contains(juvenile.juvenileId));
        Check(juvArea != null && juvArea.State == Cov.State.Excluded && juvArea.Reasons.Contains(Cov.Reason.PlantedJuvenile),
            "the planted juvenile excludes its area as PlantedJuvenile with its own id as evidence", juvArea == null ? "no area carries " + juvenile.juvenileId : juvArea.Id + " " + juvArea.State);
        Check(!afterPlanting.Areas.Any(a => a.Evidence.Contains("order-" + plantOrder.workOrderId)), "the completed order is no longer active management work");
        if (manager.Shelters.Count > 0)
        {
            BrowseShelter shelter = manager.Shelters.Last();
            Cov.Area shelterArea = afterPlanting.Areas.FirstOrDefault(a => a.Evidence.Contains(shelter.shelterId));
            Check(shelterArea != null && shelterArea.Reasons.Contains(Cov.Reason.Shelter), "the shelter excludes its area as Shelter with its own id as evidence");
        }
        else Debug.Log("MFM08A_NOTE no shelter was installed by this planting");
        if (manager.ClearancePatches.Count > 0)
        {
            PlantingClearancePatch patch = manager.ClearancePatches.Last();
            Cov.Area patchArea = afterPlanting.Areas.FirstOrDefault(a => a.Reasons.Contains(Cov.Reason.ClearancePatch));
            Check(patchArea != null, "the planting clearance patch excludes its area as ClearancePatch", patch.center.ToString());
        }
        else Debug.Log("MFM08A_NOTE no clearance patch was recorded by this planting");
        Check(MaskView(afterPlanting) != "" && afterPlanting.Valid, "coverage stays valid after management work");
    }
}
#endif
