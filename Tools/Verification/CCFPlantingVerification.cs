using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

// Disposable Unity runner for Beech Planting v1. Copy this file alone into
// Assets, run CCFPlantingVerification.Begin in Editor batchmode, then remove
// the Assets copy and generated metadata. It preserves the user's save.
public static class CCFPlantingVerification
{
#if UNITY_EDITOR
    public static void Begin()
    {
        // MixedSpeciesTest wires Beech into the spawner; ForestTest does not yet.
        EditorSceneManager.OpenScene("Assets/Scenes/MixedSpeciesTest.unity");
        EditorApplication.isPlaying = true;
    }
#endif

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        new GameObject("CCF Planting Verification").AddComponent<CCFPlantingRunner>();
    }
}

public sealed class CCFPlantingRunner : MonoBehaviour
{
    private ForestEcologyController ecology;
    private ForestTreeSpawner spawner;
    private ForestSaveController saves;
    private TreeSpeciesDefinition beech;
    private TreeSpeciesDefinition sitka;
    private string savePath;
    private byte[] previousSave;
    private bool hadPreviousSave;

    private IEnumerator Start()
    {
        yield return null;
        Exception failure = null;
        IEnumerator test = Test();
        while (true)
        {
            bool more;
            object current = null;
            try
            {
                more = test.MoveNext();
                if (more) current = test.Current;
            }
            catch (Exception ex) { failure = ex; break; }
            if (!more) break;
            yield return current;
        }

        if (failure == null) Debug.Log("PLANTING_VERIFY_PASS");
        else Debug.LogError("PLANTING_VERIFY_FAIL: " + failure);
        RestoreUserSave();
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(failure == null ? 0 : 1);
#endif
    }

    private IEnumerator Test()
    {
        ecology = FindFirstObjectByType<ForestEcologyController>();
        spawner = FindFirstObjectByType<ForestTreeSpawner>();
        saves = FindFirstObjectByType<ForestSaveController>();
        Require(ecology != null && spawner != null && saves != null, "ecology/spawner/save systems missing");
        beech = spawner.ResolveSpecies("beech");
        sitka = spawner.DefaultSpecies;
        Require(beech != null, "Beech species missing");
        Require(beech.SupportsRegeneration, "Beech regeneration is not enabled");
        Require(sitka != null, "default Sitka species missing");

        savePath = Path.Combine(Application.persistentDataPath, "forest-save.json");
        hadPreviousSave = File.Exists(savePath);
        if (hadPreviousSave) previousSave = File.ReadAllBytes(savePath);

        VerifyPlantingApi();
        VerifyLightProbe();
        VerifyDeepShadeAndRelease();
        VerifyEquivalence();
        VerifySharedOccupancy();
        VerifyPureBeechRegression();
        VerifySanityScenarios();
        VerifyFullLifecycle();
        IEnumerator saveReload = VerifySaveReload();
        while (true)
        {
            bool more = saveReload.MoveNext();
            if (!more) break;
            yield return saveReload.Current;
        }
    }

