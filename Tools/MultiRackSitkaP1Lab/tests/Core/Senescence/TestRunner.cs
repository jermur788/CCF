using System.Globalization;

namespace SitkaP1.Core.Senescence.Tests;

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

    /// <summary>Compares the exact IEEE-754 bit patterns, so 0.20 must be exactly the double nearest 0.20.</summary>
    public static void BitEq(double expected, double actual, string message)
    {
        if (BitConverter.DoubleToInt64Bits(expected) != BitConverter.DoubleToInt64Bits(actual))
            throw new InvalidOperationException(string.Create(CultureInfo.InvariantCulture,
                $"Assertion failed ({message}): expected bits of {expected:R}, got {actual:R}."));
    }

    /// <summary>Null must match null; otherwise both values must carry identical IEEE-754 bits.</summary>
    public static void SameBits(double? expected, double? actual, string message)
    {
        if (expected.HasValue != actual.HasValue)
            throw new InvalidOperationException(string.Create(CultureInfo.InvariantCulture,
                $"Assertion failed ({message}): expected {(expected.HasValue ? expected.Value.ToString("R", CultureInfo.InvariantCulture) : "null")}, got {(actual.HasValue ? actual.Value.ToString("R", CultureInfo.InvariantCulture) : "null")}."));
        if (expected is { } e && actual is { } a)
            BitEq(e, a, message);
    }

    /// <summary>Asserts a nullable increment is present and returns it.</summary>
    public static double Val(double? value, string message)
    {
        if (value is not { } v)
            throw new InvalidOperationException($"Assertion failed ({message}): expected a value, got null.");
        return v;
    }

    public static void ApproxEq(double expected, double actual, double tolerance, string message)
    {
        if (!double.IsFinite(actual) || Math.Abs(expected - actual) > tolerance)
            throw new InvalidOperationException(string.Create(CultureInfo.InvariantCulture,
                $"Assertion failed ({message}): expected {expected:R} ± {tolerance:R}, got {actual:R}."));
    }

    /// <summary>
    /// Requires exactly <typeparamref name="TException"/>, not a subclass: ArgumentOutOfRangeException and
    /// ArgumentNullException are both ArgumentExceptions, and the spec names the exact type for each refusal.
    /// </summary>
    public static void Throws<TException>(Action action, string message) where TException : Exception
    {
        try
        {
            action();
        }
        catch (Exception ex) when (ex.GetType() == typeof(TException))
        {
            return;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Assertion failed ({message}): expected {typeof(TException).Name}, got {ex.GetType().Name}: {ex.Message}");
        }
        throw new InvalidOperationException($"Assertion failed ({message}): expected {typeof(TException).Name}, nothing was thrown.");
    }

    public static void NoThrow(Action action, string message)
    {
        try
        {
            action();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Assertion failed ({message}): unexpected {ex.GetType().Name}: {ex.Message}");
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
            Console.WriteLine($"SUITE {kv.Key}: {kv.Value.Passed}/{kv.Value.Passed + kv.Value.Failed} passed, {kv.Value.Failed} failed");
        Console.WriteLine($"TOTAL: {totalPassed}/{totalPassed + totalFailed} passed, {totalFailed} failed");

        return totalFailed == 0 && totalPassed > 0 ? 0 : 1;
    }
}
