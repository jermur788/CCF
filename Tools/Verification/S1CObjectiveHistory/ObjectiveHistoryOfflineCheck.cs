// S1-C T-05 offline check: compiles the PRODUCTION save schema, validators, migration, hash path (extracted verbatim from
// ScenarioReferenceArchive.cs), history query layer and ScenarioOneManager.ObjectiveHistory.cs, and runs them on stock .NET.
// Proves the pure rules; it does not prove Unity import, the manager's other wiring (source-audited here) or any UI rendering.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public static class ObjectiveHistoryOfflineCheck
{
    private static int checks;
    private static void Check(bool condition, string label)
    {
        if (!condition) throw new Exception("FAIL " + label);
        checks++;
        Console.WriteLine("PASS " + label);
    }

    private static ForestSaveData V22World(params ScenarioObjectiveAchievement[] records)
    {
        var data = new ForestSaveData
        {
            ecologicalYear = 30, growthModel = 1, regenerationModel = 1,
            trees = new List<TreeSaveData> { new TreeSaveData { treeId = "P1", stage = 0 } },
            scenarioOne = new ScenarioOneSaveData { initialized = true, objectiveAchievements = records.ToList() }
        };
        return data;
    }
    private static ScenarioObjectiveAchievement A(string id, int year, float target = 1f)
        => new ScenarioObjectiveAchievement { objectiveId = id, achievedYear = year, targetValueAtAchievement = target };
    private static string Json(ForestSaveData data) => JsonUtility.ToJson(data);
    // Re-labels a v22 JSON as an older save that never wrote the field.
    private static string AsOlder(string json, int version)
        => Regex.Replace(json, @",""objectiveAchievements"":\[[^\]]*\]", "").Replace("\"version\":22,", "\"version\":" + version + ",");

    private static string Refusal(string json, ForestSaveData typed)
        => ForestSaveValidation.ValidateObjectiveHistoryJson(json, typed) ?? ForestSaveValidation.Validate(typed, 1, 64);
    private static string Refusal(ForestSaveData typed) => Refusal(Json(typed), typed);

    public static int Main(string[] args)
    {
        string root = Environment.GetEnvironmentVariable("CCF_REPO_ROOT");
        string baseline = args.Length > 0 ? args[0] : null;

        // ---- 1. schema and v21 -> v22 round trip -------------------------------------------------------------------
        Check(ForestSaveData.CurrentVersion == 22, "schema: CurrentVersion is 22 (next after Rack v21)");
        var fresh = new ForestSaveData();
        Check(fresh.version == 22 && fresh.scenarioOne == null, "schema: new ForestSaveData stamps v22");
        Check(new ScenarioOneSaveData().objectiveAchievements != null && new ScenarioOneSaveData().objectiveAchievements.Count == 0,
            "schema: field defaults to an empty list (not recorded)");
        ForestSaveData world = V22World(A("minimum-year", 25, 25f), A("managed-opening", 30, 1f));
        world.version = 22;
        string json = Json(world);
        ForestSaveData back = JsonUtility.FromJson<ForestSaveData>(json);
        Check(back.version == 22 && back.scenarioOne.objectiveAchievements.Count == 2, "round trip: v22 keeps both records");
        Check(back.scenarioOne.objectiveAchievements[0].objectiveId == "minimum-year" && back.scenarioOne.objectiveAchievements[0].achievedYear == 25
            && back.scenarioOne.objectiveAchievements[1].achievedYear == 30 && back.scenarioOne.objectiveAchievements[1].targetValueAtAchievement == 1f,
            "round trip: ids, years and recorded targets preserved in order");
        Check(JsonUtility.ToJson(back) == json, "round trip: serialized bytes identical after load and re-save");
        Check(Refusal(json, back) == null, "round trip: valid v22 history passes both validators");
        Check(json.IndexOf("\"racks\":", StringComparison.Ordinal) < json.IndexOf("\"objectiveAchievements\":", StringComparison.Ordinal),
            "layout: objectiveAchievements is declared after the v21 rack run");
        // A v21 save loads into v22 code unchanged; going v21 -> v22 only adds an empty history.
        ForestSaveData v21 = JsonUtility.FromJson<ForestSaveData>(AsOlder(json, 21));
        Check(v21.version == 21 && v21.scenarioOne.objectiveAchievements.Count == 0, "v21 save: missing field loads as an empty history");
        Check(Refusal(AsOlder(json, 21), v21) == null, "v21 save: validates (JSON and typed)");
        var resaved = JsonUtility.FromJson<ForestSaveData>(JsonUtility.ToJson(v21)); resaved.version = 22;
        Check(Refusal(Json(resaved), resaved) == null && resaved.scenarioOne.objectiveAchievements.Count == 0, "v21 -> v22 re-save: explicit empty list is valid, nothing synthesised");

        // ---- 2. old saves load unchanged --------------------------------------------------------------------------
        foreach (int old in new[] { 20, 19, 15, 12, 10 })
        {
            ForestSaveData o = JsonUtility.FromJson<ForestSaveData>(AsOlder(json, old));
            Check(o.version == old && ForestSaveValidation.ValidateObjectiveHistory(o) == null
                && ForestSaveValidation.ValidateObjectiveHistoryJson(AsOlder(json, old), o) == null, $"v{old} save: history validators pass with no field");
        }
        ForestSaveData noScenario = new ForestSaveData { version = 8 };
        Check(ForestSaveValidation.ValidateObjectiveHistory(noScenario) == null && ForestSaveValidation.ValidateObjectiveHistoryJson("{\"version\":8}", noScenario) == null,
            "v8 save: no scenarioOne means nothing to validate");
        var legacyData = new ScenarioOneSaveData { objectiveAchievements = new List<ScenarioObjectiveAchievement> { A("minimum-year", 3) } };
        ScenarioOneSaveMigration.NormalizeObjectiveHistory(legacyData, 21);
        Check(legacyData.objectiveAchievements.Count == 0, "migration: a pre-v22 save is normalised to an empty history");
        var keep = new ScenarioOneSaveData { objectiveAchievements = new List<ScenarioObjectiveAchievement> { A("minimum-year", 3) } };
        ScenarioOneSaveMigration.NormalizeObjectiveHistory(keep, 22);
        Check(keep.objectiveAchievements.Count == 1, "migration: a v22 save keeps its history");
        ScenarioOneSaveMigration.NormalizeObjectiveHistory(null, 10);
        Check(true, "migration: null scenario data is tolerated");

        // ---- 3. refusal (all checks run before any state is applied: validators are pure) -------------------------
        void Refuses(string label, ForestSaveData data, string expectedPart, string rawJson = null)
        {
            string before = Json(data);
            string reason = rawJson != null ? Refusal(rawJson, data) : Refusal(data);
            Check(reason != null && reason.IndexOf(expectedPart, StringComparison.OrdinalIgnoreCase) >= 0, $"refused: {label} -> {reason}");
            Check(Json(data) == before, $"no mutation: {label}");
        }
        Refuses("duplicate objective", V22World(A("minimum-year", 10), A("minimum-year", 12)), "twice");
        Refuses("unknown objective", V22World(A("world-peace", 10)), "unknown objective");
        Refuses("empty objective id", V22World(A("", 10)), "unknown objective");
        Refuses("bare introduced prefix", V22World(A("introduced-", 10)), "unknown objective");
        Refuses("negative year", V22World(A("minimum-year", -1)), "impossible");
        Refuses("year after the save", V22World(A("minimum-year", 31)), "impossible");
        Refuses("out of order", V22World(A("minimum-year", 20), A("managed-opening", 10)), "out of order");
        Refuses("negative target", V22World(A("minimum-year", 10, -1f)), "target");
        Check(ForestSaveValidation.ValidateObjectiveHistory(V22World(A("minimum-year", 10, float.NaN))) != null
            && ForestSaveValidation.ValidateObjectiveHistory(V22World(A("minimum-year", 10, float.PositiveInfinity))) != null, "refused: non-finite recorded target");
        Refuses("null record", new ForestSaveData { ecologicalYear = 30, scenarioOne = new ScenarioOneSaveData { objectiveAchievements = new List<ScenarioObjectiveAchievement> { null } } }, "missing");
        ForestSaveData v21With = V22World(A("minimum-year", 10)); v21With.version = 21;
        Refuses("history inside a v21 save", v21With, "older than version 22");
        ForestSaveData v22Flag = V22World(); v22Flag.version = 22;
        string good = Json(v22Flag);
        Check(ForestSaveValidation.ValidateObjectiveHistoryJson(good.Replace(",\"objectiveAchievements\":[]", ""), v22Flag) != null, "refused: v22 JSON without the field");
        Check(ForestSaveValidation.ValidateObjectiveHistoryJson(good.Replace("\"objectiveAchievements\":[]", "\"objectiveAchievements\":{}"), v22Flag) != null, "refused: v22 field is not an array");
        Check(ForestSaveValidation.ValidateObjectiveHistoryJson(good.Replace("\"objectiveAchievements\":[]", "\"objectiveAchievements\":null"), v22Flag) != null, "refused: v22 field is null");
        string rec = good.Replace("\"objectiveAchievements\":[]", "\"objectiveAchievements\":[{\"objectiveId\":\"minimum-year\",\"achievedYear\":10,\"targetValueAtAchievement\":25}]");
        Check(ForestSaveValidation.ValidateObjectiveHistoryJson(rec, v22Flag) == null, "accepted: well-typed record (integer-valued target)");
        Check(ForestSaveValidation.ValidateObjectiveHistoryJson(rec.Replace("\"achievedYear\":10", "\"achievedYear\":\"10\""), v22Flag) != null, "refused: year typed as a string");
        Check(ForestSaveValidation.ValidateObjectiveHistoryJson(rec.Replace("\"achievedYear\":10", "\"achievedYear\":10.5"), v22Flag) != null, "refused: year typed as a float");
        Check(ForestSaveValidation.ValidateObjectiveHistoryJson(rec.Replace(",\"targetValueAtAchievement\":25", ""), v22Flag) != null, "refused: record missing its target");
        Check(ForestSaveValidation.ValidateObjectiveHistoryJson(rec.Replace("\"objectiveId\":\"minimum-year\",", ""), v22Flag) != null, "refused: record missing its id");
        Check(ForestSaveValidation.ValidateObjectiveHistoryJson(rec.Replace("\"achievedYear\":10,", "\"achievedYear\":10,\"achievedYear\":11,"), v22Flag) != null, "refused: duplicate JSON key");
        Check(ForestSaveValidation.ValidateObjectiveHistoryJson(rec.Replace("[{", "[5,{"), v22Flag) != null, "refused: non-object record");
        Check(ForestSaveValidation.ValidateObjectiveHistoryJson("{not json", v22Flag) != null, "refused: unparseable v22 JSON");
        Check(ForestSaveValidation.ValidateObjectiveHistoryJson(Regex.Replace(good, @",""scenarioOne"":.*$", "}"), v22Flag) == null, "v22 without scenarioOne needs no history field");
        Check(Refusal(V22World(A("introduced-oak", 10), A("introduced-beech", 12), A("retained-canopy", 12))) == null, "accepted: per-species introduced-* ids and equal years");

        // ---- 4. recording rules (the real ScenarioOneManager.ObjectiveHistory.cs) ---------------------------------
        ScenarioObjectiveResult R(string id, bool achieved, float target = 1f) => new ScenarioObjectiveResult { objectiveId = id, achieved = achieved, targetValue = target };
        var manager = new ScenarioOneManager();
        ScenarioOneObjectives.Hook = () => new List<ScenarioObjectiveResult> { R("minimum-year", false, 25f), R("managed-opening", false) };
        manager.EvaluateActive(5);
        Check(manager.ObjectiveAchievements.Count == 0, "record: nothing achieved, nothing recorded");
        ScenarioOneObjectives.Hook = () => new List<ScenarioObjectiveResult> { R("minimum-year", false, 25f), R("managed-opening", true, 1f) };
        manager.EvaluateActive(6);
        Check(manager.ObjectiveAchievements.Count == 1 && manager.ObjectiveAchievedYear("managed-opening") == 6, "record: managed-opening recorded in the year first seen achieved");
        Check(manager.ObjectiveAchievements[0].targetValueAtAchievement == 1f, "record: target at achievement stored");
        Check(manager.ObjectiveAchievedYear("minimum-year") == -1, "record: unachieved objective reports -1 (not recorded)");
        ScenarioOneObjectives.Hook = () => new List<ScenarioObjectiveResult> { R("minimum-year", true, 25f), R("managed-opening", true, 1f) };
        manager.EvaluateActive(25);
        manager.EvaluateActive(26);
        Check(manager.ObjectiveAchievements.Count == 2 && manager.ObjectiveAchievedYear("managed-opening") == 6 && manager.ObjectiveAchievedYear("minimum-year") == 25,
            "record: first achievement only; later evaluations never duplicate or move a year");
        ScenarioOneObjectives.Hook = () => new List<ScenarioObjectiveResult> { R("minimum-year", true, 25f), R("managed-opening", false) };
        manager.EvaluateActive(27);
        Check(manager.ObjectiveAchievedYear("managed-opening") == 6 && manager.ObjectiveAchievements.Count == 2, "record: a lost objective keeps its first-achievement record");
        ScenarioOneObjectives.Hook = () => new List<ScenarioObjectiveResult> { R("minimum-year", true, 25f), R("managed-opening", true), R("retained-canopy", true, 20f) };
        manager.SetOutcome(ScenarioOneOutcome.Completed);
        manager.EvaluateActive(28);
        Check(manager.ObjectiveAchievedYear("retained-canopy") == -1, "record: nothing is recorded once the scenario outcome is final (as before)");
        manager.SetOutcome(ScenarioOneOutcome.Active);

        // save -> load round trip through the real manager code
        ScenarioOneSaveData captured = manager.Capture();
        captured.objectiveAchievements[0].achievedYear = 999;
        Check(manager.ObjectiveAchievedYear("managed-opening") == 6, "capture: save data is a copy, not the live list");
        captured = manager.Capture();
        var reloaded = new ScenarioOneManager();
        ScenarioOneObjectives.Hook = () => new List<ScenarioObjectiveResult> { R("minimum-year", true, 25f), R("managed-opening", true) };
        reloaded.Restore(JsonUtility.FromJson<ScenarioOneSaveData>(JsonUtility.ToJson(captured)), 22, 27);
        Check(reloaded.ObjectiveAchievements.Select(item => item.objectiveId + "@" + item.achievedYear).SequenceEqual(new[] { "managed-opening@6", "minimum-year@25" }),
            "load: v22 history restored exactly, in saved order");
        reloaded.EvaluateActive(28);
        Check(reloaded.ObjectiveAchievements.Count == 2, "load: a fully recorded v22 save records nothing new for already-recorded objectives");

        // pre-v22 save: no invented past
        var legacy = new ScenarioOneManager();
        ScenarioOneObjectives.Hook = () => new List<ScenarioObjectiveResult> { R("minimum-year", true, 25f), R("managed-opening", true), R("retained-canopy", false, 20f) };
        var oldData = new ScenarioOneSaveData { objectiveAchievements = new List<ScenarioObjectiveAchievement> { A("minimum-year", 1) } };
        legacy.Restore(oldData, 21, 27);
        Check(legacy.ObjectiveAchievements.Count == 0, "pre-v22 load: history starts empty even if the object carried records");
        legacy.EvaluateActive(28);
        Check(legacy.ObjectiveAchievements.Count == 0 && legacy.ObjectiveAchievedYear("managed-opening") == -1,
            "pre-v22 load: objectives already achieved are never stamped with a later year");
        ScenarioOneObjectives.Hook = () => new List<ScenarioObjectiveResult> { R("minimum-year", true, 25f), R("managed-opening", true), R("retained-canopy", true, 20f) };
        legacy.EvaluateActive(29);
        Check(legacy.ObjectiveAchievements.Count == 1 && legacy.ObjectiveAchievedYear("retained-canopy") == 29, "pre-v22 load: an objective achieved after the load is recorded normally");
        ScenarioOneSaveData savedAgain = legacy.Capture();
        var reload2 = new ScenarioOneManager();
        reload2.Restore(JsonUtility.FromJson<ScenarioOneSaveData>(JsonUtility.ToJson(savedAgain)), 22, 29);
        reload2.EvaluateActive(30);
        Check(reload2.ObjectiveAchievements.Count == 1 && reload2.ObjectiveAchievedYear("managed-opening") == -1,
            "pre-v22 -> v22 re-save and reload: still not recorded (the guard is rebuilt on every load)");
        legacy.Reset();
        Check(legacy.ObjectiveAchievements.Count == 0, "reset: a new game starts with no history");
        legacy.Restore(null, 21, 0);
        Check(legacy.ObjectiveAchievements.Count == 0, "restore(null scenario): empty history");

        // ---- 5. deterministic ordering ----------------------------------------------------------------------------
        ForestSaveData shuffled = V22World(A("managed-opening", 12), A("fallen-deadwood", 12), A("minimum-year", 5));
        ForestSaveData sorted = V22World(A("minimum-year", 5), A("fallen-deadwood", 12), A("managed-opening", 12));
        ScenarioReferenceArchive.Canonicalize(shuffled); ScenarioReferenceArchive.Canonicalize(sorted);
        Check(shuffled.scenarioOne.objectiveAchievements.Select(a => a.objectiveId).SequenceEqual(new[] { "minimum-year", "fallen-deadwood", "managed-opening" })
            && JsonUtility.ToJson(shuffled) == JsonUtility.ToJson(sorted), "ordering: canonical order is (year, ordinal id), independent of input order");
        Check(Refusal(sorted) == null, "ordering: the canonical order is itself a valid history");

        // ---- 6. history surface (T-03 query layer) ----------------------------------------------------------------
        var events = new List<ScenarioManagementEvent> { new ScenarioManagementEvent { eventId = 1, year = 5, eventType = ScenarioManagementEventType.YearAdvanced }, new ScenarioManagementEvent { eventId = 2, year = 6, eventType = ScenarioManagementEventType.YearAdvanced } };
        var reports = new List<ScenarioAnnualReport> { new ScenarioAnnualReport { year = 5 }, new ScenarioAnnualReport { year = 6 } };
        var none = new List<ScenarioEcologicalSnapshot>(); var dw = new List<ScenarioDeadwoodRecord>(); var wo = new List<ScenarioOneWorkOrder>(); var st = new List<StormEventRecord>();
        var achievements = new List<ScenarioObjectiveAchievement> { A("managed-opening", 6), A("minimum-year", 25) };
        List<YearHistoryView> views = ScenarioManagementHistory.BuildYearViews(events, reports, none, dw, wo, st, achievements);
        Check(views.Count == 2 && views[0].ObjectivesAchieved.Count == 0 && views[1].ObjectivesAchieved.Count == 1 && views[1].ObjectivesAchieved[0].objectiveId == "managed-opening",
            "history: a year view lists exactly the objectives first achieved that year");
        Check(ScenarioManagementHistory.BuildYearViews(events, reports, none, dw, wo, st).All(v => v.ObjectivesAchieved.Count == 0)
            && ScenarioManagementHistory.BuildYearViews(events, reports, none, dw, wo, st, null).All(v => v.ObjectivesAchieved.Count == 0),
            "history: no achievements (pre-v22 save or old call site) leaves every year empty, not invented");
        Check(ScenarioManagementHistory.BuildYearView(events, reports, none, dw, wo, st, 6, achievements).ObjectivesAchieved.Count == 1, "history: single-year query carries the same field");

        // ---- 7. hash compatibility --------------------------------------------------------------------------------
        if (baseline != null)
        {
            string expected = File.ReadAllText(baseline), actual = HashFixtures.Dump();
            string[] e = expected.Split('\n', StringSplitOptions.RemoveEmptyEntries), a = actual.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            Check(e.Length == a.Length && e.Length == HashFixtures.Build().Count, $"hash: {e.Length} fixture worlds in both builds");
            for (int i = 0; i < e.Length; i++)
                Check(e[i] == a[i], "hash: pre-v22 (aca74ac, v21) hashes reproduced exactly for " + e[i].Split('|')[0]);
            Check(expected.Contains("|V15=") && expected.Split('\n').Any(l => l.Contains("V18=") && !l.EndsWith("null")), "hash: baseline covers WorldHash, Current, LegacyCompat and V15-V18");
        }
        foreach (var fixture in HashFixtures.Build())
        {
            string plain = ScenarioReferenceArchive.WorldHash(fixture.Value);
            ForestSaveData withHistory = JsonUtility.FromJson<ForestSaveData>(JsonUtility.ToJson(fixture.Value));
            withHistory.scenarioOne.objectiveAchievements = new List<ScenarioObjectiveAchievement> { A("minimum-year", 3, 25f), A("introduced-oak", 9, 1f) };
            Check(ScenarioReferenceArchive.WorldHash(withHistory) == plain && ScenarioReferenceArchive.CurrentWorldHash(withHistory) == ScenarioReferenceArchive.CurrentWorldHash(fixture.Value)
                && ScenarioReferenceArchive.LegacyV18WorldHash(withHistory) == ScenarioReferenceArchive.LegacyV18WorldHash(fixture.Value),
                "hash: recorded achievements do not move the world identity for " + fixture.Key);
        }

        // ---- 8. protected state and wiring audits (source text) --------------------------------------------------
        if (root != null)
        {
            string fp = Path.Combine(root, "Assets", "ForestPrototype");
            string manager_ = File.ReadAllText(Path.Combine(fp, "ScenarioOne", "ScenarioOneManager.cs"));
            string eval = Method(manager_, "private void EvaluateProgress()");
            int active = eval.IndexOf("if (outcome == ScenarioOneOutcome.Active)", StringComparison.Ordinal);
            int record = eval.IndexOf("RecordObjectiveAchievements(objectives, snapshot.year);", StringComparison.Ordinal);
            int setOutcome = eval.IndexOf("SetOutcome(ScenarioOneOutcome.Completed", StringComparison.Ordinal);
            Check(active >= 0 && record > active && record < setOutcome, "audit: achievements are recorded inside the Active branch of EvaluateProgress, before the outcome is set");
            Check(Method(manager_, "private void SetOutcome(").Contains("outcomeYear = year;") && !Method(manager_, "private void SetOutcome(").Contains("objectiveAchievements"), "audit: outcomeYear semantics unchanged and independent of the history");
            Check(Method(manager_, "public ScenarioOneSaveData CaptureSaveData()").Contains("objectiveAchievements = CloneObjectiveAchievements(objectiveAchievements)"), "audit: CaptureSaveData clones the history");
            Check(Method(manager_, "public void RestoreSaveData(ScenarioOneSaveData data, int saveVersion)").Contains("NormalizeObjectiveHistory(data, saveVersion)")
                && Method(manager_, "public void RestoreSaveData(ScenarioOneSaveData data, int saveVersion)").Contains("RestoreObjectiveHistory(data, saveVersion);"), "audit: RestoreSaveData normalises then restores the history");
            string controller = File.ReadAllText(Path.Combine(fp, "ForestSaveController.cs"));
            Check(controller.Contains("ValidateRackV2Json(json, data)\n                ?? ForestSaveValidation.ValidateObjectiveHistoryJson(json, data)"), "audit: disk loader runs the history JSON check in the pre-mutation refusal chain");
            string validation = File.ReadAllText(Path.Combine(fp, "ForestSaveValidation.cs"));
            Check(validation.IndexOf("ValidateRackV2(data)", StringComparison.Ordinal) < validation.IndexOf("ValidateObjectiveHistory(data)", StringComparison.Ordinal)
                && validation.IndexOf("ValidateObjectiveHistory(data)", StringComparison.Ordinal) < validation.IndexOf("return ValidateManagementEventIds", StringComparison.Ordinal),
                "audit: semantic check runs after Rack v21 and before the final T-06 event-ID check, which keeps its position");
            string archive = File.ReadAllText(Path.Combine(fp, "ScenarioOne", "ScenarioReferenceArchive.cs"));
            Check(archive.Contains("saveVersion == 12 && saveVersion <= ForestSaveData.CurrentVersion") && archive.Contains("Hash(compact.ToString()) != milestone.worldHash"),
                "audit: Reference Future v1 identity check (saveVersion 12, original-bytes hash) is intact");
            string normalization = File.ReadAllText(Path.Combine(fp, "ScenarioOne", "ScenarioOneSaveMigration.cs"));
            Check(normalization.Contains("if (data == null || version >= 21) return;") && normalization.Contains("if (data == null || version >= 15) return;"), "audit: v15 and v21 normalisation untouched");
        }
        Console.WriteLine($"S1C_T05_OFFLINE_PASS checks={checks}");
        return 0;
    }

    private static string Method(string text, string signature)
    {
        int start = text.IndexOf(signature, StringComparison.Ordinal);
        if (start < 0) throw new Exception("FAIL audit: missing " + signature);
        int brace = text.IndexOf('{', start), depth = 1, end = brace + 1;
        while (depth > 0) { if (text[end] == '{') depth++; else if (text[end] == '}') depth--; end++; }
        return text.Substring(start, end - start);
    }
}
