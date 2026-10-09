using System.Collections.Generic;
using System.Linq;
using CCF.Forestry.WorkEconomy;
using UnityEngine;
using UnityEngine.UIElements;

// Screen 5 — annual review: WORK DONE, MONEY, FOREST, then compact trends and
// objectives. Built from the recorded annual report, management events and
// ecological snapshots; it adds no causal claim the simulation does not make.
public sealed class AnnualReviewView
{
    private readonly ScenarioOneUiRoot ui;
    public VisualElement Root { get; }
    private readonly Label title;
    private readonly ScrollView scroll;
    private readonly VisualElement acknowledgement;
    private string shownKey = "";

    public AnnualReviewView(ScenarioOneUiRoot ui)
    {
        this.ui = ui;
        Root = UiKit.Box("layer", "modal-backdrop");
        VisualElement modal = UiKit.Box("panel", "modal");
        Root.Add(modal);
        VisualElement header = UiKit.Box("modal-header");
        title = UiKit.Add(header, "", "title");
        VisualElement buttons = UiKit.Box("row");
        buttons.Add(UiKit.Button("Help [F1]", () => ui.ShowHelp(MenuHelpView.Menu.AnnualReview)));
        buttons.Add(UiKit.Button("Objectives [O]", () => ui.ShowObjectives()));
        buttons.Add(UiKit.Button("Open Work Plan", () => ui.ShowWorkPlan()));
        buttons.Add(UiKit.Button("Walk the forest [Esc]", () => ui.CloseAll(), true, "btn-primary"));
        header.Add(buttons);
        modal.Add(header);
        scroll = new ScrollView(ScrollViewMode.Vertical);
        scroll.style.flexGrow = 1;
        modal.Add(scroll);
        acknowledgement = UiKit.Box("modal-footer");
        UiKit.Add(acknowledgement, "Read WORK DONE, MONEY and FOREST before continuing.", "body");
        var acknowledge = UiKit.Button("I've read the annual results", () => ui.AcknowledgeAnnualReview(), true, "btn-primary");
        acknowledge.name = "annual-review-acknowledge";
        acknowledgement.Add(acknowledge);
        modal.Add(acknowledgement);
    }

    public void Refresh(bool force)
    {
        ScenarioOneManager m = ui.Manager;
        string key = m.CurrentEcologicalYear + "|" + m.AnnualReports.Count + "|" + m.CashCents + "|" + m.AnnualReviewSeen;
        if (!force && key == shownKey)
            return;
        shownKey = key;
        scroll.Clear();
        acknowledgement.style.display = ui.NeedsAnnualReview ? DisplayStyle.Flex : DisplayStyle.None;

        ScenarioAnnualReport report = m.AnnualReports.Count > 0 ? m.AnnualReports[m.AnnualReports.Count - 1] : null;
        title.text = report != null ? $"Annual review · Year {report.year} complete" : "Annual review";
        if (report == null)
        {
            UiKit.Add(scroll, "Advance a year from the Work Plan to see the first annual results.", "body");
            BuildObjectives(m);
            return;
        }

        VisualElement columns = UiKit.Box("columns");
        scroll.Add(columns);
        BuildWork(m, report, Column(columns, "Work done"));
        BuildMoney(m, report, Column(columns, "Money"));
        BuildForest(m, report, Column(columns, "Forest"));
        BuildStorm(m, report);
        BuildTrends(m);
        BuildObjectives(m);
        if (m.CenturyReview != null)
            BuildCentury(m);
    }

