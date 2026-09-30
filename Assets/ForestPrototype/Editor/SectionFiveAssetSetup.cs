using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Engine-side import only. The delivered source files and the other agent's
// modelling workspace are read-only inputs.
public static class SectionFiveAssetSetup
{
    private const string Source = "/media/jer/ZX20/Unity Assets/";
    private const string Art = "Assets/ForestPrototype/Art/SectionFive/";
    private const string Prefabs = "Assets/ForestPrototype/Prefabs/Forestry/SectionFive/";
    private const string CatalogPath = "Assets/ForestPrototype/ScenarioOne/Resources/SectionFiveVisualCatalog.asset";
    private static readonly string[] Floor = { "Oak_LeafLitter_Patch_01", "Beech_LeafLitter_Patch_01",
        "Mixed_LeafLitter_Patch_01", "Small_Deadwood_Scatter_01", "Small_Deadwood_Scatter_02" };
    private static readonly string[] Woodland = { "ShadeFern_Clump_01", "Bracken_Clump_01", "Bramble_Clump_01",
        "Moss_Deadwood_Patch_01", "SS_Log_Fresh_01", "SS_Log_Decayed_01", "SS_Stump_01" };

    [MenuItem("Tools/Forest Prototype/Integrate Delivered Section 5 Assets")]
    public static void BuildAndWire()
    {
        CopyPackage("Forest_Floor_Detail", Floor, true);
        CopyPackage("Woodland_Assets", Woodland, false);
        CopyPackage("Tree_Variation_Additions", new[] { "Beech_Sapling_01" }, false);
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        foreach (string asset in Floor) Build("Forest_Floor_Detail", asset, true);
        foreach (string asset in Woodland) Build("Woodland_Assets", asset, false);
        Build("Tree_Variation_Additions", "Beech_Sapling_01", false);
        Folder(Path.GetDirectoryName(CatalogPath));
        SectionFiveVisualCatalog catalog = AssetDatabase.LoadAssetAtPath<SectionFiveVisualCatalog>(CatalogPath);
        if (catalog == null)
        {
            catalog = ScriptableObject.CreateInstance<SectionFiveVisualCatalog>();
            AssetDatabase.CreateAsset(catalog, CatalogPath);
        }
        catalog.oakLitter = Load(Floor[0]); catalog.beechLitter = Load(Floor[1]); catalog.mixedLitter = Load(Floor[2]);
        catalog.smallDeadwood = new[] { Load(Floor[3]), Load(Floor[4]) };
        catalog.shadeFern = Load(Woodland[0]); catalog.bracken = Load(Woodland[1]); catalog.bramble = Load(Woodland[2]);
        catalog.mossOnWood = Load(Woodland[3]); catalog.freshLog = Load(Woodland[4]);
        catalog.decayedLog = Load(Woodland[5]); catalog.sitkaStump = Load(Woodland[6]);
        catalog.beechSapling = Load("Beech_Sapling_01");
        EditorUtility.SetDirty(catalog);
        AssetDatabase.SaveAssets();
        foreach (string path in new[] { "Assets/Scenes/ForestTest.unity", "Assets/Scenes/MixedSpeciesTest.unity" })
        {
            var scene = EditorSceneManager.OpenScene(path);
            ForestTreeSpawner spawner = UnityEngine.Object.FindFirstObjectByType<ForestTreeSpawner>();
            var serialized = new SerializedObject(spawner);
            WireSpawner(serialized);
            EditorSceneManager.SaveScene(scene);
        }
        ForestSceneBuilder.Validate();
        VerifyAssets();
        Debug.Log("SECTION_FIVE_ASSETS_PASS prefabs=13 lodModels=39 atlas=reviewed_v2 sourceFilesUnchanged=True");
    }

    public static void WireSpawner(SerializedObject spawner)
    {
        SectionFiveVisualCatalog catalog = AssetDatabase.LoadAssetAtPath<SectionFiveVisualCatalog>(CatalogPath);
        if (catalog == null) return;
        spawner.FindProperty("stumpPrefab").objectReferenceValue = catalog.sitkaStump;
        var sets = spawner.FindProperty("speciesVisualSets");
        for (int i = 0; i < sets.arraySize; i++)
        {
            var set = sets.GetArrayElementAtIndex(i);
            var species = set.FindPropertyRelative("species").objectReferenceValue as TreeSpeciesDefinition;
            if (species != null && species.SpeciesId == "beech")
                set.FindPropertyRelative("seedlingVisualPrefab").objectReferenceValue = catalog.beechSapling;
        }
        spawner.ApplyModifiedPropertiesWithoutUndo();
    }

