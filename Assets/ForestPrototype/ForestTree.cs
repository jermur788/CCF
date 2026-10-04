using UnityEngine;

// Legacy stage values kept for save compatibility. Harvesting now leaves a stump
// indefinitely; future regeneration will create new individuals with new ids.
public enum ForestTreeStage
{
    Mature,
    Stump,
    Sapling,
    Young
}

// [D] Descriptive DBH bands, not age, reproductive or timber-quality classes.
public enum ForestTreeSizeClass { Small, Medium, Large }

// Persistent management mark on a living tree. Mutually exclusive: a tree is
// either marked for felling, designated a Crop Tree, or unmarked.
public enum TreeMarkType
{
    None = 0,
    Fell = 1,
    CropTree = 2
}

[DisallowMultipleComponent]
public sealed class ForestTree : MonoBehaviour
{
    public static event System.Action<ForestTree> Felled;
    // Biological death is not management execution. Causes are stable string
    // identifiers so later processes can add causes without changing enum IDs.
    public static event System.Action<ForestTree> MortalityApplied;
    [SerializeField] private bool biologicallyDead;
    [SerializeField] private string mortalityCause = "";
    [SerializeField] private int mortalityYear = -1;
    [SerializeField] private string treeId = "";
    [SerializeField] private Transform trunk;
    [SerializeField] private Transform canopy;
    [SerializeField] private TreeSpeciesDefinition species;
    [SerializeField, Min(0)] private int ageYears = 45;
    [SerializeField, Min(0.1f)] private float heightMeters = 5f;
    [SerializeField, Min(1f)] private float diameterCm = 65f;
    [SerializeField, Min(0.1f)] private float crownRadiusMeters = 1.65f;
    [SerializeField, Min(1)] private int chopsRequired = 4;
    // Optional authored visuals. When set, the
    // placeholder trunk/canopy stay as invisible interaction proxies and the
    // polished meshes visualise the authoritative data instead.
    [SerializeField] private GameObject visualPrefab;
    [SerializeField] private GameObject stumpPrefab;
    [Tooltip("[D] Visual-only pole-stage model for living trees shorter than this height (the mature model takes over above it). The pole asset covers 3.5-12 m; the seedling cohort indicator covers the growth before promotion.")]
    [SerializeField, Min(3f)] private float poleVisualMaxHeightM = 12f;
    [SerializeField] private GameObject poleVisualPrefab;
    [SerializeField] private bool visualOverrideActive;
    // Which prefab the baked PolishedVisual child was built from. Recorded so
    // a prefab swap (visual variety) replaces the old baked mesh instead of
    // reusing it. Stump visuals never vary, so they need no marker.
    [SerializeField] private string polishedVisualSourcePrefab = "";
    // Explicit authored pruning states chosen by the tree's own pruning data.
    [SerializeField] private ForestTreeVisualBase maturePruningBase;
    [SerializeField] private ForestTreeVisualBase polePruningBase;
    private ForestEcologyController ecologyForVisualYear;
    private RecentAssetVisualCatalog recentVisualCatalog;
    private bool useRecentStandVariants;
    private bool usePlantationVisuals;
    private PlantationVisualCatalog plantationCatalog;

    private GameObject polishedVisual;
    private GameObject stumpVisual;
    private float polishedNaturalHeight = -1f;
    private float stumpNaturalHeight = -1f;

    private ForestTreeStage stage = ForestTreeStage.Mature;
    private int chopProgress;
    private float stageTimer;
    [SerializeField] private TreeMarkType markType = TreeMarkType.None;

    public string TreeId => treeId;
    public TreeSpeciesDefinition Species => species;
    public int AgeYears => ageYears;
    public string ActiveVisualSource => polishedVisualSourcePrefab;
    public bool IsBiologicallyDead => biologicallyDead;
    public bool IsLiving => !IsStump && !biologicallyDead;
    public string MortalityCause => mortalityCause;
    public int MortalityYear => mortalityYear;
    public TreeMarkType MarkType => markType;
    public bool IsCropTree => markType == TreeMarkType.CropTree;
    public bool IsMarkedForFell => markType == TreeMarkType.Fell;

    // Mutually exclusive mark: setting one type clears the other automatically
    // because the enum holds exactly one value at a time.
    public void SetMark(TreeMarkType type)
    {
        markType = IsLiving ? type : TreeMarkType.None;
        RefreshVisuals();
    }

