using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

// One extra "What you are leaving" row supplied by another feature.
public readonly struct ResidualRow
{
    public readonly string Label, Value, Note;
    public ResidualRow(string label, string value, string note = null) { Label = label; Value = value; Note = note; }
}

// Extension point for rows that depend on systems this packet does not own:
// the Crop Tree release estimate (P2) and wind stability after work (storm
// model). A provider returns nothing until its system is authoritative, so
// no placeholder or the old universal wind label is ever shown.
public interface IWorkPlanResidualRows
{
    IEnumerable<ResidualRow> Rows(ScenarioOneManager manager, ResidualStandResult stand);
}

// Work Plan overview, shown above the job cards: MONEY (what the plan costs
// and earns), WHAT YOU ARE LEAVING (the trees and ground that remain if the
// plan is carried out) and a warning only when the plan would leave too
// little cash to commission another harvest. Review only: nothing here marks,
// selects or approves work; decisions stay in the walked forest.
public static class WorkPlanOverview
{
    public static readonly List<IWorkPlanResidualRows> ExtraRows = new List<IWorkPlanResidualRows>();

    public static void Build(VisualElement parent, ScenarioOneUiRoot ui)
    {
        ScenarioOneManager m = ui.Manager;
        ForestEcologyController eco = ui.Ecology;
        if (m == null || m.Definition == null) return;

        List<ScenarioOneWorkOrder> fell = m.WorkOrders.Where(o => o.IsOpen && o.type == ScenarioWorkType.FellTree
            && string.IsNullOrEmpty(o.validationMessage)).ToList();
        List<ScenarioOneWorkOrder> clearance = m.WorkOrders.Where(o => o.IsOpen && o.type == ScenarioWorkType.RemoveRegeneration
            && o.cellIndex >= 0 && string.IsNullOrEmpty(o.speciesId)).OrderBy(o => o.cellIndex).ToList();
        ScenarioHarvestJob all = m.GetHarvestQuote(false);
        ScenarioHarvestJob approved = m.GetHarvestQuote(true);
        CashOutlook outlook = CashOutlook.Evaluate(OutlookInput(m, all, approved));

        BuildWarning(parent, m, outlook);
        VisualElement columns = UiKit.Box("columns");
        parent.Add(columns);
        BuildMoney(columns, m, all, outlook, fell.Count > 0);
        if (eco != null && eco.Cells != null)
            BuildLeaving(columns, m, eco, fell, clearance, all);
    }

    public static CashOutlookInput OutlookInput(ScenarioOneManager m, ScenarioHarvestJob all, ScenarioHarvestJob approved)
    {
        long approvedOther = 0, pendingOther = 0;
        foreach (ScenarioOneWorkOrder o in m.WorkOrders)
        {
            if (!o.IsOpen || o.type == ScenarioWorkType.FellTree || !string.IsNullOrEmpty(o.validationMessage)) continue;
            if (o.status == ScenarioWorkStatus.Approved) approvedOther += o.estimatedCostCents;
            else pendingOther += o.estimatedCostCents;
        }
        return new CashOutlookInput(m.CashCents, m.Definition.MinimumHarvestJobCents,
            approved.CostCents, approved.RevenueCents, all.CostCents, all.RevenueCents, approvedOther, pendingOther);
    }

    // The consequence, not a prohibition: approving remains the player's choice.
    public static string WarningText(CashOutlook outlook, long minimum)
    {
        string min = UiKit.Money(minimum);
        switch (outlook.State)
        {
            case CashOutlookState.PendingCrossesMinimum:
                return $"If you approve the pending work, expected cash after this year is {UiKit.Money(outlook.ExpectedIfAllApproved)}: less than the "
                    + $"{min} minimum for a harvesting visit. Timber sales are your only income in this scenario, so you might not be able to commission "
                    + "another harvest. Review the plan before approving it.";
            case CashOutlookState.ApprovedCrossesMinimum:
                return $"After the work you have approved, expected cash is {UiKit.Money(outlook.ExpectedAfterApproved)}: less than the {min} minimum "
                    + "for a harvesting visit. Timber sales are your only income in this scenario, so you might not be able to commission another harvest. "
                    + "Approved work can still be cancelled in this plan.";
            case CashOutlookState.BelowMinimumNow:
                return $"Your cash is below the {min} minimum for a harvesting visit, so no harvest can be commissioned. Timber sales are your only "
                    + "income in this scenario. You can still walk, observe, and plant stock you already own with your own time (shelters cost money).";
            default:
                return "";
        }
    }

    private static void BuildWarning(VisualElement parent, ScenarioOneManager m, CashOutlook outlook)
    {
        string text = WarningText(outlook, m.Definition.MinimumHarvestJobCents);
        if (string.IsNullOrEmpty(text)) return;
        VisualElement card = UiKit.Box("card", "workplan-warning");
        card.name = "workplan-cash-warning";
        // Inline, so this packet does not edit the shared stylesheet.
        card.style.borderLeftWidth = 4;
        card.style.borderLeftColor = new Color(240f / 255f, 186f / 255f, 110f / 255f);
        UiKit.Add(card, "⚠ CASH AFTER THIS PLAN", "heading");
        UiKit.Add(card, text, "body");
        parent.Add(card);
    }

