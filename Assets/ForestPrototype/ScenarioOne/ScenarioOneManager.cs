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
    [SerializeField] private int nextManagementEventId = 1;
    [SerializeField] private List<ScenarioManagementEvent> managementEvents = new List<ScenarioManagementEvent>();
    [SerializeField] private List<ScenarioEcologicalSnapshot> ecologicalSnapshots = new List<ScenarioEcologicalSnapshot>();
    [SerializeField] private List<ScenarioUnderstoreyCell> understoreyCells = new List<ScenarioUnderstoreyCell>();
    [SerializeField] private int nextDeadwoodId = 1;
    [SerializeField] private List<ScenarioDeadwoodRecord> deadwoodRecords = new List<ScenarioDeadwoodRecord>();
    [SerializeField] private ScenarioSoundscapeState soundscapeState = new ScenarioSoundscapeState();
    [SerializeField] private FellingMaterialOutcome planningFellingOutcome = FellingMaterialOutcome.SellAndExtract;
    [SerializeField] private ScenarioOneOutcome outcome;
    [SerializeField] private int outcomeYear = -1;
    [SerializeField] private string outcomeReason = "";
    [SerializeField] private bool annualReviewSeen;
    [SerializeField] private ScenarioCenturyReview centuryReview;
    [SerializeField] private float retainedTimberM3;
    [SerializeField] private List<PlantedJuvenile> plantedJuveniles = new List<PlantedJuvenile>();
    [SerializeField] private List<PlantingClearancePatch> clearancePatches = new List<PlantingClearancePatch>();
    private int nextJuvenileId = 1;
    private readonly Dictionary<int, GameObject> plantingMarkers = new Dictionary<int, GameObject>();
    private Material plantingMarkerMaterial;
    private Material approvedPlantingMaterial;

    private ForestEcologyController ecology;
    private ForestPlayer player;
    private ScenarioReferenceArchive referenceArchive;
    private ForestSaveData previewReturnData;
    private Vector3 previewPlayerPosition;
    private Quaternion previewPlayerRotation;
    private Quaternion previewCameraRotation;
    private float previewPlayerPitch;
    private int previewYear;
    private bool referencePreviewActive;
    private bool referenceAuthoring;
    private Material deadwoodMaterial;
    [SerializeField] private GameObject fellingResidueGreenPrefab;
    [SerializeField] private GameObject fellingResidueGreenAltPrefab;
    [SerializeField] private GameObject fellingResidueDryPrefab;
    [SerializeField] private GameObject fellingResidueDryAltPrefab;
    private ScenarioOneSoundscapePlayer soundscapePlayer;
    private ScenarioHabitatVisuals habitatVisuals;
    private bool workPlanOpen;
    private string feedback = "";
    private Vector2 scroll;
    private string selectedShopItemId = "";
    private string selectedRemovalSpeciesId = "";
    private string purchaseQuantity = "1";
    private bool annualReviewOpen;
    private GUIStyle titleStyle;
    private GUIStyle headingStyle;
    private GUIStyle bodyStyle;
    private GUIStyle mutedStyle;
    private GUIStyle moneyStyle;
    private GUIStyle closedPromptStyle;
    private GUIStyle buttonStyle;
    private GUIStyle cellButtonStyle;
    private GUIStyle inputStyle;
    private Texture2D buttonFace;
    private Texture2D buttonFaceHover;
    private Texture2D buttonFacePressed;

    public ScenarioOneDefinition Definition => definition;
    public long CashCents => cashCents;
    public IReadOnlyList<ScenarioOneWorkOrder> WorkOrders => workOrders;
    public IReadOnlyList<ScenarioInventoryEntry> Inventory => inventory;
    public IReadOnlyList<ScenarioAnnualReport> AnnualReports => annualReports;
    public IReadOnlyList<ScenarioManagementEvent> ManagementEvents => managementEvents;
    public IReadOnlyList<ScenarioEcologicalSnapshot> EcologicalSnapshots => ecologicalSnapshots;
    public IReadOnlyList<ScenarioUnderstoreyCell> UnderstoreyCells => understoreyCells;
    public IReadOnlyList<ScenarioDeadwoodRecord> DeadwoodRecords => deadwoodRecords;
    public ScenarioSoundscapeState SoundscapeState => soundscapeState;
    public ScenarioOneOutcome Outcome => outcome;
    public int OutcomeYear => outcomeYear;
    public string OutcomeReason => outcomeReason;
    public ScenarioCenturyReview CenturyReview => centuryReview;
    public float RetainedTimberM3 => retainedTimberM3;
    public string Feedback => feedback;
    public IReadOnlyList<PlantedJuvenile> PlantedJuveniles => plantedJuveniles;
    public IReadOnlyList<PlantingClearancePatch> ClearancePatches => clearancePatches;

    // Exact-position planting: a pending order reserves one owned sapling;
    // the juvenile itself is created only when approved contractor work resolves.
    public bool TryDesignateExactPlanting(string itemId, Vector3 groundPosition)
    {
        if (!CanManage()) return false;
        ScenarioShopEntry offer = definition?.FindShopEntry(itemId);
        ForestTreeSpawner spawner = UnityEngine.Object.FindFirstObjectByType<ForestTreeSpawner>();
        TreeSpeciesDefinition species = offer != null && spawner != null ? spawner.ResolveSpecies(offer.speciesId) : null;
        if (offer == null || species == null || !species.SupportsRegeneration)
        {
            feedback = "Choose a valid nursery species.";
            return false;
        }
        if (GetStockQuantity(itemId) <= GetReservedStockQuantity(itemId))
        {
            feedback = $"No {offer.displayName} ready to plant. Purchase more stock.";
            return false;
        }
        if (HasOpenExactPlantingAt(groundPosition))
        {
            feedback = "A planting marker already exists at this position.";
            return false;
        }
        int cellIndex = ecology?.GetCellIndex(groundPosition) ?? -1;
        if (cellIndex < 0)
        {
            feedback = "Planting position is outside the stand.";
            return false;
        }
        if (LivingTreesById().Values.Any(tree => Vector2.Distance(
                new Vector2(tree.transform.position.x, tree.transform.position.z),
                new Vector2(groundPosition.x, groundPosition.z)) < 0.7f))
        {
            feedback = "Choose a position clear of standing tree stems.";
            return false;
        }

        int minutes = Mathf.Max(1, offer.plantingMinutes);
        var order = new ScenarioOneWorkOrder
        {
            workOrderId = nextWorkOrderId++,
            type = ScenarioWorkType.PlantJuvenile,
            status = ScenarioWorkStatus.Pending,
            speciesId = species.SpeciesId,
            stockItemId = itemId,
            requiredStockQuantity = 1,
            cellIndex = cellIndex,
            worldPosition = groundPosition,
            exactPosition = true,
            estimatedMinutes = minutes,
            estimatedCostCents = DivideRoundUp((long)minutes * definition.ContractorHourlyRateCents, 60L),
            createdYear = ecology.EcologicalYear
        };
        workOrders.Add(order);
        ShowPlantingMarker(order);
        RecordOrderEvent(order, ScenarioManagementEventType.OrderCreated, ScenarioManagementOutcome.None, CurrentYear);
        feedback = $"Marked {species.DisplayName} planting at ({groundPosition.x:0.0}, {groundPosition.z:0.0}).";
        return true;
    }

    private bool HasOpenExactPlantingAt(Vector3 position)
    {
        return workOrders.Any(order => order.type == ScenarioWorkType.PlantJuvenile && order.IsOpen
            && Vector2.Distance(new Vector2(order.worldPosition.x, order.worldPosition.z),
                new Vector2(position.x, position.z)) < 0.5f)
            || plantedJuveniles.Any(j => j.alive && string.IsNullOrEmpty(j.promotedTreeId)
                && Vector2.Distance(new Vector2(j.position.x, j.position.z),
                    new Vector2(position.x, position.z)) < 0.5f);
    }

    private void ShowPlantingMarker(ScenarioOneWorkOrder order)
    {
        if (!order.exactPosition || !order.IsOpen || plantingMarkers.ContainsKey(order.workOrderId))
            return;
        Shader shader = Shader.Find("Universal Render Pipeline/Unlit") ?? Shader.Find("Unlit/Color");
        if (shader == null)
            return;
        if (plantingMarkerMaterial == null)
            plantingMarkerMaterial = new Material(shader) { name = "Planting site", color = new Color(1f, 0.83f, 0.15f) };
        if (approvedPlantingMaterial == null)
            approvedPlantingMaterial = new Material(shader) { name = "Approved planting site", color = new Color(0.2f, 0.9f, 0.45f) };
        var marker = new GameObject("Planting Marker #" + order.workOrderId);
        marker.transform.SetParent(transform, true);
        marker.transform.position = order.worldPosition;
        GameObject peg = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        peg.name = "Planting peg";
        peg.transform.SetParent(marker.transform, false);
        peg.transform.localPosition = new Vector3(0f, 0.55f, 0f);
        peg.transform.localScale = new Vector3(0.09f, 0.55f, 0.09f);
        Destroy(peg.GetComponent<Collider>());
        GameObject disc = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        disc.name = "1 m² planting site";
        disc.transform.SetParent(marker.transform, false);
        disc.transform.localPosition = new Vector3(0f, 0.035f, 0f);
        disc.transform.localScale = new Vector3(1.128f, 0.012f, 1.128f);
        Destroy(disc.GetComponent<Collider>());
        Material material = order.status == ScenarioWorkStatus.Approved
            ? approvedPlantingMaterial : plantingMarkerMaterial;
        peg.GetComponent<Renderer>().sharedMaterial = material;
        disc.GetComponent<Renderer>().sharedMaterial = material;
        plantingMarkers[order.workOrderId] = marker;
    }

    private void RemovePlantingMarker(int workOrderId)
    {
        if (!plantingMarkers.TryGetValue(workOrderId, out GameObject marker))
            return;
        if (marker != null)
            Destroy(marker);
        plantingMarkers.Remove(workOrderId);
    }

    private void ClearPlantingMarkers()
    {
        foreach (GameObject marker in plantingMarkers.Values)
            if (marker != null)
                Destroy(marker);
        plantingMarkers.Clear();
    }

    private void RefreshPlantingMarkers()
    {
        ClearPlantingMarkers();
        foreach (ScenarioOneWorkOrder order in workOrders)
            ShowPlantingMarker(order);
    }

    // All-or-nothing debit: a build cannot partially consume a stockpile and fail.
    public float TrySpendRetainedTimber(float requestedVolume)
    {
        if (float.IsNaN(requestedVolume) || float.IsInfinity(requestedVolume)
            || requestedVolume <= 0f || retainedTimberM3 + 0.00001f < requestedVolume)
            return 0f;
        retainedTimberM3 = Mathf.Max(0f, retainedTimberM3 - requestedVolume);
        return requestedVolume;
    }
    public int BatchPruneCropTrees()
    {
        if (!CanManage()) return 0;
        if (definition == null) return 0;
        Dictionary<string, ForestTree> trees = LivingTreesById();
        var cropTrees = trees.Values.Where(t => t != null && t.IsCropTree && t.CanChop)
            .OrderBy(t => t.TreeId, StringComparer.Ordinal).ToList();
        int eligible = 0, ineligible = 0, alreadyTreated = 0, scheduled = 0;
        foreach (ForestTree tree in cropTrees)
        {
            if (HasOpenTreeOrder(tree.TreeId, ScenarioWorkType.FellTree))
            {
                ineligible++;
                continue;
            }
            if (HasOpenTreeOrder(tree.TreeId, ScenarioWorkType.PruneTree))
            {
                scheduled++;
                continue;
            }
            float target = definition.NextPruningTargetHeightM(tree.PruningLifts);
            if (target <= 0f)
            {
                alreadyTreated++;
                continue;
            }
            if (target >= tree.Height * 0.6f || tree.CanPrune(target, CurrentYear + 1) != null)
            {
                ineligible++;
                continue;
            }
            eligible++;
            int minutes = Mathf.Max(1, Mathf.CeilToInt(definition.PruningBaseMinutes
                + target * definition.PruningMinutesPerMetre));
            var order = new ScenarioOneWorkOrder
            {
                workOrderId = nextWorkOrderId++,
                type = ScenarioWorkType.PruneTree,
                status = ScenarioWorkStatus.Pending,
                targetTreeId = tree.TreeId,
                speciesId = tree.Species?.SpeciesId ?? "",
                worldPosition = tree.transform.position,
                cellIndex = ecology?.GetCellIndex(tree.transform.position) ?? -1,
                estimatedMinutes = minutes,
                estimatedCostCents = DivideRoundUp((long)minutes * definition.ContractorHourlyRateCents, 60L),
                expectedVolumeM3 = 0f,
                requiresCropTree = true,
                targetCrownBaseHeightM = target,
                createdYear = ecology?.EcologicalYear ?? 0
            };
            workOrders.Add(order);
            RecordOrderEvent(order, ScenarioManagementEventType.OrderCreated, ScenarioManagementOutcome.None, CurrentYear);
        }
        feedback = $"Crop Tree pruning: {eligible} eligible, {ineligible} ineligible, "
            + $"{alreadyTreated} already treated, {scheduled} scheduled.";
        return eligible;
    }
    public bool AnnualReviewSeen => annualReviewSeen;
    public IReadOnlyList<ScenarioObjectiveResult> Objectives => ScenarioOneObjectives.Evaluate(definition,
        ecologicalSnapshots.Count > 0 ? ecologicalSnapshots[ecologicalSnapshots.Count - 1] : null,
        managementEvents, OriginalSpeciesId, workOrders);
    public FellingMaterialOutcome PlanningFellingOutcome
    {
        get => planningFellingOutcome;
        set => planningFellingOutcome = value;
    }
    public bool WorkPlanOpen => workPlanOpen;
    public bool ReferencePreviewActive => referencePreviewActive;

    // Reference generation uses the same work and ecology, but must not
    // compare its own Century Review against a previously frozen copy of itself.
    // This transient flag does not enter saves or change any biological work.
    public void SetReferenceAuthoring(bool active)
    {
        referenceAuthoring = active;
    }

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
        if (Application.isPlaying)
        {
            soundscapePlayer = GetComponent<ScenarioOneSoundscapePlayer>()
                ?? gameObject.AddComponent<ScenarioOneSoundscapePlayer>();
            habitatVisuals = GetComponent<ScenarioHabitatVisuals>()
                ?? gameObject.AddComponent<ScenarioHabitatVisuals>();
        }
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
        ClearPlantingMarkers();
        if (plantingMarkerMaterial != null)
            Destroy(plantingMarkerMaterial);
        if (approvedPlantingMaterial != null)
            Destroy(approvedPlantingMaterial);
        if (deadwoodMaterial != null)
            Destroy(deadwoodMaterial);
    }

    private void Start()
    {
        EnsureBaselineSnapshot();
        referenceArchive = ScenarioReferenceArchive.Load();
    }

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
            return;
        if (referencePreviewActive)
        {
            if (keyboard.tabKey.wasPressedThisFrame || keyboard.escapeKey.wasPressedThisFrame)
                EndReferencePreview();
            return;
        }
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
        nextManagementEventId = 1;
        managementEvents.Clear();
        ecologicalSnapshots.Clear();
        understoreyCells.Clear();
        nextDeadwoodId = 1;
        ClearDeadwoodVisuals();
        ClearFellingResidueVisuals();
        deadwoodRecords.Clear();
        ClearPlantingMarkers();
        plantedJuveniles.Clear();
        clearancePatches.Clear();
        habitatVisuals?.Rebuild(ecology, understoreyCells, deadwoodRecords, plantedJuveniles);
        nextJuvenileId = 1;
        retainedTimberM3 = 0f;
        soundscapeState = new ScenarioSoundscapeState();
        if (soundscapePlayer != null)
            soundscapePlayer.Route(soundscapeState);
        outcome = ScenarioOneOutcome.Active;
        outcomeYear = -1;
        outcomeReason = "";
        annualReviewSeen = false;
        centuryReview = null;
        planningFellingOutcome = definition.DefaultFellingOutcome;
        selectedShopItemId = "";
        selectedRemovalSpeciesId = "";
        feedback = "Scenario started. Walk the stand, mark trees, then build the annual Work Plan.";
    }

    public bool TryBeginReferencePreview(int year)
    {
        if (referencePreviewActive)
            return false;
        if (referenceArchive == null)
            referenceArchive = ScenarioReferenceArchive.Load();
        ScenarioReferenceMilestone milestone = referenceArchive?.AtYear(year);
        if (milestone?.world == null || !referenceArchive.Matches(definition, ecology))
        {
            feedback = "A compatible verified reference milestone is not available.";
            return false;
        }
        ForestSaveController saves = UnityEngine.Object.FindFirstObjectByType<ForestSaveController>();
        if (saves == null)
        {
            feedback = "The save controller is unavailable for reference preview.";
            return false;
        }
        ForestSaveData example = JsonUtility.FromJson<ForestSaveData>(JsonUtility.ToJson(milestone.world));
        if (!milestone.verifiedFrozenWorld)
        {
            feedback = "Reference milestone hash mismatch; preview was not opened.";
            return false;
        }
        previewReturnData = saves.CaptureData();
        player = UnityEngine.Object.FindFirstObjectByType<ForestPlayer>();
        if (player != null)
        {
            previewPlayerPosition = player.transform.position;
            previewPlayerRotation = player.transform.rotation;
            previewPlayerPitch = player.LookPitch;
        }
        Camera camera = UnityEngine.Object.FindFirstObjectByType<Camera>();
        previewCameraRotation = camera != null ? camera.transform.localRotation : Quaternion.identity;
        SetWorkPlanOpen(false);
        previewYear = year;
        referencePreviewActive = true;
        if (!saves.LoadData(example, false))
        {
            // The milestone was rejected before anything in the world changed,
            // so the player's own forest is still in place.
            referencePreviewActive = false;
            previewReturnData = null;
            previewYear = 0;
            feedback = "The reference milestone could not be loaded; preview was not opened.";
            return false;
        }
        MoveToReferenceView(example);
        return true;
    }

    public void EndReferencePreview()
    {
        if (!referencePreviewActive)
            return;
        referencePreviewActive = false;
        ForestSaveController saves = UnityEngine.Object.FindFirstObjectByType<ForestSaveController>();
        if (saves != null && previewReturnData != null && !saves.LoadData(previewReturnData, false))
            Debug.LogError("Returning from the reference preview failed: the captured forest was rejected on reload.", this);
        previewReturnData = null;
        player = UnityEngine.Object.FindFirstObjectByType<ForestPlayer>();
        if (player != null)
        {
            player.transform.position = previewPlayerPosition;
            player.RestoreLook(previewPlayerRotation, previewPlayerPitch);
        }
        Camera camera = UnityEngine.Object.FindFirstObjectByType<Camera>();
        if (camera != null)
            camera.transform.localRotation = previewCameraRotation;
        previewYear = 0;
        feedback = "Returned from Reference Future preview to your own forest.";
    }

    // Presentation-only viewpoint for a walkable sample. The starting road
    // viewpoint can intersect new century-old crowns or recruited stems;
    // choose an open position near established broadleaf individuals without
    // changing a single tree, cell or saved ecological value.
    private void MoveToReferenceView(ForestSaveData world)
    {
        if (player == null || ecology == null || world?.trees == null)
            return;
        List<TreeSaveData> living = world.trees.Where(tree => tree != null
            && tree.stage != (int)ForestTreeStage.Stump).ToList();
        List<TreeSaveData> broadleaf = living.Where(tree => tree.speciesId == "beech"
            || tree.speciesId == "sessile-oak").ToList();
        if (broadleaf.Count == 0)
            return;

        float bestScore = float.NegativeInfinity;
        Vector3 bestPosition = player.transform.position;
        TreeSaveData bestTarget = null;
        ForestBuildable[] buildables = UnityEngine.Object.FindObjectsByType<ForestBuildable>(
            FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int x = -17; x <= 17; x += 2)
        for (int z = -17; z <= 17; z += 2)
        {
            var point = new Vector3(x, 0f, z);
            int cell = ecology.GetCellIndex(point);
            if (cell < 0)
                continue;
            float nearestTree = float.MaxValue;
            foreach (TreeSaveData tree in living)
                nearestTree = Mathf.Min(nearestTree,
                    Vector2.Distance(new Vector2(x, z), new Vector2(tree.position.x, tree.position.z)));
            if (nearestTree < 3.2f)
                continue;
            if (buildables.Any(buildable => buildable != null &&
                Vector2.Distance(new Vector2(x, z), new Vector2(buildable.transform.position.x,
                    buildable.transform.position.z)) < 3f))
                continue;
            TreeSaveData closestBroadleaf = null;
            float broadleafDistance = float.MaxValue;
            foreach (TreeSaveData tree in broadleaf)
            {
                float distance = Vector2.Distance(new Vector2(x, z),
                    new Vector2(tree.position.x, tree.position.z));
                if (distance < broadleafDistance)
                {
                    broadleafDistance = distance;
                    closestBroadleaf = tree;
                }
            }
            float score = Mathf.Min(nearestTree, 6f) * 2f
                - Mathf.Abs(broadleafDistance - 10f) * 2f
                + ecology.Cells[cell].Light * 6f;
            if (score <= bestScore)
                continue;
            bestScore = score;
            bestPosition = new Vector3(x, 0.08f, z);
            bestTarget = closestBroadleaf;
        }
        if (bestTarget == null)
            return;
        CharacterController controller = player.GetComponent<CharacterController>();
        if (controller != null)
            controller.enabled = false;
        player.transform.position = bestPosition;
        player.LookToward(bestTarget.position + Vector3.up
            * Mathf.Clamp(bestTarget.heightMeters * 0.3f, 2.5f, 7f));
        if (controller != null)
            controller.enabled = true;
    }

    public int GetStockQuantity(string itemId)
    {
        ScenarioInventoryEntry entry = inventory.Find(item => item != null && item.itemId == itemId);
        return entry != null ? entry.quantity : 0;
    }

    public string[] ReadyPlantingItemIds()
    {
        return definition?.ShopEntries?.Where(offer => offer != null
            && GetStockQuantity(offer.itemId) > GetReservedStockQuantity(offer.itemId))
            .Select(offer => offer.itemId).ToArray() ?? Array.Empty<string>();
    }

    public int GetReservedStockQuantity(string itemId)
    {
        return workOrders.Where(order => order.IsOpen && order.type == ScenarioWorkType.PlantJuvenile
            && order.stockItemId == itemId)
            .Sum(order => order.requiredStockQuantity);
    }

    public long ReservedContractorCashCents => workOrders.Where(order => order.status == ScenarioWorkStatus.Approved)
        .Sum(order => order.estimatedCostCents);

    public bool TryPurchaseStock(string itemId, int quantity)
    {
        if (!CanManage()) return false;
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
        RecordEvent(new ScenarioManagementEvent
        {
            year = CurrentYear,
            eventType = ScenarioManagementEventType.StockPurchased,
            outcome = ScenarioManagementOutcome.Succeeded,
            taskType = ScenarioWorkType.PlantJuvenile,
            speciesId = offer.speciesId,
            stockItemId = offer.itemId,
            quantity = quantity,
            stockCostCents = total,
            cashDeltaCents = -total
        });
        feedback = $"Bought {quantity} {offer.displayName} for {Money(total)}. In stock: {entry.quantity}.";
        return true;
    }

    // The Work Plan designates a stable ecology cell, not a free-floating
    // click position. Biology still decides whether planting succeeds at work time.
    public bool TryDesignatePlanting(string itemId, int cellIndex)
    {
        if (!CanManage()) return false;
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
        var order = new ScenarioOneWorkOrder
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
        };
        workOrders.Add(order);
        RecordOrderEvent(order, ScenarioManagementEventType.OrderCreated, ScenarioManagementOutcome.None, CurrentYear);
        feedback = $"Designated {species.DisplayName} planting in cell {cellIndex}. "
            + "Stock and contractor cash are required for approval.";
        return true;
    }

    public bool TryDesignateRegenerationRemoval(string speciesId, int cellIndex)
    {
        if (!CanManage()) return false;
        if (ecology == null)
            ecology = UnityEngine.Object.FindFirstObjectByType<ForestEcologyController>();
        ForestTreeSpawner spawner = UnityEngine.Object.FindFirstObjectByType<ForestTreeSpawner>();
        TreeSpeciesDefinition species = !string.IsNullOrEmpty(speciesId) && spawner != null
            ? spawner.ResolveSpecies(speciesId) : null;
        if (ecology == null || ecology.Cells == null || definition == null
            || cellIndex < 0 || cellIndex >= ecology.CellCount || species == null)
        {
            feedback = "Choose a registered species and a valid stand cell.";
            return false;
        }
        if (HasOpenRegenerationRemovalOrder(speciesId, cellIndex))
        {
            feedback = "This species already has a regeneration-removal order in that cell.";
            return false;
        }
        ForestRegenerationCohort cohort = ecology.Cells[cellIndex].FindCohort(speciesId);
        if (cohort == null || cohort.Density <= 0f)
        {
            feedback = $"No {species.DisplayName} regeneration is present in that cell.";
            return false;
        }

        int minutes = Mathf.Max(1, Mathf.CeilToInt(definition.RemovalBaseMinutes
            + cohort.Density * definition.RemovalMinutesPerCohortDensity));
        var order = new ScenarioOneWorkOrder
        {
            workOrderId = nextWorkOrderId++,
            type = ScenarioWorkType.RemoveRegeneration,
            status = ScenarioWorkStatus.Pending,
            speciesId = speciesId,
            cellIndex = cellIndex,
            worldPosition = new Vector3(ecology.Cells[cellIndex].Center.x, 0f, ecology.Cells[cellIndex].Center.y),
            estimatedMinutes = minutes,
            estimatedCostCents = DivideRoundUp((long)minutes * definition.ContractorHourlyRateCents, 60L),
            expectedRegenerationDensity = cohort.Density,
            createdYear = ecology.EcologicalYear
        };
        workOrders.Add(order);
        RecordOrderEvent(order, ScenarioManagementEventType.OrderCreated, ScenarioManagementOutcome.None, CurrentYear);
        feedback = $"Designated removal of {species.DisplayName} regeneration in cell {cellIndex}.";
        return true;
    }

    // Designates one evidence-backed clear-stem pruning lift on a living tree.
    // The target height is the next configured lift for the tree's current lift
    // count; Forestry rejects invalid lifts at resolution time.
    public bool TryDesignatePruning(string treeId)
    {
        if (!CanManage()) return false;
        if (definition == null)
            return false;
        Dictionary<string, ForestTree> trees = LivingTreesById();
        if (!trees.TryGetValue(treeId, out ForestTree tree) || tree == null || !tree.CanChop)
        {
            feedback = "Choose a living tree to prune.";
            return false;
        }
        if (HasOpenTreeOrder(treeId, ScenarioWorkType.PruneTree))
        {
            feedback = "This tree already has an open pruning order.";
            return false;
        }
        float targetHeight = definition.NextPruningTargetHeightM(tree.PruningLifts);
        if (targetHeight <= 0f)
        {
            feedback = $"{tree.TreeId} has already received the maximum configured pruning lifts.";
            return false;
        }
        if (targetHeight >= tree.Height * 0.6f)
        {
            feedback = $"{tree.TreeId} is too short for the next lift ({targetHeight:0.0} m target).";
            return false;
        }
        string pruningProblem = tree.CanPrune(targetHeight, CurrentYear + 1);
        if (pruningProblem != null)
        {
            feedback = pruningProblem;
            return false;
        }

        int minutes = Mathf.Max(1, Mathf.CeilToInt(definition.PruningBaseMinutes
            + targetHeight * definition.PruningMinutesPerMetre));
        var order = new ScenarioOneWorkOrder
        {
            workOrderId = nextWorkOrderId++,
            type = ScenarioWorkType.PruneTree,
            status = ScenarioWorkStatus.Pending,
            targetTreeId = tree.TreeId,
            speciesId = tree.Species != null ? tree.Species.SpeciesId : "",
            worldPosition = tree.transform.position,
            cellIndex = ecology != null ? ecology.GetCellIndex(tree.transform.position) : -1,
            estimatedMinutes = minutes,
            estimatedCostCents = DivideRoundUp((long)minutes * definition.ContractorHourlyRateCents, 60L),
            expectedVolumeM3 = 0f,
            targetCrownBaseHeightM = targetHeight,
            createdYear = ecology != null ? ecology.EcologicalYear : 0
        };
        workOrders.Add(order);
        RecordOrderEvent(order, ScenarioManagementEventType.OrderCreated, ScenarioManagementOutcome.None, CurrentYear);
        feedback = $"Designated pruning lift {tree.PruningLifts + 1} on {tree.TreeId} to {targetHeight:0.0} m.";
        return true;
    }

    public int AddMarkedTreesToWorkPlan()
    {
        return AddMarkedTreesToWorkPlan(true);
    }

    private int AddMarkedTreesToWorkPlan(bool report)
    {
        if (!CanManage()) return 0;
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
            ScenarioOneWorkOrder order = CreateFellingOrder(tree);
            workOrders.Add(order);
            RecordOrderEvent(order, ScenarioManagementEventType.OrderCreated, ScenarioManagementOutcome.None, CurrentYear);
            added++;
        }
        if (added > 0)
            marking.ClearAll(); // consumes Fell marks only; Crop Tree designations persist
        if (added > 0)
            feedback = $"Added {added} marked tree{(added == 1 ? "" : "s")} to the Work Plan.";
        else if (report)
            feedback = "No new eligible marked trees were available.";
        return added;
    }

    public bool RemovePendingOrder(int workOrderId)
    {
        ScenarioOneWorkOrder order = workOrders.FirstOrDefault(candidate =>
            candidate.workOrderId == workOrderId && candidate.status == ScenarioWorkStatus.Pending);
        if (order == null)
            return false;
        workOrders.Remove(order);
        RemovePlantingMarker(order.workOrderId);
        RecordOrderEvent(order, ScenarioManagementEventType.OrderCancelled, ScenarioManagementOutcome.Cancelled, CurrentYear);
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
        RemovePlantingMarker(order.workOrderId);
        RecordOrderEvent(order, ScenarioManagementEventType.OrderCancelled, ScenarioManagementOutcome.Cancelled, CurrentYear);
        feedback = "Cancelled " + order.ShortLabel + ". Contractor cash and stock are available again.";
        return true;
    }

    public bool ApprovePendingWork()
    {
        if (!CanManage()) return false;
        AddMarkedTreesToWorkPlan(false);
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
        {
            order.status = ScenarioWorkStatus.Approved;
            RecordOrderEvent(order, ScenarioManagementEventType.OrderApproved, ScenarioManagementOutcome.None, CurrentYear);
        }
        RefreshPlantingMarkers();
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
        if (!CanManage()) return false;

        EnsureBaselineSnapshot();

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
        {
            long beforeCash = cashCents;
            long beforeCost = report.contractorCostCents;
            long beforeRevenue = report.timberRevenueCents;
            float beforeVolume = report.harvestedVolumeM3;
            float beforeDeadwood = report.deadwoodCreatedM3;
            float beforeKept = report.keptForUseVolumeM3;
            int beforeStock = order.type == ScenarioWorkType.PlantJuvenile ? GetStockQuantity(order.stockItemId) : 0;
            float beforeRemovedDensity = report.removedRegenerationDensity;
            ResolveOrder(order, report);
            ScenarioManagementEvent result = RecordOrderEvent(order, ScenarioManagementEventType.WorkResolved,
                order.status == ScenarioWorkStatus.Completed ? ScenarioManagementOutcome.Succeeded : ScenarioManagementOutcome.Failed,
                report.year);
            result.contractorCostCents = report.contractorCostCents - beforeCost;
            result.timberRevenueCents = report.timberRevenueCents - beforeRevenue;
            result.biologicalVolumeM3 = (report.harvestedVolumeM3 - beforeVolume)
                + (report.deadwoodCreatedM3 - beforeDeadwood)
                + (report.keptForUseVolumeM3 - beforeKept);
            result.regenerationDensityRemoved = report.removedRegenerationDensity - beforeRemovedDensity;
            result.stockUsed = order.type == ScenarioWorkType.PlantJuvenile
                ? beforeStock - GetStockQuantity(order.stockItemId) : 0;
            result.cashDeltaCents = cashCents - beforeCash;
            result.failureReason = order.status == ScenarioWorkStatus.Failed ? order.validationMessage : "";
            result.ecologicalTreatment = order.status == ScenarioWorkStatus.Completed
                ? TreatmentFor(order) : ScenarioEcologicalTreatment.None;
        }

        // Scenario One's single authoritative annual sequence: approved work,
        // immediate financial settlement, then exactly one Forestry annual step.
        ecology.AdvanceOneYear();
        AdvancePlantedJuveniles();
        RefreshPlantingMarkers();
        AdvanceUnderstorey();
        report.deadwoodDecayedM3 = AdvanceDeadwood();
        RefreshFellingResidueVisuals();
        report.closingCashCents = cashCents;
        annualReports.Add(report);
        RecordEvent(new ScenarioManagementEvent
        {
            year = report.year,
            eventType = ScenarioManagementEventType.YearAdvanced,
            outcome = ScenarioManagementOutcome.Succeeded
        });
        RecordEcologicalSnapshot();
        EvaluateProgress();
        feedback = $"Year {report.year} complete: {report.completedTasks} task(s), "
            + $"cost {Money(report.contractorCostCents)}, timber {Money(report.timberRevenueCents)}, "
            + $"closing cash {Money(cashCents)}."
            + (outcome != ScenarioOneOutcome.Active ? " " + outcomeReason : "");
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
            nextManagementEventId = nextManagementEventId,
            workOrders = CloneOrders(workOrders),
            inventory = CloneInventory(inventory),
            annualReports = CloneReports(annualReports),
            managementEvents = CloneEvents(managementEvents),
            ecologicalSnapshots = CloneSnapshots(ecologicalSnapshots),
            understoreyCells = CloneUnderstorey(understoreyCells),
            deadwoodRecords = CloneDeadwood(deadwoodRecords),
            nextDeadwoodId = nextDeadwoodId,
            retainedTimberM3 = retainedTimberM3,
            plantedJuveniles = plantedJuveniles.Select(j => new PlantedJuvenileSaveData
            {
                juvenileId = j.juvenileId, speciesId = j.speciesId, position = j.position,
                cellIndex = j.cellIndex, plantingYear = j.plantingYear, ageYears = j.ageYears,
                heightMeters = j.heightMeters, alive = j.alive, stockItemId = j.stockItemId,
                promotedTreeId = j.promotedTreeId, legacyCohortManaged = j.legacyCohortManaged
            }).ToList(),
            clearancePatches = clearancePatches.Select(p => new PlantingClearancePatch
            {
                center = p.center, radiusMeters = p.radiusMeters, createdYear = p.createdYear
            }).ToList(),
            interactionSchemaVersion = 2,
            outcome = outcome,
            outcomeYear = outcomeYear,
            outcomeReason = outcomeReason,
            annualReviewSeen = annualReviewSeen,
            centuryReview = centuryReview == null ? null
                : JsonUtility.FromJson<ScenarioCenturyReview>(JsonUtility.ToJson(centuryReview))
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
        managementEvents = CloneEvents(data.managementEvents);
        ecologicalSnapshots = CloneSnapshots(data.ecologicalSnapshots);
        understoreyCells = CloneUnderstorey(data.understoreyCells);
        ClearDeadwoodVisuals();
        deadwoodRecords = CloneDeadwood(data.deadwoodRecords);
        nextDeadwoodId = Mathf.Max(1, data.nextDeadwoodId);
        if (deadwoodRecords.Count > 0)
            nextDeadwoodId = Mathf.Max(nextDeadwoodId,
                deadwoodRecords.Max(item => int.TryParse(item.deadwoodId != null && item.deadwoodId.StartsWith("DW")
                    ? item.deadwoodId.Substring(2) : "0", out int id) ? id : 0) + 1);
        planningFellingOutcome = definition != null ? definition.DefaultFellingOutcome : FellingMaterialOutcome.SellAndExtract;
        retainedTimberM3 = Mathf.Max(0f, data.retainedTimberM3);
        plantedJuveniles = data.plantedJuveniles?.Select(j => new PlantedJuvenile
        {
            juvenileId = j.juvenileId, speciesId = j.speciesId, position = j.position,
            cellIndex = j.cellIndex, plantingYear = j.plantingYear, ageYears = j.ageYears,
            heightMeters = j.heightMeters, alive = j.alive, stockItemId = j.stockItemId,
                promotedTreeId = j.promotedTreeId,
                legacyCohortManaged = j.legacyCohortManaged || data.interactionSchemaVersion < 2
        }).ToList() ?? new List<PlantedJuvenile>();
        clearancePatches = data.clearancePatches?.Select(p => new PlantingClearancePatch
        {
            center = p.center, radiusMeters = p.radiusMeters, createdYear = p.createdYear
        }).ToList() ?? new List<PlantingClearancePatch>();
        nextJuvenileId = plantedJuveniles.Count > 0
            ? plantedJuveniles.Max(j => int.TryParse(j.juvenileId?.StartsWith("PJ") == true ? j.juvenileId.Substring(2) : "0", out int id) ? id : 0) + 1
            : 1;
        nextManagementEventId = Mathf.Max(1, data.nextManagementEventId);
        if (managementEvents.Count > 0)
            nextManagementEventId = Mathf.Max(nextManagementEventId, managementEvents.Max(item => item.eventId) + 1);
        outcome = data.outcome;
        outcomeYear = outcome == ScenarioOneOutcome.Active ? -1 : data.outcomeYear;
        outcomeReason = data.outcomeReason ?? "";
        annualReviewSeen = data.annualReviewSeen;
        // JsonUtility can deserialize an omitted nullable serializable object
        // as an empty instance (year 0). It is not a completed Century Review.
        centuryReview = data.centuryReview == null || data.centuryReview.year < ReviewYear
            ? null : JsonUtility.FromJson<ScenarioCenturyReview>(JsonUtility.ToJson(data.centuryReview));
        RestoreDeadwoodVisuals();
        RefreshFellingResidueVisuals();
        habitatVisuals?.Rebuild(ecology, understoreyCells, deadwoodRecords, plantedJuveniles);
        soundscapeState = ecologicalSnapshots.Count == 0 ? new ScenarioSoundscapeState()
            : ScenarioSoundscape.Compute(ecologicalSnapshots[ecologicalSnapshots.Count - 1],
                understoreyCells, deadwoodRecords, definition);
        if (soundscapePlayer != null)
            soundscapePlayer.Route(soundscapeState);
        ValidateOpenOrders();
        RefreshPlantingMarkers();
        feedback = "Scenario management state loaded.";
    }

    private ScenarioOneWorkOrder CreateFellingOrder(ForestTree tree)
    {
        float volume = tree.BiologicalStemVolumeM3;
        int minutes = Mathf.Max(1, Mathf.CeilToInt(definition.FellingBaseMinutes
            + volume * definition.FellingMinutesPerCubicMetre));
        long cost = DivideRoundUp((long)minutes * definition.ContractorHourlyRateCents, 60L);
        string speciesId = tree.Species != null ? tree.Species.SpeciesId : "";
        long revenue = planningFellingOutcome == FellingMaterialOutcome.SellAndExtract
            ? (long)Math.Round(volume * definition.TimberValueCentsPerCubicMetre(speciesId),
                MidpointRounding.AwayFromZero) : 0L;
        return new ScenarioOneWorkOrder
        {
            workOrderId = nextWorkOrderId++,
            type = ScenarioWorkType.FellTree,
            status = ScenarioWorkStatus.Pending,
            targetTreeId = tree.TreeId,
            speciesId = speciesId,
            worldPosition = tree.transform.position,
            cellIndex = ecology != null ? ecology.GetCellIndex(tree.transform.position) : -1,
            fellingOutcome = planningFellingOutcome,
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
            case ScenarioWorkType.RemoveRegeneration:
                ResolveRegenerationRemoval(order, report);
                break;
            case ScenarioWorkType.PruneTree:
                ResolvePruning(order, report);
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
        bool retainDeadwood = order.fellingOutcome == FellingMaterialOutcome.RetainAsFallenDeadwood;
        bool keepForUse = order.fellingOutcome == FellingMaterialOutcome.KeepForUse;
        long revenue = order.fellingOutcome == FellingMaterialOutcome.SellAndExtract
            ? (long)Math.Round(volume * definition.TimberValueCentsPerCubicMetre(speciesId), MidpointRounding.AwayFromZero)
            : 0L;
        cashCents -= order.estimatedCostCents;
        cashCents += revenue;

        ScenarioDeadwoodRecord deadwood = null;
        if (retainDeadwood)
        {
            deadwood = new ScenarioDeadwoodRecord
            {
                deadwoodId = "DW" + nextDeadwoodId++.ToString("0000"),
                treeId = tree.TreeId,
                speciesId = speciesId,
                worldPosition = tree.transform.position,
                cellIndex = ecology != null ? ecology.GetCellIndex(tree.transform.position) : -1,
                originalVolumeM3 = volume,
                remainingVolumeM3 = volume,
                originalHeightMeters = tree.Height,
                originalDiameterCm = tree.Diameter,
                fallenYear = ecology != null ? ecology.EcologicalYear + 1 : 0,
                lastDecayYear = ecology != null ? ecology.EcologicalYear + 1 : 0
            };
        }

        tree.Fell();
        if (deadwood != null)
        {
            deadwood.visualName = SpawnFallenLogVisual(deadwood);
            deadwoodRecords.Add(deadwood);
        }
        if (keepForUse)
            retainedTimberM3 += volume;

        order.status = ScenarioWorkStatus.Completed;
        order.resolvedYear = ecology.EcologicalYear + 1;
        SpawnFellingResidueVisual(order);
        order.expectedVolumeM3 = volume;
        order.expectedRevenueCents = revenue;
        report.completedTasks++;
        report.contractorCostCents += order.estimatedCostCents;
        report.timberRevenueCents += revenue;
        if (retainDeadwood)
        {
            report.deadwoodCreated++;
            report.deadwoodCreatedM3 += volume;
        }
        else if (keepForUse)
        {
            report.keptForUseVolumeM3 += volume;
        }
        else
            report.harvestedVolumeM3 += volume;
    }

    // Management-layer presentation only: a simple fallen stem marker at the
    // felling position. Forestry's authoritative stump visual is untouched.
    private string SpawnFallenLogVisual(ScenarioDeadwoodRecord record)
    {
        if (record == null)
            return "";
        SectionFiveVisualCatalog catalog = SectionFiveVisualCatalog.Load();
        if (record.speciesId == "sitka-spruce" && catalog != null && catalog.freshLog != null && catalog.decayedLog != null)
        {
            var authoredLog = new GameObject("Fallen Log " + record.deadwoodId);
            authoredLog.transform.SetParent(transform, false);
            authoredLog.transform.position = record.worldPosition;
            uint heading = 2166136261u;
            foreach (char c in record.deadwoodId) heading = (heading ^ c) * 16777619u;
            authoredLog.transform.rotation = Quaternion.Euler(0f, (heading & 0xFFFF) / 65535f * 360f, 0f);
            authoredLog.AddComponent<ScenarioFallenLogVisual>().Refresh(record, catalog);
            return authoredLog.name;
        }
        var log = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        log.name = "Fallen Log " + record.deadwoodId;
        log.transform.SetParent(transform, false);
        float length = Mathf.Max(1f, record.originalHeightMeters * 0.8f);
        float radius = Mathf.Max(0.05f, record.originalDiameterCm / 200f);
        log.transform.position = new Vector3(record.worldPosition.x, radius, record.worldPosition.z);
        uint hash = 2166136261u;
        foreach (char character in record.deadwoodId)
        {
            hash ^= character;
            hash *= 16777619u;
        }
        log.transform.rotation = Quaternion.Euler(0f, (hash & 0xFFFF) / 65535f * 360f, 90f);
        float scale = Mathf.Pow(Mathf.Clamp01(record.remainingVolumeM3
            / Mathf.Max(0.0001f, record.originalVolumeM3)), 1f / 3f);
        log.transform.localScale = new Vector3(radius * 2f, length * 0.5f, radius * 2f) * scale;
        var collider = log.GetComponent<Collider>();
        if (collider != null)
            Destroy(collider);
        var renderer = log.GetComponent<Renderer>();
        if (renderer != null)
        {
            ForestTreeSpawner spawner = UnityEngine.Object.FindFirstObjectByType<ForestTreeSpawner>();
            if (spawner != null && spawner.BarkMaterial != null)
                renderer.sharedMaterial = spawner.BarkMaterial;
            else
            {
                if (deadwoodMaterial == null)
                {
                    Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
                    if (shader != null)
                        deadwoodMaterial = new Material(shader) { color = new Color(0.28f, 0.19f, 0.11f) };
                }
                if (deadwoodMaterial != null)
                    renderer.sharedMaterial = deadwoodMaterial;
            }
            StyleDeadwood(renderer, record);
        }
        return log.name;
    }

    private static void StyleDeadwood(Renderer renderer, ScenarioDeadwoodRecord record)
    {
        if (renderer == null || record == null) return;
        float decay = Mathf.Clamp01(record.DecayClass / 5f);
        Color color = Color.Lerp(new Color(0.32f, 0.21f, 0.12f),
            new Color(0.20f, 0.31f, 0.19f), decay);
        var block = new MaterialPropertyBlock();
        renderer.GetPropertyBlock(block);
        block.SetColor("_BaseColor", color);
        block.SetColor("_Color", color);
        renderer.SetPropertyBlock(block);
    }

    private void ClearDeadwoodVisuals()
    {
        foreach (Transform child in transform)
            if (child.name.StartsWith("Fallen Log ", StringComparison.Ordinal))
                Destroy(child.gameObject);
    }

    private void RestoreDeadwoodVisuals()
    {
        foreach (ScenarioDeadwoodRecord record in deadwoodRecords)
            if (record != null && record.originalVolumeM3 > 0f)
                record.visualName = SpawnFallenLogVisual(record);
    }

    // Felling leaves brash at the stump whatever happens to the stem. Only
    // completed felling orders spawn it; green/dry reads years since the
    // recorded felling year and encodes no decay rule [D].
    private void RefreshFellingResidueVisuals()
    {
        ClearFellingResidueVisuals();
        foreach (ScenarioOneWorkOrder order in workOrders)
            if (order != null && order.type == ScenarioWorkType.FellTree
                && order.status == ScenarioWorkStatus.Completed)
                SpawnFellingResidueVisual(order);
    }

    private void SpawnFellingResidueVisual(ScenarioOneWorkOrder order)
    {
        if (order.speciesId != "sitka-spruce") return; // authored residue is spruce boughs
        GameObject fresh = (order.workOrderId & 1) == 0
            ? fellingResidueGreenPrefab : fellingResidueGreenAltPrefab;
        GameObject dry = (order.workOrderId & 1) == 0
            ? fellingResidueDryPrefab : fellingResidueDryAltPrefab;
        bool recent = order.resolvedYear < 0
            || CurrentYear - order.resolvedYear < 5; // [D] appearance only
        GameObject prefab = (recent ? fresh : dry) ?? (recent ? fellingResidueGreenPrefab : fellingResidueDryPrefab);
        if (prefab == null)
            return;
        uint hash = 2166136261u;
        foreach (char character in "residue-" + order.workOrderId)
        {
            hash ^= character;
            hash *= 16777619u;
        }
        float angle = (hash & 0xFFFFu) / 65535f * 360f;
        float distance = 0.9f + ((hash >> 16) & 0xFFu) / 255f * 1.1f;
        var site = Instantiate(prefab, transform);
        site.name = "Felling Residue " + order.workOrderId;
        site.transform.position = order.worldPosition
            + new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0f, Mathf.Sin(angle * Mathf.Deg2Rad)) * distance;
        site.transform.rotation = Quaternion.Euler(0f, angle, 0f);
        site.transform.localScale = Vector3.one * (0.85f + ((hash >> 24) & 0x3Fu) / 63f * 0.4f);
        foreach (Collider collider in site.GetComponentsInChildren<Collider>())
            Destroy(collider);
    }

    private void ClearFellingResidueVisuals()
    {
        foreach (Transform child in transform)
            if (child.name.StartsWith("Felling Residue ", StringComparison.Ordinal))
                Destroy(child.gameObject);
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

        if (!order.exactPosition)
        {
            // Historical v12 cell-designated orders retain their original
            // biology; Reference Future v1 is a frozen example of that run.
            PlantingResult result = ecology.TryPlantJuvenile(species, order.worldPosition);
            if (!result.Success)
            {
                Fail(order, report, result.Message);
                return;
            }
        }
        ScenarioInventoryEntry stock = inventory.Find(item => item != null && item.itemId == order.stockItemId);
        stock.quantity -= order.requiredStockQuantity;
        cashCents -= order.estimatedCostCents;

        if (order.exactPosition)
        {
            plantedJuveniles.Add(new PlantedJuvenile
            {
                juvenileId = "PJ" + nextJuvenileId++.ToString("0000"),
                speciesId = order.speciesId,
                position = order.worldPosition,
                cellIndex = order.cellIndex,
                plantingYear = report.year,
                ageYears = ecology.PlantedJuvenileAgeYears,
                heightMeters = ecology.PlantedJuvenileHeightM,
                alive = true,
                stockItemId = order.stockItemId
            });
            ApplyPlantingClearance(order.worldPosition, report.year);
        }

        order.status = ScenarioWorkStatus.Completed;
        order.resolvedYear = report.year;
        report.completedTasks++;
        report.contractorCostCents += order.estimatedCostCents;
    }

    // Compute the current year's new circular area in every touched 5x5m cell.
    // Older patches are a history, not permanent exclusions from seed rain.
    private void ApplyPlantingClearance(Vector3 position, int year)
    {
        var patch = new PlantingClearancePatch
        {
            center = position,
            radiusMeters = Mathf.Sqrt(1f / Mathf.PI),
            createdYear = year
        };
        var thisYear = clearancePatches.Where(p => p.createdYear == year).ToList();
        float halfCell = ecology.CellSizeMeters * 0.5f;
        float cellArea = ecology.CellSizeMeters * ecology.CellSizeMeters;
        foreach (ForestEcologyCell cell in ecology.Cells)
        {
            if (Mathf.Abs(cell.Center.x - position.x) > halfCell + patch.radiusMeters
                || Mathf.Abs(cell.Center.y - position.z) > halfCell + patch.radiusMeters)
                continue;
            ForestRegenerationCohort sitka = cell.FindCohort("sitka-spruce");
            if (sitka == null || sitka.Density <= 0f)
                continue;
            float priorArea = ClearanceUnionArea(cell.Center, halfCell, thisYear);
            thisYear.Add(patch);
            float newArea = ClearanceUnionArea(cell.Center, halfCell, thisYear);
            thisYear.RemoveAt(thisYear.Count - 1);
            // Remaining uncovered area is the appropriate denominator because
            // density was already reduced by previous clearances this year.
            sitka.Density *= Mathf.Clamp01((cellArea - newArea) / Mathf.Max(0.0001f, cellArea - priorArea));
            if (sitka.Density <= 0.001f)
                cell.RemoveCohortIfEmpty(sitka);
        }
        clearancePatches.Add(patch);
    }

    // Deterministic vertical-strip integration of clipped circle union. A
    // 2-cm strip bounds area error well below a regeneration-density unit;
    // intervals are merged before counting, so overlap cannot be double paid.
    public static float ClearanceUnionArea(Vector2 cellCenter, float halfCell, IReadOnlyList<PlantingClearancePatch> patches)
    {
        if (patches == null || patches.Count == 0 || halfCell <= 0f)
            return 0f;
        float left = cellCenter.x - halfCell;
        float bottom = cellCenter.y - halfCell;
        float top = cellCenter.y + halfCell;
        int strips = Mathf.CeilToInt(halfCell * 2f / 0.02f);
        float dx = halfCell * 2f / strips;
        float area = 0f;
        var spans = new List<Vector2>();
        for (int i = 0; i < strips; i++)
        {
            float x = left + (i + 0.5f) * dx;
            spans.Clear();
            foreach (PlantingClearancePatch patch in patches)
            {
                float offset = x - patch.center.x;
                float r = patch.radiusMeters;
                if (Mathf.Abs(offset) >= r)
                    continue;
                float halfHeight = Mathf.Sqrt(r * r - offset * offset);
                float low = Mathf.Max(bottom, patch.center.z - halfHeight);
                float high = Mathf.Min(top, patch.center.z + halfHeight);
                if (high > low)
                    spans.Add(new Vector2(low, high));
            }
            spans.Sort((a, b) => a.x.CompareTo(b.x));
            if (spans.Count == 0)
                continue;
            float lo = spans[0].x, hi = spans[0].y;
            for (int j = 1; j < spans.Count; j++)
            {
                if (spans[j].x <= hi)
                    hi = Mathf.Max(hi, spans[j].y);
                else
                {
                    area += (hi - lo) * dx;
                    lo = spans[j].x;
                    hi = spans[j].y;
                }
            }
            area += (hi - lo) * dx;
        }
        return Mathf.Min(4f * halfCell * halfCell, area);
    }

    private void AdvancePlantedJuveniles()
    {
        if (plantedJuveniles.Count == 0 || ecology?.Cells == null)
            return;
        ForestTreeSpawner spawner = UnityEngine.Object.FindFirstObjectByType<ForestTreeSpawner>();
        if (spawner == null)
            return;
        foreach (PlantedJuvenile juvenile in plantedJuveniles.OrderBy(j => j.juvenileId, StringComparer.Ordinal))
        {
            if (!juvenile.alive || juvenile.legacyCohortManaged || !string.IsNullOrEmpty(juvenile.promotedTreeId))
                continue;
            TreeSpeciesDefinition species = spawner.ResolveSpecies(juvenile.speciesId);
            int index = ecology.GetCellIndex(juvenile.position);
            if (species == null || index < 0)
                continue;
            ForestEcologyCell cell = ecology.Cells[index];
            juvenile.ageYears += 1f;
            juvenile.heightMeters += species.RegenHeightGrowthMPerYear
                * species.JuvenileLightResponse(cell.Light) * cell.SiteProductivity;
            if (SurvivalRoll(juvenile.juvenileId, ecology.EcologicalYear, ecology.SimulationSeed)
                >= species.JuvenileSurvivalResponse(cell.Light))
            {
                juvenile.alive = false;
                continue;
            }
            if (juvenile.heightMeters < species.PromotionHeightM || cell.Light < species.PromotionMinimumLight)
                continue;
            string treeId = "PL-" + juvenile.juvenileId;
            float dbh = Mathf.Clamp(juvenile.heightMeters * 1.5f, 2f, 20f);
            ForestTree tree = spawner.Spawn(treeId, species, juvenile.position,
                Mathf.Max(1, Mathf.RoundToInt(juvenile.ageYears)), dbh, juvenile.heightMeters,
                species.PotentialCrownRadiusM(dbh));
            if (tree != null)
                juvenile.promotedTreeId = treeId;
        }
    }

    private static float SurvivalRoll(string id, int year, int seed)
    {
        uint hash = 2166136261u;
        foreach (char c in id)
            hash = (hash ^ c) * 16777619u;
        hash = (hash ^ (uint)year) * 16777619u;
        hash = (hash ^ (uint)seed) * 16777619u;
        return (hash & 0xFFFFFFu) / 16777216f;
    }

    private void ResolveRegenerationRemoval(ScenarioOneWorkOrder order, ScenarioAnnualReport report)
    {
        ForestTreeSpawner spawner = UnityEngine.Object.FindFirstObjectByType<ForestTreeSpawner>();
        TreeSpeciesDefinition species = spawner != null ? spawner.ResolveSpecies(order.speciesId) : null;
        if (species == null || ecology.GetCellIndex(order.worldPosition) != order.cellIndex)
        {
            Fail(order, report, "Regeneration species or target cell is unavailable.");
            return;
        }
        if (cashCents < order.estimatedCostCents)
        {
            Fail(order, report, "Insufficient cash when the contractor attempted regeneration removal.");
            return;
        }
        ForestRegenerationCohort cohort = ecology.Cells[order.cellIndex].FindCohort(order.speciesId);
        float density = cohort != null ? cohort.Density : 0f;
        UprootingResult result = ecology.TryUprootRegeneration(order.worldPosition, species);
        if (!result.Success)
        {
            Fail(order, report, result.Message);
            return;
        }
        cashCents -= order.estimatedCostCents;
        order.status = ScenarioWorkStatus.Completed;
        order.resolvedYear = report.year;
        order.expectedRegenerationDensity = density;
        report.completedTasks++;
        report.regenerationRemovalTasks++;
        report.removedRegenerationDensity += density;
        report.contractorCostCents += order.estimatedCostCents;
    }

    private void ResolvePruning(ScenarioOneWorkOrder order, ScenarioAnnualReport report)
    {
        Dictionary<string, ForestTree> trees = LivingTreesById();
        if (!trees.TryGetValue(order.targetTreeId, out ForestTree tree) || tree == null || !tree.CanChop)
        {
            Fail(order, report, "Target tree is no longer eligible for pruning.");
            return;
        }
        if (cashCents < order.estimatedCostCents)
        {
            Fail(order, report, "Insufficient cash when the contractor attempted pruning.");
            return;
        }

        float targetHeight = PruningTarget(order);
        string rejection = tree.TryPrune(targetHeight, report.year);
        if (rejection != null)
        {
            Fail(order, report, rejection);
            return;
        }

        cashCents -= order.estimatedCostCents;
        order.status = ScenarioWorkStatus.Completed;
        order.resolvedYear = report.year;
        order.expectedVolumeM3 = 0f;
        report.completedTasks++;
        report.contractorCostCents += order.estimatedCostCents;
    }

    private static void Fail(ScenarioOneWorkOrder order, ScenarioAnnualReport report, string reason)
    {
        order.status = ScenarioWorkStatus.Failed;
        order.validationMessage = reason;
        order.resolvedYear = report.year;
        report.failedTasks++;
    }

    private void ValidateOpenOrders()
    {
        Dictionary<string, ForestTree> trees = LivingTreesById();
        var reservedStock = new Dictionary<string, int>(StringComparer.Ordinal);
        var designatedCells = new HashSet<string>(StringComparer.Ordinal);
        var removalCells = new HashSet<string>(StringComparer.Ordinal);
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
                else if (order.fellingOutcome != FellingMaterialOutcome.SellAndExtract
                    && order.fellingOutcome != FellingMaterialOutcome.RetainAsFallenDeadwood
                    && order.fellingOutcome != FellingMaterialOutcome.KeepForUse)
                    order.validationMessage = "Unknown felling material outcome.";
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
                else if (!order.exactPosition && !designatedCells.Add(order.speciesId + ":" + order.cellIndex))
                    order.validationMessage = "Another order already plants this species in this cell.";
                else
                {
                    ForestRegenerationCohort cohort = ecology.Cells[order.cellIndex].FindCohort(order.speciesId);
                    if (!order.exactPosition && cohort != null && cohort.Density > 0f)
                        order.validationMessage = "This species is already regenerating in the cell.";
                    else if (order.exactPosition && workOrders.Any(other => other != order && other.IsOpen
                        && other.workOrderId < order.workOrderId
                        && Vector2.Distance(new Vector2(order.worldPosition.x, order.worldPosition.z),
                            new Vector2(other.worldPosition.x, other.worldPosition.z)) < 0.5f
                        && other.type == ScenarioWorkType.PlantJuvenile))
                        order.validationMessage = "Another marker occupies this planting position.";
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
            else if (order.type == ScenarioWorkType.RemoveRegeneration)
            {
                ForestTreeSpawner spawner = UnityEngine.Object.FindFirstObjectByType<ForestTreeSpawner>();
                TreeSpeciesDefinition species = !string.IsNullOrEmpty(order.speciesId) && spawner != null
                    ? spawner.ResolveSpecies(order.speciesId) : null;
                if (species == null)
                    order.validationMessage = "Regeneration species is unavailable.";
                else if (ecology == null || order.cellIndex < 0 || order.cellIndex >= ecology.CellCount
                    || ecology.GetCellIndex(order.worldPosition) != order.cellIndex)
                    order.validationMessage = "Regeneration cell is outside the stand.";
                else if (!removalCells.Add(order.speciesId + ":" + order.cellIndex))
                    order.validationMessage = "Another order already removes this species in this cell.";
                else
                {
                    ForestRegenerationCohort cohort = ecology.Cells[order.cellIndex].FindCohort(order.speciesId);
                    if (cohort == null || cohort.Density <= 0f)
                        order.validationMessage = "This species is no longer regenerating in the cell.";
                }
            }
            else if (order.type == ScenarioWorkType.PruneTree)
            {
                if (string.IsNullOrEmpty(order.targetTreeId))
                    order.validationMessage = "Missing target tree ID.";
                else if (!trees.TryGetValue(order.targetTreeId, out ForestTree tree) || tree == null || !tree.CanChop)
                    order.validationMessage = "Target tree is missing or no longer eligible for pruning.";
                else if (HasOpenTreeOrder(order.targetTreeId, ScenarioWorkType.FellTree))
                    order.validationMessage = "Tree is scheduled for felling.";
                else if (order.requiresCropTree && !tree.IsCropTree)
                    order.validationMessage = "Tree is no longer designated a Crop Tree.";
                else if (definition != null && definition.NextPruningTargetHeightM(tree.PruningLifts) < 0f)
                    order.validationMessage = "Tree has already received the maximum pruning lifts.";
                else
                    order.validationMessage = tree.CanPrune(PruningTarget(order), CurrentYear + 1);
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

    private bool HasOpenRegenerationRemovalOrder(string speciesId, int cellIndex)
    {
        return workOrders.Any(order => order.type == ScenarioWorkType.RemoveRegeneration && order.IsOpen
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
        if (open)
            AddMarkedTreesToWorkPlan(false);
        if (player == null)
            player = UnityEngine.Object.FindFirstObjectByType<ForestPlayer>();
        if (player != null)
            player.enabled = !open;
        Cursor.lockState = open ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = open;
    }

    private void OnGUI()
    {
        if (referencePreviewActive)
        {
            DrawReferencePreviewPrompt();
            return;
        }
        if (!workPlanOpen)
        {
            DrawMainHud();
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
        IReadOnlyList<ScenarioObjectiveResult> objectives = Objectives;
        GUILayout.Label($"Scenario: {outcome} · {objectives.Count(item => item.achieved)}/{objectives.Count} "
            + $"objectives · Century Review year {ReviewYear}", headingStyle);
        GUILayout.Label(outcome == ScenarioOneOutcome.Failed ? outcomeReason : TutorialHint, bodyStyle);
        GUILayout.Space(8f);

        scroll = GUILayout.BeginScrollView(scroll, GUILayout.ExpandHeight(true));
        if (GUILayout.Button(annualReviewOpen ? "Hide annual review" : "Show annual review", buttonStyle,
                GUILayout.Height(30f * scale)))
        {
            annualReviewOpen = !annualReviewOpen;
            if (annualReviewOpen)
                annualReviewSeen = true;
        }
        if (annualReviewOpen)
        {
            DrawAnnualReview();
            GUILayout.Space(12f);
        }
        DrawReferencePreviewControls();
        DrawNursery();
        GUILayout.Space(12f);
        DrawSpatialSummary();
        GUILayout.Space(12f);
        DrawWorkOrdersSummary();
        GUILayout.Space(12f);
        DrawCropTreePruning();
        GUILayout.EndScrollView();

        GUILayout.Space(8f);
        if (!string.IsNullOrEmpty(feedback))
            GUILayout.Label(feedback, bodyStyle);
        GUILayout.BeginHorizontal();
        GUI.enabled = outcome != ScenarioOneOutcome.Failed && CurrentYear < ReviewYear;
        if (GUILayout.Button("Add marked trees", buttonStyle, GUILayout.Height(42f * scale)))
            AddMarkedTreesToWorkPlan();
        if (GUILayout.Button("Approve pending work", buttonStyle, GUILayout.Height(42f * scale)))
            ApprovePendingWork();
        GUI.enabled = outcome != ScenarioOneOutcome.Failed && CurrentYear < ReviewYear
            && (totals.approvedCount == 0 || totals.approvedCostCents <= cashCents);
        if (GUILayout.Button("Advance one year", buttonStyle, GUILayout.Height(42f * scale)))
            AdvanceYear();
        GUI.enabled = true;
        if (GUILayout.Button("Close", buttonStyle, GUILayout.Height(42f * scale)))
            SetWorkPlanOpen(false);
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    private void DrawReferencePreviewControls()
    {
        if (referenceArchive == null || !referenceArchive.Matches(definition, ecology))
            return;
        GUILayout.Label("EXPLORE REFERENCE FUTURE v1 (FROZEN v12)", headingStyle);
        GUILayout.Label("Walk the verified historical v12 forest at a milestone, then press [Tab] to return to your own stand. "
            + "Your work and save slot are preserved.", bodyStyle);
        GUILayout.BeginHorizontal();
        foreach (int year in new[] { 20, 50, 100 })
            if (GUILayout.Button("Visit year " + year, buttonStyle, GUILayout.Height(36f * ForestHud.Scale)))
                TryBeginReferencePreview(year);
        GUILayout.EndHorizontal();
        GUILayout.Space(10f);
    }

    private void DrawReferencePreviewPrompt()
    {
        EnsureStyles();
        float scale = ForestHud.Scale;
        float width = Mathf.Min(650f * scale, Screen.width - 32f);
        Rect panel = new Rect((Screen.width - width) * 0.5f, 16f, width, 80f * scale);
        ForestHud.Panel(panel);
        GUI.Label(new Rect(panel.x + 12f, panel.y + 8f, panel.width - 24f, panel.height - 16f),
            $"REFERENCE FUTURE v1 — YEAR {previewYear}  ·  [Tab] Return to your forest", bodyStyle);
    }

    private void DrawSpatialSummary()
    {
        GUILayout.Label("SPATIAL SUMMARY — YOUR FORESTRY DECISIONS", headingStyle);
        int fellingMarks = workOrders.Count(o => o.type == ScenarioWorkType.FellTree && o.IsOpen);
        int plantingMarks = workOrders.Count(o => o.type == ScenarioWorkType.PlantJuvenile && o.IsOpen);
        int pruningMarks = workOrders.Count(o => o.type == ScenarioWorkType.PruneTree && o.IsOpen);
        int cropTrees = LivingTreesById().Values.Count(t => t != null && t.IsCropTree);
        GUILayout.Label($"Felling marks: {fellingMarks}  |  Planting markers: {plantingMarks}  |  "
            + $"Pruning tasks: {pruningMarks}  |  Crop Trees: {cropTrees}", bodyStyle);
        GUILayout.Label($"Retained timber: {retainedTimberM3:0.00} m³  |  Planted juveniles: "
            + $"{plantedJuveniles.Count(j => j.alive && !j.legacyCohortManaged && string.IsNullOrEmpty(j.promotedTreeId))}  |  "
            + $"Clearance patches: {clearancePatches.Count}", bodyStyle);
        if (fellingMarks > 0)
            GUILayout.Label("Felling outcomes: " + string.Join(", ", workOrders
                .Where(o => o.type == ScenarioWorkType.FellTree && o.IsOpen)
                .GroupBy(o => o.fellingOutcome)
                .Select(g => $"{g.Key}: {g.Count()}")), bodyStyle);
    }

    private void DrawCropTreePruning()
    {
        GUILayout.Label("CROP TREE PRUNING", headingStyle);
        if (definition == null)
            return;
        var cropTrees = LivingTreesById().Values.Where(tree => tree.IsCropTree).ToList();
        int eligible = cropTrees.Count(tree => !HasOpenTreeOrder(tree.TreeId, ScenarioWorkType.FellTree)
            && !HasOpenTreeOrder(tree.TreeId, ScenarioWorkType.PruneTree)
            && definition.NextPruningTargetHeightM(tree.PruningLifts) > 0f
            && tree.CanPrune(definition.NextPruningTargetHeightM(tree.PruningLifts), CurrentYear + 1) == null);
        GUILayout.Label($"{cropTrees.Count} designated blue · {eligible} eligible for a pruning lift. "
            + "Biological limits, recovery interval and existing orders still apply.", bodyStyle);
        GUI.enabled = eligible > 0;
        if (GUILayout.Button($"Add {eligible} eligible Crop Tree pruning tasks", buttonStyle,
                GUILayout.Height(36f * ForestHud.Scale)))
            BatchPruneCropTrees();
        GUI.enabled = true;
    }

    private void DrawWorkOrdersSummary()
    {
        GUILayout.Label("WORK ORDERS", headingStyle);
        List<ScenarioOneWorkOrder> visible = workOrders.Where(order => order.IsOpen)
            .OrderBy(order => order.workOrderId).ToList();
        if (visible.Count == 0)
            GUILayout.Label("No work is planned. Mark trees in the forest, place planting markers, or designate pruning.", bodyStyle);
        foreach (ScenarioOneWorkOrder order in visible)
            DrawOrder(order);
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
                + $"stock: {order.requiredStockQuantity} {order.stockItemId} · "
                + $"position ({order.worldPosition.x:0.0}, {order.worldPosition.z:0.0})", bodyStyle);
        else if (order.type == ScenarioWorkType.RemoveRegeneration)
            GUILayout.Label($"{Minutes(order.estimatedMinutes)} · contractor {Money(order.estimatedCostCents)} · "
                + $"whole {order.speciesId} cohort · cell {order.cellIndex} · estimated density {order.expectedRegenerationDensity:0.00}", bodyStyle);
        else if (order.type == ScenarioWorkType.PruneTree)
            GUILayout.Label($"{Minutes(order.estimatedMinutes)} · contractor {Money(order.estimatedCostCents)} · "
                + $"clear-stem lift to {PruningTarget(order):0.0} m · tree {order.targetTreeId}", bodyStyle);
        else
        {
            string outcomeLabel = order.fellingOutcome == FellingMaterialOutcome.RetainAsFallenDeadwood
                ? "Retain as fallen deadwood (no timber revenue)"
                : order.fellingOutcome == FellingMaterialOutcome.KeepForUse
                    ? "Keep for construction (no timber revenue)" : "Sell and extract timber";
            GUILayout.Label($"{Minutes(order.estimatedMinutes)} · contractor {Money(order.estimatedCostCents)} · "
                + $"{order.expectedVolumeM3:0.00} m³ · expected revenue {Money(order.expectedRevenueCents)} · {outcomeLabel}", bodyStyle);
            if (order.status == ScenarioWorkStatus.Pending)
            {
                GUILayout.BeginHorizontal();
                foreach (FellingMaterialOutcome choice in new[] { FellingMaterialOutcome.SellAndExtract,
                    FellingMaterialOutcome.KeepForUse, FellingMaterialOutcome.RetainAsFallenDeadwood })
                {
                    GUI.enabled = order.fellingOutcome != choice;
                    if (GUILayout.Button(choice.ToString(), buttonStyle))
                    {
                        order.fellingOutcome = choice;
                        order.expectedRevenueCents = choice == FellingMaterialOutcome.SellAndExtract
                            ? (long)Math.Round(order.expectedVolumeM3 * definition.TimberValueCentsPerCubicMetre(order.speciesId),
                                MidpointRounding.AwayFromZero) : 0L;
                        feedback = $"Order #{order.workOrderId}: {choice}.";
                    }
                }
                GUI.enabled = true;
                GUILayout.EndHorizontal();
            }
        }
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

    private void DrawRegenerationRemovalGrid()
    {
        GUILayout.Label("REGENERATION CONTROL", headingStyle);
        GUILayout.Label("Select a species, then designate cells containing its live regeneration for annual contractor removal.", bodyStyle);
        ForestTreeSpawner spawner = UnityEngine.Object.FindFirstObjectByType<ForestTreeSpawner>();
        if (ecology == null || ecology.Cells == null || spawner == null)
            return;
        GUILayout.BeginHorizontal();
        foreach (TreeSpeciesDefinition species in spawner.KnownSpecies)
        {
            bool hasCohorts = ecology.Cells.Any(cell =>
            {
                ForestRegenerationCohort cohort = cell.FindCohort(species.SpeciesId);
                return cohort != null && cohort.Density > 0f;
            });
            GUI.enabled = hasCohorts;
            if (GUILayout.Button((selectedRemovalSpeciesId == species.SpeciesId ? "Selected: " : "Choose: ")
                    + species.DisplayName, buttonStyle, GUILayout.Height(30f * ForestHud.Scale)))
                selectedRemovalSpeciesId = species.SpeciesId;
            GUI.enabled = true;
        }
        GUILayout.EndHorizontal();
        if (string.IsNullOrEmpty(selectedRemovalSpeciesId))
        {
            GUILayout.Label("No species selected; available cohorts become selectable as the forest regenerates.", bodyStyle);
            return;
        }
        int axis = ecology.CellsPerAxis;
        for (int z = axis - 1; z >= 0; z--)
        {
            GUILayout.BeginHorizontal();
            for (int x = 0; x < axis; x++)
            {
                int index = z * axis + x;
                ForestRegenerationCohort cohort = ecology.Cells[index].FindCohort(selectedRemovalSpeciesId);
                GUI.enabled = cohort != null && cohort.Density > 0f
                    && !HasOpenRegenerationRemovalOrder(selectedRemovalSpeciesId, index);
                if (GUILayout.Button($"{x + 1},{z + 1}", cellButtonStyle,
                        GUILayout.Width(66f * ForestHud.Scale), GUILayout.Height(30f * ForestHud.Scale)))
                    TryDesignateRegenerationRemoval(selectedRemovalSpeciesId, index);
                GUI.enabled = true;
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.Label("Only live selected-species cohorts can be designated. Each order removes the entire cohort in one cell.", bodyStyle);
    }

    private void DrawPruningSection()
    {
        GUILayout.Label("PRUNING — CLEAR-STEM LIFTS", headingStyle);
        GUILayout.Label("Designate evidence-backed pruning lifts on living trees. Each lift raises the clear stem "
            + "and modestly reduces crown radius; Forestry enforces lift limits and recovery intervals.", bodyStyle);
        if (definition == null)
            return;
        Dictionary<string, ForestTree> trees = LivingTreesById();
        var eligible = trees.Values
            .Where(tree => tree != null && tree.CanChop && definition.NextPruningTargetHeightM(tree.PruningLifts) > 0f
                && definition.NextPruningTargetHeightM(tree.PruningLifts) < tree.Height * 0.6f
                && tree.CanPrune(definition.NextPruningTargetHeightM(tree.PruningLifts), CurrentYear + 1) == null
                && !HasOpenTreeOrder(tree.TreeId, ScenarioWorkType.PruneTree))
            .OrderBy(tree => tree.TreeId, StringComparer.Ordinal)
            .ToList();
        if (eligible.Count == 0)
        {
            GUILayout.Label("No trees currently accept another pruning lift.", bodyStyle);
            return;
        }
        foreach (ForestTree tree in eligible.Take(12))
        {
            float target = definition.NextPruningTargetHeightM(tree.PruningLifts);
            GUILayout.BeginHorizontal(GUI.skin.box);
            GUILayout.Label($"{tree.TreeId} · {tree.Species?.DisplayName ?? "unknown"} · "
                + $"lift {tree.PruningLifts + 1} to {target:0.0} m · height {tree.Height:0.0} m · "
                + $"DBH {tree.Diameter:0.0} cm", bodyStyle, GUILayout.MinWidth(420f * ForestHud.Scale));
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Designate pruning", buttonStyle,
                    GUILayout.Width(180f * ForestHud.Scale), GUILayout.Height(30f * ForestHud.Scale)))
                TryDesignatePruning(tree.TreeId);
            GUILayout.EndHorizontal();
        }
        if (eligible.Count > 12)
            GUILayout.Label($"...and {eligible.Count - 12} more eligible trees.", bodyStyle);
    }

    private void DrawAnnualReview()
    {
        GUILayout.Label("ANNUAL REVIEW — ECOLOGICAL OUTCOMES", headingStyle);
        if (ecologicalSnapshots.Count == 0)
        {
            GUILayout.Label("No ecological snapshot has been recorded yet.", bodyStyle);
            return;
        }
        ScenarioEcologicalSnapshot initial = ecologicalSnapshots[0];
        ScenarioEcologicalSnapshot current = ecologicalSnapshots[ecologicalSnapshots.Count - 1];
        GUILayout.Label($"Year {current.year} (baseline year {initial.year}) · trees {current.livingTrees} "
            + $"({current.livingTrees - initial.livingTrees:+#;-#;0}) · basal area "
            + $"{current.basalAreaM2PerHa:0.0} m²/ha · mean DBH {current.meanDbhCm:0.0} cm", bodyStyle);
        GUILayout.Label($"Mean light {current.meanLight:0.00} · mean canopy {current.meanCanopy:0.00} · "
            + $"regenerating cells {current.occupiedRegenerationCells}/{ecology.CellCount} · "
            + $"mean shared occupancy {current.meanSharedRegenerationOccupancy:0.00} · "
            + $"recent opening {current.totalRecentOpening:0.0}", bodyStyle);
        GUILayout.Label($"Functional-group cover [D] — moss {current.meanMosses:0.00}, ferns {current.meanFerns:0.00}, "
            + $"grasses {current.meanGrasses:0.00}, forbs {current.meanForbs:0.00}, "
            + $"shrubs {current.meanShrubs:0.00}, fungi {current.meanFungi:0.00}", bodyStyle);
        GUILayout.Label($"Fallen deadwood [D] — {current.deadwoodCount} log(s), "
            + $"{current.deadwoodVolumeM3:0.00} m³ remaining, mean decay class {current.meanDeadwoodDecayClass:0.0}, "
            + $"habitat value {current.deadwoodHabitatValue:0.00}", bodyStyle);
        if (soundscapeState?.layers != null)
            GUILayout.Label("Habitat sound cues [D] — " + string.Join(", ", soundscapeState.layers
                .Select(layer => $"{layer.displayName} {layer.volume:0.00}")), bodyStyle);
        foreach (ScenarioSpeciesOutcome species in current.species)
            GUILayout.Label($"{species.speciesId}: {species.livingTrees} trees · {species.basalAreaM2PerHa:0.0} m²/ha · "
                + $"cohorts in {species.regenerationCells} cell(s) (legacy planted: {species.plantedRegenerationCells}) · "
                + $"individual planted juveniles {species.plantedJuveniles} · "
                + $"density {species.regenerationDensity:0.00}", bodyStyle);

        GUILayout.Space(8f);
        GUILayout.Label("SCENARIO OBJECTIVES", headingStyle);
        foreach (ScenarioObjectiveResult objective in Objectives)
            GUILayout.Label($"{(objective.achieved ? "✓" : "○")} {objective.displayName}: "
                + $"{objective.currentValue:0.##} / {objective.targetValue:0.##}", bodyStyle);
        if (centuryReview != null)
        {
            GUILayout.Space(8f);
            GUILayout.Label($"CENTURY REVIEW — YEAR {centuryReview.year}", headingStyle);
            GUILayout.Label("HABITAT INTERPRETATION [D] — "
                + ScenarioHabitatInterpretation.Describe(initial, current,
                    habitatVisuals != null ? habitatVisuals.OldWoodlandSourceConfidence : 0f), bodyStyle);
            GUILayout.Label($"Outcome: {centuryReview.outcome} · completed year "
                + (centuryReview.completedYear >= 0 ? centuryReview.completedYear.ToString() : "not achieved")
                + (centuryReview.referenceId == "aspirational-design-targets"
                    ? ". No compatible reference run loaded; these are provisional design targets [D]."
                     : ". Compared with the frozen v12 Reference Future, not an optimal score or prescription."), bodyStyle);
            foreach (ScenarioObjectiveResult comparison in centuryReview.referenceComparisons)
                GUILayout.Label($"{comparison.displayName}: actual {comparison.currentValue:0.##} · "
                    + $"reference {comparison.targetValue:0.##}", bodyStyle);
            if (centuryReview.managementComparisons != null)
                foreach (ScenarioReferenceManagementComparison comparison in centuryReview.managementComparisons)
                    GUILayout.Label($"{comparison.displayName}: your forest {comparison.playerValue:0.##} · "
                        + $"reference {comparison.referenceValue:0.##}", bodyStyle);
        }

        GUILayout.Space(8f);
        GUILayout.Label("RECENT ANNUAL REPORTS", headingStyle);
        foreach (ScenarioAnnualReport report in annualReports.AsEnumerable().Reverse().Take(6))
            GUILayout.Label($"Year {report.year}: {report.completedTasks} completed, {report.failedTasks} failed · "
                + $"contractor {Money(report.contractorCostCents)} · timber {Money(report.timberRevenueCents)} · "
                 + $"extracted {report.harvestedVolumeM3:0.00} m³ · kept {report.keptForUseVolumeM3:0.00} m³ · deadwood +{report.deadwoodCreatedM3:0.00} m³"
                + (report.deadwoodDecayedM3 > 0f ? $" (−{report.deadwoodDecayedM3:0.00} decayed)" : "") + " · "
                + $"regeneration removed {report.regenerationRemovalTasks} cohort(s) / "
                + $"{report.removedRegenerationDensity:0.00} density · cash {Money(report.closingCashCents)}", bodyStyle);
        if (annualReports.Count == 0)
            GUILayout.Label("Advance a year to record the first annual outcome.", bodyStyle);

        GUILayout.Space(8f);
        GUILayout.Label("RECENT MANAGEMENT EVENTS", headingStyle);
        foreach (ScenarioManagementEvent entry in managementEvents.AsEnumerable().Reverse().Take(8))
        {
            string target = !string.IsNullOrEmpty(entry.targetTreeId)
                ? "tree " + entry.targetTreeId
                : entry.cellIndex >= 0 ? "cell " + entry.cellIndex : entry.stockItemId;
            string action = entry.eventType == ScenarioManagementEventType.YearAdvanced
                ? "Ecological year advanced"
                : entry.eventType == ScenarioManagementEventType.ScenarioCompleted
                    ? "Scenario objectives completed"
                    : entry.eventType == ScenarioManagementEventType.ScenarioFailed
                        ? "Scenario failed"
                        : entry.eventType == ScenarioManagementEventType.CenturyReviewed
                            ? "Century Review recorded"
                : entry.eventType == ScenarioManagementEventType.StockPurchased
                    ? $"Bought {entry.quantity} {entry.stockItemId}"
                    : $"{entry.eventType} · {entry.taskType} {entry.speciesId} {target}";
            string result = entry.eventType == ScenarioManagementEventType.WorkResolved
                ? $" · {entry.outcome} · {entry.ecologicalTreatment}"
                : "";
            GUILayout.Label($"#{entry.eventId} · year {entry.year} · {action}{result} · cash {Money(entry.cashDeltaCents)}"
                + (string.IsNullOrEmpty(entry.failureReason) ? "" : " · " + entry.failureReason), bodyStyle);
        }
    }

    private void DrawMainHud()
    {
        EnsureStyles();
        float scale = ForestHud.Scale;
        closedPromptStyle.fontSize = Mathf.RoundToInt(16f * scale);
        int stockTypes = definition?.ShopEntries?.Count ?? 0;
        float width = Mathf.Min(410f * scale, Screen.width - 32f);
        float height = (132f + Mathf.Max(1, stockTypes) * 26f) * scale;
        Rect panel = new Rect(Screen.width - width - 16f, 16f, width, height);
        ForestHud.Panel(panel);
        float left = panel.x + 14f * scale;
        float contentWidth = panel.width - 28f * scale;

        GUI.Label(new Rect(left, panel.y + 10f * scale, contentWidth, 28f * scale),
            $"SCENARIO ONE  ·  YEAR {CurrentYear}", closedPromptStyle);
        GUI.Label(new Rect(left, panel.y + 39f * scale, contentWidth * 0.4f, 30f * scale),
            "Cash", bodyStyle);
        GUI.Label(new Rect(left, panel.y + 39f * scale, contentWidth, 30f * scale),
            Money(cashCents), moneyStyle);
        GUI.Label(new Rect(left, panel.y + 72f * scale, contentWidth, 24f * scale),
            "Saplings ready to plant", bodyStyle);

        if (stockTypes == 0)
            GUI.Label(new Rect(left, panel.y + 98f * scale, contentWidth, 26f * scale),
                "No saplings offered", bodyStyle);
        else
            for (int i = 0; i < stockTypes; i++)
            {
                ScenarioShopEntry offer = definition.ShopEntries[i];
                if (offer == null)
                    continue;
                int reserved = GetReservedStockQuantity(offer.itemId);
                int ready = Mathf.Max(0, GetStockQuantity(offer.itemId) - reserved);
                float y = panel.y + (98f + i * 26f) * scale;
                GUI.Label(new Rect(left, y, contentWidth * 0.55f, 26f * scale),
                    offer.displayName, bodyStyle);
                GUI.Label(new Rect(left + contentWidth * 0.56f, y, contentWidth * 0.44f, 26f * scale),
                    reserved > 0 ? $"{ready} ready · {reserved} held" : $"{ready} ready", moneyStyle);
            }
        GUI.Label(new Rect(left, panel.y + panel.height - 26f * scale, contentWidth, 22f * scale),
            "[Tab] Work Plan", closedPromptStyle);
    }

    private void EnsureStyles()
    {
        float scale = ForestHud.Scale;
        if (titleStyle == null)
        {
            titleStyle = new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold };
            headingStyle = new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold };
            bodyStyle = new GUIStyle(GUI.skin.label) { wordWrap = true };
            mutedStyle = new GUIStyle(GUI.skin.label) { wordWrap = true };
            moneyStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleRight };
            closedPromptStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.UpperLeft,
                fontStyle = FontStyle.Bold
            };
            buttonStyle = new GUIStyle(GUI.skin.button)
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold
            };
            cellButtonStyle = new GUIStyle(buttonStyle);
            inputStyle = new GUIStyle(GUI.skin.textField);
            buttonFace = ForestHud.Solid(new Color(0.15f, 0.20f, 0.13f, 1f));
            buttonFaceHover = ForestHud.Solid(new Color(0.24f, 0.32f, 0.19f, 1f));
            buttonFacePressed = ForestHud.Solid(new Color(0.10f, 0.15f, 0.09f, 1f));
        }

        // Re-applied on every call: this project enters Play Mode without
        // domain or scene reload, so these style objects survive between play
        // sessions and code changes. Construction-time colours cannot be
        // trusted; white on ForestHud's near-black panel is the readable pair.
        titleStyle.fontSize = Mathf.RoundToInt(26f * scale);
        headingStyle.fontSize = Mathf.RoundToInt(20f * scale);
        bodyStyle.fontSize = Mathf.RoundToInt(17f * scale);
        mutedStyle.fontSize = Mathf.RoundToInt(17f * scale);
        moneyStyle.fontSize = Mathf.RoundToInt(20f * scale);
        closedPromptStyle.fontSize = Mathf.RoundToInt(16f * scale);
        buttonStyle.fontSize = Mathf.RoundToInt(16f * scale);
        cellButtonStyle.fontSize = Mathf.RoundToInt(15f * scale);
        inputStyle.fontSize = Mathf.RoundToInt(17f * scale);

        White(titleStyle);
        White(headingStyle);
        White(bodyStyle);
        White(closedPromptStyle);
        White(buttonStyle);
        White(cellButtonStyle);
        SetTextColor(mutedStyle, new Color(1f, 0.72f, 0.55f));
        SetTextColor(moneyStyle, new Color(0.65f, 1f, 0.68f));

        buttonStyle.normal.background = buttonFace;
        buttonStyle.hover.background = buttonFaceHover;
        buttonStyle.active.background = buttonFacePressed;
        buttonStyle.focused.background = buttonFaceHover;
    }

    private static void White(GUIStyle style)
    {
        SetTextColor(style, Color.white);
    }

    // GUIStyle has eight states; Unity itself tints controls when GUI.enabled
    // is false. There is no separate disabled GUIStyleState.
    private static void SetTextColor(GUIStyle style, Color enabled)
    {
        style.normal.textColor = enabled;
        style.hover.textColor = enabled;
        style.active.textColor = enabled;
        style.focused.textColor = enabled;
        style.onNormal.textColor = enabled;
        style.onHover.textColor = enabled;
        style.onActive.textColor = enabled;
        style.onFocused.textColor = enabled;
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
    private int ReviewYear => definition != null
        ? Mathf.Max(definition.MinimumCompletionYear, definition.CenturyReviewYear) : 100;
    private string OriginalSpeciesId
    {
        get
        {
            ForestTreeSpawner spawner = UnityEngine.Object.FindFirstObjectByType<ForestTreeSpawner>();
            return spawner != null && spawner.DefaultSpecies != null
                ? spawner.DefaultSpecies.SpeciesId : "sitka-spruce";
        }
    }

    public string TutorialHint
    {
        get
        {
            if (!managementEvents.Any(entry => entry.eventType == ScenarioManagementEventType.OrderCreated
                && entry.taskType == ScenarioWorkType.FellTree)
                && !workOrders.Any(order => order.type == ScenarioWorkType.FellTree))
                return "1. Inspect a living tree, mark it with M, then import the mark into the Work Plan.";
            if (definition != null && definition.ShopEntries != null && definition.ShopEntries.Any(offer =>
                offer != null && !managementEvents.Any(entry =>
                    entry.eventType == ScenarioManagementEventType.StockPurchased && entry.stockItemId == offer.itemId)
                && !inventory.Any(item => item.itemId == offer.itemId && item.quantity > 0)
                && !workOrders.Any(order => order.type == ScenarioWorkType.PlantJuvenile
                    && order.stockItemId == offer.itemId)))
                return "2. Buy Beech and Sessile Oak saplings from the nursery.";
            if (!managementEvents.Any(entry => entry.eventType == ScenarioManagementEventType.OrderApproved
                && entry.taskType == ScenarioWorkType.PlantJuvenile)
                && !workOrders.Any(order => order.type == ScenarioWorkType.PlantJuvenile
                    && (order.status == ScenarioWorkStatus.Approved
                        || order.status == ScenarioWorkStatus.Completed)))
                 return "3. Press G in the stand, place saplings on the ground and approve the planting markers.";
            if (annualReports.Count == 0)
                return "4. Advance one ecological year to execute approved work.";
            return annualReviewSeen ? "Tutorial complete. Continue management toward the Century Review."
                : "5. Open the annual review to inspect ecological outcomes and management history.";
        }
    }

    private bool CanManage()
    {
        if (outcome != ScenarioOneOutcome.Failed && CurrentYear < ReviewYear)
            return true;
        feedback = outcome == ScenarioOneOutcome.Failed
            ? "The scenario has failed. Review the outcome in the Work Plan."
            : $"Year {ReviewYear} reached. Open the Century Review in the Work Plan.";
        return false;
    }

    private void EvaluateProgress()
    {
        if (ecologicalSnapshots.Count == 0 || definition == null)
            return;
        ScenarioEcologicalSnapshot snapshot = ecologicalSnapshots[ecologicalSnapshots.Count - 1];
        if (outcome == ScenarioOneOutcome.Active)
        {
            List<ScenarioObjectiveResult> objectives = ScenarioOneObjectives.Evaluate(definition,
                snapshot, managementEvents, OriginalSpeciesId, workOrders);
            if (objectives.Count > 0 && objectives.All(result => result.achieved))
                SetOutcome(ScenarioOneOutcome.Completed, snapshot.year,
                    $"Scenario complete in year {snapshot.year}. Continue to the Century Review.");
            else if (cashCents == 0L && definition.ContractorHourlyRateCents > 0
                && !workOrders.Any(order => order.status == ScenarioWorkStatus.Approved))
                SetOutcome(ScenarioOneOutcome.Failed, snapshot.year,
                    "No cash remains to hire a contractor; the management plan cannot continue.");
            else if (snapshot.year >= ReviewYear)
                SetOutcome(ScenarioOneOutcome.Failed, snapshot.year,
                    $"Year {ReviewYear} ended before the continuous-cover objectives were reached.");
        }
        if (snapshot.year >= ReviewYear && centuryReview == null)
        {
            ScenarioReferenceArchive compatible = !referenceAuthoring && referenceArchive != null
                && referenceArchive.Matches(definition, ecology) ? referenceArchive : null;
            string originalSpeciesId = OriginalSpeciesId;
            int oldSitka = LivingTreesById().Values.Count(tree => tree.TreeId.StartsWith("P", StringComparison.Ordinal)
                && tree.Species != null && tree.Species.SpeciesId == originalSpeciesId);
            centuryReview = ScenarioOneObjectives.Review(definition, snapshot, outcome,
                outcome == ScenarioOneOutcome.Completed ? outcomeYear : -1, originalSpeciesId,
                compatible, managementEvents, annualReports, oldSitka);
            RecordEvent(new ScenarioManagementEvent
            {
                year = snapshot.year,
                eventType = ScenarioManagementEventType.CenturyReviewed,
                outcome = outcome == ScenarioOneOutcome.Completed
                    ? ScenarioManagementOutcome.Succeeded : ScenarioManagementOutcome.Failed
            });
        }
    }

    private void SetOutcome(ScenarioOneOutcome result, int year, string reason)
    {
        outcome = result;
        outcomeYear = year;
        outcomeReason = reason;
        RecordEvent(new ScenarioManagementEvent
        {
            year = year,
            eventType = result == ScenarioOneOutcome.Completed
                ? ScenarioManagementEventType.ScenarioCompleted : ScenarioManagementEventType.ScenarioFailed,
            outcome = result == ScenarioOneOutcome.Completed
                ? ScenarioManagementOutcome.Succeeded : ScenarioManagementOutcome.Failed,
            failureReason = result == ScenarioOneOutcome.Failed ? reason : ""
        });
    }

    private static float PruningTarget(ScenarioOneWorkOrder order) => order.targetCrownBaseHeightM > 0f
        ? order.targetCrownBaseHeightM : order.expectedRegenerationDensity;

    private static long DivideRoundUp(long value, long divisor) => (value + divisor - 1L) / divisor;
    private static string Money(long cents) => $"€{cents / 100.0:0.00}";
    private static string Minutes(int minutes) => minutes < 60 ? minutes + " min" : $"{minutes / 60f:0.0} h";

    private ScenarioManagementEvent RecordOrderEvent(ScenarioOneWorkOrder order,
        ScenarioManagementEventType eventType, ScenarioManagementOutcome outcome, int year)
    {
        var entry = new ScenarioManagementEvent
        {
            year = year,
            eventType = eventType,
            outcome = outcome,
            workOrderId = order.workOrderId,
            taskType = order.type,
            speciesId = order.speciesId,
            targetTreeId = order.targetTreeId,
            cellIndex = order.cellIndex >= 0 ? order.cellIndex
                : (ecology != null ? ecology.GetCellIndex(order.worldPosition) : -1),
            worldPosition = order.worldPosition,
            stockItemId = order.stockItemId,
            quantity = order.type == ScenarioWorkType.PlantJuvenile ? order.requiredStockQuantity : 1,
            estimatedContractorCostCents = order.estimatedCostCents
        };
        RecordEvent(entry);
        return entry;
    }

    private void RecordEvent(ScenarioManagementEvent entry)
    {
        entry.eventId = nextManagementEventId++;
        entry.closingCashCents = cashCents;
        managementEvents.Add(entry);
    }

    private static ScenarioEcologicalTreatment TreatmentFor(ScenarioOneWorkOrder order)
    {
        switch (order.type)
        {
            case ScenarioWorkType.FellTree:
                return order.fellingOutcome == FellingMaterialOutcome.RetainAsFallenDeadwood
                    ? ScenarioEcologicalTreatment.TreeRetainedAsDeadwood
                    : ScenarioEcologicalTreatment.TreeFelledAndExtracted;
            case ScenarioWorkType.PlantJuvenile:
                return ScenarioEcologicalTreatment.JuvenilePlanted;
            case ScenarioWorkType.RemoveRegeneration:
                return ScenarioEcologicalTreatment.RegenerationRemoved;
            case ScenarioWorkType.PruneTree:
                return ScenarioEcologicalTreatment.TreePruned;
            default:
                return ScenarioEcologicalTreatment.None;
        }
    }

    private void EnsureBaselineSnapshot()
    {
        EnsureUnderstoreyGrid();
        if (ecologicalSnapshots.Count == 0)
            RecordEcologicalSnapshot();
    }

    private void EnsureUnderstoreyGrid()
    {
        if (ecology == null || ecology.Cells == null)
            return;
        bool valid = understoreyCells.Count == ecology.CellCount;
        if (valid)
            for (int i = 0; i < ecology.CellCount; i++)
                if (understoreyCells[i] == null || understoreyCells[i].cellIndex != i)
                {
                    valid = false;
                    break;
                }
        if (valid)
            return;

        understoreyCells.Clear();
        for (int i = 0; i < ecology.CellCount; i++)
            understoreyCells.Add(ScenarioOneUnderstorey.Initially(i, ecology.Cells[i], ecology.EcologicalYear));
    }

    private void AdvanceUnderstorey()
    {
        EnsureUnderstoreyGrid();
        for (int i = 0; i < understoreyCells.Count; i++)
            ScenarioOneUnderstorey.Advance(understoreyCells[i], ecology.Cells[i], ecology.EcologicalYear,
                definition.UnderstoreyColonisationRate, definition.UnderstoreyLossRate);
    }

    // Deterministic coarse-wood decay. Returns total volume lost this year so the
    // annual report can record it. Deadwood never feeds back into Forestry biology.
    private float AdvanceDeadwood()
    {
        float decayed = 0f;
        foreach (ScenarioDeadwoodRecord record in deadwoodRecords)
        {
            decayed += ScenarioDeadwood.Decay(record, ecology.EcologicalYear);
            if (record == null)
                continue;
            Transform visual = transform.Find("Fallen Log " + record.deadwoodId);
            if (visual == null)
                continue;
            ScenarioFallenLogVisual authored = visual.GetComponent<ScenarioFallenLogVisual>();
            if (authored != null)
            {
                authored.Refresh(record, SectionFiveVisualCatalog.Load());
                continue;
            }
            float radius = Mathf.Max(0.05f, record.originalDiameterCm / 200f);
            float length = Mathf.Max(1f, record.originalHeightMeters * 0.8f);
            float scale = Mathf.Pow(Mathf.Clamp01(record.remainingVolumeM3
                / Mathf.Max(0.0001f, record.originalVolumeM3)), 1f / 3f);
            visual.localScale = new Vector3(radius * 2f, length * 0.5f, radius * 2f) * scale;
            StyleDeadwood(visual.GetComponent<Renderer>(), record);
        }
        return decayed;
    }

    private void RecordEcologicalSnapshot()
    {
        if (ecology == null || ecology.Cells == null || ecology.Cells.Length == 0)
            return;
        EnsureUnderstoreyGrid();
        if (ecologicalSnapshots.Count > 0 && ecologicalSnapshots[ecologicalSnapshots.Count - 1].year == ecology.EcologicalYear)
            return;

        var snapshot = new ScenarioEcologicalSnapshot { year = ecology.EcologicalYear, cellCount = ecology.CellCount };
        float squaredDbhSum = 0f;
        var bySpecies = new Dictionary<string, ScenarioSpeciesOutcome>(StringComparer.Ordinal);
        ForestTreeSpawner spawner = UnityEngine.Object.FindFirstObjectByType<ForestTreeSpawner>();
        if (spawner != null)
            foreach (TreeSpeciesDefinition known in spawner.KnownSpecies)
                bySpecies[known.SpeciesId] = new ScenarioSpeciesOutcome { speciesId = known.SpeciesId };

        ScenarioSpeciesOutcome SpeciesOutcome(string speciesId)
        {
            if (!bySpecies.TryGetValue(speciesId, out ScenarioSpeciesOutcome outcome))
            {
                outcome = new ScenarioSpeciesOutcome { speciesId = speciesId };
                bySpecies.Add(speciesId, outcome);
            }
            return outcome;
        }

        float areaHa = Mathf.Max(0.0001f, ecology.StandAreaHectares);
        foreach (ForestTree tree in UnityEngine.Object.FindObjectsByType<ForestTree>(
                     FindObjectsInactive.Exclude, FindObjectsSortMode.None)
                     .Where(item => item != null && !item.IsStump)
                     .OrderBy(item => item.TreeId, StringComparer.Ordinal))
        {
            string id = tree.Species != null ? tree.Species.SpeciesId : "unknown";
            ScenarioSpeciesOutcome species = SpeciesOutcome(id);
            float basalArea = Mathf.PI * Mathf.Pow(tree.Diameter / 200f, 2f) / areaHa;
            species.livingTrees++;
            species.basalAreaM2PerHa += basalArea;
            snapshot.livingTrees++;
            snapshot.basalAreaM2PerHa += basalArea;
            snapshot.meanDbhCm += tree.Diameter;
            squaredDbhSum += tree.Diameter * tree.Diameter;
        }
        if (snapshot.livingTrees > 0)
        {
            snapshot.meanDbhCm /= snapshot.livingTrees;
            float variance = Mathf.Max(0f, squaredDbhSum / snapshot.livingTrees
                - snapshot.meanDbhCm * snapshot.meanDbhCm);
            snapshot.dbhCoefficientOfVariation = Mathf.Sqrt(variance) / Mathf.Max(0.01f, snapshot.meanDbhCm);
        }
        foreach (ForestEcologyCell cell in ecology.Cells)
        {
            snapshot.meanLight += cell.Light;
            snapshot.meanCanopy += cell.Canopy;
            snapshot.meanSharedRegenerationOccupancy += cell.SharedOccupancy;
            snapshot.totalRecentOpening += cell.RecentOpening;
            if (cell.HasRegeneration)
                snapshot.occupiedRegenerationCells++;
            foreach (ForestRegenerationCohort cohort in cell.Regeneration)
            {
                if (cohort == null || cohort.Density <= 0f || string.IsNullOrEmpty(cohort.SpeciesId))
                    continue;
                ScenarioSpeciesOutcome species = SpeciesOutcome(cohort.SpeciesId);
                species.regenerationCells++;
                species.regenerationDensity += cohort.Density;
                if (cohort.Origin == RegenerationOrigin.Planted)
                    species.plantedRegenerationCells++;
            }
        }
        var individualCells = new HashSet<int>();
        foreach (PlantedJuvenile juvenile in plantedJuveniles)
        {
            if (!juvenile.alive || juvenile.legacyCohortManaged || !string.IsNullOrEmpty(juvenile.promotedTreeId))
                continue;
            SpeciesOutcome(juvenile.speciesId).plantedJuveniles++;
            int cellIndex = ecology.GetCellIndex(juvenile.position);
            if (cellIndex >= 0 && individualCells.Add(cellIndex) && !ecology.Cells[cellIndex].HasRegeneration)
                snapshot.occupiedRegenerationCells++;
        }
        snapshot.meanLight /= ecology.CellCount;
        snapshot.meanCanopy /= ecology.CellCount;
        snapshot.meanSharedRegenerationOccupancy /= ecology.CellCount;
        foreach (ScenarioUnderstoreyCell cell in understoreyCells)
        {
            snapshot.meanMosses += cell.mosses;
            snapshot.meanFerns += cell.ferns;
            snapshot.meanGrasses += cell.grasses;
            snapshot.meanForbs += cell.forbs;
            snapshot.meanShrubs += cell.shrubs;
            snapshot.meanFungi += cell.fungi;
        }
        snapshot.meanMosses /= ecology.CellCount;
        snapshot.meanFerns /= ecology.CellCount;
        snapshot.meanGrasses /= ecology.CellCount;
        snapshot.meanForbs /= ecology.CellCount;
        snapshot.meanShrubs /= ecology.CellCount;
        snapshot.meanFungi /= ecology.CellCount;

        float decayClassSum = 0f;
        foreach (ScenarioDeadwoodRecord record in deadwoodRecords)
        {
            if (record == null || record.remainingVolumeM3 <= 0f)
                continue;
            snapshot.deadwoodCount++;
            snapshot.deadwoodVolumeM3 += record.remainingVolumeM3;
            snapshot.deadwoodHabitatValue += ScenarioDeadwood.HabitatValue(record);
            decayClassSum += record.DecayClass;
        }
        if (snapshot.deadwoodCount > 0)
            snapshot.meanDeadwoodDecayClass = decayClassSum / snapshot.deadwoodCount;

        snapshot.species = bySpecies.Values.OrderBy(entry => entry.speciesId, StringComparer.Ordinal).ToList();
        ecologicalSnapshots.Add(snapshot);
        soundscapeState = ScenarioSoundscape.Compute(snapshot, understoreyCells, deadwoodRecords, definition);
        if (soundscapePlayer != null)
            soundscapePlayer.Route(soundscapeState);
        habitatVisuals?.Rebuild(ecology, understoreyCells, deadwoodRecords, plantedJuveniles);
    }

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
            regenerationRemovalTasks = item.regenerationRemovalTasks,
            removedRegenerationDensity = item.removedRegenerationDensity,
            deadwoodCreated = item.deadwoodCreated,
            deadwoodCreatedM3 = item.deadwoodCreatedM3,
            deadwoodDecayedM3 = item.deadwoodDecayedM3,
            keptForUseVolumeM3 = item.keptForUseVolumeM3,
            closingCashCents = item.closingCashCents
        }).ToList();
    }

    private static List<ScenarioManagementEvent> CloneEvents(List<ScenarioManagementEvent> source)
    {
        if (source == null) return new List<ScenarioManagementEvent>();
        return source.Select(item => JsonUtility.FromJson<ScenarioManagementEvent>(JsonUtility.ToJson(item))).ToList();
    }

    private static List<ScenarioEcologicalSnapshot> CloneSnapshots(List<ScenarioEcologicalSnapshot> source)
    {
        if (source == null) return new List<ScenarioEcologicalSnapshot>();
        return source.Select(item => JsonUtility.FromJson<ScenarioEcologicalSnapshot>(JsonUtility.ToJson(item))).ToList();
    }

    private static List<ScenarioUnderstoreyCell> CloneUnderstorey(List<ScenarioUnderstoreyCell> source)
    {
        if (source == null) return new List<ScenarioUnderstoreyCell>();
        return source.Select(item => JsonUtility.FromJson<ScenarioUnderstoreyCell>(JsonUtility.ToJson(item))).ToList();
    }

    private static List<ScenarioDeadwoodRecord> CloneDeadwood(List<ScenarioDeadwoodRecord> source)
    {
        if (source == null) return new List<ScenarioDeadwoodRecord>();
        return source.Select(item => JsonUtility.FromJson<ScenarioDeadwoodRecord>(JsonUtility.ToJson(item))).ToList();
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
