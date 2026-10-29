#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;
using CCF.Forestry.WorkEconomy;

// S1-C T-05 (save v22 objective/progression history) Unity qualification harness. Disposable: staged by run_gate.py into
// Assets/ForestPrototype together with HashFixtures.cs, run once, then removed with its .meta.
//
// Real production paths only: StandaloneSessionMenu.StartNewScenario, ScenarioOneManager.AdvanceYear / EvaluateProgress /
// CaptureSaveData / RestoreSaveData, ForestSaveController.Save / CanLoad / LoadData / Load, ForestSaveValidation,
// ScenarioReferenceArchive hashing through Unity JsonUtility, and the real AnnualReviewView. Nothing is injected into the
// manager's private state. The only fixtures are save bytes: genuine v20/v21 files written by earlier builds (copied
// read-only by the runner) and in-memory/typed edits of those bytes to model a damaged old save.
// The player's forest-save.json/.bak/.tmp are restored byte for byte in the end; the runner also isolates XDG_CONFIG_HOME.
public static class S1CObjectiveHistoryUnityVerification
{
    private const string ExitKey = "CCF.S1C.T05.Unity.Exit";
    public const string EntryName = "S1CObjectiveHistoryUnityVerification.Begin";

    public static void Begin()
    {
        SessionState.EraseInt(ExitKey);
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        S1CObjectiveHistoryUnityRunner.SetGameSize(1280, 720);
        EditorApplication.isPlaying = true;
    }

    public static void Finish(bool pass)
    {
        SessionState.SetInt(ExitKey, pass ? 0 : 1);
        ResumePendingExit();
        EditorApplication.ExitPlaymode();
    }

    [InitializeOnLoadMethod]
    private static void ResumePendingExit()
    {
        if (SessionState.GetInt(ExitKey, -1) < 0) return;
        EditorApplication.update -= ExitWhenStopped;
        EditorApplication.update += ExitWhenStopped;
    }

    private static void ExitWhenStopped()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling || EditorApplication.isUpdating) return;
        int code = SessionState.GetInt(ExitKey, -1);
        if (code < 0) return;
        SessionState.EraseInt(ExitKey);
        EditorApplication.update -= ExitWhenStopped;
        EditorApplication.delayCall += () => EditorApplication.Exit(code);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (!Environment.GetCommandLineArgs().Any(a => a == EntryName)) return;
        new GameObject("S1-C T-05 Unity qualification").AddComponent<S1CObjectiveHistoryUnityRunner>();
    }
}

public sealed class S1CObjectiveHistoryUnityRunner : MonoBehaviour
{
    private static readonly FieldInfo MessageField = typeof(ForestSaveController).GetField("message", BindingFlags.NonPublic | BindingFlags.Instance);

    private string output;
    private Vector2Int size = new Vector2Int(1280, 720);
    private int passes, failures, shots, cases;
    private ScenarioOneUiRoot ui;
    private ScenarioOneManager manager;
    private ForestSaveController saves;
    private ForestEcologyController ecology;
    private ForestPlayer player;
    private ForestTreeSpawner spawner;
    private ForestTreeMarkingManager marks;
    private StandaloneSessionMenu menu;
    private string path;
    private readonly Dictionary<string, byte[]> originals = new Dictionary<string, byte[]>();
    private readonly List<string> errors = new List<string>();
    private readonly List<string> notes = new List<string>();
    private readonly List<string> results = new List<string>();
    private readonly Dictionary<string, string> texts = new Dictionary<string, string>();

    private byte[] goodBytes;            // disk bytes of the recorded v22 world (Save() output)
    private string goodCompact;          // JsonUtility.ToJson of the same capture
    private ForestSaveData goodData;
    private List<Fx> fixtures = new List<Fx>();

    private sealed class Fx { public string name, file, note; public byte[] bytes; public int version; public string sha; }

    // ------------------------------------------------------------------------------------------ Editor plumbing

