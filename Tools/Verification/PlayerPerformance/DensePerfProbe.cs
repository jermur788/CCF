using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Profiling;
using UnityEngine.Rendering;
using Debug = UnityEngine.Debug;

// Disposable measurement probe for the INTERNAL Development Player only (dense-stand performance baseline).
//
// Inactive unless the Player is started with   --ccf-perf-out <dir> --ccf-perf-plan <plan.json>
// It never changes simulation rules, saves, quality assets or the frame cap. A plan is a list of steps (see Step) so views, mark counts and
// render A/B toggles can change without rebuilding. Render toggles act on the loaded URP asset / QualitySettings at runtime and are reverted
// by the "reset" step; nothing is written to ProjectSettings or to any asset.
//
// The probe sets Application.runInBackground so a desktop notification cannot pause the measurement; this is measurement support only.
public sealed class DensePerfProbe : MonoBehaviour
{
    [Serializable] public class Step { public string op, name, arg; public float seconds, f; public int n; }
    [Serializable] public class PlanFile { public Step[] steps; }
    [Serializable] public class Series { public string name; public double mean, median, p95, p99, max, min; }
    [Serializable] public class Hitch { public int frame; public double ms, atSeconds, gcAllocBytes; public int gc0Delta; }
    [Serializable]
    public class Window
    {
        public string name, view, note, toggles;
        public int frames, marks, gc0Delta;
        public double seconds, meanMs, medianMs, p95Ms, p99Ms, maxMs, minMs, focusedFraction;
        public Series[] series;
        public Hitch[] hitches;
    }
    [Serializable] public class Timing { public string group, name; public int n; public double meanMs, medianMs, p95Ms, minMs, maxMs; public long monoBytesPerCall; public double[] samplesMs; public string note; }
    [Serializable] public class MemoryRow { public string phase; public long workingSet, privateBytes, monoUsed, unityAllocated, unityReserved, graphicsDriver, gcTotal; public int gc0; }
    [Serializable]
    public class Report
    {
        public string status, unity, os, cpu, gpu, graphicsApi, quality, pipeline, failure, markerNotes;
        public int width, height, vSync, targetFrameRate, cpuCount, systemMemoryMb, treeCount, cells, geometry, growth, rng, regeneration, storm;
        public bool developmentBuild, runInBackground;
        public float lodBias, shadowDistance;
        public string[] availableTimingNotes;
        public List<Window> windows = new List<Window>();
        public List<Timing> timings = new List<Timing>();
        public List<MemoryRow> memory = new List<MemoryRow>();
    }

    private sealed class Marker { public string category, name; public double scale; public ProfilerRecorder recorder; public List<double> values = new List<double>(); }

    private static readonly Stopwatch Startup = Stopwatch.StartNew();
    private readonly Report report = new Report();
    private string output;
    private PlanFile plan;
    private readonly List<Marker> markers = new List<Marker>();
    private readonly FrameTiming[] timing = new FrameTiming[1];
    private StandaloneSessionMenu menu;
    private ScenarioOneManager manager;
    private ForestEcologyController ecology;
    private ForestSaveController saves;
    private ScenarioOneUiRoot ui;
    private ForestPlayer player;
    private ForestTreeMarkingManager marking;
    private Vector3 spawnPosition;
    private Quaternion spawnRotation;
    private float spawnPitch;
    private string currentView = "spawn", currentToggles = "";
    private readonly List<string> openedBy = new List<string>();
    // Original render values, restored by "reset".
    private readonly Dictionary<string, object> originals = new Dictionary<string, object>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Install()
    {
#if !UNITY_EDITOR
        string[] args = Environment.GetCommandLineArgs();
        if (!args.Contains("--ccf-perf-out")) return;
        var host = new GameObject("Internal performance probe");
        DontDestroyOnLoad(host);
        host.AddComponent<DensePerfProbe>();
#endif
    }

    private static string Arg(string name)
    {
        string[] args = Environment.GetCommandLineArgs();
        int i = Array.IndexOf(args, name);
        return i >= 0 && i + 1 < args.Length ? args[i + 1] : null;
    }

