using System.Globalization;

namespace SitkaP1.Core.CustomWoodlandBuilder.Tests;

/// <summary>Minimal console test runner: prints one line per case and returns 1 when any case fails.</summary>
internal static class TestRunner
{
    private sealed record TestCase(string Suite, string Name, Action Body);

    private static readonly List<TestCase> Cases = new();

    public static void Add(string suite, string name, Action body) => Cases.Add(new TestCase(suite, name, body));

    public static void True(bool condition, string message)
    {
        if (!condition)
            throw new InvalidOperationException("Assertion failed: " + message);
    }

    public static void Eq<T>(T expected, T actual, string message)
    {
        if (!EqualityComparer<T>.Default.Equals(expected, actual))
            throw new InvalidOperationException($"Assertion failed ({message}): expected '{expected}', got '{actual}'.");
    }

    /// <summary>Compares the exact IEEE-754 bit patterns, so 1.0 must be exactly 1.0.</summary>
    public static void BitEq(double expected, double actual, string message)
    {
        if (BitConverter.DoubleToInt64Bits(expected) != BitConverter.DoubleToInt64Bits(actual))
            throw new InvalidOperationException(string.Create(CultureInfo.InvariantCulture,
                $"Assertion failed ({message}): expected bits of {expected:R}, got {actual:R}."));
    }

    public static void ApproxEq(double expected, double actual, double tolerance, string message)
    {
        if (!double.IsFinite(actual) || Math.Abs(expected - actual) > tolerance)
            throw new InvalidOperationException(string.Create(CultureInfo.InvariantCulture,
                $"Assertion failed ({message}): expected {expected} ± {tolerance}, got {actual}."));
    }

    public static void Throws<TException>(Action action, string message) where TException : Exception
    {
        try
        {
            action();
        }
        catch (TException)
        {
            return;
        }
        catch (Exception exception)
        {
            throw new InvalidOperationException(
                $"Assertion failed ({message}): expected {typeof(TException).Name}, got {exception.GetType().Name}: {exception.Message}");
        }

        throw new InvalidOperationException($"Assertion failed ({message}): expected {typeof(TException).Name}.");
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
            Console.WriteLine($"SUITE {kv.Key}: {kv.Value.Passed}/{kv.Value.Passed + kv.Value.Failed} passed, {kv.Value.Failed} failed");
        Console.WriteLine($"TOTAL: {totalPassed}/{totalPassed + totalFailed} passed, {totalFailed} failed");

        return totalFailed == 0 && totalPassed > 0 ? 0 : 1;
    }
}
