// MFM-06 offline check: compiles the PRODUCTION v23 schema, validators, migration, hash path (extracted verbatim from
// ScenarioReferenceArchive.cs) and ScenarioOneManager.Prescriptions.cs and runs them on stock .NET. It proves the pure rules;
// it does not prove Unity import, the scene-level load order or UI (the exact-candidate Unity gate does). Manager call sites
// that this build cannot compile are audited as source text at the end.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public static class PrescriptionSaveOfflineCheck
{
    private static int checks;
    private static void Check(bool condition, string label)
    {
        if (!condition) throw new Exception("FAIL " + label);
        checks++;
        Console.WriteLine("PASS " + label);
    }

    private static PrescriptionSaveData Rx(int id, bool approved = true, int rack = 1, int year = 10, int approvalYear = 12)
        => new PrescriptionSaveData
        {
            prescriptionId = id, ruleVersion = "MFM-V1-RULE-1", calibrationVersion = "MFM-V1-CAL-1",
            sourceRackId = rack, sourceYear = year, sourceAnchorX = 12.5f, sourceAnchorZ = -3.25f,
            sampleCropTreeIds = new List<string> { "P0200", "P0100" }, sampleRemovalIds = new List<string> { "P0301" },
            coverageDecisions = new List<PrescriptionCoverageDecisionSaveData>
            {
                new PrescriptionCoverageDecisionSaveData { unitId = "R2W", choice = PrescriptionCoverageChoice.LeftManual },
                new PrescriptionCoverageDecisionSaveData { unitId = "R1C", choice = PrescriptionCoverageChoice.AcceptedCovered }
            },
            approvalState = approved ? PrescriptionApprovalState.Approved : PrescriptionApprovalState.Accepted,
            approvalYear = approved ? approvalYear : -1
        };

    // A v23 Enlarged80 Rack V2 world with two prescriptions, a prescription-generated order, an ordinary order, marks, history.
    private static ForestSaveData World()
    {
        var data = new ForestSaveData
        {
            ecologicalYear = 20, standGeometryModel = 1, regenerationModel = 0, growthModel = 0,
            markedTreeIds = new List<string> { "P0900" }, cropTreeIds = new List<string> { "P0100", "P0200" },
            trees = new List<TreeSaveData> { new TreeSaveData { treeId = "P0100" } },
            scenarioOne = new ScenarioOneSaveData
            {
                initialized = true, rackPlanningModel = RackPlanningModel.RackPlanningFieldMarkingV2, rackPlanConfirmed = false, nextRackId = 3,
                racks = new List<RackSaveData> { new RackSaveData { rackId = 1, entranceX = 0f, entranceZ = 0f, farX = 20f, farZ = 0f },
                                                 new RackSaveData { rackId = 2, entranceX = 0f, entranceZ = 10f, farX = 20f, farZ = 10f } },
                workOrders = new List<ScenarioOneWorkOrder>
                {
                    new ScenarioOneWorkOrder { workOrderId = 1, type = ScenarioWorkType.FellTree, targetTreeId = "P0900", createdYear = 3 },
                    new ScenarioOneWorkOrder { workOrderId = 2, type = ScenarioWorkType.FellTree, targetTreeId = "P0302", createdYear = 12,
                        prescriptionId = 1, prescriptionCoverageUnitId = "R1C", prescriptionCropTreeId = "P0100",
                        prescriptionReason = PrescriptionReasonCode.CompetitorHighShareAroundConfirmedCropTree }
                },
                managementEvents = new List<ScenarioManagementEvent>
                {
                    new ScenarioManagementEvent { eventId = 1, year = 3, eventType = ScenarioManagementEventType.OrderCreated, workOrderId = 1 },
                    new ScenarioManagementEvent { eventId = 2, year = 12, eventType = ScenarioManagementEventType.OrderCreated, workOrderId = 2, prescriptionId = 1 }
                },
                objectiveAchievements = new List<ScenarioObjectiveAchievement> { new ScenarioObjectiveAchievement { objectiveId = "minimum-year", achievedYear = 5, targetValueAtAchievement = 25f } },
                nextPrescriptionId = 3,
                prescriptions = new List<PrescriptionSaveData> { Rx(1), Rx(2, approved: false) }
            }
        };
        return data;
    }

    private static string Json(ForestSaveData d) => JsonUtility.ToJson(d);
    private static ForestSaveData Parse(string json) => JsonUtility.FromJson<ForestSaveData>(json);

    // The disk path: JSON-presence check, then the typed validators (what LoadData and CanLoad both run before any mutation).
    private static string Refusal(string json, ForestSaveData typed)
        => ForestSaveValidation.ValidatePrescriptionsJson(json, typed) ?? ForestSaveValidation.Validate(typed, 1, 256);
    private static string Refusal(ForestSaveData typed) => Refusal(Json(typed), typed);

    // Rewrites a current-version JSON as a v22 writer produced it: no prescription fields anywhere, v22 header.
    private static string AsV22(string json)
    {
        json = json.Replace("\"version\":" + ForestSaveData.CurrentVersion + ",", "\"version\":22,");
        json = Regex.Replace(json, @",""nextPrescriptionId"":\d+,""prescriptions"":\[.*\](?=\}\}$)", "", RegexOptions.Singleline);
        json = json.Replace(",\"nextPrescriptionId\":1,\"prescriptions\":[]", "");
        json = Regex.Replace(json, @",""prescriptionId"":\d+,""prescriptionCoverageUnitId"":""[^""]*"",""prescriptionCropTreeId"":""[^""]*"",""prescriptionReason"":\d+", "");
        json = Regex.Replace(json, @",""prescriptionId"":\d+\}", "}");
        return json;
    }

    public static int Main(string[] args)
    {
        string root = Environment.GetEnvironmentVariable("CCF_REPO_ROOT");
        string baseline = args.Length > 0 ? args[0] : null;

        // ---- 1. schema --------------------------------------------------------------------------------------------
        Check(ForestSaveData.CurrentVersion == 24, "schema: CurrentVersion is 24 (D-066 single teaching latch after MFM v23)");
        var fresh = new ForestSaveData();
        Check(fresh.version == 24 && fresh.scenarioOne == null, "schema: a new ForestSaveData stamps v24");
        var blank = new ScenarioOneSaveData();
        Check(blank.nextPrescriptionId == 1 && blank.prescriptions != null && blank.prescriptions.Count == 0, "schema: defaults are the empty 'no prescription' state");
        Check(new ScenarioOneWorkOrder().prescriptionId == 0 && new ScenarioOneWorkOrder().prescriptionCoverageUnitId == "" && new ScenarioManagementEvent().prescriptionId == 0,
            "schema: orders and events default to no prescription");
        Check(PrescriptionApprovalState.Accepted == 1 && PrescriptionApprovalState.Approved == 2 && PrescriptionCoverageChoice.AcceptedCovered == 1 && PrescriptionCoverageChoice.LeftManual == 2,
            "schema: stored enum numbers are the pinned values");

        var proposalReasons = Enum.GetValues(typeof(ScenarioOnePrescription.ProposalReason)).Cast<int>().Select(v => v + 1).ToList();
        Check(proposalReasons.All(PrescriptionReasonCode.IsKnown) && Enumerable.Range(-2, 200).Count(PrescriptionReasonCode.IsKnown) == proposalReasons.Count
            && !PrescriptionReasonCode.IsKnown(PrescriptionReasonCode.None),
            "schema: the stored reason codes are exactly the ProposalReason vocabulary (kept in step with the core)");

        // ---- 2. v23 round trip of every new persisted field ---------------------------------------------------------
        ForestSaveData world = World();
        string json = Json(world);
        ForestSaveData back = Parse(json);
        ScenarioOneSaveData s = back.scenarioOne;
        Check(back.version == 24 && s.nextPrescriptionId == 3 && s.prescriptions.Count == 2, "round trip: version, allocator and both prescriptions");
        PrescriptionSaveData r1 = s.prescriptions[0];
        Check(r1.prescriptionId == 1 && r1.ruleVersion == "MFM-V1-RULE-1" && r1.calibrationVersion == "MFM-V1-CAL-1", "round trip: id, rule version and calibration version");
        Check(r1.sourceRackId == 1 && r1.sourceYear == 10 && r1.sourceAnchorX == 12.5f && r1.sourceAnchorZ == -3.25f, "round trip: source rack, year and anchor");
        Check(r1.sampleCropTreeIds.SequenceEqual(new[] { "P0200", "P0100" }) && r1.sampleRemovalIds.SequenceEqual(new[] { "P0301" }), "round trip: derivation inputs (lists keep their order)");
        Check(r1.coverageDecisions.Count == 2 && r1.coverageDecisions[0].unitId == "R2W" && r1.coverageDecisions[0].choice == PrescriptionCoverageChoice.LeftManual
            && r1.coverageDecisions[1].unitId == "R1C" && r1.coverageDecisions[1].choice == PrescriptionCoverageChoice.AcceptedCovered, "round trip: accepted coverage decisions");
        Check(r1.approvalState == PrescriptionApprovalState.Approved && r1.approvalYear == 12 && s.prescriptions[1].approvalState == PrescriptionApprovalState.Accepted && s.prescriptions[1].approvalYear == -1,
            "round trip: approval state and year");
        ScenarioOneWorkOrder generated = s.workOrders[1], manual = s.workOrders[0];
        Check(generated.prescriptionId == 1 && generated.prescriptionCoverageUnitId == "R1C" && generated.prescriptionCropTreeId == "P0100"
            && generated.prescriptionReason == PrescriptionReasonCode.CompetitorHighShareAroundConfirmedCropTree, "round trip: work-order provenance");
        Check(manual.prescriptionId == 0 && manual.prescriptionCoverageUnitId == "", "round trip: the ordinary order stays ordinary");
        Check(s.managementEvents[1].prescriptionId == 1 && s.managementEvents[0].prescriptionId == 0, "round trip: history linkage on the management event");
        Check(s.objectiveAchievements.Count == 1 && s.objectiveAchievements[0].achievedYear == 5, "round trip: S1-C v22 objective history is carried unchanged");
        Check(Json(back) == json, "round trip: serialized bytes identical after load and re-save");
        Check(Refusal(json, back) == null, "round trip: the valid v23 world passes the JSON and typed validators -> " + Refusal(json, back));
        Check(json.IndexOf("\"objectiveAchievements\":", StringComparison.Ordinal) < json.IndexOf("\"nextPrescriptionId\":", StringComparison.Ordinal),
            "layout: the v23 run is declared after the v22 field");
        Check(json.EndsWith("]}}") || json.Contains("\"prescriptions\":[{"), "layout: prescriptions serialize inside scenarioOne");

        // ---- 3. stable ids through repeated save/load (manager restore path) ----------------------------------------
        var manager = new ScenarioOneManager();
        string last = null;
        ScenarioOneSaveData cycle = Parse(json).scenarioOne;
        for (int i = 0; i < 4; i++)
        {
            manager.Restore(cycle, 23);
            cycle = manager.Capture();
            string now = JsonUtility.ToJson(cycle);
            Check(last == null || now == last, $"stable ids: capture after load #{i + 1} is identical to the previous one");
            last = now;
        }
        Check(manager.Prescriptions.Select(p => p.prescriptionId).SequenceEqual(new[] { 1, 2 }) && manager.NextPrescriptionId == 3, "stable ids: ids and allocator survive four save/load cycles");
        ScenarioOneSaveData lowAllocator = Parse(json).scenarioOne; lowAllocator.nextPrescriptionId = 1;
        manager.Restore(lowAllocator, 23);
        Check(manager.NextPrescriptionId == 3, "stable ids: the allocator never moves back onto an id in use");
        ScenarioOneSaveData highAllocator = Parse(json).scenarioOne; highAllocator.nextPrescriptionId = 50;
        manager.Restore(highAllocator, 23);
        Check(manager.NextPrescriptionId == 50, "stable ids: ids are not reused (a higher allocator is kept)");
        var captured = manager.Capture(); captured.prescriptions[0].ruleVersion = "TAMPERED";
        Check(manager.Prescriptions[0].ruleVersion == "MFM-V1-RULE-1", "isolation: capture hands out copies, not live state");

        // ---- 4. absent fields mean no prescription state; nothing is fabricated ---------------------------------------
        string v22Json = AsV22(Json(World()));
        Check(!v22Json.Contains("rescription"), "v22 shape: the fixture contains no prescription text at all");
        ForestSaveData v22 = Parse(v22Json);
        Check(v22.version == 22 && v22.scenarioOne.prescriptions.Count == 0 && v22.scenarioOne.nextPrescriptionId == 1, "v22 save: missing fields load as no prescription state");
        Check(Refusal(v22Json, v22) == null, "v22 save: validates (JSON and typed)");
        Check(v22.scenarioOne.markedTreeIdsPlaceholder() && v22.markedTreeIds.Count == 1 && v22.cropTreeIds.Count == 2, "v22 save: existing marks and Crop Trees are ordinary existing state");
        manager.Restore(v22.scenarioOne, 22);
        Check(manager.Prescriptions.Count == 0 && manager.NextPrescriptionId == 1, "v22 restore: no prescription state is created");
        Check(v22.scenarioOne.workOrders.All(o => o.prescriptionId == 0 && o.prescriptionCoverageUnitId == "" && o.prescriptionCropTreeId == "" && o.prescriptionReason == 0)
            && v22.scenarioOne.managementEvents.All(e => e.prescriptionId == 0), "v22 restore: existing marks/orders/events acquire no prescription id or provenance");
        Check(v22.scenarioOne.objectiveAchievements.Count == 1 && v22.scenarioOne.objectiveAchievements[0].achievedYear == 5 && v22.scenarioOne.objectiveAchievements[0].targetValueAtAchievement == 25f,
            "v22 save: S1-C objective history survives the upgrade unchanged");
        // A pre-v23 save that carries state in memory (a corrupted writer) is normalised away AND refused by the validator.
        var dirty = World(); dirty.version = 22;
        Check(Refusal(dirty) != null && Refusal(dirty).Contains("older than version 23"), "v22 claiming prescription state: refused");
        ScenarioOneSaveMigration.NormalizePrescriptions(dirty.scenarioOne, 22);
        Check(dirty.scenarioOne.prescriptions.Count == 0 && dirty.scenarioOne.nextPrescriptionId == 1 && dirty.scenarioOne.workOrders.All(o => o.prescriptionId == 0 && o.prescriptionReason == 0)
            && dirty.scenarioOne.managementEvents.All(e => e.prescriptionId == 0), "migration: a pre-v23 save is normalised to no prescription state");
        var keepV23 = World(); ScenarioOneSaveMigration.NormalizePrescriptions(keepV23.scenarioOne, 23);
        Check(keepV23.scenarioOne.prescriptions.Count == 2, "migration: a v23 save keeps its prescriptions");
        ScenarioOneSaveMigration.NormalizePrescriptions(null, 10);
        Check(true, "migration: null scenario data is tolerated");
        foreach (int old in new[] { 21, 20, 15, 12, 8 })
        {
            ForestSaveData o = Parse(Regex.Replace(v22Json, "\"version\":22,", "\"version\":" + old + ","));
            Check(ForestSaveValidation.ValidatePrescriptions(o) == null && ForestSaveValidation.ValidatePrescriptionsJson(Json(o), o) == null, $"v{old} save: prescription validators pass with no field");
        }
        Check(ForestSaveValidation.ValidatePrescriptions(new ForestSaveData { version = 8 }) == null, "v8 save: no scenarioOne means nothing to validate");
        // Legacy40 (no Rack V2) and pre-feature Enlarged80 worlds with no prescription state load clean at v23 too.
        var legacy40 = new ForestSaveData { ecologicalYear = 10, trees = new List<TreeSaveData> { new TreeSaveData { treeId = "P1" } }, scenarioOne = new ScenarioOneSaveData { initialized = true } };
        Check(Refusal(legacy40) == null, "v23 Legacy40 world with no prescription state: valid -> " + Refusal(legacy40));
        var enlarged = World(); enlarged.scenarioOne.prescriptions.Clear(); enlarged.scenarioOne.nextPrescriptionId = 1;
        enlarged.scenarioOne.workOrders[1].prescriptionId = 0; enlarged.scenarioOne.workOrders[1].prescriptionCoverageUnitId = ""; enlarged.scenarioOne.workOrders[1].prescriptionCropTreeId = ""; enlarged.scenarioOne.workOrders[1].prescriptionReason = 0;
        enlarged.scenarioOne.managementEvents[1].prescriptionId = 0;
        Check(Refusal(enlarged) == null, "v23 Enlarged80 world with no prescription state: valid");

        // ---- 5. refusal and no-mutation (every refusal is pure: it runs before any state is applied) -------------------
        void Refuses(string label, Action<ForestSaveData> corrupt, string expectedPart)
        {
            ForestSaveData data = World();
            corrupt(data);
            string before = Json(data);
            string reason = Refusal(data);
            Check(reason != null && reason.IndexOf(expectedPart, StringComparison.OrdinalIgnoreCase) >= 0, $"refused: {label} -> {reason}");
            Check(Json(data) == before, $"no mutation: {label}");
        }
        Refuses("prescription id 0", d => d.scenarioOne.prescriptions[0].prescriptionId = 0, "invalid prescription id");
        Refuses("negative prescription id", d => d.scenarioOne.prescriptions[0].prescriptionId = -4, "invalid prescription id");
        Refuses("duplicate prescription id", d => d.scenarioOne.prescriptions[1].prescriptionId = 1, "twice");
        Refuses("allocator below an id in use", d => d.scenarioOne.nextPrescriptionId = 2, "allocator");
        Refuses("allocator zero", d => d.scenarioOne.nextPrescriptionId = 0, "allocator");
        Refuses("null record", d => d.scenarioOne.prescriptions[1] = null, "missing");
        Refuses("empty rule version", d => d.scenarioOne.prescriptions[0].ruleVersion = "", "rule version");
        Refuses("rule version with a space", d => d.scenarioOne.prescriptions[0].ruleVersion = "MFM V1", "rule version");
        Refuses("rule version too long", d => d.scenarioOne.prescriptions[0].ruleVersion = new string('A', 65), "rule version");
        Refuses("empty calibration version", d => d.scenarioOne.prescriptions[0].calibrationVersion = "", "calibration version");
        Refuses("source rack 0", d => d.scenarioOne.prescriptions[0].sourceRackId = 0, "source rack");
        Refuses("source rack never allocated", d => d.scenarioOne.prescriptions[0].sourceRackId = 3, "source rack");
        Refuses("prescription in a legacy-rack world", d => { d.scenarioOne.rackPlanningModel = RackPlanningModel.Legacy; d.scenarioOne.racks.Clear(); d.scenarioOne.nextRackId = 1; }, "Rack Planning V2");
        Refuses("source year in the future", d => d.scenarioOne.prescriptions[0].sourceYear = 21, "source year");
        Refuses("negative source year", d => d.scenarioOne.prescriptions[0].sourceYear = -1, "source year");
        // The offline JSON shim cannot write NaN/Infinity, so these two use the typed validator directly.
        foreach (float bad in new[] { float.NaN, float.PositiveInfinity })
        {
            ForestSaveData nonFinite = World(); nonFinite.scenarioOne.prescriptions[0].sourceAnchorX = bad;
            string reason = ForestSaveValidation.ValidatePrescriptions(nonFinite);
            Check(reason != null && reason.Contains("anchor"), $"refused: non-finite anchor {bad} -> {reason}");
        }
        Refuses("null crop-tree list", d => d.scenarioOne.prescriptions[0].sampleCropTreeIds = null, "missing");
        Refuses("blank sample tree id", d => d.scenarioOne.prescriptions[0].sampleCropTreeIds[0] = "", "invalid or repeated");
        Refuses("padded sample tree id", d => d.scenarioOne.prescriptions[0].sampleRemovalIds[0] = " P0301", "invalid or repeated");
        Refuses("repeated sample tree id", d => d.scenarioOne.prescriptions[0].sampleCropTreeIds[1] = "P0200", "invalid or repeated");
        Refuses("control character in an id", d => d.scenarioOne.prescriptions[0].sampleRemovalIds[0] = "P03\n01", "invalid or repeated");
        Refuses("null decisions", d => d.scenarioOne.prescriptions[0].coverageDecisions = null, "missing");
        Refuses("blank coverage unit", d => d.scenarioOne.prescriptions[0].coverageDecisions[0].unitId = "", "coverage unit");
        Refuses("unknown coverage choice", d => d.scenarioOne.prescriptions[0].coverageDecisions[0].choice = 9, "coverage choice");
        Refuses("zero coverage choice", d => d.scenarioOne.prescriptions[0].coverageDecisions[0].choice = 0, "coverage choice");
        Refuses("repeated coverage unit", d => d.scenarioOne.prescriptions[0].coverageDecisions[1].unitId = "R2W", "twice");
        Refuses("unknown approval state", d => d.scenarioOne.prescriptions[0].approvalState = 7, "approval state");
        Refuses("unapproved with an approval year", d => d.scenarioOne.prescriptions[1].approvalYear = 15, "approval year");
        Refuses("approved before the source year", d => d.scenarioOne.prescriptions[0].approvalYear = 9, "approval year");
        Refuses("approved after the save", d => d.scenarioOne.prescriptions[0].approvalYear = 21, "approval year");
        Refuses("approved without a year", d => d.scenarioOne.prescriptions[0].approvalYear = -1, "approval year");
        Refuses("order names an unknown prescription", d => d.scenarioOne.workOrders[1].prescriptionId = 9, "unknown prescription");
        Refuses("order negative prescription id", d => d.scenarioOne.workOrders[1].prescriptionId = -1, "invalid prescription link");
        Refuses("order from an unapproved prescription", d => d.scenarioOne.workOrders[1].prescriptionId = 2, "not approved");
        Refuses("order created before approval", d => d.scenarioOne.workOrders[1].createdYear = 11, "predates");
        Refuses("order blank coverage unit", d => d.scenarioOne.workOrders[1].prescriptionCoverageUnitId = "", "coverage unit");
        Refuses("order bad crop tree id", d => d.scenarioOne.workOrders[1].prescriptionCropTreeId = " x", "Crop Tree");
        Refuses("generated order with reason None", d => d.scenarioOne.workOrders[1].prescriptionReason = PrescriptionReasonCode.None, "reason");
        Refuses("order unknown reason", d => d.scenarioOne.workOrders[1].prescriptionReason = 99, "reason");
        Refuses("manual order carrying a coverage unit", d => d.scenarioOne.workOrders[0].prescriptionCoverageUnitId = "R1C", "no prescription");
        Refuses("manual order carrying a reason", d => d.scenarioOne.workOrders[0].prescriptionReason = 1, "no prescription");
        Refuses("event names an unknown prescription", d => d.scenarioOne.managementEvents[0].prescriptionId = 7, "unknown prescription");
        // Ordering: an older failure keeps its own reason when the new data is also bad.
        {
            ForestSaveData both = World();
            both.scenarioOne.managementEvents[1].eventId = 1;                 // duplicate event id (older rule)
            both.scenarioOne.prescriptions[0].prescriptionId = 0;             // and a bad prescription
            string reason = Refusal(both);
            Check(reason != null && reason.IndexOf("event", StringComparison.OrdinalIgnoreCase) >= 0 && reason.IndexOf("prescription id", StringComparison.OrdinalIgnoreCase) < 0,
                "validation order: an older rule's refusal keeps its reason ahead of the prescription checks -> " + reason);
        }
        Check(ForestSaveValidation.Validate(World(), 1, 256) == null, "validation: the valid world passes the typed validator");

        // JSON presence and typing (what JsonUtility would otherwise turn into silent defaults).
        string good = Json(World());
        void RefusesJson(string label, string bad)
        {
            ForestSaveData typed;
            try { typed = Parse(bad); }
            catch (Exception) { Check(true, $"refused (json): {label} -> unparsable, so the disk read refuses it before validation"); return; }
            string reason = ForestSaveValidation.ValidatePrescriptionsJson(bad, typed);
            Check(reason != null, $"refused (json): {label} -> {reason}");
        }
        Check(ForestSaveValidation.ValidatePrescriptionsJson(good, Parse(good)) == null, "accepted (json): the valid v23 document");
        RefusesJson("v23 without the allocator", good.Replace("\"nextPrescriptionId\":3,", ""));
        RefusesJson("v23 without the prescriptions array", Regex.Replace(good, @",""prescriptions"":\[.*\]\}\}$", "}}", RegexOptions.Singleline));
        RefusesJson("prescriptions is an object", good.Replace("\"prescriptions\":[", "\"prescriptions\":{\"a\":["));
        RefusesJson("prescriptions is null", Regex.Replace(good, @"""prescriptions"":\[\{.*\}\]\}\}$", "\"prescriptions\":null}}", RegexOptions.Singleline));
        RefusesJson("id typed as a string", good.Replace("\"prescriptionId\":1,\"ruleVersion\"", "\"prescriptionId\":\"1\",\"ruleVersion\""));
        RefusesJson("id typed as a float", good.Replace("\"prescriptionId\":1,\"ruleVersion\"", "\"prescriptionId\":1.5,\"ruleVersion\""));
        RefusesJson("rule version typed as a number", good.Replace("\"ruleVersion\":\"MFM-V1-RULE-1\"", "\"ruleVersion\":4"));
        RefusesJson("record missing the rule version", good.Replace("\"ruleVersion\":\"MFM-V1-RULE-1\",", ""));
        RefusesJson("record missing the approval year", good.Replace("\"approvalYear\":12", "\"approvalX\":12"));
        RefusesJson("anchor typed as a string", good.Replace("\"sourceAnchorX\":12.5", "\"sourceAnchorX\":\"12.5\""));
        RefusesJson("sample list holds a number", good.Replace("\"sampleRemovalIds\":[\"P0301\"]", "\"sampleRemovalIds\":[7]"));
        RefusesJson("coverage decisions not an array", good.Replace("\"coverageDecisions\":[", "\"coverageDecisions\":{\"x\":["));
        RefusesJson("decision choice typed as a string", good.Replace("\"choice\":2", "\"choice\":\"2\""));
        RefusesJson("work order without its provenance", good.Replace(",\"prescriptionCoverageUnitId\":\"\"", ""));
        RefusesJson("order reason typed as a string", good.Replace("\"prescriptionReason\":0", "\"prescriptionReason\":\"0\""));
        RefusesJson("event without its prescription link", good.Replace("\"ownerMinutes\":0,\"prescriptionId\":0}", "\"ownerMinutes\":0}"));
        RefusesJson("duplicate key", good.Replace("\"nextPrescriptionId\":3,", "\"nextPrescriptionId\":3,\"nextPrescriptionId\":3,"));
        RefusesJson("truncated document", good.Substring(0, good.Length - 40));
        Check(ForestSaveValidation.ValidatePrescriptionsJson(good.Replace("\"version\":" + ForestSaveData.CurrentVersion + ",", "\"version\":22,"), Parse(good.Replace("\"version\":" + ForestSaveData.CurrentVersion + ",", "\"version\":22,"))) == null,
            "json: the presence rule applies only from v23 (a v22 header is not asked for the fields)");

        // ---- 6. hash compatibility and determinism ------------------------------------------------------------------
        if (baseline != null)
        {
            string expected = File.ReadAllText(baseline), actual = HashFixtures.Dump();
            Check(expected == actual, "hash: every pre-v23 fixture world reproduces the v22 (ac876a6) identity hashes byte for byte");
            Check(expected.Split('\n').Count(l => l.StartsWith("parsed-v22-")) == 4 && expected.Contains("|V15=") && expected.Split('\n').Any(l => l.Contains("V18=") && !l.EndsWith("null")),
                "hash: baseline covers parsed v22/v21 inputs, WorldHash, Current, LegacyCompat and V15-V18");
        }
        ForestSaveData defaultWorld = World(); defaultWorld.scenarioOne.prescriptions.Clear(); defaultWorld.scenarioOne.nextPrescriptionId = 1;
        foreach (var o in defaultWorld.scenarioOne.workOrders) { o.prescriptionId = 0; o.prescriptionCoverageUnitId = ""; o.prescriptionCropTreeId = ""; o.prescriptionReason = 0; }
        foreach (var e in defaultWorld.scenarioOne.managementEvents) e.prescriptionId = 0;
        ForestSaveData asV22 = Parse(AsV22(Json(defaultWorld)));
        Check(asV22.version == 22 && ScenarioReferenceArchive.WorldHash(asV22) == ScenarioReferenceArchive.WorldHash(defaultWorld),
            "hash: a world without prescription state hashes the same as a v22 save of it");
        Check(ScenarioReferenceArchive.CurrentWorldHash(asV22) == ScenarioReferenceArchive.CurrentWorldHash(defaultWorld), "hash: and under the current-identity hash");
        string withState = ScenarioReferenceArchive.WorldHash(World());
        Check(withState != ScenarioReferenceArchive.WorldHash(defaultWorld), "hash: prescription state is part of a world's identity once it exists");
        ForestSaveData historyOnly = World(); historyOnly.scenarioOne.objectiveAchievements.Clear();
        Check(ScenarioReferenceArchive.WorldHash(historyOnly) == withState && ScenarioReferenceArchive.CurrentWorldHash(historyOnly) == ScenarioReferenceArchive.CurrentWorldHash(World()),
            "hash: v22 objective history is never part of a world identity, prescription state or not");
        ForestSaveData shuffled = World();
        shuffled.scenarioOne.prescriptions.Reverse(); shuffled.scenarioOne.prescriptions[0].coverageDecisions.Reverse();
        shuffled.scenarioOne.prescriptions[1].sampleCropTreeIds.Reverse();
        Check(ScenarioReferenceArchive.WorldHash(shuffled) == withState, "hash: canonical ordering makes the identity independent of list order");
        ForestSaveData changedRule = World(); changedRule.scenarioOne.prescriptions[0].ruleVersion = "MFM-V1-RULE-2";
        ForestSaveData changedDecision = World(); changedDecision.scenarioOne.prescriptions[0].coverageDecisions[0].choice = PrescriptionCoverageChoice.AcceptedCovered;
        ForestSaveData changedOrder = World(); changedOrder.scenarioOne.workOrders[1].prescriptionCoverageUnitId = "R9";
        Check(new[] { changedRule, changedDecision, changedOrder }.All(w => ScenarioReferenceArchive.WorldHash(w) != withState), "hash: rule, decision and provenance changes each move the identity");
        Check(ScenarioReferenceArchive.WorldHash(World()) == withState && Json(World()) == Json(World()), "determinism: identical worlds serialize and hash identically");
        ForestSaveData legacyWithState = World(); legacyWithState.standGeometryModel = 0;
        Check(ScenarioReferenceArchive.LegacyV18WorldHash(legacyWithState) == null && ScenarioReferenceArchive.LegacyV17WorldHash(legacyWithState) == null
            && ScenarioReferenceArchive.LegacyV16WorldHash(legacyWithState) == null && ScenarioReferenceArchive.LegacyV15WorldHash(legacyWithState) == null,
            "hash: a world carrying prescription state has no historical (v15-v18) layout, so no anchor can claim it");

        // ---- 7. wiring audits (source text of call sites this build cannot compile) -------------------------------------
        if (root != null)
        {
            string fp = Path.Combine(root, "Assets", "ForestPrototype");
            string manager_ = File.ReadAllText(Path.Combine(fp, "ScenarioOne", "ScenarioOneManager.cs"));
            Check(manager_.Contains("ScenarioOneSaveMigration.NormalizePrescriptions(data, saveVersion);"), "audit: RestoreSaveData normalises prescriptions");
            Check(manager_.Contains("RestorePrescriptions(data, saveVersion);"), "audit: RestoreSaveData restores prescriptions by save version");
            Check(manager_.Contains("prescriptions = ClonePrescriptions(prescriptions)") && manager_.Contains("nextPrescriptionId = nextPrescriptionId"), "audit: CaptureSaveData writes the allocator and a copy of the prescriptions");
            int init = manager_.IndexOf("public void InitializeNewScenario()", StringComparison.Ordinal);
            Check(init > 0 && manager_.IndexOf("ClearPrescriptions();", init, StringComparison.Ordinal) > init, "audit: a new game starts with no prescription state");
            string controller = File.ReadAllText(Path.Combine(fp, "ForestSaveController.cs"));
            Check(controller.Contains("ForestSaveValidation.ValidatePrescriptionsJson(json, data)"), "audit: the disk read (shared by Load and CanLoad) runs the prescription JSON check");
            string validation = File.ReadAllText(Path.Combine(fp, "ForestSaveValidation.cs"));
            Check(validation.Contains("return ValidatePrescriptions(data);") && validation.IndexOf("ValidatePrescriptions(data)", StringComparison.Ordinal) > validation.IndexOf("ValidateManagementEventIds(data.scenarioOne)", StringComparison.Ordinal),
                "audit: Validate runs the prescription check last, after every older rule");
            Check(!manager_.Contains("prescriptions.Add(") && !File.ReadAllText(Path.Combine(fp, "ScenarioOne", "ScenarioOneManager.Prescriptions.cs")).Contains("prescriptions.Add("),
                "audit: MFM-06 creates no prescription anywhere (no MFM-07 behaviour)");
            string migration = File.ReadAllText(Path.Combine(fp, "ScenarioOne", "ScenarioOneSaveMigration.cs"));
            Check(migration.Contains("if (data == null || version >= 23) return;"), "audit: prescription normalisation applies only below v23");
            string archive = File.ReadAllText(Path.Combine(fp, "ScenarioOne", "ScenarioReferenceArchive.cs"));
            Check(archive.Contains("if (!PrescriptionStateIsDefault(data))"), "audit: the world-identity layout keys on prescription state");
        }
        Console.WriteLine($"MFM06_SAVE_OFFLINE_PASS checks={checks}");
        return 0;
    }
}

internal static class PlaceholderExtensions
{
    // Present only to keep the v22 assertion list readable; always true.
    public static bool markedTreeIdsPlaceholder(this ScenarioOneSaveData data) => data != null;
}
