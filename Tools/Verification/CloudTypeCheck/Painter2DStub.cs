// Compatibility stub for the Unity 6 UI Toolkit vector API used by the UI sources.
// The 2021.3 reference assemblies do not contain Painter2D or MeshGenerationContext.painter2D,
// so without this the compiler stops after those declaration errors and never binds method bodies.
//
// This is an approximation only: nothing here runs, and it is not a substitute for a Unity compile.
// run_typecheck.py compiles it in both modes and rewrites the exact `context.painter2D` access to
// Painter2DCompat.Get(context) in a temporary copy, because the project stays on C# 9 and C# 9 has
// no extension properties. The rewrite touches that one known member access and nothing else.
using UnityEngine;

namespace UnityEngine.UIElements
{
    public enum LineCap { Butt, Round }
    public enum LineJoin { Miter, Round, Bevel }
    public enum ArcDirection { Clockwise, CounterClockwise }

    public class Painter2D
    {
        public Color strokeColor, fillColor;
        public float lineWidth, miterLimit;
        public LineCap lineCap;
        public LineJoin lineJoin;
        public void BeginPath() { }
        public void ClosePath() { }
        public void MoveTo(Vector2 point) { }
        public void LineTo(Vector2 point) { }
        public void Arc(Vector2 centre, float radius, Angle start, Angle end, ArcDirection direction = ArcDirection.Clockwise) { }
        public void Stroke() { }
        public void Fill() { }
    }

    public struct Angle
    {
        public static Angle Radians(float value) => default;
        public static Angle Degrees(float value) => default;
    }

    public static class Painter2DCompat
    {
        public static Painter2D Get(MeshGenerationContext context) => null;
    }
}