    private IEnumerator Start()
    {
        Application.runInBackground = true;
        output = Arg("--ccf-perf-out");
        Directory.CreateDirectory(output);
        plan = JsonUtility.FromJson<PlanFile>(File.ReadAllText(Arg("--ccf-perf-plan")));
        Exception failure = null;
        var stack = new Stack<IEnumerator>();
        stack.Push(RunPlan());
        while (stack.Count > 0)
        {
            object next = null; bool more = false;
            try { more = stack.Peek().MoveNext(); if (more) next = stack.Peek().Current; }
            catch (Exception e) { failure = e; break; }
            if (!more) { stack.Pop(); continue; }
            if (next is IEnumerator nested) stack.Push(nested); else yield return next;
        }
        foreach (Marker m in markers) if (m.recorder.Valid) m.recorder.Dispose();
        report.status = failure == null ? "PASS" : "FAIL";
        if (failure != null) { report.failure = failure.ToString(); Debug.LogError("CCF_PERF_FAIL " + failure); }
        else Debug.Log("CCF_PERF_PASS");
        File.WriteAllText(Path.Combine(output, "report.json"), JsonUtility.ToJson(report, true));
        Application.Quit(failure == null ? 0 : 1);
    }

    private static void Check(bool ok, string what) { if (!ok) throw new InvalidOperationException(what); }

    // ---------- plan interpreter ----------

    private IEnumerator RunPlan()
    {
        report.unity = Application.unityVersion; report.os = SystemInfo.operatingSystem; report.cpu = SystemInfo.processorType;
        report.cpuCount = SystemInfo.processorCount; report.systemMemoryMb = SystemInfo.systemMemorySize;
        report.gpu = SystemInfo.graphicsDeviceName; report.graphicsApi = SystemInfo.graphicsDeviceType.ToString();
        report.developmentBuild = Debug.isDebugBuild; report.runInBackground = Application.runInBackground;
        for (int i = 0; i < plan.steps.Length; i++)
        {
            Step step = plan.steps[i];
            Debug.Log($"CCF_PERF_STEP {i} {step.op} {step.name} {step.arg}");
            switch (step.op)
            {
                case "discover": Discover(); break;
                case "markers": StartMarkers(step.arg); break;
                case "start_new": yield return StartNew(); break;
                case "continue_load": yield return ContinueLoad(); break;
                case "settle": yield return new WaitForSecondsRealtime(step.seconds); break;
                case "view": SetView(step.arg); yield return null; yield return null; break;
                case "window": yield return Window(step.name, step.seconds, false); break;
                case "walk": yield return Window(step.name, step.seconds, true); break;
                case "marks": PlaceMarks(step.n, step.arg); yield return null; yield return null; break;
                case "time_marking": yield return TimeMarking(step.name, step.n); break;
                case "toggle":
                    try { Toggle(step.arg, step.f); }
                    catch (Exception e) { report.markerNotes += $"; toggle {step.arg} failed: {e.Message}"; Debug.LogWarning("CCF_PERF_TOGGLE_FAILED " + step.arg + " " + e); }
                    yield return null; yield return null; break;
                case "reset": ResetToggles(); yield return null; yield return null; break;
                case "advance_year": yield return TimeAdvance(step.name, step.n, step.arg == "restore"); break;
                case "save_load": yield return TimeSaveLoad(step.name, step.n); break;
                case "restart": yield return TimeRestart(step.name); break;
                case "memory": Memory(step.name); break;
                case "capture": yield return Capture(step.name); break;
                case "profiler": yield return ProfilerCapture(step.name, step.seconds); break;
                case "hud": SetHud(step.arg == "on"); yield return null; yield return null; break;
                default: throw new InvalidOperationException("unknown op " + step.op);
            }
            File.WriteAllText(Path.Combine(output, "report-in-progress.json"), JsonUtility.ToJson(report, true));
        }
    }

    // ---------- discovery and recorders ----------

    private void Discover()
    {
        var handles = new List<ProfilerRecorderHandle>();
        ProfilerRecorderHandle.GetAvailable(handles);
        var lines = new List<string>();
        foreach (ProfilerRecorderHandle h in handles)
        {
            ProfilerRecorderDescription d = ProfilerRecorderHandle.GetDescription(h);
            lines.Add($"{d.Category.Name}|{d.Name}|{d.UnitType}|{d.DataType}");
        }
        lines.Sort(StringComparer.Ordinal);
        File.WriteAllLines(Path.Combine(output, "available-markers.txt"), lines);
        Debug.Log("CCF_PERF_DISCOVERED " + lines.Count);
    }