    public static void VerifyAssets()
    {
        foreach (string asset in Floor.Concat(Woodland).Concat(new[] { "Beech_Sapling_01" }))
        {
            GameObject prefab = Load(asset);
            if (prefab == null || prefab.GetComponent<LODGroup>()?.GetLODs().Length != 3)
                throw new InvalidOperationException("Section 5 prefab/LODs missing: " + asset);
            if (prefab.GetComponentsInChildren<Collider>(true).Length != 0)
                throw new InvalidOperationException("Unexpected collision on display asset: " + asset);
        }
        string atlasPath = Art + "Forest_Floor_Detail/Textures/Broadleaf_Litter_BaseColorAlpha_v2.png";
        var atlas = (TextureImporter)AssetImporter.GetAtPath(atlasPath);
        if (!atlas.DoesSourceTextureHaveAlpha() || !atlas.sRGBTexture || !atlas.mipmapEnabled
            || !atlas.mipMapsPreserveCoverage || !atlas.alphaIsTransparency)
            throw new InvalidOperationException("Reviewed litter atlas alpha/mip setup is incorrect");
        foreach (Renderer renderer in Load(Floor[0]).GetComponentsInChildren<Renderer>(true))
            foreach (Material material in renderer.sharedMaterials)
                if (!material.IsKeywordEnabled("_ALPHATEST_ON") || material.GetFloat("_Cull") != 0f
                    || Mathf.Abs(material.GetFloat("_Cutoff") - 0.4f) > 0.001f
                    || AssetDatabase.GetAssetPath(material.GetTexture("_BaseMap")) != atlasPath)
                    throw new InvalidOperationException("Leaf litter is not using the repaired double-sided cutout atlas");
        foreach (Renderer renderer in Load(Floor[3]).GetComponentsInChildren<Renderer>(true))
            foreach (Material material in renderer.sharedMaterials)
                if (material.GetFloat("_Cull") != 0f || material.GetFloat("_AlphaClip") != 0f)
                    throw new InvalidOperationException("Twig/bark fragments must be opaque and double-sided");
        Debug.Log("SECTION_FIVE_MATERIALS_PASS repairedAtlas=True alphaCutoff=0.40 preserveMipCoverage=True colliders=0");
    }

    internal static void CopyPackage(string package, string[] assets, bool flatFbx)
    {
        string source = Source + package;
        string target = Art + package;
        foreach (string asset in assets)
            for (int lod = 0; lod < 3; lod++)
            {
                string relative = (flatFbx ? "FBX/" : asset + "/") + asset + "_LOD" + lod + ".fbx";
                Copy(Path.Combine(source, relative), Path.Combine(target, relative));
            }
        foreach (string texture in Directory.GetFiles(source + "/Textures", "*.png"))
        {
            if (Path.GetFileName(texture) == "Broadleaf_Litter_BaseColorAlpha.png") continue; // superseded atlas
            Copy(texture, target + "/Textures/" + Path.GetFileName(texture));
        }
        foreach (string document in new[] { "README.md", "README.txt", "Manifest.json", "Asset_Info.json",
            "Validation.json", "Geometry_Counts.md", "Asset_Summary.md", "Delivery_SHA256.json", "Atlas_Repair.json",
            "Defect_Validation.json", "Defect_Checks.json", "Visual_Review.json" })
            if (File.Exists(source + "/" + document))
                Copy(source + "/" + document, target + "/Docs/" + document);
    }

