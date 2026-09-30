using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

// Builds Unity materials and LODGroup prefabs for the delivered forestry art
// (crop-tree pruning states, Sitka benchmark, ring-barked, coarse-branch and
// ground additions) and wires the result into the tree spawner and Scenario
// One. Every material slot is mapped explicitly from the FBX material name;
// simulation rules never depend on mesh names or bounds.
public static class ForestryAssetSetup
{
    private const string Art = "Assets/ForestPrototype/Art";
    private const string PruningRoot = Art + "/CropTreePruning";
    private const string BenchmarkRoot = Art + "/SitkaMatureBenchmark";
    private const string RingRoot = Art + "/SitkaRingBarked";
    private const string CoarseRoot = Art + "/CoarseBranchSet";
    private const string GroundRoot = Art + "/ForestryGround";
    private const string Prefabs = "Assets/ForestPrototype/Prefabs/Forestry";

    // Unpruned, Low recent/healed, Medium recent/healed, High recent/healed.
    // Matches the delivered FBX naming exactly.
    private static readonly string[] StateKeys =
    {
        "Unpruned", "LowPruned_Recent", "LowPruned_Healed",
        "MediumPruned_Recent", "MediumPruned_Healed",
        "HighPruned_Recent", "HighPruned_Healed"
    };

    private enum Family { Crop, Benchmark, Ground, Coarse, Ring }
    private enum Group { SitkaMature, SitkaPole, BeechMature, OakMature, OakPole }

    private struct TreeBase
    {
        public string name;
        public string modelDir;
        public float height;
        public Family family;
        public Group group;
    }

    // Authored heights from Crop_Tree_Pruning/Coverage.md and each package's
    // Manifest/Geometry_Counts. Used only to normalise prefabs to unit height.
    private static readonly TreeBase[] TreeBases =
    {
        new TreeBase { name = "Sitka_Mature_Benchmark_01", modelDir = BenchmarkRoot + "/FBX", height = 26f, family = Family.Benchmark, group = Group.SitkaMature },
        new TreeBase { name = "Sitka_Mature_01", modelDir = PruningRoot + "/Sitka_Mature_01", height = 28.011f, family = Family.Crop, group = Group.SitkaMature },
        new TreeBase { name = "Sitka_Mature_02", modelDir = PruningRoot + "/Sitka_Mature_02", height = 26f, family = Family.Crop, group = Group.SitkaMature },
        new TreeBase { name = "Sitka_Mature_03", modelDir = PruningRoot + "/Sitka_Mature_03", height = 24f, family = Family.Crop, group = Group.SitkaMature },
        new TreeBase { name = "Sitka_OldLarge_01", modelDir = PruningRoot + "/Sitka_OldLarge_01", height = 32f, family = Family.Crop, group = Group.SitkaMature },
        new TreeBase { name = "Sitka_OldLarge_02", modelDir = PruningRoot + "/Sitka_OldLarge_02", height = 30f, family = Family.Crop, group = Group.SitkaMature },
        new TreeBase { name = "Sitka_Pole_01", modelDir = PruningRoot + "/Sitka_Pole_01", height = 8f, family = Family.Crop, group = Group.SitkaPole },
        new TreeBase { name = "Sitka_Young_02", modelDir = PruningRoot + "/Sitka_Young_02", height = 10.5f, family = Family.Crop, group = Group.SitkaPole },
        new TreeBase { name = "Beech_Mature_01", modelDir = PruningRoot + "/Beech_Mature_01", height = 25.057f, family = Family.Crop, group = Group.BeechMature },
        new TreeBase { name = "Beech_Mature_02", modelDir = PruningRoot + "/Beech_Mature_02", height = 23.009f, family = Family.Crop, group = Group.BeechMature },
        new TreeBase { name = "Sessile_Oak_Mature_01", modelDir = PruningRoot + "/Sessile_Oak_Mature_01", height = 19.992f, family = Family.Crop, group = Group.OakMature },
        new TreeBase { name = "Sessile_Oak_Mature_02", modelDir = PruningRoot + "/Sessile_Oak_Mature_02", height = 22.009f, family = Family.Crop, group = Group.OakMature },
        new TreeBase { name = "Sessile_Oak_Young_01", modelDir = PruningRoot + "/Sessile_Oak_Young_01", height = 7.994f, family = Family.Crop, group = Group.OakPole }
    };

