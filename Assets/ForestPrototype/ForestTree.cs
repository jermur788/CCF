using UnityEngine;

// Simplified gameplay rules for the prototype, not researched forest ecology.
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
    [SerializeField] private string treeId = "";
    [SerializeField] private Transform trunk;
    [SerializeField] private Transform canopy;
    [SerializeField, Min(1)] private int chopsRequired = 4;
    // Placeholder timings for the prototype, not researched ecology.
    [SerializeField, Min(0.1f)] private float stumpToSaplingSeconds = 20f;
    [SerializeField, Min(0.1f)] private float saplingToYoungSeconds = 30f;

    private ForestTreeStage stage = ForestTreeStage.Mature;
    private int chopProgress;
    private float stageTimer;
    private Vector3 matureTrunkScale;
    private Vector3 matureCanopyScale;
    private Vector3 matureCanopyLocalPosition;
    private bool matureShapeCached;

    public string TreeId => treeId;
    public ForestTreeStage Stage => stage;
    public bool IsStump => stage == ForestTreeStage.Stump;
    public bool CanChop => stage == ForestTreeStage.Mature || stage == ForestTreeStage.Young;
    public int ChopsRequired => chopsRequired;
    public int ChopProgress => chopProgress;
    public float StageTimer => stageTimer;
    public float Height => trunk != null ? trunk.localScale.y * 2f : 0f;
    public float Diameter => trunk != null ? trunk.localScale.x * 100f : 0f;
    public int WoodYield => Mathf.Clamp(Mathf.RoundToInt(Height), 3, 10);
    // Scene tree roots sit at the world origin, so measure from the trunk instead.
    public Vector3 InteractionPoint => trunk != null ? trunk.position : transform.position;

    public string StageLabel
    {
        get
        {
            switch (stage)
            {
                case ForestTreeStage.Stump: return "Harvested stump";
                case ForestTreeStage.Sapling: return "Sapling (regrowing)";
                case ForestTreeStage.Young: return "Young growing stock";
                default: return "Mature canopy tree";
            }
        }
    }

    public float RegrowthRemaining
    {
        get
        {
            if (stage == ForestTreeStage.Stump)
                return Mathf.Max(0f, stumpToSaplingSeconds - stageTimer);
            if (stage == ForestTreeStage.Sapling)
                return Mathf.Max(0f, saplingToYoungSeconds - stageTimer);
            return 0f;
        }
    }

    private float BaseThickness
    {
        get
        {
            if (matureShapeCached && matureTrunkScale.x > 0f)
                return matureTrunkScale.x;
            return trunk != null ? trunk.localScale.x : 0.65f;
        }
    }

    private void Awake()
    {
        if (trunk == null)
            trunk = transform.Find("Trunk");
        if (canopy == null)
            canopy = transform.Find("Canopy");

        if (trunk == null)
            Debug.LogError("ForestTree requires a trunk child.", this);

        if (stage == ForestTreeStage.Mature)
        {
            matureTrunkScale = trunk != null ? trunk.localScale : Vector3.one;
            if (canopy != null)
            {
                matureCanopyScale = canopy.localScale;
                matureCanopyLocalPosition = canopy.localPosition;
            }
            matureShapeCached = true;
        }
    }

    private void Update()
    {
        if (stage == ForestTreeStage.Stump)
        {
            stageTimer += Time.deltaTime;
            if (stageTimer >= stumpToSaplingSeconds)
                SetStage(ForestTreeStage.Sapling);
        }
        else if (stage == ForestTreeStage.Sapling)
        {
            stageTimer += Time.deltaTime;
            if (stageTimer >= saplingToYoungSeconds)
                SetStage(ForestTreeStage.Young);
        }
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

        switch (next)
        {
            case ForestTreeStage.Stump:
                SetCanopyActive(false, Vector3.zero, 0f);
                SetTrunkShape(0.35f, BaseThickness * 1.15f);
                break;
            case ForestTreeStage.Sapling:
                SetCanopyActive(true, new Vector3(1.1f, 1.2f, 1.1f), 1.1f);
                SetTrunkShape(1.1f, BaseThickness * 0.45f);
                break;
            case ForestTreeStage.Young:
                SetCanopyActive(true, matureShapeCached ? matureCanopyScale * 0.6f : new Vector3(2f, 2.2f, 2f), 2.7f);
                SetTrunkShape(2.7f, BaseThickness * 0.7f);
                break;
            default:
                SetCanopyActive(true, matureShapeCached ? matureCanopyScale : (canopy != null ? canopy.localScale : Vector3.one),
                    matureShapeCached ? matureCanopyLocalPosition.y : (canopy != null ? canopy.localPosition.y : 5f));
                if (matureShapeCached)
                    SetTrunkShape(matureTrunkScale.y * 2f, matureTrunkScale.x);
                break;
        }
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
