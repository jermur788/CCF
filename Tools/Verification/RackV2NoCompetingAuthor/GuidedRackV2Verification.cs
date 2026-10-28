#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

// G3-DISPOSITION: required V2 replacement for the retained model-0 native-authoring branch.
// Staged together with GuidedOnboardingVerification, SequentialObjectivesVerification and RackUxShots.
public sealed partial class GuidedOnboardingRunner
{
    partial void AddV2Checks(List<IEnumerator> extras) => extras.Add(VerifyV2AuthoringAbsent());
    readonly RackAuthoringNativeInput native = new RackAuthoringNativeInput();
    static bool OldRackCopy(string text) => new[] { "[B]", "Press B", "Use B", "racks (B)", "B extraction rack" }.Any(text.Contains);
    string RackState() => JsonUtility.ToJson(saves.CaptureData().scenarioOne);
    void V2Inert(string before, string route)
    {
        Check(!ui.RackTool.Active && !ui.RackTool.IntroPending && !RackTool.AnyActive
            && !ui.RackTool.HasOrigin && ui.RackTool.Proposal == null, route + " cannot own first-person rack input");
        Check(RackState() == before && manager.PlannedRackCount == 0 && manager.MarkedRackEntranceCount == 0
            && manager.RackObjectCount == 0 && ui.RackTool.ConfirmedCount == 0 && manager.NextRackId == 1
            && !manager.RackPlanConfirmed && manager.RackPlan.Count == 0 && manager.WorkOrders.Count == 0,
            route + " leaves authoritative plan, counters, allocator and all work unchanged");
    }
    IEnumerator V2AimAt(ForestTree tree)
    {
        foreach (Vector3 direction in new[] { Vector3.back, Vector3.forward, Vector3.right, Vector3.left,
            new Vector3(-1,0,-1).normalized, new Vector3(1,0,-1).normalized,
            new Vector3(-1,0,1).normalized, new Vector3(1,0,1).normalized })
        foreach (float distance in new[] { 3f, 2.5f, 2f })
        {
            ui.Player.transform.position = tree.transform.position + direction * distance;
            ui.Player.LookToward(tree.transform.position + Vector3.up * 1.4f);
            Physics.SyncTransforms();
            yield return Pause(10);
            if (ui.Marking.AimedTree == tree && ui.Player.AimedTree == tree) yield break;
        }
        throw new InvalidOperationException("no clear ordinary interaction vantage for " + tree.TreeId);
    }
    IEnumerator VerifyV2AuthoringAbsent()
    {
        Check(manager.RackPlanningV2Active && ui.Ecology.StandGeometryModelVersion == StandGeometryModel.Enlarged80,
            "V2 absence fixture is explicitly rack model 1 and Enlarged80");
        ui.CloseAll(); ui.CloseHelp();
        UnityEngine.Cursor.lockState = CursorLockMode.Locked; UnityEngine.Cursor.visible = false;
        yield return Pause(20);
        string before = RackState();
        yield return native.Send(RackAuthoringNativeInput.Op.Key("b"));
        V2Inert(before, "real OS B (first use)");
        Check(!ui.Help.IsOpen && !Visible(ui.RootElement.Q("hud-rack-panel")), "V2 B opens neither rack intro nor preview panel");
        typeof(RackTool).GetMethod("Request", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(ui.RackTool, null);
        ui.RackTool.Enter();
        Check(!ui.RackTool.SelectOrigin(new RackPoint(0,0)) && !ui.RackTool.Confirm(), "direct entry/origin/confirmation are inert in V2");
        ui.RackTool.ReselectOrigin(); ui.RackTool.Turn(1);
        yield return Pause(); V2Inert(before, "direct tool APIs");
        RackPlanner planner = ui.RackTool.Planner; planner.Refresh();
        RackProposal proposal = planner.Propose(new RackPoint(0, planner.LayoutLines[0]), 0f);
        Check(manager.ConfirmRack(proposal, "forest") == 0, "legacy forest backend still refuses V2");
        V2Inert(before, "legacy backend");

        // Exercise a queued first-use race state explicitly; this is transient UI state, never progress/credit.
        typeof(RackTool).GetField("introPending", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(ui.RackTool, true);
        ui.Help.Show(MenuHelpView.Menu.Rack);
        yield return Pause();
        Check(!OldRackCopy(Text()) && Text().Contains("Plan Racks") && Text().Contains("Stand Map"), "V2 rack help renders map-only guidance");
        yield return Shoot("v2-rack-help");
        yield return native.Send(RackAuthoringNativeInput.Op.Key("Escape"));
        V2Inert(before, "deferred intro close");
        yield return native.Send(RackAuthoringNativeInput.Op.Key("b"), RackAuthoringNativeInput.Op.Click(),
            RackAuthoringNativeInput.Op.Click(3), RackAuthoringNativeInput.Op.Key("Return"),
            RackAuthoringNativeInput.Op.Key("KP_Enter"));
        V2Inert(before, "repeat B / pointer / right-click / Enter / numpad Enter");
        Check(!Visible(ui.RootElement.Q("hud-rack-panel")), "no V2 first-person author panel after former input routes");
        Check(!OldRackCopy(Text()) && Text().Contains("Stand Map"), "V2 walking HUD copy offers Stand Map without B");
        yield return Shoot("v2-walking-no-rack-author");
        ui.Help.Show(MenuHelpView.Menu.WalkingHud); yield return Pause();
        Check(!OldRackCopy(Text()) && Text().Contains("Plan Racks"), "V2 full Walking Help has no obsolete authoring control");
        yield return Shoot("v2-walking-help"); ui.CloseHelp();
        ui.ShowObjectives(); yield return Pause(20);
        foreach (Foldout foldout in ui.RootElement.Query<Foldout>().ToList()) foldout.value = true;
        yield return Pause(20);
        Check(!OldRackCopy(Text()) && Text().Contains("Plan Racks"), "V2 objective and optional rack practice copy contain no first-person authoring instruction");
        yield return Shoot("v2-objective-copy"); ui.CloseAll(); yield return Pause(20);

        ui.CloseHelp(); ui.CloseAll();
        if (ui.Player.IsInspecting) yield return native.Send(RackAuthoringNativeInput.Op.Key("e"));
        ForestTree target = FindObjectsByType<ForestTree>(FindObjectsSortMode.None).Where(t => t.IsLiving && t.CanChop)
            .OrderBy(t => t.TreeId, StringComparer.Ordinal).Skip(100).First();
        yield return V2AimAt(target);
        yield return native.Send(RackAuthoringNativeInput.Op.Key("x"));
        Check(target.IsMarkedForFell && !RackTool.AnyActive, "real OS X still marks an ordinary Fell tree in V2");
        yield return native.Send(RackAuthoringNativeInput.Op.Key("c"));
        Check(target.IsCropTree && !target.IsMarkedForFell, "real OS C still makes exclusive Crop Tree mark in V2");
        yield return native.Send(RackAuthoringNativeInput.Op.Key("e"));
        if (ui.Help.IsOpen) ui.CloseHelp(); yield return Pause();
        Check(ui.Player.IsInspecting && ui.Player.InspectedTree == target && !RackTool.AnyActive,
            "real OS E inspection is not suppressed by former rack input");
        yield return Shoot("v2-ordinary-crop-inspection");
        yield return native.Send(RackAuthoringNativeInput.Op.Key("e"));
        if (ui.Help.IsOpen) ui.CloseHelp();
        yield return native.Send(RackAuthoringNativeInput.Op.Key("c"));
        Check(!target.IsCropTree && !target.IsMarkedForFell, "ordinary marking fixture can be cleared without rack work");
        Check(manager.PlannedRackCount == 0 && manager.WorkOrders.Count == 0, "ordinary inspection/marks fabricate no rack or order");

        yield return native.Send(RackAuthoringNativeInput.Op.Key("m"));
        if (ui.Help.IsOpen) ui.CloseHelp(); yield return Pause(20);
        Check(ui.CurrentScreen == ScenarioOneUiRoot.UiScreen.Map && ui.Map.RackPlanningAvailable
            && Button("Plan Racks") != null && manager.PlannedRackCount == 0 && manager.RackGroups.Count == 0,
            "real OS M reaches Plan Racks immediately without a forest rack");
        yield return Click(ui.RootElement.Q<Button>("map-plan-rack"));
        Check(ui.Map.RackMode, "V2 replacement planning surface is active");
        yield return Click(ui.RootElement.Q<Button>("map-edit-reset"));
        Check(manager.PlannedRackCount == 5 && manager.WorkOrders.Count == 0 && !manager.RackPlanConfirmed,
            "real map reset action creates five authoritative draft racks without work");
        yield return Click(ui.RootElement.Q<Button>("map-rack-plan-confirm"));
        ui.RackLayout.Update(true);
        Check(manager.RackPlanConfirmed && ui.RackLayout.Complete && manager.WorkOrders.Any(o => o.rackId > 0)
            && manager.WorkOrders.All(o => o.rackId > 0 && o.status == ScenarioWorkStatus.Pending),
            "map Confirm Rack Plan creates only authoritative pending rack work");
        Check(ui.Learning.CurrentTopicIndex == 0 && !RackTool.AnyActive, "planning does not fabricate the post-execution lesson");
        yield return Shoot("v2-map-confirmed");
        ui.ShowWorkPlan(); yield return Pause(20); if (ui.Help.IsOpen) ui.CloseHelp(); yield return Pause();
        // The "Show trees" control belongs to the session RackGroups that legacy confirmation creates (model-0 branch, retained in
        // GuidedOnboardingVerification). A confirmed V2 plan makes no such group: its rack-owned jobs are ordinary open fell orders,
        // which the Work Plan lists one per line with the same Sell / Keep / Remove controls. Assert that actual presentation.
        List<ScenarioOneWorkOrder> ownedJobs = manager.WorkOrders.Where(o => o.rackId > 0 && o.IsOpen && ScenarioOneManager.IsHarvestOrder(o)).ToList();
        int openFell = manager.WorkOrders.Count(o => o.IsOpen && ScenarioOneManager.IsHarvestOrder(o));
        Check(manager.RackGroups.Count == 0 && Button("Show trees") == null, "V2 confirmation creates no legacy session rack group or Show trees control");
        List<TextElement> planText = ui.RootElement.Query<TextElement>().ToList().Where(Visible).ToList();
        Check(ownedJobs.Count == openFell && openFell > 0
            && planText.Any(t => t.text.StartsWith("Harvest and salvage job", StringComparison.Ordinal) && t.text.Contains(openFell + " stem(s)")),
            "V2 Work Plan harvest card counts every confirmed rack-owned job", $"jobs={ownedJobs.Count} openFell={openFell}");
        ScenarioOneWorkOrder first = ownedJobs.OrderBy(o => o.workOrderId).First();
        TextElement line = planText.FirstOrDefault(t => t.text.StartsWith(first.targetTreeId + " · ", StringComparison.Ordinal));
        List<Button> rowButtons = line != null ? line.parent.Query<Button>().ToList().Where(Visible).ToList() : new List<Button>();
        Check(line != null && ownedJobs.All(o => planText.Any(t => t.text.StartsWith(o.targetTreeId + " · ", StringComparison.Ordinal))),
            "V2 Work Plan lists every rack-owned job as its own line");
        Check(rowButtons.Any(b => b.text == "Sell" && b.enabledInHierarchy) && rowButtons.Any(b => b.text == "Remove" && b.enabledInHierarchy),
            "V2 rack-owned job line offers the real enabled Sell and Remove controls");
        if (line != null) { line.GetFirstAncestorOfType<ScrollView>()?.ScrollTo(line); yield return Pause(); }
        yield return Shoot("v2-work-plan-rack-jobs"); ui.CloseAll(); yield return Pause();
    }
}
#endif