    // "category|name|scale" per line; scale multiplies LastValue (1e-6 turns nanoseconds into milliseconds).
    private void StartMarkers(string path)
    {
        foreach (Marker m in markers) if (m.recorder.Valid) m.recorder.Dispose();
        markers.Clear();
        var notes = new List<string>();
        foreach (string raw in File.ReadAllLines(path))
        {
            string line = raw.Trim();
            if (line.Length == 0 || line.StartsWith("#")) continue;
            string[] parts = line.Split('|');
            var marker = new Marker { category = parts[0], name = parts[1], scale = parts.Length > 2 ? double.Parse(parts[2], System.Globalization.CultureInfo.InvariantCulture) : 1.0 };
            marker.recorder = ProfilerRecorder.StartNew(new ProfilerCategory(marker.category), marker.name, 1, ProfilerRecorderOptions.Default | ProfilerRecorderOptions.SumAllSamplesInFrame);
            if (!marker.recorder.Valid) notes.Add("INVALID " + line);
            markers.Add(marker);
        }
        report.markerNotes = string.Join("; ", notes);
    }

    // ---------- world ----------

    private void FindSystems()
    {
        ecology = FindFirstObjectByType<ForestEcologyController>(); manager = FindFirstObjectByType<ScenarioOneManager>();
        saves = FindFirstObjectByType<ForestSaveController>(); ui = FindFirstObjectByType<ScenarioOneUiRoot>();
        player = FindFirstObjectByType<ForestPlayer>(); marking = FindFirstObjectByType<ForestTreeMarkingManager>();
    }

    private IEnumerator StartNew()
    {
        for (int frame = 0; frame < 1800; frame++)
        {
            menu = FindFirstObjectByType<StandaloneSessionMenu>();
            if (menu != null && StandaloneSessionMenu.IsOpen && menu.StartupScreenActive) break;
            yield return null;
        }
        Check(menu != null && menu.StartupScreenActive, "usable startup/title");
        report.width = Screen.width; report.height = Screen.height; report.vSync = QualitySettings.vSyncCount; report.targetFrameRate = Application.targetFrameRate;
        report.quality = QualitySettings.names[QualitySettings.GetQualityLevel()]; report.lodBias = QualitySettings.lodBias;
        report.pipeline = GraphicsSettings.currentRenderPipeline != null ? GraphicsSettings.currentRenderPipeline.name : "built-in";
        FindSystems();
        report.geometry = ecology.StandGeometryModelVersion; report.cells = ecology.CellCount; report.growth = ecology.GrowthModelVersion; report.rng = ecology.RngModelVersion;
        report.regeneration = ecology.RegenerationModelVersion; report.storm = ecology.StormModelVersion;
        report.treeCount = FindObjectsByType<ForestTree>(FindObjectsSortMode.None).Count(t => t.IsLiving);
        report.timings.Add(new Timing { group = "startup", name = "process_start_to_usable_title", n = 1, meanMs = Startup.Elapsed.TotalMilliseconds, medianMs = Startup.Elapsed.TotalMilliseconds, note = "includes initial world construction" });
        Memory("usable_title");
        menu.StartNewScenario();
        yield return null; yield return null;
        ui.CloseHelp(); ui.CloseAll();
        yield return null;
        Check(player.enabled && !StandaloneSessionMenu.IsOpen, "forest control enabled");
        spawnPosition = player.transform.position; spawnRotation = player.transform.rotation; spawnPitch = player.LookPitch;
        report.shadowDistance = ShadowDistance();
        Memory("after_start_new");
    }

    private IEnumerator ContinueLoad()
    {
        for (int frame = 0; frame < 1800; frame++)
        {
            menu = FindFirstObjectByType<StandaloneSessionMenu>();
            if (menu != null && StandaloneSessionMenu.IsOpen && menu.StartupScreenActive) break;
            yield return null;
        }
        FindSystems();
        Check(menu.ContinueAvailable, "same-build save available for Continue");
        var clock = Stopwatch.StartNew();
        menu.ContinueSavedForest();
        clock.Stop();
        report.timings.Add(new Timing { group = "load", name = "continue_from_title_call", n = 1, meanMs = clock.Elapsed.TotalMilliseconds, medianMs = clock.Elapsed.TotalMilliseconds });
        yield return null; yield return null;
        ui.CloseHelp(); ui.CloseAll();
        Memory("after_continue");
    }

