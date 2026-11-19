using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CCF.Forestry.WorkEconomy;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

// Disposable Editor gate for the Compact30 E2 management loop. It is kept under Tools/Verification
// and must be copied into Assets/ForestPrototype/ for the run, then removed with its generated .meta
// (the documented disposable workflow). It writes no save and no scene.
//
// What it proves when it runs: the E1 visual shell binds the existing management systems; the 175
// roster identities and 36-cell ecology exist; the teaching gates and four G1 sites produce real
// Pending -> Approved -> Completed planting work; the annual step creates linked juveniles; CT-3
// blocks the next advance until the covering Annual Review is acknowledged; approved work cannot be
// demoted; the historical two-cycle completion is not awarded; and old geometry sizes are unchanged.
//
// It does NOT prove: player feel, rendered composition, performance, save/load (HELD), or owner
// acceptance. Editor/native execution is required and was NOT RUN by the source-only parent.

public static class Sen1CompactE2ReuseVerification
{
    public static void Begin()
    {
        EditorSceneManager.OpenScene("Assets/ForestPrototype/Scenes/ScenarioOneCompact30.unity");
        EditorApplication.isPlaying = true;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (SceneManager.GetActiveScene().name != Compact30Bootstrap.SceneName) return;
        if (UnityEngine.Object.FindFirstObjectByType<Sen1CompactE2ReuseRunner>() != null) return;
        new GameObject("Sen1 Compact30 E2 verification").AddComponent<Sen1CompactE2ReuseRunner>();
    }
}

public sealed class Sen1CompactE2ReuseRunner : MonoBehaviour
{
    private readonly List<string> log = new List<string>();
    private int checks;

    private void Check(bool condition, string message)
    {
        checks++;
        if (!condition) throw new Exception("CHECK FAILED: " + message);
        log.Add("PASS " + message);
    }

    private IEnumerator Start()
    {
        Exception failure = null;
        IEnumerator test = Test();
        while (true)
        {
            bool more = false;
            object current = null;
            try
            {
                more = test.MoveNext();
                if (more) current = test.Current;
            }
            catch (Exception exception)
            {
                failure = exception;
            }
            if (failure != null || !more) break;
            yield return current;
        }

        foreach (string line in log) Debug.Log("COMPACT30_E2 " + line);
        if (failure == null) Debug.Log("COMPACT30_E2_VERIFY_PASS checks=" + checks);
        else Debug.LogError("COMPACT30_E2_VERIFY_FAIL " + failure);
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(failure == null ? 0 : 1);
    }

