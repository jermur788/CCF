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
public static class AreaCalibrationReview
{
    public static void Begin(){EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");EditorApplication.isPlaying=true;}
    public static void SetGameSize(int width, int height)
    {
#if UNITY_EDITOR
        if (Application.isBatchMode) return;
        Assembly assembly = typeof(Editor).Assembly;
        Type sizesType = assembly.GetType("UnityEditor.GameViewSizes");
        Type singleton = typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
        object sizes = singleton.GetProperty("instance", BindingFlags.Public | BindingFlags.Static).GetValue(null);
        MethodInfo getGroup = sizesType.GetMethod("GetGroup");
        object group = getGroup.Invoke(sizes, new[] { Enum.ToObject(getGroup.GetParameters()[0].ParameterType, 0) });
        Type sizeType = assembly.GetType("UnityEditor.GameViewSize"), kind = assembly.GetType("UnityEditor.GameViewSizeType");
        object size = Activator.CreateInstance(sizeType, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
            null, new object[] { Enum.ToObject(kind, 1), width, height, "Review " + width + "x" + height }, null);
        int index = (int)group.GetType().GetMethod("GetTotalCount").Invoke(group, null);
        group.GetType().GetMethod("AddCustomSize").Invoke(group, new[] { size });
        Type viewType = assembly.GetType("UnityEditor.GameView");
        EditorWindow view = EditorWindow.GetWindow(viewType);
        viewType.GetProperty("selectedSizeIndex", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).SetValue(view, index);
        view.Show(); view.Focus(); view.Repaint();
#endif
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Install(){if(Environment.GetCommandLineArgs().Contains("AreaCalibrationReview.Begin"))new GameObject("Calibration UI review").AddComponent<AreaCalibrationReviewRunner>();}
}
public sealed class AreaCalibrationReviewRunner : MonoBehaviour
{
    ScenarioOneUiRoot ui; ScenarioOneManager m; ForestSaveController saves;
    static void Check(bool ok,string why){if(!ok)throw new InvalidOperationException(why);}
    IEnumerator Start()
    {
        for(int i=0;i<8;i++)yield return null;
        ui=FindFirstObjectByType<ScenarioOneUiRoot>();m=ui.Manager;saves=FindFirstObjectByType<ForestSaveController>();
        var stack=new Stack<IEnumerator>();stack.Push(Run());Exception failure=null;
        while(stack.Count>0){bool more;object current=null;try{more=stack.Peek().MoveNext();if(more)current=stack.Peek().Current;}catch(Exception e){failure=e;break;}if(!more){stack.Pop();continue;}if(current is IEnumerator nested){stack.Push(nested);continue;}yield return current;}
        Debug.Log(failure==null?"AREA_CALIBRATION_REVIEW_PASS":"AREA_CALIBRATION_REVIEW_FAIL "+failure);EditorApplication.Exit(failure==null?0:1);
    }
    IEnumerator Settle(){for(int i=0;i<14;i++){ui.CloseHelp();yield return null;}}
    IEnumerator Capture(string name)
    {
        yield return new WaitForEndOfFrame();string output=Environment.GetEnvironmentVariable("CCF_ACCEPTANCE_OUTPUT");Directory.CreateDirectory(output);var image=ScreenCapture.CaptureScreenshotAsTexture();File.WriteAllBytes(Path.Combine(output,name+".png"),image.EncodeToPNG());Destroy(image);
    }
    void CheckLabels(IEnumerable<Label> labels)
    {
        foreach(var label in labels.Where(x=>x.resolvedStyle.display==DisplayStyle.Flex))
        {Check(label.worldBound.width<=label.parent.worldBound.width+1,"label wider than parent: "+label.text);Check(label.worldBound.height<=label.parent.worldBound.height+1,"label taller than parent: "+label.text);}
    }
    IEnumerator Run()
    {
        var initial=saves.CaptureData();Check(m.AdvanceYear(),"annual report");var annual=saves.CaptureData();
        foreach(int width in new[]{1280,1920})
        {
            int height=width==1280?720:1080;AreaCalibrationReview.SetGameSize(width,height);yield return Settle();
            Check(saves.LoadData(annual,false),"restore E80 annual");yield return Settle();ui.ShowObjectives();yield return Settle();
            var fold=ui.RootElement.Q<Foldout>("scenario-success-objectives");Check(fold!=null,"Objectives foldout");var labels=fold.Query<Label>().ToList();
            foreach(var result in m.Objectives)Check(labels.Any(x=>x.text==ScenarioOneUiFacts.ObjectiveLine(result)),"Objectives target mismatch "+result.objectiveId);
            CheckLabels(labels);yield return Capture("objectives-e80-"+width);
            ui.ShowReview();yield return Settle();
            Label objective=ui.RootElement.Query<Label>().ToList().First(x=>x.text==ScenarioOneUiFacts.ObjectiveLine(m.Objectives.First()));
            foreach(var result in m.Objectives)Check(ui.RootElement.Query<Label>().ToList().Any(x=>x.text==ScenarioOneUiFacts.ObjectiveLine(result)),"Annual Review target mismatch "+result.objectiveId);
            var scroll=objective.GetFirstAncestorOfType<ScrollView>();scroll.ScrollTo(objective);yield return Settle();CheckLabels(scroll.Query<Label>().ToList());yield return Capture("annual-objectives-e80-"+width);
            var fallback=ScenarioOneObjectives.Review(m.Definition,m.EcologicalSnapshots.Last(),m.Outcome,-1,"sitka-spruce",geometry:StandGeometryModel.Enlarged80);fallback.year=100;
            typeof(ScenarioOneManager).GetField("centuryReview",BindingFlags.NonPublic|BindingFlags.Instance).SetValue(m,fallback);
            // Reopen forces the existing view to render this presentation fixture; no saved asset/world edits.
            ui.ShowObjectives();ui.ShowReview();yield return Settle();
            Label copy=ui.RootElement.Query<Label>().ToList().FirstOrDefault(x=>x.text.Contains("aspirational design targets")&&x.text.Contains("not a forecast"));Check(copy!=null,"fallback wording");
            scroll=copy.GetFirstAncestorOfType<ScrollView>();scroll.ScrollTo(copy);yield return Settle();CheckLabels(copy.parent.Query<Label>().ToList());
            Check(copy.parent.Query<Label>().ToList().All(x=>!x.text.Contains("frozen Reference Future")&&!x.text.Contains("· reference")),"fallback claims reference");yield return Capture("century-targets-e80-"+width);
            var reference=ScenarioReferenceArchive.Load();Check(saves.LoadData(reference.AtYear(100).world,false),"Legacy40 reference restore");yield return Settle();
            var frozen=ScenarioOneObjectives.Review(m.Definition,m.EcologicalSnapshots.Last(),m.Outcome,m.OutcomeYear,"sitka-spruce",reference,geometry:StandGeometryModel.Legacy40);
            typeof(ScenarioOneManager).GetField("centuryReview",BindingFlags.NonPublic|BindingFlags.Instance).SetValue(m,frozen);ui.ShowObjectives();ui.ShowReview();yield return Settle();
            Label frozenCopy=ui.RootElement.Query<Label>().ToList().FirstOrDefault(x=>x.text.Contains("Compared with the frozen Reference Future"));Check(frozenCopy!=null,"frozen wording");frozenCopy.GetFirstAncestorOfType<ScrollView>().ScrollTo(frozenCopy);yield return Settle();CheckLabels(frozenCopy.parent.Query<Label>().ToList());yield return Capture("century-reference-l40-"+width);
            Debug.Log("AREA_CALIBRATION_REVIEW_SIZE_PASS "+width+"x"+height);
        }
        Check(saves.LoadData(initial,false),"restore original");
    }
}
#endif
