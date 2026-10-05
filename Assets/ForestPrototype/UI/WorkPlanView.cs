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
    private int purchaseQuantity = 5;
    private string shownSignature = "";

    public WorkPlanView(ScenarioOneUiRoot ui)
    {
        this.ui = ui;
        Root = UiKit.Box("layer", "modal-backdrop");
        VisualElement modal = UiKit.Box("panel", "modal");
        Root.Add(modal);

        VisualElement header = UiKit.Box("modal-header");
        VisualElement left = new VisualElement();
        title = UiKit.Add(left, "", "title");
        owner = UiKit.Add(left, "", "subtitle");
        header.Add(left);
        VisualElement right = UiKit.Box("row");
        cash = UiKit.Add(right, "", "money", "title");
        right.Add(UiKit.Button("Annual review", () => ui.ShowReview()));
        right.Add(UiKit.Button("Stand map [N]", () => ui.ShowMap()));
        right.Add(UiKit.Button("Back to forest", () => ui.CloseAll()));
        header.Add(right);
        modal.Add(header);

        UiKit.Add(modal, "Tasks come from marks and planting spots you set while walking. Remove a task here; add new ones in the forest.", "muted");
        summary = UiKit.Add(modal, "", "body");

        scroll = new ScrollView(ScrollViewMode.Vertical);
        scroll.style.flexGrow = 1;
        modal.Add(scroll);

        feedback = UiKit.Add(modal, "", "muted");
        footer = UiKit.Box("modal-footer");
        modal.Add(footer);
    }

    public void Refresh(bool force)
    {
        ScenarioOneManager m = ui.Manager;
        string signature = Signature(m);
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
        BuildHarvest(m);
        BuildPlanting(m);
        BuildPruning(m);
        BuildRemoval(m);
        BuildNursery(m);
        BuildReference(m);
        scroll.schedule.Execute(() => scroll.scrollOffset = new UnityEngine.Vector2(0, scrollY));

        feedback.text = m.Outcome == ScenarioOneOutcome.Failed ? m.OutcomeReason : m.Feedback + "   ·   " + m.TutorialHint;
        footer.Clear();
        footer.Add(UiKit.Button("Add marked trees", () => { m.AddMarkedTreesToWorkPlan(); Refresh(true); }, m.CanEditWorkPlan));
        footer.Add(UiKit.Button("Approve pending work", () => { m.ApprovePendingWork(); Refresh(true); }, m.CanEditWorkPlan));
        footer.Add(UiKit.Button("Advance one year", () =>
        {
            if (m.AdvanceYear()) ui.ShowReview();
            else Refresh(true);
        }, m.CanAdvanceYearNow, "btn-primary"));
        footer.Add(UiKit.Button("Close [Tab]", () => ui.CloseAll()));
    }

    private static string Signature(ScenarioOneManager m)
    {
        var sb = new StringBuilder();
        sb.Append(m.CurrentEcologicalYear).Append('|').Append(m.CashCents).Append('|').Append(m.Feedback).Append('|')
          .Append(m.PlanningPlantingMethod).Append(m.PlanningInstallShelter).Append('|').Append(m.Outcome);
        foreach (ScenarioOneWorkOrder o in m.WorkOrders)
            if (o.IsOpen) sb.Append(o.workOrderId).Append(o.status).Append(o.fellingOutcome).Append(o.executionMethod).Append(o.installShelter).Append(o.validationMessage).Append(';');
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

    private void BuildHarvest(ScenarioOneManager m)
    {
        ScenarioHarvestJob job = m.GetHarvestQuote(false);
        List<ScenarioOneWorkOrder> fell = m.WorkOrders.Where(o => o.IsOpen && o.type == ScenarioWorkType.FellTree)
            .OrderBy(o => o.workOrderId).ToList();
        if (fell.Count == 0)
            return;
        VisualElement card = Card($"Thinning job · {fell.Count} tree(s)", "One commissioned contractor visit · Contractor only — specialist harvesting work");
        if (job.Resolution != null)
        {
            var revenueByProduct = job.Resolution.Quote.Timber.GroupBy(v => v.Batch.Assortment)
                .ToDictionary(g => g.Key, g => g.Sum(v => v.SaleRevenueCents));
            VisualElement head = UiKit.Box("table-row");
            head.Add(UiKit.Text("Product", "cell-name", "faint"));
            head.Add(UiKit.Text("Volume", "cell-num", "faint"));
            head.Add(UiKit.Text("Roadside value", "cell-num", "faint"));
            card.Add(head);
            foreach (AssortmentYieldSummaryLine line in job.Yield.Assortments.Select(a => new AssortmentYieldSummaryLine(a.Assortment.ToString(), a.Disposition, a.VolumeCm3))
                         .GroupBy(l => l.Name + "|" + l.Disposition).Select(g => new AssortmentYieldSummaryLine(g.First().Name, g.First().Disposition, g.Sum(x => x.Volume))))
            {
                VisualElement row = UiKit.Box("table-row");
                string name = line.Disposition == TimberDisposition.SellAndExtract ? line.Name
                    : line.Disposition == TimberDisposition.KeepForUse ? line.Name + " (kept for use)" : line.Name + " (retained as deadwood)";
                row.Add(UiKit.Text(name, "cell-name", "body"));
                row.Add(UiKit.Text(UiKit.M3(line.Volume), "cell-num", "body"));
                long value = line.Disposition == TimberDisposition.SellAndExtract
                    && System.Enum.TryParse(line.Name, out TimberAssortment asm) && revenueByProduct.TryGetValue(asm, out long v) ? v : 0;
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
        UiKit.Add(card, job.Eligible ? "Re-quoted and settled when the year is resolved." : "Cannot proceed: " + job.Problem, job.Eligible ? "faint" : "body");
        if (job.HasUnmarketedSpecies)
            UiKit.Add(card, "No broadleaf timber market is configured: those stems are charged for felling but earn no sale revenue.", "muted");

        foreach (ScenarioOneWorkOrder order in fell)
        {
            VisualElement row = UiKit.Row(card, "row-wrap");
            UiKit.Chip(row, "■ FELL", "chip-fell");
            UiKit.Add(row, $"{order.targetTreeId} · {UiKit.F(order.expectedVolumeM3, "0.00")} m³ · {Status(order)}", "body", "gap");
            if (order.status == ScenarioWorkStatus.Pending)
            {
                foreach ((FellingMaterialOutcome choice, string label) in new[] {
                    (FellingMaterialOutcome.SellAndExtract, "Sell"), (FellingMaterialOutcome.KeepForUse, "Keep for use"),
                    (FellingMaterialOutcome.RetainAsFallenDeadwood, "Leave as deadwood") })
                {
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

    private void BuildPlanting(ScenarioOneManager m)
    {
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
        List<ScenarioOneWorkOrder> prune = m.WorkOrders.Where(o => o.IsOpen && o.type == ScenarioWorkType.PruneTree)
            .OrderBy(o => o.workOrderId).ToList();
        int eligible = m.EligibleCropTreePruningCount;
        VisualElement card = Card($"Pruning · {prune.Count} crop tree(s)", $"{m.CropTreeCount} crop trees designated (blue) · {eligible} eligible for a lift now · Contractor");
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
        VisualElement card = Card($"Vegetation clearance · {removal.Count} cell(s)", "Contractor clears competing vegetation in each marked area; standing trees remain.");
        foreach (ScenarioOneWorkOrder order in removal)
        {
            VisualElement row = UiKit.Row(card, "row-wrap");
            UiKit.Add(row, $"{(string.IsNullOrEmpty(order.speciesId) ? "All competing vegetation" : ScenarioOneUiFacts.SpeciesName(order.speciesId))} · cell {UiKit.CellLabel(order.cellIndex, ui.Ecology.CellsPerAxis)} · "
                + $"{ScenarioOneManager.FormatMinutes(order.estimatedMinutes)} · {UiKit.Money(order.estimatedCostCents)} · {Status(order)}", "body", "gap");
            AddRemove(m, row, order);
            Problem(card, order);
        }
    }

    private void BuildNursery(ScenarioOneManager m)
    {
        VisualElement card = Card("Nursery — planting stock", "Saplings are paid for when bought and used when planted.");
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
            row.Add(UiKit.Button($"Buy {purchaseQuantity}", () => { m.TryPurchaseStock(offer.itemId, purchaseQuantity); Refresh(true); }, m.CanEditWorkPlan));
        }
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
            UiKit.Add(card, "Problem: " + order.validationMessage, "muted");
    }

    private static string Status(ScenarioOneWorkOrder order)
        => order.status == ScenarioWorkStatus.Pending ? "PENDING approval"
            : order.status == ScenarioWorkStatus.Approved ? "APPROVED" : order.status.ToString().ToUpperInvariant();

    private static string OutcomeLabel(FellingMaterialOutcome outcome)
        => outcome == FellingMaterialOutcome.KeepForUse ? "Keep for use"
            : outcome == FellingMaterialOutcome.RetainAsFallenDeadwood ? "Leave as deadwood" : "Sell";

    private readonly struct AssortmentYieldSummaryLine
    {
        public readonly string Name; public readonly TimberDisposition Disposition; public readonly long Volume;
        public AssortmentYieldSummaryLine(string name, TimberDisposition disposition, long volume) { Name = name; Disposition = disposition; Volume = volume; }
    }
}
