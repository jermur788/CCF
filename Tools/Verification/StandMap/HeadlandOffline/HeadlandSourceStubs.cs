using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityEngine
{
    public class Object
    {
        public bool Destroyed;
        public static bool operator ==(Object first, Object second)
        {
            bool firstNull = ReferenceEquals(first, null) || first.Destroyed;
            bool secondNull = ReferenceEquals(second, null) || second.Destroyed;
            return firstNull || secondNull ? firstNull == secondNull : ReferenceEquals(first, second);
        }
        public static bool operator !=(Object first, Object second) => !(first == second);
        public override bool Equals(object other) => ReferenceEquals(this, other);
        public override int GetHashCode() => base.GetHashCode();
    }

    public class Component : Object
    {
        public GameObject gameObject;
        public Transform transform => gameObject.transform;
        public string name => gameObject.name;
        public T GetComponent<T>() where T : Component => gameObject.Components.OfType<T>().FirstOrDefault(value => value != null);
        public T[] GetComponents<T>() where T : Component
        {
            gameObject.AttachedLookups++;
            gameObject.LookupTimes.Add(Time.unscaledTime);
            return gameObject.Components.OfType<T>().Where(value => value != null).ToArray();
        }
    }

    public class GameObject : Object
    {
        public readonly List<Component> Components = new List<Component>();
        public readonly Transform transform;
        public string name;
        public bool activeSelf = true;
        public bool activeInHierarchy => activeSelf && (transform.Parent == null || transform.Parent.gameObject.activeInHierarchy);
        public Scene scene = new Scene { handle = new SceneHandle(1) };
        public int AttachedLookups;
        public readonly List<float> LookupTimes = new List<float>();

        public GameObject(string name)
        {
            this.name = name;
            transform = Attach(new Transform());
        }

        public T Attach<T>(T component) where T : Component
        {
            component.gameObject = this;
            Components.Add(component);
            return component;
        }
    }

    // Stand-in for UnityEngine.SceneManagement.SceneHandle. Unity 6000.6 deprecates its implicit conversion to int, so the stub
    // has none: source that converts it fails to compile here, as it does against the real DLLs. Equality compares the raw value.
    // (Like the other stubs it lives in the UnityEngine namespace; the real namespace is covered by the real-DLL compile.)
    public readonly struct SceneHandle : System.IEquatable<SceneHandle>
    {
        private readonly ulong raw;
        public SceneHandle(ulong raw) { this.raw = raw; }
        public ulong GetRawData() => raw;
        public bool Equals(SceneHandle other) => raw == other.raw;
        public override bool Equals(object other) => other is SceneHandle handle && Equals(handle);
        public override int GetHashCode() => raw.GetHashCode();
        public static bool operator ==(SceneHandle left, SceneHandle right) => left.Equals(right);
        public static bool operator !=(SceneHandle left, SceneHandle right) => !left.Equals(right);
    }

    public struct Scene { public SceneHandle handle; }

    public class Transform : Component, IEnumerable<Transform>
    {
        public readonly List<Transform> Children = new List<Transform>();
        public Transform Parent;
        public IEnumerator<Transform> GetEnumerator() => Children.Where(child => child != null).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class MeshRenderer : Component { public bool enabled = true; }
    public static class Time { public static float unscaledTime; }
}

public class ForestEcologyController : Component { public int StandGeometryModelVersion = StandGeometryModel.Enlarged80; }
public class ForestStartingStand : Component { }
public class ScenarioOneHeadlandPresentation : Component { public bool isActiveAndEnabled = true; }
public class ScenarioOneManager : Component { public bool RackPlanningV2Active = true; public bool ReferencePreviewActive; }
public static class StandGeometryModel { public const int Enlarged80 = 1; public const int Legacy40 = 0; }
public class ScenarioOneUiRoot { public ScenarioOneManager Manager; public ForestEcologyController Ecology; }

public sealed class HeadlandSourceFixture
{
    public readonly GameObject Root = new GameObject("Starting stand");
    public readonly ScenarioOneUiRoot Ui = new ScenarioOneUiRoot();
    public readonly ForestEcologyController Ecology;
    public readonly ScenarioOneManager Manager;
    public ScenarioOneHeadlandPresentation Presentation;
    public GameObject Surface;
    public MeshRenderer Renderer;
    public readonly HeadlandSourceSeam Source;

    public HeadlandSourceFixture(bool presentation = true, bool surface = true)
    {
        Ecology = Root.Attach(new ForestEcologyController());
        Manager = Root.Attach(new ScenarioOneManager());
        Root.Attach(new ForestStartingStand());
        Ui.Manager = Manager;
        Ui.Ecology = Ecology;
        if (presentation) Presentation = Root.Attach(new ScenarioOneHeadlandPresentation());
        if (presentation && surface) AddSurface();
        Source = new HeadlandSourceSeam(Ui);
    }

    public void AddSurface()
    {
        Surface = new GameObject("Scenario One V2 Headland");
        Surface.transform.Parent = Root.transform;
        Root.transform.Children.Add(Surface.transform);
        Renderer = Surface.Attach(new MeshRenderer());
    }
}