    private static void BuildMoney(VisualElement columns, ScenarioOneManager m, ScenarioHarvestJob all, CashOutlook outlook, bool hasHarvest)
    {
        VisualElement card = UiKit.Box("card", "column", "workplan-overview");
        card.style.flexGrow = 1; card.style.flexBasis = 0; card.style.minWidth = 360;
        card.name = "workplan-money";
        UiKit.Add(card, "MONEY", "heading");
        UiKit.Add(card, "Expected for all open jobs; re-quoted when the year is resolved.", "muted");
        long other = m.WorkOrders.Where(o => o.IsOpen && o.type != ScenarioWorkType.FellTree && string.IsNullOrEmpty(o.validationMessage))
            .Sum(o => o.estimatedCostCents);
        long minimumPart = all.Resolution != null ? all.Resolution.Quote.Costs.MinimumJobAdjustmentCents : 0;
        if (hasHarvest)
        {
            UiKit.Line(card, "Timber sales", UiKit.SignedMoney(all.RevenueCents), "money");
            UiKit.Line(card, "Harvesting work" + (minimumPart > 0 ? $" (incl. small-job minimum {UiKit.Money(minimumPart)})" : ""),
                UiKit.SignedMoney(-all.CostCents), "money-negative");
        }
        if (other > 0)
            UiKit.Line(card, "Planting, pruning and clearance", UiKit.SignedMoney(-other), "money-negative");
        long net = all.RevenueCents - all.CostCents - other;
        UiKit.Line(card, "Net for planned work", UiKit.SignedMoney(net), net >= 0 ? "money" : "money-negative");
        UiKit.Line(card, "Cash now", UiKit.Money(m.CashCents));
        UiKit.Line(card, "Expected cash after approved work", UiKit.Money(outlook.ExpectedAfterApproved));
        if (outlook.HasPendingWork)
            UiKit.Line(card, "… if all pending work is approved", UiKit.Money(outlook.ExpectedIfAllApproved));
        if (hasHarvest || minimumPart > 0)
            UiKit.Add(card, $"Harvesting has a minimum charge per contractor visit ({UiKit.Money(m.Definition.MinimumHarvestJobCents)} in this scenario). "
                + "All felling approved for the same year shares one visit, so one visit costs less per tree than several small visits in different years. "
                + "Planting, pruning and clearance are charged by time, with no minimum.", "muted");
        if (outlook.State == CashOutlookState.Comfortable)
            UiKit.Add(card, $"You could spend {UiKit.Money(outlook.Headroom)} more before expected cash falls below one harvesting visit.", "faint");
        columns.Add(card);
    }

    private static void BuildLeaving(VisualElement columns, ScenarioOneManager m, ForestEcologyController eco,
        List<ScenarioOneWorkOrder> fell, List<ScenarioOneWorkOrder> clearance, ScenarioHarvestJob all)
    {
        VisualElement card = UiKit.Box("card", "column", "workplan-overview");
        card.style.flexGrow = 1; card.style.flexBasis = 0; card.style.minWidth = 360;
        card.name = "workplan-leaving";
        UiKit.Add(card, "WHAT YOU ARE LEAVING", "heading");
        UiKit.Add(card, "If all open work is carried out. An estimate before this year's growth; Annual Review shows what actually happened.", "muted");

        ResidualStandResult stand = ResidualStand.Compute(SceneTrees(eco, fell), eco.StandAreaHectares, eco.CellsPerAxis, eco.CellSizeMeters);
        if (stand.PlannedFells == 0)
            UiKit.Line(card, "Trees standing", $"{stand.TreesBefore} (no felling planned)");
        else
        {
            UiKit.Line(card, "Trees standing", $"{stand.TreesBefore} → {stand.TreesAfter} (−{stand.PlannedFells})");
            UiKit.Line(card, "Basal area", $"{UiKit.F(stand.BasalAreaBefore, "0.0")} → {UiKit.F(stand.BasalAreaAfter, "0.0")} m²/ha");
            UiKit.Line(card, "Standing stem volume", $"{UiKit.F(stand.VolumeBefore, "0.0")} → {UiKit.F(stand.VolumeAfter, "0.0")} m³");
            UiKit.Line(card, "Removal across the stand",
                $"{stand.CellsTouched} cell{(stand.CellsTouched == 1 ? "" : "s")} · largest opening {stand.LargestOpeningCells} cell{(stand.LargestOpeningCells == 1 ? "" : "s")} ({UiKit.F(stand.LargestOpeningM2, "0")} m²)");
            string pattern = ResidualStand.PatternSentence(stand);
            if (!string.IsNullOrEmpty(pattern)) UiKit.Add(card, pattern, "muted");
        }
        UiKit.Line(card, "Crop Trees kept", stand.CropTrees > 0 ? $"{stand.CropTreesKept} of {stand.CropTrees}" : "none chosen");
        if (stand.SeedTreesBefore > 0)
            UiKit.Line(card, "Trees producing seed", $"{stand.SeedTreesBefore} → {stand.SeedTreesAfter}");
        float onSite = m.DeadwoodRecords.Sum(d => d.remainingVolumeM3);
        float planned = all.DeadwoodVolumeCm3 / 1000000f;
        if (planned > 0f || onSite > 0f)
            UiKit.Line(card, "Fallen deadwood", planned > 0f
                ? $"+{UiKit.F(planned, "0.00")} m³ left from felling · {UiKit.F(onSite, "0.00")} m³ on site now"
                : $"{UiKit.F(onSite, "0.00")} m³ on site");
        foreach (IWorkPlanResidualRows provider in ExtraRows)
            foreach (ResidualRow row in provider.Rows(m, stand))
            {
                UiKit.Line(card, row.Label, row.Value);
                if (!string.IsNullOrEmpty(row.Note)) UiKit.Add(card, row.Note, "muted");
            }
        BuildClearance(card, m, eco, clearance);
        columns.Add(card);
    }

