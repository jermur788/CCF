#if UNITY_EDITOR
using System;
using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Runs only when explicitly requested in Editor batchmode.
public static class ScenarioHabitatPresentationVerification
{
    private const string Requested = "ScenarioHabitatPresentationVerification.Requested";

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
        new GameObject("Habitat presentation verification").AddComponent<ScenarioHabitatPresentationGate>();
    }
}

public sealed class ScenarioHabitatPresentationGate : MonoBehaviour
{
    private struct Sample
    {
        public int year, moss, fern, bracken, grass, bramble, shrub, herbs, fungi, logs;
        public float conifer, mixed, gap;
    }

    private static void Check(bool result, string message)
    {
        if (!result) throw new InvalidOperationException(message);
    }

    private IEnumerator Start()
    {
        yield return null;
        Exception failure = null;
        IEnumerator verify = Verify();
        while (true)
        {
            bool more;
            object current = null;
            try { more = verify.MoveNext(); if (more) current = verify.Current; }
            catch (Exception error) { failure = error; break; }
            if (!more) break;
            yield return current;
        }
        if (failure == null) Debug.Log("SCENARIO_HABITAT_PRESENTATION_VERIFY_PASS");
        else Debug.LogError("SCENARIO_HABITAT_PRESENTATION_VERIFY_FAIL: " + failure);
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(failure == null ? 0 : 1);
    }

    private IEnumerator Verify()
    {
        ScenarioOneManager scenario = FindFirstObjectByType<ScenarioOneManager>();
        ForestEcologyController ecology = FindFirstObjectByType<ForestEcologyController>();
        ForestSaveController saves = FindFirstObjectByType<ForestSaveController>();
        ScenarioHabitatVisuals visuals = scenario != null ? scenario.GetComponent<ScenarioHabitatVisuals>() : null;
        ScenarioOneSoundscapePlayer audio = scenario != null ? scenario.GetComponent<ScenarioOneSoundscapePlayer>() : null;
        ScenarioReferenceArchive archive = ScenarioReferenceArchive.Load();
        Check(scenario != null && ecology != null && saves != null && visuals != null && audio != null
            && archive != null && archive.Matches(scenario.Definition, ecology),
            "habitat layer or frozen reference resource missing");
        string ownWorldHash = ScenarioReferenceArchive.WorldHash(saves.CaptureData());
        string ownVisualSignature = MainStandVisualSignature();
        VerifyMainTreeVariants();
        Sample opening = ReadSample(0, scenario, visuals, audio);
        // Grass follows the understorey proxy, which is zero below 0.40 light.
        // Under the calibrated canopy/light (k10a10) the scene's road and work
        // clearing cells exceed 0.40 at Year 0, so grass there is expected;
        // grass anywhere else, or with no bright cell, is not.
        int brightCells = ecology.Cells.Count(cell => cell.Light > 0.40f);
        Debug.Log($"HABITAT_YEAR0_GRASS patches={visuals.GrassPatchCount} cellsAbove0.40Light={brightCells}");
        Check(RecentAssetVisualCatalog.Load()?.grasses?.Length == 3 && (brightCells > 0 || visuals.GrassPatchCount == 0),
            "Grass catalog missing or grass appeared despite the Year-0 grass-cover proxy");
        Check(SectionFiveVisualCatalog.Load() != null && visuals.LitterPatchCount == 0,
            "Section 5 catalog missing or broadleaf litter appeared in the pure-Sitka start");
        Check(opening.moss > 0 && opening.conifer > opening.mixed && opening.fungi == 0
            && visuals.OldWoodlandSourceConfidence == 0f,
            "Year 0 is not a sparse, shaded, recognisably coniferous floor");
        foreach (int year in new[] { 20, 50, 100 })
        {
            Check(scenario.TryBeginReferencePreview(year), "historical preview unavailable at Year " + year);
            yield return null;
            Sample sample = ReadSample(year, scenario, visuals, audio);
            ScenarioReferenceMilestone milestone = archive.AtYear(year);
            Check(milestone != null && milestone.verifiedFrozenWorld,
                "frozen v12 full-world hash became invalid at Year " + year);
            if (year == 20)
                Check(sample.fern + sample.bracken + sample.grass > opening.fern + opening.bracken + opening.grass
                    && sample.conifer > sample.mixed,
                    "Year 20 lost its conifer identity or local gap response");
            if (year == 50)
                Check(sample.logs == 37 && sample.fungi > 0
                    && sample.mixed > opening.mixed,
                    "Year 50 lacks visible decaying wood or mixed habitat cues");
            if (year == 100)
            {
                Check(visuals.GrassPatchCount > 0, "Authored grass did not appear in century gap habitat");
                Check(visuals.LitterPatchCount > 0 && visuals.SmallDeadwoodPatchCount > 0,
                    "Section 5 litter/deadwood did not respond to the century forest");
                Check(sample.logs == 70 && sample.conifer > 0.2f
                    && sample.mixed > opening.mixed && sample.fern > 0,
                    "Year 100 lost retained Sitka, mixed sounds or ground-layer patches");
                string interpretation = ScenarioHabitatInterpretation.Describe(
                    scenario.EcologicalSnapshots.First(), scenario.EcologicalSnapshots.Last(),
                    visuals.OldWoodlandSourceConfidence);
                Check(scenario.CenturyReview != null && interpretation.Contains("162 Sitka")
                    && interpretation.Contains("unverified"),
                    "Century Review implied ancient woodland or erased the retained Sitka");
                ScenarioDeadwoodRecord oldest = scenario.DeadwoodRecords
                    .OrderByDescending(record => record.DecayClass).First();
                ScenarioDeadwoodRecord newest = scenario.DeadwoodRecords
                    .OrderBy(record => record.DecayClass).First();
                var oldTint = new MaterialPropertyBlock();
                var freshTint = new MaterialPropertyBlock();
                scenario.transform.Find("Fallen Log " + oldest.deadwoodId)
                    .GetComponentInChildren<Renderer>().GetPropertyBlock(oldTint);
                scenario.transform.Find("Fallen Log " + newest.deadwoodId)
                    .GetComponentInChildren<Renderer>().GetPropertyBlock(freshTint);
                Check(oldest.DecayClass > newest.DecayClass
                    && oldTint.GetColor("_BaseColor") != freshTint.GetColor("_BaseColor"),
                    "visible log colour did not change with recorded decay");
            }
            scenario.EndReferencePreview();
            yield return null;
            Check(ecology.EcologicalYear == 0
                && ScenarioReferenceArchive.WorldHash(saves.CaptureData()) == ownWorldHash,
                "viewing derived flora/fauna changed the player's saved world");
            Check(visuals.LitterPatchCount == 0, "returning to Year 0 retained future broadleaf litter");
            Check(MainStandVisualSignature() == ownVisualSignature,
                "Reference return/save loading changed the player's tree-model assignments");
        }
        Check(archive.AtYear(100).worldHash == "7AD177B3CC2F73C7"
            && ScenarioHabitatPalette.SpecialistHerbSignal(
                new ScenarioUnderstoreyCell { forbs = 1f }, visuals.OldWoodlandSourceConfidence) == 0f,
            "the frozen reference changed or unsourced old-woodland herbs appeared");
    }

