using System.Collections.Generic;
using System.Linq;
using System.Text;
using CCF.Forestry.WorkEconomy;
using UnityEngine.UIElements;

// Screen 4 — Work Plan. Reviews and approves work created in the walked
// world; it does not design the forest. Every figure is read from the
// authoritative orders and the WorkEconomy/TimberYield quotes; every button
// calls an existing ScenarioOneManager action.
public sealed class WorkPlanView
{
    private readonly ScenarioOneUiRoot ui;
    public VisualElement Root { get; }
    private readonly Label title, cash, owner, summary, feedback;
    private readonly ScrollView scroll;
    private readonly VisualElement footer;
    // Asked before a year passes with planned work that was never approved (see AdvanceGuard). Inline, not another modal.
    private readonly VisualElement advanceWarning;
    private readonly Label advanceHeadline, advanceDetail;
    private bool confirmingAdvance;
    private int purchaseQuantity = 5;
    // Extraction racks whose trees are listed one by one (a rack is one line until opened).
    private readonly HashSet<int> expandedRacks = new HashSet<int>();
    private string shownSignature = "";

    public WorkPlanView(ScenarioOneUiRoot ui)
    {
        this.ui = ui;
        Root = UiKit.Box("layer", "modal-backdrop");
        UiKit.MakeModal(Root);
        VisualElement modal = UiKit.Box("panel", "modal", "modal-workplan");
        Root.Add(modal);

        VisualElement header = UiKit.Box("modal-header");
        VisualElement left = new VisualElement();
        title = UiKit.Add(left, "", "title");
        owner = UiKit.Add(left, "", "subtitle");
        header.Add(left);
        VisualElement right = UiKit.Box("row", "screen-actions");
        cash = UiKit.Add(right, "", "money", "title");
        right.Add(UiKit.Button("Help [F1]", () => ui.ShowHelp(MenuHelpView.Menu.WorkPlan)));
        right.Add(UiKit.Button("Annual review", () => ui.ShowReview()));
        right.Add(UiKit.Button("Stand map [M]", () => ui.ShowMap()));
        right.Add(UiKit.Button("Objectives [O]", () => ui.ShowObjectives()));
        right.Add(UiKit.Button("Back to forest", () => ui.CloseAll()));
        header.Add(right);
        modal.Add(header);

        UiKit.Add(modal, "Your marks and planting spots join this plan when you open it. Review each job and its cost, then approve the plan; Advance one year carries out what you approved. Remove a job here, or add more in the forest.", "muted");
        summary = UiKit.Add(modal, "", "body");

        scroll = new ScrollView(ScrollViewMode.Vertical);
        scroll.style.flexGrow = 1;
        modal.Add(scroll);

        advanceWarning = UiKit.Box("card");
        advanceWarning.name = "advance-warning";
        advanceWarning.style.display = DisplayStyle.None;
        // Semantic warning colour mirrors the shared visual-system token.
        advanceWarning.style.borderLeftWidth = 2;
        advanceWarning.style.borderLeftColor = UiKit.WarningColour;
        advanceHeadline = UiKit.Add(advanceWarning, "", "heading");
        advanceDetail = UiKit.Add(advanceWarning, "", "body");
        VisualElement choices = UiKit.Row(advanceWarning, "row-wrap");
        choices.Add(UiKit.Button(AdvanceGuard.ReturnLabel, CancelAdvanceConfirmationAndRefresh, true, "btn-primary"));
        choices.Add(UiKit.Button(AdvanceGuard.ApproveLabel, () => { ui.ApproveWorkPlan(); Refresh(true); }));
        choices.Add(UiKit.Button(AdvanceGuard.AdvanceAnywayLabel, () => { confirmingAdvance = false; AdvanceNow(); }));
        modal.Add(advanceWarning);

        feedback = UiKit.Add(modal, "", "muted");
        footer = UiKit.Box("modal-footer");
        modal.Add(footer);
    }

    // The orders as "Approve Work Plan" would see them. Counted when asked (a click or a refresh), never per frame.
    private static AdvanceGuard.Decision AdvanceDecision(ScenarioOneManager m)
    {
        int pending = 0, approved = 0;
        foreach (ScenarioOneWorkOrder order in m.WorkOrders)
        {
            if (order.status == ScenarioWorkStatus.Approved) approved++;
            else if (order.status == ScenarioWorkStatus.Pending && string.IsNullOrEmpty(order.validationMessage)) pending++;
        }
        return AdvanceGuard.Evaluate(pending, approved);
    }

