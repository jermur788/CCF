using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

// Disposable save-v21 gate (RACK-V2-03B): the Rack Planning & Field Marking V2 persistence foundation.
// Copy into Assets/ForestPrototype, run RackV2SaveVerification.Begin (batch is enough), then remove the
// copy and its .meta (the gate runner does this). It never writes the player's save file and never
// saves the scene: every round trip goes through in-memory CaptureData/LoadData/RestoreSaveData.
// It asserts v21 schema behaviour, the load/initialization policy, malformed-save rejection and the
// world-hash compatibility path. Historical anchors themselves are asserted by the existing gates
// (StandGeometry, Enlarged80, ScenarioReference, Completion); nothing here rebaselines them.
public static class RackV2SaveVerification
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
        if (Environment.GetCommandLineArgs().Any(a => a == "RackV2SaveVerification.Begin"))
            new GameObject("Disposable rack v2 save verification").AddComponent<RackV2SaveVerificationRunner>();
    }
}

public sealed class RackV2SaveVerificationRunner : MonoBehaviour
{
    private ForestEcologyController ecology;
    private ScenarioOneManager manager;
    private ForestSaveController saves;
    private readonly List<string> log = new List<string>();

    private static void Check(bool ok, string message) { if (!ok) throw new InvalidOperationException(message); }
    private void Pass(string message) { log.Add(message); Debug.Log("RACKV2SAVE_CHECK " + message); }

    // The literal bytes save v21 adds while a world's V2 rack plan is the empty default, rebuilt here
    // independently of production code so a wrong production strip cannot pass by agreeing with itself.
    private static string StripV21(string json)
        => json.Replace(",\"rackPlanningModel\":0,\"rackPlanConfirmed\":false,\"nextRackId\":1,\"racks\":[]", "")
               .Replace(",\"rackPlanningModel\":1,\"rackPlanConfirmed\":false,\"nextRackId\":1,\"racks\":[]", "")
               .Replace(",\"rackId\":0", "");

    private static RackSaveData Rack(int id, float ex, float ez, float fx, float fz, string[] removal, string entrance, bool marked)
        => new RackSaveData
        {
            rackId = id,
            entranceX = ex, entranceZ = ez, farX = fx, farZ = fz,
            removalTreeIds = new List<string>(removal),
            entranceTreeId = entrance,
            entranceMarked = marked
        };

    private static ScenarioOneWorkOrder Order(int id, string treeId, int rackId)
        => new ScenarioOneWorkOrder
        {
            workOrderId = id,
            type = ScenarioWorkType.FellTree,
            status = ScenarioWorkStatus.Pending,
            targetTreeId = treeId,
            createdYear = 0,
            rackId = rackId
        };

    private IEnumerator Start()
    {
        for (int i = 0; i < 5; i++) yield return null;
        Exception failure = null;
        var stack = new Stack<IEnumerator>();
        stack.Push(Run());
        while (stack.Count > 0)
        {
            bool more; object current = null;
            try { more = stack.Peek().MoveNext(); if (more) current = stack.Peek().Current; }
            catch (Exception error) { failure = error; break; }
            if (!more) { stack.Pop(); continue; }
            if (current is IEnumerator nested) { stack.Push(nested); continue; }
            yield return current;
        }
        Debug.Log(failure == null ? "RACKV2SAVE_VERIFY_PASS checks=" + log.Count : "RACKV2SAVE_VERIFY_FAIL: " + failure);
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(failure == null ? 0 : 1);
#endif
    }

