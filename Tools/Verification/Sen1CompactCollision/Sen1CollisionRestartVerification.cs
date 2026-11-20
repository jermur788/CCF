using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Two genuine Play Mode entries in one Editor. SessionState survives the domain reload.
[InitializeOnLoad]
public static class Sen1CollisionRestartVerification
{
    private const string StageKey = "Sen1.886.restart.stage";
    private static double entered;
    static Sen1CollisionRestartVerification()
    {
        if (Environment.GetEnvironmentVariable("SEN1_COLLISION_RESTART_OUT") == null) return;
        EditorApplication.playModeStateChanged += OnState;
        EditorApplication.update += Tick;
    }
    public static void Begin()
    {
        SessionState.SetInt(StageKey, 0);
        SessionState.SetBool(StageKey+".failed", false);
        string output=Environment.GetEnvironmentVariable("SEN1_COLLISION_RESTART_OUT");
        Directory.CreateDirectory(output); File.WriteAllText(Path.Combine(output,"verdict.txt"), "");
        EditorSceneManager.OpenScene("Assets/ForestPrototype/Scenes/ScenarioOneCompact30.unity");
        EditorApplication.isPlaying=true;
    }
    private static void OnState(PlayModeStateChange state)
    {
        if (state==PlayModeStateChange.EnteredPlayMode) entered=EditorApplication.timeSinceStartup;
        if (state!=PlayModeStateChange.EnteredEditMode) return;
        int stage=SessionState.GetInt(StageKey,-1);
        if(stage==1) EditorApplication.delayCall+=()=>EditorApplication.isPlaying=true;
        else if(stage==2)
        {
            bool failed=SessionState.GetBool(StageKey+".failed",true);
            string verdict="SEN1_COLLISION_RESTART_"+(failed?"FAIL":"PASS")+" two Play Mode entries; 175 unique live trunks each";
            File.AppendAllText(Path.Combine(Environment.GetEnvironmentVariable("SEN1_COLLISION_RESTART_OUT"),"verdict.txt"),verdict+"\n");
            Debug.Log(verdict);EditorApplication.Exit(failed?1:0);
        }
    }
    private static void Tick()
    {
        if(!EditorApplication.isPlaying || entered==0 || EditorApplication.timeSinceStartup-entered<3) return;
        entered=0;
        var trees=UnityEngine.Object.FindObjectsByType<ForestTree>(FindObjectsSortMode.None).Where(t=>t.GetComponent<Compact30AuthoredTree>()!=null).ToArray();
        bool ok=trees.Length==175 && trees.Select(t=>t.TreeId).Distinct().Count()==175 && trees.All(t=>t.IsLiving && t.MarkType==TreeMarkType.None
            && t.GetComponentsInChildren<Collider>().Count(c=>c.enabled && !c.isTrigger)==1)
            && UnityEngine.Object.FindObjectsByType<Transform>(FindObjectsSortMode.None).Count(t=>t.name.StartsWith("Compact30 stump "))==0;
        int stage=SessionState.GetInt(StageKey,0);
        string line=(ok?"PASS":"FAIL")+" Play Mode entry "+(stage+1)+": roots="+trees.Length+", one solid collider per living unmarked stem, no stale stump";
        File.AppendAllText(Path.Combine(Environment.GetEnvironmentVariable("SEN1_COLLISION_RESTART_OUT"),"verdict.txt"),line+"\n");Debug.Log(line);
        SessionState.SetBool(StageKey+".failed",SessionState.GetBool(StageKey+".failed",false)||!ok);
        SessionState.SetInt(StageKey,stage+1);EditorApplication.isPlaying=false;
    }
}
