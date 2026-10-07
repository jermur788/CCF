using System.Collections.Generic;
using UnityEngine;

// Transient geometry only: no scene, save, target material or collider changes.
public sealed class ClearancePreview : MonoBehaviour
{
    private GameObject root;
    private Material material;
    private readonly List<LineRenderer> lines = new List<LineRenderer>();
    public ClearanceTargets Targets { get; private set; }
    public bool Visible => root != null && root.activeSelf;
    // Names the young trees it would remove; makes no claim that clearing helps seedlings.
    public string Label => Targets == null ? "" : "Clearance · " + Targets.Footprint.SizeLabel
        + ": removes ground plants and young trees inside\nAffected now: " + Targets.Summary;

    public void Show(ClearanceTargets targets)
    {
        Targets = targets;
        if (root == null)
        {
            root = new GameObject("Clearance footprint preview"); root.transform.SetParent(transform, false);
            Shader shader = Shader.Find("Universal Render Pipeline/Unlit") ?? Shader.Find("Unlit/Color");
            material = new Material(shader) { name = "Transient clearance boundary", color = new Color(1f, .82f, .2f) };
        }
        root.SetActive(true);
        int used = 0;
        ClearanceFootprint f = targets.Footprint;
        var boundary = new List<Vector3>();
        if (f.IsCircle)
            for (int i = 0; i <= 64; i++)
            {
                float angle = i * Mathf.PI * 2f / 64;
                boundary.Add(f.Center + new Vector3(Mathf.Cos(angle) * f.Radius, .045f, Mathf.Sin(angle) * f.Radius));
            }
        else
        {
            boundary.Add(f.Center + new Vector3(-f.HalfCell, .045f, -f.HalfCell));
            boundary.Add(f.Center + new Vector3(f.HalfCell, .045f, -f.HalfCell));
            boundary.Add(f.Center + new Vector3(f.HalfCell, .045f, f.HalfCell));
            boundary.Add(f.Center + new Vector3(-f.HalfCell, .045f, f.HalfCell));
            boundary.Add(boundary[0]);
        }
        Draw(used++, boundary.ToArray(), .035f);
        foreach (ClearanceCohortTarget c in targets.Cohorts)
            if (f.Contains(c.DisplayPosition)) Pin(ref used, c.DisplayPosition);
        foreach (PlantedJuvenile j in targets.Juveniles) Pin(ref used, j.position);
        foreach (Vector3 p in targets.GroundPlants) Pin(ref used, p);
        for (int i = used; i < lines.Count; i++) lines[i].gameObject.SetActive(false);
    }
    private void Pin(ref int used, Vector3 p)
    {
        // A raised diamond + stem is legible without relying only on colour.
        Draw(used++, new[] { p + Vector3.up * .08f, p + Vector3.up * .75f,
            p + new Vector3(-.12f, .6f, 0f), p + Vector3.up * .45f,
            p + new Vector3(.12f, .6f, 0f), p + Vector3.up * .75f }, .025f);
    }
    private void Draw(int index, Vector3[] points, float width)
    {
        if (index == lines.Count)
        {
            var o = new GameObject("Boundary / affected target"); o.transform.SetParent(root.transform, false);
            var line = o.AddComponent<LineRenderer>(); line.sharedMaterial = material; line.useWorldSpace = true;
            line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off; line.receiveShadows = false;
            lines.Add(line);
        }
        LineRenderer renderer = lines[index]; renderer.gameObject.SetActive(true);
        renderer.startWidth = renderer.endWidth = width; renderer.positionCount = points.Length; renderer.SetPositions(points);
    }
    public void Clear()
    { Targets = null; if (root != null) root.SetActive(false); }
    private void LateUpdate()
    {
        ForestPlayer player = GetComponent<ForestPlayer>();
        ScenarioOneManager manager = Object.FindFirstObjectByType<ScenarioOneManager>();
        if (player == null || !player.enabled || Cursor.lockState != CursorLockMode.Locked || player.IsInspecting
            || manager == null || manager.AnyPanelOpen || manager.ReferencePreviewActive) Clear();
    }
    private void OnDisable() => Clear();
    private void OnDestroy() { if (root != null) Destroy(root); if (material != null) Destroy(material); }
}
