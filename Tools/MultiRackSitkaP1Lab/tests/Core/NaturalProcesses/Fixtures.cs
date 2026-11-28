using System.Globalization;

namespace SitkaP1.Core.NaturalProcesses.Tests;

/// <summary>Shared deterministic fixtures. The 80 m × 80 m woodland is 0.64 ha (caps 6 on-map, 4 off-map).</summary>
internal static class Fixtures
{
    public const int Seed = 12345;

    public static WoodlandFootprint Woodland064() => WoodlandFootprint.Rectangle(0, 0, 80, 80);

    public static readonly OakMastYearOutcome Mast = new(true, "good");
    public static readonly OakMastYearOutcome NoMast = new(false, "normal");

    public static readonly JayAcornDispersalSettings JayOn = new(JayAcornDispersal: true);
    public static JayAcornDispersalSettings JayAndNearby(OuterEdge edge) => new(true, true, edge);

    /// <summary><paramref name="perTree"/> candidates from each of <paramref name="trees"/> oaks placed on a grid.</summary>
    public static List<OnMapOakSeedCandidate> OakCandidates(int trees, int perTree)
    {
        var list = new List<OnMapOakSeedCandidate>();
        for (int t = 0; t < trees; t++)
        {
            string treeId = "OAK-" + t.ToString("D3", CultureInfo.InvariantCulture);
            var position = new LocalPoint(10 + (t % 4) * 20, 10 + (t / 4) * 20);
            for (int c = 0; c < perTree; c++)
                list.Add(new OnMapOakSeedCandidate(treeId + "-C" + c.ToString("D2", CultureInfo.InvariantCulture), treeId, "sessile-oak", position));
        }
        return list;
    }

    public static JayDispersalRequest JayRequest(JayAcornDispersalSettings settings, OakMastYearOutcome mast,
        IReadOnlyList<OnMapOakSeedCandidate> candidates, int year = 7, int seed = Seed) =>
        new(year, seed, settings, mast, Woodland064(), candidates);

    /// <summary>A mixed stand of storm inputs spread over two lanes, with tall exposed dominants and small trees.</summary>
    public static List<StormTreeInput> StormTrees(int count, string? laneOverride = null)
    {
        var list = new List<StormTreeInput>();
        for (int i = 0; i < count; i++)
        {
            float height = 8f + (i % 23);
            float dbh = 12f + (i % 17) * 1.5f;
            float top = 28f;
            float light = (i % 10) / 10f;
            float recent = (i % 5) * 0.5f;
            string lane = laneOverride ?? (i % 2 == 0 ? "LANE-A" : "LANE-B");
            list.Add(new StormTreeInput("T" + i.ToString("D4", CultureInfo.InvariantCulture), lane,
                "C" + (i % 64).ToString("D2", CultureInfo.InvariantCulture), height, dbh, top, light, recent));
        }
        return list;
    }

    /// <summary>Frozen Unity StormModel 1 event years for seed 12345 over years 1..1000 (parity harness output).</summary>
    public static readonly (int Year, float Severity)[] FrozenEvents12345 =
    {
        (28, .06f), (39, .06f), (108, .18f), (194, .06f), (219, .02f), (227, .06f), (229, .18f), (283, .06f), (292, .02f),
        (374, .06f), (519, .02f), (528, .18f), (563, .02f), (679, .06f), (730, .18f), (770, .18f), (825, .06f), (866, .18f),
        (955, .06f),
    };

    /// <summary>
    /// Proves a collection is a sealed read-only snapshot (R1-02, R2-01): it is not an array or List, implements none of
    /// ICollection / ICollection&lt;T&gt; / IList / IList&lt;T&gt; (so there is no SyncRoot, setter, Add or Clear to
    /// reach the storage), every such cast fails, and no public or non-public property named SyncRoot exists anywhere on
    /// its runtime type or interfaces.
    /// </summary>
    public static void AssertReadOnly<T>(IReadOnlyList<T> list, string name)
    {
        TestRunner.True(list is not null, name + " is not null");
        object boxed = list!;
        int before = list!.Count;
        var snapshot = list.ToArray();
        TestRunner.True(boxed is not T[], name + " is not a raw array");
        TestRunner.True(boxed is not Array, name + " is not any array");
        TestRunner.True(boxed is not List<T>, name + " is not a List");
        TestRunner.True(boxed is not System.Collections.ICollection, name + " is not ICollection");
        TestRunner.True(boxed is not ICollection<T>, name + " is not ICollection<T>");
        TestRunner.True(boxed is not System.Collections.IList, name + " is not IList");
        TestRunner.True(boxed is not IList<T>, name + " is not IList<T>");
        TestRunner.True(boxed is not System.Collections.ObjectModel.ReadOnlyCollection<T>, name + " is not ReadOnlyCollection<T>");
        Throws<InvalidCastException>(() => _ = (System.Collections.ICollection)boxed, name + " cast to ICollection");
        Throws<InvalidCastException>(() => _ = (ICollection<T>)boxed, name + " cast to ICollection<T>");
        Throws<InvalidCastException>(() => _ = (System.Collections.IList)boxed, name + " cast to IList");
        Throws<InvalidCastException>(() => _ = (IList<T>)boxed, name + " cast to IList<T>");
        Throws<InvalidCastException>(() => _ = (T[])boxed, name + " cast to T[]");

        const System.Reflection.BindingFlags all = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static;
        Type type = boxed.GetType();
        var types = new[] { type }.Concat(type.GetInterfaces()).ToArray();
        TestRunner.True(types.All(t => t.GetProperties(all).All(p => !p.Name.EndsWith("SyncRoot", StringComparison.Ordinal))),
            name + " exposes no SyncRoot member (reflection)");
        TestRunner.True(type.IsSealed && !type.IsValueType, name + " is a sealed reference type");
        TestRunner.True(types.All(t => t.GetProperties(all).All(p => p.SetMethod is null)), name + " has no property setter (incl. indexer)");

        TestRunner.Eq(before, list.Count, name + " count unchanged");
        TestRunner.True(snapshot.SequenceEqual(list), name + " elements unchanged");
    }