    public void RestoreMark(TreeMarkType type)
    {
        markType = IsLiving ? type : TreeMarkType.None;
        RefreshVisuals();
    }
    [SerializeField, Min(0f)] private float equivalentSuppressedYears;
    // Recorded individual-tree history only; no inferred pre-spawn history.
    public float EquivalentSuppressedYears => equivalentSuppressedYears;
    public ForestTreeSizeClass SizeClass => Diameter < 10f ? ForestTreeSizeClass.Small
        : Diameter < 30f ? ForestTreeSizeClass.Medium : ForestTreeSizeClass.Large;
    public string SizeClassLabel => SizeClass == ForestTreeSizeClass.Small ? "Small (<10 cm DBH)"
        : SizeClass == ForestTreeSizeClass.Medium ? "Medium (10–<30 cm DBH)" : "Large (≥30 cm DBH)";

    public void RecordSuppressionYear(float suppression)
    {
        if (IsLiving && !float.IsNaN(suppression) && !float.IsInfinity(suppression))
            equivalentSuppressedYears += Mathf.Clamp01(suppression);
    }

    public void RestoreSuppressionHistory(float years)
    {
        equivalentSuppressedYears = float.IsNaN(years) || float.IsInfinity(years) ? 0f : Mathf.Max(0f, years);
    }

    // Restores pruning history recorded by the management layer. Version 11+
    // saves carry the lift count, crown base and interval state; earlier saves
    // load as unpruned.
    public void RestorePruningHistory(int lifts, float crownBase, int lastYear)
    {
        pruningLifts = Mathf.Max(0, lifts);
        crownBaseHeightM = Mathf.Max(0f, crownBase);
        lastPruningYear = lastYear;
    }

    public void SetSpecies(TreeSpeciesDefinition speciesDefinition)
    {
        species = speciesDefinition;
    }
    public ForestTreeStage Stage => stage;
    // Display stage derives from authoritative height for living trees so what
    // the player sees matches what the card says (pole-looking trees are not
    // called mature); the stored stage stays authoritative for felling,
    // chopping and saves. Thresholds mirror the visual model chain:
    // seedling cohort indicator below promotion (3.5 m [D]), pole asset below
    // poleVisualMaxHeightM (12 m [D]), mature model above.
    public ForestTreeStage DisplayStage
    {
        get
        {
            if (stage == ForestTreeStage.Stump || stage == ForestTreeStage.Sapling)
                return stage;
            if (Height < 3.5f)
                return ForestTreeStage.Sapling;
            if (Height < poleVisualMaxHeightM)
                return ForestTreeStage.Young;
            return ForestTreeStage.Mature;
        }
    }
    public bool IsStump => stage == ForestTreeStage.Stump;
    public bool CanChop => IsLiving && (stage == ForestTreeStage.Mature || stage == ForestTreeStage.Young);
    public int ChopsRequired => chopsRequired;
    public int ChopProgress => chopProgress;
    public float StageTimer => stageTimer;
    // Authoritative simulation values; placeholder meshes only visualise them.
    public float Height => CurrentHeight;
    public float Diameter => diameterCm;
    public float CrownRadius => crownRadiusMeters;
    public float SimulationHeightMeters => heightMeters;
    public float SimulationDiameterCm => diameterCm;
    public float SimulationCrownRadiusMeters => crownRadiusMeters;
    public int WoodYield => Mathf.Clamp(Mathf.RoundToInt(Height), 3, 10);
    public Vector3 InteractionPoint => transform.position;

    // Evidence-backed pruning state. Crown-base height records the highest
    // pruning lift; crown radius shrinks deterministically per lift so the
    // biological light-interception change is represented without adding a
    // second crown model. Lifts follow common Sitka clear-stem practice [D].
    [SerializeField, Min(0)] private int pruningLifts;
    [SerializeField, Min(0f)] private float crownBaseHeightM;
    [SerializeField] private int lastPruningYear = -1;
    public int PruningLifts => pruningLifts;
    public float CrownBaseHeightM => crownBaseHeightM;
    public int LastPruningYear => lastPruningYear;

    // [D] Pruning model calibration shared by all species: each lift removes a
    // fixed fraction of the current crown radius, with a hard cap of three lifts
    // and a minimum recovery interval between lifts.
    private const int MaxPruningLifts = 3;
    private const float CrownRadiusReductionPerLift = 0.08f;
    private const int MinimumYearsBetweenLifts = 5;
    public float PrunedCrownTargetFactor => pruningLifts == 0
        ? 1f : Mathf.Pow(1f - CrownRadiusReductionPerLift, pruningLifts);

