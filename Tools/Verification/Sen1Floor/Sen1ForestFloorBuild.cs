// Sen-1 #836 forest-floor pass — one-off authoring + verification tooling.
//
// This file is NOT game code. It is copied into Assets/ForestPrototype/Editor/
// for a single batchmode Editor session, run through -executeMethod, and then
// removed together with its generated .meta (Tools/Verification workflow).
//
// Presentation only: it swaps the ground material reference in
// ScenarioOneCompact30, adds two opaque groundcover meshes with no colliders,
// and never touches ecology, saves, world manifests, planting masks or bounds.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public static class Sen1ForestFloorBuild
{
    public const string ScenePath =
        "Assets/ForestPrototype/Scenes/ScenarioOneCompact30.unity";
    public const string ArtRoot = "Assets/ForestPrototype/Art/Sen1SpruceFloor";
    public const string TextureRoot = ArtRoot + "/Textures";
    public const string MaterialRoot = ArtRoot + "/Materials";
    public const string MeshRoot = ArtRoot + "/Meshes";

    public const string OldGroundMaterial =
        "Assets/ForestPrototype/Materials/Forest Ground.mat";
    public const string FloorMaterialPath = MaterialRoot + "/Sen1_SpruceFloor.mat";
    public const string MossMaterialPath = MaterialRoot + "/Sen1_FloorMoss.mat";
    public const string TwigMaterialPath = MaterialRoot + "/Sen1_FloorTwig.mat";
    public const string MarginMaterialPath = MaterialRoot + "/Sen1_FloorMarginMoss.mat";
    public const string FarMaterialPath = MaterialRoot + "/Sen1_SpruceFloorFar.mat";
    public const string ScatterRootName = "Sen1_SpruceFloorScatter";

    public const string CaptureRoot =
        "/media/jer/Files/CCF/Verification/Sen1-836-Floor-20261119/captures";

    const float TileMetres = 4.0f; // albedo tile size
    const float BumpTileMetres = 1.6f; // normal-map tile size

    // ------------------------------------------------------------ entry points

    public static void Run()
    {
        try
        {
            ImportTextures();
            BuildMaterials();
            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            var context = new SceneContext(scene);
            BuildScatter(context);
            WireScene(context);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            Log(StructuralReport(context));
            Debug.Log("SEN1_FLOOR_SETUP_PASS");
            EditorApplication.Exit(0);
        }
        catch (Exception e)
        {
            Debug.LogError("SEN1_FLOOR_SETUP_FAIL " + e);
            EditorApplication.Exit(1);
        }
    }

    // Renders the scene exactly as saved, with no material swap, into the folder
    // named by SEN1_CAPTURE_DIR. Used for an honest before (base commit) and after
    // (candidate commit) pair from the same poses.
    public static void CaptureCurrent()
    {
        try
        {
            string dir = Environment.GetEnvironmentVariable("SEN1_CAPTURE_DIR");
            if (string.IsNullOrEmpty(dir))
                throw new InvalidOperationException("SEN1_CAPTURE_DIR not set");
            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            var context = new SceneContext(scene);
            Directory.CreateDirectory(dir);
            RenderAll(dir, context, true);
            Log(StructuralReport(context));
            Log("SEN1_FLOOR_CAPTURE_CURRENT_PASS " + dir);
            EditorApplication.Exit(0);
        }
        catch (Exception e)
        {
            Debug.LogError("SEN1_FLOOR_CAPTURE_CURRENT_FAIL " + e);
            EditorApplication.Exit(1);
        }
    }

    public static void VerifyOnly()
    {
        try
        {
            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            var context = new SceneContext(scene);
            Log(StructuralReport(context));
            Debug.Log("SEN1_FLOOR_VERIFY_PASS");
            EditorApplication.Exit(0);
        }
        catch (Exception e)
        {
            Debug.LogError("SEN1_FLOOR_VERIFY_FAIL " + e);
            EditorApplication.Exit(1);
        }
    }

    public static void Capture()
    {
        try
        {
            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            var context = new SceneContext(scene);
            Directory.CreateDirectory(CaptureRoot);
            string afterDir = Path.Combine(CaptureRoot, "after");
            string beforeDir = Path.Combine(CaptureRoot, "before");
            Directory.CreateDirectory(afterDir);
            Directory.CreateDirectory(beforeDir);

            RenderAll(afterDir, context, false);

            // Before state: put the old shared ground material back and hide the
            // new groundcover. Nothing is saved, so the scene asset is untouched.
            var oldGround = AssetDatabase.LoadAssetAtPath<Material>(OldGroundMaterial);
            var swapped = context.SwapGroundMaterialForBefore(oldGround);
            RenderAll(beforeDir, context, true);
            context.RestoreGroundMaterial(swapped);

            Log("SEN1_FLOOR_CAPTURE_PASS after=" + afterDir + " before=" + beforeDir);
            EditorApplication.Exit(0);
        }
        catch (Exception e)
        {
            Debug.LogError("SEN1_FLOOR_CAPTURE_FAIL " + e);
            EditorApplication.Exit(1);
        }
    }

    // ------------------------------------------------------------ textures

    static void ImportTextures()
    {
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        Configure(TextureRoot + "/Sen1_SpruceFloor_Base_2048.png", 2048, true,
            TextureImporterType.Default);
        Configure(TextureRoot + "/Sen1_SpruceFloor_Normal_1024.png", 1024, false,
            TextureImporterType.NormalMap);
        Configure(TextureRoot + "/Sen1_SpruceFloor_Macro_512.png", 512, false,
            TextureImporterType.Default, clamp: true);
        Configure(TextureRoot + "/Sen1_FloorMoss_512.png", 512, true,
            TextureImporterType.Default);
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
    }

    static void Configure(string path, int maxSize, bool sRGB,
        TextureImporterType type, bool clamp = false)
    {
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null)
            throw new InvalidOperationException("Texture importer missing for " + path);
        importer.textureType = type;
        importer.sRGBTexture = sRGB;
        importer.maxTextureSize = maxSize;
        importer.mipmapEnabled = true;
        importer.wrapMode = clamp ? TextureWrapMode.Clamp : TextureWrapMode.Repeat;
        importer.filterMode = FilterMode.Trilinear;
        importer.anisoLevel = 4;
        importer.alphaSource = TextureImporterAlphaSource.None;
        importer.textureCompression = TextureImporterCompression.CompressedHQ;
        importer.SaveAndReimport();
    }

    // ------------------------------------------------------------ materials

    static void BuildMaterials()
    {
        foreach (string dir in new[] { MaterialRoot, MeshRoot })
            Directory.CreateDirectory(dir);
        AssetDatabase.Refresh();

        var floor = LoadOrCreateMaterial(FloorMaterialPath);
        SetTex(floor, "_BaseMap", "Sen1_SpruceFloor_Base_2048", new Vector2(
            30f / TileMetres, 30f / TileMetres));
        SetTex(floor, "_BumpMap", "Sen1_SpruceFloor_Normal_1024", new Vector2(
            30f / BumpTileMetres, 30f / BumpTileMetres));
        SetTex(floor, "_OcclusionMap", "Sen1_SpruceFloor_Macro_512", Vector2.one);
        floor.SetColor("_BaseColor", new Color(1.00f, 0.99f, 0.96f, 1f));
        floor.SetFloat("_Metallic", 0f);
        floor.SetFloat("_Smoothness", 0.07f);
        floor.SetFloat("_BumpScale", 1.35f);
        floor.SetFloat("_OcclusionStrength", 0.70f);
        floor.EnableKeyword("_NORMALMAP");
        floor.EnableKeyword("_OCCLUSIONMAP");
        floor.enableInstancing = true;
        EditorUtility.SetDirty(floor);

        var moss = LoadOrCreateMaterial(MossMaterialPath);
        SetTex(moss, "_BaseMap", "Sen1_FloorMoss_512", new Vector2(1f, 1f));
        moss.SetColor("_BaseColor", new Color(0.88f, 0.96f, 0.82f, 1f));
        moss.SetFloat("_Metallic", 0f);
        moss.SetFloat("_Smoothness", 0.09f);
        EditorUtility.SetDirty(moss);

        var margin = LoadOrCreateMaterial(MarginMaterialPath);
        SetTex(margin, "_BaseMap", "Sen1_FloorMoss_512", new Vector2(2f, 2f));
        margin.SetColor("_BaseColor", new Color(0.84f, 0.90f, 0.80f, 1f));
        margin.SetFloat("_Metallic", 0f);
        margin.SetFloat("_Smoothness", 0.08f);
        EditorUtility.SetDirty(margin);

        var far = LoadOrCreateMaterial(FarMaterialPath);
        SetTex(far, "_BaseMap", "Sen1_SpruceFloor_Base_2048", new Vector2(65f, 65f));
        far.SetColor("_BaseColor", new Color(0.88f, 0.87f, 0.83f, 1f));
        far.SetFloat("_Metallic", 0f);
        far.SetFloat("_Smoothness", 0.07f);
        EditorUtility.SetDirty(far);

        var twig = LoadOrCreateMaterial(TwigMaterialPath);
        twig.SetTexture("_BaseMap", null);
        twig.SetColor("_BaseColor", new Color(0.38f, 0.32f, 0.24f, 1f));
        twig.SetFloat("_Metallic", 0f);
        twig.SetFloat("_Smoothness", 0.12f);
        EditorUtility.SetDirty(twig);

        AssetDatabase.SaveAssets();
    }

    static Material LoadOrCreateMaterial(string path)
    {
        var existing = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (existing != null) return existing;
        var shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) throw new InvalidOperationException("URP Lit shader missing");
        var material = new Material(shader) { name = Path.GetFileNameWithoutExtension(path) };
        AssetDatabase.CreateAsset(material, path);
        return material;
    }

    static void SetTex(Material material, string property, string textureName,
        Vector2 tiling)
    {
        string path = TextureRoot + "/" + textureName + ".png";
        var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        if (texture == null) throw new InvalidOperationException("Missing texture " + path);
        material.SetTexture(property, texture);
        material.SetTextureScale(property, tiling);
    }

    // ------------------------------------------------------------ scene context

    class SceneContext
    {
        public readonly UnityEngine.SceneManagement.Scene Scene;
        public readonly GameObject Ground;
        public readonly List<Renderer> LowCover = new List<Renderer>();
        public readonly GameObject TreesRoot;
        public readonly List<Vector2> Trunks = new List<Vector2>();
        public readonly List<Vector2> PathPoints = new List<Vector2>();
        public readonly List<Vector2> ClearingHull = new List<Vector2>();
        public readonly List<Vector2> G1 = new List<Vector2>();
        public readonly List<Collider> Containment = new List<Collider>();
        public Renderer OutsideGround;
        public GameObject ScatterRoot;

        public SceneContext(UnityEngine.SceneManagement.Scene scene)
        {
            Scene = scene;
            foreach (var root in scene.GetRootGameObjects())
            {
                if (root.name == "Ground") Ground = root;
                if (root.name == "Trees") TreesRoot = root;
            }
            if (Ground == null) Ground = FindByName(scene, "Ground");
            if (TreesRoot == null) TreesRoot = FindByName(scene, "Trees");
            if (Ground == null) throw new InvalidOperationException("Ground object missing");
            if (TreesRoot == null) throw new InvalidOperationException("Trees root missing");

            var all = new List<GameObject>();
            foreach (var root in scene.GetRootGameObjects())
                Collect(root.transform, all);

            foreach (var go in all)
            {
                if (go.name.StartsWith("LowCover_"))
                {
                    var r = go.GetComponent<Renderer>();
                    if (r != null) LowCover.Add(r);
                }
                if (go.name == "Outside_Ground_Continuation")
                    OutsideGround = go.GetComponent<Renderer>();
                if (go.name == "Containment")
                {
                    var collider = go.GetComponent<Collider>();
                    if (collider != null) Containment.Add(collider);
                }
                if (go.name.StartsWith("G1_"))
                    G1.Add(new Vector2(go.transform.position.x, go.transform.position.z));
                if (go.name == ScatterRootName) ScatterRoot = go;
            }

            // Direct children of the Trees root are the stand's tree prefab
            // instances; going deeper would collect branch transforms.
            foreach (Transform child in TreesRoot.transform)
            {
                var p = child.position;
                if (Mathf.Abs(p.x) < 15f && Mathf.Abs(p.z) < 15f)
                    Trunks.Add(new Vector2(p.x, p.z));
            }

            var ribbon = AssetDatabase.LoadAssetAtPath<Mesh>(
                "Assets/ForestPrototype/Art/Sen1PathFence/Meshes/Sen1_PathRibbon.asset");
            if (ribbon != null)
                foreach (var v in ribbon.vertices)
                    PathPoints.Add(new Vector2(v.x, v.z));

            var clearing = AssetDatabase.LoadAssetAtPath<Mesh>(
                "Assets/ForestPrototype/Art/Sen1PathFence/Meshes/Sen1_H1Clearing.asset");
            if (clearing != null)
            {
                var pts = new List<Vector2>();
                foreach (var v in clearing.vertices) pts.Add(new Vector2(v.x, v.z));
                ClearingHull = ConvexHull(pts);
            }
        }

        public Material GroundMaterial
        {
            get { return Ground.GetComponent<Renderer>().sharedMaterial; }
            set { Ground.GetComponent<Renderer>().sharedMaterial = value; }
        }

        public List<Material> SwapGroundMaterialForBefore(Material oldGround)
        {
            var previous = new List<Material> { GroundMaterial };
            GroundMaterial = oldGround;
            foreach (var r in LowCover)
            {
                previous.Add(r.sharedMaterial);
                r.sharedMaterial = oldGround;
            }
            if (OutsideGround != null)
            {
                previous.Add(OutsideGround.sharedMaterial);
                OutsideGround.sharedMaterial = oldGround;
            }
            if (ScatterRoot != null) ScatterRoot.SetActive(false);
            return previous;
        }

        public void RestoreGroundMaterial(List<Material> previous)
        {
            GroundMaterial = previous[0];
            for (int i = 0; i < LowCover.Count; i++)
                LowCover[i].sharedMaterial = previous[i + 1];
            if (OutsideGround != null)
                OutsideGround.sharedMaterial = previous[LowCover.Count + 1];
            if (ScatterRoot != null) ScatterRoot.SetActive(true);
        }
    }

    static GameObject FindByName(UnityEngine.SceneManagement.Scene scene, string name)
    {
        foreach (var root in scene.GetRootGameObjects())
        {
            if (root.name == name) return root;
            foreach (var t in root.GetComponentsInChildren<Transform>(true))
                if (t.name == name) return t.gameObject;
        }
        return null;
    }

    static void Collect(Transform t, List<GameObject> into)
    {
        into.Add(t.gameObject);
        for (int i = 0; i < t.childCount; i++) Collect(t.GetChild(i), into);
    }

    // ------------------------------------------------------------ placement rules

    static float DistanceToPath(Vector2 p)
    {
        float best = float.MaxValue;
        foreach (var q in Context.PathPoints) best = Mathf.Min(best, (q - p).sqrMagnitude);
        return Mathf.Sqrt(best);
    }

    static bool InsideClearing(Vector2 p, float margin)
    {
        var hull = Context.ClearingHull;
        if (hull.Count < 3) return false;
        bool inside = true;
        for (int i = 0; i < hull.Count; i++)
        {
            Vector2 a = hull[i], b = hull[(i + 1) % hull.Count];
            Vector2 e = b - a;
            Vector2 n = new Vector2(-e.y, e.x).normalized;
            if (Vector2.Dot(p - a, n) > 0f) { inside = false; break; }
        }
        if (inside) return true;
        for (int i = 0; i < hull.Count; i++)
            if (DistancePointSegment(p, hull[i], hull[(i + 1) % hull.Count]) < margin)
                return true;
        return false;
    }

    static float DistancePointSegment(Vector2 p, Vector2 a, Vector2 b)
    {
        Vector2 ab = b - a;
        float t = Mathf.Clamp01(Vector2.Dot(p - a, ab) / Mathf.Max(1e-6f, ab.sqrMagnitude));
        return (a + ab * t - p).magnitude;
    }

    static float DistanceToTrunk(Vector2 p)
    {
        float best = float.MaxValue;
        foreach (var t in Context.Trunks) best = Mathf.Min(best, (t - p).sqrMagnitude);
        return Mathf.Sqrt(best);
    }

    static float DistanceToG1(Vector2 p)
    {
        float best = float.MaxValue;
        foreach (var g in Context.G1) best = Mathf.Min(best, (g - p).sqrMagnitude);
        return Mathf.Sqrt(best);
    }

    static bool Allowed(Vector2 p, float pathClear, float trunkClear, float g1Clear)
    {
        if (Mathf.Abs(p.x) > 14.2f || Mathf.Abs(p.y) > 14.2f) return false;
        if (DistanceToPath(p) < pathClear) return false;
        if (InsideClearing(p, 0.45f)) return false;
        if (DistanceToTrunk(p) < trunkClear) return false;
        if (DistanceToG1(p) < g1Clear) return false;
        return true;
    }

    static List<Vector2> ConvexHull(List<Vector2> points)
    {
        points.Sort((a, b) => a.x == b.x ? a.y.CompareTo(b.y) : a.x.CompareTo(b.x));
        var hull = new List<Vector2>();
        foreach (var p in points)
        {
            while (hull.Count >= 2 && Cross(hull[hull.Count - 2], hull[hull.Count - 1], p) <= 0)
                hull.RemoveAt(hull.Count - 1);
            hull.Add(p);
        }
        int lower = hull.Count + 1;
        for (int i = points.Count - 2; i >= 0; i--)
        {
            var p = points[i];
            while (hull.Count >= lower && Cross(hull[hull.Count - 2], hull[hull.Count - 1], p) <= 0)
                hull.RemoveAt(hull.Count - 1);
            hull.Add(p);
        }
        hull.RemoveAt(hull.Count - 1);
        return hull;
    }

    static float Cross(Vector2 o, Vector2 a, Vector2 b)
    {
        return (a.x - o.x) * (b.y - o.y) - (a.y - o.y) * (b.x - o.x);
    }

    // ------------------------------------------------------------ scatter meshes

    sealed class MeshBuilder
    {
        public readonly List<Vector3> Vertices = new List<Vector3>();
        public readonly List<Vector3> Normals = new List<Vector3>();
        public readonly List<Vector2> Uvs = new List<Vector2>();
        public readonly List<int> Triangles = new List<int>();

        public int AddVertex(Vector3 v, Vector2 uv)
        {
            Vertices.Add(v);
            Uvs.Add(uv);
            Normals.Add(Vector3.up);
            return Vertices.Count - 1;
        }

        public void AddTriangle(int a, int b, int c)
        {
            Triangles.Add(a); Triangles.Add(b); Triangles.Add(c);
        }

        public Mesh Build(string name)
        {
            var mesh = new Mesh { name = name };
            mesh.indexFormat = Vertices.Count > 65000
                ? IndexFormat.UInt32 : IndexFormat.UInt16;
            for (int i = 0; i < Vertices.Count; i++)
            {
                var v = Vertices[i];
                if (float.IsNaN(v.x) || float.IsNaN(v.y) || float.IsNaN(v.z))
                    throw new InvalidOperationException(name + ": NaN vertex at " + i);
            }
            mesh.SetVertices(Vertices);
            mesh.SetUVs(0, Uvs);
            // Keep the visible side up: if the summed face normal points down,
            // reverse every triangle rather than hand-checking each ring.
            float up = 0f;
            for (int i = 0; i < Triangles.Count; i += 3)
            {
                Vector3 n = Vector3.Cross(
                    Vertices[Triangles[i + 1]] - Vertices[Triangles[i]],
                    Vertices[Triangles[i + 2]] - Vertices[Triangles[i]]);
                up += n.y;
            }
            var ordered = new List<int>(Triangles);
            if (up < 0f)
                for (int i = 0; i < ordered.Count; i += 3)
                {
                    int tmp = ordered[i + 1];
                    ordered[i + 1] = ordered[i + 2];
                    ordered[i + 2] = tmp;
                }
            mesh.SetTriangles(ordered, 0);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }
    }

    static void BuildScatter(SceneContext context)
    {
        Context = context;
        var old = context.ScatterRoot;
        if (old != null) UnityEngine.Object.DestroyImmediate(old);

        var rnd = new System.Random(836_2026);
        var root = new GameObject(ScatterRootName);
        var world = FindByName(context.Scene, "Compact30World");
        if (world != null) root.transform.SetParent(world.transform, false);
        context.ScatterRoot = root;

        // ---- moss cushions
        var cushions = new List<Cushion>();
        int attempts = 0;
        int nearTrunk = 0;
        while (cushions.Count < 44 && attempts++ < 6000)
        {
            Vector2 p;
            bool hugTrunk = nearTrunk < 24 && context.Trunks.Count > 0 && rnd.NextDouble() < 0.45;
            if (hugTrunk)
            {
                var t = context.Trunks[rnd.Next(context.Trunks.Count)];
                float ang = (float)(rnd.NextDouble() * Math.PI * 2);
                float dist = 0.42f + (float)rnd.NextDouble() * 0.75f;
                p = t + new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)) * dist;
            }
            else
            {
                p = new Vector2((float)(rnd.NextDouble() * 28.0 - 14.0),
                    (float)(rnd.NextDouble() * 28.0 - 14.0));
            }
            if (!Allowed(p, 1.15f, 0.40f, 0.75f)) continue;
            float radius = 0.08f + (float)rnd.NextDouble() * 0.22f;
            // Cushions may touch a trunk root; keep them clear of the stem itself.
            cushions.Add(new Cushion
            {
                Centre = p,
                Radius = radius,
                Height = 0.015f + (float)rnd.NextDouble() * 0.030f,
                Seed = rnd.Next()
            });
            if (hugTrunk) nearTrunk++;
        }

        var mossMesh = BuildCushionMesh(cushions);
        var mossAsset = SaveMesh(mossMesh, MeshRoot + "/Sen1_FloorMossPatches.asset");
        AddRenderer(root.transform, "MossPatches", mossAsset,
            AssetDatabase.LoadAssetAtPath<Material>(MossMaterialPath), true);

        // ---- twigs
        var twigs = new List<Twig>();
        attempts = 0;
        while (twigs.Count < 44 && attempts++ < 4000)
        {
            var p = new Vector2((float)(rnd.NextDouble() * 28.0 - 14.0),
                (float)(rnd.NextDouble() * 28.0 - 14.0));
            if (!Allowed(p, 1.05f, 0.35f, 0.5f)) continue;
            twigs.Add(new Twig
            {
                Centre = p,
                Angle = (float)(rnd.NextDouble() * Math.PI),
                Length = 0.09f + (float)rnd.NextDouble() * 0.14f,
                Radius = 0.0035f + (float)rnd.NextDouble() * 0.0045f,
                Bend = (float)(rnd.NextDouble() - 0.5) * 0.10f,
                Seed = rnd.Next()
            });
        }

        var twigMesh = BuildTwigMesh(twigs);
        var twigAsset = SaveMesh(twigMesh, MeshRoot + "/Sen1_FloorTwigs.asset");
        AddRenderer(root.transform, "Twigs", twigAsset,
            AssetDatabase.LoadAssetAtPath<Material>(TwigMaterialPath), false);

        foreach (var c in cushions)
            Debug.Log("SEN1_FLOOR_CUSHION " + c.Centre.x.ToString("0.00",
                CultureInfo.InvariantCulture) + " " + c.Centre.y.ToString("0.00",
                CultureInfo.InvariantCulture) + " r=" + c.Radius.ToString("0.00",
                CultureInfo.InvariantCulture));
        Debug.Log("SEN1_FLOOR_SCATTER cushions=" + cushions.Count +
            " (near_trunk=" + nearTrunk + ") twigs=" + twigs.Count);
    }

    struct Cushion
    {
        public Vector2 Centre;
        public float Radius;
        public float Height;
        public int Seed;
    }

    struct Twig
    {
        public Vector2 Centre;
        public float Angle;
        public float Length;
        public float Radius;
        public float Bend;
        public int Seed;
    }

    static Mesh BuildCushionMesh(List<Cushion> cushions)
    {
        var b = new MeshBuilder();
        const int segments = 18;
        const int rings = 4;
        foreach (var c in cushions)
        {
            var local = new System.Random(c.Seed);
            var phase1 = (float)local.NextDouble() * 6.283f;
            var phase2 = (float)local.NextDouble() * 6.283f;
            int centreIndex = b.AddVertex(
                new Vector3(c.Centre.x, c.Height, c.Centre.y), new Vector2(0.5f, 0.5f));
            int baseRing = b.Vertices.Count;
            for (int r = 1; r <= rings; r++)
            {
                float t = r / (float)rings;
                for (int s = 0; s < segments; s++)
                {
                    float a = s * Mathf.PI * 2f / segments;
                    float wobble = 1f + 0.20f * Mathf.Sin(3f * a + phase1)
                        + 0.12f * Mathf.Sin(5f * a + phase2);
                    float radius = c.Radius * t * wobble;
                    // Cos(pi/2) can be a tiny negative float; Pow(negative, 1.15)
                    // is NaN, which silently produces an unusable mesh.
                    float profile = Mathf.Max(0f, Mathf.Cos(t * Mathf.PI * 0.5f));
                    float lift = c.Height * Mathf.Pow(profile, 1.15f)
                        * (0.85f + 0.15f * Mathf.Sin(4f * a + phase2));
                    float x = c.Centre.x + Mathf.Cos(a) * radius;
                    float z = c.Centre.y + Mathf.Sin(a) * radius;
                    // Bulge outward a little at mid-ring so the cushion has a shoulder.
                    float shoulder = c.Height * 0.20f * Mathf.Sin(t * Mathf.PI);
                    b.AddVertex(new Vector3(x, lift + shoulder, z),
                        new Vector2(x * 1.6f, z * 1.6f));
                }
            }

            for (int s = 0; s < segments; s++)
            {
                int i0 = baseRing + s;
                int i1 = baseRing + (s + 1) % segments;
                b.AddTriangle(centreIndex, i0, i1);
            }
            for (int r = 1; r < rings; r++)
            {
                int inner = baseRing + (r - 1) * segments;
                int outer = baseRing + r * segments;
                for (int s = 0; s < segments; s++)
                {
                    int s1 = (s + 1) % segments;
                    b.AddTriangle(inner + s, outer + s, outer + s1);
                    b.AddTriangle(inner + s, outer + s1, inner + s1);
                }
            }
        }
        return b.Build("Sen1_FloorMossPatches");
    }

    static Mesh BuildTwigMesh(List<Twig> twigs)
    {
        var b = new MeshBuilder();
        const int sides = 6;
        foreach (var t in twigs)
        {
            var local = new System.Random(t.Seed);
            Vector2 dir = new Vector2(Mathf.Cos(t.Angle), Mathf.Sin(t.Angle));
            Vector2 perp = new Vector2(-dir.y, dir.x);
            int rings = 4;
            int startRing = b.Vertices.Count;
            for (int r = 0; r < rings; r++)
            {
                float u = r / (float)(rings - 1);
                float along = (u - 0.5f) * t.Length;
                Vector2 centre = t.Centre + dir * along
                    + perp * (t.Bend * (u - 0.5f) * (u - 0.5f) * 4f);
                float radius = t.Radius * Mathf.Lerp(1.0f, 0.45f, u)
                    * (1f + 0.12f * (float)Math.Sin(u * 9.0 + t.Seed));
                float y = radius * 0.92f;
                for (int s = 0; s < sides; s++)
                {
                    float a = s * Mathf.PI * 2f / sides;
                    float ox = Mathf.Cos(a) * radius;
                    float oy = Mathf.Sin(a) * radius;
                    b.AddVertex(
                        new Vector3(centre.x + perp.x * ox, y + oy,
                            centre.y + perp.y * ox),
                        new Vector2(u * 2f, s / (float)sides));
                }
            }
            for (int r = 0; r < rings - 1; r++)
            {
                for (int s = 0; s < sides; s++)
                {
                    int s1 = (s + 1) % sides;
                    int i0 = startRing + r * sides + s;
                    int i1 = startRing + r * sides + s1;
                    int i2 = startRing + (r + 1) * sides + s;
                    int i3 = startRing + (r + 1) * sides + s1;
                    b.AddTriangle(i0, i2, i1);
                    b.AddTriangle(i1, i2, i3);
                }
            }
        }
        return b.Build("Sen1_FloorTwigs");
    }

    static Mesh SaveMesh(Mesh mesh, string path)
    {
        var existing = AssetDatabase.LoadAssetAtPath<Mesh>(path);
        if (existing != null) AssetDatabase.DeleteAsset(path);
        AssetDatabase.CreateAsset(mesh, path);
        return AssetDatabase.LoadAssetAtPath<Mesh>(path);
    }

    static void AddRenderer(Transform parent, string name, Mesh mesh, Material material,
        bool castShadows)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.AddComponent<MeshFilter>().sharedMesh = mesh;
        var renderer = go.AddComponent<MeshRenderer>();
        renderer.sharedMaterial = material;
        renderer.shadowCastingMode = castShadows
            ? ShadowCastingMode.On : ShadowCastingMode.Off;
        renderer.receiveShadows = true;
        renderer.lightProbeUsage = LightProbeUsage.BlendProbes;
    }

    // ------------------------------------------------------------ wiring

    static void WireScene(SceneContext context)
    {
        var floor = AssetDatabase.LoadAssetAtPath<Material>(FloorMaterialPath);
        var margin = AssetDatabase.LoadAssetAtPath<Material>(MarginMaterialPath);
        if (floor == null || margin == null)
            throw new InvalidOperationException("Floor materials missing");

        var far = AssetDatabase.LoadAssetAtPath<Material>(FarMaterialPath);
        context.GroundMaterial = floor;
        foreach (var renderer in context.LowCover) renderer.sharedMaterial = margin;
        if (context.OutsideGround != null) context.OutsideGround.sharedMaterial = far;
        Debug.Log("SEN1_FLOOR_WIRED ground=" + floor.name +
            " lowcover=" + context.LowCover.Count);
    }

    // ------------------------------------------------------------ capture

    struct Pose
    {
        public string Name;
        public Vector3 Eye;
        public Vector3 Target;
    }

    static readonly Pose[] Poses =
    {
        new Pose { Name = "01_interior_trunk_foot",
            Eye = new Vector3(4.10f, 1.55f, 2.60f),
            Target = new Vector3(2.30f, 0.05f, 0.30f) },
        new Pose { Name = "02_interior_floor_close",
            Eye = new Vector3(-2.60f, 1.60f, 5.40f),
            Target = new Vector3(-3.60f, 0.02f, 4.10f) },
        new Pose { Name = "03_margin_light_edge",
            Eye = new Vector3(11.40f, 1.60f, -2.20f),
            Target = new Vector3(15.40f, 0.55f, -3.60f) },
        new Pose { Name = "04_path_h1_join",
            Eye = new Vector3(5.60f, 1.60f, -1.20f),
            Target = new Vector3(-1.20f, 0.05f, -8.20f) },
        new Pose { Name = "05_walk_the_floor",
            Eye = new Vector3(0.40f, 1.62f, 1.60f),
            Target = new Vector3(0.90f, 0.00f, -6.00f) },
        new Pose { Name = "06_overhead_floor",
            Eye = new Vector3(0.00f, 9.00f, -2.00f),
            Target = new Vector3(0.00f, 0.00f, -2.01f) },
        new Pose { Name = "07_g1_planting_floor",
            Eye = new Vector3(-0.30f, 1.58f, -1.60f),
            Target = new Vector3(-1.20f, 0.03f, -2.90f) },
        new Pose { Name = "10_moss_and_twig_close",
            Eye = new Vector3(5.15f, 1.25f, 4.55f),
            Target = new Vector3(5.94f, 0.02f, 5.37f) },
        new Pose { Name = "09_sunlit_dapple",
            Eye = new Vector3(6.20f, 1.50f, -5.40f),
            Target = new Vector3(4.20f, 0.02f, -7.20f) },
        new Pose { Name = "08_floor_wide",
            Eye = new Vector3(9.20f, 2.10f, 8.40f),
            Target = new Vector3(-1.00f, 0.00f, -2.00f) },
        new Pose { Name = "11_c0_mouth_seam",
            Eye = new Vector3(5.80f, 1.60f, -8.60f),
            Target = new Vector3(2.99f, 0.00f, -4.75f) },
        new Pose { Name = "12_h1_from_south_opening",
            Eye = new Vector3(-2.25f, 1.60f, -14.60f),
            Target = new Vector3(0.00f, 0.40f, -5.00f) },
        new Pose { Name = "13_fence_corner_sw_inside",
            Eye = new Vector3(-11.00f, 1.60f, -11.00f),
            Target = new Vector3(-15.75f, 0.90f, -15.75f) },
        new Pose { Name = "14_fence_corner_ne_outside",
            Eye = new Vector3(20.00f, 1.70f, 20.00f),
            Target = new Vector3(15.75f, 0.90f, 15.75f) },
        new Pose { Name = "15_south_opening_outside",
            Eye = new Vector3(-2.25f, 1.65f, -21.00f),
            Target = new Vector3(-2.25f, 0.80f, -12.00f) },
        new Pose { Name = "16_path_north_a7_a8",
            Eye = new Vector3(-0.60f, 1.60f, 5.20f),
            Target = new Vector3(-2.40f, 0.00f, 12.50f) },
        new Pose { Name = "17_overhead_enclosure",
            Eye = new Vector3(0.00f, 42.00f, -0.50f),
            Target = new Vector3(0.00f, 0.00f, 0.00f) },
        new Pose { Name = "18_fence_east_run_inside",
            Eye = new Vector3(11.50f, 1.60f, -12.00f),
            Target = new Vector3(15.75f, 0.90f, 6.00f) }
    };

    static void WarmUp(Camera camera)
    {
        var rt = new RenderTexture(320, 180, 24, RenderTextureFormat.ARGB32,
            RenderTextureReadWrite.sRGB);
        rt.Create();
        var previous = camera.targetTexture;
        camera.targetTexture = rt;
        for (int i = 0; i < 6; i++) camera.Render();
        camera.targetTexture = previous;
        rt.Release();
        UnityEngine.Object.DestroyImmediate(rt);
    }

    static void RenderAll(string directory, SceneContext context, bool logs)
    {
        var main = Camera.main;
        if (main == null)
        {
            foreach (var c in UnityEngine.Object.FindObjectsByType<Camera>(
                         FindObjectsSortMode.None)) { main = c; break; }
        }
        if (main == null) throw new InvalidOperationException("Main Camera missing");
        var original = UnityEngine.Object.Instantiate(main.gameObject);
        original.name = "Sen1_FloorCaptureCamera";
        foreach (var behaviour in original.GetComponents<MonoBehaviour>())
            if (!(behaviour is UniversalAdditionalCameraData)) behaviour.enabled = false;
        var camera = original.GetComponent<Camera>();
        var extra = camera.GetUniversalAdditionalCameraData();
        extra.renderPostProcessing = true;
        camera.fieldOfView = 60f;
        camera.nearClipPlane = 0.04f;
        camera.farClipPlane = 240f;

        // URP needs a few rendered frames before the sun/shadow atlas and probe
        // data settle; the first frame after a camera is created is under-lit.
        WarmUp(camera);

        int[] widths = { 1280, 1920 };
        foreach (var pose in Poses)
        {
            camera.transform.position = pose.Eye;
            camera.transform.LookAt(pose.Target);
            WarmUp(camera);
            foreach (int width in widths)
            {
                int height = Mathf.RoundToInt(width * 9f / 16f);
                var rt = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32,
                    RenderTextureReadWrite.sRGB) { antiAliasing = 4 };
                rt.Create();
                camera.targetTexture = rt;
                camera.aspect = width / (float)height;
                camera.Render();
                RenderTexture.active = rt;
                var tex = new Texture2D(width, height, TextureFormat.RGB24, false,
                    false);
                tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
                tex.Apply();
                string file = Path.Combine(directory,
                    pose.Name + "_" + width + "x" + height + ".png");
                File.WriteAllBytes(file, tex.EncodeToPNG());
                UnityEngine.Object.DestroyImmediate(tex);
                RenderTexture.active = null;
                camera.targetTexture = null;
                rt.Release();
                UnityEngine.Object.DestroyImmediate(rt);
                if (logs) Debug.Log("SEN1_FLOOR_CAPTURE " + file);
            }
        }
        UnityEngine.Object.DestroyImmediate(original);
    }

    public static void Diagnose()
    {
        try
        {
            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            var context = new SceneContext(scene);
            string dir = Path.Combine(CaptureRoot, "diagnose");
            Directory.CreateDirectory(dir);
            var oldGround = AssetDatabase.LoadAssetAtPath<Material>(OldGroundMaterial);
            var floor = AssetDatabase.LoadAssetAtPath<Material>(FloorMaterialPath);

            RenderPoseTo(dir, "a_new_floor", 0, repeat: 3);
            if (context.ScatterRoot != null) context.ScatterRoot.SetActive(false);
            RenderPoseTo(dir, "b_no_scatter", 0, repeat: 2);
            if (context.ScatterRoot != null) context.ScatterRoot.SetActive(true);
            context.GroundMaterial = oldGround;
            RenderPoseTo(dir, "c_old_material", 0, repeat: 2);
            context.GroundMaterial = floor;
            floor.SetFloat("_OcclusionStrength", 0f);
            RenderPoseTo(dir, "d_no_occlusion", 0, repeat: 2);
            floor.SetFloat("_OcclusionStrength", 1f);
            Debug.Log("SEN1_FLOOR_DIAGNOSE_PASS " + dir);
            EditorApplication.Exit(0);
        }
        catch (Exception e)
        {
            Debug.LogError("SEN1_FLOOR_DIAGNOSE_FAIL " + e);
            EditorApplication.Exit(1);
        }
    }

    static void RenderPoseTo(string directory, string label, int poseIndex, int repeat)
    {
        var main = Camera.main;
        var clone = UnityEngine.Object.Instantiate(main.gameObject);
        foreach (var behaviour in clone.GetComponents<MonoBehaviour>())
            if (!(behaviour is UniversalAdditionalCameraData)) behaviour.enabled = false;
        var camera = clone.GetComponent<Camera>();
        camera.GetUniversalAdditionalCameraData().renderPostProcessing = true;
        camera.fieldOfView = 60f;
        camera.nearClipPlane = 0.04f;
        var pose = Poses[poseIndex];
        camera.transform.position = pose.Eye;
        camera.transform.LookAt(pose.Target);
        for (int i = 0; i < repeat; i++)
        {
            var rt = new RenderTexture(640, 360, 24, RenderTextureFormat.ARGB32,
                RenderTextureReadWrite.sRGB);
            rt.Create();
            camera.targetTexture = rt;
            camera.Render();
            RenderTexture.active = rt;
            var tex = new Texture2D(640, 360, TextureFormat.RGB24, false);
            tex.ReadPixels(new Rect(0, 0, 640, 360), 0, 0);
            tex.Apply();
            File.WriteAllBytes(Path.Combine(directory, label + "_f" + i + ".png"),
                tex.EncodeToPNG());
            UnityEngine.Object.DestroyImmediate(tex);
            RenderTexture.active = null;
            camera.targetTexture = null;
            rt.Release();
            UnityEngine.Object.DestroyImmediate(rt);
        }
        UnityEngine.Object.DestroyImmediate(clone);
    }

    // ------------------------------------------------------------ verification

    static SceneContext Context;

    static string StructuralReport(SceneContext context)
    {
        var sb = new StringBuilder();
        sb.AppendLine("SEN1_FLOOR_STRUCTURE_BEGIN");
        sb.AppendLine("ground_material=" + context.GroundMaterial.name);
        sb.AppendLine("lowcover_renderers=" + context.LowCover.Count +
            " shared_material=" + (context.LowCover.Count > 0
                ? context.LowCover[0].sharedMaterial.name : "none"));
        sb.AppendLine("containment_colliders=" + context.Containment.Count);
        sb.AppendLine("outside_ground_material=" + (context.OutsideGround != null
            ? context.OutsideGround.sharedMaterial.name : "none"));
        sb.AppendLine("trees_root=" + context.TreesRoot.name +
            " inside_trunks=" + context.Trunks.Count);

        int colliders = 0;
        int renderers = 0;
        long triangles = 0;
        foreach (var root in context.Scene.GetRootGameObjects())
        {
            foreach (var c in root.GetComponentsInChildren<Collider>(true)) colliders++;
            foreach (var r in root.GetComponentsInChildren<Renderer>(true))
            {
                renderers++;
                var mf = r.GetComponent<MeshFilter>();
                if (mf != null && mf.sharedMesh != null)
                    triangles += mf.sharedMesh.triangles.Length / 3;
            }
        }
        sb.AppendLine("scene_colliders=" + colliders + " scene_renderers=" + renderers +
            " scene_triangles=" + triangles);

        var scatter = context.ScatterRoot;
        sb.AppendLine("scatter_root=" + (scatter != null ? "present" : "MISSING"));
        if (scatter != null)
        {
            foreach (var r in scatter.GetComponentsInChildren<Renderer>(true))
            {
                var mf = r.GetComponent<MeshFilter>();
                sb.AppendLine("  scatter " + r.name + " material=" + r.sharedMaterial.name +
                    " verts=" + (mf != null && mf.sharedMesh != null
                        ? mf.sharedMesh.vertexCount : -1) +
                    " tris=" + (mf != null && mf.sharedMesh != null
                        ? mf.sharedMesh.triangles.Length / 3 : -1) +
                    " shadows=" + r.shadowCastingMode +
                    " colliders=" + r.GetComponents<Collider>().Length);
            }
        }
        var floorMaterial = AssetDatabase.LoadAssetAtPath<Material>(FloorMaterialPath);
        if (floorMaterial != null)
        {
            sb.AppendLine("floor_base=" + Tex(floorMaterial, "_BaseMap"));
            sb.AppendLine("floor_normal=" + Tex(floorMaterial, "_BumpMap"));
            sb.AppendLine("floor_occlusion=" + Tex(floorMaterial, "_OcclusionMap"));
            sb.AppendLine("floor_keywords=" + string.Join(",",
                new List<string>(floorMaterial.shaderKeywords)));
        }
        sb.AppendLine("SEN1_FLOOR_STRUCTURE_END");
        return sb.ToString();
    }

    static string Tex(Material material, string property)
    {
        var t = material.GetTexture(property);
        return t == null ? "none" : t.name;
    }

    static void Log(string message)
    {
        foreach (var line in message.Split('\n'))
            if (line.Trim().Length > 0) Debug.Log(line.TrimEnd());
    }
}
