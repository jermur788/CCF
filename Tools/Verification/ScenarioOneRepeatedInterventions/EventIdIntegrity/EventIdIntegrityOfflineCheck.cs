// Offline check for S1-D T-06 management-event ID integrity (FR-E3 / AC-7). Compiles the PRODUCTION
// ForestSaveValidation*.cs and the real save-record classes and runs on the plain .NET SDK. Nothing here is used by the
// game and nothing is written inside the repository.
//
// Proves: valid distinct positive IDs accepted (any order or gaps); zero, negative and duplicate IDs rejected with a
// deterministic reason in every save version; null/empty lists still accepted; every older rejection keeps its reason and
// precedence; the validator never repairs, renumbers or reorders; the disk (JsonUtility) path and the parsed-data path
// reach the same verdict; every shipped/accepted fixture that carries an event list still validates; and the controller
// still funnels CanLoad and LoadData through one pre-mutation validation seam with no second ID rule.
//
// It does NOT prove Unity behaviour. The CanLoad/LoadData/disk-bytes/no-world-mutation proof needs the Editor harness
// SaveEventIdIntegrityVerification.cs. Usage: run_event_id_integrity_offline.py (prints EVENT_ID_OFFLINE_* tokens).
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using UnityEngine;

public static class EventIdIntegrityOfflineCheck
{
    private static int checks, failures;
    private static string root;

    private static void Check(bool condition, string label)
    {
        checks++;
        if (condition) { Console.WriteLine("PASS " + label); return; }
        failures++;
        Console.WriteLine("FAIL " + label);
    }

    private static ScenarioManagementEvent Event(int id, int cellIndex = -1)
        => new ScenarioManagementEvent { eventId = id, year = 1, cellIndex = cellIndex, eventType = ScenarioManagementEventType.YearAdvanced };

    // A small valid Legacy40-sized save (64 cells), as in the N3 offline check.
    private static ForestSaveData Save(List<ScenarioManagementEvent> events, int version = ForestSaveData.CurrentVersion)
        => new ForestSaveData
        {
            version = version, ecologicalYear = 25, growthModel = 1, regenerationModel = 1, stormModel = 0, standGeometryModel = 0,
            trees = new List<TreeSaveData> { new TreeSaveData { treeId = "P1", stage = 0 } },
            scenarioOne = new ScenarioOneSaveData
            {
                initialized = true, managementEvents = events,
                nextManagementEventId = (events == null ? 0 : events.Where(e => e != null).Select(e => e.eventId).DefaultIfEmpty(0).Max()) + 1
            }
        };

    private static string Validate(ForestSaveData data) => ForestSaveValidation.Validate(data, 1, 64);
    private static List<ScenarioManagementEvent> Ids(params int[] ids) => ids.Select(id => Event(id)).ToList();

    public static int Main()
    {
        root = Environment.GetEnvironmentVariable("CCF_REPO_ROOT");
        Accepts();
        Rejects();
        OldRulesUnchanged();
        NoRepair();
        DiskPathParity();
        Fixtures();
        SingleSeam();
        Console.WriteLine((failures == 0 ? "EVENT_ID_OFFLINE_PASS" : "EVENT_ID_OFFLINE_FAIL") + " checks=" + checks + " failures=" + failures);
        return failures == 0 ? 0 : 1;
    }

    private static void Accepts()
    {
        Check(Validate(Save(Ids(1, 2, 3))) == null, "valid history 1,2,3 accepted");
        Check(Validate(Save(Ids(9, 2, 5))) == null, "valid distinct IDs accepted in any order (no chronology rule)");
        Check(Validate(Save(Ids(4, 100, 7000))) == null, "valid distinct IDs with gaps accepted");
        Check(Validate(Save(Ids(1))) == null, "single event accepted");
        Check(Validate(Save(new List<ScenarioManagementEvent>())) == null, "empty event list accepted");
        Check(Validate(Save(null)) == null, "null event list accepted");
        var noScenario = Save(Ids(1)); noScenario.scenarioOne = null;
        Check(Validate(noScenario) == null, "save without Scenario One data accepted");
        foreach (int version in new[] { 1, 3, 12, 14, 15, 19, 20, 21 })
            Check(Validate(Save(Ids(2, 1, 3), version)) == null, "valid history accepted at save version " + version);
        // An older save whose JSON simply has no managementEvents property.
        var jsonRoot = JObject.Parse(JsonUtility.ToJson(Save(Ids(1))));
        ((JObject)jsonRoot["scenarioOne"]).Remove("managementEvents");
        var absentList = JsonUtility.FromJson<ForestSaveData>(jsonRoot.ToString());
        Check(Validate(absentList) == null, "event list absent from the JSON is accepted");
    }

