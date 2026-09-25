using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

// Disposable Scenario One progression runner. Move to Assets/ForestPrototype,
// run ScenarioOneProgressVerification.Begin, then move it back to Tools.
public static class ScenarioOneProgressVerification
{
#if UNITY_EDITOR
    public static void Begin()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        EditorApplication.isPlaying = true;
    }
#endif

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        new GameObject("Scenario One Progress Verification").AddComponent<ScenarioOneProgressVerificationRunner>();
    }
}

public sealed class ScenarioOneProgressVerificationRunner : MonoBehaviour
{
    private string savePath;
    private byte[] backup;

    private IEnumerator Start()
    {
        yield return null;
        Exception failure = null;
        IEnumerator checks = Verify();
        while (true)
        {
            bool more;
            object current = null;
            try { more = checks.MoveNext(); if (more) current = checks.Current; }
            catch (Exception error) { failure = error; break; }
            if (!more) break;
            yield return current;
        }
        if (failure == null) Debug.Log("SCENARIO_ONE_PROGRESS_VERIFY_PASS");
        else Debug.LogError("SCENARIO_ONE_PROGRESS_VERIFY_FAIL: " + failure);
        if (savePath != null)
        {
            if (backup != null) File.WriteAllBytes(savePath, backup);
            else if (File.Exists(savePath)) File.Delete(savePath);
        }
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(failure == null ? 0 : 1);
#endif
    }

    private IEnumerator Verify()
    {
        ScenarioOneManager manager = FindFirstObjectByType<ScenarioOneManager>();
        ForestEcologyController ecology = FindFirstObjectByType<ForestEcologyController>();
        ForestSaveController saves = FindFirstObjectByType<ForestSaveController>();
        ForestTreeSpawner spawner = FindFirstObjectByType<ForestTreeSpawner>();
        ForestTreeMarkingManager marking = FindFirstObjectByType<ForestTreeMarkingManager>();
        Check(manager != null && ecology != null && saves != null && spawner != null && marking != null,
            "scenario systems missing");
        savePath = Path.Combine(Application.persistentDataPath, "forest-save.json");
        if (File.Exists(savePath)) backup = File.ReadAllBytes(savePath);
        Check(manager.Outcome == ScenarioOneOutcome.Active && manager.CenturyReview == null
            && manager.Objectives.Count > 0 && manager.TutorialHint.StartsWith("1."),
            "fresh tutorial or objective state missing");
        Check(manager.GetComponent<ScenarioOneSoundscapePlayer>() != null
            && manager.SoundscapeState.layers.Count == 5,
            "habitat soundscape has no routing or audio backend");
        float baselineWoodpeckers = manager.SoundscapeState.layers.Single(layer =>
            layer.layerId == "woodpeckers").volume;
        ScenarioSoundscapeState gridProbe = ScenarioSoundscape.Compute(new ScenarioEcologicalSnapshot
        {
            cellCount = 4,
            occupiedRegenerationCells = 2,
            species = new System.Collections.Generic.List<ScenarioSpeciesOutcome>
            {
                new ScenarioSpeciesOutcome { speciesId = "sitka-spruce", livingTrees = 1 },
                new ScenarioSpeciesOutcome { speciesId = "beech" }
            }
        }, new System.Collections.Generic.List<ScenarioUnderstoreyCell>(),
            new System.Collections.Generic.List<ScenarioDeadwoodRecord>(), manager.Definition);
        Check(Mathf.Approximately(gridProbe.regenerationActivity, 0.5f)
            && gridProbe.structuralDiversity < 0.3f,
            "soundscape used fixed grid size or counted absent species as habitat");

        ForestTree target = FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude,
            FindObjectsSortMode.None).First(tree => tree.TreeId == "P0000");
        ForestPlayer player = FindFirstObjectByType<ForestPlayer>();
        Check(player != null, "player missing");
        typeof(ForestPlayer).GetMethod("TryHarvestInteraction", BindingFlags.NonPublic | BindingFlags.Instance)
            .Invoke(player, new object[] { target });
        Check(!target.IsStump && target.ChopProgress == 0,
            "Scenario One manual F input bypassed contractor felling");
        manager.PlanningFellingOutcome = FellingMaterialOutcome.RetainAsFallenDeadwood;
        marking.Mark(target, false);
        Check(manager.AddMarkedTreesToWorkPlan() == 1 && manager.TutorialHint.StartsWith("2."),
            "marking did not advance the tutorial");
        Check(manager.TryPurchaseStock("beech-sapling", 1)
            && manager.TryPurchaseStock("sessile-oak-sapling", 1)
            && manager.TutorialHint.StartsWith("3."), "purchasing did not advance the tutorial");

