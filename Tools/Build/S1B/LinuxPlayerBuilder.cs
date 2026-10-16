using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.Build;
using UnityEngine;

// Disposable Linux Standalone (Mono) builder for the Tier-3 Scenario One Player gate.
// Mirrors the accepted Windows candidate builder's structure and checks, but is
// target-specific (StandaloneLinux64) and deliberately separate from it: the accepted
// Windows path in WindowsCandidateBuilder.cs is not modified or refactored.
//
// Invoked by Tools/Build/S1B/build_linux.py, which stages this file under
// Assets/ForestPrototype/ for the build and always removes it afterwards. Not part of
// the game or a shipped Player. No distribution/tester-package claim is made here.
public static class LinuxPlayerBuilder
{
    private static bool CrashUploadAllowed()
    {
        // Same installed-6000.6 surface the Windows builder uses: upload permission is
        // internal, and the legacy public enabled flag checks a different feature.
        var p = typeof(UnityEditor.CrashReporting.CrashReportingSettings).GetProperty("canUploadReports",
            System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
        if (p == null) throw new Exception("Installed crash-upload API changed");
        return (bool)p.GetValue(null);
    }

    public static void Build()
    {
        string output = Environment.GetEnvironmentVariable("CCF_BUILD_OUTPUT");
        string identity = Environment.GetEnvironmentVariable("CCF_BUILD_IDENTITY");
        string stamp = "Assets/ForestPrototype/UI/Resources/CCFBuildIdentity.json";
        var target = UnityEditor.Build.NamedBuildTarget.Standalone;
        var originalBackend = PlayerSettings.GetScriptingBackend(target);
        bool originalEngineDiagnostics = UnityEditor.EngineDiagnostics.EngineDiagnosticsSettings.enabled;
        try
        {
            if (!BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.Standalone, BuildTarget.StandaloneLinux64))
                throw new Exception("Linux x86-64 module unavailable");
            if (string.IsNullOrEmpty(output) || string.IsNullOrEmpty(identity) || File.Exists(stamp))
                throw new Exception("Missing output/identity or pre-existing identity asset");
            foreach (var path in LinuxReleaseContentFilter.PerformanceResources)
                if (File.Exists(path) || File.Exists(path + ".meta")) throw new Exception("Pre-existing performance resource: " + path);
            File.WriteAllText(stamp, identity); AssetDatabase.ImportAsset(stamp, ImportAssetOptions.ForceSynchronousImport);
            PlayerSettings.SetScriptingBackend(target, ScriptingImplementation.Mono2x);
            UnityEditor.EngineDiagnostics.EngineDiagnosticsSettings.enabled = false;
            if (UnityEditor.Analytics.AnalyticsSettings.enabled || UnityEditor.Analytics.AnalyticsSettings.deviceStatsEnabledInBuild || CrashUploadAllowed())
                throw new Exception("Unexpected analytics/device-stats/crash-upload setting; inspect canonical disabled settings");
            Debug.Log("S1B_PRIVACY_SETTINGS_PASS analytics=false deviceStats=false crashUpload=false");
            var scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();
            if (scenes.Length != 1 || scenes[0] != "Assets/Scenes/ForestTest.unity") throw new Exception("Unexpected build scenes; inspect configuration");
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions { scenes = scenes, target = BuildTarget.StandaloneLinux64,
                locationPathName = Path.Combine(output, "CCF.x86_64"), options = BuildOptions.DetailedBuildReport });
            File.WriteAllLines(Path.Combine(output, "included-assets.txt"), report.packedAssets.SelectMany(p => p.contents).Select(c => c.sourceAssetPath).Where(p => !string.IsNullOrEmpty(p)).Distinct().OrderBy(p => p));
            File.WriteAllText(Path.Combine(output, "build-result.txt"), $"{report.summary.result}\nErrors: {report.summary.totalErrors}\nWarnings: {report.summary.totalWarnings}\nBytes: {report.summary.totalSize}\nSeconds: {report.summary.totalTime.TotalSeconds}\n");
            if (UnityEditor.Analytics.AnalyticsSettings.deviceStatsEnabledInBuild || CrashUploadAllowed()) throw new Exception("Unexpected upload setting after build");
            if (report.summary.result != BuildResult.Succeeded || report.summary.totalErrors != 0) throw new Exception("Linux Player build failed");
            foreach (var file in Directory.GetFiles(Path.Combine(output, "CCF_Data/Managed"), "*.dll"))
            {
                var references = System.Reflection.Assembly.ReflectionOnlyLoadFrom(file).GetReferencedAssemblies();
                if (references.Any(a => LinuxReleaseContentFilter.TestAssemblies.Contains(a.Name + ".dll")))
                    throw new Exception("Runtime dependency on excluded test assembly: " + file);
            }
            Debug.Log("S1B_RUNTIME_DEPENDENCY_AUDIT_PASS excluded test assembly references=0");
            Debug.Log("S1B_LINUX_BUILD_PASS " + identity);
        }
        catch (Exception e) { Debug.LogException(e); EditorApplication.Exit(1); return; }
        finally
        {
            if (File.Exists(stamp)) AssetDatabase.DeleteAsset(stamp);
            PlayerSettings.SetScriptingBackend(target, originalBackend);
            UnityEditor.EngineDiagnostics.EngineDiagnosticsSettings.enabled = originalEngineDiagnostics;
        }
        EditorApplication.Exit(0);
    }
}

// Only this disposable build helper changes candidate inclusion; installed packages and
// their asmdefs remain intact for the accepted Editor regression fixtures. Same exclusion
// contract as the Windows builder (S1BReleaseContentFilter), defined here under a distinct
// name so the two builders never collide if a future task ever stages both.
public sealed class LinuxReleaseContentFilter : IPreprocessBuildWithReport, IFilterBuildAssemblies
{
    public int callbackOrder => int.MaxValue;
    public static readonly string[] PerformanceResources = {
        "Assets/Resources/PerformanceTestRunInfo.json", "Assets/Resources/PerformanceTestRunSettings.json" };
    public static readonly string[] TestAssemblies = {
        "Unity.Collections.Tests.CoreCLR.InternalJobNestedPrivate.dll",
        "Unity.Collections.Tests.CoreCLR.PrivateJobNested.dll",
        "Unity.Collections.Tests.CoreCLR.ProtectedJobNested.dll",
        "Unity.Collections.Tests.CoreCLR.PublicJobPrivateGeneric.dll" };
    public void OnPreprocessBuild(BuildReport report)
    {
        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("CCF_BUILD_OUTPUT"))) return;
        foreach (string path in PerformanceResources)
            if (File.Exists(path)) AssetDatabase.DeleteAsset(path);
        Debug.Log("S1B_GENERATED_TEST_RESOURCES_EXCLUDED");
    }
    public string[] OnFilterAssemblies(BuildOptions options, string[] assemblies)
    {
        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("CCF_BUILD_OUTPUT"))) return assemblies;
        return assemblies.Where(path => !TestAssemblies.Contains(Path.GetFileName(path))).ToArray();
    }
}
