using UnityEditor;
using UnityEngine;

// Scenario 1 starting-stand floor presentation (Docs/Scenario1StartingStandAssetFix.md).
// Presentation only: no ecology, placement population or save data changes.
//  - Ground moss: the delivered Ground_Moss_Shoots_03 shoot/cushion mat replaces
//    the flat Ground_Moss_Flat_01 sheet in the habitat catalog.
//  - Ground: the flat-colour Forest Ground material gets the existing project
//    soil texture, tiled and tinted toward conifer needle litter [D].
public static class ScenarioOneFloorPresentationSetup
{
    private const string CatalogPath = "Assets/ForestPrototype/ScenarioOne/Resources/RecentAssetVisualCatalog.asset";
    private const string MossPrefab = "Assets/ForestPrototype/Prefabs/Forestry/SectionFive/Ground_Moss_ShootMat_03.prefab";
    private const string GroundMaterial = "Assets/ForestPrototype/Materials/Forest Ground.mat";
    private const string SoilTexture = "Assets/ForestPrototype/Art/ForestryGround/Textures/Forest_RootPlate_Soil_BaseColor.png";
    // [D] ~2.5 m texture repeat on the 40 m ground plane; warm needle-litter tint.
    private static readonly Vector2 GroundTiling = new Vector2(16f, 16f);
    private static readonly Color NeedleLitterTint = new Color(0.92f, 0.74f, 0.58f);

    [MenuItem("Tools/Forest Prototype/Scenario 1 Floor Presentation")]
    public static void BuildAndWire()
    {
        SectionFiveAssetSetup.CopyPackage("Ground_Moss_Shoots_03", new[] { "Ground_Moss_ShootMat_03" }, true);
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        SectionFiveAssetSetup.Build("Ground_Moss_Shoots_03", "Ground_Moss_ShootMat_03", true);
        var catalog = AssetDatabase.LoadAssetAtPath<RecentAssetVisualCatalog>(CatalogPath);
        var moss = AssetDatabase.LoadAssetAtPath<GameObject>(MossPrefab);
        if (catalog == null || moss == null)
            throw new System.InvalidOperationException("Moss prefab or habitat catalog missing");
        catalog.groundMoss = moss;
        EditorUtility.SetDirty(catalog);

        var ground = AssetDatabase.LoadAssetAtPath<Material>(GroundMaterial);
        var soil = AssetDatabase.LoadAssetAtPath<Texture2D>(SoilTexture);
        if (ground == null || soil == null)
            throw new System.InvalidOperationException("Forest Ground material or soil texture missing");
        ground.SetTexture("_BaseMap", soil);
        ground.SetTextureScale("_BaseMap", GroundTiling);
        ground.SetTexture("_MainTex", soil);
        ground.SetTextureScale("_MainTex", GroundTiling);
        ground.SetColor("_BaseColor", NeedleLitterTint);
        ground.SetColor("_Color", NeedleLitterTint);
        ground.SetFloat("_Smoothness", 0.05f);
        EditorUtility.SetDirty(ground);
        AssetDatabase.SaveAssets();
        Debug.Log("SCENARIO1_FLOOR_PRESENTATION_PASS moss=Ground_Moss_ShootMat_03 groundTexture=Forest_RootPlate_Soil_BaseColor tiling=16");
    }
    [MenuItem("Tools/Forest Prototype/Scenario 1 Track Presentation")]
    public static void TextureTrack()
    {
        var path = AssetDatabase.LoadAssetAtPath<Material>("Assets/ForestPrototype/Materials/Forest Path.mat");
        var soil = AssetDatabase.LoadAssetAtPath<Texture2D>(SoilTexture);
        if (path == null || soil == null)
            throw new System.InvalidOperationException("Forest Path material or soil texture missing");
        // Existing authored soil detail, with a lighter tint to keep access distinct
        // from the dark needle-litter floor. No route geometry or ecology changes.
        var tint = new Color(1.6f, 1.4f, 1.05f);
        foreach (string property in new[] { "_BaseMap", "_MainTex" })
        {
            path.SetTexture(property, soil);
            path.SetTextureScale(property, new Vector2(3f, 3f));
        }
        path.SetColor("_BaseColor", tint);
        path.SetColor("_Color", tint);
        path.SetFloat("_Smoothness", 0.05f);
        EditorUtility.SetDirty(path);
        AssetDatabase.SaveAssets();
        Debug.Log("SCENARIO1_TRACK_PRESENTATION_PASS existingSoilTexture=true");
    }

}