    // Dropped whenever the plan is (re)opened, so an old question never greets the player on a later visit.
    public void CancelAdvanceConfirmation() => confirmingAdvance = false;

    private void CancelAdvanceConfirmationAndRefresh()
    {
        confirmingAdvance = false;
        Refresh(true);
    }

    private void AdvanceNow()
    {
        if (ui.AdvanceFromWorkPlan()) return;
        Refresh(true);
    }

    private void OnAdvanceClicked()
    {
        if (AdvanceDecision(ui.Manager).Warn)
        {
            confirmingAdvance = true;
            Refresh(true);
            return;
        }
        AdvanceNow();
    }

    public void Refresh(bool force)
    {
        ScenarioOneManager m = ui.Manager;
        // The teaching stage decides which sections are usable, so it is part of what makes the plan redraw.
        string rackProblem = ui.RackPlanProblem;
        string approvalProblem = ui.RackApprovalProblem;
        string signature = Signature(m) + "|L" + ui.Learning.CurrentTopicIndex + rackProblem + "|A" + approvalProblem;
        if (!force && signature == shownSignature)
            return;
        shownSignature = signature;
        float scrollY = scroll.scrollOffset.y;

        ScenarioOneManager.WorkPlanSummary totals = m.GetWorkPlanSummary();
        title.text = $"Work Plan · Year {m.CurrentEcologicalYear}";
        cash.text = UiKit.Money(m.CashCents);
        owner.text = $"Your time next year: {ScenarioOneManager.FormatMinutes(m.PlannedOwnerMinutes)} planned of "
            + $"{ScenarioOneManager.FormatMinutes(m.OwnerMinutesPerYear)}";
        summary.text = $"{totals.OpenCount} open task(s), {totals.ApprovedCount} approved · external cost {UiKit.Money(totals.ExternalCostCents)} · "
            + $"expected timber {UiKit.Money(totals.ExpectedRevenueCents)} · net {UiKit.SignedMoney(totals.ExpectedRevenueCents - totals.ExternalCostCents)} · "
            + $"cash after approved work {UiKit.Money(m.CashCents - m.ReservedContractorCashCents)}";

        scroll.Clear();
        BuildRacks(m);
        WorkPlanOverview.Build(scroll, ui);
        BuildAttention(m);
        BuildHarvest(m);
        BuildPlanting(m);
        BuildPruning(m);
        BuildRemoval(m);
        BuildNursery(m);
        BuildReference(m);
        scroll.schedule.Execute(() => scroll.scrollOffset = new UnityEngine.Vector2(0, scrollY));

        feedback.text = !string.IsNullOrEmpty(rackProblem) ? rackProblem
            : !string.IsNullOrEmpty(approvalProblem) ? approvalProblem
            : m.Outcome == ScenarioOneOutcome.Failed ? m.OutcomeReason : m.Feedback + "   ·   " + m.TutorialHint;

        // The question stays only while it still applies: approving, removing or cancelling the planned work answers it.
        AdvanceGuard.Decision guard = AdvanceDecision(m);
        if (!guard.Warn) confirmingAdvance = false;
        advanceWarning.style.display = confirmingAdvance ? DisplayStyle.Flex : DisplayStyle.None;
        if (confirmingAdvance)
        {
            advanceHeadline.text = guard.Headline;
            advanceDetail.text = guard.Detail;
        }

        footer.Clear();
        footer.Add(UiKit.Button("Approve Work Plan", () => { ui.ApproveWorkPlan(); Refresh(true); },
            m.CanEditWorkPlan && string.IsNullOrEmpty(rackProblem) && string.IsNullOrEmpty(approvalProblem)));
        footer.Add(UiKit.Button("Advance one year", OnAdvanceClicked, m.CanAdvanceYearNow && !ui.NeedsAnnualReview && string.IsNullOrEmpty(rackProblem), "btn-primary"));
        if (ui.NeedsAnnualReview)
            footer.Add(UiKit.Button("Read first annual results to continue", () => ui.ShowReview(), true, "btn-primary"));
        footer.Add(UiKit.Button("Close [Tab]", () => ui.CloseAll()));
    }

