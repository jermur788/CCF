#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Explicit batchmode gate; never starts automatically in ordinary play.
public static class ScenarioOneInteractionVerification
{
    private const string Requested = "ScenarioOneInteractionVerification.Requested";

    public static void Begin()
    {
        EditorPrefs.SetBool(Requested, true);
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        EditorApplication.isPlaying = true;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (!EditorPrefs.GetBool(Requested, false)) return;
        EditorPrefs.SetBool(Requested, false);
        new GameObject("Scenario One interaction gate").AddComponent<ScenarioOneInteractionGate>();
    }
}

public sealed class ScenarioOneInteractionGate : MonoBehaviour
{
    private IEnumerator Start()
    {
        yield return null;
        Exception failure = null;
        IEnumerator verify = Verify();
        while (true)
        {
            object current = null;
            bool more;
            try { more = verify.MoveNext(); if (more) current = verify.Current; }
            catch (Exception error) { failure = error; break; }
            if (!more) break;
            yield return current;
        }
        if (failure == null) Debug.Log("SCENARIO_ONE_INTERACTION_VERIFY_PASS");
        else Debug.LogError("SCENARIO_ONE_INTERACTION_VERIFY_FAIL: " + failure);
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(failure == null ? 0 : 1);
    }

    private static void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }

    private static object Invoke(object target, string method, params object[] args)
    {
        MethodInfo action = target.GetType().GetMethod(method, BindingFlags.Instance | BindingFlags.NonPublic);
        Check(action != null, method + " unavailable");
        return action.Invoke(target, args);
    }

    private static float DistanceXZ(Vector3 a, Vector3 b) => Vector2.Distance(
        new Vector2(a.x, a.z), new Vector2(b.x, b.z));

    private IEnumerator Verify()
    {
        ScenarioOneManager manager = FindFirstObjectByType<ScenarioOneManager>();
        ForestEcologyController ecology = FindFirstObjectByType<ForestEcologyController>();
        ForestSaveController saves = FindFirstObjectByType<ForestSaveController>();
        ForestTreeMarkingManager marks = FindFirstObjectByType<ForestTreeMarkingManager>();
        ForestPlayer player = FindFirstObjectByType<ForestPlayer>();
        Check(manager != null && ecology != null && saves != null && marks != null && player != null,
            "ForestTest scenario components missing");
        Check(ecology.LivingTreeCount == 336, "v12 starting stand changed");
        ScenarioReferenceArchive archive = ScenarioReferenceArchive.Load();
        Check(archive != null && archive.Matches(manager.Definition, ecology)
            && archive.AtYear(0).verifiedFrozenWorld && archive.AtYear(100).verifiedFrozenWorld
            && archive.AtYear(100).worldHash == "7AD177B3CC2F73C7"
            && archive.scheduleHash == "56C8B99FA1E8DDD1", "frozen v12 reference archive cannot be verified");
        string beforePreview = ScenarioReferenceArchive.WorldHash(saves.CaptureData());
        Check(manager.TryBeginReferencePreview(20) && ecology.EcologicalYear == 20,
            "frozen v12 future could not be walked from v13");
        manager.EndReferencePreview();
        yield return null;
        Check(ecology.EcologicalYear == 0
            && ScenarioReferenceArchive.WorldHash(saves.CaptureData()) == beforePreview,
            "historical reference preview changed the player's v13 forest");

        ForestTree[] eligible = FindObjectsByType<ForestTree>(FindObjectsSortMode.None)
            .Where(tree => tree != null && tree.CanChop && tree.CanPrune(2.5f, 1) == null)
            .OrderBy(tree => tree.TreeId, StringComparer.Ordinal).Take(3).ToArray();
        Check(eligible.Length == 3, "not enough biologically eligible trees");
        marks.Mark(eligible[0], TreeMarkType.CropTree, false);
        marks.Mark(eligible[1], TreeMarkType.CropTree, false);
        marks.Mark(eligible[2], TreeMarkType.Fell, false);
        Check(eligible[0].IsCropTree && !marks.GetMarkedIds().Contains(eligible[0].TreeId)
            && marks.GetMarkedIds().SequenceEqual(new[] { eligible[2].TreeId }), "red and blue marks were mixed");
        marks.Mark(eligible[0], TreeMarkType.Fell, false);
        Check(!eligible[0].IsCropTree && marks.GetMarkedIds().Contains(eligible[0].TreeId),
            "red did not replace blue on the same tree");
        marks.Mark(eligible[0], TreeMarkType.CropTree, false);
        Check(marks.GetMarkedIds().Count == 1 && marks.GetCropTreeIds().Count == 2,
            "blue did not replace red on the same tree");
        Check(manager.BatchPruneCropTrees() == 2, "batch did not designate both eligible Crop Trees");
        Check(manager.WorkOrders.Count(order => order.type == ScenarioWorkType.PruneTree && order.IsOpen) == 2,
            "batch did not generate two pruning tasks");
        manager.PlanningFellingOutcome = FellingMaterialOutcome.KeepForUse;
        Check(manager.AddMarkedTreesToWorkPlan() == 1 && marks.GetMarkedIds().Count == 0
            && eligible[0].IsCropTree && eligible[1].IsCropTree, "felling import cleared blue marks");
        Check(manager.ApprovePendingWork() && manager.AdvanceYear(), "batch approval/resolution failed");
        Check(manager.WorkOrders.Count(order => order.type == ScenarioWorkType.PruneTree
            && order.status == ScenarioWorkStatus.Completed) == 2, "eligible Crop Trees not pruned together");
        Check(manager.RetainedTimberM3 > 0f && manager.AnnualReports.Last().keptForUseVolumeM3 > 0f
            && manager.AnnualReports.Last().timberRevenueCents == 0L, "KeepForUse was sold or not stored");

        for (int i = 0; i < 2; i++) Check(manager.AdvanceYear(), "annual advance failed");
        ForestSaveData marked = saves.CaptureData();
        Check(marked.version == ForestSaveData.CurrentVersion && marked.cropTreeIds.Count == 2 && marked.markedTreeIds.Count == 0,
            "current save mixed red and blue IDs");
        marks.Unmark(eligible[0], false);
        saves.LoadData(JsonUtility.FromJson<ForestSaveData>(JsonUtility.ToJson(marked)), false);
        yield return null;
        Check(eligible[0].IsCropTree && eligible[1].IsCropTree && marks.GetCropTreeIds().Count == 2,
            "blue mark did not survive years and save/load");
        ForestSaveData oldV13Marks = JsonUtility.FromJson<ForestSaveData>(JsonUtility.ToJson(marked));
        oldV13Marks.markedTreeIds.Add(eligible[0].TreeId);
        oldV13Marks.cropTreeIds.Clear();
        saves.LoadData(oldV13Marks, false);
        yield return null;
        Check(eligible[0].IsCropTree && eligible[1].IsCropTree
            && marks.GetMarkedIds().Count == 0, "early v13 mixed mark lists overrode the saved tree types");

        // Use the real build action, but lower the test fixture's wood cost to
        // one unit so a single young harvested Sitka can finance it.
        ForestBuildable build = FindObjectsByType<ForestBuildable>(FindObjectsSortMode.None)
            .FirstOrDefault(item => !item.IsBuilt && item.IsPrerequisiteMet
                && (int)typeof(ForestBuildable).GetField("plankCost", BindingFlags.Instance | BindingFlags.NonPublic)
                    .GetValue(item) == 0);
        Check(build != null && manager.RetainedTimberM3 >= 0.1f, "no eligible construction resource probe");
        typeof(ForestBuildable).GetField("woodCost", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(build, 1);
        player.RestoreCarriedWood(0);
        foreach (ForestWoodStorage storage in FindObjectsByType<ForestWoodStorage>(FindObjectsSortMode.None))
            storage.RestoreStoredWood(0);
        float beforeBuild = manager.RetainedTimberM3;
        Invoke(build, "TryBuild");
        Check(build.IsBuilt && player.CarriedWood == 0
            && Mathf.Abs(manager.RetainedTimberM3 - (beforeBuild - 0.1f)) < 0.0001f,
            "construction did not debit retained timber exactly once");

        ForestTree[] treatmentTrees = FindObjectsByType<ForestTree>(FindObjectsSortMode.None)
            .Where(t => t != null && t.CanChop && !t.IsCropTree)
            .OrderBy(t => t.TreeId, StringComparer.Ordinal).Take(2).ToArray();
        Check(treatmentTrees.Length == 2, "insufficient trees for sell/deadwood regression");
        manager.PlanningFellingOutcome = FellingMaterialOutcome.SellAndExtract;
        marks.Mark(treatmentTrees[0], false);
        Check(manager.AddMarkedTreesToWorkPlan() == 1, "selling order not imported");
        manager.PlanningFellingOutcome = FellingMaterialOutcome.RetainAsFallenDeadwood;
        marks.Mark(treatmentTrees[1], false);
        Check(manager.AddMarkedTreesToWorkPlan() == 1 && manager.ApprovePendingWork()
            && manager.AdvanceYear(), "sell/deadwood contractor resolution failed");
        Check(manager.AnnualReports.Last().harvestedVolumeM3 > 0f
            && manager.AnnualReports.Last().timberRevenueCents > 0
            && manager.AnnualReports.Last().deadwoodCreated == 1
            && manager.DeadwoodRecords.Count == 1, "sell or fallen-deadwood outcome regressed");
        foreach (ForestTree felled in treatmentTrees)
        {
            Transform polished = felled.transform.Find("PolishedVisual");
            Transform stumpVisual = felled.transform.Find("StumpVisual");
            Check(felled.IsStump && (polished == null || !polished.gameObject.activeSelf)
                && stumpVisual != null && stumpVisual.gameObject.activeSelf,
                "felled tree still shows its standing visual");
        }

        // A red Fell mark must reach the annual plan without a hidden import
        // button: approval itself schedules any remaining marks.
        ForestTree autoImported = FindObjectsByType<ForestTree>(FindObjectsSortMode.None)
            .Where(t => t != null && t.CanChop && !t.IsCropTree)
            .OrderBy(t => t.TreeId, StringComparer.Ordinal).First();
        marks.Mark(autoImported, false);
        Check(manager.ApprovePendingWork() && manager.AdvanceYear(),
            "a Fell mark was silently skipped by approval/annual resolution");
        Check(autoImported.IsStump, "approved Fell mark did not fell its tree");

        Check(manager.TryPurchaseStock("sessile-oak-sapling", 2)
            && manager.TryPurchaseStock("beech-sapling", 1), "nursery purchase failed");
        Vector3 a = default, b = default, c = default;
        bool located = false;
        ForestTree[] standing = FindObjectsByType<ForestTree>(FindObjectsSortMode.None)
            .Where(t => t != null && !t.IsStump).ToArray();
        foreach (ForestEcologyCell cell in ecology.Cells.OrderByDescending(cell => cell.Light))
        {
            a = new Vector3(cell.Center.x - 0.35f, 0f, cell.Center.y);
            b = new Vector3(cell.Center.x + 0.35f, 0f, cell.Center.y);
            if (standing.All(t => DistanceXZ(a, t.transform.position) > 0.85f
                && DistanceXZ(b, t.transform.position) > 0.85f)) { located = true; break; }
        }
        Check(located && ecology.GetCellIndex(a) == ecology.GetCellIndex(b),
            "could not find a real opening for two overlapping planting sites");
        c = new Vector3(a.x, 0f, a.z + 1.25f);
        if (ecology.GetCellIndex(c) < 0 || standing.Any(t => DistanceXZ(c, t.transform.position) < 0.7f))
        {
            c = ecology.Cells.OrderByDescending(cell => cell.Light).Select(cell =>
                new Vector3(cell.Center.x, 0f, cell.Center.y))
                .First(p => DistanceXZ(p, a) > 1f && standing.All(t => DistanceXZ(p, t.transform.position) > 0.7f));
        }
        Check(manager.ReadyPlantingItemIds().Length == 2, "planting hotbar does not offer both owned species");
        Invoke(player, "EnterPlantingMode");
        Check((bool)typeof(ForestPlayer).GetField("isPlantingMode", BindingFlags.Instance | BindingFlags.NonPublic)
            .GetValue(player) && (string)Invoke(player, "SelectedPlantingSpeciesId") == "beech-sapling",
            "G-mode did not select ready Beech stock");
        Invoke(player, "CyclePlantingSpecies");
        Check((string)Invoke(player, "SelectedPlantingSpeciesId") == "sessile-oak-sapling",
            "R-mode did not cycle to owned Oak stock");
        Invoke(player, "PlantSelectedSpeciesAt", a);
        Check(!manager.TryDesignateExactPlanting("sessile-oak-sapling", a),
            "duplicate site was accepted despite its existing visible marker");
        Invoke(player, "PlantSelectedSpeciesAt", b);
        Check((string)Invoke(player, "SelectedPlantingSpeciesId") == "beech-sapling",
            "hotbar did not drop exhausted Oak and select the remaining Beech");
        Invoke(player, "PlantSelectedSpeciesAt", c);
        Check(!(bool)typeof(ForestPlayer).GetField("isPlantingMode", BindingFlags.Instance | BindingFlags.NonPublic)
            .GetValue(player) && manager.WorkOrders.Count(order => order.type == ScenarioWorkType.PlantJuvenile
                && order.IsOpen) == 3, "player placement did not create exactly three contractor markers");
        Check(manager.ReadyPlantingItemIds().Length == 0 && manager.GetReservedStockQuantity("sessile-oak-sapling") == 2,
            "pending markers did not reserve individual stock");
        Check(FindObjectsByType<Transform>(FindObjectsSortMode.None)
            .Count(t => t.name.StartsWith("Planting Marker #", StringComparison.Ordinal)) == 3,
            "world planting markers are not visible for all orders");

        var first = new PlantingClearancePatch { center = a, radiusMeters = Mathf.Sqrt(1f / Mathf.PI) };
        var second = new PlantingClearancePatch { center = b, radiusMeters = first.radiusMeters };
        ForestEcologyCell patchCell = ecology.Cells[ecology.GetCellIndex(a)];
        float half = ecology.CellSizeMeters * 0.5f;
        float one = ScenarioOneManager.ClearanceUnionArea(patchCell.Center, half, new[] { first });
        float twice = ScenarioOneManager.ClearanceUnionArea(patchCell.Center, half, new[] { first, second });
        Check(Mathf.Abs(one - 1f) < 0.04f && twice > one && twice < 1.9f,
            "1 m² disk or overlapping union area is incorrect");
        ForestEcologyCell interior = ecology.Cells[ecology.CellsPerAxis + 1];
        var onBoundary = new PlantingClearancePatch
        {
            center = new Vector3(interior.Center.x + half, 0f, interior.Center.y + half),
            radiusMeters = first.radiusMeters
        };
        float acrossCells = ecology.Cells.Sum(cell => ScenarioOneManager.ClearanceUnionArea(
            cell.Center, half, new[] { onBoundary }));
        Check(Mathf.Abs(acrossCells - 1f) < 0.04f,
            "clearance on a cell corner did not cover its four actual neighbours");
        ForestRegenerationCohort sitka = patchCell.GetOrCreateCohort(FindFirstObjectByType<ForestTreeSpawner>().DefaultSpecies);
        sitka.Restore(1f, 0.6f, ecology.EcologicalYear);
        Invoke(manager, "ApplyPlantingClearance", a, ecology.EcologicalYear);
        Invoke(manager, "ApplyPlantingClearance", b, ecology.EcologicalYear);
        Check(Mathf.Abs(sitka.Density - (1f - twice / (ecology.CellSizeMeters * ecology.CellSizeMeters))) < 0.004f,
            "overlapping treatment double-cleared Sitka or deleted the whole cell");
        sitka.Restore(1f, 0.6f, ecology.EcologicalYear);
        // Reset the treatment probe's patches; the next two are contractor work.
        var prePlant = manager.CaptureSaveData();
        prePlant.clearancePatches.Clear();
        manager.RestoreSaveData(prePlant);
        Check(manager.ApprovePendingWork() && manager.AdvanceYear(), "exact-position contractor planting failed");
        Check(manager.PlantedJuveniles.Count == 3 && manager.ClearancePatches.Count == 3
            && manager.PlantedJuveniles.All(j => j.alive && string.IsNullOrEmpty(j.promotedTreeId)),
            "individual juveniles or clearance patches were not recorded");
        ScenarioEcologicalSnapshot plantedSnapshot = manager.EcologicalSnapshots.Last();
        Check(plantedSnapshot.species.Single(s => s.speciesId == "sessile-oak").plantedJuveniles == 2
            && plantedSnapshot.species.Single(s => s.speciesId == "beech").plantedJuveniles == 1,
            "the annual snapshot did not expose living planted individuals");
        Check(manager.PlantedJuveniles.Any(j => j.speciesId == "beech" && j.position == c)
            && manager.PlantedJuveniles.Count(j => j.speciesId == "sessile-oak"
                && (j.position == a || j.position == b)) == 2,
            "individual planted positions or species changed");
        yield return null; // Unity destroys resolved marker visuals at frame end.
        foreach (PlantedJuvenile juvenile in manager.PlantedJuveniles)
        {
            GameObject display = GameObject.Find("Planted juvenile " + juvenile.juvenileId);
            Check(display != null && display.transform.position == juvenile.position,
                "Section 5 planted sapling was invisible or displaced: " + juvenile.juvenileId);
        }
        Check(FindObjectsByType<Transform>(FindObjectsSortMode.None)
            .All(t => !t.name.StartsWith("Planting Marker #", StringComparison.Ordinal)),
            "resolved markers remained in the stand");

        ForestSaveData planted = saves.CaptureData();
        manager.InitializeNewScenario();
        saves.LoadData(JsonUtility.FromJson<ForestSaveData>(JsonUtility.ToJson(planted)), false);
        yield return null;
        Check(manager.PlantedJuveniles.Count == 3 && manager.ClearancePatches.Count == 3
            && manager.PlantedJuveniles.Any(j => j.position == c), "individuals/patches lost on v13 reload");
        // Force a mature-enough fixture at adequate light; promotion itself
        // must use the individual's stored position and never a cell random offset.
        PlantedJuvenile oak = manager.PlantedJuveniles.First(j => j.speciesId == "sessile-oak");
        TreeSpeciesDefinition oakSpecies = FindFirstObjectByType<ForestTreeSpawner>().ResolveSpecies("sessile-oak");
        oak.heightMeters = oakSpecies.PromotionHeightM + 0.1f;
        ecology.Cells[oak.cellIndex].Light = 1f;
        Invoke(manager, "AdvancePlantedJuveniles");
        ForestTree promoted = FindObjectsByType<ForestTree>(FindObjectsSortMode.None)
            .SingleOrDefault(t => t.TreeId == "PL-" + oak.juvenileId);
        Check(promoted != null && promoted.transform.position == oak.position
            && oak.promotedTreeId == promoted.TreeId, "planted individual promoted at a random cell position");
        ForestSaveData promotedState = saves.CaptureData();
        saves.LoadData(JsonUtility.FromJson<ForestSaveData>(JsonUtility.ToJson(promotedState)), false);
        yield return null;
        Check(FindObjectsByType<ForestTree>(FindObjectsSortMode.None).Count(t => t.TreeId == promoted.TreeId) == 1
            && FindObjectsByType<ForestTree>(FindObjectsSortMode.None).Single(t => t.TreeId == promoted.TreeId)
                .transform.position == oak.position, "promoted individual duplicated or moved on reload");

        // The unchanged 80-year canonical Sitka fixture and fingerprint. The
        // expected value is the calibrated lifecycle (C8 + k10a10); it was
        // 7E39B70A14959FAD before that calibration.
        ForestStandScenarios.ApplyLifecycleFixture();
        for (int year = 0; year < 80; year++)
        {
            ecology.AdvanceOneYear();
            if (year % 10 == 9) yield return null;
        }
        string canonical = LifecycleHash(ecology);
        Check(canonical == "BFC55473C1506067", "canonical Sitka lifecycle changed: " + canonical);
        Debug.Log("SCENARIO_ONE_CANONICAL_SITKA_PASS hash=" + canonical);

        // The frozen Year-50 save is v12. It must still load and continue to
        // Year 100 under current code, keeping every Year-50 tree's identity
        // and position. Since the C8 + k10a10 calibration the continued
        // biology is deliberately no longer the frozen v1 biology, so the
        // Year-100 tree/cell comparison is reported, not required; the frozen
        // archive itself must still verify unchanged.
        ForestSaveData year50 = archive.AtYear(50).world;
        saves.LoadData(JsonUtility.FromJson<ForestSaveData>(JsonUtility.ToJson(year50)), false);
        yield return null;
        yield return null;
        Check(ecology.EcologicalYear == 50, "frozen v12 Year-50 save failed to load");
        var survey = new ScenarioReferenceSurvey
        {
            futureTreeIds = new HashSet<string>(archive.futureTreeIds, StringComparer.Ordinal),
            oakPlantedCells = new List<int>(archive.oakPlantingCells),
            beechPlantedCells = new List<int>(archive.beechPlantingCells)
        };
        ScenarioReferenceSchedule schedule = ScenarioReferenceRunner.BuildSchedule();
        for (int year = 50; year < 100; year++)
        {
            ScenarioReferenceRunner.ExecuteYear(manager, ecology, marks, schedule, survey, year);
            manager.ApprovePendingWork();
            Check(manager.AdvanceYear(), "historical replay stopped at year " + (year + 1));
            yield return null;
        }
        ForestSaveData replay = saves.CaptureData();
        ForestSaveData expected = archive.AtYear(100).world;
        Check(expected.version == 12 && replay.version == ForestSaveData.CurrentVersion
            && replay.ecologicalYear == 100, "frozen v12 Year-50 continuation did not reach a current-schema Year 100");
        // Stored v1 hashes, as before. Recomputing WorldHash re-serialises the
        // v12 world through the current (v14) save classes, so it is not a
        // valid archive-integrity check across schema versions.
        Check(archive.AtYear(100).worldHash == "7AD177B3CC2F73C7"
            && archive.AtYear(50).worldHash == "D5E75D6D21D631AC" && year50.version == 12,
            "frozen Reference Future v1 archive no longer verifies");
        var historicalSameYearPruneFell = new HashSet<string>(expected.scenarioOne.workOrders
            .Where(order => order.status == ScenarioWorkStatus.Completed
                && (order.type == ScenarioWorkType.PruneTree || order.type == ScenarioWorkType.FellTree))
            .GroupBy(order => new { order.targetTreeId, order.resolvedYear })
            .Where(group => group.Any(order => order.type == ScenarioWorkType.PruneTree)
                && group.Any(order => order.type == ScenarioWorkType.FellTree))
            .Select(group => group.Key.targetTreeId), StringComparer.Ordinal);
        Check(historicalSameYearPruneFell.SetEquals(new[] { "P0601" }),
            "the known v12 same-year pruning/felling exception changed");
        Dictionary<string, TreeSaveData> replayTrees = replay.trees.ToDictionary(tree => tree.treeId);
        foreach (TreeSaveData tree in year50.trees)
            Check(replayTrees.TryGetValue(tree.treeId, out TreeSaveData actual)
                && tree.speciesId == actual.speciesId && tree.position == actual.position,
                "Year-50 tree identity/position lost in continuation: " + tree.treeId);
        int sameTrees = 0, divergedTrees = 0, missingFrozen = 0;
        float maxDbhDiff = 0f;
        foreach (TreeSaveData tree in expected.trees)
        {
            if (!replayTrees.TryGetValue(tree.treeId, out TreeSaveData actual)) { missingFrozen++; continue; }
            bool same = tree.stage == actual.stage && tree.heightMeters == actual.heightMeters
                && tree.diameterCm == actual.diameterCm && tree.crownRadiusMeters == actual.crownRadiusMeters;
            if (same) sameTrees++; else divergedTrees++;
            maxDbhDiff = Mathf.Max(maxDbhDiff, Mathf.Abs(tree.diameterCm - actual.diameterCm));
        }
        Debug.Log("SCENARIO_ONE_LEGACY_REFERENCE_DIVERGENCE expected=calibrated frozenTrees=" + expected.trees.Count
            + " replayTrees=" + replay.trees.Count + " identical=" + sameTrees + " diverged=" + divergedTrees
            + " frozenIdsAbsent=" + missingFrozen + " maxDbhDiffCm=" + maxDbhDiff.ToString("0.00"));
        Debug.Log("SCENARIO_ONE_LEGACY_REFERENCE_REPLAY_PASS year50=v12 year100=v" + replay.version + " trees="
            + replay.trees.Count + " historicalSameYearPruneFell=P0601 frozenYear100="
            + archive.AtYear(100).worldHash + " biology=calibrated");
    }

    private static string LifecycleHash(ForestEcologyController ecology)
    {
        ulong hash = 14695981039346656037UL;
        void Bytes(byte[] values)
        {
            foreach (byte value in values) { hash ^= value; hash *= 1099511628211UL; }
        }
        void Text(string value) { Bytes(Encoding.UTF8.GetBytes(value ?? "")); Bytes(new byte[1]); }
        void Float(float value) => Bytes(BitConverter.GetBytes(value));
        List<ForestTree> trees = FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .OrderBy(tree => tree.TreeId ?? "", StringComparer.Ordinal).ToList();
        Text("TREES:" + trees.Count);
        foreach (ForestTree tree in trees)
        {
            Text(tree.TreeId ?? ""); Text(((int)tree.Stage).ToString()); Text(tree.AgeYears.ToString());
            Float(tree.Height); Float(tree.Diameter); Float(tree.CrownRadius);
            Float(tree.transform.position.x); Float(tree.transform.position.z);
        }
        Text("YEAR:" + ecology.EcologicalYear); Text("MAST:" + ecology.LastMastLabel);
        Float(ecology.LastMastMultiplier);
        if (ecology.Cells != null)
        {
            Text("CELLS:" + ecology.Cells.Length);
            foreach (ForestEcologyCell cell in ecology.Cells)
            {
                if (cell == null) { Float(-1f); continue; }
                Float(cell.Canopy); Float(cell.Light);
                TreeSpeciesDefinition species = ecology.ResolveSpecies();
                ForestRegenerationCohort cohort = species != null ? cell.FindCohort(species.SpeciesId) : null;
                Float(cohort != null ? cohort.SeedRain : 0f);
                Float(cohort != null ? cohort.Density : 0f);
                Float(cohort != null ? cohort.Height : 0f);
                Float(cohort != null ? cohort.EstablishYear : -1f);
                Float(cell.RecentOpening); Float(cell.EstablishmentSuitability);
            }
        }
        return hash.ToString("X16");
    }
}
#endif
