// Recording stand-ins for the small part of UnityEngine / UI Toolkit that TeachingDiagram.cs uses, so the REAL painter source can be compiled
// and run on plain .NET (issue #296 bounded repair, M-2).
//
// WHAT THIS PROVES: which elements the painter creates (names, classes, texts, the inline styles it sets) and the exact sequence of Painter2D
// commands it issues (paths, points, fill/stroke colours with alpha, line widths and caps), for a given lesson, world flag and frame width.
// WHAT IT DOES NOT PROVE: anything Unity does with them. There is no layout engine, font, text measurement, USS, anti-aliasing, mesh
// tessellation or blending here. Pixel results, wrapping, fit on screen and legibility still need the Unity gates.
//
// Only members the painter actually touches exist; a painter change that needs more fails to compile here, which the runner reports.
using System;
using System.Collections.Generic;
using System.Globalization;

namespace UnityEngine
{
    public struct Vector2
    {
        public float x, y;
        public Vector2(float x, float y) { this.x = x; this.y = y; }
    }

    public struct Rect
    {
        public float x, y, width, height;
        public Rect(float x, float y, float width, float height) { this.x = x; this.y = y; this.width = width; this.height = height; }
    }

    public struct Color
    {
        public float r, g, b, a;
        public Color(float r, float g, float b, float a) { this.r = r; this.g = g; this.b = b; this.a = a; }
        public static implicit operator Color(Color32 c) => new Color(c.r / 255f, c.g / 255f, c.b / 255f, c.a / 255f);
        public string Key() => string.Join(",", new[] { r, g, b, a }.Select255());
    }

    public struct Color32
    {
        public byte r, g, b, a;
        public Color32(byte r, byte g, byte b, byte a) { this.r = r; this.g = g; this.b = b; this.a = a; }
    }

    internal static class ColourText
    {
        public static IEnumerable<string> Select255(this float[] values)
        {
            foreach (float v in values) yield return ((int)Math.Round(v * 255f)).ToString(CultureInfo.InvariantCulture);
        }
    }

    public enum TextAnchor { UpperLeft, UpperCenter, UpperRight, MiddleLeft, MiddleCenter, MiddleRight, LowerLeft, LowerCenter, LowerRight }

    public static class Mathf
    {
        public const float PI = (float)Math.PI;
        public const float Deg2Rad = PI / 180f;
        public static float Max(float a, float b) => a > b ? a : b;
        public static float Min(float a, float b) => a < b ? a : b;
        public static float Abs(float v) => Math.Abs(v);
        public static float Sqrt(float v) => (float)Math.Sqrt(v);
        public static float Cos(float v) => (float)Math.Cos(v);
        public static float Sin(float v) => (float)Math.Sin(v);
        public static float Clamp01(float v) => v < 0f ? 0f : v > 1f ? 1f : v;
        public static int RoundToInt(float v) => (int)Math.Round(v, MidpointRounding.ToEven);
    }
}

namespace UnityEngine.UIElements
{
    public enum Position { Relative, Absolute }
    public enum PickingMode { Position, Ignore }
    public enum StyleKeyword { Undefined, Null, Auto, None, Initial }
    public enum LineCap { Butt, Round }
    public enum LineJoin { Miter, Bevel, Round }

    // Every inline style value is kept as text, keyed by property, so the probe can dump exactly what the painter set.
    public struct StyleValue
    {
        public string Text;
        public static implicit operator StyleValue(float v) => new StyleValue { Text = v.ToString("0.###", CultureInfo.InvariantCulture) };
        public static implicit operator StyleValue(StyleKeyword k) => new StyleValue { Text = "keyword:" + k };
        public static implicit operator StyleValue(Position p) => new StyleValue { Text = "position:" + p };
        public static implicit operator StyleValue(TextAnchor t) => new StyleValue { Text = "anchor:" + t };
        public static implicit operator StyleValue(Color c) => new StyleValue { Text = "rgba:" + c.Key() };
    }