    // A fixed set of named views so every condition is compared from the same camera.
    private void SetView(string view)
    {
        currentView = view;
        Vector3 position = spawnPosition; float yaw = spawnRotation.eulerAngles.y;
        if (view.StartsWith("spawn") && view.Length > 5) yaw += float.Parse(view.Substring(5), System.Globalization.CultureInfo.InvariantCulture);
        else if (view.StartsWith("centre") || view.StartsWith("edge"))
        {
            ForestTree[] trees = FindObjectsByType<ForestTree>(FindObjectsSortMode.None).Where(t => t.IsLiving).ToArray();
            Vector3 centroid = trees.Aggregate(Vector3.zero, (a, t) => a + t.transform.position) / Mathf.Max(1, trees.Length);
            position = view.StartsWith("centre") ? centroid : trees.OrderBy(t => t.transform.position.x).First().transform.position + new Vector3(2f, 0f, 0f);
            string tail = view.StartsWith("centre") ? view.Substring(6) : view.Substring(4);
            yaw = tail.Length > 0 ? float.Parse(tail, System.Globalization.CultureInfo.InvariantCulture) : 0f;
        }
        else if (view != "spawn") throw new InvalidOperationException("unknown view " + view);
        if (Physics.Raycast(position + Vector3.up * 100f, Vector3.down, out RaycastHit hit, 300f, ~0, QueryTriggerInteraction.Ignore)) position.y = hit.point.y + 0.05f;
        var controller = player.GetComponent<CharacterController>();
        if (controller != null) controller.enabled = false;
        player.transform.position = position;
        if (controller != null) controller.enabled = true;
        player.RestoreLook(Quaternion.Euler(0f, yaw, 0f), spawnPitch);
    }

    private void PlaceMarks(int count, string kind)
    {
        foreach (ForestTree t in FindObjectsByType<ForestTree>(FindObjectsSortMode.None))
        {
            if (t.IsMarkedForFell) marking.Mark(t, TreeMarkType.Fell, false);
            if (t.IsCropTree) marking.Mark(t, TreeMarkType.CropTree, false);
        }
        Vector3 here = player.transform.position;
        ForestTree[] near = FindObjectsByType<ForestTree>(FindObjectsSortMode.None)
            .Where(t => t.IsLiving && t.Species != null && t.Species.SpeciesId == "sitka-spruce")
            .OrderBy(t => (t.transform.position - here).sqrMagnitude).ThenBy(t => t.TreeId, StringComparer.Ordinal).Skip(3).ToArray();
        int crop = kind == "cropfell" ? Mathf.Max(1, count / 5) : kind == "crop" ? count : 0;
        int fell = kind == "crop" ? 0 : count;
        for (int i = 0; i < crop; i++) marking.Mark(near[i], TreeMarkType.CropTree, false);
        for (int i = 0; i < fell; i++) marking.Mark(near[crop + i], TreeMarkType.Fell, false);
        Debug.Log($"CCF_PERF_MARKS fell={marking.LivingMarkedCount} crop={marking.LivingCropTreeCount}");
    }

    private void SetHud(bool on)
    {
        if (ui != null && ui.RootElement != null) ui.RootElement.style.display = on ? UnityEngine.UIElements.DisplayStyle.Flex : UnityEngine.UIElements.DisplayStyle.None;
        currentToggles = on ? RemoveToken(currentToggles, "hud-off") : AddToken(currentToggles, "hud-off");
    }

    private static string AddToken(string list, string token) => list.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Contains(token) ? list : (list + " " + token).Trim();
    private static string RemoveToken(string list, string token) => string.Join(" ", list.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Where(t => t != token));

    // ---------- render A/B toggles (runtime only, reverted by "reset") ----------

    private static object Asset() => GraphicsSettings.currentRenderPipeline;

    private static object GetMember(object target, string name)
    {
        PropertyInfo p = target.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (p != null) return p.GetValue(target);
        FieldInfo f = target.GetType().GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (f != null) return f.GetValue(target);
        throw new InvalidOperationException("no member " + name + " on " + target.GetType().Name);
    }

