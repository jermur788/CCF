using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using CCF.Forestry.WorkEconomy;

[DisallowMultipleComponent]
public sealed partial class ScenarioOneManager : MonoBehaviour
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
    private List<BrowseShelter> shelters = new List<BrowseShelter>();
    private List<BrowseProtectedArea> protectedAreas = new List<BrowseProtectedArea>();
    private int ownerMinutesUsedThisYear;
    private WorkExecutionMethod planningPlantingMethod;
    private bool planningInstallShelter;
    private ScenarioHarvestJob cachedOpenHarvest, cachedApprovedHarvest;
    private string cachedOpenHarvestKey, cachedApprovedHarvestKey;
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
    public int OwnerMinutesUsedThisYear => ownerMinutesUsedThisYear;
    public int OwnerMinutesPerYear => definition != null ? definition.OwnerMinutesPerYear : 2400;
    public IReadOnlyList<BrowseShelter> Shelters => shelters;

    // Exact-position planting: a pending order reserves one owned sapling;
    // the juvenile itself is created only when approved contractor work resolves.
    public bool TryDesignateExactPlanting(string itemId, Vector3 groundPosition)
        => TryDesignateExactPlanting(itemId, groundPosition, planningPlantingMethod, planningInstallShelter);

    public bool TryDesignateExactPlanting(string itemId, Vector3 groundPosition, WorkExecutionMethod method, bool installShelter)
    {
        if (!Enum.IsDefined(typeof(WorkExecutionMethod), method)) { feedback = "Unknown execution method."; return false; }
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
            executionMethod = method,
            installShelter = installShelter,
            estimatedMinutes = minutes,
            estimatedCostCents = DivideRoundUp((long)minutes * definition.ContractorHourlyRateCents, 60L),
            createdYear = ecology.EcologicalYear
        };
        workOrders.Add(order);
        ShowPlantingMarker(order);
        ValidateOpenOrders();
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
        managementEvents, OriginalSpeciesId, workOrders, ecology.StandGeometryModelVersion);
    public FellingMaterialOutcome PlanningFellingOutcome
    {
        get => planningFellingOutcome;
        set => planningFellingOutcome = value;
    }
    public bool WorkPlanOpen => workPlanOpen;
    public bool ReferencePreviewActive => referencePreviewActive;

    // ----- Read-only presentation access for the Scenario One UI screens -----
    // The UI reads authoritative state and calls the public actions above; it
    // owns no copy of cash, orders, marks, ecology or reports.
    public int CurrentEcologicalYear => CurrentYear;
    public int CenturyReviewYear => ReviewYear;
    public int ReferencePreviewYear => previewYear;
    public bool ReferenceArchiveAvailable => referenceArchive != null && referenceArchive.Matches(definition, ecology);
    public WorkExecutionMethod PlanningPlantingMethod => planningPlantingMethod;
    public bool PlanningInstallShelter => planningInstallShelter;
    public bool CanAdvanceYearNow
    {
        get
        {
            WorkPlanTotals totals = CalculateTotals();
            return outcome != ScenarioOneOutcome.Failed && CurrentYear < ReviewYear
                && (totals.approvedCount == 0 || totals.approvedCostCents <= cashCents);
        }
    }
    public bool CanEditWorkPlan => outcome != ScenarioOneOutcome.Failed && CurrentYear < ReviewYear;

    public struct WorkPlanSummary
    {
        public int OpenCount, ApprovedCount, NonHarvestMinutes;
        public long ExternalCostCents, ExpectedRevenueCents, ApprovedCostCents;
    }

    public WorkPlanSummary GetWorkPlanSummary()
    {
        WorkPlanTotals totals = CalculateTotals();
        return new WorkPlanSummary
        {
            OpenCount = totals.openCount, ApprovedCount = totals.approvedCount, NonHarvestMinutes = totals.minutes,
            ExternalCostCents = totals.costCents, ExpectedRevenueCents = totals.revenueCents,
            ApprovedCostCents = totals.approvedCostCents
        };
    }

    // Owner minutes reserved by open landowner-simulated planting orders.
    public int PlannedOwnerMinutes => workOrders.Where(order => order.IsOpen && order.type == ScenarioWorkType.PlantJuvenile
        && string.IsNullOrEmpty(order.validationMessage) && order.executionMethod == WorkExecutionMethod.LandownerSimulated)
        .Sum(order => order.estimatedMinutes);

    public int CropTreeCount => LivingTreesById().Values.Count(tree => tree != null && tree.IsCropTree);

    public int EligibleCropTreePruningCount
    {
        get
        {
            if (definition == null) return 0;
            return LivingTreesById().Values.Count(tree => tree.IsCropTree
                && !HasOpenTreeOrder(tree.TreeId, ScenarioWorkType.FellTree)
                && !HasOpenTreeOrder(tree.TreeId, ScenarioWorkType.PruneTree)
                && definition.NextPruningTargetHeightM(tree.PruningLifts) > 0f
                && tree.CanPrune(definition.NextPruningTargetHeightM(tree.PruningLifts), CurrentYear + 1) == null);
        }
    }

    public static float PruningTargetHeight(ScenarioOneWorkOrder order) => PruningTarget(order);
    public static string FormatMoney(long cents) => Money(cents);
    public static string FormatMinutes(int minutes) => Minutes(minutes);

    // Pending felling orders can change their material outcome before approval.
    public bool SetPendingFellingOutcome(int workOrderId, FellingMaterialOutcome choice)
    {
        ScenarioOneWorkOrder order = workOrders.Find(o => o != null && o.workOrderId == workOrderId);
        if (!IsHarvestOrder(order) || order.status != ScenarioWorkStatus.Pending
            || (order.type == ScenarioWorkType.SalvageDeadwood && choice == FellingMaterialOutcome.RetainAsFallenDeadwood))
            return false;
        order.fellingOutcome = choice;
        order.expectedRevenueCents = 0;
        InvalidateEconomyQuotes();
        feedback = $"Order #{order.workOrderId}: {choice}.";
        return true;
    }

    // Explicit acknowledgement of actual annual results completes the saved review step.
    public void MarkAnnualReviewSeen()
    {
        if (annualReports.Count > 0) annualReviewSeen = true;
    }

    // Work Plan plus the other full-screen panels (map, annual review). While any
    // is open the player is paused and the cursor is free; the marking manager
    // ignores input. Only the UI changes the auxiliary flag.
    public bool AnyPanelOpen => workPlanOpen || auxiliaryPanelOpen;
    private bool auxiliaryPanelOpen;

    public void OpenWorkPlan(bool open) => SetWorkPlanOpen(open);

    public void SetAuxiliaryPanelOpen(bool open)
    {
        if (auxiliaryPanelOpen == open)
            return;
        auxiliaryPanelOpen = open;
        ApplyPanelInputState();
    }

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
        if (Application.isPlaying)
            ApplyBrowsingConfiguration();
    }

    // Browse pressure is scenario configuration, not saved state (v14). Applied
    // in Start so it follows ForestEcologyController.Awake's stand default.
    private void ApplyBrowsingConfiguration()
    {
        if (ecology != null && definition != null)
            ecology.Browsing.BackgroundPressure = definition.BackgroundBrowsePressure;
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
            if (GetComponent<ScenarioOneUiRoot>() == null)
                gameObject.AddComponent<ScenarioOneUiRoot>();
        }
        if (!initialized && definition != null)
        {
            // A fresh session is a new game: it uses the corrected, versioned
            // random domains. Loads replace this with the model recorded in the
            // save (absent field = legacy model 0), so existing saves and the
            // frozen Reference Future v1 keep their original behaviour.
            // InitializeNewScenario stays model-neutral because v1-9 save loads
            // also call it.
            if (ecology != null)
            {
                ecology.RngModelVersion = NewGameRngModel;
                ecology.RegenerationModelVersion = NewGameRegenerationModel;
                ecology.GrowthModelVersion = NewGameGrowthModel;
                ecology.StormModelVersion = StormModel.None;
                // Geometry is part of a new game's setup too (D-056): Enlarged80 by policy (Legacy40 only for the
                // Editor-only verification override). A load replaces it with the geometry recorded in the save.
                ecology.ApplyStandGeometry(StandGeometryPolicy.NewGameModelForSession);
            }
            InitializeNewScenario();
        }
        if (ecology != null)
            ecology.SetManagementAnnualControl(true);
    }

    // RNG model for newly created Scenario One games (see Awake).
    public const int NewGameRngModel = SimulationRandom.MixedModel;

    // Regeneration representation for new games (age bands + competition trial). Loads
    // restore the saved model; saves before v16 and Reference v1 are model 0.
    public const int NewGameRegenerationModel = RegenerationModel.Competition;

    // Adult growth for newly created games: Irish site Class III height and
    // adult density mortality. Saves before v17 and Reference v1 are model 0.
    public const int NewGameGrowthModel = GrowthModel.SiteClassDensity;

    private void OnEnable()
    {
        ForestTree.MortalityApplied += OnTreeBiologicalDeath;
    }

    private void OnDisable()
    {
        ForestTree.MortalityApplied -= OnTreeBiologicalDeath;
    }

    // Growth model 1: a biological death leaves its stem as fallen deadwood,
    // using the same record, log visual, decay and save path as felled
    // deadwood. ApplyMortality raises this event once per death; restored
    // deaths do not raise it, so each tree yields one record.
    private void OnTreeBiologicalDeath(ForestTree tree)
    {
        if (tree == null || ecology == null || (ecology.GrowthModelVersion < GrowthModel.SiteClassDensity
            && !(ecology.StormModelVersion == StormModel.WindthrowV1 && tree.MortalityCause == "windthrow")))
            return;
        var stormRecordWatch = resolvingStorm ? System.Diagnostics.Stopwatch.StartNew() : null;
        float volume = tree.Diameter * tree.Diameter * 0.00007854f * tree.Height
            * (tree.Species != null ? tree.Species.FormHeightRatio : 0.5f);
        if (volume <= 0f)
            return;
        var deadwood = new ScenarioDeadwoodRecord
        {
            deadwoodId = "DW" + nextDeadwoodId++.ToString("0000"),
            treeId = tree.TreeId,
            speciesId = tree.Species != null ? tree.Species.SpeciesId : "",
            worldPosition = tree.transform.position,
            cellIndex = ecology.GetCellIndex(tree.transform.position),
            originalVolumeM3 = volume,
            remainingVolumeM3 = volume,
            originalHeightMeters = tree.Height,
            originalDiameterCm = tree.Diameter,
            fallenYear = ecology.EcologicalYear,
            lastDecayYear = ecology.EcologicalYear
        };
        deadwoodRecords.Add(deadwood);
        if (resolvingStorm)
        {
            pendingStormDeadwood.Add(deadwood);
            stormDeadwoodMilliseconds += stormRecordWatch.Elapsed.TotalMilliseconds;
        }
        else deadwood.visualName = SpawnFallenLogVisual(deadwood);
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
        ApplyBrowsingConfiguration();
        EnsureBaselineSnapshot();
        referenceArchive = ScenarioReferenceArchive.Load();
    }

    private void Update()
    {
        if (StandaloneSessionMenu.IsOpen) return; // Session controls own input while open.
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
        else if (workPlanOpen && !auxiliaryPanelOpen && keyboard.escapeKey.wasPressedThisFrame)
            SetWorkPlanOpen(false);
    }

    public void InitializeNewScenario()
    {
        InvalidateEconomyQuotes();
        if (definition == null)
            throw new InvalidOperationException("Scenario One requires a definition asset.");
        initialized = true;
        shelters.Clear();
        protectedAreas.Clear();
        ownerMinutesUsedThisYear = 0;
        ecology?.Browsing.ClearProtection();
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
        stormEvents.Clear();
        ClearStormVisualState();
        pendingStormDeadwood.Clear();
        resolvingStorm = false;
        ClearDeadwoodVisuals();
        ClearFellingResidueVisuals();
        deadwoodRecords.Clear();
        ClearPlantingMarkers();
        plantedJuveniles.Clear();
        clearancePatches.Clear();
        InvalidateCompetition();
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
        planningPlantingMethod = WorkExecutionMethod.Contractor;
        planningInstallShelter = false;
        feedback = "Scenario started. Walk the stand, mark trees, then build the annual Work Plan.";
    }

    public bool TryBeginReferencePreview(int year)
    {
        if (referencePreviewActive)
            return false;
        if (referenceArchive == null)
            referenceArchive = ScenarioReferenceArchive.Load();
        ScenarioReferenceMilestone milestone = referenceArchive?.AtYear(year);
        // IdentityMatches, not Matches: preview applies the archive's Legacy40 geometry itself when it loads the
        // archived world, and LoadData(previewReturnData) restores the player's own geometry and exact world on exit.
        if (milestone?.world == null || !referenceArchive.IdentityMatches(definition, ecology))
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

    public long ReservedContractorCashCents => GetHarvestQuote(true).CostCents + workOrders
        .Where(order => order.status == ScenarioWorkStatus.Approved && !IsHarvestOrder(order) && string.IsNullOrEmpty(order.validationMessage))
        .Sum(order => order.estimatedCostCents);

    private void InvalidateEconomyQuotes()
    {
        cachedOpenHarvest = null; cachedApprovedHarvest = null;
        cachedOpenHarvestKey = null; cachedApprovedHarvestKey = null;
    }

    public ScenarioHarvestJob GetHarvestQuote(bool approvedOnly = false)
    {
        var trees = LivingTreesById();
        foreach (var pair in windVictims) trees[pair.Key] = pair.Value;
        var fallen = deadwoodRecords.Where(record => windVictims.ContainsKey(record.treeId)).ToDictionary(record => record.treeId, StringComparer.Ordinal);
        var orders = workOrders.Where(order => order.IsOpen && IsHarvestOrder(order)
            && (!approvedOnly || order.status == ScenarioWorkStatus.Approved) && string.IsNullOrEmpty(order.validationMessage)).OrderBy(order => order.workOrderId).ToList();
        int interventions = managementEvents.Where(entry => entry.eventType == ScenarioManagementEventType.WorkResolved
            && entry.outcome == ScenarioManagementOutcome.Succeeded && entry.taskType == ScenarioWorkType.FellTree).Select(entry => entry.year).Distinct().Count();
        string key = CurrentYear + ":" + cashCents + ":" + definition.MinimumHarvestJobCents + ":" + interventions + ":"
            + string.Join("|", orders.Select(order => order.workOrderId + "/" + order.executionMethod + "/" + order.fellingOutcome + "/" + order.targetTreeId + "/"
                + (trees.TryGetValue(order.targetTreeId ?? "", out var tree) ? tree.Species.SpeciesId + "/" + tree.Height.ToString("R", System.Globalization.CultureInfo.InvariantCulture)
                    + "/" + tree.Diameter.ToString("R", System.Globalization.CultureInfo.InvariantCulture)
                    + "/" + (fallen.TryGetValue(tree.TreeId, out var record) ? record.remainingVolumeM3.ToString("R", System.Globalization.CultureInfo.InvariantCulture) + "/" + record.fallenYear : "living") : "missing")));
        var cached = approvedOnly ? cachedApprovedHarvest : cachedOpenHarvest;
        if (cached != null && key == (approvedOnly ? cachedApprovedHarvestKey : cachedOpenHarvestKey)) return cached;
        var job = ScenarioOneEconomyAdapter.QuoteHarvest(orders, trees, definition, CurrentYear + 1, interventions, cashCents, fallen);
        foreach (var order in job.Orders)
        {
            order.harvestJobId = job.JobId; order.estimatedCostCents = 0;
            order.expectedRevenueCents = job.Resolution != null ? job.Resolution.Quote.Timber.Where(value => value.Batch.SourceTreeId == order.targetTreeId).Sum(value => value.SaleRevenueCents) : 0;
        }
        if (approvedOnly) { cachedApprovedHarvest = job; cachedApprovedHarvestKey = key; }
        else { cachedOpenHarvest = job; cachedOpenHarvestKey = key; }
        return job;
    }

    public ScenarioPlantingQuote GetPlantingQuote(ScenarioOneWorkOrder order, int availableOwnerMinutes = -1)
        => ScenarioOneEconomyAdapter.QuotePlanting(order, definition, GetStockQuantity(order.stockItemId), cashCents,
            availableOwnerMinutes < 0 ? OwnerMinutesPerYear : availableOwnerMinutes);

    public void SetPlantingExecution(WorkExecutionMethod method)
    {
        if (!Enum.IsDefined(typeof(WorkExecutionMethod), method)) { feedback = "Unknown executor."; return; }
        planningPlantingMethod = method;
        foreach (var order in workOrders.Where(order => order.IsOpen && order.type == ScenarioWorkType.PlantJuvenile))
            if (order.executionMethod != method) { order.executionMethod = method; order.status = ScenarioWorkStatus.Pending; }
        ValidateOpenOrders(); feedback = "Planting execution changed; review and approve the plan again.";
    }

    public void SetPlantingShelters(bool install)
    {
        planningInstallShelter = install;
        foreach (var order in workOrders.Where(order => order.IsOpen && order.type == ScenarioWorkType.PlantJuvenile && order.exactPosition))
            if (order.installShelter != install) { order.installShelter = install; order.status = ScenarioWorkStatus.Pending; }
        ValidateOpenOrders(); feedback = "Shelter choice changed; review and approve the plan again.";
    }

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
        if (ecology.Cells[cellIndex].SpeciesDensity(species.SpeciesId) > 0f)
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

    // Historical species orders retain their scope for Reference/archive callers.
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
        // All bands of the species in the cell are the removal scope.
        float speciesDensity = ecology.Cells[cellIndex].SpeciesDensity(speciesId);
        if (speciesDensity <= 0f)
        {
            feedback = $"No {species.DisplayName} regeneration is present in that cell.";
            return false;
        }

        int minutes = Mathf.Max(1, Mathf.CeilToInt(definition.RemovalBaseMinutes
            + speciesDensity * definition.RemovalMinutesPerCohortDensity));
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
            expectedRegenerationDensity = speciesDensity,
            createdYear = ecology.EcologicalYear
        };
        workOrders.Add(order);
        RecordOrderEvent(order, ScenarioManagementEventType.OrderCreated, ScenarioManagementOutcome.None, CurrentYear);
        feedback = $"Designated removal of {species.DisplayName} regeneration in cell {cellIndex}.";
        return true;
    }

    public bool TryDesignateVegetationClearance(int cellIndex)
    {
        if (!CanManage()) return false;
        if (ecology == null || ecology.Cells == null || definition == null
            || cellIndex < 0 || cellIndex >= ecology.CellCount)
        { feedback = "Choose a valid stand cell."; return false; }
        if (workOrders.Any(o => o.IsOpen && o.type == ScenarioWorkType.RemoveRegeneration && o.cellIndex == cellIndex))
        { feedback = "Vegetation clearance is already planned in this cell."; return false; }
        if (workOrders.Any(o => o.type == ScenarioWorkType.RemoveRegeneration && string.IsNullOrEmpty(o.speciesId)
            && o.status == ScenarioWorkStatus.Completed && o.resolvedYear == ecology.EcologicalYear && o.cellIndex == cellIndex))
        { feedback = "This area was already cleared this year."; return false; }
        ClearanceTargets targets = QueryClearance(ClearanceFootprint.Cell(ecology, cellIndex));
        if (!targets.HasTargets) { feedback = "No competing vegetation is present in this cell."; return false; }
        int minutes = Mathf.Max(1, Mathf.CeilToInt(definition.RemovalBaseMinutes
            + targets.Density * definition.RemovalMinutesPerCohortDensity));
        var order = new ScenarioOneWorkOrder
        {
            workOrderId = nextWorkOrderId++, type = ScenarioWorkType.RemoveRegeneration,
            status = ScenarioWorkStatus.Pending, speciesId = "", cellIndex = cellIndex,
            worldPosition = targets.Footprint.Center, estimatedMinutes = minutes,
            estimatedCostCents = DivideRoundUp((long)minutes * definition.ContractorHourlyRateCents, 60L),
            expectedRegenerationDensity = targets.Density, createdYear = ecology.EcologicalYear
        };
        workOrders.Add(order);
        RecordOrderEvent(order, ScenarioManagementEventType.OrderCreated, ScenarioManagementOutcome.None, CurrentYear);
        feedback = "Planned competing vegetation clearance in cell " + cellIndex + ". Review and approve in Work Plan.";
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
        if (added > 0) GetHarvestQuote(false);
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
        var allHarvest = GetHarvestQuote(false);
        long cost = allHarvest.CostCents + workOrders.Where(order => order.IsOpen && !IsHarvestOrder(order)
            && string.IsNullOrEmpty(order.validationMessage)).Sum(order => order.estimatedCostCents);
        if (!allHarvest.Eligible || cost > cashCents)
        {
            feedback = !allHarvest.Eligible ? allHarvest.Problem : $"Approval needs {Money(cost)} including approved work; available cash is {Money(cashCents)}.";
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
        var harvest = GetHarvestQuote(true);
        long requiredCash = harvest.CostCents + approved.Where(order => !IsHarvestOrder(order) && string.IsNullOrEmpty(order.validationMessage))
            .Sum(order => order.estimatedCostCents);
        if (!harvest.Eligible || requiredCash > cashCents)
        {
            feedback = !harvest.Eligible ? harvest.Problem : $"Approved work costs {Money(requiredCash)}; available cash is {Money(cashCents)}.";
            return false;
        }

        var report = new ScenarioAnnualReport { year = ecology.EcologicalYear + 1 };
        ownerMinutesUsedThisYear = 0;
        bool harvestSettled = harvest.Orders.Count == 0;
        // Fellings in this resolution share one canopy and seed-rain rebuild.
        ecology.BeginChangeBatch();
        try
        {
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
                ScenarioPlantingQuote plantingQuote = order.type == ScenarioWorkType.PlantJuvenile && string.IsNullOrEmpty(order.validationMessage)
                    ? GetPlantingQuote(order, OwnerMinutesPerYear - ownerMinutesUsedThisYear) : null;
                if (IsHarvestOrder(order))
                {
                    if (harvest.Orders.Contains(order))
                    {
                        if (order.type == ScenarioWorkType.SalvageDeadwood) ResolveSalvage(order, report);
                        else ResolveFelling(order, report);
                    }
                    else
                    {
                        order.status = ScenarioWorkStatus.Failed; order.resolvedYear = report.year;
                        order.validationMessage = "Cancelled: target missing, dead, already felled or otherwise invalid; no harvest charge.";
                    }
                }
                else ResolveOrder(order, report);
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
                result.executionMethod = order.executionMethod;
                if (order.status == ScenarioWorkStatus.Completed && plantingQuote != null)
                {
                    result.stockCostCents = plantingQuote.MaterialCents;
                    result.ownerMinutes = plantingQuote.OwnerMinutes;
                }
                result.cashDeltaCents = cashCents - beforeCash;
                result.failureReason = order.status == ScenarioWorkStatus.Failed ? order.validationMessage : "";
                result.ecologicalTreatment = order.status == ScenarioWorkStatus.Completed
                    ? TreatmentFor(order) : ScenarioEcologicalTreatment.None;
                if (!harvestSettled && IsHarvestOrder(order) && order.status == ScenarioWorkStatus.Completed
                    && harvest.Orders.All(target => target.status == ScenarioWorkStatus.Completed))
                {
                    SettleHarvestJob(harvest, report, result);
                    harvestSettled = true;
                }
            }
            if (!harvestSettled) throw new InvalidOperationException("Prevalidated atomic harvest world effect failed; financial settlement was not applied.");
        }
        finally
        {
            ecology.EndChangeBatch();
        }

        // Scenario One's single authoritative annual sequence: approved work,
        // immediate financial settlement, then exactly one Forestry annual step.
        ecology.AdvanceOneYear();
        report.deadwoodCreated += ecology.LastStormPerformance.Victims;
        report.deadwoodCreatedM3 += deadwoodRecords.Where(record => record.fallenYear == report.year
            && windVictims.ContainsKey(record.treeId)).Sum(record => record.originalVolumeM3);
        AdvancePlantedJuveniles();
        RefreshPlantingMarkers();
        AdvanceUnderstorey();
        report.deadwoodDecayedM3 = AdvanceDeadwood();
        RefreshFellingResidueVisuals();
        report.closingCashCents = cashCents;
        report.ownerMinutes = ownerMinutesUsedThisYear;
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

    private void SettleHarvestJob(ScenarioHarvestJob harvest, ScenarioAnnualReport report, ScenarioManagementEvent lastFellingEvent)
    {
        var resolution = ScenarioOneEconomyAdapter.ReResolveHarvest(harvest, definition, cashCents);
        if (!resolution.Resolved) throw new InvalidOperationException("Prevalidated harvest can no longer settle.");
        cashCents = checked(cashCents + resolution.ExternalCashFlowCents);
        retainedTimberM3 += harvest.RetainedVolumeCm3 / 1000000f;
        report.contractorCostCents += resolution.Quote.Costs.ContractorWorkCents;
        report.timberRevenueCents += resolution.Quote.TimberSaleRevenueCents;
        report.harvestMinimumAdjustmentCents = resolution.Quote.Costs.MinimumJobAdjustmentCents;
        foreach (var product in resolution.Quote.Timber.GroupBy(value => value.Batch.Assortment).OrderBy(group => group.Key))
            report.timberSales.Add(new ScenarioTimberSale { assortment = product.Key, soldVolumeCm3 = product.Sum(value => value.Batch.Quantity), revenueCents = product.Sum(value => value.SaleRevenueCents) });
        lastFellingEvent.contractorCostCents = resolution.Quote.Costs.ContractorWorkCents;
        lastFellingEvent.timberRevenueCents = resolution.Quote.TimberSaleRevenueCents;
        lastFellingEvent.cashDeltaCents = resolution.ExternalCashFlowCents;
        lastFellingEvent.closingCashCents = cashCents;
    }

    public ScenarioOneSaveData CaptureSaveData()
    {
        // A freshly reset model-2 scenario must be saveable before its first
        // annual step or map/inspection access initializes the vegetation grid.
        if (ecology != null && ecology.RegenerationModelVersion == RegenerationModel.Competition)
            EnsureUnderstoreyGrid();
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
                center = p.center, radiusMeters = p.radiusMeters, createdYear = p.createdYear,
                brambleCover = p.brambleCover, brackenCover = p.brackenCover,
                competitionUpdatedYear = p.competitionUpdatedYear
            }).ToList(),
            interactionSchemaVersion = 2,
            shelters = CloneRecords(ecology != null ? ecology.Browsing.Shelters : shelters),
            protectedAreas = CloneRecords(ecology != null ? ecology.Browsing.ProtectedAreas : protectedAreas),
            ownerMinutesUsedThisYear = ownerMinutesUsedThisYear,
            stormEvents = CloneStormEvents(stormEvents),
            outcome = outcome,
            outcomeYear = outcomeYear,
            outcomeReason = outcomeReason,
            annualReviewSeen = annualReviewSeen,
            centuryReview = centuryReview == null ? null
                : JsonUtility.FromJson<ScenarioCenturyReview>(JsonUtility.ToJson(centuryReview))
        };
    }

    public void RestoreSaveData(ScenarioOneSaveData data)
        => RestoreSaveData(data, ForestSaveData.CurrentVersion);

    public void RestoreSaveData(ScenarioOneSaveData data, int saveVersion)
    {
        InvalidateEconomyQuotes();
        if (ecology == null) ecology = UnityEngine.Object.FindFirstObjectByType<ForestEcologyController>();
        ecology?.Browsing.ClearProtection();
        ScenarioOneSaveMigration.NormalizeLegacy(data, saveVersion);
        shelters = CloneRecords(data?.shelters);
        protectedAreas = CloneRecords(data?.protectedAreas);
        ownerMinutesUsedThisYear = data != null ? data.ownerMinutesUsedThisYear : 0;
        if (ecology != null)
        {
            ecology.Browsing.Shelters.AddRange(shelters);
            ecology.Browsing.ProtectedAreas.AddRange(protectedAreas);
        }
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
        planningPlantingMethod = workOrders.FirstOrDefault(order => order.IsOpen && order.type == ScenarioWorkType.PlantJuvenile)?.executionMethod ?? WorkExecutionMethod.Contractor;
        planningInstallShelter = workOrders.FirstOrDefault(order => order.IsOpen && order.type == ScenarioWorkType.PlantJuvenile && order.exactPosition)?.installShelter ?? false;
        inventory = CloneInventory(data.inventory);
        annualReports = CloneReports(data.annualReports);
        managementEvents = CloneEvents(data.managementEvents);
        ecologicalSnapshots = CloneSnapshots(data.ecologicalSnapshots);
        understoreyCells = CloneUnderstorey(data.understoreyCells);
        ClearDeadwoodVisuals();
        deadwoodRecords = CloneDeadwood(data.deadwoodRecords);
        stormEvents = saveVersion >= 19 ? CloneStormEvents(data.stormEvents) : new List<StormEventRecord>();
        pendingStormDeadwood.Clear();
        resolvingStorm = false;
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
            center = p.center, radiusMeters = p.radiusMeters, createdYear = p.createdYear,
                brambleCover = p.brambleCover, brackenCover = p.brackenCover,
                competitionUpdatedYear = p.competitionUpdatedYear
        }).ToList() ?? new List<PlantingClearancePatch>();
        InvalidateCompetition();
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
        ScenarioOneObjectives.RefreshAspirationalTargets(centuryReview, definition, ecology.StandGeometryModelVersion);
        RestoreStormVisualState();
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
        long cost = 0; // Commissioned-job quote owns cost; never a per-tree minimum/hourly charge.
        string speciesId = tree.Species != null ? tree.Species.SpeciesId : "";
        long revenue = 0; // Set from the grouped yield/market quote.
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
            executionMethod = WorkExecutionMethod.Contractor,
            harvestJobId = CurrentYear + 1,
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
            case ScenarioWorkType.SalvageDeadwood:
                ResolveSalvage(order, report);
                break;
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
        float volume = tree.BiologicalStemVolumeM3;
        string speciesId = tree.Species != null ? tree.Species.SpeciesId : order.speciesId;
        bool retainDeadwood = order.fellingOutcome == FellingMaterialOutcome.RetainAsFallenDeadwood;
        bool keepForUse = order.fellingOutcome == FellingMaterialOutcome.KeepForUse;
        // World effect only. All felling finance is settled once by AdvanceYear's commissioned job.

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
        order.status = ScenarioWorkStatus.Completed;
        order.resolvedYear = ecology.EcologicalYear + 1;
        order.expectedVolumeM3 = volume;
        SpawnFellingResidueVisual(order);
        report.completedTasks++;
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
        if (TrySpawnWindthrowVisual(record, out string windName)) return windName;
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
            if (IsHarvestOrder(order)
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
        string visualName = "Felling Residue " + order.workOrderId;
        Transform existing = transform.Find(visualName);
        if (existing != null && existing.gameObject.activeSelf) return;
        var site = Instantiate(prefab, transform);
        site.name = visualName;
        foreach (Collider collider in site.GetComponentsInChildren<Collider>())
            Destroy(collider);

        // [D] A compact visual patch, not a residue mass or decay model. The
        // authored brash piles are a whole crown's boughs, about 6.7 x 5.1 m
        // and 1.4 m high, so they are sized here from their own bounds to a
        // 1.9-2.4 m patch: still readable as recent work, but no longer
        // reaching across neighbouring planting positions. The resolved stem
        // volume only nudges the size, so live felling and save/load agree.
        Bounds authored = FellingResidueLocalBounds(site.transform);
        float footprintM = Mathf.Clamp(1.8f + 0.6f * Mathf.Pow(Mathf.Max(0f, order.expectedVolumeM3), 1f / 3f),
            1.9f, 2.4f) * (0.92f + ((hash >> 24) & 0xFFu) / 255f * 0.16f);
        float scale = footprintM / Mathf.Max(0.01f, Mathf.Max(authored.size.x, authored.size.z));
        // Flattening pushes the overlapping boughs toward the ground plane so
        // the patch reads as a low mat rather than a stack of separate pieces.
        site.transform.localScale = new Vector3(scale, scale * 0.8f, scale);
        site.transform.rotation = Quaternion.Euler(0f, angle, 0f);

        // The patch lies beside the stump with its near edge at the stump,
        // and its lowest point is set slightly into the ground so no bough hovers.
        float distance = footprintM * 0.4f + 0.15f;
        float direction = (angle + 40f + ((hash >> 16) & 0xFFu) / 255f * 100f) * Mathf.Deg2Rad;
        Vector3 centre = order.worldPosition + new Vector3(Mathf.Cos(direction), 0f, Mathf.Sin(direction)) * distance;
        Vector3 pivotOffset = site.transform.TransformVector(new Vector3(authored.center.x, authored.min.y, authored.center.z));
        site.transform.position = centre - pivotOffset - Vector3.up * FellingResidueGroundSinkM;
    }

    private const float FellingResidueGroundSinkM = 0.01f;

    // Renderer bounds in the residue root's own space, so sizing does not
    // depend on the yaw it is later given.
    private static Bounds FellingResidueLocalBounds(Transform root)
    {
        Bounds bounds = new Bounds();
        bool first = true;
        foreach (Renderer renderer in root.GetComponentsInChildren<Renderer>(true))
        {
            Matrix4x4 toRoot = root.worldToLocalMatrix * renderer.transform.localToWorldMatrix;
            Bounds local = renderer.localBounds;
            for (int corner = 0; corner < 8; corner++)
            {
                Vector3 point = toRoot.MultiplyPoint3x4(local.center + Vector3.Scale(local.extents,
                    new Vector3((corner & 1) == 0 ? -1 : 1, (corner & 2) == 0 ? -1 : 1, (corner & 4) == 0 ? -1 : 1)));
                if (first) { bounds = new Bounds(point, Vector3.zero); first = false; }
                else bounds.Encapsulate(point);
            }
        }
        return bounds;
    }

    private void ClearFellingResidueVisuals()
    {
        // Destroy is deferred to the end of the frame; hide and rename first so
        // a same-frame refresh cannot show the old and new patch together.
        foreach (Transform child in transform)
            if (child.name.StartsWith("Felling Residue ", StringComparison.Ordinal))
            {
                child.gameObject.SetActive(false);
                child.name = "Retired Felling Residue";
                Destroy(child.gameObject);
            }
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
        var work = GetPlantingQuote(order, OwnerMinutesPerYear - ownerMinutesUsedThisYear);
        if (GetStockQuantity(order.stockItemId) < order.requiredStockQuantity || !work.Eligible)
        {
            Fail(order, report, string.IsNullOrEmpty(work.Problem) ? "Insufficient planting stock." : work.Problem);
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

        if (order.exactPosition)
        {
            ApplyPlantingClearance(order.worldPosition, report.year);
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
            if (order.installShelter)
            {
                var juvenile = plantedJuveniles[plantedJuveniles.Count - 1];
                var shelter = new BrowseShelter { shelterId = "S-J" + juvenile.juvenileId,
                    position = new Vector2(juvenile.position.x, juvenile.position.z), installedYear = report.year, effectiveYears = 8, failedYear = -1 };
                shelters.Add(shelter); ecology.Browsing.Shelters.Add(shelter);
            }
        }

        cashCents = checked(cashCents + work.LedgerCashDelta);
        ownerMinutesUsedThisYear = checked(ownerMinutesUsedThisYear + work.OwnerMinutes);

        order.status = ScenarioWorkStatus.Completed;
        order.resolvedYear = report.year;
        report.completedTasks++;
        report.contractorCostCents += work.WorkCents;
    }

    // Compute the current year's new circular area in every touched 5x5m cell.
    // Older patches are a history, not permanent exclusions from seed rain.
    private void ApplyPlantingClearance(Vector3 position, int year)
    {
        ClearanceTargets targets = QueryClearance(ClearanceFootprint.Planting(position), year);
        if (targets.AlreadyTreated) return;
        ApplyClearance(targets, year);
        clearancePatches.Add(new PlantingClearancePatch
        { center = position, radiusMeters = targets.Footprint.Radius, createdYear = year,
            competitionUpdatedYear = year, brambleCover = 0f, brackenCover = 0f });
        InvalidateCompetition();
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
        PlantedCompetitionAccount = new PlantedCompetitionAnnualAccount { Year = ecology != null ? ecology.EcologicalYear : 0 };
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
            PlantedCompetitionSpeciesAccount population = PlantedCompetitionAccount.For(juvenile.speciesId);
            population.Starting++;
            juvenile.ageYears += 1f;
            if (ecology.RegenerationModelVersion == RegenerationModel.Competition)
            {
                float probability = UnderstoreyCompetition.LossProbability(CompetitionExposureAt(juvenile.position),
                    juvenile.heightMeters, CompetitionCalibration);
                if (probability > 0f && SimulationRandom.Roll(ecology.RngModelVersion,
                    juvenile.juvenileId + "/vegetation", ecology.EcologicalYear, ecology.SimulationSeed) < probability)
                {
                    juvenile.alive = false;
                    population.VegetationDeaths++;
                    continue;
                }
            }
            // Browsing v1: same shared response as cohorts, realised as one
            // deterministic annual event per individual (height before growth).
            BrowseAssessment browse = ecology.AssessIndividualBrowse(juvenile.position, species, juvenile.heightMeters);
            bool browsed = JuvenileEcologyRules.RealiseBrowse(browse.Probability, ecology.RngModelVersion,
                juvenile.juvenileId, ecology.EcologicalYear, ecology.SimulationSeed);
            juvenile.lastBrowseAssessment = browse;
            juvenile.lastBrowseAssessmentYear = ecology.EcologicalYear;
            juvenile.lastYearBrowsed = browsed;
            JuvenileEcologyRules.GrowHeight(ref juvenile.heightMeters, species, cell.Light, cell.SiteProductivity,
                browsed ? 1f : 0f);
            if (!JuvenileEcologyRules.Survives(species, cell.Light,
                SimulationRandom.Roll(ecology.RngModelVersion, juvenile.juvenileId, ecology.EcologicalYear, ecology.SimulationSeed),
                browsed))
            {
                juvenile.alive = false;
                float roll = SimulationRandom.Roll(ecology.RngModelVersion, juvenile.juvenileId,
                    ecology.EcologicalYear, ecology.SimulationSeed);
                if (roll >= JuvenileEcologyRules.SurvivalResponse(species, cell.Light)) population.LightDeaths++;
                else population.BrowseDeaths++;
                continue;
            }
            population.Remaining++;
            if (!JuvenileEcologyRules.CanPromote(species, juvenile.heightMeters, cell.Light))
                continue;
            string treeId = "PL-" + juvenile.juvenileId;
            float dbh = JuvenileEcologyRules.PromotionDbhCm(juvenile.heightMeters);
            ForestTree tree = spawner.Spawn(treeId, species, juvenile.position,
                Mathf.Max(1, Mathf.RoundToInt(juvenile.ageYears)), dbh, juvenile.heightMeters,
                species.PotentialCrownRadiusM(dbh));
            if (tree != null)
            {
                juvenile.promotedTreeId = treeId;
                population.Remaining--;
                population.Promoted++;
            }
        }
    }

    private void ResolveRegenerationRemoval(ScenarioOneWorkOrder order, ScenarioAnnualReport report)
    {
        if (string.IsNullOrEmpty(order.speciesId))
        {
            if (ecology == null || order.cellIndex < 0 || order.cellIndex >= ecology.CellCount
                || ecology.GetCellIndex(order.worldPosition) != order.cellIndex)
            { Fail(order, report, "Clearance cell is unavailable."); return; }
            ClearanceTargets targets = QueryClearance(ClearanceFootprint.Cell(ecology, order.cellIndex));
            if (!targets.HasTargets) { Fail(order, report, "No competing vegetation remains."); return; }
            if (cashCents < order.estimatedCostCents)
            { Fail(order, report, "Insufficient cash for vegetation clearance."); return; }
            ApplyClearance(targets, report.year);
            cashCents -= order.estimatedCostCents;
            order.status = ScenarioWorkStatus.Completed; order.resolvedYear = report.year;
            order.expectedRegenerationDensity = targets.Density;
            report.completedTasks++; report.regenerationRemovalTasks++;
            report.removedRegenerationDensity += targets.Density;
            report.contractorCostCents += order.estimatedCostCents;
            return;
        }
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
        float density = ecology.Cells[order.cellIndex].SpeciesDensity(order.speciesId);
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
        int reservedOwnerMinutes = 0;
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
                else if (order.executionMethod != WorkExecutionMethod.Contractor)
                    order.validationMessage = "Scenario One harvest is contractor-only; owner production felling is ineligible.";
                order.estimatedCostCents = 0;
            }
            else if (order.type == ScenarioWorkType.SalvageDeadwood)
            {
                if (!CanSalvage(order.targetTreeId)) order.validationMessage = "Fallen stem is missing, already salvaged or too decayed.";
                else if (order.fellingOutcome != FellingMaterialOutcome.SellAndExtract && order.fellingOutcome != FellingMaterialOutcome.KeepForUse)
                    order.validationMessage = "Salvage can sell/extract or keep for use; leave unselected stems as deadwood.";
                else if (order.executionMethod != WorkExecutionMethod.Contractor) order.validationMessage = "Scenario One salvage is contractor-only.";
                order.estimatedCostCents = 0;
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
                    if (!order.exactPosition && ecology.Cells[order.cellIndex].SpeciesDensity(order.speciesId) > 0f)
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
                if (string.IsNullOrEmpty(order.speciesId))
                {
                    if (ecology == null || order.cellIndex < 0 || order.cellIndex >= ecology.CellCount
                        || ecology.GetCellIndex(order.worldPosition) != order.cellIndex)
                        order.validationMessage = "Clearance cell is outside the stand.";
                    else if (!removalCells.Add("area:" + order.cellIndex))
                        order.validationMessage = "Another order already clears this cell.";
                    else if (!QueryClearance(ClearanceFootprint.Cell(ecology, order.cellIndex)).HasTargets)
                        order.validationMessage = "No competing vegetation remains in this cell.";
                    continue;
                }
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
                    if (ecology.Cells[order.cellIndex].SpeciesDensity(order.speciesId) <= 0f)
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
            if (order.type == ScenarioWorkType.PlantJuvenile && string.IsNullOrEmpty(order.validationMessage))
            {
                if (!Enum.IsDefined(typeof(WorkExecutionMethod), order.executionMethod)) order.validationMessage = "Unknown execution method.";
                else
                {
                    var quote = GetPlantingQuote(order, OwnerMinutesPerYear - reservedOwnerMinutes);
                    order.estimatedCostCents = quote.CostCents; order.estimatedMinutes = quote.PersonMinutes;
                    if (!quote.Eligible) order.validationMessage = quote.Problem;
                    else reservedOwnerMinutes += quote.OwnerMinutes;
                }
            }
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
            if (tree != null && tree.IsLiving && !string.IsNullOrEmpty(tree.TreeId))
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
        ApplyPanelInputState();
    }

    private void ApplyPanelInputState()
    {
        bool open = AnyPanelOpen;
        if (player == null)
            player = UnityEngine.Object.FindFirstObjectByType<ForestPlayer>();
        if (player != null)
            player.enabled = !open;
        Cursor.lockState = open ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = open;
    }

    private WorkPlanTotals CalculateTotals()
    {
        var result = new WorkPlanTotals();
        foreach (ScenarioOneWorkOrder order in workOrders)
        {
            if (!order.IsOpen)
                continue;
            result.openCount++;
            if (!string.IsNullOrEmpty(order.validationMessage)) continue;
            if (!IsHarvestOrder(order)) result.minutes += order.estimatedMinutes;
            if (!IsHarvestOrder(order)) result.costCents += order.estimatedCostCents;
            if (order.status == ScenarioWorkStatus.Approved)
            {
                result.approvedCount++;
                if (!IsHarvestOrder(order)) result.approvedCostCents += order.estimatedCostCents;
            }
        }
        var harvest = GetHarvestQuote(false); result.costCents += harvest.CostCents; result.revenueCents = harvest.RevenueCents;
        result.approvedCostCents += GetHarvestQuote(true).CostCents;
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
            if (annualReports.Count > 0 && !annualReviewSeen)
                return "Read WORK DONE, MONEY and FOREST in Annual Review, then acknowledge the results to continue.";
            ScenarioOneUiRoot teaching = GetComponent<ScenarioOneUiRoot>();
            if (teaching != null && teaching.Learning != null)
                return teaching.Learning.Summary + " · Learn at your own pace across years.";
            if (!managementEvents.Any(entry => entry.eventType == ScenarioManagementEventType.OrderCreated
                && entry.taskType == ScenarioWorkType.FellTree)
                && !workOrders.Any(order => order.type == ScenarioWorkType.FellTree))
                return "1. Inspect a living tree, mark it with X, then import the mark into the Work Plan.";
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
                snapshot, managementEvents, OriginalSpeciesId, workOrders, ecology.StandGeometryModelVersion);
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
                compatible, managementEvents, annualReports, oldSitka, ecology.StandGeometryModelVersion);
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
            case ScenarioWorkType.SalvageDeadwood:
                return ScenarioEcologicalTreatment.WindthrowSalvaged;
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
        {
            var state = ScenarioOneUnderstorey.Initially(i, ecology.Cells[i], ecology.EcologicalYear);
            if (ecology.RegenerationModelVersion == RegenerationModel.Competition)
                UnderstoreyCompetition.Initialize(state, ecology.Cells[i], CompetitionCalibration);
            understoreyCells.Add(state);
        }
        InvalidateCompetition();
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
            // Zero stem volume records preserve salvaged root-plate history.
            // They must not reappear through the ordinary decay floor.
            if (record != null && record.remainingVolumeM3 <= 0f && windVictims.ContainsKey(record.treeId))
                record.lastDecayYear = ecology.EcologicalYear;
            else decayed += ScenarioDeadwood.Decay(record, ecology.EcologicalYear);
            if (record == null)
                continue;
            Transform visual = transform.Find("Fallen Log " + record.deadwoodId);
            if (visual == null)
                continue;
            ScenarioWindthrowVisual wind = visual.GetComponent<ScenarioWindthrowVisual>();
            if (wind != null) { wind.Refresh(record); continue; }
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
            // Cells are counted once per species (and once per planted species)
            // even when a species holds several age bands.
            var countedSpecies = new HashSet<string>();
            var countedPlanted = new HashSet<string>();
            foreach (ForestRegenerationCohort cohort in cell.Regeneration)
            {
                if (cohort == null || cohort.Density <= 0f || string.IsNullOrEmpty(cohort.SpeciesId))
                    continue;
                ScenarioSpeciesOutcome species = SpeciesOutcome(cohort.SpeciesId);
                if (countedSpecies.Add(cohort.SpeciesId))
                    species.regenerationCells++;
                species.regenerationDensity += cohort.Density;
                if (cohort.Origin == RegenerationOrigin.Planted && countedPlanted.Add(cohort.SpeciesId))
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
            closingCashCents = item.closingCashCents,
            harvestMinimumAdjustmentCents = item.harvestMinimumAdjustmentCents,
            ownerMinutes = item.ownerMinutes,
            timberSales = CloneRecords(item.timberSales)
        }).ToList();
    }

    private static List<T> CloneRecords<T>(List<T> source)
    {
        if (source == null) return new List<T>();
        return source.Where(item => item != null).Select(item => JsonUtility.FromJson<T>(JsonUtility.ToJson(item))).ToList();
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
