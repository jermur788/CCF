using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Copy into Assets temporarily, then run CCFOakPlayerPlantingVerification.Begin.
public static class CCFOakPlayerPlantingVerification
{
    public static void Begin()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        EditorApplication.isPlaying = true;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        new GameObject("Oak player planting verification").AddComponent<CCFOakPlayerPlantingRunner>();
    }
}

public sealed class CCFOakPlayerPlantingRunner : MonoBehaviour
{
    private static readonly MethodInfo EnterPlantingMode = typeof(ForestPlayer).GetMethod(
        "EnterPlantingMode", BindingFlags.Instance | BindingFlags.NonPublic);
    private static readonly MethodInfo ExitPlantingMode = typeof(ForestPlayer).GetMethod(
        "ExitPlantingMode", BindingFlags.Instance | BindingFlags.NonPublic);
    private static readonly MethodInfo SelectPlantingSpecies = typeof(ForestPlayer).GetMethod(
        "SelectPlantingSpecies", BindingFlags.Instance | BindingFlags.NonPublic);
    private static readonly MethodInfo CyclePlantingSpecies = typeof(ForestPlayer).GetMethod(
        "CyclePlantingSpecies", BindingFlags.Instance | BindingFlags.NonPublic);
    private static readonly MethodInfo SelectedPlantingSpeciesId = typeof(ForestPlayer).GetMethod(
        "SelectedPlantingSpeciesId", BindingFlags.Instance | BindingFlags.NonPublic);
    private static readonly MethodInfo PlantSelectedSpecies = typeof(ForestPlayer).GetMethod(
        "PlantSelectedSpeciesAt", BindingFlags.Instance | BindingFlags.NonPublic);
    private static readonly FieldInfo IsPlantingMode = typeof(ForestPlayer).GetField(
        "isPlantingMode", BindingFlags.Instance | BindingFlags.NonPublic);
    private static readonly FieldInfo PlantingSpeciesIds = typeof(ForestPlayer).GetField(
        "plantingSpeciesIds", BindingFlags.Instance | BindingFlags.NonPublic);
    private static readonly FieldInfo LastMessage = typeof(ForestPlayer).GetField(
        "lastHarvestMessage", BindingFlags.Instance | BindingFlags.NonPublic);
    private static readonly MethodInfo Promote = typeof(ForestEcologyController).GetMethod(
        "PromoteCohorts", BindingFlags.Instance | BindingFlags.NonPublic);

    private string savePath;
    private bool saveExisted;
    private byte[] saveBackup;

    private static void Check(bool condition, string message)
    {
        if (!condition)
            throw new InvalidOperationException(message);
    }

    private static bool UsesStageAsset(ForestTree tree, string stage)
    {
        Transform visual = tree != null ? tree.transform.Find("PolishedVisual") : null;
        return visual != null && visual.GetComponent<LODGroup>()?.GetLODs().Length == 3 &&
               visual.GetComponentsInChildren<MeshFilter>(true)
                   .Any(filter => filter.sharedMesh != null && filter.sharedMesh.name.Contains(stage));
    }

