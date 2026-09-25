using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

// Disposable Scenario One pruning verification. Copy into
// Assets/ForestPrototype, run ScenarioOnePruningVerification.Begin in Editor
// batchmode, then remove the temporary Assets copy and generated .meta.
public static class ScenarioOnePruningVerification
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
        new GameObject("Scenario One Pruning Verification").AddComponent<ScenarioOnePruningVerificationRunner>();
    }
}

public sealed class ScenarioOnePruningVerificationRunner : MonoBehaviour
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
        if (failure == null) Debug.Log("SCENARIO_ONE_PRUNING_VERIFY_PASS");
        else Debug.LogError("SCENARIO_ONE_PRUNING_VERIFY_FAIL: " + failure);
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

        ForestTree target = FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
            .First(tree => tree.TreeId == "P0000");
        Check(target.PruningLifts == 0 && target.CrownBaseHeightM == 0f, "fresh tree is already pruned");

        // Direct Forestry API checks.
        string rejection = target.TryPrune(0.5f, 0);
        Check(rejection == null, "valid pruning lift was rejected: " + rejection);
        Check(target.PruningLifts == 1 && Mathf.Abs(target.CrownBaseHeightM - 0.5f) < 1e-4f,
            "pruning lift state is wrong");
        Check(target.TryPrune(0.4f, 1) != null, "lower crown-base target was accepted");
        Check(target.TryPrune(0.6f, 1) != null, "recovery interval was not enforced");
        Check(target.TryPrune(target.Height * 0.9f, 10) != null, "over-height target was accepted");
        Check(target.TryPrune(1.2f, 10) == null, "second lift was rejected");
        Check(target.PruningLifts == 2, "second lift not recorded");
        Check(target.TryPrune(2.0f, 20) == null, "third lift was rejected");
        Check(target.TryPrune(2.5f, 30) != null, "fourth lift exceeded the cap");
        Check(target.PruningLifts == 3, "lift cap not enforced");

        // Work-order flow for a second tree.
        ForestTree orderTarget = FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
            .First(tree => tree.TreeId == "P0001");
        float crownBefore = orderTarget.CrownRadius;
        Check(manager.TryDesignatePruning("P0001"), "pruning designation failed");
        Check(!manager.TryDesignatePruning("P0001"), "duplicate pruning designation accepted");
        Check(manager.WorkOrders.Count == 1 && manager.WorkOrders[0].type == ScenarioWorkType.PruneTree
            && manager.WorkOrders[0].targetTreeId == "P0001"
            && Mathf.Abs(manager.WorkOrders[0].expectedRegenerationDensity
                - manager.Definition.NextPruningTargetHeightM(0)) < 1e-4f,
            "pruning order is wrong");
        Check(manager.ApprovePendingWork(), "pruning approval failed");
        long cost = manager.WorkOrders[0].estimatedCostCents;
        long beforeCash = manager.CashCents;
        Check(manager.AdvanceYear(), "pruning year did not advance");
        Check(orderTarget.PruningLifts == 1 && orderTarget.CrownBaseHeightM > 0f
            && orderTarget.CrownRadius < crownBefore,
            "work-order pruning did not change the tree");
        Check(manager.CashCents == beforeCash - cost, "pruning settlement is wrong");

        ScenarioManagementEvent resolved = manager.ManagementEvents.Single(entry =>
            entry.eventType == ScenarioManagementEventType.WorkResolved);
        Check(resolved.outcome == ScenarioManagementOutcome.Succeeded
            && resolved.ecologicalTreatment == ScenarioEcologicalTreatment.TreePruned
            && resolved.targetTreeId == "P0001" && resolved.contractorCostCents == cost
            && resolved.timberRevenueCents == 0,
            "structured history lost the pruning treatment");

        // Save / load round trip.
        ScenarioOneSaveData before = manager.CaptureSaveData();
        saves.Save();
        manager.InitializeNewScenario();
        saves.Load();
        yield return null;
        Check(orderTarget.PruningLifts == 1
            && Mathf.Abs(orderTarget.CrownBaseHeightM - before.workOrders[0].expectedRegenerationDensity) < 1e-4f,
            "save/load changed pruning history");
        Check(JsonUtility.ToJson(manager.CaptureSaveData()) == JsonUtility.ToJson(before),
            "save/load changed the complete scenario state");

        // Legacy migration loads as unpruned.
        ForestSaveData legacy = JsonUtility.FromJson<ForestSaveData>(File.ReadAllText(savePath));
        legacy.version = 9;
        File.WriteAllText(savePath, JsonUtility.ToJson(legacy));
        saves.Load();
        yield return null;
        Check(orderTarget.PruningLifts == 0 && orderTarget.CrownBaseHeightM == 0f,
            "version-9 migration kept pruning history");

        Debug.Log($"SCENARIO_ONE_PRUNING_DETAIL tree=P0001 lifts=1 crownBefore={crownBefore:0.000} "
            + $"crownAfter={orderTarget.CrownRadius:0.000} cost={cost}");
    }

    private static void Check(bool okay, string message)
    {
        if (!okay) throw new InvalidOperationException(message);
    }
}
