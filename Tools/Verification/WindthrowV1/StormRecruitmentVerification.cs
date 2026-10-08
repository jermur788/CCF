#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
public static class StormRecruitmentVerification
{
 public static void Begin(){EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");EditorApplication.isPlaying=true;}
 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]static void Install(){if(Environment.GetCommandLineArgs().Contains("StormRecruitmentVerification.Begin"))new GameObject("Disposable storm recruitment causal gate").AddComponent<StormRecruitmentVerificationRunner>();}
}
public sealed class StormRecruitmentVerificationRunner:MonoBehaviour
{
 const BindingFlags Flags=BindingFlags.NonPublic|BindingFlags.Instance;
 ForestEcologyController e;ScenarioOneManager m;ForestSaveController saves;ForestTreeSpawner spawner;int checks;
 void Check(bool condition,string reason){checks++;if(!condition)throw new Exception(reason);}
 void Call(object value,string method,params object[] args)=>value.GetType().GetMethod(method,Flags).Invoke(value,args);
 string F(double value)=>value.ToString("R",CultureInfo.InvariantCulture);
 IEnumerator Start(){yield return null;yield return null;Exception failed=null;var work=Run();while(true){bool more=false;object current=null;try{more=work.MoveNext();if(more)current=work.Current;}catch(Exception error){failed=error;Debug.LogError("STORM_RECRUITMENT_VERIFICATION_FAIL "+error);break;}if(!more)break;yield return current;}if(failed==null)Debug.Log("STORM_RECRUITMENT_VERIFICATION_PASS checks="+checks);EditorApplication.ExitPlaymode();EditorApplication.Exit(failed==null?0:1);}
 IEnumerator Run()
 {
  e=FindFirstObjectByType<ForestEcologyController>();m=FindFirstObjectByType<ScenarioOneManager>();saves=FindFirstObjectByType<ForestSaveController>();spawner=FindFirstObjectByType<ForestTreeSpawner>();var original=saves.CaptureData();var oldCalibration=m.CompetitionCalibration;float[] site=e.Cells.Select(c=>c.SiteProductivity).ToArray();
  string dir=Environment.GetEnvironmentVariable("CCF_STORM_OUTPUT");
  try
  {
   e.StormModelVersion=1;e.UseStormCalibrationForVerification(new StormCalibration(0,new[]{.02f,.06f,.18f},new[]{1f,1f,1f}));e.ForceStormNextYear(.00000001f,0);Check(m.AdvanceYear(),"zero-damage annual");yield return null;
   Check(m.StormEvents.Count==1&&e.LastStormPerformance.Victims==0&&e.LastStormPerformance.CanopyRebuilds==0&&e.LastStormPerformance.SeedRebuilds==0,"zero-victim event persisted with zero storm rebuilds");Check(saves.LoadData(saves.CaptureData(),false),"zero-victim event save/load");
   var worlds=new ForestSaveData[2];float[] lightControl=null;
   for(int storm=0;storm<2;storm++)
   {
    Check(saves.LoadData(original,false),"causal baseline reload");yield return null;
    e.StormModelVersion=1;e.UseStormCalibrationForVerification(new StormCalibration(0,new[]{.02f,.06f,.18f},new[]{1f,1f,1f}));
    if(storm==1)e.ForceStormNextYear(.7f,135);Check(m.AdvanceYear(),"causal annual storm/control");yield return null;worlds[storm]=saves.CaptureData();if(storm==0)lightControl=e.Cells.Select(c=>c.Light).ToArray();
   }
   Check(worlds[1].scenarioOne.stormEvents.Count==1,"resolved causal storm");
   Check(saves.LoadData(worlds[1],false),"select affected cell");yield return null;
   int selected=Enumerable.Range(0,e.CellCount).OrderByDescending(i=>e.Cells[i].Light-lightControl[i]).First();
   Check(e.Cells[selected].Light>lightControl[selected],"actual damage opens the selected cell");
   using(var table=new StreamWriter(Path.Combine(dir,"storm_recruitment_causality.csv")))
   {
    table.WriteLine("storm,competition_strength,species,cell,light,recent_opening,bramble,bracken,natural_starting,natural_vegetation_loss,natural_light_loss,natural_browse_loss,natural_remaining,exact_starting,exact_vegetation_deaths,exact_light_deaths,exact_browse_deaths,exact_remaining,near_tree_size_promoted,natural_promoted_bands,natural_exported_density");
    double[] cover=new double[2];int stormNaturalPromotions=0,stormExactPromotions=0;
    for(int storm=0;storm<2;storm++)foreach(float strength in new[]{0f,.35f})foreach(string id in new[]{"sitka-spruce","sessile-oak","beech"})
    {
     Check(saves.LoadData(worlds[storm],false),"reload physical causal world");yield return null;
     m.CompetitionCalibration=new UnderstoreyCompetitionCalibration{Strength=strength};m.RebuildCompetitionExposure();var cell=e.Cells[selected];float physicalLight=cell.Light;cover[storm]=m.CompetitionCellExposure(selected);
     foreach(var c in e.Cells)c.ClearRegeneration();var species=spawner.ResolveSpecies(id);var band=new ForestRegenerationCohort(species);band.Restore(.5f,.2f,0);cell.InsertBand(band);
     // Freeze growth only for the survival equivalence fixture, then restore the unsaved site driver.
     float originalSite=cell.SiteProductivity;cell.SiteProductivity=0;
     var juveniles=(List<PlantedJuvenile>)typeof(ScenarioOneManager).GetField("plantedJuveniles",Flags).GetValue(m);juveniles.Clear();
     const int count=256;for(int j=0;j<count;j++)juveniles.Add(new PlantedJuvenile{juvenileId="storm-pair-"+id+"-"+j,speciesId=id,position=new Vector3(cell.Center.x,0,cell.Center.y),heightMeters=.2f});
     e.Browsing.BackgroundPressure=.2f;typeof(ForestEcologyController).GetField("regenerationAccount",Flags).SetValue(e,new RegenerationAnnualAccount{Year=e.EcologicalYear,Model=2});
     Call(e,"GrowExistingRegeneration");Call(m,"AdvancePlantedJuveniles");cell.SiteProductivity=originalSite;
     var natural=e.LastRegenerationAccount.For(id);var exact=m.PlantedCompetitionAccount.For(id);float remaining=cell.Regeneration.Sum(c=>c.Density);
     Check(Mathf.Abs(.5f-natural.VegetationLoss-natural.LightLoss-natural.BrowseLoss-remaining)<.000001f,"natural cause ledger balances");Check(exact.Starting==exact.VegetationDeaths+exact.LightDeaths+exact.BrowseDeaths+exact.Remaining+exact.Promoted,"exact cause ledger balances");
     Check(strength!=0||(natural.VegetationLoss==0&&exact.VegetationDeaths==0),"strength-zero removes only vegetation losses");Check(strength==0||natural.VegetationLoss>0,"nonzero competition reduces survival under actual storm/control cover");
     double probability=remaining/.5;Check(Math.Abs(exact.Remaining-count*probability)<=5*Math.Sqrt(count*probability*(1-probability))+2,"natural expected and exact realised survival agree within binomial tolerance");
     int ve=exact.VegetationDeaths,li=exact.LightDeaths,br=exact.BrowseDeaths,re=exact.Remaining;
     // Demonstrate the existing promotion rule at actual post-event light, rather than inventing recruitment from damage.
     juveniles.Clear();for(int j=0;j<8;j++)juveniles.Add(new PlantedJuvenile{juvenileId="storm-promote-"+id+"-"+j,speciesId=id,position=new Vector3(cell.Center.x,0,cell.Center.y),heightMeters=species.PromotionHeightM+.01f});Call(m,"AdvancePlantedJuveniles");int promoted=m.PlantedCompetitionAccount.For(id).Promoted;
     Check(promoted<=8,"existing exact promotion remains bounded");
     foreach(var c in e.Cells)c.ClearRegeneration();var nearTreeSize=new ForestRegenerationCohort(species);nearTreeSize.Restore(.5f,species.PromotionHeightM+.01f,0);cell.InsertBand(nearTreeSize);
     Call(e,"PromoteCohorts",spawner.DefaultSpecies,SimulationRandom.Create(e.RngModelVersion,e.SimulationSeed,e.EcologicalYear,0));
     Check(natural.PromotedBands<=1&&natural.ExactTreesCreated==natural.PromotedBands,"Natural representation handoff remains bounded one band to one tree");
     Check(Mathf.Abs(.5f-cell.Regeneration.Sum(c=>c.Density)-natural.PromotionExported)<.000001f,"Natural promotion export balances");
     if(storm==1){stormNaturalPromotions+=natural.PromotedBands;stormExactPromotions+=promoted;}

     table.WriteLine(string.Join(",",storm,F(strength),id,selected,F(physicalLight),F(cell.RecentOpening),F(m.UnderstoreyCells[selected].brambleCover),F(m.UnderstoreyCells[selected].brackenCover),F(.5),F(natural.VegetationLoss),F(natural.LightLoss),F(natural.BrowseLoss),F(remaining),count,ve,li,br,re,promoted,natural.PromotedBands,F(natural.PromotionExported)));table.Flush();
    }
    Check(stormNaturalPromotions>0&&stormExactPromotions>0,"Actual storm-opened cells support existing natural and exact promotion rules");
    Check(cover[1]>cover[0],"actual storm light raises actual Model2 cover target and realised competitor exposure");
   }
  }
  finally{for(int i=0;i<site.Length;i++)e.Cells[i].SiteProductivity=site[i];m.CompetitionCalibration=oldCalibration;saves.LoadData(original,false);}
 }
}
#endif