    private void BuildStorm(ScenarioOneManager manager, ScenarioAnnualReport report)
    {
        StormDamageSummary summary = manager.StormDamageInYear(report.year);
        if (summary == null) return;
        var card = UiKit.Box("card"); card.name = "annual-storm-review"; scroll.Add(card);
        UiKit.Add(card, "STORM AND WINDTHROW", "heading");
        string severity = summary.Event.severity < .04f ? "Light" : summary.Event.severity < .12f ? "Moderate" : "Severe";
        UiKit.Add(card, $"Year {summary.Event.year} · {severity} modelled event. These are game severity bands, not measured wind speeds.", "body");
        UiKit.Add(card, $"{summary.TreesLost} trees lost, including {summary.Event.cropTreesLost} Crop Trees · {UiKit.F(summary.OriginalVolumeM3, "0.00")} m³ of original fallen stem material.", "body");
        UiKit.Add(card, $"Damage touches {summary.AffectedCells} map cells. This cell footprint is approximate; it does not mean every affected cell is fully open.", "muted");
        UiKit.Add(card, $"{UiKit.F(summary.DeadwoodRemainingM3, "0.00")} m³ remains as deadwood · {UiKit.F(summary.SalvagedVolumeM3, "0.00")} m³ salvaged since this event.", "body");
        UiKit.Add(card, "Fallen trees stop contributing to canopy and seed rain. Openings increase light and recent exposure; bramble and bracken can respond, affecting small young trees through the existing competition model. Inspect the changed cells before choosing work.", "body");
        UiKit.Add(card, "Salvage is optional. Aim at an eligible fallen stem and press X to add it to the Work Plan; choose Sell or Keep there. Unselected stems remain deadwood, and a small salvage visit may cost more than its timber earns.", "body");
        if (summary.WaypointCell >= 0) card.Add(UiKit.Button("Mark storm damage on the map", () => ui.ShowStormWaypoint(summary.WaypointCell)));
    }

    private static VisualElement Column(VisualElement parent, string heading)
    {
        VisualElement column = UiKit.Box("card", "column");
        UiKit.Add(column, heading.ToUpperInvariant(), "heading");
        parent.Add(column);
        return column;
    }

    private static void BuildWork(ScenarioOneManager m, ScenarioAnnualReport report, VisualElement c)
    {
        List<ScenarioManagementEvent> resolved = m.ManagementEvents
            .Where(e => e.year == report.year && e.eventType == ScenarioManagementEventType.WorkResolved).ToList();
        List<ScenarioManagementEvent> done = resolved.Where(e => e.outcome == ScenarioManagementOutcome.Succeeded).ToList();
        Dictionary<int, ScenarioOneWorkOrder> orders = m.WorkOrders.ToDictionary(o => o.workOrderId);
        int felled = done.Count(e => e.taskType == ScenarioWorkType.FellTree);
        int salvaged = done.Count(e => e.taskType == ScenarioWorkType.SalvageDeadwood);
        if (salvaged > 0) UiKit.Add(c, $"{salvaged} windthrow stem(s) salvaged (contractor)", "body");
        if (felled > 0) UiKit.Add(c, $"{felled} tree(s) thinned (contractor)", "body");
        var planted = done.Where(e => e.taskType == ScenarioWorkType.PlantJuvenile).ToList();
        if (planted.Count > 0)
        {
            int sheltered = planted.Count(e => orders.TryGetValue(e.workOrderId, out var o) && o.installShelter);
            int byOwner = planted.Count(e => e.executionMethod == WorkExecutionMethod.LandownerSimulated);
            UiKit.Add(c, $"{planted.Count} planted ({string.Join(", ", planted.GroupBy(e => e.speciesId).Select(g => $"{g.Count()} {ScenarioOneUiFacts.SpeciesName(g.Key)}"))}), "
                + $"{sheltered} with shelters · {byOwner} by you, {planted.Count - byOwner} by contractor", "body");
        }
        int pruned = done.Count(e => e.taskType == ScenarioWorkType.PruneTree);
        if (pruned > 0) UiKit.Add(c, $"{pruned} crop tree(s) pruned (contractor)", "body");
        if (report.regenerationRemovalTasks > 0) UiKit.Add(c, $"{report.regenerationRemovalTasks} regeneration cohort(s) removed", "body");
        if (felled + salvaged + planted.Count + pruned + report.regenerationRemovalTasks == 0) UiKit.Add(c, "No work was resolved this year.", "body");
        var failed = resolved.Where(e => e.outcome != ScenarioManagementOutcome.Succeeded).ToList();
        UiKit.Add(c, failed.Count == 0 ? "No tasks failed." : $"{failed.Count} task(s) failed:", failed.Count == 0 ? "muted" : "body");
        foreach (ScenarioManagementEvent e in failed.Take(5))
            UiKit.Add(c, $"· {e.taskType}: {e.failureReason}", "muted");
    }