    private static string Signature(ScenarioOneManager m)
    {
        var sb = new StringBuilder();
        sb.Append(m.CurrentEcologicalYear).Append('|').Append(m.CashCents).Append('|').Append(m.Feedback).Append('|')
          .Append(m.AnnualReviewSeen).Append('|').Append(m.PlanningPlantingMethod).Append(m.PlanningInstallShelter).Append('|').Append(m.Outcome);
        foreach (ScenarioOneWorkOrder o in m.WorkOrders)
            if (o.IsOpen) sb.Append(o.workOrderId).Append(o.status).Append(o.fellingOutcome).Append(o.executionMethod).Append(o.installShelter).Append(o.validationMessage).Append(';');
        foreach (RackGroup rack in m.RackGroups) sb.Append('R').Append(rack.Id).Append(':').Append(rack.TreeCount).Append(';');
        foreach (ScenarioInventoryEntry e in m.Inventory) sb.Append(e.itemId).Append(e.quantity);
        return sb.ToString();
    }

    private VisualElement Card(string heading, string sub = null)
    {
        VisualElement card = UiKit.Box("card");
        UiKit.Add(card, heading, "heading");
        if (!string.IsNullOrEmpty(sub)) UiKit.Add(card, sub, "muted");
        scroll.Add(card);
        return card;
    }

    // Open orders that carry a problem, or were planned a couple of years ago. They are never expired or deleted: this
    // only says which ones to look at again, and why.
    private void BuildAttention(ScenarioOneManager m)
    {
        System.Collections.Generic.IReadOnlyList<StaleOrderNote> notes = m.StaleOrders;
        if (notes.Count == 0)
            return;
        VisualElement card = Card($"Orders needing attention · {notes.Count}", "Nothing is removed automatically. Change, cancel or approve each one, then check the quote.");
        card.name = "orders-attention";
        foreach (StaleOrderNote note in notes)
        {
            VisualElement row = UiKit.Row(card, "row-wrap");
            UiKit.Chip(row, note.Order.status == ScenarioWorkStatus.Approved ? "APPROVED" : "DRAFT", "chip-fell");
            UiKit.Add(row, note.Order.ShortLabel, "body", "gap");
            UiKit.Add(card, note.Reason, "muted");
        }
    }

    // The extraction racks, first in the plan: one card per rack (see BuildRack), under one heading that says what a rack is here.
    private void BuildRacks(ScenarioOneManager m)
    {
        List<RackGroup> racks = m.RackGroups.ToList();
        if (racks.Count == 0)
            return;
        List<ScenarioOneWorkOrder> fell = m.WorkOrders.Where(o => o.IsOpen && ScenarioOneManager.IsHarvestOrder(o)).OrderBy(o => o.workOrderId).ToList();
        ScenarioHarvestJob job = m.GetHarvestQuote(false);
        int trees = racks.Sum(r => r.TreeCount);
        VisualElement card = Card($"Extraction racks · {racks.Count} rack{(racks.Count == 1 ? "" : "s")}, {trees} trees",
            "Each rack is a corridor of trees felled so machinery can drive along it now and in later thinnings. Racks are draft work until you approve the plan.");
        card.name = "workplan-racks";
        foreach (RackGroup rack in racks)
            BuildRack(card, m, rack, fell, job);
    }

