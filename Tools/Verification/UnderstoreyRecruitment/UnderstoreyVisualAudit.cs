using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
public static class UnderstoreyVisualAudit
{
#if UNITY_EDITOR
 public static void Begin(){EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");Size(1600,900);EditorApplication.isPlaying=true;}
 public static void Size(int width,int height)
 {
  Assembly a=typeof(Editor).Assembly;Type t=a.GetType("UnityEditor.GameViewSizes");object sizes=typeof(ScriptableSingleton<>).MakeGenericType(t).GetProperty("instance",BindingFlags.Public|BindingFlags.Static).GetValue(null);var get=t.GetMethod("GetGroup");object group=get.Invoke(sizes,new[]{Enum.ToObject(get.GetParameters()[0].ParameterType,0)});
  object size=Activator.CreateInstance(a.GetType("UnityEditor.GameViewSize"),BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance,null,new object[]{Enum.ToObject(a.GetType("UnityEditor.GameViewSizeType"),1),width,height,"Asset audit "+width},null);int index=(int)group.GetType().GetMethod("GetTotalCount").Invoke(group,null);group.GetType().GetMethod("AddCustomSize").Invoke(group,new[]{size});Type view=a.GetType("UnityEditor.GameView");EditorWindow win=EditorWindow.GetWindow(view);view.GetProperty("selectedSizeIndex",BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance).SetValue(win,index);var gizmos=view.GetProperty("showGizmos",BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance);if(gizmos!=null&&gizmos.CanWrite)gizmos.SetValue(win,false);win.Show();win.Focus();win.Repaint();
 }
#endif
 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]private static void Install(){if(Environment.GetCommandLineArgs().Contains("UnderstoreyVisualAudit.Begin"))new GameObject("Visual audit").AddComponent<UnderstoreyVisualRunner>();}
}
public sealed class UnderstoreyVisualRunner:MonoBehaviour
{
 private string dir;private ForestEcologyController e;private ScenarioOneManager m;private ForestPlayer player;private ScenarioOneUiRoot ui;
 private IEnumerator Shot(string name){for(int i=0;i<120;i++)yield return null;ui.CloseHelp();yield return new WaitForEndOfFrame();var tex=ScreenCapture.CaptureScreenshotAsTexture();if(tex==null)throw new Exception("No screenshot");File.WriteAllBytes(Path.Combine(dir,name+".png"),tex.EncodeToPNG());Destroy(tex);}
 private void Measure(string adapter,string species,float target,string objectName)
 {
  var visual=FindObjectsByType<Transform>(FindObjectsSortMode.None).FirstOrDefault(x=>x.name==objectName);if(visual==null)throw new Exception("Missing measured juvenile "+objectName);
  var rs=visual.GetComponentsInChildren<Renderer>(true);float min=rs.Min(x=>x.bounds.min.y),max=rs.Max(x=>x.bounds.max.y);File.AppendAllText(Path.Combine(dir,"juvenile_display_heights.csv"),string.Join(",",adapter,species,target.ToString(System.Globalization.CultureInfo.InvariantCulture),(max-min).ToString(System.Globalization.CultureInfo.InvariantCulture),visual.localScale.y.ToString(System.Globalization.CultureInfo.InvariantCulture))+"\n");
 }
 private IEnumerator Execute()
 {
  dir=Environment.GetEnvironmentVariable("CCF_UNDERSTOREY_OUTPUT");Directory.CreateDirectory(dir);File.WriteAllText(Path.Combine(dir,"juvenile_display_heights.csv"),"adapter,species,recorded_height_m,rendered_bounds_height_m,root_scale_y\n");e=FindFirstObjectByType<ForestEcologyController>();m=FindFirstObjectByType<ScenarioOneManager>();player=FindFirstObjectByType<ForestPlayer>();ui=FindFirstObjectByType<ScenarioOneUiRoot>();ui.CloseAll();
  using(var w=new StreamWriter(Path.Combine(dir,"runtime_renderers.csv")))
  {
   w.WriteLine("object_name,mesh_asset,material_slots,cast_shadows,colliders_local,world_bounds,scale");
   foreach(var r in FindObjectsByType<Renderer>(FindObjectsSortMode.None))
   {
    var filter=r.GetComponent<MeshFilter>();string path="procedural";
#if UNITY_EDITOR
    if(filter!=null&&filter.sharedMesh!=null)path=AssetDatabase.GetAssetPath(filter.sharedMesh);
#endif
    string Q(object v)=>"\""+v.ToString().Replace("\"","\"\"")+"\"";
    w.WriteLine(string.Join(",",new object[]{r.gameObject.name,path,r.sharedMaterials.Length,r.shadowCastingMode,r.GetComponents<Collider>().Length,r.bounds.size.ToString("R"),r.transform.lossyScale.ToString("R")}.Select(Q)));
   }
  }
  foreach(var size in new[]{new Vector2Int(1280,720),new Vector2Int(1600,900),new Vector2Int(1920,1080)})
  {
#if UNITY_EDITOR
   UnderstoreyVisualAudit.Size(size.x,size.y);
#endif
   yield return Shot("plantation-"+size.x);ui.ShowMap();yield return Shot("map-"+size.x);ui.ShowWorkPlan();yield return Shot("workplan-"+size.x);ui.ShowReview();yield return Shot("review-"+size.x);ui.CloseAll();
  }
  var spawner=FindFirstObjectByType<ForestTreeSpawner>();int cell=e.GetCellIndex(player.transform.position+player.transform.forward*4);if(cell<0)cell=e.CellCount/2;
  Vector2 p=e.Cells[cell].Center;
  foreach(string species in new[]{"sitka-spruce","sessile-oak","beech"})
  {
   var s=spawner.ResolveSpecies(species);var band=new ForestRegenerationCohort(s);band.Restore(.3f,.4f,0);e.Cells[cell].InsertBand(band);cell=(cell+1)%e.CellCount;
  }
  e.RefreshRegenerationDisplays();player.LookToward(new Vector3(p.x,.4f,p.y));yield return Shot("juvenile-patch");
  foreach(var u in m.UnderstoreyCells){u.ferns=.7f;u.grasses=.6f;u.shrubs=.7f;}
  var visuals=FindFirstObjectByType<ScenarioHabitatVisuals>();visuals.Rebuild(e,m.UnderstoreyCells,m.DeadwoodRecords,m.PlantedJuveniles);yield return Shot("heavy-understorey");
  var targets=m.QueryClearance(ClearanceFootprint.Cell(e,e.GetCellIndex(new Vector3(p.x,0,p.y))),e.EcologicalYear);
  typeof(ScenarioOneManager).GetMethod("ApplyClearance",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(m,new object[]{targets,e.EcologicalYear});visuals.Rebuild(e,m.UnderstoreyCells,m.DeadwoodRecords,m.PlantedJuveniles);yield return Shot("cleared-patch");
  foreach(string species in new[]{"sitka-spruce","sessile-oak","beech"})
  {
   int index=e.GetCellIndex(new Vector3(p.x,0,p.y));
   if(index<0)throw new Exception("Missing staged visual cell");e.Cells[index].ClearRegeneration();var s=spawner.ResolveSpecies(species);var band=new ForestRegenerationCohort(s);band.Restore(.3f,.6f,0);e.Cells[index].InsertBand(band);e.RefreshRegenerationDisplays();
   Vector3 location=e.RegenerationDisplayPosition(index,species);player.transform.position=location+new Vector3(0,0,-2);player.LookToward(location+Vector3.up*.35f);yield return Shot("isolated-natural-"+species);Measure("natural",species,.6f,s.DisplayName+" seedling cell "+index);
  }
  var planted=(System.Collections.Generic.List<PlantedJuvenile>)typeof(ScenarioOneManager).GetField("plantedJuveniles",BindingFlags.Instance|BindingFlags.NonPublic).GetValue(m);
  foreach(string species in new[]{"sessile-oak","beech"})
  {
   Vector3 location=new Vector3(p.x+1,0,p.y);planted.Clear();planted.Add(new PlantedJuvenile{juvenileId="visual-"+species,speciesId=species,position=location,heightMeters=.6f,ageYears=1});visuals.Rebuild(e,m.UnderstoreyCells,m.DeadwoodRecords,m.PlantedJuveniles);player.transform.position=location+new Vector3(0,0,-2);player.LookToward(location+Vector3.up*.3f);yield return Shot("isolated-planted-"+species);Measure("planted",species,.6f,"Planted juvenile visual-"+species);
  }
  var alive=FindObjectsByType<ForestTree>(FindObjectsSortMode.None).Where(t=>t.IsLiving).OrderBy(t=>t.TreeId).ToArray();
  foreach(var removed in alive.Take(alive.Length/3))removed.Fell();e.RecomputeCanopy();player.transform.position=new Vector3(p.x,0,p.y);player.LookToward(new Vector3(0,2,0));yield return Shot("thinned-area");
  var mortality=FindObjectsByType<ForestTree>(FindObjectsSortMode.None).First(t=>t.IsLiving);Vector3 deadPosition=mortality.transform.position;mortality.ApplyMortality("audit-fixture",e.EcologicalYear);visuals.Rebuild(e,m.UnderstoreyCells,m.DeadwoodRecords,m.PlantedJuveniles);player.transform.position=deadPosition+new Vector3(0,0,-3);player.LookToward(deadPosition+Vector3.up*.15f);yield return Shot("deadwood");
  var tree=FindObjectsByType<ForestTree>(FindObjectsSortMode.None).Where(t=>t.IsLiving).OrderBy(t=>(t.transform.position-player.transform.position).sqrMagnitude).First();player.transform.position=tree.transform.position+new Vector3(0,0,-2);player.LookToward(tree.InteractionPoint);typeof(ForestPlayer).GetMethod("InspectTree",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(player,new object[]{tree});yield return Shot("inspection");
  Debug.Log("UNDERSTOREY_VISUAL_AUDIT_PASS sizes=3 stagedJuveniles=true stagedCover=true clearance=true");
 }
 private IEnumerator Start()
 {
  yield return null;yield return null;var stack=new System.Collections.Generic.Stack<IEnumerator>();stack.Push(Execute());string error=null;
  while(stack.Count>0){bool more=false;object next=null;try{more=stack.Peek().MoveNext();if(more)next=stack.Peek().Current;}catch(Exception ex){error=ex.ToString();break;}if(!more){stack.Pop();continue;}if(next is IEnumerator nested){stack.Push(nested);continue;}yield return next;}
  if(error!=null)Debug.LogError("UNDERSTOREY_VISUAL_AUDIT_FAIL "+error);
#if UNITY_EDITOR
  EditorApplication.ExitPlaymode();EditorApplication.Exit(error==null?0:1);
#endif
 }
}
