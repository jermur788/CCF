#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Unity.Profiling;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using Debug = UnityEngine.Debug;

// Disposable DIAGNOSIS (not a regression gate) for the player-test report "mouse look feels slightly jerky".
// It measures; it never smooths anything. Question it must answer: where is the look input or the frame time being lost?
//
//   W. WARP COST     - how long Mouse.WarpCursorPosition takes (the call ForestPlayer makes to recentre the locked pointer).
//   A. IDLE PACING   - frame times with the HUD up and no input: spikes, and whether they line up with the 0.25 s UI observation tick.
//   B. SYNTHETIC     - a constant-velocity mouse injected straight into the Input System (the mouse.delta ForestPlayer reads). A frame that
//                      is injected but applied as zero is a DROPPED input frame (the recentre grace in ForestPlayer.Update).
//   C. REAL INPUT    - (CCF_LOOK_REAL=1, needs real_motion_driver.py) real X11 pointer motion delivered by XTEST into the nested display, so
//                      the whole OS -> Unity -> Input System -> ForestPlayer path runs, including the warp artefact. The driver reports how
//                      many pixels it sent; that is the ground truth the applied yaw is compared with.
//   D. ALLOCATION    - GC-allocated bytes per frame (ProfilerRecorder) and GC collections during every phase, and (CCF_LOOK_GC=1) the same
//                      with each project component disabled in turn, so the allocation is attributed to a component, not guessed.
//
// Run (graphical Editor, ideally a private Xephyr so the window has focus):
//   -executeMethod CameraLookDiagnosis.Begin   CCF_DIAG_DIR=<folder>   CCF_LOOK_TAG=before|after   [CCF_LOOK_REAL=1] [CCF_LOOK_GC=1]
// Results: CAMLOOK_* lines in the log, plus camlook-<tag>-frames.csv and camlook-<tag>-summary.json in CCF_DIAG_DIR.
public static class CameraLookDiagnosis
{
    public static void Begin()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        SetGameSize(1280, 720);
        EditorApplication.isPlaying = true;
    }

    private static void SetGameSize(int width, int height)
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
            null, new object[] { Enum.ToObject(kind, 1), width, height, "CamLook " + width + "x" + height }, null);
        int index = (int)group.GetType().GetMethod("GetTotalCount").Invoke(group, null);
        group.GetType().GetMethod("AddCustomSize").Invoke(group, new[] { size });
        Type viewType = assembly.GetType("UnityEditor.GameView"); EditorWindow view = EditorWindow.GetWindow(viewType);
        viewType.GetProperty("selectedSizeIndex", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).SetValue(view, index);
        view.Show(); view.Focus();
        // A docked Game view smaller than the target is drawn scaled down, which multiplies pointer travel and leaves the pointer outside the
        // view. Maximised, the 1280 x 720 target renders 1:1 and the pointer can sit inside it like a player's.
        view.maximized = true;
        view.Repaint();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Prepare()
    {
        if (!Environment.GetCommandLineArgs().Any(a => a == "CameraLookDiagnosis.Begin")) return;
        foreach (MenuHelpView.Menu menu in Enum.GetValues(typeof(MenuHelpView.Menu))) PlayerPrefs.SetInt(MenuHelpView.PreferencePrefix + menu, 1);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (Environment.GetCommandLineArgs().Any(a => a == "CameraLookDiagnosis.Begin"))
            new GameObject("Camera look diagnosis").AddComponent<CameraLookDiagnosisRunner>();
    }
}

public sealed class CameraLookDiagnosisRunner : MonoBehaviour
{
    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
    private struct Frame
    {
        public int index; public double time; public float dt; public float injected; public float yawDelta; public float yawRaw;
        public float rawX, rawY;                    // what the Input System reports as mouse.delta this frame (what ForestPlayer reads)
        public float pointerX, pointerY; public long gcBytes; public int gcCollections; public bool focused, locked;
    }
    [Serializable] private class DriverReport { public float speed; public float seconds; public double sentPx; public double sentAbsPx; public int events; }

