using System.Diagnostics;
using System.Reflection;

namespace SitkaLab.Testing;

[AttributeUsage(AttributeTargets.Method)]
public sealed class TestAttribute : Attribute { }

public sealed class TestFailure : Exception { public TestFailure(string m) : base(m) { } }

/// <summary>Dependency-free test kit (no NuGet packages): discovers [Test] static methods, prints PASS/FAIL, returns a process exit code.</summary>
public static class T
{
    public static void True(bool cond, string msg) { if (!cond) throw new TestFailure(msg); }
    public static void False(bool cond, string msg) { if (cond) throw new TestFailure(msg); }

    public static void Eq<V>(V expected, V actual, string msg)
    {
        if (!EqualityComparer<V>.Default.Equals(expected, actual)) throw new TestFailure($"{msg}: expected <{expected}> but was <{actual}>");
    }

    public static void Near(double expected, double actual, double tol, string msg)
    {
        if (double.IsNaN(actual) || Math.Abs(expected - actual) > tol) throw new TestFailure($"{msg}: expected {expected:R} ± {tol:R} but was {actual:R}");
    }

    public static void Seq<V>(IEnumerable<V> expected, IEnumerable<V> actual, string msg)
    {
        var e = expected.ToList(); var a = actual.ToList();
        if (!e.SequenceEqual(a)) throw new TestFailure($"{msg}: expected [{string.Join(",", e)}] but was [{string.Join(",", a)}]");
    }

    public static TEx Throws<TEx>(Action a, string msg) where TEx : Exception
    {
        try { a(); }
        catch (TEx ex) { return ex; }
        catch (Exception ex) { throw new TestFailure($"{msg}: expected {typeof(TEx).Name} but got {ex.GetType().Name}: {ex.Message}"); }
        throw new TestFailure($"{msg}: expected {typeof(TEx).Name} but nothing was thrown");
    }

    public static int RunAll(Assembly assembly, string[] args)
    {
        string? filter = args.FirstOrDefault(a => !a.StartsWith("--"));
        var tests = assembly.GetTypes()
            .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic))
            .Where(m => m.GetCustomAttribute<TestAttribute>() is not null)
            .OrderBy(m => m.DeclaringType!.FullName, StringComparer.Ordinal).ThenBy(m => m.Name, StringComparer.Ordinal)
            .ToList();
        int pass = 0, fail = 0;
        var failures = new List<string>();
        var total = Stopwatch.StartNew();
        foreach (var m in tests)
        {
            string name = $"{m.DeclaringType!.Name}.{m.Name}";
            if (filter is not null && !name.Contains(filter, StringComparison.OrdinalIgnoreCase)) continue;
            var sw = Stopwatch.StartNew();
            try
            {
                m.Invoke(null, null);
                pass++;
                Console.WriteLine($"PASS  {name}  ({sw.ElapsedMilliseconds} ms)");
            }
            catch (Exception ex)
            {
                var inner = ex is TargetInvocationException tie && tie.InnerException is not null ? tie.InnerException : ex;
                fail++;
                string line = $"FAIL  {name}: {inner.GetType().Name}: {inner.Message}";
                Console.WriteLine(line);
                failures.Add(line);
            }
        }
        Console.WriteLine($"---- {assembly.GetName().Name}: {pass} passed, {fail} failed, {total.ElapsedMilliseconds} ms ----");
        foreach (var f in failures) Console.WriteLine(f);
        return fail == 0 && pass > 0 ? 0 : 1;
    }
}

public static class ToolPaths
{
    public static string ToolRoot { get; } = Find();
    public static string FixtureDir => Path.Combine(ToolRoot, "fixtures", "synthetic");

    private static string Find()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            if (File.Exists(Path.Combine(dir.FullName, "MultiRackSitkaLab.sln")) || File.Exists(Path.Combine(dir.FullName, "MultiRackSitkaLab.slnx"))) return dir.FullName;
            dir = dir.Parent;
        }
        throw new InvalidOperationException("MultiRackSitkaLab tool root not found above " + AppContext.BaseDirectory);
    }
}