    // ---------------------------------------------------------------------
    // Planting API
    // ---------------------------------------------------------------------
    private void VerifyPlantingApi()
    {
        ResetWorld();
        int index = MiddleCell();
        PlantingResult outside = ecology.TryPlantBeech(new Vector3(1000f, 0f, 1000f));
        Require(!outside.Success && outside.Outcome == PlantingOutcome.OutsideStand, "outside-stand planting not rejected");

        PlantingResult planted = ecology.TryPlantBeech(CellCenter(index));
        Require(planted.Success, "planting failed: " + planted.Message);
        Require(planted.CellIndex == index, "planting resolved the wrong cell");
        ForestRegenerationCohort cohort = ecology.Cells[index].FindCohort(beech.SpeciesId);
        Require(cohort != null, "planted cohort missing");
        Require(cohort.Origin == RegenerationOrigin.Planted, "planted origin not recorded");
        Require(cohort.OriginYear == ecology.EcologicalYear, "planted origin year wrong");
        Require(cohort.EstablishYear == ecology.EcologicalYear - ecology.PlantedJuvenileAgeYears, "planted establishment year wrong");
        Require(Mathf.Abs(cohort.Height - ecology.PlantedJuvenileHeightM) < 1e-4f, "planted height wrong");
        Require(Mathf.Abs(cohort.Density - ecology.PlantedJuvenileDensity) < 1e-4f, "planted density wrong");

        PlantingResult again = ecology.TryPlantBeech(CellCenter(index));
        Require(!again.Success && again.Outcome == PlantingOutcome.AlreadyOccupied, "duplicate planting not rejected");

        // A cell with no shared capacity left must reject the insertion and not
        // leave a half-created cohort behind.
        ResetWorld();
        int full = MiddleCell();
        ForestRegenerationCohort sitkaCohort = ecology.Cells[full].GetOrCreateCohort(sitka);
        sitkaCohort.Density = sitka.RegenDensityMax;
        sitkaCohort.Height = 1f;
        sitkaCohort.EstablishYear = 0;
        PlantingResult noCapacity = ecology.TryPlantBeech(CellCenter(full));
        Require(!noCapacity.Success && noCapacity.Outcome == PlantingOutcome.NoCapacity, "full cell did not reject planting");
        Require(ecology.Cells[full].FindCohort(beech.SpeciesId) == null, "rejected planting left a Beech cohort behind");

        Debug.Log("PLANTING_API_PASS origin=Planted age=" + ecology.PlantedJuvenileAgeYears
            + " height=" + ecology.PlantedJuvenileHeightM.ToString("0.00", CultureInfo.InvariantCulture)
            + " density=" + ecology.PlantedJuvenileDensity.ToString("0.00", CultureInfo.InvariantCulture)
            + " duplicateRejected=True noCapacityRejected=True");
    }

    // ---------------------------------------------------------------------
    // Light response probe (one growth step at fixed light)
    // ---------------------------------------------------------------------
    private void VerifyLightProbe()
    {
        float[] lights = { 0.01f, 0.05f, 0.10f, 0.20f, 0.35f };
        var report = new StringBuilder();
        foreach (float light in lights)
        {
            ResetWorld();
            int index = MiddleCell();
            ecology.Cells[index].Light = light;
            PlantingResult result = ecology.TryPlantBeech(CellCenter(index));
            Require(result.Success, "probe planting failed at light " + light);
            ForestRegenerationCohort cohort = ecology.Cells[index].FindCohort(beech.SpeciesId);
            InvokePrivate(ecology, "GrowExistingRegeneration");
            report.Append($" {light:0.00}->{cohort.Height.ToString("0.0000", CultureInfo.InvariantCulture)}/{cohort.Density.ToString("0.000", CultureInfo.InvariantCulture)}");
        }
        Debug.Log("PLANTING_LIGHT_PROBE" + report);
    }

    // ---------------------------------------------------------------------
    // Deep shade stagnation/failure and release response
    // ---------------------------------------------------------------------
    private void VerifyDeepShadeAndRelease()
    {
        ResetWorld();
        int index = MiddleCell();
        ecology.Cells[index].Light = 0.01f;
        Require(ecology.TryPlantBeech(CellCenter(index)).Success, "deep-shade planting failed");
        ForestRegenerationCohort shade = ecology.Cells[index].FindCohort(beech.SpeciesId);
        for (int i = 0; i < 40; i++) InvokePrivate(ecology, "GrowExistingRegeneration");
        float shadeHeight = shade.Height;
        float shadeDensity = shade.Density;
        Require(shadeHeight < 0.7f || shadeDensity < 0.01f, "deep shade did not stagnate or fail");

        ResetWorld();
        index = MiddleCell();
        ecology.Cells[index].Light = 0.01f;
        Require(ecology.TryPlantBeech(CellCenter(index)).Success, "release planting failed");
        ForestRegenerationCohort released = ecology.Cells[index].FindCohort(beech.SpeciesId);
        for (int i = 0; i < 10; i++) InvokePrivate(ecology, "GrowExistingRegeneration");
        float darkHeight = released.Height;
        ecology.Cells[index].Light = 0.35f;
        for (int i = 0; i < 10; i++) InvokePrivate(ecology, "GrowExistingRegeneration");
        float releasedHeight = released.Height;
        Require(releasedHeight - darkHeight > 1.5f, "release did not accelerate planted growth");
        Debug.Log("PLANTING_SHADE_RELEASE shade40h=" + shadeHeight.ToString("0.000", CultureInfo.InvariantCulture)
            + " shade40d=" + shadeDensity.ToString("0.0000", CultureInfo.InvariantCulture)
            + " dark10h=" + darkHeight.ToString("0.000", CultureInfo.InvariantCulture)
            + " released20h=" + releasedHeight.ToString("0.000", CultureInfo.InvariantCulture));
    }