    private void BuildHarvest(ScenarioOneManager m)
    {
        ScenarioHarvestJob job = m.GetHarvestQuote(false);
        List<ScenarioOneWorkOrder> fell = m.WorkOrders.Where(o => o.IsOpen && ScenarioOneManager.IsHarvestOrder(o))
            .OrderBy(o => o.workOrderId).ToList();
        if (fell.Count == 0)
            return;
        VisualElement card = Card($"Harvest and salvage job · {fell.Count} stem(s)", "One commissioned contractor visit · Contractor only — specialist harvesting work");
        if (job.Resolution != null)
        {
            var revenueByProduct = job.Resolution.Quote.Timber.GroupBy(v => v.Batch.Assortment)
                .ToDictionary(g => g.Key, g => g.Sum(v => v.SaleRevenueCents));
            VisualElement head = UiKit.Box("table-row");
            head.Add(UiKit.Text("Product", "cell-name", "faint"));
            head.Add(UiKit.Text("Volume", "cell-num", "faint"));
            head.Add(UiKit.Text("Roadside value", "cell-num", "faint"));
            card.Add(head);
            foreach (AssortmentYieldSummaryLine line in job.Yield.Assortments.Select(a => new AssortmentYieldSummaryLine(a.Assortment, a.Disposition, a.VolumeCm3))
                         .GroupBy(l => l.Assortment + "|" + l.Disposition).Select(g => new AssortmentYieldSummaryLine(g.First().Assortment, g.First().Disposition, g.Sum(x => x.Volume))))
            {
                VisualElement row = UiKit.Box("table-row");
                string label = TimberAssortmentLabels.Label(line.Assortment);
                string name = line.Disposition == TimberDisposition.SellAndExtract ? label
                    : line.Disposition == TimberDisposition.KeepForUse ? label + " (kept for use)" : label + " (retained as deadwood)";
                row.Add(UiKit.Text(name, "cell-name", "body"));
                row.Add(UiKit.Text(UiKit.M3(line.Volume), "cell-num", "body"));
                long value = line.Disposition == TimberDisposition.SellAndExtract
                    && revenueByProduct.TryGetValue(line.Assortment, out long v) ? v : 0;
                row.Add(UiKit.Text(line.Disposition == TimberDisposition.SellAndExtract ? UiKit.Money(value) : "—", "cell-num", "body"));
                card.Add(row);
            }
            UiKit.Line(card, "Tops and residue (left on site)", UiKit.M3(job.ResidualVolumeCm3));
            if (job.RetainedVolumeCm3 > 0) UiKit.Line(card, "Retained usable timber", UiKit.M3(job.RetainedVolumeCm3));
            if (job.DeadwoodVolumeCm3 > 0) UiKit.Line(card, "Retained as fallen deadwood", UiKit.M3(job.DeadwoodVolumeCm3));
            var costs = job.Resolution.Quote.Costs;
            UiKit.Line(card, "Timber sales", UiKit.SignedMoney(job.RevenueCents), "money");
            UiKit.Line(card, "Harvesting work", UiKit.SignedMoney(-(costs.ContractorWorkCents - costs.MinimumJobAdjustmentCents)), "money-negative");
            if (costs.MinimumJobAdjustmentCents > 0)
                UiKit.Line(card, $"Small-job minimum (visit minimum {UiKit.Money(m.Definition.MinimumHarvestJobCents)})",
                    UiKit.SignedMoney(-costs.MinimumJobAdjustmentCents), "money-negative");
            long net = job.RevenueCents - job.CostCents;
            UiKit.Line(card, "Net for this job", UiKit.SignedMoney(net), net >= 0 ? "money" : "money-negative");
            if (costs.MinimumJobAdjustmentCents > 0)
                UiKit.Add(card, "This visit is small, so the contractor's minimum charge dominates. Combining more trees into one visit spreads that cost.", "muted");
        }
        if (fell.Any(order => order.type == ScenarioWorkType.SalvageDeadwood))
            UiKit.Add(card, "Windthrow salvage is optional. Remove or cancel a salvage order to leave that stem as deadwood. Basal damage and time since the fall can reduce timber grade; the contractor minimum still applies once to the combined visit.", "muted");
        UiKit.Add(card, job.Eligible ? "Re-quoted and settled when the year is resolved." : "Cannot proceed: " + job.Problem, job.Eligible ? "faint" : "body");
        if (m.BroadleafMarketActive)
            UiKit.Add(card, "If this visit includes Oak or Beech: " + ScenarioOneUiFacts.BroadleafQualityLimit + " " + ScenarioOneUiFacts.PremiumOakNote, "muted");
        if (job.HasUnmarketedSpecies)
            UiKit.Add(card, "No timber market is configured for some of these stems' species: they are charged for felling but earn no sale revenue.", "muted");

        // Extraction racks are shown as groups at the top of the plan; every other felling job stays on its own line here.
        var inRacks = new HashSet<int>(m.RackGroups.SelectMany(g => g.OrderIds));
        foreach (ScenarioOneWorkOrder order in fell.Where(o => !inRacks.Contains(o.workOrderId)))
        {
            VisualElement row = UiKit.Row(card, "row-wrap");
            UiKit.Chip(row, order.type == ScenarioWorkType.SalvageDeadwood ? "■ SALVAGE" : "■ FELL", "chip-fell");
            UiKit.Add(row, $"{order.targetTreeId} · {UiKit.F(order.expectedVolumeM3, "0.00")} m³ · {Status(order)}", "body", "gap");
            if (order.status == ScenarioWorkStatus.Pending)
            {
                foreach ((FellingMaterialOutcome choice, string label) in new[] {
                    (FellingMaterialOutcome.SellAndExtract, "Sell"), (FellingMaterialOutcome.KeepForUse, "Keep for use"),
                    (FellingMaterialOutcome.RetainAsFallenDeadwood, "Leave as deadwood") })
                {
                    if (order.type == ScenarioWorkType.SalvageDeadwood && choice == FellingMaterialOutcome.RetainAsFallenDeadwood) continue;
                    Button b = UiKit.Button(label, () => { m.SetPendingFellingOutcome(order.workOrderId, choice); Refresh(true); });
                    if (order.fellingOutcome == choice) b.AddToClassList("btn-selected");
                    row.Add(b);
                }
            }
            else
                UiKit.Add(row, OutcomeLabel(order.fellingOutcome), "muted", "gap");
            AddRemove(m, row, order);
            Problem(card, order);
        }
    }

