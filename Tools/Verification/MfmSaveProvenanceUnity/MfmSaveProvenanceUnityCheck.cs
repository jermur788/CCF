#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// MFM-06 disposable Editor-only gate (save v23 prescription/provenance foundation). Stage with the runner, then remove source and
// .meta. It drives the REAL ForestSaveController (Save / CanLoad / Load / LoadData / CaptureData) in the production Enlarged80
// Rack V2 world. Disk files are written only into the isolated persistentDataPath the runner configures. Nothing here creates a
// prescription through game behaviour (that is MFM-07): prescription state is stamped onto captured save data, exactly as a
// future writer would persist it, and the load path must carry it back unchanged.
public static class MfmSaveProvenanceUnityCheck
{
    public static void Begin()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        EditorApplication.isPlaying = true;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (Environment.GetCommandLineArgs().Any(a => a == "MfmSaveProvenanceUnityCheck.Begin"))
            new GameObject("Disposable MFM-06 save provenance gate").AddComponent<MfmSaveProvenanceUnityCheckRunner>();
    }
}

public sealed class MfmSaveProvenanceUnityCheckRunner : MonoBehaviour
{
    private static readonly string[] Entrances = { "PO0203", "PO1204", "PO2003", "PO2803", "PO3803" };
    private ScenarioOneUiRoot ui;
    private ScenarioOneManager manager;
    private ForestEcologyController ecology;
    private ForestSaveController saves;
    private ForestSaveData original;
    private bool saveControllerWasEnabled;
    private int checks;

    private struct Identity
    {
        public string Json, Hash, Prescriptions, OrderLinks;
        public int Trees, Wood;
    }

    private string SavePath => Path.Combine(Application.persistentDataPath, "forest-save.json");

    private void Check(bool ok, string what, string detail = "")
    {
        if (!ok) { Debug.Log("MFM06_UNITY_CHECK_FAIL " + what + " " + detail); throw new InvalidOperationException(what + " " + detail); }
        checks++;
        Debug.Log("MFM06_UNITY_CHECK " + what + (detail.Length == 0 ? "" : " [" + detail + "]"));
    }

    private IEnumerator Start()
    {
        Exception failure = null;
        IEnumerator run = Run();
        while (true)
        {
            bool more; object current = null;
            try { more = run.MoveNext(); if (more) current = run.Current; }
            catch (Exception error) { failure = error; break; }
            if (!more) break;
            yield return current;
        }
        if (original != null)
        {
            try
            {
                Check(saves.LoadData(original, false), "original world restored in memory");
                saves.enabled = saveControllerWasEnabled;
            }
            catch (Exception error) { failure = failure == null ? error : new AggregateException(failure, error); }
        }
        Debug.Log(failure == null ? "MFM06_UNITY_PASS checks=" + checks : "MFM06_UNITY_CHECK_FAIL final checks=" + checks + " " + failure);
        EditorApplication.Exit(failure == null ? 0 : 1);
    }

