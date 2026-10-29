#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// S1-D T-06 Editor gate: management-event ID integrity at the real load routes (FR-E3 / AC-7).
// Disposable: stage under Assets/ForestPrototype only for this gate (run_save_event_id_integrity.py does it), then remove
// the source and its .meta. Restores the player's forest-save.json, .bak and .tmp byte for byte.
//
// For every case it writes a disk save, then drives CanLoad, LoadData(parsed copy) and Load() (the disk path) against
// the live ForestTest world, checks all three agree, that a rejection names the same shared reason as
// ForestSaveValidation.Validate, and that a rejection changes neither the live world nor any disk byte.
public static class SaveEventIdIntegrityVerification
{
    private const string ExitKey = "CCF.SaveEventIdIntegrity.Exit";
    [InitializeOnLoadMethod] private static void ExitHook()
    {
        EditorApplication.update += () =>
        {
            int code = SessionState.GetInt(ExitKey, -1);
            if (code < 0 || EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling || EditorApplication.isUpdating) return;
            SessionState.EraseInt(ExitKey); EditorApplication.Exit(code);
        };
    }
    public static void Begin() { EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity"); EditorApplication.isPlaying = true; }
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)] private static void Install()
    {
        if (Environment.GetCommandLineArgs().Contains("SaveEventIdIntegrityVerification.Begin"))
            new GameObject("Disposable save event-ID verification").AddComponent<SaveEventIdIntegrityRunner>();
    }
    public static void Finish(int code) { SessionState.SetInt(ExitKey, code); EditorApplication.delayCall += EditorApplication.ExitPlaymode; }
}

public sealed class SaveEventIdIntegrityRunner : MonoBehaviour
{
    private ForestSaveController saves;
    private ForestEcologyController ecology;
    private ForestPlayer player;
    private string path;
    private readonly Dictionary<string, byte[]> originals = new Dictionary<string, byte[]>();
    private static readonly FieldInfo MessageField = typeof(ForestSaveController).GetField("message", BindingFlags.NonPublic | BindingFlags.Instance);
    private int cases, rejected, accepted;

    private IEnumerator Start()
    {
        yield return null; yield return null;
        Exception failure = null;
        var routines = new Stack<IEnumerator>(); routines.Push(Verify());
        while (routines.Count > 0)
        {
            bool more = false; object current = null;
            try { more = routines.Peek().MoveNext(); if (more) current = routines.Peek().Current; }
            catch (Exception error) { failure = error; break; }
            if (!more) { routines.Pop(); continue; }
            if (current is IEnumerator nested) routines.Push(nested);
            else yield return current;
        }
        foreach (var entry in originals)
            if (entry.Value == null) { if (File.Exists(entry.Key)) File.Delete(entry.Key); }
            else File.WriteAllBytes(entry.Key, entry.Value);
        if (failure == null) Debug.Log($"SAVE_EVENT_ID_VERIFY_PASS cases={cases} rejected={rejected} accepted={accepted} canLoad=LoadData=Load sharedReason=exact diskBytes=unchanged noMutation=exact");
        else Debug.LogError("SAVE_EVENT_ID_VERIFY_FAIL " + failure);
        SaveEventIdIntegrityVerification.Finish(failure == null ? 0 : 1);
    }

    private static void Check(bool ok, string message) { if (!ok) throw new InvalidOperationException(message); }
    private static ForestSaveData Clone(ForestSaveData data) => JsonUtility.FromJson<ForestSaveData>(JsonUtility.ToJson(data));

