#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

public static class Fr10RenderedOracle
{
    public static void Begin()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        EditorApplication.isPlaying = true;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Install()
    {
        if (Environment.GetCommandLineArgs().Contains("Fr10RenderedOracle.Begin"))
            new GameObject("Disposable FR10 oracle").AddComponent<Fr10RenderedRunner>();
    }
}

public sealed class Fr10RenderedRunner : MonoBehaviour
{
    ScenarioOneUiRoot ui;
    ScenarioOneManager manager;
    ForestSaveController saves;
    AnnualReviewView review;
    D073Runner inherited;
    ForestSaveData history;
    string output;
    int checks, failures, caseFailures;
    readonly List<string> results = new List<string>();
    static readonly BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;
    static object Get(object target, string name) => target.GetType().GetField(name, Private).GetValue(target);
    static void Set(object target, string name, object value) => target.GetType().GetField(name, Private).SetValue(target, value);
    IEnumerator Old(string name, params object[] arguments) => (IEnumerator)typeof(D073Runner).GetMethod(name, Private).Invoke(inherited, arguments);
    ScrollView Scroll => (ScrollView)Get(review, "scroll");
    ForestSaveData Clone(ForestSaveData state) => JsonUtility.FromJson<ForestSaveData>(JsonUtility.ToJson(state));
    string Canon(ForestSaveData state) { state = Clone(state); ScenarioReferenceArchive.Canonicalize(state); return JsonUtility.ToJson(state); }
    string Hash(ForestSaveData state) => ScenarioReferenceArchive.CurrentWorldHash(Clone(state));
    IEnumerator Settle() { yield return new WaitForSecondsRealtime(.18f); for (int frame = 0; frame < 4; frame++) yield return null; }

    void Check(bool okay, string name, string detail = "")
    {
        checks++;
        if (!okay) failures++;
        string line = "FR10_CHECK " + (okay ? "PASS " : "FAIL ") + name + " | " + detail;
        results.Add(line); Debug.Log(line);
    }

    IEnumerator Case(string name, IEnumerator action)
    {
        int before = failures, count = checks;
        var stack = new Stack<IEnumerator>(); stack.Push(action);
        Exception exception = null;
        while (stack.Count > 0)
        {
            bool more = false; object next = null;
            try { more = stack.Peek().MoveNext(); if (more) next = stack.Peek().Current; }
            catch (Exception caught) { exception = caught; break; }
            if (!more) { stack.Pop(); continue; }
            if (next is IEnumerator child) stack.Push(child); else yield return next;
        }
        if (exception != null) Check(false, name + " exception", exception.ToString());
        bool passed = before == failures;
        if (!passed) caseFailures++;
        string line = "FR10_CASE " + name + " " + (passed ? "PASS" : "FAIL") + " checks=" + (checks - count);
        results.Add(line); Debug.Log(line);
        File.WriteAllLines(Path.Combine(output, "fr10-results.txt"), results);
    }

