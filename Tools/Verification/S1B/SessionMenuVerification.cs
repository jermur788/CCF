using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
public static class SessionMenuVerification
{
#if UNITY_EDITOR
 public static void Begin() { EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity"); EditorApplication.isPlaying = true; }
#endif
 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
 private static void Install()
 {
  if (Environment.GetCommandLineArgs().Contains("SessionMenuVerification.Begin")) new GameObject("Disposable S1-B session verification").AddComponent<S1BSessionMenuRunner>();
 }
}
public sealed class S1BSessionMenuRunner : MonoBehaviour
{
 private StandaloneSessionMenu menu;
 private void Check(bool ok,string message) { if(!ok)throw new Exception(message);Debug.Log("S1B_SESSION_CHECK "+message); }
 private IEnumerator Start()
 {
  yield return null;yield return null;
  var run=Verify();string failure=null;
  while(true)
  {
   bool more;object current=null;
   try{more=run.MoveNext();if(more)current=run.Current;}
   catch(Exception e){Debug.LogException(e);failure=e.Message;break;}
   if(!more)break;yield return current;
  }
  if(menu!=null){menu.Close();Destroy(menu.gameObject);}
  Debug.Log(failure==null ? "S1B_SESSION_MENU_VERIFY_PASS" : "S1B_SESSION_MENU_VERIFY_FAIL "+failure);
#if UNITY_EDITOR
  EditorApplication.ExitPlaymode();EditorApplication.Exit(failure==null?0:1);
#endif
 }
 private IEnumerator Verify()
 {
  var saves=FindFirstObjectByType<ForestSaveController>();var ui=FindFirstObjectByType<ScenarioOneUiRoot>();
  ui.CloseHelp();ui.CloseAll();yield return null;
  string before=ScenarioReferenceArchive.WorldHash(saves.CaptureData());var player=FindFirstObjectByType<ForestPlayer>();bool enabled=player.enabled;
  menu=new GameObject("Session controls under test").AddComponent<StandaloneSessionMenu>();
  yield return null;yield return null;yield return null;yield return null;
  Check(StandaloneSessionMenu.IsOpen && Time.timeScale==0,"Menu pauses time and opens");
  Check(!player.enabled && !saves.enabled && !ui.enabled,"Gameplay/shortcut/UI input blocked while menu open");
  Check(ScenarioReferenceArchive.WorldHash(saves.CaptureData())==before,"Opening menu leaves forest/save state unchanged");
  var root=menu.GetComponent<UIDocument>().rootVisualElement;
  Check(root.Query<Button>().ToList().Any(b=>b.text.StartsWith("Start a fresh")),"Fresh-test route exposed");
  Check(root.Query<Button>().ToList().Any(b=>b.text.StartsWith("Quit")),"Normal quit route exposed");
  menu.Close();yield return null;yield return null;
  Check(!StandaloneSessionMenu.IsOpen && Time.timeScale==1 && player.enabled==enabled && saves.enabled && ui.enabled,"Close restores prior control state");
  Check(ScenarioReferenceArchive.WorldHash(saves.CaptureData())==before,"Menu roundtrip remains read-only");
  Check(ui.Manager.TryBeginReferencePreview(0),"Reference preview opens");menu.Open();
  Check(!StandaloneSessionMenu.IsOpen,"Reference preview cannot enter session save route");ui.Manager.EndReferencePreview();
 }
}