    // One extraction rack: its trees, timber and income as a group, with one button to take the whole rack out of the plan again.
    private void BuildRack(VisualElement card, ScenarioOneManager m, RackGroup rack, List<ScenarioOneWorkOrder> fell, ScenarioHarvestJob job)
    {
        List<ScenarioOneWorkOrder> orders = fell.Where(o => rack.OrderIds.Contains(o.workOrderId)).OrderBy(o => o.workOrderId).ToList();
        if (orders.Count == 0)
            return;
        float volume = orders.Sum(o => o.expectedVolumeM3);
        long income = orders.Sum(o => o.expectedRevenueCents);
        float jobVolume = fell.Sum(o => o.expectedVolumeM3);
        // The contractor's cost is one figure for the whole visit; a rack's share is by stem volume, an estimate.
        long costShare = jobVolume > 0f ? (long)(job.CostCents * (double)volume / jobVolume) : 0L;
        bool approved = orders.All(o => o.status == ScenarioWorkStatus.Approved);
        VisualElement box = UiKit.Box("card");
        box.name = "rack-group-" + rack.Id;
        box.style.borderLeftWidth = 2;
        box.style.borderLeftColor = UiKit.RackColour;
        VisualElement head = UiKit.Row(box, "row-wrap");
        UiKit.Chip(head, "RACK", "chip-rack");
        UiKit.Add(head, $"Extraction rack {rack.Id} · {orders.Count} tree{(orders.Count == 1 ? "" : "s")}"
            + (rack.LayoutIndex >= 0 ? " · planned layout" : "") + " · " + (approved ? "APPROVED" : "DRAFT · not approved yet"), "heading", "gap");
        UiKit.Add(box, $"Timber {UiKit.F(volume, "0.0")} m³ · estimated income {UiKit.Money(income)} · share of harvesting cost about {UiKit.Money(costShare)}", "body");
        VisualElement buttons = UiKit.Row(box, "row-wrap");
        RackGroup captured = rack;
        buttons.Add(UiKit.Button(approved ? "Cancel approval" : "Remove rack", () => { m.RemoveRackGroup(captured.Id); Refresh(true); }));
        bool open = expandedRacks.Contains(rack.Id);
        // This expands the rack's real job list; it does not select trees in the forest. Use the normal enabled style.
        if (orders.Count > 0)
        {
            Button showTrees = UiKit.Button(open ? "Hide trees" : $"Show trees ({orders.Count})", () =>
            {
                if (!expandedRacks.Remove(captured.Id)) expandedRacks.Add(captured.Id);
                Refresh(true);
            });
            showTrees.name = "rack-show-trees-" + rack.Id;
            buttons.Add(showTrees);
        }
        if (open)
            foreach (ScenarioOneWorkOrder order in orders)
                UiKit.Add(box, $"{order.targetTreeId} · {UiKit.F(order.expectedVolumeM3, "0.00")} m³ · {Status(order)}", "muted");
        card.Add(box);
    }

    // A section the current objective does not use: greyed and disabled with one short explanation, never hidden.
    private void LockedCard(string heading)
    {
        VisualElement card = Card(heading, ScenarioOneWorkPlanFocus.LaterObjectiveNote);
        card.AddToClassList("card-locked");
        card.SetEnabled(false);
    }

    // Is planting offered as a normal action at the current teaching stage? The Work Plan and the HUD's key hint ask the same question here, so they
    // never disagree. It only decides what is presented: the planting mechanic, its orders and its costs are unchanged.
    public static bool PlantingOffered(ScenarioOneUiRoot ui)
    {
        ScenarioOneManager m = ui.Manager;
        return ScenarioOneWorkPlanFocus.PlantingOpen(ui.Learning.CurrentTopicIndex,
            m.WorkOrders.Any(o => o.type == ScenarioWorkType.PlantJuvenile) || m.Inventory.Any(e => e.quantity > 0));
    }