    private struct PropAsset
    {
        public string asset;
        public Family family;
        public string modelDir;
        public float height;
    }

    // Ground props stay at natural scale; tree-family props normalise like the
    // pruning bases. These are presentation assets awaiting an authored spawn
    // decision (see Docs/ForestryArtIntegration.md).
    private static readonly PropAsset[] Props =
    {
        new PropAsset { asset = "SS_WindthrowBase_Fresh_01", family = Family.Ground, modelDir = GroundRoot + "/FBX", height = 3.4f },
        new PropAsset { asset = "SS_WindthrowBase_Weathered_01", family = Family.Ground, modelDir = GroundRoot + "/FBX", height = 3.4f },
        new PropAsset { asset = "SS_Brash_Green_01", family = Family.Ground, modelDir = GroundRoot + "/FBX", height = 1f },
        new PropAsset { asset = "SS_Brash_Green_02", family = Family.Ground, modelDir = GroundRoot + "/FBX", height = 1f },
        new PropAsset { asset = "SS_Brash_Dry_01", family = Family.Ground, modelDir = GroundRoot + "/FBX", height = 1f },
        new PropAsset { asset = "SS_Brash_Dry_02", family = Family.Ground, modelDir = GroundRoot + "/FBX", height = 1f },
        new PropAsset { asset = "SS_Defect_CoarseBranch_Young_01", family = Family.Coarse, modelDir = CoarseRoot + "/FBX", height = 11.5f },
        new PropAsset { asset = "SS_Defect_CoarseBranch_Mature_01", family = Family.Coarse, modelDir = CoarseRoot + "/FBX", height = 25f },
        new PropAsset { asset = "SS_Defect_CoarseBranch_PostMature_01", family = Family.Coarse, modelDir = CoarseRoot + "/FBX", height = 31f },
        new PropAsset { asset = "Sitka_RingBarked_01_Fresh", family = Family.Ring, modelDir = RingRoot + "/FBX", height = 26f },
        new PropAsset { asset = "Sitka_RingBarked_01_Dead", family = Family.Ring, modelDir = RingRoot + "/FBX", height = 26f }
    };