    private static void SetMember(object target, string name, object value)
    {
        PropertyInfo p = target.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        MethodInfo setter = p != null ? p.GetSetMethod(true) : null;   // some URP setters are internal
        if (setter != null) { setter.Invoke(target, new[] { value }); return; }
        FieldInfo f = target.GetType().GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (f != null) { f.SetValue(target, value); return; }
        throw new InvalidOperationException("cannot set " + name + " on " + target.GetType().Name);
    }

    private float ShadowDistance() => Asset() != null ? (float)GetMember(Asset(), "shadowDistance") : QualitySettings.shadowDistance;

    private List<object> SsaoFeatures()
    {
        var result = new List<object>();
        object asset = Asset();
        if (asset == null) return result;
        var list = (Array)GetMember(asset, "m_RendererDataList");
        foreach (object data in list)
        {
            if (data == null) continue;
            foreach (object feature in (IEnumerable)GetMember(data, "rendererFeatures"))
                if (feature != null && feature.GetType().Name.Contains("ScreenSpaceAmbientOcclusion")) result.Add(feature);
        }
        return result;
    }

    private void Remember(string key, object value) { if (!originals.ContainsKey(key)) originals[key] = value; }

    private void Toggle(string what, float value)
    {
        object asset = Asset();
        switch (what)
        {
            case "ssao-off":
                foreach (object f in SsaoFeatures()) { Remember("ssao:" + f.GetHashCode(), GetMember(f, "isActive")); f.GetType().GetMethod("SetActive").Invoke(f, new object[] { false }); }
                break;
            case "shadows-off":
                Remember("mainshadows", GetMember(asset, "supportsMainLightShadows")); SetMember(asset, "supportsMainLightShadows", false); break;
            case "shadow-distance":
                Remember("shadowDistance", GetMember(asset, "shadowDistance")); SetMember(asset, "shadowDistance", value); break;
            case "shadow-resolution":
                Remember("shadowRes", GetMember(asset, "mainLightShadowmapResolution")); SetMember(asset, "mainLightShadowmapResolution", (int)value); break;
            case "shadow-cascades":
                Remember("cascades", GetMember(asset, "shadowCascadeCount")); SetMember(asset, "shadowCascadeCount", (int)value); break;
            case "soft-shadows-off":
                Remember("softShadows", GetMember(asset, "supportsSoftShadows")); SetMember(asset, "supportsSoftShadows", false); break;
            case "lod-bias":
                Remember("lodBias", QualitySettings.lodBias); QualitySettings.lodBias = value; break;
            case "render-scale":
                Remember("renderScale", GetMember(asset, "renderScale")); SetMember(asset, "renderScale", value); break;
            case "msaa-off":
                Remember("msaa", GetMember(asset, "msaaSampleCount")); SetMember(asset, "msaaSampleCount", 1); break;
            default: throw new InvalidOperationException("unknown toggle " + what);
        }
        currentToggles = AddToken(RemoveToken(currentToggles, what), what + (value != 0f ? "=" + value : ""));
        Debug.Log("CCF_PERF_TOGGLE " + currentToggles);
    }

    private void ResetToggles()
    {
        object asset = Asset();
        foreach (var item in originals.ToList())
        {
            string key = item.Key;
            if (key.StartsWith("ssao:")) { foreach (object f in SsaoFeatures()) if ("ssao:" + f.GetHashCode() == key) f.GetType().GetMethod("SetActive").Invoke(f, new object[] { item.Value }); }
            else if (key == "mainshadows") SetMember(asset, "supportsMainLightShadows", item.Value);
            else if (key == "shadowDistance") SetMember(asset, "shadowDistance", item.Value);
            else if (key == "shadowRes") SetMember(asset, "mainLightShadowmapResolution", item.Value);
            else if (key == "cascades") SetMember(asset, "shadowCascadeCount", item.Value);
            else if (key == "softShadows") SetMember(asset, "supportsSoftShadows", item.Value);
            else if (key == "lodBias") QualitySettings.lodBias = (float)item.Value;
            else if (key == "renderScale") SetMember(asset, "renderScale", item.Value);
            else if (key == "msaa") SetMember(asset, "msaaSampleCount", item.Value);
        }
        originals.Clear();
        currentToggles = currentToggles.Contains("hud-off") ? "hud-off" : "";
        Debug.Log("CCF_PERF_RESET");
    }

