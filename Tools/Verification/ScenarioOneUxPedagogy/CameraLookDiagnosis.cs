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

// Disposable DIAGNOSIS (not a regression gate) for the player-test report "mouse look feels slightly jerky".
// It measures; it never smooths anything. Question it must answer: where is the look input or the frame time being lost?
//
//   A. IDLE PACING   - frame times with the HUD up and no input: spikes, and whether they line up with the 0.25 s UI observation tick.
//   B. LOOK SWEEPS   - a constant-velocity mouse is injected through the Input System (the same mouse.delta ForestPlayer reads) at
//                      several speeds; per frame it records the injected delta, the yaw ForestPlayer actually applied and the
//                      pointer position. A frame that is injected but applied as zero is a DROPPED input frame (the recentre
//                      grace in ForestPlayer.Update); a pointer jump is a WarpCursorPosition recentre.
//   C. ALLOCATION    - GC-allocated bytes per frame (ProfilerRecorder) and GC collections during A and B.
//
// Run (graphical Editor, ideally a private Xephyr so the window has focus):
//   -executeMethod CameraLookDiagnosis.Begin   CCF_DIAG_DIR=<folder>   CCF_LOOK_TAG=before|after
// Results: CAMLOOK_* lines in the log, plus camlook-<tag>-*.csv and camlook-<tag>-summary.json in CCF_DIAG_DIR.
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
        view.Show(); view.Focus(); view.Repaint();
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
        public float pointerX, pointerY; public long gcBytes; public int gcCollections; public bool focused, locked;
    }
    private readonly List<Frame> frames = new List<Frame>();
    private ForestPlayer player;
    private ScenarioOneUiRoot ui;
    private string outDir, tag;
    private ProfilerRecorder gcRecorder;
    // The coroutine queues a delta during frame N; the Input System delivers it at the start of frame N+1, which is where ForestPlayer
    // reads it. So the delta ForestPlayer saw in a frame is the one queued in the frame before (consumedThisFrame).
    private float queuedForNextFrame, consumedThisFrame;
    private float lastYaw;
    private readonly List<(string name, int first, int last)> phases = new List<(string, int, int)>();

    private IEnumerator Start()
    {
        outDir = Environment.GetEnvironmentVariable("CCF_DIAG_DIR") ?? Path.Combine(Application.dataPath, "..", "Logs");
        Directory.CreateDirectory(outDir);
        tag = Environment.GetEnvironmentVariable("CCF_LOOK_TAG") ?? "run";
        gcRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC Allocated In Frame");
        for (int i = 0; i < 20; i++) yield return null;
        ui = FindFirstObjectByType<ScenarioOneUiRoot>();
        player = FindFirstObjectByType<ForestPlayer>();
        if (ui != null) ui.CloseHelp();
        // The window must own the keyboard/mouse for ForestPlayer to read look input.
        for (int i = 0; i < 90 && !(Application.isFocused && Cursor.lockState == CursorLockMode.Locked); i++) yield return null;
        Debug.Log($"CAMLOOK_ENV focused={Application.isFocused} lock={Cursor.lockState} screen={Screen.width}x{Screen.height} " +
            $"vSync={QualitySettings.vSyncCount} targetFps={Application.targetFrameRate} editor={Application.isEditor} " +
            $"mouseSensitivity=0.12 (ForestPlayer) loadavg={SafeRead("/proc/loadavg")}");
        if (player == null) { Debug.Log("CAMLOOK_UNVERIFIED no ForestPlayer"); Finish(1); yield break; }

        yield return Phase("idle", 480, 0f);
        foreach (float pixelsPerSecond in new[] { 120f, 360f, 720f, 1400f })
            yield return Phase("sweep-" + pixelsPerSecond.ToString("0", Inv), 300, pixelsPerSecond);
        Summarise();
        Finish(0);
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
        frames.Add(new Frame
        {
            index = Time.frameCount, time = Time.unscaledTimeAsDouble, dt = Time.unscaledDeltaTime,
            injected = consumedThisFrame,
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
        var csv = new StringBuilder("phase,frame,time,dt_ms,injected_px,yaw_applied_deg,expected_deg,pointer_x,pointer_y,gc_bytes,gc_collections,focused,locked\n");
        var json = new StringBuilder("{\n  \"tag\": \"" + tag + "\",\n  \"phases\": [\n");
        foreach (var (name, first, last) in phases)
        {
            List<Frame> slice = frames.Skip(first).Take(last - first + 1).ToList();
            foreach (Frame f in slice)
                csv.Append(name).Append(',').Append(f.index).Append(',').Append(f.time.ToString("0.0000", Inv)).Append(',')
                   .Append((f.dt * 1000f).ToString("0.00", Inv)).Append(',').Append(f.injected.ToString("0", Inv)).Append(',')
                   .Append(f.yawDelta.ToString("0.0000", Inv)).Append(',').Append((f.injected * 0.12f).ToString("0.0000", Inv)).Append(',')
                   .Append(f.pointerX.ToString("0", Inv)).Append(',').Append(f.pointerY.ToString("0", Inv)).Append(',')
                   .Append(f.gcBytes).Append(',').Append(f.gcCollections).Append(',').Append(f.focused ? 1 : 0).Append(',').Append(f.locked ? 1 : 0).Append('\n');

            float[] dts = slice.Skip(5).Select(f => f.dt * 1000f).OrderBy(x => x).ToArray();
            float median = dts[dts.Length / 2], p95 = dts[(int)(dts.Length * 0.95f)], p99 = dts[(int)(dts.Length * 0.99f)], max = dts[dts.Length - 1];
            int spikes = dts.Count(x => x > median * 2f);
            // Frames whose pointer moved far in one frame: a recentre warp (the locked pointer should otherwise sit still).
            int warps = 0;
            for (int i = 1; i < slice.Count; i++)
                if (Vector2.Distance(new Vector2(slice[i].pointerX, slice[i].pointerY), new Vector2(slice[i - 1].pointerX, slice[i - 1].pointerY)) > 60f) warps++;
            float pointerRange = slice.Count > 0 ? Mathf.Max(slice.Max(f => f.pointerX) - slice.Min(f => f.pointerX), slice.Max(f => f.pointerY) - slice.Min(f => f.pointerY)) : 0f;
            // Spike phase against the 0.25 s UI observation tick.
            int tickAligned = 0, spikeCount = 0;
            for (int i = 1; i < slice.Count; i++)
            {
                if (slice[i].dt * 1000f <= median * 2f) continue;
                spikeCount++;
                double phase = slice[i].time % 0.25;
                if (phase < 0.03 || phase > 0.22) tickAligned++;
            }
            long gcSum = slice.Sum(f => Math.Max(0L, f.gcBytes));
            int gcCollections = slice.Count > 1 ? slice[slice.Count - 1].gcCollections - slice[0].gcCollections : 0;
            float injectedSum = slice.Sum(f => f.injected) * 0.12f, appliedSum = slice.Sum(f => f.yawDelta);
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
                $"spikeTickAligned={tickAligned}/{spikeCount} gcBytes/frame={(slice.Count > 0 ? gcSum / slice.Count : 0)} gcCollections={gcCollections} " +
                $"injectedFrames={injectedFrames} droppedFrames={dropped} maxDroppedRun={maxRun} injectedDeg={injectedSum:0.00} appliedDeg={appliedSum:0.00} " +
                $"lossPct={(injectedSum > 0.01f ? 100f * (injectedSum - appliedSum) / injectedSum : 0f):0.0} warpsDetected={warps} pointerRangePx={pointerRange:0} " +
                $"yawVelocity[mean={vMean:0.0} std={vStd:0.0} cv={(vMean > 0 ? vStd / vMean : 0f):0.000}] focusedAll={slice.All(f => f.focused)} lockedAll={slice.All(f => f.locked)}";
            Debug.Log(line);
            json.Append("    { \"name\": \"" + name + "\", \"frames\": " + slice.Count + ", \"dtMedianMs\": " + median.ToString("0.00", Inv) + ", \"dtP99Ms\": " + p99.ToString("0.00", Inv)
                + ", \"dtMaxMs\": " + max.ToString("0.00", Inv) + ", \"spikes\": " + spikes + ", \"gcBytesPerFrame\": " + (slice.Count > 0 ? gcSum / slice.Count : 0)
                + ", \"droppedFrames\": " + dropped + ", \"injectedFrames\": " + injectedFrames + ", \"warps\": " + warps + ", \"lossPct\": "
                + (injectedSum > 0.01f ? 100f * (injectedSum - appliedSum) / injectedSum : 0f).ToString("0.0", Inv) + ", \"velocityCv\": " + (vMean > 0 ? vStd / vMean : 0f).ToString("0.000", Inv) + " },\n");
        }
        string jsonText = json.ToString().TrimEnd(',', '\n') + "\n  ]\n}\n";
        File.WriteAllText(Path.Combine(outDir, "camlook-" + tag + "-frames.csv"), csv.ToString());
        File.WriteAllText(Path.Combine(outDir, "camlook-" + tag + "-summary.json"), jsonText);
        Debug.Log("CAMLOOK_DONE tag=" + tag + " frames=" + frames.Count);
    }

    private static string SafeRead(string path) { try { return File.ReadAllText(path).Trim(); } catch { return "n/a"; } }

    private void Finish(int code)
    {
        gcRecorder.Dispose();
        Debug.Log("CAMLOOK_DONE_MARKER");
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(code);
    }
}
#endif
