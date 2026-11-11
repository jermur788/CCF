#if UNITY_EDITOR
using UnityEditor; using UnityEngine; using System; using System.IO; using System.Reflection;
[InitializeOnLoad] public static class PathGuard381 {
static PathGuard381() { var c=Environment.GetEnvironmentVariable("XDG_CONFIG_HOME"); if(string.IsNullOrEmpty(c)||!c.Contains("CCF-381-Readiness-20261111/continuation/")) { EditorApplication.Exit(2); throw new Exception("Isolation absent"); }
var p=Application.persistentDataPath; if(!p.StartsWith(c+"/")) {EditorApplication.Exit(2);throw new Exception("Persistent path unsafe: "+p);}
var s=typeof(ForestSaveController).GetProperty("SavePath",BindingFlags.NonPublic|BindingFlags.Public|BindingFlags.Static|BindingFlags.Instance);
File.WriteAllText(Path.Combine(Environment.GetEnvironmentVariable("CCF_DIAG_DIR"),"process-paths.txt"),"persistent="+p+"\nconfig="+c+"\nF5/F9="+s.GetValue(null));
PlayerPrefs.SetInt("CCF381Isolation",381);PlayerPrefs.Save(); if(!File.Exists(Path.Combine(p,"prefs"))) {EditorApplication.Exit(2);throw new Exception("Prefs absent");}
} }
#endif
