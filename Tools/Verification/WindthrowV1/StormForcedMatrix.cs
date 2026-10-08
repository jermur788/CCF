#if UNITY_EDITOR
using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class StormForcedMatrix
{
 public static void Begin(){EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");EditorApplication.isPlaying=true;}
 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]static void Install(){if(Environment.GetCommandLineArgs().Contains("StormForcedMatrix.Begin"))new GameObject("Disposable forced storm matrix").AddComponent<StormForcedMatrixRunner>();}
}
public sealed class StormForcedMatrixRunner:MonoBehaviour
{
 ForestEcologyController e;ScenarioOneManager m;ForestSaveController saves;ForestTreeMarkingManager marking;int checks,cases;
 void Check(bool ok,string reason){checks++;if(!ok)throw new Exception(reason);}
 string F(double value)=>value.ToString("R",CultureInfo.InvariantCulture);
 IEnumerator Start()
 {
  yield return null;yield return null;Exception failed=null;var work=Run();
  while(true){bool more=false;object current=null;try{more=work.MoveNext();if(more)current=work.Current;}catch(Exception error){failed=error;Debug.LogError("STORM_FORCED_MATRIX_FAIL "+error);break;}if(!more)break;yield return current;}
  if(failed==null)Debug.Log("STORM_FORCED_MATRIX_PASS cases="+cases+" checks="+checks);
  EditorApplication.ExitPlaymode();EditorApplication.Exit(failed==null?0:1);
 }
 ForestTree[] Living()=>FindObjectsByType<ForestTree>(FindObjectsSortMode.None).Where(tree=>tree.IsLiving).OrderBy(tree=>tree.TreeId,StringComparer.Ordinal).ToArray();
 double BA(ForestTree tree)=>Math.PI*Math.Pow(tree.Diameter/200.0,2);
 bool RecentTreatment=>Environment.GetEnvironmentVariable("CCF_STORM_RECENT")=="1";
 void PlanTreatment(int treatment)
 {
    var living=Living();foreach(ForestTree tree in living.Where((tree,index)=>index%12==0))marking.Mark(tree,TreeMarkType.CropTree,false);
    double fraction=treatment==1?.15:treatment==2?.3:treatment==3?.5:0;
    double target=living.Sum(BA)*fraction,removed=0;
    var eligible=living.Where(tree=>!tree.IsCropTree);
    var ordered=treatment==3?eligible.OrderBy(tree=>tree.transform.position.x).ThenBy(tree=>tree.TreeId,StringComparer.Ordinal):eligible.OrderBy(tree=>tree.Diameter).ThenBy(tree=>tree.TreeId,StringComparer.Ordinal);
    foreach(ForestTree tree in ordered){if(removed>=target)break;marking.Mark(tree,TreeMarkType.Fell,false);removed+=BA(tree);}
    if(treatment>0)Check(m.AddMarkedTreesToWorkPlan()>0&&m.ApprovePendingWork(),"paid thinning plan");
 }
 IEnumerator Run()
 {
  e=FindFirstObjectByType<ForestEcologyController>();m=FindFirstObjectByType<ScenarioOneManager>();saves=FindFirstObjectByType<ForestSaveController>();marking=FindFirstObjectByType<ForestTreeMarkingManager>();var initial=saves.CaptureData();
  string dir=Environment.GetEnvironmentVariable("CCF_STORM_OUTPUT");
  using(var summary=new StreamWriter(Path.Combine(dir,"forced_candidate_summary.csv")))
  using(var individual=new StreamWriter(Path.Combine(dir,"forced_candidate_trees.csv")))
  using(var causal=new StreamWriter(Path.Combine(dir,"forced_causal_outcomes.csv")))
  {
   summary.WriteLine("treatment,scenario_year,authored_age,candidate,transform,intensity,scale_to_reference,trees,victims,volume_lost_m3,ba_lost_m2,mean_v,mean_hd,mean_victim_hd,mean_victim_height");
   individual.WriteLine("treatment,year,candidate,transform,intensity,tree_id,height_m,dbh_cm,hd,local_top_m,light,recent_opening,vulnerability,failure_chance,victim,crop");
   causal.WriteLine("treatment,year,intensity,victims,crop_losses,deadwood_added_m3,affected_cell_area_m2,pre_light,control_light,storm_light,control_cover,storm_cover,control_regen,storm_regen,canopy_rebuilds,seed_rebuilds,evaluation_ms,mortality_ms,rebuild_ms,records_ms,visuals_ms,cash_cents");
   foreach(int treatment in new[]{0,1,2,3})
   {
    Check(saves.LoadData(initial,false),"restore starting world");yield return null;e.Browsing.BackgroundPressure=.2f;
    if(!RecentTreatment)PlanTreatment(treatment);
    ForestTree[] living;

    foreach(int checkpoint in new[]{5,20,40})
    {
     if(RecentTreatment)
     {
      Check(saves.LoadData(initial,false),"fresh-event timing restore");yield return null;e.Browsing.BackgroundPressure=.2f;
      while(e.EcologicalYear<checkpoint-1){Check(m.AdvanceYear(),"unthinned pre-treatment development");yield return null;}
      PlanTreatment(treatment);
     }
     while(e.EcologicalYear<checkpoint){Check(m.AdvanceYear(),"baseline treatment annual advance: "+m.Feedback);yield return null;}
     var state=saves.CaptureData();living=Living();float beforeLight=e.Cells.Average(cell=>cell.Light);
     foreach(StormVulnerabilityCandidate candidate in Enum.GetValues(typeof(StormVulnerabilityCandidate)))
     foreach(StormProbabilityTransform transform in Enum.GetValues(typeof(StormProbabilityTransform)))
     foreach(float level in new[]{.02f,.06f,.18f})
     {
      // A different raw index is not rejected merely for using different units.
      float scale=candidate==StormVulnerabilityCandidate.CurrentDiagnostic?1.3f/43.75f:candidate==StormVulnerabilityCandidate.HeightAndSlenderness?1.3f/(70f/75f):1f;
      var profile=new StormCalibration(0,new[]{.02f*scale,.06f*scale,.18f*scale},new[]{1f,1f,1f},candidate,transform);
      e.UseStormCalibrationForVerification(profile);
      var values=e.EvaluateStorm(new StormEventRecord{year=checkpoint+1,severity=level*scale,directionDegrees=135});var losses=values.Where(value=>value.Victim).ToArray();
      summary.WriteLine(string.Join(",",treatment,checkpoint,20+checkpoint,candidate,transform,F(level*scale),F(scale),values.Count,losses.Length,F(losses.Sum(value=>(double)value.Tree.BiologicalStemVolumeM3)),F(losses.Sum(value=>BA(value.Tree))),F(values.Average(value=>value.Vulnerability)),F(values.Average(value=>value.Context.Height/(value.Context.DiameterCm/100))),F(losses.Length>0?losses.Average(value=>value.Context.Height/(value.Context.DiameterCm/100)):0),F(losses.Length>0?losses.Average(value=>value.Context.Height):0)));
      foreach(var value in values)individual.WriteLine(string.Join(",",treatment,checkpoint,candidate,transform,F(level*scale),value.Tree.TreeId,F(value.Context.Height),F(value.Context.DiameterCm),F(value.Context.Height/(value.Context.DiameterCm/100)),F(value.Context.LocalTopHeight),F(value.Context.Light),F(value.Context.RecentOpening),F(value.Vulnerability),F(value.FailureChance),value.Victim,value.WasCropTree));
      Check(values.All(value=>value.FailureChance>=0&&value.FailureChance<1),"bounded probability");cases++;
     }
     // Resolve the provisional simplest candidate through the actual annual path.
     Check(saves.LoadData(state,false),"control checkpoint restore");yield return null;Check(m.AdvanceYear(),"control annual");yield return null;
     float controlLight=e.Cells.Average(cell=>cell.Light),controlCover=m.UnderstoreyCells.Average(cell=>Mathf.Max(cell.brambleCover,cell.brackenCover));
     float controlRegen=e.Cells.Sum(cell=>cell.Regeneration.Sum(band=>band.Density));
     foreach(float level in new[]{.02f,.06f,.18f})
     {
      Check(saves.LoadData(state,false),"forced checkpoint restore");yield return null;e.StormModelVersion=1;e.UseStormCalibrationForVerification(new StormCalibration(0,new[]{.02f,.06f,.18f},new[]{1f,1f,1f}));
      e.ForceStormNextYear(level,135);Check(m.AdvanceYear(),"forced annual path");yield return null;
      var performance=e.LastStormPerformance;var victims=e.LastStormEvaluations.Where(value=>value.Victim).ToArray();
      Check(performance.CanopyRebuilds==(victims.Length>0?1:0)&&performance.SeedRebuilds==(victims.Length>0?1:0),"batched forced event");
      Check(m.StormEvents.Count==1&&m.StormEvents[0].cropTreesLost==victims.Count(value=>value.WasCropTree),"one event retains crop losses");
      Check(victims.All(value=>m.DeadwoodRecords.Count(record=>record.treeId==value.Tree.TreeId)==1),"one deadwood record per selected victim");
      causal.WriteLine(string.Join(",",treatment,checkpoint,F(level),victims.Length,m.StormEvents[0].cropTreesLost,F(m.DeadwoodRecords.Where(record=>record.fallenYear==checkpoint+1&&victims.Any(value=>value.Tree.TreeId==record.treeId)).Sum(record=>record.originalVolumeM3)),victims.Select(value=>value.CellIndex).Distinct().Count()*e.CellSizeMeters*e.CellSizeMeters,F(beforeLight),F(controlLight),F(e.Cells.Average(cell=>cell.Light)),F(controlCover),F(m.UnderstoreyCells.Average(cell=>Mathf.Max(cell.brambleCover,cell.brackenCover))),F(controlRegen),F(e.Cells.Sum(cell=>cell.Regeneration.Sum(band=>band.Density))),performance.CanopyRebuilds,performance.SeedRebuilds,F(performance.EvaluationMilliseconds),F(performance.MortalityMilliseconds),F(performance.RebuildMilliseconds),F(performance.DeadwoodMilliseconds),F(performance.VisualMilliseconds),m.CashCents));
     }
     Check(saves.LoadData(state,false),"continue undamaged treatment timeline");yield return null;
    }
    Debug.Log("STORM_FORCED_MATRIX_TREATMENT_DONE "+treatment);summary.Flush();individual.Flush();causal.Flush();
   }
  }
  Check(saves.LoadData(initial,false),"restore matrix world");yield return null;
 }
}
#endif