    // ---------------------------------------------------------------------
    // Equivalence: planted and natural juveniles at the same state must follow
    // the same trajectory, and their promoted trees must be identical.
    // ---------------------------------------------------------------------
    private void VerifyEquivalence()
    {
        const int years = 25;

        ResetWorld();
        int index = MiddleCell();
        Require(ecology.TryPlantBeech(CellCenter(index)).Success, "equivalence planted setup failed");
        for (int i = 0; i < years; i++) ecology.AdvanceOneYear();
        ForestTree plantedTree = Trees().FirstOrDefault(t => t.Species == beech && t.TreeId.StartsWith("PL", StringComparison.Ordinal));
        Require(plantedTree != null, "planted Beech did not promote");
        Require(LivingTrees().Length == 1, "planted equivalence produced extra trees before reproduction");

        ResetWorld();
        ForestEcologyCell naturalCell = ecology.Cells[index];
        ForestRegenerationCohort natural = naturalCell.GetOrCreateCohort(beech);
        natural.Height = ecology.PlantedJuvenileHeightM;
        natural.Density = ecology.PlantedJuvenileDensity;
        natural.EstablishYear = -ecology.PlantedJuvenileAgeYears;
        natural.Origin = RegenerationOrigin.Natural;
        natural.OriginYear = natural.EstablishYear;
        for (int i = 0; i < years; i++) ecology.AdvanceOneYear();
        ForestTree naturalTree = Trees().FirstOrDefault(t => t.Species == beech && t.TreeId.StartsWith("R", StringComparison.Ordinal));
        Require(naturalTree != null, "natural Beech did not promote");
        Require(LivingTrees().Length == 1, "natural equivalence produced extra trees before reproduction");

        Require(plantedTree.AgeYears == naturalTree.AgeYears, "equivalence age differs");
        Require(Mathf.Abs(plantedTree.Height - naturalTree.Height) < 1e-5f, "equivalence height differs");
        Require(Mathf.Abs(plantedTree.Diameter - naturalTree.Diameter) < 1e-5f, "equivalence diameter differs");
        Require(Mathf.Abs(plantedTree.CrownRadius - naturalTree.CrownRadius) < 1e-5f, "equivalence crown differs");
        Require(Mathf.Abs(plantedTree.EquivalentSuppressedYears - naturalTree.EquivalentSuppressedYears) < 1e-5f, "equivalence suppression history differs");
        Debug.Log("PLANTING_EQUIVALENCE_PASS years=" + years + " age=" + plantedTree.AgeYears
            + " height=" + plantedTree.Height.ToString("0.000", CultureInfo.InvariantCulture)
            + " dbh=" + plantedTree.Diameter.ToString("0.000", CultureInfo.InvariantCulture));
    }

