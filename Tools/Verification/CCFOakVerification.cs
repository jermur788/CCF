using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Copy into Assets temporarily, then run CCFOakVerification.Begin in batch mode.
public static class CCFOakVerification
{
    public static void Begin()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/MixedSpeciesTest.unity");
        EditorApplication.isPlaying = true;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        new GameObject("Oak verification").AddComponent<CCFOakVerificationRunner>();
    }
}

public sealed class CCFOakVerificationRunner : MonoBehaviour
{
    private ForestEcologyController ecology;
    private ForestTreeSpawner spawner;
    private ForestSaveController saves;
    private TreeSpeciesDefinition sitka;
    private TreeSpeciesDefinition beech;
    private TreeSpeciesDefinition oak;
    private string savePath;
    private byte[] saveBackup;
    private bool saveExisted;

    private static readonly MethodInfo GrowRegeneration = typeof(ForestEcologyController).GetMethod(
        "GrowExistingRegeneration", BindingFlags.Instance | BindingFlags.NonPublic);
    private static readonly MethodInfo PromoteRegeneration = typeof(ForestEcologyController).GetMethod(
        "PromoteCohorts", BindingFlags.Instance | BindingFlags.NonPublic);

    private static void Check(bool condition, string message)
    {
        if (!condition)
            throw new InvalidOperationException(message);
    }

    private static void Near(float actual, float expected, float tolerance, string message)
    {
        Check(Mathf.Abs(actual - expected) <= tolerance,
            $"{message}: expected {expected:R}, got {actual:R}");
    }

    private Vector3 CellPosition(int index)
    {
        Vector2 center = ecology.Cells[index].Center;
        return new Vector3(center.x, 0f, center.y);
    }

    private static string CohortBytes(ForestRegenerationCohort cohort)
    {
        return string.Join("|", cohort.SpeciesId, cohort.Density.ToString("R"),
            cohort.Height.ToString("R"), cohort.EstablishYear, (int)cohort.Origin, cohort.OriginYear);
    }

    private void ClearCohorts()
    {
        foreach (ForestEcologyCell cell in ecology.Cells)
        {
            cell.ClearRegeneration();
            cell.RecentOpening = 0f;
            cell.EstablishmentSuitability = 1f;
        }
    }

