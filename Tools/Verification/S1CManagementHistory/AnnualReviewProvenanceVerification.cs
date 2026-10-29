#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;
using CCF.Forestry.WorkEconomy;

// Disposable T-04 provenance-label rendered fixture, adapted from the T-03 harness.
// Copy/run/remove via run_t04_gate.py. Never install permanently. Exact executeMethod guard only.
// Real APIs: felling/planting/approval/advance/cancellation. Two approved harvest targets are made
// absent in memory to exercise real failure resolution; one resulting event reason is blanked to
// represent an old incomplete record. No disk save is written. World hash + serialized scenario
// records + counts/acknowledgement are compared before/after every browsing/expansion phase.
// UI callbacks drive the actual UI Toolkit view and renderer; OS-native pointer targeting is untested.
public static class AnnualReviewProvenanceVerification
{
    private const string ExitKey = "CCF.S1C.T04.PendingExit";

    public static void Begin()
    {
        SessionState.EraseInt(ExitKey);
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        AnnualReviewProvenanceRunner.SetGameSize(1280, 720);
        EditorApplication.isPlaying = true;
    }

    public static void Finish(bool pass)
    {
        // Closing the Editor in the same coroutine that requests a Play Mode exit can race native domain teardown.
        // SessionState survives that reload so the Editor closes only once it is safely back in Edit Mode.
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
        if (!Environment.GetCommandLineArgs().Any(a => a == "AnnualReviewProvenanceVerification.Begin")) return;
        new GameObject("S1-C T-04 provenance verification").AddComponent<AnnualReviewProvenanceRunner>();
    }
}

public sealed class AnnualReviewProvenanceRunner : MonoBehaviour
{
    private const string EarlierText = "◀ Earlier year";
    private const string LaterText = "Later year ▶";

    private string output, mode;
    private Vector2Int size = new Vector2Int(1280, 720);
    private int passes, failures, shots;
    private ScenarioOneUiRoot ui;
    private ScenarioOneManager manager;
    private ForestSaveController saves;
    private ForestEcologyController ecology;
    private ForestTreeMarkingManager marks;
    private StandaloneSessionMenu menu;
    private readonly List<string> errors = new List<string>();
    private readonly List<string> notes = new List<string>();
    private readonly List<string> results = new List<string>();
    private readonly StringBuilder gateTable = new StringBuilder();
    private readonly Dictionary<string, string> texts = new Dictionary<string, string>();

