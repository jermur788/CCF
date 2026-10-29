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
        // A scenario is a table: objective id -> predicate over the evaluated year (what the real Evaluate would report).
        void Scenario(Dictionary<string, Func<int, bool>> table, Dictionary<string, float> targets = null)
            => ScenarioOneObjectives.Hook = snapshot => table.Select(pair => new ScenarioObjectiveResult
            { objectiveId = pair.Key, achieved = pair.Value(snapshot.year), targetValue = targets != null && targets.ContainsKey(pair.Key) ? targets[pair.Key] : 1f }).ToList();
        string Ledger(ScenarioOneManager m) => string.Join(",", m.ObjectiveAchievements.Select(item => item.objectiveId + "@" + item.achievedYear));
        // Saves the manager, round-trips the scenario data through JSON and reloads it into a fresh manager.
        ScenarioOneManager SaveAndLoad(ScenarioOneManager from, int version = 22)
        {
            var to = new ScenarioOneManager();
            to.CopyYearlyRecordsFrom(from);
            to.Restore(JsonUtility.FromJson<ScenarioOneSaveData>(JsonUtility.ToJson(from.Capture())), version);
            return to;
        }

        var manager = new ScenarioOneManager();
        Scenario(new Dictionary<string, Func<int, bool>> { ["minimum-year"] = y => y >= 25, ["managed-opening"] = y => y >= 6 }, new Dictionary<string, float> { ["minimum-year"] = 25f });
        manager.EvaluateActive(5);
        Check(manager.ObjectiveAchievements.Count == 0, "record: nothing achieved, nothing recorded");
        manager.EvaluateActive(6);
        Check(manager.ObjectiveAchievements.Count == 1 && manager.ObjectiveAchievedYear("managed-opening") == 6, "record: managed-opening recorded in the year first seen achieved");
        Check(manager.ObjectiveAchievements[0].targetValueAtAchievement == 1f, "record: target at achievement stored");
        Check(manager.ObjectiveYear("minimum-year", out ObjectiveYearSource none0) == -1 && none0 == ObjectiveYearSource.NotRecorded, "record: unachieved objective reports -1 (not recorded)");
        manager.EvaluateActive(25); manager.EvaluateActive(26);
        Check(Ledger(manager) == "managed-opening@6,minimum-year@25", "record: first achievement only; later evaluations never duplicate or move a year");
        Scenario(new Dictionary<string, Func<int, bool>> { ["minimum-year"] = y => true, ["managed-opening"] = y => false });
        manager.EvaluateActive(27);
        Check(manager.ObjectiveAchievedYear("managed-opening") == 6 && manager.ObjectiveAchievements.Count == 2, "record: a lost objective keeps its first-achievement record");
        Scenario(new Dictionary<string, Func<int, bool>> { ["minimum-year"] = y => true, ["retained-canopy"] = y => true });
        manager.SetOutcome(ScenarioOneOutcome.Completed);
        manager.EvaluateActive(28);
        Check(manager.ObjectiveAchievedYear("retained-canopy") == -1, "record: nothing is recorded once the scenario outcome is final (as before)");
        manager.SetOutcome(ScenarioOneOutcome.Active);

        ScenarioOneSaveData captured = manager.Capture();
        captured.objectiveAchievements[0].achievedYear = 999;
        Check(manager.ObjectiveAchievedYear("managed-opening") == 6, "capture: save data is a copy, not the live list");
        ScenarioOneManager reloaded = SaveAndLoad(manager);
        Check(Ledger(reloaded) == "managed-opening@6,minimum-year@25", "load: v22 history restored exactly, in saved order");

        // B-1: a NEW v22 game. The year-0 baseline is never evaluated, so objectives already met at the start must still
        // be recorded at the first review, across save/reload at year 0 and later.
        Scenario(new Dictionary<string, Func<int, bool>> { ["retained-canopy"] = y => true, ["continuous-canopy"] = y => true, ["regeneration"] = y => y >= 4 });
        var fresh0 = new ScenarioOneManager();
        fresh0.AddBaseline();
        fresh0.AddEvent(0, ScenarioManagementEventType.StockPurchased);   // year-0 activity must not make year 0 an evaluated year
        ScenarioOneManager atYear0 = SaveAndLoad(fresh0);
        Check(atYear0.ObjectiveAchievements.Count == 0, "new game: reload at year 0 starts with no history and nothing guarded");
        atYear0.EvaluateActive(1);
        Check(Ledger(atYear0) == "retained-canopy@1,continuous-canopy@1", "new game: objectives met at the start are recorded at the first review after a year-0 reload");
        ScenarioOneManager atYear1 = SaveAndLoad(atYear0);
        atYear1.EvaluateActive(2); atYear1.EvaluateActive(3); atYear1.EvaluateActive(4);
        Check(Ledger(atYear1) == "retained-canopy@1,continuous-canopy@1,regeneration@4", "new game: reload at year 1, later achievement recorded, earlier years not moved");
        ScenarioOneManager atYear4 = SaveAndLoad(atYear1);
        atYear4.EvaluateActive(5);
        Check(Ledger(atYear4) == Ledger(atYear1) && atYear4.ObjectiveYear("regeneration", out ObjectiveYearSource src4) == 4 && src4 == ObjectiveYearSource.Recorded,
            "new game: later save/reload keeps every record and labels it recorded");
        var untouched = new ScenarioOneManager();
        untouched.AddBaseline();
        untouched.EvaluateActive(1); untouched.EvaluateActive(2);
        Check(Ledger(SaveAndLoad(untouched)) == "retained-canopy@1,continuous-canopy@1", "new game: no-reload play and reload agree");

        // B-2 / AC-4: a pre-v22 save. Two years evaluated before recording existed: managed-opening met at year 3 (exactly
        // derivable); minimum-year met at year 5; canopy never met. One objective with an evidence gap is not derivable.
        Scenario(new Dictionary<string, Func<int, bool>> { ["managed-opening"] = y => y >= 3, ["minimum-year"] = y => y >= 5, ["regeneration"] = y => y >= 2 });
        var legacyYear = new ScenarioOneManager();   // played before v22: the yearly records exist, the ledger is not carried over
        legacyYear.AddBaseline();
        for (int y = 1; y <= 6; y++) legacyYear.EvaluateActive(y);
        ScenarioOneSaveData oldSave = new ScenarioOneSaveData { objectiveAchievements = new List<ScenarioObjectiveAchievement> { A("minimum-year", 1) } };
        var v21Load = new ScenarioOneManager();
        v21Load.CopyYearlyRecordsFrom(legacyYear);
        v21Load.Restore(oldSave, 21);
        Check(v21Load.ObjectiveAchievements.Count == 0, "pre-v22 load: history starts empty even if the object carried records");
        int d1 = v21Load.ObjectiveYear("managed-opening", out ObjectiveYearSource s1), d2 = v21Load.ObjectiveYear("regeneration", out ObjectiveYearSource s2), d3 = v21Load.ObjectiveYear("minimum-year", out ObjectiveYearSource s3);
        Check(d1 == 3 && s1 == ObjectiveYearSource.Derived && d2 == 2 && s2 == ObjectiveYearSource.Derived && d3 == 5 && s3 == ObjectiveYearSource.Derived,
            "R-1: exactly derivable objectives show the derived year (3, 2, 5), labelled derived");
        Check(v21Load.ObjectiveYear("retained-canopy", out ObjectiveYearSource s4) == -1 && s4 == ObjectiveYearSource.NotRecorded, "R-1: an objective never achieved stays not recorded");
        // Not derivable: the year-4 snapshot is missing from the save, so first-achievement could have been in that gap.
        Scenario(new Dictionary<string, Func<int, bool>> { ["managed-opening"] = y => y >= 3, ["minimum-year"] = y => y >= 5 });
        var gapSource = new ScenarioOneManager();
        gapSource.AddBaseline();
        for (int y = 1; y <= 6; y++) gapSource.EvaluateActive(y);
        gapSource.DropSnapshot(3);
        var gapLoad = new ScenarioOneManager();
        gapLoad.CopyYearlyRecordsFrom(gapSource);
        gapLoad.Restore(new ScenarioOneSaveData(), 21);
        Check(gapLoad.ObjectiveYear("managed-opening", out ObjectiveYearSource sg1) == -1 && sg1 == ObjectiveYearSource.NotRecorded,
            "R-1: evidence gap before the first achieved snapshot -> not recorded, never a guessed year");
        Check(gapLoad.ObjectiveYear("minimum-year", out ObjectiveYearSource sg2) == -1 && sg2 == ObjectiveYearSource.NotRecorded, "R-1: a gap anywhere earlier also makes later first years not exact");
        // pre-v22 load never stamps a later year, and an objective met only after the load is recorded normally
        Scenario(new Dictionary<string, Func<int, bool>> { ["managed-opening"] = y => y >= 3, ["minimum-year"] = y => y >= 5, ["regeneration"] = y => y >= 2, ["retained-canopy"] = y => y >= 8 });
        v21Load.EvaluateActive(7);
        Check(v21Load.ObjectiveAchievements.Count == 0, "pre-v22 load: objectives achieved before the load are never stamped with a later year");
        v21Load.EvaluateActive(8);
        Check(Ledger(v21Load) == "retained-canopy@8", "pre-v22 load: an objective first met after the load is recorded normally");
        ScenarioOneManager v22Again = SaveAndLoad(v21Load);
        v22Again.EvaluateActive(9);
        Check(Ledger(v22Again) == "retained-canopy@8" && v22Again.ObjectiveYear("managed-opening", out ObjectiveYearSource sa) == 3 && sa == ObjectiveYearSource.Derived,
            "pre-v22 -> v22 re-save and reload: still derived/not recorded for old years, new record kept");

        // B-3: met in year 8, lost by 12, met again in 14 (pre-v22 lineage). The first year is knowable (8) and the
        // re-achievement must never become a record.
        Scenario(new Dictionary<string, Func<int, bool>> { ["regeneration"] = y => (y >= 8 && y <= 11) || y >= 14 });
        var lost = new ScenarioOneManager();
        lost.AddBaseline();
        for (int y = 1; y <= 12; y++) lost.EvaluateActive(y);
        Check(lost.ObjectiveAchievedYear("regeneration") == 8, "lost/regained: (control) a v22 game records year 8");
        var lostLoad = new ScenarioOneManager();
        lostLoad.CopyYearlyRecordsFrom(lost);
        lostLoad.Restore(new ScenarioOneSaveData(), 21);
        lostLoad.EvaluateActive(13); lostLoad.EvaluateActive(14); lostLoad.EvaluateActive(15);
        Check(lostLoad.ObjectiveAchievements.Count == 0, "lost/regained: pre-v22 re-achievement in year 14 is never recorded");
        Check(lostLoad.ObjectiveYear("regeneration", out ObjectiveYearSource sl) == 8 && sl == ObjectiveYearSource.Derived, "lost/regained: shows derived year 8, not 14");
        ScenarioOneManager lostAgain = SaveAndLoad(lostLoad);
        lostAgain.EvaluateActive(16);
        Check(lostAgain.ObjectiveAchievements.Count == 0 && lostAgain.ObjectiveYear("regeneration", out ObjectiveYearSource sl2) == 8 && sl2 == ObjectiveYearSource.Derived,
            "lost/regained: still derived 8 after save as v22 and reload (guard rebuilt from every evaluated year)");
        // achieved at the last snapshot only (the previous guard's case) keeps working
        Scenario(new Dictionary<string, Func<int, bool>> { ["regeneration"] = y => y == 12 });
        var lastOnly = new ScenarioOneManager();
        lastOnly.AddBaseline();
        for (int y = 1; y <= 12; y++) lastOnly.EvaluateActive(y);
        var lastLoad = new ScenarioOneManager();
        lastLoad.CopyYearlyRecordsFrom(lastOnly);
        lastLoad.Restore(new ScenarioOneSaveData(), 21);
        Check(lastLoad.ObjectiveYear("regeneration", out ObjectiveYearSource sl3) == 12 && sl3 == ObjectiveYearSource.Derived && lastLoad.ObjectiveAchievements.Count == 0, "pre-v22 achieved at the last snapshot: derived 12, nothing recorded");

        // a final outcome stops derivation at the outcome year
        Scenario(new Dictionary<string, Func<int, bool>> { ["regeneration"] = y => y >= 3, ["fallen-deadwood"] = y => y >= 9 });
        var done = new ScenarioOneManager();
        done.AddBaseline();
        for (int y = 1; y <= 5; y++) done.EvaluateActive(y);
        done.SetOutcome(ScenarioOneOutcome.Completed);
        done.EvaluateActive(6); done.EvaluateActive(9);   // never evaluated: outcome final (stub mirrors production)
        var doneLoad = new ScenarioOneManager();
        doneLoad.CopyYearlyRecordsFrom(done);
        doneLoad.Restore(new ScenarioOneSaveData(), 21);
        Check(doneLoad.ObjectiveYear("fallen-deadwood", out ObjectiveYearSource sd) == -1 && sd == ObjectiveYearSource.NotRecorded, "R-1: snapshots after the outcome was fixed were never evaluated -> no derived year");

        legacyYear.Reset();
        Check(legacyYear.ObjectiveAchievements.Count == 0 && legacyYear.ObjectiveYear("managed-opening", out ObjectiveYearSource sr) == -1, "reset: a new game starts with no history");
        legacyYear.Restore(null, 21);
        Check(legacyYear.ObjectiveAchievements.Count == 0, "restore(null scenario): empty history");

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
