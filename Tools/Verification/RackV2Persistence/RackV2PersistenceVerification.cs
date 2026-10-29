using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

// Disposable RACK-V2-11 gate: Rack V2 persistence through the REAL disk save path.
//
// The RACK-V2-03B save gate (Tools/Verification/RackV2Save) proves the v21 schema rules in memory, and the
// 03C runtime / 09 field gates prove in-memory CaptureData -> JSON -> LoadData round trips. None of them
// writes or reads forest-save.json. This gate does: every round trip below goes through
// ForestSaveController.Save() (the F5 path), CanLoad() (the title screen's Continue offer) and Load()
// (F9 / Continue), so the on-disk JSON, TryReadSave's JSON-presence checks and the refusal paths are covered.
//
// Modes (CCF_RACKV2_11_MODE, set by run_gate.py):
//   v2         fresh Enlarged80 game: new game, draft with stable non-contiguous ids, confirmation, partial and
//              all entrance marks, pre-V2 (v20) save before planning, pre-V2 save with legacy rack progress,
//              malformed V2 files, Reference Future v1 untouched. Exports the partial-mark save for coldstart.
//   legacy40   fresh Legacy40 game (CCF_STAND_GEOMETRY=0): v21 and v20 Legacy40 saves load unchanged and never
//              enter V2, with and without legacy rack progress; a Legacy40 file claiming V2 is refused.
//   coldstart  a NEW Editor process whose isolated save slot holds the v2 run's partial-mark save: Continue
//              (CanLoad + Load) restores the exact rack plan, ids, marks and world hash.
//
// Save safety: refuses to run unless Application.persistentDataPath is inside the runner's isolated
// XDG_CONFIG_HOME. Any forest-save.json / .bak / .tmp already in that slot is backed up first and restored at
// the end (coldstart's seeded file included). It never saves a scene, writes a reference, or advances a year.
// Copy into Assets/ForestPrototype, run RackV2PersistenceVerification.Begin, then remove the copy and its .meta
// (run_gate.py does all of this). No assertion here rebaselines any protected anchor.
public static class RackV2PersistenceVerification
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
        if (Environment.GetCommandLineArgs().Any(a => a == "RackV2PersistenceVerification.Begin"))
            new GameObject("Disposable rack v2 persistence verification").AddComponent<RackV2PersistenceVerificationRunner>();
    }
}

// What the v2 run hands the coldstart run (written next to the exported save, never into Assets).
[Serializable]
public sealed class RackV2PersistenceExpectation
{
    public string signature;
    public string worldHash;
    public int markedCount;
    public int nextRackId;
    public List<int> rackIds = new List<int>();
    public List<string> markedEntranceTreeIds = new List<string>();
}

public sealed class RackV2PersistenceVerificationRunner : MonoBehaviour
{
    private const string SaveFileName = "forest-save.json";
    private const string PartialExportName = "rackv2-11-partial-save.json";
    private const string ExpectationName = "rackv2-11-partial-expected.json";

    private ForestEcologyController ecology;
    private ScenarioOneManager manager;
    private ForestSaveController saves;
    private ForestTreeMarkingManager marks;
    private readonly List<string> log = new List<string>();
    private readonly Dictionary<string, byte[]> slotBackup = new Dictionary<string, byte[]>();
    private string mode;

    private static void Check(bool ok, string message) { if (!ok) throw new InvalidOperationException(message); }
    private void Pass(string message) { log.Add(message); Debug.Log("RACKV2PERSIST_CHECK " + message); }
    private static void Note(string message) => Debug.Log("RACKV2PERSIST_NOTE " + message);

    private static string SlotPath(string name) => Path.Combine(Application.persistentDataPath, name);
    private static string SavePath => SlotPath(SaveFileName);
    private static readonly string[] SlotFiles = { SaveFileName, SaveFileName + ".bak", SaveFileName + ".tmp" };
    private static string EvidenceDir => Environment.GetEnvironmentVariable("CCF_DIAG_DIR");

    // ---- harness plumbing ---------------------------------------------------------------------------------