    private readonly List<Frame> frames = new List<Frame>();
    private readonly Dictionary<string, DriverReport> realReports = new Dictionary<string, DriverReport>();
    private ForestPlayer player;
    private ScenarioOneUiRoot ui;
    private string outDir, tag;
    private float sensitivity = 0.12f;
    private ProfilerRecorder gcRecorder;
    // The coroutine queues a delta during frame N; the Input System delivers it at the start of frame N+1, which is where ForestPlayer
    // reads it. So the delta ForestPlayer saw in a frame is the one queued in the frame before (consumedThisFrame).
    private float queuedForNextFrame, consumedThisFrame;
    private float lastYaw;
    private readonly List<(string name, int first, int last)> phases = new List<(string, int, int)>();
    private readonly List<string> warpCostLines = new List<string>();

    private IEnumerator Start()
    {
        outDir = Environment.GetEnvironmentVariable("CCF_DIAG_DIR") ?? Path.Combine(Application.dataPath, "..", "Logs");
        Directory.CreateDirectory(outDir);
        tag = Environment.GetEnvironmentVariable("CCF_LOOK_TAG") ?? "run";
        bool real = Environment.GetEnvironmentVariable("CCF_LOOK_REAL") == "1";
        bool gc = Environment.GetEnvironmentVariable("CCF_LOOK_GC") == "1";
        gcRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC Allocated In Frame");
        for (int i = 0; i < 20; i++) yield return null;
        ui = FindFirstObjectByType<ScenarioOneUiRoot>();
        player = FindFirstObjectByType<ForestPlayer>();
        if (ui != null) ui.CloseHelp();
        if (player != null)
        {
            FieldInfo field = typeof(ForestPlayer).GetField("mouseSensitivity", BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null) sensitivity = (float)field.GetValue(player);
        }
        // The window must own the keyboard/mouse for ForestPlayer to read look input.
        for (int i = 0; i < 90 && !(Application.isFocused && Cursor.lockState == CursorLockMode.Locked); i++) yield return null;
        Debug.Log($"CAMLOOK_ENV focused={Application.isFocused} lock={Cursor.lockState} screen={Screen.width}x{Screen.height} " +
            $"vSync={QualitySettings.vSyncCount} targetFps={Application.targetFrameRate} editor={Application.isEditor} " +
            $"mouseSensitivity={sensitivity} inputUpdateMode={InputSystem.settings.updateMode} fixedDt={Time.fixedDeltaTime} maxDt={Time.maximumDeltaTime} " +
            $"real={real} gc={gc} loadavg={SafeRead("/proc/loadavg")}");
        if (player == null) { Debug.Log("CAMLOOK_UNVERIFIED no ForestPlayer"); Finish(1); yield break; }

        LogGameView();
        yield return Calibrate();
        yield return WarpCost(60);
        yield return Phase("idle", 360, 0f);
        foreach (float pixelsPerSecond in new[] { 120f, 360f, 720f, 1400f })
            yield return Phase("sweep-" + pixelsPerSecond.ToString("0", Inv), 240, pixelsPerSecond);
        if (real)
        {
            // Alternate direction so a pointer that really moves on screen is not driven into an edge by the test itself.
            float sign = 1f;
            // How people actually look around: pan, pause, pan back, pause. The driver runs the pattern; speed is ignored.
            yield return RealPhase("real-natural", 0f, 12.5f, "natural");
            for (int i = 0; i < 30; i++) yield return null;
            foreach (float pixelsPerSecond in new[] { 120f, 360f, 720f, 1400f })
            {
                yield return RealPhase("real-" + pixelsPerSecond.ToString("0", Inv), pixelsPerSecond * sign, 6f, "sweep");
                sign = -sign;
                for (int i = 0; i < 30; i++) yield return null;
            }
        }
        if (gc) yield return GcAttribution();
        Summarise();
        Finish(0);
    }

    // Game-pixels the pointer moves per pixel the driver sends. 1 when the Game view is shown 1:1; the Editor scales pointer travel when the
    // view is drawn smaller or larger than its target size. The driver divides what it sends by this, so the motion Unity sees is what a player's hand would make.
    private float pointerGain = 1f;

