using System.Collections.Generic;
using UnityEngine;

// Functional presentation of individual deer shelters (Scenario 1 ecology
// completion). Derived entirely from ForestEcologyController.Browsing.Shelters:
// no state of its own, nothing saved, no scene/prefab edits. A shelter is drawn
// once it is installed for the upcoming annual step; effective shelters stand
// upright, expired ones are grey and failed ones lean over.
public sealed class ScenarioProtectionVisuals : MonoBehaviour
{
    public const float TubeHeightM = 1.2f;
    public const float TubeRadiusM = 0.1f;

    private static readonly Color EffectiveColor = new Color(0.62f, 0.78f, 0.52f);
    private static readonly Color ExpiredColor = new Color(0.55f, 0.52f, 0.48f);
    private static readonly Color FailedColor = new Color(0.45f, 0.38f, 0.33f);

    private ForestEcologyController ecology;
    private readonly List<GameObject> tubes = new List<GameObject>();
    private long signature = long.MinValue;
    private Material effectiveMaterial, expiredMaterial, failedMaterial;

    public int VisualCount { get; private set; }
    public int EffectiveVisualCount { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        if (FindFirstObjectByType<ForestEcologyController>() == null || FindFirstObjectByType<ScenarioProtectionVisuals>() != null)
            return;
        new GameObject("Scenario protection visuals").AddComponent<ScenarioProtectionVisuals>();
    }

    private void LateUpdate()
    {
        if (ecology == null)
            ecology = FindFirstObjectByType<ForestEcologyController>();
        if (ecology == null)
            return;
        long current = Signature(ecology);
        if (current != signature)
            RefreshNow();
    }

    // Rebuild immediately from the authoritative records (tests call this directly).
    public void RefreshNow()
    {
        if (ecology == null)
            ecology = FindFirstObjectByType<ForestEcologyController>();
        foreach (GameObject tube in tubes)
            if (tube != null)
            {
                // Destroy completes at frame end; hide now so counts are exact.
                tube.SetActive(false);
                Destroy(tube);
            }
        tubes.Clear();
        VisualCount = 0;
        EffectiveVisualCount = 0;
        if (ecology == null)
            return;
        signature = Signature(ecology);
        int upcoming = ecology.EcologicalYear + 1;
        foreach (BrowseShelter shelter in ecology.Browsing.Shelters)
        {
            if (shelter == null || upcoming < shelter.installedYear)
                continue;
            bool failed = shelter.failedYear >= 0 && upcoming >= shelter.failedYear;
            bool effective = shelter.IsEffective(upcoming);
            tubes.Add(CreateTube(shelter, effective ? 0 : failed ? 2 : 1));
            VisualCount++;
            if (effective)
                EffectiveVisualCount++;
        }
    }

    private GameObject CreateTube(BrowseShelter shelter, int state)
    {
        GameObject tube = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        tube.name = "Shelter " + shelter.shelterId;
        Collider collider = tube.GetComponent<Collider>();
        if (collider != null)
            Destroy(collider);
        tube.transform.SetParent(transform, false);
        float ground = Terrain.activeTerrain != null
            ? Terrain.activeTerrain.SampleHeight(new Vector3(shelter.position.x, 0f, shelter.position.y)) + Terrain.activeTerrain.transform.position.y
            : 0f;
        // Unity's cylinder is 2 units tall and 1 unit across.
        tube.transform.localScale = new Vector3(TubeRadiusM * 2f, TubeHeightM * 0.5f, TubeRadiusM * 2f);
        tube.transform.position = new Vector3(shelter.position.x, ground + TubeHeightM * 0.5f, shelter.position.y);
        if (state == 2)
        {
            tube.transform.rotation = Quaternion.Euler(0f, 0f, 28f);
            tube.transform.position += new Vector3(0.25f, -0.1f, 0f);
        }
        Renderer renderer = tube.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.sharedMaterial = state == 0 ? Material(ref effectiveMaterial, EffectiveColor)
                : state == 1 ? Material(ref expiredMaterial, ExpiredColor) : Material(ref failedMaterial, FailedColor);
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }
        return tube;
    }

    private static Material Material(ref Material cached, Color color)
    {
        if (cached != null)
            return cached;
        Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Universal Render Pipeline/Simple Lit");
        cached = shader != null ? new Material(shader) : new Material(Shader.Find("Hidden/InternalErrorShader"));
        cached.color = color;
        if (cached.HasProperty("_BaseColor"))
            cached.SetColor("_BaseColor", color);
        return cached;
    }

    // Cheap change detector over the authoritative records and the year.
    private static long Signature(ForestEcologyController ecology)
    {
        unchecked
        {
            long hash = 1469598103934665603L ^ ecology.EcologicalYear;
            foreach (BrowseShelter shelter in ecology.Browsing.Shelters)
            {
                if (shelter == null) continue;
                hash = (hash ^ shelter.installedYear) * 1099511628211L;
                hash = (hash ^ shelter.effectiveYears) * 1099511628211L;
                hash = (hash ^ shelter.failedYear) * 1099511628211L;
                hash = (hash ^ shelter.position.GetHashCode()) * 1099511628211L;
            }
            return (hash ^ ecology.Browsing.Shelters.Count) * 1099511628211L;
        }
    }

    private void OnDestroy()
    {
        foreach (Material material in new[] { effectiveMaterial, expiredMaterial, failedMaterial })
            if (material != null)
                Destroy(material);
    }
}
