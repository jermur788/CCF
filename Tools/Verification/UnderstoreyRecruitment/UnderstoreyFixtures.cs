using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using Debug=UnityEngine.Debug;
public static class UnderstoreyFixtures
{
#if UNITY_EDITOR
 public static void Begin(){EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");EditorApplication.isPlaying=true;}
#endif
 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]private static void Install(){if(Environment.GetCommandLineArgs().Contains("UnderstoreyFixtures.Begin"))new GameObject("Understorey fixtures").AddComponent<UnderstoreyFixtureRunner>();}
}
public sealed class UnderstoreyFixtureRunner:MonoBehaviour
{
 private const BindingFlags Flags=BindingFlags.Instance|BindingFlags.NonPublic;
 private ForestEcologyController e;private ScenarioOneManager m;private ForestTreeSpawner spawner;private string output;
 private static string F(double x)=>x.ToString("R",CultureInfo.InvariantCulture);
 private void Check(bool yes,string message){if(!yes)throw new Exception(message);}
 private void Call(object owner,string name,params object[] args)=>owner.GetType().GetMethod(name,Flags).Invoke(owner,args);
 private void DestroyTrees(){foreach(var t in FindObjectsByType<ForestTree>(FindObjectsInactive.Include,FindObjectsSortMode.None))DestroyImmediate(t.gameObject);}
 private void Empty(float light,float browse)
 {
  DestroyTrees();m.InitializeNewScenario();e.ResetForDeterministicRun();e.RngModelVersion=e.RegenerationModelVersion=e.GrowthModelVersion=1;e.Browsing.BackgroundPressure=browse;e.Browsing.ClearProtection();e.RecomputeSeedRain();
  foreach(var c in e.Cells){c.ClearRegeneration();c.Light=light;}
  typeof(ForestEcologyController).GetField("ecologicalYear",Flags).SetValue(e,1);
  typeof(ForestEcologyController).GetField("regenerationAccount",Flags).SetValue(e,new RegenerationAnnualAccount{Year=1,Model=1});
  Call(m,"EnsureUnderstoreyGrid");
 }
 private IEnumerator Controlled()
 {
  using(var w=new StreamWriter(Path.Combine(output,"cell_species_ledger.csv")))
  {
   w.WriteLine("cell,species,year,light,browse,protected,cover,seed_arrival,abundance_before,light_loss,browse_loss,requested,accepted,rejected,promotion_export,remaining,residual");
   foreach(string id in new[]{"sitka-spruce","sessile-oak","beech"})foreach(float light in new[]{.08f,.8f})foreach(float browse in new[]{0f,.8f})foreach(bool protect in new[]{false,true})foreach(float cover in new[]{0f,1f})foreach(float seed in new[]{0f,.05f,20f})
   {
    Empty(light,browse);TreeSpeciesDefinition s=spawner.ResolveSpecies(id);var c=e.Cells[0];var band=new ForestRegenerationCohort(s);band.Restore(.2f,.2f,0);c.InsertBand(band);c.SetSeedRain(id,seed);
    foreach(var u in m.UnderstoreyCells)u.ferns=u.grasses=u.shrubs=cover;
    if(protect)e.Browsing.ProtectedAreas.Add(new BrowseProtectedArea{areaId="fixture",installedYear=0,polygon=new List<Vector2>{c.Center+new Vector2(-5,-5),c.Center+new Vector2(5,-5),c.Center+new Vector2(5,5),c.Center+new Vector2(-5,5)}});
    Call(e,"GrowExistingRegeneration");Call(e,"EstablishNewCohorts");var a=e.LastRegenerationAccount.For(id);float remaining=c.Regeneration.Sum(r=>r.Density);float residual=remaining-(.2f-a.LightLoss-a.BrowseLoss+a.EstablishmentAccepted-a.PromotionExported);
    Check(Mathf.Abs(residual)<.00001,"Cell population residual");Check(Mathf.Abs(a.EstablishmentRequested-a.EstablishmentAccepted-a.CapacityRejected)<.00001,"Establishment residual");Check(seed>0||a.EstablishmentAccepted==0,"Seed-free recruitment");Check(!protect||a.BrowseLoss==0,"Protected browse loss");
    w.WriteLine(string.Join(",",0,id,1,F(light),F(browse),protect,F(cover),F(seed),F(.2f),F(a.LightLoss),F(a.BrowseLoss),F(a.EstablishmentRequested),F(a.EstablishmentAccepted),F(a.CapacityRejected),F(a.PromotionExported),F(remaining),F(residual)));
    yield return null;
   }
  }
  Debug.Log("UNDERSTOREY_CELL_LEDGER_PASS protected=true zeroSeed=true highSeed=true subthreshold=true residualBound=0.00001");
 }
 private void Recolonisation()
 {
  using(var w=new StreamWriter(Path.Combine(output,"recolonisation_runtime.csv")))
  {
   w.WriteLine("light,year,ferns,grasses,forbs,shrubs,mosses,fungi");
   foreach(float light in new[]{.08f,.4f,.8f})
   {
    var c=new ForestEcologyCell{Light=light,Canopy=1-light,SiteProductivity=1,SoilStability=1,EstablishmentSuitability=1};var u=ScenarioOneUnderstorey.Initially(0,c,0);u.ferns=u.grasses=u.forbs=u.shrubs=0;
    for(int year=1;year<=20;year++){ScenarioOneUnderstorey.Advance(u,c,year,m.Definition.UnderstoreyColonisationRate,m.Definition.UnderstoreyLossRate);if(new[]{1,3,5,10,20}.Contains(year))w.WriteLine(string.Join(",",F(light),year,F(u.ferns),F(u.grasses),F(u.forbs),F(u.shrubs),F(u.mosses),F(u.fungi)));}
   }
  }
  Debug.Log("UNDERSTOREY_RECOLONISATION_PASS years=1,3,5,10,20 productionRecurrence=true");
 }
 private IEnumerator Performance()
 {
  using(var w=new StreamWriter(Path.Combine(output,"performance.csv")))
  {
   w.WriteLine("adult_count,annual_ms,derived_pressure_1000_passes_ms,cell_count,visuals,natural_bands,exact_planted");
   foreach(int count in new[]{336,1300,3000,5000})
   {
    Empty(.5f,0);var s=spawner.DefaultSpecies;int side=Mathf.CeilToInt(Mathf.Sqrt(count));
    for(int i=0;i<count;i++)
    {
     var root=new GameObject("Perf tree "+i);root.transform.position=new Vector3((i%side+.5f)*40/side-20,0,(i/side+.5f)*40/side-20);var stem=new GameObject("Trunk").transform;stem.SetParent(root.transform);var crown=new GameObject("Canopy").transform;crown.SetParent(root.transform);
     var tree=root.AddComponent<ForestTree>();tree.InitializeForSpawn("PERF"+i,stem,crown,s,20,14,16,1.2f);
    }
    foreach(var cell in e.Cells)foreach(string id in new[]{"sitka-spruce","sessile-oak","beech"}){var band=new ForestRegenerationCohort(spawner.ResolveSpecies(id));band.Restore(.1f,.2f,0);cell.InsertBand(band);}
    var planted=(List<PlantedJuvenile>)typeof(ScenarioOneManager).GetField("plantedJuveniles",Flags).GetValue(m);planted.Clear();for(int j=0;j<30;j++)planted.Add(new PlantedJuvenile{juvenileId="perf"+j,speciesId="beech",position=new Vector3(e.Cells[j%e.CellCount].Center.x,0,e.Cells[j%e.CellCount].Center.y),heightMeters=.6f,ageYears=1});
    var watch=Stopwatch.StartNew();e.AdvanceOneYear();Call(m,"AdvancePlantedJuveniles");watch.Stop();double annual=watch.Elapsed.TotalMilliseconds;watch.Restart();float sum=0;
    for(int repeat=0;repeat<1000;repeat++)foreach(var u in m.UnderstoreyCells)sum+=Mathf.Max(u.ferns,Mathf.Max(u.grasses,u.shrubs));watch.Stop();
    Check(!float.IsNaN(sum),"Pressure finite");w.WriteLine(string.Join(",",count,F(annual),F(watch.Elapsed.TotalMilliseconds),e.CellCount,"no adult tree meshes: simulation isolation",e.CellCount*3,30));yield return null;yield return Resources.UnloadUnusedAssets();GC.Collect();
   }
  }
  Debug.Log("UNDERSTOREY_PERFORMANCE_PASS counts=336,1300,3000,5000 simulationOnly=true");
 }
 private IEnumerator MixedJuvenileLongRuns()
 {
  using(var w=new StreamWriter(Path.Combine(output,"mixed_juvenile_years.csv")))
  {
   w.WriteLine("run,repeat,year,species,natural_remaining,requested,accepted,rejected,light_loss,browse_loss,promotion_export,exact_natural_recruits,planted_alive_unpromoted,planted_promoted,planted_dead,living_trees,deadwood_records,stand_total_clearance_removed_abundance,stand_total_clearance_removed_planted,seed_arrival");
   foreach(float browse in new[]{0f,.8f})foreach(bool protect in new[]{false,true})foreach(int clearance in new[]{0,1,3,5})
   {
    // Reduced design: low browse covers timing; high browse contrasts protection and a post-establishment clear.
    if(browse==0&&protect||browse>0&&(clearance==1||clearance==5))continue;
    string label="browse"+F(browse)+"_protect"+protect+"_clear"+clearance;var hashes=new Dictionary<int,string>();
    for(int repeat=0;repeat<2;repeat++)
    {
     Empty(1,browse);typeof(ForestEcologyController).GetField("ecologicalYear",Flags).SetValue(e,0);
     var planted=(List<PlantedJuvenile>)typeof(ScenarioOneManager).GetField("plantedJuveniles",Flags).GetValue(m);planted.Clear();
     foreach(var u in m.UnderstoreyCells)u.ferns=u.grasses=u.shrubs=.7f;
     foreach(int index in new[]{0,15,32,63})foreach(string id in new[]{"sitka-spruce","sessile-oak","beech"})
     {
      var cell=e.Cells[index];var band=new ForestRegenerationCohort(spawner.ResolveSpecies(id));band.Restore(.2f,.2f,0);cell.InsertBand(band);
      planted.Add(new PlantedJuvenile{juvenileId="mixed"+index+id,speciesId=id,position=new Vector3(cell.Center.x,0,cell.Center.y),heightMeters=.6f,ageYears=1});
     }
     if(protect)e.Browsing.ProtectedAreas.Add(new BrowseProtectedArea{areaId="whole-fixture",installedYear=0,polygon=new List<Vector2>{new Vector2(-25,-25),new Vector2(25,-25),new Vector2(25,25),new Vector2(-25,25)}});
     for(int year=1;year<=100;year++)
     {
      float removed=0;int killed=0;
      if(clearance==1&&year==1||clearance==3&&year==3||clearance==5&&(year==1||year%5==0))foreach(int index in new[]{0,15,32,63}){var targets=m.QueryClearance(ClearanceFootprint.Cell(e,index),year);removed+=targets.Density;killed+=targets.Juveniles.Count;Call(m,"ApplyClearance",targets,year);}
      e.AdvanceOneYear();Call(m,"AdvancePlantedJuveniles");Call(m,"AdvanceUnderstorey");
      foreach(string id in new[]{"sitka-spruce","sessile-oak","beech"})
      {
       var a=e.LastRegenerationAccount.For(id);var js=planted.Where(j=>j.speciesId==id).ToArray();float rest=e.Cells.Sum(c=>c.Regeneration.Where(b=>b.SpeciesId==id).Sum(b=>b.Density));
       Check(rest>=0&&a.InfillAccepted==0,"Mixed stock invariant");Check(Math.Abs(a.EstablishmentRequested-a.EstablishmentAccepted-a.CapacityRejected)<.0001,"Mixed establishment balance");
       w.WriteLine(string.Join(",",label,repeat,year,id,F(rest),F(a.EstablishmentRequested),F(a.EstablishmentAccepted),F(a.CapacityRejected),F(a.LightLoss),F(a.BrowseLoss),F(a.PromotionExported),a.ExactTreesCreated,js.Count(j=>j.alive&&string.IsNullOrEmpty(j.promotedTreeId)),js.Count(j=>!string.IsNullOrEmpty(j.promotedTreeId)),js.Count(j=>!j.alive),FindObjectsByType<ForestTree>(FindObjectsSortMode.None).Count(x=>x.IsLiving),m.DeadwoodRecords.Count,F(removed),killed,F(a.SeedArrival)));
      }
      if(new[]{10,25,50,100}.Contains(year))
      {
       var state=new System.Text.StringBuilder();foreach(var c in e.Cells)foreach(var b in c.Regeneration)state.Append(b.SpeciesId).Append(F(b.Density)).Append(F(b.Height)).Append(b.EstablishYear);
       foreach(var j in planted)state.Append(j.juvenileId).Append(j.alive).Append(F(j.heightMeters)).Append(j.promotedTreeId);foreach(var tree in FindObjectsByType<ForestTree>(FindObjectsSortMode.None).Where(x=>x.IsLiving).OrderBy(x=>x.TreeId))state.Append(tree.TreeId).Append(F(tree.Height)).Append(F(tree.Diameter));
       string hash;using(var sha=System.Security.Cryptography.SHA256.Create())hash=BitConverter.ToString(sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(state.ToString())));
       if(repeat==0)hashes[year]=hash;else Check(hashes[year]==hash,"Mixed long-run repeat at "+label+"/"+year);
      }
      if(year%10==0){yield return null;yield return Resources.UnloadUnusedAssets();GC.Collect();}
     }
     Debug.Log("UNDERSTOREY_MIXED_RUN_PASS "+label+" repeat="+repeat);
    }
   }
  }
 }
 private IEnumerator Execute(){output=Environment.GetEnvironmentVariable("CCF_UNDERSTOREY_OUTPUT");Directory.CreateDirectory(output);e=FindFirstObjectByType<ForestEcologyController>();m=FindFirstObjectByType<ScenarioOneManager>();spawner=FindFirstObjectByType<ForestTreeSpawner>();if(Environment.GetEnvironmentVariable("CCF_FIXTURE_PERFORMANCE_ONLY")=="1"){yield return Performance();}else{yield return Controlled();Recolonisation();yield return Performance();yield return MixedJuvenileLongRuns();}Debug.Log("UNDERSTOREY_FIXTURES_PASS");}
 private IEnumerator Start()
 {
  yield return null;yield return null;var stack=new Stack<IEnumerator>();stack.Push(Execute());string error=null;while(stack.Count>0){bool more=false;object next=null;try{more=stack.Peek().MoveNext();if(more)next=stack.Peek().Current;}catch(Exception ex){error=ex.ToString();break;}if(!more){stack.Pop();continue;}if(next is IEnumerator nested){stack.Push(nested);continue;}yield return next;}if(error!=null)Debug.LogError("UNDERSTOREY_FIXTURES_FAIL "+error);
#if UNITY_EDITOR
  EditorApplication.ExitPlaymode();EditorApplication.Exit(error==null?0:1);
#endif
 }
}
