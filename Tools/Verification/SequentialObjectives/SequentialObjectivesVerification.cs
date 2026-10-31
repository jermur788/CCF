#if UNITY_EDITOR
using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UIElements;
using CCF.Forestry.WorkEconomy;

// Companion to the existing guided/native-input gate. All interventions below use production APIs.
public sealed partial class GuidedOnboardingRunner
{
    partial void AddSequenceChecks(System.Collections.Generic.List<IEnumerator> extras)
    {
        if (Environment.GetEnvironmentVariable("CCF_SEQUENCE_VERIFY") == "1") extras.Add(VerifySequentialObjectives());
    }
    void Stage(int index, string message)
    {
        ui.RackLayout.Update(true); ui.Learning.Refresh(true);
        int actual = ui.Learning.CurrentTopicIndex;
        string title = ui.Learning.CurrentTitle;
        Check(actual == index, "sequence " + message + ": expected index " + index + " got index " + actual + " title '" + title + "'");
    }

    // The accepted Objectives 2-5 lesson is one rack area with three distributed Crop Trees. The sequence verification must use
    // the same production analysis as the lesson and the rack gate; it must not invent a sequence index or bypass the real marks.
    ScenarioOneRackBay.Analysis Bay()
    {
        // G3 split: V2 progression follows the confirmed player plan through the production sample adapter.
        // Model 0 keeps the exact authored-line fixture below.
        if (manager.RackPlanningV2Active)
        {
            var sample = ScenarioOneRackSampleGeometry.Derive(manager.ConfirmedRacks,
                StandGeometryModel.StandSizeMeters(StandGeometryModel.Enlarged80) * 0.5f);
            return ScenarioOneRackBayReader.Read(FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None), manager.WorkOrders, sample);
        }
        var layout = ui.RackTool.Planner.LayoutStatus();
        var line = layout[layout.Count / 2];
        return ScenarioOneRackBayReader.Read(FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None), manager.WorkOrders,
            ui.RackTool.Planner.LayoutLines, new ScenarioOneRackBay.Frame(line.Start, line.End,
                ScenarioOneRackBay.HalfWidth(ui.RackTool.Planner.LayoutLines, layout.Count / 2)));
    }

    static ForestTree[] LivingSpruce() => FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
        .Where(t => t.IsLiving && t.Species != null && t.Species.SpeciesId == "sitka-spruce")
        .OrderBy(t => t.TreeId, StringComparer.Ordinal).ToArray();

    // The same accepted production rule the rack lesson uses: add in-footprint competitors for the least-released future tree until
    // the planned release reaches the 30% initial-release calibration. The cumulative safeguard is asserted after the loop.
    void PlanRelease()
    {
        ForestTree[] trees = LivingSpruce();
        for (int guard = 0; guard < 150; guard++)
        {
            ScenarioOneRackBay.Analysis bay = Bay();
            if (bay.MinPlannedAndExecuted >= ScenarioOneRackBay.FirstReleaseShare) return;
            ScenarioOneRackBay.FutureTree future = bay.Future.OrderBy(t => t.ReleaseShare).First();
            ForestTree next = trees.Where(t => !t.IsCropTree && !manager.IsPlannedForFelling(t.TreeId)
                && bay.Contains(t.transform.position.x, t.transform.position.z)
                && Vector2.Distance(new Vector2(future.X, future.Z), new Vector2(t.transform.position.x, t.transform.position.z)) <= ForestEcologyController.HegyiCutoffMeters)
                .OrderByDescending(t => ForestEcologyController.HegyiTerm(t.Diameter, future.DbhCm,
                    Vector2.Distance(new Vector2(future.X, future.Z), new Vector2(t.transform.position.x, t.transform.position.z)))).First();
            ui.Marking.Mark(next, TreeMarkType.Fell, false);
            manager.AddMarkedTreesToWorkPlan();
        }
    }

    // Post-release review uses the same real production inspection path as the rack lesson gate. The fixed -Z vantage is not
    // valid for every tree at the accepted 2.0 m spacing, so the gate searches deterministic clear stand points first.
    private static readonly Vector3[] InspectionDirections =
    {
        Vector3.back, Vector3.forward, Vector3.right, Vector3.left,
        new Vector3(-1f, 0f, -1f).normalized, new Vector3(1f, 0f, -1f).normalized,
        new Vector3(-1f, 0f, 1f).normalized, new Vector3(1f, 0f, 1f).normalized
    };
    private static readonly string[] InspectionDirectionNames = { "-Z (legacy)", "+Z", "+X", "-X", "-X-Z", "+X-Z", "-X+Z", "+X+Z" };
    private static readonly float[] InspectionDistances = { 3f, 2.5f, 2f };
    private readonly Collider[] inspectionOverlaps = new Collider[16];

    private static string PoseLabel(int direction, int distance) =>
        InspectionDirectionNames[direction] + "@" + InspectionDistances[distance].ToString("0.#", System.Globalization.CultureInfo.InvariantCulture) + "m";

    private bool OverlapsOtherTree(Vector3 standPoint, ForestTree target, out string blocking)
    {
        blocking = "";
        int count = Physics.OverlapSphereNonAlloc(standPoint + Vector3.up * 0.9f, 0.35f, inspectionOverlaps);
        for (int i = 0; i < count; i++)
        {
            Collider collider = inspectionOverlaps[i];
            Transform hit = collider.transform;
            if (hit == ui.Player.transform || hit.IsChildOf(ui.Player.transform))
                continue;
            ForestTree tree = collider.GetComponentInParent<ForestTree>();
            if (tree == target)
                continue;
            blocking = tree != null ? tree.TreeId : collider.name;
            return true;
        }
        return false;
    }

    private IEnumerator AimAt(ForestTree tree)
    {
        Vector3 aimPoint = tree.transform.position + Vector3.up * 1.4f;
        var attempts = new System.Collections.Generic.List<string>();
        for (int direction = 0; direction < InspectionDirections.Length; direction++)
        {
            for (int distance = 0; distance < InspectionDistances.Length; distance++)
            {
                Vector3 standPoint = tree.transform.position + InspectionDirections[direction] * InspectionDistances[distance];
                if (OverlapsOtherTree(standPoint, tree, out string blocking))
                {
                    attempts.Add(PoseLabel(direction, distance) + " blocked by " + blocking);
                    continue;
                }
                ui.Player.transform.position = standPoint;
                ui.Player.LookToward(aimPoint);
                yield return Pause(8);
                ForestTree aimed = ui.Marking.AimedTree;
                if (aimed == tree) yield break;
                attempts.Add(PoseLabel(direction, distance) + " aimed " + (aimed != null ? aimed.TreeId : "nothing"));
            }
        }
        throw new InvalidOperationException("could not aim at " + tree.TreeId + " [" + string.Join("; ", attempts) + "]");
    }

    private IEnumerator Press(Key key)
    {
        Keyboard keyboard = Keyboard.current ?? InputSystem.AddDevice<Keyboard>();
        InputSystem.QueueStateEvent(keyboard, new KeyboardState(key)); yield return Pause(2);
        InputSystem.QueueStateEvent(keyboard, new KeyboardState()); yield return Pause(2);
    }

    private IEnumerator Inspect(ForestTree tree)
    {
        if (ui.Player.IsInspecting) yield return Press(Key.E);
        ui.CloseAll(); ui.CloseHelp();
        yield return AimAt(tree);
        yield return Press(Key.E);
        yield return Pause(8);
        ui.CloseHelp();
        yield return Pause(4);
        Check(ui.Player.IsInspecting && ui.Player.InspectedTree == tree, "review inspection opened: " + tree.TreeId);
    }

    ScenarioOneTeachingSequence.Facts LiveFacts()
    {
        MethodInfo method = typeof(LearningObjectivesView).GetMethod("TeachingFacts", BindingFlags.Instance | BindingFlags.NonPublic);
        return (ScenarioOneTeachingSequence.Facts)method.Invoke(ui.Learning, null);
    }

    IEnumerator ReviewStand()
    {
        ui.CloseAll(); ui.CloseHelp();
        foreach (string id in Bay().Future.Select(t => t.Id).ToArray())
        {
            yield return Inspect(LivingSpruce().First(t => t.TreeId == id));
            yield return Press(Key.J);
            yield return Pause(6);
        }
        for (int part = 0; part < 6; part++)
        {
            ScenarioOneRackBay.Analysis bay = Bay();
            ForestTree tree = LivingSpruce().First(t => !t.IsMarkedForFell && !manager.IsPlannedForFelling(t.TreeId)
                && bay.Portion(t.transform.position.x, t.transform.position.z) == part);
            yield return Inspect(tree);
            yield return Press(Key.J);
            yield return Pause(6);
        }
        if (ui.Player.IsInspecting) yield return Press(Key.E);
        yield return Pause(20);
        ScenarioOneTeachingSequence.Facts facts = LiveFacts();
        Check(facts.RackReviewComplete && facts.RevisitedTrees == 3 && facts.StandPortionsReviewed == 6,
            "three future trees and six stand portions reviewed", "trees " + facts.RevisitedTrees + " portions " + facts.StandPortionsReviewed);
    }
    IEnumerator LessonShot(string label)
    {
        ui.CloseHelp(); ui.ShowObjectives(); ui.Learning.Refresh(true);
        ui.Learning.Root.Q<Foldout>("scenario-success-objectives").value = false;
        var scroller = ui.Learning.Root.Q<ScrollView>(); scroller.scrollOffset = Vector2.zero;
        yield return Pause(25);
        string hash = ScenarioReferenceArchive.WorldHash(saves.CaptureData());
        for (int i = 0; i < 8; i++) ui.Learning.Refresh(true);
        Check(hash == ScenarioReferenceArchive.WorldHash(saves.CaptureData()), "sequence projection read-only: " + label);
        yield return Shoot("sequence-" + label);
        var current = ui.Learning.Root.Q("current-objective");
        foreach (string name in new[] { "current-objective-title", "current-objective-concept", "current-objective-instruction", "current-objective-progress", "teaching-diagram" })
        {
            var child = current.Q(name);
            Check(child != null && Visible(child) && current.worldBound.Contains(child.worldBound.min) && current.worldBound.Contains(child.worldBound.max), "lesson content inside card: " + label + " / " + name);
        }
        Check(!ui.Learning.Root.Q<Foldout>("future-lessons").value, "future preview collapsed: " + label);
    }
    void RoundTrip(int index, string label)
    {
        ui.CloseAll(); ui.CloseHelp();
        var data = saves.CaptureData(); string hash = ScenarioReferenceArchive.WorldHash(data);
        Check(saves.LoadData(JsonUtility.FromJson<ForestSaveData>(JsonUtility.ToJson(data)), false), "save/load " + label);
        ui.RackLayout.Update(true); ui.Learning.ReloadProgress(); Stage(index, "save/load " + label);
        Check(ScenarioReferenceArchive.WorldHash(saves.CaptureData()) == hash, "save world hash retained: " + label);
    }

    // D-066 makes the first guided rack sample/review completion one monotone durable latch. After that boundary a load must
    // keep the completed teaching stage and must not demand another session rack review. The individual review detail is still
    // session-only, so the reset session review must stay incomplete while the durable latch carries the progression.
    IEnumerator RoundTripReviewed(int index, string label)
    {
        ui.CloseAll(); ui.CloseHelp();
        var data = saves.CaptureData(); string hash = ScenarioReferenceArchive.WorldHash(data);
        Check(data.scenarioOne != null && data.scenarioOne.firstGuidedRackReviewComplete,
            "D-066 historical teaching latch captured: " + label);
        Check(saves.LoadData(JsonUtility.FromJson<ForestSaveData>(JsonUtility.ToJson(data)), false), "save/load " + label);
        ui.RackLayout.Update(true); ui.Learning.ReloadProgress();
        yield return Pause(4);
        ScenarioOneTeachingSequence.Facts facts = LiveFacts();
        Check(facts.FirstGuidedRackReviewComplete, "D-066 historical teaching latch survives save/load: " + label);
        Check(!facts.RackReviewComplete, "session rack review detail remains non-persistent: " + label);
        Stage(index, "save/load " + label + " keeps the completed teaching stage without a fresh review");
        Check(ScenarioReferenceArchive.WorldHash(saves.CaptureData()) == hash, "save world hash retained: " + label);
    }
    IEnumerator AcknowledgeResults(string label)
    {
        ui.ShowReview(); ui.CloseHelp(); yield return Pause(30);
        var button = ui.RootElement.Q<Button>("annual-review-acknowledge");
        Check(button != null && Visible(button) && button.enabledInHierarchy, "reachable saved-review action: " + label);
        yield return Click(button);
    }
    IEnumerator VerifySequentialObjectives()
    {
        string[] goalIds = manager.Objectives.Select(o => o.objectiveId).ToArray();
        float[] targets = manager.Objectives.Select(o => o.targetValue).ToArray();
        Check(goalIds.Length == 8 && ForestSaveData.CurrentVersion == 24, "eight original goals and the v24 schema");
        Stage(1, "post rack, before annual acknowledgement");
        Check(ui.Learning.CurrentInstruction.Contains("Read the Annual Review"), "observation cue precedes crop action");
        RoundTrip(1, "access year boundary");
        yield return AcknowledgeResults("access"); Stage(1, "reviewed but no future tree yet");
        yield return LessonShot("02-future-trees");
        ForestTree[] trees = LivingSpruce();
        // The accepted lesson needs three distributed Crop Trees. Prove the old single-tree shortcut cannot advance the lesson.
        string[] cropIds = { "P0714", "P1109", "P0706" };
        if (manager.RackPlanningV2Active)
        {
            // Same rack-relative fixture selection as the integrated MFM-02 gate: follow the actual confirmed teaching frame.
            var bay = Bay(); var selected = new System.Collections.Generic.List<string>();
            foreach (Vector2 point in new[] { new Vector2(-8f,4.5f), new Vector2(0f,-4.5f), new Vector2(8f,4.5f) })
                selected.Add(trees.Where(t => !selected.Contains(t.TreeId) && bay.Contains(t.transform.position.x,t.transform.position.z))
                    .OrderBy(t => Mathf.Pow(bay.Geometry.Along(t.transform.position.x,t.transform.position.z)-point.x,2f)
                        + Mathf.Pow(bay.Geometry.Across(t.transform.position.x,t.transform.position.z)-point.y,2f)).First().TreeId);
            cropIds = selected.ToArray();
        }
        foreach (string id in cropIds.Take(2))
        {
            ui.Marking.Mark(trees.First(t => t.TreeId == id), TreeMarkType.CropTree, false);
            Stage(1, id == cropIds[0] ? "one crop tree is not enough" : "two crop trees are not enough");
        }
        ui.Marking.Mark(trees.First(t => t.TreeId == cropIds[2]), TreeMarkType.CropTree, false);
        ScenarioOneRackBay.Analysis futureBay = Bay();
        Check(futureBay.HasEnoughFuture && futureBay.NearSelected && futureBay.MiddleSelected && futureBay.FarSelected && futureBay.BothSides,
            "three distributed crop trees satisfy the future-tree requirement",
            "future " + futureBay.FutureCount + " near " + futureBay.NearSelected + " middle " + futureBay.MiddleSelected
            + " far " + futureBay.FarSelected + " both sides " + futureBay.BothSides);
        Stage(2, "three distributed crop trees selected");
        yield return LessonShot("03-selective-release");
        PlanRelease();
        Check(manager.WorkOrders.Any(o => o.IsOpen && o.type == ScenarioWorkType.FellTree), "plan existing relevant competitors");
        ScenarioOneRackBay.Analysis plannedBay = Bay();
        Check(plannedBay.ReleasePlanned && plannedBay.CanopyKept, "three-tree release plan reaches 30% within the cumulative safeguard",
            "min planned " + plannedBay.MinPlannedAndExecuted.ToString("0.###") + " removed BA " + plannedBay.RemovedBasalShare.ToString("0.###"));
        Stage(3, "release planned; disposition before execution");
        var plannedOrders = manager.WorkOrders.Where(o => o.IsOpen && o.type == ScenarioWorkType.FellTree).ToArray();
        var plannedTreeIds = plannedOrders.Select(o => o.targetTreeId).ToArray();
        foreach (var plannedOrder in plannedOrders)
            Check(manager.RemovePendingOrder(plannedOrder.workOrderId), "cancel pending release without false lesson completion: " + plannedOrder.targetTreeId);
        Stage(2, "cancel returns to actionable release guidance");
        foreach (string id in plannedTreeIds)
            ui.Marking.Mark(trees.First(t => t.TreeId == id), TreeMarkType.Fell, false);
        Check(manager.AddMarkedTreesToWorkPlan() == plannedTreeIds.Length, "replan the cancelled competitors");
        Stage(3, "release replanned");
        var order = manager.WorkOrders.Last(o => o.IsOpen && o.type == ScenarioWorkType.FellTree);
        Check(manager.SetPendingFellingOutcome(order.workOrderId, FellingMaterialOutcome.RetainAsFallenDeadwood), "retain fallen wood through existing work order");
        Stage(3, "material planned is not material executed");
        yield return LessonShot("04-habitat-and-advance");
        Check(manager.ApprovePendingWork(), "approve selective intervention");
        Stage(3, "approval cannot bypass time gate"); RoundTrip(3, "approved second intervention");
        ui.CloseAll(); Check(manager.AdvanceYear(), "resolve second intervention"); Stage(4, "second intervention needs result review");
        Check(ScenarioOneCompletionRules.HasReviewedTwoCycleSequence(manager.ManagementEvents, manager.WorkOrders), "protected reviewed two-cycle rule still satisfied by real history");
        RoundTrip(4, "second intervention resolved");
        yield return LessonShot("05-response-and-repeated-management");
        ui.ShowReview(); ui.CloseHelp(); yield return Pause(25); yield return Shoot("sequence-history-after-second-intervention");
        yield return AcknowledgeResults("release after N3");
        yield return ReviewStand();
        Stage(5, "review unlocks regeneration/protection");
        if (Environment.GetEnvironmentVariable("CCF_WRITE_STAGE5_SAVE") == "1")
        {
            saves.Save();
            string source = System.IO.Path.Combine(Application.persistentDataPath, "forest-save.json");
            System.IO.File.Copy(source, System.IO.Path.Combine(Application.persistentDataPath, "stage5-save.json"), true);
            Debug.Log("GUIDED_STAGE5_SAVE_WRITTEN " + Application.persistentDataPath);
        }
        yield return LessonShot("06-regeneration-and-protection");
        ui.CloseAll();
        foreach (string item in new[] { "beech-sapling", "sessile-oak-sapling" })
        {
            Check(manager.TryPurchaseStock(item, 4), "purchase existing nursery stock " + item);
            int planned = 0;
            foreach (int cell in Enumerable.Range(0, ui.Ecology.CellCount).OrderByDescending(i => ui.Ecology.Cells[i].Light))
            {
                Vector2 centre = ui.Ecology.Cells[cell].Center;
                for (int z = -2; z <= 2 && planned < 4; z++)
                for (int x = -2; x <= 2 && planned < 4; x++)
                    if (manager.TryDesignateExactPlanting(item, new Vector3(centre.x + x * .9f, 0, centre.y + z * .9f),
                        WorkExecutionMethod.LandownerSimulated, planned % 2 == 0)) planned++;
                if (planned == 4) break;
            }
            Check(planned == 4, "player-controlled planting sites " + item);
        }
        Check(manager.WorkOrders.Any(o => o.IsOpen && o.type == ScenarioWorkType.PlantJuvenile && o.installShelter)
            && manager.WorkOrders.Any(o => o.IsOpen && o.type == ScenarioWorkType.PlantJuvenile && !o.installShelter), "both existing protection decisions represented");
        Check(manager.ApprovePendingWork(), "approve planting"); Stage(5, "approved planting does not establish species");
        Check(manager.AdvanceYear(), "resolve planting"); Stage(6, "planting year needs later observation");
        yield return RoundTripReviewed(6, "planting time gate");
        yield return LessonShot("07-establishment-advance");
        yield return AcknowledgeResults("planting year"); Stage(6, "planting-year acknowledgement cannot skip extra year");
        ui.CloseAll(); Check(manager.AdvanceYear(), "later observation year"); Stage(6, "later year still needs review");
        yield return RoundTripReviewed(6, "later observation before acknowledgement");
        yield return LessonShot("07-establishment-review");
        yield return AcknowledgeResults("later observation year"); Stage(7, "ongoing transformation, no false scenario completion");
        Check(manager.Outcome == ScenarioOneOutcome.Active, "teaching path does not replace scenario completion or minimum year");
        // Optional fixture export for the menu gate's legitimate Stage 7 replay. The save is written only in the isolated
        // verification config and only when the caller asks for it; normal guided qualification does not write a save.
        if (Environment.GetEnvironmentVariable("CCF_WRITE_STAGE7_SAVE") == "1")
        {
            saves.Save();
            string source = System.IO.Path.Combine(Application.persistentDataPath, "forest-save.json");
            System.IO.File.Copy(source, System.IO.Path.Combine(Application.persistentDataPath, "stage7-save.json"), true);
            Debug.Log("GUIDED_STAGE7_SAVE_WRITTEN " + Application.persistentDataPath);
        }
        yield return LessonShot("08-continue-transformation");
        yield return RoundTripReviewed(7, "ongoing transformation");
        ui.CloseAll(); Check(manager.AdvanceYear() && manager.AdvanceYear(), "multiple advances after teaching path"); Stage(7, "extra advances preserve valid progression");
        Check(goalIds.SequenceEqual(manager.Objectives.Select(o => o.objectiveId)) && targets.SequenceEqual(manager.Objectives.Select(o => o.targetValue)), "all eight goal IDs and targets unchanged");
        Check(ForestSaveData.CurrentVersion == 24, "the save schema version is unchanged");
        // A later generation can refill an original access corridor. The real rack tool should report maintenance;
        // a completed teaching lesson must not restart. This is a disposable stand-in for a promoted young tree.
        string beforeRegrowth = ScenarioReferenceArchive.WorldHash(saves.CaptureData());
        string beforeV2RackState = manager.RackPlanningV2Active ? JsonUtility.ToJson(saves.CaptureData().scenarioOne) : null;
        string beforeV2Counter = manager.RackPlanningV2Active ? ui.RackLayout.V2CounterText : null;
        int[] completedRackWork = manager.WorkOrders.Where(o => o.rackId > 0 && o.status == ScenarioWorkStatus.Completed)
            .Select(o => o.workOrderId).OrderBy(id => id).ToArray();
        var originalTree = FindObjectsByType<ForestTree>(FindObjectsSortMode.None).First(t => t.CanChop);
        var regrowth = Instantiate(originalTree.gameObject).GetComponent<ForestTree>();
        typeof(ForestTree).GetField("treeId", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(regrowth, "REGROWTH_TEACHING_CHECK");
        regrowth.SetMark(TreeMarkType.None);
        regrowth.transform.position = new Vector3(0, originalTree.transform.position.y, ui.RackTool.Planner.LayoutLines[0]);
        ui.RackLayout.Update(true);
        // Direct-user bounded G3 amendment: model 0 measures live authored corridors; V2 measures
        // resolved rack-owned work. A new stem cannot rewrite the V2 removal set or fabricate new work.
        if (manager.RackPlanningV2Active)
        {
            Check(ui.RackLayout.Harvested && completedRackWork.Length > 0
                && completedRackWork.SequenceEqual(manager.WorkOrders.Where(o => o.rackId > 0 && o.status == ScenarioWorkStatus.Completed)
                    .Select(o => o.workOrderId).OrderBy(id => id)), "V2 transient regrowth preserves completed rack-owned work");
            Check(beforeV2RackState == JsonUtility.ToJson(saves.CaptureData().scenarioOne)
                && beforeV2Counter == ui.RackLayout.V2CounterText,
                "V2 transient regrowth preserves authoritative rack geometry, order state and counters");
        }
        else
            Check(!ui.RackLayout.Harvested, "live rack tracker still reports actual later regrowth");
        Stage(7, "later access maintenance does not restart initial lesson");
        Destroy(regrowth.gameObject); yield return Pause(2); ui.RackLayout.Update(true);
        Check(beforeRegrowth == ScenarioReferenceArchive.WorldHash(saves.CaptureData()), "regrowth fixture removed without changing saved world");
    }
}
#endif
