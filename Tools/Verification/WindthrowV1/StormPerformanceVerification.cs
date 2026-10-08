#if UNITY_EDITOR
using System;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Debug=UnityEngine.Debug;
public static class StormPerformanceVerification
{
 public static void Begin(){EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");EditorApplication.isPlaying=true;}
 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]static void Install(){if(Environment.GetCommandLineArgs().Contains("StormPerformanceVerification.Begin"))new GameObject("Disposable storm performance gate").AddComponent<StormPerformanceVerificationRunner>();}
}
public sealed class StormPerformanceVerificationRunner:MonoBehaviour
{
 ForestEcologyController e;ScenarioOneManager m;ForestTreeSpawner spawner;ForestSaveController saves;int cases;
 string F(double value)=>value.ToString("R",CultureInfo.InvariantCulture);
 IEnumerator Start(){yield return null;yield return null;Exception failed=null;var work=Run();while(true){bool more=false;object current=null;try{more=work.MoveNext();if(more)current=work.Current;}catch(Exception error){failed=error;Debug.LogError("STORM_PERFORMANCE_VERIFICATION_FAIL "+error);break;}if(!more)break;yield return current;}if(failed==null)Debug.Log("STORM_PERFORMANCE_VERIFICATION_PASS cases="+cases+" exactIdRollVictims=true simulationOnly=true");EditorApplication.ExitPlaymode();EditorApplication.Exit(failed==null?0:1);}
 void Prepare(int count)
 {
  foreach(var tree in FindObjectsByType<ForestTree>(FindObjectsInactive.Include,FindObjectsSortMode.None))DestroyImmediate(tree.gameObject);
  m.InitializeNewScenario();e.ResetForDeterministicRun();e.SimulationSeed=20260914;e.RngModelVersion=1;e.RegenerationModelVersion=2;e.GrowthModelVersion=0;e.StormModelVersion=1;e.Browsing.BackgroundPressure=0;
  int side=Mathf.CeilToInt(Mathf.Sqrt(count));
  for(int i=0;i<count;i++){var root=new GameObject("Storm performance "+i);root.transform.position=new Vector3((i%side+.5f)*40/side-20,0,(i/side+.5f)*40/side-20);var stem=new GameObject("Trunk").transform;stem.SetParent(root.transform);var crown=new GameObject("Canopy").transform;crown.SetParent(root.transform);root.AddComponent<ForestTree>().InitializeForSpawn("STORMPERF"+i.ToString("0000"),stem,crown,spawner.DefaultSpecies,20,14,16,1.2f);}
  e.RecomputeCanopy();e.RecomputeSeedRain();
 }
 float IntensityForVictims(int desired)
 {
  var evaluated=e.EvaluateStorm(new StormEventRecord{year=1,severity=1,directionDegrees=135});
  var thresholds=evaluated.Select(value=>{double roll=SimulationRandom.Roll(e.RngModelVersion,"WINDTHROW-v1-"+value.Tree.TreeId,1,e.SimulationSeed);return roll/((1-roll)*value.Vulnerability);}).OrderBy(value=>value).ToArray();
  if(desired<1||desired>=thresholds.Length)throw new Exception("Unsupported exact damage size");
  float severity=(float)((thresholds[desired-1]+thresholds[desired])*.5);
  if(severity<=0||severity>1)throw new Exception("Exact damage size exceeds bounded intensity");
  if(e.EvaluateStorm(new StormEventRecord{year=1,severity=severity,directionDegrees=135}).Count(value=>value.Victim)!=desired)throw new Exception("Threshold selection disagrees with production float comparison");return severity;
 }
 IEnumerator Run()
 {
  e=FindFirstObjectByType<ForestEcologyController>();m=FindFirstObjectByType<ScenarioOneManager>();spawner=FindFirstObjectByType<ForestTreeSpawner>();saves=FindFirstObjectByType<ForestSaveController>();var original=saves.CaptureData();string dir=Environment.GetEnvironmentVariable("CCF_STORM_OUTPUT");
  using(var table=new StreamWriter(Path.Combine(dir,"storm_step_performance.csv")))
  {
   table.WriteLine("trees,victims,repeat,warmup,intensity,total_annual_ms,storm_evaluation_ms,storm_mortality_ms,storm_rebuild_ms,record_ms_subset_of_mortality,storm_visual_creation_ms,canopy_rebuilds,seed_rebuilds,windthrow_deadwood_records,simulation_only");
   foreach(int count in new[]{336,1300,5000})foreach(int desired in count==5000?new[]{10,50,100,1000}:new[]{10,50,100})for(int repeat=0;repeat<4;repeat++)
   {
    Prepare(count);yield return null;float severity=IntensityForVictims(desired);GC.Collect();e.ForceStormNextYear(severity,135);var watch=Stopwatch.StartNew();e.AdvanceOneYear();watch.Stop();var performance=e.LastStormPerformance;
    if(performance.Victims!=desired||performance.CanopyRebuilds!=1||performance.SeedRebuilds!=1||m.DeadwoodRecords.Count!=desired)throw new Exception("Exact batched windthrow performance budget failed");
    table.WriteLine(string.Join(",",count,desired,repeat,repeat==0,F(severity),F(watch.Elapsed.TotalMilliseconds),F(performance.EvaluationMilliseconds),F(performance.MortalityMilliseconds),F(performance.RebuildMilliseconds),F(performance.DeadwoodMilliseconds),F(performance.VisualMilliseconds),performance.CanopyRebuilds,performance.SeedRebuilds,m.DeadwoodRecords.Count,true));table.Flush();cases++;yield return null;yield return Resources.UnloadUnusedAssets();
   }
  }
  foreach(var tree in FindObjectsByType<ForestTree>(FindObjectsInactive.Include,FindObjectsSortMode.None))DestroyImmediate(tree.gameObject);
  if(!saves.LoadData(original,false))throw new Exception("Could not restore performance fixture");yield return null;
 }
}
#endif
