using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

// CCF #359 verification-only probe, session 3 (batch is enough): the save-v24 replacement for exactly the save03b
// (Tools/Verification/RackV2Save/RackV2SaveVerification.cs) assertions that no passing current-version gate covers. That historic gate is
// NOT edited, NOT re-pinned and keeps its FAIL. Everything here is in memory (CaptureData / LoadData / RestoreSaveData): no save file is read
// or written, no scene is saved, no year advances, no anchor is rebaselined.
//
//   F  hash compatibility at v24: an empty-plan world must hash exactly as an INDEPENDENTLY stripped pre-v21 byte layout. The strip below is
//      written here, not taken from production, and includes the v24 field firstGuidedRackReviewComplete. Provenance witness: the historic
//      save03b strip (which predates v24) must NOT match - that reproduces and explains the #358 diagnostic failure instead of hiding it.
//      Also: the guided-review latch never moves an identity; rack content does; the workflow flag alone does not.
//   G  the nine save03b semantic rejections with no v24 counterpart in RackV2Persistence (whose 21 disk cases cover the others), checked
//      against the CURRENT validator (ForestSaveValidation changed after save03b was last updated, parity-08 3eb9c954).
//   E3 RestoreSaveData(null) stays model 0.     J  V2 is never active on the Legacy40 stand even with model 1; such a save is refused.
public static class Ccf359V24SaveContract
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
        if (Environment.GetCommandLineArgs().Any(a => a == "Ccf359V24SaveContract.Begin"))
            new GameObject("CCF 359 v24 save contract").AddComponent<Ccf359V24SaveContractRunner>();
    }
}

public sealed class Ccf359V24SaveContractRunner : MonoBehaviour
{
    private ForestEcologyController ecology;
    private ScenarioOneManager manager;
    private ForestSaveController saves;
    private int passed;
    private readonly List<string> failures = new List<string>();

    private void Check(string name, bool ok, string detail = "")
    {
        if (ok) passed++; else failures.Add(name + " " + detail);
        Debug.Log($"C359SAVE_{(ok ? "PASS" : "FAIL")} {name} {detail}");
    }

    // save03b's own strip, verbatim (the historic comparator that predates v24). Used ONLY as the provenance witness.
    private static string HistoricStripV21(string json)
        => json.Replace(",\"rackPlanningModel\":0,\"rackPlanConfirmed\":false,\"nextRackId\":1,\"racks\":[]", "")
               .Replace(",\"rackPlanningModel\":1,\"rackPlanConfirmed\":false,\"nextRackId\":1,\"racks\":[]", "")
               .Replace(",\"rackId\":0", "")
               .Replace(",\"objectiveAchievements\":[]", "")
               .Replace(",\"nextPrescriptionId\":1,\"prescriptions\":[]", "")
               .Replace(",\"prescriptionId\":0,\"prescriptionCoverageUnitId\":\"\",\"prescriptionCropTreeId\":\"\",\"prescriptionReason\":0", "")
               .Replace(",\"prescriptionId\":0", "")
               .Replace("\"version\":" + ForestSaveData.CurrentVersion + ",", "\"version\":20,");

    // The v24-aware independent strip: the historic bytes PLUS the v24 teaching latch (default false), whose literal JSON text is written
    // out here. The header is relabelled only from the literal 24 this probe qualifies.
    private static string IndependentStripV24(string json)
        => HistoricStripV21(json.Replace("\"firstGuidedRackReviewComplete\":false,", "")).Replace("\"version\":24,", "\"version\":20,");

    private static RackSaveData Rack(int id, float ex, float ez, float fx, float fz, string[] removal, string entrance, bool marked)
        => new RackSaveData { rackId = id, entranceX = ex, entranceZ = ez, farX = fx, farZ = fz, removalTreeIds = new List<string>(removal), entranceTreeId = entrance, entranceMarked = marked };

    private static ScenarioOneWorkOrder Order(int id, string treeId, int rackId)
        => new ScenarioOneWorkOrder { workOrderId = id, type = ScenarioWorkType.FellTree, status = ScenarioWorkStatus.Pending, targetTreeId = treeId, createdYear = 0, rackId = rackId };

    private IEnumerator Start()
    {
        for (int i = 0; i < 5; i++) yield return null;
        Exception failure = null;
        var stack = new Stack<IEnumerator>(); stack.Push(Run());
        while (stack.Count > 0)
        {
            bool more; object current = null;
            try { more = stack.Peek().MoveNext(); if (more) current = stack.Peek().Current; }
            catch (Exception e) { failure = e; break; }
            if (!more) { stack.Pop(); continue; }
            if (current is IEnumerator nested) stack.Push(nested); else yield return current;
        }
        bool ok = failure == null && failures.Count == 0;
        Debug.Log(ok ? "CCF359_V24SAVE_PASS checks=" + passed : $"CCF359_V24SAVE_FAIL checks={passed} exception={failure} failures=[{string.Join(" | ", failures)}]");
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(ok ? 0 : 1);
#endif
    }

