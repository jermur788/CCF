using System.Text;
using SitkaP1.Core.NaturalProcesses.Senescence;
using SitkaP1.Core.SpeciesWave2;

namespace SitkaP1.Core.Senescence.Tests;

/// <summary>Shared deterministic fixtures. Nothing here uses System.Random, the clock or machine state.</summary>
internal static class Fixtures
{
    public const int Seed = 20261128;

    public const string Alder = SpeciesMoistureTable.CommonAlder;
    public const string Birch = SpeciesMoistureTable.DownyBirch;
    public const string Sitka = SpeciesMoistureTable.SitkaSpruce;
    public const string Beech = SpeciesMoistureTable.EuropeanBeech;
    public const string Pine = SpeciesMoistureTable.ScotsPine;
    public const string Oak = SpeciesMoistureTable.SessileOak;

    /// <summary>The six species in ascending onset order.</summary>
    public static readonly string[] SixSpecies = { Alder, Birch, Sitka, Beech, Pine, Oak };

    /// <summary>
    /// <paramref name="count"/> trees cycling through all six species with ages spread over 0..300 (about a third of
    /// them are 200 or older). IDs are T0000, T0001, ... so the list is already in ordinal ID order.
    /// </summary>
    public static List<SenescenceTreeInput> MixedPopulation(int count)
    {
        var trees = new List<SenescenceTreeInput>(count);
        for (int i = 0; i < count; i++)
            trees.Add(new SenescenceTreeInput($"T{i:D4}", SixSpecies[i % 6], (i * 37) % 301));
        return trees;
    }

    /// <summary>An even-aged cohort of one species.</summary>
    public static List<SenescenceTreeInput> Cohort(string speciesId, int ageYears, int count, string idPrefix)
    {
        var trees = new List<SenescenceTreeInput>(count);
        for (int i = 0; i < count; i++)
            trees.Add(new SenescenceTreeInput($"{idPrefix}{i:D4}", speciesId, ageYears));
        return trees;
    }

    /// <summary>Fisher-Yates shuffle driven by a SplitMix64 counter, so the permutation is fixed for a given seed.</summary>
    public static List<T> DeterministicShuffle<T>(IReadOnlyList<T> items, ulong seed)
    {
        var copy = new List<T>(items);
        ulong state = seed;
        for (int i = copy.Count - 1; i > 0; i--)
        {
            state = unchecked(state + 0x9E3779B97F4A7C15UL);
            ulong z = state;
            z = unchecked((z ^ (z >> 30)) * 0xBF58476D1CE4E5B9UL);
            z = unchecked((z ^ (z >> 27)) * 0x94D049BB133111EBUL);
            z ^= z >> 31;
            int j = (int)(z % (ulong)(i + 1));
            (copy[i], copy[j]) = (copy[j], copy[i]);
        }
        return copy;
    }

    public static SenescenceResult Run(int year, IReadOnlyList<SenescenceTreeInput> trees, int seed = Seed) =>
        SenescenceEngine.Evaluate(new SenescenceRequest(year, seed, SenescenceModelVersion.OldAgeV1, trees));

    public static string DeathIds(SenescenceResult result) => string.Join(",", result.Deaths.Select(d => d.TreeId));

    /// <summary>Every observable field of a result as one line, for exact before/after comparisons.</summary>
    public static string Describe(SenescenceResult result)
    {
        var text = new StringBuilder();
        text.Append(result.Model).Append('|').Append(result.RngDrawCount).Append('|').Append(result.TreesEvaluated).Append('|');
        foreach (SenescenceDeathRecord d in result.Deaths)
            text.Append(d.TreeId).Append('/').Append(d.SpeciesId).Append('/').Append(d.Year).Append(';');
        text.Append('|');
        foreach (SenescenceHazardEvaluation e in result.HazardEvaluations)
            text.Append(e.TreeId).Append('/').Append(e.AgeYears).Append('/')
                .Append(BitConverter.DoubleToInt64Bits(e.AnnualHazard).ToString("X16", System.Globalization.CultureInfo.InvariantCulture))
                .Append(';');
        return text.ToString();
    }

    /// <summary>Walks up from the executable to find a repository directory such as src/Core/NaturalProcesses/Senescence.</summary>
    public static string? FindDir(params string[] parts)
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            string candidate = Path.Combine(new[] { dir.FullName }.Concat(parts).ToArray());
            if (Directory.Exists(candidate))
                return candidate;
            dir = dir.Parent;
        }
        return null;
    }

    /// <summary>
    /// A hostile caller collection: its CopyTo keeps a reference to the destination array so the caller can overwrite a
    /// snapshot that was taken with <c>ToArray()</c> or <c>new List&lt;T&gt;(source)</c>. Mirrors the NaturalProcesses
    /// fixture of the same name.
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
                for (int i = 0; i < array.Length; i++)
                    array[i] = replacement;
        }

        public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
        public bool Contains(T item) => _items.Contains(item);
        public void Add(T item) => throw new NotSupportedException();
        public void Clear() => throw new NotSupportedException();
        public bool Remove(T item) => throw new NotSupportedException();
    }
}
