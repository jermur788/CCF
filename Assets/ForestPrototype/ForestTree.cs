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

[DisallowMultipleComponent]
public sealed class ForestTree : MonoBehaviour
{
    public static event System.Action<ForestTree> Felled;
    [SerializeField] private string treeId = "";
    [SerializeField] private Transform trunk;
    [SerializeField] private Transform canopy;
    [SerializeField] private TreeSpeciesDefinition species;
    [SerializeField, Min(0)] private int ageYears = 45;
    [SerializeField, Min(0.1f)] private float heightMeters = 5f;
    [SerializeField, Min(1f)] private float diameterCm = 65f;
    [SerializeField, Min(0.1f)] private float crownRadiusMeters = 1.65f;
    [SerializeField, Min(1)] private int chopsRequired = 4;
    // Optional polished visuals (Ultimate Nature spruce/stump). When set, the
    // placeholder trunk/canopy stay as invisible interaction proxies and the
    // polished meshes visualise the authoritative data instead.
    [SerializeField] private GameObject visualPrefab;
    [SerializeField] private GameObject stumpPrefab;
    [SerializeField] private bool visualOverrideActive;
    // Which prefab the baked PolishedVisual child was built from. Recorded so
    // a prefab swap (visual variety) replaces the old baked mesh instead of
    // reusing it. Stump visuals never vary, so they need no marker.
    [SerializeField] private string polishedVisualSourcePrefab = "";

    private GameObject polishedVisual;
    private GameObject stumpVisual;
    private float polishedNaturalHeight = -1f;
    private float stumpNaturalHeight = -1f;

    private ForestTreeStage stage = ForestTreeStage.Mature;
    private int chopProgress;
    private float stageTimer;

    public string TreeId => treeId;
    public TreeSpeciesDefinition Species => species;
    public int AgeYears => ageYears;
    public ForestTreeStage Stage => stage;
    // Display stage derives from age for living trees so recruited trees read
    // honestly ("young trees are not mature"); the stored stage stays
    // authoritative for felling, chopping and saves.
    public const int SaplingMaxAge = 5; // [D] display threshold
    public ForestTreeStage DisplayStage
    {
        get
        {
            if (stage == ForestTreeStage.Stump || stage == ForestTreeStage.Sapling)
                return stage;
            int onset = species != null ? Mathf.RoundToInt(species.MaturityOnsetYears) : 20;
            if (AgeYears < SaplingMaxAge)
                return ForestTreeStage.Sapling;
            if (AgeYears < onset)
                return ForestTreeStage.Young;
            return ForestTreeStage.Mature;
        }
    }
    public bool IsStump => stage == ForestTreeStage.Stump;
    public bool CanChop => stage == ForestTreeStage.Mature || stage == ForestTreeStage.Young;
    public int ChopsRequired => chopsRequired;
    public int ChopProgress => chopProgress;
    public float StageTimer => stageTimer;
    // Authoritative simulation values; placeholder meshes only visualise them.
    public float Height => CurrentHeight;
    public float Diameter => diameterCm;
    public float CrownRadius => crownRadiusMeters;
    public int WoodYield => Mathf.Clamp(Mathf.RoundToInt(Height), 3, 10);
    public Vector3 InteractionPoint => transform.position;

    // Biological timber interface: stem volume from DBH and form height.
    // Volume_m3 = DBH_cm^2 * 0.00007854 * formHeight_m  (0.00007854 = pi/4 * 1e-4).
    // Additive read-only output; gameplay wood yield stays separate.
    public float BiologicalStemVolumeM3
    {
        get
        {
            if (species == null || IsStump)
                return 0f;
            return Diameter * Diameter * 0.00007854f * (Height * species.FormHeightRatio);
        }
    }

    public string StageLabel
    {
        get
        {
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
        visualPrefab = visual;
        stumpPrefab = stump;
        visualOverrideActive = visual != null;
        RefreshVisuals();
    }

    private void ApplyVisualOverride(float targetHeight)
    {
        if (visualPrefab == null)
            return;

        Renderer trunkRenderer = trunk != null ? trunk.GetComponent<Renderer>() : null;
        if (trunkRenderer != null)
            trunkRenderer.enabled = false;
        if (canopy != null)
            canopy.gameObject.SetActive(false);

        Transform existing = transform.Find("PolishedVisual");
        if (existing != null && polishedVisualSourcePrefab != visualPrefab.name)
        {
            // The baked visual was built from a different model (prefab swap);
            // replace it so the authoritative prefab actually shows.
            if (Application.isPlaying)
                Destroy(existing.gameObject);
            else
                DestroyImmediate(existing.gameObject);
            existing = null;
            polishedNaturalHeight = -1f;
        }
        if (polishedVisual == null)
        {
            // The polished visual may already exist (saved with the scene from
            // an editor pass); reuse it instead of duplicating children.
            polishedVisual = existing != null ? existing.gameObject : Instantiate(visualPrefab, transform);
            polishedVisual.name = "PolishedVisual";
            polishedVisualSourcePrefab = visualPrefab.name;
            polishedNaturalHeight = -1f;
        }
        DestroyDuplicateChildren("PolishedVisual", polishedVisual.transform);
        StripInteractionColliders(polishedVisual.transform);
        if (polishedNaturalHeight < 0f)
        {
            Bounds bounds = LocalRenderBounds(polishedVisual.transform);
            polishedNaturalHeight = Mathf.Max(0.1f, bounds.size.y);
        }
        polishedVisual.transform.localScale = Vector3.one * (targetHeight / polishedNaturalHeight);
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
        diameterCm = Mathf.Clamp(diameterCm + dbhDeltaCm, 1f, 200f);
        heightMeters = Mathf.Clamp(heightMeters + heightDeltaM, 0.1f, 60f);
        RefreshVisuals();
    }

    public void RelaxCrownRadius(float targetRadiusM, float relaxationPerYear)
    {
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
