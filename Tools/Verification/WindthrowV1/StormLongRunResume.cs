#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CCF.Forestry.WorkEconomy;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
public static class StormLongRunResume
{
 public static void Begin(){EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");EditorApplication.isPlaying=true;}
 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]static void Install(){if(Environment.GetCommandLineArgs().Contains("StormLongRunResume.Begin"))new GameObject("Disposable storm century calibration").AddComponent<StormLongRunResumeRunner>();}
}
public sealed class StormLongRunResumeRunner:MonoBehaviour
{
 ForestEcologyController e;ScenarioOneManager m;ForestSaveController saves;ForestTreeMarkingManager marking;int worlds,checks,ordinal;
 void Check(bool ok,string reason){checks++;if(!ok)throw new Exception(reason+" / "+m.Feedback);}
 string F(double value)=>value.ToString("R",CultureInfo.InvariantCulture);
 ForestTree[] Living()=>FindObjectsByType<ForestTree>(FindObjectsSortMode.None).Where(tree=>tree.IsLiving).OrderBy(tree=>tree.TreeId,StringComparer.Ordinal).ToArray();
 double BA(ForestTree tree)=>Math.PI*Math.Pow(tree.Diameter/200.0,2);
 IEnumerator Start(){yield return null;yield return null;Exception failed=null;var work=Run();while(true){bool more=false;object current=null;try{more=work.MoveNext();if(more)current=work.Current;}catch(Exception error){failed=error;Debug.LogError("STORM_LONG_RUN_RESUME_FAIL "+error);break;}if(!more)break;yield return current;}if(failed==null)Debug.Log("STORM_LONG_RUN_RESUME_PASS worlds="+worlds+" checks="+checks);EditorApplication.ExitPlaymode();EditorApplication.Exit(failed==null?0:1);}
 void PlanFirstThin()
 {
  var live=Living();foreach(var tree in live.GroupBy(tree=>(Mathf.FloorToInt(tree.transform.position.x/10),Mathf.FloorToInt(tree.transform.position.z/10))).Select(group=>group.OrderByDescending(tree=>tree.Diameter).ThenBy(tree=>tree.TreeId,StringComparer.Ordinal).First()))marking.Mark(tree,TreeMarkType.CropTree,false);
  var selected=SelectThin(.27f);m.PlanningFellingOutcome=FellingMaterialOutcome.RetainAsFallenDeadwood;
  foreach(var tree in selected.Take(4))marking.Mark(tree,TreeMarkType.Fell,false);Check(m.AddMarkedTreesToWorkPlan()==4,"initial retained deadwood");
  m.PlanningFellingOutcome=FellingMaterialOutcome.KeepForUse;foreach(var tree in selected.Skip(4).Take(3))marking.Mark(tree,TreeMarkType.Fell,false);Check(m.AddMarkedTreesToWorkPlan()==3,"initial kept stems");
  m.PlanningFellingOutcome=FellingMaterialOutcome.SellAndExtract;foreach(var tree in selected.Skip(7))marking.Mark(tree,TreeMarkType.Fell,false);Check(m.AddMarkedTreesToWorkPlan()==selected.Length-7,"initial commissioned release");Check(m.ApprovePendingWork(),"approve first thinning");
 }
 ForestTree[] SelectThin(float fraction)
 {
  var live=Living().Where(tree=>tree.Species.SpeciesId=="sitka-spruce").ToArray();var crops=live.Where(tree=>tree.IsCropTree).ToArray();double target=live.Sum(BA)*fraction,taken=0;var selected=new List<ForestTree>();
  foreach(var tree in live.Where(tree=>!tree.IsCropTree&&tree.CanChop).OrderBy(tree=>crops.Length>0?crops.Min(crop=>Vector3.Distance(crop.transform.position,tree.transform.position)):0).ThenBy(tree=>tree.TreeId,StringComparer.Ordinal)){if(taken>=target)break;selected.Add(tree);taken+=BA(tree);}return selected.ToArray();
 }
 void PlanPlanting()
 {
  foreach(string item in new[]{"sessile-oak-sapling","beech-sapling"})
  {
   Check(m.TryPurchaseStock(item,8),"buy established-plan stock");int pairs=0;
   foreach(int cell in Enumerable.Range(0,e.CellCount).OrderByDescending(index=>e.Cells[index].Light).ThenBy(index=>index))
   {
    if(pairs==4)break;Vector2 centre=e.Cells[cell].Center;Vector3? first=null;
    for(int x=-2;x<=2;x++)for(int z=-2;z<=2;z++)
    {
     Vector3 point=new Vector3(centre.x+x*.9f,0,centre.y+z*.9f);if(first.HasValue&&Vector3.Distance(first.Value,point)<.8f)continue;
     if(!m.TryDesignateExactPlanting(item,point,first.HasValue?WorkExecutionMethod.Contractor:WorkExecutionMethod.LandownerSimulated,!first.HasValue))continue;
     if(!first.HasValue){first=point;continue;}pairs++;goto NextCell;
    }
    NextCell:;
   }
   Check(pairs==4,"four protected/exposed planting pairs per species");
  }
  Check(m.ApprovePendingWork(),"approve initial planting");
 }
 void PlanYearSix()
 {
  m.PlanningFellingOutcome=FellingMaterialOutcome.KeepForUse;var selected=Living().Where(tree=>tree.Species.SpeciesId=="sitka-spruce"&&!tree.IsCropTree&&tree.CanChop).Take(3).ToArray();foreach(var tree in selected)marking.Mark(tree,TreeMarkType.Fell,false);m.AddMarkedTreesToWorkPlan();
  Check(m.TryPurchaseStock("sessile-oak-sapling",1),"buy year6 oak");bool planted=false;
  foreach(int cell in Enumerable.Range(0,e.CellCount).OrderByDescending(index=>e.Cells[index].Light).ThenBy(index=>index))
  {var centre=e.Cells[cell].Center;for(int x=-2;x<=2&&!planted;x++)for(int z=-2;z<=2&&!planted;z++)planted=m.TryDesignateExactPlanting("sessile-oak-sapling",new Vector3(centre.x+x*.9f+.3f,0,centre.y+z*.9f+.3f),WorkExecutionMethod.LandownerSimulated,true);if(planted)break;}
  Check(planted&&m.ApprovePendingWork(),"year6 common-plan response");
 }
 IEnumerator Run()
 {
  e=FindFirstObjectByType<ForestEcologyController>();m=FindFirstObjectByType<ScenarioOneManager>();saves=FindFirstObjectByType<ForestSaveController>();marking=FindFirstObjectByType<ForestTreeMarkingManager>();var initial=saves.CaptureData();var authoredIds=new HashSet<string>(initial.trees.Select(tree=>tree.treeId),StringComparer.Ordinal);string dir=Environment.GetEnvironmentVariable("CCF_STORM_OUTPUT");bool pilot=Environment.GetEnvironmentVariable("CCF_STORM_PILOT")=="1";
  using(var years=new StreamWriter(Path.Combine(dir,"storm_long_run_years.csv")))using(var horizons=new StreamWriter(Path.Combine(dir,"storm_long_run_horizons.csv")))using(var events=new StreamWriter(Path.Combine(dir,"storm_long_run_events.csv")))
  {
   string header="run,seed,frequency,weights,managed,year,reached_horizon,completed,completion_year,cash_cents,minimum_cash_cents,living_trees,original_living,crops,original_crops_lost,canopy,light,regeneration_cells,regeneration_density,planted_alive,promoted_living,bramble,bracken,deadwood_m3,storm_events,windthrow_victims,windthrow_volume_m3,failed_objectives";
   years.WriteLine(header);horizons.WriteLine(header);events.WriteLine("run,seed,year,intensity,direction,victims,crops_lost,volume_m3,affected_cells,canopy_rebuilds,seed_rebuilds");
   foreach(float frequency in pilot?new[]{0f,.02f}:new[]{0f,.01f,.02f,.03f,.05f})
   foreach(int weights in frequency==0||pilot?new[]{0}:new[]{0,1})
   foreach(bool managed in pilot?new[]{true}:new[]{false,true})
   foreach(int seedIndex in pilot?new[]{0}:Enumerable.Range(0,8))
   {
    if(ordinal++<40)continue;
    int seed=initial.simulationSeed+seedIndex*7919;string id="f"+F(frequency)+"-w"+weights+"-m"+managed+"-s"+seedIndex;
    Check(saves.LoadData(initial,false),"new calibration world");yield return null;e.SimulationSeed=seed;e.Browsing.BackgroundPressure=.2f;e.StormModelVersion=frequency>0?1:0;
    e.UseStormCalibrationForVerification(new StormCalibration(frequency,new[]{.02f,.06f,.18f},weights==0?new[]{1f,1f,1f}:new[]{4f,2f,1f}));
    long minimumCash=m.CashCents;int cumulativeWind=0,cumulativeCropLoss=0;float cumulativeWindVolume=0;
    if(managed)PlanFirstThin();
    for(int year=1;year<=100;year++)
    {
     if(managed&&year==2)PlanPlanting();if(managed&&year==7)PlanYearSix();
     if(managed&&year==17){m.PlanningFellingOutcome=FellingMaterialOutcome.SellAndExtract;var selected=SelectThin(.2f);foreach(var tree in selected)marking.Mark(tree,TreeMarkType.Fell,false);if(selected.Length>0){m.AddMarkedTreesToWorkPlan();Check(m.ApprovePendingWork(),"approve second common-plan intervention");}}
     if(!m.AdvanceYear())break;yield return null;minimumCash=Math.Min(minimumCash,m.CashCents);
     var storm=m.StormDamageInYear(year);if(storm!=null){cumulativeWind+=storm.TreesLost;cumulativeCropLoss+=storm.Event.cropTreesLost;cumulativeWindVolume+=storm.OriginalVolumeM3;events.WriteLine(string.Join(",",id,seed,year,F(storm.Event.severity),F(storm.Event.directionDegrees),storm.TreesLost,storm.Event.cropTreesLost,F(storm.OriginalVolumeM3),storm.AffectedCells,e.LastStormPerformance.CanopyRebuilds,e.LastStormPerformance.SeedRebuilds));}
     var live=Living();var snapshot=m.EcologicalSnapshots.Last();string failedObjectives=string.Join(";",m.Objectives.Where(objective=>!objective.achieved).Select(objective=>objective.objectiveId));
     string row=string.Join(",",id,seed,F(frequency),weights==0?"1:1:1":"4:2:1",managed,year,true,m.Outcome==ScenarioOneOutcome.Completed,m.OutcomeYear,m.CashCents,minimumCash,live.Length,live.Count(tree=>authoredIds.Contains(tree.TreeId)),live.Count(tree=>tree.IsCropTree),cumulativeCropLoss,F(snapshot.meanCanopy),F(e.Cells.Average(cell=>cell.Light)),snapshot.occupiedRegenerationCells,F(e.Cells.Sum(cell=>cell.Regeneration.Sum(band=>band.Density))),m.PlantedJuveniles.Count(juvenile=>juvenile.alive&&string.IsNullOrEmpty(juvenile.promotedTreeId)),live.Count(tree=>!authoredIds.Contains(tree.TreeId)),F(m.UnderstoreyCells.Average(cell=>cell.brambleCover)),F(m.UnderstoreyCells.Average(cell=>cell.brackenCover)),F(ScenarioDeadwood.TotalVolume(m.DeadwoodRecords)),m.StormEvents.Count,cumulativeWind,F(cumulativeWindVolume),failedObjectives);
     years.WriteLine(row);if(year==25||year==50||year==100)horizons.WriteLine(row);
     Check(m.CashCents>=0,"no negative cash");
     if(m.Outcome==ScenarioOneOutcome.Failed&&year<100){Debug.Log("STORM_LONG_RUN_EARLY_STOP "+id+" year="+year+" reason="+m.Feedback);break;}
    }
    worlds++;years.Flush();horizons.Flush();events.Flush();Debug.Log("STORM_LONG_RUN_WORLD_DONE "+id+" year="+e.EcologicalYear+" completed="+(m.Outcome==ScenarioOneOutcome.Completed)+" completionYear="+m.OutcomeYear+" wind="+cumulativeWind);
   }
  }
  Check(saves.LoadData(initial,false),"restore longrun world");yield return null;
 }
}
#endif
