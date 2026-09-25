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

// Disposable Scenario One regeneration-control verification. Copy into
// Assets/ForestPrototype, run ScenarioOneRemovalVerification.Begin in Editor
// batchmode, then remove the temporary Assets copy and generated .meta.
public static class ScenarioOneRemovalVerification
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
        new GameObject("Scenario One Removal Verification").AddComponent<ScenarioOneRemovalVerificationRunner>();
    }
}

public sealed class ScenarioOneRemovalVerificationRunner : MonoBehaviour
{
    private string savePath;
    private byte[] backup;

    private IEnumerator Start()
    {
        yield return null;
        Exception failure = null;
        IEnumerator verify = Verify();
        while (true)
        {
            bool more;
            object current = null;
            try
            {
                more = verify.MoveNext();
                if (more) current = verify.Current;
            }
            catch (Exception error) { failure = error; break; }
            if (!more) break;
            yield return current;
        }
        if (failure == null) Debug.Log("SCENARIO_ONE_REMOVAL_VERIFY_PASS");
        else Debug.LogError("SCENARIO_ONE_REMOVAL_VERIFY_FAIL: " + failure);
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
        ForestTreeSpawner spawner = FindFirstObjectByType<ForestTreeSpawner>();
        ForestSaveController saves = FindFirstObjectByType<ForestSaveController>();
        ForestPlayer player = FindFirstObjectByType<ForestPlayer>();
        Check(manager != null && ecology != null && spawner != null && saves != null && player != null,
            "scenario systems missing");
        TreeSpeciesDefinition beech = spawner.ResolveSpecies("beech");
        TreeSpeciesDefinition oak = spawner.ResolveSpecies("sessile-oak");
        Check(beech != null && oak != null, "cohort species unavailable");
        savePath = Path.Combine(Application.persistentDataPath, "forest-save.json");
        if (File.Exists(savePath)) backup = File.ReadAllBytes(savePath);

        int cellIndex = Enumerable.Range(0, ecology.CellCount)
            .OrderByDescending(index => ecology.Cells[index].Light).First();
        Vector2 center = ecology.Cells[cellIndex].Center;
        Vector3 position = new Vector3(center.x, 0f, center.y);
        Check(ecology.TryPlantJuvenile(beech, position).Success && ecology.TryPlantJuvenile(oak, position).Success,
            "mixed-species regeneration setup failed");
        float removedDensity = ecology.Cells[cellIndex].FindCohort("beech").Density;
        float otherDensity = ecology.Cells[cellIndex].FindCohort("sessile-oak").Density;
        Check(removedDensity > 0f && otherDensity > 0f, "cohorts did not share the cell");
        Check(!manager.TryDesignateRegenerationRemoval("beech", -1)
            && !manager.TryDesignateRegenerationRemoval("unknown", cellIndex), "invalid removal designation accepted");
        Check(manager.TryDesignateRegenerationRemoval("beech", cellIndex), "Beech removal was not designated");
        Check(!manager.TryDesignateRegenerationRemoval("beech", cellIndex), "duplicate removal designation accepted");
        Check(manager.TryDesignateRegenerationRemoval("sessile-oak", cellIndex), "other species could not be designated");
        Check(manager.RemovePendingOrder(2), "pending removal could not be cancelled");
        Check(manager.ManagementEvents.Last().eventType == ScenarioManagementEventType.OrderCancelled,
            "pending removal cancellation missing from history");
        Check(manager.ApprovePendingWork(), "removal approval failed");
        long cost = manager.WorkOrders.Single().estimatedCostCents;
        long beforeCash = manager.CashCents;
        Check(manager.AdvanceYear() && ecology.EcologicalYear == 1, "regeneration removal blocked annual step");
        Check(manager.CashCents == beforeCash - cost && manager.Inventory.Count == 0,
            "removal settlement used stock or charged the wrong amount");
        Check(ecology.Cells[cellIndex].FindCohort("beech") == null
            && ecology.Cells[cellIndex].FindCohort("sessile-oak")?.Density > 0f,
            "species-selective treatment changed the wrong cohort");
        Check(manager.AnnualReports.Count == 1 && manager.AnnualReports[0].regenerationRemovalTasks == 1
            && Mathf.Abs(manager.AnnualReports[0].removedRegenerationDensity - removedDensity) < 1e-5f,
            "annual report lost the biological removal");
        Check(manager.EcologicalSnapshots.Count == 2 && manager.EcologicalSnapshots[0].year == 0
            && manager.EcologicalSnapshots[1].year == 1
            && manager.EcologicalSnapshots[1].species.Single(entry => entry.speciesId == "beech").regenerationCells == 0
            && manager.EcologicalSnapshots[1].species.Single(entry => entry.speciesId == "sessile-oak").regenerationCells > 0,
            "annual ecological snapshot lost the species-selective outcome");
        ScenarioManagementEvent removed = manager.ManagementEvents.Single(entry =>
            entry.eventType == ScenarioManagementEventType.WorkResolved);
        Check(removed.outcome == ScenarioManagementOutcome.Succeeded
            && removed.ecologicalTreatment == ScenarioEcologicalTreatment.RegenerationRemoved
            && removed.year == 1 && removed.speciesId == "beech" && removed.cellIndex == cellIndex
            && removed.stockUsed == 0 && removed.contractorCostCents == cost
            && Mathf.Abs(removed.regenerationDensityRemoved - removedDensity) < 1e-5f,
            "structured history lost the selective removal treatment");

        // The old player U interaction must not bypass the Scenario One contractor.
        MethodInfo manualUproot = typeof(ForestPlayer).GetMethod("UpdateUprooting", BindingFlags.Instance | BindingFlags.NonPublic);
        manualUproot.Invoke(player, new object[] { true, 100f });
        Check(((string)typeof(ForestPlayer).GetField("lastHarvestMessage", BindingFlags.Instance | BindingFlags.NonPublic)
            .GetValue(player)).Contains("Work Plan"), "manual U did not route to the Work Plan");
        manualUproot.Invoke(player, new object[] { false, 0f });

        // If the target disappears after approval, no labour is charged and no
        // biological treatment is claimed in the event stream.
        int failedCell = (cellIndex + 1) % ecology.CellCount;
        center = ecology.Cells[failedCell].Center;
        Vector3 failedPosition = new Vector3(center.x, 0f, center.y);
        Check(ecology.TryPlantJuvenile(beech, failedPosition).Success, "failure fixture could not plant");
        Check(manager.TryDesignateRegenerationRemoval("beech", failedCell) && manager.ApprovePendingWork(),
            "failure fixture could not approve work");
        long beforeFailure = manager.CashCents;
        Check(ecology.TryUprootRegeneration(failedPosition, beech).Success, "failure fixture did not clear cohort");
        Check(manager.AdvanceYear() && manager.CashCents == beforeFailure
            && manager.WorkOrders.Last().status == ScenarioWorkStatus.Failed,
            "missing cohort removal charged cash or blocked the year");
        ScenarioManagementEvent failed = manager.ManagementEvents.Last(entry =>
            entry.eventType == ScenarioManagementEventType.WorkResolved);
        Check(failed.outcome == ScenarioManagementOutcome.Failed
            && failed.ecologicalTreatment == ScenarioEcologicalTreatment.None
            && failed.regenerationDensityRemoved == 0f && failed.contractorCostCents == 0
            && !string.IsNullOrEmpty(failed.failureReason), "failed removal was recorded as a successful treatment");

        ScenarioOneSaveData beforeSave = manager.CaptureSaveData();
        saves.Save();
        manager.InitializeNewScenario();
        saves.Load();
        yield return null;
        Check(manager.WorkOrders.Count == 2 && manager.AnnualReports.Count == 2
            && manager.ManagementEvents.Count == beforeSave.managementEvents.Count
            && JsonUtility.ToJson(manager.CaptureSaveData()) == JsonUtility.ToJson(beforeSave),
            "v11 save/load changed removal work, reports or management history");
        Debug.Log($"SCENARIO_ONE_REMOVAL_DETAIL cell={cellIndex} density={removedDensity:0.000} cost={cost}");
    }

    private static void Check(bool okay, string message)
    {
        if (!okay) throw new InvalidOperationException(message);
    }
}
