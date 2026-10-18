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
    private readonly Button earlierYear, laterYear;
    private string shownKey = "";
    // The footer's first-review wording, captured once so the completion guidance can replace it and then give it back.
    private string defaultAcknowledgementPrompt;
    // The year being read. -1 follows the latest year; a past year is a drill-down into that year's recorded review.
    private int viewYear = -1;
    private int lastLatestYear = -1;

    public AnnualReviewView(ScenarioOneUiRoot ui)
    {
        this.ui = ui;
        Root = UiKit.Box("layer", "modal-backdrop");
        UiKit.MakeModal(Root);
        VisualElement modal = UiKit.Box("panel", "modal");
        Root.Add(modal);
        VisualElement header = UiKit.Box("modal-header");
        title = UiKit.Add(header, "", "title");
        VisualElement buttons = UiKit.Box("row");
        earlierYear = UiKit.Button("◀ Earlier year", () => StepYear(-1));
        laterYear = UiKit.Button("Later year ▶", () => StepYear(1));
        buttons.Add(earlierYear);
        buttons.Add(laterYear);
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
        UiKit.Add(acknowledgement, MenuHelpView.AnnualReviewReadPrompt, "body");
        var acknowledge = UiKit.Button("I've read the annual results", () => ui.AcknowledgeAnnualReview(), true, "btn-primary");
        acknowledge.name = "annual-review-acknowledge";
        acknowledgement.Add(acknowledge);
        modal.Add(acknowledgement);
    }

    // Opening the review starts on the newest year; a past year is only ever a deliberate drill-down.
    public void ShowLatest() => viewYear = -1;

    public void Refresh(bool force)
    {
        ScenarioOneManager m = ui.Manager;
        ScenarioAnnualReport latest = m.AnnualReports.Count > 0 ? m.AnnualReports[m.AnnualReports.Count - 1] : null;
        // A new year always opens on the newest review; a past year is chosen with the year buttons.
        if (latest != null && latest.year != lastLatestYear) { lastLatestYear = latest.year; viewYear = -1; }
        ScenarioAnnualReport report = latest;
        if (viewYear >= 0)
        {
            report = m.AnnualReports.FirstOrDefault(r => r.year == viewYear);
            if (report == null) { report = latest; viewYear = -1; }
        }
        bool historical = report != null && latest != null && report.year != latest.year;
        string key = m.CurrentEcologicalYear + "|" + m.AnnualReports.Count + "|" + m.CashCents + "|" + m.AnnualReviewSeen + "|" + viewYear + "|" + m.ManagementEvents.Count;
        if (!force && key == shownKey)
            return;
        shownKey = key;
        scroll.Clear();
        acknowledgement.style.display = (ui.NeedsAnnualReview || m.NeedsCompletionReviewAcknowledgement) && !historical ? DisplayStyle.Flex : DisplayStyle.None;
        // Completion guidance only: after the first review the footer says why this acknowledgement matters. The first review keeps its wording.
        Label acknowledgementPrompt = acknowledgement.Q<Label>();
        if (acknowledgementPrompt != null)
        {
            if (defaultAcknowledgementPrompt == null) defaultAcknowledgementPrompt = acknowledgementPrompt.text;
            acknowledgementPrompt.text = !ui.NeedsAnnualReview && m.CompletionGuidance == CompletionGuidanceState.AcknowledgeReview
                ? ScenarioOneCompletionGuidance.GuidanceText(CompletionGuidanceState.AcknowledgeReview) : defaultAcknowledgementPrompt;
        }
        earlierYear.SetEnabled(report != null && m.AnnualReports.Any(r => r.year < report.year));
        laterYear.SetEnabled(historical);
        title.text = report == null ? "Annual review"
            : historical ? $"Annual review · Year {report.year} (earlier year; latest is Year {latest.year})"
            : $"Annual review · Year {report.year} complete";
        if (report == null)
        {
            UiKit.Add(scroll, "Advance a year from the Work Plan to see the first annual results.", "body");
            BuildObjectives(m);
            return;
        }

        IReadOnlyList<InterventionSummary> interventions = m.Interventions;
        // The completion summary opens the review of the year the objectives were met (not year 100).
        CompletionSummary completion = m.CompletionReview;
        if (completion != null && report.year == m.OutcomeYear)
            BuildCompletion(m, completion);
        // After the first intervention, say what to look for. A prompt, not a gate: Advance is never blocked by it.
        if (interventions.Count == 1 && report.year == interventions[0].Year)
            BuildFirstInterventionPrompt(interventions[0]);

        VisualElement columns = UiKit.Box("columns");
        scroll.Add(columns);
        BuildWork(m, report, Column(columns, "Work done"));
        BuildMoney(m, report, Column(columns, "Money"));
        BuildForest(m, report, Column(columns, "Forest"), historical);
        BuildStorm(m, report);
        BuildHistory(m, interventions, completion, report);
        BuildTrends(m);
        BuildObjectives(m, historical);
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
        UiKit.Add(card, $"Damage touches {summary.AffectedCells} map squares. This footprint is approximate; it does not mean every affected square is fully open.", "muted");
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
            UiKit.Add(c, $"· {e.taskType}: {ScenarioOneUiFacts.PlainText(e.failureReason)}", "muted");
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

    private static void BuildForest(ScenarioOneManager m, ScenarioAnnualReport report, VisualElement c, bool historical)
    {
        // A past year is described by the snapshots recorded then. The live-state lines (opened cells now, browsing now,
        // shelters now) would describe today's forest under an old date, so they are left out.
        ForestEcologyController eco = historical ? null : Object.FindFirstObjectByType<ForestEcologyController>();
        var snaps = m.EcologicalSnapshots;
        ScenarioEcologicalSnapshot current = snaps.FirstOrDefault(snap => snap.year == report.year) ?? (snaps.Count > 0 ? snaps[snaps.Count - 1] : null);
        ScenarioEcologicalSnapshot previous = current == null ? null : snaps.LastOrDefault(snap => snap.year < current.year);
        if (current != null)
            foreach (string line in ScenarioEcologyReviewLines.Lines(previous, current, eco, historical ? null : m.PlantedJuveniles))
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
        if (!historical)
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

    private void BuildObjectives(ScenarioOneManager m, bool historical = false)
    {
        var objectives = m.Objectives;
        VisualElement card = UiKit.Box("card");
        UiKit.Add(card, $"OBJECTIVES {objectives.Count(o => o.achieved)} OF {objectives.Count}" + (historical ? " (current state)" : ""), "heading");
        foreach (ScenarioObjectiveResult o in objectives)
            UiKit.Add(card, ScenarioOneUiFacts.ObjectiveLine(o), "body");
        UiKit.Add(card, m.Outcome == ScenarioOneOutcome.Completed ? $"Scenario objectives met in year {m.OutcomeYear}. Continue to the Century Review (year {m.CenturyReviewYear})."
            : m.Outcome == ScenarioOneOutcome.Failed ? m.OutcomeReason : m.TutorialHint, "muted");
        scroll.Add(card);
    }

    private void StepYear(int direction)
    {
        ScenarioOneManager m = ui.Manager;
        var years = m.AnnualReports.Select(r => r.year).OrderBy(y => y).ToList();
        if (years.Count == 0) return;
        int current = viewYear >= 0 ? years.IndexOf(viewYear) : years.Count - 1;
        if (current < 0) current = years.Count - 1;
        int next = Mathf.Clamp(current + direction, 0, years.Count - 1);
        viewYear = next == years.Count - 1 ? -1 : years[next];
        Refresh(true);
    }

    private static HistoryTextContext TextContext() => new HistoryTextContext
    {
        Money = UiKit.Money, SignedMoney = UiKit.SignedMoney, SpeciesName = ScenarioOneUiFacts.SpeciesName
    };

    private void BuildFirstInterventionPrompt(InterventionSummary first)
    {
        VisualElement card = UiKit.Box("card");
        card.name = "annual-first-intervention";
        UiKit.Add(card, "YOUR FIRST INTERVENTION", "heading");
        UiKit.Add(card, ManagementHistoryText.FirstInterventionPrompt(first), "body");
        scroll.Add(card);
    }

    // One block per resolved work plan, built from the persisted records on demand (nothing here is saved).
    private void BuildHistory(ScenarioOneManager m, IReadOnlyList<InterventionSummary> interventions, CompletionSummary completion, ScenarioAnnualReport shown)
    {
        HistoryTextContext text = TextContext();
        VisualElement card = UiKit.Box("card");
        card.name = "management-history";
        UiKit.Add(card, $"MANAGEMENT HISTORY · {interventions.Count} intervention{(interventions.Count == 1 ? "" : "s")}", "heading");
        UiKit.Add(card, "An intervention is a work plan that was carried out in one year: felling, pruning, planting, protection or other commissioned work.", "muted");
        if (interventions.Count == 0)
            UiKit.Add(card, "None yet. Inspect trees, mark them, plan the work in the Work Plan [Tab], approve it and advance a year.", "body");
        foreach (InterventionSummary i in interventions)
        {
            VisualElement block = UiKit.Box("card");
            VisualElement head = UiKit.Box("row");
            UiKit.Add(head, ManagementHistoryText.Title(i), "heading");
            InterventionSummary captured = i;
            head.Add(UiKit.Button(i.Year == shown.year ? "Shown above" : $"Read year {i.Year}", () => { viewYear = captured.Year; Refresh(true); }, i.Year != shown.year));
            block.Add(head);
            UiKit.Add(block, ManagementHistoryText.Work(i, text), "body");
            UiKit.Add(block, ManagementHistoryText.Money(i, text), "body");
            UiKit.Add(block, ManagementHistoryText.DuringPlan(i), "muted");
            UiKit.Add(block, ManagementHistoryText.Since(i, text), "muted");
            card.Add(block);
        }
        if (interventions.Count > 0) UiKit.Add(card, ManagementHistoryText.NotCausalNote, "faint");
        IReadOnlyList<StaleOrderNote> stale = m.StaleOrders;
        if (stale.Count > 0)
            UiKit.Add(card, $"{stale.Count} open order{(stale.Count == 1 ? "" : "s")} need attention. Open the Work Plan [Tab] to review {(stale.Count == 1 ? "it" : "them")}.", "body");
        scroll.Add(card);
    }

    private void BuildCompletion(ScenarioOneManager m, CompletionSummary completion)
    {
        HistoryTextContext text = TextContext();
        VisualElement card = UiKit.Box("card");
        card.name = "completion-review";
        UiKit.Add(card, $"SCENARIO ONE COMPLETE · YEAR {completion.Year}", "heading");
        foreach (string line in ManagementHistoryText.Completion(completion, text))
            UiKit.Add(card, line, line.StartsWith("  ") ? "muted" : "body");
        UiKit.Add(card, "Objectives met", "heading");
        foreach (ScenarioObjectiveResult o in m.Objectives)
            UiKit.Add(card, ScenarioOneUiFacts.ObjectiveLine(o), "body");
        scroll.Add(card);
    }

    private void BuildCentury(ScenarioOneManager m)
    {
        ScenarioCenturyReview century = m.CenturyReview;
        VisualElement card = UiKit.Box("card");
        UiKit.Add(card, $"CENTURY REVIEW — YEAR {century.year}", "heading");
        UiKit.Add(card, $"Outcome: {century.outcome}. {ScenarioOneUiFacts.CenturyComparisonDescription(century)}", "body");
        foreach (ScenarioObjectiveResult c in century.referenceComparisons)
            UiKit.Add(card, $"{ScenarioOneUiFacts.ObjectiveName(c)}: yours {c.currentValue:0.##} · {ScenarioOneUiFacts.CenturyComparisonValueLabel(century)} {c.targetValue:0.##}", "body");
        scroll.Add(card);
    }
}
