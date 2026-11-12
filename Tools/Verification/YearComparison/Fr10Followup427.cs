#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

public static class Fr10Followup427
{
    public static void Begin()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        EditorApplication.isPlaying = true;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Install()
    {
        if (Environment.GetCommandLineArgs().Contains("Fr10Followup427.Begin"))
            new GameObject("Disposable 427 follow-up").AddComponent<Fr10FollowupRunner427>();
    }
}

public sealed class Fr10FollowupRunner427 : MonoBehaviour
{
    ScenarioOneUiRoot ui;
    ScenarioOneManager manager;
    ForestSaveController saves;
    AnnualReviewView review;
    D073Runner inherited;
    Fr10RenderedRunner oracle;
    ForestSaveData history;
    string output, probe, variant;
    int checks, failures, caseFailures;
    double previousClick = -1;
    object previousCard;
    readonly List<string> results = new List<string>();
    readonly RackAuthoringNativeInput native = new RackAuthoringNativeInput();
    static readonly BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;
    static object Get(object target, string name) => target.GetType().GetField(name, Private).GetValue(target);
    static void Set(object target, string name, object value) => target.GetType().GetField(name, Private).SetValue(target, value);
    IEnumerator Invoke(object target, string name, params object[] arguments) => (IEnumerator)target.GetType().GetMethod(name, Private).Invoke(target, arguments);
    ScrollView Scroll => (ScrollView)Get(review, "scroll");
    Foldout Fold => review.Root.Q<Foldout>("annual-year-comparison-species");
    ForestSaveData Clone(ForestSaveData state) => JsonUtility.FromJson<ForestSaveData>(JsonUtility.ToJson(state));
    string Canon(ForestSaveData state) { state = Clone(state); ScenarioReferenceArchive.Canonicalize(state); return JsonUtility.ToJson(state); }
    IEnumerator Settle(float seconds = .3f) { yield return new WaitForSecondsRealtime(seconds); for (int frame = 0; frame < 4; frame++) yield return null; }

    void Log(string message)
    {
        string line = "FOLLOWUP_TRACE " + variant + " " + Screen.width + "x" + Screen.height + " " + probe + " time=" + Time.realtimeSinceStartupAsDouble.ToString("F6") + " " + message;
        File.AppendAllText(Path.Combine(output, "click-trace.txt"), line + "\n");
        Debug.Log(line);
    }

    string Element(VisualElement element) => element == null ? "null" : element.GetType().Name + "#" + element.name + "[" + string.Join(",", element.GetClasses()) + "]";

    void Snapshot(string stage, Vector2 point)
    {
        var fold = Fold;
        var card = review.Root.Q("annual-year-comparison");
        Log(stage + " value=" + fold?.value + " pick=" + Element(review.Root.panel.Pick(point)) + " point=" + point.ToString("F4")
            + " toggleBound=" + fold?.Q<Toggle>()?.worldBound.ToString("F4") + " foldId=" + (fold == null ? 0 : RuntimeHelpers.GetHashCode(fold))
            + " cardId=" + (card == null ? 0 : RuntimeHelpers.GetHashCode(card)) + " rebuiltSinceClick=" + (previousCard != null && !ReferenceEquals(previousCard, card))
            + " elapsedSinceClick=" + (previousClick < 0 ? "none" : (Time.realtimeSinceStartupAsDouble - previousClick).ToString("F6"))
            + " scroll=" + Scroll.scrollOffset.ToString("F4"));
    }

    void InstallTracing()
    {
        review.Root.RegisterCallback<PointerDownEvent>(eventData =>
        {
            Snapshot("pointer-down target=" + Element(eventData.target as VisualElement) + " clickCount=" + eventData.clickCount, eventData.position);
            previousClick = Time.realtimeSinceStartupAsDouble;
            previousCard = review.Root.Q("annual-year-comparison");
        }, TrickleDown.TrickleDown);
        review.Root.RegisterCallback<PointerUpEvent>(eventData =>
        {
            Vector2 point = eventData.position;
            Snapshot("pointer-up target=" + Element(eventData.target as VisualElement), point);
            review.Root.schedule.Execute(() => Snapshot("after-native-click", point));
        });
        review.Root.RegisterCallback<ChangeEvent<bool>>(eventData => Log("value-change target=" + Element(eventData.target as VisualElement)
            + " before=" + eventData.previousValue + " after=" + eventData.newValue), TrickleDown.TrickleDown);
    }

