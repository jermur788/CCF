using SitkaP1.Core;

namespace SitkaP1.EconomyDispositions.Tests;

/// <summary>Minimal dependency-free test runner (same shape as tests/Core.Tests/Runner.cs).</summary>
internal static class T
{
    private static readonly List<(string Name, Action Body)> Tests = new();

    public static void Add(string name, Action body) => Tests.Add((name, body));

    public static void Refuses(string code, Action a)
    {
        try { a(); }
        catch (P1Refusal e)
        {
            if (e.Code == code) return;
            throw new Exception($"expected refusal {code}, got {e.Code}: {e.Message}");
        }
        throw new Exception($"expected refusal {code}, but the call succeeded");
    }

    public static void True(bool ok, string what) { if (!ok) throw new Exception("assertion failed: " + what); }

    public static void Eq<TV>(TV expected, TV actual, string what)
    {
        if (!EqualityComparer<TV>.Default.Equals(expected, actual)) throw new Exception($"{what}: expected {expected}, got {actual}");
    }

    public static int RunAll()
    {
        int failed = 0;
        foreach (var (name, body) in Tests)
        {
            try { body(); Console.WriteLine("PASS " + name); }
            catch (Exception e) { failed++; Console.WriteLine("FAIL " + name + " — " + e.Message); }
        }
        Console.WriteLine($"{Tests.Count - failed}/{Tests.Count} passed, {failed} failed");
        return failed == 0 ? 0 : 1;
    }
}
