using UnityEngine;

// A neutral world-space cage around the tree whose card is open: a ring on the ground, a ring above eye level and four
// uprights, in pale white. It shows WHICH tree the card describes without borrowing any management colour (red is a Fell
// mark, blue a Crop Tree, violet a planned extraction rack, gold the numbered competitor rings).
//
// Presentation only: it reads the tree, writes no tree, mark, save or ecology state, and is switched off whenever no tree is
// being inspected. The lines are drawn in world space, so the cage stays on the tree while the player moves around it.
public sealed class InspectedTreeIndicator
{
    private const int RingPoints = 40;
    private const float CageHeightM = 1.9f;
    private const float LineWidthM = 0.022f;
    private static readonly Color CageColour = new Color(0.94f, 0.97f, 1f, 1f);

    private GameObject root;
    private Material material;
    private LineRenderer bottom, top;
    private readonly LineRenderer[] uprights = new LineRenderer[4];
    private ForestTree shown;

    // The tree the cage is on, or null when it is hidden. The verification reads this to check the cage follows the card.
    public ForestTree Target => shown;
    public bool Visible => root != null && root.activeSelf && shown != null;

    public void Update(ForestTree inspected)
    {
        if (inspected == null || !inspected.IsLiving)
        {
            Hide();
            return;
        }
        Ensure();
        shown = inspected;
        root.SetActive(true);
        // Trunk radius from DBH, plus a margin that keeps the cage clear of the bark and of the red/blue ground marks (about 0.6 m).
        float radius = Mathf.Max(0.75f, inspected.Diameter / 200f + 0.5f);
        Vector3 foot = inspected.transform.position + Vector3.up * 0.05f;
        SetRing(bottom, foot, radius);
        SetRing(top, foot + Vector3.up * CageHeightM, radius);
        for (int i = 0; i < uprights.Length; i++)
        {
            float angle = i * Mathf.PI * 0.5f + Mathf.PI * 0.25f;
            Vector3 edge = foot + new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius);
            uprights[i].SetPosition(0, edge);
            uprights[i].SetPosition(1, edge + Vector3.up * CageHeightM);
        }
    }

    public void Hide()
    {
        shown = null;
        if (root != null) root.SetActive(false);
    }

    public void Destroy()
    {
        if (root != null) Object.Destroy(root);
        if (material != null) Object.Destroy(material);
        root = null;
        material = null;
    }

    private void Ensure()
    {
        if (root != null) return;
        root = new GameObject("Inspected tree indicator (runtime)");
        Shader shader = Shader.Find("Universal Render Pipeline/Unlit") ?? Shader.Find("Unlit/Color");
        material = new Material(shader) { name = "Transient inspected-tree cage", color = CageColour };
        bottom = NewLine("Ground ring", RingPoints, true);
        top = NewLine("Upper ring", RingPoints, true);
        for (int i = 0; i < uprights.Length; i++) uprights[i] = NewLine("Upright " + (i + 1), 2, false);
    }

    private LineRenderer NewLine(string name, int points, bool loop)
    {
        var go = new GameObject(name);
        go.transform.SetParent(root.transform, false);
        var line = go.AddComponent<LineRenderer>();
        line.sharedMaterial = material;
        line.useWorldSpace = true;
        line.loop = loop;
        line.positionCount = points;
        line.startWidth = line.endWidth = LineWidthM;
        line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        line.receiveShadows = false;
        return line;
    }

    private static void SetRing(LineRenderer ring, Vector3 centre, float radius)
    {
        for (int p = 0; p < ring.positionCount; p++)
        {
            float angle = p * Mathf.PI * 2f / ring.positionCount;
            ring.SetPosition(p, centre + new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius));
        }
    }
}