    public static void SetGameSize(int width, int height)
    {
        if (Application.isBatchMode) return;
        Assembly assembly = typeof(Editor).Assembly;
        Type sizesType = assembly.GetType("UnityEditor.GameViewSizes");
        Type singleton = typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
        object sizes = singleton.GetProperty("instance", BindingFlags.Public | BindingFlags.Static).GetValue(null);
        MethodInfo getGroup = sizesType.GetMethod("GetGroup");
        object group = getGroup.Invoke(sizes, new[] { Enum.ToObject(getGroup.GetParameters()[0].ParameterType, 0) });
        Type sizeType = assembly.GetType("UnityEditor.GameViewSize"), kind = assembly.GetType("UnityEditor.GameViewSizeType");
        object gameSize = Activator.CreateInstance(sizeType, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
            null, new object[] { Enum.ToObject(kind, 1), width, height, "S1C T05 " + width + "x" + height }, null);
        int index = (int)group.GetType().GetMethod("GetTotalCount").Invoke(group, null);
        group.GetType().GetMethod("AddCustomSize").Invoke(group, new[] { gameSize });
        Type viewType = assembly.GetType("UnityEditor.GameView"); EditorWindow view = EditorWindow.GetWindow(viewType);
        viewType.GetProperty("selectedSizeIndex", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).SetValue(view, index);
        PropertyInfo gizmos = viewType.GetProperty("showGizmos", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (gizmos != null && gizmos.CanWrite) gizmos.SetValue(view, false);
        view.Show(); view.Focus(); view.Repaint();
    }

    private IEnumerator Start()
    {
        output = Environment.GetEnvironmentVariable("CCF_DIAG_DIR");
        if (string.IsNullOrEmpty(output)) throw new InvalidOperationException("CCF_DIAG_DIR must name durable evidence outside the repo");
        Directory.CreateDirectory(Path.Combine(output, "review-texts"));
        Directory.CreateDirectory(Path.Combine(output, "shots"));
        Application.logMessageReceived += OnLogMessage;
        Exception failure = null;
        var stack = new Stack<IEnumerator>();
        stack.Push(Play());
        while (stack.Count > 0)
        {
            bool more; object current = null;
            try { more = stack.Peek().MoveNext(); if (more) current = stack.Peek().Current; }
            catch (Exception error) { failure = error; break; }
            if (!more) { stack.Pop(); continue; }
            if (current is IEnumerator nested) stack.Push(nested); else yield return current;
        }
        // The player's own save slot comes back byte for byte whatever happened above.
        try
        {
            foreach (var entry in originals)
                if (entry.Value == null) { if (File.Exists(entry.Key)) File.Delete(entry.Key); }
                else File.WriteAllBytes(entry.Key, entry.Value);
        }
        catch (Exception error) { failure = failure ?? error; }
        if (errors.Count > 0)
        {
            failures++;
            Debug.Log("S1C_T05_CHECK_FAIL NO_CONSOLE_ERRORS " + string.Join(" | ", errors.Take(4)));
        }
        bool pass = failure == null && failures == 0;
        Application.logMessageReceived -= OnLogMessage;
        WriteReports(pass, failure);
        Debug.Log(pass
            ? $"S1C_T05_UNITY_PASS checks={passes} cases={cases} shots={shots}"
            : $"S1C_T05_UNITY_FAIL checks={passes} failures={failures} exception={failure}");
        S1CObjectiveHistoryUnityVerification.Finish(pass);
    }

    // ------------------------------------------------------------------------------------------ the qualification

    private IEnumerator Play()
    {
        yield return StartSession();
        yield return PhaseA_NewGameRecording();
        yield return PhaseB_Refusals();
        LoadFixtures();
        yield return PhaseC_GenuineOldSaves();
        yield return PhaseD_EvidenceGaps();
        PhaseE_Hashes();
        yield return PhaseF_Rendered();
    }

    // ------------------------------------------------------------------------------------------ Phase A (items 2, 3, 8)

    private string RecordsText() => string.Join(";", manager.ObjectiveAchievements.Select(r => $"{r.objectiveId}@{r.achievedYear}@{r.targetValueAtAchievement:R}"));

    private IEnumerator SaveReload(string label)
    {
        string before = RecordsText();
        string arrayBefore = ArrayText(JsonUtility.ToJson(saves.CaptureData()));
        int yearBefore = ecology.EcologicalYear;
        saves.Save(); yield return Settle(2);
        JObject disk = JObject.Parse(File.ReadAllText(path));
        Check((int)disk["version"] == 22 && disk["scenarioOne"]["objectiveAchievements"] is JArray, label + "_DISK_V22_EXPLICIT_ARRAY");
        Check(saves.CanLoad(out string problem), label + "_CANLOAD", problem);
        saves.Load(); yield return Settle(3);
        Check(ecology.EcologicalYear == yearBefore, label + "_YEAR_RESTORED", $"{ecology.EcologicalYear} vs {yearBefore}");
        Check(RecordsText() == before, label + "_RECORDS_SURVIVE_RELOAD", $"before=[{before}] after=[{RecordsText()}]");
        Check(ArrayText(JsonUtility.ToJson(saves.CaptureData())) == arrayBefore, label + "_ARRAY_JSON_IDENTICAL_AFTER_RELOAD");
    }

    private static string ArrayText(string json)
    {
        Match m = Regex.Match(json, "\"objectiveAchievements\":\\[[^\\]]*\\]");
        return m.Success ? m.Value : "MISSING";
    }

    private IEnumerator PhaseA_NewGameRecording()
    {
        Section("A: new game, year-0 save/reload, year-1 recording (B-1), distinct-year records");
        ForestSaveData start = saves.CaptureData();
        Check(ForestSaveData.CurrentVersion == 22 && start.version == 22, "A0_CURRENT_VERSION_22", "version=" + start.version);
        Check(start.scenarioOne.objectiveAchievements != null && start.scenarioOne.objectiveAchievements.Count == 0, "A0_NEW_GAME_EMPTY_HISTORY");
        Check(manager.CurrentEcologicalYear == 0 && manager.ObjectiveAchievements.Count == 0, "A0_YEAR0_NO_RECORDS");
        string worldYear0 = WorldHash();
        saves.Save(); yield return Settle(2);
        JObject disk0 = JObject.Parse(File.ReadAllText(path));
        Check((int)disk0["version"] == 22 && disk0["scenarioOne"]["objectiveAchievements"] is JArray a0 && a0.Count == 0, "A0_YEAR0_DISK_EXPLICIT_EMPTY_ARRAY");
        Check(saves.CanLoad(out string p0), "A0_YEAR0_CANLOAD", p0);
        saves.Load(); yield return Settle(3);
        Check(manager.CurrentEcologicalYear == 0 && manager.ObjectiveAchievements.Count == 0, "A0_YEAR0_RELOAD_STILL_EMPTY");
        notes.Add($"year-0 world hash before/after reload: {worldYear0} / {WorldHash()} (informational)");

        // Year 1 evaluation after the year-0 reload: the baseline snapshot must not count as an evaluated year (B-1).
        Check(manager.AdvanceYear(), "A1_ADVANCE_TO_YEAR1", manager.Feedback);
        yield return Settle(4);
        var achieved = manager.Objectives.Where(o => o.achieved).ToList();
        notes.Add("achieved at year 1: " + string.Join(",", achieved.Select(o => o.objectiveId)));
        Check(achieved.Count > 0, "A1_SOME_OBJECTIVE_ACHIEVED_AT_YEAR1");
        Check(achieved.All(o => manager.ObjectiveAchievedYear(o.objectiveId) == 1), "A1_EVERY_ACHIEVED_OBJECTIVE_RECORDED_YEAR1", RecordsText());
        Check(manager.ObjectiveAchievements.All(r => achieved.Any(o => o.objectiveId == r.objectiveId && Mathf.Approximately(o.targetValue, r.targetValueAtAchievement))),
            "A1_RECORDS_CARRY_OBJECTIVE_TARGET");
        Check(achieved.Any(o => o.objectiveId == "retained-canopy") && manager.ObjectiveAchievedYear("retained-canopy") == 1, "A1_RETENTION_OBJECTIVE_RECORDED_AFTER_YEAR1_B1");
        Check(achieved.Any(o => o.objectiveId == "continuous-canopy") && manager.ObjectiveAchievedYear("continuous-canopy") == 1, "A1_CANOPY_OBJECTIVE_RECORDED_AFTER_YEAR1_B1");
        AssertOracle("A1");
        yield return SaveReload("A1_SAVE_RELOAD");

        // Year 2: a real commissioned thinning -> managed-opening first reached in a different year.
        yield return PlanFelling(1);
        AdvanceWithWork("thinning year 2");
        yield return Settle(4);
        Check(manager.CurrentEcologicalYear == 2, "A2_YEAR2");
        Check(manager.ObjectiveAchievedYear("managed-opening") == 2, "A2_MANAGED_OPENING_RECORDED_YEAR2", RecordsText());
        Check(manager.ObjectiveAchievedYear("retained-canopy") == 1, "A2_EARLIER_RECORD_UNTOUCHED");
        AssertOracle("A2");
        yield return SaveReload("A2_SAVE_RELOAD");

        // Year 3: a real oak planting (introduced-<species> is the shop-species objective); it may need further years to establish.
        yield return PlanPlanting(true, WorkExecutionMethod.LandownerSimulated);
        AdvanceWithWork("planting year 3");
        yield return Settle(4);
        Check(manager.CurrentEcologicalYear == 3, "A3_YEAR3");
        notes.Add("records at year 3: " + RecordsText());
        AssertOracle("A3");
        int distinctYears = manager.ObjectiveAchievements.Select(r => r.achievedYear).Distinct().Count();
        Check(distinctYears >= 2, "A3_AT_LEAST_TWO_OBJECTIVES_IN_DIFFERENT_YEARS", RecordsText());
        yield return SaveReload("A3_SAVE_RELOAD");

        // Raw-JSON movement is exactly the additive record, and no world hash moves with it.
        ForestSaveData capture = saves.CaptureData();
        string withRecords = JsonUtility.ToJson(capture);
        ForestSaveData bare = Clone(capture); bare.scenarioOne.objectiveAchievements = new List<ScenarioObjectiveAchievement>();
        string without = JsonUtility.ToJson(bare);
        Check(withRecords != without && withRecords.Replace(ArrayText(withRecords), "\"objectiveAchievements\":[]") == without, "A4_RAW_JSON_DIFFERS_ONLY_BY_ACHIEVEMENT_ARRAY");
        Check(HashLine(capture) == HashLine(bare), "A4_ALL_WORLD_HASHES_IGNORE_THE_RECORD", HashLine(capture) + " vs " + HashLine(bare));
        notes.Add("live y3 world hashes: " + HashLine(capture));

        saves.Save(); yield return Settle(2);
        goodBytes = File.ReadAllBytes(path);
        goodData = saves.CaptureData();
        goodCompact = JsonUtility.ToJson(goodData);
        File.WriteAllBytes(Path.Combine(output, "recorded-v22-year3-save.json"), goodBytes);
        Check(goodData.scenarioOne.objectiveAchievements.Count >= 2, "A5_RECORDED_WORLD_FIXTURE_CAPTURED", RecordsText());
    }

    // The independent oracle: replays the year-tagged snapshots through the real ScenarioOneObjectives.Evaluate and compares.
    private void AssertOracle(string label)
    {
        foreach (ScenarioObjectiveAchievement record in manager.ObjectiveAchievements)
        {
            int first = OracleFirstYear(record.objectiveId, out _);
            Check(first == record.achievedYear, label + "_ORACLE_FIRST_YEAR_" + record.objectiveId, $"record={record.achievedYear} oracle={first}");
        }
    }

    // First evaluated year (>0) in which the real evaluator reports the objective achieved, replaying each year with the
    // events/orders as they stood then. gapBefore: some evaluated year at or before it lacks its snapshot or YearAdvanced event.
    private int OracleFirstYear(string id, out bool gapBefore)
    {
        gapBefore = false;
        int last = manager.Outcome == ScenarioOneOutcome.Active ? int.MaxValue : manager.OutcomeYear;
        var eventYears = new HashSet<int>(manager.ManagementEvents.Where(e => e != null && e.eventType == ScenarioManagementEventType.YearAdvanced && e.year <= last).Select(e => e.year));
        var snapYears = new HashSet<int>(manager.EcologicalSnapshots.Where(s => s != null && s.year > 0 && s.year <= last).Select(s => s.year));
        bool gap = false;
        foreach (int year in eventYears.Union(snapYears).OrderBy(y => y))
        {
            ScenarioEcologicalSnapshot snapshot = manager.EcologicalSnapshots.FirstOrDefault(s => s != null && s.year == year);
            if (snapshot == null || !eventYears.Contains(year) || !snapYears.Contains(year)) gap = true;
            if (snapshot == null) continue;
            var events = manager.ManagementEvents.Where(e => e != null && e.year <= year).ToList();
            var orders = manager.WorkOrders.Where(o => o != null && (o.status != ScenarioWorkStatus.Completed || o.resolvedYear <= year)).ToList();
            if (ScenarioOneObjectives.Evaluate(manager.Definition, snapshot, events, spawner.DefaultSpecies.SpeciesId, orders, ecology.StandGeometryModelVersion)
                    .Any(r => r.objectiveId == id && r.achieved))
            {
                gapBefore = gap;
                return year;
            }
        }
        return -1;
    }

    // ------------------------------------------------------------------------------------------ Phase B (item 5)

    private string World()
    {
        ForestSaveData data = saves.CaptureData();
        return ecology.StandGeometryModelVersion + "|" + ecology.EcologicalYear + "|" + FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None).Length
            + "|" + JsonUtility.ToJson(player.transform.position) + "|" + JsonUtility.ToJson(player.transform.rotation)
            + "|" + ScenarioReferenceArchive.CurrentWorldHash(data) + "|" + JsonUtility.ToJson(data.scenarioOne);
    }
    private string Disk() => string.Join("|", new[] { path, path + ".bak", path + ".tmp" }.Select(p => File.Exists(p) ? Convert.ToBase64String(File.ReadAllBytes(p)) : "missing"));
    private string Message() => (string)MessageField.GetValue(saves);

