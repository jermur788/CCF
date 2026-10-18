using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// The in-world extraction-rack tool. Press B in the forest, aim at a planted row, turn the corridor with the mouse wheel, click to
// add the rack to the draft Work Plan. A translucent violet corridor shows where the rack would run (boundary to boundary of
// the stand), and every tree it would take carries a violet ring and post; violet is used for nothing else, so it cannot be read
// as the red Fell mark or the blue Crop Tree.
//
// The tool only previews until the click. Escape, B or opening any panel leaves it without changing a mark, an order or a tree.
// It uses the same RackPlanner and ScenarioOneManager.ConfirmRack as the Stand Map, so a rack means the same wherever it is planned.
public sealed class RackTool : MonoBehaviour
{
    public const float RotateStepDegrees = 15f;
    private const float AimReachM = 80f;
    private const float RefreshSeconds = 0.5f;
    private const float NoticeSeconds = 4f;
    private static readonly Color CorridorColour = new Color(0.74f, 0.46f, 0.95f, 0.24f);
    private static readonly Color RingColour = new Color(0.82f, 0.58f, 1f, 1f);

    // True while a rack is being aimed: the forest ignores inspection, planting and clearance input meanwhile.
    public static bool AnyActive { get; private set; }
    // The frame in which the tool took Escape (to cancel). The player does not also free the mouse on that Escape, whichever of the
    // two components updates first.
    public static int EscapeFrame { get; private set; } = -1;

    private ScenarioOneUiRoot ui;
    private ScenarioOneManager manager;
    private ForestEcologyController ecology;
    private Camera view;
    private RackPlanner planner;
    private bool active, introPending, wasLocked;
    private float angle, nextRefresh, noticeUntil, introClosedAt = -1f;
    private string notice = "";
    private RackProposal proposal;
    private GameObject root;
    private Mesh mesh;
    private MeshRenderer corridor;
    private Material corridorMaterial, ringMaterial;
    private readonly List<LineRenderer> lines = new List<LineRenderer>();
    private readonly RaycastHit[] hits = new RaycastHit[16];

    public bool Active => active;
    // True from the first B press of a new device until the first-use screen is closed (the tool opens when it is).
    public bool IntroPending => introPending;
    public RackProposal Proposal => active ? proposal : null;
    public float AngleDegrees => angle;
    public int ConfirmedCount { get; private set; }
    public string Notice => Time.unscaledTime < noticeUntil ? notice : "";
    public RackPlanner Planner => planner;

    public void Initialise(ScenarioOneUiRoot owner, ScenarioOneManager scenario, ForestEcologyController eco)
    {
        ui = owner;
        manager = scenario;
        ecology = eco;
        planner = new RackPlanner(manager, ecology);
        view = Camera.main;
    }

    private void Update()
    {
        if (manager == null)
            return;
        Keyboard keyboard = Keyboard.current;
        Mouse mouse = Mouse.current;
        bool locked = Cursor.lockState == CursorLockMode.Locked;

        bool canUse = locked && !manager.AnyPanelOpen && !manager.ReferencePreviewActive && !ui.Help.IsOpen && manager.CanEditWorkPlan;
        if (introPending && !ui.Help.IsOpen)
        {
            // The first-use screen has been closed: open the tool as soon as the mouse is back in the forest, or give up if it is not.
            if (introClosedAt < 0f) introClosedAt = Time.unscaledTime;
            if (canUse)
            {
                introPending = false;
                introClosedAt = -1f;
                Enter();
            }
            else if (Time.unscaledTime - introClosedAt > 1f)
            {
                introPending = false;
                introClosedAt = -1f;
            }
        }
        if (!active)
        {
            if (keyboard != null && keyboard.bKey.wasPressedThisFrame && canUse)
                Request();
            wasLocked = locked;
            return;
        }
        // Anything that frees the mouse or opens a panel ends the preview without a change.
        if (!locked || !canUse || (keyboard != null && (keyboard.escapeKey.wasPressedThisFrame || keyboard.bKey.wasPressedThisFrame)))
        {
            if (keyboard != null && keyboard.escapeKey.wasPressedThisFrame) EscapeFrame = Time.frameCount;
            Exit();
            wasLocked = locked;
            return;
        }

        if (mouse != null)
        {
            float wheel = mouse.scroll.ReadValue().y;
            if (Mathf.Abs(wheel) > 0.1f)
                Turn(wheel > 0f ? 1 : -1);
        }
        if (Time.unscaledTime >= nextRefresh)
        {
            nextRefresh = Time.unscaledTime + RefreshSeconds;
            planner.Refresh();
        }
        if (TryAim(out RackPoint aim))
            proposal = planner.Propose(aim, angle);
        Draw();

        // The click that took the mouse back (after Esc) must not also confirm a rack.
        bool click = mouse != null && mouse.leftButton.wasPressedThisFrame && wasLocked;
        bool enter = keyboard != null && (keyboard.enterKey.wasPressedThisFrame || keyboard.numpadEnterKey.wasPressedThisFrame);
        if ((click || enter) && proposal != null)
            Confirm();
        wasLocked = locked;
    }