    private bool PlantingOpen(ScenarioOneManager m) => PlantingOffered(ui);

    private void BuildPlanting(ScenarioOneManager m)
    {
        if (!PlantingOpen(m)) { LockedCard("Planting"); return; }
        List<ScenarioOneWorkOrder> plant = m.WorkOrders.Where(o => o.IsOpen && o.type == ScenarioWorkType.PlantJuvenile)
            .OrderBy(o => o.workOrderId).ToList();
        string species = plant.Count == 0 ? "no planting spots marked"
            : string.Join(", ", plant.GroupBy(o => o.speciesId).Select(g => $"{g.Count()} {ScenarioOneUiFacts.SpeciesName(g.Key)}"));
        int sheltered = plant.Count(o => o.installShelter);
        VisualElement card = Card($"Planting · {species}", plant.Count > 0 ? $"{sheltered} with tree shelters · stock is consumed when planted" : "Press G while walking to mark planting spots.");

        VisualElement who = UiKit.Row(card, "row-wrap");
        UiKit.Add(who, "Who plants?", "body", "gap");
        foreach ((WorkExecutionMethod method, string label) in new[] { (WorkExecutionMethod.Contractor, "Contractor"), (WorkExecutionMethod.LandownerSimulated, "Me (landowner)") })
        {
            Button b = UiKit.Button(label, () => { m.SetPlantingExecution(method); Refresh(true); }, m.CanEditWorkPlan);
            if (m.PlanningPlantingMethod == method) b.AddToClassList("btn-selected");
            who.Add(b);
        }
        Button shelter = UiKit.Button(m.PlanningInstallShelter ? "Shelters: ON" : "Shelters: OFF",
            () => { m.SetPlantingShelters(!m.PlanningInstallShelter); Refresh(true); }, m.CanEditWorkPlan);
        if (m.PlanningInstallShelter) shelter.AddToClassList("btn-selected");
        who.Add(shelter);
        UiKit.Add(card, $"Tree shelter: {UiKit.Money(m.Definition.TreeShelterMaterialCents)} each (material, same executor as planting). "
            + "Choices apply to new planting spots and open planting orders; changed approved work needs approval again.", "muted");

        long cost = 0; int minutes = 0, ownerMinutes = 0;
        foreach (ScenarioOneWorkOrder order in plant.Where(o => string.IsNullOrEmpty(o.validationMessage)))
        {
            cost += order.estimatedCostCents;
            minutes += order.estimatedMinutes;
            if (order.executionMethod == WorkExecutionMethod.LandownerSimulated) ownerMinutes += order.estimatedMinutes;
        }
        if (plant.Count > 0)
        {
            foreach (var group in plant.GroupBy(o => o.stockItemId))
            {
                ScenarioShopEntry offer = m.Definition.FindShopEntry(group.Key);
                UiKit.Line(card, $"Saplings from stock · {(offer != null ? offer.displayName : group.Key)}",
                    $"{group.Sum(o => o.requiredStockQuantity)} used (own {m.GetStockQuantity(group.Key)})");
            }
            UiKit.Line(card, "Labour", ScenarioOneManager.FormatMinutes(minutes) + (ownerMinutes > 0 ? $" ({ScenarioOneManager.FormatMinutes(ownerMinutes)} your time)" : ""));
            UiKit.Line(card, "Cost (contractor work and shelter material)", UiKit.SignedMoney(-cost), "money-negative");
        }
        foreach (ScenarioOneWorkOrder order in plant)
        {
            VisualElement row = UiKit.Row(card, "row-wrap");
            UiKit.Add(row, $"#{order.workOrderId} {ScenarioOneUiFacts.SpeciesName(order.speciesId)} at ({UiKit.F(order.worldPosition.x, "0.0")}, {UiKit.F(order.worldPosition.z, "0.0")}) · "
                + $"{(order.executionMethod == WorkExecutionMethod.LandownerSimulated ? "you" : "contractor")} · "
                + $"{(order.installShelter ? "with shelter" : "no shelter")} · {ScenarioOneManager.FormatMinutes(order.estimatedMinutes)} · {UiKit.Money(order.estimatedCostCents)} · {Status(order)}", "body", "gap");
            AddRemove(m, row, order);
            Problem(card, order);
        }
    }