    // ---------- frame windows ----------

    private static double Percentile(List<double> sorted, double p) => sorted.Count == 0 ? 0 : sorted[(int)Math.Min(sorted.Count - 1, Math.Floor((sorted.Count - 1) * p))];

    private static Series Summarise(string name, List<double> values)
    {
        if (values.Count == 0) return new Series { name = name };
        var sorted = values.OrderBy(x => x).ToList();
        return new Series { name = name, mean = values.Average(), median = Percentile(sorted, 0.5), p95 = Percentile(sorted, 0.95), p99 = Percentile(sorted, 0.99), max = sorted[sorted.Count - 1], min = sorted[0] };
    }

    private IEnumerator Window(string name, double seconds, bool walk)
    {
        var dt = new List<double>(4096);
        var main = new List<double>(4096); var render = new List<double>(4096); var gpu = new List<double>(4096); var cpuFrame = new List<double>(4096);
        var alloc = new List<double>(4096);
        foreach (Marker m in markers) m.values.Clear();
        int gcStart = GC.CollectionCount(0), focused = 0, lastGc = gcStart;
        var gcAtFrame = new List<int>(4096);
        var clock = Stopwatch.StartNew();
        Vector3 startPosition = player.transform.position;
        while (clock.Elapsed.TotalSeconds < seconds)
        {
            if (walk)
            {
                if (Keyboard.current != null) InputSystem.QueueStateEvent(Keyboard.current, new KeyboardState(Key.W));
                if (Mouse.current != null) InputSystem.QueueDeltaStateEvent(Mouse.current.delta, new Vector2(2f, 0f));
            }
            yield return null;
            dt.Add(Time.unscaledDeltaTime * 1000.0);
            if (Application.isFocused) focused++;
            FrameTimingManager.CaptureFrameTimings();
            if (FrameTimingManager.GetLatestTimings(1, timing) > 0)
            {
                main.Add(timing[0].cpuMainThreadFrameTime); render.Add(timing[0].cpuRenderThreadFrameTime);
                gpu.Add(timing[0].gpuFrameTime); cpuFrame.Add(timing[0].cpuFrameTime);
            }
            double frameAlloc = 0;
            foreach (Marker m in markers)
            {
                double value = m.recorder.Valid ? m.recorder.LastValue * m.scale : 0;
                m.values.Add(value);
                if (m.name == "GC Allocated In Frame") frameAlloc = m.recorder.Valid ? m.recorder.LastValue : 0;
            }
            alloc.Add(frameAlloc);
            int gc = GC.CollectionCount(0); gcAtFrame.Add(gc - lastGc); lastGc = gc;
        }
        if (walk && Keyboard.current != null) InputSystem.QueueStateEvent(Keyboard.current, new KeyboardState());
        Check(dt.Count >= 20, "frame sample count " + name);
        var sorted = dt.OrderBy(x => x).ToList();
        double median = Percentile(sorted, 0.5);
        var series = new List<Series> { Summarise("cpuMainThreadFrameTime_ms", main), Summarise("cpuRenderThreadFrameTime_ms", render), Summarise("gpuFrameTime_ms", gpu), Summarise("cpuFrameTime_ms", cpuFrame) };
        foreach (Marker m in markers) series.Add(Summarise(m.category + "/" + m.name, m.values));
        var hitches = new List<Hitch>();
        double elapsed = 0;
        for (int i = 0; i < dt.Count; i++)
        {
            elapsed += dt[i] / 1000.0;
            if (dt[i] > Math.Max(3 * median, median + 20) && hitches.Count < 25)
                hitches.Add(new Hitch { frame = i, ms = dt[i], atSeconds = elapsed, gcAllocBytes = alloc[i], gc0Delta = gcAtFrame[i] });
        }
        var win = new Window
        {
            name = name, view = currentView, toggles = currentToggles, marks = marking != null ? marking.LivingMarkedCount + marking.LivingCropTreeCount : 0, frames = dt.Count, seconds = clock.Elapsed.TotalSeconds,
            meanMs = dt.Average(), medianMs = median, p95Ms = Percentile(sorted, 0.95), p99Ms = Percentile(sorted, 0.99), maxMs = sorted[sorted.Count - 1], minMs = sorted[0],
            gc0Delta = GC.CollectionCount(0) - gcStart, focusedFraction = (double)focused / dt.Count, series = series.ToArray(), hitches = hitches.ToArray(),
            note = walk ? $"synthetic walk moved {Vector3.Distance(startPosition, player.transform.position):0.0} m" : ""
        };
        report.windows.Add(win);
        // Per-frame CSV for later analysis.
        var header = new List<string> { "frame", "dt_ms", "cpuMain_ms", "cpuRender_ms", "gpu_ms", "cpuFrame_ms", "gcAlloc_bytes", "gc0_delta" };
        header.AddRange(markers.Select(m => m.category + "/" + m.name));
        var rows = new List<string> { string.Join(",", header) };
        for (int i = 0; i < dt.Count; i++)
        {
            var row = new List<string> { i.ToString(), dt[i].ToString("R", System.Globalization.CultureInfo.InvariantCulture) };
            Func<List<double>, string> at = list => i < list.Count ? list[i].ToString("R", System.Globalization.CultureInfo.InvariantCulture) : "";
            row.Add(at(main)); row.Add(at(render)); row.Add(at(gpu)); row.Add(at(cpuFrame)); row.Add(at(alloc)); row.Add(gcAtFrame[i].ToString());
            foreach (Marker m in markers) row.Add(at(m.values));
            rows.Add(string.Join(",", row));
        }
        File.WriteAllLines(Path.Combine(output, name + "-frames.csv"), rows);
        Debug.Log($"CCF_PERF_WINDOW {name} view={currentView} toggles=[{currentToggles}] frames={dt.Count} meanMs={win.meanMs:0.00} medianMs={win.medianMs:0.00} p95={win.p95Ms:0.00} p99={win.p99Ms:0.00} max={win.maxMs:0.00}");
    }