    void Check(bool passed, string name, string detail = "")
    {
        checks++;
        if (!passed) failures++;
        string line = "FOLLOWUP_CHECK " + (passed ? "PASS " : "FAIL ") + name + " | " + detail;
        results.Add(line); Debug.Log(line);
    }

    IEnumerator Case(string name, IEnumerator action)
    {
        int before = failures, count = checks;
        var stack = new Stack<IEnumerator>(); stack.Push(action);
        while (stack.Count > 0)
        {
            bool more = false; object next = null; Exception failure = null;
            try { more = stack.Peek().MoveNext(); if (more) next = stack.Peek().Current; }
            catch (Exception exception) { failure = exception; }
            if (failure != null) { Check(false, name + " exception", failure.ToString()); break; }
            if (!more) stack.Pop();
            else if (next is IEnumerator nested) stack.Push(nested);
            else yield return next;
        }
        bool passed = failures == before;
        if (!passed) caseFailures++;
        string line = "FOLLOWUP_CASE " + name + " " + (passed ? "PASS" : "FAIL") + " checks=" + (checks - count);
        results.Add(line); Debug.Log(line);
        File.WriteAllLines(Path.Combine(output, "followup-results.txt"), results);
    }

    IEnumerator Start()
    {
        for (int frame = 0; frame < 12; frame++) yield return null;
        output = Environment.GetEnvironmentVariable("CCF_DIAG_DIR"); Directory.CreateDirectory(output);
        variant = Environment.GetEnvironmentVariable("CCF_FR10_NEGATIVE") == "1" ? "base77a56a44" : "candidate339517ed";
        ui = FindFirstObjectByType<ScenarioOneUiRoot>(); manager = ui.Manager;
        saves = FindFirstObjectByType<ForestSaveController>(); review = (AnnualReviewView)Get(ui, "review");
        Debug.Log("FOLLOWUP_ISOLATION persistentDataPath=" + Application.persistentDataPath + " XDG_CONFIG_HOME=" + Environment.GetEnvironmentVariable("XDG_CONFIG_HOME"));
        Check(Application.persistentDataPath.StartsWith("/media/jer/Files/CCF/Verification/D073-FR10-427-20261112/followup1/"), "isolated save and config");
        var dormant = new GameObject("Dormant unchanged assertion libraries"); dormant.SetActive(false);
        inherited = dormant.AddComponent<D073Runner>(); oracle = dormant.AddComponent<Fr10RenderedRunner>();
        foreach (object library in new object[] { inherited, oracle })
        {
            Set(library, "ui", ui); Set(library, "manager", manager); Set(library, "saves", saves); Set(library, "output", output);
        }
        Set(oracle, "review", review); Set(oracle, "inherited", inherited);
        history = JsonUtility.FromJson<ForestSaveData>(File.ReadAllText(Environment.GetEnvironmentVariable("CCF_D073_RESUME_HISTORY")));
        Set(oracle, "history", history);
        InstallTracing();
        bool diagnostic = Environment.GetEnvironmentVariable("CCF_427_PHASE") == "diagnostic";
        foreach (var size in new[] { new Vector2Int(1280,720), new Vector2Int(1920,1080) })
        {
            Cadence408GameSize.SetGameSize(size.x, size.y); yield return Settle(1.1f);
            Check(Screen.width == size.x && Screen.height == size.y, "exact render dimensions", Screen.width + "x" + Screen.height);
            if (diagnostic) yield return Case(size + "/foldout-diagnostic", Diagnostic());
            else
            {
                yield return Case(size + "/required-d-inherited", Invoke(inherited, "NativeSize", Clone(history), size));
                yield return Case(size + "/required-d-corrected-close", CorrectedClose());
                foreach (bool sources in new[] { true, false })
                    yield return Case(size + "/distinct-ID-ambiguous-event-" + (sources ? "on" : "off"), AmbiguousEvent(sources));
            }
        }
        File.WriteAllLines(Path.Combine(output, "inherited-checks.txt"), (List<string>)Get(inherited, "evidence"));
        File.WriteAllLines(Path.Combine(output, "oracle-checks.txt"), (List<string>)Get(oracle, "results"));
        int oracleFailures = (int)Get(oracle, "failures");
        Debug.Log("FOLLOWUP_DONE checks=" + checks + " failures=" + failures + " caseFailures=" + caseFailures + " oracleChecks=" + Get(oracle, "checks") + " oracleFailures=" + oracleFailures + " inheritedChecks=" + Get(inherited, "checks"));
        File.WriteAllLines(Path.Combine(output, "followup-results.txt"), results);
        EditorApplication.ExitPlaymode(); EditorApplication.Exit(failures == 0 && oracleFailures == 0 ? 0 : 1);
    }