    private IEnumerator RemoveAllTrees()
    {
        foreach (ForestTree tree in FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            if (tree != null)
                Destroy(tree.gameObject);
        yield return null;
        ecology.ResetForDeterministicRun();
    }

    private void VerifyParameters()
    {
        Check(oak != null, "Sessile Oak species is not registered with the spawner");
        Check(oak.SpeciesId == "sessile-oak" && oak.LatinName == "Quercus petraea", "Oak identity is wrong");
        Near(oak.MaxHeightM, 40f, 0.0001f, "Oak max height");
        Near(oak.MaxDbhCm, 120f, 0.0001f, "Oak soft DBH scale");
        Near(oak.MaturityOnsetYears, 40f, 0.0001f, "Oak maturity onset");
        Near(oak.MaturityFullYears, 60f, 0.0001f, "Oak full maturity");
        Near(oak.SeedDispersalScaleM, 4f, 0.0001f, "Oak dispersal scale");
        Near(oak.SeedDispersalCutoffM, 80f, 0.0001f, "Oak dispersal cutoff");
        Check(oak.UsesDistinctJuvenileLightResponses, "Oak distinct juvenile responses are disabled");
        float[] light = { 0.01f, 0.05f, 0.10f, 0.15f, 0.20f, 0.30f, 0.40f, 0.50f, 1f };
        float[] growth = { 0f, 0.02f, 0.07f, 0.15f, 0.30f, 0.60f, 0.80f, 1f, 1f };
        for (int i = 0; i < light.Length; i++)
            Near(oak.JuvenileLightResponse(light[i]), growth[i], 0.0001f, $"Oak growth response at {light[i]:P0}");
        Check(oak.JuvenileEstablishmentResponse(0.01f) > 0.7f,
            "Oak cannot establish beneath canopy independently of growth");
        Check(oak.JuvenileSurvivalResponse(0.10f) < 1f && oak.JuvenileSurvivalResponse(0.15f) < 1f &&
              oak.JuvenileSurvivalResponse(0.20f) >= 0.999f, "Oak survival thresholds are wrong");
        Check(!sitka.UsesDistinctJuvenileLightResponses && !beech.UsesDistinctJuvenileLightResponses,
            "Legacy species unexpectedly entered the distinct-response path");
        Debug.Log("OAK_PARAMETERS_PASS");
    }

    private void VerifyLightGradientAndRelease()
    {
        ClearCohorts();
        float[] light = { 0.01f, 0.05f, 0.10f, 0.15f, 0.20f, 0.30f, 0.40f, 0.50f, 1f };
        for (int i = 0; i < light.Length; i++)
        {
            ForestEcologyCell cell = ecology.Cells[i];
            cell.Light = light[i];
            cell.GetOrCreateCohort(oak).Restore(1f, oak.RegenInitialHeightM, 0);
        }
        for (int year = 0; year < 12; year++)
            GrowRegeneration.Invoke(ecology, null);

        Check(ecology.Cells[0].FindCohort(oak.SpeciesId).Density <= 0.01f, "1% Oak did not fail rapidly");
        Check(ecology.Cells[1].FindCohort(oak.SpeciesId).Density < 0.02f, "5% Oak persisted too long");
        Check(ecology.Cells[2].FindCohort(oak.SpeciesId).Density < 0.05f, "10% Oak did not ultimately decline");
        Check(ecology.Cells[3].FindCohort(oak.SpeciesId).Density > 0.20f, "15% Oak did not persist for years");
        Check(ecology.Cells[4].FindCohort(oak.SpeciesId).Density >= 1f, "20% Oak was not sustainable");
        Check(ecology.Cells[8].FindCohort(oak.SpeciesId).Height > ecology.Cells[5].FindCohort(oak.SpeciesId).Height,
            "Oak high-light growth is not stronger than 30% growth");

        // Even an artificially tall advance-regeneration cohort cannot recruit
        // permanently at <=10% light.
        ForestRegenerationCohort dark = ecology.Cells[2].FindCohort(oak.SpeciesId);
        dark.Density = 1f;
        dark.Height = oak.PromotionHeightM + 1f;
        PromoteRegeneration.Invoke(ecology, new object[] { sitka, new System.Random(123) });
        Check(!FindObjectsByType<ForestTree>(FindObjectsSortMode.None)
                .Any(t => t.Species == oak && ecology.GetCellIndex(t.transform.position) == 2),
            "Oak recruited at 10% light");

        ForestRegenerationCohort release = ecology.Cells[3].FindCohort(oak.SpeciesId);
        float beforeReleaseHeight = release.Height;
        ecology.Cells[3].Light = 0.5f;
        for (int year = 0; year < 10; year++)
            GrowRegeneration.Invoke(ecology, null);
        Check(release.Height > beforeReleaseHeight + 3f, "Oak did not respond to release");
        PromoteRegeneration.Invoke(ecology, new object[] { sitka, new System.Random(124) });
        Check(FindObjectsByType<ForestTree>(FindObjectsSortMode.None).Any(t => t.Species == oak),
            "Released Oak did not promote");
        Debug.Log("OAK_LIGHT_RELEASE_PASS");
    }

    private void VerifyBeechComparisonAndMixedCell()
    {
        ClearCohorts();
        float[] comparisonLight = { 0.10f, 0.20f, 0.30f, 0.50f };
        var comparison = new List<Tuple<ForestRegenerationCohort, ForestRegenerationCohort>>();
        for (int i = 0; i < comparisonLight.Length; i++)
        {
            ForestEcologyCell cell = ecology.Cells[10 + i];
            cell.Light = comparisonLight[i];
            ForestRegenerationCohort comparisonOak = cell.GetOrCreateCohort(oak);
            ForestRegenerationCohort comparisonBeech = cell.GetOrCreateCohort(beech);
            comparisonOak.Restore(0.45f, 0.18f, 1);
            comparisonBeech.Restore(0.45f, 0.18f, 1);
            comparison.Add(Tuple.Create(comparisonOak, comparisonBeech));
        }
        GrowRegeneration.Invoke(ecology, null);
        for (int i = 0; i < comparison.Count; i++)
        {
            ForestRegenerationCohort comparisonOak = comparison[i].Item1;
            ForestRegenerationCohort comparisonBeech = comparison[i].Item2;
            Check(comparisonBeech.Height >= comparisonOak.Height && comparisonBeech.Density >= comparisonOak.Density,
                $"Beech is not at least as strong as Oak at {comparisonLight[i]:P0} light");
        }
        Check(comparison[0].Item2.Height > comparison[0].Item1.Height &&
              comparison[0].Item2.Density > comparison[0].Item1.Density,
            "Beech is not the stronger deep-shade regeneration strategy");

        // Oak and Sitka share the same cohort machinery at closed, moderate
        // and strong light without exceeding normalized cell capacity.
        float[] sitkaLight = { 0.10f, 0.30f, 0.50f };
        for (int i = 0; i < sitkaLight.Length; i++)
        {
            ForestEcologyCell cell = ecology.Cells[15 + i];
            cell.Light = sitkaLight[i];
            cell.GetOrCreateCohort(oak).Restore(0.6f, 0.18f, 1);
            cell.GetOrCreateCohort(sitka).Restore(0.6f, 0.18f, 1);
        }
        GrowRegeneration.Invoke(ecology, null);
        for (int i = 0; i < sitkaLight.Length; i++)
        {
            ForestEcologyCell cell = ecology.Cells[15 + i];
            Check(cell.FindCohort(oak.SpeciesId).Height > 0.18f &&
                  cell.FindCohort(sitka.SpeciesId).Height > 0.18f,
                $"Oak/Sitka did not both respond at {sitkaLight[i]:P0} light");
            Check(cell.SharedOccupancy <= 1.000001f,
                $"Oak/Sitka capacity exceeded at {sitkaLight[i]:P0} light");
        }

        ForestEcologyCell mixed = ecology.Cells[19];
        mixed.Light = 0.5f;
        ForestRegenerationCohort s = mixed.GetOrCreateCohort(sitka);
        ForestRegenerationCohort b = mixed.GetOrCreateCohort(beech);
        ForestRegenerationCohort o = mixed.GetOrCreateCohort(oak);
        s.Restore(0.3f, 0.4f, 2, RegenerationOrigin.Natural, 2);
        b.Restore(0.3f, 0.5f, 3, RegenerationOrigin.Natural, 3);
        o.Restore(0.3f, 0.6f, 4, RegenerationOrigin.Planted, 4);
        Check(mixed.SharedOccupancy <= 1.000001f, "Three-species occupancy exceeds one");
        RegenerationQueryResult query = ecology.QueryRegeneration(CellPosition(19));
        Check(query.Success && query.Cohorts.Count == 3, "Three-species query did not return all cohorts");
        string sitkaBefore = CohortBytes(s);
        string beechBefore = CohortBytes(b);
        UprootingResult removed = ecology.TryUprootRegeneration(CellPosition(19), oak);
        Check(removed.Success && mixed.FindCohort(oak.SpeciesId) == null, "Selective Oak uprooting failed");
        Check(CohortBytes(s) == sitkaBefore && CohortBytes(b) == beechBefore,
            "Selective Oak uprooting changed another species");
        Debug.Log("OAK_MIXED_PASS");
    }

    private void VerifyPlanting()
    {
        ClearCohorts();
        PlantingResult result = ecology.TryPlantJuvenile(oak, CellPosition(20));
        Check(result.Success, "Generic Oak planting failed: " + result.Message);
        ForestRegenerationCohort planted = ecology.Cells[20].FindCohort(oak.SpeciesId);
        Check(planted != null && planted.Origin == RegenerationOrigin.Planted &&
              planted.OriginYear == ecology.EcologicalYear &&
              planted.EstablishYear == ecology.EcologicalYear - ecology.PlantedJuvenileAgeYears,
            "Oak planting provenance is wrong");
        Near(planted.Height, 0.6f, 0.0001f, "Oak planted height");
        Near(planted.Density, 1f, 0.0001f, "Oak planted density");

        ForestRegenerationCohort natural = ecology.Cells[21].GetOrCreateCohort(oak);
        natural.Restore(planted.Density, planted.Height, planted.EstablishYear,
            RegenerationOrigin.Natural, planted.OriginYear);
        ecology.Cells[20].Light = ecology.Cells[21].Light = 0.5f;
        GrowRegeneration.Invoke(ecology, null);
        Near(planted.Height, natural.Height, 0.000001f, "Natural/planted Oak height diverged");
        Near(planted.Density, natural.Density, 0.000001f, "Natural/planted Oak density diverged");
        Debug.Log("OAK_PLANTING_PASS");
    }

    private IEnumerator VerifyAdultCompetitionAndLifecycle()
    {
        yield return RemoveAllTrees();
        ForestTree openOak = spawner.Spawn("OAK-OPEN", oak, Vector3.zero, 20, 20f, 8f,
            oak.PotentialCrownRadiusM(20f));
        ecology.AdvanceOneYear();
        float openGrowth = openOak.Diameter - 20f;
        Destroy(openOak.gameObject);
        yield return null;
        ecology.ResetForDeterministicRun();

        ForestTree crowdedOak = spawner.Spawn("OAK-CROWDED", oak, Vector3.zero, 20, 20f, 8f,
            oak.PotentialCrownRadiusM(20f));
        spawner.Spawn("SITKA-COMP", sitka, new Vector3(1.5f, 0f, 0f), 30, 30f, 12f,
            sitka.PotentialCrownRadiusM(30f));
        spawner.Spawn("BEECH-COMP", beech, new Vector3(-1.5f, 0f, 0f), 30, 30f, 12f,
            beech.PotentialCrownRadiusM(30f));
        ecology.AdvanceOneYear();
        float crowdedGrowth = crowdedOak.Diameter - 20f;
        Check(crowdedGrowth < openGrowth, "Hegyi competition did not suppress Oak DBH growth");
        float suppressionBeforeRelease = crowdedOak.EquivalentSuppressedYears;
        foreach (ForestTree competitor in FindObjectsByType<ForestTree>(FindObjectsSortMode.None)
                     .Where(t => t != crowdedOak).ToArray())
            Destroy(competitor.gameObject);
        yield return null;
        float dbhBeforeRelease = crowdedOak.Diameter;
        ecology.AdvanceOneYear();
        float releasedGrowth = crowdedOak.Diameter - dbhBeforeRelease;
        Check(releasedGrowth > crowdedGrowth, "Oak DBH did not release after competitors were removed");
        Check(crowdedOak.EquivalentSuppressedYears >= suppressionBeforeRelease,
            "Oak suppression history reset after release");

        yield return RemoveAllTrees();
        ForestTree parent = spawner.Spawn("OAK-PARENT", oak, Vector3.zero, 80, 55f, 25f,
            oak.PotentialCrownRadiusM(55f));
        bool sawGoodMast = false;
        bool sawPoorMast = false;
        bool sawSeed = false;
        bool sawEstablishment = false;
        bool sawPromotion = false;
        for (int year = 0; year < 150; year++)
        {
            ecology.AdvanceOneYear();
            string mast = ecology.GetMastLabel(oak);
            sawGoodMast |= mast == "good";
            sawPoorMast |= mast == "poor";
            sawSeed |= ecology.Cells.Any(c => c.FindCohort(oak.SpeciesId)?.SeedRain > 0f);
            sawEstablishment |= ecology.Cells.Any(c => c.FindCohort(oak.SpeciesId)?.Density > 0f);
            sawPromotion |= FindObjectsByType<ForestTree>(FindObjectsSortMode.None)
                .Any(t => t != parent && t.Species == oak);
        }
        ForestTree[] recruits = FindObjectsByType<ForestTree>(FindObjectsSortMode.None)
            .Where(t => t != parent && t.Species == oak).ToArray();
        Check(sawGoodMast && sawPoorMast, "Oak did not show episodic mast states");
        Check(sawSeed && sawEstablishment && sawPromotion, "Oak lifecycle did not reach seed, establishment and promotion");
        ForestTree matureRecruit = recruits.FirstOrDefault(t => t.AgeYears >= 60);
        Check(matureRecruit != null, "No regenerated Oak reached reproductive age in 150 years");
        Destroy(parent.gameObject);
        yield return null;
        ecology.RecomputeSeedRain();
        Check(ecology.Cells.Any(c => c.FindCohort(oak.SpeciesId)?.SeedRain > 0f),
            "Regenerated mature Oak did not produce second-generation acorns");
        Debug.Log($"OAK_LIFECYCLE_PASS recruits={recruits.Length} matureRecruitAge={matureRecruit.AgeYears}");
    }

    private IEnumerator VerifySaveLoadContinuation()
    {
        ClearCohorts();
        ForestRegenerationCohort natural = ecology.Cells[30].GetOrCreateCohort(oak);
        ForestRegenerationCohort planted = ecology.Cells[31].GetOrCreateCohort(oak);
        natural.Restore(0.4f, 0.8f, ecology.EcologicalYear - 4, RegenerationOrigin.Natural, ecology.EcologicalYear - 4);
        planted.Restore(0.6f, 0.9f, ecology.EcologicalYear - 3, RegenerationOrigin.Planted, ecology.EcologicalYear);
        saves.Save();
        string checkpoint = File.ReadAllText(savePath);
        ecology.AdvanceOneYear();
        saves.Save();
        string uninterrupted = File.ReadAllText(savePath);

        File.WriteAllText(savePath, checkpoint);
        saves.Load();
        yield return null;
        yield return null;
        ForestRegenerationCohort restoredNatural = ecology.Cells[30].FindCohort(oak.SpeciesId);
        ForestRegenerationCohort restoredPlanted = ecology.Cells[31].FindCohort(oak.SpeciesId);
        Check(restoredNatural != null && restoredNatural.Origin == RegenerationOrigin.Natural,
            "Natural Oak cohort did not survive save/load");
        Check(restoredPlanted != null && restoredPlanted.Origin == RegenerationOrigin.Planted,
            "Planted Oak cohort did not survive save/load");
        Check(FindObjectsByType<ForestTree>(FindObjectsSortMode.None).Any(t => t.Species == oak),
            "Oak individual did not survive save/load");
        ecology.AdvanceOneYear();
        saves.Save();
        Check(File.ReadAllText(savePath) == uninterrupted, "Oak deterministic continuation differs after load");
        Debug.Log("OAK_SAVE_LOAD_PASS");
    }

    private IEnumerator Run()
    {
        yield return null;
        ecology = FindFirstObjectByType<ForestEcologyController>();
        spawner = FindFirstObjectByType<ForestTreeSpawner>();
        saves = FindFirstObjectByType<ForestSaveController>();
        Check(ecology != null && spawner != null && saves != null, "Required scene systems are missing");
        sitka = spawner.ResolveSpecies("sitka-spruce");
        beech = spawner.ResolveSpecies("beech");
        oak = spawner.ResolveSpecies("sessile-oak");
        Check(sitka != null && beech != null, "Baseline species are missing");
        VerifyParameters();
        VerifyLightGradientAndRelease();
        VerifyBeechComparisonAndMixedCell();
        VerifyPlanting();
        yield return VerifyAdultCompetitionAndLifecycle();
        yield return VerifySaveLoadContinuation();
        Debug.Log("OAK_VERIFY_PASS");
    }

    private IEnumerator Start()
    {
        savePath = Path.Combine(Application.persistentDataPath, "forest-save.json");
        saveExisted = File.Exists(savePath);
        if (saveExisted)
            saveBackup = File.ReadAllBytes(savePath);
        Exception failure = null;
        IEnumerator test = Run();
        while (true)
        {
            bool more = false;
            object current = null;
            try
            {
                more = test.MoveNext();
                if (more)
                    current = test.Current;
            }
            catch (Exception exception)
            {
                failure = exception;
            }
            if (failure != null || !more)
                break;
            if (current is IEnumerator nested)
                yield return StartCoroutine(nested);
            else
                yield return current;
        }

        if (saveExisted)
            File.WriteAllBytes(savePath, saveBackup);
        else if (File.Exists(savePath))
            File.Delete(savePath);
        if (failure != null)
            Debug.LogError("OAK_VERIFY_FAIL " + failure);
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(failure == null ? 0 : 1);
    }
}