    private IEnumerator Verify()
    {
        yield return null;
        ForestPlayer player = FindFirstObjectByType<ForestPlayer>();
        ForestEcologyController ecology = FindFirstObjectByType<ForestEcologyController>();
        ForestTreeSpawner spawner = FindFirstObjectByType<ForestTreeSpawner>();
        ForestSaveController saves = FindFirstObjectByType<ForestSaveController>();
        Check(player != null && ecology != null && spawner != null && saves != null,
            "Required ForestTest systems are missing");
        Check(EnterPlantingMode != null && ExitPlantingMode != null && SelectPlantingSpecies != null &&
              CyclePlantingSpecies != null && SelectedPlantingSpeciesId != null &&
              PlantSelectedSpecies != null && IsPlantingMode != null && PlantingSpeciesIds != null &&
              LastMessage != null && Promote != null,
            "Planting hotbar, common action or lifecycle methods are missing");
        TreeSpeciesDefinition oak = spawner.ResolveSpecies("sessile-oak");
        Check(oak != null, "Oak is unavailable to the player planting path");

        string[] plantingSpecies = (string[])PlantingSpeciesIds.GetValue(player);
        Check(plantingSpecies != null && plantingSpecies.Length >= 2 &&
              plantingSpecies[0] == "beech" && plantingSpecies[1] == oak.SpeciesId,
            "Planting hotbar does not expose Beech and Oak in its configured options");
        EnterPlantingMode.Invoke(player, null);
        Check((bool)IsPlantingMode.GetValue(player), "Planting mode did not open");
        Check((string)SelectedPlantingSpeciesId.Invoke(player, null) == "beech",
            "Planting mode did not preserve its default Beech selection");
        SelectPlantingSpecies.Invoke(player, new object[] { 1 });
        Check((string)SelectedPlantingSpeciesId.Invoke(player, null) == oak.SpeciesId,
            "Oak could not be selected through hotbar slot 2");
        CyclePlantingSpecies.Invoke(player, null);
        Check((string)SelectedPlantingSpeciesId.Invoke(player, null) == "beech",
            "The scalable next-species control did not cycle the planting list");
        SelectPlantingSpecies.Invoke(player, new object[] { 1 });

        ForestEcologyCell cell = ecology.Cells[0];
        Vector3 point = new Vector3(cell.Center.x, 0f, cell.Center.y);
        PlantSelectedSpecies.Invoke(player, new object[] { point });
        ForestRegenerationCohort cohort = cell.FindCohort(oak.SpeciesId);
        Check(cohort != null && cohort.Origin == RegenerationOrigin.Planted,
            "The selected-species planting action did not create a planted Oak cohort");
        Check(cohort.OriginYear == ecology.EcologicalYear &&
              cohort.EstablishYear == ecology.EcologicalYear - ecology.PlantedJuvenileAgeYears,
            "Player-planted Oak provenance or biological age is wrong");
        Check(Mathf.Approximately(cohort.Height, ecology.PlantedJuvenileHeightM) &&
              Mathf.Approximately(cohort.Density, ecology.PlantedJuvenileDensity),
            "Player-planted Oak does not use generic planting calibration");
        Check((string)LastMessage.GetValue(player) == "Sessile oak juvenile planted",
            "Player feedback did not use Forestry's success message");
        float densityBeforeDuplicate = cohort.Density;
        PlantSelectedSpecies.Invoke(player, new object[] { point });
        Check(cell.FindCohort(oak.SpeciesId) == cohort &&
              Mathf.Approximately(cohort.Density, densityBeforeDuplicate),
            "A repeated player action duplicated or changed the Oak cohort");
        Check(((string)LastMessage.GetValue(player)).Contains("already regenerating"),
            "Player feedback did not use Forestry's duplicate-cohort message");
        ExitPlantingMode.Invoke(player, null);
        Check(!(bool)IsPlantingMode.GetValue(player), "Planting mode did not exit cleanly");
        yield return null;
        GameObject sapling = GameObject.Find("Sessile oak seedling cell 0");
        Check(sapling != null && sapling.GetComponent<LODGroup>()?.GetLODs().Length == 3,
            "Player-planted Oak did not receive the sapling visual");
        Debug.Log("PLANTING_HOTBAR_PASS options=Beech,Oak commonAction=G cancel=Escape");
        Debug.Log("OAK_PLAYER_PLANT_PASS origin=Planted");

        // Drive the same authoritative cohort through the existing promotion
        // boundary, then through the existing height-based mature visual switch.
        cell.Light = 1f;
        cohort.Height = oak.PromotionHeightM;
        Promote.Invoke(ecology, new object[] { ecology.ResolveSpecies(), new System.Random(73) });
        ForestTree promoted = FindObjectsByType<ForestTree>(FindObjectsSortMode.None)
            .FirstOrDefault(tree => tree.Species == oak && tree.TreeId.StartsWith("PL", StringComparison.Ordinal));
        Check(promoted != null && cell.FindCohort(oak.SpeciesId).Density <= 0f,
            "Player-planted Oak did not use the existing cohort promotion path");
        Check(UsesStageAsset(promoted, "Young"), "Promoted planted Oak did not use the young-tree asset");
        promoted.ApplyGrowth(1f, 10f);
        yield return null;
        Check(UsesStageAsset(promoted, "Mature"), "Growing planted Oak did not transition to the mature asset");
        Debug.Log("OAK_PLAYER_LIFECYCLE_PASS cohortToYoungToMature=True");

        saves.Save();
        string promotedId = promoted.TreeId;
        Destroy(promoted.gameObject);
        yield return null;
        saves.Load();
        yield return null;
        yield return null;
        ForestTree restored = FindObjectsByType<ForestTree>(FindObjectsSortMode.None)
            .FirstOrDefault(tree => tree.TreeId == promotedId);
        Check(restored != null && restored.Species == oak && UsesStageAsset(restored, "Mature"),
            "Save/load did not restore the player-planted mature Oak");
        Debug.Log("OAK_PLAYER_SAVELOAD_PASS");
    }

    private IEnumerator Start()
    {
        savePath = Path.Combine(Application.persistentDataPath, "forest-save.json");
        saveExisted = File.Exists(savePath);
        if (saveExisted)
            saveBackup = File.ReadAllBytes(savePath);
        Exception failure = null;
        IEnumerator verification = Verify();
        while (true)
        {
            bool more = false;
            object current = null;
            try
            {
                more = verification.MoveNext();
                if (more)
                    current = verification.Current;
            }
            catch (Exception exception)
            {
                failure = exception;
            }
            if (failure != null || !more)
                break;
            yield return current;
        }
        if (saveExisted)
            File.WriteAllBytes(savePath, saveBackup);
        else if (File.Exists(savePath))
            File.Delete(savePath);
        if (failure == null)
            Debug.Log("OAK_PLAYER_PLANTING_VERIFY_PASS");
        else
            Debug.LogError("OAK_PLAYER_PLANTING_VERIFY_FAIL " + failure);
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(failure == null ? 0 : 1);
    }
}