    private void BuildPruning(ScenarioOneManager m)
    {
        if (!ScenarioOneWorkPlanFocus.PruningOpen(ui.Learning.CurrentTopicIndex, m.WorkOrders.Any(o => o.type == ScenarioWorkType.PruneTree)))
        { LockedCard("Pruning"); return; }
        List<ScenarioOneWorkOrder> prune = m.WorkOrders.Where(o => o.IsOpen && o.type == ScenarioWorkType.PruneTree)
            .OrderBy(o => o.workOrderId).ToList();
        int eligible = m.EligibleCropTreePruningCount;
        VisualElement card = Card($"Pruning · {prune.Count} crop tree(s)", $"{m.CropTreeCount} crop trees designated (blue) · {eligible} eligible for a lift now · Contractor");
        UiKit.Add(card, "Pruning is a retained-tree timber-quality treatment and sets no price by itself. Where Oak timber is marketed, the recorded pruned section can make a large log eligible for a higher grade; Beech has no such class. Fork and defect shape is not modelled.", "muted");
        card.Add(UiKit.Button($"Add {eligible} eligible crop tree pruning task(s)", () => { m.BatchPruneCropTrees(); Refresh(true); }, eligible > 0 && m.CanEditWorkPlan));
        if (prune.Count > 0)
        {
            UiKit.Line(card, "Contractor labour", ScenarioOneManager.FormatMinutes(prune.Sum(o => o.estimatedMinutes)));
            UiKit.Line(card, "Cost", UiKit.SignedMoney(-prune.Sum(o => o.estimatedCostCents)), "money-negative");
        }
        foreach (ScenarioOneWorkOrder order in prune)
        {
            VisualElement row = UiKit.Row(card, "row-wrap");
            UiKit.Chip(row, "◆ CROP", "chip-crop");
            UiKit.Add(row, $"{order.targetTreeId} · clear stem to {UiKit.F(ScenarioOneManager.PruningTargetHeight(order), "0.0")} m · "
                + $"{ScenarioOneManager.FormatMinutes(order.estimatedMinutes)} · {UiKit.Money(order.estimatedCostCents)} · {Status(order)}", "body", "gap");
            AddRemove(m, row, order);
            Problem(card, order);
        }
    }

    private void BuildRemoval(ScenarioOneManager m)
    {
        List<ScenarioOneWorkOrder> removal = m.WorkOrders.Where(o => o.IsOpen && o.type == ScenarioWorkType.RemoveRegeneration)
            .OrderBy(o => o.workOrderId).ToList();
        if (removal.Count == 0) return;
        VisualElement card = Card($"Vegetation clearance · {removal.Count} map square{(removal.Count == 1 ? "" : "s")}", "Contractor clears competing vegetation in each marked area; standing trees remain.");
        foreach (ScenarioOneWorkOrder order in removal)
        {
            VisualElement row = UiKit.Row(card, "row-wrap");
            UiKit.Add(row, $"{(string.IsNullOrEmpty(order.speciesId) ? "All competing vegetation" : ScenarioOneUiFacts.SpeciesName(order.speciesId))} · square {UiKit.CellLabel(order.cellIndex, ui.Ecology.CellsPerAxis)} · "
                + $"{ScenarioOneManager.FormatMinutes(order.estimatedMinutes)} · {UiKit.Money(order.estimatedCostCents)} · {Status(order)}", "body", "gap");
            AddRemove(m, row, order);
            Problem(card, order);
        }
    }

    private void BuildNursery(ScenarioOneManager m)
    {
        if (!PlantingOpen(m)) { LockedCard("Nursery — planting stock"); return; }
        VisualElement card = Card("Nursery — planting stock", "Buying stock does not plant it. Contractor planting and shelter materials cost extra; unsheltered planting with your own time has no external labour charge. Review execution costs before spending your cash.");
        VisualElement qty = UiKit.Row(card, "row-wrap");
        UiKit.Add(qty, "Quantity:", "body", "gap");
        foreach (int amount in new[] { 1, 5, 10, 25 })
        {
            Button b = UiKit.Button(amount.ToString(), () => { purchaseQuantity = amount; Refresh(true); });
            if (purchaseQuantity == amount) b.AddToClassList("btn-selected");
            qty.Add(b);
        }
        foreach (ScenarioShopEntry offer in m.Definition.ShopEntries ?? new List<ScenarioShopEntry>())
        {
            if (offer == null) continue;
            VisualElement row = UiKit.Row(card, "row-wrap");
            UiKit.Add(row, $"{offer.displayName} · {UiKit.Money(offer.unitPriceCents)} each · own {m.GetStockQuantity(offer.itemId)} · "
                + $"reserved {m.GetReservedStockQuantity(offer.itemId)}", "body", "gap");
            long purchaseCost = (long)purchaseQuantity * offer.unitPriceCents;
            UiKit.Add(card, NurseryPurchaseSummary(m, offer, purchaseQuantity), "muted").name = "nursery-warning-" + offer.itemId;
            row.Add(UiKit.Button($"Buy {purchaseQuantity} · {UiKit.Money(purchaseCost)}", () => { m.TryPurchaseStock(offer.itemId, purchaseQuantity); Refresh(true); }, m.CanEditWorkPlan));
        }
    }