    IEnumerator Start()
    {
        for (int frame = 0; frame < 12; frame++) yield return null;
        output = Environment.GetEnvironmentVariable("CCF_DIAG_DIR"); Directory.CreateDirectory(output);
        ui = FindFirstObjectByType<ScenarioOneUiRoot>(); manager = ui.Manager;
        saves = FindFirstObjectByType<ForestSaveController>(); review = (AnnualReviewView)Get(ui, "review");
        Debug.Log("FR10_ISOLATION persistentDataPath=" + Application.persistentDataPath + " XDG_CONFIG_HOME=" + Environment.GetEnvironmentVariable("XDG_CONFIG_HOME"));
        Check(Application.persistentDataPath.StartsWith("/media/jer/Files/CCF/Verification/D073-FR10-427-20261112/"), "isolated save and Prefs path", Application.persistentDataPath);
        var dormant = new GameObject("Dormant inherited assertion library"); dormant.SetActive(false);
        inherited = dormant.AddComponent<D073Runner>();
        Set(inherited, "ui", ui); Set(inherited, "manager", manager); Set(inherited, "saves", saves); Set(inherited, "output", output);
        history = JsonUtility.FromJson<ForestSaveData>(File.ReadAllText(Environment.GetEnvironmentVariable("CCF_D073_RESUME_HISTORY")));
        Check(history.ecologicalYear == 6 && history.scenarioOne.firstGuidedRackReviewComplete, "authentic inherited Year6 history");
        bool negative = Environment.GetEnvironmentVariable("CCF_FR10_NEGATIVE") == "1";
        foreach (var size in negative ? new[] { new Vector2Int(1280,720) } : new[] { new Vector2Int(1280,720), new Vector2Int(1920,1080) })
        {
            Cadence408GameSize.SetGameSize(size.x, size.y); yield return Settle();
            string dimensions = size.x + "x" + size.y;
            Check(Screen.width == size.x && Screen.height == size.y, "exact screenshot dimensions " + dimensions, Screen.width + "x" + Screen.height);
            foreach (bool sources in new[] { true, false })
                yield return Case(dimensions + "/rendered-" + (sources ? "on" : "off"), Rendered("full", sources));
            if (negative) continue;
            yield return Case(dimensions + "/inherited-native", Old("NativeSize", Clone(history), size));
            yield return Case(dimensions + "/native-foldout-close-and-latest", NativeExtra());
            foreach (string mode in new[] { "missing-selected", "missing-latest", "both-missing", "ambiguous", "event-only", "ambiguous-event", "species-one-sided" })
                foreach (bool sources in new[] { true, false })
                    yield return Case(dimensions + "/" + mode + "-" + (sources ? "on" : "off"), Rendered(mode, sources));
            yield return Case(dimensions + "/inherited-readonly-ACK-AB", Old("ReadOnlyAB", Clone(history)));
            yield return Case(dimensions + "/inherited-current-save", Old("CurrentDisk", Clone(history)));
            yield return Case(dimensions + "/inherited-v9", Old("OldSave", Clone(history)));
            yield return Case(dimensions + "/inherited-reference", Old("Reference", Clone(history)));
            yield return Case(dimensions + "/old-negative-assertions-retained", Old("NegativeViews", Clone(history)));
        }
        if (!negative) yield return Case("inherited-real-type-mapper", Old("Provenance"));
        File.WriteAllLines(Path.Combine(output, "inherited-checks.txt"), (List<string>)Get(inherited, "evidence"));
        Debug.Log("FR10_DONE checks=" + checks + " failures=" + failures + " casesFailed=" + caseFailures + " inheritedChecks=" + Get(inherited,"checks"));
        File.WriteAllLines(Path.Combine(output, "fr10-results.txt"), results);
        EditorApplication.ExitPlaymode(); EditorApplication.Exit(failures == 0 ? 0 : 1);
    }

    ForestSaveData World(string mode)
    {
        var state = Clone(history); int selected = state.scenarioOne.annualReports.First().year;
        int latest = state.scenarioOne.annualReports.Last().year;
        var snapshots = state.scenarioOne.ecologicalSnapshots;
        if (mode == "missing-selected" || mode == "both-missing") snapshots.RemoveAll(snapshot => snapshot.year == selected);
        if (mode == "missing-latest" || mode == "both-missing") snapshots.RemoveAll(snapshot => snapshot.year == latest);
        if (mode == "ambiguous")
        {
            var snapshot = snapshots.First(item => item.year == selected);
            snapshot.species.Add(JsonUtility.FromJson<ScenarioSpeciesOutcome>(JsonUtility.ToJson(snapshot.species.First())));
        }
        if (mode == "species-one-sided") snapshots.First(snapshot => snapshot.year == selected).species.RemoveAll(species => species.speciesId == "beech");
        if (mode == "event-only" || mode == "ambiguous-event")
        {
            var work = state.scenarioOne.managementEvents.First(item => item.year == selected && item.eventType == ScenarioManagementEventType.WorkResolved);
            if (mode == "event-only") state.scenarioOne.workOrders.RemoveAll(order => order.workOrderId == work.workOrderId);
            else state.scenarioOne.managementEvents.Add(JsonUtility.FromJson<ScenarioManagementEvent>(JsonUtility.ToJson(work)));
        }
        return state;
    }

    bool Displayed(VisualElement element)
    {
        if (element == null || element.panel == null) return false;
        for (var ancestor = element; ancestor != null; ancestor = ancestor.parent)
            if (ancestor.resolvedStyle.display == DisplayStyle.None || ancestor.resolvedStyle.visibility != Visibility.Visible) return false;
        return element.worldBound.width > 0 && element.worldBound.height > 0;
    }

