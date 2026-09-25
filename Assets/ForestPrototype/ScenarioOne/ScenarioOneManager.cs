using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public sealed class ScenarioOneManager : MonoBehaviour
{
    [SerializeField] private ScenarioOneDefinition definition;
    [SerializeField] private bool initialized;
    [SerializeField] private long cashCents;
    [SerializeField] private int nextWorkOrderId = 1;
    [SerializeField] private List<ScenarioOneWorkOrder> workOrders = new List<ScenarioOneWorkOrder>();
    [SerializeField] private List<ScenarioInventoryEntry> inventory = new List<ScenarioInventoryEntry>();
    [SerializeField] private List<ScenarioAnnualReport> annualReports = new List<ScenarioAnnualReport>();

    private ForestEcologyController ecology;
    private ForestPlayer player;
    private bool workPlanOpen;
    private string feedback = "";
    private Vector2 scroll;
    private string selectedShopItemId = "";
    private string purchaseQuantity = "1";
    private GUIStyle titleStyle;
    private GUIStyle headingStyle;
    private GUIStyle bodyStyle;
    private GUIStyle mutedStyle;
    private GUIStyle moneyStyle;
    private GUIStyle closedPromptStyle;
    private GUIStyle buttonStyle;
    private GUIStyle cellButtonStyle;
    private GUIStyle inputStyle;

    public ScenarioOneDefinition Definition => definition;
    public long CashCents => cashCents;
    public IReadOnlyList<ScenarioOneWorkOrder> WorkOrders => workOrders;
    public IReadOnlyList<ScenarioInventoryEntry> Inventory => inventory;
    public IReadOnlyList<ScenarioAnnualReport> AnnualReports => annualReports;
    public bool WorkPlanOpen => workPlanOpen;

    public void ConfigureDefinition(ScenarioOneDefinition configuredDefinition)
    {
        definition = configuredDefinition;
    }

    private void Awake()
    {
        ecology = GetComponent<ForestEcologyController>();
        if (ecology == null)
            ecology = UnityEngine.Object.FindFirstObjectByType<ForestEcologyController>();
        player = UnityEngine.Object.FindFirstObjectByType<ForestPlayer>();
        if (!initialized && definition != null)
            InitializeNewScenario();
        if (ecology != null)
            ecology.SetManagementAnnualControl(true);
    }

    private void OnDestroy()
    {
        if (ecology != null)
            ecology.SetManagementAnnualControl(false);
        SetWorkPlanOpen(false);
    }

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
            return;
        if (keyboard.tabKey.wasPressedThisFrame)
            SetWorkPlanOpen(!workPlanOpen);
        else if (workPlanOpen && keyboard.escapeKey.wasPressedThisFrame)
            SetWorkPlanOpen(false);
    }

    public void InitializeNewScenario()
    {
        if (definition == null)
            throw new InvalidOperationException("Scenario One requires a definition asset.");
        initialized = true;
        cashCents = definition.StartingCashCents;
        nextWorkOrderId = 1;
        workOrders.Clear();
        inventory.Clear();
        annualReports.Clear();
        selectedShopItemId = "";
        feedback = "Scenario started. Walk the stand, mark trees, then build the annual Work Plan.";
    }

    public int GetStockQuantity(string itemId)
    {
        ScenarioInventoryEntry entry = inventory.Find(item => item != null && item.itemId == itemId);
        return entry != null ? entry.quantity : 0;
    }

    public int GetReservedStockQuantity(string itemId)
    {
        return workOrders.Where(order => order.status == ScenarioWorkStatus.Approved && order.stockItemId == itemId)
            .Sum(order => order.requiredStockQuantity);
    }

    public long ReservedContractorCashCents => workOrders.Where(order => order.status == ScenarioWorkStatus.Approved)
        .Sum(order => order.estimatedCostCents);

    public bool TryPurchaseStock(string itemId, int quantity)
    {
        ScenarioShopEntry offer = definition != null ? definition.FindShopEntry(itemId) : null;
        if (offer == null || string.IsNullOrEmpty(offer.itemId) || string.IsNullOrEmpty(offer.speciesId)
            || offer.unitPriceCents < 0 || quantity <= 0)
        {
            feedback = "Choose a valid nursery item and a positive whole-number quantity.";
            return false;
        }
        int owned = GetStockQuantity(itemId);
        if (quantity > int.MaxValue - owned)
        {
            feedback = "Inventory quantity is too large.";
            return false;
        }
        long total = (long)quantity * offer.unitPriceCents;
        long reservedCash = ReservedContractorCashCents;
        if (total > cashCents - reservedCash)
        {
            feedback = $"Buying {quantity} {offer.displayName} needs {Money(total)}; "
                + $"uncommitted cash is {Money(cashCents - reservedCash)}.";
            return false;
        }

        ScenarioInventoryEntry entry = inventory.Find(item => item != null && item.itemId == itemId);
        if (entry == null)
        {
            entry = new ScenarioInventoryEntry { itemId = itemId };
            inventory.Add(entry);
        }
        entry.quantity += quantity;
        cashCents -= total;
        feedback = $"Bought {quantity} {offer.displayName} for {Money(total)}. In stock: {entry.quantity}.";
        return true;
    }

    // The Work Plan designates a stable ecology cell, not a free-floating
    // click position. Biology still decides whether planting succeeds at work time.
    public bool TryDesignatePlanting(string itemId, int cellIndex)
    {
        if (ecology == null)
            ecology = UnityEngine.Object.FindFirstObjectByType<ForestEcologyController>();
        ScenarioShopEntry offer = definition != null ? definition.FindShopEntry(itemId) : null;
        ForestTreeSpawner spawner = UnityEngine.Object.FindFirstObjectByType<ForestTreeSpawner>();
        TreeSpeciesDefinition species = offer != null && spawner != null ? spawner.ResolveSpecies(offer.speciesId) : null;
        if (ecology == null || ecology.Cells == null || cellIndex < 0 || cellIndex >= ecology.CellCount
            || offer == null || string.IsNullOrEmpty(offer.itemId) || species == null || !species.SupportsRegeneration
            || offer.plantingMinutes < 0)
        {
            feedback = "Choose an available sapling and a valid stand cell.";
            return false;
        }
        if (HasOpenPlantingOrder(species.SpeciesId, cellIndex))
        {
            feedback = "This species already has a planting order in that cell.";
            return false;
        }
        ForestRegenerationCohort cohort = ecology.Cells[cellIndex].FindCohort(species.SpeciesId);
        if (cohort != null && cohort.Density > 0f)
        {
            feedback = $"{species.DisplayName} is already regenerating in that cell.";
            return false;
        }

        int minutes = Mathf.Max(1, offer.plantingMinutes);
        workOrders.Add(new ScenarioOneWorkOrder
        {
            workOrderId = nextWorkOrderId++,
            type = ScenarioWorkType.PlantJuvenile,
            status = ScenarioWorkStatus.Pending,
            speciesId = species.SpeciesId,
            stockItemId = offer.itemId,
            requiredStockQuantity = 1,
            cellIndex = cellIndex,
            worldPosition = new Vector3(ecology.Cells[cellIndex].Center.x, 0f, ecology.Cells[cellIndex].Center.y),
            estimatedMinutes = minutes,
            estimatedCostCents = DivideRoundUp((long)minutes * definition.ContractorHourlyRateCents, 60L),
            createdYear = ecology.EcologicalYear
        });
        feedback = $"Designated {species.DisplayName} planting in cell {cellIndex}. "
            + "Stock and contractor cash are required for approval.";
        return true;
    }

    public int AddMarkedTreesToWorkPlan()
    {
        ForestTreeMarkingManager marking = UnityEngine.Object.FindFirstObjectByType<ForestTreeMarkingManager>();
        if (marking == null)
        {
            feedback = "No marking manager is available.";
            return 0;
        }

        List<string> ids = marking.GetMarkedIds();
        ids.Sort(StringComparer.Ordinal);
        var living = LivingTreesById();
        int added = 0;
        foreach (string id in ids)
        {
            if (HasOpenTreeOrder(id, ScenarioWorkType.FellTree))
                continue;
            if (!living.TryGetValue(id, out ForestTree tree) || tree == null || !tree.CanChop)
                continue;
            workOrders.Add(CreateFellingOrder(tree));
            added++;
        }
        if (added > 0)
            marking.ClearAll();
        feedback = added > 0
            ? $"Added {added} marked tree{(added == 1 ? "" : "s")} to the Work Plan."
            : "No new eligible marked trees were available.";
        return added;
    }

    public bool RemovePendingOrder(int workOrderId)
    {
        ScenarioOneWorkOrder order = workOrders.FirstOrDefault(candidate =>
            candidate.workOrderId == workOrderId && candidate.status == ScenarioWorkStatus.Pending);
        if (order == null)
            return false;
        workOrders.Remove(order);
        feedback = "Removed " + order.ShortLabel + ".";
        return true;
    }

    public bool CancelApprovedOrder(int workOrderId)
    {
        ScenarioOneWorkOrder order = workOrders.FirstOrDefault(candidate =>
            candidate.workOrderId == workOrderId && candidate.status == ScenarioWorkStatus.Approved);
        if (order == null)
            return false;
        workOrders.Remove(order);
        feedback = "Cancelled " + order.ShortLabel + ". Contractor cash and stock are available again.";
        return true;
    }

    public bool ApprovePendingWork()
    {
        ValidateOpenOrders();
        List<ScenarioOneWorkOrder> pending = workOrders
            .Where(order => order.status == ScenarioWorkStatus.Pending && string.IsNullOrEmpty(order.validationMessage))
            .OrderBy(order => order.workOrderId)
            .ToList();
        if (pending.Count == 0)
        {
            feedback = "There is no valid pending work to approve.";
            return false;
        }
        long cost = pending.Sum(order => order.estimatedCostCents);
        long alreadyApproved = workOrders.Where(order => order.status == ScenarioWorkStatus.Approved
            && string.IsNullOrEmpty(order.validationMessage)).Sum(order => order.estimatedCostCents);
        if (cost > cashCents - alreadyApproved)
        {
            feedback = $"Approval needs {Money(cost + alreadyApproved)} including approved work; available cash is {Money(cashCents)}.";
            return false;
        }
        foreach (ScenarioOneWorkOrder order in pending)
            order.status = ScenarioWorkStatus.Approved;
        feedback = $"Approved {pending.Count} task{(pending.Count == 1 ? "" : "s")} for {Money(cost)}.";
        return true;
    }

    public bool AdvanceYear()
    {
        if (ecology == null)
            ecology = UnityEngine.Object.FindFirstObjectByType<ForestEcologyController>();
        if (ecology == null)
        {
            feedback = "Annual advance is unavailable because the ecology controller is missing.";
            return false;
        }

        ValidateOpenOrders();
        List<ScenarioOneWorkOrder> approved = workOrders
            .Where(order => order.status == ScenarioWorkStatus.Approved)
            .OrderBy(order => order.workOrderId)
            .ToList();
        long requiredCash = approved.Where(order => string.IsNullOrEmpty(order.validationMessage))
            .Sum(order => order.estimatedCostCents);
        if (requiredCash > cashCents)
        {
            feedback = $"Approved work costs {Money(requiredCash)}; available cash is {Money(cashCents)}.";
            return false;
        }

        var report = new ScenarioAnnualReport { year = ecology.EcologicalYear + 1 };
        foreach (ScenarioOneWorkOrder order in approved)
            ResolveOrder(order, report);

        // Scenario One's single authoritative annual sequence: approved work,
        // immediate financial settlement, then exactly one Forestry annual step.
        ecology.AdvanceOneYear();
        report.closingCashCents = cashCents;
        annualReports.Add(report);
        feedback = $"Year {report.year} complete: {report.completedTasks} task(s), "
            + $"cost {Money(report.contractorCostCents)}, timber {Money(report.timberRevenueCents)}, "
            + $"closing cash {Money(cashCents)}.";
        return true;
    }

    public ScenarioOneSaveData CaptureSaveData()
    {
        return new ScenarioOneSaveData
        {
            scenarioId = definition != null ? definition.ScenarioId : "scenario-one",
            initialized = initialized,
            cashCents = cashCents,
            nextWorkOrderId = nextWorkOrderId,
            workOrders = CloneOrders(workOrders),
            inventory = CloneInventory(inventory),
            annualReports = CloneReports(annualReports)
        };
    }

    public void RestoreSaveData(ScenarioOneSaveData data)
    {
        if (data == null)
        {
            // Versions 1-9 had no scenario state. Loading one starts the
            // management layer from its configured opening position while the
            // legacy forest/ecology state continues to load normally.
            InitializeNewScenario();
            return;
        }
        if (definition != null && !string.IsNullOrEmpty(data.scenarioId) && data.scenarioId != definition.ScenarioId)
            Debug.LogWarning($"Save scenario '{data.scenarioId}' is being loaded into '{definition.ScenarioId}'.", this);
        initialized = data.initialized;
        cashCents = Math.Max(0L, data.cashCents);
        nextWorkOrderId = Mathf.Max(1, data.nextWorkOrderId);
        workOrders = CloneOrders(data.workOrders);
        inventory = CloneInventory(data.inventory);
        annualReports = CloneReports(data.annualReports);
        ValidateOpenOrders();
        feedback = "Scenario management state loaded.";
    }

    private ScenarioOneWorkOrder CreateFellingOrder(ForestTree tree)
    {
        float volume = tree.BiologicalStemVolumeM3;
        int minutes = Mathf.Max(1, Mathf.CeilToInt(definition.FellingBaseMinutes
            + volume * definition.FellingMinutesPerCubicMetre));
        long cost = DivideRoundUp((long)minutes * definition.ContractorHourlyRateCents, 60L);
        string speciesId = tree.Species != null ? tree.Species.SpeciesId : "";
        long revenue = (long)Math.Round(volume * definition.TimberValueCentsPerCubicMetre(speciesId),
            MidpointRounding.AwayFromZero);
        return new ScenarioOneWorkOrder
        {
            workOrderId = nextWorkOrderId++,
            type = ScenarioWorkType.FellTree,
            status = ScenarioWorkStatus.Pending,
            targetTreeId = tree.TreeId,
            speciesId = speciesId,
            worldPosition = tree.transform.position,
            fellingOutcome = FellingMaterialOutcome.SellAndExtract,
            estimatedMinutes = minutes,
            estimatedCostCents = cost,
            expectedRevenueCents = revenue,
            expectedVolumeM3 = volume,
            createdYear = ecology != null ? ecology.EcologicalYear : 0
        };
    }

    private void ResolveOrder(ScenarioOneWorkOrder order, ScenarioAnnualReport report)
    {
        if (!string.IsNullOrEmpty(order.validationMessage))
        {
            Fail(order, report, order.validationMessage);
            return;
        }
        switch (order.type)
        {
            case ScenarioWorkType.FellTree:
                ResolveFelling(order, report);
                break;
            case ScenarioWorkType.PlantJuvenile:
                ResolvePlanting(order, report);
                break;
            default:
                Fail(order, report, "This task type is not enabled in the current Scenario One slice.");
                break;
        }
    }

    private void ResolveFelling(ScenarioOneWorkOrder order, ScenarioAnnualReport report)
    {
        Dictionary<string, ForestTree> trees = LivingTreesById();
        if (!trees.TryGetValue(order.targetTreeId, out ForestTree tree) || tree == null || !tree.CanChop)
        {
            Fail(order, report, "Target tree is no longer eligible for felling.");
            return;
        }
        if (cashCents < order.estimatedCostCents)
        {
            Fail(order, report, "Insufficient cash when the contractor attempted the task.");
            return;
        }

        float volume = tree.BiologicalStemVolumeM3;
        string speciesId = tree.Species != null ? tree.Species.SpeciesId : order.speciesId;
        long revenue = order.fellingOutcome == FellingMaterialOutcome.SellAndExtract
            ? (long)Math.Round(volume * definition.TimberValueCentsPerCubicMetre(speciesId), MidpointRounding.AwayFromZero)
            : 0L;
        cashCents -= order.estimatedCostCents;
        cashCents += revenue;
        tree.Fell();
        order.status = ScenarioWorkStatus.Completed;
        order.resolvedYear = ecology.EcologicalYear + 1;
        order.expectedVolumeM3 = volume;
        order.expectedRevenueCents = revenue;
        report.completedTasks++;
        report.contractorCostCents += order.estimatedCostCents;
        report.timberRevenueCents += revenue;
        report.harvestedVolumeM3 += volume;
    }

    private void ResolvePlanting(ScenarioOneWorkOrder order, ScenarioAnnualReport report)
    {
        ScenarioShopEntry offer = definition != null ? definition.FindShopEntry(order.stockItemId) : null;
        ForestTreeSpawner spawner = UnityEngine.Object.FindFirstObjectByType<ForestTreeSpawner>();
        TreeSpeciesDefinition species = offer != null && spawner != null ? spawner.ResolveSpecies(offer.speciesId) : null;
        if (offer == null || species == null || species.SpeciesId != order.speciesId
            || !species.SupportsRegeneration || ecology.GetCellIndex(order.worldPosition) != order.cellIndex)
        {
            Fail(order, report, "Planting species, stock or target cell is unavailable.");
            return;
        }
        if (GetStockQuantity(order.stockItemId) < order.requiredStockQuantity || cashCents < order.estimatedCostCents)
        {
            Fail(order, report, "Insufficient stock or cash when the contractor attempted planting.");
            return;
        }

        PlantingResult result = ecology.TryPlantJuvenile(species, order.worldPosition);
        if (!result.Success)
        {
            Fail(order, report, result.Message);
            return;
        }
        ScenarioInventoryEntry stock = inventory.Find(item => item != null && item.itemId == order.stockItemId);
        stock.quantity -= order.requiredStockQuantity;
        cashCents -= order.estimatedCostCents;
        order.status = ScenarioWorkStatus.Completed;
        order.resolvedYear = report.year;
        report.completedTasks++;
        report.contractorCostCents += order.estimatedCostCents;
    }

    private static void Fail(ScenarioOneWorkOrder order, ScenarioAnnualReport report, string reason)
    {
        order.status = ScenarioWorkStatus.Failed;
        order.validationMessage = reason;
        report.failedTasks++;
    }

    private void ValidateOpenOrders()
    {
        Dictionary<string, ForestTree> trees = LivingTreesById();
        var reservedStock = new Dictionary<string, int>(StringComparer.Ordinal);
        var designatedCells = new HashSet<string>(StringComparer.Ordinal);
        // Approved work reserves its stock before pending work is evaluated.
        foreach (ScenarioOneWorkOrder order in workOrders.Where(item => item.IsOpen)
                     .OrderBy(item => item.status == ScenarioWorkStatus.Approved ? 0 : 1)
                     .ThenBy(item => item.workOrderId))
        {
            order.validationMessage = "";
            if (order.type == ScenarioWorkType.FellTree)
            {
                if (string.IsNullOrEmpty(order.targetTreeId))
                    order.validationMessage = "Missing target tree ID.";
                else if (!trees.TryGetValue(order.targetTreeId, out ForestTree tree) || tree == null || !tree.CanChop)
                    order.validationMessage = "Target tree is missing or already felled.";
            }
            else if (order.type == ScenarioWorkType.PlantJuvenile)
            {
                ScenarioShopEntry offer = definition != null ? definition.FindShopEntry(order.stockItemId) : null;
                ForestTreeSpawner spawner = UnityEngine.Object.FindFirstObjectByType<ForestTreeSpawner>();
                TreeSpeciesDefinition species = offer != null && spawner != null ? spawner.ResolveSpecies(offer.speciesId) : null;
                if (offer == null || species == null || !species.SupportsRegeneration || species.SpeciesId != order.speciesId
                    || order.requiredStockQuantity != 1)
                    order.validationMessage = "Planting stock or species is unavailable.";
                else if (ecology == null || order.cellIndex < 0 || order.cellIndex >= ecology.CellCount
                    || ecology.GetCellIndex(order.worldPosition) != order.cellIndex)
                    order.validationMessage = "Planting cell is outside the stand.";
                else if (!designatedCells.Add(order.speciesId + ":" + order.cellIndex))
                    order.validationMessage = "Another order already plants this species in this cell.";
                else
                {
                    ForestRegenerationCohort cohort = ecology.Cells[order.cellIndex].FindCohort(order.speciesId);
                    if (cohort != null && cohort.Density > 0f)
                        order.validationMessage = "This species is already regenerating in the cell.";
                    else
                    {
                        reservedStock.TryGetValue(order.stockItemId, out int reserved);
                        if (GetStockQuantity(order.stockItemId) <= reserved)
                            order.validationMessage = "Purchase more " + offer.displayName + " stock before approval.";
                        else
                            reservedStock[order.stockItemId] = reserved + 1;
                    }
                }
            }
            else
                order.validationMessage = "This task type is not yet available.";
        }
    }

    private bool HasOpenTreeOrder(string treeId, ScenarioWorkType type)
    {
        return workOrders.Any(order => order.type == type && order.targetTreeId == treeId && order.IsOpen);
    }

    private bool HasOpenPlantingOrder(string speciesId, int cellIndex)
    {
        return workOrders.Any(order => order.type == ScenarioWorkType.PlantJuvenile && order.IsOpen
            && order.speciesId == speciesId && order.cellIndex == cellIndex);
    }

    private static Dictionary<string, ForestTree> LivingTreesById()
    {
        var result = new Dictionary<string, ForestTree>(StringComparer.Ordinal);
        foreach (ForestTree tree in UnityEngine.Object.FindObjectsByType<ForestTree>(
                     FindObjectsInactive.Exclude, FindObjectsSortMode.None))
        {
            if (tree != null && !tree.IsStump && !string.IsNullOrEmpty(tree.TreeId))
                result[tree.TreeId] = tree;
        }
        return result;
    }

    private void SetWorkPlanOpen(bool open)
    {
        if (workPlanOpen == open)
            return;
        workPlanOpen = open;
        if (player == null)
            player = UnityEngine.Object.FindFirstObjectByType<ForestPlayer>();
        if (player != null)
            player.enabled = !open;
        Cursor.lockState = open ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = open;
    }

    private void OnGUI()
    {
        if (!workPlanOpen)
        {
            DrawClosedPrompt();
            return;
        }
        EnsureStyles();
        float scale = ForestHud.Scale;
        float width = Mathf.Min(1120f * scale, Screen.width - 32f);
        float height = Mathf.Min(760f * scale, Screen.height - 32f);
        Rect panel = new Rect((Screen.width - width) * 0.5f, (Screen.height - height) * 0.5f, width, height);
        ForestHud.Panel(panel);
        GUILayout.BeginArea(new Rect(panel.x + 22f, panel.y + 18f, panel.width - 44f, panel.height - 36f));
        GUILayout.Label("SCENARIO ONE — ANNUAL WORK PLAN", titleStyle);
        GUILayout.BeginHorizontal();
        GUILayout.Label($"Ecological year {CurrentYear}", headingStyle);
        GUILayout.FlexibleSpace();
        GUILayout.Label(Money(cashCents), moneyStyle);
        GUILayout.EndHorizontal();
        GUILayout.Space(8f);

        ValidateOpenOrders();
        WorkPlanTotals totals = CalculateTotals();
        GUILayout.Label($"Open tasks: {totals.openCount}   Contractor time: {Minutes(totals.minutes)}   "
            + $"Contractor cost: {Money(totals.costCents)}   Expected timber: {Money(totals.revenueCents)}   "
            + $"Expected net: {Money(totals.revenueCents - totals.costCents)}", bodyStyle);
        GUILayout.Label($"Approved contractor reserve: {Money(ReservedContractorCashCents)}   "
            + $"Uncommitted cash: {Money(cashCents - ReservedContractorCashCents)}", bodyStyle);
        GUILayout.Space(8f);

        scroll = GUILayout.BeginScrollView(scroll, GUILayout.ExpandHeight(true));
        DrawNursery();
        GUILayout.Space(12f);
        DrawPlantingGrid();
        GUILayout.Space(12f);
        GUILayout.Label("WORK ORDERS", headingStyle);
        List<ScenarioOneWorkOrder> visible = workOrders.Where(order => order.IsOpen)
            .OrderBy(order => order.workOrderId).ToList();
        if (visible.Count == 0)
            GUILayout.Label("No work is planned. Mark trees or designate planting cells above.", bodyStyle);
        foreach (ScenarioOneWorkOrder order in visible)
            DrawOrder(order);
        GUILayout.EndScrollView();

        GUILayout.Space(8f);
        if (!string.IsNullOrEmpty(feedback))
            GUILayout.Label(feedback, bodyStyle);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Add marked trees", buttonStyle, GUILayout.Height(42f * scale)))
            AddMarkedTreesToWorkPlan();
        if (GUILayout.Button("Approve pending work", buttonStyle, GUILayout.Height(42f * scale)))
            ApprovePendingWork();
        GUI.enabled = totals.approvedCount == 0 || totals.approvedCostCents <= cashCents;
        if (GUILayout.Button("Advance one year", buttonStyle, GUILayout.Height(42f * scale)))
            AdvanceYear();
        GUI.enabled = true;
        if (GUILayout.Button("Close", buttonStyle, GUILayout.Height(42f * scale)))
            SetWorkPlanOpen(false);
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    private void DrawOrder(ScenarioOneWorkOrder order)
    {
        GUILayout.BeginVertical(GUI.skin.box);
        GUILayout.BeginHorizontal();
        GUILayout.Label($"#{order.workOrderId}  {order.ShortLabel}", headingStyle);
        GUILayout.FlexibleSpace();
        GUILayout.Label(order.status.ToString(), bodyStyle, GUILayout.Width(100f * ForestHud.Scale));
        GUILayout.EndHorizontal();
        if (order.type == ScenarioWorkType.PlantJuvenile)
            GUILayout.Label($"{Minutes(order.estimatedMinutes)} · contractor {Money(order.estimatedCostCents)} · "
                + $"stock: {order.requiredStockQuantity} {order.stockItemId} · cell {order.cellIndex}", bodyStyle);
        else
            GUILayout.Label($"{Minutes(order.estimatedMinutes)} · cost {Money(order.estimatedCostCents)} · "
                + $"{order.expectedVolumeM3:0.00} m³ · expected revenue {Money(order.expectedRevenueCents)}", bodyStyle);
        if (!string.IsNullOrEmpty(order.validationMessage))
            GUILayout.Label("Problem: " + order.validationMessage, mutedStyle);
        if (order.status == ScenarioWorkStatus.Pending && GUILayout.Button("Remove from plan", buttonStyle))
            RemovePendingOrder(order.workOrderId);
        if (order.status == ScenarioWorkStatus.Approved && GUILayout.Button("Cancel approved work", buttonStyle))
            CancelApprovedOrder(order.workOrderId);
        GUILayout.EndVertical();
    }

    private void DrawNursery()
    {
        GUILayout.Label("NURSERY — BUY PLANTING STOCK", headingStyle);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Quantity to buy (whole saplings):", bodyStyle, GUILayout.Width(310f * ForestHud.Scale));
        purchaseQuantity = GUILayout.TextField(purchaseQuantity, 12, inputStyle,
            GUILayout.Width(110f * ForestHud.Scale), GUILayout.Height(30f * ForestHud.Scale));
        GUILayout.EndHorizontal();
        if (definition == null || definition.ShopEntries == null)
            return;
        foreach (ScenarioShopEntry offer in definition.ShopEntries)
        {
            if (offer == null)
                continue;
            GUILayout.BeginHorizontal(GUI.skin.box);
            GUILayout.Label($"{offer.displayName} · {Money(offer.unitPriceCents)} each · "
                + $"owned {GetStockQuantity(offer.itemId)} · reserved {GetReservedStockQuantity(offer.itemId)}",
                bodyStyle, GUILayout.Width(590f * ForestHud.Scale));
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(selectedShopItemId == offer.itemId ? "Selected" : "Select", buttonStyle,
                    GUILayout.Width(100f * ForestHud.Scale), GUILayout.Height(30f * ForestHud.Scale)))
                selectedShopItemId = offer.itemId;
            if (GUILayout.Button("Buy", buttonStyle, GUILayout.Width(80f * ForestHud.Scale),
                    GUILayout.Height(30f * ForestHud.Scale)))
            {
                if (int.TryParse(purchaseQuantity, out int amount))
                    TryPurchaseStock(offer.itemId, amount);
                else
                    feedback = "Enter a positive whole-number quantity to purchase.";
            }
            GUILayout.EndHorizontal();
        }
    }

    private void DrawPlantingGrid()
    {
        ScenarioShopEntry selected = definition != null ? definition.FindShopEntry(selectedShopItemId) : null;
        GUILayout.Label("PLANTING DESIGNATIONS", headingStyle);
        GUILayout.Label(selected != null
            ? $"Selected: {selected.displayName}. Choose a stand cell; each order needs one sapling and contractor time. "
                + "Occupied cells can still be designated for another species when space permits."
            : "Select a nursery species above, then click a stand cell.", bodyStyle);
        if (ecology == null || ecology.Cells == null)
            return;
        int axis = ecology.CellsPerAxis;
        for (int z = axis - 1; z >= 0; z--)
        {
            GUILayout.BeginHorizontal();
            for (int x = 0; x < axis; x++)
            {
                int index = z * axis + x;
                ForestRegenerationCohort cohort = selected != null
                    ? ecology.Cells[index].FindCohort(selected.speciesId) : null;
                bool available = selected != null && (cohort == null || cohort.Density <= 0f)
                    && !HasOpenPlantingOrder(selected.speciesId, index);
                GUI.enabled = available;
                if (GUILayout.Button($"{x + 1},{z + 1}", cellButtonStyle,
                        GUILayout.Width(66f * ForestHud.Scale), GUILayout.Height(30f * ForestHud.Scale)))
                    TryDesignatePlanting(selectedShopItemId, index);
                GUI.enabled = true;
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.Label("Cells are numbered west to east, south to north. Grey cells already have this species or an open designation.", bodyStyle);
    }

    private void DrawClosedPrompt()
    {
        if (closedPromptStyle == null)
        {
            closedPromptStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.UpperRight,
                fontStyle = FontStyle.Bold
            };
            closedPromptStyle.normal.textColor = new Color(0.9f, 0.92f, 0.82f);
        }
        closedPromptStyle.fontSize = Mathf.RoundToInt(18f * ForestHud.Scale);
        GUI.Label(new Rect(Screen.width - 360f * ForestHud.Scale, 18f, 340f * ForestHud.Scale, 30f * ForestHud.Scale),
            "[Tab] Work Plan", closedPromptStyle);
    }

    private void EnsureStyles()
    {
        if (titleStyle != null)
            return;
        float scale = ForestHud.Scale;
        titleStyle = new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold, fontSize = Mathf.RoundToInt(26f * scale) };
        headingStyle = new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold, fontSize = Mathf.RoundToInt(20f * scale) };
        bodyStyle = new GUIStyle(GUI.skin.label) { fontSize = Mathf.RoundToInt(17f * scale), wordWrap = true };
        mutedStyle = new GUIStyle(bodyStyle);
        mutedStyle.normal.textColor = new Color(1f, 0.72f, 0.55f);
        moneyStyle = new GUIStyle(headingStyle) { alignment = TextAnchor.MiddleRight };
        moneyStyle.normal.textColor = new Color(0.65f, 1f, 0.68f);
        buttonStyle = new GUIStyle(GUI.skin.button)
        {
            fontSize = Mathf.RoundToInt(16f * scale),
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold
        };
        cellButtonStyle = new GUIStyle(buttonStyle) { fontSize = Mathf.RoundToInt(15f * scale) };
        inputStyle = new GUIStyle(GUI.skin.textField) { fontSize = Mathf.RoundToInt(17f * scale) };
    }

    private WorkPlanTotals CalculateTotals()
    {
        var result = new WorkPlanTotals();
        foreach (ScenarioOneWorkOrder order in workOrders)
        {
            if (!order.IsOpen)
                continue;
            result.openCount++;
            result.minutes += order.estimatedMinutes;
            result.costCents += order.estimatedCostCents;
            result.revenueCents += order.expectedRevenueCents;
            if (order.status == ScenarioWorkStatus.Approved)
            {
                result.approvedCount++;
                result.approvedCostCents += order.estimatedCostCents;
            }
        }
        return result;
    }

    private int CurrentYear => ecology != null ? ecology.EcologicalYear : 0;

    private static long DivideRoundUp(long value, long divisor) => (value + divisor - 1L) / divisor;
    private static string Money(long cents) => $"€{cents / 100.0:0.00}";
    private static string Minutes(int minutes) => minutes < 60 ? minutes + " min" : $"{minutes / 60f:0.0} h";

    private static List<ScenarioOneWorkOrder> CloneOrders(List<ScenarioOneWorkOrder> source)
    {
        if (source == null) return new List<ScenarioOneWorkOrder>();
        return source.Select(item => JsonUtility.FromJson<ScenarioOneWorkOrder>(JsonUtility.ToJson(item))).ToList();
    }

    private static List<ScenarioInventoryEntry> CloneInventory(List<ScenarioInventoryEntry> source)
    {
        if (source == null) return new List<ScenarioInventoryEntry>();
        return source.Select(item => new ScenarioInventoryEntry { itemId = item.itemId, quantity = item.quantity }).ToList();
    }

    private static List<ScenarioAnnualReport> CloneReports(List<ScenarioAnnualReport> source)
    {
        if (source == null) return new List<ScenarioAnnualReport>();
        return source.Select(item => new ScenarioAnnualReport
        {
            year = item.year,
            completedTasks = item.completedTasks,
            failedTasks = item.failedTasks,
            contractorCostCents = item.contractorCostCents,
            timberRevenueCents = item.timberRevenueCents,
            harvestedVolumeM3 = item.harvestedVolumeM3,
            closingCashCents = item.closingCashCents
        }).ToList();
    }

    private struct WorkPlanTotals
    {
        public int openCount;
        public int approvedCount;
        public int minutes;
        public long costCents;
        public long approvedCostCents;
        public long revenueCents;
    }
}