    // Applies one clear-stem pruning lift. targetCrownBaseHeightM must exceed
    // the current crown base and stay below 60% of tree height [D]. year is the
    // ecological year used for the recovery-interval check. Returns null on
    // success or a player-readable rejection reason.
    public string CanPrune(float targetCrownBaseHeightM, int year)
    {
        if (!CanChop)
            return "Only living trees can be pruned.";
        if (pruningLifts >= MaxPruningLifts)
            return $"Already pruned {pruningLifts} times (maximum {MaxPruningLifts}).";
        if (lastPruningYear >= 0 && year - lastPruningYear < MinimumYearsBetweenLifts)
            return $"Wait {MinimumYearsBetweenLifts - (year - lastPruningYear)} more year(s) before the next lift.";
        float treeHeight = Height;
        if (targetCrownBaseHeightM <= crownBaseHeightM)
            return "Target crown base must exceed the current pruned height.";
        if (float.IsNaN(targetCrownBaseHeightM) || float.IsInfinity(targetCrownBaseHeightM)
            || targetCrownBaseHeightM <= 0f || targetCrownBaseHeightM >= treeHeight * 0.6f)
            return "Target crown base must be positive and below 60% of tree height.";

        return null;
    }

    public string TryPrune(float targetCrownBaseHeightM, int year)
    {
        string rejection = CanPrune(targetCrownBaseHeightM, year);
        if (rejection != null)
            return rejection;
        crownBaseHeightM = targetCrownBaseHeightM;
        crownRadiusMeters = Mathf.Max(0.1f, crownRadiusMeters * (1f - CrownRadiusReductionPerLift));
        pruningLifts++;
        lastPruningYear = year;
        RefreshVisuals();
        return null;
    }

    // Biological timber interface: stem volume from DBH and form height.
    // Volume_m3 = DBH_cm^2 * 0.00007854 * formHeight_m  (0.00007854 = pi/4 * 1e-4).
    // Additive read-only output; gameplay wood yield stays separate.
    public float BiologicalStemVolumeM3
    {
        get
        {
            if (species == null || !IsLiving)
                return 0f;
            return Diameter * Diameter * 0.00007854f * (Height * species.FormHeightRatio);
        }
    }

    public string StageLabel
    {
        get
        {
            if (biologicallyDead)
                return $"Biologically dead ({mortalityCause}, year {mortalityYear})";
            if (stage == ForestTreeStage.Stump)
                return "Harvested stump";
            switch (DisplayStage)
            {
                case ForestTreeStage.Sapling: return "Sapling";
                case ForestTreeStage.Young: return "Young growing stock";
                default: return "Mature canopy tree";
            }
        }
    }

    private float CurrentHeight
    {
        get
        {
            switch (stage)
            {
                case ForestTreeStage.Stump: return 0.35f;
                case ForestTreeStage.Sapling: return 1.1f;
                case ForestTreeStage.Young: return Mathf.Min(2.7f, heightMeters);
                default: return heightMeters;
            }
        }
    }

    private float MatureThickness => diameterCm / 100f;

    private Vector3 MatureCanopyScale => new Vector3(crownRadiusMeters * 2f, crownRadiusMeters * 2.2f, crownRadiusMeters * 2f);

    private void Awake()
    {
        if (trunk == null)
            trunk = transform.Find("Trunk");
        if (canopy == null)
            canopy = transform.Find("Canopy");

        if (trunk == null)
            Debug.LogError("ForestTree requires a trunk child.", this);

        if (biologicallyDead)
        {
            gameObject.SetActive(false);
            return;
        }

        if (stage == ForestTreeStage.Mature)
            ApplyMatureShape();
    }

    [ContextMenu("Apply visuals from data")]
    private void ApplyMatureShape()
    {
        SetTrunkShape(heightMeters, MatureThickness);
        SetCanopyActive(true, MatureCanopyScale, heightMeters);
        ApplyVisualOverride(heightMeters);
    }

    public void SetVisualPrefabs(GameObject visual, GameObject stump)
    {
        SetVisualStages(visual, null, stump);
    }