    // ---------------------------------------------------------------------
    // Shared occupancy never exceeds one while planting several together
    // ---------------------------------------------------------------------
    private void VerifySharedOccupancy()
    {
        ResetWorld();
        int[] indices = { MiddleCell(), MiddleCell() + 1, MiddleCell() + ecology.CellsPerAxis };
        foreach (int index in indices)
        {
            // A Sitka cohort already occupies part of each cell; planting must
            // share the remaining capacity rather than exceed it.
            ForestRegenerationCohort other = ecology.Cells[index].GetOrCreateCohort(sitka);
            other.Density = sitka.RegenDensityMax * 0.5f;
            other.Height = 1f;
            other.EstablishYear = 0;
        }
        int planted = 0;
        foreach (int index in indices)
        {
            for (int k = 0; k < 5; k++)
            {
                // Re-planting the same cell is rejected; count only successes.
                if (ecology.TryPlantBeech(CellCenter(index)).Success) planted++;
            }
        }
        float maxOccupancy = ecology.Cells.Max(c => c.SharedOccupancy);
        Require(maxOccupancy <= 1.000001f, "shared occupancy exceeded one: " + maxOccupancy);
        Require(planted == indices.Length, "expected one successful planting per prepared cell, got " + planted);
        Debug.Log("PLANTING_OCCUPANCY_PASS max=" + maxOccupancy.ToString("0.000000", CultureInfo.InvariantCulture)
            + " planted=" + planted);
    }

    // ---------------------------------------------------------------------
    // Pure Beech control: the accepted four-parent fixture must be unchanged.
    // ---------------------------------------------------------------------
    private void VerifyPureBeechRegression()
    {
        ResetWorld();
        for (int i = 0; i < 4; i++)
            spawner.Spawn("B" + i, beech, new Vector3((i % 2) * 12f - 6f, 0f, (i / 2) * 12f - 6f), 60, 40f, 20f, 4f);
        ecology.ResetForDeterministicRun();

        int establishment = 0, promotion = 0, positiveMaturity = 0, secondGenerationSeed = 0;
        float maxRain = 0f, maxDensity = 0f, maxHeight = 0f, maxOccupancy = 0f;
        for (int year = 1; year <= 150; year++)
        {
            ecology.AdvanceOneYear();
            ForestTree[] recruits = Trees().Where(t => t.TreeId.StartsWith("R", StringComparison.Ordinal)).ToArray();
            if (establishment == 0 && ecology.Cells.Any(c => c.HasRegeneration)) establishment = year;
            if (promotion == 0 && recruits.Length > 0) promotion = year;
            if (positiveMaturity == 0 && recruits.Any(t => beech.Maturity(t.AgeYears) > 0f)) positiveMaturity = year;
            if (secondGenerationSeed == 0 && recruits.Any(t => ecology.GetSeedPotential(t) > 0f)) secondGenerationSeed = year;
            foreach (ForestEcologyCell cell in ecology.Cells)
            {
                maxOccupancy = Mathf.Max(maxOccupancy, cell.SharedOccupancy);
                foreach (ForestRegenerationCohort cohort in cell.Regeneration)
                {
                    Require(cohort.SpeciesId == beech.SpeciesId, "foreign cohort in pure Beech fixture");
                    maxRain = Mathf.Max(maxRain, cohort.SeedRain);
                    maxDensity = Mathf.Max(maxDensity, cohort.Density);
                    maxHeight = Mathf.Max(maxHeight, cohort.Height);
                }
            }
            Require(maxOccupancy <= 1.000001f, "pure Beech exceeded shared occupancy");
        }
        ForestTree[] final = LivingTrees();
        Require(establishment > 0 && promotion > establishment && secondGenerationSeed > promotion, "pure Beech lifecycle incomplete");
        Debug.Log("PLANTING_PURE_BEECH parents=4 age=60 establishment=" + establishment + " promotion=" + promotion
            + " firstPositiveMaturity=" + positiveMaturity + " secondGenerationSeedPotential=" + secondGenerationSeed
            + " trees=" + final.Length + " reproductive=" + final.Count(t => beech.Maturity(t.AgeYears) > 0f)
            + " maxRain=" + maxRain.ToString("R", CultureInfo.InvariantCulture)
            + " maxCellDensity=" + maxDensity.ToString("R", CultureInfo.InvariantCulture)
            + " maxHeight=" + maxHeight.ToString("R", CultureInfo.InvariantCulture)
            + " maxOccupancy=" + maxOccupancy.ToString("R", CultureInfo.InvariantCulture));
    }