    // ---------- timed operations ----------

    private static Timing Summary(string group, string name, List<double> ms, long monoBytes, string note = "")
    {
        var sorted = ms.OrderBy(x => x).ToList();
        return new Timing { group = group, name = name, n = ms.Count, meanMs = ms.Average(), medianMs = Percentile(sorted, 0.5), p95Ms = Percentile(sorted, 0.95), minMs = sorted[0], maxMs = sorted[sorted.Count - 1], monoBytesPerCall = monoBytes, samplesMs = ms.ToArray(), note = note };
    }

    // The three marking-manager costs the Update path pays, called directly so they are separated from rendering.
    private IEnumerator TimeMarking(string name, int calls)
    {
        MethodInfo summary = typeof(ForestTreeMarkingManager).GetMethod("UpdateMarkedSummary", BindingFlags.Instance | BindingFlags.NonPublic);
        MethodInfo cropIds = typeof(ForestTreeMarkingManager).GetMethod("GetCropTreeIds", BindingFlags.Instance | BindingFlags.Public);
        MethodInfo outcome = typeof(ForestTreeMarkingManager).GetMethod("UpdateTreatmentOutcome", BindingFlags.Instance | BindingFlags.NonPublic);
        Check(summary != null && cropIds != null && outcome != null, "marking manager methods found");
        string label = $"{name}(fell={marking.LivingMarkedCount},crop={marking.LivingCropTreeCount})";
        foreach (var item in new[] { ("UpdateMarkedSummary", summary), ("GetCropTreeIds", cropIds), ("UpdateTreatmentOutcome", outcome) })
        {
            var ms = new List<double>(); long bytes = 0;
            for (int i = 0; i < calls; i++)
            {
                long before = Profiler.GetMonoUsedSizeLong(); int c0 = GC.CollectionCount(0);
                var clock = Stopwatch.StartNew(); item.Item2.Invoke(marking, null); clock.Stop();
                if (GC.CollectionCount(0) == c0) bytes += Profiler.GetMonoUsedSizeLong() - before;
                ms.Add(clock.Elapsed.TotalMilliseconds);
                if (i % 5 == 4) yield return null;
            }
            report.timings.Add(Summary("marking", label + " " + item.Item1, ms, bytes / calls));
        }
    }