    // B: open the tool. The first time on a device the short explanation is shown first and the tool opens when it is closed.
    private void Request()
    {
        if (!MenuHelpView.Introduced(MenuHelpView.Menu.Rack))
        {
            introPending = true;
            ui.Help.Introduce(MenuHelpView.Menu.Rack);
            manager.SetAuxiliaryPanelOpen(true);
            return;
        }
        Enter();
    }

    public void Enter()
    {
        if (manager == null || active)
            return;
        planner.Refresh();
        nextRefresh = Time.unscaledTime + RefreshSeconds;
        Vector3 forward = view != null ? view.transform.forward : Vector3.forward;
        float facing = Mathf.Atan2(forward.z, forward.x) * Mathf.Rad2Deg;
        angle = planner.DefaultAngle(facing, manager.RackFirstStageActive && !LayoutComplete());
        active = true;
        AnyActive = true;
        proposal = null;
        EnsureVisuals();
    }

    public void Exit()
    {
        active = false;
        AnyActive = false;
        proposal = null;
        introPending = false;
        if (root != null)
            root.SetActive(false);
    }

    public void Turn(int steps)
    {
        angle = RackGeometry.NormaliseAngle(angle + steps * RotateStepDegrees);
    }

    // Left click or Enter: the previewed rack joins the draft Work Plan. The tool stays open for the next rack.
    public bool Confirm()
    {
        if (proposal == null || !proposal.CanConfirm)
        {
            Say(proposal != null && !string.IsNullOrEmpty(proposal.Problem) ? proposal.Problem : "Aim at a planted row first.");
            return false;
        }
        int added = manager.ConfirmRack(proposal, "forest");
        Say(added > 0 ? $"Rack added to the Work Plan: {added} tree{(added == 1 ? "" : "s")}." : manager.Feedback);
        if (added > 0)
            ConfirmedCount++;
        planner.Refresh();
        proposal = null;
        return added > 0;
    }

    private bool LayoutComplete()
    {
        foreach (RackLayoutLine line in planner.LayoutStatus())
            if (!line.Done) return false;
        return true;
    }

    private void Say(string text)
    {
        notice = text;
        noticeUntil = Time.unscaledTime + NoticeSeconds;
    }

    // The point on the ground (or tree) the reticle is on; a look at the sky uses the ground plane under the player.
    private bool TryAim(out RackPoint aim)
    {
        aim = default;
        if (view == null)
            view = Camera.main;
        if (view == null)
            return false;
        Transform me = ui.Player != null ? ui.Player.transform : null;
        int count = Physics.RaycastNonAlloc(view.transform.position, view.transform.forward, hits, AimReachM);
        bool found = false;
        RaycastHit nearest = default;
        for (int i = 0; i < count; i++)
        {
            Transform t = hits[i].collider.transform;
            if (me != null && (t == me || t.IsChildOf(me)))
                continue;
            if (!found || hits[i].distance < nearest.distance)
            {
                nearest = hits[i];
                found = true;
            }
        }
        if (found)
        {
            aim = new RackPoint(nearest.point.x, nearest.point.z);
            return true;
        }
        float groundY = me != null ? me.position.y : 0f;
        Vector3 origin = view.transform.position, dir = view.transform.forward;
        if (dir.y < -0.02f)
        {
            float t = (groundY - origin.y) / dir.y;
            Vector3 p = origin + dir * t;
            aim = new RackPoint(p.x, p.z);
            return true;
        }
        return proposal != null;
    }