    // ---------------------------------------------------------------------
    // Controlled qualitative scenarios. Site-moisture scenarios (F/G) are not
    // representable yet: every cell carries the same SiteProductivity, so a
    // wet/dry contrast cannot be simulated in this slice.
    // ---------------------------------------------------------------------
    private void VerifySanityScenarios()
    {
        // A. No Beech seed source: natural Beech must never appear.
        ForestStandScenarios.ApplyLifecycleFixture();
        RunYears(100);
        Require(BeechTreeCount() == 0, "Beech appeared without a seed source");
        Require(CountBeechCohortCells() == 0, "Beech cohorts appeared without a seed source");
        Debug.Log("PLANTING_SCENARIO_A noSeedSource=True beechTrees=0 beechCohortCells=0");

        // B. Dense/unthinned Sitka + Beech seed source.
        ForestStandScenarios.ApplyLifecycleFixture();
        SpawnBeechSources(4);
        RunYears(100);
        int denseTrees = BeechTreeCount();
        int denseCells = CountBeechCohortCells();
        float denseOccupancy = MaxOccupancy();
        Require(denseCells > 0 || denseTrees > 0, "dense Sitka + source produced no Beech");
        Require(denseOccupancy <= 1.000001f, "dense scenario exceeded shared occupancy");
        Debug.Log("PLANTING_SCENARIO_B denseSitkaWithSource=True beechTrees=" + denseTrees
            + " beechCohortCells=" + denseCells
            + " maxOccupancy=" + denseOccupancy.ToString("0.000000", CultureInfo.InvariantCulture));

        // C. Repeated light thinning of the Sitka matrix.
        ForestStandScenarios.ApplyLifecycleFixture();
        SpawnBeechSources(4);
        for (int decade = 0; decade < 10; decade++)
        {
            RunYears(10);
            ThinLargestSitka(0.1f);
        }
        int thinTrees = BeechTreeCount();
        int thinCells = CountBeechCohortCells();
        Require(MaxOccupancy() <= 1.000001f, "thinning scenario exceeded shared occupancy");
        Debug.Log("PLANTING_SCENARIO_C repeatedThinning=True beechTrees=" + thinTrees + " beechCohortCells=" + thinCells);

        // E. Large bright opening with a Beech seed source only.
        ResetWorld();
        SpawnBeechSources(4);
        RunYears(100);
        int openTrees = BeechTreeCount();
        Require(openTrees > 0, "bright opening produced no Beech recruits");
        Require(MaxOccupancy() <= 1.000001f, "opening scenario exceeded shared occupancy");
        Debug.Log("PLANTING_SCENARIO_E brightOpening=True beechTrees=" + openTrees
            + " beechCohortCells=" + CountBeechCohortCells());

        // H. Initial Beech abundance changes recruitment.
        ResetWorld();
        SpawnBeechSources(1);
        RunYears(100);
        int oneSource = BeechTreeCount();
        ResetWorld();
        SpawnBeechSources(4);
        RunYears(100);
        int fourSources = BeechTreeCount();
        Require(fourSources >= oneSource, "more seed sources reduced Beech recruitment");
        Debug.Log("PLANTING_SCENARIO_H abundance=True oneSource=" + oneSource + " fourSources=" + fourSources);
    }

