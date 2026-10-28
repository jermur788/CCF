#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

// Actual geometry-0 / rack-model-0 compatibility, in addition to the full pre-v21 Enlarged80 guided sequence.
public static class Legacy40RackAuthoringVerification
{
    public static void Begin()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        RackUxShots.SetGameSize(1280,720); EditorApplication.isPlaying = true;
    }
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Prepare()
    {
        if (!Environment.GetCommandLineArgs().Contains("Legacy40RackAuthoringVerification.Begin")) return;
        MenuHelpView.ResetProgress(); LearningObjectivesView.ResetProgress();
    }
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Install()
    {
        if (Environment.GetCommandLineArgs().Contains("Legacy40RackAuthoringVerification.Begin"))
            new GameObject("Disposable Legacy40 rack authoring verification").AddComponent<Legacy40RackAuthoringRunner>();
    }
}
public sealed class Legacy40RackAuthoringRunner : MonoBehaviour
{
    ScenarioOneUiRoot ui; ScenarioOneManager manager;
    int checks; readonly RackAuthoringNativeInput input = new RackAuthoringNativeInput();
    void Check(bool ok, string name)
    {
        checks++; Debug.Log("LEGACY40_RACK_" + (ok ? "PASS " : "FAIL ") + name);
        if (!ok) throw new InvalidOperationException(name);
    }
    static IEnumerator Pause(int n=20) { for(int i=0;i<n;i++) yield return null; }
    IEnumerator Start()
    {
        yield return Pause(50);
        var stack=new Stack<IEnumerator>(); stack.Push(Run()); Exception error=null;
        while(stack.Count>0)
        {
            bool more; object value=null;
            try { more=stack.Peek().MoveNext(); if(more)value=stack.Peek().Current; }
            catch(Exception e){error=e;break;}
            if(!more)stack.Pop();else if(value is IEnumerator next)stack.Push(next);else yield return value;
        }
        Debug.Log(error==null ? "LEGACY40_RACK_AUTHORING_PASS checks="+checks : "LEGACY40_RACK_AUTHORING_FAIL "+error);
        EditorApplication.ExitPlaymode(); EditorApplication.Exit(error==null?0:1);
    }
    IEnumerator Run()
    {
        ui=FindFirstObjectByType<ScenarioOneUiRoot>(); manager=ui.Manager;
        Check(ui.Ecology.StandGeometryModelVersion==StandGeometryModel.Legacy40 && manager.RackPlanningModelVersion==RackPlanningModel.Legacy
            && !manager.RackPlanningV2Active,"explicit real Legacy40/model-0 fixture");
        while(ui.Help.Onboarding.IsOpen)
        {
            Button next=ui.RootElement.Q<Button>("onboarding-next");
            Check(next!=null && next.enabledInHierarchy,"real introduction Next/Begin control");
            typeof(Clickable).GetMethod("Invoke",BindingFlags.NonPublic|BindingFlags.Instance).Invoke(next.clickable,new object[]{null});
            yield return Pause();
        }
        ui.CloseAll(); UnityEngine.Cursor.lockState=CursorLockMode.Locked; UnityEngine.Cursor.visible=false;
        ForestTree target=FindObjectsByType<ForestTree>(FindObjectsSortMode.None).Where(t=>t.IsLiving && t.CanChop
            && t.transform.position.x>-17f).OrderBy(t=>Math.Abs(t.transform.position.z)).ThenBy(t=>t.transform.position.x).First();
        Vector3 p=target.transform.position;
        ui.Player.transform.position=new Vector3(p.x-3f,ui.Player.transform.position.y,p.z);
        ui.Player.LookToward(p+Vector3.up*1.2f); yield return Pause(30);
        yield return input.Send(RackAuthoringNativeInput.Op.Key("b"));
        Check(ui.Help.IsOpen && ui.RackTool.IntroPending,"legacy B retains real first-use/deferred entry");
        yield return input.Send(RackAuthoringNativeInput.Op.Key("Escape"));
        Check(ui.RackTool.Active && RackTool.AnyActive,"legacy deferred close enters first-person tool");
        yield return input.Send(RackAuthoringNativeInput.Op.Click());
        Check(ui.RackTool.HasOrigin,"legacy OS click selects origin");
        float angle=ui.RackTool.AngleDegrees;
        yield return input.Send(RackAuthoringNativeInput.Op.Move(160,0));
        Check(Math.Abs(Mathf.DeltaAngle(angle,ui.RackTool.AngleDegrees))>10f,"legacy OS pointer changes preview angle");
        yield return input.Send(RackAuthoringNativeInput.Op.Move(-160,0),RackAuthoringNativeInput.Op.Click(3));
        Check(ui.RackTool.Active && !ui.RackTool.HasOrigin,"legacy OS right-click reselects without work");
        yield return input.Send(RackAuthoringNativeInput.Op.Click(),RackAuthoringNativeInput.Op.Key("Escape"));
        Check(!ui.RackTool.Active && !RackTool.AnyActive && manager.WorkOrders.Count==0,"legacy cancellation creates no work");
        yield return input.Send(RackAuthoringNativeInput.Op.Key("b"),RackAuthoringNativeInput.Op.Click());
        Check(ui.RackTool.Proposal!=null && ui.RackTool.Proposal.CanConfirm,"legacy native preview can confirm");
        yield return input.Send(RackAuthoringNativeInput.Op.Click());
        Check(manager.RackGroups.Count==1 && ui.RackTool.ConfirmedCount==1 && manager.WorkOrders.Any(o=>o.type==ScenarioWorkType.FellTree
            && o.status==ScenarioWorkStatus.Pending && o.rackId==0),"legacy real OS confirm creates ordinary pending work");
        ui.RackTool.Exit();
        string[] ids=manager.WorkOrders.Where(o=>o.IsOpen).Select(o=>o.targetTreeId).ToArray();
        manager.ApprovePendingWork(); int year=manager.CurrentEcologicalYear;
        Check(ui.AdvanceFromWorkPlan(),"legacy existing annual UI progression remains viable");
        yield return Pause(35);
        Check(manager.CurrentEcologicalYear==year+1 && ids.All(id=>manager.WorkOrders.Any(o=>o.targetTreeId==id
            && o.status==ScenarioWorkStatus.Completed)),"legacy approved rack work actually executes once");
        Check(manager.PlannedRackCount==0 && manager.RackPlanningModelVersion==RackPlanningModel.Legacy,"legacy progression never creates V2 racks");
        yield return new WaitForEndOfFrame();
        Texture2D image=ScreenCapture.CaptureScreenshotAsTexture();
        File.WriteAllBytes(Path.Combine(Environment.GetEnvironmentVariable("CCF_DIAG_DIR"),"legacy40-after-rack-year.png"),image.EncodeToPNG());
        Destroy(image);
    }
}
#endif