    private static void Copy(string source, string target)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(target));
        using (SHA256 hash = SHA256.Create())
        {
            byte[] bytes = File.ReadAllBytes(source);
            if (File.Exists(target))
            {
                if (!hash.ComputeHash(bytes).SequenceEqual(hash.ComputeHash(File.ReadAllBytes(target))))
                    throw new InvalidOperationException("Existing imported asset differs; inspect before overwrite: " + target);
            }
            else File.WriteAllBytes(target, bytes);
            if (!hash.ComputeHash(bytes).SequenceEqual(hash.ComputeHash(File.ReadAllBytes(target))))
                throw new InvalidOperationException("Import checksum mismatch: " + target);
        }
    }

    internal static void Build(string package, string asset, bool flatFbx)
    {
        Folder(Prefabs.TrimEnd('/'));
        Folder(Art + package + "/Materials");
        var root = new GameObject(asset);
        try
        {
            var lods = new LOD[3];
            for (int lod = 0; lod < 3; lod++)
            {
                string path = Art + package + "/" + (flatFbx ? "FBX/" : asset + "/") + asset + "_LOD" + lod + ".fbx";
                var importer = (ModelImporter)AssetImporter.GetAtPath(path);
                importer.importAnimation = false; importer.importCameras = false; importer.importLights = false;
                importer.addCollider = false; importer.materialImportMode = ModelImporterMaterialImportMode.ImportStandard;
                importer.materialLocation = ModelImporterMaterialLocation.InPrefab;
                importer.SaveAndReimport();
                var model = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(path));
                model.transform.SetParent(root.transform, false);
                Renderer[] renderers = model.GetComponentsInChildren<Renderer>(true);
                foreach (Renderer renderer in renderers)
                    renderer.sharedMaterials = renderer.sharedMaterials.Select(m => MaterialFor(package, m)).ToArray();
                bool tree = package == "SS_Bent_Refined_Set" || package == "SS_Cavity_Refined_Set";
                lods[lod] = new LOD((tree ? new[] { 0.55f, 0.20f, 0.04f } : new[] { 0.35f, 0.12f, 0.015f })[lod], renderers);
            }
            var group = root.AddComponent<LODGroup>();
            group.SetLODs(lods); group.RecalculateBounds();
            // Dither crossfade would need an additional shader keyword. Hard
            // LODs keep the cutout atlas predictable in this first import.
            group.fadeMode = LODFadeMode.None;
            PrefabUtility.SaveAsPrefabAsset(root, Prefabs + asset + ".prefab");
            foreach (LOD lod in group.GetLODs())
            {
                if (lod.renderers.Length == 0) throw new InvalidOperationException("Empty LOD: " + asset);
                foreach (Renderer renderer in lod.renderers)
                    if (renderer.sharedMaterials.Any(m => m == null || m.shader.name != "Universal Render Pipeline/Lit"))
                        throw new InvalidOperationException("Missing URP material: " + asset);
            }
        }
        finally { UnityEngine.Object.DestroyImmediate(root); }
    }

    private static Material MaterialFor(string package, Material original)
    {
        if (original == null) throw new InvalidOperationException("FBX material slot is empty");
        string name = original.name;
        string key = name.ToLowerInvariant();
        string texture = null;
        bool cutout = false, twoSided = false;
        float cutoff = 0.4f, smoothness = 0.06f;
        bool refinedTree = package == "SS_Bent_Refined_Set" || package == "SS_Cavity_Refined_Set";
        if (package == "Forest_Floor_Detail")
        {
            twoSided = true;
            if (key.Contains("litter")) { texture = "Broadleaf_Litter_BaseColorAlpha_v2.png"; cutout = true; }
            else if (key.Contains("deadwood")) texture = "Woodland_DecayedWood_Albedo.png";
            else throw new InvalidOperationException("Unmapped Section 5 material: " + name);
        }
        else if (package == "Woodland_Assets")
        {
            if (key.Contains("endgrain") || key.Contains("cut")) texture = "Woodland_Endgrain_Albedo.png";
            else if (key.Contains("decay")) texture = "Woodland_DecayedWood_Albedo.png";
            else if (key.Contains("bark")) texture = "Woodland_Sitka_Bark_Albedo.png";
            else if (key.Contains("moss")) { texture = "Woodland_Moss_Albedo.png"; twoSided = true; }
            else if (key.Contains("leaf") || key.Contains("leaves")) { texture = "Woodland_Leaf_Albedo.png"; twoSided = true; }
            // Ordinary stem/cane/interior colours are supplied by the FBX.
        }
        else if (package == "Woodland_Grass_Rush")
        {
            if (key.Contains("grass") && !key.Contains("dry")) texture = "Woodland_Leaf_Albedo.png";
            twoSided = true; // geometric blade/stem silhouettes, no alpha cards
        }
        else if (refinedTree)
        {
            if (key == "ss_benchmark_bark") { texture = "Sitka_Bark_BaseColor_v2.png"; smoothness = 0.15f; }
            else if (key == "ss_benchmark_needles")
            {
                texture = "Sitka_NeedleSprigs_BaseColorAlpha.png";
                cutout = true; twoSided = true; cutoff = 0.38f; smoothness = 0.14f;
            }
            else if (key == "ss_cavity_decayedwood")
            { texture = "Sitka_Cavity_DecayedWood_BaseColor.png"; smoothness = 0.02f; }
            else throw new InvalidOperationException("Unmapped refined-tree material: " + name);
        }
        else
        {
            if (key.Contains("bark")) texture = "Beech_Bark_Albedo.png";
            twoSided = key.Contains("leaf") || key.Contains("leaves") || key.Contains("foliage");
        }
        string path = Art + package + "/Materials/" + name + ".mat";
        Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (material == null)
        {
            material = new Material(Shader.Find("Universal Render Pipeline/Lit")) { name = name, enableInstancing = true };
            AssetDatabase.CreateAsset(material, path);
        }
        material.SetColor("_BaseColor", original.HasProperty("_Color") ? original.color : Color.white);
        if (texture != null)
        {
            var map = AssetDatabase.LoadAssetAtPath<Texture2D>(Art + package + "/Textures/" + texture);
            if (map == null) throw new InvalidOperationException("Missing texture " + texture);
            material.SetTexture("_BaseMap", map);
            material.SetColor("_BaseColor", Color.white);
            var importer = (TextureImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(map));
            if (cutout && !importer.mipMapsPreserveCoverage)
            {
                importer.mipmapEnabled = true; importer.mipMapsPreserveCoverage = true;
                importer.alphaTestReferenceValue = cutoff; importer.alphaIsTransparency = true;
                importer.sRGBTexture = true; importer.SaveAndReimport();
            }
        }
        if (refinedTree && key == "ss_benchmark_bark")
        {
            string normalPath = Art + package + "/Textures/Sitka_Bark_Normal_v2.png";
            var normalImporter = (TextureImporter)AssetImporter.GetAtPath(normalPath);
            if (normalImporter.textureType != TextureImporterType.NormalMap)
            { normalImporter.textureType = TextureImporterType.NormalMap; normalImporter.SaveAndReimport(); }
            material.SetTexture("_BumpMap", AssetDatabase.LoadAssetAtPath<Texture2D>(normalPath));
            material.SetFloat("_BumpScale", 0.7f); material.EnableKeyword("_NORMALMAP");
            // The shared metallic/smoothness PNG is RGB-only, with no alpha
            // roughness data. Use matte bark rather than reintroducing shine.
            material.SetTexture("_MetallicGlossMap", null); material.DisableKeyword("_METALLICSPECGLOSSMAP");
        }
        material.SetFloat("_Smoothness", smoothness); material.SetFloat("_Metallic", 0f);
        material.SetFloat("_Cull", twoSided ? 0f : 2f); material.doubleSidedGI = twoSided;
        material.SetFloat("_AlphaClip", cutout ? 1f : 0f); material.SetFloat("_Cutoff", cutoff);
        if (cutout) material.EnableKeyword("_ALPHATEST_ON"); else material.DisableKeyword("_ALPHATEST_ON");
        material.renderQueue = cutout ? 2450 : 2000;
        material.SetOverrideTag("RenderType", cutout ? "TransparentCutout" : "Opaque");
        EditorUtility.SetDirty(material);
        return material;
    }

    private static GameObject Load(string asset) => AssetDatabase.LoadAssetAtPath<GameObject>(Prefabs + asset + ".prefab");
    private static void Folder(string path)
    {
        path = path.Replace('\\', '/');
        if (AssetDatabase.IsValidFolder(path)) return;
        string parent = Path.GetDirectoryName(path).Replace('\\', '/');
        Folder(parent);
        AssetDatabase.CreateFolder(parent, Path.GetFileName(path));
    }
}