    // ---------------------------------------------------------------------
    // Full planted lifecycle: juvenile -> promotion -> maturity -> seed ->
    // natural cohort.
    // ---------------------------------------------------------------------
    private void VerifyFullLifecycle()
    {
        ResetWorld();
        int index = MiddleCell();
        PlantingResult planted = ecology.TryPlantBeech(CellCenter(index));
        Require(planted.Success, "lifecycle planting failed");

        bool promoted = false;
        int promotionYear = -1;
        int seedYear = -1;
        int naturalCohortCells = 0;
        for (int year = 1; year <= 200; year++)
        {
            ecology.AdvanceOneYear();
            ForestTree source = Trees().FirstOrDefault(t => t.Species == beech && t.TreeId.StartsWith("PL", StringComparison.Ordinal));
            if (source != null && !promoted)
            {
                promoted = true;
                promotionYear = year;
            }
            if (source != null && promoted && seedYear < 0 && ecology.GetSeedPotential(source) > 0f)
                seedYear = year;
            naturalCohortCells = CountNaturalBeechCohortCells();
            if (promoted && seedYear > 0 && naturalCohortCells > 0) break;
        }
        Require(promoted, "planted Beech never promoted");
        Require(seedYear > 0, "planted Beech never produced seed");
        Require(naturalCohortCells > 0, "planted Beech never produced a natural Beech cohort");
        Require(MaxOccupancy() <= 1.000001f, "lifecycle exceeded shared occupancy");
        Debug.Log("PLANTING_LIFECYCLE_PASS promotionYear=" + promotionYear + " firstSeedYear=" + seedYear
            + " naturalCohortCells=" + naturalCohortCells
            + " naturalTrees=" + Trees().Count(t => t.Species == beech && t.TreeId.StartsWith("R", StringComparison.Ordinal)));
    }

    // ---------------------------------------------------------------------
    // Save/reload while the planted Beech is still juvenile. Only persisted
    // state is compared; derived seed rain is excluded.
    // ---------------------------------------------------------------------
    private IEnumerator VerifySaveReload()
    {
        ResetWorld();
        int index = MiddleCell();
        ecology.Cells[index].Light = 0.2f;
        PlantingResult planted = ecology.TryPlantBeech(CellCenter(index));
        Require(planted.Success, "save/reload planting failed");
        ecology.AdvanceOneYear();
        string before = PersistedSnapshot();

        saves.Save();
        ResetWorld();
        saves.Load();
        yield return null;
        yield return null;

        string after = PersistedSnapshot();
        Require(before == after, "persisted planting state changed after reload\nbefore=" + before + "\nafter=" + after);
        ForestRegenerationCohort cohort = ecology.Cells[index].FindCohort(beech.SpeciesId);
        Require(cohort != null, "planted cohort missing after reload");
        Require(cohort.Origin == RegenerationOrigin.Planted, "planted origin not restored");
        Require(cohort.OriginYear == ecology.EcologicalYear - 1, "planted origin year not restored");
        Require(cohort.EstablishYear == ecology.EcologicalYear - 1 - ecology.PlantedJuvenileAgeYears, "planted age not restored");

        // One post-load annual step must match an uninterrupted run.
        ResetWorld();
        ecology.Cells[index].Light = 0.2f;
        Require(ecology.TryPlantBeech(CellCenter(index)).Success, "continuation setup failed");
        ecology.AdvanceOneYear();
        saves.Save();
        ecology.AdvanceOneYear();
        string uninterrupted = PersistedSnapshot();
        ResetWorld();
        saves.Load();
        yield return null;
        yield return null;
        ecology.AdvanceOneYear();
        Require(uninterrupted == PersistedSnapshot(), "post-load step diverged from uninterrupted run");
        Debug.Log("PLANTING_SAVELOAD_PASS persistedRestored=True nextStepMatches=True");
    }

    // ---------------------------------------------------------------------
    // Helpers
    // ---------------------------------------------------------------------
    private void ResetWorld()
    {
        foreach (ForestTree tree in Trees()) DestroyImmediate(tree.gameObject);
        ecology.ResetForDeterministicRun();
    }

    private int MiddleCell()
    {
        int perAxis = Mathf.Max(1, ecology.CellsPerAxis);
        int half = perAxis / 2;
        return half * perAxis + half;
    }

