#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.SceneManagement;
public class Ccf393 : MonoBehaviour {
static string Out => Environment.GetEnvironmentVariable("CCF_DIAG_DIR");
public static void Begin() {
var cfg=Environment.GetEnvironmentVariable("XDG_CONFIG_HOME");
if(string.IsNullOrEmpty(cfg)||!Application.persistentDataPath.StartsWith(cfg+"/")) { EditorApplication.Exit(3); return; }
File.WriteAllText(Path.Combine(Out,"paths.txt"),Application.persistentDataPath+"\nHOME="+Environment.GetEnvironmentVariable("HOME"));
EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
ScenarioOneTeachingCopyVerification.SetGameSize(1280,720); EditorApplication.isPlaying=true;
}
[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)] static void Install(){if(Environment.GetCommandLineArgs().Contains("Ccf393.Begin"))new GameObject("Disposable393").AddComponent<Ccf393>();}
static IEnumerator Settle(){yield return new WaitForSecondsRealtime(.4f);for(int i=0;i<12;i++)yield return null;}
IEnumerator Start(){yield return new WaitForSecondsRealtime(8);
var ui=FindFirstObjectByType<ScenarioOneUiRoot>(); var saves=FindFirstObjectByType<ForestSaveController>();
if(ui.Help.Onboarding.IsOpen){ui.Help.Onboarding.Next();ui.Help.Onboarding.Next();ui.Help.Onboarding.Next();}
ui.CloseAll();ui.CloseHelp();
string fixture=Environment.GetEnvironmentVariable("CCF393_FIXTURE");
if(!File.Exists(fixture)) File.WriteAllText(fixture,JsonUtility.ToJson(saves.CaptureData(),true));
var data=JsonUtility.FromJson<ForestSaveData>(File.ReadAllText(fixture));
Debug.Log("C393_HASH saved="+ScenarioReferenceArchive.CurrentWorldHash(data)+" year="+data.ecologicalYear+" version="+data.version);
foreach(var mile in ScenarioReferenceArchive.Load().milestones) Debug.Log("C393_REFERENCE year="+mile.year+" current="+ScenarioReferenceArchive.CurrentWorldHash(mile.world)+" world="+ScenarioReferenceArchive.WorldHash(mile.world));
var manager=ui.Manager; long cash=manager.CashCents;
var cashField=typeof(ScenarioOneManager).GetField("cashCents",BindingFlags.NonPublic|BindingFlags.Instance);
cashField.SetValue(manager,manager.Definition.MinimumHarvestJobCents+100L);
if(!manager.TryPurchaseStock(manager.Definition.FindShopEntry("sessile-oak-sapling").itemId,25)) {Debug.LogError("C393_PURCHASE_FAIL");EditorApplication.Exit(5);yield break;}
cashField.SetValue(manager,cash);
ui.ShowWorkPlan(); yield return Settle();ui.CloseHelp(); yield return Settle();
var wp=(WorkPlanView)typeof(ScenarioOneUiRoot).GetField("workPlan",BindingFlags.NonPublic|BindingFlags.Instance).GetValue(ui);
typeof(ScenarioOneManager).GetField("cashCents",BindingFlags.NonPublic|BindingFlags.Instance).SetValue(ui.Manager,ui.Manager.Definition.MinimumHarvestJobCents+100L);wp.Refresh(true);
var plan=wp.Root; ScrollView scroll=plan.Q<ScrollView>(); yield return Settle();
var warning=plan.Q("nursery-warning-sessile-oak-sapling");if(warning!=null) scroll.ScrollTo(warning);yield return Settle();
yield return new WaitForEndOfFrame(); var image=ScreenCapture.CaptureScreenshotAsTexture();int positive=image.GetPixels32().Count(p=>p.r>8||p.g>8||p.b>8);File.WriteAllBytes(Path.Combine(Out,"nursery.png"),image.EncodeToPNG());
Debug.Log("C393_SANITY positive="+positive+" total="+image.width*image.height+" size="+Screen.width+"x"+Screen.height);
if(positive<image.width*image.height/100){Debug.LogError("C393_BLACK");EditorApplication.Exit(4);yield break;}
bool visible=warning!=null && warning.worldBound.Overlaps(scroll.contentViewport.worldBound);
Debug.Log("C393_NURSERY nursery warning visible="+visible+" warning="+(warning==null?"MISSING":warning.worldBound.ToString())+" viewport="+scroll.contentViewport.worldBound+" display="+(warning==null?"MISSING":warning.resolvedStyle.display.ToString()));
Debug.Log("C393_DONE");EditorApplication.Exit(0);
}
}
#endif
