using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using Debug = UnityEngine.Debug;

// Disposable, explicitly invoked measurement. No disk saves, asset edits or production changes.
public static class UnderstoreyDiagnostics
{
    public static string Output => Environment.GetEnvironmentVariable("CCF_UNDERSTOREY_OUTPUT");
#if UNITY_EDITOR
    public static void Begin()
    {
        Directory.CreateDirectory(Output);
        if(Environment.GetEnvironmentVariable("CCF_SKIP_INVENTORY") != "1") Inventory();
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        EditorApplication.isPlaying = true;
    }
    private static string Q(object value) => "\"" + (value?.ToString() ?? "").Replace("\"", "\"\"") + "\"";
    private static void Inventory()
    {
        var paths = AssetDatabase.GetAllAssetPaths().Where(p => p.StartsWith("Assets/") && !Directory.Exists(p)).OrderBy(p=>p,StringComparer.Ordinal).ToArray();
        var scenes = new HashSet<string>(AssetDatabase.GetDependencies(new[]{"Assets/Scenes/ForestTest.unity"}, true));
        var prefabs = new HashSet<string>();
        foreach (string p in paths.Where(p=>p.EndsWith(".prefab"))) foreach(string d in AssetDatabase.GetDependencies(p,true)) prefabs.Add(d);
        using(var w=new StreamWriter(Path.Combine(Output,"AssetInventory.csv")))
        {
            w.WriteLine("path,type,name,scene_dependency,prefab_dependency,triangles_distinct_meshes,vertices_distinct_meshes,material_slots,textures_dependencies,texture_width,texture_height,lod_groups,lod_levels,colliders,local_scale,bounds,shader,source_license_status,notes");
            int n=0;
            foreach(string p in paths)
            {
                Type type=AssetDatabase.GetMainAssetTypeAtPath(p);
                if(type==null || type==typeof(MonoScript)) continue;
                UnityEngine.Object asset=AssetDatabase.LoadMainAssetAtPath(p);
                if(asset==null) continue;
                GameObject go=asset as GameObject; long tris=0, verts=0; int slots=0,colliders=0,lods=0,groups=0;
                string scale="", bounds="", shader="";
                if(go!=null)
                {
                    Mesh[] meshes=go.GetComponentsInChildren<MeshFilter>(true).Select(m=>m.sharedMesh).Concat(go.GetComponentsInChildren<SkinnedMeshRenderer>(true).Select(m=>m.sharedMesh)).Where(m=>m!=null).Distinct().ToArray();
                    foreach(Mesh m in meshes){verts+=m.vertexCount;for(int j=0;j<m.subMeshCount;j++)if(m.GetTopology(j)==MeshTopology.Triangles)tris+=m.GetIndexCount(j)/3;}
                    Renderer[] rs=go.GetComponentsInChildren<Renderer>(true); slots=rs.Sum(r=>r.sharedMaterials.Length);
                    var lg=go.GetComponentsInChildren<LODGroup>(true);groups=lg.Length;lods=lg.Sum(l=>l.lodCount);
                    colliders=go.GetComponentsInChildren<Collider>(true).Length;scale=go.transform.localScale.ToString("R");
                    if(rs.Length>0){Bounds b=rs[0].bounds;foreach(var r in rs.Skip(1))b.Encapsulate(r.bounds);bounds=b.size.ToString("R");}
                }
                Texture2D texture=asset as Texture2D; Material material=asset as Material;
                if(material!=null)shader=material.shader!=null?material.shader.name:"MISSING";
                string textures=string.Join(";",AssetDatabase.GetDependencies(p,true).Where(d=>new[]{".png",".jpg",".jpeg",".tga",".exr",".hdr"}.Contains(Path.GetExtension(d).ToLowerInvariant())));
                w.WriteLine(string.Join(",",new object[]{p,type.Name,asset.name,scenes.Contains(p),prefabs.Contains(p),tris,verts,slots,textures,texture?.width??0,texture?.height??0,groups,lods,colliders,scale,bounds,shader,"requires provenance register review",go!=null?"imported hierarchy; runtime use measured separately":""}.Select(Q)));
                if(++n%150==0){EditorUtility.UnloadUnusedAssetsImmediate();GC.Collect();}
            }
        }
        Debug.Log("UNDERSTOREY_INVENTORY_PASS paths="+paths.Length);
    }
#endif
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)] private static void Install()
    {
        if(Environment.GetCommandLineArgs().Contains("UnderstoreyDiagnostics.Begin"))new GameObject("Understorey diagnostics").AddComponent<UnderstoreyRunner>();
    }
}
public sealed class UnderstoreyRunner:MonoBehaviour
{
    private const BindingFlags Private=BindingFlags.Instance|BindingFlags.NonPublic;
    private ForestEcologyController e;private ForestStartingStand stand;private ForestTreeSpawner spawner;private ScenarioOneManager manager;
    private static string F(double n)=>n.ToString("R",CultureInfo.InvariantCulture);
    private void Check(bool yes,string message){if(!yes)throw new InvalidOperationException(message);}
    private void Call(object owner,string name,params object[] args)=>owner.GetType().GetMethod(name,Private).Invoke(owner,args);
    private void Set(object owner,string name,object value)=>owner.GetType().GetField(name,Private).SetValue(owner,value);
    private ForestTree[] Living()=>FindObjectsByType<ForestTree>(FindObjectsSortMode.None).Where(t=>t.IsLiving).OrderBy(t=>t.TreeId,StringComparer.Ordinal).ToArray();
    private void Models(){e.RngModelVersion=1;e.RegenerationModelVersion=1;e.GrowthModelVersion=1;}
    private void Build(float removal,float browse)
    {
        foreach(var t in FindObjectsByType<ForestTree>(FindObjectsInactive.Include,FindObjectsSortMode.None))DestroyImmediate(t.gameObject);
        manager.InitializeNewScenario();e.ResetForDeterministicRun();Models();stand.Generate();e.ResetForDeterministicRun();Models();e.Browsing.BackgroundPressure=browse;e.Browsing.ClearProtection();
        var ts=Living();e.BeginChangeBatch();foreach(var t in ts.OrderBy(t=>t.Diameter).ThenBy(t=>t.TreeId,StringComparer.Ordinal).Take(Mathf.RoundToInt(ts.Length*removal)))t.Fell();e.EndChangeBatch();
        Call(manager,"EnsureUnderstoreyGrid");
    }
    private float Abundance(string species)=>e.Cells.Sum(c=>c.Regeneration.Where(b=>b.SpeciesId==species).Sum(b=>b.Density));
    private string Hash()
    {
        var b=new StringBuilder();
        foreach(var t in Living())b.Append(t.TreeId).Append('/').Append(F(t.Height)).Append('/').Append(F(t.Diameter)).Append('/').Append(t.AgeYears).Append(';');
        foreach(var c in e.Cells)foreach(var r in c.Regeneration)b.Append(r.SpeciesId).Append('/').Append(F(r.Density)).Append('/').Append(F(r.Height)).Append('/').Append(r.EstablishYear).Append(';');
        using(var sha=SHA256.Create())return BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(b.ToString()))).Replace("-","");
    }
    private IEnumerator GapFixtures()
    {
        using(var w=new StreamWriter(Path.Combine(UnderstoreyDiagnostics.Output,"gap_fixtures.csv")))
        {
            w.WriteLine("species,adapter,light,browse,cover,after_density_or_alive,after_height,established_abundance");
            foreach(string species in new[]{"sitka-spruce","sessile-oak","beech"})
            {
                TreeSpeciesDefinition s=spawner.ResolveSpecies(species);
                Check(s!=null,"Missing fixture species "+species);
                foreach(string adapter in new[]{"natural","planted"})foreach(float light in new[]{.08f,.8f})foreach(float browse in new[]{0f,.8f})
                {
                    string expected=null;
                    foreach(float cover in new[]{0f,1f})
                    {
                        Build(0,browse);foreach(var t in FindObjectsByType<ForestTree>(FindObjectsInactive.Include,FindObjectsSortMode.None))DestroyImmediate(t.gameObject);
                        e.ResetForDeterministicRun();Models();e.Browsing.BackgroundPressure=browse;e.RecomputeCanopy();e.RecomputeSeedRain();
                        foreach(var c in e.Cells){c.ClearRegeneration();c.Light=light;}
                        foreach(var u in manager.UnderstoreyCells){u.ferns=u.grasses=u.forbs=u.shrubs=cover;}
                        var cell=e.Cells[0];var band=new ForestRegenerationCohort(s);band.Restore(.2f,.2f,0);cell.InsertBand(band);
                        var juveniles=(List<PlantedJuvenile>)typeof(ScenarioOneManager).GetField("plantedJuveniles",Private).GetValue(manager);juveniles.Clear();
                        var j=new PlantedJuvenile{juvenileId="gap-fixed",speciesId=s.SpeciesId,position=new Vector3(cell.Center.x,0,cell.Center.y),heightMeters=.2f,ageYears=1};
                        if(adapter=="planted")juveniles.Add(j);
                        Set(e,"ecologicalYear",1);Call(e,"GrowExistingRegeneration");Call(e,"EstablishNewCohorts");
                        if(adapter=="planted")Call(manager,"AdvancePlantedJuveniles");
                        float abundance=cell.Regeneration.Sum(r=>r.Density);float result=adapter=="natural"?abundance:(j.alive?1:0);float height=adapter=="natural"?cell.Regeneration.First().Height:j.heightMeters;
                        string value=F(result)+"/"+F(height)+"/"+F(abundance);if(expected==null)expected=value;else Check(value==expected,"Cover changed baseline juvenile outcome");
                        Check(abundance<=.2f+1e-6f,"No seed created new abundance");
                        w.WriteLine(string.Join(",",s.SpeciesId,adapter,F(light),F(browse),F(cover),F(result),F(height),F(abundance)));
                    }
                    yield return null;
                }
            }
        }
        Debug.Log("UNDERSTOREY_GAP_PASS species=3 adapters=2 light=2 browse=2 cover=2 noSeed=true");
    }
    private IEnumerator LongRuns()
    {
        using(var w=new StreamWriter(Path.Combine(UnderstoreyDiagnostics.Output,"baseline_years.csv")))
        using(var h=new StreamWriter(Path.Combine(UnderstoreyDiagnostics.Output,"repeat_hashes.csv")))
        {
            w.WriteLine("run,repeat,year,species,seed_arrival,requested,accepted,rejected,light_loss,browse_loss,exported,exact_recruits,remaining,ferns,grasses,shrubs,living_adults,deadwood_records,annual_ms,clearance_cells");
            h.WriteLine("run,repeat,year,hash");
            foreach(float thin in new[]{0f,.2f,.4f,.6f})foreach(float browse in new[]{0f,.8f})foreach(int clearance in new[]{0,1,5})
            {
                // Reduced factorial: full no-treatment contrasts plus targeted clearance at moderate/heavy openings.
                if(clearance!=0&&thin<.4f)continue;
                string label="thin"+F(thin)+"_browse"+F(browse)+"_clear"+clearance;var hashes=new Dictionary<int,string>();
                for(int repeat=0;repeat<2;repeat++)
                {
                    Build(thin,browse);
                    for(int y=1;y<=100;y++)
                    {
                        int cleared=0;
                        if((clearance==1&&y==1)||(clearance==5&&(y==1||y%5==0)))
                            for(int i=0;i<e.CellCount;i++)if(e.Cells[i].Light>=.4f)
                            {
                                var targets=manager.QueryClearance(ClearanceFootprint.Cell(e,i),y);Call(manager,"ApplyClearance",targets,y);cleared++;
                            }
                        var watch=Stopwatch.StartNew();e.AdvanceOneYear();Call(manager,"AdvancePlantedJuveniles");Call(manager,"AdvanceUnderstorey");watch.Stop();
                        foreach(var pair in e.LastRegenerationAccount.Species)
                        {
                            var a=pair.Value;float rest=Abundance(pair.Key);
                            Check(rest>=0&&a.InfillAccepted==0,"Model 1 stock invariant");Check(Math.Abs(a.EstablishmentRequested-a.EstablishmentAccepted-a.CapacityRejected)<.0001,"Recruitment accounting");
                            w.WriteLine(string.Join(",",label,repeat,y,pair.Key,F(a.SeedArrival),F(a.EstablishmentRequested),F(a.EstablishmentAccepted),F(a.CapacityRejected),F(a.LightLoss),F(a.BrowseLoss),F(a.PromotionExported),a.ExactTreesCreated,F(rest),F(manager.UnderstoreyCells.Average(u=>u.ferns)),F(manager.UnderstoreyCells.Average(u=>u.grasses)),F(manager.UnderstoreyCells.Average(u=>u.shrubs)),Living().Length,manager.DeadwoodRecords.Count,F(watch.Elapsed.TotalMilliseconds),cleared));
                        }
                        if(new[]{10,25,50,100}.Contains(y)){string hash=Hash();h.WriteLine(label+","+repeat+","+y+","+hash);if(repeat==0)hashes[y]=hash;else Check(hashes[y]==hash,"Non-deterministic baseline "+label+" year "+y);}
                        if(y%10==0){yield return null;yield return Resources.UnloadUnusedAssets();GC.Collect();}
                    }
                    Debug.Log("UNDERSTOREY_RUN_PASS "+label+" repeat="+repeat);
                }
            }
        }
    }
    private IEnumerator Execute()
    {
        e=FindFirstObjectByType<ForestEcologyController>();stand=FindFirstObjectByType<ForestStartingStand>();spawner=FindFirstObjectByType<ForestTreeSpawner>();manager=FindFirstObjectByType<ScenarioOneManager>();
        Models();yield return GapFixtures();yield return LongRuns();Debug.Log("UNDERSTOREY_DIAGNOSTICS_PASS");
    }
    private IEnumerator Start()
    {
        yield return null;yield return null;
        // Flatten nested iterators so errors are captured and the disposable Editor always exits.
        var stack=new Stack<IEnumerator>();stack.Push(Execute());string failure=null;
        while(stack.Count>0){object current=null;bool more=false;try{more=stack.Peek().MoveNext();if(more)current=stack.Peek().Current;}catch(Exception ex){failure=ex.ToString();break;}
            if(!more){stack.Pop();continue;}if(current is IEnumerator nested){stack.Push(nested);continue;}yield return current;}
        if(failure!=null)Debug.LogError("UNDERSTOREY_DIAGNOSTICS_FAIL "+failure);
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();EditorApplication.Exit(failure==null?0:1);
#endif
    }
}
