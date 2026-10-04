using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

// One core plus retained whole branch systems. Combined meshes are shared by
// instances with the same retained systems; no render-mesh physics colliders.
public sealed class PlantationTreeVisual : MonoBehaviour
{
    public PlantationVisualCatalog catalog;
    public int baseIndex;
    private sealed class Assembly
    {
        public Mesh[][] meshes;
        public int users;
    }
    private static readonly Dictionary<string, Assembly> Cache = new Dictionary<string, Assembly>();
    private Assembly assembly;
    private string assemblyKey;
    private LODGroup group;
    private readonly List<PlantationWorldCapsule> worldContacts = new List<PlantationWorldCapsule>();
    private bool[] retained;
    private float lastScale = -1f;
    private Matrix4x4 lastMatrix;
    private bool lastUnpruned;
    public ForestTree Tree { get; private set; }
    public PlantationBase Base => catalog.bases[baseIndex];
    public IReadOnlyList<PlantationWorldCapsule> Contacts => worldContacts;
    public bool IsUnprunedYoung => Tree != null && !Tree.IsStump && Tree.PruningLifts == 0 && Tree.Height < 20f;
    public int RetainedBranchCount { get; private set; }
    public Bounds ContactBounds { get; private set; }
    public static int CachedAssemblyCount => Cache.Count;
    public bool IsRetained(int branch) => retained != null && retained[branch];

    public void Apply(ForestTree tree, float height, bool healed)
    {
        Tree = tree;
        float scale = height / Base.authoredHeight;
        transform.localScale = Vector3.one * scale;
        var mask = new char[Base.branches.Length];
        if (retained == null || retained.Length != mask.Length) retained = new bool[mask.Length];
        RetainedBranchCount = 0;
        float groundY = tree.transform.position.y;
        for (int i = 0; i < mask.Length; i++)
        {
            // Bounds include every render LOD; remove whole wood/foliage
            // systems that extend below the recorded world clearance.
            bool keep = tree.PruningLifts == 0
                || MinimumWorldY(Base.branches[i].bounds, transform.localToWorldMatrix) - groundY
                    >= tree.CrownBaseHeightM - 0.0001f;
            retained[i] = keep;
            mask[i] = keep ? '1' : '0';
            if (keep) RetainedBranchCount++;
        }
        string key = catalog.GetEntityId() + ":" + baseIndex + ":" + new string(mask)
            + (RetainedBranchCount == mask.Length ? "" : healed ? ":healed" : ":recent");
        bool changed = key != assemblyKey;
        if (changed)
        {
            ReleaseAssembly();
            if (!Cache.TryGetValue(key, out assembly))
            {
                assembly = BuildAssembly(healed);
                Cache.Add(key, assembly);
            }
            assembly.users++;
            assemblyKey = key;
            BindRenderers();
        }
        Matrix4x4 matrix = transform.localToWorldMatrix;
        if (changed || lastScale != scale || matrix != lastMatrix || lastUnpruned != IsUnprunedYoung)
        {
            RebuildContacts(matrix);
            lastScale = scale;
            lastMatrix = matrix;
            lastUnpruned = IsUnprunedYoung;
        }
    }

    public static float MinimumWorldY(Bounds bounds, Matrix4x4 matrix)
    {
        float minimum = float.PositiveInfinity;
        for (int i = 0; i < 8; i++)
        {
            Vector3 p = bounds.center + Vector3.Scale(bounds.extents,
                new Vector3((i & 1) == 0 ? -1 : 1, (i & 2) == 0 ? -1 : 1, (i & 4) == 0 ? -1 : 1));
            minimum = Mathf.Min(minimum, matrix.MultiplyPoint3x4(p).y);
        }
        return minimum;
    }

    private Assembly BuildAssembly(bool healed)
    {
        var result = new Assembly { meshes = new Mesh[3][] };
        for (int lod = 0; lod < 3; lod++)
        {
            var lists = new List<CombineInstance>[catalog.materials.Length];
            for (int m = 0; m < lists.Length; m++) lists[m] = new List<CombineInstance>();
            void Add(PlantationLod source, Matrix4x4 matrix)
            {
                foreach (PlantationPiece piece in source.pieces)
                    for (int sub = 0; sub < piece.materialSlots.Length; sub++)
                        lists[piece.materialSlots[sub]].Add(new CombineInstance
                        { mesh = piece.mesh, subMeshIndex = sub, transform = matrix });
            }
            Add(Base.core[lod], Matrix4x4.identity);
            for (int branch = 0; branch < retained.Length; branch++)
                if (retained[branch]) Add(Base.branches[branch].lods[lod], Matrix4x4.identity);
                else Add((healed ? catalog.healedScars : catalog.recentScars)[lod],
                    Base.branches[branch].scarTransforms[lod]);
            result.meshes[lod] = new Mesh[lists.Length];
            for (int m = 0; m < lists.Length; m++)
            {
                if (lists[m].Count == 0) continue;
                var mesh = new Mesh { name = Base.id + " assembled LOD" + lod,
                    indexFormat = IndexFormat.UInt32 };
                mesh.CombineMeshes(lists[m].ToArray(), true, true, false);
                mesh.RecalculateBounds();
                result.meshes[lod][m] = mesh;
            }
        }
        return result;
    }