        int[] brightest = Enumerable.Range(0, ecology.CellCount)
            .OrderByDescending(index => ecology.Cells[index].Light).Take(3).ToArray();
        Check(manager.TryDesignatePlanting("beech-sapling", brightest[0])
            && manager.TryDesignatePlanting("sessile-oak-sapling", brightest[1])
            && manager.ApprovePendingWork() && manager.TutorialHint.StartsWith("4."),
            "work approval did not advance the tutorial");
        Check(manager.AdvanceYear() && ecology.EcologicalYear == 1
            && manager.Outcome == ScenarioOneOutcome.Active && manager.TutorialHint.StartsWith("5."),
            "first annual step ended the scenario or skipped tutorial review");
        Check(manager.DeadwoodRecords.Count == 1 && manager.DeadwoodRecords[0].remainingVolumeM3 > 0f,
            "retained deadwood missing from the conversion plan");
        Check(manager.SoundscapeState.layers.Single(layer => layer.layerId == "woodpeckers").volume
            > baselineWoodpeckers, "retained deadwood did not change habitat-driven sound routing");

        // The managed forest survives until the configurable review year. A
        // deterministic fixture isolates progression scoring from growth rates:
        // two planted broadleaf species and one Sitka cohort remain present.
        ecology.RestoreEcologyState(manager.Definition.MinimumCompletionYear - 1, ecology.SimulationSeed);
        int[] cells = Enumerable.Range(0, ecology.CellCount)
            .OrderByDescending(index => ecology.Cells[index].Light).Take(3).ToArray();
        Check(ecology.TryPlantJuvenile(spawner.ResolveSpecies("beech"), Center(ecology, cells[0])).Success
            && ecology.TryPlantJuvenile(spawner.ResolveSpecies("sessile-oak"), Center(ecology, cells[1])).Success,
            "broadleaf review fixture could not plant");
        ForestRegenerationCohort sitka = ecology.Cells[cells[2]].GetOrCreateCohort(spawner.DefaultSpecies);
        sitka.Restore(0.5f, 0.6f, ecology.EcologicalYear);
        Check(manager.AdvanceYear() && manager.Outcome == ScenarioOneOutcome.Completed
            && manager.OutcomeYear == manager.Definition.MinimumCompletionYear,
            "achievable management plan did not complete at the review year");
        Check(manager.Objectives.All(item => item.achieved)
            && manager.ManagementEvents.Count(item => item.eventType
                == ScenarioManagementEventType.ScenarioCompleted) == 1,
            "completion was not supported by every objective or emitted more than once");

        ecology.RestoreEcologyState(manager.Definition.CenturyReviewYear - 1, ecology.SimulationSeed);
        Check(manager.AdvanceYear() && manager.CenturyReview != null
            && manager.CenturyReview.year == manager.Definition.CenturyReviewYear
            && manager.CenturyReview.completedYear == manager.Definition.MinimumCompletionYear
            && manager.CenturyReview.referenceComparisons.Count >= 4,
            "the Century Review did not compare actual outcomes with reference targets");
        Check(!manager.AdvanceYear() && ecology.EcologicalYear == manager.Definition.CenturyReviewYear,
            "terminal century review allowed additional annual steps");

        ScenarioOneSaveData completed = manager.CaptureSaveData();
        saves.Save();
        manager.InitializeNewScenario();
        saves.Load();
        yield return null;
        Check(JsonUtility.ToJson(manager.CaptureSaveData()) == JsonUtility.ToJson(completed)
            && manager.Outcome == ScenarioOneOutcome.Completed
            && manager.SoundscapeState.year == ecology.EcologicalYear
            && manager.CenturyReview.referenceComparisons.Count == completed.centuryReview.referenceComparisons.Count,
            "v12 save/load changed completed objectives or the Century Review");

        manager.InitializeNewScenario();
        ecology.RestoreEcologyState(0, ecology.SimulationSeed);
        ScenarioOneSaveData bankrupt = manager.CaptureSaveData();
        bankrupt.cashCents = 0;
        manager.RestoreSaveData(bankrupt);
        Check(manager.AdvanceYear() && manager.Outcome == ScenarioOneOutcome.Failed
            && !manager.AdvanceYear(), "irrecoverable zero-cash scenario did not fail");
        ScenarioOneSaveData failed = manager.CaptureSaveData();
        saves.Save();
        manager.InitializeNewScenario();
        saves.Load();
        yield return null;
        Check(manager.Outcome == ScenarioOneOutcome.Failed
            && JsonUtility.ToJson(manager.CaptureSaveData()) == JsonUtility.ToJson(failed),
            "failed outcome or history did not survive save/load");
        Debug.Log($"SCENARIO_ONE_PROGRESS_DETAIL completedYear={completed.outcomeYear} "
            + $"century={completed.centuryReview.year} failureYear={manager.OutcomeYear}");
    }

    private static Vector3 Center(ForestEcologyController ecology, int index)
    {
        Vector2 value = ecology.Cells[index].Center;
        return new Vector3(value.x, 0f, value.y);
    }

    private static void Check(bool okay, string message)
    {
        if (!okay) throw new InvalidOperationException(message);
    }
}