    [MenuItem("Tools/Forest Prototype/Build Crop-Tree Pruning Art")]
    public static void BuildPruningArt()
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();
        foreach (TreeBase treeBase in TreeBases)
        {
            ConfigureModels(treeBase.modelDir, treeBase.name);
            EnsureFamilyMaterials(treeBase.family);
            for (int state = 0; state < StateKeys.Length; state++)
                BuildTreeStatePrefab(treeBase, state);
        }
        AssetDatabase.SaveAssets();
        WireScenes();
        ValidateTreePrefabs();
        ForestSceneBuilder.Validate();
        Debug.Log($"CROP_TREE_PRUNING_ART_BUILT bases={TreeBases.Length} prefabs={TreeBases.Length * StateKeys.Length} seconds={watch.Elapsed.TotalSeconds:0}");
    }

    [MenuItem("Tools/Forest Prototype/Build Forestry Ground & Defect Art")]
    public static void BuildGroundArt()
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();
        foreach (PropAsset prop in Props)
        {
            ConfigureModels(prop.modelDir, prop.asset);
            EnsureFamilyMaterials(prop.family);
            BuildPropPrefab(prop);
        }
        AssetDatabase.SaveAssets();
        WireScenes();
        ValidatePropPrefabs();
        ForestSceneBuilder.Validate();
        Debug.Log($"FORESTRY_GROUND_ART_BUILT props={Props.Length} seconds={watch.Elapsed.TotalSeconds:0}");
    }

    [MenuItem("Tools/Forest Prototype/Build All Forestry Art")]
    public static void BuildAllArt()
    {
        BuildPruningArt();
        BuildGroundArt();
    }

    // Shared with ForestSceneBuilder so a rebuilt scene links the same art.
    public static void WireSpawnerAndScenario(SerializedObject spawner, SerializedObject scenario)
    {
        if (spawner == null)
            return;
        SetBaseArray(spawner.FindProperty("defaultMaturePruningBases"), Group.SitkaMature);
        SetBaseArray(spawner.FindProperty("defaultPolePruningBases"), Group.SitkaPole);
        SerializedProperty sets = spawner.FindProperty("speciesVisualSets");
        for (int i = 0; i < sets.arraySize; i++)
        {
            SerializedProperty element = sets.GetArrayElementAtIndex(i);
            var species = element.FindPropertyRelative("species").objectReferenceValue as TreeSpeciesDefinition;
            string id = species != null ? species.SpeciesId : "";
            if (id == "beech")
                SetBaseArray(element.FindPropertyRelative("maturePruningBases"), Group.BeechMature);
            else if (id == "sessile-oak")
            {
                SetBaseArray(element.FindPropertyRelative("maturePruningBases"), Group.OakMature);
                SetBaseArray(element.FindPropertyRelative("polePruningBases"), Group.OakPole);
            }
        }
        spawner.ApplyModifiedPropertiesWithoutUndo();
        if (scenario == null)
            return;
        scenario.FindProperty("fellingResidueGreenPrefab").objectReferenceValue = PropPrefab("SS_Brash_Green_01");
        scenario.FindProperty("fellingResidueGreenAltPrefab").objectReferenceValue = PropPrefab("SS_Brash_Green_02");
        scenario.FindProperty("fellingResidueDryPrefab").objectReferenceValue = PropPrefab("SS_Brash_Dry_01");
        scenario.FindProperty("fellingResidueDryAltPrefab").objectReferenceValue = PropPrefab("SS_Brash_Dry_02");
        scenario.ApplyModifiedPropertiesWithoutUndo();
    }

    // Diagnostics: prints every embedded FBX material slot name so the
    // explicit mapping table can be completed from ground truth.
    public static void DumpFbxMaterials()
    {
        var names = new SortedSet<string>();
        foreach (string dir in new[]
        {
            PruningRoot, BenchmarkRoot + "/FBX", RingRoot + "/FBX", CoarseRoot + "/FBX", GroundRoot + "/FBX"
        })
        {
            foreach (string file in Directory.GetFiles(dir, "*.fbx", SearchOption.AllDirectories))
            {
                var model = AssetDatabase.LoadAssetAtPath<GameObject>(file.Replace('\\', '/'));
                if (model == null)
                    continue;
                foreach (Renderer renderer in model.GetComponentsInChildren<Renderer>(true))
                    foreach (Material material in renderer.sharedMaterials)
                        if (material != null)
                            names.Add(material.name);
            }
        }
        foreach (string name in names)
            Debug.Log("FBX_MATERIAL_SLOT " + name);
        Debug.Log("FBX_MATERIAL_SLOTS " + names.Count);
    }

    // --- importers -------------------------------------------------------

    private static void ConfigureModels(string modelDir, string prefix)
    {
        foreach (string file in Directory.GetFiles(modelDir, prefix + "*.fbx"))
        {
            string path = file.Replace('\\', '/');
            if (AssetImporter.GetAtPath(path) is ModelImporter importer)
            {
                bool changed = importer.importAnimation || importer.addCollider || importer.isReadable
                    || importer.importCameras || importer.importLights
                    || importer.materialImportMode != ModelImporterMaterialImportMode.ImportStandard
                    || importer.materialLocation != ModelImporterMaterialLocation.InPrefab;
                if (!changed)
                    continue;
                importer.importAnimation = false;
                importer.addCollider = false;
                importer.isReadable = false;
                importer.importCameras = false;
                importer.importLights = false;
                importer.materialImportMode = ModelImporterMaterialImportMode.ImportStandard;
                importer.materialLocation = ModelImporterMaterialLocation.InPrefab;
                importer.SaveAndReimport();
            }
        }
        ConfigureTextures(TexturesFolder(ForModelDir(modelDir)));
    }

    private static Family ForModelDir(string modelDir)
    {
        foreach (TreeBase treeBase in TreeBases)
            if (treeBase.modelDir == modelDir)
                return treeBase.family;
        foreach (PropAsset prop in Props)
            if (prop.modelDir == modelDir)
                return prop.family;
        return Family.Crop;
    }

    private static void ConfigureTextures(string textureFolder)
    {
        if (!Directory.Exists(textureFolder))
            return;
        foreach (string file in Directory.GetFiles(textureFolder, "*.png"))
        {
            string path = file.Replace('\\', '/');
            if (!(AssetImporter.GetAtPath(path) is TextureImporter importer))
                continue;
            string name = Path.GetFileName(path);
            TextureImporterType type = name.Contains("Normal")
                ? TextureImporterType.NormalMap : TextureImporterType.Default;
            bool linear = name.Contains("Normal") || name.Contains("MetallicSmoothness");
            if (importer.textureType == type && importer.sRGBTexture == !linear)
                continue;
            importer.textureType = type;
            importer.sRGBTexture = !linear;
            importer.SaveAndReimport();
        }
    }

    // --- materials -------------------------------------------------------

    private static void EnsureFamilyMaterials(Family family)
    {
        switch (family)
        {
            case Family.Crop:
                foreach (string name in new[]
                {
                    "Sitka_Bark", "Sitka_Pole_Bark", "Sitka_Needles", "Beech_Bark", "Beech_Leaf",
                    "Oak_Bark", "Oak_Leaf_1", "Oak_Leaf_2", "Oak_Leaf_3", "Oak_Leaf_4",
                    "Pruning_ExposedWood", "Pruning_HealedScar"
                })
                    EnsureMaterial(family, name);
                break;
            case Family.Benchmark:
                foreach (string name in new[]
                    { "SS_Benchmark_Bark", "SS_Benchmark_Needles", "SS_Benchmark_CutWood", "SS_Benchmark_WeatheredScar" })
                    EnsureMaterial(family, name);
                break;
            case Family.Ground:
                foreach (string name in new[]
                {
                    "SS_Benchmark_Bark", "SS_Benchmark_Needles", "SS_Benchmark_CutWood",
                    "Forest_RootPlate_Moss", "Forest_RootPlate_Soil", "Forest_RootPlate_SoilLighter",
                    "Forest_Weathered_RootWood"
                })
                    EnsureMaterial(family, name);
                break;
            case Family.Coarse:
                foreach (string name in new[] { "SS_Benchmark_Bark", "SS_Benchmark_Needles" })
                    EnsureMaterial(family, name);
                break;
            default:
                foreach (string name in new[]
                {
                    "SS_Benchmark_Bark", "SS_Benchmark_Needles",
                    "SS_Ring_ExposedWood", "SS_Ring_WeatheredWood", "SS_Ring_DeadBranches"
                })
                    EnsureMaterial(family, name);
                break;
        }
    }

    private static Material EnsureMaterial(Family family, string name)
    {
        string path = MaterialsFolder(family) + "/" + name + ".mat";
        Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (material != null)
        {
            ConfigureMaterial(material, family, name);
            EditorUtility.SetDirty(material);
            return material;
        }
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null)
            throw new InvalidOperationException("URP Lit shader missing.");
        Directory.CreateDirectory(MaterialsFolder(family));
        material = new Material(shader) { name = name, enableInstancing = true };
        ConfigureMaterial(material, family, name);
        AssetDatabase.CreateAsset(material, path);
        return material;
    }

    private static void ConfigureMaterial(Material material, Family family, string name)
    {
        Texture2D Tex(string file) => AssetDatabase.LoadAssetAtPath<Texture2D>(TexturesFolder(family) + "/" + file);
        switch (name)
        {
            case "Sitka_Bark":
                BaseMap(material, Tex("Sitka_Bark_Albedo.png")); Opaque(material); break;
            case "Sitka_Pole_Bark":
                BaseMap(material, Tex("Sitka_Pole_Bark_Albedo.png")); Opaque(material); break;
            case "Sitka_Needles":
                BaseMap(material, Tex("Sitka_Needles_Albedo.png")); CutoutBoth(material, 0.35f); break;
            case "Beech_Bark":
                BaseMap(material, Tex("Beech_Bark_Albedo.png")); Opaque(material); break;
            case "Beech_Leaf":
                material.SetColor("_BaseColor", new Color(0.11f, 0.24f, 0.05f));
                material.SetColor("_Color", new Color(0.11f, 0.24f, 0.05f));
                CutoutBoth(material, 0.35f); break;
            case "Oak_Bark":
                BaseMap(material, Tex("Oak_Bark_Albedo.png")); Opaque(material); break;
            case "Oak_Leaf_1":
                LeafColor(material, new Color(0.085f, 0.19f, 0.028f)); break;
            case "Oak_Leaf_2":
                LeafColor(material, new Color(0.14f, 0.27f, 0.045f)); break;
            case "Oak_Leaf_3":
                LeafColor(material, new Color(0.19f, 0.32f, 0.055f)); break;
            case "Oak_Leaf_4":
                LeafColor(material, new Color(0.105f, 0.22f, 0.038f)); break;
            case "Pruning_ExposedWood":
                BaseMap(material, Tex("Pruning_ExposedWood.png")); Opaque(material); break;
            case "Pruning_HealedScar":
                BaseMap(material, Tex("Pruning_HealedScar.png")); Opaque(material); break;
            case "SS_Benchmark_Bark":
                BaseMap(material, Tex("Sitka_Bark_BaseColor_v2.png"));
                Texture2D barkNormal = Tex("Sitka_Bark_Normal_v2.png");
                if (barkNormal != null) NormalMap(material, barkNormal);
                Opaque(material, 0.15f);
                material.SetTexture("_MetallicGlossMap", null);
                material.SetFloat("_Metallic", 0f);
                material.DisableKeyword("_METALLICSPECGLOSSMAP");
                break;
            case "SS_Benchmark_Needles":
                BaseMap(material, Tex("Sitka_NeedleSprigs_BaseColorAlpha.png")); CutoutBoth(material, 0.38f); break;
            case "SS_Benchmark_CutWood":
                BaseMap(material, Tex("Sitka_CutWood_BaseColor.png")); Opaque(material); break;
            case "SS_Benchmark_WeatheredScar":
                Tint(material, new Color(0.44f, 0.40f, 0.33f)); Opaque(material); break;
            case "Forest_RootPlate_Moss":
                BaseMap(material, Tex("Woodland_Moss_Albedo.png")); CutoutBoth(material, 0.35f); break;
            case "Forest_RootPlate_Soil":
                BaseMap(material, Tex("Forest_RootPlate_Soil_BaseColor.png")); Opaque(material); break;
            case "Forest_RootPlate_SoilLighter":
                BaseMap(material, Tex("Forest_RootPlate_Soil_BaseColor.png"));
                Tint(material, new Color(1.25f, 1.2f, 1.05f)); Opaque(material); break;
            case "Forest_Weathered_RootWood":
                BaseMap(material, Tex("Woodland_DecayedWood_Albedo.png")); Opaque(material); break;
            case "SS_Ring_ExposedWood":
                BaseMap(material, Tex("Sitka_ExposedWood_BaseColor.png"));
                Texture2D woodNormal = Tex("Sitka_ExposedWood_Normal.png");
                if (woodNormal != null) NormalMap(material, woodNormal);
                Opaque(material); break;
            case "SS_Ring_WeatheredWood":
                Tint(material, new Color(0.42f, 0.38f, 0.32f)); Opaque(material); break;
            case "SS_Ring_DeadBranches":
                Tint(material, new Color(0.35f, 0.32f, 0.27f)); Opaque(material); break;
            default:
                throw new InvalidOperationException("No material setup for '" + name + "' in " + family + ".");
        }
        EditorUtility.SetDirty(material);
    }

    private static void LeafColor(Material material, Color color)
    {
        Tint(material, color);
        CutoutBoth(material, 0.35f);
    }

    private static void Tint(Material material, Color color)
    {
        material.SetColor("_BaseColor", color);
        material.SetColor("_Color", color);
    }

    private static void BaseMap(Material material, Texture2D texture)
    {
        if (texture == null)
            return;
        material.SetTexture("_BaseMap", texture);
        material.SetTexture("_MainTex", texture);
    }

    private static void NormalMap(Material material, Texture2D texture)
    {
        material.SetTexture("_BumpMap", texture);
        material.SetFloat("_BumpScale", 0.7f);
        material.EnableKeyword("_NORMALMAP");
    }

    private static void Opaque(Material material, float smoothness = 0.05f)
    {
        material.DisableKeyword("_ALPHATEST_ON");
        material.SetFloat("_Surface", 0f);
        material.SetFloat("_AlphaClip", 0f);
        material.SetFloat("_SrcBlend", 1f);
        material.SetFloat("_DstBlend", 0f);
        material.SetFloat("_ZWrite", 1f);
        material.SetFloat("_Cull", 2f);
        material.SetFloat("_Smoothness", smoothness);
        material.SetOverrideTag("RenderType", "Opaque");
        material.renderQueue = -1;
    }

    private static void CutoutBoth(Material material, float cutoff)
    {
        material.EnableKeyword("_ALPHATEST_ON");
        material.SetFloat("_Surface", 0f);
        material.SetFloat("_AlphaClip", 1f);
        material.SetFloat("_Cutoff", cutoff);
        material.SetFloat("_AlphaToMask", 1f);
        material.SetFloat("_SrcBlend", 1f);
        material.SetFloat("_DstBlend", 0f);
        material.SetFloat("_ZWrite", 1f);
        material.SetFloat("_Cull", 0f);
        material.SetFloat("_Smoothness", 0.05f);
        material.SetOverrideTag("RenderType", "TransparentCutout");
        material.renderQueue = 2450;
        material.doubleSidedGI = true;
    }

    // Explicit FBX material name -> canonical project material name. Unity may
    // suffix duplicates with ".001"; that is stripped before lookup.
    private static string CanonicalMaterial(string embeddedName, Family family)
    {
        string key = embeddedName ?? "";
        int suffix = key.LastIndexOf('.');
        if (suffix > 0 && int.TryParse(key.Substring(suffix + 1), out _))
            key = key.Substring(0, suffix);
        switch (key)
        {
            case "Sitka_Bark":
            case "Sitka_Bark_Albedo":
            case "RingTrunk":
                return family == Family.Crop ? "Sitka_Bark" : "SS_Benchmark_Bark";
            case "Sitka_Pole_Bark":
            case "Sitka_Pole_Bark_Albedo":
                return "Sitka_Pole_Bark";
            case "Sitka_Needles":
            case "Sitka_Needles_Albedo":
            case "Sitka_NeedleSprigs_BaseColorAlpha":
            case "Sitka_Pole_Foliage":
            case "Foliage":
            case "RingFoliage":
                return family == Family.Crop ? "Sitka_Needles" : "SS_Benchmark_Needles";
            case "Beech_Bark":
            case "Beech_Bark_Albedo":
            case "Beech_SmoothGreyBark":
                return "Beech_Bark";
            case "Beech_Leaves":
            case "Beech_Leaf_0":
            case "Beech_Leaf_1":
            case "Beech_Leaf_2":
            case "Beech_Leaf_3":
                return "Beech_Leaf";
            case "Oak_Bark":
            case "Oak_Bark_Albedo":
            case "Oak_Furrowed_Bark":
                return "Oak_Bark";
            case "Oak_Leaves":
            case "Oak_Leaf_1": return "Oak_Leaf_1";
            case "Oak_Leaf_2": return "Oak_Leaf_2";
            case "Oak_Leaf_3": return "Oak_Leaf_3";
            case "Oak_Leaf_4": return "Oak_Leaf_4";
            case "Pruning_ExposedWood":
            case "Pruning_ExposedWood.png":
                return "Pruning_ExposedWood";
            case "Pruning_HealedScar":
            case "Pruning_HealedScar.png":
            case "PruningScars":
                return family == Family.Crop ? "Pruning_HealedScar" : "SS_Benchmark_WeatheredScar";
            case "SS_Benchmark_Bark":
            case "RingBranches":
                return "SS_Benchmark_Bark";
            case "SS_Benchmark_Needles":
                return "SS_Benchmark_Needles";
            case "SS_Benchmark_CutWood":
                return family == Family.Ground || family == Family.Benchmark ? "SS_Benchmark_CutWood" : "SS_Benchmark_CutWood";
            case "SS_Benchmark_WeatheredScar":
                return "SS_Benchmark_WeatheredScar";
            case "Forest_RootPlate_Moss":
                return "Forest_RootPlate_Moss";
            case "Forest_RootPlate_Soil":
            case "Forest_RootPlate_Soil_BaseColor.png":
                return "Forest_RootPlate_Soil";
            case "Forest_RootPlate_SoilLighter":
                return "Forest_RootPlate_SoilLighter";
            case "Forest_Weathered_RootWood":
                return "Forest_Weathered_RootWood";
            case "SS_Ring_DamagedBarkEdge":
            case "SS_Ring_ExposedLongitudinalWood":
                return "SS_Ring_ExposedWood";
            case "SS_Ring_WeatheredBarkEdge":
            case "SS_Ring_WeatheredExposedWood":
                return "SS_Ring_WeatheredWood";
            case "SS_Ring_DeadBranches":
                return "SS_Ring_DeadBranches";
            default:
                throw new InvalidOperationException("Unmapped FBX material '" + embeddedName + "' (family " + family + ").");
        }
    }

    private static Material ResolveMaterial(string embeddedName, Family family)
    {
        return EnsureMaterial(family, CanonicalMaterial(embeddedName, family));
    }

    // --- prefabs ---------------------------------------------------------

    private static void BuildTreeStatePrefab(TreeBase treeBase, int state)
    {
        string stateKey = StateKeys[state];
        string[] models =
        {
            $"{treeBase.modelDir}/{treeBase.name}_{stateKey}_LOD0.fbx",
            $"{treeBase.modelDir}/{treeBase.name}_{stateKey}_LOD1.fbx",
            $"{treeBase.modelDir}/{treeBase.name}_{stateKey}_LOD2.fbx"
        };
        BuildLodPrefab($"{treeBase.name}_{stateKey}", models,
            $"{Prefabs}/Pruning/{treeBase.name}/{treeBase.name}_{stateKey}.prefab",
            treeBase.height, true, treeBase.family);
    }

    private static void BuildPropPrefab(PropAsset prop)
    {
        string[] models =
        {
            $"{prop.modelDir}/{prop.asset}_LOD0.fbx",
            $"{prop.modelDir}/{prop.asset}_LOD1.fbx",
            $"{prop.modelDir}/{prop.asset}_LOD2.fbx"
        };
        bool treeFamily = prop.family != Family.Ground;
        BuildLodPrefab(prop.asset, models, PropPrefabPath(prop.asset), prop.height, treeFamily, prop.family);
    }

    private static void BuildLodPrefab(string label, string[] modelPaths, string prefabPath,
        float authoredHeight, bool normalize, Family family)
    {
        foreach (string modelPath in modelPaths)
            if (AssetDatabase.LoadAssetAtPath<GameObject>(modelPath) == null)
                throw new InvalidOperationException("Missing delivered model " + modelPath);
        Directory.CreateDirectory(Path.GetDirectoryName(prefabPath) ?? Prefabs);
        var root = new GameObject(label);
        try
        {
            var lods = new LOD[3];
            float[] transitions = { 0.55f, 0.20f, 0.04f };
            for (int lod = 0; lod < 3; lod++)
            {
                GameObject source = AssetDatabase.LoadAssetAtPath<GameObject>(modelPaths[lod]);
                GameObject instance = PrefabUtility.InstantiatePrefab(source) as GameObject;
                if (instance == null)
                    throw new InvalidOperationException("Could not instantiate " + modelPaths[lod]);
                instance.name = "LOD" + lod;
                instance.transform.SetParent(root.transform, false);
                instance.transform.localPosition = Vector3.zero;
                instance.transform.localRotation = Quaternion.identity;
                instance.transform.localScale = normalize ? Vector3.one / authoredHeight : Vector3.one;
                Renderer[] renderers = instance.GetComponentsInChildren<Renderer>(true);
                if (renderers.Length == 0)
                    throw new InvalidOperationException("No renderers in " + modelPaths[lod]);
                foreach (Renderer renderer in renderers)
                {
                    Material[] slots = renderer.sharedMaterials;
                    for (int slot = 0; slot < slots.Length; slot++)
                        if (slots[slot] != null)
                            slots[slot] = ResolveMaterial(slots[slot].name, family);
                    renderer.sharedMaterials = slots;
                }
                lods[lod] = new LOD(transitions[lod], renderers) { fadeTransitionWidth = 0.1f };
            }
            var group = root.AddComponent<LODGroup>();
            group.fadeMode = LODFadeMode.CrossFade;
            group.animateCrossFading = false;
            group.SetLODs(lods);
            group.RecalculateBounds();
            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(root);
        }
    }

    private static string PropPrefabPath(string asset)
    {
        return (asset.StartsWith("SS_Brash", StringComparison.Ordinal)
            || asset.StartsWith("SS_Windthrow", StringComparison.Ordinal)
                ? $"{Prefabs}/Ground/{asset}.prefab"
                : $"{Prefabs}/Defects/{asset}.prefab");
    }

    private static GameObject PropPrefab(string asset)
    {
        return AssetDatabase.LoadAssetAtPath<GameObject>(PropPrefabPath(asset));
    }

    private static string StatePrefabPath(TreeBase treeBase, int state)
    {
        return $"{Prefabs}/Pruning/{treeBase.name}/{treeBase.name}_{StateKeys[state]}.prefab";
    }

    // --- scene wiring ----------------------------------------------------

    private static void WireScenes()
    {
        foreach (string scenePath in new[] { "Assets/Scenes/ForestTest.unity", "Assets/Scenes/MixedSpeciesTest.unity" })
        {
            if (!File.Exists(scenePath))
                continue;
            Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            ForestTreeSpawner spawner = null;
            ScenarioOneManager scenario = null;
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                spawner = spawner != null ? spawner : root.GetComponentInChildren<ForestTreeSpawner>(true);
                scenario = scenario != null ? scenario : root.GetComponentInChildren<ScenarioOneManager>(true);
            }
            if (spawner == null)
                throw new InvalidOperationException("No tree spawner in " + scenePath);
            var spawnerSerialized = new SerializedObject(spawner);
            var scenarioSerialized = scenario != null ? new SerializedObject(scenario) : null;
            WireSpawnerAndScenario(spawnerSerialized, scenarioSerialized);
            EditorSceneManager.SaveScene(scene);
        }
    }

    private static void SetBaseArray(SerializedProperty property, Group group)
    {
        var names = new List<string>();
        foreach (TreeBase treeBase in TreeBases)
            if (treeBase.group == group)
                names.Add(treeBase.name);
        property.arraySize = names.Count;
        for (int i = 0; i < names.Count; i++)
        {
            SerializedProperty element = property.GetArrayElementAtIndex(i);
            element.FindPropertyRelative("label").stringValue = names[i];
            SerializedProperty states = element.FindPropertyRelative("pruningStates");
            states.arraySize = StateKeys.Length;
            for (int state = 0; state < StateKeys.Length; state++)
            {
                string prefabPath = $"{Prefabs}/Pruning/{names[i]}/{names[i]}_{StateKeys[state]}.prefab";
                states.GetArrayElementAtIndex(state).objectReferenceValue =
                    AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            }
        }
    }

    // --- validation ------------------------------------------------------

    private static void ValidateTreePrefabs()
    {
        foreach (TreeBase treeBase in TreeBases)
            for (int state = 0; state < StateKeys.Length; state++)
                ValidatePrefab(StatePrefabPath(treeBase, state), true);
    }

    private static void ValidatePropPrefabs()
    {
        foreach (PropAsset prop in Props)
            ValidatePrefab(PropPrefabPath(prop.asset), false);
    }

    private static void ValidatePrefab(string path, bool normalized)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (prefab == null)
            throw new InvalidOperationException("Missing prefab " + path);
        if (!(prefab.GetComponent<LODGroup>() is LODGroup group) || group.GetLODs().Length != 3)
            throw new InvalidOperationException("Prefab lacks three LOD levels: " + path);
        foreach (LOD lod in group.GetLODs())
            foreach (Renderer renderer in lod.renderers)
                foreach (Material material in renderer.sharedMaterials)
                    if (material == null || material.shader == null)
                        throw new InvalidOperationException("Prefab has a missing material: " + path);
        GameObject instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        if (instance == null)
            throw new InvalidOperationException("Could not instantiate " + path);
        try
        {
            Renderer[] firstLod = instance.GetComponent<LODGroup>().GetLODs()[0].renderers;
            Bounds bounds = firstLod[0].bounds;
            for (int i = 1; i < firstLod.Length; i++)
                bounds.Encapsulate(firstLod[i].bounds);
            if (normalized && (bounds.size.y < 0.8f || bounds.size.y > 1.2f || Mathf.Abs(bounds.min.y) > 0.08f))
                throw new InvalidOperationException($"Prefab is not normalized and grounded: {path}, bounds={bounds}");
            if (!normalized && bounds.size.y <= 0f)
                throw new InvalidOperationException("Prop has empty bounds: " + path);
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(instance);
        }
    }

    private static string MaterialsFolder(Family family)
    {
        switch (family)
        {
            case Family.Crop: return PruningRoot + "/Materials";
            case Family.Benchmark: return BenchmarkRoot + "/Materials";
            case Family.Ground: return GroundRoot + "/Materials";
            case Family.Coarse: return CoarseRoot + "/Materials";
            default: return RingRoot + "/Materials";
        }
    }

    private static string TexturesFolder(Family family)
    {
        switch (family)
        {
            case Family.Crop: return PruningRoot + "/Textures";
            case Family.Benchmark: return BenchmarkRoot + "/Textures";
            case Family.Ground: return GroundRoot + "/Textures";
            case Family.Coarse: return CoarseRoot + "/Textures";
            default: return RingRoot + "/Textures";
        }
    }
}
