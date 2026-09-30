using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

// This palette is a read-only interpretation of saved functional groups, not
// eight new populations. It is deliberately silent about unverified soil,
// drainage and connection to older woodland at the Scenario One site.
public enum HabitatVisualClass
{
    MossCarpet, ShadeFern, BrackenType, Grass, BrambleType,
    DwarfShrubType, GenericHerbs, DeadwoodFungi
}

public static class ScenarioHabitatPalette
{
    public static float Strength(HabitatVisualClass kind, ScenarioUnderstoreyCell plants,
        ForestEcologyCell habitat, float oldWoodlandSourceConfidence = 0f)
    {
        if (plants == null || habitat == null) return 0f;
        float light = Mathf.Clamp01(habitat.Light);
        switch (kind)
        {
            case HabitatVisualClass.MossCarpet: return Mathf.Clamp01(plants.mosses);
            case HabitatVisualClass.ShadeFern:
                return Mathf.Clamp01(plants.ferns * (1f - Mathf.Clamp01((light - 0.3f) / 0.5f)));
            case HabitatVisualClass.BrackenType:
                // Native bracken-type gap response, not an invasive-alien flag
                // or an independent competition rule.
                return Mathf.Clamp01((plants.ferns + plants.grasses * 0.5f)
                    * Mathf.Clamp01((light - 0.28f) / 0.5f));
            case HabitatVisualClass.Grass: return Mathf.Clamp01(plants.grasses);
            case HabitatVisualClass.BrambleType:
                return Mathf.Clamp01(plants.shrubs * Mathf.Clamp01((light - 0.12f) / 0.5f));
            case HabitatVisualClass.DwarfShrubType:
                return Mathf.Clamp01(plants.shrubs * (1f - light) * 0.6f);
            case HabitatVisualClass.GenericHerbs:
                return Mathf.Clamp01(plants.forbs * (0.45f + 0.2f * Mathf.Clamp01(oldWoodlandSourceConfidence)));
            default: return 0f; // deadwood fungi attach to observed fallen logs
        }
    }

    // Suitable-looking habitat is insufficient evidence for slow, ancient-
    // woodland specialists. Zero by default; this never changes Forestry.
    public static float SpecialistHerbSignal(ScenarioUnderstoreyCell plants, float sourceConfidence)
    {
        return plants == null ? 0f : Mathf.Clamp01(plants.forbs) * Mathf.Clamp01(sourceConfidence);
    }
}

