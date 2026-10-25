using UnityEngine;
using UnityEngine.Rendering;

// The "selected tree" feedback for the tree whose card is open: a ring on the ground and an outline of the trunk, both in one muted
// ochre (the same colour as the card's accent edge, ScenarioOneUi.uss --select). It never uses white and borrows no management
// colour: a Fell mark is an orange diamond, a Crop Tree a blue trunk band, a planned rack violet, a listed competitor a teal numbered
// ring. Selection is a ring plus an outline; a management mark is a painted band or symbol. The style differs, not only the hue.
//
// Each line is drawn twice so it reads on dark bark, bright ground and dense stands alike: a dark casing under the ochre core (the
// casing is what keeps ochre visible against a bright background), and a faint copy of the core that shows only where another tree
// hides it, so an overlapped tree can still be told apart.
//
// Presentation only: it reads the tree, writes no tree, mark, save or ecology state, and is off whenever no tree is being inspected.
// The lines are in world space, so they stay on the tree while the player walks around it; the outline is rebuilt in LateUpdate,
// after the camera has turned, so it always faces the camera.
public sealed class InspectedTreeIndicator
{
    public static readonly Color SelectColour = UiKit.SelectionColour;
    // Selection fill on the ground, and the colour of the outline where another tree hides it.
    public static readonly Color SelectSoft = new Color(SelectColour.r, SelectColour.g, SelectColour.b, 0.42f);
    private static readonly Color CasingColour = new Color(0.12f, 0.08f, 0.03f, 0.88f);

    private const int RingPoints = 48;
    private const float OutlineHeightM = 4.0f;
    // Line thickness in screen pixels, converted to metres at the tree's distance so the outline stays about as thick on screen near
    // and far; the limits keep it from vanishing on a distant tree or becoming a slab on a near one.
    private const float CorePixels = 3.5f, CasingPixels = 8f;
    private const float MinWidthM = 0.03f, MaxWidthM = 0.30f;

    private sealed class Pass
    {
        public LineRenderer casing, core, hiddenCore;
    }

    private GameObject root;
    private Material solidMaterial, hiddenMaterial;
    private Pass ring, outline;
    private ForestTree shown;
    private float ringRadius;

