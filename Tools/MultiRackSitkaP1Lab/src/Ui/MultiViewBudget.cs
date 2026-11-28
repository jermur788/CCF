using System.Globalization;

namespace SitkaP1.Ui;

/// <summary>Active byte budgets for <c>/multi</c> JSON view and lane-filtered stand payloads.</summary>
internal readonly record struct MultiViewBudgetLimit(int MaxViewBytes, int MaxEcologyBytes, int HardMaxViewBytes);

/// <summary>
/// The soft view target is 512 KiB for up to two areas. For larger stands it scales with genesis stems
/// (<c>524,288 × ceil(stems / 300)</c>) up to 2.5 MiB. Whole-stand and lane-filtered reads use the same budgets.
/// The defensive HTTP 503 ceiling is at least 4 MiB and is at least as large as the soft target without a test override.
/// </summary>
internal static class MultiViewBudget
{
    internal const int DefaultViewBytes = 524_288;      // 512 KiB soft target and per-300-stem scaling unit
    internal const int DefaultEcologyBytes = 153_600;   // 150 KiB
    internal const int CeilingViewBytes = 2_621_440;    // 2.5 MiB maximum scaled soft target
    internal const int CeilingEcologyBytes = 768_000;   // 750 KiB proportional ecology ceiling
    internal const int DefensiveHardCeilingBytes = 4_194_304;
    internal const int StemsPerBudgetUnit = 300;
    internal const string TestOverrideEnvVar = "CCF_TEST_MAX_VIEW_BYTES";

    internal static MultiViewBudgetLimit ForStand(int areaCount, int genesisStems, int? testMaxViewBytesOverride = null)
    {
        int softViewBytes = DefaultViewBytes;
        int ecologyBytes = DefaultEcologyBytes;
        if (areaCount >= 3)
        {
            int safeStems = Math.Max(1, genesisStems);
            int units = Math.Max(1, (safeStems + StemsPerBudgetUnit - 1) / StemsPerBudgetUnit);
            softViewBytes = (int)Math.Min((long)CeilingViewBytes, (long)DefaultViewBytes * units);
            ecologyBytes = (int)Math.Min((long)CeilingEcologyBytes, (long)DefaultEcologyBytes * units);
        }

        int hardViewBytes = Math.Max(softViewBytes, DefensiveHardCeilingBytes);
        // Test overrides use the legacy strict limit, including when the override exceeds the computed target.
        if (testMaxViewBytesOverride is > 0)
        {
            softViewBytes = Math.Min(softViewBytes, testMaxViewBytesOverride.Value);
            ecologyBytes = Math.Min(ecologyBytes, testMaxViewBytesOverride.Value);
            hardViewBytes = softViewBytes;
        }

        return new MultiViewBudgetLimit(softViewBytes, ecologyBytes, hardViewBytes);
    }

    internal static MultiViewBudgetLimit ForLaneFiltered(int totalAreaCount, int genesisStems, int? testMaxViewBytesOverride = null) =>
        ForStand(totalAreaCount, genesisStems, testMaxViewBytesOverride);

    internal static int? ReadTestOverrideFromEnvironment()
    {
        string? raw = Environment.GetEnvironmentVariable(TestOverrideEnvVar);
        if (string.IsNullOrWhiteSpace(raw)) return null;
        return int.TryParse(raw.Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out int parsed) && parsed > 0
            ? parsed
            : null;
    }
}