    // Since Rack Planning V2 (save v21, D-064) the live Enlarged80 world uses rack planning model 1, which the save validator
    // correctly refuses on any other stand. A Legacy40 copy must therefore also drop to model 0 with no V2 plan.
    // Returns how many rack-owned work orders were removed (a fresh world is expected to have none).
    private static int MakeRackModelZero(ForestSaveData data)
    {
        ScenarioOneSaveData scenario = data.scenarioOne;
        scenario.rackPlanningModel = RackPlanningModel.Legacy;
        scenario.racks = new List<RackSaveData>();
        scenario.rackPlanConfirmed = false;
        scenario.nextRackId = 1;
        if (scenario.workOrders == null) scenario.workOrders = new List<ScenarioOneWorkOrder>();
        return scenario.workOrders.RemoveAll(o => o != null && o.rackId > 0);
    }
    private string Message() => (string)MessageField.GetValue(saves);
    private string World()
    {
        ForestSaveData data = saves.CaptureData();
        return ecology.StandGeometryModelVersion + "|" + ecology.EcologicalYear + "|" + FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None).Length
            + "|" + JsonUtility.ToJson(player.transform.position) + "|" + JsonUtility.ToJson(player.transform.rotation)
            + "|" + ScenarioReferenceArchive.CurrentWorldHash(data) + "|" + JsonUtility.ToJson(data.scenarioOne);
    }
    private string Disk() => string.Join("|", new[] { path, path + ".bak", path + ".tmp" }
        .Select(p => File.Exists(p) ? Convert.ToBase64String(File.ReadAllBytes(p)) : "missing"));

    private static ScenarioManagementEvent Event(int id)
        => new ScenarioManagementEvent { eventId = id, year = 1, cellIndex = -1, eventType = ScenarioManagementEventType.YearAdvanced };

    // json: optional disk text override (used to model a save whose file has no managementEvents property).
    private IEnumerator Fixture(string label, ForestSaveData data, bool valid, string reasonFragment = null, string json = null)
    {
        cases++;
        File.WriteAllText(path, json ?? JsonUtility.ToJson(data));
        File.WriteAllText(path + ".bak", "backup fixture — must remain unchanged");
        File.WriteAllText(path + ".tmp", "temporary fixture — must remain unchanged");
        string world = World(), disk = Disk();
        int year = ecology.EcologicalYear, trees = FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None).Length;
        string validation = ForestSaveValidation.Validate(data, trees, 0);
        Check((validation == null) == valid, label + " central validation mismatch: " + validation);
        if (!valid) Check(reasonFragment != null && validation.Contains(reasonFragment), label + " reason '" + validation + "' lacks '" + reasonFragment + "'");

        // Route 1: CanLoad reads the disk save through the shared FindLoadProblem seam.
        bool can = saves.CanLoad(out string canProblem);
        Check(can == valid, label + " CanLoad mismatch: " + canProblem);
        Check(World() == world && Disk() == disk, label + " CanLoad mutated world/disk");
        if (!valid) Check(canProblem == $"Save not loaded: {validation}.", label + " CanLoad reason differs from the shared validator: " + canProblem);

        // Route 2: LoadData on equivalent parsed data.
        bool loaded = saves.LoadData(Clone(data), true);
        Check(loaded == valid, label + " LoadData acceptance differs from CanLoad");
        if (!valid)
        {
            rejected++;
            Check(Message() == $"Save not loaded: {validation}.", label + " LoadData message differs from the shared validator: " + Message());
            Check(World() == world && Disk() == disk, label + " LoadData mutated before refusal");
            Check(ecology.EcologicalYear == year, label + " ecological year changed on rejection");
            Check(FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None).Length == trees, label + " tree count changed on rejection");
            // Route 3: Load() reads the disk save, then takes the same LoadData path.
            saves.Load();
            Check(Message() == $"Save not loaded: {validation}.", label + " disk Load message differs from the shared validator: " + Message());
            Check(World() == world && Disk() == disk, label + " disk Load mutated before refusal");
            yield return null;
            Check(World() == world && Disk() == disk, label + " delayed mutation after refusal");
        }
        else
        {
            accepted++;
            Check(Disk() == disk, label + " valid LoadData wrote disk");
            yield return null;
            saves.Load(); yield return null;
            Check(ecology.EcologicalYear == data.ecologicalYear && Disk() == disk, label + " valid disk Load mismatch");
        }
        Debug.Log("SAVE_EVENT_ID_CASE " + label + " " + (valid ? "ACCEPT" : "REJECT " + validation) + " CanLoad=LoadData=Load disk=unchanged"
            + (valid ? "" : " world=unchanged") + " beforeWorld=" + ScenarioReferenceArchive.Hash(world) + " afterWorld=" + ScenarioReferenceArchive.Hash(World())
            + " beforeDisk=" + ScenarioReferenceArchive.Hash(disk) + " afterDisk=" + ScenarioReferenceArchive.Hash(Disk()));
    }

    private ForestSaveData WithEvents(ForestSaveData basis, params int[] ids)
    {
        ForestSaveData data = Clone(basis);
        data.scenarioOne.managementEvents = ids.Select(Event).ToList();
        data.scenarioOne.nextManagementEventId = Mathf.Max(1, ids.DefaultIfEmpty(0).Max() + 1);
        return data;
    }

    private IEnumerator Verify()
    {
        saves = FindFirstObjectByType<ForestSaveController>(); ecology = FindFirstObjectByType<ForestEcologyController>(); player = FindFirstObjectByType<ForestPlayer>();
        Check(saves != null && ecology != null && player != null && MessageField != null, "scene services or message field missing");
        player.enabled = false; // Freeze movement during the no-mutation snapshots.
        path = Path.Combine(Application.persistentDataPath, "forest-save.json"); Directory.CreateDirectory(Path.GetDirectoryName(path));
        foreach (string p in new[] { path, path + ".bak", path + ".tmp" }) originals[p] = File.Exists(p) ? File.ReadAllBytes(p) : null;

        // Same basis as the cell-index gate: the live Enlarged80 world, and a Legacy40 copy of it.
        ForestSaveData enlarged = saves.CaptureData(); Check(enlarged.standGeometryModel == 1 && ecology.CellCount == 256, "gate must start on actual Enlarged80 policy");
        var recorded = enlarged.cells.ToDictionary(c => c.index);
        enlarged.cells = Enumerable.Range(0, 256).Select(i => recorded.TryGetValue(i, out var cell) ? cell : new ForestCellSaveData { index = i, establishmentSuitability = 1f }).ToList();
        ForestSaveData legacy = Clone(enlarged); legacy.standGeometryModel = 0; legacy.cells = legacy.cells.Take(64).ToList();
        legacy.scenarioOne.understoreyCells = legacy.scenarioOne.understoreyCells.Take(64).ToList();
        int rackOrdersRemoved = MakeRackModelZero(legacy);
        // Precondition: the Legacy40 basis is a valid model-0 world, so every geometry=0 verdict below is about event IDs.
        Check(legacy.standGeometryModel == 0 && legacy.cells.Count == 64 && legacy.scenarioOne.understoreyCells.Count == 64,
            "Legacy40 basis must be the 64-cell model-0 stand");
        Check(legacy.scenarioOne.rackPlanningModel == RackPlanningModel.Legacy && legacy.scenarioOne.racks.Count == 0
            && !legacy.scenarioOne.rackPlanConfirmed && legacy.scenarioOne.nextRackId == 1
            && !legacy.scenarioOne.workOrders.Any(o => o != null && o.rackId > 0), "Legacy40 basis must carry no Rack V2 state");
        Check(ForestSaveValidation.ValidateRackV2(legacy) == null, "Legacy40 basis refused by the Rack V2 validator: " + ForestSaveValidation.ValidateRackV2(legacy));
        Debug.Log("SAVE_EVENT_ID_LEGACY40_BASIS geometry=0 cells=64 rackPlanningModel=0 racks=0 confirmed=false nextRackId=1 rackOwnedOrdersRemoved=" + rackOrdersRemoved
            + " enlargedRackPlanningModel=" + enlarged.scenarioOne.rackPlanningModel + " enlargedRacks=" + enlarged.scenarioOne.racks.Count);

        foreach (ForestSaveData basis in new[] { legacy, enlarged })
        {
            string geometry = " geometry=" + basis.standGeometryModel;
            yield return Fixture("valid distinct positive IDs 1,2,3" + geometry, WithEvents(basis, 1, 2, 3), true);
            yield return Fixture("valid distinct IDs out of order with gaps 9,2,5" + geometry, WithEvents(basis, 9, 2, 5), true);
            yield return Fixture("empty event list" + geometry, WithEvents(basis), true);
            ForestSaveData nullList = Clone(basis); nullList.scenarioOne.managementEvents = null;
            JObject missing = JObject.Parse(JsonUtility.ToJson(WithEvents(basis, 1)));
            ((JObject)missing["scenarioOne"]).Remove("managementEvents");
            yield return Fixture("event list absent from the disk JSON" + geometry, nullList, true, null, missing.ToString());
            yield return Fixture("eventId zero" + geometry, WithEvents(basis, 1, 0, 3), false, "non-positive ID (0)");
            yield return Fixture("eventId negative" + geometry, WithEvents(basis, 1, 2, -7), false, "non-positive ID (-7)");
            yield return Fixture("duplicate positive eventId" + geometry, WithEvents(basis, 1, 2, 3, 2), false, "ID 2 appears twice");
            yield return Fixture("duplicate eventId 1 (first two records)" + geometry, WithEvents(basis, 1, 1), false, "ID 1 appears twice");
        }
        // Older versions keep the rule (no schema branch) and a clean older save still loads.
        ForestSaveData historical = WithEvents(legacy, 1, 2); historical.version = 19;
        yield return Fixture("historical v19 valid history", historical, true);
        ForestSaveData historicalBad = WithEvents(legacy, 4, 4); historicalBad.version = 19;
        yield return Fixture("historical v19 duplicate history", historicalBad, false, "ID 4 appears twice");
        // The disk rejection must not have rewritten, renumbered or backed up anything.
        yield return Fixture("restore enlarged baseline", Clone(enlarged), true);
    }
}
#endif