    bool Fits(VisualElement element)
    {
        if (!Displayed(element)) return false;
        Rect rectangle = element.worldBound, viewport = Scroll.contentViewport.worldBound;
        Rect panel = ui.RootElement.worldBound;
        return rectangle.xMin >= Mathf.Max(viewport.xMin,panel.xMin)-1 && rectangle.xMax <= Mathf.Min(viewport.xMax,panel.xMax)+1
            && rectangle.yMin >= Mathf.Max(viewport.yMin,panel.yMin)-1 && rectangle.yMax <= Mathf.Min(viewport.yMax,panel.yMax)+1;
    }

    IEnumerator Reveal(VisualElement element)
    {
        if (element != null) Scroll.ScrollTo(element);
        yield return Settle();
    }

    IEnumerator Screenshot(string name)
    {
        yield return Settle(); yield return new WaitForEndOfFrame();
        var image = ScreenCapture.CaptureScreenshotAsTexture();
        string prefix = Screen.width + "x" + Screen.height + "-" + name;
        File.WriteAllBytes(Path.Combine(output,prefix+".png"),image.EncodeToPNG());
        Debug.Log("FR10_SCREENSHOT " + prefix + " " + image.width + "x" + image.height); Destroy(image);
        File.WriteAllLines(Path.Combine(output,prefix+"-labels.txt"), review.Root.Query<Label>().ToList().Select(label => label.name+" | "+label.text+" | "+label.worldBound+" | "+label.resolvedStyle.display));
    }