    private IEnumerator TimeAdvance(string name, int repeats, bool restore)
    {
        ForestSaveData baseline = saves.CaptureData();
        var ms = new List<double>(); long bytes = 0;
        for (int i = 0; i < repeats; i++)
        {
            long before = Profiler.GetMonoUsedSizeLong();
            var clock = Stopwatch.StartNew();
            Check(manager.AdvanceYear(), "annual advance");
            clock.Stop(); ms.Add(clock.Elapsed.TotalMilliseconds); bytes += Profiler.GetMonoUsedSizeLong() - before;
            yield return null; yield return null;
            Memory(name + "_after_advance_" + i);
            ui.CloseHelp(); ui.CloseAll();
            if (restore && i < repeats - 1)
            {
                Check(saves.LoadData(JsonUtility.FromJson<ForestSaveData>(JsonUtility.ToJson(baseline)), false), "controlled state restored");
                yield return null; yield return null;
            }
        }
        report.timings.Add(Summary("annual", name, ms, bytes / repeats, restore ? "state restored between repeats" : "consecutive years"));
    }

    private IEnumerator TimeSaveLoad(string name, int repeats)
    {
        var save = new List<double>(); var load = new List<double>();
        for (int i = 0; i < repeats; i++)
        {
            var clock = Stopwatch.StartNew(); saves.Save(); clock.Stop(); save.Add(clock.Elapsed.TotalMilliseconds);
            yield return null;
            clock.Restart(); saves.Load(); clock.Stop(); load.Add(clock.Elapsed.TotalMilliseconds);
            yield return null; yield return null;
            ui.CloseHelp(); ui.CloseAll();
        }
        report.timings.Add(Summary("save", name + "_save", save, 0));
        report.timings.Add(Summary("load", name + "_load", load, 0));
        Memory(name + "_after");
    }

    private IEnumerator TimeRestart(string name)
    {
        var oldEcology = ecology.GetEntityId();
        var clock = Stopwatch.StartNew();
        menu.Restart();
        yield return null;
        for (int frame = 0; frame < 1800; frame++)
        {
            var candidate = FindFirstObjectByType<ForestEcologyController>();
            if (candidate != null && !candidate.GetEntityId().Equals(oldEcology) && candidate.CellCount == report.cells) break;
            yield return null;
        }
        clock.Stop();
        FindSystems();
        report.timings.Add(new Timing { group = "restart", name = name, n = 1, meanMs = clock.Elapsed.TotalMilliseconds, medianMs = clock.Elapsed.TotalMilliseconds, note = "menu.Restart to a new forest, world construction" });
        ui.CloseHelp(); ui.CloseAll();
        yield return null;
        Memory(name + "_after");
    }

    private void Memory(string phase)
    {
        using (Process p = Process.GetCurrentProcess())
        {
            p.Refresh();
            report.memory.Add(new MemoryRow { phase = phase, workingSet = p.WorkingSet64, privateBytes = p.PrivateMemorySize64, monoUsed = Profiler.GetMonoUsedSizeLong(),
                unityAllocated = Profiler.GetTotalAllocatedMemoryLong(), unityReserved = Profiler.GetTotalReservedMemoryLong(), graphicsDriver = Profiler.GetAllocatedMemoryForGraphicsDriver(),
                gcTotal = GC.GetTotalMemory(false), gc0 = GC.CollectionCount(0) });
        }
    }

    private IEnumerator Capture(string name)
    {
        yield return new WaitForEndOfFrame();
        Texture2D image = ScreenCapture.CaptureScreenshotAsTexture();
        File.WriteAllBytes(Path.Combine(output, name + ".png"), image.EncodeToPNG());
        Destroy(image);
    }

    // A binary Unity Profiler recording of a short stretch, for offline per-marker attribution in the Editor.
    private IEnumerator ProfilerCapture(string name, float seconds)
    {
        Profiler.logFile = Path.Combine(output, name);
        Profiler.enableBinaryLog = true;
        Profiler.maxUsedMemory = 512 * 1024 * 1024;
        Profiler.enabled = true;
        yield return new WaitForSecondsRealtime(seconds);
        Profiler.enabled = false;
        Profiler.enableBinaryLog = false;
        yield return null;
    }
}