    private IEnumerator Start()
    {
        for (int i = 0; i < 6; i++) yield return null;
        Exception failure = null;
        bool backedUp = false;
        try
        {
            mode = Environment.GetEnvironmentVariable("CCF_RACKV2_11_MODE") ?? "";
            string config = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME");
            Check(!string.IsNullOrEmpty(config) && Path.GetFullPath(Application.persistentDataPath)
                .StartsWith(Path.GetFullPath(config) + Path.DirectorySeparatorChar, StringComparison.Ordinal),
                "persistentDataPath must be inside the isolated XDG_CONFIG_HOME: " + Application.persistentDataPath);
            Check(!string.IsNullOrEmpty(EvidenceDir) && Directory.Exists(EvidenceDir), "CCF_DIAG_DIR evidence directory required");
            Directory.CreateDirectory(Application.persistentDataPath);
            BackUpSlot();
            backedUp = true;
        }
        catch (Exception error) { failure = error; }

        if (failure == null)
        {
            var stack = new Stack<IEnumerator>();
            stack.Push(mode == "v2" ? RunV2() : mode == "legacy40" ? RunLegacy40() : mode == "coldstart" ? RunColdStart() : BadMode());
            while (stack.Count > 0)
            {
                bool more; object current = null;
                try { more = stack.Peek().MoveNext(); if (more) current = stack.Peek().Current; }
                catch (Exception error) { failure = error; break; }
                if (!more) { stack.Pop(); continue; }
                if (current is IEnumerator nested) { stack.Push(nested); continue; }
                yield return current;
            }
        }

        if (backedUp)
        {
            try { RestoreSlot(); }
            catch (Exception error) { failure = failure ?? error; }
        }
        Debug.Log(failure == null
            ? "RACKV2PERSIST_VERIFY_PASS mode=" + mode + " checks=" + log.Count
            : "RACKV2PERSIST_VERIFY_FAIL mode=" + mode + ": " + failure);
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(failure == null ? 0 : 1);
#endif
    }

    private IEnumerator BadMode()
    {
        Check(false, "CCF_RACKV2_11_MODE must be v2, legacy40 or coldstart (got '" + mode + "')");
        yield break;
    }

    // Whatever was in the isolated slot before the gate is put back byte-for-byte afterwards; files the gate
    // created are removed. Run inside the isolated config only (checked above), so the player's save is never here.
    private void BackUpSlot()
    {
        foreach (string name in SlotFiles)
            if (File.Exists(SlotPath(name)))
                slotBackup[name] = File.ReadAllBytes(SlotPath(name));
        Note("slot backup: " + (slotBackup.Count == 0 ? "empty slot" : string.Join(",", slotBackup.Keys)));
    }

    private void RestoreSlot()
    {
        foreach (string name in SlotFiles)
        {
            if (slotBackup.TryGetValue(name, out byte[] bytes)) File.WriteAllBytes(SlotPath(name), bytes);
            else if (File.Exists(SlotPath(name))) File.Delete(SlotPath(name));
        }
        foreach (string name in SlotFiles)
            Check(slotBackup.TryGetValue(name, out byte[] bytes) ? File.ReadAllBytes(SlotPath(name)).SequenceEqual(bytes) : !File.Exists(SlotPath(name)),
                "slot restore exact for " + name);
        Note("slot restored to its pre-gate contents");
    }

    private void FindScene()
    {
        ecology = FindFirstObjectByType<ForestEcologyController>();
        manager = FindFirstObjectByType<ScenarioOneManager>();
        saves = FindFirstObjectByType<ForestSaveController>();
        marks = FindFirstObjectByType<ForestTreeMarkingManager>();
        Check(ecology != null && manager != null && saves != null && marks != null, "not the Scenario One ForestTest scene");
        Check(ForestSaveData.CurrentVersion == 21, "this gate qualifies save v21 (no schema change expected)");
    }

    private static IEnumerator Settle()
    {
        yield return null;
        yield return null;
    }

    private string Hash() => ScenarioReferenceArchive.WorldHash(saves.CaptureData());