    // The tree the indicator is on, or null when it is hidden. The verification reads this to check it follows the card.
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
        // Trunk radius from DBH, plus a margin that keeps the ring clear of the bark and of the red/blue ground marks (about 0.6 m).
        ringRadius = Mathf.Max(0.75f, inspected.Diameter / 200f + 0.5f);
        BuildRing(inspected);
        BuildOutline(inspected);
    }

    // Called every frame after the camera has moved, so the outline and the line widths follow the view.
    public void LateUpdate()
    {
        if (!Visible) return;
        BuildOutline(shown);
        SetWidth(ring, shown.transform.position, 1f);
    }

    public void Hide()
    {
        shown = null;
        if (root != null) root.SetActive(false);
    }

    public void Destroy()
    {
        if (root != null) Object.Destroy(root);
        if (solidMaterial != null) Object.Destroy(solidMaterial);
        if (hiddenMaterial != null) Object.Destroy(hiddenMaterial);
        root = null;
        solidMaterial = hiddenMaterial = null;
    }

    private void Ensure()
    {
        if (root != null) return;
        root = new GameObject("Selected tree indicator (runtime)");
        // Hidden/Internal-Colored is a built-in, always-included vertex-colour shader with a settable depth test; URP Unlit has none.
        Shader shader = Shader.Find("Hidden/Internal-Colored") ?? Shader.Find("Universal Render Pipeline/Unlit") ?? Shader.Find("Unlit/Color");
        solidMaterial = NewMaterial(shader, "Selected tree (visible part)", CompareFunction.LessEqual, 3500);
        hiddenMaterial = NewMaterial(shader, "Selected tree (where hidden)", CompareFunction.Greater, 3490);
        ring = NewPass("Ground ring", RingPoints, true, true);
        outline = NewPass("Trunk outline", 13, true, false);
    }

    private static Material NewMaterial(Shader shader, string name, CompareFunction depthTest, int queue)
    {
        var material = new Material(shader) { name = name, hideFlags = HideFlags.HideAndDontSave };
        // Alpha-blended, double-sided (the flat ring is seen from above and below), never writing depth.
        material.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
        material.SetInt("_Cull", (int)CullMode.Off);
        material.SetInt("_ZWrite", 0);
        material.SetInt("_ZTest", (int)depthTest);
        material.renderQueue = queue;
        return material;
    }

    private Pass NewPass(string name, int points, bool loop, bool flat)
    {
        var pass = new Pass
        {
            hiddenCore = NewLine(name + " (hidden part)", points, loop, flat, hiddenMaterial),
            casing = NewLine(name + " casing", points, loop, flat, solidMaterial),
            core = NewLine(name, points, loop, flat, solidMaterial)
        };
        pass.hiddenCore.startColor = pass.hiddenCore.endColor = SelectSoft;
        pass.casing.startColor = pass.casing.endColor = CasingColour;
        pass.core.startColor = pass.core.endColor = SelectColour;
        return pass;
    }

    private LineRenderer NewLine(string name, int points, bool loop, bool flat, Material material)
    {
        var go = new GameObject(name);
        go.transform.SetParent(root.transform, false);
        // A flat ribbon needs its facing axis pointing up; the outline is a camera-facing ribbon.
        if (flat) go.transform.rotation = Quaternion.LookRotation(Vector3.up);
        var line = go.AddComponent<LineRenderer>();
        line.sharedMaterial = material;
        line.useWorldSpace = true;
        line.alignment = flat ? LineAlignment.TransformZ : LineAlignment.View;
        line.loop = loop;
        line.positionCount = points;
        line.numCornerVertices = 2;
        line.numCapVertices = 2;
        line.shadowCastingMode = ShadowCastingMode.Off;
        line.receiveShadows = false;
        return line;
    }

    private static float GroundY(Vector3 position)
    {
        Terrain terrain = Terrain.activeTerrain;
        return terrain != null ? terrain.SampleHeight(position) + terrain.transform.position.y : position.y;
    }

    private static float WidthAt(Vector3 point, float pixels)
    {
        Camera camera = Camera.main;
        if (camera == null || Screen.height <= 0) return MinWidthM * 2f;
        float distance = Mathf.Max(1f, Vector3.Distance(camera.transform.position, point));
        float metresPerPixel = 2f * distance * Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad) / Screen.height;
        return Mathf.Clamp(pixels * metresPerPixel, MinWidthM, MaxWidthM);
    }

    private static void SetWidth(Pass pass, Vector3 at, float scale)
    {
        float core = WidthAt(at, CorePixels) * scale, casing = WidthAt(at, CasingPixels) * scale;
        pass.core.widthMultiplier = core;
        pass.hiddenCore.widthMultiplier = core;
        pass.casing.widthMultiplier = casing;
    }

    private static void SetPositions(Pass pass, Vector3[] points)
    {
        pass.core.SetPositions(points);
        pass.casing.SetPositions(points);
        pass.hiddenCore.SetPositions(points);
    }

    private readonly Vector3[] ringPoints = new Vector3[RingPoints];
    private void BuildRing(ForestTree tree)
    {
        Vector3 centre = tree.transform.position;
        for (int i = 0; i < RingPoints; i++)
        {
            float angle = i * Mathf.PI * 2f / RingPoints;
            Vector3 flat = centre + new Vector3(Mathf.Cos(angle) * ringRadius, 0f, Mathf.Sin(angle) * ringRadius);
            // Follow the ground so the ring is not buried on a slope; lifted a little so it is not lost in short vegetation.
            flat.y = GroundY(flat) + 0.08f;
            ringPoints[i] = flat;
        }
        SetPositions(ring, ringPoints);
        SetWidth(ring, centre, 1f);
    }

    private readonly Vector3[] outlinePoints = new Vector3[13];
    // A closed outline hugging the lower trunk, drawn in the plane facing the camera: up the left edge, over a rounded top, down the
    // right edge. Its extent comes from the tree's own DBH and height; it is an outline of where the stem is, not of the crown.
    private void BuildOutline(ForestTree tree)
    {
        Camera camera = Camera.main;
        Vector3 across = camera != null ? camera.transform.right : Vector3.right;
        across.y = 0f;
        across = across.sqrMagnitude < 1e-4f ? Vector3.right : across.normalized;
        Vector3 foot = tree.transform.position;
        foot.y = GroundY(foot);
        float height = Mathf.Min(OutlineHeightM, Mathf.Max(1.5f, tree.Height * 0.6f));
        // DBH is measured at 1.3 m; the stem is wider lower down (root flare) and its mesh is not a clean cylinder, so the outline stands off the bark by
        // a margin that grows with the trunk, or the near side of it would sit inside the bark and be hidden by it.
        float r0 = tree.Diameter / 200f * 1.3f + 0.08f, r1 = Mathf.Max(0.05f, r0 * 0.88f);
        // 13 points: bottom-left, up the left edge, an arch over the top (7 points), down the right edge, bottom-right.
        outlinePoints[0] = foot - across * r0;
        outlinePoints[1] = foot + Vector3.up * (height * 0.5f) - across * ((r0 + r1) * 0.5f);
        for (int i = 0; i < 7; i++)
        {
            float t = i / 6f;                       // 0 = left, 1 = right
            float side = Mathf.Lerp(-1f, 1f, t);
            float lift = Mathf.Sqrt(Mathf.Max(0f, 1f - side * side)) * Mathf.Max(0.12f, r1 * 0.9f);
            outlinePoints[2 + i] = foot + Vector3.up * (height + lift) + across * (side * r1);
        }
        outlinePoints[9] = foot + Vector3.up * (height * 0.5f) + across * ((r0 + r1) * 0.5f);
        outlinePoints[10] = foot + across * r0;
        outlinePoints[11] = foot + across * (r0 * 0.5f) + Vector3.up * 0.01f;
        outlinePoints[12] = foot - across * (r0 * 0.5f) + Vector3.up * 0.01f;
        SetPositions(outline, outlinePoints);
        SetWidth(outline, foot + Vector3.up * (height * 0.5f), 1f);
    }
}
