#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using CCF.Forestry.WorkEconomy;
public static class Model2Matrix
{
 public static void Begin(){EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");EditorApplication.isPlaying=true;}
 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]static void Install(){if(Environment.GetCommandLineArgs().Contains("Model2Matrix.Begin"))new GameObject("Model2 matrix").AddComponent<Model2MatrixRunner>();}
}
public sealed class Model2MatrixRunner:MonoBehaviour
{
 sealed class Case
 {
  public string Id;public float Thin,Browse;public int ClearYear,Interval;public bool Shelters;public float Dense=-1;
  public UnderstoreyCompetitionCalibration Calibration=new UnderstoreyCompetitionCalibration();public int Repeat;
 }
 ForestEcologyController e;ScenarioOneManager m;ForestSaveController saves;ForestTreeMarkingManager marking;ForestSaveData original;StreamWriter years,horizons;string output;
 string F(float v)=>v.ToString("R",CultureInfo.InvariantCulture);
 void Check(bool ok,string why){if(!ok)throw new Exception(why+" / "+m.Feedback);}
 IEnumerator Start()
 {
  yield return null;yield return null;Exception error=null;
  e=FindFirstObjectByType<ForestEcologyController>();m=FindFirstObjectByType<ScenarioOneManager>();saves=FindFirstObjectByType<ForestSaveController>();marking=FindFirstObjectByType<ForestTreeMarkingManager>();original=saves.CaptureData();output=Environment.GetEnvironmentVariable("CCF_MODEL2_OUTPUT");
  using(years=new StreamWriter(Path.Combine(output,"model2_years.csv")))using(horizons=new StreamWriter(Path.Combine(output,"model2_horizons.csv")))
  {
   string header="run,repeat,year,species,mean_bramble,mean_bracken,natural_remaining,vegetation_loss,light_loss,browse_loss,cumulative_natural_promotions,first_promotion_year,planted_alive,planted_promoted,planted_vegetation_deaths,planted_light_deaths,planted_browse_deaths,clearance_removals_relative,clearance_removed_planted,clearance_count,clearance_cost_cents,planting_count,shelter_count,cash_cents,living_adults,total_basal_area_m2,world_hash";
   years.WriteLine(header);horizons.WriteLine(header);
   var stack=new Stack<IEnumerator>();stack.Push(Run());
   while(stack.Count>0){bool more=false;object next=null;try{more=stack.Peek().MoveNext();if(more)next=stack.Peek().Current;}catch(Exception ex){error=ex;break;}if(!more){stack.Pop();continue;}if(next is IEnumerator nested){stack.Push(nested);continue;}yield return next;}
  }
  if(error!=null)Debug.LogError("MODEL2MATRIX_FAIL "+error);else Debug.Log("MODEL2MATRIX_PASS horizons=10,25,50,100 paidWorkPlan=true");
  EditorApplication.ExitPlaymode();EditorApplication.Exit(error==null?0:1);
 }
 uint Rank(string id){uint value=2166136261;foreach(char c in id){value^=c;value*=16777619;}return value;}
 void PlanStart(Case c)
 {
  Check(saves.LoadData(JsonUtility.FromJson<ForestSaveData>(JsonUtility.ToJson(original)),false),"restore");
  m.CompetitionCalibration=c.Calibration;e.Browsing.BackgroundPressure=c.Browse;
  foreach(var u in m.UnderstoreyCells){UnderstoreyCompetition.Initialize(u,e.Cells[u.cellIndex],c.Calibration);if(c.Dense>=0)u.brambleCover=u.brackenCover=c.Dense;}
  m.RebuildCompetitionExposure();
  var live=FindObjectsByType<ForestTree>(FindObjectsSortMode.None).Where(t=>t.IsLiving).OrderBy(t=>Rank(t.TreeId)).ThenBy(t=>t.TreeId,StringComparer.Ordinal).ToList();
  int count=Mathf.RoundToInt(live.Count*c.Thin);
  foreach(var tree in live.Take(count))marking.Mark(tree,TreeMarkType.Fell,false);
  if(count>0)Check(m.AddMarkedTreesToWorkPlan()==count,"paid distributedthin");
  if(c.ClearYear==1)PlanClear();
  foreach(string item in new[]{"sessile-oak-sapling","beech-sapling"})
  {
   Check(m.TryPurchaseStock(item,6),"buystock");int placed=0;
   for(int j=0;j<e.CellCount&&placed<6;j++)
   {
    int index=(j*17+(item.StartsWith("beech")?3:0))%e.CellCount;var centre=e.Cells[index].Center;
    for(int x=-2;x<=2&&placed<6;x++)for(int z=-2;z<=2&&placed<6;z++)
    {
     Vector3 point=new Vector3(centre.x+x*.8f,0,centre.y+z*.8f);
     if(m.TryDesignateExactPlanting(item,point,WorkExecutionMethod.Contractor,c.Shelters)){placed++;goto NextCell;}
    }
    NextCell:;
   }
   Check(placed==6,"exactplanting");
  }
 }
 void PlanClear(){for(int i=0;i<e.CellCount;i++)m.TryDesignateVegetationClearance(i);}
 IEnumerator Run()
 {
  var cases=new List<Case>();
  foreach(float thin in new[]{0f,.2f,.4f,.6f})
  {
   foreach(int timing in new[]{0,1,3})cases.Add(new Case{Id="thin"+F(thin)+"-clear"+timing,Thin=thin,ClearYear=timing,Interval=timing==3?5:0});
   cases.Add(new Case{Id="thin"+F(thin)+"-highbrowse",Thin=thin,Browse=.8f});
  }
  cases.Add(new Case{Id="moderate-highbrowse-sheltered",Thin=.4f,Browse=.8f,Shelters=true});
  cases.Add(new Case{Id="moderate-highbrowse-sheltered-clear",Thin=.4f,Browse=.8f,Shelters=true,ClearYear=1});
  foreach(int clear in new[]{0,1,3})cases.Add(new Case{Id="dense-opening-clear"+clear,Thin=.6f,Dense=.9f,ClearYear=clear});
  foreach(int clear in new[]{0,1})cases.Add(new Case{Id="lowcompetition-clear"+clear,Thin=.4f,ClearYear=clear,Calibration=new UnderstoreyCompetitionCalibration{TargetMaximum=.05f}});
  // One-at-a-time sensitivity, not an unnecessary full Cartesian grid.
  cases.Add(new Case{Id="strength-low",Thin=.4f,Calibration=new UnderstoreyCompetitionCalibration{Strength=.15f}});
  cases.Add(new Case{Id="strength-high",Thin=.4f,Calibration=new UnderstoreyCompetitionCalibration{Strength=.6f}});
  cases.Add(new Case{Id="escape-short",Thin=.4f,Calibration=new UnderstoreyCompetitionCalibration{EscapeHeight=1f}});
  cases.Add(new Case{Id="escape-tall",Thin=.4f,Calibration=new UnderstoreyCompetitionCalibration{EscapeHeight=2.5f}});
  cases.Add(new Case{Id="recovery-slow",Thin=.4f,ClearYear=1,Calibration=new UnderstoreyCompetitionCalibration{Gain=.15f,Loss=.25f}});
  cases.Add(new Case{Id="recovery-fast",Thin=.4f,ClearYear=1,Calibration=new UnderstoreyCompetitionCalibration{Gain=.45f,Loss=.65f}});
  cases.Add(new Case{Id="initial-low",Thin=.4f,Calibration=new UnderstoreyCompetitionCalibration{InitialFraction=.1f}});
  cases.Add(new Case{Id="initial-high",Thin=.4f,Calibration=new UnderstoreyCompetitionCalibration{InitialFraction=.5f}});
  cases.Add(new Case{Id="thin0.4-clear0",Thin=.4f,Repeat=1});
  cases.Add(new Case{Id="dense-opening-clear1",Thin=.6f,Dense=.9f,ClearYear=1,Repeat=1});
  var hashes=new Dictionary<string,string>();int repeats=0;
  foreach(var c in cases)
  {
   PlanStart(c);yield return null;
   var promotions=new Dictionary<string,int>();var first=new Dictionary<string,int>();var vloss=new Dictionary<string,float>();var lloss=new Dictionary<string,float>();var bloss=new Dictionary<string,float>();
   var pveg=new Dictionary<string,int>();var plight=new Dictionary<string,int>();var pbrowse=new Dictionary<string,int>();
   foreach(string id in new[]{"sitka-spruce","sessile-oak","beech"}){promotions[id]=0;first[id]=-1;vloss[id]=lloss[id]=bloss[id]=0;pveg[id]=plight[id]=pbrowse[id]=0;}
   float removed=0;int removedPlants=0;long costs=0;int clearCount=0;
   for(int year=1;year<=100;year++)
   {
    if(year>1&&c.ClearYear>0&&(year==c.ClearYear||c.Interval>0&&year>c.ClearYear&&(year-c.ClearYear)%c.Interval==0))PlanClear();
    var pending=m.WorkOrders.Where(o=>o.status==ScenarioWorkStatus.Pending).ToList();
    int existingAlive=m.PlantedJuveniles.Count(j=>j.alive&&string.IsNullOrEmpty(j.promotedTreeId));
    int willRemove=m.WorkOrders.Where(o=>o.status==ScenarioWorkStatus.Pending&&o.type==ScenarioWorkType.RemoveRegeneration).SelectMany(o=>m.QueryClearance(ClearanceFootprint.Cell(e,o.cellIndex)).Juveniles).Distinct().Count();
    if(pending.Count>0)Check(m.ApprovePendingWork(),"approval");
    Check(m.AdvanceYear(),"annualstep"+c.Id+"year"+year);
    removedPlants+=willRemove;
    foreach(var order in m.WorkOrders.Where(o=>o.status==ScenarioWorkStatus.Completed&&o.resolvedYear==year&&o.type==ScenarioWorkType.RemoveRegeneration)){removed+=order.expectedRegenerationDensity;clearCount++;costs+=order.estimatedCostCents;}
    string hash="";if(new[]{10,25,50,100}.Contains(year))
    {
     hash=ScenarioReferenceArchive.WorldHash(saves.CaptureData());string key=c.Id+"/"+year;
     if(c.Repeat==0)hashes[key]=hash;else{Check(hashes[key]==hash,"repeatmodel2horizon");repeats++;}
    }
    foreach(string id in new[]{"sitka-spruce","sessile-oak","beech"})
    {
     var a=e.LastRegenerationAccount.For(id);promotions[id]+=a.ExactTreesCreated;vloss[id]+=a.VegetationLoss;lloss[id]+=a.LightLoss;bloss[id]+=a.BrowseLoss;
     var pa=m.PlantedCompetitionAccount.For(id);pveg[id]+=pa.VegetationDeaths;plight[id]+=pa.LightDeaths;pbrowse[id]+=pa.BrowseDeaths;
     if(first[id]<0&&(a.ExactTreesCreated>0||pa.Promoted>0))first[id]=year;
     float remaining=e.Cells.Sum(cell=>cell.Regeneration.Where(b=>b.SpeciesId==id).Sum(b=>b.Density));
     float residual=a.StartingAbundance+a.EstablishmentAccepted-a.VegetationLoss-a.LightLoss-a.BrowseLoss-a.PromotionExported-remaining;
     Check(Mathf.Abs(residual)<.002f,"relativeaccountbalance "+id+" "+F(residual));
     Check(pa.Starting==pa.VegetationDeaths+pa.LightDeaths+pa.BrowseDeaths+pa.Remaining+pa.Promoted,"plantedaccountbalance");
     var adults=FindObjectsByType<ForestTree>(FindObjectsSortMode.None).Where(t=>t.IsLiving).ToArray();
     string line=string.Join(",",c.Id,c.Repeat,year,id,F(m.UnderstoreyCells.Average(u=>u.brambleCover)),F(m.UnderstoreyCells.Average(u=>u.brackenCover)),F(remaining),F(vloss[id]),F(lloss[id]),F(bloss[id]),promotions[id],first[id],m.PlantedJuveniles.Count(j=>j.speciesId==id&&j.alive&&string.IsNullOrEmpty(j.promotedTreeId)),m.PlantedJuveniles.Count(j=>j.speciesId==id&&!string.IsNullOrEmpty(j.promotedTreeId)),pveg[id],plight[id],pbrowse[id],F(removed),removedPlants,clearCount,costs,m.PlantedJuveniles.Count,m.Shelters.Count,m.CashCents,adults.Length,F(adults.Sum(t=>Mathf.PI*Mathf.Pow(t.Diameter*.01f*.5f,2))),hash);
     years.WriteLine(line);if(hash!="")horizons.WriteLine(line);
    }
    yield return null;
   }
   years.Flush();horizons.Flush();Debug.Log("MODEL2_MATRIX_CASE_PASS "+c.Id+" repeat="+c.Repeat);yield return Resources.UnloadUnusedAssets();GC.Collect();
  }
  File.WriteAllText(Path.Combine(output,"matrix_summary.json"),"{\"worlds\":"+cases.Count+",\"years\":100,\"repeatHorizonMatches\":"+repeats+",\"realPaidWork\":true,\"naturalBalanceTolerance\":0.002}");
 }
}
#endif
