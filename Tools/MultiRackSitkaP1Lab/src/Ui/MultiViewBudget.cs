using System.Globalization;

namespace SitkaP1.Ui;

/// <summary>Active byte budgets for <c>/multi</c> JSON view and lane-filtered stand payloads.</summary>
internal readonly record struct MultiViewBudgetLimit(int MaxViewBytes, int MaxEcologyBytes);

/// <summary>
/// Scaled payload budget formula (spec §8.1 / T-2A2-07).
/// Two-area stands (<c>MA-1</c>, <c>MA-1-MIXED</c>) keep the exact unchanged 512 KiB / 150 KiB caps.
/// Stands with 3 or more areas scale linearly with genesis stems up to the 2.5 MiB ceiling, while
/// 1–3 lane filtered reads under <c>/api/multi/stand?lanes=...</c> remain strictly bounded to 512 KiB.
/// </summary>
internal static class MultiViewBudget
{
    internal const int DefaultViewBytes = 524_288;      // 512 KiB (frozen MA-1 / MA-1-MIXED budget & <=3-lane stand budget)
    internal const int DefaultEcologyBytes = 153_600;   // 150 KiB
    internal const int CeilingViewBytes = 2_621_440;    // 2.5 MiB hard ceiling for >=3-area whole-stand views
    internal const int CeilingEcologyBytes = 768_000;   // 750 KiB proportional ecology ceiling
    internal const int StemsPerBudgetUnit = 300;
    internal const string TestOverrideEnvVar = "CCF_TEST_MAX_VIEW_BYTES";

    internal static MultiViewBudgetLimit ForStand(int areaCount, int genesisStems, int? testMaxViewBytesOverride = null)
    {
        MultiViewBudgetLimit computed;
        if (areaCount <= 2)
        {
            computed = new MultiViewBudgetLimit(DefaultViewBytes, DefaultEcologyBytes);
        }
        else
        {
            int safeStems = Math.Max(1, genesisStems);
            int units = Math.Max(1, (safeStems + StemsPerBudgetUnit - 1) / StemsPerBudgetUnit);
            int viewCap = (int)Math.Min((long)CeilingViewBytes, (long)DefaultViewBytes * units);
            int ecoCap = (int)Math.Min((long)CeilingEcologyBytes, (long)DefaultEcologyBytes * units);
            computed = new MultiViewBudgetLimit(viewCap, ecoCap);
        }

        // Spec §8: test budget override may only LOWER the active budget, never raise it.
        if (testMaxViewBytesOverride is > 0)
        {
            return new MultiViewBudgetLimit(
                Math.Min(computed.MaxViewBytes, testMaxViewBytesOverride.Value),
                Math.Min(computed.MaxEcologyBytes, testMaxViewBytesOverride.Value));
        }

        return computed;
    }

    internal static MultiViewBudgetLimit ForLaneFiltered(int requestedLaneCount, int totalAreaCount, int genesisStems, int? testMaxViewBytesOverride = null)
    {
        var computed = requestedLaneCount <= 3
            ? new MultiViewBudgetLimit(DefaultViewBytes, DefaultEcologyBytes)
            : ForStand(totalAreaCount, genesisStems);

        // Spec §8: test budget override may only LOWER the active budget, never raise it.
        if (testMaxViewBytesOverride is > 0)
        {
            return new MultiViewBudgetLimit(
                Math.Min(computed.MaxViewBytes, testMaxViewBytesOverride.Value),
                Math.Min(computed.MaxEcologyBytes, testMaxViewBytesOverride.Value));
        }

        return computed;
    }

    internal static int? ReadTestOverrideFromEnvironment()
    {
        string? raw = Environment.GetEnvironmentVariable(TestOverrideEnvVar);
        if (string.IsNullOrWhiteSpace(raw)) return null;
        return int.TryParse(raw.Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out int parsed) && parsed > 0
            ? parsed
            : null;
    }
}
