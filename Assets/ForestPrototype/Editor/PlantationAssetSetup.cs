using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class PlantationAssetSetup
{
    private const string Source = "/media/jer/ZX20/Unity Assets/Sitka_Plantation_02/";
    public const string Art = "Assets/ForestPrototype/Art/SitkaPlantation02/";
    public const string CatalogPath = "Assets/ForestPrototype/ScenarioOne/Resources/PlantationVisualCatalog.asset";
    private const string Meshes = "Assets/ForestPrototype/Meshes/Plantation02/";
    private const string Prefabs = "Assets/ForestPrototype/Prefabs/Forestry/Plantation02/";
    private static readonly string[] MaterialNames = { "SS_Benchmark_Bark", "SS_Benchmark_Needles",
        "SS_Benchmark_CutWood", "SS_Benchmark_WeatheredScar" };
    private static readonly Dictionary<string, PlantationLod> Converted = new Dictionary<string, PlantationLod>();
    private static Dictionary<string, JToken> exports;

    [MenuItem("Tools/Forest Prototype/Integrate Plantation Sitka 02")]
    public static void BuildAndWire()
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();
        JObject hashes = JObject.Parse(File.ReadAllText(Source + "Delivery_SHA256.json"));
        int verified = 0;
        foreach (JProperty entry in hashes.Properties())
        {
            string path = Source + entry.Name;
            using (var sha = SHA256.Create())
                if (BitConverter.ToString(sha.ComputeHash(File.ReadAllBytes(path))).Replace("-", "").ToLowerInvariant()
                    != (string)entry.Value) throw new InvalidOperationException("Delivery checksum mismatch: " + entry.Name);
            verified++;
        }
        JObject manifest = JObject.Parse(File.ReadAllText(Source + "Manifest.json"));
        exports = manifest["exports"].ToDictionary(row => (string)row["file"]);
        if (exports.Count != 474) throw new InvalidOperationException("Unexpected plantation export coverage");
        foreach (string path in exports.Keys) Copy(path, Art + path);
        foreach (string path in Directory.GetFiles(Source + "Textures", "*.png"))
            Copy("Textures/" + Path.GetFileName(path), Art + "Textures/" + Path.GetFileName(path));
        foreach (string name in new[] { "README.md", "Coverage.md", "Integration_Handoff.md", "Manifest.json",
            "Branch_Modules.json", "Branch_Contact.json", "Scar_Placement.json", "ScaleAware_Pruning_Examples.json",
            "Validation.json", "Eye_Level_Validation.json", "Geometry_Counts.md", "Visual_Review.json", "Delivery_SHA256.json" })
            Copy(name, Art + "Docs/" + name);
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        foreach (string path in exports.Keys)
        {
            var importer = (ModelImporter)AssetImporter.GetAtPath(Art + path);
            if (!importer.importAnimation && !importer.importCameras && !importer.importLights
                && !importer.addCollider && importer.isReadable
                && importer.materialImportMode == ModelImporterMaterialImportMode.ImportStandard
                && importer.materialLocation == ModelImporterMaterialLocation.InPrefab) continue;
            importer.importAnimation = false; importer.importCameras = false; importer.importLights = false;
            importer.addCollider = false; importer.isReadable = true;
            importer.materialImportMode = ModelImporterMaterialImportMode.ImportStandard;
            importer.materialLocation = ModelImporterMaterialLocation.InPrefab;
            importer.SaveAndReimport();
        }
        Converted.Clear();
        Directory.CreateDirectory(Meshes); Directory.CreateDirectory(Prefabs);
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        var catalog = AssetDatabase.LoadAssetAtPath<PlantationVisualCatalog>(CatalogPath);
        if (catalog == null)
        {
            catalog = ScriptableObject.CreateInstance<PlantationVisualCatalog>();
            AssetDatabase.CreateAsset(catalog, CatalogPath);
        }
        catalog.materials = MaterialNames.Select(MaterialFor).ToArray();
        JObject contact = JObject.Parse(File.ReadAllText(Source + "Branch_Contact.json"));
        JObject scars = JObject.Parse(File.ReadAllText(Source + "Scar_Placement.json"));
        var placements = scars["placements"].ToDictionary(row => (string)row["branch_id"]);
        JToken[] bases = manifest["bases"].ToArray();
        catalog.bases = new PlantationBase[bases.Length];
        // Confirm actual Unity FBX orientation from multiple asymmetric module
        // bounds. The delivered export matrix alone does not encode handedness.
        Matrix4x4 basis = DetectBasis(bases);
        for (int b = 0; b < bases.Length; b++)
        {
            JToken record = bases[b];
            var data = new PlantationBase { id = (string)record["base_id"], authoredHeight = (float)record["height_m"],
                core = new PlantationLod[3] };
            for (int lod = 0; lod < 3; lod++) data.core[lod] = Convert((string)record["core_files"][lod.ToString()]);
            JToken[] records = record["branch_records"].ToArray();
            data.branches = new PlantationBranch[records.Length];
            var contacts = contact["bases"].Single(row => (string)row["base_id"] == data.id)["branch_systems"]
                .ToDictionary(row => (string)row["branch_id"]);
            for (int i = 0; i < records.Length; i++)
            {
                JToken branch = records[i]; string id = (string)branch["branch_id"];
                var result = new PlantationBranch { id = id, attachmentHeight = (float)branch["attachment_height_m"],
                    lods = new PlantationLod[3], scarTransforms = new Matrix4x4[3] };
                Bounds union = new Bounds(); bool first = true;
                for (int lod = 0; lod < 3; lod++)
                {
                    result.lods[lod] = Convert((string)branch["module_files"][lod.ToString()]);
                    foreach (PlantationPiece piece in result.lods[lod].pieces)
                        if (first) { union = piece.mesh.bounds; first = false; } else union.Encapsulate(piece.mesh.bounds);
                    JToken placement = placements[id]["placement_by_lod"][lod.ToString()];
                    var author = Matrix4x4.identity;
                    float size = (float)placement["piece_scale"];
                    author.SetColumn(0, Vec(placement["tangent_x_local"]) * size);
                    author.SetColumn(1, Vec(placement["tangent_y_local"]) * size);
                    author.SetColumn(2, Vec(placement["normal_local"]) * size);
                    author.SetColumn(3, new Vector4((float)placement["position_local_m"][0],
                        (float)placement["position_local_m"][1], (float)placement["position_local_m"][2], 1));
                    result.scarTransforms[lod] = basis * author * basis.inverse;
                }
                result.bounds = union;
                Bounds expected = TransformBounds(branch["all_lod_bounds_m"], basis);
                if ((union.min - expected.min).magnitude > 0.003f || (union.max - expected.max).magnitude > 0.003f)
                    throw new InvalidOperationException("Imported module bounds/axis mismatch: " + id + " " + union + " " + expected);
                result.contacts = contacts[id]["woody_capsules"].Concat(contacts[id]["optional_foliage_capsules"])
                    .Select(row => new PlantationCapsule
                    {
                        segmentId = (string)row["segment_id"], start = basis.MultiplyPoint3x4(Vec(row["axis_start_local_m"])),
                        end = basis.MultiplyPoint3x4(Vec(row["axis_end_local_m"])), radius = (float)row["radius_m"],
                        flexible = (string)row["kind"] == "flexible_foliage"
                    }).ToArray();
                ValidateAxes(result);
                data.branches[i] = result;
            }
            catalog.bases[b] = data;
        }
        catalog.recentScars = Enumerable.Range(0, 3).Select(lod => Convert((string)scars["reusable_pieces"]["Recent"][lod.ToString()])).ToArray();
        catalog.healedScars = Enumerable.Range(0, 3).Select(lod => Convert((string)scars["reusable_pieces"]["Healed"][lod.ToString()])).ToArray();
        for (int b = 0; b < catalog.bases.Length; b++)
        {
            var root = new GameObject(catalog.bases[b].id);
            try
            {
                var visual = root.AddComponent<PlantationTreeVisual>(); visual.catalog = catalog; visual.baseIndex = b;
                GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, Prefabs + catalog.bases[b].id + ".prefab");
                if (catalog.bases[b].authoredHeight == 8f) catalog.polePrefab = prefab; else catalog.firstThinningPrefab = prefab;
            }
            finally { UnityEngine.Object.DestroyImmediate(root); }
        }
        EditorUtility.SetDirty(catalog); AssetDatabase.SaveAssets();
        ForestryAssetSetup.RemoveLegacySitkaFromStand();
        Debug.Log($"PLANTATION_ASSETS_PASS models={exports.Count} branches={catalog.bases.Sum(b => b.branches.Length)} verifiedFiles={verified} seconds={watch.Elapsed.TotalSeconds:0.0}");
    }

    public static void WireSpawner(SerializedObject spawner)
    {
        var catalog = AssetDatabase.LoadAssetAtPath<PlantationVisualCatalog>(CatalogPath);
        if (catalog == null || catalog.polePrefab == null || catalog.firstThinningPrefab == null) return;
        spawner.FindProperty("usePlantationVisuals").boolValue = true;
        spawner.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void Copy(string relative, string target)
    {
        byte[] bytes = File.ReadAllBytes(Source + relative);
        Directory.CreateDirectory(Path.GetDirectoryName(target));
        if (File.Exists(target) && !bytes.SequenceEqual(File.ReadAllBytes(target)))
            throw new InvalidOperationException("Existing import differs: " + target);
        if (!File.Exists(target)) File.WriteAllBytes(target, bytes);
    }

    private static Material MaterialFor(string name)
    {
        string path = Art + "Materials/" + name + ".mat";
        var material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (material == null)
        {
            Directory.CreateDirectory(Art + "Materials"); AssetDatabase.Refresh();
            material = new Material(Shader.Find("Universal Render Pipeline/Lit")) { name = name, enableInstancing = true };
            AssetDatabase.CreateAsset(material, path);
        }
        string file = name == MaterialNames[0] ? "Sitka_Bark_BaseColor_v2.png"
            : name == MaterialNames[1] ? "Sitka_NeedleSprigs_BaseColorAlpha.png"
            : name == MaterialNames[2] ? "Sitka_CutWood_BaseColor.png" : null;
        if (file != null)
        {
            string texturePath = Art + "Textures/" + file;
            var importer = (TextureImporter)AssetImporter.GetAtPath(texturePath);
            importer.sRGBTexture = true; importer.mipmapEnabled = true;
            if (name == MaterialNames[1])
            { importer.alphaIsTransparency = true; importer.mipMapsPreserveCoverage = true; importer.alphaTestReferenceValue = 0.38f; }
            importer.SaveAndReimport();
            material.SetTexture("_BaseMap", AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath));
        }
        material.SetColor("_BaseColor", name == MaterialNames[3] ? new Color(0.235f, 0.205f, 0.18f) : Color.white);
        bool needles = name == MaterialNames[1];
        material.SetFloat("_Metallic", 0); material.SetFloat("_Smoothness", name == MaterialNames[0] ? 0.15f : 0.05f);
        material.SetFloat("_Surface", 0); material.SetFloat("_AlphaClip", needles ? 1 : 0);
        material.SetFloat("_Cutoff", 0.38f); material.SetFloat("_Cull", needles ? 0 : 2);
        material.SetFloat("_ZWrite", 1); material.SetFloat("_SrcBlend", 1); material.SetFloat("_DstBlend", 0);
        if (needles) material.EnableKeyword("_ALPHATEST_ON"); else material.DisableKeyword("_ALPHATEST_ON");
        material.renderQueue = needles ? 2450 : -1;
        material.SetOverrideTag("RenderType", needles ? "TransparentCutout" : "Opaque");
        // Existing RGB-only benchmark gloss export has no useful alpha.
        material.SetTexture("_MetallicGlossMap", null); material.DisableKeyword("_METALLICSPECGLOSSMAP");
        if (name == MaterialNames[0])
        {
            string normal = Art + "Textures/Sitka_Bark_Normal_v2.png";
            var importer = (TextureImporter)AssetImporter.GetAtPath(normal);
            importer.textureType = TextureImporterType.NormalMap; importer.SaveAndReimport();
            material.SetTexture("_BumpMap", AssetDatabase.LoadAssetAtPath<Texture2D>(normal));
            material.SetFloat("_BumpScale", 0.7f); material.EnableKeyword("_NORMALMAP");
        }
        EditorUtility.SetDirty(material); return material;
    }

    private static PlantationLod Convert(string relative)
    {
        if (Converted.TryGetValue(relative, out var found)) return found;
        JToken row = exports[relative];
        GameObject model = UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(Art + relative));
        try
        {
            var pieces = new List<PlantationPiece>();
            foreach (JToken slotRecord in row["material_slots"])
            {
                string objectName = (string)slotRecord["object"];
                MeshFilter filter = model.GetComponentsInChildren<MeshFilter>(true).Single(f => f.name == objectName);
                Mesh original = filter.sharedMesh;
                var mesh = UnityEngine.Object.Instantiate(original);
                Matrix4x4 matrix = filter.transform.localToWorldMatrix;
                mesh.vertices = original.vertices.Select(matrix.MultiplyPoint3x4).ToArray();
                Matrix4x4 normals = matrix.inverse.transpose;
                mesh.normals = original.normals.Select(n => normals.MultiplyVector(n).normalized).ToArray();
                mesh.tangents = original.tangents.Select(t =>
                {
                    Vector3 xyz = matrix.MultiplyVector(new Vector3(t.x, t.y, t.z)).normalized;
                    return new Vector4(xyz.x, xyz.y, xyz.z, t.w * Mathf.Sign(matrix.determinant));
                }).ToArray();
                if (matrix.determinant < 0)
                    for (int sub = 0; sub < mesh.subMeshCount; sub++)
                    {
                        int[] indices = mesh.GetTriangles(sub);
                        for (int i = 0; i < indices.Length; i += 3) { int swap = indices[i]; indices[i] = indices[i + 1]; indices[i + 1] = swap; }
                        mesh.SetTriangles(indices, sub);
                    }
                mesh.RecalculateBounds(); mesh.name = objectName + " Unity metres";
                string path = Meshes + objectName + ".asset";
                Mesh existing = AssetDatabase.LoadAssetAtPath<Mesh>(path);
                if (existing == null) AssetDatabase.CreateAsset(mesh, path);
                else { EditorUtility.CopySerialized(mesh, existing); UnityEngine.Object.DestroyImmediate(mesh); mesh = existing; }
                // FBX import can discard unused slots (Recent has no healed
                // centre and vice versa). Resolve each imported submesh against
                // the explicitly declared slot names, never positional guesses.
                int[] slots = filter.GetComponent<Renderer>().sharedMaterials.Select(material =>
                {
                    string declared = slotRecord["slots"].Select(slot => (string)slot["material"])
                        .Single(name => material != null && (material.name == name || material.name.StartsWith(name + ".", StringComparison.Ordinal)));
                    return Array.IndexOf(MaterialNames, declared);
                }).ToArray();
                if (slots.Any(index => index < 0) || slots.Length != mesh.subMeshCount)
                    throw new InvalidOperationException("Material/submesh mapping mismatch: " + objectName);
                pieces.Add(new PlantationPiece { mesh = mesh, materialSlots = slots });
            }
            found = new PlantationLod { pieces = pieces.ToArray() }; Converted.Add(relative, found); return found;
        }
        finally { UnityEngine.Object.DestroyImmediate(model); }
    }

    private static Matrix4x4 DetectBasis(JToken[] bases)
    {
        Matrix4x4 best = Matrix4x4.identity; float bestError = float.PositiveInfinity;
        for (int sx = -1; sx <= 1; sx += 2)
            for (int sz = -1; sz <= 1; sz += 2)
            {
                Matrix4x4 candidate = Matrix4x4.zero;
                candidate.m00 = sx; candidate.m12 = 1; candidate.m21 = sz; candidate.m33 = 1;
                float error = 0;
                foreach (JToken row in bases[0]["branch_records"].Take(5))
                {
                    PlantationLod lod = Convert((string)row["module_files"]["0"]);
                    Bounds actual = lod.pieces[0].mesh.bounds;
                    foreach (PlantationPiece piece in lod.pieces.Skip(1)) actual.Encapsulate(piece.mesh.bounds);
                    // Use LOD0 plus other LODs to compare the exported union.
                    for (int level = 1; level < 3; level++)
                        foreach (PlantationPiece piece in Convert((string)row["module_files"][level.ToString()]).pieces)
                            actual.Encapsulate(piece.mesh.bounds);
                    Bounds expected = TransformBounds(row["all_lod_bounds_m"], candidate);
                    error += (actual.min - expected.min).sqrMagnitude + (actual.max - expected.max).sqrMagnitude;
                }
                if (error < bestError) { bestError = error; best = candidate; }
            }
        if (bestError > 0.0001f) throw new InvalidOperationException("Cannot establish Unity importer axis/metre basis: " + bestError);
        Debug.Log($"PLANTATION_IMPORT_BASIS x={best.m00} unityY=authorZ unityZ={best.m21}*authorY boundsError={bestError}");
        return best;
    }
    private static Vector3 Vec(JToken value) => new Vector3((float)value[0], (float)value[1], (float)value[2]);
    private static Bounds TransformBounds(JToken record, Matrix4x4 basis)
    {
        Vector3 min = Vec(record["min"]), max = Vec(record["max"]);
        Vector3 a = basis.MultiplyPoint3x4(min), b = basis.MultiplyPoint3x4(max);
        var bounds = new Bounds(); bounds.SetMinMax(Vector3.Min(a, b), Vector3.Max(a, b)); return bounds;
    }
    private static void ValidateAxes(PlantationBranch branch)
    {
        Vector3[] vertices = branch.lods[0].pieces.Where(piece => piece.materialSlots.All(slot => slot == 0))
            .SelectMany(piece => piece.mesh.vertices).ToArray();
        foreach (PlantationCapsule capsule in branch.contacts.Where(c => !c.flexible))
            foreach (Vector3 point in new[] { capsule.start, capsule.end })
                if (vertices.Min(v => (v - point).sqrMagnitude) > Mathf.Pow(capsule.radius + 0.025f, 2))
                    throw new InvalidOperationException("Contact/render axis misalignment: " + capsule.segmentId);
    }
}
