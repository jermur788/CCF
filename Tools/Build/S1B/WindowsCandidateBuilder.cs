using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class WindowsCandidateBuilder
{
    public static void Build()
    {
        string output = Environment.GetEnvironmentVariable("CCF_BUILD_OUTPUT");
        string identity = Environment.GetEnvironmentVariable("CCF_BUILD_IDENTITY");
        string stamp = "Assets/ForestPrototype/UI/Resources/CCFBuildIdentity.json";
        var target = UnityEditor.Build.NamedBuildTarget.Standalone;
        var originalBackend = PlayerSettings.GetScriptingBackend(target);
        try
        {
            if (!BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64)) throw new Exception("Windows x86-64 module unavailable");
            if (string.IsNullOrEmpty(output) || string.IsNullOrEmpty(identity) || File.Exists(stamp)) throw new Exception("Missing output/identity or pre-existing identity asset");
            File.WriteAllText(stamp, identity); AssetDatabase.ImportAsset(stamp, ImportAssetOptions.ForceSynchronousImport);
            PlayerSettings.SetScriptingBackend(target, ScriptingImplementation.Mono2x);
            var scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();
            if (scenes.Length != 1 || scenes[0] != "Assets/Scenes/ForestTest.unity") throw new Exception("Unexpected build scenes; inspect configuration");
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions { scenes = scenes, target = BuildTarget.StandaloneWindows64,
                locationPathName = Path.Combine(output, "CCF.exe"), options = BuildOptions.DetailedBuildReport });
            File.WriteAllLines(Path.Combine(output, "included-assets.txt"), report.packedAssets.SelectMany(p => p.contents).Select(c => c.sourceAssetPath).Where(p => !string.IsNullOrEmpty(p)).Distinct().OrderBy(p => p));
            File.WriteAllText(Path.Combine(output, "build-result.txt"), $"{report.summary.result}\nErrors: {report.summary.totalErrors}\nWarnings: {report.summary.totalWarnings}\nBytes: {report.summary.totalSize}\nSeconds: {report.summary.totalTime.TotalSeconds}\n");
            if (report.summary.result != BuildResult.Succeeded || report.summary.totalErrors != 0) throw new Exception("Windows candidate build failed");
            Debug.Log("S1B_WINDOWS_BUILD_PASS " + identity);
        }
        catch (Exception e) { Debug.LogException(e); EditorApplication.Exit(1); return; }
        finally
        {
            if (File.Exists(stamp)) AssetDatabase.DeleteAsset(stamp);
            PlayerSettings.SetScriptingBackend(target, originalBackend);
        }
        EditorApplication.Exit(0);
    }
}
