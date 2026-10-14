using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Profiling;
using UnityEditorInternal;
using UnityEngine;

// Disposable Editor helper: reads a binary Unity Profiler capture written by the Development Player (the probe's "profiler" step) and writes two
// plain CSV files next to it, so main-thread time can be attributed to individual script callbacks without opening the Profiler window.
//
//   CCF_RAW      path of the .raw capture (the probe also writes <name>.raw.data files beside it; give the .raw path)
//   CCF_RAW_OUT  path prefix for the outputs
//
// <prefix>-self.csv   per thread and marker: self time summed over the captured frames, calls, GC bytes (flat, inverted hierarchy)
// <prefix>-paths.csv  main thread only: inclusive time by call path (depth <= 7), summed over frames, for the heaviest paths
public static class RawProfileAnalyzer
{
    private static string F(float v) => v.ToString("0.####", CultureInfo.InvariantCulture);

    public static void Run()
    {
        string raw = Environment.GetEnvironmentVariable("CCF_RAW");
        string prefix = Environment.GetEnvironmentVariable("CCF_RAW_OUT");
        try
        {
            ProfilerDriver.LoadProfile(raw, false);
            int first = ProfilerDriver.firstFrameIndex, last = ProfilerDriver.lastFrameIndex;
            Debug.Log($"RAW_PROFILE frames {first}..{last}");
            var self = new Dictionary<string, (double ms, int calls, long gc)>();
            var paths = new Dictionary<string, (double ms, int frames)>();
            var threadNames = new SortedSet<string>();
            int frameCount = 0;
            for (int frame = first; frame <= last; frame++)
            {
                frameCount++;
                for (int thread = 0; thread < 24; thread++)
                {
                    using (HierarchyFrameDataView flat = ProfilerDriver.GetHierarchyFrameDataView(frame, thread,
                        HierarchyFrameDataView.ViewModes.MergeSamplesWithTheSameName | HierarchyFrameDataView.ViewModes.InvertHierarchy, HierarchyFrameDataView.columnSelfTime, false))
                    {
                        if (flat == null || !flat.valid) break;
                        string threadName = thread + ":" + flat.threadName;
                        threadNames.Add(threadName);
                        var children = new List<int>();
                        flat.GetItemChildren(flat.GetRootItemID(), children);
                        foreach (int id in children)
                        {
                            string key = threadName + "|" + flat.GetItemName(id);
                            float ms = flat.GetItemColumnDataAsFloat(id, HierarchyFrameDataView.columnSelfTime);
                            int calls = (int)flat.GetItemColumnDataAsFloat(id, HierarchyFrameDataView.columnCalls);
                            long gc = (long)flat.GetItemColumnDataAsFloat(id, HierarchyFrameDataView.columnGcMemory);
                            self.TryGetValue(key, out var old);
                            self[key] = (old.ms + ms, old.calls + calls, old.gc + gc);
                        }
                    }
                    if (thread != 0) continue;
                    using (HierarchyFrameDataView tree = ProfilerDriver.GetHierarchyFrameDataView(frame, 0,
                        HierarchyFrameDataView.ViewModes.MergeSamplesWithTheSameName, HierarchyFrameDataView.columnTotalTime, false))
                    {
                        if (tree == null || !tree.valid) continue;
                        Walk(tree, tree.GetRootItemID(), "", 0, paths);
                    }
                }
            }
            Directory.CreateDirectory(Path.GetDirectoryName(prefix));
            File.WriteAllLines(prefix + "-self.csv", new[] { "thread,marker,self_ms_total,self_ms_per_frame,calls_per_frame,gc_bytes_per_frame" }.Concat(
                self.OrderByDescending(kv => kv.Value.ms).Take(400).Select(kv =>
                {
                    int bar = kv.Key.IndexOf('|');
                    return $"\"{kv.Key.Substring(0, bar)}\",\"{kv.Key.Substring(bar + 1).Replace("\"", "'")}\",{F((float)kv.Value.ms)},{F((float)(kv.Value.ms / frameCount))},{F((float)kv.Value.calls / frameCount)},{F((float)kv.Value.gc / frameCount)}";
                })));
            File.WriteAllLines(prefix + "-paths.csv", new[] { "path,total_ms_per_frame,frames_seen" }.Concat(
                paths.OrderByDescending(kv => kv.Value.ms).Take(300).Select(kv => $"\"{kv.Key.Replace("\"", "'")}\",{F((float)(kv.Value.ms / frameCount))},{kv.Value.frames}")));
            File.WriteAllText(prefix + "-meta.txt", $"frames={frameCount} threads={string.Join(";", threadNames)}\n");
            Debug.Log("RAW_PROFILE_DONE " + prefix);
        }
        catch (Exception e)
        {
            Debug.LogError("RAW_PROFILE_FAIL " + e);
            EditorApplication.Exit(1);
            return;
        }
        EditorApplication.Exit(0);
    }

    private static void Walk(HierarchyFrameDataView view, int id, string path, int depth, Dictionary<string, (double ms, int frames)> paths)
    {
        var children = new List<int>();
        view.GetItemChildren(id, children);
        foreach (int child in children)
        {
            string name = view.GetItemName(child);
            float total = view.GetItemColumnDataAsFloat(child, HierarchyFrameDataView.columnTotalTime);
            string full = path.Length == 0 ? name : path + " > " + name;
            if (total >= 0.02f)
            {
                paths.TryGetValue(full, out var old);
                paths[full] = (old.ms + total, old.frames + 1);
            }
            if (depth < 7 && total >= 0.02f) Walk(view, child, full, depth + 1, paths);
        }
    }
}
