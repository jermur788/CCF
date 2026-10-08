#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Debug=UnityEngine.Debug;
public static class Model2Performance
{
 public static void Begin(){EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");EditorApplication.isPlaying=true;}
 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]static void Install(){if(Environment.GetCommandLineArgs().Contains("Model2Performance.Begin"))new GameObject("Model2 performance").AddComponent<Model2PerformanceRunner>();}
}
public sealed class Model2PerformanceRunner:MonoBehaviour
{
 const BindingFlags Flags=BindingFlags.Instance|BindingFlags.NonPublic;
 ForestEcologyController e;ScenarioOneManager m;ForestTreeSpawner spawner;string output;
 string F(double x)=>x.ToString("R",CultureInfo.InvariantCulture);
 void Call(object obj,string name)=>obj.GetType().GetMethod(name,Flags).Invoke(obj,null);
 IEnumerator Start()
 {
  yield return null;yield return null;e=FindFirstObjectByType<ForestEcologyController>();m=FindFirstObjectByType<ScenarioOneManager>();spawner=FindFirstObjectByType<ForestTreeSpawner>();output=Environment.GetEnvironmentVariable("CCF_MODEL2_OUTPUT");
  var stack=new Stack<IEnumerator>();stack.Push(Run());Exception error=null;
  while(stack.Count>0){bool more=false;object next=null;try{more=stack.Peek().MoveNext();if(more)next=stack.Peek().Current;}catch(Exception ex){error=ex;break;}if(!more){stack.Pop();continue;}if(next is IEnumerator nested){stack.Push(nested);continue;}yield return next;}
  if(error!=null)Debug.LogError("MODEL2PERFORMANCE_FAIL "+error);else Debug.Log("MODEL2PERFORMANCE_PASS actualAnnual=true overlappingHistory=true");EditorApplication.ExitPlaymode();EditorApplication.Exit(error==null?0:1);
 }
 void Prepare(int count,int model,int patchCount)
 {
  foreach(var t in FindObjectsByType<ForestTree>(FindObjectsInactive.Include,FindObjectsSortMode.None))DestroyImmediate(t.gameObject);
  m.InitializeNewScenario();e.ResetForDeterministicRun();e.RngModelVersion=e.GrowthModelVersion=1;e.RegenerationModelVersion=model;e.Browsing.BackgroundPressure=0;e.Browsing.ClearProtection();
  var species=spawner.DefaultSpecies;int side=Mathf.CeilToInt(Mathf.Sqrt(count));
  for(int i=0;i<count;i++){var root=new GameObject("Performance "+i);root.transform.position=new Vector3((i%side+.5f)*40/side-20,0,(i/side+.5f)*40/side-20);var stem=new GameObject("Trunk").transform;stem.SetParent(root.transform);var crown=new GameObject("Canopy").transform;crown.SetParent(root.transform);root.AddComponent<ForestTree>().InitializeForSpawn("PERF"+i,stem,crown,species,20,14,16,1.2f);}
  foreach(var cell in e.Cells){cell.ClearRegeneration();foreach(string id in new[]{"sitka-spruce","sessile-oak","beech"}){var b=new ForestRegenerationCohort(spawner.ResolveSpecies(id));b.Restore(.1f,.2f,0);cell.InsertBand(b);}}
  var juveniles=(List<PlantedJuvenile>)typeof(ScenarioOneManager).GetField("plantedJuveniles",Flags).GetValue(m);juveniles.Clear();for(int j=0;j<30;j++)juveniles.Add(new PlantedJuvenile{juvenileId="perf"+j,speciesId="beech",position=new Vector3(e.Cells[j%e.CellCount].Center.x,0,e.Cells[j%e.CellCount].Center.y),heightMeters=.6f,ageYears=1});
  Call(m,"EnsureUnderstoreyGrid");foreach(var u in m.UnderstoreyCells)u.brambleCover=u.brackenCover=.5f;var patches=(List<PlantingClearancePatch>)typeof(ScenarioOneManager).GetField("clearancePatches",Flags).GetValue(m);patches.Clear();for(int j=0;j<patchCount;j++){var centre=e.Cells[j%e.CellCount].Center;patches.Add(new PlantingClearancePatch{center=new Vector3(centre.x,0,centre.y),radiusMeters=.564f,createdYear=0,competitionUpdatedYear=0});}m.RebuildCompetitionExposure();
 }
 IEnumerator Run()
 {
  using(var w=new StreamWriter(Path.Combine(output,"model2_annual_performance.csv")))
  {
   w.WriteLine("adults,model,repeat,annual_ms,natural_bands,exact_planted,patches,simulation_only");
   foreach(int count in new[]{336,1300,3000,5000})foreach(int patchCount in new[]{0,12})for(int repeat=0;repeat<4;repeat++)foreach(int model in repeat%2==0?new[]{1,2}:new[]{2,1})
   {
    Prepare(count,model,patchCount);yield return null;GC.Collect();var watch=Stopwatch.StartNew();e.AdvanceOneYear();Call(m,"AdvancePlantedJuveniles");Call(m,"AdvanceUnderstorey");watch.Stop();w.WriteLine(string.Join(",",count,model,repeat,F(watch.Elapsed.TotalMilliseconds),e.CellCount*3,30,patchCount,true));w.Flush();yield return null;yield return Resources.UnloadUnusedAssets();
   }
  }
  using(var w=new StreamWriter(Path.Combine(output,"model2_patch_performance.csv")))
  {
   w.WriteLine("layout,patches,repeat,cache_build_ms,point_queries,point_ms,nodes_visited,patch_cell_assignments");
   foreach(string layout in new[]{"distributed","overlapping"})foreach(int count in new[]{100,1000,5000})for(int repeat=0;repeat<3;repeat++)
   {
    var patches=new List<PlantingClearancePatch>();Rect bounds=e.StandBounds;int side=Mathf.CeilToInt(Mathf.Sqrt(count));
    for(int i=0;i<count;i++){float x=layout=="distributed"?bounds.xMin+(i%side+.5f)*bounds.width/side:e.Cells[0].Center.x+(i%100)*.0001f;float z=layout=="distributed"?bounds.yMin+(i/side+.5f)*bounds.height/side:e.Cells[0].Center.y+(i/100)*.0001f;patches.Add(new PlantingClearancePatch{center=new Vector3(x,0,z),radiusMeters=.564f,createdYear=i/100,competitionUpdatedYear=i/100,brambleCover=(i%7)*.1f,brackenCover=(i%3)*.2f});}
    var watch=Stopwatch.StartNew();var cache=new CompetitionExposureCache(e,m.UnderstoreyCells,patches,new int[e.CellCount]);watch.Stop();double build=watch.Elapsed.TotalMilliseconds;watch.Restart();double checksum=0;int queries=10000;
    for(int i=0;i<queries;i++){int index=layout=="overlapping"?0:i%e.CellCount;var c=e.Cells[index].Center;var p=new Vector3(c.x+(i%17-8)*.07f,0,c.y+(i%13-6)*.07f);checksum+=cache.PointCovers(index,p).x;}watch.Stop();if(double.IsNaN(checksum))throw new Exception("nonfinite query");w.WriteLine(string.Join(",",layout,count,repeat,F(build),queries,F(watch.Elapsed.TotalMilliseconds),cache.PointNodesVisited,cache.PatchCellAssignments));w.Flush();yield return null;
   }
  }
 }
}
#endif