    private Identity Capture()
    {
        ForestSaveData data = saves.CaptureData();
        return new Identity
        {
            Json = JsonUtility.ToJson(data),
            Hash = ScenarioReferenceArchive.WorldHash(saves.CaptureData()),
            Trees = FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None).Length,
            Wood = data.wood,
            Prescriptions = JsonUtility.ToJson(new PrescriptionList { next = manager.NextPrescriptionId, items = manager.Prescriptions.ToList() }),
            OrderLinks = string.Join(";", manager.WorkOrders.Select(o => o.workOrderId + ":" + o.prescriptionId + ":" + o.prescriptionCoverageUnitId + ":" + o.prescriptionCropTreeId + ":" + o.prescriptionReason))
                + "|" + string.Join(";", manager.ManagementEvents.Select(e => e.eventId + ":" + e.prescriptionId))
        };
    }

    [Serializable] private sealed class PrescriptionList { public int next; public List<PrescriptionSaveData> items; }

    private bool Same(Identity a, Identity b)
        => a.Json == b.Json && a.Hash == b.Hash && a.Trees == b.Trees && a.Wood == b.Wood && a.Prescriptions == b.Prescriptions && a.OrderLinks == b.OrderLinks;

    private static string Pretty(JObject root) => root.ToString(Formatting.Indented);

    // Removes every v23 field from a saved document and stamps the v22 header: what a v22 writer produced.
    private static string AsV22(string text)
    {
        JObject root = JObject.Parse(text);
        root["version"] = 22;
        if (root["scenarioOne"] is JObject scenario)
        {
            scenario.Remove("nextPrescriptionId");
            scenario.Remove("prescriptions");
            foreach (JObject order in ((JArray)scenario["workOrders"]).OfType<JObject>())
                foreach (string key in new[] { "prescriptionId", "prescriptionCoverageUnitId", "prescriptionCropTreeId", "prescriptionReason" })
                    order.Remove(key);
            foreach (JObject entry in ((JArray)scenario["managementEvents"]).OfType<JObject>())
                entry.Remove("prescriptionId");
        }
        return Pretty(root);
    }

    // Writes a (possibly malformed) document, then proves the load path refuses it BEFORE touching the world.
    private void ExpectRefused(string label, string text, Identity expected, string reasonPart)
    {
        File.WriteAllText(SavePath, text);
        bool canLoad = saves.CanLoad(out string problem);
        Check(!canLoad && problem != null, "CanLoad refuses: " + label, problem);
        if (reasonPart != null)
            Check(problem.IndexOf(reasonPart, StringComparison.OrdinalIgnoreCase) >= 0, "refusal names the cause: " + label, problem);
        Check(Same(Capture(), expected), "CanLoad changed nothing: " + label);
        saves.Load();
        Check(Same(Capture(), expected), "Load refused and changed nothing (world, hash, trees, wood, prescriptions, links): " + label);
    }

    private IEnumerator Run()
    {
        float deadline = Time.realtimeSinceStartup + 30f;
        do
        {
            ui = FindFirstObjectByType<ScenarioOneUiRoot>();
            manager = ui != null ? ui.Manager : null;
            ecology = FindFirstObjectByType<ForestEcologyController>();
            saves = FindFirstObjectByType<ForestSaveController>();
            if (ui != null && manager != null && ecology != null && saves != null) break;
            yield return null;
        } while (Time.realtimeSinceStartup < deadline);
        Check(ui != null && manager != null && ecology != null && saves != null, "scene components ready");
        for (int i = 0; i < 20; i++) yield return null;
        string config = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME");
        Check(!string.IsNullOrEmpty(config) && Application.persistentDataPath.StartsWith(config.TrimEnd('/') + "/", StringComparison.Ordinal),
            "isolated persistentDataPath", Application.persistentDataPath);
        Check(!File.Exists(SavePath), "isolated save slot starts empty");
        saveControllerWasEnabled = saves.enabled;
        saves.enabled = false; // no F5/F9 keyboard handler during this run
        original = saves.CaptureData();
        Check(ForestSaveData.CurrentVersion == 23 && original.version == 23, "candidate writes save version 23");

        // ---- a brand-new world has no prescription state ---------------------------------------------------------
        Check(manager.RackPlanningV2Active && ecology.StandGeometryModelVersion == StandGeometryModel.Enlarged80, "production Enlarged80 Rack V2 world");
        Check(manager.Prescriptions.Count == 0 && manager.NextPrescriptionId == 1, "new world: no prescription state");
        Check(original.scenarioOne.prescriptions.Count == 0 && original.scenarioOne.nextPrescriptionId == 1
            && original.scenarioOne.workOrders.All(o => o.prescriptionId == 0) && original.scenarioOne.managementEvents.All(e => e.prescriptionId == 0), "new world: captured save carries the empty prescription defaults");
        string archiveHash0 = ScenarioReferenceArchive.WorldHash(saves.CaptureData());

        // ---- build a real ordinary world with marks and orders (existing state that must never gain provenance) ---------
        float half = StandGeometryModel.StandSizeMeters(ecology.StandGeometryModelVersion) * 0.5f;
        if (!manager.RackPlanConfirmed)
        {
            for (int i = 0; i < Entrances.Length; i++)
            {
                ForestTree tree = FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None).SingleOrDefault(t => t.TreeId == Entrances[i] && t.IsLiving);
                Check(tree != null, "live fixture entrance exists", Entrances[i]);
                var added = manager.AddRack(new RackPoint(ScenarioOneHeadlandGeometry.HeadlandX, tree.transform.position.z), new RackPoint(half, tree.transform.position.z));
                Check(added.Succeeded && added.RacksAdded == 1, "fixture rack added in memory", added.Message);
            }
            var confirmed = manager.ConfirmRackPlan();
            Check(confirmed.Succeeded, "fixture plan confirmed in memory (pending access orders only)", confirmed.Message);
        }
        Check(manager.RackPlanConfirmed && manager.ConfirmedRacks.Count == 5, "five confirmed racks available");
        Check(manager.WorkOrders.Count > 0 && manager.WorkOrders.Any(o => o.IsOpen), "ordinary open work orders exist");
        yield return null;
        Identity ordinary = Capture();
        ForestSaveData ordinaryData = saves.CaptureData();
        Check(ordinaryData.scenarioOne.prescriptions.Count == 0 && ordinaryData.scenarioOne.workOrders.All(o => o.prescriptionId == 0), "ordinary world: orders carry no prescription provenance");

        // ---- pre-v23 (v22-shaped) file of the ordinary world loads unchanged, fabricating nothing -------------------------
        saves.Save();
        Check(File.Exists(SavePath), "Save() wrote the isolated save slot");
        string v23Ordinary = File.ReadAllText(SavePath);
        Check(JObject.Parse(v23Ordinary)["version"].Value<int>() == 23 && v23Ordinary.Contains("nextPrescriptionId"), "Save() writes the v23 prescription fields explicitly");
        string v22Text = AsV22(v23Ordinary);
        File.WriteAllText(SavePath, v22Text);
        Check(!v22Text.Contains("rescription"), "v22 fixture contains no prescription text");
        Check(saves.CanLoad(out string v22Problem), "CanLoad accepts the v22 file", v22Problem);
        saves.Load();
        yield return null;
        Identity afterV22 = Capture();
        Check(afterV22.Json == ordinary.Json, "v22 load: world JSON identical to the ordinary world (nothing fabricated or lost)");
        Check(afterV22.Hash == ordinary.Hash && afterV22.Hash != null, "v22 load: world-identity hash identical", afterV22.Hash);
        Check(manager.Prescriptions.Count == 0 && manager.NextPrescriptionId == 1 && manager.WorkOrders.All(o => o.prescriptionId == 0)
            && manager.ManagementEvents.All(e => e.prescriptionId == 0), "v22 load: no prescription, order or event provenance exists");
        // Older headers are accepted the same way.
        foreach (int old in new[] { 21, 20 })
        {
            JObject older = JObject.Parse(v22Text);
            older["version"] = old;
            // A pre-v22 document has no objective history either.
            ((JObject)older["scenarioOne"]).Remove("objectiveAchievements");
            File.WriteAllText(SavePath, Pretty(older));
            bool accepted = saves.CanLoad(out string olderProblem);
            Debug.Log("MFM06_UNITY_NOTE v" + old + " header with otherwise-v22 body: CanLoad=" + accepted + " " + olderProblem);
        }

        // ---- v23 prescription state: stamp onto real captured data, load through the real path -----------------------------
        ForestSaveData crafted = saves.CaptureData();
        int year = crafted.ecologicalYear;
        ScenarioOneWorkOrder target = crafted.scenarioOne.workOrders.First(o => o.IsOpen);
        ScenarioManagementEvent targetEvent = crafted.scenarioOne.managementEvents.FirstOrDefault(e => e.workOrderId == target.workOrderId)
            ?? crafted.scenarioOne.managementEvents.FirstOrDefault();
        crafted.scenarioOne.nextPrescriptionId = 3;
        crafted.scenarioOne.prescriptions = new List<PrescriptionSaveData>
        {
            new PrescriptionSaveData
            {
                prescriptionId = 1, ruleVersion = ScenarioOnePrescriptionCalibration.RuleVersion, calibrationVersion = "MFM03-CAL-1",
                sourceRackId = crafted.scenarioOne.racks[0].rackId, sourceYear = year, sourceAnchorX = 4.5f, sourceAnchorZ = -2.25f,
                sampleCropTreeIds = new List<string> { "PO0100", "PO0200" }, sampleRemovalIds = new List<string> { "PO0301" },
                coverageDecisions = new List<PrescriptionCoverageDecisionSaveData>
                { new PrescriptionCoverageDecisionSaveData { unitId = "R2W", choice = PrescriptionCoverageChoice.LeftManual } },
                approvalState = PrescriptionApprovalState.Approved, approvalYear = year
            },
            new PrescriptionSaveData
            {
                prescriptionId = 2, ruleVersion = ScenarioOnePrescriptionCalibration.RuleVersion, calibrationVersion = "MFM03-CAL-1",
                sourceRackId = crafted.scenarioOne.racks[0].rackId, sourceYear = year, sourceAnchorX = 0f, sourceAnchorZ = 0f,
                approvalState = PrescriptionApprovalState.Accepted, approvalYear = -1
            }
        };
        target.prescriptionId = 1;
        target.prescriptionCoverageUnitId = "R1C";
        target.prescriptionCropTreeId = "PO0100";
        target.prescriptionReason = (int)ScenarioOnePrescription.ProposalReason.CompetitorHighShareAroundConfirmedCropTree;
        if (targetEvent != null) targetEvent.prescriptionId = 1;
        Check(targetEvent != null && crafted.scenarioOne.managementEvents.Count >= 2, "existing management events are available to carry the history link", "order " + target.workOrderId + " event " + (targetEvent != null ? targetEvent.eventId.ToString() : "none") + " of " + crafted.scenarioOne.managementEvents.Count);
        string craftedJson = JsonUtility.ToJson(crafted);
        string craftedHash = ScenarioReferenceArchive.WorldHash(JsonUtility.FromJson<ForestSaveData>(craftedJson));
        Check(craftedHash != archiveHash0 && craftedHash != ordinary.Hash, "prescription state changes the world identity hash", craftedHash);
        Check(saves.LoadData(JsonUtility.FromJson<ForestSaveData>(craftedJson), false), "LoadData accepts the v23 prescription world");
        yield return null;
        Identity loaded = Capture();
        Check(manager.Prescriptions.Select(p => p.prescriptionId).SequenceEqual(new[] { 1, 2 }) && manager.NextPrescriptionId == 3, "manager holds both prescriptions and the allocator");
        PrescriptionSaveData first = manager.Prescriptions[0];
        Check(first.ruleVersion == ScenarioOnePrescriptionCalibration.RuleVersion && first.calibrationVersion == "MFM03-CAL-1" && first.sourceAnchorX == 4.5f
            && first.coverageDecisions.Count == 1 && first.coverageDecisions[0].unitId == "R2W" && first.approvalYear == year && first.sampleCropTreeIds.Count == 2, "restored prescription fields are intact");
        ScenarioOneWorkOrder live = manager.WorkOrders.First(o => o.workOrderId == target.workOrderId);
        Check(live.prescriptionId == 1 && live.prescriptionCoverageUnitId == "R1C" && live.prescriptionCropTreeId == "PO0100", "order provenance restored");
        Check(manager.WorkOrders.Where(o => o.workOrderId != target.workOrderId).All(o => o.prescriptionId == 0), "every other order stays ordinary");
        Check(loaded.Json == craftedJson, "in-memory round trip: captured JSON is byte-identical to the crafted v23 world");
        Check(loaded.Hash == craftedHash, "in-memory round trip: world-identity hash is stable");

        // ---- disk round trip, repeated: stable ids -------------------------------------------------------------------------
        string firstDisk = null;
        for (int cycle = 1; cycle <= 3; cycle++)
        {
            saves.Save();
            string disk = File.ReadAllText(SavePath);
            if (firstDisk == null) firstDisk = disk;
            Check(disk == firstDisk, "disk save #" + cycle + " is byte-identical to the first");
            Check(saves.CanLoad(out string cycleProblem), "CanLoad accepts the saved v23 file #" + cycle, cycleProblem);
            saves.Load();
            yield return null;
            Check(Same(Capture(), loaded), "Load #" + cycle + " reproduces the identical world, hash, ids and links");
        }
        Check(manager.Prescriptions.Select(p => p.prescriptionId).SequenceEqual(new[] { 1, 2 }) && manager.NextPrescriptionId == 3, "prescription ids stable across repeated disk save/load");

        // ---- invalid v23 data refuses before any world mutation ---------------------------------------------------------------
        Identity before = Capture();
        int wood = original.wood;
        JObject Mut(Action<JObject> edit)
        {
            JObject root = JObject.Parse(firstDisk);
            root["wood"] = root["wood"].Value<int>() + 7; // any partial application would show as wood the world never had
            edit((JObject)root["scenarioOne"]);
            return root;
        }
        JArray Rx(JObject s) => (JArray)s["prescriptions"];
        JObject Order(JObject s) => ((JArray)s["workOrders"]).OfType<JObject>().First(o => o["prescriptionId"].Value<int>() == 1);
        JObject Plain(JObject s) => ((JArray)s["workOrders"]).OfType<JObject>().First(o => o["prescriptionId"].Value<int>() == 0);
        var cases = new List<(string label, string text, string part)>
        {
            ("duplicate prescription id", Pretty(Mut(s => Rx(s)[1]["prescriptionId"] = 1)), "twice"),
            ("prescription id 0", Pretty(Mut(s => Rx(s)[0]["prescriptionId"] = 0)), "invalid prescription id"),
            ("allocator at an id in use", Pretty(Mut(s => s["nextPrescriptionId"] = 2)), "allocator"),
            ("blank rule version", Pretty(Mut(s => Rx(s)[0]["ruleVersion"] = "")), "rule version"),
            ("rule version with a space", Pretty(Mut(s => Rx(s)[0]["ruleVersion"] = "MFM V1")), "rule version"),
            ("source rack never allocated", Pretty(Mut(s => Rx(s)[0]["sourceRackId"] = 99)), "source rack"),
            ("source year in the future", Pretty(Mut(s => Rx(s)[0]["sourceYear"] = 500)), "source year"),
            ("approval year impossible", Pretty(Mut(s => Rx(s)[0]["approvalYear"] = 500)), "approval year"),
            ("unapproved prescription with a year", Pretty(Mut(s => Rx(s)[1]["approvalYear"] = year)), "approval year"),
            ("unknown approval state", Pretty(Mut(s => Rx(s)[0]["approvalState"] = 9)), "approval state"),
            ("unknown coverage choice", Pretty(Mut(s => Rx(s)[0]["coverageDecisions"][0]["choice"] = 9)), "coverage choice"),
            ("order names an unknown prescription", Pretty(Mut(s => Order(s)["prescriptionId"] = 9)), "unknown prescription"),
            ("order from an unapproved prescription", Pretty(Mut(s => Order(s)["prescriptionId"] = 2)), "not approved"),
            ("order with an unknown reason", Pretty(Mut(s => Order(s)["prescriptionReason"] = 77)), "reason"),
            ("ordinary order carrying provenance", Pretty(Mut(s => Plain(s)["prescriptionCoverageUnitId"] = "R1C")), "no prescription"),
            ("event names an unknown prescription", Pretty(Mut(s => ((JArray)s["managementEvents"]).OfType<JObject>().First(e => e["prescriptionId"].Value<int>() == 0)["prescriptionId"] = 7)), "unknown prescription"),
            ("prescriptions array missing", Pretty(Mut(s => s.Remove("prescriptions"))), "explicit prescription fields"),
            ("allocator missing", Pretty(Mut(s => s.Remove("nextPrescriptionId"))), "explicit prescription fields"),
            ("prescription id typed as a string", Pretty(Mut(s => Rx(s)[0]["prescriptionId"] = "1")), "required fields"),
            ("order provenance missing", Pretty(Mut(s => Order(s).Remove("prescriptionCoverageUnitId"))), "prescription link"),
            ("prescription state inside a v22 header", Pretty(Mut(s => { })).Replace("\"version\": 23", "\"version\": 22"), "older than version 23"),
            ("truncated document", firstDisk.Substring(0, firstDisk.Length / 2), null),
            ("duplicate key", firstDisk.Replace("\"nextPrescriptionId\": 3,", "\"nextPrescriptionId\": 3,\n    \"nextPrescriptionId\": 3,").Replace("\"wood\": " + wood, "\"wood\": " + (wood + 7)), null),
        };
        foreach (var c in cases)
        {
            ExpectRefused(c.label, c.text, before, c.part);
            yield return null;
        }
        // The in-memory entry (LoadData) runs the same validation before it touches the scene. Corrupt typed data directly.
        var typedCases = new List<(string label, Action<ForestSaveData> corrupt)>
        {
            ("typed: duplicate prescription id", d => d.scenarioOne.prescriptions[1].prescriptionId = 1),
            ("typed: allocator below an id in use", d => d.scenarioOne.nextPrescriptionId = 2),
            ("typed: unknown rule version text", d => d.scenarioOne.prescriptions[0].ruleVersion = "bad version!"),
            ("typed: source rack never allocated", d => d.scenarioOne.prescriptions[0].sourceRackId = 99),
            ("typed: order from an unknown prescription", d => d.scenarioOne.workOrders.First(o => o.prescriptionId == 1).prescriptionId = 9),
            ("typed: provenance on an ordinary order", d => d.scenarioOne.workOrders.First(o => o.prescriptionId == 0).prescriptionReason = 1),
            ("typed: prescription state in a v22 save", d => d.version = 22),
        };
        foreach (var c in typedCases)
        {
            ForestSaveData typed = JsonUtility.FromJson<ForestSaveData>(craftedJson);
            typed.wood += 7;
            c.corrupt(typed);
            Check(!saves.LoadData(typed, false), "LoadData refuses: " + c.label);
            Check(Same(Capture(), before), "LoadData refusal changed nothing: " + c.label);
            yield return null;
        }

        // A good file still loads after all the refusals: nothing was left half-applied.
        File.WriteAllText(SavePath, firstDisk);
        Check(saves.CanLoad(out string goodProblem), "the unmodified v23 file still loads after every refusal", goodProblem);
        saves.Load();
        Check(Same(Capture(), loaded), "good file after refusals reproduces the identical world");

        // ---- return to the starting world ----------------------------------------------------------------------------------
        Check(saves.LoadData(original, false), "original world restored");
        original = null;
        saves.enabled = saveControllerWasEnabled;
        Check(manager.Prescriptions.Count == 0 && manager.NextPrescriptionId == 1, "starting world has no prescription state again");
    }
}
#endif
