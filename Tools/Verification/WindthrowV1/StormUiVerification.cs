#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using Stopwatch=System.Diagnostics.Stopwatch;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UIElements;
using Unity.Profiling;
public static class StormUiVerification
{
 public static void Begin(){EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");SetGameSize(1600,900);EditorApplication.isPlaying=true;}
    public static void SetGameSize(int width, int height)
    {
        if (Application.isBatchMode) return;
        Assembly assembly = typeof(Editor).Assembly;
        Type sizesType = assembly.GetType("UnityEditor.GameViewSizes");
        Type singleton = typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
        object sizes = singleton.GetProperty("instance", BindingFlags.Public | BindingFlags.Static).GetValue(null);
        MethodInfo getGroup = sizesType.GetMethod("GetGroup");
        object group = getGroup.Invoke(sizes, new[] { Enum.ToObject(getGroup.GetParameters()[0].ParameterType, 0) });
        Type sizeType = assembly.GetType("UnityEditor.GameViewSize"), kind = assembly.GetType("UnityEditor.GameViewSizeType");
        object size = Activator.CreateInstance(sizeType, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
            null, new object[] { Enum.ToObject(kind, 1), width, height, "Menu tutorial review " + width + "x" + height }, null);
        int index = (int)group.GetType().GetMethod("GetTotalCount").Invoke(group, null);
        group.GetType().GetMethod("AddCustomSize").Invoke(group, new[] { size });
        Type viewType = assembly.GetType("UnityEditor.GameView"); EditorWindow view = EditorWindow.GetWindow(viewType);
        viewType.GetProperty("selectedSizeIndex", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).SetValue(view, index);
        var gizmos = viewType.GetProperty("showGizmos", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (gizmos != null && gizmos.CanWrite) gizmos.SetValue(view, false);
        view.Show(); view.Focus(); view.Repaint();
    }

 public static readonly Dictionary<string,int?> Before=new Dictionary<string,int?>();
 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]static void Prepare(){if(!Environment.GetCommandLineArgs().Contains("StormUiVerification.Begin"))return;foreach(MenuHelpView.Menu menu in Enum.GetValues(typeof(MenuHelpView.Menu))){string key=MenuHelpView.PreferencePrefix+menu;Before[key]=PlayerPrefs.HasKey(key)?PlayerPrefs.GetInt(key):(int?)null;PlayerPrefs.SetInt(key,1);}foreach(string id in LearningObjectivesView.StepIds){string key=LearningObjectivesView.PreferencePrefix+id;Before[key]=PlayerPrefs.HasKey(key)?PlayerPrefs.GetInt(key):(int?)null;}}
 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]static void Install(){if(Environment.GetCommandLineArgs().Contains("StormUiVerification.Begin"))new GameObject("Disposable storm UI verification").AddComponent<StormUiVerificationRunner>();}
}
public sealed class StormUiVerificationRunner:MonoBehaviour
{
 ForestEcologyController e;ScenarioOneManager m;ForestSaveController saves;ScenarioOneUiRoot ui;ForestPlayer player;ForestSaveData original;int checks;string dir;Keyboard testKeyboard;
 void Check(bool condition,string reason){checks++;if(!condition)throw new Exception(reason);}
 string Hash()=>ScenarioReferenceArchive.WorldHash(saves.CaptureData());
 IEnumerator Start()
 {
  yield return null;yield return null;Exception failed=null;var steps=new Stack<IEnumerator>();steps.Push(Run());
  while(steps.Count>0){bool more=false;object current=null;try{more=steps.Peek().MoveNext();if(more)current=steps.Peek().Current;}catch(Exception error){failed=error;Debug.LogError("STORM_UI_VERIFICATION_FAIL "+error);break;}if(!more){steps.Pop();continue;}if(current is IEnumerator child){steps.Push(child);continue;}yield return current;}
  if(original!=null&&saves!=null)saves.LoadData(original,false);foreach(var value in StormUiVerification.Before)if(value.Value.HasValue)PlayerPrefs.SetInt(value.Key,value.Value.Value);else PlayerPrefs.DeleteKey(value.Key);PlayerPrefs.Save();
  if(testKeyboard!=null)InputSystem.RemoveDevice(testKeyboard);if(failed==null)Debug.Log("STORM_UI_VERIFICATION_PASS checks="+checks);EditorApplication.ExitPlaymode();EditorApplication.Exit(failed==null?0:1);
 }
 IEnumerator Press(Key key){var viewType=typeof(Editor).Assembly.GetType("UnityEditor.GameView");EditorWindow.GetWindow(viewType).Focus();yield return null;testKeyboard.MakeCurrent();InputSystem.QueueStateEvent(testKeyboard,new KeyboardState(key));yield return null;yield return null;InputSystem.QueueStateEvent(testKeyboard,new KeyboardState());yield return null;yield return null;}
 IEnumerator Click(Button button){Check(button!=null&&button.enabledInHierarchy,"Enabled UI action required");typeof(Clickable).GetMethod("Invoke",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(button.clickable,new object[]{null});yield return null;yield return null;}
 IEnumerator Capture(string name){for(int i=0;i<5;i++)yield return null;yield return new WaitForEndOfFrame();var capture=ScreenCapture.CaptureScreenshotAsTexture();Check(capture!=null&&capture.width>600,"Rendered capture");File.WriteAllBytes(Path.Combine(dir,name+".png"),capture.EncodeToPNG());Destroy(capture);}
 IEnumerator Run()
 {
  e=FindFirstObjectByType<ForestEcologyController>();m=FindFirstObjectByType<ScenarioOneManager>();saves=FindFirstObjectByType<ForestSaveController>();ui=FindFirstObjectByType<ScenarioOneUiRoot>();player=ui.Player;original=saves.CaptureData();dir=Environment.GetEnvironmentVariable("CCF_STORM_OUTPUT");testKeyboard=InputSystem.AddDevice<Keyboard>("Storm verification keyboard");testKeyboard.MakeCurrent();
  foreach(var tree in FindObjectsByType<ForestTree>(FindObjectsSortMode.None).Where(t=>t.IsLiving&&t.TreeId.EndsWith("0")))tree.SetMark(TreeMarkType.CropTree);
  e.StormModelVersion=1;e.UseStormCalibrationForVerification(new StormCalibration(0,new[]{.02f,.06f,.18f},new[]{1f,1f,1f}));e.ForceStormNextYear(.7f,135);Check(m.AdvanceYear(),"Resolve visible storm");yield return null;yield return null;
  var summary=m.StormDamageInYear(1);Check(summary!=null&&summary.TreesLost>0&&summary.WaypointCell>=0,"Derived damage summary");var physical=saves.CaptureData();string before=Hash();
  ui.ShowReview();ui.CloseHelp();yield return null;yield return null;
  var card=ui.RootElement.Q<VisualElement>("annual-storm-review");Check(card!=null&&card.Query<Label>().ToList().Any(l=>l.text.Contains("Crop Trees")),"Crop-loss and storm review text");
  foreach(var size in new[]{new Vector2Int(1280,720),new Vector2Int(1600,900),new Vector2Int(1920,1080)})
  {
   StormUiVerification.SetGameSize(size.x,size.y);yield return null;yield return null;card=ui.RootElement.Q<VisualElement>("annual-storm-review");var scroll=card.GetFirstAncestorOfType<ScrollView>();scroll.ScrollTo(card);yield return Capture("storm-review-"+size.x);
   Rect bounds=scroll.contentViewport.worldBound;Check(bounds.width>500&&bounds.yMax<=ui.RootElement.worldBound.yMax,"Review scroll stays inside viewport");
  }
  Check(Hash()==before,"Reading review changes no saved state");yield return Click(card.Query<Button>().ToList().Single(b=>b.text=="Mark storm damage on the map"));Check(ui.CurrentScreen==ScenarioOneUiRoot.UiScreen.Map&&ui.Map.HasWaypoint,"Actual storm callback sets map waypoint");yield return Press(Key.M);ui.CloseHelp();yield return null;yield return null;
  Debug.Log("STORM_WAYPOINT_DIAGNOSTIC screen="+ui.CurrentScreen+" hud="+ui.RootElement.Q("hud-waypoint-panel").resolvedStyle.display+" keyboard="+Keyboard.current.name+" focus="+Application.isFocused);Check(ui.CurrentScreen==ScenarioOneUiRoot.UiScreen.None&&ui.RootElement.Q("hud-waypoint-panel").resolvedStyle.display==DisplayStyle.Flex,"Storm waypoint shown on walking HUD after M return");Check(Hash()==before,"Waypoint modifies no forest state");yield return Capture("storm-waypoint-hud");ui.ShowReview();ui.CloseHelp();yield return null;yield return Click(ui.RootElement.Q<Button>("annual-review-acknowledge"));Check(m.AnnualReviewSeen,"Actual review acknowledgement enables subsequent work");ui.CloseAll();yield return null;
  var visuals=m.GetComponentsInChildren<ScenarioWindthrowVisual>();bool aimed=false;ScenarioWindthrowVisual target=null;
  foreach(var visual in visuals.OrderByDescending(v=>e.Cells[e.GetCellIndex(v.transform.position)].Light))
  {
   ui.CloseAll();ui.CloseHelp();var box=visual.GetComponent<BoxCollider>();Vector3 point=visual.transform.TransformPoint(box.center);player.transform.position=point+visual.transform.right*2;player.LookToward(point);UnityEngine.Cursor.lockState=CursorLockMode.Locked;UnityEngine.Cursor.visible=false;Physics.SyncTransforms();for(int frame=0;frame<8;frame++){player.LookToward(point);if(Mouse.current!=null)InputSystem.QueueDeltaStateEvent(Mouse.current.delta,Vector2.zero);yield return null;}
   Debug.Log("STORM_AIM_DIAGNOSTIC target="+visual.TreeId+" cursor="+UnityEngine.Cursor.lockState+" player="+player.enabled+" panels="+m.AnyPanelOpen+" camera="+Camera.main.transform.position+" point="+point+" forward="+Camera.main.transform.forward+" prompt="+player.InteractionPromptText()+" hits="+string.Join(",",Physics.RaycastAll(Camera.main.transform.position,Camera.main.transform.forward,3.5f).OrderBy(h=>h.distance).Select(h=>h.collider.name+":"+h.distance)));
   if(player.InteractionPromptText().Contains(visual.TreeId)&&player.InteractionPromptText().Contains("salvage")){aimed=true;target=visual;break;}
  }
  Check(aimed,"Actual camera raycast produces fallen-stem HUD prompt");yield return Capture("aim-fallen-stem");yield return Press(Key.X);Check(m.WorkOrders.Any(o=>o.type==ScenarioWorkType.SalvageDeadwood&&o.targetTreeId==target.TreeId&&o.status==ScenarioWorkStatus.Pending),"Actual X adds salvage");yield return Press(Key.X);Check(!m.WorkOrders.Any(o=>o.type==ScenarioWorkType.SalvageDeadwood&&o.targetTreeId==target.TreeId&&o.status==ScenarioWorkStatus.Pending),"Actual X cancels salvage");yield return Press(Key.X);yield return Press(Key.Tab);ui.CloseHelp();yield return null;yield return null;
  Check(ui.RootElement.Query<Label>().ToList().Any(l=>l.text.Contains("SALVAGE")),"Work plan exposes salvage type");Check(ui.RootElement.Query<Button>().ToList().Any(b=>b.text=="Keep for use"),"Salvage supports Keep");yield return Click(ui.RootElement.Query<Button>().ToList().First(b=>b.text=="Keep for use"));Check(m.OpenSalvageOrder(target.TreeId).fellingOutcome==FellingMaterialOutcome.KeepForUse,"Actual Keep callback selects retained timber");yield return Click(ui.RootElement.Query<Button>().ToList().First(b=>b.text=="Sell"));Check(m.OpenSalvageOrder(target.TreeId).fellingOutcome==FellingMaterialOutcome.SellAndExtract,"Actual Sell callback selects optional sale");Check(!ui.RootElement.Query<Button>().ToList().Any(b=>b.text=="Leave as deadwood"&&b.enabledInHierarchy),"Salvage has no paid Leave option");yield return Capture("salvage-work-plan");
  ui.CloseAll();Check(m.ApprovePendingWork()&&m.AdvanceYear(),"Salvage-only job resolves");yield return null;ui.ShowReview();ui.CloseHelp();yield return null;yield return null;Check(ui.RootElement.Query<Label>().ToList().Any(l=>l.text.Contains("windthrow stem(s) salvaged")),"Annual work shows pure salvage job");Check(ui.RootElement.Query<Label>().ToList().Any(l=>l.text.Contains("Small-job minimum")),"Pure salvage minimum visible");yield return Capture("salvage-only-annual-money");
  Check(saves.LoadData(physical,false),"Restore fresh visual fixture");yield return null;ui.CloseAll();ui.CloseHelp();var budget=m.GetComponentInChildren<ScenarioWindthrowVisualBudget>();target=m.GetComponentsInChildren<ScenarioWindthrowVisual>().OrderByDescending(v=>e.Cells[e.GetCellIndex(v.transform.position)].Light).First();
  player.enabled=false;player.transform.position=target.transform.TransformPoint(new Vector3(-2,0,-2));player.LookToward(target.transform.position+Vector3.up*.6f);budget.RefreshAt(Camera.main.transform.position);yield return Capture("fresh-root-plate-visible");
  player.transform.position=new Vector3(0,20,-28);player.LookToward(new Vector3(0,0,0));budget.RefreshAt(Camera.main.transform.position);yield return Capture("fresh-fallen-crown-overview");yield return RenderComparison("fresh",m.GetComponentsInChildren<ScenarioWindthrowVisual>());
  yield return BenchmarkResolvedVictims();
  // Independent diagnostic century fixture includes normal growth mortality and storms.
  Check(saves.LoadData(original,false),"Century render baseline");yield return null;e.StormModelVersion=1;e.UseStormCalibrationForVerification(new StormCalibration(.05f,new[]{.02f,.06f,.18f},new[]{1f,1f,1f}));
  for(int year=0;year<100;year++){if(!m.AdvanceYear())break;yield return null;}ui.CloseAll();ui.CloseHelp();player.enabled=false;player.transform.position=new Vector3(0,20,-28);player.LookToward(Vector3.zero);yield return Capture("century-combined-deadwood");yield return RenderComparison("century",m.GetComponentsInChildren<ScenarioWindthrowVisual>());
 }
 float ExactIntensity(int desired)
 {
  int year=e.EcologicalYear+1;var values=e.EvaluateStorm(new StormEventRecord{year=year,severity=1,directionDegrees=135});
  var thresholds=values.Select(value=>{double roll=SimulationRandom.Roll(e.RngModelVersion,"WINDTHROW-v1-"+value.Tree.TreeId,year,e.SimulationSeed);return roll/((1-roll)*value.Vulnerability);}).OrderBy(value=>value).ToArray();
  Check(desired>0&&desired<thresholds.Length,"Bounded requested render damage size");float severity=(float)((thresholds[desired-1]+thresholds[desired])*.5);Check(severity>0&&severity<=1,"Render damage achievable within bounded intensity");
  Check(e.EvaluateStorm(new StormEventRecord{year=year,severity=severity,directionDegrees=135}).Count(value=>value.Victim)==desired,"Actual preview yields exact render victim count");return severity;
 }
 IEnumerator BenchmarkResolvedVictims()
 {
  Check(saves.LoadData(original,false),"Age-development render fixture restore");yield return null;
  for(int year=0;year<20;year++){Check(m.AdvanceYear(),"Develop authored forest for render damage sizes");yield return null;}
  var aged=saves.CaptureData();
  using(var table=new StreamWriter(Path.Combine(dir,"render-event-step-cost.csv")))
  {
   table.WriteLine("condition,living_before,victims,intensity,total_annual_ms,evaluation_ms,mortality_ms,rebuild_ms,record_ms_subset_of_mortality,visual_creation_ms,canopy_rebuilds,seed_rebuilds");
   foreach(int desired in new[]{10,50,100,500})
   {
    Check(saves.LoadData(aged,false),"Fresh independent render event");yield return null;
    if(desired==500)
    {
     foreach(var tree in FindObjectsByType<ForestTree>(FindObjectsInactive.Include,FindObjectsSortMode.None))DestroyImmediate(tree.gameObject);
     m.InitializeNewScenario();e.ResetForDeterministicRun();e.RngModelVersion=1;e.RegenerationModelVersion=2;e.GrowthModelVersion=0;
     var spawner=FindFirstObjectByType<ForestTreeSpawner>();const int count=1300;int side=Mathf.CeilToInt(Mathf.Sqrt(count));
     for(int i=0;i<count;i++)spawner.Spawn("RENDER-STORM-"+i.ToString("0000"),spawner.DefaultSpecies,new Vector3((i%side+.5f)*40/side-20,0,(i/side+.5f)*40/side-20),40,30,25,spawner.DefaultSpecies.PotentialCrownRadiusM(30));
     e.RecomputeCanopy();e.RecomputeSeedRain();yield return null;
    }
    e.GrowthModelVersion=0;e.StormModelVersion=1;e.UseStormCalibrationForVerification(new StormCalibration(0,new[]{.02f,.06f,.18f},new[]{1f,1f,1f}));int before=e.LivingTreeCount;float severity=ExactIntensity(desired);e.ForceStormNextYear(severity,135);
    var watch=Stopwatch.StartNew();e.AdvanceOneYear();watch.Stop();var performance=e.LastStormPerformance;Check(performance.Victims==desired&&performance.CanopyRebuilds==1&&performance.SeedRebuilds==1,"Exact authored visual damage size and batched rebuilds");
    string condition="victims-"+desired;table.WriteLine(string.Join(",",condition,before,desired,severity.ToString("R",CultureInfo.InvariantCulture),watch.Elapsed.TotalMilliseconds,performance.EvaluationMilliseconds,performance.MortalityMilliseconds,performance.RebuildMilliseconds,performance.DeadwoodMilliseconds,performance.VisualMilliseconds,performance.CanopyRebuilds,performance.SeedRebuilds));table.Flush();
    ui.CloseAll();ui.CloseHelp();player.enabled=false;player.transform.position=new Vector3(0,20,-28);player.LookToward(Vector3.zero);var budget=m.GetComponentInChildren<ScenarioWindthrowVisualBudget>();budget.RefreshAt(Camera.main.transform.position);Check(budget.ActiveCrowns<=20,"Authored render crown budget");yield return Capture(condition);yield return RenderComparison(condition,m.GetComponentsInChildren<ScenarioWindthrowVisual>());
   }
  }
 }
 IEnumerator RenderComparison(string condition,ScenarioWindthrowVisual[] visuals)
 {
  Vector3 cameraPosition=Camera.main.transform.position;Quaternion cameraRotation=Camera.main.transform.rotation;
  string path=Path.Combine(dir,"render-"+condition+".csv");using(var table=new StreamWriter(path))
  {
   table.WriteLine("condition,wind_visuals_enabled,frame,wall_frame_ms,main_thread_ms,main_thread_recorder_valid,storm_visuals,normal_and_storm_deadwood_records,editor_only");
   foreach(bool enabled in new[]{true,false})
   {
    foreach(var visual in visuals)visual.gameObject.SetActive(enabled);for(int i=0;i<30;i++)yield return null;
    using(var recorder=ProfilerRecorder.StartNew(ProfilerCategory.Internal,"Main Thread",120))for(int frame=0;frame<90;frame++){yield return null;Check((Camera.main.transform.position-cameraPosition).sqrMagnitude<.000001f&&Quaternion.Angle(Camera.main.transform.rotation,cameraRotation)<.001f,"Identical fixed render camera");table.WriteLine(string.Join(",",condition,enabled,frame,(Time.unscaledDeltaTime*1000).ToString("R",CultureInfo.InvariantCulture),(recorder.Valid?recorder.LastValue/1000000.0:double.NaN).ToString("R",CultureInfo.InvariantCulture),recorder.Valid,visuals.Length,m.DeadwoodRecords.Count,true));}
   }
  }foreach(var visual in visuals)visual.gameObject.SetActive(true);
 }
}
#endif