    public sealed class InlineStyle
    {
        public readonly SortedDictionary<string, string> Set = new SortedDictionary<string, string>(StringComparer.Ordinal);
        private StyleValue Put(string key, StyleValue v) { Set[key] = v.Text; return v; }
        private StyleValue Get(string key) => new StyleValue { Text = Set.TryGetValue(key, out string t) ? t : null };
        public StyleValue flexShrink { get => Get("flexShrink"); set => Put("flexShrink", value); }
        public StyleValue maxWidth { get => Get("maxWidth"); set => Put("maxWidth", value); }
        public StyleValue position { get => Get("position"); set => Put("position", value); }
        public StyleValue left { get => Get("left"); set => Put("left", value); }
        public StyleValue top { get => Get("top"); set => Put("top", value); }
        public StyleValue right { get => Get("right"); set => Put("right", value); }
        public StyleValue bottom { get => Get("bottom"); set => Put("bottom", value); }
        public StyleValue width { get => Get("width"); set => Put("width", value); }
        public StyleValue height { get => Get("height"); set => Put("height", value); }
        public StyleValue fontSize { get => Get("fontSize"); set => Put("fontSize", value); }
        public StyleValue color { get => Get("color"); set => Put("color", value); }
        public StyleValue backgroundColor { get => Get("backgroundColor"); set => Put("backgroundColor", value); }
        public StyleValue unityTextAlign { get => Get("unityTextAlign"); set => Put("unityTextAlign", value); }
        public StyleValue marginBottom { get => Get("marginBottom"); set => Put("marginBottom", value); }
        public StyleValue marginTop { get => Get("marginTop"); set => Put("marginTop", value); }
    }

    public delegate void EventCallback<in T>(T evt);

    public class GeometryChangedEvent
    {
        public Rect newRect;
    }

    public class VisualElement
    {
        public string name = "";
        public PickingMode pickingMode;
        public readonly InlineStyle style = new InlineStyle();
        public readonly List<string> Classes = new List<string>();
        public readonly List<VisualElement> Children = new List<VisualElement>();
        public readonly List<Delegate> GeometryCallbacks = new List<Delegate>();
        public Action<MeshGenerationContext> generateVisualContent;
        public Rect contentRect { get; set; }
        public int RepaintRequests;

        public void Add(VisualElement child) => Children.Add(child);
        public void AddToClassList(string className) => Classes.Add(className);
        public bool ClassListContains(string className) => Classes.Contains(className);
        public void MarkDirtyRepaint() => RepaintRequests++;
        public void RegisterCallback<T>(EventCallback<T> callback) where T : class
        {
            if (typeof(T) != typeof(GeometryChangedEvent)) throw new NotSupportedException("probe records only geometry callbacks");
            GeometryCallbacks.Add(callback);
        }
    }

    public class Label : VisualElement
    {
        public string text;
        public Label() { }
        public Label(string text) { this.text = text; }
    }

    public class MeshGenerationContext
    {
        public Painter2D painter2D;
    }

    // Records every command as one line of text. Colours are 0-255 RGBA; coordinates and widths are rounded to 0.001.
    public class Painter2D
    {
        public readonly List<string> Log = new List<string>();
        private LineCap cap;
        private LineJoin join;
        public Color strokeColor, fillColor;
        public float lineWidth;
        public LineCap lineCap { get => cap; set { cap = value; Log.Add("CAP " + value); } }
        public LineJoin lineJoin { get => join; set { join = value; Log.Add("JOIN " + value); } }
        private static string N(float v) => Math.Round(v, 3).ToString("0.###", CultureInfo.InvariantCulture);
        public void BeginPath() => Log.Add("BEGIN");
        public void MoveTo(Vector2 p) => Log.Add("M " + N(p.x) + " " + N(p.y));
        public void LineTo(Vector2 p) => Log.Add("L " + N(p.x) + " " + N(p.y));
        public void ClosePath() => Log.Add("CLOSE");
        public void Fill() => Log.Add("FILL " + fillColor.Key());
        public void Stroke() => Log.Add("STROKE " + strokeColor.Key() + " w=" + N(lineWidth) + " cap=" + cap);
    }
}

// The project's UiKit.Text, reduced to what the painter relies on: a Label carrying the given classes.
public static class UiKit
{
    public static UnityEngine.UIElements.Label Text(string text, params string[] classes)
    {
        var label = new UnityEngine.UIElements.Label(text);
        foreach (string c in classes) label.AddToClassList(c);
        return label;
    }
}
