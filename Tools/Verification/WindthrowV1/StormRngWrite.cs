#if UNITY_EDITOR
using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
public static class StormRngWrite
{
 public static void Begin(){EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");EditorApplication.isPlaying=true;}
 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]static void Install(){if(Environment.GetCommandLineArgs().Contains("StormRngWrite.Begin"))new GameObject("Disposable RNG-mode storm replay").AddComponent<StormRngWriteRunner>();}
}
public sealed class StormRngWriteRunner:MonoBehaviour
{
 IEnumerator Start()
 {
  yield return null;yield return null;Exception failed=null;
  try
  {
   var saves=FindFirstObjectByType<ForestSaveController>();var e=FindFirstObjectByType<ForestEcologyController>();var m=FindFirstObjectByType<ScenarioOneManager>();var original=saves.CaptureData();string output=Environment.GetEnvironmentVariable("CCF_STORM_OUTPUT");
   for(int model=0;model<=1;model++)
   {
    if(!saves.LoadData(original,false))throw new Exception("Restore initial RNG fixture");e.RngModelVersion=model;e.StormModelVersion=1;e.Browsing.BackgroundPressure=.2f;
    var storm=new StormEventRecord{year=1,severity=.18f,directionDegrees=135};var trees=FindObjectsByType<ForestTree>(FindObjectsSortMode.None).Where(t=>t.IsLiving).ToArray();
    var forward=e.EvaluateStorm(storm,trees);var reverse=e.EvaluateStorm(storm,trees.Reverse().ToArray());
    if(!forward.Select(v=>v.Tree.TreeId+":"+v.Victim).SequenceEqual(reverse.Select(v=>v.Tree.TreeId+":"+v.Victim)))throw new Exception("Victims depend on enumeration in RNG mode "+model);
    e.ForceStormNextYear(.18f,135);if(!m.AdvanceYear())throw new Exception("Resolve RNG mode storm");var checkpoint=saves.CaptureData();string resolved=ScenarioReferenceArchive.WorldHash(checkpoint);File.WriteAllText(Path.Combine(output,"world-"+model+".json"),JsonUtility.ToJson(checkpoint,true));
    for(int year=2;year<=10;year++)if(!m.AdvanceYear())throw new Exception("Advance RNG mode future");string future=ScenarioReferenceArchive.WorldHash(saves.CaptureData());File.WriteAllText(Path.Combine(output,"hashes-"+model+".txt"),resolved+"\n"+future+"\n");
   }
   saves.LoadData(original,false);Debug.Log("STORM_RNG_WRITE_PASS models=0,1 iterationOrder=true throughYear10=true");
  }catch(Exception error){failed=error;Debug.LogError("STORM_RNG_WRITE_FAIL "+error);}
  EditorApplication.ExitPlaymode();EditorApplication.Exit(failed==null?0:1);
 }
}
#endif
