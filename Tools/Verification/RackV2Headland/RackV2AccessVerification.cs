using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
public static class RackV2AccessVerification
{
#if UNITY_EDITOR
    public static void Begin() { EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity"); EditorApplication.isPlaying = true; }
#endif
    public static void Size(int width,int height)
    {
#if UNITY_EDITOR
        var assembly=typeof(Editor).Assembly; Type sizesType=assembly.GetType("UnityEditor.GameViewSizes");
        Type singleton=typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
        object sizes=singleton.GetProperty("instance",BindingFlags.Public|BindingFlags.Static).GetValue(null);
        MethodInfo getGroup=sizesType.GetMethod("GetGroup"); object group=getGroup.Invoke(sizes,new[]{Enum.ToObject(getGroup.GetParameters()[0].ParameterType,0)});
        Type sizeType=assembly.GetType("UnityEditor.GameViewSize"),kind=assembly.GetType("UnityEditor.GameViewSizeType");
        object size=Activator.CreateInstance(sizeType,BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance,null,new object[]{Enum.ToObject(kind,1),width,height,"V2 junction "+width},null);
        int index=(int)group.GetType().GetMethod("GetTotalCount").Invoke(group,null);group.GetType().GetMethod("AddCustomSize").Invoke(group,new[]{size});
        Type viewType=assembly.GetType("UnityEditor.GameView"); EditorWindow view=EditorWindow.GetWindow(viewType);
        viewType.GetProperty("selectedSizeIndex",BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance).SetValue(view,index);view.Show();view.Focus();view.Repaint();
#endif
    }
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install() { if (Environment.GetCommandLineArgs().Any(a => a == "RackV2AccessVerification.Begin")) new GameObject("Disposable V2 access gate").AddComponent<RackV2AccessVerificationRunner>(); }
}
public sealed class RackV2AccessVerificationRunner : MonoBehaviour
{
    private int checks;
    private void Check(bool ok, string message) { if (!ok) throw new InvalidOperationException(message); checks++; Debug.Log("HEADLAND_ACCESS_CHECK " + message); }
    private IEnumerator Start()
    {
        for(int i=0;i<5;i++) yield return null;
        Exception failure=null; var stack=new Stack<IEnumerator>(); stack.Push(Run());
        while(stack.Count>0){bool more;object current=null;try{more=stack.Peek().MoveNext();if(more)current=stack.Peek().Current;}catch(Exception e){failure=e;break;}if(!more){stack.Pop();continue;}if(current is IEnumerator nested)stack.Push(nested);else yield return current;}
        Debug.Log(failure==null?"HEADLAND_ACCESS_PASS checks="+checks:"HEADLAND_ACCESS_FAIL checks="+checks+" "+failure);
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode(); EditorApplication.Exit(failure==null?0:1);
#endif
    }
    private IEnumerator Capture(string name)
    {
        yield return new WaitForEndOfFrame(); Texture2D image=ScreenCapture.CaptureScreenshotAsTexture();
        File.WriteAllBytes(Path.Combine(Environment.GetEnvironmentVariable("CCF_ACCEPTANCE_OUTPUT"),name+".png"),image.EncodeToPNG()); Destroy(image);
    }
    private IEnumerator Run()
    {
        var p=FindFirstObjectByType<ForestPlayer>();var m=FindFirstObjectByType<ScenarioOneManager>();var ui=FindFirstObjectByType<ScenarioOneUiRoot>();
        Check(p!=null&&m!=null&&ui!=null&&m.RackPlanningV2Active,"fresh V2 gameplay ready");
        for(int i=0;i<15;i++){ui.CloseHelp();yield return null;}
        Cursor.lockState=CursorLockMode.Locked; Cursor.visible=false;
        var keyboard=Keyboard.current??InputSystem.AddDevice<Keyboard>(); float y=p.transform.position.y;
        Vector3[] starts={new Vector3(-5f,y,-27.4f),new Vector3(-35.4f,y,-25f)};
        Vector3[] targets={new Vector3(-20f,1.3f,-27.4f),new Vector3(-35.4f,1.3f,-10f)};
        for(int leg=0;leg<2;leg++)
        {
            p.transform.position=starts[leg]; Physics.SyncTransforms(); p.LookToward(targets[leg]);
            for(int i=0;i<5;i++)yield return null;
            Vector3 before=p.transform.position;
            InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.W)); yield return new WaitForSeconds(1f);
            InputSystem.QueueStateEvent(keyboard,new KeyboardState()); yield return null;
            Vector3 after=p.transform.position; float moved=Vector2.Distance(new Vector2(before.x,before.z),new Vector2(after.x,after.z));
            Check(moved>1f,"keyboard traversal "+(leg==0?"feeder":"headland")+" moved="+moved.ToString("F2"));
        }
        string[] ids={"PO0203","PO1204","PO2003","PO2803","PO3803"};float[] zs={-35.403477f,-16.015210f,0.014705888f,16.022398f,35.436646f};
        var trees=FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude,FindObjectsSortMode.None);
        for(int j=0;j<ids.Length;j++)
        {
            // ForestPlayer.interactionDistance is 3.5 m. The headland is 2.6 m wide (x -36.7..-34.1), so try the centreline first, then positions
            // inside that width nearer the stand; a standing point anywhere on the headland counts as "from the headland".
            ForestTree t=trees.Single(x=>x.TreeId==ids[j]); bool visible=false; string where="";
            foreach(float standX in new float[]{-35.4f,-34.8f,-34.5f})
            {
                foreach(float offset in new float[]{0f,-0.5f,0.5f,-1f,1f})
                {
                    p.transform.position=new Vector3(standX,y,zs[j]+offset); Physics.SyncTransforms();
                    p.LookToward(t.transform.position+Vector3.up*1.3f);
                    for(int frame=0;frame<5;frame++)yield return null;
                    if(p.AimedTree==t){visible=true;where=" stand=("+standX.ToString("F1")+","+(zs[j]+offset).ToString("F2")+") dist="+Vector2.Distance(new Vector2(standX,zs[j]+offset),new Vector2(t.transform.position.x,t.transform.position.z)).ToString("F2");break;}
                }
                if(visible)break;
            }
            Check(visible,"first-person aim from headland to entrance tree "+ids[j]+where);
        }
        for(int pass=0;pass<2;pass++)
        {
            int width=pass==0?1280:1920,height=pass==0?720:1080;RackV2AccessVerification.Size(width,height);
            for(int frame=0;frame<20;frame++){ui.CloseHelp();yield return null;}
            p.transform.position=new Vector3(-2.436166f,y,-23.5f);p.LookToward(new Vector3(-2.436166f,1.1f,-27.4f));
            for(int frame=0;frame<10;frame++)yield return null;yield return Capture("path-feeder-junction-"+width);
            p.transform.position=new Vector3(-30f,y,-27.4f);p.LookToward(new Vector3(-35.4f,1.1f,-27.4f));
            for(int frame=0;frame<10;frame++)yield return null;yield return Capture("feeder-headland-junction-"+width);
        }
    }
}
