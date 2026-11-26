namespace SitkaP1.Core.WoodlandImport.Tests;

internal static class TestRunner
{
    private sealed record TestCase(string Suite, string Name, Action Body);

    private static readonly List<TestCase> Cases = new();

    public static void Add(string suite, string name, Action body) =>
        Cases.Add(new TestCase(suite, name, body));

    public static void True(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException("Assertion failed: " + message);
        }
    }

    public static void Eq<T>(T expected, T actual, string message)
    {
        if (!EqualityComparer<T>.Default.Equals(expected, actual))
        {
            throw new InvalidOperationException($"Assertion failed ({message}): expected '{expected}', got '{actual}'.");
        }
    }

    public static void ApproxEq(double expected, double actual, double tolerance, string message)
    {
        if (!double.IsFinite(actual) || Math.Abs(expected - actual) > tolerance)
        {
            throw new InvalidOperationException(
                string.Create(
                    System.Globalization.CultureInfo.InvariantCulture,
                    $"Assertion failed ({message}): expected {expected} ± {tolerance}, got {actual}."));
        }
    }

    public static int RunAll()
    {
        int totalPassed = 0;
        int totalFailed = 0;
        var suiteCounts = new SortedDictionary<string, (int Passed, int Failed)>(StringComparer.Ordinal);

        foreach (TestCase tc in Cases)
        {
            suiteCounts.TryGetValue(tc.Suite, out var counts);
            try
            {
                tc.Body();
                totalPassed++;
                counts.Passed++;
                Console.WriteLine($"PASS [{tc.Suite}] {tc.Name}");
            }
            catch (Exception ex)
            {
                totalFailed++;
                counts.Failed++;
                Console.WriteLine($"FAIL [{tc.Suite}] {tc.Name}: {ex.Message}");
            }
            suiteCounts[tc.Suite] = counts;
        }

        Console.WriteLine("---");
        foreach (var kv in suiteCounts)
        {
            Console.WriteLine($"SUITE {kv.Key}: {kv.Value.Passed}/{kv.Value.Passed + kv.Value.Failed} passed, {kv.Value.Failed} failed");
        }
        Console.WriteLine($"TOTAL: {totalPassed}/{totalPassed + totalFailed} passed, {totalFailed} failed");

        return totalFailed == 0 ? 0 : 1;
    }
}