    private static void BuildMoney(ScenarioOneManager m, ScenarioAnnualReport report, VisualElement c)
    {
        var events = m.ManagementEvents.Where(e => e.year == report.year && e.eventType == ScenarioManagementEventType.WorkResolved
            && e.outcome == ScenarioManagementOutcome.Succeeded).ToList();
        long harvestCost = events.Where(e => e.taskType == ScenarioWorkType.FellTree || e.taskType == ScenarioWorkType.SalvageDeadwood).Sum(e => e.contractorCostCents);
        long materials = events.Sum(e => e.stockCostCents);
        foreach (ScenarioTimberSale sale in report.timberSales ?? new List<ScenarioTimberSale>())
            UiKit.Line(c, $"Timber sold · {sale.assortment} ({UiKit.M3(sale.soldVolumeCm3)})", UiKit.SignedMoney(sale.revenueCents), "money");
        if (harvestCost > 0)
            UiKit.Line(c, "Harvesting work", UiKit.SignedMoney(-(harvestCost - report.harvestMinimumAdjustmentCents)), "money-negative");
        if (report.harvestMinimumAdjustmentCents > 0)
            UiKit.Line(c, "Small-job minimum", UiKit.SignedMoney(-report.harvestMinimumAdjustmentCents), "money-negative");
        long otherWork = report.contractorCostCents - harvestCost;
        if (otherWork > 0)
            UiKit.Line(c, "Planting, pruning and other contractor work", UiKit.SignedMoney(-otherWork), "money-negative");
        if (materials > 0)
            UiKit.Line(c, "Tree shelters (material)", UiKit.SignedMoney(-materials), "money-negative");
        UiKit.Line(c, "Closing cash", UiKit.Money(report.closingCashCents), "money");
        UiKit.Add(c, $"Your time used: {ScenarioOneManager.FormatMinutes(report.ownerMinutes)} of {ScenarioOneManager.FormatMinutes(m.OwnerMinutesPerYear)}", "muted");
        UiKit.Add(c, "Saplings are paid for when bought from the nursery.", "faint");
    }

    private static void BuildForest(ScenarioOneManager m, ScenarioAnnualReport report, VisualElement c)
    {
        ForestEcologyController eco = Object.FindFirstObjectByType<ForestEcologyController>();
        var snaps = m.EcologicalSnapshots;
        ScenarioEcologicalSnapshot current = snaps.Count > 0 ? snaps[snaps.Count - 1] : null;
        ScenarioEcologicalSnapshot previous = snaps.Count > 1 ? snaps[snaps.Count - 2] : null;
        if (current != null)
            foreach (string line in ScenarioEcologyReviewLines.Lines(previous, current, eco, m.PlantedJuveniles))
            {
                // Browsing and protection are rebuilt below without per-leader
                // probabilities or unimplemented fencing.
                if (line.StartsWith("Browsing pressure") || line.StartsWith("Protected juveniles")) continue;
                UiKit.Add(c, line, "body");
            }
        if (eco != null && eco.Browsing.BackgroundPressure > 0f)
        {
            var recorded = m.PlantedJuveniles.Where(j => j.lastBrowseAssessmentYear == eco.EcologicalYear && eco.EcologicalYear > 0).ToList();
            UiKit.Add(c, $"Browsing pressure {BrowsingConditions.PressureBand(eco.Browsing.BackgroundPressure)}"
                + (recorded.Count > 0 ? $": {recorded.Count(j => j.lastYearBrowsed)} of {recorded.Count} planted juveniles browsed last year." : "."), "body");
            int upcoming = eco.EcologicalYear + 1, sheltered = 0;
            foreach (PlantedJuvenile j in m.PlantedJuveniles.Where(j => j.alive && string.IsNullOrEmpty(j.promotedTreeId)))
                if (eco.Browsing.ProtectionAt(new Vector2(j.position.x, j.position.z), upcoming, out _) == BrowseProtectionState.EffectiveShelter)
                    sheltered++;
            if (sheltered > 0) UiKit.Add(c, $"{sheltered} planted juvenile(s) protected by shelters.", "body");
        }

        UiKit.Add(c, "An opening creates an opportunity, not guaranteed regeneration. Walk back to inspect what remains and how the forest responds; reassess before further work.", "muted");
        UiKit.Add(c, "Retained material", "heading");
        UiKit.Line(c, "Retained usable timber (this year)", UiKit.F(report.keptForUseVolumeM3, "0.00") + " m³");
        UiKit.Line(c, "Retained usable timber (stock)", UiKit.F(m.RetainedTimberM3, "0.00") + " m³");
        UiKit.Line(c, "Retained deadwood (new this year)", UiKit.F(report.deadwoodCreatedM3, "0.00") + " m³");
        if (current != null)
            UiKit.Line(c, "Fallen deadwood on site", $"{current.deadwoodCount} log(s) · {UiKit.F(current.deadwoodVolumeM3, "0.00")} m³");
    }

