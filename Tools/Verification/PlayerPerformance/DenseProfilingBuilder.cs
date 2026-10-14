using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

// Disposable Editor helper: builds the INTERNAL Linux Development Player used for the dense-stand performance baseline.
// Staged into Assets/ForestPrototype/Editor/ by run_dense_build.py and removed afterwards. It keeps the S1-B privacy policy (analytics,
// device statistics and crash upload must stay off) and the S1-B content filter (test-only assemblies are not shipped).
public static class DenseProfilingBuilder
{
    private static bool CrashUploadAllowed()
    {
        var p = typeof(UnityEditor.CrashReporting.CrashReportingSettings).GetProperty("canUploadReports",
            System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
        if (p == null) throw new Exception("Installed crash-upload API changed");
        return (bool)p.GetValue(null);
    }

    public static void Build()
    {
        string output = Environment.GetEnvironmentVariable("CCF_BUILD_OUTPUT");
        var target = NamedBuildTarget.Standalone;
        var originalBackend = PlayerSettings.GetScriptingBackend(target);
        bool originalEngineDiagnostics = UnityEditor.EngineDiagnostics.EngineDiagnosticsSettings.enabled;
        try
        {
            if (!BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.Standalone, BuildTarget.StandaloneLinux64)) throw new Exception("Linux x86-64 module unavailable");
            if (string.IsNullOrEmpty(output)) throw new Exception("CCF_BUILD_OUTPUT missing");
            foreach (string path in DenseProfilingContentFilter.PerformanceResources)
                if (File.Exists(path) || File.Exists(path + ".meta")) throw new Exception("Pre-existing performance resource: " + path);
            PlayerSettings.SetScriptingBackend(target, ScriptingImplementation.Mono2x);
            UnityEditor.EngineDiagnostics.EngineDiagnosticsSettings.enabled = false;
            if (UnityEditor.Analytics.AnalyticsSettings.enabled || UnityEditor.Analytics.AnalyticsSettings.deviceStatsEnabledInBuild || CrashUploadAllowed())
                throw new Exception("Unexpected analytics/device-stats/crash-upload setting");
            Debug.Log("S1B_PRIVACY_SETTINGS_PASS analytics=false deviceStats=false crashUpload=false");
            string[] scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();
            if (scenes.Length != 1 || scenes[0] != "Assets/Scenes/ForestTest.unity") throw new Exception("Unexpected build scenes");
            BuildReport report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = scenes, target = BuildTarget.StandaloneLinux64, locationPathName = Path.Combine(output, "CCF.x86_64"),
                options = BuildOptions.Development | BuildOptions.DetailedBuildReport
            });
            File.WriteAllText(Path.Combine(output, "build-result.txt"), $"{report.summary.result}\nErrors: {report.summary.totalErrors}\nWarnings: {report.summary.totalWarnings}\nBytes: {report.summary.totalSize}\nSeconds: {report.summary.totalTime.TotalSeconds}\n");
            if (UnityEditor.Analytics.AnalyticsSettings.deviceStatsEnabledInBuild || CrashUploadAllowed()) throw new Exception("Unexpected upload setting after build");
            if (report.summary.result != BuildResult.Succeeded || report.summary.totalErrors != 0) throw new Exception("Linux Development build failed");
            Debug.Log("DENSE_PERF_DEVELOPMENT_BUILD_PASS");
        }
        catch (Exception e) { Debug.LogException(e); EditorApplication.Exit(1); return; }
        finally
        {
            PlayerSettings.SetScriptingBackend(target, originalBackend);
            UnityEditor.EngineDiagnostics.EngineDiagnosticsSettings.enabled = originalEngineDiagnostics;
        }
        EditorApplication.Exit(0);
    }
}

public sealed class DenseProfilingContentFilter : IPreprocessBuildWithReport, IFilterBuildAssemblies
{
    public int callbackOrder => int.MaxValue;
    public static readonly string[] PerformanceResources = { "Assets/Resources/PerformanceTestRunInfo.json", "Assets/Resources/PerformanceTestRunSettings.json" };
    public static readonly string[] TestAssemblies =
    {
        "Unity.Collections.Tests.CoreCLR.InternalJobNestedPrivate.dll", "Unity.Collections.Tests.CoreCLR.PrivateJobNested.dll",
        "Unity.Collections.Tests.CoreCLR.ProtectedJobNested.dll", "Unity.Collections.Tests.CoreCLR.PublicJobPrivateGeneric.dll"
    };
    public void OnPreprocessBuild(BuildReport report)
    {
        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("CCF_BUILD_OUTPUT"))) return;
        foreach (string path in PerformanceResources) if (File.Exists(path)) AssetDatabase.DeleteAsset(path);
    }
    public string[] OnFilterAssemblies(BuildOptions options, string[] assemblies)
    {
        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("CCF_BUILD_OUTPUT"))) return assemblies;
        return assemblies.Where(path => !TestAssemblies.Contains(Path.GetFileName(path))).ToArray();
    }
}