    private void PutDisk(byte[] bytes)
    {
        File.WriteAllBytes(path, bytes);
        File.WriteAllText(path + ".bak", "backup fixture — must remain unchanged");
        File.WriteAllText(path + ".tmp", "temporary fixture — must remain unchanged");
    }

    // A refused save: all three routes that can see the problem must refuse with the same words, and nothing may change.
    // typed == null means the problem is only visible in the JSON text (the typed route cannot see it).
    private IEnumerator Refuse(string label, string json, ForestSaveData typed, string fragment)
    {
        cases++;
        PutDisk(Encoding.UTF8.GetBytes(json));
        string world = World(), disk = Disk(), hash = WorldHash();
        int year = ecology.EcologicalYear;
        string records = RecordsText();
        bool can = saves.CanLoad(out string canProblem);
        Check(!can, "B_REFUSED_CANLOAD_" + label, canProblem);
        Check(canProblem != null && canProblem.Contains(fragment), "B_REASON_" + label, $"reason=[{canProblem}] wanted fragment [{fragment}]");
        Check(World() == world && Disk() == disk, "B_CANLOAD_NO_MUTATION_" + label);
        if (typed != null)
        {
            bool loaded = saves.LoadData(Clone(typed), true);
            Check(!loaded, "B_REFUSED_TYPED_LOADDATA_" + label);
            Check(Message() == canProblem, "B_TYPED_REASON_EQUALS_CANLOAD_" + label, $"typed=[{Message()}] canLoad=[{canProblem}]");
            Check(World() == world && Disk() == disk, "B_TYPED_NO_MUTATION_" + label);
        }
        saves.Load();
        Check(Message() == canProblem, "B_DISK_LOAD_REASON_EQUALS_CANLOAD_" + label, $"load=[{Message()}] canLoad=[{canProblem}]");
        yield return Settle(2);
        Check(World() == world && Disk() == disk, "B_DISK_LOAD_NO_MUTATION_" + label);
        Check(WorldHash() == hash && ecology.EcologicalYear == year && RecordsText() == records, "B_LIVE_WORLD_HASH_YEAR_RECORDS_UNCHANGED_" + label);
        Debug.Log($"S1C_T05_REFUSAL {label} reason=[{canProblem}] worldHash={hash} diskBytesUnchanged=true");
    }

    private IEnumerator Accept(string label, string json, ForestSaveData typed)
    {
        cases++;
        PutDisk(Encoding.UTF8.GetBytes(json));
        string disk = Disk();
        bool can = saves.CanLoad(out string canProblem);
        Check(can, "B_ACCEPT_CANLOAD_" + label, canProblem);
        Check(saves.LoadData(Clone(typed), false), "B_ACCEPT_TYPED_" + label);
        string viaTyped = RecordsText();
        saves.Load(); yield return Settle(2);
        Check(RecordsText() == viaTyped && Disk() == disk, "B_ACCEPT_ROUTES_AGREE_AND_DISK_UNCHANGED_" + label, $"typed=[{viaTyped}] disk=[{RecordsText()}]");
        Debug.Log($"S1C_T05_ACCEPT {label} records=[{RecordsText()}]");
    }