    private static Sample ReadSample(int year, ScenarioOneManager scenario,
        ScenarioHabitatVisuals visuals, ScenarioOneSoundscapePlayer audio)
    {
        RecentAssetVisualCatalog catalog = RecentAssetVisualCatalog.Load();
        Check(catalog != null && visuals.RushPatchCount == catalog.rushDressingPositions.Length * 2,
            "Rush accents were not placed in the playable stand");
        Transform[] activeDisplays = FindObjectsByType<Transform>(FindObjectsSortMode.None);
        Check(catalog.rushes.All(prefab => prefab != null && activeDisplays.Any(display =>
            display.name.StartsWith(prefab.name + "(Clone)", StringComparison.Ordinal))),
            "One of the rush assets remains gallery-only");
        Check(scenario.EcologicalSnapshots.Last().year == year && scenario.SoundscapeState.year == year,
            "presentation was not rebuilt for the displayed year " + year);
        Check(scenario.SoundscapeState.layers.Count == 7
            && audio.AudibleLayerCount == audio.BoundLayerCount
            && !audio.GetComponents<AudioSource>().Any(source => source.clip != null
                && source.clip.name.StartsWith("Habitat cue ", StringComparison.Ordinal)),
            "unbound layers produced sound or an old procedural clip survived");
        ScenarioSoundscapeLayer Layer(string name) => scenario.SoundscapeState.layers.Single(layer => layer.layerId == name);
        var sample = new Sample
        {
            year = year, logs = scenario.DeadwoodRecords.Count,
            moss = visuals.VisibleVertexCount(HabitatVisualClass.MossCarpet),
            fern = visuals.VisibleVertexCount(HabitatVisualClass.ShadeFern),
            bracken = visuals.VisibleVertexCount(HabitatVisualClass.BrackenType),
            grass = visuals.VisibleVertexCount(HabitatVisualClass.Grass),
            bramble = visuals.VisibleVertexCount(HabitatVisualClass.BrambleType),
            shrub = visuals.VisibleVertexCount(HabitatVisualClass.DwarfShrubType),
            herbs = visuals.VisibleVertexCount(HabitatVisualClass.GenericHerbs),
            fungi = visuals.VisibleVertexCount(HabitatVisualClass.DeadwoodFungi),
            conifer = Layer("conifer-birds").volume,
            mixed = Layer("mixed-woodland-birds").volume,
            gap = Layer("gap-edge-birds").volume
        };
        ForestTree[] living = FindObjectsByType<ForestTree>(FindObjectsSortMode.None).Where(tree => !tree.IsStump).ToArray();
        int bent = living.Count(tree => catalog.bentStages.Any(prefab => prefab.name == tree.ActiveVisualSource));
        int cavity = living.Count(tree => catalog.cavityStages.Any(prefab => prefab.name == tree.ActiveVisualSource));
        Check(bent > 0 && cavity > 0, "Recent tree variants are absent from the playable stand");
        Debug.Log($"HABITAT_YEAR_{year} moss={sample.moss} shadeFern={sample.fern} "
            + $"brackenType={sample.bracken} grass={sample.grass} brambleType={sample.bramble} "
            + $"dwarfShrubType={sample.shrub} herbs={sample.herbs} logFungi={sample.fungi} "
            + $"logs={sample.logs} conifer={sample.conifer:0.00} mixed={sample.mixed:0.00} "
            + $"gap={sample.gap:0.00} litterPatches={visuals.LitterPatchCount} smallWood={visuals.SmallDeadwoodPatchCount} grassPatches={visuals.GrassPatchCount} rushClumps={visuals.RushPatchCount} bentTrees={bent} cavityTrees={cavity}");
        return sample;
    }

