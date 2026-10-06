using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
public static class CCFBeechVerification {
 public static void Begin() { EditorSceneManager.OpenScene("Assets/Scenes/MixedSpeciesTest.unity"); EditorApplication.isPlaying=true; }
 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)] static void Install(){new GameObject("Beech verification").AddComponent<CCFBeechRunner>();}
}
public class CCFBeechRunner:MonoBehaviour {
 string path; byte[] backup; bool existed; TreeSpeciesDefinition beech; bool enabledBefore;
 ForestEcologyController ecology; ForestSaveController saves;
 void Check(bool ok,string message){if(!ok)throw new Exception(message);}
 string Persisted(){saves.Save();var d=JsonUtility.FromJson<ForestSaveData>(File.ReadAllText(path));d.trees=d.trees.OrderBy(t=>t.treeId,StringComparer.Ordinal).ToList();d.cells=d.cells.OrderBy(c=>c.index).ToList();foreach(var c in d.cells)c.cohorts=c.cohorts.OrderBy(x=>x.speciesId,StringComparer.Ordinal).ToList();return JsonUtility.ToJson(d);}
 string Rain(){return string.Join("|",ecology.Cells.SelectMany((c,i)=>c.Regeneration.Where(x=>x.SeedRain>0).Select(x=>i+":"+x.SpeciesId+":"+x.SeedRain.ToString("R",System.Globalization.CultureInfo.InvariantCulture))));}
 IEnumerator Test(){
 yield return null;
 ecology=FindFirstObjectByType<ForestEcologyController>();saves=FindFirstObjectByType<ForestSaveController>();
 // Legacy single-cohort contract: regeneration model 0 unless CCF_REGEN_MODEL overrides it (diagnostics).
 string forced=Environment.GetEnvironmentVariable("CCF_REGEN_MODEL");
 ecology.RegenerationModelVersion=string.IsNullOrEmpty(forced)?RegenerationModel.Legacy:int.Parse(forced);
 Debug.Log("BEECH_REGEN_MODEL "+ecology.RegenerationModelVersion);
 beech=FindFirstObjectByType<ForestTreeSpawner>().ResolveSpecies("beech");Check(beech!=null,"Beech absent");enabledBefore=beech.SupportsRegeneration;
 typeof(TreeSpeciesDefinition).GetField("supportsRegeneration",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(beech,true);
 string initial=Persisted(); bool shared=false;
 double tSeed=0,tReq=0,tAcc=0,tRej=0,tInf=0,tThr=0,tLight=0,tBrowse=0,tSub=0,maxReq=0; int tBands=0,tTrees=0,estYears=0,tCross=0;
 for(int i=0;i<100;i++){ecology.AdvanceOneYear();shared |= ecology.Cells.Any(c=>c.Regeneration.Count(x=>x.Density>0)>1);Check(ecology.Cells.All(c=>c.SharedOccupancy<=1.000001f),"capacity exceeded");
  var a=ecology.LastRegenerationAccount.For("beech");
  tSeed+=a.SeedArrival;tBrowse+=a.BrowseLoss;tSub+=a.SubThresholdRecruitment;tCross+=a.ThresholdCrossings;tReq+=a.EstablishmentRequested;tAcc+=a.EstablishmentAccepted;tRej+=a.CapacityRejected;tInf+=a.InfillAccepted;tThr+=a.ThresholdExtinction;tLight+=a.LightLoss;tBands+=a.BandsCreated;tTrees+=a.ExactTreesCreated;
  if(a.EstablishmentRequested>0){estYears++;maxReq=Math.Max(maxReq,a.EstablishmentRequested);}var bb=ecology.Cells.SelectMany(c=>c.Regeneration.Where(x=>x.SpeciesId=="beech"&&x.Density>0)).ToList();
  if(i%10==9)Debug.Log($"BEECH_DIAG year={ecology.EcologicalYear} seed={a.SeedArrival:0.###} req={a.EstablishmentRequested:0.###} acc={a.EstablishmentAccepted:0.###} rej={a.CapacityRejected:0.###} infill={a.InfillAccepted:0.###} light={a.LightLoss:0.###} thr={a.ThresholdExtinction:0.###} promoted={a.ExactTreesCreated} bands={bb.Count} abundance={bb.Sum(x=>x.Density):0.###} maxH={(bb.Count>0?bb.Max(x=>x.Height):0):0.##} cellsLight>={(bb.Count>0?ecology.Cells.Where(c=>c.SpeciesDensity("beech")>0).Max(c=>c.Light):0):0.###}");}
 var beechNow=ecology.Cells.SelectMany(c=>c.Regeneration.Where(x=>x.SpeciesId=="beech"&&x.Density>0)).ToList();
 Debug.Log($"BEECH_TOTALS model={ecology.RegenerationModelVersion} seedArrival={tSeed:0.###} establishmentYears={estYears} requested={tReq:0.####} maxAnnualRequest={maxReq:0.####} accepted={tAcc:0.####} rejected={tRej:0.####} legacyInfill={tInf:0.####} lightLoss={tLight:0.####} browseLoss={tBrowse:0.####} thresholdExtinction={tThr:0.####} subThresholdRecruitment={tSub:0.####} thresholdCrossings={tCross} bandsCreated={tBands} exactTrees={tTrees} remainingBeech={beechNow.Sum(x=>x.Density):0.####} remainingRecords={beechNow.Count} accumulatorAbundance={beechNow.Where(x=>x.Density<RegenerationModel.RepresentationThreshold).Sum(x=>x.Density):0.#####}");
 string before=Persisted();string annualRain=Rain();
 Check(shared,"no shared cell during run");
 var trees=FindObjectsByType<ForestTree>(FindObjectsSortMode.None);Check(trees.Select(t=>t.TreeId).Distinct().Count()==trees.Length,"duplicate IDs");
 Check(trees.Any(t=>t.TreeId.StartsWith("R")&&t.Species==beech),"no Beech recruit");
 Debug.Log("BEECH_MIXED_COUNTS "+string.Join(" ",trees.GroupBy(t=>t.Species.SpeciesId).Select(g=>g.Key+"="+g.Count())));
 ecology.AdvanceOneYear();string uninterrupted=Persisted();string uninterruptedRain=Rain();
 File.WriteAllText(path,before);saves.Load();yield return null;yield return null;
 string restored=Persisted();Check(before==restored,"persisted state mismatch after load");
 ecology.RecomputeCanopy();ecology.RecomputeSeedRain();string rain=Rain();ecology.RecomputeSeedRain();Check(rain==Rain(),"derived rain is not repeatable");
 Debug.Log("BEECH_PERSISTED_PASS annualDerivedRainChanged="+(annualRain!=rain));
 ecology.AdvanceOneYear();Check(uninterrupted==Persisted(),"next-year persisted state differs from uninterrupted run");Check(uninterruptedRain==Rain(),"next-year seed rain differs from uninterrupted run");Debug.Log("BEECH_CONTINUATION_PASS");
 File.WriteAllText(path,initial);saves.Load();yield return null;yield return null;
 for(int i=0;i<100;i++)ecology.AdvanceOneYear();Check(before==Persisted(),"mixed A/B persisted determinism failed");Check(annualRain==Rain(),"mixed A/B seed rain determinism failed");Debug.Log("BEECH_AB_PASS");
 }
 IEnumerator Start(){path=Path.Combine(Application.persistentDataPath,"forest-save.json");existed=File.Exists(path);if(existed)backup=File.ReadAllBytes(path);Exception failure=null;var test=Test();while(true){bool more=false;object current=null;try{more=test.MoveNext();if(more)current=test.Current;}catch(Exception ex){failure=ex;}if(failure!=null||!more)break;yield return current;}
 if(beech!=null)typeof(TreeSpeciesDefinition).GetField("supportsRegeneration",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(beech,enabledBefore);
 if(existed)File.WriteAllBytes(path,backup);else if(File.Exists(path))File.Delete(path);
 if(failure==null)Debug.Log("BEECH_VERIFY_PASS");else Debug.LogError("BEECH_VERIFY_FAIL "+failure);
 EditorApplication.ExitPlaymode();EditorApplication.Exit(failure==null?0:1);}
}