    IEnumerator Rendered(string mode, bool sources)
    {
        var world = World(mode); yield return Old("Restore", world); ui.ShowReview();
        int selected = world.scenarioOne.annualReports.First().year;
        review.TryShowYear(selected); yield return Settle();
        string before = Canon(saves.CaptureData()), hash = Hash(saves.CaptureData());
        int events = saves.CaptureData().scenarioOne.managementEvents.Count;
        review.Root.Q<Toggle>("annual-show-data-sources").value = sources; yield return Settle();
        var card = review.Root.Q("annual-year-comparison");
        Check(card != null, mode+" card exists"); if (card == null) yield break;
        var foldout = card.Q<Foldout>(); if (foldout != null) foldout.value = true; yield return Settle();
        var mapped = ScenarioOneYearComparisonText.Describe(ScenarioOneManagementFollowonReadout.Build(manager.HistoryYears, selected),selected,ScenarioOneUiFacts.SpeciesName);
        var named = card.Query<VisualElement>().ToList().Where(element => !string.IsNullOrEmpty(element.name)).GroupBy(element => element.name).Where(group => group.Count()>1).Select(group=>group.Key).ToArray();
        Check(named.Length==0,mode+" no duplicate element names",string.Join(",",named));
        var header = mapped.Rows.FirstOrDefault(row => row.Kind == ScenarioOneYearComparisonText.RowKind.TableHeader);
        int measures = 0;
        foreach (var row in mapped.Rows)
        {
            string prefix = "annual-year-comparison-"+row.Key;
            bool measure = row.Kind == ScenarioOneYearComparisonText.RowKind.StandMeasure || row.Kind == ScenarioOneYearComparisonText.RowKind.SpeciesMeasure;
            if (!measure)
            {
                if (row.Kind == ScenarioOneYearComparisonText.RowKind.SpeciesHeading) continue;
                var label = card.Q<Label>(prefix);
                if (row.Kind == ScenarioOneYearComparisonText.RowKind.GuidancePointer && label == null) continue;
                Check(label != null && label.text == row.Text,mode+" exact line "+row.Key,label?.text);
                if (label == null) continue;
                yield return Reveal(label);
                Check(Fits(label),mode+" line visible "+row.Key,label.worldBound.ToString());
                if (row.Provenance == HistoryProvenance.NotRecorded)
                {
                    var chips = label.parent.Query<Label>(className:"prov").ToList();
                    Check(chips.Count==1 && chips[0].text=="NOT RECORDED",mode+" uncertainty never RECORDED "+row.Key,string.Join(",",chips.Select(chip=>chip.text)));
                    Check(chips.All(chip=>Displayed(chip)==sources),mode+" warning chip display "+row.Key);
                }
                if (row.Key.Contains("missing") || row.Key.Contains("ambiguous") || row.Key.Contains("event-only") || row.Kind == ScenarioOneYearComparisonText.RowKind.SpeciesNotRecorded)
                    yield return Screenshot(mode+"-"+(sources?"on":"off")+"-"+row.Key);
                continue;
            }
            measures++;
            var values = card.Q(prefix+"-values");
            Check(values != null && values.childCount == new[]{row.SelectedValue,row.LatestValue,row.Difference}.Count(value=>!string.IsNullOrEmpty(value)),mode+" separate entries "+row.Key);
            Check(!card.Query<Label>().ToList().Any(label=>label.text==row.Text),mode+" no mixed observation/difference label "+row.Key);
            string[] suffixes={"year-a","year-b","difference"}, texts={row.SelectedValue,row.LatestValue,row.Difference};
            string[] headings={header?.SelectedValue,header?.LatestValue,header?.Difference};
            for(int column=0;column<3;column++)
            {
                string entryName=prefix+"-"+suffixes[column]; var entry=card.Q(entryName);
                if(string.IsNullOrEmpty(texts[column])) {Check(entry==null,mode+" absent record has no false value/tag "+entryName);continue;}
                Check(entry!=null && entry.parent==values,mode+" own value entry "+entryName);
                if(entry==null)continue;
                var value=entry.Q<Label>(entryName+"-value"); var heading=entry.Q<Label>(entryName+"-heading"); var tag=entry.Q<Label>(entryName+"-tag");
                Check(value!=null && value.text==texts[column],mode+" exact mapper value "+entryName,value?.text);
                Check(heading!=null && heading.text==headings[column],mode+" table header heading "+entryName,heading?.text);
                var provenance=column==2?row.DifferenceProvenance:row.ValueProvenance;
                Check(provenance==(column==2?HistoryProvenance.Derived:HistoryProvenance.Recorded),mode+" expected mapper provenance "+entryName,provenance.ToString());
                Check(provenance.HasValue ? tag!=null && tag.text==provenance.Value.ToString().ToUpperInvariant() : tag==null,mode+" exact chip "+entryName,tag?.text);
                yield return Reveal(entry);
                Check(Fits(value)&&Fits(heading),mode+" visible value and heading "+entryName,entry.worldBound.ToString());
                Check(sources?Fits(tag):!Displayed(tag),mode+" chip rendered state "+entryName,tag?.worldBound.ToString());
            }
            if(row.Kind==ScenarioOneYearComparisonText.RowKind.StandMeasure && (row.Key=="stand-canopy" || measures==1) || row.FirstRowOfSpecies)
                yield return Screenshot(mode+"-"+(sources?"on":"off")+"-"+row.Key);
        }
        Check(mode=="full" ? measures==17 : true,mode+" every stand/species measure enumerated",measures.ToString());
        if(mode=="event-only") Check(mapped.Rows.Any(row=>row.Key=="work-event-only"),"event-only fixture actually warning");
        if(mode=="ambiguous-event") Check(mapped.Rows.Any(row=>row.Key=="work-ambiguous"),"ambiguous event fixture actually warning");
        if(mode=="ambiguous") Check(mapped.State==ScenarioOneYearComparisonText.CardState.CannotCompare && measures==0,"ambiguous snapshots no invented values");
        Check(Canon(saves.CaptureData())==before && Hash(saves.CaptureData())==hash && saves.CaptureData().scenarioOne.managementEvents.Count==events,mode+" exact state/hash/events unchanged");
        Debug.Log("FR10_WORLD "+mode+" sources="+sources+" beforeHash="+hash+" afterHash="+Hash(saves.CaptureData())+" events="+events);
    }

    IEnumerator NativeExtra()
    {
        yield return Old("Restore",Clone(history));ui.ShowReview();review.TryShowYear(3);yield return Settle();
        var foldout=review.Root.Q<Foldout>("annual-year-comparison-species");yield return Reveal(foldout.Q<Toggle>());
        if(!foldout.value)yield return Old("NativeClick",foldout.Q<Toggle>());
        yield return Old("NativeClick",foldout.Q<Toggle>());Check(!foldout.value,"native species foldout closes");
        for(int step=0;(int)Get(review,"viewYear")>=0 && step<manager.HistoryYears.Count;step++)
            yield return Old("NativeClick",review.Root.Query<Button>().ToList().First(button=>button.text=="Later year ▶"));
        Check((int)Get(review,"viewYear")==-1,"native later returns follow-latest");
        yield return Screenshot("native-extra-closed-latest");
    }
}
#endif