    private void BindRenderers()
    {
        if (group == null)
        {
            group = gameObject.GetComponent<LODGroup>();
            if (group == null) group = gameObject.AddComponent<LODGroup>();
        }
        var lods = new LOD[3];
        for (int lod = 0; lod < 3; lod++)
        {
            var renderers = new List<Renderer>();
            for (int m = 0; m < catalog.materials.Length; m++)
            {
                string name = "LOD" + lod + " Material" + m;
                Transform child = transform.Find(name);
                if (child == null)
                {
                    var obj = new GameObject(name);
                    obj.transform.SetParent(transform, false);
                    obj.AddComponent<MeshFilter>();
                    obj.AddComponent<MeshRenderer>();
                    child = obj.transform;
                }
                Mesh mesh = assembly.meshes[lod][m];
                child.GetComponent<MeshFilter>().sharedMesh = mesh;
                var renderer = child.GetComponent<MeshRenderer>();
                renderer.sharedMaterial = catalog.materials[m];
                child.gameObject.SetActive(mesh != null);
                if (mesh != null) renderers.Add(renderer);
            }
            lods[lod] = new LOD(new[] { 0.55f, 0.20f, 0.025f }[lod], renderers.ToArray());
        }
        group.fadeMode = LODFadeMode.None;
        group.SetLODs(lods);
        group.RecalculateBounds();
    }

    private void RebuildContacts(Matrix4x4 matrix)
    {
        PlantationBranchMovement.Unregister(this);
        worldContacts.Clear();
        float radiusScale = Mathf.Max(matrix.GetColumn(0).magnitude,
            Mathf.Max(matrix.GetColumn(1).magnitude, matrix.GetColumn(2).magnitude));
        bool hasBounds = false;
        for (int branch = 0; branch < retained.Length; branch++)
        {
            if (!retained[branch]) continue;
            foreach (PlantationCapsule capsule in Base.branches[branch].contacts)
            {
                var world = new PlantationWorldCapsule
                {
                    start = matrix.MultiplyPoint3x4(capsule.start), end = matrix.MultiplyPoint3x4(capsule.end),
                    radius = capsule.radius * radiusScale, flexible = capsule.flexible, branch = branch
                };
                // Only body-height contact is indexed; upper crowns cannot
                // affect ground walking. Queries still check the actual body.
                if (Mathf.Min(world.start.y, world.end.y) - world.radius > Tree.transform.position.y + 3f) continue;
                worldContacts.Add(world);
                Bounds bounds = new Bounds(world.start, Vector3.one * world.radius * 2f);
                bounds.Encapsulate(new Bounds(world.end, Vector3.one * world.radius * 2f));
                if (!hasBounds) { ContactBounds = bounds; hasBounds = true; }
                else { Bounds total = ContactBounds; total.Encapsulate(bounds); ContactBounds = total; }
            }
        }
        if (isActiveAndEnabled && worldContacts.Count > 0) PlantationBranchMovement.Register(this);
    }

    private void OnDisable() { PlantationBranchMovement.Unregister(this); }
    private void OnEnable()
    {
        if (worldContacts.Count > 0) PlantationBranchMovement.Register(this);
    }
    private void OnDestroy() { PlantationBranchMovement.Unregister(this); ReleaseAssembly(); }
    private void ReleaseAssembly()
    {
        if (assembly == null) return;
        if (--assembly.users == 0)
        {
            foreach (Mesh[] lod in assembly.meshes)
                foreach (Mesh mesh in lod)
                    if (mesh != null)
                    {
                        if (Application.isPlaying) Destroy(mesh); else DestroyImmediate(mesh);
                    }
            Cache.Remove(assemblyKey);
        }
        assembly = null;
        assemblyKey = null;
    }
}

public struct PlantationWorldCapsule
{
    public Vector3 start, end;
    public float radius;
    public bool flexible;
    public int branch;
}