    private IEnumerator Run()
    {
        ecology = FindFirstObjectByType<ForestEcologyController>();
        manager = FindFirstObjectByType<ScenarioOneManager>();
        saves = FindFirstObjectByType<ForestSaveController>();
        if (ecology == null || manager == null || saves == null) throw new InvalidOperationException("not the Scenario One scene");
        Check("FRESH_ENLARGED80_V2_WORLD", ecology.StandGeometryModelVersion == StandGeometryModel.Enlarged80 && manager.RackPlanningV2Active && manager.PlannedRackCount == 0);

        // ---- the v24 schema this probe qualifies ----
        ForestSaveData fresh = saves.CaptureData();
        string freshJson = JsonUtility.ToJson(fresh);
        Check("CAPTURE_WRITES_THE_CURRENT_VERSION_WHICH_IS_THE_V24_THIS_PROBE_QUALIFIES", fresh.version == ForestSaveData.CurrentVersion && fresh.version == 24, "version=" + fresh.version);
        Check("V24_STATES_THE_TEACHING_LATCH_EXPLICITLY_AND_FALSE", freshJson.Contains("\"firstGuidedRackReviewComplete\":false,"));

        // ---- F: hash compatibility ----
        ForestSaveData hashWorld = saves.CaptureData();
        ScenarioReferenceArchive.Canonicalize(hashWorld);
        string raw = JsonUtility.ToJson(hashWorld);
        string v24Strip = IndependentStripV24(raw), historic = HistoricStripV21(raw);
        Check("INDEPENDENT_V24_STRIP_REMOVES_EVERY_POST_V20_FIELD", !v24Strip.Contains("rackPlanningModel") && !v24Strip.Contains("rackId") && !v24Strip.Contains("firstGuidedRackReviewComplete")
            && !v24Strip.Contains("objectiveAchievements") && !v24Strip.Contains("prescription") && v24Strip.Contains("\"version\":20,"));
        string production = ScenarioReferenceArchive.CurrentWorldHash(hashWorld);
        Check("EMPTY_PLAN_V24_WORLD_HASHES_EXACTLY_AS_ITS_INDEPENDENT_PRE_V21_LAYOUT", production == ScenarioReferenceArchive.Hash(v24Strip), $"production={production} independent={ScenarioReferenceArchive.Hash(v24Strip)}");
        Check("PROVENANCE_WITNESS_THE_HISTORIC_SAVE03B_STRIP_KEEPS_THE_V24_LATCH_AND_DOES_NOT_MATCH",
            historic.Contains("firstGuidedRackReviewComplete") && production != ScenarioReferenceArchive.Hash(historic), $"historic={ScenarioReferenceArchive.Hash(historic)}");
        // The latch never participates in an identity, even when true.
        ForestSaveData latched = JsonUtility.FromJson<ForestSaveData>(raw.Replace("\"firstGuidedRackReviewComplete\":false,", "\"firstGuidedRackReviewComplete\":true,"));
        Check("LATCH_TRUE_PARSES", latched.scenarioOne.firstGuidedRackReviewComplete);
        Check("THE_GUIDED_REVIEW_LATCH_NEVER_MOVES_A_WORLD_HASH", ScenarioReferenceArchive.CurrentWorldHash(latched) == production && ScenarioReferenceArchive.WorldHash(latched) == ScenarioReferenceArchive.WorldHash(hashWorld));
        string[] allTrees = fresh.trees.Select(t => t.treeId).ToArray();
        ForestSaveData withRack = JsonUtility.FromJson<ForestSaveData>(raw);
        withRack.scenarioOne.rackPlanningModel = RackPlanningModel.RackPlanningFieldMarkingV2;
        withRack.scenarioOne.nextRackId = 2;
        withRack.scenarioOne.racks = new List<RackSaveData> { Rack(1, -12f, -20f, 12f, -20f, new[] { allTrees[4] }, allTrees[4], false) };
        Check("RACK_CONTENT_GIVES_A_NEW_IDENTITY", ScenarioReferenceArchive.CurrentWorldHash(withRack) != production);
        ForestSaveData flagWorld = JsonUtility.FromJson<ForestSaveData>(raw);
        flagWorld.scenarioOne.rackPlanningModel = RackPlanningModel.Legacy;
        string asLegacy = ScenarioReferenceArchive.CurrentWorldHash(flagWorld);
        flagWorld.scenarioOne.rackPlanningModel = RackPlanningModel.RackPlanningFieldMarkingV2;
        Check("THE_WORKFLOW_FLAG_ALONE_DOES_NOT_MOVE_A_HASH", ScenarioReferenceArchive.CurrentWorldHash(flagWorld) == asLegacy);

        // ---- G: the nine uncovered semantic rejections, against the current validator ----
        ForestSaveData good = saves.CaptureData();
        good.scenarioOne.rackPlanningModel = RackPlanningModel.RackPlanningFieldMarkingV2;
        good.scenarioOne.nextRackId = 2;
        good.scenarioOne.racks = new List<RackSaveData> { Rack(1, -12f, -20f, 12f, -20f, new[] { allTrees[5] }, allTrees[5], false) };
        good.scenarioOne.workOrders = new List<ScenarioOneWorkOrder> { Order(1, allTrees[7], 0) };
        string baseError = ForestSaveValidation.ValidateRackV2(good);
        Check("POSITIVE_CONTROL_THE_BASE_SAVE_IS_VALID", baseError == null, baseError ?? "");
        void Rejected(string label, Action<ForestSaveData> mutate)
        {
            ForestSaveData bad = JsonUtility.FromJson<ForestSaveData>(JsonUtility.ToJson(good));
            mutate(bad);
            string error = ForestSaveValidation.ValidateRackV2(bad);
            Check("REJECTS_" + label, error != null, error ?? "accepted");
        }
        Rejected("NEXT_RACK_ID_ZERO", d => d.scenarioOne.nextRackId = 0);
        Rejected("DEGENERATE_RACK_GEOMETRY", d => { d.scenarioOne.racks[0].farX = d.scenarioOne.racks[0].entranceX; d.scenarioOne.racks[0].farZ = d.scenarioOne.racks[0].entranceZ; });
        Rejected("EMPTY_REMOVAL_TREE_ID", d => d.scenarioOne.racks[0].removalTreeIds.Add(""));
        Rejected("ONE_TREE_OWNED_BY_TWO_RACKS", d => { d.scenarioOne.nextRackId = 3; d.scenarioOne.racks.Add(Rack(2, -12f, 8f, 12f, 8f, new[] { allTrees[5] }, "", false)); });
        Rejected("CROP_TREE_INSIDE_A_REMOVAL_SET", d => { foreach (TreeSaveData t in d.trees) if (t.treeId == allTrees[5]) t.markType = (int)TreeMarkType.CropTree; });
        Rejected("MARKED_ENTRANCE_ALSO_CARRIES_A_FELL_MARK", d => { d.scenarioOne.racks[0].entranceMarked = true; foreach (TreeSaveData t in d.trees) if (t.treeId == allTrees[5]) t.markType = (int)TreeMarkType.Fell; });
        Rejected("OPEN_INDEPENDENT_ORDER_ON_A_RACK_TREE", d => d.scenarioOne.workOrders = new List<ScenarioOneWorkOrder> { Order(1, allTrees[5], 0) });
        Rejected("MODEL_0_WORLD_WITH_RACK_OWNED_ORDERS", d => { d.scenarioOne.rackPlanningModel = RackPlanningModel.Legacy; d.scenarioOne.racks = new List<RackSaveData>(); d.scenarioOne.nextRackId = 1; d.scenarioOne.workOrders = new List<ScenarioOneWorkOrder> { Order(1, allTrees[7], 1) }; });
        Rejected("MODEL_0_WORLD_CARRYING_V2_RACK_ID_STATE", d => { d.scenarioOne.rackPlanningModel = RackPlanningModel.Legacy; d.scenarioOne.racks = new List<RackSaveData>(); d.scenarioOne.workOrders = new List<ScenarioOneWorkOrder>(); });

        // ---- E3: the legacy initialization path ----
        Check("FRESH_WORLD_RELOADS", saves.LoadData(JsonUtility.FromJson<ForestSaveData>(freshJson), false));
        yield return null;
        manager.RestoreSaveData(null, 10);
        Check("RESTORE_SAVE_DATA_NULL_STAYS_MODEL_0", manager.RackPlanningModelVersion == RackPlanningModel.Legacy && manager.Racks.Count == 0 && !manager.RackPlanningV2Active);

        // ---- J: the V2 predicate is geometry-bound (destructive grid switch, so last) ----
        ecology.ApplyStandGeometry(StandGeometryModel.Legacy40);
        ScenarioOneSaveData claim = saves.CaptureData().scenarioOne;
        claim.rackPlanningModel = RackPlanningModel.RackPlanningFieldMarkingV2; claim.rackPlanConfirmed = false; claim.nextRackId = 1; claim.racks = new List<RackSaveData>();
        manager.RestoreSaveData(claim, ForestSaveData.CurrentVersion);
        Check("V2_NEVER_ACTIVE_ON_LEGACY40_EVEN_WITH_MODEL_1", !manager.RackPlanningV2Active);
        Check("A_MODEL_1_LEGACY40_SAVE_IS_REFUSED", ForestSaveValidation.ValidateRackV2(saves.CaptureData()) != null);
        manager.RestoreSaveData(null, ForestSaveData.CurrentVersion);
        Check("LEGACY40_RESET_IS_MODEL_0", manager.RackPlanningModelVersion == RackPlanningModel.Legacy && !manager.RackPlanningV2Active);
    }
}