    private IEnumerator PhaseB_Refusals()
    {
        Section("B: refusals and accepted controls at the real load routes");
        path = Path.Combine(Application.persistentDataPath, "forest-save.json");
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        string xdg = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME");
        Check(!string.IsNullOrEmpty(xdg) && path.StartsWith(xdg), "B0_SAVE_SLOT_IS_THE_ISOLATED_CONFIG", path + " under " + xdg);
        player.enabled = false; // freeze movement so the no-mutation snapshots are exact
        string worldAtStart = World();
        string recordsAtStart = RecordsText();

        // Accepted controls first (they change the live world only to an equal one).
        yield return Accept("v22 with records", goodCompact, goodData);
        ForestSaveData empty = Clone(goodData); empty.scenarioOne.objectiveAchievements = new List<ScenarioObjectiveAchievement>();
        yield return Accept("v22 explicit empty list", JsonUtility.ToJson(empty), empty);
        Check(RecordsText() == "", "B_ACCEPT_EMPTY_LIST_LEAVES_NO_RECORDS");
        yield return Accept("v22 world restored", goodCompact, goodData);
        Check(RecordsText() == recordsAtStart, "B_RESTORED_RECORDS_EQUAL");
        worldAtStart = World();

        // Semantic refusals: the typed route and the disk routes must give one answer.
        ForestSaveData preV22 = Clone(goodData); preV22.version = 21;
        yield return Refuse("pre-v22 save carrying records", JsonUtility.ToJson(preV22), preV22, "older than version 22");
        ForestSaveData v20Records = Clone(goodData); v20Records.version = 20;
        yield return Refuse("v20 save carrying records", JsonUtility.ToJson(v20Records), v20Records, "older than version 22");
        ForestSaveData dup = Clone(goodData); dup.scenarioOne.objectiveAchievements.Add(Clone(dup.scenarioOne.objectiveAchievements[0]));
        yield return Refuse("duplicate ids", JsonUtility.ToJson(dup), dup, "appears twice");
        ForestSaveData negYear = Clone(goodData); negYear.scenarioOne.objectiveAchievements[0].achievedYear = -1;
        yield return Refuse("negative year", JsonUtility.ToJson(negYear), negYear, "impossible achieved year");
        ForestSaveData futureYear = Clone(goodData); futureYear.scenarioOne.objectiveAchievements[futureYear.scenarioOne.objectiveAchievements.Count - 1].achievedYear = futureYear.ecologicalYear + 1;
        yield return Refuse("year after the save", JsonUtility.ToJson(futureYear), futureYear, "impossible achieved year");
        ForestSaveData unknown = Clone(goodData); unknown.scenarioOne.objectiveAchievements[0].objectiveId = "not-an-objective";
        yield return Refuse("unknown objective id", JsonUtility.ToJson(unknown), unknown, "unknown objective");
        ForestSaveData emptyId = Clone(goodData); emptyId.scenarioOne.objectiveAchievements[0].objectiveId = "";
        yield return Refuse("empty objective id", JsonUtility.ToJson(emptyId), emptyId, "unknown objective");
        ForestSaveData introEmpty = Clone(goodData); introEmpty.scenarioOne.objectiveAchievements[0].objectiveId = "introduced-";
        yield return Refuse("introduced- without a species", JsonUtility.ToJson(introEmpty), introEmpty, "unknown objective");
        ForestSaveData order = Clone(goodData);
        order.scenarioOne.objectiveAchievements.Reverse();
        yield return Refuse("records out of year order", JsonUtility.ToJson(order), order, "out of order");
        ForestSaveData negTarget = Clone(goodData); negTarget.scenarioOne.objectiveAchievements[0].targetValueAtAchievement = -0.5f;
        yield return Refuse("negative target", JsonUtility.ToJson(negTarget), negTarget, "invalid recorded target");
        ForestSaveData nanTarget = Clone(goodData); nanTarget.scenarioOne.objectiveAchievements[0].targetValueAtAchievement = float.NaN;
        yield return Refuse("NaN target", JsonUtility.ToJson(nanTarget), nanTarget, "");
        ForestSaveData infTarget = Clone(goodData); infTarget.scenarioOne.objectiveAchievements[0].targetValueAtAchievement = float.PositiveInfinity;
        yield return Refuse("infinite target", JsonUtility.ToJson(infTarget), infTarget, "");

        // JSON-text refusals (only the disk routes see these).
        Match array = Regex.Match(goodCompact, ",\"objectiveAchievements\":\\[[^\\]]*\\]");
        Check(array.Success, "B_FIXTURE_ARRAY_LOCATED");
        string noField = goodCompact.Replace(array.Value, "");
        yield return Refuse("field absent in a v22 save", noField, null, "explicit objective achievement history");
        yield return Refuse("field null", goodCompact.Replace(array.Value, ",\"objectiveAchievements\":null"), null, "explicit objective achievement history");
        yield return Refuse("field is an object", goodCompact.Replace(array.Value, ",\"objectiveAchievements\":{}"), null, "explicit objective achievement history");
        yield return Refuse("field is a string", goodCompact.Replace(array.Value, ",\"objectiveAchievements\":\"none\""), null, "explicit objective achievement history");
        string noYear = Regex.Replace(array.Value, "\"achievedYear\":\\d+,", "", RegexOptions.None);
        yield return Refuse("record missing achievedYear", goodCompact.Replace(array.Value, noYear), null, "missing required fields");
        string stringYear = Regex.Replace(array.Value, "\"achievedYear\":(\\d+)", "\"achievedYear\":\"$1\"");
        yield return Refuse("achievedYear is a string", goodCompact.Replace(array.Value, stringYear), null, "missing required fields");
        string noTarget = Regex.Replace(array.Value, ",\"targetValueAtAchievement\":[^,}]+", "");
        yield return Refuse("record missing target", goodCompact.Replace(array.Value, noTarget), null, "missing required fields");
        string noId = Regex.Replace(array.Value, "\"objectiveId\":\"[^\"]*\",", "");
        yield return Refuse("record missing objectiveId", goodCompact.Replace(array.Value, noId), null, "missing required fields");
        yield return Refuse("duplicate objectiveAchievements keys", goodCompact.Replace(array.Value, array.Value + ",\"objectiveAchievements\":[]"), null, "invalid version 22 objective history JSON");
        yield return Refuse("duplicate keys inside a record", goodCompact.Replace("\"objectiveId\":", "\"objectiveId\":\"x\",\"objectiveId\":"), null, "");
        yield return Refuse("parse error: truncated", goodCompact.Substring(0, goodCompact.Length / 2), null, "unreadable");
        yield return Refuse("parse error: not JSON", "{ this is not json", null, "unreadable");

        // Back to the accepted v22 world: none of the refusals changed anything in the live world.
        Check(World() == worldAtStart, "B_WORLD_AFTER_ALL_REFUSALS_EQUALS_BEFORE");
        Check(RecordsText() == recordsAtStart, "B_RECORDS_AFTER_ALL_REFUSALS_EQUAL_BEFORE");
    }

    // ------------------------------------------------------------------------------------------ fixtures

    private void LoadFixtures()
    {
        string dir = Environment.GetEnvironmentVariable("CCF_T05_FIXTURES");
        if (string.IsNullOrEmpty(dir)) throw new InvalidOperationException("CCF_T05_FIXTURES must name the read-only genuine save fixtures");
        JArray manifest = JArray.Parse(File.ReadAllText(Path.Combine(dir, "manifest.json")));
        foreach (JObject item in manifest)
        {
            var fx = new Fx { name = (string)item["name"], file = (string)item["file"], note = (string)item["note"] };
            fx.bytes = File.ReadAllBytes(Path.Combine(dir, fx.file));
            fx.version = (int)JObject.Parse(Encoding.UTF8.GetString(fx.bytes))["version"];
            fx.sha = Sha(fx.bytes);
            fixtures.Add(fx);
            notes.Add($"fixture {fx.name}: version {fx.version} sha256 {fx.sha} ({fx.note})");
        }
    }

    private static string Sha(byte[] bytes)
    {
        using (var sha = System.Security.Cryptography.SHA256.Create())
            return string.Concat(sha.ComputeHash(bytes).Select(b => b.ToString("x2")));
    }

    // ------------------------------------------------------------------------------------------ Phase C (items 4, 6)

    private string SourceText(string id)
    {
        int year = manager.ObjectiveYear(id, out ObjectiveYearSource source);
        return source + (year >= 0 ? ":" + year : "");
    }

    private string SourcesText() => string.Join(",", manager.Objectives.Where(o => o.achieved).Select(o => o.objectiveId + "=" + SourceText(o.objectiveId)));