// Habitat presentation layer. Delivered assets (bilberry, herbs, ground moss,
// deadwood fungi) replace procedural placeholders; remaining classes use
// lightweight meshes. Rebuilt from current cells and log records on load/annual
// resolution; nothing here enters a world save.
[DisallowMultipleComponent]
public sealed class ScenarioHabitatVisuals : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)] private float verifiedOldWoodlandSourceConfidence;
    private const int ClassCount = 8;
    private readonly Mesh[] meshes = new Mesh[ClassCount];
    private readonly Material[] materials = new Material[ClassCount];
    private readonly int[] authoredVertices = new int[ClassCount];
    private Transform authoredRoot;
    private SectionFiveVisualCatalog catalog;
    private RecentAssetVisualCatalog recentCatalog;
    public int LitterPatchCount { get; private set; }
    public int SmallDeadwoodPatchCount { get; private set; }
    public int JuvenileVisualCount { get; private set; }
    public int GrassPatchCount { get; private set; }
    public int RushPatchCount { get; private set; }
    private static readonly Color[] Colors =
    {
        new Color(0.17f, 0.34f, 0.17f), new Color(0.22f, 0.42f, 0.22f),
        new Color(0.40f, 0.48f, 0.17f), new Color(0.41f, 0.55f, 0.26f),
        new Color(0.22f, 0.36f, 0.16f), new Color(0.17f, 0.28f, 0.23f),
        new Color(0.57f, 0.59f, 0.36f), new Color(0.67f, 0.57f, 0.40f)
    };

    public int VisibleVertexCount(HabitatVisualClass kind) => (meshes[(int)kind]?.vertexCount ?? 0)
        + authoredVertices[(int)kind];
    public float OldWoodlandSourceConfidence => verifiedOldWoodlandSourceConfidence;

    public void Rebuild(ForestEcologyController ecology, IReadOnlyList<ScenarioUnderstoreyCell> understorey,
        IReadOnlyList<ScenarioDeadwoodRecord> deadwood, IReadOnlyList<PlantedJuvenile> planted = null)
    {
        if (!Application.isPlaying || ecology?.Cells == null || understorey == null)
            return;
        if (catalog == null) catalog = SectionFiveVisualCatalog.Load();
        if (recentCatalog == null) recentCatalog = RecentAssetVisualCatalog.Load();
        if (authoredRoot != null) { authoredRoot.gameObject.SetActive(false); Destroy(authoredRoot.gameObject); }
        authoredRoot = new GameObject("Authored forest floor").transform;
        authoredRoot.SetParent(transform, false);
        Array.Clear(authoredVertices, 0, authoredVertices.Length);
        LitterPatchCount = 0;
        SmallDeadwoodPatchCount = 0;
        JuvenileVisualCount = 0;
        GrassPatchCount = 0;
        RushPatchCount = 0;
        Shader shader = Shader.Find("Universal Render Pipeline/Unlit") ?? Shader.Find("Unlit/Color");
        if (shader == null) return;
        var vertices = new List<Vector3>[ClassCount];
        var triangles = new List<int>[ClassCount];
        for (int type = 0; type < ClassCount; type++)
        {
            vertices[type] = new List<Vector3>();
            triangles[type] = new List<int>();
            EnsureMesh(type, shader);
        }

        for (int i = 0; i < ecology.CellCount && i < understorey.Count; i++)
        {
            ForestEcologyCell cell = ecology.Cells[i];
            ScenarioUnderstoreyCell state = understorey[i];
            if (cell == null || state == null || state.cellIndex != i) continue;
            for (int type = 0; type < ClassCount - 1; type++)
            {
                float strength = ScenarioHabitatPalette.Strength((HabitatVisualClass)type,
                    state, cell, verifiedOldWoodlandSourceConfidence);
                int count = strength < 0.025f ? 0 : Mathf.Max(1, Mathf.RoundToInt(strength * 4f));
                for (int plant = 0; plant < count; plant++)
                {
                    float x = cell.Center.x + Jitter(i, type, plant, 0) * ecology.CellSizeMeters * 0.42f;
                    float z = cell.Center.y + Jitter(i, type, plant, 1) * ecology.CellSizeMeters * 0.42f;
                    float angle = Jitter(i, type, plant, 2) * Mathf.PI;
                    float size = 0.7f + strength * 0.6f;
                    Vector3 position = new Vector3(x, 0f, z);
                    GameObject prefab = PrefabFor((HabitatVisualClass)type, i, plant);
                    if (prefab != null)
                    {
                        authoredVertices[type] += Place(prefab, position, angle * Mathf.Rad2Deg, size);
                        if (type == (int)HabitatVisualClass.Grass) GrassPatchCount++;
                    }
                    else
                        AddPlant(vertices[type], triangles[type], position,
                            (HabitatVisualClass)type, angle, size);
                }
            }
        }

        if (deadwood != null)
            foreach (ScenarioDeadwoodRecord log in deadwood)
            {
                if (log == null || log.remainingVolumeM3 <= 0f || log.DecayClass < 2) continue;
                int fungiCount = Mathf.Min(4, log.DecayClass);
                for (int i = 0; i < fungiCount; i++)
                {
                    float angle = i * Mathf.PI * 2f / fungiCount;
                    Vector3 place = log.worldPosition + new Vector3(Mathf.Cos(angle), 0f,
                        Mathf.Sin(angle)) * Mathf.Max(0.18f, log.originalDiameterCm / 150f);
                    GameObject fungiPrefab = (i % 2 == 0 ? recentCatalog?.deadwoodMushroom
                        : recentCatalog?.deadwoodBracket) ?? recentCatalog?.deadwoodMushroom
                        ?? recentCatalog?.deadwoodBracket;
                    if (fungiPrefab != null)
                        authoredVertices[7] += Place(fungiPrefab, place, angle * Mathf.Rad2Deg,
                            0.7f + 0.1f * log.DecayClass);
                    else
                        AddPlant(vertices[7], triangles[7], place, HabitatVisualClass.DeadwoodFungi,
                            angle, 0.7f + 0.1f * log.DecayClass);
                }
                if (catalog != null)
                {
                    Place(catalog.mossOnWood, log.worldPosition + Vector3.right * 0.5f, 0f, 0.6f);
                    if (catalog.smallDeadwood != null && catalog.smallDeadwood.Length > 0)
                    {
                        int variant = Mathf.Abs(log.cellIndex) % catalog.smallDeadwood.Length;
                        Place(catalog.smallDeadwood[variant], log.worldPosition + Vector3.forward * 0.6f, 33f * variant, 0.8f);
                        SmallDeadwoodPatchCount++;
                    }
                }
            }
        PlaceLitter(ecology);
        PlaceJuveniles(planted);
        PlaceRushDressing(ecology);
        for (int i = 0; i < ClassCount; i++)
        {
            Mesh mesh = meshes[i];
            mesh.Clear();
            mesh.SetVertices(vertices[i]);
            mesh.SetTriangles(triangles[i], 0);
            mesh.RecalculateBounds();
        }
    }

    private GameObject PrefabFor(HabitatVisualClass kind, int cell, int plant)
    {
        if (recentCatalog == null) recentCatalog = RecentAssetVisualCatalog.Load();
        if (kind == HabitatVisualClass.Grass && recentCatalog?.grasses != null && recentCatalog.grasses.Length > 0)
            return recentCatalog.grasses[(cell + plant) % recentCatalog.grasses.Length];
        if (kind == HabitatVisualClass.MossCarpet && recentCatalog?.groundMoss != null)
            return recentCatalog.groundMoss;
        if (kind == HabitatVisualClass.DwarfShrubType && recentCatalog?.bilberryCover != null)
            return recentCatalog.bilberryCover;
        if (kind == HabitatVisualClass.GenericHerbs && recentCatalog != null)
        {
            GameObject[] herbs = { recentCatalog.herbRosette, recentCatalog.herbFlowering };
            GameObject pick = herbs[(cell + plant) % 2];
            return pick ?? herbs[0] ?? herbs[1];
        }
        if (catalog == null) return null;
        switch (kind)
        {
            case HabitatVisualClass.ShadeFern: return catalog.shadeFern;
            case HabitatVisualClass.BrackenType: return catalog.bracken;
            case HabitatVisualClass.BrambleType: return catalog.bramble;
            default: return null;
        }
    }

    private void PlaceRushDressing(ForestEcologyController ecology)
    {
        if (recentCatalog?.rushes == null || recentCatalog.rushes.Length == 0
            || recentCatalog.rushDressingPositions == null) return;
        // These are authored scenery accents. Distribution is not inferred
        // from light, understorey diversity or unmodelled drainage/soil data.
        for (int patch = 0; patch < recentCatalog.rushDressingPositions.Length; patch++)
        {
            Vector3 anchor = recentCatalog.rushDressingPositions[patch];
            if (ecology.GetCellIndex(anchor) < 0) continue;
            for (int clump = 0; clump < 2; clump++)
            {
                GameObject prefab = recentCatalog.rushes[(patch + clump) % recentCatalog.rushes.Length];
                if (prefab == null) continue;
                Place(prefab, anchor + new Vector3(clump * 0.5f, 0f, clump * 0.35f),
                    patch * 47f + clump * 79f, clump == 0 ? 1f : 0.85f);
                RushPatchCount++;
            }
        }
    }

    private int Place(GameObject prefab, Vector3 position, float yaw, float scale)
    {
        if (prefab == null) return 0;
        GameObject instance = Instantiate(prefab, authoredRoot);
        instance.transform.position = position + Vector3.up * 0.012f;
        instance.transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        instance.transform.localScale = Vector3.one * scale;
        int vertexCount = 0;
        foreach (MeshFilter filter in instance.GetComponentsInChildren<MeshFilter>(true))
            if (filter.sharedMesh != null) vertexCount += filter.sharedMesh.vertexCount;
        return vertexCount;
    }

    private void PlaceLitter(ForestEcologyController ecology)
    {
        if (catalog == null) return;
        ForestTree[] trees = UnityEngine.Object.FindObjectsByType<ForestTree>(FindObjectsSortMode.None);
        // Litter is an appearance under observed broadleaf crowns, not a
        // species colonisation or nutrient-feedback model. No Oak/Beech means
        // no broadleaf patches at the Year-0 Sitka start.
        for (int cellIndex = 0; cellIndex < ecology.CellCount; cellIndex++)
        {
            Vector2 center = ecology.Cells[cellIndex].Center;
            bool oak = false, beech = false;
            foreach (ForestTree tree in trees)
            {
                if (tree == null || tree.IsStump || tree.Species == null) continue;
                float distance = Vector2.Distance(center, new Vector2(tree.transform.position.x, tree.transform.position.z));
                if (distance > Mathf.Max(1.5f, tree.CrownRadius + 1f)) continue;
                oak |= tree.Species.SpeciesId == "sessile-oak";
                beech |= tree.Species.SpeciesId == "beech";
            }
            if (!oak && !beech) continue;
            GameObject prefab = oak && beech ? catalog.mixedLitter : oak ? catalog.oakLitter : catalog.beechLitter;
            if (prefab == null) continue;
            Vector3 position = new Vector3(center.x + Jitter(cellIndex, 8, 0, 0) * 0.4f, 0f,
                center.y + Jitter(cellIndex, 8, 0, 1) * 0.4f);
            Place(prefab, position, Jitter(cellIndex, 8, 0, 2) * 180f, 0.9f);
            LitterPatchCount++;
        }
    }

    private void PlaceJuveniles(IReadOnlyList<PlantedJuvenile> planted)
    {
        if (catalog == null || planted == null) return;
        ForestTreeSpawner spawner = UnityEngine.Object.FindFirstObjectByType<ForestTreeSpawner>();
        foreach (PlantedJuvenile juvenile in planted)
        {
            if (!juvenile.alive || juvenile.legacyCohortManaged || !string.IsNullOrEmpty(juvenile.promotedTreeId)) continue;
            GameObject prefab = juvenile.speciesId == "beech" ? catalog.beechSapling
                : spawner?.GetSeedlingVisualPrefab(spawner.ResolveSpecies(juvenile.speciesId));
            if (prefab == null) continue;
            GameObject instance = Instantiate(prefab, authoredRoot);
            instance.name = "Planted juvenile " + juvenile.juvenileId;
            instance.transform.position = juvenile.position;
            float min = float.PositiveInfinity, max = float.NegativeInfinity;
            foreach (Renderer renderer in instance.GetComponentsInChildren<Renderer>(true))
            {
                min = Mathf.Min(min, renderer.bounds.min.y);
                max = Mathf.Max(max, renderer.bounds.max.y);
            }
            if (max > min)
                instance.transform.localScale = Vector3.one * juvenile.heightMeters / (max - min);
            JuvenileVisualCount++;
        }
    }

    private void EnsureMesh(int index, Shader shader)
    {
        if (meshes[index] != null) return;
        var child = new GameObject("Habitat view: " + (HabitatVisualClass)index);
        child.transform.SetParent(transform, false);
        var filter = child.AddComponent<MeshFilter>();
        Mesh mesh = new Mesh { name = child.name };
        meshes[index] = mesh;
        filter.sharedMesh = mesh;
        var renderer = child.AddComponent<MeshRenderer>();
        materials[index] = new Material(shader) { name = child.name, color = Colors[index] };
        renderer.sharedMaterial = materials[index];
        renderer.shadowCastingMode = ShadowCastingMode.Off;
        renderer.receiveShadows = false;
    }

    private static float Jitter(int cell, int kind, int plant, int axis)
    {
        uint value = unchecked((uint)(cell * 73856093 ^ kind * 19349663 ^ plant * 83492791 ^ axis * 265443576));
        value ^= value >> 16;
        value *= 0x7feb352du;
        value ^= value >> 15;
        return (value & 65535u) / 32767.5f - 1f;
    }

    private static void AddPlant(List<Vector3> points, List<int> faces, Vector3 center,
        HabitatVisualClass kind, float rotation, float size)
    {
        float height = kind == HabitatVisualClass.MossCarpet ? 0.015f
            : kind == HabitatVisualClass.BrackenType ? 0.9f * size
            : kind == HabitatVisualClass.BrambleType ? 0.55f * size
            : kind == HabitatVisualClass.DeadwoodFungi ? 0.24f * size
            : 0.35f * size;
        float spread = kind == HabitatVisualClass.MossCarpet ? 0.5f * size : 0.18f * size;
        int fronds = kind == HabitatVisualClass.MossCarpet ? 1 : 4;
        for (int i = 0; i < fronds; i++)
        {
            float angle = rotation + i * Mathf.PI * 2f / fronds;
            var outward = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
            var sideways = new Vector3(-outward.z, 0f, outward.x);
            Vector3 basePoint = center + outward * (kind == HabitatVisualClass.MossCarpet ? spread : 0f);
            Vector3 tip = center + outward * spread * 2f + Vector3.up * height;
            if (kind == HabitatVisualClass.MossCarpet)
            {
                Vector3 a = center - outward * spread - sideways * spread;
                Vector3 b = center - outward * spread + sideways * spread;
                Vector3 c = center + outward * spread + sideways * spread;
                Vector3 d = center + outward * spread - sideways * spread;
                Quad(points, faces, a + Vector3.up * height, b + Vector3.up * height,
                    c + Vector3.up * height, d + Vector3.up * height);
            }
            else
                Quad(points, faces, basePoint - sideways * spread * 0.3f,
                    center + Vector3.up * height * 0.55f - sideways * spread,
                    tip, center + Vector3.up * height * 0.55f + sideways * spread);
        }
    }

    private static void Quad(List<Vector3> points, List<int> faces,
        Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
        int start = points.Count;
        points.Add(a); points.Add(b); points.Add(c); points.Add(d);
        faces.Add(start); faces.Add(start + 1); faces.Add(start + 2);
        faces.Add(start); faces.Add(start + 2); faces.Add(start + 3);
        faces.Add(start + 2); faces.Add(start + 1); faces.Add(start);
        faces.Add(start + 3); faces.Add(start + 2); faces.Add(start);
    }

    private void OnDestroy()
    {
        foreach (Mesh mesh in meshes) if (mesh != null) Destroy(mesh);
        foreach (Material material in materials) if (material != null) Destroy(material);
    }
}