    private static void VerifyMainTreeVariants()
    {
        RecentAssetVisualCatalog catalog = RecentAssetVisualCatalog.Load();
        Check(catalog != null, "Recent main-game assets missing");
        ForestTree[] trees = FindObjectsByType<ForestTree>(FindObjectsSortMode.None);
        foreach (GameObject[] family in new[] { catalog.bentStages, catalog.cavityStages })
        {
            Check(family != null && family.Length == 3, "Incomplete main tree family");
            ForestTree tree = trees.FirstOrDefault(t => !t.IsStump && family.Any(prefab => prefab != null
                && prefab.name == t.ActiveVisualSource));
            Check(tree != null, "An imported family exists only in the review scene");
            int age = tree.AgeYears;
            float height = tree.SimulationHeightMeters, dbh = tree.Diameter, crown = tree.CrownRadius;
            string initialSource = tree.ActiveVisualSource;
            for (int stage = 0; stage < 3; stage++)
            {
                // Disposable physical-size fixture verifies every delivered
                // stage can be selected on a normal ForestTree, not a gallery
                // object. Restore before any annual biology or hash check.
                tree.SetSimulationState(age, new[] { 11f, 18f, 28f }[stage],
                    new[] { 18f, 26f, 35f }[stage], crown);
                Check(tree.ActiveVisualSource == family[stage].name, "Main tree stage selection failed");
            }
            tree.SetSimulationState(age, height, dbh, crown);
            tree.SetMark(TreeMarkType.CropTree);
            Check(!family.Any(prefab => prefab.name == tree.ActiveVisualSource),
                "Crop Tree retained a visual with no pruning coverage");
            tree.SetMark(TreeMarkType.None);
            Check(tree.ActiveVisualSource == initialSource && tree.Height == height && tree.Diameter == dbh,
                "Cosmetic designation changed tree biology or failed to restore its look");
        }
        Debug.Log("MAIN_TREE_VARIANTS_PASS families=Bent,Cavity stages=Young,Mature,Older cropTreePruningFamily=True");
    }

    private static string MainStandVisualSignature()
    {
        return string.Join("|", FindObjectsByType<ForestTree>(FindObjectsSortMode.None)
            .Where(tree => !tree.IsStump).OrderBy(tree => tree.TreeId, StringComparer.Ordinal)
            .Select(tree => tree.TreeId + ":" + tree.ActiveVisualSource));
    }
}
#endif