    // Expected source for a currently achieved objective, from the independent oracle.
    private void AssertSources(string label)
    {
        foreach (ScenarioObjectiveResult o in manager.Objectives.Where(x => x.achieved))
        {
            int year = manager.ObjectiveYear(o.objectiveId, out ObjectiveYearSource source);
            if (manager.ObjectiveAchievedYear(o.objectiveId) >= 0)
            {
                Check(source == ObjectiveYearSource.Recorded, label + "_RECORDED_SOURCE_" + o.objectiveId, source.ToString());
                continue;
            }
            int oracle = OracleFirstYear(o.objectiveId, out bool gap);
            ObjectiveYearSource expected = oracle >= 0 && !gap ? ObjectiveYearSource.Derived : ObjectiveYearSource.NotRecorded;
            Check(source == expected && (expected == ObjectiveYearSource.NotRecorded ? year == -1 : year == oracle),
                label + "_SOURCE_" + o.objectiveId, $"production={source}:{year} oracle={oracle} gapBefore={gap}");
        }
    }

    private IEnumerator PhaseC_GenuineOldSaves()
    {
        Section("C: genuine v20/v21 saves through ForestSaveController (typed and disk routes), R-1, v22 re-save");
        int derived = 0, notRecordedAchieved = 0;
        foreach (Fx fx in fixtures)
        {
            string label = "C_" + fx.name;
            cases++;
            PutDisk(fx.bytes);
            string diskBefore = Disk();
            Check(saves.CanLoad(out string problem), label + "_CANLOAD", problem);
            Check(Disk() == diskBefore, label + "_CANLOAD_LEAVES_DISK");

            // Typed route.
            ForestSaveData typed = JsonUtility.FromJson<ForestSaveData>(Encoding.UTF8.GetString(fx.bytes));
            Check(typed.version == fx.version && (typed.scenarioOne.objectiveAchievements == null || typed.scenarioOne.objectiveAchievements.Count == 0), label + "_PARSED_PRE_V22_HAS_NO_RECORDS");
            Check(saves.LoadData(Clone(typed), false), label + "_TYPED_LOADDATA");
            yield return Settle(3);
            string typedCapture = CaptureFingerprint();
            string typedSources = SourcesText();
            Check(manager.ObjectiveAchievements.Count == 0, label + "_TYPED_NO_RETROSPECTIVE_RECORD", RecordsText());
            Check(Disk() == diskBefore, label + "_TYPED_LOAD_LEAVES_DISK");

            // Disk route.
            saves.Load(); yield return Settle(3);
            string diskCapture = CaptureFingerprint();
            Check(manager.ObjectiveAchievements.Count == 0, label + "_DISK_NO_RETROSPECTIVE_RECORD", RecordsText());
            Check(typedCapture == diskCapture, label + "_TYPED_AND_DISK_ROUTES_AGREE", typedCapture + " vs " + diskCapture);
            Check(SourcesText() == typedSources, label + "_ROUTES_AGREE_ON_SOURCES", typedSources + " vs " + SourcesText());
            Check(Disk() == diskBefore, label + "_DISK_LOAD_LEAVES_BYTES");

            ForestSaveData loaded = saves.CaptureData();
            Check(loaded.version == 22 && loaded.scenarioOne.objectiveAchievements != null && loaded.scenarioOne.objectiveAchievements.Count == 0, label + "_MIGRATED_TO_V22_EMPTY_HISTORY");
            AssertSources(label);
            string sourcesBeforeSave = SourcesText();
            derived += manager.Objectives.Count(o => o.achieved && SourceText(o.objectiveId).StartsWith("Derived"));
            notRecordedAchieved += manager.Objectives.Count(o => o.achieved && SourceText(o.objectiveId).StartsWith("NotRecorded"));
            Debug.Log($"S1C_T05_OLD_SAVE {fx.name} v{fx.version} year={ecology.EcologicalYear} sources=[{sourcesBeforeSave}] unachieved={string.Join(",", manager.Objectives.Where(o => !o.achieved).Select(o => o.objectiveId))} fingerprint={Short(typedCapture)}");

            // v22 re-save with an explicit empty list, then reload through both routes.
            string captureBeforeSave = CaptureFingerprint();
            saves.Save(); yield return Settle(2);
            JObject resaved = JObject.Parse(File.ReadAllText(path));
            Check((int)resaved["version"] == 22 && resaved["scenarioOne"]["objectiveAchievements"] is JArray ra && ra.Count == 0, label + "_RESAVE_V22_EXPLICIT_EMPTY_LIST");
            Check(File.ReadAllBytes(path + ".bak").SequenceEqual(fx.bytes), label + "_RESAVE_KEPT_ORIGINAL_AS_BAK");
            Check(saves.CanLoad(out string p2), label + "_RESAVE_CANLOAD", p2);
            saves.Load(); yield return Settle(3);
            Check(manager.ObjectiveAchievements.Count == 0 && SourcesText() == sourcesBeforeSave, label + "_RESAVE_RELOAD_SOURCES_UNCHANGED", $"{sourcesBeforeSave} vs {SourcesText()}");
            Check(CaptureFingerprint() == captureBeforeSave, label + "_RESAVE_RELOAD_WORLD_UNCHANGED");
            ForestSaveData typedAgain = JsonUtility.FromJson<ForestSaveData>(File.ReadAllText(path));
            Check(saves.LoadData(typedAgain, false), label + "_RESAVE_TYPED_ROUTE");
            yield return Settle(2);
            Check(manager.ObjectiveAchievements.Count == 0 && SourcesText() == sourcesBeforeSave && CaptureFingerprint() == captureBeforeSave, label + "_RESAVE_TYPED_AND_DISK_AGREE");
            AssertSources(label + "_AFTER_RESAVE");
        }
        Check(derived >= 1, "C_R1_AT_LEAST_ONE_OBJECTIVE_DERIVED_FROM_GENUINE_SAVE", "derived=" + derived);
        notes.Add($"genuine-save totals: derived={derived} achievedButNotRecorded={notRecordedAchieved}");

        // Control: advancing an evidence-complete genuine v20 world records only genuinely new first achievements.
        Fx big = fixtures.First(f => f.name == "v20-y5");
        PutDisk(big.bytes);
        saves.Load(); yield return Settle(3);
        var before = manager.Objectives.Where(o => o.achieved).Select(o => o.objectiveId).ToList();
        Check(manager.AdvanceYear(), "C_CONTROL_ADVANCE_GENUINE_V20", manager.Feedback);
        yield return Settle(3);
        int newYear = manager.CurrentEcologicalYear;
        foreach (ScenarioObjectiveAchievement r in manager.ObjectiveAchievements)
        {
            bool achievedEarlier = OracleFirstYear(r.objectiveId, out _) < newYear;
            Check(r.achievedYear == newYear && !achievedEarlier && !before.Contains(r.objectiveId), "C_CONTROL_NO_FALSE_FIRST_" + r.objectiveId, $"record={r.achievedYear}");
        }
        foreach (string id in before)
            Check(manager.ObjectiveAchievedYear(id) < 0, "C_CONTROL_PREVIOUSLY_ACHIEVED_STAYS_UNRECORDED_" + id, RecordsText());
        notes.Add("control advance: records after advancing the genuine v20 world = [" + RecordsText() + "]");
    }

    private string CaptureFingerprint()
    {
        ForestSaveData d = saves.CaptureData();
        return ScenarioReferenceArchive.CurrentWorldHash(d) + "/" + ScenarioReferenceArchive.Hash(JsonUtility.ToJson(d.scenarioOne));
    }
    private static string Short(string text) => text.Length <= 60 ? text : text.Substring(0, 60) + "…";

    // ------------------------------------------------------------------------------------------ Phase D (item 7)

