#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CCF.Forestry.WorkEconomy;
using CCF.Forestry.TimberYield;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
public static class StormSalvageVerification
{
 public static void Begin(){EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");EditorApplication.isPlaying=true;}
 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]static void Install(){if(Environment.GetCommandLineArgs().Contains("StormSalvageVerification.Begin"))new GameObject("Disposable storm salvage gate").AddComponent<StormSalvageVerificationRunner>();}
}
public sealed class StormSalvageVerificationRunner:MonoBehaviour
{
 int checks;void Check(bool ok,string reason){checks++;if(!ok)throw new Exception(reason);}
 IEnumerator Start(){yield return null;yield return null;Exception failed=null;var work=Run();while(true){bool more=false;object current=null;try{more=work.MoveNext();if(more)current=work.Current;}catch(Exception error){failed=error;Debug.LogError("STORM_SALVAGE_VERIFICATION_FAIL "+error);break;}if(!more)break;yield return current;}if(failed==null)Debug.Log("STORM_SALVAGE_VERIFICATION_PASS checks="+checks);EditorApplication.ExitPlaymode();EditorApplication.Exit(failed==null?0:1);}
 IEnumerator Run()
 {
  var e=FindFirstObjectByType<ForestEcologyController>();var m=FindFirstObjectByType<ScenarioOneManager>();var saves=FindFirstObjectByType<ForestSaveController>();var marking=FindFirstObjectByType<ForestTreeMarkingManager>();var initial=saves.CaptureData();
  for(int year=0;year<20;year++){Check(m.AdvanceYear(),"quiet base annual");yield return null;}
  var beforeDamage=saves.CaptureData();
  e.StormModelVersion=1;e.UseStormCalibrationForVerification(new StormCalibration(0,new[]{.02f,.06f,.18f},new[]{1f,1f,1f}));e.ForceStormNextYear(.18f,135);long beforeStormCash=m.CashCents;Check(m.AdvanceYear(),"forced damage annual");yield return null;
  Check(m.CashCents==beforeStormCash,"leaving windthrow creates no contractor bill");
  var baseline=saves.CaptureData();var fallen=m.DeadwoodRecords.Where(record=>record.fallenYear==21&&record.remainingVolumeM3>0).OrderByDescending(record=>record.originalDiameterCm).ToArray();Check(fallen.Length>=10,"enough eligible stems for partial selection");
  Check(m.AnnualReports.Last().deadwoodCreated==e.LastStormPerformance.Victims,"storm creation reconciles annual report");
  var selected=fallen.Take(7).Select(record=>record.treeId).ToArray();
  foreach(string id in selected.Take(5))Check(m.TryDesignateSalvage(id,FellingMaterialOutcome.SellAndExtract),"designate sale salvage");
  foreach(string id in selected.Skip(5))Check(m.TryDesignateSalvage(id,FellingMaterialOutcome.KeepForUse),"designate kept salvage");
  Check(!m.TryDesignateSalvage(selected[0],FellingMaterialOutcome.SellAndExtract),"duplicate selection rejected");
  Check(!m.TryDesignateSalvage(fallen[7].treeId,FellingMaterialOutcome.RetainAsFallenDeadwood),"leave uses absence of order, no paid pseudo task");
  var living=FindObjectsByType<ForestTree>(FindObjectsSortMode.None).Where(tree=>tree.IsLiving&&tree.CanChop).OrderByDescending(tree=>tree.Diameter).Take(2).ToArray();
  foreach(ForestTree tree in living)marking.Mark(tree,TreeMarkType.Fell,false);
  Check(m.AddMarkedTreesToWorkPlan()==2,"mixed living harvest joins work plan");
  var quote=m.GetHarvestQuote();Check(quote.Eligible&&quote.Orders.Count==9,"one mixed harvest and salvage job");
  Check(quote.CostCents>=m.Definition.MinimumHarvestJobCents&&quote.Resolution.Ledger.Count(row=>row.Category==LedgerCategory.MinimumJobAdjustment)<=1,"one existing contractor minimum");
  Check(quote.TotalStemVolumeCm3==quote.SoldVolumeCm3+quote.RetainedVolumeCm3+quote.DeadwoodVolumeCm3+quote.ResidualVolumeCm3,"salvage yield conserves supplied volume");
  var allTrees=FindObjectsByType<ForestTree>(FindObjectsInactive.Include,FindObjectsSortMode.None).ToDictionary(tree=>tree.TreeId,StringComparer.Ordinal);
  var fallenById=fallen.ToDictionary(record=>record.treeId,StringComparer.Ordinal);var salvageOrders=quote.Orders.Where(order=>order.type==ScenarioWorkType.SalvageDeadwood).ToArray();
  string dir=Environment.GetEnvironmentVariable("CCF_STORM_OUTPUT");
  using(var table=new StreamWriter(Path.Combine(dir,"salvage_multiplier_comparison.csv")))
  {
   table.WriteLine("basis_points,orders,volume_cm3,sold_cm3,kept_cm3,residual_cm3,cost_cents,revenue_cents,minimum_adjustment_cents");long previous=0;
   foreach(int multiplier in new[]{10000,11500,12500,15000})
   {var candidate=ScenarioOneEconomyAdapter.QuoteHarvest(salvageOrders,allTrees,m.Definition,22,0,m.CashCents,fallenById,multiplier);Check(candidate.Eligible&&candidate.CostCents>=previous,"candidate costs monotone and eligible");Check(candidate.Resolution.Ledger.Count(row=>row.Category==LedgerCategory.MinimumJobAdjustment)<=1,"candidate minimum charged once");previous=candidate.CostCents;table.WriteLine(string.Join(",",multiplier,candidate.Orders.Count,candidate.TotalStemVolumeCm3,candidate.SoldVolumeCm3,candidate.RetainedVolumeCm3,candidate.ResidualVolumeCm3,candidate.CostCents,candidate.RevenueCents,candidate.Resolution.Quote.Costs.MinimumJobAdjustmentCents));}
  }
  var stem=WindthrowSalvage.Measurements(fallen[0],22);Check(stem.QualitySections.Length==1&&stem.QualitySections[0].Flags==StemQualityFlags.WindDamage,"existing damage grading used");
  var delayed=WindthrowSalvage.Measurements(fallen[0],26);Check(delayed.QualitySections[0].EndHeightMm>stem.QualitySections[0].EndHeightMm,"delay increases candidate damage window");
  var request=new StemYieldRequest{Stem=stem,DefaultDisposition=TimberDisposition.SellAndExtract,ExtractRetainedToRoadside=true};
  var damaged=TimberYieldCalculator.ResolveStandOperation("damaged",new[]{request},TimberYieldDefaults.CreateSitka());
  stem.QualitySections=Array.Empty<StemQualitySection>();var sound=TimberYieldCalculator.ResolveStandOperation("sound",new[]{request},TimberYieldDefaults.CreateSitka());
  Check(damaged.Stems[0].Logs.Where(log=>log.Assortment==TimberAssortment.Sawlog).Sum(log=>log.VolumeCm3)<=sound.Stems[0].Logs.Where(log=>log.Assortment==TimberAssortment.Sawlog).Sum(log=>log.VolumeCm3),"damage cannot increase premium-grade volume");
  var pending=saves.CaptureData();Check(saves.LoadData(pending,false),"pending salvage save/load");yield return null;
  e.UseStormCalibrationForVerification(new StormCalibration(0,new[]{.02f,.06f,.18f},new[]{1f,1f,1f}));
  Check(m.GetHarvestQuote().CostCents==quote.CostCents&&m.GetHarvestQuote().RevenueCents==quote.RevenueCents,"pending quote stable after load");
  Check(m.ApprovePendingWork(),"approve mixed job");quote=m.GetHarvestQuote(true);Check(m.ReservedContractorCashCents==quote.CostCents,"cash reserved once for mixed job");
  long cash=m.CashCents;float retained=m.RetainedTimberM3;int historicalRecords=m.DeadwoodRecords.Count;var opening=e.Cells.Select(cell=>cell.RecentOpening).ToArray();
  Check(m.AdvanceYear(),"resolve mixed job");yield return null;
  Check(m.CashCents-cash==quote.Resolution.ExternalCashFlowCents,"one exact mixed-job settlement");
  Check(Mathf.Abs(m.RetainedTimberM3-retained-quote.RetainedVolumeCm3/1000000f)<.00001f,"kept timber deposited once");
  foreach(string id in selected){Check(m.DeadwoodRecords.Single(record=>record.treeId==id).remainingVolumeM3==0,"selected stem extracted with root-plate history retained");Check(!m.CanSalvage(id)&&!m.TryDesignateSalvage(id,FellingMaterialOutcome.SellAndExtract),"cannot salvage twice");var visual=m.GetComponentsInChildren<ScenarioWindthrowVisual>().Single(value=>value.TreeId==id);visual.SetCrownVisible(true);m.GetComponentInChildren<ScenarioWindthrowVisualBudget>().RefreshAt(visual.transform.position);Check(!visual.HasCrown&&!visual.transform.Find("Directed fallen stem").gameObject.activeSelf&&!visual.GetComponent<BoxCollider>().enabled,"salvaged stem/crown/target removed, root plate remains");}
  Check(fallen.Skip(7).All(record=>m.DeadwoodRecords.Single(item=>item.treeId==record.treeId).remainingVolumeM3>0),"unselected stems remain deadwood");
  Check(m.DeadwoodRecords.Count>=historicalRecords,"one historical record remains per victim");
  var resolved=saves.CaptureData();Check(ForestSaveValidation.Validate(resolved,allTrees.Count,e.CellCount)==null,"resolved salvage schema valid");Check(saves.LoadData(resolved,false),"resolved salvage reload");yield return null;
  e.UseStormCalibrationForVerification(new StormCalibration(0,new[]{.02f,.06f,.18f},new[]{1f,1f,1f}));cash=m.CashCents;retained=m.RetainedTimberM3;Check(m.AdvanceYear(),"post-salvage quiet annual");yield return null;
  Check(m.CashCents==cash&&m.RetainedTimberM3==retained&&selected.All(id=>m.DeadwoodRecords.Single(record=>record.treeId==id).remainingVolumeM3==0),"future decay cannot recreate sold stems or repeat settlement");
  File.WriteAllText(Path.Combine(dir,"salvage_resolved.json"),JsonUtility.ToJson(resolved,true));
  Check(saves.LoadData(beforeDamage,false),"large-event cost comparison baseline");yield return null;e.StormModelVersion=1;e.UseStormCalibrationForVerification(new StormCalibration(0,new[]{.02f,.06f,.18f},new[]{1f,1f,1f}));e.ForceStormNextYear(.7f,135);Check(m.AdvanceYear(),"larger actual windthrow event");yield return null;
  var largeFallen=m.DeadwoodRecords.Where(record=>m.CanSalvage(record.treeId)).ToDictionary(record=>record.treeId,StringComparer.Ordinal);
  foreach(var record in largeFallen.Values)Check(m.TryDesignateSalvage(record.treeId,FellingMaterialOutcome.SellAndExtract),"large-event optional salvage selection");
  var largeOrders=m.WorkOrders.Where(order=>order.type==ScenarioWorkType.SalvageDeadwood&&order.status==ScenarioWorkStatus.Pending).ToArray();
  allTrees=FindObjectsByType<ForestTree>(FindObjectsInactive.Include,FindObjectsSortMode.None).ToDictionary(tree=>tree.TreeId,StringComparer.Ordinal);
  using(var table=new StreamWriter(Path.Combine(dir,"salvage_large_job_comparison.csv")))
  {
   table.WriteLine("basis_points,orders,volume_cm3,cost_cents,revenue_cents,minimum_adjustment_cents");long previous=0;bool aboveMinimum=false;
   foreach(int multiplier in new[]{10000,11500,12500,15000})
   {var candidate=ScenarioOneEconomyAdapter.QuoteHarvest(largeOrders,allTrees,m.Definition,22,0,m.CashCents,largeFallen,multiplier);Check(candidate.Eligible&&candidate.CostCents>=previous,"large-job candidate costs monotone");aboveMinimum|=candidate.CostCents>m.Definition.MinimumHarvestJobCents;previous=candidate.CostCents;table.WriteLine(string.Join(",",multiplier,candidate.Orders.Count,candidate.TotalStemVolumeCm3,candidate.CostCents,candidate.RevenueCents,candidate.Resolution.Quote.Costs.MinimumJobAdjustmentCents));}
   Check(!aboveMinimum,"authored stand comparison records the binding contractor minimum");
  }
  // A larger, dimensionally consistent stand exposes work costs above the minimum.
  var spawner=FindFirstObjectByType<ForestTreeSpawner>();
  foreach(var tree in FindObjectsByType<ForestTree>(FindObjectsInactive.Include,FindObjectsSortMode.None))DestroyImmediate(tree.gameObject);
  m.InitializeNewScenario();e.ResetForDeterministicRun();e.SimulationSeed=20260914;e.RngModelVersion=1;e.RegenerationModelVersion=2;e.GrowthModelVersion=0;e.StormModelVersion=1;
  const int count=1300;int side=Mathf.CeilToInt(Mathf.Sqrt(count));
  for(int i=0;i<count;i++){var root=new GameObject("Scaled salvage fixture "+i);root.transform.position=new Vector3((i%side+.5f)*40/side-20,0,(i/side+.5f)*40/side-20);var trunk=new GameObject("Trunk").transform;trunk.SetParent(root.transform);var canopy=new GameObject("Canopy").transform;canopy.SetParent(root.transform);root.AddComponent<ForestTree>().InitializeForSpawn("SALVAGECOST"+i.ToString("0000"),trunk,canopy,spawner.DefaultSpecies,40,25,30,1.2f);}
  e.RecomputeCanopy();e.RecomputeSeedRain();e.UseStormCalibrationForVerification(new StormCalibration(0,new[]{.02f,.06f,.18f},new[]{1f,1f,1f}));e.ForceStormNextYear(.7f,135);Check(m.AdvanceYear(),"scaled actual windthrow event");yield return null;
  largeFallen=m.DeadwoodRecords.Where(record=>m.CanSalvage(record.treeId)).ToDictionary(record=>record.treeId,StringComparer.Ordinal);
  Check(largeFallen.Count>112,"scaled cost fixture creates more eligible windthrow");
  foreach(var record in largeFallen.Values)Check(m.TryDesignateSalvage(record.treeId,FellingMaterialOutcome.SellAndExtract),"scaled optional selection");
  largeOrders=m.WorkOrders.Where(order=>order.type==ScenarioWorkType.SalvageDeadwood&&order.status==ScenarioWorkStatus.Pending).ToArray();allTrees=FindObjectsByType<ForestTree>(FindObjectsInactive.Include,FindObjectsSortMode.None).ToDictionary(tree=>tree.TreeId,StringComparer.Ordinal);
  using(var table=new StreamWriter(Path.Combine(dir,"salvage_scaled_job_comparison.csv")))
  {
   table.WriteLine("basis_points,orders,volume_cm3,cost_cents,revenue_cents,minimum_adjustment_cents");long previous=0,volume=-1,revenue=-1;
   foreach(int multiplier in new[]{10000,11500,12500,15000})
   {var candidate=ScenarioOneEconomyAdapter.QuoteHarvest(largeOrders,allTrees,m.Definition,2,0,100000000L,largeFallen,multiplier);Check(candidate.Eligible&&candidate.CostCents>previous&&candidate.CostCents>m.Definition.MinimumHarvestJobCents,"scaled costs reveal multiplier above minimum: cost="+candidate.CostCents+" orders="+candidate.Orders.Count+" problem="+candidate.Problem);Check(candidate.Resolution.Ledger.Count(row=>row.Category==LedgerCategory.MinimumJobAdjustment)<=1,"scaled minimum charged at most once");Check(candidate.TotalStemVolumeCm3==candidate.SoldVolumeCm3+candidate.RetainedVolumeCm3+candidate.DeadwoodVolumeCm3+candidate.ResidualVolumeCm3,"scaled volume conserved");Check(volume<0||(volume==candidate.TotalStemVolumeCm3&&revenue==candidate.RevenueCents),"cost premium changes neither volume nor timber prices");previous=candidate.CostCents;volume=candidate.TotalStemVolumeCm3;revenue=candidate.RevenueCents;table.WriteLine(string.Join(",",multiplier,candidate.Orders.Count,volume,previous,revenue,candidate.Resolution.Quote.Costs.MinimumJobAdjustmentCents));}
  }
  Check(saves.LoadData(initial,false),"restore salvage fixture");yield return null;
 }
}
#endif