    public static void Throws<TException>(Action action, string message) where TException : Exception
    {
        try { action(); }
        catch (TException) { return; }
        throw new InvalidOperationException("Assertion failed: expected " + typeof(TException).Name + " from " + message);
    }

    public static List<T> Shuffled<T>(IEnumerable<T> items, int shuffleSeed)
    {
        var list = items.ToList();
        var random = new Random(shuffleSeed); // test-only shuffle; production code never uses System.Random
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
        return list;
    }

    public static string Describe(JayDispersalResult r) => string.Join("|",
        new[] { r.Ran.ToString(), r.OnMapCap.ToString(CultureInfo.InvariantCulture), r.OffMapCap.ToString(CultureInfo.InvariantCulture),
            string.Join(",", r.DivertedOnMapCandidateIds), r.RngDrawCount.ToString(CultureInfo.InvariantCulture) }
        .Concat(r.Delivered.Concat(r.Exported).Select(c => string.Create(CultureInfo.InvariantCulture,
            $"{c.CandidateId}:{c.Fate}:{c.Destination.X:R},{c.Destination.Y:R}:{c.DistanceM:R}"))));

    public static string Describe(StormResult r) => string.Join("|",
        new[] { r.Model.ToString(), r.Event is null ? "calm" : string.Create(CultureInfo.InvariantCulture,
            $"{r.Event.Year}:{r.Event.SeverityLevel}:{r.Event.DirectionDegrees:R}:{r.Event.FallGlyphOrientationDegrees:R}"),
            r.RngDrawCount.ToString(CultureInfo.InvariantCulture) }
        .Concat(r.Victims.Select(v => v.TreeId + ":" + v.Deadwood.DeadwoodId + ":" + v.Opening.CellId))
        .Concat(r.Exposure.Select(e => e.TreeId + ":" + e.Label)));

    /// <summary>
    /// A hostile caller collection (R3-01). It implements both <see cref="IReadOnlyList{T}"/> and
    /// <see cref="ICollection{T}"/>; its <see cref="CopyTo"/> copies correctly but keeps every destination array it is
    /// handed, so a test can overwrite those arrays after construction. LINQ <c>ToArray()</c> and
    /// <c>new List&lt;T&gt;(source)</c> both call CopyTo on such a source.
    /// </summary>
    public sealed class RetainingCollection<T> : IReadOnlyList<T>, ICollection<T>
    {
        private readonly List<T> _items;
        private readonly List<T[]> _captured = new();

        public RetainingCollection(IEnumerable<T> items) => _items = items.ToList();

        public int CopyToCalls { get; private set; }
        public int Count => _items.Count;
        public bool IsReadOnly => true;
        public T this[int index] => _items[index];

        public void CopyTo(T[] array, int arrayIndex)
        {
            CopyToCalls++;
            _items.CopyTo(array, arrayIndex);
            _captured.Add(array);
        }

        /// <summary>Overwrites every element of every retained destination with <paramref name="replacement"/>.</summary>
        public void OverwriteCaptured(T replacement)
        {
            foreach (T[] array in _captured)
                for (int i = 0; i < array.Length; i++) array[i] = replacement;
        }

        /// <summary>Overwrites one index of every retained destination long enough to hold it.</summary>
        public void OverwriteCaptured(int index, T replacement)
        {
            foreach (T[] array in _captured)
                if (index < array.Length) array[index] = replacement;
        }

        public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
        public bool Contains(T item) => _items.Contains(item);
        public void Add(T item) => throw new NotSupportedException();
        public void Clear() => throw new NotSupportedException();
        public bool Remove(T item) => throw new NotSupportedException();
    }
}
