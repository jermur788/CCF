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

        // In Scenario One, U plans contractor work for the previewed area; it must
        // not remove vegetation or charge cash immediately. Since the accepted
        // clearance correction (Docs/Scenario1ClearanceCorrection.md) the
        // walking control plans speciesless area clearance of the previewed
        // cell through the shared authoritative query (empty species field);
        // the species-selective API above remains for saved/legacy callers.
        typeof(ForestPlayer).GetField("aimedSurfacePoint", BindingFlags.Instance | BindingFlags.NonPublic)
            .SetValue(player, position);
        typeof(ForestPlayer).GetField("isAimingGround", BindingFlags.Instance | BindingFlags.NonPublic)
            .SetValue(player, true);
        typeof(ForestPlayer).GetMethod("RefreshAimedRegeneration", BindingFlags.Instance | BindingFlags.NonPublic)
            .Invoke(player, null);
        string selectedSpecies = (string)typeof(ForestPlayer).GetField("selectedRegenerationSpeciesId",
            BindingFlags.Instance | BindingFlags.NonPublic).GetValue(player);
        Check(!string.IsNullOrEmpty(selectedSpecies), "ground aim did not select a live cohort");
        // The walking ray normally fills the preview each frame; batch mode has
        // no cursor lock, so show the same authoritative cell query directly.
        ClearanceTargets aimedTargets = manager.QueryClearance(ClearanceFootprint.Cell(ecology, cellIndex));
        Check(aimedTargets.HasTargets && aimedTargets.Cohorts.Count > 0, "aimed cell has no clearance targets");
        player.Clearance.Show(aimedTargets);
        float beforeManualU = ecology.Cells[cellIndex].Regeneration.Where(c => c != null).Sum(c => c.Density);
        long beforeManualCash = manager.CashCents;
        int ordersBeforeManualU = manager.WorkOrders.Count;
        MethodInfo manualUproot = typeof(ForestPlayer).GetMethod("UpdateUprooting", BindingFlags.Instance | BindingFlags.NonPublic);
        manualUproot.Invoke(player, new object[] { true, 100f });
        Check(manager.WorkOrders.Count == ordersBeforeManualU + 1, "in-world U did not plan exactly one order: " + manager.Feedback);
        ScenarioOneWorkOrder aimedOrder = manager.WorkOrders.Last();
        Check(aimedOrder.type == ScenarioWorkType.RemoveRegeneration && aimedOrder.IsOpen
            && string.IsNullOrEmpty(aimedOrder.speciesId) && aimedOrder.cellIndex == cellIndex
            && manager.CashCents == beforeManualCash
            && ecology.Cells[cellIndex].Regeneration.Where(c => c != null).Sum(c => c.Density) == beforeManualU,
            "in-world U did not create a speciesless area-clearance order without clearing the cell");
        Check(manager.RemovePendingOrder(aimedOrder.workOrderId), "in-world clearance mark could not be cancelled");
        manualUproot.Invoke(player, new object[] { false, 0f });
        player.Clearance.Clear();

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