    private ForestTree Tree(string id)
        => FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).FirstOrDefault(t => t != null && t.TreeId == id);

    // Live rack plan + every work order + every tree mark, from the authoritative manager state.
    private string Signature()
    {
        var parts = new List<string>
        {
            "model=" + manager.RackPlanningModelVersion, "confirmed=" + manager.RackPlanConfirmed, "next=" + manager.NextRackId,
            "geometry=" + ecology.StandGeometryModelVersion, "v2=" + manager.RackPlanningV2Active
        };
        foreach (RackSaveData rack in manager.Racks)
            parts.Add(rack.rackId + ":" + rack.entranceX.ToString("R") + "/" + rack.entranceZ.ToString("R") + ":"
                + rack.farX.ToString("R") + "/" + rack.farZ.ToString("R") + ":" + rack.entranceTreeId + ":" + rack.entranceMarked + ":"
                + string.Join(",", rack.removalTreeIds));
        foreach (ScenarioOneWorkOrder order in manager.WorkOrders.OrderBy(order => order.workOrderId))
            parts.Add("o" + order.workOrderId + ":" + order.type + ":" + order.status + ":" + order.targetTreeId + ":" + order.rackId);
        foreach (TreeSaveData tree in saves.CaptureData().trees.Where(t => t.markType != 0).OrderBy(t => t.treeId, StringComparer.Ordinal))
            parts.Add("m" + tree.treeId + ":" + tree.markType);
        return string.Join("|", parts);
    }

    private static T NoThrow<T>(Func<T> action, string label)
    {
        try { return action(); }
        catch (Exception error) { throw new InvalidOperationException(label + " threw (a crash on load): " + error); }
    }

    private static void NoThrow(Action action, string label) => NoThrow(() => { action(); return 0; }, label);

    // F5: the production atomic disk write. Returns the exact file text.
    private string SaveToDisk(string label)
    {
        NoThrow(() => saves.Save(), "Save() for " + label);
        Check(File.Exists(SavePath), "Save() wrote forest-save.json for " + label);
        Check(!File.Exists(SlotPath(SaveFileName + ".tmp")), "Save() left no temporary file for " + label);
        return File.ReadAllText(SavePath);
    }

    // Continue: CanLoad offers it, Load acts. Both must accept a valid file.
    private void ContinueFromDisk(string label)
    {
        string problem = null;
        Check(NoThrow(() => saves.CanLoad(out problem), "CanLoad() for " + label), "Continue is offered for " + label + ": " + problem);
        NoThrow(() => saves.Load(), "Load() for " + label);
    }

    // Writes the given text as the save file, then proves Continue is refused, Load() neither throws nor changes the world.
    private void ExpectDiskRefusal(string label, string text)
    {
        string before = Hash(), beforeSig = Signature();
        File.WriteAllText(SavePath, text);
        string problem = null;
        bool offered = NoThrow(() => saves.CanLoad(out problem), "CanLoad() for malformed " + label);
        Check(!offered && !string.IsNullOrEmpty(problem), "Continue must be refused for malformed " + label);
        NoThrow(() => saves.Load(), "Load() for malformed " + label);
        Check(Hash() == before && Signature() == beforeSig, "a refused " + label + " leaves the world and rack plan untouched");
        log.Add("refused on disk: " + label + " -> " + problem);
        Debug.Log("RACKV2PERSIST_REFUSED " + label + " -> " + problem);
    }

    private static string Mutate(string json, Action<JObject> change)
    {
        JObject root = JObject.Parse(json);
        change(root);
        return root.ToString(Newtonsoft.Json.Formatting.Indented);
    }

    private static JObject Scenario(JObject root) => (JObject)root["scenarioOne"];
    private static JArray RackArray(JObject root) => (JArray)Scenario(root)["racks"];

    // The literal bytes v21 adds while the plan is empty, removed independently of production code (same rule as the
    // 03B gate): the result is the byte layout a genuine v20 file has. Works on compact JsonUtility output.
    private static string ToV20(string compactV21)
    {
        string v20 = compactV21
            .Replace(",\"rackPlanningModel\":0,\"rackPlanConfirmed\":false,\"nextRackId\":1,\"racks\":[]", "")
            .Replace(",\"rackPlanningModel\":1,\"rackPlanConfirmed\":false,\"nextRackId\":1,\"racks\":[]", "")
            .Replace(",\"rackId\":0", "")
            .Replace("\"version\":21,", "\"version\":20,");
        Check(!v20.Contains("rackPlanningModel") && !v20.Contains("\"rackId\"") && v20.Contains("\"version\":20,"),
            "the v20 projection carries no v21 bytes");
        return v20;
    }

    private string CompactCapture() => JsonUtility.ToJson(saves.CaptureData());

    private IEnumerator LoadInMemory(string json, string label)
    {
        Check(saves.LoadData(JsonUtility.FromJson<ForestSaveData>(json), false), label);
        yield return Settle();
    }

    private void CheckArchiveUntouched()
    {
        ScenarioReferenceArchive archive = ScenarioReferenceArchive.Load();
        Check(archive != null && archive.milestones.Count == 4, "the frozen Reference Future v1 archive still loads");
        Check(archive.milestones.All(m => m.verifiedFrozenWorld), "every frozen milestone still matches its embedded world hash");
        Check(archive.AtYear(100).worldHash == "7AD177B3CC2F73C7", "the frozen Year-100 world hash is untouched");
        Pass("Reference Future v1 archive verified and Year-100 hash 7AD177B3CC2F73C7 untouched");
    }

    // Legacy model-0 rack felling, exactly as a pre-V2 player did it (the ConfirmRack path, not V2 objects).
    private int PlanLegacyRack(RackPlanner planner, float lineZ)
    {
        foreach (float offset in new[] { 0f, 8f, -8f, 4f, -4f })
        {
            RackProposal proposal = planner.Propose(new RackPoint(0f, lineZ + offset), 0f);
            if (proposal == null || !proposal.CanConfirm) continue;
            int added = manager.ConfirmRack(proposal, "verification");
            if (added > 0) return added;
        }
        return 0;
    }

    private void CheckNeverV2(string label)
    {
        Check(manager.RackPlanningModelVersion == RackPlanningModel.Legacy && !manager.RackPlanningV2Active && manager.Racks.Count == 0
            && !manager.RackPlanConfirmed && manager.NextRackId == 1, label + ": model 0, V2 inactive, no V2 rack state");
        Check(manager.WorkOrders.All(order => order.rackId == 0), label + ": every order stays outside the V2 rack link");
        Check(manager.AddRack(new RackPoint(-10f, 0f), new RackPoint(10f, 0f)).Status == RackPlanOperationStatus.Invalid
            && manager.Racks.Count == 0, label + ": the V2 authoring API refuses to create a rack");
        Check(!manager.ConfirmRackPlan().Succeeded && !manager.RackPlanConfirmed, label + ": the V2 confirm API refuses");
    }

    // ---- v2 mode ------------------------------------------------------------------------------------------

    private IEnumerator RunV2()
    {
        FindScene();
        Check(StandGeometryPolicy.NewGameModelForSession == StandGeometryModel.Enlarged80, "v2 mode needs the production Enlarged80 new-game policy");
        Check(!File.Exists(SavePath), "v2 mode starts from an empty isolated save slot");

        // ---- 1. New game -------------------------------------------------------------------------------
        Check(ecology.StandGeometryModelVersion == StandGeometryModel.Enlarged80 && manager.RackPlanningV2Active
            && manager.Racks.Count == 0 && !manager.RackPlanConfirmed && manager.NextRackId == 1, "a new game is an empty V2 world");
        string freshCompact = CompactCapture();
        string freshHash = Hash(), freshSig = Signature();
        string freshFile = SaveToDisk("new game");
        JObject freshRoot = JObject.Parse(freshFile);
        Check((int)freshRoot["version"] == 21 && (int)freshRoot["standGeometryModel"] == StandGeometryModel.Enlarged80
            && (int)Scenario(freshRoot)["rackPlanningModel"] == RackPlanningModel.RackPlanningFieldMarkingV2
            && (bool)Scenario(freshRoot)["rackPlanConfirmed"] == false && (int)Scenario(freshRoot)["nextRackId"] == 1
            && RackArray(freshRoot).Count == 0, "the new-game file states v21, Enlarged80 and the empty V2 plan explicitly");
        // Change the live world, then Continue must bring the saved new game back.
        Check(manager.ResetRacksToSuggestion(manager.GetSuggestedRackLayout()).Succeeded && manager.Racks.Count == 5, "draft the suggestion before reload");
        ContinueFromDisk("new game");
        yield return Settle();
        Check(Signature() == freshSig && Hash() == freshHash, "Continue restores the saved new game exactly (empty V2 plan, world hash)");
        Pass("new game: v21 disk save, Continue offered, reload exact");

        // ---- 2. Draft before confirmation, with stable non-contiguous ids --------------------------------
        Check(manager.ResetRacksToSuggestion(manager.GetSuggestedRackLayout()).Succeeded && manager.Racks.Count == 5 && !manager.RackPlanConfirmed,
            "the suggested layout becomes five draft racks");
        int[] firstIds = manager.Racks.Select(r => r.rackId).ToArray();
        RackSaveData removed = manager.Racks[1];
        RackPoint removedEntrance = new RackPoint(removed.entranceX, removed.entranceZ), removedFar = new RackPoint(removed.farX, removed.farZ);
        int nextBefore = manager.NextRackId;
        Check(manager.DeleteRack(removed.rackId).Succeeded && manager.Racks.Count == 4, "a draft rack can be deleted");
        RackPlanOperationResult readd = manager.AddRack(removedEntrance, removedFar);
        Check(readd.Succeeded && readd.RackId == nextBefore && !firstIds.Contains(readd.RackId), "a re-added rack gets a new id, never a reused one");
        int[] ids = manager.Racks.Select(r => r.rackId).ToArray();
        Check(ids.Length == 5 && ids.Distinct().Count() == 5 && manager.NextRackId == readd.RackId + 1, "five draft racks with a gap in their ids");
        Note("draft rack ids " + string.Join(",", ids) + " next " + manager.NextRackId);
        string draftHash = Hash(), draftSig = Signature();
        string draftFile = SaveToDisk("draft");
        Check(RackArray(JObject.Parse(draftFile)).Select(r => (int)r["rackId"]).SequenceEqual(ids)
            && (int)Scenario(JObject.Parse(draftFile))["nextRackId"] == manager.NextRackId, "the draft file stores the exact ids and allocator");
        yield return LoadInMemory(freshCompact, "clear to the new game before reloading the draft");
        Check(manager.Racks.Count == 0, "world cleared");
        ContinueFromDisk("draft");
        yield return Settle();
        Check(Signature() == draftSig && Hash() == draftHash, "Continue restores the draft exactly");
        Check(manager.Racks.Select(r => r.rackId).SequenceEqual(ids) && !manager.RackPlanConfirmed
            && manager.WorkOrders.All(o => o.rackId == 0), "draft ids survive; no rack work exists before confirmation");
        Pass("draft before confirmation: non-contiguous ids " + string.Join(",", ids) + " and nextRackId survive disk reload");

        // ---- 3. Confirmation ---------------------------------------------------------------------------
        Check(manager.ConfirmRackPlanFromMap().Succeeded && manager.RackPlanConfirmed && manager.Racks.Count == 5, "the plan confirms");
        Check(manager.WorkOrders.Any(o => o.IsOpen && o.rackId > 0) && manager.WorkOrders.Where(o => o.rackId > 0).All(o => ids.Contains(o.rackId)),
            "confirmation creates rack-owned work linked to the stable ids");
        Check(manager.MarkedRackEntranceCount == 0, "nothing is marked at confirmation");
        string confirmedHash = Hash(), confirmedSig = Signature();
        SaveToDisk("confirmed");
        yield return LoadInMemory(freshCompact, "clear to the new game before reloading the confirmed plan");
        ContinueFromDisk("confirmed");
        yield return Settle();
        Check(Signature() == confirmedSig && Hash() == confirmedHash, "Continue restores the confirmed plan, links and frozen sets exactly");
        Check(manager.RackPlanConfirmed && manager.Racks.Select(r => r.rackId).SequenceEqual(ids) && manager.MarkedRackEntranceCount == 0,
            "confirmed ids survive and the mark count is derived as 0");
        Pass("save/reload after confirmation exact on disk");

        // ---- 4. Partial entrance marking ---------------------------------------------------------------
        int[] partialIds = { ids[0], ids[4] };   // includes the re-added id
        foreach (int id in partialIds)
            Check(manager.MarkRackEntrance(id).Succeeded, "mark rack " + id + " entrance");
        Check(manager.MarkedRackEntranceCount == 2, "two entrances marked");
        string partialHash = Hash(), partialSig = Signature();
        string partialFile = SaveToDisk("partial marks");
        yield return LoadInMemory(freshCompact, "clear to the new game before reloading partial marks");
        Check(manager.MarkedRackEntranceCount == 0, "world cleared");
        ContinueFromDisk("partial marks");
        yield return Settle();
        Check(Signature() == partialSig && Hash() == partialHash, "Continue restores the partial-mark state exactly");
        Check(manager.MarkedRackEntranceCount == 2, "the derived marked count is 2 after reload");
        foreach (RackSaveData rack in manager.Racks)
        {
            ForestTree entrance = Tree(rack.entranceTreeId);
            bool shouldBeMarked = partialIds.Contains(rack.rackId);
            Check(rack.entranceMarked == shouldBeMarked && entrance != null && marks.HasRackEntranceMark(entrance) == shouldBeMarked
                && entrance.MarkType == TreeMarkType.None, "rack " + rack.rackId + " orange state is projected from its record only");
        }
        Pass("partial entrance marking (racks " + string.Join(",", partialIds) + ") survives disk reload exactly");
        File.Copy(SavePath, Path.Combine(EvidenceDir, PartialExportName), true);
        var expectation = new RackV2PersistenceExpectation
        {
            signature = partialSig, worldHash = partialHash, markedCount = 2, nextRackId = manager.NextRackId,
            rackIds = ids.ToList(), markedEntranceTreeIds = manager.Racks.Where(r => r.entranceMarked).Select(r => r.entranceTreeId).ToList()
        };
        File.WriteAllText(Path.Combine(EvidenceDir, ExpectationName), JsonUtility.ToJson(expectation, true));

        // ---- 5. All marks ------------------------------------------------------------------------------
        foreach (int id in ids.Except(partialIds))
            Check(manager.MarkRackEntrance(id).Succeeded, "mark rack " + id + " entrance");
        Check(manager.MarkedRackEntranceCount == 5, "all five entrances marked");
        string fullHash = Hash(), fullSig = Signature();
        string fullFile = SaveToDisk("all marks");
        yield return LoadInMemory(freshCompact, "clear to the new game before reloading all marks");
        ContinueFromDisk("all marks");
        yield return Settle();
        Check(Signature() == fullSig && Hash() == fullHash && manager.MarkedRackEntranceCount == 5, "Continue restores all five marks exactly");
        Check(manager.Racks.All(r => Tree(r.entranceTreeId) is ForestTree t && marks.HasRackEntranceMark(t)), "all five orange marks are projected after reload");
        Check(manager.Racks.Select(r => r.rackId).SequenceEqual(ids), "rack ids are stable through every reload");
        // Saving the reloaded world again writes the same rack state (no drift on a second save cycle).
        string fullAgain = SaveToDisk("all marks re-save");
        Check(JToken.DeepEquals(Scenario(JObject.Parse(fullAgain))["racks"], Scenario(JObject.Parse(fullFile))["racks"])
            && JToken.DeepEquals(Scenario(JObject.Parse(fullAgain))["workOrders"], Scenario(JObject.Parse(fullFile))["workOrders"]),
            "a second save of the reloaded world writes identical rack records and work orders");
        Pass("all entrance marks survive disk reload and a second save cycle exactly");

        // ---- 6. Malformed V2 files: refused, no crash, world untouched ---------------------------------
        string valid = fullFile;
        int next = (int)Scenario(JObject.Parse(valid))["nextRackId"];
        ExpectDiskRefusal("truncated file", valid.Substring(0, valid.Length / 2));
        ExpectDiskRefusal("not JSON", "{ this is not a forest save");
        ExpectDiskRefusal("v21 missing nextRackId", Mutate(valid, r => Scenario(r).Property("nextRackId").Remove()));
        ExpectDiskRefusal("v21 missing racks", Mutate(valid, r => Scenario(r).Property("racks").Remove()));
        ExpectDiskRefusal("racks of the wrong type", Mutate(valid, r => Scenario(r)["racks"] = "five racks"));
        ExpectDiskRefusal("rack record missing entranceMarked", Mutate(valid, r => ((JObject)RackArray(r)[0]).Property("entranceMarked").Remove()));
        ExpectDiskRefusal("rack record missing rackId", Mutate(valid, r => ((JObject)RackArray(r)[0]).Property("rackId").Remove()));
        ExpectDiskRefusal("work order missing rackId", Mutate(valid, r => ((JObject)((JArray)Scenario(r)["workOrders"])[0]).Property("rackId").Remove()));
        ExpectDiskRefusal("non-string removal tree id", Mutate(valid, r => ((JArray)RackArray(r)[0]["removalTreeIds"]).Add(42)));
        string indented = Mutate(valid, r => { });
        string duplicated = indented.Replace("\"nextRackId\": " + next + ",", "\"nextRackId\": " + next + ", \"nextRackId\": " + next + ",");
        Check(duplicated != indented, "the duplicate-property fixture was built");
        ExpectDiskRefusal("duplicate nextRackId property", duplicated);
        ExpectDiskRefusal("NaN rack endpoint", Mutate(valid, r => RackArray(r)[0]["farX"] = new JValue(double.NaN)));
        ExpectDiskRefusal("unknown rackPlanningModel", Mutate(valid, r => Scenario(r)["rackPlanningModel"] = 2));
        ExpectDiskRefusal("duplicate rack ids", Mutate(valid, r => RackArray(r)[1]["rackId"] = RackArray(r)[0]["rackId"]));
        ExpectDiskRefusal("nextRackId reuses a live id", Mutate(valid, r => Scenario(r)["nextRackId"] = ids.Max()));
        ExpectDiskRefusal("confirmed plan with four racks", Mutate(valid, r => RackArray(r).RemoveAt(4)));
        ExpectDiskRefusal("marked entrance with no identity", Mutate(valid, r => RackArray(r)[0]["entranceTreeId"] = ""));
        ExpectDiskRefusal("entrance tree outside its removal set", Mutate(valid, r => RackArray(r)[0]["entranceTreeId"] = RackArray(r)[1]["entranceTreeId"]));
        ExpectDiskRefusal("unknown removal tree id", Mutate(valid, r => ((JArray)RackArray(r)[0]["removalTreeIds"]).Add("NO-SUCH-TREE")));
        ExpectDiskRefusal("work order linked to a missing rack", Mutate(valid, r =>
        {
            foreach (JToken order in (JArray)Scenario(r)["workOrders"])
                if ((int)order["rackId"] > 0) { order["rackId"] = ids.Max() + 50; break; }
        }));
        ExpectDiskRefusal("model-0 world carrying V2 racks", Mutate(valid, r => Scenario(r)["rackPlanningModel"] = 0));
        ExpectDiskRefusal("pre-v21 header claiming V2", Mutate(valid, r => r["version"] = 20));
        Check(log.Count(l => l.StartsWith("refused on disk:")) == 21, "all 21 malformed disk cases ran");
        File.WriteAllText(SavePath, valid);
        ContinueFromDisk("valid file restored after the malformed cases");
        yield return Settle();
        Check(Signature() == fullSig && Hash() == fullHash, "the valid file still loads exactly after the malformed cases");
        Pass("malformed V2 disk data: 21 cases refused by Continue and Load without a crash or any world change");

        // ---- 7. Old save before rack planning (v20 Enlarged80) ------------------------------------------
        string v20Fresh = ToV20(freshCompact);
        File.WriteAllText(SavePath, v20Fresh);
        ContinueFromDisk("v20 Enlarged80 before rack planning");
        yield return Settle();
        Check(ecology.StandGeometryModelVersion == StandGeometryModel.Enlarged80, "the v20 save keeps its Enlarged80 geometry");
        CheckNeverV2("v20 Enlarged80 save");
        Check(Hash() == freshHash, "the pre-planning old save loads with its pre-v21 world identity (empty-plan hash)");
        string legacyResave = SaveToDisk("v20 re-saved as v21");
        Check((int)JObject.Parse(legacyResave)["version"] == 21 && (int)Scenario(JObject.Parse(legacyResave))["rackPlanningModel"] == RackPlanningModel.Legacy,
            "re-saving an old world writes v21 with model 0 stated explicitly");
        yield return LoadInMemory(freshCompact, "clear to the V2 new game");
        ContinueFromDisk("re-saved old world");
        yield return Settle();
        CheckNeverV2("re-saved old world on its second load");
        Check(Hash() == freshHash, "the re-saved old world keeps its world hash");
        Pass("old save before rack planning: v20 loads model 0, re-saves as explicit model 0, never becomes V2");

        // ---- 8. Old save with existing (legacy) rack progress -------------------------------------------
        var planner = new RackPlanner(manager, ecology);
        planner.Refresh();
        Check(planner.LayoutLines.Count == 5, "the Enlarged80 stand has five authored layout lines");
        int legacyOrders = PlanLegacyRack(planner, planner.LayoutLines[0]) + PlanLegacyRack(planner, planner.LayoutLines[2]);
        Check(legacyOrders > 0 && manager.RackGroups.Count > 0, "the model-0 world plans legacy rack felling through ConfirmRack");
        Check(manager.WorkOrders.Count(o => o.IsOpen && o.type == ScenarioWorkType.FellTree) >= legacyOrders && manager.WorkOrders.All(o => o.rackId == 0),
            "legacy rack felling is ordinary rackId-0 work");
        string progressHash = Hash(), progressSig = Signature();
        string progressV21 = SaveToDisk("legacy rack progress (v21 model 0)");
        string progressV20 = ToV20(CompactCapture());
        foreach (var file in new[] { ("v21 model 0", progressV21), ("v20", progressV20) })
        {
            yield return LoadInMemory(freshCompact, "clear to the V2 new game before " + file.Item1);
            File.WriteAllText(SavePath, file.Item2);
            ContinueFromDisk("legacy rack progress " + file.Item1);
            yield return Settle();
            Check(Signature() == progressSig && Hash() == progressHash, "legacy rack progress (" + file.Item1 + ") reloads with every order and mark exact");
            CheckNeverV2("legacy rack progress " + file.Item1);
        }
        Note("legacy RackGroups after reload: " + manager.RackGroups.Count + " (session-only, not persisted; recorded behaviour, no assertion)");
        Pass("old save with legacy rack progress (v20 and v21 model 0): orders and marks kept as ordinary work, never V2");

        // ---- 9. Reference Future v1 -----------------------------------------------------------------------
        CheckArchiveUntouched();
    }

    // ---- legacy40 mode ------------------------------------------------------------------------------------

    private IEnumerator RunLegacy40()
    {
        FindScene();
        Check(StandGeometryPolicy.NewGameModelForSession == StandGeometryModel.Legacy40 && ecology.StandGeometryModelVersion == StandGeometryModel.Legacy40,
            "legacy40 mode needs a fresh Legacy40 game (CCF_STAND_GEOMETRY=0)");
        Check(!File.Exists(SavePath), "legacy40 mode starts from an empty isolated save slot");
        CheckNeverV2("fresh Legacy40 game");
        string freshCompact = CompactCapture();
        string freshHash = Hash(), freshSig = Signature();

        // ---- 1. A v21 Legacy40 save loads unchanged --------------------------------------------------------
        string v21File = SaveToDisk("Legacy40 v21");
        JObject root = JObject.Parse(v21File);
        Check((int)root["version"] == 21 && (int)root["standGeometryModel"] == StandGeometryModel.Legacy40
            && (int)Scenario(root)["rackPlanningModel"] == RackPlanningModel.Legacy && RackArray(root).Count == 0,
            "the Legacy40 file states Legacy40 and model 0 with an empty V2 plan");
        ContinueFromDisk("Legacy40 v21");
        yield return Settle();
        Check(ecology.StandGeometryModelVersion == StandGeometryModel.Legacy40 && Signature() == freshSig && Hash() == freshHash,
            "the Legacy40 v21 save loads unchanged");
        CheckNeverV2("Legacy40 v21 after load");
        string v21Again = SaveToDisk("Legacy40 v21 re-save");
        Note("Legacy40 re-save byte-identical to first save: " + (v21Again == v21File));
        Check(JToken.DeepEquals(Scenario(JObject.Parse(v21Again)), Scenario(root)) && (int)JObject.Parse(v21Again)["standGeometryModel"] == StandGeometryModel.Legacy40,
            "re-saving the loaded Legacy40 world writes the same Scenario One state");
        Pass("Legacy40 v21 save: Continue loads it unchanged (geometry, signature, world hash) and it never enters V2");

        // ---- 2. A v20 Legacy40 save loads unchanged ------------------------------------------------------
        File.WriteAllText(SavePath, ToV20(freshCompact));
        ContinueFromDisk("Legacy40 v20");
        yield return Settle();
        Check(ecology.StandGeometryModelVersion == StandGeometryModel.Legacy40 && Signature() == freshSig && Hash() == freshHash,
            "the Legacy40 v20 save loads unchanged");
        CheckNeverV2("Legacy40 v20 after load");
        Pass("Legacy40 v20 save loads unchanged and never enters V2");

        // ---- 3. Legacy40 with legacy rack progress ---------------------------------------------------------
        var planner = new RackPlanner(manager, ecology);
        planner.Refresh();
        var rows = planner.Rows(0f);
        Check(rows.Count > 2, "Legacy40 has planted rows");
        int added = PlanLegacyRack(planner, rows[rows.Count / 2].Centre);
        Check(added > 0 && manager.WorkOrders.All(o => o.rackId == 0), "Legacy40 plans legacy rack felling as ordinary work");
        string progressHash = Hash(), progressSig = Signature();
        string progressV21 = SaveToDisk("Legacy40 legacy rack progress");
        string progressV20 = ToV20(CompactCapture());
        foreach (var file in new[] { ("v21", progressV21), ("v20", progressV20) })
        {
            yield return LoadInMemory(freshCompact, "clear to the fresh Legacy40 game before " + file.Item1);
            File.WriteAllText(SavePath, file.Item2);
            ContinueFromDisk("Legacy40 legacy rack progress " + file.Item1);
            yield return Settle();
            Check(Signature() == progressSig && Hash() == progressHash, "Legacy40 rack progress (" + file.Item1 + ") reloads unchanged");
            CheckNeverV2("Legacy40 rack progress " + file.Item1);
        }
        Pass("Legacy40 with legacy rack progress (v20 and v21): unchanged, never V2");

        // ---- 4. A Legacy40 file that claims V2 is refused ---------------------------------------------------
        ExpectDiskRefusal("Legacy40 claiming model 1", Mutate(progressV21, r => Scenario(r)["rackPlanningModel"] = RackPlanningModel.RackPlanningFieldMarkingV2));
        ExpectDiskRefusal("Legacy40 carrying a V2 rack record", Mutate(progressV21, r =>
        {
            Scenario(r)["rackPlanningModel"] = RackPlanningModel.RackPlanningFieldMarkingV2;
            Scenario(r)["nextRackId"] = 2;
            RackArray(r).Add(JObject.FromObject(new { rackId = 1, entranceX = -10f, entranceZ = 0f, farX = 10f, farZ = 0f,
                removalTreeIds = new string[0], entranceTreeId = "", entranceMarked = false }));
        }));
        CheckNeverV2("Legacy40 after refused V2 claims");
        Pass("Legacy40 files claiming V2 are refused without a crash or world change");

        CheckArchiveUntouched();
    }

    // ---- coldstart mode -----------------------------------------------------------------------------------

    private IEnumerator RunColdStart()
    {
        FindScene();
        string expectationPath = Environment.GetEnvironmentVariable("CCF_RACKV2_11_EXPECTED");
        Check(!string.IsNullOrEmpty(expectationPath) && File.Exists(expectationPath), "coldstart needs the v2 run's expectation file");
        Check(File.Exists(SavePath), "coldstart needs the v2 run's partial-mark save seeded into the isolated slot");
        var expected = JsonUtility.FromJson<RackV2PersistenceExpectation>(File.ReadAllText(expectationPath));
        Check(manager.RackPlanningV2Active && manager.Racks.Count == 0, "the new Editor process came up as a fresh V2 game");
        string seeded = File.ReadAllText(SavePath);

        ContinueFromDisk("cold-start Continue of the partial-mark save");
        yield return Settle();
        Check(Signature() == expected.signature, "cold-start Continue restores the exact rack plan, orders and marks");
        Check(Hash() == expected.worldHash, "cold-start Continue restores the exact world hash");
        Check(manager.Racks.Select(r => r.rackId).SequenceEqual(expected.rackIds) && manager.NextRackId == expected.nextRackId,
            "rack ids and the id allocator are stable across processes");
        Check(manager.RackPlanConfirmed && manager.MarkedRackEntranceCount == expected.markedCount, "the derived marked count is restored");
        Check(manager.Racks.Where(r => r.entranceMarked).Select(r => r.entranceTreeId).SequenceEqual(expected.markedEntranceTreeIds)
            && expected.markedEntranceTreeIds.All(id => Tree(id) is ForestTree t && marks.HasRackEntranceMark(t)), "the orange marks are projected after cold start");
        Check(File.ReadAllText(SavePath) == seeded, "Continue does not rewrite the save file");
        Pass("cold-start Continue in a new Editor process restores the partial-mark V2 world exactly");

        CheckArchiveUntouched();
    }
}