    private void EnsureVisuals()
    {
        if (root == null)
        {
            // A scene root, not a child of the player: the corridor mesh is built in world coordinates.
            root = new GameObject("Extraction rack preview (runtime)");
            mesh = new Mesh { name = "Rack corridor preview" };
            mesh.MarkDynamic();
            var corridorObject = new GameObject("Corridor");
            corridorObject.transform.SetParent(root.transform, false);
            corridorObject.AddComponent<MeshFilter>().sharedMesh = mesh;
            corridor = corridorObject.AddComponent<MeshRenderer>();
            // Sprites/Default is always included in a build; it is unlit and alpha-blended, which is all a floor tint needs.
            Shader blended = Shader.Find("Sprites/Default") ?? Shader.Find("Universal Render Pipeline/Unlit");
            corridorMaterial = new Material(blended) { name = "Rack corridor preview", color = CorridorColour };
            corridor.sharedMaterial = corridorMaterial;
            corridor.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            corridor.receiveShadows = false;
            Shader unlit = Shader.Find("Universal Render Pipeline/Unlit") ?? Shader.Find("Unlit/Color");
            ringMaterial = new Material(unlit) { name = "Rack tree ring", color = RingColour };
        }
        root.SetActive(true);
    }

    private void Draw()
    {
        if (root == null)
            return;
        if (proposal == null || proposal.Selection == null || proposal.Selection.LengthM <= 0.01f)
        {
            corridor.enabled = false;
            foreach (LineRenderer line in lines) line.gameObject.SetActive(false);
            return;
        }
        float ground = GroundHeight(proposal.Selection.Start, proposal.Selection.End);
        List<RackPoint> polygon = RackGeometry.CorridorPolygon(proposal.Selection.Start, proposal.Selection.End, proposal.WidthM, planner.HalfExtentM);
        corridor.enabled = polygon.Count >= 3;
        if (polygon.Count >= 3)
        {
            var vertices = new List<Vector3>(polygon.Count);
            foreach (RackPoint p in polygon) vertices.Add(new Vector3(p.X, ground + 0.07f, p.Z));
            var triangles = new List<int>();
            for (int i = 1; i < vertices.Count - 1; i++) { triangles.Add(0); triangles.Add(i + 1); triangles.Add(i); }
            mesh.Clear();
            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateBounds();
        }
        // Two lines per selected tree: a ring at breast height and a post beside the trunk, so the rack's trees read from a distance.
        int index = 0;
        foreach (RackTree tree in proposal.Selection.Trees)
        {
            Vector3 foot = new Vector3(tree.X, ground, tree.Z);
            LineRenderer ring = Line(index++, 24, true, 0.05f);
            for (int p = 0; p < ring.positionCount; p++)
            {
                float a = p * Mathf.PI * 2f / ring.positionCount;
                ring.SetPosition(p, foot + new Vector3(Mathf.Cos(a) * 0.62f, 1.3f, Mathf.Sin(a) * 0.62f));
            }
            LineRenderer post = Line(index++, 2, false, 0.06f);
            post.SetPosition(0, foot + new Vector3(0.62f, 0.05f, 0f));
            post.SetPosition(1, foot + new Vector3(0.62f, 2.6f, 0f));
        }
        for (int i = index; i < lines.Count; i++)
            lines[i].gameObject.SetActive(false);
    }

    private LineRenderer Line(int index, int points, bool loop, float width)
    {
        while (lines.Count <= index)
        {
            var go = new GameObject("Rack tree line " + lines.Count);
            go.transform.SetParent(root.transform, false);
            var line = go.AddComponent<LineRenderer>();
            line.sharedMaterial = ringMaterial;
            line.useWorldSpace = true;
            line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            line.receiveShadows = false;
            lines.Add(line);
        }
        LineRenderer result = lines[index];
        result.gameObject.SetActive(true);
        result.loop = loop;
        result.positionCount = points;
        result.startWidth = result.endWidth = width;
        return result;
    }

    // The stand's ground is one flat box named "Ground" (see StandWorldBoundary); the preview sits just above its top face. A ray from above
    // would hit a tree crown or trunk first, so the ground's own collider is used.
    private Collider groundCollider;
    private float GroundHeight(RackPoint start, RackPoint end)
    {
        if (groundCollider == null)
        {
            GameObject ground = GameObject.Find("Ground");
            groundCollider = ground != null ? ground.GetComponent<Collider>() : null;
        }
        return groundCollider != null ? groundCollider.bounds.max.y : 0f;
    }

    private void OnDisable()
    {
        Exit();
    }

    private void OnDestroy()
    {
        Exit();
        if (root != null) Destroy(root);
        if (mesh != null) Destroy(mesh);
        if (corridorMaterial != null) Destroy(corridorMaterial);
        if (ringMaterial != null) Destroy(ringMaterial);
    }
}
