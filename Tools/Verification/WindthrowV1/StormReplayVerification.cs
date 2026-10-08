#if UNITY_EDITOR
using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
public static class StormReplayVerification
{
 public static void Begin(){EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");EditorApplication.isPlaying=true;}
 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]static void Install(){if(Environment.GetCommandLineArgs().Contains("StormReplayVerification.Begin"))new GameObject("Disposable independent storm replay").AddComponent<StormReplayVerificationRunner>();}
}
public sealed class StormReplayVerificationRunner:MonoBehaviour
{
 IEnumerator Start()
 {
  yield return null;yield return null;Exception failure=null;
  try{
   var saves=FindFirstObjectByType<ForestSaveController>();var ecology=FindFirstObjectByType<ForestEcologyController>();
   string dir=Environment.GetEnvironmentVariable("CCF_STORM_OUTPUT");
   string path=Path.Combine(Application.dataPath,"../Build/WindthrowV1/StormCoreVerification/evidence/resolved_storm.json");
   var data=JsonUtility.FromJson<ForestSaveData>(File.ReadAllText(path));
   if(!saves.LoadData(data,false))throw new Exception("Independent process load rejected");
   string resolved=ScenarioReferenceArchive.WorldHash(saves.CaptureData());
   if(resolved!="84796FAEF211823C")throw new Exception("Independent resolved hash changed: "+resolved);
   ecology.AdvanceOneYear();string future=ScenarioReferenceArchive.WorldHash(saves.CaptureData());
   if(future!="52037B943B62DD39")throw new Exception("Independent future hash changed: "+future);
   File.WriteAllText(Path.Combine(dir,"replay.txt"),"resolved="+resolved+"\ncontinued="+future+"\n");
   Debug.Log("STORM_REPLAY_VERIFICATION_PASS resolved="+resolved+" future="+future);
  }catch(Exception error){failure=error;Debug.LogError("STORM_REPLAY_VERIFICATION_FAIL "+error);}
  EditorApplication.ExitPlaymode();EditorApplication.Exit(failure==null?0:1);
 }
}
#endif