    // visual = mature model; pole = pole-stage model (optional, picked by
    // height); stump = felled prop. Height-based model choice uses the pole
    // asset inside its designed range and the mature asset above it.
    public void SetVisualStages(GameObject visual, GameObject pole, GameObject stump)
    {
        visualPrefab = visual;
        poleVisualPrefab = pole;
        stumpPrefab = stump;
        visualOverrideActive = visual != null || maturePruningBase != null || polePruningBase != null;
        RefreshVisuals();
    }

    public void SetPruningBases(ForestTreeVisualBase pole, ForestTreeVisualBase mature)
    {
        polePruningBase = pole;
        maturePruningBase = mature;
    }

    public void SetRecentStandVariants(bool enabled)
    {
        useRecentStandVariants = enabled;
    }

    public void SetPlantationVisuals(bool enabled) { usePlantationVisuals = enabled; }

    private GameObject ModelForHeight(float targetHeight)
    {
        // [D] Young/first-thinning render range. Later trees keep the mature
        // benchmark/approved variants; these thresholds do not change biology.
        if (usePlantationVisuals && targetHeight < 20f)
        {
            if (plantationCatalog == null) plantationCatalog = PlantationVisualCatalog.Load();
            if (plantationCatalog != null)
                return targetHeight < poleVisualMaxHeightM ? plantationCatalog.polePrefab : plantationCatalog.firstThinningPrefab;
        }
        // Cosmetic stem variety only. Crop Tree designation and any recorded
        // pruning use the fully authored pruning family; delivered defect
        // references have no pruning states and never alter eligibility.
        if (useRecentStandVariants && !IsStump && !IsCropTree && pruningLifts == 0)
        {
            if (recentVisualCatalog == null) recentVisualCatalog = RecentAssetVisualCatalog.Load();
            uint hash = 2166136261u;
            foreach (char c in treeId) hash = (hash ^ c) * 16777619u;
            int family = (int)(hash % 10u);
            GameObject[] variants = family == 0 ? recentVisualCatalog?.bentStages
                : family == 1 ? recentVisualCatalog?.cavityStages : null;
            // [D] Renderer-stage ranges only, measured against existing size
            // state. They introduce no defect-development or biological age.
            int visualStage = targetHeight < poleVisualMaxHeightM ? 0
                : targetHeight >= 24f && Diameter >= 30f ? 2 : 1;
            if (variants != null && variants.Length > visualStage && variants[visualStage] != null)
                return variants[visualStage];
        }
        bool poleRange = targetHeight < poleVisualMaxHeightM;
        ForestTreeVisualBase pruningBase = poleRange ? polePruningBase : maturePruningBase;
        if (pruningBase != null)
        {
            GameObject state = pruningBase.PrefabFor(pruningLifts, ScarsLookHealed());
            if (state != null)
                return state;
        }
        // Legacy fallback only when no pruning family is assigned at all.
        if (pruningBase == null)
        {
            if (poleRange && poleVisualPrefab != null)
                return poleVisualPrefab;
            return visualPrefab;
        }
        return null;
    }

    // [D] visual-only scar age: pruning cuts read as recent until five years
    // after the recorded lift, then as healed management history.
    private bool ScarsLookHealed()
    {
        if (lastPruningYear < 0)
            return true;
        if (ecologyForVisualYear == null)
            ecologyForVisualYear = Object.FindFirstObjectByType<ForestEcologyController>();
        int year = ecologyForVisualYear != null ? ecologyForVisualYear.EcologicalYear : lastPruningYear;
        return year - lastPruningYear >= 5;
    }

