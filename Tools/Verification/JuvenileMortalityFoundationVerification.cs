#if UNITY_EDITOR
using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Copy this file into Assets/ForestPrototype for an explicit batch run; remove
// the copy and generated meta afterwards. This gate never writes the save slot.
public static class JuvenileMortalityFoundationVerification
{
    private const string Requested = "JuvenileMortalityFoundationVerification.Requested";
    public static void Begin()
    {
        EditorPrefs.SetBool(Requested, true);
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        EditorApplication.isPlaying = true;
    }
    public static void BeginCanonical()
    {
        EditorPrefs.SetBool("JuvenileMortalityFoundationVerification.Canonical", true);
        Begin();
    }
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (!EditorPrefs.GetBool(Requested, false)) return;
        EditorPrefs.SetBool(Requested, false);
        new GameObject("Juvenile mortality foundation gate").AddComponent<JuvenileMortalityFoundationGate>();
    }
}

public sealed class JuvenileMortalityFoundationGate : MonoBehaviour
{
    private static void Check(bool ok, string message) { if (!ok) throw new InvalidOperationException(message); }
    private static object Invoke(object target, string name, params object[] args)
    {
        MethodInfo method = target.GetType().GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic);
        Check(method != null, "Missing verification entry point " + name);
        return method.Invoke(target, args);
    }
    private IEnumerator Start()
    {
        yield return null;
        Exception failure = null;
        bool canonical = EditorPrefs.GetBool("JuvenileMortalityFoundationVerification.Canonical", false);
        EditorPrefs.SetBool("JuvenileMortalityFoundationVerification.Canonical", false);
        IEnumerator run = canonical ? VerifyCanonical() : Verify();
        while (true)
        {
            bool more; object current = null;
            try { more = run.MoveNext(); if (more) current = run.Current; }
            catch (Exception error) { failure = error; break; }
            if (!more) break;
            yield return current;
        }
        if (failure == null) Debug.Log(canonical ? "JUVENILE_CANONICAL_VERIFY_PASS" : "JUVENILE_MORTALITY_FOUNDATION_VERIFY_PASS");
        else Debug.LogError("JUVENILE_MORTALITY_FOUNDATION_VERIFY_FAIL " + failure);
        EditorApplication.ExitPlaymode(); EditorApplication.Exit(failure == null ? 0 : 1);
    }

    private static ForestTree FindTree(string id) => FindObjectsByType<ForestTree>(FindObjectsInactive.Include,
        FindObjectsSortMode.None).Single(tree => tree.TreeId == id);

    private IEnumerator VerifyCanonical()
    {
        var ecology = FindFirstObjectByType<ForestEcologyController>();
        MethodInfo hashMethod = typeof(ScenarioOneInteractionGate).GetMethod("LifecycleHash", BindingFlags.Static | BindingFlags.NonPublic);
        // Scenario One applies its [C] browse pressure (0.2). The neutral anchor
        // runs with browsing explicitly off; normal play runs with the scenario value.
        float scenarioPressure = ecology.Browsing.BackgroundPressure;
        string hash = null, scenarioHash = null;
        foreach (float pressure in new[] { 0f, scenarioPressure })
        {
            ForestStandScenarios.ApplyLifecycleFixture();
            ecology.RngModelVersion = SimulationRandom.LegacyModel;
            ecology.Browsing.BackgroundPressure = pressure;
            for (int year = 0; year < 80; year++)
            {
                ecology.AdvanceOneYear();
                if (year % 10 == 9) yield return null;
            }
            string result = (string)hashMethod.Invoke(null, new object[] { ecology });
            if (pressure == 0f && hash == null) hash = result; else scenarioHash = result;
        }
        ecology.Browsing.BackgroundPressure = scenarioPressure;
        Debug.Log("CANONICAL_ISOLATION_HASH " + hash);
        Debug.Log($"SCENARIO_ONE_LIFECYCLE_HASH pressure={scenarioPressure:0.00} hash={scenarioHash}");
        // Calibrated canonical lifecycle (was 7E39B70A14959FAD before C8 + k10a10).
        Check(hash == "BFC55473C1506067", "Canonical lifecycle drift: " + hash);
        // Normal Scenario One play with low browse pressure 0.2 [C].
        Check(scenarioPressure == 0.2f && scenarioHash == "3485B6630C9EA448",
            $"Scenario One low-browsing lifecycle drift: pressure={scenarioPressure} hash={scenarioHash}");
    }

    private IEnumerator Verify()
    {
        var ecology = FindFirstObjectByType<ForestEcologyController>();
        var manager = FindFirstObjectByType<ScenarioOneManager>();
        var saves = FindFirstObjectByType<ForestSaveController>();
        var spawner = FindFirstObjectByType<ForestTreeSpawner>();
        var marks = FindFirstObjectByType<ForestTreeMarkingManager>();
        Check(ecology != null && manager != null && saves != null && spawner != null && marks != null, "Scene controllers missing");
        ForestSaveData original = saves.CaptureData();
        string originalHash = ScenarioReferenceArchive.WorldHash(original);
        // These checks assert exact light-only identities between storage forms.
        // Browsing (Scenario One pressure 0.2) is realised per individual and as
        // an expected fraction per cohort, so it is verified statistically by
        // BrowsingProtectionVerification instead; run this gate browse-neutral.
        float scenarioPressure = ecology.Browsing.BackgroundPressure;
        ecology.Browsing.BackgroundPressure = 0f;

        int combinations = 0;
        foreach (TreeSpeciesDefinition species in spawner.KnownSpecies.Where(s => s.SupportsRegeneration))
        foreach (float light in new[] { 0f, 0.1f, 0.25f, 0.55f, 1f })
        foreach (float site in new[] { 0.6f, 1f })
        {
            ecology.RestoreEcologyState(1, original.simulationSeed, 0);
            ForestEcologyCell naturalCell = ecology.Cells[0], legacyCell = ecology.Cells[1], exactCell = ecology.Cells[2];
            foreach (ForestEcologyCell cell in new[] { naturalCell, legacyCell, exactCell })
            { cell.Light = light; cell.SiteProductivity = site; }
            ForestRegenerationCohort natural = naturalCell.GetOrCreateCohort(species);
            natural.Restore(0.5f, 0.6f, 0, RegenerationOrigin.Natural, 0);
            ForestRegenerationCohort legacy = legacyCell.GetOrCreateCohort(species);
            legacy.Restore(0.5f, 0.6f, -2, RegenerationOrigin.Planted, 0);
            Vector3 exactPosition = new Vector3(exactCell.Center.x + 0.1f, 0, exactCell.Center.y - 0.2f);
            ScenarioOneSaveData state = JsonUtility.FromJson<ScenarioOneSaveData>(JsonUtility.ToJson(original.scenarioOne));
            state.interactionSchemaVersion = 2;
            state.plantedJuveniles.Clear();
            state.plantedJuveniles.Add(new PlantedJuvenileSaveData { juvenileId = "PJ9000", speciesId = species.SpeciesId,
                position = exactPosition, cellIndex = 2, ageYears = 3, heightMeters = 0.6f, alive = true });
            manager.RestoreSaveData(state);
            Invoke(ecology, "GrowExistingRegeneration");
            Invoke(manager, "AdvancePlantedJuveniles");
            PlantedJuvenile exact = manager.PlantedJuveniles.Single();
            Check(natural.Height == legacy.Height && natural.Height == exact.heightMeters,
                "Origin/storage changed juvenile height calculations: " + species.SpeciesId);
            Check(natural.Density == legacy.Density && natural.Origin == RegenerationOrigin.Natural
                && legacy.Origin == RegenerationOrigin.Planted && legacy.EstablishYear == -2,
                "Legacy cohort-managed planting changed origin/history or survival");
            float response = species.JuvenileSurvivalResponse(light);
            Check(exact.alive == (SimulationRandom.Roll(0, exact.juvenileId, 1, original.simulationSeed) < response),
                "Individual survival did not realise the same species survival response");
            Check(exact.position == exactPosition && exact.juvenileId == "PJ9000", "Exact juvenile moved or lost identity");
            Check(JuvenileEcologyRules.LightResponse(species, light) == species.JuvenileLightResponse(light)
                && (float)JuvenileEcologyRules.SurvivalResponse(species, light) == response,
                "Shared rule differs from existing species responses");
            Check(JuvenileEcologyRules.CanPromote(species, species.PromotionHeightM, species.PromotionMinimumLight)
                && !JuvenileEcologyRules.CanPromote(species, species.PromotionHeightM - 0.01f, 1f),
                "Shared promotion boundary is incorrect");
            if (species.PromotionMinimumLight > 0f)
                Check(!JuvenileEcologyRules.CanPromote(species, species.PromotionHeightM + 0.1f,
                    species.PromotionMinimumLight - 0.001f), "Shared promotion ignored minimum light");
            // Existing cohort capacity stays normalized; it is not silently
            // converted into a new stems-per-cell cap on exact-position stock.
            naturalCell.AddDensityWithSharedCapacity(natural, species.RegenDensityMax * 10f);
            Check(naturalCell.SharedOccupancy <= 1.0001f, "Cohort capacity exceeded its existing limit");
            combinations++;
        }
        Check(combinations >= 30, "Not enough species/environment fixtures tested");
        Debug.Log("SHARED_JUVENILE_RULES_VERIFY_PASS combinations=" + combinations + " origins=Natural,Planted exactPosition=True");

        ecology.RestoreEcologyState(5, original.simulationSeed, 1);
        TreeSpeciesDefinition promotedSpecies = spawner.ResolveSpecies("beech");
        Check(promotedSpecies != null && promotedSpecies.SupportsRegeneration, "Beech unavailable for promotion fixture");
        foreach (int cell in new[] { 10, 11, 12 }) ecology.Cells[cell].Light = 1f;
        var naturalPromotion = ecology.Cells[10].GetOrCreateCohort(promotedSpecies);
        naturalPromotion.Restore(0.5f, promotedSpecies.PromotionHeightM + 0.1f, 1, RegenerationOrigin.Natural, 1);
        var legacyPromotion = ecology.Cells[11].GetOrCreateCohort(promotedSpecies);
        legacyPromotion.Restore(0.5f, promotedSpecies.PromotionHeightM + 0.1f, -2, RegenerationOrigin.Planted, 1);
        Vector3 plantedPosition = new Vector3(ecology.Cells[12].Center.x + 0.31f, 0, ecology.Cells[12].Center.y - 0.27f);
        string id = null;
        for (int i = 1; i < 1000; i++)
        {
            string candidate = "PJ" + (9100 + i);
            if (SimulationRandom.Roll(1, candidate, 5, original.simulationSeed) < promotedSpecies.JuvenileSurvivalResponse(1f))
            { id = candidate; break; }
        }
        Check(id != null, "Could not construct a deterministic surviving juvenile fixture");
        ScenarioOneSaveData promotionState = JsonUtility.FromJson<ScenarioOneSaveData>(JsonUtility.ToJson(original.scenarioOne));
        promotionState.interactionSchemaVersion = 2;
        promotionState.plantedJuveniles.Clear();
        promotionState.plantedJuveniles.Add(new PlantedJuvenileSaveData { juvenileId = id, speciesId = promotedSpecies.SpeciesId,
            position = plantedPosition, cellIndex = 12, ageYears = 3, heightMeters = promotedSpecies.PromotionHeightM + 0.1f });
        manager.RestoreSaveData(promotionState);
        Invoke(ecology, "PromoteCohorts", spawner.DefaultSpecies, SimulationRandom.Create(1, original.simulationSeed, 5, 0));
        Invoke(manager, "AdvancePlantedJuveniles");
        ForestTree naturalTree = FindTree("R5-10-beech"), legacyTree = FindTree("PL5-11-beech");
        PlantedJuvenile promoted = manager.PlantedJuveniles.Single();
        ForestTree exactTree = FindTree("PL-" + id);
        Check(naturalTree.Species == promotedSpecies && legacyTree.Species == promotedSpecies
            && exactTree.Species == promotedSpecies && exactTree.transform.position == plantedPosition
            && promoted.promotedTreeId == exactTree.TreeId, "Promotion lost species/origin/position semantics");
        Check(naturalPromotion.Density == 0 && legacyPromotion.Density == 0, "Promoted cohort storage remained occupied");
        ForestSaveData promotedSave = saves.CaptureData();
        Check(saves.LoadData(JsonUtility.FromJson<ForestSaveData>(JsonUtility.ToJson(promotedSave)), false), "Promoted save rejected");
        yield return null;
        Check(FindTree(exactTree.TreeId).transform.position == plantedPosition
            && manager.PlantedJuveniles.Single().promotedTreeId == exactTree.TreeId && ecology.RngModelVersion == 1,
            "Promotion or RNG-version persistence regressed");
        // Early v13 duplicate juvenile records must stay cohort-managed, not
        // advance a second individual or promote a second tree.
        var legacyData = JsonUtility.FromJson<ScenarioOneSaveData>(JsonUtility.ToJson(promotionState));
        legacyData.interactionSchemaVersion = 1;
        manager.RestoreSaveData(legacyData);
        float legacyHeight = manager.PlantedJuveniles.Single().heightMeters;
        Invoke(manager, "AdvancePlantedJuveniles");
        Check(manager.PlantedJuveniles.Single().legacyCohortManaged
            && manager.PlantedJuveniles.Single().heightMeters == legacyHeight,
            "Early v13 cohort-managed record advanced twice");
        Debug.Log("JUVENILE_PROMOTION_PERSISTENCE_VERIFY_PASS natural=True legacyPlanted=True exact=True rngModel1=True");

        // Isolate two tree individuals so canopy/competition differences are
        // observable rather than saturated by the 336-tree starting stand.
        foreach (ForestTree tree in FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            DestroyImmediate(tree.gameObject);
        ecology.ResetForDeterministicRun(); ecology.RngModelVersion = 0;
        manager.RestoreSaveData(original.scenarioOne);
        Vector3 position = new Vector3(ecology.Cells[27].Center.x, 0, ecology.Cells[27].Center.y);
        ForestTree focal = spawner.Spawn("MORTALITY-FOCAL", spawner.DefaultSpecies, position, 45, 30, 15, 4);
        // 6 m: inside the calibrated 8 m Hegyi cutoff (was 12 m under the 20 m
        // cutoff) and outside the neighbour's own 4.5 m shade reach, so the
        // focal cell still opens when the focal tree dies.
        ForestTree neighbour = spawner.Spawn("MORTALITY-NEIGHBOUR", spawner.DefaultSpecies, position + Vector3.right * 6f, 45, 20, 12, 2);
        ecology.InvalidateCompetition(); ecology.RecomputeCanopy(); ecology.RecomputeSeedRain();
        int focalCell = ecology.GetCellIndex(position);
        float canopyBefore = ecology.Cells[focalCell].Canopy, ciBefore = ecology.GetCompetitionIndex(neighbour);
        float seedBefore = ecology.GetSeedRain(focalCell, spawner.DefaultSpecies);
        Check(canopyBefore > 0.9f && ciBefore > 0 && ecology.GetSeedPotential(focal) > 0, "Mortality fixture has no living influence");
        ForestSaveData livingSave = saves.CaptureData();
        long cash = manager.CashCents;
        float timber = manager.RetainedTimberM3;
        int deadwood = manager.DeadwoodRecords.Count, events = manager.ManagementEvents.Count;
        int fellCalls = 0, mortalityCalls = 0;
        Action<ForestTree> onFell = tree => fellCalls++;
        Action<ForestTree> onMortality = tree => mortalityCalls++;
        ForestTree.Felled += onFell; ForestTree.MortalityApplied += onMortality;
        try
        {
            marks.Mark(focal, TreeMarkType.Fell, false);
            Check(focal.ApplyMortality("verification-explicit", ecology.EcologicalYear), "Explicit death was not applied");
            Check(!focal.IsLiving && focal.IsBiologicallyDead && !focal.IsStump
                && focal.MortalityCause == "verification-explicit" && focal.MortalityYear == 0,
                "Biological death was not separate from harvest state");
            Check(ecology.LivingTreeCount == 1 && ecology.GetCompetitionIndex(neighbour) < ciBefore
                && ecology.GetCompetitionIndex(focal) == 0 && ecology.Cells[focalCell].Canopy < canopyBefore
                && ecology.Cells[focalCell].Light > 0 && ecology.GetSeedPotential(focal) == 0
                && ecology.GetSeedRain(focalCell, spawner.DefaultSpecies) < seedBefore,
                "Dead tree retained living ecology contributions");
            Check(!focal.CanChop && focal.CanPrune(2.5f, 1) != null && marks.GetMarkedIds().Count == 0,
                "Mortality retained management eligibility/marks");
            focal.Fell(); marks.Mark(focal, TreeMarkType.CropTree, false);
            Check(!focal.ApplyMortality("another-cause", 10) && mortalityCalls == 1 && fellCalls == 0
                && focal.MortalityCause == "verification-explicit" && focal.MortalityYear == 0,
                "Death was duplicated, overwritten, or masqueraded as felling");
            Check(manager.CashCents == cash && manager.RetainedTimberM3 == timber && manager.DeadwoodRecords.Count == deadwood
                && manager.ManagementEvents.Count == events, "Mortality awarded resources or invented management/deadwood outcomes");
            // Defensive enumeration boundary: reactivating a dead record must
            // not make its canopy, competition or seed production living again.
            focal.gameObject.SetActive(true);
            ecology.InvalidateCompetition(); ecology.RecomputeCanopy(); ecology.RecomputeSeedRain();
            Check(ecology.LivingTreeCount == 1 && ecology.GetCompetitionIndex(focal) == 0
                && ecology.GetSeedPotential(focal) == 0, "Reactivation revived biological contributions");
            focal.RefreshVisuals();
            ForestSaveData deadSave = saves.CaptureData();
            Check(deadSave.version == ForestSaveData.CurrentVersion && deadSave.version >= 14 && deadSave.trees.Single(t => t.treeId == focal.TreeId).biologicallyDead,
                "Inactive dead record missing from save");
            DestroyImmediate(focal.gameObject);
            Check(saves.LoadData(JsonUtility.FromJson<ForestSaveData>(JsonUtility.ToJson(deadSave)), false), "Mortality save rejected");
            yield return null;
            focal = FindTree("MORTALITY-FOCAL");
            Check(focal.IsBiologicallyDead && !focal.gameObject.activeSelf && focal.MortalityYear == 0
                && focal.MortalityCause == "verification-explicit" && mortalityCalls == 1 && fellCalls == 0,
                "Missing-tree mortality restore lost state or emitted a new event");
            ForestSaveData bad = JsonUtility.FromJson<ForestSaveData>(JsonUtility.ToJson(deadSave));
            bad.trees.Single(t => t.biologicallyDead).mortalityCause = "";
            Check(!saves.LoadData(bad, false) && ecology.LivingTreeCount == 1, "Invalid mortality record changed the world");
            ForestSaveData legacySave = JsonUtility.FromJson<ForestSaveData>(JsonUtility.ToJson(livingSave));
            legacySave.version = 13;
            Check(saves.LoadData(legacySave, false), "Legacy current save rejected after mortality");
            yield return null;
            focal = FindTree("MORTALITY-FOCAL");
            Check(focal.IsLiving && !focal.IsBiologicallyDead && focal.MortalityYear == -1
                && focal.gameObject.activeSelf && ecology.LivingTreeCount == 2,
                "Legacy restore did not clear a previous timeline's death");
            ecology.BeginChangeBatch();
            float beforeBatch = ecology.Cells[focalCell].Canopy;
            focal.ApplyMortality("verification-batched", 0);
            Check(ecology.Cells[focalCell].Canopy == beforeBatch && ecology.GetSeedPotential(focal) == 0,
                "Mortality failed to defer expensive recomputation or immediately exclude seed potential");
            ecology.EndChangeBatch();
            Check(ecology.Cells[focalCell].Canopy < beforeBatch, "Mortality batch did not flush canopy removal");
            Check(saves.LoadData(livingSave, false), "Could not restore live foundation fixture");
            yield return null;
            focal = FindTree("MORTALITY-FOCAL");
            focal.RestoreSuppressionHistory(1000f);
            for (int year = 0; year < 5; year++) ecology.AdvanceOneYear();
            Check(focal.IsLiving && FindTree("MORTALITY-NEIGHBOUR").IsLiving,
                "Foundation activated an unauthorised annual mortality trigger");
        }
        finally { ForestTree.Felled -= onFell; ForestTree.MortalityApplied -= onMortality; }
        Debug.Log("TREE_MORTALITY_FOUNDATION_VERIFY_PASS causeYear=True persistent=True livingExclusion=True noHarvest=True noDeadwoodPose=True batch=True noAnnualTrigger=True");
        ecology.Browsing.BackgroundPressure = scenarioPressure;
        Check(saves.LoadData(original, false), "Original world restore rejected");
        yield return null;
        Check(ScenarioReferenceArchive.WorldHash(saves.CaptureData()) == originalHash,
            "Foundation gate altered the captured player world");
    }
}
#endif