    private void BuildTrends(ScenarioOneManager m)
    {
        VisualElement card = UiKit.Box("card");
        UiKit.Add(card, "TRENDS", "heading");
        VisualElement row = UiKit.Box("columns");
        card.Add(row);
        var reports = m.AnnualReports.Skip(System.Math.Max(0, m.AnnualReports.Count - 12)).ToList();
        row.Add(Bars("Cash at year end", reports.Select(r => ((float)r.closingCashCents, $"Y{r.year}")).ToList(),
            v => UiKit.Money((long)v)));
        var snaps = m.EcologicalSnapshots.Skip(System.Math.Max(0, m.EcologicalSnapshots.Count - 12)).ToList();
        row.Add(Bars($"Canopy cover (objective ≥ {UiKit.F(m.Definition.MinimumMeanCanopy, "0.00")})", snaps.Select(s => (s.meanCanopy, $"Y{s.year}")).ToList(),
            v => UiKit.F(v, "0.00")));
        scroll.Add(card);
    }

    // Compact bar strip: each bar carries its value as text, not colour alone.
    private static VisualElement Bars(string heading, List<(float value, string label)> points, System.Func<float, string> format)
    {
        VisualElement box = UiKit.Box("column");
        UiKit.Add(box, heading, "body");
        if (points.Count == 0) { UiKit.Add(box, "—", "muted"); return box; }
        float max = Mathf.Max(0.0001f, points.Max(p => p.value));
        VisualElement strip = UiKit.Box("row");
        strip.style.alignItems = Align.FlexEnd;
        strip.style.height = 70;
        foreach ((float value, string label) in points)
        {
            VisualElement col = new VisualElement();
            col.style.alignItems = Align.Center;
            col.style.marginRight = 4;
            col.style.justifyContent = Justify.FlexEnd;
            col.style.height = 70;
            var bar = new VisualElement();
            bar.style.width = 16;
            bar.style.height = Mathf.Max(2f, 46f * Mathf.Max(0f, value) / max);
            bar.style.backgroundColor = new Color(0.55f, 0.75f, 0.5f, 0.9f);
            col.Add(bar);
            col.Add(UiKit.Text(label, "faint"));
            strip.Add(col);
        }
        box.Add(strip);
        UiKit.Add(box, $"Latest: {format(points.Last().value)}", "muted");
        return box;
    }

    private void BuildObjectives(ScenarioOneManager m)
    {
        var objectives = m.Objectives;
        VisualElement card = UiKit.Box("card");
        UiKit.Add(card, $"OBJECTIVES {objectives.Count(o => o.achieved)} OF {objectives.Count}", "heading");
        foreach (ScenarioObjectiveResult o in objectives)
            UiKit.Add(card, ScenarioOneUiFacts.ObjectiveLine(o), "body");
        UiKit.Add(card, m.Outcome == ScenarioOneOutcome.Completed ? $"Scenario objectives met in year {m.OutcomeYear}. Continue to the Century Review (year {m.CenturyReviewYear})."
            : m.Outcome == ScenarioOneOutcome.Failed ? m.OutcomeReason : m.TutorialHint, "muted");
        scroll.Add(card);
    }

    private void BuildCentury(ScenarioOneManager m)
    {
        ScenarioCenturyReview century = m.CenturyReview;
        VisualElement card = UiKit.Box("card");
        UiKit.Add(card, $"CENTURY REVIEW — YEAR {century.year}", "heading");
        UiKit.Add(card, $"Outcome: {century.outcome}. Compared with the frozen Reference Future, not an optimal score or prescription.", "body");
        foreach (ScenarioObjectiveResult c in century.referenceComparisons)
            UiKit.Add(card, $"{ScenarioOneUiFacts.ObjectiveName(c)}: yours {c.currentValue:0.##} · reference {c.targetValue:0.##}", "body");
        scroll.Add(card);
    }
}
