using System.Collections.Generic;
using System.Text;
using UnityEngine;

// Shows, in the forest, every tree that is in the Work Plan for felling, so nothing is removed that the player cannot see.
//
// A Fell mark (red) is a proposal; when the Work Plan opens, the marks become felling orders and the marks are consumed. Without
// this, the trees that are about to be felled would carry no sign at all between planning and harvest. Trees planned as part of an
// extraction rack carry a VIOLET marker (two stacked diamonds and a wider ground ring), every other planned tree the same red as
// a Fell mark. Neither is blue (Crop Tree), and the neutral white cage is reserved for the inspected tree.
//
// Presentation only: the markers are parented to the trees, are rebuilt from the work orders a few times a second, and write
// no tree, order, save or ecology state. The grouping into racks is session state (see ScenarioOneManager.Racks), so after a
// load rack trees show as ordinary planned trees.
public sealed class PlannedFellMarkers : MonoBehaviour
{
    public static readonly Color RackColour = new Color(0.74f, 0.46f, 0.95f, 1f);
    public static readonly Color FellColour = new Color(1f, 0.35f, 0.05f, 1f);
    private const float SyncSeconds = 0.25f;

    private ScenarioOneManager manager;
    private readonly Dictionary<string, GameObject> markers = new Dictionary<string, GameObject>();
    private readonly Dictionary<string, bool> markerIsRack = new Dictionary<string, bool>();
    private Material rackMaterial, fellMaterial;
    private string signature = "";
    private float nextSync;

    // How many trees currently carry a planned-felling marker, and how many of those are rack trees (the verification reads these).
    public int MarkerCount => markers.Count;
    public int RackMarkerCount { get { int n = 0; foreach (bool rack in markerIsRack.Values) if (rack) n++; return n; } }
    public bool HasMarker(string treeId) => markers.ContainsKey(treeId);
    public bool IsRackMarker(string treeId) => markerIsRack.TryGetValue(treeId, out bool rack) && rack;

    private void Update()
    {
        if (Time.unscaledTime < nextSync)
            return;
        nextSync = Time.unscaledTime + SyncSeconds;
        if (manager == null)
            manager = GetComponent<ScenarioOneManager>() ?? FindFirstObjectByType<ScenarioOneManager>();
        if (manager == null)
            return;
        // A Reference Future preview swaps the forest for the frozen reference: the player's plan has no trees there to mark.
        if (manager.ReferencePreviewActive)
        {
            ClearAll();
            return;
        }
        Sync();
    }

    private void ClearAll()
    {
        foreach (GameObject marker in markers.Values)
            if (marker != null) Destroy(marker);
        markers.Clear();
        markerIsRack.Clear();
        signature = "";
    }

    private void Sync()
    {
        var wanted = new Dictionary<string, bool>(System.StringComparer.Ordinal);
        var sb = new StringBuilder();
        foreach (ScenarioOneWorkOrder order in manager.WorkOrders)
        {
            if (!order.IsOpen || order.type != ScenarioWorkType.FellTree || string.IsNullOrEmpty(order.targetTreeId))
                continue;
            // V2 communicates confirmed routes on the map and painted entrances in the forest.
            // Automatic pending-order discs would look like orange marks before the player sprays.
            if (manager.RackPlanningV2Active && order.rackId > 0) continue;
            bool rack = manager.RackGroupOfOrder(order.workOrderId) != null;
            wanted[order.targetTreeId] = rack;
            sb.Append(order.targetTreeId).Append(rack ? 'R' : 'F').Append(';');
        }
        // The same trees can be replaced by a load, so the identity of the tree objects is part of what is watched.
        ForestTree[] all = FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        long identity = 17;
        foreach (ForestTree tree in all)
            if (tree != null && wanted.ContainsKey(tree.TreeId))
                identity += System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(tree) * 31L;
        sb.Append('#').Append(identity);
        string next = sb.ToString();
        if (next == signature)
            return;
        signature = next;

        var stale = new List<string>();
        foreach (KeyValuePair<string, GameObject> pair in markers)
            if (pair.Value == null || !wanted.TryGetValue(pair.Key, out bool rack) || markerIsRack[pair.Key] != rack)
                stale.Add(pair.Key);
        foreach (string id in stale)
        {
            if (markers[id] != null) Destroy(markers[id]);
            markers.Remove(id);
            markerIsRack.Remove(id);
        }
        foreach (ForestTree tree in all)
        {
            if (tree == null || !tree.IsLiving || markers.ContainsKey(tree.TreeId) || !wanted.TryGetValue(tree.TreeId, out bool isRack))
                continue;
            markers[tree.TreeId] = Build(tree, isRack);
            markerIsRack[tree.TreeId] = isRack;
        }
    }

    private GameObject Build(ForestTree tree, bool rack)
    {
        Material material = rack ? RackMaterial() : FellMaterial();
        var marker = new GameObject(rack ? "Rack Mark" : "Planned Fell Mark");
        marker.transform.SetParent(tree.transform, false);
        AddPrimitive(marker.transform, PrimitiveType.Cube, "Diamond", new Vector3(0f, tree.Height + 0.6f, 0f),
            Quaternion.Euler(45f, 45f, 0f), Vector3.one * 0.45f, material);
        if (rack)
            AddPrimitive(marker.transform, PrimitiveType.Cube, "Second diamond", new Vector3(0f, tree.Height + 1.25f, 0f),
                Quaternion.Euler(45f, 45f, 0f), Vector3.one * 0.3f, material);
        float disc = rack ? 1.7f : 1.2f;
        AddPrimitive(marker.transform, PrimitiveType.Cylinder, "Disc", new Vector3(0f, 0.05f, 0f),
            Quaternion.identity, new Vector3(disc, 0.02f, disc), material);
        return marker;
    }

    private static void AddPrimitive(Transform parent, PrimitiveType type, string name, Vector3 position, Quaternion rotation, Vector3 scale, Material material)
    {
        GameObject part = GameObject.CreatePrimitive(type);
        part.name = name;
        Collider collider = part.GetComponent<Collider>();
        if (collider != null) Destroy(collider);
        part.transform.SetParent(parent, false);
        part.transform.localPosition = position;
        part.transform.localRotation = rotation;
        part.transform.localScale = scale;
        Renderer renderer = part.GetComponent<Renderer>();
        renderer.sharedMaterial = material;
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
    }

    private Material RackMaterial() => rackMaterial != null ? rackMaterial : rackMaterial = NewMaterial("Planned rack mark", RackColour);
    private Material FellMaterial() => fellMaterial != null ? fellMaterial : fellMaterial = NewMaterial("Planned fell mark", FellColour);

    private static Material NewMaterial(string name, Color colour)
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Unlit") ?? Shader.Find("Unlit/Color");
        return new Material(shader) { name = name, color = colour };
    }

    private void OnDestroy()
    {
        foreach (GameObject marker in markers.Values)
            if (marker != null) Destroy(marker);
        markers.Clear();
        if (rackMaterial != null) Destroy(rackMaterial);
        if (fellMaterial != null) Destroy(fellMaterial);
    }
}
