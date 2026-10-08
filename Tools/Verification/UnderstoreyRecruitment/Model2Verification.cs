#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Globalization;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
public static class Model2Verification
{
 public static void Begin(){EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");EditorApplication.isPlaying=true;}
 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]static void Install(){if(Environment.GetCommandLineArgs().Contains("Model2Verification.Begin"))new GameObject("Model2 verification").AddComponent<Model2VerificationRunner>();}
}
public sealed class Model2VerificationRunner:MonoBehaviour
{
 const BindingFlags Flags=BindingFlags.NonPublic|BindingFlags.Instance;
 ForestEcologyController e;ScenarioOneManager m;ForestSaveController saves;ForestTreeSpawner spawner;int checks;string output;
 void Check(bool ok,string reason){checks++;if(!ok)throw new Exception(reason);}
 void Call(object obj,string method,params object[] args)=>obj.GetType().GetMethod(method,Flags).Invoke(obj,args);
 string F(float value)=>value.ToString("R",CultureInfo.InvariantCulture);
 IEnumerator Start()
 {
  yield return null;yield return null;Exception error=null;
  try{e=FindFirstObjectByType<ForestEcologyController>();m=FindFirstObjectByType<ScenarioOneManager>();saves=FindFirstObjectByType<ForestSaveController>();spawner=FindFirstObjectByType<ForestTreeSpawner>();output=Environment.GetEnvironmentVariable("CCF_MODEL2_OUTPUT");Verify();Debug.Log("MODEL2VERIFICATION_PASS checks="+checks);}
  catch(Exception ex){error=ex;Debug.LogError("MODEL2VERIFICATION_FAIL "+ex);}
  EditorApplication.ExitPlaymode();EditorApplication.Exit(error==null?0:1);
 }
 ForestSaveData Clone(ForestSaveData d)=>JsonUtility.FromJson<ForestSaveData>(JsonUtility.ToJson(d));
 void Verify()
 {
  var original=saves.CaptureData();Check(original.version==19&&original.regenerationModel==2,"new default19/2");
  Check(ForestSaveValidation.Validate(original,336,e.CellCount)==null,"valid new state: "+ForestSaveValidation.Validate(original,336,e.CellCount));
  m.InitializeNewScenario();var immediatelySaved=saves.CaptureData();
  Check(immediatelySaved.scenarioOne.understoreyCells.Count==e.CellCount,"reset saved before first step has complete competitor grid");
  Check(ForestSaveValidation.Validate(immediatelySaved,336,e.CellCount)==null,"immediate reset save is valid");
  Check(saves.LoadData(immediatelySaved,false),"immediate reset save reloads");
  Check(saves.LoadData(original,false),"restore before remaining model2 cases");
  var model2LegacyGrowth=Clone(original);model2LegacyGrowth.growthModel=0;Check(ScenarioReferenceArchive.LegacyV16WorldHash(model2LegacyGrowth)==null&&ScenarioReferenceArchive.LegacyV17WorldHash(original)==null,"model2 never substituted by historical layout hash");
  var exponent=Clone(original);exponent.version=17;exponent.regenerationModel=1;
  foreach(var u in exponent.scenarioOne.understoreyCells){u.brambleCover=1e-10f;u.brackenCover=1e-11f;}
  string tinyJson=JsonUtility.ToJson(exponent);Check(tinyJson.Contains("E-")||tinyJson.Contains("e-"),"fixture emits negative scientific exponent");
  var zeros=Clone(exponent);foreach(var u in zeros.scenarioOne.understoreyCells)u.brambleCover=u.brackenCover=0;
  Check(ScenarioReferenceArchive.LegacyV17WorldHash(exponent)==ScenarioReferenceArchive.LegacyV17WorldHash(zeros),"historical hash ignores tiny appended cover values completely");
  var old=Clone(original);old.version=17;old.regenerationModel=1;Check(saves.LoadData(old,false)&&e.RegenerationModelVersion==1,"v17 stays1");
  foreach(var u in m.UnderstoreyCells)u.brambleCover=u.brackenCover=1;
  e.RecomputeCanopy();e.AdvanceOneYear();Check(e.LastRegenerationAccount.Species.Values.Sum(a=>a.VegetationLoss)==0,"model1 ignores competitor state");
  Check(saves.LoadData(original,false),"restore original2");
  string hash=ScenarioReferenceArchive.WorldHash(saves.CaptureData());
  foreach(float bad in new[]{-0.01f,1.01f,float.NaN,float.PositiveInfinity})
  {
   var data=Clone(original);data.scenarioOne.understoreyCells[0].brambleCover=bad;
   Check(!saves.LoadData(data,false),"rejectbadcover");Check(hash==ScenarioReferenceArchive.WorldHash(saves.CaptureData()),"bad load atomic");
  }
  var outside=Clone(original);outside.scenarioOne.clearancePatches.Add(new PlantingClearancePatch{center=new Vector3(1000,0,1000),radiusMeters=.8f});Check(!saves.LoadData(outside,false),"patch center outside grid rejected");Check(hash==ScenarioReferenceArchive.WorldHash(saves.CaptureData()),"outside patch rejection atomic");
  var unordered=Clone(original);unordered.scenarioOne.understoreyCells.Reverse();Check(!saves.LoadData(unordered,false),"unordered competitor grid rejected rather than reinitialized");Check(hash==ScenarioReferenceArchive.WorldHash(saves.CaptureData()),"unordered rejection atomic");
  var future=Clone(original);future.scenarioOne.understoreyCells[0].lastUpdatedYear=future.ecologicalYear+1;Check(!saves.LoadData(future,false),"rejectfuture cell");
  string raw=JsonUtility.ToJson(original);Check(ForestSaveValidation.ValidateCompetitionJson(raw,original)==null,"validraw");
  var token=Newtonsoft.Json.Linq.JObject.Parse(raw);((Newtonsoft.Json.Linq.JObject)token["scenarioOne"]["understoreyCells"][0]).Remove("brambleCover");
  Check(ForestSaveValidation.ValidateCompetitionJson(token.ToString(),original)!=null,"rejectmissingcover");
  var patch=new PlantingClearancePatch{center=new Vector3(e.Cells[0].Center.x,0,e.Cells[0].Center.y),radiusMeters=.8f,createdYear=0,competitionUpdatedYear=0,brambleCover=.4f,brackenCover=.2f};
  var local=Clone(original);local.scenarioOne.clearancePatches.Add(patch);
  Check(ForestSaveValidation.Validate(local,336,e.CellCount)==null,"validpatch");
  local.scenarioOne.clearancePatches[0].competitionUpdatedYear=local.ecologicalYear+1;Check(!saves.LoadData(local,false),"futurepatch rejected");
  patch.competitionUpdatedYear=0;
  local=Clone(original);local.scenarioOne.clearancePatches.Add(patch);local.scenarioOne.clearancePatches.Add(patch);Check(!saves.LoadData(local,false),"duplicategeometryyear rejected");
  local=Clone(original);local.scenarioOne.clearancePatches.Add(patch);Check(saves.LoadData(local,false),"patchroundtripload");
  var captured=saves.CaptureData();Check(captured.scenarioOne.clearancePatches[0].brambleCover==.4f&&captured.scenarioOne.clearancePatches[0].brackenCover==.2f,"patch coverspersist");
  CrossBorder(original);Overlap();Ledger();PairedSurvival();Recovery();SaveSize(original,captured);
  Check(saves.LoadData(original,false),"restore new save");
  e.Browsing.BackgroundPressure=.2f;
  string a=ScenarioReferenceArchive.WorldHash(saves.CaptureData());string legacyStart=ScenarioReferenceArchive.LegacyV18WorldHash(saves.CaptureData());e.AdvanceOneYear();Call(m,"AdvancePlantedJuveniles");string b=ScenarioReferenceArchive.WorldHash(saves.CaptureData());string legacyYear=ScenarioReferenceArchive.LegacyV18WorldHash(saves.CaptureData());
  Check(legacyStart=="FA855239CDDA32D8","historical model2 start v18 layout unchanged");Check(legacyYear=="02334804F65C0234","historical altered-site model2 fixture v18 layout unchanged");
  Debug.Log("MODEL2_V18_COMPAT_ANCHOR start="+legacyStart+" alteredSiteOneYear="+legacyYear);
  Check(saves.LoadData(original,false),"repeatrestore");e.AdvanceOneYear();Call(m,"AdvancePlantedJuveniles");Check(b==ScenarioReferenceArchive.WorldHash(saves.CaptureData()),"model2repeat anchor");
  Debug.Log("MODEL2_ANCHOR start="+a+" oneYear="+b);
  for(int year=2;year<=8;year++){e.AdvanceOneYear();Call(m,"AdvancePlantedJuveniles");}
  var checkpoint=saves.CaptureData();e.AdvanceOneYear();Call(m,"AdvancePlantedJuveniles");string continued=ScenarioReferenceArchive.WorldHash(saves.CaptureData());
  Check(saves.LoadData(checkpoint,false),"midrun reload");e.AdvanceOneYear();Call(m,"AdvancePlantedJuveniles");Check(continued==ScenarioReferenceArchive.WorldHash(saves.CaptureData()),"model2 midrun continuedstate");
 }
 void CrossBorder(ForestSaveData original)
 {
  var local=Clone(original);var c0=e.Cells[0].Center;var c1=e.Cells[1].Center;float border=(c0.x+c1.x)*.5f;
  local.scenarioOne.clearancePatches.Add(new PlantingClearancePatch{center=new Vector3(border,0,c0.y),radiusMeters=1,createdYear=0,competitionUpdatedYear=0,brambleCover=.8f,brackenCover=.7f});
  string raw=JsonUtility.ToJson(local);var token=Newtonsoft.Json.Linq.JObject.Parse(raw);((Newtonsoft.Json.Linq.JObject)token["scenarioOne"]["clearancePatches"][0]).Remove("competitionUpdatedYear");Check(ForestSaveValidation.ValidateCompetitionJson(token.ToString(),local)!=null,"missing patch year rejected");
  token=Newtonsoft.Json.Linq.JObject.Parse(raw);token["scenarioOne"]["clearancePatches"][0]["competitionUpdatedYear"]=long.MaxValue;Check(ForestSaveValidation.ValidateCompetitionJson(token.ToString(),local)!=null,"oversized year rejected");
  Check(saves.LoadData(local,false),"crossborder load");Check(m.TryDesignateVegetationClearance(0)&&m.ApprovePendingWork()&&m.AdvanceYear(),"paid cellclear");
  Vector3 inside=new Vector3(border-.2f,0,c0.y),outside=new Vector3(border+.2f,0,c0.y);
  Check(m.CompetitionExposureAt(inside)==0,"paid whole-cellclear masks local override inside");Check(m.CompetitionExposureAt(outside)>.2f,"crossborder patch preserved outside");
  var checkpoint=saves.CaptureData();float before=m.CompetitionExposureAt(outside);Check(saves.LoadData(checkpoint,false),"crossborder reload");Check(m.CompetitionExposureAt(inside)==0&&m.CompetitionExposureAt(outside)==before,"area treatment mask persists without extra state");
 }
 void Overlap()
 {
  int i=e.CellCount/2;var centre=e.Cells[i].Center;
  var state=m.UnderstoreyCells.ToList();state[i].brambleCover=.8f;state[i].brackenCover=.6f;
  var first=new PlantingClearancePatch{center=new Vector3(centre.x-.2f,0,centre.y),radiusMeters=1,createdYear=0,brambleCover=0,brackenCover=.2f};
  var recent=new PlantingClearancePatch{center=new Vector3(centre.x+.2f,0,centre.y),radiusMeters=1,createdYear=1,brambleCover=.4f,brackenCover=0};
  var cache=new CompetitionExposureCache(e,state,new[]{first,recent},new int[e.CellCount]);
  var reversed=new CompetitionExposureCache(e,state,new[]{recent,first},new int[e.CellCount]);
  Vector3 point=new Vector3(centre.x,0,centre.y);Check(cache.PointCovers(i,point).x==.4f,"latest wins");Check(cache.CellPressure[i]==reversed.CellPressure[i],"order independent exposure");
  // Independent dense 2D area oracle, separate from production strip sweep.
  double sum=0;int steps=500;float width=e.CellSizeMeters;
  for(int x=0;x<steps;x++)for(int y=0;y<steps;y++)
  {
   Vector2 p=centre+new Vector2((x+.5f)*width/steps-width*.5f,(y+.5f)*width/steps-width*.5f);
   float v=(p-new Vector2(recent.center.x,recent.center.z)).sqrMagnitude<=1?.4f:(p-new Vector2(first.center.x,first.center.z)).sqrMagnitude<=1?.2f:.8f;sum+=v;
  }
  Check(Math.Abs(cache.CellPressure[i]-sum/(steps*steps))<.001,"overlap counted once area oracle");
  recent.createdYear=0;cache=new CompetitionExposureCache(e,state,new[]{recent,first},new int[e.CellCount]);reversed=new CompetitionExposureCache(e,state,new[]{first,recent},new int[e.CellCount]);
  Check(cache.PointCovers(i,point)==reversed.PointCovers(i,point)&&cache.PointCovers(i,point).x==.4f,"stable geometry tie");
  // Independent exact-point oracle includes corners outside circles but
  // inside their boxes: the concentrated-history performance edge case.
  for(int x=0;x<31;x++)for(int y=0;y<31;y++)
  {
   Vector3 p=new Vector3(centre.x+(x-15)*.09f,0,centre.y+(y-15)*.09f);PlantingClearancePatch winner=null;
   foreach(var candidate in new[]{first,recent})if((new Vector2(p.x-candidate.center.x,p.z-candidate.center.z)).sqrMagnitude<=candidate.radiusMeters*candidate.radiusMeters&&(winner==null||CompetitionExposureCache.Compare(candidate,winner)>0))winner=candidate;
   Vector2 expected=winner==null?new Vector2(.8f,.6f):new Vector2(winner.brambleCover,winner.brackenCover);Check(cache.PointCovers(i,p)==expected,"exactpoint independent oracle");
  }
  var masked=new int[e.CellCount];masked[i]=2;cache=new CompetitionExposureCache(e,state,new[]{first,recent},masked);Check(cache.CellPressure[i]==.8f,"latercell clear masksold overrides once");
  File.WriteAllText(Path.Combine(output,"overlap.json"),"{\"stripExposure\":"+F(reversed.CellPressure[i])+",\"independentAreaOracle\":"+(sum/(steps*steps)).ToString("R",CultureInfo.InvariantCulture)+",\"tolerance\":0.001}");
 }
 void Ledger()
 {
  ((List<PlantingClearancePatch>)typeof(ScenarioOneManager).GetField("clearancePatches",Flags).GetValue(m)).Clear();
  using(var w=new StreamWriter(Path.Combine(output,"model2_cell_ledger.csv")))
  {
   w.WriteLine("species,cover,browse,protected,light,starting,vegetation_loss,light_loss,browse_loss,remaining,residual");
   foreach(string id in new[]{"sitka-spruce","sessile-oak","beech"})foreach(float cover in new[]{0f,.9f})foreach(float browse in new[]{0f,.8f})foreach(bool protect in new[]{false,true})foreach(float light in new[]{.08f,.4f,.8f})
   {
    foreach(var cell in e.Cells)cell.ClearRegeneration();var c=e.Cells[0];c.Light=light;var species=spawner.ResolveSpecies(id);var band=new ForestRegenerationCohort(species);band.Restore(.5f,.2f,0);c.InsertBand(band);
    foreach(var u in m.UnderstoreyCells)u.brambleCover=u.brackenCover=cover;
    e.Browsing.ClearProtection();e.Browsing.BackgroundPressure=browse;
    if(protect)e.Browsing.ProtectedAreas.Add(new BrowseProtectedArea{areaId="model2",installedYear=0,polygon=new List<Vector2>{c.Center+new Vector2(-5,-5),c.Center+new Vector2(5,-5),c.Center+new Vector2(5,5),c.Center+new Vector2(-5,5)}});
    typeof(ForestEcologyController).GetField("regenerationAccount",Flags).SetValue(e,new RegenerationAnnualAccount{Year=e.EcologicalYear,Model=2});m.RebuildCompetitionExposure();Call(e,"GrowExistingRegeneration");
    var a=e.LastRegenerationAccount.For(id);float remaining=c.Regeneration.Sum(b=>b.Density);float residual=.5f-a.VegetationLoss-a.LightLoss-a.BrowseLoss-remaining;
    Check(Mathf.Abs(residual)<.000001f,"naturalbalance");Check(cover>0||a.VegetationLoss==0,"zerocover");Check(!protect||a.BrowseLoss==0,"browseprotection");Check(cover==0||a.VegetationLoss>0,"vegetationindependent");
    w.WriteLine(string.Join(",",id,F(cover),F(browse),protect,F(light),F(.5f),F(a.VegetationLoss),F(a.LightLoss),F(a.BrowseLoss),F(remaining),F(residual)));
   }
  }
 }
 void PairedSurvival()
 {
  var juveniles=(List<PlantedJuvenile>)typeof(ScenarioOneManager).GetField("plantedJuveniles",Flags).GetValue(m);
  using(var w=new StreamWriter(Path.Combine(output,"model2_paired_survival.csv")))
  {
   w.WriteLine("species,cover,browse,protected,natural_expected_survival,exact_starting,exact_survivors,vegetation_deaths,light_deaths,browse_deaths,tolerance_individuals");
   foreach(string id in new[]{"sitka-spruce","sessile-oak","beech"})foreach(float cover in new[]{0f,.9f})foreach(float browse in new[]{0f,.8f})foreach(bool protect in new[]{false,true})
   {
    foreach(var cell in e.Cells)cell.ClearRegeneration();var c=e.Cells[0];c.Light=.4f;c.SiteProductivity=0;var band=new ForestRegenerationCohort(spawner.ResolveSpecies(id));band.Restore(.5f,.2f,0);c.InsertBand(band);
    foreach(var u in m.UnderstoreyCells)u.brambleCover=u.brackenCover=cover;
    e.Browsing.ClearProtection();e.Browsing.BackgroundPressure=browse;
    if(protect)e.Browsing.ProtectedAreas.Add(new BrowseProtectedArea{areaId="paired",installedYear=0,polygon=new List<Vector2>{c.Center+new Vector2(-5,-5),c.Center+new Vector2(5,-5),c.Center+new Vector2(5,5),c.Center+new Vector2(-5,5)}});
    juveniles.Clear();const int count=256;for(int j=0;j<count;j++)juveniles.Add(new PlantedJuvenile{juvenileId="paired-"+id+"-"+j,speciesId=id,position=new Vector3(c.Center.x,0,c.Center.y),heightMeters=.2f});
    typeof(ForestEcologyController).GetField("regenerationAccount",Flags).SetValue(e,new RegenerationAnnualAccount{Year=e.EcologicalYear,Model=2});m.RebuildCompetitionExposure();Call(e,"GrowExistingRegeneration");Call(m,"AdvancePlantedJuveniles");
    double expected=c.Regeneration.Sum(b=>b.Density)/.5;var a=m.PlantedCompetitionAccount.For(id);double tolerance=5*Math.Sqrt(count*expected*(1-expected))+2;
    Check(Math.Abs(a.Remaining-count*expected)<=tolerance,"paired natural/exact survival");Check(a.Starting==count&&a.Starting==a.VegetationDeaths+a.LightDeaths+a.BrowseDeaths+a.Remaining+a.Promoted,"paired exact ledger");Check(!protect||a.BrowseDeaths==0,"exactprotection");Check(cover>0||a.VegetationDeaths==0,"exactzerocover");
    w.WriteLine(string.Join(",",id,F(cover),F(browse),protect,expected.ToString("R",CultureInfo.InvariantCulture),count,a.Remaining,a.VegetationDeaths,a.LightDeaths,a.BrowseDeaths,tolerance.ToString("R",CultureInfo.InvariantCulture)));
   }
  }
 }
 void Recovery()
 {
  using(var w=new StreamWriter(Path.Combine(output,"model2_recovery.csv")))
  {
   w.WriteLine("light,gain,loss,year,bramble,bracken,target");
   foreach(float light in new[]{.08f,.4f,.8f})foreach(float gain in new[]{.15f,.3f,.45f})foreach(float loss in new[]{.25f,.45f,.65f})
   {
    var c=new UnderstoreyCompetitionCalibration{Gain=gain,Loss=loss};var cell=new ForestEcologyCell{Light=light,SiteProductivity=1,SoilStability=1};float target=UnderstoreyCompetition.Target(cell,c),b=0,k=1;
    for(int year=1;year<=20;year++){b=UnderstoreyCompetition.Step(b,target,c);k=UnderstoreyCompetition.Step(k,target,c);if(new[]{1,3,5,10,20}.Contains(year))w.WriteLine(string.Join(",",F(light),F(gain),F(loss),year,F(b),F(k),F(target)));}
   }
  }
 }
 void SaveSize(ForestSaveData original,ForestSaveData patch)
 {
  string json=JsonUtility.ToJson(patch);string old=System.Text.RegularExpressions.Regex.Replace(json,@",""(?:brambleCover|brackenCover|competitionUpdatedYear)"":-?[0-9.Ee+]+","");
  int full=System.Text.Encoding.UTF8.GetByteCount(json),before=System.Text.Encoding.UTF8.GetByteCount(old);
  File.WriteAllText(Path.Combine(output,"save_size.json"),"{\"actualModel2Bytes\":"+full+",\"sameWorldWithoutFiveFieldsBytes\":"+before+",\"schemaBytes\":"+(full-before)+",\"cells\":"+patch.scenarioOne.understoreyCells.Count+",\"patches\":"+patch.scenarioOne.clearancePatches.Count+"}");
  Check(full>before,"measuredschemaBytes");
  string pretty=JsonUtility.ToJson(patch,true);string prettyBefore=System.Text.RegularExpressions.Regex.Replace(pretty,@",\s*""(?:brambleCover|brackenCover|competitionUpdatedYear)""\s*:\s*-?[0-9.Ee+]+","");
  File.WriteAllText(Path.Combine(output,"save18_sample.json"),pretty);File.WriteAllText(Path.Combine(output,"schema_comparator_sample.json"),prettyBefore);
  long diskBytes=new FileInfo(Path.Combine(output,"save18_sample.json")).Length,oldDiskBytes=new FileInfo(Path.Combine(output,"schema_comparator_sample.json")).Length;
  var parsed=JsonUtility.FromJson<ForestSaveData>(prettyBefore);Check(parsed.trees.Count==patch.trees.Count,"pretty comparator preserves world");
  File.WriteAllText(Path.Combine(output,"save_disk_size.json"),"{\"actualSaveFormatBytes\":"+diskBytes+",\"sameWorldWithoutFiveFieldsBytes\":"+oldDiskBytes+",\"schemaBytes\":"+(diskBytes-oldDiskBytes)+",\"cells\":"+patch.scenarioOne.understoreyCells.Count+",\"patches\":"+patch.scenarioOne.clearancePatches.Count+",\"format\":\"JsonUtility pretty=true, actual UTF8 files\"}");
  Check(diskBytes>oldDiskBytes,"actual disk schema overhead");
 }
}
#endif