    // ---------------------------------------------------------------- Editor plumbing

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
            null, new object[] { Enum.ToObject(kind, 1), width, height, "S1C T04 " + width + "x" + height }, null);
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
        Directory.CreateDirectory(output);
        Directory.CreateDirectory(Path.Combine(output, "review-texts"));
        Directory.CreateDirectory(Path.Combine(output, "shots"));
        mode = (Environment.GetEnvironmentVariable("CCF_T02_MODE") ?? "candidate").Trim().ToLowerInvariant();
        gateTable.AppendLine("label\ttitle\tfooterVisible\tneedsAnnualReview\tcompletionAck\tteachingAck\tearlierEnabled\tlaterEnabled\tseen");
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

        if (errors.Count > 0)
        {
            failures++;
            Debug.Log("S1C_T04_CHECK_FAIL NO_CONSOLE_ERRORS " + string.Join(" | ", errors.Take(4)));
        }
        bool pass = failure == null && failures == 0;
        Application.logMessageReceived -= OnLogMessage;
        WriteReports(pass, failure);
        Debug.Log(pass
            ? $"S1C_T04_HARNESS_PASS mode={mode} checks={passes} shots={shots}"
            : $"S1C_T04_HARNESS_FAIL mode={mode} checks={passes} failures={failures} exception={failure}");
        AnnualReviewProvenanceVerification.Finish(pass);
    }

    // ---------------------------------------------------------------- the scenario

    private string scenarioBefore;
    private string hashBefore;
    private int eventsBefore, reportsBefore, yearBefore;
    private long cashBefore;
    private int cancelledId;
    private readonly List<string> matrix = new List<string>();
    private static readonly HistoryProvenance[] AllKinds = (HistoryProvenance[])Enum.GetValues(typeof(HistoryProvenance));

    // Guidance and explanation lines that are not history facts. Everything else with a body/muted style must carry a label.
    private static readonly string[] UnlabelledAllowed =
    {
        "Advance a year from the Work Plan", "Fallen trees stop contributing", "Salvage is optional", "An opening creates an opportunity",
        "An intervention is a work plan", "None yet.", "Cash at year end", "Canopy cover (objective", "Latest:", "—"
    };

    private IEnumerator Play()
    {
        yield return StartSession();
        yield return BuildFixture();
        Check(manager.HistoryYears.Count == 5, "FIVE_COMPLETED_YEARS");
        Check(manager.StormDamageInYear(5) != null, "STORM_IN_YEAR_5");
        hashBefore = WorldHash();
        scenarioBefore = JsonUtility.ToJson(manager.CaptureSaveData());
        eventsBefore = manager.ManagementEvents.Count; reportsBefore = manager.AnnualReports.Count;
        yearBefore = manager.CurrentEcologicalYear; cashBefore = manager.CashCents;
        notes.Add("world hash after fixture: " + hashBefore);
        notes.Add("Failure fixture: two approved target IDs replaced in memory before real AdvanceYear; second recorded failure reason then blanked to model absent provenance.");
        notes.Add("Storm fixture: ForceStormNextYear in year 5 through the verified Windthrow route; victims and deadwood come from the real simulation.");

        foreach (Vector2Int target in Sizes())
        {
            yield return UseSize(target);
            ui.CloseAll(); yield return Settle(4); ui.ShowReview(); yield return Settle(8);
            string tag = target.x + "x" + target.y;

            // ---- latest year (5): storm, derived trend, current objectives, session-only browsing ----
            Check(Title().Contains("Year 5 complete"), "LATEST_YEAR_OPENS_" + tag);
            yield return RecordState("latest-" + tag);
            AssertLabelling("latest-" + tag);
            Check(KindCount(HistoryProvenance.Recorded) > 0 && KindCount(HistoryProvenance.Current) > 0 && KindCount(HistoryProvenance.Derived) > 0
                && KindCount(HistoryProvenance.Reconstructed) > 0, "LATEST_HAS_RECORDED_CURRENT_DERIVED_RECONSTRUCTED_" + tag);
            Check(ui.RootElement.Q(name: "provenance-legend") != null, "LEGEND_PRESENT_" + tag);
            yield return Shot("latest-top", false);

            VisualElement storm = Modal().Q(name: "annual-storm-review");
            Check(storm != null, "STORM_CARD_PRESENT_" + tag);
            if (storm != null)
            {
                Check(KindIn(storm, HistoryProvenance.Reconstructed) >= 2 && KindIn(storm, HistoryProvenance.Recorded) == 1 && KindIn(storm, HistoryProvenance.Current) == 1
                    && KindIn(storm, HistoryProvenance.Derived) == 1,
                    "STORM_CARD_CLASSES_LATEST_" + tag, $"reconstructed={KindIn(storm, HistoryProvenance.Reconstructed)} recorded={KindIn(storm, HistoryProvenance.Recorded)} current={KindIn(storm, HistoryProvenance.Current)} derived={KindIn(storm, HistoryProvenance.Derived)}");
                Check(KindOf("m³ salvaged since this event") == HistoryProvenance.Derived && !Text(storm).Contains("remains as deadwood now ·"), "STORM_SALVAGE_AND_REMAINING_ARE_SEPARATE_LINES_" + tag);
                Check(KindOf("trees lost ·") == HistoryProvenance.Reconstructed && KindOf("Crop Trees lost") == HistoryProvenance.Recorded
                    && KindOf("map squares. This footprint") == HistoryProvenance.Reconstructed && KindOf("m³ remains as deadwood now") == HistoryProvenance.Current,
                    "STORM_LINES_ARE_CLASSIFIED_AS_SPECIFIED_" + tag);
                Check(Text(storm).Contains("rebuilt from tree-mortality and deadwood records"), "STORM_RECONSTRUCTION_NOTE_" + tag);
                yield return FocusShot("latest-storm", storm);
            }
            Check(KindOf("No work was resolved this year.") == HistoryProvenance.Derived && KindOf("No tasks failed.") == HistoryProvenance.Derived, "LATEST_WORK_STATEMENTS_ARE_DERIVED_" + tag);
            Check(KindOf("Retained usable timber (stock)") == HistoryProvenance.Current, "STOCK_IS_CURRENT_" + tag);
            Check(KindOf("Closing cash") == HistoryProvenance.Recorded, "CLOSING_CASH_RECORDED_" + tag);
            Check(KindCount(HistoryProvenance.SessionOnly) > 0, "SESSION_ONLY_BROWSING_PRESENT_" + tag);
            string session = Text(Modal());
            Check(session.Contains(HistoryProvenanceText.SessionOnlyBrowsingMissing) || session.Contains(HistoryProvenanceText.SessionOnlyBrowsingNote), "SESSION_ONLY_WORDING_" + tag);
            Check(KindOf("Browsing pressure") == HistoryProvenance.Current, "BROWSING_BAND_IS_CURRENT_" + tag);
            VisualElement trends = Modal().Query(className: "card").ToList().FirstOrDefault(c => c.Q<Label>(className: "heading") != null && c.Q<Label>(className: "heading").text == "TRENDS");
            Check(trends != null && KindIn(trends, HistoryProvenance.Derived) == 1, "TRENDS_ARE_DERIVED_" + tag);
            VisualElement objectives = Modal().Query(className: "card").ToList().FirstOrDefault(c => c.Q<Label>(className: "heading") != null && c.Q<Label>(className: "heading").text.StartsWith("OBJECTIVES"));
            Check(objectives != null && KindIn(objectives, HistoryProvenance.Current) == manager.Objectives.Count, "OBJECTIVE_STATE_LINES_ARE_CURRENT_" + tag);
            if (objectives != null) yield return FocusShot("latest-objectives", objectives);
            VisualElement history = Modal().Q(name: "management-history");
            Check(history != null && KindIn(history, HistoryProvenance.Recorded) == 0 && KindIn(history, HistoryProvenance.Derived) >= 3, "HISTORY_CARD_AGGREGATES_ARE_DERIVED_" + tag);
            Check(KindOf("MANAGEMENT HISTORY ·") == HistoryProvenance.Derived, "HISTORY_HEADING_COUNT_IS_DERIVED_" + tag);
            if (history != null) yield return FocusShot("latest-history", history);

            // ---- year 4: genuinely absent failure reason is labelled, not invented ----
            yield return GoYear(4);
            yield return RecordState("year4-" + tag);
            AssertLabelling("year4-" + tag);
            Check(KindCount(HistoryProvenance.NotRecorded) > 0 && KindOf("reason not recorded") == HistoryProvenance.NotRecorded, "YEAR4_ABSENT_REASON_IS_NOT_RECORDED_" + tag);
            Check(KindOf("Cancelled: target missing") == HistoryProvenance.Recorded && KindOf("task(s) failed") == HistoryProvenance.Derived, "YEAR4_REASON_RECORDED_COUNT_DERIVED_" + tag);
            yield return Shot("year4-not-recorded", false);
            yield return Toggle("failed");
            Check(Detail("failed").Query(className: "prov-notrecorded").ToList().Count == 1, "YEAR4_DETAIL_ENTRY_LABELLED_NOT_RECORDED_ONCE_" + tag);
            EntryRows("failed", tag, 2, new[] { "Reason: not recorded" });
            yield return RecordState("year4-open-" + tag);
            AssertLabelling("year4-open-" + tag);
            yield return FocusShot("year4-detail", Detail("failed"));
            yield return Toggle("failed");

            // ---- year 3: cancelled order has no execution method on record ----
            yield return GoYear(3);
            // ---- earlier storm year: the storm record stays, the live remaining-deadwood line does not appear ----
            VisualElement earlier = Modal().Q(name: "annual-storm-review");
            Check(earlier != null, "EARLIER_STORM_CARD_PRESENT_" + tag);
            if (earlier != null)
            {
                Check(KindIn(earlier, HistoryProvenance.Current) == 0 && KindIn(earlier, HistoryProvenance.Recorded) == 1 && KindIn(earlier, HistoryProvenance.Reconstructed) >= 2
                    && KindIn(earlier, HistoryProvenance.Derived) == 1, "EARLIER_STORM_CARD_HAS_NO_LIVE_LINE_" + tag,
                    $"current={KindIn(earlier, HistoryProvenance.Current)} recorded={KindIn(earlier, HistoryProvenance.Recorded)} reconstructed={KindIn(earlier, HistoryProvenance.Reconstructed)} derived={KindIn(earlier, HistoryProvenance.Derived)}");
                Check(!Text(earlier).Contains("remains as deadwood now"), "EARLIER_STORM_OMITS_DEADWOOD_NOW_" + tag);
                Check(KindOf("m³ salvaged since this event") == HistoryProvenance.Derived && KindOf("Year 3 ·") == HistoryProvenance.Recorded, "EARLIER_STORM_SALVAGE_DERIVED_YEAR_RECORDED_" + tag);
                yield return FocusShot("year3-earlier-storm", earlier);
            }
            yield return Toggle("cancelled");
            Check(Detail("cancelled").Query(className: "prov-notrecorded").ToList().Count == 1, "YEAR3_CANCELLED_ENTRY_NOT_RECORDED_" + tag);
            EntryRows("cancelled", tag, 1, new[] { "execution method not recorded" });
            AssertLabelling("year3-open-" + tag);
            yield return FocusShot("year3-cancelled", Detail("cancelled"));
            yield return Toggle("cancelled");

            // ---- year 1: historical view carries no live-forest line under the old date ----
            yield return GoYear(1);
            yield return RecordState("year1-" + tag);
            AssertLabelling("year1-" + tag);
            VisualElement forest = Column("FOREST");
            Check(forest != null && KindIn(forest, HistoryProvenance.Current) == 0 && KindIn(forest, HistoryProvenance.SessionOnly) == 0, "HISTORICAL_FOREST_COLUMN_HAS_NO_LIVE_OR_SESSION_LINES_" + tag);
            Check(KindOf("Retained usable timber (stock)") == null && KindOf("Browsing pressure") == null, "HISTORICAL_OMITS_LIVE_LINES_" + tag);
            Check(KindOf("Canopy ") != null, "HISTORICAL_CANOPY_LINE_LABELLED_" + tag);
            Check(KindOf("tree(s) thinned") == HistoryProvenance.Derived, "YEAR1_THINNED_COUNT_IS_DERIVED_" + tag);
            VisualElement settlement = Modal().Q(name: "annual-harvest-settlement");
            Check(settlement != null && KindIn(settlement, HistoryProvenance.Derived) == 1 && KindIn(settlement, HistoryProvenance.Recorded) == 1
                && KindOf("Timber income") == HistoryProvenance.Recorded && KindOf("harvest order(s) settled together") == HistoryProvenance.Derived, "SETTLEMENT_COUNT_DERIVED_MONEY_RECORDED_" + tag);
            yield return Toggle("done-FellTree");
            EntryRows("done-FellTree", tag, 12, new string[0]);
            yield return Toggle("done-FellTree");
            VisualElement oldObjectives = Modal().Query(className: "card").ToList().FirstOrDefault(c => c.Q<Label>(className: "heading") != null && c.Q<Label>(className: "heading").text.Contains("(current state)"));
            Check(oldObjectives != null && KindIn(oldObjectives, HistoryProvenance.Current) == manager.Objectives.Count, "HISTORICAL_OBJECTIVES_REMAIN_LABELLED_CURRENT_" + tag);
            yield return Shot("year1-historical", false);
            AssertReadOnly("GENERATED_RENDER_" + tag);
        }
        yield return OldSaves();
        notes.Add("Callback/API driven rendered check; no OS-native pointer delivery qualification.");
    }

    // Real pre-feature and recorded v22 saves from the S1-C T-05 evidence, loaded through the production typed route.
    private IEnumerator OldSaves()
    {
        string fixtures = Environment.GetEnvironmentVariable("CCF_T05_FIXTURES");
        string earlier = Environment.GetEnvironmentVariable("CCF_T05_PRIOR_EVIDENCE");
        if (string.IsNullOrEmpty(fixtures) || string.IsNullOrEmpty(earlier)) throw new InvalidOperationException("CCF_T05_FIXTURES and CCF_T05_PRIOR_EVIDENCE are required");
        string path = Path.Combine(Application.persistentDataPath, "forest-save.json");
        Check(path.StartsWith(Environment.GetEnvironmentVariable("XDG_CONFIG_HOME")), "ISOLATED_SAVE_PATH");
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        var originals = new Dictionary<string, byte[]>();
        foreach (string p in new[] { path, path + ".bak", path + ".tmp" }) originals[p] = File.Exists(p) ? File.ReadAllBytes(p) : null;
        FindFirstObjectByType<ForestPlayer>().enabled = false;
        var states = new[] {
            (name: "recorded", file: Path.Combine(earlier, "recorded-v22-year3-save.json"), kind: HistoryProvenance.Recorded, phrase: "(recorded)"),
            (name: "derived", file: Path.Combine(fixtures, "v20-y5.json"), kind: HistoryProvenance.Derived, phrase: "(derived from saved records)"),
            (name: "notrecorded", file: Path.Combine(earlier, "render-gap-v20-y5-no-year1-snapshot.json"), kind: HistoryProvenance.NotRecorded, phrase: "Year reached: not recorded") };
        try
        {
            foreach (Vector2Int target in Sizes())
            {
                yield return UseSize(target);
                string tag = target.x + "x" + target.y;
                foreach (var state in states)
                {
                    ui.CloseAll(); yield return Settle(4);
                    byte[] bytes = File.ReadAllBytes(state.file); File.WriteAllBytes(path, bytes);
                    Check(saves.CanLoad(out string problem), "OLD_" + state.name + "_CAN_LOAD", problem);
                    Check(saves.LoadData(JsonUtility.FromJson<ForestSaveData>(Encoding.UTF8.GetString(bytes)), false), "OLD_" + state.name + "_TYPED_LOAD");
                    yield return Settle(12); ui.CloseAll(); ui.ShowReview(); yield return Settle(16);
                    manager = ui.Manager;
                    string saveBefore = JsonUtility.ToJson(manager.CaptureSaveData());
                    string label = "old-" + state.name + "-" + tag;
                    yield return RecordState(label);
                    AssertLabelling(label);
                    var yearLines = Modal().Query<Label>().ToList().Where(l => l.text != null && (l.text.StartsWith("First reached") || l.text.StartsWith("Year reached"))).ToList();
                    Check(yearLines.Count > 0 && yearLines.Any(l => l.text.Contains(state.phrase)), "OLD_" + state.name + "_YEAR_LINE_PRESENT_" + tag);
                    Check(yearLines.Where(l => l.text.Contains(state.phrase)).All(l => ChipKindBefore(l) == state.kind), "OLD_" + state.name + "_YEAR_LINE_CLASS_" + tag,
                        string.Join(" | ", yearLines.Select(l => ChipKindBefore(l) + ":" + l.text)));
                    if (state.name == "notrecorded")
                        Check(yearLines.All(l => l.text == state.phrase) && yearLines.Count > 0, "OLD_NOT_RECORDED_IMPLIES_NO_YEAR_" + tag, string.Join(" | ", yearLines.Select(l => l.text)));
                    if (state.name == "derived")
                        Check(yearLines.All(l => l.text.StartsWith("First reached in year ") && l.text.EndsWith("(derived from saved records)")), "OLD_DERIVED_ONLY_FROM_RECORDS_" + tag, string.Join(" | ", yearLines.Select(l => l.text)));
                    Check(KindCount(HistoryProvenance.SessionOnly) > 0 && Text(Modal()).Contains(HistoryProvenanceText.SessionOnlyBrowsingMissing), "OLD_" + state.name + "_BROWSING_NOT_RECORDED_THIS_SESSION_" + tag);
                    if (yearLines.Count > 0)
                    {
                        ScrollView scroll = Modal().Q<ScrollView>();
                        scroll.ScrollTo(yearLines[0].parent.parent); yield return Settle(12);
                        scroll.verticalScroller.value = scroll.verticalScroller.highValue; yield return Settle(12);
                        yield return Shot("old-" + state.name + "-objectives", false, false);
                    }
                    yield return Shot("old-" + state.name + "-top", false);
                    // Browsing every year only reads: the loaded save is byte-identical afterwards.
                    for (int i = 0; i < 6 && IsEnabled(EarlierText); i++) yield return ClickButton(EarlierText);
                    Check(JsonUtility.ToJson(manager.CaptureSaveData()) == saveBefore, "OLD_" + state.name + "_BROWSING_LEAVES_SAVE_UNCHANGED_" + tag);
                    AssertLabelling(label + "-earliest");
                }
            }
        }
        finally
        {
            foreach (var original in originals)
            {
                if (original.Value != null) File.WriteAllBytes(original.Key, original.Value);
                else if (File.Exists(original.Key)) File.Delete(original.Key);
            }
        }
    }

    // ---------------------------------------------------------------- provenance reading

    private static HistoryProvenance? KindOfChip(VisualElement chip)
    {
        foreach (HistoryProvenance kind in AllKinds)
            if (chip.ClassListContains("prov-" + kind.ToString().ToLowerInvariant())) return kind;
        return null;
    }

    private IEnumerable<Label> VisibleChips(VisualElement scope) => scope.Query<Label>(className: "prov").ToList().Where(Visible);
    private int KindCount(HistoryProvenance kind) => KindIn(Modal(), kind);
    private int KindIn(VisualElement scope, HistoryProvenance kind) => VisibleChips(scope).Count(c => KindOfChip(c) == kind);

    // The class of the visible line whose text contains the phrase: the chip shares its row (or table-row) with that text.
    private HistoryProvenance? KindOf(string phrase)
    {
        foreach (Label label in Modal().Query<Label>().ToList())
        {
            if (label.ClassListContains("prov") || string.IsNullOrEmpty(label.text) || !label.text.Contains(phrase) || !Visible(label)) continue;
            HistoryProvenance? kind = ChipKindBefore(label);
            if (kind.HasValue) return kind;
        }
        return null;
    }

    private static HistoryProvenance? ChipKindBefore(Label text)
    {
        // The label shares a row with its text, or (name/value lines) sits one level up beside the wrapper holding the text.
        VisualElement row = text.parent;
        for (int depth = 0; depth < 3 && row != null && !row.ClassListContains("card") && !row.ClassListContains("column"); depth++, row = row.parent)
            foreach (VisualElement child in row.Children())
                if (child.ClassListContains("prov")) return KindOfChip(child);
        return null;
    }

    private VisualElement Column(string heading) =>
        Modal().Query(className: "column").ToList().FirstOrDefault(c => c.Q<Label>(className: "heading") != null && c.Q<Label>(className: "heading").text == heading);

    // Structure rules: one label per row, text clear of its label, no clipped wrapped text; every other body/muted line is
    // guidance on the reviewed allowlist. Also writes the class/text matrix for the handoff.
    private void AssertLabelling(string label)
    {
        VisualElement modal = Modal();
        var lines = new StringBuilder();
        int rowsWithMany = 0, overlaps = 0, clipped = 0;
        var clippedText = new List<string>();
        foreach (Label chip in VisibleChips(modal))
        {
            VisualElement row = chip.parent;
            if (row.Children().Count(c => c.ClassListContains("prov")) != 1) rowsWithMany++;
            foreach (Label text in row.Query<Label>().ToList().Where(t => !t.ClassListContains("prov") && !t.ClassListContains("chip") && Visible(t)))
            {
                if (text.worldBound.xMin < chip.worldBound.xMax - 1.5f && text.worldBound.yMin < chip.worldBound.yMax - 1.5f && text.worldBound.yMax > chip.worldBound.yMin + 1.5f) overlaps++;
                // Money values are single-line (white-space: nowrap); the panel-containment audit covers them, not wrap measurement.
                Vector2 measured = text.MeasureTextSize(text.text, text.contentRect.width, VisualElement.MeasureMode.Exactly, 0, VisualElement.MeasureMode.Undefined);
                if (!text.ClassListContains("line-value") && measured.y > text.contentRect.height + 1.5f) { clipped++; clippedText.Add($"{Short(text.text)} measured={measured.y:0.0} content={text.contentRect.height:0.0} w={text.contentRect.width:0.0}"); }
                lines.AppendLine(HistoryProvenanceText.Label(KindOfChip(chip).Value) + "\t" + text.text.Replace('\n', ' '));
            }
        }
        Check(rowsWithMany == 0, "ONE_LABEL_PER_ROW_" + label);
        Check(overlaps == 0, "LABEL_DOES_NOT_OVERLAP_TEXT_" + label);
        Check(clipped == 0, "LABELLED_TEXT_NOT_CLIPPED_" + label, string.Join(" | ", clippedText.Take(4)));

        string tutorialHint = manager.TutorialHint ?? "";
        var unlabelled = new List<string>();
        foreach (Label text in modal.Query<Label>().ToList())
        {
            if (!Visible(text) || string.IsNullOrEmpty(text.text) || text.ClassListContains("prov") || text is Button) continue;
            if (!(text.ClassListContains("body") || text.ClassListContains("muted"))) continue;
            if (text.GetFirstAncestorOfType<Button>() != null) continue;
            if (AncestorWithClass(text, "modal-footer") != null || AncestorWithClass(text, "modal-header") != null) continue;
            if (ChipKindBefore(text).HasValue) continue;
            if (UnlabelledAllowed.Any(a => text.text.StartsWith(a)) || text.text == tutorialHint) continue;
            unlabelled.Add(text.text);
        }
        Check(unlabelled.Count == 0, "NO_UNLABELLED_HISTORY_LINES_" + label, string.Join(" || ", unlabelled.Take(3)));
        File.WriteAllText(Path.Combine(output, "review-texts", "matrix-" + label + ".tsv"), lines.ToString());
        matrix.Add(label + "\t" + AllKinds.Select(k => HistoryProvenanceText.Label(k) + "=" + KindCount(k)).Aggregate((a, b) => a + " " + b));
    }

    // Every drill-down entry: a RECORDED title row and a RECORDED facts row, plus exactly the NOT RECORDED row that names the missing
    // values (never a chip implying the whole mixed entry has one provenance).
    private void EntryRows(string key, string tag, int expectedEntries, string[] missingTexts)
    {
        var entries = Detail(key).Children().Where(c => !(c is Label)).ToList();
        Check(entries.Count == expectedEntries, "DETAIL_ENTRY_COUNT_" + key + "_" + tag, entries.Count.ToString());
        int withMissing = 0;
        foreach (VisualElement entry in entries)
        {
            Check(KindIn(entry, HistoryProvenance.Recorded) >= 2, "ENTRY_HAS_RECORDED_TITLE_AND_FACTS_ROWS_" + key + "_" + tag);
            var notRecorded = entry.Query(className: "prov-notrecorded").ToList();
            if (notRecorded.Count > 0)
            {
                withMissing++;
                string rowText = string.Join(" | ", notRecorded.Select(c => string.Join(" ", c.parent.Query<Label>().ToList().Where(l => !l.ClassListContains("prov")).Select(l => l.text))));
                Check(notRecorded.Count == 1 && missingTexts.All(m => rowText.Contains(m)), "ENTRY_NOT_RECORDED_ROW_NAMES_THE_MISSING_VALUE_" + key + "_" + tag, rowText);
                Check(!entry.Query<Label>().ToList().Where(l => !l.ClassListContains("prov") && ChipKindBefore(l) == HistoryProvenance.Recorded).Any(l => l.text.Contains("not recorded")),
                    "RECORDED_ROWS_CARRY_NO_MISSING_TEXT_" + key + "_" + tag);
            }
        }
        Check(withMissing == (missingTexts.Length > 0 ? 1 : 0), "ENTRIES_WITH_MISSING_VALUES_" + key + "_" + tag, withMissing.ToString());
    }

    private static VisualElement AncestorWithClass(VisualElement element, string className)
    {
        for (VisualElement p = element.parent; p != null; p = p.parent)
            if (p.ClassListContains(className)) return p;
        return null;
    }

    private IEnumerator BuildFixture()
    {
        yield return PlanFelling(12); AdvanceWithWork("harvest year 1");
        yield return PlanPlanting(true, WorkExecutionMethod.LandownerSimulated);
        yield return PlanPlanting(false, WorkExecutionMethod.Contractor);
        AdvanceWithWork("planting year 2");
        // Year 3: a forced storm too, so an EARLIER storm year can be browsed while a later year is the latest review.
        ecology.StormModelVersion = 1;
        ecology.UseStormCalibrationForVerification(new StormCalibration(0, new[] { .02f, .06f, .18f }, new[] { 1f, 1f, 1f }));
        ecology.ForceStormNextYear(.7f, 135);
        Check(manager.AdvanceYear(), "FIXTURE_STORM_YEAR3");
        StormDamageSummary earlierStorm = manager.StormDamageInYear(3);
        Check(earlierStorm != null && earlierStorm.TreesLost > 0, "FIXTURE_REAL_EARLIER_STORM_SUMMARY", earlierStorm == null ? "none" : "lost=" + earlierStorm.TreesLost);
        yield return PlanFelling(1);
        var cancel = manager.WorkOrders.Last(o => o.status == ScenarioWorkStatus.Pending);
        cancelledId = cancel.workOrderId;
        Check(manager.RemovePendingOrder(cancelledId), "FIXTURE_CANCEL_REAL_API_AT_YEAR3");
        yield return PlanFelling(2);
        Check(manager.ApprovePendingWork(), "FIXTURE_APPROVE_FAILURES");
        var failures = manager.WorkOrders.Where(o => o.status == ScenarioWorkStatus.Approved).ToList();
        Check(failures.Count == 2, "FIXTURE_TWO_APPROVED_FAILURE_TARGETS");
        for (int i = 0; i < failures.Count; i++) failures[i].targetTreeId = "T03-MISSING-" + i;
        Check(manager.AdvanceYear(), "FIXTURE_RESOLVE_INVALID_TARGETS_YEAR4");
        var failed = manager.ManagementEvents.Where(e => e.year == 4 && e.eventType == ScenarioManagementEventType.WorkResolved).ToList();
        Check(failed.Count == 2 && failed.All(e => e.outcome == ScenarioManagementOutcome.Failed && e.failureReason.Length > 0), "REAL_FAILURE_RESOLUTION_RECORDED");
        failed.Last().failureReason = "";
        // Year 5: a forced storm (the verified route of the Windthrow UI gate) so the review holds a real reconstructed storm summary.
        ecology.ForceStormNextYear(.7f, 135);
        Check(manager.AdvanceYear(), "FIXTURE_STORM_YEAR5");
        StormDamageSummary storm = manager.StormDamageInYear(5);
        Check(storm != null && storm.TreesLost > 0, "FIXTURE_REAL_STORM_SUMMARY", storm == null ? "none" : "lost=" + storm.TreesLost);
        yield return Settle(8);
    }

    private VisualElement Detail(string key) => Modal().Q(name: "work-detail-" + key);
    private bool IsOpen(string key) => Detail(key) != null && Detail(key).resolvedStyle.display == DisplayStyle.Flex;
    private static string Text(VisualElement element) => string.Join("\n", element.Query<TextElement>().ToList().Where(Visible).Select(e => e.text));
    private IEnumerator Toggle(string key)
    {
        Button button = Modal().Q<Button>("work-detail-toggle-" + key);
        Check(button != null && Detail(key) != null, "NAMED_TOGGLE_AND_BODY_" + key);
        if (button == null) throw new InvalidOperationException("missing toggle " + key);
        bool before = IsOpen(key);
        yield return ClickButton(button, true);
        Check(IsOpen(key) != before, "TOGGLE_CHANGED_DISPLAY_" + key);
    }
    private IEnumerator GoYear(int target)
    {
        for (int guard = 0; guard < 10; guard++)
        {
            string title = Title();
            int current = int.Parse(title.Split(new[] { "Year " }, StringSplitOptions.None)[1].Split(' ')[0]);
            if (current == target) yield break;
            yield return ClickButton(current > target ? EarlierText : LaterText);
        }
        throw new InvalidOperationException("cannot navigate to year " + target);
    }
    private void AssertReadOnly(string phase)
    {
        Check(WorldHash() == hashBefore && JsonUtility.ToJson(manager.CaptureSaveData()) == scenarioBefore
            && manager.ManagementEvents.Count == eventsBefore && manager.AnnualReports.Count == reportsBefore
            && manager.CurrentEcologicalYear == yearBefore && manager.CashCents == cashBefore && !manager.AnnualReviewSeen,
            "WORLD_SAVE_COUNTS_ACK_UNCHANGED_" + phase);
    }
    private IEnumerator FocusShot(string label, VisualElement target)
    {
        var scroll = Modal().Q<ScrollView>();
        scroll.ScrollTo(target); yield return Settle(8);
        Check(target.worldBound.Overlaps(scroll.contentViewport.worldBound), "SCROLL_REACHES_LAST_ENTRY_" + label + size);
        yield return Shot(label, false, false);
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
        notes.Add("Application focused at start: " + Application.isFocused);

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
        marks = ui.Marking != null ? ui.Marking : UnityEngine.Object.FindFirstObjectByType<ForestTreeMarkingManager>();
        if (manager == null || saves == null || ecology == null || marks == null)
            throw new InvalidOperationException("manager, save controller, ecology or marking manager missing");
        Check(manager.AnnualReports.Count == 0 && manager.HistoryYears.Count == 0, "NEW_GAME_HAS_NO_COMPLETED_YEARS");
        notes.Add("stand geometry model " + ecology.StandGeometryModelVersion + "; trees at start " + Trees().Length);
        yield return RecordState("before-first-year");
    }

    private IEnumerator PlanFelling(int count)
    {
        manager.PlanningFellingOutcome = FellingMaterialOutcome.SellAndExtract;
        ForestTree[] trees = Trees();
        if (trees.Length < count) throw new InvalidOperationException("not enough thinnable trees");
        for (int i = 0; i < count; i++) marks.Mark(trees[i], TreeMarkType.Fell, false);
        manager.AddMarkedTreesToWorkPlan();
        yield return null;
    }

    private IEnumerator PlanPlanting(bool shelter, WorkExecutionMethod method = WorkExecutionMethod.LandownerSimulated)
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

    // ---------------------------------------------------------------- UI reading and driving

    private VisualElement Modal() => ui.RootElement.Q(className: "modal-review");

    private string Title()
    {
        VisualElement modal = Modal();
        Label title = modal == null ? null : modal.Q<Label>(className: "title");
        return title == null ? "" : title.text;
    }

    private Button FindButton(string text) => Modal().Query<Button>().ToList().FirstOrDefault(b => b.text == text);

    private bool IsEnabled(string text) { Button b = FindButton(text); return b != null && b.enabledInHierarchy; }

    private bool FooterVisible()
    {
        Button ack = ui.RootElement.Q<Button>("annual-review-acknowledge");
        return ack != null && Visible(ack);
    }

    private static bool Visible(VisualElement element)
    {
        if (element == null) return false;
        for (VisualElement p = element; p != null; p = p.parent)
            if (p.resolvedStyle.display == DisplayStyle.None || p.resolvedStyle.visibility == Visibility.Hidden) return false;
        return element.resolvedStyle.width > 0.5f && element.resolvedStyle.height > 0.5f;
    }

    private IEnumerator ClickButton(string text)
    {
        Button button = FindButton(text);
        Check(button != null, "BUTTON_PRESENT_" + text);
        if (button != null) yield return ClickButton(button, true);
    }

    private IEnumerator ClickButton(Button button, bool requireEnabled)
    {
        if (requireEnabled) Check(button.enabledInHierarchy, "BUTTON_ENABLED_" + button.text);
        MethodInfo invoke = typeof(Clickable).GetMethod("Invoke", BindingFlags.Instance | BindingFlags.NonPublic);
        if (invoke == null) throw new MissingMethodException("Clickable.Invoke is unavailable in this Unity version");
        invoke.Invoke(button.clickable, new object[] { null });
        yield return Settle(6);
    }

    private IEnumerator RecordState(string label)
    {
        yield return Settle(4);
        VisualElement modal = Modal();
        var sb = new StringBuilder();
        sb.AppendLine("TITLE: " + Title());
        if (modal != null)
            foreach (TextElement element in modal.Query<TextElement>().ToList())
                if (!string.IsNullOrEmpty(element.text) && Visible(element))
                    sb.AppendLine((element is Button ? "[button] " : "") + element.text);
        texts[label] = sb.ToString();
        File.WriteAllText(Path.Combine(output, "review-texts", label + ".txt"), texts[label]);

        bool teaching = ui.Learning != null && ui.Learning.NeedsReviewAcknowledgement;
        bool footer = FooterVisible(), needs = ui.NeedsAnnualReview, completion = manager.NeedsCompletionReviewAcknowledgement;
        bool historical = Title().Contains("earlier year");
        gateTable.AppendLine(string.Join("\t", label, Title(), footer, needs, completion, teaching,
            modal != null && IsEnabled(EarlierText), modal != null && IsEnabled(LaterText), manager.AnnualReviewSeen));
        // The acknowledgement footer follows the existing gate rule: any of the three flags, and only on the newest review.
        bool reviewOpen = ui.CurrentScreen == ScenarioOneUiRoot.UiScreen.Review && manager.AnnualReports.Count > 0;
        if (reviewOpen)
            Check(footer == ((needs || completion || teaching) && !historical), "GATE_RULE_HOLDS_" + label,
                $"footer={footer} needs={needs} completion={completion} teaching={teaching} historical={historical}");
    }

    // ---------------------------------------------------------------- rendering

    private IEnumerator UseSize(Vector2Int target)
    {
        size = target;
        SetGameSize(target.x, target.y);
        for (int frame = 0; frame < 120 && (Screen.width != target.x || Screen.height != target.y); frame++) yield return null;
        yield return Settle(24);
        Check(Screen.width == target.x && Screen.height == target.y, $"TARGET_RENDER_SIZE_{target.x}x{target.y}", $"actual={Screen.width}x{Screen.height}");
    }

    private IEnumerator Shot(string name, bool scrolledToEnd, bool reset = true)
    {
        ScrollView scroll = Modal().Q<ScrollView>();
        if (scroll != null && reset)
        {
            scroll.verticalScroller.value = scrolledToEnd ? scroll.verticalScroller.highValue : scroll.verticalScroller.lowValue;
            yield return Settle(6);
        }
        yield return new WaitForEndOfFrame();
        Texture2D image = ScreenCapture.CaptureScreenshotAsTexture();
        if (image == null) { Check(false, "SHOT_" + name, "no rendered capture"); yield break; }
        Check(image.width == size.x && image.height == size.y, "CAPTURE_SIZE_" + name, $"capture={image.width}x{image.height} target={size.x}x{size.y}");
        Check(!LooksBlank(image), "SHOT_NOT_BLANK_" + name);
        string file = $"{name}-{size.x}x{size.y}.png";
        File.WriteAllBytes(Path.Combine(output, "shots", file), image.EncodeToPNG());
        Destroy(image);
        shots++;
        AuditLayout(name);
        Debug.Log($"S1C_T04_SHOT {file}");
    }

    // Horizontal containment of every visible text element and no overlap between the title and the header buttons.
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
        Check(overflow.Count == 0, $"[{size.x}x{size.y}]_TEXT_INSIDE_PANEL_" + name, string.Join(" | ", overflow.Take(3)));

        foreach (Button toggle in modal.Query<Button>().ToList().Where(b => b.name.StartsWith("work-detail-toggle-") && Visible(b)))
        {
            Vector2 measured = toggle.MeasureTextSize(toggle.text, toggle.contentRect.width, VisualElement.MeasureMode.Exactly,
                0, VisualElement.MeasureMode.Undefined);
            Check(measured.y <= toggle.contentRect.height + 1.5f && toggle.worldBound.xMin >= toggle.parent.worldBound.xMin
                && toggle.worldBound.xMax <= toggle.parent.worldBound.xMax + 1.5f,
                $"[{size.x}x{size.y}]_DETAIL_LABEL_FITS_{name}_{toggle.name}",
                $"textHeight={measured.y:0.0} contentHeight={toggle.contentRect.height:0.0} width={toggle.contentRect.width:0.0}");
        }
        Label title = modal.Q<Label>(className: "title");
        var clashes = new List<string>();
        if (title != null)
        {
            Rect t = Shrink(title.worldBound, 1f);
            foreach (Button b in modal.Query<Button>().ToList())
                if (Visible(b) && b.parent != null && b.parent.ClassListContains("screen-actions") && t.Overlaps(Shrink(b.worldBound, 1f))) clashes.Add(b.text);
        }
        Check(clashes.Count == 0, $"[{size.x}x{size.y}]_TITLE_CLEAR_OF_HEADER_BUTTONS_" + name, string.Join(",", clashes));
    }

    private static Rect Shrink(Rect r, float by) => new Rect(r.x + by, r.y + by, Mathf.Max(0f, r.width - 2f * by), Mathf.Max(0f, r.height - 2f * by));
    private static string Short(string text) => text.Length <= 40 ? text : text.Substring(0, 40) + "…";

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

    // ---------------------------------------------------------------- bookkeeping

    private static IEnumerator Settle(int frames) { for (int i = 0; i < frames; i++) yield return null; }

    private string WorldHash()
    {
        if (saves == null) throw new InvalidOperationException("save controller missing for world hash");
        return ScenarioReferenceArchive.WorldHash(saves.CaptureData());
    }

    private void Check(bool ok, string name, string detail = "")
    {
        if (ok) passes++; else failures++;
        string line = (ok ? "S1C_T04_CHECK_PASS " : "S1C_T04_CHECK_FAIL ") + name + (string.IsNullOrEmpty(detail) ? "" : " " + detail);
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
        File.WriteAllText(Path.Combine(output, "gate-states.tsv"), gateTable.ToString());
        var sb = new StringBuilder();
        sb.AppendLine("mode: " + mode);
        sb.AppendLine("result: " + (pass ? "PASS" : "FAIL"));
        sb.AppendLine("checks passed: " + passes + "  failed: " + failures + "  shots: " + shots);
        if (failure != null) sb.AppendLine("exception: " + failure);
        sb.AppendLine(); sb.AppendLine("NOTES");
        foreach (string note in notes) sb.AppendLine("- " + note);
        if (errors.Count > 0) { sb.AppendLine(); sb.AppendLine("CONSOLE ERRORS"); foreach (string e in errors) sb.AppendLine("- " + e); }
        sb.AppendLine(); sb.AppendLine("CHECKS");
        sb.AppendLine(); sb.AppendLine("PROVENANCE COUNTS PER STATE");
        foreach (string line in matrix) sb.AppendLine(line);
        sb.AppendLine(); sb.AppendLine("CHECK LIST");
        foreach (string line in results) sb.AppendLine(line);
        File.WriteAllText(Path.Combine(output, "t04-result.txt"), sb.ToString());
    }
}
#endif