    private void ApplyVisualOverride(float targetHeight)
    {
        if (visualPrefab == null && maturePruningBase == null && polePruningBase == null)
            return;

        GameObject model = ModelForHeight(targetHeight);
        if (model == null)
            return;

        Renderer trunkRenderer = trunk != null ? trunk.GetComponent<Renderer>() : null;
        if (trunkRenderer != null)
            trunkRenderer.enabled = false;
        if (canopy != null)
            canopy.gameObject.SetActive(false);

        Transform existing = transform.Find("PolishedVisual");
        if (existing != null && polishedVisualSourcePrefab != model.name)
        {
            // The baked visual was built from a different model (prefab swap or
            // a tree growing across the pole/mature boundary); replace it so the
            // authoritative prefab actually shows. Destroy is deferred in play
            // mode, so the cached field must be dropped here or a same-frame
            // re-entry would skip the rebuild against the doomed instance.
            existing.gameObject.SetActive(false);
            existing.name = "RetiredVisual";
            if (Application.isPlaying)
                Destroy(existing.gameObject);
            else
                DestroyImmediate(existing.gameObject);
            existing = null;
            polishedVisual = null;
            polishedNaturalHeight = -1f;
        }
        if (polishedVisual == null)
        {
            // The polished visual may already exist (saved with the scene from
            // an editor pass); reuse it instead of duplicating children.
            polishedVisual = existing != null ? existing.gameObject : Instantiate(model, transform);
            polishedVisual.name = "PolishedVisual";
            polishedVisualSourcePrefab = model.name;
            polishedNaturalHeight = -1f;
        }
        DestroyDuplicateChildren("PolishedVisual", polishedVisual.transform);
        StripInteractionColliders(polishedVisual.transform);
        PlantationTreeVisual plantation = polishedVisual.GetComponent<PlantationTreeVisual>();
        if (plantation != null)
            plantation.Apply(this, targetHeight, ScarsLookHealed());
        else
        {
            if (polishedNaturalHeight < 0f)
            {
                Bounds bounds = LocalRenderBounds(polishedVisual.transform);
                polishedNaturalHeight = Mathf.Max(0.1f, bounds.size.y);
            }
            polishedVisual.transform.localScale = Vector3.one * (targetHeight / polishedNaturalHeight);
        }
        polishedVisual.transform.localPosition = Vector3.zero;
        polishedVisual.SetActive(!IsStump);

        if (stumpPrefab != null)
        {
            if (stumpVisual == null)
            {
                Transform existingStump = transform.Find("StumpVisual");
                stumpVisual = existingStump != null ? existingStump.gameObject : Instantiate(stumpPrefab, transform);
                stumpVisual.name = "StumpVisual";
                Bounds stumpBounds = LocalRenderBounds(stumpVisual.transform);
                stumpNaturalHeight = Mathf.Max(0.1f, stumpBounds.size.y);
            }
            StripInteractionColliders(stumpVisual.transform);
            stumpVisual.transform.localScale = Vector3.one * (0.35f / stumpNaturalHeight);
            stumpVisual.transform.localPosition = Vector3.zero;
            stumpVisual.SetActive(IsStump);
        }
        if (stumpVisual != null)
            DestroyDuplicateChildren("StumpVisual", stumpVisual.transform);
    }

