using UnityEngine;

public sealed class ForestEcologyController : MonoBehaviour
{
    [SerializeField] private int ecologicalYear;
    [SerializeField] private float standSizeMeters = 40f;
    [SerializeField] private float cellSizeMeters = 5f;
    [SerializeField] private bool showDebugGrid;

    private ForestEcologyCell[] cells;
    private int cellsPerAxis;

    public int EcologicalYear => ecologicalYear;
    public int CellCount => cells != null ? cells.Length : 0;
    public int CellsPerAxis => cellsPerAxis;
    public float CellSizeMeters => cellSizeMeters;
    public ForestEcologyCell[] Cells => cells;

    public bool ShowDebugGrid
    {
        get => showDebugGrid;
        set => showDebugGrid = value;
    }

    private void Awake()
    {
        RebuildGrid();
    }

    private void OnEnable()
    {
        ForestTree.Felled += OnTreeFelled;
    }

    private void OnDisable()
    {
        ForestTree.Felled -= OnTreeFelled;
    }

    private void OnTreeFelled(ForestTree tree)
    {
        RecomputeCanopy();
    }

    // One explicit step for editor, MCP and debug tooling. Nothing in normal
    // gameplay advances ecological time yet; one ecological year is not tied
    // to a game day or real-time minute.
    [ContextMenu("Advance one ecological year")]
    public void AdvanceOneYear()
    {
        ecologicalYear++;
        RecomputeCanopy();
    }

    [ContextMenu("Rebuild ecology grid")]
    public void RebuildGrid()
    {
        cellsPerAxis = Mathf.Max(1, Mathf.CeilToInt(standSizeMeters / cellSizeMeters));
        cells = new ForestEcologyCell[cellsPerAxis * cellsPerAxis];
        float origin = -standSizeMeters * 0.5f;
        for (int z = 0; z < cellsPerAxis; z++)
        for (int x = 0; x < cellsPerAxis; x++)
        {
            float centerX = origin + (x + 0.5f) * cellSizeMeters;
            float centerZ = origin + (z + 0.5f) * cellSizeMeters;
            cells[z * cellsPerAxis + x] = new ForestEcologyCell
            {
                Center = new Vector2(centerX, centerZ),
                Canopy = 1f,
                Light = 0f
            };
        }
        RecomputeCanopy();
    }

    // Simplified local crown influence: each living crown shades a cell by a
    // lateral falloff from its authoritative position, crown radius and height,
    // combined as fractional cover (1 - product of gaps). No global percentage.
    [ContextMenu("Recompute canopy and light")]
    public void RecomputeCanopy()
    {
        if (cells == null)
            return;

        ForestTree[] trees = Object.FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (ForestEcologyCell cell in cells)
        {
            float gap = 1f;
            foreach (ForestTree tree in trees)
            {
                if (tree == null || tree.IsStump)
                    continue;
                Vector2 treePosition = new Vector2(tree.transform.position.x, tree.transform.position.z);
                float distance = Vector2.Distance(cell.Center, treePosition);
                // Crown influence reaches half a cell beyond the crown so that
                // cells overlapping the crown respond, not only its centre.
                float reach = tree.CrownRadius * 1.5f + cellSizeMeters * 0.5f;
                float lateral = Mathf.Clamp01(1f - distance / reach);
                float vertical = Mathf.Clamp01(tree.Height / 8f);
                float influence = Mathf.Clamp01(lateral * vertical);
                gap *= 1f - influence;
            }
            float canopy = Mathf.Clamp01(1f - gap);
            cell.Canopy = canopy;
            cell.Light = 1f - canopy;
        }
    }

    private void OnGUI()
    {
        if (!showDebugGrid || cells == null || cellsPerAxis <= 0)
            return;

        const float mapSize = 180f;
        float cell = mapSize / cellsPerAxis;
        Rect origin = new Rect(Screen.width - mapSize - 20f, 20f, mapSize, mapSize);
        GUI.Box(new Rect(origin.x - 8f, origin.y - 26f, mapSize + 16f, mapSize + 34f), GUIContent.none);

        for (int z = 0; z < cellsPerAxis; z++)
        for (int x = 0; x < cellsPerAxis; x++)
        {
            ForestEcologyCell data = cells[z * cellsPerAxis + x];
            Color previous = GUI.color;
            GUI.color = Color.Lerp(new Color(0.95f, 0.9f, 0.45f), new Color(0.05f, 0.28f, 0.06f), data.Canopy);
            GUI.DrawTexture(new Rect(origin.x + x * cell, origin.y + (cellsPerAxis - 1 - z) * cell, cell - 1f, cell - 1f), Texture2D.whiteTexture);
            GUI.color = previous;
        }

        GUI.Label(new Rect(origin.x - 4f, origin.y - 22f, mapSize + 12f, 20f), $"CCF ecology — year {ecologicalYear} — {cellSizeMeters:0.#} m cells");
    }
}
