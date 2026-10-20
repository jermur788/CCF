#if UNITY_EDITOR
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
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
        Check(ui.Learning.CurrentTopicIndex == index, "sequence " + message + ": expected " + index + " got " + ui.Learning.CurrentTitle);
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
        Check(goalIds.Length == 8 && ForestSaveData.CurrentVersion == 20, "eight original goals and v20 schema");
        Stage(1, "post rack, before annual acknowledgement");
        Check(ui.Learning.CurrentInstruction.Contains("Read the Annual Review"), "observation cue precedes crop action");
        RoundTrip(1, "access year boundary");
        yield return AcknowledgeResults("access"); Stage(1, "reviewed but no future tree yet");
        yield return LessonShot("02-future-trees");
        var trees = FindObjectsByType<ForestTree>(FindObjectsSortMode.None).Where(t => t.IsLiving && t.CanChop).ToArray();
        ForestTree crop = trees.OrderByDescending(t => t.Diameter).First();
        ui.Marking.Mark(crop, TreeMarkType.CropTree, false);
        Stage(2, "crop tree selected");
        yield return LessonShot("03-selective-release");
        var competitor = trees.Where(t => t != crop && Vector2.Distance(
            new Vector2(t.transform.position.x, t.transform.position.z), new Vector2(crop.transform.position.x, crop.transform.position.z)) <= ForestEcologyController.HegyiCutoffMeters)
            .OrderByDescending(t => t.Diameter).First();
        ui.Marking.Mark(competitor, TreeMarkType.Fell, false);
        Check(manager.AddMarkedTreesToWorkPlan() > 0, "plan existing relevant competitor");
        Stage(3, "release planned; disposition before execution");
        var order = manager.WorkOrders.Last(o => o.IsOpen && o.type == ScenarioWorkType.FellTree);
        int orderId = order.workOrderId;
        Check(manager.RemovePendingOrder(orderId), "cancel pending release without false lesson completion");
        Stage(2, "cancel returns to actionable release guidance");
        ui.Marking.Mark(competitor, TreeMarkType.Fell, false); manager.AddMarkedTreesToWorkPlan();
        order = manager.WorkOrders.Last(o => o.IsOpen && o.type == ScenarioWorkType.FellTree);
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
        yield return AcknowledgeResults("release after N3"); Stage(5, "review unlocks regeneration/protection");
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
        RoundTrip(6, "planting time gate");
        yield return LessonShot("07-establishment-advance");
        yield return AcknowledgeResults("planting year"); Stage(6, "planting-year acknowledgement cannot skip extra year");
        ui.CloseAll(); Check(manager.AdvanceYear(), "later observation year"); Stage(6, "later year still needs review");
        RoundTrip(6, "later observation before acknowledgement");
        yield return LessonShot("07-establishment-review");
        yield return AcknowledgeResults("later observation year"); Stage(7, "ongoing transformation, no false scenario completion");
        Check(manager.Outcome == ScenarioOneOutcome.Active, "teaching path does not replace scenario completion or minimum year");
        yield return LessonShot("08-continue-transformation");
        RoundTrip(7, "ongoing transformation");
        ui.CloseAll(); Check(manager.AdvanceYear() && manager.AdvanceYear(), "multiple advances after teaching path"); Stage(7, "extra advances preserve valid progression");
        Check(goalIds.SequenceEqual(manager.Objectives.Select(o => o.objectiveId)) && targets.SequenceEqual(manager.Objectives.Select(o => o.targetValue)), "all eight goal IDs and targets unchanged");
        Check(ForestSaveData.CurrentVersion == 20, "no save schema change");
        // A later generation can refill an original access corridor. The real rack tool should report maintenance;
        // a completed teaching lesson must not restart. This is a disposable stand-in for a promoted young tree.
        string beforeRegrowth = ScenarioReferenceArchive.WorldHash(saves.CaptureData());
        var originalTree = FindObjectsByType<ForestTree>(FindObjectsSortMode.None).First(t => t.CanChop);
        var regrowth = Instantiate(originalTree.gameObject).GetComponent<ForestTree>();
        typeof(ForestTree).GetField("treeId", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(regrowth, "REGROWTH_TEACHING_CHECK");
        regrowth.SetMark(TreeMarkType.None);
        regrowth.transform.position = new Vector3(0, originalTree.transform.position.y, ui.RackTool.Planner.LayoutLines[0]);
        ui.RackLayout.Update(true);
        Check(!ui.RackLayout.Harvested, "live rack tracker still reports actual later regrowth");
        Stage(7, "later access maintenance does not restart initial lesson");
        Destroy(regrowth.gameObject); yield return Pause(2); ui.RackLayout.Update(true);
        Check(beforeRegrowth == ScenarioReferenceArchive.WorldHash(saves.CaptureData()), "regrowth fixture removed without changing saved world");
    }
}
#endif
