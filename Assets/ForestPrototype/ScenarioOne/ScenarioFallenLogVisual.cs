using UnityEngine;

// Rebuildable presentation of a recorded fallen stem. Both authored logs run
// along local X, are grounded, and scale to the already-recorded stem size.
public sealed class ScenarioFallenLogVisual : MonoBehaviour
{
    private GameObject model;
    private GameObject source;
    private Vector3 naturalSize;

    public void Refresh(ScenarioDeadwoodRecord record, SectionFiveVisualCatalog catalog)
    {
        GameObject prefab = record.DecayClass >= 2 ? catalog.decayedLog : catalog.freshLog;
        if (prefab == null) return;
        if (source != prefab || model == null)
        {
            if (model != null) { model.SetActive(false); Destroy(model); }
            model = Instantiate(prefab, transform);
            source = prefab;
            Bounds bounds = new Bounds();
            bool first = true;
            foreach (MeshFilter filter in model.GetComponentsInChildren<MeshFilter>(true))
            {
                if (filter.sharedMesh == null) continue;
                Bounds meshBounds = filter.sharedMesh.bounds;
                for (int corner = 0; corner < 8; corner++)
                {
                    Vector3 point = meshBounds.center + Vector3.Scale(meshBounds.extents,
                        new Vector3((corner & 1) == 0 ? -1 : 1,
                            (corner & 2) == 0 ? -1 : 1, (corner & 4) == 0 ? -1 : 1));
                    point = model.transform.InverseTransformPoint(filter.transform.TransformPoint(point));
                    if (first) { bounds = new Bounds(point, Vector3.zero); first = false; }
                    else bounds.Encapsulate(point);
                }
            }
            naturalSize = bounds.size;
        }
        float decayScale = Mathf.Pow(Mathf.Clamp01(record.remainingVolumeM3
            / Mathf.Max(0.0001f, record.originalVolumeM3)), 1f / 3f);
        float diameter = Mathf.Max(0.1f, record.originalDiameterCm / 100f);
        model.transform.localScale = new Vector3(
            Mathf.Max(1f, record.originalHeightMeters * 0.8f) / Mathf.Max(0.01f, naturalSize.x),
            diameter / Mathf.Max(0.01f, naturalSize.y),
            diameter / Mathf.Max(0.01f, naturalSize.z)) * decayScale;
        // Tint multiplies the authored textures rather than replacing them.
        var block = new MaterialPropertyBlock();
        Color tint = Color.Lerp(Color.white, new Color(0.65f, 0.78f, 0.58f), record.DecayClass / 5f);
        block.SetColor("_BaseColor", tint);
        foreach (Renderer renderer in model.GetComponentsInChildren<Renderer>(true))
            renderer.SetPropertyBlock(block);
    }
}