    private IEnumerator Test()
    {
        yield return null;
        yield return null;
        Compact30Session session = UnityEngine.Object.FindFirstObjectByType<Compact30Session>();
        ScenarioOneManager manager = UnityEngine.Object.FindFirstObjectByType<ScenarioOneManager>();
        ForestEcologyController ecology = UnityEngine.Object.FindFirstObjectByType<ForestEcologyController>();
        Check(session != null, "Compact30Session bound in the scene");
        Check(manager != null, "ScenarioOneManager bound");
        Check(ecology != null, "ForestEcologyController bound");
        Check(session.DataProblem.Length == 0, "candidate fixtures accepted: " + session.DataProblem);
        Check(ecology.StandGeometryModelVersion == StandGeometryModel.Compact30, "Compact30 geometry active");
        Check(ecology.CellCount == 36 && ecology.CellsPerAxis == 6, "36 ecology cells on a 6x6 grid");

        TextAsset rosterAsset = Resources.Load<TextAsset>("Compact30Roster");
        Compact30RosterFile roster = JsonUtility.FromJson<Compact30RosterFile>(rosterAsset.text);
        Check(Compact30Rules.RosterIsValid(roster, out string rosterProblem), "roster valid: " + rosterProblem);
        ForestTree[] trees = UnityEngine.Object.FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        Check(trees.Length == 175, "175 living trees bound from the E1 shell");
        Check(trees.Select(t => t.TreeId).Distinct().Count() == 175, "175 distinct source ids");
        Check(!trees.Any(t => t.IsCropTree || t.IsMarkedForFell), "no implicit crop or fell marks at start");
        Check(!manager.RackPlanningV2Active && manager.RackObjectCount == 0, "no rack plan in Compact");
        Check(!StandGeometryModel.IsKnown(StandGeometryModel.Compact30), "Compact30 is not admitted as a save geometry");
        Check(StandGeometryModel.StandSizeMeters(StandGeometryModel.Legacy40) == 40f
            && StandGeometryModel.StandSizeMeters(StandGeometryModel.Enlarged80) == 80f, "old geometry sizes unchanged");
        Check(UnityEngine.Object.FindFirstObjectByType<ForestSaveController>() == null, "no save controller / no save writes");

        // Three player-selected living Sitka Crop Trees are real marks but never a progression gate.
        ForestTreeMarkingManager marking = UnityEngine.Object.FindFirstObjectByType<ForestTreeMarkingManager>();
        Check(marking != null, "tree marking manager bound");
        ForestTree[] cropCandidates = trees.Where(t => t.Species != null && t.Species.SpeciesId == "sitka-spruce").Take(3).ToArray();
        foreach (ForestTree tree in cropCandidates) marking.Mark(tree, TreeMarkType.CropTree);
        Check(manager.CropTreeCount == 3, "three living Sitka Crop Trees selected");
        Check(!session.TeachingComplete, "crop trees alone do not award teaching progress");

        // Teaching gates: designation is refused before both acknowledgements.
        session.TryDesignate(0);
        Check(manager.WorkOrders.Count == 0, "designation refused before arrival/assessment");
        session.AcknowledgeArrival();
        Check(session.ArrivalAcknowledged, "H1 arrival acknowledged");
        session.AcknowledgeAssessment();
        Check(!session.AssessmentAcknowledged, "assessment refused without opening the live assessment");
        session.TryDesignate(0);
        Check(manager.WorkOrders.Count == 0, "designation refused before assessment acknowledgement");

        // Open the live assessment, then perform the same explicit acknowledgement the Y key performs.
        session.OpenAssessment();
        session.AcknowledgeAssessment();
        Check(session.AssessmentAcknowledged, "current stand assessment acknowledged after it was opened");
        Check(manager.TryPurchaseStock("sessile-oak-sapling", 2), "bought two sessile oak saplings");
        Check(manager.TryPurchaseStock("beech-sapling", 2), "bought two European beech saplings");
        Check(manager.GetStockQuantity("sessile-oak-sapling") == 2 && manager.GetStockQuantity("beech-sapling") == 2, "stock is owned");

        int beforeFree = manager.WorkOrders.Count;
        Check(!manager.TryDesignateExactPlanting("sessile-oak-sapling", new Vector3(5f, 0f, 5f),
            WorkExecutionMethod.Contractor, false), "free exact planting refused in Compact");
        Check(manager.WorkOrders.Count == beforeFree, "free exact planting created no order");

        for (int i = 0; i < 4; i++) session.TryDesignate(i);
        Check(manager.WorkOrders.Count == 4, "four distinct Pending G1 orders");
        Check(manager.WorkOrders.All(o => o.status == ScenarioWorkStatus.Pending && o.type == ScenarioWorkType.PlantJuvenile && o.exactPosition),
            "all four are exact Pending PlantJuvenile orders");
        Check(manager.PlantedJuveniles.Count == 0, "no juvenile exists before resolution");
        int firstOrderId = manager.WorkOrders.OrderBy(o => o.workOrderId).First().workOrderId;
        session.TryDesignate(0);
        Check(manager.WorkOrders.Count == 4, "pending re-designation does not grow the four-slot group");
        Check(!manager.WorkOrders.Any(o => o.workOrderId == firstOrderId), "pending re-designation creates a fresh order id");

        Check(manager.ApprovePendingWork(), "Work Plan approval succeeds");
        Check(manager.WorkOrders.All(o => o.status == ScenarioWorkStatus.Approved), "all four are Approved");
        WorkExecutionMethod method = manager.PlanningPlantingMethod;
        bool shelter = manager.PlanningInstallShelter;
        manager.SetPlantingExecution(method == WorkExecutionMethod.Contractor ? WorkExecutionMethod.LandownerSimulated : WorkExecutionMethod.Contractor);
        manager.SetPlantingShelters(!shelter);
        Check(manager.WorkOrders.All(o => o.status == ScenarioWorkStatus.Approved), "approved orders were not demoted by bulk setters");
        Check(manager.PlanningPlantingMethod == method && manager.PlanningInstallShelter == shelter, "bulk setters refused while approved work exists");

        Check(manager.AdvanceYear(), "year 1 annual advance succeeds");
        yield return null;
        Check(manager.CurrentEcologicalYear == 1, "ecological year is 1");
        Check(session.GroupIsComplete(), "four successful linked 2 Oak + 2 Beech rows");
        Check(session.CoveringYear == 1, "covering year is the actual resolution year");
        Check(session.Group.All(g => g.status == ScenarioWorkStatus.Completed && g.juvenileId.Length > 0), "each order links to a real juvenile");
        Check(manager.PlantedJuveniles.Count == 4 && manager.PlantedJuveniles.All(j => !string.IsNullOrEmpty(j.juvenileId)),
            "four real planted juveniles exist");
        Check(!session.TeachingComplete, "T1 is not complete before the covering review");

        Check(!manager.AdvanceYear(), "CT-3 blocks the next annual advance");
        Check(manager.CurrentEcologicalYear == 1, "blocked advance did not change the year");

        manager.MarkAnnualReviewSeen();
        Check(session.ReviewAcknowledgedYear == 1, "covering Annual Review acknowledgement recorded");
        Check(session.TeachingComplete, "T1 teaching complete after the covering review");
        Check(manager.AdvanceYear(), "annual advance released after the covering review");
        Check(manager.CurrentEcologicalYear == 2, "advanced to year 2");
        Check(manager.Outcome != ScenarioOneOutcome.Completed, "historical two-cycle completion not awarded");
        Check(session.LastNoWorkYear == -1, "no no-work decision was fabricated");
    }
}
