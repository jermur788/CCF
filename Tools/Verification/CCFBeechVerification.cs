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
 beech=FindFirstObjectByType<ForestTreeSpawner>().ResolveSpecies("beech");Check(beech!=null,"Beech absent");enabledBefore=beech.SupportsRegeneration;
 typeof(TreeSpeciesDefinition).GetField("supportsRegeneration",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(beech,true);
 string initial=Persisted(); bool shared=false;
 for(int i=0;i<100;i++){ecology.AdvanceOneYear();shared |= ecology.Cells.Any(c=>c.Regeneration.Count(x=>x.Density>0)>1);Check(ecology.Cells.All(c=>c.SharedOccupancy<=1.000001f),"capacity exceeded");}
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