    private static void Rejects()
    {
        string zero = Validate(Save(Ids(0)));
        Check(zero == "management event record 0 has a non-positive ID (0)", "eventId 0 rejected: " + zero);
        string negative = Validate(Save(Ids(1, 2, -3)));
        Check(negative == "management event record 2 has a non-positive ID (-3)", "negative eventId rejected: " + negative);
        string duplicate = Validate(Save(Ids(1, 2, 2)));
        Check(duplicate == "management event ID 2 appears twice (records 1 and 2)", "adjacent duplicate rejected: " + duplicate);
        string distant = Validate(Save(Ids(5, 6, 7, 8, 5)));
        Check(distant == "management event ID 5 appears twice (records 0 and 4)", "non-adjacent duplicate rejected: " + distant);
        string intMin = Validate(Save(Ids(int.MinValue)));
        Check(intMin != null && intMin.Contains("non-positive"), "int.MinValue rejected");
        Check(Validate(Save(Ids(1, 0, 1))) == "management event record 1 has a non-positive ID (0)", "first problem in list order is reported (zero before later duplicate)");
        Check(Validate(Save(Ids(3, 3, 0))) == "management event ID 3 appears twice (records 0 and 1)", "first problem in list order is reported (duplicate before later zero)");
        Check(Validate(Save(Ids(7, 7))) == Validate(Save(Ids(7, 7))), "reason is deterministic");
        foreach (int version in new[] { 1, 12, 14, 15, 19, 21 })
        {
            Check(Validate(Save(Ids(1, 1), version)) != null, "duplicate rejected at save version " + version);
            Check(Validate(Save(Ids(0), version)) != null, "zero rejected at save version " + version);
        }
        // The player-facing wrapper is "Save not loaded: <reason>."; the reason must read as a sentence fragment.
        Check(zero.Length < 80 && !zero.EndsWith("."), "reason is short and suits the 'Save not loaded: <reason>.' wrapper");
    }

    private static void OldRulesUnchanged()
    {
        // Existing rejections keep their exact reason and precedence even when the event IDs are also bad.
        var badCell = Save(new List<ScenarioManagementEvent> { Event(0, 64) });
        Check(Validate(badCell) == "management event cell 64 is outside the stand grid", "cell-index reason still wins over a bad ID");
        var badCellDuplicate = Save(new List<ScenarioManagementEvent> { Event(2, 0), Event(2, -2) });
        Check(Validate(badCellDuplicate) == "management event cell -2 is outside the stand grid", "cell-index reason still wins over a duplicate ID");
        var nullEntry = Save(new List<ScenarioManagementEvent> { Event(1), null });
        Check(Validate(nullEntry) == "a management event record is missing", "null record reason unchanged");
        var badGrowth = Save(Ids(0, 0)); badGrowth.growthModel = 99;
        Check(Validate(badGrowth) == "unknown growth model 99", "unrelated older rejection (growth model) keeps its reason over a bad ID");
        var duplicateTree = Save(Ids(1, 1)); duplicateTree.trees.Add(new TreeSaveData { treeId = "P1", stage = 0 });
        Check(Validate(duplicateTree) == "tree ID P1 appears twice", "unrelated older rejection (tree IDs) keeps its reason over a bad ID");
        Check(Validate(null) == "the file is empty or not a forest save", "null save reason unchanged");
        var noTrees = Save(Ids(1)); noTrees.trees = null;
        Check(Validate(noTrees) == "the save has no tree list", "missing tree list reason unchanged");
        // A save with other, non-event record IDs repeated stays acceptable (the rule is event-ID only).
        var sharedCell = Save(new List<ScenarioManagementEvent> { Event(1, 5), Event(2, 5) });
        Check(Validate(sharedCell) == null, "two events sharing a cell with distinct IDs stay accepted");
    }

    private static void NoRepair()
    {
        foreach (var events in new[] { Ids(0, 4, 4, -1), Ids(3, 1, 2), Ids(5, 5) })
        {
            var save = Save(events);
            string before = JsonUtility.ToJson(save);
            Validate(save);
            Check(JsonUtility.ToJson(save) == before, "validation never repairs, renumbers, dedupes or reorders: ids " + string.Join(",", events.Select(e => e.eventId)));
            Check(save.scenarioOne.nextManagementEventId == JsonUtility.FromJson<ForestSaveData>(before).scenarioOne.nextManagementEventId, "next-ID counter untouched");
        }
    }

