#if UNITY_EDITOR
using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
public static class StormRngReplay
{
 public static void Begin(){EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");EditorApplication.isPlaying=true;}
 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]static void Install(){if(Environment.GetCommandLineArgs().Contains("StormRngReplay.Begin"))new GameObject("Disposable RNG-mode storm replay").AddComponent<StormRngReplayRunner>();}
}
public sealed class StormRngReplayRunner:MonoBehaviour
{
 IEnumerator Start()
 {
  yield return null;yield return null;Exception failed=null;
  try
  {
   var saves=FindFirstObjectByType<ForestSaveController>();var e=FindFirstObjectByType<ForestEcologyController>();var m=FindFirstObjectByType<ScenarioOneManager>();var original=saves.CaptureData();string output=Environment.GetEnvironmentVariable("CCF_STORM_OUTPUT");
   for(int model=0;model<=1;model++)
   {
    string source=Path.Combine(Application.dataPath,"../Build/WindthrowV1/StormRngWrite/evidence");var data=JsonUtility.FromJson<ForestSaveData>(File.ReadAllText(Path.Combine(source,"world-"+model+".json")));var hashes=File.ReadAllLines(Path.Combine(source,"hashes-"+model+".txt"));
    if(!saves.LoadData(data,false)||e.RngModelVersion!=model)throw new Exception("Independent RNG mode restore");e.Browsing.BackgroundPressure=.2f;if(ScenarioReferenceArchive.WorldHash(saves.CaptureData())!=hashes[0])throw new Exception("Resolved RNG mode hash mismatch");
    for(int year=2;year<=10;year++)if(!m.AdvanceYear())throw new Exception("Advance independently loaded RNG mode future");if(ScenarioReferenceArchive.WorldHash(saves.CaptureData())!=hashes[1])throw new Exception("Future RNG mode hash mismatch");File.WriteAllText(Path.Combine(output,"hashes-"+model+".txt"),string.Join("\n",hashes)+"\n");
   }
   saves.LoadData(original,false);Debug.Log("STORM_RNG_REPLAY_PASS models=0,1 iterationOrder=true throughYear10=true");
  }catch(Exception error){failed=error;Debug.LogError("STORM_RNG_REPLAY_FAIL "+error);}
  EditorApplication.ExitPlaymode();EditorApplication.Exit(failed==null?0:1);
 }
}
#endif
