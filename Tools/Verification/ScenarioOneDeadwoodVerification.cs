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

// Disposable Scenario One deadwood verification. Copy into
// Assets/ForestPrototype, run ScenarioOneDeadwoodVerification.Begin in Editor
// batchmode, then remove the temporary Assets copy and generated .meta.
public static class ScenarioOneDeadwoodVerification
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
        new GameObject("Scenario One Deadwood Verification").AddComponent<ScenarioOneDeadwoodVerificationRunner>();
    }
}

public sealed class ScenarioOneDeadwoodVerificationRunner : MonoBehaviour
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
        if (failure == null) Debug.Log("SCENARIO_ONE_DEADWOOD_VERIFY_PASS");
        else Debug.LogError("SCENARIO_ONE_DEADWOOD_VERIFY_FAIL: " + failure);
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
        Check(manager != null && ecology != null && saves != null, "scenario systems missing");
        savePath = Path.Combine(Application.persistentDataPath, "forest-save.json");
        if (File.Exists(savePath)) backup = File.ReadAllBytes(savePath);

        // Baseline snapshot has no deadwood.
        Check(manager.EcologicalSnapshots.Count == 1 && manager.EcologicalSnapshots[0].deadwoodCount == 0
            && manager.EcologicalSnapshots[0].deadwoodVolumeM3 == 0f, "baseline snapshot has phantom deadwood");

        // ---- Retain-as-deadwood outcome --------------------------------
        ForestTree target = FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
            .First(tree => tree.TreeId == "P0000");
        float stemVolume = target.BiologicalStemVolumeM3;
        float stemHeight = target.Height;
        float stemDiameter = target.Diameter;
        int cellIndex = ecology.GetCellIndex(target.transform.position);
        ForestTreeMarkingManager marking = FindFirstObjectByType<ForestTreeMarkingManager>();
        marking.Mark(target, false);
        Check(manager.AddMarkedTreesToWorkPlan() == 1, "felling order not created");

        ScenarioOneWorkOrder order = manager.WorkOrders[0];
        Check(order.fellingOutcome == FellingMaterialOutcome.SellAndExtract,
            "definition default felling outcome changed");
        manager.PlanningFellingOutcome = FellingMaterialOutcome.RetainAsFallenDeadwood;
        Check(order.expectedRevenueCents > 0f, "pre-switch revenue should be positive");

        // Toggle through the public planning outcome by rewriting the order the
        // same way the Work Plan button does.
        order.fellingOutcome = FellingMaterialOutcome.RetainAsFallenDeadwood;
        order.expectedRevenueCents = 0L;
        Check(manager.ApprovePendingWork(), "deadwood-retention order approval failed");
        long cost = order.estimatedCostCents;
        long beforeCash = manager.CashCents;
        Check(manager.AdvanceYear() && ecology.EcologicalYear == 1, "deadwood year did not advance");

        Check(manager.DeadwoodRecords.Count == 1, "deadwood record was not created");
        ScenarioDeadwoodRecord record = manager.DeadwoodRecords[0];
        Check(record.deadwoodId == "DW0001" && record.treeId == "P0000" && record.speciesId == "sitka-spruce",
            "deadwood identity is wrong");
        Check(record.cellIndex == cellIndex && Mathf.Abs(record.originalVolumeM3 - stemVolume) < 1e-5f
            && Mathf.Abs(record.remainingVolumeM3 - stemVolume) < 1e-5f
            && Mathf.Abs(record.originalHeightMeters - stemHeight) < 1e-3f
            && Mathf.Abs(record.originalDiameterCm - stemDiameter) < 1e-3f
            && record.fallenYear == 1 && record.DecayClass == 0,
            "deadwood biology or provenance is wrong");
        Check(target.IsStump, "Forestry stump was not produced by retention felling");
        Check(GameObject.Find("Fallen Log " + record.deadwoodId) != null, "fallen log visual is missing");

        Check(manager.CashCents == beforeCash - cost,
            "retention felling paid revenue or charged the wrong contractor cost");
        Check(manager.AnnualReports[0].timberRevenueCents == 0
            && manager.AnnualReports[0].harvestedVolumeM3 == 0f
            && manager.AnnualReports[0].deadwoodCreated == 1
            && Mathf.Abs(manager.AnnualReports[0].deadwoodCreatedM3 - stemVolume) < 1e-5f
            && manager.AnnualReports[0].deadwoodDecayedM3 == 0f,
            "annual report mis-recorded retained deadwood");

        ScenarioManagementEvent resolved = manager.ManagementEvents.Single(entry =>
            entry.eventType == ScenarioManagementEventType.WorkResolved);
        Check(resolved.outcome == ScenarioManagementOutcome.Succeeded
            && resolved.ecologicalTreatment == ScenarioEcologicalTreatment.TreeRetainedAsDeadwood
            && resolved.targetTreeId == "P0000" && resolved.timberRevenueCents == 0
            && resolved.contractorCostCents == cost
            && Mathf.Abs(resolved.biologicalVolumeM3 - stemVolume) < 1e-5f,
            "structured history lost the deadwood treatment");

        ScenarioEcologicalSnapshot snapshot = manager.EcologicalSnapshots.Last();
        Check(snapshot.deadwoodCount == 1 && Mathf.Abs(snapshot.deadwoodVolumeM3 - stemVolume) < 1e-5f
            && snapshot.meanDeadwoodDecayClass == 0f && snapshot.deadwoodHabitatValue > 0f,
            "ecological snapshot lost the deadwood outcome");

        // ---- Deterministic decay across years -------------------------
        float volumeAfterYear1 = record.remainingVolumeM3;
        for (int year = 2; year <= 6; year++)
            Check(manager.AdvanceYear(), "decay year did not advance");
        Check(record.remainingVolumeM3 < volumeAfterYear1 && record.remainingVolumeM3 > 0f
            && record.lastDecayYear == 6 && record.DecayClass >= 1,
            "deadwood did not decay deterministically");
        Check(Mathf.Abs(manager.AnnualReports.Last().deadwoodDecayedM3) > 0f,
            "annual report did not record decay");

        // Same-year double decay must be a no-op.
        float stable = record.remainingVolumeM3;
        ScenarioDeadwood.Decay(record, 6);
        Check(Mathf.Approximately(record.remainingVolumeM3, stable), "same-year decay ran twice");

        // ---- Sell-and-extract still pays and creates no deadwood ------
        manager.PlanningFellingOutcome = FellingMaterialOutcome.SellAndExtract;
        ForestTree second = FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
            .First(tree => tree.TreeId == "P0001");
        float secondVolume = second.BiologicalStemVolumeM3;
        marking.Mark(second, false);
        Check(manager.AddMarkedTreesToWorkPlan() == 1, "second felling order not created");
        ScenarioOneWorkOrder sellOrder = manager.WorkOrders.Last(o => o.targetTreeId == "P0001");
        Check(sellOrder.fellingOutcome == FellingMaterialOutcome.SellAndExtract
            && sellOrder.expectedRevenueCents > 0,
            $"sell-and-extract order wrong: outcome={sellOrder.fellingOutcome} revenue={sellOrder.expectedRevenueCents} "
            + $"volume={sellOrder.expectedVolumeM3} species={sellOrder.speciesId} secondVolume={secondVolume}");
        Check(manager.ApprovePendingWork(), "sell order approval failed");
        long sellCost = sellOrder.estimatedCostCents;
        long beforeSell = manager.CashCents;
        Check(manager.AdvanceYear(), "sell year did not advance");
        long expectedRevenue = (long)Math.Round(secondVolume * manager.Definition.TimberValueCentsPerCubicMetre("sitka-spruce"),
            MidpointRounding.AwayFromZero);
        Check(manager.DeadwoodRecords.Count == 1, "extraction created phantom deadwood");
        Check(manager.CashCents == beforeSell - sellCost + expectedRevenue,
            "extraction settlement is wrong");
        Check(manager.AnnualReports.Last().harvestedVolumeM3 > 0f
            && manager.AnnualReports.Last().deadwoodCreated == 0,
            "extraction report mis-recorded");

        // ---- Save / load round trip ----------------------------------
        ScenarioOneSaveData before = manager.CaptureSaveData();
        saves.Save();
        manager.InitializeNewScenario();
        saves.Load();
        yield return null;
        Check(manager.DeadwoodRecords.Count == 1
            && JsonUtility.ToJson(manager.DeadwoodRecords[0]) == JsonUtility.ToJson(before.deadwoodRecords[0])
            && manager.DeadwoodRecords[0].deadwoodId == "DW0001"
            && manager.DeadwoodRecords[0].remainingVolumeM3 < manager.DeadwoodRecords[0].originalVolumeM3,
            "save/load changed deadwood records");
        Check(manager.SoundscapeState.year == ecology.EcologicalYear
            && manager.SoundscapeState.layers.Count == 5,
            "loaded soundscape was not rebuilt from the saved ecological state");
        Check(manager.transform.GetComponentsInChildren<Renderer>(true)
                .Count(renderer => renderer != null && renderer.name == "Fallen Log DW0001") == 1,
            "save/load did not rebuild exactly one retained-log visual");
        Check(JsonUtility.ToJson(manager.CaptureSaveData()) == JsonUtility.ToJson(before),
            "save/load changed the complete scenario state");

        // ---- Legacy migration ---------------------------------------
        ForestSaveData legacy = JsonUtility.FromJson<ForestSaveData>(File.ReadAllText(savePath));
        legacy.version = 9;
        File.WriteAllText(savePath, JsonUtility.ToJson(legacy));
        saves.Load();
        yield return null;
        Check(manager.DeadwoodRecords.Count == 0 && manager.ManagementEvents.Count == 0,
            "version-9 migration invented deadwood history");
        Check(manager.transform.GetComponentsInChildren<Renderer>(true)
                .All(renderer => renderer == null || !renderer.name.StartsWith("Fallen Log ", StringComparison.Ordinal)),
            "legacy migration left retained-log visuals in the scene");

        Debug.Log($"SCENARIO_ONE_DEADWOOD_DETAIL stem={stemVolume:0.0000} remaining={record.remainingVolumeM3:0.0000} "
            + $"decayClass={record.DecayClass} habitat={ScenarioDeadwood.HabitatValue(record):0.000} years=6");
    }

    private static void Check(bool okay, string message)
    {
        if (!okay) throw new InvalidOperationException(message);
    }
}