    // Read existing quotes and minimums; this warning never rejects a purchase.
    public static string NurseryPurchaseSummary(ScenarioOneManager m, ScenarioShopEntry offer, int quantity)
    {
        long cost = (long)quantity * offer.unitPriceCents;
        CashOutlookInput current = WorkPlanOverview.OutlookInput(m, m.GetHarvestQuote(false), m.GetHarvestQuote(true));
        CashOutlook after = CashOutlook.Evaluate(new CashOutlookInput(current.Cash - cost, current.MinimumHarvestCharge,
            current.ApprovedHarvestCost, current.ApprovedHarvestRevenue, current.AllHarvestCost, current.AllHarvestRevenue,
            current.ApprovedOtherCost, current.PendingOtherCost));
        string text = $"Purchase now: {UiKit.Money(cost)}. Cash after purchase: {UiKit.Money(m.CashCents - cost)}; "
            + $"uncommitted after approved work costs: {UiKit.Money(m.CashCents - cost - m.ReservedContractorCashCents)}. "
            + "Stock remains in the nursery inventory until planting work succeeds. ";
        if (cost > m.CashCents - m.ReservedContractorCashCents)
            text += "This purchase exceeds your uncommitted cash. ";
        string warning = WorkPlanOverview.WarningText(after, current.MinimumHarvestCharge);
        return text + (string.IsNullOrEmpty(warning)
            ? $"Expected cash after purchase and all planned work: {UiKit.Money(after.ExpectedIfAllApproved)}. Future planting work is extra; harvesting visits cost at least {UiKit.Money(current.MinimumHarvestCharge)}."
            : "After this purchase: " + warning);
    }

    private void BuildReference(ScenarioOneManager m)
    {
        if (!m.ReferenceArchiveAvailable) return;
        VisualElement card = Card("Reference Future v1 (frozen)", "Walk the verified historical forest at a milestone, then press Tab to return. Your forest and save are unchanged.");
        VisualElement row = UiKit.Row(card, "row-wrap");
        foreach (int year in new[] { 20, 50, 100 })
            row.Add(UiKit.Button("Visit year " + year, () => m.TryBeginReferencePreview(year)));
    }

    private void AddRemove(ScenarioOneManager m, VisualElement row, ScenarioOneWorkOrder order)
    {
        if (order.status == ScenarioWorkStatus.Pending)
            row.Add(UiKit.Button("Remove", () => { m.RemovePendingOrder(order.workOrderId); Refresh(true); }));
        else if (order.status == ScenarioWorkStatus.Approved)
            row.Add(UiKit.Button("Cancel approval", () => { m.CancelApprovedOrder(order.workOrderId); Refresh(true); }));
    }

    private static void Problem(VisualElement card, ScenarioOneWorkOrder order)
    {
        if (!string.IsNullOrEmpty(order.validationMessage))
            UiKit.Add(card, "Problem: " + ScenarioOneUiFacts.PlainText(order.validationMessage), "muted");
    }

    private static string Status(ScenarioOneWorkOrder order)
        => order.status == ScenarioWorkStatus.Pending ? "DRAFT · not approved yet"
            : order.status == ScenarioWorkStatus.Approved ? "APPROVED" : order.status.ToString().ToUpperInvariant();

    private static string OutcomeLabel(FellingMaterialOutcome outcome)
        => outcome == FellingMaterialOutcome.KeepForUse ? "Keep for use"
            : outcome == FellingMaterialOutcome.RetainAsFallenDeadwood ? "Leave as deadwood" : "Sell";

    private readonly struct AssortmentYieldSummaryLine
    {
        public readonly TimberAssortment Assortment; public readonly TimberDisposition Disposition; public readonly long Volume;
        public AssortmentYieldSummaryLine(TimberAssortment assortment, TimberDisposition disposition, long volume) { Assortment = assortment; Disposition = disposition; Volume = volume; }
    }
}
