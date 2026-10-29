#if UNITY_EDITOR
using System;
using System.Collections;
using System.IO;
using UnityEngine;

// Requests OS events from the private-display driver. No InputSystem injection or game-state writes.
public sealed class RackAuthoringNativeInput
{
    [Serializable] public sealed class Op
    {
        public string op, key;
        public int x, y, button;
        public static Op Key(string key) => new Op { op = "key", key = key };
        public static Op Click(int button = 1) => new Op { op = "click", button = button };
        public static Op Move(int x, int y) => new Op { op = "relative", x = x, y = y };
    }
    [Serializable] sealed class Request { public int seq; public Op[] ops; }
    private int sequence;
    public IEnumerator Send(params Op[] ops)
    {
        string folder = Environment.GetEnvironmentVariable("CCF_DRIVER_DIR");
        if (string.IsNullOrEmpty(folder)) throw new InvalidOperationException("OS-input driver directory required");
        int seq = ++sequence;
        string path = Path.Combine(folder, $"request-{seq:D4}.json");
        File.WriteAllText(path + ".tmp", JsonUtility.ToJson(new Request { seq = seq, ops = ops }));
        File.Move(path + ".tmp", path);
        double until = Time.realtimeSinceStartupAsDouble + 30;
        while (!File.Exists(Path.Combine(folder, $"done-{seq:D4}")) && Time.realtimeSinceStartupAsDouble < until)
        {
            string error = Path.Combine(folder, "driver-error");
            if (File.Exists(error)) throw new InvalidOperationException(File.ReadAllText(error));
            yield return null;
        }
        if (!File.Exists(Path.Combine(folder, $"done-{seq:D4}"))) throw new TimeoutException("OS input request " + seq);
        for (int i = 0; i < 12; i++) yield return null;
    }
}
#endif