    private IEnumerator Run()
    {
        ecology = FindFirstObjectByType<ForestEcologyController>();
        manager = FindFirstObjectByType<ScenarioOneManager>();
        saves = FindFirstObjectByType<ForestSaveController>();
        Check(ecology != null && manager != null && saves != null, "not the Scenario One ForestTest scene");

        // ---- A. Fresh-game identity and the v21 new-save round trip -----------------------------------
        // The scene just ran the real new-game path (Awake): an Enlarged80 game is a V2 world from its
        // first frame - model 1, zero racks, unconfirmed, nextRackId 1 - and RackPlanningV2Active agrees.
        Check(StandGeometryPolicy.NewGameModelForSession == StandGeometryModel.Enlarged80,
            "this gate runs with the production Enlarged80 new-game policy");
        Check(ecology.StandGeometryModelVersion == StandGeometryModel.Enlarged80, "fresh world is Enlarged80");
        Check(manager.RackPlanningModelVersion == RackPlanningModel.RackPlanningFieldMarkingV2, "a fresh Enlarged80 game is a V2 world");
        Check(!manager.RackPlanConfirmed && manager.NextRackId == 1 && manager.Racks.Count == 0, "a fresh V2 world starts with an empty draft plan");
        Check(manager.RackPlanningV2Active, "V2 is active on a fresh Enlarged80 model-1 world");
        Pass("fresh Enlarged80 game identifies as V2 (model 1, empty draft plan, V2 active)");

        ForestSaveData fresh = saves.CaptureData();
        Check(fresh.version == ForestSaveData.CurrentVersion && fresh.version == 21, "capture writes save v21");
        Check(fresh.scenarioOne != null && fresh.scenarioOne.rackPlanningModel == RackPlanningModel.RackPlanningFieldMarkingV2
            && !fresh.scenarioOne.rackPlanConfirmed && fresh.scenarioOne.nextRackId == 1
            && fresh.scenarioOne.racks != null && fresh.scenarioOne.racks.Count == 0, "the v21 save states the empty V2 plan explicitly");
        string freshJson = JsonUtility.ToJson(fresh);
        ForestSaveData freshParsed = JsonUtility.FromJson<ForestSaveData>(freshJson);
        Check(ForestSaveValidation.ValidateRackV2Json(freshJson, freshParsed) == null, "a genuine v21 save passes the explicit-presence check");
        Check(ForestSaveValidation.ValidateRackV2(freshParsed) == null, "a fresh v21 save passes rack validation");
        Check(freshParsed.scenarioOne.rackPlanningModel == fresh.scenarioOne.rackPlanningModel
            && freshParsed.scenarioOne.racks.Count == 0, "v21 new-save round trip preserves the plan fields");
        Check(saves.LoadData(JsonUtility.FromJson<ForestSaveData>(freshJson), false), "the v21 new save loads");
        yield return null;
        Check(manager.RackPlanningModelVersion == RackPlanningModel.RackPlanningFieldMarkingV2, "the round trip keeps the V2 identity");
        Check(ScenarioReferenceArchive.WorldHash(saves.CaptureData()) == ScenarioReferenceArchive.WorldHash(fresh),
            "the v21 new-save round trip is hash-exact");
        Pass("v21 new-save round trip exact (identity, empty plan, world hash)");

        // ---- B. Model-1 draft rack data round trip ----------------------------------------------------
        // The accepted draft state (packet RACK-V2-03B): 0-5 rack geometry records, unconfirmed, and no
        // entrance identity or mark yet is valid; entrance identity, when present, sits in the frozen set.
        string[] allTrees = fresh.trees.Select(t => t.treeId).ToArray();
        Check(allTrees.Length > 10, "the live world has trees to plan against");
        ForestSaveData draft = saves.CaptureData();
        draft.scenarioOne.rackPlanningModel = RackPlanningModel.RackPlanningFieldMarkingV2;
        draft.scenarioOne.nextRackId = 3;
        draft.scenarioOne.racks = new List<RackSaveData>
        {
            Rack(1, -12f, -20f, 12f, -20f, new[] { allTrees[0], allTrees[1] }, allTrees[0], false),
            Rack(2, -12f, 0f, 12f, 0.5f, new string[0], "", false)   // draft with no frozen removal set yet
        };
        string draftJson = JsonUtility.ToJson(draft);
        ForestSaveData draftParsed = JsonUtility.FromJson<ForestSaveData>(draftJson);
        Check(ForestSaveValidation.ValidateRackV2Json(draftJson, draftParsed) == null, "a draft rack save passes the explicit-presence check");
        Check(ForestSaveValidation.ValidateRackV2(draftParsed) == null, "an unconfirmed 2-rack draft validates");
        Check(saves.LoadData(JsonUtility.FromJson<ForestSaveData>(draftJson), false), "the draft rack save loads");
        yield return null;
        Check(manager.Racks.Count == 2 && manager.Racks[0].rackId == 1 && manager.Racks[1].rackId == 2
            && manager.Racks[0].removalTreeIds.SequenceEqual(new[] { allTrees[0], allTrees[1] })
            && manager.Racks[0].entranceTreeId == allTrees[0] && !manager.Racks[0].entranceMarked
            && manager.Racks[1].removalTreeIds.Count == 0 && manager.Racks[1].entranceTreeId == "",
            "draft rack records survive the load exactly");
        Check(manager.RackPlanningModelVersion == RackPlanningModel.RackPlanningFieldMarkingV2 && !manager.RackPlanConfirmed
            && manager.NextRackId == 3, "draft plan fields survive the load");
        ForestSaveData draftAgain = saves.CaptureData();
        Check(draftAgain.scenarioOne.racks.Count == 2
            && draftAgain.scenarioOne.racks[0].entranceX == -12f && draftAgain.scenarioOne.racks[0].farZ == -20f
            && draftAgain.scenarioOne.racks[1].farZ == 0.5f, "draft endpoints survive a capture round trip");
        Pass("model-1 draft rack data round trip exact (2 records, no removal set and a frozen set)");

        // ---- C. Positive rackId work-order round trip --------------------------------------------------
        ForestSaveData linked = saves.CaptureData();
        linked.scenarioOne.rackPlanningModel = RackPlanningModel.RackPlanningFieldMarkingV2;
        linked.scenarioOne.nextRackId = 2;
        linked.scenarioOne.racks = new List<RackSaveData> { Rack(1, -12f, -20f, 12f, -20f, new[] { allTrees[2] }, allTrees[2], true) };
        linked.scenarioOne.workOrders = new List<ScenarioOneWorkOrder> { Order(1, allTrees[2], 1) };
        // The orange entrance mark is exclusive: the entrance tree must not also carry Fell or Crop Tree.
        foreach (TreeSaveData tree in linked.trees)
            if (tree.treeId == allTrees[2])
                tree.markType = (int)TreeMarkType.None;
        string linkedJson = JsonUtility.ToJson(linked);
        Check(ForestSaveValidation.ValidateRackV2(JsonUtility.FromJson<ForestSaveData>(linkedJson)) == null,
            "a rack-owned work order validates");
        Check(saves.LoadData(JsonUtility.FromJson<ForestSaveData>(linkedJson), false), "the rack-owned order save loads");
        yield return null;
        Check(manager.WorkOrders.Count == 1 && manager.WorkOrders[0].rackId == 1 && manager.WorkOrders[0].targetTreeId == allTrees[2],
            "a positive rackId survives the load");
        Check(manager.Racks.Count == 1 && manager.Racks[0].entranceMarked, "the entrance mark survives the load");
        Check(saves.CaptureData().scenarioOne.workOrders[0].rackId == 1, "a positive rackId survives a capture round trip");
        Pass("positive rackId work-order round trip exact");

        // ---- D. Confirmed-plan invariant ---------------------------------------------------------------
        ForestSaveData confirmed = saves.CaptureData();
        confirmed.scenarioOne.rackPlanningModel = RackPlanningModel.RackPlanningFieldMarkingV2;
        confirmed.scenarioOne.rackPlanConfirmed = true;
        confirmed.scenarioOne.nextRackId = 6;
        confirmed.scenarioOne.racks = new List<RackSaveData>();
        confirmed.scenarioOne.workOrders = new List<ScenarioOneWorkOrder>();
        for (int i = 0; i < 5; i++)
            confirmed.scenarioOne.racks.Add(Rack(i + 1, -20f, -30f + 15f * i, 20f, -30f + 15f * i,
                new[] { allTrees[i * 2], allTrees[i * 2 + 1] }, allTrees[i * 2], i == 0));
        foreach (TreeSaveData tree in confirmed.trees)
            if (tree.treeId == confirmed.scenarioOne.racks[0].entranceTreeId)
                tree.markType = (int)TreeMarkType.None;
        Check(ForestSaveValidation.ValidateRackV2(confirmed) == null, "a confirmed five-rack plan validates");
        Check(saves.LoadData(JsonUtility.FromJson<ForestSaveData>(JsonUtility.ToJson(confirmed)), false), "the confirmed plan save loads");
        yield return null;
        Check(manager.RackPlanConfirmed && manager.Racks.Count == 5, "the confirmed five-rack plan survives the load");
        Pass("confirmed plan: exactly five records with frozen removal sets and entrance identities");

        // ---- E. Compatibility: pre-v21 saves stay model 0 -----------------------------------------------
        // The v20 layout is rebuilt from the v21 capture by removing exactly the new bytes and the header
        // bump - the same transformation a genuine pre-V2 file represents. It must load as model 0 with
        // empty V2 state, and the re-save must keep the world hash.
        ForestSaveData preV2Base = saves.CaptureData();
        preV2Base.scenarioOne.rackPlanningModel = RackPlanningModel.Legacy;
        preV2Base.scenarioOne.rackPlanConfirmed = false;
        preV2Base.scenarioOne.nextRackId = 1;
        preV2Base.scenarioOne.racks = new List<RackSaveData>();
        preV2Base.scenarioOne.workOrders = new List<ScenarioOneWorkOrder>();
        string v20Json = StripV21(JsonUtility.ToJson(preV2Base)).Replace("\"version\":21", "\"version\":20");
        Check(!v20Json.Contains("rackPlanningModel") && !v20Json.Contains("rackId"), "the v20 layout carries no rack bytes");
        ForestSaveData v20Parsed = JsonUtility.FromJson<ForestSaveData>(v20Json);
        Check(v20Parsed.version == 20 && v20Parsed.scenarioOne.rackPlanningModel == RackPlanningModel.Legacy,
            "a v20 save parses as the legacy model");
        Check(ForestSaveValidation.ValidateRackV2Json(v20Json, v20Parsed) == null, "a v20 save needs no v21 fields");
        Check(ForestSaveValidation.ValidateRackV2(v20Parsed) == null, "a v20 save with no V2 claim validates");
        Check(saves.LoadData(JsonUtility.FromJson<ForestSaveData>(v20Json), false), "a v20 Enlarged80 save loads");
        yield return null;
        Check(manager.RackPlanningModelVersion == RackPlanningModel.Legacy && !manager.RackPlanConfirmed
            && manager.Racks.Count == 0, "a v20 Enlarged80 save loads model 0 with an empty plan");
        Check(manager.NextRackId == 1, "pre-v21 loads normalize the id allocator to 1");
        Check(!manager.RackPlanningV2Active, "a pre-V2 Enlarged80 continuation is not a V2 world");
        ForestSaveData resaved = saves.CaptureData();
        Check(resaved.version == 21 && resaved.scenarioOne.rackPlanningModel == RackPlanningModel.Legacy, "old save re-saved as v21 remains model 0");
        Check(ScenarioReferenceArchive.WorldHash(resaved) == ScenarioReferenceArchive.WorldHash(preV2Base),
            "old save re-saved as v21 keeps its world hash");
        Pass("v20 Enlarged80 loads model 0; re-save as v21 stays model 0 and hash-stable");

        // Every pre-v21 order is effectively rackId 0. The legacy JSON has no rack link at all.
        ForestSaveData legacyOrder = saves.CaptureData();
        legacyOrder.scenarioOne.workOrders = new List<ScenarioOneWorkOrder> { Order(1, allTrees[3], 0) };
        string legacyOrderJson = StripV21(JsonUtility.ToJson(legacyOrder)).Replace("\"version\":21", "\"version\":20");
        Check(!legacyOrderJson.Contains("rackId"), "a v20 work order carries no rack link");
        Check(saves.LoadData(JsonUtility.FromJson<ForestSaveData>(legacyOrderJson), false), "a v20 save with a legacy order loads");
        yield return null;
        Check(manager.WorkOrders.Count == 1 && manager.WorkOrders[0].rackId == 0 && manager.WorkOrders[0].targetTreeId == allTrees[3],
            "every pre-v21 order is effectively rackId 0");
        Pass("pre-v21 work orders load at rackId 0");

        // Legacy loads never activate V2: the legacy initialization path lands on model 0.
        manager.RestoreSaveData(null, 10);
        Check(manager.RackPlanningModelVersion == RackPlanningModel.Legacy && manager.Racks.Count == 0 && !manager.RackPlanningV2Active,
            "RestoreSaveData(null) does not activate V2");
        Pass("legacy initialization path (RestoreSaveData(null)) stays model 0");

        // ---- F. Hash compatibility path ---------------------------------------------------------------
        // While the plan is empty, the production hash must equal the independently rebuilt pre-v21 layout
        // (geometry-aware), and real rack content must change it: new V2 worlds get new identities.
        ForestSaveData hashWorld = saves.CaptureData();
        ScenarioReferenceArchive.Canonicalize(hashWorld);
        string raw = JsonUtility.ToJson(hashWorld);
        string stripped = StripV21(raw);
        Check(stripped.Length < raw.Length && !stripped.Contains("rackPlanningModel") && !stripped.Contains("rackId"),
            "the independent strip removes exactly the v21 bytes");
        Check(ScenarioReferenceArchive.CurrentWorldHash(hashWorld) == ScenarioReferenceArchive.Hash(stripped),
            "an empty-plan world hashes exactly as its pre-v21 layout");
        ForestSaveData withRack = JsonUtility.FromJson<ForestSaveData>(raw);
        withRack.scenarioOne.rackPlanningModel = RackPlanningModel.RackPlanningFieldMarkingV2;
        withRack.scenarioOne.nextRackId = 2;
        withRack.scenarioOne.racks = new List<RackSaveData> { Rack(1, -12f, -20f, 12f, -20f, new[] { allTrees[4] }, allTrees[4], false) };
        string withRackHash = ScenarioReferenceArchive.CurrentWorldHash(withRack);
        Check(withRackHash != ScenarioReferenceArchive.Hash(stripped) && withRackHash != ScenarioReferenceArchive.CurrentWorldHash(hashWorld),
            "a world that carries rack state gets a new V2 identity");
        Pass("hash compatibility: empty plans keep the pre-v21 identity; rack content gets a new one");

        // The workflow flag alone never moves a world hash while the plan is empty: the protected
        // fresh-game anchors were recorded before v21 and must survive the new identity flag. Rack
        // CONTENT is what gives a V2 world its new identity (checked just above).
        ForestSaveData flagWorld = saves.CaptureData();
        ScenarioReferenceArchive.Canonicalize(flagWorld);
        flagWorld.scenarioOne.rackPlanningModel = RackPlanningModel.Legacy;
        string asLegacy = ScenarioReferenceArchive.CurrentWorldHash(flagWorld);
        flagWorld.scenarioOne.rackPlanningModel = RackPlanningModel.RackPlanningFieldMarkingV2;
        Check(ScenarioReferenceArchive.CurrentWorldHash(flagWorld) == asLegacy,
            "the workflow flag alone does not change a world hash while the plan is empty");
        Pass("rackPlanningModel alone never moves a world hash (empty plan)");

        // ---- G. Validation negatives --------------------------------------------------------------------
        ForestSaveData good = saves.CaptureData();
        good.scenarioOne.rackPlanningModel = RackPlanningModel.RackPlanningFieldMarkingV2;
        good.scenarioOne.nextRackId = 2;
        good.scenarioOne.racks = new List<RackSaveData> { Rack(1, -12f, -20f, 12f, -20f, new[] { allTrees[5] }, allTrees[5], false) };
        good.scenarioOne.workOrders = new List<ScenarioOneWorkOrder> { Order(1, allTrees[7], 0) };
        Check(ForestSaveValidation.ValidateRackV2(good) == null, "the negative-case base save is valid");

        void ExpectRejected(string label, Action<ForestSaveData> mutate)
        {
            ForestSaveData bad = JsonUtility.FromJson<ForestSaveData>(JsonUtility.ToJson(good));
            mutate(bad);
            Check(ForestSaveValidation.ValidateRackV2(bad) != null, "expected rejection missing for " + label);
            log.Add("rejected: " + label);
        }
        ExpectRejected("unknown rackPlanningModel", d => d.scenarioOne.rackPlanningModel = 2);
        ExpectRejected("duplicate rack ids", d => d.scenarioOne.racks.Add(Rack(1, -12f, 5f, 12f, 5f, new string[0], "", false)));
        ExpectRejected("invalid nextRackId (zero)", d => d.scenarioOne.nextRackId = 0);
        ExpectRejected("invalid nextRackId (at or below used)", d => d.scenarioOne.nextRackId = 1);
        ExpectRejected("non-finite endpoints", d => d.scenarioOne.racks[0].farX = float.NaN);
        ExpectRejected("degenerate rack geometry", d =>
        {
            d.scenarioOne.racks[0].farX = d.scenarioOne.racks[0].entranceX;
            d.scenarioOne.racks[0].farZ = d.scenarioOne.racks[0].entranceZ;
        });
        ExpectRejected("unknown tree id", d => d.scenarioOne.racks[0].removalTreeIds.Add("NO-SUCH-TREE"));
        ExpectRejected("empty tree id", d => d.scenarioOne.racks[0].removalTreeIds.Add(""));
        ExpectRejected("duplicate tree ownership between racks", d =>
        {
            d.scenarioOne.nextRackId = 3;
            d.scenarioOne.racks.Add(Rack(2, -12f, 8f, 12f, 8f, new[] { allTrees[5] }, "", false));
        });
        ExpectRejected("entrance tree outside the frozen removal set", d => d.scenarioOne.racks[0].entranceTreeId = allTrees[6]);
        ExpectRejected("entranceMarked without identity", d =>
        {
            d.scenarioOne.racks[0].entranceMarked = true;
            d.scenarioOne.racks[0].entranceTreeId = "";
        });
        ExpectRejected("Crop Tree inside a removal set", d =>
        {
            foreach (TreeSaveData tree in d.trees) if (tree.treeId == allTrees[5]) tree.markType = (int)TreeMarkType.CropTree;
        });
        ExpectRejected("marked entrance already carries a Fell mark", d =>
        {
            d.scenarioOne.racks[0].entranceMarked = true;
            foreach (TreeSaveData tree in d.trees) if (tree.treeId == allTrees[5]) tree.markType = (int)TreeMarkType.Fell;
        });
        ExpectRejected("confirmed plan without five racks", d => d.scenarioOne.rackPlanConfirmed = true);
        ExpectRejected("positive rackId with no matching rack", d => d.scenarioOne.workOrders = new List<ScenarioOneWorkOrder> { Order(1, allTrees[7], 5) });
        ExpectRejected("open independent order stealing a rack tree", d =>
            d.scenarioOne.workOrders = new List<ScenarioOneWorkOrder> { Order(1, allTrees[5], 0) });
        ExpectRejected("model-0 world with V2 rack records", d =>
        {
            d.scenarioOne.rackPlanningModel = RackPlanningModel.Legacy;
            d.scenarioOne.nextRackId = 1;
        });
        ExpectRejected("model-0 world with rack-owned orders", d =>
        {
            d.scenarioOne.rackPlanningModel = RackPlanningModel.Legacy;
            d.scenarioOne.racks = new List<RackSaveData>();
            d.scenarioOne.nextRackId = 1;
            d.scenarioOne.workOrders = new List<ScenarioOneWorkOrder> { Order(1, allTrees[7], 1) };
        });
        ExpectRejected("model-0 world carrying V2 rack id state", d =>
        {
            d.scenarioOne.rackPlanningModel = RackPlanningModel.Legacy;
            d.scenarioOne.racks = new List<RackSaveData>();
            d.scenarioOne.workOrders = new List<ScenarioOneWorkOrder>();
        });
        ExpectRejected("V2 on the Legacy40 stand", d => d.standGeometryModel = StandGeometryModel.Legacy40);
        ExpectRejected("pre-v21 save claiming a rack planning model", d =>
        {
            d.version = 20;
            d.scenarioOne.rackPlanningModel = 1;
        });

        // JSON-level presence checks: JsonUtility's silent zeros must not stand in for missing fields.
        string goodJson = JsonUtility.ToJson(good);
        JObject root = JObject.Parse(goodJson);
        ((JObject)root["scenarioOne"]).Property("nextRackId").Remove();
        Check(ForestSaveValidation.ValidateRackV2Json(root.ToString(Newtonsoft.Json.Formatting.None), good) != null,
            "a v21 save missing nextRackId is refused");
        root = JObject.Parse(goodJson);
        ((JObject)((JArray)((JObject)root["scenarioOne"])["racks"])[0]).Property("entranceMarked").Remove();
        Check(ForestSaveValidation.ValidateRackV2Json(root.ToString(Newtonsoft.Json.Formatting.None), good) != null,
            "a v21 rack record missing entranceMarked is refused");
        root = JObject.Parse(goodJson);
        ((JObject)((JArray)((JObject)root["scenarioOne"])["workOrders"])[0]).Property("rackId").Remove();
        Check(ForestSaveValidation.ValidateRackV2Json(root.ToString(Newtonsoft.Json.Formatting.None), good) != null,
            "a v21 work order missing rackId is refused");
        Pass("malformed rack data rejected: 19 semantic cases + 3 missing-field cases");

        // ---- H. Rejection happens before any world mutation ---------------------------------------------
        string before = ScenarioReferenceArchive.WorldHash(saves.CaptureData());
        ForestSaveData malformed = JsonUtility.FromJson<ForestSaveData>(JsonUtility.ToJson(good));
        malformed.scenarioOne.racks[0].farX = float.NaN;
        Check(!saves.LoadData(malformed, false), "LoadData refuses malformed rack data");
        Check(ScenarioReferenceArchive.WorldHash(saves.CaptureData()) == before, "a refused save leaves the world untouched");
        Check(manager.RackPlanningModelVersion == RackPlanningModel.Legacy && manager.Racks.Count == 0,
            "a refused save leaves the live plan as it was");
        Pass("malformed save rejected before any world mutation");

        // ---- I. Reference Future v1 is unaffected ---------------------------------------------------------
        ScenarioReferenceArchive archive = ScenarioReferenceArchive.Load();
        Check(archive != null && archive.milestones.Count == 4, "the frozen Reference Future v1 archive still loads");
        Check(archive.milestones.All(m => m.verifiedFrozenWorld), "every frozen milestone still matches its embedded world hash");
        Check(archive.AtYear(100).worldHash == "7AD177B3CC2F73C7", "the frozen Year-100 world hash is untouched");
        Pass("Reference Future v1 archive hash untouched (7AD177B3CC2F73C7) and still verified");

        // ---- J. The V2 predicate is geometry- and identity-bound -------------------------------------------
        // Last phase: switching the live grid is destructive, so it runs once, at the end.
        ecology.ApplyStandGeometry(StandGeometryModel.Legacy40);
        ScenarioOneSaveData claim = saves.CaptureData().scenarioOne;
        claim.rackPlanningModel = RackPlanningModel.RackPlanningFieldMarkingV2;
        claim.rackPlanConfirmed = false;
        claim.nextRackId = 1;
        claim.racks = new List<RackSaveData>();
        manager.RestoreSaveData(claim, 21);
        Check(!manager.RackPlanningV2Active, "V2 is never active on the Legacy40 stand, even with model 1 set");
        ForestSaveData legacyClaim = saves.CaptureData();
        Check(ForestSaveValidation.ValidateRackV2(legacyClaim) != null, "a model-1 Legacy40 save is refused");
        manager.RestoreSaveData(null, 21);
        Check(manager.RackPlanningModelVersion == RackPlanningModel.Legacy && !manager.RackPlanningV2Active,
            "a legacy reset on Legacy40 is model 0 with V2 inactive");
        Pass("V2 activation requires Enlarged80 AND model 1 (never geometry alone, never the file version)");
    }
}