    private IEnumerator PhaseD_EvidenceGaps()
    {
        Section("D: evidence-gap regression (missing snapshot, missing YearAdvanced) never yields a later false first");
        Fx big = fixtures.First(f => f.name == "v20-y5");
        PutDisk(big.bytes);
        saves.Load(); yield return Settle(3);
        var achievedNow = manager.Objectives.Where(o => o.achieved).Select(o => o.objectiveId).ToList();
        var firstYears = achievedNow.ToDictionary(id => id, id => OracleFirstYear(id, out _));
        notes.Add("v20-y5 oracle first-achieved years: " + string.Join(",", firstYears.Select(kv => kv.Key + "=" + kv.Value)));
        int gapYear = firstYears.Values.Where(y => y >= 1 && y <= 4).DefaultIfEmpty(2).Min();
        Check(achievedNow.Count > 0, "D0_GENUINE_FIXTURE_HAS_ACHIEVED_OBJECTIVES");

        foreach (string kind in new[] { "snapshot", "event" })
        {
            ForestSaveData gap = JsonUtility.FromJson<ForestSaveData>(Encoding.UTF8.GetString(big.bytes));
            if (kind == "snapshot")
            {
                int removed = gap.scenarioOne.ecologicalSnapshots.RemoveAll(s => s != null && s.year == gapYear);
                Check(removed == 1, "D_" + kind + "_FIXTURE_REMOVED_ONE_SNAPSHOT", "year=" + gapYear);
            }
            else
            {
                int removed = gap.scenarioOne.managementEvents.RemoveAll(e => e != null && e.eventType == ScenarioManagementEventType.YearAdvanced && e.year == gapYear);
                Check(removed >= 1, "D_" + kind + "_FIXTURE_REMOVED_YEARADVANCED", "year=" + gapYear + " removed=" + removed);
            }
            string gapJson = JsonUtility.ToJson(gap);
            foreach (string route in new[] { "typed", "disk" })
            {
                string label = $"D_{kind}_{route}";
                cases++;
                if (route == "typed") { Check(saves.LoadData(Clone(gap), false), label + "_LOAD_ACCEPTED"); }
                else { PutDisk(Encoding.UTF8.GetBytes(gapJson)); Check(saves.CanLoad(out string p), label + "_CANLOAD", p); saves.Load(); }
                yield return Settle(3);
                Check(manager.ObjectiveAchievements.Count == 0, label + "_NO_RECORD_AT_LOAD");
                AssertSources(label + "_LOAD");
                foreach (string id in achievedNow.Where(id => firstYears[id] >= gapYear))
                    Check(SourceText(id) == "NotRecorded", label + "_AT_OR_AFTER_GAP_IS_NOT_RECORDED_" + id, SourceText(id));
                string sources0 = SourcesText();
                notes.Add($"{label}: sources at load [{sources0}]");

                // The regression itself: advance real years; the still-achieved objectives must not get a late "first".
                for (int step = 1; step <= 2; step++)
                {
                    Check(manager.AdvanceYear(), label + "_ADVANCE_" + step, manager.Feedback);
                    yield return Settle(3);
                    Check(manager.ObjectiveAchievements.Count == 0, label + "_NO_FALSE_FIRST_AFTER_ADVANCE_" + step, RecordsText());
                    foreach (string id in achievedNow.Where(id => firstYears[id] >= gapYear))
                        Check(manager.ObjectiveAchievedYear(id) == -1 && SourceText(id) == "NotRecorded", label + $"_STILL_NOT_RECORDED_{step}_{id}", SourceText(id));
                }
                // And survives a v22 re-save and reload.
                saves.Save(); yield return Settle(2);
                saves.Load(); yield return Settle(3);
                Check(manager.ObjectiveAchievements.Count == 0, label + "_RESAVE_RELOAD_STILL_NO_RECORD", RecordsText());
                foreach (string id in achievedNow.Where(id => firstYears[id] >= gapYear))
                    Check(SourceText(id) == "NotRecorded", label + "_RESAVE_RELOAD_NOT_RECORDED_" + id, SourceText(id));
                Check(manager.AdvanceYear(), label + "_ADVANCE_AFTER_RELOAD", manager.Feedback);
                yield return Settle(3);
                Check(manager.ObjectiveAchievements.Count == 0, label + "_NO_FALSE_FIRST_AFTER_RELOAD_ADVANCE", RecordsText());
            }
        }
    }

    // ------------------------------------------------------------------------------------------ Phase E (item 8)

    private static string HashLine(ForestSaveData d)
        => "World=" + ScenarioReferenceArchive.WorldHash(d) + "|Current=" + ScenarioReferenceArchive.CurrentWorldHash(d)
        + "|LegacyCompat=" + (ScenarioReferenceArchive.LegacyCompatibleWorldHash(d) ?? "null")
        + "|V15=" + (ScenarioReferenceArchive.LegacyV15WorldHash(d) ?? "null") + "|V16=" + (ScenarioReferenceArchive.LegacyV16WorldHash(d) ?? "null")
        + "|V17=" + (ScenarioReferenceArchive.LegacyV17WorldHash(d) ?? "null") + "|V18=" + (ScenarioReferenceArchive.LegacyV18WorldHash(d) ?? "null");

    private void PhaseE_Hashes()
    {
        Section("E: ScenarioReferenceArchive hashes through Unity JsonUtility (legacy, default-rack, rack-state, parsed v21)");
        var table = new StringBuilder();
        var lines = new Dictionary<string, string>();
        foreach (var fixture in HashFixtures.Build())
        {
            ForestSaveData d = fixture.Value;
            string line = HashLine(d);
            lines[fixture.Key] = line;
            table.Append(fixture.Key).Append('|').Append("version=").Append(d.version).Append('|').Append(line).Append('\n');
            ForestSaveData recorded = Clone(d);
            recorded.version = d.version;
            recorded.scenarioOne.objectiveAchievements = new List<ScenarioObjectiveAchievement>
            {
                new ScenarioObjectiveAchievement { objectiveId = "retained-canopy", achievedYear = 1, targetValueAtAchievement = 3f },
                new ScenarioObjectiveAchievement { objectiveId = "managed-opening", achievedYear = 2, targetValueAtAchievement = 1f }
            };
            Check(HashLine(recorded) == line, "E_RECORDS_DO_NOT_MOVE_ANY_HASH_" + fixture.Key, HashLine(recorded) + " vs " + line);
        }
        File.WriteAllText(Path.Combine(output, "unity-hash-fixtures.txt"), table.ToString());
        foreach (var pair in new[] { ("parsed-v21-legacy40-default", "legacy40-regen2-growth1"), ("parsed-v21-enlarged80-model1-empty-plan", "enlarged80-model1-empty-plan"),
                                     ("parsed-v21-enlarged80-rack-state", "enlarged80-model1-rack-state"), ("parsed-v21-legacy40-rack-order-only", "legacy40-rack-order-only") })
            Check(lines[pair.Item1] == lines[pair.Item2], "E_PARSED_V21_HASHES_AS_THE_SAME_V22_WORLD_" + pair.Item1, lines[pair.Item1] + " vs " + lines[pair.Item2]);
        notes.Add("hash fixture table: unity-hash-fixtures.txt");
    }

    // ------------------------------------------------------------------------------------------ Phase F (item 9)

