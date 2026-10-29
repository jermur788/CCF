using System.Collections.Generic;
using System.Linq;
using CCF.Forestry.WorkEconomy;
using UnityEngine;
using UnityEngine.UIElements;

// Screen 5 — annual review: WORK DONE, MONEY, FOREST, then compact trends and
// objectives. Built from the recorded annual report, management events and
// ecological snapshots; it adds no causal claim the simulation does not make.
//
// S1-C year browsing: the completed years, and everything shown for the selected year (report, resolved work, money,
// snapshots, storms), come from ScenarioOneManager.HistoryYears. This view does not re-join the raw record lists for a
// year. Browsing an earlier year is read-only presentation: it never acknowledges anything.
//
// S1-C intervention drill-down: the Work column stays aggregate-first. Each aggregate line can be opened (and closed) to the
// recorded tasks behind it, from ScenarioManagementHistory.DescribeWork. Opening is presentation state only; nothing is written.
// S1-C provenance (T-04): each line carries one label from HistoryProvenance (recorded, current, derived, reconstructed,
// session-only, not recorded). Labels are presentation only; no label adds a fact or a causal claim.
// A fact the records do not hold is shown as "not recorded", never rebuilt from today's forest. A commissioned harvest settles
// once as a group, so its money is shown as one job settlement and never split per order.
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
    // True while an earlier year is on screen. The acknowledgement belongs to the newest review only.
    private bool showingEarlierYear;
    // Work rows the player has opened, keyed by year and group, so each year keeps its own open rows when years are switched.
    private readonly HashSet<string> openDetails = new HashSet<string>();

    public AnnualReviewView(ScenarioOneUiRoot ui)
    {
        this.ui = ui;
        Root = UiKit.Box("layer", "modal-backdrop");
        UiKit.MakeModal(Root);
        VisualElement modal = UiKit.Box("panel", "modal", "modal-review");
        Root.Add(modal);
        VisualElement header = UiKit.Box("modal-header");
        title = UiKit.Add(header, "", "title");
        VisualElement buttons = UiKit.Box("row", "screen-actions");
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
        // The footer is hidden for earlier years; this guard makes the same rule hold for the callback itself.
        var acknowledge = UiKit.Button("I've read the annual results", () => { if (!showingEarlierYear) ui.AcknowledgeAnnualReview(); }, true, "btn-primary");
        acknowledge.name = "annual-review-acknowledge";
        acknowledgement.Add(acknowledge);
        modal.Add(acknowledgement);
    }

    // Opening the review starts on the newest year; a past year is only ever a deliberate drill-down.
    public void ShowLatest() { viewYear = -1; openDetails.Clear(); }

    public void Refresh(bool force)
    {
        ScenarioOneManager m = ui.Manager;
        bool teachingReview = ui.Learning != null && ui.Learning.NeedsReviewAcknowledgement;
        // The root calls this every frame while the review is open. HistoryYears rebuilds every completed year, so it is
        // only read once the cheap change key says something changed.
        if (!force && ChangeKey(m, teachingReview) == shownKey)
            return;

        IReadOnlyList<YearHistoryView> years = m.HistoryYears;
        YearHistoryView latest = years.Count > 0 ? years[years.Count - 1] : null;
        // A new year always opens on the newest review; a past year is chosen with the year buttons.
        if (latest != null && latest.Year != lastLatestYear) { lastLatestYear = latest.Year; viewYear = -1; openDetails.Clear(); }
        YearHistoryView year = latest;
        if (viewYear >= 0)
        {
            year = FindYear(years, viewYear);
            if (year == null) { year = latest; viewYear = -1; }
        }
        bool historical = year != null && latest != null && year != latest;
        showingEarlierYear = historical;
        shownKey = ChangeKey(m, teachingReview);
        scroll.Clear();
        acknowledgement.style.display = (ui.NeedsAnnualReview || m.NeedsCompletionReviewAcknowledgement || teachingReview) && !historical ? DisplayStyle.Flex : DisplayStyle.None;
        // Completion guidance only: after the first review the footer says why this acknowledgement matters. The first review keeps its wording.
        Label acknowledgementPrompt = acknowledgement.Q<Label>();
        if (acknowledgementPrompt != null)
        {
            if (defaultAcknowledgementPrompt == null) defaultAcknowledgementPrompt = acknowledgementPrompt.text;
            acknowledgementPrompt.text = !ui.NeedsAnnualReview && m.CompletionGuidance == CompletionGuidanceState.AcknowledgeReview
                ? ScenarioOneCompletionGuidance.GuidanceText(CompletionGuidanceState.AcknowledgeReview) : defaultAcknowledgementPrompt;
        }
        earlierYear.SetEnabled(year != null && years.Count > 0 && year != years[0]);
        laterYear.SetEnabled(historical);
        title.text = year == null ? "Annual review"
            : historical ? $"Annual review · Year {year.Year} (earlier year; latest is Year {latest.Year})"
            : $"Annual review · Year {year.Year} complete";
        if (year == null)
        {
            UiKit.Add(scroll, "Advance a year from the Work Plan to see the first annual results.", "body");
            BuildObjectives(m);
            return;
        }

        VisualElement legend = UiKit.Add(scroll, HistoryProvenanceText.Legend, "faint");
        legend.name = "provenance-legend";

        IReadOnlyList<InterventionSummary> interventions = m.Interventions;
        // The completion summary opens the review of the year the objectives were met (not year 100).
        CompletionSummary completion = m.CompletionReview;
        if (completion != null && year.Year == m.OutcomeYear)
            BuildCompletion(m, completion);
        // After the first intervention, say what to look for. A prompt, not a gate: Advance is never blocked by it.
        if (interventions.Count == 1 && year.Year == interventions[0].Year)
            BuildFirstInterventionPrompt(interventions[0]);

        VisualElement columns = UiKit.Box("columns");
        scroll.Add(columns);
        BuildWork(m, year, Column(columns, "Work done"));
        BuildMoney(m, year, Column(columns, "Money"));
        BuildForest(m, year, Column(columns, "Forest"), historical);
        BuildStorm(m, year, historical);
        BuildHistory(m, interventions, completion, year);
        BuildTrends(m);
        BuildObjectives(m, historical);
        if (m.CenturyReview != null)
            BuildCentury(m);
    }

    // Cheap change detector for the early-out in Refresh. It is not a source of review content or navigation.
    private string ChangeKey(ScenarioOneManager m, bool teachingReview) =>
        m.CurrentEcologicalYear + "|" + m.AnnualReports.Count + "|" + m.CashCents + "|" + m.AnnualReviewSeen + "|" + viewYear + "|" + m.ManagementEvents.Count + "|" + teachingReview;

    private static YearHistoryView FindYear(IReadOnlyList<YearHistoryView> years, int year)
    {
        foreach (YearHistoryView candidate in years)
            if (candidate.Year == year) return candidate;
        return null;
    }

    private void BuildStorm(ScenarioOneManager manager, YearHistoryView year, bool historical)
    {
        if (year.Storms.Count == 0) return;
        StormDamageSummary summary = manager.StormDamageInYear(year.Year);
        if (summary == null) return;
        var card = UiKit.Box("card"); card.name = "annual-storm-review"; scroll.Add(card);
        UiKit.Add(card, "STORM AND WINDTHROW", "heading");
        string severity = summary.Event.severity < .04f ? "Light" : summary.Event.severity < .12f ? "Moderate" : "Severe";
        AddTagged(card, HistoryProvenance.Recorded, $"Year {summary.Event.year} · {severity} modelled event · {summary.Event.cropTreesLost} Crop Trees lost. These are game severity bands, not measured wind speeds.");
        AddTagged(card, HistoryProvenance.Reconstructed, $"{summary.TreesLost} trees lost · {UiKit.F(summary.OriginalVolumeM3, "0.00")} m³ of original fallen stem material.");
        AddTagged(card, HistoryProvenance.Reconstructed, $"Damage touches {summary.AffectedCells} map squares. This footprint is approximate; it does not mean every affected square is fully open.", "muted");
        UiKit.Add(card, HistoryProvenanceText.StormReconstructionNote, "faint");
        // Salvage since the event is added up from completed salvage orders: derived, and fine under the storm's own year.
        AddTagged(card, HistoryProvenance.Derived, $"{UiKit.F(summary.SalvagedVolumeM3, "0.00")} m³ salvaged since this event, to date.");
        // How much deadwood is left is today's forest. It is shown only on the latest review, never under an earlier year.
        if (!historical)
            AddTagged(card, HistoryProvenance.Current, $"{UiKit.F(summary.DeadwoodRemainingM3, "0.00")} m³ remains as deadwood now.");
        UiKit.Add(card, "Fallen trees stop contributing to canopy and seed rain. Openings increase light and recent exposure; bramble and bracken can respond, affecting small young trees through the existing competition model. Inspect the changed cells before choosing work.", "body");
        UiKit.Add(card, "Salvage is optional. Aim at an eligible fallen stem and press X to add it to the Work Plan; choose Sell or Keep there. Unselected stems remain deadwood, and a small salvage visit may cost more than its timber earns.", "body");
        if (summary.WaypointCell >= 0) card.Add(UiKit.Button("Mark storm damage on the map", () => ui.ShowStormWaypoint(summary.WaypointCell)));
    }

    private static string ChipStyle(HistoryProvenance kind)
    {
        switch (kind)
        {
            case HistoryProvenance.Recorded: return "chip-ok";
            case HistoryProvenance.Current: return "chip-rack";
            case HistoryProvenance.Derived: return "chip-crop";
            case HistoryProvenance.Reconstructed:
            case HistoryProvenance.SessionOnly: return "chip-warn";
            default: return "chip-bad";
        }
    }

    // The label is words, not colour alone. Its class name ("prov-recorded", ...) lets the checks find every label.
    private static Label Tag(VisualElement parent, HistoryProvenance kind)
    {
        Label chip = UiKit.Chip(parent, HistoryProvenanceText.Label(kind), ChipStyle(kind));
        chip.AddToClassList("prov");
        chip.AddToClassList("prov-" + kind.ToString().ToLowerInvariant());
        chip.style.flexShrink = 0;
        chip.style.marginTop = 2;
        return chip;
    }

    // A line of text with its provenance label in front. The text wraps beside the label instead of pushing past the card.
    private static Label AddTagged(VisualElement parent, HistoryProvenance kind, string text, string textClass = "body")
    {
        VisualElement row = UiKit.Box("row");
        row.style.alignItems = Align.FlexStart;
        Tag(row, kind);
        Label label = UiKit.Add(row, text, textClass);
        label.style.flexShrink = 1;
        label.style.flexGrow = 1;
        label.style.flexBasis = 0;
        parent.Add(row);
        return label;
    }

    // Name/value line (as UiKit.Line) with a provenance label in front. A long value goes under its name, so the name keeps room
    // to read beside the label in a narrow column.
    private static void TaggedLine(VisualElement parent, HistoryProvenance kind, string name, string value, string valueClass = "value")
    {
        VisualElement row = UiKit.Box("row");
        row.style.alignItems = Align.FlexStart;
        Tag(row, kind);
        VisualElement line = UiKit.Box();
        line.style.flexGrow = 1;
        line.style.flexShrink = 1;
        line.style.flexBasis = 0;
        UiKit.Line(line, name, value, valueClass, value.Length > 12);
        row.Add(line);
        parent.Add(row);
    }

    private static VisualElement Column(VisualElement parent, string heading)
    {
        VisualElement column = UiKit.Box("card", "column");
        UiKit.Add(column, heading.ToUpperInvariant(), "heading");
        parent.Add(column);
        return column;
    }

    private void BuildWork(ScenarioOneManager m, YearHistoryView year, VisualElement c)
    {
        // The year's resolved work already pairs each WorkResolved event with its order record (where the order still exists).
        // DescribeWork groups the same events for drill-down; the aggregate lines below are unchanged.
        YearWorkDetail detail = ScenarioManagementHistory.DescribeWork(year);
        List<ResolvedWorkRecord> resolved = year.ResolvedWork;
        List<ResolvedWorkRecord> done = resolved.Where(r => r.Event.outcome == ScenarioManagementOutcome.Succeeded).ToList();
        int regenerationRemovals = year.Report != null ? year.Report.regenerationRemovalTasks : 0;
        int felled = done.Count(r => r.Event.taskType == ScenarioWorkType.FellTree);
        int salvaged = done.Count(r => r.Event.taskType == ScenarioWorkType.SalvageDeadwood);
        if (salvaged > 0)
        {
            AddTagged(c, HistoryProvenance.Derived, $"{salvaged} windthrow stem(s) salvaged (contractor)");
            AddDetail(c, year.Year, detail.DoneFor(ScenarioWorkType.SalvageDeadwood));
        }
        if (felled > 0)
        {
            AddTagged(c, HistoryProvenance.Derived, $"{felled} tree(s) thinned (contractor)");
            AddDetail(c, year.Year, detail.DoneFor(ScenarioWorkType.FellTree));
        }
        if (detail.HarvestSettlement != null) AddHarvestSettlement(c, detail);
        var planted = done.Where(r => r.Event.taskType == ScenarioWorkType.PlantJuvenile).ToList();
        if (planted.Count > 0)
        {
            int sheltered = planted.Count(r => r.Order != null && r.Order.installShelter);
            int byOwner = planted.Count(r => r.Event.executionMethod == WorkExecutionMethod.LandownerSimulated);
            AddTagged(c, HistoryProvenance.Derived, $"{planted.Count} planted ({string.Join(", ", planted.GroupBy(r => r.Event.speciesId).Select(g => $"{g.Count()} {ScenarioOneUiFacts.SpeciesName(g.Key)}"))}), "
                + $"{sheltered} with shelters · {byOwner} by you, {planted.Count - byOwner} by contractor");
            AddDetail(c, year.Year, detail.DoneFor(ScenarioWorkType.PlantJuvenile));
        }
        int pruned = done.Count(r => r.Event.taskType == ScenarioWorkType.PruneTree);
        if (pruned > 0)
        {
            AddTagged(c, HistoryProvenance.Derived, $"{pruned} crop tree(s) pruned (contractor)");
            AddDetail(c, year.Year, detail.DoneFor(ScenarioWorkType.PruneTree));
        }
        if (regenerationRemovals > 0)
        {
            AddTagged(c, HistoryProvenance.Recorded, $"{regenerationRemovals} regeneration cohort(s) removed");
            AddDetail(c, year.Year, detail.DoneFor(ScenarioWorkType.RemoveRegeneration));
        }
        if (felled + salvaged + planted.Count + pruned + regenerationRemovals == 0) AddTagged(c, HistoryProvenance.Derived, "No work was resolved this year.");
        var failed = resolved.Where(r => r.Event.outcome != ScenarioManagementOutcome.Succeeded).ToList();
        AddTagged(c, HistoryProvenance.Derived, failed.Count == 0 ? "No tasks failed." : $"{failed.Count} task(s) failed:", failed.Count == 0 ? "muted" : "body");
        foreach (ResolvedWorkRecord r in failed.Take(5))
        {
            if (r.Event.failureReason != null && r.Event.failureReason.Length > 0)
                AddTagged(c, HistoryProvenance.Recorded, $"· {r.Event.taskType}: {ScenarioOneUiFacts.PlainText(r.Event.failureReason)}", "muted");
            else
                AddTagged(c, HistoryProvenance.NotRecorded, $"· {r.Event.taskType}: reason not recorded", "muted");
        }
        AddDetail(c, year.Year, detail.Failed);
        // A cancelled order was deleted when it was cancelled, so these entries come from the cancellation records alone.
        int cancelled = detail.Cancelled.Items.Count;
        if (cancelled > 0)
        {
            AddTagged(c, HistoryProvenance.Derived, $"{cancelled} order(s) cancelled before they were carried out.");
            AddDetail(c, year.Year, detail.Cancelled);
        }
    }

    // The year's one grouped harvest settlement. The records hold a single settlement for the whole commissioned job, so this
    // is the only place harvest money is shown beside the work; individual orders and trees carry no money of their own.
    private static void AddHarvestSettlement(VisualElement c, YearWorkDetail detail)
    {
        GroupedHarvestSettlement settlement = detail.HarvestSettlement;
        int orders = detail.Done.Where(g => g.IsGroupedHarvest).Sum(g => g.Items.Count);
        VisualElement box = UiKit.Box();
        box.name = "annual-harvest-settlement";
        // The order count and year label are grouped at display time; the two money figures are the one persisted settlement.
        AddTagged(box, HistoryProvenance.Derived, $"Harvest job · year {settlement.ResolutionYearLabel}: {orders} harvest order(s) settled together as one job.");
        AddTagged(box, HistoryProvenance.Recorded, $"Timber income {UiKit.Money(settlement.SettlementRevenueCents)} · harvesting cost {UiKit.Money(settlement.SettlementContractorCostCents)}.");
        UiKit.Add(box, "The records hold one settlement for the whole job, so no money is shown per order or per tree. The year number labels the job; it is not a unique job ID.", "faint");
        c.Add(box);
    }

    // A "Show recorded details" row under an aggregate line. Everything it reveals comes from the recorded task events;
    // the content is built the first time the row is opened. The open state is presentation only.
    private void AddDetail(VisualElement parent, int year, WorkGroupDetail group)
    {
        if (group == null || group.Items.Count == 0) return;
        string key = year + "|" + group.Key;
        VisualElement body = UiKit.Box("work-detail");
        body.name = "work-detail-" + group.Key;
        body.style.marginLeft = 10;
        body.style.marginBottom = 6;
        bool built = false;
        Button toggle = null;
        void Apply(bool open)
        {
            if (open && !built) { BuildDetailBody(body, group); built = true; }
            body.style.display = open ? DisplayStyle.Flex : DisplayStyle.None;
            toggle.text = open ? "Hide recorded details" : DetailToggleText(group);
        }
        toggle = UiKit.Button("", () =>
        {
            bool open = !openDetails.Contains(key);
            if (open) openDetails.Add(key); else openDetails.Remove(key);
            Apply(open);
        }, true, "btn-ghost");
        toggle.name = "work-detail-toggle-" + group.Key;
        toggle.style.alignSelf = Align.FlexStart;
        // Long aggregate labels must wrap inside the Work card at smaller review sizes.
        toggle.style.maxWidth = Length.Percent(100);
        toggle.style.whiteSpace = WhiteSpace.Normal;
        parent.Add(toggle);
        parent.Add(body);
        Apply(openDetails.Contains(key));
    }

    private static string DetailToggleText(WorkGroupDetail group)
    {
        string text = $"Show recorded details · {group.Items.Count} task{(group.Items.Count == 1 ? "" : "s")}";
        if (group.DistinctRecordedCells > 0)
            text += $" · {group.DistinctRecordedCells} map square{(group.DistinctRecordedCells == 1 ? "" : "s")}";
        return text;
    }

    private static void BuildDetailBody(VisualElement body, WorkGroupDetail group)
    {
        UiKit.Add(body, "Each entry is read from a recorded task event. A fact the record lacks is shown on its own NOT RECORDED line.", "faint");
        if (group.IsGroupedHarvest)
            UiKit.Add(body, "These orders were settled together as one harvest job. No money is recorded for a single order or tree.", "faint");
        // The map-square label needs the grid width; fetch it once for the whole list.
        ForestEcologyController eco = Object.FindFirstObjectByType<ForestEcologyController>();
        int cellsPerAxis = eco != null ? eco.CellsPerAxis : 0;
        foreach (WorkItemDetail item in group.Items)
            BuildDetailItem(body, item, cellsPerAxis);
        if (group.Status == WorkItemStatus.Cancelled)
            UiKit.Add(body, "A cancelled order no longer exists. Each entry above is read from its cancellation record only.", "faint");
    }

    // One entry = one RECORDED title row, one RECORDED facts row, and (only when something is absent) one NOT RECORDED row that
    // names exactly what the record lacks. No single label stands for a mixed entry.
    private static void BuildDetailItem(VisualElement body, WorkItemDetail item, int cellsPerAxis)
    {
        VisualElement entry = UiKit.Box();
        entry.style.marginTop = 3;
        var missing = new List<string>();
        VisualElement head = UiKit.Box("row", "row-wrap");
        if (item.Status == WorkItemStatus.Failed) UiKit.Chip(head, "FAILED", "chip-bad");
        else if (item.Status == WorkItemStatus.Cancelled) UiKit.Chip(head, "CANCELLED", "chip-warn");
        string target = TargetText(item);
        if (target == "tree not recorded") { missing.Add(target); target = ""; }
        Tag(head, HistoryProvenance.Recorded);
        UiKit.Add(head, WorkLabel(item.TaskType) + (target.Length > 0 ? " · " + target : "") + $" · order {item.WorkOrderId}", "body");
        entry.Add(head);

        var facts = new List<string>();
        if (item.SpeciesId.Length > 0) facts.Add(ScenarioOneUiFacts.SpeciesName(item.SpeciesId));
        string location = LocationText(item, cellsPerAxis);
        if (location.Length > 0) facts.Add(location); else missing.Add("location not recorded");
        if (item.ExecutionMethodRecorded)
            facts.Add(item.ExecutionMethod == WorkExecutionMethod.LandownerSimulated ? "done by you" : "done by contractor");
        else
            missing.Add("execution method not recorded");
        if (item.Status == WorkItemStatus.Cancelled)
            facts.Add($"cancelled in year {item.Year}");
        else
            facts.Add($"resolved in year {item.Year}");
        if (item.ShelterRecorded) facts.Add(item.ShelterInstalled ? "shelter fitted" : "no shelter");
        if (item.Status == WorkItemStatus.Succeeded)
        {
            string treatment = TreatmentText(item.Treatment);
            if (treatment.Length > 0) facts.Add("result: " + treatment);
            if (item.BiologicalVolumeM3 > 0f) facts.Add($"recorded stem volume {UiKit.F(item.BiologicalVolumeM3, "0.00")} m³");
            if (item.OwnMoneyRecorded)
            {
                if (item.ContractorCostCents > 0) facts.Add($"contractor cost {UiKit.Money(item.ContractorCostCents)}");
                if (item.StockCostCents > 0) facts.Add($"materials {UiKit.Money(item.StockCostCents)}");
                if (item.OwnerMinutes > 0) facts.Add($"your time {ScenarioOneManager.FormatMinutes(item.OwnerMinutes)}");
            }
        }
        AddTagged(entry, HistoryProvenance.Recorded, string.Join(" · ", facts), "muted");
        if (item.Status == WorkItemStatus.Failed)
        {
            if (item.FailureReason.Length > 0)
                AddTagged(entry, HistoryProvenance.Recorded, "Reason: " + ScenarioOneUiFacts.PlainText(item.FailureReason), "muted");
            else
                missing.Add("Reason: not recorded");
        }
        if (missing.Count > 0)
            AddTagged(entry, HistoryProvenance.NotRecorded, string.Join(" · ", missing), "muted");
        body.Add(entry);
    }

    private static string WorkLabel(ScenarioWorkType type)
    {
        switch (type)
        {
            case ScenarioWorkType.FellTree: return "Felling";
            case ScenarioWorkType.SalvageDeadwood: return "Windthrow salvage";
            case ScenarioWorkType.PlantJuvenile: return "Planting";
            case ScenarioWorkType.PruneTree: return "Pruning";
            case ScenarioWorkType.RemoveRegeneration: return "Regeneration clearing";
            default: return type.ToString();
        }
    }

    // Only work on a standing or fallen tree has a target tree. Planting and regeneration clearing are about a place.
    private static string TargetText(WorkItemDetail item)
    {
        bool treeTask = item.TaskType == ScenarioWorkType.FellTree || item.TaskType == ScenarioWorkType.PruneTree
            || item.TaskType == ScenarioWorkType.SalvageDeadwood;
        if (!treeTask) return "";
        return item.TargetTreeId.Length > 0 ? "tree " + item.TargetTreeId : "tree not recorded";
    }

    private static string LocationText(WorkItemDetail item, int cellsPerAxis)
    {
        var parts = new List<string>();
        if (item.CellIndex >= 0)
            parts.Add("map square " + (cellsPerAxis > 0 ? UiKit.CellLabel(item.CellIndex, cellsPerAxis) : "#" + item.CellIndex));
        if (item.PositionRecorded)
            parts.Add($"position ({UiKit.F(item.PositionX, "0.0")}, {UiKit.F(item.PositionZ, "0.0")})");
        return string.Join(" · ", parts);
    }

    private static string TreatmentText(ScenarioEcologicalTreatment treatment)
    {
        switch (treatment)
        {
            case ScenarioEcologicalTreatment.TreeFelledAndExtracted: return "felled and extracted";
            case ScenarioEcologicalTreatment.JuvenilePlanted: return "young tree planted";
            case ScenarioEcologicalTreatment.RegenerationRemoved: return "regeneration removed";
            case ScenarioEcologicalTreatment.TreePruned: return "pruned";
            case ScenarioEcologicalTreatment.TreeRetainedAsDeadwood: return "felled and left as fallen deadwood";
            case ScenarioEcologicalTreatment.WindthrowSalvaged: return "windthrow stem salvaged";
            default: return "";
        }
    }

    private static void BuildMoney(ScenarioOneManager m, YearHistoryView year, VisualElement c)
    {
        // The annual report is the per-year money authority (YearMoneySummary.Authority). A commissioned harvest settles
        // once as a group, so the harvest line below is that group's total and is never split per order.
        YearMoneySummary money = year.Money;
        if (money.Authority == null)
        {
            AddTagged(c, HistoryProvenance.NotRecorded, "No annual report was recorded for this year, so its money is not shown.", "muted");
            return;
        }
        var events = year.ResolvedWork.Where(r => r.Event.outcome == ScenarioManagementOutcome.Succeeded).Select(r => r.Event).ToList();
        long harvestCost = events.Where(e => e.taskType == ScenarioWorkType.FellTree || e.taskType == ScenarioWorkType.SalvageDeadwood).Sum(e => e.contractorCostCents);
        long materials = events.Sum(e => e.stockCostCents);
        foreach (ScenarioTimberSale sale in money.Sales)
            TaggedLine(c, HistoryProvenance.Recorded, $"Timber sold · {sale.assortment} ({UiKit.M3(sale.soldVolumeCm3)})", UiKit.SignedMoney(sale.revenueCents), "money");
        if (harvestCost > 0)
            TaggedLine(c, HistoryProvenance.Derived, "Harvesting work", UiKit.SignedMoney(-(harvestCost - money.HarvestMinimumAdjustmentCents)), "money-negative");
        if (money.HarvestMinimumAdjustmentCents > 0)
            TaggedLine(c, HistoryProvenance.Recorded, "Small-job minimum", UiKit.SignedMoney(-money.HarvestMinimumAdjustmentCents), "money-negative");
        long otherWork = money.ContractorCostCents - harvestCost;
        if (otherWork > 0)
            TaggedLine(c, HistoryProvenance.Derived, "Planting, pruning and other contractor work", UiKit.SignedMoney(-otherWork), "money-negative");
        if (materials > 0)
            TaggedLine(c, HistoryProvenance.Derived, "Tree shelters (material)", UiKit.SignedMoney(-materials), "money-negative");
        if (year.HarvestSettlement != null)
            UiKit.Add(c, "Timber sold and harvesting work above belong to one grouped harvest job for the year, not to single trees or orders.", "faint");
        TaggedLine(c, HistoryProvenance.Recorded, "Closing cash", UiKit.Money(money.ClosingCashCents), "money");
        AddTagged(c, HistoryProvenance.Recorded, $"Your time used: {ScenarioOneManager.FormatMinutes(money.OwnerMinutes)} of {ScenarioOneManager.FormatMinutes(m.OwnerMinutesPerYear)}", "muted");
        UiKit.Add(c, "Saplings are paid for when bought from the nursery.", "faint");
    }

    private static void BuildForest(ScenarioOneManager m, YearHistoryView year, VisualElement c, bool historical)
    {
        // A past year is described by the snapshots recorded then. The live-state lines (opened cells now, browsing now,
        // shelters now) would describe today's forest under an old date, so they are left out.
        ForestEcologyController eco = historical ? null : Object.FindFirstObjectByType<ForestEcologyController>();
        // The year's own snapshot and the one before it. If the year has none, say so rather than borrowing the latest.
        ScenarioEcologicalSnapshot current = year.Snapshot;
        ScenarioEcologicalSnapshot previous = year.PreviousSnapshot;
        ScenarioAnnualReport report = year.Report;
        if (current == null)
            AddTagged(c, HistoryProvenance.NotRecorded, "No ecological snapshot was recorded for this year.", "muted");
        else
            foreach (string line in ScenarioEcologyReviewLines.Lines(previous, current, eco, historical ? null : m.PlantedJuveniles))
            {
                // Browsing and protection are rebuilt below without per-leader
                // probabilities or unimplemented fencing.
                if (line.StartsWith("Browsing pressure") || line.StartsWith("Protected juveniles")) continue;
                HistoryProvenance? kind = HistoryProvenanceText.EcologyLine(line, previous != null);
                if (kind.HasValue) AddTagged(c, kind.Value, line);
                else UiKit.Add(c, line, "body");
            }
        if (eco != null && eco.Browsing.BackgroundPressure > 0f)
        {
            var recorded = m.PlantedJuveniles.Where(j => j.lastBrowseAssessmentYear == eco.EcologicalYear && eco.EcologicalYear > 0).ToList();
            AddTagged(c, HistoryProvenance.Current, $"Browsing pressure {BrowsingConditions.PressureBand(eco.Browsing.BackgroundPressure)} now.");
            // Last year's browsing result is held in memory only; after a load there is nothing to show, so say that.
            if (recorded.Count > 0)
            {
                AddTagged(c, HistoryProvenance.SessionOnly, $"{recorded.Count(j => j.lastYearBrowsed)} of {recorded.Count} planted juveniles browsed last year.");
                UiKit.Add(c, HistoryProvenanceText.SessionOnlyBrowsingNote, "faint");
            }
            else
                AddTagged(c, HistoryProvenance.SessionOnly, HistoryProvenanceText.SessionOnlyBrowsingMissing, "muted");
            int upcoming = eco.EcologicalYear + 1, sheltered = 0;
            foreach (PlantedJuvenile j in m.PlantedJuveniles.Where(j => j.alive && string.IsNullOrEmpty(j.promotedTreeId)))
                if (eco.Browsing.ProtectionAt(new Vector2(j.position.x, j.position.z), upcoming, out _) == BrowseProtectionState.EffectiveShelter)
                    sheltered++;
            if (sheltered > 0) AddTagged(c, HistoryProvenance.Current, $"{sheltered} planted juvenile(s) protected by shelters now.");
        }

        UiKit.Add(c, "An opening creates an opportunity, not guaranteed regeneration. Walk back to inspect what remains and how the forest responds; reassess before further work.", "muted");
        foreach (ScenarioObjectiveAchievement achieved in year.ObjectivesAchieved)
            AddTagged(c, HistoryProvenance.Recorded, $"Objective first reached: {ObjectiveName(m, achieved.objectiveId)}");
        UiKit.Add(c, "Retained material", "heading");
        if (report != null)
            TaggedLine(c, HistoryProvenance.Recorded, "Retained usable timber (this year)", UiKit.F(report.keptForUseVolumeM3, "0.00") + " m³");
        if (!historical)
            TaggedLine(c, HistoryProvenance.Current, "Retained usable timber (stock)", UiKit.F(m.RetainedTimberM3, "0.00") + " m³");
        if (report != null)
            TaggedLine(c, HistoryProvenance.Recorded, "Retained deadwood (new this year)", UiKit.F(report.deadwoodCreatedM3, "0.00") + " m³");
        if (current != null)
            TaggedLine(c, HistoryProvenance.Recorded, "Fallen deadwood on site", $"{current.deadwoodCount} log(s) · {UiKit.F(current.deadwoodVolumeM3, "0.00")} m³");
    }

    private void BuildTrends(ScenarioOneManager m)
    {
        VisualElement card = UiKit.Box("card");
        UiKit.Add(card, "TRENDS", "heading");
        AddTagged(card, HistoryProvenance.Derived, "Drawn from the saved annual reports and snapshots of up to the last 12 years; the strip itself is not saved.", "muted");
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
            bar.style.backgroundColor = UiKit.ForestryColour;
            col.Add(bar);
            col.Add(UiKit.Text(label, "faint"));
            strip.Add(col);
        }
        box.Add(strip);
        UiKit.Add(box, $"Latest: {format(points.Last().value)}", "muted");
        return box;
    }

    private static string ObjectiveName(ScenarioOneManager m, string objectiveId)
    {
        ScenarioObjectiveResult match = m.Objectives.FirstOrDefault(o => o.objectiveId == objectiveId);
        return match != null ? match.displayName : objectiveId;
    }

    private void BuildObjectives(ScenarioOneManager m, bool historical = false)
    {
        var objectives = m.Objectives;
        VisualElement card = UiKit.Box("card");
        UiKit.Add(card, $"OBJECTIVES {objectives.Count(o => o.achieved)} OF {objectives.Count}" + (historical ? " (current state)" : ""), "heading");
        foreach (ScenarioObjectiveResult o in objectives)
        {
            // Whether an objective is met is read from the live forest, so it is current even beside an old year.
            AddTagged(card, HistoryProvenance.Current, ScenarioOneUiFacts.ObjectiveLine(o));
            if (!o.achieved) continue;
            int achievedYear = m.ObjectiveYear(o.objectiveId, out ObjectiveYearSource source);
            if (achievedYear < 0) AddTagged(card, HistoryProvenance.NotRecorded, "Year reached: not recorded", "faint");
            else if (source == ObjectiveYearSource.Derived) AddTagged(card, HistoryProvenance.Derived, $"First reached in year {achievedYear} (derived from saved records)", "faint");
            else AddTagged(card, HistoryProvenance.Recorded, $"First reached in year {achievedYear} (recorded)", "faint");
        }
        // The outcome and its year are saved records; the tutorial hint is guidance for now, not a historical fact.
        if (m.Outcome == ScenarioOneOutcome.Completed)
            AddTagged(card, HistoryProvenance.Recorded, $"Scenario objectives met in year {m.OutcomeYear}. Continue to the Century Review (year {m.CenturyReviewYear}).", "muted");
        else if (m.Outcome == ScenarioOneOutcome.Failed)
            AddTagged(card, HistoryProvenance.Recorded, m.OutcomeReason, "muted");
        else
            UiKit.Add(card, m.TutorialHint, "muted");
        scroll.Add(card);
    }

    private void StepYear(int direction)
    {
        // HistoryYears is oldest first, so stepping is plain index movement; the newest year is always "follow latest".
        IReadOnlyList<YearHistoryView> years = ui.Manager.HistoryYears;
        if (years.Count == 0) return;
        int current = years.Count - 1;
        if (viewYear >= 0)
            for (int i = 0; i < years.Count; i++)
                if (years[i].Year == viewYear) { current = i; break; }
        int next = Mathf.Clamp(current + direction, 0, years.Count - 1);
        viewYear = next == years.Count - 1 ? -1 : years[next].Year;
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
        AddTagged(card, HistoryProvenance.Derived, ManagementHistoryText.FirstInterventionPrompt(first));
        scroll.Add(card);
    }

    // One block per resolved work plan, built from the persisted records on demand (nothing here is saved).
    private void BuildHistory(ScenarioOneManager m, IReadOnlyList<InterventionSummary> interventions, CompletionSummary completion, YearHistoryView shown)
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
            head.Add(UiKit.Button(i.Year == shown.Year ? "Shown above" : $"Read year {i.Year}", () => { viewYear = captured.Year; Refresh(true); }, i.Year != shown.Year));
            block.Add(head);
            AddTagged(block, ManagementHistoryText.WorkProvenance, ManagementHistoryText.Work(i, text));
            AddTagged(block, ManagementHistoryText.MoneyProvenance, ManagementHistoryText.Money(i, text));
            AddTagged(block, ManagementHistoryText.DuringPlanProvenance(i), ManagementHistoryText.DuringPlan(i), "muted");
            AddTagged(block, ManagementHistoryText.SinceProvenance(i), ManagementHistoryText.Since(i, text), "muted");
            card.Add(block);
        }
        if (interventions.Count > 0) UiKit.Add(card, ManagementHistoryText.NotCausalNote, "faint");
        IReadOnlyList<StaleOrderNote> stale = m.StaleOrders;
        if (stale.Count > 0)
            AddTagged(card, HistoryProvenance.Current, $"{stale.Count} open order{(stale.Count == 1 ? "" : "s")} need attention. Open the Work Plan [Tab] to review {(stale.Count == 1 ? "it" : "them")}.");
        scroll.Add(card);
    }

    private void BuildCompletion(ScenarioOneManager m, CompletionSummary completion)
    {
        HistoryTextContext text = TextContext();
        VisualElement card = UiKit.Box("card");
        card.name = "completion-review";
        UiKit.Add(card, $"SCENARIO ONE COMPLETE · YEAR {completion.Year}", "heading");
        foreach (string line in ManagementHistoryText.Completion(completion, text))
        {
            HistoryProvenance? kind = HistoryProvenanceText.CompletionLine(line);
            string style = line.StartsWith("  ") ? "muted" : "body";
            if (kind.HasValue) AddTagged(card, kind.Value, line, style);
            else UiKit.Add(card, line, style);
        }
        UiKit.Add(card, "Objectives met (current state)", "heading");
        foreach (ScenarioObjectiveResult o in m.Objectives)
            AddTagged(card, HistoryProvenance.Current, ScenarioOneUiFacts.ObjectiveLine(o));
        scroll.Add(card);
    }

    private void BuildCentury(ScenarioOneManager m)
    {
        ScenarioCenturyReview century = m.CenturyReview;
        VisualElement card = UiKit.Box("card");
        UiKit.Add(card, $"CENTURY REVIEW — YEAR {century.year}", "heading");
        AddTagged(card, HistoryProvenance.Recorded, $"Outcome: {century.outcome}. {ScenarioOneUiFacts.CenturyComparisonDescription(century)}");
        foreach (ScenarioObjectiveResult c in century.referenceComparisons)
            AddTagged(card, HistoryProvenance.Recorded, $"{ScenarioOneUiFacts.ObjectiveName(c)}: yours {c.currentValue:0.##} · {ScenarioOneUiFacts.CenturyComparisonValueLabel(century)} {c.targetValue:0.##}");
        scroll.Add(card);
    }
}