    private Vector3 CellCenter(int index)
    {
        Vector2 center = ecology.Cells[index].Center;
        return new Vector3(center.x, 0f, center.y);
    }

    private void SpawnBeechSources(int count)
    {
        Vector3[] positions =
        {
            new Vector3(-10f, 0f, -10f),
            new Vector3(-2f, 0f, -10f),
            new Vector3(6f, 0f, -10f),
            new Vector3(10f, 0f, -6f)
        };
        for (int i = 0; i < count; i++)
        {
            Vector3 position = positions[Mathf.Clamp(i, 0, positions.Length - 1)];
            float dbh = 16f + (i % 2) * 2f;
            spawner.Spawn("B-" + i, beech, position, 45, dbh, 10f, beech.PotentialCrownRadiusM(dbh));
        }
        ecology.InvalidateCompetition();
        ecology.RecomputeCanopy();
        ecology.RecomputeSeedRain();
    }

    private void RunYears(int years)
    {
        for (int i = 0; i < years; i++) ecology.AdvanceOneYear();
    }

    private void ThinLargestSitka(float fraction)
    {
        var sitkaTrees = LivingTrees().Where(t => t.Species == sitka).OrderByDescending(t => t.Diameter).ToList();
        int remove = Mathf.FloorToInt(sitkaTrees.Count * fraction);
        for (int i = 0; i < remove; i++)
            sitkaTrees[i].Fell();
    }

    private int BeechTreeCount() => LivingTrees().Count(t => t.Species == beech);

    private int CountBeechCohortCells()
    {
        int count = 0;
        foreach (ForestEcologyCell cell in ecology.Cells)
        {
            ForestRegenerationCohort cohort = cell.FindCohort(beech.SpeciesId);
            if (cohort != null && cohort.Density > 0f) count++;
        }
        return count;
    }

    private int CountNaturalBeechCohortCells()
    {
        int count = 0;
        foreach (ForestEcologyCell cell in ecology.Cells)
        {
            ForestRegenerationCohort cohort = cell.FindCohort(beech.SpeciesId);
            if (cohort != null && cohort.Density > 0f && cohort.Origin == RegenerationOrigin.Natural) count++;
        }
        return count;
    }

    private float MaxOccupancy() => ecology.Cells.Max(c => c.SharedOccupancy);

    private ForestTree[] LivingTrees() => Trees().Where(t => t != null && !t.IsStump).ToArray();

    private static ForestTree[] Trees() => FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None);

    private string PersistedSnapshot()
    {
        var builder = new StringBuilder();
        ForestEcologyCell[] cells = ecology.Cells;
        for (int i = 0; i < cells.Length; i++)
        {
            ForestEcologyCell cell = cells[i];
            if (cell == null) continue;
            var cohorts = cell.Regeneration
                .Where(c => c != null && c.Density > 0f)
                .OrderBy(c => c.SpeciesId, StringComparer.Ordinal);
            foreach (ForestRegenerationCohort cohort in cohorts)
            {
                builder.Append(i).Append('|').Append(cohort.SpeciesId)
                    .Append('|').Append(cohort.Density.ToString("R", CultureInfo.InvariantCulture))
                    .Append('|').Append(cohort.Height.ToString("R", CultureInfo.InvariantCulture))
                    .Append('|').Append(cohort.EstablishYear)
                    .Append('|').Append((int)cohort.Origin)
                    .Append('|').Append(cohort.OriginYear)
                    .Append(';');
            }
        }
        return builder.ToString();
    }

    private static void InvokePrivate(object target, string method, params object[] args)
    {
        MethodInfo info = target.GetType().GetMethod(method, BindingFlags.Instance | BindingFlags.NonPublic);
        Require(info != null, "missing private method " + method);
        info.Invoke(target, args);
    }

    private void RestoreUserSave()
    {
        if (string.IsNullOrEmpty(savePath)) return;
        if (hadPreviousSave) File.WriteAllBytes(savePath, previousSave);
        else if (File.Exists(savePath)) File.Delete(savePath);
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
