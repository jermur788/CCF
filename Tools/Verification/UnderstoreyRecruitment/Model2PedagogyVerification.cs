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
public static class Model2PedagogyVerification
{
 public static void Begin(){EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");SetGameSize(1600,900);EditorApplication.isPlaying=true;}
 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)] static void Install(){if(Environment.GetCommandLineArgs().Contains("Model2PedagogyVerification.Begin"))new GameObject("Model2 pedagogy verification").AddComponent<Model2PedagogyRunner>();}
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
        object size = Activator.CreateInstance(sizeType, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
            null, new object[] { Enum.ToObject(kind, 1), width, height, "Menu tutorial review " + width + "x" + height }, null);
        int index = (int)group.GetType().GetMethod("GetTotalCount").Invoke(group, null);
        group.GetType().GetMethod("AddCustomSize").Invoke(group, new[] { size });
        Type viewType = assembly.GetType("UnityEditor.GameView"); EditorWindow view = EditorWindow.GetWindow(viewType);
        viewType.GetProperty("selectedSizeIndex", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).SetValue(view, index);
        var gizmos = viewType.GetProperty("showGizmos", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (gizmos != null && gizmos.CanWrite) gizmos.SetValue(view, false);
        view.Show(); view.Focus(); view.Repaint();
    }
}
public sealed class Model2PedagogyRunner:MonoBehaviour
{
 int checks;ScenarioOneUiRoot ui;ForestSaveController saves;string output;
 void Check(bool ok,string why){checks++;if(!ok)throw new Exception(why);}
 IEnumerator Start()
 {
  yield return null;yield return null;yield return null;yield return null;
  ui=FindFirstObjectByType<ScenarioOneUiRoot>();saves=FindFirstObjectByType<ForestSaveController>();output=Environment.GetEnvironmentVariable("CCF_ACCEPTANCE_OUTPUT");Directory.CreateDirectory(output);
  var stack=new Stack<IEnumerator>();stack.Push(Run());Exception error=null;
  while(stack.Count>0){bool more=false;object next=null;try{more=stack.Peek().MoveNext();if(more)next=stack.Peek().Current;}catch(Exception ex){error=ex;break;}if(!more){stack.Pop();continue;}if(next is IEnumerator nested){stack.Push(nested);continue;}yield return next;}
  if(error!=null)Debug.LogError("MODEL2_PEDAGOGY_FAIL "+error);else Debug.Log("MODEL2_PEDAGOGY_PASS checks="+checks+" legacyTruth=true model2Copy=true readOnly=true renderedResolutions=3");
  EditorApplication.ExitPlaymode();EditorApplication.Exit(error==null?0:1);
 }
 Foldout Clearance(){return ui.Learning.Root.Query<Foldout>().ToList().Single(f=>f.text.StartsWith("7. Vegetation clearance"));}
 string Displayed(){return string.Join("\n",Clearance().Query<Label>().ToList().Select(l=>l.text));}
 IEnumerator Run()
 {
  Check(ui.Ecology.RegenerationModelVersion==2,"integrated new-game model2");
  var original=saves.CaptureData();string hash=ScenarioReferenceArchive.WorldHash(original);int progress=ui.Learning.CompletedSteps;
  ui.CloseAll();ui.ShowObjectives();ui.CloseHelp();ui.Learning.Refresh(true);yield return null;
  foreach(int model in new[]{2,1,0,2})
  {
   ui.Ecology.RegenerationModelVersion=model;ui.Learning.Refresh(false);yield return null;
   string shown=Displayed();string expected=LearningObjectivesView.ClearanceExplanation(model);
   Check(shown.Contains(expected),"actual displayed copy follows loaded model "+model);
   if(model==2)Check(shown.Contains("Dense bramble or bracken")&&shown.Contains("already growing inside")&&shown.Contains("can return"),"approved model2 meaning");
   else Check(shown.Contains("does not change young-tree survival or growth")&&!shown.Contains("Dense bramble or bracken"),"legacy truth without competition promise");
  }
  foreach(var size in new[]{new Vector2Int(1280,720),new Vector2Int(1600,900),new Vector2Int(1920,1080)})
  {
   Model2PedagogyVerification.SetGameSize(size.x,size.y);ui.Learning.Refresh(true);
   foreach(var foldout in ui.Learning.Root.Query<Foldout>().ToList())foldout.value=foldout==Clearance();
   ui.Learning.Root.Q<ScrollView>().scrollOffset=Vector2.zero;
   for(int frame=0;frame<5;frame++)yield return null;
   var panel=ui.Learning.Root.Children().First();Rect bounds=panel.worldBound,viewport=ui.RootElement.worldBound;
   Check(viewport.Contains(bounds.min)&&viewport.Contains(bounds.max),"learning modal fits "+size.x);
   Check(Clearance().Query<Label>().ToList().Any(l=>l.text==LearningObjectivesView.ClearanceExplanation(2)&&l.worldBound.height>20),"approved copy laid out "+size.x);
   yield return new WaitForEndOfFrame();var image=ScreenCapture.CaptureScreenshotAsTexture();Check(image!=null&&image.width>=600,"actual rendered capture");File.WriteAllBytes(Path.Combine(output,"model2-clearance-"+size.x+".png"),image.EncodeToPNG());Destroy(image);
  }
  ui.CloseAll();Check(ui.Learning.CompletedSteps==progress,"reading gate does not change lesson progress");Check(ScenarioReferenceArchive.WorldHash(saves.CaptureData())==hash,"P1 views and version-specific explanation do not alter forest state");
 }
}
#endif