    // What the Game view looks like, so a pointer that behaves oddly can be explained instead of guessed at.
    private void LogGameView()
    {
        try
        {
            Type viewType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.GameView");
            EditorWindow view = EditorWindow.GetWindow(viewType);
            var sb = new StringBuilder($"CAMLOOK_VIEW window=({view.position.x:0},{view.position.y:0},{view.position.width:0},{view.position.height:0}) maximized={view.maximized} " +
                $"screen={Screen.width}x{Screen.height} current={Screen.currentResolution.width}x{Screen.currentResolution.height} display={Display.main.renderingWidth}x{Display.main.renderingHeight}");
            foreach (FieldInfo field in viewType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            {
                string lower = field.Name.ToLowerInvariant();
                if (!lower.Contains("zoom") && !lower.Contains("scale") && !lower.Contains("targetsize") && !lower.Contains("selectedsize")) continue;
                object value = field.GetValue(view);
                sb.Append(' ').Append(field.Name).Append('=').Append(value == null ? "null" : value.ToString());
                if (value != null && field.FieldType.Name == "ZoomableArea")
                    foreach (string inner in new[] { "scale", "shownArea", "drawRect", "m_Scale", "m_DrawArea" })
                    {
                        FieldInfo f = field.FieldType.GetField(inner, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                        PropertyInfo prop = field.FieldType.GetProperty(inner, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                        object innerValue = f != null ? f.GetValue(value) : prop != null ? prop.GetValue(value) : null;
                        if (innerValue != null) sb.Append(" [").Append(inner).Append('=').Append(innerValue).Append(']');
                    }
            }
            Debug.Log(sb.ToString());
        }
        catch (Exception e) { Debug.Log("CAMLOOK_VIEW unavailable: " + e.Message); }
    }

    // Measure the gain with ForestPlayer off (so nothing recentres): the pointer is free, the driver sends 300 px, and the Game-space travel says how far
    // one sent pixel really goes in this window. Also asks the driver for a screenshot of the display, so the layout can be read, not assumed.
    private IEnumerator Calibrate()
    {
        Mouse mouse = Mouse.current;
        if (mouse == null || player == null) yield break;
        player.enabled = false;
        for (int i = 0; i < 20; i++) yield return null;
        mouse.WarpCursorPosition(new Vector2(Screen.width * 0.5f, Screen.height * 0.5f));
        for (int i = 0; i < 25; i++) yield return null;
        Vector2 before = mouse.position.ReadValue();
        File.WriteAllText(Path.Combine(outDir, "real-start-calib.txt"), "speed=300\nseconds=1.0\npattern=sweep\ngain=1\nshot=calib-start\n");
        string report = Path.Combine(outDir, "real-driver-calib.json");
        for (int i = 0; i < 200 && !File.Exists(report); i++) yield return null;
        for (int i = 0; i < 10; i++) yield return null;
        Vector2 after = mouse.position.ReadValue();
        if (File.Exists(report))
        {
            DriverReport r = JsonUtility.FromJson<DriverReport>(File.ReadAllText(report));
            float travelled = after.x - before.x;
            if (Mathf.Abs((float)r.sentPx) > 10f) pointerGain = Mathf.Clamp(travelled / (float)r.sentPx, 0.2f, 8f);
            Debug.Log($"CAMLOOK_CALIB sentPx={r.sentPx:0} pointerBefore=({before.x:0},{before.y:0}) pointerAfter=({after.x:0},{after.y:0}) travelledGamePx={travelled:0} gain={pointerGain:0.000}");
        }
        else Debug.Log("CAMLOOK_UNVERIFIED no calibration report");
        player.enabled = true;
        for (int i = 0; i < 30; i++) yield return null;
    }

    // The synchronous cost of the recentre call. ForestPlayer makes this call every time the look budget is spent.
    private IEnumerator WarpCost(int count)
    {
        Mouse mouse = Mouse.current;
        var ms = new List<double>();
        for (int i = 0; i < count && mouse != null; i++)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            mouse.WarpCursorPosition(new Vector2(Screen.width * 0.5f, Screen.height * 0.5f));
            watch.Stop();
            ms.Add(watch.Elapsed.TotalMilliseconds);
            yield return null;
        }
        if (ms.Count == 0) yield break;
        ms.Sort();
        Debug.Log($"CAMLOOK_WARPCOST calls={ms.Count} ms[median={ms[ms.Count / 2]:0.000} p95={ms[(int)(ms.Count * 0.95)]:0.000} max={ms[ms.Count - 1]:0.000}]");
        // Let ForestPlayer's own grace window (armed by nothing here, but cheap to be safe) and any queued pointer events settle.
        for (int i = 0; i < 20; i++) yield return null;
    }

    // Records `count` frames. A positive speed injects a mouse delta every frame, rounded to whole pixels the way a real mouse
    // reports it, with the fraction carried so the average speed is exact.
    private IEnumerator Phase(string name, int count, float pixelsPerSecond)
    {
        int first = frames.Count;
        float remainder = 0f, previousDt = 1f / 60f;
        for (int i = 0; i < count; i++)
        {
            if (pixelsPerSecond > 0f)
            {
                remainder += pixelsPerSecond * previousDt;
                float whole = Mathf.Floor(remainder);
                remainder -= whole;
                queuedForNextFrame = whole;
                QueueMouse(whole);
            }
            else queuedForNextFrame = 0f;
            yield return null;
            previousDt = Mathf.Clamp(Time.unscaledDeltaTime, 0.001f, 0.1f);
        }
        phases.Add((name, first, frames.Count - 1));
    }

    // Real pointer motion: the external driver sees real-start-<name>.txt, sends `seconds` of XTEST relative motion at the stated
    // speed, and writes real-driver-<name>.json with the pixels it actually sent.
    private IEnumerator RealPhase(string name, float signedSpeed, float seconds, string pattern)
    {
        queuedForNextFrame = 0f;
        // Start every phase with the real pointer at the middle of the Game view, using Unity's own warp so the coordinates are Unity's.
        Mouse mouse = Mouse.current;
        if (mouse != null) mouse.WarpCursorPosition(new Vector2(Screen.width * 0.5f, Screen.height * 0.5f));
        for (int i = 0; i < 25; i++) yield return null;
        Vector2 start = mouse != null ? mouse.position.ReadValue() : Vector2.zero;
        Debug.Log($"CAMLOOK_REALSTART name={name} pointerAtStart=({start.x:0},{start.y:0}) gameCentre=({Screen.width * 0.5f:0},{Screen.height * 0.5f:0})");
        int first = frames.Count;
        File.WriteAllText(Path.Combine(outDir, "real-start-" + name + ".txt"),
            "speed=" + signedSpeed.ToString("0", Inv) + "\nseconds=" + seconds.ToString("0.0", Inv) + "\npattern=" + pattern + "\ngain=" + pointerGain.ToString("0.0000", Inv) + "\n");
        float end = Time.realtimeSinceStartup + seconds + 0.8f;
        while (Time.realtimeSinceStartup < end) yield return null;
        phases.Add((name, first, frames.Count - 1));
        string report = Path.Combine(outDir, "real-driver-" + name + ".json");
        for (int i = 0; i < 80 && !File.Exists(report); i++) yield return null;
        if (File.Exists(report)) realReports[name] = JsonUtility.FromJson<DriverReport>(File.ReadAllText(report));
        else Debug.Log("CAMLOOK_UNVERIFIED no driver report for " + name);
    }

    // Disable every project component in turn and measure what is still allocated. The difference is that component's share.
    private IEnumerator GcAttribution()
    {
        var groups = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
            .Where(b => b != null && b != this && b.enabled && b.gameObject.activeInHierarchy && b.GetType().Assembly.GetName().Name == "Assembly-CSharp")
            .GroupBy(b => b.GetType()).OrderBy(g => g.Key.Name).ToList();
        Debug.Log("CAMLOOK_GC_TYPES " + string.Join(",", groups.Select(g => g.Key.Name + "x" + g.Count())));
        yield return Phase("gc-all", 120, 0f);
        foreach (var group in groups)
        {
            List<MonoBehaviour> list = group.ToList();
            foreach (MonoBehaviour b in list) b.enabled = false;
            yield return Phase("gcoff-" + group.Key.Name, 60, 0f);
            foreach (MonoBehaviour b in list) if (b != null) b.enabled = true;
            for (int i = 0; i < 6; i++) yield return null;
        }
        yield return Phase("gc-all-after", 120, 0f);
    }

    private static void QueueMouse(float dx)
    {
        Mouse mouse = Mouse.current ?? InputSystem.AddDevice<Mouse>();
        InputSystem.QueueDeltaStateEvent(mouse.delta, new Vector2(dx, 0f));
    }

    // After every script has updated: what yaw ForestPlayer left the player with this frame.
    private void LateUpdate()
    {
        if (player == null) return;
        float yaw = player.transform.eulerAngles.y;
        float delta = Mathf.DeltaAngle(lastYaw, yaw);
        lastYaw = yaw;
        Mouse mouse = Mouse.current;
        Vector2 pointer = mouse != null ? mouse.position.ReadValue() : Vector2.zero;
        Vector2 raw = mouse != null ? mouse.delta.ReadValue() : Vector2.zero;
        frames.Add(new Frame
        {
            index = Time.frameCount, time = Time.unscaledTimeAsDouble, dt = Time.unscaledDeltaTime,
            injected = consumedThisFrame, rawX = raw.x, rawY = raw.y,
            yawDelta = delta, yawRaw = yaw, pointerX = pointer.x, pointerY = pointer.y,
            gcBytes = gcRecorder.Valid ? gcRecorder.LastValue : -1, gcCollections = GC.CollectionCount(0),
            focused = Application.isFocused, locked = Cursor.lockState == CursorLockMode.Locked
        });
        // What was queued this frame is what ForestPlayer will read next frame.
        consumedThisFrame = queuedForNextFrame;
        queuedForNextFrame = 0f;
    }

    private void Summarise()
    {
        foreach (string line in warpCostLines) Debug.Log(line);
        var csv = new StringBuilder("phase,frame,time,dt_ms,injected_px,raw_x,raw_y,yaw_applied_deg,expected_deg,pointer_x,pointer_y,gc_bytes,gc_collections,focused,locked\n");
        var json = new StringBuilder("{\n  \"tag\": \"" + tag + "\",\n  \"sensitivity\": " + sensitivity.ToString("0.000", Inv) + ",\n  \"phases\": [\n");
        var gcRows = new List<(string name, long bytes, float dtMedian)>();
        foreach (var (name, first, last) in phases)
        {
            List<Frame> slice = frames.Skip(first).Take(last - first + 1).ToList();
            foreach (Frame f in slice)
                csv.Append(name).Append(',').Append(f.index).Append(',').Append(f.time.ToString("0.0000", Inv)).Append(',')
                   .Append((f.dt * 1000f).ToString("0.00", Inv)).Append(',').Append(f.injected.ToString("0", Inv)).Append(',')
                   .Append(f.rawX.ToString("0.0", Inv)).Append(',').Append(f.rawY.ToString("0.0", Inv)).Append(',')
                   .Append(f.yawDelta.ToString("0.0000", Inv)).Append(',').Append((f.injected * sensitivity).ToString("0.0000", Inv)).Append(',')
                   .Append(f.pointerX.ToString("0", Inv)).Append(',').Append(f.pointerY.ToString("0", Inv)).Append(',')
                   .Append(f.gcBytes).Append(',').Append(f.gcCollections).Append(',').Append(f.focused ? 1 : 0).Append(',').Append(f.locked ? 1 : 0).Append('\n');
            if (slice.Count < 8) continue;

            float[] dts = slice.Skip(5).Select(f => f.dt * 1000f).OrderBy(x => x).ToArray();
            float median = dts[dts.Length / 2], p95 = dts[(int)(dts.Length * 0.95f)], p99 = dts[(int)(dts.Length * 0.99f)], max = dts[dts.Length - 1];
            int spikes = dts.Count(x => x > median * 2f);
            long gcSum = slice.Sum(f => Math.Max(0L, f.gcBytes));
            long gcPerFrame = gcSum / slice.Count;
            int gcCollections = slice[slice.Count - 1].gcCollections - slice[0].gcCollections;
            if (name.StartsWith("gc"))
            {
                gcRows.Add((name, gcPerFrame, median));
                continue;
            }
            // Frames whose pointer moved far in one frame: a recentre warp (the locked pointer should otherwise sit still).
            int warps = 0;
            for (int i = 1; i < slice.Count; i++)
                if (Vector2.Distance(new Vector2(slice[i].pointerX, slice[i].pointerY), new Vector2(slice[i - 1].pointerX, slice[i - 1].pointerY)) > 60f) warps++;
            float pointerRange = Mathf.Max(slice.Max(f => f.pointerX) - slice.Min(f => f.pointerX), slice.Max(f => f.pointerY) - slice.Min(f => f.pointerY));
            // Spike phase against the 0.25 s UI observation tick.
            int tickAligned = 0, spikeCount = 0;
            for (int i = 1; i < slice.Count; i++)
            {
                if (slice[i].dt * 1000f <= median * 2f) continue;
                spikeCount++;
                double phase = slice[i].time % 0.25;
                if (phase < 0.03 || phase > 0.22) tickAligned++;
            }
            float injectedSum = slice.Sum(f => f.injected) * sensitivity, appliedSum = slice.Sum(f => f.yawDelta);
            int injectedFrames = slice.Count(f => f.injected > 0.5f);
            int dropped = slice.Count(f => f.injected > 0.5f && Mathf.Abs(f.yawDelta) < 0.0001f);
            int maxRun = 0, run = 0;
            foreach (Frame f in slice)
            {
                if (f.injected > 0.5f && Mathf.Abs(f.yawDelta) < 0.0001f) { run++; maxRun = Mathf.Max(maxRun, run); } else run = 0;
            }
            // Smoothness of the applied angular velocity (deg/s) over frames that received input.
            float[] velocity = slice.Where(f => f.injected > 0.5f && f.dt > 0f).Select(f => Mathf.Abs(f.yawDelta) / f.dt).ToArray();
            float vMean = velocity.Length > 0 ? velocity.Average() : 0f;
            float vStd = velocity.Length > 1 ? Mathf.Sqrt(velocity.Sum(v => (v - vMean) * (v - vMean)) / velocity.Length) : 0f;
            string line = $"CAMLOOK_PHASE name={name} frames={slice.Count} dt_ms[median={median:0.00} p95={p95:0.00} p99={p99:0.00} max={max:0.00}] spikes>2x={spikes} " +
                $"spikeTickAligned={tickAligned}/{spikeCount} gcBytes/frame={gcPerFrame} gcCollections={gcCollections} " +
                $"injectedFrames={injectedFrames} droppedFrames={dropped} maxDroppedRun={maxRun} injectedDeg={injectedSum:0.00} appliedDeg={appliedSum:0.00} " +
                $"lossPct={(injectedSum > 0.01f ? 100f * (injectedSum - appliedSum) / injectedSum : 0f):0.0} warpsDetected={warps} pointerRangePx={pointerRange:0} " +
                $"yawVelocity[mean={vMean:0.0} std={vStd:0.0} cv={(vMean > 0 ? vStd / vMean : 0f):0.000}] focusedAll={slice.All(f => f.focused)} lockedAll={slice.All(f => f.locked)}";
            Debug.Log(line);
            string realJson = "";
            if (realReports.TryGetValue(name, out DriverReport report))
            {
                // End to end: pixels the OS pointer really moved (driver) -> pixels Unity reported (raw) -> pixels ForestPlayer applied.
                double rawSum = slice.Sum(f => (double)f.rawX), rawAbs = slice.Sum(f => (double)Mathf.Abs(f.rawX));
                double appliedPx = slice.Sum(f => (double)f.yawDelta) / sensitivity;
                double sent = report.sentPx;
                double appliedAbsPx = slice.Sum(f => (double)Mathf.Abs(f.yawDelta)) / sensitivity;
                double sentAbsGame = report.sentAbsPx * pointerGain;           // the driver already divided its motion by the gain, so this is what Unity should see
                double lossAbsPct = sentAbsGame > 1 ? 100.0 * (1.0 - appliedAbsPx / sentAbsGame) : 0.0;
                int recentres = 0;
                for (int i = 1; i < slice.Count; i++)
                    if (Vector2.Distance(new Vector2(slice[i].pointerX, slice[i].pointerY), new Vector2(slice[i - 1].pointerX, slice[i - 1].pointerY)) > 60f) recentres++;
                double reportedPct = Math.Abs(sent) > 1 ? 100.0 * rawSum / sent : 0.0, appliedPct = Math.Abs(sent) > 1 ? 100.0 * appliedPx / sent : 0.0;
                int spikeFrames = slice.Count(f => Mathf.Abs(f.rawX) > 3f * Mathf.Max(8f, (float)Math.Abs(sent) / Mathf.Max(1f, report.seconds) * median / 1000f));
                float biggestRaw = slice.Max(f => Mathf.Abs(f.rawX));
                int zeroApplied = slice.Count(f => Mathf.Abs(f.rawX) > 0.5f && Mathf.Abs(f.yawDelta) < 0.0001f);
                Debug.Log($"CAMLOOK_REAL name={name} sentPx={sent:0} events={report.events} rawReportedPx={rawSum:0} ({reportedPct:0.0}% of sent) appliedPxEquivalent={appliedPx:0} " +
                    $"({appliedPct:0.0}% of sent) lossVsSentPct={(100.0 - appliedPct):0.0} rawAbsPx={rawAbs:0} spikeFrames>3x={spikeFrames} biggestRawPx={biggestRaw:0} framesRawNonZeroButAppliedZero={zeroApplied} " +
                    $"PATH sentAbsPx={report.sentAbsPx:0} gain={pointerGain:0.000} expectedGamePx={sentAbsGame:0} appliedAbsPx={appliedAbsPx:0} pathLossPct={lossAbsPct:0.0} pointerJumps={recentres}");
                realJson = ", \"sentPx\": " + sent.ToString("0", Inv) + ", \"rawReportedPx\": " + rawSum.ToString("0", Inv) + ", \"appliedPxEq\": " + appliedPx.ToString("0", Inv)
                    + ", \"appliedPctOfSent\": " + appliedPct.ToString("0.0", Inv) + ", \"rawNonZeroAppliedZero\": " + zeroApplied
                    + ", \"sentAbsPx\": " + report.sentAbsPx.ToString("0", Inv) + ", \"appliedAbsPx\": " + appliedAbsPx.ToString("0", Inv) + ", \"pathLossPct\": " + lossAbsPct.ToString("0.0", Inv)
                    + ", \"pointerJumps\": " + recentres;
            }
            json.Append("    { \"name\": \"" + name + "\", \"frames\": " + slice.Count + ", \"dtMedianMs\": " + median.ToString("0.00", Inv) + ", \"dtP99Ms\": " + p99.ToString("0.00", Inv)
                + ", \"dtMaxMs\": " + max.ToString("0.00", Inv) + ", \"spikes\": " + spikes + ", \"gcBytesPerFrame\": " + gcPerFrame
                + ", \"droppedFrames\": " + dropped + ", \"injectedFrames\": " + injectedFrames + ", \"warps\": " + warps + ", \"lossPct\": "
                + (injectedSum > 0.01f ? 100f * (injectedSum - appliedSum) / injectedSum : 0f).ToString("0.0", Inv) + ", \"velocityCv\": " + (vMean > 0 ? vStd / vMean : 0f).ToString("0.000", Inv)
                + realJson + " },\n");
        }
        if (gcRows.Count > 0)
        {
            long all = gcRows.Where(r => r.name == "gc-all").Select(r => r.bytes).DefaultIfEmpty(0).First();
            foreach (var (name, bytes, dtMedian) in gcRows)
                Debug.Log($"CAMLOOK_GCATTR name={name} gcBytes/frame={bytes} deltaVsAll={bytes - all} dtMedianMs={dtMedian:0.00}");
        }
        LogWarpEvents();
        string jsonText = json.ToString().TrimEnd(',', '\n') + "\n  ]\n}\n";
        File.WriteAllText(Path.Combine(outDir, "camlook-" + tag + "-frames.csv"), csv.ToString());
        File.WriteAllText(Path.Combine(outDir, "camlook-" + tag + "-summary.json"), jsonText);
        Debug.Log("CAMLOOK_DONE tag=" + tag + " frames=" + frames.Count);
    }

    // For the first few pointer jumps in each phase: the raw delta, the yaw that was applied and the frame time around the jump. This is
    // the warp's signature: what the OS reports for the recentre, and which frames ForestPlayer then throws away.
    private void LogWarpEvents()
    {
        foreach (var (name, first, last) in phases)
        {
            if (name.StartsWith("gc")) continue;
            int shown = 0;
            for (int i = first + 1; i <= last && shown < 4; i++)
            {
                Frame a = frames[i - 1], b = frames[i];
                if (Vector2.Distance(new Vector2(a.pointerX, a.pointerY), new Vector2(b.pointerX, b.pointerY)) <= 60f) continue;
                shown++;
                var sb = new StringBuilder($"CAMLOOK_WARP phase={name} frame={b.index} pointer=({a.pointerX:0},{a.pointerY:0})->({b.pointerX:0},{b.pointerY:0}) frames[-1..+3](rawX/rawY yawDeg dtMs):");
                for (int k = Mathf.Max(first, i - 1); k <= Mathf.Min(last, i + 3); k++)
                    sb.Append($" [{frames[k].rawX:0.#}/{frames[k].rawY:0.#} {frames[k].yawDelta:0.00} {frames[k].dt * 1000f:0}]");
                Debug.Log(sb.ToString());
            }
        }
    }

    private static string SafeRead(string path) { try { return File.ReadAllText(path).Trim(); } catch { return "n/a"; } }

    private void Finish(int code)
    {
        gcRecorder.Dispose();
        File.WriteAllText(Path.Combine(outDir, "camlook-finished.txt"), "done\n");
        Debug.Log("CAMLOOK_DONE_MARKER");
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(code);
    }
}
#endif