    // Each planned clearance: what it removes, and (Model 2) the competing cover there.
    private static void BuildClearance(VisualElement card, ScenarioOneManager m, ForestEcologyController eco, List<ScenarioOneWorkOrder> clearance)
    {
        if (clearance.Count == 0) return;
        bool model2 = eco.RegenerationModelVersion == RegenerationModel.Competition;
        float escape = m.CompetitionCalibration != null ? m.CompetitionCalibration.EscapeHeight : 1.5f;
        foreach (ScenarioOneWorkOrder order in clearance)
        {
            ClearanceConsequence c = Consequence(m, eco, order.cellIndex, escape);
            string cell = UiKit.CellLabel(order.cellIndex, eco.CellsPerAxis);
            UiKit.Line(card, $"Clearance {cell}", c.YoungTreeGroups + c.PlantedSaplings == 0 ? "no young trees inside"
                : $"removes {c.YoungTreeGroups} young-tree group{(c.YoungTreeGroups == 1 ? "" : "s")}, {c.PlantedSaplings} planted sapling{(c.PlantedSaplings == 1 ? "" : "s")}");
            if (model2)
                UiKit.Add(card, $"Bramble {UiKit.F(c.Bramble, "0.00")} · bracken {UiKit.F(c.Bracken, "0.00")} cover here, removed this year and able to return."
                    + (c.TallerThanEscape > 0 ? $" {c.TallerThanEscape} of the young trees removed are already taller than {UiKit.F(escape, "0.0")} m, above the reach of these plants." : ""), "muted");
        }
        UiKit.Add(card, LearningObjectivesView.ClearanceExplanation(eco.RegenerationModelVersion), "muted");
    }

    public struct ClearanceConsequence
    {
        public int YoungTreeGroups, PlantedSaplings, TallerThanEscape;
        public float Bramble, Bracken;
    }

    public static ClearanceConsequence Consequence(ScenarioOneManager m, ForestEcologyController eco, int cellIndex, float escapeHeight)
    {
        ClearanceTargets targets = m.QueryClearance(ClearanceFootprint.Cell(eco, cellIndex));
        var c = new ClearanceConsequence
        {
            YoungTreeGroups = targets.Cohorts.Count,
            PlantedSaplings = targets.Juveniles.Count,
            TallerThanEscape = targets.Cohorts.Count(t => t.Cohort != null && t.Cohort.Height >= escapeHeight)
                + targets.Juveniles.Count(j => j.heightMeters >= escapeHeight)
        };
        ScenarioUnderstoreyCell state = m.UnderstoreyCells.FirstOrDefault(u => u != null && u.cellIndex == cellIndex);
        if (state != null) { c.Bramble = state.brambleCover; c.Bracken = state.brackenCover; }
        return c;
    }

    // Living trees in id order, with the open Fell orders marked as planned removals.
    public static List<ResidualTree> SceneTrees(ForestEcologyController eco, IEnumerable<ScenarioOneWorkOrder> fell)
    {
        var planned = new HashSet<string>(fell.Select(o => o.targetTreeId), System.StringComparer.Ordinal);
        return Object.FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
            .Where(t => t != null && t.IsLiving)
            .OrderBy(t => t.TreeId, System.StringComparer.Ordinal)
            .Select(t => new ResidualTree(t.TreeId, eco.GetCellIndex(t.transform.position), t.Diameter, t.BiologicalStemVolumeM3,
                planned.Contains(t.TreeId), t.IsCropTree, eco.GetSeedPotential(t) > 0f))
            .ToList();
    }
}