    private IEnumerator PhaseF_Rendered()
    {
        Section("F: rendered Annual Review, real AnnualReviewView, 1280x720 and 1920x1080");
        Fx big = fixtures.First(f => f.name == "v20-y5");
        ForestSaveData gap = JsonUtility.FromJson<ForestSaveData>(Encoding.UTF8.GetString(big.bytes));
        gap.scenarioOne.ecologicalSnapshots.RemoveAll(s => s != null && s.year == 1);
        byte[] gapBytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(gap));
        File.WriteAllBytes(Path.Combine(output, "render-gap-v20-y5-no-year1-snapshot.json"), gapBytes);
        var states = new[]
        {
            ("recorded", goodBytes, "(recorded)", "(derived from saved records)", "Year reached: not recorded"),
            ("derived", big.bytes, "(derived from saved records)", "(recorded)", "Year reached: not recorded"),
            ("not-recorded", gapBytes, "Year reached: not recorded", "First reached in year", "")
        };
        foreach (Vector2Int target in Sizes())
        {
            yield return UseSize(target);
            foreach (var state in states)
            {
                PutDisk(state.Item2);
                Check(saves.CanLoad(out string problem), $"F_{state.Item1}_{size.x}_CANLOAD", problem);
                saves.Load(); yield return Settle(6);
                ui.CloseAll(); yield return Settle(4); ui.ShowReview(); yield return Settle(10);
                VisualElement modal = Modal();
                Check(modal != null, $"F_{state.Item1}_{size.x}_REVIEW_OPEN");
                if (modal == null) continue;
                string text = Text(modal);
                File.WriteAllText(Path.Combine(output, "review-texts", $"{state.Item1}-{size.x}x{size.y}.txt"), text);
                Check(text.Contains(state.Item3), $"F_{state.Item1}_{size.x}_SHOWS_{state.Item3}", Short(text));
                Check(!text.Contains(state.Item4) || state.Item4 == "", $"F_{state.Item1}_{size.x}_DOES_NOT_SHOW_{state.Item4}");
                if (state.Item1 == "recorded")
                    Check(Regex.Matches(text, "First reached in year \\d+ \\(recorded\\)").Count >= 2, $"F_recorded_{size.x}_TWO_RECORDED_LINES");
                if (state.Item1 == "derived")
                    Check(Regex.IsMatch(text, "First reached in year [1-9]\\d* \\(derived from saved records\\)"), $"F_derived_{size.x}_DERIVED_LINE_WITH_YEAR");
                if (state.Item1 == "not-recorded")
                    Check(!text.Contains("First reached in year"), $"F_not-recorded_{size.x}_NO_FIRST_REACHED_LINES");
                AuditLayout($"{state.Item1}");
                Label card = modal.Query<Label>().ToList().FirstOrDefault(l => l.text != null && l.text.StartsWith("OBJECTIVES"));
                Check(card != null, $"F_{state.Item1}_{size.x}_OBJECTIVES_CARD");
                yield return Shot($"review-{state.Item1}-top", null);
                if (card != null) yield return Shot($"review-{state.Item1}-objectives", card);
            }
        }
    }

    // ------------------------------------------------------------------------------------------ fixtures through real manager operations

    private IEnumerator PlanFelling(int count)
    {
        manager.PlanningFellingOutcome = FellingMaterialOutcome.SellAndExtract;
        ForestTree[] trees = Trees();
        if (trees.Length < count) throw new InvalidOperationException("not enough thinnable trees");
        for (int i = 0; i < count; i++) marks.Mark(trees[i], TreeMarkType.Fell, false);
        manager.AddMarkedTreesToWorkPlan();
        yield return null;
    }

    private IEnumerator PlanPlanting(bool shelter, WorkExecutionMethod method)
    {
        Check(manager.TryPurchaseStock("sessile-oak-sapling", 1), "FIXTURE_BUY_OAK_STOCK");
        bool placed = false;
        foreach (Vector3 point in PlantingCandidates())
            if (manager.TryDesignateExactPlanting("sessile-oak-sapling", point, method, shelter)) { placed = true; break; }
        Check(placed, "FIXTURE_DESIGNATE_PLANTING");
        yield return null;
    }

    private void AdvanceWithWork(string what)
    {
        bool approved = manager.ApprovePendingWork();
        Check(approved, "FIXTURE_APPROVE_" + what.Replace(' ', '_'), manager.Feedback);
        bool advanced = manager.AdvanceYear();
        Check(advanced, "FIXTURE_ADVANCE_" + what.Replace(' ', '_'), manager.Feedback);
        if (!approved || !advanced) throw new InvalidOperationException("fixture could not complete " + what);
    }

    private ForestTree[] Trees() => UnityEngine.Object.FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
        .Where(tree => tree != null && tree.IsLiving && tree.CanChop && tree.TreeId.StartsWith("P", StringComparison.Ordinal))
        .OrderBy(tree => tree.TreeId, StringComparer.Ordinal).ToArray();

    private IEnumerable<Vector3> PlantingCandidates()
    {
        ForestTree[] trees = Trees();
        foreach (var cell in ecology.Cells.OrderByDescending(c => c.Light).ThenBy(c => c.Center.x).ThenBy(c => c.Center.y))
            foreach (float x in new[] { -1.5f, 0f, 1.5f })
                foreach (float z in new[] { -1.5f, 0f, 1.5f })
                {
                    var point = new Vector3(cell.Center.x + x, 0f, cell.Center.y + z);
                    if (trees.All(tree => Vector2.Distance(new Vector2(tree.transform.position.x, tree.transform.position.z), new Vector2(point.x, point.z)) >= 0.8f))
                        yield return point;
                }
    }

    private IEnumerator StartSession()
    {
        for (int frame = 0; frame < 900 && (menu == null || !StandaloneSessionMenu.IsOpen); frame++)
        {
            if (menu == null) menu = UnityEngine.Object.FindFirstObjectByType<StandaloneSessionMenu>();
            yield return null;
        }
        if (menu == null || !StandaloneSessionMenu.IsOpen) throw new InvalidOperationException("the startup screen did not open");
        for (int frame = 0; frame < 180 && !Application.isFocused; frame++) yield return null;
        // The harness may only ever touch the isolated config's save slot; remember it before anything writes.
        path = Path.Combine(Application.persistentDataPath, "forest-save.json");
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        foreach (string p in new[] { path, path + ".bak", path + ".tmp" }) originals[p] = File.Exists(p) ? File.ReadAllBytes(p) : null;

        menu.StartNewScenario();
        for (int frame = 0; frame < 600 && (ui == null || ui.Help == null || !ui.Help.IsOpen); frame++)
        {
            if (ui == null) ui = UnityEngine.Object.FindFirstObjectByType<ScenarioOneUiRoot>();
            yield return null;
        }
        if (ui == null || ui.Help == null) throw new InvalidOperationException("Scenario One UI missing after Start New");
        ui.CloseHelp();
        yield return Settle(20);
        manager = ui.Manager;
        saves = UnityEngine.Object.FindFirstObjectByType<ForestSaveController>();
        ecology = UnityEngine.Object.FindFirstObjectByType<ForestEcologyController>();
        player = UnityEngine.Object.FindFirstObjectByType<ForestPlayer>();
        spawner = UnityEngine.Object.FindFirstObjectByType<ForestTreeSpawner>();
        marks = ui.Marking != null ? ui.Marking : UnityEngine.Object.FindFirstObjectByType<ForestTreeMarkingManager>();
        if (manager == null || saves == null || ecology == null || marks == null || player == null || spawner == null || MessageField == null)
            throw new InvalidOperationException("manager, save controller, ecology, player, spawner or marking manager missing");
        notes.Add("stand geometry model " + ecology.StandGeometryModelVersion + "; trees at start " + Trees().Length + "; save slot " + path);
        Check(manager.AnnualReports.Count == 0, "NEW_GAME_HAS_NO_COMPLETED_YEARS");
    }

    // ------------------------------------------------------------------------------------------ UI reading and rendering

    private VisualElement Modal() => ui.RootElement.Q(className: "modal-review");

    private static string Text(VisualElement element) => string.Join("\n", element.Query<TextElement>().ToList().Where(Visible).Select(e => e.text));

    private static bool Visible(VisualElement element)
    {
        if (element == null) return false;
        for (VisualElement p = element; p != null; p = p.parent)
            if (p.resolvedStyle.display == DisplayStyle.None || p.resolvedStyle.visibility == Visibility.Hidden) return false;
        return element.resolvedStyle.width > 0.5f && element.resolvedStyle.height > 0.5f;
    }

    private IEnumerator UseSize(Vector2Int target)
    {
        size = target;
        SetGameSize(target.x, target.y);
        for (int frame = 0; frame < 120 && (Screen.width != target.x || Screen.height != target.y); frame++) yield return null;
        yield return Settle(24);
        Check(Screen.width == target.x && Screen.height == target.y, $"TARGET_RENDER_SIZE_{target.x}x{target.y}", $"actual={Screen.width}x{Screen.height}");
    }

    private IEnumerator Shot(string name, VisualElement focus)
    {
        ScrollView scroll = Modal().Q<ScrollView>();
        if (scroll != null)
        {
            if (focus == null) scroll.verticalScroller.value = scroll.verticalScroller.lowValue; else scroll.ScrollTo(focus);
            yield return Settle(8);
        }
        if (focus != null && scroll != null)
            Check(focus.worldBound.Overlaps(scroll.contentViewport.worldBound), $"[{size.x}x{size.y}]_FOCUS_VISIBLE_IN_VIEWPORT_{name}");
        yield return new WaitForEndOfFrame();
        Texture2D image = ScreenCapture.CaptureScreenshotAsTexture();
        if (image == null) { Check(false, "SHOT_" + name, "no rendered capture"); yield break; }
        Check(image.width == size.x && image.height == size.y, $"CAPTURE_SIZE_{name}_{size.x}", $"capture={image.width}x{image.height} target={size.x}x{size.y}");
        Check(!LooksBlank(image), $"SHOT_NOT_BLANK_{name}_{size.x}");
        string file = $"{name}-{size.x}x{size.y}.png";
        File.WriteAllBytes(Path.Combine(output, "shots", file), image.EncodeToPNG());
        Destroy(image);
        shots++;
        Debug.Log($"S1C_T05_SHOT {file}");
    }

    // Every visible text element is horizontally inside the review panel; the objective lines are not clipped by their card.
    private void AuditLayout(string name)
    {
        VisualElement modal = Modal();
        Rect panel = modal.worldBound;
        var overflow = new List<string>();
        foreach (TextElement element in modal.Query<TextElement>().ToList())
        {
            if (string.IsNullOrEmpty(element.text) || !Visible(element)) continue;
            Rect r = element.worldBound;
            if (r.xMin < panel.xMin - 1.5f || r.xMax > panel.xMax + 1.5f) overflow.Add(Short(element.text) + $" [{r.xMin:0}..{r.xMax:0} vs {panel.xMin:0}..{panel.xMax:0}]");
        }
        Check(overflow.Count == 0, $"[{size.x}x{size.y}]_TEXT_INSIDE_PANEL_{name}", string.Join(" | ", overflow.Take(3)));
        var clipped = new List<string>();
        foreach (Label label in modal.Query<Label>().ToList().Where(l => Visible(l) && l.text != null
                     && (l.text.StartsWith("First reached") || l.text.StartsWith("Year reached"))))
        {
            Vector2 measured = label.MeasureTextSize(label.text, label.contentRect.width, VisualElement.MeasureMode.Exactly, 0, VisualElement.MeasureMode.Undefined);
            VisualElement card = label.parent;
            if (measured.y > label.contentRect.height + 1.5f || label.worldBound.xMax > card.worldBound.xMax + 1.5f) clipped.Add(label.text);
        }
        Check(clipped.Count == 0, $"[{size.x}x{size.y}]_OBJECTIVE_HISTORY_LINES_NOT_CLIPPED_{name}", string.Join(" | ", clipped));
        notes.Add($"[{size.x}x{size.y}] {name}: history lines " + string.Join(" / ", modal.Query<Label>().ToList().Where(l => Visible(l) && l.text != null
            && (l.text.StartsWith("First reached") || l.text.StartsWith("Year reached"))).Select(l => l.text)));
    }

    private static bool LooksBlank(Texture2D image)
    {
        Color32[] pixels = image.GetPixels32();
        int step = Mathf.Max(1, pixels.Length / 4000), different = 0;
        Color32 first = pixels[0];
        for (int i = 0; i < pixels.Length; i += step)
            if (Mathf.Abs(pixels[i].r - first.r) + Mathf.Abs(pixels[i].g - first.g) + Mathf.Abs(pixels[i].b - first.b) > 24) different++;
        return different < 40;
    }

    private static IEnumerable<Vector2Int> Sizes()
    {
        string text = Environment.GetEnvironmentVariable("CCF_UI_SIZES");
        var list = new List<Vector2Int>();
        foreach (string part in (string.IsNullOrEmpty(text) ? "1280x720,1920x1080" : text).Split(','))
        {
            string[] xy = part.Trim().Split('x');
            if (xy.Length == 2 && int.TryParse(xy[0], out int w) && int.TryParse(xy[1], out int h)) list.Add(new Vector2Int(w, h));
        }
        if (!list.Contains(new Vector2Int(1280, 720)) || !list.Contains(new Vector2Int(1920, 1080)))
            throw new InvalidOperationException("CCF_UI_SIZES must include 1280x720 and 1920x1080");
        return list;
    }

    // ------------------------------------------------------------------------------------------ bookkeeping

    private static ForestSaveData Clone(ForestSaveData data) => JsonUtility.FromJson<ForestSaveData>(JsonUtility.ToJson(data));
    private static ScenarioObjectiveAchievement Clone(ScenarioObjectiveAchievement r) => JsonUtility.FromJson<ScenarioObjectiveAchievement>(JsonUtility.ToJson(r));
    private static IEnumerator Settle(int frames) { for (int i = 0; i < frames; i++) yield return null; }
    private string WorldHash() => ScenarioReferenceArchive.WorldHash(saves.CaptureData());

    private void Section(string title) { notes.Add("== " + title); Debug.Log("S1C_T05_SECTION " + title); }

    private void Check(bool ok, string name, string detail = "")
    {
        if (ok) passes++; else failures++;
        string line = (ok ? "S1C_T05_CHECK_PASS " : "S1C_T05_CHECK_FAIL ") + name + (string.IsNullOrEmpty(detail) ? "" : " " + detail);
        results.Add(line);
        Debug.Log(line);
    }

    private void OnLogMessage(string condition, string stackTrace, LogType type)
    {
        if (type != LogType.Error && type != LogType.Exception && type != LogType.Assert) return;
        if (errors.Count < 40) errors.Add(type + ": " + condition);
    }

    private void WriteReports(bool pass, Exception failure)
    {
        var sb = new StringBuilder();
        sb.AppendLine("result: " + (pass ? "PASS" : "FAIL"));
        sb.AppendLine("checks passed: " + passes + "  failed: " + failures + "  cases: " + cases + "  shots: " + shots);
        if (failure != null) sb.AppendLine("exception: " + failure);
        sb.AppendLine(); sb.AppendLine("NOTES");
        foreach (string note in notes) sb.AppendLine("- " + note);
        if (errors.Count > 0) { sb.AppendLine(); sb.AppendLine("CONSOLE ERRORS"); foreach (string e in errors) sb.AppendLine("- " + e); }
        sb.AppendLine(); sb.AppendLine("CHECKS");
        foreach (string line in results) sb.AppendLine(line);
        File.WriteAllText(Path.Combine(output, "t05-unity-result.txt"), sb.ToString());
    }
}
#endif