    IEnumerator Fresh()
    {
        yield return Invoke(inherited, "Restore", Clone(history)); ui.ShowReview(); review.TryShowYear(3); yield return Settle(.18f);
    }

    IEnumerator Position(bool opened, float fraction = 0)
    {
        Fold.value = opened; yield return Settle();
        Scroll.ScrollTo(Fold.Q<Toggle>()); yield return Settle();
        var viewport = Scroll.contentViewport.worldBound;
        float desired = Mathf.Lerp(viewport.yMin + 25, viewport.yMax - 25, fraction);
        Scroll.verticalScroller.value += Fold.Q<Toggle>().worldBound.center.y - desired;
        yield return Settle();
    }

    IEnumerator Click(bool moveAway, bool burst = false)
    {
        var toggle = Fold.Q<Toggle>(); var point = toggle.worldBound.center; var root = ui.RootElement.worldBound;
        var view = EditorWindow.GetWindow(typeof(Editor).Assembly.GetType("UnityEditor.GameView"));
        var property = view.GetType().GetProperty("targetInParent", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        Rect rectangle = (Rect)property.GetValue(view);
        int desktopX = Mathf.RoundToInt(view.position.x + rectangle.x + point.x / root.width * rectangle.width);
        int desktopY = Mathf.RoundToInt(view.position.y + rectangle.y + point.y / root.height * rectangle.height);
        if (moveAway)
        {
            yield return native.Send(new RackAuthoringNativeInput.Op { op = "absolute", x = desktopX, y = desktopY - 45 });
            yield return Settle(1.1f);
        }
        Vector2 mapped = new Vector2((desktopX - view.position.x - rectangle.x) / rectangle.width * root.width,
            (desktopY - view.position.y - rectangle.y) / rectangle.height * root.height);
        Log("request desktop=" + desktopX + "," + desktopY + " gameTarget=" + rectangle.ToString("F4") + " window=" + view.position.ToString("F4") + " moveAway=" + moveAway + " burst100ms=" + burst);
        Snapshot("before-request rounded-panel-point", mapped);
        yield return native.Send(new RackAuthoringNativeInput.Op { op = "absolute", x = desktopX, y = desktopY }, new RackAuthoringNativeInput.Op { op = burst ? "burst" : "click", button = 1 });
        yield return Settle(.18f);
        Snapshot("after-request rounded-panel-point", mapped);
    }

    IEnumerator Diagnostic()
    {
        probe = "original-sequence"; yield return Fresh();
        Scroll.ScrollTo(Fold.Q<Toggle>()); yield return Settle(.18f);
        Log("original-start value=" + Fold.value);
        if (!Fold.value) yield return Click(false);
        yield return Click(false);
        Log("original-end expectedClosed=True actualClosed=" + !Fold.value);
        probe = "single-closed-open"; yield return Position(false); yield return Click(true);
        Check(Fold.value, "human-paced closed to open " + variant);
        probe = "single-open-close"; yield return Settle(1.1f); yield return Click(true);
        Check(!Fold.value, "human-paced open to closed " + variant);
        probe = "two-clicks-100ms"; yield return Position(false); yield return Click(true, true);
        Log("burst-end value=" + Fold.value);
        foreach (float fraction in new[] { 0f, .5f, 1f })
        {
            probe = "scroll-position-" + fraction; yield return Position(false, fraction); yield return Click(true);
            Check(Fold.value, "scroll-position opens " + fraction + " " + variant);
            yield return Settle(1.1f); yield return Click(true);
            Check(!Fold.value, "scroll-position closes " + fraction + " " + variant);
        }
        yield return Invoke(oracle, "Screenshot", "diagnostic-closed");
    }

    IEnumerator CorrectedClose()
    {
        probe = "corrected-close"; yield return Fresh(); yield return Position(false, .5f);
        string before = Canon(saves.CaptureData());
        Check(!Fold.value, "corrected probe verifies closed starting state");
        yield return Click(true); Check(Fold.value, "corrected native single click opens");
        yield return Settle(1.1f); yield return Click(true); Check(!Fold.value, "corrected native single click closes");
        Check(before == Canon(saves.CaptureData()), "corrected native close exact read-only world");
        yield return Invoke(oracle, "Screenshot", "corrected-native-closed");
        for (int step = 0; (int)Get(review,"viewYear") >= 0 && step < manager.HistoryYears.Count; step++)
            yield return Invoke(inherited, "NativeClick", review.Root.Query<Button>().ToList().First(button => button.text == "Later year ▶"));
        Check((int)Get(review, "viewYear") == -1, "corrected native returns follow-latest");
    }

    IEnumerator AmbiguousEvent(bool sources)
    {
        probe = "distinct-ID-ambiguous-event";
        var state = Clone(history);
        int selected = state.scenarioOne.annualReports.First().year;
        var original = state.scenarioOne.managementEvents.First(entry => entry.year == selected && entry.eventType == ScenarioManagementEventType.WorkResolved);
        var duplicate = JsonUtility.FromJson<ScenarioManagementEvent>(JsonUtility.ToJson(original));
        duplicate.eventId = state.scenarioOne.managementEvents.Max(entry => entry.eventId) + 1;
        state.scenarioOne.nextManagementEventId = duplicate.eventId + 1;
        state.scenarioOne.managementEvents.Add(duplicate);
        Check(original.eventId != duplicate.eventId && original.workOrderId == duplicate.workOrderId, "distinct event IDs, repeated resolved order identity", original.eventId + "/" + duplicate.eventId + " order=" + original.workOrderId);
        yield return Invoke(inherited, "Restore", Clone(state));
        var card = ScenarioOneYearComparisonText.Describe(ScenarioOneManagementFollowonReadout.Build(manager.HistoryYears, selected), selected, ScenarioOneUiFacts.SpeciesName);
        Check(card.Rows.Any(row => row.Key == "work-ambiguous" && row.Text.StartsWith("2 work record(s)")), "valid loaded world exercises exact ambiguity");
        Set(oracle, "history", state);
        int before = (int)Get(oracle, "failures");
        yield return Invoke(oracle, "Rendered", "full", sources);
        Check((int)Get(oracle, "failures") == before, "unchanged rendered oracle on valid distinct-ID fixture sources=" + sources);
        var warning = review.Root.Q<Label>("annual-year-comparison-work-ambiguous");
        Check(warning != null && warning.text.StartsWith("2 work record(s)"), "actual warning retained sources=" + sources, warning?.text);
        Scroll.ScrollTo(warning); yield return Invoke(oracle, "Screenshot", "distinct-ID-ambiguous-" + (sources ? "on" : "off"));
        Set(oracle, "history", history);
    }
}
#endif