    private void DestroyDuplicateChildren(string childName, Transform keep)
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            if (child != keep && child.name == childName)
            {
                child.gameObject.SetActive(false);
                child.name = "RetiredVisual";
                if (Application.isPlaying)
                    Destroy(child.gameObject);
                else
                    DestroyImmediate(child.gameObject);
            }
        }
    }

    // Polished visuals are display-only; strip their colliders so aim raycasts
    // keep hitting the trunk proxy. Idempotent and safe in edit and play mode.
    private void StripInteractionColliders(Transform root)
    {
        foreach (var collider in root.GetComponentsInChildren<Collider>())
        {
            if (Application.isPlaying)
                Destroy(collider);
            else
                DestroyImmediate(collider);
        }
    }

    private static Bounds LocalRenderBounds(Transform root)
    {
        Bounds bounds = new Bounds(root.position, Vector3.zero);
        bool hasBounds = false;
        foreach (var renderer in root.GetComponentsInChildren<Renderer>())
        {
            if (!hasBounds)
            {
                bounds = renderer.bounds;
                hasBounds = true;
            }
            else
            {
                bounds.Encapsulate(renderer.bounds);
            }
        }
        return bounds;
    }

    public int AddChop()
    {
        chopProgress++;
        return chopProgress;
    }

    public void Fell()
    {
        if (!CanChop)
            return;
        SetStage(ForestTreeStage.Stump);
        Felled?.Invoke(this);
        markType = TreeMarkType.None;
    }

    // Explicit foundation API only: no annual mortality trigger calls this.
    // Physical data remains available for a later accepted deadwood resolver.
    // Until a standing/fallen outcome is specified, hide the unsupported living
    // representation rather than turning death into a harvested stump/art event.
    public bool ApplyMortality(string cause, int year)
    {
        if (!IsLiving) return false;
        if (string.IsNullOrWhiteSpace(cause) || year < 0)
            throw new System.ArgumentException("Mortality requires a non-empty cause and non-negative year.");
        biologicallyDead = true;
        mortalityCause = cause.Trim();
        mortalityYear = year;
        markType = TreeMarkType.None;
        chopProgress = 0;
        gameObject.SetActive(false);
        MortalityApplied?.Invoke(this);
        return true;
    }

    // Save restoration is state restoration, not a new death/harvest event.
    public void RestoreMortality(bool dead, string cause, int year)
    {
        if (dead && (string.IsNullOrWhiteSpace(cause) || year < 0))
            throw new System.ArgumentException("Invalid persisted mortality cause/year.");
        bool wasDead = biologicallyDead;
        biologicallyDead = dead;
        mortalityCause = dead ? cause.Trim() : "";
        mortalityYear = dead ? year : -1;
        if (dead)
        {
            markType = TreeMarkType.None;
            gameObject.SetActive(false);
        }
        else if (wasDead)
            gameObject.SetActive(true);
    }

    public void RestoreState(ForestTreeStage restoredStage, float restoredStageTimer, int restoredChopProgress)
    {
        SetStage(restoredStage);
        stageTimer = Mathf.Max(0f, restoredStageTimer);
        chopProgress = Mathf.Max(0, restoredChopProgress);
    }

    private void SetStage(ForestTreeStage next)
    {
        stage = next;
        chopProgress = 0;
        stageTimer = 0f;
        ApplyStageShape(next);
    }

    private void ApplyStageShape(ForestTreeStage target)
    {
        switch (target)
        {
            case ForestTreeStage.Stump:
                SetCanopyActive(false, Vector3.zero, 0f);
                SetTrunkShape(0.35f, MatureThickness * 1.15f);
                break;
            case ForestTreeStage.Sapling:
                SetCanopyActive(true, new Vector3(1.1f, 1.2f, 1.1f), 1.1f);
                SetTrunkShape(1.1f, MatureThickness * 0.45f);
                break;
            case ForestTreeStage.Young:
                SetCanopyActive(true, MatureCanopyScale * 0.6f, 2.7f);
                SetTrunkShape(2.7f, MatureThickness * 0.7f);
                break;
            default:
                ApplyMatureShape();
                return;
        }
        ApplyVisualOverride(CurrentHeight);
    }

    // Simulation writes go through these so mesh scale can never drive tree state.
    public void ApplyGrowth(float dbhDeltaCm, float heightDeltaM)
    {
        if (!IsLiving) return;
        diameterCm = Mathf.Clamp(diameterCm + dbhDeltaCm, 1f, 200f);
        heightMeters = Mathf.Clamp(heightMeters + heightDeltaM, 0.1f, 60f);
        RefreshVisuals();
    }

    public void RelaxCrownRadius(float targetRadiusM, float relaxationPerYear)
    {
        if (!IsLiving) return;
        crownRadiusMeters = Mathf.Max(0.1f, Mathf.Lerp(crownRadiusMeters, targetRadiusM, Mathf.Clamp01(relaxationPerYear)));
        RefreshVisuals();
    }

    public void SetAgeYears(int age)
    {
        ageYears = Mathf.Max(0, age);
    }

    public void SetSimulationState(int age, float height, float dbhCm, float crownRadius)
    {
        ageYears = Mathf.Max(0, age);
        heightMeters = Mathf.Clamp(height, 0.1f, 60f);
        diameterCm = Mathf.Clamp(dbhCm, 1f, 200f);
        crownRadiusMeters = Mathf.Max(0.1f, crownRadius);
        RefreshVisuals();
    }

    public void InitializeForSpawn(string id, Transform trunkTransform, Transform canopyTransform, TreeSpeciesDefinition speciesDefinition, int age, float height, float dbhCm, float crownRadius)
    {
        treeId = id;
        trunk = trunkTransform;
        canopy = canopyTransform;
        species = speciesDefinition;
        SetSimulationState(age, height, dbhCm, crownRadius);
    }

    public void RefreshVisuals()
    {
        if (biologicallyDead)
        {
            gameObject.SetActive(false);
            return;
        }
        ApplyStageShape(stage);
    }

    private void SetTrunkShape(float height, float thickness)
    {
        if (trunk == null)
            return;

        trunk.localScale = new Vector3(thickness, height * 0.5f, thickness);
        Vector3 position = trunk.localPosition;
        trunk.localPosition = new Vector3(position.x, height * 0.5f, position.z);
    }

    private void SetCanopyActive(bool active, Vector3 scale, float localHeight)
    {
        if (canopy == null)
            return;

        canopy.gameObject.SetActive(active);
        if (!active)
            return;

        canopy.localScale = scale;
        Vector3 position = canopy.localPosition;
        canopy.localPosition = new Vector3(position.x, localHeight, position.z);
    }
}