    private static void DiskPathParity()
    {
        // TryReadSave parses with JsonUtility.FromJson and then LoadData/CanLoad call Validate on that object.
        foreach (var events in new[] { Ids(0), Ids(-4, 1), Ids(2, 2), Ids(1, 2, 3) })
        {
            var parsedData = Save(events);
            var fromDisk = JsonUtility.FromJson<ForestSaveData>(JsonUtility.ToJson(parsedData));
            Check(Validate(parsedData) == Validate(fromDisk), "disk-parsed and in-memory data reach the same reason: ids " + string.Join(",", events.Select(e => e.eventId)));
        }
        // A record whose eventId field is missing from the file parses as 0 and is rejected, not defaulted to a new ID.
        string json = JsonUtility.ToJson(Save(Ids(1, 2)));
        var jsonRoot = JObject.Parse(json);
        ((JObject)jsonRoot["scenarioOne"]["managementEvents"][1]).Remove("eventId");
        var missingField = JsonUtility.FromJson<ForestSaveData>(jsonRoot.ToString());
        Check(missingField.scenarioOne.managementEvents[1].eventId == 0 && Validate(missingField) != null, "missing eventId parses as 0 and is rejected");
    }

    private static void Fixtures()
    {
        if (string.IsNullOrEmpty(root)) { Check(false, "CCF_REPO_ROOT not set"); return; }
        // Reference Future v1: gzip JSON archive of full-world milestone saves (save v12). Frozen; read-only here.
        string archivePath = Path.Combine(root, "Assets/ForestPrototype/ScenarioOne/Resources/ScenarioOneReferenceFutureV1.bytes");
        string archiveJson;
        using (var compressed = new MemoryStream(File.ReadAllBytes(archivePath)))
        using (var decoder = new GZipStream(compressed, CompressionMode.Decompress))
        using (var text = new MemoryStream())
        {
            decoder.CopyTo(text);
            archiveJson = Encoding.UTF8.GetString(text.ToArray());
        }
        var milestones = (JArray)JObject.Parse(archiveJson)["milestones"];
        Check(milestones.Count == 4, "Reference Future v1 has four milestones");
        foreach (JToken milestone in milestones)
        {
            var world = JsonUtility.FromJson<ForestSaveData>(milestone["world"].ToString());
            int events = world.scenarioOne?.managementEvents?.Count ?? -1;
            Check(events >= 0 && world.scenarioOne.managementEvents.All(e => e.eventId > 0)
                && world.scenarioOne.managementEvents.Select(e => e.eventId).Distinct().Count() == events,
                "Reference Future v1 year " + milestone["year"] + " has " + events + " events with positive unique IDs");
            Check(Validate2(world) == null, "Reference Future v1 year " + milestone["year"] + " (save v" + world.version + ") passes production validation: " + Validate2(world));
        }
        // Accepted storm evidence saves (save v19), including the 49-event salvage history.
        string evidence = Path.Combine(root, "Docs/Verification/StormsWindthrowIntegration/Evidence");
        foreach (string relative in new[] { "StormCoreVerification/resolved_storm.json", "StormRngWrite/world-0.json", "StormRngWrite/world-1.json", "StormSalvageVerification/salvage_resolved.json" })
        {
            var world = JsonUtility.FromJson<ForestSaveData>(File.ReadAllText(Path.Combine(evidence, relative)));
            Check(Validate2(world) == null, "accepted storm evidence save " + relative + " (v" + world.version + ", " + world.scenarioOne.managementEvents.Count + " events) passes production validation: " + Validate2(world));
        }
    }

    private static string Validate2(ForestSaveData world) => ForestSaveValidation.Validate(world, world.trees.Count, 0);

    private static string Between(string text, string start, string end)
    {
        int from = text.IndexOf(start, StringComparison.Ordinal);
        if (from < 0) return "";
        int to = text.IndexOf(end, from + start.Length, StringComparison.Ordinal);
        return to < 0 ? text.Substring(from) : text.Substring(from, to - from);
    }

    private static void SingleSeam()
    {
        string controller = File.ReadAllText(Path.Combine(root, "Assets/ForestPrototype/ForestSaveController.cs"));
        string canLoad = Between(controller, "public bool CanLoad(", "private static bool TryReadSave");
        string loadData = Between(controller, "public bool LoadData(", "First change to the world");
        string findProblem = Between(controller, "private static string FindLoadProblem(", "public bool LoadData(");
        Check(canLoad.Contains("FindLoadProblem("), "CanLoad runs FindLoadProblem");
        Check(loadData.Contains("FindLoadProblem(") && loadData.Contains("return false;"), "LoadData runs FindLoadProblem and refuses before the first world change");
        Check(findProblem.Contains("ForestSaveValidation.Validate("), "FindLoadProblem delegates to ForestSaveValidation.Validate");
        Check(!controller.Contains("eventId") && !controller.Contains("managementEvents"), "ForestSaveController holds no second event-ID rule");
        string saveData = File.ReadAllText(Path.Combine(root, "Assets/ForestPrototype/ForestSaveData.cs"));
        Check(saveData.Contains("public const int CurrentVersion = 23;"), "no save schema/version bump (CurrentVersion still 23)");
        Check(ForestSaveData.CurrentVersion == 23, "compiled CurrentVersion is 23");
    }
}
